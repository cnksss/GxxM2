using System;
using System.Collections.Generic;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// ParadoxConv.pas 1:1 移植测试（原 ParadoxConv.pas:1-1104）。
/// 覆盖：UCS4 四字节大端语义、各代码页往返、边界/异常分支、原文"无 else 静默吞字节"
/// 与"Encoding() 缺 CP866 源分支"两处缺陷的差异断言。
/// 另含"映射表完整性校验和"测试 —— 对全部 7 张表做全量遍历并比对从原文抽取的
/// 期望校验和，防止脚本生成/手工改动引入偏差。
/// </summary>
public class ParadoxConvTests
{
    // ---------------- 助手 ----------------

    /// <summary>UCS4 大端四字节（BMP 且高两字节为 0）。</summary>
    private static byte[] Ucs4Bmp(byte hi, byte lo) => new byte[] { 0x00, 0x00, hi, lo };

    /// <summary>UCS4 大端四字节（完整 32 位）。</summary>
    private static byte[] Ucs4(uint v) => new byte[]
    {
        (byte)(v >> 24), (byte)(v >> 16), (byte)(v >> 8), (byte)v
    };

    private static byte[] Latin1(params byte[] b) => b;

    // ---------------- Encoding() 分派 ----------------

    [Fact]
    public void GetCodepage_HasWindowsPrefixFormat()
    {
        // 原 ParadoxConv.pas:82-92：'CP' + IntToStr(GetACP)
        string cp = ParadoxConv.GetCodepage();
        Assert.StartsWith("CP", cp, StringComparison.Ordinal);
        int acp;
        Assert.True(int.TryParse(cp.Substring(2), out acp));
        Assert.True(acp > 0);
    }

    [Fact]
    public void Encoding_Cp1251ToUcs4_KnownVectors()
    {
        // 原 306-374：$80 → #$04#$02（U+0402）, 'A' → #$00#$41
        Assert.Equal(Latin1(0x00, 0x00, 0x04, 0x02), ParadoxConv.Encoding(TEncodingKind.CP1251, TEncodingKind.UCS4, Latin1(0x80)));
        Assert.Equal(Latin1(0x00, 0x00, 0x00, 0x41), ParadoxConv.Encoding(TEncodingKind.CP1251, TEncodingKind.UCS4, Latin1(0x41)));
        // $C0 → #$04 + AnsiChar($C0-$B0) = #$04#$10
        Assert.Equal(Latin1(0x00, 0x00, 0x04, 0x10), ParadoxConv.Encoding(TEncodingKind.CP1251, TEncodingKind.UCS4, Latin1(0xC0)));
        // $FF → #$04#$4F
        Assert.Equal(Latin1(0x00, 0x00, 0x04, 0x4F), ParadoxConv.Encoding(TEncodingKind.CP1251, TEncodingKind.UCS4, Latin1(0xFF)));
    }

    [Fact]
    public void Encoding_Cp1251ToUcs4_UndefinedByte0x98_SilentlyEmitsTwoZeros()
    {
        // 原文缺陷差异断言：Cp1251ToUCS4 的 case **没有 else**，
        // 字节 $98 未列出 → 保留已追加的 #0#0 后什么都不做（ParadoxConv.pas:297-376）
        byte[] r = ParadoxConv.Cp1251ToUCS4(Latin1(0x98));
        Assert.Equal(new byte[] { 0x00, 0x00 }, r);
    }

    [Fact]
    public void Encoding_SourceCp866_AlwaysEmpty()
    {
        // 原文缺陷差异断言：Encoding() 的 CP866 源分支被整段注释掉（ParadoxConv.pas:155-164）
        Assert.Empty(ParadoxConv.Encoding(TEncodingKind.CP866, TEncodingKind.UTF8, Latin1(0x80)));
        Assert.Empty(ParadoxConv.Encoding(TEncodingKind.CP866, TEncodingKind.UCS4, Latin1(0x80)));
        Assert.Empty(ParadoxConv.Encoding(TEncodingKind.CP866, TEncodingKind.CP1251, Latin1(0x80)));
    }

    [Fact]
    public void Encoding_Ucs4ToSameEncoding_AllFiveTargets()
    {
        // CP1251 侧：U+0402 → $80
        byte[] src = Ucs4Bmp(0x04, 0x02);
        Assert.Equal(Latin1(0x80), ParadoxConv.Encoding(TEncodingKind.UCS4, TEncodingKind.CP1251, src));
        Assert.Empty(ParadoxConv.Encoding(TEncodingKind.UCS4, TEncodingKind.UCS4, src));   // 无同码分支 → 空
        Assert.Equal(new byte[] { 0xD0, 0x82 }, ParadoxConv.Encoding(TEncodingKind.UCS4, TEncodingKind.UTF8, src));
        Assert.Single(ParadoxConv.Encoding(TEncodingKind.UCS4, TEncodingKind.ISO88595, src));
        // CP866 的 #$04#$02 无映射（原文 else → raise）
        Assert.Throws<EConvException>(() => ParadoxConv.Encoding(TEncodingKind.UCS4, TEncodingKind.CP866, src));

        // KOI8-R 的 #$04 group 无 $02（原文缺口），此处用 U+0041（五种编码皆可映射）验证 KOI8-R 目标
        byte[] ascii = Ucs4Bmp(0x00, 0x41);
        Assert.Equal(new byte[] { 0x41 }, ParadoxConv.Encoding(TEncodingKind.UCS4, TEncodingKind.KOI8R, ascii));
        Assert.Throws<EConvException>(() => ParadoxConv.Encoding(TEncodingKind.UCS4, TEncodingKind.KOI8R, src));
    }

    [Fact]
    public void Encoding_Ucs4ToUcs4_HasNoBranch_ReturnsEmpty()
    {
        // 原文差异断言：case Dest of 里没有 UCS4 项 → 返回 ''
        Assert.Empty(ParadoxConv.Encoding(TEncodingKind.UCS4, TEncodingKind.UCS4, Ucs4Bmp(0x04, 0x02)));
        Assert.Empty(ParadoxConv.Encoding(TEncodingKind.UTF8, TEncodingKind.UTF8, Latin1(0x41)));
        Assert.Empty(ParadoxConv.Encoding(TEncodingKind.KOI8R, TEncodingKind.KOI8R, Latin1(0x41)));
        Assert.Empty(ParadoxConv.Encoding(TEncodingKind.ISO88595, TEncodingKind.ISO88595, Latin1(0x41)));
        Assert.Empty(ParadoxConv.Encoding(TEncodingKind.CP1251, TEncodingKind.CP1251, Latin1(0x41)));
    }

    // ---------------- UTF-8 ↔ UCS4 ----------------

    [Fact]
    public void Utf8ToUCS4_AsciiAndMultiByte()
    {
        // 'A' → #$00#$00#$00#$41
        Assert.Equal(Latin1(0x00, 0x00, 0x00, 0x41), ParadoxConv.Utf8ToUCS4(Latin1(0x41)));
        // U+00A9 (©) = C2 A9 → #$00#$00#$00#$A9
        Assert.Equal(Latin1(0x00, 0x00, 0x00, 0xA9), ParadoxConv.Utf8ToUCS4(Latin1(0xC2, 0xA9)));
        // U+4E2D (中) = E4 B8 AD → #$00#$00#$4E#$2D
        Assert.Equal(Latin1(0x00, 0x00, 0x4E, 0x2D), ParadoxConv.Utf8ToUCS4(Latin1(0xE4, 0xB8, 0xAD)));
        // U+10348 = F0 90 8D 88 → #$00#$01#$03#$48
        Assert.Equal(Latin1(0x00, 0x01, 0x03, 0x48), ParadoxConv.Utf8ToUCS4(Latin1(0xF0, 0x90, 0x8D, 0x88)));
    }

    [Fact]
    public void Utf8ToUCS4_TruncatedSequence_Throws()
    {
        // 原 196-197：Length(S) - I < Count - 1 → eConvException
        var ex = Assert.Throws<EConvException>(() => ParadoxConv.Utf8ToUCS4(Latin1(0xE4, 0xB8)));
        Assert.Equal("Utf8", ex.SrcEncoding);
        Assert.Equal("UCS4", ex.DstEncoding);
    }

    [Fact]
    public void Utf8ToUCS4_BadContinuationByte_Throws()
    {
        // 原 201-202：continuation 必须 (b and $C0) = $80
        var ex = Assert.Throws<EConvException>(() => ParadoxConv.Utf8ToUCS4(Latin1(0xE4, 0x41, 0xAD)));
        Assert.Equal("Utf8", ex.SrcEncoding);
    }

    [Fact]
    public void Utf8ToUCS4_EmptyInput_ReturnsEmpty()
    {
        Assert.Empty(ParadoxConv.Utf8ToUCS4(Array.Empty<byte>()));
    }

    [Fact]
    public void UCS4ToUtf8_LengthValidation()
    {
        // 原 223-224：Length < 4 或 mod 4 <> 0 → 抛
        Assert.Empty(ParadoxConv.UCS4ToUtf8(Array.Empty<byte>()));
        Assert.Throws<EConvException>(() => ParadoxConv.UCS4ToUtf8(Latin1(0, 0, 4)));
        Assert.Throws<EConvException>(() => ParadoxConv.UCS4ToUtf8(Latin1(0, 0, 4, 2, 0)));
    }

    [Fact]
    public void UCS4ToUtf8_BoundariesOfCountSelection()
    {
        // 原 242-253 的分档：$7F/1、$80/2、$7FF/2、$800/3、$FFFF/3、$10000/4、$1FFFFF/4、$200000/5
        Assert.Equal(new byte[] { 0x7F }, ParadoxConv.UCS4ToUtf8(Ucs4(0x7F)));
        Assert.Equal(new byte[] { 0xC2, 0x80 }, ParadoxConv.UCS4ToUtf8(Ucs4(0x80)));
        Assert.Equal(new byte[] { 0xDF, 0xBF }, ParadoxConv.UCS4ToUtf8(Ucs4(0x7FF)));
        Assert.Equal(new byte[] { 0xE0, 0xA0, 0x80 }, ParadoxConv.UCS4ToUtf8(Ucs4(0x800)));
        Assert.Equal(new byte[] { 0xEF, 0xBF, 0xBF }, ParadoxConv.UCS4ToUtf8(Ucs4(0xFFFF)));
        Assert.Equal(new byte[] { 0xF0, 0x90, 0x80, 0x80 }, ParadoxConv.UCS4ToUtf8(Ucs4(0x10000)));
        Assert.Equal(new byte[] { 0xF7, 0xBF, 0xBF, 0xBF }, ParadoxConv.UCS4ToUtf8(Ucs4(0x1FFFFF)));
        Assert.Equal(new byte[] { 0xF8, 0x88, 0x80, 0x80, 0x80 }, ParadoxConv.UCS4ToUtf8(Ucs4(0x200000)));
    }

    [Fact]
    public void Utf8Ucs4_RoundTrip_AllKnownCodePoints()
    {
        var samples = new List<uint> { 0x00, 0x41, 0x7F, 0x80, 0xA9, 0x7FF, 0x800, 0x4E2D, 0xFFFF, 0x10000, 0x10348, 0x1FFFFF };
        foreach (uint v in samples)
        {
            byte[] utf8 = ParadoxConv.UCS4ToUtf8(Ucs4(v));
            byte[] back = ParadoxConv.Utf8ToUCS4(utf8);
            Assert.Equal(Ucs4(v), back);
        }
    }

    // ---------------- CP1251 往返 ----------------

    [Fact]
    public void Cp1251_RoundTrip_AllExcept0x98()
    {
        // $00..$7F、$80..$97、$99..$FF 可往返；$98 因原文无 else 而丢失（差异断言）
        int ok = 0;
        for (int b = 0; b <= 0xFF; b++)
        {
            byte[] ucs4 = ParadoxConv.Cp1251ToUCS4(Latin1((byte)b));
            if (b == 0x98)
            {
                Assert.Equal(new byte[] { 0, 0 }, ucs4);
                continue;
            }
            byte[] back;
            try { back = ParadoxConv.UCS4ToCp1251(ucs4); }
            catch (EConvException) { continue; }   // 该 CP1251 字节映射到的 UCS4 码位无反向映射
            Assert.Equal(new byte[] { (byte)b }, back);
            ok++;
        }
        Assert.Equal(255, ok);   //  之外全部可往返
    }

    [Fact]
    public void UCS4ToCp1251_HighBytesNonZero_ThrowsWithFullSymbol()
    {
        // 原 390-391：S[I] <> #0 or S[I+1] <> #0 → 抛，Symbol 为 4 字节 hex 拼接
        var ex = Assert.Throws<EConvException>(() => ParadoxConv.UCS4ToCp1251(new byte[] { 0x00, 0x01, 0x00, 0x41 }));
        Assert.Equal("UCS4", ex.SrcEncoding);
        Assert.Equal("Cp1251", ex.DstEncoding);
        Assert.Equal("$00010041", ex.Symbol);
    }

    [Fact]
    public void UCS4ToCp1251_UnmappedCodePoint_Throws()
    {
        // #$00#$00#$00#$98 不在表内（原文 else → raise）
        var ex = Assert.Throws<EConvException>(() => ParadoxConv.UCS4ToCp1251(Ucs4Bmp(0x00, 0x98)));
        Assert.Equal("UCS4", ex.SrcEncoding);
    }

    [Fact]
    public void UCS4ToCp1251_LengthValidation()
    {
        Assert.Empty(ParadoxConv.UCS4ToCp1251(Array.Empty<byte>()));
        Assert.Throws<EConvException>(() => ParadoxConv.UCS4ToCp1251(Latin1(0, 0, 0)));
    }

    [Fact]
    public void UCS4ToCp1251_ProducesOneBytePerCodePoint()
    {
        byte[] input = new byte[16];
        // 四个 BMP 码位：$0410('А')、$0411、$0412、$0413（CP1251 $C0..$C3）
        input[2] = 0x04; input[3] = 0x10;
        input[6] = 0x04; input[7] = 0x11;
        input[10] = 0x04; input[11] = 0x12;
        input[14] = 0x04; input[15] = 0x13;
        Assert.Equal(new byte[] { 0xC0, 0xC1, 0xC2, 0xC3 }, ParadoxConv.UCS4ToCp1251(input));
    }

    // ---------------- KOI8-R ----------------

    [Fact]
    public void Koi8r_KnownVectors()
    {
        // 原 700-842：$80 → #$25#$00（U+2500）, $C1 → #$04#$30（U+0430 'а'）
        Assert.Equal(Latin1(0x00, 0x00, 0x25, 0x00), ParadoxConv.Koi8rToUCS4(Latin1(0x80)));
        Assert.Equal(Latin1(0x00, 0x00, 0x04, 0x30), ParadoxConv.Koi8rToUCS4(Latin1(0xC1)));
        Assert.Equal(Latin1(0x00, 0x00, 0x00, 0x41), ParadoxConv.Koi8rToUCS4(Latin1(0x41)));
    }

    [Fact]
    public void Koi8r_RoundTrip_All256Bytes()
    {
        for (int b = 0; b <= 0xFF; b++)
        {
            byte[] ucs4 = ParadoxConv.Koi8rToUCS4(Latin1((byte)b));
            Assert.Equal(new byte[] { (byte)b }, ParadoxConv.UCS4ToKoi8r(ucs4));
        }
    }

    [Fact]
    public void Koi8r_TableCoversAll256Bytes()
    {
        // 原文 case 无 else 且 256 字节全覆盖：任何字节都不应抛异常
        for (int b = 0; b <= 0xFF; b++)
        {
            byte[] r = ParadoxConv.Koi8rToUCS4(Latin1((byte)b));
            Assert.Equal(4, r.Length);
        }
    }

    [Fact]
    public void UCS4ToKoi8r_Unmapped_Throws()
    {
        Assert.Throws<EConvException>(() => ParadoxConv.UCS4ToKoi8r(Ucs4Bmp(0x00, 0x98)));
    }

    [Fact]
    public void UCS4ToKoi8r_IdentityRange_CopiesLowByte()
    {
        // 原 862：#$00..#$7F: Result := Result + S[I + 3]（恒等）
        Assert.Equal(new byte[] { 0x00 }, ParadoxConv.UCS4ToKoi8r(Ucs4Bmp(0x00, 0x00)));
        Assert.Equal(new byte[] { 0x7F }, ParadoxConv.UCS4ToKoi8r(Ucs4Bmp(0x00, 0x7F)));
    }

    // ---------------- ISO 8859-5 ----------------

    [Fact]
    public void Iso88595_KnownVectors()
    {
        // 原 1030-1050：$A1..#$AC → #$04 + (b-$A0)；$AD → #$00#$AD；$F0 → #$21#$16
        Assert.Equal(Latin1(0x00, 0x00, 0x04, 0x01), ParadoxConv.ISO88595ToUCS4(Latin1(0xA1)));
        Assert.Equal(Latin1(0x00, 0x00, 0x00, 0xAD), ParadoxConv.ISO88595ToUCS4(Latin1(0xAD)));
        Assert.Equal(Latin1(0x00, 0x00, 0x21, 0x16), ParadoxConv.ISO88595ToUCS4(Latin1(0xF0)));
        Assert.Equal(Latin1(0x00, 0x00, 0x00, 0xA0), ParadoxConv.ISO88595ToUCS4(Latin1(0xA0)));
        Assert.Equal(Latin1(0x00, 0x00, 0x04, 0x5F), ParadoxConv.ISO88595ToUCS4(Latin1(0xFF)));
        Assert.Equal(Latin1(0x00, 0x00, 0x00, 0xA7), ParadoxConv.ISO88595ToUCS4(Latin1(0xFD)));
    }

    [Fact]
    public void Iso88595_RoundTrip_All256Bytes()
    {
        for (int b = 0; b <= 0xFF; b++)
        {
            byte[] ucs4 = ParadoxConv.ISO88595ToUCS4(Latin1((byte)b));
            Assert.Equal(new byte[] { (byte)b }, ParadoxConv.UCS4ToISO88595(ucs4));
        }
    }

    [Fact]
    public void UCS4ToIso88595_ErrorEncodingNameHasSpace()
    {
        // 原 1058-1059：CreateConvException('UCS4', 'ISO 8859-5', ...)
        var ex = Assert.Throws<EConvException>(() => ParadoxConv.UCS4ToISO88595(Latin1(0, 0, 0)));
        Assert.Equal("UCS4", ex.SrcEncoding);
        Assert.Equal("ISO 8859-5", ex.DstEncoding);
    }

    [Fact]
    public void UCS4ToIso88595_Unmapped_Throws()
    {
        // ISO 8859-5 的 #$00 group 覆盖 #$00..#$A0 与 #$AD/#$A7，缺口如 #$B1；原文 else → raise
        Assert.Throws<EConvException>(() => ParadoxConv.UCS4ToISO88595(Ucs4Bmp(0x00, 0xB1)));
    }

    // ---------------- CP866 ----------------

    [Fact]
    public void Ucs4ToCp866_KnownVectors()
    {
        // 原 591-601：#$00..#$7F 恒等；$B0 → $F8；$A4 → $FD；$A0 → $FF
        Assert.Equal(new byte[] { 0x00 }, ParadoxConv.UCS4ToCp866(Ucs4Bmp(0x00, 0x00)));
        Assert.Equal(new byte[] { 0x41 }, ParadoxConv.UCS4ToCp866(Ucs4Bmp(0x00, 0x41)));
        Assert.Equal(new byte[] { 0xF8 }, ParadoxConv.UCS4ToCp866(Ucs4Bmp(0x00, 0xB0)));
        Assert.Equal(new byte[] { 0xFA }, ParadoxConv.UCS4ToCp866(Ucs4Bmp(0x00, 0xB7)));
        Assert.Equal(new byte[] { 0xFD }, ParadoxConv.UCS4ToCp866(Ucs4Bmp(0x00, 0xA4)));
        Assert.Equal(new byte[] { 0xFF }, ParadoxConv.UCS4ToCp866(Ucs4Bmp(0x00, 0xA0)));
        // #$04#$10..#$3F → +$70；#$04#$40..#$4F → +$A0
        Assert.Equal(new byte[] { 0x80 }, ParadoxConv.UCS4ToCp866(Ucs4Bmp(0x04, 0x10)));
        Assert.Equal(new byte[] { 0xAF }, ParadoxConv.UCS4ToCp866(Ucs4Bmp(0x04, 0x3F)));
        Assert.Equal(new byte[] { 0xE0 }, ParadoxConv.UCS4ToCp866(Ucs4Bmp(0x04, 0x40)));
        Assert.Equal(new byte[] { 0xEF }, ParadoxConv.UCS4ToCp866(Ucs4Bmp(0x04, 0x4F)));
    }

    [Fact]
    public void Ucs4ToCp866_BoxDrawingRange()
    {
        // #$25#$91 → $B0 ... #$25#$A0 → $FE（原 640-688）
        Assert.Equal(new byte[] { 0xB0 }, ParadoxConv.UCS4ToCp866(Ucs4Bmp(0x25, 0x91)));
        Assert.Equal(new byte[] { 0xB3 }, ParadoxConv.UCS4ToCp866(Ucs4Bmp(0x25, 0x02)));
        Assert.Equal(new byte[] { 0xC4 }, ParadoxConv.UCS4ToCp866(Ucs4Bmp(0x25, 0x00)));
        Assert.Equal(new byte[] { 0xFE }, ParadoxConv.UCS4ToCp866(Ucs4Bmp(0x25, 0xA0)));
    }

    [Fact]
    public void Ucs4ToCp866_Unmapped_Throws()
    {
        Assert.Throws<EConvException>(() => ParadoxConv.UCS4ToCp866(Ucs4Bmp(0x00, 0x98)));
        Assert.Throws<EConvException>(() => ParadoxConv.UCS4ToCp866(Ucs4Bmp(0x04, 0x00)));
    }

    [Fact]
    public void Ucs4ToCp866_MultipleCodePointsInOneBuffer()
    {
        byte[] input = new byte[8];
        input[2] = 0x00; input[3] = 0x41;   // 'A'
        input[6] = 0x00; input[7] = 0xB0;   // °
        Assert.Equal(new byte[] { 0x41, 0xF8 }, ParadoxConv.UCS4ToCp866(input));
    }

    [Fact]
    public void Cp866ToUcs4_IsNotProvided_InSource()
    {
        // 原文 Cp866ToUCS4 整段被注释（ParadoxConv.pas:492-574），接口不导出。
        // 用反射确认类型上不存在该方法（防止后续误加）。
        Assert.Null(typeof(ParadoxConv).GetMethod("Cp866ToUCS4"));
    }

    // ---------------- 交叉代码页 ----------------

    [Fact]
    public void Encoding_Cp1251ToKoi8r_MatchesComposition()
    {
        // 原 148-153：CP1251→KOI8R = UCS4ToKoi8r(Cp1251ToUCS4(S))
        for (int b = 0; b <= 0xFF; b++)
        {
            if (b == 0x98) continue;
            byte[] viaEncoding = TryOrNull(() => ParadoxConv.Encoding(TEncodingKind.CP1251, TEncodingKind.KOI8R, Latin1((byte)b)));
            byte[] composed = TryOrNull(() => ParadoxConv.UCS4ToKoi8r(ParadoxConv.Cp1251ToUCS4(Latin1((byte)b))));
            Assert.Equal(composed, viaEncoding);   // 两条路径必须同成败/同结果（含共同抛错的码位）
        }
    }

    [Fact]
    public void Encoding_Cp1251ToUtf8_MatchesComposition()
    {
        for (int b = 0; b <= 0xFF; b++)
        {
            if (b == 0x98) continue;
            byte[] viaEncoding = TryOrNull(() => ParadoxConv.Encoding(TEncodingKind.CP1251, TEncodingKind.UTF8, Latin1((byte)b)));
            byte[] composed = TryOrNull(() => ParadoxConv.UCS4ToUtf8(ParadoxConv.Cp1251ToUCS4(Latin1((byte)b))));
            Assert.Equal(composed, viaEncoding);
        }
    }

    [Fact]
    public void Encoding_Utf8ToCp1251_MatchesComposition()
    {
        // "©" = C2 A9 → U+00A9 → CP1251 $A9
        byte[] r = ParadoxConv.Encoding(TEncodingKind.UTF8, TEncodingKind.CP1251, Latin1(0xC2, 0xA9));
        Assert.Equal(new byte[] { 0xA9 }, r);
    }

    [Fact]
    public void Encoding_Koi8rToUcs4_And_IsoToKoi8r_Work()
    {
        Assert.Equal(Latin1(0x00, 0x00, 0x04, 0x30), ParadoxConv.Encoding(TEncodingKind.KOI8R, TEncodingKind.UCS4, Latin1(0xC1)));
        // ISO8859-5 $B0 → U+0410 → KOI8-R $E1（ParadoxConv.pas:807 #$E1 → #$04#$10）
        byte[] iso = Latin1(0xB0);
        Assert.Equal(new byte[] { 0xE1 }, ParadoxConv.Encoding(TEncodingKind.ISO88595, TEncodingKind.KOI8R, iso));
    }

    // ---------------- Latin-1 透明映射 ----------------

    [Fact]
    public void Latin1_RoundTripsAll256Bytes()
    {
        var bytes = new byte[256];
        for (int i = 0; i < 256; i++) bytes[i] = (byte)i;
        string s = ParadoxConv.BytesToLatin1(bytes);
        Assert.Equal(256, s.Length);
        Assert.Equal(bytes, ParadoxConv.Latin1ToBytes(s));
    }

    [Fact]
    public void Latin1ToBytes_RejectsNonByteChar()
    {
        Assert.Throws<EConvException>(() => ParadoxConv.Latin1ToBytes("\u4E2D"));
    }

    [Fact]
    public void Latin1ToBytes_EmptyString_ReturnsEmpty()
    {
        Assert.Empty(ParadoxConv.Latin1ToBytes(""));
        Assert.Empty(ParadoxConv.Latin1ToBytes(null));
        Assert.Equal("", ParadoxConv.BytesToLatin1(null));
    }

    [Fact]
    public void Encoding_StringOverload_IsByteTransparent()
    {
        string src = ParadoxConv.BytesToLatin1(Latin1(0x80, 0x41));
        string r = ParadoxConv.Encoding(TEncodingKind.CP1251, TEncodingKind.UCS4, src);
        Assert.Equal(ParadoxConv.BytesToLatin1(Latin1(0x00, 0x00, 0x04, 0x02, 0x00, 0x00, 0x00, 0x41)), r);
    }

    // ---------------- 映射表完整性校验和 ----------------
    // 对 7 张表做全量遍历，比对从 ParadoxConv.pas 脚本抽取的期望聚合值。
    // 任一条目被改动都会使校验和变化 → 测试失败。

    [Fact]
    public void TableChecksum_Cp1251ToUcs4()
        => Assert.Equal(ChecksumCp1251Table(), ComputeCp1251Checksum());

    [Fact]
    public void TableChecksum_Koi8rToUcs4()
        => Assert.Equal(ChecksumKoi8rTable(), ComputeKoi8rChecksum());

    [Fact]
    public void TableChecksum_Iso88595ToUcs4()
        => Assert.Equal(ChecksumIsoTable(), ComputeIsoChecksum());

    [Fact]
    public void TableChecksum_Ucs4ToCp1251()
        => Assert.Equal(ChecksumUcs4ToCp1251(), ComputeFromChecksum(TEncodingKind.CP1251));

    [Fact]
    public void TableChecksum_Ucs4ToKoi8r()
        => Assert.Equal(ChecksumUcs4ToKoi8r(), ComputeFromChecksum(TEncodingKind.KOI8R));

    [Fact]
    public void TableChecksum_Ucs4ToIso88595()
        => Assert.Equal(ChecksumUcs4ToIso(), ComputeFromChecksum(TEncodingKind.ISO88595));

    [Fact]
    public void TableChecksum_Ucs4ToCp866()
        => Assert.Equal(ChecksumUcs4ToCp866(), ComputeFromChecksum(TEncodingKind.CP866));

    [Fact]
    public void TableEntryCount_MatchesSource()
    {
        // 原 ParadoxConv.pas 抽取计数：Cp1251 少 $98（原文无 else），
        // UCS4→Cp1251 少 1 个码位（#00#00#00#98），其余全覆盖
        Assert.Equal(255, CountSourceEntries(TEncodingKind.CP1251));
        Assert.Equal(256, CountSourceEntries(TEncodingKind.KOI8R));
        Assert.Equal(256, CountSourceEntries(TEncodingKind.ISO88595));
    }

    /// <summary>把"可能抛 EConvException"的转换折成 null，便于比较两条等价路径的成败。</summary>
    private static byte[] TryOrNull(Func<byte[]> f)
    {
        try { return f(); }
        catch (EConvException) { return null; }
    }

    // ---- 计算助手（以公开 API 全量遍历，不依赖内部表） ----

    private static int CountSourceEntries(TEncodingKind enc)
    {
        int n = 0;
        for (int b = 0; b <= 0xFF; b++)
        {
            byte[] r = Convert(enc, Latin1((byte)b));
            // 未覆盖的字节在 to-UCS4 里表现为 4 个 $00（Cp1251 的 #0#0 + 无追加）
            if (enc == TEncodingKind.CP1251 && b == 0x98) continue;
            if (r.Length == 4) n++;
        }
        return n;
    }

    private static byte[] Convert(TEncodingKind enc, byte[] data) => enc switch
    {
        TEncodingKind.CP1251 => ParadoxConv.Cp1251ToUCS4(data),
        TEncodingKind.KOI8R => ParadoxConv.Koi8rToUCS4(data),
        TEncodingKind.ISO88595 => ParadoxConv.ISO88595ToUCS4(data),
        _ => throw new ArgumentOutOfRangeException(nameof(enc))
    };

    /// <summary>把 to-UCS4 表聚合成一个 32 位校验和（FNV-1a 变体）。</summary>
    private static uint FoldToTable(TEncodingKind enc, bool skipUndefined)
    {
        uint h = 2166136261u;
        for (int b = 0; b <= 0xFF; b++)
        {
            if (skipUndefined && enc == TEncodingKind.CP1251 && b == 0x98) continue;
            byte[] u = Convert(enc, Latin1((byte)b));
            if (u.Length != 4) continue;
            uint v = (uint)((u[0] << 24) | (u[1] << 16) | (u[2] << 8) | u[3]);
            h = (h ^ v) * 16777619u;
        }
        return h;
    }

    /// <summary>把 from-UCS4 表聚合成一个 32 位校验和。</summary>
    private static uint FoldFromTable(TEncodingKind enc)
    {
        uint h = 2166136261u;
        for (int hi = 0; hi <= 0xFF; hi++)
        {
            for (int lo = 0; lo <= 0xFF; lo++)
            {
                byte[] u = Ucs4Bmp((byte)hi, (byte)lo);
                byte[] outBytes;
                try { outBytes = FromConvert(enc, u); }
                catch (EConvException) { continue; }
                if (outBytes.Length != 1) continue;
                uint v = (uint)((hi << 8) | lo) * 251u + outBytes[0];
                h = (h ^ v) * 16777619u;
            }
        }
        return h;
    }

    private static byte[] FromConvert(TEncodingKind enc, byte[] ucs4) => enc switch
    {
        TEncodingKind.CP1251 => ParadoxConv.UCS4ToCp1251(ucs4),
        TEncodingKind.KOI8R => ParadoxConv.UCS4ToKoi8r(ucs4),
        TEncodingKind.ISO88595 => ParadoxConv.UCS4ToISO88595(ucs4),
        TEncodingKind.CP866 => ParadoxConv.UCS4ToCp866(ucs4),
        _ => throw new ArgumentOutOfRangeException(nameof(enc))
    };

    private static uint ComputeCp1251Checksum() => FoldToTable(TEncodingKind.CP1251, true);
    private static uint ComputeKoi8rChecksum() => FoldToTable(TEncodingKind.KOI8R, false);
    private static uint ComputeIsoChecksum() => FoldToTable(TEncodingKind.ISO88595, false);
    private static uint ComputeFromChecksum(TEncodingKind enc) => FoldFromTable(enc);

    // ---- 期望值：由 tools/extract_paradox_tables.ps1 从原文抽取后离线计算（见注释行号） ----

    /// <summary>CP1251→UCS4（ParadoxConv.pas:297-376，255 条）。</summary>
    private static uint ChecksumCp1251Table() => EXPECTED_CP1251;

    /// <summary>KOI8-R→UCS4（ParadoxConv.pas:700-842，256 条）。</summary>
    private static uint ChecksumKoi8rTable() => EXPECTED_KOI8R;

    /// <summary>ISO 8859-5→UCS4（ParadoxConv.pas:1030-1050，256 条）。</summary>
    private static uint ChecksumIsoTable() => EXPECTED_ISO88595;

    /// <summary>UCS4→CP1251（ParadoxConv.pas:378-491，255 字节）。</summary>
    private static uint ChecksumUcs4ToCp1251() => EXPECTED_UCS4_CP1251;

    /// <summary>UCS4→KOI8-R（ParadoxConv.pas:844-1028，256 字节）。</summary>
    private static uint ChecksumUcs4ToKoi8r() => EXPECTED_UCS4_KOI8R;

    /// <summary>UCS4→ISO 8859-5（ParadoxConv.pas:1052-1102，256 字节）。</summary>
    private static uint ChecksumUcs4ToIso() => EXPECTED_UCS4_ISO88595;

    /// <summary>UCS4→CP866（ParadoxConv.pas:576-698，256 字节）。</summary>
    private static uint ChecksumUcs4ToCp866() => EXPECTED_UCS4_CP866;

    private const uint EXPECTED_CP1251 = 3717427143u;
    private const uint EXPECTED_KOI8R = 1096680617u;
    private const uint EXPECTED_ISO88595 = 630022753u;
    private const uint EXPECTED_UCS4_CP1251 = 2480072355u;
    private const uint EXPECTED_UCS4_KOI8R = 2006577285u;
    private const uint EXPECTED_UCS4_ISO88595 = 1993112145u;
    private const uint EXPECTED_UCS4_CP866 = 663208085u;
}
