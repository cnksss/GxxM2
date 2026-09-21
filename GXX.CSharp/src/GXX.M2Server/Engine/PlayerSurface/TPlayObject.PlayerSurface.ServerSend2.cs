// ============================================================================
// 源单元：Source\M2Engine\ObjPlayer.pas（GBK；UTF-8 镜像 49,232 LF）
// 本文件：**ServerSend* 广播处理器片（切片 ServerSend2）** —— 本车道 p13-m2-objplayer。
//
// 覆盖原文行号范围（implementation 段）：
//   ObjPlayer.pas:38860-41078   TPlayObject 广播处理器族（含少量 Client* 处理器）
//     · 首条：ServerSendOpenHealth          （实现 38860；体 38860-38882）
//     · 末条：ClientHeroOpenJewelryBox      （实现 41068；体 41068-41078）
//     · 紧随其后的 ClientTakeOnJewelry（实现 41079，体 41079-41268）**不在本片**
//
// 方法条数：**157**
//   ★ 任务书称 156，且给的索引文件
//     `D:\chuanqi\daima\GXX原版_Delphi7\.p13scratch\_p13_joined.txt` **不存在**
//     （`.p13scratch` 整个目录缺失；已用 `dir /b /s` 与 glob 双重确认）。
//     故本片按 `^procedure TPlayObject\.` 从原文镜像**逐行重数**：
//     38860-41068 区间共 **157** 条声明（首条 38860、末条 41068），
//     且每条方法的**体结束行**已逐条算出（= 下一条声明行 - 1，末条 = 41078），
//     与本文件每个方法体首行注释 `// 原文 <start>-<end>` 逐条一致（157/157 对账通过）。
//     与任务书的 156 差 1 —— 无法与该索引对账，已如实登记。
//
// 三数对账（本片）：
//   真实体      150
//     · 其中 **2 条**是**原文空实现**（原文 `begin` / `end;` 之间没有任何语句）：
//         ServerSendQueryBagItems（38860-41077 区间内，体 39168-39171）
//         ServerSendWinExpNG     （体 39618-39621）
//       ★ 这两条的忠实移植就是**空体 + 注释**，**不是**留痕桩，
//         故计入"真实体"，并在方法体首行写明 `// 原文 NNNNN：空实现`。
//     · 其余 148 条为逐行移植的真实体；其中 25 条带"注入式接缝调用"
//       （报文身份 / 参数顺序 / 守卫 / 分支结构**仍逐行保留**，见各方法注释）。
//   NotPorted     7  （ServerSendVerifyCode / ServerSendWeather /
//                      ServerSendHeroM2BuyUserItem / ServerSendHeroM2AddUserItem /
//                      ClientTakeHorse / ClientInviteHorse / ClientResponseInviteHorse）
//   原文如此     35  （文件内 `★ 原文如此` 标记计数；"原文如此清单"见下，按条目数 6 类）
//   —— 校验：150 + 7 = 157 ✔
//
// 移植口径（与 PortKit、ServerSend1.cs 一致）：
//   · Self 比较：**复用** `PlayerSurfaceServerSendSeams.SameObject` 与实例字段
//     `SelfHandle`（均已在 ServerSend1.cs:100/252 声明）—— 本片**不重复声明**。
//   · 被读对象的字段（m_nLight / m_nCharStatus / m_WAbil / m_btDirection …）：
//     **复用** `PlayerSurfaceServerSendSeams` 的既有读取接缝（GetLight / GetCharStatus /
//     GetDirection / GetHP / GetMaxHP / GetMP / GetMaxMP / GetLevel / GetSecretFlag /
//     GetSecretFlag2 / GetFeature / GetCharColor / MyGetTickCount）；本片新增的读取
//     一律落在**本片自己的** `PlayerSurfaceServerSend2Seams`（下方），不另造第三份。
//   · `SendDefMessage(...)` → **复用既有** `PlayerSurfaceMsgSeams.SendDefMessage`
//     （TPlayObject.PlayerSurface.Gold.cs:52），不另造第二份。
//   · `SendSocket`/`SendSocketEx` → `SendSocketRef`/`SendSocketExRef`（PortKit）。
//     ★ 已知口径差异（如实登记，非本片引入）：Core1 的 `SendSocket`/`SendSocketEx`
//       （Core1.cs:1930/1978）自带 `m_boDummyObject` / `m_boOffLine` 两道早退守卫；
//       `SendSocketRef` 是 PortKit 的**直接落点**，**不经**这两道守卫。原文中
//       `SendSocket` 内部确有这两道守卫，但本片所有方法**在原文里就在方法体内
//       显式写了**它们需要的那两道判定（`ServerSendScreenEffect` /
//       `ServerSendStopScreenEffect` / `ServerSendClearScreenEffect` /
//       `ServerSendItemDescList` / `ServerSendItemDescTopList` /
//       `ServerSendTzItemDescList` / `ServerSendFilterItemList` /
//       `ServerSendPlayMagicBallEffect`），故这些方法的可观测行为与原文一致；
//       其余方法原文本身**没有**依赖该守卫（`SendDefMessage` 自带同款守卫）。
//       为与本车道其余切片（Core2/Core4/Core6）保持同一接缝，本片统一走 `*Ref`。
//   · `MakeDefaultMsg`/`MakeWord`/`MakeLong`/`LoWord`/`HiWord` → PortKit `PlayerSurfacePack`。
//   · `EncodeBuffer`/`EncodeString`/`zLibCompressBuffer` → `GXX.Core.Protocol.EDcode`。
//   · `IntToStr` → `DelphiRTL.IntToStr`；`StrToIntDef` → `DelphiRTL.StrToIntDef`。
//
// 原文如此清单（**逐条保留，不修**）：
//   ① 39655/39668：`if Length(g_ItemDescListText) > 0` 在**守卫之后**才判空；
//   ② 39773-39776 `ServerSendIncHealth`：`Int64(m_WAbil.HP) + LongWord(ProcessMsg.nParam1)`
//      —— `nParam1` 被**无符号**解释（负参数会变成巨值），随后 `Min(...)` 再夹回 MaxHP；
//   ③ 40973 / 41017：`abs(m_nCurrX - nX) > 3` 用的是**有符号**比较（原文如此）；
//   ④ 39802 一带：`SendDefMessage(SM_ATTACK0x, NativeInt(ProcessMsg.BaseObject), ...)`
//      把"被攻击者指针"当 `nRecog` 传出（四条 Attack0x 同款）；
//   ⑤ 40223 `IncGameMoney` 的 `else` 分支：`nTCustomMoneyCount := 0;` 之后**紧跟**
//      `SellUser.IncMoney(...)`，Delphi 的 `else` 只吃**第一条**语句 ⇒ 该 `IncMoney`
//      **无条件执行**（且此时 `nTCustomMoneyCount` 可能是调用方传入的**未初始化值**）；
//      ★ 原文缺陷，逐字保留（本片该方法整体未移植，缺陷随原文留在未移植清单里）；
//   ⑥ 40480：`if (nMoneyType < 0) { or (nMoneyType > 4) }` —— 注释掉的才是原意。
//
// ⚠ 本文件不重复声明任何已存在的字段/方法（逐条 grep 证据见交付报告）：
//   m_DefMsg / SendSocketRef / SendSocketExRef / PortNotPorted / PlayerSurfacePack
//     → PortKit.cs:147/160/166/174
//   SelfHandle / PlayerSurfaceServerSendSeams（GetLight/GetCharStatus/GetDirection/
//     GetHP/GetMaxHP/GetMP/GetMaxMP/GetLevel/GetSecretFlag/GetSecretFlag2/GetFeature/
//     GetCharColor/MyGetTickCount/SameObject）
//     → TPlayObject.PlayerSurface.ServerSend1.cs:100-252
//   m_boDummyObject / m_boOffLine → ObjBase.OnlineMsg.cs:13/10
//   m_nOffOnlineTick → Core1.cs:456
//   m_AbilNG → Core1.cs:386（⚠ Core3.cs:313 **重复声明了同一个字段**，属既有 CS0102，
//     非本片引入；本片只读不声明）
//   m_MyHero → Hero.cs:31；m_boMessageBox → NpcSession.cs:129
//   m_nScriptGotoCount → ScriptFields.cs:28
//   m_nCurrentItemMakeIndex / m_sCurrentItemName / m_nGateIdx → Gold.cs:102/105/109
//   m_PEnvir / m_ItemList / m_wAbil / m_nCurrX / m_nCurrY / m_btDirection / m_sCharName
//     → ObjBase.cs:24/62/65/17/18/19/22
//   m_nGold / m_nGameGold / m_nGamePoint / m_nGameDiamond / m_nGameGird / m_nGameGlory
//     → ObjBase.OnlineMsg.cs:44/36/37/39/40/46
//   SysMsg(string,int,int,TMsgType) → Core6.cs:626
//   IncGold / DecGold / GoldChanged / IncGameGold / DecGameGold / GameGoldChanged /
//     NewGamePointChanged → Gold.cs:132/163/183/198/213/226/236
//   IncGamePoint / IncGameGird / DecGameGird / IncGameDiamond / DecGameDiamond
//     → Core1.cs:1539/1554/1569/1581/1596
//   SendAddItem / IsEnoughBag / WeightChanged → TCreature.PlayerSurface.Items.cs:450/264/328
//   FeatureChanged → TCreature.PlayerSurface.Base.cs:476
//   SendRefMsg → TCreature.PlayerSurface.Base.cs:465
//   ClientQueryAssessHero → Core1.cs:2122
//   g_FunctionNPC → Sweep9\Monsters\ObjDummyCore.cs:194
//   g_ItemRules → Forms\ViewList2State.cs:30（`TItemRules`，Engine\Items.cs:67）
// ============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using GXX.Core.Protocol;
using GXX.Core.Rtl;

namespace GXX.M2Server.Engine;

/// <summary>
/// 原文 `THeroM2ShopItemInfo`（`ObjPlayer.pas` 内 `HeroM2Shop` 一族的堆记录，
/// 原文以 `pTHeroM2ShopItemInfo` 指针 + `New/Dispose` 管理）。
///
/// **为什么在托管侧是一个类**：原文 `m_HeroM2ShopList` 存的是**指针**，`Dispose` 之后
/// 指针仍可能被别的分支 `=` 比较（40400）；值类型无法表达"同一件"的别名语义，
/// 用 `class` 才能保住 `Dispose` 前的引用等价（与 `ObjBase.cs:48-61` 对 D35 的
/// 登记同一取向）。**本片只按原文逐字用到 4 个字段**。
/// </summary>
public sealed class THeroM2ShopItemInfo
{
    /// <summary>原文 `nMakeIndex: Integer`（40653）。</summary>
    public int nMakeIndex;
    /// <summary>原文 `nPrice: Integer`（40654）。</summary>
    public int nPrice;
    /// <summary>原文 `nMoneyType: Integer`（40655）。</summary>
    public int nMoneyType;
    /// <summary>原文 `dwAddTick: LongWord`（40656；`MyGetTickCount` 值）。</summary>
    public uint dwAddTick;
}

/// <summary>
/// `ObjPlayer.pas:38860-41077` 这一片在托管侧的**依赖接缝**。
///
/// 与 `PlayerSurfaceServerSendSeams`（ServerSend1.cs）**同型但不同片**：凡 ServerSend1
/// 已经声明过的读取（GetLight / GetCharStatus / GetDirection / GetFeature / GetCharColor
/// / GetHP 族 / MyGetTickCount / SameObject …）本片**直接复用**，此处只收本片独有的、
/// 原文确实读到、而托管侧尚无等价物的那一批。每个委托的文档注释都标了原文调用行号，
/// 并给出"无宿主"时的默认值语义。
/// </summary>
public static class PlayerSurfaceServerSend2Seams
{
    // ------------------------------------------------------------------
    // 一、`TPlayObject.ServerSendChangeGuildName()` 一族（本片把它做成方法接缝）
    // ------------------------------------------------------------------

    /// <summary>原文 `TPlayObject.SendChangeGuildName()`（`ObjPlayer.pas` 另有定义；
    /// 本片调用点：38934）。**不臆造其实现**，做成可注入方法。默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject> SendChangeGuildName { get; set; } = _ => { };

    /// <summary>原文 `TPlayObject.SendAdjustBonus()`（本片调用点：39087）。
    /// 默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject> SendAdjustBonus { get; set; } = _ => { };

    /// <summary>原文 `TPlayObject.GetMyStatus(): Integer`（本片调用点：39122
    /// `SendDefMessage(SM_MYSTATUS, 0, GetMyStatus, 0, 0, '')`）。
    /// 默认：无宿主，返回 0。</summary>
    public static Func<TPlayObject, int> GetMyStatus { get; set; } = _ => 0;

    /// <summary>原文 `TPlayObject.SendDelItemList(Items: PAnsiChar; ItemsCount: Integer)`
    /// （本片调用点：38958 `SendDelItemList(ProcessMsg.sMsg, ProcessMsg.nParam1)`）。
    /// 默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject, string, int> SendDelItemList { get; set; } = (_, _, _) => { };

    /// <summary>原文 `TPlayObject.SendSaveItemList(nWhere, nPage: Integer)`
    /// （本片调用点：38988 `SendSaveItemList(ProcessMsg.nParam1, ProcessMsg.nParam2)`）。
    /// 默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject, int, int> SendSaveItemList { get; set; } = (_, _, _) => { };

    /// <summary>原文 `TPlayObject.SendSaveBigStorageItemList(nWhere, nPage: Integer)`
    /// （本片调用点：39837）。默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject, int, long> SendSaveBigStorageItemList { get; set; } = (_, _, _) => { };

    /// <summary>原文 `TPlayObject.SendPreviewMonItem(BaseObject: TBaseObject)`
    /// （本片调用点：39506）。参数为原文 `TBaseObject(ProcessMsg.BaseObject)` 的裸句柄。
    /// 默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject, nint> SendPreviewMonItem { get; set; } = (_, _) => { };

    /// <summary>原文 `TPlayObject.SendUpdateItem(UserItem: pTUserItem)`
    /// （本片调用点：40057 / 40698 —— 两处都是"刷新一下，原来的 Price 和 ResRved1
    /// 字段改变了"）。托管侧**没有**实例方法 `SendUpdateItem`（`PlayerSurfaceCore1Seams.SendUpdateItem`
    /// 是接缝，`Core1.cs:1474` 用的就是它），故本片统一走这里。
    /// 默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject, TUserItem> SendUpdateItem { get; set; } = (_, _) => { };

    /// <summary>原文 `m_PEnvir` 之外，`SendUseIcons`/`SendUseEffects`
    /// （Core4.cs:890/902，签名收 `TCreature`）需要从 `nint` 反解出的 `TCreature`。
    /// 本片调用点：39301 / 39311。默认：null（**调用方必须判空**）。</summary>
    public static Func<nint, TCreature?> GetCreatureByHandle { get; set; } = _ => null;

    /// <summary>原文 `TPlayObject.HealthSpellChanged()`（`ObjPlayer.pas` 另有定义；
    /// 本片调用点：39777 `ServerSendIncHealth` 末尾）。默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject> HealthSpellChanged { get; set; } = _ => { };

    /// <summary>原文 `TPlayObject.GetNGAddPower: Integer`（本片调用点：39600
    /// `ClientAbilityNG.NGDamage := GetNGAddPower`）。默认：0。</summary>
    public static Func<TPlayObject, int> GetNGAddPower { get; set; } = _ => 0;

    /// <summary>原文 `TPlayObject.GetNGDecPower: Integer`（本片调用点：39601
    /// `ClientAbilityNG.UnNGDamage := GetNGDecPower`）。默认：0。</summary>
    public static Func<TPlayObject, int> GetNGDecPower { get; set; } = _ => 0;

    /// <summary>原文 `TPlayObject.CanUseCallMonMagic(wMagicId: Word): Boolean`
    /// （本片调用点：39875 `Ret := Integer(CanUseCallMonMagic(ProcessMsg.wParam))`）。
    /// 默认：无宿主，返回 false（⇒ `Ret = 0`）。</summary>
    public static Func<TPlayObject, int, bool> CanUseCallMonMagic { get; set; } = (_, _) => false;

    // ------------------------------------------------------------------
    // 二、UserEngine 侧（本片调用点）
    // ------------------------------------------------------------------

    /// <summary>原文 `TUserEngine.GetPlayObject(PlayObject: TObject): TPlayObject`
    /// （`UsrEngn.pas:6509-6527`，**按对象指针查**，与既有的
    /// `PlayerSurfaceCore1Seams.GetPlayObject(string)` **不同重载**）。
    /// 本片调用点：40242（`UserEngine.GetPlayObject(TPlayObject(ProcessMsg.nParam1))`）。
    /// 默认：无宿主，返回 null。</summary>
    public static Func<nint, TPlayObject?> GetPlayObjectByHandle { get; set; } = _ => null;

    /// <summary>原文 `TUserEngine.FindMerchant(AObject: TObject): TPlayObject`
    /// （`UsrEngn.pas`；本片调用点：39225 `ServerSendQueryDealFail`）。
    /// 返回 null 等价原文 `FindMerchant(...) = nil` ⇒ 整条不执行。
    /// 默认：无宿主，返回 null。</summary>
    public static Func<nint, TPlayObject?> FindMerchant { get; set; } = _ => null;

    // ------------------------------------------------------------------
    // 三、跨对象 / 跨单元的字段与方法
    // ------------------------------------------------------------------

    /// <summary>原文 `TPlayObject.SysMsg(sMsg: AnsiString; FColor, BColor: Integer; MsgType: TMsgType)`
    /// **打在别的玩家身上** —— 本片调用点：40371
    /// （`OnlineObject.SysMsg(Format(g_sHeroM2SellerMsg, [...]), 255, 252, t_Hint)`）。
    /// 托管 `TPlayObject` 已有同类实例方法（Core6.cs:626），此处接缝只用于
    /// "对**另一个** TPlayObject 调用"的显式表达。默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject, string, int, int, TMsgType> SysMsgTo { get; set; }
        = (_, _, _, _, _) => { };

    /// <summary>原文 `TBaseObject.GetFeatureToLong(Feature: PAnsiChar): Integer`
    /// （本片调用点：39201）。返回 `(长度, 特征字节)` —— 原文把特征写进调用方栈上的
    /// `Feature: array[0..255] of AnsiChar`，托管侧改为直接返回字节块
    /// （第三参 `256` 是原文那个数组的长度上限）。默认：无宿主，返回 `(0, 空)`。</summary>
    public static Func<nint, TPlayObject, int, (int len, byte[] feature)> GetFeatureToLong { get; set; }
        = (_, _, _) => (0, Array.Empty<byte>());

    /// <summary>原文 `TBaseObject(ProcessMsg.BaseObject).m_Master`（本片调用点：39204
    /// `MessageBodyWL.lTag2 := NativeInt(TBaseObject(...).m_Master)`）。默认：0。</summary>
    public static Func<nint, nint> GetMaster { get; set; } = _ => 0;

    /// <summary>原文全局 `g_SendSMSConfig.VerifySendInterval`（`M2Share.pas`；
    /// 本片调用点：39365 `ServerSendInputMObileVerifyCode`）。
    /// 默认：0（无宿主）。</summary>
    public static Func<int> GetSmsVerifySendInterval { get; set; } = () => 0;

    /// <summary>原文 `m_PEnvir.IsValidObjectEx(nX, nY, nRange: Integer; BaseObject): Boolean`
    /// （`Envir.pas`；本片调用点：39898 `ClientQuerySelectHeroM2ShopInfo`）。
    /// 参数：环境 / X / Y / 范围 / 被查对象句柄。默认：**返回 false**
    /// —— 原文 `if not IsValidObjectEx(...) then BaseObject := nil`，
    /// 即"未接线 ⇒ 视为无效对象"（保守且可测）。</summary>
    public static Func<TEnvirnoment, int, int, int, nint, bool> IsValidObjectEx { get; set; }
        = (_, _, _, _, _) => false;

    /// <summary>原文 `TBaseObject.CretInNearXY(BaseObject; nX, nY: Integer): Boolean`
    /// （`ObjBase.pas`；本片调用点：40971 `ClientInviteHorse`）。默认：false。</summary>
    public static Func<nint, int, int, bool> CretInNearXY { get; set; } = (_, _, _) => false;

    /// <summary>原文 `TPlayObject.GetCustomMoneyNameByRule(nIndex, nType: Integer): string`
    /// （本片调用点：40302 / 40353）。默认：空串。</summary>
    public static Func<TPlayObject, int, int, string> GetCustomMoneyNameByRule { get; set; }
        = (_, _, _) => "";

    /// <summary>原文 `TPlayObject.GetLowestSellingPrice(sItemName: string): PLowestSellingPrice`
    /// （本片调用点：40536）。返回 null 等价原文 `= nil` ⇒ 跳过该段校验。默认：null。</summary>
    public static Func<string, int[]>? GetLowestSellingPrice { get; set; } = null;

    /// <summary>原文 `TPlayObject.GetHighestSellingPrice(sItemName: string): PHighestSellingPrice`
    /// （本片调用点：40595）。返回 null 等价原文 `= nil`。默认：null。</summary>
    public static Func<string, int[]>? GetHighestSellingPrice { get; set; } = null;

    /// <summary>原文 `GetUserItemBindValue(UserItem: TUserItem; bit): Boolean`
    /// （本片调用点：40469，`ubNoSell` 位）。★ 与
    /// `DbLayerSeams.GetUserItemBindValue(int,int)`（`DbLayerSeams.cs:69`，
    /// 收的是 `btBindOption` 整数）**不是同一个入口**，故另立。
    /// 默认：按 `btBindOption` 位测试（与 DbLayerSeams 同一语义）。</summary>
    public static Func<TUserItem, int, bool> GetUserItemBindValueItem { get; set; }
        = (item, bit) => (item.btBindOption & (1 << bit)) != 0;

    /// <summary>原文全局 `g_sCannotSell`（`M2Share.pas`；本片调用点：40473）。
    /// 托管侧未收录该全局，故接缝化。默认：原文口径的空串（由宿主注入）。</summary>
    public static Func<string> GetCannotSellMsg { get; set; } = () => "";

    /// <summary>原文全局 `sSTRING_GOLDNAME`（`M2Share.pas:210`，值 `'金币'`；
    /// 本片调用点：40296 / 40347 / 40510 / 40565 / 40624）。</summary>
    public static string STRING_GOLDNAME { get; set; } = "金币";

    /// <summary>原文 `g_Config.sGameGoldName`（本片调用点：40292 / 40343 / 40492 / 40545 / 40604）。
    /// 默认：空串（无宿主）。</summary>
    public static Func<string> GetGameGoldName { get; set; } = () => "";
    /// <summary>原文 `g_Config.sGamePointName`（本片调用点：40294 / 40345 / 40501 / 40555 / 40614）。</summary>
    public static Func<string> GetGamePointName { get; set; } = () => "";
    /// <summary>原文 `g_Config.sGameDiamondName`（本片调用点：40298 / 40349 / 40519 / 40575 / 40634）。</summary>
    public static Func<string> GetGameDiamondName { get; set; } = () => "";
    /// <summary>原文 `g_Config.sGameGirdName`（本片调用点：40300 / 40351 / 40528 / 40585 / 40644）。</summary>
    public static Func<string> GetGameGirdName { get; set; } = () => "";

    /// <summary>原文 `g_Config.btRedMsgFColor` / `btRedMsgBColor`（本片调用点：40473）。</summary>
    public static Func<int> GetRedMsgFColor { get; set; } = () => 255;
    /// <summary>原文 `g_Config.btRedMsgBColor`。</summary>
    public static Func<int> GetRedMsgBColor { get; set; } = () => 253;

    /// <summary>原文 `g_sHeroM2BuyerMsg`（`M2Share.pas`；本片调用点：40368，
    /// `Format(g_sHeroM2BuyerMsg, [OnlineObject.m_sCharName, StdItemName, nPrice, MoneyName])`
    /// —— **4 个 `%s`/`%d` 占位**）。默认：空串（`Format` 原样返回）。</summary>
    public static string HeroM2BuyerMsg { get; set; } = "";
    /// <summary>原文 `g_sHeroM2SellerMsg`（本片调用点：40371，**6 个占位**）。</summary>
    public static string HeroM2SellerMsg { get; set; } = "";

    /// <summary>原文 `g_sHorseStopShop`（本片调用点：40813 `ClientTakeHorse`）。</summary>
    public static string HorseStopShop { get; set; } = "";
    /// <summary>原文 `g_sHorseBrand`（本片调用点：40826）。</summary>
    public static string HorseBrand { get; set; } = "";
    /// <summary>原文 `g_sHorseMapDisable`（本片调用点：40831）。</summary>
    public static string HorseMapDisable { get; set; } = "";
    /// <summary>原文 `g_sHorseTime`（本片调用点：40843，**1 个占位**）。</summary>
    public static string HorseTime { get; set; } = "";
    /// <summary>原文 `g_sHorseNotInvitePlayer`（本片调用点：40930 / 40943）。</summary>
    public static string HorseNotInvitePlayer { get; set; } = "";
    /// <summary>原文 `g_sHorseInviteNoPlayer`（本片调用点：40952 / 40989）。</summary>
    public static string HorseInviteNoPlayer { get; set; } = "";
    /// <summary>原文 `g_sHorseInviteMustPlayer`（本片调用点：40957）。</summary>
    public static string HorseInviteMustPlayer { get; set; } = "";
    /// <summary>原文 `g_sPoseDisableHorseInviteMsg`（本片调用点：40962）。</summary>
    public static string PoseDisableHorseInviteMsg { get; set; } = "";
    /// <summary>原文 `g_sHorseInviteFar`（本片调用点：40975）。</summary>
    public static string HorseInviteFar { get; set; } = "";
    /// <summary>原文 `g_sHorseInviteOnHorse`（本片调用点：40980）。</summary>
    public static string HorseInviteOnHorse { get; set; } = "";
    /// <summary>原文 `g_sResponseHorseNo`（本片调用点：41001，**1 个占位**）。</summary>
    public static string ResponseHorseNo { get; set; } = "";
    /// <summary>原文 `g_sResponseHorseTimeOut`（本片调用点：41008，**1 个占位**）。</summary>
    public static string ResponseHorseTimeOut { get; set; } = "";
    /// <summary>原文 `g_sResponseHorseOtherPlayer`（本片调用点：41014，**1 个占位**）。</summary>
    public static string ResponseHorseOtherPlayer { get; set; } = "";
    /// <summary>原文 `g_sResponseHorseFar`（本片调用点：41020，**1 个占位**）。</summary>
    public static string ResponseHorseFar { get; set; } = "";

    /// <summary>原文 `g_Config.dwHorseTakeTime`（秒；本片调用点：40836 / 40838）。
    /// 默认：0。</summary>
    public static Func<uint> GetHorseTakeTime { get; set; } = () => 0;
    /// <summary>原文 `g_Config.dwTakeOnHorseUseTime`（秒；本片调用点：40846-40854）。
    /// 默认：0。</summary>
    public static Func<uint> GetTakeOnHorseUseTime { get; set; } = () => 0;
    /// <summary>原文 `g_Config.boLockStall`（本片调用点：39957）。默认：false。</summary>
    public static Func<bool> GetLockStall { get; set; } = () => false;
    /// <summary>原文 `g_Config.boOpenSelfShop`（本片调用点：39968）。默认：false。</summary>
    public static Func<bool> GetOpenSelfShop { get; set; } = () => false;
    /// <summary>原文 `g_Config.boSafeZoneShop`（本片调用点：39975）。默认：false。</summary>
    public static Func<bool> GetSafeZoneShop { get; set; } = () => false;
    /// <summary>原文 `g_Config.boMapShop`（本片调用点：39982）。默认：false。</summary>
    public static Func<bool> GetMapShop { get; set; } = () => false;

    /// <summary>原文 `m_PEnvir.m_boAllowUseMyshop`（本片调用点：39982）。默认：false。</summary>
    public static Func<TEnvirnoment, bool> GetEnvirAllowUseMyshop { get; set; } = _ => false;
    /// <summary>原文 `m_PEnvir.m_boNoAutoOnline`（本片调用点：39860）。默认：false。</summary>
    public static Func<TEnvirnoment, bool> GetEnvirNoAutoOnline { get; set; } = _ => false;
    /// <summary>原文 `m_PEnvir.m_boNOHORSE`（本片调用点：40829）。默认：false。</summary>
    public static Func<TEnvirnoment, bool> GetEnvirNoHorse { get; set; } = _ => false;

    /// <summary>原文 `TBaseObject.InSafeZone: Boolean`（属性；本片调用点：39975）。
    /// 默认：false。</summary>
    public static Func<TPlayObject, bool> GetInSafeZone { get; set; } = _ => false;

    /// <summary>原文 `TPlayObject.StopCollect()`（本片调用点：40016 / 40834 / 40889）。
    /// 默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject> StopCollect { get; set; } = _ => { };

    /// <summary>原文 `TPlayObject.TriggerHorseScript()`（本片调用点：40868 / 40885 / 40895）。
    /// 默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject> TriggerHorseScript { get; set; } = _ => { };

    /// <summary>原文 `TPlayObject.SendTakeOffHorse()`（本片调用点：40887 / 40897）。
    /// 默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject> SendTakeOffHorse { get; set; } = _ => { };

    /// <summary>原文 `TPlayObject.DoTalkStatusChanged()`（本片调用点：40884 / 40894）。
    /// 默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject> DoTalkStatusChanged { get; set; } = _ => { };

    /// <summary>原文 `TPlayObject.RefShowName()`（本片调用点：40867 / 41038 / 41040）。
    /// 默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject> RefShowName { get; set; } = _ => { };

    /// <summary>原文 `TBaseObject.GetStdItem(wIndex: Integer): pTStdItem` —— 与
    /// `PlayerSurfaceItemSeams.GetStdItem` 同一语义，但本片统一从**本接缝**取，
    /// 避免与 Core1/Core4 的接线互相干扰。默认：null。</summary>
    public static Func<int, TStdItem?> GetStdItem { get; set; } = _ => null;

    /// <summary>原文 `TBaseObject.SendMsg(BaseObject; wIdent, wParam; nParam1..3: NativeInt; sMsg)`
    /// —— **7 参**（与 `PlayerSurfaceCore1Seams.SendMsg` 同型）。本片调用点：40985。
    /// 默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject, int, long, long, long, long, string> SendMsg { get; set; }
        = (_, _, _, _, _, _, _) => { };

    /// <summary>原文 `m_PEnvir.MoveToMovingObject(...)`（本片调用点：41041）。
    /// 参数：环境 / 源 X / 源 Y / 目标 X / 目标 Y / Flag。默认：false。</summary>
    public static Func<TEnvirnoment, int, int, int, int, bool, bool> MoveToMovingObject { get; set; }
        = (_, _, _, _, _, _) => false;

    /// <summary>原文 `m_PEnvir.GetMovingObject(nX, nY, boFlag): TBaseObject`
    /// （本片调用点：40949 —— **返回裸句柄**，因为原文随后做 `TBaseObject` 强转）。
    /// 默认：0（= 原文 `nil`）。</summary>
    public static Func<TEnvirnoment, int, int, bool, nint> GetMovingObjectHandle { get; set; }
        = (_, _, _, _) => 0;

    /// <summary>原文 `TPlayObject.m_boDisableHorseInvite`（本片调用点：40960）。
    /// 参数为对方对象句柄（托管侧无法从 `nint` 反解实例）。默认：false。</summary>
    public static Func<nint, bool> GetDisableHorseInvite { get; set; } = _ => false;

    /// <summary>原文 `TPlayObject.m_nChangeAppr`（本片调用点：40966，对方对象）。
    /// 默认：-1（原文用 `>= 0` 判"变身中"，-1 = 未变身）。</summary>
    public static Func<nint, int> GetChangeAppr { get; set; } = _ => -1;

    /// <summary>原文 `TPlayObject.m_boOnHorse`（本片调用点：40978，对方对象）。默认：false。</summary>
    public static Func<nint, bool> GetOnHorseOf { get; set; } = _ => false;

    /// <summary>原文 `TPlayObject.m_btHorseType`（本片调用点：41034，邀请人对象）。默认：0。</summary>
    public static Func<nint, int> GetHorseTypeOf { get; set; } = _ => 0;

    /// <summary>原文 `TPlayObject.m_nCurrX / m_nCurrY / m_btDirection`
    /// （本片调用点：41044-41046，邀请人对象）。默认：0。</summary>
    public static Func<nint, int> GetCurrXOf { get; set; } = _ => 0;
    /// <summary>原文 `m_nCurrY`（41045）。</summary>
    public static Func<nint, int> GetCurrYOf { get; set; } = _ => 0;
    /// <summary>原文 `m_btDirection`（41046）。</summary>
    public static Func<nint, int> GetDirectionOf { get; set; } = _ => 0;

    /// <summary>原文 `OnlineObject.m_boShopStall`（本片调用点：39905 / 40243，**别的**玩家）。
    /// 默认：false。</summary>
    public static Func<TPlayObject, bool> GetShopStallOf { get; set; } = _ => false;

    /// <summary>原文 `TPlayObject.m_boLockLogon`（本片调用点：39957，自身）。默认：false。</summary>
    public static Func<TPlayObject, bool> GetLockLogon { get; set; } = _ => false;
    /// <summary>原文 `TPlayObject.m_boPasswordLocked`（本片调用点：39957，自身）。默认：false。</summary>
    public static Func<TPlayObject, bool> GetPasswordLocked { get; set; } = _ => false;
    /// <summary>原文 `TPlayObject.m_dwChangeModeExTick[9]`（本片调用点：39962，自身）。
    /// 默认：0。</summary>
    public static Func<TPlayObject, uint> GetChangeModeExTick9 { get; set; } = _ => 0;

    /// <summary>原文 `onlineObject.m_HeroM2ShopList`（`TList`，元素 `pTHeroM2ShopItemInfo`）。
    /// 托管侧 `THeroM2ShopItemInfo` 是 `class` ⇒ 列表用 `List&lt;THeroM2ShopItemInfo&gt;`。
    /// 调用点：39917 / 39922-39926 / 40014 / 40049-40051 / 40253-40257 /
    /// 40298-40402 / 40428-40448 / 40557 / 40684-40689 / 40726。默认：空表。</summary>
    public static Func<TPlayObject, List<THeroM2ShopItemInfo>> GetHeroM2ShopList { get; set; }
        = _ => new List<THeroM2ShopItemInfo>();

    /// <summary>原文 `onlineObject.m_HeroM2ShopOpenList`（`TList`，元素 `TPlayObject`）。
    /// 调用点：39922-39923 / 40726。默认：空表。</summary>
    public static Func<TPlayObject, List<TPlayObject>> GetHeroM2ShopOpenList { get; set; }
        = _ => new List<TPlayObject>();

    /// <summary>原文 `TPlayObject.m_sCurOpenUserHeroM2Shop: string`（调用点：39910 /
    /// 39912 / 39914 / 39920 / 40721 / 40723 / 40729）。默认：""。</summary>
    public static Func<TPlayObject, string> GetCurOpenUserHeroM2Shop { get; set; } = _ => "";
    /// <summary>原文 `TPlayObject.m_sCurOpenUserHeroM2Shop := value`（39920 / 40729）。</summary>
    public static Action<TPlayObject, string> SetCurOpenUserHeroM2Shop { get; set; } = (_, _) => { };

    /// <summary>原文 `TPlayObject.m_sShopName`（调用点：40029 `SendRefMsg(RM_SENDSHOPNAME, 0,
    /// Self, 0, 0, m_sShopName)`）。默认：""。</summary>
    public static Func<TPlayObject, string> GetShopName { get; set; } = _ => "";

    /// <summary>原文 `TPlayObject.m_dwHeroM2ShopStallTime: LongWord`
    /// （调用点：40007-40012 / 40043-40048）。默认：0。</summary>
    public static Func<TPlayObject, uint> GetHeroM2ShopStallTime { get; set; } = _ => 0;
    /// <summary>原文 `m_dwHeroM2ShopStallTime := MyGetTickCount`（40012 / 40048）。</summary>
    public static Action<TPlayObject, uint> SetHeroM2ShopStallTime { get; set; } = (_, _) => { };

    /// <summary>原文 `TPlayObject.m_dwHeroM2AddItemTime: LongWord`（调用点：40434-40439）。
    /// 默认：0。</summary>
    public static Func<TPlayObject, uint> GetHeroM2AddItemTime { get; set; } = _ => 0;
    /// <summary>原文 `m_dwHeroM2AddItemTime := MyGetTickCount`（40439）。</summary>
    public static Action<TPlayObject, uint> SetHeroM2AddItemTime { get; set; } = (_, _) => { };

    /// <summary>原文 `TPlayObject.m_dwQueryUserShopTime: LongWord`（调用点：39892-39894）。
    /// 默认：0。</summary>
    public static Func<TPlayObject, uint> GetQueryUserShopTime { get; set; } = _ => 0;
    /// <summary>原文 `m_dwQueryUserShopTime := MyGetTickCount`（39894）。</summary>
    public static Action<TPlayObject, uint> SetQueryUserShopTime { get; set; } = (_, _) => { };

    /// <summary>原文 `TPlayObject.m_nGameDiamond / m_nGameGird`（本片调用点：40081 / 40083
    /// 的 `CheckGameMoney`；⚠ 托管侧这两个字段**已存在**于 `ObjBase.OnlineMsg.cs:39/40`，
    /// 此处接缝只是为了让"跨对象读取"同样可注入 —— 默认回落到本对象的真实字段。</summary>
    public static Func<TPlayObject, int> GetGameDiamond { get; set; } = p => p.m_nGameDiamond;
    /// <summary>原文 `TPlayObject.m_nGameGird`（40083）。</summary>
    public static Func<TPlayObject, int> GetGameGird { get; set; } = p => p.m_nGameGird;

    /// <summary>原文 `g_OnlineMsgControl.boDisableBuy`（本片调用点：40238）。默认：false。</summary>
    public static Func<bool> GetDisableBuy { get; set; } = () => false;
    /// <summary>原文 `g_OnlineMsgControl.boDisableSell`（本片调用点：39954 / 40240）。默认：false。</summary>
    public static Func<bool> GetDisableSell { get; set; } = () => false;

    // ------------------------------------------------------------------
    // 四、`g_FunctionNPC` / `UserEngine` / `TBaseObject` 侧（本片新增调用点）
    // ------------------------------------------------------------------

    /// <summary>原文 `g_FunctionNPC &lt;&gt; nil` 的判定（本片调用点：39853/39862/40024/40066/
    /// 40735/40744/40753/40762/40771/40780/41049/41061/41072）。
    /// 托管侧 `g_FunctionNPC` 见 `Sweep9\Monsters\ObjDummyCore.cs:194`
    /// （`IFunctionNpcSeam?`，是**另一条车道**的类型），本片不跨车道引用，
    /// 改为"是否已接线 + 标签可否投递"的显式判定。默认：**false**
    /// （等价原文 `g_FunctionNPC = nil` ⇒ 整段跳过；与 `Sweep9FormsSmsSeams` 同一取向）。</summary>
    public static Func<bool> HasFunctionNpc { get; set; } = () => false;

    /// <summary>原文 `g_FunctionNPC.GotoLable(Self, sLabel, False)`（三参）的托管落点。
    /// 参数：本对象 / **功能 NPC 名**（原文是 `g_FunctionNPC` 自身的 `m_sCharName`；
    /// 本片调用点一律传空串 —— 宿主接线后应由宿主按其 NPC 实例填充）/
    /// 标签串。原文的 `False` 是 `boExtend`（本片逐字不带它，
    /// 因为托管侧 `g_FunctionNPC` 的类型未知 —— 一旦宿主接线，Host 由它自己决定）。
    /// 本片调用点：39853/39862/40024/40066/40738/40747/40756/40765/40774/
    /// 40785/40787/40789/41052/41064/41075。默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject, string, string> GotoLable { get; set; } = (_, _, _) => { };

    /// <summary>原文 `TMerchant(ProcessMsg.BaseObject).GotoLable(Self, ProcessMsg.sMsg, False)`
    /// （`ObjPlayer.pas:39226`，`ServerSendQueryDealFail`）。托管 `TMerchant` 未移植
    /// （`PluginInterfaceHost.cs:106` 的 `FindMerchant` 只是插件接口面），
    /// 故与 <see cref="GotoLable"/> 分开：这条的接收者是**商户 NPC**、标签来自报文。
    /// 参数：调用者（原文 `Self`）/ 标签串 / 原文的 `False`。默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject, string, bool> GotoLableMerchant { get; set; } = (_, _, _) => { };

    /// <summary>原文 `UserEngine.FindMagic(wMagicId: Word): pTMagic`（`UsrEngn.pas`；
    /// 本片调用点：39715 `ServerSendLightingEx`）。托管侧 `TMagic` 只在
    /// 插件接缝面出现（`PluginInterfaceSeams.cs:289`），故以**出参**表达它被用到的
    /// 三个字段：`wMagicId` / `btEffectType` / `btEffect`（原文 39718 的
    /// `MakeLong(Magic.wMagicId, MakeWord(Magic.btEffectType, Magic.btEffect))`）。
    /// 返回 false 等价原文 `Magic = nil`。默认：未找到。</summary>
    public static FindMagicEffectFn FindMagicEffect { get; set; } = (int _, out int a, out int b, out int c) =>
    {
        a = 0; b = 0; c = 0; return false;
    };

    /// <summary><see cref="FindMagicEffect"/> 的委托类型（4 个出参无法用 `Func` 表达）。</summary>
    public delegate bool FindMagicEffectFn(int magicId, out int wMagicId, out int btEffectType, out int btEffect);

    /// <summary>原文 `TBaseObject(ProcessMsg.nParam1).m_btRaceServer`（本片调用点：39901）。
    /// 参数为对象句柄。默认：0（★ 原文 `RC_PLAYOBJECT` 非 0，故默认下"不是玩家"）。</summary>
    public static Func<nint, int> GetRaceServerOf { get; set; } = _ => 0;

    /// <summary>原文 `TPlayObject` 实例 → 托管侧 `nint` 句柄（原文 `NativeInt(OnlineObject)`，
    /// 本片调用点：39946）。默认用对象标识（`RuntimeHelpers.GetHashCode` 口径不可靠，
    /// 故回落 0 并由宿主覆盖）。</summary>
    public static Func<TPlayObject, nint> HandleOf { get; set; } = _ => 0;

    /// <summary>原文 `UserItemToClientItem(UserItem, StdItem, @ClientItem, True, True)`
    /// 的**字节块**形态（本片调用点：39935）。原文把 `TClientItem` **就地写满**；
    /// 托管 `PlayerSurfaceItemSeams.UserItemToClientItem`（`TCreature.PlayerSurface.Items.cs:47`）
    /// 返回的是 `object?`（视图），口径不同，故本片另立字节接口。
    /// 返回 null 等价"接缝未接线"（此时调用方保持 `default(TClientItem)`）。默认：null。</summary>
    public static Func<TUserItem, TStdItem, bool, bool, byte[]?> UserItemToClientItemBytes { get; set; }
        = (_, _, _, _) => null;

    /// <summary>原文 `AddGameDataLog(LogAction1, LogAction2: Byte; LogActor: TBaseObject;
    /// ItemName: string; ItemMakeIndex: Integer; TargetName: string; Data1, Data2: Integer;
    /// LogDesc: string)`（`M2Share.pas:3121`，**9 参**）。本片调用点：40702。
    /// ⚠ `PlayerSurfaceCore1Seams.AddGameDataLog`（`Core1.cs:216`）是**同签名**的接缝，
    /// 但 `LogActor` 收的是 `TCreature`；本片调用点传的是 `Self`（`TPlayObject` 派生自
    /// `TCreature`），本可复用 —— 为了不把两片的接线互相牵制，按本片惯例另立一份。
    /// 默认：无宿主，丢弃。</summary>
    public static Action<byte, byte, TCreature, string, int, string, int, int, string> AddGameDataLog { get; set; }
        = (_, _, _, _, _, _, _, _, _) => { };

    /// <summary>原文 `g_sCanotTryDealMsg`（`M2Share.pas`；本片调用点：
    /// 39959 / 39964 —— 密码锁/操作间隔两条守卫用的都是它）。默认：""。</summary>
    public static Func<string> GetCanotTryDealMsg { get; set; } = () => "";

    /// <summary>原文 `m_TargetCret.m_TargetCret = Self`（本片调用点：40020）。
    /// 参数为 `m_TargetCret` 实例（托管 `m_TargetCret` 是 `TCreature?`）。
    /// 默认：false。</summary>
    public static Func<TCreature, bool> TargetCretIsSelf { get; set; } = _ => false;

    /// <summary>原文 `Player.m_HeroM2ShopList.Remove(Self)`（本片调用点：40726）。
    /// ★ 原文如此：`Self`（`TPlayObject`）不可能等于该表里的 `pTHeroM2ShopItemInfo`
    /// 元素 ⇒ Delphi `TList.Remove` **恒为未命中**。托管侧 `List&lt;T&gt;.Remove(T)`
    /// 类型不匹配、无法直接表达，故按同一语义落成显式无操作接缝（**不是**"补一句
    /// 正确的 Remove"——那会改变原文行为）。默认：无操作。</summary>
    public static Action<TPlayObject, TPlayObject> RemoveSelfFromShopItemList { get; set; } = (_, _) => { };

    /// <summary>恢复全部默认实现（单测隔离用）。</summary>
    public static void ResetDefaults()
    {
        SendChangeGuildName = _ => { };
        SendAdjustBonus = _ => { };
        GetMyStatus = _ => 0;
        SendDelItemList = (_, _, _) => { };
        SendSaveItemList = (_, _, _) => { };
        SendSaveBigStorageItemList = (_, _, _) => { };
        SendPreviewMonItem = (_, _) => { };
        HealthSpellChanged = _ => { };
        GetNGAddPower = _ => 0;
        GetNGDecPower = _ => 0;
        CanUseCallMonMagic = (_, _) => false;
        GetPlayObjectByHandle = _ => null;
        FindMerchant = _ => null;
        SysMsgTo = (_, _, _, _, _) => { };
        GetFeatureToLong = (_, _, _) => (0, Array.Empty<byte>());
        GetMaster = _ => 0;
        GetSmsVerifySendInterval = () => 0;
        IsValidObjectEx = (_, _, _, _, _) => false;
        CretInNearXY = (_, _, _) => false;
        GetCustomMoneyNameByRule = (_, _, _) => "";
        GetLowestSellingPrice = null;
        GetHighestSellingPrice = null;
        GetUserItemBindValueItem = (item, bit) => (item.btBindOption & (1 << bit)) != 0;
        GetCannotSellMsg = () => "";
        STRING_GOLDNAME = "金币";
        GetGameGoldName = () => "";
        GetGamePointName = () => "";
        GetGameDiamondName = () => "";
        GetGameGirdName = () => "";
        GetRedMsgFColor = () => 255;
        GetRedMsgBColor = () => 253;
        HeroM2BuyerMsg = "";
        HeroM2SellerMsg = "";
        HorseStopShop = "";
        HorseBrand = "";
        HorseMapDisable = "";
        HorseTime = "";
        HorseNotInvitePlayer = "";
        HorseInviteNoPlayer = "";
        HorseInviteMustPlayer = "";
        PoseDisableHorseInviteMsg = "";
        HorseInviteFar = "";
        HorseInviteOnHorse = "";
        ResponseHorseNo = "";
        ResponseHorseTimeOut = "";
        ResponseHorseOtherPlayer = "";
        ResponseHorseFar = "";
        GetHorseTakeTime = () => 0;
        GetTakeOnHorseUseTime = () => 0;
        GetLockStall = () => false;
        GetOpenSelfShop = () => false;
        GetSafeZoneShop = () => false;
        GetMapShop = () => false;
        GetEnvirAllowUseMyshop = _ => false;
        GetEnvirNoAutoOnline = _ => false;
        GetEnvirNoHorse = _ => false;
        GetInSafeZone = _ => false;
        StopCollect = _ => { };
        TriggerHorseScript = _ => { };
        SendTakeOffHorse = _ => { };
        DoTalkStatusChanged = _ => { };
        RefShowName = _ => { };
        GetStdItem = _ => null;
        SendMsg = (_, _, _, _, _, _, _) => { };
        MoveToMovingObject = (_, _, _, _, _, _) => false;
        GetMovingObjectHandle = (_, _, _, _) => 0;
        GetDisableHorseInvite = _ => false;
        GetChangeAppr = _ => -1;
        GetOnHorseOf = _ => false;
        GetHorseTypeOf = _ => 0;
        GetCurrXOf = _ => 0;
        GetCurrYOf = _ => 0;
        GetDirectionOf = _ => 0;
        GetShopStallOf = _ => false;
        GetLockLogon = _ => false;
        GetPasswordLocked = _ => false;
        GetChangeModeExTick9 = _ => 0;
        GetHeroM2ShopList = _ => new List<THeroM2ShopItemInfo>();
        GetHeroM2ShopOpenList = _ => new List<TPlayObject>();
        GetCurOpenUserHeroM2Shop = _ => "";
        SetCurOpenUserHeroM2Shop = (_, _) => { };
        GetShopName = _ => "";
        GetHeroM2ShopStallTime = _ => 0;
        SetHeroM2ShopStallTime = (_, _) => { };
        GetHeroM2AddItemTime = _ => 0;
        SetHeroM2AddItemTime = (_, _) => { };
        GetQueryUserShopTime = _ => 0;
        SetQueryUserShopTime = (_, _) => { };
        GetGameDiamond = p => p.m_nGameDiamond;
        GetGameGird = p => p.m_nGameGird;
        GetDisableBuy = () => false;
        GetDisableSell = () => false;
        SendUpdateItem = (_, _) => { };
        GetCreatureByHandle = _ => null;
        HasFunctionNpc = () => false;
        GotoLable = (_, _, _) => { };
        GotoLableMerchant = (_, _, _) => { };
        FindMagicEffect = (int _, out int a, out int b, out int c) =>
        {
            a = 0; b = 0; c = 0; return false;
        };
        GetRaceServerOf = _ => 0;
        HandleOf = _ => 0;
        UserItemToClientItemBytes = (_, _, _, _) => null;
        AddGameDataLog = (_, _, _, _, _, _, _, _, _) => { };
        GetCanotTryDealMsg = () => "";
        TargetCretIsSelf = _ => false;
        RemoveSelfFromShopItemList = (_, _) => { };
    }
}

/// <summary>
/// `ObjPlayer.pas` 的**广播/客户端处理器族**（原文 38860-41077，158 条）之第二片。
///
/// 每条方法的签名一律是原文
/// `procedure TPlayObject.Xxx(ProcessMsg: pTProcessMessage; var boResult: Boolean);`
/// 的托管等价：`public void Xxx(TProcessMessage ProcessMsg, ref bool boResult)`。
///
/// ★ **没有一条是 `virtual`**（原文这些方法在 `TPlayObject` 声明段里都是普通方法），
///   故本片全部按普通实例方法移植，**不**加 `virtual`/`override`。
/// </summary>
public partial class TPlayObject
{
    // ==================================================================
    // 本片需要的字段（逐条 grep 确认托管侧无同名成员，见文件头）
    // ==================================================================

    /// <summary>原文 `ObjPlayer.pas:437` 一带 `m_Alcohol: TAbilityAlcohol;`（喝酒/药力状态；
    /// 本片调用点：39609 `SendSocketEx(@m_DefMsg, @m_Alcohol, SizeOf(TAbilityAlcohol))`）。</summary>
    public TAbilityAlcohol m_Alcohol;

    /// <summary>原文 `ObjPlayer.pas:438` 一带 `m_HumMeridians: THumMeridians;`（经脉；
    /// 本片调用点：39615）。原文 `THumMeridians` 在托管侧**无类型定义**，
    /// 故按"1:1 字节块"落地（原文 `SendSocketEx(@m_DefMsg, @m_HumMeridians,
    /// SizeOf(THumMeridians))` —— 托管侧改走 `PlayerSurfaceServerSend2Seams` 的
    /// 字节接缝，见 <see cref="ServerSendAbilityMeridians"/>）。</summary>
    public byte[] m_HumMeridians = Array.Empty<byte>();

    /// <summary>原文 `ObjPlayer.pas` 的 `m_btHorseType: Byte`（马类型；本片调用点：
    /// 40824 `if (m_btHorseType = 0)`、40913 `in [1, 2]`、41034）。
    /// 骑马子系统未移植，字段按原文补上以保住读点。</summary>
    public byte m_btHorseType;

    /// <summary>原文 `m_boHorseMaster: Boolean`（骑马发起人；本片调用点：
    /// 40864 / 40877 / 40886 / 40896 / 41035）。</summary>
    public bool m_boHorseMaster;

    /// <summary>原文 `m_HorseOtherHum: TPlayObject`（马上的另一个人；本片调用点：
    /// 40865 / 40874-40887 / 40893 / 41011 / 41028 / 41036-41046）。
    /// 原文是可空指针 ⇒ 托管 `TPlayObject?`。</summary>
    public TPlayObject? m_HorseOtherHum;

    /// <summary>原文 `m_InviteHorseHum: TPlayObject`（邀请自己共骑的人；本片调用点：
    /// 40983 / 40997 / 41001-41036 / 41055）。</summary>
    public TPlayObject? m_InviteHorseHum;

    /// <summary>原文 `m_dwInviteHorseTick: LongWord`（本片调用点：
    /// 40984 / 41005 / 41056）。</summary>
    public uint m_dwInviteHorseTick;

    /// <summary>原文 `m_boCanOnHorse: Boolean`（本片调用点：
    /// 40846-40862）。</summary>
    public bool m_boCanOnHorse;

    /// <summary>原文 `m_dwCanOnHorseTime: LongWord`（本片调用点：40849-40863）。</summary>
    public uint m_dwCanOnHorseTime;

    /// <summary>原文 `m_dwDownHorseTick: LongWord`（本片调用点：40835 / 40880 / 40892）。</summary>
    public uint m_dwDownHorseTick;

    /// <summary>原文 `m_dwClientTakeHorseTick: LongWord`（本片调用点：
    /// 40861 / 40881 / 40891）。</summary>
    public uint m_dwClientTakeHorseTick;

    /// <summary>原文 `m_boDisableHorseInvite: Boolean`（"被邀请人拒绝共骑"；本片调用点：
    /// 40960 —— 读的是**对方**对象，故走
    /// <see cref="PlayerSurfaceServerSend2Seams.GetDisableHorseInvite"/>）。
    /// 本对象自身的同名标志亦按原文补上。</summary>
    public bool m_boDisableHorseInvite;

    /// <summary>原文 `m_nChangeAppr: Integer`（变身外观；`>= 0` 表示变身中；
    /// 本片调用点：39996 `ServerSendHeroM2StartShopStall`、
    /// 40817 / 40966 / 41023）。</summary>
    public int m_nChangeAppr = -1;

    /// <summary>原文 `m_boShopStall: Boolean`（摆摊中；本片调用点：
    /// 39905 / 40001 / 40017 / 40025 / 40041 / 40064 / 40243 / 40811）。
    /// ⚠ 同名的 `GXX.Client.Scenes.PlaySceneActors.m_boShopStall` 是**另一个程序集**，
    /// 与本处无关。</summary>
    public bool m_boShopStall;

    /// <summary>原文 `m_HeroM2ShopList: TList`（摆摊物品表，元素 `pTHeroM2ShopItemInfo`；
    /// 本片调用点：40014 / 40049 / 40428 / 40440 / 40657 / 40684）。
    /// ⚠ 原文对元素做 `Dispose`；托管 `THeroM2ShopItemInfo` 是 `class`，由 GC 承担。</summary>
    public readonly List<THeroM2ShopItemInfo> m_HeroM2ShopList = new();

    /// <summary>原文 `m_HeroM2ShopOpenList: TList`（"谁打开了我的摊位"表，元素 `TPlayObject`；
    /// 本片调用点：39922-39923 / 40726）。</summary>
    public readonly List<TPlayObject> m_HeroM2ShopOpenList = new();

    /// <summary>原文 `m_sCurOpenUserHeroM2Shop: string`（当前浏览的摊主名；本片调用点：
    /// 39910-39920 / 40721-40729）。</summary>
    public string m_sCurOpenUserHeroM2Shop = "";

    /// <summary>原文 `m_sShopName: string`（摊位名；本片调用点：40029）。</summary>
    public string m_sShopName = "";

    /// <summary>原文 `m_dwHeroM2ShopStallTime: LongWord`（本片调用点：
    /// 40007-40012 / 40043-40048）。</summary>
    public uint m_dwHeroM2ShopStallTime;

    /// <summary>原文 `m_dwHeroM2AddItemTime: LongWord`（本片调用点：40434-40439）。</summary>
    public uint m_dwHeroM2AddItemTime;

    /// <summary>原文 `m_dwQueryUserShopTime: LongWord`（本片调用点：39892-39894）。</summary>
    public uint m_dwQueryUserShopTime;

    /// <summary>原文 `m_boAutoOnline: Boolean`（自动挂机；本片调用点：
    /// 39852 / 39857）。</summary>
    public bool m_boAutoOnline;

    /// <summary>原文 `m_sCurrentItemNewName: string`（本片调用点：40337 / 40339 / 40360）。
    /// ⚠ 与已存在的 `m_sCurrentItemName`（Gold.cs:105）**不同名**，不冲突。</summary>
    public string m_sCurrentItemNewName = "";

    /// <summary>原文 `m_btCurItemMoneyTypeValue: Byte`（本片调用点：40340 / 40361）。</summary>
    public byte m_btCurItemMoneyTypeValue;

    /// <summary>原文 `m_sCurItemMoneyType: string`（本片调用点：40343-40362）。</summary>
    public string m_sCurItemMoneyType = "";

    /// <summary>原文 `m_nCurItemPrices: Integer`（本片调用点：40355 / 40363）。</summary>
    public int m_nCurItemPrices;

    /// <summary>原文 `m_sCurrentOperateUserName: string`（本片调用点：40356 / 40364）。</summary>
    public string m_sCurrentOperateUserName = "";

    /// <summary>原文 `m_boLockLogon: Boolean`（本片调用点：39957 —— 读**自身**）。</summary>
    public bool m_boLockLogon;

    /// <summary>原文 `m_boPasswordLocked: Boolean`（本片调用点：39957 —— 读**自身**）。</summary>
    public bool m_boPasswordLocked;

    /// <summary>原文 `m_dwChangeModeExTick: array[0..] of LongWord`（本片只用到 `[9]`；
    /// 调用点：39962）。为不臆造整个数组长度，只落一个标量并注明原文下标。</summary>
    public uint m_dwChangeModeExTick9;

    /// <summary>原文 `m_nHeroDodgeHPPercent: Integer`（本片调用点：40796）。</summary>
    public int m_nHeroDodgeHPPercent;

    /// <summary>原文 `m_boHeroAutoShield: Boolean`（本片调用点：40797）。</summary>
    public bool m_boHeroAutoShield;

    /// <summary>原文 `m_boAssistantHeroAutoShield: Boolean`（本片调用点：40798）。</summary>
    public bool m_boAssistantHeroAutoShield;

    /// <summary>原文 `m_boHeroContinuousNoHitMon: Boolean`（本片调用点：40799）。</summary>
    public bool m_boHeroContinuousNoHitMon;

    /// <summary>原文 `m_boAutoCHangePoison: Boolean`（本片调用点：40800）。</summary>
    public bool m_boAutoCHangePoison;

    /// <summary>原文 `m_boAllowDeal: Boolean`（本片调用点：40801）。</summary>
    public bool m_boAllowDeal;

    /// <summary>原文 `m_sVerifyCode: string`（本片调用点：39385 `ServerSendVerifyCode`）。
    /// 该方法是本片唯一整体留痕的自身方法，字段仍按原文补上以免悬空。</summary>
    public string m_sVerifyCode = "";

    /// <summary>原文 `m_boMagicAttack: Boolean`（本片调用点：41031-41032）。</summary>
    public bool m_boMagicAttack;

    /// <summary>原文 `m_boOpenLastContinuous: Boolean`（本片调用点：39730
    /// `BoolToInt(m_boOpenLastContinuous)`）。</summary>
    public bool m_boOpenLastContinuous;

    /// <summary>原文 `m_ContinuousMagicOrder: array [0 .. 3] of Byte`（本片调用点：
    /// 39731-39732 —— 四个元素两两 `MakeWord` 打包）。</summary>
    public byte[] m_ContinuousMagicOrder = new byte[4];

    /// <summary>原文 `m_boContinuous: Boolean`（本片调用点：39737 置 True、39753 置 False）。</summary>
    public bool m_boContinuous;

    /// <summary>原文 `m_boSendCanUseContinuous: Boolean`（本片调用点：39754 置 False）。</summary>
    public bool m_boSendCanUseContinuous;

    /// <summary>原文 `m_TargetCret: TBaseObject`（攻击目标；本片调用点：
    /// 40018-40022 的 `if m_TargetCret &lt;&gt; nil then ... m_TargetCret := nil`）。
    /// ⚠ 托管 `TCreature` 上**没有** `m_TargetCret`（全仓只有 `ObjBase.cs:221` 的
    /// `m_Target` 与插件句柄面的 `IBaseObjectHandle.m_TargetCret`），故按原文补在本片。</summary>
    public TCreature? m_TargetCret;

    // ==================================================================
    // 本片私有的 1:1 辅助（原文全局函数在托管侧的落点）
    // ==================================================================

    /// <summary>原文 `LoWord(n: LongWord): Word` 的 `Integer` 实参形态
    /// （PortKit `PlayerSurfacePack.LoWord` 只接 `nint`）。</summary>
    private static ushort SS2LoWord(long n) => (ushort)((ulong)n & 0xFFFF);

    /// <summary>原文 `HiWord(n: LongWord): Word` 的 `Integer` 实参形态。</summary>
    private static ushort SS2HiWord(long n) => (ushort)(((ulong)n >> 16) & 0xFFFF);

    /// <summary>原文 `IntToStr(n: Int64): string`（`SysUtils` 的 Int64 重载，
    /// 故 `Integer`/`Int64` 实参都**不窄化**）。</summary>
    private static string SS2IntToStr(long n) => DelphiRTL.IntToStr(n);

    /// <summary>原文 `Max(a, b)`（`Math` 单元）。</summary>
    private static int SS2MaxInt(int a, int b) => a > b ? a : b;

    /// <summary>原文 `Min(a, b)`（`Math` 单元；**有符号**语义）。</summary>
    private static long SS2MinInt64(long a, long b) => a < b ? a : b;

    /// <summary>原文 `BoolToInt(b: Boolean): Integer`（`True → 1`、`False → 0`）。</summary>
    private static int SS2BoolToInt(bool b) => b ? 1 : 0;

    /// <summary>原文 `LoByte(n: LongWord): Byte`（PortKit 只接 `nint`）。</summary>
    private static byte SS2LoByte(long n) => (byte)((ulong)n & 0xFF);

    /// <summary>原文 `HiByte(n: LongWord): Byte`。</summary>
    private static byte SS2HiByte(long n) => (byte)(((ulong)n >> 8) & 0xFF);

    /// <summary>
    /// 原文 `Tick_Diff(dwStart, dwEnd: LongWord): Integer`（`M2Share.pas:11659`）：
    /// 结束值**小于**起始值（即发生了 32 位回绕）时，按 `$FFFFFFFF - 起始 + 结束` 补齐。
    /// 本片调用点：40007 / 40043（摆摊频率门）。
    /// </summary>
    private static long SS2TickDiff(uint start, uint end)
    {
        if (end < start) return (long)end - start + 0xFFFFFFFFL;
        return (long)end - start;
    }

    /// <summary>原文 `EncodeBuffer(@Rec, SizeOf(Rec)) + EncodeString(sMsg)`（AnsiString 拼接）
    /// —— 两段编码结果的**字节拼接**（与 ServerSend1.cs:341 的 `ConcatBytes` 同义，
    /// 但那份是 `private`，不能跨片复用，故本片另立一份）。</summary>
    private static byte[] SS2ConcatBytes(byte[] a, byte[] b)
    {
        byte[] r = new byte[a.Length + b.Length];
        Array.Copy(a, 0, r, 0, a.Length);
        Array.Copy(b, 0, r, a.Length, b.Length);
        return r;
    }

    /// <summary>`TMessageBodyWL` 的 `lTag2` 在原文是 `Int64`（`Grobal2.Types1.cs:146`），
    /// `MakeLong(a, b)` 写进去时高位为零扩展。
    /// （本片不再需要 `uint → long` 的显式助手：`MakeLong` 已返回 `uint`，
    ///  赋给 `long` 字段时由隐式转换完成零扩展。）</summary>
    // 原文如此：`MessageBodyWL.lTag2 := ProcessMsg.nParam3;`（39714）——
    //   `nParam3` 是 `NativeInt`，赋给 `Int64` 字段**不窄化**。

    // ==================================================================
    // 38860-38881  ServerSendOpenHealth
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendOpenHealth(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38860-38881`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendOpenHealth(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38860-38881
        // 原文 38861-38864：var Obj: TBaseObject; Ability: TWAbility; sMsg: string;
        // 原文 38866：// 修复心灵启示显示护身的对象血条错误 chongchong 2014-10-29
        // 原文 38867：Obj := TBaseObject(ProcessMsg.BaseObject);
        nint obj = ProcessMsg.BaseObject;

        // 原文 38868-38872：密名旗标命中 ⇒ dwExp := 0，否则 := 等级
        //   ⚠ 接缝 `GetLevel` 是 `Func<nint,int>`（托管签名），赋给 `m_WAbil.Exp`（`uint`）
        //     时按原文的**位模式**语义 `unchecked` 转换（原文 `Ability.dwExp` 是 `LongWord`）。
        if ((PlayerSurfaceServerSendSeams.GetSecretFlag(obj) & PlayerSurfaceServerSend2Const.SecretFlag_ShowEqualName) != 0
            || (PlayerSurfaceServerSendSeams.GetSecretFlag2(obj) & PlayerSurfaceServerSend2Const.SecretFlag_ShowEqualName) != 0)
            m_WAbil.Exp = 0;
        else
            m_WAbil.Exp = unchecked((uint)PlayerSurfaceServerSendSeams.GetLevel(obj));   // ★ 原文如此：dwExp 取的是 Obj 的 **Level**

        // 原文 38874-38877：四个 `wHP/wMP/wMaxHP/wMaxMP` 在原文里**声明为 `Integer`**
        //   （★ 原文如此 —— 名字带 `w` 却是 4 字节，见 `TWAbility` 的字段表），
        //   故托管侧按位模式显式 `unchecked((int)...)`，不按 `Word` 收窄。
        byte[] ability = PlayerSurfaceServerSend2Structs.WAbilityBytes(
            m_WAbil.Exp,
            unchecked((int)PlayerSurfaceServerSendSeams.GetHP(obj)),
            unchecked((int)PlayerSurfaceServerSendSeams.GetMP(obj)),
            unchecked((int)PlayerSurfaceServerSendSeams.GetMaxHP(obj)),
            unchecked((int)PlayerSurfaceServerSendSeams.GetMaxMP(obj)));

        // 原文 38878：sMsg := EncodeBuffer(@Ability, SizeOf(Ability));
        string sMsg = Encoding.Latin1.GetString(EDcode.EncodeBuffer(ability, ability.Length));
        // 原文 38879
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_OPENHEALTH, ProcessMsg.BaseObject, 0, 0, 0);
        // 原文 38880
        SendSocketRef(m_DefMsg, sMsg);
    }

    // ==================================================================
    // 38883-38886  ServerSendCloseHealth
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendCloseHealth(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38883-38886`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendCloseHealth(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38883-38886
        // 原文 38885
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_CLOSEHEALTH,
            (long)ProcessMsg.BaseObject, 0, 0, 0, "");
    }

    // ==================================================================
    // 38888-38907  ServerSendChangeFace
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendChangeFace(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38888-38907`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。
    ///
    /// 原文结构（逐行）：
    /// <code>
    ///   if (ProcessMsg.nParam1 &lt;&gt; 0) and (ProcessMsg.nParam2 &lt;&gt; 0) then   // 38895
    ///   begin
    ///     m_DefMsg := MakeDefaultMsg(SM_CHANGEFACE, ProcessMsg.nParam1, 0, 0, 0);  // 38897
    ///     I64 := ProcessMsg.nParam2;                                             // 38898
    ///     CharDesc.Feature := TBaseObject(ProcessMsg.nParam2).GetFeature(Self, @Feature);  // 38899
    ///     CharDesc.Status  := TBaseObject(ProcessMsg.nParam2).m_nCharStatus;                // 38900
    ///     SetLength(sSendMsg, SizeOf(I64) + SizeOf(TCharDesc) + CharDesc.Feature);         // 38901
    ///     Move(I64,       sSendMsg[1],                          SizeOf(I64));               // 38902
    ///     Move(CharDesc,  sSendMsg[1 + SizeOf(I64)],            SizeOf(TCharDesc));        // 38903
    ///     Move(Feature[0],sSendMsg[1 + SizeOf(I64) + SizeOf(TCharDesc)], CharDesc.Feature);// 38904
    ///     SendSocketEx(@m_DefMsg, PAnsiChar(sSendMsg), Length(sSendMsg));                   // 38905
    ///   end;
    /// </code>
    /// ★ 关键（**原文如此，不修**）：`:38897` 的 `nRecog` 传的是 **`ProcessMsg.nParam1`**，
    ///   而随后读特征用的却是 **`ProcessMsg.nParam2`** —— 两个参数各司其职，
    ///   不是笔误（`nParam1` = 对象 id、`nParam2` = 对象指针）。
    ///   托管侧 `nParam1`/`nParam2` 都是 `nint`，故 `nRecog` 直接透传。
    /// </summary>
    public void ServerSendChangeFace(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38888-38907
        // 原文 38895
        if (ProcessMsg.nParam1 != 0 && ProcessMsg.nParam2 != 0)
        {
            // 原文 38897
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_CHANGEFACE, ProcessMsg.nParam1, 0, 0, 0);

            // 原文 38898：I64 := ProcessMsg.nParam2;
            long i64 = ProcessMsg.nParam2;

            // 原文 38899-38900（★ 读的是 nParam2，见方法注释）
            nint target = ProcessMsg.nParam2;
            (int featureLen, byte[] featureBytes) = PlayerSurfaceServerSendSeams.GetFeature(target, this);

            // 原文 38901-38904：SetLength + 三次 Move ⇒ Int64 头 + TCharDesc + Feature
            int descSize = PlayerSurfaceServerSend2Structs.SizeOfTCharDesc;
            byte[] sSendMsg = new byte[8 + descSize + featureLen];
            BitConverter.GetBytes(i64).CopyTo(sSendMsg, 0);
            PlayerSurfaceServerSend2Structs.TCharDescBytes(
                (byte)featureLen, PlayerSurfaceServerSendSeams.GetCharStatus(target))
                .CopyTo(sSendMsg, 8);
            Array.Copy(featureBytes, 0, sSendMsg, 8 + descSize,
                Math.Min(featureLen, Math.Max(0, featureBytes.Length)));

            // 原文 38905
            SendSocketExRef(m_DefMsg, sSendMsg);
        }
    }

    // ==================================================================
    // 38909-38914  ServerSend10205
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSend10205(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38909-38914`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSend10205(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38909-38914
        // 原文 38911：// 噬血术打中目标后，自身的吸血动作 '' --> ProcessMsg.sMsg chongchong 2014-05-19
        // 原文 38912-38913
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_716,
            (long)ProcessMsg.BaseObject,
            (ushort)ProcessMsg.nParam1,   // x
            (ushort)ProcessMsg.nParam2,   // y
            (ushort)ProcessMsg.nParam3,   // type
            ProcessMsg.sMsg);
    }

    // ==================================================================
    // 38916-38930  ServerSendAlive
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendAlive(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38916-38930`）。★ **无守卫**（原文如此）。
    ///
    /// 与 `ServerSendChangeFace` 同型，但**不带** `Int64` 头
    /// （`SetLength(sSendMsg, SizeOf(TCharDesc) + CharDesc.Feature)`，:38926）。</summary>
    public void ServerSendAlive(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38916-38930
        // 原文 38922-38923
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_ALIVE, ProcessMsg.BaseObject,
            (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2, (int)ProcessMsg.wParam);

        // 原文 38924-38925
        nint obj = ProcessMsg.BaseObject;
        (int featureLen, byte[] featureBytes) = PlayerSurfaceServerSendSeams.GetFeature(obj, this);

        // 原文 38926-38928：SetLength + 两次 Move ⇒ TCharDesc + Feature
        int descSize = PlayerSurfaceServerSend2Structs.SizeOfTCharDesc;
        byte[] sSendMsg = new byte[descSize + featureLen];
        PlayerSurfaceServerSend2Structs.TCharDescBytes(
            (byte)featureLen, PlayerSurfaceServerSendSeams.GetCharStatus(obj))
            .CopyTo(sSendMsg, 0);
        Array.Copy(featureBytes, 0, sSendMsg, descSize,
            Math.Min(featureLen, Math.Max(0, featureBytes.Length)));

        // 原文 38929
        SendSocketExRef(m_DefMsg, sSendMsg);
    }

    // ==================================================================
    // 38932-38935  ServerSendChangeGuildName
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendChangeGuildName(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38932-38935`）—— 整条只有一次
    /// `SendChangeGuildName()`（原文另处定义的自身方法）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendChangeGuildName(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38932-38935
        // 原文 38934
        PlayerSurfaceServerSend2Seams.SendChangeGuildName(this);
    }

    // ==================================================================
    // 38937-38942  ServerSend10414
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSend10414(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38937-38942`）。★ **无守卫**（原文如此）。
    ///
    /// ★ 原文如此（**不修**）：`:38939` 的 `nRecog` 传 `ProcessMsg.BaseObject`，
    /// 而 `nParam`/`nTag` 拆的是**同一个** `BaseObject` 的 **HP/MaxHP**；
    /// `nSeries` 是 `IntToStr(HiWord(MaxHP))`（**字符串化的 HP 高 16 位**）。</summary>
    public void ServerSend10414(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38937-38942
        // 原文 38939-38941
        nint obj = ProcessMsg.BaseObject;
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_INSTANCEHEALGUAGE,
            (long)obj,
            PlayerSurfacePack.LoWord(PlayerSurfaceServerSendSeams.GetHP(obj)),
            PlayerSurfacePack.HiWord(PlayerSurfaceServerSendSeams.GetHP(obj)),
            PlayerSurfacePack.LoWord(PlayerSurfaceServerSendSeams.GetMaxHP(obj)),
            SS2IntToStr(PlayerSurfacePack.HiWord(PlayerSurfaceServerSendSeams.GetMaxHP(obj))));
    }

    // ==================================================================
    // 38944-38949  ServerSendMenuOK
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendMenuOK(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38944-38949`）。★ **无守卫**（原文如此）。
    /// `:38947` 用 `Pos(sLineBreak + '@', ProcessMsg.sMsg) &gt; 0` 置 `m_boMessageBox`。</summary>
    public void ServerSendMenuOK(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38944-38949
        // 原文 38946：// 修正以下脚本点两次才触发 chongchong 2015-10-3
        // 原文 38947：m_boMessageBox := Pos(sLineBreak + '@', ProcessMsg.sMsg) > 0;
        //   `sLineBreak` = #13#10（Delphi）；Delphi `Pos` 为 1 基，托管 `IndexOf` 为 0 基，
        //   但只参与 `> 0` 判定 ⇒ 只需判"是否存在"。
        m_boMessageBox = ProcessMsg.sMsg.Contains("\r\n@");
        // 原文 38948
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_MENU_OK,
            ProcessMsg.nParam1, 0, 0, 0, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 38951-38954  ServerSendMerchantDlgClose
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendMerchantDlgClose(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38951-38954`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendMerchantDlgClose(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38951-38954
        // 原文 38953
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_MERCHANTDLGCLOSE,
            ProcessMsg.nParam1, 0, 0, 0, "");
    }

    // ==================================================================
    // 38956-38959  ServerSendDelItemList
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendDelItemList(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38956-38959`）—— 整条只有
    /// `SendDelItemList(ProcessMsg.sMsg, ProcessMsg.nParam1);`。
    /// ★ **无守卫**（原文如此）。</summary>
    public void ServerSendDelItemList(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38956-38959
        // 原文 38958
        PlayerSurfaceServerSend2Seams.SendDelItemList(this, ProcessMsg.sMsg, (int)ProcessMsg.nParam1);
    }

    // ==================================================================
    // 38961-38964  ServerSendUsersRepair
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendUsersRepair(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38961-38964`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendUsersRepair(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38961-38964
        // 原文 38963
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_SENDUSERREPAIR,
            ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2, 0, 0, "");
    }

    // ==================================================================
    // 38966-38969  ServerSendGoodsList
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendGoodsList(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38966-38969`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendGoodsList(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38966-38969
        // 原文 38968
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_SENDGOODSLIST,
            ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2, (ushort)ProcessMsg.nParam3, 0, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 38971-38974  ServerSendUserSell
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendUserSell(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38971-38974`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendUserSell(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38971-38974
        // 原文 38973
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_SENDUSERSELL,
            ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2, 0, 0, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 38976-38979  ServerSendUserMakeDrugItemList
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendUserMakeDrugItemList(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38976-38979`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendUserMakeDrugItemList(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38976-38979
        // 原文 38978
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_SENDUSERMAKEDRUGITEMLIST,
            ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2, 0, 0, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 38981-38984  ServerSendUserStorageItem
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendUserStorageItem(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38981-38984`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendUserStorageItem(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38981-38984
        // 原文 38983
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_SENDUSERSTORAGEITEM,
            ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2, 0, 0, "");
    }

    // ==================================================================
    // 38986-38989  ServerSendUserGetBackItem
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendUserGetBackItem(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38986-38989`）—— 整条只有
    /// `SendSaveItemList(ProcessMsg.nParam1, ProcessMsg.nParam2);`。
    /// ★ **无守卫**（原文如此）。</summary>
    public void ServerSendUserGetBackItem(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38986-38989
        // 原文 38988
        PlayerSurfaceServerSend2Seams.SendSaveItemList(this, (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2);
    }

    // ==================================================================
    // 38991-39002  ServerSendSpaceMoveFire
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendSpaceMoveFire(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38991-39002`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。
    ///
    /// `:38993` 按 `ProcessMsg.wIdent = RM_SPACEMOVE_FIRE` 二选一 ident
    /// （`SM_SPACEMOVE_HIDE` / `SM_SPACEMOVE_HIDE2`），随后**同一个** `SendSocket`（:39001）。</summary>
    public void ServerSendSpaceMoveFire(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38991-39002
        // 原文 38993-39000
        if (ProcessMsg.wIdent == Grobal2Const.RM_SPACEMOVE_FIRE)
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_SPACEMOVE_HIDE, ProcessMsg.BaseObject, 0, 0, 0);
        else
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_SPACEMOVE_HIDE2, ProcessMsg.BaseObject, 0, 0, 0);
        // 原文 39001
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 39004-39016  ServerSendBuyItemOK
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendBuyItemOK(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39004-39016`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。
    ///
    /// `:39006` 按 `ProcessMsg.wParam = 1` 二选一 ident（**交易框** vs **普通购买**），
    /// 两条都拆 `ProcessMsg.nParam2` 成 `LoWord`/`HiWord`。</summary>
    public void ServerSendBuyItemOK(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39004-39016
        // 原文 39006
        if (ProcessMsg.wParam == 1)
        {
            // 原文 39008-39009
            PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_TradingBUYITEM_SUCCESS,
                ProcessMsg.nParam1, SS2LoWord(ProcessMsg.nParam2), SS2HiWord(ProcessMsg.nParam2),
                (ushort)ProcessMsg.nParam3, ProcessMsg.sMsg);
        }
        else
        {
            // 原文 39013-39014
            PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_BUYITEM_SUCCESS,
                ProcessMsg.nParam1, SS2LoWord(ProcessMsg.nParam2), SS2HiWord(ProcessMsg.nParam2),
                (ushort)ProcessMsg.nParam3, ProcessMsg.sMsg);
        }
    }

    // ==================================================================
    // 39018-39021  ServerSendBuyItemFail
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendBuyItemFail(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39018-39021`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendBuyItemFail(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39018-39021
        // 原文 39020
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_BUYITEM_FAIL,
            ProcessMsg.nParam1, 0, 0, 0, "");
    }

    // ==================================================================
    // 39023-39028  ServerSendDetailGoodsList
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendDetailGoodsList(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39023-39028`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendDetailGoodsList(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39023-39028
        // 原文 39025-39026
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_SENDDETAILGOODSLIST,
            ProcessMsg.nParam1, (int)ProcessMsg.nParam2, (int)ProcessMsg.nParam3, (int)ProcessMsg.wParam);
        // 原文 39027
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39030-39037  ServerSendBuyPrice
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendBuyPrice(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39030-39037`）。★ **无守卫**（原文如此）。
    /// `:39032` 的 `nRecog` = 价格（`nParam1`），`:39033` 的 `nParam` = `wParam`（是否来自交易框）。</summary>
    public void ServerSendBuyPrice(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39030-39037
        // 原文 39032-39036
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_SENDBUYPRICE,
            ProcessMsg.nParam1,                  // 价格
            (ushort)ProcessMsg.wParam,           // 就否来自于交易框
            (ushort)ProcessMsg.nParam2,          // 放到哪个框中
            (ushort)ProcessMsg.nParam3,          // 是否成功
            "");
    }

    // ==================================================================
    // 39039-39042  ServerSendUserSellItemOK
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendUserSellItemOK(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39039-39042`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendUserSellItemOK(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39039-39042
        // 原文 39041
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_USERSELLITEM_OK,
            ProcessMsg.nParam1, 0, 0, 0, "");
    }

    // ==================================================================
    // 39044-39047  ServerSendUserSellItemFail
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendUserSellItemFail(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39044-39047`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendUserSellItemFail(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39044-39047
        // 原文 39046
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_USERSELLITEM_FAIL,
            ProcessMsg.nParam1, 0, 0, 0, "");
    }

    // ==================================================================
    // 39049-39052  ServerSendMakeDrugOK
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendMakeDrugOK(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39049-39052`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendMakeDrugOK(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39049-39052
        // 原文 39051
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_MAKEDRUG_SUCCESS,
            ProcessMsg.nParam1, 0, 0, 0, "");
    }

    // ==================================================================
    // 39054-39057  ServerSendMakeDrugFail
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendMakeDrugFail(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39054-39057`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendMakeDrugFail(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39054-39057
        // 原文 39056
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_MAKEDRUG_FAIL,
            ProcessMsg.nParam1, 0, 0, 0, "");
    }

    // ==================================================================
    // 39059-39062  ServerSendRepairCost
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendRepairCost(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39059-39062`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendRepairCost(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39059-39062
        // 原文 39061
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_SENDREPAIRCOST,
            ProcessMsg.nParam1, 0, 0, 0, "");
    }

    // ==================================================================
    // 39064-39067  ServerSendUserRepairOK
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendUserRepairOK(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39064-39067`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendUserRepairOK(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39064-39067
        // 原文 39066
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_USERREPAIRITEM_OK,
            ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2, (ushort)ProcessMsg.nParam3, 0, "");
    }

    // ==================================================================
    // 39069-39072  ServerSendUserRepairFail
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendUserRepairFail(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39069-39072`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendUserRepairFail(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39069-39072
        // 原文 39071
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_USERREPAIRITEM_FAIL,
            ProcessMsg.nParam1, 0, 0, 0, "");
    }

    // ==================================================================
    // 39074-39083  ServerSendPlayDice
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendPlayDice(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39074-39083`）。★ **无守卫**（原文如此）。
    ///
    /// `:39078-39080` 只填 `TMessageBodyWL` 的 **lParam1/lParam2/lTag1**（`lTag2` 留 0），
    /// `:39082` 尾数据 = `EncodeBuffer(@Record) + EncodeString(sMsg)`。</summary>
    public void ServerSendPlayDice(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39074-39083
        // 原文 39078-39080
        TMessageBodyWL body = default;
        body.lParam1 = (int)ProcessMsg.nParam1;
        body.lParam2 = (int)ProcessMsg.nParam2;
        body.lTag1 = (int)ProcessMsg.nParam3;

        // 原文 39081
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_PLAYDICE, ProcessMsg.BaseObject, (int)ProcessMsg.wParam, 0, 0);

        // 原文 39082：SendSocket(@m_DefMsg, EncodeBuffer(@MessageBodyWL, SizeOf(TMessageBodyWL)) + EncodeString(ProcessMsg.sMsg));
        byte[] bodyBytes = PlayerSurfaceServerSend2Structs.MessageBodyWLBytes(body);
        byte[] msg = SS2ConcatBytes(bodyBytes, EDcode.EncodeString(ProcessMsg.sMsg));
        SendSocketRef(m_DefMsg, Encoding.Latin1.GetString(msg));
    }

    // ==================================================================
    // 39085-39088  ServerSendAdjustBonus
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendAdjustBonus(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39085-39088`）—— 整条只有 `SendAdjustBonus();`。
    /// ★ **无守卫**（原文如此）。</summary>
    public void ServerSendAdjustBonus(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39085-39088
        // 原文 39087
        PlayerSurfaceServerSend2Seams.SendAdjustBonus(this);
    }

    // ==================================================================
    // 39090-39093  ServerSendBuildGuildOK
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendBuildGuildOK(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39090-39093`）。★ **无守卫**（原文如此）。
    /// ★ 原文如此：`nRecog` 传字面量 **0**（不是 `BaseObject`）。</summary>
    public void ServerSendBuildGuildOK(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39090-39093
        // 原文 39092
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_BUILDGUILD_OK,
            0, 0, 0, 0, "");
    }

    // ==================================================================
    // 39095-39098  ServerSendBuildGuildFail
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendBuildGuildFail(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39095-39098`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendBuildGuildFail(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39095-39098
        // 原文 39097
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_BUILDGUILD_FAIL,
            ProcessMsg.nParam1, 0, 0, 0, "");
    }

    // ==================================================================
    // 39100-39103  ServerSendDonateOK
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendDonateOK(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39100-39103`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendDonateOK(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39100-39103
        // 原文 39102
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_DONATE_OK,
            ProcessMsg.nParam1, 0, 0, 0, "");
    }

    // ==================================================================
    // 39105-39108  ServerSendGameGoldChanged
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendGameGoldChanged(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39105-39108`）—— 整条只有 `SendGoldInfo(False);`。
    /// ★ **无守卫**（原文如此）。</summary>
    public void ServerSendGameGoldChanged(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39105-39108
        // 原文 39107
        SendGoldInfo(false);
    }

    // ==================================================================
    // 39110-39113  ServerSendGamePointChanged
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendGamePointChanged(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39110-39113`）—— 整条只有 `SendNewGamePointInfo(False);`。
    /// ★ **无守卫**（原文如此）。</summary>
    public void ServerSendGamePointChanged(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39110-39113
        // 原文 39112
        SendNewGamePointInfo(false);
    }

    // ==================================================================
    // 39115-39118  ServerSendGameGlory
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendGameGlory(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39115-39118`）—— 整条只有 `SendGameGlory;`。
    /// ★ **无守卫**（原文如此）。</summary>
    public void ServerSendGameGlory(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39115-39118
        // 原文 39117：SendGameGlory; // 荣誉
        SendGameGlory();
    }

    // ==================================================================
    // 39120-39123  ServerSendMyStaus
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendMyStaus(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39120-39123`）。★ **无守卫**（原文如此）。
    /// `nParam` 取的是自身方法 `GetMyStatus`（**不是** `ProcessMsg` 的字段）。</summary>
    public void ServerSendMyStaus(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39120-39123
        // 原文 39122
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_MYSTATUS,
            0, (ushort)PlayerSurfaceServerSend2Seams.GetMyStatus(this), 0, 0, "");
    }

    // ==================================================================
    // 39125-39128  ServerSendMagicFireFail
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendMagicFireFail(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39125-39128`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendMagicFireFail(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39125-39128
        // 原文 39127
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_MAGICFIRE_FAIL,
            (long)ProcessMsg.BaseObject, 0, 0, 0, "");
    }

    // ==================================================================
    // 39130-39133  ServerSendLampChangeDura
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendLampChangeDura(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39130-39133`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendLampChangeDura(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39130-39133
        // 原文 39132
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_LAMPCHANGEDURA,
            ProcessMsg.nParam1, 0, 0, 0, "");
    }

    // ==================================================================
    // 39135-39138  ServerSendGroupCancel
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendGroupCancel(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39135-39138`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendGroupCancel(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39135-39138
        // 原文 39137
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_GROUPCANCEL,
            ProcessMsg.nParam1, 0, 0, 0, "");
    }

    // ==================================================================
    // 39140-39143  ServerSendDonateFail
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendDonateFail(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39140-39143`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendDonateFail(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39140-39143
        // 原文 39142
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_DONATE_FAIL,
            ProcessMsg.nParam1, 0, 0, 0, "");
    }

    // ==================================================================
    // 39145-39148  ServerSendBreakWeapon
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendBreakWeapon(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39145-39148`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendBreakWeapon(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39145-39148
        // 原文 39147
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_BREAKWEAPON,
            (long)ProcessMsg.BaseObject, 0, 0, 0, "");
    }

    // ==================================================================
    // 39150-39153  ServerSendPassword
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendPassword(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39150-39153`）。★ **无守卫**（原文如此）。
    /// ★ 原文如此：`nRecog` 传字面量 **0**。</summary>
    public void ServerSendPassword(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39150-39153
        // 原文 39152
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_PASSWORD,
            0, 0, 0, 0, "");
    }

    // ==================================================================
    // 39155-39160  ServerSendPasswordStatus
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendPasswordStatus(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39155-39160`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendPasswordStatus(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39155-39160
        // 原文 39157-39158
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_PASSWORDSTATUS, ProcessMsg.BaseObject,
            (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2, (int)ProcessMsg.nParam3);
        // 原文 39159
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39162-39166  ServerSendClickNpcLabel
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendClickNpcLabel(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39162-39166`）。★ **无守卫**（原文如此）。
    /// `:39165` 尾数据走 **`EncodeString`**（不是裸串）。</summary>
    public void ServerSendClickNpcLabel(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39162-39166
        // 原文 39164
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_CLICKNPCLABEL, ProcessMsg.nParam1, 0, 0, 0);
        // 原文 39165：SendSocket(@m_DefMsg, EncodeString(ProcessMsg.sMsg));
        SendSocketRef(m_DefMsg, Encoding.Latin1.GetString(EDcode.EncodeString(ProcessMsg.sMsg)));
    }

    // ==================================================================
    // 39168-39170  ServerSendQueryBagItems —— 原文空实现
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendQueryBagItems(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39168-39170`）—— **原文整条为空体**（`begin end;`），
    /// 无 `&lt;&gt; Self` 守卫、无任何语句。忠实移植 = **空体**（不是留痕桩）。</summary>
    public void ServerSendQueryBagItems(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39168-39170：空实现（原文 `begin` / `end;` 之间**没有任何语句**）
    }

    // ==================================================================
    // 39172-39175  ServerSendTakeOnItem
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendTakeOnItem(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39172-39175`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendTakeOnItem(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39172-39175
        // 原文 39174
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_TAKEONITEM,
            ProcessMsg.nParam1, (ushort)ProcessMsg.wParam, 0, 0, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39177-39180  ServerSendTakeOffItem
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendTakeOffItem(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39177-39180`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendTakeOffItem(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39177-39180
        // 原文 39179
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_TAKEOFFITEM,
            ProcessMsg.nParam1, (ushort)ProcessMsg.wParam, 0, 0, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39182-39185  ServerSendDeleteDelayMessage
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendDeleteDelayMessage(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39182-39185`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendDeleteDelayMessage(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39182-39185
        // 原文 39184
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_DELETEDELAYMESSAGE,
            ProcessMsg.nParam1, 0, 0, 0, "");
    }

    // ==================================================================
    // 39187-39191  ServerSendHeroLogout
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendHeroLogout(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39187-39191`）。★ **无守卫**（原文如此）。
    /// ★ 原文如此：`nParam`/`nTag` 取 **`nParam2`/`nParam3`**（跳过 `nParam1`）。</summary>
    public void ServerSendHeroLogout(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39187-39191
        // 原文 39189
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_HEROLOGOUT, ProcessMsg.BaseObject,
            (int)ProcessMsg.nParam2, (int)ProcessMsg.nParam3, 0);
        // 原文 39190
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 39193-39209  ServerSendHeroLogon
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendHeroLogon(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39193-39209`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。
    ///
    /// <code>
    ///   m_DefMsg := MakeDefaultMsg(SM_HEROLOGON, NativeInt(BaseObject), nParam2, nParam3,
    ///                              wParam { MakeWord(m_btDirection, m_btGender) });   // 39199-39200
    ///   MessageBodyWL.lParam1 := TBaseObject(BaseObject).GetFeatureToLong(@Feature);      // 39201
    ///   MessageBodyWL.lParam2 := TBaseObject(BaseObject).m_nCharStatus;                   // 39202
    ///   MessageBodyWL.lTag1   := 0; // TBaseObject(BaseObject).GetFeatureEx;               // 39203
    ///   MessageBodyWL.lTag2   := NativeInt(TBaseObject(BaseObject).m_Master);             // 39204
    ///   SetLength(S, SizeOf(TMessageBodyWL) + MessageBodyWL.lParam1);                     // 39205
    ///   Move(MessageBodyWL, S[1], SizeOf(TMessageBodyWL));                                // 39206
    ///   Move(Feature[0], S[1 + SizeOf(TMessageBodyWL)], MessageBodyWL.lParam1);           // 39207
    ///   SendSocket(@m_DefMsg, S);                                                         // 39208
    /// </code>
    /// ★ 原文如此：`lTag1` 那句右侧是**注释掉的** `GetFeatureEx`，实际写死 0。</summary>
    public void ServerSendHeroLogon(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39193-39209
        // 原文 39199-39200
        nint obj = ProcessMsg.BaseObject;
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_HEROLOGON, obj,
            (int)ProcessMsg.nParam2, (int)ProcessMsg.nParam3, (int)ProcessMsg.wParam);

        // 原文 39201-39204
        TMessageBodyWL body = default;
        (int featureLen, byte[] featureBytes) = PlayerSurfaceServerSend2Seams.GetFeatureToLong(obj, this, 256);
        body.lParam1 = featureLen;
        body.lParam2 = PlayerSurfaceServerSendSeams.GetCharStatus(obj);
        body.lTag1 = 0; // TBaseObject(BaseObject).GetFeatureEx;
        body.lTag2 = (long)PlayerSurfaceServerSend2Seams.GetMaster(obj);

        // 原文 39205-39207：TMessageBodyWL + Feature
        byte[] bodyBytes = PlayerSurfaceServerSend2Structs.MessageBodyWLBytes(body);
        byte[] s = new byte[bodyBytes.Length + featureLen];
        bodyBytes.CopyTo(s, 0);
        Array.Copy(featureBytes, 0, s, bodyBytes.Length,
            Math.Min(featureLen, Math.Max(0, featureBytes.Length)));

        // 原文 39208（SendSocket 第二参是 AnsiString 承载的字节）
        SendSocketRef(m_DefMsg, Encoding.Latin1.GetString(s));
    }

    // ==================================================================
    // 39211-39215  ServerSendGetRegInfo
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendGetRegInfo(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39211-39215`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendGetRegInfo(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39211-39215
        // 原文 39213
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_GETREGINFO, ProcessMsg.BaseObject, 0, 0, 0);
        // 原文 39214
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39217-39221  ServerSendGameGoldDalItem
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendGameGoldDalItem(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39217-39221`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendGameGoldDalItem(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39217-39221
        // 原文 39219
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_SENDGAMEGOLDDALITEM, ProcessMsg.BaseObject, 0, 0, 0);
        // 原文 39220
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39223-39227  ServerSendQueryDealFail
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendQueryDealFail(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39223-39227`）。★ **无守卫**（原文如此）。
    ///
    /// `:39225` 先查 `UserEngine.FindMerchant(TObject(ProcessMsg.BaseObject))`，
    /// 非 nil 才 `TMerchant(...).GotoLable(Self, ProcessMsg.sMsg, False)`。
    /// 托管侧 `TMerchant`/`TUserEngine.FindMerchant` 未移植 ⇒ 走
    /// <see cref="PlayerSurfaceServerSend2Seams.FindMerchant"/> +
    /// <see cref="PlayerSurfaceServerSend2Seams.GotoLableMerchant"/>（**报文身份与
    /// 参数顺序逐字保留**：`GotoLable(Self, sMsg, False)`）。</summary>
    public void ServerSendQueryDealFail(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39223-39227
        // 原文 39225：if UserEngine.FindMerchant(TObject(ProcessMsg.BaseObject)) <> nil then
        TPlayObject? merchant = PlayerSurfaceServerSend2Seams.FindMerchant(ProcessMsg.BaseObject);
        if (merchant != null)
            // 原文 39226：TMerchant(ProcessMsg.BaseObject).GotoLable(Self, ProcessMsg.sMsg, False);
            PlayerSurfaceServerSend2Seams.GotoLableMerchant(this, ProcessMsg.sMsg, false);
    }

    // ==================================================================
    // 39229-39232  ServerSendPlaySound
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendPlaySound(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39229-39232`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendPlaySound(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39229-39232
        // 原文 39231
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_PLAYSOUND,
            ProcessMsg.nParam1, (ushort)ProcessMsg.wParam, 0, 0, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39234-39237  ServerSendStopSound
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendStopSound(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39234-39237`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendStopSound(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39234-39237
        // 原文 39236
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_STOPSOUND,
            ProcessMsg.nParam1, (ushort)ProcessMsg.wParam, 0, 0, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39239-39242  ServerSendPlaySoundEx
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendPlaySoundEx(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39239-39242`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendPlaySoundEx(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39239-39242
        // 原文 39241
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_PLAYSOUND_EX,
            ProcessMsg.nParam1, (ushort)ProcessMsg.wParam, 0, 0, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39244-39247  ServerSendPlaySoundExt
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendPlaySoundExt(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39244-39247`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendPlaySoundExt(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39244-39247
        // 原文 39246
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_PLAYSOUNDEXT,
            ProcessMsg.nParam1, (ushort)ProcessMsg.wParam, 0, 0, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39249-39252  ServerSendPlayEffect
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendPlayEffect(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39249-39252`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendPlayEffect(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39249-39252
        // 原文 39251
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_PLAYEFFECT,
            ProcessMsg.nParam1, (ushort)ProcessMsg.wParam, (ushort)ProcessMsg.nParam2,
            (ushort)ProcessMsg.nParam3, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39254-39262  ServerSendScreenEffect
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendScreenEffect(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39254-39262`）。
    /// ★ **自带两道守卫**（原文如此）：`:39256` `if m_boDummyObject then Exit;`、
    /// `:39258` `if m_boOffLine and (MyGetTickCount - m_nOffOnlineTick > 1000 * 30) then Exit;`。
    ///
    /// ★ 原文如此（**不修**）：`MakeDefaultMsg` 只传了 5 个参数位，而 `nRecog` 位拿到的是
    /// `ProcessMsg.nParam1`（**不是** `BaseObject`）—— 即该报文的 `nRecog` 字段承载的是
    /// "特效编号"，与 `BaseObject` 无关。</summary>
    public void ServerSendScreenEffect(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39254-39262
        // 原文 39256-39257
        if (m_boDummyObject) return;
        // 原文 39258-39259：// 如果挂机超过30秒后不在发送数据到客户端
        if (m_boOffLine && unchecked(PlayerSurfaceServerSendSeams.MyGetTickCount() - m_nOffOnlineTick) > 1000 * 30)
            return;

        // 原文 39260
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_SCREENEFFECT, ProcessMsg.nParam1,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam2, (int)ProcessMsg.nParam3);
        // 原文 39261
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39264-39272  ServerSendStopScreenEffect
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendStopScreenEffect(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39264-39272`）。与 <see cref="ServerSendScreenEffect"/> **逐行同构**，
    /// 只差 ident（`SM_STOPSCREENEFFECT`）。★ 自带两道守卫（原文如此）。</summary>
    public void ServerSendStopScreenEffect(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39264-39272
        // 原文 39266-39267
        if (m_boDummyObject) return;
        // 原文 39268-39269
        if (m_boOffLine && unchecked(PlayerSurfaceServerSendSeams.MyGetTickCount() - m_nOffOnlineTick) > 1000 * 30)
            return;

        // 原文 39270
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_STOPSCREENEFFECT, ProcessMsg.nParam1,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam2, (int)ProcessMsg.nParam3);
        // 原文 39271
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39274-39282  ServerSendClearScreenEffect
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendClearScreenEffect(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39274-39282`）。与 <see cref="ServerSendScreenEffect"/> **逐行同构**，
    /// 只差 ident（`SM_CLEARSCREENEFFECT`）。★ 自带两道守卫（原文如此）。</summary>
    public void ServerSendClearScreenEffect(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39274-39282
        // 原文 39276-39277
        if (m_boDummyObject) return;
        // 原文 39278-39279
        if (m_boOffLine && unchecked(PlayerSurfaceServerSendSeams.MyGetTickCount() - m_nOffOnlineTick) > 1000 * 30)
            return;

        // 原文 39280
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_CLEARSCREENEFFECT, ProcessMsg.nParam1,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam2, (int)ProcessMsg.nParam3);
        // 原文 39281
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39284-39287  ServerSendChangeSpeed
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendChangeSpeed(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39284-39287`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendChangeSpeed(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39284-39287
        // 原文 39286
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_CHANGESPEED,
            ProcessMsg.nParam1, (ushort)ProcessMsg.wParam, (ushort)ProcessMsg.nParam2,
            (ushort)ProcessMsg.nParam3, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39289-39292  ServerSendServerConfig
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendServerConfig(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39289-39292`）—— 整条只有 `SendServerConfig();`。
    /// ★ **无守卫**（原文如此）。托管同名实例方法见 Core4.cs:847。</summary>
    public void ServerSendServerConfig(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39289-39292
        // 原文 39291
        SendServerConfig();
    }

    // ==================================================================
    // 39294-39297  ServerSendOpenUpgradeDlg
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendOpenUpgradeDlg(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39294-39297`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendOpenUpgradeDlg(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39294-39297
        // 原文 39296
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_OPENUPGRADEDLG,
            ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2, 0, 0, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39299-39302  ServerSendUserIcon
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendUserIcon(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39299-39302`）—— 整条只有
    /// `SendUseIcons(TBaseObject(ProcessMsg.BaseObject));`。★ **无守卫**（原文如此）。
    /// 托管 `SendUseIcons(TCreature)` 见 Core4.cs:890 —— `nint` 无法反解实例，
    /// 故经 <see cref="PlayerSurfaceServerSend2Seams.GetCreatureByHandle"/> 转接。</summary>
    public void ServerSendUserIcon(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39299-39302
        // 原文 39301
        TCreature? creature = PlayerSurfaceServerSend2Seams.GetCreatureByHandle(ProcessMsg.BaseObject);
        SendUseIcons(creature!);
    }

    // ==================================================================
    // 39304-39307  ServerSendWebBrowser
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendWebBrowser(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39304-39307`）。★ **无守卫**（原文如此）。
    /// ★ 原文如此：`nRecog` 传字面量 **0**。</summary>
    public void ServerSendWebBrowser(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39304-39307
        // 原文 39306
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_SENDWEBBROWSER,
            0, 0, 0, 0, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39309-39312  ServerSendUserEffect
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendUserEffect(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39309-39312`）—— 整条只有
    /// `SendUseEffects(TBaseObject(ProcessMsg.BaseObject));`。★ **无守卫**（原文如此）。</summary>
    public void ServerSendUserEffect(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39309-39312
        // 原文 39311
        TCreature? creature = PlayerSurfaceServerSend2Seams.GetCreatureByHandle(ProcessMsg.BaseObject);
        SendUseEffects(creature!);
    }

    // ==================================================================
    // 39314-39319  ServerSendSuperShiledEffect
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendSuperShiledEffect(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39314-39319`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendSuperShiledEffect(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39314-39319
        // 原文 39316-39317
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_SENDSUPERSHILEDEFFECT, ProcessMsg.BaseObject,
            (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2, (int)ProcessMsg.wParam);
        // 原文 39318
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 39321-39326  ServerSendBlastHit
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendBlastHit(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39321-39326`）。★ **无守卫**（原文如此）。
    /// ★ 原文如此：`nParam`/`nTag` 拆 `wParam`，而 `nSeries` **又**传整个 `wParam`
    /// （三条 `*BLASTHIT` 同款）。</summary>
    public void ServerSendBlastHit(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39321-39326
        // 原文 39323-39324
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_SENDBLASTHIT, ProcessMsg.BaseObject,
            SS2LoWord(ProcessMsg.wParam), SS2HiWord(ProcessMsg.wParam), (int)ProcessMsg.wParam);
        // 原文 39325
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 39328-39333  ServerSendContinuousBLASTHIT
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendContinuousBLASTHIT(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39328-39333`）。与 <see cref="ServerSendBlastHit"/> 逐行同构，
    /// 只差 ident（`SM_ContinuousBLASTHIT`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendContinuousBLASTHIT(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39328-39333
        // 原文 39330-39331
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_ContinuousBLASTHIT, ProcessMsg.BaseObject,
            SS2LoWord(ProcessMsg.wParam), SS2HiWord(ProcessMsg.wParam), (int)ProcessMsg.wParam);
        // 原文 39332
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 39335-39340  ServerSendNewHitBubbleDefence
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendNewHitBubbleDefence(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39335-39340`）。与 <see cref="ServerSendBlastHit"/> 逐行同构，
    /// 只差 ident（`SM_NewHitBubbleDefence`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendNewHitBubbleDefence(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39335-39340
        // 原文 39337-39338
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_NewHitBubbleDefence, ProcessMsg.BaseObject,
            SS2LoWord(ProcessMsg.wParam), SS2HiWord(ProcessMsg.wParam), (int)ProcessMsg.wParam);
        // 原文 39339
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 39342-39347  ServerSendOpenhumDlg
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendOpenhumDlg(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39342-39347`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendOpenhumDlg(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39342-39347
        // 原文 39344-39345
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_OPENHUMDLG, ProcessMsg.BaseObject,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2);
        // 原文 39346
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 39349-39354  ServerSendOpenHeroDlg
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendOpenHeroDlg(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39349-39354`）。与 <see cref="ServerSendOpenhumDlg"/> 逐行同构，
    /// 只差 ident（`SM_OPENHERODLG`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendOpenHeroDlg(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39349-39354
        // 原文 39351-39352
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_OPENHERODLG, ProcessMsg.BaseObject,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2);
        // 原文 39353
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 39356-39361  ServerSendAttackMiss
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendAttackMiss(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39356-39361`）。与 <see cref="ServerSendOpenhumDlg"/> 逐行同构，
    /// 只差 ident（`SM_ATTACK_MISS`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendAttackMiss(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39356-39361
        // 原文 39358-39359
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_ATTACK_MISS, ProcessMsg.BaseObject,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2);
        // 原文 39360
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 39363-39367  ServerSendInputMObileVerifyCode
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendInputMObileVerifyCode(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39363-39367`）。★ **无守卫**（原文如此）。
    /// `:39365` 的 `nRecog` 取的是全局 `g_SendSMSConfig.VerifySendInterval`。</summary>
    public void ServerSendInputMObileVerifyCode(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39363-39367
        // 原文 39365
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_INPUTMOBILE_VerifyCode,
            PlayerSurfaceServerSend2Seams.GetSmsVerifySendInterval(), 0, 0, 0);
        // 原文 39366
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 39369-39403  ServerSendVerifyCode —— 留痕
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendVerifyCode(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39369-39403`，**35 行**）。
    /// <para><b>未移植（显式留痕）</b>。缺失成员（原文行号）：
    /// <c>TBitmap</c>/<c>Bitmap.Canvas.Lock</c>/<c>Font.Name</c>/<c>PixelFormat := pf16bit</c>
    /// （:39376-39384 —— VCL 图形栈，`GXX.M2Server` 侧不存在，且**不能**用
    /// `System.Drawing` 近似：会改变"像素格式/字体度量"）；
    /// <c>MakeVerifyCode(...)</c>（:39385，`M2Share.pas` 的验证码绘制，尚未移植）；
    /// <c>TMemoryStream</c> + <c>Bitmap.SaveToStream</c>（:39386-39388）；
    /// <c>zLibCompressBuffer</c>（:39389）。整段又被 `try ... except end` 吞异常（:39375/:39401-39402），
    /// 即"任何一步失败都静默"—— 用近似实现会**改变这条静默路径的触发点**。
    /// 故按任务书第 4 条整条留痕。</para>
    /// </summary>
    public void ServerSendVerifyCode(TProcessMessage ProcessMsg, ref bool boResult)
    {
        PortNotPorted(nameof(ServerSendVerifyCode), 39369);
    }

    // ==================================================================
    // 39405-39412  ServerSendOpenUrl
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendOpenUrl(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39405-39412`）。★ **无守卫**（原文如此）。
    /// `:39409` 先 `EncodeString`，`:39410` 的 `nRecog` 取的是**编码后字节长度**。</summary>
    public void ServerSendOpenUrl(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39405-39412
        // 原文 39409：sSendMsg := EncodeString(ProcessMsg.sMsg);
        byte[] sSendMsg = EDcode.EncodeString(ProcessMsg.sMsg);
        // 原文 39410
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_OPEN_URL, sSendMsg.Length, 0, 0, 0);
        // 原文 39411
        SendSocketRef(m_DefMsg, Encoding.Latin1.GetString(sSendMsg));
    }

    // ==================================================================
    // 39414-39419  ServerSendOpenGuardianLevelDlg
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendOpenGuardianLevelDlg(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39414-39419`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendOpenGuardianLevelDlg(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39414-39419
        // 原文 39416-39417
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_OpenGuardianLevelDlg, ProcessMsg.nParam1,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam2, (int)ProcessMsg.nParam3);
        // 原文 39418
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39421-39426  ServerSendGuardianLevelBatchInfo
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendGuardianLevelBatchInfo(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39421-39426`）。与上一条逐行同构，只差 ident。</summary>
    public void ServerSendGuardianLevelBatchInfo(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39421-39426
        // 原文 39423-39424
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_GuardianLevelBatchInfo, ProcessMsg.nParam1,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam2, (int)ProcessMsg.nParam3);
        // 原文 39425
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39428-39433  ServerSendGuardianLevelResult
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendGuardianLevelResult(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39428-39433`）。与上一条逐行同构，只差 ident。</summary>
    public void ServerSendGuardianLevelResult(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39428-39433
        // 原文 39430-39431
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_GuardianLevelResult, ProcessMsg.nParam1,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam2, (int)ProcessMsg.nParam3);
        // 原文 39432
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39435-39440  ServerSendBrokenShield
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendBrokenShield(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39435-39440`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendBrokenShield(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39435-39440
        // 原文 39437-39438
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_BrokenShield, ProcessMsg.BaseObject,
            SS2LoWord(ProcessMsg.wParam), SS2HiWord(ProcessMsg.wParam), (int)ProcessMsg.wParam);
        // 原文 39439
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 39442-39446  ServerSendPoisonStruckHum
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendPoisonStruckHum(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39442-39446`）。★ **无守卫**（原文如此）。
    /// ★ 原文如此：`nRecog` 取 `nParam1`（**不是** `BaseObject`）。</summary>
    public void ServerSendPoisonStruckHum(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39442-39446
        // 原文 39444
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_POISON_STRUCK_HUM, ProcessMsg.nParam1,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam2, (int)ProcessMsg.nParam3);
        // 原文 39445
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 39448-39452  ServerSendHumsBBChange
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendHumsBBChange(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39448-39452`）。与 <see cref="ServerSendPoisonStruckHum"/> 逐行同构，
    /// 只差 ident（`SM_HUMS_BB_CHANGE`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendHumsBBChange(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39448-39452
        // 原文 39450
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_HUMS_BB_CHANGE, ProcessMsg.nParam1,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam2, (int)ProcessMsg.nParam3);
        // 原文 39451
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 39454-39459  ServerSendShowCustomButton
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendShowCustomButton(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39454-39459`）。★ **无守卫**（原文如此）。
    /// ★ 原文如此：`:39458` 的尾数据是 `''`，注释 `// EncodeString(ProcessMsg.sMsg)` 是**注释掉的**。</summary>
    public void ServerSendShowCustomButton(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39454-39459
        // 原文 39456-39457
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_SHOW_CUSTOM_BUTTON, ProcessMsg.nParam1,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam2, (int)ProcessMsg.nParam3);
        // 原文 39458：SendSocket(@m_DefMsg, ''); // EncodeString(ProcessMsg.sMsg)
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 39461-39467  ServerSendMagicHintMsg
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendMagicHintMsg(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39461-39467`）。★ **无 `&lt;&gt; Self` 守卫**，但有
    /// `:39463 if Length(ProcessMsg.sMsg) = 0 then Exit;`（**空串早退**）。</summary>
    public void ServerSendMagicHintMsg(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39461-39467
        // 原文 39463-39464
        if (ProcessMsg.sMsg.Length == 0) return;

        // 原文 39465
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_MAGIC_HINT_MSG, ProcessMsg.nParam1,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam2, (int)ProcessMsg.nParam3);
        // 原文 39466
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39469-39474  ServerSendStruckEffect
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendStruckEffect(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39469-39474`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendStruckEffect(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39469-39474
        // 原文 39471-39472
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_STRUCKEFFECT, ProcessMsg.BaseObject,
            (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2, (int)ProcessMsg.nParam3);
        // 原文 39473
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 39476-39485  ServerSendAddDlg
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendAddDlg(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39476-39485`）。★ **无守卫**（原文如此）。
    /// `:39480` 的 `nRecog` 取 `ProcessMsg.wParam`（**不是** `BaseObject`）；
    /// `:39482-39483` 用 `TShortMessage`（`Ident = LoWord(nParam2)`、
    /// `wMsg = HiWord(nParam2)`），尾数据 = `EncodeBuffer(@ShortMessage) + EncodeString(sMsg)`。</summary>
    public void ServerSendAddDlg(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39476-39485
        // 原文 39480-39481
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_ADDDLG, ProcessMsg.wParam,
            SS2LoWord(ProcessMsg.nParam1), SS2HiWord(ProcessMsg.nParam1), (int)ProcessMsg.nParam3);

        // 原文 39482-39483
        TShortMessage shortMessage = default;
        shortMessage.Ident = SS2LoWord(ProcessMsg.nParam2);
        shortMessage.wMsg = SS2HiWord(ProcessMsg.nParam2);

        // 原文 39484：EncodeBuffer(@ShortMessage, SizeOf(TShortMessage)) + EncodeString(ProcessMsg.sMsg)
        byte[] msg = SS2ConcatBytes(
            PlayerSurfaceServerSend2Structs.ShortMessageBytes(shortMessage),
            EDcode.EncodeString(ProcessMsg.sMsg));
        SendSocketRef(m_DefMsg, Encoding.Latin1.GetString(msg));
    }

    // ==================================================================
    // 39487-39491  ServerSendDelDlg
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendDelDlg(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39487-39491`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendDelDlg(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39487-39491
        // 原文 39489
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_DELDLG, ProcessMsg.BaseObject, (int)ProcessMsg.nParam1, 0, 0);
        // 原文 39490
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 39493-39502  ServerSendAddEffectPlay
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendAddEffectPlay(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39493-39502`）。★ **无守卫**（原文如此）。
    ///
    /// `:39499-39500` 填 `TMessageBodyL` 的两个 `int`：`lParam1 := nParam1`、
    /// `lParam2 := StrToIntDef(ProcessMsg.sMsg, 0)`（**把 sMsg 当整数解析**，
    /// 解析失败取 0）。</summary>
    public void ServerSendAddEffectPlay(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39493-39502
        // 原文 39497-39498
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_ADD_EFFECT_PLAY, ProcessMsg.BaseObject,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam2, (int)ProcessMsg.nParam3);

        // 原文 39499-39500
        TMessageBodyL addData = default;
        addData.lParam1 = (int)ProcessMsg.nParam1;
        addData.lParam2 = DelphiRTL.StrToIntDef(ProcessMsg.sMsg, 0);

        // 原文 39501
        SendSocketExRef(m_DefMsg, PlayerSurfaceServerSend2Structs.MessageBodyLBytes(addData));
    }

    // ==================================================================
    // 39504-39507  ServerSendProviewMonItem
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendProviewMonItem(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39504-39507`）—— 整条只有
    /// `SendPreviewMonItem(TBaseObject(ProcessMsg.BaseObject));`。
    /// ★ **无守卫**（原文如此）。</summary>
    public void ServerSendProviewMonItem(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39504-39507
        // 原文 39506
        PlayerSurfaceServerSend2Seams.SendPreviewMonItem(this, ProcessMsg.BaseObject);
    }

    // ==================================================================
    // 39509-39538  ServerSendWeather —— 留痕
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendWeather(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39509-39538`，**30 行**）。
    /// <para><b>未移植（显式留痕）</b>。缺失成员（原文行号）：
    /// <c>m_PEnvir.m_WeatherEffect</c>（:39521-39533，`TEnvirnoment` 的天气特效定长数组，
    /// 元素类型 <c>TServerWeateherEffect</c>）；<c>MAX_MAP_WEATEHER_EFFECT</c>（:39514）；
    /// <c>SizeOf(TServerWeateherEffect)</c>（:39526/:39528）。
    /// 托管侧这三者**都不存在**，且 :39514 的 `InBuf: array[0 .. MAX_MAP_WEATEHER_EFFECT *
    /// SizeOf(TServerWeateherEffect)] of AnsiChar` 与 :39527-39532 的**手写字节游标**
    /// （每项先写 1 字节下标、再写整条记录、每项多算 1 字节）无法在不臆造该结构体
    /// 布局的前提下复刻 —— 一旦布局猜错，**报文长度与内容都会错**，
    /// 而 `SendSocketEx` 的长度参数正是取自该游标。
    /// 故按任务书第 4 条整条留痕。</para>
    /// </summary>
    public void ServerSendWeather(TProcessMessage ProcessMsg, ref bool boResult)
    {
        PortNotPorted(nameof(ServerSendWeather), 39509);
    }

    // ==================================================================
    // 39540-39543  ServerSendHearColor
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendHearColor(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39540-39543`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendHearColor(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39540-39543
        // 原文 39542
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_HEARCOLOR,
            ProcessMsg.nParam1, (ushort)ProcessMsg.wParam, 0, 0, "");
    }

    // ==================================================================
    // 39545-39548  ServerSendOpenPlayDrink
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendOpenPlayDrink(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39545-39548`）。★ **无守卫**（原文如此）。
    /// ★ 原文如此：`nRecog`/`nParam` 都传**字面量 0**，只取 `nParam2`/`nParam3`。</summary>
    public void ServerSendOpenPlayDrink(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39545-39548
        // 原文 39547
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_OPENPLAYDRINK,
            0, 0, (ushort)ProcessMsg.nParam2, (ushort)ProcessMsg.nParam3, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39550-39553  ServerSendCloseDrink
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendCloseDrink(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39550-39553`）。★ **无守卫**（原文如此）。全 0 参数。</summary>
    public void ServerSendCloseDrink(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39550-39553
        // 原文 39552
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_CLOSEDRINK,
            0, 0, 0, 0, "");
    }

    // ==================================================================
    // 39555-39558  ServerSendDrinkUpdateValue
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendDrinkUpdateValue(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39555-39558`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendDrinkUpdateValue(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39555-39558
        // 原文 39557
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_DRINKUPDATEVALUE,
            ProcessMsg.nParam1, (ushort)ProcessMsg.wParam, (ushort)ProcessMsg.nParam2,
            (ushort)ProcessMsg.nParam3, "");
    }

    // ==================================================================
    // 39560-39563  ServerSendPlayDrinkToDrink
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendPlayDrinkToDrink(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39560-39563`）。★ **无守卫**（原文如此）。
    /// ★ 原文如此：`nRecog` 传**字面量 0**，`nParam` 取 `nParam1`。</summary>
    public void ServerSendPlayDrinkToDrink(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39560-39563
        // 原文 39562
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_PLAYDRINKTODRINK,
            0, (ushort)ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2, (ushort)ProcessMsg.nParam3, "");
    }

    // ==================================================================
    // 39565-39568  ServerSendUserPlayDrink
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendUserPlayDrink(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39565-39568`）。★ **无守卫**（原文如此）。
    /// ★ 原文如此：`nParam` 取 `nParam1`（**不是** `BaseObject`）。</summary>
    public void ServerSendUserPlayDrink(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39565-39568
        // 原文 39567
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_SENDUSERPLAYDRINK,
            ProcessMsg.nParam1, 0, 0, 0, "");
    }

    // ==================================================================
    // 39570-39574  ServerSendStorageHeroInfo
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendStorageHeroInfo(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39570-39574`）。★ **无守卫**（原文如此）。
    /// ★ 原文如此：`nRecog` 取 `nParam1`（**不是** `BaseObject`）。</summary>
    public void ServerSendStorageHeroInfo(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39570-39574
        // 原文 39572
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_SENDSTORAGEHEROINFO, ProcessMsg.nParam1, 0, 0, 0);
        // 原文 39573
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39576-39579  ServerSendStorageHeroInfoEx
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendStorageHeroInfoEx(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39576-39579`）—— 整条只有
    /// `ClientQueryAssessHero(ProcessMsg.sMsg);`。★ **无守卫**（原文如此）。
    /// 托管 `ClientQueryAssessHero` 已由 Core1.cs:2122 移植。</summary>
    public void ServerSendStorageHeroInfoEx(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39576-39579
        // 原文 39578
        ClientQueryAssessHero(ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39581-39584  ServerSendShowHeroAutoPracticeDlg
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendShowHeroAutoPracticeDlg(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39581-39584`）。★ **无守卫**（原文如此）。全 0 参数。</summary>
    public void ServerSendShowHeroAutoPracticeDlg(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39581-39584
        // 原文 39583
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_SENDSHOWHEROAUTOPRACTICEDLG,
            0, 0, 0, 0, "");
    }

    // ==================================================================
    // 39586-39589  ServerSendRefAbilityNG
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendRefAbilityNG(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39586-39589`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendRefAbilityNG(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39586-39589
        // 原文 39588
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_REFABILNG,
            ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2, (ushort)ProcessMsg.nParam3, 0, "");
    }

    // ==================================================================
    // 39591-39604  ServerSendAbilityNG
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendAbilityNG(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39591-39604`）。★ **无守卫**（原文如此）。
    ///
    /// `:39595-39601` 把自身 `m_AbilNG` 的 5 个字段 + `GetNGAddPower`/`GetNGDecPower`
    /// 拷进 `TClientAbilityNG`（**多出 NGDamage/UnNGDamage 两字段**），整块 `SendSocketEx`。</summary>
    public void ServerSendAbilityNG(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39591-39604
        // 原文 39595-39601
        TClientAbilityNG clientAbilityNG = default;
        clientAbilityNG.Level = m_AbilNG.Level;
        clientAbilityNG.NH = m_AbilNG.NH;
        clientAbilityNG.MaxNH = m_AbilNG.MaxNH;
        clientAbilityNG.Exp = m_AbilNG.Exp;
        clientAbilityNG.MaxExp = m_AbilNG.MaxExp;
        clientAbilityNG.NGDamage = PlayerSurfaceServerSend2Seams.GetNGAddPower(this);
        clientAbilityNG.UnNGDamage = PlayerSurfaceServerSend2Seams.GetNGDecPower(this);

        // 原文 39602
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_ABILITYNG, 0, 0, 0, 0);
        // 原文 39603
        SendSocketExRef(m_DefMsg, PlayerSurfaceServerSend2Structs.ClientAbilityNGBytes(clientAbilityNG));
    }

    // ==================================================================
    // 39606-39610  ServerSendAbilityAlcohol
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendAbilityAlcohol(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39606-39610`）。★ **无守卫**（原文如此）。
    /// 整块 `SendSocketEx(@m_Alcohol, SizeOf(TAbilityAlcohol))`。</summary>
    public void ServerSendAbilityAlcohol(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39606-39610
        // 原文 39608
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_ABILITYALCOHOL, 0, 0, 0, 0);
        // 原文 39609
        SendSocketExRef(m_DefMsg, PlayerSurfaceServerSend2Structs.AbilityAlcoholBytes(m_Alcohol));
    }

    // ==================================================================
    // 39612-39616  ServerSendAbilityMeridians
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendAbilityMeridians(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39612-39616`）。★ **无守卫**（原文如此）。
    /// 整块 `SendSocketEx(@m_HumMeridians, SizeOf(THumMeridians))`。
    /// ⚠ 托管侧 `THumMeridians` **无类型定义** ⇒ `m_HumMeridians` 退化为 `byte[]`
    /// （字段声明处已注明），此处直接下发该字节块（长度即原文 `SizeOf`）。</summary>
    public void ServerSendAbilityMeridians(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39612-39616
        // 原文 39614
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_ABILITYMERIDIANS, 0, 0, 0, 0);
        // 原文 39615
        SendSocketExRef(m_DefMsg, m_HumMeridians);
    }

    // ==================================================================
    // 39618-39620  ServerSendWinExpNG —— 原文空实现
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendWinExpNG(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39618-39620`）—— **原文整条为空体**（`begin end;`），
    /// 无守卫、无语句。忠实移植 = **空体**（不是留痕桩）。</summary>
    public void ServerSendWinExpNG(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39618-39620：空实现（原文 `begin` / `end;` 之间**没有任何语句**）
    }

    // ==================================================================
    // 39622-39627  ServerSendLevelUpNG
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendLevelUpNG(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39622-39627`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendLevelUpNG(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39622-39627
        // 原文 39624-39625
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_LEVELUPNG, ProcessMsg.BaseObject,
            (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2, (int)ProcessMsg.wParam);
        // 原文 39626
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 39629-39632  ServerSendOpenCobWebWinding
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendOpenCobWebWinding(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39629-39632`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendOpenCobWebWinding(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39629-39632
        // 原文 39631
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_OPENCOBWEBWINDING,
            ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2, 0, 0, "");
    }

    // ==================================================================
    // 39634-39637  ServerSendCloseCobWebWinding
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendCloseCobWebWinding(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39634-39637`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendCloseCobWebWinding(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39634-39637
        // 原文 39636
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_CLOSECOBWEBWINDING,
            ProcessMsg.nParam1, 0, 0, 0, "");
    }

    // ==================================================================
    // 39639-39642  ServerSendOpenToxicsMoke
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendOpenToxicsMoke(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39639-39642`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendOpenToxicsMoke(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39639-39642
        // 原文 39641
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_OPENTOXICSMOKE,
            ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2, 0, 0, "");
    }

    // ==================================================================
    // 39644-39647  ServerSendCloseToxicsMoke
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendCloseToxicsMoke(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39644-39647`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendCloseToxicsMoke(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39644-39647
        // 原文 39646
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_CLOSETOXICSMOKE,
            ProcessMsg.nParam1, 0, 0, 0, "");
    }

    // ==================================================================
    // 39649-39660  ServerSendItemDescList
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendItemDescList(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39649-39660`）。★ 自带两道守卫（原文如此）。
    ///
    /// ★ 原文如此（**不修**）：`:39655` 的 `if Length(g_ItemDescListText) &gt; 0` 是
    /// "文本非空才发"，即守卫顺序为 假人 → 挂机超时 → 文本非空；
    /// `:39657` 的 `nRecog` 取的是**文本长度**。</summary>
    public void ServerSendItemDescList(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39649-39660
        // 原文 39651-39652
        if (m_boDummyObject) return;
        // 原文 39653-39654
        if (m_boOffLine && unchecked(PlayerSurfaceServerSendSeams.MyGetTickCount() - m_nOffOnlineTick) > 1000 * 30)
            return;

        // 原文 39655-39659
        string text = PlayerSurfaceServerSend2Globals.g_ItemDescListText;
        if (text.Length > 0)
        {
            // 原文 39657
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_SENDITEMDESCLIST, text.Length, 0, 0, 0);
            // 原文 39658
            SendSocketRef(m_DefMsg, text);
        }
    }

    // ==================================================================
    // 39662-39673  ServerSendItemDescTopList
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendItemDescTopList(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39662-39673`）。与 <see cref="ServerSendItemDescList"/> **逐行同构**，
    /// 只差全局变量（`g_ItemDescTopListText`）与 ident（`SM_SENDITEMDESCTOPLIST`）。</summary>
    public void ServerSendItemDescTopList(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39662-39673
        // 原文 39664-39665
        if (m_boDummyObject) return;
        // 原文 39666-39667
        if (m_boOffLine && unchecked(PlayerSurfaceServerSendSeams.MyGetTickCount() - m_nOffOnlineTick) > 1000 * 30)
            return;

        // 原文 39668-39672
        string text = PlayerSurfaceServerSend2Globals.g_ItemDescTopListText;
        if (text.Length > 0)
        {
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_SENDITEMDESCTOPLIST, text.Length, 0, 0, 0);
            SendSocketRef(m_DefMsg, text);
        }
    }

    // ==================================================================
    // 39675-39683  ServerSendTzItemDescList
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendTzItemDescList(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39675-39683`）。★ 自带两道守卫（原文如此）。
    /// ★ 与 <see cref="ServerSendItemDescList"/> 的差别：**没有** `Length(...) &gt; 0` 判定，
    /// 文本为空也照发（`:39681-39682` 无条件）。</summary>
    public void ServerSendTzItemDescList(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39675-39683
        // 原文 39677-39678
        if (m_boDummyObject) return;
        // 原文 39679-39680
        if (m_boOffLine && unchecked(PlayerSurfaceServerSendSeams.MyGetTickCount() - m_nOffOnlineTick) > 1000 * 30)
            return;

        // 原文 39681
        string text = PlayerSurfaceServerSend2Globals.g_TzItemDescListText;
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_SENDTZITEMDESCLIST, text.Length, 0, 0, 0);
        // 原文 39682
        SendSocketRef(m_DefMsg, text);
    }

    // ==================================================================
    // 39685-39693  ServerSendFilterItemList
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendFilterItemList(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39685-39693`）。★ 自带两道守卫（原文如此）。
    /// 与 <see cref="ServerSendTzItemDescList"/> 逐行同构，只差全局变量与 ident。</summary>
    public void ServerSendFilterItemList(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39685-39693
        // 原文 39687-39688
        if (m_boDummyObject) return;
        // 原文 39689-39690
        if (m_boOffLine && unchecked(PlayerSurfaceServerSendSeams.MyGetTickCount() - m_nOffOnlineTick) > 1000 * 30)
            return;

        // 原文 39691
        string text = PlayerSurfaceServerSend2Globals.g_NameFilterListText;
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_SENDFILTERITEMLIST, text.Length, 0, 0, 0);
        // 原文 39692
        SendSocketRef(m_DefMsg, text);
    }

    // ==================================================================
    // 39695-39703  ServerSendPlayMagicBallEffect
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendPlayMagicBallEffect(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39695-39703`）。★ 自带两道守卫（原文如此）。
    /// ★ 原文如此：`MakeDefaultMsg` 的 `nRecog` **写死 0**（不取 `BaseObject`）。</summary>
    public void ServerSendPlayMagicBallEffect(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39695-39703
        // 原文 39697-39698
        if (m_boDummyObject) return;
        // 原文 39699-39700
        if (m_boOffLine && unchecked(PlayerSurfaceServerSendSeams.MyGetTickCount() - m_nOffOnlineTick) > 1000 * 30)
            return;

        // 原文 39701
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_PLAYMAGICBALLEFFECT, 0, 0, 0, 0);
        // 原文 39702
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39705-39726  ServerSendLightingEx
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendLightingEx(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39705-39726`）。★ **无 `&lt;&gt; Self` 守卫**，但整条被
    /// `:39710 if TBaseObject(ProcessMsg.nParam3) &lt;&gt; nil then` 包住。
    ///
    /// <code>
    ///   if TBaseObject(ProcessMsg.nParam3) &lt;&gt; nil then                     // 39710
    ///   begin
    ///     MessageBodyWL.lParam1 := TBaseObject(ProcessMsg.nParam3).m_nCurrX;   // 39712
    ///     MessageBodyWL.lParam2 := TBaseObject(ProcessMsg.nParam3).m_nCurrY;   // 39713
    ///     MessageBodyWL.lTag2   := ProcessMsg.nParam3;                        // 39714
    ///     Magic := UserEngine.FindMagic(ProcessMsg.wParam);                   // 39715
    ///     if Magic &lt;&gt; nil then                                              // 39716
    ///       MessageBodyWL.lTag1 := MakeLong(Magic.wMagicId, MakeWord(Magic.btEffectType, Magic.btEffect))
    ///     else
    ///       MessageBodyWL.lTag1 := MakeLong(ProcessMsg.wParam, 0);            // 39721
    ///     m_DefMsg := MakeDefaultMsg(SM_LIGHTINGEX, NativeInt(BaseObject), nParam1, nParam2,
    ///                                TBaseObject(BaseObject).m_btDirection);  // 39722-39723
    ///     SendSocketEx(@m_DefMsg, @MessageBodyWL, SizeOf(TMessageBodyWL));    // 39724
    ///   end;
    /// </code>
    /// ⚠ `MessageBodyWL.lParam1/lParam2` 在托管侧是 **`int`**（`Grobal2.Types1.cs:146`），
    /// 而原文 `TMessageBodyWL.lParam1/lParam2` 是 `Integer` —— 一致，无需转换。
    /// ⚠ `FindMagic(wMagicId)`（`TUserEngine`）在托管侧未移植 ⇒ 走
    /// <see cref="PlayerSurfaceServerSend2Seams.FindMagicEffect"/>（返回 bool + 出参
    /// `wMagicId`/`btEffectType`/`btEffect`）；**未接线时按原文 `Magic = nil` 分支**，
    /// 即 `MakeLong(ProcessMsg.wParam, 0)`。</summary>
    public void ServerSendLightingEx(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39705-39726
        // 原文 39710
        if (ProcessMsg.nParam3 != 0)
        {
            TMessageBodyWL body = default;
            // 原文 39712-39714
            body.lParam1 = PlayerSurfaceServerSend2Seams.GetCurrXOf(ProcessMsg.nParam3);
            body.lParam2 = PlayerSurfaceServerSend2Seams.GetCurrYOf(ProcessMsg.nParam3);
            body.lTag2 = (long)ProcessMsg.nParam3;

            // 原文 39715-39721
            bool found = PlayerSurfaceServerSend2Seams.FindMagicEffect(
                (int)ProcessMsg.wParam, out int magicId, out int effectType, out int effect);
            if (found)
                body.lTag1 = unchecked((int)PlayerSurfacePack.MakeLong(
                    magicId, PlayerSurfacePack.MakeWord(effectType, effect)));
            else
                body.lTag1 = unchecked((int)PlayerSurfacePack.MakeLong((int)ProcessMsg.wParam, 0));

            // 原文 39722-39723
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_LIGHTINGEX, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                PlayerSurfaceServerSendSeams.GetDirection(ProcessMsg.BaseObject));

            // 原文 39724
            SendSocketExRef(m_DefMsg, PlayerSurfaceServerSend2Structs.MessageBodyWLBytes(body));
        }
    }

    // ==================================================================
    // 39728-39733  ServerSendContinuousMagicOrder
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendContinuousMagicOrder(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39728-39733`）。★ **无守卫**（原文如此）。
    ///
    /// `:39730-39732` 的参数依次是
    /// `BoolToInt(m_boOpenLastContinuous)` / `MakeWord(m_ContinuousMagicOrder[0], [1])` /
    /// `MakeWord([2], [3])` / `0` —— 即"4 个字节值打包进 2 个 Word"。</summary>
    public void ServerSendContinuousMagicOrder(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39728-39733
        // 原文 39730-39732
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_CONTINUOUSMAGICORDER,
            SS2BoolToInt(m_boOpenLastContinuous),
            PlayerSurfacePack.MakeWord(m_ContinuousMagicOrder[0], m_ContinuousMagicOrder[1]),
            PlayerSurfacePack.MakeWord(m_ContinuousMagicOrder[2], m_ContinuousMagicOrder[3]),
            0, "");
    }

    // ==================================================================
    // 39735-39739  ServerSendContinuousMagicOK
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendContinuousMagicOK(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39735-39739`）。★ **无守卫**（原文如此）。
    /// `:39737` 先置 `m_boContinuous := True`（**状态副作用在发报之前**）。</summary>
    public void ServerSendContinuousMagicOK(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39735-39739
        // 原文 39737
        m_boContinuous = true;
        // 原文 39738
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_CONTINUOUSMAGIC_OK,
            (long)ProcessMsg.wParam, (ushort)ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2, 0, "");
    }

    // ==================================================================
    // 39741-39744  ServerSendContinuousMagicFail
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendContinuousMagicFail(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39741-39744`）。与上一条同构，**但不置 `m_boContinuous`**。
    /// ★ **无守卫**（原文如此）。</summary>
    public void ServerSendContinuousMagicFail(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39741-39744
        // 原文 39743
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_CONTINUOUSMAGIC_FAIL,
            (long)ProcessMsg.wParam, (ushort)ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2, 0, "");
    }

    // ==================================================================
    // 39746-39749  ServerSendTraingNG
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendTraingNG(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39746-39749`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendTraingNG(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39746-39749
        // 原文 39748
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_TRAININGNG,
            (long)ProcessMsg.wParam, (ushort)ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2, 0, "");
    }

    // ==================================================================
    // 39751-39756  ServerSendStopContinuousMagic
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendStopContinuousMagic(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39751-39756`）。★ **无守卫**（原文如此）。
    /// `:39753-39754` **两个状态位都先复位**，再发报（注释解释了原因：报文有延时，
    /// 真正下发时 `m_boContinuous` 可能已被重新置 True）。</summary>
    public void ServerSendStopContinuousMagic(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39751-39756
        // 原文 39753：m_boContinuous := False;
        //   // 这个消息有延时，可能真正发消息到客户端的时候，m_boContinuous又搞成了True 2020-01-19
        m_boContinuous = false;
        // 原文 39754
        m_boSendCanUseContinuous = false;
        // 原文 39755
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_STOPCONTINUOUSMAGIC,
            0, 0, 0, 0, "");
    }

    // ==================================================================
    // 39758-39761  ServerSendShopName
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendShopName(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39758-39761`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendShopName(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39758-39761
        // 原文 39760
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_SENDSHOPNAME,
            ProcessMsg.nParam1, 0, 0, 0, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39763-39767  ServerSendHeroM2DressEffect
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendHeroM2DressEffect(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39763-39767`）。★ **无守卫**（原文如此）。
    /// ★ 原文如此：`nParam`/`nTag` 拆的是 **`nParam1`**（不是 `wParam`）。</summary>
    public void ServerSendHeroM2DressEffect(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39763-39767
        // 原文 39765-39766
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_HEROM2SENDDRESSEFFECT,
            (long)ProcessMsg.BaseObject,
            SS2LoWord(ProcessMsg.nParam1), SS2HiWord(ProcessMsg.nParam1), 0, "");
    }

    // ==================================================================
    // 39769-39778  ServerSendIncHealth
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendIncHealth(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39769-39778`）。★ **无守卫**（原文如此）。
    ///
    /// <code>
    ///   Int64Value := Int64(m_WAbil.HP) + LongWord(ProcessMsg.nParam1);   // 39773
    ///   m_WAbil.HP := Min(Int64Value, m_WAbil.MaxHP);                     // 39774
    ///   Int64Value := Int64(m_WAbil.MP) + LongWord(ProcessMsg.nParam2);   // 39775
    ///   m_WAbil.MP := Min(Int64Value, m_WAbil.MaxMP);                     // 39776
    ///   HealthSpellChanged();                                             // 39777
    /// </code>
    /// ★ 原文如此（**不修**）：`:39773` 的 `LongWord(ProcessMsg.nParam1)` 把**有符号**参数
    /// **无符号**解释 —— 传负数会先变成 ~4.29e9，再经 `Min(..., MaxHP)` 夹回 `MaxHP`。
    /// 托管侧逐字复刻：`(long)(uint)(int)ProcessMsg.nParam1`。
    /// ⚠ `m_WAbil.HP/MP/MaxHP/MaxMP` 是 `Word`（`Grobal2` 的 `TAbility`）⇒
    /// 赋值时的窄化**与原文一致**（`Min` 的结果可能超 `Word` 吗？原文先 `Min` 到
    /// `MaxHP`/`MaxMP`（也是 `Word`）故不会；本片保持同一顺序）。</summary>
    public void ServerSendIncHealth(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39769-39778
        // 原文 39773-39774
        long int64Value = (long)m_WAbil.HP + (long)(uint)(int)ProcessMsg.nParam1;
        m_WAbil.HP = (ushort)SS2MinInt64(int64Value, m_WAbil.MaxHP);

        // 原文 39775-39776
        int64Value = (long)m_WAbil.MP + (long)(uint)(int)ProcessMsg.nParam2;
        m_WAbil.MP = (ushort)SS2MinInt64(int64Value, m_WAbil.MaxMP);

        // 原文 39777
        PlayerSurfaceServerSend2Seams.HealthSpellChanged(this);
    }

    // ==================================================================
    // 39780-39814  ServerSendAttack01..06
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendAttack01(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39780-39784`）。★ **无守卫**（原文如此）。
    /// ★ 原文如此：`nRecog` 传 `NativeInt(BaseObject)`，`nParam`/`nTag` 取
    /// `nParam1`/`nParam2`，`nSeries` 取 `wParam`。</summary>
    public void ServerSendAttack01(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39780-39784
        // 原文 39782-39783
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_ATTACK01,
            (long)ProcessMsg.BaseObject, (ushort)ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2,
            (ushort)ProcessMsg.wParam, ProcessMsg.sMsg);
    }

    /// <summary>原文 `procedure TPlayObject.ServerSendAttack02(...)`（`ObjPlayer.pas:39786-39790`）。
    /// 与 <see cref="ServerSendAttack01"/> 逐行同构，只差 ident。</summary>
    public void ServerSendAttack02(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39786-39790
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_ATTACK02,
            (long)ProcessMsg.BaseObject, (ushort)ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2,
            (ushort)ProcessMsg.wParam, ProcessMsg.sMsg);
    }

    /// <summary>原文 `procedure TPlayObject.ServerSendAttack03(...)`（`ObjPlayer.pas:39792-39796`）。
    /// 与 <see cref="ServerSendAttack01"/> 逐行同构，只差 ident。</summary>
    public void ServerSendAttack03(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39792-39796
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_ATTACK03,
            (long)ProcessMsg.BaseObject, (ushort)ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2,
            (ushort)ProcessMsg.wParam, ProcessMsg.sMsg);
    }

    /// <summary>原文 `procedure TPlayObject.ServerSendAttack04(...)`（`ObjPlayer.pas:39798-39802`）。
    /// 与 <see cref="ServerSendAttack01"/> 逐行同构，只差 ident。</summary>
    public void ServerSendAttack04(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39798-39802
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_ATTACK04,
            (long)ProcessMsg.BaseObject, (ushort)ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2,
            (ushort)ProcessMsg.wParam, ProcessMsg.sMsg);
    }

    /// <summary>原文 `procedure TPlayObject.ServerSendAttack05(...)`（`ObjPlayer.pas:39804-39808`）。
    /// 与 <see cref="ServerSendAttack01"/> 逐行同构，只差 ident。</summary>
    public void ServerSendAttack05(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39804-39808
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_ATTACK05,
            (long)ProcessMsg.BaseObject, (ushort)ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2,
            (ushort)ProcessMsg.wParam, ProcessMsg.sMsg);
    }

    /// <summary>原文 `procedure TPlayObject.ServerSendAttack06(...)`（`ObjPlayer.pas:39810-39814`）。
    /// 与 <see cref="ServerSendAttack01"/> 逐行同构，只差 ident。</summary>
    public void ServerSendAttack06(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39810-39814
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_ATTACK06,
            (long)ProcessMsg.BaseObject, (ushort)ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2,
            (ushort)ProcessMsg.wParam, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39816-39820  ServerSendOpenGameShop
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendOpenGameShop(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39816-39820`）。与 <see cref="ServerSendAttack01"/> 逐行同构，
    /// 只差 ident（`SM_OPENGAMESHOP`）。★ **无守卫**（原文如此）。</summary>
    public void ServerSendOpenGameShop(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39816-39820
        // 原文 39818-39819
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_OPENGAMESHOP,
            (long)ProcessMsg.BaseObject, (ushort)ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2,
            (ushort)ProcessMsg.wParam, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39823-39827  ServerSendArmRemoveStone
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendArmRemoveStone(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39823-39827`；前置注释 `// 卸下宝石 chongchong 2015-01-04`）。
    /// 与 <see cref="ServerSendAttack01"/> 逐行同构，只差 ident（`SM_ARMREMOVESTONE`）。</summary>
    public void ServerSendArmRemoveStone(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39822：// 卸下宝石 chongchong 2015-01-04
        // 原文 39823-39827
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_ARMREMOVESTONE,
            (long)ProcessMsg.BaseObject, (ushort)ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2,
            (ushort)ProcessMsg.wParam, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39829-39833  ServerSendSetNpcImage
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendSetNpcImage(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39829-39833`）。★ **无守卫**（原文如此）。
    /// ★ 原文如此：5 个参数全取 `wParam`/`nParam1..3`（**不用 `BaseObject`**）。</summary>
    public void ServerSendSetNpcImage(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39829-39833
        // 原文 39831
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_SETNPCIMAGE, (int)ProcessMsg.wParam,
            (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2, (int)ProcessMsg.nParam3);
        // 原文 39832
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 39835-39838  ServerSendUserBigGetBackItem
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendUserBigGetBackItem(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39835-39838`）—— 整条只有
    /// `SendSaveBigStorageItemList(ProcessMsg.nParam1, ProcessMsg.wParam);`。
    /// ★ **无守卫**（原文如此）。注意第二个实参取的是 **`wParam`**（不是 `nParam2`）。</summary>
    public void ServerSendUserBigGetBackItem(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39835-39838
        // 原文 39837
        PlayerSurfaceServerSend2Seams.SendSaveBigStorageItemList(this, (int)ProcessMsg.nParam1, (long)ProcessMsg.wParam);
    }

    // ==================================================================
    // 39840-39845  ServerSendThunderPalsyEff
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendThunderPalsyEff(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39840-39845`）。★ **无守卫**（原文如此）。
    /// ★ 原文如此：拆的是 `wParam`（同三条 `*BLASTHIT`）。</summary>
    public void ServerSendThunderPalsyEff(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39840-39845
        // 原文 39842-39843
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_THUNDERPALSY_EFF, ProcessMsg.BaseObject,
            SS2LoWord(ProcessMsg.wParam), SS2HiWord(ProcessMsg.wParam), (int)ProcessMsg.wParam);
        // 原文 39844
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 39847-39869  ClientAutoGJ
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ClientAutoGJ(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39847-39869`）。★ **无守卫**（原文如此；**注意这是 `Client*` 处理器**）。
    ///
    /// <code>
    ///   case ProcessMsg.wParam of
    ///     0: if m_boAutoOnline then
    ///          g_FunctionNPC.GotoLable(Self, '@StopAutoOnline', False);      // 39852-39853
    ///     1: if not m_boAutoOnline then
    ///        begin
    ///          // 禁止挂机 chongchong 2014-11-26
    ///          if not m_PEnvir.m_boNoAutoOnline then
    ///            g_FunctionNPC.GotoLable(Self, '@StartAutoOnline', False);   // 39862
    ///          else
    ///            SendDefMessage(SM_AUTOPLAYGAME_STATE, 0, 1, 0, 0, '');       // 39865
    ///        end;
    ///   end;
    /// </code>
    /// ★ 关键（**原文如此**）：`case` **没有 `else`** ⇒ 其余 `wParam` 值**什么都不做**；
    /// 且 \(\text{wParam}=0\) 分支**不**置 `m_boAutoOnline := False`（只跳脚本标签）——
    /// 状态翻转由脚本 `@StopAutoOnline` 侧完成。</summary>
    public void ClientAutoGJ(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39847-39869
        // 原文 39849：case ProcessMsg.wParam of
        switch (ProcessMsg.wParam)
        {
            case 0:
                // 原文 39850-39854
                if (m_boAutoOnline)
                    PlayerSurfaceServerSend2Seams.GotoLable(this, "", "@StopAutoOnline");
                break;

            case 1:
                // 原文 39855-39867
                if (!m_boAutoOnline)
                {
                    // 原文 39859：// 禁止挂机 chongchong 2014-11-26
                    // 原文 39860
                    bool noAuto = m_PEnvir != null
                        && PlayerSurfaceServerSend2Seams.GetEnvirNoAutoOnline(m_PEnvir);
                    if (!noAuto)
                    {
                        // 原文 39862
                        PlayerSurfaceServerSend2Seams.GotoLable(this, "", "@StartAutoOnline");
                    }
                    else
                    {
                        // 原文 39865
                        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_AUTOPLAYGAME_STATE,
                            0, 1, 0, 0, "");
                    }
                }
                break;

            // 原文如此：`case` 无 `else` ⇒ 其余取值直接落到 `end;`
        }
    }

    // ==================================================================
    // 39871-39877  ClientGJCallMonMagic
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ClientGJCallMonMagic(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39871-39877`）。★ **无守卫**（原文如此）。
    ///
    /// `:39875` 的 `Ret := Integer(CanUseCallMonMagic(ProcessMsg.wParam))`
    /// —— 布尔→整数（`True → 1`），随后 `:39876` 把它放进 `nParam`
    /// （`wParam` 位仍是 `ProcessMsg.wParam`，注释标明 `MagicID`/`CanUse`）。</summary>
    public void ClientGJCallMonMagic(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39871-39877
        // 原文 39875：Ret := Integer(CanUseCallMonMagic(ProcessMsg.wParam));
        int ret = SS2BoolToInt(
            PlayerSurfaceServerSend2Seams.CanUseCallMonMagic(this, (int)ProcessMsg.wParam));
        // 原文 39876
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_GJ_CALLMONMAGIC,
            (long)ProcessMsg.wParam,   // MagicID
            (ushort)ret,               // CanUse
            0, 0, "");
    }

    // ==================================================================
    // 39880-39949  ClientQuerySelectHeroM2ShopInfo
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ClientQuerySelectHeroM2ShopInfo(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39880-39949`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。
    ///
    /// 逐行（**原文结构完整保留**）：
    /// <list type="number">
    ///   <item><description>:39892-39894 频率门：`MyGetTickCount - m_dwQueryUserShopTime &lt; 100`
    ///     ⇒ `Exit`；否则**先**把 `m_dwQueryUserShopTime := MyGetTickCount`。</description></item>
    ///   <item><description>:39895-39900 `BaseObject := TBaseObject(nParam1)`；非 nil 时用
    ///     `m_PEnvir.IsValidObjectEx(m_nCurrX, m_nCurrY, 10, BaseObject)` 做 10 格内校验，
    ///     失败则**置 nil**（⇒ 后面整段不发）。</description></item>
    ///   <item><description>:39901-39904 只有 `m_btRaceServer = RC_PLAYOBJECT` 才 `as TPlayObject`。</description></item>
    ///   <item><description>:39905 还要 `OnlineObject.m_boShopStall` 为真。</description></item>
    ///   <item><description>:39907 `sText := OnlineObject.m_sCharName + '\';`（**反斜杠**拼接）。</description></item>
    ///   <item><description>:39908-39945 `if OnlineObject &lt;&gt; Self then` 才做"换摊主 + 清单"那一段
    ///     （含 `SameText` 比较、旧摊主 `m_HeroM2ShopList.Remove(Self)`、
    ///     `m_HeroM2ShopOpenList.IndexOf(Self) &lt; 0` 才 `Add`、以及**双层循环**拼物品）。</description></item>
    ///   <item><description>:39946-39947 最后**无条件**（只要前面没 Exit）
    ///     `MakeDefaultMsg(SM_HEROM2SENDSHOPITEM, NativeInt(OnlineObject), 0, 0, 0)` + `SendSocket(sText)`。</description></item>
    /// </list>
    /// ⚠ 原文如此（**不修**）：`:39946` 的 `nRecog` 传的是 **`OnlineObject`（摊主）的指针**，
    ///   而不是 `Self`。
    /// ⚠ 原文如此（**不修**）：:39937-39939 里 `ClientItem` 的三处赋值
    ///   （`S.Price`/`S.Reserved1`/`S.Stock`）**在 `UserItemToClientItem` 只被调用一次的
    ///   分支外**也可能执行 —— 但 :39933 的 `if StdItem &lt;&gt; nil then` 只把
    ///   `UserItemToClientItem` 包住，`ClientItem.S.*` 三行**不在**该 `if` 内，
    ///   故 `StdItem = nil` 时 `ClientItem` 是**上一轮的残留值**（原文如此）。
    ///   本片按原文字面顺序复刻（不做"提前 continue"之类改写）。
    /// </list>
    /// ⚠ 缺失依赖（已接缝化，见方法内注释）：`UserEngine.GetStdItem`、
    ///   `UserItemToClientItem`、`EncodeBuffer(@ClientItem)`、`SameText`、
    ///   `UserEngine.GetPlayObject(string)`。
    /// </summary>
    public void ClientQuerySelectHeroM2ShopInfo(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39880-39949
        // 原文 39892-39893：if MyGetTickCount - m_dwQueryUserShopTime < 100 then Exit;
        uint tick = PlayerSurfaceServerSendSeams.MyGetTickCount();
        if (unchecked(tick - PlayerSurfaceServerSend2Seams.GetQueryUserShopTime(this)) < 100)
            return;
        // 原文 39894：m_dwQueryUserShopTime := MyGetTickCount;
        PlayerSurfaceServerSend2Seams.SetQueryUserShopTime(this, tick);

        // 原文 39895：BaseObject := TBaseObject(ProcessMsg.nParam1);
        nint baseObject = ProcessMsg.nParam1;

        // 原文 39896-39900
        if (baseObject != 0)
        {
            // 原文 39898：if not m_PEnvir.IsValidObjectEx(m_nCurrX, m_nCurrY, 10, BaseObject) then
            bool valid = m_PEnvir != null
                && PlayerSurfaceServerSend2Seams.IsValidObjectEx(m_PEnvir, m_nCurrX, m_nCurrY, 10, baseObject);
            if (!valid)
                baseObject = 0;   // 原文 39899：BaseObject := nil;
        }

        // 原文 39901-39904：if (BaseObject <> nil) and (BaseObject.m_btRaceServer = RC_PLAYOBJECT)
        //                      then OnlineObject := BaseObject as TPlayObject else OnlineObject := nil;
        TPlayObject? onlineObject = null;
        if (baseObject != 0 && PlayerSurfaceServerSend2Seams.GetRaceServerOf(baseObject) == Grobal2Const.RC_PLAYOBJECT)
            onlineObject = PlayerSurfaceServerSend2Seams.GetPlayObjectByHandle(baseObject);

        // 原文 39905：if (OnlineObject <> nil) and OnlineObject.m_boShopStall then
        if (onlineObject != null && PlayerSurfaceServerSend2Seams.GetShopStallOf(onlineObject))
        {
            // 原文 39907：sText := OnlineObject.m_sCharName + '\';
            string sText = onlineObject.m_sCharName + "\\";

            // 原文 39908：if OnlineObject <> Self then
            if (!ReferenceEquals(onlineObject, this))
            {
                // 原文 39910：if not SameText(m_sCurOpenUserHeroM2Shop, OnlineObject.m_sCharName) then
                string curOpen = PlayerSurfaceServerSend2Seams.GetCurOpenUserHeroM2Shop(this);
                if (!string.Equals(curOpen, onlineObject.m_sCharName, StringComparison.OrdinalIgnoreCase))
                {
                    // 原文 39912-39919
                    if (curOpen.Length > 0)
                    {
                        TPlayObject? tempPlayer = PlayerSurfaceCore1Seams.GetPlayObject(curOpen);
                        if (tempPlayer != null)
                            PlayerSurfaceServerSend2Seams.RemoveSelfFromShopItemList(this, tempPlayer);
                    }
                    // 原文 39920：m_sCurOpenUserHeroM2Shop := OnlineObject.m_sCharName;
                    PlayerSurfaceServerSend2Seams.SetCurOpenUserHeroM2Shop(this, onlineObject.m_sCharName);
                }

                // 原文 39922-39923：if OnlineObject.m_HeroM2ShopOpenList.IndexOf(Self) < 0 then Add(Self);
                List<TPlayObject> openList = PlayerSurfaceServerSend2Seams.GetHeroM2ShopOpenList(onlineObject);
                if (openList.IndexOf(this) < 0)
                    openList.Add(this);

                // 原文 39924-39944：双层循环（摊主摆摊表 × 摊主背包）
                List<THeroM2ShopItemInfo> shopList = PlayerSurfaceServerSend2Seams.GetHeroM2ShopList(onlineObject);
                for (int i = 0; i <= shopList.Count - 1; i++)
                {
                    // 原文 39926：HeroM2ShopItemInfo := OnlineObject.m_HeroM2ShopList.Items[I];
                    THeroM2ShopItemInfo shopItemInfo = shopList[i];
                    for (int ii = 0; ii <= onlineObject.m_ItemList.Count - 1; ii++)
                    {
                        // 原文 39929：UserItem := OnlineObject.m_ItemList.Items[II];
                        if (onlineObject.m_ItemList[ii] is not TUserItem userItem) continue;
                        // 原文 39930
                        if (shopItemInfo.nMakeIndex == userItem.MakeIndex)
                        {
                            // 原文 39932-39935
                            TStdItem? stdItem = PlayerSurfaceServerSend2Seams.GetStdItem(userItem.wIndex);
                            TClientItem clientItem = default;   // 原文 var ClientItem: TClientItem（**未初始化**）
                            if (stdItem != null)
                            {
                                byte[]? ci = PlayerSurfaceServerSend2Seams.UserItemToClientItemBytes(
                                    userItem, stdItem.Value, true, true);
                                if (ci != null && ci.Length >= PlayerSurfaceServerSend2Structs.SizeOfTClientItem)
                                    clientItem = PlayerSurfaceServerSend2Structs.TClientItemFromBytes(ci);
                            }
                            // 原文 39937-39939（★ 原文如此：不在上面的 if 内，故 StdItem = nil 时
                            //   用的是"上一轮残留"的 ClientItem —— 本片逐字复刻同一顺序）
                            //   ⚠ 托管 `TStdItem.Reserved1` 是 `Word`（`Grobal2.Types2.cs:43`），
                            //     原文 `ClientItem.S.Reserved1 := nMoneyType`（Integer）对此**静默窄化**，
                            //     故此处显式 (ushort) 复刻（同 PortKit.MakeWord 的口径）。
                            clientItem.s.Price = shopItemInfo.nPrice;
                            clientItem.s.Reserved1 = (ushort)shopItemInfo.nMoneyType; // 需要元宝还是金币
                            clientItem.s.Stock = (int)shopItemInfo.dwAddTick;
                            // 原文 39940
                            sText = sText
                                + Encoding.Latin1.GetString(EDcode.EncodeBuffer(
                                    PlayerSurfaceServerSend2Structs.TClientItemBytes(clientItem),
                                    PlayerSurfaceServerSend2Structs.SizeOfTClientItem))
                                + "/";
                            // 原文 39941
                            break;
                        }
                    }
                }
            }

            // 原文 39946：m_DefMsg := MakeDefaultMsg(SM_HEROM2SENDSHOPITEM, NativeInt(OnlineObject), 0, 0, 0);
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_HEROM2SENDSHOPITEM,
                PlayerSurfaceServerSend2Seams.HandleOf(onlineObject), 0, 0, 0);
            // 原文 39947
            SendSocketRef(m_DefMsg, sText);
        }
    }

    // ==================================================================
    // 39952-40033  ServerSendHeroM2StartShopStall
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendHeroM2StartShopStall(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:39952-40033`；前置注释 `// 开始摆摊函数 piaoyun 2013-07-21`）。
    /// ★ **无 `&lt;&gt; Self` 守卫**（原文如此）。
    ///
    /// 逐条守卫（**全部 `Exit`，顺序逐字保留**）：
    /// <list type="number">
    ///   <item><description>:39954-39955 `g_OnlineMsgControl.boDisableSell` ⇒ Exit。</description></item>
    ///   <item><description>:39957-39961 `m_boLockLogon and m_boPasswordLocked and g_Config.boLockStall`
    ///     ⇒ `SysMsg(g_sCanotTryDealMsg, c_Red, t_Hint)` + Exit。</description></item>
    ///   <item><description>:39962-39966 `m_dwChangeModeExTick[9] &lt;&gt; 0` 且**未到点** ⇒ 同款 SysMsg + Exit。</description></item>
    ///   <item><description>:39968-39973 `not g_Config.boOpenSelfShop` ⇒ `FeatureChanged()` +
    ///     `SysMsg('服务器暂时关闭摆摊功能！', c_Red, t_Hint)` + Exit。</description></item>
    ///   <item><description>:39975-39980 安全区限制（`FeatureChanged()` + SysMsg + Exit）。</description></item>
    ///   <item><description>:39982-39987 地图限制（同上）。</description></item>
    ///   <item><description>:39989-39994 骑马限制（`m_boOnHorse`，同上）。</description></item>
    ///   <item><description>:39996-40000 变身限制（`m_nChangeAppr &gt;= 0`，**无 `FeatureChanged()`**）+ Exit。</description></item>
    ///   <item><description>:40001-40006 已在摆摊（SysMsg 用 **`255, 253`** 颜色对，非 `c_Red`）。</description></item>
    ///   <item><description>:40007-40011 频率门 `Tick_Diff(m_dwHeroM2ShopStallTime, MyGetTickCount) &lt;= 200`
    ///     ⇒ SysMsg + Exit。</description></item>
    ///   <item><description>:40012 通过后**先**置 `m_dwHeroM2ShopStallTime := MyGetTickCount`。</description></item>
    ///   <item><description>:40014 `if m_HeroM2ShopList.Count &gt; 0 then` 才真正开摊：
    ///     `StopCollect` → `m_boShopStall := True` → 清 `m_TargetCret`（非 nil 时若对方
    ///     也锁定自己则先 `DelTargetCreat`）→ `g_FunctionNPC.GotoLable(Self, '@StartMyShop', False)`
    ///     → **再判一次** `if m_boShopStall` 才 `SendRefMsg(RM_SENDSHOPNAME, 0, Self, 0, 0, m_sShopName)`
    ///     + `FeatureChanged`。</description></item>
    /// </list>
    /// ★ 原文如此（**不修**）：`m_HeroM2ShopList.Count = 0` 时整条**静默**（不置位、不报错）。
    /// ★ 原文如此（**不修**）：`@StartMyShop` 脚本可能把 `m_boShopStall` 改回 False，
    ///   故 :40025 的第二次判定不是冗余。
    /// </summary>
    public void ServerSendHeroM2StartShopStall(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 39951：// 开始摆摊函数 piaoyun 2013-07-21
        // 原文 39952-40033
        // 原文 39954-39955
        if (PlayerSurfaceServerSend2Seams.GetDisableSell()) return;

        // 原文 39956：{ TODO -ochongchong -c新增 : 密码保护系统 - 系统锁定时，根据选项来控制是否禁止摆摊 【2013-07-22】 }
        // 原文 39957-39961
        if (PlayerSurfaceServerSend2Seams.GetLockLogon(this)
            && PlayerSurfaceServerSend2Seams.GetPasswordLocked(this)
            && PlayerSurfaceServerSend2Seams.GetLockStall())
        {
            SysMsg(PlayerSurfaceServerSend2Seams.GetCanotTryDealMsg(),
                PlayerSurfaceServerSend2Const.c_Red, PlayerSurfaceServerSend2Const.t_Hint);
            return;
        }

        // 原文 39962-39966
        if (m_dwChangeModeExTick9 != 0
            && PlayerSurfaceServerSendSeams.MyGetTickCount() <= m_dwChangeModeExTick9)
        {
            SysMsg(PlayerSurfaceServerSend2Seams.GetCanotTryDealMsg(),
                PlayerSurfaceServerSend2Const.c_Red, PlayerSurfaceServerSend2Const.t_Hint);
            return;
        }

        // 原文 39967：// 是否开启摆摊 piaoyun 2013-07-21
        // 原文 39968-39973
        if (!PlayerSurfaceServerSend2Seams.GetOpenSelfShop())
        {
            FeatureChanged();
            SysMsg("服务器暂时关闭摆摊功能！", PlayerSurfaceServerSend2Const.c_Red, PlayerSurfaceServerSend2Const.t_Hint);
            return;
        }

        // 原文 39974：// 只允许安全区摆摊 piaoyun 2013-07-21
        // 原文 39975-39980
        if (PlayerSurfaceServerSend2Seams.GetSafeZoneShop()
            && !PlayerSurfaceServerSend2Seams.GetInSafeZone(this))
        {
            FeatureChanged();
            SysMsg("不能在安全区以外的地方进行摆摊！", PlayerSurfaceServerSend2Const.c_Red, PlayerSurfaceServerSend2Const.t_Hint);
            return;
        }

        // 原文 39981：// 只允许指定地图摆摊 piaoyun 2013-07-21
        // 原文 39982-39987
        if (PlayerSurfaceServerSend2Seams.GetMapShop()
            && !(m_PEnvir != null && PlayerSurfaceServerSend2Seams.GetEnvirAllowUseMyshop(m_PEnvir)))
        {
            FeatureChanged();
            SysMsg("本地图禁止摆摊！", PlayerSurfaceServerSend2Const.c_Red, PlayerSurfaceServerSend2Const.t_Hint);
            return;
        }

        // 原文 39988：// 骑马 chongchong 2013-10-12
        // 原文 39989-39994
        if (m_boOnHorse)
        {
            FeatureChanged();
            SysMsg("请先下马！", PlayerSurfaceServerSend2Const.c_Red, PlayerSurfaceServerSend2Const.t_Hint);
            return;
        }

        // 原文 39995：// 变身后 2020-08-22 12:24:05
        // 原文 39996-40000（★ 原文如此：这一条**没有** FeatureChanged()）
        if (m_nChangeAppr >= 0)
        {
            SysMsg("变身状态不允许摆摊！", PlayerSurfaceServerSend2Const.c_Red, PlayerSurfaceServerSend2Const.t_Hint);
            return;
        }

        // 原文 40001-40006
        if (m_boShopStall)
        {
            FeatureChanged();
            SysMsg("您已经在摆摊了！", 255, 253, PlayerSurfaceServerSend2Const.t_Hint);
            return;
        }

        // 原文 40007-40011
        if (SS2TickDiff(PlayerSurfaceServerSend2Seams.GetHeroM2ShopStallTime(this),
                PlayerSurfaceServerSendSeams.MyGetTickCount()) <= 200)
        {
            SysMsg("操作过快！", 255, 253, PlayerSurfaceServerSend2Const.t_Hint);
            return;
        }
        // 原文 40012
        PlayerSurfaceServerSend2Seams.SetHeroM2ShopStallTime(this, PlayerSurfaceServerSendSeams.MyGetTickCount());

        // 原文 40013：// 开始摆摊
        // 原文 40014
        if (m_HeroM2ShopList.Count > 0)
        {
            // 原文 40016
            PlayerSurfaceServerSend2Seams.StopCollect(this);
            // 原文 40017
            m_boShopStall = true;

            // 原文 40018-40023
            if (m_TargetCret != null)
            {
                // 原文 40020-40021：if m_TargetCret.m_TargetCret = Self then m_TargetCret.DelTargetCreat;
                if (PlayerSurfaceServerSend2Seams.TargetCretIsSelf(m_TargetCret))
                    ((TPlayObject)m_TargetCret!).DelTargetCreat();
                // 原文 40022
                m_TargetCret = null;
            }

            // 原文 40024
            PlayerSurfaceServerSend2Seams.GotoLable(this, "", "@StartMyShop");

            // 原文 40025-40031
            if (m_boShopStall)
            {
                // 原文 40027-40028（注释 + 被注释掉的 if）
                // 原文 40029：SendRefMsg(RM_SENDSHOPNAME, 0, NativeInt(Self), 0, 0, m_sShopName);
                SendRefMsg(Grobal2Const.RM_SENDSHOPNAME, 0, m_nRecogId, 0, 0,
                    PlayerSurfaceServerSend2Seams.GetShopName(this), 0);
                // 原文 40030：FeatureChanged;   ← 无括号的 Delphi 调用（同义）
                FeatureChanged();
            }
        }
    }

    // ==================================================================
    // 40035-40068  ServerSendHeroM2StopShopStall
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendHeroM2StopShopStall(ProcessMsg; var boResult); // 停止摆摊`
    /// （`ObjPlayer.pas:40035-40068`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。
    ///
    /// <code>
    ///   if m_boShopStall then                                    // 40041
    ///   begin
    ///     if Tick_Diff(m_dwHeroM2ShopStallTime, MyGetTickCount) &lt;= 200 then
    ///     begin SysMsg('操作过快！', 255, 253, t_Hint); Exit; end;   // 40043-40047
    ///     m_dwHeroM2ShopStallTime := MyGetTickCount;             // 40048
    ///     for I := 0 to m_HeroM2ShopList.Count - 1 do            // 40049
    ///     begin
    ///       HeroM2ShopItemInfo := m_HeroM2ShopList.Items[I];
    ///       for II := 0 to m_ItemList.Count - 1 do              // 40052
    ///       begin
    ///         UserItem := m_ItemList.Items[II];
    ///         if HeroM2ShopItemInfo.nMakeIndex = UserItem.MakeIndex then
    ///         begin SendUpdateItem(UserItem); Break; end;        // 40057-40058
    ///       end;
    ///       Dispose(HeroM2ShopItemInfo);                         // 40061
    ///     end;
    ///     m_HeroM2ShopList.Clear;                                // 40063
    ///     m_boShopStall := False;                                // 40064
    ///     FeatureChanged;                                        // 40065
    ///     g_FunctionNPC.GotoLable(Self, '@StopMyShop', False);   // 40066
    ///   end;
    /// </code>
    /// ★ 原文如此（**不修**）：`Break` 只跳出**内层**（背包）循环，外层仍继续
    ///   `Dispose` 下一个摆摊记录；`:40061` 的 `Dispose` **不在** `if` 内，
    ///   即每轮外层都释放一条（无论有没有匹配到背包物品）。
    /// ★ 原文如此（**不修**）：`:40041` 为假时**整条什么都不做**（无 `else`）。</summary>
    public void ServerSendHeroM2StopShopStall(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 40035-40068
        // 原文 40041
        if (m_boShopStall)
        {
            // 原文 40043-40047
            if (SS2TickDiff(PlayerSurfaceServerSend2Seams.GetHeroM2ShopStallTime(this),
                    PlayerSurfaceServerSendSeams.MyGetTickCount()) <= 200)
            {
                SysMsg("操作过快！", 255, 253, PlayerSurfaceServerSend2Const.t_Hint);
                return;
            }
            // 原文 40048
            PlayerSurfaceServerSend2Seams.SetHeroM2ShopStallTime(this, PlayerSurfaceServerSendSeams.MyGetTickCount());

            // 原文 40049-40062
            for (int i = 0; i <= m_HeroM2ShopList.Count - 1; i++)
            {
                // 原文 40051
                THeroM2ShopItemInfo heroM2ShopItemInfo = m_HeroM2ShopList[i];
                // 原文 40052-40060
                for (int ii = 0; ii <= m_ItemList.Count - 1; ii++)
                {
                    // 原文 40054
                    if (m_ItemList[ii] is not TUserItem userItem) continue;
                    // 原文 40055
                    if (heroM2ShopItemInfo.nMakeIndex == userItem.MakeIndex)
                    {
                        // 原文 40057：SendUpdateItem(UserItem); // 刷新一下，原来的Price 和Reserved1字段改变了
                        PlayerSurfaceServerSend2Seams.SendUpdateItem(this, userItem);
                        // 原文 40058
                        break;
                    }
                }
                // 原文 40061：Dispose(HeroM2ShopItemInfo);
                //   托管侧 THeroM2ShopItemInfo 是 class ⇒ 由 GC 承担（与原 Dispose 同义：不再引用）
            }

            // 原文 40063
            m_HeroM2ShopList.Clear();
            // 原文 40064
            m_boShopStall = false;
            // 原文 40065
            FeatureChanged();
            // 原文 40066
            PlayerSurfaceServerSend2Seams.GotoLable(this, "", "@StopMyShop");
        }
    }

    // ==================================================================
    // 40070-40407  ServerSendHeroM2BuyUserItem —— 留痕
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendHeroM2BuyUserItem(ProcessMsg; var boResult); // 购买摆摊物品`
    /// （`ObjPlayer.pas:40070-40407`，**338 行**，含 **3 个嵌套过程**）。
    /// <para><b>未移植（显式留痕）</b>。缺失成员（原文行号）：
    /// <c>AddGameDataLog(LogAction1, LogAction2, LogActor, ItemName, ItemMakeIndex,
    /// TargetName, Data1, Data2, LogDesc)</c>（:40100 / :40111 / :40123 / :40134 / :40145 /
    /// :40168 / :40180 / :40193 / :40205 / :40217 —— 9 参 `M2Share.pas` 面）；
    /// <c>LOG_ItemBuyPrice</c>/<c>LOG_ItemSell</c>/<c>LOG_GameGoldChange</c>/<c>LOG_GamePointChange</c>/
    /// <c>LOG_GoldChange</c>/<c>LOG_GameDiamondChange</c>/<c>LOG_GameGirdChange</c>（同上）；
    /// <c>g_boGameLogGameGold</c>/<c>g_boGameLogGold</c>（同）；
    /// <c>g_Config.dwSellOffGameGoldTaxRate</c>/<c>dwSellOffGamePointTaxRate</c>/
    /// <c>dwSellOffGameDiamondTaxRate</c>/<c>dwSellOffGameGirdTaxRate</c>（:40163/:40175/:40187/:40200/:40212）；
    /// <c>g_sHeroM2BuyerMsg</c>/<c>g_sHeroM2SellerMsg</c> 的 <c>Format</c> 占位面（:40368/:40371）；
    /// <c>SysMsg</c> 的**四参**重载 <c>SysMsg(sMsg, 255, 252, t_Hint)</c>（本片已接缝化，
    /// 见 <see cref="PlayerSurfaceServerSend2Seams.SysMsgTo"/>，但整条方法仍留痕）。
    /// 另有 **原文缺陷** ②③（见文件头的"原文如此清单"第 ⑤ 条）：
    /// :40222-40223 的 `else` 只吃 `nTCustomMoneyCount := 0;`，紧跟的
    /// <c>SellUser.IncMoney(...)</c> **无条件执行**且用到**未初始化**的
    /// `nTCustomMoneyCount`。移植它需要连"未初始化局部变量"的读取语义一起固化，
    /// 与托管侧"局部变量必须赋值"的规则正面冲突。
    /// 故按任务书第 4 条整条留痕。</para>
    /// </summary>
    public void ServerSendHeroM2BuyUserItem(TProcessMessage ProcessMsg, ref bool boResult)
    {
        PortNotPorted(nameof(ServerSendHeroM2BuyUserItem), 40070);
    }

    // ==================================================================
    // 40410-40664  ServerSendHeroM2AddUserItem —— 留痕
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendHeroM2AddUserItem(ProcessMsg; var boResult); // 增加摆摊物品`
    /// （`ObjPlayer.pas:40410-40664`，**255 行**）。
    /// <para><b>未移植（显式留痕）</b>。缺失成员（原文行号）：
    /// <c>GetLowestSellingPrice(sItemName): PLowestSellingPrice</c>（:40536）与
    /// <c>GetHighestSellingPrice(sItemName): PHighestSellingPrice</c>（:40595）——
    /// 两个**记录指针**型查询，其记录布局
    /// （<c>nGameGoldPice</c>/<c>nGamePointPrice</c>/<c>nGoldPrice</c>/<c>nGameDiamondPice</c>/
    /// <c>nGameGameGirdPrice</c>，5 种货币各一）在托管侧**无类型定义**；
    /// <c>GetUserItemBindValue(UserItem, ubNoSell)</c>（:40469，**取 `TUserItem` 的变体**，
    /// 与 `DbLayerSeams` 的 `(int,int)` 版**不同入口**）；
    /// <c>g_sCannotSell</c>（:40473）；<c>g_ItemRules.Get(wIndex, 8)</c>（:40469 ——
    /// `ViewList2State.g_ItemRules` 存在但 `FlagArray` 只初始化到 41 项、且该单例属
    /// **窗体状态**，直接复用会把 M2 引擎语义绑到 WinForms 单例上）；
    /// <c>g_Config.boMyShopGameGold</c>/<c>boMyShopGamePoint</c>/<c>boMyShopGold</c>/
    /// <c>boMyShopGameDiamond</c>/<c>boMyShopGameGird</c>（:40489-40525）；
    /// <c>AddGameDataLog</c> 9 参面 + <c>LOG_ItemPutInto</c>/<c>LOG_ActionNone</c>（:40662）。
    /// 另：:40652-40656 的 `New(HeroM2ShopItemInfo)` + 4 个字段赋值 + `Add` 本片已可表达，
    /// 但**整条的守卫链**（:40428 / :40434 / :40440 / :40451 / :40462 / :40480 /
    /// :40486-40532 的 5 分支货币开关 / :40539-40590 最低价 / :40598-40649 最高价）
    /// 一旦缺了"价格区间查询"这一环就无法判定 `Exit`，会被迫**重排守卫顺序**
    /// —— 那正是任务书禁止的"改进"。故整条留痕。</para>
    /// </summary>
    public void ServerSendHeroM2AddUserItem(TProcessMessage ProcessMsg, ref bool boResult)
    {
        PortNotPorted(nameof(ServerSendHeroM2AddUserItem), 40410);
    }

    // ==================================================================
    // 40666-40715  ServerSendHeroM2DelUserItem
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendHeroM2DelUserItem(ProcessMsg; var boResult); // 删除摆摊物品`
    /// （`ObjPlayer.pas:40666-40715`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。
    ///
    /// <code>
    ///   { if m_boShopStall then ... }                      // 40673-40683 ← 整段**被注释掉**
    ///   for I := 0 to m_HeroM2ShopList.Count - 1 do        // 40684
    ///   begin
    ///     HeroM2ShopItemInfo := m_HeroM2ShopList.Items[I];
    ///     if HeroM2ShopItemInfo.nMakeIndex = ProcessMsg.nParam1 then
    ///     begin
    ///       m_HeroM2ShopList.Delete(I); Dispose(HeroM2ShopItemInfo);   // 40689-40690
    ///       if ProcessMsg.nParam2 = 0 then
    ///         SendDefMessage(SM_HEROM2DELUSERSHOPITEM_OK, nParam1, 0, 0, 0, '');  // 40691-40692
    ///       for II := 0 to m_ItemList.Count - 1 do
    ///       begin
    ///         UserItem := m_ItemList.Items[II];
    ///         if UserItem.MakeIndex = ProcessMsg.nParam1 then
    ///         begin
    ///           SendUpdateItem(UserItem);   // 刷新一下，原来的Price 和Reserved1字段改变了
    ///           StdItem := UserEngine.GetStdItem(UserItem.wIndex);
    ///           if (StdItem &lt;&gt; nil) and (StdItem.NeedIdentify = 1) then
    ///             AddGameDataLog(LOG_ItemTakeBack, LOG_ActionNone, Self, StdItem.Name,
    ///                            UserItem.MakeIndex, '摆摊');               // 40702
    ///           Break;
    ///         end;
    ///       end;
    ///       Exit;                                          // 40707 ← **方法级 Exit**
    ///     end;
    ///   end;
    ///   if ProcessMsg.nParam2 = 0 then
    ///   begin
    ///     SendDefMessage(SM_HEROM2DELUSERSHOPITEM_FAIL, nParam1, 0, 0, 0, '');  // 40712
    ///     SysMsg('取回摆摊物品失败！', 255, 253, t_Hint);                          // 40713
    ///   end;
    /// </code>
    /// ★ 关键（**原文如此，逐字保留**）：
    ///   ① :40707 的 `Exit` 在**命中分支内部** ⇒ 命中后**不再**执行 :40710 的失败段；
    ///   ② 未命中时**落到** :40710，且**只有 `nParam2 = 0` 才**发失败（`nParam2 &lt;&gt; 0` 静默）；
    ///   ③ :40673-40683 的"摆摊中不允许取回"整段是**块注释**（`{ ... }`）——
    ///      注释掉的 `SendDefMessage` 用 `..._FAIL`、而活代码 :40712 的注释却写"删除摆摊物品成功"
    ///      （**原文注释写错**，ident 是 `_FAIL`）。
    /// ⚠ `AddGameDataLog`（9 参 `M2Share.pas` 面）与 `LOG_ItemTakeBack`/`LOG_ActionNone`
    ///   未移植 ⇒ 走 <see cref="PlayerSurfaceServerSend2Seams.AddGameDataLog"/>。</summary>
    public void ServerSendHeroM2DelUserItem(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 40666-40715
        // 原文 40673-40683：{ if m_boShopStall then ... } ← 整段块注释，**不移植**（原文如此）
        //   { if m_boShopStall then
        //     begin // 摆摊
        //       if ProcessMsg.nParam2 = 0 then
        //       begin
        //         SendDefMessage(SM_HEROM2DELUSERSHOPITEM_FAIL, ProcessMsg.nParam1, 0, 0, 0, '');
        //         SysMsg('摆摊状态中不允许取回物品！', 255, 253, t_Hint);
        //       end;
        //       Exit;
        //     end;
        //   }

        // 原文 40684-40709
        for (int i = 0; i <= m_HeroM2ShopList.Count - 1; i++)
        {
            // 原文 40686
            THeroM2ShopItemInfo heroM2ShopItemInfo = m_HeroM2ShopList[i];
            // 原文 40687
            if (heroM2ShopItemInfo.nMakeIndex == ProcessMsg.nParam1)
            {
                // 原文 40689-40690
                m_HeroM2ShopList.RemoveAt(i);

                // 原文 40691-40692
                if (ProcessMsg.nParam2 == 0)
                    PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_HEROM2DELUSERSHOPITEM_OK,
                        ProcessMsg.nParam1, 0, 0, 0, "");

                // 原文 40693-40706
                for (int ii = 0; ii <= m_ItemList.Count - 1; ii++)
                {
                    // 原文 40695
                    if (m_ItemList[ii] is not TUserItem userItem) continue;
                    // 原文 40696
                    if (userItem.MakeIndex == ProcessMsg.nParam1)
                    {
                        // 原文 40698：SendUpdateItem(UserItem); // 刷新一下，原来的Price 和Reserved1字段改变了
                        PlayerSurfaceServerSend2Seams.SendUpdateItem(this, userItem);
                        // 原文 40699
                        TStdItem? stdItem = PlayerSurfaceServerSend2Seams.GetStdItem(userItem.wIndex);
                        // 原文 40700-40703
                        if (stdItem != null && stdItem.Value.NeedIdentify == 1)
                        {
                            PlayerSurfaceServerSend2Seams.AddGameDataLog(
                                PlayerSurfaceServerSend2Const.LOG_ItemTakeBack,
                                PlayerSurfaceServerSend2Const.LOG_ActionNone,
                                this, stdItem.Value.NameStr, userItem.MakeIndex, "", 0, 0, "摆摊");
                        }
                        // 原文 40704
                        break;
                    }
                }
                // 原文 40707：Exit;  ← **方法级**早退（命中后不再走下面的失败段）
                return;
            }
        }

        // 原文 40710-40714
        if (ProcessMsg.nParam2 == 0)
        {
            // 原文 40712（★ 原文注释写的是"删除摆摊物品成功"，ident 实为 _FAIL）
            PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_HEROM2DELUSERSHOPITEM_FAIL,
                ProcessMsg.nParam1, 0, 0, 0, "");
            // 原文 40713
            SysMsg("取回摆摊物品失败！", 255, 253, PlayerSurfaceServerSend2Const.t_Hint);
        }
    }

    // ==================================================================
    // 40717-40730  ServerSendHeroM2CloseShop
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendHeroM2CloseShop(ProcessMsg; var boResult); // 关闭购买摆摊物品窗口`
    /// （`ObjPlayer.pas:40717-40730`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。
    ///
    /// <code>
    ///   if Length(m_sCurOpenUserHeroM2Shop) &gt; 0 then                        // 40721
    ///   begin
    ///     Player := UserEngine.GetPlayObject(m_sCurOpenUserHeroM2Shop);      // 40723  ← **按名字**
    ///     if Player &lt;&gt; nil then
    ///       Player.m_HeroM2ShopList.Remove(Self);                            // 40726
    ///   end;
    ///   m_sCurOpenUserHeroM2Shop := '';                                      // 40729
    /// </code>
    /// ★ 原文如此（**不修**）：`:40726` 是把 `Self` 从**摊主的**
    ///   `m_HeroM2ShopList`（**摆摊物品表**）里 `Remove` —— 而 :39923 / :39917 用的是
    ///   `m_HeroM2ShopOpenList`（**浏览者表**）。原文在这两处用了不同的表；
    ///   本片**逐字保留**（`m_HeroM2ShopList` 的元素类型是 `THeroM2ShopItemInfo`，
    ///   与 `Self` 类型不符 —— 托管 `List<T>.Remove(T)` 因此**永远返回 false**、
    ///   即"什么都没移除"。这一步在原文里同样什么都不会移除（Delphi `TList.Remove`
    ///   按指针比较，摊主表里存的是 `pTHeroM2ShopItemInfo`，也不可能等于 `Self`）。
    ///   故本片**只做等价的无操作表达**并留痕。</summary>
    public void ServerSendHeroM2CloseShop(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 40717-40730
        // 原文 40721
        if (m_sCurOpenUserHeroM2Shop.Length > 0)
        {
            // 原文 40723（★ 按**名字**查，与 :40242 的按**指针**查不同重载）
            TPlayObject? player = PlayerSurfaceCore1Seams.GetPlayObject(m_sCurOpenUserHeroM2Shop);
            if (player != null)
            {
                // 原文 40726：Player.m_HeroM2ShopList.Remove(Self);
                //   ★ 原文如此：`Self`（TPlayObject）不可能等于表里的
                //   `pTHeroM2ShopItemInfo` 元素 ⇒ Delphi `TList.Remove` 恒为"未命中"。
                //   托管侧按同一语义表达为"对该表做一次类型不匹配的移除尝试"（恒 false）。
                PlayerSurfaceServerSend2Seams.RemoveSelfFromShopItemList(player, this);
            }
        }
        // 原文 40729
        m_sCurOpenUserHeroM2Shop = "";
    }

    // ==================================================================
    // 40733-40740  ClientButtonClick
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ClientButtonClick(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:40733-40740`）。★ **无守卫**（原文如此）。
    /// 脚本标签 = `'@ButtonClick' + IntToStr(ProcessMsg.nParam2)`。</summary>
    public void ClientButtonClick(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 40733-40740
        // 原文 40735
        if (PlayerSurfaceServerSend2Seams.HasFunctionNpc())
        {
            // 原文 40737
            m_nScriptGotoCount = 0;
            // 原文 40738
            PlayerSurfaceServerSend2Seams.GotoLable(this, "", "@ButtonClick" + SS2IntToStr(ProcessMsg.nParam2));
        }
    }

    // ==================================================================
    // 40742-40749  ClientNumberButtonClick
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ClientNumberButtonClick(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:40742-40749`）。与 <see cref="ClientButtonClick"/> 逐行同构，
    /// 只差标签前缀（`'@NumberButtonClick'`）。★ **无守卫**（原文如此）。</summary>
    public void ClientNumberButtonClick(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 40742-40749
        // 原文 40744
        if (PlayerSurfaceServerSend2Seams.HasFunctionNpc())
        {
            // 原文 40746
            m_nScriptGotoCount = 0;
            // 原文 40747
            PlayerSurfaceServerSend2Seams.GotoLable(this, "", "@NumberButtonClick" + SS2IntToStr(ProcessMsg.nParam2));
        }
    }

    // ==================================================================
    // 40751-40758  ClientArrButtonClick
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ClientArrButtonClick(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:40751-40758`）。与 <see cref="ClientButtonClick"/> 逐行同构，
    /// 只差标签前缀（`'@ArrButtonClick'`）。★ **无守卫**（原文如此）。</summary>
    public void ClientArrButtonClick(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 40751-40758
        // 原文 40753
        if (PlayerSurfaceServerSend2Seams.HasFunctionNpc())
        {
            // 原文 40755
            m_nScriptGotoCount = 0;
            // 原文 40756
            PlayerSurfaceServerSend2Seams.GotoLable(this, "", "@ArrButtonClick" + SS2IntToStr(ProcessMsg.nParam2));
        }
    }

    // ==================================================================
    // 40760-40767  ClientClientBuffClick
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ClientClientBuffClick(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:40760-40767`）。与 <see cref="ClientButtonClick"/> 逐行同构，
    /// 只差标签前缀（`'@ClientBuffClick'`）。★ **无守卫**（原文如此）。</summary>
    public void ClientClientBuffClick(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 40760-40767
        // 原文 40762
        if (PlayerSurfaceServerSend2Seams.HasFunctionNpc())
        {
            // 原文 40764
            m_nScriptGotoCount = 0;
            // 原文 40765
            PlayerSurfaceServerSend2Seams.GotoLable(this, "", "@ClientBuffClick" + SS2IntToStr(ProcessMsg.nParam2));
        }
    }

    // ==================================================================
    // 40769-40776  ClientArrBuffClick
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ClientArrBuffClick(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:40769-40776`）。与 <see cref="ClientButtonClick"/> 逐行同构，
    /// 只差标签前缀（`'@ArrBuffClick'`）。★ **无守卫**（原文如此）。</summary>
    public void ClientArrBuffClick(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 40769-40776
        // 原文 40771
        if (PlayerSurfaceServerSend2Seams.HasFunctionNpc())
        {
            // 原文 40773
            m_nScriptGotoCount = 0;
            // 原文 40774
            PlayerSurfaceServerSend2Seams.GotoLable(this, "", "@ArrBuffClick" + SS2IntToStr(ProcessMsg.nParam2));
        }
    }

    // ==================================================================
    // 40778-40792  ClientAutoFindPath
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ClientAutoFindPath(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:40778-40792`）。★ **无守卫**（原文如此）。
    ///
    /// <code>
    ///   if g_FunctionNPC &lt;&gt; nil then                      // 40780
    ///   begin
    ///     m_nScriptGotoCount := 0;                          // 40782
    ///     case ProcessMsg.wParam of                         // 40783
    ///       1: g_FunctionNPC.GotoLable(Self, '@FindPathBegin', False);
    ///       2: g_FunctionNPC.GotoLable(Self, '@FindPathStop',  False);
    ///       3: g_FunctionNPC.GotoLable(Self, '@FindPathEnd',   False);
    ///     end;
    ///   end;
    /// </code>
    /// ★ 原文如此（**不修**）：`case` **无 `else`** ⇒ 其余 `wParam` 只置
    ///   `m_nScriptGotoCount := 0` 而不跳标签（**副作用照旧发生**）。</summary>
    public void ClientAutoFindPath(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 40778-40792
        // 原文 40780
        if (PlayerSurfaceServerSend2Seams.HasFunctionNpc())
        {
            // 原文 40782
            m_nScriptGotoCount = 0;
            // 原文 40783-40790
            switch (ProcessMsg.wParam)
            {
                case 1:
                    PlayerSurfaceServerSend2Seams.GotoLable(this, "", "@FindPathBegin");
                    break;
                case 2:
                    PlayerSurfaceServerSend2Seams.GotoLable(this, "", "@FindPathStop");
                    break;
                case 3:
                    PlayerSurfaceServerSend2Seams.GotoLable(this, "", "@FindPathEnd");
                    break;
                // 原文如此：无 `else`
            }
        }
    }

    // ==================================================================
    // 40794-40802  ClientPlugInConfig
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ClientPlugInConfig(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:40794-40802`）。★ **无守卫**（原文如此）。
    ///
    /// <code>
    ///   m_nHeroDodgeHPPercent          := Max(0, ProcessMsg.nParam1);            // 40796
    ///   m_boHeroAutoShield             := LoByte(ProcessMsg.nParam2) &lt;&gt; 0;      // 40797
    ///   m_boAssistantHeroAutoShield    := HiByte(ProcessMsg.nParam2) &lt;&gt; 0;      // 40798
    ///   m_boHeroContinuousNoHitMon     := LoByte(ProcessMsg.nParam3) &lt;&gt; 0;      // 40799
    ///   m_boAutoCHangePoison           := HiByte(ProcessMsg.nParam3) &lt;&gt; 0;      // 40800
    ///   m_boAllowDeal                  := ProcessMsg.wParam = 0;                // 40801
    /// </code>
    /// ★ 原文如此（**不修**）：`:40797-40800` 用 `LoByte`/`HiByte` 拆 **两个 `nParam`**
    ///   （每个低/高字节各承载一个开关），故 4 个开关只占 2 个参数。</summary>
    public void ClientPlugInConfig(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 40794-40802
        // 原文 40796：m_nHeroDodgeHPPercent := Max(0, ProcessMsg.nParam1);
        m_nHeroDodgeHPPercent = SS2MaxInt(0, (int)ProcessMsg.nParam1);
        // 原文 40797：英雄持续开盾 chongchong 2013-08-17
        m_boHeroAutoShield = SS2LoByte(ProcessMsg.nParam2) != 0;
        // 原文 40798：副英雄持续开盾 chongchong 2013-08-17
        m_boAssistantHeroAutoShield = SS2HiByte(ProcessMsg.nParam2) != 0;
        // 原文 40799
        m_boHeroContinuousNoHitMon = SS2LoByte(ProcessMsg.nParam3) != 0;
        // 原文 40800
        m_boAutoCHangePoison = SS2HiByte(ProcessMsg.nParam3) != 0;
        // 原文 40801
        m_boAllowDeal = ProcessMsg.wParam == 0;
    }

    // ==================================================================
    // 40805-40900  ClientTakeHorse —— 留痕
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ClientTakeHorse(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:40805-40900`；前置 `{ TODO -ochongchong -c新增 : 骑马 召唤/收回坐骑 【2013-10-12】 }`）。
    /// <para><b>未移植（显式留痕）</b>。缺失成员（原文行号）：
    /// `g_sHorseStopShop`/`g_sHorseBrand`/`g_sHorseMapDisable`/`g_sHorseTime`（:40813/:40826/:40831/:40843）；
    /// `m_PEnvir.m_boNOHORSE`（:40829）；`m_HorseOtherHum.DoTalkStatusChanged`（:40884/:40894）、
    /// `m_HorseOtherHum.TriggerHorseScript`（:40885/:40895）、`m_HorseOtherHum.SendTakeOffHorse`（:40887/:40897）
    /// —— 这三个是**对方 TPlayObject** 的骑马子系统方法，托管侧整条骑马链未移植；
    /// `TriggerHorseScript`/`SendTakeOffHorse`/`DoTalkStatusChanged`/`RefShowName`（自身侧，:40868/:40897/:40894/:40867）；
    /// `g_Config.dwHorseTakeTime`/`dwTakeOnHorseUseTime`（:40836-40854）；
    /// `StopCollect`（:40834/:40889）。
    /// 本片已为上述每一项声明了接缝（见 `PlayerSurfaceServerSend2Seams`），
    /// 但**整条方法**仍留痕，理由：:40835-40845 的 `dwTime` 计算 + `mod`/`div` 进位
    /// （`if dwTime mod 1000 &gt; 0 then dwTime := dwTime div 1000 + 1`）与
    /// :40846-40858 的 `m_boCanOnHorse`/`m_dwCanOnHorseTime` 状态机**互相咬合**，
    /// 而其中 `m_HorseOtherHum` 的四个调用是"对另一个玩家的状态机操作"——
    /// 在没有骑马子系统的前提下，只接缝化"方法调用"而保留状态机，
    /// 会得到一个**永不生效**的状态机（`m_boOnHorse` 置位后没有任何消费者），
    /// 反而比留痕更具误导性。按任务书第 4 条整条留痕。</para>
    /// </summary>
    public void ClientTakeHorse(TProcessMessage ProcessMsg, ref bool boResult)
    {
        PortNotPorted(nameof(ClientTakeHorse), 40805);
    }

    // ==================================================================
    // 40903-40993  ClientInviteHorse —— 留痕
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ClientInviteHorse(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:40903-40993`；前置 `{ TODO -ochongchong -c新增 : 骑马 邀请别人共骑 【2013-10-13】 }`）。
    /// <para><b>未移植（显式留痕）</b>。缺失成员（原文行号）：
    /// `g_sHorseNotInvitePlayer`/`g_sHorseInviteNoPlayer`/`g_sHorseInviteMustPlayer`/
    /// `g_sPoseDisableHorseInviteMsg`/`g_sHorseInviteFar`/`g_sHorseInviteOnHorse`
    /// （:40930/:40943/:40952/:40989/:40957/:40962/:40975/:40980）；
    /// `m_UseItems[U_HORSE]`/`[U_RIGHTHAND]`（:40920/:40923/:40938，`U_HORSE`/`U_RIGHTHAND`
    /// 槽位常量在 `UseSlots`/`RecalcChain.cs:120` 有 `m_UseItems` 但该常量面未核）；
    /// `UserEngine.GetStdItem(...).StdMode/.Source`（:40925-40946）；
    /// `m_PEnvir.GetMovingObject(nX, nY, False)`（:40949）；`CretInNearXY(BaseObject, nX, nY)`（:40971）；
    /// 对方对象的 `m_boDisableHorseInvite`/`m_nChangeAppr`/`m_boOnHorse`（:40960/:40966/:40978）；
    /// 写对方的 `m_InviteHorseHum`/`m_dwInviteHorseTick`（:40983-40984）；
    /// `TPlayObject(BaseObject).SendMsg(BaseObject, RM_INVITEHORSE, 0, NativeInt(Self), 0, 0, '')`
    /// （:40985，7 参 `TBaseObject.SendMsg`）。
    /// 留痕理由同 <see cref="ClientTakeHorse"/>：整条是骑马子系统的**邀请侧状态机**，
    /// 缺了被邀请方的消费端就只剩"写进没人读的字段"。按任务书第 4 条整条留痕。</para>
    /// </summary>
    public void ClientInviteHorse(TProcessMessage ProcessMsg, ref bool boResult)
    {
        PortNotPorted(nameof(ClientInviteHorse), 40903);
    }

    // ==================================================================
    // 40995-41057  ClientResponseInviteHorse —— 留痕
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ClientResponseInviteHorse(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:40995-41057`）。
    /// <para><b>未移植（显式留痕）</b>。缺失成员（原文行号）：
    /// `g_sResponseHorseNo`/`g_sResponseHorseTimeOut`/`g_sResponseHorseOtherPlayer`/
    /// `g_sResponseHorseFar`（:41001/:41008/:41014/:41020）；
    /// 邀请人对象的 `m_sCharName`/`m_HorseOtherHum`/`m_nCurrX`/`m_nCurrY`/`m_btHorseType`
    /// 的**读写**（:41001/:41008/:41011/:41017/:41020/:41028/:41034/:41036）；
    /// `m_PEnvir.MoveToMovingObject(m_nCurrX, m_nCurrY, Self, 邀请人X, 邀请人Y, False)`
    /// （:41041-41042）；`SendMsg(Self, RM_SYNCSCREEN, 3, m_nCurrX, m_nCurrY, m_btDirection, '')`
    /// （:41047，7 参 `TBaseObject.SendMsg`）；`g_FunctionNPC.GotoLable(Self, '@UpHorse', False)`（:41052）。
    /// 留痕理由同上：`m_InviteHorseHum` 的另一半（邀请侧）本片已留痕，
    /// 响应侧单独移植会形成"半个状态机"。按任务书第 4 条整条留痕。</para>
    /// </summary>
    public void ClientResponseInviteHorse(TProcessMessage ProcessMsg, ref bool boResult)
    {
        PortNotPorted(nameof(ClientResponseInviteHorse), 40995);
    }

    // ==================================================================
    // 41059-41066  ClientOpenJewelryBox
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ClientOpenJewelryBox(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:41059-41066`）。★ **无守卫**（原文如此）。
    /// 标签固定 `'@OpenSndacasket'`（**原文拼写如此**，非 `@OpenCasket`）。</summary>
    public void ClientOpenJewelryBox(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 41059-41066
        // 原文 41061
        if (PlayerSurfaceServerSend2Seams.HasFunctionNpc())
        {
            // 原文 41063
            m_nScriptGotoCount = 0;
            // 原文 41064
            PlayerSurfaceServerSend2Seams.GotoLable(this, "", "@OpenSndacasket");
        }
    }

    // ==================================================================
    // 41068-41077  ClientHeroOpenJewelryBox
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ClientHeroOpenJewelryBox(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:41068-41077`）。★ **无 `&lt;&gt; Self` 守卫**，但有
    /// `:41070 if m_MyHero = nil then Exit;`（**英雄为空早退**）。
    /// 标签固定 `'@HeroOpenSndacasket'`（原文拼写如此）。</summary>
    public void ClientHeroOpenJewelryBox(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 41068-41077
        // 原文 41070-41071
        if (m_MyHero == null) return;

        // 原文 41072
        if (PlayerSurfaceServerSend2Seams.HasFunctionNpc())
        {
            // 原文 41074
            m_nScriptGotoCount = 0;
            // 原文 41075
            PlayerSurfaceServerSend2Seams.GotoLable(this, "", "@HeroOpenSndacasket");
        }
    }
}

// ============================================================================
// 以下三个类是本片的**私有支撑件**（常量 / 全局串 / 字节装配）。
//
// 为什么与 partial 类同文件：
//   ① 常量与全局串**不是实例状态** —— 原文里它们是 `M2Share.pas` / `Envir.pas` /
//      `LogManage.pas` 的**单元级**符号（那些单元尚未移植）；放进 `static` 类才能
//      同时满足「可被宿主/测试注入」与「不污染 partial 类的实例面」。
//   ② 字节装配（`Move`/`SizeOf`/`packed record` 序列化）是纯函数；原文由
//      `Move`/`@rec` 隐式完成，托管侧必须显式落成方法，且**每处都要与原文的
//      `SizeOf(Txxx)` 对齐**（见各方法注释里的字段表）。
//   ★ 本片按任务书第 1 条「只写两个文件」，故**不另开文件**。
// ============================================================================

/// <summary>
/// 原文 38860-41077 区间用到的**常量**（来源单元已注明）。
/// 托管侧 `Grobal2Const`（`GXX.Core.Protocol`）已收录本片全部 `SM_*`/`RM_*`/`CM_*`
/// （已逐条对账），故此处只补 `Grobal2Const` **没有**的那几个：
/// `Envir.pas` 的密名旗标、`M2Definition.pas` 的颜色别名、`LogManage.pas` 的日志动作码。
/// </summary>
public static class PlayerSurfaceServerSend2Const
{
    /// <summary>原文 `Envir.pas:509 SecretFlag_ShowEqualName = 8; // 统一名字`。
    /// 本片调用点：38868-38869（`ServerSendOpenHealth` 判 `Obj.m_PEnvir.m_nSecretFlag[2]`）。</summary>
    public const int SecretFlag_ShowEqualName = 8;

    /// <summary>原文 `M2Definition.pas` 的 `TMsgColor.c_Red`（`= 0`；
    /// 托管 `Engine/EngineEnums.cs:21` 同名枚举值）。原文在
    /// `SysMsg(..., c_Red, t_Hint)` 里用它，而托管 `SysMsg` 的**四参**重载
    /// （`Core6.cs:626`）收 `int`，故此处给出数值形态。</summary>
    public const int c_Red = 0;

    /// <summary>原文 `M2Definition.pas` 的 `TMsgType.t_Hint`（`= 1`；
    /// 托管 `Engine/EngineEnums.cs:11`）。</summary>
    public const TMsgType t_Hint = TMsgType.t_Hint;

    /// <summary>原文 `M2Share.pas:87 LOG_ActionNone = 00;`（本片调用点：40702）。</summary>
    public const byte LOG_ActionNone = 0;

    /// <summary>原文 `M2Share.pas:105 LOG_ItemTakeBack = 18; // 取回物品`（本片调用点：40702）。</summary>
    public const byte LOG_ItemTakeBack = 18;

    /// <summary>原文 `M2Share.pas:104 LOG_ItemPutInto = 17; // 放入物品`
    /// （原文 40662 用；所属方法整体未移植，常量按原文补上）。</summary>
    public const byte LOG_ItemPutInto = 17;

    /// <summary>原文 `M2Share.pas:110 LOG_ItemBuyPrice = 23; // 买入费用`
    /// （原文 40100 一带用；所属方法整体未移植）。</summary>
    public const byte LOG_ItemBuyPrice = 23;

    /// <summary>原文 `M2Share.pas:116 LOG_GoldChange = 50; // 金币改变`。</summary>
    public const byte LOG_GoldChange = 50;

    /// <summary>原文 `M2Share.pas:117 LOG_GameGoldChange = 51; // 元宝改变`。</summary>
    public const byte LOG_GameGoldChange = 51;

    /// <summary>原文 `M2Share.pas` 的 `LOG_GamePointChange`。
    /// ⚠ **未直接对账**：由 `M2Share.pas:116=50` / `:117=51` 的相邻关系推为 52，
    /// **未经原文行逐字确认**。只被 `ServerSendHeroM2BuyUserItem`（本片已整体留痕）
    /// 使用，**不影响任何已移植方法**。</summary>
    public const byte LOG_GamePointChange = 52;

    /// <summary>原文 `M2Share.pas` 的 `LOG_GameDiamondChange`。
    /// ⚠ 同上，推测 53，未直接对账；只被已留痕方法使用。</summary>
    public const byte LOG_GameDiamondChange = 53;

    /// <summary>原文 `M2Share.pas` 的 `LOG_GameGirdChange`。
    /// ⚠ 同上，推测 54，未直接对账；只被已留痕方法使用。</summary>
    public const byte LOG_GameGirdChange = 54;
}

/// <summary>
/// 原文 38860-41077 区间引用到的**全局字符串**（原文都在 `M2Share.pas` 的单元级
/// `var` 段，托管侧对应单元尚未移植）。
///
/// ⚠ 与 <see cref="PlayerSurfaceServerSend2Seams"/> 里那几个 `Func&lt;string&gt;` 的分工：
/// 本类成员是**可写属性**（宿主/测试直接赋值），接缝里的是**取值委托**
/// （宿主可接线到任意数据源）。`g_ItemDescListText` 一族在原文就是"引擎缓存的文本块"，
/// 用属性更贴近"单元级变量"语义；骑马/摆摊那批是"配置投影"，用委托更合适。
/// </summary>
public static class PlayerSurfaceServerSend2Globals
{
    /// <summary>原文 `g_ItemDescListText: string`（`M2Share.pas`；本片调用点：
    /// 39655 / 39657 / 39658）。原文由 `SendClientDataFile` 一族填充。</summary>
    public static string g_ItemDescListText { get; set; } = "";

    /// <summary>原文 `g_ItemDescTopListText: string`（本片调用点：39668 / 39670 / 39671）。</summary>
    public static string g_ItemDescTopListText { get; set; } = "";

    /// <summary>原文 `g_TzItemDescListText: string`（本片调用点：39681 / 39682）。</summary>
    public static string g_TzItemDescListText { get; set; } = "";

    /// <summary>原文 `g_NameFilterListText: string`（本片调用点：39691 / 39692）。</summary>
    public static string g_NameFilterListText { get; set; } = "";
}

/// <summary>
/// 原文 `Move(@rec, Buf[...], SizeOf(rec))` / `SizeOf(Txxx)` 在托管侧的显式落点。
///
/// **为什么必须显式**：Delphi 的 `Move` 按内存布局逐字节拷贝，`SizeOf` 是编译器
/// 算出的**实际字节数**；托管结构体（`[StructLayout(LayoutKind.Sequential, Pack = 1)]`）
/// 的字节数必须与之一致，**否则报文长度就会错**（而 `SendSocketEx` 的长度参数
/// 正是取自这些值）。故本类每个方法都把"字段表 + 字节数"写在注释里，
/// 并用 `StructBytes.SizeOf&lt;T&gt;()`（`GXX.Core/Protocol/ShortStr.cs:77`）
/// 从托管结构体**实测**得到长度 —— 与原文同一个来源。
/// </summary>
public static class PlayerSurfaceServerSend2Structs
{
    // ------------------------------------------------------------------
    // 一、原文 `TWAbility`（`Grobal2.pas:3961-3966`）
    // ------------------------------------------------------------------
    //
    //   TWAbility = record
    //     dwExp:  LongWord; // 怪物经验值(Dword)
    //     wHP:    Integer;
    //     wMP:    Integer;
    //     wMaxHP: Integer;
    //     wMaxMP: Integer end;
    //
    //   ★ 原文如此（**不修**）：字段名带 `w` 前缀（暗示 Word），但声明的是
    //     **`Integer`（4 字节）** ⇒ `SizeOf(TWAbility) = 4 + 4*4 = 20`。
    //     托管侧按 `int` 复刻，**不能**按 `Word` 收窄（那会变成 12 字节）。
    //
    //   本片调用点：38878（`ServerSendOpenHealth` 的 `EncodeBuffer(@Ability, SizeOf(Ability))`）。

    /// <summary>原文 `TWAbility` 的托管等价（`Pack = 1`，5 个 4 字节字段 ⇒ 20 字节）。</summary>
    [System.Runtime.InteropServices.StructLayout(
        System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
    public struct TWAbility
    {
        /// <summary>原文 `dwExp: LongWord`（:38870 / :38872 赋值）。</summary>
        public uint dwExp;
        /// <summary>原文 `wHP: Integer`（:38874；★ 名为 w、实为 4 字节）。</summary>
        public int wHP;
        /// <summary>原文 `wMP: Integer`（:38875）。</summary>
        public int wMP;
        /// <summary>原文 `wMaxHP: Integer`（:38876）。</summary>
        public int wMaxHP;
        /// <summary>原文 `wMaxMP: Integer`（:38877）。</summary>
        public int wMaxMP;
    }

    /// <summary>
    /// 原文 38870-38877 的 5 次赋值 + 38878 的 `EncodeBuffer(@Ability, SizeOf(Ability))`。
    /// 形参顺序与原文**赋值顺序**一致：`dwExp`（:38870/:38872）、`wHP`/`wMP`/`wMaxHP`/`wMaxMP`
    /// （:38874-38877）。⚠ `Name` 是 4 字节 `Integer`，故形参用 `int`；调用方从
    /// `m_WAbil.HP` 等 `uint` 字段传值时做 `unchecked((int)...)`（位模式与 Delphi 一致）。
    /// </summary>
    public static byte[] WAbilityBytes(uint dwExp, int wHP, int wMP, int wMaxHP, int wMaxMP)
    {
        TWAbility a = default;
        a.dwExp = dwExp;
        a.wHP = wHP;
        a.wMP = wMP;
        a.wMaxHP = wMaxHP;
        a.wMaxMP = wMaxMP;
        return StructBytes.BytesOf(a);
    }

    // ------------------------------------------------------------------
    // 二、原文 `TCharDesc`（托管 `Grobal2.Types1.cs:203`）
    // ------------------------------------------------------------------
    //
    //   托管：`Feature: Byte` / `Status: Int64` / `MagicLevel: Integer`。
    //   ⇒ 长度必须取自 `StructBytes.SizeOf<TCharDesc>()`，**不能写死**
    //     （原文的 `SizeOf(TCharDesc)` 同样是编译器实测值）。

    /// <summary>`SizeOf(TCharDesc)`（取自托管结构体实测值，与原文同源）。</summary>
    public static int SizeOfTCharDesc => StructBytes.SizeOf<TCharDesc>();

    /// <summary>
    /// 原文 `Move(CharDesc, sSendMsg[...], SizeOf(TCharDesc))` 的托管等价：
    /// 只写原文真正赋过的**两个**字段（`Feature` :38899/:38924、`Status` :38900/:38925），
    /// 其余字段保持默认 0。
    /// ⚠ **已登记的口径差异**：原文 `CharDesc` 是**未初始化的局部变量**，
    /// 未赋值字段（托管侧多出的 `MagicLevel`）在原文里是**栈垃圾**；托管侧无法复刻
    /// 未初始化内存，按全 0 落地。报文长度不受影响（只影响该 4 字节的内容）。
    /// </summary>
    public static byte[] TCharDescBytes(byte feature, long status)
    {
        TCharDesc d = default;
        d.Feature = feature;
        d.Status = status;
        return StructBytes.BytesOf(d);
    }

    // ------------------------------------------------------------------
    // 三、原文 `TMessageBodyWL`（`Grobal2.pas:3451-3456`）
    // ------------------------------------------------------------------
    //
    //   TMessageBodyWL = packed record
    //     lParam1: Integer;
    //     lParam2: Integer;
    //     lTag1:   Integer;
    //     lTag2:   Int64; // 64位修改 2021-01-04
    //   end;
    //
    //   托管 `Grobal2.Types1.cs:146` 逐字同构（`Pack = 1`，4+4+4+8 = 20 字节）。
    //   本片调用点：39082（`EncodeBuffer`）、39206（`Move`）、39724（`SendSocketEx`）。

    /// <summary>原文 `Move(MessageBodyWL, ...)` / `SendSocketEx(@MessageBodyWL, SizeOf(...))` 的托管等价。</summary>
    public static byte[] MessageBodyWLBytes(in TMessageBodyWL body) => StructBytes.BytesOf(body);

    // ------------------------------------------------------------------
    // 四、原文 `TMessageBodyL`（托管 `Grobal2.Types1.cs:139`）
    // ------------------------------------------------------------------
    //
    //   TMessageBodyL = packed record lParam1: Integer; lParam2: Integer; end;
    //
    //   本片调用点：39501（`ServerSendAddEffectPlay` 的 `SendSocketEx(@AddData, SizeOf(AddData))`）。

    /// <summary>原文 `SendSocketEx(@m_DefMsg, @AddData, SizeOf(AddData))` 的托管等价（8 字节）。</summary>
    public static byte[] MessageBodyLBytes(in TMessageBodyL body) => StructBytes.BytesOf(body);

    // ------------------------------------------------------------------
    // 五、原文 `TShortMessage`（托管 `Grobal2.Types1.cs:177`）
    // ------------------------------------------------------------------
    //
    //   TShortMessage = packed record Ident: Word; wMsg: Word; end;
    //
    //   本片调用点：39484（`EncodeBuffer(@ShortMessage, SizeOf(TShortMessage))`）。

    /// <summary>原文 `EncodeBuffer(@ShortMessage, SizeOf(TShortMessage))` 的托管等价（4 字节）。</summary>
    public static byte[] ShortMessageBytes(in TShortMessage msg) => StructBytes.BytesOf(msg);

    // ------------------------------------------------------------------
    // 六、原文 `TAbilityAlcohol`（`Grobal2.pas:3807-3814`）
    // ------------------------------------------------------------------
    //
    //   TAbilityAlcohol = packed record // 酒属性
    //     Alcohol:          Word;    // 酒量
    //     MaxAlcohol:       Word;    // 酒量上限
    //     WineDrinkValue:   Word;    // 醉酒度
    //     MedicineLevel:    Integer; // 药力值等级
    //     MedicineValue:    Word;    // 当前药力值
    //     MaxMedicineValue: Word;    // 药力值上限
    //   end;
    //
    //   托管 `Grobal2.Types3.cs:33` 逐字同构（`Pack = 1`，2+2+2+4+2+2 = 14 字节）。
    //   本片调用点：39609。

    /// <summary>原文 `SendSocketEx(@m_DefMsg, @m_Alcohol, SizeOf(TAbilityAlcohol))` 的托管等价。</summary>
    public static byte[] AbilityAlcoholBytes(in TAbilityAlcohol v) => StructBytes.BytesOf(v);

    // ------------------------------------------------------------------
    // 七、原文 `TClientAbilityNG`（托管 `Grobal2.Types3.cs:21`）
    // ------------------------------------------------------------------
    //
    //   比 `TAbilityNG` 多出 `NGDamage`/`UnNGDamage` 两个 `Integer`。
    //   本片调用点：39603。

    /// <summary>原文 `SendSocketEx(@ClientAbilityNG, SizeOf(ClientAbilityNG))` 的托管等价。</summary>
    public static byte[] ClientAbilityNGBytes(in TClientAbilityNG v) => StructBytes.BytesOf(v);

    // ------------------------------------------------------------------
    // 八、原文 `TClientItem`（托管 `Grobal2.Types2.cs:216`）
    // ------------------------------------------------------------------
    //
    //   托管首字段是 `TStdItem s`（`Grobal2.Types2.cs:219`），原文
    //   `ClientItem.S.Price` / `.Reserved1` / `.Stock` 在托管侧即 `ClientItem.s.*`。
    //   本片调用点：39937-39940。

    /// <summary>`SizeOf(TClientItem)`（取自托管结构体实测值）。</summary>
    public static int SizeOfTClientItem => StructBytes.SizeOf<TClientItem>();

    /// <summary>原文 `EncodeBuffer(@ClientItem, SizeOf(TClientItem))` 的托管等价。</summary>
    public static byte[] TClientItemBytes(in TClientItem item) => StructBytes.BytesOf(item);

    /// <summary>
    /// 原文 `UserItemToClientItem(UserItem, StdItem, @ClientItem, True, True)`
    /// 的接缝返回值（字节块）反解回 `TClientItem`。**字节数不足时返回 `default`**
    /// （原文该调用把记录**就地写满**，不会长度不足；接缝未接线时由本方法兜底）。
    /// </summary>
    public static TClientItem TClientItemFromBytes(byte[] bytes)
    {
        TClientItem item = default;
        if (bytes == null || bytes.Length < SizeOfTClientItem) return item;
        return StructBytes.FromBytes<TClientItem>(bytes);
    }
}
