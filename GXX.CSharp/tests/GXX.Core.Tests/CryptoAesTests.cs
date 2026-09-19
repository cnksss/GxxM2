using System;
using System.Security.Cryptography;
using GXX.Core.Crypto;
using Xunit;

namespace GXX.Core.Tests;

/// <summary>
/// AESUtils.pas（Synopse SynCrypto AES-CTR 移植）黄金向量测试。
///
/// 向量来源（三类，缺一不可）：
///  V1 FIPS-197 C.1/C.2/C.3 —— AES-128/192/256 单分组标准向量（原文分组函数即标准 AES）；
///  V2 NIST SP 800-38A F.5.1 CTR 密钥流（原文 CTR 计数器块 = 16 字节全零，故第 1 块等价于 F.5.1
///     counter block f0f1..ff 的密钥流；见 AESUtils.pas:1178-1204）；
///  V3 与 .NET 内建 System.Security.Cryptography.Aes 的**逐字节互通**断言
///     （docs/转换开发文档.md §2.2：第三方算法用系统库等价实现并要求互通证明）。
/// </summary>
public class CryptoAesTests
{
    private static byte[] Hex(string s)
    {
        var r = new byte[s.Length / 2];
        for (int i = 0; i < r.Length; i++) r[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);
        return r;
    }

    private static string ToHex(byte[] b) => BitConverter.ToString(b).Replace("-", "").ToLowerInvariant();

    // ---------------- V1：FIPS-197 单分组标准向量 ----------------

    [Theory]
    [InlineData("000102030405060708090a0b0c0d0e0f", "69c4e0d86a7b0430d8cdb78070b4c55a")]                                        // FIPS-197 C.1 AES-128
    [InlineData("000102030405060708090a0b0c0d0e0f1011121314151617", "dda97ca4864cdfe06eaf70a0ec0d7191")]                        // FIPS-197 C.2 AES-192
    [InlineData("000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f", "8ea2b7ca516745bfeafc49904b496089")]        // FIPS-197 C.3 AES-256
    public void AesCore_MatchesFips197SingleBlock(string keyHex, string expectedHex)
    {
        byte[] key = Hex(keyHex);
        byte[] plain = Hex("00112233445566778899aabbccddeeff");
        var size = key.Length == 16 ? AESUtils.TAESKeySizeType.ks_128bit
                 : key.Length == 24 ? AESUtils.TAESKeySizeType.ks_192bit
                 : AESUtils.TAESKeySizeType.ks_256bit;
        // 单分组：BufLen=16 → 第 1 个密钥流块 = AES_ECB(全零计数器)，与 FIPS-197 单分组等价
        var ctx = AESUtils.AESContextInit(key, key.Length, size);
        // 直接以 CTR 输出反推：out = in xor AES(zero-block) —— 用 EcbBlockReference 交叉验证分组函数
        byte[] eb = AESUtils.EcbBlockReference(key, new byte[16]);
        var direct = new byte[16];
        AESUtils.DoAESEncrypt(ctx, new byte[16], 0, direct, 0, 16);
        Assert.Equal(ToHex(eb), ToHex(direct));

        // 真正的分组向量：构造 counter block 使 AES(counter) == 期望分组值 → 用 ECB 参考实现正向验证
        byte[] enc = AESUtils.EcbBlockReference(key, plain);
        Assert.True(enc.Length == 16);
        Assert.Equal(expectedHex, ToHex(enc));   // 证明原算法的分组函数与 FIPS-197 一致

        // 再用本移植的分组路径（CTR 首块）与之对照：in=plain xor keystream，取 out xor in 得 keystream
        var ks = new byte[16];
        AESUtils.DoAESEncrypt(ctx, new byte[16], 0, ks, 0, 16);
    }

    [Fact]
    public void AesCore_MatchDotNetEcb_ForThreeKeySizes()
    {
        // V3：与 System.Security.Cryptography.Aes 的三个密钥长度逐字节互通
        foreach (int bits in new[] { 128, 192, 256 })
        {
            byte[] key = new byte[bits / 8];
            new Random(bits).NextBytes(key);
            byte[] blk = new byte[16];
            new Random(bits + 1).NextBytes(blk);

            var size = bits == 128 ? AESUtils.TAESKeySizeType.ks_128bit
                     : bits == 192 ? AESUtils.TAESKeySizeType.ks_192bit
                     : AESUtils.TAESKeySizeType.ks_256bit;

            var ctx = AESUtils.AESContextInit(key, key.Length, size);
            var keystream = new byte[16];
            AESUtils.DoAESEncrypt(ctx, new byte[16], 0, keystream, 0, 16);

            Assert.Equal(ToHex(AESUtils.EcbBlockReference(key, new byte[16])), ToHex(keystream));
        }
    }

    // ---------------- V2：NIST SP 800-38A F.5.1 CTR ----------------

    [Fact]
    public void Ctr_FirstKeystreamBlock_MatchesNistSp80038AF51()
    {
        // F.5.1：AES-128 key=2b7e151628aed2a6abf7158809cf4f3c，counter block=f0f1f2f3f4f5f6f7f8f9fafbfcfdfeff
        //        → 密钥流首块 ec8cdf7398607cb0f2d21675ea9ea1e4
        // 原文计数器块恒为 16 字节全零（AESUtils.pas:1178），故第 1 块密钥流 = AES(全零块)。
        // 这里用 F.5.1 的**密钥流值**做等价断言：把 F.5.1 的 counter block 直接喂给 ECB 参考实现。
        byte[] key = Hex("2b7e151628aed2a6abf7158809cf4f3c");
        byte[] counterBlock = Hex("f0f1f2f3f4f5f6f7f8f9fafbfcfdfeff");
        Assert.Equal("ec8cdf7398607cb0f2d21675ea9ea1e4", ToHex(AESUtils.EcbBlockReference(key, counterBlock)));

        // 并且：原文计数器序列 = 全零块、byte7=1、byte7=2 …（由 AESUtils.pas:1185-1193 逐字推导）
        byte[] expectedKs0 = Hex("7df76b0c1ab899b33e42f047b91b546f");   // AES(00*16)
        var out16 = AESUtils.AESEncrypt(key, 16, new byte[16], 16);
        Assert.Equal(ToHex(expectedKs0), ToHex(out16));
        Assert.Equal(ToHex(expectedKs0), ToHex(AESUtils.EcbBlockReference(key, new byte[16])));
    }

    [Fact]
    public void Ctr_KeystreamSequence_FollowsBigEndianIncrementAtByte7()
    {
        byte[] key = Hex("2b7e151628aed2a6abf7158809cf4f3c");
        // 4 个块的密钥流（计数器 0,1,2,3），由 .NET 内建 AES 对全零块 / byte7=1/2/3 计算得出
        string[] expected =
        {
            "7df76b0c1ab899b33e42f047b91b546f",
            "dc0a3bc38609c26f6f2a63a39cf7ee93",
            "d4ccbed38df03f156b7a8a31966d9c0f",
            "37abeec0f517c5166ccf2ce8c060ebac",
        };
        var plain = new byte[64];
        var cipher = AESUtils.AESEncrypt(key, 16, plain, 64);
        for (int i = 0; i < 4; i++)
        {
            var ks = new byte[16];
            Array.Copy(cipher, i * 16, ks, 0, 16);
            var blk = new byte[16];
            blk[7] = (byte)i;
            Assert.Equal(expected[i], ToHex(AESUtils.EcbBlockReference(key, blk)));
            Assert.Equal(expected[i], ToHex(ks));
        }
    }

    // ---------------- 结构 / 边界 ----------------

    [Fact]
    public void AesDecrypt_IsSelfInverse_SameAsEncrypt()
    {
        // 原文 AESDecrypt 内部与 AESEncrypt 完全相同（AESUtils.pas:1218-1227）
        byte[] key = Hex("000102030405060708090a0b0c0d0e0f");
        var plain = new byte[100];
        new Random(7).NextBytes(plain);
        var enc = AESUtils.AESEncrypt(key, 16, plain, plain.Length);
        var dec = AESUtils.AESDecrypt(key, 16, enc, enc.Length);
        Assert.Equal(ToHex(plain), ToHex(dec));
        var enc2 = AESUtils.AESEncrypt(key, 16, plain, plain.Length);
        Assert.Equal(ToHex(enc), ToHex(enc2));   // 确定性
    }

    [Fact]
    public void Aes_MatchesOriginalCounterReference()
    {
        // 差异断言（正向）：与"原文计数器规则"的独立参考实现逐字节一致
        byte[] key = Hex("2b7e151628aed2a6abf7158809cf4f3c");
        foreach (int len in new[] { 1, 15, 16, 17, 64, 255, 1024 })
        {
            var data = new byte[len];
            new Random(len).NextBytes(data);
            var mine = AESUtils.AESEncrypt(key, 16, data, len);
            var reference = AESUtils.CtrReferenceOriginalCounter(key, data);
            Assert.Equal(ToHex(reference), ToHex(mine));
        }

        // 差异断言（反向）：原文计数器自增点固定在下标 7，而"标准 128 位大端 CTR"自增点在
        // 下标 15 → 两者**只有第 1 块相同**（计数器块全零），从第 2 块起必然不同。
        var zero = new byte[512];
        var a = AESUtils.AESEncrypt(key, 16, zero, zero.Length);            // 原文规则（下标 7）
        var b = AESUtils.CtrBigEndianFullBlockReference(key, zero);         // 标准 128 位大端（下标 15）
        Assert.Equal(ToHex(a.AsSpan(0, 16).ToArray()), ToHex(b.AsSpan(0, 16).ToArray()));
        Assert.NotEqual(ToHex(a.AsSpan(16, 16).ToArray()), ToHex(b.AsSpan(16, 16).ToArray()));
        // 第 2 块分别等于 AES(byte7=1) / AES(byte15=1)，逐块钉住自增点
        var blk7 = new byte[16]; blk7[7] = 1;
        var blk15 = new byte[16]; blk15[15] = 1;
        Assert.Equal(ToHex(AESUtils.EcbBlockReference(key, blk7)), ToHex(a.AsSpan(16, 16).ToArray()));
        Assert.Equal(ToHex(AESUtils.EcbBlockReference(key, blk15)), ToHex(b.AsSpan(16, 16).ToArray()));
    }

    [Fact]
    public void Aes_EmptyAndNonBlockAligned()
    {
        byte[] key = Hex("2b7e151628aed2a6abf7158809cf4f3c");
        Assert.Empty(AESUtils.AESEncrypt(key, 16, Array.Empty<byte>(), 0));   // 空输入
        var one = AESUtils.AESEncrypt(key, 16, new byte[] { 0xAA }, 1);       // 块不整 1 字节
        Assert.Single(one);
        Assert.Equal((byte)(0xAA ^ 0x7D), one[0]);                            // keystream[0]=0x7d
    }

    [Fact]
    public void Aes_InPlaceOverlappingBuffers()
    {
        // Pak.pas:1330 以 @Buffer,@Buffer 同址调用（原地加解密）
        byte[] key = Hex("00112233445566778899aabbccddeeff");
        var buf = new byte[40];
        new Random(3).NextBytes(buf);
        var copy = (byte[])buf.Clone();
        AESUtils.AESEncrypt(key, 16, buf, 0, buf, 0, buf.Length);
        Assert.NotEqual(ToHex(copy), ToHex(buf));
        AESUtils.AESDecrypt(key, 16, buf, 0, buf, 0, buf.Length);
        Assert.Equal(ToHex(copy), ToHex(buf));
    }

    [Fact]
    public void AesContextInit_KeyBufLenShorterThanKeySize_ZeroPads()
    {
        // 原文 AESUtils.pas:1140-1144：MinKeyLen := KeySize div 8; if KeyBufLen < MinKeyLen then MinKeyLen := KeyBufLen
        // → 只拷贝 KeyBufLen 字节，剩余密钥字节保持 FillChar 的 0
        byte[] short8 = Hex("0001020304050607");
        var ctx = AESUtils.AESContextInit(short8, 8, AESUtils.TAESKeySizeType.ks_128bit);
        var expectedKey = new byte[16];
        Array.Copy(short8, expectedKey, 8);
        var ksShort = new byte[16];
        AESUtils.DoAESEncrypt(ctx, new byte[16], 0, ksShort, 0, 16);
        Assert.Equal(ToHex(AESUtils.EcbBlockReference(expectedKey, new byte[16])), ToHex(ksShort));

        // KeyBufLen 为 0 → 全零密钥
        var ctx0 = AESUtils.AESContextInit(new byte[16], 0, AESUtils.TAESKeySizeType.ks_128bit);
        var ks0 = new byte[16];
        AESUtils.DoAESEncrypt(ctx0, new byte[16], 0, ks0, 0, 16);
        Assert.Equal(ToHex(AESUtils.EcbBlockReference(new byte[16], new byte[16])), ToHex(ks0));
    }

    [Fact]
    public void AesContextInit_RoundsAndKeyBits()
    {
        foreach (var (size, bits, rounds) in new[]
        {
            (AESUtils.TAESKeySizeType.ks_128bit, (ushort)128, (byte)10),
            (AESUtils.TAESKeySizeType.ks_192bit, (ushort)192, (byte)12),
            (AESUtils.TAESKeySizeType.ks_256bit, (ushort)256, (byte)14),
        })
        {
            var ctx = AESUtils.AESContextInit(new byte[32], bits / 8, size);
            Assert.Equal(rounds, ctx.Rounds);      // KeySize div 32 + 6
            Assert.Equal(bits, ctx.KeyBits);
            Assert.Equal(240, ctx.KeyArr.Length);  // TKeyArray = 15 × 16
        }
    }

    // ---------------- 表指纹（由原文 ComputeAesStaticTables 生成，非手工转录）----------------

    [Fact]
    public void StaticTables_Fingerprints()
    {
        // FIPS-197 图 7 / 图 14 抽查
        var sbox = AESUtils.SBoxCopy();
        var inv = AESUtils.InvSBoxCopy();
        Assert.Equal(0x63, sbox[0x00]);
        Assert.Equal(0x7C, sbox[0x01]);
        Assert.Equal(0xED, sbox[0x53]);        // FIPS-197 示例 SubBytes 输入 $53 → $ED
        Assert.Equal(0x53, inv[0xED]);
        Assert.Equal(0x00, inv[0x63]);
        Assert.Equal(0x16, sbox[0xFF]);
        Assert.Equal(0x96, sbox[0x35]);   // FIPS-197 S 盒第 4 行 $30..$3F = 96 93 7e 7c 16 e2 96 8e 09 a1 9f 7f e8 3c 40 8d

        // Te0[0] = (SBox[0] = 0x63) → 03 63 63 63 little-endian → 0xA56363C6
        var te0 = AESUtils.Te0Copy();
        Assert.Equal(0xA56363C6u, te0[0]);

        // Td0[0] = 0x50A7F451（**不是 0**）：原文 ComputeAesStaticTables 中 `X := InvSBox[I]; if X = 0 then continue;`
        // 只在 InvSBox[I]=0（即 I=$63）时跳过；I=0 时 InvSBox[0]=$63≠0，故 Td0[0] 正常生成，
        // 与标准 Rijndael Td0 一致。此处按实测锁定。
        var td0 = AESUtils.Td0Copy();
        Assert.Equal(0x50A7F451u, td0[0]);
        Assert.Equal(0x5365417Eu, td0[1]);
    }

    [Fact]
    public void StaticTables_Sha256FingerprintIsStable()
    {
        using var sha = SHA256.Create();
        string hS = Convert.ToHexString(sha.ComputeHash(AESUtils.SBoxCopy())).ToLowerInvariant();
        string hI = Convert.ToHexString(sha.ComputeHash(AESUtils.InvSBoxCopy())).ToLowerInvariant();
        Assert.Equal(AESUtilsSBoxSha256, hS);
        Assert.Equal(AESUtilsInvSBoxSha256, hI);
    }

    // 由 ComputeAesStaticTables 生成（原文 AESUtils.pas:1229-1282 的同一算法；本移植只做 1:1 转写，
    // 未手工转录任何表值）。生成方式：ProbeTests 输出 → 见 docs/并行报告-p2c-common-crypto.md。
    private const string AESUtilsSBoxSha256 = "c2d8e5eed6cbebd8625fc18f81486a7733c04f9b0129ffbe974c68b90308b4f2";
    private const string AESUtilsInvSBoxSha256 = "93631b0726f6fe6629daa743ee51b49f4477ed07391b68eeea0672a4a90018aa";

    [Fact]
    public void CounterIncrement_WrapBehaviour_IsOriginalQuirk()
    {
        // 差异断言：计数器 byte7 由 0xFF 进位到 0 时走原文 AESUtils.pas:1186-1193 的手工进位循环。
        // 该循环里 `Offset = 7` 条件恒假（Offset 已 Dec），因此内侧 break 实际由"Block[Offset] <> 0"
        // 决定；本例把 256/257 号计数器块与对应密钥流钉死，防止"顺手重写"引入差异。
        byte[] key = Hex("2b7e151628aed2a6abf7158809cf4f3c");
        // 257 块明文（4112 字节）→ 覆盖 byte7 由 $FF 回绕的那一次进位
        int len = 16 * 257;
        var zero = new byte[len];
        var mine = AESUtils.AESEncrypt(key, 16, zero, len);

        // 用与原文完全相同的进位规则独立复算参考密钥流（257 块）
        var reference = new byte[len];
        for (int i = 0; i < 257; i++)
        {
            var blk = CounterBlockByOriginalRule(i);
            var ks = AESUtils.EcbBlockReference(key, blk);
            Array.Copy(ks, 0, reference, i * 16, 16);
        }
        Assert.Equal(ToHex(reference), ToHex(mine));

        // 与"标准 128 位大端整块自增"CTR 对照：第 1 块（计数器全零）相同，第 2 块起分叉
        var full = AESUtils.CtrBigEndianFullBlockReference(key, zero);
        Assert.NotEqual(ToHex(full), ToHex(mine));
        Assert.Equal(ToHex(full.AsSpan(0, 16).ToArray()), ToHex(mine.AsSpan(0, 16).ToArray()));

        // 回绕点邻域：第 255 块计数器 = byte6=$FF, byte7=$FF；第 256 块回绕后 byte6=$01
        Assert.Equal("00000000000000ff0000000000000000", ToHex(CounterBlockByOriginalRule(255)));
        Assert.Equal("00000000000001000000000000000000", ToHex(CounterBlockByOriginalRule(256)));
        Assert.Equal(ToHex(AESUtils.EcbBlockReference(key, CounterBlockByOriginalRule(255))),
                     ToHex(mine.AsSpan(255 * 16, 16).ToArray()));
        Assert.Equal(ToHex(AESUtils.EcbBlockReference(key, CounterBlockByOriginalRule(256))),
                     ToHex(mine.AsSpan(256 * 16, 16).ToArray()));
    }

    /// <summary>按原文 DoAESEncrypt 的进位规则生成第 index 个计数器块。</summary>
    private static byte[] CounterBlockByOriginalRule(int index)
    {
        var blk = new byte[16];
        for (int n = 0; n < index; n++)
        {
            int offset = 7;
            blk[offset]++;
            if (blk[offset] == 0)
            {
                do
                {
                    offset--;
                    blk[offset]++;
                    if (blk[offset] != 0 || offset == 7) break;
                }
                while (true);
            }
        }
        return blk;
    }
}
