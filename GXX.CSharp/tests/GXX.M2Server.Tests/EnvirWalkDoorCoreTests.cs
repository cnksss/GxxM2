using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J161：`TEnvirnoment` 行走与门相关判定族 1:1 测试。
/// **`CanFly` 的"位置永不前进"缺陷用逐轮打印检查点验证、
/// 八方向守卫用全网格等价性枚举验证、取整用银行家舍入实测。**
/// </summary>
public sealed class EnvirWalkDoorCoreTests
{
    // ===================== 常量与锁 =====================

    [Fact]
    public void CellLockIds()
    {
        Assert.True(EnvirWalkDoorCore.CellLockIds32And46());
        Assert.Equal(new[] { 32, 46 }, EnvirWalkDoorCore.CellLockIds);
    }

    [Fact]
    public void LockPresenceMatrix()
    {
        // **七个方法里只有两个加锁**
        Assert.True(EnvirWalkDoorCore.LockPresenceMatrix());
        Assert.True(EnvirWalkDoorCore.DoorListReadsUnlocked());
        Assert.True(EnvirWalkDoorCore.NoLockInSetter());
        Assert.Equal(7, EnvirWalkDoorCore.LockPresence.Length);
    }

    // ===================== 一、GetNextPosition =====================

    [Fact]
    public void ResultSemantics()
    {
        // **返回"位置真的变了"而不是"方向合法"**
        Assert.True(EnvirWalkDoorCore.OutputInitializedToInput());
        Assert.True(EnvirWalkDoorCore.ResultMeansMovedNotValid());
        Assert.True(EnvirWalkDoorCore.UnknownDirectionReturnsFalse());
    }

    [Fact]
    public void EightDirections()
    {
        Assert.True(EnvirWalkDoorCore.EightDirections());
    }

    [Fact]
    public void DirectionConstants()
    {
        Assert.Equal(0, EnvirWalkDoorCore.DR_UP);
        Assert.Equal(1, EnvirWalkDoorCore.DR_UPRIGHT);
        Assert.Equal(2, EnvirWalkDoorCore.DR_RIGHT);
        Assert.Equal(3, EnvirWalkDoorCore.DR_DOWNRIGHT);
        Assert.Equal(4, EnvirWalkDoorCore.DR_DOWN);
        Assert.Equal(5, EnvirWalkDoorCore.DR_DOWNLEFT);
        Assert.Equal(6, EnvirWalkDoorCore.DR_LEFT);
        Assert.Equal(7, EnvirWalkDoorCore.DR_UPLEFT);
    }

    [Fact]
    public void GuardTemplates()
    {
        Assert.True(EnvirWalkDoorCore.FourGuardTemplatesCount());
        Assert.True(EnvirWalkDoorCore.AsymmetricGuardShapes());
        Assert.Equal(4, EnvirWalkDoorCore.FourGuardTemplates.Length);
    }

    [Fact]
    public void EquivalentToInBounds()
    {
        // **四套守卫全部等价于"结果落在界内"**
        Assert.True(EnvirWalkDoorCore.EquivalentToInBounds());
        Assert.True(EnvirWalkDoorCore.EquivalentForAnyStep());
    }

    [Fact]
    public void BoundaryValues()
    {
        Assert.True(EnvirWalkDoorCore.BoundaryValuesUp());
        Assert.True(EnvirWalkDoorCore.BoundaryValuesDown());
        Assert.True(EnvirWalkDoorCore.BoundaryValuesLeftRight());
    }

    [Fact]
    public void BoundaryExact()
    {
        // **y=1 可上、y=0 被拦；y=8 可下、y=9 被拦**
        var (_, y1, m1) = EnvirWalkDoorCore.NextPosition(5, 1, EnvirWalkDoorCore.DR_UP, 1, 10, 10);
        var (_, y0, m0) = EnvirWalkDoorCore.NextPosition(5, 0, EnvirWalkDoorCore.DR_UP, 1, 10, 10);
        var (_, y8, m8) = EnvirWalkDoorCore.NextPosition(5, 8, EnvirWalkDoorCore.DR_DOWN, 1, 10, 10);
        var (_, y9, m9) = EnvirWalkDoorCore.NextPosition(5, 9, EnvirWalkDoorCore.DR_DOWN, 1, 10, 10);

        Assert.Equal(0, y1);
        Assert.True(m1);
        Assert.Equal(0, y0);
        Assert.False(m0);
        Assert.Equal(9, y8);
        Assert.True(m8);
        Assert.Equal(9, y9);
        Assert.False(m9);
    }

    [Fact]
    public void Diagonals()
    {
        Assert.True(EnvirWalkDoorCore.DiagonalCombinesGuards());
        Assert.True(EnvirWalkDoorCore.RightUsesXPlusTemplate());
        Assert.True(EnvirWalkDoorCore.DownLeftSubtractsXAddsY());
        Assert.True(EnvirWalkDoorCore.DiagonalGuardCombination());
    }

    [Fact]
    public void StepSize()
    {
        // **nFlag 是任意步长**
        Assert.True(EnvirWalkDoorCore.FlagIsStepSize());
        Assert.True(EnvirWalkDoorCore.FlagTwoBoundary());
        Assert.True(EnvirWalkDoorCore.ZeroFlagNeverMoves());
    }

    [Fact]
    public void StepSizeExact()
    {
        var (_, y, moved) = EnvirWalkDoorCore.NextPosition(5, 5, EnvirWalkDoorCore.DR_UP, 3, 10, 10);

        Assert.True(moved);
        Assert.Equal(2, y);
    }

    // ===================== 二、CanSafeWalk =====================

    [Fact]
    public void CanSafeWalkShape()
    {
        Assert.True(EnvirWalkDoorCore.ReverseIteration());
        Assert.True(EnvirWalkDoorCore.NoBreakOnVeto());
        Assert.True(EnvirWalkDoorCore.ResultStartsTrueUnlikeCanWalk());
        Assert.True(EnvirWalkDoorCore.OnlyEventObjects());
        Assert.True(EnvirWalkDoorCore.IgnoresCellFlag());
    }

    [Fact]
    public void DamageVeto()
    {
        // **只有严格大于 0 的伤害才否决**
        Assert.True(EnvirWalkDoorCore.DamagePositiveVetoes());
        Assert.True(EnvirWalkDoorCore.DamageStrictlyPositive());
        Assert.True(EnvirWalkDoorCore.DamageVetoValues());
        Assert.False(EnvirWalkDoorCore.DamageVetoes(0));
        Assert.True(EnvirWalkDoorCore.DamageVetoes(1));
    }

    [Fact]
    public void CanSafeWalkBehavior()
    {
        Assert.True(EnvirWalkDoorCore.CanSafeWalkVeto());
        Assert.True(EnvirWalkDoorCore.EmptyCellIsSafe());
        Assert.True(EnvirWalkDoorCore.CanSafeWalk(new[] { EnvirWalkDoorCore.ObjEvent }, new[] { 5 }) == false);
        Assert.True(EnvirWalkDoorCore.CanSafeWalk(new[] { 1 }, new[] { 5 }));
    }

    // ===================== 三、ArroundDoorOpened =====================

    [Fact]
    public void Neighbourhood()
    {
        // **3×3 邻域、恰好 9 格**
        Assert.True(EnvirWalkDoorCore.ThreeByThreeNeighbourhood());
        Assert.True(EnvirWalkDoorCore.NeighbourhoodSizeNine());
        Assert.True(EnvirWalkDoorCore.DiagonalIsNeighbour());
        Assert.True(EnvirWalkDoorCore.DistanceTwoExcluded());
    }

    [Fact]
    public void ChebyshevNotManhattan()
    {
        // **切比雪夫（含对角）9 格，而不是曼哈顿 5 格**
        Assert.True(EnvirWalkDoorCore.ChebyshevNotManhattan());
        Assert.True(EnvirWalkDoorCore.ManhattanWouldBeFive());
    }

    [Fact]
    public void NeighbourhoodBoundary()
    {
        Assert.True(EnvirWalkDoorCore.InNeighbourhood(1, 1, 0, 0));
        Assert.True(EnvirWalkDoorCore.InNeighbourhood(0, 0, 0, 0));
        Assert.False(EnvirWalkDoorCore.InNeighbourhood(2, 0, 0, 0));
    }

    [Fact]
    public void DoorVeto()
    {
        Assert.True(EnvirWalkDoorCore.AnyUnopenedDoorVetoes());
        Assert.True(EnvirWalkDoorCore.BreakOnFirstUnopened());
        Assert.True(EnvirWalkDoorCore.ArroundDoorOpenedValues());
    }

    [Fact]
    public void ExceptionMessage()
    {
        // **裸方法名、以空格结尾、没有错误码**
        Assert.True(EnvirWalkDoorCore.ResourceStringBareMethodName());
        Assert.True(EnvirWalkDoorCore.ResourceStringInsideFunction());
        Assert.True(EnvirWalkDoorCore.MessageEndsWithSpace());
        Assert.True(EnvirWalkDoorCore.NoErrorCodeInMessage());
        Assert.Equal("[Exception] TEnvirnoment.ArroundDoorOpened ", EnvirWalkDoorCore.ExceptionMsg);
    }

    [Fact]
    public void DoorInfoFields()
    {
        // **坐标字段没有 m_ 前缀**
        Assert.True(EnvirWalkDoorCore.FourDoorInfoFields());
        Assert.True(EnvirWalkDoorCore.NoPrefixInDoorInfo());
        Assert.Equal(new[] { "nX", "nY", "n08", "Status" }, EnvirWalkDoorCore.DoorInfoFields);
    }

    [Fact]
    public void DoorStatusFields()
    {
        // **两个字段用裸偏移命名、与 bo2B9 同一族**
        Assert.True(EnvirWalkDoorCore.FiveDoorStatusFields());
        Assert.True(EnvirWalkDoorCore.DoorStatusHasTwoOffsetNames());
        Assert.True(EnvirWalkDoorCore.SameFamilyAsBo2B9());
        Assert.True(EnvirWalkDoorCore.ThreeOffsetNamedFields());
        Assert.Equal(5, EnvirWalkDoorCore.DoorStatusFields.Length);
    }

    [Fact]
    public void ArroundUsesDoorInfoFields()
    {
        Assert.True(EnvirWalkDoorCore.ArroundUsesDoorInfoFields());
    }

    // ===================== 四、GetDoor =====================

    [Fact]
    public void GetDoorShape()
    {
        Assert.True(EnvirWalkDoorCore.GetDoorFirstMatchWithExit());
        Assert.True(EnvirWalkDoorCore.GetDoorExactMatch());
        Assert.True(EnvirWalkDoorCore.GetDoorFirstWins());
        Assert.True(EnvirWalkDoorCore.GetDoorNullWhenMissing());
    }

    [Fact]
    public void ExactVersusTolerant()
    {
        // **一个精确、一个容差 1**
        Assert.True(EnvirWalkDoorCore.ContrastWithArroundDoorOpened());
        Assert.True(EnvirWalkDoorCore.ExactVersusTolerant());
    }

    [Fact]
    public void FieldNameDiffersAcrossStructs()
    {
        // **同一概念两个结构里字段名不同**
        Assert.True(EnvirWalkDoorCore.FieldNameDiffersAcrossStructs());
        Assert.True(EnvirWalkDoorCore.TwoCoordFieldNames());
        Assert.Equal(2, EnvirWalkDoorCore.CoordFieldNames.Length);
    }

    // ===================== 五、GetEvent =====================

    [Fact]
    public void FourContrasts()
    {
        Assert.True(EnvirWalkDoorCore.FourContrastsWithCanSafeWalk());
        Assert.True(EnvirWalkDoorCore.FourContrastCount());
        Assert.Equal(4, EnvirWalkDoorCore.Contrasts.Length);
    }

    [Fact]
    public void GetEventLastWins()
    {
        // **最后一个事件覆盖前一个**
        Assert.True(EnvirWalkDoorCore.GetEventLastWins());
        Assert.True(EnvirWalkDoorCore.LastEventWins());
        Assert.True(EnvirWalkDoorCore.NoEventReturnsNull());
        Assert.True(EnvirWalkDoorCore.GetEventForwardIteration());
    }

    [Fact]
    public void GetEventBo2C()
    {
        // **顺手把 bo2C 置假且不再恢复**
        Assert.True(EnvirWalkDoorCore.GetEventTouchesBo2C());
        Assert.True(EnvirWalkDoorCore.Bo2CSetFalseAndNeverRestored());
        Assert.False(EnvirWalkDoorCore.GetEventBo2CValue());
    }

    [Fact]
    public void Bo2CIsSharedSideEffect()
    {
        Assert.True(EnvirWalkDoorCore.Bo2CIsSharedSideEffect());
        Assert.True(EnvirWalkDoorCore.ThreeBo2CWriters());
        Assert.Equal(3, EnvirWalkDoorCore.Bo2CWriters.Length);
    }

    [Fact]
    public void ReturnTypeWidened()
    {
        Assert.True(EnvirWalkDoorCore.ReturnTypeWidened());
        Assert.True(EnvirWalkDoorCore.TwoEventReturnTypes());
        Assert.Equal(2, EnvirWalkDoorCore.EventReturnTypes.Length);
    }

    [Fact]
    public void ObjEventConstant()
    {
        Assert.Equal(3, EnvirWalkDoorCore.ObjEvent);
    }

    // ===================== 六、SetMapXYFlag =====================

    [Fact]
    public void WritesZeroOrTwo()
    {
        // **真写 0、假写 2 —— 不是 0/1**
        Assert.True(EnvirWalkDoorCore.WritesZeroOrTwo());
        Assert.True(EnvirWalkDoorCore.NotZeroOrOne());
        Assert.Equal(0, EnvirWalkDoorCore.FlagValue(true));
        Assert.Equal(2, EnvirWalkDoorCore.FlagValue(false));
    }

    [Fact]
    public void FlagSemantics()
    {
        Assert.True(EnvirWalkDoorCore.TrueMeansPassable());
        Assert.True(EnvirWalkDoorCore.TwoIsBlocked());
        Assert.True(EnvirWalkDoorCore.PairsWithChFlagZeroChecks());
        Assert.True(EnvirWalkDoorCore.TwoChFlagCheckers());
        Assert.Equal(2, EnvirWalkDoorCore.ChFlagZeroCheckers.Length);
    }

    [Fact]
    public void SilentOnOutOfBounds()
    {
        // **越界静默忽略、不报错、不返回状态**
        Assert.True(EnvirWalkDoorCore.SilentOnOutOfBounds());
        Assert.True(EnvirWalkDoorCore.SilentOnOutOfBoundsValues());
        Assert.True(EnvirWalkDoorCore.NoStatusReturned());
        Assert.Equal(7, EnvirWalkDoorCore.SetFlagIfValid(false, true, 7));
    }

    // ===================== 七、CanFly =====================

    [Fact]
    public void CanFlyIntent()
    {
        Assert.True(EnvirWalkDoorCore.DesignIntentIsRayMarch());
        Assert.True(EnvirWalkDoorCore.CanFlyIntentKnown());
    }

    [Fact]
    public void PositionNeverAdvances()
    {
        // **确凿缺陷：位置从不前进**
        Assert.True(EnvirWalkDoorCore.PositionNeverAdvances());
        Assert.True(EnvirWalkDoorCore.SamePointEveryIteration());
        Assert.True(EnvirWalkDoorCore.OnlyOneDistinctPointChecked());
    }

    [Fact]
    public void AllIterationsSamePoint()
    {
        // **探针实测十轮全是 (3,0)**
        var pts = EnvirWalkDoorCore.CanFlyCheckedPoints(0, 0, 30, 0);

        Assert.Equal(10, pts.Count);

        foreach (var p in pts)
            Assert.Equal((3, 0), p);
    }

    [Fact]
    public void CounterBreaksNotProgress()
    {
        Assert.True(EnvirWalkDoorCore.CounterBreaksNotProgress());
        Assert.True(EnvirWalkDoorCore.MaximumTenCanWalkCalls());
        Assert.True(EnvirWalkDoorCore.ResultIndependentOfLaterCells());
        Assert.True(EnvirWalkDoorCore.CheckedPointIsOneTenth());
    }

    [Fact]
    public void CommentedOldCode()
    {
        // **被注释掉的旧代码用了错误变量** nDY - nDX
        Assert.True(EnvirWalkDoorCore.CommentedOldCodeUsesWrongVariable());
        Assert.True(EnvirWalkDoorCore.FixedTypoIsDyMinusDx());
        Assert.True(EnvirWalkDoorCore.LiveCodeIsDyMinusSy());
        Assert.Contains("nDY - nDX", EnvirWalkDoorCore.CommentedOldCode);
    }

    [Fact]
    public void FixDatedComment()
    {
        Assert.True(EnvirWalkDoorCore.FixDatedComment());
        Assert.True(EnvirWalkDoorCore.FixCommentContent());
        Assert.True(EnvirWalkDoorCore.MoreFundamentalBugNotFixed());
    }

    [Fact]
    public void InvalidMapReturnsTrue()
    {
        // **无效地图视为可以飞 —— 与其它无效检查方向相反**
        Assert.True(EnvirWalkDoorCore.InvalidMapReturnsTrue());
        Assert.True(EnvirWalkDoorCore.InvalidMapReturnsTrueValues());
        Assert.True(EnvirWalkDoorCore.OppositeToOtherInvalidChecks());
        Assert.True(EnvirWalkDoorCore.ThreeInvalidDirections());
        Assert.Equal(3, EnvirWalkDoorCore.InvalidCheckDirections.Length);
    }

    [Fact]
    public void BankersRounding()
    {
        // **Delphi 的 Round 用银行家舍入**
        Assert.True(EnvirWalkDoorCore.BankersRounding());
        Assert.True(EnvirWalkDoorCore.HalfRoundsToEven());
        Assert.Equal(0, EnvirWalkDoorCore.DelphiRound(0.5));
        Assert.Equal(2, EnvirWalkDoorCore.DelphiRound(1.5));
        Assert.Equal(2, EnvirWalkDoorCore.DelphiRound(2.5));
        Assert.Equal(4, EnvirWalkDoorCore.DelphiRound(3.5));
    }

    [Fact]
    public void RoundingTrap()
    {
        // **不能写成 (int)(x + 0.5)**
        Assert.True(EnvirWalkDoorCore.NotCastPlusHalf());
        Assert.True(EnvirWalkDoorCore.NegativeHalfRoundsToZero());
        Assert.Equal(0, EnvirWalkDoorCore.DelphiRound(-0.5));
    }

    [Fact]
    public void FloatDivision()
    {
        // **位移 5 → 0.5 → 舍成不动；15 → 1.5 → 2；30 → 3**
        Assert.True(EnvirWalkDoorCore.FloatDivisionNotInteger());
        Assert.True(EnvirWalkDoorCore.FiveRoundsToZero());
        Assert.True(EnvirWalkDoorCore.FifteenRoundsToTwo());
        Assert.True(EnvirWalkDoorCore.ThirtyRoundsToThree());
        Assert.True(EnvirWalkDoorCore.SmallDisplacementChecksOrigin());
    }

    [Fact]
    public void SmallDisplacement()
    {
        var pts = EnvirWalkDoorCore.CanFlyCheckedPoints(0, 0, 4, 0);

        Assert.Equal((0, 0), pts[0]);
    }

    // ===================== 八、行数 =====================

    [Fact]
    public void MethodLineCounts()
    {
        Assert.Equal(new[] { 56, 31, 25, 16, 29, 12, 29 }, EnvirWalkDoorCore.MethodLineCounts);
        Assert.True(EnvirWalkDoorCore.SevenMethods());
    }

    [Fact]
    public void LineExtremes()
    {
        Assert.True(EnvirWalkDoorCore.GetNextPositionIsLongest());
        Assert.True(EnvirWalkDoorCore.SetMapXYFlagIsShortest());
        Assert.True(EnvirWalkDoorCore.TwoTiedAt29());
    }

    [Fact]
    public void TotalLines()
    {
        Assert.True(EnvirWalkDoorCore.TotalLinesValues());
        Assert.Equal(198, EnvirWalkDoorCore.TotalLines());
        Assert.True(EnvirWalkDoorCore.LongestVsRest());
    }
}
