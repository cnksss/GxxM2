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

    /// <summary>接缝：原文 `LOG_ItemSell = 10; // 卖出物品`（M2Share.pas:97）。</summary>
    public const byte LOG_ItemSell = 10;

    /// <summary>接缝：原文 `LOG_ItemBuy = 11; // 买入物品`（M2Share.pas:98）。</summary>
    public const byte LOG_ItemBuy = 11;

    /// <summary>接缝：原文 `LOG_ItemUpgrade = 14; // 物品升级`（M2Share.pas:101）。</summary>
    public const byte LOG_ItemUpgrade = 14;

    // ★ 第十二轮：`sNF_Upgradeing/OK/Fail` 三条**已移出本类** → `NpcProcessCmd`
    //   （原文同属 `NpcCommon.pas:75/77/79`，单一来源）。

    /// <summary>接缝：原文 `LOG_GoldChange = 50; // 金币改变`（M2Share.pas:116）。</summary>
    public const byte LOG_GoldChange = 50;

    /// <summary>接缝：原文 `sSTRING_GOLDNAME = '金币'`（M2Share.pas:210）。</summary>
    public const string sSTRING_GOLDNAME = "金币";

    // ★ 第十二轮：`sNF_Repair`/`sNF_RepairOK`/`nNF_SuperRepair`/`nNF_Repair` 四条**已移出本类**，
    //   统一声明在 `NpcProcessCmd`（原文同属 `NpcCommon.pas`，单一来源）。
    //   同理 `sNF_Upgradeing/OK/Fail` 也迁到 `NpcProcessCmd`。

    /// <summary>
    /// 接缝：原文 `TUserItemBindValueType` 的 `ubNoSell { 禁止出售 }`（M2Share.pas:395；
    /// 枚举序 `ubNoDrop=0, ubNoDeal=1, ubNoStorage=2, ubNoRepair=3, ubNoSell=4`）。
    /// </summary>
    public const int ubNoSell = 4;
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

    // -----------------------------------------------------------------------
    // ★ 已去接缝（2026 第三轮）：原文四个基类/玩家成员已在 `Engine/PlayerSurface/**` 落地
    //   （车道 `p6-m2-playersurface`），本文件原先的
    //     GetMyHero / GetCurrTarget / GetLastHiter / GetPoseCreate
    //   四个委托**已删除**，调用点改为直接读成员：
    //     `TPlayObject.m_MyHero`（PlayerSurface.Hero.cs:31）
    //     `TCreature.m_CurrTarget` / `m_LastHiter`（PlayerSurface.Base.cs:202/199）
    //     `TCreature.GetPoseCreate()`（PlayerSurface.Base.cs:362）
    //   这样做同时消除了"接缝臆造"风险 —— 原先四个委托的默认实现恒返回 null，
    //   会把"字段语义错"伪装成"分支没命中"。
    // -----------------------------------------------------------------------

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
    // ★ 已去接缝（2026 第三轮）：原文的 4 类**玩家变量容器**已在
    //   `Engine/PlayerSurface/TPlayObject.PlayerSurface.Vars.cs` 落地，本文件原先的
    //     GetPlayerPVal / GetPlayerSString / GetPlayerTVal / GetPlayerArrayListValue
    //   四个委托**已删除**，`GetValNameValue` 改为直接读：
    //     `m_nVal[1000]`(:52) / `m_sString[1000]`(:68) / `m_TVal[500]`(:60) / `m_ArrayList`(:76)
    // -----------------------------------------------------------------------

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
    /// 原文 `UserEngine.GetStdItem(wIndex): pTStdItem`（UsrEngn.pas / ObjPlayer.pas:3369/12648）。
    /// ObjNpc.pas:1481 / 1665 / 3284 处调用。`TStdItem` 在托管侧是值类型结构 → 用 `Nullable` 表达 nil。
    /// <para><b>★ 去重（2026 第三轮）</b>：本属性已改为**转发**到 Engine 侧的唯一存储
    /// `Engine.PlayerSurfaceMsgSeams.GetStdItem`（`TPlayObject.PlayerSurface.Gold.cs:63`；
    /// `PlayerSurfaceItemSeams.GetStdItem` 也转发到同一处）—— 三个名字**一个后备存储**，不再是三份独立接缝。</para>
    /// </summary>
    public static Func<int, TStdItem?> GetStdItem
    {
        get => PlayerSurfaceMsgSeams.GetStdItem;
        set => PlayerSurfaceMsgSeams.GetStdItem = value;
    }

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
    /// 原文 `UserEngine.GetStdItemName(wIndex): string`（ObjBase.pas:41678）。
    /// ObjNpc.pas:1712（升级材料名比较）与 3259（`$USERWEAPON`）调用。
    /// <para><b>★ 去重（2026 第三轮）</b>：转发到 Engine 唯一存储
    /// `Engine.PlayerSurfaceMsgSeams.GetStdItemName`（`TPlayObject.PlayerSurface.Gold.cs:60`）。</para>
    /// </summary>
    public static Func<int, string> GetStdItemName
    {
        get => PlayerSurfaceMsgSeams.GetStdItemName;
        set => PlayerSurfaceMsgSeams.GetStdItemName = value;
    }

    // ★ 2026 第十一轮：`GetUseItemsWeapon` 与 `SetUseItemsWeapon` 两个替身接缝**已删除**。
    //   `Engine/RecalcChain.cs` 的 `m_UseItems` 已按方案 A 统一为权威类型 **`TUserItem?[]`**
    //   （与 `m_ItemList` 同口径，报告 §17），故 ObjNpc 侧一律**直读直写**：
    //   `User.m_UseItems[UseSlots.U_WEAPON]`（读）/ `User.m_UseItems[UseSlots.U_WEAPON] = item;`（写回）。
    //   这正是"正式归属落地后去掉替身"。

    /// <summary>
    /// 原文 `TNormNpc.SendCustemMsg`（ObjNpc.pas:9837-9862）—— **已由虚方法外壳升级为真实现**
    /// （见 `ObjNpcVars.cs` / `ObjNpcConversation.cs`），故本委托**已删除**；
    /// `TMerchant`/`TGuildOfficial` 的 `inherited` 现在直接走 `base.SendCustemMsg(...)`。
    /// </summary>
    // (已移除：NpcSeams.SendCustemMsg)

    /// <summary>
    /// 原文 `g_Config.boSendCustemMsg`（M2Share.pas；ObjNpc.pas:9841 的"喊话"总开关）。
    /// 接缝：`M2Config` 暂无同名字段。默认 `false`（= 原文默认关闭）。
    /// </summary>
    public static bool boSendCustemMsg { get; set; }

    /// <summary>
    /// 原文 `g_sSendCustMsgCanNotUseNowMsg`（M2Share.pas；ObjNpc.pas:9843 的提示串）。
    /// 接缝：原文由 `M2Share.LoadString` 从资源载入、源码只有键名 —— 此处为**语义占位，非原文文案**。
    /// </summary>
    public static string g_sSendCustMsgCanNotUseNowMsg { get; set; } = "当前无法使用喊话功能";

    /// <summary>
    /// 原文全局 `g_FilterTexts`（ObjNpc.pas:9847 的敏感词过滤器）。
    /// 托管侧 `Engine.TFilterTexts`（`Engine/FilterTexts.cs:24`，含 `Filter(string, out string)`）
    /// 已存在，但**没有单元级全局实例**（唯一实例挂在 `Forms/ViewList2Form.cs:55` 上）→ 接缝。
    /// 返回 null 表示"无过滤器"（原文的 `g_FilterTexts = nil` 分支）。
    /// </summary>
    public static Func<TFilterTexts?> GetFilterTexts { get; set; } = () => null;

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
    /// 原文 `function OverLapItems(BaseObject: TBaseObject; StdItem: pTStdItem; wDura: Word): pTUserItem;`
    /// （`ObjBase.pas:1343` 声明 / `:1873` 实现；`ObjPlayer.pas:31435` 另有一个二参重载）。
    /// ObjNpc.pas:3441 用三参版 `OverLapItems(PlayObject, StdItem, nCount - 1)`：
    /// 在**玩家背包**里找一件"可与之叠加"的同名物品（找到返回该件，找不到返回 nil）。
    /// 接缝：待 ObjBase.pas 的 OverLapItems 移植后接入。
    /// </summary>
    public static Func<TPlayObject, TStdItem, ushort, TUserItem?> OverLapItems { get; set; } =
        (_, _, _) => null;

    /// <summary>
    /// 原文 `function CopyToUserItemFromName(sItemName: string; Item: pTUserItem): Boolean;`
    /// （`UsrEngn.pas:284`）。按物品名把标准物品数据**填进** `Item`（并分配 `MakeIndex`），
    /// 失败返回 False（ObjNpc.pas:3553 的 `if ... then ... else Dispose` 分支）。
    /// <para>`Item` 原文是指针、且调用方随后读 `OverLapItem.MakeIndex`（:3555）→ 必须按引用回写，
    /// 故用 `ref` 委托而非 `Func`。</para>
    /// 接缝：待 UsrEngn.pas 提供该查询后接入。
    /// </summary>
    public delegate bool CopyToUserItemFromNameDelegate(string sItemName, ref TUserItem item);

    /// <summary>原文 `UserEngine.CopyToUserItemFromName`（ObjNpc.pas:3553）。接缝（默认失败）。</summary>
    public static CopyToUserItemFromNameDelegate CopyToUserItemFromName { get; set; } =
        (string _, ref TUserItem _) => false;

    /// <summary>
    /// 原文 `TUserCastle(m_Castle).m_boUnderWar`（**Castle.pas**"城堡处于攻城中"标志；
    /// ObjNpc.pas:2533 的 `UserSelect` 前置门）。
    /// <para><b>★ 为什么是接缝而不是字段（调度方裁定 B①；报告 §21 登记为偏差 D37）</b>：
    /// `TUserCastle` 确已移植（`Engine/Castle.cs`），但 `m_boUnderWar` 的**赋值点在未移植的
    /// 城堡战逻辑里**（只有 `ArcherGuardCore.cs:24`、`CanWalkCore.cs:80` 的注释提到）。
    /// 若在真实类型上加一个**没人赋值的字段**，它会**恒为 false** —— 那是"伪装成正式归属的
    /// 静默中性值"，比接缝更糟（接缝可检索/可登记/可删除；恒假字段会被后人当成已完成的状态）。</para>
    /// <para><b>默认抛异常</b>（台账 §25.2）：未接线时**立即暴露**，不静默返回 false。</para>
    /// <para><b>★ 删除条件（可执行）</b>：当 `TUserCastle.m_boUnderWar` 字段落地**且其赋值点接通**时，
    /// 删除本接缝，改为直读。判据：`grep -n 'm_boUnderWar' src/GXX.M2Server/Engine/Castle.cs`
    /// 中出现**赋值**（`=` 左侧）而非仅声明。</para>
    /// <para>⚠ 触发面是**窄路径**：仅 `m_boCastle = true` 的城堡 NPC 调用 `UserSelect` 才走到；
    /// 绝大多数 NPC 的 `m_boCastle` 为假，不会触达本接缝。</para>
    /// </summary>
    public static Func<object, bool> GetCastleUnderWar { get; set; } =
        _ => throw new NotSupportedException(
            "NpcSeams.GetCastleUnderWar 未接线：原文 TUserCastle.m_boUnderWar 未移植（报告 §21 / 偏差 D37）");

    // ★ 第十二轮：`NpcProcessCommandIndexOf` 接缝**已删除** —— 派发基础设施
    //   （`g_NpcProcessCommand` 表 + `nNF_*`/`sNF_*` 常量）已按原文 1:1 落地在
    //   `Npc/NpcProcessCommand.cs`（原文同属 NpcCommon.pas），故直接调用
    //   `NpcProcessCmd.g_NpcProcessCommand.GetCommand(sLabel)` / `.IndexOf(sLabel)`，
    //   不再经委托。这正是"正式归属落地后去掉替身"。

    // -----------------------------------------------------------------------
    // TMerchant.UpgradeWapon 外层体（ObjNpc.pas:1830-1901）需要的宿主面。
    // -----------------------------------------------------------------------

    // ★ 第十一轮：`SetUseItemsWeapon` 写回接缝**已删除**（见上方 `m_UseItems` 口径统一的说明）。
    //   原先它是"`m_UseItems` 元素类型选错"的权宜替身；口径统一后改为直写
    //   `User.m_UseItems[UseSlots.U_WEAPON] = item;`（ObjNpc.pas:1886 / D35 契约）。

    /// <summary>
    /// 原文 `g_sCannotUpgradeWeapon`（M2Share.pas:8165，默认
    /// `'你的武器[%Item]不允许升级'`，由 `:21346/:21352` 的 `LoadString` 覆盖）。
    /// ObjNpc.pas:1856 用它拼提示串（`AnsiReplaceStr(..., '%Item', StdItem.Name)`）。
    /// 接缝：待 M2Share 的 StringConf 接入；默认值**即原文默认值**（不是语义占位）。
    /// </summary>
    public static string g_sCannotUpgradeWeapon { get; set; } = "你的武器[%Item]不允许升级";

    /// <summary>
    /// 原文 `g_boGameLogGold`（M2Share.pas:3748 声明）—— 金币变动是否写日志。
    /// ObjNpc.pas:1862 用它决定 `AddGameDataLog(LOG_ItemUpgrade, LOG_GoldChange, ...)` 是否落库。
    /// 接缝：待 M2Share 移植后接入。默认 `false`（= 原文默认关闭）。
    /// </summary>
    public static bool g_boGameLogGold { get; set; }

    /// <summary>
    /// 原文 `TPlayObject.SysMsg(sMsg: AnsiString; FColor, BColor: Integer; MsgType: TMsgType;
    /// boAddPrefix: Boolean = True)` —— **第二个重载**（ObjPlayer.pas:1286；
    /// 第一个重载见 <see cref="SysMsg"/>(sMsg, MsgColor: TMsgColor, MsgType)）。
    /// <para>ObjNpc.pas:1857 与 1875 处用的是本重载（传 `g_Config.btRedMsgFColor`/`btRedMsgBColor` 两个**字节色值**）。
    /// 参数按原文顺序：(target, sMsg, FColor, BColor, MsgType)；`boAddPrefix` 取默认 True（接缝隐含）。</para>
    /// </summary>
    public static Action<TCreature, string, int, int, TMsgType> SysMsgFB { get; set; } =
        (_, _, _, _, _) => { };

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
    /// （`TBaseObject.SendMsg`，`ObjBase.pas:30339`；ObjNpc.pas:1719/1794/1825/10226/10228/10254/10402 等处的
    /// **网络下发**版）。
    /// <para><b>★ 命名裁定（调度方 2026 第三轮）</b>：本方法与托管侧既有
    /// `Engine.TCreature.SendMsg(ushort, long, long, long, long, string)`（`Engine/ObjBase.cs:48`，
    /// **入本地消息队列**版）**同名不同义**。为避免 `override`/`new` 掩盖语义，裁定**新成员用新名**，
    /// 故此处不叫 `SendMsg`/`SendMsgToClient`，而统一叫 <b>`SendTo`</b> ——
    /// 阅读顺序与原文一致：`User.SendTo(Self, wIdent, wParam, nParam1, nParam2, nParam3, sMsg)`。
    /// 托管侧以扩展方法 <see cref="ObjNpcSendToExtensions.SendTo"/> 暴露（不污染 Engine 类型）。</para>
    /// </summary>
    public static Action<TCreature, TCreature, ushort, long, long, long, long, string> SendTo { get; set; } =
        (_, _, _, _, _, _, _, _) => { };

    // -----------------------------------------------------------------------
    // TGuildOfficial / TCastleOfficial（公会官员 / 攻城官员）需要的最小宿主面。
    // -----------------------------------------------------------------------

    /// <summary>
    /// 原文 `TNormNpc.Click`（ObjNpc.pas:4431-4442，本车道未覆盖）—— 供
    /// `TMerchant.Click`(3228-3232)、`TGuildOfficial.Click`(10049-10053)、
    /// `TCastleOfficial.Click`(1115 的 `inherited`) 三处转发。默认无操作。
    /// </summary>
    public static Action<TNormNpc, TPlayObject> Click { get; set; } = (_, _) => { };

    /// <summary>
    /// 原文 `TPlayObject.SysMsg(sMsg, MsgColor, MsgType)`（ObjPlayer.pas；本单元多处调用）。
    /// 接缝把 `c_Red` / `c_Green` 与 `t_Hint` / `t_Castle` 一起透传（用既有枚举）。
    /// </summary>
    public static Action<TCreature, string, TMsgColor, TMsgType> SysMsg { get; set; } = (_, _, _, _) => { };

    /// <summary>原文 `TPlayObject.m_btPermission`（ObjPlayer.pas，权限等级；ObjNpc.pas:1114 判定 `&gt;= 3`）。接缝。</summary>
    public static Func<TPlayObject, byte> GetPlayerPermission { get; set; } = _ => 0;

    /// <summary>原文 `TPlayObject.m_boSendMsgFlag`（ObjPlayer.pas；ObjNpc.pas:9857/10399 读）。接缝。</summary>
    public static Func<TPlayObject, bool> GetSendMsgFlag { get; set; } = _ => false;

    /// <summary>原文 `TPlayObject.m_boSendMsgFlag := False`（ObjNpc.pas:9859/10401）。接缝。</summary>
    public static Action<TPlayObject> ClearSendMsgFlag { get; set; } = _ => { };

    /// <summary>
    /// 原文 `UserEngine.SendBroadCastMsg(sMsg, MsgType)`（UsrEngn.pas；
    /// ObjNpc.pas:9860 `t_Cust`、10402 `t_Castle`）。接缝：待 UsrEngn.pas 提供全服广播后接入。
    /// </summary>
    public static Action<string, TMsgType> SendBroadCastMsg { get; set; } = (_, _) => { };

    /// <summary>
    /// 原文 `g_CastleManager.GetCastleNameList(List: TStringList)`（Castle.pas；
    /// ObjNpc.pas:10071 用 `&lt;$REQUESTCASTLELIST&gt;` 生成攻城列表）。
    /// <para><b>★ 已接入真实现（2026 第三轮）</b>：默认实现直接转调
    /// `Engine.CastleState.g_CastleManager.GetCastleNameList`（`CastleState.cs:9` 单例 +
    /// `Castle.cs:424` 的 `partial class TCastleManager` 新方法）。
    /// 保留本属性只是为了让单测能注入假列表，**不再是空实现**。</para>
    /// </summary>
    public static Action<TStringList> GetCastleNameList { get; set; } =
        list => CastleState.g_CastleManager.GetCastleNameList(list);

    /// <summary>
    /// 原文 `GetUserItemBindValue(UserItem, ubNoSell)`（M2Share.pas:395 的绑定位判定）。
    /// <para><b>★ 去重</b>：转发到 DbLayer 车道已建的唯一实现
    /// `GXX.M2Server.DbLayer.DbLayerRunSeam.GetUserItemBindValue(bindOption, bit)`
    /// （`DbLayerSeams.cs:69`）—— 本属性只是 Npc 侧的别名，**单一后备存储**。</para>
    /// </summary>
    public static Func<int, int, bool> GetUserItemBindValue
    {
        get => GXX.M2Server.DbLayer.DbLayerRunSeam.GetUserItemBindValue;
        set => GXX.M2Server.DbLayer.DbLayerRunSeam.GetUserItemBindValue = value;
    }

    /// <summary>
    /// 原文 `g_ItemRules.Get(wIndex, 4)`（物品规则第 4 项 = 禁止出售）。
    /// <para><b>★ 去重</b>：转发到 `GXX.M2Server.DbLayer.DbLayerRunSeam.GetItemRule(wIndex, idx)`
    /// （`DbLayerSeams.cs:72`）。</para>
    /// </summary>
    public static Func<ushort, int, bool> GetItemRule
    {
        get => GXX.M2Server.DbLayer.DbLayerRunSeam.GetItemRule;
        set => GXX.M2Server.DbLayer.DbLayerRunSeam.GetItemRule = value;
    }

    /// <summary>
    /// 原文 `g_sCanotUserSellItem`（M2Share.pas:7928，默认 `'此物品禁止出售!'`，
    /// 由 `:21441-21443` 的 `StringConf` 覆盖）。ObjNpc.pas:3826 的提示串。
    /// 接缝：待 M2Share 的 StringConf 接入；默认值即原文默认值（**不是**语义占位）。
    /// </summary>
    public static string g_sCanotUserSellItem { get; set; } = "此物品禁止出售!";

    /// <summary>
    /// 原文 `TUserCastle(m_Castle).IncRateGold(nGold)`（Castle.pas；ObjNpc.pas:3843 的税收上账）。
    /// 接缝：`Engine.TUserCastle.IncRateGold(int)` 已存在，但本车道手上的 `m_Castle` 是 `object`
    /// （见 <see cref="GetNpcCastle"/>），故仍需一层委托。
    /// </summary>
    public static Action<object, int> IncRateGoldOnCastle { get; set; } = (_, _) => { };

    /// <summary>
    /// 原文 `g_CastleManager.IncRateGold(nGold)`（Castle.pas 的**管理器级**税收上账；
    /// ObjNpc.pas:3847/1874 调用）。注意与 <see cref="IncRateGoldOnCastle"/>（`TUserCastle` 实例级）
    /// **是两个不同的方法** —— 托管侧 `TUserCastle.IncRateGold(int)` 已存在（`Castle.cs:367`），
    /// 但 `TCastleManager` 上没有同名方法，故此处单独接缝。
    /// </summary>
    public static Action<int> IncRateGoldOnCastleManager { get; set; } = _ => { };

    /// <summary>
    /// 原文 `g_Config.boSubkMasterSendMsg`（M2Share.pas；ObjNpc.pas:10393 的"城主喊话"开关）。
    /// 接缝：待 M2Share 的 g_Config 接入（`M2Config` 暂无同名字段）。
    /// </summary>
    public static bool boSubkMasterSendMsg { get; set; }

    /// <summary>
    /// 原文 `g_sSubkMasterMsgCanNotUseNowMsg`（M2Share.pas；ObjNpc.pas:10395 的提示串）。
    /// 接缝：待 M2Share 移植后接入（此处为语义占位，**非原文文案**，已在报告登记）。
    /// </summary>
    public static string g_sSubkMasterMsgCanNotUseNowMsg { get; set; } = "当前无法使用城主喊话功能";

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
        // ★ 2026 第三轮：GetMyHero / GetCurrTarget / GetLastHiter / GetPoseCreate
        //   与 GetPlayerPVal / GetPlayerSString / GetPlayerTVal / GetPlayerArrayListValue
        //   八个委托已删除（改为直读 Engine 成员），故此处不再复位。
        IsCopyMon = _ => false;
        IsFunctionOrMissionNpc = _ => false;
        MainOutMessage = _ => { };
        GetValNameNo = CombatPowerUtils.GetValNameNo;
        GetVariableText = (_, _, sMsg, _, _) => (false, sMsg, false);
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
        boSendCustemMsg = false;
        g_sSendCustMsgCanNotUseNowMsg = "当前无法使用喊话功能";
        GetFilterTexts = () => null;
        GetItemAddValue = (ref TUserItem _, ref TStdItem _) => { };
        AddGameDataLog = (_, _, _, _, _, _, _, _, _) => { };
        sBlackStone = "黑铁矿";
        IsUseItem = nIndex =>
        {
            TStdItem? StdItem = GetStdItem(nIndex);
            return StdItem.Value.StdMode is 19 or 20 or 21 or 22 or 23 or 24 or 26;
        };
        SendTo = (_, _, _, _, _, _, _, _) => { };
        Click = (_, _) => { };
        SysMsg = (_, _, _, _) => { };
        GetPlayerPermission = _ => 0;
        GetSendMsgFlag = _ => false;
        ClearSendMsgFlag = _ => { };
        SendBroadCastMsg = (_, _) => { };
        GetCastleNameList = list => CastleState.g_CastleManager.GetCastleNameList(list);
        GetUserItemBindValue = (bindOption, bit) => (bindOption & (1 << bit)) != 0;
        GetItemRule = (_, _) => false;
        g_sCanotUserSellItem = "此物品禁止出售!";
        IncRateGoldOnCastle = (_, _) => { };
        IncRateGoldOnCastleManager = _ => { };
        OverLapItems = (_, _, _) => null;
        CopyToUserItemFromName = (string _, ref TUserItem _) => false;
        GetCastleUnderWar = _ => throw new NotSupportedException(
            "NpcSeams.GetCastleUnderWar 未接线：原文 TUserCastle.m_boUnderWar 未移植（报告 §21 / 偏差 D37）");
        g_sCannotUpgradeWeapon = "你的武器[%Item]不允许升级";
        g_boGameLogGold = false;
        SysMsgFB = (_, _, _, _, _) => { };
        boSubkMasterSendMsg = false;
        g_sSubkMasterMsgCanNotUseNowMsg = "当前无法使用城主喊话功能";
    }
}

/// <summary>
/// ★ 2026 第三轮命名裁定：原文 `TBaseObject.SendMsg(RecvObject, wIdent, wParam, nParam1, nParam2, nParam3, sMsg)`
/// （`ObjBase.pas:30339`，**网络下发**版）在托管侧**不能**沿用 `SendMsg` 这个名字 ——
/// `Engine.TCreature` 上已有一个同名但语义完全不同的 `SendMsg`（`Engine/ObjBase.cs:48`，把消息压进本地队列）。
/// 若用 `new`/`override` 覆盖，多态路径会静默走错版本。
/// <para>故以**扩展方法** `SendTo` 暴露：调用点写法与原文阅读顺序一致 ——
/// `User.SendTo(Self, wIdent, wParam, nParam1, nParam2, nParam3, sMsg)`，映射到原文 `User.SendMsg(Self, ...)`。
/// 扩展方法不污染 Engine 类型，也无需改 `Engine/**`。</para>
/// </summary>
public static class ObjNpcSendToExtensions
{
    /// <summary>
    /// 原文 `User.SendMsg(Self, wIdent, wParam, nParam1, nParam2, nParam3, sMsg)` 的托管入口。
    /// 实际下发由 <see cref="NpcSeams.SendTo"/> 承接（接入时转调 `PlayerSurfaceMsgSeams.SendDefMessage` 之类）。
    /// </summary>
    public static void SendTo(this TCreature target, TCreature sender, ushort wIdent, long wParam,
        long nParam1, long nParam2, long nParam3, string sMsg)
        => NpcSeams.SendTo(sender, target, wIdent, wParam, nParam1, nParam2, nParam3, sMsg);
}
