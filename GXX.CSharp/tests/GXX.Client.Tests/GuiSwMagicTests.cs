using System;
using System.Collections.Generic;
using GXX.Client.GUI.NewStateWin;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 并行批次 P1 切片 B：StateWindows.pas 技能分页与升级按钮（8110-8811 / 8450-8490 / 11625-11703 行）。
/// </summary>
public sealed class GuiSwMagicTests
{
    private sealed class Slot : TStateWindowsMagic.IMagicSlot
    {
        public int MagicId { get; init; }
        public int CanUpgrade { get; init; }
        public int MaxUpgradeLevel { get; init; }
        public int NewLevel { get; init; }
    }

    private static List<TStateWindowsMagic.IMagicSlot> List(params TStateWindowsMagic.IMagicSlot[] items)
        => new List<TStateWindowsMagic.IMagicSlot>(items);

    // ===================== GetCountOnPage（原文 8116-8119） =====================

    [Theory]
    [InlineData(false, 0, 6)]
    [InlineData(false, 1, 6)]
    [InlineData(true, 1, 6)]
    [InlineData(true, 0, 5)]
    public void CountOnPageRequiresBothOldSerialWindowsAndTypeZero(bool oldSerial, int type, int expected)
    {
        Assert.Equal(expected, TStateWindowsMagic.GetCountOnPage(oldSerial, type));
    }

    [Fact]
    public void CountOnPageIgnoresNonZeroStateWindowTypeCompletely()
    {
        // 差异断言：boStateWindowsType 只与 0 比较（其余任何值都等价），不是「<= 0」或「<> 1」。
        Assert.Equal(6, TStateWindowsMagic.GetCountOnPage(true, -1));
        Assert.Equal(6, TStateWindowsMagic.GetCountOnPage(true, 2));
    }

    // ===================== MagicPageChange（原文 8110-8147） =====================

    [Fact]
    public void EmptyListYieldsZeroZero()
    {
        var r = TStateWindowsMagic.MagicPageChange(0, 0, 6);
        Assert.Equal(0, r.Page);
        Assert.Equal(0, r.PageCount);
        Assert.Equal(0, r.Index);
    }

    [Fact]
    public void EmptyListIgnoresNonZeroIndex()
    {
        // 差异断言：Count = 0 时不走 nPage 修正分支，Index 原样返回（不回零）。
        var r = TStateWindowsMagic.MagicPageChange(0, 17, 6);
        Assert.Equal(0, r.Page);
        Assert.Equal(0, r.PageCount);
        Assert.Equal(17, r.Index);
    }

    [Theory]
    [InlineData(1, 0, 1, 1)]
    [InlineData(6, 0, 1, 1)]
    [InlineData(7, 0, 1, 2)]
    [InlineData(12, 0, 1, 2)]
    [InlineData(13, 0, 1, 3)]
    public void PageCountRoundsUp(int count, int index, int expectedPage, int expectedPageCount)
    {
        var r = TStateWindowsMagic.MagicPageChange(count, index, 6);
        Assert.Equal(expectedPage, r.Page);
        Assert.Equal(expectedPageCount, r.PageCount);
    }

    [Theory]
    // nPage := (index + 1) div 6 + 1  →  index 0..4 第 1 页，5..10 第 2 页，11..16 第 3 页
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(4, 1)]
    [InlineData(5, 2)]
    [InlineData(6, 2)]
    [InlineData(7, 2)]
    [InlineData(10, 2)]
    [InlineData(11, 3)]
    [InlineData(12, 3)]
    public void PageIsOneBasedBucketOfIndexPlusOne(int index, int expectedPage)
    {
        var r = TStateWindowsMagic.MagicPageChange(60, index, 6);
        Assert.Equal(expectedPage, r.Page);
    }

    [Fact]
    public void CountOnPageFiveShiftsPageBoundaries()
    {
        // 176 界面：每页 5 个 → (index + 1) div 5 + 1，index 0..3 第 1 页、4..8 第 2 页、9.. 第 3 页。
        Assert.Equal(1, TStateWindowsMagic.MagicPageChange(60, 3, 5).Page);
        Assert.Equal(2, TStateWindowsMagic.MagicPageChange(60, 4, 5).Page);
        Assert.Equal(2, TStateWindowsMagic.MagicPageChange(60, 5, 5).Page);
        Assert.Equal(2, TStateWindowsMagic.MagicPageChange(60, 8, 5).Page);
        Assert.Equal(3, TStateWindowsMagic.MagicPageChange(60, 9, 5).Page);
        var pages = TStateWindowsMagic.MagicPageChange(60, 0, 5);
        Assert.Equal(12, pages.PageCount);
    }

    [Fact]
    public void OverrunClampsPageAndPullsIndexBackByCountOnPage()
    {
        // 原文 8130-8135：列表从 7 缩到 6、MagicIndex 仍是 6（第 2 页）时的重定位。
        var r = TStateWindowsMagic.MagicPageChange(6, 6, 6);
        Assert.Equal(1, r.Page);          // nPage 被夹到 nPageCount
        Assert.Equal(1, r.PageCount);
        Assert.Equal(0, r.Index);        // Dec(MagicIndex, 6) → 0
    }

    [Fact]
    public void OverrunWithZeroIndexKeepsZero()
    {
        // index = 0 时 Dec 不执行，回写仍是 0。
        var r = TStateWindowsMagic.MagicPageChange(1, 0, 6);
        Assert.Equal(1, r.Page);
        Assert.Equal(1, r.PageCount);
        Assert.Equal(0, r.Index);
    }

    [Fact]
    public void OverrunWithCountOnPageFiveFallsBelowZeroAndIsClamped()
    {
        // 差异断言：用 CountOnPage = 5 时 index=2（第 1 页，不超界）不算超界；
        // index=5 且列表只有 5 条时 nPage=2 > nPageCount=1 → Dec 5 → 0。
        var r = TStateWindowsMagic.MagicPageChange(5, 5, 5);
        Assert.Equal(1, r.Page);
        Assert.Equal(1, r.PageCount);
        Assert.Equal(0, r.Index);
    }

    [Fact]
    public void OverrunDropsOnlyOnePageEvenWhenFarOutOfRange()
    {
        // 原文只减一次 CountOnPage（无 while），故 index=100 只降到 94 —— 保留该「不彻底」行为。
        var r = TStateWindowsMagic.MagicPageChange(6, 100, 6);
        Assert.Equal(1, r.Page);
        Assert.Equal(94, r.Index);
    }

    [Fact]
    public void PageCountNeverZeroWhenListNonEmpty()
    {
        // 兜底：nPageCount <= 0 then nPageCount := 1（列表非空时 count div 6 可能是 0）。
        var r = TStateWindowsMagic.MagicPageChange(1, 0, 6);
        Assert.Equal(1, r.PageCount);
    }

    // ===================== FixedSixPageChange（原文 8417-8448 等） =====================

    [Fact]
    public void FixedSixEmptyListYieldsZeroZero()
    {
        var r = TStateWindowsMagic.FixedSixPageChange(0, 3);
        Assert.Equal(0, r.Page);
        Assert.Equal(0, r.PageCount);
        Assert.Equal(3, r.Index);
    }

    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(6, 1, 1)]
    [InlineData(7, 1, 2)]
    [InlineData(60, 1, 10)]
    public void FixedSixPageCount(int count, int index, int expectedPageCount)
    {
        var r = TStateWindowsMagic.FixedSixPageChange(count, index);
        Assert.Equal(1, r.Page);
        Assert.Equal(expectedPageCount, r.PageCount);
    }

    [Fact]
    public void FixedSixClampsOnOverrun()
    {
        var r = TStateWindowsMagic.FixedSixPageChange(6, 6);
        Assert.Equal(1, r.Page);
        Assert.Equal(0, r.Index);
    }

    [Fact]
    public void FixedSixIgnoresOldSerialWindowsConfig()
    {
        // 差异断言：MG/英雄页永远 6，与 MagicPageChange 在 176 界面下的 5 不同。
        var hero = TStateWindowsMagic.FixedSixPageChange(60, 6);
        var self176 = TStateWindowsMagic.MagicPageChange(60, 6, TStateWindowsMagic.GetCountOnPage(true, 0));

        Assert.Equal(10, hero.PageCount);       // 60/6
        Assert.Equal(12, self176.PageCount);    // 60/5
        Assert.Equal(2, hero.Page);
        Assert.Equal(2, self176.Page);
    }

    // ===================== 翻页（原文 8470-8489 / 8450-8468 等） =====================

    [Fact]
    public void PageUpAtZeroDoesNotGoNegative()
    {
        Assert.Equal(0, TStateWindowsMagic.PageUpDown(100, 0, TStateWindowsMagic.PageDirection.Up));
        // index < 6 时 Dec 会产生负数，随后被夹回 0。
        Assert.Equal(0, TStateWindowsMagic.PageUpDown(100, 3, TStateWindowsMagic.PageDirection.Up));
    }

    [Fact]
    public void PageUpStepsBySix()
    {
        // 差异断言：步长恒为 6，即使自己技能页在 176 界面下每页只有 5 个。
        Assert.Equal(6, TStateWindowsMagic.PageUpDown(100, 12, TStateWindowsMagic.PageDirection.Up));
        Assert.Equal(12, TStateWindowsMagic.PageUpDown(100, 18, TStateWindowsMagic.PageDirection.Up));
        // index = 6 时 6 - 6 = 0，不是 1（步长不是 CountOnPage）。
        Assert.Equal(0, TStateWindowsMagic.PageUpDown(100, 6, TStateWindowsMagic.PageDirection.Up));
    }

    [Theory]
    // PageDown: if index + 6 < Count then Inc(index, 6)
    [InlineData(100, 0, 6)]
    [InlineData(100, 93, 99)]     // 93 + 6 = 99 < 100 → 99
    [InlineData(100, 94, 94)]     // 94 + 6 = 100 不 < 100 → 不动（停在 94）
    [InlineData(6, 0, 0)]         // 6 条数据：0 + 6 不 < 6 → 不翻页
    [InlineData(7, 0, 6)]         // 7 条数据：0 + 6 < 7 → 翻到 6（此页只剩 1 个）
    [InlineData(0, 0, 0)]
    public void PageDownUsesStrictLessThan(int count, int index, int expected)
    {
        Assert.Equal(expected, TStateWindowsMagic.PageUpDown(count, index, TStateWindowsMagic.PageDirection.Down));
    }

    [Fact]
    public void PageDownThenPageUpReturnsToSameIndexWhenAligned()
    {
        int after = TStateWindowsMagic.PageUpDown(100, 6, TStateWindowsMagic.PageDirection.Down);
        Assert.Equal(12, after);
        Assert.Equal(6, TStateWindowsMagic.PageUpDown(100, after, TStateWindowsMagic.PageDirection.Up));
    }

    // ===================== RefreshUpgradeButtons（原文 11625-11703） =====================

    [Fact]
    public void EmptyPageLeavesAllButtonsHidden()
    {
        var buttons = TStateWindowsMagic.RefreshUpgradeButtons(List(), 0, 6);

        Assert.Equal(6, buttons.Length);
        foreach (var b in buttons)
        {
            Assert.False(b.Visible);
            Assert.Equal(0, b.Tag);
        }
    }

    [Fact]
    public void VisibleAndEnabledUseDifferentPredicates()
    {
        // CanUpgrade = 2：Visible 为真但 Enabled 为假（判据不同，原文 11657-11658）。
        var can2 = TStateWindowsMagic.EvalUpgradeButton(new Slot { CanUpgrade = 2, MaxUpgradeLevel = 5, NewLevel = 0, MagicId = 7 });
        Assert.True(can2.Visible);
        Assert.False(can2.Enabled);
        Assert.Equal(7, can2.Tag);

        // CanUpgrade = 1：两者皆真。
        var can1 = TStateWindowsMagic.EvalUpgradeButton(new Slot { CanUpgrade = 1, MaxUpgradeLevel = 5, NewLevel = 0, MagicId = 8 });
        Assert.True(can1.Visible);
        Assert.True(can1.Enabled);

        // CanUpgrade = 0：两者皆假。
        var can0 = TStateWindowsMagic.EvalUpgradeButton(new Slot { CanUpgrade = 0, MaxUpgradeLevel = 5, NewLevel = 0, MagicId = 9 });
        Assert.False(can0.Visible);
        Assert.False(can0.Enabled);
    }

    [Fact]
    public void MaxedLevelHidesButStillEnabled()
    {
        // NewLevel < MaxUpgradeLevel 严格小于：满级时 Visible 假，但 Enabled 仍为真（可学下一级）。
        var maxed = TStateWindowsMagic.EvalUpgradeButton(new Slot { CanUpgrade = 1, MaxUpgradeLevel = 3, NewLevel = 3 });
        Assert.False(maxed.Visible);
        Assert.True(maxed.Enabled);

        var over = TStateWindowsMagic.EvalUpgradeButton(new Slot { CanUpgrade = 1, MaxUpgradeLevel = 3, NewLevel = 4 });
        Assert.False(over.Visible);
        Assert.True(over.Enabled);

        var under = TStateWindowsMagic.EvalUpgradeButton(new Slot { CanUpgrade = 1, MaxUpgradeLevel = 3, NewLevel = 2 });
        Assert.True(under.Visible);
    }

    [Fact]
    public void NullSlotsAreSkippedNotThrown()
    {
        var list = List(new Slot { CanUpgrade = 1, MaxUpgradeLevel = 5, NewLevel = 0, MagicId = 11 }, null!,
            new Slot { CanUpgrade = 1, MaxUpgradeLevel = 5, NewLevel = 0, MagicId = 13 });

        var buttons = TStateWindowsMagic.RefreshUpgradeButtons(list, 0, 6);

        Assert.True(buttons[0].Visible);
        Assert.Equal(11, buttons[0].Tag);
        Assert.False(buttons[1].Visible);   // nil 元素跳过：保持清空状态
        Assert.Equal(0, buttons[1].Tag);
        Assert.True(buttons[2].Visible);
        Assert.Equal(13, buttons[2].Tag);
    }

    [Fact]
    public void ButtonsBeyondPageRemainCleared()
    {
        var list = List(
            new Slot { CanUpgrade = 1, MaxUpgradeLevel = 5, NewLevel = 0, MagicId = 1 },
            new Slot { CanUpgrade = 1, MaxUpgradeLevel = 5, NewLevel = 0, MagicId = 2 });

        // CountOnPage = 5 时第 5 个槽位仍有数据（index 4），用 6 时它落到页外。
        var page5 = TStateWindowsMagic.RefreshUpgradeButtons(list, 0, 5);
        var page6 = TStateWindowsMagic.RefreshUpgradeButtons(list, 0, 6);

        Assert.True(page5[0].Visible);
        Assert.True(page5[1].Visible);
        Assert.False(page5[2].Visible);
        Assert.Equal(page5[0].Visible, page6[0].Visible);
        Assert.Equal(page5[1].Visible, page6[1].Visible);
    }

    [Fact]
    public void IndexOffsetMapsListPositionToButtonSlot()
    {
        // MagicIndex = 6：列表第 6 项落在按钮 0。
        var list = List(
            new Slot { MagicId = 0 }, new Slot { MagicId = 1 }, new Slot { MagicId = 2 },
            new Slot { MagicId = 3 }, new Slot { MagicId = 4 }, new Slot { MagicId = 5 },
            new Slot { CanUpgrade = 1, MaxUpgradeLevel = 5, NewLevel = 0, MagicId = 66 });

        var buttons = TStateWindowsMagic.RefreshUpgradeButtons(list, 6, 6);

        Assert.True(buttons[0].Visible);
        Assert.Equal(66, buttons[0].Tag);
        for (int i = 1; i < 6; i++)
            Assert.False(buttons[i].Visible);
    }

    [Fact]
    public void NegativeMagicIndexIsSkippedButDoesNotThrow()
    {
        var list = List(new Slot { CanUpgrade = 1, MaxUpgradeLevel = 5, NewLevel = 0, MagicId = 5 });

        var buttons = TStateWindowsMagic.RefreshUpgradeButtons(list, -3, 6);

        // i 从 -3 起：i=-3..0，只有 i=0 合法 → 落进按钮槽 (0 - (-3)) = 3。
        Assert.True(buttons[3].Visible);
        Assert.Equal(5, buttons[3].Tag);
        Assert.False(buttons[0].Visible);
    }

    [Fact]
    public void EnabledFlagSurvivesIntoUnfilledSlotsLikeOriginal()
    {
        // 原文只清 Visible / Tag，**不清 Enabled**（11647-11648）→ 上一帧的 Enabled 残留。
        var previous = TStateWindowsMagic.RefreshUpgradeButtons(
            List(new Slot { CanUpgrade = 1, MaxUpgradeLevel = 5, NewLevel = 0, MagicId = 1 }), 0, 6);
        Assert.True(previous[0].Enabled);

        var next = TStateWindowsMagic.RefreshUpgradeButtons(List(), 0, 6, previous);

        Assert.False(next[0].Visible);
        Assert.Equal(0, next[0].Tag);
        Assert.True(next[0].Enabled);   // 残留
    }

    [Fact]
    public void ClearUpgradeButtonsWithoutPreviousStartsDisabled()
    {
        var cleared = TStateWindowsMagic.ClearUpgradeButtons();
        foreach (var b in cleared)
        {
            Assert.False(b.Visible);
            Assert.False(b.Enabled);
            Assert.Equal(0, b.Tag);
        }
        Assert.Equal(6, TStateWindowsMagic.UpgradeButtonCount);
        Assert.Equal(6, TStateWindowsMagic.PageStep);
    }

    [Fact]
    public void MouseMoveButtonTagIsAddedToMagicIndex()
    {
        // 原文 8737：Index := DxButton.Tag + MagicNGIndex（把 Tag 当按钮序号用）。
        Assert.Equal(9, TStateWindowsMagic.ButtonTagToListItemIndex(3, 6));
        Assert.Equal(6, TStateWindowsMagic.ButtonTagToListItemIndex(0, 6));
        Assert.Equal(0, TStateWindowsMagic.ButtonTagToListItemIndex(0, 0));
    }
}
