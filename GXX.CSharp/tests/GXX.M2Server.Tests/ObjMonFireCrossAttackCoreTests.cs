using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J230：`ObjMon.pas` 中 `TFireCrossMonster` 的 `OneAttack` 与 `TwoAttack`
/// 的 1:1 测试（227 行）—— 补上 J212 那批的缺口。
/// **本批最有价值的发现**：
/// ① `GetRangeTargetCount`（25 行）整段是**死代码**（唯一调用点在花括号注释里）；
/// ② 7810-7811 正是 **J212 那条别名谱系相关性的现场**
///    （`WAbil := @m_WAbil` 紧跟**省掉 `Max(…, 1)`** 的 `GetAttackPower`）；
/// ③ `OneAttack` 是 J229 近身路径的逐字复制 + 6 行中毒插入，
///    且两个方法的尾巴 10 行逐字相同；
/// ④ **同一个循环里**同时出现 J209 普查里"互斥"的两种过滤写法。
/// </summary>
public sealed class ObjMonFireCrossAttackCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(7717, ObjMonFireCrossAttackCore.OneStart);
        Assert.Equal(7756, ObjMonFireCrossAttackCore.OneEnd);
        Assert.Equal(40, ObjMonFireCrossAttackCore.OneLines);
        Assert.Equal(7758, ObjMonFireCrossAttackCore.TwoStart);
        Assert.Equal(7944, ObjMonFireCrossAttackCore.TwoEnd);
        Assert.Equal(187, ObjMonFireCrossAttackCore.TwoLines);
        Assert.Equal(227, ObjMonFireCrossAttackCore.TotalLines);
        Assert.Equal(7760, ObjMonFireCrossAttackCore.RangeCountStart);
        Assert.Equal(7784, ObjMonFireCrossAttackCore.RangeCountEnd);
        Assert.Equal(25, ObjMonFireCrossAttackCore.RangeCountLines);
        Assert.Equal(12, ObjMonFireCrossAttackCore.TwoVarLines);
        Assert.Equal(7799, ObjMonFireCrossAttackCore.TwoBodyStart);
        Assert.Equal(7943, ObjMonFireCrossAttackCore.TwoBodyEnd);

        Assert.Equal(7814, ObjMonFireCrossAttackCore.DeadCallLine);
        Assert.Equal(2, ObjMonFireCrossAttackCore.RangeCountOccurrences);
        Assert.Equal(5, ObjMonFireCrossAttackCore.CommentedRadius);
        Assert.Equal(3, ObjMonFireCrossAttackCore.LiveRadius);
        Assert.Equal(2, ObjMonFireCrossAttackCore.TargetCountThreshold);

        Assert.Equal(7810, ObjMonFireCrossAttackCore.AliasLine);
        Assert.Equal(7811, ObjMonFireCrossAttackCore.NoMaxLine);
        Assert.Equal(7790, ObjMonFireCrossAttackCore.WAbilDeclLine);
        Assert.Equal(13, ObjMonFireCrossAttackCore.J212AliasCount);

        Assert.Equal(7602, ObjMonFireCrossAttackCore.J229MeleeStart);
        Assert.Equal(7613, ObjMonFireCrossAttackCore.J229MeleeEnd);
        Assert.Equal(12, ObjMonFireCrossAttackCore.J229MeleeLines);
        Assert.Equal(7724, ObjMonFireCrossAttackCore.SkeletonStart);
        Assert.Equal(7740, ObjMonFireCrossAttackCore.SkeletonEnd);
        Assert.Equal(17, ObjMonFireCrossAttackCore.SkeletonLines);
        Assert.Equal(6, ObjMonFireCrossAttackCore.PoisonInsertLines);
        Assert.Equal(7724, ObjMonFireCrossAttackCore.DirCheckLine);
        Assert.Equal(7726, ObjMonFireCrossAttackCore.CooldownLine);
        Assert.Equal(7728, ObjMonFireCrossAttackCore.HitTickLine);
        Assert.Equal(7729, ObjMonFireCrossAttackCore.HitDelayLine);
        Assert.Equal(7730, ObjMonFireCrossAttackCore.FocusTickLine);
        Assert.Equal(7731, ObjMonFireCrossAttackCore.BaseAttackLine);
        Assert.Equal(7732, ObjMonFireCrossAttackCore.PoisonStart);
        Assert.Equal(7737, ObjMonFireCrossAttackCore.PoisonEnd);
        Assert.Equal(7738, ObjMonFireCrossAttackCore.BreakSeizeLine);
        Assert.Equal(7740, ObjMonFireCrossAttackCore.ResultTrueLine);
        Assert.Equal(7719, ObjMonFireCrossAttackCore.Bt06DeclLine);

        Assert.Equal(7744, ObjMonFireCrossAttackCore.OneTailStart);
        Assert.Equal(7753, ObjMonFireCrossAttackCore.OneTailEnd);
        Assert.Equal(7932, ObjMonFireCrossAttackCore.TwoTailStart);
        Assert.Equal(7941, ObjMonFireCrossAttackCore.TwoTailEnd);
        Assert.Equal(10, ObjMonFireCrossAttackCore.TailLines);
        Assert.Equal(0, ObjMonFireCrossAttackCore.TailDiffLines);
        Assert.Equal(7746, ObjMonFireCrossAttackCore.SetTargetXYLine);
        Assert.Equal(7747, ObjMonFireCrossAttackCore.SetTargetXYAddrLine);
        Assert.Equal(7751, ObjMonFireCrossAttackCore.DelTargetLine);
        Assert.Equal(7752, ObjMonFireCrossAttackCore.DelTargetAddrLine);
        Assert.Equal(7934, ObjMonFireCrossAttackCore.TwoSetTargetXYLine);
        Assert.Equal(7939, ObjMonFireCrossAttackCore.TwoDelTargetLine);

        Assert.Equal(7814, ObjMonFireCrossAttackCore.FireGateLine);
        Assert.Equal(7816, ObjMonFireCrossAttackCore.DurationLine);
        Assert.Equal(6, ObjMonFireCrossAttackCore.DurationBound);
        Assert.Equal(3, ObjMonFireCrossAttackCore.DurationBase);
        Assert.Equal(3, ObjMonFireCrossAttackCore.DurationMin);
        Assert.Equal(8, ObjMonFireCrossAttackCore.DurationMax);
        Assert.Equal(7817, ObjMonFireCrossAttackCore.RateLine);
        Assert.Equal(100, ObjMonFireCrossAttackCore.RateDefault);
        Assert.Equal(1639, ObjMonFireCrossAttackCore.RateDeclLine);
        Assert.Equal(4477, ObjMonFireCrossAttackCore.RateDefaultLine);
        Assert.Equal(5, ObjMonFireCrossAttackCore.FireTiles);
        Assert.Equal(4, ObjMonFireCrossAttackCore.J219RingTiles);
        Assert.Equal(5, ObjMonFireCrossAttackCore.ET_FIRE);
        Assert.Equal(3120, ObjMonFireCrossAttackCore.ET_FIRE_Line);
        Assert.Equal(6, ObjMonFireCrossAttackCore.FireCtorArgs);
        Assert.Equal(7, ObjMonFireCrossAttackCore.FireCtorFullArgs);
        Assert.False(ObjMonFireCrossAttackCore.SeventhParamDefault);
        Assert.True(ObjMonFireCrossAttackCore.J219SeventhParam);
        Assert.Equal(7845, ObjMonFireCrossAttackCore.FireExitLine);
        Assert.Equal(7843, ObjMonFireCrossAttackCore.FireEffectLine);
        Assert.Equal(2, ObjMonFireCrossAttackCore.FireEffectId);
        Assert.Equal(7926, ObjMonFireCrossAttackCore.RangeEffectLine);
        Assert.Equal(1, ObjMonFireCrossAttackCore.RangeEffectId);
        Assert.Equal(7809, ObjMonFireCrossAttackCore.DirectionStoreLine);

        Assert.Equal(7847, ObjMonFireCrossAttackCore.ListCreateLine);
        Assert.Equal(7848, ObjMonFireCrossAttackCore.GetMapLine);
        Assert.Equal(7853, ObjMonFireCrossAttackCore.RejectFilterLine);
        Assert.Equal(7856, ObjMonFireCrossAttackCore.ConjunctFilterStart);
        Assert.Equal(7860, ObjMonFireCrossAttackCore.ConjunctFilterEnd);
        Assert.Equal(7850, ObjMonFireCrossAttackCore.LoopLine);
        Assert.Equal(7861, ObjMonFireCrossAttackCore.PerTargetPoisonStart);
        Assert.Equal(7866, ObjMonFireCrossAttackCore.PerTargetPoisonEnd);
        Assert.Equal("UnPosion", ObjMonFireCrossAttackCore.MisspelledProperty);
        Assert.Equal("UnPoison", ObjMonFireCrossAttackCore.CorrectProperty);
        Assert.Equal(0, ObjMonFireCrossAttackCore.POISON_DECHEALTH);
        Assert.Equal(20, ObjMonFireCrossAttackCore.PoisonPower);
        Assert.Equal(7924, ObjMonFireCrossAttackCore.ListFreeLine);
        Assert.Equal(7767, ObjMonFireCrossAttackCore.NestedListCreateLine);
        Assert.Equal(7783, ObjMonFireCrossAttackCore.NestedListFreeLine);
        Assert.Equal(7778, ObjMonFireCrossAttackCore.NestedDeleteLine);

        Assert.Equal(7946, ObjMonFireCrossAttackCore.AttackTargetLine);
        Assert.Equal(7948, ObjMonFireCrossAttackCore.AttackTargetRandomLine);
        Assert.Equal(4, ObjMonFireCrossAttackCore.AttackTargetRandomBound);
        Assert.Equal(4, ObjMonFireCrossAttackCore.ClassMethodCount);
        Assert.Equal(3, ObjMonFireCrossAttackCore.DoneMethodCount);
        Assert.Equal(32, ObjMonFireCrossAttackCore.ClassesCovered);
        Assert.Equal(22, ObjMonFireCrossAttackCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonFireCrossAttackCore.SpanMatches());
        Assert.True(ObjMonFireCrossAttackCore.TotalLinesAddUp());
        Assert.True(ObjMonFireCrossAttackCore.TwoDecompositionAddsUp());
        Assert.True(ObjMonFireCrossAttackCore.MethodsAscending());
        Assert.True(ObjMonFireCrossAttackCore.MethodsContiguous());
        Assert.True(ObjMonFireCrossAttackCore.WithinUnit());
        Assert.True(ObjMonFireCrossAttackCore.NoInstrumentation());
    }

    // ===================== 一、死代码 =====================

    [Fact]
    public void DeadCodeFacts()
    {
        Assert.True(ObjMonFireCrossAttackCore.GetRangeTargetCountIsDead());
        Assert.True(ObjMonFireCrossAttackCore.OnlyTwoOccurrences());
        Assert.True(ObjMonFireCrossAttackCore.OnlyCallSiteCommented());
        Assert.True(ObjMonFireCrossAttackCore.TwentyFiveLinesWasted());
        Assert.True(ObjMonFireCrossAttackCore.BraceCommentDisablesCondition());
        Assert.True(ObjMonFireCrossAttackCore.ThresholdTwoTargetsDropped());
        Assert.True(ObjMonFireCrossAttackCore.KillsTheFunctionToo());
        Assert.True(ObjMonFireCrossAttackCore.SecondKindOfBraceDisable());
    }

    [Fact]
    public void TwoRadiusFacts()
    {
        Assert.True(ObjMonFireCrossAttackCore.CommentedRadiusFive());
        Assert.True(ObjMonFireCrossAttackCore.LiveRadiusThree());
        Assert.True(ObjMonFireCrossAttackCore.TwoRadiiTwoPurposes());
        Assert.True(ObjMonFireCrossAttackCore.NotATypo());

        Assert.NotEqual(ObjMonFireCrossAttackCore.CommentedRadius,
            ObjMonFireCrossAttackCore.LiveRadius);
    }

    [Fact]
    public void FireGateBoundaries()
    {
        Assert.True(ObjMonFireCrossAttackCore.IgnoresTargetCount());
        Assert.True(ObjMonFireCrossAttackCore.MissedRollBlocks());

        // **条件被禁后，掷中就能放、与人数无关**
        Assert.True(ObjMonFireCrossAttackCore.FireGateFires(0, false));
        Assert.True(ObjMonFireCrossAttackCore.FireGateFires(0, true));
        Assert.False(ObjMonFireCrossAttackCore.FireGateFires(1, true));
        Assert.False(ObjMonFireCrossAttackCore.FireGateFires(2, true));
    }

    // ===================== 二、WAbil 谱系 =====================

    [Fact]
    public void AliasLineageFacts()
    {
        Assert.True(ObjMonFireCrossAttackCore.AliasThenNoMax());
        Assert.True(ObjMonFireCrossAttackCore.ConfirmsJ212Lineage());
        Assert.True(ObjMonFireCrossAttackCore.Site7810IsJ212sOwn());
        Assert.True(ObjMonFireCrossAttackCore.AliasTableContainsSite());
        Assert.True(ObjMonFireCrossAttackCore.AliasTableHasThirteen());
        Assert.True(ObjMonFireCrossAttackCore.OnlyInThisMethodWithinClass());
        Assert.True(ObjMonFireCrossAttackCore.RunnableConfirmation());
        Assert.True(ObjMonFireCrossAttackCore.PointerTypedAlias());
        Assert.True(ObjMonFireCrossAttackCore.DerefSyntax());
        Assert.True(ObjMonFireCrossAttackCore.SameValueDifferentForm());
        Assert.True(ObjMonFireCrossAttackCore.TwoChangesTogether());

        Assert.Equal(13, ObjMonFireCrossAttackCore.J212AliasLines.Length);
        Assert.Contains(7810, ObjMonFireCrossAttackCore.J212AliasLines);
    }

    [Fact]
    public void PowerFormulaBoundaries()
    {
        Assert.True(ObjMonFireCrossAttackCore.DifferWhenInverted());
        Assert.True(ObjMonFireCrossAttackCore.SameWhenNormal());
        Assert.True(ObjMonFireCrossAttackCore.NoMaxGivesSmaller());

        // **DC2 < DC1 时无 `Max` 版给出更小值**
        Assert.Equal(5, ObjMonFireCrossAttackCore.PowerNoMax(10, 5));
        Assert.Equal(11, ObjMonFireCrossAttackCore.PowerWithMax(10, 5));

        // **正常时相同**
        Assert.Equal(ObjMonFireCrossAttackCore.PowerNoMax(5, 10),
            ObjMonFireCrossAttackCore.PowerWithMax(5, 10));
    }

    // ===================== 三、OneAttack 的骨架 =====================

    [Fact]
    public void SkeletonCopyFacts()
    {
        Assert.True(ObjMonFireCrossAttackCore.OneAttackCopiesJ229Melee());
        Assert.True(ObjMonFireCrossAttackCore.TwelveVsSeventeen());
        Assert.True(ObjMonFireCrossAttackCore.SixLinePoisonInsert());
        Assert.True(ObjMonFireCrossAttackCore.SameFourCallsInSameOrder());
        Assert.True(ObjMonFireCrossAttackCore.TailBlockVerbatim());
        Assert.True(ObjMonFireCrossAttackCore.TenLinesZeroDiff());
        Assert.True(ObjMonFireCrossAttackCore.DuplicatedAcrossMethods());
    }

    [Fact]
    public void Bt06Facts()
    {
        Assert.True(ObjMonFireCrossAttackCore.Bt06FourthAppearance());
        Assert.True(ObjMonFireCrossAttackCore.SameAsJ229());
        Assert.True(ObjMonFireCrossAttackCore.DifferentFromJ216());
        Assert.True(ObjMonFireCrossAttackCore.Bt06LinesChecked());

        Assert.Equal(new[] { 7719, 7724, 7731 }, ObjMonFireCrossAttackCore.Bt06Lines);
    }

    [Fact]
    public void DirectionStoreFacts()
    {
        Assert.True(ObjMonFireCrossAttackCore.DirectionStoredHere());
        Assert.True(ObjMonFireCrossAttackCore.J229DidNotStore());
        Assert.True(ObjMonFireCrossAttackCore.ThirdDifferenceInSkeleton());
    }

    [Fact]
    public void AnnotationPairFacts()
    {
        Assert.True(ObjMonFireCrossAttackCore.BraceLabelPlusAddressComment());
        Assert.True(ObjMonFireCrossAttackCore.PairsOfAnnotations());
        Assert.True(ObjMonFireCrossAttackCore.DuplicatedLabelsToo());
        Assert.True(ObjMonFireCrossAttackCore.Shape36PairVariant());
        Assert.True(ObjMonFireCrossAttackCore.TailOffsetsMatch());
    }

    // ===================== 四、火墙段 =====================

    [Fact]
    public void FireWallFacts()
    {
        Assert.True(ObjMonFireCrossAttackCore.FiveTileFilledPlus());
        Assert.True(ObjMonFireCrossAttackCore.IncludesCentre());
        Assert.True(ObjMonFireCrossAttackCore.OrderUpLeftCentreRightDown());
        Assert.True(ObjMonFireCrossAttackCore.DiffersFromJ219RingByCentre());
        Assert.True(ObjMonFireCrossAttackCore.FireOffsetsDisjoint());
        Assert.True(ObjMonFireCrossAttackCore.FireTileLinesChecked());
        Assert.True(ObjMonFireCrossAttackCore.SixArgsOnly());
        Assert.True(ObjMonFireCrossAttackCore.SeventhParamOmitted());
        Assert.True(ObjMonFireCrossAttackCore.DefaultFalse());
        Assert.True(ObjMonFireCrossAttackCore.ContrastWithJ219True());
    }

    [Fact]
    public void FireTileTable()
    {
        Assert.Equal(5, ObjMonFireCrossAttackCore.FireOffsets.Length);
        Assert.Equal((0, -1), (ObjMonFireCrossAttackCore.FireOffsets[0].Dx,
            ObjMonFireCrossAttackCore.FireOffsets[0].Dy));
        Assert.Equal((-1, 0), (ObjMonFireCrossAttackCore.FireOffsets[1].Dx,
            ObjMonFireCrossAttackCore.FireOffsets[1].Dy));
        Assert.Equal((0, 0), (ObjMonFireCrossAttackCore.FireOffsets[2].Dx,
            ObjMonFireCrossAttackCore.FireOffsets[2].Dy));
        Assert.Equal((1, 0), (ObjMonFireCrossAttackCore.FireOffsets[3].Dx,
            ObjMonFireCrossAttackCore.FireOffsets[3].Dy));
        Assert.Equal((0, 1), (ObjMonFireCrossAttackCore.FireOffsets[4].Dx,
            ObjMonFireCrossAttackCore.FireOffsets[4].Dy));

        Assert.Equal(new[] { 7818, 7823, 7828, 7833, 7838 },
            ObjMonFireCrossAttackCore.FireTileLines);
    }

    [Fact]
    public void DurationAndRateFacts()
    {
        Assert.True(ObjMonFireCrossAttackCore.DurationThreeToEight());
        Assert.True(ObjMonFireCrossAttackCore.MinDuration());
        Assert.True(ObjMonFireCrossAttackCore.MaxDuration());
        Assert.True(ObjMonFireCrossAttackCore.MsConversion());
        Assert.True(ObjMonFireCrossAttackCore.RateConfigDefaultHundred());
        Assert.True(ObjMonFireCrossAttackCore.RateLinesChecked());
        Assert.True(ObjMonFireCrossAttackCore.NoPowerRoleSwap());
        Assert.True(ObjMonFireCrossAttackCore.DefaultRateNoChange());
        Assert.True(ObjMonFireCrossAttackCore.HalfRateHalves());

        Assert.Equal(3, ObjMonFireCrossAttackCore.Duration(0));
        Assert.Equal(8, ObjMonFireCrossAttackCore.Duration(5));
        Assert.Equal(3000, ObjMonFireCrossAttackCore.Duration(0) * 1000);
    }

    [Fact]
    public void TwoPathFacts()
    {
        Assert.True(ObjMonFireCrossAttackCore.FireWallExitsEarly());
        Assert.True(ObjMonFireCrossAttackCore.EffectTwoVersusOne());
        Assert.True(ObjMonFireCrossAttackCore.TwoExitPathsOneMethod());
        Assert.True(ObjMonFireCrossAttackCore.RollZeroFirewall());
        Assert.True(ObjMonFireCrossAttackCore.OtherwiseRange());
        Assert.True(ObjMonFireCrossAttackCore.EtFireIsFive());
        Assert.True(ObjMonFireCrossAttackCore.EtFireDeclChecked());

        Assert.Equal("firewall", ObjMonFireCrossAttackCore.PickPath(true));
        Assert.Equal("range", ObjMonFireCrossAttackCore.PickPath(false));
    }

    // ===================== 五、范围伤害段 =====================

    [Fact]
    public void BothFilterFormsFacts()
    {
        Assert.True(ObjMonFireCrossAttackCore.BothFilterFormsInOneLoop());
        Assert.True(ObjMonFireCrossAttackCore.RejectFormThenConjunctForm());
        Assert.True(ObjMonFireCrossAttackCore.RefutesJ209Dichotomy());
        Assert.True(ObjMonFireCrossAttackCore.SameLoopTwoStyles());
        Assert.True(ObjMonFireCrossAttackCore.BothCanFireTogether());
    }

    [Fact]
    public void FilterFormBoundaries()
    {
        // **拒绝式**
        Assert.True(ObjMonFireCrossAttackCore.RejectForm(true, false, true));
        Assert.False(ObjMonFireCrossAttackCore.RejectForm(false, false, true));
        Assert.True(ObjMonFireCrossAttackCore.RejectForm(false, false, false));

        // **合取式**
        Assert.True(ObjMonFireCrossAttackCore.ConjunctForm(true, true, true));
        Assert.False(ObjMonFireCrossAttackCore.ConjunctForm(false, true, true));
        Assert.False(ObjMonFireCrossAttackCore.ConjunctForm(true, true, false));
    }

    [Fact]
    public void FilteringStyleFacts()
    {
        Assert.True(ObjMonFireCrossAttackCore.ContinueNotDelete());
        Assert.True(ObjMonFireCrossAttackCore.VersusDeleteInDeadFunction());
        Assert.True(ObjMonFireCrossAttackCore.TwoFilteringStylesInOneMethod());
        Assert.True(ObjMonFireCrossAttackCore.UsedAsBooleanHere());
        Assert.True(ObjMonFireCrossAttackCore.StatementElsewhere());
        Assert.True(ObjMonFireCrossAttackCore.EmptyListSkipsBlock());
    }

    [Fact]
    public void ResourceFacts()
    {
        Assert.True(ObjMonFireCrossAttackCore.NoTryFinallyBoth());
        Assert.True(ObjMonFireCrossAttackCore.FreeOutsideIf());
        Assert.True(ObjMonFireCrossAttackCore.CounterExampleToTheRule());
    }

    [Fact]
    public void PerTargetPoisonFacts()
    {
        Assert.True(ObjMonFireCrossAttackCore.PoisonPerTarget());
        Assert.True(ObjMonFireCrossAttackCore.SameAsOneAttackWithSwap());
        Assert.True(ObjMonFireCrossAttackCore.SameAsJ212Config());
        Assert.True(ObjMonFireCrossAttackCore.PoisonApplyLinesChecked());
        Assert.True(ObjMonFireCrossAttackCore.MisspelledDiceProperty());
        Assert.True(ObjMonFireCrossAttackCore.ReadOnce());
        Assert.True(ObjMonFireCrossAttackCore.TwoIndependentRolls());
        Assert.True(ObjMonFireCrossAttackCore.UnguardedAntiPoisonRoll());

        Assert.Equal(new[] { 7736, 7865 }, ObjMonFireCrossAttackCore.PoisonApplyLines);
    }

    [Fact]
    public void PoisonBoundaries()
    {
        Assert.True(ObjMonFireCrossAttackCore.AllConditionsPoison());
        Assert.True(ObjMonFireCrossAttackCore.AlreadyPoisonedBlocks());
        Assert.True(ObjMonFireCrossAttackCore.ImmuneBlocks());
        Assert.True(ObjMonFireCrossAttackCore.ResistRollBlocks());
        Assert.True(ObjMonFireCrossAttackCore.ChanceBlocks());

        Assert.True(ObjMonFireCrossAttackCore.PoisonFires(false, false, 100, 0, 0));
        Assert.False(ObjMonFireCrossAttackCore.PoisonFires(true, false, 100, 0, 0));
        Assert.False(ObjMonFireCrossAttackCore.PoisonFires(false, true, 100, 0, 0));
        Assert.False(ObjMonFireCrossAttackCore.PoisonFires(false, false, 100, 1, 0));
        Assert.False(ObjMonFireCrossAttackCore.PoisonFires(false, false, 100, 0, 1));
    }

    // ===================== 六、其他与整体 =====================

    [Fact]
    public void NamingAndOverallFacts()
    {
        Assert.True(ObjMonFireCrossAttackCore.NHTimeIsFireDuration());
        Assert.True(ObjMonFireCrossAttackCore.UnrelatedToMnHitTime());
        Assert.True(ObjMonFireCrossAttackCore.ConfirmsJ212NamingNote());
        Assert.True(ObjMonFireCrossAttackCore.CompletesJ212Gap());
        Assert.True(ObjMonFireCrossAttackCore.ThreeOfFourDone());
        Assert.True(ObjMonFireCrossAttackCore.AttackTargetStillPending());
        Assert.True(ObjMonFireCrossAttackCore.RandomFourSeenAt7948());
        Assert.True(ObjMonFireCrossAttackCore.ThirtyTwoClassesCovered());
        Assert.True(ObjMonFireCrossAttackCore.RemainingApprox());
    }
}
