using System;
using System.Text;
using GXX.Core.Crypto;
using GXX.Core.Rtl;
using Xunit;

namespace GXX.Core.Tests;

/// <summary>UnitDes 自研 DES-CBC（BS=20）测试。</summary>
public class UnitDesTests
{
    [Fact]
    public void EncryptDecrypt_RoundTrip()
    {
        string key = "12345678";
        byte[] plain = new byte[200];
        new Random(9).NextBytes(plain);
        byte[] cipher = new byte[plain.Length];
        UnitDes.EncryptDes(plain, cipher, plain.Length, key);
        Assert.False(plain.AsSpan().SequenceEqual(cipher), "密文不应与明文相同");
        byte[] back = new byte[plain.Length];
        UnitDes.DecryptDes(cipher, back, plain.Length, key);
        Assert.True(plain.AsSpan().SequenceEqual(back));
    }

    [Fact]
    public void EncryptDecrypt_Text()
    {
        string key = "20120101";
        string text = "传奇服务器测试 Legendary Mir Test Data 12345";
        byte[] enc = UnitDes.EncryptStrDesBytes(EncodingInit.GBK.GetBytes(text), key);
        byte[] dec = UnitDes.DecryptStrDes(enc, key);
        Assert.Equal(text, EncodingInit.GBK.GetString(dec));
    }

    [Fact]
    public void EncryptStrDes_Deterministic()
    {
        // GetKeyData 由 SHA-1(Key) 派生 → 相同 Key+明文 必须产生相同密文（CBC 链 IV 固定 $8F）
        byte[] a = UnitDes.EncryptStrDesBytes(EncodingInit.GBK.GetBytes("determinism"), "key1");
        byte[] b = UnitDes.EncryptStrDesBytes(EncodingInit.GBK.GetBytes("determinism"), "key1");
        Assert.True(a.AsSpan().SequenceEqual(b));
    }

    [Fact]
    public void CBCPartialTail_UsesEncryptedChain()
    {
        // 尾部不足 20 字节时按链值异或 → 长度无关紧要，往返必须一致
        for (int len = 1; len <= 45; len++)
        {
            byte[] plain = new byte[len];
            new Random(len).NextBytes(plain);
            byte[] cipher = new byte[len];
            UnitDes.EncryptDes(plain, cipher, len, "tail");
            byte[] back = new byte[len];
            UnitDes.DecryptDes(cipher, back, len, "tail");
            Assert.True(plain.AsSpan().SequenceEqual(back), $"len={len}");
        }
    }

    [Fact]
    public void Hash_MatchesSha1()
    {
        byte[] digest = new byte[20];
        UnitDes.Hash("abc", digest);
        Assert.Equal("a9993e364706816aba3e25717850c26c9cd0d89d", BitConverter.ToString(digest).Replace("-", "").ToLowerInvariant());
    }

    [Fact]
    public void CRC32_StandardVector()
    {
        // CRC-32 标准向量："123456789" → 0xCBF43926
        byte[] data = Encoding.ASCII.GetBytes("123456789");
        Assert.Equal(0xCBF43926u, UnitDes.CalcCrc32(data, 0, data.Length));
    }
}

/// <summary>EncryptUnit 组合加解密测试。</summary>
public class EncryptUnitTests
{
    [Fact]
    public void EncryDecryString_RoundTrip()
    {
        string text = "测试账号 testuser1";
        string enc = EncryptUnit.EncryString(text);
        Assert.True(enc.Length > 8, "前缀2字符 + 密文 + 后缀2字符");
        Assert.Equal(text, EncryptUnit.DecryString(enc));
    }

    [Fact]
    public void EncryDecryStringK_RoundTrip()
    {
        string text = "Hello 传奇 K-Series";
        string enc = EncryptUnit.EncryStringK(text, "mykey123");
        Assert.Equal(text, EncryptUnit.DecryStringK(enc, "mykey123"));
    }

    [Fact]
    public void zEncryDecryStringK_RoundTrip()
    {
        string text = "压缩版本 z-Series 数据 Compression Test";
        string enc = EncryptUnit.zEncryStringK(text, "zk");
        Assert.Equal(text, EncryptUnit.zDecryStringK(enc, "zk"));
    }

    [Fact]
    public void EncryDecryScript()
    {
        string text = "NPC脚本行 CHECKGOLD 1000";
        string enc = EncryptUnit.EncryScript(text);
        Assert.Equal(text, EncryptUnit.DecryScript(enc));
    }

    [Fact]
    public void GetKeyValue_4Chars()
    {
        // 前提：每个字节必须 < 58（Delphi Random(58) 保证；越界在原版中同样未定义）
        string s = EncryptUnit.GetKeyValue(0x0D0C0B0A);
        Assert.Equal(4, s.Length);
    }

    [Fact]
    public void EncryStrHex_RoundTrip()
    {
        string text = "AB中文";
        string hex = EncryptUnit.EncryStrHex(text);
        Assert.Equal(text, EncryptUnit.DecryStrHex(hex));
    }

    [Fact]
    public void Base64_RoundTrip()
    {
        byte[] data = Encoding.UTF8.GetBytes("Man is distinguished, not only by his reason");
        string b64 = Base64Util.Base64EncodeStr(data);
        Assert.True(data.AsSpan().SequenceEqual(Base64Util.Base64DecodeStr(b64)));
        // 标准向量
        Assert.Equal("TWFu", Base64Util.Base64EncodeStr(Encoding.ASCII.GetBytes("Man")));
    }

    [Fact]
    public void MD5_StandardVector()
    {
        Assert.Equal("900150983cd24fb0d6963f7d28e17f72", MD5Util.MD5String("abc"));
    }
}
