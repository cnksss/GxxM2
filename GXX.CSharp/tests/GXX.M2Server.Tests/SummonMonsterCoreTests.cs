using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J138：召唤系怪物 —— `TBeeQueen` / `TCentipedeKingMonster` / `TSpiderHouseMonster`
/// （ObjMon2.pas 24-64、701-769、773-999、1087-1167）1:1 测试。
/// </summary>
public sealed class SummonMonsterCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(SummonMonsterCore.ConstantsMatchSource());
        Assert.Equal(20006, SummonMonsterCore.RmHit);
        Assert.Equal(20109, SummonMonsterCore.RmZenBee);
        Assert.Equal(30004, SummonMonsterCore.RmDelayMagic);
        Assert.Equal(0, SummonMonsterCore.PoisonDecHealth);
        Assert.Equal(5, SummonMonsterCore.PoisonStone);
        Assert.Equal(15, SummonMonsterCore.BbListCap);
    }

    [Fact]
    public void DefaultNames()
    {
        Assert.True(SummonMonsterCore.DefaultNames());
        Assert.Equal("小角蝇", SummonMonsterCore.DefaultBeeName);
        Assert.Equal("小蜘蛛", SummonMonsterCore.DefaultSpiderName);
    }

    [Fact]
    public void ViewRangeValues()
    {
        Assert.True(SummonMonsterCore.ViewRangeValues());
        Assert.Equal(9, SummonMonsterCore.BeeViewRange);
        Assert.Equal(9, SummonMonsterCore.SpiderViewRange);
        Assert.Equal(8, SummonMonsterCore.CentipedeViewRange);
        Assert.Equal(7, SummonMonsterCore.StickViewRange);
    }

    [Fact]
    public void CommentIsRelativeNotAbsolute()
    {
        // **注释说"增大"，但 8 比蜜蜂的 9 小 —— 是相对自身历史值而言**
        Assert.True(SummonMonsterCore.CommentIsRelativeNotAbsolute());
        Assert.True(SummonMonsterCore.ViewRangeCommentHasDate());
    }

    [Fact]
    public void CommentsMatchSource()
    {
        Assert.True(SummonMonsterCore.CommentsMatchSource());
        Assert.Equal("// 0x560", SummonMonsterCore.AttickTickOffset);
        Assert.Equal("// 防毒", SummonMonsterCore.AntiPoisonComment);
        Assert.Contains("去掉定身", SummonMonsterCore.CanMoveComment);
    }

    // ===================== 一、比较符的两代分野 =====================

    [Fact]
    public void TwoGenerationsOfComparison()
    {
        // **召唤型用 >=、潜伏型用 >**
        Assert.True(SummonMonsterCore.BeeAndSpiderUseGe());
        Assert.True(SummonMonsterCore.CentipedeUsesGt());
        Assert.True(SummonMonsterCore.TwoGenerationsOfComparison());
    }

    [Fact]
    public void EqualitySeparatesStyles()
    {
        // **恰好相等时两种风格结果不同**
        Assert.True(SummonMonsterCore.EqualitySeparatesStyles());
        Assert.True(SummonMonsterCore.Due(0, 5, 3, 2, SummonMonsterCore.CompareStyle.GreaterOrEqual));
        Assert.False(SummonMonsterCore.Due(0, 5, 3, 2, SummonMonsterCore.CompareStyle.Greater));
    }

    [Fact]
    public void TickDiffWrapsShort()
    {
        Assert.Equal(100u, SummonMonsterCore.TickDiff(100, 200));
        Assert.Equal(0u, SummonMonsterCore.TickDiff(uint.MaxValue, 0));
    }

    [Fact]
    public void BothUseGhostFirstOrder()
    {
        Assert.True(SummonMonsterCore.BothUseGhostFirstOrder());
        Assert.True(SummonMonsterCore.SameOrderAsStickMonster());
        Assert.True(SummonMonsterCore.RunGateTruthTable());
    }

    // ===================== 二、BBList 清理的两种写法 =====================

    [Fact]
    public void BeeNilGuardEscaped()
    {
        // **蜜蜂版的运算符优先级陷阱：ghost 分支在 nil 时仍被求值**
        Assert.True(SummonMonsterCore.BeeNilGuardEscaped());
        Assert.True(SummonMonsterCore.BeeAndBindsTighterThanOr());
        Assert.True(SummonMonsterCore.NilCaseDiffers());
    }

    [Fact]
    public void SpiderNilGuardIsCorrect()
    {
        // **蜘蛛版修掉了蜜蜂版的空指针问题**
        Assert.True(SummonMonsterCore.SpiderNilGuardIsCorrect());
        Assert.True(SummonMonsterCore.SpiderFixesBeeBug());
        Assert.True(SummonMonsterCore.AnotherSiblingCodePairPrecedent());
    }

    [Fact]
    public void RemovePredicateValues()
    {
        // 健康对象：两者都不删
        Assert.False(SummonMonsterCore.BeeRemovePredicate(false, false, false));
        Assert.False(SummonMonsterCore.SpiderRemovePredicate(false, false, false));

        // 已死：两者都删
        Assert.True(SummonMonsterCore.BeeRemovePredicate(false, true, false));
        Assert.True(SummonMonsterCore.SpiderRemovePredicate(false, true, false));

        // 已幽灵：两者都删
        Assert.True(SummonMonsterCore.BeeRemovePredicate(false, false, true));
        Assert.True(SummonMonsterCore.SpiderRemovePredicate(false, false, true));

        // nil 且幽灵：蜜蜂删（危险）、蜘蛛不删
        Assert.True(SummonMonsterCore.BeeRemovePredicate(true, false, true));
        Assert.False(SummonMonsterCore.SpiderRemovePredicate(true, false, true));

        // nil 且非幽灵：两者都不删
        Assert.False(SummonMonsterCore.BeeRemovePredicate(true, false, false));
        Assert.False(SummonMonsterCore.SpiderRemovePredicate(true, false, false));
    }

    [Fact]
    public void BeeLacksCountGuard()
    {
        Assert.True(SummonMonsterCore.BeeLacksCountGuard());
        Assert.True(SummonMonsterCore.SpiderHasExtraCountBreak());
    }

    [Fact]
    public void CountBreakIsRedundant()
    {
        // **实测：倒序删除不会越界，故那个 Break 是安全的冗余**
        Assert.True(SummonMonsterCore.CountBreakIsRedundant());
        Assert.True(SummonMonsterCore.DescendingDeleteNeverOverflows());
        Assert.True(SummonMonsterCore.GuardMakesNoDifference());
    }

    [Fact]
    public void PruneHelpers()
    {
        Assert.True(SummonMonsterCore.PruneWithGuardWorks());
        Assert.True(SummonMonsterCore.PruneWithGuardPartial());

        // 全删
        Assert.Empty(SummonMonsterCore.PruneWithoutGuard(new[] { true, true, true, true }));

        // 只删最后一个
        Assert.Equal(new[] { 0, 1, 2 }, SummonMonsterCore.PruneWithoutGuard(new[] { false, false, false, true }));

        // 只删第一个
        Assert.Equal(new[] { 1, 2, 3 }, SummonMonsterCore.PruneWithoutGuard(new[] { true, false, false, false }));
    }

    [Fact]
    public void PruneBreakNeverFires()
    {
        for (int mask = 0; mask < 16; mask++)
        {
            var remove = new bool[4];

            for (int b = 0; b < 4; b++)
                remove[b] = (mask & (1 << b)) != 0;

            Assert.False(SummonMonsterCore.PruneWithGuard(remove).BreakFired);
        }
    }

    // ===================== 三、召唤逻辑 =====================

    [Fact]
    public void SpawnPositionsDiffer()
    {
        // **蜜蜂原地、蜘蛛下方一格**
        Assert.True(SummonMonsterCore.SpawnPositionsDiffer());
        Assert.True(SummonMonsterCore.BeeSpawnsInPlace());
        Assert.True(SummonMonsterCore.SpiderSpawnsOneBelow());
        Assert.Equal((10, 20), SummonMonsterCore.BeeSpawnPos(10, 20));
        Assert.Equal((10, 21), SummonMonsterCore.SpiderSpawnPos(10, 20));
    }

    [Fact]
    public void SpiderRequiresWalkable()
    {
        // **蜘蛛要求下方可走，且失败时静默**
        Assert.True(SummonMonsterCore.SpiderRequiresWalkable());
        Assert.True(SummonMonsterCore.SpiderSilentlyFails());
        Assert.True(SummonMonsterCore.BeeHasNoTerrainCheck());
    }

    [Fact]
    public void CapGatesEquivalent()
    {
        // **写法相反但完全等价**
        Assert.True(SummonMonsterCore.BothCapAtFifteen());
        Assert.True(SummonMonsterCore.CapCheckInverted());
        Assert.True(SummonMonsterCore.CapGatesEquivalent());
        Assert.True(SummonMonsterCore.CapAtExactlyFifteen());
    }

    [Fact]
    public void BeeCanMakeValues()
    {
        Assert.True(SummonMonsterCore.BeeCanMake(0));
        Assert.True(SummonMonsterCore.BeeCanMake(14));
        Assert.False(SummonMonsterCore.BeeCanMake(15));
        Assert.False(SummonMonsterCore.BeeCanMake(20));
    }

    [Fact]
    public void OffspringInheritsTarget()
    {
        Assert.True(SummonMonsterCore.OffspringInheritsTarget());
        Assert.True(SummonMonsterCore.NilTargetPropagates());
        Assert.True(SummonMonsterCore.OnlyAddNonNullOffspring());
        Assert.True(SummonMonsterCore.HandlesOffspring(false));
        Assert.False(SummonMonsterCore.HandlesOffspring(true));
    }

    [Fact]
    public void DelayedSelfMessagePattern()
    {
        // **RM_HIT + 延迟 500 的自发 RM_ZEN_BEE**
        Assert.True(SummonMonsterCore.TwoMessagesSentForSpawning());
        Assert.True(SummonMonsterCore.DelayedSelfMessagePattern());
        Assert.True(SummonMonsterCore.DelayIs500());
        Assert.Equal(20109, SummonMonsterCore.SelfMessageIdent());
        Assert.True(SummonMonsterCore.IdentMatches());
        Assert.True(SummonMonsterCore.BothForwardInheritedOperate());
    }

    [Fact]
    public void HitArgsValues()
    {
        Assert.Equal(20006, SummonMonsterCore.HitArgs().Msg);
        Assert.Equal(0, SummonMonsterCore.HitArgs().Arg5);
    }

    [Fact]
    public void SearchTickInitDiffers()
    {
        // **蜜蜂用当前时刻、蜘蛛用 0**
        Assert.True(SummonMonsterCore.SearchTickInitDiffers());
        Assert.True(SummonMonsterCore.BeeSearchTickIsNow());
        Assert.True(SummonMonsterCore.SpiderSearchTickZeroMeansImmediate());
        Assert.True(SummonMonsterCore.SearchTimeSharedWithStick());
        Assert.True(SummonMonsterCore.SearchTimeMatchesStick());
    }

    // ===================== 四、蜈蚣王 =====================

    [Fact]
    public void CentipedeOverridesN558()
    {
        // **蜈蚣王把 n558 覆盖为 6**
        Assert.True(SummonMonsterCore.CentipedeOverridesN558());
        Assert.True(SummonMonsterCore.TriggerRadiusUnchanged());
        Assert.Equal(6, SummonMonsterCore.CentipedeLeashRadius);
    }

    [Fact]
    public void DeadZoneWidensToThreeValues()
    {
        // **死区从 {4} 扩大到 {4,5,6}**
        Assert.True(SummonMonsterCore.DeadZoneWidensToThreeValues());
        Assert.True(SummonMonsterCore.StickDeadZoneIsOne());
        Assert.Equal(new[] { 4, 5, 6 }, SummonMonsterCore.CentipedeDeadZone());
    }

    [Fact]
    public void ThreeTierTimingStateMachine()
    {
        Assert.True(SummonMonsterCore.ThreeTierTimingStateMachine());
        Assert.True(SummonMonsterCore.TwoThresholds3000And10000());
        Assert.True(SummonMonsterCore.Nested10000Inside3000());
        Assert.True(SummonMonsterCore.NestedSemantics());
    }

    [Fact]
    public void TimingThresholdValues()
    {
        // 3000 门：恰好 3000 不成立（严格大于）
        Assert.False(SummonMonsterCore.AttackDue(0, 3000));
        Assert.True(SummonMonsterCore.AttackDue(0, 3001));

        // 10000 门
        Assert.False(SummonMonsterCore.HideScanDue(0, 10000));
        Assert.True(SummonMonsterCore.HideScanDue(0, 10001));
    }

    [Fact]
    public void UnhideBehaviorDiffers()
    {
        // **蜈蚣王从潜伏现身时回满血，钉刺怪不回血**
        Assert.True(SummonMonsterCore.CentipedeUnhideRestoresFullHp());
        Assert.True(SummonMonsterCore.StickMonsterDoesNotHeal());
        Assert.True(SummonMonsterCore.UnhideBehaviorDiffers());
        Assert.True(SummonMonsterCore.UnhideRestoresHp());
        Assert.True(SummonMonsterCore.StickUnhideKeepsHp());
        Assert.True(SummonMonsterCore.BothAgreeWhenAlreadyFull());

        // 同一组输入跑两个实现，结果确实不同（且已满血时相同）
        Assert.NotEqual(SummonMonsterCore.AfterCentipedeUnhide(100, 500),
            SummonMonsterCore.AfterStickUnhide(100, 500));
        Assert.Equal((500, 500), SummonMonsterCore.AfterCentipedeUnhide(100, 500));
        Assert.Equal((100, 500), SummonMonsterCore.AfterStickUnhide(100, 500));
    }

    [Fact]
    public void CentipedeHitsAllVisible()
    {
        // **群体攻击，目标是最后一个命中者**
        Assert.True(SummonMonsterCore.CentipedeHitsAllVisible());
        Assert.True(SummonMonsterCore.TargetBecomesLastHit());
        Assert.Equal(9, SummonMonsterCore.LastHitTarget(new[] { 3, 7, 9 }));
    }

    [Fact]
    public void ReusesDevilkingRandomSpan()
    {
        // **复用恶魔弓箭手的随机跨度公式（含 SmallInt 截断风险）**
        Assert.True(SummonMonsterCore.ReusesDevilkingRandomSpan());
        Assert.True(SummonMonsterCore.SameTruncationRisk());
        Assert.Equal(11, SummonMonsterCore.RandomSpan(10, 20));
        Assert.Equal(-25535, SummonMonsterCore.RandomSpan(0, 40000));
    }

    [Fact]
    public void CommentedEquivalentFormula()
    {
        Assert.True(SummonMonsterCore.CommentedEquivalentFormula());
        Assert.True(SummonMonsterCore.CommentedFormulaPresent());
        Assert.Contains("HiWord", SummonMonsterCore.CommentedFormula[0]);
    }

    [Fact]
    public void RangeCheckInclusive()
    {
        // **视野判定用 <=（含边界）**
        Assert.True(SummonMonsterCore.RangeCheckInclusive());
        Assert.True(SummonMonsterCore.InViewRange(8, 8, 8));
        Assert.False(SummonMonsterCore.InViewRange(9, 0, 8));
        Assert.True(SummonMonsterCore.RangeStyleDiffersFromStick());
    }

    // ===================== 五、三层随机中毒 =====================

    [Fact]
    public void ThreeLayerRandomPoison()
    {
        Assert.True(SummonMonsterCore.ThreeLayerRandomPoison());
        Assert.True(SummonMonsterCore.PoisonBranchSplit());
        Assert.True(SummonMonsterCore.PoisonProbabilityValues());
        Assert.Equal((2, 1, 12), SummonMonsterCore.PoisonProbabilities());
    }

    [Fact]
    public void PoisonLayerValues()
    {
        Assert.True(SummonMonsterCore.FirstLayer(0));
        Assert.False(SummonMonsterCore.FirstLayer(1));

        Assert.True(SummonMonsterCore.SecondLayerToGreenPoison(1));
        Assert.True(SummonMonsterCore.SecondLayerToGreenPoison(2));
        Assert.False(SummonMonsterCore.SecondLayerToGreenPoison(0));
    }

    [Fact]
    public void GreenPoisonTruthTable()
    {
        Assert.True(SummonMonsterCore.GreenPoisonTruthTable());
        Assert.True(SummonMonsterCore.GreenPoisonGate(false, 0, 0));
        Assert.False(SummonMonsterCore.GreenPoisonGate(true, 0, 0));
    }

    [Fact]
    public void PoisonParams()
    {
        Assert.True(SummonMonsterCore.GreenPoisonParamsMatch());
        Assert.True(SummonMonsterCore.StoneParamsMatch());
        Assert.Equal((0, 60, 3), SummonMonsterCore.GreenPoisonParams());
        Assert.Equal((5, 5, 0), SummonMonsterCore.StoneParams());
    }

    [Fact]
    public void GreenLastsLongerThanStone()
    {
        // **绿毒 60 远长于石化 5**
        Assert.True(SummonMonsterCore.GreenLastsLongerThanStone());
        Assert.True(SummonMonsterCore.GreenModulusNeverZero());
    }

    // ===================== 六、默认参数与 CanMove =====================

    [Fact]
    public void CanStoneDefaultParam()
    {
        // **CanStone 默认参数 0 → Random(m_btAntiPoison) = 0**
        Assert.True(SummonMonsterCore.CanStoneDefaultParam());
        Assert.Equal(0, SummonMonsterCore.CanStoneDefault());
        Assert.True(SummonMonsterCore.CanStone(false, 100, 0, 0));
        Assert.False(SummonMonsterCore.CanStone(false, 100, 0, 1));
    }

    [Fact]
    public void CanStoneRandomZeroHazard()
    {
        // **m_btAntiPoison 为 0 时 Random(0) 未定义**
        Assert.True(SummonMonsterCore.CanStoneRandomZeroHazard());
    }

    [Fact]
    public void SendAttackMsgDefaultParam()
    {
        // **第五参默认 0，调用点只给四个实参**
        Assert.True(SummonMonsterCore.SendAttackMsgDefaultParam());
        Assert.Equal(0, SummonMonsterCore.SendAttackMsgArgs().NewLevel);
        Assert.True(SummonMonsterCore.DefaultParamPortingHazard());
        Assert.True(SummonMonsterCore.TwoMethodsUseDefaults());
    }

    [Fact]
    public void CanMoveFiveConditions()
    {
        Assert.True(SummonMonsterCore.CanMoveFiveConditions());
        Assert.True(SummonMonsterCore.CanMove(false, false, false, false, false));
        Assert.False(SummonMonsterCore.CanMove(true, false, false, false, false));
        Assert.False(SummonMonsterCore.CanMove(false, false, false, false, true));
    }

    [Fact]
    public void DingShenCommentedOut()
    {
        // **定身那一项被注释掉，故不再阻止移动**
        Assert.True(SummonMonsterCore.DingShenCommentedOut());
        Assert.Equal(1, SummonMonsterCore.CommentedOutConditions());
        Assert.True(SummonMonsterCore.CanMovePatchComment());
        Assert.True(SummonMonsterCore.DingShenNoLongerBlocks());
    }

    // ===================== 七、仿真 =====================

    [Fact]
    public void SummonerGateBlocks()
    {
        Assert.True(SummonMonsterCore.SummonerGateBlocks());
    }

    [Fact]
    public void SummonerTickNotDue()
    {
        Assert.True(SummonMonsterCore.SummonerTickNotDue());
    }

    [Fact]
    public void SummonerSpawnsWithTarget()
    {
        Assert.True(SummonMonsterCore.SummonerSpawnsWithTarget());
        Assert.True(SummonMonsterCore.SummonerSearchesWithoutTarget());
    }

    [Fact]
    public void SummonerRespectsCap()
    {
        Assert.True(SummonMonsterCore.SummonerRespectsCap());
    }

    [Fact]
    public void SummonerPrunes()
    {
        // **清理已死与已幽灵的召唤物（保留健康的）**
        Assert.True(SummonMonsterCore.SummonerPrunes());
    }

    [Fact]
    public void SummonerRunValues()
    {
        var r = SummonMonsterCore.SummonerRun(false, false, true, true, true, true,
            Array.Empty<(bool, bool, bool)>());

        Assert.True(r.Searched);
        Assert.True(r.Spawned);
        Assert.Equal(0, r.Removed);
        Assert.Equal("ran", r.Path);
    }
}
