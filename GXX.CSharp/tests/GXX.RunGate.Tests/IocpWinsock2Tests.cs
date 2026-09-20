using System;
using GXX.Core.Rtl;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// IocpWinsock2Compat.cs 测试 —— 覆盖 Source/RunGate/Common/IocpWinsock2.pas:3131-3287
/// （implementation 段的全部宏函数）。
/// </summary>
public class IocpWinsock2Tests
{
    private static TFdSet NewSet() => TFdSet.Create();

    // ---------------- fd_set ----------------

    [Fact]
    public void FdSetSize_Is64_AndArrayIs260Bytes()
    {
        Assert.Equal(64, IocpWinsock2Compat.FD_SETSIZE);
        var s = NewSet();
        Assert.Equal(64, s.fd_array.Length);
        Assert.Equal(0u, s.fd_count);
        // 4（fd_count）+ 64*4 = 260
        Assert.Equal(260, 4 + s.fd_array.Length * 4);
    }

    [Fact]
    public void Set_AddsInOrder_AndRejectsDuplicates()
    {
        var s = NewSet();
        IocpWinsock2Compat._FD_SET(10, ref s);
        IocpWinsock2Compat._FD_SET(20, ref s);
        IocpWinsock2Compat._FD_SET(10, ref s);          // 已存在 → 不动
        Assert.Equal(2u, s.fd_count);
        Assert.Equal(10u, s.fd_array[0]);
        Assert.Equal(20u, s.fd_array[1]);
    }

    [Fact]
    public void Set_OverflowBeyond64_IsSilentlyDropped()
    {
        var s = NewSet();
        for (uint i = 0; i < 70; i++) IocpWinsock2Compat._FD_SET(i, ref s);
        Assert.Equal(64u, s.fd_count);
        for (uint i = 0; i < 64; i++) Assert.True(IocpWinsock2Compat.FD_ISSET(i, s));
        for (uint i = 64; i < 70; i++) Assert.False(IocpWinsock2Compat.FD_ISSET(i, s));
    }

    [Fact]
    public void IsSet_RespectsCountNotArrayContent()
    {
        var s = NewSet();
        s.fd_array[0] = 777;                            // 直接写数组但 count = 0
        Assert.False(IocpWinsock2Compat.FD_ISSET(777, s));

        IocpWinsock2Compat._FD_SET(777, ref s);
        Assert.True(IocpWinsock2Compat.FD_ISSET(777, s));
    }

    [Fact]
    public void Zero_OnlyResetsCount_NotArray()
    {
        var s = NewSet();
        IocpWinsock2Compat._FD_SET(5, ref s);
        IocpWinsock2Compat.FD_ZERO(ref s);
        Assert.Equal(0u, s.fd_count);
        Assert.Equal(5u, s.fd_array[0]);                // 原 3173-3176 不清数组
        Assert.False(IocpWinsock2Compat.FD_ISSET(5, s));
    }

    [Fact]
    public void Clr_RemovesAndShiftsLeft_PreservingOrder()
    {
        var s = NewSet();
        foreach (uint v in new uint[] { 1, 2, 3, 4, 5 }) IocpWinsock2Compat._FD_SET(v, ref s);
        IocpWinsock2Compat.FD_CLR(3, ref s);
        Assert.Equal(4u, s.fd_count);
        Assert.Equal(new uint[] { 1, 2, 4, 5 }, s.fd_array[..4]);
        Assert.False(IocpWinsock2Compat.FD_ISSET(3, s));
        Assert.True(IocpWinsock2Compat.FD_ISSET(4, s));
    }

    [Fact]
    public void Clr_NonExistent_IsNoOp()
    {
        var s = NewSet();
        IocpWinsock2Compat._FD_SET(1, ref s);
        IocpWinsock2Compat.FD_CLR(99, ref s);
        Assert.Equal(1u, s.fd_count);
        Assert.True(IocpWinsock2Compat.FD_ISSET(1, s));
    }

    [Fact]
    public void Clr_LastElement_LeavesStaleValueButDecrementedCount()
    {
        var s = NewSet();
        foreach (uint v in new uint[] { 7, 8 }) IocpWinsock2Compat._FD_SET(v, ref s);
        IocpWinsock2Compat.FD_CLR(8, ref s);
        Assert.Equal(1u, s.fd_count);
        Assert.Equal(8u, s.fd_array[1]);                // 腾空槽未清零（与 Windows 宏一致）
        Assert.False(IocpWinsock2Compat.FD_ISSET(8, s));
    }

    [Fact]
    public void Clr_OnEmptySet_IsNoOp()
    {
        var s = NewSet();
        IocpWinsock2Compat.FD_CLR(1, ref s);
        Assert.Equal(0u, s.fd_count);
    }

    // ---------------- timeval ----------------

    [Fact]
    public void TimerIsSet_AnyNonZeroComponent()
    {
        var tv = new TTimeVal { tv_sec = 0, tv_usec = 0 };
        Assert.False(IocpWinsock2Compat.timerisset(tv));
        tv.tv_usec = 1;
        Assert.True(IocpWinsock2Compat.timerisset(tv));
        tv = new TTimeVal { tv_sec = 1, tv_usec = 0 };
        Assert.True(IocpWinsock2Compat.timerisset(tv));
        // 负值也算"已设置"（原 3185 是 <> 0 判定）
        tv = new TTimeVal { tv_sec = -1, tv_usec = 0 };
        Assert.True(IocpWinsock2Compat.timerisset(tv));
    }

    [Fact]
    public void TimerClear_ZeroesBothFields()
    {
        var tv = new TTimeVal { tv_sec = 12, tv_usec = 34 };
        IocpWinsock2Compat.timerclear(ref tv);
        Assert.Equal(0, tv.tv_sec);
        Assert.Equal(0, tv.tv_usec);
        Assert.False(IocpWinsock2Compat.timerisset(tv));
    }

    // ---------------- ioctl 编码 ----------------

    [Fact]
    public void IoctlConstants_MatchSource()
    {
        Assert.Equal(0x7Fu, IocpWinsock2Compat.IOCPARM_MASK);
        Assert.Equal(0x20000000u, IocpWinsock2Compat.IOC_VOID);
        Assert.Equal(0x40000000u, IocpWinsock2Compat.IOC_OUT);
        Assert.Equal(0x80000000u, IocpWinsock2Compat.IOC_IN);
        Assert.Equal(0xC0000000u, IocpWinsock2Compat.IOC_INOUT);
        Assert.Equal(0x18000000u, IocpWinsock2Compat.IOC_VENDOR);
    }

    [Fact]
    public void Fionbio_MatchesFormula_AndWindowsValue()
    {
        // 原 266：IOC_IN or ((SizeOf(u_long) and $7f) shl 16) or (Ord('f') shl 8) or 126
        Assert.Equal(0x8004667Eu, IocpWinsock2Compat.FIONBIO);
        // 标准 Windows FIONBIO = 0x8004667E
        Assert.Equal(0x8004667Eu, IocpWinsock2Compat._IOW((uint)'f', 126u, 4u));
    }

    [Fact]
    public void SioKeepAliveVals_MatchesIocpCommon()
    {
        // IocpCommon.pas:94 —— IOC_IN or IOC_VENDOR or 4 = 0x98000004
        Assert.Equal(0x98000004u, IocpWinsock2Compat.SIO_KEEPALIVE_VALS);
    }

    [Fact]
    public void Io_Ior_Iow_DirectionBits()
    {
        Assert.Equal(IocpWinsock2Compat.IOC_VOID | (uint)('x' << 8) | 7u,
                     IocpWinsock2Compat._IO((uint)'x', 7u));
        Assert.Equal(IocpWinsock2Compat.IOC_OUT | (12u << 16) | (uint)('x' << 8) | 7u,
                     IocpWinsock2Compat._IOR((uint)'x', 7u, 12u));
        Assert.Equal(IocpWinsock2Compat.IOC_IN | (12u << 16) | (uint)('x' << 8) | 7u,
                     IocpWinsock2Compat._IOW((uint)'x', 7u, 12u));
    }

    [Fact]
    public void Ioctl_ParmMaskTruncatesSize()
    {
        // t and $7f：128 → 0；255 → 127
        Assert.Equal(IocpWinsock2Compat.IOC_OUT | (0u << 16) | (uint)('a' << 8) | 1u,
                     IocpWinsock2Compat._IOR((uint)'a', 1u, 128u));
        Assert.Equal(IocpWinsock2Compat.IOC_OUT | (127u << 16) | (uint)('a' << 8) | 1u,
                     IocpWinsock2Compat._IOR((uint)'a', 1u, 255u));
    }

    [Fact]
    public void WsaIo_VariantsDifferFromIo_BecauseNoShift()
    {
        // 原 3239-3257：_WSAIO(x,y) = IOC_VOID or x or y（没有 x shl 8）
        Assert.Equal(IocpWinsock2Compat.IOC_VOID | 0x10u | 0x01u, IocpWinsock2Compat._WSAIO(0x10u, 0x01u));
        Assert.NotEqual(IocpWinsock2Compat._IO(0x10u, 0x01u), IocpWinsock2Compat._WSAIO(0x10u, 0x01u));
        Assert.Equal(IocpWinsock2Compat.IOC_OUT | 0x10u | 0x01u, IocpWinsock2Compat._WSAIOR(0x10u, 0x01u));
        Assert.Equal(IocpWinsock2Compat.IOC_IN | 0x10u | 0x01u, IocpWinsock2Compat._WSAIOW(0x10u, 0x01u));
        Assert.Equal(IocpWinsock2Compat.IOC_INOUT | 0x10u | 0x01u, IocpWinsock2Compat._WSAIORW(0x10u, 0x01u));
    }

    // ---------------- 地址分类 ----------------

    [Fact]
    public void InClass_PredicatesAreMutuallyExclusive()
    {
        // 0.0.0.0 → A 类；128.0.0.0 → B；192.0.0.0 → C；224.0.0.0 → D
        Assert.True(IocpWinsock2Compat.IN_CLASSA(0x00000000u));
        Assert.True(IocpWinsock2Compat.IN_CLASSB(0x80000000u));
        Assert.True(IocpWinsock2Compat.IN_CLASSC(0xC0000000u));
        Assert.True(IocpWinsock2Compat.IN_CLASSD(0xE0000000u));

        Assert.False(IocpWinsock2Compat.IN_CLASSA(0x80000000u));
        Assert.False(IocpWinsock2Compat.IN_CLASSB(0xC0000000u));
        Assert.False(IocpWinsock2Compat.IN_CLASSC(0xE0000000u));
        Assert.False(IocpWinsock2Compat.IN_CLASSD(0xF0000000u));
    }

    [Fact]
    public void InClass_BoundariesWithNetworkOrderAddresses()
    {
        // 注意：这些宏判的是 in_addr.s_addr，即**网络字节序**的 32 位值。
        // 127.0.0.1 网络序 = 0x7F000001（而不是主机序 0x0100007F）。
        Assert.True(IocpWinsock2Compat.IN_CLASSA(0x7F000001u));     // 127.x → A
        Assert.True(IocpWinsock2Compat.IN_CLASSB(0xBFFFFFFFu));     // 191.255.255.255 → B
        Assert.True(IocpWinsock2Compat.IN_CLASSC(0xDFFFFFFFu));     // 223.255.255.255 → C
        Assert.True(IocpWinsock2Compat.IN_CLASSD(0xEFFFFFFFu));     // 239.255.255.255 → D
        Assert.False(IocpWinsock2Compat.IN_CLASSA(0xF0000000u));    // 240.0.0.0 → E（都不是）
        Assert.False(IocpWinsock2Compat.IN_CLASSB(0xF0000000u));
        Assert.False(IocpWinsock2Compat.IN_CLASSC(0xF0000000u));
        Assert.False(IocpWinsock2Compat.IN_CLASSD(0xF0000000u));

        // 差异断言：同一个 127.0.0.1 若误用主机序（0x0100007F）也恰好落在 A 类，
        // 但 191.255.255.255 若误用主机序（0xFFFFFFBF）就会被判成非 B 类 —— 字节序必须用对。
        Assert.True(IocpWinsock2Compat.IN_CLASSB(0xBFFFFFFFu));
        Assert.False(IocpWinsock2Compat.IN_CLASSB(0xFFFFFFBFu));
    }

    [Fact]
    public void InMulticast_IsExactlyInClassD()
    {
        foreach (uint v in new uint[] { 0u, 1u, 0x80000000u, 0xC0000000u, 0xE0000000u, 0xEFFFFFFFu, 0xF0000000u })
            Assert.Equal(IocpWinsock2Compat.IN_CLASSD(v), IocpWinsock2Compat.IN_MULTICAST(v));
    }

    // ---------------- WSAAsyncSelect 消息打包 ----------------

    [Fact]
    public void AsyncReply_RoundTripsThroughGetters()
    {
        uint p = IocpWinsock2Compat.WSAMAKEASYNCREPLY(0x1234, 0x5678);
        Assert.Equal(0x56781234u, p);
        Assert.Equal((ushort)0x1234, IocpWinsock2Compat.WSAGETASYNCBUFLEN(p));
        Assert.Equal((ushort)0x5678, IocpWinsock2Compat.WSAGETASYNCERROR(p));
    }

    [Fact]
    public void SelectReply_RoundTripsThroughGetters()
    {
        uint p = IocpWinsock2Compat.WSAMAKESELECTREPLY(0x00FF, 0xAA00);
        Assert.Equal(0xAA0000FFu, p);
        Assert.Equal((ushort)0x00FF, IocpWinsock2Compat.WSAGETSELECTEVENT(p));
        Assert.Equal((ushort)0xAA00, IocpWinsock2Compat.WSAGETSELECTERROR(p));
    }

    [Fact]
    public void GetAsyncBufLen_EqualsGetSelectEvent_BothLowWord()
    {
        // 两者实现相同（原 3269-3272 与 3279-3282），这是"看起来一样实则一样"的罕见例子 ——
        // 但用途不同：一个配 AsyncGet*，一个配 WSAAsyncSelect。
        uint v = 0xDEADBEEFu;
        Assert.Equal(IocpWinsock2Compat.WSAGETASYNCBUFLEN(v), IocpWinsock2Compat.WSAGETSELECTEVENT(v));
        Assert.Equal(IocpWinsock2Compat.WSAGETASYNCERROR(v), IocpWinsock2Compat.WSAGETSELECTERROR(v));
    }

    // ---------------- keepalive ----------------

    [Fact]
    public void KeepAliveDefaults_MatchCodeNotComments()
    {
        // IocpCommon.pas:138/141 的注释写"30秒/每30秒一次"，代码写的是 5000 / 1
        Assert.Equal(5000, IocpTcpKeepAlive.KeepAliveTimeMs);
        Assert.Equal(1, IocpTcpKeepAlive.KeepAliveInterval);
        Assert.Equal(1, IocpTcpKeepAlive.OnOff);
        Assert.Equal(1, IocpTcpKeepAlive.SoKeepAliveOptValue);
    }

    [Fact]
    public void KeepAliveBuffer_LayoutIsThreeDwords()
    {
        var b = IocpTcpKeepAlive.BuildKeepAliveBuffer();
        Assert.Equal(12, b.Length);
        Assert.Equal(1, BitConverter.ToInt32(b, 0));
        Assert.Equal(5000, BitConverter.ToInt32(b, 4));
        Assert.Equal(1, BitConverter.ToInt32(b, 8));
    }

    [Fact]
    public void MakeLongHelper_UsedByAsyncReply_IsLowHighOrder()
    {
        Assert.Equal(0x56781234, DelphiRTL.MakeLong(0x1234, 0x5678));
    }
}
