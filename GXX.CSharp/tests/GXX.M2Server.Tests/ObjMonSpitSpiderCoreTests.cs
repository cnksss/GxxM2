using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J201：`ObjMon.pas` 中 `TSlowATMonster` / `TScorpion` / `TSpitSpider`
/// 三个派生类 1:1 测试（七方法合计 141 行）。
/// **两个方法论要点**：
/// ① 发现两处查表下标写法不同（`[btDir, n18, n14]` 与 `[btDir, nC, n10]`）、
/// **看似颠倒但逐一追坐标后证明轴序相同、属误报**、已撤回；
/// ② 伤害值 `n1C` 在循环外算一次、循环内被改写七次、
/// **导致同一次吐攻击里越靠后的目标受伤越小**。
/// </summary>
public sealed class ObjMonSpitSpiderCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(1502, ObjMonSpitSpiderCore.SlowCommentLine);
        Assert.Equal(1503, ObjMonSpitSpiderCore.SlowCreateStart);
        Assert.Equal(4, ObjMonSpitSpiderCore.SlowCreateLines);
        Assert.Equal(1508, ObjMonSpitSpiderCore.SlowDestroyStart);
        Assert.Equal(4, ObjMonSpitSpiderCore.SlowDestroyLines);
        Assert.Equal(1513, ObjMonSpitSpiderCore.ScorpionCommentLine);
        Assert.Equal(1514, ObjMonSpitSpiderCore.ScorpionCreateStart);
        Assert.Equal(5, ObjMonSpitSpiderCore.ScorpionCreateLines);
        Assert.Equal(1520, ObjMonSpitSpiderCore.ScorpionDestroyStart);
        Assert.Equal(4, ObjMonSpitSpiderCore.ScorpionDestroyLines);
        Assert.Equal(1525, ObjMonSpitSpiderCore.SpiderCommentLine);
        Assert.Equal(1526, ObjMonSpitSpiderCore.SpiderCreateStart);
        Assert.Equal(7, ObjMonSpitSpiderCore.SpiderCreateLines);
        Assert.Equal(1534, ObjMonSpitSpiderCore.SpiderDestroyStart);
        Assert.Equal(4, ObjMonSpitSpiderCore.SpiderDestroyLines);
        Assert.Equal(1539, ObjMonSpitSpiderCore.SpitStart);
        Assert.Equal(1651, ObjMonSpitSpiderCore.SpitEnd);
        Assert.Equal(113, ObjMonSpitSpiderCore.SpitLines);
        Assert.Equal(141, ObjMonSpitSpiderCore.TotalLines);
        Assert.Equal(8, ObjMonSpitSpiderCore.SpitMapDirs);
        Assert.Equal(5, ObjMonSpitSpiderCore.SpitMapSide);
        Assert.Equal(2, ObjMonSpitSpiderCore.SpitRadius);
        Assert.Equal(-2, ObjMonSpitSpiderCore.SpitOrigin);
        Assert.Equal(20, ObjMonSpitSpiderCore.PoisonDenominator);
        Assert.Equal(30, ObjMonSpitSpiderCore.PoisonTime);
        Assert.Equal(1, ObjMonSpitSpiderCore.PoisonDechealth);
        Assert.Equal(5, ObjMonSpitSpiderCore.PoisonStone);
        Assert.Equal(1000, ObjMonSpitSpiderCore.SuckRateDenominator);
        Assert.Equal(100, ObjMonSpitSpiderCore.SuckProbabilityDenominator);
        Assert.Equal(8, ObjMonSpitSpiderCore.AnimalAssignCount);
        Assert.Equal(0, ObjMonSpitSpiderCore.AnimalReadCountInFile);
        Assert.Equal(7, ObjMonSpitSpiderCore.ShellSiteCount);
        Assert.Equal(7, ObjMonSpitSpiderCore.DamageRewriteCount);
        Assert.Equal(7, ObjMonSpitSpiderCore.PipelineStages);
        Assert.Equal(5, ObjMonSpitSpiderCore.HitConditionCount);
        Assert.Equal(5, ObjMonSpitSpiderCore.ClassesCovered);
        Assert.Equal(49, ObjMonSpitSpiderCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonSpitSpiderCore.SpanMatches());
        Assert.True(ObjMonSpitSpiderCore.TotalLinesAddUp());
        Assert.True(ObjMonSpitSpiderCore.StartsAscending());
        Assert.True(ObjMonSpitSpiderCore.CommentsPrecedeBlocks());
        Assert.True(ObjMonSpitSpiderCore.WithinUnit());
        Assert.True(ObjMonSpitSpiderCore.NoInstrumentation());
    }

    // ===================== 一、纯 shell 类与字段 =====================

    [Fact]
    public void ShellClasses()
    {
        Assert.True(ObjMonSpitSpiderCore.SlowCreateIsShell());
        Assert.True(ObjMonSpitSpiderCore.SlowDestroyIsShell());
        Assert.True(ObjMonSpitSpiderCore.BothEquivalentToNoOverride());
        Assert.True(ObjMonSpitSpiderCore.SevenShellSitesInFile());
        Assert.True(ObjMonSpitSpiderCore.ShellSitesExtracted());
        Assert.True(ObjMonSpitSpiderCore.ThisBatchContributesThree());
        Assert.True(ObjMonSpitSpiderCore.NameSuggestsSlow());
        Assert.True(ObjMonSpitSpiderCore.NoRunOverride());
        Assert.True(ObjMonSpitSpiderCore.NoAttackOverride());
        Assert.True(ObjMonSpitSpiderCore.IdenticalBehaviorToParent());

        Assert.Equal(7, ObjMonSpitSpiderCore.ShellSites.Length);
        Assert.Equal("TMonster.Operate", ObjMonSpitSpiderCore.ShellSites[0]);
        Assert.Equal("TSpitSpider.Destroy", ObjMonSpitSpiderCore.ShellSites[6]);
    }

    [Fact]
    public void AnimalFieldFacts()
    {
        Assert.True(ObjMonSpitSpiderCore.ScorpionSetsAnimalTrue());
        Assert.True(ObjMonSpitSpiderCore.InheritedBeforeAssign());
        Assert.True(ObjMonSpitSpiderCore.SameOrderAsJ199());
        Assert.True(ObjMonSpitSpiderCore.SpiderSetsThreeFields());
        Assert.True(ObjMonSpitSpiderCore.SearchTimeVerbatimWithJ200());
        Assert.True(ObjMonSpitSpiderCore.SpiderAlsoDoesNotReadIt());
        Assert.True(ObjMonSpitSpiderCore.AnimalWriteOnlyInFile());
        Assert.True(ObjMonSpitSpiderCore.AnimalReadersElsewhere());
        Assert.True(ObjMonSpitSpiderCore.EightAssignsZeroReads());
        Assert.True(ObjMonSpitSpiderCore.AnimalAssignSitesExtracted());
        Assert.True(ObjMonSpitSpiderCore.AnimalReadersExtracted());
        Assert.True(ObjMonSpitSpiderCore.ConsumedByBaseClass());
        Assert.True(ObjMonSpitSpiderCore.DiggingRelatedComment());
        Assert.True(ObjMonSpitSpiderCore.AnimalOffsetExtracted());
        Assert.True(ObjMonSpitSpiderCore.NoReaderInThisFile());

        Assert.Equal("0x2BB", ObjMonSpitSpiderCore.AnimalOffset);

        // **八个赋值点、无一在本批三个类之外被读**
        Assert.Equal(new[] { 1517, 1530, 1687, 1700, 1714, 2089, 7958, 9364 },
            ObjMonSpitSpiderCore.AnimalAssignSites);

        // **读取点全在别的文件**
        Assert.Equal(11, ObjMonSpitSpiderCore.AnimalReaders.Length);
        Assert.StartsWith("ObjBase.pas", ObjMonSpitSpiderCore.AnimalReaders[0]);
        Assert.Equal("ObjCustomMon.pas:1819", ObjMonSpitSpiderCore.AnimalReaders[10]);
    }

    // ===================== 二、5×5 图案与"撤回的怀疑" =====================

    [Fact]
    public void SpitMapFacts()
    {
        Assert.True(ObjMonSpitSpiderCore.LookupTableDriven());
        Assert.True(ObjMonSpitSpiderCore.TableIs8x5x5());
        Assert.True(ObjMonSpitSpiderCore.TableInitializedAsLiteral());
        Assert.True(ObjMonSpitSpiderCore.UpIsTwoCellLine());
        Assert.True(ObjMonSpitSpiderCore.UpHasExactlyTwoOnes());
        Assert.True(ObjMonSpitSpiderCore.DoubleLoopOver5x5());
        Assert.True(ObjMonSpitSpiderCore.OuterIsYInnerIsX());
        Assert.True(ObjMonSpitSpiderCore.MultipleTargetsPerSpit());

        Assert.Equal(2, ObjMonSpitSpiderCore.CountOnes(ObjMonSpitSpiderCore.UpPattern));

        // **DR_UP 真实值：1 在前两行的中列**
        Assert.Equal(1, ObjMonSpitSpiderCore.UpPattern[0][2]);
        Assert.Equal(1, ObjMonSpitSpiderCore.UpPattern[1][2]);
        Assert.Equal(0, ObjMonSpitSpiderCore.UpPattern[2][2]);
    }

    [Fact]
    public void SuspicionRetracted()
    {
        // **两处查表写法文本上不同**
        Assert.True(ObjMonSpitSpiderCore.SisterLookupDiffersTextually());

        // **但轴序相同、不是下标颠倒**
        Assert.True(ObjMonSpitSpiderCore.ButAxisOrderIdentical());
        Assert.True(ObjMonSpitSpiderCore.NotATranspositionBug());
        Assert.True(ObjMonSpitSpiderCore.XIsInnerYIsOuter());
        Assert.True(ObjMonSpitSpiderCore.SuspicionRetracted());
        Assert.True(ObjMonSpitSpiderCore.SisterAgreesOnCell());
    }

    [Fact]
    public void AxisConversion()
    {
        Assert.True(ObjMonSpitSpiderCore.MinDeltaMapsToZero());
        Assert.True(ObjMonSpitSpiderCore.MaxDeltaMapsToFour());
        Assert.True(ObjMonSpitSpiderCore.IndexRoundTrips());

        Assert.Equal(0, ObjMonSpitSpiderCore.ToIndex(-2));
        Assert.Equal(4, ObjMonSpitSpiderCore.ToIndex(2));
        Assert.Equal(0, ObjMonSpitSpiderCore.ToDelta(0) + 2);
    }

    // ===================== 三、变量复用 =====================

    [Fact]
    public void VariableReuseFacts()
    {
        Assert.True(ObjMonSpitSpiderCore.VariableHasTwoRoles());
        Assert.True(ObjMonSpitSpiderCore.DamageComputedOnceOutsideLoop());
        Assert.True(ObjMonSpitSpiderCore.RewrittenSevenTimesInside());
        Assert.True(ObjMonSpitSpiderCore.DamageDrainsAcrossTargets());
        Assert.True(ObjMonSpitSpiderCore.ZeroStopsLaterTargets());
        Assert.True(ObjMonSpitSpiderCore.DeadGuardShape());
        Assert.True(ObjMonSpitSpiderCore.RandomOnlyWhenPositive());
        Assert.True(ObjMonSpitSpiderCore.ExitBeforeSendRefMsg());
        Assert.True(ObjMonSpitSpiderCore.NoHitMessageWhenNonPositive());
        Assert.True(ObjMonSpitSpiderCore.OldPackedDcFormCommentedOut());
        Assert.True(ObjMonSpitSpiderCore.FieldUpgradeLeftTrace());
    }

    [Fact]
    public void DamageRollBoundaries()
    {
        Assert.True(ObjMonSpitSpiderCore.RangeWidthIsDc2MinusDc1Plus1());

        // **正常区间：DC1..DC2**
        Assert.Equal(10, ObjMonSpitSpiderCore.RollDamage(10, 20, 0));
        Assert.Equal(20, ObjMonSpitSpiderCore.RollDamage(10, 20, 10));

        // **`DC2 < DC1` 时结果是 `DC2 + 1`（本批自查纠正）**
        Assert.True(ObjMonSpitSpiderCore.DegeneratesWhenDc2BelowDc1());
        Assert.Equal(11, ObjMonSpitSpiderCore.RollDamage(20, 10, 5));
    }

    [Fact]
    public void DamageDrainsAcrossTargets()
    {
        // **第二个目标受伤更少**
        Assert.True(ObjMonSpitSpiderCore.SecondTargetTakesLess());

        // **吸干后后续目标为零**
        Assert.True(ObjMonSpitSpiderCore.DrainedLeavesZero());

        // **无吸收时不变**
        Assert.True(ObjMonSpitSpiderCore.NoAbsorbKeepsValue());

        Assert.Equal(new[] { 70, 70 },
            ObjMonSpitSpiderCore.DamageAcrossTargets(100, new[] { 30, 0 }));
        Assert.Equal(new[] { 0, 0, 0 },
            ObjMonSpitSpiderCore.DamageAcrossTargets(50, new[] { 50, 0, 0 }));
        Assert.Equal(new[] { 77, 77, 77 },
            ObjMonSpitSpiderCore.DamageAcrossTargets(77, new[] { 0, 0, 0 }));
    }

    // ===================== 四、伤害管线 =====================

    [Fact]
    public void PipelineFacts()
    {
        Assert.True(ObjMonSpitSpiderCore.SevenStagePipeline());
        Assert.True(ObjMonSpitSpiderCore.OrderIsFixed());
        Assert.True(ObjMonSpitSpiderCore.DefenseTwoArities());
        Assert.True(ObjMonSpitSpiderCore.ThreeArgVsFourArg());
        Assert.True(ObjMonSpitSpiderCore.AbsorbOnlyForPlayers());
        Assert.True(ObjMonSpitSpiderCore.ThreeRacesAbsorb());
        Assert.True(ObjMonSpitSpiderCore.ThreeAbsorbLayers());
        Assert.True(ObjMonSpitSpiderCore.SuckClampedByPool());
        Assert.True(ObjMonSpitSpiderCore.SuckNeverExceedsPool());
        Assert.True(ObjMonSpitSpiderCore.TenPercentSucksTen());
        Assert.True(ObjMonSpitSpiderCore.PoisonAndParalysisSeparate());
        Assert.True(ObjMonSpitSpiderCore.PoisonFivePercent());
        Assert.True(ObjMonSpitSpiderCore.PoisonFixedThirty());
        Assert.True(ObjMonSpitSpiderCore.ParalysisTwoPaths());
        Assert.True(ObjMonSpitSpiderCore.ExplicitRandomizeCall());
        Assert.True(ObjMonSpitSpiderCore.MaxGuardOnRandomArg());
        Assert.True(ObjMonSpitSpiderCore.DefensiveWrite());
        Assert.True(ObjMonSpitSpiderCore.ReboundIsLast());
        Assert.True(ObjMonSpitSpiderCore.ReboundPassesNil());
        Assert.True(ObjMonSpitSpiderCore.ReboundTagFT());
        Assert.True(ObjMonSpitSpiderCore.PositiveTagEmpty());
        Assert.True(ObjMonSpitSpiderCore.MutatesTargetNgPool());
        Assert.True(ObjMonSpitSpiderCore.CallsRefAbilNH());
    }

    [Fact]
    public void AbsorbRaceBoundaries()
    {
        Assert.True(ObjMonSpitSpiderCore.PlayerAbsorbs());
        Assert.True(ObjMonSpitSpiderCore.HeroAbsorbs());
        Assert.True(ObjMonSpitSpiderCore.MonsterDoesNotAbsorb());

        Assert.True(ObjMonSpitSpiderCore.CanAbsorb(0));
        Assert.True(ObjMonSpitSpiderCore.CanAbsorb(1));
        Assert.True(ObjMonSpitSpiderCore.CanAbsorb(152));
        Assert.False(ObjMonSpitSpiderCore.CanAbsorb(80));
        Assert.False(ObjMonSpitSpiderCore.CanAbsorb(10));
    }

    [Fact]
    public void SuckBoundaries()
    {
        // **吸血不超过池子**
        Assert.Equal(5, ObjMonSpitSpiderCore.SuckPoint(1000, 100, 5));

        // **比率 100/1000 时吸一成**
        Assert.Equal(10, ObjMonSpitSpiderCore.SuckPoint(100, 100, 999));

        // **池子为零时吸不到**
        Assert.Equal(0, ObjMonSpitSpiderCore.SuckPoint(1000, 100, 0));
    }

    // ===================== 五、命中与循环 =====================

    [Fact]
    public void HitAndLoopFacts()
    {
        Assert.True(ObjMonSpitSpiderCore.FiveHitConditions());
        Assert.True(ObjMonSpitSpiderCore.HitFormula());
        Assert.True(ObjMonSpitSpiderCore.NoGuardOnRandomArgSpeedPoint());
        Assert.True(ObjMonSpitSpiderCore.NoExplicitDeathCheck());
        Assert.True(ObjMonSpitSpiderCore.DelegatedToIsProperTarget());
        Assert.True(ObjMonSpitSpiderCore.WhileNotFor());
        Assert.True(ObjMonSpitSpiderCore.ManualInc());
        Assert.True(ObjMonSpitSpiderCore.TwoBreaksCommentedOut());
        Assert.True(ObjMonSpitSpiderCore.DifferentCommentStyles());
    }

    [Fact]
    public void HitBoundaries()
    {
        Assert.True(ObjMonSpitSpiderCore.HigherHitEasier());
        Assert.True(ObjMonSpitSpiderCore.ZeroSpeedUndefined());

        Assert.True(ObjMonSpitSpiderCore.RollsHit(10, 9, 8));
        Assert.False(ObjMonSpitSpiderCore.RollsHit(10, 3, 8));

        // **敏捷为 0 时无定义（此处视为不中）**
        Assert.False(ObjMonSpitSpiderCore.RollsHit(0, 10, 0));
    }

    [Fact]
    public void StaleCommentFacts()
    {
        Assert.True(ObjMonSpitSpiderCore.SixLineBraceComment());
        Assert.True(ObjMonSpitSpiderCore.CommentShowsUpPattern());
        Assert.True(ObjMonSpitSpiderCore.CommentIsStale());
        Assert.True(ObjMonSpitSpiderCore.RowsReversedVsReal());
        Assert.True(ObjMonSpitSpiderCore.CommentLessTrustworthyThanCode());
        Assert.True(ObjMonSpitSpiderCore.SameCountDifferentPlacement());

        // **1 的个数相同、但位置相反**
        Assert.Equal(2, ObjMonSpitSpiderCore.CountOnes(ObjMonSpitSpiderCore.UpPattern));
        Assert.Equal(2,
            ObjMonSpitSpiderCore.CountOnes(ObjMonSpitSpiderCore.UpPatternInComment));

        // **真实值 1 在第 0 行、注释里第 0 行是 0**
        Assert.Equal(1, ObjMonSpitSpiderCore.UpPattern[0][2]);
        Assert.Equal(0, ObjMonSpitSpiderCore.UpPatternInComment[0][2]);

        // **注释里 1 在第 3 行、真实值第 3 行是 0**
        Assert.Equal(1, ObjMonSpitSpiderCore.UpPatternInComment[3][2]);
        Assert.Equal(0, ObjMonSpitSpiderCore.UpPattern[3][2]);
    }

    // ===================== 六、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonSpitSpiderCore.OverridesAttackTarget());
        Assert.True(ObjMonSpitSpiderCore.FfebMarkerAgain());
        Assert.True(ObjMonSpitSpiderCore.BraceWrappedVirtual());
        Assert.True(ObjMonSpitSpiderCore.AttackTargetDeferred());
        Assert.True(ObjMonSpitSpiderCore.NextBatchScope());
        Assert.True(ObjMonSpitSpiderCore.FiveClassesCovered());
        Assert.True(ObjMonSpitSpiderCore.RemainingApprox());
    }
}
