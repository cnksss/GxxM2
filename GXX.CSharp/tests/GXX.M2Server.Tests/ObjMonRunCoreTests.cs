using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J198：`ObjMon.pas` 中生效版 `TMonster.Run` 1:1 测试（259 行）。
/// **核心是"同一条件被复制多遍、正负混用"**：魔王岭宝宝条件出现三次、
/// 宠物使能表达式逐字出现两次、主人"放松"判据出现三次且后果各异。
/// 另记两处真实不一致：同一方法内 `tick_diff` 与裸减法并存；
/// 1301 的"横纵都不等"判据与防重叠的通常写法不对称。
/// </summary>
public sealed class ObjMonRunCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(1121, ObjMonRunCore.RunStart);
        Assert.Equal(1379, ObjMonRunCore.RunEnd);
        Assert.Equal(259, ObjMonRunCore.RunLines);
        Assert.Equal(934, ObjMonRunCore.OldRunStart);
        Assert.Equal(933, ObjMonRunCore.CommentOpen);
        Assert.Equal(1119, ObjMonRunCore.CommentClose);
        Assert.Equal(1120, ObjMonRunCore.VersionNoteLine);
        Assert.Equal(4, ObjMonRunCore.InheritedCount);
        Assert.Equal(3, ObjMonRunCore.EarlyReturnCount);
        Assert.Equal(4, ObjMonRunCore.BraceAnnotationCount);
        Assert.Equal(3, ObjMonRunCore.GuardianPetGuardCount);
        Assert.Equal(2, ObjMonRunCore.PetEnableRepeatCount);
        Assert.Equal(3, ObjMonRunCore.RelaxGuardCount);
        Assert.Equal(1, ObjMonRunCore.moNoMove);
        Assert.Equal(0, ObjMonRunCore.moMoveNormal);
        Assert.Equal(2, ObjMonRunCore.moProtect);
        Assert.Equal(155, ObjMonRunCore.GuardianRaceLiteral);
        Assert.Equal(156, ObjMonRunCore.GuardianRaceImgLiteral);
        Assert.Equal(0, ObjMonRunCore.RC_PLAYOBJECT);
        Assert.Equal(1, ObjMonRunCore.RC_HEROOBJECT);
        Assert.Equal(1, ObjMonRunCore.TargetDiffThreshold);
        Assert.Equal(20, ObjMonRunCore.MasterDistanceThreshold);
        Assert.Equal(3, ObjMonRunCore.MissionArrivalRadius);
        Assert.Equal(-1, ObjMonRunCore.NoTargetSentinel);
        Assert.Equal(0, ObjMonRunCore.PetEnableFollowConfig);
        Assert.Equal(1, ObjMonRunCore.PetEnableForceOn);
        Assert.True(ObjMonRunCore.MovingObjectCheckThirdParam);
        Assert.Equal(7, ObjMonRunCore.NestingDepth);
        Assert.Equal(1245, ObjMonRunCore.StartPickUpThreeArgLine);
        Assert.Equal(1169, ObjMonRunCore.StartPickUpFourArgLine);
        Assert.Equal(1330, ObjMonRunCore.RawSubtractionLine);
        Assert.Equal(1195, ObjMonRunCore.TickDiffLine);
        Assert.Equal(1301, ObjMonRunCore.BothAxesDifferLine);
        Assert.Equal(185, ObjMonRunCore.OldRunContentLines);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonRunCore.SpanMatches());
        Assert.True(ObjMonRunCore.RunLinesIs259());
        Assert.True(ObjMonRunCore.AfterCommentRegion());
        Assert.True(ObjMonRunCore.VersionNoteAdjacent());
        Assert.True(ObjMonRunCore.OldBeforeNew());
        Assert.True(ObjMonRunCore.WithinUnit());
        Assert.True(ObjMonRunCore.NoInstrumentation());
    }

    // ===================== 一、计数 =====================

    [Fact]
    public void InheritedSites()
    {
        Assert.True(ObjMonRunCore.FourInheritedSites());
        Assert.True(ObjMonRunCore.ThreeEarlyReturns());
        Assert.True(ObjMonRunCore.OneFinalFallthrough());
        Assert.True(ObjMonRunCore.InheritedSitesExtracted());
        Assert.True(ObjMonRunCore.FinalInheritedAt1378());

        Assert.Equal(new[] { 1146, 1200, 1338, 1378 }, ObjMonRunCore.InheritedSites);
    }

    [Fact]
    public void BraceAnnotations()
    {
        Assert.True(ObjMonRunCore.FourBraceAnnotations());
        Assert.True(ObjMonRunCore.AllInlineNotWholeLine());
        Assert.True(ObjMonRunCore.BraceHidesNoCode());
        Assert.True(ObjMonRunCore.StaleVariableNameHint());
        Assert.True(ObjMonRunCore.BraceSitesExtracted());

        Assert.Equal(new[] { 1198, 1290, 1297, 1314 }, ObjMonRunCore.BraceSites);
    }

    // ===================== 二、魔王岭三重绕过 =====================

    [Fact]
    public void GuardianGuards()
    {
        Assert.True(ObjMonRunCore.GuardedThreeTimes());
        Assert.True(ObjMonRunCore.OnePositiveOneNegative());
        Assert.True(ObjMonRunCore.EmptyThenBranch());
        Assert.True(ObjMonRunCore.InvertedViaEmptyBlock());
        Assert.True(ObjMonRunCore.GuardianSitesExtracted());
        Assert.True(ObjMonRunCore.LiteralsNotConstants());
        Assert.True(ObjMonRunCore.SameInOldRun());
        Assert.True(ObjMonRunCore.SameCommentText());
    }

    [Fact]
    public void GuardianPetPredicate()
    {
        Assert.True(ObjMonRunCore.GuardianLiteralsMatch());
        Assert.True(ObjMonRunCore.RaceAloneNotEnough());

        // **两个都要命中**
        Assert.True(ObjMonRunCore.IsGuardianPet(155, 156));
        Assert.False(ObjMonRunCore.IsGuardianPet(155, 0));
        Assert.False(ObjMonRunCore.IsGuardianPet(0, 156));
        Assert.False(ObjMonRunCore.IsGuardianPet(154, 156));
    }

    [Fact]
    public void MoveOptionOrdinals()
    {
        Assert.True(ObjMonRunCore.MoNoMoveIsOne());
        Assert.True(ObjMonRunCore.MoveOptionOrdinals());
        Assert.True(ObjMonRunCore.MoveOptionsExtracted());
        Assert.True(ObjMonRunCore.NotCustomNotNoMove());
        Assert.True(ObjMonRunCore.WrongOrdinalNotNoMove());

        Assert.Equal("moNoMove", ObjMonRunCore.MoveOptions[1]);
        Assert.True(ObjMonRunCore.IsNoMoveCustomMonster(true, 1));
        Assert.False(ObjMonRunCore.IsNoMoveCustomMonster(true, 0));
        Assert.False(ObjMonRunCore.IsNoMoveCustomMonster(true, 2));
        Assert.False(ObjMonRunCore.IsNoMoveCustomMonster(false, 1));
    }

    // ===================== 三、回位与防叠 =====================

    [Fact]
    public void RevertFacts()
    {
        Assert.True(ObjMonRunCore.TargetDiffThresholdOne());
        Assert.True(ObjMonRunCore.BothAxesChecked());
        Assert.True(ObjMonRunCore.RequiresBothAxesDiffer());
        Assert.True(ObjMonRunCore.NotEitherAxisDiffers());
        Assert.True(ObjMonRunCore.AsymmetricGuard());
        Assert.True(ObjMonRunCore.BothAxesDifferLineExtracted());
        Assert.True(ObjMonRunCore.OccupiedReverts());
        Assert.True(ObjMonRunCore.FreeDoesNotRevert());
        Assert.True(ObjMonRunCore.ThirdParamTrue());
        Assert.True(ObjMonRunCore.RevertToCurrentXY());
    }

    [Fact]
    public void SpaceMoveGuard()
    {
        Assert.True(ObjMonRunCore.SpaceMoveAllowedNormally());
        Assert.True(ObjMonRunCore.MinusOneTargetBlocksFly());
        Assert.True(ObjMonRunCore.FarDistanceAllowsFly());
        Assert.True(ObjMonRunCore.NearSameMapBlocksFly());
        Assert.True(ObjMonRunCore.RelaxBlocksFly());
        Assert.True(ObjMonRunCore.RelaxPetCanFly());
        Assert.True(ObjMonRunCore.MinusOneSentinel());
        Assert.True(ObjMonRunCore.ClearedAt1206());
        Assert.True(ObjMonRunCore.UsedAsGate());

        // **距离阈值 20：21 可以、20 不行（严格大于）**
        Assert.True(ObjMonRunCore.CanSpaceMove(false, false, false, false, 21, 0, 10, 10));
        Assert.False(ObjMonRunCore.CanSpaceMove(false, false, false, false, 20, 0, 10, 10));
    }

    // ===================== 四、任务点 =====================

    [Fact]
    public void MissionFacts()
    {
        Assert.True(ObjMonRunCore.MissionTripleGuard());
        Assert.True(ObjMonRunCore.BoundsCheckedBeforeUse());
        Assert.True(ObjMonRunCore.MissionUsableNormally());
        Assert.True(ObjMonRunCore.MissionEmptyRejected());
        Assert.True(ObjMonRunCore.MissionIndexOutOfRangeRejected());
        Assert.True(ObjMonRunCore.MissionOffRejected());
        Assert.True(ObjMonRunCore.ClampLowerBound());
        Assert.True(ObjMonRunCore.ClampUpperBound());
        Assert.True(ObjMonRunCore.StopsAtLastPoint());
        Assert.True(ObjMonRunCore.NoWrapAround());
        Assert.True(ObjMonRunCore.ClampNegativeToZero());
        Assert.True(ObjMonRunCore.ArrivalRadiusThree());
        Assert.True(ObjMonRunCore.BothAxesRequired());
        Assert.True(ObjMonRunCore.MissionExcludesPickup());
    }

    [Fact]
    public void MissionIndexAdvance()
    {
        Assert.True(ObjMonRunCore.AdvanceIncrements());
        Assert.True(ObjMonRunCore.AdvanceStopsAtEnd());
        Assert.True(ObjMonRunCore.AdvanceClampsBeyondEnd());

        Assert.Equal(1, ObjMonRunCore.AdvanceMissionIndex(0, 3));
        Assert.Equal(2, ObjMonRunCore.AdvanceMissionIndex(2, 3));
        Assert.Equal(2, ObjMonRunCore.AdvanceMissionIndex(5, 3));
    }

    [Fact]
    public void MissionArrivalBoundaries()
    {
        Assert.True(ObjMonRunCore.ExactlyThreeArrives());
        Assert.True(ObjMonRunCore.FourDoesNotArrive());
        Assert.True(ObjMonRunCore.OneAxisBeyondFails());

        // **半径三：三格到、四格不到**
        Assert.True(ObjMonRunCore.IsArrived(0, 0, 3, 3));
        Assert.False(ObjMonRunCore.IsArrived(0, 0, 4, 0));
        Assert.False(ObjMonRunCore.IsArrived(0, 0, 3, 4));
    }

    // ===================== 五、宠物拾取 =====================

    [Fact]
    public void PetPickupFacts()
    {
        Assert.True(ObjMonRunCore.PetPickupEnabledTwice());
        Assert.True(ObjMonRunCore.DuplicatedVerbatim());
        Assert.True(ObjMonRunCore.ThreeStateField());
        Assert.True(ObjMonRunCore.MapCanVetoPickup());
        Assert.True(ObjMonRunCore.StartPickUpTwoArities());
        Assert.True(ObjMonRunCore.PickRangeAlwaysFour());
        Assert.True(ObjMonRunCore.DefaultParamOmitted());
        Assert.True(ObjMonRunCore.ReturnIgnoredAt1169());
        Assert.True(ObjMonRunCore.ReturnCheckedAt1245());
        Assert.True(ObjMonRunCore.InconsistentUse());
        Assert.True(ObjMonRunCore.ArityLinesExtracted());
    }

    [Fact]
    public void PetPickupEnableTruthTable()
    {
        Assert.True(ObjMonRunCore.ZeroFollowsConfigOn());
        Assert.True(ObjMonRunCore.ZeroFollowsConfigOff());
        Assert.True(ObjMonRunCore.OneForcesOn());
        Assert.True(ObjMonRunCore.OthersDisabled());

        // **0 跟随全局**
        Assert.True(ObjMonRunCore.PetPickupEnabled(0, true));
        Assert.False(ObjMonRunCore.PetPickupEnabled(0, false));

        // **1 强制开（即使全局关）**
        Assert.True(ObjMonRunCore.PetPickupEnabled(1, false));

        // **其他值一律关（即使全局开）**
        Assert.False(ObjMonRunCore.PetPickupEnabled(2, true));
    }

    // ===================== 六、SmartObject =====================

    [Fact]
    public void SmartObjectFacts()
    {
        Assert.True(ObjMonRunCore.RaceCheckedBeforeCast());
        Assert.True(ObjMonRunCore.TwoRacesAllowed());
        Assert.True(ObjMonRunCore.PlayerIsCandidate());
        Assert.True(ObjMonRunCore.HeroIsCandidate());
        Assert.True(ObjMonRunCore.MonsterNotCandidate());
        Assert.True(ObjMonRunCore.ThreeGranularities());
        Assert.True(ObjMonRunCore.SetMembershipVsEquality());
        Assert.True(ObjMonRunCore.ThreeNestedGates());
        Assert.True(ObjMonRunCore.KeyMustBeConfigured());
        Assert.True(ObjMonRunCore.RangeMustBePositive());
    }

    [Fact]
    public void AutoRangePickGates()
    {
        Assert.True(ObjMonRunCore.AllGatesPass());
        Assert.True(ObjMonRunCore.MapGateBlocks());
        Assert.True(ObjMonRunCore.KeyGateBlocks());
        Assert.True(ObjMonRunCore.RangeGateBlocks());

        // **四道门全过才行**
        Assert.True(ObjMonRunCore.CanAutoRangePick(true, true, 1, 1));
        Assert.False(ObjMonRunCore.CanAutoRangePick(false, true, 1, 1));
        Assert.False(ObjMonRunCore.CanAutoRangePick(true, false, 1, 1));
        Assert.False(ObjMonRunCore.CanAutoRangePick(true, true, 0, 1));
        Assert.False(ObjMonRunCore.CanAutoRangePick(true, true, 1, 0));
    }

    // ===================== 七、走步锁与站稳 =====================

    [Fact]
    public void WalkLockFacts()
    {
        Assert.True(ObjMonRunCore.LockExpiresByTime());
        Assert.True(ObjMonRunCore.UnlockBeforeMoveCheck());
        Assert.True(ObjMonRunCore.LockOnCounterExceed());
        Assert.True(ObjMonRunCore.CounterResetOnLock());
        Assert.True(ObjMonRunCore.LockTickRecorded());
        Assert.True(ObjMonRunCore.StationTickNotThinkTick());
        Assert.True(ObjMonRunCore.UsesWalkSpeedAsDelay());
        Assert.True(ObjMonRunCore.AttackBeforeMove());
        Assert.True(ObjMonRunCore.RunAwaySkipsMoveBlock());
        Assert.True(ObjMonRunCore.RunAwayTimeoutClears());
        Assert.True(ObjMonRunCore.TimeThenFlagCleared());
    }

    [Fact]
    public void WalkWaitBoundaries()
    {
        Assert.True(ObjMonRunCore.JustLockedNotExpired());
        Assert.True(ObjMonRunCore.ExceededExpires());

        Assert.False(ObjMonRunCore.WalkWaitExpired(1000, 1000, 500));
        Assert.False(ObjMonRunCore.WalkWaitExpired(1000, 1500, 500));
        Assert.True(ObjMonRunCore.WalkWaitExpired(1000, 1501, 500));
    }

    [Fact]
    public void WalkLockCounter()
    {
        Assert.True(ObjMonRunCore.ExceedingStepLocks());
        Assert.True(ObjMonRunCore.EqualDoesNotLock());
        Assert.True(ObjMonRunCore.BelowDoesNotLock());

        // **严格大于：4>3 加锁、3>3 不加**
        Assert.True(ObjMonRunCore.ShouldLockWalk(4, 3));
        Assert.False(ObjMonRunCore.ShouldLockWalk(3, 3));
    }

    [Fact]
    public void StationBoundaries()
    {
        Assert.True(ObjMonRunCore.JustStationedNotStable());
        Assert.True(ObjMonRunCore.ExceededWalkSpeedIsStable());

        Assert.False(ObjMonRunCore.IsStationed(1000, 1000, 500));
        Assert.False(ObjMonRunCore.IsStationed(1000, 1500, 500));
        Assert.True(ObjMonRunCore.IsStationed(1000, 1501, 500));
    }

    [Fact]
    public void InconsistentTimeDiff()
    {
        Assert.True(ObjMonRunCore.RawSubtractionAt1330());
        Assert.True(ObjMonRunCore.TickDiffElsewhere());
        Assert.True(ObjMonRunCore.NoWraparoundCompensation());
        Assert.True(ObjMonRunCore.InconsistentTimeDiff());

        // **两处行号确实不同（同一方法两种求差写法）**
        Assert.NotEqual(ObjMonRunCore.RawSubtractionLine, ObjMonRunCore.TickDiffLine);
    }

    [Fact]
    public void RunAwayBoundaries()
    {
        Assert.True(ObjMonRunCore.ZeroTimeNotExpired());
        Assert.True(ObjMonRunCore.NotYetExpired());
        Assert.True(ObjMonRunCore.ExceededExpiresRunAway());

        // **时长为 0：永不超时**
        Assert.False(ObjMonRunCore.RunAwayExpired(0, 0, 999999));

        // **未到时间**
        Assert.False(ObjMonRunCore.RunAwayExpired(500, 1000, 1200));

        // **超过时间**
        Assert.True(ObjMonRunCore.RunAwayExpired(500, 1000, 1600));
    }

    // ===================== 八、放松模式 =====================

    [Fact]
    public void RelaxFacts()
    {
        Assert.True(ObjMonRunCore.RelaxGuardThreeTimes());
        Assert.True(ObjMonRunCore.ThreeConsequences());
        Assert.True(ObjMonRunCore.At1336Exits());
        Assert.True(ObjMonRunCore.At1186ClearsTarget());
        Assert.True(ObjMonRunCore.TwoStateClearedTogether());
    }

    [Fact]
    public void RelaxGuardTruthTable()
    {
        Assert.True(ObjMonRunCore.NoMasterNotRelaxed());
        Assert.True(ObjMonRunCore.FlagOffNotRelaxed());
        Assert.True(ObjMonRunCore.NonPetRelaxed());
        Assert.True(ObjMonRunCore.ControlledPetRelaxed());
        Assert.True(ObjMonRunCore.FreePetNotRelaxed());

        // **宠物不受控时不放松**
        Assert.False(ObjMonRunCore.IsRelaxed(true, true, true, false));
        Assert.True(ObjMonRunCore.IsRelaxed(true, true, true, true));
    }

    // ===================== 九、自定义怪物距离 =====================

    [Fact]
    public void CustomMonsterFacts()
    {
        Assert.True(ObjMonRunCore.MinRangeOneShortcut());
        Assert.True(ObjMonRunCore.DiagonalCase());
        Assert.True(ObjMonRunCore.BothAxesNonZero());
        Assert.True(ObjMonRunCore.AxesUnequal());
        Assert.True(ObjMonRunCore.NilTargetStillGoto());
        Assert.True(ObjMonRunCore.GotoTargetTwoStyles());
        Assert.True(ObjMonRunCore.LegalInDelphi());
        Assert.True(ObjMonRunCore.SameAsJ195Pattern());
        Assert.True(ObjMonRunCore.NoTargetXYNoMove());
        Assert.True(ObjMonRunCore.WonderingOnlyWithoutTarget());
    }

    [Fact]
    public void DiagonalPredicate()
    {
        Assert.True(ObjMonRunCore.TrueDiagonalMatches());
        Assert.True(ObjMonRunCore.EqualAxesNotDiagonal());
        Assert.True(ObjMonRunCore.SingleAxisNotDiagonal());
        Assert.True(ObjMonRunCore.SameMagnitudeSignIgnored());

        Assert.True(ObjMonRunCore.IsDiagonal(3, 5));
        Assert.False(ObjMonRunCore.IsDiagonal(3, 3));
        Assert.False(ObjMonRunCore.IsDiagonal(3, 0));
        Assert.True(ObjMonRunCore.IsDiagonal(-3, 5));
    }

    [Fact]
    public void WonderingPredicate()
    {
        Assert.True(ObjMonRunCore.NilTargetWonders());
        Assert.True(ObjMonRunCore.HasTargetNoWonder());

        Assert.True(ObjMonRunCore.ShouldWonder(true));
        Assert.False(ObjMonRunCore.ShouldWonder(false));
    }

    // ===================== 十、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonRunCore.LongestMethodSoFar());
        Assert.True(ObjMonRunCore.SevenLevelNesting());
    }

    [Fact]
    public void OldRunIsShorter()
    {
        Assert.True(ObjMonRunCore.OldRunShorterThanRun());
        Assert.True(ObjMonRunCore.RunGrewBy74());

        // **旧 185 行、新 259 行**
        Assert.Equal(185, ObjMonRunCore.OldRunContentLines);
        Assert.Equal(259, ObjMonRunCore.RunLines);
        Assert.Equal(74, ObjMonRunCore.RunLines - ObjMonRunCore.OldRunContentLines);
    }
}
