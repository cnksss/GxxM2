using System;
using System.Collections.Generic;
using System.Threading;

// 源单元：Source/M2Engine/AsyncCalls.pas（同源副本：Source/Client-HGE/AsyncCalls.pas、Source/RunGate/AsyncCalls.pas）

namespace GXX.Core.Async;

/// <summary>
/// 可等待对象的托管替身：对应原文的 Win32 <c>THandle</c>（自动重置/手动重置事件）。
/// <para>
/// 原文用 <c>CreateEvent(nil, True, False, nil)</c> 创建<b>手动重置</b>事件，
/// 由 <c>GetEvent</c> 暴露给等待方；<c>null</c>/<see cref="AsyncCallsConst.WAIT_OBJECT_0"/> 之外的
/// 0 句柄在托管侧即 <c>null</c>。
/// </para>
/// </summary>
public interface IAsyncWaitObject
{
    /// <summary>句柄标识（对应原文 THandle 的数值身份；仅用于诊断/断言）。</summary>
    nint Handle { get; }

    /// <summary>是否已置位（对应 <c>WaitForSingleObject(Handle, 0) = WAIT_OBJECT_0</c>）。</summary>
    bool IsSignaled { get; }

    /// <summary>对应 <c>SetEvent</c>。</summary>
    void Set();

    /// <summary>对应 <c>ResetEvent</c>（原文未直接调用，保留以完整覆盖 Win32 事件语义）。</summary>
    void Reset();
}

/// <summary>
/// 等待服务的可注入替身（<b>时间可注入点</b>）。
/// <para>
/// 原文直接调用 Win32 <c>WaitForMultipleObjects</c> / <c>MsgWaitForMultipleObjects</c>。
/// 移植后把这些调用收敛到本接口：测试用脚本化替身在<b>不消耗真实时间</b>的前提下
/// 精确复现 WAIT_TIMEOUT / 同步事件唤醒 / 消息唤醒等分支，
/// 因此断言里不出现 <c>Thread.Sleep</c> / <c>Stopwatch</c>。
/// </para>
/// </summary>
public interface IAsyncWaitService
{
    /// <summary>对应 <c>GetCurrentThreadId = MainThreadId</c>。</summary>
    bool IsMainThread { get; }

    /// <summary>对应 <c>CreateEvent(nil, True, False, nil)</c>（手动重置、初始未置位）。</summary>
    IAsyncWaitObject CreateManualResetEvent(bool initialState);

    /// <summary>
    /// 对应 Win32 <c>WaitForMultipleObjects</c>，返回 <c>WAIT_OBJECT_0 + index</c> /
    /// <c>WAIT_OBJECT_0</c>（waitAll）/ <c>WAIT_TIMEOUT</c> / <c>WAIT_FAILED</c>。
    /// </summary>
    uint WaitForMultipleObjects(IAsyncWaitObject[] handles, bool waitAll, uint milliseconds);

    /// <summary>
    /// 对应 Win32 <c>MsgWaitForMultipleObjects</c>：消息到达时返回
    /// <c>WAIT_OBJECT_0 + handles.Length</c>。
    /// </summary>
    uint MsgWaitForMultipleObjects(IAsyncWaitObject[] handles, bool waitAll, uint milliseconds, uint wakeMask);
}

/// <summary>
/// 池化线程启动的可注入替身。
/// <para>
/// 原文 <c>TThreadPool.AllocThread</c> 构造 <c>TAsyncCallThread</c>（真正的 OS 线程）。
/// 测试用替身只记录“应启动第几号线程”，从而在不引入并发的前提下
/// 验证 <c>AllocThread</c> 的 CAS 计数与上限钳制。
/// </para>
/// </summary>
public interface IAsyncThreadLauncher
{
    /// <summary>启动第 <paramref name="index"/> 号池化线程，线程体为 <paramref name="body"/>。</summary>
    void StartThread(int index, Action body);
}

/// <summary>
/// 主线程消息泵的可注入替身。
/// <para>
/// 对应原文的 <c>AllocateHWnd</c>/<c>PostMessage</c>/<c>PeekMessage</c>/<c>DispatchMessage</c>
/// 以及 RTL 的 <c>CheckSynchronize</c>。
/// </para>
/// <para>接缝：待 VCL 消息泵（Forms.pas / Classes.pas）移植后接入真实实现。</para>
/// </summary>
public interface IMainThreadDispatch
{
    /// <summary>
    /// RTL 的同步事件（原文 1416、1448 处的 <c>SyncEvent</c>）。
    /// <para>
    /// 原文在 Delphi 5 分支（<c>{$IFNDEF DELPHI7_UP}</c>，740 行）自己声明了 <c>SyncEvent</c>，
    /// <b>Delphi 7 下该声明不编译</b>，故 <c>SyncEvent</c> 实际解析为
    /// <c>Classes.pas</c> 中由 <c>TThread.Synchronize/CheckSynchronize</c> 使用的单元级全局事件。
    /// 此处按接缝暴露。
    /// </para>
    /// </summary>
    IAsyncWaitObject RtlSyncEvent { get; }

    /// <summary>对应 <c>CheckSynchronize</c>（RTL 的 TThread 同步队列）。</summary>
    void CheckSynchronize();

    /// <summary>对应 <c>TThreadPool.ProcessMainThreadSync</c>（本池的 WM_VCLSYNC 队列）。</summary>
    void ProcessMainThreadSync();

    /// <summary>
    /// 对应 <c>PostMessage(FMainThreadVclHandle, WM_VCLSYNC, 0, LPARAM(Call))</c>；
    /// 返回 false 表示投递失败（原文随即 <c>Call.Quit(0)</c>）。
    /// </summary>
    bool PostVclSync(TInternalAsyncCall call);
}

/// <summary>
/// 变参 <c>cdecl</c> 调用层的接缝。
/// <para>
/// 原文 <c>TAsyncCallArrayOfConst.ExecuteAsyncCall</c> 用内联 x86 asm 逐参数压栈后
/// <c>Result := FProc</c> 直接跳入被调函数（2339-2408）。托管运行时无此语义。
/// 可移植的部分（判别式分类、深拷贝、栈字节记账）已在 <see cref="TVarRecMarshal"/> 落地并单测；
/// 真正的调用由本接缝承接。
/// </para>
/// <para>接缝：待 x86 调用约定层（或改由 <c>Delegate.DynamicInvoke</c> 重建）移植后接入。</para>
/// </summary>
public interface IVarRecInvoker
{
    /// <summary>调用 <paramref name="Proc"/>，实参为 <paramref name="Args"/>（cdecl 顺序）。</summary>
    int Invoke(nint Proc, object MethodData, TVarRec[] Args);
}

/// <summary>本移植的全部外部依赖打包（对应原文散落的全局 <c>ThreadPool</c> / Win32 调用）。</summary>
public sealed class TAsyncCallRuntime
{
    public TAsyncCallRuntime(
        IAsyncWaitService waitService,
        IAsyncThreadLauncher threadLauncher,
        IMainThreadDispatch mainThread,
        IVarRecInvoker varRecInvoker)
    {
        WaitService = waitService ?? throw new ArgumentNullException(nameof(waitService));
        ThreadLauncher = threadLauncher ?? throw new ArgumentNullException(nameof(threadLauncher));
        MainThread = mainThread ?? throw new ArgumentNullException(nameof(mainThread));
        VarRecInvoker = varRecInvoker ?? throw new ArgumentNullException(nameof(varRecInvoker));
    }

    public IAsyncWaitService WaitService { get; }

    public IAsyncThreadLauncher ThreadLauncher { get; }

    public IMainThreadDispatch MainThread { get; }

    public IVarRecInvoker VarRecInvoker { get; }

    /// <summary>默认运行时：真实等待 + 后台线程 + 无消息泵（控制台/服务端场景）。</summary>
    public static TAsyncCallRuntime Default { get; } = new TAsyncCallRuntime(
        new SystemAsyncWaitService(),
        new SystemAsyncThreadLauncher(),
        new NullMainThreadDispatch(),
        new NotPortedVarRecInvoker());
}

// ---------------------------------------------------------------------------
// 以下为默认（真实）实现。测试不使用它们，故不参与断言路径。
// ---------------------------------------------------------------------------

/// <summary>真实等待对象：包装 <see cref="ManualResetEventSlim"/>（手动重置语义同原文）。</summary>
public sealed class SystemAsyncWaitObject : IAsyncWaitObject, IDisposable
{
    private readonly ManualResetEventSlim _event;

    public SystemAsyncWaitObject(nint handle, bool initialState)
    {
        Handle = handle;
        _event = new ManualResetEventSlim(initialState, 1);
    }

    public nint Handle { get; }

    public bool IsSignaled => _event.IsSet;

    public void Set() => _event.Set();

    public void Reset() => _event.Reset();

    /// <summary>底层内核句柄，供 <c>WaitHandle.WaitAny/WaitAll</c> 使用。</summary>
    public WaitHandle WaitHandle => _event.WaitHandle;

    public void Dispose() => _event.Dispose();
}

/// <summary>真实等待服务：把等待数组映射到 <see cref="WaitHandle"/> 的 WaitAny/WaitAll。</summary>
public sealed class SystemAsyncWaitService : IAsyncWaitService
{
    private long _nextHandle = 0x1000;

    public bool IsMainThread => false; // 接缝：服务端无 UI 主线程；UI 场景由宿主注入

    public IAsyncWaitObject CreateManualResetEvent(bool initialState)
    {
        return new SystemAsyncWaitObject((nint)Interlocked.Increment(ref _nextHandle), initialState);
    }

    public uint WaitForMultipleObjects(IAsyncWaitObject[] handles, bool waitAll, uint milliseconds)
    {
        if (handles == null || handles.Length == 0)
            return AsyncCallsConst.WAIT_FAILED; // Win32：句柄数为 0 → WAIT_FAILED

        var waits = new WaitHandle[handles.Length];
        for (int i = 0; i < handles.Length; i++)
        {
            if (handles[i] is SystemAsyncWaitObject o)
                waits[i] = o.WaitHandle;
            else
                return AsyncCallsConst.WAIT_FAILED; // 非本实现的对象无法进入内核等待
        }

        int timeout = milliseconds == AsyncCallsConst.INFINITE ? Timeout.Infinite : (int)milliseconds;
        if (waitAll)
            return WaitHandle.WaitAll(waits, timeout) ? AsyncCallsConst.WAIT_OBJECT_0 : AsyncCallsConst.WAIT_TIMEOUT;

        int index = WaitHandle.WaitAny(waits, timeout);
        if (index == WaitHandle.WaitTimeout)
            return AsyncCallsConst.WAIT_TIMEOUT;
        return AsyncCallsConst.WAIT_OBJECT_0 + (uint)index;
    }

    public uint MsgWaitForMultipleObjects(IAsyncWaitObject[] handles, bool waitAll, uint milliseconds, uint wakeMask)
    {
        // 接缝：控制台/服务端无消息队列 → 消息永不置位，退化为普通等待。
        _ = wakeMask;
        return WaitForMultipleObjects(handles, waitAll, milliseconds);
    }
}

/// <summary>真实线程启动器：后台线程（对应原文 <c>TAsyncCallThread.Create(False)</c>）。</summary>
public sealed class SystemAsyncThreadLauncher : IAsyncThreadLauncher
{
    public void StartThread(int index, Action body)
    {
        if (body == null)
            throw new ArgumentNullException(nameof(body));

        var thread = new Thread(() => body())
        {
            IsBackground = true,
            Name = "AsyncCalls#" + index.ToString(System.Globalization.CultureInfo.InvariantCulture),
        };
        thread.Start();
    }
}

/// <summary>
/// 无消息泵的主线程派发实现（服务端/测试场景）。
/// <para>接缝：待 VCL 消息泵移植后接入真实实现。</para>
/// </summary>
public sealed class NullMainThreadDispatch : IMainThreadDispatch
{
    private readonly IAsyncWaitObject _rtlSyncEvent;

    public NullMainThreadDispatch(IAsyncWaitObject rtlSyncEvent = null)
    {
        _rtlSyncEvent = rtlSyncEvent;
    }

    public IAsyncWaitObject RtlSyncEvent => _rtlSyncEvent;

    public void CheckSynchronize()
    {
    }

    public void ProcessMainThreadSync()
    {
    }

    public bool PostVclSync(TInternalAsyncCall call) => false;
}

/// <summary>
/// 变参调用接缝的未移植占位实现。
/// <para>接缝：待 x86 cdecl 调用层移植后接入。</para>
/// </summary>
public sealed class NotPortedVarRecInvoker : IVarRecInvoker
{
    public int Invoke(nint Proc, object MethodData, TVarRec[] Args)
    {
        throw new NotSupportedException(
            "接缝：TAsyncCallArrayOfConst 的 x86 cdecl 压栈调用层尚未移植（AsyncCalls.pas:2324-2408 内联 asm）。");
    }
}
