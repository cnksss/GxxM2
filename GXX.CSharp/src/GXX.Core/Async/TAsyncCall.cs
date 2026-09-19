using System;

// 源单元：Source/M2Engine/AsyncCalls.pas（同源副本：Source/Client-HGE/AsyncCalls.pas、Source/RunGate/AsyncCalls.pas）

namespace GXX.Core.Async;

/// <summary>
/// 原文 536-556：<c>TAsyncCall = class(TInterfacedObject, IAsyncCall, IAsyncCallEx)</c>
/// —— 对外交付的 <see cref="IAsyncCall"/> 外壳。
/// <para>
/// 关键语义：所有方法先 <c>CheckForget</c>；<c>Forget</c> 之后本对象与内部调用脱钩，
/// 继续调用任何方法都抛 <see cref="AsyncCallsConst.RsForgetWasCalled"/>。
/// </para>
/// </summary>
public sealed class TAsyncCall : IAsyncCall, IAsyncCallEx, IDisposable
{
    private TInternalAsyncCall FCall;
    private readonly TAsyncCallRuntime _runtime;

    /// <summary>原文 545：<c>constructor Create(ACall: TInternalAsyncCall)</c>（Delphi 中为 private）。</summary>
    public TAsyncCall(TInternalAsyncCall ACall, TAsyncCallRuntime runtime = null)
    {
        if (ACall == null)
            throw new ArgumentNullException(nameof(ACall));
        _runtime = runtime;
        ACall.AddRef();
        FCall = ACall;
    }

    /// <summary>原文 3411-3415：<c>if FCall = nil then raise EAsyncCallError.CreateRes(@RsForgetWasCalled)</c></summary>
    private void CheckForget()
    {
        if (FCall == null)
            throw new TAsyncCallError(AsyncCallsConst.RsForgetWasCalled);
    }

    /// <summary>
    /// 原文 3339-3350：<c>destructor Destroy</c>。
    /// <para>
    /// <c>try FCall.Sync finally FCall.Release end</c> —— 接口释放时若异步调用尚未完成，
    /// 会在此<b>阻塞等待</b>并把异步函数抛出的异常在调用方线程重抛。
    /// </para>
    /// </summary>
    public void Destroy()
    {
        if (FCall != null)
        {
            try
            {
                FCall.Sync(); // throws raised exception
            }
            finally
            {
                FCall.Release();
                FCall = null;
            }
        }
    }

    /// <summary>原文 3352-3356</summary>
    public bool Finished()
    {
        CheckForget();
        return FCall.Finished();
    }

    /// <summary>原文 3358-3362</summary>
    public void ForceDifferentThread()
    {
        CheckForget();
        FCall.ForceDifferentThread();
    }

    /// <summary>
    /// 原文 3364-3373：
    /// <code>
    /// CheckForget;
    /// C := FCall; FCall := nil; C.Forget; C.Release;
    /// </code>
    /// <b>顺序要点</b>：先摘链再 <c>Forget</c> 再 <c>Release</c>；
    /// 因此 Forget 之后本对象不再持有内部引用，内部调用仍会被池执行完。
    /// </summary>
    public void Forget()
    {
        CheckForget();
        var C = FCall;
        FCall = null;
        C.Forget();
        C.Release();
    }

    /// <summary>原文 3375-3379</summary>
    public int ReturnValue()
    {
        CheckForget();
        return FCall.ReturnValue();
    }

    /// <summary>原文 3381-3385</summary>
    public int Sync()
    {
        CheckForget();
        return FCall.Sync();
    }

    /// <summary>原文 3387-3391（IAsyncCallEx）</summary>
    public IAsyncWaitObject GetEvent()
    {
        CheckForget();
        return FCall.GetEvent();
    }

    /// <summary>原文 3393-3397（IAsyncCallEx）</summary>
    public bool SyncInThisThreadIfPossible()
    {
        CheckForget();
        return FCall.SyncInThisThreadIfPossible();
    }

    /// <summary>原文 3399-3403</summary>
    public bool Canceled()
    {
        CheckForget();
        return FCall.Canceled();
    }

    /// <summary>原文 3405-3409</summary>
    public void CancelInvocation()
    {
        CheckForget();
        FCall.CancelInvocation();
    }

    /// <summary>是否仍与内部调用相连（原文无访问器，仅为可测性暴露）。</summary>
    public bool IsConnected => FCall != null;

    /// <summary>对应 Delphi 的接口引用释放（<c>_Release</c> → <c>Destroy</c>）。</summary>
    public void Dispose() => Destroy();

    /// <summary>当前注入的运行时（原文无此概念，仅为可测性暴露）。</summary>
    internal TAsyncCallRuntime RuntimeOrDefault => _runtime ?? AsyncCalls.Runtime;
}

/// <summary>
/// 原文 558-574 + 1991-2027：<c>TSyncCall</c> —— 已经执行完毕的“假” <see cref="IAsyncCall"/>。
/// <para>
/// 用于 <c>MaxThreads = 0</c> 时就地同步执行的分支（如 1120-1121），
/// 以及 <c>VCLInvoke</c> 在主线程内的短路分支（3296-3299）。
/// </para>
/// </summary>
public sealed class TSyncCall : IAsyncCall
{
    private readonly int FReturnValue;

    /// <summary>原文 2000-2004：<c>constructor Create(AReturnValue: Integer)</c>（Delphi 中为 private）</summary>
    public TSyncCall(int AReturnValue)
    {
        FReturnValue = AReturnValue;
    }

    /// <summary>原文 1991-1994：<c>Result := False</c>（同步调用无所谓“被取消”）</summary>
    public bool Canceled() => false;

    /// <summary>原文 1996-1998：空实现</summary>
    public void CancelInvocation()
    {
    }

    /// <summary>原文 2006-2009：<c>Result := True</c></summary>
    public bool Finished() => true;

    /// <summary>原文 2011-2013：空实现</summary>
    public void ForceDifferentThread()
    {
    }

    /// <summary>原文 2015-2017：空实现</summary>
    public void Forget()
    {
    }

    /// <summary>原文 2019-2022：<c>Result := FReturnValue</c></summary>
    public int ReturnValue() => FReturnValue;

    /// <summary>原文 2024-2027：<c>Result := FReturnValue</c></summary>
    public int Sync() => FReturnValue;
}
