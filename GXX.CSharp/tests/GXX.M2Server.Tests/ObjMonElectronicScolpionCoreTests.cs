using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J248：`ObjMon.pas` 中 `TElectronicScolpionMon`（电子蝎子）五方法的 1:1 测试（180 行）。
/// **本批最有价值的发现**：
/// ① 本方法 **95 行、是本系列最长的方法**，末尾挂着**四路外观级联**，
///    一次性引入**五个外观值**（619/638/628/622/614）—— 与整个系列此前记录的总数持平；
/// ② **`619` 在级联里出现两次** ⇒ **特效圆心取决于一次掷骰**
///    （掷中 `Random(2) = 0` 放绿毒、掷不中落到无条件的那一支 ⇒ 圆心**自己**；
///    若无那一支则会落到最后 `else` ⇒ 圆心目标）—— 本系列**第一次**见重复分支条件；
/// ③ `Run` 的花括号注释里是**同一个条件的重复**（`{ or (m_wAppr = 628) }`）
///    ⇒ 取消注释是**重言式**、不改变任何行为 —— 第**八**种花括号用法；
/// ④ 外观特判**第一次影响到几何形状**（614/638 要"**恰好**在 3 格环上"；
///    其余要"2 格内 且（低于半血 或 恰好在 2 环上）"）。
/// </summary>
public sealed class ObjMonElectronicScolpionCoreTests
{
    [Fact]
    public void Constants()
    {
        Assert.Equal(3067, ObjMonElectronicScolpionCore.CreateStart);
        Assert.Equal(3074, ObjMonElectronicScolpionCore.CreateEnd);
        Assert.Equal(8, ObjMonElectronicScolpionCore.CreateLines);
        Assert.Equal(3075, ObjMonElectronicScolpionCore.DestroyStart);
        Assert.Equal(3079, ObjMonElectronicScolpionCore.DestroyEnd);
        Assert.Equal(5, ObjMonElectronicScolpionCore.DestroyLines);
        Assert.Equal(3080, ObjMonElectronicScolpionCore.AttackStart);
        Assert.Equal(3174, ObjMonElectronicScolpionCore.AttackEnd);
        Assert.Equal(95, ObjMonElectronicScolpionCore.AttackLines);
        Assert.Equal(3176, ObjMonElectronicScolpionCore.RefreshStart);
        Assert.Equal(3180, ObjMonElectronicScolpionCore.RefreshEnd);
        Assert.Equal(5, ObjMonElectronicScolpionCore.RefreshLines);
        Assert.Equal(3182, ObjMonElectronicScolpionCore.RunStart);
        Assert.Equal(3248, ObjMonElectronicScolpionCore.RunEnd);
        Assert.Equal(67, ObjMonElectronicScolpionCore.RunLines);
        Assert.Equal(180, ObjMonElectronicScolpionCore.TotalLines);
        Assert.Equal(5, ObjMonElectronicScolpionCore.MethodCount);
        Assert.Equal(1, ObjMonElectronicScolpionCore.ClassCount);

        Assert.Equal(3149, ObjMonElectronicScolpionCore.CascadeLine);
        Assert.Equal(3149, ObjMonElectronicScolpionCore.Branch1Line);
        Assert.Equal(3155, ObjMonElectronicScolpionCore.Branch2Line);
        Assert.Equal(3161, ObjMonElectronicScolpionCore.Branch3Line);
        Assert.Equal(3167, ObjMonElectronicScolpionCore.Branch4Line);
        Assert.Equal(3171, ObjMonElectronicScolpionCore.ElseLine);

        Assert.Equal(30, ObjMonElectronicScolpionCore.PoisonBound);
        Assert.Equal(30, ObjMonElectronicScolpionCore.PoisonBase);
        Assert.Equal(3, ObjMonElectronicScolpionCore.OtherPoisonMin);
        Assert.Equal(8, ObjMonElectronicScolpionCore.OtherPoisonMax);
        Assert.Equal(2, ObjMonElectronicScolpionCore.FrozenBase);
        Assert.Equal(3, ObjMonElectronicScolpionCore.FrozenBound);
        Assert.Equal(10, ObjMonElectronicScolpionCore.FrozenGate);

        Assert.Equal(3091, ObjMonElectronicScolpionCore.AliasLine);
        Assert.Equal(3092, ObjMonElectronicScolpionCore.PowerLine);
        Assert.Equal(3100, ObjMonElectronicScolpionCore.RateAddLine);
        Assert.Equal(3102, ObjMonElectronicScolpionCore.NextDamageLine);
        Assert.Equal(3104, ObjMonElectronicScolpionCore.PowerMaxLine);
        Assert.Equal(3131, ObjMonElectronicScolpionCore.GetBackLine);
        Assert.Equal(3137, ObjMonElectronicScolpionCore.ParalysisLine);

        Assert.Equal(3187, ObjMonElectronicScolpionCore.DuplicateCondLine);
        Assert.Equal(628, ObjMonElectronicScolpionCore.DuplicateValue);
        Assert.Equal(7, ObjMonElectronicScolpionCore.PriorBraceUsages);
        Assert.Equal(3204, ObjMonElectronicScolpionCore.ThrottleLine);
        Assert.Equal(1000, ObjMonElectronicScolpionCore.ThresholdMs);
        Assert.Equal(3212, ObjMonElectronicScolpionCore.InheritedExitLine);

        Assert.Equal(3, ObjMonElectronicScolpionCore.OuterRadius);
        Assert.Equal(2, ObjMonElectronicScolpionCore.InnerRadius);
        Assert.Equal(3247, ObjMonElectronicScolpionCore.InheritedNamedLine);
        Assert.Equal(7, ObjMonElectronicScolpionCore.DigUpBlockLines);

        Assert.Equal(368, ObjMonElectronicScolpionCore.ClassDeclLine);
        Assert.Equal(369, ObjMonElectronicScolpionCore.FirstFlagFieldLine);
        Assert.Equal(371, ObjMonElectronicScolpionCore.UseMagicFieldLine);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonElectronicScolpionCore.SpanMatches());
        Assert.True(ObjMonElectronicScolpionCore.TotalLinesAddUp());
        Assert.True(ObjMonElectronicScolpionCore.MethodsAscending());
        Assert.True(ObjMonElectronicScolpionCore.MethodsContiguous());
        Assert.True(ObjMonElectronicScolpionCore.WithinUnit());
        Assert.True(ObjMonElectronicScolpionCore.NoInstrumentation());
    }

    // ===================== 一、四路级联 =====================

    [Fact]
    public void CascadeShapeFacts()
    {
        Assert.True(ObjMonElectronicScolpionCore.LongestMethodSoFar());
        Assert.True(ObjMonElectronicScolpionCore.FourWayCascade());
        Assert.True(ObjMonElectronicScolpionCore.FiveAppearanceValuesHere());
        Assert.True(ObjMonElectronicScolpionCore.MatchesWholeSeriesSoFar());
        Assert.True(ObjMonElectronicScolpionCore.FiveDistinct());

        Assert.Equal(new[] { 619, 638, 628, 622, 614 },
            ObjMonElectronicScolpionCore.AppearanceValues);
        Assert.Equal(5, ObjMonElectronicScolpionCore.PriorAppearanceValues.Length);
        Assert.Equal(231, ObjMonElectronicScolpionCore.PriorAppearanceValues[0].Value);
        Assert.Equal(218, ObjMonElectronicScolpionCore.PriorAppearanceValues[4].Value);
    }

    [Fact]
    public void RepeatedBranchFacts()
    {
        Assert.True(ObjMonElectronicScolpionCore.Value619AppearsTwice());
        Assert.True(ObjMonElectronicScolpionCore.EffectCentreDependsOnARoll());
        Assert.True(ObjMonElectronicScolpionCore.SelfCentredWhenRollFails());
        Assert.True(ObjMonElectronicScolpionCore.TargetCentredIfBranchAbsent());
        Assert.True(ObjMonElectronicScolpionCore.FirstRepeatedBranchCondition());
    }

    [Fact]
    public void CascadeBoundaries()
    {
        // **619：掷中放毒、掷不中则特效以自己为心**
        Assert.True(ObjMonElectronicScolpionCore.SixNineteenHitsFirst());
        Assert.True(ObjMonElectronicScolpionCore.SixNineteenMissesToSelf());
        Assert.Equal("green-poison-1",
            ObjMonElectronicScolpionCore.Cascade(619, 0, 1, 1, 1));
        Assert.Equal("effect-self",
            ObjMonElectronicScolpionCore.Cascade(619, 1, 1, 1, 1));

        // **638：掷中放毒、掷不中落到最后（圆心目标）** —— 与 619 不同
        Assert.True(ObjMonElectronicScolpionCore.SixThirtyEightHits());
        Assert.True(ObjMonElectronicScolpionCore.SixThirtyEightFallsToTarget());
        Assert.Equal("effect-target",
            ObjMonElectronicScolpionCore.Cascade(638, 1, 1, 1, 1));

        // **628 / 622 / 未知**
        Assert.True(ObjMonElectronicScolpionCore.SixTwentyEightSecond());
        Assert.True(ObjMonElectronicScolpionCore.SixTwentyTwoFreezes());
        Assert.True(ObjMonElectronicScolpionCore.UnknownFallsToTarget());
    }

    [Fact]
    public void ThreePoisonPathsFacts()
    {
        Assert.True(ObjMonElectronicScolpionCore.ThreePoisonPaths());
        Assert.True(ObjMonElectronicScolpionCore.IdenticalPoisonBody());
        Assert.True(ObjMonElectronicScolpionCore.DifferentGatesForSameEffect());
        Assert.True(ObjMonElectronicScolpionCore.SameCommentThreeTimes());

        Assert.Equal(new[] { 3151, 3157 }, ObjMonElectronicScolpionCore.PoisonCommentLines);
    }

    [Fact]
    public void PoisonDurationFacts()
    {
        Assert.True(ObjMonElectronicScolpionCore.PoisonDurationThirtyToFiftyNine());
        Assert.True(ObjMonElectronicScolpionCore.TwoMagnitudesOfGreenPoison());
        Assert.True(ObjMonElectronicScolpionCore.NotATypo());
        Assert.True(ObjMonElectronicScolpionCore.PoisonRange());

        Assert.Equal(30, ObjMonElectronicScolpionCore.PoisonDuration(0));
        Assert.Equal(59, ObjMonElectronicScolpionCore.PoisonDuration(29));
    }

    [Fact]
    public void FrozenFacts()
    {
        Assert.True(ObjMonElectronicScolpionCore.FrozenTwoToFour());
        Assert.True(ObjMonElectronicScolpionCore.OneInTenGate());
        Assert.True(ObjMonElectronicScolpionCore.FrozenRange());
        Assert.True(ObjMonElectronicScolpionCore.ReadsDicePropertyOnce());
        Assert.True(ObjMonElectronicScolpionCore.UnFrozenIsNewHere());

        Assert.Equal(2, ObjMonElectronicScolpionCore.FrozenDuration(0));
        Assert.Equal(4, ObjMonElectronicScolpionCore.FrozenDuration(2));
    }

    // ===================== 伤害管线 =====================

    [Fact]
    public void PipelineFacts()
    {
        Assert.True(ObjMonElectronicScolpionCore.UsesMagicPower());
        Assert.True(ObjMonElectronicScolpionCore.AliasLine3091IsInTheList());
        Assert.True(ObjMonElectronicScolpionCore.McVersionNotInTheList());
        Assert.True(ObjMonElectronicScolpionCore.TwoAbilityPairsInOneIdiom());
        Assert.True(ObjMonElectronicScolpionCore.FiveStepPipeline());
        Assert.True(ObjMonElectronicScolpionCore.MpLowByteAsDivisor());
        Assert.True(ObjMonElectronicScolpionCore.SameIdiomElsewhere());
        Assert.True(ObjMonElectronicScolpionCore.CompleteParalysisForm());

        Assert.Contains(3091, ObjMonElectronicScolpionCore.J212AliasLines);
    }

    [Fact]
    public void RecoveryBoundaries()
    {
        Assert.True(ObjMonElectronicScolpionCore.ZeroMpNoRecovery());
        Assert.True(ObjMonElectronicScolpionCore.NonZeroMpRecovers());
        Assert.True(ObjMonElectronicScolpionCore.Truncates());

        Assert.Equal(100, ObjMonElectronicScolpionCore.RecoveredHp(100, 100, 0));
        Assert.Equal(110, ObjMonElectronicScolpionCore.RecoveredHp(100, 100, 10));
        Assert.Equal(101, ObjMonElectronicScolpionCore.RecoveredHp(100, 15, 10));
    }

    // ===================== 二、Run =====================

    [Fact]
    public void DuplicateConditionFacts()
    {
        Assert.True(ObjMonElectronicScolpionCore.DuplicateConditionInBrace());
        Assert.True(ObjMonElectronicScolpionCore.UncommentingChangesNothing());
        Assert.True(ObjMonElectronicScolpionCore.EighthBraceUsage());
        Assert.True(ObjMonElectronicScolpionCore.IsEighth());
        Assert.True(ObjMonElectronicScolpionCore.LikelyIntendedAnotherValue());
        Assert.True(ObjMonElectronicScolpionCore.EquivalentToSingle());
    }

    [Fact]
    public void TautologyBoundaries()
    {
        Assert.True(ObjMonElectronicScolpionCore.Tautology(628));
        Assert.False(ObjMonElectronicScolpionCore.Tautology(627));
        Assert.False(ObjMonElectronicScolpionCore.Tautology(629));

        // **穷举 620..640 证明它与单个条件完全等价**
        for (int a = 620; a <= 640; a++)
        {
            Assert.Equal(a == 628, ObjMonElectronicScolpionCore.Tautology(a));
        }
    }

    [Fact]
    public void MagicModeFacts()
    {
        Assert.True(ObjMonElectronicScolpionCore.HalfHpThreshold());
        Assert.True(ObjMonElectronicScolpionCore.RecomputedEveryTick());
        Assert.True(ObjMonElectronicScolpionCore.BooleanAsDerivedState());
        Assert.True(ObjMonElectronicScolpionCore.OneLessIsTrue());
        Assert.True(ObjMonElectronicScolpionCore.ExactlyHalfIsFalse());
        Assert.True(ObjMonElectronicScolpionCore.FullIsFalse());

        Assert.True(ObjMonElectronicScolpionCore.UseMagic(49, 100));
        Assert.False(ObjMonElectronicScolpionCore.UseMagic(50, 100));
        Assert.False(ObjMonElectronicScolpionCore.UseMagic(100, 100));
    }

    [Fact]
    public void ThrottleFacts()
    {
        Assert.True(ObjMonElectronicScolpionCore.SingleThresholdPlusNoTarget());
        Assert.True(ObjMonElectronicScolpionCore.ThirdNarrowedThrottle());
        Assert.True(ObjMonElectronicScolpionCore.SameAsJ231());
        Assert.True(ObjMonElectronicScolpionCore.NoResearchWithTarget());
        Assert.True(ObjMonElectronicScolpionCore.SearchAfterOne());
        Assert.True(ObjMonElectronicScolpionCore.ExactlyOneBlocks());
    }

    [Fact]
    public void ThrottleBoundaries()
    {
        Assert.True(ObjMonElectronicScolpionCore.ShouldSearch(1001, false));
        Assert.False(ObjMonElectronicScolpionCore.ShouldSearch(1000, false));
        Assert.False(ObjMonElectronicScolpionCore.ShouldSearch(99999, true));
    }

    [Fact]
    public void InheritedExitFacts()
    {
        Assert.True(ObjMonElectronicScolpionCore.InheritedBeforeExit());
        Assert.True(ObjMonElectronicScolpionCore.DatedExplanationComment());
        Assert.True(ObjMonElectronicScolpionCore.ExplainsTheCounterIntuitiveOrder());
        Assert.True(ObjMonElectronicScolpionCore.NoInstrumentationHere());

        Assert.Equal(new[] { 3209, 3212 }, ObjMonElectronicScolpionCore.ExplainCommentLines);
    }

    // ---------- 两套几何 ----------

    [Fact]
    public void TwoGeometryFacts()
    {
        Assert.True(ObjMonElectronicScolpionCore.CachesAxesInLocals());
        Assert.True(ObjMonElectronicScolpionCore.OthersRecomputeInline());
        Assert.True(ObjMonElectronicScolpionCore.TwoRangeShapes());
        Assert.True(ObjMonElectronicScolpionCore.RingOfThreeOnly());
        Assert.True(ObjMonElectronicScolpionCore.TwoGridInsideOrMagicMode());
        Assert.True(ObjMonElectronicScolpionCore.FirstGeometryDifference());

        Assert.Equal(new[] { 3215, 3216 }, ObjMonElectronicScolpionCore.AxisLines);
        Assert.Equal(new[] { 614, 638 }, ObjMonElectronicScolpionCore.RingShapedApprs);
    }

    [Fact]
    public void RingShapeBoundaries()
    {
        Assert.True(ObjMonElectronicScolpionCore.ThreeThreeAttacks());
        Assert.True(ObjMonElectronicScolpionCore.ThreeTwoAttacks());
        Assert.True(ObjMonElectronicScolpionCore.TwoTwoCannot());
        Assert.True(ObjMonElectronicScolpionCore.FourCannot());

        Assert.True(ObjMonElectronicScolpionCore.RingShape(3, 3));
        Assert.True(ObjMonElectronicScolpionCore.RingShape(3, 2));
        Assert.False(ObjMonElectronicScolpionCore.RingShape(2, 2));
        Assert.False(ObjMonElectronicScolpionCore.RingShape(4, 0));
    }

    [Fact]
    public void GridShapeBoundaries()
    {
        Assert.True(ObjMonElectronicScolpionCore.MagicModeOneOne());
        Assert.True(ObjMonElectronicScolpionCore.NormalOneOneCannot());
        Assert.True(ObjMonElectronicScolpionCore.NormalTwoTwo());
        Assert.True(ObjMonElectronicScolpionCore.GridThreeCannot());
        Assert.True(ObjMonElectronicScolpionCore.ShapesDisagreeAtTwoTwo());

        Assert.True(ObjMonElectronicScolpionCore.GridShape(1, 1, true));
        Assert.False(ObjMonElectronicScolpionCore.GridShape(1, 1, false));
        Assert.True(ObjMonElectronicScolpionCore.GridShape(2, 2, false));
        Assert.False(ObjMonElectronicScolpionCore.GridShape(3, 0, true));
    }

    [Fact]
    public void InheritedNameStyleFacts()
    {
        Assert.True(ObjMonElectronicScolpionCore.InheritedWithExplicitName());
        Assert.True(ObjMonElectronicScolpionCore.FirstSuchStyle());
        Assert.True(ObjMonElectronicScolpionCore.SemanticallyEquivalent());
    }

    // ===================== 三、其余 =====================

    [Fact]
    public void RefreshAndDisguiseFacts()
    {
        Assert.True(ObjMonElectronicScolpionCore.RefreshApprOnlyFor628());
        Assert.True(ObjMonElectronicScolpionCore.SameValueThreePlaces());
        Assert.True(ObjMonElectronicScolpionCore.ReHidesItself());
        Assert.True(ObjMonElectronicScolpionCore.ThreePlacesRecorded());
        Assert.True(ObjMonElectronicScolpionCore.ThreeFieldsInCreate());
        Assert.True(ObjMonElectronicScolpionCore.BaseIsFifteenHundred());
        Assert.True(ObjMonElectronicScolpionCore.FixedHideSetLaterNotInCreate());
        Assert.True(ObjMonElectronicScolpionCore.ThirdDisguiseArrangement());
        Assert.True(ObjMonElectronicScolpionCore.NoWalkDelayAfterDigUp());
        Assert.True(ObjMonElectronicScolpionCore.ThreeEndingsOfTheSameBlock());
        Assert.True(ObjMonElectronicScolpionCore.FourThingsNoDelay());

        Assert.Equal(3, ObjMonElectronicScolpionCore.PlacesOf628.Length);
    }

    [Fact]
    public void DestroyFacts()
    {
        Assert.True(ObjMonElectronicScolpionCore.PureShellDestroy());
        Assert.True(ObjMonElectronicScolpionCore.ThirtyFiveTotal());
        Assert.True(ObjMonElectronicScolpionCore.ConsistentWithJ244ToJ247());
    }

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonElectronicScolpionCore.ElectronicScolpionClosed());
        Assert.True(ObjMonElectronicScolpionCore.ZombieFamilyComplete());
        Assert.True(ObjMonElectronicScolpionCore.OneRowToFlip());
        Assert.True(ObjMonElectronicScolpionCore.ZombieFamilyDeclLinesChecked());
        Assert.True(ObjMonElectronicScolpionCore.DeclLinesChecked());

        Assert.Equal(new[] { 368, 380, 389, 398, 409 },
            ObjMonElectronicScolpionCore.ZombieFamilyDeclLines);
    }
}
