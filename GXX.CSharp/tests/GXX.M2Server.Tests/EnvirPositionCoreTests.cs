using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J172：`TEnvirnoment` 位置推算与安全判定 1:1 测试。
/// **八个方向的单向越界保护用逐点枚举固证、
/// 倒序扫描与正序结果一致性用穷举、三乘三邻域用二十五行枚举。**
/// </summary>
public sealed class EnvirPositionCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Directions()
    {
        Assert.True(EnvirPositionCore.EightDirections());
        Assert.True(EnvirPositionCore.EightDirectionEntries());
        Assert.True(EnvirPositionCore.CardinalAreEven());
        Assert.Equal(8, EnvirPositionCore.Directions.Length);
    }

    [Fact]
    public void DirectionValues()
    {
        Assert.Equal(0, EnvirPositionCore.DrUp);
        Assert.Equal(1, EnvirPositionCore.DrUpRight);
        Assert.Equal(2, EnvirPositionCore.DrRight);
        Assert.Equal(3, EnvirPositionCore.DrDownRight);
        Assert.Equal(4, EnvirPositionCore.DrDown);
        Assert.Equal(5, EnvirPositionCore.DrDownLeft);
        Assert.Equal(6, EnvirPositionCore.DrLeft);
        Assert.Equal(7, EnvirPositionCore.DrUpLeft);
        Assert.Equal(3, EnvirPositionCore.ObjEvent);
    }

    // ===================== 一、GetNextPosition =====================

    [Fact]
    public void ResultSemantics()
    {
        Assert.True(EnvirPositionCore.ResultIsMovedOrNot());
        Assert.True(EnvirPositionCore.BlockedReasonNotReported());
        Assert.True(EnvirPositionCore.OutputsStartAsInput());
    }

    [Fact]
    public void AllEightOffsets()
    {
        // **八个方向的位移都正确**
        Assert.True(EnvirPositionCore.AllEightOffsets());

        Assert.Equal((true, 50, 47), EnvirPositionCore.NextPosition(50, 50, 0, 3, 100, 100));
        Assert.Equal((true, 50, 53), EnvirPositionCore.NextPosition(50, 50, 4, 3, 100, 100));
        Assert.Equal((true, 47, 50), EnvirPositionCore.NextPosition(50, 50, 6, 3, 100, 100));
        Assert.Equal((true, 53, 50), EnvirPositionCore.NextPosition(50, 50, 2, 3, 100, 100));
        Assert.Equal((true, 47, 47), EnvirPositionCore.NextPosition(50, 50, 7, 3, 100, 100));
        Assert.Equal((true, 53, 47), EnvirPositionCore.NextPosition(50, 50, 1, 3, 100, 100));
        Assert.Equal((true, 47, 53), EnvirPositionCore.NextPosition(50, 50, 5, 3, 100, 100));
        Assert.Equal((true, 53, 53), EnvirPositionCore.NextPosition(50, 50, 3, 3, 100, 100));
    }

    [Fact]
    public void AxesCorrect()
    {
        Assert.True(EnvirPositionCore.UpDownUseHeight());
        Assert.True(EnvirPositionCore.LeftRightUseWidth());
        Assert.True(EnvirPositionCore.FourDiagonalsCorrectAxes());
        Assert.True(EnvirPositionCore.AxesCorrectButGuardsOneSided());
        Assert.True(EnvirPositionCore.NotFullyClean());
    }

    [Fact]
    public void NoAxisMixup()
    {
        // **八个方向的边界只受自己那条轴影响**
        Assert.True(EnvirPositionCore.UpIgnoresWidth());
        Assert.True(EnvirPositionCore.DownIgnoresWidth());
        Assert.True(EnvirPositionCore.LeftIgnoresHeight());
        Assert.True(EnvirPositionCore.RightIgnoresHeight());
        Assert.True(EnvirPositionCore.NoAxisMixup());
    }

    // ---------- 单向越界保护 ----------

    [Fact]
    public void OneSidedGuards()
    {
        // **上方向无上界、下方向无下界**
        Assert.True(EnvirPositionCore.UpHasNoUpperBound());
        Assert.True(EnvirPositionCore.DownHasNoLowerBound());
        Assert.True(EnvirPositionCore.OneSidedGuardPerAxis());
        Assert.True(EnvirPositionCore.RangeExhaustive());
    }

    [Fact]
    public void UpOnlyDependsOnStep()
    {
        Assert.True(EnvirPositionCore.UpDependsOnlyOnStep());
        Assert.True(EnvirPositionCore.UpExceedsMapHeight());

        // 高度十、纵坐标五十仍能向上走
        var r = EnvirPositionCore.NextPosition(5, 50, 0, 1, 10, 10);
        Assert.True(r.Moved);
        Assert.Equal(49, r.Y);
    }

    [Fact]
    public void DownOnlyDependsOnSize()
    {
        Assert.True(EnvirPositionCore.DownIgnoresStepForLowerEnd());
        Assert.True(EnvirPositionCore.DownAtTopStillMoves());

        // 纵坐标为零时任何步长都能向下
        var r = EnvirPositionCore.NextPosition(5, 0, 4, 1, 100, 100);
        Assert.True(r.Moved);
        Assert.Equal(1, r.Y);
    }

    [Fact]
    public void LowerBoundInclusive()
    {
        // **上方向判据是"大于步长减一" → 下界含**
        Assert.True(EnvirPositionCore.LowerBoundInclusive());
        Assert.True(EnvirPositionCore.BelowLowerBoundBlocked());

        Assert.True(EnvirPositionCore.NextPosition(5, 2, 0, 2, 100, 100).Moved);
        Assert.False(EnvirPositionCore.NextPosition(5, 1, 0, 2, 100, 100).Moved);
    }

    [Fact]
    public void UpperBoundExclusive()
    {
        // **下方向判据是严格小于 → 上界不含**
        Assert.True(EnvirPositionCore.UpperBoundExclusive());
        Assert.True(EnvirPositionCore.AboveUpperBoundBlocked());
        Assert.True(EnvirPositionCore.MaxResultIsSizeMinusOne());

        Assert.False(EnvirPositionCore.NextPosition(5, 98, 4, 2, 100, 100).Moved);
        var r = EnvirPositionCore.NextPosition(5, 97, 4, 2, 100, 100);
        Assert.True(r.Moved);
        Assert.Equal(99, r.Y);
    }

    [Fact]
    public void ZeroStep()
    {
        // **步长为零时八个方向全假（坐标没动）**
        Assert.True(EnvirPositionCore.ZeroStepAlwaysFalse());
    }

    [Fact]
    public void NegativeStep()
    {
        // **负步长会反向但仍返回真**
        Assert.True(EnvirPositionCore.NegativeStepReverses());
        Assert.True(EnvirPositionCore.ReturnsTrueButReversed());

        var r = EnvirPositionCore.NextPosition(50, 50, 0, -3, 100, 100);
        Assert.True(r.Moved);
        Assert.Equal(53, r.Y);
    }

    [Fact]
    public void BoundaryBehaviour()
    {
        Assert.True(EnvirPositionCore.NoClamping());
        Assert.True(EnvirPositionCore.StaysUnchangedOnBoundary());

        var r = EnvirPositionCore.NextPosition(50, 0, 0, 3, 100, 100);
        Assert.False(r.Moved);
        Assert.Equal(50, r.X);
        Assert.Equal(0, r.Y);
    }

    [Fact]
    public void InvalidDirections()
    {
        Assert.True(EnvirPositionCore.NoElseBranch());
        Assert.True(EnvirPositionCore.InvalidDirectionSilentFail());
    }

    // ===================== 二、CanSafeWalk =====================

    [Fact]
    public void BackwardIteration()
    {
        Assert.True(EnvirPositionCore.IteratesBackwards());
        Assert.True(EnvirPositionCore.OnlyBackwardScanner());
        Assert.True(EnvirPositionCore.OthersForward());
    }

    [Fact]
    public void SafeWalkCases()
    {
        Assert.True(EnvirPositionCore.DamagingEventMakesUnsafe());
        Assert.True(EnvirPositionCore.ZeroDamageEventSafe());
        Assert.True(EnvirPositionCore.NonEventIgnored());
        Assert.True(EnvirPositionCore.DefaultTrue());
        Assert.True(EnvirPositionCore.EmptyCellIsSafe());
        Assert.True(EnvirPositionCore.InvalidCellIsSafe());
    }

    [Fact]
    public void SafeWalkModel()
    {
        Assert.False(EnvirPositionCore.CanSafeWalk(new List<(int, int)> { (3, 10) }));
        Assert.True(EnvirPositionCore.CanSafeWalk(new List<(int, int)> { (3, 0) }));
        Assert.True(EnvirPositionCore.CanSafeWalk(new List<(int, int)> { (1, 9999) }));
        Assert.True(EnvirPositionCore.CanSafeWalk(new List<(int, int)>()));
    }

    [Fact]
    public void OrderIndependence()
    {
        // **结果被覆盖但单调 —— 只要存在带伤事件即为假**
        Assert.True(EnvirPositionCore.ResultOverwrittenButMonotone());
        Assert.True(EnvirPositionCore.OrderIndependent());
        Assert.True(EnvirPositionCore.AnyDamagingWins());
        Assert.True(EnvirPositionCore.BackwardMatchesForward());
    }

    [Fact]
    public void SafeWalkSemantics()
    {
        // **它只看事件伤害、不看通行标志与可收集标志**
        Assert.True(EnvirPositionCore.IgnoresChFlag());
        Assert.True(EnvirPositionCore.IgnoresBo2B9());
        Assert.True(EnvirPositionCore.AboutTrapsNotBlocking());
        Assert.True(EnvirPositionCore.HasLock32());
    }

    [Fact]
    public void DamageSemantics()
    {
        Assert.True(EnvirPositionCore.DamageIsDamageAmount());
        Assert.Equal(4, EnvirPositionCore.DamageAssignmentCount());
        Assert.True(EnvirPositionCore.FourAssignments());
        Assert.True(EnvirPositionCore.ZeroMeansCleared());
    }

    [Fact]
    public void SameTypeDifferentQuestion()
    {
        Assert.True(EnvirPositionCore.SameTypeCheckDifferentQuestion());
        Assert.True(EnvirPositionCore.TwoContrastEntries());
        Assert.Equal(2, EnvirPositionCore.EventCheckContrast.Length);
    }

    // ===================== 三、ArroundDoorOpened =====================

    [Fact]
    public void DoorBasics()
    {
        Assert.True(EnvirPositionCore.ScansGlobalDoorList());
        Assert.True(EnvirPositionCore.ThreeByThreeNeighbourhood());
        Assert.True(EnvirPositionCore.IncludesCenterCell());
        Assert.True(EnvirPositionCore.EmptyDoorListIsTrue());
    }

    [Fact]
    public void DoorCases()
    {
        Assert.True(EnvirPositionCore.ClosedDoorMakesFalse());
        Assert.True(EnvirPositionCore.OpenDoorContinues());
        Assert.True(EnvirPositionCore.AnyClosedDoorWins());
    }

    [Fact]
    public void DoorBoundaries()
    {
        // **差一算在邻域内、差二不算**
        Assert.True(EnvirPositionCore.DifferenceOfOneCounts());
        Assert.True(EnvirPositionCore.DifferenceOfTwoDoesNot());
        Assert.True(EnvirPositionCore.NeighbourhoodExhaustive());
    }

    [Fact]
    public void DoorModel()
    {
        Assert.False(EnvirPositionCore.ArroundDoorOpened(
            new List<(int, int, bool)> { (5, 5, false) }, 5, 5));
        Assert.True(EnvirPositionCore.ArroundDoorOpened(
            new List<(int, int, bool)> { (5, 5, true) }, 5, 5));
        Assert.False(EnvirPositionCore.ArroundDoorOpened(
            new List<(int, int, bool)> { (4, 4, true), (6, 6, false) }, 5, 5));
        Assert.True(EnvirPositionCore.ArroundDoorOpened(
            new List<(int, int, bool)> { (7, 5, false) }, 5, 5));
    }

    [Fact]
    public void ExceptionHandling()
    {
        Assert.True(EnvirPositionCore.LocalResourcestring());
        Assert.True(EnvirPositionCore.SwallowsException());
        Assert.True(EnvirPositionCore.NoReraiseNoReset());
        Assert.True(EnvirPositionCore.NondeterministicOnException());
        Assert.True(EnvirPositionCore.ExceptionIsDefensive());
    }

    [Fact]
    public void ExceptionMessage()
    {
        // **末尾有一个空格**
        Assert.True(EnvirPositionCore.MessageHasTrailingSpace());
        Assert.True(EnvirPositionCore.MessageShape());
        Assert.EndsWith(" ", EnvirPositionCore.ExceptionMessage, StringComparison.Ordinal);
        Assert.StartsWith("[Exception]", EnvirPositionCore.ExceptionMessage, StringComparison.Ordinal);
    }

    [Fact]
    public void TryExceptPlacement()
    {
        // **只有邻域判定包了 try-except**
        Assert.True(EnvirPositionCore.OnlyThisHasTryExcept());
        Assert.Equal(1, EnvirPositionCore.TryExceptCount());
        Assert.True(EnvirPositionCore.OneTryExcept());
        Assert.Equal(3, EnvirPositionCore.TryTable.Length);

        Assert.False(EnvirPositionCore.TryTable[0].HasTryExcept);
        Assert.False(EnvirPositionCore.TryTable[1].HasTryExcept);
        Assert.True(EnvirPositionCore.TryTable[2].HasTryExcept);
    }

    [Fact]
    public void LockAsymmetry()
    {
        Assert.True(EnvirPositionCore.NoLock());
        Assert.True(EnvirPositionCore.LockAsymmetry());
    }

    // ===================== 行数 =====================

    [Fact]
    public void LineCounts()
    {
        Assert.True(EnvirPositionCore.ThreeMethods());
        Assert.Equal(3, EnvirPositionCore.MethodLineCounts.Length);
        Assert.Equal(new[] { 56, 31, 25 }, EnvirPositionCore.MethodLineCounts);
    }

    [Fact]
    public void RelativeLengths()
    {
        Assert.True(EnvirPositionCore.NextPositionIsLongest());
        Assert.True(EnvirPositionCore.NeighbourhoodIsShortest());
    }

    [Fact]
    public void Totals()
    {
        Assert.True(EnvirPositionCore.TotalLinesValues());
        Assert.Equal(112, EnvirPositionCore.TotalLines());
        Assert.True(EnvirPositionCore.NextPositionIsHalf());
        Assert.True(EnvirPositionCore.ShareValues());
    }
}
