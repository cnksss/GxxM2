using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GXX.Core.Async;

namespace GXX.Core.Tests;

/// <summary>
/// AsyncCalls 移植的测试替身集合。
/// <para>
/// 全部替身都不消耗真实时间：等待结果由脚本给定，线程由记录器代劳，
/// 因此断言中不出现 <c>Thread.Sleep</c> / <c>Stopwatch</c>。
/// </para>
/// </summary>
internal sealed class FakeWaitObject : IAsyncWaitObject
{
    public FakeWaitObject(nint handle, bool initial = false)
    {
        Handle = handle;
        IsSignaled = initial;
    }

    public nint Handle { get; }

    public bool IsSignaled { get; private set; }

    public int SetCount { get; private set; }

    public int ResetCount { get; private set; }

    public void Set()
    {
        IsSignaled = true;
        SetCount++;
    }

    public void Reset()
    {
        IsSignaled = false;
        ResetCount++;
    }
}

/// <summary>一次等待调用的记录（用于断言句柄数组与超时是否被改写）。</summary>
internal sealed class WaitCallRecord
{
    public nint[] Handles;
    public bool WaitAll;
    public uint Milliseconds;
    public bool IsMsgWait;
    public uint WakeMask;

    public override string ToString()
        => $"{(IsMsgWait ? "Msg" : "Wait")}([{string.Join(",", Handles.Select(h => "0x" + h.ToString("X")))}], all={WaitAll}, ms={Milliseconds})";
}

/// <summary>
/// 脚本化等待服务：按 <see cref="Scripted"/> 顺序返回结果，耗尽后用 <see cref="DefaultResult"/>。
/// </summary>
internal sealed class FakeWaitService : IAsyncWaitService
{
    private long _nextHandle = 0x100;

    public bool IsMainThread { get; set; }

    public List<WaitCallRecord> Calls { get; } = new List<WaitCallRecord>();

    public Queue<uint> Scripted { get; } = new Queue<uint>();

    public uint DefaultResult { get; set; } = AsyncCallsConst.WAIT_TIMEOUT;

    /// <summary>
    /// 毫秒数为 0 的轮询（对应 <c>WaitForSingleObject(Handle, 0)</c>，即 <c>Finished()</c> 的探测）
    /// 不消耗 <see cref="Scripted"/>，直接返回本值。
    /// </summary>
    public uint ZeroTimeoutResult { get; set; } = AsyncCallsConst.WAIT_TIMEOUT;

    public IAsyncWaitObject CreateManualResetEvent(bool initialState)
    {
        return new FakeWaitObject((nint)Interlocked.Increment(ref _nextHandle), initialState);
    }

    public WaitCallRecord LastCall => Calls.Count == 0 ? null : Calls[Calls.Count - 1];

    public uint WaitForMultipleObjects(IAsyncWaitObject[] handles, bool waitAll, uint milliseconds)
    {
        Calls.Add(Record(handles, waitAll, milliseconds, false, 0));
        return Next(milliseconds);
    }

    public uint MsgWaitForMultipleObjects(IAsyncWaitObject[] handles, bool waitAll, uint milliseconds, uint wakeMask)
    {
        Calls.Add(Record(handles, waitAll, milliseconds, true, wakeMask));
        return Next(milliseconds);
    }

    private static WaitCallRecord Record(IAsyncWaitObject[] handles, bool waitAll, uint ms, bool msg, uint mask)
    {
        return new WaitCallRecord
        {
            Handles = handles == null ? Array.Empty<nint>() : handles.Select(h => h == null ? (nint)0 : h.Handle).ToArray(),
            WaitAll = waitAll,
            Milliseconds = ms,
            IsMsgWait = msg,
            WakeMask = mask,
        };
    }

    private uint Next(uint milliseconds)
    {
        if (milliseconds == 0)
            return ZeroTimeoutResult;
        return Scripted.Count > 0 ? Scripted.Dequeue() : DefaultResult;
    }
}

/// <summary>记录启动请求的线程启动器（不真正创建线程）。</summary>
internal sealed class FakeThreadLauncher : IAsyncThreadLauncher
{
    public List<int> Started { get; } = new List<int>();

    public List<Action> Bodies { get; } = new List<Action>();

    /// <summary>若为 true，<see cref="StartThread"/> 立即<b>同步</b>执行线程体（用于测线程主循环）。</summary>
    public bool RunInline { get; set; }

    public void StartThread(int index, Action body)
    {
        Started.Add(index);
        Bodies.Add(body);
        if (RunInline)
            body();
    }
}

/// <summary>记录同步调用的主线程派发替身。</summary>
internal sealed class FakeMainThreadDispatch : IMainThreadDispatch
{
    public FakeMainThreadDispatch(IAsyncWaitObject rtlSyncEvent)
    {
        RtlSyncEvent = rtlSyncEvent;
    }

    public IAsyncWaitObject RtlSyncEvent { get; }

    public int CheckSynchronizeCount { get; private set; }

    public int ProcessMainThreadSyncCount { get; private set; }

    /// <summary>对应 <c>PostMessage</c> 是否成功。</summary>
    public bool PostVclSyncResult { get; set; }

    public List<TInternalAsyncCall> Posted { get; } = new List<TInternalAsyncCall>();

    public void CheckSynchronize() => CheckSynchronizeCount++;

    public void ProcessMainThreadSync() => ProcessMainThreadSyncCount++;

    public bool PostVclSync(TInternalAsyncCall call)
    {
        Posted.Add(call);
        return PostVclSyncResult;
    }
}

/// <summary>记录变参调用请求的接缝替身。</summary>
internal sealed class FakeVarRecInvoker : IVarRecInvoker
{
    public int Result { get; set; }

    public int CallCount { get; private set; }

    public nint LastProc { get; private set; }

    public object LastMethodData { get; private set; }

    public TVarRec[] LastArgs { get; private set; }

    public List<Exception> ObservedErrors { get; } = new List<Exception>();

    public int Invoke(nint Proc, object MethodData, TVarRec[] Args)
    {
        CallCount++;
        LastProc = Proc;
        LastMethodData = MethodData;
        LastArgs = Args;
        return Result;
    }
}

/// <summary>把四个接缝打包成运行时，便于测试统一构造。</summary>
internal sealed class FakeRuntime
{
    public FakeWaitService Wait = new FakeWaitService();
    public FakeThreadLauncher Launcher = new FakeThreadLauncher();
    public FakeMainThreadDispatch MainThread;
    public FakeVarRecInvoker VarRec = new FakeVarRecInvoker();

    private TAsyncCallRuntime _runtime;

    public FakeRuntime()
    {
        MainThread = new FakeMainThreadDispatch(Wait.CreateManualResetEvent(false));
    }

    public TAsyncCallRuntime Runtime => _runtime ??= new TAsyncCallRuntime(Wait, Launcher, MainThread, VarRec);

    public TThreadPool NewPool(int processors = 4) => new TThreadPool(Runtime, processors);
}

/// <summary>最小的 <see cref="TInternalAsyncCall"/> 测试实现（函数体可注入）。</summary>
internal sealed class TestAsyncCall : TInternalAsyncCall
{
    private readonly Func<int> _body;

    public TestAsyncCall(Func<int> body, TAsyncCallRuntime runtime) : base(runtime)
    {
        _body = body;
        CreateEvent();
    }

    public int ExecuteCount { get; private set; }

    protected override int ExecuteAsyncCall()
    {
        ExecuteCount++;
        return _body();
    }
}

/// <summary>完整实现 <see cref="IAsyncCall"/> + <see cref="IAsyncCallEx"/> 的假异步调用。</summary>
internal sealed class FakeAsyncCall : IAsyncCall, IAsyncCallEx
{
    public FakeAsyncCall(IAsyncWaitObject ev)
    {
        Event = ev;
    }

    public IAsyncWaitObject Event { get; set; }

    public int SyncCount { get; private set; }

    public bool SyncResult { get; set; }

    public int ReturnValueValue { get; set; }

    public bool SyncInThisThreadResult { get; set; }

    public int SyncInThisThreadCount { get; private set; }

    public int ForceDifferentThreadCount { get; private set; }

    public int CancelInvocationCount { get; private set; }

    public int ForgetCount { get; private set; }

    public int Sync()
    {
        SyncCount++;
        return ReturnValueValue;
    }

    public bool Finished() => true;

    public int ReturnValue() => ReturnValueValue;

    public bool Canceled() => false;

    public void ForceDifferentThread() => ForceDifferentThreadCount++;

    public void CancelInvocation() => CancelInvocationCount++;

    public void Forget() => ForgetCount++;

    public IAsyncWaitObject GetEvent() => Event;

    public bool SyncInThisThreadIfPossible()
    {
        SyncInThisThreadCount++;
        return SyncInThisThreadResult;
    }
}

/// <summary>只实现 <see cref="IAsyncCall"/>（<b>不</b>实现 <see cref="IAsyncCallEx"/>）—— 触发短路分支。</summary>
internal sealed class PlainAsyncCall : IAsyncCall
{
    public int SyncCount { get; private set; }

    public int Sync()
    {
        SyncCount++;
        return 0;
    }

    public bool Finished() => true;

    public int ReturnValue() => 0;

    public bool Canceled() => false;

    public void ForceDifferentThread()
    {
    }

    public void CancelInvocation()
    {
    }

    public void Forget()
    {
    }
}

/// <summary>用于 IAsyncRunnable 重载的探针。</summary>
internal sealed class ProbeRunnable : IAsyncRunnable
{
    public int RunCount { get; private set; }

    public void AsyncRun() => RunCount++;
}
