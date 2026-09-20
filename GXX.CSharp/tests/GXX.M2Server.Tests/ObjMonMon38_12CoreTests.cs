using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J235：`ObjMon.pas` 中 `TMon38_12Monster` 三个方法的 1:1 测试（232 行）。
/// **本批最有价值的发现**：
/// ① 外观 `342` 决定的是**整套动作集**（2 路 vs 4 路）—— 本系列外观特判里改动幅度最大的一次，
///    且 `342` 全文件只此一见；
/// ② **8708 是 J212 那 13 处别名谱系里的第二处**，
///    且**同一个类的另一个方法用相反写法**（直接访问 `m_wAbel` 且带 `Max`）——
///    在类内部正反相对；
/// ③ `nType` 一个数字**身兼三职**：发送延迟（2→2000ms）、是否麻痹（只有 1 号）、特效编号；
/// ④ 一段 `{ }` 一次关掉**两个**麻痹条件、使麻痹从两道门降到一道门。
/// </summary>
public sealed class ObjMonMon38_12CoreTests
{
    [Fact]
    public void Constants()
    {
        Assert.Equal(8632, ObjMonMon38_12Core.AttackStart);
        Assert.Equal(8695, ObjMonMon38_12Core.AttackEnd);
        Assert.Equal(64, ObjMonMon38_12Core.AttackLines);
        Assert.Equal(8698, ObjMonMon38_12Core.Attack0Start);
        Assert.Equal(8778, ObjMonMon38_12Core.Attack0End);
        Assert.Equal(81, ObjMonMon38_12Core.Attack0Lines);
        Assert.Equal(8779, ObjMonMon38_12Core.GroupStart);
        Assert.Equal(8865, ObjMonMon38_12Core.GroupEnd);
        Assert.Equal(87, ObjMonMon38_12Core.GroupLines);
        Assert.Equal(232, ObjMonMon38_12Core.TotalLines);
        Assert.Equal(3, ObjMonMon38_12Core.MethodCount);

        Assert.Equal(8634, ObjMonMon38_12Core.DirDeclLine);
        Assert.Equal(8639, ObjMonMon38_12Core.DirCheckLine);
        Assert.Equal(8641, ObjMonMon38_12Core.CooldownLine);
        Assert.Equal(8643, ObjMonMon38_12Core.HitTickLine);
        Assert.Equal(8644, ObjMonMon38_12Core.HitDelayLine);
        Assert.Equal(8645, ObjMonMon38_12Core.FocusTickLine);
        Assert.Equal(8646, ObjMonMon38_12Core.ApprGateLine);
        Assert.Equal(342, ObjMonMon38_12Core.ApprValue);
        Assert.Equal(8648, ObjMonMon38_12Core.ApprRollLine);
        Assert.Equal(8650, ObjMonMon38_12Core.ApprGroupCallLine);
        Assert.Equal(8655, ObjMonMon38_12Core.ApprMeleeLine);
        Assert.Equal(8659, ObjMonMon38_12Core.PhysicalCommentLine);
        Assert.Equal(8660, ObjMonMon38_12Core.PhysicalGateLine);
        Assert.Equal(8662, ObjMonMon38_12Core.PhysicalCallLine);
        Assert.Equal(8664, ObjMonMon38_12Core.Magic1CommentLine);
        Assert.Equal(8665, ObjMonMon38_12Core.Magic1GateLine);
        Assert.Equal(8667, ObjMonMon38_12Core.Magic1CallLine);
        Assert.Equal(8669, ObjMonMon38_12Core.Magic2CommentLine);
        Assert.Equal(8670, ObjMonMon38_12Core.Magic2GateLine);
        Assert.Equal(8672, ObjMonMon38_12Core.Magic2CallLine);
        Assert.Equal(8677, ObjMonMon38_12Core.ElseMeleeLine);
        Assert.Equal(8678, ObjMonMon38_12Core.ElseBreakSeizeLine);
        Assert.Equal(8681, ObjMonMon38_12Core.ResultTrueLine);

        Assert.Equal(8708, ObjMonMon38_12Core.AliasLine);
        Assert.Equal(8709, ObjMonMon38_12Core.NoMaxLine);
        Assert.Equal(7810, ObjMonMon38_12Core.J230AliasSite);
        Assert.Equal(8708, ObjMonMon38_12Core.ThisAliasSite);
        Assert.Equal(8712, ObjMonMon38_12Core.ListCreateLine);
        Assert.Equal(8713, ObjMonMon38_12Core.TryLine);
        Assert.Equal(8714, ObjMonMon38_12Core.GetMapLine);
        Assert.Equal(4, ObjMonMon38_12Core.Radius);
        Assert.Equal(8718, ObjMonMon38_12Core.RejectFilterLine);
        Assert.Equal(8721, ObjMonMon38_12Core.ConjunctFilterStart);
        Assert.Equal(8725, ObjMonMon38_12Core.ConjunctFilterEnd);
        Assert.Equal(8731, ObjMonMon38_12Core.RateAddLine);
        Assert.Equal(8733, ObjMonMon38_12Core.NextDamageLine);
        Assert.Equal(8735, ObjMonMon38_12Core.PowerMaxLine);
        Assert.Equal(8760, ObjMonMon38_12Core.PositiveGuardLine);
        Assert.Equal(8773, ObjMonMon38_12Core.EffectLine);
        Assert.Equal(0, ObjMonMon38_12Core.EffectId);
        Assert.Equal(8774, ObjMonMon38_12Core.FinallyLine);
        Assert.Equal(8775, ObjMonMon38_12Core.FreeLine);
        Assert.Equal(7853, ObjMonMon38_12Core.J230RejectLine);
        Assert.Equal(8570, ObjMonMon38_12Core.J234RejectLine);

        Assert.Equal(8787, ObjMonMon38_12Core.GroupListLine);
        Assert.Equal(8788, ObjMonMon38_12Core.GroupTryLine);
        Assert.Equal(8789, ObjMonMon38_12Core.SelfRageLine);
        Assert.Equal(8790, ObjMonMon38_12Core.SelfGetMapLine);
        Assert.Equal(8792, ObjMonMon38_12Core.TargetGetMapLine);
        Assert.Equal(8793, ObjMonMon38_12Core.WithMaxLine);
        Assert.Equal(8799, ObjMonMon38_12Core.GroupRejectLine);
        Assert.Equal(8806, ObjMonMon38_12Core.GroupRateAddLine);
        Assert.Equal(8808, ObjMonMon38_12Core.MultipleLine);
        Assert.Equal(8809, ObjMonMon38_12Core.GroupNextDamageLine);
        Assert.Equal(8811, ObjMonMon38_12Core.GroupPowerMaxLine);
        Assert.Equal(8836, ObjMonMon38_12Core.GroupGuardLine);
        Assert.Equal(8839, ObjMonMon38_12Core.DelayBranch1Line);
        Assert.Equal(2000, ObjMonMon38_12Core.LongDelay);
        Assert.Equal(200, ObjMonMon38_12Core.ShortDelay);
        Assert.Equal(8845, ObjMonMon38_12Core.ParalysisLine);
        Assert.Equal(8846, ObjMonMon38_12Core.BraceDisabledLine);
        Assert.Equal(8848, ObjMonMon38_12Core.MakePosionLine);
        Assert.Equal(3, ObjMonMon38_12Core.ParalysisDuration);
        Assert.Equal(5, ObjMonMon38_12Core.POISON_STONE);
        Assert.Equal(8854, ObjMonMon38_12Core.DelayBranch2Line);
        Assert.Equal(8861, ObjMonMon38_12Core.GroupEffectLine);
        Assert.Equal(8862, ObjMonMon38_12Core.GroupFinallyLine);
        Assert.Equal(8863, ObjMonMon38_12Core.GroupFreeLine);
        Assert.Equal("UnParalysis", ObjMonMon38_12Core.LivePropertyName);
        Assert.Equal("m_boUnParalysis", ObjMonMon38_12Core.LegacyFieldName);

        Assert.Equal(90, ObjMonMon38_12Core.ClassDeclLine);
        Assert.Equal(92, ObjMonMon38_12Core.GroupDeclLine);
        Assert.Equal(93, ObjMonMon38_12Core.Attack0DeclLine);
        Assert.Equal(95, ObjMonMon38_12Core.AttackDeclLine);
        Assert.Equal(116, ObjMonMon38_12Core.RemainingMemberLine);
        Assert.Equal(8868, ObjMonMon38_12Core.NextImplLine);
        Assert.Equal(39, ObjMonMon38_12Core.ClassesCovered);
        Assert.Equal(15, ObjMonMon38_12Core.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonMon38_12Core.SpanMatches());
        Assert.True(ObjMonMon38_12Core.TotalLinesAddUp());
        Assert.True(ObjMonMon38_12Core.MethodsAscending());
        Assert.True(ObjMonMon38_12Core.MethodsContiguous());
        Assert.True(ObjMonMon38_12Core.WithinUnit());
        Assert.True(ObjMonMon38_12Core.NoInstrumentation());
        Assert.True(ObjMonMon38_12Core.CooldownOrder());
    }

    // ===================== 一、外观换整套动作集 =====================

    [Fact]
    public void AppearanceRepertoireFacts()
    {
        Assert.True(ObjMonMon38_12Core.FiveWayCascade());
        Assert.True(ObjMonMon38_12Core.AppearanceSelectsRepertoire());
        Assert.True(ObjMonMon38_12Core.TwoVsFourOutcomes());
        Assert.True(ObjMonMon38_12Core.LargestAppearanceDivergenceSoFar());
        Assert.True(ObjMonMon38_12Core.Appr342TwoOutcomes());
        Assert.True(ObjMonMon38_12Core.ElseFourOutcomes());
        Assert.True(ObjMonMon38_12Core.RepertoiresDiffer());
        Assert.True(ObjMonMon38_12Core.Appr342HasNoPhysicalGroup());
        Assert.True(ObjMonMon38_12Core.Appr342HasNoMagic1Or2());
        Assert.True(ObjMonMon38_12Core.Appr342UsesTypeThree());
        Assert.True(ObjMonMon38_12Core.BothHaveMelee());
    }

    [Fact]
    public void RepertoireTables()
    {
        Assert.Equal(new[] { "magic-group-3", "melee" },
            ObjMonMon38_12Core.AppearanceOutcomes(true));
        Assert.Equal(new[] { "physical-group", "magic-group-1", "magic-group-2", "melee" },
            ObjMonMon38_12Core.AppearanceOutcomes(false));
    }

    [Fact]
    public void AppearanceCensusFacts()
    {
        Assert.True(ObjMonMon38_12Core.Appearance342SingleUse());
        Assert.True(ObjMonMon38_12Core.PositiveUse());
        Assert.True(ObjMonMon38_12Core.ContrastWithJ234Negative());
        Assert.True(ObjMonMon38_12Core.FourAppearanceValuesTable());
        Assert.True(ObjMonMon38_12Core.MagnitudeVariesWidely());
        Assert.True(ObjMonMon38_12Core.FourDistinctValues());
        Assert.True(ObjMonMon38_12Core.OneNegativeThreePositive());
        Assert.True(ObjMonMon38_12Core.FourthAppearanceSpecialCase());

        Assert.Equal(4, ObjMonMon38_12Core.AppearanceTable.Length);
        Assert.Equal(231, ObjMonMon38_12Core.AppearanceTable[0].Value);
        Assert.Equal(607, ObjMonMon38_12Core.AppearanceTable[1].Value);
        Assert.Equal(640, ObjMonMon38_12Core.AppearanceTable[2].Value);
        Assert.Equal(342, ObjMonMon38_12Core.AppearanceTable[3].Value);
    }

    [Fact]
    public void CommentPlacementFacts()
    {
        Assert.True(ObjMonMon38_12Core.CommentBeforeElseIf());
        Assert.True(ObjMonMon38_12Core.ThreeConsistentPlacements());
        Assert.True(ObjMonMon38_12Core.HabitNotTypo());
        Assert.True(ObjMonMon38_12Core.CommentLinesChecked());
        Assert.True(ObjMonMon38_12Core.EachCommentPrecedesItsGate());

        Assert.Equal(new[] { 8659, 8664, 8669 }, ObjMonMon38_12Core.CommentLines);
    }

    [Fact]
    public void ProbabilityFacts()
    {
        Assert.True(ObjMonMon38_12Core.SequentialProbabilities());
        Assert.True(ObjMonMon38_12Core.FourOutcomeProbabilities());
        Assert.True(ObjMonMon38_12Core.MultiplyAndSubtract());
        Assert.True(ObjMonMon38_12Core.SumIsOne());
        Assert.True(ObjMonMon38_12Core.PhysicalGroupIsLikeliest());

        Assert.Equal(new[] { 3, 5, 3 }, ObjMonMon38_12Core.CascadeBounds);
    }

    [Fact]
    public void ProbabilityValues()
    {
        Assert.True(Math.Abs(ObjMonMon38_12Core.PhysicalProbability() - 1.0 / 3) < 1e-9);
        Assert.True(Math.Abs(ObjMonMon38_12Core.Magic1Probability() - 2.0 / 15) < 1e-9);
        Assert.True(Math.Abs(ObjMonMon38_12Core.Magic2Probability() - 8.0 / 45) < 1e-9);
        Assert.True(Math.Abs(ObjMonMon38_12Core.MeleeProbability() - 16.0 / 45) < 1e-9);
    }

    // ===================== 二、别名谱系的第二个现场 =====================

    [Fact]
    public void AliasLineageFacts()
    {
        Assert.True(ObjMonMon38_12Core.AliasThenNoMaxAgain());
        Assert.True(ObjMonMon38_12Core.Site8708IsJ212sOwn());
        Assert.True(ObjMonMon38_12Core.SecondRunnableSite());
        Assert.True(ObjMonMon38_12Core.TwoOfThirteenPorted());
        Assert.True(ObjMonMon38_12Core.AliasTableHasThirteen());
        Assert.True(ObjMonMon38_12Core.TwoSitesDistinctAndPresent());

        Assert.Equal(13, ObjMonMon38_12Core.J212AliasLines.Length);
        Assert.Contains(7810, ObjMonMon38_12Core.J212AliasLines);
        Assert.Contains(8708, ObjMonMon38_12Core.J212AliasLines);
    }

    [Fact]
    public void InClassContrastFacts()
    {
        Assert.True(ObjMonMon38_12Core.OppositeFormsInSameClass());
        Assert.True(ObjMonMon38_12Core.AliasWithoutMaxVersusDirectWithMax());
        Assert.True(ObjMonMon38_12Core.InClassConfirmation());
        Assert.True(ObjMonMon38_12Core.StrongerThanJ230());
    }

    [Fact]
    public void PowerBoundaries()
    {
        Assert.True(ObjMonMon38_12Core.DifferWhenInverted());
        Assert.True(ObjMonMon38_12Core.SameWhenNormal());

        Assert.Equal(5, ObjMonMon38_12Core.PowerNoMax(10, 5));
        Assert.Equal(11, ObjMonMon38_12Core.PowerWithMax(10, 5));
        Assert.Equal(ObjMonMon38_12Core.PowerNoMax(5, 10),
            ObjMonMon38_12Core.PowerWithMax(5, 10));
    }

    [Fact]
    public void RadiusAndCenterFacts()
    {
        Assert.True(ObjMonMon38_12Core.HardcodedFourSelfCentered());
        Assert.True(ObjMonMon38_12Core.FourRadiusSources());
        Assert.True(ObjMonMon38_12Core.ThreeCenterKinds());
        Assert.True(ObjMonMon38_12Core.TableGrewAgain());

        Assert.Equal(new[] { "config", "hardcoded", "field", "parameter" },
            ObjMonMon38_12Core.RadiusSources());
        Assert.Equal(new[] { "self", "target", "parameter" },
            ObjMonMon38_12Core.CenterKinds());
    }

    [Fact]
    public void FilterFormFacts()
    {
        Assert.True(ObjMonMon38_12Core.BothFilterFormsThirdTime());
        Assert.True(ObjMonMon38_12Core.SameLiteralsAsJ230J234());
        Assert.True(ObjMonMon38_12Core.ThirdDisproof());
        Assert.True(ObjMonMon38_12Core.GroupOnlyHasRejectForm());
        Assert.True(ObjMonMon38_12Core.InconsistentEvenWithinOneClass());
        Assert.True(ObjMonMon38_12Core.StrongerThanCoexistence());
        Assert.True(ObjMonMon38_12Core.BothCanFireTogether());
    }

    [Fact]
    public void FilterBoundaries()
    {
        Assert.True(ObjMonMon38_12Core.RejectForm(true, false, true));
        Assert.False(ObjMonMon38_12Core.RejectForm(false, false, true));
        Assert.True(ObjMonMon38_12Core.ConjunctForm(true, true, true));
        Assert.False(ObjMonMon38_12Core.ConjunctForm(true, true, false));
    }

    [Fact]
    public void Attack0MiscFacts()
    {
        Assert.True(ObjMonMon38_12Core.FiveStepPipeline());
        Assert.True(ObjMonMon38_12Core.EffectZero());
        Assert.True(ObjMonMon38_12Core.HasTryFinally());
        Assert.True(ObjMonMon38_12Core.FreeInFinally());
    }

    // ===================== 三、nType 身兼三职 =====================

    [Fact]
    public void NTypeTripleDutyFacts()
    {
        Assert.True(ObjMonMon38_12Core.NTypeTripleDuty());
        Assert.True(ObjMonMon38_12Core.DelayChosenByType());
        Assert.True(ObjMonMon38_12Core.ParalysisOnlyForTypeOne());
        Assert.True(ObjMonMon38_12Core.EffectEqualsType());
        Assert.True(ObjMonMon38_12Core.OneNumberThreeLayers());
        Assert.True(ObjMonMon38_12Core.ThreeCallSiteSemantics());
        Assert.True(ObjMonMon38_12Core.ThreeDistinctTypes());
    }

    [Fact]
    public void CallSiteArgs()
    {
        Assert.Equal(new[] { 3, 1, 3 }, ObjMonMon38_12Core.ApprGroupArgs);
        Assert.Equal(new[] { 3, 1, 1 }, ObjMonMon38_12Core.Magic1Args);
        Assert.Equal(new[] { 6, 2, 2 }, ObjMonMon38_12Core.Magic2Args);
    }

    [Fact]
    public void DelayBoundaries()
    {
        Assert.True(ObjMonMon38_12Core.TypeTwoLongDelay());
        Assert.True(ObjMonMon38_12Core.TypeOneAndThreeShort());
        Assert.True(ObjMonMon38_12Core.LongIsTenTimesShort());

        Assert.Equal(2000, ObjMonMon38_12Core.DelayFor(2));
        Assert.Equal(200, ObjMonMon38_12Core.DelayFor(1));
        Assert.Equal(200, ObjMonMon38_12Core.DelayFor(3));
    }

    [Fact]
    public void ParalysisBoundaries()
    {
        Assert.True(ObjMonMon38_12Core.TypeOneUnresisted());
        Assert.True(ObjMonMon38_12Core.TypeTwoNoParalysis());
        Assert.True(ObjMonMon38_12Core.TypeThreeNoParalysis());
        Assert.True(ObjMonMon38_12Core.ResistedBlocks());

        Assert.True(ObjMonMon38_12Core.ParalysisFires(1, false));
        Assert.False(ObjMonMon38_12Core.ParalysisFires(1, true));
        Assert.False(ObjMonMon38_12Core.ParalysisFires(2, false));
        Assert.False(ObjMonMon38_12Core.ParalysisFires(3, false));
    }

    [Fact]
    public void EffectBoundaries()
    {
        Assert.Equal(1, ObjMonMon38_12Core.EffectFor(1));
        Assert.Equal(2, ObjMonMon38_12Core.EffectFor(2));
        Assert.Equal(3, ObjMonMon38_12Core.EffectFor(3));
    }

    [Fact]
    public void MultipleFacts()
    {
        Assert.True(ObjMonMon38_12Core.MultipleIsParameter());
        Assert.True(ObjMonMon38_12Core.GeneralizesJ234LiteralTwo());
        Assert.True(ObjMonMon38_12Core.OnlyTypeTwoIsDoubled());
        Assert.True(ObjMonMon38_12Core.TypeTwoIsTheHeavierOne());
        Assert.True(ObjMonMon38_12Core.MultipleTwoDoubles());
        Assert.True(ObjMonMon38_12Core.MultipleOneKeeps());
        Assert.True(ObjMonMon38_12Core.TypeTwoHasLargerRadius());

        Assert.Equal(200, ObjMonMon38_12Core.ApplyMultiple(100, 2));
        Assert.Equal(100, ObjMonMon38_12Core.ApplyMultiple(100, 1));
    }

    [Fact]
    public void PipelineVariantFacts()
    {
        Assert.True(ObjMonMon38_12Core.FourthPipelineVariant());
        Assert.True(ObjMonMon38_12Core.HasRateAddAndMultiplier());
        Assert.True(ObjMonMon38_12Core.VersusJ230AndJ234());
        Assert.True(ObjMonMon38_12Core.MultipleAfterRateAdd());
        Assert.True(ObjMonMon38_12Core.MultipleBeforeCap());
    }

    [Fact]
    public void PipelineArithmetic()
    {
        Assert.Equal(200, ObjMonMon38_12Core.FinalDamage(100, true, 2));
        Assert.Equal(100, ObjMonMon38_12Core.FinalDamage(100, true, 1));
        Assert.Equal(200, ObjMonMon38_12Core.FinalDamage(100, false, 2));
    }

    [Fact]
    public void BraceDisableFacts()
    {
        Assert.True(ObjMonMon38_12Core.BraceCommentDisablesTwoConditions());
        Assert.True(ObjMonMon38_12Core.ParalysisDowngradedToOneGate());
        Assert.True(ObjMonMon38_12Core.FixedDurationThree());
        Assert.True(ObjMonMon38_12Core.UsesPropertyNotLegacyField());
        Assert.True(ObjMonMon38_12Core.ContrastWithJ232J233());
        Assert.True(ObjMonMon38_12Core.PropertyNameIsTheNewOne());
        Assert.True(ObjMonMon38_12Core.LegacyNameChecked());
        Assert.True(ObjMonMon38_12Core.SlotIsFive());
    }

    [Fact]
    public void GroupMiscFacts()
    {
        Assert.True(ObjMonMon38_12Core.TrailingSemicolonOmitted());
        Assert.True(ObjMonMon38_12Core.SameAsJ234());
        Assert.True(ObjMonMon38_12Core.NoSecondFilter());
        Assert.True(ObjMonMon38_12Core.BothHaveTryFinally());
        Assert.True(ObjMonMon38_12Core.CenterFromParameter());
    }

    // ===================== 四、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonMon38_12Core.Mon38_12Closed());
        Assert.True(ObjMonMon38_12Core.CompletesLine92Member());
        Assert.True(ObjMonMon38_12Core.ThreeOfFourMembersPorted());
        Assert.True(ObjMonMon38_12Core.Only116Remains());
        Assert.True(ObjMonMon38_12Core.FamilyDeclLinesChecked());
        Assert.True(ObjMonMon38_12Core.NextClassIsMon35_2());
        Assert.True(ObjMonMon38_12Core.WouldCompleteTheFamily());
        Assert.True(ObjMonMon38_12Core.BaseIsTATMonster());
        Assert.True(ObjMonMon38_12Core.TwoPrivateMethods());
        Assert.True(ObjMonMon38_12Core.DeclLinesChecked());
        Assert.True(ObjMonMon38_12Core.ThirtyNineClassesCovered());
        Assert.True(ObjMonMon38_12Core.RemainingApprox());

        Assert.Equal(new[] { 54, 92, 100, 116 }, ObjMonMon38_12Core.FamilyDeclLines);
        Assert.Equal(new[] { 54, 100, 92 }, ObjMonMon38_12Core.PortedMembers);
    }
}
