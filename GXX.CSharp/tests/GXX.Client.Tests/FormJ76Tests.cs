using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次J76：Actor.pas TActor.Say（9846-9932）喊话标记剥离与双字节感知换行 1:1 测试。
/// 字体度量以固定宽度委托注入，从而把换行位置变成可精确断言的确定性结果。
/// </summary>
public sealed class ActorSayTests : IDisposable
{
    private readonly Func<string, int> _savedWidth;
    private readonly uint _savedTickRestore;

    public ActorSayTests()
    {
        _savedWidth = TActorCore.TextWidthFn;
        _savedTickRestore = 0;
    }

    public void Dispose()
    {
        TActorCore.TextWidthFn = _savedWidth;
        SceneTime.TickNow = () => (uint)Environment.TickCount;
    }

    /// <summary>每字符固定宽度（ASCII 单字节按 1 计）——用于精确控制换行点。</summary>
    private static void UseFixedWidth(int perChar) => TActorCore.TextWidthFn = s => s.Length * perChar;

    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchOriginal()
    {
        Assert.Equal(150, ActorSay.MaxWidth);
        Assert.Equal(5, ActorSay.MaxSay);
    }

    // ===================== 标记剥离 =====================

    [Fact]
    public void StripColorMarkup_NoBraceReturnsInputVerbatim()
    {
        Assert.Equal("普通文本", ActorSay.StripColorMarkup("普通文本"));
        Assert.Equal("", ActorSay.StripColorMarkup(""));
        Assert.Equal("a}b", ActorSay.StripColorMarkup("a}b"));   // 无 '{' → 完全不解析
    }

    [Fact]
    public void StripColorMarkup_RemovesPairedMarkupEntirely()
    {
        // {自定义文字颜色|249:0}aa
        Assert.Equal("aa", ActorSay.StripColorMarkup("{自定义文字颜色|249:0}aa"));
        // 前缀 + 标记 + 后缀
        Assert.Equal("前中后", ActorSay.StripColorMarkup("前{自定义文字颜色|249:0}中{abc|1:2}后"));
    }

    [Fact]
    public void StripColorMarkup_UnpairedBraceKeepsBraceLiteral()
    {
        // 无 '}' → ProcessCustomColor 返回 False，'{' 作普通字符保留
        Assert.Equal("{未闭合文本", ActorSay.StripColorMarkup("{未闭合文本"));
        Assert.Equal("a{b", ActorSay.StripColorMarkup("a{b"));
    }

    [Fact]
    public void StripColorMarkup_EmptyMarkupIsRemoved()
    {
        Assert.Equal("ab", ActorSay.StripColorMarkup("a{}b"));
    }

    [Fact]
    public void StripColorMarkup_TakesFirstClosingBrace()
    {
        // 原文 Pos('}') 取首个 '}'，因此内层 '{' 一并被吞掉
        Assert.Equal("xy", ActorSay.StripColorMarkup("{a{b}xy"));
    }

    // ===================== 换行：ASCII =====================

    [Fact]
    public void PlanSay_ShortTextYieldsSingleLine()
    {
        UseFixedWidth(1);                                  // MAXWIDTH 150 → 最多 150 字符

        var layout = ActorSay.PlanSay("hello", TActorCore.TextWidthFn);

        Assert.Single(layout.Lines);
        Assert.Equal(0, layout.Lines[0].Index);
        Assert.Equal("hello", layout.Lines[0].Text);
        Assert.Empty(layout.OverflowLines);
        Assert.Equal(1, layout.LineCount);
    }

    [Fact]
    public void PlanSay_ExactBoundaryDoesNotWrap()
    {
        UseFixedWidth(1);
        var exactly150 = new string('a', 150);

        var layout = ActorSay.PlanSay(exactly150, TActorCore.TextWidthFn);

        Assert.Single(layout.Lines);
        Assert.Equal(150, layout.Lines[0].Text.Length);
    }

    [Fact]
    public void PlanSay_OneOverBoundaryWraps()
    {
        UseFixedWidth(1);                                  // 151 字符 → 超限

        var layout = ActorSay.PlanSay(new string('a', 151), TActorCore.TextWidthFn);

        // 第 151 个字符使宽度 151 > 150 → 落行（含该字符），随后 Copy(Str, I+1, ...) 为空
        Assert.Equal(151, layout.Lines[0].Text.Length);
    }

    [Fact]
    public void PlanSay_LongTextSplitsIntoMultipleLines()
    {
        UseFixedWidth(10);                                 // 每 16 字符超 150（160 > 150）

        var layout = ActorSay.PlanSay(new string('a', 32), TActorCore.TextWidthFn);

        Assert.Equal(2, layout.Lines.Count);
        Assert.Equal(0, layout.Lines[0].Index);
        Assert.Equal(1, layout.Lines[1].Index);
        Assert.Equal(16, layout.Lines[0].Text.Length);
        Assert.Equal(16, layout.Lines[1].Text.Length);
    }

    [Fact]
    public void PlanSay_ResultIndexesAreSequential()
    {
        UseFixedWidth(10);

        var layout = ActorSay.PlanSay(new string('a', 48), TActorCore.TextWidthFn);

        Assert.Equal(3, layout.Lines.Count);
        for (int i = 0; i < layout.Lines.Count; i++)
            Assert.Equal(i, layout.Lines[i].Index);
    }

    // ===================== 换行：GBK 双字节 =====================

    [Fact]
    public void PlanSay_TwoByteCharsAreConsumedInPairs()
    {
        // 度量按“字符数×10”，每个汉字占 2 个 char（模拟 GBK 双字节）
        UseFixedWidth(10);

        var text = new string('汉', 16);                   // 32 char → 宽 320
        var layout = ActorSay.PlanSay(text, TActorCore.TextWidthFn);

        // 每个汉字贡献 2 字符 → 宽 20；第 16 个汉字时宽 160 > 150 → 落行含 16 个汉字
        Assert.Equal(16, layout.Lines[0].Text.Length);
    }

    [Fact]
    public void PlanSay_OddTrailingTwoByteCharDoesNotThrow()
    {
        UseFixedWidth(1);
        // 构造以孤立的 >=128 字符结尾：i++ 后 i > len → loop=false 且 break
        var text = "abc" + '\u00E9';                       // é 为 0xE9 >= 128

        var layout = ActorSay.PlanSay(text, TActorCore.TextWidthFn);

        // 不抛异常即可；末尾字符已被 temp 吸收
        Assert.NotEmpty(layout.Lines);
    }

    // ===================== MAXSAY 截断与原文越界缺陷 =====================

    [Fact]
    public void PlanSay_StopsAtMaxSayButEmitsOverflowLineLikeOriginal()
    {
        UseFixedWidth(10);                                 // 每 16 字符一行

        var layout = ActorSay.PlanSay(new string('a', 200), TActorCore.TextWidthFn);

        // 原文 9911-9914 在 n 达到 5 时 Break 且未清零 temp/未截断 Str，
        // 控制流落到 9921 再落一行 → 共 6 行，其中第 6 行 Index=5 越界
        Assert.Equal(5, layout.Lines.Count);
        Assert.Single(layout.OverflowLines);
        Assert.Equal(ActorSay.MaxSay, layout.OverflowLines[0].Index);
        Assert.Equal(6, layout.LineCount);                 // m_nSayLineCount 亦为 6
    }

    [Fact]
    public void PlanSay_ExactlyMaxSayLinesAlsoEmitsOverflow()
    {
        UseFixedWidth(10);

        var layout = ActorSay.PlanSay(new string('a', 80), TActorCore.TextWidthFn);

        Assert.Equal(5, layout.Lines.Count);
        Assert.Equal(4, layout.Lines[^1].Index);
        Assert.Single(layout.OverflowLines);
    }

    [Fact]
    public void PlanSay_FourLinesProduceNoOverflow()
    {
        UseFixedWidth(10);

        var layout = ActorSay.PlanSay(new string('a', 64), TActorCore.TextWidthFn);

        Assert.Equal(4, layout.Lines.Count);
        Assert.Empty(layout.OverflowLines);
        Assert.Equal(4, layout.LineCount);
    }

    [Fact]
    public void PlanSay_OverflowLineHoldsSameTextAsFifthLineGate()
    {
        UseFixedWidth(10);

        var layout = ActorSay.PlanSay(new string('a', 200), TActorCore.TextWidthFn);

        // 越界行内容是触发 n>=MAXSAY 的那个 temp（尚未清零）
        Assert.Equal(16, layout.OverflowLines[0].Text.Length);
        Assert.Equal(layout.Lines[4].Text, layout.OverflowLines[0].Text);
    }

    // ===================== 清空语义 =====================

    [Fact]
    public void PlanSay_EmptyStringProducesNoLines()
    {
        UseFixedWidth(1);

        var layout = ActorSay.PlanSay("", TActorCore.TextWidthFn);

        Assert.Empty(layout.Lines);
        Assert.Equal(0, layout.LineCount);
    }

    [Fact]
    public void PlanSay_MarkupOnlyTextProducesNoLines()
    {
        UseFixedWidth(1);

        var layout = ActorSay.PlanSay("{abc|1:2}", TActorCore.TextWidthFn);

        Assert.Empty(layout.Lines);
    }

    // ===================== TActorCore.Say 落地 =====================

    [Fact]
    public void Say_StampsTimeAndResetsLineCount()
    {
        UseFixedWidth(1);
        SceneTime.TickNow = () => 4242;

        var a = new TActor();
        var lines = a.Say("你好");

        Assert.Equal(4242u, a.m_dwSayTime);
        Assert.Equal(lines.Count, a.m_nSayLineCount);
    }

    [Fact]
    public void Say_ClearsAllExistingSlotsBeforeFilling()
    {
        UseFixedWidth(1);

        var a = new TActor();
        a.EnsureSayingSlots();

        // 预置脏数据
        for (int i = 0; i < ActorSay.MaxSay; i++)
        {
            a.SayingText[i] = "脏" + i;
            a.SayingArr[i].Width = 99;
            a.SayingArr[i].Height = 88;
            a.SayingArr[i].HasImage = true;
        }

        a.Say("新");

        Assert.Equal("新", a.SayingText[0]);
        Assert.Equal(0, a.SayingArr[0].Width);
        Assert.Equal(0, a.SayingArr[0].Height);
        Assert.False(a.SayingArr[0].HasImage);

        // 其余槽位被清空
        for (int i = 1; i < ActorSay.MaxSay; i++)
        {
            Assert.Equal("", a.SayingText[i]);
            Assert.False(a.SayingArr[i].HasImage);
        }
    }

    [Fact]
    public void Say_WritesMultipleLinesIntoSequentialSlots()
    {
        UseFixedWidth(10);

        var a = new TActor();
        a.Say(new string('a', 32));

        Assert.Equal(2, a.m_nSayLineCount);
        Assert.Equal(16, a.SayingText[0].Length);
        Assert.Equal(16, a.SayingText[1].Length);
        Assert.Equal("", a.SayingText[2]);
    }

    [Fact]
    public void Say_OverflowLineIsNotWrittenIntoSlotsButCounted()
    {
        UseFixedWidth(10);

        var a = new TActor();
        a.Say(new string('a', 200));

        // 原文越界写 m_SayingArr[5]；C# 侧仅记入审计位，5 个槽位写满
        Assert.Equal(ActorSay.MaxSay, a.SayingArr.Count);
        Assert.All(a.SayingText, t => Assert.NotEqual("", t));
        Assert.Single(a.SayOverflowLines);
        Assert.Equal(6, a.m_nSayLineCount);                // 计数含越界行（原文语义）
    }

    [Fact]
    public void Say_NormalTextHasNoOverflow()
    {
        UseFixedWidth(1);

        var a = new TActor();
        a.Say("短句");

        Assert.Empty(a.SayOverflowLines);
        Assert.Equal(1, a.m_nSayLineCount);
    }

    [Fact]
    public void Say_MarkupStrippedBeforeWrapping()
    {
        UseFixedWidth(1);

        var a = new TActor();
        a.Say("{红色|249:0}你好");

        Assert.Equal("你好", a.SayingText[0]);
    }

    [Fact]
    public void Say_IsRepeatableAndDoesNotAccumulate()
    {
        UseFixedWidth(1);

        var a = new TActor();
        a.Say("第一次");
        a.Say("第二次");

        Assert.Equal(1, a.m_nSayLineCount);
        Assert.Equal("第二次", a.SayingText[0]);
    }

    [Fact]
    public void Say_DefaultTextWidthFnScalesWithLength()
    {
        // 默认接缝为 s.Length * 6（无需注入即可工作）
        var a = new TActor();
        a.Say("abc");

        Assert.Equal(1, a.m_nSayLineCount);
        Assert.Equal("abc", a.SayingText[0]);
    }

    [Fact]
    public void EnsureSayingSlots_IsIdempotent()
    {
        var a = new TActor();
        a.EnsureSayingSlots();
        a.EnsureSayingSlots();

        Assert.Equal(ActorSay.MaxSay, a.SayingArr.Count);
        Assert.Equal(ActorSay.MaxSay, a.SayingText.Count);
    }
}
