using System;
using GXX.Core.Async;
using Xunit;

namespace GXX.Core.Tests;

/// <summary>
/// 单元级门面（<c>AsyncCalls</c>）与 <c>TAsyncCallsFacade</c> 的测试
/// （原文 AsyncCalls.pas:1102-1406、1287-1299、326-3326）。
/// <para>
/// 每个入口都通过可选参数注入本地池/运行时，故此测试类<b>不</b>改动全局状态
/// （只有 <c>Globals_*</c> 用例例外，且集中在本类内串行执行）。
/// </para>
/// </summary>
public class AsyncCallFacadeTests
{
    private static FakeRuntime Rt() => new FakeRuntime();

    // ------------------------------------------------------------------
    // SetMaxAsyncCallThreads / GetMaxAsyncCallThreads（原文 1102-1113）
    // ------------------------------------------------------------------

    [Theory]
    [InlineData(300, 256)]
    [InlineData(256, 256)]
    [InlineData(7, 7)]
    [InlineData(0, 0)]
    [InlineData(-3, 14)] // 负数被忽略
    public void SetMaxAsyncCallThreads_ClampsThenIgnoresNegatives(int requested, int expected)
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4); // 默认 14

        AsyncCalls.SetMaxAsyncCallThreads(requested, pool);

        Assert.Equal(expected, AsyncCalls.GetMaxAsyncCallThreads(pool));
    }

    [Fact]
    public void GetMaxAsyncCallThreads_ReflectsPoolSetting()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);

        Assert.Equal(14, AsyncCalls.GetMaxAsyncCallThreads(pool));
        AsyncCalls.SetMaxAsyncCallThreads(3, pool);
        Assert.Equal(3, AsyncCalls.GetMaxAsyncCallThreads(pool));
    }

    // ------------------------------------------------------------------
    // MaxThreads = 0 ⇒ 就地同步执行（原文各重载的公共分支）
    // ------------------------------------------------------------------

    [Fact]
    public void AsyncCall_ObjectProc_WithZeroThreads_RunsSynchronously()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        pool.SetMaxThreads(0);
        object seen = null;

        IAsyncCall call = AsyncCalls.AsyncCall(
            (TAsyncCallArgObjectProc)(a => { seen = a; return 17; }), "arg", pool, rt.Runtime);

        Assert.IsType<TSyncCall>(call);
        Assert.Equal("arg", seen);
        Assert.Equal(17, call.Sync());
        Assert.Equal(0, pool.EnqueuedCallCount);
    }

    [Fact]
    public void AsyncCall_StringProc_WithZeroThreads_RunsSynchronously()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        pool.SetMaxThreads(0);
        string seen = null;

        IAsyncCall call = AsyncCalls.AsyncCall(
            (TAsyncCallArgStringProc)(s => { seen = s; return 3; }), "hello", pool, rt.Runtime);

        Assert.IsType<TSyncCall>(call);
        Assert.Equal("hello", seen);
        Assert.Equal(3, call.ReturnValue());
    }

    [Fact]
    public void AsyncCall_ExtendedProc_WithZeroThreads_RunsSynchronously()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        pool.SetMaxThreads(0);
        double seen = 0;

        IAsyncCall call = AsyncCalls.AsyncCall(
            (TAsyncCallArgExtendedProc)(d => { seen = d; return 1; }), 2.5, pool, rt.Runtime);

        Assert.IsType<TSyncCall>(call);
        Assert.Equal(2.5, seen);
    }

    [Fact]
    public void AsyncCall_WithPositiveThreads_EnqueuesTypedInternalCall()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);

        AsyncCalls.AsyncCall((TAsyncCallArgObjectProc)(a => 0), new object(), pool, rt.Runtime);
        Assert.IsType<TAsyncCallArgObject>(pool.AsyncCallHead);
        Assert.Equal(1, pool.EnqueuedCallCount);

        AsyncCalls.AsyncCall((TAsyncCallArgStringProc)(s => 0), "s", pool, rt.Runtime);
        Assert.IsType<TAsyncCallArgString>(pool.AsyncCallTail);

        AsyncCalls.AsyncCall((TAsyncCallArgWideStringProc)(s => 0), "s", pool, rt.Runtime);
        Assert.IsType<TAsyncCallArgWideString>(pool.AsyncCallTail);

        AsyncCalls.AsyncCall((TAsyncCallArgInterfaceProc)(o => 0), new object(), pool, rt.Runtime);
        Assert.IsType<TAsyncCallArgInterface>(pool.AsyncCallTail);

        AsyncCalls.AsyncCall((TAsyncCallArgExtendedProc)(d => 0), 1.0, pool, rt.Runtime);
        Assert.IsType<TAsyncCallArgExtended>(pool.AsyncCallTail);

        AsyncCalls.AsyncCallVar((TAsyncCallArgVariantProc)(o => 0), 1, pool, rt.Runtime);
        Assert.IsType<TAsyncCallArgVariant>(pool.AsyncCallTail);

        Assert.Equal(6, pool.EnqueuedCallCount);
        // 池策略：无睡眠线程时每次入队都扩容一个线程（替身不执行线程体，故无真实并发）
        Assert.Equal(6, rt.Launcher.Started.Count);
    }

    [Fact]
    public void AsyncCall_MethodOverloads_EnqueueMethodTypedCalls()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);

        AsyncCalls.AsyncCall((TAsyncCallArgObjectMethod)(a => 0), new object(), pool, rt.Runtime);
        Assert.IsType<TAsyncCallMethodArgObject>(pool.AsyncCallTail);

        AsyncCalls.AsyncCall((TAsyncCallArgStringMethod)(s => 0), "s", pool, rt.Runtime);
        Assert.IsType<TAsyncCallMethodArgString>(pool.AsyncCallTail);

        AsyncCalls.AsyncCall((TAsyncCallArgWideStringMethod)(s => 0), "s", pool, rt.Runtime);
        Assert.IsType<TAsyncCallMethodArgWideString>(pool.AsyncCallTail);

        AsyncCalls.AsyncCall((TAsyncCallArgInterfaceMethod)(o => 0), new object(), pool, rt.Runtime);
        Assert.IsType<TAsyncCallMethodArgInterface>(pool.AsyncCallTail);

        AsyncCalls.AsyncCall((TAsyncCallArgExtendedMethod)(d => 0), 1.0, pool, rt.Runtime);
        Assert.IsType<TAsyncCallMethodArgExtended>(pool.AsyncCallTail);

        AsyncCalls.AsyncCallVar((TAsyncCallArgVariantMethod)(o => 0), 1, pool, rt.Runtime);
        Assert.IsType<TAsyncCallMethodArgVariant>(pool.AsyncCallTail);
    }

    // ------------------------------------------------------------------
    // Integer 重载的特殊转调（原文 1126-1129、1187-1190）
    // ------------------------------------------------------------------

    [Fact]
    public void AsyncCall_IntegerProc_WithZeroThreads_PassesTheIntegerThrough()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        pool.SetMaxThreads(0);
        int seen = 0;

        IAsyncCall call = AsyncCalls.AsyncCall(
            (TAsyncCallArgIntegerProc)(i => { seen = i; return 5; }), 7, pool, rt.Runtime);

        Assert.IsType<TSyncCall>(call);
        Assert.Equal(7, seen);
        Assert.Equal(5, call.ReturnValue());
    }

    [Fact]
    public void AsyncCall_IntegerProc_QueuesObjectTypedCallNotAnIntegerVariant()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);

        AsyncCalls.AsyncCall((TAsyncCallArgIntegerProc)(i => i), 7, pool, rt.Runtime);

        // 差异断言：原文 1126-1129 把 Integer 塞进 TObject 槽转调 Object 重载，
        // 原文并不存在 TAsyncCallArgInteger 这个类。
        Assert.IsType<TAsyncCallArgObject>(pool.AsyncCallHead);
        Assert.Equal(1, pool.EnqueuedCallCount);
    }

    [Fact]
    public void AsyncCall_IntegerProc_WithZeroThreads_IsDrivenByObjectOverload()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        pool.SetMaxThreads(0);
        int seen = -1;

        // 若 Integer 重载自己判定了 MaxThreads，这里也会同步执行；关键是走的是 Object 分支
        AsyncCalls.AsyncCall((TAsyncCallArgIntegerProc)(i => { seen = i; return 0; }), -42, pool, rt.Runtime);

        Assert.Equal(-42, seen);
    }

    [Fact]
    public void AsyncCall_IntegerMethod_QueuesMethodObjectTypedCall()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);

        AsyncCalls.AsyncCall((TAsyncCallArgIntegerMethod)(i => i), 9, pool, rt.Runtime);

        Assert.IsType<TAsyncCallMethodArgObject>(pool.AsyncCallHead);
    }

    // ------------------------------------------------------------------
    // 事件重载（原文 1239-1272）
    // ------------------------------------------------------------------

    [Fact]
    public void AsyncCall_EventOverload_WithZeroThreads_InvokesEventAndReturnsZero()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        pool.SetMaxThreads(0);
        object seen = null;
        int assigned = 0;

        IAsyncCall call = AsyncCalls.AsyncCall(
            (TAsyncCallArgObjectEvent)(a => { seen = a; assigned++; }), "evt", pool, rt.Runtime);

        Assert.Equal(1, assigned);
        Assert.Equal("evt", seen);
        Assert.IsType<TSyncCall>(call);
        Assert.Equal(0, call.ReturnValue());
    }

    [Fact]
    public void AsyncCall_EventOverload_QueuesMethodTypedCall()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);

        AsyncCalls.AsyncCall((TAsyncCallArgIntegerEvent)(i => { }), 1, pool, rt.Runtime);

        Assert.IsType<TAsyncCallMethodArgObject>(pool.AsyncCallHead);
    }

    [Fact]
    public void AsyncCall_StringEventOverload_ReachesEvent()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        pool.SetMaxThreads(0);
        string seen = null;

        AsyncCalls.AsyncCall((TAsyncCallArgStringEvent)(s => seen = s), "abc", pool, rt.Runtime);

        Assert.Equal("abc", seen);
    }

    // ------------------------------------------------------------------
    // IAsyncRunnable（原文 1274-1283）
    // ------------------------------------------------------------------

    [Fact]
    public void AsyncCall_Runnable_InvokesAsyncRunOnce()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        pool.SetMaxThreads(0);
        var runnable = new ProbeRunnable();

        IAsyncCall call = AsyncCalls.AsyncCall(runnable, pool, rt.Runtime);

        Assert.Equal(1, runnable.RunCount);
        Assert.Equal(0, call.ReturnValue());
    }

    [Fact]
    public void AsyncCall_Runnable_QueuesInterfaceTypedCall()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);

        AsyncCalls.AsyncCall(new ProbeRunnable(), pool, rt.Runtime);

        // 原文 1280-1283：转调 Interface 重载
        Assert.IsType<TAsyncCallArgInterface>(pool.AsyncCallHead);
    }

    // ------------------------------------------------------------------
    // AsyncCallEx（原文 1354-1375）
    // ------------------------------------------------------------------

    [Fact]
    public void AsyncCallEx_RecordProc_WithZeroThreads_RunsSynchronously()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        pool.SetMaxThreads(0);
        var ptr = new IntPtr(0x1234);
        IntPtr seen = IntPtr.Zero;

        IAsyncCall call = AsyncCalls.AsyncCallEx(
            (TAsyncCallArgRecordProc)(p => { seen = p; return 64; }), ptr, pool, rt.Runtime);

        Assert.IsType<TSyncCall>(call);
        Assert.Equal(ptr, seen);
        Assert.Equal(64, call.ReturnValue());
    }

    [Fact]
    public void AsyncCallEx_RecordProc_QueuesRecordTypedCall()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);

        AsyncCalls.AsyncCallEx((TAsyncCallArgRecordProc)(p => 0), new IntPtr(1), pool, rt.Runtime);
        Assert.IsType<TAsyncCallArgRecord>(pool.AsyncCallHead);

        AsyncCalls.AsyncCallEx((TAsyncCallArgRecordMethod)(p => 0), new IntPtr(2), pool, rt.Runtime);
        Assert.IsType<TAsyncCallMethodArgRecord>(pool.AsyncCallTail);
    }

    [Fact]
    public void AsyncCallEx_RecordEvent_DelegatesToRecordMethod()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        pool.SetMaxThreads(0);
        IntPtr seen = IntPtr.Zero;

        IAsyncCall call = AsyncCalls.AsyncCallEx(
            (TAsyncCallArgRecordEvent)(p => seen = p), new IntPtr(0x77), pool, rt.Runtime);

        Assert.Equal(new IntPtr(0x77), seen);
        Assert.Equal(0, call.ReturnValue());
    }

    // ------------------------------------------------------------------
    // array of const 重载（原文 1380-1406）
    // ------------------------------------------------------------------

    [Fact]
    public void AsyncCall_VarArgs_WithZeroThreads_ReturnsRealWrapperNotSyncCall()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        pool.SetMaxThreads(0);
        rt.VarRec.Result = 42;
        var args = new[] { new TVarRec { VType = TVarRecType.vtInteger, VInteger = 5 } };

        IAsyncCall call = AsyncCalls.AsyncCall((nint)0x1000, args, pool, rt.Runtime);

        // 差异断言：与其它重载不同，原文此处走 InternExecuteSyncCall + TAsyncCall.Create，
        // 返回的不是 TSyncCall。
        Assert.IsType<TAsyncCall>(call);
        Assert.Equal(1, rt.VarRec.CallCount);
        Assert.Equal(42, call.Sync());
    }

    [Fact]
    public void AsyncCall_VarArgs_WithPositiveThreads_EnqueuesArrayOfConstCall()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        var args = new[] { new TVarRec { VType = TVarRecType.vtAnsiString, VAnsiString = "x" } };

        AsyncCalls.AsyncCall((nint)0x1000, args, pool, rt.Runtime);

        var queued = Assert.IsType<TAsyncCallArrayOfConst>(pool.AsyncCallHead);
        Assert.Single(queued.Args);
        Assert.Equal("x", queued.Args[0].VAnsiString);
    }

    [Fact]
    public void AsyncCall_VarArgs_MethodForm_InsertsSelfSlotOnTheQueuedCall()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        var methodData = new object();
        var args = new[] { new TVarRec { VType = TVarRecType.vtInteger, VInteger = 1 } };

        AsyncCalls.AsyncCall((nint)0x2000, methodData, args, pool, rt.Runtime);

        var queued = Assert.IsType<TAsyncCallArrayOfConst>(pool.AsyncCallHead);
        Assert.Equal(2, queued.Args.Length);
        Assert.Equal(TVarRecType.vtObject, queued.Args[0].VType);
        Assert.Same(methodData, queued.Args[0].VObject);
    }

    [Fact]
    public void AsyncCall_VarArgs_UnsupportedType_ThrowsFromTheSyncPath()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        pool.SetMaxThreads(0);
        var args = new[] { new TVarRec { VType = TVarRecType.vtUnicodeString, VPointer = "u" } };

        var ex = Assert.Throws<TAsyncCallError>(() => AsyncCalls.AsyncCall((nint)0x1000, args, pool, rt.Runtime));
        Assert.Contains("17", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void AsyncCall_VarArgs_InvokerSeamIsUsedWithProcAndMethodData()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        pool.SetMaxThreads(0);
        rt.VarRec.Result = 7;
        var methodData = new object();
        var args = new[] { new TVarRec { VType = TVarRecType.vtInteger, VInteger = 1 } };

        IAsyncCall call = AsyncCalls.AsyncCall((nint)0x9999, methodData, args, pool, rt.Runtime);

        Assert.Equal(new IntPtr(0x9999), rt.VarRec.LastProc);
        Assert.Same(methodData, rt.VarRec.LastMethodData);
        Assert.Equal(7, call.ReturnValue());
    }

    // ------------------------------------------------------------------
    // AsyncExec（原文 1287-1299）
    // ------------------------------------------------------------------

    [Fact]
    public void AsyncExec_WithIdleMethod_LoopsWhileMessageIsSignaled()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        rt.Wait.IsMainThread = true;
        // 两次“消息被置位”（返回 Count = 1），第三次返回已完成下标 0
        rt.Wait.Scripted.Enqueue(AsyncCallsConst.WAIT_OBJECT_0 + 3);
        rt.Wait.Scripted.Enqueue(AsyncCallsConst.WAIT_OBJECT_0 + 3);
        rt.Wait.Scripted.Enqueue(AsyncCallsConst.WAIT_OBJECT_0);
        int idleCalls = 0;

        AsyncCalls.AsyncExec(
            (TAsyncCallArgObjectEvent)(a => { }), "a",
            () => idleCalls++,
            pool, rt.Runtime);

        Assert.Equal(3, idleCalls); // 1 次进入循环前 + 2 次消息唤醒
        Assert.Equal(2, rt.MainThread.ProcessMainThreadSyncCount);
        // AsyncExec 会先要求换线程，避免在等待期间就地执行
        Assert.True(pool.AsyncCallHead.ForceDifferentThreadSet);
    }

    [Fact]
    public void AsyncExec_WithoutIdleMethod_DoesNoWaitingAtAll()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        rt.Wait.IsMainThread = true;

        AsyncCalls.AsyncExec((TAsyncCallArgObjectEvent)(a => { }), "a", null, pool, rt.Runtime);

        Assert.Empty(rt.Wait.Calls);
        Assert.Equal(1, pool.EnqueuedCallCount);
    }

    // ------------------------------------------------------------------
    // TAsyncCallsFacade.MsgExec（原文 3310-3324，D2009+ 分支）
    // ------------------------------------------------------------------

    [Fact]
    public void MsgExec_NotMainThread_FallsBackToSync()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        rt.Wait.IsMainThread = false;
        var call = new FakeAsyncCall(rt.Wait.CreateManualResetEvent(false));

        TAsyncCallsFacade.MsgExec(call, () => { }, pool, rt.Runtime);

        Assert.Equal(1, call.SyncCount);
        Assert.Equal(0, call.ForceDifferentThreadCount);
    }

    [Fact]
    public void MsgExec_MainThread_RunsIdleLoop()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        rt.Wait.IsMainThread = true;
        var call = new FakeAsyncCall(rt.Wait.CreateManualResetEvent(false));
        rt.Wait.Scripted.Enqueue(AsyncCallsConst.WAIT_OBJECT_0 + 3);
        rt.Wait.Scripted.Enqueue(AsyncCallsConst.WAIT_OBJECT_0);
        int idleCalls = 0;

        TAsyncCallsFacade.MsgExec(call, () => idleCalls++, pool, rt.Runtime);

        Assert.Equal(2, idleCalls);
        Assert.Equal(1, call.ForceDifferentThreadCount);
        Assert.Equal(0, call.SyncCount);
    }

    [Fact]
    public void MsgExec_MainThreadWithoutIdleMethod_DoesNothing()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        rt.Wait.IsMainThread = true;
        var call = new FakeAsyncCall(rt.Wait.CreateManualResetEvent(false));

        TAsyncCallsFacade.MsgExec(call, null, pool, rt.Runtime);

        Assert.Equal(0, call.SyncCount);
        Assert.Empty(rt.Wait.Calls);
    }

    // ------------------------------------------------------------------
    // TAsyncCallsFacade.VCLInvoke / VCLSync（原文 3292-3308、3271-3290，D2009+ 分支）
    // ------------------------------------------------------------------

    [Fact]
    public void VCLInvoke_MainThread_ExecutesInlineAndReportsFinished()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        rt.Wait.IsMainThread = true;
        int runs = 0;

        IAsyncCall call = TAsyncCallsFacade.VCLInvoke(() => runs++, pool, rt.Runtime);

        Assert.Equal(1, runs);
        Assert.IsType<TSyncCall>(call);
        Assert.True(call.Finished());
        Assert.Empty(rt.MainThread.Posted);
    }

    [Fact]
    public void VCLInvoke_NotMainThread_PostsToMainThreadPump()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        rt.Wait.IsMainThread = false;
        rt.MainThread.PostVclSyncResult = true;
        int runs = 0;

        IAsyncCall call = TAsyncCallsFacade.VCLInvoke(() => runs++, pool, rt.Runtime);

        Assert.Equal(0, runs);              // 移交消息泵，尚未执行
        Assert.Single(rt.MainThread.Posted);
        Assert.True(pool.MainThreadSyncEvent.IsSignaled);
        Assert.NotNull(((IAsyncCallEx)call).GetEvent());
    }

    [Fact]
    public void VCLInvoke_NotMainThreadWhenPostFails_QuitsWithZero()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        rt.Wait.IsMainThread = false;
        rt.MainThread.PostVclSyncResult = false;

        IAsyncCall call = TAsyncCallsFacade.VCLInvoke(() => { }, pool, rt.Runtime);

        Assert.True(call.Finished());
        Assert.Equal(0, call.ReturnValue());
    }

    [Fact]
    public void VCLInvoke_AfterPoolDestroyed_ThrowsNoVclSyncPossible()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        rt.Wait.IsMainThread = false;
        pool.Destroy();

        var ex = Assert.Throws<TAsyncCallError>(() => TAsyncCallsFacade.VCLInvoke(() => { }, pool, rt.Runtime));
        Assert.Equal(AsyncCallsConst.RsNoVclSyncPossible, ex.Message);
    }

    [Fact]
    public void VCLSync_MainThread_ExecutesInline()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        rt.Wait.IsMainThread = true;
        int runs = 0;

        TAsyncCallsFacade.VCLSync(() => runs++, pool, rt.Runtime);

        Assert.Equal(1, runs);
    }

    [Fact]
    public void VCLSync_NotMainThread_ReachesTheStaticSynchronizeSeam()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        rt.Wait.IsMainThread = false;

        // 接缝：StaticSynchronize（Classes.TThread.Synchronize）尚未移植
        Assert.Throws<NotSupportedException>(() => TAsyncCallsFacade.VCLSync(() => { }, pool, rt.Runtime));
    }

    [Fact]
    public void VCLSync_NotMainThreadOnDestroyedPool_ThrowsCheckDestroyingFirst()
    {
        var rt = Rt();
        TThreadPool pool = rt.NewPool(4);
        rt.Wait.IsMainThread = false;
        pool.Destroy();

        Assert.Throws<TAsyncCallError>(() => TAsyncCallsFacade.VCLSync(() => { }, pool, rt.Runtime));
    }

    // ------------------------------------------------------------------
    // 模块级全局量（原文 1100、3417-3436）
    // ------------------------------------------------------------------

    [Fact]
    public void Globals_ThreadPoolIsLazilyCreatedAndReplaceable()
    {
        TThreadPool original = AsyncCalls.ThreadPool;
        Assert.NotNull(original);

        var rt = Rt();
        TThreadPool replacement = rt.NewPool(2);
        try
        {
            AsyncCalls.ThreadPool = replacement;
            Assert.Same(replacement, AsyncCalls.ThreadPool);
            Assert.Equal(6, AsyncCalls.GetMaxAsyncCallThreads());
        }
        finally
        {
            AsyncCalls.ThreadPool = original;
        }
    }

    [Fact]
    public void Globals_FinalizeUnitDropsTheGlobalPool()
    {
        TThreadPool before = AsyncCalls.ThreadPool;

        AsyncCallsGlobals.FinalizeUnit();

        TThreadPool after = AsyncCalls.ThreadPool;
        Assert.NotNull(after);
        Assert.NotSame(before, after);
        Assert.Equal(0, after.ThreadCount);
    }
}
