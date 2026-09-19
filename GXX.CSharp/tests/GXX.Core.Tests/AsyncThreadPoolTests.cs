using System;
using System.Collections.Generic;
using System.Linq;
using GXX.Core.Async;
using Xunit;

namespace GXX.Core.Tests;

/// <summary>
/// <c>TThreadPool</c> 队列语义与线程分配策略测试（原文 AsyncCalls.pas:1751-1990）。
/// <para>线程经替身记录、等待经脚本化服务返回 —— 无真实线程、无真实时间。</para>
/// </summary>
public class AsyncThreadPoolTests
{
    private static TestAsyncCall NewCall(FakeRuntime rt, Func<int> body = null)
    {
        return new TestAsyncCall(body ?? (() => 0), rt.Runtime);
    }

    // ------------------------------------------------------------------
    // 构造与线程上限（原文 1751-1767、1102-1108）
    // ------------------------------------------------------------------

    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 6)]
    [InlineData(4, 14)]
    [InlineData(16, 62)]
    [InlineData(64, 254)]
    [InlineData(65, 256)]   // 65*4-2 = 258 → 钳制到 256
    [InlineData(100, 256)]
    [InlineData(256, 256)]
    public void ComputeDefaultMaxThreads_FollowsProcessorCountFormula(int processors, int expected)
    {
        Assert.Equal(expected, TThreadPool.ComputeDefaultMaxThreads(processors));
    }

    [Fact]
    public void ComputeDefaultMaxThreads_ZeroProcessors_IsNegativeWithoutLowerClamp()
    {
        // 差异断言：原文只钳上界（if FMaxThreads > Length(FThreads)），0 个处理器会得到 -2
        Assert.Equal(-2, TThreadPool.ComputeDefaultMaxThreads(0));
    }

    [Fact]
    public void Constructor_UsesInjectedProcessorCountAndExposesEvents()
    {
        var rt = new FakeRuntime();
        var pool = new TThreadPool(rt.Runtime, 8);

        Assert.Equal(8u, pool.NumberOfProcessors);
        Assert.Equal(30, pool.MaxThreads); // 8*4-2
        Assert.NotNull(pool.MainThreadSyncEvent);
        Assert.False(pool.Destroying);
        Assert.Equal(0, pool.ThreadCount);
        Assert.Equal(0, pool.EnqueuedCallCount);
    }

    [Theory]
    [InlineData(300, 256)]
    [InlineData(256, 256)]
    [InlineData(255, 255)]
    [InlineData(1, 1)]
    [InlineData(0, 0)]
    [InlineData(-1, 14)]   // 负数被忽略 ⇒ 保持构造值
    [InlineData(int.MinValue, 14)]
    public void SetMaxThreads_ClampsUpperBoundAndIgnoresNegative(int requested, int expected)
    {
        var rt = new FakeRuntime();
        var pool = new TThreadPool(rt.Runtime, 4); // 默认 14

        pool.SetMaxThreads(requested);

        Assert.Equal(expected, pool.MaxThreads);
    }

    // ------------------------------------------------------------------
    // 入队 / 出队（原文 1815-1911）
    // ------------------------------------------------------------------

    [Fact]
    public void AddAsyncCall_EnqueuesInFifoOrderAndTakesOneRef()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);
        TestAsyncCall a = NewCall(rt);
        TestAsyncCall b = NewCall(rt);

        pool.AddAsyncCall(a);
        pool.AddAsyncCall(b);

        Assert.Equal(2, pool.EnqueuedCallCount);
        Assert.Same(a, pool.AsyncCallHead);
        Assert.Same(b, pool.AsyncCallTail);
        Assert.Same(b, a.Next);
        Assert.Equal(1, a.RefCount);
        Assert.Equal(1, b.RefCount);
    }

    [Fact]
    public void AddAsyncCall_WithNoSleepingThreads_AllocatesOneThreadPerCall()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);

        pool.AddAsyncCall(NewCall(rt));
        Assert.Equal(1, pool.ThreadCount);

        pool.AddAsyncCall(NewCall(rt));
        Assert.Equal(2, pool.ThreadCount);

        Assert.Equal(new[] { 0, 1 }, rt.Launcher.Started.ToArray());
    }

    [Fact]
    public void AddAsyncCall_AtMaxThreads_SilentlyStopsAllocating()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(1); // MaxThreads = 2
        Assert.Equal(2, pool.MaxThreads);

        pool.AddAsyncCall(NewCall(rt));
        pool.AddAsyncCall(NewCall(rt));
        pool.AddAsyncCall(NewCall(rt)); // 超限

        // 差异断言：AllocThread 的 CAS 循环在 Index = FMaxThreads 退出且不建线程
        Assert.Equal(2, pool.ThreadCount);
        Assert.Equal(2, rt.Launcher.Started.Count);
        Assert.Equal(3, pool.EnqueuedCallCount);
    }

    [Fact]
    public void GetNextAsyncCall_DequeuesInFifoOrderAndDecrementsCount()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);
        TestAsyncCall a = NewCall(rt);
        TestAsyncCall b = NewCall(rt);
        TestAsyncCall c = NewCall(rt);
        pool.AddAsyncCall(a);
        pool.AddAsyncCall(b);
        pool.AddAsyncCall(c);

        Assert.Same(a, pool.GetNextAsyncCall());
        Assert.Equal(2, pool.EnqueuedCallCount);
        Assert.Same(b, pool.GetNextAsyncCall());
        Assert.Same(c, pool.GetNextAsyncCall());
        Assert.Equal(0, pool.EnqueuedCallCount);
        Assert.Null(pool.AsyncCallHead);
        Assert.Null(pool.AsyncCallTail);
    }

    [Fact]
    public void GetNextAsyncCall_OnEmptyQueue_SleepsOnWakeUpAndTerminateEvents()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);

        Assert.Null(pool.GetNextAsyncCall());

        WaitCallRecord call = Assert.Single(rt.Wait.Calls);
        Assert.Equal(2, call.Handles.Length);
        Assert.False(call.WaitAll);
        Assert.Equal(AsyncCallsConst.INFINITE, call.Milliseconds);
        Assert.Equal(0, pool.SleepingThreadCount); // 唤醒后计数复原
    }

    [Fact]
    public void GetNextAsyncCall_OnlySleepsWhenQueueIsEmpty()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);
        pool.AddAsyncCall(NewCall(rt));
        pool.AddAsyncCall(NewCall(rt));

        pool.GetNextAsyncCall(); // 队列仍有 1 项 ⇒ 不 Sleep
        Assert.Empty(rt.Wait.Calls);

        pool.GetNextAsyncCall(); // 最后一项
        Assert.Empty(rt.Wait.Calls);

        pool.GetNextAsyncCall(); // 空队列 ⇒ Sleep
        Assert.Single(rt.Wait.Calls);
    }

    [Fact]
    public void GetNextAsyncCall_TailIsFixedWhenLastItemLeaves()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);
        TestAsyncCall a = NewCall(rt);
        pool.AddAsyncCall(a);
        pool.GetNextAsyncCall();

        Assert.Null(pool.AsyncCallTail);

        TestAsyncCall b = NewCall(rt);
        pool.AddAsyncCall(b);

        Assert.Same(b, pool.AsyncCallHead);
        Assert.Same(b, pool.AsyncCallTail);
    }

    // ------------------------------------------------------------------
    // RemoveAsyncCall（原文 1842-1876）
    // ------------------------------------------------------------------

    [Fact]
    public void RemoveAsyncCall_Head_UnlinksAndReleases()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);
        TestAsyncCall a = NewCall(rt);
        TestAsyncCall b = NewCall(rt);
        pool.AddAsyncCall(a);
        pool.AddAsyncCall(b);

        Assert.True(pool.RemoveAsyncCall(a));

        Assert.Same(b, pool.AsyncCallHead);
        Assert.Same(b, pool.AsyncCallTail);
        Assert.Equal(1, pool.EnqueuedCallCount);
        Assert.Equal(0, a.RefCount);       // Release ⇒ 引用归零 ⇒ Destroy
        Assert.Null(a.GetEvent());
    }

    [Fact]
    public void RemoveAsyncCall_Middle_KeepsChainIntact()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);
        TestAsyncCall a = NewCall(rt);
        TestAsyncCall b = NewCall(rt);
        TestAsyncCall c = NewCall(rt);
        pool.AddAsyncCall(a);
        pool.AddAsyncCall(b);
        pool.AddAsyncCall(c);

        Assert.True(pool.RemoveAsyncCall(b));

        Assert.Same(a, pool.AsyncCallHead);
        Assert.Same(c, a.Next);
        Assert.Same(c, pool.AsyncCallTail);
        Assert.Equal(2, pool.EnqueuedCallCount);
        Assert.Equal(0, b.RefCount);
    }

    [Fact]
    public void RemoveAsyncCall_Tail_MovesTailBackToPrevious()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);
        TestAsyncCall a = NewCall(rt);
        TestAsyncCall b = NewCall(rt);
        pool.AddAsyncCall(a);
        pool.AddAsyncCall(b);

        Assert.True(pool.RemoveAsyncCall(b));

        Assert.Same(a, pool.AsyncCallHead);
        Assert.Same(a, pool.AsyncCallTail);
        Assert.Equal(1, pool.EnqueuedCallCount);
        Assert.Equal(1, a.RefCount); // a 未被释放
    }

    [Fact]
    public void RemoveAsyncCall_NotFound_ReturnsFalseAndKeepsRefCount()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);
        TestAsyncCall a = NewCall(rt);
        TestAsyncCall ghost = NewCall(rt);
        pool.AddAsyncCall(a);

        Assert.False(pool.RemoveAsyncCall(ghost));

        Assert.Equal(1, pool.EnqueuedCallCount);
        Assert.Equal(0, ghost.RefCount);  // 从未 AddRef，也未被 Release
        Assert.NotNull(ghost.GetEvent());
    }

    [Fact]
    public void RemoveAsyncCall_OnEmptyQueue_ReturnsFalse()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);

        Assert.False(pool.RemoveAsyncCall(NewCall(rt)));
    }

    // ------------------------------------------------------------------
    // AllocThread（原文 1913-1924）
    // ------------------------------------------------------------------

    [Fact]
    public void AllocThread_UsesSequentialIndicesUpToMaxThreads()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(1); // MaxThreads = 2

        pool.AllocThread();
        pool.AllocThread();
        pool.AllocThread(); // 超限

        Assert.Equal(2, pool.ThreadCount);
        Assert.Equal(new[] { 0, 1 }, rt.Launcher.Started.ToArray());
        Assert.NotNull(pool.ThreadSlot(0));
        Assert.NotNull(pool.ThreadSlot(1));
        Assert.Null(pool.ThreadSlot(2));
    }

    [Fact]
    public void AllocThread_WithZeroMaxThreads_NeverAllocates()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);
        pool.SetMaxThreads(0);

        pool.AllocThread();

        Assert.Equal(0, pool.ThreadCount);
        Assert.Empty(rt.Launcher.Started);
    }

    // ------------------------------------------------------------------
    // SendVclSync / CheckDestroying（原文 1809-1813、1926-1934）
    // ------------------------------------------------------------------

    [Fact]
    public void SendVclSync_PostSucceeds_SignalsMainThreadSyncEvent()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);
        rt.MainThread.PostVclSyncResult = true;
        TestAsyncCall call = NewCall(rt);

        pool.SendVclSync(call);

        Assert.Single(rt.MainThread.Posted);
        Assert.True(pool.MainThreadSyncEvent.IsSignaled);
        Assert.False(call.Finished());
    }

    [Fact]
    public void SendVclSync_PostFails_QuitsCallWithZero()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);
        rt.MainThread.PostVclSyncResult = false;
        TestAsyncCall call = NewCall(rt);

        pool.SendVclSync(call);

        // 原文 1931：if not PostMessage(...) then Call.Quit(0)
        Assert.True(call.Finished());
        Assert.Equal(0, call.ReturnValue());
        Assert.False(pool.MainThreadSyncEvent.IsSignaled);
    }

    [Fact]
    public void SendVclSync_AfterDestroy_ThrowsNoVclSyncPossible()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);
        pool.Destroy();

        var ex = Assert.Throws<TAsyncCallError>(() => pool.SendVclSync(NewCall(rt)));
        Assert.Equal(AsyncCallsConst.RsNoVclSyncPossible, ex.Message);
    }

    [Fact]
    public void CheckDestroying_OnlyThrowsWhileDestroying()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);

        pool.CheckDestroying(); // 不抛
        pool.Destroy();
        Assert.Throws<TAsyncCallError>(pool.CheckDestroying);
    }

    // ------------------------------------------------------------------
    // Destroy（原文 1769-1807）
    // ------------------------------------------------------------------

    [Fact]
    public void Destroy_DrainsQueueReleasingEveryCall()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);
        TestAsyncCall a = NewCall(rt);
        TestAsyncCall b = NewCall(rt);
        TestAsyncCall c = NewCall(rt);
        pool.AddAsyncCall(a);
        pool.AddAsyncCall(b);
        pool.AddAsyncCall(c);

        pool.Destroy();

        Assert.Null(pool.AsyncCallHead);
        Assert.Null(pool.AsyncCallTail);
        Assert.Equal(0, pool.EnqueuedCallCount);
        foreach (TestAsyncCall call in new[] { a, b, c })
        {
            Assert.Equal(0, call.RefCount);
            Assert.Null(call.GetEvent());
        }
    }

    [Fact]
    public void Destroy_FreezesMaxThreadsAtCurrentThreadCount()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);
        pool.AddAsyncCall(NewCall(rt));
        pool.AddAsyncCall(NewCall(rt));
        Assert.Equal(2, pool.ThreadCount);

        pool.Destroy();

        Assert.Equal(2, pool.MaxThreads);
        Assert.True(pool.Destroying);
    }

    [Fact]
    public void Destroy_IsIdempotent()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);

        pool.Destroy();
        pool.Destroy();

        Assert.True(pool.Destroying);
    }

    // ------------------------------------------------------------------
    // 线程主循环（原文 1694-1746）
    // ------------------------------------------------------------------

    [Fact]
    public void ThreadExecute_TerminatedWithEmptyQueue_ExitsAfterTwoSleeps()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);
        pool.Terminate();

        pool.ThreadExecute(); // 若不能终止，此测试会挂死

        Assert.Equal(2, rt.Wait.Calls.Count); // 两轮 GetNextAsyncCall 都 Sleep 了一次
        Assert.All(rt.Wait.Calls, c => Assert.Equal(2, c.Handles.Length));
    }

    [Fact]
    public void ThreadExecute_RunsQueuedCallThenReleasesIt()
    {
        int runs = 0;
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);
        TestAsyncCall call = NewCall(rt, () => { runs++; return 5; });
        pool.AddAsyncCall(call);
        pool.Terminate();

        pool.ThreadExecute();

        Assert.Equal(1, runs);
        Assert.Equal(5, call.ReturnValue());
        Assert.Equal(0, call.RefCount); // 执行后被 Release
    }

    [Fact]
    public void ThreadExecute_SwallowsBodyExceptions()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);
        TestAsyncCall call = NewCall(rt, () => throw new InvalidOperationException("worker"));
        pool.AddAsyncCall(call);
        pool.Terminate();

        pool.ThreadExecute(); // 原文 1718-1725 的 except 分支吞掉异常

        Assert.True(call.Finished());
    }

    // ------------------------------------------------------------------
    // 主线程窗口过程（原文 1959-1974）
    // ------------------------------------------------------------------

    [Fact]
    public void MainThreadWndProc_WmVclSync_ExecutesTheCallSynchronously()
    {
        int runs = 0;
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);
        TestAsyncCall call = NewCall(rt, () => { runs++; return 8; });

        pool.MainThreadWndProc(AsyncCallsConst.WM_VCLSYNC, call);

        Assert.Equal(1, runs);
        Assert.Equal(8, call.ReturnValue());
    }

    [Fact]
    public void MainThreadWndProc_WmRaiseException_Throws()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);

        Assert.Throws<TAsyncCallError>(() => pool.MainThreadWndProc(AsyncCallsConst.WM_RAISEEXCEPTION, NewCall(rt)));
    }

    [Fact]
    public void MainThreadWndProc_UnknownMessage_DoesNothing()
    {
        int runs = 0;
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);
        TestAsyncCall call = NewCall(rt, () => { runs++; return 1; });

        pool.MainThreadWndProc(0x7FFF, call);

        Assert.Equal(0, runs);
        Assert.False(call.Finished());
    }

    [Fact]
    public void MainThreadWndProc_WmVclSync_OnCancelledCall_MarksCanceled()
    {
        int runs = 0;
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);
        TestAsyncCall call = NewCall(rt, () => { runs++; return 1; });
        call.CancelInvocation();

        pool.MainThreadWndProc(AsyncCallsConst.WM_VCLSYNC, call);

        Assert.Equal(0, runs);
        Assert.True(call.Canceled());
    }

    // ------------------------------------------------------------------
    // ProcessMainThreadSync 接缝
    // ------------------------------------------------------------------

    [Fact]
    public void ProcessMainThreadSync_DelegatesToMainThreadSeam()
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(4);

        pool.ProcessMainThreadSync();

        Assert.Equal(1, rt.MainThread.ProcessMainThreadSyncCount);
    }

    [Fact]
    public void Constants_MatchOriginalDerivations()
    {
        Assert.Equal(64, AsyncCallsConst.MAXIMUM_WAIT_OBJECTS);
        Assert.Equal(61, AsyncCallsConst.MAXIMUM_ASYNC_WAIT_OBJECTS);
        Assert.Equal(256, AsyncCallsConst.ASYNC_CALL_THREAD_ARRAY_LENGTH);
        Assert.Equal(255, AsyncCallsConst.ASYNC_CALL_THREAD_ARRAY_HIGH);
        Assert.Equal(0x0400 + 12, AsyncCallsConst.WM_VCLSYNC);
        Assert.Equal(0x0400 + 13, AsyncCallsConst.WM_RAISEEXCEPTION);
    }

    [Fact]
    public void WaitFailedAndInfinite_ShareTheSameValue()
    {
        // 差异断言：Win32 中 WAIT_FAILED == INFINITE == 0xFFFFFFFF —— 二者数值不可区分。
        // 原文 InternalAsyncMultiSync 超限时返回 WAIT_FAILED，而超时默认值正是 INFINITE。
        Assert.Equal(AsyncCallsConst.INFINITE, AsyncCallsConst.WAIT_FAILED);
        Assert.NotEqual(AsyncCallsConst.WAIT_TIMEOUT, AsyncCallsConst.WAIT_FAILED);
        Assert.Equal(0x102u, AsyncCallsConst.WAIT_TIMEOUT);
        Assert.Equal(0x80u, AsyncCallsConst.WAIT_ABANDONED_0);
    }
}
