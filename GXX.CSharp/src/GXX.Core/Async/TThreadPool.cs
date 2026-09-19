using System;
using System.Threading;

// 源单元：Source/M2Engine/AsyncCalls.pas（同源副本：Source/Client-HGE/AsyncCalls.pas、Source/RunGate/AsyncCalls.pas）

namespace GXX.Core.Async;

/// <summary>
/// Delphi <c>Classes.TThreadPriority</c>（原文 1691/1711 使用 <c>tpHigher</c> / <c>tpNormal</c>）。
/// 枚举顺序与 Delphi 一致，数值即原档位。
/// </summary>
public enum TThreadPriority
{
    tpIdle = 0,
    tpLowest = 1,
    tpLower = 2,
    tpNormal = 3,
    tpHigher = 4,
    tpHighest = 5,
    tpTimeCritical = 6,
}

/// <summary>
/// 原文 855-899：<c>TThreadPool</c> —— 池化线程 + FIFO 待执行队列 + 主线程消息同步。
/// <para>
/// <b>可测的纯逻辑</b>：队列的入队/出队/摘除、<c>AvailableThreadCount = FSleepingThreadCount -
/// FEnqueuedCallCount</c> 的扩容判据、<c>AllocThread</c> 的 CAS 计数、<c>FMaxThreads</c> 的
/// 默认公式与钳制。<b>线程外壳</b>经 <see cref="IAsyncThreadLauncher"/> 注入、等待经
/// <see cref="IAsyncWaitService"/> 注入。
/// </para>
/// </summary>
public sealed class TThreadPool : IDisposable
{
    // 原文 858-871
    private readonly IAsyncWaitObject FWakeUpEvent;
    private readonly IAsyncWaitObject FThreadTerminateEvent;
    private int FSleepingThreadCount;
    private int FEnqueuedCallCount;
    private int FMaxThreads;
    private bool FDestroying;

    private int FThreadCount;
    private readonly object[] FThreads = new object[AsyncCallsConst.ASYNC_CALL_THREAD_ARRAY_LENGTH];

    private readonly object FAsyncCallsCritSect = new object();
    private TInternalAsyncCall FAsyncCallHead;
    private TInternalAsyncCall FAsyncCallTail;

    private uint FNumberOfProcessors;

    // 原文 876-877：FMainThreadSyncEvent / FMainThreadVclHandle
    private readonly IAsyncWaitObject FMainThreadSyncEvent;

    private readonly TAsyncCallRuntime _runtime;
    private volatile bool FTerminateRequested;

    /// <summary>
    /// 原文 1751-1767：<c>constructor TThreadPool.Create</c>。
    /// </summary>
    /// <param name="runtime">注入的运行时；null ⇒ <see cref="TAsyncCallRuntime.Default"/>。</param>
    /// <param name="numberOfProcessors">
    /// 测试钩子：对应原文 <c>GetSystemInfo</c> 取得的 <c>dwNumberOfProcessors</c>。
    /// 传 null 时取 <see cref="Environment.ProcessorCount"/>（生产行为）。
    /// </param>
    public TThreadPool(TAsyncCallRuntime runtime = null, int? numberOfProcessors = null)
    {
        _runtime = runtime ?? TAsyncCallRuntime.Default;

        // 原文 1756-1757：FMainThreadVclHandle := AllocateHWnd(MainThreadWndProc);
        //                    FMainThreadSyncEvent := CreateEvent(nil, False, False, nil);
        // 接缝：窗口句柄由 IMainThreadDispatch 承接。
        FMainThreadSyncEvent = _runtime.WaitService.CreateManualResetEvent(false);

        // 原文 1758-1759：自动重置事件（第二个参数 False = 自动重置）
        FWakeUpEvent = _runtime.WaitService.CreateManualResetEvent(false);
        FThreadTerminateEvent = _runtime.WaitService.CreateManualResetEvent(true);

        // 原文 1760：InitializeCriticalSectionAndSpinCount(FAsyncCallsCritSect, 4000)
        // 托管侧 lock 不可设自旋计数；语义等价（登记为差异）。

        // 原文 1762-1763：GetSystemInfo(SysInfo); FNumberOfProcessors := SysInfo.dwNumberOfProcessors;
        FNumberOfProcessors = (uint)(numberOfProcessors ?? Environment.ProcessorCount);

        // 原文 1764-1766：
        //   FMaxThreads := SysInfo.dwNumberOfProcessors * 4 - 2 {main thread};
        //   if FMaxThreads > Length(FThreads) then FMaxThreads := Length(FThreads);
        FMaxThreads = ComputeDefaultMaxThreads(numberOfProcessors ?? Environment.ProcessorCount);
    }

    /// <summary>
    /// 原文 1764-1766 的默认线程数公式：<c>n * 4 - 2</c>，再钳制到 256。
    /// <para>
    /// <b>边界</b>：<c>n = 0</c> 时原文会得到 <c>-2</c>（不钳制下界）—— 保留原样。
    /// </para>
    /// </summary>
    public static int ComputeDefaultMaxThreads(int numberOfProcessors)
    {
        int maxThreads = numberOfProcessors * 4 - 2;
        if (maxThreads > AsyncCallsConst.ASYNC_CALL_THREAD_ARRAY_LENGTH)
            maxThreads = AsyncCallsConst.ASYNC_CALL_THREAD_ARRAY_LENGTH;
        return maxThreads;
    }

    // 原文 896-898：属性
    public int MaxThreads => FMaxThreads;

    public uint NumberOfProcessors => FNumberOfProcessors;

    public IAsyncWaitObject MainThreadSyncEvent => FMainThreadSyncEvent;

    /// <summary>当前池化线程计数（原文无访问器，仅为可测性暴露）。</summary>
    public int ThreadCount => Volatile.Read(ref FThreadCount);

    /// <summary>已入队但未被取走的调用数（原文无访问器，仅为可测性暴露）。</summary>
    public int EnqueuedCallCount => Volatile.Read(ref FEnqueuedCallCount);

    /// <summary>睡眠线程数（原文无访问器，仅为可测性暴露）。</summary>
    public int SleepingThreadCount => Volatile.Read(ref FSleepingThreadCount);

    /// <summary>队首（原文 869 的 <c>FAsyncCallHead</c>；仅为可测性暴露，不改变语义）。</summary>
    public TInternalAsyncCall AsyncCallHead => FAsyncCallHead;

    /// <summary>队尾（原文 869 的 <c>FAsyncCallTail</c>；仅为可测性暴露，不改变语义）。</summary>
    public TInternalAsyncCall AsyncCallTail => FAsyncCallTail;

    /// <summary>原文 863：<c>FDestroying</c>（被 <c>SyncInThisThreadIfPossible</c> 读取）</summary>
    public bool Destroying => FDestroying;

    /// <summary>第 <paramref name="index"/> 号线程槽（原文 866：<c>FThreads[index]</c>）。</summary>
    public object ThreadSlot(int index) => FThreads[index];

    /// <summary>原文 1102-1108 的目标对象：池自身的线程上限设定。</summary>
    public void SetMaxThreads(int MaxThreads)
    {
        if (MaxThreads >= FThreads.Length)
            MaxThreads = FThreads.Length;
        if (MaxThreads >= 0)
            FMaxThreads = MaxThreads;
    }

    /// <summary>
    /// 原文 1769-1807：<c>destructor TThreadPool.Destroy</c>。
    /// <para>
    /// <c>FMaxThreads := FThreadCount</c>（不再新分配线程）、<c>FDestroying := True</c>、
    /// 逐个 <c>Terminate</c>、置位 <c>FThreadTerminateEvent</c> 唤醒全部睡眠线程、
    /// 回收线程、排空尚未释放的 <c>TInternalAsyncCall</c>、关闭句柄、删除临界区。
    /// </para>
    /// </summary>
    public void Destroy()
    {
        FMaxThreads = FThreadCount; // Do not allocation new threads
        FDestroying = true; // => Sync in this thread because there is no other thread

        // 原文 1779-1780
        FTerminateRequested = true;

        // 原文 1782：SetEvent(FThreadTerminateEvent) —— 保持唤醒以便线程终止
        FThreadTerminateEvent.Set();

        // 原文 1784-1785：Wait 并销毁线程。
        // 接缝：托管线程由 IAsyncThreadLauncher 持有，池只清空槽位。
        for (int I = FThreadCount - 1; I >= 0; I--)
        {
            object thread = FThreads[I];
            if (thread is IDisposable d)
                d.Dispose();
            FThreads[I] = null;
        }

        // 原文 1788-1793：清理尚未释放的 TInternalAsyncCalls
        while (FAsyncCallHead != null)
        {
            TInternalAsyncCall Call = FAsyncCallHead.FNext;
            FAsyncCallHead.Release();
            FAsyncCallHead = Call;
        }
        FAsyncCallTail = null;
        FEnqueuedCallCount = 0;

        // 原文 1796-1797：处理“被遗忘”的异步调用遗留的致命异常（PeekMessage/DispatchMessage）
        // 接缝：由 IMainThreadDispatch 承接。

        // 原文 1799-1804：关闭句柄 + DeallocateHWnd + DeleteCriticalSection
        if (FThreadTerminateEvent is IDisposable dte)
            dte.Dispose();
        if (FWakeUpEvent is IDisposable dwe)
            dwe.Dispose();
        if (FMainThreadSyncEvent is IDisposable dmse)
            dmse.Dispose();
    }

    /// <summary>对应 Delphi <c>TObject.Free</c>。</summary>
    public void Free() => Destroy();

    /// <inheritdoc />
    public void Dispose() => Destroy();

    /// <summary>原文 1809-1813：<c>if FDestroying then raise EAsyncCallError.CreateRes(@RsNoVclSyncPossible)</c></summary>
    public void CheckDestroying()
    {
        if (FDestroying)
            throw new TAsyncCallError(AsyncCallsConst.RsNoVclSyncPossible);
    }

    /// <summary>
    /// 原文 1815-1840：池化线程取下一个待执行调用；队列为空则 <c>Sleep</c>。
    /// <para>
    /// <b>原文如此</b>：<c>if FAsyncCallHead &lt;&gt; nil then WakeUpThread</c> 是
    /// <b>临界区之外</b>的无保护访问（原文 1834 自带注释承认 unsafe）；
    /// 而 <c>FAsyncCallTail := nil</c> 只在出队者恰为尾节点时发生。
    /// </para>
    /// </summary>
    public TInternalAsyncCall GetNextAsyncCall()
    {
        TInternalAsyncCall Result;
        lock (FAsyncCallsCritSect) // spinning
        {
            // Skip reference count handling, because we would increment (Result) and decrement (ListRemove) it.
            Result = FAsyncCallHead;
            if (Result != null)
                FEnqueuedCallCount--;
            if (FAsyncCallHead != null)
                FAsyncCallHead = FAsyncCallHead.FNext;
            if (Result == FAsyncCallTail)
                FAsyncCallTail = null;
        }

        if (FAsyncCallHead != null)
            WakeUpThread();

        if (Result == null)
            Sleep();

        return Result;
    }

    /// <summary>
    /// 原文 1842-1876：从队列中摘除指定调用（供 <c>SyncInThisThreadIfPossible</c> 使用）。
    /// </summary>
    public bool RemoveAsyncCall(TInternalAsyncCall Call)
    {
        bool Result = false;
        lock (FAsyncCallsCritSect)
        {
            TInternalAsyncCall Item = FAsyncCallHead;
            if (ReferenceEquals(Item, Call))
            {
                FAsyncCallHead = Call.FNext;
                if (FAsyncCallHead == null)
                    FAsyncCallTail = null;
                FEnqueuedCallCount--;
                Result = true;
            }
            else
            {
                while (Item != null && !ReferenceEquals(Item.FNext, Call))
                    Item = Item.FNext;
                if (Item != null)
                {
                    Item.FNext = Call.FNext;
                    if (ReferenceEquals(Call, FAsyncCallTail))
                        FAsyncCallTail = Item;
                    FEnqueuedCallCount--;
                    Result = true;
                }
            }
        }
        if (Result)
            Call.Release(); // removed from list
        return Result;
    }

    /// <summary>
    /// 原文 1878-1911：入队并决定是否扩容线程。
    /// <para>
    /// <b>扩容判据</b>：<c>AvailableThreadCount := FSleepingThreadCount - FEnqueuedCallCount</c>
    /// 是在 <c>Inc(FEnqueuedCallCount)</c> <b>之前</b>算出的，
    /// 即“本次入队前，睡眠线程是否已被既有排队任务预定完”。
    /// </para>
    /// </summary>
    public void AddAsyncCall(TInternalAsyncCall Call)
    {
        Call.AddRef(); // added to list

        int AvailableThreadCount;
        lock (FAsyncCallsCritSect)
        {
            if (FAsyncCallTail == null)
            {
                FAsyncCallHead = Call;
                FAsyncCallTail = Call;
            }
            else
            {
                FAsyncCallTail.FNext = Call;
                FAsyncCallTail = Call;
            }
            AvailableThreadCount = FSleepingThreadCount - FEnqueuedCallCount;
            FEnqueuedCallCount++;
        }

        if (AvailableThreadCount <= 0)
        {
            if (FThreadCount < MaxThreads)
                AllocThread();
        }

        WakeUpThread();
    }

    /// <summary>
    /// 原文 1913-1924：CAS 递增 <c>FThreadCount</c> 并启动新线程。
    /// <code>
    /// repeat Index := FThreadCount;
    /// until (Index = FMaxThreads) or (InterlockedCompareExchange(FThreadCount, Index + 1, Index) = Index);
    /// if Index &lt; FMaxThreads then FThreads[Index] := TAsyncCallThread.Create(False);
    /// </code>
    /// <para>
    /// <b>边界</b>：<c>Index = FMaxThreads</c> 时循环以 <c>Index = FMaxThreads</c> 退出，
    /// 且因 <c>Index &lt; FMaxThreads</c> 为假而<b>不</b>建线程 —— 这正是“超限即静默不扩容”。
    /// </para>
    /// </summary>
    public void AllocThread()
    {
        int Index;
        do
        {
            Index = FThreadCount;
        }
        while (Index != FMaxThreads
               && Interlocked.CompareExchange(ref FThreadCount, Index + 1, Index) != Index);

        if (Index < FMaxThreads)
        {
            int slot = Index;
            FThreads[slot] = _runtime.ThreadLauncher;
            _runtime.ThreadLauncher.StartThread(slot, () => ThreadExecute());
        }
    }

    /// <summary>
    /// 原文 1926-1934：<c>SendVclSync</c> —— 投递失败则就地 <c>Quit(0)</c>。
    /// </summary>
    public void SendVclSync(TInternalAsyncCall Call)
    {
        CheckDestroying();

        if (!_runtime.MainThread.PostVclSync(Call))
            Call.Quit(0);
        else
            FMainThreadSyncEvent.Set();
    }

    /// <summary>原文 1936-1940：<c>SetEvent(FWakeUpEvent)</c></summary>
    public void WakeUpThread()
    {
        FWakeUpEvent.Set();
    }

    /// <summary>
    /// 原文 1942-1957：睡眠线程计数 +1、等待唤醒或终止、计数 -1。
    /// <para>
    /// <c>SetThreadPriority(GetCurrentThread, THREAD_PRIORITY_ABOVE_NORMAL)</c> 为线程优先级接缝。
    /// </para>
    /// </summary>
    public void Sleep()
    {
        var Handles = new[] { FWakeUpEvent, FThreadTerminateEvent };

        Interlocked.Increment(ref FSleepingThreadCount);

        // 原文 1952：SetThreadPriority(GetCurrentThread, THREAD_PRIORITY_ABOVE_NORMAL)
        // 接缝：当前线程优先级由 IAsyncThreadLauncher 实现方维护。

        _runtime.WaitService.WaitForMultipleObjects(Handles, false, AsyncCallsConst.INFINITE);
        // SetThreadPriority(GetCurrentThread, THREAD_PRIORITY_NORMAL);  done in TAsyncThread.Execute

        Interlocked.Decrement(ref FSleepingThreadCount);
    }

    /// <summary>
    /// 原文 1976-1986：<c>ProcessMainThreadSync</c> —— 排空本池的 WM_VCLSYNC 消息。
    /// <para>接缝：真实消息泵由 <see cref="IMainThreadDispatch"/> 承接。</para>
    /// </summary>
    public void ProcessMainThreadSync()
    {
        _runtime.MainThread.ProcessMainThreadSync();
    }

    /// <summary>池级终止请求（对应 <c>TThread.Terminated</c>，原文在 1731 行读取）。</summary>
    public bool Terminated => FTerminateRequested;

    /// <summary>对应 <c>TThread.Terminate</c>（原文 1780）。</summary>
    public void Terminate() => FTerminateRequested = true;

    /// <summary>
    /// 原文 1694-1746：<c>TAsyncCallThread.Execute</c> —— 池化线程主循环。
    /// <para>
    /// <c>CoInitialize</c>/<c>CoUninitialize</c> 为 COM 套间初始化接缝（原文 1703-1705、1743-1744）。
    /// </para>
    /// </summary>
    public void ThreadExecute()
    {
        // 原文 1702-1706：CoInitialize(nil) 返回值 S_OK/S_FALSE 视为成功。
        // 接缝：托管侧无 COM 套间初始化需求，由宿主决定（登记为差异）。

        while (true)
        {
            TInternalAsyncCall FAsyncCall = GetNextAsyncCall(); // calls Sleep if nothing has to be done
            // 原文 1711：Priority := tpNormal;

            if (FAsyncCall != null)
            {
                try
                {
                    FAsyncCall.InternExecuteAsyncCall();
                }
                catch
                {
                    // 原文 1720-1725：{$IFDEF DEBUG_ASYNCCALLS_ODS} 未定义 ⇒ 空 except
                }
            }
            else if (Terminated)
            {
                // Thread will quit if the application terminates and no further task is in the queue.
                FAsyncCall = GetNextAsyncCall();
                if (FAsyncCall == null)
                    break;
            }

            if (FAsyncCall != null)
                FAsyncCall.Release();
        }
    }

    /// <summary>原文 1959-1974：主线程窗口过程的 WM_VCLSYNC / WM_RAISEEXCEPTION 分支。</summary>
    public void MainThreadWndProc(int Msg, TInternalAsyncCall LParam)
    {
        switch (Msg)
        {
            case AsyncCallsConst.WM_VCLSYNC:
                LParam.InternExecuteSyncCall();
                break;
            case AsyncCallsConst.WM_RAISEEXCEPTION:
                // 原文：raise Exception(Msg.LParam) at Pointer(Msg.WParam)
                throw new TAsyncCallError("WM_RAISEEXCEPTION");
            default:
                // 原文：Msg.Result := DefWindowProc(FMainThreadVclHandle, Msg.Msg, Msg.WParam, Msg.LParam)
                break;
        }
    }
}
