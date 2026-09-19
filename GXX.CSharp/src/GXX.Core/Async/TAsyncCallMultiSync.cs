using System;
using System.Collections.Generic;

// 源单元：Source/M2Engine/AsyncCalls.pas（同源副本：Source/Client-HGE/AsyncCalls.pas、Source/RunGate/AsyncCalls.pas）

namespace GXX.Core.Async;

/// <summary>
/// <c>InternalAsyncMultiSync.InternalWait</c>（原文 1527-1605）中
/// “等待句柄数组 + 下标映射 + 短路”的纯逻辑部分。
/// </summary>
/// <remarks>
/// 原文把 <c>List</c>（IAsyncCall 数组）与 <c>Handles</c>（原生句柄数组）
/// 摊平成一个等待数组，并记录每个等待槽对应的<b>对外下标</b>（<c>Mapping</c>），
/// 最后把 Win32 的 <c>WAIT_OBJECT_0 + slot</c> 反查回“第几个 async call / 第几个 handle”。
/// </remarks>
public sealed class TAsyncMultiSyncPlan
{
    /// <summary>摊平后的等待对象数组（长度 = List.Length + Handles.Length）。</summary>
    public IAsyncWaitObject[] WaitHandles;

    /// <summary>等待槽 → 对外下标（List 优先，其后是 Handles）。</summary>
    public int[] Mapping;

    /// <summary>实际有效的等待对象个数。</summary>
    public int Count;

    /// <summary>
    /// 原文 1553-1559 的短路：<c>WaitAll = False</c> 时若遇到“不是异步调用”的项
    /// （即 <c>Sync</c> 型实现），立即返回该下标。为 -1 表示未短路。
    /// </summary>
    public int ShortCircuitIndex;

    /// <summary>是否发生了短路（此时 <see cref="Count"/> 无意义）。</summary>
    public bool HasShortCircuit => ShortCircuitIndex >= 0;

    /// <summary>
    /// 原文 1537-1568：构造等待计划。
    /// <para>
    /// <b>原文要点</b>：<c>GetEvent</c> 返回 0 的异步调用<b>不</b>占等待槽
    /// （<c>if WaitHandles[Count] &lt;&gt; 0 then</c>），但其下标也不会出现在 Mapping 中；
    /// 抛出的结果只会指向真正占槽的项。
    /// </para>
    /// </summary>
    public static TAsyncMultiSyncPlan Build(IReadOnlyList<IAsyncCall> List, IAsyncWaitObject[] Handles, bool WaitAll)
    {
        List ??= Array.Empty<IAsyncCall>();
        Handles ??= Array.Empty<IAsyncWaitObject>();

        var plan = new TAsyncMultiSyncPlan
        {
            WaitHandles = new IAsyncWaitObject[List.Count + Handles.Length],
            Mapping = new int[List.Count + Handles.Length],
            Count = 0,
            ShortCircuitIndex = -1,
        };

        // Get the TInternalAsyncCall events
        for (int I = 0; I < List.Count; I++)
        {
            if (List[I] is IAsyncCallEx EventIntf)
            {
                IAsyncWaitObject ev = EventIntf.GetEvent();
                plan.WaitHandles[plan.Count] = ev; // 0 句柄 ⇒ null
                if (ev != null)
                {
                    plan.Mapping[plan.Count] = I;
                    plan.Count++;
                }
            }
            else if (!WaitAll)
            {
                // There are synchron calls in List[] and the caller does not want to wait for all handles.
                plan.ShortCircuitIndex = I;
                return plan;
            }
        }

        // Append other handles
        for (int I = 0; I < Handles.Length; I++)
        {
            plan.WaitHandles[plan.Count] = Handles[I];
            plan.Mapping[plan.Count] = List.Count + I;
            plan.Count++;
        }

        return plan;
    }

    /// <summary>
    /// 原文 1596-1601：把 Win32 等待结果换算成对外下标。
    /// <code>
    /// if SignalState &lt; WAIT_OBJECT_0 + Count then Result := WAIT_OBJECT_0 + Mapping[SignalState - WAIT_OBJECT_0]
    /// else if (SignalState >= WAIT_ABANDONED_0) and (SignalState &lt; WAIT_ABANDONED_0 + Count) then
    ///   Result := WAIT_ABANDONED_0 + Mapping[SignalState - WAIT_ABANDONED_0]
    /// else Result := SignalState;
    /// </code>
    /// </summary>
    public uint Translate(uint SignalState)
    {
        if (SignalState < AsyncCallsConst.WAIT_OBJECT_0 + (uint)Count)
            return AsyncCallsConst.WAIT_OBJECT_0 + (uint)Mapping[SignalState - AsyncCallsConst.WAIT_OBJECT_0];

        if (SignalState >= AsyncCallsConst.WAIT_ABANDONED_0
            && SignalState < AsyncCallsConst.WAIT_ABANDONED_0 + (uint)Count)
            return AsyncCallsConst.WAIT_ABANDONED_0 + (uint)Mapping[SignalState - AsyncCallsConst.WAIT_ABANDONED_0];

        return SignalState;
    }
}

/// <summary>
/// 原文 1411-1663：主线程等待包装 + 多对象等待。
/// <para>
/// 所有等待均经 <see cref="IAsyncWaitService"/> 注入，故分支可用脚本化替身精确复现，
/// 不消耗真实时间。
/// </para>
/// </summary>
public static class TAsyncCallMultiSync
{
    /// <summary>
    /// 原文 1411-1434：<c>WaitForSingleObjectMainThread</c>。
    /// <para>
    /// 等待 3 个对象（目标事件、RTL <c>SyncEvent</c>、池的 <c>MainThreadSyncEvent</c>）；
    /// 被后两者唤醒时执行同步并使循环继续。
    /// </para>
    /// <para>
    /// <b>原文如此（AsyncCalls.pas:1422-1428）</b>：循环<b>不</b>递减 <c>Timeout</c>，
    /// 因此被同步事件反复唤醒时，总等待时间可以远超调用方给定的 <c>Timeout</c>。
    /// 这是原设计固有的行为，<b>不得顺手修正</b>（已有差异断言守卫）。
    /// </para>
    /// </summary>
    public static uint WaitForSingleObjectMainThread(
        TAsyncCallRuntime runtime,
        TThreadPool pool,
        IAsyncWaitObject AHandle,
        uint Timeout)
    {
        if (runtime == null)
            throw new ArgumentNullException(nameof(runtime));
        if (pool == null)
            throw new ArgumentNullException(nameof(pool));

        var Handles = new IAsyncWaitObject[3];
        Handles[0] = AHandle;
        Handles[1] = runtime.MainThread.RtlSyncEvent;
        Handles[2] = pool.MainThreadSyncEvent;

        uint Result;
        do
        {
            Result = runtime.WaitService.WaitForMultipleObjects(Handles, false, Timeout);
            if (Result == AsyncCallsConst.WAIT_OBJECT_0 + 1)
                runtime.MainThread.CheckSynchronize();
            else if (Result == AsyncCallsConst.WAIT_OBJECT_0 + 2)
                runtime.MainThread.ProcessMainThreadSync();
        }
        while (Result == AsyncCallsConst.WAIT_OBJECT_0 + 1 || Result == AsyncCallsConst.WAIT_OBJECT_0 + 2);

        return Result;
    }

    /// <summary>
    /// 原文 1436-1520：<c>WaitForMultipleObjectsMainThread</c>。
    /// <para>
    /// 在调用方句柄数组后追加 RTL <c>SyncEvent</c> 与池 <c>MainThreadSyncEvent</c>。
    /// <c>WaitAll = False</c> 时直接转发；<c>WaitAll = True</c> 时退化为
    /// “逐个等待 + 命中后压缩句柄数组”的循环（原文 1479-1513），
    /// 并把<b>第一个</b>完成的句柄下标作为返回值（<c>FirstFinished</c>）。
    /// </para>
    /// </summary>
    public static uint WaitForMultipleObjectsMainThread(
        IAsyncWaitService waitService,
        IMainThreadDispatch mainThread,
        IAsyncWaitObject syncEvent,
        IAsyncWaitObject mainThreadSyncEvent,
        int Count,
        IAsyncWaitObject[] AHandles,
        bool WaitAll,
        uint Timeout,
        bool MsgWait,
        uint dwWakeMask)
    {
        if (waitService == null)
            throw new ArgumentNullException(nameof(waitService));
        if (mainThread == null)
            throw new ArgumentNullException(nameof(mainThread));

        // Wait for the specified events, for the VCL SyncEvent and for the MainThreadSync event
        uint OriginalCount = (uint)Count;
        var Handles = new IAsyncWaitObject[Count + 2];
        for (int i = 0; i < Count; i++)
            Handles[i] = AHandles[i];
        Handles[Count] = syncEvent;
        Handles[Count + 1] = mainThreadSyncEvent;

        uint Result;
        if (!WaitAll)
        {
            do
            {
                if (MsgWait)
                {
                    Result = waitService.MsgWaitForMultipleObjects(Handles, WaitAll, Timeout, dwWakeMask);
                    if (Result == AsyncCallsConst.WAIT_OBJECT_0 + (uint)Count + 2)
                    {
                        mainThread.ProcessMainThreadSync(); // also uses the message queue
                        Result = AsyncCallsConst.WAIT_OBJECT_0 + OriginalCount; // caller doesn't know about the 2 synchronization events
                        return Result;
                    }
                }
                else
                    Result = waitService.WaitForMultipleObjects(Handles, WaitAll, Timeout);

                if (Result == AsyncCallsConst.WAIT_OBJECT_0 + (uint)Count)
                    mainThread.CheckSynchronize();
                else if (Result == AsyncCallsConst.WAIT_OBJECT_0 + (uint)Count + 1)
                    mainThread.ProcessMainThreadSync();
            }
            while (Result == AsyncCallsConst.WAIT_OBJECT_0 + (uint)Count
                   || Result == AsyncCallsConst.WAIT_OBJECT_0 + (uint)Count + 1);
        }
        else
        {
            uint FirstFinished = AsyncCallsConst.WAIT_TIMEOUT;
            do
            {
                // 原文把整个动态数组 + 显式的 Count + 2 传给 Win32；托管侧改为传入等长的切片，
                // 因为 IAsyncWaitService 只认数组长度（否则压缩后会多等一个失效槽）。
                var slice = new IAsyncWaitObject[Count + 2];
                Array.Copy(Handles, slice, Count + 2);

                if (MsgWait)
                {
                    // 原文如此（AsyncCalls.pas:1482）：此处硬编码 False，而非传入 WaitAll（= True）
                    Result = waitService.MsgWaitForMultipleObjects(slice, false, Timeout, dwWakeMask);
                    if (Result == AsyncCallsConst.WAIT_OBJECT_0 + (uint)Count + 2)
                    {
                        mainThread.ProcessMainThreadSync(); // also uses the message queue
                        Result = AsyncCallsConst.WAIT_OBJECT_0 + OriginalCount; // caller doesn't know about the 2 synchronization events
                        return Result;
                    }
                }
                else
                    Result = waitService.WaitForMultipleObjects(slice, false, Timeout);

                if (Result == AsyncCallsConst.WAIT_OBJECT_0 + (uint)Count)
                    mainThread.CheckSynchronize();
                else if (Result == AsyncCallsConst.WAIT_OBJECT_0 + (uint)Count + 1)
                    mainThread.ProcessMainThreadSync();
                // 原文如此（AsyncCalls.pas:1498）：注释掉的 {(Result >= WAIT_OBJECT_0) and} —— 下界判断被省略
                else if (Result <= AsyncCallsConst.WAIT_OBJECT_0 + (uint)Count)
                {
                    if (FirstFinished == AsyncCallsConst.WAIT_TIMEOUT)
                        FirstFinished = Result;
                    Count--;
                    if (Count > 0)
                    {
                        uint Index = Result - AsyncCallsConst.WAIT_OBJECT_0;
                        // Move(Handles[Index + 1], Handles[Index], ((Count + 2) - Index) * SizeOf(THandle));
                        for (int i = (int)Index; i < Count + 2; i++)
                            Handles[i] = Handles[i + 1];
                    }
                }
                else
                    break;
            }
            while (Count != 0);

            if (Count == 0)
                Result = FirstFinished;
        }

        return Result;
    }

    /// <summary>
    /// 原文 1524-1639：<c>InternalAsyncMultiSync</c>（含内嵌 <c>InternalWait</c> /
    /// <c>InternalWaitAllInfinite</c>）。
    /// </summary>
    public static uint InternalAsyncMultiSync(
        TAsyncCallRuntime runtime,
        TThreadPool pool,
        IReadOnlyList<IAsyncCall> List,
        IAsyncWaitObject[] Handles,
        bool WaitAll,
        uint Milliseconds,
        bool MsgWait,
        uint dwWakeMask)
    {
        if (runtime == null)
            throw new ArgumentNullException(nameof(runtime));

        List ??= Array.Empty<IAsyncCall>();
        Handles ??= Array.Empty<IAsyncWaitObject>();

        int Count = List.Count + Handles.Length;
        if (Count > 0 && Count <= AsyncCallsConst.MAXIMUM_ASYNC_WAIT_OBJECTS)
        {
            if (WaitAll && Milliseconds == AsyncCallsConst.INFINITE && !MsgWait && !runtime.WaitService.IsMainThread)
                return InternalWaitAllInfinite(runtime, pool, List, Handles);

            return InternalWait(runtime, pool, List, Handles, WaitAll, Milliseconds, MsgWait, dwWakeMask);
        }

        // 原文 1638：对象数超限或为 0 ⇒ WAIT_FAILED（与 INFINITE 同值，见 AsyncCallsConst.WAIT_FAILED）
        return AsyncCallsConst.WAIT_FAILED;
    }

    /// <summary>原文 1527-1605：<c>InternalWait</c>。</summary>
    public static uint InternalWait(
        TAsyncCallRuntime runtime,
        TThreadPool pool,
        IReadOnlyList<IAsyncCall> List,
        IAsyncWaitObject[] Handles,
        bool WaitAll,
        uint Milliseconds,
        bool MsgWait,
        uint dwWakeMask)
    {
        var plan = TAsyncMultiSyncPlan.Build(List, Handles, WaitAll);

        // 原文 1553-1559：遇到非 IAsyncCallEx 且 WaitAll=False ⇒ 直接返回该下标
        if (plan.HasShortCircuit)
            return (uint)plan.ShortCircuitIndex;

        // Wait for the async calls
        if (plan.Count > 0)
        {
            uint SignalState;
            if (runtime.WaitService.IsMainThread)
            {
                var waitHandles = new IAsyncWaitObject[plan.Count];
                Array.Copy(plan.WaitHandles, waitHandles, plan.Count);
                SignalState = WaitForMultipleObjectsMainThread(
                    runtime.WaitService, runtime.MainThread, runtime.MainThread.RtlSyncEvent,
                    pool.MainThreadSyncEvent, plan.Count, waitHandles, WaitAll, Milliseconds, MsgWait, dwWakeMask);

                if (SignalState == (uint)plan.Count) // "message" was signaled
                    return SignalState;
            }
            else
            {
                var waitHandles = new IAsyncWaitObject[plan.Count];
                Array.Copy(plan.WaitHandles, waitHandles, plan.Count);

                if (MsgWait)
                {
                    SignalState = runtime.WaitService.MsgWaitForMultipleObjects(waitHandles, WaitAll, Milliseconds, dwWakeMask);
                    if (SignalState == (uint)plan.Count) // "message" was signaled
                        return SignalState;
                }
                else
                    SignalState = runtime.WaitService.WaitForMultipleObjects(waitHandles, WaitAll, Milliseconds);
            }

            return plan.Translate(SignalState);
        }

        // 原文 1603-1604：没有任何等待对象 ⇒ 视为全部已完成
        return AsyncCallsConst.WAIT_OBJECT_0;
    }

    /// <summary>
    /// 原文 1607-1624：<c>InternalWaitAllInfinite</c>。
    /// <para>
    /// 逐个 <c>List[I].Sync</c> 后（<c>nil</c> 项跳过）再等外部句柄；
    /// 返回值<b>恒为</b> <c>WAIT_OBJECT_0</c>（原文忽略底层等待结果）。
    /// </para>
    /// </summary>
    public static uint InternalWaitAllInfinite(
        TAsyncCallRuntime runtime,
        TThreadPool pool,
        IReadOnlyList<IAsyncCall> List,
        IAsyncWaitObject[] Handles)
    {
        // Wait for the async calls that aren't finished yet.
        for (int I = 0; I < List.Count; I++)
            if (List[I] != null)
                List[I].Sync();

        if (Handles.Length > 0)
        {
            if (runtime.WaitService.IsMainThread)
                WaitForMultipleObjectsMainThread(
                    runtime.WaitService, runtime.MainThread, runtime.MainThread.RtlSyncEvent,
                    pool.MainThreadSyncEvent, Handles.Length, Handles, true, AsyncCallsConst.INFINITE, false, 0);
            else
                runtime.WaitService.WaitForMultipleObjects(Handles, true, AsyncCallsConst.INFINITE);
        }

        return AsyncCallsConst.WAIT_OBJECT_0;
    }

    // ------------------------------------------------------------------
    // 原文 1641-1663：四个对外函数
    // ------------------------------------------------------------------

    /// <summary>原文 1641-1645：<c>AsyncMultiSync(List, WaitAll, Milliseconds)</c></summary>
    public static uint AsyncMultiSync(
        TAsyncCallRuntime runtime, TThreadPool pool,
        IReadOnlyList<IAsyncCall> List, bool WaitAll = true, uint Milliseconds = AsyncCallsConst.INFINITE)
    {
        return InternalAsyncMultiSync(runtime, pool, List, Array.Empty<IAsyncWaitObject>(), WaitAll, Milliseconds, false, 0);
    }

    /// <summary>原文 1647-1651：<c>AsyncMultiSyncEx</c></summary>
    public static uint AsyncMultiSyncEx(
        TAsyncCallRuntime runtime, TThreadPool pool,
        IReadOnlyList<IAsyncCall> List, IAsyncWaitObject[] Handles,
        bool WaitAll = true, uint Milliseconds = AsyncCallsConst.INFINITE)
    {
        return InternalAsyncMultiSync(runtime, pool, List, Handles, WaitAll, Milliseconds, false, 0);
    }

    /// <summary>原文 1653-1657：<c>MsgAsyncMultiSync</c></summary>
    public static uint MsgAsyncMultiSync(
        TAsyncCallRuntime runtime, TThreadPool pool,
        IReadOnlyList<IAsyncCall> List, bool WaitAll, uint Milliseconds, uint dwWakeMask)
    {
        return InternalAsyncMultiSync(runtime, pool, List, Array.Empty<IAsyncWaitObject>(), WaitAll, Milliseconds, true, dwWakeMask);
    }

    /// <summary>原文 1659-1663：<c>MsgAsyncMultiSyncEx</c></summary>
    public static uint MsgAsyncMultiSyncEx(
        TAsyncCallRuntime runtime, TThreadPool pool,
        IReadOnlyList<IAsyncCall> List, IAsyncWaitObject[] Handles,
        bool WaitAll, uint Milliseconds, uint dwWakeMask)
    {
        return InternalAsyncMultiSync(runtime, pool, List, Handles, WaitAll, Milliseconds, true, dwWakeMask);
    }
}
