using GXX.Core.Util;
using Xunit;

using TEncoding = System.Text.Encoding;

namespace GXX.Core.Tests;

/// <summary>
/// 车道 p8-m2-itemprop-misc：**请求 #2 / #3 / #4 落地**的 Core 侧测试 ——
/// <list type="bullet">
/// <item>#2 <c>FastIniFile.pas</c> 固定格式日期时间（<c>FIXED_*</c>、<c>StrToDateDef/StrToTimeDef</c>、
/// <c>TFastIniFile.ReadFixedDateTime/WriteFixedDateTime</c>）1:1 断言；</item>
/// <item>#3 <c>TStringList.Text</c>（<c>TStrings.GetTextStr/SetTextStr</c>）与
/// <c>DefaultEncoding/SetEncoding/GetEncoding</c>/<c>LineBreak</c>；</item>
/// <item>#4 <c>TStringList.LoadFromFile</c>/<c>SaveToFile</c> 接到
/// <c>TEncodingHelper.GetBufferEncoding</c> 的**编码嗅探差异断言**。</item>
/// </list>
///
/// <para>
/// 文件命名说明：本车道在 <c>tests/GXX.Core.Tests</c> 的授权前缀是 <c>EncodingHelper*</c>；
/// 本文件承载请求 #2（FastIniFile 回收）与请求 #4（TEncodingHelper 接线）的 Core 侧用例，
/// 为**不越出文件分区**而沿用该前缀，内容与文件名不完全对应，已在交付报告登记。
/// </para>
/// </summary>
public sealed class EncodingHelperCoreWiringTests : IDisposable
{
    private readonly string _dir;

    public EncodingHelperCoreWiringTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "p8core_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, true); } catch { /* 临时目录清理失败不影响门禁 */ }
    }

    private TFastIniFile NewIni() => new(Path.Combine(_dir, "dt.ini"));

    private static double Dt(int y, int m, int d, int hh = 0, int mm = 0, int ss = 0)
        => new DateTime(y, m, d, hh, mm, ss).ToOADate();

    // ==================================================================
    // 常量（FastIniFile.pas:189-194）
    // ==================================================================

    [Fact]
    public void FixedFormatConstants_MatchFastIniFile()
    {
        Assert.Equal('-', TIniFixedDateTime.FIXED_DS);
        Assert.Equal(':', TIniFixedDateTime.FIXED_TS);
        Assert.Equal("dd-mm-yyyy", TIniFixedDateTime.FIXED_DATE);
        Assert.Equal("hh:nn:ss", TIniFixedDateTime.FIXED_TIME);
        Assert.Equal("dd-mm-yyyy hh:nn:ss", TIniFixedDateTime.FIXED_DATETIME);
        Assert.Equal(10, TIniFixedDateTime.FIXED_DATE.Length);      // Copy(S, 1, 10)
        Assert.Equal(8, TIniFixedDateTime.FIXED_TIME.Length);       // Copy(S, I+1, 8)
    }

    // ==================================================================
    // StrToDateDef / StrToTimeDef（FastIniFile.pas:988-1024）
    // ==================================================================

    [Fact]
    public void StrToDateDef_DashSeparatedFixedFormat_Parses()
        => Assert.Equal(Dt(2023, 12, 25), TIniFixedDateTime.StrToDateDef("25-12-2023", -1));

    [Fact]
    public void StrToDateDef_SlashSeparator_ReturnsDefault()
        => Assert.Equal(-1.0, TIniFixedDateTime.StrToDateDef("25/12/2023", -1));

    [Fact]
    public void StrToDateDef_Garbage_ReturnsDefault()
        => Assert.Equal(-1.0, TIniFixedDateTime.StrToDateDef("not-a-date", -1));

    [Fact]
    public void StrToDateDef_ImpossibleDate_ReturnsDefault()
        => Assert.Equal(-1.0, TIniFixedDateTime.StrToDateDef("31-02-2023", -1));

    [Fact]
    public void StrToTimeDef_ColonSeparatedFixedFormat_Parses()
        => Assert.Equal(new TimeSpan(13, 45, 7).TotalDays, TIniFixedDateTime.StrToTimeDef("13:45:07", 0));

    [Fact]
    public void StrToTimeDef_Garbage_ReturnsDefault()
        => Assert.Equal(0.0, TIniFixedDateTime.StrToTimeDef("garbage!", 0));

    [Fact]
    public void StrToTimeDef_OutOfRangeHour_ReturnsDefault()
        => Assert.Equal(0.0, TIniFixedDateTime.StrToTimeDef("99:00:00", 0));

    [Fact]
    public void FormatFixedDateTime_ZeroIsDelphiEpoch()
        => Assert.Equal("30-12-1899 00:00:00", TIniFixedDateTime.FormatFixedDateTime(0));

    [Fact]
    public void FormatFixedDateTime_KnownValue()
        => Assert.Equal("25-12-2023 13:45:07", TIniFixedDateTime.FormatFixedDateTime(Dt(2023, 12, 25, 13, 45, 7)));

    [Fact]
    public void FormatFixedDateTime_OutOfOleRange_YieldsEmpty()
    {
        Assert.Equal("", TIniFixedDateTime.FormatFixedDateTime(double.NaN));
        Assert.Equal("", TIniFixedDateTime.FormatFixedDateTime(1e12));
        Assert.Equal("", TIniFixedDateTime.FormatFixedDateTime(-1e12));
    }

    // ==================================================================
    // TFastIniFile.ReadFixedDateTime（FastIniFile.pas:2953-2971）—— 逐分支
    // ==================================================================

    [Fact]
    public void ReadFixedDateTime_ValidValue_ReturnsDatePlusTime()
    {
        var ini = NewIni();
        ini.WriteString("S", "V", "25-12-2023 13:45:07");
        Assert.Equal(Dt(2023, 12, 25, 13, 45, 7), ini.ReadFixedDateTime("S", "V", 0));
    }

    [Fact]
    public void ReadFixedDateTime_ValidValue_IgnoresTrailingGarbageBeyondEightChars()
    {
        // 时间段只取 8 字符（Copy(S, I+1, 8)）→ 'xyz' 被丢弃
        var ini = NewIni();
        ini.WriteString("S", "V", "25-12-2023 13:45:07xyz");
        Assert.Equal(Dt(2023, 12, 25, 13, 45, 7), ini.ReadFixedDateTime("S", "V", 0));
    }

    [Fact]
    public void ReadFixedDateTime_NoSpace_ReturnsDefaultWithoutParsingDate()
    {
        // 分支 1（:2963-2964）：Pos(' ', S) = 0 → 连日期都不解析
        var ini = NewIni();
        ini.WriteString("S", "V", "25-12-2023");
        Assert.Equal(-7.0, ini.ReadFixedDateTime("S", "V", -7));
    }

    [Fact]
    public void ReadFixedDateTime_MissingKey_ReturnsDefault()
    {
        var ini = NewIni();
        Assert.Equal(-3.0, ini.ReadFixedDateTime("S", "absent", -3));
    }

    [Fact]
    public void ReadFixedDateTime_EmptyValue_ReturnsDefault()
    {
        var ini = NewIni();
        ini.WriteString("S", "V", "");
        Assert.Equal(12.5, ini.ReadFixedDateTime("S", "V", 12.5));
    }

    [Fact]
    public void ReadFixedDateTime_BadDate_ReturnsDefault()
    {
        // 分支 2（:2966-2968）：日期段 StrToDateDef 失败 → -1 → 整体返回 Default
        var ini = NewIni();
        ini.WriteString("S", "V", "99-99-9999 13:45:07");
        Assert.Equal(-5.0, ini.ReadFixedDateTime("S", "V", -5));
    }

    [Fact]
    public void ReadFixedDateTime_ShortDateSegment_IsCutAtTenCharsAndFails()
    {
        // ★ 日期段是**定长 10 字符**（Copy(S, 1, 10)）：'1-1-2023 13:45:07' 取到 '1-1-2023 1' → 解析失败
        var ini = NewIni();
        ini.WriteString("S", "V", "1-1-2023 13:45:07");
        Assert.Equal(-9.0, ini.ReadFixedDateTime("S", "V", -9));
    }

    [Fact]
    public void ReadFixedDateTime_BadTime_IsSilentlyAcceptedAsMidnight_OriginalFlaw()
    {
        // ★★ 分支 3（:2967-2968，原文缺陷）：StrToTimeDef 的 Default 是 0，判据是 T <> -1 → 恒真。
        //    于是"坏时间"被静默归零为当天 00:00:00，而不是回退 Default。
        var ini = NewIni();
        ini.WriteString("S", "V", "25-12-2023 garbage!");
        Assert.Equal(Dt(2023, 12, 25), ini.ReadFixedDateTime("S", "V", -1));
        Assert.NotEqual(-1.0, ini.ReadFixedDateTime("S", "V", -1));
    }

    [Fact]
    public void ReadFixedDateTime_ValidDateNoTimePart_AlsoYieldsMidnight()
    {
        // 同一条原文缺陷的另一形态：'25-12-2023 ' （有空格但时间缺失）→ T = 0 → 当天 00:00:00
        var ini = NewIni();
        ini.WriteString("S", "V", "25-12-2023 ");
        Assert.Equal(Dt(2023, 12, 25), ini.ReadFixedDateTime("S", "V", -2));
    }

    // ==================================================================
    // TFastIniFile.WriteFixedDateTime（FastIniFile.pas:2985-2989）
    // ==================================================================

    [Fact]
    public void WriteFixedDateTime_WritesFixedFormatString()
    {
        var ini = NewIni();
        ini.WriteFixedDateTime("S", "V", Dt(2019, 1, 2, 3, 4, 5));
        Assert.Equal("02-01-2019 03:04:05", ini.ReadString("S", "V", ""));
    }

    [Fact]
    public void WriteFixedDateTime_Zero_WritesDelphiEpoch()
    {
        var ini = NewIni();
        ini.WriteFixedDateTime("S", "V", 0);
        Assert.Equal("30-12-1899 00:00:00", ini.ReadString("S", "V", ""));
    }

    [Fact]
    public void WriteFixedDateTime_OutOfRange_WritesEmptyString()
    {
        var ini = NewIni();
        ini.WriteFixedDateTime("S", "V", 1e12);
        Assert.Equal("", ini.ReadString("S", "V", "x"));
    }

    [Fact]
    public void WriteThenReadFixedDateTime_RoundTrips()
    {
        var ini = NewIni();
        double v = Dt(2020, 7, 8, 9, 10, 11);
        ini.WriteFixedDateTime("S", "V", v);
        Assert.Equal(v, ini.ReadFixedDateTime("S", "V", 0));
    }

    // ==================================================================
    // 请求 #3：TStringList.Text（TStrings.GetTextStr / SetTextStr）
    // ==================================================================

    [Fact]
    public void Text_EmptyList_IsEmptyString()
        => Assert.Equal("", new TStringList().Text);

    [Fact]
    public void Text_OneLine_HasTrailingCrlf()
    {
        var l = new TStringList();
        l.Add("a");
        Assert.Equal("a\r\n", l.Text);
    }

    [Fact]
    public void Text_TwoLines_EachLineFollowedByCrlf()
    {
        var l = new TStringList();
        l.Add("a");
        l.Add("b");
        Assert.Equal("a\r\nb\r\n", l.Text);
    }

    [Fact]
    public void Text_EmptyLine_YieldsBareCrlf()
    {
        var l = new TStringList();
        l.Add("");
        Assert.Equal("\r\n", l.Text);
    }

    [Fact]
    public void Text_SingleLineWithoutNewline_StillGetsTrailingCrlf()
    {
        // "单行无换行"边界：GetTextStr 给**每一行**都补 LineBreak（含最后一行）
        var l = new TStringList();
        l.Add("only");
        Assert.Equal("only\r\n", l.Text);
        Assert.False(l.Text.EndsWith("only"));
    }

    [Fact]
    public void Text_Setter_EmptyString_YieldsNoLines()
    {
        var l = new TStringList();
        l.Add("stale");
        l.Text = "";
        Assert.Equal(0, l.Count);
    }

    [Fact]
    public void Text_Setter_TrailingCrlf_DoesNotProduceEmptyLastLine()
    {
        var l = new TStringList();
        l.Text = "a\r\n";
        Assert.Equal(1, l.Count);
        Assert.Equal("a", l[0]);
    }

    [Fact]
    public void Text_Setter_LeadingCrlf_ProducesOneEmptyLine()
    {
        var l = new TStringList();
        l.Text = "\r\n";
        Assert.Equal(1, l.Count);
        Assert.Equal("", l[0]);
    }

    [Fact]
    public void Text_Setter_ConsecutiveLf_ProducesEmptyMiddleLine()
    {
        var l = new TStringList();
        l.Text = "a\n\nb";
        Assert.Equal(3, l.Count);
        Assert.Equal(new[] { "a", "", "b" }, new[] { l[0], l[1], l[2] });
    }

    [Fact]
    public void Text_Setter_BareCr_IsAlsoALineBreak()
    {
        var l = new TStringList();
        l.Text = "a\rb";
        Assert.Equal(2, l.Count);
        Assert.Equal(new[] { "a", "b" }, new[] { l[0], l[1] });
    }

    [Fact]
    public void Text_Setter_MixedCrLf_IsSingleBreak()
    {
        var l = new TStringList();
        l.Text = "a\r\nb";
        Assert.Equal(2, l.Count);
        Assert.Equal("b", l[1]);
    }

    [Fact]
    public void Text_Setter_ReplacesPreviousContent()
    {
        var l = new TStringList();
        l.Add("x");
        l.Add("y");
        l.Text = "z";
        Assert.Equal(1, l.Count);
    }

    [Fact]
    public void Text_RoundTrip_PreservesLinesAndShape()
    {
        var src = new TStringList();
        src.Add("第一行");
        src.Add("");
        src.Add("line3");
        string text = src.Text;
        Assert.Equal("第一行\r\n\r\nline3\r\n", text);

        var dst = new TStringList();
        dst.Text = text;
        Assert.Equal(3, dst.Count);
        Assert.Equal("第一行", dst[0]);
        Assert.Equal("", dst[1]);
        Assert.Equal("line3", dst[2]);
    }

    [Fact]
    public void Text_GetterUsesLineBreakProperty_SetterIgnoresIt()
    {
        // 原文：GetTextStr 用 LineBreak；SetTextStr **硬编码** #13/#10
        var l = new TStringList();
        l.LineBreak = "\n";
        l.Add("a");
        Assert.Equal("a\n", l.Text);

        var m = new TStringList();
        m.LineBreak = "\n";
        m.Text = "a\nb";                      // 仍按 #10 切行
        Assert.Equal(2, m.Count);
        Assert.Equal("a\nb\n", m.Text);       // 但读出来用设定的 LineBreak
    }

    [Fact]
    public void LineBreak_EmptyString_Throws()
    {
        var l = new TStringList();
        Assert.Throws<ArgumentException>(() => l.LineBreak = "");
    }

    [Fact]
    public void LineBreak_DefaultsToCrlf()
        => Assert.Equal("\r\n", new TStringList().LineBreak);

    // ==================================================================
    // 请求 #3：编码状态（DefaultEncoding / GetEncoding / SetEncoding）
    // ==================================================================

    [Fact]
    public void DefaultEncoding_IsSystemAnsiGbk()
    {
        Assert.Equal(936, new TStringList().DefaultEncoding.CodePage);   // Delphi TEncoding.Default
        Assert.NotEqual(TEncoding.UTF8.CodePage, new TStringList().DefaultEncoding.CodePage);
    }

    [Fact]
    public void GetEncoding_FallsBackToDefaultEncodingAndCaches()
    {
        var l = new TStringList();
        Assert.Same(l.DefaultEncoding, l.GetEncoding());
        Assert.Same(l.GetEncoding(), l.GetEncoding());                   // 缓存同一实例
    }

    [Fact]
    public void SetEncoding_OverridesGetEncoding()
    {
        var l = new TStringList();
        l.SetEncoding(TEncoding.UTF8);
        Assert.Same(TEncoding.UTF8, l.GetEncoding());
        Assert.NotSame(l.DefaultEncoding, l.GetEncoding());
    }

    // ==================================================================
    // 请求 #4：LoadFromFile / SaveToFile 的编码嗅探（差异断言）
    // ==================================================================

    private string WriteBytes(string name, byte[] bytes)
    {
        string path = Path.Combine(_dir, name);
        File.WriteAllBytes(path, bytes);
        return path;
    }

    [Fact]
    public void LoadFromFile_GbkFile_DecodesAsGbkAndKeepsEncodingGbk()
    {
        string path = WriteBytes("gbk.txt", EncodingInit.GBK.GetBytes("中文行\r\n第二行"));
        var l = new TStringList();
        l.LoadFromFile(path);
        Assert.Equal(2, l.Count);
        Assert.Equal("中文行", l[0]);
        Assert.Equal("第二行", l[1]);
        Assert.Equal(936, l.GetEncoding().CodePage);                     // 纯 ASCII/GBK → 默认编码
    }

    [Fact]
    public void LoadFromFile_Utf8NoBomFile_IsDecodedAsUtf8_NotGbk()
    {
        // ★ 差异断言（请求 #4 的核心）：旧实现固定 GBK → 这里过去是乱码；
        //   现在拿到 NoBomUTF8（preamble 为空）→ 正确解码且不跳字节。
        string path = WriteBytes("u8.txt", TEncoding.UTF8.GetBytes("中文行\r\n第二行"));
        var l = new TStringList();
        l.LoadFromFile(path);
        Assert.Equal("中文行", l[0]);
        Assert.Equal("第二行", l[1]);
        Assert.Equal(65001, l.GetEncoding().CodePage);
        Assert.Empty(l.GetEncoding().GetPreamble());                     // NoBomUTF8
    }

    [Fact]
    public void LoadFromFile_Utf8WithBom_StripsBomAndKeepsUtf8EncodingWithPreamble()
    {
        byte[] bom = TEncoding.UTF8.GetPreamble();
        byte[] body = TEncoding.UTF8.GetBytes("中文行");
        string path = WriteBytes("u8bom.txt", bom.Concat(body).ToArray());
        var l = new TStringList();
        l.LoadFromFile(path);
        Assert.Equal(1, l.Count);
        Assert.Equal("中文行", l[0]);                                    // BOM 已被跳掉，没变成 \uFEFF
        Assert.Equal(65001, l.GetEncoding().CodePage);
        Assert.Equal(3, l.GetEncoding().GetPreamble().Length);           // Encoding.UTF8（带 BOM）
    }

    [Fact]
    public void LoadFromFile_Utf16LeFile_IsDecodedAsUtf16()
    {
        byte[] bom = TEncoding.Unicode.GetPreamble();
        byte[] body = TEncoding.Unicode.GetBytes("中文行");
        string path = WriteBytes("u16.txt", bom.Concat(body).ToArray());
        var l = new TStringList();
        l.LoadFromFile(path);
        Assert.Equal("中文行", l[0]);
        Assert.Equal(1200, l.GetEncoding().CodePage);
    }

    [Fact]
    public void LoadFromFile_SameTextDifferentEncodings_YieldsSameInflatedLines()
    {
        // ★ 差异断言：同一段"文本"用 3 种编码落盘，嗅探后必须都能还原（编码对象不同、结果相同）
        string gbk = WriteBytes("a_gbk.txt", EncodingInit.GBK.GetBytes("甲\r\n乙"));
        string u8 = WriteBytes("a_u8.txt", TEncoding.UTF8.GetBytes("甲\r\n乙"));
        string u16 = WriteBytes("a_u16.txt", TEncoding.Unicode.GetPreamble()
            .Concat(TEncoding.Unicode.GetBytes("甲\r\n乙")).ToArray());

        foreach (string path in new[] { gbk, u8, u16 })
        {
            var l = new TStringList();
            l.LoadFromFile(path);
            Assert.Equal(new[] { "甲", "乙" }, new[] { l[0], l[1] });
        }
    }

    [Fact]
    public void LoadFromFile_MissingFile_LeavesEmptyAndDoesNotThrow()
    {
        // 偏离 D-P8-12：保持既有托管行为（Delphi 会抛 EFOpenError）
        var l = new TStringList();
        l.Add("stale");
        l.LoadFromFile(Path.Combine(_dir, "nope.txt"));
        Assert.Equal(0, l.Count);
    }

    [Fact]
    public void LoadFromFile_EmptyFile_YieldsNoLines()
    {
        string path = WriteBytes("empty.txt", Array.Empty<byte>());
        var l = new TStringList();
        l.LoadFromFile(path);
        Assert.Equal(0, l.Count);
    }

    [Fact]
    public void SaveToFile_DefaultGbkEncoding_WritesByteIdenticalToLegacyPath()
    {
        // 从未 LoadFrom* 的列表 → GetEncoding() = GBK（preamble 为空）→ 与旧实现字节一致
        var l = new TStringList();
        l.Add("甲");
        l.Add("乙");
        string path = Path.Combine(_dir, "s_gbk.txt");
        l.SaveToFile(path);
        Assert.Equal(EncodingInit.GBK.GetBytes("甲\r\n乙\r\n"), File.ReadAllBytes(path));
    }

    [Fact]
    public void SaveToFile_AfterLoadingUtf8_WithBom_RoundTripsBomAndBytes()
    {
        byte[] original = TEncoding.UTF8.GetPreamble().Concat(TEncoding.UTF8.GetBytes("甲\r\n")).ToArray();
        string src = WriteBytes("rt_u8bom.txt", original);
        var l = new TStringList();
        l.LoadFromFile(src);

        string dst = Path.Combine(_dir, "rt_out.txt");
        l.SaveToFile(dst);
        Assert.Equal(original, File.ReadAllBytes(dst));                   // BOM + UTF-8 字节原样写回
    }

    [Fact]
    public void SaveToFile_AfterLoadingUtf8NoBom_DoesNotAddBom()
    {
        byte[] original = TEncoding.UTF8.GetBytes("甲\r\n");
        string src = WriteBytes("rt_u8.txt", original);
        var l = new TStringList();
        l.LoadFromFile(src);

        string dst = Path.Combine(_dir, "rt_out2.txt");
        l.SaveToFile(dst);
        Assert.Equal(original, File.ReadAllBytes(dst));                   // NoBomUTF8 → 不补 BOM
    }

    [Fact]
    public void LoadThenSaveThenLoad_IsStable()
    {
        string src = WriteBytes("stable.txt", TEncoding.UTF8.GetBytes("一\r\n\r\n三\r\n"));
        var a = new TStringList();
        a.LoadFromFile(src);
        string mid = Path.Combine(_dir, "stable2.txt");
        a.SaveToFile(mid);
        var b = new TStringList();
        b.LoadFromFile(mid);
        Assert.Equal(a.Count, b.Count);
        Assert.Equal(a.Text, b.Text);
        Assert.Equal(3, b.Count);
        Assert.Equal("", b[1]);
    }
}
