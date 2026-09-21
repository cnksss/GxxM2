// ============================================================================
// 源单元：Source\M2Engine\ObjPlayer.pas（GBK / UTF-8 镜像 49,232 LF）
// 本文件：**ServerSend* 广播处理器片（切片 ServerSend1）** —— 本车道 p13-m2-objplayer。
//
// 覆盖原文行号范围（implementation 段）：
//   ObjPlayer.pas:36854-38859   TPlayObject.ServerSend* 一族
//     · 首条：ServerSendTurn        （36854-36885）
//     · 末条：ServerSendShowEvent   （38812-38858）
//     · 紧随其后的 ServerSendOpenHealth（38860）**不在本片**
//
// 方法条数：**117**
//   （原任务书称 118。已按 `^procedure TPlayObject\.ServerSend` 在 36854-38859 区间
//    重数两遍，确为 **117**；差异 1 条系任务书把区间外邻接的
//    `ClientSlaveTarget`(36628-36849) 或 `ServerSendOpenHealth`(38860) 之一计了进来。
//    见交付报告"条数对账"。）
//
// 三数对账（本片）：
//   真实体     112
//   NotPorted    5  （ServerSendStruck 37393 / ServerSendMagicshieldStruck 37577 /
//                    ServerSendLogon 38001 / ServerSendChangeMap 38503 /
//                    ServerSendShowEvent 38812）
//   原文如此     1  （ServerSendChangeMap 里 `g_Config.ClientConfigs[73]` 的硬编码下标 73；
//                    该条随方法整体留痕，**不**另计为已移植方法的缺陷）
//   以上三项合计 118 条目（112 + 5 + 1）。
//   ★ 本片**没有**任何"半移植"：凡留痕者整体留痕，凡移植者整条移植。
//
// 移植口径（与 PortKit 一致，见 TPlayObject.PlayerSurface.PortKit.cs）：
//   · Self 比较：原文 `TObject(ProcessMsg.BaseObject) <> Self` 在托管侧无法从裸
//     `nint` 反解出 TPlayObject，故按任务书建议采用**后一种**方案：
//     实例字段 `SelfHandle`（"托管侧的原文 Self 指针替身"）+ 静态接缝
//     `PlayerSurfaceServerSendSeams.SameObject(nint, nint)`。
//     默认实现是引用等价 `(a, b) => a == b`。
//     ⚠ 未接线时 `SelfHandle = 0`、非 Self 报文的 `BaseObject` 也常为 0 ⇒ 守卫会
//     判成"等于 Self"而**不发送**。这正是接缝必须被接线的原因；测试通过显式设置
//     `SelfHandle` / `SameObject` 驱动两条分支。
//   · 其他对象字段（m_nLight / m_nCharStatus / m_WAbil / ...）：一律走
//     `PlayerSurfaceServerSendSeams` 的同名读取接缝，**不臆造 TBaseObject**。
//   · `SendSocket`/`SendSocketEx` → `SendSocketRef`/`SendSocketExRef`（PortKit）。
//   · `SendDefMessage(...)` → 复用**既有** `PlayerSurfaceMsgSeams.SendDefMessage`
//     （TPlayObject.PlayerSurface.Gold.cs:52），**不另造第二份**。
//   · `MakeDefaultMsg`/`MakeWord`/`MakeLong`/`LoWord`/`HiWord` → PortKit
//     `PlayerSurfacePack`；PortKit 的 `LoWord`/`HiWord` 只接 `nint`，本片另加两个
//     `long` 私有重载（`LoWordInt`/`HiWordInt`）承接已窄化/已扩宽的中间量。
//   · `EncodeString`/`EncodeBuffer` → `GXX.Core.Protocol.EDcode`（原文 `EDcode.pas`
//     同名函数，**已存在，直接复用**）。
//   · `IntToStr` → `GXX.Core.Rtl.DelphiRTL.IntToStr`。★ 原书 `IntToStr` 来自
//     `SysUtils`，**有 Int64 重载** ⇒ `IntToStr(ProcessMsg.nParam3)` **不窄化**，
//     故本片直接传 `long`（**不是** `(int)`）。
//   · `zLibCompressBuffer(@rec, SizeOf(rec))` → `EDcode.zLibCompressBuffer(byte[], int)`；
//     `@rec` 的"取结构体字节"用 `StructBytes.BytesOf`（与原文 `Move` 同义）。
//   · `BoolToInt(b)` → `GXX.Core.Util.HUtil32.BoolToInt`（原文 `HUtil32.pas:723`，
//     已存在）。★ HUtil32 在 `GXX.Core.Util` 命名空间（不是 `GXX.Core.Rtl`），
//     且本片**显式全限定**调用，避免与同命名空间的 `ChangeStateCore.BoolToInt` 混淆。
//
// ⚠ 本文件不重复声明任何已存在的字段/方法（逐条 grep 证据见交付报告）：
//   m_DefMsg / SendSocketRef / SendSocketExRef / PortNotPorted / PlayerSurfacePack
//     → PlayerSurface.PortKit.cs:147
//   m_Abil / m_AbilNG                       → PlayerSurface.Core1.cs:351/358
//   m_MyHero                                → PlayerSurface.Hero.cs:31
//   m_btJob / m_btRaceServer / m_nAntiMagic → MagicModel.cs:52 / MagicModel.cs:54
//   m_nCurrX / m_nCurrY / m_btDirection / m_sCharName / m_wAbil → ObjBase.cs:17/18/19/22/65
//   m_nGold / m_nGameGold / m_btPermission / m_btGender → ObjBase.OnlineMsg.cs:44/36/35/16
//   m_btHitPoint / m_btSpeedPoint / m_btAntiPoison / m_nPoisonRecover /
//     m_nHealthRecover / m_nSpellRecover     → RecalcAbilitys.cs:146-151
//   SendUseItems / SendUseMagic / SendMapCanRun → PlayerSurface.Core4.cs:877/916/520
// ============================================================================

using System;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.M2Server.Engine;

/// <summary>
/// `ObjPlayer.pas` 的 `ServerSend*` 族在托管侧的**依赖接缝**。
///
/// **为什么是接缝**：原文这些处理器要从 `ProcessMsg.BaseObject`（托管是裸 `nint`，
/// 见 `MsgQueueConsumeCore.cs:15`）反解出 `TBaseObject` 再读它的字段。托管侧
/// `TBaseObject` 尚未切出（`Engine/ObjBase.cs` 只有 `TCreature`），故按任务书
/// 第 4 条把这些**字段读取**做成显式、可注入的接缝 —— 方法体的其余部分
/// （`&lt;&gt; Self` 守卫、`MakeDefaultMsg` 的 ident/参数顺序、`SendSocket*` 的装配）
/// **仍逐行 1:1 移植**。
///
/// 每个委托的文档注释都标了它在原文里的调用行号与默认值语义。
/// </summary>
public static class PlayerSurfaceServerSendSeams
{
    // ------------------------------------------------------------------
    // 一、Self 身份比较（原文 `TObject(ProcessMsg.BaseObject) <> Self`）
    // ------------------------------------------------------------------

    /// <summary>
    /// 原文 `TObject(ProcessMsg.BaseObject) &lt;&gt; Self` / `= Self` 的判定
    /// （`ObjPlayer.pas:36862 / 36868 / 36968 / 37003 ...` 共数十处）。
    /// 参数：本对象句柄（<see cref="TPlayObject.SelfHandle"/>）/ 报文的 `BaseObject`。
    /// 默认：**引用等价** `a == b`。
    /// ⚠ 未接线且两值都为 `0` 时会误判为"同一对象"——这是接缝必须接线的原因。
    /// </summary>
    public static Func<nint, nint, bool> SameObject { get; set; } = (a, b) => a == b;

    /// <summary>原文 `TBaseObject(ProcessMsg.BaseObject) &lt;&gt; nil` / `= nil`
    /// （本片调用点：37357/38224/38278/38719/38735 —— `BaseObject` 与
    /// `ProcessMsg.nParam3` 两个对象位的"是否为 nil"判定）。默认：`h != 0`。
    /// ⚠ 托管侧 `nint` 无法区分解引用结果，故以"0 视作 nil"为默认语义，
    ///   与原文 `Pointer = nil` 的判定同形。</summary>
    public static Func<nint, bool> ObjectPresent { get; set; } = h => h != 0;

    /// <summary>原文 `m_MyHero` 的**对象位**（用于
    /// `TObject(ProcessMsg.BaseObject) &lt;&gt; m_MyHero` 比较，本片调用点：
    /// 37367/37372/37886/38253/38258/38293/38297）。默认：0。
    /// 宿主应把它接到 `m_MyHero` 的对象标识。</summary>
    public static Func<TPlayObject, nint> HeroHandle { get; set; } = _ => 0;

    // ------------------------------------------------------------------
    // 二、被读对象的字段（原文 `TBaseObject(...)` 强转后读取）
    // ------------------------------------------------------------------

    /// <summary>原文 `TBaseObject(ProcessMsg.BaseObject).m_nLight`（`ObjBase.pas` 字段；
    /// 本片调用点：36865/36890/36907/36910/36916/36926/36971/36982/36992/37965/37972/
    /// 38697/38711/38765/38769）。默认：0（无宿主）。</summary>
    public static Func<nint, int> GetLight { get; set; } = _ => 0;

    /// <summary>原文 `TBaseObject(ProcessMsg.BaseObject).m_nCharStatus`（本片调用点：
    /// 36947/36972/36983/36993/37963/37970/38659/38772）。默认：0。</summary>
    public static Func<nint, int> GetCharStatus { get; set; } = _ => 0;

    /// <summary>原文 `TBaseObject(ProcessMsg.BaseObject).m_btDirection`（本片调用点：
    /// 38742 `MakeDefaultMsg(SM_LIGHTING, ..., m_btDirection)`）。默认：0。</summary>
    public static Func<nint, int> GetDirection { get; set; } = _ => 0;

    /// <summary>原文 `TBaseObject(ProcessMsg.nParam3).m_nCurrX`
    /// （本片调用点：38721/38737，`ServerSendFlyAxe` / `ServerSendLighting` 的攻击者坐标）。
    /// 默认：0。</summary>
    public static Func<nint, int> GetCurrX { get; set; } = _ => 0;

    /// <inheritdoc cref="GetCurrX"/>
    public static Func<nint, int> GetCurrY { get; set; } = _ => 0;

    /// <summary>原文 `TBaseObject(ProcessMsg.BaseObject).m_btJob`（本片调用点：
    /// 37388/38228/38282 `MakeWord(m_btJob, 1|0)`）。默认：0。</summary>
    public static Func<nint, int> GetJob { get; set; } = _ => 0;

    /// <summary>原文 `TBaseObject(ProcessMsg.BaseObject).m_btRaceServer`
    /// （本片调用点：37855/37881 `ServerSendLevelUp` 的两个顶层分支）。默认：0。
    /// ⚠ 原文此处**不做 nil 检查**直接解引用（见 `ServerSendLevelUp` 的 `原文如此` 注）。</summary>
    public static Func<nint, int> GetRaceServer { get; set; } = _ => 0;

    /// <summary>原文 `TBaseObject(ProcessMsg.nParam3).m_btRaceServer`
    /// （本片调用点：38237 —— `ServerSendHealthSpellChanged` 判"攻击者是不是玩家"）。
    /// 默认：0。</summary>
    public static Func<nint, int> GetAttackerRaceServer { get; set; } = _ => 0;

    /// <summary>原文 `TBaseObject(ProcessMsg.nParam3).Master &lt;&gt; nil`
    /// （本片调用点：38238 `(AttackFrom.Master &lt;&gt; nil)`）。默认：false。</summary>
    public static Func<nint, bool> AttackerHasMaster { get; set; } = _ => false;

    /// <summary>原文 `TBaseObject(ProcessMsg.nParam3).Master.m_btRaceServer`
    /// （本片调用点：38238）。默认：0。</summary>
    public static Func<nint, int> GetAttackerMasterRaceServer { get; set; } = _ => 0;

    /// <summary>原文 `TBaseObject(ProcessMsg.BaseObject).GetFeature(Self, @Feature): Integer`
    /// （`ObjBase.pas`；本片调用点：36867/37969/38658/38698/38771）。
    /// 返回"特征字节个数"，并**同时**产出那串特征字节。默认：(0, 空)。</summary>
    public static Func<nint, TPlayObject, (int Feature, byte[] Bytes)> GetFeature { get; set; }
        = (_, _) => (0, Array.Empty<byte>());

    /// <summary>原文 `TSmartObject(BaseObject).m_boMysteriousMan`
    /// （本片调用点：37847，`ServerSendUserName`）。默认：false。</summary>
    public static Func<nint, bool> GetMysteriousMan { get; set; } = _ => false;

    /// <summary>原文 `TBaseObject(ProcessMsg.BaseObject).GetShowName(True): string`
    /// （`ObjBase.pas`；本片调用点：37848）。默认：空串。</summary>
    public static Func<nint, bool, string> GetShowName { get; set; } = (_, _) => "";

    // ------------------------------------------------------------------
    // 三、被读对象的 TAbility 字段（原文 `X.m_WAbil.HP` 等）
    // ------------------------------------------------------------------

    /// <summary>原文 …`m_WAbil.HP`（本片调用点：37359/38232/38245/38286）。默认：0。</summary>
    public static Func<nint, int> GetHP { get; set; } = _ => 0;

    /// <summary>原文 …`m_WAbil.MaxHP`（本片调用点：37360/38246/38287）。默认：0。</summary>
    public static Func<nint, int> GetMaxHP { get; set; } = _ => 0;

    /// <summary>原文 …`m_WAbil.MP`；原文对它取 `LoWord`/`HiWord`（本片调用点：
    /// 37387-37388/38227-38228/38281-38282）。默认：0。</summary>
    public static Func<nint, int> GetMP { get; set; } = _ => 0;

    /// <summary>原文 …`m_WAbil.MaxMP`（本片调用点：37377/38263/38302）。默认：0。</summary>
    public static Func<nint, int> GetMaxMP { get; set; } = _ => 0;

    /// <summary>原文 …`m_WAbil.Level`（本片调用点：37375/38261/38300）。默认：0。</summary>
    public static Func<nint, int> GetLevel { get; set; } = _ => 0;

    // ------------------------------------------------------------------
    // 四、环境与"秘名"标志（原文 `X.m_PEnvir` / `m_PEnvir.m_nSecretFlag`）
    // ------------------------------------------------------------------

    /// <summary>原文 `X.m_PEnvir &lt;&gt; nil`（本片调用点：37364/37369/38250/38255）。
    /// 默认：false（无宿主 ⇒ 原文的 `&lt;&gt; nil` 为假）。</summary>
    public static Func<nint, bool> HasEnvir { get; set; } = _ => false;

    /// <summary>原文 `X.m_PEnvir.m_nSecretFlag`（本片调用点：37365/38251/38291）。
    /// 默认：0。⚠ 原文在 38291 处**不做 nil 检查**（见该方法内 `原文如此` 注）。</summary>
    public static Func<nint, int> GetSecretFlag { get; set; } = _ => 0;

    /// <summary>原文 …`m_nSecretFlag2`（本片调用点：37370/38256/38295）。</summary>
    public static Func<nint, int> GetSecretFlag2 { get; set; } = _ => 0;

    /// <summary>原文 `m_PEnvir.m_boDARK` / `m_PEnvir.m_boDAY`
    /// （本片调用点：38314/38330，`ServerSendDayChangeing` 的昼夜判定）。
    /// 参数：真=白昼(`boDAY`)、假=黑夜(`boDARK`)。默认：false。</summary>
    public static Func<TPlayObject, bool, bool> GetMapDayFlag { get; set; } = (_, _) => false;

    // ------------------------------------------------------------------
    // 五、本片剩余的真实调用（不在本单元，且尚未移植）
    // ------------------------------------------------------------------

    /// <summary>原文 `GetCharColor(BaseObject): Integer`（`ObjBase.pas`；本片调用点：
    /// 36874/36949/37844/37913/38776）。默认：0。</summary>
    public static Func<nint, int> GetCharColor { get; set; } = _ => 0;

    /// <summary>原文 `GetItemInfo(sMsg: string)`（`ObjPlayer.pas`；本片调用点：
    /// 37714/37722，`ServerSendSysMessage(Ex)` 在装配报文**之前**先调它）。
    /// 默认：空操作。</summary>
    public static Action<TPlayObject, string> GetItemInfo { get; set; } = (_, _) => { };

    /// <summary>原文 `MyGetTickCount(): LongWord`（`HUtil32.pas` / `M2Share.pas`）。
    /// 本片调用点：38783/38785/38788/38790/38793/38795（`ServerSendSpaceMoveShow`
    /// 的 6 个 tick 复位）。默认转调 <see cref="DelphiRTL.GetTickCount"/>。</summary>
    public static Func<uint> MyGetTickCount { get; set; } = () => DelphiRTL.GetTickCount();

    /// <summary>恢复全部默认实现（单测隔离用）。</summary>
    public static void ResetDefaults()
    {
        SameObject = (a, b) => a == b;
        ObjectPresent = h => h != 0;
        HeroHandle = _ => 0;
        GetLight = _ => 0;
        GetCharStatus = _ => 0;
        GetDirection = _ => 0;
        GetCurrX = _ => 0;
        GetCurrY = _ => 0;
        GetJob = _ => 0;
        GetRaceServer = _ => 0;
        GetAttackerRaceServer = _ => 0;
        AttackerHasMaster = _ => false;
        GetAttackerMasterRaceServer = _ => 0;
        GetFeature = (_, _) => (0, Array.Empty<byte>());
        GetMysteriousMan = _ => false;
        GetShowName = (_, _) => "";
        GetHP = _ => 0;
        GetMaxHP = _ => 0;
        GetMP = _ => 0;
        GetMaxMP = _ => 0;
        GetLevel = _ => 0;
        HasEnvir = _ => false;
        GetSecretFlag = _ => 0;
        GetSecretFlag2 = _ => 0;
        GetMapDayFlag = (_, _) => false;
        GetCharColor = _ => 0;
        GetItemInfo = (_, _) => { };
        MyGetTickCount = () => DelphiRTL.GetTickCount();
    }
}

/// <summary>
/// `ObjPlayer.pas` 的 **ServerSend* 广播处理器族**（原文 36854-38859，117 条）。
///
/// 每条方法的签名一律是原文
/// `procedure TPlayObject.Xxx(ProcessMsg: pTProcessMessage; var boResult: Boolean);`
/// 的托管等价：`public void Xxx(TProcessMessage ProcessMsg, ref bool boResult)`。
///
/// ★ **没有一条是 `virtual`**（原文这些函数在 `TPlayObject` 的声明段里都是普通方法），
///   故本片全部按普通实例方法移植，**不**加 `virtual`/`override`。
/// </summary>
public partial class TPlayObject
{
    // ==================================================================
    // 本片需要的字段（逐条 grep 确认托管侧无同名成员，见文件头）
    // ==================================================================

    /// <summary>
    /// 原文 `Self`（`TPlayObject` 自身）在托管侧的**指针替身**。
    ///
    /// 原文的 `TObject(ProcessMsg.BaseObject) &lt;&gt; Self` 是**指针比较**；托管侧
    /// `ProcessMsg.BaseObject` 是裸 `nint`，无法反解出 `TPlayObject` 实例，故按任务书
    /// 采用"后一种"方案：本实例持有自己的句柄，比较交由
    /// <see cref="PlayerSurfaceServerSendSeams.SameObject"/>。
    ///
    /// ⚠ 默认 `0`：**必须由宿主接线**（把投递方写进 `ProcessMsg.BaseObject` 的对象标识
    /// 与各 `TPlayObject` 的 `SelfHandle` 对齐），否则守卫语义不成立。
    /// </summary>
    public nint SelfHandle;

    /// <summary>
    /// 原文 `m_nBright: Integer;`（`ObjPlayer.pas` 的 `TPlayObject` 字段，人物亮度 0..3）。
    /// 本片调用点：38318（`case m_nBright of 1 / 0,2 / 3`）与 38333
    /// （`MakeDefaultMsg(SM_DAYCHANGING, 0, m_nBright, nObjCount, 0)`）。
    /// </summary>
    public int m_nBright;

    /// <summary>原文 `m_nNPRecoverTime: Integer;`（人物内力恢复间隔，`ObjPlayer.pas` 字段；
    /// 本片调用点：37878/38752 的
    /// `IntToStr(m_nNPRecoverTime) + '/' + IntToStr(m_nNPRecoverPoint)`）。</summary>
    public int m_nNPRecoverTime;

    /// <summary>原文 `m_nNPRecoverPoint: Integer;`（同上）。</summary>
    public int m_nNPRecoverPoint;

    /// <summary>原文 `m_boReconnection: Boolean;`（`ObjPlayer.pas` 字段；本片调用点：
    /// 38801 `ServerSendReconnection` 置 `True`）。</summary>
    public bool m_boReconnection;

    /// <summary>原文 `m_dwIncGoldTick: LongWord;`（本片调用点：38783）。</summary>
    public uint m_dwIncGoldTick;

    /// <summary>原文 `m_boIncGold: Boolean;`（本片调用点：38784）。</summary>
    public bool m_boIncGold;

    /// <summary>原文 `m_dwDecGoldTick: LongWord;`（本片调用点：38785）。</summary>
    public uint m_dwDecGoldTick;

    /// <summary>原文 `m_boDecGold: Boolean;`（本片调用点：38786）。</summary>
    public bool m_boDecGold;

    /// <summary>原文 `m_dwIncGameGoldTick: LongWord;`（本片调用点：38788）。</summary>
    public uint m_dwIncGameGoldTick;

    /// <summary>原文 `m_boIncGameGold: Boolean;`（本片调用点：38789）。</summary>
    public bool m_boIncGameGold;

    /// <summary>原文 `m_dwDecGameGoldTick: LongWord;`（本片调用点：38790）。</summary>
    public uint m_dwDecGameGoldTick;

    /// <summary>原文 `m_boDecGameGold: Boolean;`（本片调用点：38791）。</summary>
    public bool m_boDecGameGold;

    /// <summary>原文 `m_dwIncGamePointTick: LongWord;`（本片调用点：38793）。</summary>
    public uint m_dwIncGamePointTick;

    /// <summary>原文 `m_boIncGamePoint: Boolean;`（本片调用点：38794）。</summary>
    public bool m_boIncGamePoint;

    /// <summary>原文 `m_dwDecGamePointTick: LongWord;`（本片调用点：38795）。</summary>
    public uint m_dwDecGamePointTick;

    /// <summary>原文 `m_boDecGamePoint: Boolean;`（本片调用点：38796）。</summary>
    public bool m_boDecGamePoint;

    // ==================================================================
    // 本片私有的 1:1 辅助（原文的全局函数在托管侧的落点）
    // ==================================================================

    /// <summary>原文 `LoWord(n: LongWord): Word` 的**非 `nint` 实参**形态
    /// （PortKit 的 <c>PlayerSurfacePack.LoWord</c> 只接 `nint`）。</summary>
    private static ushort LoWordInt(long n) => (ushort)((ulong)n & 0xFFFF);

    /// <summary>原文 `HiWord(n: LongWord): Word` 的**非 `nint` 实参**形态。</summary>
    private static ushort HiWordInt(long n) => (ushort)(((ulong)n >> 16) & 0xFFFF);

    /// <summary>原文 `IntToStr(n: Int64): string`（`SysUtils` 的 **Int64 重载**，
    /// 故 `Integer`/`Int64` 实参都**不窄化**）。</summary>
    private static string IntToStr(long n) => DelphiRTL.IntToStr(n);

    /// <summary>原文 `BoolToInt(b: Boolean): Integer`（`HUtil32.pas:723`）。</summary>
    private static int BoolToInt(bool b) => GXX.Core.Util.HUtil32.BoolToInt(b);

    /// <summary>本对象所对应的"原文英雄对象指针"（`m_MyHero` 的对象位）。</summary>
    private nint HeroHandleOrZero => PlayerSurfaceServerSendSeams.HeroHandle(this);

    /// <summary>`ProcessMsg.nParam1` 等 `Int64` 字段在原文里被当对象指针用时的显式改名。</summary>
    private static nint AsNint(long v) => (nint)v;

    /// <summary>
    /// 原文 `GetFeature(x, @Feature): Integer` + 紧随其后的
    /// `SetLength(Buf, SizeOf(TCharDesc) + Feature); Move(CharDesc, Buf[1], SizeOf(TCharDesc));
    ///  Move(Feature[0], Buf[1 + SizeOf(TCharDesc)], Feature);`
    /// 的托管复刻：返回 `SizeOf(TCharDesc) + Feature` 字节的缓冲（头部 `TCharDesc` + 特征字节）。
    /// </summary>
    private static byte[] BuildCharDescWithFeature(in TCharDesc desc, byte[] feature, int featureLen)
    {
        int descSize = StructBytes.SizeOf<TCharDesc>();
        byte[] buf = new byte[descSize + featureLen];
        StructBytes.ToBytes(desc, buf, 0);
        int copy = Math.Min(featureLen, Math.Max(0, feature?.Length ?? 0));
        if (copy > 0)
            Array.Copy(feature!, 0, buf, descSize, copy);
        return buf;
    }

    /// <summary>原文 `EncodeBuffer(@Rec, SizeOf(Rec)) + EncodeString(sMsg)`（AnsiString 拼接）
    /// 的托管等价：两段编码结果的**字节拼接**。</summary>
    private static byte[] ConcatBytes(byte[] a, byte[] b)
    {
        byte[] r = new byte[a.Length + b.Length];
        Array.Copy(a, 0, r, 0, a.Length);
        Array.Copy(b, 0, r, a.Length, b.Length);
        return r;
    }

    /// <summary>
    /// 原文 `THeroObject(m_MyHero)` 显式强转的托管等价（`ServerSendWinExp:37833`、
    /// `ServerSendLevelUp:37890-37906`）。
    ///
    /// 托管 `m_MyHero` 的声明类型是 <see cref="TCreature"/>（`PlayerSurface.Hero.cs:31`，
    /// 因为原文 `m_MyHero` 就是 `TBaseObject`，用时才强转），而 `m_AbilNG`/
    /// `m_nAntiMagic`/`m_nNPRecoverTime` 等落在 `TPlayObject` 上，故用模式匹配表达同一强转。
    /// ★ 原文的强转**不做运行时检查**（强转错了就是内存乱读）；托管侧在此显式抛异常，
    ///   属"把原文的未定义行为变成明确失败"，是本片唯一的语义升级点，已登记。
    /// </summary>
    private TPlayObject HeroAsPlayObject()
        => m_MyHero as TPlayObject
           ?? throw new InvalidOperationException(
               "ObjPlayer.pas:37833/37890 的 THeroObject(m_MyHero) 强转在托管侧失败：m_MyHero 不是 TPlayObject。");

    // ==================================================================
    // 36854-36885  ServerSendTurn
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendTurn(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:36854-36885`）。</summary>
    public void ServerSendTurn(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 36854-36885
        // 原文 36861：// ////////// 下面是新版的，原封不动 ///////////////////////
        // 原文 36862：if (TObject(ProcessMsg.BaseObject) <> Self) or (ProcessMsg.wIdent = RM_TURN2) then
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject)
            || ProcessMsg.wIdent == Grobal2Const.RM_TURN2)
        {
            TCharDesc CharDesc = default;

            // 原文 36864-36865
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_TURN, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                PlayerSurfacePack.MakeWord((int)ProcessMsg.wParam,
                    PlayerSurfaceServerSendSeams.GetLight(ProcessMsg.BaseObject)));

            // 原文 36867：CharDesc.Feature := TBaseObject(ProcessMsg.BaseObject).GetFeature(TBaseObject(ProcessMsg.BaseObject), @Feature);
            (int featureLen, byte[] featureBytes) = PlayerSurfaceServerSendSeams.GetFeature(ProcessMsg.BaseObject, this);
            CharDesc.Feature = (byte)featureLen;
            // 原文 36868：CharDesc.Status := TBaseObject(ProcessMsg.BaseObject).m_nCharStatus;
            CharDesc.Status = PlayerSurfaceServerSendSeams.GetCharStatus(ProcessMsg.BaseObject);

            // 原文 36870-36872：SetLength(s1C, SizeOf(TCharDesc) + CharDesc.Feature); Move(CharDesc, s1C[1], ...); Move(Feature[0], ...);
            byte[] s1C = BuildCharDescWithFeature(CharDesc, featureBytes, featureLen);

            // 原文 36874：nObjCount := GetCharColor(TBaseObject(ProcessMsg.BaseObject));
            int nObjCount = PlayerSurfaceServerSendSeams.GetCharColor(ProcessMsg.BaseObject);
            // 原文 36875-36881
            if (ProcessMsg.wIdent == Grobal2Const.RM_100HIT)
            {
                if (ProcessMsg.sMsg != "")
                    s1C = ConcatBytes(s1C, EDcode.EncodeString("|" + ProcessMsg.sMsg));
            }
            else if (ProcessMsg.sMsg != "")
                s1C = ConcatBytes(s1C, EDcode.EncodeString(ProcessMsg.sMsg + "/" + IntToStr(nObjCount)));

            // 原文 36883：SendSocket(@m_DefMsg, s1C);
            SendSocketRef(m_DefMsg, DelphiRTL.AnsiString(s1C));
        }
    }

    // ==================================================================
    // 36887-36892  ServerSendTurnEx
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendTurnEx(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:36887-36892`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendTurnEx(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 36887-36892
        // 原文 36889-36890
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_TURN, ProcessMsg.BaseObject,
            (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
            PlayerSurfacePack.MakeWord((int)ProcessMsg.wParam,
                PlayerSurfaceServerSendSeams.GetLight(ProcessMsg.BaseObject)));
        // 原文 36891
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 36894-36964  ServerSendRush
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendRush(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:36894-36964`）。
    /// ★ **无 `&lt;&gt; Self` 守卫**；`case ProcessMsg.wIdent of` **无 `else` 分支** ——
    ///   未匹配的 ident 会让 `m_DefMsg` 保持**上一次**的值（对象字段，不会被清零）后照样
    ///   `SendSocket`（原文如此）。</summary>
    public void ServerSendRush(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 36894-36964
        TCharDesc CharDesc = default;
        int nObjCount;
        string s1C;
        int nIndex;

        // 原文 36901-36945
        switch (ProcessMsg.wIdent)
        {
            case Grobal2Const.RM_PUSH:
                // 原文 36903-36904
                m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                    Grobal2Const.SM_BACKSTEP, ProcessMsg.BaseObject,
                    (int)ProcessMsg.nParam1 /* x */, (int)ProcessMsg.nParam2 /* y */,
                    PlayerSurfacePack.MakeWord((int)ProcessMsg.wParam /* dir */, 1 /* Step */));
                break;
            case Grobal2Const.RM_RUSH:
                // 原文 36906-36907
                m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                    Grobal2Const.SM_RUSH, ProcessMsg.BaseObject,
                    (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                    PlayerSurfacePack.MakeWord((int)ProcessMsg.wParam,
                        PlayerSurfaceServerSendSeams.GetLight(ProcessMsg.BaseObject)));
                break;
            case Grobal2Const.RM_RUSHKUNG:
                // 原文 36909-36910
                m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                    Grobal2Const.SM_RUSHKUNG, ProcessMsg.BaseObject,
                    (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                    PlayerSurfacePack.MakeWord((int)ProcessMsg.wParam,
                        PlayerSurfaceServerSendSeams.GetLight(ProcessMsg.BaseObject)));
                break;
            case Grobal2Const.RM_100HIT: // 追心刺
                // 原文 36912-36913
                m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                    Grobal2Const.SM_100HIT, ProcessMsg.BaseObject,
                    (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                    PlayerSurfacePack.MakeWord((int)ProcessMsg.wParam, (int)ProcessMsg.nParam3));
                break;
            case Grobal2Const.RM_MAGICMOVE: // 十步一杀
                // 原文 36915-36916
                m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                    Grobal2Const.SM_MAGICMOVE, ProcessMsg.BaseObject,
                    (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                    PlayerSurfacePack.MakeWord((int)ProcessMsg.wParam,
                        PlayerSurfaceServerSendSeams.GetLight(ProcessMsg.BaseObject)));
                break;
            case Grobal2Const.RM_CUSTOM_MAGICMOVE: // 自定义技能 瞬移
                {
                    // 原文 36919-36928
                    nIndex = (int)ProcessMsg.nParam3 - Grobal2Const.CUSTOM_MAGIC_START_ID;
                    if (nIndex >= 0)
                    {
                        nIndex = Grobal2Const.SM_CUSTOM_MAGICMOVE001 + nIndex;
                        if (nIndex >= Grobal2Const.SM_CUSTOM_MAGICMOVE001
                            && nIndex < Grobal2Const.SM_CUSTOM_MAGICMOVE001 + Grobal2Const.CUSTOM_MAGIC_COUNT)
                        {
                            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                                nIndex, ProcessMsg.BaseObject,
                                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                                PlayerSurfacePack.MakeWord((int)ProcessMsg.wParam,
                                    PlayerSurfaceServerSendSeams.GetLight(ProcessMsg.BaseObject)));
                        }
                    }
                }
                break;
            case Grobal2Const.RM_CUSTOM_PUSH: // 自定义技能 追心刺推动方式
                {
                    // 原文 36932-36943
                    nIndex = PlayerSurfacePack.HiWord(ProcessMsg.wParam) - Grobal2Const.CUSTOM_MAGIC_START_ID;
                    // 原文 36933：ProcessMsg.wParam := LoWord(ProcessMsg.wParam);
                    // ★ 这是对**调用方消息对象**的原地改写（真实副作用），必须保留。
                    ProcessMsg.wParam = PlayerSurfacePack.LoWord(ProcessMsg.wParam);
                    if (nIndex >= 0)
                    {
                        nIndex = Grobal2Const.SM_CUSTOM_PUSH001 + nIndex;
                        if (nIndex >= Grobal2Const.SM_CUSTOM_PUSH001
                            && nIndex < Grobal2Const.SM_CUSTOM_PUSH001 + Grobal2Const.CUSTOM_MAGIC_COUNT)
                        {
                            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                                nIndex, ProcessMsg.BaseObject,
                                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                                PlayerSurfacePack.MakeWord((int)ProcessMsg.wParam, (int)ProcessMsg.nParam3));
                            // 原文 36941 的被注释掉的实参：
                            // { MakeWord(ProcessMsg.wParam, TBaseObject(ProcessMsg.BaseObject).m_nLight) }
                        }
                    }
                }
                break;
        }

        // 原文 36946-36947：CharDesc.Feature := 0; CharDesc.Status := TBaseObject(ProcessMsg.BaseObject).m_nCharStatus;
        CharDesc.Feature = 0;
        CharDesc.Status = PlayerSurfaceServerSendSeams.GetCharStatus(ProcessMsg.BaseObject);
        // 原文 36948：s1C := EncodeBuffer(@CharDesc, SizeOf(TCharDesc));
        s1C = DelphiRTL.AnsiString(EDcode.EncodeBuffer(
            StructBytes.BytesOf(CharDesc), StructBytes.SizeOf<TCharDesc>()));
        // 原文 36949：nObjCount := GetCharColor(TBaseObject(ProcessMsg.BaseObject));
        nObjCount = PlayerSurfaceServerSendSeams.GetCharColor(ProcessMsg.BaseObject);
        // 原文 36950-36959
        if (ProcessMsg.wIdent == Grobal2Const.RM_100HIT || ProcessMsg.wIdent == Grobal2Const.RM_CUSTOM_PUSH)
        {
            if (ProcessMsg.sMsg != "")
                s1C = s1C + "|" + ProcessMsg.sMsg;
        }
        else
        {
            if (ProcessMsg.sMsg != "")
                s1C = s1C + DelphiRTL.AnsiString(EDcode.EncodeString(ProcessMsg.sMsg + "/" + IntToStr(nObjCount)));
        }
        // 原文 36960
        SendSocketRef(m_DefMsg, s1C);
        // 原文 36961-36963
        if (PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject)
            && m_btRaceServer == Grobal2Const.RC_PLAYOBJECT
            && (ProcessMsg.wIdent == Grobal2Const.RM_PUSH || ProcessMsg.wIdent == Grobal2Const.RM_RUSH))
            SendMapCanRun();
    }

    // ==================================================================
    // 36966-36974  ServerSendWalk
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendWalk(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:36966-36974`）。</summary>
    public void ServerSendWalk(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 36966-36974
        // 原文 36968：if TObject(ProcessMsg.BaseObject) <> Self then
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 36970-36971
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_WALK, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                PlayerSurfacePack.MakeWord((int)ProcessMsg.wParam,
                    PlayerSurfaceServerSendSeams.GetLight(ProcessMsg.BaseObject)));
            // 原文 36972：SendSocketEx(@m_DefMsg, @TBaseObject(ProcessMsg.BaseObject).m_nCharStatus, SizeOf(Integer));
            SendSocketExRef(m_DefMsg, StructBytes.BytesOf(
                PlayerSurfaceServerSendSeams.GetCharStatus(ProcessMsg.BaseObject)));
        }
    }

    // ==================================================================
    // 36976-36985  ServerSendRun
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendRun(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:36976-36985`）。</summary>
    public void ServerSendRun(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 36976-36985
        // 原文 36978：if TObject(ProcessMsg.BaseObject) <> Self then
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 36980（注释）：// MainOutMessage('向' + m_sCharName + '发送' + TBaseObject(ProcessMsg.BaseObject).m_sCharName + '跑步' + IntToStr(MyGetTickCount));
            // 原文 36981-36982
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_RUN, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                PlayerSurfacePack.MakeWord((int)ProcessMsg.wParam,
                    PlayerSurfaceServerSendSeams.GetLight(ProcessMsg.BaseObject)));
            // 原文 36983
            SendSocketExRef(m_DefMsg, StructBytes.BytesOf(
                PlayerSurfaceServerSendSeams.GetCharStatus(ProcessMsg.BaseObject)));
        }
    }

    // ==================================================================
    // 36987-36995  ServerSendHorseRun
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendHorseRun(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:36987-36995`）。</summary>
    public void ServerSendHorseRun(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 36987-36995
        // 原文 36989：if TObject(ProcessMsg.BaseObject) <> Self then
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 36991-36992
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_HORSERUN, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                PlayerSurfacePack.MakeWord((int)ProcessMsg.wParam,
                    PlayerSurfaceServerSendSeams.GetLight(ProcessMsg.BaseObject)));
            // 原文 36993
            SendSocketExRef(m_DefMsg, StructBytes.BytesOf(
                PlayerSurfaceServerSendSeams.GetCharStatus(ProcessMsg.BaseObject)));
        }
    }

    // ==================================================================
    // 36997-36999  ServerSendSitDown （原文空体）
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendSitDown(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:36997-36999`）——**原文就是这样：`begin end;` 空过程体**。
    /// 本移植体同样为空方法体（既不是 `=> true;` 桩，也不是 `PortNotPorted`）。</summary>
    public void ServerSendSitDown(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 36997-36999
        // 原文 36998-36999：begin end;  ← 原文如此（空过程体，什么都不做）
    }

    // ==================================================================
    // 37001-37015  ServerSendHit
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendHit(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37001-37015`）。</summary>
    public void ServerSendHit(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37001-37015
        // 原文 37003
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37005-37010 是被注释掉的调试块（原文如此，原样保留为注释）：
            //   if TObject(ProcessMsg.BaseObject) = m_MyHero then
            //   begin
            //     MainOutMessage('~~~~HeroSend: ' + IntToStr(MyGetTickCount - THeroObject(m_MyHero).m_dwLastAttackTick));
            //   end;
            // 原文 37011-37012
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_HIT, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37013
            SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
        }
    }

    // ==================================================================
    // 37017-37031  ServerSendHeavyHit
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendHeavyHit(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37017-37031`）。</summary>
    public void ServerSendHeavyHit(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37017-37031
        // 原文 37019
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37021-37026（注释掉的 HeroSend 调试块，原文如此）
            // 原文 37027-37028
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_HEAVYHIT, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37029
            SendSocketRef(m_DefMsg, "");
        }
    }

    // ==================================================================
    // 37033-37047  ServerSendBigHit
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendBigHit(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37033-37047`）。</summary>
    public void ServerSendBigHit(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37033-37047
        // 原文 37035
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37037-37042（注释掉的 HeroSend 调试块，原文如此）
            // 原文 37043-37044
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_BIGHIT, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37045
            SendSocketRef(m_DefMsg, "");
        }
    }

    // ==================================================================
    // 37049-37063  ServerSendPowerHit
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendPowerHit(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37049-37063`）。</summary>
    public void ServerSendPowerHit(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37049-37063
        // 原文 37051
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37053-37058（注释掉的 HeroSend 调试块，原文如此）
            // 原文 37059-37060
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_POWERHIT, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37061
            SendSocketRef(m_DefMsg, "");
        }
    }

    // ==================================================================
    // 37065-37079  ServerSendLongHit
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendLongHit(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37065-37079`）。</summary>
    public void ServerSendLongHit(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37065-37079
        // 原文 37067
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37069-37074（注释掉的 HeroSend 调试块，原文如此）
            // 原文 37075-37076
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_LONGHIT, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37077
            SendSocketRef(m_DefMsg, IntToStr(ProcessMsg.nParam3));
        }
    }

    // ==================================================================
    // 37081-37095  ServerSendWideHit
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendWideHit(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37081-37095`）。</summary>
    public void ServerSendWideHit(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37081-37095
        // 原文 37083
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37085-37090（注释掉的 HeroSend 调试块，原文如此）
            // 原文 37091-37092
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_WIDEHIT, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37093
            SendSocketRef(m_DefMsg, IntToStr(ProcessMsg.nParam3));
        }
    }

    // ==================================================================
    // 37097-37111  ServerSendFireHit
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendFireHit(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37097-37111`）。</summary>
    public void ServerSendFireHit(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37097-37111
        // 原文 37099
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37101-37106（注释掉的 HeroSend 调试块，原文如此）
            // 原文 37107-37108
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_FIREHIT, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37109
            SendSocketRef(m_DefMsg, IntToStr(ProcessMsg.nParam3));
        }
    }

    // ==================================================================
    // 37113-37127  ServerSendCrsHit
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendCrsHit(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37113-37127`）。</summary>
    public void ServerSendCrsHit(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37113-37127
        // 原文 37115
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37117-37122（注释掉的 HeroSend 调试块，原文如此）
            // 原文 37123-37124
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_CRSHIT, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37125
            SendSocketRef(m_DefMsg, "");
        }
    }

    // ==================================================================
    // 37129-37143  ServerSendSWordHit
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendSWordHit(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37129-37143`）。</summary>
    public void ServerSendSWordHit(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37129-37143
        // 原文 37131
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37133-37138（注释掉的 HeroSend 调试块，原文如此）
            // 原文 37139-37140
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_SWORDHIT, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37141
            SendSocketRef(m_DefMsg, IntToStr(ProcessMsg.nParam3));
        }
    }

    // ==================================================================
    // 37145-37159  ServerSendTwnHit
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendTwnHit(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37145-37159`）。</summary>
    public void ServerSendTwnHit(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37145-37159
        // 原文 37147
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37149-37154（注释掉的 HeroSend 调试块，原文如此）
            // 原文 37155-37156
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_TWNHIT, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37157
            SendSocketRef(m_DefMsg, "");
        }
    }

    // ==================================================================
    // 37161-37175  ServerSend43Hit
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSend43Hit(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37161-37175`）。</summary>
    public void ServerSend43Hit(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37161-37175
        // 原文 37163
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37165-37170（注释掉的 HeroSend 调试块，原文如此）
            // 原文 37171-37172
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_43HIT, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37173
            SendSocketRef(m_DefMsg, "");
        }
    }

    // ==================================================================
    // 37177-37191  ServerSend60Hit
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSend60Hit(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37177-37191`）。</summary>
    public void ServerSend60Hit(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37177-37191
        // 原文 37179
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37181-37186（注释掉的 HeroSend 调试块，原文如此）
            // 原文 37187-37188
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_60HIT, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37189
            SendSocketRef(m_DefMsg, "");
        }
    }

    // ==================================================================
    // 37193-37207  ServerSend61Hit
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSend61Hit(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37193-37207`）。</summary>
    public void ServerSend61Hit(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37193-37207
        // 原文 37195
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37197-37202（注释掉的 HeroSend 调试块，原文如此）
            // 原文 37203-37204
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_61HIT, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37205
            SendSocketRef(m_DefMsg, "");
        }
    }

    // ==================================================================
    // 37209-37223  ServerSend62Hit
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSend62Hit(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37209-37223`）。</summary>
    public void ServerSend62Hit(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37209-37223
        // 原文 37211
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37213-37218（注释掉的 HeroSend 调试块，原文如此）
            // 原文 37219-37220
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_62HIT, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37221
            SendSocketRef(m_DefMsg, "");
        }
    }

    // ==================================================================
    // 37225-37239  ServerSend66Hit
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSend66Hit(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37225-37239`）。</summary>
    public void ServerSend66Hit(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37225-37239
        // 原文 37227
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37229-37234（注释掉的 HeroSend 调试块，原文如此）
            // 原文 37235-37236
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_66HIT, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37237
            SendSocketRef(m_DefMsg, "");
        }
    }

    // ==================================================================
    // 37242-37256  ServerSend66Hit1
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSend66Hit1(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37242-37256`；原文 37241 注释「开天斩轻击 piaoyun 2013-08-24」）。</summary>
    public void ServerSend66Hit1(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37242-37256
        // 原文 37244
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37246-37251（注释掉的 HeroSend 调试块，原文如此）
            // 原文 37252-37253
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_66HIT1, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37254
            SendSocketRef(m_DefMsg, "");
        }
    }

    // ==================================================================
    // 37258-37266  ServerSend101Hit
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSend101Hit(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37258-37266`）。</summary>
    public void ServerSend101Hit(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37258-37266
        // 原文 37260
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37262-37263
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_101HIT, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37264
            SendSocketRef(m_DefMsg, "");
        }
    }

    // ==================================================================
    // 37268-37276  ServerSend102Hit
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSend102Hit(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37268-37276`）。</summary>
    public void ServerSend102Hit(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37268-37276
        // 原文 37270
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37272-37273
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_102HIT, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37274
            SendSocketRef(m_DefMsg, "");
        }
    }

    // ==================================================================
    // 37278-37286  ServerSend103Hit
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSend103Hit(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37278-37286`）。</summary>
    public void ServerSend103Hit(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37278-37286
        // 原文 37280
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37282-37283
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_103HIT, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37284
            SendSocketRef(m_DefMsg, "");
        }
    }

    // ==================================================================
    // 37288-37296  ServerSend113Hit
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSend113Hit(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37288-37296`）。</summary>
    public void ServerSend113Hit(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37288-37296
        // 原文 37290
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37292-37293
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_113HIT, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37294
            SendSocketRef(m_DefMsg, "");
        }
    }

    // ==================================================================
    // 37298-37306  ServerSend115Hit
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSend115Hit(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37298-37306`）。</summary>
    public void ServerSend115Hit(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37298-37306
        // 原文 37300
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37302-37303
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_115HIT, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37304
            SendSocketRef(m_DefMsg, "");
        }
    }

    // ==================================================================
    // 37308-37313  ServerSend115HitTargetEffect
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSend115HitTargetEffect(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37308-37313`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSend115HitTargetEffect(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37308-37313
        // 原文 37310-37311
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_115HIT_TARGET_EFFECT, ProcessMsg.BaseObject,
            PlayerSurfacePack.LoWord(ProcessMsg.wParam), PlayerSurfacePack.HiWord(ProcessMsg.wParam),
            (int)ProcessMsg.wParam);
        // 原文 37312
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 37315-37323  ServerSendCustomHit
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendCustomHit(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37315-37323`）。
    /// ⚠ 原文 `SM_CUSTOM_HIT001 + ProcessMsg.nParam3` **不做范围校验**——越界 ident
    ///   照样下发（原文如此；`MakeDefaultMsg` 第一参数是 `Word`，故高位**静默窄化**）。</summary>
    public void ServerSendCustomHit(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37315-37323
        // 原文 37317
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37319-37320
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_CUSTOM_HIT001 + (int)ProcessMsg.nParam3, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37321
            SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
        }
    }

    // ==================================================================
    // 37325-37330  ServerSendCustomHitTargetEff
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendCustomHitTargetEff(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37325-37330`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendCustomHitTargetEff(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37325-37330
        // 原文 37327-37328
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_CUSTOM_HIT_TARGET_EFF, ProcessMsg.BaseObject,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam1,
            (int)ProcessMsg.nParam2);
        // 原文 37329
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 37332-37337  ServerSendCustomMagicSelfKeepPlay
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendCustomMagicSelfKeepPlay(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37332-37337`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendCustomMagicSelfKeepPlay(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37332-37337
        // 原文 37334-37335
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_CUSTOM_MAGIC_SELFKEEP_PLAY, ProcessMsg.BaseObject,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam1,
            0);
        // 原文 37336
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 37339-37347  ServerSendMonMove
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendMonMove(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37339-37347`）。
    /// ⚠ 原文如此：ident 用的是 **`SM_SITDOWN`**（不是 `SM_MONMOVE` 之类），照抄。</summary>
    public void ServerSendMonMove(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37339-37347
        // 原文 37341
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37343-37344
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_SITDOWN, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                (int)ProcessMsg.wParam);
            // 原文 37345
            SendSocketRef(m_DefMsg, "");
        }
    }

    // ==================================================================
    // 37349-37391  ServerSendHealthSpellChangedStruck
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendHealthSpellChangedStruck(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37349-37391`）。</summary>
    public void ServerSendHealthSpellChangedStruck(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37349-37391
        TNewMessageBodyWL NewMessageBodyWL = default;

        // 原文 37354-37355（注释）：
        //   修正护身只掉蓝时显示错误 chongchong 2017-09-28
        //   由于 TBaseObject.DamageHealth 中要用 SendRefMsg(RM_HEALTHSPELLCHANGED_STRUCK, 0, 0, 0, 0, '', 0);
        //     将 ProcessMsg.wParam > 0 改为 ProcessMsg.wParam >= 0
        // 原文 37356：BaseObject := TBaseObject(ProcessMsg.BaseObject);
        // 原文 37357：if (BaseObject <> nil) { and (ProcessMsg.wParam >= 0) } then
        //   ⚠ 原文如此：`wParam >= 0` 那一半被注释掉了，故这里**只有** nil 判定。
        if (PlayerSurfaceServerSendSeams.ObjectPresent(ProcessMsg.BaseObject))
        {
            // 原文 37359-37361
            NewMessageBodyWL.lParam1 = PlayerSurfaceServerSendSeams.GetHP(ProcessMsg.BaseObject);
            NewMessageBodyWL.lParam2 = PlayerSurfaceServerSendSeams.GetMaxHP(ProcessMsg.BaseObject);
            NewMessageBodyWL.lTag1 = (int)ProcessMsg.wParam; // 当前血量改变

            // 原文 37363-37375：修正浑水摸鱼地图，英雄受到攻击后，等级为0 2020-05-12 22:21:22
            if (PlayerSurfaceServerSendSeams.HasEnvir(ProcessMsg.BaseObject)
                && (PlayerSurfaceServerSendSeams.GetSecretFlag(ProcessMsg.BaseObject)
                    & PlayerSurfaceServerSend2Const.SecretFlag_ShowEqualName) != 0
                && !PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject)
                && !PlayerSurfaceServerSendSeams.SameObject(HeroHandleOrZero, ProcessMsg.BaseObject))
                NewMessageBodyWL.lTag2 = 0;
            else if (PlayerSurfaceServerSendSeams.HasEnvir(ProcessMsg.BaseObject)
                && (PlayerSurfaceServerSendSeams.GetSecretFlag2(ProcessMsg.BaseObject)
                    & PlayerSurfaceServerSend2Const.SecretFlag_ShowEqualName) != 0
                && !PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject)
                && !PlayerSurfaceServerSendSeams.SameObject(HeroHandleOrZero, ProcessMsg.BaseObject))
                NewMessageBodyWL.lTag2 = 0;
            else
                NewMessageBodyWL.lTag2 = PlayerSurfaceServerSendSeams.GetLevel(ProcessMsg.BaseObject);

            // 原文 37377-37378
            NewMessageBodyWL.lTag3 = PlayerSurfaceServerSendSeams.GetMaxMP(ProcessMsg.BaseObject);
            NewMessageBodyWL.lTag4 = PlayerSurfaceServerSendSeams.GetCharStatus(ProcessMsg.BaseObject); // 受攻击者状态
            // 原文 37379-37381（注释：这个消息只是施毒用…）
            NewMessageBodyWL.lTag5 = 0;

            // 原文 37383-37385
            NewMessageBodyWL.BlastHitType = GXX.Core.Protocol.TBlastHitType.bhtNone;
            NewMessageBodyWL.ResID = (int)ProcessMsg.nParam1;       // Cursor 2023-06-08 14:08:24 HumanHP命令参数5
            NewMessageBodyWL.ResStartIdx = (int)ProcessMsg.nParam2; // Cursor 2023-06-08 14:08:24 HumanHP命令参数6

            // 原文 37387-37388
            int mp = PlayerSurfaceServerSendSeams.GetMP(ProcessMsg.BaseObject);
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_HEALTHSPELLCHANGED, ProcessMsg.BaseObject,
                LoWordInt(mp), HiWordInt(mp),
                PlayerSurfacePack.MakeWord(PlayerSurfaceServerSendSeams.GetJob(ProcessMsg.BaseObject), 1));
            // 原文 37389
            SendSocketExRef(m_DefMsg, StructBytes.BytesOf(NewMessageBodyWL));
        }
    }

    // ==================================================================
    // 37393-37575  ServerSendStruck —— 留痕
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendStruck(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37393-37575`，**183 行**）。
    /// <para><b>未移植（显式留痕）</b>。缺失成员（原文行号）：
    /// <c>SetPKFlag(AttackFrom.m_Master)</c>（:37424）与 <c>SetPKFlag(AttackFrom)</c>（:37428）
    /// —— 托管侧全仓无 `SetPKFlag`；
    /// <c>g_CastleManager.IsCastleMember(Self)</c>（:37437）—— 托管 <c>Castle.cs</c> 无该重载；
    /// <c>TBaseObject.bo2B0</c>（:37439）与 <c>m_dw2B4Tick</c>（:37440）；
    /// <c>SetLastHiter</c>（:37432）；
    /// <c>m_nHealthTick</c>/<c>m_nSpellTick</c>/<c>m_nPerHealth</c>/<c>m_nPerSpell</c>/
    /// <c>m_dwStruckTick</c>（:37443-37447，本对象状态字段，托管侧未切出）；
    /// <c>g_Config.boDisableSelfStruck</c>/<c>boDisableStruck</c>/<c>boHeroDisableSelfStruck</c>/
    /// <c>boHeroDisableStruck</c>/<c>boSlaveDisableStruck</c>（:37455-37462）；
    /// <c>ProcessMsg.nParam3</c> 的"攻击者"对象面（<c>m_btRaceServer</c>/<c>Master</c>，
    /// :37487-37489）与 <c>TempObject.m_Master</c>（:37461）。
    /// 其中 <c>BlastHitTypeValues</c>（:37492-37503）与 <c>TNewMessageBodyWL</c> 已存在，
    /// 但不足以补齐本方法。</para>
    /// </summary>
    public void ServerSendStruck(TProcessMessage ProcessMsg, ref bool boResult)
    {
        PortNotPorted(nameof(ServerSendStruck), 37393);
    }

    // ==================================================================
    // 37577-37691  ServerSendMagicshieldStruck —— 留痕
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendMagicshieldStruck(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37577-37691`，**115 行**）。
    /// <para><b>未移植（显式留痕）</b>。缺失成员（原文行号）：
    /// <c>g_Config.boDisableSelfStruck</c>/<c>boDisableStruck</c>/<c>boHeroDisableSelfStruck</c>/
    /// <c>boHeroDisableStruck</c>（:37590-37594）；
    /// <c>ProcessMsg.nParam3</c> 攻击者对象面的 <c>m_btRaceServer</c>/<c>Master</c>
    /// （:37618-37620）；与 <see cref="ServerSendStruck"/> 同源的
    /// <c>BlastHitTypeValues</c> 比较（:37665-37676）已存在但不足以补齐。
    /// <c>TNewMessageBodyWL</c> 与 <c>SendSocketEx</c> 已就绪。</para>
    /// </summary>
    public void ServerSendMagicshieldStruck(TProcessMessage ProcessMsg, ref bool boResult)
    {
        PortNotPorted(nameof(ServerSendMagicshieldStruck), 37577);
    }

    // ==================================================================
    // 37693-37697  ServerSendHear
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendHear(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37693-37697`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendHear(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37693-37697
        // 原文 37695
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_HEAR, ProcessMsg.BaseObject,
            PlayerSurfacePack.MakeWord((int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2), 0, 1);
        // 原文 37696
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 37699-37704  ServerSendWhisper
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendWhisper(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37699-37704`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendWhisper(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37699-37704
        // 原文 37701-37702
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_WHISPER, ProcessMsg.BaseObject,
            PlayerSurfacePack.MakeWord((int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2), 0, 1);
        // 原文 37703
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 37706-37710  ServerSendCry
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendCry(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37706-37710`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendCry(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37706-37710
        // 原文 37708
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_CRY, ProcessMsg.BaseObject,
            PlayerSurfacePack.MakeWord((int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2), 0, 1);
        // 原文 37709
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 37712-37718  ServerSendSysMessage
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendSysMessage(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37712-37718`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendSysMessage(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37712-37718
        // 原文 37714：GetItemInfo(ProcessMsg.sMsg);
        PlayerSurfaceServerSendSeams.GetItemInfo(this, ProcessMsg.sMsg);
        // 原文 37715-37716
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_SYSMESSAGE, ProcessMsg.BaseObject,
            PlayerSurfacePack.MakeWord((int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2),
            (int)ProcessMsg.wParam, 1);
        // 原文 37717
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 37720-37726  ServerSendSysMessageEx
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendSysMessageEx(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37720-37726`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。
    /// ⚠ 原文如此：本方法与 <see cref="ServerSendSysMessage"/> **逐字相同**
    ///   （含 `GetItemInfo` 调用与 `SM_SYSMESSAGE`），**没有**用 `SM_SYSMESSAGE_EX`。</summary>
    public void ServerSendSysMessageEx(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37720-37726
        // 原文 37722：GetItemInfo(ProcessMsg.sMsg);
        PlayerSurfaceServerSendSeams.GetItemInfo(this, ProcessMsg.sMsg);
        // 原文 37723-37724
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_SYSMESSAGE, ProcessMsg.BaseObject,
            PlayerSurfacePack.MakeWord((int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2),
            (int)ProcessMsg.wParam, 1);
        // 原文 37725
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 37728-37733  ServerSendGroupMessage
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendGroupMessage(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37728-37733`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。
    /// ⚠ 原文如此：ident 用的是 **`SM_SYSMESSAGE`**（不是 `SM_GROUPMESSAGE`），照抄。</summary>
    public void ServerSendGroupMessage(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37728-37733
        // 原文 37730-37731
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_SYSMESSAGE, ProcessMsg.BaseObject,
            PlayerSurfacePack.MakeWord((int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2), 0, 1);
        // 原文 37732
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 37735-37740  ServerSendGuildMessage
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendGuildMessage(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37735-37740`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendGuildMessage(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37735-37740
        // 原文 37737-37738
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_GUILDMESSAGE, ProcessMsg.BaseObject,
            PlayerSurfacePack.MakeWord((int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2), 0, 1);
        // 原文 37739
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 37742-37747  ServerSendMerchantSay
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendMerchantSay(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37742-37747`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendMerchantSay(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37742-37747
        // 原文 37744-37745
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_MERCHANTSAY, ProcessMsg.BaseObject,
            PlayerSurfacePack.MakeWord((int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2), 0, 1);
        // 原文 37746
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 37749-37753  ServerSendMoveMessage
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendMoveMessage(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37749-37753`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。
    /// ⚠ 原文如此：`nRecog` 取的是 **`ProcessMsg.nParam3`**（不是 `BaseObject`）。</summary>
    public void ServerSendMoveMessage(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37749-37753
        // 原文 37751
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_MOVEMESSAGE, ProcessMsg.nParam3,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2);
        // 原文 37752
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 37755-37759  ServerSendMoveMessageEx
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendMoveMessageEx(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37755-37759`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。
    /// ⚠ 原文如此：与 <see cref="ServerSendMoveMessage"/> 逐字相同
    ///   （ident 同为 `SM_MOVEMESSAGE`）。</summary>
    public void ServerSendMoveMessageEx(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37755-37759
        // 原文 37757
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_MOVEMESSAGE, ProcessMsg.nParam3,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2);
        // 原文 37758
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 37761-37765  ServerSendNewMoveMessage
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendNewMoveMessage(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37761-37765`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendNewMoveMessage(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37761-37765
        // 原文 37763
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_MOVEMESSAGE_NEW, ProcessMsg.nParam3,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2);
        // 原文 37764
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 37767-37771  ServerSendDelayMessage
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendDelayMessage(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37767-37771`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendDelayMessage(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37767-37771
        // 原文 37769
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_DELAYMESSAGE, ProcessMsg.nParam1,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam2, (int)ProcessMsg.nParam3);
        // 原文 37770
        SendSocketRef(m_DefMsg, DelphiRTL.AnsiString(EDcode.EncodeString(ProcessMsg.sMsg)));
    }

    // ==================================================================
    // 37773-37777  ServerSendMoveHintMsg
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendMoveHintMsg(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37773-37777`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendMoveHintMsg(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37773-37777
        // 原文 37775
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_MOVEHINTMSG, ProcessMsg.nParam1,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam2, (int)ProcessMsg.nParam3);
        // 原文 37776
        SendSocketRef(m_DefMsg, DelphiRTL.AnsiString(EDcode.EncodeString(ProcessMsg.sMsg)));
    }

    // ==================================================================
    // 37779-37783  ServerSendCenterMessage
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendCenterMessage(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37779-37783`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendCenterMessage(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37779-37783
        // 原文 37781
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_CENTERMESSAGE, ProcessMsg.nParam3,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2);
        // 原文 37782
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 37785-37789  ServerSendCenterMessageEx
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendCenterMessageEx(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37785-37789`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。
    /// ⚠ 原文如此：与 <see cref="ServerSendCenterMessage"/> 逐字相同。</summary>
    public void ServerSendCenterMessageEx(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37785-37789
        // 原文 37787
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_CENTERMESSAGE, ProcessMsg.nParam3,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2);
        // 原文 37788
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 37791-37796  ServerSendTopChatBoardMessage
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendTopChatBoardMessage(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37791-37796`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendTopChatBoardMessage(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37791-37796
        // 原文 37793-37794
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_TOPCHATBOARDMESSAGE, ProcessMsg.nParam3,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2);
        // 原文 37795
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 37798-37803  ServerSendTopChatBoardMessageEx
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendTopChatBoardMessageEx(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37798-37803`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。
    /// ⚠ 原文如此：与 <see cref="ServerSendTopChatBoardMessage"/> 逐字相同。</summary>
    public void ServerSendTopChatBoardMessageEx(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37798-37803
        // 原文 37800-37801
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_TOPCHATBOARDMESSAGE, ProcessMsg.nParam3,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2);
        // 原文 37802
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 37805-37810  ServerSendAuctionBroadcastMsg
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendAuctionBroadcastMsg(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37805-37810`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。
    /// ⚠ 原文 ident 常量的拼写就是大小写混排的 `SM_AuctionBroadcastMsg`
    ///   （`Grobal2.pas`），照抄。</summary>
    public void ServerSendAuctionBroadcastMsg(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37805-37810
        // 原文 37807-37808
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_AuctionBroadcastMsg, ProcessMsg.nParam1,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam2, (int)ProcessMsg.nParam3);
        // 原文 37809
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 37812-37816  ServerSendPlayDrinkSay
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendPlayDrinkSay(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37812-37816`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendPlayDrinkSay(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37812-37816
        // 原文 37814
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_PLAYDRINKSAY, ProcessMsg.nParam3,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2);
        // 原文 37815
        SendSocketRef(m_DefMsg, DelphiRTL.AnsiString(EDcode.EncodeString(ProcessMsg.sMsg)));
    }

    // ==================================================================
    // 37818-37837  ServerSendWinExp
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendWinExp(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37818-37837`）。
    /// ★ **守卫方向与全族相反**：原文是 `if TObject(ProcessMsg.BaseObject) = Self then`
    ///   （**等号**），自走 `SM_WINEXP`、非自且有英雄时走 `SM_HEROWINEXP`。
    /// ★ `m_Abil.Exp` 是 `LongWord`，而 `MakeDefaultMsg` 的 `nRecog` 是 `Int64`
    ///   —— 无窄化。</summary>
    public void ServerSendWinExp(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37818-37837
        // 原文 37820：if TObject(ProcessMsg.BaseObject) = Self then
        if (PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37822-37825
            if (ProcessMsg.wParam == 0)
                m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                    Grobal2Const.SM_WINEXP, m_Abil.Exp,
                    LoWordInt(ProcessMsg.nParam1), HiWordInt(ProcessMsg.nParam1), 0);
            else
                m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                    Grobal2Const.SM_WINEXP, m_AbilNG.Exp,
                    LoWordInt(ProcessMsg.nParam1), HiWordInt(ProcessMsg.nParam1), 1);
            // 原文 37826
            SendSocketRef(m_DefMsg, "");
        }
        // 原文 37828：else if m_MyHero <> nil then
        else if (m_MyHero != null)
        {
            // 原文 37830-37834
            if (ProcessMsg.wParam == 0)
                m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                    Grobal2Const.SM_HEROWINEXP, m_MyHero!.m_wAbil.Exp,
                    LoWordInt(ProcessMsg.nParam1), HiWordInt(ProcessMsg.nParam1), 0);
            else
                // 原文 37833：THeroObject(m_MyHero).m_AbilNG.Exp —— 原文是**显式强转**；
                // 托管 m_MyHero 是 TCreature?，m_AbilNG 在 TPlayObject 上，故用模式匹配表达同一强转。
                m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                    Grobal2Const.SM_HEROWINEXP, HeroAsPlayObject().m_AbilNG.Exp,
                    LoWordInt(ProcessMsg.nParam1), HiWordInt(ProcessMsg.nParam1), 1);
            // 原文 37835
            SendSocketRef(m_DefMsg, "");
        }
    }

    // ==================================================================
    // 37839-37851  ServerSendUserName
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendUserName(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37839-37851`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendUserName(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37839-37851
        // 原文 37843-37844
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_USERNAME, ProcessMsg.BaseObject,
            PlayerSurfaceServerSendSeams.GetCharColor(ProcessMsg.BaseObject), 0, 0);
        // 原文 37845：BaseObject := TBaseObject(ProcessMsg.BaseObject);
        // 原文 37846-37848：if (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER])
        //                     and TSmartObject(BaseObject).m_boMysteriousMan and (m_btPermission >= 10) then
        //                     SendSocket(@m_DefMsg, TBaseObject(ProcessMsg.BaseObject).GetShowName(True))
        // 原文 37849-37850：else SendSocket(@m_DefMsg, ProcessMsg.sMsg);
        int race = PlayerSurfaceServerSendSeams.GetRaceServer(ProcessMsg.BaseObject);
        if ((race == Grobal2Const.RC_PLAYOBJECT
                || race == Grobal2Const.RC_HEROOBJECT
                || race == Grobal2Const.RC_PLAYMOSTER)
            && PlayerSurfaceServerSendSeams.GetMysteriousMan(ProcessMsg.BaseObject)
            && m_btPermission >= 10)
            SendSocketRef(m_DefMsg, PlayerSurfaceServerSendSeams.GetShowName(ProcessMsg.BaseObject, true));
        else
            SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 37853-37909  ServerSendLevelUp
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendLevelUp(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37853-37909`）。
    /// ★ 原文第一行 `if TBaseObject(ProcessMsg.BaseObject).m_btRaceServer = RC_PLAYOBJECT`
    ///   **不做 nil 检查**（`// 原文如此` + `// ★ 原文缺陷`，见方法内注）。</summary>
    public void ServerSendLevelUp(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37853-37909
        // 原文 37855：if TBaseObject(ProcessMsg.BaseObject).m_btRaceServer = RC_PLAYOBJECT then
        // ★ 原文缺陷（原文如此）：此处**未做 nil 判定**就解引用 `ProcessMsg.BaseObject`；
        //   指针为 nil 时原文会在此抛访问违例（由上层 Operate 的 try..except 兜住）。
        //   托管侧照抄"不判空"——不替原文补 nil 检查。
        if (PlayerSurfaceServerSendSeams.GetRaceServer(ProcessMsg.BaseObject) == Grobal2Const.RC_PLAYOBJECT)
        {
            // 原文 37857-37858
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_LEVELUP, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2, (int)ProcessMsg.wParam);
            // 原文 37859
            SendSocketRef(m_DefMsg, "");
            // 原文 37860：if TObject(ProcessMsg.BaseObject) = Self then
            if (PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
            {
                // 原文 37862-37866
                m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                    Grobal2Const.SM_ABILITY, (long)m_nGold,
                    PlayerSurfacePack.MakeWord(m_btJob, 99),
                    LoWordInt(m_nGameGold), HiWordInt(m_nGameGold));
                m_wAbil.Exp = m_Abil.Exp;
                m_wAbil.MaxExp = m_Abil.MaxExp;
                m_wAbil.Level = m_Abil.Level;
                SendSocketRef(m_DefMsg, DelphiRTL.AnsiString(
                    EDcode.zLibCompressBuffer(StructBytes.BytesOf(m_wAbil), StructBytes.SizeOf<TAbility>())));
                // 原文 37867-37875（注释掉的"扩展准确、敏捷为 65536 之前的旧版本"，原样保留）
                // 原文 37876-37878
                PlayerSurfaceMsgSeams.SendDefMessage(
                    this, (ushort)Grobal2Const.SM_SUBABILITY,
                    (long)PlayerSurfacePack.MakeLong(
                        PlayerSurfacePack.MakeWord(m_nAntiMagic, 0), m_btSpeedPoint),
                    (ushort)m_btHitPoint,
                    PlayerSurfacePack.MakeWord(m_btAntiPoison, m_nPoisonRecover),
                    PlayerSurfacePack.MakeWord(m_nHealthRecover, m_nSpellRecover),
                    IntToStr(m_nNPRecoverTime) + "/" + IntToStr(m_nNPRecoverPoint));
            }
        }
        // 原文 37881：else if TBaseObject(ProcessMsg.BaseObject).m_btRaceServer = RC_HEROOBJECT then
        else if (PlayerSurfaceServerSendSeams.GetRaceServer(ProcessMsg.BaseObject) == Grobal2Const.RC_HEROOBJECT)
        {
            // 原文 37883-37884
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_HEROLEVELUP, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2, (int)ProcessMsg.wParam);
            // 原文 37885
            SendSocketRef(m_DefMsg, "");
            // 原文 37886：if (m_MyHero <> nil) and (TObject(ProcessMsg.BaseObject) = m_MyHero) then
            //   ⚠ 原文把 `m_MyHero <> nil` 写在**前面**（Delphi 默认短路）；托管 `&&` 同样短路。
            if (m_MyHero != null
                && PlayerSurfaceServerSendSeams.SameObject(HeroHandleOrZero, ProcessMsg.BaseObject))
            {
                // 原文 37888-37889
                m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                    Grobal2Const.SM_HEROABILITY, ProcessMsg.BaseObject,
                    PlayerSurfacePack.MakeWord(m_MyHero!.m_btJob, m_MyHero!.m_btGender), 0, 0);
                m_MyHero!.m_wAbil.Exp = HeroAsPlayObject().m_Abil.Exp;
                m_MyHero!.m_wAbil.MaxExp = HeroAsPlayObject().m_Abil.MaxExp;
                m_MyHero!.m_wAbil.Level = HeroAsPlayObject().m_Abil.Level;
                SendSocketRef(m_DefMsg, DelphiRTL.AnsiString(
                    EDcode.zLibCompressBuffer(
                        StructBytes.BytesOf(m_MyHero!.m_wAbil), StructBytes.SizeOf<TAbility>())));
                // 原文 37894-37902（注释掉的旧版本，原样保留）
                // 原文 37903-37906
                PlayerSurfaceMsgSeams.SendDefMessage(
                    this, (ushort)Grobal2Const.SM_HEROSUBABILITY,
                    (long)PlayerSurfacePack.MakeLong(
                        PlayerSurfacePack.MakeWord(HeroAsPlayObject().m_nAntiMagic, 0),
                        m_MyHero!.m_btSpeedPoint),
                    (ushort)m_MyHero!.m_btHitPoint,
                    PlayerSurfacePack.MakeWord(m_MyHero!.m_btAntiPoison, m_MyHero!.m_nPoisonRecover),
                    PlayerSurfacePack.MakeWord(m_MyHero!.m_nHealthRecover, m_MyHero!.m_nSpellRecover),
                    IntToStr(HeroAsPlayObject().m_nNPRecoverTime) + "/"
                        + IntToStr(HeroAsPlayObject().m_nNPRecoverPoint));
            }
        }
    }

    // ==================================================================
    // 37911-37915  ServerSendChangeNameColor
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendChangeNameColor(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37911-37915`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendChangeNameColor(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37911-37915
        // 原文 37913-37914
        PlayerSurfaceMsgSeams.SendDefMessage(
            this, (ushort)Grobal2Const.SM_CHANGENAMECOLOR, (long)ProcessMsg.BaseObject,
            (ushort)PlayerSurfaceServerSendSeams.GetCharColor(ProcessMsg.BaseObject), 0, 0, "");
    }

    // ==================================================================
    // 37917-37928  ServerSendSpell
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendSpell(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37917-37928`）。</summary>
    public void ServerSendSpell(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37917-37928
        // 原文 37919
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37921-37922
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_SPELL, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2, (int)ProcessMsg.wParam);
            // 原文 37923-37925：{$IF DEBUG_SPELL_DEAY = 1} MainOutMessage(IntToStr(MyGetTickCount)); {$IFEND}
            //   —— 条件编译默认关闭，原样保留为注释。
            // 原文 37926
            SendSocketRef(m_DefMsg, IntToStr(ProcessMsg.nParam3) + "|" + ProcessMsg.sMsg);
        }
    }

    // ==================================================================
    // 37930-37938  ServerSendSpell2
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendSpell2(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37930-37938`）。
    /// ⚠ 原文如此：ident 用的是 **`SM_POWERHIT`**（不是 `SM_SPELL`），照抄。</summary>
    public void ServerSendSpell2(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37930-37938
        // 原文 37932
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 37934-37935
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_POWERHIT, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2, (int)ProcessMsg.wParam);
            // 原文 37936
            SendSocketRef(m_DefMsg, IntToStr(ProcessMsg.nParam3));
        }
    }

    // ==================================================================
    // 37940-37945  ServerSendSpell3
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendSpell3(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37940-37945`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendSpell3(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37940-37945
        // 原文 37942-37943
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_SPELL, ProcessMsg.BaseObject,
            (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2, (int)ProcessMsg.wParam);
        // 原文 37944
        SendSocketRef(m_DefMsg, IntToStr(ProcessMsg.nParam3) + "|" + ProcessMsg.sMsg);
    }

    // ==================================================================
    // 37947-37951  ServerSendMoveFail
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendMoveFail(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37947-37951`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。
    /// ⚠ 原文如此：`nRecog` 传的是 **`NativeInt(Self)`**（不是 `ProcessMsg.BaseObject`），
    ///   但同一句的尾数据读的却是 **`ProcessMsg.BaseObject` 的 `m_nCharStatus`** ——
    ///   同一句里两个不同对象，照抄，**不**"修正"为 Self。</summary>
    public void ServerSendMoveFail(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37947-37951
        // 原文 37949
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_MOVEFAIL, SelfHandle,
            m_nCurrX, m_nCurrY, m_btDirection);
        // 原文 37950
        SendSocketRef(m_DefMsg, DelphiRTL.AnsiString(
            EDcode.EncodeBuffer(
                StructBytes.BytesOf(PlayerSurfaceServerSendSeams.GetCharStatus(ProcessMsg.BaseObject)),
                sizeof(int))));
    }

    // ==================================================================
    // 37953-37983  ServerSendDeath
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendDeath(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37953-37983`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。
    /// ★ 两条分支的 `CharDesc.Feature` 来源不同：`nParam3 = 1` 时**恒为 0**
    ///   （原文把 `GetFeature` 那行注释掉了，见 :37962），否则才真正调用 `GetFeature`。</summary>
    public void ServerSendDeath(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37953-37983
        TCharDesc CharDesc = default;
        byte[] Feature = Array.Empty<byte>();

        // 原文 37959：if ProcessMsg.nParam3 = 1 then
        if (ProcessMsg.nParam3 == 1)
        {
            // 原文 37961：CharDesc.Feature := 0;
            CharDesc.Feature = 0;
            // 原文 37962（注释）：// CharDesc.Feature := TBaseObject(ProcessMsg.BaseObject).GetFeature(Self, @Feature);
            // 原文 37963
            CharDesc.Status = PlayerSurfaceServerSendSeams.GetCharStatus(ProcessMsg.BaseObject);
            // 原文 37964-37965
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_NOWDEATH, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                PlayerSurfacePack.MakeWord((int)ProcessMsg.wParam,
                    PlayerSurfaceServerSendSeams.GetLight(ProcessMsg.BaseObject)));
        }
        else
        {
            // 原文 37969：CharDesc.Feature := TBaseObject(ProcessMsg.BaseObject).GetFeature(Self, @Feature);
            (int featureLen, byte[] featureBytes) = PlayerSurfaceServerSendSeams.GetFeature(ProcessMsg.BaseObject, this);
            CharDesc.Feature = (byte)featureLen;
            Feature = featureBytes;
            // 原文 37970
            CharDesc.Status = PlayerSurfaceServerSendSeams.GetCharStatus(ProcessMsg.BaseObject);
            // 原文 37971-37972
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_DEATH, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                PlayerSurfacePack.MakeWord((int)ProcessMsg.wParam,
                    PlayerSurfaceServerSendSeams.GetLight(ProcessMsg.BaseObject)));
        }

        // 原文 37974：if CharDesc.Feature > 0 then
        if (CharDesc.Feature > 0)
        {
            // 原文 37976-37978
            byte[] sSendMsg = BuildCharDescWithFeature(CharDesc, Feature, CharDesc.Feature);
            // 原文 37979：SendSocketEx(@m_DefMsg, PAnsiChar(sSendMsg), Length(sSendMsg));
            SendSocketExRef(m_DefMsg, sSendMsg);
        }
        else
            // 原文 37982：SendSocketEx(@m_DefMsg, @CharDesc, SizeOf(TCharDesc));
            SendSocketExRef(m_DefMsg, StructBytes.BytesOf(CharDesc));
    }

    // ==================================================================
    // 37985-37999  ServerSendDisppear
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendDisppear(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:37985-37999`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendDisppear(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 37985-37999
        // 原文 37989
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_DISAPPEAR, ProcessMsg.BaseObject, 0, 0, 0);
        // 原文 37990-37991（注释）：
        //   神兽变形的时候用，锁定目标后，目标变形又没有锁定
        //   来源： ObjMon.pas SendRefMsg(RM_DISAPPEAR, 0, NativeInt(ElfMon), 0, 0, '');
        // 原文 37992：if ProcessMsg.nParam1 = 0 then
        if (ProcessMsg.nParam1 == 0)
            // 原文 37993
            SendSocketRef(m_DefMsg, "");
        else
        {
            // 原文 37996-37997
            long I64 = ProcessMsg.nParam1;
            SendSocketExRef(m_DefMsg, StructBytes.BytesOf(I64));
        }
    }

    // ==================================================================
    // 38001-38205  ServerSendLogon —— 留痕
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendLogon(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38001-38205`，**204 行**）。
    /// <para><b>未移植（显式留痕）</b>。缺失成员（原文行号）：
    /// <c>m_PEnvir.m_boDARK</c>/<c>m_boDAY</c>/<c>MapName</c>/<c>sMapName</c>
    /// （:38011/38026/38030-38036 —— 托管 <c>Envir.cs</c> 无这些成员）；
    /// <c>SendLogon()</c>（:38039，客户端登录总入口，未移植）；
    /// <c>ClientQueryUserName(ProcessMsg, boResult)</c>（:38043，原文 :20189-20210，未移植）；
    /// <c>SendServerConfig</c>（:38046）/ <c>SendEnableClientUploadPickItems</c>（:38047）/
    /// <c>RefUserState</c>（:38049）/ <c>SendMapDescription</c>（:38050）/
    /// <c>SendMapCanRun</c>（:38051，本仓已留痕）/ <c>SendGoldInfo</c>（:38052）/
    /// <c>SendGameGlory</c>（:38053）/ <c>SendNewGamePointInfo</c>（:38056）/
    /// <c>SendAcupointLevels</c>（:38057）/ <c>SendJewelryBox</c>（:38060）/
    /// <c>SendUpdateGodBless</c>（:38061）/ <c>SendUpdateFengHao</c>（:38062）/
    /// <c>SendGamePetList</c>（:38063）/ <c>SendGamePetBagItemList</c>（:38064）/
    /// <c>SendStorageOpenStatus</c>（:38065）；
    /// <c>m_boDummyObject</c>（:38044/38058）、<c>m_ActiveFengHao</c>/<c>m_FengHaoItems</c>
    /// （:38066-38068）、<c>m_nScriptGotoCount</c>/<c>g_FunctionNPC</c>（:38072-38073）；
    /// <c>m_boSaveKillMonExpRate</c>/<c>m_dwKillMonExpRateTime</c>/<c>m_nKillMonExpRate</c>
    /// （:38079-38098）、<c>m_boAttackHumSavePowerRate</c>/<c>m_nAttackHumPowerRate</c>/
    /// <c>m_dwAttackHumPowerRateTime</c>/<c>m_boAttackMonSavePowerRate</c>/
    /// <c>m_nAttackMonPowerRate</c>/<c>m_dwAttackMonPowerRateTime</c>（:38101-38168）、
    /// <c>m_boSaveKillMonBurstRate</c>/<c>m_dwKillMonBurstRate</c>/
    /// <c>m_dwKillMonBurstRateTime</c>（:38170-38183）、
    /// <c>m_dwHighLevelKillMonFixExpTime</c>（:38185-38188）；
    /// <c>g_sKillMonExpRateForeverMsg</c> 等 6 个全局提示串 + <c>FloatToStr</c>/
    /// <c>StringReplace</c>（:38084-38188）、<c>UserEngine.GetLogonMessage</c>（:38193）、
    /// <c>m_IsSendUserLogon</c>（:38204）。
    /// 按任务书第 4 条，**不臆造替身**。</para>
    /// </summary>
    public void ServerSendLogon(TProcessMessage ProcessMsg, ref bool boResult)
    {
        PortNotPorted(nameof(ServerSendLogon), 38001);
    }

    // ==================================================================
    // 38207-38214  ServerSendAbility
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendAbility(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38207-38214`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendAbility(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38207-38214
        // 原文 38209
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_ABILITY, (long)m_nGold,
            PlayerSurfacePack.MakeWord(m_btJob, 99),
            LoWordInt(m_nGameGold), HiWordInt(m_nGameGold));
        // 原文 38210-38212
        m_wAbil.Exp = m_Abil.Exp;
        m_wAbil.MaxExp = m_Abil.MaxExp;
        m_wAbil.Level = m_Abil.Level;
        // 原文 38213：SendSocket(@m_DefMsg, zLibCompressBuffer(@m_WAbil, SizeOf(TAbility)));
        SendSocketRef(m_DefMsg, DelphiRTL.AnsiString(
            EDcode.zLibCompressBuffer(StructBytes.BytesOf(m_wAbil), StructBytes.SizeOf<TAbility>())));
    }

    // ==================================================================
    // 38216-38270  ServerSendHealthSpellChanged
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendHealthSpellChanged(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38216-38270`）。</summary>
    public void ServerSendHealthSpellChanged(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38216-38270
        TNewMessageBodyWL NewMessageBodyWL = default;
        TMessageHealthSpellChangedInfo HealthSpellChangedInfo = default;

        // 原文 38223-38225：BaseObject := TBaseObject(ProcessMsg.BaseObject); if BaseObject = nil then Exit;
        if (!PlayerSurfaceServerSendSeams.ObjectPresent(ProcessMsg.BaseObject))
            return;

        // 原文 38227-38228
        int mp = PlayerSurfaceServerSendSeams.GetMP(ProcessMsg.BaseObject);
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_HEALTHSPELLCHANGED, ProcessMsg.BaseObject,
            LoWordInt(mp), HiWordInt(mp),
            PlayerSurfacePack.MakeWord(PlayerSurfaceServerSendSeams.GetJob(ProcessMsg.BaseObject), 0));

        // 原文 38230：if ProcessMsg.wParam > 0 then
        if (ProcessMsg.wParam > 0)
        {
            // 原文 38232-38233
            HealthSpellChangedInfo.ChangeHP = (uint)PlayerSurfaceServerSendSeams.GetHP(ProcessMsg.BaseObject);
            HealthSpellChangedInfo.IsAttackFromHum = 0;
            // 原文 38234：if ProcessMsg.nParam3 <> 0 then
            if (ProcessMsg.nParam3 != 0)
            {
                // 原文 38236-38239
                nint attackFrom = AsNint(ProcessMsg.nParam3);
                if (PlayerSurfaceServerSendSeams.GetAttackerRaceServer(attackFrom) == Grobal2Const.RC_PLAYOBJECT
                    || (PlayerSurfaceServerSendSeams.AttackerHasMaster(attackFrom)
                        && PlayerSurfaceServerSendSeams.GetAttackerMasterRaceServer(attackFrom)
                            == Grobal2Const.RC_PLAYOBJECT))
                    HealthSpellChangedInfo.IsAttackFromHum = 1;
            }
            // 原文 38241
            SendSocketExRef(m_DefMsg, StructBytes.BytesOf(HealthSpellChangedInfo));
        }
        else
        {
            // 原文 38245-38247
            NewMessageBodyWL.lParam1 = PlayerSurfaceServerSendSeams.GetHP(ProcessMsg.BaseObject);
            NewMessageBodyWL.lParam2 = PlayerSurfaceServerSendSeams.GetMaxHP(ProcessMsg.BaseObject);
            NewMessageBodyWL.lTag1 = 0;

            // 原文 38249-38261
            if (PlayerSurfaceServerSendSeams.HasEnvir(ProcessMsg.BaseObject)
                && (PlayerSurfaceServerSendSeams.GetSecretFlag(ProcessMsg.BaseObject)
                    & PlayerSurfaceServerSend2Const.SecretFlag_ShowEqualName) != 0
                && !PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject)
                && !PlayerSurfaceServerSendSeams.SameObject(HeroHandleOrZero, ProcessMsg.BaseObject))
                NewMessageBodyWL.lTag2 = 0;
            else if (PlayerSurfaceServerSendSeams.HasEnvir(ProcessMsg.BaseObject)
                && (PlayerSurfaceServerSendSeams.GetSecretFlag2(ProcessMsg.BaseObject)
                    & PlayerSurfaceServerSend2Const.SecretFlag_ShowEqualName) != 0
                && !PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject)
                && !PlayerSurfaceServerSendSeams.SameObject(HeroHandleOrZero, ProcessMsg.BaseObject))
                NewMessageBodyWL.lTag2 = 0;
            else
                NewMessageBodyWL.lTag2 = PlayerSurfaceServerSendSeams.GetLevel(ProcessMsg.BaseObject);

            // 原文 38263-38266
            NewMessageBodyWL.lTag3 = PlayerSurfaceServerSendSeams.GetMaxMP(ProcessMsg.BaseObject);
            NewMessageBodyWL.lTag4 = 0;
            // Cursor 2023-09-07 14:02:29 清空 NewMessageBodyWL.lTag5 升级时没有攻击人
            NewMessageBodyWL.lTag5 = 0;
            NewMessageBodyWL.BlastHitType = GXX.Core.Protocol.TBlastHitType.bhtNone;

            // 原文 38268
            SendSocketExRef(m_DefMsg, StructBytes.BytesOf(NewMessageBodyWL));
        }
    }

    // ==================================================================
    // 38272-38308  ServerSendHPMPChangedFormStone
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendHPMPChangedFormStone(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38272-38308`）。
    /// ⚠ 原文如此：只有 `wParam > 0` 分支会发送；`wParam &lt;= 0` 时**什么都不发**
    ///   （`m_DefMsg` 已装配好却被丢弃）。</summary>
    public void ServerSendHPMPChangedFormStone(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38272-38308
        TNewMessageBodyWL NewMessageBodyWL = default;

        // 原文 38277-38279：BaseObject := TBaseObject(ProcessMsg.BaseObject); if BaseObject = nil then Exit;
        if (!PlayerSurfaceServerSendSeams.ObjectPresent(ProcessMsg.BaseObject))
            return;

        // 原文 38281-38282
        int mp = PlayerSurfaceServerSendSeams.GetMP(ProcessMsg.BaseObject);
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_HPMPCHANGED_FORM_STONE, ProcessMsg.BaseObject,
            LoWordInt(mp), HiWordInt(mp),
            PlayerSurfacePack.MakeWord(PlayerSurfaceServerSendSeams.GetJob(ProcessMsg.BaseObject), 0));

        // 原文 38284：if ProcessMsg.wParam > 0 then
        if (ProcessMsg.wParam > 0)
        {
            // 原文 38286-38287
            NewMessageBodyWL.lParam1 = PlayerSurfaceServerSendSeams.GetHP(ProcessMsg.BaseObject);
            NewMessageBodyWL.lParam2 = PlayerSurfaceServerSendSeams.GetMaxHP(ProcessMsg.BaseObject);
            // 原文 38288：lTag1 := MakeWord(Integer(g_Config.boHPStoneHideHealthNum), Integer(g_Config.boMPStoneHideHealthNum));
            NewMessageBodyWL.lTag1 = PlayerSurfacePack.MakeWord(
                BoolToInt(M2Config.boHPStoneHideHealthNum),
                BoolToInt(M2Config.boMPStoneHideHealthNum));

            // 原文 38290-38300
            // ⚠ 原文如此：本分支**不做 `m_PEnvir <> nil` 判定**（与 :38250 不同），
            //   直接解引用 `BaseObject.m_PEnvir.m_nSecretFlag`。托管侧接缝退化为"读 0"。
            if ((PlayerSurfaceServerSendSeams.GetSecretFlag(ProcessMsg.BaseObject)
                    & PlayerSurfaceServerSend2Const.SecretFlag_ShowEqualName) != 0
                && !PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject)
                && !PlayerSurfaceServerSendSeams.SameObject(HeroHandleOrZero, ProcessMsg.BaseObject))
                NewMessageBodyWL.lTag2 = 0;
            else if ((PlayerSurfaceServerSendSeams.GetSecretFlag2(ProcessMsg.BaseObject)
                    & PlayerSurfaceServerSend2Const.SecretFlag_ShowEqualName) != 0
                && !PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject)
                && !PlayerSurfaceServerSendSeams.SameObject(HeroHandleOrZero, ProcessMsg.BaseObject))
                NewMessageBodyWL.lTag2 = 0;
            else
                NewMessageBodyWL.lTag2 = PlayerSurfaceServerSendSeams.GetLevel(ProcessMsg.BaseObject);

            // 原文 38302-38305
            NewMessageBodyWL.lTag3 = PlayerSurfaceServerSendSeams.GetMaxMP(ProcessMsg.BaseObject);
            NewMessageBodyWL.lTag4 = 0;
            NewMessageBodyWL.lTag5 = 0;
            NewMessageBodyWL.BlastHitType = GXX.Core.Protocol.TBlastHitType.bhtNone;
            // 原文 38306
            SendSocketExRef(m_DefMsg, StructBytes.BytesOf(NewMessageBodyWL));
        }
    }

    // ==================================================================
    // 38310-38335  ServerSendDayChangeing
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendDayChangeing(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38310-38335`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendDayChangeing(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38310-38335
        int nObjCount;

        // 原文 38314：if m_PEnvir.m_boDARK then
        if (PlayerSurfaceServerSendSeams.GetMapDayFlag(this, false))
            // 原文 38315
            nObjCount = 1;
        else
        {
            // 原文 38318-38324
            switch (m_nBright)
            {
                case 1:
                    nObjCount = 0;
                    break;
                case 0:
                case 2:
                    nObjCount = 2;
                    break;
                case 3:
                    nObjCount = 1;
                    break;
                default:
                    nObjCount = 1;
                    break;
            }
        }

        // 原文 38330-38331：if m_PEnvir.m_boDAY then nObjCount := 0;
        if (PlayerSurfaceServerSendSeams.GetMapDayFlag(this, true))
            nObjCount = 0;

        // 原文 38333：m_DefMsg := MakeDefaultMsg(SM_DAYCHANGING, 0, m_nBright, nObjCount, 0);
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_DAYCHANGING, 0, m_nBright, nObjCount, 0);
        // 原文 38334
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 38337-38340  ServerSendItemShow
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendItemShow(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38337-38340`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendItemShow(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38337-38340
        // 原文 38339
        PlayerSurfaceMsgSeams.SendDefMessage(
            this, (ushort)Grobal2Const.SM_ITEMSHOW, ProcessMsg.nParam1,
            (ushort)ProcessMsg.nParam2, (ushort)ProcessMsg.nParam3, (ushort)ProcessMsg.wParam,
            ProcessMsg.sMsg);
    }

    // ==================================================================
    // 38342-38345  ServerSendItemHide
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendItemHide(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38342-38345`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。
    /// ⚠ 原文如此：`nTag` 传 **0**（不是 `ProcessMsg.wParam`）。</summary>
    public void ServerSendItemHide(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38342-38345
        // 原文 38344
        PlayerSurfaceMsgSeams.SendDefMessage(
            this, (ushort)Grobal2Const.SM_ITEMHIDE, ProcessMsg.nParam1,
            (ushort)ProcessMsg.nParam2, (ushort)ProcessMsg.nParam3, 0,
            "");
    }

    // ==================================================================
    // 38347-38352  ServerSendDoorOpen
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendDoorOpen(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38347-38352`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendDoorOpen(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38347-38352
        // 原文 38349-38351
        PlayerSurfaceMsgSeams.SendDefMessage(
            this, (ushort)Grobal2Const.SM_OPENDOOR_OK, 0,
            (ushort)ProcessMsg.nParam1 /* x */, (ushort)ProcessMsg.nParam2 /* y */, 0,
            "");
    }

    // ==================================================================
    // 38354-38357  ServerSendDoorClose
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendDoorClose(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38354-38357`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendDoorClose(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38354-38357
        // 原文 38356
        PlayerSurfaceMsgSeams.SendDefMessage(
            this, (ushort)Grobal2Const.SM_CLOSEDOOR, 0,
            (ushort)ProcessMsg.nParam1, (ushort)ProcessMsg.nParam2, 0,
            "");
    }

    // ==================================================================
    // 38359-38362  ServerSendUseItems
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendUseItems(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38359-38362`）—— 原文体只有一句 `SendUseItems();`。
    /// ★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendUseItems(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38359-38362
        // 原文 38361：SendUseItems();
        SendUseItems();
    }

    // ==================================================================
    // 38364-38367  ServerSendWeightChanged
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendWeightChanged(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38364-38367`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendWeightChanged(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38364-38367
        // 原文 38366
        PlayerSurfaceMsgSeams.SendDefMessage(
            this, (ushort)Grobal2Const.SM_WEIGHTCHANGED, m_wAbil.Weight,
            (ushort)m_wAbil.WearWeight, (ushort)m_wAbil.HandWeight, 0,
            "");
    }

    // ==================================================================
    // 38369-38374  ServerSendFeatureChanged
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendFeatureChanged(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38369-38374`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendFeatureChanged(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38369-38374
        // 原文 38371-38372
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_FEATURECHANGED_NEW, ProcessMsg.BaseObject,
            (int)ProcessMsg.wParam,
            LoWordInt(ProcessMsg.nParam1), HiWordInt(ProcessMsg.nParam1));
        // 原文 38373
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 38376-38381  ServerSendNationMessage
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendNationMessage(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38376-38381`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendNationMessage(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38376-38381
        // 原文 38378-38379
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_NATIONMESSAGE, ProcessMsg.BaseObject,
            PlayerSurfacePack.MakeWord((int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2), 0, 1);
        // 原文 38380
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 38383-38387  ServerSendSetClientBuff
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendSetClientBuff(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38383-38387`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendSetClientBuff(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38383-38387
        // 原文 38385
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_SETCLIENTBUFF, ProcessMsg.nParam2,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam3);
        // 原文 38386
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 38389-38393  ServerSendCloseClientBuff
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendCloseClientBuff(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38389-38393`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendCloseClientBuff(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38389-38393
        // 原文 38391
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_CLOSECLIENTBUFF, 0, 0, (int)ProcessMsg.nParam1, 0);
        // 原文 38392
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 38395-38399  ServerSendShowClientBuff
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendShowClientBuff(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38395-38399`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendShowClientBuff(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38395-38399
        // 原文 38397
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_SHOWCLIENTBUFF, 0, (int)ProcessMsg.wParam, (int)ProcessMsg.nParam1, 0);
        // 原文 38398
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 38401-38405  ServerSendSetArrBuff
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendSetArrBuff(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38401-38405`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendSetArrBuff(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38401-38405
        // 原文 38403
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_SETARRBUFF, ProcessMsg.nParam2,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam3);
        // 原文 38404
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 38407-38411  ServerSendCloseArrBuff
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendCloseArrBuff(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38407-38411`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendCloseArrBuff(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38407-38411
        // 原文 38409
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_CLOSEARRBUFF, 0, 0, (int)ProcessMsg.nParam1, 0);
        // 原文 38410
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 38413-38417  ServerSendShowArrBuff
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendShowArrBuff(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38413-38417`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendShowArrBuff(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38413-38417
        // 原文 38415
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_SHOWARRBUFF, 0, (int)ProcessMsg.wParam, (int)ProcessMsg.nParam1, 0);
        // 原文 38416
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 38420-38424  ServerSendOpenBooks
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendOpenBooks(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38420-38424`；原文 38419 注释「卧龙 piaoyun 2013-08-20」）。
    /// ★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendOpenBooks(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38420-38424
        // 原文 38422
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_OPENBOOKS, ProcessMsg.BaseObject, (int)ProcessMsg.nParam1, 0, 0);
        // 原文 38423
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 38427-38431  ServerSendSceneShake
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendSceneShake(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38427-38431`；原文 38426 注释「屏幕震动 piaoyun 2013-09-14」）。
    /// ★ **无 `&lt;&gt; Self` 守卫**（原文如此）。⚠ 原文 `nRecog` 取 `ProcessMsg.wParam`。</summary>
    public void ServerSendSceneShake(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38427-38431
        // 原文 38429
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_SCENESHAKE, ProcessMsg.wParam, (int)ProcessMsg.nParam1, 0, 0);
        // 原文 38430
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 38433-38438  ServerSendEffectStep
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendEffectStep(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38433-38438`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendEffectStep(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38433-38438
        // 原文 38435-38436
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_EFFECTSTEP, ProcessMsg.BaseObject,
            (int)ProcessMsg.wParam, (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2);
        // 原文 38437
        SendSocketRef(m_DefMsg, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 38440-38449  ServerSendAddButton
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendAddButton(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38440-38449`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendAddButton(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38440-38449
        TShortMessage ShortMessage = default;

        // 原文 38444-38445
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_ADDBUTTON, ProcessMsg.wParam,
            LoWordInt(ProcessMsg.nParam1), HiWordInt(ProcessMsg.nParam1), (int)ProcessMsg.nParam3);
        // 原文 38446-38447
        ShortMessage.Ident = LoWordInt(ProcessMsg.nParam2);
        ShortMessage.wMsg = HiWordInt(ProcessMsg.nParam2);
        // 原文 38448：SendSocket(@m_DefMsg, EncodeBuffer(@ShortMessage, SizeOf(TShortMessage)) + EncodeString(ProcessMsg.sMsg));
        SendSocketRef(m_DefMsg, DelphiRTL.AnsiString(ConcatBytes(
            EDcode.EncodeBuffer(StructBytes.BytesOf(ShortMessage), StructBytes.SizeOf<TShortMessage>()),
            EDcode.EncodeString(ProcessMsg.sMsg))));
    }

    // ==================================================================
    // 38451-38455  ServerSendDelButton
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendDelButton(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38451-38455`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendDelButton(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38451-38455
        // 原文 38453
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_DELBUTTON, ProcessMsg.BaseObject, (int)ProcessMsg.nParam1, 0, 0);
        // 原文 38454
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 38457-38466  ServerSendAddArrButton
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendAddArrButton(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38457-38466`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendAddArrButton(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38457-38466
        TShortMessage ShortMessage = default;

        // 原文 38461-38462
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_ADDARRBUTTON, ProcessMsg.wParam,
            LoWordInt(ProcessMsg.nParam1), HiWordInt(ProcessMsg.nParam1), (int)ProcessMsg.nParam3);
        // 原文 38463-38464
        ShortMessage.Ident = LoWordInt(ProcessMsg.nParam2);
        ShortMessage.wMsg = HiWordInt(ProcessMsg.nParam2);
        // 原文 38465
        SendSocketRef(m_DefMsg, DelphiRTL.AnsiString(ConcatBytes(
            EDcode.EncodeBuffer(StructBytes.BytesOf(ShortMessage), StructBytes.SizeOf<TShortMessage>()),
            EDcode.EncodeString(ProcessMsg.sMsg))));
    }

    // ==================================================================
    // 38468-38472  ServerSendDelArrButton
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendDelArrButton(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38468-38472`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendDelArrButton(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38468-38472
        // 原文 38470
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_DELARRBUTTON, ProcessMsg.BaseObject, (int)ProcessMsg.nParam1, 0, 0);
        // 原文 38471
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 38474-38478  ServerSendAddNumberButton
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendAddNumberButton(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38474-38478`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendAddNumberButton(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38474-38478
        // 原文 38476
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_ADDNUMBERBUTTON, ProcessMsg.wParam,
            (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2, (int)ProcessMsg.nParam3);
        // 原文 38477
        SendSocketRef(m_DefMsg, DelphiRTL.AnsiString(EDcode.EncodeString(ProcessMsg.sMsg)));
    }

    // ==================================================================
    // 38480-38484  ServerSendDelNumberButton
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendDelNumberButton(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38480-38484`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendDelNumberButton(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38480-38484
        // 原文 38482
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_DELNUMBERBUTTON, ProcessMsg.BaseObject, (int)ProcessMsg.nParam1, 0, 0);
        // 原文 38483
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 38486-38490  ServerSendShowPhantom
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendShowPhantom(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38486-38490`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendShowPhantom(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38486-38490
        // 原文 38488
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_SHOWPHANTOM, ProcessMsg.BaseObject, (int)ProcessMsg.wParam, 0, 0);
        // 原文 38489
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 38492-38496  ServerSendClosePhantom
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendClosePhantom(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38492-38496`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendClosePhantom(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38492-38496
        // 原文 38494
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_CLOSEPHANTOM, ProcessMsg.BaseObject, 0, 0, 0);
        // 原文 38495
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 38498-38501  ServerSendClearObjects
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendClearObjects(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38498-38501`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendClearObjects(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38498-38501
        // 原文 38500
        PlayerSurfaceMsgSeams.SendDefMessage(
            this, (ushort)Grobal2Const.SM_CLEAROBJECTS, 0, 0, 0, 0, "");
    }

    // ==================================================================
    // 38503-38589  ServerSendChangeMap —— 留痕
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendChangeMap(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38503-38589`，**87 行**）。
    /// <para><b>未移植（显式留痕）</b>。缺失成员（原文行号）：
    /// <c>DoClientAction(catChangeMap)</c>（:38515）；
    /// <c>m_PEnvir.m_boDARK</c>/<c>m_boDAY</c>/<c>sMapDesc</c>/<c>m_WeatherEffect[]</c>
    /// （:38516/38531/38550/38563-38574）；
    /// <c>g_Config.ClientConfigs[73]</c>（:38537）；<c>m_GroupOwner.m_GroupMembers</c>
    /// （:38544-38546）；<c>RefUserState</c>/<c>SendMapDescription</c>/<c>SendMapCanRun</c>
    /// （:38534-38536）；<c>m_PEnvir.m_boNoRecallHero</c> 与
    /// <c>THeroObject(m_MyHero).LogOut</c>（:38580-38583）；
    /// <c>m_PEnvir.m_boNoCallPet</c> 与 <c>DoGamePetRetake</c>（:38585-38587）；
    /// <c>PTServerWeateherEffect</c>/<c>TServerWeateherEffect</c>/
    /// <c>MAX_MAP_WEATEHER_EFFECT</c> 与 <c>InBuf</c> 的手工指针拼装
    /// （:38508-38512/38561-38576）。</para>
    /// <para>★ <b>`原文如此`（本片唯一一条）</b>：原文 :38537 用
    /// <c>g_Config.ClientConfigs[73]</c> —— **硬编码下标 73**，无符号常量（魔法数字）。
    /// 属原文缺陷，**不修**。本方法整体留痕，故该缺陷随留痕登记、
    /// 不计入已移植方法的缺陷数。</para>
    /// </summary>
    public void ServerSendChangeMap(TProcessMessage ProcessMsg, ref bool boResult)
    {
        PortNotPorted(nameof(ServerSendChangeMap), 38503);
    }

    // ==================================================================
    // 38591-38599  ServerSendButch
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendButch(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38591-38599`）。</summary>
    public void ServerSendButch(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38591-38599
        // 原文 38593
        if (!PlayerSurfaceServerSendSeams.SameObject(SelfHandle, ProcessMsg.BaseObject))
        {
            // 原文 38595-38596
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_BUTCH, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2, (int)ProcessMsg.wParam);
            // 原文 38597
            SendSocketRef(m_DefMsg, "");
        }
    }

    // ==================================================================
    // 38601-38615  ServerSendMagicFire
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendMagicFire(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38601-38615`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendMagicFire(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38601-38615
        TCharDesc CharDesc = default;

        // 原文 38605-38606
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_MAGICFIRE, ProcessMsg.BaseObject,
            LoWordInt(ProcessMsg.nParam2), HiWordInt(ProcessMsg.nParam2), (int)ProcessMsg.nParam1);
        // 原文 38607：CharDesc.Feature := ProcessMsg.wParam; // NewLevel
        CharDesc.Feature = (byte)ProcessMsg.wParam;
        // 原文 38608：CharDesc.Status := ProcessMsg.nParam3; // TargeTBaseObject
        CharDesc.Status = ProcessMsg.nParam3;
        // 原文 38609-38613：4级技能强化 -- 4级灵魂火符 4级灭天火 chongchong 2013-12-04
        if (ProcessMsg.sMsg.Length > 0)
            CharDesc.MagicLevel = DelphiRTL.StrToIntDef(ProcessMsg.sMsg, 0);
        else
            CharDesc.MagicLevel = 0;
        // 原文 38614
        SendSocketExRef(m_DefMsg, StructBytes.BytesOf(CharDesc));
    }

    // ==================================================================
    // 38617-38626  ServerSendMagicFireEx
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendMagicFireEx(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38617-38626`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendMagicFireEx(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38617-38626
        // 原文 38621-38622
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_MAGICFIRE_EX, ProcessMsg.BaseObject,
            LoWordInt(ProcessMsg.nParam2), HiWordInt(ProcessMsg.nParam2), (int)ProcessMsg.nParam1);
        // 原文 38623-38624
        string sMsg = IntToStr(ProcessMsg.wParam /* NewLevel */) + "|"
            + IntToStr(ProcessMsg.nParam3 /* TargeTBaseObject */) + "|" + ProcessMsg.sMsg;
        // 原文 38625
        SendSocketRef(m_DefMsg, sMsg);
    }

    // ==================================================================
    // 38628-38637  ServerSendMagicFireEx2
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendMagicFireEx2(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38628-38637`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendMagicFireEx2(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38628-38637
        // 原文 38632-38633
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_MAGICFIRE_EX_2, ProcessMsg.BaseObject,
            LoWordInt(ProcessMsg.nParam2), HiWordInt(ProcessMsg.nParam2), (int)ProcessMsg.nParam1);
        // 原文 38634-38635
        string sMsg = IntToStr(ProcessMsg.wParam /* NewLevel */) + "|"
            + IntToStr(ProcessMsg.nParam3 /* TargeTBaseObject */) + "|" + ProcessMsg.sMsg;
        // 原文 38636
        SendSocketRef(m_DefMsg, sMsg);
    }

    // ==================================================================
    // 38639-38642  ServerSendMyMagic
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendMyMagic(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38639-38642`）—— 原文体只有一句 `SendUseMagic;`。
    /// ★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendMyMagic(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38639-38642
        // 原文 38641：SendUseMagic;
        SendUseMagic();
    }

    // ==================================================================
    // 38644-38648  ServerSendMagicLVEXP
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendMagicLVEXP(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38644-38648`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendMagicLVEXP(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38644-38648
        // 原文 38646-38647
        PlayerSurfaceMsgSeams.SendDefMessage(
            this, (ushort)Grobal2Const.SM_MAGIC_LVEXP, ProcessMsg.nParam1,
            (ushort)ProcessMsg.nParam2,
            LoWordInt(ProcessMsg.nParam3), HiWordInt(ProcessMsg.nParam3),
            ProcessMsg.sMsg);
    }

    // ==================================================================
    // 38650-38664  ServerSendSkeleton
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendSkeleton(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38650-38664`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。
    /// ⚠ 原文如此：**无条件** `SetLength(sSendMsg, SizeOf(TCharDesc) + CharDesc.Feature)`
    ///   并发送 `Length(sSendMsg)` 字节（与 <see cref="ServerSendDeath"/> 的
    ///   "Feature &gt; 0 才拼"不同）。</summary>
    public void ServerSendSkeleton(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38650-38664
        TCharDesc CharDesc = default;

        // 原文 38656-38657
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_SKELETON, ProcessMsg.BaseObject,
            (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2, (int)ProcessMsg.wParam);
        // 原文 38658：CharDesc.Feature := TBaseObject(ProcessMsg.BaseObject).GetFeature(Self, @Feature);
        (int featureLen, byte[] featureBytes) = PlayerSurfaceServerSendSeams.GetFeature(ProcessMsg.BaseObject, this);
        CharDesc.Feature = (byte)featureLen;
        // 原文 38659
        CharDesc.Status = PlayerSurfaceServerSendSeams.GetCharStatus(ProcessMsg.BaseObject);
        // 原文 38660-38662
        byte[] sSendMsg = BuildCharDescWithFeature(CharDesc, featureBytes, featureLen);
        // 原文 38663
        SendSocketExRef(m_DefMsg, sSendMsg);
    }

    // ==================================================================
    // 38666-38670  ServerSendDuraChange
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendDuraChange(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38666-38670`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendDuraChange(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38666-38670
        // 原文 38668
        PlayerSurfaceMsgSeams.SendDefMessage(
            this, (ushort)Grobal2Const.SM_DURACHANGE, ProcessMsg.nParam1,
            (ushort)ProcessMsg.wParam,
            LoWordInt(ProcessMsg.nParam2), HiWordInt(ProcessMsg.nParam2),
            ProcessMsg.sMsg);
    }

    // ==================================================================
    // 38672-38675  ServerSendGoldChanged
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendGoldChanged(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38672-38675`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendGoldChanged(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38672-38675
        // 原文 38674：SendDefMessage(SM_GOLDCHANGED, m_nGold, LoWord(m_nGameGold), HiWord(m_nGameGold), 0, '');
        PlayerSurfaceMsgSeams.SendDefMessage(
            this, (ushort)Grobal2Const.SM_GOLDCHANGED, (long)m_nGold,
            LoWordInt(m_nGameGold), HiWordInt(m_nGameGold), 0,
            "");
    }

    // ==================================================================
    // 38677-38680  ServerSendChangeLight
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendChangeLight(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38677-38680`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendChangeLight(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38677-38680
        // 原文 38679
        PlayerSurfaceMsgSeams.SendDefMessage(
            this, (ushort)Grobal2Const.SM_CHANGELIGHT, (long)ProcessMsg.BaseObject,
            (ushort)PlayerSurfaceServerSendSeams.GetLight(ProcessMsg.BaseObject), 0, 0,
            "");
    }

    // ==================================================================
    // 38682-38688  ServerSendCharStatusChanged
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendCharStatusChanged(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38682-38688`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendCharStatusChanged(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38682-38688
        // 原文 38684-38687
        PlayerSurfaceMsgSeams.SendDefMessage(
            this, (ushort)Grobal2Const.SM_CHARSTATUSCHANGED, (long)ProcessMsg.BaseObject,
            LoWordInt(ProcessMsg.nParam1), // m_nCharStatus
            HiWordInt(ProcessMsg.nParam1), // m_nCharStatus
            (ushort)ProcessMsg.wParam,     // m_nHitSpeed
            "");
    }

    // ==================================================================
    // 38690-38706  ServerSendDigUp
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendDigUp(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38690-38706`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。
    /// ⚠ 原文如此：本方法用 **`TMessageBodyWL`**（含 `lTag2: Int64`），且把
    ///   `GetFeature` 的返回值写进 **`lParam1`**（不是 `CharDesc.Feature`）。</summary>
    public void ServerSendDigUp(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38690-38706
        TMessageBodyWL MessageBodyWL = default;

        // 原文 38696-38697
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_DIGUP, ProcessMsg.BaseObject,
            (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
            PlayerSurfacePack.MakeWord((int)ProcessMsg.wParam,
                PlayerSurfaceServerSendSeams.GetLight(ProcessMsg.BaseObject)));
        // 原文 38698-38701
        (int featureLen, byte[] featureBytes) = PlayerSurfaceServerSendSeams.GetFeature(ProcessMsg.BaseObject, this);
        MessageBodyWL.lParam1 = featureLen;
        MessageBodyWL.lParam2 = PlayerSurfaceServerSendSeams.GetCharStatus(ProcessMsg.BaseObject);
        MessageBodyWL.lTag1 = (int)ProcessMsg.nParam3;
        MessageBodyWL.lTag2 = 0;
        // 原文 38702-38704：SetLength(S, SizeOf(TMessageBodyWL) + MessageBodyWL.lParam1); Move(...); Move(...);
        int bodySize = StructBytes.SizeOf<TMessageBodyWL>();
        byte[] S = new byte[bodySize + MessageBodyWL.lParam1];
        StructBytes.ToBytes(MessageBodyWL, S, 0);
        int copy = Math.Min(MessageBodyWL.lParam1, Math.Max(0, featureBytes?.Length ?? 0));
        if (copy > 0)
            Array.Copy(featureBytes!, 0, S, bodySize, copy);
        // 原文 38705
        SendSocketRef(m_DefMsg, DelphiRTL.AnsiString(S));
    }

    // ==================================================================
    // 38708-38713  ServerSendDigDown
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendDigDown(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38708-38713`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendDigDown(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38708-38713
        // 原文 38710-38711
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
            Grobal2Const.SM_DIGDOWN, ProcessMsg.BaseObject,
            (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
            PlayerSurfacePack.MakeWord((int)ProcessMsg.wParam,
                PlayerSurfaceServerSendSeams.GetLight(ProcessMsg.BaseObject)));
        // 原文 38712
        SendSocketRef(m_DefMsg, "");
    }

    // ==================================================================
    // 38715-38729  ServerSendFlyAxe
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendFlyAxe(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38715-38729`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendFlyAxe(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38715-38729
        TMessageBodyWL MessageBodyWL = default;

        // 原文 38719：if TBaseObject(ProcessMsg.nParam3) <> nil then
        if (PlayerSurfaceServerSendSeams.ObjectPresent(AsNint(ProcessMsg.nParam3)))
        {
            // 原文 38721-38724
            MessageBodyWL.lParam1 = PlayerSurfaceServerSendSeams.GetCurrX(AsNint(ProcessMsg.nParam3));
            MessageBodyWL.lParam2 = PlayerSurfaceServerSendSeams.GetCurrY(AsNint(ProcessMsg.nParam3));
            MessageBodyWL.lTag1 = 0;
            MessageBodyWL.lTag2 = ProcessMsg.nParam3;
            // 原文 38725-38726
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_FLYAXE, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2, (int)ProcessMsg.wParam);
            // 原文 38727
            SendSocketExRef(m_DefMsg, StructBytes.BytesOf(MessageBodyWL));
        }
    }

    // ==================================================================
    // 38731-38745  ServerSendLighting
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendLighting(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38731-38745`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。
    /// ⚠ 原文如此：`nSeries` 传的是 **`TBaseObject(ProcessMsg.BaseObject).m_btDirection`**，
    ///   而尾数据走 `EncodeBuffer`（同一方法里读两个不同对象：BaseObject 与 nParam3）。</summary>
    public void ServerSendLighting(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38731-38745
        TMessageBodyWL MessageBodyWL = default;

        // 原文 38735：if TBaseObject(ProcessMsg.nParam3) <> nil then
        if (PlayerSurfaceServerSendSeams.ObjectPresent(AsNint(ProcessMsg.nParam3)))
        {
            // 原文 38737-38740
            MessageBodyWL.lParam1 = PlayerSurfaceServerSendSeams.GetCurrX(AsNint(ProcessMsg.nParam3));
            MessageBodyWL.lParam2 = PlayerSurfaceServerSendSeams.GetCurrY(AsNint(ProcessMsg.nParam3));
            MessageBodyWL.lTag1 = (int)ProcessMsg.wParam;
            MessageBodyWL.lTag2 = ProcessMsg.nParam3;
            // 原文 38741-38742
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_LIGHTING, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                PlayerSurfaceServerSendSeams.GetDirection(ProcessMsg.BaseObject));
            // 原文 38743
            SendSocketRef(m_DefMsg, DelphiRTL.AnsiString(EDcode.EncodeBuffer(
                StructBytes.BytesOf(MessageBodyWL), StructBytes.SizeOf<TMessageBodyWL>())));
        }
    }

    // ==================================================================
    // 38747-38753  ServerSendSubAbility
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendSubAbility(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38747-38753`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendSubAbility(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38747-38753
        // 原文 38749-38752：扩展准确、敏捷为65536 chongchong 2018-01-11
        PlayerSurfaceMsgSeams.SendDefMessage(
            this, (ushort)Grobal2Const.SM_SUBABILITY,
            (long)PlayerSurfacePack.MakeLong(
                PlayerSurfacePack.MakeWord(m_nAntiMagic, 0), m_btSpeedPoint),
            (ushort)m_btHitPoint,
            PlayerSurfacePack.MakeWord(m_btAntiPoison, m_nPoisonRecover),
            PlayerSurfacePack.MakeWord(m_nHealthRecover, m_nSpellRecover),
            IntToStr(m_nNPRecoverTime) + "/" + IntToStr(m_nNPRecoverPoint));
    }

    // ==================================================================
    // 38755-38797  ServerSendSpaceMoveShow
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendSpaceMoveShow(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38755-38797`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendSpaceMoveShow(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38755-38797
        TCharDesc CharDesc = default;

        // 原文 38762：if ProcessMsg.wIdent = RM_SPACEMOVE_SHOW then
        if (ProcessMsg.wIdent == Grobal2Const.RM_SPACEMOVE_SHOW)
        {
            // 原文 38764-38765
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_SPACEMOVE_SHOW, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                PlayerSurfacePack.MakeWord((int)ProcessMsg.wParam,
                    PlayerSurfaceServerSendSeams.GetLight(ProcessMsg.BaseObject)));
        }
        else
            // 原文 38768-38769
            m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(
                Grobal2Const.SM_SPACEMOVE_SHOW2, ProcessMsg.BaseObject,
                (int)ProcessMsg.nParam1, (int)ProcessMsg.nParam2,
                PlayerSurfacePack.MakeWord((int)ProcessMsg.wParam,
                    PlayerSurfaceServerSendSeams.GetLight(ProcessMsg.BaseObject)));

        // 原文 38771-38772
        (int featureLen, byte[] featureBytes) = PlayerSurfaceServerSendSeams.GetFeature(ProcessMsg.BaseObject, this);
        CharDesc.Feature = (byte)featureLen;
        CharDesc.Status = PlayerSurfaceServerSendSeams.GetCharStatus(ProcessMsg.BaseObject);
        // 原文 38773-38775
        byte[] s1C = BuildCharDescWithFeature(CharDesc, featureBytes, featureLen);
        // 原文 38776：nObjCount := GetCharColor(TBaseObject(ProcessMsg.BaseObject));
        int nObjCount = PlayerSurfaceServerSendSeams.GetCharColor(ProcessMsg.BaseObject);
        // 原文 38777-38778
        if (ProcessMsg.sMsg != "")
            s1C = ConcatBytes(s1C, EDcode.EncodeString(ProcessMsg.sMsg + "/" + IntToStr(nObjCount)));

        // 原文 38780
        SendSocketRef(m_DefMsg, DelphiRTL.AnsiString(s1C));

        // 原文 38782-38796：更换地图时初始化地图自增自减相关状态
        m_dwIncGoldTick = PlayerSurfaceServerSendSeams.MyGetTickCount(); // 地图自增金币Tick
        m_boIncGold = true;
        m_dwDecGoldTick = PlayerSurfaceServerSendSeams.MyGetTickCount(); // 地图自减金币Tick
        m_boDecGold = true;

        m_dwIncGameGoldTick = PlayerSurfaceServerSendSeams.MyGetTickCount(); // 命令自增游戏币Tick
        m_boIncGameGold = false;
        m_dwDecGameGoldTick = PlayerSurfaceServerSendSeams.MyGetTickCount(); // 命令自减游戏币Tick
        m_boDecGameGold = false;

        m_dwIncGamePointTick = PlayerSurfaceServerSendSeams.MyGetTickCount(); // 地图自增游戏点Tick
        m_boIncGamePoint = true;
        m_dwDecGamePointTick = PlayerSurfaceServerSendSeams.MyGetTickCount(); // 地图自减游戏点Tick
        m_boDecGamePoint = true;
    }

    // ==================================================================
    // 38799-38803  ServerSendReconnection
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendReconnection(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38799-38803`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendReconnection(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38799-38803
        // 原文 38801
        m_boReconnection = true;
        // 原文 38802
        PlayerSurfaceMsgSeams.SendDefMessage(
            this, (ushort)Grobal2Const.SM_RECONNECT, 0, 0, 0, 0, ProcessMsg.sMsg);
    }

    // ==================================================================
    // 38805-38810  ServerSendHideEvent
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendHideEvent(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38805-38810`）。★ **无 `&lt;&gt; Self` 守卫**（原文如此）。</summary>
    public void ServerSendHideEvent(TProcessMessage ProcessMsg, ref bool boResult)
    {
        // 原文 38805-38810
        // 原文 38807-38808（TODO + 注释，原样保留）：
        //   { TODO -opiaoyun -c修改 : 烟花效果处理【2013-6-7】 }
        //   // if (TGameEvent(ProcessMsg.nParam1) is TMapEffectEvent) then
        // 原文 38809
        PlayerSurfaceMsgSeams.SendDefMessage(
            this, (ushort)Grobal2Const.SM_HIDEEVENT, ProcessMsg.nParam1,
            (ushort)ProcessMsg.wParam, (ushort)ProcessMsg.nParam2, (ushort)ProcessMsg.nParam3,
            "");
    }

    // ==================================================================
    // 38812-38858  ServerSendShowEvent —— 留痕
    // ==================================================================

    /// <summary>原文 `procedure TPlayObject.ServerSendShowEvent(ProcessMsg; var boResult);`
    /// （`ObjPlayer.pas:38812-38858`，**47 行**）。
    /// <para><b>未移植（显式留痕）</b>。缺失成员：原文整段依赖 **Delphi RTTI 的
    /// <c>is</c> 类型判定** —— <c>TGameEvent</c>/<c>TMapEffectEvent</c>/
    /// <c>TCustomEffectEvent</c>/<c>TCustomMagicEffectEvent</c>/<c>TMapMagicGameEvent</c>/
    /// <c>TSafeEvent</c> 在托管侧**都不是可实例化的类型**（`src` 下不存在这些 class，
    /// 仅有 <c>GameEventCore</c>/<c>GameEventSubclassCore</c> 等"只固化语义"的 Core 常量类）。
    /// 涉及行：:38820 <c>is TMapEffectEvent</c>（读 <c>m_nImageIndex</c>/<c>m_nImageCount</c>/
    /// <c>m_btLight</c>/<c>m_dwSpeedTime</c>/<c>m_nFileIndex</c>/<c>m_nLoopCount</c>）、
    /// :38832 <c>is TCustomEffectEvent</c>（读 <c>OwnerAppr</c>/<c>AttackIndex</c>）、
    /// :38838 <c>is TCustomMagicEffectEvent</c>（读 <c>MagicID</c>/<c>NewLevel</c>）、
    /// :38848 <c>is TMapMagicGameEvent</c>（读 <c>KeepVisible</c>）、
    /// :38853 <c>is TSafeEvent</c>（读 <c>Dir</c>）。
    /// 用 <c>(object)</c> 强转 + <c>is</c> 近似会**改变原文语义**（原文是"未匹配任何类型则
    /// 发全 0"，托管侧没有这些类型就无法表达"匹配"），故按任务书第 4 条整条留痕。</para>
    /// </summary>
    public void ServerSendShowEvent(TProcessMessage ProcessMsg, ref bool boResult)
    {
        PortNotPorted(nameof(ServerSendShowEvent), 38812);
    }
}
