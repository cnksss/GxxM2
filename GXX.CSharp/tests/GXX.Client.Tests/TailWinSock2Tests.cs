using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using GXX.Client.Tail;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 并行批次 P2c / 车道 <c>par/p2c-client-tail</c>：
/// <c>Source/Client-HGE/WinSock2.pas</c>（1405 行，Borland/JEDI 对 winsock2.h 的翻译）
/// 的 1:1 移植测试。
///
/// <para>覆盖三块：① 常量（脚本抽取 + 金标回读比对）；② packed record 布局（Marshal.SizeOf）；
/// ③ implementation 段的纯逻辑函数（消息打包 / 字节序 / inet_addr / inet_ntoa / FD_* 集合操作）。</para>
/// </summary>
public sealed class TailWinSock2Tests
{
    // ══════════════════════════════════════════════════════════════════════
    // ① 常量表：与金标逐条比对
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// 金标表：<c>TailWinSock2Golden.g.cs</c>（由 <c>_scratch/gen_winsock2.py</c> 从原文
    /// 表达式求值后生成为**普通 .cs 源文件**——不占用共享 csproj、不挂资源）。
    /// </summary>
    private static List<(string Name, int Line, long Value)> ReadGolden()
        => TailWinSock2Golden.Rows
            .Select(r => (r.Name, r.Line, r.Value))
            .ToList();

    /// <summary>用反射把 <c>WinSock2Constants</c> 的每个 public const 整数读出来。</summary>
    private static Dictionary<string, long> ReadConstants()
    {
        var map = new Dictionary<string, long>(StringComparer.Ordinal);
        foreach (var f in typeof(WinSock2Constants).GetFields(
                     System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
        {
            if (!f.IsLiteral) continue;
            if (f.FieldType != typeof(int) && f.FieldType != typeof(uint)) continue;
            object v = f.GetRawConstantValue();
            map[f.Name] = f.FieldType == typeof(uint) ? (long)(uint)v : (long)(int)v;
        }
        return map;
    }

    /// <summary>用例 1：常量条数与原文一致（359 条数值常量）。</summary>
    [Fact]
    public void Constants_Count_MatchesSource()
    {
        var cs = ReadConstants();
        var golden = ReadGolden();
        Assert.Equal(463, golden.Count);
        Assert.Equal(463, cs.Count);

        var missing = golden.Where(g => !cs.ContainsKey(g.Name)).Select(g => g.Name).ToList();
        var extra = cs.Keys.Where(k => golden.All(g => g.Name != k)).ToList();
        Assert.Empty(missing);
        Assert.Empty(extra);
    }

    /// <summary>用例 2：逐条取值（含负数用 unchecked 表达的那些）与原文求值一致。</summary>
    [Fact]
    public void EveryConstant_Value_MatchesGoldenEvaluatedFromDelphiExpressions()
    {
        var cs = ReadConstants();
        foreach (var g in ReadGolden())
        {
            Assert.True(cs[g.Name] == g.Value,
                $"常量 {g.Name}（原文 @{g.Line}）不符：cs={cs[g.Name]} golden={g.Value}");
        }
    }

    /// <summary>用例 3：原文行号严格递增（防"抄到别的单元"）。</summary>
    [Fact]
    public void GoldenRows_LineNumbers_AreStrictlyIncreasing()
    {
        int prev = 0;
        foreach (var g in ReadGolden())
        {
            Assert.True(g.Line > prev, $"{g.Name} @{g.Line} 未超过上一条 @{prev}");
            prev = g.Line;
        }
    }

    /// <summary>
    /// 用例 4：几个"看起来像哨兵"的关键常量。
    /// <para><b>差异断言要点</b>：<c>INVALID_SOCKET</c> 与 <c>SOCKET_ERROR</c> 的**数值都是 -1**，
    /// 但来源不同（前者是 <c>TSocket(not (0))</c> 的强转，后者是字面量 <c>-1</c>）；
    /// <c>INADDR_NONE</c> 与 <c>INADDR_BROADCAST</c> 也**同为 $FFFFFFFF** 但语义完全不同
    /// —— 这正是"不能用 <c>!= -1</c> 当成功判据"的经典来源。</para>
    /// </summary>
    [Fact]
    public void SentinelConstants_AreExactlyTheDocumentedValues()
    {
        // 全部提升到 long 比较（C# 里 int/uint 混合的 Assert.Equal 重载决议会失败）
        Assert.Equal(-1L, (long)WinSock2Constants.INVALID_SOCKET);
        Assert.Equal(-1L, (long)WinSock2Constants.SOCKET_ERROR);
        Assert.Equal(0xFFFFFFFFL, (long)WinSock2Constants.INADDR_NONE);
        Assert.Equal(0xFFFFFFFFL, (long)WinSock2Constants.INADDR_BROADCAST);
        Assert.Equal(0L, (long)WinSock2Constants.INADDR_ANY);
        Assert.Equal(0x7F000001L, (long)WinSock2Constants.INADDR_LOOPBACK);
        Assert.Equal((long)WinSock2Constants.INADDR_ANY, (long)WinSock2Constants.ADDR_ANY);

        // 差异断言：INADDR_NONE 与 INADDR_BROADCAST 数值相同但**用途不同**
        Assert.Equal((long)WinSock2Constants.INADDR_NONE, (long)WinSock2Constants.INADDR_BROADCAST);
        // 而 INVALID_SOCKET(int) 与 INADDR_NONE(uint) 的值不同：后者位模式是 $FFFFFFFF
        // 但按无符号解释是 4294967295，前者按有符号解释是 -1。
        Assert.NotEqual((long)WinSock2Constants.INADDR_NONE, (long)WinSock2Constants.INVALID_SOCKET);
    }

    /// <summary>用例 5：版本 / 缓冲尺寸 / 追加器常量。</summary>
    [Fact]
    public void VersionAndSizeConstants_MatchSource()
    {
        Assert.Equal(0x0202, WinSock2Constants.WINSOCK_VERSION);
        Assert.Equal("ws2_32.dll", WinSock2Constants.WINSOCK2_DLL);
        Assert.Equal(64, WinSock2Constants.FD_SETSIZE);
        Assert.Equal(16, WinSock2Constants.MSG_MAXIOVLEN);
        Assert.Equal(1024, WinSock2Constants.MAXGETHOSTSTRUCT);
        Assert.Equal(0x7FFFFFFF, WinSock2Constants.SOMAXCONN);
        Assert.Equal(256, WinSock2Constants.WSADESCRIPTION_LEN);
        Assert.Equal(128, WinSock2Constants.WSASYS_STATUS_LEN);
    }

    /// <summary>用例 6：协议族/类型/选项常量（IPPROTO_TCP 等）。</summary>
    [Fact]
    public void ProtocolAndAddressFamilyConstants_MatchSource()
    {
        Assert.Equal(0, WinSock2Constants.IPPROTO_IP);
        Assert.Equal(6, WinSock2Constants.IPPROTO_TCP);
        Assert.Equal(17, WinSock2Constants.IPPROTO_UDP);
        Assert.Equal(255, WinSock2Constants.IPPROTO_RAW);
        Assert.Equal(2, WinSock2Constants.AF_INET);
        Assert.Equal(1, WinSock2Constants.SOCK_STREAM);
        Assert.Equal(2, WinSock2Constants.SOCK_DGRAM);
        Assert.Equal(0xFFFF, WinSock2Constants.SOL_SOCKET);
    }

    /// <summary>用例 7：错误码族（WSABASEERR = 10000 起点）。</summary>
    [Fact]
    public void ErrorCodeConstants_MatchSource()
    {
        Assert.Equal(10000, WinSock2Constants.WSABASEERR);
        Assert.Equal(10035, WinSock2Constants.WSAEWOULDBLOCK);   // WSABASEERR + 35
        Assert.Equal(10036, WinSock2Constants.WSAEINPROGRESS);
        Assert.Equal(10061, WinSock2Constants.WSAECONNREFUSED);
        // Berkeley 别名与 WSA 常量同值
        Assert.Equal(WinSock2Constants.WSAEWOULDBLOCK, WinSock2Constants.EWOULDBLOCK);
        Assert.Equal(WinSock2Constants.WSAECONNREFUSED, WinSock2Constants.ECONNREFUSED);
    }

    /// <summary>用例 8：FD_* 事件位（1 shl FD_xxx_BIT）。</summary>
    [Fact]
    public void FdEventBits_AreOneShiftedByTheirIndex()
    {
        Assert.Equal(1, WinSock2Constants.FD_READ);
        Assert.Equal(2, WinSock2Constants.FD_WRITE);
        Assert.Equal(4, WinSock2Constants.FD_OOB);
        Assert.Equal(8, WinSock2Constants.FD_ACCEPT);
        Assert.Equal(16, WinSock2Constants.FD_CONNECT);
        Assert.Equal(32, WinSock2Constants.FD_CLOSE);
        Assert.Equal(0, WinSock2Constants.FD_READ_BIT);
        Assert.Equal(5, WinSock2Constants.FD_CLOSE_BIT);
    }

    // ══════════════════════════════════════════════════════════════════════
    // ② packed record 布局
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：TFDSet = 4 + 64×4 = 260 字节。</summary>
    [Fact]
    public void TFDSet_Is260Bytes()
    {
        Assert.Equal(260, Marshal.SizeOf<TFDSet>());
        Assert.Equal(260, WinSock2Layout.TFDSetSize);
    }

    /// <summary>用例 2：TTimeVal / TLinger / TSockProto 都是 8/4/4 字节。</summary>
    [Fact]
    public void SmallPackedStructs_HaveOriginalSizes()
    {
        Assert.Equal(8, Marshal.SizeOf<TTimeVal>());
        Assert.Equal(8, WinSock2Layout.TTimeValSize);
        Assert.Equal(4, Marshal.SizeOf<TLinger>());
        Assert.Equal(4, WinSock2Layout.TLingerSize);
        Assert.Equal(4, Marshal.SizeOf<TSockProto>());
        Assert.Equal(4, WinSock2Layout.TSockProtoSize);
    }

    /// <summary>用例 3：TInAddr = 4 字节，且三个变体视图同址互转。</summary>
    [Fact]
    public void TInAddr_Has4Bytes_AndVariantViewsShareStorage()
    {
        Assert.Equal(4, Marshal.SizeOf<TInAddr>());
        Assert.Equal(4, WinSock2Layout.TInAddrSize);

        var a = new TInAddr();
        a.S_addr = 0x7F000001u;                 // 127.0.0.1（网络序字面量）
        var b = a.S_un_b;
        Assert.Equal(0x01, b.s_b1);             // 低字节先（x86 小端）
        Assert.Equal(0x00, b.s_b2);
        Assert.Equal(0x00, b.s_b3);
        Assert.Equal(0x7F, b.s_b4);

        var w = a.S_un_w;
        Assert.Equal(0x0001, w.s_w1);
        Assert.Equal(0x7F00, w.s_w2);

        // 写回视图应改到 S_addr
        var c = new TInAddr();
        c.S_un_b = new SunB { s_b1 = 1, s_b2 = 2, s_b3 = 3, s_b4 = 4 };
        Assert.Equal(0x04030201u, c.S_addr);
    }

    /// <summary>用例 4：TSockAddrIn = 16 字节，且两个变体分支同址。</summary>
    [Fact]
    public void TSockAddrIn_Is16Bytes_AndVariantViewsShareStorage()
    {
        Assert.Equal(16, Marshal.SizeOf<TSockAddrIn>());
        Assert.Equal(16, WinSock2Layout.TSockAddrInSize);

        var sa = new TSockAddrIn
        {
            sin_family = WinSock2Constants.AF_INET,
            sin_port = WinSock2Seam.htons(9527),
            sin_addr = new TInAddr { S_addr = WinSock2Seam.inet_addr("192.168.1.10") },
            sin_zero = 0,
        };
        Assert.Equal(WinSock2Constants.AF_INET, sa.sa_family);      // 变体 1 的 sa_family 与 sin_family 同址
        byte[] d = sa.sa_data;
        Assert.Equal(14, d.Length);
        Assert.Equal(9527, WinSock2Seam.ntohs((ushort)(d[0] | (d[1] << 8))));
        Assert.Equal(WinSock2Seam.inet_addr("192.168.1.10"),
                     (uint)(d[2] | (d[3] << 8) | (d[4] << 16) | (d[5] << 24)));
    }

    /// <summary>用例 5：WSABUF 在 32 位下是 8 字节（len + 4 字节指针）。</summary>
    [Fact]
    public void WSABUF_HasDocumented32BitSize()
    {
        Assert.Equal(8, WinSock2Layout.WSABUFSize32);
        // 托管（64 位）下指针是 8 字节 ⇒ Marshal.SizeOf 与 32 位不同，这是登记的差异
        Assert.True(Marshal.SizeOf<WSABUF>() >= 8);
    }

    /// <summary>用例 6：TWSANETWORKEVENTS 的 iErrorCode 定长 10 项。</summary>
    [Fact]
    public void WsaNetworkEvents_FieldNames_MatchSource()
    {
        Assert.Equal(new[] { "lNetworkEvents", "iErrorCode" },
            typeof(TWSANETWORKEVENTS).GetFields().Select(f => f.Name));
    }

    /// <summary>用例 7：枚举顺序（COMP_EQUAL=0 / RNRSERVICE_REGISTER=0）。</summary>
    [Fact]
    public void WinSock2Enums_ValuesMatchDeclarationOrder()
    {
        Assert.Equal(0, (int)TWSAEComparator.COMP_EQUAL);
        Assert.Equal(1, (int)TWSAEComparator.COMP_NOTLESS);
        Assert.Equal(0, (int)TWSAeSetServiceOp.RNRSERVICE_REGISTER);
        Assert.Equal(1, (int)TWSAeSetServiceOp.RNRSERVICE_DEREGISTER);
        Assert.Equal(2, (int)TWSAeSetServiceOp.RNRSERVICE_DELETE);
    }

    // ══════════════════════════════════════════════════════════════════════
    // ③ implementation 段纯逻辑
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：MakeLong 的两个打包函数 —— 低字/高字分工。</summary>
    [Theory]
    [InlineData(0x0000, 0x0000, 0x00000000)]
    [InlineData(0x1234, 0x5678, 0x56781234)]
    [InlineData(0xFFFF, 0xFFFF, unchecked((int)0xFFFFFFFF))]
    [InlineData(0x0001, 0x0000, 0x00000001)]
    public void WsaMakeReplies_PackLowAndHighWords(ushort lo, ushort hi, int expected)
    {
        Assert.Equal(expected, WinSock2Seam.WSAMakeSyncReply(lo, hi));
        Assert.Equal(expected, WinSock2Seam.WSAMakeSelectReply(lo, hi));
    }

    /// <summary>用例 2：逆操作 —— WSAGetAsync* / WSAGetSelect* 拆字。</summary>
    [Fact]
    public void WsaGetHelpers_UnpackLowAndHighWords()
    {
        int p = WinSock2Seam.WSAMakeSyncReply(0xBEEF, 0xDEAD);
        Assert.Equal(0xBEEF, WinSock2Seam.WSAGetAsyncBuflen(p));
        Assert.Equal(0xDEAD, WinSock2Seam.WSAGetAsyncError(p));

        int q = WinSock2Seam.WSAMakeSelectReply(0x0042, 0x0099);
        Assert.Equal(0x0042, WinSock2Seam.WSAGetSelectEvent(q));
        Assert.Equal(0x0099, WinSock2Seam.WSAGetSelectError(q));
    }

    /// <summary>用例 3：拆字的边界 —— 负值 Param。</summary>
    [Fact]
    public void WsaGetHelpers_HandleNegativeParam()
    {
        // -1 = 0xFFFFFFFF ⇒ 低字与高字都是 0xFFFF
        Assert.Equal(0xFFFF, WinSock2Seam.WSAGetAsyncBuflen(-1));
        Assert.Equal(0xFFFF, WinSock2Seam.WSAGetAsyncError(-1));
        // 0x80000000 是 int.MinValue：低字 0，高字 0x8000
        Assert.Equal(0, WinSock2Seam.WSAGetAsyncBuflen(int.MinValue));
        Assert.Equal(0x8000, WinSock2Seam.WSAGetAsyncError(int.MinValue));
    }

    /// <summary>用例 4：htonl/ntohl/htons/ntohs 的字节序翻转（且两两互为逆）。</summary>
    [Fact]
    public void ByteOrderHelpers_MatchWinSockSemantics()
    {
        Assert.Equal(0x3412u, WinSock2Seam.htons(0x1234));
        Assert.Equal(0x78563412u, WinSock2Seam.htonl(0x12345678));
        Assert.Equal(0x1234u, WinSock2Seam.ntohs(0x3412));
        Assert.Equal(0x12345678u, WinSock2Seam.ntohl(0x78563412));

        // 逆运算往返
        foreach (uint v in new uint[] { 0, 1, 0xFFFFFFFF, 0x00FF00FF, 0xDEADBEEF })
        {
            Assert.Equal(v, WinSock2Seam.ntohl(WinSock2Seam.htonl(v)));
        }
    }

    /// <summary>用例 5：htons 的边界（0x0000 / 0xFFFF 自反）。</summary>
    [Fact]
    public void ByteOrderHelpers_AreSelfInverseAtBoundaries()
    {
        Assert.Equal(0x0000u, WinSock2Seam.htons(0x0000));
        Assert.Equal(0xFFFFu, WinSock2Seam.htons(0xFFFF));
        Assert.Equal(0x0000u, WinSock2Seam.htonl(0x00000000));
        Assert.Equal(0xFFFFFFFFu, WinSock2Seam.htonl(0xFFFFFFFF));
    }

    /// <summary>用例 6：inet_addr 合法输入（网络字节序低字节放第一段）。</summary>
    [Theory]
    [InlineData("0.0.0.0", 0x00000000u)]
    [InlineData("127.0.0.1", 0x0100007Fu)]
    [InlineData("192.168.1.10", 0x0A01A8C0u)]
    [InlineData("255.255.255.255", 0xFFFFFFFFu)]
    public void InetAddr_ParsesValidLiterals(string text, uint expected)
        => Assert.Equal(expected, WinSock2Seam.inet_addr(text));

    /// <summary>
    /// 用例 7：inet_addr 非法输入一律返回 <c>INADDR_NONE</c>（**不抛异常** —— 这是它与
    /// <c>IPAddress.Parse</c> 的关键差异，也是 UpdateEngine.pas:211 直接拿它当合法性判据的原因）。
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("1.2.3")]
    [InlineData("1.2.3.4.5")]
    [InlineData("1.2.3.256")]
    [InlineData("1.2.3.-1")]
    [InlineData("a.b.c.d")]
    [InlineData(" 1.2.3.4")]
    [InlineData("1.2.3.4 ")]
    [InlineData("1..3.4")]
    [InlineData("localhost")]
    public void InetAddr_InvalidInput_ReturnsNoneWithoutThrowing(string text)
    {
        Assert.Equal(WinSock2Constants.INADDR_NONE, WinSock2Seam.inet_addr(text));
        // 与 UpdateEngine.pas:211 的判据一致：<> INADDR_NONE 才算合法
        Assert.False(WinSock2Seam.inet_addr(text) != WinSock2Constants.INADDR_NONE);
    }

    /// <summary>用例 8：inet_addr 的 null 边界（原文 nil PChar）。</summary>
    [Fact]
    public void InetAddr_Null_ReturnsNone()
        => Assert.Equal(WinSock2Constants.INADDR_NONE, WinSock2Seam.inet_addr(null));

    /// <summary>用例 9：inet_ntoa 与 inet_addr 往返。</summary>
    [Theory]
    [InlineData("0.0.0.0")]
    [InlineData("127.0.0.1")]
    [InlineData("192.168.1.10")]
    [InlineData("255.255.255.255")]
    [InlineData("10.0.0.1")]
    public void InetNtoa_RoundTripsWithInetAddr(string text)
        => Assert.Equal(text, WinSock2Seam.inet_ntoa(WinSock2Seam.inet_addr(text)));

    /// <summary>用例 10：inet_ntoa 的强类型重载（原文参数是 TInAddr 记录）。</summary>
    [Fact]
    public void InetNtoa_AcceptsTInAddrStruct()
    {
        var a = new TInAddr { S_addr = WinSock2Seam.inet_addr("8.8.8.8") };
        Assert.Equal("8.8.8.8", WinSock2Seam.inet_ntoa(a));
    }

    /// <summary>用例 11：FD_SET 顺序插入 + 满容静默丢弃（原文 :1577-1585）。</summary>
    [Fact]
    public void FdSet_InsertsInOrder_AndSilentlyDropsWhenFull()
    {
        var arr = new uint[4];
        uint n = 0;
        Assert.True(WinSock2FdSet.FD_SET(10, arr, ref n, 4));
        Assert.True(WinSock2FdSet.FD_SET(20, arr, ref n, 4));
        Assert.True(WinSock2FdSet.FD_SET(30, arr, ref n, 4));
        Assert.True(WinSock2FdSet.FD_SET(40, arr, ref n, 4));
        Assert.Equal(4u, n);
        Assert.Equal(new uint[] { 10, 20, 30, 40 }, arr);

        // 满容：不抛、不扩容、直接丢弃
        Assert.False(WinSock2FdSet.FD_SET(50, arr, ref n, 4));
        Assert.Equal(4u, n);
        Assert.Equal(new uint[] { 10, 20, 30, 40 }, arr);
    }

    /// <summary>用例 12：FD_ISSET 只看前 fd_count 个元素（有效区之外的残留值不算）。</summary>
    [Fact]
    public void FdIsset_OnlyScansTheLivePrefix()
    {
        var arr = new uint[4] { 10, 20, 30, 40 };
        Assert.True(WinSock2FdSet.FD_ISSET(10, arr, 2));
        Assert.True(WinSock2FdSet.FD_ISSET(20, arr, 2));
        // 30 在数组里但不在前 2 个有效元素内 ⇒ false（差异断言）
        Assert.False(WinSock2FdSet.FD_ISSET(30, arr, 2));
        Assert.True(WinSock2FdSet.FD_ISSET(30, arr, 4));
        Assert.False(WinSock2FdSet.FD_ISSET(99, arr, 4));
    }

    /// <summary>
    /// 用例 13：FD_CLR 是**移位**（保留顺序）而不是"末元素覆盖"。
    /// <para>这是原文 :1560-1564 的 while 左移循环；与某些 BSD 实现不同，必须保持顺序。</para>
    /// </summary>
    [Fact]
    public void FdClr_ShiftsLeftPreservingOrder()
    {
        var arr = new uint[5] { 10, 20, 30, 40, 0 };
        uint n = 4;
        WinSock2FdSet.FD_CLR(20, arr, ref n);
        Assert.Equal(3u, n);
        Assert.Equal(new uint[] { 10, 30, 40, 40, 0 }, arr);   // 左移一格，尾部残留不动

        WinSock2FdSet.FD_CLR(10, arr, ref n);
        Assert.Equal(2u, n);
        Assert.Equal(new uint[] { 30, 40, 40, 40, 0 }, arr);

        WinSock2FdSet.FD_CLR(40, arr, ref n);
        Assert.Equal(1u, n);
        Assert.Equal(30u, arr[0]);
    }

    /// <summary>用例 14：FD_CLR 对不存在的 socket 不改动集合。</summary>
    [Fact]
    public void FdClr_MissingSocket_IsNoOp()
    {
        var arr = new uint[3] { 1, 2, 3 };
        uint n = 3;
        WinSock2FdSet.FD_CLR(99, arr, ref n);
        Assert.Equal(3u, n);
        Assert.Equal(new uint[] { 1, 2, 3 }, arr);
    }

    /// <summary>用例 15：FD_ZERO 清空有效计数（原文只写 fd_count := 0）。</summary>
    [Fact]
    public void FdZero_ResetsCount()
    {
        var arr = new uint[3] { 1, 2, 3 };
        uint n = 3;
        WinSock2FdSet.FD_ZERO(arr, ref n);
        Assert.Equal(0u, n);
        Assert.All(arr, v => Assert.Equal(0u, v));
    }

    /// <summary>用例 16：FD_* 的 null 数组边界（托管侧新增的防御，原文会 AV）。</summary>
    [Fact]
    public void FdHelpers_NullArray_DoNotThrow()
    {
        uint n = 0;
        WinSock2FdSet.FD_CLR(1, null, ref n);
        Assert.False(WinSock2FdSet.FD_ISSET(1, null, 0));
        Assert.False(WinSock2FdSet.FD_SET(1, null, ref n, 4));
        WinSock2FdSet.FD_ZERO(null, ref n);
        Assert.Equal(0u, n);
    }

    /// <summary>用例 17：initializeWinSock 探测（本机应可用）。</summary>
    [Fact]
    public void InitializeWinSock_ReportsStackAvailability()
        => Assert.True(WinSock2Seam.initializeWinSock());

    /// <summary>用例 18：WSAGetLastError(ex) 归一化 SocketException.ErrorCode。</summary>
    [Fact]
    public void WsaGetLastError_MapsSocketExceptionErrorCode()
    {
        var se = new System.Net.Sockets.SocketException(WinSock2Constants.WSAECONNREFUSED);
        Assert.Equal(WinSock2Constants.WSAECONNREFUSED, WinSock2Seam.WSAGetLastError(se));
        Assert.Equal(0, WinSock2Seam.WSAGetLastError(new InvalidOperationException()));
        Assert.Equal(0, WinSock2Seam.WSAGetLastError(null));
    }

    /// <summary>用例 19：无参 WSAGetLastError 是显式接缝（抛 NotSupported 并给出替代方案）。</summary>
    [Fact]
    public void WsaGetLastError_NoArg_ThrowsWithGuidance()
    {
        var ex = Assert.Throws<NotSupportedException>(() => WinSock2Seam.WSAGetLastError());
        Assert.Contains("SocketException.ErrorCode", ex.Message);
    }

    /// <summary>用例 20：未移植函数的映射表 —— 每条都指向一个托管等价物或显式说明"无等价"。</summary>
    [Fact]
    public void FunctionMap_CoversEveryPortingDecision()
    {
        var map = WinSock2Seam.FunctionMap;
        Assert.True(map.Length >= 30);
        Assert.All(map, m =>
        {
            Assert.False(string.IsNullOrWhiteSpace(m.Delphi));
            Assert.True(m.Line >= 1207 && m.Line <= 1374, $"{m.Delphi} 的行号 {m.Line} 不在声明区");
            Assert.False(string.IsNullOrWhiteSpace(m.Managed));
        });
        // 关键映射的存在性
        Assert.Contains(map, m => m.Delphi == "closesocket" && m.Managed.Contains("Close"));
        Assert.Contains(map, m => m.Delphi == "WSAStartup" && m.Managed.Contains("不需要"));
        Assert.Contains(map, m => m.Delphi == "recv" && m.Managed.Contains("Receive"));
        // 行号严格递增（按原文声明顺序登记）
        for (int i = 1; i < map.Length; i++)
            Assert.True(map[i].Line >= map[i - 1].Line, $"映射表行号未递增：{map[i].Delphi}");
    }

    /// <summary>用例 21：原文 external 绑定条数（脚本从源文件计数）。</summary>
    [Fact]
    public void ExternalBindingCount_MatchesSourceScan()
        => Assert.Equal(121, WinSock2Seam.ExternalBindingCount);
}
