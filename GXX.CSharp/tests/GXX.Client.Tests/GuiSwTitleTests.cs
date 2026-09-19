using System;
using GXX.Client.GUI.NewStateWin;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 并行批次 P1 切片 D：StateWindows.pas 称号分页 + 时装首饰显隐
/// （12187-12206 / 12636-12655 / 13124-13143 / 13145-13206 / 2166-2174 行）。
/// </summary>
public sealed class GuiSwTitleTests
{
    private const int Cv176 = TStateWindowsTitle.TStateWindowsClientVersion.cv176;
    private const int CvSerial = TStateWindowsTitle.TStateWindowsClientVersion.cvSerial;
    private const int CvNewUI205 = TStateWindowsTitle.TStateWindowsClientVersion.cvMirNewUI205;

    // ===================== 版本枚举序号（DxComponents.pas:27） =====================

    [Fact]
    public void ClientVersionOrdinalsMatchEnumDeclaration()
    {
        // TClientVersion = (cv176, cv185, cvHero, cvSerial, cvMirSequel, cvMirNewUI205)
        Assert.Equal(0, TStateWindowsTitle.TStateWindowsClientVersion.cv176);
        Assert.Equal(1, TStateWindowsTitle.TStateWindowsClientVersion.cv185);
        Assert.Equal(2, TStateWindowsTitle.TStateWindowsClientVersion.cvHero);
        Assert.Equal(3, TStateWindowsTitle.TStateWindowsClientVersion.cvSerial);
        Assert.Equal(4, TStateWindowsTitle.TStateWindowsClientVersion.cvMirSequel);
        Assert.Equal(5, TStateWindowsTitle.TStateWindowsClientVersion.cvMirNewUI205);
        // MShare.pas:2324 的初值
        Assert.Equal(CvSerial, TStateWindowsTitle.TStateWindowsClientVersion.Default);
    }

    // ===================== 每页称号数 =====================

    [Theory]
    [InlineData(0, 6)]  // cv176
    [InlineData(1, 6)]  // cv185
    [InlineData(2, 6)]  // cvHero
    [InlineData(3, 6)]  // cvSerial
    [InlineData(4, 6)]  // cvMirSequel
    [InlineData(5, 5)]  // cvMirNewUI205
    public void TitlePageCountIsSixExceptNewUI205(int version, int expected)
    {
        Assert.Equal(expected, TStateWindowsTitle.GetTitlePageCount(version));
    }

    [Fact]
    public void TitlePageCountOnlySpecialCasesNewUI205()
    {
        // 差异断言：判据是 `<> cvMirNewUI205`，所以任何**未知**版本号都是 6。
        Assert.Equal(6, TStateWindowsTitle.GetTitlePageCount(99));
        Assert.Equal(6, TStateWindowsTitle.GetTitlePageCount(-1));
    }

    // ===================== 翻页 =====================

    [Fact]
    public void PageUpAtZeroStaysZero()
    {
        Assert.Equal(0, TStateWindowsTitle.PageClick(100, 0, CvSerial, TStateWindowsTitle.PageDirection.Up));
    }

    [Fact]
    public void PageUpWithinFirstPageClampsToZero()
    {
        // index = 3 < 6 → 3 - 6 = -3 → 夹回 0（原文如此，不是「不动」）。
        Assert.Equal(0, TStateWindowsTitle.PageClick(100, 3, CvSerial, TStateWindowsTitle.PageDirection.Up));
    }

    [Fact]
    public void PageUpStepsByPageCount()
    {
        Assert.Equal(6, TStateWindowsTitle.PageClick(100, 12, CvSerial, TStateWindowsTitle.PageDirection.Up));
        // 205 版步长是 5
        Assert.Equal(5, TStateWindowsTitle.PageClick(100, 10, CvNewUI205, TStateWindowsTitle.PageDirection.Up));
    }

    [Theory]
    // 向下翻页：if index + PageCount < Count then Inc(index, PageCount)
    [InlineData(100, 0, 6)]
    [InlineData(100, 93, 99)]    // 93 + 6 = 99 < 100 → 99
    [InlineData(100, 94, 94)]    // 94 + 6 = 100 不 < 100 → 不动
    [InlineData(6, 0, 0)]        // 6 条：0 + 6 不 < 6 → 不动
    [InlineData(7, 0, 6)]        // 7 条：0 + 6 < 7 → 6
    [InlineData(0, 0, 0)]
    public void PageDownUsesStrictLessThan(int count, int index, int expected)
    {
        Assert.Equal(expected, TStateWindowsTitle.PageClick(count, index, CvSerial, TStateWindowsTitle.PageDirection.Down));
    }

    [Fact]
    public void PageDownWithNewUi205UsesFiveStep()
    {
        Assert.Equal(5, TStateWindowsTitle.PageClick(100, 0, CvNewUI205, TStateWindowsTitle.PageDirection.Down));
        Assert.Equal(0, TStateWindowsTitle.PageClick(5, 0, CvNewUI205, TStateWindowsTitle.PageDirection.Down));
    }

    [Fact]
    public void TitlePagingHasNoOverrunRelocation()
    {
        // 差异断言：与技能分页（MagicPageChange）不同，称号翻页**不**做「越界则回退一页」的修正。
        int idx = TStateWindowsTitle.PageClick(100, 94, CvSerial, TStateWindowsTitle.PageDirection.Down);
        Assert.Equal(94, idx);
    }

    [Fact]
    public void ThreeTitleListsUseIdenticalArithmetic()
    {
        // 原文 12196-12205 / 12645-12654 / 13133-13142 三处逐字相同，仅索引变量/列表不同。
        int self = TStateWindowsTitle.PageClick(20, 0, CvSerial, TStateWindowsTitle.PageDirection.Down);
        int user = TStateWindowsTitle.PageClick(20, 0, CvSerial, TStateWindowsTitle.PageDirection.Down);
        int hero = TStateWindowsTitle.PageClick(20, 0, CvSerial, TStateWindowsTitle.PageDirection.Down);
        Assert.Equal(self, user);
        Assert.Equal(self, hero);
        Assert.Equal(6, self);
    }

    // ===================== 时装首饰显隐 =====================

    [Fact]
    public void FashionStateWhenJewelryOpen()
    {
        var state = TStateWindowsTitle.BuildFashionJewelryState(sex: 0, boFashionJewelryOpen: true);

        Assert.True(state.IsMale);
        Assert.False(state.UseSetting2);   // UseSetting2 = not open = False
        Assert.True(state.JewelryVisible);
    }

    [Fact]
    public void FashionStateWhenJewelryClosed()
    {
        var state = TStateWindowsTitle.BuildFashionJewelryState(sex: 1, boFashionJewelryOpen: false);

        Assert.False(state.IsMale);
        Assert.True(state.UseSetting2);
        Assert.False(state.JewelryVisible);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(1, false)]
    [InlineData(2, false)]
    public void OnlySexZeroIsMale(int sex, bool expectedMale)
    {
        Assert.Equal(expectedMale, TStateWindowsTitle.BuildFashionJewelryState(sex, true).IsMale);
    }

    [Fact]
    public void UserStateFashionGenderComesFromFeatureByte()
    {
        Assert.True(TStateWindowsTitle.IsUserStateFashionMale(0));
        Assert.False(TStateWindowsTitle.IsUserStateFashionMale(1));
    }

    [Fact]
    public void RefreshDispatchIsNotElseIf()
    {
        // 差异断言：13147-13154 是三个独立 if，三个窗口同时可见时三个都会刷新。
        var all = TStateWindowsTitle.RefreshDispatch(true, true, true);
        Assert.Equal((true, true, true), all);

        var none = TStateWindowsTitle.RefreshDispatch(false, false, false);
        Assert.Equal((false, false, false), none);

        var mixed = TStateWindowsTitle.RefreshDispatch(true, false, true);
        Assert.Equal((true, false, true), mixed);
    }

    // ===================== FLableCaption_BigGold 提取（原文 2166-2174） =====================

    [Fact]
    public void BigGoldPrefixKeepsNonDigitCharItself()
    {
        // 「金币」后跟数字：从尾部找到 '币'（index=2，1-based）→ Copy(s,1,2) = "金币"
        Assert.Equal("金币", TStateWindowsTitle.ExtractBigGoldPrefix("金币123"));
    }

    [Fact]
    public void BigGoldPrefixKeepsAllLetters()
    {
        // 「金币abc」：'c' 在 index=5（1-based，串长 5）→ Copy 整串。
        Assert.Equal("金币abc", TStateWindowsTitle.ExtractBigGoldPrefix("金币abc"));
        // 差异断言：「金币abc1」最后一位是数字，向上找到 'c'（index=5）→ Copy(s,1,5) = "金币abc"（末尾 '1' 被去掉）。
        Assert.Equal("金币abc", TStateWindowsTitle.ExtractBigGoldPrefix("金币abc1"));
        Assert.Equal("金币abc", TStateWindowsTitle.ExtractBigGoldPrefix("金币abc12"));
    }

    [Fact]
    public void BigGoldAllDigitsKeepsWholeString()
    {
        // 原文如此：整串都是数字时循环不 Break → 保持原串。
        Assert.Equal("123456", TStateWindowsTitle.ExtractBigGoldPrefix("123456"));
    }

    [Fact]
    public void BigGoldTrimsFirst()
    {
        // Trim 之后从尾部找非数字：'  金币1  ' → '金币1' → '币' 在 index=2 → "金币"
        Assert.Equal("金币", TStateWindowsTitle.ExtractBigGoldPrefix("  金币1  "));
        Assert.Equal(string.Empty, TStateWindowsTitle.ExtractBigGoldPrefix(""));
        Assert.Equal(string.Empty, TStateWindowsTitle.ExtractBigGoldPrefix("      "));
        Assert.Equal(string.Empty, TStateWindowsTitle.ExtractBigGoldPrefix(null));
    }

    [Fact]
    public void BigGoldSingleNonDigitCharIsKept()
    {
        // 单个非数字字符：index 从 1 递减到 1，Copy(s,1,1) = 该字符。
        Assert.Equal("金", TStateWindowsTitle.ExtractBigGoldPrefix("金"));
    }

    [Fact]
    public void BigGoldKeepsDigitImmediatelyBeforeSeparator()
    {
        // 「金币1abc」：尾部 'c'（index=6，1-based）→ Copy(s,1,6) = 整串。
        Assert.Equal("金币1abc", TStateWindowsTitle.ExtractBigGoldPrefix("金币1abc"));
        // 「金币12X」：'X' 在 index=5 → 整串
        Assert.Equal("金币12X", TStateWindowsTitle.ExtractBigGoldPrefix("金币12X"));
    }
}
