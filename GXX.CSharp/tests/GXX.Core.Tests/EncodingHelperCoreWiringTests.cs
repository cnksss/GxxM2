using GXX.Core.Util;
using Xunit;

namespace GXX.Core.Tests;

/// <summary>
/// 车道 p8-m2-itemprop-misc：**请求 #2 落地**的 Core 侧测试 ——
/// `FastIniFile.pas` 的固定格式日期时间（<c>FIXED_*</c> 常量、<c>StrToDateDef/StrToTimeDef</c>、
/// <c>TFastIniFile.ReadFixedDateTime/WriteFixedDateTime</c>）1:1 断言。
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
}
