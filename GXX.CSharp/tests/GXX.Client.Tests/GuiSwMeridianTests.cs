using System;
using System.Collections.Generic;
using GXX.Client.GUI.NewStateWin;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 并行批次 P1 切片 C：StateWindows.pas 经脉（Meridians）族
/// （4801-4827 / 5595-5601 / 6275-6304 / 6306-6485 / 4099-4111 / 9843-9851 行）。
/// </summary>
public sealed class GuiSwMeridianTests
{
    // ===================== 常量表 =====================

    [Fact]
    public void MeridiansImageIndexArrayMatchesInitialize()
    {
        // 原文 4803-4807。
        Assert.Equal(new[] { 860, 870, 880, 890, 1180 }, TStateWindowsMeridians.MeridiansImageIndexArray);
        Assert.Equal(5, TStateWindowsMeridians.MeridianPageCount);
        Assert.Equal(5, TStateWindowsMeridians.AcupointCount);
    }

    [Fact]
    public void MeridianNamesMatchCommentedConst()
    {
        // 原文 6286 被注释掉的 Arr_Names（6298 仍在引用）。
        Assert.Equal(new[] { "冲脉", "阴跷", "阴维", "任脉", "奇经" }, TStateWindowsMeridians.MeridianNames);
    }

    [Fact]
    public void TrainingCaptionsMatchSwitch()
    {
        // 原文 6468-6472。
        Assert.Equal(new[] { "修炼穴位", "修炼冲脉", "修炼阴跷", "修炼阴维", "修炼任脉" },
            TStateWindowsMeridians.TrainingButtonCaptions);
    }

    [Fact]
    public void AcupointNamesTableHasFivePagesOfFiveEach()
    {
        // 原文 6364-6404。
        Assert.Equal(5, TStateWindowsMeridians.AcupointNamesByActivePage.Length);
        foreach (var page in TStateWindowsMeridians.AcupointNamesByActivePage)
            Assert.Equal(5, page.Length);

        Assert.Equal("神冲穴", TStateWindowsMeridians.AcupointNamesByActivePage[0][0]);
        Assert.Equal("涌泉穴", TStateWindowsMeridians.AcupointNamesByActivePage[0][4]);
        Assert.Equal("幽门穴", TStateWindowsMeridians.AcupointNamesByActivePage[1][0]);
        Assert.Equal("骨曲穴", TStateWindowsMeridians.AcupointNamesByActivePage[4][4]);
    }

    // ===================== 页索引 ↔ 经脉索引 =====================

    [Theory]
    [InlineData(0, 4)]
    [InlineData(1, 0)]
    [InlineData(2, 1)]
    [InlineData(3, 2)]
    [InlineData(4, 3)]
    public void ActivePageToMeridianIndexWrapsZeroToFour(int activePage, int expected)
    {
        Assert.Equal(expected, TStateWindowsMeridians.ActivePageToMeridianIndex(activePage));
    }

    [Fact]
    public void ActivePageToMeridianIndexClampsNegativesToFour()
    {
        // 原文 `if nPage < 0 then nPage := 4`（不是 0）。
        Assert.Equal(4, TStateWindowsMeridians.ActivePageToMeridianIndex(-1));
        Assert.Equal(4, TStateWindowsMeridians.ActivePageToMeridianIndex(-99));
    }

    [Fact]
    public void FunAArgsArePageAndNameIndexSwapped()
    {
        // 差异断言：FunA 的第一个实参是经脉序号，第二个是按钮名前缀序号，两者**反序**。
        Assert.True(TStateWindowsMeridians.ActivePageToFunAArgs(0, out int p0, out int n0));
        Assert.Equal((4, 0), (p0, n0));

        Assert.True(TStateWindowsMeridians.ActivePageToFunAArgs(1, out int p1, out int n1));
        Assert.Equal((0, 1), (p1, n1));

        Assert.True(TStateWindowsMeridians.ActivePageToFunAArgs(4, out int p4, out int n4));
        Assert.Equal((3, 4), (p4, n4));
    }

    [Fact]
    public void FunAArgsOutOfRangeReturnsFalse()
    {
        Assert.False(TStateWindowsMeridians.ActivePageToFunAArgs(5, out _, out _));
        Assert.False(TStateWindowsMeridians.ActivePageToFunAArgs(-1, out _, out _));
    }

    [Fact]
    public void AcupointButtonNameFormatMatchesOriginal()
    {
        Assert.Equal("DBotAcupoints0_3", string.Format(TStateWindowsMeridians.AcupointButtonNameFormat, 0, 3));
        Assert.Equal(2, TStateWindowsMeridians.MatchAcupointIndex("DBotAcupoints1_2", 1));
        Assert.Equal(0, TStateWindowsMeridians.MatchAcupointIndex("DBotAcupoints4_0", 4));
        // 不匹配 → -1（原文两个消息都保持空串）。
        Assert.Equal(-1, TStateWindowsMeridians.MatchAcupointIndex("DBotAcupoints1_2", 2));
        Assert.Equal(-1, TStateWindowsMeridians.MatchAcupointIndex("", 0));
    }

    [Fact]
    public void AcupointMatchIsCaseSensitiveLikeDelphi()
    {
        // Delphi 字符串 = 兼容比较（区分大小写），故大小写不同不匹配。
        Assert.Equal(-1, TStateWindowsMeridians.MatchAcupointIndex("dbotacupoints1_2", 1));
    }

    // ===================== GetMeridianStateInfo =====================

    [Fact]
    public void NotOpenedShowsWeiTong()
    {
        Assert.Equal("经\r络\r未\r通", TStateWindowsMeridians.GetMeridianStateInfo(0, 0));
        // 差异断言：Acupoints[4] 是唯一判据，Level 再高也不影响。
        Assert.Equal("经\r络\r未\r通", TStateWindowsMeridians.GetMeridianStateInfo(0, 10));
        Assert.Equal("经\r络\r未\r通", TStateWindowsMeridians.GetMeridianStateInfo(-1, 5));
    }

    [Fact]
    public void OpenedWithoutLevelShowsYiTong()
    {
        Assert.Equal("经\r络\r已\r通", TStateWindowsMeridians.GetMeridianStateInfo(1, 0));
        Assert.Equal("经\r络\r已\r通", TStateWindowsMeridians.GetMeridianStateInfo(5, -1));
    }

    [Theory]
    [InlineData(1, "一")]
    [InlineData(2, "二")]
    [InlineData(3, "三")]
    [InlineData(4, "四")]
    [InlineData(5, "五")]
    [InlineData(6, "六")]
    [InlineData(7, "七")]
    [InlineData(8, "八")]
    [InlineData(9, "九")]
    [InlineData(10, "十")]
    public void OpenedWithLevelShowsLevelString(int level, string expected)
    {
        // 原文 4816 MeridianLevelStrings:array[1..10]，1-based。
        Assert.Equal(expected + "\r重\r经\r络", TStateWindowsMeridians.GetMeridianStateInfo(1, level));
    }

    [Fact]
    public void SeparatorIsCarriageReturnNotCrLf()
    {
        // 差异断言：用的是 #13（CR）而不是 #13#10，也不是 '\'。
        string s = TStateWindowsMeridians.GetMeridianStateInfo(1, 1);
        Assert.Contains("\r", s);
        Assert.DoesNotContain("\n", s);
        Assert.DoesNotContain("\\", s);
        Assert.Equal(7, s.Length); // 1 + 1 + 1 + 1 + 1 + 1 + 1
    }

    [Fact]
    public void LevelOutOfTableRangeYieldsEmptyLevelText()
    {
        // 原文越界（Level = 11）在 Delphi 不检查；此处退化为空串但保留其余文本。
        Assert.Equal("\r重\r经\r络", TStateWindowsMeridians.GetMeridianStateInfo(1, 11));
    }

    [Fact]
    public void ProviderOverloadReadsFromSeam()
    {
        TStateWindowsMeridians.MeridianStatusProvider provider = (int idx, out int acupoint4, out int level) =>
        {
            acupoint4 = 1;
            level = 3;
        };

        Assert.Equal("三\r重\r经\r络", TStateWindowsMeridians.GetMeridianStateInfo(2, provider));
    }

    // ===================== SetMeridiansLevel =====================

    [Fact]
    public void SetMeridiansLevelAddsLevelToBaseIndex()
    {
        Assert.True(TStateWindowsMeridians.TrySetMeridiansLevel(1, 0, 0, out int i0));
        Assert.Equal(860, i0);

        Assert.True(TStateWindowsMeridians.TrySetMeridiansLevel(1, 4, 5, out int i4));
        Assert.Equal(1185, i4);

        Assert.True(TStateWindowsMeridians.TrySetMeridiansLevel(1, 2, 3, out int i2));
        Assert.Equal(883, i2);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(5, 0)]
    [InlineData(0, -1)]
    [InlineData(0, 6)]
    public void SetMeridiansLevelRejectsOutOfRange(int meridian, int level)
    {
        Assert.False(TStateWindowsMeridians.TrySetMeridiansLevel(1, meridian, level, out int idx));
        Assert.Equal(0, idx);
    }

    [Fact]
    public void SetMeridiansLevelRequiresInitialized()
    {
        // 原文 6277：if not Initialized then Exit;
        Assert.False(TStateWindowsMeridians.TrySetMeridiansLevel(0, 2, 3, out _));
        Assert.True(TStateWindowsMeridians.TrySetMeridiansLevel(1, 2, 3, out _));
    }

    // ===================== 点击事件 =====================

    [Fact]
    public void TrainingMeridianClickSendsMessageWithMappedPage()
    {
        int nPage = TStateWindowsMeridians.TryBuildTrainingMeridianClick(0, 0, out var msg);

        Assert.Equal(4, nPage);
        Assert.Equal(Grobal2Const.CM_SENDTRAININGMERIDIANCLICK, msg.Ident);
        Assert.Equal(146, msg.Ident);
        Assert.Equal(0, msg.Recog);
        Assert.Equal(4, msg.Param);
        Assert.Equal(0, msg.Tag);
        Assert.Equal(0, msg.Series);
    }

    [Fact]
    public void TrainingMeridianClickPage1SendsZero()
    {
        TStateWindowsMeridians.TryBuildTrainingMeridianClick(0, 1, out var msg);
        Assert.Equal(0, msg.Param);
    }

    [Fact]
    public void TrainingMeridianClickBailsOutWhenMySelfIsNull()
    {
        // 原文 6291：if g_MySelf = nil then Exit;（不发消息）
        Assert.Equal(-1, TStateWindowsMeridians.TryBuildTrainingMeridianClick(1, 2, out _));
    }

    [Fact]
    public void AcupointClickPassesSenderTagThrough()
    {
        int nPage = TStateWindowsMeridians.TryBuildAcupointClick(0, 1, 3, out var msg);

        Assert.Equal(0, nPage);
        Assert.Equal(Grobal2Const.CM_SENDACUPOINTCLICK, msg.Ident);
        Assert.Equal(145, msg.Ident);
        Assert.Equal(0, msg.Recog);
        Assert.Equal(0, msg.Param);
        Assert.Equal(3, msg.Tag);       // TDxControl(Sender).Tag 原样透传
        Assert.Equal(0, msg.Series);
    }

    [Fact]
    public void AcupointClickBailsOutWhenMySelfIsNull()
    {
        Assert.Equal(-1, TStateWindowsMeridians.TryBuildAcupointClick(1, 0, 0, out _));
    }

    [Fact]
    public void RecallDeputyHeroDefaultsToJobZero()
    {
        // 三个都不勾选 → btJob 保持 0（原文 4103）。
        var msg = TStateWindowsMeridians.BuildRecallDeputyHeroClick(false, false, false);

        Assert.Equal(Grobal2Const.CM_HEROLOGON, msg.Ident);
        Assert.Equal(151, msg.Ident);
        Assert.Equal(1, msg.Recog);   // 原文写死 1
        Assert.Equal(0, msg.Param);
        Assert.Equal(0, msg.Tag);
        Assert.Equal(0, msg.Series);
    }

    [Theory]
    [InlineData(true, false, false, 0)]
    [InlineData(false, true, false, 1)]
    [InlineData(false, false, true, 2)]
    public void RecallDeputyHeroReadsCheckedFlags(bool c0, bool c1, bool c2, int expected)
    {
        Assert.Equal(expected, TStateWindowsMeridians.BuildRecallDeputyHeroClick(c0, c1, c2).Param);
    }

    [Fact]
    public void RecallDeputyHeroUsesIfElseIfSoFirstMatchWins()
    {
        // 差异断言：原文用 else if 链，故多个勾选时**第一个**命中者胜出（不是最后一个）。
        Assert.Equal(0, TStateWindowsMeridians.BuildRecallDeputyHeroClick(true, true, true).Param);
        Assert.Equal(1, TStateWindowsMeridians.BuildRecallDeputyHeroClick(false, true, true).Param);
        Assert.Equal(2, TStateWindowsMeridians.BuildRecallDeputyHeroClick(false, false, true).Param);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    public void SetDeputyHeroJobSelectsButton(int job, int expected)
    {
        Assert.Equal(expected, TStateWindowsMeridians.TrySetDeputyHeroJob(1, job));
    }

    [Fact]
    public void SetDeputyHeroJobRejectsOutOfRangeAndUninitialized()
    {
        Assert.Equal(-1, TStateWindowsMeridians.TrySetDeputyHeroJob(1, 3));
        Assert.Equal(-1, TStateWindowsMeridians.TrySetDeputyHeroJob(1, 255));
        Assert.Equal(-1, TStateWindowsMeridians.TrySetDeputyHeroJob(1, -1));
        Assert.Equal(-1, TStateWindowsMeridians.TrySetDeputyHeroJob(0, 1));
    }

    // ===================== 穴位悬浮提示 =====================

    private static readonly string[] Page0Names = TStateWindowsMeridians.AcupointNamesByActivePage[0];

    [Fact]
    public void OpenedAcupointShowsAlreadyOpen()
    {
        var hint = TStateWindowsMeridians.BuildAcupointHint(
            Page0Names, "DBotAcupoints0_2", 0, _ => 1, currentNgLevel: 1, requiredLevel: _ => 9);

        Assert.Equal("夹脊穴：已打通", hint.Msg1);
        Assert.Equal(string.Empty, hint.Msg2);
    }

    [Fact]
    public void LevelSufficientGoesToMsg1WithBackslashSeparator()
    {
        // 原文 6340：'：待打通\' + '需要内功等级N级' —— 分隔符是反斜杠。
        var hint = TStateWindowsMeridians.BuildAcupointHint(
            Page0Names, "DBotAcupoints0_0", 0, _ => 0, currentNgLevel: 5, requiredLevel: _ => 5);

        Assert.Equal("神冲穴：待打通\\需要内功等级5级", hint.Msg1);
        Assert.Equal(string.Empty, hint.Msg2);
    }

    [Fact]
    public void LevelInsufficientGoesToMsg2WithSameText()
    {
        var low = TStateWindowsMeridians.BuildAcupointHint(
            Page0Names, "DBotAcupoints0_0", 0, _ => 0, currentNgLevel: 4, requiredLevel: _ => 5);

        Assert.Equal(string.Empty, low.Msg1);
        Assert.Equal("神冲穴：待打通\\需要内功等级5级", low.Msg2);

        // 差异断言：>= 走 Msg1（黄字），< 走 Msg2（红字），文本相同但颜色不同。
        var equal = TStateWindowsMeridians.BuildAcupointHint(
            Page0Names, "DBotAcupoints0_0", 0, _ => 0, currentNgLevel: 5, requiredLevel: _ => 5);
        Assert.Equal(low.Msg2, equal.Msg1);
    }

    [Fact]
    public void NonMatchingButtonNameYieldsBothEmpty()
    {
        var hint = TStateWindowsMeridians.BuildAcupointHint(
            Page0Names, "SomeOtherButton", 0, _ => 1, currentNgLevel: 9, requiredLevel: _ => 1);

        Assert.Equal(string.Empty, hint.Msg1);
        Assert.Equal(string.Empty, hint.Msg2);
    }

    [Fact]
    public void OnlyFirstMatchingIndexIsHandled()
    {
        // 原文 for + Break：只处理第一个匹配项。
        var names = new List<string> { "同名穴", "同名穴", "同名穴", "同名穴", "同名穴" };
        var hint = TStateWindowsMeridians.BuildAcupointHint(
            names, "DBotAcupoints0_1", 0, _ => 0, currentNgLevel: 0, requiredLevel: _ => 7);

        Assert.Equal("同名穴：待打通\\需要内功等级7级", hint.Msg2);
    }

    [Fact]
    public void AcupointIndexDrivesOpenedAndRequiredLookups()
    {
        var openedIdx = -1;
        var reqIdx = -1;

        // 按钮名后缀决定穴位页内序号（DBotAcupoints0_4 → i = 4 → 涌泉穴）。
        // 未打通时才会去读 g_AcupointLevels（已打通分支在读到需求等级前就 Break 了 —— 原文 6334-6336）。
        TStateWindowsMeridians.BuildAcupointHint(
            Page0Names, "DBotAcupoints0_4", 0,
            i => { openedIdx = i; return 0; },
            currentNgLevel: 0,
            i => { reqIdx = i; return 1; });

        Assert.Equal(4, openedIdx);
        Assert.Equal(4, reqIdx);
        Assert.Equal("涌泉穴：已打通", TStateWindowsMeridians.BuildAcupointHint(
            Page0Names, "DBotAcupoints0_4", 0, _ => 1, 0, _ => 1).Msg1);
    }

    [Fact]
    public void OpenedBranchDoesNotQueryRequiredLevel()
    {
        // 差异断言：Acupoints[i] > 0 时原文直接 Break，**不会**访问 g_AcupointLevels。
        bool requiredLevelQueried = false;

        var hint = TStateWindowsMeridians.BuildAcupointHint(
            Page0Names, "DBotAcupoints0_1", 0,
            _ => 1,
            currentNgLevel: 0,
            _ => { requiredLevelQueried = true; return 1; });

        Assert.False(requiredLevelQueried);
        Assert.Equal("二百穴：已打通", hint.Msg1);
    }

    [Fact]
    public void NameIndexAndAcupointIndexAreIndependent()
    {
        // 差异断言：按钮名前缀（NameIndex）与页内序号（i）分别来自 FunA 的两个实参，
        // 名字对不上时即使穴位序号存在也不出提示。
        var wrongPrefix = TStateWindowsMeridians.BuildAcupointHint(
            Page0Names, "DBotAcupoints1_4", 0, _ => 1, 9, _ => 1);
        Assert.Equal(string.Empty, wrongPrefix.Msg1);

        var rightPrefix = TStateWindowsMeridians.BuildAcupointHint(
            Page0Names, "DBotAcupoints1_4", 1, _ => 1, 9, _ => 1);
        Assert.Equal("涌泉穴：已打通", rightPrefix.Msg1);
    }
}
