using System;
using System.Collections.Generic;
using GXX.Core;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.GatewayKit.Rest11;

/// <summary>
/// 车道 p11-logingate-filter 的**可选（opt-in）**LoginGate 执法面设施。
///
/// <para>
/// 来源：`Source/LoginGate/Misc.pas`(321) + `FuncForComm.pas`(590) + `IPAddrFilter.pas`(400 残部) +
/// `ConfigManager.pas`(269 残部) + `ClientSession.pas`(820 残部)。
/// 复核报告：`docs/并行报告-p11-logingate-review.md` §2/§3/§6/§7、§5.4 车道建议。
/// </para>
///
/// <para>
/// ⚠ 与既有设施的关系（**并存**，不合并、不"统一"）：
/// <list type="number">
///   <item>`GXX.SelGate.CSelGateIPFilter`（SelGateIPAddrFilter.cs，与 LoginGate 副本 SHA256 相同）
///         用 **SelGate 的** `CConfigMgr` / `SelGateGlobals` / `SelGateProtocol._STR_*`；
///         LoginGate 的配置文件段名（`[LoginGate]/[Integer]/[Switch]/[Method]` vs SelGate 的
///         `Strings/Integer/Switch/Method`）、默认端口（5500/7000+i "vs" 5100/7100+i）与
///         `Misc.ReverseIP` 的**模块限定名**都不同 ⇒ 本命名空间的类型**自带**这些差异，
///         SelGate 侧一行不改，`SelGate.Tests` 必须继续全绿。</item>
///   <item>`GXX.GatewayKit.GateService` 的 `CheckIP/_blockList/_perIP/AddBlockIP`
///         （GateService.cs:203-227）保持**默认生效且语义不变**；本设施**不替换**它。</item>
///   <item>`Rest11LoginGateOptions.Enable*` 全为 **false** 时，本命名空间的所有类型
///         对既有运行路径**零影响**（没有任何静态构造副作用之外的接线）。</item>
/// </list>
/// </para>
///
/// <para>
/// 命名约定：`T`/`p` 前缀、`m_`/`g_` 前缀逐字保留（1:1 忠实要求），故这些名字**刻意**与
/// Delphi 原文同名；`Rest11` 前缀用于类名以避免与既有 `TPerIPAddr` 等托管投影撞名。
/// </para>
/// </summary>
public sealed class Rest11LoginGateOptions
{
    /// <summary>对应 `Advapi32.DWORD`（原文 `ReverseIP`/`TIPArea.Low` 的类型）。</summary>
    public const int Rest11DefaultUserArrayCount = 1000 + 48; // USER_ARRAY_COUNT = MAX_GAME_USER + 48（AcceptExWorkedThread.pas:11-13）

    /// <summary>FuncForComm.pas 的 `TProcMsgThread` 超时/延迟关闭巡检（客户端超时踢线 + DelayClose）。</summary>
    public bool EnableProcMsgThread;

    /// <summary>ConfigManager.pas 的 19 字段 + `[LoginGate]` 原文段名（**不兼容**现有 Config.ini）。</summary>
    public bool EnableLoginGateIniSections;

    /// <summary>IPAddrFilter.pas 残部：IP 段过滤 / 临时黑名单 / CheckNewIDOfIP / m_fCheckNullSession。</summary>
    public bool EnableIpAddrFilterResidual;

    /// <summary>Misc.pas 的 8 个执法例程（KickUser/BlockUser/CloseIPConnect/ReverseIP/...）。</summary>
    public bool EnableMiscEnforcement;

    /// <summary>ClientSession.pas 的 LoginGate 独有残部（协议密码 / 二级密码 / DelayClose / RotateBits）。</summary>
    public bool EnableSessionResidual;

    /// <summary>全部关闭（默认）—— 保证"默认不改变现有行为"。</summary>
    public static Rest11LoginGateOptions Disabled => new();

    /// <summary>全部打开（供 LoginGate 显式 opt-in 或测试）。</summary>
    public static Rest11LoginGateOptions All => new()
    {
        EnableProcMsgThread = true,
        EnableLoginGateIniSections = true,
        EnableIpAddrFilterResidual = true,
        EnableMiscEnforcement = true,
        EnableSessionResidual = true
    };
}

/// <summary>
/// `Protocol.pas:83` `TBlockIPMethod = (mDisconnect, mBlock, mBlockList)`。
/// 与 `GXX.SelGate.TBlockIPMethod`、`GXX.GatewayKit.TBlockIPMethod` 数值一致，但**不合并**：
/// 三个模块各自持有自己的投影（§14.2"不造第三份实现"指**实现**，纯枚举投影按模块保留以免跨模块耦合）。
/// </summary>
public enum Rest11TBlockIPMethod
{
    mDisconnect = 0, // :83 第 1 项
    mBlock = 1,      // :83 第 2 项 ← "动态/临时"过滤
    mBlockList = 2   // :83 第 3 项 ← "永久"过滤
}

/// <summary>`Protocol.pas:84` `TSockThreadStutas = (stConnecting, stConnected, stTimeOut)`。</summary>
public enum Rest11TSockThreadStutas
{
    stConnecting = 0, // :84
    stConnected = 1,   // :84
    stTimeOut = 2      // :84
}

/// <summary>`Protocol.pas:86-90` `TPerIPAddr = record IPaddr: LongInt; Count: Integer`（每 IP 连接数）。</summary>
public struct TPerIPAddr
{
    public int IPaddr; // :87
    public int Count;  // :88
}

/// <summary>`Protocol.pas:92-97` `TNewIDAddr = record IPaddr; Count; dwIDCountTick`（每 IP 换 ID 频率）。</summary>
public struct TNewIDAddr
{
    public int IPaddr;         // :93
    public int Count;          // :94
    public uint dwIDCountTick; // :95
}

/// <summary>`Protocol.pas:99-103` `TIPArea = record Low: DWORD; High: DWORD`（IP 段）。</summary>
public struct TIPArea
{
    public uint Low;  // :100
    public uint High; // :101
}

/// <summary>
/// `ClientSession.pas` 的 `TSessionObj` 中，`Misc.pas` / `FuncForComm.pas` / `IPAddrFilter.pas`
/// 判定所需的**只读接缝**（其余字段见 `Rest11LoginGateSession`）。
///
/// <para>
/// `m_tLastGameSvr.Active`（:166/:167/:211 的 `UserObj.m_tLastGameSvr.Active`）在托管侧由
/// <see cref="Active"/> 表达；`m_pUserOBJ._SendObj.Socket` 由 <see cref="Socket"/> 表达。
/// </para>
/// </summary>
public interface IRest11SessionObj
{
    /// <summary>`m_pUserOBJ.nIPAddr`（LongInt，inet_addr 结果）。</summary>
    int IPAddr { get; }

    /// <summary>`m_pUserOBJ.pszIPAddr`（点分字符串，日志用）。</summary>
    string IPText { get; }

    /// <summary>`m_tLastGameSvr &lt;&gt; nil and m_tLastGameSvr.Active`（合成为一个判定，原文两处都这么用）。</summary>
    bool LastGameSvrActive { get; }

    /// <summary>`m_pUserOBJ._SendObj.Socket`。</summary>
    int Socket { get; }

    /// <summary>`m_fKickFlag`（可写：KickUser/BlockUser/DelayClose 都要置位）。</summary>
    bool KickFlag { get; set; }

    /// <summary>`m_fHandleLogin: Byte`（`>= 2` 走 SM_OUTOFCONNECTION，`< 3` 才进入超时判定）。</summary>
    byte HandleLogin { get; }

    /// <summary>`m_nSvrObject: Integer`。</summary>
    int SvrObject { get; }

    /// <summary>`m_dwClientTimeOutTick: LongWord`（可写：巡检会刷新它）。</summary>
    uint dwClientTimeOutTick { get; set; }

    /// <summary>`m_IsDelayClose: Boolean`（可写：DelayClose 到期时被清掉）。</summary>
    bool IsDelayClose { get; set; }

    /// <summary>`m_dwDelayCloseTick: LongWord`。</summary>
    uint dwDelayCloseTick { get; set; }
}

/// <summary>
/// `CloseIPConnect`（Misc.pas:41-66）/ `KickUser` 系列所需的执法副作用的**注入点**。
///
/// <para>
/// 原文直接调用 `SHSocket.FreeSocket(...)`、`UserObj.SendDefMessage(...)`、
/// `AddToBlockIPList/AddToTempBlockIPList` 与 `g_fServiceStarted` 全局；
/// 托管侧一律经本接口注入，使测试**不依赖真实网络端口**（见 `tests/GXX.GatewayKit.Tests/Rest11*.cs`）。
/// </para>
/// </summary>
public interface IRest11EnforcementChannel
{
    /// <summary>`g_fServiceStarted`（Protocol.pas:109）。</summary>
    bool ServiceStarted { get; }

    /// <summary>`SHSocket.FreeSocket(Socket)` —— 关闭该连接。</summary>
    void FreeSocket(int socket);

    /// <summary>
    /// `UserObj.SendDefMessage(SM_OUTOFCONNECTION, UserObj.m_nSvrObject, 0, 0, 0, '')`
    /// （Misc.pas:172）；`ident` 由实现决定（调用点只发 SM_OUTOFCONNECTION）。
    /// </summary>
    void SendOutOfConnection(IRest11SessionObj session, int nSvrObject);

    /// <summary>`AddToTempBlockIPList`（IPAddrFilter.pas:114/126）。</summary>
    void AddToTempBlockIPList(int nIP);

    /// <summary>`AddToBlockIPList`（IPAddrFilter.pas:79/91）。</summary>
    void AddToBlockIPList(int nIP);

    /// <summary>`g_pLogMgr.CheckLevel(nLevel)`（LogManager.pas）。</summary>
    bool CheckLevel(int level);

    /// <summary>`g_pLogMgr.Add(sMsg)`（LogManager.pas）。</summary>
    void AddLog(string sMsg);
}

/// <summary>
/// `Misc.pas:141-175` `SendGameCenterMsg` 的托管接缝：
/// 对应 `SendMessage(g_hGameCenterHandle, WM_COPYDATA, nParam, @SendData)`。
///
/// <para>
/// LoginGate 副本与 SelGate 副本的差异（**必须保留**）：LoginGate 的 `nParam` 恒为
/// `MakeLong(Word(tLoginGate), wIdent)`（`Misc.pas:280`，tLoginGate = 4），
/// **没有** SelGate 的 `g_boNetComGate → tSelGate1` 分支（`SelGateMisc.cs:150-153`）。
/// </para>
/// </summary>
public interface IRest11GameCenterChannel
{
    /// <summary>`SendMessage(hWnd, WM_COPYDATA, nParam, @SendData)`；文本含结尾 `#0`（`cbData = Length + 1`）。</summary>
    void SendCopyData(IntPtr hWnd, int nParam, string sSendMsg);
}

/// <summary>
/// 一个**可自增的下标**，用于 1:1 复刻 Delphi 的 `var nPos: Integer` / `var dest: string`
/// 出参（`AnsiStrToVal` / `GetValidStr3`）。
/// </summary>
public sealed class Rest11Ref<T>
{
    public T Value;

    public Rest11Ref(T value) => Value = value;

    public static implicit operator T(Rest11Ref<T> r) => r.Value;

    public override string ToString() => Value?.ToString() ?? "";
}

/// <summary>
/// `Misc.pas:244-250` `ReverseIP(dwIP: DWORD): DWORD` 与 WinSock `inet_addr` / `inet_ntoa`
/// 的 **LoginGate 副本自带**实现（原文 `IPAddrFilter.pas:278-279` 写的是 `Misc.ReverseIP(...)`，
/// 即 LoginGate 的 `Misc` 单元，因此这里与 `CSelGateIPFilter.ReverseIP` 是**同一算法的两份投影**；
/// 因命名空间不同、且 LoginGate 不得依赖 SelGate，故并存并在此写明来源）。
/// </summary>
public static class Rest11LoginGateNet
{
    /// <summary>WinSock `INADDR_NONE = $FFFFFFFF`（`LongInt` 视角 = -1）。</summary>
    public const int INADDR_NONE = -1;

    /// <summary>`g_hGameCenterHandle`（Protocol.pas:107）默认值。</summary>
    public static IntPtr g_hGameCenterHandle;

    /// <summary>
    /// `Misc.pas:244-250`：
    /// <code>
    /// Result := (LOBYTE(LOWORD(dwIP)) shl 24) or (HIBYTE(LOWORD(dwIP)) shl 16) or
    ///           (LOBYTE(HIWORD(dwIP)) shl 8)  or (HIBYTE(HIWORD(dwIP)));
    /// </code>
    /// </summary>
    public static uint ReverseIP(uint dwIP)
        => ((uint)(byte)(dwIP & 0xFF) << 24)          // :246 LOBYTE(LOWORD(dwIP)) shl 24
         | ((uint)(byte)((dwIP >> 8) & 0xFF) << 16)   // :247 HIBYTE(LOWORD(dwIP)) shl 16
         | ((uint)(byte)((dwIP >> 16) & 0xFF) << 8)   // :248 LOBYTE(HIWORD(dwIP)) shl 8
         | (uint)(byte)((dwIP >> 24) & 0xFF);         // :249 HIBYTE(HIWORD(dwIP))

    /// <summary>
    /// WinSock `inet_addr`：支持 a.b.c.d / a.b.c / a.b / a；数值段按 C 字面量解析
    /// （`0x` 十六进制、前导 0 八进制）；非法输入返回 <see cref="INADDR_NONE"/>；
    /// 段数不足时把最后一段放在低位（"1.2.3" → 0x01020003）。
    /// 与 `CSelGateIPFilter.InetAddr` 同算法（同一份 WinSock 语义的两处投影）。
    /// </summary>
    public static int InetAddr(string s)
    {
        if (string.IsNullOrEmpty(s)) return INADDR_NONE;

        // 原文 inet_addr 自身不 Trim（调用点大多已 Trim）——保留严格行为。
        string[] parts = s.Split('.');
        if (parts.Length > 4) return INADDR_NONE;

        long[] v = new long[4];
        for (int i = 0; i < parts.Length; i++)
        {
            if (!TryParseCLong(parts[i], out long parsed)) return INADDR_NONE;
            v[i] = parsed;
        }

        long addr;
        switch (parts.Length)
        {
            case 1:
                if (v[0] > 0xFFFFFFFFL) return INADDR_NONE;
                addr = v[0];
                break;
            case 2:
                if (v[0] > 0xFF || v[1] > 0xFFFFFFL) return INADDR_NONE;
                addr = (v[0] << 24) | v[1];
                break;
            case 3:
                if (v[0] > 0xFF || v[1] > 0xFF || v[2] > 0xFFFFL) return INADDR_NONE;
                addr = (v[0] << 24) | (v[1] << 16) | v[2];
                break;
            default:
                if (v[0] > 0xFF || v[1] > 0xFF || v[2] > 0xFF || v[3] > 0xFF) return INADDR_NONE;
                addr = (v[0] << 24) | (v[1] << 16) | (v[2] << 8) | v[3];
                break;
        }
        return unchecked((int)addr);
    }

    /// <summary>C 字符串数值字面量："10"(十进制) / "0x1f"(十六进制) / "017"(八进制)。</summary>
    public static bool TryParseCLong(string s, out long value)
    {
        value = 0;
        if (string.IsNullOrEmpty(s)) return false;
        string t = s.Trim();
        if (t.Length == 0) return false;

        int radix = 10;
        if (t.Length > 2 && (t.StartsWith("0x") || t.StartsWith("0X")))
        {
            radix = 16;
            t = t.Substring(2);
        }
        else if (t.Length > 1 && t[0] == '0')
        {
            radix = 8;
            t = t.Substring(1);
        }
        if (t.Length == 0) { value = 0; return true; }

        long acc = 0;
        foreach (char c in t)
        {
            int d;
            if (c >= '0' && c <= '9') d = c - '0';
            else if (c >= 'a' && c <= 'f') d = c - 'a' + 10;
            else if (c >= 'A' && c <= 'F') d = c - 'A' + 10;
            else return false;
            if (d >= radix) return false;
            acc = acc * radix + d;
            if (acc > 0xFFFFFFFFL) return false;
        }
        value = acc;
        return true;
    }

    /// <summary>WinSock `inet_ntoa`（网络序整数 → 点分串）。</summary>
    public static string? InetNtoa(int nIP)
    {
        uint v = (uint)nIP;
        return $"{(byte)((v >> 24) & 0xFF)}.{(byte)((v >> 16) & 0xFF)}.{(byte)((v >> 8) & 0xFF)}.{(byte)(v & 0xFF)}";
    }

    /// <summary>
    /// `HUtil32.GetValidStr3(Str, Dest, Divider)`：按分隔符切第一段到 `Str`，
    /// 其余到 `Dest`。原文（HUtil32.pas:1254-…）**不跳过**紧随分隔符的空白，
    /// 此处逐字对齐 `GXX.Core.Util.HUtil32.GetValidStr3`。
    ///
    /// ★ 注意方向与命名：`GXX.Core.Util.HUtil32.GetValidStr3` 的 `ref string dest`
    /// 收到的才是**第一段**、返回值是**剩余串**（该实现的**出参命名与 Delphi 原文相反**，
    /// 但**已核对语义与 Delphi 一致**）；此处按 Delphi 的 `Result=第一段 / Dest=剩余串` 对外呈现。
    /// 另：核心实现在 :283 把分隔符**吃掉**，而 Delphi 原文的剩余串**保留**分隔符 —— 偏差登记见报告 §6。
    /// </summary>
    public static string GetValidStr3(string str, Rest11Ref<string> dest, char[] divider)
    {
        string d = "";
        string remainder = HUtil32.GetValidStr3(str, ref d, divider);
        dest.Value = remainder;
        return d;
    }
}
