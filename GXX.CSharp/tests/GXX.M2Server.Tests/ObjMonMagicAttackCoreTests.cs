using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J206：`ObjMon.pas` 中 `TMagicAttackMonster` 四个方法 1:1 测试
/// （合计 129 行）。
/// **本批最有价值的发现**：`MagicAttackTarget`（50 行）里
/// **只有 4 行不是注释**（其中真正执行的只有 `Result := False;` 一行）——
/// 它是一个永远返回假的空壳，作为 **12 个子类**的公共基座。
/// </summary>
public sealed class ObjMonMagicAttackCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(4597, ObjMonMagicAttackCore.CreateStart);
        Assert.Equal(4602, ObjMonMagicAttackCore.CreateEnd);
        Assert.Equal(6, ObjMonMagicAttackCore.CreateLines);
        Assert.Equal(4604, ObjMonMagicAttackCore.MagicStart);
        Assert.Equal(4653, ObjMonMagicAttackCore.MagicEnd);
        Assert.Equal(50, ObjMonMagicAttackCore.MagicLines);
        Assert.Equal(4655, ObjMonMagicAttackCore.AttackTargetStart);
        Assert.Equal(4693, ObjMonMagicAttackCore.AttackTargetEnd);
        Assert.Equal(39, ObjMonMagicAttackCore.AttackTargetLines);
        Assert.Equal(4695, ObjMonMagicAttackCore.RunStart);
        Assert.Equal(4728, ObjMonMagicAttackCore.RunEnd);
        Assert.Equal(34, ObjMonMagicAttackCore.RunLines);
        Assert.Equal(129, ObjMonMagicAttackCore.TotalLines);

        Assert.Equal(4605, ObjMonMagicAttackCore.Comment1Start);
        Assert.Equal(4630, ObjMonMagicAttackCore.Comment1End);
        Assert.Equal(26, ObjMonMagicAttackCore.Comment1Lines);
        Assert.Equal(4633, ObjMonMagicAttackCore.Comment2Start);
        Assert.Equal(4652, ObjMonMagicAttackCore.Comment2End);
        Assert.Equal(20, ObjMonMagicAttackCore.Comment2Lines);
        Assert.Equal(4, ObjMonMagicAttackCore.LiveLines);
        Assert.Equal(46, ObjMonMagicAttackCore.CommentLines);
        Assert.Equal(92, ObjMonMagicAttackCore.CommentPercent);
        Assert.Equal(4632, ObjMonMagicAttackCore.ResultFalseLine);

        Assert.Equal(4667, ObjMonMagicAttackCore.PhysicalStart);
        Assert.Equal(4690, ObjMonMagicAttackCore.PhysicalEnd);
        Assert.Equal(24, ObjMonMagicAttackCore.PhysicalLines);

        Assert.Equal(7, ObjMonMagicAttackCore.ViewRange);
        Assert.Equal(4947, ObjMonMagicAttackCore.MagicFlagFalseLine);
        Assert.Equal(5, ObjMonMagicAttackCore.MagicFlagSites);
        Assert.Equal(2, ObjMonMagicAttackCore.MagicFlagAssigns);
        Assert.Equal(3, ObjMonMagicAttackCore.MagicFlagReads);
        Assert.Equal(7, ObjMonMagicAttackCore.ViewRangeSevenSites);
        Assert.Equal(12, ObjMonMagicAttackCore.SubclassCount);
        Assert.Equal(12, ObjMonMagicAttackCore.MagicImplementations);
        Assert.Equal(134, ObjMonMagicAttackCore.MlsbDeclLine);

        Assert.Equal(8000, ObjMonMagicAttackCore.SearchWithTargetMs);
        Assert.Equal(1000, ObjMonMagicAttackCore.SearchWithoutTargetMs);
        Assert.Equal(4, ObjMonMagicAttackCore.OuterBand);
        Assert.Equal(2, ObjMonMagicAttackCore.InnerBand);
        Assert.Equal(2, ObjMonMagicAttackCore.InnerRandomBound);
        Assert.Equal(5, ObjMonMagicAttackCore.OuterRandomBound);
        Assert.Equal(-1, ObjMonMagicAttackCore.TargetSentinel);
        Assert.Equal(4725, ObjMonMagicAttackCore.InheritedLine);
        Assert.Equal(11, ObjMonMagicAttackCore.ClassesCovered);
        Assert.Equal(43, ObjMonMagicAttackCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonMagicAttackCore.SpanMatches());
        Assert.True(ObjMonMagicAttackCore.TotalLinesAddUp());
        Assert.True(ObjMonMagicAttackCore.StartsAscending());
        Assert.True(ObjMonMagicAttackCore.MethodsContiguous());
        Assert.True(ObjMonMagicAttackCore.WithinUnit());
        Assert.True(ObjMonMagicAttackCore.NoInstrumentation());
        Assert.True(ObjMonMagicAttackCore.PhysicalSpanMatches());
        Assert.True(ObjMonMagicAttackCore.PhysicalInsideAttackTarget());
    }

    // ===================== 一、MagicAttackTarget 被掏空 =====================

    [Fact]
    public void MethodIsGutted()
    {
        // **修正后的核心断言：4 行非注释（含函数头）+ 46 行注释 = 50 行**
        Assert.True(ObjMonMagicAttackCore.FourLiveLinesOnly());
        Assert.True(ObjMonMagicAttackCore.OnlyOneExecutableLine());
        Assert.True(ObjMonMagicAttackCore.SkeletonPlusExecutionAddUp());
        Assert.True(ObjMonMagicAttackCore.FortySixCommentLines());
        Assert.True(ObjMonMagicAttackCore.AlwaysReturnsFalse());
        Assert.True(ObjMonMagicAttackCore.DoesNothing());
        Assert.True(ObjMonMagicAttackCore.CommentPlusLiveAddUp());
        Assert.True(ObjMonMagicAttackCore.CommentSpansMatch());
        Assert.True(ObjMonMagicAttackCore.ResultFalseBetweenComments());
        Assert.True(ObjMonMagicAttackCore.InnerLinesAddUp());

        // **4 + 46 = 50**
        Assert.Equal(4, ObjMonMagicAttackCore.LiveLines);
        Assert.Equal(46, ObjMonMagicAttackCore.CommentLines);
        Assert.Equal(
            ObjMonMagicAttackCore.MagicLines,
            ObjMonMagicAttackCore.LiveLines + ObjMonMagicAttackCore.CommentLines);
    }

    [Fact]
    public void VirtualStubFacts()
    {
        Assert.True(ObjMonMagicAttackCore.VirtualNotAbstract());
        Assert.True(ObjMonMagicAttackCore.DefaultStub());
        Assert.True(ObjMonMagicAttackCore.TwelveImplementations());
        Assert.True(ObjMonMagicAttackCore.SubclassesOverride());
        Assert.True(ObjMonMagicAttackCore.TwelveSubclasses());
        Assert.True(ObjMonMagicAttackCore.TableExtracted());
        Assert.True(ObjMonMagicAttackCore.UnlocksTwelveSubclasses());
        Assert.True(ObjMonMagicAttackCore.SubclassDeclsAscending());

        Assert.Equal(12, ObjMonMagicAttackCore.Subclasses.Length);
        Assert.Equal("TMon35_2Monster", ObjMonMagicAttackCore.Subclasses[0].Name);
        Assert.Equal(114, ObjMonMagicAttackCore.Subclasses[0].DeclLine);
        Assert.Equal("TExplosionAttackMonster", ObjMonMagicAttackCore.Subclasses[1].Name);
        Assert.Equal("TMLSBAttackMonster", ObjMonMagicAttackCore.Subclasses[3].Name);
        Assert.Equal("TFireSpiritMonster", ObjMonMagicAttackCore.Subclasses[11].Name);
    }

    [Fact]
    public void OldApiFacts()
    {
        Assert.True(ObjMonMagicAttackCore.OldApiSignatures());
        Assert.True(ObjMonMagicAttackCore.TwoArgGetMagStruckDamage());
        Assert.True(ObjMonMagicAttackCore.OneArgStruckDamage());
        Assert.True(ObjMonMagicAttackCore.ReboundWasProperty());
        Assert.True(ObjMonMagicAttackCore.NoModernPipeline());
        Assert.True(ObjMonMagicAttackCore.UninitializedMagicId());
        Assert.True(ObjMonMagicAttackCore.WouldSendGarbage());
    }

    [Fact]
    public void SameAxisTypoInComment()
    {
        Assert.True(ObjMonMagicAttackCore.SameAxisComparedTwice());
        Assert.True(ObjMonMagicAttackCore.SecondShouldBeY());
        Assert.True(ObjMonMagicAttackCore.DuplicatedInSibling());
        Assert.True(ObjMonMagicAttackCore.TwoSitesSameTypo());
        Assert.True(ObjMonMagicAttackCore.TypoLinesExtracted());
        Assert.True(ObjMonMagicAttackCore.TypoLinesEightApart());
        Assert.True(ObjMonMagicAttackCore.BuggyIgnoresY());
        Assert.True(ObjMonMagicAttackCore.VersionsDiffer());

        Assert.Equal(new[] { 4638, 4646 }, ObjMonMagicAttackCore.SameAxisTypoLines);

        // **笔误版忽略纵轴：纵轴超限时两版结论相反**
        Assert.True(ObjMonMagicAttackCore.BuggyAxisCheck(0, 99, 6));
        Assert.False(ObjMonMagicAttackCore.CorrectAxisCheck(0, 99, 6));

        // **两轴都在限内时两版一致**
        Assert.True(ObjMonMagicAttackCore.BuggyAxisCheck(3, 3, 6));
        Assert.True(ObjMonMagicAttackCore.CorrectAxisCheck(3, 3, 6));
    }

    [Fact]
    public void SentinelFacts()
    {
        Assert.True(ObjMonMagicAttackCore.SentinelMinusOne());
        Assert.True(ObjMonMagicAttackCore.RunSetsItTwice());
        Assert.True(ObjMonMagicAttackCore.ConsistentConvention());
        Assert.True(ObjMonMagicAttackCore.MinusOneMeansNone());
        Assert.True(ObjMonMagicAttackCore.OtherMeansSet());

        Assert.True(ObjMonMagicAttackCore.HasNoTargetPoint(-1));
        Assert.False(ObjMonMagicAttackCore.HasNoTargetPoint(0));
        Assert.False(ObjMonMagicAttackCore.HasNoTargetPoint(5));
    }

    // ===================== 二、AttackTarget 二选一 =====================

    [Fact]
    public void BranchFacts()
    {
        Assert.True(ObjMonMagicAttackCore.MagicBranchDelegates());
        Assert.True(ObjMonMagicAttackCore.PhysicalBranchIsFullBody());
        Assert.True(ObjMonMagicAttackCore.PhysicalMatchesOneAttack());
        Assert.True(ObjMonMagicAttackCore.SameAsJ202());
        Assert.True(ObjMonMagicAttackCore.AlwaysFalseForBase());
        Assert.True(ObjMonMagicAttackCore.NeverAttacksAsBase());
        Assert.True(ObjMonMagicAttackCore.SubclassMustOverride());
        Assert.True(ObjMonMagicAttackCore.FlagTrueGoesMagic());
        Assert.True(ObjMonMagicAttackCore.FlagFalseGoesPhysical());
        Assert.True(ObjMonMagicAttackCore.BaseResultIsFalse());

        Assert.Equal("magic", ObjMonMagicAttackCore.Branch(true));
        Assert.Equal("physical", ObjMonMagicAttackCore.Branch(false));
    }

    [Fact]
    public void MagicFlagCensus()
    {
        Assert.True(ObjMonMagicAttackCore.OnlyOneFalseAssign());
        Assert.True(ObjMonMagicAttackCore.OwnedByMlsb());
        Assert.True(ObjMonMagicAttackCore.PhysicalBranchForOneSubclassOnly());
        Assert.True(ObjMonMagicAttackCore.TwoOtherReaders());
        Assert.True(ObjMonMagicAttackCore.CrossClassFlag());
        Assert.True(ObjMonMagicAttackCore.FlagSitesExtracted());
        Assert.True(ObjMonMagicAttackCore.AssignsAndReadsAddUp());
        Assert.True(ObjMonMagicAttackCore.ExactlyOneFalseAssign());
        Assert.True(ObjMonMagicAttackCore.UnusedInMagicBranch());
        Assert.True(ObjMonMagicAttackCore.SharedDeclarationBlock());
        Assert.True(ObjMonMagicAttackCore.SameFamilyAsJ205());

        Assert.Equal(5, ObjMonMagicAttackCore.MagicFlagSites1.Length);
        Assert.Equal("assign", ObjMonMagicAttackCore.MagicFlagSites1[0].Kind);
        Assert.Equal(4601, ObjMonMagicAttackCore.MagicFlagSites1[0].Line);
        Assert.Equal("read", ObjMonMagicAttackCore.MagicFlagSites1[1].Kind);
        Assert.Equal(4662, ObjMonMagicAttackCore.MagicFlagSites1[1].Line);
        Assert.Equal(4947, ObjMonMagicAttackCore.MagicFlagSites1[2].Line);
        Assert.Equal("read", ObjMonMagicAttackCore.MagicFlagSites1[4].Kind);
        Assert.Equal(5748, ObjMonMagicAttackCore.MagicFlagSites1[4].Line);
    }

    // ===================== 三、Create 与 Run =====================

    [Fact]
    public void CreateFacts()
    {
        Assert.True(ObjMonMagicAttackCore.ViewRangeSeven());
        Assert.True(ObjMonMagicAttackCore.SevenSitesInFile());
        Assert.True(ObjMonMagicAttackCore.MostCommonValues());
        Assert.True(ObjMonMagicAttackCore.ViewRangeSitesExtracted());
        Assert.True(ObjMonMagicAttackCore.ViewRangeInsideCreate());

        Assert.Equal(7, ObjMonMagicAttackCore.ViewRangeSevenLines.Length);
        Assert.Equal(new[] { 2167, 2397, 2683, 2723, 4600, 5283, 8326 },
            ObjMonMagicAttackCore.ViewRangeSevenLines);
    }

    [Fact]
    public void RunGuardFacts()
    {
        Assert.True(ObjMonMagicAttackCore.FourFoldGuard());
        Assert.True(ObjMonMagicAttackCore.SameAs1402());
        Assert.True(ObjMonMagicAttackCore.Bo554IsSharedBaseField());
        Assert.True(ObjMonMagicAttackCore.ThreeDeclarationsNoOwner());
        Assert.True(ObjMonMagicAttackCore.Bo554SitesExtracted());
        Assert.True(ObjMonMagicAttackCore.AllTrueRuns());
        Assert.True(ObjMonMagicAttackCore.DeathBlocks());
        Assert.True(ObjMonMagicAttackCore.Bo554Blocks());
        Assert.True(ObjMonMagicAttackCore.GhostBlocks());
        Assert.True(ObjMonMagicAttackCore.CannotMoveBlocks());

        Assert.Equal(new[] { 11, 182, 507 }, ObjMonMagicAttackCore.Bo554DeclLines);
    }

    [Fact]
    public void SearchThrottleBoundaries()
    {
        Assert.True(ObjMonMagicAttackCore.TwoTierThrottle());
        Assert.True(ObjMonMagicAttackCore.EightSecondsWithTarget());
        Assert.True(ObjMonMagicAttackCore.OneSecondWithoutTarget());
        Assert.True(ObjMonMagicAttackCore.SameNumbersAsJ200());

        // **有目标：严格大于 8000**
        Assert.True(ObjMonMagicAttackCore.SearchAfterEightWithTarget());
        Assert.True(ObjMonMagicAttackCore.ExactlyEightBlocks());
        Assert.False(ObjMonMagicAttackCore.ShouldSearch(8000, true));

        // **无目标：严格大于 1000**
        Assert.True(ObjMonMagicAttackCore.SearchAfterOneWithoutTarget());
        Assert.True(ObjMonMagicAttackCore.ExactlyOneBlocks());
        Assert.True(ObjMonMagicAttackCore.UnderOneBlocks());

        // **有目标时只超 1 秒不够**
        Assert.True(ObjMonMagicAttackCore.OneSecondNotEnoughWithTarget());
    }

    [Fact]
    public void BackOffBandFacts()
    {
        Assert.True(ObjMonMagicAttackCore.TwoDistanceBands());
        Assert.True(ObjMonMagicAttackCore.CloserMeansMoreLikely());
        Assert.True(ObjMonMagicAttackCore.FiftyPercentVsTwentyPercent());
        Assert.True(ObjMonMagicAttackCore.NotGuaranteed());
        Assert.True(ObjMonMagicAttackCore.InverseDistanceProbability());
        Assert.True(ObjMonMagicAttackCore.RatioTwoPointFive());
        Assert.True(ObjMonMagicAttackCore.NotLinear());
        Assert.True(ObjMonMagicAttackCore.BandsExtracted());
        Assert.True(ObjMonMagicAttackCore.SquareNotCircular());
        Assert.True(ObjMonMagicAttackCore.ConcentricSquares());
        Assert.True(ObjMonMagicAttackCore.SameGeometryAsJ204());

        Assert.Equal(2, ObjMonMagicAttackCore.Bands.Length);
        Assert.Equal(2, ObjMonMagicAttackCore.Bands[0].Band);
        Assert.Equal(2, ObjMonMagicAttackCore.Bands[0].RandomBound);
        Assert.Equal(50, ObjMonMagicAttackCore.Bands[0].Percent);
        Assert.Equal(4, ObjMonMagicAttackCore.Bands[1].Band);
        Assert.Equal(5, ObjMonMagicAttackCore.Bands[1].RandomBound);
        Assert.Equal(20, ObjMonMagicAttackCore.Bands[1].Percent);
    }

    [Fact]
    public void BackOffBoundaries()
    {
        // **内档（<=2）：掷 0 后退、掷 1 不退**
        Assert.True(ObjMonMagicAttackCore.InnerRollZeroBacksOff());
        Assert.True(ObjMonMagicAttackCore.InnerRollOneStays());

        // **外档（3..4）：掷 0 后退、掷 4 不退**
        Assert.True(ObjMonMagicAttackCore.OuterRollZeroBacksOff());
        Assert.True(ObjMonMagicAttackCore.OuterRollFourStays());

        // **超出外档（>=5）一律不后退**
        Assert.True(ObjMonMagicAttackCore.BeyondOuterNeverBacksOff());

        // **边界：恰好 2 走内档、恰好 4 走外档**
        Assert.True(ObjMonMagicAttackCore.ShouldBackOff(2, 2, 0));
        Assert.True(ObjMonMagicAttackCore.ShouldBackOff(4, 4, 0));
        Assert.False(ObjMonMagicAttackCore.ShouldBackOff(4, 4, 1));
    }

    [Fact]
    public void RunStructureFacts()
    {
        Assert.True(ObjMonMagicAttackCore.ClearedAroundInherited());
        Assert.True(ObjMonMagicAttackCore.TwiceReset());
        Assert.True(ObjMonMagicAttackCore.DistrustOfBaseValue());
        Assert.True(ObjMonMagicAttackCore.EmptyCommentResidue());
        Assert.True(ObjMonMagicAttackCore.SameFamilyAsJ203());
        Assert.True(ObjMonMagicAttackCore.MixedWithinOneMethod());
        Assert.True(ObjMonMagicAttackCore.RawForSearch());
        Assert.True(ObjMonMagicAttackCore.TickDiffForWalk());
        Assert.True(ObjMonMagicAttackCore.SearchTimeUntouched());
        Assert.True(ObjMonMagicAttackCore.ContinuesJ200Finding());
        Assert.True(ObjMonMagicAttackCore.AlwaysCallsInherited());
        Assert.True(ObjMonMagicAttackCore.DoesNotReplaceMovement());
        Assert.True(ObjMonMagicAttackCore.SameShapeAsJ204());

        Assert.True(ObjMonMagicAttackCore.ClearBeforeLine
            < ObjMonMagicAttackCore.InheritedLine);
        Assert.True(ObjMonMagicAttackCore.ClearAfterLine
            > ObjMonMagicAttackCore.InheritedLine);
    }

    // ===================== 四、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonMagicAttackCore.NinetyTwoPercentComment());
        Assert.True(ObjMonMagicAttackCore.HighestRatioSoFar());
        Assert.True(ObjMonMagicAttackCore.FifthCommentStyle());
        Assert.True(ObjMonMagicAttackCore.CommentStylesExtracted());
        Assert.True(ObjMonMagicAttackCore.ElevenClassesCovered());
        Assert.True(ObjMonMagicAttackCore.RemainingApprox());

        Assert.Equal(5, ObjMonMagicAttackCore.CommentStyles.Length);
        Assert.StartsWith("// single-line", ObjMonMagicAttackCore.CommentStyles[0]);
        Assert.Contains("nested procedure", ObjMonMagicAttackCore.CommentStyles[3]);
    }
}
