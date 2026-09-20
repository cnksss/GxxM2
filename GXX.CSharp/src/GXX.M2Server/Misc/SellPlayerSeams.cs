// ============================================================================
// SellPlayer.pas 的**接缝层**（M2Share.pas / UsrEngn.pas / M2Threads.pas / FastIniFile.pas 缺口）
// 车道 p8-m2-itemprop-misc ｜ 命名空间 GXX.M2Server.Misc
//
// 服务单元（源单元名一律写全路径，供 tools/audit-coverage.ps1 的 E2 证据规则识别）：
//   Source/M2Engine/SellPlayer.pas   —— 本文件
//   Source/M2Engine/UsrEngn.pas      —— TOffLineData、UserEngine.m_AutoLoadSellPlayerList /
//                                       m_boStartAutoLoadSellPlayer
//   Source/M2Engine/M2Share.pas      —— MyGetTickCount（M2Share.pas:3589 external timeGetTime）
//   Source/Common/FastIniFile/FastIniFile.pas —— ReadFixedDateTime :2953-2971 /
//                                       WriteFixedDateTime :2985-2989（GXX.Core.Util.TFastIniFile 缺这两个方法）
//   Source/M2Engine/M2Threads.pas    —— g_MultiThreadRun
//
// ★ 接缝纪律（《并行派发台账》§25.2）：**不得以"静默返回中性值"的默认实现掩盖缺陷**。
//   本文件的做法：
//     * `MyGetTickCount` **接线到真实现**（DelphiRTL.GetTickCount == timeGetTime 的 DWORD 语义），
//       不是中性值；
//     * `m_AutoLoadSellPlayerList` / `m_boStartAutoLoadSellPlayer` / `g_MultiThreadRun` 是**数据面**，
//       其默认值 = 原文构造期状态（空 TSafeList / False / False，UsrEngn.pas:473-475、M2Threads.pas），
//       属"照抄原文状态"，已在报告登记；
//     * `TOffLineData` 是**类型面**：正式归属是 UsrEngn.pas:14-21，托管侧此处只是占位替身，
//       待 UsrEngn.pas 移植批次落地后**必须删除本处定义**（报告 §6 越区请求/去重待办）。
// ============================================================================

using System.Globalization;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.M2Server.Misc;

// ---------------------------------------------------------------------------
// UsrEngn.pas:14-21 TOffLineData（接缝替身）
// ---------------------------------------------------------------------------

/// <summary>
/// UsrEngn.pas:14-21 <c>TOffLineData</c>（原文 <c>record</c> + <c>pTOffLineData = ^TOffLineData</c>）。
/// <para>
/// 托管侧为 class：原文以 <c>New(OffLineData)</c> 分配指针、按指针存入 TSafeList 并在
/// UsrEngn.pas:646 逐项 <c>Dispose</c>，故引用语义 == 指针语义。
/// </para>
/// <para>★ 正式归属 = UsrEngn.pas（UsrEngn 批次）。本处仅为 SellPlayer.pas 的最小接缝；正式定义落地后删除本类。</para>
/// </summary>
public sealed class TOffLineData
{
    /// <summary>sAccount: string（UsrEngn.pas:15）。</summary>
    public string sAccount = "";

    /// <summary>sCharName: string（:16）。</summary>
    public string sCharName = "";

    /// <summary>boStartLogin: Boolean（:17）。</summary>
    public bool boStartLogin;

    /// <summary>dwStartLoginTick: LongWord（:18）。</summary>
    public uint dwStartLoginTick;

    /// <summary>boPassWordSuccess: Boolean（:19）。</summary>
    public bool boPassWordSuccess;

    /// <summary>SessInfo: pTSessInfo（:20；接缝：TSessInfo 待 UsrEngn/TGate 批次移植后接入，此处只保留占位）。</summary>
    public object? SessInfo;
}

// ---------------------------------------------------------------------------
// M2Share / UsrEngn / M2Threads 接缝
// ---------------------------------------------------------------------------

/// <summary>
/// SellPlayer.pas（:52 uses M2Share, UsrEngn, M2Threads）用到的全局量与函数。
/// </summary>
public static class SellPlayerGlobals
{
    /// <summary>
    /// <c>UserEngine.m_AutoLoadSellPlayerList: TSafeList</c>（UsrEngn.pas:176，构造于 :475）。
    /// <para>
    /// ★ 数据面：托管侧用 <see cref="List{T}"/>，空列表即原文构造期状态（<c>TSafeList.Create</c>）。
    /// 正式接线待 UsrEngn.pas 移植批次（届时改为转调 TUserEngine 的真实列表）。
    /// </para>
    /// </summary>
    public static readonly List<TOffLineData> m_AutoLoadSellPlayerList = new();

    /// <summary><c>UserEngine.m_boStartAutoLoadSellPlayer: Boolean</c>（UsrEngn.pas:174；原文构造期 False，:473）。</summary>
    public static bool m_boStartAutoLoadSellPlayer;

    /// <summary><c>g_MultiThreadRun: Boolean</c>（M2Threads.pas）。原文构造期 False。</summary>
    public static bool g_MultiThreadRun;

    /// <summary>
    /// 原文 <c>MyGetTickCount: DWORD; stdcall; external mmsyst name 'timeGetTime'</c>
    /// （M2Share.pas:3589）。**接线到真实现** <see cref="DelphiRTL.GetTickCount"/>（uint 回绕），
    /// 非接缝臆造；测试可用它注入确定值（与 SweepSeam.MyGetTickCount 同一处置）。
    /// </summary>
    public static Func<uint> MyGetTickCount = DelphiRTL.GetTickCount;

    /// <summary>
    /// <c>m_AutoLoadSellPlayerList.LockW(Arg)</c> 调用记录（原文 :278 <c>LockW(2)</c>；MULTI_THREAD=1 时）。
    /// </summary>
    public static readonly List<(string Op, int Arg)> ListLockCalls = new();

    /// <summary>原文 <c>m_AutoLoadSellPlayerList.LockW(Arg)</c>（UsrEngn/TSafeList）。</summary>
    public static void LockWAutoLoadSellPlayerList(int arg) => ListLockCalls.Add(("LockW", arg));

    /// <summary>原文 <c>m_AutoLoadSellPlayerList.UnLockW</c>（:297）。</summary>
    public static void UnLockWAutoLoadSellPlayerList() => ListLockCalls.Add(("UnLockW", 0));

    /// <summary>复位全部接缝静态量（xUnit 串行集合里逐测调用）。</summary>
    public static void Reset()
    {
        m_AutoLoadSellPlayerList.Clear();
        m_boStartAutoLoadSellPlayer = false;
        g_MultiThreadRun = false;
        MyGetTickCount = DelphiRTL.GetTickCount;
        ListLockCalls.Clear();
        M2Server.Engine.M2ServerLog.Messages.Clear();
    }
}

// ---------------------------------------------------------------------------
// FastIniFile.pas 的固定格式日期时间（已回收至 GXX.Core，本类只保留转调）
// ---------------------------------------------------------------------------

/// <summary>
/// FastIniFile.pas 的固定格式日期时间读写（<c>:189-194</c>、<c>:988-1024</c>、<c>:2947-2989</c>）。
///
/// <para>
/// ★ <b>迁移记录（车道 p8-m2-itemprop-misc，请求 #2，已执行）</b>：
/// 这两份 1:1 实现原先因 <c>SellPlayer.pas:216/:255</c> 的需要临时落在本类里；
/// 现已按"<c>GXX.Core</c> 不得反向依赖 <c>GXX.M2Server</c>"的方向约束
/// **整体搬进 <c>GXX.Core.Util.TFastIniFile</c>（方法）与 <c>GXX.Core.Util.TIniFixedDateTime</c>（常量与解析器）**，
/// 本类降级为**纯转调**（保留 Delphi 单元级函数名，供对照与既有用例使用）。
/// </para>
/// <para>
/// 原文缺陷提醒（照抄，勿"修"）：<c>FastIniFile.pas:2967-2968</c> 里时间段解析的 Default 是 <c>0</c>、
/// 而判据是 <c>T &lt;&gt; -1</c> ⇒ **坏时间被静默归零**为当天 00:00:00。
/// </para>
/// </summary>
public static class SellPlayerIni
{
    /// <summary>FastIniFile.pas:190 <c>FIXED_DS</c>（转调 <see cref="TIniFixedDateTime"/>）。</summary>
    public const char FIXED_DS = TIniFixedDateTime.FIXED_DS;

    /// <summary>FastIniFile.pas:191 <c>FIXED_DATE = 'dd-mm-yyyy'</c>。</summary>
    public const string FIXED_DATE = TIniFixedDateTime.FIXED_DATE;

    /// <summary>FastIniFile.pas:192 <c>FIXED_TS</c>。</summary>
    public const char FIXED_TS = TIniFixedDateTime.FIXED_TS;

    /// <summary>FastIniFile.pas:193 <c>FIXED_TIME = 'hh:nn:ss'</c>。</summary>
    public const string FIXED_TIME = TIniFixedDateTime.FIXED_TIME;

    /// <summary>FastIniFile.pas:194 <c>FIXED_DATETIME</c>。</summary>
    public const string FIXED_DATETIME = TIniFixedDateTime.FIXED_DATETIME;

    /// <summary>FastIniFile.pas:2953-2971 <c>TFastIniFile.ReadFixedDateTime</c>（转调）。</summary>
    public static double ReadFixedDateTime(TFastIniFile ini, string section, string ident, double Default)
        => ini.ReadFixedDateTime(section, ident, Default);

    /// <summary>FastIniFile.pas:2985-2989 <c>TFastIniFile.WriteFixedDateTime</c>（转调）。</summary>
    public static void WriteFixedDateTime(TFastIniFile ini, string section, string ident, double Value)
        => ini.WriteFixedDateTime(section, ident, Value);

    /// <summary>Delphi <c>FormatDateTime('dd-mm-yyyy hh:nn:ss', Value)</c>（转调）。</summary>
    public static string FormatFixedDateTime(double value)
        => TIniFixedDateTime.FormatFixedDateTime(value);

    /// <summary>FastIniFile.pas:988-1007 局部 <c>StrToDateDef</c>（转调）。</summary>
    public static double StrToDateDef(string Value, double Default)
        => TIniFixedDateTime.StrToDateDef(Value, Default);

    /// <summary>FastIniFile.pas:1009-1024 局部 <c>StrToTimeDef</c>（转调）。</summary>
    public static double StrToTimeDef(string Value, double Default)
        => TIniFixedDateTime.StrToTimeDef(Value, Default);
}
