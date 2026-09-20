// HardInfo.pas 的**纯逻辑**断言（不触任何接缝）。
// 期望值一律回读原文行号手工推导；"看起来一样实则不同"的分支都配了差异断言。
using System;
using GXX.Core.Hardware;
using Xunit;

namespace GXX.Core.Tests;

public class HardwarePureLogicTests
{
    // =========================================================================================
    // ChangeByteOrder —— 原文 :128-142 / :487-501
    // =========================================================================================

    [Fact]
    public void ChangeByteOrder_SwapsPairsInPlace()
    {
        var d = new byte[] { 1, 2, 3, 4 };
        HardInfo.ChangeByteOrder(d, 0, 4);
        Assert.Equal(new byte[] { 2, 1, 4, 3 }, d);
    }

    [Fact]
    public void ChangeByteOrder_OddSizeLeavesLastByteUntouched()
    {
        // 原文 `for i := 0 to (Size shr 1) - 1` ⇒ 3 shr 1 = 1 ⇒ 只交换 1 对
        var d = new byte[] { 1, 2, 3, 4 };
        HardInfo.ChangeByteOrder(d, 0, 3);
        Assert.Equal(new byte[] { 2, 1, 3, 4 }, d);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void ChangeByteOrder_SizeZeroOrOne_NoOp(int size)
    {
        var d = new byte[] { 9, 8, 7, 6 };
        HardInfo.ChangeByteOrder(d, 0, size);
        Assert.Equal(new byte[] { 9, 8, 7, 6 }, d);
    }

    [Fact]
    public void ChangeByteOrder_RespectsOffset()
    {
        var d = new byte[] { 1, 2, 3, 4, 5, 6 };
        HardInfo.ChangeByteOrder(d, 2, 4);
        Assert.Equal(new byte[] { 1, 2, 4, 3, 6, 5 }, d);
    }

    [Fact]
    public void ChangeByteOrder_20ByteField_MatchesIdeSerialNumberCase()
    {
        var d = System.Text.Encoding.ASCII.GetBytes("0123456789ABCDEFGHIJ");
        HardInfo.ChangeByteOrder(d, 0, 20);
        Assert.Equal("1032547698BADCFEHGJI", System.Text.Encoding.ASCII.GetString(d));
    }

    [Fact]
    public void ChangeByteOrder_NegativeSize_ThrowsAtArrayBoundary()
    {
        // ★ 差异登记：Delphi 的 `shr` 是**逻辑**右移 ⇒ (-4) shr 1 = 0x7FFFFFFE 次迭代，
        // 原文会越权读写到 AV；托管侧在第 3 对越界处抛 IndexOutOfRangeException。
        var d = new byte[] { 1, 2, 3, 4, 5, 6 };
        Assert.Throws<IndexOutOfRangeException>(() => HardInfo.ChangeByteOrder(d, 0, -4));
        // 前三对已经换过（原文同样会执行到崩溃前）
        Assert.Equal(new byte[] { 2, 1, 4, 3, 6, 5 }, d);
    }

    [Fact]
    public void ChangeByteOrderLoopCount_IsLogicalShift()
    {
        Assert.Equal(0, HardInfo.ChangeByteOrderLoopCount(0));
        Assert.Equal(0, HardInfo.ChangeByteOrderLoopCount(1));
        Assert.Equal(1, HardInfo.ChangeByteOrderLoopCount(2));
        Assert.Equal(1, HardInfo.ChangeByteOrderLoopCount(3));
        Assert.Equal(2, HardInfo.ChangeByteOrderLoopCount(4));
        Assert.Equal(10, HardInfo.ChangeByteOrderLoopCount(20));
        // ★ 差异断言：负数是"逻辑右移"，不是 0、也不是负数
        Assert.Equal(2147483647L, HardInfo.ChangeByteOrderLoopCount(-1));
        Assert.Equal(1073741824L, HardInfo.ChangeByteOrderLoopCount(int.MinValue));
    }

    // =========================================================================================
    // CountSetBits —— 原文 :233-251
    // =========================================================================================

    [Theory]
    [InlineData(0u, 1u)]   // ★ 0 得 1（不是 0）
    [InlineData(1u, 1u)]
    [InlineData(2u, 0u)]
    [InlineData(3u, 0u)]
    [InlineData(4u, 0u)]
    [InlineData(0x80000000u, 0u)]
    [InlineData(0x7FFFFFFFu, 0u)] // 除 bit0 外全置位 ⇒ 与 0xFFFFFFFE 相与非 0
    [InlineData(0xFFFFFFFFu, 0u)]
    [InlineData(0xFFFFFFFEu, 0u)]
    public void CountSetBits_IsNotAPopcount(uint input, uint expected)
    {
        // ★★ 原文缺陷：bitSetCount 每轮**赋值**（不累加），循环 0..30，末轮掩码 = $FFFFFFFE
        //     ⇒ 结果 = 1 iff input ∈ {0, 1}
        Assert.Equal(expected, HardInfo.CountSetBits(input));
    }

    [Fact]
    public void CountSetBits_DiffersFromRealPopcount()
    {
        // 差异断言：真正的置位数
        Assert.Equal(8, System.Numerics.BitOperations.PopCount(0xFFu));
        Assert.Equal(0u, HardInfo.CountSetBits(0xFFu));   // 原文实现给 0
        Assert.Equal(0, System.Numerics.BitOperations.PopCount(0u));
        Assert.Equal(1u, HardInfo.CountSetBits(0u));      // 原文实现给 1
    }

    // =========================================================================================
    // AssembleCpuName —— 原文 :305-324 的寄存器拼装
    // =========================================================================================

    private static uint Le(string four)
    {
        var b = System.Text.Encoding.ASCII.GetBytes(four);
        return (uint)(b[0] | (b[1] << 8) | (b[2] << 16) | (b[3] << 24));
    }

    [Fact]
    public void AssembleCpuName_GenuineIntel_TruncatesAtEmbeddedNul()
    {
        // 原文 :317-320 的 Move 是**小端原样**拷贝；叶 $80000002 的 EDX 为 0 ⇒ 字符串在下标 12 处截断
        var regs = new[]
        {
            new TRegisters { EAX = Le("Genu"), EBX = Le("ineI"), ECX = Le("ntel"), EDX = 0 },
            default,
            default,
        };
        Assert.Equal("GenuineIntel", HardInfo.AssembleCpuName(regs));
    }

    [Fact]
    public void AssembleCpuName_DoesNotByteSwapRegisters()
    {
        // ★ 差异断言：与 ChangeByteOrder 语义相反 —— EAX 的**低字节**落在下标 0
        var regs = new[]
        {
            new TRegisters { EAX = Le("ABCD"), EBX = Le("EFGH"), ECX = Le("IJKL"), EDX = Le("MNOP") },
            new TRegisters { EAX = Le("QRST"), EBX = Le("UVWX"), ECX = Le("YZ01"), EDX = Le("2345") },
            new TRegisters { EAX = Le("6789"), EBX = Le("abcd"), ECX = Le("efgh"), EDX = Le("ijkl") },
        };
        Assert.Equal("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijkl", HardInfo.AssembleCpuName(regs));
    }

    [Fact]
    public void AssembleCpuName_AllZeroRegisters_IsEmpty()
    {
        Assert.Equal("", HardInfo.AssembleCpuName(new TRegisters[3]));
    }

    [Fact]
    public void AssembleCpuName_Uses48BytesAndTrailingNul()
    {
        // 48 个可见字符 + 下标 48 的 #0 ⇒ 恰好 48 长
        var regs = new[]
        {
            new TRegisters { EAX = Le("0000"), EBX = Le("1111"), ECX = Le("2222"), EDX = Le("3333") },
            new TRegisters { EAX = Le("4444"), EBX = Le("5555"), ECX = Le("6666"), EDX = Le("7777") },
            new TRegisters { EAX = Le("8888"), EBX = Le("9999"), ECX = Le("AAAA"), EDX = Le("BBBB") },
        };
        var s = HardInfo.AssembleCpuName(regs);
        Assert.Equal(48, s.Length);
        Assert.Equal("0000111122223333444455556666777788889999AAAABBBB", s);
    }

    [Fact]
    public void AssembleCpuName_RejectsTooFewLeaves()
    {
        Assert.Throws<ArgumentException>(() => HardInfo.AssembleCpuName(null));
        Assert.Throws<ArgumentException>(() => HardInfo.AssembleCpuName(new TRegisters[2]));
    }

    // =========================================================================================
    // FormatMemorySize —— 原文 :287-303
    // =========================================================================================

    [Theory]
    [InlineData(0L, "0Bytes")]
    [InlineData(1L, "1Bytes")]
    [InlineData(1023L, "1023Bytes")]
    [InlineData(1024L, "1024Bytes")]      // ★ `> 1024` 严格大于 ⇒ 恰好 1KB 仍走 Bytes
    [InlineData(1025L, "1KB")]
    [InlineData(1536L, "2KB")]            // Round(1.5) = 2（半值取偶）
    [InlineData(2560L, "2KB")]            // ★ 差异断言：Round(2.5) = 2（半值取偶），半值上取会得 3
    [InlineData(1048575L, "1024KB")]      // 1023.999… → 1024
    [InlineData(1048576L, "1024KB")]      // ★ 恰好 1MB 仍走 KB
    [InlineData(1048577L, "1MB")]
    [InlineData(1073741823L, "1024MB")]   // 1023.9999… → 1024
    [InlineData(1073741824L, "1024MB")]   // ★ 恰好 1GB 仍走 MB
    [InlineData(1073741825L, "1GB")]
    [InlineData(1610612736L, "2GB")]      // 1.5GiB → 2（偶）
    [InlineData(2684354560L, "2GB")]      // ★ 2.5GiB → 2（偶），非 3
    [InlineData(3758096384L, "4GB")]      // 3.5GiB → 4（偶）
    [InlineData(8589934592L, "8GB")]
    [InlineData(-1L, "-1Bytes")]          // 负数落 else 分支（原文无符号来源不会出现，但语义照抄）
    [InlineData(-2000L, "-2000Bytes")]
    public void FormatMemorySize_BranchBoundaries(long cSize, string expected)
    {
        Assert.Equal(expected, HardInfo.FormatMemorySize(cSize));
    }

    // =========================================================================================
    // FormatCpuIDstringEx —— 原文 :387-394
    // =========================================================================================

    [Fact]
    public void FormatCpuIDstringEx_ConcatenatesWithoutSeparator()
    {
        var si = new TSystemInfo
        {
            dwOemId = 0,
            dwNumberOfProcessors = 8,
            dwProcessorType = 586,
            wProcessorRevision = 9728,
        };
        Assert.Equal("085869728", HardInfo.FormatCpuIDstringEx(in si));
    }

    [Fact]
    public void FormatCpuIDstringEx_IsAmbiguous()
    {
        // ★ 原文缺陷：无分隔符 ⇒ 结果不可逆（两个不同输入得到同一串）
        var a = new TSystemInfo { dwOemId = 1, dwNumberOfProcessors = 23, dwProcessorType = 4, wProcessorRevision = 5 };
        var b = new TSystemInfo { dwOemId = 12, dwNumberOfProcessors = 3, dwProcessorType = 4, wProcessorRevision = 5 };
        Assert.Equal("12345", HardInfo.FormatCpuIDstringEx(in a));
        Assert.Equal("12345", HardInfo.FormatCpuIDstringEx(in b));
        Assert.Equal(HardInfo.FormatCpuIDstringEx(in a), HardInfo.FormatCpuIDstringEx(in b));
    }

    [Fact]
    public void FormatCpuIDstringEx_ZeroIsNotDashOrEmpty()
    {
        var si = new TSystemInfo();
        Assert.Equal("0000", HardInfo.FormatCpuIDstringEx(in si));
    }

    [Fact]
    public void FormatCpuIDstringEx_HighBitDwordIsPrintedUnsigned()
    {
        // ★ 存疑项锁定：Delphi 7 的 IntToStr 有 Integer/Int64 两个重载，DWORD 实参落到哪个无法在本仓库取证。
        //   本移植沿 GXX.Core.Rtl.DelphiRTL.IntToStr(uint)（无符号）。Core 若改判，本断言会红。
        var si = new TSystemInfo { dwOemId = 0x80000000u };
        Assert.Equal("2147483648000", HardInfo.FormatCpuIDstringEx(in si));
    }

    // =========================================================================================
    // FormatWindowsVersion —— 原文 :597-660
    // =========================================================================================

    private static TOSVersionInfo Vi(uint platform, uint major, uint minor, uint build, string csd = "")
    {
        var v = new TOSVersionInfo();
        v.dwPlatformId = platform;
        v.dwMajorVersion = major;
        v.dwMinorVersion = minor;
        v.dwBuildNumber = build;
        v.SetCSDVersion(csd);
        return v;
    }

    /// <summary>构造 TOSVersionInfo 并调用被测格式化（`in` 形参需要实参为变量，故经此包装）。</summary>
    private static string WV(uint platform, uint major, uint minor, uint build, string csd = "")
    {
        var v = Vi(platform, major, minor, build, csd);
        return HardInfo.FormatWindowsVersion(in v);
    }

    [Fact]
    public void FormatWindowsVersion_Nt5_0_2000_WithServicePack()
    {
        Assert.Equal("Windows 2000 Service pack: Service Pack 4, Build: 2195", WV(2, 5, 0, 2195, "Service Pack 4"));
    }

    [Fact]
    public void FormatWindowsVersion_Nt5_1_Whistler()
    {
        Assert.Equal("Windows Whistler, Build: 2600", WV(2, 5, 1, 2600));
    }

    [Fact]
    public void FormatWindowsVersion_Nt5_2_IsUnrecognised_LeavingEmptyBase()
    {
        // ★★ 原文缺陷（:641-644 内层 case 无 else）：major=5 且 minor∉{0,1} ⇒ result 保持初值 ''
        //    最终只剩 ", Build: N"（Windows XP = 5.1 之外的 5.2/5.3 落到这里）
        Assert.Equal(" Service pack: Service Pack 2, Build: 3790", WV(2, 5, 2, 3790, "Service Pack 2"));
        Assert.Equal(", Build: 3790", WV(2, 5, 2, 3790));
    }

    [Fact]
    public void FormatWindowsVersion_Nt6AndAbove_UnknownVersion()
    {
        Assert.Equal("Unknown Version, Build: 7600", WV(2, 6, 1, 7600));
        Assert.Equal("Unknown Version, Build: 19045", WV(2, 10, 0, 19045));
    }

    [Fact]
    public void FormatWindowsVersion_Nt3And4()
    {
        Assert.Equal("Windows NT 3.51, Build: 1057", WV(2, 3, 51, 1057));
        Assert.Equal("Windows NT 4.0, Build: 1381", WV(2, 4, 0, 1381));
    }

    [Fact]
    public void FormatWindowsVersion_ServicePackOnlyAppliesToPlatform2()
    {
        // 原文 :649-651 的 service pack 拼接在 `case 2` 内部（platform 1 即使有 CSDVersion 也不拼）
        Assert.Equal("Windows 98 SE, Build: 2222", WV(1, 4, 10, 2222, " A "));
        Assert.Equal("Windows 2000 Service pack:  A , Build: 2195", WV(2, 5, 0, 2195, " A "));
    }

    [Fact]
    public void FormatWindowsVersion_Win98Se_ChecksSecondByteNotFirst()
    {
        // ★★ 差异断言：原文 :622 的判据是 szCSDVersion[1]（第 2 个字符）
        Assert.Equal("Windows 98 SE, Build: 2222", WV(1, 4, 10, 2222, " A "));
        Assert.Equal("Windows 98, Build: 2222", WV(1, 4, 10, 2222, "A"));
        Assert.Equal("Windows 98, Build: 2222", WV(1, 4, 10, 2222, ""));
    }

    [Fact]
    public void FormatWindowsVersion_Win9xBranches()
    {
        Assert.Equal("Windows 95, Build: 950", WV(1, 4, 0, 950));
        Assert.Equal("Windows Millenium, Build: 3000", WV(1, 4, 90, 3000));
        Assert.Equal("Unknown Version, Build: 1", WV(1, 4, 5, 1));
    }

    [Fact]
    public void FormatWindowsVersion_Platform0AndUnknownPlatform()
    {
        Assert.Equal("Windows 3.11, Build: 0", WV(0, 0, 0, 0));
        Assert.Equal("Unknown Platform, Build: 7", WV(3, 0, 0, 7));
    }

    [Fact]
    public void FormatWindowsVersion_BuildIsTruncatedToLowWord()
    {
        // ★★ 原文缺陷：:658 `IntToStr(Loword(dwBuildNumber))` ⇒ 只取低 16 位
        Assert.Equal("Unknown Version, Build: 5", WV(2, 6, 1, 0x00010005));
        Assert.Equal("Unknown Version, Build: 65535", WV(2, 6, 1, 0xFFFFFFFF));
    }

    // =========================================================================================
    // FormatCPUCount —— 原文 :327-382 的统计段
    // =========================================================================================

    private static SYSTEM_LOGICAL_PROCESSOR_INFORMATION E(TLogicalProcessorRelationship rel, uint mask = 0)
        => new() { Relationship = rel, ProcessorMask = mask };

    [Fact]
    public void FormatCPUCount_EmptyBuffer()
    {
        Assert.Equal("NumaNodes=0 PhysicalProcessorPackages=0 ProcessorCores=0 LogicalProcessors=0",
            HardInfo.FormatCPUCount(Array.Empty<SYSTEM_LOGICAL_PROCESSOR_INFORMATION>(), 0));
    }

    [Fact]
    public void FormatCPUCount_MapsEachRelationshipToItsLabel()
    {
        var buf = new[]
        {
            E(TLogicalProcessorRelationship.RelationProcessorCore),
            E(TLogicalProcessorRelationship.RelationProcessorCore),
            E(TLogicalProcessorRelationship.RelationNumaNode),
            E(TLogicalProcessorRelationship.RelationProcessorPackage),
        };
        // ★ 注意标签序：Numa(1) Package(1) Core(2) Logical(0)
        Assert.Equal("NumaNodes=1 PhysicalProcessorPackages=1 ProcessorCores=2 LogicalProcessors=0",
            HardInfo.FormatCPUCount(buf, 4));
    }

    [Theory]
    [InlineData(0u, 1)]  // ★ 差异断言：mask = 0 也被判为"单逻辑处理器"（CountSetBits(0) = 1）
    [InlineData(1u, 1)]
    [InlineData(2u, 0)]  // ★ 差异断言：mask = 2（2 个逻辑核）反而不计入
    [InlineData(3u, 0)]
    [InlineData(0x80000000u, 0)]
    public void FormatCPUCount_RelationCacheUsesBrokenCountSetBits(uint mask, int expectedLogical)
    {
        var buf = new[] { E(TLogicalProcessorRelationship.RelationCache, mask) };
        Assert.Equal("NumaNodes=0 PhysicalProcessorPackages=0 ProcessorCores=0 LogicalProcessors=" + expectedLogical,
            HardInfo.FormatCPUCount(buf, 1));
    }

    [Fact]
    public void FormatCPUCount_IgnoresOutOfEnumRelationships()
    {
        // ★ 差异断言：原文 case 无 else ⇒ 现代 Windows 的 RelationGroup(4)/Die(5)/NumaNodeEx(6)/Module(7) 全部被丢弃
        var buf = new[]
        {
            E((TLogicalProcessorRelationship)4),
            E((TLogicalProcessorRelationship)5),
            E((TLogicalProcessorRelationship)6),
            E((TLogicalProcessorRelationship)7),
            E((TLogicalProcessorRelationship)0xFFFF),
        };
        Assert.Equal("NumaNodes=0 PhysicalProcessorPackages=0 ProcessorCores=0 LogicalProcessors=0",
            HardInfo.FormatCPUCount(buf, 5));
    }

    [Fact]
    public void FormatCPUCount_OnlyExaminesCountEntries()
    {
        var buf = new[]
        {
            E(TLogicalProcessorRelationship.RelationProcessorCore),
            E(TLogicalProcessorRelationship.RelationProcessorCore),
        };
        Assert.Equal("NumaNodes=0 PhysicalProcessorPackages=0 ProcessorCores=1 LogicalProcessors=0",
            HardInfo.FormatCPUCount(buf, 1));
    }

    // =========================================================================================
    // FormatAdapterAddress6 / FormatNdisAddressHex —— 原文 :699-700、:802-805
    // =========================================================================================

    [Fact]
    public void FormatAdapterAddress6_IsUppercaseWidth2()
    {
        Assert.Equal("001A2B3C4D5E", HardInfo.FormatAdapterAddress6(new byte[] { 0x00, 0x1A, 0x2B, 0x3C, 0x4D, 0x5E }));
        Assert.Equal("000000000000", HardInfo.FormatAdapterAddress6(new byte[6]));
        Assert.Equal("FFFFFFFFFFFF", HardInfo.FormatAdapterAddress6(new byte[] { 255, 255, 255, 255, 255, 255 }));
    }

    [Fact]
    public void FormatAdapterAddress6_ShortArrayThrows()
    {
        // 原文无边界检查（越权读栈）；托管侧抛异常（差异登记）
        Assert.Throws<IndexOutOfRangeException>(() => HardInfo.FormatAdapterAddress6(new byte[5]));
        Assert.Throws<IndexOutOfRangeException>(() => HardInfo.FormatAdapterAddress6(Array.Empty<byte>()));
    }

    [Fact]
    public void FormatNdisAddressHex_UsesBytesReturnedAndIsOneBasedInOriginal()
    {
        var buf = new byte[256];
        buf[0] = 0xDE; buf[1] = 0xAD; buf[2] = 0xBE; buf[3] = 0xEF; buf[4] = 0x00; buf[5] = 0x11;
        Assert.Equal("DEADBEEF0011", HardInfo.FormatNdisAddressHex(buf, 6));
        Assert.Equal("DE", HardInfo.FormatNdisAddressHex(buf, 1));
        Assert.Equal("", HardInfo.FormatNdisAddressHex(buf, 0));
        // ★ 与 FormatAdapterAddress6 的区别：长度由驱动决定（这里是 8 而不是 6）
        Assert.Equal("DEADBEEF00110000", HardInfo.FormatNdisAddressHex(buf, 8));
    }

    [Fact]
    public void FormatNdisAddressHex_OverrunReadsPastBuffer()
    {
        // 原文 :802 `for i := 1 to BytesReturned` 未与 256 比较（原文缺陷）；托管侧抛异常
        Assert.Throws<IndexOutOfRangeException>(() => HardInfo.FormatNdisAddressHex(new byte[256], 257));
    }

    // =========================================================================================
    // FormatMac —— 原文 :815-831
    // =========================================================================================

    [Theory]
    [InlineData("A1B2C3D4E5F6", "A1-B2-C3-D4-E5-F6")]
    [InlineData("a1b2c3d4e5f6", "a1-b2-c3-d4-e5-f6")] // 原文只切分，不做大小写归一
    [InlineData("A1B2C3D4E5F6A1B2", "A1-B2-C3-D4-E5-F6")] // 超过 12 位被丢弃
    [InlineData("A1", "A1-----")]   // ★ 循环恒 6 轮，不足位数仍补足 5 个分隔符
    [InlineData("A", "A-----")]
    [InlineData("", "-----")]
    [InlineData(null, "-----")]
    public void FormatMac_AlwaysRunsSixRounds(string input, string expected)
    {
        Assert.Equal(expected, HardInfo.FormatMac(input));
    }

    // =========================================================================================
    // MatchesPrimaryDisplayName —— 原文 :227
    // =========================================================================================

    [Theory]
    [InlineData("\\\\.\\Display1", true)]
    [InlineData("\\\\.\\DISPLAY1", true)]
    [InlineData("\\\\.\\DISPLAY2", false)]
    [InlineData("\\\\.\\display1", false)]   // ★ Delphi 的字符串 = 是大小写敏感的
    [InlineData("\\\\.\\Display1 ", false)]
    [InlineData("Display1", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void MatchesPrimaryDisplayName_IsCaseSensitiveAndExact(string name, bool expected)
    {
        Assert.Equal(expected, HardInfo.MatchesPrimaryDisplayName(name));
    }

    // =========================================================================================
    // SplitTextStr —— Classes.TStrings.SetTextStr 语义（原文 :759）
    // =========================================================================================

    [Fact]
    public void SplitTextStr_MatchesDelphiSetTextStr()
    {
        Assert.Equal(Array.Empty<string>(), HardInfo.SplitTextStr(""));
        Assert.Equal(Array.Empty<string>(), HardInfo.SplitTextStr(null));
        Assert.Equal(new[] { "A" }, HardInfo.SplitTextStr("A"));
        Assert.Equal(new[] { "A", "B" }, HardInfo.SplitTextStr("A\r\nB"));
        Assert.Equal(new[] { "A" }, HardInfo.SplitTextStr("A\r\n"));       // 结尾换行不产生空行
        Assert.Equal(new[] { "A", "", "B" }, HardInfo.SplitTextStr("A\n\nB"));
        Assert.Equal(new[] { "A", "B" }, HardInfo.SplitTextStr("A\rB"));
        Assert.Equal(new[] { "A", "B" }, HardInfo.SplitTextStr("A\nB"));
        Assert.Equal(new[] { "" }, HardInfo.SplitTextStr("\r\n"));
        Assert.Equal(new[] { "0000", "0001" }, HardInfo.SplitTextStr("0000\r\n0001"));
    }

    // =========================================================================================
    // TCPUID —— 原文 :19（1-based 数组）
    // =========================================================================================

    [Fact]
    public void TCPUID_IsOneBased()
    {
        var t = new TCPUID();
        t[1] = 11; t[2] = 22; t[3] = 33; t[4] = 44;
        Assert.Equal(11, t[1]);
        Assert.Equal(22, t[2]);
        Assert.Equal(33, t[3]);
        Assert.Equal(44, t[4]);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public void TCPUID_OutOfRangeThrows(int index)
    {
        var t = new TCPUID();
        Assert.Throws<IndexOutOfRangeException>(() => { t[index] = 1; });
        Assert.Throws<IndexOutOfRangeException>(() => { _ = t[index]; });
    }
}
