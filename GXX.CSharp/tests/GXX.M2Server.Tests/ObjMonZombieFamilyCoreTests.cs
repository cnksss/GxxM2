using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J245：`ObjMon.pas` 中僵尸族三兄弟（`TLightingZombi`、`TDigOutZombi`、`TZilKinZombi`）
/// 十二方法的 1:1 测试（203 行）。
/// **本批最有价值的发现**：
/// ① `TDigOutZombi.sub_4AA8DC` 的四行**正是 J233 里被 `{ }` 禁用的那一块** ——
///    "被停用的那一代并没有消失、它在别的类里还在用"；
/// ② `LightingAttack` 是**以两点定义**的贯穿光束（近点距离 1 检查、远点距离 9 丢弃），
///    而它的别名行 **2107 正是 J212 记的 13 处之一** ⇒ 13 处里已有**四处**被移植；
/// ③ `TZilKinZombi` 的复活计数**读写阈值不一致**（`Die` 判 `> 0`、`Run` 判 `>= 0`、`Dec` 在 `if` 外）；
/// ④ `Run` 的搜索节流把标准写法的 `or` **改成了嵌套** ⇒ 有目标时永不重搜。
/// </summary>
public sealed class ObjMonZombieFamilyCoreTests
{
    [Fact]
    public void Constants()
    {
        Assert.Equal(2084, ObjMonZombieFamilyCore.LzCreateStart);
        Assert.Equal(2090, ObjMonZombieFamilyCore.LzCreateEnd);
        Assert.Equal(7, ObjMonZombieFamilyCore.LzCreateLines);
        Assert.Equal(2092, ObjMonZombieFamilyCore.LzDestroyStart);
        Assert.Equal(2095, ObjMonZombieFamilyCore.LzDestroyEnd);
        Assert.Equal(4, ObjMonZombieFamilyCore.LzDestroyLines);
        Assert.Equal(2097, ObjMonZombieFamilyCore.AttackStart);
        Assert.Equal(2121, ObjMonZombieFamilyCore.AttackEnd);
        Assert.Equal(25, ObjMonZombieFamilyCore.AttackLines);
        Assert.Equal(2123, ObjMonZombieFamilyCore.LzRunStart);
        Assert.Equal(2160, ObjMonZombieFamilyCore.LzRunEnd);
        Assert.Equal(38, ObjMonZombieFamilyCore.LzRunLines);

        Assert.Equal(2163, ObjMonZombieFamilyCore.DoCreateStart);
        Assert.Equal(2172, ObjMonZombieFamilyCore.DoCreateEnd);
        Assert.Equal(10, ObjMonZombieFamilyCore.DoCreateLines);
        Assert.Equal(2174, ObjMonZombieFamilyCore.DoDestroyStart);
        Assert.Equal(2177, ObjMonZombieFamilyCore.DoDestroyEnd);
        Assert.Equal(4, ObjMonZombieFamilyCore.DoDestroyLines);
        Assert.Equal(2179, ObjMonZombieFamilyCore.DigUpStart);
        Assert.Equal(2187, ObjMonZombieFamilyCore.DigUpEnd);
        Assert.Equal(9, ObjMonZombieFamilyCore.DigUpLines);
        Assert.Equal(2189, ObjMonZombieFamilyCore.DoRunStart);
        Assert.Equal(2249, ObjMonZombieFamilyCore.DoRunEnd);
        Assert.Equal(61, ObjMonZombieFamilyCore.DoRunLines);

        Assert.Equal(2252, ObjMonZombieFamilyCore.ZkCreateStart);
        Assert.Equal(2264, ObjMonZombieFamilyCore.ZkCreateEnd);
        Assert.Equal(13, ObjMonZombieFamilyCore.ZkCreateLines);
        Assert.Equal(2266, ObjMonZombieFamilyCore.ZkDestroyStart);
        Assert.Equal(2269, ObjMonZombieFamilyCore.ZkDestroyEnd);
        Assert.Equal(4, ObjMonZombieFamilyCore.ZkDestroyLines);
        Assert.Equal(2271, ObjMonZombieFamilyCore.DieStart);
        Assert.Equal(2283, ObjMonZombieFamilyCore.DieEnd);
        Assert.Equal(13, ObjMonZombieFamilyCore.DieLines);
        Assert.Equal(2285, ObjMonZombieFamilyCore.ZkRunStart);
        Assert.Equal(2299, ObjMonZombieFamilyCore.ZkRunEnd);
        Assert.Equal(15, ObjMonZombieFamilyCore.ZkRunLines);

        Assert.Equal(203, ObjMonZombieFamilyCore.TotalLines);
        Assert.Equal(12, ObjMonZombieFamilyCore.MethodCount);
        Assert.Equal(3, ObjMonZombieFamilyCore.ClassCount);

        Assert.Equal(2102, ObjMonZombieFamilyCore.DirLine);
        Assert.Equal(2103, ObjMonZombieFamilyCore.EffectLine);
        Assert.Equal(1, ObjMonZombieFamilyCore.EffectId);
        Assert.Equal(2104, ObjMonZombieFamilyCore.NearProbeLine);
        Assert.Equal(1, ObjMonZombieFamilyCore.NearDistance);
        Assert.Equal(2106, ObjMonZombieFamilyCore.FarProbeLine);
        Assert.Equal(9, ObjMonZombieFamilyCore.FarDistance);
        Assert.Equal(2117, ObjMonZombieFamilyCore.PassThroughLine);
        Assert.Equal(2120, ObjMonZombieFamilyCore.BreakSeizeLine);
        Assert.Equal(2107, ObjMonZombieFamilyCore.AliasLine);
        Assert.Equal(2108, ObjMonZombieFamilyCore.OriginalCommentLine);

        Assert.Equal(2127, ObjMonZombieFamilyCore.OuterGuardLine);
        Assert.Equal(2129, ObjMonZombieFamilyCore.InnerSearchLine);
        Assert.Equal(8000, ObjMonZombieFamilyCore.SearchWithTargetMs);
        Assert.Equal(1000, ObjMonZombieFamilyCore.SearchWithoutTargetMs);
        Assert.Equal(2134, ObjMonZombieFamilyCore.WalkBlockLine);
        Assert.Equal(4, ObjMonZombieFamilyCore.WalkRadius);
        Assert.Equal(2, ObjMonZombieFamilyCore.CloseRadius);
        Assert.Equal(3, ObjMonZombieFamilyCore.CloseRollBound);
        Assert.Equal(2143, ObjMonZombieFamilyCore.BackPosLine);
        Assert.Equal(2145, ObjMonZombieFamilyCore.RelaxLine);
        Assert.Equal(2150, ObjMonZombieFamilyCore.AttackGateLine);
        Assert.Equal(6, ObjMonZombieFamilyCore.AttackRadius);
        Assert.Equal(2156, ObjMonZombieFamilyCore.AttackCallLine);

        Assert.Equal(2166, ObjMonZombieFamilyCore.Bo554InitLine);
        Assert.Equal(2170, ObjMonZombieFamilyCore.RaceServerLine);
        Assert.Equal(95, ObjMonZombieFamilyCore.RaceServerValue);
        Assert.Equal(96, ObjMonZombieFamilyCore.ZilKinRaceValue);
        Assert.Equal(100, ObjMonZombieFamilyCore.WhiteSkeletonRaceValue);
        Assert.Equal(2168, ObjMonZombieFamilyCore.SearchTimeLine);
        Assert.Equal(2500, ObjMonZombieFamilyCore.SearchTimeBase);
        Assert.Equal(300000, ObjMonZombieFamilyCore.EventDurationMs);
        Assert.Equal(2183, ObjMonZombieFamilyCore.EventCreateLine);
        Assert.Equal(2184, ObjMonZombieFamilyCore.AddEventLine);
        Assert.Equal(2186, ObjMonZombieFamilyCore.DigUpMsgLine);
        Assert.Equal(2201, ObjMonZombieFamilyCore.LockLine);
        Assert.Equal(2234, ObjMonZombieFamilyCore.FinallyLine);
        Assert.Equal(2235, ObjMonZombieFamilyCore.UnlockLine);
        Assert.Equal(2213, ObjMonZombieFamilyCore.OfflineFilterLine);
        Assert.Equal(3, ObjMonZombieFamilyCore.RevealRadius);
        Assert.Equal(1000, ObjMonZombieFamilyCore.RevealWalkDelay);

        Assert.Equal(2259, ObjMonZombieFamilyCore.CountInitLine);
        Assert.Equal(2260, ObjMonZombieFamilyCore.CountRollLine);
        Assert.Equal(2262, ObjMonZombieFamilyCore.CountSetLine);
        Assert.Equal(2277, ObjMonZombieFamilyCore.DieGreaterLine);
        Assert.Equal(2279, ObjMonZombieFamilyCore.DieTickLine);
        Assert.Equal(2280, ObjMonZombieFamilyCore.DieDelayLine);
        Assert.Equal(2282, ObjMonZombieFamilyCore.DecLine);
        Assert.Equal(2287, ObjMonZombieFamilyCore.RunGreaterEqualLine);
        Assert.Equal(2290, ObjMonZombieFamilyCore.HalfMaxHpLine);
        Assert.Equal(2291, ObjMonZombieFamilyCore.HalfExpLine);
        Assert.Equal(2294, ObjMonZombieFamilyCore.ReAliveLine);
        Assert.Equal(20, ObjMonZombieFamilyCore.ReviveRollBound);
        Assert.Equal(4, ObjMonZombieFamilyCore.ReviveBaseSeconds);
        Assert.Equal(2276, ObjMonZombieFamilyCore.NoDropLine);
        Assert.Equal(2087, ObjMonZombieFamilyCore.ViewRangeCommentLine);
        Assert.Equal(7, ObjMonZombieFamilyCore.DigOutViewRange);
        Assert.Equal(6, ObjMonZombieFamilyCore.ZilKinViewRange);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonZombieFamilyCore.SpanMatches());
        Assert.True(ObjMonZombieFamilyCore.TotalLinesAddUp());
        Assert.True(ObjMonZombieFamilyCore.MethodsAscending());
        Assert.True(ObjMonZombieFamilyCore.WithinUnit());
        Assert.True(ObjMonZombieFamilyCore.NoInstrumentation());
    }

    // ===================== 一、贯穿光束 =====================

    [Fact]
    public void BeamFacts()
    {
        Assert.True(ObjMonZombieFamilyCore.TwoPointBeam());
        Assert.True(ObjMonZombieFamilyCore.NearProbeCheckedFarDiscarded());
        Assert.True(ObjMonZombieFamilyCore.MagPassThroughMagic());
        Assert.True(ObjMonZombieFamilyCore.ThirdBeamStyle());
        Assert.True(ObjMonZombieFamilyCore.BreakSeizeOutsideTheIf());
        Assert.True(ObjMonZombieFamilyCore.BeamLengthNine());
        Assert.True(ObjMonZombieFamilyCore.StartsAtOne());
        Assert.True(ObjMonZombieFamilyCore.BothEndsGiven());

        Assert.Equal(new[] { 1, 9 }, ObjMonZombieFamilyCore.BeamTiles());
    }

    [Fact]
    public void InlineRollFacts()
    {
        Assert.True(ObjMonZombieFamilyCore.ThirdCopyOfTheInlineRoll());
        Assert.True(ObjMonZombieFamilyCore.AliasLine2107IsJ212sOwn());
        Assert.True(ObjMonZombieFamilyCore.FourOfThirteenPorted());
        Assert.True(ObjMonZombieFamilyCore.CommentAboveHereBelowThere());
        Assert.True(ObjMonZombieFamilyCore.RollInRange());
        Assert.True(ObjMonZombieFamilyCore.NegativeGivesDc2PlusOne());
        Assert.True(ObjMonZombieFamilyCore.AllTrueSitesInTable());

        Assert.Equal(13, ObjMonZombieFamilyCore.J212AliasLines.Length);
        Assert.Contains(2107, ObjMonZombieFamilyCore.J212AliasLines);
        Assert.Equal(new[] { 7810, 8708, 1995, 2107 }, ObjMonZombieFamilyCore.TrueSites);
        Assert.Equal(4, ObjMonZombieFamilyCore.InlineRollSites.Length);
    }

    [Fact]
    public void InlineRollBoundaries()
    {
        Assert.Equal(10, ObjMonZombieFamilyCore.InlineRoll(10, 20, 0));
        Assert.Equal(20, ObjMonZombieFamilyCore.InlineRoll(10, 20, 10));
        Assert.Equal(6, ObjMonZombieFamilyCore.InlineRoll(10, 5, 0));
    }

    [Fact]
    public void NarrowedThrottleFacts()
    {
        Assert.True(ObjMonZombieFamilyCore.OrRewrittenAsNesting());
        Assert.True(ObjMonZombieFamilyCore.NoResearchWhileTargeted());
        Assert.True(ObjMonZombieFamilyCore.SecondVariantOfTheNarrowedThrottle());
        Assert.True(ObjMonZombieFamilyCore.ContrastWithJ231());
        Assert.True(ObjMonZombieFamilyCore.DifferWithTarget());
        Assert.True(ObjMonZombieFamilyCore.StandardResearches());
        Assert.True(ObjMonZombieFamilyCore.NarrowedDoesNot());
        Assert.True(ObjMonZombieFamilyCore.AgreeWithoutTarget());
        Assert.True(ObjMonZombieFamilyCore.NeitherAtOneSecond());
    }

    [Fact]
    public void ThrottleBoundaries()
    {
        // **有目标且已过 8 秒：标准写法会重搜、改窄后不会**
        Assert.True(ObjMonZombieFamilyCore.StandardThrottle(9000, true));
        Assert.False(ObjMonZombieFamilyCore.NarrowedThrottle(9000, true));

        // **无目标时两者一致**
        Assert.True(ObjMonZombieFamilyCore.StandardThrottle(9000, false));
        Assert.True(ObjMonZombieFamilyCore.NarrowedThrottle(9000, false));

        // **恰好 1 秒两者都不搜**
        Assert.False(ObjMonZombieFamilyCore.StandardThrottle(1000, false));
        Assert.False(ObjMonZombieFamilyCore.NarrowedThrottle(1000, false));
    }

    [Fact]
    public void RetreatAndBoundaryFacts()
    {
        Assert.True(ObjMonZombieFamilyCore.BackPositionOverwritesTarget());
        Assert.True(ObjMonZombieFamilyCore.RetreatBySideEffect());
        Assert.True(ObjMonZombieFamilyCore.TwoTierProximity());
        Assert.True(ObjMonZombieFamilyCore.RandomThreeNonZero());
        Assert.True(ObjMonZombieFamilyCore.CloseAndRollDelegates());
        Assert.True(ObjMonZombieFamilyCore.CloseButZeroRetreats());
        Assert.True(ObjMonZombieFamilyCore.FartherRetreats());
        Assert.True(ObjMonZombieFamilyCore.StrictLessThanSix());
        Assert.True(ObjMonZombieFamilyCore.MixedBoundaryStyles());
        Assert.True(ObjMonZombieFamilyCore.SameThresholdBothComparisons());
        Assert.True(ObjMonZombieFamilyCore.ConjunctionWithDisjunction());
    }

    [Fact]
    public void StrictBoundaryBoundaries()
    {
        Assert.True(ObjMonZombieFamilyCore.FiveCanAttack());
        Assert.True(ObjMonZombieFamilyCore.SixCannotAttack());
        Assert.True(ObjMonZombieFamilyCore.LessEqualWouldAllowSix());

        Assert.True(ObjMonZombieFamilyCore.CanAttack(5, 5));
        Assert.False(ObjMonZombieFamilyCore.CanAttack(6, 0));
        Assert.False(ObjMonZombieFamilyCore.CanAttack(0, 6));
    }

    [Fact]
    public void RelaxBoundaries()
    {
        Assert.True(ObjMonZombieFamilyCore.AllRelax());
        Assert.True(ObjMonZombieFamilyCore.NoMasterWorks());
        Assert.True(ObjMonZombieFamilyCore.PetUncontrolledWorks());
        Assert.True(ObjMonZombieFamilyCore.PetControlledRelaxes());

        Assert.True(ObjMonZombieFamilyCore.ShouldRelax(true, true, false, false));
        Assert.False(ObjMonZombieFamilyCore.ShouldRelax(false, true, false, false));
        Assert.False(ObjMonZombieFamilyCore.ShouldRelax(true, true, true, false));
        Assert.True(ObjMonZombieFamilyCore.ShouldRelax(true, true, true, true));
    }

    // ===================== 二、TDigOutZombi =====================

    [Fact]
    public void RevivesJ233Facts()
    {
        Assert.True(ObjMonZombieFamilyCore.RevivesJ233sDisabledBlock());
        Assert.True(ObjMonZombieFamilyCore.SameFourLines());
        Assert.True(ObjMonZombieFamilyCore.FiveMinuteEvent());
        Assert.True(ObjMonZombieFamilyCore.CrossClassConfirmation());
        Assert.True(ObjMonZombieFamilyCore.DisabledHereAliveThere());

        Assert.Equal(new[] { 8305, 8309 }, ObjMonZombieFamilyCore.J233DisabledBlock);
    }

    [Fact]
    public void RevealScanFacts()
    {
        Assert.True(ObjMonZombieFamilyCore.ThirdCopyOfTheRevealScan());
        Assert.True(ObjMonZombieFamilyCore.ExtraOfflineFilterHere());
        Assert.True(ObjMonZombieFamilyCore.J233HadNone());
        Assert.True(ObjMonZombieFamilyCore.FilterDepthVaries());
        Assert.True(ObjMonZombieFamilyCore.UsesLockTryFinally());
        Assert.True(ObjMonZombieFamilyCore.ThreeReveals());
        Assert.True(ObjMonZombieFamilyCore.FourDoesNot());
    }

    [Fact]
    public void RevealBoundaries()
    {
        Assert.True(ObjMonZombieFamilyCore.ShouldReveal(3, 3));
        Assert.True(ObjMonZombieFamilyCore.ShouldReveal(0, 3));
        Assert.False(ObjMonZombieFamilyCore.ShouldReveal(4, 0));
        Assert.False(ObjMonZombieFamilyCore.ShouldReveal(0, 4));
    }

    [Fact]
    public void Bo554AndRaceFacts()
    {
        Assert.True(ObjMonZombieFamilyCore.Bo554ExplicitlyInitialized());
        Assert.True(ObjMonZombieFamilyCore.OnlyAssignmentInFourBatches());
        Assert.True(ObjMonZombieFamilyCore.ThreeChecksOneAssignment());
        Assert.True(ObjMonZombieFamilyCore.HardcodedRaceServers());
        Assert.True(ObjMonZombieFamilyCore.NinetyFiveNinetySixHundred());
        Assert.True(ObjMonZombieFamilyCore.ThreeDistinctRaceValues());
        Assert.True(ObjMonZombieFamilyCore.MostClassesDoNotSetIt());
    }

    [Fact]
    public void SearchTimeBaseFacts()
    {
        Assert.True(ObjMonZombieFamilyCore.ThirdSearchTimeBase());
        Assert.True(ObjMonZombieFamilyCore.BasesAreFiveHundredFifteenHundredTwentyFiveHundred());
        Assert.True(ObjMonZombieFamilyCore.NotATypo());
        Assert.True(ObjMonZombieFamilyCore.SearchTimeRange());

        Assert.Equal(new[] { 500, 1500, 2500 }, ObjMonZombieFamilyCore.SearchTimeBases);
        Assert.Equal(2500, ObjMonZombieFamilyCore.SearchTime(0));
        Assert.Equal(3999, ObjMonZombieFamilyCore.SearchTime(1499));
    }

    // ===================== 三、TZilKinZombi =====================

    [Fact]
    public void ThresholdMismatchFacts()
    {
        Assert.True(ObjMonZombieFamilyCore.ReadWriteThresholdMismatch());
        Assert.True(ObjMonZombieFamilyCore.GreaterThanZeroInDie());
        Assert.True(ObjMonZombieFamilyCore.GreaterEqualZeroInRun());
        Assert.True(ObjMonZombieFamilyCore.DecOutsideTheIf());
        Assert.True(ObjMonZombieFamilyCore.TwoThirdsGetZero());
    }

    [Fact]
    public void CountBoundaries()
    {
        Assert.True(ObjMonZombieFamilyCore.ZeroWhenMissed());
        Assert.True(ObjMonZombieFamilyCore.OneToThreeWhenHit());

        Assert.Equal(0, ObjMonZombieFamilyCore.InitialCount(1, 0));
        Assert.Equal(1, ObjMonZombieFamilyCore.InitialCount(0, 0));
        Assert.Equal(3, ObjMonZombieFamilyCore.InitialCount(0, 2));
    }

    [Fact]
    public void CountAfterDieBoundaries()
    {
        Assert.True(ObjMonZombieFamilyCore.ZeroGoesNegative());
        Assert.True(ObjMonZombieFamilyCore.AfterDieNegativeBlocks());
        Assert.True(ObjMonZombieFamilyCore.KeepsDecrementing());

        Assert.Equal(-1, ObjMonZombieFamilyCore.CountAfterDie(0));
        Assert.Equal(-2, ObjMonZombieFamilyCore.CountAfterDie(-1));
        Assert.False(ObjMonZombieFamilyCore.RunAllowsRevive(-1));
        Assert.True(ObjMonZombieFamilyCore.RunAllowsRevive(0));
    }

    [Fact]
    public void HalvingFacts()
    {
        Assert.True(ObjMonZombieFamilyCore.HalvedOnEachRevive());
        Assert.True(ObjMonZombieFamilyCore.TwoHalvingStyles());
        Assert.True(ObjMonZombieFamilyCore.BothHpFieldsWritten());
        Assert.True(ObjMonZombieFamilyCore.ReAliveCall());
        Assert.True(ObjMonZombieFamilyCore.ShrEqualsDiv());
        Assert.True(ObjMonZombieFamilyCore.OddTruncated());
        Assert.True(ObjMonZombieFamilyCore.RepeatedHalvingReachesZero());

        Assert.Equal(3, ObjMonZombieFamilyCore.Halve(7));
        Assert.Equal(3, 7 / 2);
        Assert.Equal(50, ObjMonZombieFamilyCore.Halve(100));
    }

    [Fact]
    public void ReviveGateFacts()
    {
        Assert.True(ObjMonZombieFamilyCore.RequiresVisibleAudience());
        Assert.True(ObjMonZombieFamilyCore.VisibilityGatesRevive());
        Assert.True(ObjMonZombieFamilyCore.AllConditionsRevive());
        Assert.True(ObjMonZombieFamilyCore.NoAudienceNoRevive());
        Assert.True(ObjMonZombieFamilyCore.TooEarlyNoRevive());
        Assert.True(ObjMonZombieFamilyCore.GhostNoRevive());
        Assert.True(ObjMonZombieFamilyCore.NegativeCountNoRevive());
        Assert.True(ObjMonZombieFamilyCore.AliveNoRevive());
    }

    [Fact]
    public void ReviveGateBoundaries()
    {
        Assert.True(ObjMonZombieFamilyCore.Revives(true, false, 0, true, 1, 5000, 5000));
        Assert.False(ObjMonZombieFamilyCore.Revives(true, false, 0, true, 0, 5000, 5000));
        Assert.False(ObjMonZombieFamilyCore.Revives(true, false, 0, true, 1, 4999, 5000));
        Assert.False(ObjMonZombieFamilyCore.Revives(true, true, 0, true, 1, 5000, 5000));
        Assert.False(ObjMonZombieFamilyCore.Revives(true, false, -1, true, 1, 5000, 5000));
        Assert.False(ObjMonZombieFamilyCore.Revives(false, false, 0, true, 1, 5000, 5000));
        Assert.False(ObjMonZombieFamilyCore.Revives(true, false, 0, false, 1, 5000, 5000));
    }

    [Fact]
    public void ReviveDelayFacts()
    {
        Assert.True(ObjMonZombieFamilyCore.ReviveDelayFourToTwentyThree());
        Assert.True(ObjMonZombieFamilyCore.NewDelayRange());
        Assert.True(ObjMonZombieFamilyCore.MinReviveDelay());
        Assert.True(ObjMonZombieFamilyCore.MaxReviveDelay());

        Assert.Equal(4000, ObjMonZombieFamilyCore.ReviveDelayMs(0));
        Assert.Equal(23000, ObjMonZombieFamilyCore.ReviveDelayMs(19));
    }

    [Fact]
    public void DieStructureFacts()
    {
        Assert.True(ObjMonZombieFamilyCore.InheritedFirstInDie());
        Assert.True(ObjMonZombieFamilyCore.CommentPairExplainsAssignment());
        Assert.True(ObjMonZombieFamilyCore.CommentsAdjacent());
        Assert.True(ObjMonZombieFamilyCore.NoProtectionHere());

        Assert.Equal(new[] { 2274, 2275 }, ObjMonZombieFamilyCore.DieCommentLines);
    }

    // ===================== 四、其余 =====================

    [Fact]
    public void ViewRangeFacts()
    {
        Assert.True(ObjMonZombieFamilyCore.ViewRangeLineCommentedOut());
        Assert.True(ObjMonZombieFamilyCore.OnlyThisOneDoesNotSetIt());
        Assert.True(ObjMonZombieFamilyCore.ViewRangesExtracted());

        Assert.Equal(new[] { 7, 6, 6 }, ObjMonZombieFamilyCore.ViewRanges);
    }

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonZombieFamilyCore.ConsistentWithJ244());
        Assert.True(ObjMonZombieFamilyCore.ThreePureShellDestroys());
        Assert.True(ObjMonZombieFamilyCore.ThirtyTwoTotal());
        Assert.True(ObjMonZombieFamilyCore.ZombieFamilyPartlyClosed());
        Assert.True(ObjMonZombieFamilyCore.TwoClassesRemain());
        Assert.True(ObjMonZombieFamilyCore.DeclLinesChecked());
        Assert.True(ObjMonZombieFamilyCore.TwoOfThreeShareBase());

        Assert.Equal(new[] { 380, 389, 398 }, ObjMonZombieFamilyCore.DeclLines);
    }
}
