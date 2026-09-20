using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J214：`ObjMon.pas` 中 `TTruckMonster`（押镖车）
/// 两个方法 1:1 测试（合计 138 行）。
/// **本批最有价值的发现**：`Create` 把 `m_btRaceServer` 设成 `122`，
/// 而镖车的种族常量 `RC_TRUCKOBJECT` 是 **128** ——
/// 一个数字之差，使全工程十六处"对镖车的特殊处理"全部落空
/// （守卫会打镖车、摆摊保护失效、跨图跟随成为死代码）。
/// </summary>
public sealed class ObjMonTruckCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(5388, ObjMonTruckCore.CreateStart);
        Assert.Equal(5399, ObjMonTruckCore.CreateEnd);
        Assert.Equal(12, ObjMonTruckCore.CreateLines);
        Assert.Equal(5401, ObjMonTruckCore.RunStart);
        Assert.Equal(5526, ObjMonTruckCore.RunEnd);
        Assert.Equal(126, ObjMonTruckCore.RunLines);
        Assert.Equal(138, ObjMonTruckCore.TotalLines);

        Assert.Equal(122, ObjMonTruckCore.RaceWritten);
        Assert.Equal(128, ObjMonTruckCore.RC_TRUCKOBJECT);
        Assert.Equal(5393, ObjMonTruckCore.RaceAssignLine);
        Assert.Equal(201, ObjMonTruckCore.RaceConstantLine);
        Assert.Equal(1, ObjMonTruckCore.Race122Sites);
        Assert.Equal(33097, ObjMonTruckCore.ObjBaseGateReadLine);
        Assert.Equal(33315, ObjMonTruckCore.ObjBaseGateSetStart);
        Assert.Equal(33317, ObjMonTruckCore.ObjBaseGateSetEnd);
        Assert.Equal(33322, ObjMonTruckCore.ObjBaseMasterClearLine);
        Assert.Equal(33312, ObjMonTruckCore.ObjBaseRaceCheckLine);
        Assert.Equal(20, ObjMonTruckCore.NCodeFirst);
        Assert.Equal(28, ObjMonTruckCore.NCodeLast);
        Assert.Equal(33317, ObjMonTruckCore.EnterAnotherTrueLine);

        Assert.Equal(5391, ObjMonTruckCore.ViewRangeLine);
        Assert.Equal(9, ObjMonTruckCore.ViewRange);
        Assert.Equal(6, ObjMonTruckCore.OldViewRange);
        Assert.Equal(250, ObjMonTruckCore.RunTime);
        Assert.Equal(5537, ObjMonTruckCore.FoxRunTimeLine);
        Assert.Equal(5395, ObjMonTruckCore.SendRefMsgTickLine);
        Assert.Equal(2, ObjMonTruckCore.SendRefMsgTickSites);

        Assert.Equal(5405, ObjMonTruckCore.DelTargetLine);
        Assert.Equal(50, ObjMonTruckCore.DelTargetSites);
        Assert.Equal(5406, ObjMonTruckCore.GuardLine);
        Assert.Equal(5412, ObjMonTruckCore.Exit1Line);
        Assert.Equal(5417, ObjMonTruckCore.Exit2Line);
        Assert.Equal(5414, ObjMonTruckCore.ThinkLine);
        Assert.Equal(5525, ObjMonTruckCore.FinalInheritedLine);
        Assert.Equal(5421, ObjMonTruckCore.WaitLockRawSubLine);
        Assert.Equal(5426, ObjMonTruckCore.WalkCooldownLine);
        Assert.Equal(5431, ObjMonTruckCore.WalkCountCheckLine);
        Assert.Equal(5515, ObjMonTruckCore.RunAwayTimeoutLine);
        Assert.Equal(5444, ObjMonTruckCore.GetBackPositionLine);
        Assert.Equal(5521, ObjMonTruckCore.TargetSentinelLine);
        Assert.Equal(-1, ObjMonTruckCore.TargetSentinel);

        Assert.Equal(8285, ObjMonTruckCore.FormatStringLine);
        Assert.Equal(4, ObjMonTruckCore.FormatSpecifiers);
        Assert.Equal(2, ObjMonTruckCore.OldCallArgs);
        Assert.Equal(4, ObjMonTruckCore.NewCallArgs);
        Assert.Equal(19, ObjMonTruckCore.ClassesCovered);
        Assert.Equal(35, ObjMonTruckCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonTruckCore.SpanMatches());
        Assert.True(ObjMonTruckCore.TotalLinesAddUp());
        Assert.True(ObjMonTruckCore.CreateBeforeRun());
        Assert.True(ObjMonTruckCore.MethodsContiguous());
        Assert.True(ObjMonTruckCore.WithinUnit());
        Assert.True(ObjMonTruckCore.NoInstrumentation());
    }

    // ===================== 一、122 / 128 =====================

    [Fact]
    public void RaceTypoFacts()
    {
        Assert.True(ObjMonTruckCore.RaceIs122());
        Assert.True(ObjMonTruckCore.ConstantIs128());
        Assert.True(ObjMonTruckCore.SingleDigitTypo());
        Assert.True(ObjMonTruckCore.TensMatchOnesDiffer());
        Assert.True(ObjMonTruckCore.OnlyOne122Site());
        Assert.True(ObjMonTruckCore.NotARaceConstant());
        Assert.True(ObjMonTruckCore.ConstantExistsButUnused());
        Assert.True(ObjMonTruckCore.AllConsumersUse128());
        Assert.True(ObjMonTruckCore.NoneMatch());
    }

    [Fact]
    public void RaceTypoBoundaries()
    {
        // **十位相同（都是 12x）、个位不同（2 vs 8）—— 单个数字之差**
        Assert.Equal(12, ObjMonTruckCore.RC_TRUCKOBJECT / 10);
        Assert.Equal(12, ObjMonTruckCore.RaceWritten / 10);
        Assert.Equal(8, ObjMonTruckCore.RC_TRUCKOBJECT % 10);
        Assert.Equal(2, ObjMonTruckCore.RaceWritten % 10);

        // **真正的镖车不被认作镖车、修正后才会**
        Assert.True(ObjMonTruckCore.RealTruckNotRecognised());
        Assert.True(ObjMonTruckCore.WouldBeRecognisedIfCorrect());

        Assert.True(ObjMonTruckCore.IsTruck(128));
        Assert.False(ObjMonTruckCore.IsTruck(122));
    }

    [Fact]
    public void ConsumerFacts()
    {
        Assert.True(ObjMonTruckCore.ConsumersExtracted());
        Assert.True(ObjMonTruckCore.ConsumerCountsAddUp());
        Assert.True(ObjMonTruckCore.TwelvePlusDeadSites());
        Assert.True(ObjMonTruckCore.ConsumersSpanFiles());
        Assert.True(ObjMonTruckCore.GuardWillAttackTruck());
        Assert.True(ObjMonTruckCore.GuardWouldSpareIfCorrect());
        Assert.True(ObjMonTruckCore.StallProtectionInert());

        Assert.Equal(2, ObjMonTruckCore.GuardRaceCheckLines.Length);
        Assert.Equal(126, ObjMonTruckCore.GuardRaceCheckLines[0]);
        Assert.Equal(276, ObjMonTruckCore.GuardRaceCheckLines[1]);
        Assert.Equal(4, ObjMonTruckCore.Mon2RaceCheckLines.Length);
        Assert.Equal(1687, ObjMonTruckCore.Mon2RaceCheckLines[0]);
        Assert.Equal(3, ObjMonTruckCore.StallGuardLines.Length);
        Assert.Equal(22340, ObjMonTruckCore.StallGuardLines[0]);
    }

    [Fact]
    public void GateBranchDeadFacts()
    {
        Assert.True(ObjMonTruckCore.GateBranchIsDead());
        Assert.True(ObjMonTruckCore.OnlyTrueSetterGated());
        Assert.True(ObjMonTruckCore.TrueSetterAfterCheck());
        Assert.True(ObjMonTruckCore.AlwaysFalse());
        Assert.True(ObjMonTruckCore.ThreeFalseZeroTrue());
        Assert.True(ObjMonTruckCore.EnterAnotherTableExtracted());
        Assert.True(ObjMonTruckCore.OneRead());
        Assert.True(ObjMonTruckCore.AlwaysWarnsInsteadOfFollowing());
        Assert.True(ObjMonTruckCore.NeverFollows());
        Assert.True(ObjMonTruckCore.WouldFollowIfRaceCorrect());
    }

    [Fact]
    public void EnterAnotherMapTable()
    {
        Assert.Equal(5, ObjMonTruckCore.EnterAnotherSites.Length);
        Assert.Equal(173, ObjMonTruckCore.EnterAnotherSites[0].Line);
        Assert.Equal("declare", ObjMonTruckCore.EnterAnotherSites[0].Kind);
        Assert.Equal("assign-false", ObjMonTruckCore.EnterAnotherSites[1].Kind);
        Assert.Equal("assign-false", ObjMonTruckCore.EnterAnotherSites[2].Kind);
        Assert.Equal("read", ObjMonTruckCore.EnterAnotherSites[3].Kind);
        Assert.Equal(5461, ObjMonTruckCore.EnterAnotherSites[3].Line);
        Assert.Equal("assign-false", ObjMonTruckCore.EnterAnotherSites[4].Kind);
        Assert.Equal(3, ObjMonTruckCore.EnterAnotherFalseAssigns);
    }

    [Fact]
    public void GateFieldTable()
    {
        Assert.True(ObjMonTruckCore.GateFieldsEffectivelyConstant());
        Assert.True(ObjMonTruckCore.SetterExistsButGated());
        Assert.True(ObjMonTruckCore.CorrectedInitialJudgement());
        Assert.True(ObjMonTruckCore.FixDiffersFromJ205Shape());
        Assert.True(ObjMonTruckCore.GateTableExtracted());
        Assert.True(ObjMonTruckCore.NoRealAssignmentInThisUnit());
        Assert.True(ObjMonTruckCore.AllAssignsAreMinusOne());

        Assert.Equal(5, ObjMonTruckCore.GateSites.Length);
        Assert.Equal(174, ObjMonTruckCore.GateSites[0].Line);
        Assert.Equal(5397, ObjMonTruckCore.GateSites[1].Line);
        Assert.Equal(5464, ObjMonTruckCore.GateSites[3].Line);
        Assert.Equal("read", ObjMonTruckCore.GateSites[3].Kind);
    }

    [Fact]
    public void CrossUnitFacts()
    {
        Assert.True(ObjMonTruckCore.CrossUnitFieldWrite());
        Assert.True(ObjMonTruckCore.HardCastFromObjBase());
        Assert.True(ObjMonTruckCore.FieldsInPublicSection());
        Assert.True(ObjMonTruckCore.LayeringInversion());
        Assert.True(ObjMonTruckCore.ChannelForTheRaceBug());
        Assert.True(ObjMonTruckCore.DistanceGate());
        Assert.True(ObjMonTruckCore.UsesMastersViewRange());
        Assert.True(ObjMonTruckCore.NotTrucksOwn());
        Assert.True(ObjMonTruckCore.NCodeLadder());
        Assert.True(ObjMonTruckCore.NineSteps());
        Assert.True(ObjMonTruckCore.AbsentInThisBatch());
    }

    // ===================== 二、Create =====================

    [Fact]
    public void ViewRangeEvolutionFacts()
    {
        Assert.True(ObjMonTruckCore.ViewRangeNine());
        Assert.True(ObjMonTruckCore.OldValueInComment());
        Assert.True(ObjMonTruckCore.CommentIsTheOldValue());
        Assert.True(ObjMonTruckCore.UniqueInFile());
        Assert.True(ObjMonTruckCore.FirstOfItsKind());
        Assert.True(ObjMonTruckCore.EvolutionWitness());
        Assert.True(ObjMonTruckCore.ViewRangeIncreased());
        Assert.True(ObjMonTruckCore.FiftyPercentIncrease());

        // **视野从 6 改成 9、旧值留在行尾注释里**
        Assert.Equal(9, ObjMonTruckCore.ViewRange);
        Assert.Equal(6, ObjMonTruckCore.OldViewRange);
        Assert.True(ObjMonTruckCore.ViewRange > ObjMonTruckCore.OldViewRange);
    }

    [Fact]
    public void WriteOnlyFieldFacts()
    {
        Assert.True(ObjMonTruckCore.WriteOnlyField());
        Assert.True(ObjMonTruckCore.TwoSitesOnly());
        Assert.True(ObjMonTruckCore.IntentWasTimeThrottle());
        Assert.True(ObjMonTruckCore.ActualIsBooleanThrottle());
        Assert.True(ObjMonTruckCore.LeftoverField());
        Assert.True(ObjMonTruckCore.AssignedInCreate());

        Assert.Equal(2, ObjMonTruckCore.SendRefMsgTickSites);
    }

    [Fact]
    public void WarningFlagFacts()
    {
        Assert.True(ObjMonTruckCore.SendRefMsgTablesExtracted());
        Assert.True(ObjMonTruckCore.ReadsMatchSets());
        Assert.True(ObjMonTruckCore.SetFollowsRead());
        Assert.True(ObjMonTruckCore.ClearedOnSuccessPathsOnly());
        Assert.True(ObjMonTruckCore.NotClearedOnFarPath());
        Assert.True(ObjMonTruckCore.CHalfClears());
        Assert.True(ObjMonTruckCore.PairedClearsInA());
        Assert.True(ObjMonTruckCore.AClearsAdjacent());
        Assert.True(ObjMonTruckCore.SingleClearInB());
        Assert.True(ObjMonTruckCore.Asymmetry());
        Assert.True(ObjMonTruckCore.OneShotFlag());

        Assert.Equal(2, ObjMonTruckCore.SendRefMsgReadLines.Length);
        Assert.Equal(2, ObjMonTruckCore.SendRefMsgSetLines.Length);
        Assert.Equal(3, ObjMonTruckCore.SendRefMsgClearLines.Length);
        Assert.Equal(5486, ObjMonTruckCore.SendRefMsgReadLines[0]);
        Assert.Equal(5503, ObjMonTruckCore.SendRefMsgReadLines[1]);
    }

    [Fact]
    public void WarningBoundaries()
    {
        Assert.True(ObjMonTruckCore.WarnsFirstTime());
        Assert.True(ObjMonTruckCore.SilentSecondTime());
        Assert.True(ObjMonTruckCore.ShouldWarn(false));
        Assert.False(ObjMonTruckCore.ShouldWarn(true));
    }

    [Fact]
    public void FormatStringFacts()
    {
        Assert.True(ObjMonTruckCore.FourSpecifiers());
        Assert.True(ObjMonTruckCore.OldCallHadTwoArgs());
        Assert.True(ObjMonTruckCore.NewCallHasFourArgs());
        Assert.True(ObjMonTruckCore.CurrentCallMatches());
        Assert.True(ObjMonTruckCore.OldCallMismatches());
        Assert.True(ObjMonTruckCore.FormatMismatchIfRestored());
        Assert.True(ObjMonTruckCore.TwoCommentedOldCalls());
        Assert.True(ObjMonTruckCore.NotDeprecatedButStale());
        Assert.True(ObjMonTruckCore.WarnFollowsComment());
        Assert.True(ObjMonTruckCore.TwoWarnSites());
        Assert.True(ObjMonTruckCore.ReportsTruckPosition());
        Assert.True(ObjMonTruckCore.SentToMaster());
        Assert.True(ObjMonTruckCore.OneWayInformation());

        // **现行四参对得上四个占位符、旧的两参对不上**
        Assert.Equal(ObjMonTruckCore.FormatSpecifiers, ObjMonTruckCore.NewCallArgs);
        Assert.NotEqual(ObjMonTruckCore.FormatSpecifiers, ObjMonTruckCore.OldCallArgs);
    }

    // ===================== 三、Run 骨架 =====================

    [Fact]
    public void GuardFacts()
    {
        Assert.True(ObjMonTruckCore.FiveFoldGuard());
        Assert.True(ObjMonTruckCore.MostTermsSoFar());
        Assert.True(ObjMonTruckCore.CombinesJ204AndJ213Fields());
        Assert.True(ObjMonTruckCore.AllTrueRuns());
        Assert.True(ObjMonTruckCore.GhostBlocks());
        Assert.True(ObjMonTruckCore.DeathBlocks());
        Assert.True(ObjMonTruckCore.FixedHideBlocks());
        Assert.True(ObjMonTruckCore.StoneBlocks());
        Assert.True(ObjMonTruckCore.CannotMoveBlocks());
        Assert.True(ObjMonTruckCore.GhostFirst());
    }

    [Fact]
    public void GuardBoundaries()
    {
        Assert.True(ObjMonTruckCore.CanRun(false, false, false, false, true));
        Assert.False(ObjMonTruckCore.CanRun(true, false, false, false, true));
        Assert.False(ObjMonTruckCore.CanRun(false, true, false, false, true));
        Assert.False(ObjMonTruckCore.CanRun(false, false, true, false, true));
        Assert.False(ObjMonTruckCore.CanRun(false, false, false, true, true));
        Assert.False(ObjMonTruckCore.CanRun(false, false, false, false, false));
    }

    [Fact]
    public void DelTargetAndExitFacts()
    {
        Assert.True(ObjMonTruckCore.NoParenCall());
        Assert.True(ObjMonTruckCore.FiftySites());
        Assert.True(ObjMonTruckCore.BothStylesCoexist());
        Assert.True(ObjMonTruckCore.StyleInconsistency());

        Assert.True(ObjMonTruckCore.TwoEarlyExits());
        Assert.True(ObjMonTruckCore.EachCallsInherited());
        Assert.True(ObjMonTruckCore.FinalInheritedUnconditional());
        Assert.True(ObjMonTruckCore.NoDoubleCall());
        Assert.True(ObjMonTruckCore.ThreeInheritedSites());
    }

    [Fact]
    public void RelaxCheckBoundaries()
    {
        Assert.True(ObjMonTruckCore.RelaxCheckHasThreeTerms());
        Assert.True(ObjMonTruckCore.RelaxExits());
        Assert.True(ObjMonTruckCore.NoMasterNoRelax());
        Assert.True(ObjMonTruckCore.NonPetIgnoresSwitch());
        Assert.True(ObjMonTruckCore.PetNeedsSwitchWhenPet());
        Assert.True(ObjMonTruckCore.PetWithSwitchRelaxes());

        Assert.True(ObjMonTruckCore.ShouldRelax(true, true, false, false));
        Assert.False(ObjMonTruckCore.ShouldRelax(true, true, true, false));
        Assert.True(ObjMonTruckCore.ShouldRelax(true, true, true, true));
        Assert.False(ObjMonTruckCore.ShouldRelax(false, true, false, false));
        Assert.False(ObjMonTruckCore.ShouldRelax(true, false, false, false));
    }

    [Fact]
    public void ThinkFacts()
    {
        Assert.True(ObjMonTruckCore.ThinkAsCondition());
        Assert.True(ObjMonTruckCore.NotOverriddenHere());
        Assert.True(ObjMonTruckCore.InheritedFromAnimalObject());
        Assert.True(ObjMonTruckCore.MeansAlreadyHandled());
        Assert.True(ObjMonTruckCore.ThinkTrueExits());
    }

    [Fact]
    public void WalkThrottleFacts()
    {
        Assert.True(ObjMonTruckCore.TwoTierWalkThrottle());
        Assert.True(ObjMonTruckCore.RawSubtractForWaitLock());
        Assert.True(ObjMonTruckCore.TickDiffForWalkCooldown());
        Assert.True(ObjMonTruckCore.TwoTimeIdioms());
        Assert.True(ObjMonTruckCore.ThreeRawSubtractions());
        Assert.True(ObjMonTruckCore.GreaterHere());
        Assert.True(ObjMonTruckCore.GreaterEqualInJ213());
        Assert.True(ObjMonTruckCore.BoundaryDiffersByOne());
        Assert.True(ObjMonTruckCore.BoundaryOpposite());
    }

    [Fact]
    public void WalkCooldownBoundaries()
    {
        // **本类用 `>`：恰好等阈值不允许移动**
        Assert.False(ObjMonTruckCore.WalkCooldownHere(1000, 1500, 500, 0));

        // **J213 用 `>=`：恰好等阈值允许移动**
        Assert.True(ObjMonTruckCore.WalkCooldownJ213(1000, 1500, 500, 0));

        // **超一毫秒则两版都允许**
        Assert.True(ObjMonTruckCore.WalkCooldownHere(1000, 1501, 500, 0));
        Assert.True(ObjMonTruckCore.WalkCooldownJ213(1000, 1501, 500, 0));
    }

    [Fact]
    public void WaitLockBoundaries()
    {
        // **等待未满则仍锁**
        Assert.True(ObjMonTruckCore.WaitLockHeld());
        Assert.False(ObjMonTruckCore.WaitLockExpired(1000, 1500, 600));

        // **等待已满则解锁**
        Assert.True(ObjMonTruckCore.UnlocksAfterWait());
        Assert.True(ObjMonTruckCore.WaitLockExpired(1000, 1700, 600));

        // **恰好等于等待时间仍算未过期（严格大于）**
        Assert.False(ObjMonTruckCore.WaitLockExpired(1000, 1600, 600));
    }

    [Fact]
    public void WalkStepBoundaries()
    {
        Assert.True(ObjMonTruckCore.WalkStepCounter());
        Assert.True(ObjMonTruckCore.GreaterNotGreaterEqual());
        Assert.True(ObjMonTruckCore.OvershootsByOne());
        Assert.True(ObjMonTruckCore.ZeroNotMeaningNever());

        // **判据是 `>`：恰好等于步数不休息**
        Assert.False(ObjMonTruckCore.StepLimitReached(5, 5));
        Assert.True(ObjMonTruckCore.StepLimitReached(6, 5));
        Assert.True(ObjMonTruckCore.ExactlyStepDoesNotRest());
        Assert.True(ObjMonTruckCore.OneOverRests());

        // **步数为 0 时第一步就休息（不是"永不休息"）**
        Assert.True(ObjMonTruckCore.StepLimitReached(1, 0));
        Assert.True(ObjMonTruckCore.ZeroStepMeansAlwaysRest());
    }

    [Fact]
    public void RunAwayFacts()
    {
        Assert.True(ObjMonTruckCore.ThirdRawSubtraction());
        Assert.True(ObjMonTruckCore.RunAwayTimeout());
        Assert.True(ObjMonTruckCore.InsideElseOfRunAwayFlag());
        Assert.True(ObjMonTruckCore.SameFieldAsJ198());
        Assert.True(ObjMonTruckCore.ExpiryClears());

        // **时长为 0 则永不超时**
        Assert.True(ObjMonTruckCore.ZeroDurationNeverExpires());
        Assert.False(ObjMonTruckCore.RunAwayExpired(0, 999999, 0));
        Assert.True(ObjMonTruckCore.RunAwayExpired(1000, 2000, 500));
    }

    // ===================== 四、三路分派 =====================

    [Fact]
    public void DispatchFacts()
    {
        Assert.True(ObjMonTruckCore.ThreeWayDispatch());
        Assert.True(ObjMonTruckCore.ThreeWayExclusive());
        Assert.True(ObjMonTruckCore.CallsGetBackPositionOnMaster());
        Assert.True(ObjMonTruckCore.FirstCrossObjectCall());
        Assert.True(ObjMonTruckCore.StandsBehindMaster());
    }

    [Fact]
    public void DispatchBoundaries()
    {
        Assert.True(ObjMonTruckCore.SameMapInViewGoesA());
        Assert.True(ObjMonTruckCore.OtherMapGoesB());
        Assert.True(ObjMonTruckCore.SameMapOutOfViewGoesC());
        Assert.True(ObjMonTruckCore.NoMasterGoesC());

        Assert.Equal("A", ObjMonTruckCore.PickBranch(true, true, true));
        Assert.Equal("B", ObjMonTruckCore.PickBranch(true, false, false));
        Assert.Equal("C", ObjMonTruckCore.PickBranch(true, true, false));
        Assert.Equal("C", ObjMonTruckCore.PickBranch(false, false, false));
    }

    [Fact]
    public void MasterViewBoundaries()
    {
        Assert.True(ObjMonTruckCore.ExactlyAtViewBoundary());
        Assert.True(ObjMonTruckCore.BeyondViewFails());

        Assert.True(ObjMonTruckCore.WithinMasterView(9, 9, 9));
        Assert.True(ObjMonTruckCore.WithinMasterView(0, 0, 9));
        Assert.False(ObjMonTruckCore.WithinMasterView(10, 0, 9));
        Assert.False(ObjMonTruckCore.WithinMasterView(0, 10, 9));
    }

    [Fact]
    public void BranchClearFacts()
    {
        Assert.True(ObjMonTruckCore.BranchAClearsTwo());
        Assert.True(ObjMonTruckCore.BranchBClearsOne());
        Assert.True(ObjMonTruckCore.BranchCClearsOne());
    }

    [Fact]
    public void DuplicatedBlockFacts()
    {
        Assert.True(ObjMonTruckCore.DuplicatedThirteenLines());
        Assert.True(ObjMonTruckCore.OnlySourceDiffers());
        Assert.True(ObjMonTruckCore.SameBlockLength());
        Assert.True(ObjMonTruckCore.BlocksAreThirteenLines());
        Assert.True(ObjMonTruckCore.VerbatimSame());

        Assert.Equal(13, ObjMonTruckCore.DupALines);
        Assert.Equal(13, ObjMonTruckCore.DupBLines);
        Assert.Equal(5445, ObjMonTruckCore.DupAStart);
        Assert.Equal(5457, ObjMonTruckCore.DupAEnd);
        Assert.Equal(5466, ObjMonTruckCore.DupBStart);
        Assert.Equal(5478, ObjMonTruckCore.DupBEnd);
    }

    [Fact]
    public void RetargetBoundaries()
    {
        // **相差超过一格才更新目标**
        Assert.True(ObjMonTruckCore.DifferByTwoRetargets());
        Assert.True(ObjMonTruckCore.ShouldRetarget(0, 2, 0, 0));
        // **`DifferByOneDoesNot` 为真表示"相差一格确实不重设目标"**
        Assert.True(ObjMonTruckCore.DifferByOneDoesNot());
        Assert.False(ObjMonTruckCore.ShouldRetarget(0, 1, 0, 0));
        Assert.False(ObjMonTruckCore.ShouldRetarget(0, 0, 0, 0));

        Assert.True(ObjMonTruckCore.IsTileOccupied(true));
        Assert.False(ObjMonTruckCore.IsTileOccupied(false));
        Assert.True(ObjMonTruckCore.OccupiedStaysPut());
        Assert.True(ObjMonTruckCore.WithinTwoCheck());
    }

    [Fact]
    public void SentinelFacts()
    {
        Assert.True(ObjMonTruckCore.MinusOneSentinel());
        Assert.True(ObjMonTruckCore.ConsistentWithJ206J208J213());
        Assert.True(ObjMonTruckCore.ChecksOnlyX());
        Assert.True(ObjMonTruckCore.HarmlessDueToPairedAssignment());
        Assert.True(ObjMonTruckCore.MinusOneMeansNone());
        Assert.True(ObjMonTruckCore.OtherMeansSet());
        Assert.True(ObjMonTruckCore.ThreeTargetSets());

        Assert.True(ObjMonTruckCore.HasTarget(0));
        Assert.False(ObjMonTruckCore.HasTarget(-1));
    }

    // ===================== 五、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonTruckCore.NineteenClassesCovered());
        Assert.True(ObjMonTruckCore.RemainingApprox());
        Assert.True(ObjMonTruckCore.RunTime250());
        Assert.True(ObjMonTruckCore.SharedWithFox());
        Assert.True(ObjMonTruckCore.FieldsDefaultVisibility());
        Assert.True(ObjMonTruckCore.OneLineTwoVars());
        Assert.True(ObjMonTruckCore.ContrastWithJ213Layout());
    }
}
