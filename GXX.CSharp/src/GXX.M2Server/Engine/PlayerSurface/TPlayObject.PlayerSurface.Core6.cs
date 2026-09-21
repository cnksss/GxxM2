// ============================================================================
// 源单元：Source\M2Engine\ObjPlayer.pas（GBK；UTF-8 镜像 49,232 LF）
// 本文件：**TPlayObject 核心片 6**（车道 p13-m2-objplayer 的切片之一）。
//
// 覆盖原文行号范围：**12689 – 14898**（按镜像实测方法声明区间 12688 – 14897）。
// 该区间内共 **49** 条方法声明；其中 2 条按任务书**不重移植**：
//   · SendUpdateItem            （ObjPlayer.pas:12688-12709）—— 任务书第 4 条：同单元另一片已负责
//   · GetMaxBagCount            （ObjPlayer.pas:13677-13682）—— 已在 TCreature.PlayerSurface.Items.cs:419
// 因此**本文件处理 47 条**方法。
//
// 三方统计（本文件）：
//   **真实体 31 / NotPorted 16 / 原文如此 4**
//   · 真实体 31 = 17 条 SendUpdateItem* 族 + 4 条 SysMsg/SysMsgEx + InitSpeed + BaseObjectMove
//                + GetUserItemHandWeight + SendAddMagic + 4 条 交易/挑战报文 + OpenDealDlg/OpenChallengeDlg
//   · NotPorted 16 = 见每个方法 XML 文档中列出的「缺失成员 + ObjPlayer.pas 行号」
//   · 原文如此 4 = ①SM_UPDATEITEM_HEROM2LIGHT 与 SM_UPDATEITEM_INSURANCECOUNT 取值相同(10330)
//                  ②SendDelDealItem 把 SM_DEALREMOTEDELITEM 发给**自己**（而 SendAddDealItem 发给对方）
//                  ③SendUpdateItemPropertyText **不**做 EncodeString（而 SendUpdateItemName 做）
//                  ④SysMsg/SysMsgEx 四重载的 boAddPrefix 参数在方法体内**从未被使用**
//
// 约定（与 PortKit / 既有 PlayerSurface 片一致，未另造）：
//   · 每个方法体首行注释写 `// 原文 NNNN-NNNN`
//   · 报文装配仍逐行移植；发送落点走 PlayerSurfaceSocketSeams / PlayerSurfaceMsgSeams
//   · 未移植方法体**必须**调用 PortNotPorted(nameof(X), 行号)（禁止裸 `=> true;`）
//     ⚠ 有返回值的方法额外写一行 `return default;` —— 这是 C# 编译器的硬性要求
//       （PortNotPorted 不是 return 语句），**不是**静默桩：留痕已写，语义见各自 XML 文档。
//   · 本文件新增字段全部来自 **ObjPlayer.pas 自身的类声明段**（行号已逐条标注）；
//     其它单元（ObjBase.pas / TSmartObject）的成员一律**不冒充**，走接缝或 PortNotPorted。
//
// ★ 已核对的「不重复声明」清单（grep 证据）：
//   · m_UseItems            → Engine/RecalcChain.cs:120（TUserItem?[]）
//   · m_MagicList           → Engine/ObjBase.cs:197（List<THumMagic>）
//   · m_boOffLine / m_boDummyObject → Engine/ObjBase.OnlineMsg.cs:10/13
//   · m_boImprison          → Engine/MagicBatchH.cs:135
//   · m_sCharName / m_sMapName / m_PEnvir / m_nRecogId → Engine/ObjBase.cs
//   · m_DefMsg / SendSocketRef / SendSocketExRef / PortNotPorted → ...PlayerSurface.PortKit.cs
//   · m_nGold / m_nGoldMax / m_dwRecordBeadExp → ...PlayerSurface.Gold.cs
//   · m_boDealing / m_DealCreat / m_DealLastTick → 由并行片 Core3.cs 声明（Core1.cs:461-462 已让渡；**交付时尚未落地**，见下）
//   · m_MyGuild/m_nGuildRankNo/m_nMemberType/m_nMemberLevel 的接缝 →
//     PlayerSurfaceItemSeams.MyGuild / GuildRankNo / MemberType / MemberLevel
// ============================================================================

using System;
using GXX.Core.Protocol;
using GXX.Core.Rtl;

namespace GXX.M2Server.Engine;

/// <summary>
/// 核心片 6 的**跨单元/跨片接缝**（原文成员不在本切片内，或属于尚未移植的单元）。
///
/// 与本车道既有接缝同型（<c>PlayerSurfaceMsgSeams</c> / <c>PlayerSurfaceItemSeams</c>）：
/// 只暴露**委托**，默认实现 = 「无宿主」，由集成方在宿主就绪后注入。
/// ★ 接缝的是「投递/编码」，**报文装配与分支顺序仍逐行移植**在各方法体内。
/// </summary>
public static class PlayerSurfaceCore6Seams
{
    /// <summary>
    /// 原文 `SendAddMagic` 内 `UserMagicToClientMagic(UserMagic, @ClientMagic)`
    /// （ObjPlayer.pas:13661）—— 该成员属 **ObjBase.pas / TSmartObject 面**，托管侧未移植。
    /// 默认：返回全零的 <see cref="TClientMagic"/>（等同「未编码」）。
    /// </summary>
    public static Func<THumMagic, TClientMagic> UserMagicToClientMagic { get; set; }
        = _ => default;

    /// <summary>
    /// 原文 `CustomMagicConfig.ServerConfig.FailMsg`（ObjPlayer.pas:13670）的非空判定。
    /// ⚠ 托管 `Engine.TCustomMagicConfig.ServerConfig`（MagicBatchH.cs:48 `TCustomMagicServerConfig`）
    /// **没有** `FailMsg` 字段（Forms 侧 `CustomMagicModel.cs:190` 的 `FailMsg` 属另一个类型），
    /// 故用本接缝代理「有提示的自定义技能」判定。默认 false（= 原文 FailMsg 为空串的情形）。
    /// </summary>
    public static Func<ushort, bool> CustomMagicHasFailMsg { get; set; } = _ => false;

    /// <summary>
    /// 原文 `TBaseObject.SpaceMove(sMapName, nX, nY, nInt)`（ObjBase.pas:748，未移植）——
    /// `BaseObjectMove` 的落点。默认：无宿主，丢弃。
    /// </summary>
    public static Action<TPlayObject, string, int, int, int> SpaceMove { get; set; }
        = (_, _, _, _, _) => { };

    /// <summary>
    /// 原文 `TBaseObject.MapRandomMove(sMapName, nInt)`（ObjBase.pas:749，未移植）。
    /// 默认：无宿主，丢弃。
    /// </summary>
    public static Action<TPlayObject, string, int> MapRandomMove { get; set; }
        = (_, _, _) => { };

    /// <summary>
    /// 原文 `TPlayObject.GetBackDealItems`（ObjPlayer.pas:7777，属同单元**另一片**，尚未移植）——
    /// `OpenDealDlg` 的第一步。默认：无宿主，丢弃。
    /// </summary>
    public static Action<TPlayObject> GetBackDealItems { get; set; } = _ => { };

    /// <summary>
    /// 原文 `TPlayObject.GetBackChallengeItems`（ObjPlayer.pas:7794，属同单元**另一片**，尚未移植）。
    /// 默认：无宿主，丢弃。
    /// </summary>
    public static Action<TPlayObject> GetBackChallengeItems { get; set; } = _ => { };

    /// <summary>
    /// 原文 `TPlayObject(m_DealCreat).SendSocketEx(@m_DefMsg, @ClientItem, SizeOf(TClientItem))`
    /// （ObjPlayer.pas:14302）与挑战版的 :14366 —— **跨对象**发送：报文在**自己**身上装配
    /// （`nRecog = NativeInt(Self)`），但由**对方**的 SendSocketEx 投递。
    /// 既有 <c>PlayerSurfaceSocketSeams.SendSocketEx</c> 的第二参是 `byte[]`（此处是未编码的
    /// `TClientItem` 物件，经 `PlayerSurfaceItemSeams.UserItemToClientItem` 产出 `object?`），
    /// 签名不匹配，故在此单列。默认：无宿主，丢弃。
    /// </summary>
    public static Action<TPlayObject, TDefaultMessage, object?> SendSocketExTo { get; set; }
        = (_, _, _) => { };

    /// <summary>
    /// 原文 `inherited SysMsgEx(sMsg, MsgColor, MsgType, boAddPrefix)` 的基类落点
    /// （`TBaseObject.SysMsgEx`，ObjBase.pas）。托管 `TCreature` 只有三参 `SysMsg`
    /// （`ObjBase.OnlineMsg.cs:25`，落点为进程内 `SysMsgs` 列表），没有 `SysMsgEx` ——
    /// 默认即转调该三参版（两者在托管侧的落点相同）。
    /// </summary>
    public static Action<TCreature, string, TMsgColor, TMsgType, bool> BaseSysMsgEx { get; set; }
        = (self, msg, color, msgType, _) => self.SysMsg(msg, color, msgType);

    /// <summary>恢复全部默认实现（单测隔离用）。</summary>
    public static void ResetDefaults()
    {
        UserMagicToClientMagic = _ => default;
        CustomMagicHasFailMsg = _ => false;
        SpaceMove = (_, _, _, _, _) => { };
        MapRandomMove = (_, _, _) => { };
        GetBackDealItems = _ => { };
        GetBackChallengeItems = _ => { };
        SendSocketExTo = (_, _, _) => { };
        BaseSysMsgEx = (self, msg, color, msgType, _) => self.SysMsg(msg, color, msgType);
    }
}

/// <summary>
/// ObjPlayer.pas 12689-14898 的移植体（核心片 6）。
/// </summary>
public partial class TPlayObject
{
    // ==================================================================
    // 字段（**全部**取自 ObjPlayer.pas 自身的类声明段，行号逐条标注）
    // ==================================================================

    /// <summary>原文 `m_IsSendUserLogon: Boolean;`（ObjPlayer.pas:499）——是否已经下发过登录包。
    /// `SysMsg` / `SysMsgEx` 靠它决定「自己发」还是「走基类」。</summary>
    public bool m_IsSendUserLogon;

    /// <summary>
    /// 原文 `m_boDealing: Boolean; // 0x317`（ObjPlayer.pas:31）、
    /// `m_DealLastTick: LongWord; // 0x318`（:32）、`m_DealCreat: TPlayObject; // 0x31C`（:33）
    /// —— **本文件不声明**：并行片 `PlayerSurface.Core1.cs` 明确把这三个字段让给了
    /// `PlayerSurface.Core3.cs`（Core1.cs:461-462 的注记：「它们由并行车道 PlayerSurface.Core3.cs 声明」），
    /// 本文件与 Core1 一样**只使用**（避免 CS0102）。
    /// ⚠ **交付时状态**：截至本文件写完，`Core3.cs` 尚未落地这三个字段声明
    /// （`m_boDealing`/`m_DealCreat`/`m_DealLastTick` 当前在整个 src 树里无声明）——
    /// 这会让本文件与 `Core1.cs`（:2457-2480）同时 CS0103。已作为**跨片阻塞项**上报调度方：
    /// 必须由 Core3 落地这三个字段（或由调度方指定唯一声明方），本文件**不重复声明**。
    /// </summary>

    /// <summary>原文 `m_boChallengeing: Boolean; // 0x317`（ObjPlayer.pas:39）——正在挑战。</summary>
    public bool m_boChallengeing;

    /// <summary>原文 `m_ChallengeLastTick: LongWord; // 0x318`（ObjPlayer.pas:40）——挑战最后操作时间。</summary>
    public uint m_ChallengeLastTick;

    /// <summary>原文 `m_ChallengeCreat: TPlayObject; // 0x31C`（ObjPlayer.pas:41）——挑战者。</summary>
    public TPlayObject? m_ChallengeCreat;

    /// <summary>原文 `m_boTimeRecall: Boolean; // 0x684`（ObjPlayer.pas:129）——定时回城标记。
    /// `BaseObjectMove` 在「换了地图且自己是玩家」时置 False。</summary>
    public bool m_boTimeRecall;

    /// <summary>原文 `m_nCheckMoveCount: Integer;`（ObjPlayer.pas:347）。</summary>
    public int m_nCheckMoveCount;

    /// <summary>原文 `m_nCheckAttackCount: Integer;`（ObjPlayer.pas:348）。</summary>
    public int m_nCheckAttackCount;

    /// <summary>原文 `m_nCheckMagicAttackCount: Integer;`（ObjPlayer.pas:349）。</summary>
    public int m_nCheckMagicAttackCount;

    /// <summary>原文 `m_nMoveCount: Integer;`（ObjPlayer.pas:350）。</summary>
    public int m_nMoveCount;

    /// <summary>原文 `m_nAttackCount: Integer;`（ObjPlayer.pas:351）。</summary>
    public int m_nAttackCount;

    /// <summary>原文 `m_nMagicAttackCount: Integer;`（ObjPlayer.pas:352）。</summary>
    public int m_nMagicAttackCount;

    /// <summary>原文 `m_dwCheckMoveTick: LongWord;`（ObjPlayer.pas:353）。</summary>
    public uint m_dwCheckMoveTick;

    /// <summary>原文 `m_dwCheckAttackTick: LongWord;`（ObjPlayer.pas:354）。</summary>
    public uint m_dwCheckAttackTick;

    /// <summary>原文 `m_dwCheckMagicAttackTick: LongWord;`（ObjPlayer.pas:355）。</summary>
    public uint m_dwCheckMagicAttackTick;

    // ==================================================================
    // 常量（原文 `SizeOf(...)` 的实测值；结构定义见 Common\Grobal2.pas 3662/3676/3701）
    // ==================================================================

    /// <summary>原文 `TFluteInfo = packed record GemIndex, GemCount: Word; end;`（Grobal2.pas:3701）→ 4 字节。</summary>
    private const int CORE6_SIZEOF_FLUTE = 4;

    /// <summary>原文 `UserItem.Flutes: array [0 .. 7] of TFluteInfo`（`SendUpdateItemFlute` 的 `SizeOf(UserItem.Flutes)`）→ 32 字节。</summary>
    private const int CORE6_FLUTE_COUNT = 8;

    /// <summary>原文 `TUserItemProgress = packed record`（Grobal2.pas:3662）→ 42 字节。</summary>
    private const int CORE6_SIZEOF_PROGRESS = 42;

    /// <summary>原文 `TCustomProperty = packed record`（Grobal2.pas:3676）→ 17 字节。</summary>
    private const int CORE6_SIZEOF_CUSTOM_PROPERTY = 17;

    /// <summary>原文 `TCustomProperty.Properties: array [0 .. 19]`（Grobal2.pas:3687 起）→ 20 槽。</summary>
    private const int CORE6_CUSTOM_PROPERTY_COUNT = 20;

    // ==================================================================
    // SendUpdateItem* 族（原文 12710-12866）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUpdateItemName(nWhere: SmallInt; MakeIndex: Integer;
    /// IsBoxItem: Boolean; NewItemName: string);`（ObjPlayer.pas:12710-12717）。
    /// </summary>
    /// <remarks>
    /// 逐行：`IsHero := False;`（12714）→ `MakeDefaultMsg(SM_UPDATEITEM_NAME, MakeIndex, nWhere,
    /// Integer(IsHero) shl 1 or Integer(IsBoxItem), 0)`（12715）→ `SendSocket(@m_DefMsg, EncodeString(NewItemName))`（12716）。
    /// ★ 只有本条与 <see cref="SendUpdateItemPropertyText"/> 处理字符串，且**只有本条做 EncodeString**
    /// （见「原文如此 ③」）。
    /// ⚠ 形参 `MakeIndex` 传的是 **nRecog**（`MakeDefaultMsg` 第二参），`nWhere` 传 **wParam**（第三参）。
    /// </remarks>
    public void SendUpdateItemName(short nWhere, int makeIndex, bool isBoxItem, string newItemName)
    {
        // 原文 12714：IsHero := False;
        bool isHero = false;
        // 原文 12715
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_UPDATEITEM_NAME,
            makeIndex, nWhere, (isHero ? 1 : 0) << 1 | (isBoxItem ? 1 : 0), 0);
        // 原文 12716：SendSocket(@m_DefMsg, EncodeString(NewItemName));
        SendSocketRef(m_DefMsg, EDcode.EncodeStringText(newItemName));
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUpdateItemColor(nWhere: SmallInt; MakeIndex: Integer;
    /// IsBoxItem: Boolean; Color: Byte);`（ObjPlayer.pas:12719-12726）。
    /// </summary>
    public void SendUpdateItemColor(short nWhere, int makeIndex, bool isBoxItem, byte color)
    {
        // 原文 12723：IsHero := False;
        bool isHero = false;
        // 原文 12724
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_UPDATEITEM_COLOR,
            makeIndex, nWhere, (isHero ? 1 : 0) << 1 | (isBoxItem ? 1 : 0), color);
        // 原文 12725：SendSocket(@m_DefMsg, '');
        SendSocketRef(m_DefMsg, "");
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUpdateItemDura(nWhere: SmallInt; MakeIndex: Integer;
    /// IsBoxItem: Boolean; Dura: Word);`（ObjPlayer.pas:12728-12735）。
    /// </summary>
    public void SendUpdateItemDura(short nWhere, int makeIndex, bool isBoxItem, ushort dura)
    {
        // 原文 12732
        bool isHero = false;
        // 原文 12733
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_UPDATEITEM_DURA,
            makeIndex, nWhere, (isHero ? 1 : 0) << 1 | (isBoxItem ? 1 : 0), dura);
        // 原文 12734
        SendSocketRef(m_DefMsg, "");
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUpdateItemDuraMax(...; DuraMax: Word);`（ObjPlayer.pas:12737-12744）。
    /// </summary>
    public void SendUpdateItemDuraMax(short nWhere, int makeIndex, bool isBoxItem, ushort duraMax)
    {
        // 原文 12741
        bool isHero = false;
        // 原文 12742
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_UPDATEITEM_DURAMAX,
            makeIndex, nWhere, (isHero ? 1 : 0) << 1 | (isBoxItem ? 1 : 0), duraMax);
        // 原文 12743
        SendSocketRef(m_DefMsg, "");
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUpdateItemUpgradeCount(...; UpgradeCount: Byte);`（ObjPlayer.pas:12746-12754）。
    /// </summary>
    public void SendUpdateItemUpgradeCount(short nWhere, int makeIndex, bool isBoxItem, byte upgradeCount)
    {
        // 原文 12750
        bool isHero = false;
        // 原文 12751-12752
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_UPDATEITEM_UPGRADECOUNT,
            makeIndex, nWhere, (isHero ? 1 : 0) << 1 | (isBoxItem ? 1 : 0), upgradeCount);
        // 原文 12753
        SendSocketRef(m_DefMsg, "");
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUpdateItemNewLook(...; NewLook: Word);`（ObjPlayer.pas:12756-12763）。
    /// </summary>
    public void SendUpdateItemNewLook(short nWhere, int makeIndex, bool isBoxItem, ushort newLook)
    {
        // 原文 12760
        bool isHero = false;
        // 原文 12761
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_UPDATEITEM_NEWLOOK,
            makeIndex, nWhere, (isHero ? 1 : 0) << 1 | (isBoxItem ? 1 : 0), newLook);
        // 原文 12762
        SendSocketRef(m_DefMsg, "");
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUpdateItemNewShape(...; NewShape: Word);`（ObjPlayer.pas:12765-12772）。
    /// </summary>
    public void SendUpdateItemNewShape(short nWhere, int makeIndex, bool isBoxItem, ushort newShape)
    {
        // 原文 12769
        bool isHero = false;
        // 原文 12770
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_UPDATEITEM_NEWSHAPE,
            makeIndex, nWhere, (isHero ? 1 : 0) << 1 | (isBoxItem ? 1 : 0), newShape);
        // 原文 12771
        SendSocketRef(m_DefMsg, "");
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUpdateItemHeroM2Light(...; HeroM2Light: Byte);`（ObjPlayer.pas:12774-12782）。
    /// </summary>
    /// <remarks>★ 原文如此 ①：本条与 <see cref="SendUpdateItemInsuranceCount"/> 用的
    /// `SM_UPDATEITEM_HEROM2LIGHT` / `SM_UPDATEITEM_INSURANCECOUNT` 在 `Grobal2.pas` 里**取值相同（均 10330）**，
    /// 即两条报文在客户端无法区分 —— 照抄，不修（差异断言见 `ObjPlayerCore6Tests`）。</remarks>
    public void SendUpdateItemHeroM2Light(short nWhere, int makeIndex, bool isBoxItem, byte heroM2Light)
    {
        // 原文 12778
        bool isHero = false;
        // 原文 12779-12780
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_UPDATEITEM_HEROM2LIGHT,
            makeIndex, nWhere, (isHero ? 1 : 0) << 1 | (isBoxItem ? 1 : 0), heroM2Light);
        // 原文 12781
        SendSocketRef(m_DefMsg, "");
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUpdateItemInsuranceCount(...; InsuranceCount: Word);`
    /// （ObjPlayer.pas:12784-12793）。★ 见 <see cref="SendUpdateItemHeroM2Light"/> 的「原文如此 ①」。
    /// </summary>
    public void SendUpdateItemInsuranceCount(short nWhere, int makeIndex, bool isBoxItem, ushort insuranceCount)
    {
        // 原文 12789
        bool isHero = false;
        // 原文 12790-12791
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_UPDATEITEM_INSURANCECOUNT,
            makeIndex, nWhere, (isHero ? 1 : 0) << 1 | (isBoxItem ? 1 : 0), insuranceCount);
        // 原文 12792
        SendSocketRef(m_DefMsg, "");
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUpdateItemBind(...; IsBind: Boolean);`（ObjPlayer.pas:12795-12802）。
    /// ⚠ `Word(IsBind)`：Delphi `Boolean → Word` 是 **True=1 / False=0**（不是 $FFFF）。
    /// </summary>
    public void SendUpdateItemBind(short nWhere, int makeIndex, bool isBoxItem, bool isBind)
    {
        // 原文 12799
        bool isHero = false;
        // 原文 12800：Word(IsBind)
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_UPDATEITEM_BIND,
            makeIndex, nWhere, (isHero ? 1 : 0) << 1 | (isBoxItem ? 1 : 0), isBind ? 1 : 0);
        // 原文 12801
        SendSocketRef(m_DefMsg, "");
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUpdateItemLimitTime(nWhere: SmallInt; MakeIndex: Integer;
    /// LimitTime: Integer);`（ObjPlayer.pas:12804-12808）。
    /// ★ 本重载**没有** IsBoxItem 参数：`wTag := LoWord(LimitTime)`、`wSeries := HiWord(LimitTime)`
    /// （原文 12806 把 32 位限时拆成两个 16 位字段）。
    /// ⚠ `LoWord`/`HiWord` 作用在**有符号** Integer 上：负数按 32 位补码取低/高 16 位（托管侧用 ulong 位运算复刻）。
    /// </summary>
    public void SendUpdateItemLimitTime(short nWhere, int makeIndex, int limitTime)
    {
        // 原文 12806
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_UPDATEITEM_LIMITTIME,
            makeIndex, nWhere, PlayerSurfacePack.LoWord(limitTime), PlayerSurfacePack.HiWord(limitTime));
        // 原文 12807
        SendSocketRef(m_DefMsg, "");
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUpdateItemNewValue(...; ValueIndex: Byte; Value: Word);`
    /// （ObjPlayer.pas:12810-12815）。
    /// ⚠ `wTag := MakeWord(Byte(IsBoxItem), ValueIndex)` —— 本重载把 IsBoxItem 放在 **Tag 低字节**、
    /// ValueIndex 放在 **Tag 高字节**（与同族其余方法 `IsHero shl 1 or IsBoxItem` 的布局**不同**）。
    /// </summary>
    public void SendUpdateItemNewValue(short nWhere, int makeIndex, bool isBoxItem, byte valueIndex, ushort value)
    {
        // 原文 12813：MakeWord(Byte(IsBoxItem), ValueIndex)
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_UPDATEITEM_NEWVALUE,
            makeIndex, nWhere, PlayerSurfacePack.MakeWord(isBoxItem ? 1 : 0, valueIndex), value);
        // 原文 12814
        SendSocketRef(m_DefMsg, "");
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUpdateItemFlute(nWhere: SmallInt; IsBoxItem: Boolean;
    /// UserItem: pTUserItem);`（ObjPlayer.pas:12817-12825）。
    /// 尾数据 = `@UserItem.Flutes[0]`，长度 `SizeOf(UserItem.Flutes)` = 8×4 = **32 字节**。
    /// </summary>
    public void SendUpdateItemFlute(short nWhere, bool isBoxItem, TUserItem userItem)
    {
        // 原文 12821
        bool isHero = false;
        // 原文 12822-12823
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_UPDATEITEM_FLUTE,
            userItem.MakeIndex, nWhere, (isHero ? 1 : 0) << 1 | (isBoxItem ? 1 : 0), userItem.btFluteCount);
        // 原文 12824：SendSocketEx(@m_DefMsg, @UserItem.Flutes[0], SizeOf(UserItem.Flutes));
        SendSocketExRef(m_DefMsg, Core6FlutesToBytes(userItem));
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUpdateItemProgress(nWhere: SmallInt; IsBoxItem: Boolean;
    /// UserItem: pTUserItem; ProgressIndex: Byte);`（ObjPlayer.pas:12827-12835）。
    /// 尾数据 = `@UserItem.Progress[ProgressIndex]`，长度 `SizeOf(TUserItemProgress)` = **42 字节**。
    /// ⚠ `ProgressIndex` 是 `Byte`，原文数组只有 `[0..1]`：传 3..255 属**越界读**（原文缺陷，UB）。
    /// 托管侧无法复刻越界读，且**不发送**尾数据（见 <see cref="Core6ProgressBytes"/>）。
    /// </summary>
    public void SendUpdateItemProgress(short nWhere, bool isBoxItem, TUserItem userItem, byte progressIndex)
    {
        // 原文 12831
        bool isHero = false;
        // 原文 12832-12833
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_UPDATEITEM_PROGRESS,
            userItem.MakeIndex, nWhere, (isHero ? 1 : 0) << 1 | (isBoxItem ? 1 : 0), progressIndex);
        // 原文 12834
        SendSocketExRef(m_DefMsg, Core6ProgressBytes(userItem, progressIndex));
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUpdateItemPropertyText(...; sText: string);`（ObjPlayer.pas:12837-12844）。
    /// ★ 原文如此 ③：这里**直接** `SendSocket(@m_DefMsg, sText)`，**没有** `EncodeString`
    /// （对比 <see cref="SendUpdateItemName"/> 的 `EncodeString(NewItemName)`）—— 照抄，不修。
    /// </summary>
    public void SendUpdateItemPropertyText(short nWhere, int makeIndex, bool isBoxItem, string sText)
    {
        // 原文 12841
        bool isHero = false;
        // 原文 12842
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_UPDATEITEM_PROPERTYTEXT,
            makeIndex, nWhere, (isHero ? 1 : 0) << 1 | (isBoxItem ? 1 : 0), 0);
        // 原文 12843：SendSocket(@m_DefMsg, sText);   ← 不编码
        SendSocketRef(m_DefMsg, sText);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUpdateItemPropertyColor(...; Color: Byte);`（ObjPlayer.pas:12846-12853）。
    /// </summary>
    public void SendUpdateItemPropertyColor(short nWhere, int makeIndex, bool isBoxItem, byte color)
    {
        // 原文 12850
        bool isHero = false;
        // 原文 12851
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_UPDATEITEM_PROPERTYCOLOR,
            makeIndex, nWhere, (isHero ? 1 : 0) << 1 | (isBoxItem ? 1 : 0), color);
        // 原文 12852
        SendSocketRef(m_DefMsg, "");
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendUpdateItemPropertyValue(nWhere: SmallInt; IsBoxItem: Boolean;
    /// UserItem: pTUserItem; PropertyIndex: Byte);`（ObjPlayer.pas:12855-12865）。
    /// 尾数据 = `@UserItem.CustomProperty.Properties[PropertyIndex]`，长度 `SizeOf(TCustomProperty)` = **17 字节**。
    /// ⚠ `PropertyIndex` 是 `Byte`，原文数组是 `[0..19]`：≥20 属**越界读**（原文缺陷，UB）；
    /// 托管侧 `TUserItemProperty.GetProp(i)` 只在 0..19 有效，越界返回空尾数据（见 <see cref="Core6CustomPropertyBytes"/>）。
    /// </summary>
    public void SendUpdateItemPropertyValue(short nWhere, bool isBoxItem, TUserItem userItem, byte propertyIndex)
    {
        // 原文 12860
        bool isHero = false;
        // 原文 12861-12862
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_UPDATEITEM_PROPERTYVALUES,
            userItem.MakeIndex, nWhere, (isHero ? 1 : 0) << 1 | (isBoxItem ? 1 : 0), propertyIndex);
        // 原文 12863-12864
        SendSocketExRef(m_DefMsg, Core6CustomPropertyBytes(userItem, propertyIndex));
    }

    /// <summary>
    /// 原文 `@UserItem.Flutes[0]` + `SizeOf(UserItem.Flutes)`（ObjPlayer.pas:12824）的托管复刻：
    /// 逐元素 `StructToBytes`（`TFluteInfo` 是 packed record，4 字节）拼成连续 32 字节。
    /// </summary>
    private static byte[] Core6FlutesToBytes(TUserItem userItem)
    {
        var buf = new byte[CORE6_FLUTE_COUNT * CORE6_SIZEOF_FLUTE];
        for (int i = 0; i < CORE6_FLUTE_COUNT; i++)
        {
            var one = CustomNpcUtils.StructToBytes(userItem.GetFlute(i));
            Buffer.BlockCopy(one, 0, buf, i * CORE6_SIZEOF_FLUTE, CORE6_SIZEOF_FLUTE);
        }
        return buf;
    }

    /// <summary>
    /// 原文 `@UserItem.Progress[ProgressIndex]` + `SizeOf(UserItem.Progress[ProgressIndex])`
    /// （ObjPlayer.pas:12834）的托管复刻。
    /// ★ 原文缺陷（UB）：`ProgressIndex ≥ 2` 时原文读数组之外的内存；托管侧**不复刻**该越界读，
    /// 返回**空尾数据**（不发送尾数据），差异断言见
    /// `ObjPlayerCore6Tests.SendUpdateItemProgress_OutOfRangeIndex_SendsNoTail_OriginalWasUb`。
    /// </summary>
    private static byte[] Core6ProgressBytes(TUserItem userItem, byte progressIndex)
    {
        // 原文 12834：Progress[0] / Progress[1]（托管为 Progress0 / Progress1 两个具名字段）
        switch (progressIndex)
        {
            case 0: return CustomNpcUtils.StructToBytes(userItem.Progress0);
            case 1: return CustomNpcUtils.StructToBytes(userItem.Progress1);
            default: return Array.Empty<byte>();   // ★ 原文缺陷（越界 UB）—— 托管侧不发送尾数据
        }
    }

    /// <summary>
    /// 原文 `@UserItem.CustomProperty.Properties[PropertyIndex]` +
    /// `SizeOf(UserItem.CustomProperty.Properties[PropertyIndex])`（ObjPlayer.pas:12863-12864）的托管复刻。
    /// ★ 原文缺陷（UB）：`PropertyIndex ≥ 20` 时原文越界；托管侧返回空尾数据。
    /// </summary>
    private static byte[] Core6CustomPropertyBytes(TUserItem userItem, byte propertyIndex)
    {
        if (propertyIndex >= CORE6_CUSTOM_PROPERTY_COUNT)
            return Array.Empty<byte>();            // ★ 原文缺陷（越界 UB）—— 托管侧不发送尾数据

        var prop = userItem.CustomProperty.GetProp(propertyIndex);
        var bytes = CustomNpcUtils.StructToBytes(prop);
        // 双保险：packed 布局下应为 17 字节（Grobal2.Types2.cs:132 Pack=1）
        if (bytes.Length == CORE6_SIZEOF_CUSTOM_PROPERTY) return bytes;
        int n = Math.Min(bytes.Length, CORE6_SIZEOF_CUSTOM_PROPERTY);
        var trimmed = new byte[n];
        Buffer.BlockCopy(bytes, 0, trimmed, 0, n);
        return trimmed;
    }

    // ==================================================================
    // SysMsg / SysMsgEx（原文 12867-12990）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.SysMsg(sMsg: AnsiString; MsgColor: TMsgColor; MsgType: TMsgType;
    /// boAddPrefix: Boolean = True);`（ObjPlayer.pas:12867-12913）。
    /// </summary>
    /// <remarks>
    /// 逐行：① `m_boOffLine or m_boDummyObject → Exit`（12871-12872，注释「修改离线人物不发送」）；
    /// ② `m_IsSendUserLogon` 为真时：按 `MsgType` 加前缀（**唯一条件是 `g_Config.boShowPreFixMsg`**）→
    /// 按 `MsgColor`（`c_Green`/`c_Blue`，否则 `t_Cust` 用祝福色、其余用红字色）算 `Color` →
    /// `MakeDefaultMsg(SM_SYSMESSAGE, NativeInt(Self), Color, 0, 1)` → `SendSocket(@m_DefMsg, sMsg)`；
    /// ③ 否则 `inherited SysMsg(sMsg, MsgColor, MsgType, boAddPrefix)`（12912）。
    ///
    /// ★ 原文如此 ④：**`boAddPrefix` 参数在方法体内从未被使用** —— 前缀只由 `g_Config.boShowPreFixMsg` 决定，
    /// 传 `False` 也照样加前缀（照抄，不修；差异断言 `SysMsg_BoAddPrefixFalse_StillPrefixed_OriginalDefect`）。
    /// ★ 注意 `SendSocket(@m_DefMsg, sMsg)` **不做** `EncodeString`（与 `SendDefMessage` 不同）。
    /// ⚠ 托管侧重载解析提醒：本方法第 4 参有默认值，故**三参调用**在 C# 里优先绑定基类
    /// `TCreature.SysMsg(string, TMsgColor, TMsgType)`（`ObjBase.OnlineMsg.cs:25`），
    /// 而 Delphi 里 `TPlayObject.SysMsg` **隐藏**基类同名方法 —— 本文件内部一律**显式传第 4 参**以保证走本方法；
    /// 该解析差异已登记在交付报告（**未**额外声明 3 参重载去"修"它 —— 那会与基类同名同签名冲突并触发 CS0108）。
    /// </remarks>
    public void SysMsg(string sMsg, TMsgColor msgColor, TMsgType msgType, bool boAddPrefix = true)
    {
        // 原文 12871-12872
        if (m_boOffLine || m_boDummyObject) return;

        // 原文 12874
        if (m_IsSendUserLogon)
        {
            // 原文 12876-12894（★ boAddPrefix 未参与判定 —— 原文如此 ④）
            if (M2Config.boShowPreFixMsg)
            {
                switch (msgType)
                {
                    case TMsgType.t_Mon: sMsg = M2Config.sMonSayMsgpreFix + sMsg; break;      // 12880
                    case TMsgType.t_Hint: sMsg = M2Config.sHintMsgPreFix + sMsg; break;       // 12882
                    case TMsgType.t_GM: sMsg = M2Config.sGMRedMsgpreFix + sMsg; break;        // 12884
                    case TMsgType.t_System: sMsg = M2Config.sSysMsgPreFix + sMsg; break;      // 12886
                    case TMsgType.t_Notice: sMsg = M2Config.sLineNoticePreFix + sMsg; break;  // 12888
                    case TMsgType.t_Cust: sMsg = M2Config.sCustMsgpreFix + sMsg; break;       // 12890
                    case TMsgType.t_Castle: sMsg = M2Config.sCastleMsgpreFix + sMsg; break;   // 12892
                }
            }

            // 原文 12896-12906
            int color;
            switch (msgColor)
            {
                case TMsgColor.c_Green:   // 12898
                    color = PlayerSurfacePack.MakeWord(M2Config.btGreenMsgFColor, M2Config.btGreenMsgBColor);
                    break;
                case TMsgColor.c_Blue:    // 12900
                    color = PlayerSurfacePack.MakeWord(M2Config.btBlueMsgFColor, M2Config.btBlueMsgBColor);
                    break;
                default:                  // 12901-12905
                    if (msgType == TMsgType.t_Cust)
                        color = PlayerSurfacePack.MakeWord(M2Config.btCustMsgFColor, M2Config.btCustMsgBColor);
                    else
                        color = PlayerSurfacePack.MakeWord(M2Config.btRedMsgFColor, M2Config.btRedMsgBColor);
                    break;
            }

            // 原文 12908：MakeDefaultMsg(SM_SYSMESSAGE, NativeInt(Self), Color, 0, 1)
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_SYSMESSAGE, m_nRecogId, color, 0, 1);
            // 原文 12909
            SendSocketRef(m_DefMsg, sMsg);
        }
        else
        {
            // 原文 12912：inherited SysMsg(sMsg, MsgColor, MsgType, boAddPrefix);
            // 基类三参版显式限定（避免与上面的四参重载互相递归）。
            base.SysMsg(sMsg, msgColor, msgType);
        }
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SysMsg(sMsg: AnsiString; FColor, BColor: Integer;
    /// MsgType: TMsgType; boAddPrefix: Boolean = True);`（ObjPlayer.pas:12915-12947）。
    /// 与上一重载的差异：颜色**不由 `MsgColor` 决定**，而是 `MakeWord(FColor, BColor)`（12942）；
    /// 前缀块与退出条件完全相同。
    /// </summary>
    /// <remarks>★ 原文如此 ④ 同样适用：`boAddPrefix` 未被使用。
    /// ⚠ `base.SysMsg(sMsg, TMsgColor.c_Red, msgType)`（12946）：托管基类
    /// `TCreature.SysMsg` 的颜色参数**在实现体里未被读取**（`ObjBase.OnlineMsg.cs:25-28` 只把 `sMsg`
    /// 追加到 `SysMsgs`），故此处实参取值不影响行为。</remarks>
    public void SysMsg(string sMsg, int fColor, int bColor, TMsgType msgType, bool boAddPrefix = true)
    {
        // 原文 12917-12918
        if (m_boOffLine || m_boDummyObject) return;

        // 原文 12920
        if (m_IsSendUserLogon)
        {
            // 原文 12922-12940
            if (M2Config.boShowPreFixMsg)
            {
                switch (msgType)
                {
                    case TMsgType.t_Mon: sMsg = M2Config.sMonSayMsgpreFix + sMsg; break;
                    case TMsgType.t_Hint: sMsg = M2Config.sHintMsgPreFix + sMsg; break;
                    case TMsgType.t_GM: sMsg = M2Config.sGMRedMsgpreFix + sMsg; break;
                    case TMsgType.t_System: sMsg = M2Config.sSysMsgPreFix + sMsg; break;
                    case TMsgType.t_Notice: sMsg = M2Config.sLineNoticePreFix + sMsg; break;
                    case TMsgType.t_Cust: sMsg = M2Config.sCustMsgpreFix + sMsg; break;
                    case TMsgType.t_Castle: sMsg = M2Config.sCastleMsgpreFix + sMsg; break;
                }
            }

            // 原文 12942：MakeDefaultMsg(SM_SYSMESSAGE, NativeInt(Self), MakeWord(FColor, BColor), 0, 1)
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_SYSMESSAGE, m_nRecogId,
                PlayerSurfacePack.MakeWord(fColor, bColor), 0, 1);
            // 原文 12943
            SendSocketRef(m_DefMsg, sMsg);
        }
        else
        {
            // 原文 12946：inherited SysMsg(sMsg, FColor, BColor, MsgType, boAddPrefix);
            base.SysMsg(sMsg, TMsgColor.c_Red, msgType);
        }
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SysMsgEx(sMsg: AnsiString; MsgColor: TMsgColor; MsgType: TMsgType;
    /// boAddPrefix: Boolean = True);`（ObjPlayer.pas:12949-12975）。
    /// 与 <see cref="SysMsg(string, TMsgColor, TMsgType, bool)"/> 的唯一差别：**没有前缀块**
    /// （原文 12957-12968 直接算颜色），其余（退出条件、`SM_SYSMESSAGE`、`SendSocket`）相同。
    /// </summary>
    public void SysMsgEx(string sMsg, TMsgColor msgColor, TMsgType msgType, bool boAddPrefix = true)
    {
        // 原文 12953-12954
        if (m_boOffLine || m_boDummyObject) return;

        // 原文 12956
        if (m_IsSendUserLogon)
        {
            // 原文 12958-12968
            int color;
            switch (msgColor)
            {
                case TMsgColor.c_Green:
                    color = PlayerSurfacePack.MakeWord(M2Config.btGreenMsgFColor, M2Config.btGreenMsgBColor);
                    break;
                case TMsgColor.c_Blue:
                    color = PlayerSurfacePack.MakeWord(M2Config.btBlueMsgFColor, M2Config.btBlueMsgBColor);
                    break;
                default:
                    if (msgType == TMsgType.t_Cust)
                        color = PlayerSurfacePack.MakeWord(M2Config.btCustMsgFColor, M2Config.btCustMsgBColor);
                    else
                        color = PlayerSurfacePack.MakeWord(M2Config.btRedMsgFColor, M2Config.btRedMsgBColor);
                    break;
            }

            // 原文 12970-12971
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_SYSMESSAGE, m_nRecogId, color, 0, 1);
            SendSocketRef(m_DefMsg, sMsg);
        }
        else
        {
            // 原文 12974：inherited SysMsgEx(sMsg, MsgColor, MsgType, boAddPrefix);
            PlayerSurfaceCore6Seams.BaseSysMsgEx(this, sMsg, msgColor, msgType, boAddPrefix);
        }
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SysMsgEx(sMsg: AnsiString; FColor, BColor: Integer; MsgType: TMsgType;
    /// boAddPrefix: Boolean = True);`（ObjPlayer.pas:12977-12989）。
    /// 无前缀块、无颜色分支 —— 直接 `MakeWord(FColor, BColor)`（12984）。
    /// </summary>
    public void SysMsgEx(string sMsg, int fColor, int bColor, TMsgType msgType, bool boAddPrefix = true)
    {
        // 原文 12979-12980
        if (m_boOffLine || m_boDummyObject) return;

        // 原文 12982
        if (m_IsSendUserLogon)
        {
            // 原文 12984-12985
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_SYSMESSAGE, m_nRecogId,
                PlayerSurfacePack.MakeWord(fColor, bColor), 0, 1);
            SendSocketRef(m_DefMsg, sMsg);
        }
        else
        {
            // 原文 12988：inherited SysMsgEx(sMsg, FColor, BColor, MsgType, boAddPrefix);
            PlayerSurfaceCore6Seams.BaseSysMsgEx(this, sMsg, TMsgColor.c_Red, msgType, boAddPrefix);
        }
    }

    // ==================================================================
    // 未移植：CheckTakeOnItems（原文 12991-13373，384 行）
    // ==================================================================

    /// <summary>
    /// 原文 `function TPlayObject.CheckTakeOnItems(nWhere: Integer; StdItem: pTStdItem): Boolean;`
    /// （ObjPlayer.pas:12991-13373）—— **未移植（PortNotPorted）**。
    /// </summary>
    /// <remarks>
    /// 缺的成员（不在本切片、且尚未移植；本方法**不做**半移植以免把 40+ 条佩戴失败提示静默成空串）：
    /// <list type="bullet">
    ///   <item><description>`m_Abil: TAbility`（TBaseObject，ObjBase.pas）—— 托管 `TCreature` 只有 `m_wAbil`；
    ///     本方法 12997 起的 `m_Abil.Level` 与 13042/13056/13084/13091/13119/13170/13237/13306 全部依赖它。</description></item>
    ///   <item><description>`m_btReLevel: Byte`（ObjPlayer.pas:175）—— Need=4/40/41/42/43/44 分支。</description></item>
    ///   <item><description>M2Share 全局消息串：`sWearNotOfWoMan`(12999)、`sWearNotOfMan`(13005)、
    ///     `sWearNotOfHum`(13011)、`sHandWeightNot`(13027)、`sWearWeightNot`(13033)、`g_sLevelNot`(13045)、
    ///     `g_sDCNot`(13052)、`g_sJobOrLevelNot`(13059)、`g_sJobOrDCNot`(13066)、`g_sJobOrMCNot`(13073)、
    ///     `g_sJobOrSCNot`(13080)、`g_sLevelNotEqual`(13087)、`g_sMCNot`(13108)、`g_sSCNot`(13115)、
    ///     `g_sReNewLevelNot`(13164)、`g_sCreditPointNot`(13221)、`g_sGuildNot`(13286)、
    ///     `g_sGuildMasterNot`(13293)、`g_sSabukHumanNot`(13300)、`g_sSabukMasterManNot`(13312)、
    ///     `g_sMemberNot`(13319)、`g_sMemberTypeNot`(13326/13333)、`g_sNationKingNot`(13358)、
    ///     `g_sNationPeopleNot`(13366，需 `Format`)。</description></item>
    ///   <item><description>`m_btNation`（ObjBase.pas:408）+ `g_NationManage.Items[...].sKingName/sName`
    ///     （NationManage.pas）—— Need=108 分支（13345-13370）。</description></item>
    /// </list>
    /// **已有接缝**（移植时可直接用）：`PlayerSurfaceItemSeams.IsCastleMember`(→13038)、
    /// `.MyGuild`/`.GuildRankNo`(→13283-13312)、`.MemberType`/`.MemberLevel`(→13316-13333)、
    /// `VisibleItemLifecycleCore.CheckOverLapItem(overlap, stdMode, duraMax)`(→13015)、
    /// `PlayerSurfaceMsgSeams.GetStdItem`。
    /// ⚠ 本方法的 `Castle` 只在 Need=7/70 使用（13038 无条件取一次），且 `m_MyGuild` 的判空可与接缝一一对应。
    /// </remarks>
    public bool CheckTakeOnItems(int nWhere, TStdItem stdItem)
    {
        PortNotPorted(nameof(CheckTakeOnItems), 12991);   // 原文 12991-13373
        return default;                                  // 未移植：调用方须视作「不允许佩戴」
    }

    // ==================================================================
    // 未移植：GetUserItemWeitht（原文 13375-13414）
    // ==================================================================

    /// <summary>
    /// 原文 `function TPlayObject.GetUserItemWeitht(nWhere: Integer): Integer;`（ObjPlayer.pas:13375-13414）
    /// —— **未移植（PortNotPorted）**。
    /// </summary>
    /// <remarks>
    /// 缺的成员：`m_JewelryBoxItems`（`THumanJewelryBoxItems` = `array[0..5] of TUserItem`，ObjBase.pas）
    /// 与 `m_GodBlessItems`（`THumanGodBlessItems` = `array[0..11] of TUserItem`，ObjBase.pas）
    /// 两个装备容器（原文 13394-13412 两段遍历）；托管侧**均未声明**。
    /// 第一段（`m_UseItems`，13382-13392）本身已具备移植条件，但为避免
    /// 「半移植导致重量少算 18 格」这种**静默数值偏差**，整条留痕。
    /// 参照实现见 <see cref="GetUserItemHandWeight"/>（它只遍历 `m_UseItems`，已移植）。
    /// ⚠ 原文 13386 用的是 `(I in [U_WEAPON, U_FASHIONWEAPON, U_SHIELD])` 的**取反**条件
    /// （与 HandWeight 版互补）。
    /// </remarks>
    public int GetUserItemWeitht(int nWhere)
    {
        PortNotPorted(nameof(GetUserItemWeitht), 13375);   // 原文 13375-13414
        return default;                                   // 未移植：调用方须视作「0」
    }

    // ==================================================================
    // GetUserItemHandWeight（原文 13416-13434）—— 真实体
    // ==================================================================

    /// <summary>
    /// 原文 `function TPlayObject.GetUserItemHandWeight(nWhere: Integer): Integer;`
    /// （ObjPlayer.pas:13416-13434）—— 已移植。
    /// </summary>
    /// <remarks>
    /// 语义：累加 `m_UseItems` 中**属于腕力槽**（`U_WEAPON`=1 / `U_FASHIONWEAPON`=19 / `U_SHIELD`=16）
    /// 且**不是** `nWhere` 那一格的装备重量；`nWhere = -1` 时**全部**腕力槽都算。
    /// 单步累加 `Min(High(Word), n14 + StdItem.Weight)`（原文 13430；`High(Word)` = 65535）。
    /// ⚠ 原文 `for I := Low(THumanUseItems) to High(THumanUseItems)` 是 **0..29**；
    /// 托管 `m_UseItems` 长度是 **21**（`UseSlots.SlotCount`，偏差已在 `TCreature.PlayerSurface.Items.cs:138`
    /// 登记）—— 故按托管数组长度遍历（21..29 在原文里也无读写点）。
    /// ⚠ 原文 `m_UseItems` 是**记录数组**（无 nil）；托管是 `TUserItem?[]`（D35），
    /// 空槽按「全零记录」处理（`wIndex = 0`）后再查 `GetStdItem`，与原文一致。
    /// </remarks>
    public int GetUserItemHandWeight(int nWhere)
    {
        // 原文 13422：n14 := 0;
        int n14 = 0;
        // 原文 13423：for I := Low(THumanUseItems) to High(THumanUseItems) do
        for (int i = 0; i < m_UseItems.Length; i++)
        {
            // 原文 13426：if (nWhere = -1) or ((I <> nWhere) and (I in [U_WEAPON, U_FASHIONWEAPON, U_SHIELD])) then
            if (nWhere == -1
                || (i != nWhere
                    && (i == Grobal2Const.U_WEAPON
                        || i == Grobal2Const.U_FASHIONWEAPON
                        || i == Grobal2Const.U_SHIELD)))
            {
                // 原文 13428：StdItem := UserEngine.GetStdItem(m_UseItems[I].wIndex);
                // （空槽 = 全零记录 ⇒ wIndex = 0，与原文记录数组语义一致）
                int wIndex = m_UseItems[i]?.wIndex ?? 0;
                var stdItem = PlayerSurfaceItemSeams.GetStdItem(wIndex);
                if (stdItem != null)
                {
                    // 原文 13430：n14 := Min(High(Word), n14 + StdItem.Weight);
                    n14 = Math.Min(65535 /*High(Word)*/, n14 + stdItem.Value.Weight);
                }
            }
        }
        // 原文 13433：Result := n14;
        return n14;
    }

    // ==================================================================
    // 未移植：EatItems / ReadBook / SendAddMagic / SendDelMagic / EatUseItems
    // ==================================================================

    /// <summary>
    /// 原文 `function TPlayObject.EatItems(StdItem: pTStdItem): Boolean;`（ObjPlayer.pas:13436-13598）
    /// —— **未移植（PortNotPorted）**。
    /// </summary>
    /// <remarks>
    /// 缺的成员：`m_PEnvir.m_boNODRUG`（TEnvirnoment，原文 13443）、消息串 `sCanotUseDrugOnThisMap`(13445)、
    /// `IncHealthSpell`（ObjBase）、`m_boUserUnLockDurg`(13460)、`m_AbilNG.NH/MaxNH/Level`(13485-13489)、
    /// `m_Abil.Level`、`m_nIncHealth`/`m_nIncSpell`/`m_nIncHealing`(13496-13508)、`GetMyStatus`/`RefMyStatus`(13515-13519)、
    /// `m_nHungerStatus`(13516-13517)、`m_wStatusArrValue`/`m_dwStatusArrTimeOutTick`(13532-13584)、
    /// `g_sDCUpTime`/`g_sMCUpTime`/`g_sSCUpTime`/`g_sHitSpeedUpTime`/`g_sMaxHPUpTime`/`g_sMaxMPUpTime`(13534-13582)。
    /// 依赖本文件的 <see cref="EatUseItems"/>（同为未移植）。
    /// </remarks>
    public bool EatItems(TStdItem stdItem)
    {
        PortNotPorted(nameof(EatItems), 13437);   // 原文 13436-13598
        return default;                          // 未移植：调用方须视作「未吃下」
    }

    /// <summary>
    /// 原文 `function TPlayObject.ReadBook(StdItem: pTStdItem): Boolean;`（ObjPlayer.pas:13600-13651）
    /// —— **未移植（PortNotPorted）**。
    /// </summary>
    /// <remarks>
    /// 缺的成员：`UserEngine.FindMagic(sMagicName)`（**按名查找**重载；托管 `Engine/UsrEngn.cs:52` 只有
    /// `FindMagic(int nMagIdx, TMagicAttr)`）、`IsTrainingSkill`(13611)、`TMagic.btJob/sDescr/TrainLevel` 的
    /// 完整面、`m_Abil.Level`(13619)、`m_AbilNG.Level`/`m_boTrainingNG`(13617)、
    /// `m_nScriptGotoCount`/`m_nLearnMagicID`(13640-13643)、`g_FunctionNPC.GotoLable`（原文 13642）。
    /// </remarks>
    public bool ReadBook(TStdItem stdItem)
    {
        PortNotPorted(nameof(ReadBook), 13600);   // 原文 13600-13651
        return default;                          // 未移植：调用方须视作「未学会」
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendAddMagic(UserMagic: pTUserMagic);`（ObjPlayer.pas:13653-13675）
    /// —— 已移植（**两处接缝**，见 <see cref="PlayerSurfaceCore6Seams"/>）。
    /// </summary>
    /// <remarks>
    /// 逐行：① `m_boOffLine or m_boDummyObject → Exit`（13658-13659）；
    /// ② `UserMagicToClientMagic(UserMagic, @ClientMagic)`（13661 → 接缝，属 ObjBase 面）；
    /// ③ `dwInterval := GetMagicCD(wMagIdx)`、`dwRealInterval := dwInterval`、`dwLastUseTick := 0`（13662-13664）；
    /// ④ 自定义技能「有提示」时 `dwInterval := 0`（13666-13672；原文读的是
    /// `CustomMagicConfig.ServerConfig.FailMsg`，托管 `Engine.TCustomMagicServerConfig`（MagicBatchH.cs:48）
    /// **没有该字段** → 接缝 `CustomMagicHasFailMsg`）；
    /// ⑤ `MakeDefaultMsg(SM_ADDMAGIC, 0, 0, 0, 1)` + `SendSocketEx(@ClientMagic, SizeOf(TClientMagic))`（13673-13674）。
    /// ⚠ 原文形参是 `pTUserMagic` 指针；托管 `m_MagicList` 元素是**值类型** `THumMagic`
    /// （`Engine/ObjBase.cs:197`，另见 D35），故形参取 `THumMagic`。
    /// </remarks>
    public void SendAddMagic(THumMagic userMagic)
    {
        // 原文 13658-13659
        if (m_boOffLine || m_boDummyObject) return;

        // 原文 13661：UserMagicToClientMagic(UserMagic, @ClientMagic);  → 接缝
        TClientMagic clientMagic = PlayerSurfaceCore6Seams.UserMagicToClientMagic(userMagic);

        // 原文 13662：ClientMagic.dwInterval := GetMagicCD(UserMagic.wMagIdx);
        clientMagic.dwInterval = GetMagicCD(userMagic.wMagIdx);
        // 原文 13663
        clientMagic.dwRealInterval = clientMagic.dwInterval;
        // 原文 13664
        clientMagic.dwLastUseTick = 0;

        // 原文 13666-13667：CustomMagicConfig := GetCustomMagicConfig(UserMagic.wMagIdx);
        var customMagicConfig = M2Config.GetCustomMagicConfig(userMagic.wMagIdx);
        if (customMagicConfig != null)
        {
            // 原文 13669-13671：if Length(CustomMagicConfig.ServerConfig.FailMsg) > 0 then dwInterval := 0;
            if (PlayerSurfaceCore6Seams.CustomMagicHasFailMsg(userMagic.wMagIdx))
                clientMagic.dwInterval = 0;
        }

        // 原文 13673：MakeDefaultMsg(SM_ADDMAGIC, 0, 0, 0, 1)
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_ADDMAGIC, 0, 0, 0, 1);
        // 原文 13674：SendSocketEx(@m_DefMsg, @ClientMagic, SizeOf(TClientMagic));
        SendSocketExRef(m_DefMsg, CustomNpcUtils.StructToBytes(clientMagic));
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendDelMagic(UserMagic: pTUserMagic);`（ObjPlayer.pas:13683-13771）
    /// —— **未移植（PortNotPorted）**。
    /// </summary>
    /// <remarks>
    /// 缺的成员：`ThrustingOnOff`(13722)、`SendOpenMagic`（ObjPlayer.pas:3587，属同单元另一片）、
    /// `CheckIsCustomMagic`(13726)、`m_boCustomSkill[]`(13731-13736)、
    /// `CustomMagicConfig.ClientBaseConfig.MagicSwitchMode/MagicWarrNGOption/MagicAutoOpen`（托管
    /// `Engine.TCustomMagicConfig` 无这些字段）、以及 13 个技能指针字段
    /// `m_MagicSuperShiledSkill`/`m_MagicOneSwordSkill`/`m_MagicPowerHitSkill`/`m_MagicErgumSkill`/
    /// `m_MagicBanwolSkill`/`m_MagicFireSwordSkill`/`m_MagicCrsSkill`/`m_Magic42Skill`/`m_Magic43Skill`/
    /// `m_MagicSwordSkill`/`m_Magic66Skill`/`m_Magic113Skill`/`m_Magic115Skill`（13690-13715、13745-13770）。
    /// 报文本身（`SM_DELMAGIC` + `wMagIdx` + `Word(MagicAttr)`，13742-13743）已具备装配条件，
    /// 但缺少「删技能时顺带关被动」的副作用 ⇒ 整体留痕，避免半移植。
    /// </remarks>
    public void SendDelMagic(THumMagic userMagic)
    {
        PortNotPorted(nameof(SendDelMagic), 13684);   // 原文 13683-13771
    }

    /// <summary>
    /// 原文 `function TPlayObject.EatUseItems(nShape: Integer): Boolean;`（ObjPlayer.pas:13773-13872）
    /// —— **未移植（PortNotPorted）**。
    /// </summary>
    /// <remarks>
    /// 缺的成员：`m_sHomeMap`/`m_nHomeX`/`m_nHomeY`（TBaseObject，ObjBase.pas）、
    /// `PKLevel`/`m_nPkPoint`（TSmartObject，ObjBase.pas:13705-13708）、`m_PEnvir.m_boNORANDOMMOVE`(13805)、
    /// `m_wStatusTimeArr[POISON_STONE]`(13799)、`m_MyGuild`/`TUserCastle.IsMasterGuild/GetHomeX/GetHomeY`(13841-13847)、
    /// `m_boInFreePKArea`(13843)；以及本文件的 <see cref="WeaptonMakeLuck"/> / <see cref="RepairWeapon"/> /
    /// <see cref="SuperRepairWeapon"/> / <see cref="WinLottery"/>（四者同为未移植）。
    /// **已有**：`m_boImprison`（MagicBatchH.cs:135）、`g_Config.boDisableMoveParalysisHuman/btRedMsgFColor/
    /// btRedMsgBColor/sRedHomeMap/nRedHomeX/nRedHomeY`、`SendRefMsg`、<see cref="BaseObjectMove"/>、
    /// `PlayerSurfaceItemSeams.IsCastleMember`。
    /// </remarks>
    public bool EatUseItems(int nShape)
    {
        PortNotPorted(nameof(EatUseItems), 13777);   // 原文 13773-13872
        return default;                             // 未移植：调用方须视作「未使用」
    }

    // ==================================================================
    // 未移植：MoveToHome / MoveRandomToHome（原文 13962-13977）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.MoveToHome;`（ObjPlayer.pas:13962-13969）—— **未移植（PortNotPorted）**。
    /// </summary>
    /// <remarks>
    /// 缺的成员：`m_sHomeMap`/`m_nHomeX`/`m_nHomeY`（TBaseObject 字段，ObjBase.pas）、
    /// `PKLevel`（`function TSmartObject.PKLevel: Integer; Result := m_nPkPoint div 100;`，
    /// ObjBase.pas:13705-13708 → 需 `m_nPkPoint`）。
    /// 已有的部分：`SendRefMsg(RM_SPACEMOVE_FIRE, ...)`（`TCreature.SendRefMsg`）、本文件的
    /// <see cref="BaseObjectMove"/>、`g_Config.sRedHomeMap/nRedHomeX/nRedHomeY`。
    /// </remarks>
    public void MoveToHome()
    {
        PortNotPorted(nameof(MoveToHome), 13963);   // 原文 13962-13969
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.MoveRandomToHome;`（ObjPlayer.pas:13972-13976）—— **未移植（PortNotPorted）**。
    /// </summary>
    /// <remarks>
    /// 缺的成员：`m_sHomeMap`（TBaseObject，ObjBase.pas）与 `MapRandomMove`（ObjBase.pas:749，未移植；
    /// 本文件已在 <see cref="PlayerSurfaceCore6Seams.MapRandomMove"/> 留了接缝，但地图名成员缺失 ⇒ 仍留痕）。
    /// </remarks>
    public void MoveRandomToHome()
    {
        PortNotPorted(nameof(MoveRandomToHome), 13974);   // 原文 13972-13976
    }

    // ==================================================================
    // InitSpeed（原文 13978-14001）—— 真实体
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.InitSpeed;`（ObjPlayer.pas:13978-14001）—— 已移植。
    /// </summary>
    /// <remarks>
    /// `g_Config.boCheckActionCount` 为真：三组「检查窗口重置」——
    /// 三个 `m_dwCheck*Tick := MyGetTickCount`（**原文 13982/13985/13988 各调用一次**，共 3 次）
    /// 且 6 个计数器清零；为假：只把 6 个计数器清零（**不碰 tick**）。
    /// 所有字段均为 ObjPlayer.pas 自身声明（:347-355），本文件已声明。
    /// </remarks>
    public void InitSpeed()
    {
        // 原文 13980：if g_Config.boCheckActionCount then
        if (M2Config.boCheckActionCount)
        {
            // 原文 13982：m_dwCheckMoveTick := MyGetTickCount;
            m_dwCheckMoveTick = PlayerSurfaceNpcSeams.MyGetTickCount();
            // 原文 13983：m_nCheckMoveCount := 0;
            m_nCheckMoveCount = 0;
            // 原文 13984：m_nMoveCount := 0;
            m_nMoveCount = 0;
            // 原文 13985：m_dwCheckAttackTick := MyGetTickCount;   ← 第二次独立调用
            m_dwCheckAttackTick = PlayerSurfaceNpcSeams.MyGetTickCount();
            // 原文 13986
            m_nCheckAttackCount = 0;
            // 原文 13987
            m_nAttackCount = 0;
            // 原文 13988：m_dwCheckMagicAttackTick := MyGetTickCount;   ← 第三次独立调用
            m_dwCheckMagicAttackTick = PlayerSurfaceNpcSeams.MyGetTickCount();
            // 原文 13989
            m_nCheckMagicAttackCount = 0;
            // 原文 13990
            m_nMagicAttackCount = 0;
        }
        else
        {
            // 原文 13994-13999（**注意顺序**：先 3 个 count，再 3 个 check count；且不动 tick）
            m_nMoveCount = 0;
            m_nAttackCount = 0;
            m_nMagicAttackCount = 0;
            m_nCheckMoveCount = 0;
            m_nCheckAttackCount = 0;
            m_nCheckMagicAttackCount = 0;
        }
    }

    // ==================================================================
    // BaseObjectMove（原文 14003-14029）—— 真实体
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.BaseObjectMove(sMAP, sX, sY: string);`（ObjPlayer.pas:14003-14029）
    /// —— 已移植（`SpaceMove`/`MapRandomMove` 走接缝：属未移植的 ObjBase.pas）。
    /// </summary>
    /// <remarks>
    /// 逐行：① `m_boImprison → SysMsg('禁止使用此命令！', c_Red, t_Hint) + Exit`（14008-14012）；
    /// ② 暂存旧地图 `Envir := m_PEnvir`（14014）；③ `sMAP = '' → sMAP := m_sMapName`（14015-14016）；
    /// ④ `sX`、`sY` 都非空 → `StrToIntDef` 后 `SpaceMove(sMAP, nX, nY, 0)`，否则 `MapRandomMove(sMAP, 0)`（14018-14025）；
    /// ⑤ **换了地图**（指针比较）且 `m_btRaceServer = RC_PLAYOBJECT` → `m_boTimeRecall := False`（14027-14028）。
    /// ⚠ 原文 14022/14025 的 `SpaceMove`/`MapRandomMove` 是 `TBaseObject` 方法（ObjBase.pas，未移植）。
    /// ⚠ `SysMsg` 调用在 C# 里**显式传第 4 参**（原文在 Delphi 里绑定 `TPlayObject.SysMsg`；
    /// 托管三参调用会绑定基类重载，见 <see cref="SysMsg(string, TMsgColor, TMsgType, bool)"/> 的提醒）。
    /// </remarks>
    public void BaseObjectMove(string sMAP, string sX, string sY)
    {
        // 原文 14008-14012
        if (m_boImprison)
        {
            SysMsg("禁止使用此命令！", TMsgColor.c_Red, TMsgType.t_Hint, true);
            return;
        }

        // 原文 14014：Envir := m_PEnvir;
        var envir = m_PEnvir;

        // 原文 14015-14016：if sMAP = '' then sMAP := m_sMapName;
        if (sMAP == "") sMAP = m_sMapName;

        // 原文 14018：if (sX <> '') and (sY <> '') then
        if (sX != "" && sY != "")
        {
            // 原文 14020-14021：nX := StrToIntDef(sX, 0); nY := StrToIntDef(sY, 0);
            int nX = DelphiRTL.StrToIntDef(sX, 0);
            int nY = DelphiRTL.StrToIntDef(sY, 0);
            // 原文 14022：SpaceMove(sMAP, nX, nY, 0);
            PlayerSurfaceCore6Seams.SpaceMove(this, sMAP, nX, nY, 0);
        }
        else
        {
            // 原文 14025：MapRandomMove(sMAP, 0);
            PlayerSurfaceCore6Seams.MapRandomMove(this, sMAP, 0);
        }

        // 原文 14027-14028：if (Envir <> m_PEnvir) and (m_btRaceServer = RC_PLAYOBJECT) then m_boTimeRecall := False;
        if (!ReferenceEquals(envir, m_PEnvir) && m_btRaceServer == Grobal2Const.RC_PLAYOBJECT)
            m_boTimeRecall = false;
    }

    // ==================================================================
    // 未移植：WeaptonMakeLuck / RepairWeapon / SuperRepairWeapon / WinLottery
    // ==================================================================

    /// <summary>
    /// 原文 `function TPlayObject.WeaptonMakeLuck: Boolean;`（ObjPlayer.pas:14032-14093）
    /// —— **未移植（PortNotPorted）**。
    /// </summary>
    /// <remarks>
    /// 缺的成员：`SendUpdateItem(@m_UseItems[U_WEAPON])`（ObjPlayer.pas:**12688**，本车道按任务书第 4 条
    /// 留给同单元另一片，**当前工作区尚无实现**）、`MakeWeaponUnlock`(14049，M2Share/ObjBase 面)、
    /// 消息串 `g_sWeaptonMakeLuck`(14058/14064/14071/14078)/`g_sWeaptonNotMakeLuck`(14090)。
    /// 已有的部分：`m_UseItems`、`PlayerSurfaceItemSeams.GetStdItem`、
    /// `g_Config.nWeaponMakeUnLuckRate/nWeaponMakeLuckPoint1..3/nWeaponMakeLuckPoint2Rate/
    /// nWeaponMakeLuckPoint3Rate`（**均已存在**，M2Config.FunctionMutinyMsg.cs:18-23）、
    /// `RecalcAbilitys`、`SendMsg(Self, RM_ABILITY/RM_SUBABILITY, ...)`。
    /// ⚠ 原文 14045 的 `abs(StdItem.DC2 - StdItem.DC1) div 5` 是**整数 div**；
    /// 14075 的 `Random(nRand * g_Config.nWeaponMakeLuckPoint3Rate)` 在 `nRand = 0` 时传入 **0**
    /// （Delphi `Random(0)` 返回 0）—— 移植时须保留。
    /// </remarks>
    public bool WeaptonMakeLuck()
    {
        PortNotPorted(nameof(WeaptonMakeLuck), 14038);   // 原文 14032-14093
        return default;                                 // 未移植：调用方须视作「未成功」
    }

    /// <summary>
    /// 原文 `function TPlayObject.RepairWeapon: Boolean;`（ObjPlayer.pas:14095-14134）
    /// —— **未移植（PortNotPorted）**。
    /// </summary>
    /// <remarks>
    /// 缺的成员：`GetUserItemBindValue(UserItem, ubNoRepair)`（托管仅有
    /// `Npc.ObjNpcSeams.GetUserItemBindValue(bindOption, bit)` / `DbLayer.DbLayerRunSeam.GetUserItemBindValue`
    /// 两处接缝，且 `ubNoRepair` 常量在 ObjNpc 面）、`g_ItemRules.Get(wIndex, 3)`（ItemRules 单元）、
    /// 消息串 `g_sDisableRepairItemMsg`(14113，需 `Format`)/`g_sWeaponRepairSuccess`(14125)。
    /// 已有的部分：`m_UseItems`、`g_Config.nRepairItemDecDura`、
    /// `SendMsg(Self, RM_DURACHANGE/RM_ABILITY, ...)`、`RecalcAbilitys`。
    /// ⚠ 原文 14118 的 `Dec(UserItem.DuraMax, (UserItem.DuraMax - UserItem.Dura) div g_Config.nRepairItemDecDura)`
    /// 是**先减后 div**；14119 的 `Min(5000, ...)` 上限即 5000。
    /// </remarks>
    public bool RepairWeapon()
    {
        PortNotPorted(nameof(RepairWeapon), 14102);   // 原文 14095-14134
        return default;                              // 未移植：调用方须视作「未修复」
    }

    /// <summary>
    /// 原文 `function TPlayObject.SuperRepairWeapon: Boolean;`（ObjPlayer.pas:14136-14167）
    /// —— **未移植（PortNotPorted）**。
    /// </summary>
    /// <remarks>缺的成员与 <see cref="RepairWeapon"/> 同族：`GetUserItemBindValue(..., ubNoRepair)`、
    /// `g_ItemRules.Get(wIndex, 3)`、`g_sDisableRepairItemMsg`(14153)/`g_sWeaponRepairSuccess`(14160)。
    /// ⚠ 原文 14158 是**无条件** `Dura := DuraMax`（本例不做 `nRepairItemDecDura` 折损）。</remarks>
    public bool SuperRepairWeapon()
    {
        PortNotPorted(nameof(SuperRepairWeapon), 14141);   // 原文 14136-14167
        return default;                                   // 未移植：调用方须视作「未修复」
    }

    /// <summary>
    /// 原文 `function TPlayObject.WinLottery: Boolean;`（ObjPlayer.pas:14169-14260）
    /// —— **未移植（PortNotPorted）**。
    /// </summary>
    /// <remarks>
    /// 缺的成员：`g_Config.nWinLotteryCount` / `nNoWinLotteryCount` / `nWinLotteryLevel1..6`
    /// （托管 `M2Config.FunctionMine.cs` 只有 `nWinLotteryRate` 与 6 组 `nWinLotteryNMin/Max/Gold`，
    /// **没有 Count 与 Level 计数**）、`DropGoldDown`(14251，ObjBase/M2Share 面)、
    /// 消息串 `g_sWinLottery1Msg`..`g_sWinLottery6Msg`(14235-14245)/`g_sNotWinLotteryMsg`(14256)。
    /// 已有：`IncGold`/`GoldChanged`（`PlayerSurface.Gold.cs`）。
    /// ⚠ 原文的档位判定是 **if/else-if 链**且每档重复判 `nWinLotteryCount < nNoWinLotteryCount`
    /// （未命中判定时**不**下探到更低档）；未中奖时 `Inc(g_Config.nNoWinLotteryCount, 500)`（14255）。
    /// </remarks>
    public bool WinLottery()
    {
        PortNotPorted(nameof(WinLottery), 14173);   // 原文 14169-14260
        return default;                            // 未移植：调用方须视作「未中奖」
    }

    // ==================================================================
    // 交易报文族（原文 14262-14308）—— 真实体
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.SendDelDealItem(UserItem: pTUserItem);`（ObjPlayer.pas:14262-14287）
    /// —— 已移植。
    /// </summary>
    /// <remarks>
    /// 逐行：① `SendDefMessage(SM_DEALDELITEM_OK, 0,0,0,0, '')`（14267，无条件）；
    /// ② `m_DealCreat <> nil` 时：`GetStdItem(UserItem.wIndex)` 非空 →
    /// `UserItem.btValue[13] = 1 且 Name <> ''` 用自定义名，否则用 `StdItem.Name`（14273-14276）→
    /// **`SendDefMessage(SM_DEALREMOTEDELITEM, ...)` 发给「自己」**（14281）→
    /// `m_DealCreat.m_DealLastTick := MyGetTickCount(); m_DealLastTick := MyGetTickCount();`（14284-14285）。
    /// ★ 原文如此 ②：14281 **没有**像 <see cref="SendAddDealItem"/> 的 14302 那样发给 `m_DealCreat`
    /// —— 删除物品的通知发给了自己（对方看不到）。照抄，不修（断言
    /// `SendDelDealItem_RemoteDelGoesToSelf_OriginalDefect`）。
    /// ⚠ 原文 14277-14280 有一段被注释掉的 `if StdItem.StdMode = 50 then ...` —— 保留为注释。
    /// </remarks>
    public void SendDelDealItem(TUserItem userItem)
    {
        // 原文 14267
        PlayerSurfaceMsgSeams.SendDefMessage(this, Grobal2Const.SM_DEALDELITEM_OK, 0, 0, 0, 0, "");

        // 原文 14268
        if (m_DealCreat != null)
        {
            // 原文 14270-14271
            var stdItem = PlayerSurfaceItemSeams.GetStdItem(userItem.wIndex);
            if (stdItem != null)
            {
                string sItemName;
                // 原文 14273：if (UserItem.btValue[13] = 1) and (UserItem.Name <> '') then
                if (userItem.GetBtValue(13) == 1 && userItem.NameStr != "")
                    sItemName = userItem.NameStr;           // 原文 14274
                else
                    sItemName = stdItem.Value.NameStr;      // 原文 14276
                // 原文 14277-14280（注释保留）：if StdItem.StdMode = 50 then sItemName := sItemName + ' #' + IntToStr(UserItem.Dura);
                // 原文 14281：SendDefMessage(SM_DEALREMOTEDELITEM, UserItem.MakeIndex, 0, 0, 0, sItemName);
                //   ★ 原文如此 ②：投递对象是 **Self**（不是 m_DealCreat）
                PlayerSurfaceMsgSeams.SendDefMessage(this, Grobal2Const.SM_DEALREMOTEDELITEM,
                    userItem.MakeIndex, 0, 0, 0, sItemName);
            }

            // 原文 14284-14285
            m_DealCreat.m_DealLastTick = PlayerSurfaceNpcSeams.MyGetTickCount();
            m_DealLastTick = PlayerSurfaceNpcSeams.MyGetTickCount();
        }
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendAddDealItem(UserItem: pTUserItem);`（ObjPlayer.pas:14289-14307）
    /// —— 已移植。
    /// </summary>
    /// <remarks>
    /// 与 <see cref="SendDelDealItem"/> 对称但**投递对象相反**：14302 是
    /// `TPlayObject(m_DealCreat).SendSocketEx(@m_DefMsg, @ClientItem, SizeOf(TClientItem))`
    /// —— 报文在自己身上装配（`nRecog = NativeInt(Self)`），由**对方**投递。
    /// ⚠ 原文 14303-14304 的两个 `m_DealLastTick` 赋值在 `StdItem &lt;&gt; nil` **之内**（与删除版相同）。
    /// </remarks>
    public void SendAddDealItem(TUserItem userItem)
    {
        // 原文 14294
        PlayerSurfaceMsgSeams.SendDefMessage(this, Grobal2Const.SM_DEALADDITEM_OK, 0, 0, 0, 0, "");

        // 原文 14295
        if (m_DealCreat != null)
        {
            // 原文 14297-14298
            var stdItem = PlayerSurfaceItemSeams.GetStdItem(userItem.wIndex);
            if (stdItem != null)
            {
                // 原文 14300：UserItemToClientItem(UserItem, StdItem, @ClientItem, True, False);
                var clientItem = PlayerSurfaceItemSeams.UserItemToClientItem(ToItemView(userItem), stdItem.Value);
                // 原文 14301：MakeDefaultMsg(SM_DEALREMOTEADDITEM, NativeInt(Self), 0, 0, 1)
                m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_DEALREMOTEADDITEM, m_nRecogId, 0, 0, 1);
                // 原文 14302：TPlayObject(m_DealCreat).SendSocketEx(@m_DefMsg, @ClientItem, SizeOf(TClientItem));
                PlayerSurfaceCore6Seams.SendSocketExTo(m_DealCreat, m_DefMsg, clientItem);
                // 原文 14303-14304
                m_DealCreat.m_DealLastTick = PlayerSurfaceNpcSeams.MyGetTickCount();
                m_DealLastTick = PlayerSurfaceNpcSeams.MyGetTickCount();
            }
        }
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.OpenDealDlg(PlayObject: TPlayObject);`（ObjPlayer.pas:14309-14316）
    /// —— 已移植（`GetBackDealItems` 走接缝：ObjPlayer.pas:7777 属同单元另一片，尚未移植）。
    /// </summary>
    public void OpenDealDlg(TPlayObject playObject)
    {
        // 原文 14311：m_boDealing := True;
        m_boDealing = true;
        // 原文 14312：m_DealCreat := PlayObject;
        m_DealCreat = playObject;
        // 原文 14313：GetBackDealItems();   → 接缝
        PlayerSurfaceCore6Seams.GetBackDealItems(this);
        // 原文 14314：SendDefMessage(SM_DEALMENU, 0, 0, 0, 0, m_DealCreat.m_sCharName);
        PlayerSurfaceMsgSeams.SendDefMessage(this, Grobal2Const.SM_DEALMENU, 0, 0, 0, 0, m_DealCreat!.m_sCharName);
        // 原文 14315：m_DealLastTick := MyGetTickCount();
        m_DealLastTick = PlayerSurfaceNpcSeams.MyGetTickCount();
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.OpenChallengeDlg(PlayObject: TPlayObject);`（ObjPlayer.pas:14319-14328）
    /// —— 已移植（`GetBackChallengeItems` 走接缝：ObjPlayer.pas:7794，同单元另一片）。
    /// </summary>
    /// <remarks>原文 14322 与 14323 的顺序是「先取回物品、再设 `m_ChallengeCreat`」（与交易版的顺序不同），
    /// 14326 把 `g_Config.btChallengeGoldIndex` 放进 **wParam**（挑战附加币控制），
    /// 14324 那条不带币种参数的调用被注释掉 —— 均照抄。</remarks>
    public void OpenChallengeDlg(TPlayObject playObject)
    {
        // 原文 14321：m_boChallengeing := True;
        m_boChallengeing = true;
        // 原文 14322：GetBackChallengeItems();   → 接缝
        PlayerSurfaceCore6Seams.GetBackChallengeItems(this);
        // 原文 14323：m_ChallengeCreat := PlayObject;
        m_ChallengeCreat = playObject;
        // 原文 14324（注释保留）：// SendDefMessage(SM_CHALLENGEMENU, 0, 0, 0, 0, m_ChallengeCreat.m_sCharName);
        // 原文 14326
        PlayerSurfaceMsgSeams.SendDefMessage(this, Grobal2Const.SM_CHALLENGEMENU,
            M2Config.btChallengeGoldIndex, 0, 0, 0, m_ChallengeCreat!.m_sCharName);
        // 原文 14327：m_ChallengeLastTick := MyGetTickCount();
        m_ChallengeLastTick = PlayerSurfaceNpcSeams.MyGetTickCount();
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendDelChallengeItem(UserItem: pTUserItem);`（ObjPlayer.pas:14330-14351）
    /// —— 已移植。
    /// </summary>
    /// <remarks>与交易版的关键差别（照抄）：14345 的 `SM_CHALLENGEREMOTEDELITEM` **确实发给对方**
    /// （`TPlayObject(m_ChallengeCreat).SendDefMessage(...)`）—— 即挑战版没有交易版 ② 那个投递缺陷。</remarks>
    public void SendDelChallengeItem(TUserItem userItem)
    {
        // 原文 14335
        PlayerSurfaceMsgSeams.SendDefMessage(this, Grobal2Const.SM_CHALLENGEDELITEM_OK, 0, 0, 0, 0, "");

        // 原文 14336
        if (m_ChallengeCreat != null)
        {
            // 原文 14338-14339
            var stdItem = PlayerSurfaceItemSeams.GetStdItem(userItem.wIndex);
            if (stdItem != null)
            {
                string sItemName;
                // 原文 14341
                if (userItem.GetBtValue(13) == 1 && userItem.NameStr != "")
                    sItemName = userItem.NameStr;           // 14342
                else
                    sItemName = stdItem.Value.NameStr;      // 14344
                // 原文 14345：TPlayObject(m_ChallengeCreat).SendDefMessage(SM_CHALLENGEREMOTEDELITEM, ...)
                PlayerSurfaceMsgSeams.SendDefMessage(m_ChallengeCreat, Grobal2Const.SM_CHALLENGEREMOTEDELITEM,
                    userItem.MakeIndex, 0, 0, 0, sItemName);
            }

            // 原文 14348-14349
            m_ChallengeCreat.m_ChallengeLastTick = PlayerSurfaceNpcSeams.MyGetTickCount();
            m_ChallengeLastTick = PlayerSurfaceNpcSeams.MyGetTickCount();
        }
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.SendAddChallengeItem(UserItem: pTUserItem);`（ObjPlayer.pas:14353-14371）
    /// —— 已移植。
    /// </summary>
    public void SendAddChallengeItem(TUserItem userItem)
    {
        // 原文 14358
        PlayerSurfaceMsgSeams.SendDefMessage(this, Grobal2Const.SM_CHALLENGEADDITEM_OK, 0, 0, 0, 0, "");

        // 原文 14359
        if (m_ChallengeCreat != null)
        {
            // 原文 14361-14362
            var stdItem = PlayerSurfaceItemSeams.GetStdItem(userItem.wIndex);
            if (stdItem != null)
            {
                // 原文 14364
                var clientItem = PlayerSurfaceItemSeams.UserItemToClientItem(ToItemView(userItem), stdItem.Value);
                // 原文 14365
                m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_CHALLENGEREMOTEADDITEM, m_nRecogId, 0, 0, 1);
                // 原文 14366
                PlayerSurfaceCore6Seams.SendSocketExTo(m_ChallengeCreat, m_DefMsg, clientItem);
                // 原文 14367-14368
                m_ChallengeCreat.m_ChallengeLastTick = PlayerSurfaceNpcSeams.MyGetTickCount();
                m_ChallengeLastTick = PlayerSurfaceNpcSeams.MyGetTickCount();
            }
        }
    }

    // ==================================================================
    // 未移植：JoinGroup（原文 14373-14379）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.JoinGroup(PlayObject: TPlayObject; IsLockMember: Boolean = True);`
    /// （ObjPlayer.pas:14373-14379）—— **未移植（PortNotPorted）**。
    /// </summary>
    /// <remarks>
    /// 缺的成员：`m_GroupOwner`(14375)、`SendGroupText(sMsg, IsLockMember)`（ObjPlayer.pas:**11132**，
    /// 属同单元另一片，尚未移植）与格式串 `g_sJoinGroup`(14377，M2Share 全局)。
    /// ⚠ 原文 14376 那条不带 LockMember 的 `SendGroupText` 调用被注释掉 —— 保留为注释。
    /// </remarks>
    public void JoinGroup(TPlayObject playObject, bool isLockMember = true)
    {
        PortNotPorted(nameof(JoinGroup), 14374);   // 原文 14373-14379
    }

    // ==================================================================
    // 未移植：MakeMine（原文 14380-14501）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.MakeMine;`（ObjPlayer.pas:14380-14501，含内嵌 `RandomDrua`）
    /// —— **未移植（PortNotPorted）**。
    /// </summary>
    /// <remarks>
    /// 缺的成员：矿石配置串与区间 `g_Config.sGoldStone`/`sSilverStone`/`sSteelStone`/`sBlackStone`/`sCopperStone`、
    /// `nGoldStoneMin/Max`、`nSilverStoneMin/Max`、`nSteelStoneMin/Max`、`nBlackStoneMin/Max`
    /// （托管 `M2Config.FunctionMine.cs` 只有 `nStoneTypeRate`/`nStoneTypeRateMin`/`nStoneGeneralDuraRate` 一族，
    /// **没有上述任何一项**）、`UserEngine.CopyToUserItemFromName`（托管为
    /// `Npc.ObjNpcSeams.CopyToUserItemFromName` 接缝）、`OnMapNotifyEvent(Self, meMine, ...)` 与
    /// `ifMine`/`meMine` 枚举（MapEvent/ObjBase 面）、`m_nScatterItemX/Y`/`m_sScatterItemName`（同单元另一片）。
    /// 已有：`m_ItemList`、`GetMaxBagCount`、`WeightChanged`、`SendAddItem`。
    /// ⚠ 原文 14392 的 `if m_ItemList.Count >= GetMaxBagCount then Exit;` 是**唯一**守卫；
    /// 内嵌 `RandomDrua`（14381-14386）的两次 `Random` 顺序（先 GeneralDuraRate 再 AddDuraRate）须保留。
    /// </remarks>
    public void MakeMine()
    {
        PortNotPorted(nameof(MakeMine), 14392);   // 原文 14380-14501
    }

    // ==================================================================
    // 未移植：QuestTakeCheckItem（原文 14503-14543）
    // ==================================================================

    /// <summary>
    /// 原文 `function TPlayObject.QuestTakeCheckItem(CheckItem: pTUserItem): Boolean;`
    /// （ObjPlayer.pas:14503-14543）—— **未移植（PortNotPorted）**。
    /// </summary>
    /// <remarks>
    /// 缺的成员：`ProcessUseItemSkill(I, StdItem, False)`（ObjPlayer.pas 同单元另一片，未移植）。
    /// 另一个**语义障碍**（因此不做半移植）：原文靠**指针同一性**判定 —
    /// `UserItem = CheckItem`（14517）与 `@m_UseItems[I] = CheckItem`（14529）；
    /// 托管 `m_ItemList` / `m_UseItems` 的元素是**可空值类型** `TUserItem?`（已登记的偏差 **D35**，
    /// 见 `Engine/ObjBase.cs:48-61`），值类型无法表达「同一件」；改成 `MakeIndex` 比较会引入
    /// 「两件 MakeIndex 都为 0 的空物品互相误判」的新偏差 ⇒ 留痕，等 D35 的包装类方案落地。
    /// 已有的部分：`SendDelItem`、`m_ItemList` 的倒序删除、`m_UseItems[I].wIndex := 0` 的写回契约
    /// （14535 那句**在取 StdItem 之后**才清零 —— 顺序须保留）。
    /// </remarks>
    public bool QuestTakeCheckItem(TUserItem checkItem)
    {
        PortNotPorted(nameof(QuestTakeCheckItem), 14503);   // 原文 14503-14543
        return default;                                    // 未移植：调用方须视作「未找到/未删除」
    }

    // ==================================================================
    // 未移植：MakeSaveRcd（原文 14544-14897，354 行，生命周期关键）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.MakeSaveRcd(HumData: PTHumData); // 004B3580`
    /// （ObjPlayer.pas:14544-14897）—— **未移植（PortNotPorted）**。
    /// </summary>
    /// <remarks>
    /// 这是**存盘**入口：把人物 100+ 个字段 + 装备/背包/魔法/仓库/宠物/国战…写进
    /// `THumData`（托管已有承载类型：`GXX.Core.Protocol.THumData`，Grobal2.Types4.cs:128）。
    /// 按「不做半移植」的硬要求整体留痕 —— **半移植的存盘 = 静默丢字段**，比留痕危险得多。
    ///
    /// 缺失成员分五组（逐条给出原文行号与所属面）：
    /// <list type="number">
    ///   <item><description><b>TBaseObject/TSmartObject 面（ObjBase.pas）</b>：
    ///     `m_Abil`(14570，`Move(m_Abil, HumData.Abil, SizeOf(TOAbility))`)、`m_BonusAbil`(14581)、
    ///     `m_wStatusTimeArr`(14575-14576，含 `STATE_TRANSPARENT`)、`m_sHomeMap`/`m_nHomeX`/`m_nHomeY`(14577-14579)、
    ///     `m_nPkPoint`(14580)、`m_btAttatckMode`(14599)、`m_wContribution`(14606)、`m_wGroupRcallTime`(14610)、
    ///     `m_dBodyLuck`(14611)、`m_nKickCount`(14614)、`m_dwFBCreateTime`(14653)、`m_sStoragePwd`(14583)、
    ///     `m_QuestFlag`(14635)、`m_MoneyList`(14891-14894)、`m_nInfinityStorageExtCount`(14819)。</description></item>
    ///   <item><description><b>同单元另一片（本文件外）</b>：`m_sHeroName`/`m_sDeputyHeroName`/`m_boFixedHero`/
    ///     `m_boStorageHero`/`m_boStorageDeputyHero`/`m_btDeputyHeroJob`(14564-14569)、`m_sMasterName`/`m_boMaster`/
    ///     `m_sDearName`(14588-14590)、`m_sMobileNumber`/`m_boMobileBind`/`m_sMobileVerifyCode`/`m_dwMobileVerifyTick`/
    ///     `m_nMobileResendCount`(14820-14824)、`m_StorageItemList`(14720-14728)、`m_BigStorageItemList`+`m_nBigStorageID`(14730)、
    ///     `m_JewelryBoxItems`/`m_nJewelryBoxStatus`/`m_boShowFashion`/`m_boShowGodBless`/`m_GodBlessItemsState`/
    ///     `m_GodBlessItems`(14731-14737)、`m_FengHaoItems`/`m_ActiveFengHao`(14740-14751)、
    ///     `m_UVal`/`m_TVal`/`m_JVal`/`m_ZVal`(14753-14771)、`m_CustomSkillUseTick`(14816)、`m_AddSaveAbil`(14817-14818)、
    ///     `m_GamePetList`/`m_GamePetBagItems`(14828-14851)、`m_NpcSkillPowerAdd`(14853-14870)、
    ///     `m_Alcohol`/`m_HumMeridians`/`m_boPleaseDrink`/`m_wMasterCount`/`m_boOpenLastContinuous`/
    ///     `m_ContinuousMagicOrder`(14642-14651)、`m_nDrinkWineQuality`/`m_nDrinkWineAlcohol`/`m_boDrinkWineDrunk`(14638-14640)、
    ///     `m_boStorageOpen`(14718)、`m_nClearDayVarTime`(14825)、以及 `m_AbilNG`(14641)。</description></item>
    ///   <item><description><b>未移植单元</b>：`g_M2DataDB.StorageDB.SaveStorageItems(...)`(14730，DbLayer 面)、
    ///     `MainOutMessage`(14756/14761/14766/14771，托管有 `PlayerSurfaceOperateSeams.MainOutMessage` 接缝)。</description></item>
    ///   <item><description><b>缺失的常量/类型</b>：`MAX_STORE_ITEM`(14724)、`High(TStorageItems)`(14725)、
    ///     `SizeOf(THumanUseItems)`(14655)、`m_ContinuousMagicOrder` 的 4 元素打包(14648-14651)。</description></item>
    ///   <item><description><b>已知的原文怪癖（移植时须原样保留）</b>：
    ///     ① 14660-14665 背包写盘：`for I := 0 to m_ItemList.Count - 1` **循环体内**才判
    ///        `I >= GetMaxBagCount → Break`（上界靠 `GetMaxBagCount`，而它自己又依赖 `m_DealItemList.Count`）；
    ///     ② 14830 游戏宠物的上限判定是 `if n3C > g_Config.nGamePetMaxCount then Break`（**严格大于**，
    ///        即最多写 `nGamePetMaxCount + 1` 条）；
    ///     ③ 14888-14895 先把 `HumData.CustomMoney` **整体清零**再按 `m_MoneyList` 回填，
    ///        且回填时**不判** `I` 是否越出 `High(HumData.CustomMoney)`（序号即下标，越界即 AV）；
    ///     ④ 14705-14715 普通魔法分支用的是 `else if n1C < Length(HumData.Magics)`（**没有** `begin/end` 包裹的
    ///        独立 `if`，与上面两个分支的写法不同）—— 结构差异须保留。</description></item>
    /// </list>
    /// </remarks>
    public void MakeSaveRcd(THumData humData)
    {
        PortNotPorted(nameof(MakeSaveRcd), 14544);   // 原文 14544-14897
    }
}
