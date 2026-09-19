using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J134：冰柱怪物与墙体 —— `TIcicleMonster`（ObjMon2.pas 129-140、2002-2217）
/// 与 `TWallStructure` 其余本体（1912-1960）1:1 测试。
/// </summary>
public sealed class IcicleMonsterCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(IcicleMonsterCore.ConstantsMatchSource());
        Assert.Equal(20102, IcicleMonsterCore.RmLighting);
        Assert.Equal(20101, IcicleMonsterCore.RmFlyAxe);
        Assert.Equal(128, IcicleMonsterCore.RcTruckObject);
        Assert.Equal(10, IcicleMonsterCore.InitialRange);
        Assert.Equal(4, IcicleMonsterCore.CloseDefenseParam);
    }

    [Fact]
    public void CommentsMatchSource()
    {
        Assert.True(IcicleMonsterCore.CommentsMatchSource());
        Assert.Equal("// 不攻击镖车", IcicleMonsterCore.TruckComment);
        Assert.Contains("chongchong", IcicleMonsterCore.OfflineComment);
        Assert.Contains("2016-09-07", IcicleMonsterCore.PowerMaxComment);
    }

    [Fact]
    public void LightingAndFlyAxeAdjacent()
    {
        Assert.True(IcicleMonsterCore.LightingAndFlyAxeAdjacent());
        Assert.True(IcicleMonsterCore.FlyAxeCommented());
    }

    // ===================== 一、tick_diff 与两层节拍 =====================

    [Fact]
    public void TickDiffNormal()
    {
        Assert.True(IcicleMonsterCore.TickDiffNormal());
        Assert.True(IcicleMonsterCore.TickDiffZero());
    }

    [Fact]
    public void TickDiffWrapsShortByOne()
    {
        // **回绕时短 1**（未加 1）
        Assert.True(IcicleMonsterCore.TickDiffWrapsShortByOne());
        Assert.True(IcicleMonsterCore.TickDiffWrapGeneral());
        Assert.Equal(0u, IcicleMonsterCore.TickDiff(uint.MaxValue, 0));
        Assert.Equal(15u, IcicleMonsterCore.TickDiff(uint.MaxValue - 10, 5));
    }

    [Fact]
    public void WalkTickDueBoundaries()
    {
        Assert.True(IcicleMonsterCore.WalkTickAtExactThreshold());
        Assert.True(IcicleMonsterCore.WalkTickOneShort());
        Assert.False(IcicleMonsterCore.WalkTickDue(0, 4, 3, 2));
    }

    [Fact]
    public void WalkThresholdIsSum()
    {
        // **门槛是两字段之和**
        Assert.True(IcicleMonsterCore.WalkThresholdIsSum());
        Assert.True(IcicleMonsterCore.SumThresholdDiffersFromSingle());
    }

    [Fact]
    public void HitThresholdIsSingle()
    {
        Assert.True(IcicleMonsterCore.HitThresholdIsSingle());
        Assert.True(IcicleMonsterCore.HitTickDue(0, 100, 100));
        Assert.False(IcicleMonsterCore.HitTickDue(0, 99, 100));
    }

    [Fact]
    public void TwoIndependentTicks()
    {
        Assert.True(IcicleMonsterCore.TwoIndependentTicks());
        Assert.True(IcicleMonsterCore.DifferentThresholdSources());
        Assert.True(IcicleMonsterCore.BothTicksCanFireSameRound());
    }

    [Fact]
    public void WalkTickResetsDelay()
    {
        Assert.True(IcicleMonsterCore.WalkTickResetsDelay());
        Assert.True(IcicleMonsterCore.DelayResetVerified());
        Assert.Equal((0, 1234u), IcicleMonsterCore.AfterWalkTick(1234));
    }

    // ===================== 二、贪心选靶 =====================

    [Fact]
    public void ManhattanNotEuclidean()
    {
        Assert.True(IcicleMonsterCore.ManhattanNotEuclidean());
        Assert.True(IcicleMonsterCore.MetricsDifferDiagonally());
        Assert.Equal(6, IcicleMonsterCore.Manhattan(0, 0, 3, 3));
    }

    [Fact]
    public void ManhattanValues()
    {
        Assert.Equal(0, IcicleMonsterCore.Manhattan(5, 5, 5, 5));
        Assert.Equal(1, IcicleMonsterCore.Manhattan(5, 5, 6, 5));
        Assert.Equal(2, IcicleMonsterCore.Manhattan(5, 5, 6, 6));
        Assert.Equal(10, IcicleMonsterCore.Manhattan(0, 0, 5, 5));
    }

    [Fact]
    public void InitialRangeIsTen()
    {
        Assert.True(IcicleMonsterCore.InitialRangeIsTen());
        Assert.True(IcicleMonsterCore.StrictlyLessThanInitialRange());
    }

    [Fact]
    public void DistanceExactlyTenRejected()
    {
        // **距离恰好 10 不被选**
        Assert.True(IcicleMonsterCore.DistanceExactlyTenRejected());
        Assert.True(IcicleMonsterCore.DistanceNineAccepted());
        Assert.True(IcicleMonsterCore.NineIsPickable());
    }

    [Fact]
    public void PicksNearest()
    {
        Assert.True(IcicleMonsterCore.PicksNearest());
    }

    [Fact]
    public void TieKeepsFirstEncountered()
    {
        // **平局保留先遇到的**
        Assert.True(IcicleMonsterCore.TieKeepsFirstEncountered());
        Assert.Equal(0, IcicleMonsterCore.GreedyPick(new[] { (true, 5), (true, 5) }));
    }

    [Fact]
    public void GreedyPickEdgeCases()
    {
        Assert.True(IcicleMonsterCore.AllBeyondRangeRejected());
        Assert.True(IcicleMonsterCore.IneligibleSkipped());
        Assert.True(IcicleMonsterCore.EmptyPicksNothing());
    }

    [Fact]
    public void GreedyPickOrderMatters()
    {
        // 先遇到远的、后遇到近的 → 选近的
        Assert.Equal(1, IcicleMonsterCore.GreedyPick(new[] { (true, 8), (true, 2) }));

        // 先近后远 → 仍选近的（0）
        Assert.Equal(0, IcicleMonsterCore.GreedyPick(new[] { (true, 2), (true, 8) }));
    }

    [Fact]
    public void ClearsTargetWhenNoneFound()
    {
        Assert.True(IcicleMonsterCore.ClearsTargetWhenNoneFound());
        Assert.True(IcicleMonsterCore.BothTargetActions());
        Assert.True(IcicleMonsterCore.ClearOnlyOnTick());
    }

    [Fact]
    public void TargetActionNames()
    {
        Assert.Equal("SetTargetCreat", IcicleMonsterCore.TargetAction(0));
        Assert.Equal("DelTargetCreat", IcicleMonsterCore.TargetAction(-1));
    }

    // ===================== 三、六道过滤 =====================

    [Fact]
    public void TruckAlwaysSkipped()
    {
        // **镖车总被跳过（配置无关）**
        Assert.True(IcicleMonsterCore.TruckAlwaysSkipped());
    }

    [Fact]
    public void OfflinePlayerSkippedOnlyWhenConfigured()
    {
        // **脱机玩家仅在配置打开时被跳过**
        Assert.True(IcicleMonsterCore.OfflinePlayerSkippedOnlyWhenConfigured());
        Assert.True(IcicleMonsterCore.OnlinePlayerNeverSkipped());
    }

    [Fact]
    public void CastGuardedByRaceCheck()
    {
        Assert.True(IcicleMonsterCore.OfflineFlagReadOnPlayerOnly());
        Assert.True(IcicleMonsterCore.CastGuardedByRaceCheck());
    }

    [Fact]
    public void BasicFilters()
    {
        Assert.True(IcicleMonsterCore.DeadSkipped());
        Assert.True(IcicleMonsterCore.NullSkipped());
        Assert.True(IcicleMonsterCore.NormalActorPasses());
    }

    [Fact]
    public void PassesFiltersTruthTable()
    {
        Assert.True(IcicleMonsterCore.PassesFilters(new IcicleMonsterCore.Actor { RaceServer = 80 }, false));
        Assert.False(IcicleMonsterCore.PassesFilters(new IcicleMonsterCore.Actor { RaceServer = 80, Death = true }, false));
        Assert.False(IcicleMonsterCore.PassesFilters(new IcicleMonsterCore.Actor { RaceServer = 128 }, false));
        Assert.False(IcicleMonsterCore.PassesFilters(new IcicleMonsterCore.Actor { IsNull = true }, false));
    }

    // ===================== 四、IsProperTarget =====================

    [Fact]
    public void AcceptsAnyPlayer()
    {
        // **无条件接受任何非管理员玩家**
        Assert.True(IcicleMonsterCore.AcceptsAnyPlayer());
        Assert.True(IcicleMonsterCore.AcceptsHeroAndPlayMoster());
    }

    [Fact]
    public void RejectsOrdinaryMonster()
    {
        Assert.True(IcicleMonsterCore.RejectsOrdinaryMonster());
    }

    [Fact]
    public void AdminModeOverridesEverything()
    {
        // **④ 是最终否决权**
        Assert.True(IcicleMonsterCore.LaterRulesOverrideEarlier());
        Assert.True(IcicleMonsterCore.AdminModeOverridesEverything());
        Assert.True(IcicleMonsterCore.TempAdminAlsoVetoes());
        Assert.True(IcicleMonsterCore.StoneModeVetoes());
        Assert.True(IcicleMonsterCore.SelfVetoes());
    }

    [Fact]
    public void LastHiterAndArcherGuardRules()
    {
        Assert.True(IcicleMonsterCore.IsProperTarget(true, false, 80, false, false, false, false));
        Assert.True(IcicleMonsterCore.IsProperTarget(false, true, 80, false, false, false, false));
        Assert.False(IcicleMonsterCore.IsProperTarget(false, false, 80, false, false, false, false));
    }

    [Fact]
    public void RuleOrderMatters()
    {
        Assert.True(IcicleMonsterCore.RuleOrderMatters());

        // 先真后假：打过我、也是管理员 → 最终假
        Assert.False(IcicleMonsterCore.IsProperTarget(true, false, 0, true, false, false, false));
    }

    [Fact]
    public void NilDerefInRuleTwo()
    {
        Assert.True(IcicleMonsterCore.NilDerefInRuleTwo());
    }

    [Fact]
    public void CommentedAndLiveDiffer()
    {
        Assert.True(IcicleMonsterCore.CommentedAndLiveDiffer());
        Assert.True(IcicleMonsterCore.CommentedVersionHasPkPointRules());
        Assert.True(IcicleMonsterCore.CommentedVersionGatedByAttackType());
        Assert.True(IcicleMonsterCore.LiveVersionIgnoresAttackType());
        Assert.Equal("// 004A6A41", IcicleMonsterCore.LiveAddressMarker);
    }

    // ===================== 五、AttackTarget 管线 =====================

    [Fact]
    public void SixteenStepPipeline()
    {
        Assert.True(IcicleMonsterCore.SixteenStepPipeline());
        Assert.True(IcicleMonsterCore.PipelineHasSixteenSteps());
        Assert.Equal(16, IcicleMonsterCore.PipelineSteps.Length);
    }

    [Fact]
    public void PowerSpanLowerBound()
    {
        // **区间下界保护为 1**
        Assert.True(IcicleMonsterCore.PowerSpanLowerBound());
        Assert.Equal(1, IcicleMonsterCore.PowerSpan(10, 10));
        Assert.Equal(4, IcicleMonsterCore.PowerSpan(10, 14));
    }

    [Fact]
    public void DefenseBranchUsesFourthParam()
    {
        // **第四参硬编码 4**
        Assert.True(IcicleMonsterCore.DefenseBranchUsesFourthParam());
        Assert.True(IcicleMonsterCore.HitStruckHasTwoOverloads());
        Assert.Equal(4, IcicleMonsterCore.HitStruckArgs(true));
        Assert.Equal(3, IcicleMonsterCore.HitStruckArgs(false));
    }

    [Fact]
    public void AbsorbOnlyForPlayers()
    {
        Assert.True(IcicleMonsterCore.AbsorbOnlyForPlayers(IcicleMonsterCore.RcPlayObject));
        Assert.True(IcicleMonsterCore.ThreePlayerRacesAbsorb());
        Assert.True(IcicleMonsterCore.MonstersSkipAbsorb());
    }

    [Fact]
    public void NgGuardTruthTable()
    {
        Assert.True(IcicleMonsterCore.NgGuardTruthTable());
    }

    [Fact]
    public void NgGuardApplied()
    {
        Assert.True(IcicleMonsterCore.NgGuardApplied());
        Assert.True(IcicleMonsterCore.NgGuardFloorsAtZero());
        Assert.Equal((70, 40), IcicleMonsterCore.ApplyNgGuard(100, 60, 30, 20));
    }

    [Fact]
    public void SuckChanceIsRandom100()
    {
        Assert.True(IcicleMonsterCore.SuckChanceIsRandom100());
        Assert.True(IcicleMonsterCore.SuckChancePasses(100, 99));
        Assert.False(IcicleMonsterCore.SuckChancePasses(0, 0));
    }

    [Fact]
    public void SuckGateTruthTable()
    {
        Assert.True(IcicleMonsterCore.SuckGateTruthTable());
    }

    [Fact]
    public void SuckAmountValues()
    {
        Assert.True(IcicleMonsterCore.SuckAmountValues());
        Assert.Equal(100, IcicleMonsterCore.SuckAmount(1000, 100));
        Assert.Equal(50, IcicleMonsterCore.SuckAmount(500, 100));
    }

    [Fact]
    public void SuckClampedByPointPool()
    {
        // **再被 m_nSuckDamagePoint 夹住**
        Assert.True(IcicleMonsterCore.SuckClampedByPointPool());
        Assert.Equal(30, IcicleMonsterCore.SuckClamped(1000, 100, 30));
        Assert.Equal(10, IcicleMonsterCore.SuckClamped(100, 100, 30));
    }

    [Fact]
    public void PowerAfterSuckFloorsAtZero()
    {
        Assert.True(IcicleMonsterCore.PowerAfterSuckFloorsAtZero());
    }

    [Fact]
    public void ParalysisThreeConditions()
    {
        Assert.True(IcicleMonsterCore.ParalysisThreeConditions());
        Assert.True(IcicleMonsterCore.ImmuneNotParalyzed());
        Assert.True(IcicleMonsterCore.BoParalysisSkipsRoll());
        Assert.True(IcicleMonsterCore.UnParalysisMeansImmune());
    }

    [Fact]
    public void ModulusCanBeZero()
    {
        // **模数可为 0** → Random(0) 未定义
        Assert.True(IcicleMonsterCore.ModulusCanBeZero());
        Assert.True(IcicleMonsterCore.HighAntiPoisonIncreasesModulus());
        Assert.Equal(200, IcicleMonsterCore.RollModulus(200, 0));
    }

    [Fact]
    public void ReboundMarkers()
    {
        Assert.True(IcicleMonsterCore.ReboundSendsFTMarker());
        Assert.True(IcicleMonsterCore.MainStruckSendsEmptyMarker());
        Assert.True(IcicleMonsterCore.ReboundTargetsNil());
    }

    [Fact]
    public void LightingDirectionIsLiteralOne()
    {
        // **特效方向是字面量 1，不是 m_btDirection**
        Assert.True(IcicleMonsterCore.LightingDirectionIsLiteralOne());
        Assert.Equal(1, IcicleMonsterCore.LightingDirection());
    }

    [Fact]
    public void DelayFormula()
    {
        Assert.True(IcicleMonsterCore.DelayFormula());
        Assert.Equal(600, IcicleMonsterCore.DelayOf(0, 0));
        Assert.Equal(650, IcicleMonsterCore.DelayOf(1, 0));
        Assert.Equal(750, IcicleMonsterCore.DelayOf(2, 3));
    }

    [Fact]
    public void DelayUsesChebyshev()
    {
        // **用切比雪夫距离（_MAX）而非曼哈顿**
        Assert.True(IcicleMonsterCore.DelayUsesChebyshev());
        Assert.Equal(750, IcicleMonsterCore.DelayOf(3, 3));
    }

    [Fact]
    public void TwoDistanceMetricsInOneClass()
    {
        // **同一类里混用曼哈顿与切比雪夫**
        Assert.True(IcicleMonsterCore.TwoDistanceMetricsInOneClass());
    }

    // ===================== 六、TWallStructure 其余方法 =====================

    [Fact]
    public void CreateDiffersOnlyInThird()
    {
        // **与 TCastleDoor.Create 只差第三项**
        Assert.True(IcicleMonsterCore.CreateDiffersOnlyInThird());
        Assert.Equal((false, true, false, 200), IcicleMonsterCore.WallCreateInit());
    }

    [Fact]
    public void InitializeOrderDiffersFromDoor()
    {
        Assert.True(IcicleMonsterCore.InitializeOrderDiffersFromDoor());
        Assert.Equal(0, IcicleMonsterCore.WallInitDirection());
        Assert.True(IcicleMonsterCore.WallDirectionZeroIsLive());
    }

    [Fact]
    public void WallClampIsGeFive()
    {
        // **墙归零条件是 n08 >= 5**
        Assert.True(IcicleMonsterCore.WallClampIsGeFive());
        Assert.Equal(4, IcicleMonsterCore.WallClamp(4));
        Assert.Equal(0, IcicleMonsterCore.WallClamp(5));
    }

    [Fact]
    public void WallAllowsThreeAndFour()
    {
        // **墙允许 3 与 4 存活，门不允许 3**
        Assert.True(IcicleMonsterCore.WallAllowsThreeAndFour());
        Assert.True(IcicleMonsterCore.DoorForbidsThree());
    }

    [Fact]
    public void WallDirectionTable()
    {
        Assert.True(IcicleMonsterCore.WallDirectionTable());
        Assert.Equal(0, IcicleMonsterCore.WallDirection(100, 100));
        Assert.Equal(4, IcicleMonsterCore.WallDirection(0, 0));
        Assert.Equal(1, IcicleMonsterCore.WallDirection(50, 100));
    }

    [Fact]
    public void WallRefStatusSendsAlive()
    {
        Assert.True(IcicleMonsterCore.WallRefStatusSendsAlive());
        Assert.Equal(4, IcicleMonsterCore.WallRefStatusDeathDirection());
    }

    [Fact]
    public void DieSendsFinalStateOnce()
    {
        // **仅当方向不是 4 时才发一次**
        Assert.True(IcicleMonsterCore.DieSendsFinalStateOnce());
        Assert.True(IcicleMonsterCore.DieSkipsWhenAlreadyFour());
        Assert.True(IcicleMonsterCore.DieSendsWhenNotFour(0));
        Assert.False(IcicleMonsterCore.DieSendsWhenNotFour(4));
    }

    [Fact]
    public void SameMessageAsDoorOpen()
    {
        // **墙 Die 用 RM_DIGUP，与门 Open 同消息**
        Assert.True(IcicleMonsterCore.SameMessageAsDoorOpen());
        Assert.Equal(20099, IcicleMonsterCore.RmDigUp);
    }

    [Fact]
    public void Dw560SharedWithDoor()
    {
        Assert.True(IcicleMonsterCore.Dw560SharedWithDoor());
        Assert.True(IcicleMonsterCore.WallDieWritesDw560());
        Assert.True(IcicleMonsterCore.TwoClassesShareDw560());
        Assert.Equal(new[] { "TCastleDoor", "TWallStructure" }, IcicleMonsterCore.ClassesSharingDw560);
    }

    // ===================== 七、仿真 =====================

    [Fact]
    public void RunSelectPhaseGuards()
    {
        Assert.True(IcicleMonsterCore.DeadDoesNotSelect());
        Assert.True(IcicleMonsterCore.GhostDoesNotSelect());
        Assert.True(IcicleMonsterCore.CannotMoveDoesNotSelect());
    }

    [Fact]
    public void TickNotDueSkipsSelection()
    {
        // **节拍未到时不清靶（返回 -2）**
        Assert.True(IcicleMonsterCore.TickNotDueSkipsSelection());
        Assert.True(IcicleMonsterCore.DueWithNoTargetClears());
        Assert.True(IcicleMonsterCore.DueWithTargetPicks());
    }

    [Fact]
    public void OfflineFilteredInFullSelect()
    {
        // **配置打开时脱机玩家被过滤**
        Assert.True(IcicleMonsterCore.OfflineFilteredInFullSelect());
    }

    [Fact]
    public void TruckFilteredInFullSelect()
    {
        Assert.True(IcicleMonsterCore.TruckFilteredInFullSelect());
    }

    [Fact]
    public void FullSelectPicksNearestEligible()
    {
        var visible = new (IcicleMonsterCore.Actor, int)[]
        {
            (new IcicleMonsterCore.Actor { RaceServer = 80, Proper = true }, 2),
            (new IcicleMonsterCore.Actor { RaceServer = 128, Proper = true }, 1),
            (new IcicleMonsterCore.Actor { RaceServer = 80, Proper = true }, 5),
        };

        // 镖车虽近（1）但被过滤 → 选索引 0（距离 2）
        Assert.Equal(0, IcicleMonsterCore.FullSelect(false, false, true, true, false, visible));
    }

    [Fact]
    public void FullSelectBeyondRangePicksNothing()
    {
        var visible = new (IcicleMonsterCore.Actor, int)[]
        {
            (new IcicleMonsterCore.Actor { RaceServer = 80, Proper = true }, 10),
        };

        Assert.Equal(-1, IcicleMonsterCore.FullSelect(false, false, true, true, false, visible));
    }
}
