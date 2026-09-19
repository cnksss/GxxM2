// 源单元：Source/Client-HGE/WinSock2.pas（1405 行；本文件覆盖其 `type` 段的 packed record 定义）
// 常量部分见同目录 WinSock2Constants.cs（脚本生成）；socket 函数收敛见 WinSock2Seam.cs。
//
// ⚠ 本单元为**第三方头文件翻译**：Borland/JEDI 对 winsock2.h 的 Pascal 化
//   （JEDI，MPL 1.1；见原文 1-48 行的版权头）。它不是 GXX 业务代码。
//
// 转换策略（任务书 §"WinSock2" 条目）：
//   · 常量与 packed record **逐条 1:1 保留**（布局用 [StructLayout(Pack=1)] 锁死）；
//   · socket *调用* 不再 P/Invoke ws2_32.dll，而是收敛到 WinSock2Seam（System.Net.Sockets）；
//   · 变体记录（`case Integer of`）用 C# 显式布局 + 属性视图表达（见 TInAddr / TSockAddrIn）。
//
// 原文用 `{$ALIGN OFF}`（WinSock2.pas:56）⇒ 本文件所有结构体一律 Pack=1。
using System;
using System.Runtime.InteropServices;

namespace GXX.Client.Tail;

/// <summary>原文 WinSock2.pas:66-69 的基础别名（u_char/u_short/u_int/u_long）。</summary>
public static class WinSock2Aliases
{
    /// <summary>原文 :66 — <c>u_char = Byte;</c></summary>
    public const int u_char_Size = 1;
    /// <summary>原文 :67 — <c>u_short = Word;</c></summary>
    public const int u_short_Size = 2;
    /// <summary>原文 :68 — <c>u_int = DWORD;</c></summary>
    public const int u_int_Size = 4;
    /// <summary>原文 :69 — <c>u_long = DWORD;</c></summary>
    public const int u_long_Size = 4;
    /// <summary>原文 :72 — <c>TSocket = u_int;</c>（哨兵值见 <c>WinSock2Constants.INVALID_SOCKET</c>）</summary>
    public const int TSocket_Size = 4;
}

/// <summary>原文 WinSock2.pas:88-91 — <c>TFDSet</c>（真实布局 = <c>PFDSet</c> 指向的 4 + 4×64 = 260 字节）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TFDSet
{
    /// <summary>原文 :89 — <c>fd_count: u_int;</c></summary>
    public uint fd_count;
    /// <summary>原文 :90 — <c>fd_array: array[0..FD_SETSIZE - 1] of TSocket;</c></summary>
    public uint fd0, fd1, fd2, fd3, fd4, fd5, fd6, fd7;
    /// <summary>fd_array[8..15]</summary>
    public uint fd8, fd9, fd10, fd11, fd12, fd13, fd14, fd15;
    /// <summary>fd_array[16..23]</summary>
    public uint fd16, fd17, fd18, fd19, fd20, fd21, fd22, fd23;
    /// <summary>fd_array[24..31]</summary>
    public uint fd24, fd25, fd26, fd27, fd28, fd29, fd30, fd31;
    /// <summary>fd_array[32..39]</summary>
    public uint fd32, fd33, fd34, fd35, fd36, fd37, fd38, fd39;
    /// <summary>fd_array[40..47]</summary>
    public uint fd40, fd41, fd42, fd43, fd44, fd45, fd46, fd47;
    /// <summary>fd_array[48..55]</summary>
    public uint fd48, fd49, fd50, fd51, fd52, fd53, fd54, fd55;
    /// <summary>fd_array[56..63]</summary>
    public uint fd56, fd57, fd58, fd59, fd60, fd61, fd62, fd63;
}

/// <summary>
/// WinSock2.pas:377-424 的 <c>TFDSet</c> 操作族与 <c>FD_*</c> 宏（原文 :1377-1389 的实现段）。
/// <para>原文是必须按引用操作的 Win32 结构；托管侧改以 <c>uint[]</c>（长度 = <c>FD_SETSIZE</c>）承载，
/// 语义（前 <c>fd_count</c> 个元素有效、<c>FD_SET</c> 满则**静默丢弃**）逐条保留。</para>
/// </summary>
public static class WinSock2FdSet
{
    /// <summary>
    /// 原文 :1552-1570 <c>FD_CLR</c>：命中后**把它后面的元素整体左移一格**（保持顺序），
    /// 再 <c>Dec(fd_count)</c>。
    /// <para><b>差异断言要点</b>：这是**移位**而不是"末元素覆盖"（原文如此；
    /// 与某些 BSD 实现的 swap-with-last 不同，顺序被保留）。</para>
    /// </summary>
    public static void FD_CLR(uint socket, uint[] fdSet, ref uint fdCount)
    {
        if (fdSet == null) return;
        uint i = 0;
        while (i < fdCount)
        {
            if (fdSet[i] == socket)
            {
                // 原文如此（WinSock2.pas:1560-1564）：
                //   while i < FDSet.fd_count - 1 do begin
                //     FDSet.fd_array[i] := FDSet.fd_array[i + 1]; Inc(i); end;
                while (i + 1 < fdCount)
                {
                    fdSet[i] = fdSet[i + 1];
                    i++;
                }
                fdCount--;
                break;
            }
            i++;
        }
    }

    /// <summary>
    /// 原文 :1572-1575 <c>FD_ISSET</c>：实现是转调 <c>__WSAFDIsSet</c>（ws2_32 导出）。
    /// <para>语义与"在前 <c>fd_count</c> 个元素里线性查找"等价，本移植即按此实现，
    /// 不再 P/Invoke（<c>__WSAFDIsSet</c> 只是同一查找的库内实现）。</para>
    /// </summary>
    public static bool FD_ISSET(uint socket, uint[] fdSet, uint fdCount)
    {
        if (fdSet == null) return false;
        for (uint i = 0; i < fdCount; i++)
        {
            if (fdSet[i] == socket) return true;
        }
        return false;
    }

    /// <summary>
    /// 原文 :1577-1585 <c>FD_SET</c>：<c>if FDSet.fd_count &lt; FD_SETSIZE then begin
    /// FDSet.fd_array[FDSet.fd_count] := Socket; Inc(FDSet.fd_count); end;</c>
    /// <para><b>差异断言要点</b>：满了以后**不报错、不扩容**，直接丢弃。</para>
    /// </summary>
    /// <returns>是否真的插入（原文静默丢弃时无返回值，此处补一个便于测试）。</returns>
    public static bool FD_SET(uint socket, uint[] fdSet, ref uint fdCount, int fdSetSize = 64)
    {
        if (fdSet == null) return false;
        if (fdCount < (uint)fdSetSize)
        {
            fdSet[fdCount] = socket;
            fdCount++;
            return true;
        }
        return false;
    }

    /// <summary>原文 :1586-1591 <c>FD_ZERO</c>：<c>ZeroMemory(@FDSet, SizeOf(FDSet));</c></summary>
    public static void FD_ZERO(uint[] fdSet, ref uint fdCount)
    {
        if (fdSet != null) Array.Clear(fdSet, 0, fdSet.Length);
        fdCount = 0;
    }
}

/// <summary>原文 WinSock2.pas:94-97 — <c>TTimeVal</c>（两个 Longint = 8 字节）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TTimeVal
{
    /// <summary>原文 :95 — <c>tv_sec: Longint;</c></summary>
    public int tv_sec;
    /// <summary>原文 :96 — <c>tv_usec: Longint;</c></summary>
    public int tv_usec;
}

/// <summary>原文 WinSock2.pas:379-381 — <c>SunB</c>（4 字节视图）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SunB
{
    /// <summary>原文 :380 — <c>s_b1, s_b2, s_b3, s_b4: u_char;</c></summary>
    public byte s_b1, s_b2, s_b3, s_b4;
}

/// <summary>原文 WinSock2.pas:383-385 — <c>SunW</c>（2×Word 视图）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SunW
{
    /// <summary>原文 :384 — <c>s_w1, s_w2: u_short;</c></summary>
    public ushort s_w1, s_w2;
}

/// <summary>
/// 原文 WinSock2.pas:387-392 — <c>TInAddr</c>（变体记录：字节视图 / 字视图 / 32 位地址同址）。
/// <para>托管侧用显式布局的 4 字节缓冲承载，三种视图以属性暴露（<c>S_un_b</c>/<c>S_un_w</c>/<c>S_addr</c>）。</para>
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TInAddr
{
    /// <summary>原文 :391 — 变体 2 <c>S_addr: u_long</c>（网络字节序，即 x86 内存里的字节序原样存 4 字节）</summary>
    public uint S_addr;

    /// <summary>原文 :389 — 变体 0：<c>S_un_b: SunB</c>（与 <see cref="S_addr"/> 同址）</summary>
    public SunB S_un_b
    {
        get => new SunB
        {
            s_b1 = (byte)(S_addr & 0xFF),
            s_b2 = (byte)((S_addr >> 8) & 0xFF),
            s_b3 = (byte)((S_addr >> 16) & 0xFF),
            s_b4 = (byte)((S_addr >> 24) & 0xFF),
        };
        set => S_addr = (uint)(value.s_b1 | (value.s_b2 << 8) | (value.s_b3 << 16) | (value.s_b4 << 24));
    }

    /// <summary>原文 :390 — 变体 1：<c>S_un_w: SunW</c>（与 <see cref="S_addr"/> 同址）</summary>
    public SunW S_un_w
    {
        get => new SunW { s_w1 = (ushort)(S_addr & 0xFFFF), s_w2 = (ushort)(S_addr >> 16) };
        set => S_addr = (uint)(value.s_w1 | (value.s_w2 << 16));
    }
}

/// <summary>
/// 原文 WinSock2.pas:397-405 — <c>TSockAddrIn</c>（= <c>TSockAddr</c> = <c>SOCKADDR</c> = <c>SOCKADDR_IN</c>）。
/// <para>变体记录两个分支长度都是 16 字节：<c>0:(sin_family;sin_port;sin_addr;sin_zero[8])</c> /
/// <c>1:(sa_family;sa_data[14])</c>。托管侧以显式 16 字节缓冲 + 具名视图表达。</para>
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TSockAddrIn
{
    /// <summary>原文 :399 — <c>sin_family: u_short</c>（= 变体 1 的 <c>sa_family</c>）</summary>
    public ushort sin_family;
    /// <summary>原文 :400 — <c>sin_port: u_short</c>（网络字节序）</summary>
    public ushort sin_port;
    /// <summary>原文 :401 — <c>sin_addr: TInAddr</c></summary>
    public TInAddr sin_addr;
    /// <summary>原文 :402 — <c>sin_zero: array[0..7] of Char</c></summary>
    public ulong sin_zero;

    /// <summary>原文 :403-404 — 变体 1 的 <c>sa_family: u_short</c>（与 <see cref="sin_family"/> 同址）</summary>
    public ushort sa_family
    {
        get => sin_family;
        set => sin_family = value;
    }

    /// <summary>原文 :404 — 变体 1 的 <c>sa_data: array[0..13] of Char</c>（覆盖偏移 2..15）</summary>
    public byte[] sa_data
    {
        get
        {
            var b = new byte[14];
            b[0] = (byte)(sin_port & 0xFF);
            b[1] = (byte)(sin_port >> 8);
            b[2] = (byte)(sin_addr.S_addr & 0xFF);
            b[3] = (byte)((sin_addr.S_addr >> 8) & 0xFF);
            b[4] = (byte)((sin_addr.S_addr >> 16) & 0xFF);
            b[5] = (byte)((sin_addr.S_addr >> 24) & 0xFF);
            for (int i = 0; i < 8; i++) b[6 + i] = (byte)((sin_zero >> (8 * i)) & 0xFF);
            return b;
        }
        set
        {
            var b = value ?? new byte[14];
            sin_port = (ushort)(b[0] | (b[1] << 8));
            sin_addr.S_addr = (uint)(b[2] | (b[3] << 8) | (b[4] << 16) | (b[5] << 24));
            ulong z = 0;
            for (int i = 0; i < 8 && 6 + i < b.Length; i++) z |= (ulong)b[6 + i] << (8 * i);
            sin_zero = z;
        }
    }
}

/// <summary>原文 WinSock2.pas:413-417 — <c>TSockProto</c>。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TSockProto
{
    /// <summary>原文 :415 — <c>sp_family: u_short;</c></summary>
    public ushort sp_family;
    /// <summary>原文 :416 — <c>sp_protocol: u_short;</c></summary>
    public ushort sp_protocol;
}

/// <summary>原文 WinSock2.pas:421-424 — <c>TLinger</c>（setsockopt(SO_LINGER) 的参数）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TLinger
{
    /// <summary>原文 :422 — <c>l_onoff: u_short;</c></summary>
    public ushort l_onoff;
    /// <summary>原文 :423 — <c>l_linger: u_short;</c></summary>
    public ushort l_linger;
}

/// <summary>
/// 原文 WinSock2.pas:668-676 — <c>TWSAData</c>。
/// <para>⚠ 原文用定长 <c>array[0..256] of Char</c> / <c>array[0..128] of Char</c>
/// （注意是 **0..N 共 N+1 字节**，而 <c>WSADESCRIPTION_LEN = 256</c>/<c>WSASYS_STATUS_LEN = 128</c>）。
/// 托管侧用 <see cref="MarshalAsAttribute"/> 的 ByValArray 表达同一个定长布局。</para>
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
public struct TWSAData
{
    /// <summary>原文 :669 — <c>wVersion: Word;</c></summary>
    public ushort wVersion;
    /// <summary>原文 :670 — <c>wHighVersion: Word;</c></summary>
    public ushort wHighVersion;
    /// <summary>原文 :671 — <c>szDescription: array[0..WSADESCRIPTION_LEN] of Char;</c>（257 字节）</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 257)]
    public byte[] szDescription;
    /// <summary>原文 :672 — <c>szSystemStatus: array[0..WSASYS_STATUS_LEN] of Char;</c>（129 字节）</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 129)]
    public byte[] szSystemStatus;
    /// <summary>原文 :673 — <c>iMaxSockets: Word;</c></summary>
    public ushort iMaxSockets;
    /// <summary>原文 :674 — <c>iMaxUdpDg: Word;</c></summary>
    public ushort iMaxUdpDg;
    /// <summary>原文 :675 — <c>lpVendorInfo: PChar;</c></summary>
    public IntPtr lpVendorInfo;
}

/// <summary>原文 WinSock2.pas:693-696 — <c>WSABUF</c>（<c>wsabuf</c>，用于 WSASend/WSARecv 的缓冲描述）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct WSABUF
{
    /// <summary>原文 :694 — <c>len: u_long;</c></summary>
    public uint len;
    /// <summary>原文 :695 — <c>buf: PAnsiChar;</c></summary>
    public IntPtr buf;
}

/// <summary>原文 WinSock2.pas:752-759 — <c>TWSANETWORKEVENTS</c>（WSAEnumNetworkEvents 输出）。</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TWSANETWORKEVENTS
{
    /// <summary>原文 :754 — <c>lNetworkEvents: Longint;</c></summary>
    public int lNetworkEvents;
    /// <summary>原文 :755 — <c>iErrorCode: array[0..FD_MAX_EVENTS - 1] of Integer;</c>（10 项）</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public int[] iErrorCode;
}

/// <summary>原文 WinSock2.pas:1023-1025 — <c>TWSAEComparator</c> 枚举。</summary>
public enum TWSAEComparator
{
    /// <summary>原文 :1023 — <c>COMP_EQUAL {=0}</c></summary>
    COMP_EQUAL = 0,
    /// <summary>原文 :1023 — <c>COMP_NOTLESS</c></summary>
    COMP_NOTLESS = 1,
}

/// <summary>原文 WinSock2.pas:1105 — <c>TWSAeSetServiceOp</c> 枚举（注释里的 =0 只是第一个成员）。</summary>
public enum TWSAeSetServiceOp
{
    /// <summary>原文 :1105 — <c>RNRSERVICE_REGISTER {=0}</c></summary>
    RNRSERVICE_REGISTER = 0,
    /// <summary>原文 :1105 — <c>RNRSERVICE_DEREGISTER</c></summary>
    RNRSERVICE_DEREGISTER = 1,
    /// <summary>原文 :1105 — <c>RNRSERVICE_DELETE</c></summary>
    RNRSERVICE_DELETE = 2,
}

/// <summary>
/// WinSock2.pas `type` 段的布局自检常量（供测试锁死 Marshal.SizeOf）。
/// <para>这些数字是原文 <c>{$ALIGN OFF}</c> 下的真实线格式宽度，改动即破坏 wire 兼容。</para>
/// </summary>
public static class WinSock2Layout
{
    /// <summary><c>SizeOf(TFDSet)</c> = 4 + 64×4 = 260（原文 :88-91）</summary>
    public const int TFDSetSize = 4 + 64 * 4;
    /// <summary><c>SizeOf(TTimeVal)</c> = 4 + 4 = 8（原文 :94-97）</summary>
    public const int TTimeValSize = 8;
    /// <summary><c>SizeOf(TInAddr)</c> = 4（原文 :387-392，三个变体同宽）</summary>
    public const int TInAddrSize = 4;
    /// <summary><c>SizeOf(TSockAddrIn)</c> = 2 + 2 + 4 + 8 = 16（原文 :397-405）</summary>
    public const int TSockAddrInSize = 16;
    /// <summary><c>SizeOf(TSockProto)</c> = 4（原文 :413-417）</summary>
    public const int TSockProtoSize = 4;
    /// <summary><c>SizeOf(TLinger)</c> = 4（原文 :421-424）</summary>
    public const int TLingerSize = 4;
    /// <summary><c>SizeOf(WSABUF)</c> = 4 + 4 = 8（原文 :693-696，32 位指针）</summary>
    public const int WSABUFSize32 = 8;
}
