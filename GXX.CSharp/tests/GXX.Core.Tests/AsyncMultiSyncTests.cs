using System;
using System.Collections.Generic;
using System.Linq;
using GXX.Core.Async;
using Xunit;

namespace GXX.Core.Tests;

/// <summary>
/// 多对象等待的映射、句柄压缩与结果换算测试（原文 AsyncCalls.pas:1411-1663）。
/// </summary>
public class AsyncMultiSyncTests
{
    private const uint OBJECT_0 = AsyncCallsConst.WAIT_OBJECT_0;
    private const uint ABANDONED_0 = AsyncCallsConst.WAIT_ABANDONED_0;
    private const uint TIMEOUT = AsyncCallsConst.WAIT_TIMEOUT;

    private static FakeAsyncCall Fake(FakeWaitService wait, bool withEvent = true)
    {
        return new FakeAsyncCall(withEvent ? wait.CreateManualResetEvent(false) : null);
    }

    // ------------------------------------------------------------------
    // TAsyncMultiSyncPlan.Build（原文 1537-1568）
    // ------------------------------------------------------------------

    [Fact]
    public void Plan_Build_SkipsAsyncCallsWithoutEventAndKeepsTheirIndicesOutOfMapping()
    {
        var wait = new FakeWaitService();
        var list = new List<IAsyncCall>
        {
            Fake(wait, withEvent: true),   // 槽 0 → 对外下标 0
            Fake(wait, withEvent: false),  // 无事件 ⇒ 不占槽
            Fake(wait, withEvent: true),   // 槽 1 → 对外下标 2
        };

        TAsyncMultiSyncPlan plan = TAsyncMultiSyncPlan.Build(list, Array.Empty<IAsyncWaitObject>(), true);

        Assert.Equal(2, plan.Count);
        Assert.Equal(0, plan.Mapping[0]);
        Assert.Equal(2, plan.Mapping[1]);
        Assert.NotNull(plan.WaitHandles[0]);
        Assert.NotNull(plan.WaitHandles[1]);
        Assert.False(plan.HasShortCircuit);
    }

    [Fact]
    public void Plan_Build_AppendsExternalHandlesWithOffsetMapping()
    {
        var wait = new FakeWaitService();
        var list = new List<IAsyncCall> { Fake(wait), Fake(wait) };
        var handles = new[]
        {
            wait.CreateManualResetEvent(false),
            wait.CreateManualResetEvent(false),
        };

        TAsyncMultiSyncPlan plan = TAsyncMultiSyncPlan.Build(list, handles, true);

        Assert.Equal(4, plan.Count);
        Assert.Equal(new[] { 0, 1, 2, 3 }, plan.Mapping.Take(4).ToArray());
        Assert.Same(handles[0], plan.WaitHandles[2]);
        Assert.Same(handles[1], plan.WaitHandles[3]);
    }

    [Fact]
    public void Plan_Build_NotWaitAll_ShortCircuitsOnFirstNonAsyncCallExItem()
    {
        var wait = new FakeWaitService();
        var list = new List<IAsyncCall> { Fake(wait), new PlainAsyncCall(), Fake(wait) };

        TAsyncMultiSyncPlan plan = TAsyncMultiSyncPlan.Build(list, Array.Empty<IAsyncWaitObject>(), false);

        Assert.True(plan.HasShortCircuit);
        Assert.Equal(1, plan.ShortCircuitIndex);
    }

    [Fact]
    public void Plan_Build_WaitAll_DoesNotShortCircuitOnNonAsyncCallExItem()
    {
        var wait = new FakeWaitService();
        var list = new List<IAsyncCall> { new PlainAsyncCall(), Fake(wait) };

        TAsyncMultiSyncPlan plan = TAsyncMultiSyncPlan.Build(list, Array.Empty<IAsyncWaitObject>(), true);

        Assert.False(plan.HasShortCircuit);
        // PlainAsyncCall 不占等待槽，只有第 2 项占槽（对外下标 1）
        Assert.Equal(1, plan.Count);
        Assert.Equal(1, plan.Mapping[0]);
    }

    [Fact]
    public void Plan_Build_EmptyInputs_YieldsEmptyPlan()
    {
        TAsyncMultiSyncPlan plan = TAsyncMultiSyncPlan.Build(null, null, true);

        Assert.Equal(0, plan.Count);
        Assert.False(plan.HasShortCircuit);
    }

    // ------------------------------------------------------------------
    // TAsyncMultiSyncPlan.Translate（原文 1596-1601）
    // ------------------------------------------------------------------

    [Fact]
    public void Plan_Translate_ObjectIndexIsRemappedThroughMapping()
    {
        var wait = new FakeWaitService();
        var list = new List<IAsyncCall>
        {
            Fake(wait, withEvent: false),  // 不占槽
            Fake(wait),                    // 槽 0 → 对外 1
            Fake(wait),                    // 槽 1 → 对外 2
        };
        TAsyncMultiSyncPlan plan = TAsyncMultiSyncPlan.Build(list, Array.Empty<IAsyncWaitObject>(), true);

        Assert.Equal(OBJECT_0 + 1, plan.Translate(OBJECT_0 + 0));
        Assert.Equal(OBJECT_0 + 2, plan.Translate(OBJECT_0 + 1));
    }

    [Fact]
    public void Plan_Translate_AbandonedIndexIsRemapped()
    {
        var wait = new FakeWaitService();
        var list = new List<IAsyncCall> { Fake(wait), Fake(wait) };
        TAsyncMultiSyncPlan plan = TAsyncMultiSyncPlan.Build(list, Array.Empty<IAsyncWaitObject>(), true);

        Assert.Equal(ABANDONED_0 + 1, plan.Translate(ABANDONED_0 + 1));
    }

    [Fact]
    public void Plan_Translate_BoundaryValuesPassThroughUnchanged()
    {
        var wait = new FakeWaitService();
        var list = new List<IAsyncCall> { Fake(wait), Fake(wait) };
        TAsyncMultiSyncPlan plan = TAsyncMultiSyncPlan.Build(list, Array.Empty<IAsyncWaitObject>(), true);

        // 个数 = 2 ⇒ 0..1 是对象，2 是“消息”，ABANDONED_0+2 已越界
        Assert.Equal(OBJECT_0 + 2, plan.Translate(OBJECT_0 + 2));
        Assert.Equal(TIMEOUT, plan.Translate(TIMEOUT));
        Assert.Equal(ABANDONED_0 + 2, plan.Translate(ABANDONED_0 + 2));
    }

    [Fact]
    public void Plan_Translate_ZeroCount_PassesEverythingThrough()
    {
        TAsyncMultiSyncPlan plan = TAsyncMultiSyncPlan.Build(Array.Empty<IAsyncCall>(), Array.Empty<IAsyncWaitObject>(), true);

        Assert.Equal(OBJECT_0, plan.Translate(OBJECT_0));
        Assert.Equal(ABANDONED_0, plan.Translate(ABANDONED_0));
    }

    // ------------------------------------------------------------------
    // InternalAsyncMultiSync 的对象数门限（原文 1626-1639）
    // ------------------------------------------------------------------

    [Fact]
    public void InternalAsyncMultiSync_ZeroObjects_ReturnsWaitFailed()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();

        uint result = TAsyncCallMultiSync.InternalAsyncMultiSync(
            rt.Runtime, pool, Array.Empty<IAsyncCall>(), Array.Empty<IAsyncWaitObject>(),
            true, AsyncCallsConst.INFINITE, false, 0);

        // 原文：只有 Count > 0 才真正等待；Count = 0 落入 else ⇒ WAIT_FAILED
        Assert.Equal(AsyncCallsConst.WAIT_FAILED, result);
        Assert.Empty(rt.Wait.Calls);
    }

    [Fact]
    public void InternalAsyncMultiSync_AtMaximumAsyncWaitObjects_IsAccepted()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        List<IAsyncCall> list = Enumerable.Range(0, AsyncCallsConst.MAXIMUM_ASYNC_WAIT_OBJECTS)
            .Select(_ => (IAsyncCall)Fake(rt.Wait)).ToList();

        uint result = TAsyncCallMultiSync.InternalAsyncMultiSync(
            rt.Runtime, pool, list, Array.Empty<IAsyncWaitObject>(),
            true, AsyncCallsConst.INFINITE, false, 0);

        Assert.Equal(OBJECT_0, result);
        Assert.All(list, c => Assert.Equal(1, ((FakeAsyncCall)c).SyncCount));
    }

    [Fact]
    public void InternalAsyncMultiSync_OneOverMaximum_ReturnsWaitFailed()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        List<IAsyncCall> list = Enumerable.Range(0, AsyncCallsConst.MAXIMUM_ASYNC_WAIT_OBJECTS + 1)
            .Select(_ => (IAsyncCall)Fake(rt.Wait)).ToList();

        uint result = TAsyncCallMultiSync.InternalAsyncMultiSync(
            rt.Runtime, pool, list, Array.Empty<IAsyncWaitObject>(),
            true, AsyncCallsConst.INFINITE, false, 0);

        Assert.Equal(AsyncCallsConst.WAIT_FAILED, result);
        Assert.Empty(rt.Wait.Calls);
        Assert.All(list, c => Assert.Equal(0, ((FakeAsyncCall)c).SyncCount)); // 根本未等待
    }

    // ------------------------------------------------------------------
    // InternalWaitAllInfinite 的选取条件（原文 1607-1624、1632-1635）
    // ------------------------------------------------------------------

    [Fact]
    public void WaitAllInfinite_SelectedOnlyWhenWaitAllInfiniteNoMsgAndNotMainThread()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        rt.Wait.IsMainThread = false;
        var list = new List<IAsyncCall> { Fake(rt.Wait), Fake(rt.Wait), null };

        uint result = TAsyncCallMultiSync.InternalAsyncMultiSync(
            rt.Runtime, pool, list, Array.Empty<IAsyncWaitObject>(),
            true, AsyncCallsConst.INFINITE, false, 0);

        Assert.Equal(OBJECT_0, result);
        // 空项被跳过，非空项各 Sync 一次
        Assert.Equal(1, ((FakeAsyncCall)list[0]).SyncCount);
        Assert.Equal(1, ((FakeAsyncCall)list[1]).SyncCount);
        Assert.Empty(rt.Wait.Calls); // 无外部句柄 ⇒ 不发生等待
    }

    [Theory]
    [InlineData(true, 100u, false, false)]  // 有限超时 ⇒ 走 InternalWait
    [InlineData(true, AsyncCallsConst.INFINITE, true, false)]  // MsgWait ⇒ 走 InternalWait
    [InlineData(true, AsyncCallsConst.INFINITE, false, true)]  // 主线程 ⇒ 走 InternalWait
    [InlineData(false, AsyncCallsConst.INFINITE, false, false)] // WaitAll=False ⇒ 走 InternalWait
    public void InternalAsyncMultiSync_OtherCombinations_RouteThroughInternalWait(
        bool waitAll, uint milliseconds, bool msgWait, bool mainThread)
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        rt.Wait.IsMainThread = mainThread;
        rt.Wait.Scripted.Enqueue(OBJECT_0);
        var list = new List<IAsyncCall> { Fake(rt.Wait) };

        TAsyncCallMultiSync.InternalAsyncMultiSync(
            rt.Runtime, pool, list, Array.Empty<IAsyncWaitObject>(),
            waitAll, milliseconds, msgWait, 0x04FF);

        Assert.NotEmpty(rt.Wait.Calls);
        Assert.Equal(0, ((FakeAsyncCall)list[0]).SyncCount); // 未走 InternalWaitAllInfinite
        if (mainThread)
            Assert.Equal(3, rt.Wait.Calls[0].Handles.Length); // + RTL SyncEvent + 池 SyncEvent
        else
            Assert.Equal(1, rt.Wait.Calls[0].Handles.Length);
    }

    // ------------------------------------------------------------------
    // InternalWait（原文 1527-1605）
    // ------------------------------------------------------------------

    [Fact]
    public void InternalWait_AllEventsZero_ReturnsWaitObjectZeroWithoutWaiting()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        var list = new List<IAsyncCall> { Fake(rt.Wait, withEvent: false) };

        uint result = TAsyncCallMultiSync.InternalWait(
            rt.Runtime, pool, list, Array.Empty<IAsyncWaitObject>(), true, 50, false, 0);

        Assert.Equal(OBJECT_0, result);
        Assert.Empty(rt.Wait.Calls);
    }

    [Fact]
    public void InternalWait_ShortCircuit_ReturnsIndexWithoutWaiting()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        var list = new List<IAsyncCall> { Fake(rt.Wait), new PlainAsyncCall() };

        uint result = TAsyncCallMultiSync.InternalWait(
            rt.Runtime, pool, list, Array.Empty<IAsyncWaitObject>(), false, 10, false, 0);

        Assert.Equal(1u, result);
        Assert.Empty(rt.Wait.Calls);
    }

    [Fact]
    public void InternalWait_NonMainThread_TranslatesFinishedIndex()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        var list = new List<IAsyncCall>
        {
            Fake(rt.Wait, withEvent: false), // 不占槽
            Fake(rt.Wait),                   // 槽 0 → 对外 1
            Fake(rt.Wait),                   // 槽 1 → 对外 2
        };
        rt.Wait.Scripted.Enqueue(OBJECT_0 + 1);

        uint result = TAsyncCallMultiSync.InternalWait(
            rt.Runtime, pool, list, Array.Empty<IAsyncWaitObject>(), false, 250, false, 0);

        Assert.Equal(OBJECT_0 + 2, result);
        WaitCallRecord call = Assert.Single(rt.Wait.Calls);
        Assert.Equal(2, call.Handles.Length);
        Assert.Equal(250u, call.Milliseconds);
        Assert.False(call.IsMsgWait);
    }

    [Fact]
    public void InternalWait_NonMainThread_AbandonedIsRemapped()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        var list = new List<IAsyncCall> { Fake(rt.Wait), Fake(rt.Wait) };
        rt.Wait.Scripted.Enqueue(ABANDONED_0 + 1);

        uint result = TAsyncCallMultiSync.InternalWait(
            rt.Runtime, pool, list, Array.Empty<IAsyncWaitObject>(), false, 10, false, 0);

        Assert.Equal(ABANDONED_0 + 1, result);
    }

    [Fact]
    public void InternalWait_NonMainThread_TimeoutPassesThrough()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        var list = new List<IAsyncCall> { Fake(rt.Wait) };
        rt.Wait.DefaultResult = TIMEOUT;

        uint result = TAsyncCallMultiSync.InternalWait(
            rt.Runtime, pool, list, Array.Empty<IAsyncWaitObject>(), false, 10, false, 0);

        Assert.Equal(TIMEOUT, result);
    }

    [Fact]
    public void InternalWait_NonMainThread_MsgWaitMessageSignalReturnsCount()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        var list = new List<IAsyncCall> { Fake(rt.Wait), Fake(rt.Wait) };
        rt.Wait.Scripted.Enqueue(OBJECT_0 + 2); // == Count ⇒ “消息被置位”

        uint result = TAsyncCallMultiSync.InternalWait(
            rt.Runtime, pool, list, Array.Empty<IAsyncWaitObject>(), false, 10, true, AsyncCallsConst.QS_ALLINPUT);

        Assert.Equal(2u, result);
        WaitCallRecord call = Assert.Single(rt.Wait.Calls);
        Assert.True(call.IsMsgWait);
        Assert.Equal(AsyncCallsConst.QS_ALLINPUT, call.WakeMask);
    }

    [Fact]
    public void InternalWait_ExternalHandlesAreMappedAfterListIndices()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        var list = new List<IAsyncCall> { Fake(rt.Wait) };
        var handles = new[]
        {
            rt.Wait.CreateManualResetEvent(false),
            rt.Wait.CreateManualResetEvent(false),
        };
        rt.Wait.Scripted.Enqueue(OBJECT_0 + 2); // 槽 2 == 第 2 个外部句柄

        uint result = TAsyncCallMultiSync.InternalWait(
            rt.Runtime, pool, list, handles, false, 10, false, 0);

        Assert.Equal(OBJECT_0 + 2, result); // Mapping[2] = 1 + 1 = 2
        Assert.Equal(3, rt.Wait.Calls[0].Handles.Length);
    }

    // ------------------------------------------------------------------
    // WaitForSingleObjectMainThread（原文 1411-1434）
    // ------------------------------------------------------------------

    [Fact]
    public void WaitForSingleObjectMainThread_LoopsOverBothSyncEvents()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        IAsyncWaitObject target = rt.Wait.CreateManualResetEvent(false);
        rt.Wait.Scripted.Enqueue(OBJECT_0 + 1); // RTL SyncEvent
        rt.Wait.Scripted.Enqueue(OBJECT_0 + 2); // 池 MainThreadSyncEvent
        rt.Wait.Scripted.Enqueue(OBJECT_0);

        uint result = TAsyncCallMultiSync.WaitForSingleObjectMainThread(rt.Runtime, pool, target, 123);

        Assert.Equal(OBJECT_0, result);
        Assert.Equal(1, rt.MainThread.CheckSynchronizeCount);
        Assert.Equal(1, rt.MainThread.ProcessMainThreadSyncCount);
        Assert.Equal(3, rt.Wait.Calls.Count);
        foreach (WaitCallRecord c in rt.Wait.Calls)
        {
            Assert.Equal(3, c.Handles.Length);
            Assert.Equal(target.Handle, c.Handles[0]);
            Assert.Equal(rt.MainThread.RtlSyncEvent.Handle, c.Handles[1]);
            Assert.Equal(pool.MainThreadSyncEvent.Handle, c.Handles[2]);
            // 差异断言：原文不递减 Timeout —— 被反复唤醒时总等待可远超给定值
            Assert.Equal(123u, c.Milliseconds);
        }
    }

    [Fact]
    public void WaitForSingleObjectMainThread_TimeoutReturnsImmediately()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        IAsyncWaitObject target = rt.Wait.CreateManualResetEvent(false);
        rt.Wait.DefaultResult = TIMEOUT;

        uint result = TAsyncCallMultiSync.WaitForSingleObjectMainThread(rt.Runtime, pool, target, 0);

        Assert.Equal(TIMEOUT, result);
        Assert.Equal(0, rt.MainThread.CheckSynchronizeCount);
        Assert.Single(rt.Wait.Calls);
    }

    [Fact]
    public void WaitForSingleObjectMainThread_NullPool_Throws()
    {
        var rt = new FakeRuntime();

        Assert.Throws<ArgumentNullException>(() => TAsyncCallMultiSync.WaitForSingleObjectMainThread(
            rt.Runtime, null, rt.Wait.CreateManualResetEvent(false), 0));
        Assert.Throws<ArgumentNullException>(() => TAsyncCallMultiSync.WaitForSingleObjectMainThread(
            null, rt.NewPool(), rt.Wait.CreateManualResetEvent(false), 0));
    }

    // ------------------------------------------------------------------
    // WaitForMultipleObjectsMainThread（原文 1436-1520）
    // ------------------------------------------------------------------

    [Fact]
    public void WaitForMultipleObjectsMainThread_NotWaitAll_ForwardsAndFiltersSyncEvents()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        var handles = new[] { rt.Wait.CreateManualResetEvent(false), rt.Wait.CreateManualResetEvent(false) };
        rt.Wait.Scripted.Enqueue(OBJECT_0 + 2); // RTL SyncEvent（Count = 2）
        rt.Wait.Scripted.Enqueue(OBJECT_0 + 3); // 池 SyncEvent
        rt.Wait.Scripted.Enqueue(OBJECT_0 + 1);

        uint result = TAsyncCallMultiSync.WaitForMultipleObjectsMainThread(
            rt.Wait, rt.MainThread, rt.MainThread.RtlSyncEvent, pool.MainThreadSyncEvent,
            2, handles, false, 77, false, 0);

        Assert.Equal(OBJECT_0 + 1, result);
        Assert.Equal(1, rt.MainThread.CheckSynchronizeCount);
        Assert.Equal(1, rt.MainThread.ProcessMainThreadSyncCount);
        Assert.Equal(3, rt.Wait.Calls.Count);
        Assert.All(rt.Wait.Calls, c => Assert.Equal(4, c.Handles.Length)); // 2 + 2
        Assert.All(rt.Wait.Calls, c => Assert.Equal(77u, c.Milliseconds));
    }

    [Fact]
    public void WaitForMultipleObjectsMainThread_NotWaitAll_MsgWakeReturnsOriginalCount()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        var handles = new[] { rt.Wait.CreateManualResetEvent(false), rt.Wait.CreateManualResetEvent(false) };
        rt.Wait.Scripted.Enqueue(OBJECT_0 + 4); // Count + 2 ⇒ 消息

        uint result = TAsyncCallMultiSync.WaitForMultipleObjectsMainThread(
            rt.Wait, rt.MainThread, rt.MainThread.RtlSyncEvent, pool.MainThreadSyncEvent,
            2, handles, false, 5, true, AsyncCallsConst.QS_ALLINPUT);

        Assert.Equal(OBJECT_0 + 2, result); // 调用方不知道那 2 个同步事件
        Assert.Equal(1, rt.MainThread.ProcessMainThreadSyncCount);
        Assert.Single(rt.Wait.Calls);
        Assert.True(rt.Wait.Calls[0].IsMsgWait);
    }

    [Fact]
    public void WaitForMultipleObjectsMainThread_WaitAll_CompactsHandlesUntilAllFinished()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        var h0 = rt.Wait.CreateManualResetEvent(false);
        var h1 = rt.Wait.CreateManualResetEvent(false);
        var h2 = rt.Wait.CreateManualResetEvent(false);
        var handles = new[] { h0, h1, h2 };
        rt.Wait.Scripted.Enqueue(OBJECT_0 + 1); // h1
        rt.Wait.Scripted.Enqueue(OBJECT_0 + 0); // h0
        rt.Wait.Scripted.Enqueue(OBJECT_0 + 0); // h2（此时槽位已压缩到 0）

        uint result = TAsyncCallMultiSync.WaitForMultipleObjectsMainThread(
            rt.Wait, rt.MainThread, rt.MainThread.RtlSyncEvent, pool.MainThreadSyncEvent,
            3, handles, true, 9, false, 0);

        // 差异断言：返回的是“第一个完成”的句柄下标，而不是最后一个
        Assert.Equal(OBJECT_0 + 1, result);

        Assert.Equal(3, rt.Wait.Calls.Count);
        Assert.Equal(new[] { h0.Handle, h1.Handle, h2.Handle, rt.MainThread.RtlSyncEvent.Handle, pool.MainThreadSyncEvent.Handle },
            rt.Wait.Calls[0].Handles);
        Assert.Equal(new[] { h0.Handle, h2.Handle, rt.MainThread.RtlSyncEvent.Handle, pool.MainThreadSyncEvent.Handle },
            rt.Wait.Calls[1].Handles);
        Assert.Equal(new[] { h2.Handle, rt.MainThread.RtlSyncEvent.Handle, pool.MainThreadSyncEvent.Handle },
            rt.Wait.Calls[2].Handles);
        Assert.All(rt.Wait.Calls, c => Assert.False(c.WaitAll)); // WaitAll 分支固定传 False
        Assert.All(rt.Wait.Calls, c => Assert.Equal(9u, c.Milliseconds));
    }

    [Fact]
    public void WaitForMultipleObjectsMainThread_WaitAll_TimeoutBreaksOutAndReturnsTimeout()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        var handles = new[] { rt.Wait.CreateManualResetEvent(false), rt.Wait.CreateManualResetEvent(false) };
        rt.Wait.DefaultResult = TIMEOUT;

        uint result = TAsyncCallMultiSync.WaitForMultipleObjectsMainThread(
            rt.Wait, rt.MainThread, rt.MainThread.RtlSyncEvent, pool.MainThreadSyncEvent,
            2, handles, true, 20, false, 0);

        Assert.Equal(TIMEOUT, result);
        Assert.Single(rt.Wait.Calls);
    }

    [Fact]
    public void WaitForMultipleObjectsMainThread_WaitAll_SyncEventsAreHandledThenLoopContinues()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        var h0 = rt.Wait.CreateManualResetEvent(false);
        var handles = new[] { h0 };
        rt.Wait.Scripted.Enqueue(OBJECT_0 + 1); // Count = 1 ⇒ RTL SyncEvent
        rt.Wait.Scripted.Enqueue(OBJECT_0 + 2); // 池 SyncEvent
        rt.Wait.Scripted.Enqueue(OBJECT_0 + 0); // h0 完成

        uint result = TAsyncCallMultiSync.WaitForMultipleObjectsMainThread(
            rt.Wait, rt.MainThread, rt.MainThread.RtlSyncEvent, pool.MainThreadSyncEvent,
            1, handles, true, 3, false, 0);

        Assert.Equal(OBJECT_0, result);
        Assert.Equal(1, rt.MainThread.CheckSynchronizeCount);
        Assert.Equal(1, rt.MainThread.ProcessMainThreadSyncCount);
        Assert.Equal(3, rt.Wait.Calls.Count);
        Assert.All(rt.Wait.Calls, c => Assert.Equal(3, c.Handles.Length));
    }

    [Fact]
    public void WaitForMultipleObjectsMainThread_WaitAll_MsgWakeReturnsOriginalCount()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        var handles = new[] { rt.Wait.CreateManualResetEvent(false) };
        rt.Wait.Scripted.Enqueue(OBJECT_0 + 3); // Count(1) + 2 ⇒ 消息

        uint result = TAsyncCallMultiSync.WaitForMultipleObjectsMainThread(
            rt.Wait, rt.MainThread, rt.MainThread.RtlSyncEvent, pool.MainThreadSyncEvent,
            1, handles, true, 5, true, AsyncCallsConst.QS_ALLINPUT);

        Assert.Equal(OBJECT_0 + 1, result);
        Assert.Equal(1, rt.MainThread.ProcessMainThreadSyncCount);
        // 差异断言：WaitAll = True 时原文把 MsgWaitForMultipleObjects 的 bWaitAll 硬编码为 False
        Assert.False(rt.Wait.Calls[0].WaitAll);
    }

    [Fact]
    public void WaitForMultipleObjectsMainThread_ZeroCount_WaitsOnlyOnSyncEvents()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        rt.Wait.Scripted.Enqueue(OBJECT_0 + 2); // Count = 0 ⇒ 池 SyncEvent 在槽 1，消息在槽 2

        uint result = TAsyncCallMultiSync.WaitForMultipleObjectsMainThread(
            rt.Wait, rt.MainThread, rt.MainThread.RtlSyncEvent, pool.MainThreadSyncEvent,
            0, Array.Empty<IAsyncWaitObject>(), false, 1, false, 0);

        // Count = 0 ⇒ 只有两个同步事件在等，任意结果都原样返回
        Assert.Equal(OBJECT_0 + 2, result);
        Assert.Equal(2, rt.Wait.Calls[0].Handles.Length);
        Assert.Equal(rt.MainThread.RtlSyncEvent.Handle, rt.Wait.Calls[0].Handles[0]);
        Assert.Equal(pool.MainThreadSyncEvent.Handle, rt.Wait.Calls[0].Handles[1]);
    }

    // ------------------------------------------------------------------
    // 对外四个函数的包装（原文 1641-1663）
    // ------------------------------------------------------------------

    [Fact]
    public void AsyncMultiSync_PassesEmptyHandleArrayAndNoMsgWait()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        var list = new List<IAsyncCall> { Fake(rt.Wait) };
        rt.Wait.Scripted.Enqueue(OBJECT_0);

        uint result = TAsyncCallMultiSync.AsyncMultiSync(rt.Runtime, pool, list, false, 400);

        Assert.Equal(OBJECT_0, result);
        Assert.False(rt.Wait.Calls[0].IsMsgWait);
        Assert.Equal(1, rt.Wait.Calls[0].Handles.Length);
        Assert.Equal(400u, rt.Wait.Calls[0].Milliseconds);
    }

    [Fact]
    public void AsyncMultiSyncEx_ForwardsExternalHandles()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        var handles = new[] { rt.Wait.CreateManualResetEvent(false) };
        rt.Wait.Scripted.Enqueue(OBJECT_0);

        uint result = TAsyncCallMultiSync.AsyncMultiSyncEx(
            rt.Runtime, pool, Array.Empty<IAsyncCall>(), handles, true, AsyncCallsConst.INFINITE);

        Assert.Equal(OBJECT_0, result);
        Assert.Equal(handles[0].Handle, rt.Wait.Calls[0].Handles[0]);
    }

    [Fact]
    public void MsgAsyncMultiSync_SetsMsgWaitFlagAndWakeMask()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        var list = new List<IAsyncCall> { Fake(rt.Wait) };
        rt.Wait.Scripted.Enqueue(OBJECT_0);

        uint result = TAsyncCallMultiSync.MsgAsyncMultiSync(
            rt.Runtime, pool, list, false, AsyncCallsConst.INFINITE,
            AsyncCallsConst.QS_ALLINPUT | AsyncCallsConst.QS_ALLPOSTMESSAGE);

        Assert.Equal(OBJECT_0, result);
        Assert.True(rt.Wait.Calls[0].IsMsgWait);
        Assert.Equal(AsyncCallsConst.QS_ALLINPUT | AsyncCallsConst.QS_ALLPOSTMESSAGE, rt.Wait.Calls[0].WakeMask);
    }

    [Fact]
    public void MsgAsyncMultiSyncEx_ForwardsHandlesAndMsgWait()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();
        var handles = new[] { rt.Wait.CreateManualResetEvent(false) };
        rt.Wait.Scripted.Enqueue(OBJECT_0);

        uint result = TAsyncCallMultiSync.MsgAsyncMultiSyncEx(
            rt.Runtime, pool, null, handles, false, 12, AsyncCallsConst.QS_ALLINPUT);

        Assert.Equal(OBJECT_0, result);
        Assert.True(rt.Wait.Calls[0].IsMsgWait);
        Assert.Single(rt.Wait.Calls[0].Handles);
    }

    [Fact]
    public void InternalAsyncMultiSync_NullInputs_AreTreatedAsEmpty()
    {
        var rt = new FakeRuntime();
        TThreadPool pool = rt.NewPool();

        uint result = TAsyncCallMultiSync.InternalAsyncMultiSync(
            rt.Runtime, pool, null, null, true, AsyncCallsConst.INFINITE, false, 0);

        Assert.Equal(AsyncCallsConst.WAIT_FAILED, result);
    }

    [Fact]
    public void InternalAsyncMultiSync_NullRuntime_Throws()
    {
        var rt = new FakeRuntime();

        Assert.Throws<ArgumentNullException>(() => TAsyncCallMultiSync.InternalAsyncMultiSync(
            null, rt.NewPool(), Array.Empty<IAsyncCall>(), Array.Empty<IAsyncWaitObject>(), true, 0, false, 0));
    }
}
