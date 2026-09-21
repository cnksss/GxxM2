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
}
