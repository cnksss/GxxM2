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

    /// <summary>接缝：原文 `LOG_ActionNone = 00`（M2Share.pas:87）。</summary>
    public const byte LOG_ActionNone = 0;

    /// <summary>接缝：原文 `LOG_ItemDisappear = 09`（M2Share.pas:96）。
    /// 注：`GXX.LogDataServer.LogManage.cs:28` 有一份同值常量，但**跨工程**（M2Server 未引用 LogDataServer），故此处按原文值独立声明。</summary>
    public const byte LOG_ItemDisappear = 9;
}

// ---------------------------------------------------------------------------
// M2Definition.pas:215-230 —— ObjNpc.pas 通过 `m_ScriptList` 使用的两个记录。
// 该单元尚未移植。按「不顺手移植依赖」原则只落 ObjNpc.pas 用得到的最小面。
// 接缝：待 M2Definition.pas 移植后由该单元接管。
// ---------------------------------------------------------------------------

/// <summary>接缝：原文 `TQuestInfo = record wFlag: Word; btValue: Byte; nRandRage: Integer; end;`
/// （M2Definition.pas:215-219）。</summary>
public struct TQuestInfo
{
    /// <summary>原文 `wFlag: Word;`（M2Definition.pas:216）。</summary>
    public ushort wFlag;
    /// <summary>原文 `btValue: Byte;`（M2Definition.pas:217）。</summary>
    public byte btValue;
    /// <summary>原文 `nRandRage: Integer;`（M2Definition.pas:218）。</summary>
    public int nRandRage;
}

/// <summary>
/// 接缝：原文 `TScript = record boQuest: Boolean; QuestInfo: array[0..9] of TQuestInfo;
/// nQuest: Integer; RecordList: TList; end;`（M2Definition.pas:223-228）。
/// ObjNpc.pas 只使用 `RecordList`（4383-4429 的 ClearScript、9996-10017 的 DoSort）。
/// `TList` → 托管 `List&lt;object&gt;`。
/// </summary>
public class TScript
{
    /// <summary>原文 `boQuest: Boolean;`（M2Definition.pas:224）。</summary>
    public bool boQuest;
    /// <summary>原文 `QuestInfo: array [0 .. 9] of TQuestInfo;`（M2Definition.pas:225）。</summary>
    public TQuestInfo[] QuestInfo = new TQuestInfo[10];
    /// <summary>原文 `nQuest: Integer;`（M2Definition.pas:226）。</summary>
    public int nQuest;
    /// <summary>原文 `RecordList: TList;`（M2Definition.pas:227）。</summary>
    public List<object> RecordList = new();
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

    // -----------------------------------------------------------------------
    // 以下为 `TNormNpc.GetValNameValue`(5690-5877) 需要、但托管侧**尚无对应存储**的
    // 宿主面。全部是接缝，待 ObjPlayer.pas / M2Share.pas 移植后接入。
    // -----------------------------------------------------------------------

    /// <summary>原文 `TPlayObject.m_nVal[n01]`（ObjPlayer.pas，P 变量 0..999）。接缝。</summary>
    public static Func<TPlayObject, int, int> GetPlayerPVal { get; set; } = (_, _) => 0;

    /// <summary>原文 `TPlayObject.m_sString[n01 - 7000]`（ObjPlayer.pas，S 变量 0..999）。接缝。</summary>
    public static Func<TPlayObject, int, string> GetPlayerSString { get; set; } = (_, _) => "";

    /// <summary>原文 `TPlayObject.m_TVal[n01 - 8500]`（ObjPlayer.pas，T 私有字符串变量 0..499）。接缝。</summary>
    public static Func<TPlayObject, int, string> GetPlayerTVal { get; set; } = (_, _) => "";

    /// <summary>原文 `TPlayObject.m_ArrayList.GetIndex/ Strings`（ObjPlayer.pas，L$ 数组变量）。接缝。</summary>
    public static Func<TPlayObject, string, string> GetPlayerArrayListValue { get; set; } = (_, _) => "";

    /// <summary>原文 `g_Config.GlobaDyMval[n01 - 4000]`（M2Share.pas，I 全局数字变量 0..999）。接缝。</summary>
    public static Func<int, int> GetGlobaDyMval { get; set; } = _ => 0;

    /// <summary>原文 `g_Config.GlobalVal[n01 - 5000]`（M2Share.pas，G 全局数字变量 0..999）。接缝。</summary>
    public static Func<int, int> GetGlobalVal { get; set; } = _ => 0;

    /// <summary>
    /// 原文 `g_Config.GlobalAVal[n01 - 6000]`（M2Share.pas，A 全局字符串变量 0..999）。
    /// 托管侧 `GXX.M2Server.Engine.InterServerState.GlobalAVal`（InterServerState.cs:150）已有同布局数组，
    /// 但 M2Share 的 `g_Config` 归属尚未定案，故仍走接缝，默认读同一数组。
    /// </summary>
    public static Func<int, string> GetGlobalAVal { get; set; } = i => InterServerState.GlobalAVal[i];

    /// <summary>
    /// 原文 `TPlayObject.m_DynamicVarList`（ObjPlayer.pas，人物动态变量表，ObjNpc.pas:9882）。
    /// 接缝：待 ObjPlayer.pas 移植后接入。
    /// </summary>
    public static Func<TPlayObject, List<TDynamicVar>> GetPlayerDynamicVarList { get; set; } = _ => new();

    /// <summary>
    /// 原文 `TPlayObject.m_MyGuild` 的判空（ObjNpc.pas:9887）与 `TGUild(...).sGuildName`（9891）。
    /// 返回 null 表示 `m_MyGuild = nil`。接缝：待 Guild.pas / ObjPlayer.pas 移植后接入。
    /// </summary>
    public static Func<TPlayObject, string?> GetPlayerGuildName { get; set; } = _ => null;

    /// <summary>
    /// 原文 `TGUild(PlayObject.m_MyGuild).m_DynamicVarList`（ObjNpc.pas:9890）。接缝。
    /// </summary>
    public static Func<TPlayObject, List<TDynamicVar>> GetGuildDynamicVarList { get; set; } = _ => new();

    /// <summary>
    /// 原文 `g_DynamicVarList`（M2Share.pas 全局，ObjNpc.pas:9895；其 sName 固定为 `'GLOBAL'`）。
    /// 接缝：待 M2Share.pas 移植后接入。
    /// </summary>
    public static Func<List<TDynamicVar>> GetGlobalDynamicVarList { get; set; } = () => new();

    /// <summary>
    /// 原文 `TNormNpc.SetValNameValue`（ObjNpc.pas:4935-5325，**391 行**，本车道未覆盖）。
    /// 默认返回 `False`（= 原文未命中分支）。
    /// </summary>
    public static Func<TNormNpc, TPlayObject, string, string, int, bool>
        SetValNameValue { get; set; } = (_, _, _, _, _) => false;

    /// <summary>
    /// 原文 `GetBoxItemValue(sVar: string; PlayObject: TPlayObject; var Ret: string): Boolean`
    /// （ObjNpc.pas:5326-5689，**364 行**，本车道未覆盖）。返回 (Result, Ret)。
    /// </summary>
    public static Func<string, TPlayObject, (bool Result, string Ret)> GetBoxItemValue { get; set; } =
        (_, _) => (false, "");

    /// <summary>
    /// 原文 `SetBoxItemValue(sVar: string; PlayObject: TPlayObject; sValue: string; nValue: Integer): Boolean`
    /// （ObjNpc.pas:4645-4934，**290 行**，本车道未覆盖）。
    /// </summary>
    public static Func<string, TPlayObject, string, int, bool> SetBoxItemValue { get; set; } =
        (_, _, _, _) => false;

    /// <summary>
    /// 原文 `Random(n)`（Delphi RTL System.pas）：返回 `0..n-1`，且 **`Random(0) = 0`**（不抛异常）。
    /// ObjNpc.pas:1084（`CMD_RACE_12`）处使用。用接缝是为了单测可确定性注入。
    /// </summary>
    public static Func<int, int> Random { get; set; } = _DelphiRandom;

    // -----------------------------------------------------------------------
    // TMerchant 价格族（1446-1511 / 1630-1681 / 2052-2086 / 3160-3178 / 3272-3365）
    // 需要的宿主面。
    // -----------------------------------------------------------------------

    /// <summary>
    /// 原文 `UserEngine.GetStdItem(wIndex): pTStdItem`（UsrEngn.pas）。
    /// ObjNpc.pas:1481 / 1665 / 3284 处调用。`TStdItem` 在托管侧是值类型结构 → 用 `Nullable` 表达 nil。
    /// 接缝：待 UsrEngn.pas 提供该查询后接入。
    /// </summary>
    public static Func<int, TStdItem?> GetStdItem { get; set; } = _ => null;

    /// <summary>
    /// 原文 `FrmDB.SaveGoodPriceRecord(Self, m_sScript + '-' + m_sMapName)`（LocalDB.pas 全局 FrmDB；
    /// ObjNpc.pas:1454 调用）。接缝：待 LocalDB.pas 移植后接入。
    /// </summary>
    public static Action<TMerchant, string> SaveGoodPriceRecord { get; set; } = (_, _) => { };

    /// <summary>
    /// 原文 `TBaseObject.m_Castle`（ObjBase/ObjGame 基类字段，托管侧 Engine.TCreature 尚未声明）。
    /// ObjNpc.pas:2068-2079 的城堡价分支使用。返回 null 表示 `m_Castle = nil`。
    /// </summary>
    public static Func<TNormNpc, object?> GetNpcCastle { get; set; } = _ => null;

    /// <summary>
    /// 原文 `TUserCastle(m_Castle).IsMasterGuild(TGUild(PlayObject.m_MyGuild))`
    /// （Castle.pas；ObjNpc.pas:2071 调用）。接缝：待 Castle.pas 的 TUserCastle 接入。
    /// </summary>
    public static Func<object, TPlayObject, bool> IsMasterGuild { get; set; } = (_, _) => false;

    // -----------------------------------------------------------------------
    // FrmDB（LocalDB.pas 全局）落盘/读盘接口 —— ObjNpc.pas 的脚本/图标/商品/升级武器
    // 四类存取全走它。LocalDB.pas 整单元未移植，故全部接缝。
    // -----------------------------------------------------------------------

    /// <summary>原文 `FrmDB.LoadGoodRecord(Self, sFile)`（ObjNpc.pas:3057）。接缝。</summary>
    public static Action<TMerchant, string> LoadGoodRecord { get; set; } = (_, _) => { };

    /// <summary>原文 `FrmDB.SaveGoodRecord(Self, sFile)`（ObjNpc.pas:3067）。接缝。</summary>
    public static Action<TMerchant, string> SaveGoodRecord { get; set; } = (_, _) => { };

    /// <summary>原文 `FrmDB.LoadGoodPriceRecord(Self, sFile)`（ObjNpc.pas:3058）。接缝。</summary>
    public static Action<TMerchant, string> LoadGoodPriceRecord { get; set; } = (_, _) => { };

    /// <summary>原文 `FrmDB.LoadUpgradeWeaponRecord(sFile, m_UpgradeWeaponList)`（ObjNpc.pas:4207）。接缝。</summary>
    public static Action<string, List<object>> LoadUpgradeWeaponRecord { get; set; } = (_, _) => { };

    /// <summary>原文 `FrmDB.SaveUpgradeWeaponRecord(sFile, m_UpgradeWeaponList)`（ObjNpc.pas:1678）。接缝。</summary>
    public static Action<string, List<object>> SaveUpgradeWeaponRecord { get; set; } = (_, _) => { };

    /// <summary>原文 `FrmDB.LoadNpcScript(Self, sPath, sName)`（ObjNpc.pas:9583/9589）。接缝。</summary>
    public static Action<TNormNpc, string, string> LoadNpcScriptFile { get; set; } = (_, _, _) => { };

    /// <summary>原文 `FrmDB.LoadScriptFile(Self, sPath, sName, True)`（ObjNpc.pas:3201）。接缝。</summary>
    public static Action<TMerchant, string, string> LoadScriptFile { get; set; } = (_, _, _) => { };

    /// <summary>
    /// 原文 `FrmDB.LoadIconFile(Self, @m_ActorIcons, sNpcIcons, sName)`（ObjNpc.pas:3202/3225/9584/9600）。
    /// <para>接缝签名**吞掉了 `@m_ActorIcons`**：该字段原文在 `TBaseObject`（ObjBase/ObjGame）上，
    /// 托管侧 `Engine.TCreature` 尚无 —— 与本车道已上报的 6 个 Engine 成员同性质（见报告 §6.1-C）。
    /// 由接入方在其实现内自行取该数组，故本接缝只暴露 (npc, sNpcIcons, sName) 三元组。</para>
    /// </summary>
    public static Action<TNormNpc, string, string> LoadIconFile { get; set; } = (_, _, _) => { };

    // -----------------------------------------------------------------------
    // M2Share.pas:378-381 的目录常量（原文以 const 形式被 ObjNpc.pas 直接引用）。
    // 用可替换属性而非 const：M2Share 移植后此处一处改接。
    // -----------------------------------------------------------------------

    /// <summary>接缝：原文 `sMarket_Def = 'Market_Def\'`（M2Share.pas:378）。</summary>
    public static string sMarket_Def { get; set; } = @"Market_Def\";

    /// <summary>接缝：原文 `sNpc_def = 'Npc_def\'`（M2Share.pas:379）。</summary>
    public static string sNpc_def { get; set; } = @"Npc_def\";

    /// <summary>接缝：原文 `sNpcIcons = 'NpcIcons\'`（M2Share.pas:381）。</summary>
    public static string sNpcIcons { get; set; } = @"NpcIcons\";

    /// <summary>
    /// 原文 `UserEngine.GetStdItemName(wIndex): string`（UsrEngn.pas）。
    /// ObjNpc.pas:1712（升级材料名比较）与 3259（`$USERWEAPON`）调用。
    /// 接缝：待 UsrEngn.pas 提供该查询后接入。
    /// </summary>
    public static Func<int, string> GetStdItemName { get; set; } = _ => "";

    /// <summary>
    /// 原文 `TPlayObject.m_UseItems[U_WEAPON]`（ObjPlayer.pas 装备格数组）。
    /// ObjNpc.pas:3257/3259 只**读**该格。接缝：待 ObjPlayer.pas 移植后接入。
    /// </summary>
    public static Func<TPlayObject, TUserItem> GetUseItemsWeapon { get; set; } = _ => default;

    /// <summary>
    /// 原文 `TNormNpc.SendCustemMsg`（ObjNpc.pas:9837-9862，本车道未覆盖）—— 供
    /// `TMerchant.SendCustemMsg`（4235-4238）的 `inherited` 转发。默认无操作。
    /// </summary>
    public static Action<TNormNpc, TPlayObject, string> SendCustemMsg { get; set; } = (_, _, _) => { };

    // -----------------------------------------------------------------------
    // TMerchant.UpgradeWapon 的嵌套过程 sub_4A0218（ObjNpc.pas:1686-1828）需要的最小宿主面。
    // -----------------------------------------------------------------------

    /// <summary>
    /// 原文 `ItemUnit.GetItemAddValue(UserItem: pTUserItem; var StdItem: TStdItem)`
    /// （ItemUnit.pas；ObjNpc.pas:1733 调用）。
    /// <para>两参**都**按引用：`UserItem` 原文是指针（可就地改写），`StdItem` 原文是 `var`。
    /// 接缝：`GXX.M2Server.Engine.AddAbility.cs:76` 已注明"随物品升级批次接入"，尚未落地。</para>
    /// </summary>
    public delegate void GetItemAddValueDelegate(ref TUserItem userItem, ref TStdItem stdItem);

    /// <summary>原文 `ItemUnit.GetItemAddValue`（ObjNpc.pas:1733）。接缝（默认无操作）。</summary>
    public static GetItemAddValueDelegate GetItemAddValue { get; set; } = (ref TUserItem _, ref TStdItem _) => { };

    /// <summary>
    /// 原文 `AddGameDataLog(LogAction1, LogAction2: Byte; LogActor: TBaseObject; ItemName: string;
    /// ItemMakeIndex: Integer; TargetName: string; Data1: Integer; Data2: Integer; LogDesc: string)`
    /// （M2Share.pas:11686-11687，ObjNpc.pas:1719/1794 调用）。接缝：待 M2Share 移植后接入。
    /// </summary>
    public static Action<byte, byte, TCreature, string, int, string, int, int, string> AddGameDataLog { get; set; } =
        (_, _, _, _, _, _, _, _, _) => { };

    /// <summary>
    /// 原文 `g_Config.sBlackStone`（M2Share.pas:918 声明 / :4142 默认值 `'黑铁矿'`）。
    /// ObjNpc.pas:1712/1715 用它识别升级材料。接缝：待 M2Share 的 g_Config 接入。
    /// </summary>
    public static string sBlackStone { get; set; } = "黑铁矿";

    /// <summary>
    /// 原文 `IsUseItem(nIndex): Boolean`（M2Share.pas:11660-11669）：
    /// `StdItem.StdMode in [19,20,21,22,23,24,26]`。
    /// <para><b>默认实现即原文 1:1 逻辑</b>（只依赖本类已有的 <see cref="GetStdItem"/>），
    /// 故这里不是"另造一份"，而是把 M2Share 的 10 行判定挂在接缝上；
    /// M2Share 移植后改为转调其正式实现即可。</para>
    /// <para><b>原文缺陷（照抄）</b>：11664-11665 **没有 `StdItem &lt;&gt; nil` 检查** ——
    /// `GetStdItem` 返回 nil 时原文读 `StdItem.StdMode` 会 AV；托管侧默认实现读 `Nullable.Value`
    /// 抛 `InvalidOperationException`（等价"未定义行为即崩溃"）。</para>
    /// </summary>
    public static Func<int, bool> IsUseItem { get; set; } = nIndex =>
    {
        TStdItem? StdItem = GetStdItem(nIndex);
        return StdItem.Value.StdMode is 19 or 20 or 21 or 22 or 23 or 24 or 26;
    };

    /// <summary>
    /// 原文 `User.SendMsg(Self, wIdent, wParam, nParam1, nParam2, nParam3, sMsg)`
    /// （`TBaseObject.SendMsg`，ObjNpc.pas:1719/1794/1825 等处的**网络下发**版，
    /// 与托管侧 `Engine.TCreature.SendMsg`（入消息队列版，签名无 sender）**不是同一个方法**）。
    /// <para>接缝签名：(sender, target, wIdent, wParam, nParam1, nParam2, nParam3, sMsg)。</para>
    /// </summary>
    public static Action<TCreature, TCreature, ushort, long, long, long, long, string> SendMsgToClient { get; set; } =
        (_, _, _, _, _, _, _, _) => { };

    private static readonly System.Random _Rnd = new();

    private static int _DelphiRandom(int range)
    {
        // Delphi Random(0) = 0（不抛异常）；.NET Random.Next(0) 同样返回 0。
        if (range <= 0) return 0;
        return _Rnd.Next(range);
    }

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
        GetPlayerPVal = (_, _) => 0;
        GetPlayerSString = (_, _) => "";
        GetPlayerTVal = (_, _) => "";
        GetPlayerArrayListValue = (_, _) => "";
        GetGlobaDyMval = _ => 0;
        GetGlobalVal = _ => 0;
        GetGlobalAVal = i => InterServerState.GlobalAVal[i];
        GetPlayerDynamicVarList = _ => new();
        GetPlayerGuildName = _ => null;
        GetGuildDynamicVarList = _ => new();
        GetGlobalDynamicVarList = () => new();
        SetValNameValue = (_, _, _, _, _) => false;
        GetBoxItemValue = (_, _) => (false, "");
        SetBoxItemValue = (_, _, _, _) => false;
        Random = _DelphiRandom;
        GetStdItem = _ => null;
        SaveGoodPriceRecord = (_, _) => { };
        GetNpcCastle = _ => null;
        IsMasterGuild = (_, _) => false;
        LoadGoodRecord = (_, _) => { };
        SaveGoodRecord = (_, _) => { };
        LoadGoodPriceRecord = (_, _) => { };
        LoadUpgradeWeaponRecord = (_, _) => { };
        SaveUpgradeWeaponRecord = (_, _) => { };
        LoadNpcScriptFile = (_, _, _) => { };
        LoadScriptFile = (_, _, _) => { };
        LoadIconFile = (_, _, _) => { };
        sMarket_Def = @"Market_Def\";
        sNpc_def = @"Npc_def\";
        sNpcIcons = @"NpcIcons\";
        GetStdItemName = _ => "";
        GetUseItemsWeapon = _ => default;
        SendCustemMsg = (_, _, _) => { };
        GetItemAddValue = (ref TUserItem _, ref TStdItem _) => { };
        AddGameDataLog = (_, _, _, _, _, _, _, _, _) => { };
        sBlackStone = "黑铁矿";
        IsUseItem = nIndex =>
        {
            TStdItem? StdItem = GetStdItem(nIndex);
            return StdItem.Value.StdMode is 19 or 20 or 21 or 22 or 23 or 24 or 26;
        };
        SendMsgToClient = (_, _, _, _, _, _, _, _) => { };
    }
}
