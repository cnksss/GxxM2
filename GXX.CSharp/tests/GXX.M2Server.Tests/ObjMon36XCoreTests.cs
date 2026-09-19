using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J204：`ObjMon.pas` 中 `TMon36_XMonster` 十个方法 1:1 测试
/// （合计 564 行）。
/// **本批最严重的发现**：`RefreshAppr` 把 `m_boFixedHideMode` 置真（3452）、
/// **同一轮的 `Run` 又在 3467 无条件清掉** ——
/// 而原本的条件清（3463）恰好被注释掉了，
/// 于是 `True` 从未能存活到被读取。
/// </summary>
public sealed class ObjMon36XCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(3438, ObjMon36XCore.CreateStart);
        Assert.Equal(5, ObjMon36XCore.CreateLines);
        Assert.Equal(3444, ObjMon36XCore.DestroyStart);
        Assert.Equal(4, ObjMon36XCore.DestroyLines);
        Assert.Equal(3449, ObjMon36XCore.RefreshApprStart);
        Assert.Equal(5, ObjMon36XCore.RefreshApprLines);
        Assert.Equal(3455, ObjMon36XCore.RunStart);
        Assert.Equal(15, ObjMon36XCore.RunLines);
        Assert.Equal(3471, ObjMon36XCore.AttackTarget0Start);
        Assert.Equal(99, ObjMon36XCore.AttackTarget0Lines);
        Assert.Equal(3571, ObjMon36XCore.AttackTarget365Start);
        Assert.Equal(148, ObjMon36XCore.AttackTarget365Lines);
        Assert.Equal(3721, ObjMon36XCore.MagPushStart);
        Assert.Equal(48, ObjMon36XCore.MagPushLines);
        Assert.Equal(3771, ObjMon36XCore.MagicAttackStart);
        Assert.Equal(69, ObjMon36XCore.MagicAttackLines);
        Assert.Equal(3842, ObjMon36XCore.MagicAttackGroupStart);
        Assert.Equal(89, ObjMon36XCore.MagicAttackGroupLines);
        Assert.Equal(3989, ObjMon36XCore.AttackTargetStart);
        Assert.Equal(82, ObjMon36XCore.AttackTargetLines);
        Assert.Equal(564, ObjMon36XCore.TotalLines);
        Assert.Equal(3452, ObjMon36XCore.RefreshSetLine);
        Assert.Equal(3467, ObjMon36XCore.RunClearLine);
        Assert.Equal(3463, ObjMon36XCore.CommentedClearLine);
        Assert.Equal(5, ObjMon36XCore.FirstDirection);
        Assert.Equal(9, ObjMon36XCore.ClassesCovered);
        Assert.Equal(45, ObjMon36XCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMon36XCore.SpanMatches());
        Assert.True(ObjMon36XCore.TotalLinesAddUp());
        Assert.True(ObjMon36XCore.StartsAscending());
        Assert.True(ObjMon36XCore.FirstFourContiguous());
        Assert.True(ObjMon36XCore.WithinUnit());
        Assert.True(ObjMon36XCore.NoInstrumentation());
        Assert.True(ObjMon36XCore.CommentBlockBeforeAttackTarget());
        Assert.True(ObjMon36XCore.CommentBlockAfterGroup());
        Assert.True(ObjMon36XCore.CommentBlockBetween());
    }

    // ===================== 一、FixedHideMode 抵消 =====================

    [Fact]
    public void FixedHideModeCancels()
    {
        Assert.True(ObjMon36XCore.RefreshSetsThenRunClears());
        Assert.True(ObjMon36XCore.UnconditionalClearAt3467());
        Assert.True(ObjMon36XCore.TrueNeverSurvives());
        Assert.True(ObjMon36XCore.IneffectiveWriteAgain());
        Assert.True(ObjMon36XCore.OriginalWasConditional());
        Assert.True(ObjMon36XCore.NowUnconditional());
        Assert.True(ObjMon36XCore.CommentedOutAt3463());
        Assert.True(ObjMon36XCore.SetBeforeClear());
        Assert.True(ObjMon36XCore.CommentedLineBetween());
        Assert.True(ObjMon36XCore.FixedHideSitesExtracted());
        Assert.True(ObjMon36XCore.SixTwentyEightCommentedHere());
        Assert.True(ObjMon36XCore.ActiveElsewhere());
        Assert.True(ObjMon36XCore.SixSitesInFile());
        Assert.True(ObjMon36XCore.RunDoesNotCallRefreshAppr());
        Assert.True(ObjMon36XCore.TimingDependent());
        Assert.True(ObjMon36XCore.SameFamilyAsJ200());
        Assert.True(ObjMon36XCore.FalseAfterRun());
        Assert.True(ObjMon36XCore.TrueBeforeRun());

        // **修正后的断言：真正由源码唯一确定的事实**
        Assert.True(ObjMon36XCore.RefreshOnlySetsFor601());
        Assert.True(ObjMon36XCore.RefreshGuardIsAppr601());
    }

    [Fact]
    public void FixedHideSitesTable()
    {
        Assert.Equal(4, ObjMon36XCore.FixedHideSites.Length);
        Assert.Equal(3452, ObjMon36XCore.FixedHideSites[0].Line);
        Assert.Equal("set-true", ObjMon36XCore.FixedHideSites[0].Kind);
        Assert.Equal(3463, ObjMon36XCore.FixedHideSites[1].Line);
        Assert.Equal("commented-clear", ObjMon36XCore.FixedHideSites[1].Kind);
        Assert.Equal(3467, ObjMon36XCore.FixedHideSites[2].Line);
        Assert.Equal("unconditional-clear", ObjMon36XCore.FixedHideSites[2].Kind);
        Assert.Equal(3451, ObjMon36XCore.FixedHideSites[3].Line);
    }

    [Fact]
    public void FixedHideBoundaries()
    {
        // **调过 `Run` 之后必为假**
        Assert.False(ObjMon36XCore.SimulateFixedHide(true, true));

        // **未调 `Run` 之前可为真**
        Assert.True(ObjMon36XCore.SimulateFixedHide(true, false));

        Assert.True(ObjMon36XCore.RefreshSetLine < ObjMon36XCore.RunClearLine);
        Assert.True(ObjMon36XCore.CommentedClearLine > ObjMon36XCore.RefreshSetLine);
        Assert.True(ObjMon36XCore.CommentedClearLine < ObjMon36XCore.RunClearLine);
    }

    // ===================== 二、两条链不一致 =====================

    [Fact]
    public void ChainsDisagreeFacts()
    {
        Assert.True(ObjMon36XCore.DuplicatedSixZeroNine());
        Assert.True(ObjMon36XCore.SixZeroSevenMissingFromStrength());
        Assert.True(ObjMon36XCore.SixOneSixMissingFromEffect());
        Assert.True(ObjMon36XCore.ChainsDisagree());
        Assert.True(ObjMon36XCore.ScriptCountsMatch());
        Assert.True(ObjMon36XCore.DuplicateIsTypo());
        Assert.True(ObjMon36XCore.Occupies607Slot());
        Assert.True(ObjMon36XCore.OriginalIntentLikely607());
        Assert.True(ObjMon36XCore.DeadFourthComparison());
        Assert.True(ObjMon36XCore.ShortCircuitMakesItInert());
        Assert.True(ObjMon36XCore.SameFamilyAsJ198());
        Assert.True(ObjMon36XCore.KeptVerbatim());
        Assert.True(ObjMon36XCore.StrengthChainHasFive());
        Assert.True(ObjMon36XCore.EffectChainHasFour());
        Assert.True(ObjMon36XCore.DuplicateKept());
    }

    [Fact]
    public void ChainBoundaries()
    {
        // **`607` 不在力度链、但在特效链 —— 不一致**
        Assert.False(ObjMon36XCore.InStrengthChain(607));
        Assert.True(ObjMon36XCore.InEffectChain(607));
        Assert.True(ObjMon36XCore.SixZeroSevenNotInStrength());
        Assert.True(ObjMon36XCore.SixZeroSevenInEffect());
        Assert.True(ObjMon36XCore.SixZeroSevenInconsistent());

        // **`616` 在力度链、但不在特效链 —— 不一致**
        Assert.True(ObjMon36XCore.InStrengthChain(616));
        Assert.False(ObjMon36XCore.InEffectChain(616));
        Assert.True(ObjMon36XCore.SixOneSixInStrength());
        Assert.True(ObjMon36XCore.SixOneSixNotInEffect());
        Assert.True(ObjMon36XCore.SixOneSixInconsistent());

        // **`600`/`609`/`620` 两条链一致**
        Assert.True(ObjMon36XCore.CommonValuesAgree());

        // **链内容逐字**
        Assert.Equal(new[] { 600, 609, 616, 620, 609 }, ObjMon36XCore.StrengthChain);
        Assert.Equal(new[] { 600, 607, 609, 620 }, ObjMon36XCore.EffectChain);
    }

    // ===================== 三、AttackTarget36_5 的缺陷 =====================

    [Fact]
    public void PrecedenceBugFacts()
    {
        Assert.True(ObjMon36XCore.PrecedenceBugAt3642());
        Assert.True(ObjMon36XCore.EquivalentToAandB_orC());
        Assert.True(ObjMon36XCore.NilCanReachIsProperTarget());
        Assert.True(ObjMon36XCore.SiblingHasCorrectParens());
        Assert.True(ObjMon36XCore.BuggyEqualsAndOrForm());
        Assert.True(ObjMon36XCore.VersionsDifferWhenANilBTrue());
        Assert.True(ObjMon36XCore.NotUniversallyEquivalent());
        Assert.True(ObjMon36XCore.BuggyFalseThere());
        Assert.True(ObjMon36XCore.CorrectTrueThere());
        Assert.True(ObjMon36XCore.NilSkipsWhenImproper());
        Assert.True(ObjMon36XCore.CorrectAlsoSkipsHere());
        Assert.True(ObjMon36XCore.BothFalseForNilProper());
    }

    [Fact]
    public void PrecedenceBugIsSemanticNotJustEvaluationOrder()
    {
        // **修正后的核心结论：括号差别**确实改变语义***
        // a=false, b=true, c=false 时：
        //   错误版 (A and B) or C = false
        //   正确版 B or C         = true
        Assert.False(ObjMon36XCore.BuggyFilter(false, true, false));
        Assert.True(ObjMon36XCore.CorrectFilter(false, true, false));

        // **即"缺少括号"不是只影响求值时机、而是会给出相反结论**
        Assert.NotEqual(
            ObjMon36XCore.BuggyFilter(false, true, false),
            ObjMon36XCore.CorrectFilter(false, true, false));

        // **错误版与其代数等价式一致**
        Assert.False(ObjMon36XCore.BuggyFilter(false, true, false));
        Assert.False(ObjMon36XCore.BuggyEqualsAndOrForm() == false);
    }

    [Fact]
    public void DeadCodeAndResourceFacts()
    {
        Assert.True(ObjMon36XCore.NestedFunctionUncalled());
        Assert.True(ObjMon36XCore.DeadCodePlusLeak());
        Assert.True(ObjMon36XCore.NoTryFinallyHere());
        Assert.True(ObjMon36XCore.SiblingsUseTryFinally());
        Assert.True(ObjMon36XCore.ThreeStylesCoexist());
        Assert.True(ObjMon36XCore.CountsAfterDeleting());
        Assert.True(ObjMon36XCore.SideEffectingCount());
        Assert.True(ObjMon36XCore.RemoveAllEquivalent());
        Assert.True(ObjMon36XCore.ThreeSixFourSpecialCase());
        Assert.True(ObjMon36XCore.RadiusOneVsThree());
        Assert.True(ObjMon36XCore.TargetHitTwiceFor364());
        Assert.True(ObjMon36XCore.PipelineDuplicatedVerbatim());
        Assert.True(ObjMon36XCore.SeventyLinesCopied());
        Assert.True(ObjMon36XCore.MostReusedSubsequence());
        Assert.True(ObjMon36XCore.ReboundReusesDamage());
        Assert.True(ObjMon36XCore.ConsistentAcrossThree());
        Assert.True(ObjMon36XCore.DiffersFromJ203Variable());
        Assert.True(ObjMon36XCore.AffirmativeForm());
        Assert.True(ObjMon36XCore.SameAsJ199());
        Assert.True(ObjMon36XCore.OppositeToJ203());
        Assert.True(ObjMon36XCore.ThreeStructuresCoexist());
    }

    // ===================== 四、MagicAttack / MagicAttackGroup =====================

    [Fact]
    public void MagicAttackFacts()
    {
        Assert.True(ObjMon36XCore.NoNilGuardInMagicAttack());
        Assert.True(ObjMon36XCore.ThreeOfFourHaveGuard());
        Assert.True(ObjMon36XCore.OnlyCalledAfterGuard());
        Assert.True(ObjMon36XCore.TwoCallSites());
        Assert.True(ObjMon36XCore.ByteHereIntegerInJ203());
        Assert.True(ObjMon36XCore.SameLogicDifferentType());
        Assert.True(ObjMon36XCore.NoFilteringInMagicAttack());
        Assert.True(ObjMon36XCore.DirectStruckDamage());
        Assert.True(ObjMon36XCore.ContrastWithOthers());
        Assert.True(ObjMon36XCore.ClampedToOneOnlyInMagicAttack());
        Assert.True(ObjMon36XCore.OthersRaw());
        Assert.True(ObjMon36XCore.InconsistentDegeneration());
        Assert.True(ObjMon36XCore.SameWhenNormal());
        Assert.True(ObjMon36XCore.ClampDiffersWhenEqual());
        Assert.True(ObjMon36XCore.ClampDiffersWhenInverted());
    }

    [Fact]
    public void WidthDegenerationBoundaries()
    {
        // **正常：两者一致**
        Assert.Equal(10, ObjMon36XCore.MagicAttackWidth(10, 20));
        Assert.Equal(10, ObjMon36XCore.RawWidth(10, 20));

        // **相等：钳到 1 对裸 0**
        Assert.Equal(1, ObjMon36XCore.MagicAttackWidth(10, 10));
        Assert.Equal(0, ObjMon36XCore.RawWidth(10, 10));

        // **倒挂：钳到 1 对裸负值**
        Assert.Equal(1, ObjMon36XCore.MagicAttackWidth(20, 10));
        Assert.Equal(-10, ObjMon36XCore.RawWidth(20, 10));
    }

    [Fact]
    public void GreenPoisonThreeDurations()
    {
        Assert.True(ObjMon36XCore.ThreeDifferentPoisonDurations());
        Assert.True(ObjMon36XCore.MagicAttackUses3());
        Assert.True(ObjMon36XCore.NotRandomized());
        Assert.True(ObjMon36XCore.OtherTwoAreRanges());
        Assert.True(ObjMon36XCore.PoisonSitesExtracted());

        Assert.Equal(3, ObjMon36XCore.GreenPoisonMagicAttack);
        Assert.Equal(30, ObjMon36XCore.GreenPoison0Min);
        Assert.Equal(59, ObjMon36XCore.GreenPoison0Max);
        Assert.Equal(60, ObjMon36XCore.GreenPoison365Min);
        Assert.Equal(89, ObjMon36XCore.GreenPoison365Max);

        Assert.Equal(3, ObjMon36XCore.GreenPoisonSites.Length);
        Assert.Equal("MagicAttack", ObjMon36XCore.GreenPoisonSites[0].Site);
    }

    [Fact]
    public void GroupAttackFacts()
    {
        Assert.True(ObjMon36XCore.NoReturnValue());
        Assert.True(ObjMon36XCore.MissingExitAt4039());
        Assert.True(ObjMon36XCore.DoubleAttackPossible());
        Assert.True(ObjMon36XCore.ContrastWith4018());
        Assert.True(ObjMon36XCore.TwoCallSitesOnly());
        Assert.True(ObjMon36XCore.OneUsesDefaults());
        Assert.True(ObjMon36XCore.OneExplicit());
        Assert.True(ObjMon36XCore.SelfRageCentersOnSelf());
        Assert.True(ObjMon36XCore.FalseCentersOnTarget());
        Assert.True(ObjMon36XCore.UnguardedTargetDeref());
        Assert.True(ObjMon36XCore.ClosesWithAppr620Dispatch());
        Assert.True(ObjMon36XCore.SimplerThanAttackTarget0());
        Assert.True(ObjMon36XCore.SelfRageUsesSelf());
        Assert.True(ObjMon36XCore.FalseUsesTarget());

        Assert.Equal("self", ObjMon36XCore.GroupCenter(true));
        Assert.Equal("target", ObjMon36XCore.GroupCenter(false));
    }

    // ===================== 五、MagPushArround =====================

    [Fact]
    public void MagPushFacts()
    {
        Assert.True(ObjMon36XCore.PushesAroundSelf());
        Assert.True(ObjMon36XCore.CallSitePassesSelf());
        Assert.True(ObjMon36XCore.MisleadingParamName());
        Assert.True(ObjMon36XCore.LockUnlockPaired());
        Assert.True(ObjMon36XCore.SecondCorrectPattern());
        Assert.True(ObjMon36XCore.OnlyNestedFunctionUnprotected());
        Assert.True(ObjMon36XCore.LevelGreaterOrEqual());
        Assert.True(ObjMon36XCore.WroteAsTwoComparisons());
        Assert.True(ObjMon36XCore.SameLevelFlagCommented());
        Assert.True(ObjMon36XCore.SixLineBraceBlock());
        Assert.True(ObjMon36XCore.ProbabilityReplacedByCertainty());
        Assert.True(ObjMon36XCore.PushIsOnePlusRandom5());
        Assert.True(ObjMon36XCore.Range1To5());
        Assert.True(ObjMon36XCore.CommentedTermStillInPlace());
        Assert.True(ObjMon36XCore.DoubleCondition());
        Assert.True(ObjMon36XCore.IsProperTargetFromPusher());
        Assert.True(ObjMon36XCore.SquareRadiusOne());
        Assert.True(ObjMon36XCore.NineCells());
        Assert.True(ObjMon36XCore.SelfExcluded());
        Assert.True(ObjMon36XCore.HigherLevelPushes());
        Assert.True(ObjMon36XCore.SameLevelPushes());
        Assert.True(ObjMon36XCore.LowerLevelBlocked());
        Assert.True(ObjMon36XCore.EquivalentToGreaterOrEqual());
        Assert.True(ObjMon36XCore.MinPushDistance());
        Assert.True(ObjMon36XCore.MaxPushDistance());
        Assert.True(ObjMon36XCore.AllPushDistancesInRange());
        Assert.True(ObjMon36XCore.AdjacentInside());
        Assert.True(ObjMon36XCore.SameCellInside());
        Assert.True(ObjMon36XCore.TwoOutside());
        Assert.True(ObjMon36XCore.DiagonalTwoOutside());
    }

    [Fact]
    public void PushDistanceBoundaries()
    {
        Assert.Equal(1, ObjMon36XCore.PushDistance(0));
        Assert.Equal(5, ObjMon36XCore.PushDistance(4));

        // **恰好同级的边界**
        Assert.True(ObjMon36XCore.LevelAllowsPush(5, 5));
        Assert.False(ObjMon36XCore.LevelAllowsPush(4, 5));
        Assert.True(ObjMon36XCore.LevelAllowsPush(6, 5));

        // **方形半径：恰好一格在内、两格在外**
        Assert.True(ObjMon36XCore.WithinPushSquare(1, 1));
        Assert.False(ObjMon36XCore.WithinPushSquare(2, 0));
    }

    // ===================== 六、AttackTarget 分派 =====================

    [Fact]
    public void DispatchFacts()
    {
        Assert.True(ObjMon36XCore.EightStageChain());
        Assert.True(ObjMon36XCore.OrderedShortCircuit());
        Assert.True(ObjMon36XCore.FirstMatchWins());
        Assert.True(ObjMon36XCore.ThreeSixSixDoesNotExit());
        Assert.True(ObjMon36XCore.InconsistentExit());
        Assert.True(ObjMon36XCore.MayAttackTwice());
        Assert.True(ObjMon36XCore.DistanceDependent());
        Assert.True(ObjMon36XCore.Only364PropagatesResult());
        Assert.True(ObjMon36XCore.OthersReturnFalse());
        Assert.True(ObjMon36XCore.InconsistentReturnSemantics());
        Assert.True(ObjMon36XCore.RingDistance2To6());
        Assert.True(ObjMon36XCore.TwoIdenticalSites());
        Assert.True(ObjMon36XCore.OppositeToJ201NearFirst());
        Assert.True(ObjMon36XCore.ExplicitUpCast());
        Assert.True(ObjMon36XCore.RedundantButLegal());
        Assert.True(ObjMon36XCore.DispatchChainExtracted());
        Assert.True(ObjMon36XCore.TwoIsLowerBound());
        Assert.True(ObjMon36XCore.AxisZeroNotInRing());
        Assert.True(ObjMon36XCore.SixIsUpperBound());

        Assert.Equal(13, ObjMon36XCore.DispatchChain.Length);
    }

    [Fact]
    public void RingDistanceBoundaries()
    {
        // **修正后的边界结论：两个轴都必须 >= 2、所以坐标轴上的点不在环内**
        Assert.False(ObjMon36XCore.InRing(1, 1));
        Assert.True(ObjMon36XCore.InRing(2, 2));
        Assert.True(ObjMon36XCore.InRing(6, 6));
        Assert.False(ObjMon36XCore.InRing(7, 7));

        // **(2,0) 与 (6,0) 都不在环内 —— 方形环、非欧氏环**
        Assert.False(ObjMon36XCore.InRing(2, 0));
        Assert.False(ObjMon36XCore.InRing(6, 0));

        // **单轴不足或超出都不在环内**
        Assert.False(ObjMon36XCore.InRing(6, 1));
        Assert.False(ObjMon36XCore.InRing(2, 7));

        Assert.True(ObjMon36XCore.OneNotInRing());
        Assert.True(ObjMon36XCore.TwoInRing());
        Assert.True(ObjMon36XCore.SixInRing());
        Assert.True(ObjMon36XCore.SevenOutOfRing());
        Assert.True(ObjMon36XCore.OneAxisShortFails());
        Assert.True(ObjMon36XCore.OneAxisLongFails());
    }

    // ===================== 七、SwordWideAttack 注释块与整体 =====================

    [Fact]
    public void CommentBlockAndOverall()
    {
        Assert.True(ObjMon36XCore.FiftySevenLinesCommented());
        Assert.True(ObjMon36XCore.DeclAlsoCommented());
        Assert.True(ObjMon36XCore.NestedBraceCommentInside());
        Assert.True(ObjMon36XCore.WhileTrueWithManualBreak());
        Assert.True(ObjMon36XCore.FixedThreeIterations());
        Assert.True(ObjMon36XCore.OnlyInCommentedCode());
        Assert.True(ObjMon36XCore.HistoricalApi());
        Assert.True(ObjMon36XCore.SingleNewField());
        Assert.True(ObjMon36XCore.ThreeSitesOnly());
        Assert.True(ObjMon36XCore.NineClassesCovered());
        Assert.True(ObjMon36XCore.RemainingApprox());

        Assert.Equal(3932, ObjMon36XCore.SwordWideCommentStart);
        Assert.Equal(3988, ObjMon36XCore.SwordWideCommentEnd);
        Assert.Equal(57, ObjMon36XCore.SwordWideCommentLines);
        Assert.Equal(3, ObjMon36XCore.SwordWideIterations);
        Assert.Equal(56, ObjMon36XCore.SwordWideDeclLine);
    }

    [Fact]
    public void AppearanceSiteCounts()
    {
        Assert.Equal(6, ObjMon36XCore.Appr628Sites);
        Assert.Equal(2, ObjMon36XCore.Appr610Sites);
        Assert.Equal(4, ObjMon36XCore.Appr626Sites);
        Assert.Equal(3, ObjMon36XCore.Appr364Sites);
        Assert.Equal(2, ObjMon36XCore.Appr365Sites);
        Assert.Equal(1, ObjMon36XCore.Appr366Sites);
        Assert.Equal(5, ObjMon36XCore.Appr620Sites);
        Assert.Equal(4, ObjMon36XCore.Appr607Sites);
        Assert.Equal(3, ObjMon36XCore.Appr616Sites);
        Assert.Equal(3, ObjMon36XCore.Appr609Sites);
        Assert.Equal(2, ObjMon36XCore.Appr600Sites);
        Assert.Equal(2, ObjMon36XCore.StrengthChain609);
        Assert.Equal(0, ObjMon36XCore.StrengthChain607);
        Assert.Equal(1, ObjMon36XCore.EffectChain607);
        Assert.Equal(0, ObjMon36XCore.EffectChain616);
        Assert.Equal(1, ObjMon36XCore.StrengthChain616);
    }
}
