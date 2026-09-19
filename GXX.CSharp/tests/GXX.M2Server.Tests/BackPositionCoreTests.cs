using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J150：三个 `GetBackPosition` 重载与 `RunTo`（463 行）1:1 测试。
/// **边界与 bug 期望值均由临时探针实测后写入。**
/// </summary>
public sealed class BackPositionCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(BackPositionCore.ConstantsMatchSource());
        Assert.True(BackPositionCore.MessageIdsMatchSource());
    }

    [Fact]
    public void DirectionAndMessageConstants()
    {
        Assert.Equal(7, BackPositionCore.DrUpLeft);
        Assert.Equal(8, BackPositionCore.DirectionCount);
        Assert.Equal(20002, BackPositionCore.RmWalk);
        Assert.Equal(20004, BackPositionCore.RmRun);
        Assert.Equal(3, BackPositionCore.HorseStep);
        Assert.Equal(2, BackPositionCore.NormalStep);
        Assert.Equal(9, BackPositionCore.GmPermissionThreshold);
    }

    // ===================== 一、后退方向表 =====================

    [Fact]
    public void SharedDirectionTable()
    {
        // **八个方向都是"朝反方向退"**
        Assert.True(BackPositionCore.SharedDirectionTable());
        Assert.True(BackPositionCore.AllEightDeltasDistinct());
        Assert.True(BackPositionCore.NoZeroDeltaAmongValid());
        Assert.True(BackPositionCore.InvalidDirectionIsZeroDelta());
    }

    [Fact]
    public void BackDeltaValues()
    {
        Assert.Equal((0, 1), BackPositionCore.BackDelta(BackPositionCore.DrUp));
        Assert.Equal((0, -1), BackPositionCore.BackDelta(BackPositionCore.DrDown));
        Assert.Equal((1, 0), BackPositionCore.BackDelta(BackPositionCore.DrLeft));
        Assert.Equal((-1, 0), BackPositionCore.BackDelta(BackPositionCore.DrRight));
        Assert.Equal((1, 1), BackPositionCore.BackDelta(BackPositionCore.DrUpLeft));
        Assert.Equal((-1, 1), BackPositionCore.BackDelta(BackPositionCore.DrUpRight));
        Assert.Equal((1, -1), BackPositionCore.BackDelta(BackPositionCore.DrDownLeft));
        Assert.Equal((-1, -1), BackPositionCore.BackDelta(BackPositionCore.DrDownRight));
    }

    // ===================== 重载一 / 二 =====================

    [Fact]
    public void BackOneCardinalsAndDiagonals()
    {
        Assert.True(BackPositionCore.BackOneCardinals());
        Assert.True(BackPositionCore.BackOneDiagonals());
    }

    [Fact]
    public void BackOneBoundaries()
    {
        Assert.True(BackPositionCore.CardinaBoundaryClamps());
        Assert.True(BackPositionCore.DiagonalRequiresBothGates());
        Assert.True(BackPositionCore.DiagonalAllOrNothing());
        Assert.True(BackPositionCore.InvalidDirectionKeepsCoordinates());
    }

    [Fact]
    public void NilGuardAsymmetry()
    {
        // **注释说修了报错、却只修了三个重载里的一个**
        Assert.True(BackPositionCore.OverloadOneHasNilGuard());
        Assert.True(BackPositionCore.OverloadTwoLacksNilGuard());
        Assert.True(BackPositionCore.OverloadThreeLacksNilGuard());
        Assert.True(BackPositionCore.OnlyOneGuardDespiteComment());
        Assert.True(BackPositionCore.NilGuardTruthTable());
    }

    [Fact]
    public void AllReturnTrueOnInvalidDirection()
    {
        Assert.True(BackPositionCore.AllReturnTrueOnInvalidDirection());
        Assert.Equal((5, 5), BackPositionCore.BackOne(200, 5, 5, 10, 10));
    }

    // ===================== 重载三：DR_DOWN bug =====================

    [Fact]
    public void DownBugInOverloadThree()
    {
        // 探针实测：step3 与 step5 都得 y=19；step1 与 step9 都得 y=49
        Assert.True(BackPositionCore.DownBugInOverloadThree());
        Assert.True(BackPositionCore.DownBugIgnoresStepEntirely());

        Assert.Equal((5, 19), BackPositionCore.BackStep(4, 5, 20, 100, 100, 3));
        Assert.Equal((5, 19), BackPositionCore.BackStep(4, 5, 20, 100, 100, 5));
        Assert.Equal((5, 49), BackPositionCore.BackStep(4, 5, 50, 100, 100, 1));
        Assert.Equal((5, 49), BackPositionCore.BackStep(4, 5, 50, 100, 100, 9));
    }

    [Fact]
    public void OtherDirectionsUseStepCorrectly()
    {
        // 探针实测
        Assert.True(BackPositionCore.DownBugOnlyInOverloadThree());

        Assert.Equal((5, 23), BackPositionCore.BackStep(0, 5, 20, 100, 100, 3));
        Assert.Equal((23, 5), BackPositionCore.BackStep(6, 20, 5, 100, 100, 3));
        Assert.Equal((17, 5), BackPositionCore.BackStep(2, 20, 5, 100, 100, 3));
        Assert.Equal((23, 23), BackPositionCore.BackStep(7, 20, 20, 100, 100, 3));
        Assert.Equal((23, 17), BackPositionCore.BackStep(5, 20, 20, 100, 100, 3));
    }

    [Fact]
    public void StepZeroGate()
    {
        Assert.True(BackPositionCore.StepZeroGateTruthTable());
        Assert.False(BackPositionCore.StepZeroReturnsFalse(0));
        Assert.True(BackPositionCore.StepZeroReturnsFalse(3));
    }

    [Fact]
    public void AsymmetricStepGates()
    {
        // 探针实测：y=96 时 +3 得 99；y=97 时不动
        Assert.True(BackPositionCore.AsymmetricStepGates());

        Assert.Equal((5, 99), BackPositionCore.BackStep(0, 5, 96, 100, 100, 3));
        Assert.Equal((5, 97), BackPositionCore.BackStep(0, 5, 97, 100, 100, 3));
    }

    [Fact]
    public void StepDiagonalsAllOrNothing()
    {
        Assert.True(BackPositionCore.StepDiagonalsAllOrNothing());
        Assert.True(BackPositionCore.DiagonalsUseBeginEnd());
        Assert.True(BackPositionCore.BoundaryStylesThree());

        // 探针实测：X 可动但 Y 不可动 → 完全不动
        Assert.Equal((20, 98), BackPositionCore.BackStep(7, 20, 98, 100, 100, 3));

        // 两个都可动 → 都动
        Assert.Equal((99, 99), BackPositionCore.BackStep(7, 96, 96, 100, 100, 3));
    }

    // ===================== 二、RunTo =====================

    [Fact]
    public void RunDeltaTable()
    {
        Assert.True(BackPositionCore.RunDeltaTable());
        Assert.True(BackPositionCore.RunDeltaIsInverseOfBack());
    }

    [Fact]
    public void RunDeltaValues()
    {
        Assert.Equal((0, -1), BackPositionCore.RunDelta(0));
        Assert.Equal((0, 1), BackPositionCore.RunDelta(4));
        Assert.Equal((-1, 0), BackPositionCore.RunDelta(6));
        Assert.Equal((1, 0), BackPositionCore.RunDelta(2));
        Assert.Equal((1, 1), BackPositionCore.RunDelta(3));
    }

    [Fact]
    public void BranchSteps()
    {
        Assert.True(BackPositionCore.FiveBranchCascade());
        Assert.True(BackPositionCore.ThreeStepsForHorse());
        Assert.True(BackPositionCore.TwoStepsForOthers());
    }

    [Fact]
    public void BranchSelection()
    {
        Assert.True(BackPositionCore.BranchOrderIsPriority());
        Assert.True(BackPositionCore.DummyBeatsNormal());
        Assert.True(BackPositionCore.HorseBranchNeedsThree());
        Assert.True(BackPositionCore.HeroBranchNeedsOnlyRace());
    }

    [Fact]
    public void BranchConfigFlags()
    {
        Assert.True(BackPositionCore.ConfigFlagDiffersPerBranch());
        Assert.True(BackPositionCore.HorseAndNormalShareFlag());
        Assert.True(BackPositionCore.GmRunExceptionOnlyInHumanBranches());
    }

    [Fact]
    public void GmExceptionGate()
    {
        Assert.True(BackPositionCore.GmExceptionTruthTable());
        Assert.True(BackPositionCore.GmExceptionGate(10, true));
        Assert.False(BackPositionCore.GmExceptionGate(9, true));
        Assert.False(BackPositionCore.GmExceptionGate(10, false));
    }

    [Fact]
    public void CanRunSemantics()
    {
        // **该值是"传给 CanWalkEx 的参数"，不是"是否允许移动"的门**
        Assert.True(BackPositionCore.DefaultConfigBlocksRun());
        Assert.True(BackPositionCore.GmCanRunWhenFlagOn());
        Assert.True(BackPositionCore.FlagFalseOnlyWhenBothTermsFalse());
        Assert.True(BackPositionCore.RunFlagArg(true, 5, false));
        Assert.False(BackPositionCore.RunFlagArg(false, 0, false));

        // **探针实测：禁跑关闭但管理员例外成立时，参数仍为真**
        Assert.True(BackPositionCore.RunFlagArg(false, 10, true));
    }

    [Fact]
    public void HorseBoundaryMismatch()
    {
        // **骑马支步长 3 却只检查 Width-2，比所需更宽松一格**
        Assert.True(BackPositionCore.HorseBoundaryMismatch());
        Assert.True(BackPositionCore.HorseGateIsLooserThanCheck());

        var (gateAllows, checked3) = BackPositionCore.HorseBoundaryNumbers(100);

        Assert.Equal(97, gateAllows);
        Assert.Equal(3, checked3);
    }

    [Fact]
    public void NormalBounds()
    {
        Assert.True(BackPositionCore.NormalBranchBounds());
        Assert.True(BackPositionCore.BoundsTruthTable());
        Assert.False(BackPositionCore.LeftTopBoundOk(1));
        Assert.True(BackPositionCore.LeftTopBoundOk(2));
        Assert.True(BackPositionCore.RightBottomBoundOk(97, 100));
        Assert.False(BackPositionCore.RightBottomBoundOk(98, 100));
    }

    // ===================== RunTo 的收尾与残留 =====================

    [Fact]
    public void DingShenGate()
    {
        Assert.True(BackPositionCore.DingShenTruthTable());
        Assert.True(BackPositionCore.DingShenExitsFalse(true));
        Assert.False(BackPositionCore.DingShenExitsFalse(false));
    }

    [Fact]
    public void DirectionAssignedEarly()
    {
        // **走不动也照样转身**
        Assert.True(BackPositionCore.DirectionAssignedBeforeMoveChecks());
        Assert.Equal(BackPositionCore.DrUpLeft, BackPositionCore.DirectionAssigned(BackPositionCore.DrUpLeft));
    }

    [Fact]
    public void ResultOnlyTrueOnWalkSuccess()
    {
        Assert.True(BackPositionCore.ResultTruthTable());
        Assert.True(BackPositionCore.ResultOnlyTrueOnWalkSuccess(true, true));
        Assert.False(BackPositionCore.ResultOnlyTrueOnWalkSuccess(true, false));
        Assert.False(BackPositionCore.ResultOnlyTrueOnWalkSuccess(false, true));
    }

    [Fact]
    public void CommentedOutDestCondition()
    {
        // **花括号注释写在布尔表达式中间，依赖两个未声明变量**
        Assert.True(BackPositionCore.CommentedOutDestConditionPresent());
        Assert.True(BackPositionCore.CommentedConditionUsesUndeclaredVars());
        Assert.True(BackPositionCore.TailConditionTruthTable());
    }

    [Fact]
    public void StationTickMissingParens()
    {
        // **第三次出现"tick 赋值少了括号"**
        Assert.True(BackPositionCore.MissingParensThirdTime());
        Assert.True(BackPositionCore.StationTickMissingParens());
    }

    [Fact]
    public void RollbackOnWalkFailure()
    {
        Assert.True(BackPositionCore.RollbackOnWalkFailureValues());
        Assert.True(BackPositionCore.FailedRollbackKeepsNewPosition());

        Assert.Equal((5, 5), BackPositionCore.RollbackOnWalkFailure(false, true, 7, 7, 5, 5));
        Assert.Equal((7, 7), BackPositionCore.RollbackOnWalkFailure(false, false, 7, 7, 5, 5));
    }

    [Fact]
    public void ExceptionFormat()
    {
        // **裸方法名风格**
        Assert.True(BackPositionCore.ExceptionFormatIsBareMethodName());
        Assert.True(BackPositionCore.BodyWrappedInTryExcept());
        Assert.Equal("[Exception] TBaseObject.RunTo", BackPositionCore.RunToExceptionMsg);
    }

    [Fact]
    public void RunToIsLongestFunction()
    {
        // **463 行，长于 J146 的 419 行**
        Assert.True(BackPositionCore.RunToIsLongestFunction());
        Assert.Equal(463, BackPositionCore.RunToLineCount);
    }

    [Fact]
    public void ThreeBackPositionOverloads()
    {
        Assert.True(BackPositionCore.ThreeBackPositionOverloads());
        Assert.True(BackPositionCore.OverloadThreeIsLongest());
        Assert.Equal(3, BackPositionCore.BackPositionLineCounts.Length);
    }
}
