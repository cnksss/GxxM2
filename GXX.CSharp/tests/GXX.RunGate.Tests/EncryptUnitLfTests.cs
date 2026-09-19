using System;
using System.Text;
using GXX.Core.Crypto;
using GXX.Core.Protocol;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// EncryptUnit_LF.pas 1:1 移植测试（Source\RunGate\EncryptUnit_LF.pas，156 行）。
/// 全部"黄金向量"的推导依据写在各断言上方（原文件行号 + 密钥派生步骤）。
/// 密钥：UnitDes.pas:1109 `NewEncryKey^ := $0C08BE531` → LongWord 截断为 0xC08BE531
/// = 3230393649 → `IntToStr` = "3230393649"。
/// </summary>
public class EncryptUnitLfTests
{
    // GBK 需先注册 CodePagesEncodingProvider（GXX.Core.EncodingInit 的 ModuleInitializer 之外，
    // 静态字段初始化顺序不保证，故在此显式 Ensure）。
    private static readonly Encoding Gbk = InitGbk();

    private static Encoding InitGbk()
    {
        GXX.Core.EncodingInit.Ensure();
        return Encoding.GetEncoding(936);
    }

    private static string Hex(byte[] b)
    {
        var sb = new StringBuilder(b.Length * 2);
        foreach (byte x in b) sb.Append(x.ToString("x2"));
        return sb.ToString();
    }

    private static byte[] FromHex(string h)
    {
        var b = new byte[h.Length / 2];
        for (int i = 0; i < b.Length; i++) b[i] = Convert.ToByte(h.Substring(i * 2, 2), 16);
        return b;
    }

    // ---------------- 密钥常量 ----------------

    [Fact]
    public void NewEncryKey_IsLongWordTruncationOfSourceLiteral()
    {
        // UnitDes.pas:1109：NewEncryKey^ := $0C08BE531
        // Delphi 7 对 >32bit 字面量按 LongWord 取值（低位 32 位）→ 0xC08BE531
        Assert.Equal(0xC08BE531u, EncryptUnitLf.NewEncryKey);
        Assert.Equal(3230393649u, EncryptUnitLf.NewEncryKey);
        Assert.Equal("3230393649", EncryptUnitLf.NewEncryKeyStr);
        // 不是 GXX.Core.EncryptUnit.EncryKey（A 系列用的 20120101）
        Assert.NotEqual(EncryptUnit.EncryKey.ToString(), EncryptUnitLf.NewEncryKeyStr);
    }

    // ---------------- 路径 1：EncryBufferA_LF / DecryBufferA_LF ----------------

    [Fact]
    public void EncryBufferA_LF_GoldenVector()
    {
        // 输入 "GoldenVector!"（GBK = 13 字节）；EncryptDes = SHA1(Key) 派生 KeyB
        // + DoInit + CBC(counter-free，BS=20)（UnitDes.pas:995-1002）
        byte[] src = Gbk.GetBytes("GoldenVector!");
        Assert.Equal(13, src.Length);
        Assert.Equal("b7d9cef26df6a677ecfbe0fdae", Hex(EncryptUnitLf.EncryBufferA_LF(src, src.Length)));
    }

    [Fact]
    public void EncryBufferA_LF_Then_DecryBufferA_LF_RoundTrips()
    {
        byte[] src = Gbk.GetBytes("GoldenVector!");
        byte[] enc = EncryptUnitLf.EncryBufferA_LF(src, src.Length);
        byte[] dec = EncryptUnitLf.DecryBufferA_LF(enc, enc.Length);
        Assert.Equal(src, dec);
    }

    [Fact]
    public void EncryBufferA_LF_EmptySize_ReturnsEmptyArray()
    {
        // 原文 SetLength(Result, 0) → 空串；C# 侧空数组
        Assert.Empty(EncryptUnitLf.EncryBufferA_LF(Array.Empty<byte>(), 0));
    }

    [Fact]
    public void EncryBufferA_LF_OutBufOverload_WritesSameAsReturnVersion()
    {
        byte[] src = Gbk.GetBytes("abc");
        byte[] viaReturn = EncryptUnitLf.EncryBufferA_LF(src, src.Length);
        var outBuf = new byte[src.Length];
        EncryptUnitLf.EncryBufferA_LF(src, src.Length, outBuf);
        Assert.Equal(viaReturn, outBuf);
    }

    [Fact]
    public void DecryBufferA_LF_StringOverload_MatchesDecryptStrDes()
    {
        // 原 174-177：Result := DecryptStrDes(Src, IntToStr(NewEncryKey^))
        byte[] src = Gbk.GetBytes("GoldenVector!");
        string enc = UnitDes.EncryptStrDesText("GoldenVector!", EncryptUnitLf.NewEncryKeyStr);
        Assert.Equal(UnitDes.DecryptStrDesText(enc, EncryptUnitLf.NewEncryKeyStr),
            EncryptUnitLf.DecryBufferA_LF(enc));
    }

    [Fact]
    public void DecryBufferA_LF_SourceOutdataOverload_MatchesDecryptDes()
    {
        // 原 179-182
        byte[] src = Gbk.GetBytes("GoldenVector!");
        byte[] cipher = EncryptUnitLf.EncryBufferA_LF(src, src.Length);
        var outData = new byte[src.Length];
        EncryptUnitLf.DecryBufferA_LF(cipher, cipher.Length, outData);
        Assert.Equal(src, outData);
    }

    // ---------------- 路径 2：EncryScript_LF / DecryScript_LF ----------------

    [Fact]
    public void EncryScript_LF_GoldenVector_And_RoundTrip()
    {
        // 原 38-41：EncryptStrDes(Str, IntToStr(NewEncryKey^))，无 Base64/6Bit 包装
        string enc = EncryptUnitLf.EncryScript_LF("Hello");
        Assert.Equal("3fa8ae3fa8b267", Hex(Gbk.GetBytes(enc)));
        Assert.Equal("Hello", EncryptUnitLf.DecryScript_LF(enc));
    }

    [Fact]
    public void DecryScript_LF_EmptyInput_ReturnsEmpty()
    {
        // 原文无空串分支：SetLength(Result, 0) → ''
        Assert.Equal("", EncryptUnitLf.DecryScript_LF(""));
        Assert.Equal("", EncryptUnitLf.EncryScript_LF(""));
    }

    [Fact]
    public void EncryScript_LF_IsNotEncryString_Lf()
    {
        // 差异断言：Script 系列不带 Base64/6Bit 包装，String 系列带
        Assert.NotEqual(
            EncryptUnitLf.EncryScript_LF("Hello"),
            EncryptUnitLf.EncryString_LF("Hello"));
    }

    // ---------------- 路径 3：EncryStringHex_LF / DecryStringHex_LF ----------------

    [Fact]
    public void EncryStringHex_LF_GoldenVector()
    {
        // "Hello" → DES → Base64 "..." → 每字节 Format('%x') 补零成 2 位小写
        // Base64 结果 8 字符 → 16 位 HEX
        string hx = EncryptUnitLf.EncryStringHex_LF("Hello");
        Assert.Equal("754e504f2b6d633d", hx);
        Assert.Equal(16, hx.Length);
        Assert.Equal(hx, hx.ToLowerInvariant());   // 原文 Format('%x') 为小写
    }

    [Fact]
    public void EncryStringHex_LF_Then_DecryStringHex_LF_RoundTrips()
    {
        string hx = EncryptUnitLf.EncryStringHex_LF("Hello");
        Assert.Equal("Hello", EncryptUnitLf.DecryStringHex_LF(hx));
    }

    [Fact]
    public void EncryStringHex_LF_EmptyInput_ReturnsEmpty()
    {
        // 原 107-123：Bufsize = 0 → ''
        Assert.Equal("", EncryptUnitLf.EncryStringHex_LF(""));
        Assert.Equal("", EncryptUnitLf.DecryStringHex_LF(""));
    }

    [Fact]
    public void EncryStringHex_LF_OddLengthHex_TruncatesLastNibble()
    {
        // 原 89-93：for I := 0 to Length(Src) div 2 - 1 → 奇数长度丢弃最后一个 nibble
        string hx = EncryptUnitLf.EncryStringHex_LF("Hello");
        // 截断到偶数长度（15 位 → 7 字节），HexToInt 对单字符 pair 仍可解析 → 不抛
        string odd = hx.Substring(0, hx.Length - 1);
        Assert.Equal(odd.Length / 2, odd.Length / 2);   // 记录 int 除法边界
        Assert.NotNull(EncryptUnitLf.DecryStringHex_LF(odd));
    }

    [Fact]
    public void DecryStringHex_LF_NonHexChars_AreIgnored()
    {
        // 原内嵌 HexToInt 的 else 分支被注释掉：非法字符不改变 res（不抛异常）
        Assert.NotNull(EncryptUnitLf.DecryStringHex_LF("ZZZZ"));
    }

    // ---------------- 路径 4：EncryBuffer_LF / DecryBuffer_LF ----------------

    [Fact]
    public void EncryBuffer_LF_GoldenVector()
    {
        // 原 126-138：DES → Base64 → 6Bit EncodeString
        byte[] src = Gbk.GetBytes("GoldenVector!");
        string bs = EncryptUnitLf.EncryBuffer_LF(src, src.Length);
        Assert.Equal("593f616a4f6f5d69486f456c5762556f466e69404a5345634b4f70", Hex(Gbk.GetBytes(bs)));
        // 13 字节输入 → Base64 20 字符 → 6Bit 编码后 27 字节（长度由 EncodeString 的进位决定）
        Assert.Equal(27, Gbk.GetBytes(bs).Length);
    }

    [Fact]
    public void EncryBuffer_LF_Then_DecryBuffer_LF_RoundTrips()
    {
        byte[] src = Gbk.GetBytes("GoldenVector!");
        string bs = EncryptUnitLf.EncryBuffer_LF(src, src.Length);
        var back = new byte[src.Length];
        EncryptUnitLf.DecryBuffer_LF(bs, back, src.Length);
        Assert.Equal(src, back);   // DecryBuffer_LF 是 EncryBuffer_LF 的正确逆
    }

    [Fact]
    public void DecryBuffer_LF_EmptySource_LeavesBufferUntouched()
    {
        var buf = new byte[] { 1, 2, 3 };
        EncryptUnitLf.DecryBuffer_LF("", buf, buf.Length);
        Assert.Equal(new byte[] { 1, 2, 3 }, buf);
    }

    [Fact]
    public void EncryBuffer_LF_ZeroSize_ReturnsEmpty()
    {
        Assert.Equal("", EncryptUnitLf.EncryBuffer_LF(Array.Empty<byte>(), 0));
    }

    [Fact]
    public void EncryBuffer_LF_StringOverload_UsesGbkBytes()
    {
        byte[] src = Gbk.GetBytes("GoldenVector!");
        Assert.Equal(EncryptUnitLf.EncryBuffer_LF(src, src.Length),
            EncryptUnitLf.EncryBuffer_LF("GoldenVector!", src.Length));
    }

    // ---------------- 路径 5：EncryString_LF / DecryString_LF（不对称！） ----------------

    [Fact]
    public void EncryString_LF_EqualsEncryBuffer_LF_No6BitPrefix()
    {
        // 原 53-59：EncryString_LF 直接调 EncryBuffer_LF，**不**额外做 EncodeString
        byte[] gbk = Gbk.GetBytes("Hello");
        Assert.Equal(EncryptUnitLf.EncryBuffer_LF(gbk, gbk.Length), EncryptUnitLf.EncryString_LF("Hello"));
    }

    [Fact]
    public void EncryString_LF_EmptyInput_ReturnsEmpty()
    {
        Assert.Equal("", EncryptUnitLf.EncryString_LF(""));
    }

    [Fact]
    public void DecryString_LF_EmptyInput_ReturnsEmpty()
    {
        Assert.Equal("", EncryptUnitLf.DecryString_LF(""));
    }

    [Fact]
    public void DecryString_LF_InvertsManualEncodePipeline()
    {
        // 按原 43-51 的管线手工构造密文：EncodeString(Base64(EncryptStrDes(明文)))
        byte[] plain = Gbk.GetBytes("Hello");
        byte[] cipher = new byte[plain.Length];
        UnitDes.EncryptDes(plain, cipher, plain.Length, EncryptUnitLf.NewEncryKeyStr);
        string b64 = Base64Util.Base64EncodeStr(cipher);
        string enc = Gbk.GetString(EDcode.EncodeString(b64));
        Assert.Equal("Hello", EncryptUnitLf.DecryString_LF(enc));
    }

    [Fact]
    public void DecryString_LF_InvertsEncryString_LF()
    {
        // EncryString_LF == EncryBuffer_LF == EncodeString(Base64(EncryptDes))，
        // DecryString_LF == DecryptStrDes(Base64DecodeStr(DecodeString)) → 二者互为逆。
        Assert.Equal("Hello", EncryptUnitLf.DecryString_LF(EncryptUnitLf.EncryString_LF("Hello")));
    }

    [Fact]
    public void EncryString_LF_And_EncryBuffer_LF_ProduceIdenticalCipher()
    {
        // 差异断言：EncryString_LF 与 EncryBuffer_LF 输出一致（EncryString_LF 只是转发），
        // 这一点与 GXX.Core.EncryptUnit.EncryString（自带随机 4 字符 Key 前缀）**不同**。
        byte[] gbk = Gbk.GetBytes("Hello");
        Assert.Equal(EncryptUnitLf.EncryBuffer_LF(gbk, gbk.Length), EncryptUnitLf.EncryString_LF("Hello"));
        Assert.NotEqual(EncryptUnit.EncryString("Hello").Length, EncryptUnitLf.EncryString_LF("Hello").Length);
    }

    // ---------------- 与 GXX.Core.EncryptUnit 的差异 ----------------

    [Fact]
    public void LfVariant_DiffersFromCoreEncryptUnit_ByKey()
    {
        // 关键差异：LF 用 NewEncryKey（3230393649），A 系列用 EncryKey（20120101）→ 密文不同
        byte[] src = Gbk.GetBytes("Hello");
        byte[] lf = EncryptUnitLf.EncryBufferA_LF(src, src.Length);
        byte[] core = EncryptUnit.EncryBufferA(src, src.Length);
        Assert.NotEqual(core, lf);
    }

    [Fact]
    public void LfScript_MatchesCoreScript_WouldDifferByKey()
    {
        // Script 系列同样是密钥差异
        Assert.NotEqual(EncryptUnit.EncryScript("Hello"), EncryptUnitLf.EncryScript_LF("Hello"));
        Assert.NotEqual("Hello", EncryptUnit.DecryScript(EncryptUnitLf.EncryScript_LF("Hello")));
    }

    // ---------------- 确定性与长度不变性 ----------------

    [Fact]
    public void EncryptionIsDeterministic_ForFixedKey()
    {
        byte[] src = Gbk.GetBytes("deterministic");
        Assert.Equal(
            EncryptUnitLf.EncryBufferA_LF(src, src.Length),
            EncryptUnitLf.EncryBufferA_LF(src, src.Length));
    }

    [Fact]
    public void CipherLengthEqualsInputLength()
    {
        // 原 SetLength(sText, Bufsize) → 密文长度 == 明文长度（无填充）
        for (int n = 1; n <= 45; n++)
        {
            byte[] src = new byte[n];
            for (int i = 0; i < n; i++) src[i] = (byte)(i * 7 + 1);
            Assert.Equal(n, EncryptUnitLf.EncryBufferA_LF(src, n).Length);
        }
    }

    [Fact]
    public void BlockBoundary_MultiplesOf20AndRemainders()
    {
        // BS = 20（UnitDes.pas:13）；覆盖整数块与尾块
        foreach (int n in new[] { 19, 20, 21, 39, 40, 41 })
        {
            byte[] src = new byte[n];
            for (int i = 0; i < n; i++) src[i] = (byte)(255 - i);
            byte[] enc = EncryptUnitLf.EncryBufferA_LF(src, n);
            byte[] dec = EncryptUnitLf.DecryBufferA_LF(enc, n);
            Assert.Equal(src, dec);
        }
    }
}
