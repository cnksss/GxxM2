using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J140：经验怪 / 大刀护卫 —— `TExperienceMon` / `TGuardMonster`
/// （ObjMon2.pas 191-206、2563-2850）1:1 测试。
/// **本批次完成后 `ObjMon2.pas` 全部 17 个类均已覆盖。**
/// </summary>
public sealed class ExperienceGuardCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(ExperienceGuardCore.ConstantsMatchSource());
        Assert.True(ExperienceGuardCore.RaceConstants());
        Assert.True(ExperienceGuardCore.AttackRateIsOne());
    }

    [Fact]
    public void RaceConstantValues()
    {
        Assert.Equal(112, ExperienceGuardCore.RcArcherGuard);
        Assert.Equal(142, ExperienceGuardCore.RcMoveArcherGuard);
        Assert.Equal(128, ExperienceGuardCore.RcTruckObject);
        Assert.Equal(150, ExperienceGuardCore.RcPlayMoster);
        Assert.Equal(11, ExperienceGuardCore.RcGuard);
    }

    [Fact]
    public void DistanceConstants()
    {
        // **两套脱锁阈值：自身的 8、对主人的 20**
        Assert.Equal(8, ExperienceGuardCore.SelfUnlockDistance);
        Assert.Equal(20, ExperienceGuardCore.MasterUnlockDistance);
        Assert.Equal(2, ExperienceGuardCore.AntiOverlapDistance);
        Assert.Equal(-1, ExperienceGuardCore.NoTarget);
    }

    // ===================== 一、TExperienceMon =====================

    [Fact]
    public void AC1IsModeSelector()
    {
        // **把防御力字段复用成"经验发放模式"开关**
        Assert.True(ExperienceGuardCore.AC1IsModeSelector());
        Assert.True(ExperienceGuardCore.AC1ZeroMeansAll());
        Assert.True(ExperienceGuardCore.AC1OneMeansPhysicalOnly());
        Assert.True(ExperienceGuardCore.AC1TwoMeansMagicOnly());
        Assert.True(ExperienceGuardCore.ModeTruthTable());
    }

    [Fact]
    public void ModeTruthTableValues()
    {
        // 0 = 不限：物理与魔法都发
        Assert.True(ExperienceGuardCore.ShouldGrantExp(0, false));
        Assert.True(ExperienceGuardCore.ShouldGrantExp(0, true));

        // 1 = 仅物理
        Assert.True(ExperienceGuardCore.ShouldGrantExp(1, false));
        Assert.False(ExperienceGuardCore.ShouldGrantExp(1, true));

        // 2 = 仅魔法
        Assert.False(ExperienceGuardCore.ShouldGrantExp(2, false));
        Assert.True(ExperienceGuardCore.ShouldGrantExp(2, true));
    }

    [Fact]
    public void AC1OutOfRangeSilent()
    {
        // **AC1 >= 3 无 else 兜底 → 静默不发经验**
        Assert.True(ExperienceGuardCore.AC1OutOfRangeSilent());
        Assert.False(ExperienceGuardCore.ShouldGrantExp(3, false));
        Assert.False(ExperienceGuardCore.ShouldGrantExp(3, true));
        Assert.False(ExperienceGuardCore.ShouldGrantExp(99, true));
    }

    [Fact]
    public void ZeroExpGateUsesLessOrEqual()
    {
        // **经验值门是 > 0（即 <= 0 都不发）**
        Assert.True(ExperienceGuardCore.ZeroExpGateUsesLessOrEqual());
        Assert.False(ExperienceGuardCore.ExpGate(0));
        Assert.False(ExperienceGuardCore.ExpGate(-5));
        Assert.True(ExperienceGuardCore.ExpGate(1));
    }

    [Fact]
    public void DispatchLogicDuplicatedThreeTimes()
    {
        // **同一段发放逻辑逐字复制三次**
        Assert.True(ExperienceGuardCore.DispatchLogicDuplicatedThreeTimes());
        Assert.True(ExperienceGuardCore.ThreeCopiesIdentical());
        Assert.Equal(3, ExperienceGuardCore.DispatchCopyCount());
    }

    [Fact]
    public void HeroNeedsMaster()
    {
        Assert.True(ExperienceGuardCore.HeroNeedsMasterTruthTable());
        Assert.True(ExperienceGuardCore.HeroNeedsMaster(true, true));
        Assert.False(ExperienceGuardCore.HeroNeedsMaster(true, false));
    }

    [Fact]
    public void SecondParamIsIsHero()
    {
        // **玩家 False、英雄主人 True**
        Assert.True(ExperienceGuardCore.SecondParamIsIsHero());
        Assert.True(ExperienceGuardCore.SecondParamIsTrueForHeroOnly());
        Assert.True(ExperienceGuardCore.ThirdParamAlwaysFalse());
        Assert.Equal((false, true), ExperienceGuardCore.SecondParamValues());
    }

    [Fact]
    public void ExpTargetTruthTable()
    {
        Assert.True(ExperienceGuardCore.ExpTargetTruthTable());
        Assert.Equal("player", ExperienceGuardCore.ExpTarget(0, false));
        Assert.Equal("master", ExperienceGuardCore.ExpTarget(1, true));
        Assert.Equal("none", ExperienceGuardCore.ExpTarget(1, false));
        Assert.Equal("none", ExperienceGuardCore.ExpTarget(80, true));
    }

    [Fact]
    public void SameInitAsSoccerBall()
    {
        // **无敌 + 非动物，与 J139 足球相同**
        Assert.True(ExperienceGuardCore.SameInitAsSoccerBall());
        Assert.Equal((false, true), ExperienceGuardCore.ExpMonInit());
    }

    [Fact]
    public void StruckIsEmptyOverride()
    {
        Assert.True(ExperienceGuardCore.StruckIsEmptyOverride());
        Assert.True(ExperienceGuardCore.StruckOnlyForwards());
    }

    // ===================== 二、TGuardMonster.AttackTarget =====================

    [Fact]
    public void TargetMustBeActor()
    {
        // **目标必须是 Obj_Actor**
        Assert.True(ExperienceGuardCore.TargetTypeTruthTable());
        Assert.True(ExperienceGuardCore.TargetMustBeActor(1));
        Assert.False(ExperienceGuardCore.TargetMustBeActor(0));
        Assert.False(ExperienceGuardCore.TargetMustBeActor(2));
    }

    [Fact]
    public void FixCommentHasQuestionMarks()
    {
        // **修复注释以五个问号开头，作者当时也不确定原因**
        Assert.True(ExperienceGuardCore.FixCommentHasQuestionMarks());
        Assert.Contains("?????", ExperienceGuardCore.FixComment);
        Assert.Contains("2013-11-13", ExperienceGuardCore.FixComment);
    }

    [Fact]
    public void CrossMapDropsTarget()
    {
        // **跨图只放弃目标，本处不追击**
        Assert.True(ExperienceGuardCore.CrossMapDropsTarget());
        Assert.True(ExperienceGuardCore.NoPursuitHere());
        Assert.True(ExperienceGuardCore.DiffersFromStickMonster());
        Assert.Equal("DelTargetCreat", ExperienceGuardCore.CrossMapAction());
    }

    [Fact]
    public void TeleportsToTargetBack()
    {
        // **瞬移到目标背后一格，打完复位**
        Assert.True(ExperienceGuardCore.TeleportsToTargetBack());
        Assert.True(ExperienceGuardCore.AttackSequenceSixSteps());
        Assert.True(ExperienceGuardCore.RestoresPositionAfterAttack());
        Assert.Equal(6, ExperienceGuardCore.AttackSequence.Length);
    }

    [Fact]
    public void AttackSequenceValues()
    {
        Assert.Equal(new[] { "save", "GetBackPosition", "GetNextDirection", "SendRefMsg", "_Attack", "restore" },
            ExperienceGuardCore.AttackSequence);
    }

    [Fact]
    public void HitModeAlwaysZero()
    {
        Assert.True(ExperienceGuardCore.HitModeAlwaysZero());
        Assert.True(ExperienceGuardCore.HitModeIsZero());
        Assert.Equal(0, ExperienceGuardCore.HitModeInit());
    }

    [Fact]
    public void ClearsVictimExpHitter()
    {
        // **清掉受害者的 m_ExpHitter**
        Assert.True(ExperienceGuardCore.ExpHitterClearedOnVictim());
        Assert.True(ExperienceGuardCore.ClearsVictimExpHitter());
    }

    [Fact]
    public void PostAttackActions()
    {
        Assert.True(ExperienceGuardCore.TurnToUsesRestoredDir());
        Assert.True(ExperienceGuardCore.BreaksHolySeize());
    }

    [Fact]
    public void ResultOnlyTrueOnSameMap()
    {
        Assert.True(ExperienceGuardCore.ResultOnlyTrueOnSameMap());
        Assert.True(ExperienceGuardCore.ResultTruthTable());
        Assert.True(ExperienceGuardCore.AttackTargetResult(true));
        Assert.False(ExperienceGuardCore.AttackTargetResult(false));
    }

    [Fact]
    public void CommentedDebugMessage()
    {
        Assert.True(ExperienceGuardCore.CommentedDebugMessage());
        Assert.Contains("_Attack", ExperienceGuardCore.CommentedDebugMsg);
    }

    [Fact]
    public void HitIntervalUsesSumStrictGreater()
    {
        Assert.True(ExperienceGuardCore.HitIntervalUsesSumStrictGreater());
        Assert.False(ExperienceGuardCore.HitDue(0, 100, 60, 40));
        Assert.True(ExperienceGuardCore.HitDue(0, 101, 60, 40));
        Assert.True(ExperienceGuardCore.ThreeRefreshedFields());
    }

    // ===================== 三、TGuardMonster.Run =====================

    [Fact]
    public void InheritedFirst()
    {
        // **inherited 在最前面，死亡检查在其后 —— 与最近几批相反**
        Assert.True(ExperienceGuardCore.InheritedFirst());
        Assert.True(ExperienceGuardCore.DeathCheckAfterInherited());
        Assert.True(ExperienceGuardCore.InheritedPlacementOpposesRecentBatches());
    }

    [Fact]
    public void DeathGateTruthTable()
    {
        Assert.True(ExperienceGuardCore.DeathGateTruthTable());
        Assert.False(ExperienceGuardCore.DeathGate(false, false));
        Assert.True(ExperienceGuardCore.DeathGate(true, false));
        Assert.True(ExperienceGuardCore.DeathGate(false, true));
    }

    [Fact]
    public void TwoUnlockThresholds()
    {
        // **自身 8 格、对主人 20 格**
        Assert.True(ExperienceGuardCore.TwoUnlockThresholds());
        Assert.True(ExperienceGuardCore.ThresholdsDiffer());
        Assert.True(ExperienceGuardCore.DifferentReferencePoints());
    }

    [Fact]
    public void SelfUnlockBoundary()
    {
        Assert.True(ExperienceGuardCore.SelfUnlockBoundary());
        Assert.False(ExperienceGuardCore.SelfUnlock(8, 0, true));
        Assert.True(ExperienceGuardCore.SelfUnlock(9, 0, true));
        Assert.True(ExperienceGuardCore.SelfUnlock(0, 0, false));
    }

    [Fact]
    public void MasterUnlockBoundary()
    {
        Assert.True(ExperienceGuardCore.MasterUnlockBoundary());
        Assert.False(ExperienceGuardCore.MasterUnlock(20, 0, true));
        Assert.True(ExperienceGuardCore.MasterUnlock(21, 0, true));
    }

    [Fact]
    public void MasterCommentsPresent()
    {
        Assert.True(ExperienceGuardCore.MasterDistanceCommentHasDate());
        Assert.True(ExperienceGuardCore.MasterCommentsPresent());
        Assert.Contains("2017-07-01", ExperienceGuardCore.MasterDistanceComment);
        Assert.Contains("天关", ExperienceGuardCore.GuardianLevelComment);
    }

    [Fact]
    public void CrossMapBehaviorTruthTable()
    {
        // **天关优先于镜像；都无则继续**
        Assert.True(ExperienceGuardCore.CrossMapBehaviorTruthTable());
        Assert.True(ExperienceGuardCore.GuardianLevelTakesPrecedence());
        Assert.Equal("MakeGhost", ExperienceGuardCore.CrossMapMasterBehavior(true, true));
    }

    [Fact]
    public void MasterExitConditions()
    {
        Assert.True(ExperienceGuardCore.GuardianLevelMakesGhost());
        Assert.True(ExperienceGuardCore.MirrorMapSpaceMoves());
        Assert.True(ExperienceGuardCore.SlaveRelaxExits());
        Assert.True(ExperienceGuardCore.FirstTwoAreMutuallyExclusive());
    }

    [Fact]
    public void WalkCountResetsOnLock()
    {
        // **走 N 步就锁住一段时间**
        Assert.True(ExperienceGuardCore.WalkStepLockPattern());
        Assert.True(ExperienceGuardCore.WalkCountResetsOnLock());
        Assert.Equal((1, false), ExperienceGuardCore.AfterWalkStep(0, 3));
        Assert.Equal((0, true), ExperienceGuardCore.AfterWalkStep(3, 3));
    }

    [Fact]
    public void StepGateIsStrictGreater()
    {
        Assert.True(ExperienceGuardCore.StepGateIsStrictGreater());
        Assert.False(ExperienceGuardCore.AfterWalkStep(2, 3).Locked);
        Assert.True(ExperienceGuardCore.AfterWalkStep(3, 3).Locked);
    }

    [Fact]
    public void UnlockUsesStrictGreater()
    {
        Assert.True(ExperienceGuardCore.UnlockUsesStrictGreater());
        Assert.False(ExperienceGuardCore.UnlockDue(0, 5, 5));
        Assert.True(ExperienceGuardCore.UnlockDue(0, 6, 5));
        Assert.True(ExperienceGuardCore.WalkStepAddressComment());
    }

    [Fact]
    public void CanMoveModeDefaultsFalse()
    {
        // **出生时不能走动，只由引擎置真**
        Assert.True(ExperienceGuardCore.CanMoveModeDefaultsFalse());
        Assert.True(ExperienceGuardCore.OnlyEngineEnablesIt());
        Assert.True(ExperienceGuardCore.SpawnFrozen());
        Assert.True(ExperienceGuardCore.EngineAssignPresent());
        Assert.True(ExperienceGuardCore.CanMoveModeInit());
        Assert.Equal("UsrEngn.pas:6157", ExperienceGuardCore.EngineEnablesAt);
    }

    [Fact]
    public void UsesMasterBackPosition()
    {
        Assert.True(ExperienceGuardCore.UsesMasterBackPosition());
        Assert.True(ExperienceGuardCore.RepositionBoundary());
        Assert.False(ExperienceGuardCore.NeedsReposition(10, 10, 11, 11));
        Assert.True(ExperienceGuardCore.NeedsReposition(10, 10, 12, 10));
    }

    [Fact]
    public void AntiOverlapTruthTable()
    {
        // **防止宝宝和主人叠一起**
        Assert.True(ExperienceGuardCore.AntiOverlapCorrection());
        Assert.True(ExperienceGuardCore.AntiOverlapTruthTable());
        Assert.True(ExperienceGuardCore.ShouldHoldPosition(1, 2, false, false, true));
        Assert.False(ExperienceGuardCore.ShouldHoldPosition(3, 0, false, false, true));
        Assert.False(ExperienceGuardCore.ShouldHoldPosition(1, 2, true, false, true));
        Assert.False(ExperienceGuardCore.ShouldHoldPosition(1, 2, false, false, false));
    }

    [Fact]
    public void AntiOverlapCommentHasDate()
    {
        Assert.True(ExperienceGuardCore.AntiOverlapCommentHasDate());
        Assert.Contains("2015-09-11", ExperienceGuardCore.AntiOverlapComment);
        Assert.True(ExperienceGuardCore.CommentedAlternativeArg());
        Assert.True(ExperienceGuardCore.UsesNYNotNX());
    }

    [Fact]
    public void ChecksBothSentinels()
    {
        // **两个 -1 哨兵都要检查（与 J139 足球只查一个不同）**
        Assert.True(ExperienceGuardCore.SpaceMoveWhenFarFromMaster());
        Assert.True(ExperienceGuardCore.ChecksBothSentinels());
        Assert.True(ExperienceGuardCore.DiffersFromSoccerBallSentinelCheck());
        Assert.False(ExperienceGuardCore.SpaceMoveGate(true, 0, 0, -1, 0));
        Assert.False(ExperienceGuardCore.SpaceMoveGate(true, 0, 0, 0, -1));
        Assert.True(ExperienceGuardCore.SpaceMoveGate(true, 0, 0, 0, 0));
    }

    [Fact]
    public void GotoOrWanderTruthTable()
    {
        // **漫游只在既无路径又无目标时发生**
        Assert.True(ExperienceGuardCore.GotoOrWanderTruthTable());
        Assert.True(ExperienceGuardCore.WanderRequiresNoTarget());
        Assert.True(ExperienceGuardCore.WanderCommentPresent());
        Assert.Equal("GotoTargetXY", ExperienceGuardCore.MoveAction(5, false));
        Assert.Equal("Wondering", ExperienceGuardCore.MoveAction(-1, false));
        Assert.Equal("nothing", ExperienceGuardCore.MoveAction(-1, true));
    }

    // ===================== 索敌与过滤 =====================

    [Fact]
    public void SearchesVisibleActors()
    {
        Assert.True(ExperienceGuardCore.SearchesVisibleActors());
    }

    [Fact]
    public void SkipsTruck()
    {
        // **不攻击镖车**
        Assert.True(ExperienceGuardCore.SkipsTruck());
        Assert.True(ExperienceGuardCore.SkipTarget(128, false, false, 0, false));
    }

    [Fact]
    public void SkipsBothArcherRaces()
    {
        // **两种弓箭手各一条独立 Continue**
        Assert.True(ExperienceGuardCore.SkipsBothArcherRaces());
        Assert.True(ExperienceGuardCore.SkipTarget(112, false, false, 0, false));
        Assert.True(ExperienceGuardCore.SkipTarget(142, false, false, 0, false));
    }

    [Fact]
    public void PetFilterUsesOr()
    {
        Assert.True(ExperienceGuardCore.PetFilterUsesOr());
        Assert.True(ExperienceGuardCore.SkipTarget(80, true, true, 0, false));
        Assert.True(ExperienceGuardCore.SkipTarget(80, true, false, 1, false));
        Assert.False(ExperienceGuardCore.SkipTarget(80, true, false, 0, false));
    }

    [Fact]
    public void PlayMosterGateIsConfigurable()
    {
        Assert.True(ExperienceGuardCore.PlayMosterGateIsConfigurable());
        Assert.True(ExperienceGuardCore.SkipTarget(150, false, false, 0, true));
        Assert.False(ExperienceGuardCore.SkipTarget(150, false, false, 0, false));
    }

    [Fact]
    public void NormalMonsterNotSkipped()
    {
        Assert.True(ExperienceGuardCore.NormalMonsterNotSkipped());
        Assert.False(ExperienceGuardCore.SkipTarget(80, false, false, 0, false));
    }

    [Fact]
    public void FilterCommentsPresent()
    {
        Assert.True(ExperienceGuardCore.FilterCommentsPresent());
        Assert.Contains("2020-03-26", ExperienceGuardCore.PetComment);
        Assert.Contains("镖车", ExperienceGuardCore.TruckComment);
    }

    [Fact]
    public void TwoCommentedFilters()
    {
        Assert.True(ExperienceGuardCore.TwoCommentedFilters());
        Assert.True(ExperienceGuardCore.CommentedFilterCount());
        Assert.Equal(2, ExperienceGuardCore.CommentedFilters.Length);
    }

    [Fact]
    public void ErrCodeValuesMatch()
    {
        // **分级定位 0/3/4/5 —— 跳过 1 和 2**
        Assert.True(ExperienceGuardCore.StagedErrCode());
        Assert.True(ExperienceGuardCore.ErrCodeValuesMatch());
        Assert.True(ExperienceGuardCore.SkipsOneAndTwo());
        Assert.Equal((0, 3, 4, 5), ExperienceGuardCore.ErrCodeValues());
    }

    [Fact]
    public void ExceptionMessageFormat()
    {
        Assert.True(ExperienceGuardCore.ExceptionMessageFormat());
        Assert.Equal("TGuardMonster.Run Error, ErrCode = 3", ExperienceGuardCore.ExceptionMsg(3));
        Assert.Equal("TGuardMonster.Run Error, ErrCode = 0", ExperienceGuardCore.ExceptionMsg(0));
    }

    [Fact]
    public void FourthExceptionStyle()
    {
        // **本工程至此四种异常定位风格**
        Assert.True(ExperienceGuardCore.FourthExceptionStyle());
        Assert.True(ExperienceGuardCore.FourStylesPresent());
        Assert.Equal(4, ExperienceGuardCore.ExceptionStyles.Length);
    }

    [Fact]
    public void TryWrapsMostButNotUnlock()
    {
        Assert.True(ExperienceGuardCore.TryWrapsMostButNotUnlock());
        Assert.True(ExperienceGuardCore.UnlockExceptionPropagates());
    }

    [Fact]
    public void CommentedNoSummonRestriction()
    {
        // **"不允许召唤为宝宝"已被注释掉**
        Assert.True(ExperienceGuardCore.CommentedNoSummonRestriction());
        Assert.True(ExperienceGuardCore.NoSummonCommentPresent());
        Assert.Contains("m_Master := nil", ExperienceGuardCore.CommentedNoSummon);
    }

    [Fact]
    public void OperateIsPureForward()
    {
        Assert.True(ExperienceGuardCore.OperateIsPureForward());
    }

    [Fact]
    public void CreateFourItems()
    {
        Assert.True(ExperienceGuardCore.CreateFourItems());
        Assert.Equal(4, ExperienceGuardCore.CreateItemCount());
        Assert.Equal((false, 7, 4), ExperienceGuardCore.CreateThree());
    }

    [Fact]
    public void RaceAssignmentCommentedOut()
    {
        // **种族不再硬编码为 11**
        Assert.True(ExperienceGuardCore.RaceAssignmentCommentedOut());
        Assert.True(ExperienceGuardCore.CommentedRaceAssignPresent());
        Assert.Contains("11", ExperienceGuardCore.CommentedRaceAssign);
    }

    [Fact]
    public void DoubleSemicolonTypo()
    {
        // **`CanMoveMode := False;;` 两个分号**
        Assert.True(ExperienceGuardCore.DoubleSemicolonTypo());
        Assert.True(ExperienceGuardCore.DoubleSemicolonPresent());
        Assert.EndsWith(";;", ExperienceGuardCore.DoubleSemicolonLine);
    }

    [Fact]
    public void ViewRangeAndLight()
    {
        Assert.True(ExperienceGuardCore.ViewRangeIsSeven());
        Assert.True(ExperienceGuardCore.LightIsFour());
        Assert.True(ExperienceGuardCore.GuardIsNotSuperMan());
        Assert.True(ExperienceGuardCore.DiffersFromExperienceMonInit());
    }

    // ===================== 四、ObjMon2.pas 全覆盖 =====================

    [Fact]
    public void ClassCountIsSeventeen()
    {
        // **脚本扫描实测为 17 个类（不是 16）**
        Assert.True(ExperienceGuardCore.ClassCountIsSeventeen());
        Assert.Equal(17, ExperienceGuardCore.ObjMon2ClassCount);
        Assert.True(ExperienceGuardCore.AllSixteenClassesCovered());
    }

    [Fact]
    public void ListedClassesMatchCount()
    {
        // **清单去重后仍是 17，说明无重复、无遗漏**
        Assert.True(ExperienceGuardCore.ListedClassesMatchCount());
        Assert.Equal(17, ExperienceGuardCore.ListedClassCount());
    }

    [Fact]
    public void EveryClassHasABatch()
    {
        Assert.True(ExperienceGuardCore.EveryClassHasABatch());
        Assert.True(ExperienceGuardCore.BatchesCoverAllClasses());
        Assert.True(ExperienceGuardCore.ThisBatchIsTwo());
    }

    [Fact]
    public void AllClassesListedExplicitly()
    {
        Assert.Equal(17, ExperienceGuardCore.AllClasses.Length);

        foreach (var (cls, batch) in ExperienceGuardCore.AllClasses)
        {
            Assert.False(string.IsNullOrWhiteSpace(cls));
            Assert.StartsWith("J", batch);
        }
    }

    [Fact]
    public void ThisBatchClasses()
    {
        Assert.Equal(new[] { "TExperienceMon", "TGuardMonster" }, ExperienceGuardCore.ThisBatchClasses);
    }
}
