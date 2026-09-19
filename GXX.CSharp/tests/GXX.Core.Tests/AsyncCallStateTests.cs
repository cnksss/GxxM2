using System;
using System.Collections.Generic;
using GXX.Core.Async;
using Xunit;

namespace GXX.Core.Tests;

/// <summary>
/// <c>TInternalAsyncCall</c> / <c>TAsyncCall</c> / <c>TSyncCall</c> 状态机测试
/// （原文 AsyncCalls.pas:1991-2225、3332-3415）。
/// <para>全部等待由脚本化替身给定，无真实时间消耗。</para>
/// </summary>
public class AsyncCallStateTests
{
    private static (TestAsyncCall Call, FakeRuntime Rt, TThreadPool Pool) NewCall(Func<int> body = null,
        int processors = 4)
    {
        var rt = new FakeRuntime();
        var pool = rt.NewPool(processors);
        var call = new TestAsyncCall(body ?? (() => 0), rt.Runtime) { Pool = pool };
        return (call, rt, pool);
    }

    // ------------------------------------------------------------------
    // Finished（原文 2070-2076）
    // ------------------------------------------------------------------

    [Fact]
    public void Finished_BeforeExecutionWhenEventNotSignaled_IsFalse()
    {
        (TestAsyncCall call, _, _) = NewCall();

        Assert.False(call.Finished());
    }

    [Fact]
    public void Finished_AfterQuit_IsTrue()
    {
        (TestAsyncCall call, _, _) = NewCall();

        call.Quit(11);

        Assert.True(call.Finished());
        Assert.Equal(11, call.ReturnValue());
    }

    [Fact]
    public void Finished_AfterDestroyWhenEventIsZero_IsTrue()
    {
        (TestAsyncCall call, _, _) = NewCall();

        call.Destroy();

        Assert.Null(call.GetEvent());
        // 原文：Result := (FEvent = 0) or ...  ⇒ 事件句柄已关闭即视为完成
        Assert.True(call.Finished());
    }

    [Fact]
    public void Finished_CancelInvocationBeforeExecution_IsTrueEvenWithoutExecution()
    {
        (TestAsyncCall call, _, _) = NewCall();

        call.CancelInvocation();

        // 差异断言：FCancelInvocation and not FExecuted ⇒ 立刻视为“已完成”
        Assert.True(call.Finished());
        // 但此时 FCanceled 仍为 False（要等执行时才置位）
        Assert.False(call.Canceled());
        // 因此 ReturnValue 不抛异常，返回 0
        Assert.Equal(0, call.ReturnValue());
    }

    [Fact]
    public void Finished_EventSignaledByWaitService_IsTrue()
    {
        (TestAsyncCall call, FakeRuntime rt, _) = NewCall();

        // 模拟“另一线程已完成并置位事件”，但不设 FFinished
        rt.Wait.ZeroTimeoutResult = AsyncCallsConst.WAIT_OBJECT_0;

        Assert.True(call.Finished());
    }

    // ------------------------------------------------------------------
    // CancelInvocation / Canceled（原文 2088-2096、2112-2121）
    // ------------------------------------------------------------------

    [Fact]
    public void Canceled_DefaultsToFalse()
    {
        (TestAsyncCall call, _, _) = NewCall();

        Assert.False(call.Canceled());
    }

    [Fact]
    public void CancelInvocation_ThenAsyncExecution_SkipsBodyAndSetsCanceled()
    {
        int bodyRuns = 0;
        (TestAsyncCall call, _, _) = NewCall(() => { bodyRuns++; return 99; });

        call.CancelInvocation();
        call.InternExecuteAsyncCall();

        Assert.Equal(0, bodyRuns);
        Assert.True(call.Canceled());
        Assert.Equal(0, call.ReturnValue()); // 原文：取消后返回值为 0
        Assert.Equal(0, call.ExecuteCount);
    }

    [Fact]
    public void CancelInvocation_ThenSyncExecution_SkipsBodyAndSetsCanceled()
    {
        int bodyRuns = 0;
        (TestAsyncCall call, _, _) = NewCall(() => { bodyRuns++; return 5; });

        call.CancelInvocation();
        call.InternExecuteSyncCall();

        Assert.Equal(0, bodyRuns);
        Assert.True(call.Canceled());
        Assert.Equal(0, call.ReturnValue());
    }

    [Fact]
    public void CancelInvocation_AfterExecution_DoesNotChangeCanceled()
    {
        (TestAsyncCall call, _, _) = NewCall(() => 7);

        call.InternExecuteSyncCall();
        call.CancelInvocation();

        Assert.False(call.Canceled());   // 已执行 ⇒ 取消无效果
        Assert.Equal(7, call.ReturnValue());
    }

    // ------------------------------------------------------------------
    // ForceDifferentThread / Forget（原文 2078-2086）
    // ------------------------------------------------------------------

    [Fact]
    public void ForceDifferentThread_SetsFlag()
    {
        (TestAsyncCall call, _, _) = NewCall();

        Assert.False(call.ForceDifferentThreadSet);
        call.ForceDifferentThread();
        Assert.True(call.ForceDifferentThreadSet);
    }

    [Fact]
    public void Forget_OnInternalCall_IsExactlyForceDifferentThread()
    {
        (TestAsyncCall call, _, _) = NewCall();

        call.Forget();

        // 差异断言：原文 2083-2086 的 Forget 只是 ForceDifferentThread()，没有别的动作
        Assert.True(call.ForceDifferentThreadSet);
        Assert.False(call.Finished());
    }

    // ------------------------------------------------------------------
    // Quit / ReturnValue（原文 2148-2169）
    // ------------------------------------------------------------------

    [Fact]
    public void Quit_SetsFinishedAndReturnValueAndSignalsEvent()
    {
        (TestAsyncCall call, _, _) = NewCall();
        var ev = (FakeWaitObject)call.GetEvent();

        call.Quit(-1);

        Assert.True(call.Finished());
        Assert.Equal(-1, call.ReturnValue());
        Assert.True(ev.IsSignaled);
        Assert.Equal(1, ev.SetCount);
    }

    [Fact]
    public void Quit_ZeroAndIntMinValue_AreValidReturnValues()
    {
        (TestAsyncCall call, _, _) = NewCall();
        call.Quit(0);
        Assert.Equal(0, call.ReturnValue());

        (TestAsyncCall call2, _, _) = NewCall();
        call2.Quit(int.MinValue);
        Assert.Equal(int.MinValue, call2.ReturnValue());
    }

    [Fact]
    public void ReturnValue_BeforeFinished_ThrowsNotFinished()
    {
        (TestAsyncCall call, _, _) = NewCall();

        var ex = Assert.Throws<TAsyncCallError>(() => call.ReturnValue());
        Assert.Equal(AsyncCallsConst.RsAsyncCallNotFinished, ex.Message);
    }

    // ------------------------------------------------------------------
    // 异常暂存与重抛（原文 2119-2122、2163-2168、2191-2196）
    // ------------------------------------------------------------------

    [Fact]
    public void InternExecuteAsyncCall_BodyThrows_StoresExceptionAndQuitsWithZero()
    {
        var boom = new InvalidOperationException("boom");
        (TestAsyncCall call, _, _) = NewCall(() => throw boom);

        call.InternExecuteAsyncCall();

        Assert.True(call.Finished());
        // 原文：Value 保持 0，随后 Quit(Value)
        var ex = Assert.Throws<InvalidOperationException>(() => call.ReturnValue());
        Assert.Same(boom, ex);
    }

    [Fact]
    public void ReturnValue_SecondCallAfterRethrow_DoesNotThrowAgain()
    {
        var boom = new InvalidOperationException("boom");
        (TestAsyncCall call, _, _) = NewCall(() => throw boom);

        call.InternExecuteAsyncCall();

        Assert.Throws<InvalidOperationException>(() => call.ReturnValue());
        // 原文：重抛前把 FFatalException 置 nil ⇒ 第二次取值不再抛
        Assert.Equal(0, call.ReturnValue());
    }

    [Fact]
    public void Sync_BodyThrew_RethrowsStoredException()
    {
        var boom = new ApplicationException("bad");
        (TestAsyncCall call, _, _) = NewCall(() => throw boom);

        call.InternExecuteAsyncCall(); // 线程路径：异常被暂存

        Assert.Throws<ApplicationException>(() => call.Sync());
        // 差异断言：重抛前已把 FFatalException 置 nil ⇒ 第二次取值不再抛
        Assert.Equal(0, call.Sync());
    }

    [Fact]
    public void InternExecuteSyncCall_BodyThrows_PropagatesToCaller()
    {
        var boom = new ApplicationException("sync-boom");
        (TestAsyncCall call, _, _) = NewCall(() => throw boom);

        // 差异断言：Sync 执行路径不吞异常（finally Quit 后继续向外传播）
        var ex = Assert.Throws<ApplicationException>(() => call.InternExecuteSyncCall());
        Assert.Same(boom, ex);
        Assert.True(call.Finished()); // finally 里已完成 Quit
    }

    // ------------------------------------------------------------------
    // Sync（原文 2171-2197）
    // ------------------------------------------------------------------

    [Fact]
    public void Sync_AlreadyFinished_ReturnsValueWithoutWaiting()
    {
        (TestAsyncCall call, FakeRuntime rt, _) = NewCall(() => 31);

        call.InternExecuteSyncCall();
        int waitsBefore = rt.Wait.Calls.Count;

        Assert.Equal(31, call.Sync());
        Assert.Equal(waitsBefore, rt.Wait.Calls.Count); // 未产生新的等待
    }

    [Fact]
    public void Sync_NonMainThread_TimeoutThrowsNotFinished()
    {
        (TestAsyncCall call, FakeRuntime rt, _) = NewCall();
        rt.Wait.IsMainThread = false;
        rt.Wait.DefaultResult = AsyncCallsConst.WAIT_TIMEOUT;

        var ex = Assert.Throws<TAsyncCallError>(() => call.Sync());
        Assert.Equal(AsyncCallsConst.RsAsyncCallNotFinished, ex.Message);
    }

    [Fact]
    public void Sync_NonMainThread_Signaled_ReturnsValue()
    {
        (TestAsyncCall call, FakeRuntime rt, _) = NewCall();
        rt.Wait.IsMainThread = false;
        rt.Wait.DefaultResult = AsyncCallsConst.WAIT_OBJECT_0;

        Assert.Equal(0, call.Sync());
    }

    [Fact]
    public void Sync_MainThread_RoutesThroughMainThreadWaitWithSyncEvents()
    {
        (TestAsyncCall call, FakeRuntime rt, TThreadPool pool) = NewCall();
        rt.Wait.IsMainThread = true;
        // 第 1 次被 RTL 同步事件唤醒 → 执行 CheckSynchronize 后继续；第 2 次目标事件置位
        rt.Wait.Scripted.Enqueue(AsyncCallsConst.WAIT_OBJECT_0 + 1);
        rt.Wait.Scripted.Enqueue(AsyncCallsConst.WAIT_OBJECT_0);

        int result = call.Sync();

        Assert.Equal(0, result);
        Assert.Equal(1, rt.MainThread.CheckSynchronizeCount);
        Assert.Equal(0, rt.MainThread.ProcessMainThreadSyncCount);

        // Calls 里同时含 Finished() 的 0 毫秒探测；只看真正的阻塞等待
        var blocking = new List<WaitCallRecord>();
        foreach (WaitCallRecord c in rt.Wait.Calls)
            if (c.Milliseconds != 0)
                blocking.Add(c);

        Assert.Equal(2, blocking.Count);
        foreach (WaitCallRecord c in blocking)
        {
            Assert.Equal(3, c.Handles.Length);
            Assert.Equal(call.GetEvent().Handle, c.Handles[0]);
            Assert.Equal(rt.MainThread.RtlSyncEvent.Handle, c.Handles[1]);
            Assert.Equal(pool.MainThreadSyncEvent.Handle, c.Handles[2]);
            // 差异断言：原文不递减 Timeout，每次都用调用方给定的 INFINITE
            Assert.Equal(AsyncCallsConst.INFINITE, c.Milliseconds);
        }

        // 差异断言：未完成探测是“单句柄 + 0 毫秒”，且不消耗脚本
        Assert.Contains(rt.Wait.Calls, c => c.Milliseconds == 0 && c.Handles.Length == 1);
    }

    [Fact]
    public void Sync_MainThread_PoolSyncEventWake_CallsProcessMainThreadSync()
    {
        (TestAsyncCall call, FakeRuntime rt, _) = NewCall();
        rt.Wait.IsMainThread = true;
        rt.Wait.Scripted.Enqueue(AsyncCallsConst.WAIT_OBJECT_0 + 2);
        rt.Wait.Scripted.Enqueue(AsyncCallsConst.WAIT_OBJECT_0 + 1);
        rt.Wait.Scripted.Enqueue(AsyncCallsConst.WAIT_OBJECT_0);

        call.Sync();

        Assert.Equal(1, rt.MainThread.ProcessMainThreadSyncCount);
        Assert.Equal(1, rt.MainThread.CheckSynchronizeCount);
    }

    [Fact]
    public void Sync_MainThread_TimeoutThrowsNotFinished()
    {
        (TestAsyncCall call, FakeRuntime rt, _) = NewCall();
        rt.Wait.IsMainThread = true;
        rt.Wait.DefaultResult = AsyncCallsConst.WAIT_TIMEOUT;

        Assert.Throws<TAsyncCallError>(() => call.Sync());
    }

    // ------------------------------------------------------------------
    // SyncInThisThreadIfPossible（原文 2199-2219）
    // ------------------------------------------------------------------

    [Theory]
    [InlineData(false, false, true)]
    [InlineData(true, false, false)]
    [InlineData(true, true, true)]
    [InlineData(false, true, true)]
    public void ShouldTrySyncInThisThread_CoversAllCombinations(bool force, bool destroying, bool expected)
    {
        // 差异断言：池销毁态压过 Forget/ForceDifferentThread 标记
        Assert.Equal(expected, TInternalAsyncCall.ShouldTrySyncInThisThread(force, destroying));
    }

    [Fact]
    public void SyncInThisThreadIfPossible_AlreadyFinished_ReturnsTrueWithoutExecuting()
    {
        (TestAsyncCall call, _, _) = NewCall(() => 3);
        call.Quit(3);

        Assert.True(call.SyncInThisThreadIfPossible());
        Assert.Equal(0, call.ExecuteCount);
    }

    [Fact]
    public void SyncInThisThreadIfPossible_QueuedCall_RemovesAndExecutesInThisThread()
    {
        int bodyRuns = 0;
        (TestAsyncCall call, FakeRuntime rt, TThreadPool pool) = NewCall(() => { bodyRuns++; return 21; });

        call.ExecuteAsync(pool);
        Assert.Equal(1, pool.EnqueuedCallCount);

        bool executed = call.SyncInThisThreadIfPossible();

        Assert.True(executed);
        Assert.Equal(1, bodyRuns);                  // 就地执行
        Assert.Equal(21, call.ReturnValue());
        Assert.Equal(0, pool.EnqueuedCallCount);    // 已从队列摘除
        // 原文 AddAsyncCall 在“无睡眠线程”时会先扩容一个线程（池策略），但替身不会真正跑线程体
        Assert.Single(rt.Launcher.Started);
        Assert.Equal(0, rt.Wait.Calls.Count(c => c.Milliseconds != 0));
    }

    [Fact]
    public void SyncInThisThreadIfPossible_NotQueued_ReturnsFalse()
    {
        (TestAsyncCall call, _, _) = NewCall();

        Assert.False(call.SyncInThisThreadIfPossible());
        Assert.Equal(0, call.ExecuteCount);
    }

    [Fact]
    public void SyncInThisThreadIfPossible_ForcedDifferentThread_DoesNotRunInPlace()
    {
        (TestAsyncCall call, _, TThreadPool pool) = NewCall(() => 1);
        call.ExecuteAsync(pool);
        call.ForceDifferentThread();

        Assert.False(call.SyncInThisThreadIfPossible());
        Assert.Equal(1, pool.EnqueuedCallCount); // 仍在队列里等真正的线程
    }

    // ------------------------------------------------------------------
    // 引用计数（原文 2059-2068）
    // ------------------------------------------------------------------

    [Fact]
    public void AddRefRelease_DestroysExactlyAtZero()
    {
        (TestAsyncCall call, _, _) = NewCall();

        call.AddRef();
        Assert.Equal(1, call.RefCount);

        call.Release();
        Assert.Equal(0, call.RefCount);
        Assert.Null(call.GetEvent()); // Destroy 已关闭事件
    }

    [Fact]
    public void Release_BelowZero_DoesNotDestroyAgain()
    {
        (TestAsyncCall call, _, _) = NewCall();

        call.Release(); // 0 -> -1
        call.Release(); // -1 -> -2

        Assert.Equal(-2, call.RefCount);
        Assert.NotNull(call.GetEvent()); // 未在 0 处触发 Destroy
    }

    // ------------------------------------------------------------------
    // ExecuteAsync（原文 2221-2225）
    // ------------------------------------------------------------------

    [Fact]
    public void ExecuteAsync_EnqueuesAndReturnsWrapperHoldingTwoRefs()
    {
        (TestAsyncCall call, FakeRuntime rt, TThreadPool pool) = NewCall();

        TAsyncCall wrapper = call.ExecuteAsync(pool);

        Assert.NotNull(wrapper);
        Assert.Equal(2, call.RefCount);      // TAsyncCall.Create + AddAsyncCall
        Assert.Equal(1, pool.EnqueuedCallCount);
        Assert.Same(call, pool.AsyncCallHead);
        Assert.Same(call, pool.AsyncCallTail);
        Assert.Equal(1, pool.ThreadCount);   // 无睡眠线程 ⇒ 立刻扩容一个
        Assert.Single(rt.Launcher.Started);
    }

    // ------------------------------------------------------------------
    // Destroy 的致命异常移交（原文 2048-2055）
    // ------------------------------------------------------------------

    [Fact]
    public void Destroy_WithPendingFatalException_PostsToMainThread()
    {
        (TestAsyncCall call, FakeRuntime rt, _) = NewCall(() => throw new InvalidOperationException("x"));

        call.InternExecuteAsyncCall();
        call.Destroy();

        Assert.Single(rt.MainThread.Posted);
        Assert.Same(call, rt.MainThread.Posted[0]);
    }

    [Fact]
    public void Destroy_WithoutFatalException_DoesNotPost()
    {
        (TestAsyncCall call, FakeRuntime rt, _) = NewCall(() => 1);

        call.InternExecuteAsyncCall();
        call.Destroy();

        Assert.Empty(rt.MainThread.Posted);
    }

    [Fact]
    public void Destroy_IsIdempotent()
    {
        (TestAsyncCall call, _, _) = NewCall();

        call.Destroy();
        call.Destroy();

        Assert.Null(call.GetEvent());
    }

    // ------------------------------------------------------------------
    // GetEvent（原文 2098-2101）
    // ------------------------------------------------------------------

    [Fact]
    public void GetEvent_IsStableUntilDestroy()
    {
        (TestAsyncCall call, _, _) = NewCall();

        IAsyncWaitObject first = call.GetEvent();
        Assert.NotNull(first);
        Assert.Same(first, call.GetEvent());

        call.Destroy();
        Assert.Null(call.GetEvent());
    }

    // ------------------------------------------------------------------
    // TAsyncCall 外壳（原文 3332-3415）
    // ------------------------------------------------------------------

    [Fact]
    public void Wrapper_SyncAndReturnValue_DelegateToInternalCall()
    {
        (TestAsyncCall call, _, _) = NewCall(() => 77);
        var wrapper = new TAsyncCall(call);

        Assert.False(wrapper.Finished());
        call.InternExecuteSyncCall();
        Assert.True(wrapper.Finished());
        Assert.Equal(77, wrapper.Sync());
        Assert.Equal(77, wrapper.ReturnValue());
        Assert.False(wrapper.Canceled());
        Assert.NotNull(wrapper.GetEvent());

        wrapper.Destroy();
    }

    [Fact]
    public void Wrapper_CancelInvocation_ReachesInternalCall()
    {
        (TestAsyncCall call, _, _) = NewCall(() => 9);
        var wrapper = new TAsyncCall(call);

        wrapper.CancelInvocation();
        call.InternExecuteSyncCall();

        Assert.True(wrapper.Canceled());
        Assert.Equal(0, wrapper.Sync());
    }

    [Fact]
    public void Wrapper_ForceDifferentThread_ReachesInternalCall()
    {
        (TestAsyncCall call, _, _) = NewCall();
        var wrapper = new TAsyncCall(call);

        wrapper.ForceDifferentThread();

        Assert.True(call.ForceDifferentThreadSet);
    }

    [Fact]
    public void Wrapper_Forget_DisconnectsAndReleasesOneRef()
    {
        (TestAsyncCall call, _, _) = NewCall(() => 4);
        var wrapper = new TAsyncCall(call);
        int before = call.RefCount;

        wrapper.Forget();

        Assert.False(wrapper.IsConnected);
        Assert.Equal(before - 1, call.RefCount);
        Assert.True(call.ForceDifferentThreadSet); // Forget 内部仍走 ForceDifferentThread
    }

    [Theory]
    [InlineData("sync")]
    [InlineData("finished")]
    [InlineData("returnvalue")]
    [InlineData("canceled")]
    [InlineData("forcedifferentthread")]
    [InlineData("cancelinvocation")]
    [InlineData("forget")]
    [InlineData("getevent")]
    public void Wrapper_AfterForget_EveryMemberThrows(string member)
    {
        (TestAsyncCall call, _, _) = NewCall();
        var wrapper = new TAsyncCall(call);
        wrapper.Forget();

        Action act = member switch
        {
            "sync" => () => wrapper.Sync(),
            "finished" => () => wrapper.Finished(),
            "returnvalue" => () => wrapper.ReturnValue(),
            "canceled" => () => wrapper.Canceled(),
            "forcedifferentthread" => wrapper.ForceDifferentThread,
            "cancelinvocation" => wrapper.CancelInvocation,
            "forget" => wrapper.Forget,
            "getevent" => () => wrapper.GetEvent(),
            _ => throw new ArgumentOutOfRangeException(nameof(member)),
        };

        var ex = Assert.Throws<TAsyncCallError>(act);
        Assert.Equal(AsyncCallsConst.RsForgetWasCalled, ex.Message);
    }

    [Fact]
    public void Wrapper_ForgetThenDestroy_DoesNotTouchInternalCall()
    {
        (TestAsyncCall call, _, _) = NewCall();
        call.AddRef(); // 让 Forget 的 Release 之后仍有 1 个引用存活
        var wrapper = new TAsyncCall(call);
        Assert.Equal(2, call.RefCount);

        wrapper.Forget();
        Assert.Equal(1, call.RefCount); // Forget 释放了壳的那一份

        wrapper.Destroy(); // 原文：FCall 已为 nil ⇒ 直接 inherited Destroy，不再 Sync/Release

        Assert.False(wrapper.IsConnected);
        Assert.Equal(1, call.RefCount);
        Assert.NotNull(call.GetEvent());     // 内部调用未被销毁
        Assert.Equal(0, call.ExecuteCount);  // 也未被就地执行
    }

    [Fact]
    public void Wrapper_DestroyWithoutForget_SyncsAndReleases()
    {
        (TestAsyncCall call, FakeRuntime rt, _) = NewCall();
        rt.Wait.DefaultResult = AsyncCallsConst.WAIT_OBJECT_0; // 模拟异步完成信号
        var wrapper = new TAsyncCall(call);

        wrapper.Destroy();

        Assert.Equal(0, call.RefCount);
    }

    [Fact]
    public void Wrapper_SyncInThisThreadIfPossible_Delegates()
    {
        (TestAsyncCall call, _, TThreadPool pool) = NewCall(() => 12);
        call.ExecuteAsync(pool);
        var wrapper = new TAsyncCall(call);

        Assert.True(wrapper.SyncInThisThreadIfPossible());
        Assert.Equal(12, wrapper.ReturnValue());
    }

    // ------------------------------------------------------------------
    // TSyncCall（原文 1991-2027）
    // ------------------------------------------------------------------

    [Theory]
    [InlineData(0)]
    [InlineData(42)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void SyncCall_ReportsValueAndFinished(int value)
    {
        var sync = new TSyncCall(value);

        Assert.True(sync.Finished());
        Assert.Equal(value, sync.Sync());
        Assert.Equal(value, sync.ReturnValue());
        Assert.False(sync.Canceled());
    }

    [Fact]
    public void SyncCall_NoOpsDoNotThrowOrChangeState()
    {
        var sync = new TSyncCall(5);

        sync.ForceDifferentThread();
        sync.CancelInvocation();
        sync.Forget();

        Assert.True(sync.Finished());
        Assert.Equal(5, sync.Sync());
        Assert.False(sync.Canceled());
    }
}
