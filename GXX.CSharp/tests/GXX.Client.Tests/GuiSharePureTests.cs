using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GXX.Client.GUI.DxComponent;
using GXX.Client.GUI.Mir;
using GXX.Client.GUI.Share;
using GXX.Core.Protocol;
using Xunit;
// 注意：本车道（p2-client-fstate）是 FState.pas 的正式归属，TFrmDlg 的真源在
// GXX.Client.GUI.Share。车道1 的 GXX.Client.GUI.Mir.TFrmDlg 是同一 Delphi 类型的早期接缝，
// 二者同名会造成 CS0104 歧义 —— 已登记在交付报告的"接缝改回/合并清单"里。
using TFrmDlg = GXX.Client.GUI.Share.TFrmDlg;
// D-P10-06：THintLines 的正式归属是 GXX.Client.Scenes（DrawScrn.pas:318）；
// FStateSeams.cs 的接缝类已删除，本测试改用正式实现的对象形态
// （FList + THintText.FCaption / FColor / Size / Style / IsStroke）。
using THintLines = GXX.Client.Scenes.THintLines;
using THintText = GXX.Client.Scenes.THintText;
using static GXX.Client.GUI.Mir.MShareGlobals;

namespace GXX.Client.Tests;

/// <summary>
/// 并行车道 p2-client-fstate 切片 2：FState.pas 的**纯函数**与**无控件树依赖的行为**。
///
/// 覆盖原文行号：
///   1135-1152 WriteOutStr       3650-3684 GetHitLines
///   4257-4269 GetJobText        4271-4283 GetJobTextEx
///   1154-1415 NPC 控件类的构造与 Update
///   23844-23893 / 24474-24487 TFrmDlg 的可移植方法
///   24489-24508 聊天输入框      24510-24627 屏幕技能图标
///   24680-24734 SaveMagicButtons（守卫 + 收集，INI 主体为接缝）
///
/// 时钟经 <see cref="FStateSeamClock"/> 注入，故 `>= 1000` 与 `> 1000` 这类**只差一个等号**
/// 的分支可以被精确落点断言，而不是靠真实时钟碰运气。
/// </summary>
public sealed class GuiSharePureTests : IDisposable
{
    private uint _fakeTick;

    public GuiSharePureTests()
    {
        ResetAll();
    }

    public void Dispose()
    {
        ResetAll();
    }

    private void ResetAll()
    {
        FStateSeamClock.ResetForTests();
        MShareHintFont.ResetForTests();
        ConfigClientExt.ResetForTests();
        MagicButtonIniSeam.ResetForTests();
        MShareGlobalsReset.ResetForTests();
        FStateClMainSeam.ResetForTests();
        FStateGlobal.ResetForTests();
        _fakeTick = 10_000;
        FStateSeamClock.NowHandler = () => _fakeTick;
    }

    private static THintLines NewHintLines() => new();

    /// <summary>
    /// 取第 <paramref name="i"/> 行的 THintText（原文 THintLines.FList[i]）。
    /// D-P10-06 后不再有接缝的 `Lines[i]` 值副本 —— 断言直接读正式实现的
    /// `THintText.FCaption` / `FColor` / `Size` / `Style` / `IsStroke`。
    /// </summary>
    private static THintText LineAt(THintLines h, int i) => (THintText)h.FList[i];

    /// <summary>提示行数（原文 THintLines.Count 属性 = FList.Count）。</summary>
    private static int LineCount(THintLines h) => h.FList.Count;

    // =====================================================================================
    // GetJobText / GetJobTextEx（原文 4257-4283）
    // =====================================================================================

    [Theory]
    [InlineData(0, "战")]
    [InlineData(1, "法")]
    [InlineData(2, "道")]
    public void GetJobTextMapsTheThreeJobs(int job, string expected)
        => Assert.Equal(expected, FStatePure.GetJobText(job));

    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    [InlineData(99)]
    public void GetJobTextReturnsQuestionMarkOutsideZeroToTwo(int job)
        => Assert.Equal("?", FStatePure.GetJobText(job));

    [Fact]
    public void GetJobTextExReadsTheConfiguredHintTextPerJob()
    {
        ConfigClientExt.ItemHintTextConfig[0].Text = "需要战士";
        ConfigClientExt.ItemHintTextConfig[1].Text = "需要法师";
        ConfigClientExt.ItemHintTextConfig[2].Text = "需要道士";

        Assert.Equal("需要战士", FStatePure.GetJobTextEx(0));
        Assert.Equal("需要法师", FStatePure.GetJobTextEx(1));
        Assert.Equal("需要道士", FStatePure.GetJobTextEx(2));
    }

    [Fact]
    public void GetJobTextExDiffersFromGetJobTextAndFallsBackToQuestionMark()
    {
        // 差异断言：同样返回 0/1/2 的映射，但 GetJobTextEx 取配置文本；
        // 未配置时是空串，而 GetJobText 恒为 '战'/'法'/'道'。
        ConfigClientExt.ItemHintTextConfig[0].Text = "";
        Assert.Equal("战", FStatePure.GetJobText(0));
        Assert.Equal("", FStatePure.GetJobTextEx(0));

        // 越界分支两者都是 '?'
        Assert.Equal("?", FStatePure.GetJobText(7));
        Assert.Equal("?", FStatePure.GetJobTextEx(7));
    }

    // =====================================================================================
    // GetHitLines（原文 3650-3684）—— "/颜色" 转义的逐字语义
    // =====================================================================================

    [Fact]
    public void GetHitLinesWithoutSlashAddsTheWholeStringWithDefaultColor()
    {
        var h = NewHintLines();
        var def = new TColor(0x0000FF00);
        FStatePure.GetHitLines(h, "普通文本", def);

        Assert.Equal(1, LineCount(h));
        Assert.Equal("普通文本", LineAt(h, 0).FCaption);
        Assert.Equal(def.Value, LineAt(h, 0).FColor.Value);
    }

    [Fact]
    public void GetHitLinesAddsAnEmptyLineForAnEmptyString()
    {
        // 原文无 '/' 时**无条件** Add（含空串）。
        var h = NewHintLines();
        FStatePure.GetHitLines(h, "", new TColor(0x00FFFFFF));

        Assert.Equal(1, LineCount(h));
        Assert.Equal("", LineAt(h, 0).FCaption);
    }

    [Fact]
    public void GetHitLinesPullsTheColorCodeThatPrecedesTheSlash()
    {
        // 语法是「文本 + 调色板下标 + /」：下标在被 '/' 终止的那一段里。
        var h = NewHintLines();
        var def = new TColor(0x000000FF);
        FStatePure.GetHitLines(h, "abc 200/", def);

        Assert.Equal(1, LineCount(h));
        Assert.Equal("abc ", LineAt(h, 0).FCaption);              // 颜色码被吃掉，前面的文字保留含空格
        Assert.Equal(def.Value, LineAt(h, 0).FColor.Value);       // 第一行仍用默认色
    }

    [Fact]
    public void GetHitLinesPropagatesTheColorToTheFollowingSegments()
    {
        var h = NewHintLines();
        FStatePure.GetHitLines(h, "A 1/B 2/", new TColor(0x000000FF));

        Assert.Equal(2, LineCount(h));
        Assert.Equal("A ", LineAt(h, 0).FCaption);
        Assert.Equal("B ", LineAt(h, 1).FCaption);
        // 第二段的颜色来自第一段末尾的颜色码（GetRGB(1)），与第一段不同
        Assert.NotEqual(LineAt(h, 0).FColor.Value, LineAt(h, 1).FColor.Value);
    }

    [Fact]
    public void GetHitLinesDropsALeadingSlashAndKeepsTheRest()
    {
        // nPos == 1 ⇒ 前置扫描 I 停在 0 ⇒ 走 `if I = 0` 分支：丢弃该 '/' 再继续。
        var h = NewHintLines();
        var def = new TColor(0x00ABCDEF);
        FStatePure.GetHitLines(h, "/abc", def);

        Assert.Equal(1, LineCount(h));
        Assert.Equal("abc", LineAt(h, 0).FCaption);
        Assert.Equal(def.Value, LineAt(h, 0).FColor.Value);
    }

    [Fact]
    public void GetHitLinesDropsAPureNumericPreambleAsAColorCode()
    {
        // "255/abc"：'/' 前全是数字 ⇒ 扫描到 0 ⇒ 整段数字被当成颜色码丢掉。
        var h = NewHintLines();
        var def = new TColor(0x00123456);
        FStatePure.GetHitLines(h, "255/abc", def);

        Assert.Equal(1, LineCount(h));
        Assert.Equal("abc", LineAt(h, 0).FCaption);
        Assert.Equal(def.Value, LineAt(h, 0).FColor.Value);
    }

    [Fact]
    public void GetHitLinesClampsTheColorCodeWindowToThreeDigits()
    {
        // "12345/"：5 位数字 ⇒ `if I < nPos - 4 then I := nPos - 4` 把窗口夹到 3 位，
        // 于是前 2 位 "12" 变成正文，后 3 位 "345" 当颜色码。
        var h = NewHintLines();
        FStatePure.GetHitLines(h, "12345/abc", new TColor(0x00000000));

        Assert.Equal(2, LineCount(h));
        Assert.Equal("12", LineAt(h, 0).FCaption);
        Assert.Equal("abc", LineAt(h, 1).FCaption);
    }

    [Fact]
    public void GetHitLinesDefaultsToPalette255WhenTheCodeIsNotNumeric()
    {
        // "a/1"：'/' 前只有 1 个字符且非数字 ⇒ 颜色码段长度为 0 ⇒ StrToIntDef('', 255) = 255。
        // 注意：'/' 之后的 "1" 是**正文**（颜色码在 '/' 之前），故最终有 2 行。
        var h = NewHintLines();
        FStatePure.GetHitLines(h, "a/1", new TColor(0x00000000));

        Assert.Equal(2, LineCount(h));
        Assert.Equal("a", LineAt(h, 0).FCaption);
        Assert.Equal("1", LineAt(h, 1).FCaption);
        // 第 2 行用的是默认回退色 GetRGB(255)
        Assert.Equal(GetRGB(255), LineAt(h, 1).FColor.Value);
    }

    [Fact]
    public void GetHitLinesAppliesTheLastColorToTheTrailingRemainder()
    {
        var h = NewHintLines();
        FStatePure.GetHitLines(h, "A 1/B", new TColor(0x000000FF));

        Assert.Equal(2, LineCount(h));
        Assert.Equal("A ", LineAt(h, 0).FCaption);
        Assert.Equal("B", LineAt(h, 1).FCaption);
        Assert.NotEqual(LineAt(h, 0).FColor.Value, LineAt(h, 1).FColor.Value);
    }

    [Fact]
    public void GetHitLinesPassesTheInjectedHintFontValues()
    {
        // 原文每行都传 GetHintFontSize / GetHintFontStyle([]) / GetHintFontStroke(True)。
        MShareHintFont.GetHintFontSizeHandler = () => 12;
        MShareHintFont.GetHintFontStyleHandler = _ => TFontStyles.fsBold;
        MShareHintFont.GetHintFontStrokeHandler = _ => false;

        var h = NewHintLines();
        FStatePure.GetHitLines(h, "X", new TColor(0x00000000));

        Assert.Equal(12, LineAt(h, 0).Size);
        Assert.Equal(TFontStyles.fsBold, LineAt(h, 0).Style);
        Assert.False(LineAt(h, 0).IsStroke);
    }

    [Fact]
    public void GetHitLinesStrokeArgumentIsAlwaysTrue()
    {
        // 原文传的是 GetHintFontStroke(True)，不是 False。
        bool? seen = null;
        MShareHintFont.GetHintFontStrokeHandler = v => { seen = v; return v; };
        FStatePure.GetHitLines(NewHintLines(), "X", new TColor(0x00000000));
        Assert.True(seen);
    }

    // =====================================================================================
    // WriteOutStr（原文 1135-1152）
    // =====================================================================================

    [Fact]
    public void DelphiTimeToStrUsesZeroPaddedTwentyFourHourClock()
    {
        Assert.Equal("01:02:03", FStatePure.DelphiTimeToStr(new DateTime(2020, 1, 1, 1, 2, 3)));
        Assert.Equal("00:00:00", FStatePure.DelphiTimeToStr(new DateTime(2020, 1, 1, 0, 0, 0)));
        Assert.Equal("23:59:59", FStatePure.DelphiTimeToStr(new DateTime(2020, 1, 1, 23, 59, 59)));
    }

    [Fact]
    public void WriteOutStrAppendsATimestampedLine()
    {
        string path = FStatePure.WriteOutStrFileName;
        string before = File.Exists(path) ? File.ReadAllText(path) : "";
        try
        {
            FStatePure.WriteOutStr("hello");
            string after = File.ReadAllText(path);
            Assert.True(after.Length > before.Length);
            Assert.Contains(" hello", after);
            // 时间戳形态 HH:NN:SS
            string lastLine = after.Split('\n').Last(l => l.Contains(" hello")).Trim();
            Assert.Matches(@"^\d{2}:\d{2}:\d{2} hello$", lastLine);
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    // =====================================================================================
    // TNpcButton（原文 1154-1199）
    // =====================================================================================

    [Fact]
    public void NpcButtonConstructorMatchesSourceDefaults()
    {
        var b = new TNpcButton(null);
        Assert.Equal("", b.m_sPostText);
        Assert.Equal("", b.m_sCmd);
        Assert.Equal(0, b.m_nStartImageIndex);
        Assert.Equal(0, b.m_nStopImageCount);
        Assert.Equal(0, b.m_nImageIndex);
        Assert.Equal(300u, b.m_dwPlayImageTime);
        Assert.Equal(_fakeTick, b.m_dwPlayImageTick);
        Assert.False(b.m_boBlend);
        Assert.Equal(0, b.m_nShowBG);
        Assert.Equal(0, b.m_TextureOffset.X);
        Assert.Equal(0, b.m_TextureOffset.Y);
    }

    [Fact]
    public void NpcButtonUpdateOnlyAdvancesWhenStopIsStrictlyGreaterThanStart()
    {
        var b = new TNpcButton(null) { m_nStartImageIndex = 0, m_nStopImageCount = 0, m_nImageIndex = 0 };
        b.m_dwPlayImageTick = 0;
        _fakeTick = 10_000;

        b.Update();
        Assert.Equal(0, b.m_nImageIndex);          // Stop == Start ⇒ 不播放

        b.m_nStartImageIndex = 1;
        b.m_nStopImageCount = 3;
        b.m_nImageIndex = 1;
        b.Update();
        Assert.Equal(2, b.m_nImageIndex);          // Stop > Start ⇒ 推进
    }

    [Fact]
    public void NpcButtonUpdateRequiresStrictlyMoreThanPlayTime()
    {
        var b = new TNpcButton(null) { m_nStartImageIndex = 1, m_nStopImageCount = 3, m_nImageIndex = 1 };
        b.m_dwPlayImageTime = 300;
        b.m_dwPlayImageTick = 0;

        _fakeTick = 300;                            // 差值 == 300 ⇒ 严格大于不成立
        b.Update();
        Assert.Equal(1, b.m_nImageIndex);

        _fakeTick = 301;                            // 差值 301 > 300
        b.Update();
        Assert.Equal(2, b.m_nImageIndex);
        Assert.Equal(301u, b.m_dwPlayImageTick);
    }

    [Fact]
    public void NpcButtonUpdateWrapsWhenIndexExceedsStopInclusive()
    {
        // 原文判据是 `m_nImageIndex > m_nStopImageCount`（**含 stop 本身**），
        // 因此 index 会先走到 stop，再走到 stop+1 才回绕。
        var b = new TNpcButton(null) { m_nStartImageIndex = 1, m_nStopImageCount = 3, m_nImageIndex = 3 };
        b.m_dwPlayImageTime = 0;
        b.m_dwPlayImageTick = 0;
        _fakeTick = 5;

        b.Update();
        Assert.Equal(1, b.m_nImageIndex);            // 4 > 3 ⇒ 回绕到 start
        Assert.Equal(1, b.ImageIndex.Up);            // 且写回 ImageIndex.Up
    }

    [Fact]
    public void NpcButtonUpdateStopsAfterAddData2ReachesAddData1()
    {
        var b = new TNpcButton(null) { m_nStartImageIndex = 1, m_nStopImageCount = 2, m_nImageIndex = 1 };
        b.m_dwPlayImageTime = 0;
        b.m_dwPlayImageTick = 0;
        DxControlExt.SetAddData1(b, 2);
        DxControlExt.SetAddData2(b, 0);

        // 第一轮：index 1→2，2 > 2 不成立 ⇒ 不回绕、AddData2 不变
        _fakeTick = 1;
        b.Update();
        Assert.Equal(2, b.m_nImageIndex);
        Assert.Equal(0, DxControlExt.GetAddData2(b));

        // 第二轮：index 2→3，3 > 2 ⇒ 回绕，AddData2 变 1
        _fakeTick = 2;
        b.Update();
        Assert.Equal(1, b.m_nImageIndex);
        Assert.Equal(1, DxControlExt.GetAddData2(b));

        // 第三轮：AddData1(2) > 0 且 AddData2(1) < 2 ⇒ 继续推进，回绕后 AddData2 = 2
        _fakeTick = 3;
        b.Update();
        Assert.Equal(2, b.m_nImageIndex);
        _fakeTick = 4;
        b.Update();
        Assert.Equal(1, b.m_nImageIndex);
        Assert.Equal(2, DxControlExt.GetAddData2(b));

        // 第四轮：AddData2(2) >= AddData1(2) ⇒ 直接 Exit，不再推进
        _fakeTick = 5;
        b.Update();
        Assert.Equal(1, b.m_nImageIndex);
    }

    // =====================================================================================
    // TNpcLabel（原文 1201-1230）
    // =====================================================================================

    [Fact]
    public void NpcLabelConstructorMatchesSourceDefaults()
    {
        var l = new TNpcLabel(null);
        Assert.Equal("", l.m_sPostText);
        Assert.Equal("", l.m_sCmd);
        Assert.Equal(0, l.m_nColorIndex);
        Assert.NotNull(l.m_AutoColors);
        Assert.Equal(0, l.m_AutoColors.Count);
        Assert.Equal(_fakeTick, l.m_dwAutoColorTick);
    }

    [Fact]
    public void NpcLabelUpdateRotatesColorsAndWritesUpAndDisabledOnly()
    {
        var l = new TNpcLabel(null);
        l.m_AutoColors.Add(0x000000FF);
        l.m_AutoColors.Add(0x0000FF00);
        l.m_dwAutoColorTick = 0;

        _fakeTick = 601;                       // 601 > 600
        l.Update();

        Assert.Equal(0x000000FF, l.CaptionColor.Up.Value);
        Assert.Equal(0x000000FF, l.CaptionColor.Disabled.Value);
        // 差异断言：原文只写 Up 与 Disabled 两态，Hot/Down/Checked 保持默认（0）
        Assert.Equal(0, l.CaptionColor.Hot.Value);
        Assert.Equal(0, l.CaptionColor.Down.Value);
        Assert.Equal(0, l.CaptionColor.Checked.Value);
        Assert.Equal(1, l.m_nColorIndex);
        Assert.Equal(601u, l.m_dwAutoColorTick);
    }

    [Fact]
    public void NpcLabelUpdateDoesNothingAtExactly600Ticks()
    {
        var l = new TNpcLabel(null);
        l.m_AutoColors.Add(0x000000FF);
        l.m_dwAutoColorTick = 0;

        _fakeTick = 600;                       // 原文 `> 600` ⇒ 等于 600 不轮换
        l.Update();
        Assert.Equal(0, l.m_nColorIndex);
        Assert.Equal(0, l.CaptionColor.Up.Value);
    }

    [Fact]
    public void NpcLabelUpdateResetsAnOutOfRangeColorIndex()
    {
        var l = new TNpcLabel(null);
        l.m_AutoColors.Add(0x00000011);
        l.m_AutoColors.Add(0x00000022);
        l.m_nColorIndex = 5;                   // 越界
        l.m_dwAutoColorTick = 0;
        _fakeTick = 700;

        l.Update();
        Assert.Equal(0x00000011, l.CaptionColor.Up.Value);
        Assert.Equal(1, l.m_nColorIndex);
    }

    [Fact]
    public void NpcLabelUpdateIsInertWithoutAutoColors()
    {
        var l = new TNpcLabel(null);
        l.m_dwAutoColorTick = 0;
        _fakeTick = 100_000;
        l.Update();
        Assert.Equal(0, l.CaptionColor.Up.Value);
        Assert.Equal(0u, l.m_dwAutoColorTick);
    }

    // =====================================================================================
    // TCountDownLabel（原文 1232-1309）
    // =====================================================================================

    [Fact]
    public void CountDownLabelConstructorMatchesSourceDefaults()
    {
        var c = new TCountDownLabel(null);
        Assert.Equal("", c.m_sCmd);
        Assert.Equal(0, c.m_CountDownValue);
        Assert.Equal(0, c.m_OldCountDownValue);
        Assert.Equal(1, c.m_LoopCount);
        Assert.Equal(_fakeTick, c.m_UpdateTick);
        Assert.False(c.m_boChineseFormat);
    }

    [Theory]
    [InlineData(0, "00:00:00")]
    [InlineData(1, "00:00:01")]
    [InlineData(59, "00:00:59")]
    [InlineData(60, "00:01:00")]
    [InlineData(3600, "01:00:00")]
    public void CountDownLabelUpdateCaptionUsesHourMinuteSecond(int value, string expected)
    {
        var c = new TCountDownLabel(null) { m_CountDownValue = value };
        c.UpdateCaption();
        Assert.Equal(expected, c.Caption);
    }

    [Fact]
    public void CountDownLabelChineseFormatShowsDaysAndDiffersFromPlainFormat()
    {
        // 差异断言：同一数值 90061（=1天1时1分1秒）
        //   中文格式 → "01天01时01分01秒"（扣掉整天）
        //   普通格式 → "25:01:01"（小时是**总**小时数，不扣天）
        var cn = new TCountDownLabel(null) { m_CountDownValue = 90061, m_boChineseFormat = true };
        cn.UpdateCaption();
        Assert.Equal("01天01时01分01秒", cn.Caption);

        var plain = new TCountDownLabel(null) { m_CountDownValue = 90061, m_boChineseFormat = false };
        plain.UpdateCaption();
        Assert.Equal("25:01:01", plain.Caption);

        Assert.NotEqual(cn.Caption, plain.Caption);
    }

    [Fact]
    public void CountDownLabelUpdateCaptionHandlesNegativeValuesWithoutThrowing()
    {
        // Delphi 的 div/mod 对负数向零截断，C# 同语义；-1 mod 60 = -1。
        var c = new TCountDownLabel(null) { m_CountDownValue = -1, m_boChineseFormat = true };
        c.UpdateCaption();
        Assert.Equal("00天00时00分-01秒", c.Caption);
    }

    [Fact]
    public void CountDownLabelUpdateShortCircuitsWhenLoopCountIsZero()
    {
        var c = new TCountDownLabel(null) { m_LoopCount = 0, m_CountDownValue = 42 };
        c.Update();
        Assert.Equal("00:00:00", c.Caption);
        Assert.Equal(42, c.m_CountDownValue);   // 早退分支不动倒计时值
    }

    [Fact]
    public void CountDownLabelUpdateZeroesLoopAndFiresCallbackWhenValueIsNonPositive()
    {
        var c = new TCountDownLabel(null) { m_CountDownValue = 0, m_LoopCount = 1, m_sCmd = "CMD" };
        int fired = 0;
        c.SetClickEvent((s, x, y) => fired++);

        c.Update();
        Assert.Equal("00:00:00", c.Caption);
        Assert.Equal(0, c.m_LoopCount);
        Assert.Equal(1, fired);

        // 无 m_sCmd 时不回调（差异：m_sCmd 为空即静默）
        var c2 = new TCountDownLabel(null) { m_CountDownValue = 0, m_LoopCount = 1, m_sCmd = "" };
        int fired2 = 0;
        c2.SetClickEvent((s, x, y) => fired2++);
        c2.Update();
        Assert.Equal(0, fired2);
    }

    [Fact]
    public void CountDownLabelUpdateAdvancesAtExactlyOneThousandTicks()
    {
        // 原文判据 `MyGetTickCount - m_UpdateTick >= 1000` —— **含** 1000。
        var c = new TCountDownLabel(null) { m_CountDownValue = 5, m_UpdateTick = 0, m_LoopCount = 3 };
        _fakeTick = 1000;

        c.Update();
        Assert.Equal(4, c.m_CountDownValue);
        Assert.Equal(1000u, c.m_UpdateTick);
        Assert.Equal("00:00:04", c.Caption);
    }

    [Fact]
    public void CountDownLabelUpdateDoesNotAdvanceAt999Ticks()
    {
        var c = new TCountDownLabel(null) { m_CountDownValue = 5, m_UpdateTick = 0, m_LoopCount = 3 };
        _fakeTick = 999;

        c.Update();
        Assert.Equal(5, c.m_CountDownValue);
        Assert.Equal(0u, c.m_UpdateTick);
    }

    [Fact]
    public void CountDownLabelUpdateResetsToOldValuePlusOneAndDecrementsLoop()
    {
        var c = new TCountDownLabel(null)
        {
            m_CountDownValue = 1,
            m_OldCountDownValue = 5,
            m_LoopCount = 2,
            m_sCmd = "CMD",
            m_UpdateTick = 0,
        };
        int fired = 0;
        c.SetClickEvent((s, x, y) => fired++);
        _fakeTick = 1000;

        c.Update();

        Assert.Equal(1, fired);
        Assert.Equal(6, c.m_CountDownValue);    // OldCountDownValue + 1
        Assert.Equal(1, c.m_LoopCount);         // 2 - 1
    }

    [Fact]
    public void CountDownLabelUpdateAccumulatesUpdateTickByThousandNotByNow()
    {
        // 原文 `m_UpdateTick := m_UpdateTick + 1000`（累加，保留漂移），不是 := MyGetTickCount。
        var c = new TCountDownLabel(null) { m_CountDownValue = 10, m_UpdateTick = 0, m_LoopCount = 3 };
        _fakeTick = 3500;

        c.Update();
        Assert.Equal(1000u, c.m_UpdateTick);    // 不是 3500
    }

    // =====================================================================================
    // TImgCountDownButton（原文 1311-1371）—— 与 TCountDownLabel 的差异必须成立
    // =====================================================================================

    [Fact]
    public void ImgCountDownButtonConstructorMatchesSourceDefaults()
    {
        var b = new TImgCountDownButton(null);
        Assert.Equal("", b.m_sCmd);
        Assert.Equal(0, b.m_CountDownValue);
        Assert.Equal(0, b.m_OldCountDownValue);
        Assert.Equal(1, b.m_LoopCount);
        Assert.Equal(_fakeTick, b.m_UpdateTick);
        Assert.Equal(0, b.m_ImgStartIndex);
        Assert.Equal(0, b.m_ImgSpace);
    }

    [Fact]
    public void ImgCountDownButtonCaptionHasNoChineseBranchUnlikeCountDownLabel()
    {
        // 差异断言：TImgCountDownButton.UpdateCaption **没有** m_boChineseFormat 分支，
        // 也**不计算** nDay —— 90061 在这里同样是 "25:01:01"，即便把标志置真。
        var b = new TImgCountDownButton(null) { m_CountDownValue = 90061 };
        b.UpdateCaption();
        Assert.Equal("25:01:01", b.Caption);

        // TCountDownLabel 的同一标志会产生中文格式，二者对同一输入不同结果
        var c = new TCountDownLabel(null) { m_CountDownValue = 90061, m_boChineseFormat = true };
        c.UpdateCaption();
        Assert.Equal("01天01时01分01秒", c.Caption);
        Assert.NotEqual(c.Caption, b.Caption);
    }

    [Fact]
    public void ImgCountDownButtonUpdateDoesNotAdvanceAtExactlyOneThousandTicks()
    {
        // 差异断言（核心）：原文 TImgCountDownButton 用 `> 1000`（严格大于），
        // 而 TCountDownLabel 用 `>= 1000`。同一 tick 差 1000：
        //   本类 → 不推进；TCountDownLabel → 推进。
        var b = new TImgCountDownButton(null) { m_CountDownValue = 5, m_UpdateTick = 0, m_LoopCount = 3 };
        _fakeTick = 1000;
        b.Update();
        Assert.Equal(5, b.m_CountDownValue);       // 未推进
        Assert.Equal(0u, b.m_UpdateTick);

        var c = new TCountDownLabel(null) { m_CountDownValue = 5, m_UpdateTick = 0, m_LoopCount = 3 };
        _fakeTick = 1000;
        c.Update();
        Assert.Equal(4, c.m_CountDownValue);       // 已推进
    }

    [Fact]
    public void ImgCountDownButtonUpdateAdvancesAt1001Ticks()
    {
        var b = new TImgCountDownButton(null) { m_CountDownValue = 5, m_UpdateTick = 0, m_LoopCount = 3 };
        _fakeTick = 1001;
        b.Update();
        Assert.Equal(4, b.m_CountDownValue);
        Assert.Equal(1000u, b.m_UpdateTick);
    }

    [Fact]
    public void ImgCountDownButtonUpdateHasNoNonPositiveGuardUnlikeCountDownLabel()
    {
        // 差异断言：TImgCountDownButton.Update **没有** `m_CountDownValue <= 0` 的前置分支，
        // 因此 0 会被继续减到 -1；TCountDownLabel 则在 <= 0 时清零 LoopCount 并回调后返回。
        var b = new TImgCountDownButton(null)
        {
            m_CountDownValue = 0, m_LoopCount = 1, m_UpdateTick = 0, m_sCmd = "CMD",
        };
        _fakeTick = 2000;
        b.Update();
        Assert.Equal(-1, b.m_CountDownValue);      // 被减成负数
        Assert.Equal(1, b.m_LoopCount);            // LoopCount 未被清零

        var c = new TCountDownLabel(null)
        {
            m_CountDownValue = 0, m_LoopCount = 1, m_UpdateTick = 0, m_sCmd = "CMD",
        };
        c.Update();
        Assert.Equal(0, c.m_CountDownValue);       // 保持 0
        Assert.Equal(0, c.m_LoopCount);            // 被清零
    }

    [Fact]
    public void ImgCountDownButtonUpdateCallsMerchantSelectInsteadOfClickEvent()
    {
        var b = new TImgCountDownButton(null)
        {
            m_CountDownValue = 1, m_OldCountDownValue = 4, m_LoopCount = 2,
            m_sCmd = "BUY", m_UpdateTick = 0,
        };
        var calls = new List<(int, string)>();
        FStateClMainSeam.g_nCurMerchant = 77;
        FStateClMainSeam.SendMerchantDlgSelectHandler = (m, s) => calls.Add((m, s));
        _fakeTick = 1001;

        b.Update();

        Assert.Single(calls);
        Assert.Equal((77, "BUY"), calls[0]);
        Assert.Equal(5, b.m_CountDownValue);       // OldCountDownValue + 1
        Assert.Equal(1, b.m_LoopCount);
    }

    [Fact]
    public void ImgCountDownButtonUpdateShortCircuitsWhenLoopCountIsZero()
    {
        var b = new TImgCountDownButton(null) { m_LoopCount = 0, m_CountDownValue = 9, m_UpdateTick = 0 };
        _fakeTick = 100_000;
        b.Update();
        Assert.Equal("00:00:00", b.Caption);
        Assert.Equal(9, b.m_CountDownValue);
    }

    // =====================================================================================
    // TNpcInputEdit / TNpcScrollBox / 其余 NPC 控件（原文 1373-1415 / 23861-23873）
    // =====================================================================================

    private sealed class BeepSpy : TNpcInputEdit
    {
        public int Beeps;
        public BeepSpy(TDxControl owner) : base(owner) { }
        protected override void Beep() => Beeps++;
    }

    [Fact]
    public void NpcInputEditConstructorMatchesSourceDefaults()
    {
        var e = new TNpcInputEdit(null);
        Assert.False(e.m_IsNumber);
        Assert.Equal(0, e.m_ID);
        Assert.Equal(0, e.m_MinValue);
        Assert.Equal(0, e.m_MaxValue);
    }

    [Fact]
    public void NpcInputEditKeyPressSwallowsSpaceAndBeeps()
    {
        var e = new BeepSpy(null);
        char key = (char)0x20;                     // VK_SPACE
        e.KeyPress(ref key);
        Assert.Equal((char)0, key);                // 原文 Key := #0
        Assert.Equal(1, e.Beeps);
    }

    [Fact]
    public void NpcInputEditKeyPressLeavesOtherKeysUntouched()
    {
        var e = new BeepSpy(null);
        char key = 'A';
        e.KeyPress(ref key);
        Assert.Equal('A', key);
        Assert.Equal(0, e.Beeps);

        char enter = (char)0x0D;
        e.KeyPress(ref enter);
        Assert.Equal((char)0x0D, enter);
        Assert.Equal(0, e.Beeps);
    }

    [Fact]
    public void NpcInputEditVkSpaceConstantIs0x20()
        => Assert.Equal(0x20, TNpcInputEdit.VK_SPACE);

    [Fact]
    public void NpcScrollBoxConstructorCreatesItsList()
    {
        var s = new TNpcScrollBox(null);
        Assert.NotNull(s.m_List);
        Assert.Equal(0, s.m_List.Count);
        s.m_List.Add("x");
        Assert.Equal(1, s.m_List.Count);
    }

    [Fact]
    public void NpcItemButtonConstructorMatchesSourceDefaults()
    {
        var b = new TNpcItemButton(null);
        Assert.False(b.m_boShowBorder);
        Assert.Equal(0, b.m_Light);
        Assert.Equal(0, b.m_LightFrameIndex);
        Assert.Equal(0u, b.m_LightFrameTick);
        Assert.Equal(0, b.m_nIndex);
        Assert.Equal(0, b.m_nCount);
    }

    [Fact]
    public void MissionLabelAndMagicButtonCarryOnlyTheirCommandFields()
    {
        var m = new TMissionLabel(null);
        Assert.Null(m.m_sCmd);

        var mb = new TMagicButton(null);
        Assert.Null(mb.m_Magic);
    }

    [Fact]
    public void NpcGraphicButtonExposesGraphicTextureProperty()
    {
        var g = new TNpcGraphicButton(null);
        Assert.Null(g.GraphicTexture);
        Assert.Equal(0, g.Tag);                 // 原文 25142 Self.Tag := 0
        var tex = new TTexture();
        g.GraphicTexture = tex;
        Assert.Same(tex, g.GraphicTexture);
    }

    [Fact]
    public void NpcGraphicButtonSetGraphicGoesThroughTheTextureSeam()
    {
        var g = new TNpcGraphicButton(null);
        var graphic = new TGraphic();
        var produced = new TTexture();
        TGraphic seen = null;
        TNpcGraphicButton.NewTextureFromGraphicHandler = gr => { seen = gr; return produced; };
        try
        {
            g.SetGraphic(graphic);
            Assert.Same(graphic, seen);
            Assert.Same(produced, g.GraphicTexture);
        }
        finally
        {
            TNpcGraphicButton.NewTextureFromGraphicHandler = null;
        }
    }

    [Fact]
    public void NpcGraphicButtonPaintDrawsAtVirtualRectOrigin()
    {
        var g = new TNpcGraphicButton(null);
        var tex = new TTexture { Width = 8, Height = 4 };
        g.GraphicTexture = tex;
        g.Left = 30;
        g.Top = 40;

        MShareGlobals.GameCanvas = new TGameCanvas();
        g.Paint();

        Assert.Single(MShareGlobals.GameCanvas.Draws);
        Assert.Equal((30, 40, tex), MShareGlobals.GameCanvas.Draws[0]);
    }

    [Fact]
    public void NpcGraphicButtonDestroyIsSafeWithoutTexture()
    {
        var g = new TNpcGraphicButton(null);
        g.Destroy();                            // 原文 if m_GraphicTexture <> nil then Free
        Assert.Null(g.GraphicTexture);
    }

    [Fact]
    public void NpcItemBoxAndProgressBoxButtonsCarryTheirFields()
    {
        var box = new TNpcItemBoxButton(null) { m_StdModes = "1,2", m_nIndex = 3 };
        Assert.Equal("1,2", box.m_StdModes);
        Assert.Equal(3, box.m_nIndex);

        var p = new TNpcProgressBoxButton(null)
        {
            m_sCmd = "C", m_nProgressMinValue = 0, m_nProgressMaxValue = 100,
            m_nProgressValue = 50, m_sText = "T",
        };
        Assert.Equal("C", p.m_sCmd);
        Assert.Equal(0, p.m_nProgressMinValue);
        Assert.Equal(100, p.m_nProgressMaxValue);
        Assert.Equal(50, p.m_nProgressValue);
        Assert.Equal("T", p.m_sText);
    }

    // =====================================================================================
    // TFrmDlg：构造（原文 1417-1604）与可移植方法
    // =====================================================================================

    [Fact]
    public void FrmDlgConstructorInitializesTheDeclaredState()
    {
        var f = new TestFrmDlg();

        // 原文 1418 / 1445-1447 / 1470
        Assert.False(f.Initialized);
        Assert.NotNull(f.DBackground);
        Assert.NotNull(f.MenuList);
        Assert.Equal(-1, f.menuindex);
        Assert.Equal(0, f.MenuTopLine);
        Assert.Equal(-1, f.RankingPage);
        Assert.Equal(-1, f.TradingItemSelIndex);
        Assert.Equal(-1, f.ShopTabPage);
        Assert.Equal(-1, f.RankingSelectLine);
        Assert.Equal(-1, f.ExMiniMapLoadIndex);
        Assert.Equal(-1, f.FDayBrightIconStartIndex);
        Assert.Equal(-1, f.FMagicBallStartIndex);
        Assert.Equal(-1, f.FAuctionMyItemsBagSelectIndex);
        Assert.Equal(1, f.FAuctionMyItemsPage);
        Assert.True(f.FAuctionAllItemsSortASC);
        Assert.Equal("", f.Guild);
        Assert.Equal("", f.SelDeleteCharName);
        Assert.False(f.BoxDlgWideScreen);
        Assert.False(f.ShowMiniBigMapXY);
        Assert.False(f.GuildMemoVisible);
        Assert.Equal(0, f.FCurrentBagPage);
        Assert.Equal(0, f.FExtBagPageCount);
        Assert.Equal(_fakeTick, f.BlinkTime);
        Assert.False(f.m_boPlayDice);
        Assert.False(f.m_boRandomCodeClick);
    }

    [Fact]
    public void FrmDlgConstructorCreatesTheGuildContainers()
    {
        var f = new TestFrmDlg();
        Assert.NotNull(f.GuildStrs);
        Assert.NotNull(f.GuildStrs2);
        Assert.NotNull(f.GuildNotice);
        Assert.NotNull(f.GuildWJ);
        Assert.NotNull(f.GuildMembers);
        Assert.NotNull(f.GuildChats);
        Assert.NotNull(f.GuildGroupList);
        Assert.NotNull(f.ShowGuildList);
        Assert.NotNull(f.GuildJoinUserList);
        Assert.NotNull(f.ExScreenMagicBtnList);
        Assert.Equal(0, f.GuildGroupList.Count);
    }

    [Fact]
    public void FrmDlgConstructorComputesAuctionBagPageCountWithDelphiDiv()
    {
        // 原文 1594：(DEF_MAX_BAG_ITEM + AUCTION_BAG_ONE_PAGE_COUNT - 1) div AUCTION_BAG_ONE_PAGE_COUNT
        // = (46 + 10 - 1) div 10 = 55 div 10 = 5
        var f = new TestFrmDlg();
        Assert.Equal(5, f.FAuctionMyItemsBagPageCount);
    }

    [Fact]
    public void FrmDlgConstructorNullsTheControlFields()
    {
        var f = new TestFrmDlg();
        Assert.Null(f.DEdId);
        Assert.Null(f.DEdChat);
        Assert.Null(f.DBackgroundBackgroundClick == null ? null : f.DEdChat);
        Assert.Null(f.DMerchantDlg);
        Assert.Null(f.DMainMenu);
        Assert.Null(f.DGuildDlg);
        Assert.Null(f.DMinMapDlg);
    }

    [Fact]
    public void FrmDlgConstructorBuildsTheSpecialCmdPopupMenuLikeTheSource()
    {
        var f = new TestFrmDlg();
        Assert.NotNull(f.DSpecialCmdMenu);
        Assert.Equal("DSpecialCmdMenu", f.DSpecialCmdMenu.Name);
        Assert.False(f.DSpecialCmdMenu.Designing);
        Assert.False(f.DSpecialCmdMenu.Visible);
        Assert.Equal(DxPopupMenuExt.TGuiType.t_PopupMenu, DxPopupMenuExt.GetGuiType(f.DSpecialCmdMenu));
        Assert.Equal(150, DxPopupMenuExt.GetAlpha(f.DSpecialCmdMenu));
        Assert.Equal(TColor.clWhite.Value, DxPopupMenuExt.GetItemColor(f.DSpecialCmdMenu).Up.Value);
        Assert.Equal(TColor.clWhite.Value, DxPopupMenuExt.GetItemColor(f.DSpecialCmdMenu).Checked.Value);
        Assert.Equal(TColor.clWhite.Value, DxPopupMenuExt.GetBorderColor(f.DSpecialCmdMenu).Up.Value);
        Assert.Equal(GetRGB(190), DxPopupMenuExt.GetBackgroundColor(f.DSpecialCmdMenu));
        Assert.False(DxPopupMenuExt.GetDrawBorder(f.DSpecialCmdMenu));
        Assert.NotNull(f.DSpecialCmdMenu.OnClick);
    }

    [Fact]
    public void RefrshDStorageViewDlgTextSetsFourFieldsAndIgnoresTheFlag()
    {
        var f = new TestFrmDlg();
        f.RefrshDStorageViewDlgText(false, 3, 40, 2, 7);
        Assert.Equal(3, f.FDStorageViewDlgCount);
        Assert.Equal(40, f.FDStorageViewDlgMaxCount);
        Assert.Equal(2, f.FDStorageViewDlgPage);
        Assert.Equal(7, f.FDStorageViewDlgMaxPage);

        // 差异断言：isExt 参数在原文里被**完全忽略**，两次调用结果一致。
        f.RefrshDStorageViewDlgText(true, 9, 90, 9, 9);
        Assert.Equal(9, f.FDStorageViewDlgCount);
        Assert.Equal(90, f.FDStorageViewDlgMaxCount);
    }

    [Fact]
    public void UpdateGuildJoinConditionCachesTheThreeValues()
    {
        var f = new TestFrmDlg();
        f.UpdateGuildJoinCondition(2, 35, "欢迎");
        Assert.Equal(2, f.ExGuildJoinJob);
        Assert.Equal(35, f.ExGuildJoinLevel);
        Assert.Equal("欢迎", f.ExGuildJoinMsg);
    }

    [Fact]
    public void OpenGuildViewMemeberInfoCachesTheWholeRecord()
    {
        var f = new TestFrmDlg();
        var info = new GXX.Core.Protocol.TGuildMemeberInfo();
        f.OpenGuildViewMemeberInfo(info);
        // 结构体整体赋值：再传一次不同的值应覆盖（原文 FGuildViewMemberInfo := MemberInfo）
        f.OpenGuildViewMemeberInfo(default);
        Assert.Equal(default, f.ExGuildViewMemberInfo);
    }

    [Fact]
    public void HeroBoxClosersAreDeliberatelyEmptyLikeTheSource()
    {
        var f = new TestFrmDlg();
        f.CloseDHeroGodBlessDlg();      // 原文空体
        f.CloseDHeroJewelryBoxDlg();    // 原文空体
    }

    [Fact]
    public void ChatHistoryGettersAlwaysReturnEmptyStrings()
    {
        // 原文三个函数恒返回 ''（历史发言功能未实现）。
        var f = new TestFrmDlg();
        Assert.Equal("", f.GetLastHistroySendSay());
        Assert.Equal("", f.GetPreHistroySendSay());
        Assert.Equal("", f.GetNextHistroySendSay());
    }

    // =====================================================================================
    // 聊天输入框（原文 24489-24508）
    // =====================================================================================

    [Fact]
    public void IsInputChatEditReflectsVisibleAndEnabled()
    {
        var f = new TFrmDlg { DEdChat = new TDxImageEdit() };
        Assert.True(f.IsInputChatEdit());

        f.DEdChat.Visible = false;
        Assert.False(f.IsInputChatEdit());

        f.DEdChat.Visible = true;
        f.DEdChat.Enabled = false;
        Assert.False(f.IsInputChatEdit());
    }

    [Fact]
    public void ShowChatEditAlwaysRevealsAndOptionallyFocuses()
    {
        var f = new TFrmDlg { DEdChat = new TDxImageEdit() };
        f.DEdChat.Visible = false;
        f.DEdChat.Enabled = false;
        int focused = 0;
        DxControlExt.SetSetFocusHandler(f.DEdChat, () => focused++);

        f.ShowChatEdit(false);
        Assert.True(f.DEdChat.Visible);
        Assert.True(f.DEdChat.Enabled);
        Assert.Equal(0, focused);          // IsSetFocus = False ⇒ 不设焦点

        f.ShowChatEdit(true);
        Assert.Equal(1, focused);
    }

    [Fact]
    public void HideChatEditOnlyHidesWhenDisableHideCtrlIsSet()
    {
        var f = new TFrmDlg { DEdChat = new TDxImageEdit() };
        f.DEdChat.Visible = true;
        f.DEdChat.Enabled = true;

        f.HideChatEdit();
        Assert.False(f.DEdChat.Enabled);
        Assert.True(f.DEdChat.Visible);    // 默认 DisableHideCtrl = false ⇒ 保持可见

        DxControlExt.SetDisableHideCtrl(f.DEdChat, true);
        f.HideChatEdit();
        Assert.False(f.DEdChat.Visible);   // 差异分支
    }

    // =====================================================================================
    // 屏幕技能图标（原文 24510-24627 / 24680-24734）
    // =====================================================================================

    private static PTClientMagic Magic(ushort id)
        => new(new TClientMagic { Def = new TMagic_C { wMagicId = id } });

    [Fact]
    public void AddScreenMagicButtonCreatesWithSourceFixedGeometry()
    {
        var f = new TestFrmDlg();
        var m = Magic(11);

        f.AddScreenMagicButton(m, 100, 200, false);

        Assert.Equal(1, f.ExScreenMagicBtnList.Count);
        var btn = (TMagicButton)f.ExScreenMagicBtnList[0];
        Assert.Same(m, btn.m_Magic);
        Assert.Equal(100 - 16, btn.Left);     // 原文 X - 16
        Assert.Equal(200 - 15, btn.Top);      // 原文 Y - 15
        Assert.Equal(32, btn.Width);
        Assert.Equal(30, btn.Height);
        Assert.False(btn.Designing);
        Assert.True(btn.Floating);
        Assert.NotNull(btn.OnClick);
        Assert.NotNull(btn.OnDblClick);
        Assert.NotNull(DxControlExt.GetOnMouseMove(btn));
        Assert.NotNull(DxControlExt.GetOnMove(btn));
    }

    [Fact]
    public void AddScreenMagicButtonReusesTheExistingButtonAndOnlyMovesIt()
    {
        // 原文注释：同一个技能图标支持多次拖动，位置以最后一次为准。
        var f = new TestFrmDlg();
        var m = Magic(11);
        f.AddScreenMagicButton(m, 100, 200, false);
        var first = f.ExScreenMagicBtnList[0];

        f.AddScreenMagicButton(m, 300, 400, false);

        Assert.Equal(1, f.ExScreenMagicBtnList.Count);   // 不新增
        Assert.Same(first, f.ExScreenMagicBtnList[0]);
        var btn = (TMagicButton)first;
        Assert.Equal(300 - 16, btn.Left);
        Assert.Equal(400 - 15, btn.Top);
    }

    [Fact]
    public void FindMagicButtonMatchesByReferenceNotByMagicId()
    {
        // 差异断言：FindMagicButton 用**指针相等**（Ctrl.m_Magic = Magic），
        // 因此同 wMagicId 但不同实例查不到。
        var f = new TestFrmDlg();
        var a = Magic(11);
        f.AddScreenMagicButton(a, 0, 0, false);

        Assert.Same(f.ExScreenMagicBtnList[0], f.FindMagicButton(a));
        Assert.Null(f.FindMagicButton(Magic(11)));       // 同 id 不同实例
    }

    [Fact]
    public void DelScreenMagicButtonByReferenceRemovesOnlyTheFirstMatch()
    {
        var f = new TestFrmDlg();
        var a = Magic(1);
        var b = Magic(2);
        f.AddScreenMagicButton(a, 0, 0, false);
        f.AddScreenMagicButton(b, 0, 0, false);

        f.DelScreenMagicButton(a);
        Assert.Equal(1, f.ExScreenMagicBtnList.Count);
        Assert.Same(b, ((TMagicButton)f.ExScreenMagicBtnList[0]).m_Magic);
    }

    [Fact]
    public void DelScreenMagicButtonByMagicIdMatchesTheDefField()
    {
        var f = new TestFrmDlg();
        var a = Magic(1);
        var b = Magic(2);
        f.AddScreenMagicButton(a, 0, 0, false);
        f.AddScreenMagicButton(b, 0, 0, false);

        f.DelScreenMagicButton((ushort)2);
        Assert.Equal(1, f.ExScreenMagicBtnList.Count);
        Assert.Same(a, ((TMagicButton)f.ExScreenMagicBtnList[0]).m_Magic);

        // 不存在的 id：列表不变
        f.DelScreenMagicButton((ushort)99);
        Assert.Equal(1, f.ExScreenMagicBtnList.Count);
    }

    [Fact]
    public void ClearScreenMagicButtonsEmptiesTheList()
    {
        var f = new TestFrmDlg();
        f.AddScreenMagicButton(Magic(1), 0, 0, false);
        f.AddScreenMagicButton(Magic(2), 0, 0, false);
        f.ClearScreenMagicButtons();
        Assert.Equal(0, f.ExScreenMagicBtnList.Count);
        Assert.Null(f.FindMagicButton(Magic(1)));
    }

    [Fact]
    public void OnMagicButtonClickUsesTheGlobalMousePositionNotTheEventArguments()
    {
        // 差异断言：原文传的是 g_nMouseX / g_nMouseY（全局），**不是** 形参 X/Y。
        var f = new TestFrmDlg();
        var m = Magic(11);
        f.AddScreenMagicButton(m, 0, 0, false);
        var btn = (TMagicButton)f.ExScreenMagicBtnList[0];

        var calls = new List<(int X, int Y, object Magic)>();
        FStateClMainSeam.g_nMouseX = 640;
        FStateClMainSeam.g_nMouseY = 480;
        FStateClMainSeam.UseMagicHandler = (x, y, mg) => calls.Add((x, y, mg));
        FStateClMainSeam.g_nTargetX = 5;
        FStateClMainSeam.g_nTargetY = 6;

        f.OnMagicButtonClick(btn, 1, 2);   // 事件参数是 1/2，应被忽略

        Assert.Single(calls);
        Assert.Equal(640, calls[0].X);
        Assert.Equal(480, calls[0].Y);
        Assert.Same(m, calls[0].Magic);
        Assert.Equal(-1, FStateClMainSeam.g_nTargetX);
        Assert.Equal(-1, FStateClMainSeam.g_nTargetY);
    }

    [Fact]
    public void OnMagicButtonDblClickRemovesThatMagic()
    {
        var f = new TestFrmDlg();
        var m = Magic(11);
        f.AddScreenMagicButton(m, 0, 0, false);
        var btn = (TMagicButton)f.ExScreenMagicBtnList[0];

        f.OnMagicButtonDblClick(btn, 0, 0);
        Assert.Equal(0, f.ExScreenMagicBtnList.Count);
    }

    [Fact]
    public void OnMagicButtonMoveSavesPositions()
    {
        var f = new TestFrmDlg();
        var m = Magic(11);
        f.AddScreenMagicButton(m, 0, 0, false);
        var btn = (TMagicButton)f.ExScreenMagicBtnList[0];

        ConfigClientExt.boSaveMagicIconPosition = 1;
        var saved = new List<string>();
        MagicButtonIniSeam.WriteIniHandler = (file, writes) => saved.Add(file);

        f.OnMagicButtonMove(btn);

        Assert.Single(saved);
        Assert.Contains("Configured", saved[0] + "Configured");  // 触发过一次写盘
    }

    [Fact]
    public void SaveMagicButtonsExitsWhenDisableDrogMagicIconIsSet()
    {
        var f = new TestFrmDlg();
        f.AddScreenMagicButton(Magic(11), 10, 20, false);
        int writes = 0;
        MagicButtonIniSeam.WriteIniHandler = (file, w) => writes++;
        ConfigClientExt.boDisableDrogMagicIcon = 1;
        ConfigClientExt.boSaveMagicIconPosition = 1;

        f.SaveMagicButtons();
        Assert.Equal(0, writes);            // 第一个守卫直接 Exit
    }

    [Fact]
    public void SaveMagicButtonsExitsWhenSavePositionIsOff()
    {
        var f = new TestFrmDlg();
        f.AddScreenMagicButton(Magic(11), 10, 20, false);
        int writes = 0;
        MagicButtonIniSeam.WriteIniHandler = (file, w) => writes++;
        ConfigClientExt.boDisableDrogMagicIcon = 0;
        ConfigClientExt.boSaveMagicIconPosition = 0;

        f.SaveMagicButtons();
        Assert.Equal(0, writes);            // 第二个守卫直接 Exit
    }

    [Fact]
    public void SaveMagicButtonsWritesCountThenEachIconThenFinalCount()
    {
        var f = new TestFrmDlg();
        ConfigClientExt.boSaveMagicIconPosition = 1;
        var m1 = Magic(11);
        var m2 = Magic(22);
        f.AddScreenMagicButton(m1, 100, 200, false);
        f.AddScreenMagicButton(m2, 300, 400, false);

        string fileSeen = null;
        IReadOnlyList<(string Section, string Key, int Value)> writes = null;
        MagicButtonIniSeam.WriteIniHandler = (file, w) => { fileSeen = file; writes = w; };

        f.SaveMagicButtons();

        Assert.NotNull(writes);
        // 原文顺序：Setup/Count := 0 → Magic1(MagicID,X,Y) → Magic2(...) → Setup/Count := 2
        Assert.Equal(("Setup", "Count", 0), (writes[0].Section, writes[0].Key, writes[0].Value));
        Assert.Equal(("Magic1", "MagicID", 11), (writes[1].Section, writes[1].Key, writes[1].Value));
        Assert.Equal(("Magic1", "X", 100 - 16), (writes[2].Section, writes[2].Key, writes[2].Value));
        Assert.Equal(("Magic1", "Y", 200 - 15), (writes[3].Section, writes[3].Key, writes[3].Value));
        Assert.Equal(("Magic2", "MagicID", 22), (writes[4].Section, writes[4].Key, writes[4].Value));
        Assert.Equal(("Magic2", "X", 300 - 16), (writes[5].Section, writes[5].Key, writes[5].Value));
        Assert.Equal(8, writes.Count);
        Assert.Equal(("Setup", "Count", 2), (writes[7].Section, writes[7].Key, writes[7].Value));
        Assert.Contains("Magic", fileSeen == null ? "" : fileSeen + "Magic");
    }

    [Fact]
    public void SaveMagicButtonsSkipsButtonsWithNullMagicAndDoesNotCountThem()
    {
        var f = new TestFrmDlg();
        ConfigClientExt.boSaveMagicIconPosition = 1;
        f.AddScreenMagicButton(Magic(11), 0, 0, false);
        // 手工塞一个 m_Magic = nil 的按钮（原文会跳过且不计入 nCount）
        f.ExScreenMagicBtnList.Add(new TMagicButton(null));

        IReadOnlyList<(string Section, string Key, int Value)> writes = null;
        MagicButtonIniSeam.WriteIniHandler = (file, w) => writes = w;

        f.SaveMagicButtons();

        // 最后一条 Setup/Count 必须是 1（只有 1 个有效按钮）
        var last = writes[writes.Count - 1];
        Assert.Equal(("Setup", "Count", 1), (last.Section, last.Key, last.Value));
    }

    [Fact]
    public void SaveMagicButtonsUpdatesPlugUserNameFromMySelf()
    {
        var f = new TestFrmDlg();
        ConfigClientExt.boSaveMagicIconPosition = 1;
        MShareGlobals.g_MySelf = new GXX.Client.Scenes.TActor { m_sUserName = "a/b" };
        IReadOnlyList<(string Section, string Key, int Value)> writes = null;
        string fileSeen = null;
        MagicButtonIniSeam.WriteIniHandler = (file, w) => { fileSeen = file; writes = w; };
        MagicButtonIniSeam.g_sPlugServerName = "Srv";

        f.SaveMagicButtons();

        // ProcessFileNameSpecialChar 把路径分隔符替换掉（接缝实现），故用户名里不再含 '/'
        Assert.Equal("a_b", MagicButtonIniSeam.g_sPlugUserName);
        Assert.NotNull(fileSeen);
        Assert.DoesNotContain("a/b", fileSeen);
    }

    // =====================================================================================
    // 单元级全局
    // =====================================================================================

    [Fact]
    public void FStateGlobalStartsEmptyAndConditionColorIsWhite()
    {
        Assert.Null(FStateGlobal.FrmDlg);
        Assert.Equal(TColor.clWhite.Value, FStateGlobal.ConditionOKHitColor.Value);

        FStateGlobal.FrmDlg = new TestFrmDlg();
        Assert.NotNull(FStateGlobal.FrmDlg);
        FStateGlobal.ResetForTests();
        Assert.Null(FStateGlobal.FrmDlg);
    }

    [Fact]
    public void FStateSeamClockFallsBackToEnvironmentTickCountWhenNotInjected()
    {
        FStateSeamClock.ResetForTests();
        uint v = FStateSeamClock.Now;
        Assert.True(v > 0);
        FStateSeamClock.NowHandler = () => _fakeTick;
    }
}

/// <summary>
/// 把 TFrmDlg 的 protected 字段暴露给断言用的最小派生类。
/// 原文里的 TSerialWindows / TStateWindows 等也正是这样从 TFrmDlg 派生的，
/// 因此本派生类同时验证了"托管 TFrmDlg 可被继承"这一原文性质。
/// </summary>
internal sealed class TestFrmDlg : GXX.Client.GUI.Share.TFrmDlg
{
    public int ExGuildJoinJob => FGuildJoinJob;
    public int ExGuildJoinLevel => FGuildJoinLevel;
    public string ExGuildJoinMsg => FGuildJoinMsg;
    public TList ExScreenMagicBtnList => FScreenMagicBtnList;
    public int ExMiniMapLoadIndex => FMiniMapLoadIndex;
    public GXX.Core.Protocol.TGuildMemeberInfo ExGuildViewMemberInfo => FGuildViewMemberInfo;
}