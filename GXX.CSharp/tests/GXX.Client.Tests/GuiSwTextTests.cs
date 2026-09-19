using System;
using System.Collections.Generic;
using GXX.Client.GUI.NewStateWin;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 并行批次 P1 切片 F：StateWindows.pas 顶层文本函数
/// （GetGodBlessItemCaption 10776-10804 / GetHitLines 10808-10841 行）。
/// 期望值由脚本按原文逐字重算后回写。
/// </summary>
public sealed class GuiSwTextTests
{
    private const int DefColor = 999;

    private static IReadOnlyList<string> Cfg(params string[] lines) => lines;

    private static string Caption(int index, Func<string, IReadOnlyList<string>> cfg)
        => TStateWindowsText.GetGodBlessItemCaption(index, cfg);

    private static List<TStateWindowsText.HintLine> Hit(string s)
        => TStateWindowsText.GetHitLines(s, DefColor);

    private static string Describe(List<TStateWindowsText.HintLine> lines)
    {
        var parts = new List<string>();
        foreach (var l in lines)
            parts.Add("[" + l.Text + "]#" + l.Color);
        return string.Join("|", parts);
    }

    // ===================== GetGodBlessItemCaption =====================

    [Fact]
    public void CaptionsTableHasTwelveZodiacEntries()
    {
        Assert.Equal(12, TStateWindowsText.GodBlessCaptions.Length);
        Assert.Equal("子鼠", TStateWindowsText.GodBlessCaptions[0]);
        Assert.Equal("亥猪", TStateWindowsText.GodBlessCaptions[11]);
    }

    [Fact]
    public void OriginalTypoYiSheIsPreserved()
    {
        // 原文如此（StateWindows.pas:10784）：'已蛇'（地支本字为「巳」），照抄不改。
        Assert.Equal("已蛇", TStateWindowsText.GodBlessCaptions[5]);
    }

    [Fact]
    public void FallsBackToCaptionPlusShenYouGe()
    {
        // 配置全部为 null → Captions[Index] + '神佑格'
        Assert.Equal("子鼠神佑格", Caption(0, _ => null));
        Assert.Equal("亥猪神佑格", Caption(11, _ => null));
        Assert.Equal("已蛇神佑格", Caption(5, _ => null));
    }

    [Fact]
    public void UsesConfiguredTitleWhenPresent()
    {
        string asked = null;
        string result = Caption(3, key => { asked = key; return Cfg("自定义标题", "第二行"); });

        Assert.Equal("title4", asked);                 // 原文 'title' + IntToStr(Index + 1)
        Assert.Equal("自定义标题", result);            // 只取第 0 行
    }

    [Fact]
    public void EmptyConfiguredListFallsBack()
    {
        Assert.Equal("卯兔神佑格", Caption(3, _ => new List<string>()));
    }

    [Fact]
    public void OutOfRangeIndexUsesShenYouGePrefixNotCaptions()
    {
        // 差异断言：越界走 '神佑格' + IntToStr(Index + 1)（前缀在后），与区间内格式相反。
        Assert.Equal("神佑格13", Caption(12, _ => null));
        Assert.Equal("神佑格0", Caption(0 - 1, _ => null));   // Index = -1 → IntToStr(0)
        Assert.Equal("神佑格-4", Caption(-5, _ => null));
    }

    [Fact]
    public void OutOfRangeIndexDoesNotConsultConfig()
    {
        bool consulted = false;
        string result = Caption(99, _ => { consulted = true; return Cfg("X"); });

        Assert.False(consulted);
        Assert.Equal("神佑格100", result);
    }

    [Fact]
    public void BoundaryIndicesAreInsideTable()
    {
        Assert.Equal("子鼠神佑格", Caption(0, _ => null));
        Assert.Equal("亥猪神佑格", Caption(11, _ => null));
        Assert.Equal("神佑格13", Caption(12, _ => null));
    }

    // ===================== GetHitLines =====================

    [Fact]
    public void PlainTextYieldsSingleLineWithDefaultColor()
    {
        Assert.Equal("[纯文字]#999", Describe(Hit("纯文字")));
    }

    [Fact]
    public void EmptyStringStillYieldsOneLine()
    {
        // 差异断言：原文 Pos = 0 分支无条件 Add，空串也会产生一行（不是空列表）。
        var lines = Hit("");
        Assert.Single(lines);
        Assert.Equal(string.Empty, lines[0].Text);
        Assert.Equal(DefColor, lines[0].Color);
    }

    [Fact]
    public void ColorMarkerAppliesToFollowingSegment()
    {
        // 颜色标记作用于**其后**的文本，且前一段用旧颜色。
        Assert.Equal("[abc]#999|[def]#255", Describe(Hit("abc/255/def")));
    }

    [Fact]
    public void MultipleColorMarkersChain()
    {
        Assert.Equal("[abc]#999|[def]#255|[x]#255", Describe(Hit("abc/255/def/0/x")));
    }

    [Fact]
    public void SingleDigitColorMarkerIsConsumedButNotParsed()
    {
        // 差异断言（原文 10824-10825 的钳位）：'/7/' 只有 1 位数字，nPos - 4 = 0 < 1，
        // 于是回退扫描得到 I = 1，颜色取 Copy(S, I+1, nPos-I-1) = Copy(S, 2, 0) = '' 
        // → StrToIntDef('', 255) = 255。即「一位数颜色值**永远解析不出来**，且该 '/' 两侧的
        // 数字会被当成前一段的文本」。
        Assert.Equal("[a]#999|[b]#255", Describe(Hit("a/7/b")));
    }

    [Fact]
    public void ParsedColorIsPassedThroughResolveFunction()
    {
        // resolve 收到的是**解析出来的颜色值**（此处 255 与 234），并被写回后续段的颜色。
        var seen = new List<int>();
        var lines = TStateWindowsText.GetHitLines("ab/1234/x", DefColor, v => { seen.Add(v); return v * 2; });

        Assert.Equal(new[] { 255, 234 }, seen);
        Assert.Equal("[ab]#999|[1]#510|[x]#468", Describe(lines));
    }

    [Fact]
    public void FourDigitColorMarkerIsNotParsedAndLosesItsDigits()
    {
        // 差异断言（原文 10820-10825 的真实行为，已由逐步跟踪确认）：
        //   '/1234/' 的 4 位数字起点 = nPos-4，回退扫描实际停在该位置的**左一位** I = nPos-5，
        //   再被 `if I < nPos - 4 then I := nPos - 4` 钳回 nPos-4 = 1  →
        //   颜色取 Copy(S, 2, 0) = '' → 255；而 '1' 落进下一段文本，'234' 成了新的颜色值。
        // 结论：**任何**颜色标记的前 1 位数字都会被吞进文本，最后 3 位数字才当作颜色。
        var seen = new List<int>();
        var lines = TStateWindowsText.GetHitLines("ab/1234/x", DefColor, v => { seen.Add(v); return v; });

        Assert.Equal(new[] { 255, 234 }, seen);
        Assert.Equal("[ab]#999|[1]#255|[x]#234", Describe(lines));
    }

    [Fact]
    public void FourDigitColorIsLostWhenTextPrefixIsTooShort()
    {
        // 差异断言：'abcd/1234/x'（nPos = 5）时回退扫描停在 I = 1（= nPos-4）之前被钳到 1，
        // 取 Copy(S, 2, 0) = '' → 第一个颜色标记退化为 255，且 'bcd' 被当成文本吃掉；
        // 残留的 '234' 又在下一轮成为新的颜色值。
        var seen = new List<int>();
        var lines = TStateWindowsText.GetHitLines("abcd/1234/x", DefColor, v => { seen.Add(v); return v; });

        Assert.Equal(new[] { 255, 234 }, seen);
        Assert.Equal("[abcd]#999|[1]#255|[x]#234", Describe(lines));
    }

    [Fact]
    public void NonNumericColorFallsBackTo255()
    {
        // 原文 10832：StrToIntDef(..., 255)。'a/xy/b' 两段都会各解析一次（都取不到数字）。
        var seen = new List<int>();
        TStateWindowsText.GetHitLines("a/xy/b", DefColor, v => { seen.Add(v); return v; });

        Assert.Equal(new[] { 255, 255 }, seen);
    }

    [Fact]
    public void LeadingColorMarkerAtPositionOneIsSwallowed()
    {
        // 原文 10826-10829：I = 0 分支只丢弃该 '/'，不加行。
        var lines = Hit("/255/abc");
        Assert.Single(lines);
        Assert.Equal("abc", lines[0].Text);
        Assert.Equal(DefColor, lines[0].Color);
    }

    [Fact]
    public void LoneSlashYieldsNothing()
    {
        Assert.Empty(Hit("/"));
    }

    [Fact]
    public void TrailingSlashYieldsSingleLine()
    {
        Assert.Equal("[abc]#999", Describe(Hit("abc/")));
    }

    [Fact]
    public void LongColorRunIsSplitAtFiveDigits()
    {
        // 差异断言：I 被钳到 nPos-4，颜色串最多 4 位；S 被截成 5 个数字当**内容**。
        Assert.Equal("[abc]#999|[12345]#255|[x]#678", Describe(Hit("abc/12345678/x")));
    }

    [Fact]
    public void EightDigitColorLeavesThreeDigitColorForTail()
    {
        // 同上：16711680 → 前面被切成 '16711'（内容），后面 '680' 作为颜色。
        Assert.Equal("[文字]#999|[16711]#255|[红]#680", Describe(Hit("文字/16711680/红")));
    }

    [Fact]
    public void NullInputIsTreatedAsEmptyNotThrown()
    {
        var lines = TStateWindowsText.GetHitLines(null, DefColor);
        Assert.Single(lines);
        Assert.Equal(string.Empty, lines[0].Text);
    }

    [Fact]
    public void HintLineCarriesTextAndColor()
    {
        var line = new TStateWindowsText.HintLine("abc", 123);
        Assert.Equal("abc", line.Text);
        Assert.Equal(123, line.Color);
    }
}
