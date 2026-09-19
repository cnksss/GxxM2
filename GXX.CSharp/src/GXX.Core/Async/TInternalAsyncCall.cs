using System;
using System.Runtime.ExceptionServices;
using System.Threading;

// 源单元：Source/M2Engine/AsyncCalls.pas（同源副本：Source/Client-HGE/AsyncCalls.pas、Source/RunGate/AsyncCalls.pas）

namespace GXX.Core.Async;

/// <summary>
/// 原文 489-533：<c>TInternalAsyncCall</c> —— 所有带参异步调用的基类，
/// 持有事件、引用计数、返回值、取消/强制换线程等状态。
/// <para>
/// <b>可测的纯逻辑</b>：状态机（<see cref="Finished"/> / <see cref="Canceled"/> /
/// <see cref="CancelInvocation"/> / <see cref="ForceDifferentThread"/> / <see cref="Forget"/> /
/// <see cref="Quit"/>）、引用计数、异常暂存与重抛规则。
/// <b>线程外壳</b>：实际等待与线程执行经 <see cref="TAsyncCallRuntime"/> 注入。
/// </para>
/// </summary>
public abstract class TInternalAsyncCall : IDisposable
{
    // 原文 493：FNext: TInternalAsyncCall
    internal TInternalAsyncCall FNext;

    // 原文 495：FEvent: THandle（手动重置事件；null ↔ 0）
    private IAsyncWaitObject FEvent;

    // 原文 496-497：FFatalException / FFatalErrorAddr
    private Exception FFatalException;
    private IntPtr FFatalErrorAddr;

    // 原文 499-505
    private int FRefCount;
    private int FReturnValue;
    private bool FForceDifferentThread;
    private bool FCancelInvocation;
    private bool FCanceled;
    private bool FExecuted;
    private bool FFinished;

    private TAsyncCallRuntime _runtime;
    private TThreadPool _pool;

    /// <summary>原文 515：<c>constructor Create</c>（Delphi 中为 private，同单元可见）。</summary>
    protected TInternalAsyncCall(TAsyncCallRuntime runtime = null)
    {
        Runtime = runtime;
    }

    /// <summary>注入的运行时（等待/线程/消息泵/变参调用）。null ⇒ 取模块级默认。</summary>
    protected TAsyncCallRuntime Runtime
    {
        get => _runtime ?? AsyncCalls.Runtime;
        set => _runtime = value;
    }

    /// <summary>
    /// 所属线程池。原文各处直接引用单元级全局 <c>ThreadPool</c>（1100 行）；
    /// 此处保留该默认，同时允许显式注入以便无全局状态地单测。
    /// </summary>
    public TThreadPool Pool
    {
        get => _pool ?? AsyncCalls.ThreadPool;
        set => _pool = value;
    }

    // ------------------------------------------------------------------
    // 原文 495-497 / 2035-2044：事件句柄的建立与释放
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 2035：<c>FEvent := CreateEvent(nil, True, False, nil)</c>（手动重置、未置位）。
    /// 由派生类构造完毕后调用（对应 Delphi <c>inherited Create</c> 的时机）。
    /// </summary>
    protected void CreateEvent()
    {
        FEvent = Runtime.WaitService.CreateManualResetEvent(false);
    }

    /// <summary>
    /// 原文 2038-2057：<c>destructor Destroy</c>。
    /// <para>
    /// 原文在此把未被取回的致命异常投递回主线程
    /// （<c>PostMessage(WM_RAISEEXCEPTION, WPARAM(FFatalErrorAddr), LPARAM(FFatalException))</c>），
    /// 或直接释放。托管侧 <see cref="NotPortedMainThreadExceptionSeam"/> 为接缝。
    /// </para>
    /// </summary>
    public virtual void Destroy()
    {
        if (FEvent != null)
        {
            if (FEvent is IDisposable d)
                d.Dispose();
            FEvent = null; // 原文 2043：FEvent := 0
        }

        // 原文如此（AsyncCalls.pas:2048-2055）：未取回的异常交由主线程处理。
        // 原文条件为 Assigned(ApplicationHandleException) and (FMainThreadVclHandle <> 0)
        // and IsWindow(FMainThreadVclHandle) —— 这些成立性判定属 IMainThreadDispatch 接缝内部。
        if (FFatalException != null)
        {
            if (Runtime.MainThread.PostVclSync(this))
            {
                // 原文 2052：PostMessage(WM_RAISEEXCEPTION, WPARAM(FFatalErrorAddr), LPARAM(FFatalException))
                // —— 异常所有权移交主线程消息泵。
            }
            else
            {
                // 原文 2054：FFatalException.Free；托管侧仅解除引用。
            }
            FFatalException = null;
        }
    }

    // ------------------------------------------------------------------
    // 原文 2059-2068：引用计数
    // ------------------------------------------------------------------

    /// <summary>原文 2059-2062：<c>InterlockedIncrement(FRefCount)</c></summary>
    public void AddRef()
    {
        Interlocked.Increment(ref FRefCount);
    }

    /// <summary>原文 2064-2068：<c>if InterlockedDecrement(FRefCount) = 0 then Destroy</c></summary>
    public void Release()
    {
        if (Interlocked.Decrement(ref FRefCount) == 0)
            Destroy();
    }

    /// <summary>当前引用计数（原文无此访问器，仅为可测性暴露；不改变语义）。</summary>
    public int RefCount => Volatile.Read(ref FRefCount);

    /// <summary>对应 Delphi 的 <c>Release</c> 触发析构（显式释放路径）。</summary>
    public void Dispose() => Destroy();

    // ------------------------------------------------------------------
    // 原文 2070-2101：状态查询
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 2070-2076：
    /// <code>
    /// if FCanceled or (FCancelInvocation and not FExecuted) then Result := True
    /// else Result := (FEvent = 0) or FFinished or (WaitForSingleObject(FEvent, 0) = WAIT_OBJECT_0);
    /// </code>
    /// <b>分支顺序照抄</b>：取消状态先于事件状态判定。
    /// </summary>
    public bool Finished()
    {
        if (FCanceled || (FCancelInvocation && !FExecuted))
            return true;
        return FEvent == null || FFinished || IsEventSignaled();
    }

    /// <summary>对应 <c>WaitForSingleObject(FEvent, 0) = WAIT_OBJECT_0</c>。</summary>
    private bool IsEventSignaled()
    {
        if (FEvent == null)
            return false;
        return Runtime.WaitService.WaitForMultipleObjects(
            new[] { FEvent }, false, 0) == AsyncCallsConst.WAIT_OBJECT_0;
    }

    /// <summary>原文 2078-2081：<c>FForceDifferentThread := True</c></summary>
    public void ForceDifferentThread()
    {
        FForceDifferentThread = true;
    }

    /// <summary>
    /// 原文 2083-2086：<c>Forget</c> 仅调用 <c>ForceDifferentThread</c>。
    /// <para>原文如此（AsyncCalls.pas:2085）：Forget 与 ForceDifferentThread 在内部对象上等价。</para>
    /// </summary>
    public void Forget()
    {
        ForceDifferentThread();
    }

    /// <summary>原文 2088-2091：<c>Result := FCanceled</c></summary>
    public bool Canceled()
    {
        return FCanceled;
    }

    /// <summary>原文 2093-2096：<c>FCancelInvocation := True</c></summary>
    public void CancelInvocation()
    {
        FCancelInvocation = true;
    }

    /// <summary>原文 2098-2101：<c>Result := FEvent</c>（null ↔ 0）</summary>
    public IAsyncWaitObject GetEvent()
    {
        return FEvent;
    }

    /// <summary>强制换线程标记（原文无访问器，仅为可测性暴露）。</summary>
    public bool ForceDifferentThreadSet => FForceDifferentThread;

    /// <summary>是否已进入执行（原文无访问器，仅为可测性暴露）。</summary>
    public bool Executed => FExecuted;

    /// <summary>队列后继（原文 493 的 <c>FNext</c>；仅为可测性暴露，不改变语义）。</summary>
    public TInternalAsyncCall Next => FNext;

    // ------------------------------------------------------------------
    // 原文 2103-2153：执行与退出
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 2103-2124：由池化线程调用。
    /// <para>
    /// <c>except FFatalErrorAddr := ExceptAddr; FFatalException := Exception(AcquireExceptionObject);</c>
    /// —— 托管侧 <c>ExceptAddr</c>（抛出点机器地址）无对应概念，登记为差异；
    /// 异常对象本身照常暂存（<see cref="ExceptionDispatchInfo"/> 在重抛处恢复栈）。
    /// </para>
    /// </summary>
    public void InternExecuteAsyncCall()
    {
        int Value = 0;
        try
        {
            if (!FCancelInvocation)
            {
                FExecuted = true;
                Value = ExecuteAsyncCall();
            }
            else
                FCanceled = true;
        }
        catch (Exception E)
        {
            FFatalErrorAddr = IntPtr.Zero; // 原文：ExceptAddr
            FFatalException = E;
        }
        Quit(Value);
    }

    /// <summary>
    /// 原文 2126-2146：在当前线程内执行（<c>finally Quit</c> ⇒ 异常继续向调用方传播）。
    /// </summary>
    public void InternExecuteSyncCall()
    {
        int Value = 0;
        try
        {
            if (!FCancelInvocation)
            {
                FExecuted = true;
                Value = ExecuteAsyncCall();
            }
            else
                FCanceled = true;
        }
        finally
        {
            // Let the exception be handled by the caller because we are in sync with it
            Quit(Value);
        }
    }

    /// <summary>原文 2148-2153：<c>FReturnValue := AReturnValue; FFinished := True; SetEvent(FEvent);</c></summary>
    public void Quit(int AReturnValue)
    {
        FReturnValue = AReturnValue;
        FFinished = true;
        FEvent?.Set();
    }

    /// <summary>
    /// 原文 2155-2169：<c>ReturnValue</c>。
    /// <b>顺序要点</b>：先判定未完成（抛 <see cref="AsyncCallsConst.RsAsyncCallNotFinished"/>），
    /// 取返回值，<b>最后</b>才重抛暂存异常；重抛前把 <c>FFatalException</c> 置 nil，
    /// 因此同一异步调用第二次取值<b>不再</b>抛异常。
    /// </summary>
    public int ReturnValue()
    {
        if (!Finished())
            throw AsyncCallsConst.NotFinishedError("IAsyncCall.ReturnValue");
        int result = FReturnValue;
        RethrowFatalException();
        return result;
    }

    /// <summary>
    /// 原文 2171-2197：<c>Sync</c>。主线程走 <c>WaitForSingleObjectMainThread</c>（带消息泵），
    /// 其它线程走 <c>WaitForSingleObject</c>；等待结果非 <c>WAIT_OBJECT_0</c> 即报未完成。
    /// </summary>
    public int Sync()
    {
        if (!Finished())
        {
            if (!SyncInThisThreadIfPossible())
            {
                if (Runtime.WaitService.IsMainThread)
                {
                    if (WaitForSingleObjectMainThread(FEvent, AsyncCallsConst.INFINITE) != AsyncCallsConst.WAIT_OBJECT_0)
                        throw AsyncCallsConst.NotFinishedError("IAsyncCall.Sync");
                }
                else if (WaitForSingleObject(FEvent, AsyncCallsConst.INFINITE) != AsyncCallsConst.WAIT_OBJECT_0)
                    throw AsyncCallsConst.NotFinishedError("IAsyncCall.Sync");
            }
        }
        int result = FReturnValue;
        RethrowFatalException();
        return result;
    }

    /// <summary>对应非主线程的 <c>WaitForSingleObject(Handle, Timeout)</c>。</summary>
    private uint WaitForSingleObject(IAsyncWaitObject handle, uint milliseconds)
    {
        return Runtime.WaitService.WaitForMultipleObjects(new[] { handle }, false, milliseconds);
    }

    /// <summary>
    /// 原文 1411-1434 的 <c>WaitForSingleObjectMainThread</c>（定义在实现段，
    /// 与多对象等待同区）。此处转发到 <see cref="TAsyncCallMultiSync"/>。
    /// </summary>
    private uint WaitForSingleObjectMainThread(IAsyncWaitObject handle, uint timeout)
    {
        return TAsyncCallMultiSync.WaitForSingleObjectMainThread(Runtime, Pool, handle, timeout);
    }

    /// <summary>
    /// 原文 2199-2219：若本调用尚未被任一线程取走，则从等待队列摘除并在<b>当前线程</b>执行。
    /// <para>
    /// <b>差异断言点</b>：<c>if not FForceDifferentThread or ThreadPool.FDestroying then</c>
    /// —— <c>Forget</c> 置位的 <c>FForceDifferentThread</c> 会被池销毁态<b>覆盖</b>，
    /// 即“池正在销毁时即使要求换线程也会就地执行”。
    /// </para>
    /// </summary>
    public bool SyncInThisThreadIfPossible()
    {
        if (!Finished())
        {
            bool result = false;
            if (ShouldTrySyncInThisThread(FForceDifferentThread, Pool.Destroying))
            {
                if (Pool.RemoveAsyncCall(this))
                {
                    InternExecuteSyncCall();
                    result = true;
                }
            }
            return result;
        }
        return true;
    }

    /// <summary>
    /// 原文 2206 的条件 <c>if not FForceDifferentThread or ThreadPool.FDestroying then</c> 的纯函数形式
    /// （仅为可测性抽出，语义完全一致）。
    /// <para>
    /// <b>语义要点</b>：池销毁态<b>或</b>运算会压过 <c>Forget</c> 置位的强制换线程标记。
    /// </para>
    /// </summary>
    public static bool ShouldTrySyncInThisThread(bool forceDifferentThread, bool poolDestroying)
    {
        return !forceDifferentThread || poolDestroying;
    }

    /// <summary>
    /// 原文 2221-2225：<c>Result := TAsyncCall.Create(Self); ThreadPool.AddAsyncCall(Self);</c>
    /// </summary>
    public TAsyncCall ExecuteAsync()
    {
        return ExecuteAsync(Pool);
    }

    /// <summary>显式指定线程池的 <see cref="ExecuteAsync()"/> 重载（便于无全局状态单测）。</summary>
    public TAsyncCall ExecuteAsync(TThreadPool pool)
    {
        Pool = pool;
        var result = new TAsyncCall(this, Runtime);
        pool.AddAsyncCall(this);
        return result;
    }

    private void RethrowFatalException()
    {
        if (FFatalException == null)
            return;
        Exception E = FFatalException;
        FFatalException = null; // 原文 2165/2193：先置 nil 再 raise
        _ = FFatalErrorAddr;
        // 原文：raise E at FFatalErrorAddr —— 托管侧无“指定机器地址重抛”，
        // 用 ExceptionDispatchInfo 保留原始托管栈（语义等价且信息更完整）。
        ExceptionDispatchInfo.Capture(E).Throw();
    }

    /// <summary>原文 513：<c>function ExecuteAsyncCall: Integer; virtual; abstract;</c></summary>
    protected abstract int ExecuteAsyncCall();
}
