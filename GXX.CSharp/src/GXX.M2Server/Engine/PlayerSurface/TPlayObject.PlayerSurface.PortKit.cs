// ============================================================================
// 源单元：Source\M2Engine\ObjPlayer.pas（GBK / UTF-8 镜像 49,232 LF）
// 本文件：**移植基础设施（PortKit）** —— 本车道 p13-m2-objplayer 的公共约定层。
//
// 为什么需要它：ObjPlayer.pas 的 **815 条类方法**（TPlayObject 811 + TWarrContinueHitManager 4）
// 里绝大多数会用到四类**跨方法**的东西，若每条方法各写一遍，就会在同一个 partial 类里
// 产生 800 份重复的辅助代码（且极易互相漂移）。故按工程既有惯例（`PlayerSurfaceMsgSeams`
// / `PlayerSurfaceBaseSeams` 等）集中一次。
//
// 四个约定（本车道所有切片共用，**不得各自另造**）：
//   1. NotPorted(...)  —— 未 1:1 移入的显式留痕（台账 §48.1 硬要求；**禁止裸 `=> true;`**）
//   2. MmMakeWord / MmMakeLong 等 —— Delphi `MakeWord`/`MakeLong` 的 1:1 复刻
//   3. SendSocket* 落点 —— 原文 socket 下发面的接缝（**报文装配逻辑仍逐行移植**）
//   4. 原文行号引用约定 —— 每个方法体首行注释写 `// 原文 NNNN`
//
// ⚠ 本文件**不声明任何原文业务字段**；字段由各切片在自己的片文件里按原文声明。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.M2Server.Engine;

/// <summary>
/// 本车道（p13-m2-objplayer）的**未移植留痕**登记处。
///
/// 台账 §48.1 的硬要求：凡暂时无法 1:1 移入的方法，必须**显式**调用
/// <see cref="TPlayObject.PortNotPorted"/> 留痕，**禁止**写裸 `=> true;` 之类的
/// "沉默桩" —— 后者在覆盖率审计里看起来像"已实现"，是本工程反复出问题的根因。
///
/// 与 `GXX.Client` 的 `TMirConfigDlg.NotPortedMethods`（`MirConfigDlg.Misc.cs:51`）同型，
/// 但**不共用**（不同程序集、不同单元），故此处另立一份并保持同样的语义：
/// 每次调用追加一条 `"方法名 (ObjPlayer.pas:行号)"`。
/// </summary>
public static class PlayerSurfacePortLedger
{
    /// <summary>已留痕的未移植方法（形如 <c>ClientSpell (ObjPlayer.pas:19004)</c>）。</summary>
    public static readonly List<string> NotPortedMethods = new List<string>();

    /// <summary>留痕（幂等：同一 `方法名 (行号)` 只记一次，避免循环里刷爆列表）。</summary>
    public static void NotPorted(string method, int line)
    {
        string entry = method + " (ObjPlayer.pas:" + line + ")";
        if (!NotPortedMethods.Contains(entry))
            NotPortedMethods.Add(entry);
    }

    /// <summary>清空留痕（单测隔离用）。</summary>
    public static void ResetNotPorted() => NotPortedMethods.Clear();

    /// <summary>当前留痕条数（测试与报告据此统计真实进度）。</summary>
    public static int NotPortedCount => NotPortedMethods.Count;
}

/// <summary>
/// Delphi 位/字节打包函数的 1:1 复刻（原文 `EDcode.pas` / `HUtil32.pas` 的全局函数，
/// ObjPlayer.pas 里被 **数百次** 调用 —— 属本车道高频共享设施）。
/// </summary>
public static class PlayerSurfacePack
{
    /// <summary>
    /// 原文 `MakeWord(bLow, bHigh: Byte): Word` —— `bLow or (bHigh shl 8)`。
    /// ⚠ 原文实参常是 **`Integer`/`LongWord`**（如 `MakeWord(wParam, m_nLight)`），
    /// Delphi 会**静默窄化到 Byte**（高 24 位丢弃）——托管侧必须 `(byte)` 显式转换（**不是** `& 0xFF` 后当 int 用）。
    /// </summary>
    public static ushort MakeWord(int bLow, int bHigh)
        => (ushort)((byte)bLow | ((byte)bHigh << 8));

    /// <summary>
    /// 原文 `MakeLong(wLow, wHigh: Word): LongWord` —— `wLow or (wHigh shl 16)`。
    /// </summary>
    public static uint MakeLong(int wLow, int wHigh)
        => (uint)((ushort)wLow | ((ushort)wHigh << 16));

    /// <summary>原文 `LoWord(n: LongWord): Word` —— 取低 16 位。</summary>
    public static ushort LoWord(nint n) => (ushort)((ulong)n & 0xFFFF);

    /// <summary>原文 `HiWord(n: LongWord): Word` —— 取高 16 位（**不做符号扩展**）。</summary>
    public static ushort HiWord(nint n) => (ushort)(((ulong)n >> 16) & 0xFFFF);

    /// <summary>原文 `LoByte(n: LongWord): Byte`。</summary>
    public static byte LoByte(nint n) => (byte)((ulong)n & 0xFF);

    /// <summary>原文 `HiByte(n: LongWord): Byte`。</summary>
    public static byte HiByte(nint n) => (byte)(((ulong)n >> 8) & 0xFF);

    /// <summary>
    /// 原文 `MakeDefaultMsg(wIdent; nRecog: Int64; wParam, wTag, wSeries: Word): TDefaultMessage`。
    /// 托管侧等价物是 <see cref="TDefaultMessage.Make"/>（`Grobal2.Types1.cs:54`）——
    /// 本包装只为让移植体的字面形状与原文一致，**不做任何额外处理**。
    /// </summary>
    public static TDefaultMessage MakeDefaultMsg(int wIdent, long nRecog, int wParam, int wTag, int wSeries)
        => TDefaultMessage.Make((ushort)wIdent, nRecog, (ushort)wParam, (ushort)wTag, (ushort)wSeries);
}

/// <summary>
/// 原文 `TBaseObject.SendSocket` / `SendSocketEx` 一族在移植体里的**唯一落点**。
///
/// **为什么是接缝**：原文这两个方法最终走 `TUserEngine`/网关的 socket 发送面
/// （`ObjBase.pas:30980` 一带 + `UsrEngn.pas`），托管侧**该面尚未移植**
/// （`Engine/ObjBase.OnlineMsg.cs` 只有入队版 `TCreature.SendMsg`）。
/// 按工程惯例（台账 §14.2「不造第三份实现」+ 接缝最小面），此处只暴露委托，
/// 默认 = 「无宿主，丢弃」。
///
/// ★ 关键：**接缝的是"投递"，不是"报文装配"** —— `m_DefMsg := MakeDefaultMsg(...)`、
/// 变参编码（`EncodeBuffer`/`EncodeString`）、限流计数等等**仍逐行移植**在各方法体内。
/// 因此接缝默认不生效时，移植体里的**状态副作用与装配结果依然可测**。
/// </summary>
public static class PlayerSurfaceSocketSeams
{
    /// <summary>
    /// 原文 `SendSocket(pMsg: pTDefaultMessage; sMsg: string)`（`ObjBase.pas`）。
    /// 参数：目标玩家 / 报文 / 附加字符串（已编码）。默认：无宿主，丢弃。
    /// </summary>
    public static Action<TPlayObject, TDefaultMessage, string> SendSocket { get; set; }
        = (_, _, _) => { };

    /// <summary>
    /// 原文 `SendSocketEx(pMsg: pTDefaultMessage; pBuf: Pointer; nLen: Integer)`
    /// —— 追加一段**二进制**尾数据（如 `@m_nCharStatus`）。默认：无宿主，丢弃。
    /// </summary>
    public static Action<TPlayObject, TDefaultMessage, byte[]> SendSocketEx { get; set; }
        = (_, _, _) => { };

    /// <summary>
    /// 原文 `TBaseObject.SendMsg(BaseObject; wIdent; wParam..; sMsg)`（`ObjBase.pas:30339`）
    /// —— 带**视野白名单**的发送面。与托管 `TCreature.SendMsg(ushort,...)`（入队版）**同名不同义**，
    /// 故按 `p6-m2-playersurface` §8-G 的登记，本车道用**独立名字**避免重载混淆。
    /// 默认：无宿主，丢弃。
    /// </summary>
    public static Action<TPlayObject, TCreature?, int, long, int, int, int, string> SendViewMsg { get; set; }
        = (_, _, _, _, _, _, _, _) => { };

    /// <summary>恢复全部默认实现（单测隔离用）。</summary>
    public static void ResetDefaults()
    {
        SendSocket = (_, _, _) => { };
        SendSocketEx = (_, _, _) => { };
        SendViewMsg = (_, _, _, _, _, _, _, _) => { };
    }
}

/// <summary>
/// `ObjPlayer.pas` 移植体的公共成员：移植记账与打包辅助的**实例侧**转发。
/// </summary>
public partial class TPlayObject
{
    /// <summary>
    /// 未 1:1 移入的方法的**显式留痕**（台账 §48.1）。
    /// 用法：<c>PortNotPorted(nameof(Xxx), 12345);</c> —— **不得**改成裸 `=> true;`。
    /// </summary>
    protected static void PortNotPorted(string method, int line)
        => PlayerSurfacePortLedger.NotPorted(method, line);

    /// <summary>
    /// 原文 `m_DefMsg: TDefaultMessage;`（`ObjBase.pas`，`TBaseObject` 的报文暂存字段）
    /// —— ObjPlayer.pas 的 `ServerSend*` 族 **全部**先写它再 `SendSocket(@m_DefMsg, ...)`。
    /// </summary>
    public TDefaultMessage m_DefMsg;

    /// <summary>
    /// 原文 `SendSocket(pMsg: pTDefaultMessage; sMsg: string)` 的落点（见
    /// <see cref="PlayerSurfaceSocketSeams"/>）。
    /// </summary>
    public void SendSocketRef(TDefaultMessage msg, string sMsg)
    {
        // 原文：先写本对象暂存字段，再交给 socket 层（顺序在原文里是 SendSocket 内部完成）
        m_DefMsg = msg;
        PlayerSurfaceSocketSeams.SendSocket(this, msg, sMsg);
    }

    /// <summary>原文 `SendSocketEx(pMsg; pBuf; nLen)` 的落点。</summary>
    public void SendSocketExRef(TDefaultMessage msg, byte[] buf)
    {
        m_DefMsg = msg;
        PlayerSurfaceSocketSeams.SendSocketEx(this, msg, buf);
    }

    // ==================================================================
    // 跨切片共享字段块（**唯一归属方：本文件**）
    //
    // 为什么集中在这里：本单元被切成 6 个并行片，多片会同时用到同一批原文字段。
    // 若各片各声明一次就是 CS0102；若各片都不声明就是 CS0103（本车道集成时**两种都真实发生过**）。
    // 故按"跨片共享 ⇒ 落在基础设施文件"的原则集中于此，各片**只许引用、不许再声明**。
    // 全部是**直译**，名字与原文逐字一致（含原文的 `m_WAbil` 大小写）。
    // ==================================================================

    /// <summary>原文 `m_boDealing: Boolean; // 0x317`（`ObjPlayer.pas:31`）—— 是否正在交易。</summary>
    public bool m_boDealing;

    /// <summary>原文 `m_DealLastTick: LongWord; // 0x318`（`ObjPlayer.pas:32`）—— 交易心跳。</summary>
    public uint m_DealLastTick;

    /// <summary>原文 `m_DealCreat: TPlayObject; // 0x31C`（`ObjPlayer.pas:33`）—— 交易对手。</summary>
    public TPlayObject? m_DealCreat;

    /// <summary>
    /// 原文 `m_nViewRange: Integer;`（**`TBaseObject`**，`ObjBase.pas`）—— 视野范围。
    /// ⚠ **层级偏差（已登记）**：托管侧同名成员 `m_nViewRange` 已存在于 `TMonster`
    /// （`Engine/ObjBase.cs:220`，由 `p9-m2-monsters` 车道声明）。原文里 `TPlayObject` 也有它
    /// （`ObjPlayer.pas:1533` 初始化 `m_nViewRange := 12`），故这里在 `TPlayObject` 上补一份。
    /// `TPlayObject` 与 `TMonster` 是**兄弟类**（都直接继承 `TCreature`），不存在隐藏关系。
    /// </summary>
    public int m_nViewRange = 12;   // 原文 ObjPlayer.pas:1533 的初值

    /// <summary>原文 `m_nMemberType: Integer;`（`ObjPlayer.pas:1577` 初始化 0）—— 会员类型。</summary>
    public int m_nMemberType;

    /// <summary>原文 `m_nMemberLevel: Integer;`（`ObjPlayer.pas:1578` 初始化 0）—— 会员等级。</summary>
    public int m_nMemberLevel;

    /// <summary>原文 `m_boOnHorse: Boolean;`（`TBaseObject`，`ObjBase.pas`）—— 是否骑马。</summary>
    public bool m_boOnHorse;

    /// <summary>原文 `m_boAdminMode: Boolean;`（`TBaseObject`，`ObjBase.pas`）—— 管理员模式（隐身）。</summary>
    public bool m_boAdminMode;

    /// <summary>原文 `m_nPayMent: Integer;`（`TBaseObject`，`ObjBase.pas`）—— 登录模式/计费模式。</summary>
    public int m_nPayMent;

    /// <summary>原文 `m_boKickFlag: Boolean;`（`ObjPlayer.pas:1508` 初始化 False）。</summary>
    public bool m_boKickFlag;

    /// <summary>
    /// 原文 `m_WAbil: TAbility;` 的**大小写直译别名**（原文同一个单元里两种写法都出现：
    /// `m_wAbil` 与 `m_WAbil`；托管既有字段是 `TCreature.m_wAbil`，`Engine/ObjBase.cs:65`）。
    /// 本车道若干移植体按原文抄了 `m_WAbil`，故此处给一个 `ref` 别名，
    /// 使两种写法落到**同一块存储**（与 `Core3` 的 `m_Abil => ref m_wAbil` 同一手法）。
    /// ⚠ 只读用途之外也可写（`ref` 属性可写），语义等价。
    /// </summary>
    public ref TAbility m_WAbil => ref m_wAbil;

    /// <summary>
    /// 原文 `TBaseObject.DelTargetCreat`（`ObjBase.pas:35825-35829`）——
    /// **只是 `if m_TargetCret &lt;&gt; nil then m_TargetCret := nil`**
    /// （`p9-m2-monsters` 的 `AnimalPathCore.cs:81` 已按原文登记了四个覆写版本的行为差异）。
    /// 托管侧 `TCreature` 尚无此方法，而本单元多处会调用它
    /// （如 `ObjPlayer.pas:40020-40021` 的 `m_TargetCret.DelTargetCreat`）⇒ 在此按 `TBaseObject` 版补齐。
    /// ⚠ 层级偏差：原文在 `TBaseObject`（托管未切出），落在 `TPlayObject` 上
    /// （与本车道其它成员的既有口径一致）。四个覆写版本的行为差异**未在此表达**
    /// —— 那属 `p9-m2-monsters` 的对象模型面。
    /// </summary>
    public void DelTargetCreat()
    {
        // 原文 35827：if m_TargetCret <> nil then m_TargetCret := nil;
        if (m_TargetCret != null)
            m_TargetCret = null;
    }
}

/// <summary>
/// `M2Share.pas` 的**日志动作码**接缝（`ObjPlayer.pas` 的 `AddGameDataLog(...)` 调用点使用）。
///
/// 原文定义处（**实测**）：
/// <list type="bullet">
///   <item><description>`M2Share.pas:105` `LOG_ItemTakeBack = 18; // 取回物品`</description></item>
///   <item><description>`M2Share.pas:118` `LOG_GamePointChange = 52; // 游戏点改变`</description></item>
///   <item><description>`M2Share.pas:123` `LOG_LevelChange = 57; // 等级改变`</description></item>
/// </list>
/// 同一组常量在 `LogDataServer/LogManage.pas:121/134/139` 也有**同值**定义（交叉印证）。
/// ⚠ 既有 `GXX.M2Server.Npc.ObjNpcConst`（`Npc/ObjNpcSeams.cs:27`）已收录
/// `LOG_ActionNone` / `LOG_ItemSell` / `LOG_ItemBuy` / `LOG_ItemUpgrade` / `LOG_GoldChange` /
/// `LOG_ItemDisappear`，**但没有** `LOG_LevelChange` —— 本车道在其**自己的分区内**补齐，
/// 不修改 `Npc/**`（跨车道文件）。
/// </summary>
public static class PlayerSurfaceLogActionConst
{
    /// <summary>原文 `LOG_LevelChange = 57; // 等级改变`（`M2Share.pas:123`）。</summary>
    public const byte LOG_LevelChange = 57;
}
