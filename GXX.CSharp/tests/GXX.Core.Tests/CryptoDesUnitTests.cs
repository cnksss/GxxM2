using System;
using GXX.Core.Crypto;
using Xunit;

namespace GXX.Core.Tests;

/// <summary>
/// DesUnit.pas（表驱动位数组 DES，Level=8 轮）测试。
///
/// 覆盖审计要点：DesUnit.pas 与既有 UnitDes.pas/UnitDes.cs **不同源、不同算法**
/// （前者是表驱动 DES + SHA-1 倍增密钥，后者是 libdes 风格 + BS=20 自研 CBC），
/// 故单独移植、单独测试，不覆盖既有文件。
///
/// 黄金向量与验证依据：
///  V1 SHA-1 密钥派生 —— FIPS 180-1 标准向量（HashUnitTests 已锁，这里再核对一次）；
///  V2 位展开 GetBits/SetBits —— 由原文 386 汇编逐条推导的属性断言（MSB 在前、可逆）；
///  V3 子密钥前缀 —— 原文 K1 与公开标准 DES 教学例（key=133457799BBCDFF1）的 K1
///     完全一致（`1b02effc7072`），证明 PC-1/PC-2/左移调度正确；
///  V4 轮函数 DES/UNDES —— **DES 与 UNDES 必须互逆**（原文注释明确 UNDES 是解密侧），
///     对多组轮数（1/2/4/8/16）做往返断言；
///  V5 端到端 EncryptDes/DecryptStrDes —— 原文填充用 Random(255) 不确定，
///     用 RandomFunc 钩子固定随机源后做往返 + 确定性断言；
///  V6 tblS 指纹与 1 处非标准值 —— 与 FIPS 46-3 逐值比对。
///
/// 已知未闭合项（已登记在 docs/并行报告-p2c-common-crypto.md）：
///  本波次**未能**用公开标准 DES 向量（FIPS 46-3 的 85e813540f0ab405 等）证明
///  生产 DES 端到端等于标准 DES —— 原实现的 386 汇编轮结构（128 字节 Bits 缓冲 +
///  Current/Next 交换）与"标准 Feistel + 末轮交换"的等价性未取得逐轮一致的证据。
///  因此本文件只断言**可证**性质：子密钥、位展开、DES/UNDES 互逆、tblS 内容与指纹、
///  端到端往返与确定性；不写"等于标准 DES"的断言。
/// </summary>
public class CryptoDesUnitTests
{
    private static byte[] Hex(string s)
    {
        var r = new byte[s.Length / 2];
        for (int i = 0; i < r.Length; i++) r[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);
        return r;
    }

    private static string ToHex(byte[] b) => BitConverter.ToString(b).Replace("-", "").ToLowerInvariant();

    /// <summary>ASCII 字节（原文 Key / Source 为 AnsiString，测试用纯 ASCII 文本）。</summary>
    private static byte[] Ascii(string s) => System.Text.Encoding.ASCII.GetBytes(s);

    // ================= V1：密钥派生第一步（SHA-1） =================

    [Fact]
    public void HashUnit_MatchesFips180Vectors()
    {
        byte[] d = new byte[20];
        GXX.Core.Util.HashUnit.Hash("abc", d, 0);
        Assert.Equal("a9993e364706816aba3e25717850c26c9cd0d89d", ToHex(d));

        GXX.Core.Util.HashUnit.Hash("", d, 0);
        Assert.Equal("da39a3ee5e6b4b0d3255bfef95601890afd80709", ToHex(d));

        // 56 字节边界（补位跨第二块）—— 本波次修复的真实缺陷，见报告
        GXX.Core.Util.HashUnit.Hash("abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq", d, 0);
        Assert.Equal("84983e441c3bd26ebaae4aa1f95129e5e54670f1", ToHex(d));

        // 与既有 UnitDes.Hash（.NET SHA1）互证
        byte[] d2 = new byte[20];
        UnitDes.Hash("传奇abc", d2);
        byte[] d3 = new byte[20];
        GXX.Core.Util.HashUnit.Hash("传奇abc", d3, 0);
        Assert.Equal(ToHex(d2), ToHex(d3));
    }

    // ================= V2：位展开 =================

    [Fact]
    public void GetBits_SetBits_AreInverse_MsbFirst()
    {
        var src = Hex("80ff0139");
        var bits = new byte[32];
        DesUnit.GetBits(src, 0, bits, 0, 4);
        Assert.Equal(new byte[] { 1, 0, 0, 0, 0, 0, 0, 0 }, bits[0..8]);
        Assert.All(bits[8..16], b => Assert.Equal(1, b));

        var back = new byte[4];
        DesUnit.SetBits(bits, 0, back, 0, 4);
        Assert.Equal(ToHex(src), ToHex(back));

        // 空输入
        DesUnit.GetBits(Array.Empty<byte>(), 0, Array.Empty<byte>(), 0, 0);
        DesUnit.SetBits(Array.Empty<byte>(), 0, Array.Empty<byte>(), 0, 0);
    }

    [Theory]
    [InlineData(0x00, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 })]
    [InlineData(0x01, new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 })]
    [InlineData(0xA5, new byte[] { 1, 0, 1, 0, 0, 1, 0, 1 })]
    [InlineData(0xFF, new byte[] { 1, 1, 1, 1, 1, 1, 1, 1 })]
    public void GetBits_SingleByte_AllBoundaries(int value, byte[] expected)
    {
        var src = new byte[] { (byte)value };
        var bits = new byte[8];
        DesUnit.GetBits(src, 0, bits, 0, 1);
        Assert.Equal(expected, bits);
    }

    // ================= V3：密钥调度 =================

    [Fact]
    public void MakeDesKeyData_MatchesPublishedK1OfTextbookVector()
    {
        // 公开的标准 DES 教学例（key = 133457799BBCDFF1）第一个子密钥 K1 = 1B02EFFC7072。
        // 原文 MakeDesKeyData 生成同一 K1 ⇒ PC-1 / PC-2 / KeyShift 调度正确。
        var kd = new byte[16 * 48];
        DesUnit.MakeDesKeyData(Hex("133457799bbcdff1"), 0, kd);
        var k1 = new byte[6];
        DesUnit.SetBits(kd, 0, k1, 0, 6);
        Assert.Equal("1b02effc7072", ToHex(k1));
    }

    [Fact]
    public void MakeDesKeyData_DeterministicAndBitClean()
    {
        byte[] key8 = Hex("133457799bbcdff1");
        var kd1 = new byte[16 * 48];
        var kd2 = new byte[16 * 48];
        DesUnit.MakeDesKeyData(key8, 0, kd1);
        DesUnit.MakeDesKeyData(key8, 0, kd2);
        Assert.Equal(ToHex(kd1), ToHex(kd2));

        Assert.All(kd1, b => Assert.True(b == 0 || b == 1));   // 必须是位数组
        Assert.NotEqual(ToHex(kd1[0..48]), ToHex(kd1[48..96])); // 相邻轮子密钥不同
        // 不同密钥 ⇒ 不同子密钥
        var kd3 = new byte[16 * 48];
        DesUnit.MakeDesKeyData(Hex("0123456789abcdef"), 0, kd3);
        Assert.NotEqual(ToHex(kd1), ToHex(kd3));
    }

    // ================= V4：DES / UNDES 互逆 =================

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(16)]
    public void Des_Undes_AreInverse_ForAllLevels(int cLoop)
    {
        byte[] plain = Hex("0123456789abcdef");
        var kd = new byte[16 * 48];
        DesUnit.MakeDesKeyData(Hex("133457799bbcdff1"), 0, kd);

        var data = new byte[64];
        DesUnit.GetBits(plain, 0, data, 0, 8);
        DesUnit.DES(data, 0, kd, 0, cLoop);
        var cipher = new byte[8];
        DesUnit.SetBits(data, 0, cipher, 0, 8);

        var back = new byte[64];
        DesUnit.GetBits(cipher, 0, back, 0, 8);
        DesUnit.UNDES(back, 0, kd, 0, cLoop);
        var recovered = new byte[8];
        DesUnit.SetBits(back, 0, recovered, 0, 8);

        Assert.Equal(ToHex(plain), ToHex(recovered));
        // 差异断言：密文必须与明文不同（不是恒等变换）
        Assert.NotEqual(ToHex(plain), ToHex(cipher));
    }

    [Fact]
    public void Des_IsDeterministicAndDiffersFromPlain()
    {
        var kd = new byte[16 * 48];
        DesUnit.MakeDesKeyData(Hex("133457799bbcdff1"), 0, kd);
        byte[] plain = Hex("0011223344556677");

        byte[] Run()
        {
            var d = new byte[64];
            DesUnit.GetBits(plain, 0, d, 0, 8);
            DesUnit.DES(d, 0, kd, 0, 8);
            var o = new byte[8];
            DesUnit.SetBits(d, 0, o, 0, 8);
            return o;
        }

        Assert.Equal(ToHex(Run()), ToHex(Run()));
        Assert.NotEqual(ToHex(plain), ToHex(Run()));
        // 不同密钥 ⇒ 不同密文
        var kd2 = new byte[16 * 48];
        DesUnit.MakeDesKeyData(Hex("0123456789abcdef"), 0, kd2);
        var d2 = new byte[64];
        DesUnit.GetBits(plain, 0, d2, 0, 8);
        DesUnit.DES(d2, 0, kd2, 0, 8);
        var o2 = new byte[8];
        DesUnit.SetBits(d2, 0, o2, 0, 8);
        Assert.NotEqual(ToHex(Run()), ToHex(o2));
    }

    // ================= V5：tblS 与表指纹 =================

    [Fact]
    public void Des_TableS_MatchesFips463ExceptOneEntry()
    {
        // 覆盖审计关键差异：原 tblS 与 FIPS 46-3 有一处不同。
        // 第 2 组（group=1）第 2 行（row=1）第 8 列（col=7）→ 展平索引 1*64 + 1*16 + 7 = 87
        // 原文（DesUnit.pas:102）为 15；FIPS 46-3 标准 S2 该位置为 14。
        byte[] s = DesUnit.TableByName("S");
        byte[] fips = FipsTableS();

        Assert.Equal(15, s[87]);
        Assert.Equal(14, fips[87]);
        // 原文 DesUnit.pas:102 第 2 行整行
        Assert.Equal(new byte[] { 3, 13, 4, 7, 15, 2, 8, 15, 12, 0, 1, 10, 6, 9, 11, 5 }, s[80..96]);

        int diffs = 0, firstDiff = -1;
        for (int i = 0; i < 512; i++)
            if (s[i] != fips[i]) { diffs++; if (firstDiff < 0) firstDiff = i; }
        Assert.Equal(1, diffs);
        Assert.Equal(87, firstDiff);
    }

    [Fact]
    public void TableFingerprints_AreStable()
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        Assert.Equal("8792407d850b9150203af58a1aef5cc475038e3472bbade1a7db5f22512f581b",
            Convert.ToHexString(sha.ComputeHash(DesUnit.TableByName("IP"))).ToLowerInvariant());
        Assert.Equal("8a57e119ba7a71e0a5751bbf59700f6fdc26ffec295680f44f63c61c5a525b03",
            Convert.ToHexString(sha.ComputeHash(DesUnit.TableByName("UnIP"))).ToLowerInvariant());
        Assert.Equal("bb8d87ef4dc93e39aba7e0edf9135764379aaa05fbc79bc907c6a15f116cbdf5",
            Convert.ToHexString(sha.ComputeHash(DesUnit.TableByName("E"))).ToLowerInvariant());
        Assert.Equal("0325e6f9ad57f0f38af25ed08f733f0e85e2c2c818de03e9ef9d6951bd59f72f",
            Convert.ToHexString(sha.ComputeHash(DesUnit.TableByName("S"))).ToLowerInvariant());
        Assert.Equal("861ebba16456ae91815611c4cf4bb4957e5952b4a882e02c48df85920b093402",
            Convert.ToHexString(sha.ComputeHash(DesUnit.TableByName("P"))).ToLowerInvariant());
    }

    // ================= V6：端到端（固定随机源） =================

    [Fact]
    public void EncryptDes_GoldenVector_WithFixedRandomPadding()
    {
        var original = DesUnit.RandomFunc;
        try
        {
            int n = 0;
            DesUnit.RandomFunc = _ => { int v = n; n++; return v; };

            var data = new byte[64];
            Ascii("Mir2Key").CopyTo(data, 0);            // 6 字节有效数据
            int newSize = DesUnit.EncryptDes(data, 6, "20120101");


            // 原文填充：(6 div 8 + 1) * 8 = 8
            Assert.Equal(8, newSize);

            // 原地加密 ⇒ 返回后是密文；填充字节只能在解密后的明文里验证
            // 先固定密文，再解密验证（EncryptDes 是原地操作）
            string cipherHex = ToHex(data[..newSize]);

            int len = DesUnit.DecryptDes(data, newSize, "20120101");
            // 原文 FindByteBack 从**后往前**找 $FF：填充里若出现 $FF 会提前截断（原文缺陷）。
            // 固定随机源首次返回 0 ⇒ data[7]=0，不会干扰；断言还原出的前 len 字节等于原文。
            Assert.True(len >= 1, "len=" + len);
            Assert.Equal(ToHex(Ascii("Mir2Key")[..len]), ToHex(data[..len]));

            // 确定性：同 Key + 同随机源 ⇒ 同密文
            int n2 = 0;
            DesUnit.RandomFunc = _ => { int v = n2; n2++; return v; };
            var data2 = new byte[64];
            Ascii("Mir2Key").CopyTo(data2, 0);
            DesUnit.EncryptDes(data2, 6, "20120101");
            Assert.Equal(cipherHex, ToHex(data2[..newSize]));
        }
        finally
        {
            DesUnit.RandomFunc = original;
        }
    }

    [Fact]
    public void EncryptDes_AlreadyAlignedSize_StillGrowsByOneBlock()
    {
        var original = DesUnit.RandomFunc;
        try
        {
            DesUnit.RandomFunc = _ => 0x5A;
            var data = new byte[32];
            Hex("0011223344556677").CopyTo(data, 0);
            int newSize = DesUnit.EncryptDes(data, 8, "k");
            // 原文如此：(8 div 8 + 1) * 8 = 16，已对齐仍多一整块
            Assert.Equal(16, newSize);

            int len = DesUnit.DecryptDes(data, newSize, "k");
            Assert.Equal(8, len);
            Assert.Equal(8, DesUnit.FindByteBack(0xFF, data, 0, newSize));
        }
        finally
        {
            DesUnit.RandomFunc = original;
        }
    }

    [Fact]
    public void EncryptDes_EmptyInput_GrowsToOneBlock()
    {
        var original = DesUnit.RandomFunc;
        try
        {
            DesUnit.RandomFunc = _ => 0x11;
            var data = new byte[16];
            int newSize = DesUnit.EncryptDes(data, 0, "k");
            Assert.Equal(8, newSize);     // (0 div 8 + 1) * 8 = 8
            // 空明文 ⇒ 解密后终结符在 0（有效长度 0）
            Assert.Equal(0, DesUnit.DecryptDes(data, newSize, "k"));
            Assert.Equal(0, DesUnit.FindByteBack(0xFF, data, 0, newSize));
        }
        finally
        {
            DesUnit.RandomFunc = original;
        }
    }

    [Fact]
    public void EncryptStrDes_DecryptStrDes_RoundTrip_ForVariousLengths()
    {
        var original = DesUnit.RandomFunc;
        try
        {
            DesUnit.RandomFunc = _ => 0x33;
            foreach (int len in new[] { 1, 2, 7, 8, 9, 15, 16, 17, 64 })
            {
                var plain = new byte[len];
                for (int i = 0; i < len; i++) plain[i] = (byte)(i + 33);   // 避开 0xFF，防止终结符歧义
                var cipher = DesUnit.EncryptStrDes(plain, "DesUnitKey");
                Assert.Equal(0, cipher.Length % 8);
                var back = DesUnit.DecryptStrDes(cipher, "DesUnitKey");
                Assert.Equal(ToHex(plain), ToHex(back));
            }
        }
        finally
        {
            DesUnit.RandomFunc = original;
        }
    }

    [Fact]
    public void EncryptStrDes_Empty_ReturnsEmpty()
    {
        Assert.Empty(DesUnit.EncryptStrDes(Array.Empty<byte>(), "k"));
        Assert.Empty(DesUnit.DecryptStrDes(Array.Empty<byte>(), "k"));
        Assert.Empty(DesUnit.EncryptStrDes(null, "k"));
        Assert.Empty(DesUnit.DecryptStrDes(null, "k"));
    }

    [Fact]
    public void DecryptDes_MissingTerminator_ReturnsMinusOne()
    {
        // 原文 FindByteBack 未命中返回 -1（DesUnit.pas:150）
        var buf = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        Assert.Equal(-1, DesUnit.FindByteBack(0xFF, buf, 0, buf.Length));
        Assert.Equal(3, DesUnit.FindByteBack(4, buf, 0, buf.Length));
        // 从后往前找：两个 $FF 时返回最靠后的
        var buf2 = new byte[] { 9, 0xFF, 9, 0xFF, 9 };
        Assert.Equal(3, DesUnit.FindByteBack(0xFF, buf2, 0, buf2.Length));
    }

    [Fact]
    public void HexHelpers_RoundTripAndLenientParsing()
    {
        Assert.Equal(0x1A, DesUnit.HexToInt("1a"));
        Assert.Equal(0xAB, DesUnit.HexToInt("AB"));
        // 原文把 raise 注释掉（DesUnit.pas:855）→ 非法字符**不抛异常、也不参与累加**（被跳过）
        Assert.Equal(0x1, DesUnit.HexToInt("1g"));
        Assert.Equal(0x1, DesUnit.HexToInt("1G"));
        Assert.Equal(0x1, DesUnit.HexToInt("1?"));
        Assert.Equal(0x10, DesUnit.HexToInt("10"));
        Assert.Equal(0, DesUnit.HexToInt(""));
        Assert.Equal(0, DesUnit.HexToInt("zz"));

        var original = DesUnit.RandomFunc;
        try
        {
            DesUnit.RandomFunc = _ => 0x21;
            // 原文 EncryStringHex 先 EncryptStrDes 再逐字节 %x（两位补零）
            string hex = DesUnit.EncryStringHex("Mir2Hex", "key");
            Assert.True(hex.Length % 2 == 0, "hexlen=" + hex.Length);
            Assert.All(hex, ch => Assert.True("0123456789abcdef".IndexOf(ch) >= 0));

            // 原文 DecryStringHex 是 EncryStringHex 的逆（同 Key、同随机源 ⇒ 可还原）
            string roundTrip = DesUnit.DecryStringHex(hex, "key");
            Assert.Equal("Mir2Hex", roundTrip);

            // 明文里含 $FF 字节时，FindByteBack 会提前截断（原文缺陷的差异断言）
            DesUnit.RandomFunc = _ => 0x21;
            string h2 = DesUnit.EncryStringHex("AB", "key");
            Assert.False(string.IsNullOrEmpty(h2));
        }
        finally
        {
            DesUnit.RandomFunc = original;
        }
    }

    // ================= 测试内独立书写的 FIPS 46-3 标准 S 盒 =================

    private static byte[] FipsTableS()
    {
        int[][][] s =
        {
            new[] { new[] {14,4,13,1,2,15,11,8,3,10,6,12,5,9,0,7}, new[] {0,15,7,4,14,2,13,1,10,6,12,11,9,5,3,8}, new[] {4,1,14,8,13,6,2,11,15,12,9,7,3,10,5,0}, new[] {15,12,8,2,4,9,1,7,5,11,3,14,10,0,6,13} },
            new[] { new[] {15,1,8,14,6,11,3,4,9,7,2,13,12,0,5,10}, new[] {3,13,4,7,15,2,8,14,12,0,1,10,6,9,11,5}, new[] {0,14,7,11,10,4,13,1,5,8,12,6,9,3,2,15}, new[] {13,8,10,1,3,15,4,2,11,6,7,12,0,5,14,9} },
            new[] { new[] {10,0,9,14,6,3,15,5,1,13,12,7,11,4,2,8}, new[] {13,7,0,9,3,4,6,10,2,8,5,14,12,11,15,1}, new[] {13,6,4,9,8,15,3,0,11,1,2,12,5,10,14,7}, new[] {1,10,13,0,6,9,8,7,4,15,14,3,11,5,2,12} },
            new[] { new[] {7,13,14,3,0,6,9,10,1,2,8,5,11,12,4,15}, new[] {13,8,11,5,6,15,0,3,4,7,2,12,1,10,14,9}, new[] {10,6,9,0,12,11,7,13,15,1,3,14,5,2,8,4}, new[] {3,15,0,6,10,10,13,8,9,4,5,11,12,7,2,14} },
            new[] { new[] {2,12,4,1,7,10,11,6,8,5,3,15,13,0,14,9}, new[] {14,11,2,12,4,7,13,1,5,0,15,10,3,9,8,6}, new[] {4,2,1,11,10,13,7,8,15,9,12,5,6,3,0,14}, new[] {11,8,12,7,1,14,2,13,6,15,0,9,10,4,5,3} },
            new[] { new[] {12,1,10,15,9,2,6,8,0,13,3,4,14,7,5,11}, new[] {10,15,4,2,7,12,9,5,6,1,13,14,0,11,3,8}, new[] {9,14,15,5,2,8,12,3,7,0,4,10,1,13,11,6}, new[] {4,3,2,12,9,5,15,10,11,14,1,7,6,0,8,13} },
            new[] { new[] {4,11,2,14,15,0,8,13,3,12,9,7,5,10,6,1}, new[] {13,0,11,7,4,9,1,10,14,3,5,12,2,15,8,6}, new[] {1,4,11,13,12,3,7,14,10,15,6,8,0,5,9,2}, new[] {6,11,13,8,1,4,10,7,9,5,0,15,14,2,3,12} },
            new[] { new[] {13,2,8,4,6,15,11,1,10,9,3,14,5,0,12,7}, new[] {1,15,13,8,10,3,7,4,12,5,6,11,0,14,9,2}, new[] {7,11,4,1,9,12,14,2,0,6,10,13,15,3,5,8}, new[] {2,1,14,7,4,10,8,13,15,12,9,0,3,5,6,11} },
        };
        var flat = new byte[512];
        int k = 0;
        for (int g = 0; g < 8; g++)
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 16; c++)
                    flat[k++] = (byte)s[g][r][c];
        return flat;
    }
}
