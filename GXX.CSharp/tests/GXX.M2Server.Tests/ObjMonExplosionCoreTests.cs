using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J207：`ObjMon.pas` 中 `TExplosionAttackMonster`（冰咆哮怪物）
/// 两个方法 1:1 测试（合计 150 行）。
/// **本批最有价值的发现**：本方法的活体（4848-4875）与 J206 里
/// `TMagicAttackMonster.MagicAttackTarget` 被注释掉的旧体（4634-4651）
/// **结构完全相同**，而 J206 那两处"同一轴判两次"的笔误
/// **在本批的活体里已修正为 `Abs(X) and Abs(Y)`** ——
/// 一次跨批次的"注释体 vs 活体"演化对照。
/// </summary>
public sealed class ObjMonExplosionCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(4731, ObjMonExplosionCore.MagicStart);
        Assert.Equal(4876, ObjMonExplosionCore.MagicEnd);
        Assert.Equal(146, ObjMonExplosionCore.MagicLines);
        Assert.Equal(4733, ObjMonExplosionCore.NestedStart);
        Assert.Equal(4845, ObjMonExplosionCore.NestedEnd);
        Assert.Equal(113, ObjMonExplosionCore.NestedLines);
        Assert.Equal(4847, ObjMonExplosionCore.OuterStart);
        Assert.Equal(4876, ObjMonExplosionCore.OuterEnd);
        Assert.Equal(30, ObjMonExplosionCore.OuterLines);
        Assert.Equal(4878, ObjMonExplosionCore.RunStart);
        Assert.Equal(4881, ObjMonExplosionCore.RunEnd);
        Assert.Equal(4, ObjMonExplosionCore.RunLines);
        Assert.Equal(150, ObjMonExplosionCore.TotalLines);

        Assert.Equal(4638, ObjMonExplosionCore.J206TypoEnterLine);
        Assert.Equal(4646, ObjMonExplosionCore.J206TypoApproachLine);
        Assert.Equal(4855, ObjMonExplosionCore.FixedEnterLine);
        Assert.Equal(4866, ObjMonExplosionCore.FixedApproachLine);
        Assert.Equal(4634, ObjMonExplosionCore.J206StubStart);
        Assert.Equal(4651, ObjMonExplosionCore.J206StubEnd);
        Assert.Equal(18, ObjMonExplosionCore.J206StubLines);
        Assert.Equal(4859, ObjMonExplosionCore.LiveCallLine);

        Assert.Equal(4748, ObjMonExplosionCore.HealSectionStart);
        Assert.Equal(4756, ObjMonExplosionCore.HealSectionEnd);
        Assert.Equal(4757, ObjMonExplosionCore.PoisonSectionStart);
        Assert.Equal(4770, ObjMonExplosionCore.PoisonSectionEnd);
        Assert.Equal(4771, ObjMonExplosionCore.GroupSectionStart);
        Assert.Equal(4843, ObjMonExplosionCore.GroupSectionEnd);
        Assert.Equal(4844, ObjMonExplosionCore.EffectLine);
        Assert.Equal(231, ObjMonExplosionCore.Appr231);

        Assert.Equal(3, ObjMonExplosionCore.HealDenominator);
        Assert.Equal(0, ObjMonExplosionCore.HealMpAmount);
        Assert.Equal(10, ObjMonExplosionCore.PoisonTimeMin);
        Assert.Equal(69, ObjMonExplosionCore.PoisonTimeMax);
        Assert.Equal(60, ObjMonExplosionCore.PoisonTimeBound);
        Assert.Equal(10, ObjMonExplosionCore.PoisonTimeBase);
        Assert.Equal(0, ObjMonExplosionCore.POISON_DECHEALTH);
        Assert.Equal(5, ObjMonExplosionCore.POISON_STONE);
        Assert.Equal(6, ObjMonExplosionCore.SelfFilterRadius);
        Assert.Equal(33, ObjMonExplosionCore.IceRoarEffectType);
        Assert.Equal(1, ObjMonExplosionCore.SnowWindRangeDefault);
        Assert.Equal(12, ObjMonExplosionCore.ClassesCovered);
        Assert.Equal(42, ObjMonExplosionCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonExplosionCore.SpanMatches());
        Assert.True(ObjMonExplosionCore.TotalLinesAddUp());
        Assert.True(ObjMonExplosionCore.NestedBeforeOuter());
        Assert.True(ObjMonExplosionCore.NestedInsideMethod());
        Assert.True(ObjMonExplosionCore.OuterInsideMethod());
        Assert.True(ObjMonExplosionCore.OuterEndsAtMethodEnd());
        Assert.True(ObjMonExplosionCore.DecompositionAddsUp());
        Assert.True(ObjMonExplosionCore.RunAfterMagic());
        Assert.True(ObjMonExplosionCore.WithinUnit());
        Assert.True(ObjMonExplosionCore.NoInstrumentation());
        Assert.True(ObjMonExplosionCore.SectionsAscending());
        Assert.True(ObjMonExplosionCore.SectionSpansMatch());

        // **完整分解：1 头 + 1 空 + 113 嵌套 + 1 空 + 30 外层 = 146**
        Assert.Equal(146,
            ObjMonExplosionCore.HeaderLines + ObjMonExplosionCore.BlankBeforeNested
            + ObjMonExplosionCore.NestedLines + ObjMonExplosionCore.BlankAfterNested
            + ObjMonExplosionCore.OuterLines);
    }

    // ===================== 一、与 J206 旧体对照 =====================

    [Fact]
    public void CrossBatchEvidence()
    {
        Assert.True(ObjMonExplosionCore.LiveBodyMatchesStubShape());
        Assert.True(ObjMonExplosionCore.BothTyposFixedHere());
        Assert.True(ObjMonExplosionCore.XAxisBecomesY());
        Assert.True(ObjMonExplosionCore.SameStructureDifferentCorrectness());
        Assert.True(ObjMonExplosionCore.TypoFixesExtracted());
        Assert.True(ObjMonExplosionCore.FixLinesAscending());
        Assert.True(ObjMonExplosionCore.StubSaysSameAxis());
        Assert.True(ObjMonExplosionCore.LiveSaysTwoAxes());
        Assert.True(ObjMonExplosionCore.FixesActuallyDiffer());
        Assert.True(ObjMonExplosionCore.StubSpanMatches());
        Assert.True(ObjMonExplosionCore.StubIsShorter());
        Assert.True(ObjMonExplosionCore.TwelveLinesApart());
        Assert.True(ObjMonExplosionCore.LiveHasNilGuard());
    }

    [Fact]
    public void TypoFixTable()
    {
        Assert.Equal(2, ObjMonExplosionCore.TypoFixes.Length);

        // **第一处：进入判据，旧体同轴、活体两轴**
        Assert.Equal(4638, ObjMonExplosionCore.TypoFixes[0].StubLine);
        Assert.Equal(4855, ObjMonExplosionCore.TypoFixes[0].LiveLine);
        Assert.Contains("abs(X)<=6 and abs(X)", ObjMonExplosionCore.TypoFixes[0].Stub);
        Assert.Contains("Abs(X)<=6 and Abs(Y)", ObjMonExplosionCore.TypoFixes[0].Live);

        // **第二处：靠近判据，同样修正**
        Assert.Equal(4646, ObjMonExplosionCore.TypoFixes[1].StubLine);
        Assert.Equal(4866, ObjMonExplosionCore.TypoFixes[1].LiveLine);
        Assert.Contains("abs(X)>6 or abs(X)", ObjMonExplosionCore.TypoFixes[1].Stub);
        Assert.Contains("Abs(X)>6 or Abs(Y)", ObjMonExplosionCore.TypoFixes[1].Live);
    }

    [Fact]
    public void StubMissingPieces()
    {
        Assert.True(ObjMonExplosionCore.MissingResultInit());
        Assert.True(ObjMonExplosionCore.MissingNilGuard());
        Assert.True(ObjMonExplosionCore.CallWasCommented());
        Assert.True(ObjMonExplosionCore.CallIsLiveHere());
        Assert.True(ObjMonExplosionCore.CaseOfAbsDiffers());
        Assert.True(ObjMonExplosionCore.CaseInsensitiveSoCosmetic());
        Assert.True(ObjMonExplosionCore.EraWitness());

        Assert.Equal(2, ObjMonExplosionCore.MissingFromStub.Length);
        Assert.Equal("Result := False;", ObjMonExplosionCore.MissingFromStub[0]);
        Assert.Contains("nil", ObjMonExplosionCore.MissingFromStub[1]);
    }

    // ===================== 二、四段结构 =====================

    [Fact]
    public void FourSectionFacts()
    {
        Assert.True(ObjMonExplosionCore.FourSections());
        Assert.True(ObjMonExplosionCore.Appr231IsExclusive());
        Assert.True(ObjMonExplosionCore.SelfHealOrPoisonNeverBoth());
        Assert.True(ObjMonExplosionCore.SectionsExtracted());

        Assert.Equal(4, ObjMonExplosionCore.Sections.Length);
        Assert.Equal(1, ObjMonExplosionCore.Sections[0].Section);
        Assert.Equal(4748, ObjMonExplosionCore.Sections[0].Start);
        Assert.Equal(4, ObjMonExplosionCore.Sections[3].Section);
        Assert.Equal(4844, ObjMonExplosionCore.Sections[3].Start);
    }

    [Fact]
    public void SectionDispatchBoundaries()
    {
        // **231 低血掷中 => 自愈**
        Assert.True(ObjMonExplosionCore.Appr231Heals());
        Assert.Equal("self-heal", ObjMonExplosionCore.Section(231, true, 0, false));

        // **231 高血 => 什么都不做（既不施毒也不群攻）**
        Assert.True(ObjMonExplosionCore.Appr231HighHpDoesNothing());
        Assert.Equal("none", ObjMonExplosionCore.Section(231, false, 0, false));

        // **非 231 未中毒 => 施毒**
        Assert.True(ObjMonExplosionCore.NonAppr231Poisons());

        // **已中毒 => 群体伤害**
        Assert.True(ObjMonExplosionCore.AlreadyPoisonedGroups());

        // **互斥：231 永不施毒、非 231 永不自愈**
        Assert.True(ObjMonExplosionCore.Appr231NeverPoisons());
        Assert.True(ObjMonExplosionCore.NonAppr231NeverHeals());
    }

    [Fact]
    public void SelfHealBoundaries()
    {
        Assert.True(ObjMonExplosionCore.HalfHpThreshold());
        Assert.True(ObjMonExplosionCore.OneInThree());
        Assert.True(ObjMonExplosionCore.SameShapeAsJ205Summon());
        Assert.True(ObjMonExplosionCore.ThresholdsDiffer());
        Assert.True(ObjMonExplosionCore.LowHpRollsZeroHeals());
        Assert.True(ObjMonExplosionCore.ExactlyHalfBlocks());
        Assert.True(ObjMonExplosionCore.MissedRollNoHeal());
        Assert.True(ObjMonExplosionCore.AttackPowerAsHealAmount());
        Assert.True(ObjMonExplosionCore.ThirdParamOmitted());
        Assert.True(ObjMonExplosionCore.DefaultsToTrue());
        Assert.True(ObjMonExplosionCore.HealMpIsZero());

        // **阈值比 J205 的三分之一高**
        Assert.True(ObjMonExplosionCore.HealHpThreshold
            > ObjMonExplosionCore.J205SummonThreshold);

        // **恰好半血不触发（严格小于）**
        Assert.False(ObjMonExplosionCore.CanHeal(50, 100, 0));
        Assert.True(ObjMonExplosionCore.CanHeal(49, 100, 0));
    }

    [Fact]
    public void PoisonIndexFacts()
    {
        Assert.True(ObjMonExplosionCore.ChecksStatusTimeZero());
        Assert.True(ObjMonExplosionCore.PoisonDechealthIsZero());
        Assert.True(ObjMonExplosionCore.IndexMatchesConstant());
        Assert.True(ObjMonExplosionCore.PoisonConstantsExtracted());
        Assert.True(ObjMonExplosionCore.PoisonStoneIsFive());
        Assert.True(ObjMonExplosionCore.ParalysisIndexIsFive());
        Assert.True(ObjMonExplosionCore.GreenAndStoneDifferSlots());
        Assert.True(ObjMonExplosionCore.ValuesAreSparse());

        Assert.Equal(5, ObjMonExplosionCore.PoisonConstants.Length);
        Assert.Equal("POISON_DECHEALTH", ObjMonExplosionCore.PoisonConstants[0].Name);
        Assert.Equal(0, ObjMonExplosionCore.PoisonConstants[0].Value);
        Assert.Equal(1, ObjMonExplosionCore.PoisonConstants[1].Value);
        Assert.Equal(2, ObjMonExplosionCore.PoisonConstants[2].Value);
        Assert.Equal(4, ObjMonExplosionCore.PoisonConstants[3].Value);
        Assert.Equal("POISON_STONE", ObjMonExplosionCore.PoisonConstants[4].Name);
        Assert.Equal(5, ObjMonExplosionCore.PoisonConstants[4].Value);
    }

    [Fact]
    public void PoisonTimeBoundaries()
    {
        Assert.True(ObjMonExplosionCore.Duration10To69());
        Assert.True(ObjMonExplosionCore.UpperBoundIs69());
        Assert.True(ObjMonExplosionCore.MinPoisonTime());
        Assert.True(ObjMonExplosionCore.MaxPoisonTime());

        Assert.Equal(10, ObjMonExplosionCore.PoisonTime(0));
        Assert.Equal(69, ObjMonExplosionCore.PoisonTime(59));
        Assert.Equal(11, ObjMonExplosionCore.PoisonTime(1));
    }

    [Fact]
    public void PoisonStrengthRounding()
    {
        Assert.True(ObjMonExplosionCore.StrengthIsTenPercentPlusOne());
        Assert.True(ObjMonExplosionCore.GuaranteesAtLeastOne());
        Assert.True(ObjMonExplosionCore.ZeroPowerStillOne());
        Assert.True(ObjMonExplosionCore.HundredPowerGives11());
        Assert.True(ObjMonExplosionCore.TenPowerGives2());
        Assert.True(ObjMonExplosionCore.AlwaysPositive());
        Assert.True(ObjMonExplosionCore.FloatDivisionVsDiv());
        Assert.True(ObjMonExplosionCore.DivisionsDifferAt15());
        Assert.True(ObjMonExplosionCore.RoundVsTruncate());

        // **浮点版四舍五入得 3，整除版截断得 2**
        Assert.Equal(3, ObjMonExplosionCore.PoisonStrength(15));
        Assert.Equal(2, ObjMonExplosionCore.IntegerDivisionStrength(15));

        // **保证恒 >= 1**
        Assert.Equal(1, ObjMonExplosionCore.PoisonStrength(0));
    }

    [Fact]
    public void RandomGuardInconsistency()
    {
        Assert.True(ObjMonExplosionCore.EffectCommentedWithReason());
        Assert.True(ObjMonExplosionCore.ExitAfterPoison());
        Assert.True(ObjMonExplosionCore.DocumentedSuppression());
        Assert.True(ObjMonExplosionCore.NoMaxGuardHere());
        Assert.True(ObjMonExplosionCore.SiblingHasMaxGuard());
        Assert.True(ObjMonExplosionCore.RandomZeroRisk());
        Assert.True(ObjMonExplosionCore.InconsistentWithNeighbour());
        Assert.True(ObjMonExplosionCore.ParalysisHasGuard());
        Assert.True(ObjMonExplosionCore.PoisonLacksGuard());
        Assert.True(ObjMonExplosionCore.ContrastWithinOneMethod());
        Assert.True(ObjMonExplosionCore.UnguardedStillDecides());
        Assert.True(ObjMonExplosionCore.GuardedClamps());
    }

    [Fact]
    public void GroupFilterBoundaries()
    {
        Assert.True(ObjMonExplosionCore.CenteredOnTarget());
        Assert.True(ObjMonExplosionCore.RadiusFromConfig());
        Assert.True(ObjMonExplosionCore.DefaultIsOne());
        Assert.True(ObjMonExplosionCore.ContrastWithHardcodedSiblings());
        Assert.True(ObjMonExplosionCore.DoubleFiltering());
        Assert.True(ObjMonExplosionCore.SelfCenteredSixSquare());
        Assert.True(ObjMonExplosionCore.NoHideFilter());
        Assert.True(ObjMonExplosionCore.OnlyProperAndOffline());
        Assert.True(ObjMonExplosionCore.SwitchesTargetInLoop());
        Assert.True(ObjMonExplosionCore.AbsentInJ206Stub());
        Assert.True(ObjMonExplosionCore.NewBehaviour());
        Assert.True(ObjMonExplosionCore.TargetChangesAfterGroupAttack());

        Assert.True(ObjMonExplosionCore.AllSatisfiedPasses());
        Assert.True(ObjMonExplosionCore.BeyondSixExcluded());
        Assert.True(ObjMonExplosionCore.ExactlySixInside());
        Assert.True(ObjMonExplosionCore.ImproperExcluded());
        Assert.True(ObjMonExplosionCore.OfflineExcluded());

        // **边界：恰好 6 格在内、7 格在外**
        Assert.True(ObjMonExplosionCore.PassesBothFilters(6, 6, true, false));
        Assert.False(ObjMonExplosionCore.PassesBothFilters(7, 6, true, false));
        Assert.False(ObjMonExplosionCore.PassesBothFilters(6, 7, true, false));
    }

    [Fact]
    public void ResourceAndPipelineFacts()
    {
        Assert.True(ObjMonExplosionCore.NoTryFinally());
        Assert.True(ObjMonExplosionCore.SiblingsHaveIt());
        Assert.True(ObjMonExplosionCore.FreeBeforeSend());
        Assert.True(ObjMonExplosionCore.OppositeToJ205Stage3());
        Assert.True(ObjMonExplosionCore.CapBeforeAbsorb());
        Assert.True(ObjMonExplosionCore.SameAsJ205());
        Assert.True(ObjMonExplosionCore.StillInconsistentFileWide());
        Assert.True(ObjMonExplosionCore.FifthOccurrenceOfHealIdiom());
        Assert.True(ObjMonExplosionCore.VerbatimAgain());

        // **回血写法（第五次逐字再现）**
        Assert.Equal(0, ObjMonExplosionCore.HealAmount(1000, 0));
        Assert.Equal(100, ObjMonExplosionCore.HealAmount(1000, 10));
        Assert.True(ObjMonExplosionCore.ZeroMpNoHeal());
        Assert.True(ObjMonExplosionCore.TenMpTenthHeal());
    }

    [Fact]
    public void EffectFacts()
    {
        Assert.True(ObjMonExplosionCore.EffectType33());
        Assert.True(ObjMonExplosionCore.LargestSoFar());
        Assert.True(ObjMonExplosionCore.DocumentedAsIceRoar());

        Assert.Equal(33, ObjMonExplosionCore.IceRoarEffectType);
        Assert.Equal(200, ObjMonExplosionCore.DelayMs);
    }

    // ===================== 三、Run 与整体 =====================

    [Fact]
    public void RunAndOverallFacts()
    {
        Assert.True(ObjMonExplosionCore.PureInheritedShell());
        Assert.True(ObjMonExplosionCore.TenthOccurrence());
        Assert.True(ObjMonExplosionCore.ReusesBaseRun());
        Assert.True(ObjMonExplosionCore.TwoMethodsOnly());
        Assert.True(ObjMonExplosionCore.NoCreate());
        Assert.True(ObjMonExplosionCore.RedundantOverride());
        Assert.True(ObjMonExplosionCore.TwelveClassesCovered());
        Assert.True(ObjMonExplosionCore.RemainingApprox());

        // **`Run` 只有四行：头 + begin + inherited + end**
        Assert.Equal(4, ObjMonExplosionCore.RunLines);
    }
}
