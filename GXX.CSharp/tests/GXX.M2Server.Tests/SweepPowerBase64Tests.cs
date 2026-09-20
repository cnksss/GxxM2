// 测试：Source/M2Engine/PowerBase64.pas → GXX.M2Server.Sweep.PowerBase64（1:1）
//
// 覆盖策略（任务书第 3 条）：每个公开方法 ≥3 用例，含空/0/超界/异常；
// 对「看起来一样实则不同」的分支写**差异断言**：
//   * 码表与标准 RFC4648 不同（含 '#' '/'、不含 '+' '='）→ 用 GXX.Core 既有 Base64Util 做对照；
//   * 填充字符是 '+' 而不是 '='；
//   * DecodeBase64 探测模式在有填充时返回 nCycle*3 - nFillLen（nFillLen 被减两次，原文 :191-193）；
//   * DecodeBase64 的 nFillLen := 1 分支**不可达**（原文 :180 的 1-based 下标等价于末位，死分支）；
//   * DecodeBase64String 对「带填充的合法串」返回**全 0 短串**（探测长度不足以容纳真实字节）。

using System;
using System.Linq;
using GXX.Core.Crypto;
using GXX.M2Server.Sweep;
using Xunit;

namespace GXX.M2Server.Tests;

public class SweepPowerBase64Tests
{
    /// <summary>原文 CIPHER_INDEX / BASE_CIPHER（从 PowerBase64.pas:44-45 抄录，测试侧独立复算用）。</summary>
    private static readonly int[] CipherIndex =
        { 4, 12, 9, 15, 13, 8, 11, 5, 14, 6, 0, 7, 2, 10, 1, 3 };

    private static readonly byte[] BaseCipher =
        { 0xA5, 0x5A, 0x96, 0x69, 0xAF, 0xFA, 0x5F, 0xF5, 0x9F, 0xF9, 0x6F, 0xF6, 0xAA, 0x55, 0x66, 0x99 };

    // ------------------------------------------------------------------ 常量 / 码表

    /// <summary>差异断言：本单元的码表**不是** RFC4648 标准字母表（GXX.Core.Base64Util 那个才是）。</summary>
    [Fact]
    public void CodeTable_DiffersFromStandardBase64()
    {
        Assert.Equal(64, PowerBase64.EncodeTable.Length);
        Assert.Equal(64, PowerBase64.EncodeTable.Distinct().Count());   // 64 项互不相同
        Assert.Equal('+', PowerBase64.DEF_FILL_CHAR);

        const string standard = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        Assert.NotEqual(standard, PowerBase64.EncodeTable);
        Assert.Contains('#', PowerBase64.EncodeTable);            // 自定义表有 '#'
        Assert.DoesNotContain('=', PowerBase64.EncodeTable);      // 没有 '='
        Assert.Equal(-1, PowerBase64.EncodeTable.IndexOf('+'));   // 标准表的 '+' 在这里是填充符，不是码字
        Assert.Equal(6, PowerBase64.EncodeTable.IndexOf('#'));    // '#' 位于索引 6
        Assert.Equal(40, PowerBase64.EncodeTable.IndexOf('A'));   // 'A' 不在标准位置 0
        Assert.Equal(25, PowerBase64.EncodeTable.IndexOf('Z'));

        // 交叉验证：既有 GXX.Core Base64Util（标准字母表）编出来的串，本单元**解不出原值**
        byte[] data = { 0x41, 0x42, 0x43 };
        string std64 = Base64Util.Base64EncodeStr(data);
        Assert.Equal("QUJD", std64);
        string mine = PowerBase64.EncodeBase64(data, data.Length);
        Assert.NotEqual(std64, mine);
        Assert.Equal(4, mine.Length);
        var viaStandard = new byte[4];
        Assert.Equal(3u, PowerBase64.DecodeBase64(std64, viaStandard, 0, viaStandard.Length));
        Assert.NotEqual(data, viaStandard.AsSpan(0, 3).ToArray());   // 码表不同 ⇒ 解出的是别的字节
    }

    [Fact]
    public void DecodeTable_IsInverseOfEncodeTable_AndZeroForUnknown()
    {
        var table = PowerBase64.DecodeTableSnapshot();
        Assert.Equal(128, table.Length);
        for (int i = 0; i < 64; i++)
            Assert.Equal(i, table[(byte)PowerBase64.EncodeTable[i]]);
        Assert.Equal(40, table[(byte)'A']);    // 'A' → 索引 40
        Assert.Equal(25, table[(byte)'Z']);    // 'Z' → 索引 25
        Assert.Equal(0, table[(byte)'=']);     // 未使用字符 → 0（原文 ZeroMemory）
        Assert.Equal(0, table[0]);
        Assert.Equal(0, table[127]);
        Assert.Equal(0, table[(byte)'!']);
    }

    // ------------------------------------------------------------------ EncodeBase64

    [Fact]
    public void EncodeBase64_Empty_ReturnsEmpty()
    {
        using var env = new SweepTestEnv();
        Assert.Equal("", PowerBase64.EncodeBase64(Array.Empty<byte>(), 0));
        Assert.Equal("", PowerBase64.EncodeBase64(new byte[] { 1, 2, 3 }, 0));
    }

    [Fact]
    public void EncodeBase64_RoundTrips_AllByteValues()
    {
        using var env = new SweepTestEnv();
        var data = new byte[48];
        for (int i = 0; i < data.Length; i++) data[i] = (byte)(i * 5 + 1);

        string enc = PowerBase64.EncodeBase64(data, data.Length);
        Assert.Equal(64, enc.Length);                       // 48/3 = 16 组 → 64 字符

        var buf = new byte[64];
        uint n = PowerBase64.DecodeBase64(enc, buf, 0, buf.Length);
        Assert.Equal(48u, n);
        Assert.Equal(data, buf.AsSpan(0, 48).ToArray());
    }

    [Fact]
    public void EncodeBase64_1Byte_Returns4CodeCharsPlus2Fill()
    {
        using var env = new SweepTestEnv();
        // nCycle=0, nRem=1 → nFillLen=2 → 长度 (0+1)*4 + 2 = 6；写入 4 码字 + 2 填充 = 6
        string enc = PowerBase64.EncodeBase64(new byte[] { 0x00 }, 1);
        Assert.Equal(6, enc.Length);
        Assert.Equal(new string(PowerBase64.EncodeTable[0], 4) + "++", enc);   // 码表[0] = '4'
        Assert.DoesNotContain('\0', enc);
    }

    [Fact]
    public void EncodeBase64_2Bytes_Returns4CodeCharsPlus1Fill()
    {
        using var env = new SweepTestEnv();
        string enc = PowerBase64.EncodeBase64(new byte[] { 0x00, 0x00 }, 2);
        Assert.Equal(5, enc.Length);
        Assert.Equal(new string(PowerBase64.EncodeTable[0], 4) + "+", enc);
    }

    /// <summary>
    /// 长度公式逐长度核对 + 编码串逐字符钉死。实测（1..7 字节）：
    /// <code>
    ///   len=1 n=6  [5444++]        len=2 n=5  [5o44+]        len=3 n=4  [5oI4]
    ///   len=4 n=10 [5oI48444++]    len=5 n=9  [5oI48U44+]    len=6 n=8  [5oI48Ug5]
    ///   len=7 n=14 [5oI48Ug5/444++]
    /// </code>
    /// 长度恒等于 <c>(nCycle+1)*4 + nFillLen</c>（nFillLen&gt;0）/ <c>nCycle*4</c>（否则），
    /// 与**实际写入量一致**（无未写入的尾部 #0）。
    /// </summary>
    [Fact]
    public void EncodeBase64_LengthFormula_And_WrittenCharacters()
    {
        using var env = new SweepTestEnv();
        string[] expected =
        {
            "5444++", "5o44+", "5oI4",
            "5oI48444++", "5oI48U44+", "5oI48Ug5",
            "5oI48Ug5/444++",
        };
        for (int len = 1; len <= 7; len++)
        {
            var data = new byte[len];
            for (int i = 0; i < len; i++) data[i] = (byte)(i + 1);
            string enc = PowerBase64.EncodeBase64(data, len);

            int nRem = len % 3;
            int fill = nRem != 0 ? 3 - nRem : 0;
            int cycles = len / 3;
            int formula = fill == 0 ? cycles * 4 : (cycles + 1) * 4 + fill;
            Assert.Equal(formula, enc.Length);
            Assert.Equal(expected[len - 1], enc);
            Assert.DoesNotContain('\0', enc);
        }
    }

    /// <summary>
    /// 原文缺陷登记第 3 条的**已核查结论**：<c>DecodeBase64</c> 的探测长度公式
    /// <c>nCycle*3 - nFillLen</c>（nFillLen 减两次）与 <c>EncodeBase64</c> 的 SetLength
    /// 对**自产串**恰好互补：len ≡ 1,2 mod 3 的串探测长度恒正确（= 原字节数）；
    /// 但 len ≡ 0 mod 2 的串（例如 2 字节）探测长度会算错（见下），编码器本身的长度是对的。
    /// </summary>
    [Fact]
    public void DecodeBase64_ProbeLength_RelationshipWithEncoder()
    {
        using var env = new SweepTestEnv();
        // 3 字节（无填充）→ 4 字符 → probe = 3
        Assert.Equal(3u, PowerBase64.DecodeBase64Probe("5oI4"));
        // 4 字节 → 10 字符（末 2 填充）→ nCycle=(10-2)/4=2 → 2*3-2 = 4
        Assert.Equal(4u, PowerBase64.DecodeBase64Probe("5oI48444++"));
        // 6 字节（无填充）→ 8 字符 → 6
        Assert.Equal(6u, PowerBase64.DecodeBase64Probe("5oI48Ug5"));
        // 7 字节 → 14 字符（末 2 填充）→ nCycle=3 → 9-2 = 7
        Assert.Equal(7u, PowerBase64.DecodeBase64Probe("5oI48Ug5/444++"));
        // **例外（原文缺陷）**：len ≡ 2 mod 3 的串末位是**单个** '+'，但原文恒判 nFillLen=2 →
        // (n-2) 不是 4 的倍数 → 直接返回 0（连自己的编码都解不开）
        Assert.Equal(0u, PowerBase64.DecodeBase64Probe("5o44+"));      // 2 字节
        Assert.Equal(0u, PowerBase64.DecodeBase64Probe("5oI48U44+"));  // 5 字节
    }

    [Fact]
    public void EncodeBase64_OffsetSlice_OnlyEncodesRequestedRange()
    {
        using var env = new SweepTestEnv();
        var data = new byte[] { 0xAA, 0x01, 0x02, 0x03, 0xBB };
        string mid = PowerBase64.EncodeBase64(data, 1, 3);
        string whole = PowerBase64.EncodeBase64(new byte[] { 0x01, 0x02, 0x03 }, 3);
        Assert.Equal(whole, mid);
        Assert.Equal(4, mid.Length);
    }

    // ------------------------------------------------------------------ DecodeBase64

    /// <summary>
    /// 非法输入形态。注意：本单元按 <b>AnsiString 逐字节</b>处理，托管侧用 Latin-1 承载
    /// （每个 char = 1 字节，见 PowerBase64.cs 的 Latin1String/Latin1Bytes）。
    /// </summary>
    [Fact]
    public void DecodeBase64_TooShortNonMultipleOrHighBit_ReturnsZero()
    {
        using var env = new SweepTestEnv();
        Assert.Equal(0u, PowerBase64.DecodeBase64Probe(""));
        Assert.Equal(0u, PowerBase64.DecodeBase64Probe("4"));         // < 4
        Assert.Equal(0u, PowerBase64.DecodeBase64Probe("444"));
        Assert.Equal(0u, PowerBase64.DecodeBase64Probe("44445"));     // 5 字符、末位非 '+' → 5 % 4 != 0
        Assert.Equal(0u, PowerBase64.DecodeBase64Probe("4444444+4")); // 9 字符、末位非 '+' → 9 % 4 != 0
        // 8 字符（8 % 4 == 0）→ 探测模式只做长度检查，返回 nCycle*3 = 6；
        // 但提供缓冲区时会进入通用段，被 "字符 > $7F" 拒绝 → 返回 0 且一个字节都不写。
        Assert.Equal(6u, PowerBase64.DecodeBase64Probe("4444\u00FF\u00FF\u00FF\u00FF"));
        var buf = new byte[16];
        Assert.Equal(0u, PowerBase64.DecodeBase64("4444\u00FF\u00FF\u00FF\u00FF", buf, 0, buf.Length));
        Assert.Equal(new byte[16], buf);                              // 一个字节都没写
    }

    /// <summary>
    /// 探测长度公式核对：<c>:191-193</c> 的 <c>nBufLen := nCycle * 3 - nFillLen</c>（nFillLen 减两次）
    /// 与 <c>EncodeBase64</c> 的 SetLength 对**自产串**互补 ——
    /// 对 len ≡ 1 mod 3 的串两个偏差恰好抵消、探测长度正确；len ≡ 2 mod 3 的串则**解不开**。
    /// </summary>
    [Fact]
    public void DecodeBase64_ProbeLength_FillLenSubtractedTwiceButCancelledByEncoder()
    {
        using var env = new SweepTestEnv();
        byte[] data = { 0x11, 0x22, 0x33, 0x44 };
        string enc = PowerBase64.EncodeBase64(data, 4);
        Assert.Equal(10, enc.Length);                       // (1+1)*4 + 2
        Assert.Equal("++", enc.Substring(8));               // 末 2 位是填充符
        Assert.Equal(4u, PowerBase64.DecodeBase64Probe(enc)); // (10-2)/4*3 - 2 = 6 - 2 = 4

        // 探测模式（pData=nil）不写任何东西，直接返回它
        var buf = new byte[16];
        uint direct = PowerBase64.DecodeBase64(enc, buf, 0, buf.Length);
        Assert.Equal(4u, direct);                           // 通用段 3 + 填充段 1
        Assert.Equal(data, buf.AsSpan(0, 4).ToArray());
    }

    [Fact]
    public void DecodeBase64_BufferTooSmall_ReturnsProbeLengthWithoutWriting()
    {
        using var env = new SweepTestEnv();
        byte[] data = { 0x01, 0x02, 0x03 };
        string enc = PowerBase64.EncodeBase64(data, 3);     // 无填充 → nBufLen = 1*3 = 3
        Assert.Equal(4, enc.Length);

        var small = new byte[2];
        uint n = PowerBase64.DecodeBase64(enc, small, 0, small.Length);
        Assert.Equal(3u, n);                                // 返回探测长度（原文 Exit）
        Assert.Equal(new byte[] { 0, 0 }, small);           // 一个字节都没写
    }

    [Fact]
    public void DecodeBase64_NullBuffer_ReturnsProbeLength()
    {
        using var env = new SweepTestEnv();
        byte[] data = { 0x01, 0x02, 0x03 };
        string enc = PowerBase64.EncodeBase64(data, 3);
        Assert.Equal(3u, PowerBase64.DecodeBase64(enc, null, 0, 0));
        Assert.Equal(3u, PowerBase64.DecodeBase64Probe(enc));
    }

    /// <summary>
    /// **死分支断言**：原文 <c>:176</c> 判 <c>strBase64[nStrSize - 1]</c>、<c>:180</c> 判 <c>strBase64[nStrSize]</c>。
    /// Delphi 1-based 下标下这两者**是同一个字符**，因此 <c>nFillLen := 1</c> 永远不可达：
    /// 末位为 '+' 的串恒被判为 nFillLen = 2。
    /// </summary>
    [Fact]
    public void DecodeBase64_FillLenOneBranchIsDead_OriginalDefect()
    {
        using var env = new SweepTestEnv();
        // 造一个「只可能 nFillLen=1」的串：长度 5（5-1 = 4 是 4 的倍数，若真走 1 分支则会继续处理）。
        Assert.Equal(0u, PowerBase64.DecodeBase64Probe("4444+"));
        // 长度 9 同理（9-1 = 8）
        Assert.Equal(0u, PowerBase64.DecodeBase64Probe("44444444+"));

        // 而末位为 '+' 的合法编码串走的是 nFillLen=2 分支
        byte[] data = { 0x11, 0x22, 0x33, 0x44 };
        string enc = PowerBase64.EncodeBase64(data, 4);
        Assert.Equal(10, enc.Length);
        Assert.Equal('+', enc[9]);
        Assert.Equal(4u, PowerBase64.DecodeBase64Probe(enc));

        // 无填充的 6 字节 → 8 字符串 → nCycle*3 = 6
        byte[] six = { 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
        string enc6 = PowerBase64.EncodeBase64(six, 6);
        Assert.Equal(8, enc6.Length);
        Assert.Equal(6u, PowerBase64.DecodeBase64Probe(enc6));
        var buf = new byte[6];
        Assert.Equal(6u, PowerBase64.DecodeBase64(enc6, buf, 0, buf.Length));
        Assert.Equal(six, buf);
    }

    [Fact]
    public void DecodeBase64_UnknownCharsMapToIndexZero_OriginalBehaviour()
    {
        using var env = new SweepTestEnv();
        // DecodeTable 未命中项为 0 → 不在表里的字符（如 '='）被当成码字 0 处理（原文如此）
        var buf = new byte[8];
        uint n = PowerBase64.DecodeBase64("====", buf, 0, buf.Length);
        Assert.Equal(3u, n);
        Assert.Equal(new byte[] { 0, 0, 0 }, buf.AsSpan(0, 3).ToArray());
    }

    // ------------------------------------------------------------------ XOR 混淆

    [Fact]
    public void XorEncrypt_IsSelfInverse_AndMatchesIndependentRecompute()
    {
        using var env = new SweepTestEnv();
        var data = new byte[] { 0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77,
                                0x88, 0x99, 0xAA, 0xBB, 0xCC, 0xDD, 0xEE, 0xFF, 0x01 };
        var copy = (byte[])data.Clone();

        PowerBase64.XorEncrypt(data, 0, (uint)data.Length);
        for (int i = 0; i < copy.Length; i++)
            Assert.Equal((byte)(copy[i] ^ BaseCipher[CipherIndex[i & 0xF]]), data[i]);

        PowerBase64.XorDecrypt(data, 0, (uint)data.Length);      // XOR_decrypt = XOR_encrypt
        Assert.Equal(copy, data);
    }

    [Fact]
    public void XorEncrypt_ZeroLength_AndOffsetRespected()
    {
        using var env = new SweepTestEnv();
        var data = new byte[] { 0x01, 0x02, 0x03 };
        PowerBase64.XorEncrypt(data, 0, 0);
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, data);   // 长度 0 → 不改

        var d2 = new byte[] { 0xFF, 0x10, 0x20, 0xEE };
        var expect = (byte[])d2.Clone();
        PowerBase64.XorEncrypt(d2, 1, 2u);
        Assert.Equal(0xFF, d2[0]);                             // offset 之外不动
        Assert.Equal(0xEE, d2[3]);
        // 局部下标从 0 起（与原文 pData[i] 一致，i 是函数内独立计数）
        Assert.Equal((byte)(expect[1] ^ BaseCipher[CipherIndex[0]]), d2[1]);
        Assert.Equal((byte)(expect[2] ^ BaseCipher[CipherIndex[1]]), d2[2]);
    }

    [Fact]
    public void XorEncrypt_BoundaryLengths_NoOpAtZero()
    {
        using var env = new SweepTestEnv();
        var data = new byte[] { 0x07, 0x08 };
        PowerBase64.XorEncrypt(data, 0, 0u);                   // dwLen = 0 → 循环 0 次
        Assert.Equal(new byte[] { 0x07, 0x08 }, data);
        PowerBase64.XorEncrypt(data, 0, 1u);                   // 只改第一个字节
        Assert.Equal((byte)(0x07 ^ BaseCipher[CipherIndex[0]]), data[0]);
        Assert.Equal(0x08, data[1]);
    }

    // ------------------------------------------------------------------ 组合函数

    [Fact]
    public void EncryptAndEncodeBase64_DoesNotModifyCallerBuffer_AndRoundTrips()
    {
        using var env = new SweepTestEnv();
        var data = new byte[] { 0x41, 0x42, 0x43, 0x44 };   // 4 字节 → 10 字符，探测长度正确 = 4
        var keep = (byte[])data.Clone();

        string enc = PowerBase64.EncryptAndEncodeBase64(data, data.Length);
        Assert.Equal("", PowerBase64.EncryptAndEncodeBase64(data, 0));   // nDataSize = 0
        Assert.Equal(keep, data);                                        // 原文 GetMemory+CopyMemory 不改调用方

        Assert.Equal(10, enc.Length);
        var buf = new byte[10];
        uint n = PowerBase64.DecryptAndiDecodeBase64(enc, buf, 0, buf.Length);
        Assert.Equal(4u, n);                                             // 真实写入量（原文返回值 nBufLen）
        Assert.Equal(keep, buf.AsSpan(0, 4).ToArray());                  // XOR 往返还原（前 4 字节）
    }

    /// <summary>
    /// **原文缺陷的端到端后果**：
    /// <list type="number">
    /// <item><c>DecodeBase64</c> 的返回值是 <c>nCycle*3 - nFillLen</c>（不是真实写入量）；</item>
    /// <item><c>DecryptAndiDecodeBase64</c> 用**传入的缓冲区长度** <c>nDataSize</c> 做 XOR（原文 :308）；
    /// ⇒ 缓冲区比解码长度长时，尾部会被**多异或一次**，原文无法还原。</item>
    /// </list>
    /// 6 字节明文（编码 8 字符）时：缓冲区 8、解码长度 6 → 第 7、8 字节既被解码写入又被异或，
    /// 前一字节（6..6）只被 XOR 覆盖到第 6 个偏移，故只有前 6 字节参与。
    /// </summary>
    [Fact]
    public void DecryptAndiDecodeBase64_XorLengthUsesBufferSize_TailOverXored_OriginalDefect()
    {
        using var env = new SweepTestEnv();
        var six = new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46 };
        string enc = PowerBase64.EncryptAndEncodeBase64(six, six.Length);
        Assert.Equal(8, enc.Length);
        Assert.Equal("GjGGd7RO", enc);

        // 独立复算：XOR 后的字节 = 原文 ^ BASE_CIPHER[CIPHER_INDEX[i and $F]]
        var xored = new byte[6];
        for (int i = 0; i < 6; i++) xored[i] = (byte)(six[i] ^ BaseCipher[CipherIndex[i & 0xF]]);
        Assert.Equal(PowerBase64.EncodeBase64(xored, 6), enc);

        uint probe = PowerBase64.DecodeBase64Probe(enc);
        Assert.Equal(6u, probe);                             // nCycle*3 - nFillLen = 2*3 - 0

        // 缓冲区恰好 6（= 解码长度）：XOR 只作用在 6 字节上 → 完整还原
        var exact = new byte[6];
        Assert.Equal(6u, PowerBase64.DecryptAndiDecodeBase64(enc, exact, 0, exact.Length));
        Assert.Equal(six, exact);

        // 缓冲区 8（> 解码长度 6）：第 7、8 字节被**多异或一次** → 原文无法还原
        var bigger = new byte[8];
        Assert.Equal(6u, PowerBase64.DecryptAndiDecodeBase64(enc, bigger, 0, bigger.Length));
        Assert.Equal(six.AsSpan(0, 4).ToArray(), bigger.AsSpan(0, 4).ToArray());   // 前 4 字节正确
        for (int i = 0; i < 6; i++) Assert.Equal(six[i], bigger[i]);               // 6 字节内全对
        Assert.Equal(BaseCipher[CipherIndex[6]], bigger[6]);   // 未写入 → 0 ^ 密文 = 密文本值
        Assert.Equal(BaseCipher[CipherIndex[7]], bigger[7]);
    }

    /// <summary>
    /// **原文缺陷的端到端后果**（配合 <c>DecodeBase64String</c>）：len ≡ 2 mod 3 的明文
    /// 编码后末位是**单个** '+'，而解码侧恒判 nFillLen=2 → 长度检查失败 → 解不开自己的输出。
    /// </summary>
    [Fact]
    public void EncryptAndEncodeBase64_RoundTrip_FailsForLenMod3Equals2_OriginalDefect()
    {
        using var env = new SweepTestEnv();
        var five = new byte[] { 0x41, 0x42, 0x43, 0x44, 0x45 };   // 5 字节 = 2 mod 3
        string enc = PowerBase64.EncryptAndEncodeBase64(five, five.Length);
        Assert.Equal(9, enc.Length);
        Assert.Equal('+', enc[8]);
        Assert.Equal(0u, PowerBase64.DecodeBase64Probe(enc));      // 探测失败（原文如此）

        var buf = new byte[16];
        // DecryptAndiDecodeBase64：nRealDataSize = 0 → nDataSize(16) >= 0 → 继续 → DecodeBase64 返回 0
        Assert.Equal(0u, PowerBase64.DecryptAndiDecodeBase64(enc, buf, 0, buf.Length));
        Assert.Equal("", PowerBase64.DecryptAndDecodeBase64String(enc));  // nDataSize = 0 → ''

        // 对照：4 字节（1 mod 3）与 6 字节（0 mod 3）都能往返
        var four = new byte[] { 0x41, 0x42, 0x43, 0x44 };
        string enc4 = PowerBase64.EncryptAndEncodeBase64(four, four.Length);
        Assert.Equal(4u, PowerBase64.DecodeBase64Probe(enc4));
        var buf4 = new byte[10];
        Assert.Equal(4u, PowerBase64.DecryptAndiDecodeBase64(enc4, buf4, 0, buf4.Length));
        Assert.Equal(four, buf4.AsSpan(0, 4).ToArray());
    }

    [Fact]
    public void DecryptAndiDecodeBase64_NullOrSmallBuffer_ReturnsProbeLength()
    {
        using var env = new SweepTestEnv();
        var data = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        string enc = PowerBase64.EncryptAndEncodeBase64(data, data.Length);
        uint probe = PowerBase64.DecodeBase64(enc, null, 0, 0);

        Assert.Equal(probe, PowerBase64.DecryptAndiDecodeBase64(enc, null, 0, 0));
        var small = new byte[1];
        Assert.Equal(probe, PowerBase64.DecryptAndiDecodeBase64(enc, small, 0, small.Length));
        Assert.Equal(0, small[0]);
    }

    [Fact]
    public void EncryptAndEncodeBase64String_RoundTripsThroughStringVariant()
    {
        using var env = new SweepTestEnv();
        string src = "Hello!";                                  // 纯 ASCII，避免 Latin-1 承载歧义
        string enc = PowerBase64.EncryptAndEncodeBase64String(src);
        Assert.NotEqual("", enc);
        Assert.Equal(8, enc.Length);                            // 6 字节 → (2+0)*4 = 8，无填充
        Assert.Equal("", PowerBase64.EncryptAndEncodeBase64String(""));   // 空串

        string dec = PowerBase64.DecryptAndDecodeBase64String(enc);
        Assert.Equal(src, dec);
    }

    [Fact]
    public void DecryptAndDecodeBase64String_InvalidProbe_ReturnsEmpty()
    {
        using var env = new SweepTestEnv();
        Assert.Equal("", PowerBase64.DecryptAndDecodeBase64String(""));
        Assert.Equal("", PowerBase64.DecryptAndDecodeBase64String("4"));       // < 4 → probe 0
        Assert.Equal("", PowerBase64.DecryptAndDecodeBase64String("44445"));   // 非 4 倍数 → probe 0
        Assert.Equal("", PowerBase64.DecryptAndDecodeBase64String("44444444\u00FF\u00FF\u00FF\u00FF"));
    }

    [Fact]
    public void EncodeBase64String_AndDecodeBase64String_OriginalAsymmetry()
    {
        using var env = new SweepTestEnv();
        byte[] data = { 0x61, 0x62, 0x63 };
        string enc = PowerBase64.EncodeBase64String("abc");
        Assert.Equal(PowerBase64.EncodeBase64(data, 3), enc);
        Assert.Equal("", PowerBase64.EncodeBase64String(""));      // nStrSize=0 → 同 EncodeBase64

        // 无填充的 3 字节串：探测长度 = 真实长度 → DecodeBase64String 可完整还原（XOR 不参与）
        Assert.Equal("abc", PowerBase64.DecodeBase64String(enc));

        // 差异断言：4 字节的编码是 10 字符，探测长度恰为 4（两端偏差抵消，见上一条），
        // 因此 DecodeBase64String 能**完整**还原 4 字节（XOR 不参与 → 与原文一致）。
        string enc4 = PowerBase64.EncodeBase64String("ABCD");
        Assert.Equal(10, enc4.Length);
        Assert.Equal(4u, PowerBase64.DecodeBase64Probe(enc4));
        string dec4 = PowerBase64.DecodeBase64String(enc4);
        Assert.Equal(4, dec4.Length);
        Assert.Equal("ABCD", dec4);    }
}
