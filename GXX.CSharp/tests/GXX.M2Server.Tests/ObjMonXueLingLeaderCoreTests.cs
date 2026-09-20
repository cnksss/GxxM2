using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J237：`ObjMon.pas` 中 `TXueLingLeader`（血灵教主）五个方法的 1:1 测试（349 行）。
/// **本批最有价值的发现**：
/// ① "圆心从自己改成目标"这次改动**应用不全** —— 同一个 `GetMapBaseObjects` 出现七次：
///    三次旧的被注释、三次新的以目标为心、**还有一次旧的仍是活代码**；
/// ② `MagicAttack2` 在 `for I := …` 循环里**从不使用 `I`**、每次 `Random(nCount)` 抽下标，
///    且两个上限不一致（`>` 使实际能冻 `nMax + 1` 个）；
/// ③ 四个 `MagicAttackN` 是**四条不同的管线**（含本系列第一个**治疗**槽位）；
/// ④ **最严重**：那个治疗的公式 `HP / 100 * 110` 先做整数除法，
///    于是**血量 1..99 的友方会被直接置为 0 血（打死）**；
/// ⑤ 末尾 `(Abs > 8)` 又恒真（与 J233 同形）。
/// </summary>
public sealed class ObjMonXueLingLeaderCoreTests
{
    [Fact]
    public void Constants()
    {
        Assert.Equal(9006, ObjMonXueLingLeaderCore.CreateStart);
        Assert.Equal(9012, ObjMonXueLingLeaderCore.CreateEnd);
        Assert.Equal(7, ObjMonXueLingLeaderCore.CreateLines);
        Assert.Equal(9014, ObjMonXueLingLeaderCore.DestroyStart);
        Assert.Equal(9017, ObjMonXueLingLeaderCore.DestroyEnd);
        Assert.Equal(4, ObjMonXueLingLeaderCore.DestroyLines);
        Assert.Equal(9019, ObjMonXueLingLeaderCore.AttackStart);
        Assert.Equal(9338, ObjMonXueLingLeaderCore.AttackEnd);
        Assert.Equal(320, ObjMonXueLingLeaderCore.AttackLines);
        Assert.Equal(9340, ObjMonXueLingLeaderCore.RunStart);
        Assert.Equal(9354, ObjMonXueLingLeaderCore.RunEnd);
        Assert.Equal(15, ObjMonXueLingLeaderCore.RunLines);
        Assert.Equal(9356, ObjMonXueLingLeaderCore.WonderingStart);
        Assert.Equal(9358, ObjMonXueLingLeaderCore.WonderingEnd);
        Assert.Equal(3, ObjMonXueLingLeaderCore.WonderingLines);
        Assert.Equal(349, ObjMonXueLingLeaderCore.TotalLines);
        Assert.Equal(5, ObjMonXueLingLeaderCore.MethodCount);

        Assert.Equal(9021, ObjMonXueLingLeaderCore.Ma1Start);
        Assert.Equal(9092, ObjMonXueLingLeaderCore.Ma1End);
        Assert.Equal(72, ObjMonXueLingLeaderCore.Ma1Lines);
        Assert.Equal(9095, ObjMonXueLingLeaderCore.Ma2Start);
        Assert.Equal(9181, ObjMonXueLingLeaderCore.Ma2End);
        Assert.Equal(87, ObjMonXueLingLeaderCore.Ma2Lines);
        Assert.Equal(9183, ObjMonXueLingLeaderCore.Ma4Start);
        Assert.Equal(9212, ObjMonXueLingLeaderCore.Ma4End);
        Assert.Equal(30, ObjMonXueLingLeaderCore.Ma4Lines);
        Assert.Equal(9214, ObjMonXueLingLeaderCore.Ma5Start);
        Assert.Equal(9240, ObjMonXueLingLeaderCore.Ma5End);
        Assert.Equal(27, ObjMonXueLingLeaderCore.Ma5Lines);
        Assert.Equal(4, ObjMonXueLingLeaderCore.NestedCount);
        Assert.Equal(9242, ObjMonXueLingLeaderCore.OuterStart);
        Assert.Equal(9338, ObjMonXueLingLeaderCore.OuterEnd);
        Assert.Equal(97, ObjMonXueLingLeaderCore.OuterLines);

        Assert.Equal(-1, ObjMonXueLingLeaderCore.OldOffsetX);
        Assert.Equal(-2, ObjMonXueLingLeaderCore.OldOffsetY);
        Assert.Equal(3, ObjMonXueLingLeaderCore.CommentedOldCalls);
        Assert.Equal(3, ObjMonXueLingLeaderCore.LiveTargetCalls);
        Assert.Equal(1, ObjMonXueLingLeaderCore.LiveOldCalls);
        Assert.Equal(7, ObjMonXueLingLeaderCore.GetMapCalls);
        Assert.Equal(9223, ObjMonXueLingLeaderCore.LiveOldLine);
        Assert.Equal(10, ObjMonXueLingLeaderCore.Radius);

        Assert.Equal(9318, ObjMonXueLingLeaderCore.EffectCommentLine);
        Assert.Equal(500, ObjMonXueLingLeaderCore.LightingExDelay);

        Assert.Equal(9110, ObjMonXueLingLeaderCore.ReverseLoopLine);
        Assert.Equal(9119, ObjMonXueLingLeaderCore.DeleteLine);
        Assert.Equal(9123, ObjMonXueLingLeaderCore.CurResetLine);
        Assert.Equal(9124, ObjMonXueLingLeaderCore.CountLine);
        Assert.Equal(9125, ObjMonXueLingLeaderCore.MaxLine);
        Assert.Equal(4, ObjMonXueLingLeaderCore.MaxCap);
        Assert.Equal(9126, ObjMonXueLingLeaderCore.RandomizeLine);
        Assert.Equal(9127, ObjMonXueLingLeaderCore.MagicPowerLine);
        Assert.Equal(9130, ObjMonXueLingLeaderCore.ForwardLoopLine);
        Assert.Equal(9132, ObjMonXueLingLeaderCore.RandomIndexLine);
        Assert.Equal(9136, ObjMonXueLingLeaderCore.RawRateAddLine);
        Assert.Equal(9169, ObjMonXueLingLeaderCore.FrozenResistLine);
        Assert.Equal(9171, ObjMonXueLingLeaderCore.OpenFrozenLine);
        Assert.Equal(2, ObjMonXueLingLeaderCore.FrozenDuration);
        Assert.Equal(9172, ObjMonXueLingLeaderCore.IncCurLine);
        Assert.Equal(9174, ObjMonXueLingLeaderCore.BreakLine);

        Assert.Equal(9048, ObjMonXueLingLeaderCore.Ma1Abil3Line);
        Assert.Equal(9049, ObjMonXueLingLeaderCore.Ma1ElementGuardLine);
        Assert.Equal(9051, ObjMonXueLingLeaderCore.Ma1ElementLine);
        Assert.Equal(9053, ObjMonXueLingLeaderCore.Ma1RateAddLine);
        Assert.Equal(9054, ObjMonXueLingLeaderCore.Ma1NextDamageLine);
        Assert.Equal(9056, ObjMonXueLingLeaderCore.Ma1PowerMaxLine);
        Assert.Equal(9196, ObjMonXueLingLeaderCore.PlusHalfLine);
        Assert.Equal(2, ObjMonXueLingLeaderCore.HalfDivisor);
        Assert.Equal(9205, ObjMonXueLingLeaderCore.Ma4StruckLine);
        Assert.Equal(9229, ObjMonXueLingLeaderCore.HealFormulaLine);
        Assert.Equal(110, ObjMonXueLingLeaderCore.HealNumerator);
        Assert.Equal(100, ObjMonXueLingLeaderCore.HealDenominator);
        Assert.Equal(9233, ObjMonXueLingLeaderCore.HealthSpellChangedLine);

        Assert.Equal(9243, ObjMonXueLingLeaderCore.EffectTypeDeclLine);
        Assert.Equal(9245, ObjMonXueLingLeaderCore.ResultFalseLine);
        Assert.Equal(9246, ObjMonXueLingLeaderCore.NilGuardLine);
        Assert.Equal(9248, ObjMonXueLingLeaderCore.CooldownLine);
        Assert.Equal(9250, ObjMonXueLingLeaderCore.DelayClearLine);
        Assert.Equal(3, ObjMonXueLingLeaderCore.SummonEffectId);
        Assert.Equal(9295, ObjMonXueLingLeaderCore.RangeGateLine);
        Assert.Equal(8, ObjMonXueLingLeaderCore.RangeThreshold);
        Assert.Equal(9297, ObjMonXueLingLeaderCore.HitTickLine);
        Assert.Equal(100, ObjMonXueLingLeaderCore.RollBound);
        Assert.Equal(30, ObjMonXueLingLeaderCore.RollThreshold);
        Assert.Equal(3, ObjMonXueLingLeaderCore.SkippedNumber);
        Assert.Equal(9323, ObjMonXueLingLeaderCore.GateExitLine);
        Assert.Equal(9328, ObjMonXueLingLeaderCore.TailCheckLine);
        Assert.Equal(8294, ObjMonXueLingLeaderCore.J233TautologicalLine);

        Assert.Equal(9342, ObjMonXueLingLeaderCore.RunGuardLine);
        Assert.Equal(9344, ObjMonXueLingLeaderCore.ThrottleLine);
        Assert.Equal(8000, ObjMonXueLingLeaderCore.SearchWithTargetMs);
        Assert.Equal(1000, ObjMonXueLingLeaderCore.SearchWithoutTargetMs);
        Assert.Equal(9353, ObjMonXueLingLeaderCore.RunInheritedLine);
        Assert.Equal(9357, ObjMonXueLingLeaderCore.WonderingBeginLine);
        Assert.Equal(9358, ObjMonXueLingLeaderCore.WonderingEndLine);

        Assert.Equal(268, ObjMonXueLingLeaderCore.ClassDeclLine);
        Assert.Equal(267, ObjMonXueLingLeaderCore.ClassCommentLine);
        Assert.Equal(270, ObjMonXueLingLeaderCore.CustodianFieldLine);
        Assert.Equal(3, ObjMonXueLingLeaderCore.CustodianArrayHigh);
        Assert.Equal(271, ObjMonXueLingLeaderCore.FrozenTickFieldLine);
        Assert.Equal(9011, ObjMonXueLingLeaderCore.FillCharLine);
        Assert.Equal(5, ObjMonXueLingLeaderCore.LightValue);
        Assert.Equal(277, ObjMonXueLingLeaderCore.WonderingDeclLine);
        Assert.Equal(275, ObjMonXueLingLeaderCore.AttackDeclLine);
        Assert.Equal(9360, ObjMonXueLingLeaderCore.NextSectionLine);
        Assert.Equal(9361, ObjMonXueLingLeaderCore.NextCreateLine);
        Assert.Equal(9371, ObjMonXueLingLeaderCore.NextCommentedLine);
        Assert.Equal(9112, ObjMonXueLingLeaderCore.ItemsIndexLine);
        Assert.Equal(41, ObjMonXueLingLeaderCore.ClassesCovered);
        Assert.Equal(13, ObjMonXueLingLeaderCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonXueLingLeaderCore.SpanMatches());
        Assert.True(ObjMonXueLingLeaderCore.TotalLinesAddUp());
        Assert.True(ObjMonXueLingLeaderCore.AttackDecompositionAddsUp());
        Assert.True(ObjMonXueLingLeaderCore.MethodsAscending());
        Assert.True(ObjMonXueLingLeaderCore.MethodsContiguous());
        Assert.True(ObjMonXueLingLeaderCore.NestedAscending());
        Assert.True(ObjMonXueLingLeaderCore.WithinUnit());
        Assert.True(ObjMonXueLingLeaderCore.NoInstrumentation());
    }

    // ===================== 一、圆心改动没改干净 =====================

    [Fact]
    public void CenterChangeFacts()
    {
        Assert.True(ObjMonXueLingLeaderCore.CenterChangeIncomplete());
        Assert.True(ObjMonXueLingLeaderCore.ThreeCommentedThreeLiveTarget());
        Assert.True(ObjMonXueLingLeaderCore.OneStillSelfCentered());
        Assert.True(ObjMonXueLingLeaderCore.MissedTheFourth());
        Assert.True(ObjMonXueLingLeaderCore.OldFormSurvivesBothWays());
        Assert.True(ObjMonXueLingLeaderCore.GetMapCallsIsSeven());
        Assert.True(ObjMonXueLingLeaderCore.CommentedOldLinesChecked());
        Assert.True(ObjMonXueLingLeaderCore.LiveTargetLinesChecked());
        Assert.True(ObjMonXueLingLeaderCore.CommentPrecedesNewCall());
        Assert.True(ObjMonXueLingLeaderCore.ThreeNewOneOld());
    }

    [Fact]
    public void CenterLines()
    {
        Assert.Equal(new[] { 9034, 9107, 9191 },
            ObjMonXueLingLeaderCore.CommentedOldLines);
        Assert.Equal(new[] { 9035, 9108, 9192 },
            ObjMonXueLingLeaderCore.LiveTargetLines);
    }

    [Fact]
    public void OffsetFacts()
    {
        Assert.True(ObjMonXueLingLeaderCore.HardcodedOffset());
        Assert.True(ObjMonXueLingLeaderCore.FourOccurrencesOfOffset());
        Assert.True(ObjMonXueLingLeaderCore.RadiusAlwaysTen());
        Assert.True(ObjMonXueLingLeaderCore.TwoCenterKindsDiffer());

        Assert.Equal("self-offset", ObjMonXueLingLeaderCore.CenterKind(true));
        Assert.Equal("target", ObjMonXueLingLeaderCore.CenterKind(false));
    }

    [Fact]
    public void EffectCenterFacts()
    {
        Assert.True(ObjMonXueLingLeaderCore.EffectCenterAlsoChanged());
        Assert.True(ObjMonXueLingLeaderCore.SelfToTargetInIntArg());
        Assert.True(ObjMonXueLingLeaderCore.SameChangeTwoFaces());
        Assert.True(ObjMonXueLingLeaderCore.DifferentResidueDegrees());
        Assert.True(ObjMonXueLingLeaderCore.BothOldEffectsCommented());
        Assert.True(ObjMonXueLingLeaderCore.BothNewEffectsLive());
        Assert.True(ObjMonXueLingLeaderCore.LightingExHasDelay());

        Assert.Equal(new[] { 9319, 9320 }, ObjMonXueLingLeaderCore.OldEffectLines);
        Assert.Equal(new[] { 9321, 9322 }, ObjMonXueLingLeaderCore.NewEffectLines);
    }

    // ===================== 二、循环变量 I 没用 =====================

    [Fact]
    public void LoopVarFacts()
    {
        Assert.True(ObjMonXueLingLeaderCore.LoopVarUnused());
        Assert.True(ObjMonXueLingLeaderCore.RandomIndexInsideLoop());
        Assert.True(ObjMonXueLingLeaderCore.SameObjectMayRepeat());
        Assert.True(ObjMonXueLingLeaderCore.SomeObjectsNeverHit());
    }

    [Fact]
    public void RandomIndexBoundaries()
    {
        Assert.Equal(1, ObjMonXueLingLeaderCore.PickedIndex(4, 1));
        Assert.Equal(1, ObjMonXueLingLeaderCore.PickedIndex(4, 5));
        Assert.True(ObjMonXueLingLeaderCore.PicksCanRepeat());
        Assert.True(ObjMonXueLingLeaderCore.PicksNeedNotCover());
    }

    [Fact]
    public void TwoCapsFacts()
    {
        Assert.True(ObjMonXueLingLeaderCore.TwoCapsDisagree());
        Assert.True(ObjMonXueLingLeaderCore.LoopUsesCountNotMax());
        Assert.True(ObjMonXueLingLeaderCore.BreakUsesGreaterNotGreaterEqual());
        Assert.True(ObjMonXueLingLeaderCore.CanFreezeNMaxPlusOne());
        Assert.True(ObjMonXueLingLeaderCore.CapIsFour());
        Assert.True(ObjMonXueLingLeaderCore.CapTakesSmaller());
        Assert.True(ObjMonXueLingLeaderCore.NMaxDoesNotBreak());
        Assert.True(ObjMonXueLingLeaderCore.NMaxPlusOneBreaks());

        Assert.Equal(4, ObjMonXueLingLeaderCore.MaxOf(100));
        Assert.Equal(2, ObjMonXueLingLeaderCore.MaxOf(2));
        Assert.False(ObjMonXueLingLeaderCore.BreaksAt(3, 3));
        Assert.True(ObjMonXueLingLeaderCore.BreaksAt(4, 3));
    }

    [Fact]
    public void PipelineAndPowerFacts()
    {
        Assert.True(ObjMonXueLingLeaderCore.NoPipelinePrefix());
        Assert.True(ObjMonXueLingLeaderCore.RateAddOnRawPower());
        Assert.True(ObjMonXueLingLeaderCore.UsesMagicPowerHere());
        Assert.True(ObjMonXueLingLeaderCore.SameNameTwoAbilities());
    }

    [Fact]
    public void RandomizeFacts()
    {
        Assert.True(ObjMonXueLingLeaderCore.RandomizeInsideAttack());
        Assert.True(ObjMonXueLingLeaderCore.FiveSitesFileWide());
        Assert.True(ObjMonXueLingLeaderCore.FourthCluster());
        Assert.True(ObjMonXueLingLeaderCore.RandomizeLinesChecked());

        Assert.Equal(new[] { 1621, 6480, 6788, 7207, 9126 },
            ObjMonXueLingLeaderCore.RandomizeLines);
    }

    [Fact]
    public void FrozenResistFacts()
    {
        Assert.True(ObjMonXueLingLeaderCore.ImmunityNotAbsolute());
        Assert.True(ObjMonXueLingLeaderCore.GreaterEqualMeansResist());
        Assert.True(ObjMonXueLingLeaderCore.DirectionMatchesJ223Corrected());
        Assert.True(ObjMonXueLingLeaderCore.DurationHardcodedTwo());
        Assert.True(ObjMonXueLingLeaderCore.NotImmuneAlwaysFreezes());
        Assert.True(ObjMonXueLingLeaderCore.ImmuneAndLowRollResists());
        Assert.True(ObjMonXueLingLeaderCore.ImmuneButHighRollStillFreezes());
        Assert.True(ObjMonXueLingLeaderCore.ZeroRateDefeatsImmunity());

        Assert.True(ObjMonXueLingLeaderCore.Freezes(false, 100, 0));
        Assert.False(ObjMonXueLingLeaderCore.Freezes(true, 100, 0));
        Assert.True(ObjMonXueLingLeaderCore.Freezes(true, 100, 100));
        Assert.True(ObjMonXueLingLeaderCore.Freezes(true, 0, 0));
    }

    [Fact]
    public void OppositeLoopFacts()
    {
        Assert.True(ObjMonXueLingLeaderCore.ReverseLoopWithDelete());
        Assert.True(ObjMonXueLingLeaderCore.BothLoopDirections());
        Assert.True(ObjMonXueLingLeaderCore.ExcludesSelf());
        Assert.True(ObjMonXueLingLeaderCore.OnlyPlayersAndHeroes());
    }

    // ===================== 三、四条管线 =====================

    [Fact]
    public void PipelineVariantFacts()
    {
        Assert.True(ObjMonXueLingLeaderCore.FourDifferentPipelines());
        Assert.True(ObjMonXueLingLeaderCore.FifthPipelineVariant());
        Assert.True(ObjMonXueLingLeaderCore.RateAddAfterElementAdd());
        Assert.True(ObjMonXueLingLeaderCore.ElementAddGuardedAlone());
        Assert.True(ObjMonXueLingLeaderCore.GuardWrapsOneStatement());
        Assert.True(ObjMonXueLingLeaderCore.MagicAttack4HasNoPipeline());
        Assert.True(ObjMonXueLingLeaderCore.MagicAttack5IsAHeal());
    }

    [Fact]
    public void PlusHalfFacts()
    {
        Assert.True(ObjMonXueLingLeaderCore.PlusHalfByIntegerDivision());
        Assert.True(ObjMonXueLingLeaderCore.TruncatedHalf());
        Assert.True(ObjMonXueLingLeaderCore.SameFamilyAsShape6());
        Assert.True(ObjMonXueLingLeaderCore.HundredBecomes150());
        Assert.True(ObjMonXueLingLeaderCore.OddIsTruncated());

        Assert.Equal(150, ObjMonXueLingLeaderCore.PlusHalf(100));
        Assert.Equal(4, ObjMonXueLingLeaderCore.PlusHalf(3));
    }

    // ---------- 治疗：最严重的发现 ----------

    [Fact]
    public void HealSeverityFacts()
    {
        Assert.True(ObjMonXueLingLeaderCore.HealToHundredTenPercent());
        Assert.True(ObjMonXueLingLeaderCore.CappedAtMaxHp());
        Assert.True(ObjMonXueLingLeaderCore.IntegerDivisionAgain());
        Assert.True(ObjMonXueLingLeaderCore.FirstHealInTheSeries());
        Assert.True(ObjMonXueLingLeaderCore.UsesIsProperFriend());
        Assert.True(ObjMonXueLingLeaderCore.CallsHealthSpellChanged());
    }

    [Fact]
    public void HealCollapsesLowHpToZero()
    {
        // **探针实测：1..99 血全部归零**
        Assert.True(ObjMonXueLingLeaderCore.LowHpCollapsesToZero());
        Assert.True(ObjMonXueLingLeaderCore.NinetyNineCollapsesToZero());
        Assert.True(ObjMonXueLingLeaderCore.LethalForAlliesBelowHundred());
        Assert.True(ObjMonXueLingLeaderCore.MostSevereShape6SoFar());

        Assert.Equal(0, ObjMonXueLingLeaderCore.HealedHp(50, 1000));
        Assert.Equal(0, ObjMonXueLingLeaderCore.HealedHp(99, 1000));
        Assert.Equal(110, ObjMonXueLingLeaderCore.HealedHp(100, 1000));
        Assert.Equal(550, ObjMonXueLingLeaderCore.HealedHp(500, 1000));
    }

    [Fact]
    public void HealNormalCases()
    {
        Assert.True(ObjMonXueLingLeaderCore.HealsToTenPercentMore());
        Assert.True(ObjMonXueLingLeaderCore.HundredIsTheThreshold());
        Assert.True(ObjMonXueLingLeaderCore.FullHpUnchanged());
        Assert.True(ObjMonXueLingLeaderCore.CappedWhenHigh());

        Assert.Equal(550, ObjMonXueLingLeaderCore.HealedHp(500, 1000));
        Assert.Equal(100, ObjMonXueLingLeaderCore.HealedHp(1000, 100));
        Assert.Equal(100, ObjMonXueLingLeaderCore.HealedHp(100, 100));
    }

    [Fact]
    public void EffectNumberingFacts()
    {
        Assert.True(ObjMonXueLingLeaderCore.NoMagicAttack3());
        Assert.True(ObjMonXueLingLeaderCore.NumberSkipped());
        Assert.True(ObjMonXueLingLeaderCore.EffectThreeNeverUsed());
        Assert.True(ObjMonXueLingLeaderCore.DanglingNumberVariant());
        Assert.True(ObjMonXueLingLeaderCore.FourEffectTypes());
        Assert.True(ObjMonXueLingLeaderCore.AssignLinesChecked());

        Assert.Equal(new[] { 4, 5, 2, 1 }, ObjMonXueLingLeaderCore.EffectTypeValues);
        Assert.Equal(new[] { 9300, 9305, 9310, 9315 },
            ObjMonXueLingLeaderCore.EffectTypeAssignLines);
    }

    // ===================== 四、召唤链与冷却 =====================

    [Fact]
    public void SummonChainFacts()
    {
        Assert.True(ObjMonXueLingLeaderCore.FourHpThresholds());
        Assert.True(ObjMonXueLingLeaderCore.ThresholdsAre80604020());
        Assert.True(ObjMonXueLingLeaderCore.AllFourSlotsUsed());
        Assert.True(ObjMonXueLingLeaderCore.SummonThenExit());
        Assert.True(ObjMonXueLingLeaderCore.OffsetReappearsInSummon());
        Assert.True(ObjMonXueLingLeaderCore.TwoEffectsPerSummon());
        Assert.True(ObjMonXueLingLeaderCore.SummonEffectIsThree());
        Assert.True(ObjMonXueLingLeaderCore.SummonThreeIsSeparate());
        Assert.True(ObjMonXueLingLeaderCore.AllSummonsExit());

        Assert.Equal(new[] { 80, 60, 40, 20 }, ObjMonXueLingLeaderCore.HpThresholds);
        Assert.Equal(new[] { 9251, 9262, 9273, 9284 },
            ObjMonXueLingLeaderCore.ThresholdLines);
        Assert.Equal(new[] { 9256, 9267, 9278, 9289 },
            ObjMonXueLingLeaderCore.SummonLines);
        Assert.Equal(new[] { 0, 1, 2, 3 }, ObjMonXueLingLeaderCore.CustodianIndices);
        Assert.Equal(new[] { 9259, 9270, 9281, 9292 },
            ObjMonXueLingLeaderCore.SummonExitLines);
    }

    [Fact]
    public void CooldownFacts()
    {
        Assert.True(ObjMonXueLingLeaderCore.DelayClearedBeforeSummon());
        Assert.True(ObjMonXueLingLeaderCore.TickSetAfterSummon());
        Assert.True(ObjMonXueLingLeaderCore.ExitSkipsTickUpdate());
        Assert.True(ObjMonXueLingLeaderCore.SummonDoesNotConsumeCooldown());
        Assert.True(ObjMonXueLingLeaderCore.NoDelayFiresSooner());
        Assert.True(ObjMonXueLingLeaderCore.LargerDelayNeedsMoreTime());
    }

    [Fact]
    public void CooldownBoundaries()
    {
        Assert.True(ObjMonXueLingLeaderCore.CooldownElapsed(0, 300, 200, 0));
        Assert.False(ObjMonXueLingLeaderCore.CooldownElapsed(0, 300, 200, 200));
        Assert.True(ObjMonXueLingLeaderCore.CooldownElapsed(0, 500, 200, 200));
        Assert.False(ObjMonXueLingLeaderCore.CooldownElapsed(0, 200, 200, 0));
    }

    [Fact]
    public void ProbabilityFacts()
    {
        Assert.True(ObjMonXueLingLeaderCore.ThreeSequentialRolls());
        Assert.True(ObjMonXueLingLeaderCore.FourEffectiveProbabilities());
        Assert.True(ObjMonXueLingLeaderCore.SumIsOne());

        Assert.Equal(new[] { 9298, 9303, 9308 }, ObjMonXueLingLeaderCore.RollLines);
        Assert.True(Math.Abs(ObjMonXueLingLeaderCore.P4() - 0.30) < 1e-9);
        Assert.True(Math.Abs(ObjMonXueLingLeaderCore.P5() - 0.21) < 1e-9);
        Assert.True(Math.Abs(ObjMonXueLingLeaderCore.P2() - 0.147) < 1e-9);
        Assert.True(Math.Abs(ObjMonXueLingLeaderCore.P1() - 0.343) < 1e-9);
    }

    [Fact]
    public void TautologyFacts()
    {
        Assert.True(ObjMonXueLingLeaderCore.GateEight());
        Assert.True(ObjMonXueLingLeaderCore.TailIsNegatedGate());
        Assert.True(ObjMonXueLingLeaderCore.SecondOccurrenceOfJ233Shape());
        Assert.True(ObjMonXueLingLeaderCore.SimplerThanJ233());
        Assert.True(ObjMonXueLingLeaderCore.ComplementsAlways());
        Assert.True(ObjMonXueLingLeaderCore.OutOfRangeAlwaysTrueWhenReached());
        Assert.True(ObjMonXueLingLeaderCore.NoProbabilityGateInside());
    }

    [Fact]
    public void RangeBoundaries()
    {
        Assert.True(ObjMonXueLingLeaderCore.InRange(8, 8));
        Assert.False(ObjMonXueLingLeaderCore.InRange(9, 0));
        Assert.True(ObjMonXueLingLeaderCore.OutOfRange(9, 0));
        Assert.False(ObjMonXueLingLeaderCore.OutOfRange(8, 8));
    }

    // ---------- Run / Wondering / Create ----------

    [Fact]
    public void RunFacts()
    {
        Assert.True(ObjMonXueLingLeaderCore.FiveFoldGuard());
        Assert.True(ObjMonXueLingLeaderCore.AllOkRuns());
        Assert.True(ObjMonXueLingLeaderCore.AnyBlocks());
        Assert.True(ObjMonXueLingLeaderCore.TwoTierThrottle());
        Assert.True(ObjMonXueLingLeaderCore.SearchAfterEight());
        Assert.True(ObjMonXueLingLeaderCore.SearchAfterOne());
        Assert.True(ObjMonXueLingLeaderCore.InheritedUnconditional());
    }

    [Fact]
    public void GuardBoundaries()
    {
        Assert.True(ObjMonXueLingLeaderCore.CanRun(false, false, false, false, true));
        Assert.False(ObjMonXueLingLeaderCore.CanRun(true, false, false, false, true));
        Assert.False(ObjMonXueLingLeaderCore.CanRun(false, true, false, false, true));
        Assert.False(ObjMonXueLingLeaderCore.CanRun(false, false, true, false, true));
        Assert.False(ObjMonXueLingLeaderCore.CanRun(false, false, false, true, true));
        Assert.False(ObjMonXueLingLeaderCore.CanRun(false, false, false, false, false));
    }

    [Fact]
    public void ThrottleBoundaries()
    {
        Assert.True(ObjMonXueLingLeaderCore.ShouldSearch(8001, true));
        Assert.False(ObjMonXueLingLeaderCore.ShouldSearch(8000, true));
        Assert.True(ObjMonXueLingLeaderCore.ShouldSearch(1001, false));
        Assert.False(ObjMonXueLingLeaderCore.ShouldSearch(1000, false));
    }

    [Fact]
    public void WonderingAndCreateFacts()
    {
        Assert.True(ObjMonXueLingLeaderCore.EmptyWonderingOverride());
        Assert.True(ObjMonXueLingLeaderCore.NeverWanders());
        Assert.True(ObjMonXueLingLeaderCore.ShapeFourDeliberateSuppression());
        Assert.True(ObjMonXueLingLeaderCore.WonderingIsThreeLines());
        Assert.True(ObjMonXueLingLeaderCore.PureShellDestroy());
        Assert.True(ObjMonXueLingLeaderCore.FillCharToZero());
        Assert.True(ObjMonXueLingLeaderCore.FirstFillCharUse());
        Assert.True(ObjMonXueLingLeaderCore.ThreeThingsInCreate());
        Assert.True(ObjMonXueLingLeaderCore.LightIsFive());
        Assert.True(ObjMonXueLingLeaderCore.CreateBodyLinesChecked());

        Assert.Equal(new[] { 9009, 9010, 9011 }, ObjMonXueLingLeaderCore.CreateBodyLines);
    }

    [Fact]
    public void IndexingStyleFacts()
    {
        Assert.True(ObjMonXueLingLeaderCore.TwoIndexingStyles());
        Assert.True(ObjMonXueLingLeaderCore.ThreeBracketOneItems());
        Assert.True(ObjMonXueLingLeaderCore.ItemsIsInTheReverseLoop());

        Assert.Equal(new[] { 9038, 9199, 9226 },
            ObjMonXueLingLeaderCore.BracketIndexLines);
    }

    // ===================== 五、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonXueLingLeaderCore.BaseIsTAnimalObject());
        Assert.True(ObjMonXueLingLeaderCore.NoOverrideOnAttackTarget());
        Assert.True(ObjMonXueLingLeaderCore.FirstWonderingOverride());
        Assert.True(ObjMonXueLingLeaderCore.ClassCommentChecked());
        Assert.True(ObjMonXueLingLeaderCore.FieldsChecked());
        Assert.True(ObjMonXueLingLeaderCore.ArrayHasFourSlots());
        Assert.True(ObjMonXueLingLeaderCore.NextClassIsWealthAnimalMon());
        Assert.True(ObjMonXueLingLeaderCore.OneOfSixImmunitiesCommentedOut());
        Assert.True(ObjMonXueLingLeaderCore.NextSectionChecked());
        Assert.True(ObjMonXueLingLeaderCore.FortyOneClassesCovered());
        Assert.True(ObjMonXueLingLeaderCore.RemainingApprox());

        Assert.Equal(new[] { 270, 271 }, ObjMonXueLingLeaderCore.FieldLines);
    }
}
