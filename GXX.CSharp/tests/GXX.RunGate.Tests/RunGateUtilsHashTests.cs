using System;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// RunGateUtilsHash.cs 测试 —— 覆盖 RunGateUtils.pas:1867-1979（TRunGate.Run 的挑战应答散列）。
/// <para>
/// 黄金向量由**独立的 PowerShell 复算脚本**产出（Int64 + 显式 mod 2^32 算术，
/// 与 C# 的 uint 回绕是两条不同实现路径），命令与结果见 docs/并行报告-p2-rungate-impl.md。
/// </para>
/// </summary>
public class RunGateUtilsHashTests
{
    [Fact]
    public void Constants_MatchSource()
    {
        // 原 1883-1885 / 1971 / 1974
        Assert.Equal(0x68F4CE4Eu, RunGateChallengeHash.InitA);
        Assert.Equal(0x31A0C38Bu, RunGateChallengeHash.InitB);
        Assert.Equal(0x071C5DEFu, RunGateChallengeHash.InitC);
        Assert.Equal(60000u, RunGateChallengeHash.ResponseTimeStep);
        Assert.Equal(2, RunGateChallengeHash.RequiredResponseCount);
    }

    [Fact]
    public void BuildChallengeBuffer_Layout_IsXorData1OrData2()
    {
        // 原 1870-1871 / 1873-1877：a = d1 xor d2；b = d1 or d2；
        // S = LE32(a) ++ LE32(d1) ++ LE32(b) ++ LE32(d2)
        uint d1 = 0x12345678, d2 = 0x0F0F0F0F;
        var s = RunGateChallengeHash.BuildChallengeBuffer(d1, d2);

        Assert.Equal(16, s.Length);
        Assert.Equal(d1 ^ d2, BitConverter.ToUInt32(s, 0));
        Assert.Equal(d1, BitConverter.ToUInt32(s, 4));
        Assert.Equal(d1 | d2, BitConverter.ToUInt32(s, 8));
        Assert.Equal(d2, BitConverter.ToUInt32(s, 12));
    }

    [Theory]
    // 独立复算：0,0 → 0x724DCB78
    [InlineData(0u, 0u, 0x724DCB78u)]
    // 1,0 → 0xD623D13C
    [InlineData(1u, 0u, 0xD623D13Cu)]
    // 305419896 (0x12345678), 2596069104 (0x9ABCDEF0) → 0x74B38D26
    [InlineData(305419896u, 2596069104u, 0x74B38D26u)]
    // 0xFFFFFFFF, 0xFFFFFFFF → 0xA944C93D
    [InlineData(4294967295u, 4294967295u, 0xA944C93Du)]
    // 3735928559 (0xDEADBEEF), 16909060 (0x01020304) → 0xEA554FFF
    [InlineData(3735928559u, 16909060u, 0xEA554FFFu)]
    public void ComputeChallenge_GoldenVectors(uint data1, uint data2, uint expected)
    {
        Assert.Equal(expected, RunGateChallengeHash.ComputeChallenge(data1, data2));
    }

    [Fact]
    public void Compute_IsDeterministic_AndDependsOnEveryInputByte()
    {
        uint baseline = RunGateChallengeHash.ComputeChallenge(0xAABBCCDDu, 0x11223344u);
        Assert.Equal(baseline, RunGateChallengeHash.ComputeChallenge(0xAABBCCDDu, 0x11223344u));

        // 逐字节翻转 16 字节输入 → 结果必须改变（散列基本质量；也顺带锁住"没有整段被忽略"）
        var s = RunGateChallengeHash.BuildChallengeBuffer(0xAABBCCDDu, 0x11223344u);
        for (int i = 0; i < s.Length; i++)
        {
            var t = (byte[])s.Clone();
            t[i] ^= 0x01;
            Assert.NotEqual(baseline, RunGateChallengeHash.Compute(t));
        }
    }

    [Fact]
    public void Compute_LengthIsPackedAsOriginalLength_NotRemaining()
    {
        // 原 1924：c := c + DWORD(Length(S))
        // 用一个 24 字节输入验证"主循环跑 2 轮后加的是 24 而不是 0"。
        var s = new byte[24];
        for (int i = 0; i < 24; i++) s[i] = (byte)i;
        uint got = RunGateChallengeHash.Compute(s);

        // 独立复算：24 字节、内容 0..23
        uint a = 0x68F4CE4E, b = 0x31A0C38B, c = 0x071C5DEF;
        for (int round = 0; round < 2; round++)
        {
            int k = round * 12;
            a += Le32(s, k);
            b += Le32(s, k + 4);
            c += Le32(s, k + 8);
            Mix(ref a, ref b, ref c);
        }
        c += 24;
        Final(ref a, ref b, ref c);
        Assert.Equal(c, got);
    }

    [Fact]
    public void Compute_NullInput_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => RunGateChallengeHash.Compute(null));
    }

    [Fact]
    public void Compute_EmptyInput_OnlyAppliesLengthAndFinalMix()
    {
        // len = 0：不进主循环、不加任何字节、只 c += 0 再走 Final
        uint got = RunGateChallengeHash.Compute(Array.Empty<byte>());
        uint a = RunGateChallengeHash.InitA, b = RunGateChallengeHash.InitB, c = RunGateChallengeHash.InitC;
        Final(ref a, ref b, ref c);
        Assert.Equal(c, got);
    }

    [Fact]
    public void Compute_Shl12_IsLeftShift_NotRightShift()
    {
        // 原 1957 是 `c := c xor (b shl 12)`，而注释写 `c ^= b >> 15`。
        // 若有人"照注释修"，左右移一字之差会改掉所有输出 → 用黄金向量把这条钉死。
        Assert.Equal(0x724DCB78u, RunGateChallengeHash.ComputeChallenge(0u, 0u));
        Assert.NotEqual(0x724DCB78u ^ 1u, RunGateChallengeHash.ComputeChallenge(0u, 0u));
    }

    // ---- 测试侧独立复算（与产品代码形状不同，仅用于 24 字节用例） ----

    private static uint Le32(byte[] b, int i)
        => (uint)(b[i] | (b[i + 1] << 8) | (b[i + 2] << 16) | (b[i + 3] << 24));

    private static void Mix(ref uint a, ref uint b, ref uint c)
    {
        a -= b; a -= c; a ^= c >> 13;
        b -= c; b -= a; b ^= a << 8;
        c -= a; c -= b; c ^= b >> 15;
        a -= b; a -= c; a ^= c >> 9;
        b -= c; b -= a; b ^= a << 7;
        c -= a; c -= b; c ^= b >> 5;
        a -= b; a -= c; a ^= c >> 3;
        b -= c; b -= a; b ^= a << 10;
        c -= a; c -= b; c ^= b >> 12;
    }

    private static void Final(ref uint a, ref uint b, ref uint c)
    {
        a -= b; a -= c; a ^= c >> 13;
        b -= c; b -= a; b ^= a << 8;
        c -= a; c -= b; c ^= b >> 15;
        a -= b; a -= c; a ^= c >> 11;
        b -= c; b -= a; b ^= a << 16;
        c -= a; c -= b; c ^= b << 12;
        a -= b; a -= c; a ^= c >> 7;
        b -= c; b -= a; b ^= a << 9;
        c -= a; c -= b; c ^= b >> 7;
    }
}
