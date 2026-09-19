using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J108：MonGen.txt 解析层（LocalDB.pas 3528-3598 + HUtil32.pas 工具）1:1 测试。
/// </summary>
public sealed class MonGenParseCoreTests
{
    // ===================== CompareLStr（1981-1995） =====================

    [Fact]
    public void CompareLStrIsCaseInsensitive()
    {
        Assert.True(MonGenParseCore.CompareLStr("loadgen", "LOADGEN", 7));
        Assert.True(MonGenParseCore.CompareLStr("AbC", "aBc", 3));
    }

    [Fact]
    public void CompareLStrComparesPrefixOnly()
    {
        // 只比前 compn 个字符
        Assert.True(MonGenParseCore.CompareLStr("loadgen extra", "loadgen", 7));
    }

    [Fact]
    public void CompareLStrFailsWhenShorterThanCompn()
    {
        // 1986：任一侧长度不足即假
        Assert.False(MonGenParseCore.CompareLStr("load", "loadgen", 7));
        Assert.False(MonGenParseCore.CompareLStr("loadgen", "load", 7));
    }

    [Fact]
    public void CompareLStrFailsWhenCompnNotPositive()
    {
        // 1986：compn <= 0 即假（**空比较也算假**）
        Assert.False(MonGenParseCore.CompareLStr("abc", "abc", 0));
        Assert.False(MonGenParseCore.CompareLStr("abc", "abc", -1));
    }

    [Fact]
    public void CompareLStrMismatchAtFirstChar()
    {
        Assert.False(MonGenParseCore.CompareLStr("xoadgen", "loadgen", 7));
    }

    [Fact]
    public void IsLoadGenLineUsesLengthSeven()
    {
        Assert.True(MonGenParseCore.IsLoadGenLine("loadgen a.txt"));
        Assert.True(MonGenParseCore.IsLoadGenLine("LoadGen a.txt"));

        // 长度不足 7 → 假（1986 的长度门槛）
        Assert.False(MonGenParseCore.IsLoadGenLine("loadge a.txt"));
    }

    [Fact]
    public void IsLoadGenLineMatchesPrefixRegardlessOfRest()
    {
        // **前缀匹配即真**：'loadgenx' 的前 7 字符是 'loadgen'，故为真。
        // 这是 CompareLStr 的固有语义（只比前 compn 个字符），
        // 而非"整词相等"——原文 3517 正是靠这一点识别 `loadgen` 指令行。
        Assert.True(MonGenParseCore.IsLoadGenLine("loadgenx a.txt"));
    }

    // ===================== ShouldProcessLine（3531） =====================

    [Fact]
    public void EmptyLineSkipped()
    {
        Assert.False(MonGenParseCore.ShouldProcessLine(""));
    }

    [Fact]
    public void SemicolonLineSkipped()
    {
        Assert.False(MonGenParseCore.ShouldProcessLine("; a comment"));
    }

    [Fact]
    public void LeadingSpaceThenSemicolonIsNotComment()
    {
        // **关键**：3531 只查 sLineText[1]，故行首空格后的 ';' **不**算注释
        Assert.True(MonGenParseCore.ShouldProcessLine(" ; not a comment"));
    }

    [Fact]
    public void NormalLineProcessed()
    {
        Assert.True(MonGenParseCore.ShouldProcessLine("0 300 300 鸡 10 5 10"));
    }

    // ===================== GetValidStr3（1243-1341） =====================

    [Fact]
    public void GetValidStr3SplitsOnFirstDivider()
    {
        string rest = MonGenParseCore.GetValidStr3("abc def", out string dest, ' ');
        Assert.Equal("abc", dest);
        Assert.Equal("def", rest);
    }

    [Fact]
    public void GetValidStr3SkipsLeadingDividers()
    {
        // 1279-1292：前导分隔符全部跳过
        string rest = MonGenParseCore.GetValidStr3("   abc def", out string dest, ' ');
        Assert.Equal("abc", dest);
        Assert.Equal("def", rest);
    }

    [Fact]
    public void GetValidStr3CollapsesConsecutiveDividers()
    {
        // 前导多个分隔符（不限于一个）全被丢弃
        string rest = MonGenParseCore.GetValidStr3("  \t abc x", out string dest, ' ', '\t');
        Assert.Equal("abc", dest);
        Assert.Equal("x", rest);
    }

    [Fact]
    public void GetValidStr3NoDividerReturnsWholeAsDest()
    {
        string rest = MonGenParseCore.GetValidStr3("abc", out string dest, ' ');
        Assert.Equal("abc", dest);
        Assert.Equal("", rest);
    }

    [Fact]
    public void GetValidStr3SingleTokenThenDivider()
    {
        // 1296-1299 路径不会走到（因 1283 已 Exit）；验证 'abc ' 的行为
        string rest = MonGenParseCore.GetValidStr3("abc ", out string dest, ' ');
        Assert.Equal("abc", dest);
        Assert.Equal("", rest);
    }

    [Fact]
    public void GetValidStr3EmptyReturnsEmpty()
    {
        string rest = MonGenParseCore.GetValidStr3("", out string dest, ' ');
        Assert.Equal("", dest);
        Assert.Equal("", rest);
    }

    [Fact]
    public void GetValidStr3EmptyDividersReturnsWholeAsDest()
    {
        // 1258：DividerCount = 0 → 直接 Exit，Dest 保持为整串
        string rest = MonGenParseCore.GetValidStr3("abc", out string dest);
        Assert.Equal("abc", dest);
        Assert.Equal("", rest);
    }

    [Fact]
    public void GetValidStr3MultipleDividersAnyMatches()
    {
        string rest = MonGenParseCore.GetValidStr3("abc\tdef", out string dest, ' ', '\t');
        Assert.Equal("abc", dest);
        Assert.Equal("def", rest);
    }

    [Fact]
    public void GetValidStr3PreservesInnerDividersInRest()
    {
        // 剩余串保留内部结构
        string rest = MonGenParseCore.GetValidStr3("a b  c", out string dest, ' ');
        Assert.Equal("a", dest);
        Assert.Equal("b  c", rest);
    }

    // ===================== GetValidStrCap（1344-1361） =====================

    [Fact]
    public void GetValidStrCapHandlesQuotedName()
    {
        // 3541：怪物名支持引号
        string rest = MonGenParseCore.GetValidStrCap("\"鸡 王\" 10 5", out string dest, ' ', '\t');
        Assert.Equal("鸡 王", dest);
        Assert.Contains("10", rest);
    }

    [Fact]
    public void GetValidStrCapUnquotedFallsBackToStr3()
    {
        string rest = MonGenParseCore.GetValidStrCap("鸡 10 5", out string dest, ' ', '\t');
        Assert.Equal("鸡", dest);
        Assert.Equal("10 5", rest);
    }

    [Fact]
    public void GetValidStrCapTrimLeftFirst()
    {
        // 1346：先 TrimLeft
        string rest = MonGenParseCore.GetValidStrCap("   鸡 10", out string dest, ' ', '\t');
        Assert.Equal("鸡", dest);
        Assert.Equal("10", rest);
    }

    [Fact]
    public void GetValidStrCapEmptyGivesEmptyDest()
    {
        // 1356-1360
        string rest = MonGenParseCore.GetValidStrCap("", out string dest, ' ', '\t');
        Assert.Equal("", dest);
        Assert.Equal("", rest);
    }

    [Fact]
    public void QuotedNameWithoutCloseQuoteIsTolerated()
    {
        // 引号未闭合时不崩溃
        string rest = MonGenParseCore.GetValidStrCap("\"鸡 10", out string dest, ' ', '\t');
        Assert.Equal("鸡", dest);
        Assert.Equal("10", rest);
    }

    // ===================== IsStringNumber =====================

    [Fact]
    public void IsStringNumberTrueForDigits()
    {
        Assert.True(MonGenParseCore.IsStringNumber("123"));
        Assert.True(MonGenParseCore.IsStringNumber("0"));
    }

    [Fact]
    public void IsStringNumberFalseForEmpty()
    {
        Assert.False(MonGenParseCore.IsStringNumber(""));
    }

    [Fact]
    public void IsStringNumberFalseForNegativeOrMixed()
    {
        // **注意**：负号**不是**数字字符，故 '-1' 为假
        Assert.False(MonGenParseCore.IsStringNumber("-1"));
        Assert.False(MonGenParseCore.IsStringNumber("1a"));
        Assert.False(MonGenParseCore.IsStringNumber(" 1"));
    }

    // ===================== ParseBoolField =====================

    [Fact]
    public void BoolFieldFalseForEmpty()
    {
        Assert.False(MonGenParseCore.ParseBoolField(""));
    }

    [Fact]
    public void BoolFieldFalseForLiteralZero()
    {
        Assert.False(MonGenParseCore.ParseBoolField("0"));
    }

    [Fact]
    public void BoolFieldTrueForOne()
    {
        Assert.True(MonGenParseCore.ParseBoolField("1"));
    }

    [Fact]
    public void BoolFieldTrueForArbitraryNonZeroText()
    {
        // **关键**：只说"非空且非 '0'"，故 'false'/'no'/'00' 都为**真**
        Assert.True(MonGenParseCore.ParseBoolField("false"));
        Assert.True(MonGenParseCore.ParseBoolField("no"));
        Assert.True(MonGenParseCore.ParseBoolField("00"));
        Assert.True(MonGenParseCore.ParseBoolField("-1"));
    }

    // ===================== ParseCompareType（3582-3595） =====================

    [Fact]
    public void CompareTypeAllSixMapped()
    {
        Assert.Equal(MonGenParseCore.CompareType.ctLess, MonGenParseCore.ParseCompareType("<"));
        Assert.Equal(MonGenParseCore.CompareType.ctEqual, MonGenParseCore.ParseCompareType("="));
        Assert.Equal(MonGenParseCore.CompareType.ctGreater, MonGenParseCore.ParseCompareType(">"));
        Assert.Equal(MonGenParseCore.CompareType.ctLessEqual, MonGenParseCore.ParseCompareType("<="));
        Assert.Equal(MonGenParseCore.CompareType.ctGreaterEqual, MonGenParseCore.ParseCompareType(">="));
        Assert.Equal(MonGenParseCore.CompareType.ctNotEqual, MonGenParseCore.ParseCompareType("<>"));
    }

    [Fact]
    public void CompareTypeUnknownIsFail()
    {
        Assert.Equal(MonGenParseCore.CompareType.ctFail, MonGenParseCore.ParseCompareType("=="));
        Assert.Equal(MonGenParseCore.CompareType.ctFail, MonGenParseCore.ParseCompareType(""));
        Assert.Equal(MonGenParseCore.CompareType.ctFail, MonGenParseCore.ParseCompareType("x"));
    }

    [Fact]
    public void CompareTypeDoubleCharFormsAreDistinct()
    {
        // 6 个合法值互不相同
        var all = new[]
        {
            MonGenParseCore.ParseCompareType("<"), MonGenParseCore.ParseCompareType("="),
            MonGenParseCore.ParseCompareType(">"), MonGenParseCore.ParseCompareType("<="),
            MonGenParseCore.ParseCompareType(">="), MonGenParseCore.ParseCompareType("<>"),
        };

        for (int i = 0; i < all.Length; i++)
        for (int j = i + 1; j < all.Length; j++)
            Assert.NotEqual(all[i], all[j]);
    }

    // ===================== ParseLine 全字段 =====================

    [Fact]
    public void ParseLineAllSixteenFields()
    {
        // 地图 X Y "怪名" 范围 数量 秒 集中率 颜色 内功 国家 攻同国 异国PK 被同国攻 Trigger G序号 比较 值
        var r = MonGenParseCore.ParseLine(
            "0 300 400 \"祖玛教主\" 10 5 60 30 200 1 炎黄 1 0 1 @Trig1 G5 >= 100");

        Assert.Equal("0", r.MapName);
        Assert.Equal(300, r.X);
        Assert.Equal(400, r.Y);
        Assert.Equal("祖玛教主", r.MonName);
        Assert.Equal(10, r.Range);
        Assert.Equal(5, r.Count);
        Assert.Equal(60 * 60 * 1000, r.ZenTimeMs);   // 60 分钟 → 毫秒
        Assert.Equal(30, r.MissionGenRate);
        Assert.Equal(200, r.NameColor);
        Assert.True(r.IsNGMon);
        Assert.Equal("炎黄", r.NationaID);
        Assert.True(r.CanAttackSameNationPlayer);
        Assert.False(r.NoSameNationMonPK);           // '0' → 假
        Assert.True(r.AllowSameNationPlayerAttack);
        Assert.Equal("@Trig1", r.TriggerScript);
        Assert.Equal(5, r.GVarIndex);
        Assert.Equal(MonGenParseCore.CompareType.ctGreaterEqual, r.GVarCompareType);
        Assert.Equal(100, r.GVarValue);
    }

    [Fact]
    public void ParseLineMinimalLine()
    {
        // 仅前 7 个字段
        var r = MonGenParseCore.ParseLine("0 100 100 鸡 5 3 10");

        Assert.Equal("0", r.MapName);
        Assert.Equal(100, r.X);
        Assert.Equal(100, r.Y);
        Assert.Equal("鸡", r.MonName);
        Assert.Equal(5, r.Range);
        Assert.Equal(3, r.Count);
        Assert.Equal(10 * 60 * 1000, r.ZenTimeMs);
    }

    [Fact]
    public void ZenTimeMultipliesBySixtyThousand()
    {
        var r = MonGenParseCore.ParseLine("0 1 1 鸡 1 1 7");
        Assert.Equal(420_000, r.ZenTimeMs);   // 7 * 60 * 1000
    }

    [Fact]
    public void MissingZenTimeYieldsNegativeSixtyThousand()
    {
        // **关键**：3550 默认 -1 → -1 * 60 * 1000 = -60000
        var r = MonGenParseCore.ParseLine("0 1 1 鸡 1 1");

        Assert.Equal(-60_000, r.ZenTimeMs);
        Assert.Equal(MonGenParseCore.DefaultZenTimeMinutes, -1);
    }

    [Fact]
    public void MissingZenTimePassesDiscardCheck()
    {
        // **行为后果**：3572 只判 `= 0`，故 -60000 **能通过**校验
        var r = MonGenParseCore.ParseLine("0 1 1 鸡 1 1");

        Assert.NotEqual(0, r.ZenTimeMs);
        Assert.False(MonGenParseCore.ShouldDiscardLine(r));
    }

    [Fact]
    public void ExplicitZeroZenTimeIsDiscarded()
    {
        // 3572：dwZenTime = 0 → 丢弃
        var r = MonGenParseCore.ParseLine("0 1 1 鸡 1 1 0");
        Assert.Equal(0, r.ZenTimeMs);
        Assert.True(MonGenParseCore.ShouldDiscardLine(r));
    }

    [Fact]
    public void NameColorDefaultsTo255()
    {
        // 3556
        var r = MonGenParseCore.ParseLine("0 1 1 鸡 1 1 10");
        Assert.Equal(255, r.NameColor);
        Assert.Equal(255, MonGenParseCore.DefaultNameColor);
    }

    [Fact]
    public void GVarIndexDefaultsToMinusOne()
    {
        // 3580
        var r = MonGenParseCore.ParseLine("0 1 1 鸡 1 1 10");
        Assert.Equal(-1, r.GVarIndex);
        Assert.Equal(-1, MonGenParseCore.DefaultGVarIndex);
    }

    [Fact]
    public void GVarValueDefaultsToZero()
    {
        // 3598：与 GVarIndex 的 -1 **不同**
        var r = MonGenParseCore.ParseLine("0 1 1 鸡 1 1 10");
        Assert.Equal(0, r.GVarValue);
    }

    [Fact]
    public void RangeAndCountDefaultToZero()
    {
        var r = MonGenParseCore.ParseLine("0 1 1 鸡");
        Assert.Equal(0, r.Range);
        Assert.Equal(0, r.Count);
    }

    [Fact]
    public void MissionGenRateNonNumericBecomesZero()
    {
        // 3552-3553：非数字强制 '0'
        var r = MonGenParseCore.ParseLine("0 1 1 鸡 1 1 10 abc");
        Assert.Equal(0, r.MissionGenRate);
    }

    [Fact]
    public void MissionGenRateNumericParsed()
    {
        var r = MonGenParseCore.ParseLine("0 1 1 鸡 1 1 10 42");
        Assert.Equal(42, r.MissionGenRate);
    }

    [Fact]
    public void TriggerScriptRequiresAtSign()
    {
        // 3568-3571
        var withAt = MonGenParseCore.ParseLine("0 1 1 鸡 1 1 10 0 255 0 0 0 0 0 @MyScript");
        Assert.Equal("@MyScript", withAt.TriggerScript);

        var withoutAt = MonGenParseCore.ParseLine("0 1 1 鸡 1 1 10 0 255 0 0 0 0 0 MyScript");
        Assert.Equal("", withoutAt.TriggerScript);
    }

    [Fact]
    public void GVarIndexAcceptsGPrefix()
    {
        // 3578-3579：'G' 前缀被剥掉
        var withG = MonGenParseCore.ParseLine("0 1 1 鸡 1 1 10 0 255 0 0 0 0 0 0 G7 = 1");
        Assert.Equal(7, withG.GVarIndex);

        var withoutG = MonGenParseCore.ParseLine("0 1 1 鸡 1 1 10 0 255 0 0 0 0 0 0 7 = 1");
        Assert.Equal(7, withoutG.GVarIndex);
    }

    [Fact]
    public void GVarIndexBareGBecomesMinusOne()
    {
        // 'G' 剥掉后为空 → StrToIntDef(-1) → -1
        var r = MonGenParseCore.ParseLine("0 1 1 鸡 1 1 10 0 255 0 0 0 0 0 0 G = 1");
        Assert.Equal(-1, r.GVarIndex);
    }

    [Fact]
    public void QuotedMonsterNameWithSpaces()
    {
        var r = MonGenParseCore.ParseLine("0 1 1 \"赤月 恶魔\" 5 2 10");
        Assert.Equal("赤月 恶魔", r.MonName);
        Assert.Equal(5, r.Range);
    }

    [Fact]
    public void TabSeparatedFields()
    {
        var r = MonGenParseCore.ParseLine("0\t300\t400\t鸡\t10\t5\t60");
        Assert.Equal("0", r.MapName);
        Assert.Equal(300, r.X);
        Assert.Equal(400, r.Y);
        Assert.Equal("鸡", r.MonName);
        Assert.Equal(60 * 60 * 1000, r.ZenTimeMs);
    }

    [Fact]
    public void InvalidNumbersFallBackToDefaults()
    {
        var r = MonGenParseCore.ParseLine("0 abc def 鸡 xyz pqr 10");
        Assert.Equal(0, r.X);
        Assert.Equal(0, r.Y);
        Assert.Equal(0, r.Range);
        Assert.Equal(0, r.Count);
    }

    // ===================== ShouldDiscardLine（3572） =====================

    [Fact]
    public void DiscardWhenMapNameEmpty()
    {
        var r = new MonGenParseCore.MonGenLine { MapName = "", MonName = "鸡", ZenTimeMs = 1000 };
        Assert.True(MonGenParseCore.ShouldDiscardLine(r));
    }

    [Fact]
    public void DiscardWhenMonNameEmpty()
    {
        var r = new MonGenParseCore.MonGenLine { MapName = "0", MonName = "", ZenTimeMs = 1000 };
        Assert.True(MonGenParseCore.ShouldDiscardLine(r));
    }

    [Fact]
    public void DiscardWhenZenTimeZero()
    {
        var r = new MonGenParseCore.MonGenLine { MapName = "0", MonName = "鸡", ZenTimeMs = 0 };
        Assert.True(MonGenParseCore.ShouldDiscardLine(r));
    }

    [Fact]
    public void KeepWhenAllThreePresent()
    {
        var r = new MonGenParseCore.MonGenLine { MapName = "0", MonName = "鸡", ZenTimeMs = 1000 };
        Assert.False(MonGenParseCore.ShouldDiscardLine(r));
    }

    [Fact]
    public void KeepWhenZenTimeNegative()
    {
        // **关键**：只判 = 0，负值保留
        var r = new MonGenParseCore.MonGenLine { MapName = "0", MonName = "鸡", ZenTimeMs = -60_000 };
        Assert.False(MonGenParseCore.ShouldDiscardLine(r));
    }

    // ===================== ParseAll（3528-3674） =====================

    [Fact]
    public void ParseAllSkipsCommentsAndEmptyLines()
    {
        var lines = new List<string>
        {
            "; comment",
            "",
            "0 1 1 鸡 1 1 10",
        };

        Assert.Single(MonGenParseCore.ParseAll(lines));
    }

    [Fact]
    public void ParseAllDiscardsInvalidRows()
    {
        var lines = new List<string>
        {
            "0 1 1 鸡 1 1 10",     // 保留
            "0 1 1 鸡 1 1 0",      // zenTime=0 → 丢弃
        };

        Assert.Single(MonGenParseCore.ParseAll(lines));
    }

    [Fact]
    public void ParseAllPreservesOrder()
    {
        var lines = new List<string>
        {
            "0 1 1 鸡 1 1 10",
            "1 2 2 鹿 1 1 20",
            "2 3 3 猪 1 1 30",
        };

        var r = MonGenParseCore.ParseAll(lines);

        Assert.Equal(3, r.Count);
        Assert.Equal("鸡", r[0].MonName);
        Assert.Equal("鹿", r[1].MonName);
        Assert.Equal("猪", r[2].MonName);
    }

    [Fact]
    public void ParseAllEmptyInput()
    {
        Assert.Empty(MonGenParseCore.ParseAll(new List<string>()));
    }

    [Fact]
    public void SpacePrefixedCommentIsParsedAsData()
    {
        // **差异保护**：行首有空格时 ';' 不当注释，故该行会被当数据解析
        // 并在 3572 处因 MonName 为空之类原因丢弃，而非被注释规则跳过
        var lines = new List<string> { " ; comment" };

        // 被解析（非注释跳过），但因字段不合格被丢弃
        Assert.Empty(MonGenParseCore.ParseAll(lines));
        Assert.True(MonGenParseCore.ShouldProcessLine(" ; comment"));   // 确实进入了处理
    }
}
