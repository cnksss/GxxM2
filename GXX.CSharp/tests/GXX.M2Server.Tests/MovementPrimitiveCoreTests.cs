using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J141：服务端基础方法本体 —— `TurnTo`/`TurnToEx`/`MakeGhost`/`SetTargetXY`/
/// `Wondering`/`TAnimalObject.Run`（含注释块）/`GotoTargetXY`/`SpaceMove.GetRandXY`
/// （ObjBase.pas 15675-15737、22448-22489、33382-33392、33697-33706、40154-40220）1:1 测试。
/// </summary>
public sealed class MovementPrimitiveCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(MovementPrimitiveCore.ConstantsMatchSource());
        Assert.True(MovementPrimitiveCore.TurnMessages());
        Assert.True(MovementPrimitiveCore.DirectionsAreContiguous());
    }

    [Fact]
    public void MessageAndDirectionValues()
    {
        Assert.Equal(20001, MovementPrimitiveCore.RmTurn);
        Assert.Equal(20256, MovementPrimitiveCore.RmTurnEx);
        Assert.Equal(0, MovementPrimitiveCore.DrUp);
        Assert.Equal(7, MovementPrimitiveCore.DrUpLeft);
        Assert.Equal(8, MovementPrimitiveCore.DirectionCount);
    }

    // ===================== 一、TurnTo / TurnToEx =====================

    [Fact]
    public void TurnToAndExDifferOnlyByMessage()
    {
        // **逐字相同，只差一个消息号**
        Assert.True(MovementPrimitiveCore.TurnToAndExDifferOnlyByMessage());
        Assert.True(MovementPrimitiveCore.TwoTwinMethods());
        Assert.True(MovementPrimitiveCore.TurnMessageValues());
        Assert.True(MovementPrimitiveCore.BothAssignFirstThenSend());
    }

    [Fact]
    public void TurnValues()
    {
        Assert.Equal((3, 20001), MovementPrimitiveCore.Turn(3, false));
        Assert.Equal((3, 20256), MovementPrimitiveCore.Turn(3, true));
    }

    [Fact]
    public void NoDirectionRangeCheck()
    {
        // **无范围校验：越界方向被原样接受**
        Assert.True(MovementPrimitiveCore.NoDirectionRangeCheck());
        Assert.Equal(99, MovementPrimitiveCore.Turn(99, false).Direction);
        Assert.Equal(-5, MovementPrimitiveCore.Turn(-5, true).Direction);
    }

    [Fact]
    public void GuardUsesPlainTurnTo()
    {
        // **J140 护卫转身用的是普通 RM_TURN**
        Assert.True(MovementPrimitiveCore.GuardUsesPlainTurnTo());
    }

    // ===================== 二、MakeGhost =====================

    [Fact]
    public void MakeGhostSixSteps()
    {
        Assert.True(MovementPrimitiveCore.MakeGhostSixSteps());
        Assert.Equal(6, MovementPrimitiveCore.MakeGhostSteps.Length);
        Assert.True(MovementPrimitiveCore.MakeGhostSetsFlagAndTick());
    }

    [Fact]
    public void ClearsCurrTargetNotTargetCret()
    {
        // **清的是 m_CurrTarget/m_CurrTargetEx，不是攻击目标**
        Assert.True(MovementPrimitiveCore.ClearsCurrTargetNotTargetCret());
        Assert.True(MovementPrimitiveCore.KeepsTargetCret());
        Assert.Contains("m_CurrTarget := nil", MovementPrimitiveCore.MakeGhostSteps);
        Assert.DoesNotContain("m_TargetCret := nil", MovementPrimitiveCore.MakeGhostSteps);
    }

    [Fact]
    public void PluginHookIsGuarded()
    {
        Assert.True(MovementPrimitiveCore.PluginHookIsGuarded());
        Assert.Contains("guarded", MovementPrimitiveCore.MakeGhostSteps[5]);
    }

    // ===================== 三、SetTargetXY / Wondering =====================

    [Fact]
    public void SetTargetXYIsPureAssignment()
    {
        // **纯赋值、无任何校验**
        Assert.True(MovementPrimitiveCore.SetTargetXYIsPureAssignment());
        Assert.True(MovementPrimitiveCore.NoValidation());
        Assert.Equal((5, 7), MovementPrimitiveCore.SetTargetXY(5, 7));
    }

    [Fact]
    public void NegativeOneWritesSentinel()
    {
        // **传 -1 会直接写进哨兵值**
        Assert.True(MovementPrimitiveCore.NegativeOneWritesSentinel());
        Assert.True(MovementPrimitiveCore.MatchesSentinelConvention());
        Assert.Equal((-1, -1), MovementPrimitiveCore.SetTargetXY(-1, -1));
    }

    [Fact]
    public void WonderingTwoLevelRandom()
    {
        // **外层 1/20、内层 1/4**
        Assert.True(MovementPrimitiveCore.WonderingTwoLevelRandom());
        Assert.True(MovementPrimitiveCore.OuterGateIsFivePercent());
        Assert.True(MovementPrimitiveCore.InnerSplitIsQuarterTurn());
    }

    [Fact]
    public void WanderActionTruthTable()
    {
        Assert.True(MovementPrimitiveCore.WanderActionTruthTable());
        Assert.Equal("idle", MovementPrimitiveCore.WanderAction(1, 0, true));
        Assert.Equal("turn", MovementPrimitiveCore.WanderAction(0, 1, true));
        Assert.Equal("walk", MovementPrimitiveCore.WanderAction(0, 0, true));
        Assert.Equal("walk", MovementPrimitiveCore.WanderAction(0, 3, true));
    }

    [Fact]
    public void TurnAndWalkMutuallyExclusive()
    {
        Assert.True(MovementPrimitiveCore.TurnAndWalkMutuallyExclusive());
    }

    [Fact]
    public void WalkRequiresCanMove()
    {
        Assert.True(MovementPrimitiveCore.WalkRequiresCanMove());
        Assert.True(MovementPrimitiveCore.ImmobileNonTurnDoesNothing());
        Assert.Equal("idle", MovementPrimitiveCore.WanderAction(0, 0, false));
    }

    [Fact]
    public void CombinedProbability()
    {
        Assert.True(MovementPrimitiveCore.CombinedProbability());
        Assert.Equal((1, 3, 80), MovementPrimitiveCore.CombinedProbabilityValues());
    }

    [Fact]
    public void WanderDetailFlags()
    {
        Assert.True(MovementPrimitiveCore.TurnUsesRandomEight());
        Assert.True(MovementPrimitiveCore.WalkUsesCurrentDirection());
        Assert.True(MovementPrimitiveCore.WalkFlagIsFalse());
    }

    // ===================== 四、被注释掉的 Run 巡逻块 =====================

    [Fact]
    public void RunBodyIsFullyCommentedOut()
    {
        // **只有 inherited 是活的，其余 46 行被整体注释**
        Assert.True(MovementPrimitiveCore.RunBodyIsFullyCommentedOut());
        Assert.True(MovementPrimitiveCore.OnlyInheritedIsLive());
        Assert.True(MovementPrimitiveCore.CommentedBlockIs46Lines());
        Assert.True(MovementPrimitiveCore.LargestDeadCodeBlockSoFar());
        Assert.Equal(46, MovementPrimitiveCore.CommentedRunBlockLines);
    }

    [Fact]
    public void FourGatesOnPatrol()
    {
        Assert.True(MovementPrimitiveCore.FourGatesOnPatrol());
        Assert.Equal(4, MovementPrimitiveCore.PatrolGates.Length);
    }

    [Fact]
    public void KeepMaxIsOnePointFiveChebyshev()
    {
        // **1.5 倍切比雪夫（与 J136 记录一致）**
        Assert.True(MovementPrimitiveCore.KeepMaxIsOnePointFiveChebyshev());
        Assert.True(MovementPrimitiveCore.KeepMaxIsNotManhattan());
        Assert.Equal(5, MovementPrimitiveCore.KeepMax(3, 3));
        Assert.Equal(3, MovementPrimitiveCore.KeepMax(2, 2));
        Assert.Equal(0, MovementPrimitiveCore.KeepMax(0, 0));
    }

    [Fact]
    public void KeepCountBoundary()
    {
        // **严格大于**
        Assert.True(MovementPrimitiveCore.KeepCountBoundary());
        Assert.False(MovementPrimitiveCore.KeepCountUsesStrictGreater(5, 5));
        Assert.True(MovementPrimitiveCore.KeepCountUsesStrictGreater(6, 5));
    }

    [Fact]
    public void DoubleZeroGuardOnIndex()
    {
        // **索引双重零保护**
        Assert.True(MovementPrimitiveCore.DoubleZeroGuardOnIndex());
        Assert.Equal(0, MovementPrimitiveCore.AdvanceAroundIndex(3, 3));
        Assert.Equal(0, MovementPrimitiveCore.AdvanceAroundIndex(-1, 3));
        Assert.Equal(2, MovementPrimitiveCore.AdvanceAroundIndex(1, 3));
    }

    [Fact]
    public void WalkTickUsesStrictGreater()
    {
        Assert.True(MovementPrimitiveCore.WalkTickUsesStrictGreater());
        Assert.True(MovementPrimitiveCore.WalkDueBoundary());
    }

    [Fact]
    public void CommentedBlockDetails()
    {
        Assert.True(MovementPrimitiveCore.CommentedBlockUsesSentinel());
        Assert.True(MovementPrimitiveCore.CommentedBlockCallsGoto());
    }

    // ===================== 五、GotoTargetXY =====================

    [Fact]
    public void GotoSkipTruthTable()
    {
        Assert.True(MovementPrimitiveCore.GotoSkipTruthTable());
        Assert.False(MovementPrimitiveCore.GotoSkipsWhenAlreadyThere(5, 5, 5, 5));
        Assert.True(MovementPrimitiveCore.GotoSkipsWhenAlreadyThere(5, 5, 6, 5));
    }

    [Fact]
    public void EightWaySelection()
    {
        Assert.True(MovementPrimitiveCore.EightWaySelection());
        Assert.Equal(MovementPrimitiveCore.DrRight, MovementPrimitiveCore.ChooseDirection(5, 5, 6, 5));
        Assert.Equal(MovementPrimitiveCore.DrUpLeft, MovementPrimitiveCore.ChooseDirection(5, 5, 4, 4));
    }

    [Fact]
    public void ChooseDirectionAllEight()
    {
        Assert.Equal(MovementPrimitiveCore.DrRight, MovementPrimitiveCore.ChooseDirection(5, 5, 6, 5));
        Assert.Equal(MovementPrimitiveCore.DrLeft, MovementPrimitiveCore.ChooseDirection(5, 5, 4, 5));
        Assert.Equal(MovementPrimitiveCore.DrDown, MovementPrimitiveCore.ChooseDirection(5, 5, 5, 6));
        Assert.Equal(MovementPrimitiveCore.DrUp, MovementPrimitiveCore.ChooseDirection(5, 5, 5, 4));
        Assert.Equal(MovementPrimitiveCore.DrDownRight, MovementPrimitiveCore.ChooseDirection(5, 5, 6, 6));
        Assert.Equal(MovementPrimitiveCore.DrUpRight, MovementPrimitiveCore.ChooseDirection(5, 5, 6, 4));
        Assert.Equal(MovementPrimitiveCore.DrDownLeft, MovementPrimitiveCore.ChooseDirection(5, 5, 4, 6));
        Assert.Equal(MovementPrimitiveCore.DrUpLeft, MovementPrimitiveCore.ChooseDirection(5, 5, 4, 4));
    }

    [Fact]
    public void InitialDownIsDeadBranch()
    {
        // **DR_DOWN 初值走不到（穷举验证）**
        Assert.True(MovementPrimitiveCore.InitialDownIsDeadBranch());
    }

    [Fact]
    public void RetryLoopIsEightTimes()
    {
        Assert.True(MovementPrimitiveCore.RetryLoopIsEightTimes());
        Assert.True(MovementPrimitiveCore.RandomThreeChosenOnceOutsideLoop());
        Assert.True(MovementPrimitiveCore.RetryModulusIsThree());
    }

    [Fact]
    public void NudgeWrapsBothEnds()
    {
        // **两个方向都环绕**
        Assert.True(MovementPrimitiveCore.WrapsBothEnds());
        Assert.Equal(MovementPrimitiveCore.DrUp, MovementPrimitiveCore.NudgeDirection(7, 1));
        Assert.Equal(MovementPrimitiveCore.DrUpLeft, MovementPrimitiveCore.NudgeDirection(0, 0));
    }

    [Fact]
    public void NudgeDirections()
    {
        // roll != 0 → 顺时针
        Assert.Equal(MovementPrimitiveCore.DrUpRight, MovementPrimitiveCore.NudgeDirection(0, 1));
        Assert.Equal(MovementPrimitiveCore.DrUpRight, MovementPrimitiveCore.NudgeDirection(0, 2));

        // roll == 0 → 逆时针
        Assert.Equal(MovementPrimitiveCore.DrUpRight, MovementPrimitiveCore.NudgeDirection(2, 0));
        Assert.Equal(MovementPrimitiveCore.DrDownRight, MovementPrimitiveCore.NudgeDirection(4, 0));
    }

    [Fact]
    public void TwoRetryModes()
    {
        Assert.True(MovementPrimitiveCore.CwWhenNonZero());
        Assert.True(MovementPrimitiveCore.CcwWhenZero());
        Assert.True(MovementPrimitiveCore.TwoRetryModes());
        Assert.True(MovementPrimitiveCore.ZeroGoesToUpLeft());
        Assert.Equal("ccw", MovementPrimitiveCore.RetryDescription(0));
        Assert.Equal("cw", MovementPrimitiveCore.RetryDescription(1));
        Assert.Equal("cw", MovementPrimitiveCore.RetryDescription(2));
    }

    [Fact]
    public void NoBreakOnSuccess()
    {
        // **成功后不 Break，一直试满 8 次**
        Assert.True(MovementPrimitiveCore.NoBreakOnSuccess());
        Assert.True(MovementPrimitiveCore.OnlyFirstWalkChecksMovement());
        Assert.True(MovementPrimitiveCore.RetryWalkFlagIsFalse());
    }

    [Fact]
    public void RunToVariantExists()
    {
        Assert.True(MovementPrimitiveCore.RunToVariantExists());
        Assert.True(MovementPrimitiveCore.RunToLineValue());
        Assert.Equal(15739, MovementPrimitiveCore.RunToTargetLine);
    }

    [Fact]
    public void GotoHasCommentedTickLine()
    {
        Assert.True(MovementPrimitiveCore.GotoHasCommentedTickLine());
        Assert.Contains("dwTick3F4", MovementPrimitiveCore.CommentedTickLine);
    }

    // ===================== 六、SpaceMove / GetRandXY =====================

    [Fact]
    public void ShopStallAndTruckBlocked()
    {
        // **摆摊中的玩家与镖车都不能被空间移动**
        Assert.True(MovementPrimitiveCore.ShopStallTruthTable());
        Assert.True(MovementPrimitiveCore.ShopStallAndTruckBlocked(0, true));
        Assert.True(MovementPrimitiveCore.ShopStallAndTruckBlocked(128, false));
        Assert.False(MovementPrimitiveCore.ShopStallAndTruckBlocked(0, false));
    }

    [Fact]
    public void TwoAxisStepTables()
    {
        // **横向 2 档、纵向 3 档**
        Assert.True(MovementPrimitiveCore.TwoAxisStepTables());
        Assert.True(MovementPrimitiveCore.StepTableSizesMatch());
        Assert.Equal((2, 3), MovementPrimitiveCore.StepTableSizes());
        Assert.True(MovementPrimitiveCore.HorizontalStepValues());
        Assert.True(MovementPrimitiveCore.VerticalMarginValues());
    }

    [Fact]
    public void StepTableBoundaries()
    {
        Assert.Equal(3, MovementPrimitiveCore.HorizontalStep(79));
        Assert.Equal(10, MovementPrimitiveCore.HorizontalStep(80));

        Assert.Equal(2, MovementPrimitiveCore.VerticalMargin(49));
        Assert.Equal(15, MovementPrimitiveCore.VerticalMargin(50));
        Assert.Equal(15, MovementPrimitiveCore.VerticalMargin(149));
        Assert.Equal(50, MovementPrimitiveCore.VerticalMargin(150));
    }

    [Fact]
    public void VerticalUsesHorizontalStep()
    {
        // **纵向推进误用横向步长，n1C 只作边界余量**
        Assert.True(MovementPrimitiveCore.VerticalUsesHorizontalStep());
        Assert.True(MovementPrimitiveCore.N1COnlyUsedAsMargin());
        Assert.True(MovementPrimitiveCore.MarginAndStepDisagreeOnLargeMaps());
        Assert.Equal(10, MovementPrimitiveCore.ActualVerticalStep(200, 200));
    }

    [Fact]
    public void LoopCapIs201()
    {
        Assert.True(MovementPrimitiveCore.LoopCapIs201());
        Assert.True(MovementPrimitiveCore.LoopCapIsInclusiveUpperBound());
        Assert.False(MovementPrimitiveCore.ShouldAbort(200));
        Assert.True(MovementPrimitiveCore.ShouldAbort(201));
    }

    [Fact]
    public void CanWalkUsesTrueFlag()
    {
        Assert.True(MovementPrimitiveCore.CanWalkUsesTrueFlag());
        Assert.True(MovementPrimitiveCore.CanWalkFlagDiffersAcrossCallers());
        Assert.True(MovementPrimitiveCore.TriesCurrentPointFirst());
    }

    [Fact]
    public void AdvanceRules()
    {
        Assert.True(MovementPrimitiveCore.AdvanceHorizontal());
        Assert.True(MovementPrimitiveCore.AdvanceVerticalUsesN18());
        Assert.True(MovementPrimitiveCore.ResetsXWhenAtEdge());
        Assert.True(MovementPrimitiveCore.DoubleRandomWhenBothAtEdge());
    }

    [Fact]
    public void AdvanceValues()
    {
        Assert.Equal((10, 0), MovementPrimitiveCore.Advance(0, 0, 200, 200, 0, 0));
        Assert.Equal((0, 10), MovementPrimitiveCore.Advance(199, 0, 200, 200, 0, 0));
        Assert.Equal((42, 77), MovementPrimitiveCore.Advance(199, 199, 200, 200, 42, 77));
    }

    [Fact]
    public void FailureSemantics()
    {
        Assert.True(MovementPrimitiveCore.ReturnsFalseOnFailure());
        Assert.True(MovementPrimitiveCore.CallerMayIgnoreResult());
        Assert.True(MovementPrimitiveCore.SpaceMoveAddressComment());
        Assert.Equal("// 004BCD1C", MovementPrimitiveCore.SpaceMoveAddress);
    }

    // ===================== 顶层仿真 =====================

    [Fact]
    public void SimulateGotoValues()
    {
        Assert.True(MovementPrimitiveCore.SimulateGotoValues());
        Assert.Equal((MovementPrimitiveCore.DrDownRight, MovementPrimitiveCore.DrDown),
            MovementPrimitiveCore.SimulateGoto(5, 5, 6, 6, 1));
        Assert.Equal((MovementPrimitiveCore.DrDownRight, MovementPrimitiveCore.DrRight),
            MovementPrimitiveCore.SimulateGoto(5, 5, 6, 6, 0));
    }

    [Fact]
    public void GetRandXyImmediate()
    {
        Assert.True(MovementPrimitiveCore.GetRandXyImmediate());
    }

    [Fact]
    public void GetRandXyGivesUp()
    {
        // **永不可走时达到 201 次上限后放弃**
        Assert.True(MovementPrimitiveCore.GetRandXyGivesUp());
    }

    [Fact]
    public void GetRandXyFindsEventually()
    {
        Assert.True(MovementPrimitiveCore.GetRandXyFindsEventually());
    }

    [Fact]
    public void SimulateWander()
    {
        Assert.Equal("idle", MovementPrimitiveCore.SimulateWander(1, 0, true));
        Assert.Equal("turn", MovementPrimitiveCore.SimulateWander(0, 1, true));
        Assert.Equal("walk", MovementPrimitiveCore.SimulateWander(0, 0, true));
        Assert.Equal("idle", MovementPrimitiveCore.SimulateWander(0, 0, false));
    }
}
