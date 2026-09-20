// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas（实测 10,546 LF，GBK）
// 本文件：ObjNpc.pas 的**接缝层**（结构体/枚举 + 宿主能力委托）。
//   · interface 段  1..495   —— 常量组 10-22、记录/类声明 25-495
//   · implementation 496..10545
//   · 具体覆盖行号见各 partial 文件头（ObjNpcTypes.cs / ObjNpcLevelBase.cs / …）
//
// 命名空间 `GXX.M2Server.Npc` 是本车道独占区（见派发任务书「路径隔离」）。
// 本文件**不重复实现**任何既有类型：
//   · TBaseObject / TPlayObject / TCreature → 复用 GXX.M2Server.Engine 既有接缝类；
//   · HUtil32 / DelphiRTL / Grobal2Const / TStringList → 复用 GXX.Core；
//   · 仅定义 ObjNpc.pas **本单元自己声明**的记录/类，以及宿主未移植能力的最小委托。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Util;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Npc;

/// <summary>
/// ObjNpc.pas:10-22 的 12 个单元级常量（1:1，成员名即原文常量名）。
/// Delphi 单元级 const → 托管侧静态常量类（§3.3 命名规则）。
/// </summary>
public static class ObjNpcConst
{
    /// <summary>原文 `CMD_RACE_0 = 0; // self`（ObjNpc.pas:10）。</summary>
    public const int CMD_RACE_0 = 0;
    /// <summary>原文 `CMD_RACE_1 = 1; // hero`（ObjNpc.pas:11）。</summary>
    public const int CMD_RACE_1 = 1;
    /// <summary>原文 `CMD_RACE_2 = 2; // Master`（ObjNpc.pas:12）。</summary>
    public const int CMD_RACE_2 = 2;
    /// <summary>原文 `CMD_RACE_3 = 3; // 被攻击的目标`（ObjNpc.pas:13）。</summary>
    public const int CMD_RACE_3 = 3;
    /// <summary>原文 `CMD_RACE_4 = 4; // obj`（ObjNpc.pas:14）。</summary>
    public const int CMD_RACE_4 = 4;
    /// <summary>原文 `CMD_RACE_5 = 5; // 变量名`（ObjNpc.pas:15）。</summary>
    public const int CMD_RACE_5 = 5;
    /// <summary>原文 `CMD_RACE_6 = 6; // 最后攻击者`（ObjNpc.pas:16）。</summary>
    public const int CMD_RACE_6 = 6;
    /// <summary>原文 `CMD_RACE_7 = 7;`（ObjNpc.pas:17）。</summary>
    public const int CMD_RACE_7 = 7;
    /// <summary>原文 `CMD_RACE_8 = 8;`（ObjNpc.pas:18）。</summary>
    public const int CMD_RACE_8 = 8;
    /// <summary>原文 `CMD_RACE_9 = 9;`（ObjNpc.pas:19）。</summary>
    public const int CMD_RACE_9 = 9;
    /// <summary>原文 `CMD_RACE_10 = 10;`（ObjNpc.pas:20）。</summary>
    public const int CMD_RACE_10 = 10;
    /// <summary>原文 `CMD_RACE_11 = 11;`（ObjNpc.pas:21）。</summary>
    public const int CMD_RACE_11 = 11;
    /// <summary>原文 `CMD_RACE_12 = 12;`（ObjNpc.pas:22）。</summary>
    public const int CMD_RACE_12 = 12;
}

// ---------------------------------------------------------------------------
// M2Definition.pas:60-77 —— ObjNpc.pas 通过 uses M2Definition 使用这四个类型。
// 该单元尚未移植到 M2Server（`GXX.M2Server.Plugins.PluginInterfaceSeams.cs:298`
// 另有一份**不同用途**的 `TDynamicVar` 接缝，命名空间不同，不冲突）。
// 按「不顺手移植依赖」原则，此处只落 ObjNpc.pas 用得到的最小面。
// 接缝：待 M2Definition.pas 移植后由该单元接管。
// ---------------------------------------------------------------------------

/// <summary>接缝：原文 `TVarType = (vNone, vInteger, vString);`（M2Definition.pas:60）。</summary>
public enum TVarType
{
    /// <summary>原文 `vNone`。</summary>
    vNone = 0,
    /// <summary>原文 `vInteger`。</summary>
    vInteger = 1,
    /// <summary>原文 `vString`。</summary>
    vString = 2,
}

/// <summary>接缝：原文 `TVarAttr = (aNone, aFixVar, aDynamic, aConst);`（M2Definition.pas:61）。</summary>
public enum TVarAttr
{
    /// <summary>原文 `aNone`。</summary>
    aNone = 0,
    /// <summary>原文 `aFixVar`。</summary>
    aFixVar = 1,
    /// <summary>原文 `aDynamic`。</summary>
    aDynamic = 2,
    /// <summary>原文 `aConst`。</summary>
    aConst = 3,
}

/// <summary>
/// 接缝：原文 `TVarInfo = packed record VarType: TVarType; VarAttr: TVarAttr; end;`
/// （M2Definition.pas:63-66，SizeOf = 2）。
/// </summary>
public struct TVarInfo
{
    /// <summary>原文 `VarType: TVarType`。</summary>
    public TVarType VarType;
    /// <summary>原文 `VarAttr: TVarAttr`。</summary>
    public TVarAttr VarAttr;
}

/// <summary>
/// 接缝：原文 `TDynamicVar = record sName: string[50]; VarType: TVarType;
/// nInternet: Integer; sString: string; end;`（M2Definition.pas:70-75）。
/// 注意 `GetDynamicValue`/`SetDynamicValue` 是**按引用就地改写**该记录的元素，
/// 故托管侧用 `class`（引用语义）而非 `struct`，以免 `List&lt;T&gt;` 取元素时复制。
/// </summary>
public class TDynamicVar
{
    /// <summary>原文 `sName: string[50]`。</summary>
    public string sName = "";
    /// <summary>原文 `VarType: TVarType`。</summary>
    public TVarType VarType;
    /// <summary>原文 `nInternet: Integer`（原文如此拼写，非 `nInternal`）。</summary>
    public int nInternet;
    /// <summary>原文 `sString: string`。</summary>
    public string sString = "";
}

/// <summary>
/// ObjNpc.pas 的宿主能力接缝（原文依赖 M2Share / UsrEngn / Envir / HandleNpcCmds 等
/// **另一会话常驻区且未移植**的单元）。全部为可替换委托，默认实现 = "无宿主"
/// （返回 null / 空操作），单测通过注入委托驱动行为，无需真实宿主。
/// 这些都不是 ObjNpc.pas 自身的声明，故集中在此，待对应单元移植后一处接入。
/// </summary>
public static class NpcSeams
{
    /// <summary>
    /// 原文 `g_nKey_HeroExt`（M2Share.pas 全局；ObjNpc.pas 在 558/565/645/652/803/820/985/1002 处读取）。
    /// 接缝：待 M2Share.pas 的该全局移植后接入。
    /// </summary>
    public static int g_nKey_HeroExt { get; set; }

    /// <summary>
    /// 原文 `UserEngine.GetPlayObject(sCharName): TPlayObject`（UsrEngn.pas）。
    /// ObjNpc.pas:779（Condition）与 :959（Action）处调用。
    /// 接缝：待 UsrEngn.pas 的 TUserEngine 提供该查询后接入。
    /// </summary>
    public static Func<string, TPlayObject?> GetPlayObject { get; set; } = _ => null;

    /// <summary>
    /// 原文 `TPlayObject.m_MyHero`（ObjPlayer.pas 字段，托管侧 Engine.TPlayObject 尚未声明）。
    /// ObjNpc.pas:708/805/807/822/824/987/989/1004/1006 处读取。
    /// 接缝：待 ObjPlayer.pas 的 m_MyHero 落地后接入。
    /// </summary>
    public static Func<TPlayObject, TCreature?> GetMyHero { get; set; } = _ => null;

    /// <summary>
    /// 原文 `TBaseObject.m_CurrTarget`（ObjBase.pas:94 类字段，托管侧 Engine.TCreature 尚未声明）。
    /// ObjNpc.pas:736/807/916/989 处读取。
    /// </summary>
    public static Func<TCreature, TCreature?> GetCurrTarget { get; set; } = _ => null;

    /// <summary>
    /// 原文 `TBaseObject.m_LastHiter`（ObjBase.pas:94 类字段）。
    /// ObjNpc.pas:791/824/973/1006 处读取。
    /// </summary>
    public static Func<TCreature, TCreature?> GetLastHiter { get; set; } = _ => null;

    /// <summary>
    /// 原文 `TBaseObject.GetPoseCreate: TBaseObject`（ObjBase.pas）。
    /// ObjNpc.pas:750/930 处调用。
    /// </summary>
    public static Func<TCreature, TCreature?> GetPoseCreate { get; set; } = _ => null;

    /// <summary>
    /// 原文 `TCopyMon`（ObjMon2.pas，`CMD_RACE_11` 的 `TempObject is TCopyMon` 判定，ObjNpc.pas:1062）。
    /// 接缝：用"该对象的服务端种族值"表达类型判定，待 TCopyMon 移植后改为 `is` 判定。
    /// </summary>
    public static Func<TCreature, bool> IsCopyMon { get; set; } = _ => false;

    /// <summary>
    /// 原文 `g_FunctionNPC` / `g_MissionNPC`（M2Share.pas 全局 TNormNpc，仅用于
    /// `AllowSelect` 的诊断输出判定，ObjNpc.pas:5943）。
    /// </summary>
    public static Func<TNormNpc, bool> IsFunctionOrMissionNpc { get; set; } = _ => false;

    /// <summary>原文 `MainOutMessage(sMsg)`（M2Share.pas，ObjNpc.pas:5945 调用）。</summary>
    public static Action<string> MainOutMessage { get; set; } = _ => { };

    /// <summary>
    /// 原文 `GetValNameNo(sText): Integer`（M2Share.pas:11453）。
    /// 托管侧已有 1:1 实现 `GXX.M2Server.Engine.CombatPowerUtils.GetValNameNo`，直接转发（不复制第二份）。
    /// </summary>
    public static Func<string, int> GetValNameNo { get; set; } = CombatPowerUtils.GetValNameNo;

    /// <summary>
    /// 原文 `TNormNpc.GetVariableText(PlayObject: TPlayObject; var sMsg: string; sVariable: string;
    /// var IsBreakParseVar: Boolean; nPos: Integer = 0): Boolean`（ObjNpc.pas:6011-9262，**3,252 行**，本车道未覆盖）。
    /// <para>接缝签名映射（`var` 参数用返回值回传）：
    /// 入参 (Npc, PlayObject, sMsg, sVariable, nPos) →
    /// 返回 (Result, NewSMsg, NewIsBreakParseVar)。</para>
    /// <para>本接缝只用于 `GetLineVariableText`（5981-6009）等已覆盖调用点的转发；
    /// 默认返回 `Result = False`（= 原文 `GetVariableText` 的失败分支），
    /// 此时 `GetLineVariableText` 走 `nStartPos := nPos + 2`（原文 6001-6002）。</para>
    /// </summary>
    public static Func<TNormNpc, TPlayObject, string, string, int, (bool Result, string SMsg, bool IsBreakParseVar)>
        GetVariableText { get; set; } =
        (_, _, sMsg, _, _) => (false, sMsg, false);

    /// <summary>
    /// 还原全部接缝为默认（"无宿主"）状态 —— 单测之间互相隔离用。
    /// （非原文成员，仅为接缝层的卫生函数。）
    /// </summary>
    public static void ResetDefaults()
    {
        g_nKey_HeroExt = 0;
        GetPlayObject = _ => null;
        GetMyHero = _ => null;
        GetCurrTarget = _ => null;
        GetLastHiter = _ => null;
        GetPoseCreate = _ => null;
        IsCopyMon = _ => false;
        IsFunctionOrMissionNpc = _ => false;
        MainOutMessage = _ => { };
        GetValNameNo = CombatPowerUtils.GetValNameNo;
        GetVariableText = (_, _, sMsg, _, _) => (false, sMsg, false);
    }
}
