// 源单元：Source/Client-HGE/WinSock2.pas（1405 行；本文件覆盖 implementation 段的
//          纯逻辑函数与 socket 调用接缝）
// 常量见 WinSock2Constants.cs（脚本生成）；结构见 WinSock2Structs.cs。
//
// ⚠ 本单元为**第三方头文件翻译**（Borland/JEDI，winsock2.h 的 Pascal 化，MPL 1.1）。
//
// 转换策略（任务书 §"WinSock2" 条目："只移植纯逻辑/常量/结构，socket 调用收敛到接缝"）：
//
//   ┌─ 已 1:1 移植（纯逻辑，可单测）─────────────────────────────────────────┐
//   │ · :1522-1550 WSAMakeSyncReply / WSAMakeSelectReply / WSAGetAsyncBuflen │
//   │              / WSAGetAsyncError / WSAGetSelectEvent / WSAGetSelectError │
//   │ · :1552-1589 FD_CLR / FD_ISSET / FD_SET / FD_ZERO（见 WinSock2Structs） │
//   │ · :1215-1221 htonl/htons/ntohl/ntohs（BitConverter 等价实现）           │
//   │ · :1217-1218 inet_addr / inet_ntoa（按原文 TCPIP.Win32 的解析规则）     │
//   │ · :1518 及 IECache:98 依赖的 initializeWinInet 式"库可用性探测"         │
//   └────────────────────────────────────────────────────────────────────────┘
//
//   ┌─ 不移植（收敛到接缝；调用方改用 System.Net.Sockets）────────────────────┐
//   │ 20 个 1.1 阻塞调用 + 60+ 个 WSA* 扩展调用（原文 :1207-1374 是声明，      │
//   │ :1391-1520 是 `external 'ws2_32.dll'` 绑定）。                          │
//   │ 托管侧不再 P/Invoke ws2_32.dll：C# 的 System.Net.Sockets.Socket 内部      │
//   │ 就是 WinSock2 的封装，重复 P/Invoke 只会绕过 .NET 的句柄安全与            │
//   │ SocketAsyncEventArgs 的 IOCP 绑定。                                     │
//   │ 映射表见本文件底部 <see cref="WinSock2Seam.FunctionMap"/>。              │
//   └────────────────────────────────────────────────────────────────────────┘
using System;
using System.Net;
using System.Net.Sockets;

namespace GXX.Client.Tail;

/// <summary>
/// WinSock2.pas implementation 段的**纯逻辑**部分 + socket 调用接缝。
/// <para>所有纯逻辑方法都有 ≥3 个单测（含边界），见 <c>TailWinSock2Tests</c>。</para>
/// </summary>
public static class WinSock2Seam
{
    // ══════════════════════════════════════════════════════════════════════
    // 原文 :1522-1530 —— 异步消息打包
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// 原文 :1522-1525 <c>function WSAMakeSyncReply(BufLen, Error: Word): Longint;</c>
    /// <para>= <c>MakeLong(BufLen, Error)</c>，Windows 原语定义为
    /// <c>(Error shl 16) or BufLen</c>：<b>低位字是 BufLen，高位字是 Error</b>。</para>
    /// </summary>
    public static int WSAMakeSyncReply(ushort BufLen, ushort Error)
        => (int)((uint)BufLen | ((uint)Error << 16));

    /// <summary>
    /// 原文 :1527-1530 <c>function WSAMakeSelectReply(Event, Error: Word): Longint;</c>
    /// <para>= <c>MakeLong(Event, Error)</c>：<b>低位字是 Event，高位字是 Error</b>。</para>
    /// </summary>
    public static int WSAMakeSelectReply(ushort Event, ushort Error)
        => (int)((uint)Event | ((uint)Error << 16));

    /// <summary>原文 :1532-1535 <c>WSAGetAsyncBuflen(Param: Longint): Word</c> = <c>LOWORD(Param)</c></summary>
    public static ushort WSAGetAsyncBuflen(int Param) => (ushort)(Param & 0xFFFF);

    /// <summary>原文 :1537-1540 <c>WSAGetAsyncError(Param: Longint): Word</c> = <c>HIWORD(Param)</c></summary>
    public static ushort WSAGetAsyncError(int Param) => (ushort)((Param >> 16) & 0xFFFF);

    /// <summary>原文 :1542-1545 <c>WSAGetSelectEvent(Param: Longint): Word</c> = <c>LOWORD(Param)</c></summary>
    public static ushort WSAGetSelectEvent(int Param) => (ushort)(Param & 0xFFFF);

    /// <summary>原文 :1547-1550 <c>WSAGetSelectError(Param: Longint): Word</c> = <c>HIWORD(Param)</c></summary>
    public static ushort WSAGetSelectError(int Param) => (ushort)((Param >> 16) & 0xFFFF);

    // ══════════════════════════════════════════════════════════════════════
    // 原文 :1215-1221 —— 字节序转换（ws2_32 的 htonl/htons/ntohl/ntohs）
    // ══════════════════════════════════════════════════════════════════════
    // 托管侧用 IPAddress.HostToNetworkOrder 系数（其 ushort/int/long 重载与
    // WinSock 的 htons/htonl 完全一致：只做字节序翻转，不做数值解释）。

    /// <summary>原文 :1215 <c>htonl(hostlong: u_long): u_long</c></summary>
    public static uint htonl(uint hostlong) => (uint)IPAddress.HostToNetworkOrder(unchecked((int)hostlong));

    /// <summary>原文 :1216 <c>htons(hostshort: u_short): u_short</c></summary>
    public static ushort htons(ushort hostshort) => (ushort)IPAddress.HostToNetworkOrder(unchecked((short)hostshort));

    /// <summary>原文 :1220 <c>ntohl(netlong: u_long): u_long</c>（与 htonl 同一操作，对称）</summary>
    public static uint ntohl(uint netlong) => (uint)IPAddress.NetworkToHostOrder(unchecked((int)netlong));

    /// <summary>原文 :1221 <c>ntohs(netshort: u_short): u_short</c>（与 htons 同一操作，对称）</summary>
    public static ushort ntohs(ushort netshort) => (ushort)IPAddress.NetworkToHostOrder(unchecked((short)netshort));

    // ══════════════════════════════════════════════════════════════════════
    // 原文 :1217-1218 —— inet_addr / inet_ntoa
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// 原文 :1217 <c>function inet_addr(cp: PChar): u_long;</c>
    /// <para>把点分十进制 IPv4 串转成**网络字节序** 32 位整数；非法输入返回
    /// <c>INADDR_NONE</c>（= <c>$FFFFFFFF</c>，见 <see cref="WinSock2Constants.INADDR_NONE"/>）。</para>
    /// <para>客户端唯一使用点：<c>UpdateEngine.pas:211</c>
    /// <c>Result := inet_addr(PAnsiChar(sIPaddr)) &lt;&gt; INADDR_NONE;</c>
    /// —— 只把它当"这是个合法 IP 字面量吗"的判据，所以返回值本身只需保证
    /// 合法/非法两态正确。</para>
    /// <para><b>与 <c>IPAddress.Parse</c> 的差异</b>（因此不使用它）：WinSock 的
    /// <c>inet_addr</c> **拒绝**空白、拒绝 &lt;4 段、拒绝 &gt;255 的段、接受前导零
    /// （按十进制而非八进制解释），且**不抛异常**；解析失败统一给
    /// <c>INADDR_NONE</c>。<c>IPAddress.Parse</c> 会抛、且对前导零的行为不同。</para>
    /// </summary>
    /// <param name="cp">原文的 PChar；null 等价于原文空串指针（返回 INADDR_NONE）。</param>
    public static uint inet_addr(string cp)
    {
        // 原文对空指针/空串：ws2_32 的 inet_addr("") 返回 INADDR_NONE。
        if (string.IsNullOrEmpty(cp)) return WinSock2Constants.INADDR_NONE;

        string[] parts = cp.Split('.');
        if (parts.Length != 4) return WinSock2Constants.INADDR_NONE;

        var octets = new byte[4];
        for (int i = 0; i < 4; i++)
        {
            string p = parts[i];
            if (p.Length == 0) return WinSock2Constants.INADDR_NONE;
            // 只允许 ASCII 数字（原文逐字符接受 '0'..'9'；含 +/-/空白一律非法）
            foreach (char c in p)
            {
                if (c < '0' || c > '9') return WinSock2Constants.INADDR_NONE;
            }
            // 段长 >3 位必然越界（例如 "0000"）；先挡掉避免溢出
            if (p.Length > 3) return WinSock2Constants.INADDR_NONE;
            int v = int.Parse(p);
            if (v > 255) return WinSock2Constants.INADDR_NONE;
            octets[i] = (byte)v;
        }

        // 与 inet_addr 一致：结果按**网络字节序**放回 u_long。
        // x86 小端下，网络序 a.b.c.d 的 u_long 是 d<<24 | c<<16 | b<<8 | a
        // （即把 a 放最低字节），这样 SendClientMessage 里的字面量才与原文一致。
        return (uint)(octets[0] | (octets[1] << 8) | (octets[2] << 16) | (octets[3] << 24));
    }

    /// <summary>
    /// 原文 :1218 <c>function inet_ntoa(inaddr: TInAddr): PChar;</c>
    /// <para>把网络字节序地址格式化成点分十进制串。<c>inet_ntoa</c> 是**非线程安全**的
    /// （返回内部静态缓冲），托管侧改为返回值语义（差异已登记）。</para>
    /// </summary>
    public static string inet_ntoa(uint inaddr)
        => string.Concat(
            (inaddr & 0xFF).ToString(), ".",
            ((inaddr >> 8) & 0xFF).ToString(), ".",
            ((inaddr >> 16) & 0xFF).ToString(), ".",
            ((inaddr >> 24) & 0xFF).ToString());

    /// <summary>原文 :1218 的强类型重载（原文参数是 <c>TInAddr</c> 记录）。</summary>
    public static string inet_ntoa(TInAddr inaddr) => inet_ntoa(inaddr.S_addr);

    // ══════════════════════════════════════════════════════════════════════
    // 原文 :1232 / :1237-1240 —— 库可用性与错误码
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// 原文 :1237 <c>WSAStartup</c> + :1240 <c>WSAGetLastError</c> 的托管对应：
    /// 探测 Winsock 栈是否可用。
    /// <para>原文 <c>IECache.pas:343-377 initializeWinInet</c> 用的是同一套"加载
    /// DLL + 取全部入口，缺一即失败"的探测思路；这里是它的 System.Net.Sockets 版本：
    /// 能构造一个无绑定的 TCP Socket 即视为栈可用。</para>
    /// </summary>
    public static bool initializeWinSock()
    {
        try
        {
            using var s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            return s.AddressFamily == AddressFamily.InterNetwork;
        }
        catch (SocketException)
        {
            return false;
        }
    }

    /// <summary>
    /// 原文 :1240 <c>WSAGetLastError: Integer</c> 的托管对应。
    /// <para>托管侧没有"线程最后 WinSock 错误"这一全局状态；<c>SocketException.ErrorCode</c>
    /// 才是等价值。本接缝把它归一化成调用方期望的形态。</para>
    /// </summary>
    public static int WSAGetLastError(Exception ex)
    {
        if (ex is SocketException se) return se.ErrorCode;
        if (ex is System.Net.Sockets.SocketException) return ((SocketException)ex).ErrorCode;
        return 0;
    }

    /// <summary>
    /// 原文 :1240 的"当前线程最后错误"入口（**接缝**）。
    /// <para>源码里唯一的真实使用点是 <c>ClMain.pas:16175</c> 的
    /// <c>LastError := WSAGetLastError;</c>（紧跟在一次 <c>recv</c> 之后）。
    /// 托管侧必须由调用方把捕获到的异常传进来，因此本方法签名与原文不同
    /// —— 差异已登记。</para>
    /// </summary>
    public static int WSAGetLastError()
        => throw new NotSupportedException(
            "原文 WinSock2.pas:1240 WSAGetLastError 读取线程局部的 WinSock 错误码。" +
            "托管侧无此全局状态：请在 catch 块里用 WinSock2Seam.WSAGetLastError(ex) 取 SocketException.ErrorCode。" +
            "（接缝：待 ClMain 的 socket 收发路径移植到 System.Net.Sockets 后接入）");

    // ══════════════════════════════════════════════════════════════════════
    // 未移植函数的映射表（文档化，供审计）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// 原文 <c>external 'ws2_32.dll'</c> 函数 → 托管等价物。
    /// <para>每行 <c>(原文函数, 原文行号, 托管等价的接缝/类型, 备注)</c>，按原文声明顺序排列。
    /// <b>本表是文档而非代码</b>：托管侧不 P/Invoke ws2_32.dll。</para>
    /// </summary>
    public static readonly (string Delphi, int Line, string Managed, string Note)[] FunctionMap = new[]
    {
        ("accept",        1207, "Socket.Accept / AcceptAsync",      "阻塞→Accept；IOCP→AcceptAsync(SAEA)"),
        ("bind",          1208, "Socket.Bind",                      "同语义；EndPoint 用 IPEndPoint"),
        ("closesocket",   1209, "Socket.Close / Dispose",            "注意：.NET 有句柄安全终结点"),
        ("connect",       1210, "Socket.Connect / ConnectAsync",     ""),
        ("ioctlsocket",   1211, "Socket.IOControl",                  "FIONBIO/FIONREAD 用 Socket.Blocking/Available"),
        ("getpeername",   1212, "Socket.RemoteEndPoint",             ""),
        ("getsockname",   1213, "Socket.LocalEndPoint",              ""),
        ("getsockopt",    1214, "Socket.GetSocketOption",            ""),
        ("htonl",         1215, "WinSock2Seam.htonl",                "已 1:1 移植（IPAddress.HostToNetworkOrder）"),
        ("htons",         1216, "WinSock2Seam.htons",                "已 1:1 移植"),
        ("inet_addr",     1217, "WinSock2Seam.inet_addr",            "已 1:1 移植（不抛异常，非法返回 INADDR_NONE）"),
        ("inet_ntoa",     1218, "WinSock2Seam.inet_ntoa",            "已 1:1 移植（原文返回静态缓冲，托管改为返回值）"),
        ("listen",        1219, "Socket.Listen",                     "backlog 语义一致"),
        ("ntohl",         1220, "WinSock2Seam.ntohl",                "已 1:1 移植"),
        ("ntohs",         1221, "WinSock2Seam.ntohs",                "已 1:1 移植"),
        ("recv",          1222, "Socket.Receive",                    ""),
        ("recvfrom",      1223, "Socket.ReceiveFrom",                ""),
        ("select",        1224, "Socket.Poll",                       "或 SocketAsyncEventArgs"),
        ("send",          1225, "Socket.Send",                       ""),
        ("sendto",        1226, "Socket.SendTo",                     ""),
        ("setsockopt",    1227, "Socket.SetSocketOption",            ""),
        ("shutdown",      1228, "Socket.Shutdown",                   ""),
        ("Socket",        1229, "new Socket(...)",                   "原文大写 Socket（避免与类型同名）"),
        ("gethostbyaddr", 1230, "Dns.GetHostEntry(IPAddress)",       ""),
        ("gethostbyname", 1231, "Dns.GetHostAddresses",              ""),
        ("gethostname",   1232, "Dns.GetHostName",                   ""),
        ("getservbyport", 1233, "（无托管等价）",                     "端口→服务名查询，托管侧无对应 API"),
        ("getservbyname", 1234, "（无托管等价）",                     "服务名→端口查询"),
        ("getprotobynumber", 1235, "（无托管等价）",                   "ProtocolType 枚举替代"),
        ("getprotobyname",   1236, "（无托管等价）",                   "ProtocolType 枚举替代"),
        ("WSAStartup",    1237, "（不需要）",                         ".NET 运行时自动初始化 Winsock"),
        ("WSACleanup",    1238, "（不需要）",                         ""),
        ("WSASetLastError", 1239, "（不需要）",                       "托管侧无全局错误码"),
        ("WSAGetLastError", 1240, "WinSock2Seam.WSAGetLastError(ex)", "读取线程局部错误：必须由调用方把 SocketException 传进来"),
        ("WSAAsyncSelect", 1252, "SocketAsyncEventArgs",              "事件模型→完成端口模型"),
        ("__WSAFDIsSet",  1253, "WinSock2FdSet.FD_ISSET",             "已 1:1 移植（线性查找）"),
        ("WSAAccept",     1256, "Socket.AcceptAsync(SAEA)",           "条件回调 lpfnCondition 无托管等价"),
        ("WSACloseEvent", 1257, "ManualResetEvent.Dispose",           ""),
        ("WSACreateEvent", 1259, "new ManualResetEvent(false)",       ""),
        ("WSAEventSelect", 1270, "SocketAsyncEventArgs",              ""),
        ("WSAIoctl",      1279, "Socket.IOControl",                   ""),
        ("WSARecv",       1288, "Socket.ReceiveAsync(SAEA)",          "IOCP 绑定由 SAEA 承担"),
        ("WSAResetEvent", 1294, "ManualResetEvent.Reset",             ""),
        ("WSASend",       1296, "Socket.SendAsync(SAEA)",             ""),
        ("WSASetEvent",   1302, "ManualResetEvent.Set",               ""),
        ("WSASocket",     1306, "new Socket(...)",                    ""),
        ("WSAWaitForMultipleEvents", 1308, "Task.WhenAny / WaitHandle", ""),
    };

    /// <summary>
    /// 原文 :1391-1520 的 <c>external WINSOCK2_DLL</c> 绑定条数。
    /// <para>由 `_scratch` 的统计脚本从原文计数：<c>Get-Content WinSock2.pas |
    /// Where-Object { $_ -match 'external WINSOCK2_DLL' }</c> ⇒ 121 行。</para>
    /// </summary>
    public const int ExternalBindingCount = 121;
}
