using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J216：`ObjMon.pas` 中 `TFoxMonster`（狐狸）**后两个方法**的 1:1 测试
/// （`WonderingEx` 68 行 + `Run` 127 行 = 195 行）——
/// **本批之后 `TFoxMonster` 全部 400 行完成**（前四方法见 J215）。
/// **本批最有价值的发现**：
/// ① `WonderingEx` 那个 `m_boMagicAttack` 分支的两支**逐字相同**（25 行零差异）、是个空分派；
/// ② `Random(9)` 会产生非法方向 8 —— 但经核实**不会崩溃**，
///    只是"浪费一次迭代"、且 `nCount >= 7` 使**任何一次运行都至少漏掉一个方向**。
/// </summary>
public sealed class ObjMonFoxRunCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(5738, ObjMonFoxRunCore.WonderingStart);
        Assert.Equal(5805, ObjMonFoxRunCore.WonderingEnd);
        Assert.Equal(68, ObjMonFoxRunCore.WonderingLines);
        Assert.Equal(5807, ObjMonFoxRunCore.RunStart);
        Assert.Equal(5933, ObjMonFoxRunCore.RunEnd);
        Assert.Equal(127, ObjMonFoxRunCore.RunLines);
        Assert.Equal(195, ObjMonFoxRunCore.TotalLines);
        Assert.Equal(400, ObjMonFoxRunCore.ClassTotalLines);
        Assert.Equal(205, ObjMonFoxRunCore.J215Lines);

        Assert.Equal(5750, ObjMonFoxRunCore.Branch1Start);
        Assert.Equal(5774, ObjMonFoxRunCore.Branch1End);
        Assert.Equal(5778, ObjMonFoxRunCore.Branch2Start);
        Assert.Equal(5802, ObjMonFoxRunCore.Branch2End);
        Assert.Equal(25, ObjMonFoxRunCore.BranchLines);
        Assert.Equal(0, ObjMonFoxRunCore.BranchDiffLines);
        Assert.Equal(5748, ObjMonFoxRunCore.MagicFlagLine);

        Assert.Equal(9, ObjMonFoxRunCore.RandomNineBound);
        Assert.Equal(8, ObjMonFoxRunCore.RandomEightBound);
        Assert.Equal(5571, ObjMonFoxRunCore.ThinkRandomEightLine);
        Assert.Equal(8, ObjMonFoxRunCore.DirectionCount);
        Assert.Equal(0, ObjMonFoxRunCore.DR_UP);
        Assert.Equal(4, ObjMonFoxRunCore.DR_DOWN);
        Assert.Equal(7, ObjMonFoxRunCore.DR_UPLEFT);
        Assert.Equal(3065, ObjMonFoxRunCore.ComputeOverloadDeclLine);
        Assert.Equal(3067, ObjMonFoxRunCore.RotateOverloadDeclLine);
        Assert.Equal(10820, ObjMonFoxRunCore.RotateDefaultLine);
        Assert.Equal(4579, ObjMonFoxRunCore.GetNextPositionFalseLine);
        Assert.Equal(2736, ObjMonFoxRunCore.CanWalkEx2Impl);
        Assert.Equal(7, ObjMonFoxRunCore.LoopCap);

        Assert.Equal(5811, ObjMonFoxRunCore.GuardLine);
        Assert.Equal(5406, ObjMonFoxRunCore.J214GuardLine);
        Assert.Equal(0, ObjMonFoxRunCore.GuardDiffLines);
        Assert.Equal(18, ObjMonFoxRunCore.WalkBlockLines);
        Assert.Equal(1, ObjMonFoxRunCore.WalkBlockDiffLines);
        Assert.Equal(3, ObjMonFoxRunCore.MasterBlockDiffLines);
        Assert.Equal(32, ObjMonFoxRunCore.SharedBlockLines);
        Assert.Equal(4, ObjMonFoxRunCore.SharedBlockDiffs);
        Assert.Equal(5886, ObjMonFoxRunCore.NxCommentLine);
        Assert.Equal(5856, ObjMonFoxRunCore.AttackTargetCallLine);

        Assert.Equal(3, ObjMonFoxRunCore.MissionReachThreshold);
        Assert.Equal(47, ObjMonFoxRunCore.MissionIndexSites);
        Assert.Equal(33, ObjMonFoxRunCore.MissionPointsSites);
        Assert.Equal(20, ObjMonFoxRunCore.MasterDistanceThreshold);
        Assert.Equal(1, ObjMonFoxRunCore.SpaceMoveFourthArg);
        Assert.Equal(748, ObjMonFoxRunCore.SpaceMoveDeclLine);
        Assert.Equal(4, ObjMonFoxRunCore.EarlyExitCount);
        Assert.Equal(5930, ObjMonFoxRunCore.RemovedConditionLine);
        Assert.Equal(856, ObjMonFoxRunCore.WonderingDeclLine);
        Assert.Equal(7607, ObjMonFoxRunCore.WonderingImplLine);
        Assert.Equal(854, ObjMonFoxRunCore.GotoDeclLine);
        Assert.Equal(20, ObjMonFoxRunCore.ClassesCovered);
        Assert.Equal(34, ObjMonFoxRunCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonFoxRunCore.SpanMatches());
        Assert.True(ObjMonFoxRunCore.TotalLinesAddUp());
        Assert.True(ObjMonFoxRunCore.MethodsAscending());
        Assert.True(ObjMonFoxRunCore.MethodsContiguous());
        Assert.True(ObjMonFoxRunCore.RunDecompositionAddsUp());
        Assert.True(ObjMonFoxRunCore.WonderingDecompositionAddsUp());
        Assert.True(ObjMonFoxRunCore.TwoBatchesAddUp());
        Assert.True(ObjMonFoxRunCore.WithinUnit());
        Assert.True(ObjMonFoxRunCore.NoInstrumentation());
    }

    // ===================== 一、两支逐字相同 =====================

    [Fact]
    public void BranchIdentityFacts()
    {
        Assert.True(ObjMonFoxRunCore.TwoBranchesIdentical());
        Assert.True(ObjMonFoxRunCore.TwentyFiveLinesZeroDiff());
        Assert.True(ObjMonFoxRunCore.BranchIsPointless());
        Assert.True(ObjMonFoxRunCore.SameBodyBothArms());
        Assert.True(ObjMonFoxRunCore.GetIntoAttackPosition());
        Assert.True(ObjMonFoxRunCore.BranchSpansMatch());
        Assert.True(ObjMonFoxRunCore.BranchLengthsEqual());
        Assert.True(ObjMonFoxRunCore.Branch2FollowsBranch1());
        Assert.True(ObjMonFoxRunCore.WonderingBodyExtracted());
        Assert.True(ObjMonFoxRunCore.ArmsBehaveIdentically());
    }

    [Fact]
    public void BranchSpanArithmetic()
    {
        // **两支各 25 行、长度完全相同**
        Assert.Equal(25, ObjMonFoxRunCore.Branch1End - ObjMonFoxRunCore.Branch1Start + 1);
        Assert.Equal(25, ObjMonFoxRunCore.Branch2End - ObjMonFoxRunCore.Branch2Start + 1);
        Assert.Equal(ObjMonFoxRunCore.Branch1End - ObjMonFoxRunCore.Branch1Start,
            ObjMonFoxRunCore.Branch2End - ObjMonFoxRunCore.Branch2Start);

        Assert.Equal(15, ObjMonFoxRunCore.WonderingBody.Length);
    }

    [Fact]
    public void ArmSelection()
    {
        Assert.True(ObjMonFoxRunCore.FlagTrueArm1());
        Assert.True(ObjMonFoxRunCore.FlagFalseArm2());
        Assert.Equal("arm1", ObjMonFoxRunCore.PickArm(true));
        Assert.Equal("arm2", ObjMonFoxRunCore.PickArm(false));
    }

    // ===================== 二、Random(9) =====================

    [Fact]
    public void RandomBoundFacts()
    {
        Assert.True(ObjMonFoxRunCore.OnlyTwoRandom9Sites());
        Assert.True(ObjMonFoxRunCore.EightRandom8Sites());
        Assert.True(ObjMonFoxRunCore.SameClassCorrectInThink());
        Assert.True(ObjMonFoxRunCore.ValueEightIsInvalid());
        Assert.True(ObjMonFoxRunCore.DirectionsAreZeroToSeven());
        Assert.True(ObjMonFoxRunCore.RandomTablesExtracted());
        Assert.True(ObjMonFoxRunCore.BothNineSitesHere());
        Assert.True(ObjMonFoxRunCore.BoundExceedsValidByOne());
        Assert.True(ObjMonFoxRunCore.EightBoundIsExact());

        // **`Random(9)` 只有两处、都在本方法**
        Assert.Equal(2, ObjMonFoxRunCore.RandomNineLines.Length);
        Assert.Equal(5752, ObjMonFoxRunCore.RandomNineLines[0]);
        Assert.Equal(5780, ObjMonFoxRunCore.RandomNineLines[1]);

        // **`Random(8)` 有八处、含本类 Think 的 5571**
        Assert.Equal(8, ObjMonFoxRunCore.RandomEightLines.Length);
        Assert.Equal(5571, ObjMonFoxRunCore.RandomEightLines[4]);
    }

    [Fact]
    public void DirectionBoundaries()
    {
        // **方向合法范围 0..7、`Random(9)` 上界是 9 => 8 越界**
        Assert.Equal(0, ObjMonFoxRunCore.DR_UP);
        Assert.Equal(7, ObjMonFoxRunCore.DR_UPLEFT);
        Assert.Equal(8, ObjMonFoxRunCore.DirectionCount);
        Assert.Equal(9, ObjMonFoxRunCore.RandomNineBound);
        Assert.Equal(8, ObjMonFoxRunCore.RandomEightBound);
    }

    [Fact]
    public void NoCrashFacts()
    {
        Assert.True(ObjMonFoxRunCore.NoCrashNoOob());
        Assert.True(ObjMonFoxRunCore.CalleeCaseHasNoElse());
        Assert.True(ObjMonFoxRunCore.GetNextPositionReturnsFalseOnNoArm());
        Assert.True(ObjMonFoxRunCore.GetNextDirectionDefaultsToDown());
    }

    [Fact]
    public void CalleeBehaviourBoundaries()
    {
        // **方向 8 不产生位移（`GetNextPosition` 无匹配分支）**
        Assert.False(ObjMonFoxRunCore.NextPositionMoves(8, 1));
        Assert.True(ObjMonFoxRunCore.DirEightDoesNotMove());

        // **方向 4（DR_DOWN）产生位移**
        Assert.True(ObjMonFoxRunCore.NextPositionMoves(4, 1));
        Assert.True(ObjMonFoxRunCore.DirFourMoves());

        // **八个合法方向都产生位移**
        for (int d = 0; d < 8; d++)
            Assert.True(ObjMonFoxRunCore.NextPositionMoves(d, 1));
    }

    [Fact]
    public void RotateBoundaries()
    {
        // **方向 8 落回默认 `DR_DOWN` = 4**
        Assert.True(ObjMonFoxRunCore.DirEightFallsBackToDown());
        Assert.Equal(4, ObjMonFoxRunCore.Rotate(8));

        // **合法方向旋转一格、且循环**
        Assert.True(ObjMonFoxRunCore.ValidDirectionRotatesByOne());
        Assert.True(ObjMonFoxRunCore.RotationWraps());
        Assert.Equal(1, ObjMonFoxRunCore.Rotate(0));
        Assert.Equal(0, ObjMonFoxRunCore.Rotate(7));
    }

    [Fact]
    public void WastedIterationFacts()
    {
        Assert.True(ObjMonFoxRunCore.WastesOneIteration());
        Assert.True(ObjMonFoxRunCore.RestartsFromDown());
        Assert.True(ObjMonFoxRunCore.CapIsSevenNotEight());
        Assert.True(ObjMonFoxRunCore.AlwaysOneDirectionSkipped());
        Assert.True(ObjMonFoxRunCore.SixWhenRollIsEight());
        Assert.True(ObjMonFoxRunCore.IntentNeverFulfilled());
        Assert.True(ObjMonFoxRunCore.IncrementThenCheck());
        Assert.True(ObjMonFoxRunCore.SevenBodyExecutions());
    }

    [Fact]
    public void ScanCountBoundaries()
    {
        // **掷中合法值扫 7 个方向、掷中 8 只扫 6 个**
        Assert.True(ObjMonFoxRunCore.ValidRollScansSeven());
        Assert.True(ObjMonFoxRunCore.RollEightScansSix());
        Assert.True(ObjMonFoxRunCore.ScanCountsDifferByOne());
        Assert.Equal(7, ObjMonFoxRunCore.DirectionsScanned(0));
        Assert.Equal(6, ObjMonFoxRunCore.DirectionsScanned(8));

        // **任何一掷都少于八个方向 —— 意图从未实现**
        Assert.True(ObjMonFoxRunCore.NeverScansAllEight());
    }

    [Fact]
    public void CanWalkEx2Facts()
    {
        Assert.True(ObjMonFoxRunCore.UsesCanWalkEx2());
        Assert.True(ObjMonFoxRunCore.FirstConsumerOfJ154());
        Assert.True(ObjMonFoxRunCore.ValidatesThatPort());
        Assert.True(ObjMonFoxRunCore.CanWalkEx2LinesExtracted());
        Assert.True(ObjMonFoxRunCore.FourthArgFalse());

        Assert.Equal(2, ObjMonFoxRunCore.CanWalkEx2UseLines.Length);
        Assert.Equal(5756, ObjMonFoxRunCore.CanWalkEx2UseLines[0]);
        Assert.Equal(5784, ObjMonFoxRunCore.CanWalkEx2UseLines[1]);
    }

    [Fact]
    public void OverloadFacts()
    {
        Assert.True(ObjMonFoxRunCore.TwoOverloadsInOneMethod());
        Assert.True(ObjMonFoxRunCore.FourArgComputes());
        Assert.True(ObjMonFoxRunCore.OneArgRotates());
        Assert.True(ObjMonFoxRunCore.SameAsJ208Shape());
        Assert.True(ObjMonFoxRunCore.RotateLinesExtracted());

        Assert.Equal(2, ObjMonFoxRunCore.RotateLines.Length);
        Assert.Equal(5768, ObjMonFoxRunCore.RotateLines[0]);
        Assert.Equal(5796, ObjMonFoxRunCore.RotateLines[1]);
    }

    // ===================== 三、与 J214 的共享段 =====================

    [Fact]
    public void SharedBlockFacts()
    {
        Assert.True(ObjMonFoxRunCore.ThreeSharedBlocks());
        Assert.True(ObjMonFoxRunCore.GuardVerbatim());
        Assert.True(ObjMonFoxRunCore.WalkBlockOneCommentDiff());
        Assert.True(ObjMonFoxRunCore.MasterBlockThreeCommentDiffs());
        Assert.True(ObjMonFoxRunCore.OnlyCommentsDiffer());
        Assert.True(ObjMonFoxRunCore.NoLogicDiff());
        Assert.True(ObjMonFoxRunCore.SiblingDeclarationsAdjacent());
        Assert.True(ObjMonFoxRunCore.SharedBlocksExtracted());
        Assert.True(ObjMonFoxRunCore.TwoBlocksZeroDiff());
        Assert.True(ObjMonFoxRunCore.SharedLineCountAddsUp());
        Assert.True(ObjMonFoxRunCore.SharedDiffCountAddsUp());
        Assert.True(ObjMonFoxRunCore.SharedBlockStartsMatch());
    }

    [Fact]
    public void SharedBlockTable()
    {
        Assert.Equal(4, ObjMonFoxRunCore.SharedBlocks.Length);

        // **守卫：逐字相同**
        Assert.Equal("five-fold guard", ObjMonFoxRunCore.SharedBlocks[0].Block);
        Assert.Equal(0, ObjMonFoxRunCore.SharedBlocks[0].Diffs);

        // **搜索节流：逐字相同**
        Assert.Equal(0, ObjMonFoxRunCore.SharedBlocks[1].Diffs);

        // **行走块：18 行、1 处差异（注释）**
        Assert.Equal(18, ObjMonFoxRunCore.SharedBlocks[2].Lines);
        Assert.Equal(1, ObjMonFoxRunCore.SharedBlocks[2].Diffs);

        // **主人块：12 行、3 处差异（注释）**
        Assert.Equal(12, ObjMonFoxRunCore.SharedBlocks[3].Lines);
        Assert.Equal(3, ObjMonFoxRunCore.SharedBlocks[3].Diffs);
    }

    [Fact]
    public void GuardBoundaries()
    {
        Assert.True(ObjMonFoxRunCore.FiveFoldGuardVerbatim());
        Assert.True(ObjMonFoxRunCore.SameAsJ214());
        Assert.True(ObjMonFoxRunCore.AllTrueRuns());
        Assert.True(ObjMonFoxRunCore.AnyBlocks());

        Assert.True(ObjMonFoxRunCore.CanRun(false, false, false, false, true));
        Assert.False(ObjMonFoxRunCore.CanRun(true, false, false, false, true));
        Assert.False(ObjMonFoxRunCore.CanRun(false, true, false, false, true));
        Assert.False(ObjMonFoxRunCore.CanRun(false, false, true, false, true));
        Assert.False(ObjMonFoxRunCore.CanRun(false, false, false, true, true));
        Assert.False(ObjMonFoxRunCore.CanRun(false, false, false, false, false));
    }

    [Fact]
    public void NxCommentFacts()
    {
        Assert.True(ObjMonFoxRunCore.EmbeddedNxComment());
        Assert.True(ObjMonFoxRunCore.UniqueToThisClass());
        Assert.True(ObjMonFoxRunCore.TruckVersionIsClean());
        Assert.True(ObjMonFoxRunCore.PairingSaysNyIsCorrect());
        Assert.True(ObjMonFoxRunCore.EditResidue());
        Assert.True(ObjMonFoxRunCore.ComparesBothAxes());
        Assert.True(ObjMonFoxRunCore.NxVariantWouldBeSameAxisTwice());

        // **横纵分别比对**
        Assert.True(ObjMonFoxRunCore.TargetDiffers(0, 5, 0, 0));
        Assert.False(ObjMonFoxRunCore.TargetDiffers(0, 1, 0, 0));
    }

    // ===================== 四、Run 骨架 =====================

    [Fact]
    public void InheritedFacts()
    {
        Assert.True(ObjMonFoxRunCore.FiveInheritedSites());
        Assert.True(ObjMonFoxRunCore.FourEarlyExits());
        Assert.True(ObjMonFoxRunCore.FinalUnconditional());
        Assert.True(ObjMonFoxRunCore.OneCallPerPath());
        Assert.True(ObjMonFoxRunCore.MoreThanJ214ByOne());
        Assert.True(ObjMonFoxRunCore.InheritedTableExtracted());
        Assert.True(ObjMonFoxRunCore.ExitFollowsInherited());

        Assert.Equal(5, ObjMonFoxRunCore.InheritedLines.Length);
        Assert.Equal(5821, ObjMonFoxRunCore.InheritedLines[0]);
        Assert.Equal(5852, ObjMonFoxRunCore.InheritedLines[1]);
        Assert.Equal(5858, ObjMonFoxRunCore.InheritedLines[2]);
        Assert.Equal(5917, ObjMonFoxRunCore.InheritedLines[3]);
        Assert.Equal(5932, ObjMonFoxRunCore.InheritedLines[4]);
    }

    [Fact]
    public void OrderFacts()
    {
        Assert.True(ObjMonFoxRunCore.OrderIsSearchThinkWalk());
        Assert.True(ObjMonFoxRunCore.SearchBeforeThink());
        Assert.True(ObjMonFoxRunCore.ThinkBeforeWalkLock());
    }

    [Fact]
    public void SwitchFacts()
    {
        Assert.True(ObjMonFoxRunCore.TwoIndependentSwitches());
        Assert.True(ObjMonFoxRunCore.BothOffToAttack());
        Assert.True(ObjMonFoxRunCore.RunAwayElseOnlyExpires());
        Assert.True(ObjMonFoxRunCore.RunAwayBlocksAttack());
        Assert.True(ObjMonFoxRunCore.NoAttackBlocksAttack());
        Assert.True(ObjMonFoxRunCore.NoTargetBlocksAttack());
        Assert.True(ObjMonFoxRunCore.AllThreeAllowAttack());

        Assert.True(ObjMonFoxRunCore.CanAttack(false, false, true));
        Assert.False(ObjMonFoxRunCore.CanAttack(true, false, true));
        Assert.False(ObjMonFoxRunCore.CanAttack(false, true, true));
        Assert.False(ObjMonFoxRunCore.CanAttack(false, false, false));
    }

    [Fact]
    public void WonderingBeforeAttackBoundaries()
    {
        Assert.True(ObjMonFoxRunCore.WonderingBeforeAttack());
        Assert.True(ObjMonFoxRunCore.OneInThree());
        Assert.True(ObjMonFoxRunCore.DisplacementPreemptsAttack());
        Assert.True(ObjMonFoxRunCore.ExplainsJ215Flag());
        Assert.True(ObjMonFoxRunCore.RollZeroTriesWondering());
        Assert.True(ObjMonFoxRunCore.OtherRollsSkip());

        Assert.True(ObjMonFoxRunCore.ShouldTryWondering(0));
        Assert.False(ObjMonFoxRunCore.ShouldTryWondering(1));
        Assert.False(ObjMonFoxRunCore.ShouldTryWondering(2));
    }

    [Fact]
    public void FfebLabelFacts()
    {
        Assert.True(ObjMonFoxRunCore.FfebLabelHere());
        Assert.True(ObjMonFoxRunCore.CompletesJ215Table());
    }

    // ---------- 任务点 ----------

    [Fact]
    public void MissionFacts()
    {
        Assert.True(ObjMonFoxRunCore.MissionPointFollowing());
        Assert.True(ObjMonFoxRunCore.NegativeGuardAfterComparison());
        Assert.True(ObjMonFoxRunCore.ClampOnOverflow());
        Assert.True(ObjMonFoxRunCore.ThresholdIsThree());
        Assert.True(ObjMonFoxRunCore.SharedMechanism47Sites());
        Assert.True(ObjMonFoxRunCore.ThirtyThreePointSites());
    }

    [Fact]
    public void MissionGateBoundaries()
    {
        Assert.True(ObjMonFoxRunCore.MissionFollows());
        Assert.True(ObjMonFoxRunCore.OffMissionDoesNotFollow());
        Assert.True(ObjMonFoxRunCore.NoPointsDoesNotFollow());
        Assert.True(ObjMonFoxRunCore.NegativeIndexPassesGate());

        Assert.True(ObjMonFoxRunCore.CanFollowMission(true, 3, 0));
        Assert.False(ObjMonFoxRunCore.CanFollowMission(false, 3, 0));
        Assert.False(ObjMonFoxRunCore.CanFollowMission(true, 0, 0));

        // **负索引仍通过判据 —— 这正是后面需要那层保护的原因**
        Assert.True(ObjMonFoxRunCore.CanFollowMission(true, 3, -1));
    }

    [Fact]
    public void MissionClampBoundaries()
    {
        Assert.True(ObjMonFoxRunCore.NegativeBecomesZero());
        Assert.True(ObjMonFoxRunCore.OverflowClampsToLast());
        Assert.True(ObjMonFoxRunCore.InRangeUnchanged());

        Assert.Equal(0, ObjMonFoxRunCore.ClampNegative(-1));
        Assert.Equal(0, ObjMonFoxRunCore.ClampNegative(0));
        Assert.Equal(5, ObjMonFoxRunCore.ClampNegative(5));
        Assert.Equal(2, ObjMonFoxRunCore.ClampToLast(5, 3));
        Assert.Equal(1, ObjMonFoxRunCore.ClampToLast(1, 3));
    }

    [Fact]
    public void MissionReachBoundaries()
    {
        Assert.True(ObjMonFoxRunCore.ExactlyThreeReached());
        Assert.True(ObjMonFoxRunCore.FourNotReached());

        Assert.True(ObjMonFoxRunCore.ReachedPoint(3, 3, 0, 0));
        Assert.True(ObjMonFoxRunCore.ReachedPoint(0, 0, 0, 0));
        Assert.False(ObjMonFoxRunCore.ReachedPoint(4, 0, 0, 0));
        Assert.False(ObjMonFoxRunCore.ReachedPoint(0, 4, 0, 0));
    }

    // ---------- 主人跟随 ----------

    [Fact]
    public void MasterFollowFacts()
    {
        Assert.True(ObjMonFoxRunCore.MasterFollowBlock());
        Assert.True(ObjMonFoxRunCore.CompoundGate());
        Assert.True(ObjMonFoxRunCore.HardcodedTwenty());
        Assert.True(ObjMonFoxRunCore.SpaceMoveToMasterMap());
        Assert.True(ObjMonFoxRunCore.DifferentStrategyFromJ214Truck());
        Assert.True(ObjMonFoxRunCore.CallsOnMaster());
    }

    [Fact]
    public void SpaceMoveGateBoundaries()
    {
        Assert.True(ObjMonFoxRunCore.DifferentMapMoves());
        Assert.True(ObjMonFoxRunCore.BeyondTwentyMoves());
        Assert.True(ObjMonFoxRunCore.ExactlyTwentyDoesNotMove());
        Assert.True(ObjMonFoxRunCore.NearDoesNotMove());
        Assert.True(ObjMonFoxRunCore.RelaxBlocksSpaceMove());

        Assert.True(ObjMonFoxRunCore.ShouldSpaceMove(false, false, false, true, 0, 0));
        Assert.True(ObjMonFoxRunCore.ShouldSpaceMove(false, false, false, false, 21, 0));
        Assert.False(ObjMonFoxRunCore.ShouldSpaceMove(false, false, false, false, 20, 0));
        Assert.False(ObjMonFoxRunCore.ShouldSpaceMove(false, false, false, false, 5, 5));
    }

    [Fact]
    public void PetSwitchPolarityFacts()
    {
        // **`boPetSleepControlBySlave = False` 时游戏宠物**无视**主人休息、仍瞬移**
        Assert.True(ObjMonFoxRunCore.PetWithSwitchOffIgnoresRelax());
        Assert.True(ObjMonFoxRunCore.ShouldSpaceMove(true, true, false, false, 100, 0));

        // **开关开启且主人休息时才被挡住**
        Assert.True(ObjMonFoxRunCore.PetWithSwitchOnAndRelaxBlocks());
        Assert.False(ObjMonFoxRunCore.ShouldSpaceMove(true, true, true, false, 100, 0));

        // **开关开启但主人没休息仍可瞬移**
        Assert.True(ObjMonFoxRunCore.SwitchOnWithoutRelaxStillMoves());

        // **两项互为反面**
        Assert.True(ObjMonFoxRunCore.SwitchPolarityIsOpposite());
    }

    [Fact]
    public void SpaceMoveParamFacts()
    {
        Assert.True(ObjMonFoxRunCore.FourthParamIsOne());
        Assert.True(ObjMonFoxRunCore.NamedNIntUninformative());
        Assert.True(ObjMonFoxRunCore.SemanticallyOpaqueParam());

        Assert.Equal(1, ObjMonFoxRunCore.SpaceMoveFourthArg);
        Assert.Equal(748, ObjMonFoxRunCore.SpaceMoveDeclLine);
    }

    // ---------- 收尾 ----------

    [Fact]
    public void FinishFacts()
    {
        Assert.True(ObjMonFoxRunCore.GotoTargetParenInconsistency());
        Assert.True(ObjMonFoxRunCore.SameClassTwoStyles());
        Assert.True(ObjMonFoxRunCore.ParenCallHasExtraComment());
        Assert.True(ObjMonFoxRunCore.WonderingIsNotWonderingEx());
        Assert.True(ObjMonFoxRunCore.BaseVirtualWondering());
        Assert.True(ObjMonFoxRunCore.TwoCommentsOnOneLine());
        Assert.True(ObjMonFoxRunCore.NameDiffersByExOnly());
        Assert.True(ObjMonFoxRunCore.GotoDeclChecked());
        Assert.True(ObjMonFoxRunCore.WithTargetGoes());
        Assert.True(ObjMonFoxRunCore.NoTargetWanders());
        Assert.True(ObjMonFoxRunCore.NoTargetNoWander());

        // **两处不带括号、一处带括号**
        Assert.Equal(2, ObjMonFoxRunCore.GotoNoParenLines.Length);
        Assert.Equal(5762, ObjMonFoxRunCore.GotoNoParenLines[0]);
        Assert.Equal(5790, ObjMonFoxRunCore.GotoNoParenLines[1]);
        Assert.Equal(5922, ObjMonFoxRunCore.GotoWithParenLine);
    }

    [Fact]
    public void FinishDispatch()
    {
        Assert.Equal("goto", ObjMonFoxRunCore.PickFinish(5, false));
        Assert.Equal("wondering", ObjMonFoxRunCore.PickFinish(-1, false));
        Assert.Equal("none", ObjMonFoxRunCore.PickFinish(-1, true));
    }

    [Fact]
    public void WonderingClearTable()
    {
        Assert.Equal(2, ObjMonFoxRunCore.WonderingClearLines.Length);
        Assert.Equal(5763, ObjMonFoxRunCore.WonderingClearLines[0]);
        Assert.Equal(5791, ObjMonFoxRunCore.WonderingClearLines[1]);

        Assert.Equal(2, ObjMonFoxRunCore.GotoNoParenLines.Length);
        Assert.Equal(2, ObjMonFoxRunCore.RandomNineLines.Length);
        Assert.Equal(2, ObjMonFoxRunCore.CanWalkEx2UseLines.Length);
        Assert.Equal(2, ObjMonFoxRunCore.CountIncLines.Length);
        Assert.Equal(2, ObjMonFoxRunCore.CountCheckLines.Length);
    }

    // ===================== 五、块尾注释 =====================

    [Fact]
    public void BlockCommentFacts()
    {
        Assert.True(ObjMonFoxRunCore.TwelveBlockEndComments());
        Assert.True(ObjMonFoxRunCore.ThreeCarryDeletedPredicates());
        Assert.True(ObjMonFoxRunCore.BlockEndTableExtracted());
        Assert.True(ObjMonFoxRunCore.DeletedPredicateTableExtracted());
        Assert.True(ObjMonFoxRunCore.AllBlockEndCommentsInsideRun());
        Assert.True(ObjMonFoxRunCore.DeletedPredicatesAreBlockEnds());
        Assert.True(ObjMonFoxRunCore.StandaloneRemovedConditionComment());
        Assert.True(ObjMonFoxRunCore.OffsetStyleNames());
        Assert.True(ObjMonFoxRunCore.SameFamilyAsJ159AndJ213());
        Assert.True(ObjMonFoxRunCore.RemovedConditionBeforeEnd());

        Assert.Equal(12, ObjMonFoxRunCore.BlockEndCommentLines.Length);
        Assert.Equal(5841, ObjMonFoxRunCore.BlockEndCommentLines[0]);
        Assert.Equal(5931, ObjMonFoxRunCore.BlockEndCommentLines[11]);

        Assert.Equal(3, ObjMonFoxRunCore.DeletedPredicateCommentLines.Length);
        Assert.Equal(5880, ObjMonFoxRunCore.DeletedPredicateCommentLines[0]);
        Assert.Equal(5899, ObjMonFoxRunCore.DeletedPredicateCommentLines[1]);
        Assert.Equal(5905, ObjMonFoxRunCore.DeletedPredicateCommentLines[2]);
    }

    // ===================== 六、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonFoxRunCore.TwentyClassesCovered());
        Assert.True(ObjMonFoxRunCore.RemainingApprox());
    }
}
