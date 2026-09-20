using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J218：`ObjMon.pas` 中**两个姊妹类**的 1:1 测试（合计 225 行）：
/// `TDamageSpellAttackMonster`（吸蓝，109+4）与
/// `TDamageArmorAttackMonster`（减防御，108+4）。
/// **本批最有价值的发现**：两类逐字相同 106 行、各只差 3 行与 2 行，
/// 而那个唯一的差异揭示了一个**名不副实** ——
/// "吸蓝"类比"减防御"类**多做了一件范围减防**，
/// 且两个类用的是**两套不同的减防机制**。
/// </summary>
public sealed class ObjMonDamageSpellArmorCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(6145, ObjMonDamageSpellArmorCore.SpellStart);
        Assert.Equal(6253, ObjMonDamageSpellArmorCore.SpellEnd);
        Assert.Equal(109, ObjMonDamageSpellArmorCore.SpellLines);
        Assert.Equal(6255, ObjMonDamageSpellArmorCore.SpellRunStart);
        Assert.Equal(6258, ObjMonDamageSpellArmorCore.SpellRunEnd);
        Assert.Equal(6261, ObjMonDamageSpellArmorCore.ArmorStart);
        Assert.Equal(6368, ObjMonDamageSpellArmorCore.ArmorEnd);
        Assert.Equal(108, ObjMonDamageSpellArmorCore.ArmorLines);
        Assert.Equal(6370, ObjMonDamageSpellArmorCore.ArmorRunStart);
        Assert.Equal(6373, ObjMonDamageSpellArmorCore.ArmorRunEnd);
        Assert.Equal(4, ObjMonDamageSpellArmorCore.RunLines);
        Assert.Equal(225, ObjMonDamageSpellArmorCore.TotalLines);

        Assert.Equal(6146, ObjMonDamageSpellArmorCore.SpellHeadStart);
        Assert.Equal(6202, ObjMonDamageSpellArmorCore.SpellHeadEnd);
        Assert.Equal(6262, ObjMonDamageSpellArmorCore.ArmorHeadStart);
        Assert.Equal(6318, ObjMonDamageSpellArmorCore.ArmorHeadEnd);
        Assert.Equal(57, ObjMonDamageSpellArmorCore.HeadLines);
        Assert.Equal(0, ObjMonDamageSpellArmorCore.HeadDiffLines);
        Assert.Equal(6209, ObjMonDamageSpellArmorCore.SpellTailStart);
        Assert.Equal(6253, ObjMonDamageSpellArmorCore.SpellTailEnd);
        Assert.Equal(6324, ObjMonDamageSpellArmorCore.ArmorTailStart);
        Assert.Equal(6368, ObjMonDamageSpellArmorCore.ArmorTailEnd);
        Assert.Equal(45, ObjMonDamageSpellArmorCore.TailLines);
        Assert.Equal(0, ObjMonDamageSpellArmorCore.TailDiffLines);
        Assert.Equal(106, ObjMonDamageSpellArmorCore.IdenticalLines);
        Assert.Equal(4, ObjMonDamageSpellArmorCore.SharedEffectLines);
        Assert.Equal(3, ObjMonDamageSpellArmorCore.SpellDifferingLines);
        Assert.Equal(2, ObjMonDamageSpellArmorCore.ArmorDifferingLines);
        Assert.Equal(116, ObjMonDamageSpellArmorCore.HeadOffset);
        Assert.Equal(115, ObjMonDamageSpellArmorCore.TailOffset);

        Assert.Equal(6203, ObjMonDamageSpellArmorCore.SpellEffectStart);
        Assert.Equal(6208, ObjMonDamageSpellArmorCore.SpellEffectEnd);
        Assert.Equal(6, ObjMonDamageSpellArmorCore.SpellEffectLines);
        Assert.Equal(6319, ObjMonDamageSpellArmorCore.ArmorEffectStart);
        Assert.Equal(6323, ObjMonDamageSpellArmorCore.ArmorEffectEnd);
        Assert.Equal(5, ObjMonDamageSpellArmorCore.ArmorEffectLines);
        Assert.Equal(3, ObjMonDamageSpellArmorCore.EffectRollBound);

        Assert.Equal(6205, ObjMonDamageSpellArmorCore.DamageSpellLine);
        Assert.Equal(6207, ObjMonDamageSpellArmorCore.AreaDownLine);
        Assert.Equal(6321, ObjMonDamageSpellArmorCore.ZeroArmorLine);
        Assert.Equal(571, ObjMonDamageSpellArmorCore.DamageSpellDeclLine);
        Assert.Equal(28119, ObjMonDamageSpellArmorCore.DamageSpellImplLine);
        Assert.Equal(572, ObjMonDamageSpellArmorCore.ZeroArmorDeclLine);
        Assert.Equal(28110, ObjMonDamageSpellArmorCore.ZeroArmorImplLine);
        Assert.Equal(754, ObjMonDamageSpellArmorCore.AreaDownDeclLine);
        Assert.Equal(40995, ObjMonDamageSpellArmorCore.AreaDownImplLine);
        Assert.Equal(5149, ObjMonDamageSpellArmorCore.SkillAreaDownLine);
        Assert.Equal(2, ObjMonDamageSpellArmorCore.DamageSpellSites);
        Assert.Equal(5098, ObjMonDamageSpellArmorCore.OtherDamageSpellLine);
        Assert.Equal(1, ObjMonDamageSpellArmorCore.ZeroArmorSites);
        Assert.Equal(1, ObjMonDamageSpellArmorCore.ZeroArmorTimeMin);
        Assert.Equal(3, ObjMonDamageSpellArmorCore.ZeroArmorTimeMax);
        Assert.Equal(3, ObjMonDamageSpellArmorCore.AreaDownRange);
        Assert.Equal(2, ObjMonDamageSpellArmorCore.MonsterBtState);
        Assert.Equal(0, ObjMonDamageSpellArmorCore.SkillBtState);

        Assert.Equal(6152, ObjMonDamageSpellArmorCore.SpellMagicIdDeclLine);
        Assert.Equal(6268, ObjMonDamageSpellArmorCore.ArmorMagicIdDeclLine);
        Assert.Equal(1, ObjMonDamageSpellArmorCore.MagicIdDefault);
        Assert.Equal(2, ObjMonDamageSpellArmorCore.MagicIdEffect);
        Assert.Equal(4, ObjMonDamageSpellArmorCore.MagicIdSitesPerClass);

        Assert.Equal(6224, ObjMonDamageSpellArmorCore.SpellOuterStart);
        Assert.Equal(6253, ObjMonDamageSpellArmorCore.SpellOuterEnd);
        Assert.Equal(6339, ObjMonDamageSpellArmorCore.ArmorOuterStart);
        Assert.Equal(6368, ObjMonDamageSpellArmorCore.ArmorOuterEnd);
        Assert.Equal(30, ObjMonDamageSpellArmorCore.OuterLines);
        Assert.Equal(0, ObjMonDamageSpellArmorCore.OuterDiffLines);
        Assert.Equal(5651, ObjMonDamageSpellArmorCore.J215TemplateStart);
        Assert.Equal(5680, ObjMonDamageSpellArmorCore.J215TemplateEnd);
        Assert.Equal(7, ObjMonDamageSpellArmorCore.TemplateConfirmations);
        Assert.Equal(39, ObjMonDamageSpellArmorCore.J217OuterLines);

        Assert.Equal(213, ObjMonDamageSpellArmorCore.SpellClassDeclLine);
        Assert.Equal(219, ObjMonDamageSpellArmorCore.ArmorClassDeclLine);
        Assert.Equal(207, ObjMonDamageSpellArmorCore.FoxMagicClassDeclLine);
        Assert.Equal(6, ObjMonDamageSpellArmorCore.DeclGap);
        Assert.Equal(24, ObjMonDamageSpellArmorCore.ClassesCovered);
        Assert.Equal(30, ObjMonDamageSpellArmorCore.RemainingClasses);
        Assert.Equal(16, ObjMonDamageSpellArmorCore.ShellOccurrence);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonDamageSpellArmorCore.SpanMatches());
        Assert.True(ObjMonDamageSpellArmorCore.TotalLinesAddUp());
        Assert.True(ObjMonDamageSpellArmorCore.SpellComesFirst());
        Assert.True(ObjMonDamageSpellArmorCore.MethodsAscending());
        Assert.True(ObjMonDamageSpellArmorCore.RunsFollowTheirMethods());
        Assert.True(ObjMonDamageSpellArmorCore.ClassesAdjacent());
        Assert.True(ObjMonDamageSpellArmorCore.WithinUnit());
        Assert.True(ObjMonDamageSpellArmorCore.NoInstrumentation());
    }

    // ===================== 一、两类的逐行对照 =====================

    [Fact]
    public void IdentityFacts()
    {
        Assert.True(ObjMonDamageSpellArmorCore.IdenticalExceptEffectBlock());
        Assert.True(ObjMonDamageSpellArmorCore.HeadZeroDiff());
        Assert.True(ObjMonDamageSpellArmorCore.TailZeroDiff());
        Assert.True(ObjMonDamageSpellArmorCore.HundredSixIdenticalLines());
        Assert.True(ObjMonDamageSpellArmorCore.SpellDiffersInThree());
        Assert.True(ObjMonDamageSpellArmorCore.ArmorDiffersInTwo());
        Assert.True(ObjMonDamageSpellArmorCore.LineCountsDifferByOne());
        Assert.True(ObjMonDamageSpellArmorCore.TotalDiffsAreFive());
        Assert.True(ObjMonDamageSpellArmorCore.DifferingLinesAddUp());
        Assert.True(ObjMonDamageSpellArmorCore.OffsetIsOne());
    }

    [Fact]
    public void SegmentArithmetic()
    {
        // **吸蓝 1+57+6+45 = 109；减防御 1+57+5+45 = 108**
        Assert.Equal(109, 1 + ObjMonDamageSpellArmorCore.HeadLines
            + ObjMonDamageSpellArmorCore.SpellEffectLines
            + ObjMonDamageSpellArmorCore.TailLines);
        Assert.Equal(108, 1 + ObjMonDamageSpellArmorCore.HeadLines
            + ObjMonDamageSpellArmorCore.ArmorEffectLines
            + ObjMonDamageSpellArmorCore.TailLines);

        // **逐字相同 = 57 + 45 + 4**
        Assert.Equal(106, ObjMonDamageSpellArmorCore.HeadLines
            + ObjMonDamageSpellArmorCore.TailLines
            + ObjMonDamageSpellArmorCore.SharedEffectLines);

        Assert.Equal(1, ObjMonDamageSpellArmorCore.SpellLines
            - ObjMonDamageSpellArmorCore.ArmorLines);
    }

    [Fact]
    public void SegmentTableFacts()
    {
        Assert.True(ObjMonDamageSpellArmorCore.SegmentCompareExtracted());
        Assert.True(ObjMonDamageSpellArmorCore.TwoSegmentsZeroDiff());
        Assert.True(ObjMonDamageSpellArmorCore.HeadSpansMatch());
        Assert.True(ObjMonDamageSpellArmorCore.TailSpansMatch());
        Assert.True(ObjMonDamageSpellArmorCore.SpellIsHundredNineArmorIsHundredEight());

        Assert.Equal(4, ObjMonDamageSpellArmorCore.SegmentCompare.Length);
        Assert.Equal(1, ObjMonDamageSpellArmorCore.SegmentCompare[0].Diffs);
        Assert.Equal(0, ObjMonDamageSpellArmorCore.SegmentCompare[1].Diffs);
        Assert.Equal(57, ObjMonDamageSpellArmorCore.SegmentCompare[1].SpellLines);
        Assert.Equal(6, ObjMonDamageSpellArmorCore.SegmentCompare[2].SpellLines);
        Assert.Equal(5, ObjMonDamageSpellArmorCore.SegmentCompare[2].ArmorLines);
        Assert.Equal(0, ObjMonDamageSpellArmorCore.SegmentCompare[3].Diffs);
    }

    // ---------- 效果块 ----------

    [Fact]
    public void EffectBlockFacts()
    {
        Assert.True(ObjMonDamageSpellArmorCore.SpellClassIsSuperset());
        Assert.True(ObjMonDamageSpellArmorCore.ThreeVsTwoStatements());
        Assert.True(ObjMonDamageSpellArmorCore.TwoDifferentDefenceMechanisms());
        Assert.True(ObjMonDamageSpellArmorCore.AreaVsSingleTarget());
        Assert.True(ObjMonDamageSpellArmorCore.NamingUnderstates());
        Assert.True(ObjMonDamageSpellArmorCore.MechanismsExtracted());
        Assert.True(ObjMonDamageSpellArmorCore.ParamCountsDifferByFour());
        Assert.True(ObjMonDamageSpellArmorCore.SpellStatementsExtracted());
        Assert.True(ObjMonDamageSpellArmorCore.ArmorStatementsExtracted());
        Assert.True(ObjMonDamageSpellArmorCore.BothSetMagicIdTwo());
    }

    [Fact]
    public void EffectStatementTables()
    {
        Assert.Equal(3, ObjMonDamageSpellArmorCore.SpellEffectStatements.Length);
        Assert.Contains("DamageSpell", ObjMonDamageSpellArmorCore.SpellEffectStatements[0]);
        Assert.Contains("wMagicID := 2", ObjMonDamageSpellArmorCore.SpellEffectStatements[1]);
        Assert.Contains("MagMakeDefenceAreaDown",
            ObjMonDamageSpellArmorCore.SpellEffectStatements[2]);

        Assert.Equal(2, ObjMonDamageSpellArmorCore.ArmorEffectStatements.Length);
        Assert.Contains("ZeroArmor", ObjMonDamageSpellArmorCore.ArmorEffectStatements[0]);
        Assert.Contains("wMagicID := 2", ObjMonDamageSpellArmorCore.ArmorEffectStatements[1]);

        // **两套减防机制**
        Assert.Equal(2, ObjMonDamageSpellArmorCore.DefenceMechanisms.Length);
        Assert.Equal("MagMakeDefenceAreaDown",
            ObjMonDamageSpellArmorCore.DefenceMechanisms[0].Mechanism);
        Assert.Equal(5, ObjMonDamageSpellArmorCore.DefenceMechanisms[0].Params);
        Assert.Equal("ZeroArmor",
            ObjMonDamageSpellArmorCore.DefenceMechanisms[1].Mechanism);
        Assert.Equal(1, ObjMonDamageSpellArmorCore.DefenceMechanisms[1].Params);
    }

    [Fact]
    public void EffectGateBoundaries()
    {
        Assert.True(ObjMonDamageSpellArmorCore.OneInThree());
        Assert.True(ObjMonDamageSpellArmorCore.InsideDamageGuard());
        Assert.True(ObjMonDamageSpellArmorCore.SameBoundBothClasses());
        Assert.True(ObjMonDamageSpellArmorCore.CommonestRollBound());
        Assert.True(ObjMonDamageSpellArmorCore.RollZeroFires());
        Assert.True(ObjMonDamageSpellArmorCore.RollOneDoesNot());
        Assert.True(ObjMonDamageSpellArmorCore.RollTwoDoesNot());

        Assert.True(ObjMonDamageSpellArmorCore.EffectFires(0));
        Assert.False(ObjMonDamageSpellArmorCore.EffectFires(1));
        Assert.False(ObjMonDamageSpellArmorCore.EffectFires(2));
    }

    [Fact]
    public void ApprFacts()
    {
        Assert.True(ObjMonDamageSpellArmorCore.NoApprCheck());
        Assert.True(ObjMonDamageSpellArmorCore.ContrastWithJ207AndJ217());
    }

    // ---------- 辅助方法 ----------

    [Fact]
    public void HelperCensusFacts()
    {
        Assert.True(ObjMonDamageSpellArmorCore.TwoDamageSpellSites());
        Assert.True(ObjMonDamageSpellArmorCore.CompletesJ210Census());
        Assert.True(ObjMonDamageSpellArmorCore.BothAreManaDrainClasses());
        Assert.True(ObjMonDamageSpellArmorCore.ThisSiteChecked());
        Assert.True(ObjMonDamageSpellArmorCore.SingleZeroArmorSite());
        Assert.True(ObjMonDamageSpellArmorCore.DedicatedToArmorClass());
        Assert.True(ObjMonDamageSpellArmorCore.TimeOneToThree());
        Assert.True(ObjMonDamageSpellArmorCore.CommentSaysZeroDefence());
    }

    [Fact]
    public void DeclarationLineFacts()
    {
        Assert.True(ObjMonDamageSpellArmorCore.DeclLinesChecked());
        Assert.True(ObjMonDamageSpellArmorCore.ImplLinesChecked());
        Assert.True(ObjMonDamageSpellArmorCore.FiveParamSignature());
        Assert.True(ObjMonDamageSpellArmorCore.RangeThree());
        Assert.True(ObjMonDamageSpellArmorCore.SecGetsDamage());
        Assert.True(ObjMonDamageSpellArmorCore.StateTwoVsSkillZero());
        Assert.True(ObjMonDamageSpellArmorCore.SemanticallyOpaqueParam());
        Assert.True(ObjMonDamageSpellArmorCore.AreaDownStateIsTwo());
    }

    [Fact]
    public void ZeroArmorTimeBoundaries()
    {
        Assert.True(ObjMonDamageSpellArmorCore.MinZeroArmorTime());
        Assert.True(ObjMonDamageSpellArmorCore.MaxZeroArmorTime());

        Assert.Equal(1, ObjMonDamageSpellArmorCore.ZeroArmorTime(0));
        Assert.Equal(2, ObjMonDamageSpellArmorCore.ZeroArmorTime(1));
        Assert.Equal(3, ObjMonDamageSpellArmorCore.ZeroArmorTime(2));
    }

    // ===================== 二、外层模板 =====================

    [Fact]
    public void OuterTemplateFacts()
    {
        Assert.True(ObjMonDamageSpellArmorCore.OuterTemplateBothVerbatim());
        Assert.True(ObjMonDamageSpellArmorCore.ThirtyLinesZeroDiffBoth());
        Assert.True(ObjMonDamageSpellArmorCore.SeventhConfirmation());
        Assert.True(ObjMonDamageSpellArmorCore.J217WasTheOutlier());
        Assert.True(ObjMonDamageSpellArmorCore.OuterSpansMatch());
        Assert.True(ObjMonDamageSpellArmorCore.TemplateSpanMatches());
        Assert.True(ObjMonDamageSpellArmorCore.SameEngageGate());
        Assert.True(ObjMonDamageSpellArmorCore.OnlyEffectDiffers());
    }

    // ===================== 三、wMagicID =====================

    [Fact]
    public void MagicIdFacts()
    {
        Assert.True(ObjMonDamageSpellArmorCore.SingleRoleFlag());
        Assert.True(ObjMonDamageSpellArmorCore.FourSitesEach());
        Assert.True(ObjMonDamageSpellArmorCore.AssignedAndSentOnly());
        Assert.True(ObjMonDamageSpellArmorCore.NeverUsedInCondition());
        Assert.True(ObjMonDamageSpellArmorCore.ContrastWithJ210DualRole());
        Assert.True(ObjMonDamageSpellArmorCore.SpellMagicIdLinesChecked());
        Assert.True(ObjMonDamageSpellArmorCore.ArmorMagicIdLinesChecked());
        Assert.True(ObjMonDamageSpellArmorCore.SameRelativePositions());
        Assert.True(ObjMonDamageSpellArmorCore.LastOffsetDiffersByOne());
        Assert.True(ObjMonDamageSpellArmorCore.OffsetDiffMatchesLineCountDiff());
        Assert.True(ObjMonDamageSpellArmorCore.DefaultOneEffectTwo());
        Assert.True(ObjMonDamageSpellArmorCore.PerClassConvention());
        Assert.True(ObjMonDamageSpellArmorCore.NoGlobalTable());
        Assert.True(ObjMonDamageSpellArmorCore.ValuesReusedWithDifferentMeaning());

        Assert.Equal(new[] { 6152, 6156, 6206, 6221 },
            ObjMonDamageSpellArmorCore.SpellMagicIdLines);
        Assert.Equal(new[] { 6268, 6272, 6322, 6336 },
            ObjMonDamageSpellArmorCore.ArmorMagicIdLines);
    }

    [Fact]
    public void MagicIdBoundaries()
    {
        Assert.True(ObjMonDamageSpellArmorCore.NoEffectSendsOne());
        Assert.True(ObjMonDamageSpellArmorCore.EffectSendsTwo());

        Assert.Equal(1, ObjMonDamageSpellArmorCore.PickMagicId(false));
        Assert.Equal(2, ObjMonDamageSpellArmorCore.PickMagicId(true));
    }

    // ===================== 四、麻痹段 =====================

    [Fact]
    public void ParalysisFacts()
    {
        Assert.True(ObjMonDamageSpellArmorCore.ParalysisBlockVerbatim());
        Assert.True(ObjMonDamageSpellArmorCore.SingleUnParalysisRead());
        Assert.True(ObjMonDamageSpellArmorCore.HasMaxGuard());
        Assert.True(ObjMonDamageSpellArmorCore.CorrectForm());
        Assert.True(ObjMonDamageSpellArmorCore.ContrastWithJ217DoubleRead());
        Assert.True(ObjMonDamageSpellArmorCore.SamePropertyAsJ217());
        Assert.True(ObjMonDamageSpellArmorCore.ParalysisSlotIsFive());
    }

    [Fact]
    public void ParalysisBoundaries()
    {
        Assert.True(ObjMonDamageSpellArmorCore.AllTrueParalyses());
        Assert.True(ObjMonDamageSpellArmorCore.UnParalysisBlocks());
        Assert.True(ObjMonDamageSpellArmorCore.ResistanceRollBlocks());

        Assert.True(ObjMonDamageSpellArmorCore.ParalysisFires(false, true, 0, 0, 0));
        Assert.False(ObjMonDamageSpellArmorCore.ParalysisFires(true, true, 0, 0, 0));
        Assert.False(ObjMonDamageSpellArmorCore.ParalysisFires(false, true, 0, 0, 1));
        Assert.False(ObjMonDamageSpellArmorCore.ParalysisFires(false, false, 0, 1, 0));
    }

    // ===================== 五、资源与收尾 =====================

    [Fact]
    public void ResourceAndShellFacts()
    {
        Assert.True(ObjMonDamageSpellArmorCore.NoListBothClasses());
        Assert.True(ObjMonDamageSpellArmorCore.NoTryFinallyNeeded());
        Assert.True(ObjMonDamageSpellArmorCore.DelegatesAreaToCallee());
        Assert.True(ObjMonDamageSpellArmorCore.FifthApproachToAreaEffects());
        Assert.True(ObjMonDamageSpellArmorCore.PureInheritedShellBoth());
        Assert.True(ObjMonDamageSpellArmorCore.TwoMoreOccurrences());
        Assert.True(ObjMonDamageSpellArmorCore.FifteenthAndSixteenth());
        Assert.True(ObjMonDamageSpellArmorCore.SeventhConsecutiveBatch());
    }

    [Fact]
    public void TripletFacts()
    {
        Assert.True(ObjMonDamageSpellArmorCore.SameBaseSameShape());
        Assert.True(ObjMonDamageSpellArmorCore.Triplet());
        Assert.True(ObjMonDamageSpellArmorCore.DeclsSixApart());
        Assert.True(ObjMonDamageSpellArmorCore.SharedCommentPrefixWithJ217());

        Assert.Equal(207, ObjMonDamageSpellArmorCore.FoxMagicClassDeclLine);
        Assert.Equal(213, ObjMonDamageSpellArmorCore.SpellClassDeclLine);
        Assert.Equal(219, ObjMonDamageSpellArmorCore.ArmorClassDeclLine);
    }

    // ===================== 六、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonDamageSpellArmorCore.TwentyFourClassesCovered());
        Assert.True(ObjMonDamageSpellArmorCore.RemainingApprox());
    }
}
