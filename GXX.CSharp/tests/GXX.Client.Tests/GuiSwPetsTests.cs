using System;
using GXX.Client.GUI.NewStateWin;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 并行批次 P1 切片 E：StateWindows.pas 宠物包裹网格（13208-13297 / 13692-13864 行）。
/// </summary>
public sealed class GuiSwPetsTests
{
    private const int MaxPetBag = 30;

    // ===================== 常量 =====================

    [Fact]
    public void MaxGamePetBagCountMatchesGrobal2()
    {
        Assert.Equal(30, TStateWindowsPets.MaxGamePetBagCount);
        Assert.Equal(MaxPetBag, TStateWindowsPets.MaxGamePetBagCount);
    }

    // ===================== 下标换算 =====================

    [Theory]
    // nIdx := ACol + ARow * ColCount
    [InlineData(0, 0, 6, 0)]
    [InlineData(5, 0, 6, 5)]
    [InlineData(0, 1, 6, 6)]
    [InlineData(1, 1, 6, 7)]
    [InlineData(5, 4, 6, 29)]
    [InlineData(2, 3, 5, 17)]
    public void GridIndexIsRowMajor(int col, int row, int colCount, int expected)
    {
        Assert.Equal(expected, TStateWindowsPets.GridIndex(col, row, colCount));
    }

    [Fact]
    public void GridIndexTreatsColCountZeroAsAllZero()
    {
        // 差异断言：ColCount = 0 时不除零、不抛异常，行号被乘成 0（原文同样不会崩）。
        Assert.Equal(3, TStateWindowsPets.GridIndex(3, 9, 0));
    }

    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, true)]
    [InlineData(29, true)]
    [InlineData(30, false)]
    [InlineData(31, false)]
    public void ValidIndexIsInclusiveZeroTo29(int idx, bool expected)
    {
        Assert.Equal(expected, TStateWindowsPets.IsValidPetBagIndex(idx));
    }

    [Fact]
    public void GridBoundsUsesGridCellSizeNotRectSize()
    {
        // 原文 13750：Bounds(Rect.Left, Rect.Top, DItemGrid.ColWidth, DItemGrid.RowHeight)
        var r = TStateWindowsPets.GridCellBounds(100, 200, 32, 32);
        Assert.Equal((100, 200, 132, 232), r);

        // 即使传入的 Rect 自称很大也不影响（它只提供左上角）。
        var r2 = TStateWindowsPets.GridCellBounds(0, 0, 40, 24);
        Assert.Equal((0, 0, 40, 24), r2);
    }

    [Fact]
    public void MouseItemInfoPointUsesRowPlusOne()
    {
        // 原文 13849-13851：x = Left + ACol * ColWidth；y = Top + (ARow + 1) * RowHeight
        var p = TStateWindowsPets.MouseItemInfoPoint(10, 20, 2, 3, 32, 32);
        Assert.Equal(10 + 64, p.X);
        Assert.Equal(20 + 4 * 32, p.Y);
    }

    [Fact]
    public void MouseItemInfoPointRowZeroIsOneRowDown()
    {
        // 差异断言：第 0 行的提示 y 是 Top + RowHeight（不是 Top）。
        var p = TStateWindowsPets.MouseItemInfoPoint(0, 0, 0, 0, 32, 32);
        Assert.Equal(0, p.X);
        Assert.Equal(32, p.Y);
    }

    // ===================== 网格选择分支 =====================

    private static TStateWindowsPets.SelectAction Select(
        int col = 0, int row = 0, int colCount = 6,
        bool rightButton = false, bool itemMoving = false, bool targetHasItem = false,
        TStateWindowsPets.MovingItemType movingType = TStateWindowsPets.MovingItemType.Other,
        bool movingIndexEquals = false, bool isOverLap = false, bool waitingEmpty = true)
        => TStateWindowsPets.ClassifyGridSelect(col, row, colCount, rightButton, itemMoving,
            targetHasItem, movingType, movingIndexEquals, isOverLap, waitingEmpty);

    [Fact]
    public void OutOfRangeIndexDoesNothing()
    {
        Assert.Equal(TStateWindowsPets.SelectAction.None, Select(col: 0, row: 5, colCount: 6));   // idx = 30
        Assert.Equal(TStateWindowsPets.SelectAction.None, Select(col: 0, row: 9, colCount: 6));
    }

    [Fact]
    public void RightButtonExitsEarlyEvenWithItem()
    {
        Assert.Equal(TStateWindowsPets.SelectAction.None, Select(rightButton: true, targetHasItem: true));
    }

    [Fact]
    public void NotMovingAndHasItemPicksUp()
    {
        Assert.Equal(TStateWindowsPets.SelectAction.PickUp, Select(targetHasItem: true));
    }

    [Fact]
    public void NotMovingAndEmptyDoesNothing()
    {
        Assert.Equal(TStateWindowsPets.SelectAction.None, Select(targetHasItem: false));
    }

    [Fact]
    public void OtherMovingTypeIsRejected()
    {
        // 原文 13785：只有 mtBagItem / mtGamePetBagItem 被接受。
        Assert.Equal(TStateWindowsPets.SelectAction.None,
            Select(itemMoving: true, targetHasItem: true, movingType: TStateWindowsPets.MovingItemType.Other));
    }

    [Fact]
    public void BagItemGoesToPetBagWhenWaitingSlotEmpty()
    {
        Assert.Equal(TStateWindowsPets.SelectAction.BagToPetBag,
            Select(itemMoving: true, targetHasItem: false,
                movingType: TStateWindowsPets.MovingItemType.BagItem, waitingEmpty: true));
    }

    [Fact]
    public void BagItemIsIgnoredWhenWaitingSlotBusy()
    {
        // 原文 13788：g_WaitingUseItem.Item.s.Name = '' 不成立就什么都不做（不 Swap、不 Place）。
        Assert.Equal(TStateWindowsPets.SelectAction.None,
            Select(itemMoving: true, targetHasItem: true,
                movingType: TStateWindowsPets.MovingItemType.BagItem, waitingEmpty: false));
    }

    [Fact]
    public void BagItemIgnoresTargetOccupancy()
    {
        // 差异断言：mtBagItem 分支不看目标格是否有物品（与 mtGamePetBagItem 分支不同）。
        Assert.Equal(TStateWindowsPets.SelectAction.BagToPetBag,
            Select(itemMoving: true, targetHasItem: true,
                movingType: TStateWindowsPets.MovingItemType.BagItem, waitingEmpty: true));
    }

    [Fact]
    public void PetBagItemOverlapsWhenSameKindAndWaitingEmpty()
    {
        Assert.Equal(TStateWindowsPets.SelectAction.Overlap,
            Select(itemMoving: true, targetHasItem: true,
                movingType: TStateWindowsPets.MovingItemType.GamePetBagItem, isOverLap: true));
    }

    [Fact]
    public void PetBagOverlapWithBusyWaitingSlotIsStillClassifiedAsOverlap()
    {
        // 差异断言：原文 13802 的 `if g_WaitingUseItem.Item.s.Name = ''` 只挡 SendPetOverLapItem
        // 这个**副作用**，并没有 else 分支 —— 所以分类结果仍是 Overlap（不是 Swap、不是 None）。
        Assert.Equal(TStateWindowsPets.SelectAction.Overlap,
            Select(itemMoving: true, targetHasItem: true,
                movingType: TStateWindowsPets.MovingItemType.GamePetBagItem,
                isOverLap: true, waitingEmpty: false));

        // 细分层才能观察到 gating 生效：
        Assert.Equal(TStateWindowsPets.OverlapKind.SuppressedByWaitingItem,
            TStateWindowsPets.ClassifyOverlap(true, TStateWindowsPets.MovingItemType.GamePetBagItem,
                isOverLap: true, waitingUseItemEmpty: false));
        Assert.Equal(TStateWindowsPets.OverlapKind.SendOverlap,
            TStateWindowsPets.ClassifyOverlap(true, TStateWindowsPets.MovingItemType.GamePetBagItem,
                isOverLap: true, waitingUseItemEmpty: true));
    }

    [Fact]
    public void PetBagItemSwapsWhenNotOverlappable()
    {
        Assert.Equal(TStateWindowsPets.SelectAction.Swap,
            Select(itemMoving: true, targetHasItem: true,
                movingType: TStateWindowsPets.MovingItemType.GamePetBagItem, isOverLap: false));
    }

    [Fact]
    public void PetBagItemPlacesIntoEmptyCellIgnoringWaitingSlot()
    {
        // 差异断言：目标格为空时走 Place，**完全不看** g_WaitingUseItem。
        Assert.Equal(TStateWindowsPets.SelectAction.Place,
            Select(itemMoving: true, targetHasItem: false,
                movingType: TStateWindowsPets.MovingItemType.GamePetBagItem, waitingEmpty: false));
    }

    [Fact]
    public void MovingIndexEqualityIsNotUsedForBranching()
    {
        // 原文如此：13728 的 (g_MovingItem.Index = idx) 只出现在**双击**流程里，
        // Select 流程不看它 —— 传 true/false 结果必须一致。
        Assert.Equal(
            Select(itemMoving: true, targetHasItem: true,
                movingType: TStateWindowsPets.MovingItemType.GamePetBagItem, movingIndexEquals: true),
            Select(itemMoving: true, targetHasItem: true,
                movingType: TStateWindowsPets.MovingItemType.GamePetBagItem, movingIndexEquals: false));
    }

    // ===================== 双击用物品判据 =====================

    [Theory]
    [InlineData(0, 0, 0, true)]
    [InlineData(31, 0, 0, true)]
    [InlineData(93, 0, 0, true)]
    [InlineData(49, 10, 10, true)]    // 49 且 Dura >= DuraMax
    [InlineData(49, 9, 10, false)]    // 49 但耐久不足
    [InlineData(49, 0, 0, true)]      // 0 >= 0
    [InlineData(1, 0, 0, false)]
    [InlineData(5, 99, 1, false)]
    public void IsUsablePetItemMatchesOriginalPredicate(int stdMode, int dura, int duraMax, bool expected)
    {
        Assert.Equal(expected, TStateWindowsPets.IsUsablePetItem(stdMode, dura, duraMax));
    }

    [Fact]
    public void DuraMaxIsIgnoredForStdModeZero31And93()
    {
        // 差异断言：0/31/93 不看耐久（DuraMax = 0 也成立）。
        Assert.True(TStateWindowsPets.IsUsablePetItem(0, -5, 0));
        Assert.True(TStateWindowsPets.IsUsablePetItem(31, -5, 0));
        Assert.True(TStateWindowsPets.IsUsablePetItem(93, -5, 0));
    }

    // ===================== Ctrl 键判定 =====================

    [Fact]
    public void ControlDownIsExactlyEighty()
    {
        Assert.True(TStateWindowsPets.IsControlDown(0x80));
        Assert.False(TStateWindowsPets.IsControlDown(0x00));
        Assert.False(TStateWindowsPets.IsControlDown(0x01));   // 只判最高位等于 $80，不做掩码
        Assert.False(TStateWindowsPets.IsControlDown(0x81));
    }

    // ===================== 鼠标移动重定向 =====================

    [Fact]
    public void RightDragRedirectsToSelectWithLeftButton()
    {
        Assert.True(TStateWindowsPets.ShouldRedirectToSelectOnMouseMove(shiftHasRight: true, itemMoving: true));
        Assert.False(TStateWindowsPets.ShouldRedirectToSelectOnMouseMove(true, false));
        Assert.False(TStateWindowsPets.ShouldRedirectToSelectOnMouseMove(false, true));
        Assert.False(TStateWindowsPets.ShouldRedirectToSelectOnMouseMove(false, false));
    }

    // ===================== 悬浮提示路径 =====================

    [Fact]
    public void SuspensionShowItemConfigForcesMouseInfo()
    {
        Assert.True(TStateWindowsPets.ShouldShowMouseItemInfo(btSuspensionShowItem: 1, clientVersion: 0));
        Assert.True(TStateWindowsPets.ShouldShowMouseItemInfo(2, 0));
    }

    [Fact]
    public void VersionGreaterThanSerialForcesMouseInfo()
    {
        // cvSerial = 3；严格大于才是新路径。
        Assert.False(TStateWindowsPets.ShouldShowMouseItemInfo(0, TStateWindowsTitle.TStateWindowsClientVersion.cvSerial));
        Assert.True(TStateWindowsPets.ShouldShowMouseItemInfo(0, TStateWindowsTitle.TStateWindowsClientVersion.cvMirSequel));
        Assert.True(TStateWindowsPets.ShouldShowMouseItemInfo(0, TStateWindowsTitle.TStateWindowsClientVersion.cvMirNewUI205));
        Assert.False(TStateWindowsPets.ShouldShowMouseItemInfo(0, TStateWindowsTitle.TStateWindowsClientVersion.cv176));
    }

    [Fact]
    public void NegativeSuspensionSettingFallsBackToVersionCheck()
    {
        Assert.False(TStateWindowsPets.ShouldShowMouseItemInfo(-1, 0));
        Assert.True(TStateWindowsPets.ShouldShowMouseItemInfo(-1, 4));
    }
}
