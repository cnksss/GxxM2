using System;

namespace GXX.Core.Async;

/// <summary>
/// AsyncCalls.pas 1:1 移植：单元级 resourcestring、Windows/Messages 常量。
/// </summary>
/// <remarks>
/// 源单元：<c>Source/M2Engine/AsyncCalls.pas</c>（2,959 有效行 / 3,439 物理行）。
/// 本文件对应其中三份<b>完全同源</b>的副本：
/// <list type="bullet">
/// <item>Source/M2Engine/AsyncCalls.pas</item>
/// <item>Source/Client-HGE/AsyncCalls.pas</item>
/// <item>Source/RunGate/AsyncCalls.pas</item>
/// </list>
/// 三份 SHA256 均为 82262F0AC03D848D7ABEC5A4559914EE1574C1CA2CEFAEA4BC5C1C8F49D0C048、
/// 均 103,449 字节、均 3,439 行 —— 逐字节相同，故只移植一次供三处共用。
/// <para>
/// 条件编译基线：原树为 Delphi 7（CompilerVersion 15.0）。据此
/// <c>SUPPORT_LOCAL_FUNCTIONS</c>、<c>DELPHI7_UP</c> 生效；
/// <c>DELPHI5</c>/<c>DELPHI6</c>/<c>DELPHI2009_UP</c>/<c>UNICODE</c> 不生效。
/// 各常量的活跃性已在注释中逐条标注行号。
/// </para>
/// </remarks>
public static class AsyncCallsConst
{
    // ------------------------------------------------------------------
    // resourcestring（原文：AsyncCalls.pas:726-732）
    // ------------------------------------------------------------------

    /// <summary>原文 727：RsAsyncCallNotFinished</summary>
    public const string RsAsyncCallNotFinished = "The asynchronous call is not finished yet";

    /// <summary>原文 728：RsAsyncCallUnknownVarRecType（Format 占位 %d）</summary>
    public const string RsAsyncCallUnknownVarRecType = "Unknown TVarRec type %d";

    /// <summary>原文 729：RsLeaveMainThreadNestedError</summary>
    public const string RsLeaveMainThreadNestedError = "Unpaired call to AsyncCalls.LeaveMainThread()";

    /// <summary>原文 730：RsLeaveMainThreadThreadError</summary>
    public const string RsLeaveMainThreadThreadError =
        "AsyncCalls.LeaveMainThread() was called outside of the main thread";

    /// <summary>原文 731：RsForgetWasCalled（Delphi 的 '' 转义为单引号）</summary>
    public const string RsForgetWasCalled =
        "IAsyncCall.Forget was called. The interface isn't connected to the asynchronous call anymore";

    /// <summary>原文 732：RsNoVclSyncPossible</summary>
    public const string RsNoVclSyncPossible =
        "Cannot synchronize with the main thread anymore. Don't call IAsyncCall.Forget for functions that access the VCL";

    // ------------------------------------------------------------------
    // 自定义窗口消息（原文：AsyncCalls.pas:734-736）
    // ------------------------------------------------------------------

    /// <summary>Messages.pas 的 WM_USER（原文未定义，属 Delphi RTL）。</summary>
    public const int WM_USER = 0x0400;

    /// <summary>原文 735：WM_VCLSYNC = WM_USER + 12</summary>
    public const int WM_VCLSYNC = WM_USER + 12;

    /// <summary>原文 736：WM_RAISEEXCEPTION = WM_USER + 13</summary>
    public const int WM_RAISEEXCEPTION = WM_USER + 13;

    // ------------------------------------------------------------------
    // 等待相关常量
    // ------------------------------------------------------------------

    /// <summary>Windows.pas 的 MAXIMUM_WAIT_OBJECTS（原文未定义，属 Win32 SDK）。</summary>
    public const int MAXIMUM_WAIT_OBJECTS = 64;

    /// <summary>原文 435：MAXIMUM_ASYNC_WAIT_OBJECTS = MAXIMUM_WAIT_OBJECTS - 3（= 61）</summary>
    public const int MAXIMUM_ASYNC_WAIT_OBJECTS = MAXIMUM_WAIT_OBJECTS - 3;

    /// <summary>Windows.pas：等待成功（首个对象）。</summary>
    public const uint WAIT_OBJECT_0 = 0x00000000;

    /// <summary>Windows.pas：WAIT_ABANDONED_0（互斥体被放弃）。</summary>
    public const uint WAIT_ABANDONED_0 = 0x00000080;

    /// <summary>Windows.pas：WAIT_TIMEOUT。</summary>
    public const uint WAIT_TIMEOUT = 0x00000102;

    /// <summary>
    /// Windows.pas：WAIT_FAILED。
    /// <para>
    /// 原文易错点：<c>WAIT_FAILED</c> 与 <c>INFINITE</c> 在 Win32 中<b>同为 $FFFFFFFF</b>。
    /// 原文 <c>InternalAsyncMultiSync</c>（1638 行）在对象数超限时返回 <c>WAIT_FAILED</c>，
    /// 而 <c>Milliseconds</c> 的默认值又正是 <c>INFINITE</c> —— 二者数值不可区分。
    /// 这是原设计固有的不可区分性（同台账 §11.4 MakeIPToInt 一类），<b>不得顺手修正</b>。
    /// </para>
    /// </summary>
    public const uint WAIT_FAILED = 0xFFFFFFFF;

    /// <summary>Windows.pas：INFINITE（与 WAIT_FAILED 同值，见上）。</summary>
    public const uint INFINITE = 0xFFFFFFFF;

    /// <summary>Messages.pas：QS_ALLINPUT。</summary>
    public const uint QS_ALLINPUT = 0x04FF;

    /// <summary>Messages.pas：QS_ALLPOSTMESSAGE。</summary>
    public const uint QS_ALLPOSTMESSAGE = 0x0100;

    // ------------------------------------------------------------------
    // 线程池规模
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 866：<c>FThreads: array[0..255] of TAsyncCallThread</c>，即 <c>Length(FThreads) = 256</c>。
    /// 原文 1104、1765、1105 三处均以 <c>Length(FThreads)</c> 作为上限。
    /// </summary>
    public const int ASYNC_CALL_THREAD_ARRAY_LENGTH = 256;

    /// <summary>
    /// 原文 866 的数组下标上界（<c>High(FThreads)</c> = 255）。
    /// </summary>
    public const int ASYNC_CALL_THREAD_ARRAY_HIGH = ASYNC_CALL_THREAD_ARRAY_LENGTH - 1;

    /// <summary>原文 830-839 之外：<c>ENTIRE_ARRAY</c> 占位，本移植不需要。</summary>
    internal const int ASYNC_NO_INDEX = -1;

    // ------------------------------------------------------------------
    // 错误构造（对应原文 NotFinishedError / UnknownVarRecType，1665-1677）
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 1665-1672：<c>procedure NotFinishedError(const FunctionName: string)</c>。
    /// <c>DEBUG_ASYNCCALLS_ODS</c> 未定义，故 FunctionName 仅作诊断用途、不影响行为。
    /// </summary>
    public static TAsyncCallError NotFinishedError(string FunctionName)
    {
        // 原文如此（AsyncCalls.pas:1667-1670）：{$IFDEF DEBUG_ASYNCCALLS_ODS} 未定义 → 该分支不编译。
        // 此处保留形参语义但不输出调试串。
        _ = FunctionName;
        return new TAsyncCallError(RsAsyncCallNotFinished);
    }

    /// <summary>
    /// 原文 1674-1677：<c>procedure UnknownVarRecType(VType: Byte)</c>。
    /// 抛出 <c>EAsyncCallError.CreateFmt(RsAsyncCallUnknownVarRecType, [VType])</c>。
    /// <para>
    /// 注意：resourcestring 用的是 Delphi 的 <c>%d</c> 占位符，
    /// 必须经 <see cref="GXX.Core.Rtl.DelphiRTL.Format"/> 而非 <c>string.Format</c> 渲染
    /// （后者只认 <c>{0}</c>，会原样吐出 <c>%d</c>）。
    /// </para>
    /// </summary>
    public static TAsyncCallError UnknownVarRecType(byte VType)
    {
        return new TAsyncCallError(GXX.Core.Rtl.DelphiRTL.Format(RsAsyncCallUnknownVarRecType, VType));
    }
}
