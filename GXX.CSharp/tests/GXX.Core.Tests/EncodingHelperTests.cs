using GXX.Core.EncodingHelper;
using Xunit;

using TEncoding = System.Text.Encoding;
using TUTF8Encoding = System.Text.UTF8Encoding;

namespace GXX.Core.Tests;

/// <summary>
/// 车道 p8-m2-itemprop-misc：Source/Common/EncodingHelper.pas 1:1 测试。
///
/// <para>
/// 本单元是"**看起来一样实则不同**的分支最多"的一个，故测试重心放在**差异断言**：
/// 同一段字节在不同 BOM/编码组合下必须落到**不同**的编码对象与不同的"跳过字节数"。
/// </para>
/// <para>覆盖：IsBufferUTF8 的全部形态分支（含纯 ASCII 返回 False、5/6 字节废弃形态）、
/// GetBufferEncoding 的 5 条嗅探分支 + 2 条"AEncoding 已给定"分支、NoBomUTF8 单例与 preamble 语义。</para>
/// </summary>
public sealed class EncodingHelperTests
{
    // UTF-8 BOM / UTF-16LE BOM / UTF-16BE BOM
    private static readonly byte[] Utf8Bom = { 0xEF, 0xBB, 0xBF };
    private static readonly byte[] Utf16LeBom = { 0xFF, 0xFE };
    private static readonly byte[] Utf16BeBom = { 0xFE, 0xFF };

    /// <summary>「中」的 UTF-8 编码（E4 B8 AD），无 BOM。</summary>
    private static readonly byte[] Utf8Zhong = { 0xE4, 0xB8, 0xAD };

    private static byte[] Concat(params byte[][] parts)
    {
        var all = new List<byte>();
        foreach (byte[] p in parts) all.AddRange(p);
        return all.ToArray();
    }

    private static byte[] Ascii(string s) => TEncoding.ASCII.GetBytes(s);

    // ==================================================================
    // IsBufferUTF8（原文 :48-131）
    // ==================================================================

    [Fact]
    public void IsBufferUTF8_EmptyBuffer_IsFalse()
        => Assert.False(TEncodingHelper.IsBufferUTF8(Array.Empty<byte>()));

    [Fact]
    public void IsBufferUTF8_PureAscii_IsFalse_OriginalQuirk()
    {
        // ★ 原文 :68-69：注释写 "If all character is US-ASCII, done."，但 Result 从 :53 起一直是 False，
        //   直接 Exit 返回 **False** —— 纯 ASCII 不被判为 UTF-8，而是回落到默认（ANSI）编码。
        Assert.False(TEncodingHelper.IsBufferUTF8(Ascii("Hello, Mir2!")));
        Assert.False(TEncodingHelper.IsBufferUTF8(Ascii("a")));
        Assert.False(TEncodingHelper.IsBufferUTF8(new byte[] { 0x00, 0x7F, 0x20 }));
    }

    [Fact]
    public void IsBufferUTF8_AsciiPrefixThenValidThreeByteSequence_IsTrue()
        => Assert.True(TEncodingHelper.IsBufferUTF8(Concat(Ascii("中文测试"), Utf8Zhong)));

    [Fact]
    public void IsBufferUTF8_ValidTwoByteSequence_IsTrue()
        => Assert.True(TEncodingHelper.IsBufferUTF8(new byte[] { 0xC3, 0xA9 }));      // é

    [Fact]
    public void IsBufferUTF8_ValidFourByteSequence_IsTrue()
        => Assert.True(TEncodingHelper.IsBufferUTF8(new byte[] { 0xF0, 0x90, 0x80, 0x80 }));  // U+10000

    [Fact]
    public void IsBufferUTF8_TruncatedTwoByteSequence_IsFalse()
        => Assert.False(TEncodingHelper.IsBufferUTF8(new byte[] { 0xC3 }));

    [Fact]
    public void IsBufferUTF8_TruncatedThreeByteSequence_IsFalse()
        => Assert.False(TEncodingHelper.IsBufferUTF8(new byte[] { 0xE4, 0xB8 }));

    [Fact]
    public void IsBufferUTF8_TruncatedFourByteSequence_IsFalse()
        => Assert.False(TEncodingHelper.IsBufferUTF8(new byte[] { 0xF0, 0x90, 0x80 }));

    [Fact]
    public void IsBufferUTF8_BadContinuationByte_IsFalse()
        => Assert.False(TEncodingHelper.IsBufferUTF8(new byte[] { 0xC3, 0x41 }));      // 'A' 不是续字节

    [Fact]
    public void IsBufferUTF8_LoneContinuationByte_IsFalse()
        => Assert.False(TEncodingHelper.IsBufferUTF8(new byte[] { 0x80 }));

    [Fact]
    public void IsBufferUTF8_ByteFeOrFf_IsFalse()
    {
        Assert.False(TEncodingHelper.IsBufferUTF8(new byte[] { 0xFE }));
        Assert.False(TEncodingHelper.IsBufferUTF8(new byte[] { 0xFF }));
        Assert.False(TEncodingHelper.IsBufferUTF8(Concat(Ascii("abc"), new byte[] { 0xFF })));
    }

    [Fact]
    public void IsBufferUTF8_OverlongTwoByteEncoding_IsAccepted_OriginalQuirk()
    {
        // ★ 原文无"最短编码"校验：$C0/$C1 的过长编码照样算合法
        Assert.True(TEncodingHelper.IsBufferUTF8(new byte[] { 0xC0, 0x80 }));
        Assert.True(TEncodingHelper.IsBufferUTF8(new byte[] { 0xC1, 0xBF }));
    }

    [Fact]
    public void IsBufferUTF8_DeprecatedFiveByteForm_IsAccepted_OriginalQuirk()
    {
        // ★ $F8..$FB 是已废弃的 5 字节形态，原文照样接受
        Assert.True(TEncodingHelper.IsBufferUTF8(new byte[] { 0xF8, 0x88, 0x80, 0x80, 0x80 }));
    }

    [Fact]
    public void IsBufferUTF8_DeprecatedSixByteForm_IsAccepted_OriginalQuirk()
    {
        // ★ $FC..$FD 是已废弃的 6 字节形态，原文照样接受
        Assert.True(TEncodingHelper.IsBufferUTF8(new byte[] { 0xFC, 0x84, 0x80, 0x80, 0x80, 0x80 }));
    }

    [Fact]
    public void IsBufferUTF8_TruncatedFiveAndSixByteForms_AreFalse()
    {
        Assert.False(TEncodingHelper.IsBufferUTF8(new byte[] { 0xF8, 0x88, 0x80, 0x80 }));
        Assert.False(TEncodingHelper.IsBufferUTF8(new byte[] { 0xFC, 0x84, 0x80, 0x80, 0x80 }));
    }

    [Fact]
    public void IsBufferUTF8_SingleByteAfterValidSequence_IsTrue()
    {
        // 主循环里的 $00..$7F 分支（:75-76）只有在这一步才可达（前导 ASCII 已被跳过）
        Assert.True(TEncodingHelper.IsBufferUTF8(new byte[] { 0xE4, 0xB8, 0xAD, 0x41 }));
    }

    [Fact]
    public void IsBufferUTF8_GbkBytes_AreFalse()
    {
        // GBK「中」= D6 D0：D6 是 2 字节首字节，但 D0 不是 $80..$BF 续字节 → False
        Assert.False(TEncodingHelper.IsBufferUTF8(new byte[] { 0xD6, 0xD0 }));
    }

    // ==================================================================
    // GetBufferEncoding —— 两参重载（默认编码回落）
    // ==================================================================

    [Fact]
    public void GetBufferEncoding_Utf8Bom_PicksUtf8AndSkipsThree()
    {
        TEncoding? enc = null;
        int skip = TEncodingHelper.GetBufferEncoding(Concat(Utf8Bom, Utf8Zhong), ref enc);
        Assert.Same(TEncoding.UTF8, enc);
        Assert.Equal(3, skip);
    }

    [Fact]
    public void GetBufferEncoding_Utf16LeBom_PicksUnicodeAndSkipsTwo()
    {
        TEncoding? enc = null;
        int skip = TEncodingHelper.GetBufferEncoding(Concat(Utf16LeBom, new byte[] { 0x2D, 0x4E }), ref enc);
        Assert.Same(TEncoding.Unicode, enc);
        Assert.Equal(2, skip);
    }

    [Fact]
    public void GetBufferEncoding_Utf16BeBom_PicksBigEndianUnicodeAndSkipsTwo()
    {
        TEncoding? enc = null;
        int skip = TEncodingHelper.GetBufferEncoding(Concat(Utf16BeBom, new byte[] { 0x4E, 0x2D }), ref enc);
        Assert.Same(TEncoding.BigEndianUnicode, enc);
        Assert.Equal(2, skip);
    }

    [Fact]
    public void GetBufferEncoding_Utf8WithoutBom_PicksNoBomUtf8AndSkipsZero()
    {
        TEncoding? enc = null;
        int skip = TEncodingHelper.GetBufferEncoding(Utf8Zhong, ref enc);
        Assert.Same(TEncodingHelper.NoBomUTF8, enc);
        Assert.Equal(0, skip);                                    // NoBomUTF8 的 preamble 长度为 0
    }

    [Fact]
    public void GetBufferEncoding_PureAscii_FallsBackToSystemAnsi()
    {
        TEncoding? enc = null;
        int skip = TEncodingHelper.GetBufferEncoding(Ascii("plain ascii"), ref enc);
        Assert.Same(EncodingInit.GBK, enc);                        // Delphi TEncoding.Default = 系统 ANSI（936）
        Assert.Equal(0, skip);
    }

    [Fact]
    public void GetBufferEncoding_InvalidBytes_FallsBackToSystemAnsi()
    {
        TEncoding? enc = null;
        int skip = TEncodingHelper.GetBufferEncoding(new byte[] { 0xFF, 0xFE, 0x00, 0xFF, 0xFF }, ref enc);
        // 注意：FF FE 开头会被 UTF-16LE 分支吃掉 → 这里改成不含 BOM 前缀的非法串
        Assert.Same(TEncoding.Unicode, enc);
        Assert.Equal(2, skip);

        TEncoding? enc2 = null;
        int skip2 = TEncodingHelper.GetBufferEncoding(new byte[] { 0x41, 0xFF, 0xFF }, ref enc2);
        Assert.Same(EncodingInit.GBK, enc2);
        Assert.Equal(0, skip2);
    }

    [Fact]
    public void GetBufferEncoding_EmptyBuffer_FallsBackToSystemAnsi()
    {
        TEncoding? enc = null;
        int skip = TEncodingHelper.GetBufferEncoding(Array.Empty<byte>(), ref enc);
        Assert.Same(EncodingInit.GBK, enc);
        Assert.Equal(0, skip);
    }

    [Fact]
    public void GetBufferEncoding_BufferShorterThanAnyPreamble_FallsBackToSystemAnsi()
    {
        TEncoding? enc = null;
        int skip = TEncodingHelper.GetBufferEncoding(new byte[] { 0xEF, 0xBB }, ref enc);   // UTF-8 BOM 缺 1 字节
        Assert.Same(EncodingInit.GBK, enc);
        Assert.Equal(0, skip);
    }

    [Fact]
    public void GetBufferEncoding_DefaultIsSystemAnsiNotDotNetUtf8()
    {
        // ★ 差异断言：Delphi 的 TEncoding.Default ≠ .NET 的 Encoding.Default（后者在 .NET Core 下是 UTF-8）
        Assert.Equal(936, TEncodingHelper.Default.CodePage);
        Assert.NotSame(TEncoding.UTF8, TEncodingHelper.Default);
        Assert.NotEqual(TEncoding.UTF8.CodePage, TEncodingHelper.Default.CodePage);
    }

    // ==================================================================
    // GetBufferEncoding —— 三参重载（显式默认编码）
    // ==================================================================

    [Fact]
    public void GetBufferEncoding_ExplicitDefault_IsUsedWhenNothingMatches()
    {
        TEncoding? enc = null;
        int skip = TEncodingHelper.GetBufferEncoding(Ascii("abc"), ref enc, TEncoding.ASCII);
        Assert.Same(TEncoding.ASCII, enc);
        Assert.Equal(0, skip);                                     // ★ 提前 Exit，不读 ASCII 的 preamble
    }

    [Fact]
    public void GetBufferEncoding_ExplicitDefaultWithPreamble_StillReturnsZero_OriginalQuirk()
    {
        // ★ 原文 :178 `Exit; // Don't proceed just in case ADefaultEncoding has a Preamble`
        //   即便默认编码自己有 BOM，回落分支的返回值也恒为 0。
        TEncoding? enc = null;
        int skip = TEncodingHelper.GetBufferEncoding(Ascii("abc"), ref enc, new TUTF8Encoding(true));
        Assert.Equal(0, skip);
        Assert.NotSame(TEncodingHelper.NoBomUTF8, enc);
    }

    [Fact]
    public void GetBufferEncoding_PreSetEncodingMatchingBom_ReturnsPreambleLength()
    {
        TEncoding? enc = TEncoding.UTF8;
        int skip = TEncodingHelper.GetBufferEncoding(Concat(Utf8Bom, Utf8Zhong), ref enc);
        Assert.Same(TEncoding.UTF8, enc);                          // 不被替换
        Assert.Equal(3, skip);
    }

    [Fact]
    public void GetBufferEncoding_PreSetEncodingNotMatching_ReturnsZeroAndKeepsEncoding()
    {
        TEncoding? enc = TEncoding.UTF8;
        int skip = TEncodingHelper.GetBufferEncoding(Utf8Zhong, ref enc);   // 无 BOM
        Assert.Same(TEncoding.UTF8, enc);
        Assert.Equal(0, skip);
    }

    [Fact]
    public void GetBufferEncoding_PreSetUtf16Le_WithoutBomInBuffer_ReturnsZero()
    {
        TEncoding? enc = TEncoding.Unicode;
        int skip = TEncodingHelper.GetBufferEncoding(new byte[] { 0x2D, 0x4E }, ref enc);
        Assert.Equal(0, skip);
    }

    [Fact]
    public void GetBufferEncoding_PreSetNoBomUtf8_OnEmptyPreamble_ReturnsZero()
    {
        // 空 preamble 会命中 ContainsPreamble 的"空签名恒真"分支，但结果仍是 0（与"不命中"不可区分）
        TEncoding? enc = TEncodingHelper.NoBomUTF8;
        int skip = TEncodingHelper.GetBufferEncoding(Utf8Zhong, ref enc);
        Assert.Equal(0, skip);
        Assert.Same(TEncodingHelper.NoBomUTF8, enc);
    }

    [Fact]
    public void GetBufferEncoding_PreSetEncoding_WithEmptyBuffer_ReturnsZero()
    {
        TEncoding? enc = TEncoding.Unicode;
        int skip = TEncodingHelper.GetBufferEncoding(Array.Empty<byte>(), ref enc);
        Assert.Equal(0, skip);
    }

    // ==================================================================
    // 差异断言：同字节序列在不同 BOM/编码组合下必须落到不同结果
    // ==================================================================

    public static IEnumerable<object[]> EncodingDiffCases()
    {
        yield return new object[] { "utf8-bom", Concat(Utf8Bom, Utf8Zhong), 3, "utf8" };
        yield return new object[] { "utf16le-bom", Concat(Utf16LeBom, new byte[] { 0x2D, 0x4E }), 2, "utf16le" };
        yield return new object[] { "utf16be-bom", Concat(Utf16BeBom, new byte[] { 0x4E, 0x2D }), 2, "utf16be" };
        yield return new object[] { "utf8-nobom", Utf8Zhong, 0, "nobom" };
        yield return new object[] { "ascii", Ascii("Mir2"), 0, "ansi" };
        yield return new object[] { "invalid", new byte[] { 0x41, 0xFF }, 0, "ansi" };
        yield return new object[] { "empty", Array.Empty<byte>(), 0, "ansi" };
    }

    [Theory]
    [MemberData(nameof(EncodingDiffCases))]
    public void GetBufferEncoding_EachBomCombinationMapsToItsOwnEncoding(string name, byte[] buffer,
        int expectedSkip, string expectedKind)
    {
        TEncoding? enc = null;
        int skip = TEncodingHelper.GetBufferEncoding(buffer, ref enc);

        Assert.NotNull(enc);
        Assert.Equal(expectedSkip, skip);
        switch (expectedKind)
        {
            case "utf8": Assert.Same(TEncoding.UTF8, enc); break;
            case "utf16le": Assert.Same(TEncoding.Unicode, enc); break;
            case "utf16be": Assert.Same(TEncoding.BigEndianUnicode, enc); break;
            case "nobom": Assert.Same(TEncodingHelper.NoBomUTF8, enc); break;
            default: Assert.Same(EncodingInit.GBK, enc); break;
        }
        Assert.False(string.IsNullOrEmpty(name));
    }

    [Fact]
    public void GetBufferEncoding_SamePayloadWithAndWithoutBom_YieldsDifferentEncodingAndSkip()
    {
        // ★ 核心差异断言：同一段「中」的字节，带 BOM → Encoding.UTF8（跳 3），
        //   不带 BOM → NoBomUTF8（跳 0）。二者**不是**同一个编码对象。
        TEncoding? withBom = null;
        TEncoding? withoutBom = null;
        int skipWith = TEncodingHelper.GetBufferEncoding(Concat(Utf8Bom, Utf8Zhong), ref withBom);
        int skipWithout = TEncodingHelper.GetBufferEncoding(Utf8Zhong, ref withoutBom);

        Assert.NotSame(withBom, withoutBom);
        Assert.NotEqual(skipWith, skipWithout);
        Assert.Equal(3, skipWith);
        Assert.Equal(0, skipWithout);
    }

    [Fact]
    public void GetBufferEncoding_Utf16LeVersusBe_YieldsDifferentEncodings()
    {
        // ★ 两字节 BOM 只有字节序不同（FF FE / FE FF），必须落到不同编码
        TEncoding? le = null;
        TEncoding? be = null;
        TEncodingHelper.GetBufferEncoding(Concat(Utf16LeBom, new byte[] { 0x2D, 0x4E }), ref le);
        TEncodingHelper.GetBufferEncoding(Concat(Utf16BeBom, new byte[] { 0x4E, 0x2D }), ref be);
        Assert.Same(TEncoding.Unicode, le);
        Assert.Same(TEncoding.BigEndianUnicode, be);
        Assert.NotSame(le, be);
    }

    [Fact]
    public void GetBufferEncoding_BomWinsOverIsBufferUTF8()
    {
        // ★ EF BB BF 本身也是合法的 3 字节 UTF-8 序列，故两条路径都"命中"；
        //   原文把 BOM 嗅探放在 IsBufferUTF8 **之前** → 结果是 Encoding.UTF8（而非 NoBomUTF8）。
        TEncoding? enc = null;
        TEncodingHelper.GetBufferEncoding(Concat(Utf8Bom, Utf8Zhong), ref enc);
        Assert.Same(TEncoding.UTF8, enc);
        Assert.NotSame(TEncodingHelper.NoBomUTF8, enc);
        Assert.True(TEncodingHelper.IsBufferUTF8(Concat(Utf8Bom, Utf8Zhong)));   // 两条路径都命中
    }

    [Fact]
    public void GetBufferEncoding_SkipCountActuallySkipsTheBomWhenDecoding()
    {
        // 端到端：用返回的 skip 跳过 BOM 后解码，必须得到干净文本
        byte[] buffer = Concat(Utf8Bom, Utf8Zhong);
        TEncoding? enc = null;
        int skip = TEncodingHelper.GetBufferEncoding(buffer, ref enc);
        string text = enc!.GetString(buffer, skip, buffer.Length - skip);
        Assert.Equal("中", text);
    }

    [Fact]
    public void GetBufferEncoding_NoBomPath_DecodesWholeBufferWithoutSkipping()
    {
        byte[] buffer = Concat(Utf8Zhong, Utf8Zhong);
        TEncoding? enc = null;
        int skip = TEncodingHelper.GetBufferEncoding(buffer, ref enc);
        Assert.Equal("中中", enc!.GetString(buffer, skip, buffer.Length - skip));
    }

    // ==================================================================
    // TUTF8NoBomEncoding / NoBomUTF8（原文 :9-12、:24、:190-204）
    // ==================================================================

    [Fact]
    public void TUTF8NoBomEncoding_PreambleIsEmpty()
        => Assert.Empty(new TUTF8NoBomEncoding().GetPreamble());

    [Fact]
    public void TUTF8NoBomEncoding_IsUtf8Encoding()
    {
        var enc = new TUTF8NoBomEncoding();
        Assert.IsAssignableFrom<TUTF8Encoding>(enc);
        Assert.Equal(65001, enc.CodePage);
    }

    [Fact]
    public void Utf8Preamble_IsThreeBytes_DifferenceFromNoBomVariant()
    {
        // ★ 差异断言：TEncoding.UTF8 有 3 字节 BOM，TUTF8NoBomEncoding 没有
        Assert.Equal(new byte[] { 0xEF, 0xBB, 0xBF }, TEncoding.UTF8.GetPreamble());
        Assert.Empty(new TUTF8NoBomEncoding().GetPreamble());
    }

    [Fact]
    public void NoBomUTF8_IsSingleton()
        => Assert.Same(TEncodingHelper.NoBomUTF8, TEncodingHelper.NoBomUTF8);

    [Fact]
    public void NoBomUTF8_IsTUtf8NoBomEncodingAndHasEmptyPreamble()
    {
        TEncoding enc = TEncodingHelper.NoBomUTF8;
        Assert.IsType<TUTF8NoBomEncoding>(enc);
        Assert.Empty(enc.GetPreamble());
    }

    [Fact]
    public void NoBomUTF8_UnderConcurrency_ReturnsSameInstance()
    {
        // 复刻 AtomicCmpExchange 单例：并发首次访问只能有一个赢家
        var seen = new TEncoding?[64];
        Parallel.For(0, seen.Length, i => seen[i] = TEncodingHelper.NoBomUTF8);
        Assert.All(seen, e => Assert.Same(seen[0], e));
    }

    [Fact]
    public void NoBomUTF8_DecodesUtf8Text()
        => Assert.Equal("中文ABC", TEncodingHelper.NoBomUTF8.GetString(
            TEncoding.UTF8.GetBytes("中文ABC")));

    [Fact]
    public void NoBomUTF8_EncodeDoesNotAddBom()
    {
        // 用 GetPreamble 语义：写文件时不会先写 BOM（与 TEncoding.UTF8 的差异）
        byte[] bytes = TEncodingHelper.NoBomUTF8.GetBytes("A");
        Assert.Equal(new byte[] { 0x41 }, bytes);
        Assert.NotEqual(TEncoding.UTF8.GetPreamble().Length, TEncodingHelper.NoBomUTF8.GetPreamble().Length);
    }
}
