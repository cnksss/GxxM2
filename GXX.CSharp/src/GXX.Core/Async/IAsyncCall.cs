using System;

// 源单元：Source/M2Engine/AsyncCalls.pas（同源副本：Source/Client-HGE/AsyncCalls.pas、Source/RunGate/AsyncCalls.pas）

namespace GXX.Core.Async;

/// <summary>
/// 原文 129：<c>EAsyncCallError = class(Exception)</c>。
/// </summary>
public class TAsyncCallError : Exception
{
    public TAsyncCallError()
    {
    }

    public TAsyncCallError(string message) : base(message)
    {
    }

    public TAsyncCallError(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// 原文 131-163：<c>IAsyncCall</c>。方法名、语义、注释均照抄原文。
/// </summary>
public interface IAsyncCall
{
    /// <summary>
    /// Sync() waits until the asynchronous call has finished and returns the
    /// result value of the called function if that exists.
    /// </summary>
    int Sync();

    /// <summary>Finished() returns True if the asynchronous call has finished.</summary>
    bool Finished();

    /// <summary>
    /// ReturnValue() returns the result of the asynchronous call. It raises an
    /// exception if called before the function has finished. It returns 0 if the
    /// AsyncCall invocation was canceled.
    /// </summary>
    int ReturnValue();

    /// <summary>Canceled() returns True if the AsyncCall was canceled by CancelInvocation().</summary>
    bool Canceled();

    /// <summary>
    /// ForceDifferentThread() tells AsyncCalls that the assigned function must
    /// not be executed in the current thread.
    /// </summary>
    void ForceDifferentThread();

    /// <summary>
    /// CancelInvocation() stopps the AsyncCall from being invoked. If the AsyncCall is already
    /// processed, a call to CancelInvocation() has no effect and the Canceled() function will
    /// return False as the AsyncCall wasn't canceled.
    /// <para>原文如此（AsyncCalls.pas:151）：注释中 "stopps" 为原文拼写。</para>
    /// </summary>
    void CancelInvocation();

    /// <summary>
    /// Forget() unlinks the IAsyncCall interface from the internal AsyncCall. ...
    /// </summary>
    void Forget();
}

/// <summary>
/// 原文 165-170：<c>IAsyncCallEx</c>（"*** Internal interface. Do not use it ***"）。
/// <para>
/// 原文返回 <c>THandle</c>；托管侧以 <see cref="IAsyncWaitObject"/> 取代，
/// <c>null</c> 对应原文的 <c>0</c>（无效句柄）。见 2098-2101 的 <c>GetEvent</c>。
/// </para>
/// </summary>
public interface IAsyncCallEx
{
    /// <summary>原文 168：<c>function GetEvent: THandle</c>；null ↔ 0。</summary>
    IAsyncWaitObject GetEvent();

    /// <summary>原文 169：<c>function SyncInThisThreadIfPossible: Boolean</c></summary>
    bool SyncInThisThreadIfPossible();
}

/// <summary>
/// 原文 172-175：<c>IAsyncRunnable</c>（GUID 1A313BBD-0F89-43AD-8B57-BBA3205F4888）。
/// </summary>
public interface IAsyncRunnable
{
    /// <summary>原文 174：<c>procedure AsyncRun</c></summary>
    void AsyncRun();
}

/// <summary>
/// 原文 90：<c>TAsyncIdleMsgMethod = procedure of object</c>。
/// </summary>
public delegate void TAsyncIdleMsgMethod();

// ---------------------------------------------------------------------------
// 原文 101-127：被异步调用的函数/过程签名族。
// 方法名/委托名保留 T 前缀与原拼写；"of object" 语义在 C# 侧由委托实例自带 Target。
// ---------------------------------------------------------------------------

/// <summary>原文 101</summary>
public delegate int TAsyncCallArgObjectProc(object Arg);

/// <summary>原文 102</summary>
public delegate int TAsyncCallArgIntegerProc(int Arg);

/// <summary>原文 103</summary>
public delegate int TAsyncCallArgStringProc(string Arg);

/// <summary>原文 104（Delphi WideString = UTF-16，C# 侧即 string）</summary>
public delegate int TAsyncCallArgWideStringProc(string Arg);

/// <summary>原文 105</summary>
public delegate int TAsyncCallArgInterfaceProc(object Arg);

/// <summary>原文 106（Delphi Extended 为 80 位；C# 无对应类型，取 double，见报告）</summary>
public delegate int TAsyncCallArgExtendedProc(double Arg);

/// <summary>原文 107</summary>
public delegate int TAsyncCallArgVariantProc(object Arg);

/// <summary>原文 109</summary>
public delegate int TAsyncCallArgObjectMethod(object Arg);

/// <summary>原文 110</summary>
public delegate int TAsyncCallArgIntegerMethod(int Arg);

/// <summary>原文 111</summary>
public delegate int TAsyncCallArgStringMethod(string Arg);

/// <summary>原文 112</summary>
public delegate int TAsyncCallArgWideStringMethod(string Arg);

/// <summary>原文 113</summary>
public delegate int TAsyncCallArgInterfaceMethod(object Arg);

/// <summary>原文 114</summary>
public delegate int TAsyncCallArgExtendedMethod(double Arg);

/// <summary>原文 115</summary>
public delegate int TAsyncCallArgVariantMethod(object Arg);

/// <summary>原文 117</summary>
public delegate void TAsyncCallArgObjectEvent(object Arg);

/// <summary>原文 118</summary>
public delegate void TAsyncCallArgIntegerEvent(int Arg);

/// <summary>原文 119</summary>
public delegate void TAsyncCallArgStringEvent(string Arg);

/// <summary>原文 120</summary>
public delegate void TAsyncCallArgWideStringEvent(string Arg);

/// <summary>原文 121</summary>
public delegate void TAsyncCallArgInterfaceEvent(object Arg);

/// <summary>原文 122</summary>
public delegate void TAsyncCallArgExtendedEvent(double Arg);

/// <summary>原文 123</summary>
public delegate void TAsyncCallArgVariantEvent(object Arg);

/// <summary>原文 125：<c>TAsyncCallArgRecordProc = function(var Arg): Integer</c>（var 传引用记录）</summary>
public delegate int TAsyncCallArgRecordProc(IntPtr Arg);

/// <summary>原文 126</summary>
public delegate int TAsyncCallArgRecordMethod(IntPtr Arg);

/// <summary>原文 127</summary>
public delegate void TAsyncCallArgRecordEvent(IntPtr Arg);
