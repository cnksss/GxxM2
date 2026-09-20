using System;
using GXX.Core.Rtl;

// 源：Source/RunGate/Common/IocpWinsock2.pas（实测 3,450 物理行；审计文档写 3,101，以实际为准）
// ============================================================================
// 对照审计结论（见 docs/并行报告-p2-rungate-impl.md 的映射表）：
//   * 该单元 ~2,950 行是 **WinSock2 API 的类型/常量/external 声明**（fd_set/timeval/hostent/
//     sockaddr_in/WSADATA/WSAPROTOCOL_INFO/WSABUF/WSAIoctl... 以及 200+ 个 `external ws2_32`）。
//     → 完全由 .NET `System.Net.Sockets`（Socket / IPEndPoint / SocketOptionName / SafeSocketHandle）
//       等价覆盖，**不移植第二份**。
//   * 真正有实现体（`implementation` 段 3131-3287）的只有十几个**宏函数**，.NET 没有对应物，
//       因此在本文件 1:1 移植。
// ============================================================================

namespace GXX.RunGate;

/// <summary>fd_set（IocpWinsock2.pas:174-184）。FD_SETSIZE = 64，fd_count: u_int + 64×u_int = 260 字节。</summary>
public struct TFdSet
{
    public uint fd_count;
    public uint[] fd_array;      // 长度固定 FD_SETSIZE

    public static TFdSet Create() => new() { fd_count = 0, fd_array = new uint[IocpWinsock2Compat.FD_SETSIZE] };
}

/// <summary>timeval（IocpWinsock2.pas:206-209）。</summary>
public struct TTimeVal
{
    public int tv_sec;
    public int tv_usec;
}

/// <summary>
/// IocpWinsock2.pas `implementation` 段的宏函数 1:1 移植（纯逻辑）。<br/>
/// （`u_long` 在 ioctl 参数里直接用 <c>uint</c> 表达，故不再单列镜像类型。）
/// </summary>
public static class IocpWinsock2Compat
{
    /// <summary>IocpWinsock2.pas:174 —— FD_SETSIZE。</summary>
    public const int FD_SETSIZE = 64;

    /// <summary>IocpWinsock2.pas:241 —— IOCPARM_MASK。</summary>
    public const uint IOCPARM_MASK = 0x7Fu;

    /// <summary>IocpWinsock2.pas:243-249 —— IOC_* 方向位。</summary>
    public const uint IOC_VOID = 0x20000000u;
    public const uint IOC_OUT = 0x40000000u;
    public const uint IOC_IN = 0x80000000u;
    public const uint IOC_INOUT = IOC_IN | IOC_OUT;

    /// <summary>IocpWinsock2.pas:1902 —— IOC_VENDOR。</summary>
    public const uint IOC_VENDOR = 0x18000000u;

    /// <summary>IocpWinsock2.pas:266 —— FIONBIO = IOC_IN or ((SizeOf(u_long) and $7f) shl 16) or ('f' shl 8) or 126。</summary>
    public static readonly uint FIONBIO = IOC_IN | ((4u & IOCPARM_MASK) << 16) | ((uint)'f' << 8) | 126u;

    /// <summary>IocpCommon.pas:94 —— SIO_KEEPALIVE_VALS = IOC_IN or IOC_VENDOR or 4。</summary>
    public const uint SIO_KEEPALIVE_VALS = IOC_IN | IOC_VENDOR | 4u;

    // ------------------------- fd_set 操作 -------------------------

    /// <summary>
    /// IocpWinsock2.pas:3131-3150 —— FD_CLR。
    /// 语义：找到第一个匹配项 → **整体左移一格**（保序），<c>fd_count--</c>，随后 Break；
    /// 未找到则什么都不做；**被腾空的尾部槽位不清零**（与 Windows 宏一致）。
    /// </summary>
    public static void FD_CLR(uint fd, ref TFdSet fdset)
    {
        EnsureArray(ref fdset);
        uint i = 0;
        while (i < fdset.fd_count)
        {
            if (fdset.fd_array[i] == fd)
            {
                while (i < fdset.fd_count - 1)
                {
                    fdset.fd_array[i] = fdset.fd_array[i + 1];
                    i++;
                }
                fdset.fd_count -= 1;
                break;
            }
            i++;
        }
    }

    /// <summary>
    /// IocpWinsock2.pas:3152-3171 —— <c>_FD_SET</c>（注意函数名带下划线，是作者自己的变体）。
    /// 语义：已在集合里 → 不动；否则在 <c>fd_count &lt; FD_SETSIZE</c> 时追加。
    /// **满 64 个时静默丢弃**（不报错、不扩容）。
    /// </summary>
    public static void _FD_SET(uint fd, ref TFdSet fdset)
    {
        EnsureArray(ref fdset);
        uint i = 0;
        while (i < fdset.fd_count)
        {
            if (fdset.fd_array[i] == fd) break;
            i++;
        }
        if (i == fdset.fd_count)
        {
            if (fdset.fd_count < FD_SETSIZE)
            {
                fdset.fd_array[i] = fd;
                fdset.fd_count += 1;
            }
        }
    }

    /// <summary>IocpWinsock2.pas:3173-3176 —— FD_ZERO。只把计数清零，不清数组内容。</summary>
    public static void FD_ZERO(ref TFdSet fdset)
    {
        EnsureArray(ref fdset);
        fdset.fd_count = 0;
    }

    /// <summary>
    /// IocpWinsock2.pas:3178-3181 —— FD_ISSET。
    /// 原文调 <c>__WSAFDIsSet</c>（ws2_32）；其行为是"线性查找 fd_count 个槽位"，
    /// 因此这里等价实现为线性查找（**不读 fd_array 的越界部分**）。
    /// </summary>
    public static bool FD_ISSET(uint fd, in TFdSet fdset)
    {
        for (uint i = 0; i < fdset.fd_count; i++)
            if (fdset.fd_array[i] == fd) return true;
        return false;
    }

    // ------------------------- timeval 操作 -------------------------

    /// <summary>IocpWinsock2.pas:3183-3186 —— timerisset：任一分量为非 0 即为真（**负值也算设置**）。</summary>
    public static bool timerisset(in TTimeVal tvp) => tvp.tv_sec != 0 || tvp.tv_usec != 0;

    /// <summary>IocpWinsock2.pas:3188-3192 —— timerclear。</summary>
    public static void timerclear(ref TTimeVal tvp)
    {
        tvp.tv_sec = 0;
        tvp.tv_usec = 0;
    }

    // ------------------------- ioctl 编码 -------------------------

    /// <summary>IocpWinsock2.pas:3194-3197 —— _IO：<c>IOC_VOID or (x shl 8) or y</c>。</summary>
    public static uint _IO(uint x, uint y) => IOC_VOID | (x << 8) | y;

    /// <summary>IocpWinsock2.pas:3199-3202 —— _IOR：<c>IOC_OUT or ((t and $7f) shl 16) or (x shl 8) or y</c>。</summary>
    public static uint _IOR(uint x, uint y, uint t) => IOC_OUT | ((t & IOCPARM_MASK) << 16) | (x << 8) | y;

    /// <summary>IocpWinsock2.pas:3204-3207 —— _IOW：方向位换成 IOC_IN，其余同 _IOR。</summary>
    public static uint _IOW(uint x, uint y, uint t) => IOC_IN | ((t & IOCPARM_MASK) << 16) | (x << 8) | y;

    /// <summary>IocpWinsock2.pas:3239-3242 —— _WSAIO：<b>不左移</b>，直接 <c>IOC_VOID or x or y</c>。</summary>
    public static uint _WSAIO(uint x, uint y) => IOC_VOID | x | y;

    /// <summary>IocpWinsock2.pas:3244-3247 —— _WSAIOR。</summary>
    public static uint _WSAIOR(uint x, uint y) => IOC_OUT | x | y;

    /// <summary>IocpWinsock2.pas:3249-3252 —— _WSAIOW。</summary>
    public static uint _WSAIOW(uint x, uint y) => IOC_IN | x | y;

    /// <summary>IocpWinsock2.pas:3254-3257 —— _WSAIORW。</summary>
    public static uint _WSAIORW(uint x, uint y) => IOC_INOUT | x | y;

    // ------------------------- 地址分类 -------------------------

    /// <summary>IocpWinsock2.pas:3209-3212 —— IN_CLASSA：<c>i and $80000000 = 0</c>。</summary>
    public static bool IN_CLASSA(uint i) => (i & 0x80000000u) == 0;

    /// <summary>IocpWinsock2.pas:3214-3217 —— IN_CLASSB：<c>i and $C0000000 = $80000000</c>。</summary>
    public static bool IN_CLASSB(uint i) => (i & 0xC0000000u) == 0x80000000u;

    /// <summary>IocpWinsock2.pas:3219-3222 —— IN_CLASSC：<c>i and $E0000000 = $C0000000</c>。</summary>
    public static bool IN_CLASSC(uint i) => (i & 0xE0000000u) == 0xC0000000u;

    /// <summary>IocpWinsock2.pas:3224-3227 —— IN_CLASSD：<c>i and $F0000000 = $E0000000</c>。</summary>
    public static bool IN_CLASSD(uint i) => (i & 0xF0000000u) == 0xE0000000u;

    /// <summary>IocpWinsock2.pas:3229-3232 —— IN_MULTICAST 直接等于 IN_CLASSD。</summary>
    public static bool IN_MULTICAST(uint i) => IN_CLASSD(i);

    // ------------------------- WSAAsyncSelect 消息打包 -------------------------

    /// <summary>IocpWinsock2.pas:3259-3262 —— WSAMAKEASYNCREPLY = MAKELONG(buflen, error)：buflen 在低 16 位。</summary>
    public static uint WSAMAKEASYNCREPLY(ushort buflen, ushort error) => (uint)DelphiRTL.MakeLong(buflen, error);

    /// <summary>IocpWinsock2.pas:3264-3267 —— WSAMAKESELECTREPLY = MAKELONG(event, error)。</summary>
    public static uint WSAMAKESELECTREPLY(ushort @event, ushort error) => (uint)DelphiRTL.MakeLong(@event, error);

    /// <summary>IocpWinsock2.pas:3269-3272 —— WSAGETASYNCBUFLEN = LOWORD(lParam)。</summary>
    public static ushort WSAGETASYNCBUFLEN(uint lParam) => (ushort)(lParam & 0xFFFF);

    /// <summary>IocpWinsock2.pas:3274-3277 —— WSAGETASYNCERROR = HIWORD(lParam)。</summary>
    public static ushort WSAGETASYNCERROR(uint lParam) => (ushort)(lParam >> 16);

    /// <summary>IocpWinsock2.pas:3279-3282 —— WSAGETSELECTEVENT = LOWORD(lParam)。</summary>
    public static ushort WSAGETSELECTEVENT(uint lParam) => (ushort)(lParam & 0xFFFF);

    /// <summary>IocpWinsock2.pas:3284-3287 —— WSAGETSELECTERROR = HIWORD(lParam)。</summary>
    public static ushort WSAGETSELECTERROR(uint lParam) => (ushort)(lParam >> 16);

    private static void EnsureArray(ref TFdSet fdset)
    {
        if (fdset.fd_array == null || fdset.fd_array.Length != FD_SETSIZE)
            fdset.fd_array = new uint[FD_SETSIZE];
    }
}

/// <summary>
/// IocpCommon.pas:96-103 / 122-151 —— TCP keepalive 结构 + 参数（SIO_KEEPALIVE_VALS）。
/// 原文把 <c>KeepAliveTime</c> 注释成“30 秒”但实际写 <b>5000</b>（毫秒），<c>KeepAliveInterval</c> 写 <b>1</b>；
/// 注释与代码不符，以代码为准。
/// </summary>
public static class IocpTcpKeepAlive
{
    /// <summary>IocpCommon.pas:138 —— KeepAliveTime = 5000 ms（注释写"30秒"是错的）。</summary>
    public const int KeepAliveTimeMs = 5000;

    /// <summary>IocpCommon.pas:141 —— KeepAliveInterval = 1（秒，注释又写"每30秒一次"）。</summary>
    public const int KeepAliveInterval = 1;

    /// <summary>IocpCommon.pas:135 —— OnOff = 1。</summary>
    public const int OnOff = 1;

    /// <summary>IocpCommon.pas:129 —— SO_KEEPALIVE 的 optval = 1。</summary>
    public const int SoKeepAliveOptValue = 1;

    /// <summary>SIO_KEEPALIVE_VALS 的数据布局（TCP_KEEPALIVE）：OnOff/KeepAliveTime/KeepAliveInterval，各 4 字节。</summary>
    public static byte[] BuildKeepAliveBuffer()
    {
        var b = new byte[12];
        BitConverter.GetBytes(OnOff).CopyTo(b, 0);
        BitConverter.GetBytes(KeepAliveTimeMs).CopyTo(b, 4);
        BitConverter.GetBytes(KeepAliveInterval).CopyTo(b, 8);
        return b;
    }
}
