using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J210：`ObjMon.pas` 中 `TExtinguishDayFireAttackMonster`（灭天火怪物）
/// 两个方法 1:1 测试（合计 120 行）。
/// **本批最有价值的发现**：`wMagicID` 一个变量兼两个角色 ——
/// 既是特效编号（45 灭天火 / 6 施毒术）、又是 5051 `if wMagicID &lt;&gt; 6 then`
/// 的控制流开关，**于是"施毒"与"造成伤害"变成互斥的**。
/// </summary>
public sealed class ObjMonExtinguishFireCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(5029, ObjMonExtinguishFireCore.MagicStart);
        Assert.Equal(5144, ObjMonExtinguishFireCore.MagicEnd);
        Assert.Equal(116, ObjMonExtinguishFireCore.MagicLines);
        Assert.Equal(5031, ObjMonExtinguishFireCore.NestedStart);
        Assert.Equal(5113, ObjMonExtinguishFireCore.NestedEnd);
        Assert.Equal(83, ObjMonExtinguishFireCore.NestedLines);
        Assert.Equal(5115, ObjMonExtinguishFireCore.OuterStart);
        Assert.Equal(5144, ObjMonExtinguishFireCore.OuterEnd);
        Assert.Equal(30, ObjMonExtinguishFireCore.OuterLines);
        Assert.Equal(5146, ObjMonExtinguishFireCore.RunStart);
        Assert.Equal(5149, ObjMonExtinguishFireCore.RunEnd);
        Assert.Equal(4, ObjMonExtinguishFireCore.RunLines);
        Assert.Equal(120, ObjMonExtinguishFireCore.TotalLines);

        Assert.Equal(5036, ObjMonExtinguishFireCore.MagicIdDeclareLine);
        Assert.Equal(5040, ObjMonExtinguishFireCore.MagicIdDefaultLine);
        Assert.Equal(45, ObjMonExtinguishFireCore.DefaultMagicId);
        Assert.Equal(5048, ObjMonExtinguishFireCore.MagicIdPoisonLine);
        Assert.Equal(6, ObjMonExtinguishFireCore.PoisonMagicId);
        Assert.Equal(5051, ObjMonExtinguishFireCore.GateLine);
        Assert.Equal(5112, ObjMonExtinguishFireCore.SendEffectLine);
        Assert.Equal(5, ObjMonExtinguishFireCore.MagicIdSites);
        Assert.Equal(4766, ObjMonExtinguishFireCore.J207CommentedSixLine);

        Assert.Equal(810, ObjMonExtinguishFireCore.UnPosionPropertyLine);
        Assert.Equal(807, ObjMonExtinguishFireCore.UnParalysisPropertyLine);
        Assert.Equal(24837, ObjMonExtinguishFireCore.GetUnPosionLine);
        Assert.Equal(24795, ObjMonExtinguishFireCore.GetUnParalysisLine);
        Assert.Equal(16, ObjMonExtinguishFireCore.UnPosionIndex);
        Assert.Equal(13, ObjMonExtinguishFireCore.UnParalysisIndex);
        Assert.Equal(30, ObjMonExtinguishFireCore.NewValueCapacity);
        Assert.Equal(100, ObjMonExtinguishFireCore.RollFaces);

        Assert.Equal(1, ObjMonExtinguishFireCore.POISON_DAMAGEARMOR);
        Assert.Equal(0, ObjMonExtinguishFireCore.POISON_DECHEALTH);
        Assert.Equal(5, ObjMonExtinguishFireCore.POISON_STONE);
        Assert.Equal(60, ObjMonExtinguishFireCore.FixedPoisonTime);
        Assert.Equal(10, ObjMonExtinguishFireCore.FixedPoisonPower);
        Assert.Equal(5098, ObjMonExtinguishFireCore.DamageSpellLine);
        Assert.Equal(6205, ObjMonExtinguishFireCore.DamageSpellOtherLine);
        Assert.Equal(2, ObjMonExtinguishFireCore.DamageSpellSites);
        Assert.Equal(6, ObjMonExtinguishFireCore.HealOccurrence);
        Assert.Equal(15, ObjMonExtinguishFireCore.ClassesCovered);
        Assert.Equal(39, ObjMonExtinguishFireCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonExtinguishFireCore.SpanMatches());
        Assert.True(ObjMonExtinguishFireCore.TotalLinesAddUp());
        Assert.True(ObjMonExtinguishFireCore.DecompositionAddsUp());
        Assert.True(ObjMonExtinguishFireCore.NestedBeforeOuter());
        Assert.True(ObjMonExtinguishFireCore.RunAfterMagic());
        Assert.True(ObjMonExtinguishFireCore.WithinUnit());
        Assert.True(ObjMonExtinguishFireCore.NoInstrumentation());
        Assert.True(ObjMonExtinguishFireCore.SameStructureAsJ207());
        Assert.True(ObjMonExtinguishFireCore.OuterIdenticalLength());
    }

    // ===================== 一、wMagicID 双角色 =====================

    [Fact]
    public void DualRoleFacts()
    {
        Assert.True(ObjMonExtinguishFireCore.DualRoleVariable());
        Assert.True(ObjMonExtinguishFireCore.OneAssignTwoEffects());
        Assert.True(ObjMonExtinguishFireCore.PoisonXorDamage());
        Assert.True(ObjMonExtinguishFireCore.MutuallyExclusive());
        Assert.True(ObjMonExtinguishFireCore.FirstOfItsKind());
        Assert.True(ObjMonExtinguishFireCore.DiffersFromSameFieldDifferentMeaning());
        Assert.True(ObjMonExtinguishFireCore.ValueAsControlFlow());
        Assert.True(ObjMonExtinguishFireCore.MagicIdTableExtracted());
        Assert.True(ObjMonExtinguishFireCore.OneDeclaration());
        Assert.True(ObjMonExtinguishFireCore.TwoAssignments());
        Assert.True(ObjMonExtinguishFireCore.GateAfterBothAssigns());
        Assert.True(ObjMonExtinguishFireCore.SendAfterGate());
        Assert.True(ObjMonExtinguishFireCore.DamageBlockIsLarge());
    }

    [Fact]
    public void MagicIdSiteTable()
    {
        Assert.Equal(5, ObjMonExtinguishFireCore.MagicIdSiteTable.Length);
        Assert.Equal("declare", ObjMonExtinguishFireCore.MagicIdSiteTable[0].Kind);
        Assert.Equal(5040, ObjMonExtinguishFireCore.MagicIdSiteTable[1].Line);
        Assert.Equal("assign-45", ObjMonExtinguishFireCore.MagicIdSiteTable[1].Kind);
        Assert.Equal("assign-6", ObjMonExtinguishFireCore.MagicIdSiteTable[2].Kind);
        Assert.Equal("control-flow", ObjMonExtinguishFireCore.MagicIdSiteTable[3].Kind);
        Assert.Equal("send-effect", ObjMonExtinguishFireCore.MagicIdSiteTable[4].Kind);
        Assert.Equal(5112, ObjMonExtinguishFireCore.MagicIdSiteTable[4].Line);
    }

    [Fact]
    public void GateBoundaries()
    {
        // **默认编号（45）下伤害会跑**
        Assert.True(ObjMonExtinguishFireCore.DefaultRunsDamage());
        Assert.True(ObjMonExtinguishFireCore.DamageRuns(45));

        // **施毒编号（6）下伤害被跳过**
        Assert.True(ObjMonExtinguishFireCore.PoisonSkipsDamage());
        Assert.False(ObjMonExtinguishFireCore.DamageRuns(6));

        // **两个编号不同，且伤害段很大（>50 行）**
        Assert.True(ObjMonExtinguishFireCore.IdsDiffer());
        Assert.True(ObjMonExtinguishFireCore.DamageLines > 50);
        Assert.Equal(60, ObjMonExtinguishFireCore.DamageLines);
    }

    [Fact]
    public void EffectIdCensus()
    {
        Assert.True(ObjMonExtinguishFireCore.NineEffectIds());
        Assert.True(ObjMonExtinguishFireCore.ClientSideResourceIndex());
        Assert.True(ObjMonExtinguishFireCore.PassThrough());
        Assert.True(ObjMonExtinguishFireCore.EffectIdsExtracted());
        Assert.True(ObjMonExtinguishFireCore.EffectIdsUnique());
        Assert.True(ObjMonExtinguishFireCore.OurIdsInTable());
        Assert.True(ObjMonExtinguishFireCore.SixIsPoisonEffect());
        Assert.True(ObjMonExtinguishFireCore.CrossBatchConsistent());
        Assert.True(ObjMonExtinguishFireCore.J207CommentedTheSameSix());

        Assert.Equal(9, ObjMonExtinguishFireCore.EffectIds.Length);
        Assert.Equal(6, ObjMonExtinguishFireCore.EffectIds[4].Id);
        Assert.Equal("施毒术", ObjMonExtinguishFireCore.EffectIds[4].Meaning);
        Assert.Equal(33, ObjMonExtinguishFireCore.EffectIds[5].Id);
        Assert.Equal("冰咆哮", ObjMonExtinguishFireCore.EffectIds[5].Meaning);
        Assert.Equal(44, ObjMonExtinguishFireCore.EffectIds[6].Id);
        Assert.Equal("寒冰掌", ObjMonExtinguishFireCore.EffectIds[6].Meaning);
        Assert.Equal(45, ObjMonExtinguishFireCore.EffectIds[7].Id);
        Assert.Equal("灭天火", ObjMonExtinguishFireCore.EffectIds[7].Meaning);
        Assert.Equal(58, ObjMonExtinguishFireCore.EffectIds[8].Id);
        Assert.Equal("流星火雨", ObjMonExtinguishFireCore.EffectIds[8].Meaning);
    }

    // ===================== 二、掷骰属性 =====================

    [Fact]
    public void DicePropertyFacts()
    {
        Assert.True(ObjMonExtinguishFireCore.UnPosionIsRoll());
        Assert.True(ObjMonExtinguishFireCore.UnParalysisIsRoll());
        Assert.True(ObjMonExtinguishFireCore.PropertyGetterRolls());
        Assert.True(ObjMonExtinguishFireCore.IsResistCheckNotStateCheck());
        Assert.True(ObjMonExtinguishFireCore.ReadConsumesRandom());
        Assert.True(ObjMonExtinguishFireCore.TwoReadsMayDiffer());
        Assert.True(ObjMonExtinguishFireCore.ReadOnceHere());
        Assert.True(ObjMonExtinguishFireCore.MustNotCache());
        Assert.True(ObjMonExtinguishFireCore.NewValueCapacity30());
        Assert.True(ObjMonExtinguishFireCore.IndicesInRange());
        Assert.True(ObjMonExtinguishFireCore.DeterministicAtEnds());
        Assert.True(ObjMonExtinguishFireCore.PropertyLinesExtracted());
        Assert.True(ObjMonExtinguishFireCore.ImplsAfterDecls());
        Assert.True(ObjMonExtinguishFireCore.GettersBeforeMethod());
    }

    [Fact]
    public void ResistRollBoundaries()
    {
        // **两端确定性**
        Assert.True(ObjMonExtinguishFireCore.HundredAlwaysResists());
        Assert.True(ObjMonExtinguishFireCore.ZeroNeverResists());
        Assert.False(ObjMonExtinguishFireCore.PoisonCanLand(100, 0));
        Assert.True(ObjMonExtinguishFireCore.PoisonCanLand(0, 0));

        // **恰好等于阈值：50 < 50 为假 => 未受保护 => 毒能落下**
        Assert.True(ObjMonExtinguishFireCore.ExactlyRateNotResist());
        Assert.True(ObjMonExtinguishFireCore.PoisonCanLand(50, 50));

        // **阈值之下：49 < 50 为真 => 受保护 => 毒被挡下**
        Assert.True(ObjMonExtinguishFireCore.PoisonBelowRateIsBlocked());
        Assert.False(ObjMonExtinguishFireCore.PoisonCanLand(50, 49));

        // **三层语义：属性值 / 取反**
        Assert.True(ObjMonExtinguishFireCore.PropertySemanticsVerified());
        Assert.True(ObjMonExtinguishFireCore.PoisonCanLandIsNegationOfProperty());

        // **中间值是概率性的**
        Assert.True(ObjMonExtinguishFireCore.MiddleIsProbabilistic());
        Assert.NotEqual(
            ObjMonExtinguishFireCore.PoisonCanLand(50, 10),
            ObjMonExtinguishFireCore.PoisonCanLand(50, 90));
    }

    [Fact]
    public void RandomZeroRiskFacts()
    {
        Assert.True(ObjMonExtinguishFireCore.NoMaxGuardOnPoison());
        Assert.True(ObjMonExtinguishFireCore.GuardOnParalysis());
        Assert.True(ObjMonExtinguishFireCore.SameAsJ207Pattern());
        Assert.True(ObjMonExtinguishFireCore.RecurringShape());
        Assert.True(ObjMonExtinguishFireCore.UnguardedStillDecides());
        Assert.True(ObjMonExtinguishFireCore.GuardClamps());
        Assert.True(ObjMonExtinguishFireCore.DifferOnlyWhenNegative());

        // **无保护版：抗毒为 0 时仍会判定**
        Assert.True(ObjMonExtinguishFireCore.PoisonRollUnguarded(0, 0));
    }

    // ===================== 三、与 J207 的关系 =====================

    [Fact]
    public void SharedTemplateFacts()
    {
        Assert.True(ObjMonExtinguishFireCore.OuterBodyVerbatimSameAsJ207());
        Assert.True(ObjMonExtinguishFireCore.BothAxesCorrect());
        Assert.True(ObjMonExtinguishFireCore.SameEngageAndApproach());
        Assert.True(ObjMonExtinguishFireCore.SameSkeletonDifferentBody());
        Assert.True(ObjMonExtinguishFireCore.SharedTemplate());
        Assert.True(ObjMonExtinguishFireCore.ThreeVsTwoSections());
        Assert.True(ObjMonExtinguishFireCore.TemplateExtracted());
        Assert.True(ObjMonExtinguishFireCore.TemplateHasNoAxisTypo());

        Assert.Equal(6, ObjMonExtinguishFireCore.SharedOuterTemplate.Length);
    }

    [Fact]
    public void PoisonParameterContrast()
    {
        Assert.True(ObjMonExtinguishFireCore.RedVsGreenPoison());
        Assert.True(ObjMonExtinguishFireCore.FixedVsRandomDuration());
        Assert.True(ObjMonExtinguishFireCore.FixedVsPowerScaled());
        Assert.True(ObjMonExtinguishFireCore.FixedTimeWithinJ207Range());
        Assert.True(ObjMonExtinguishFireCore.ChecksOwnPoisonSlot());
        Assert.True(ObjMonExtinguishFireCore.AddsResistRoll());
        Assert.True(ObjMonExtinguishFireCore.J207HasNoResistRoll());
        Assert.True(ObjMonExtinguishFireCore.DifferentPoisonGates());
        Assert.True(ObjMonExtinguishFireCore.PoisonConstantsDiffer());
        Assert.True(ObjMonExtinguishFireCore.ParalysisSlotIsFive());

        // **固定 60 落在 J207 的 10..69 之内**
        Assert.True(ObjMonExtinguishFireCore.FixedPoisonTime
            >= ObjMonExtinguishFireCore.J207PoisonTimeMin);
        Assert.True(ObjMonExtinguishFireCore.FixedPoisonTime
            <= ObjMonExtinguishFireCore.J207PoisonTimeMax);
    }

    // ===================== 四、伤害管线 =====================

    [Fact]
    public void PipelineFacts()
    {
        Assert.True(ObjMonExtinguishFireCore.HasHealIdiom());
        Assert.True(ObjMonExtinguishFireCore.SixthOccurrence());
        Assert.True(ObjMonExtinguishFireCore.J209LacksIt());
        Assert.True(ObjMonExtinguishFireCore.TrulyOptional());
        Assert.True(ObjMonExtinguishFireCore.SingleTargetOnly());
        Assert.True(ObjMonExtinguishFireCore.NoTListNoGetMapBaseObjects());
        Assert.True(ObjMonExtinguishFireCore.SimplestForm());
        Assert.True(ObjMonExtinguishFireCore.PipelineExtracted());
        Assert.True(ObjMonExtinguishFireCore.CapBeforeAbsorb());
        Assert.True(ObjMonExtinguishFireCore.SameAsJ205J207J209());
        Assert.True(ObjMonExtinguishFireCore.StillInconsistent());
        Assert.True(ObjMonExtinguishFireCore.HealAfterAbsorb());
        Assert.True(ObjMonExtinguishFireCore.DamageSpellAfterStruck());
        Assert.True(ObjMonExtinguishFireCore.ParalysisAfterDamageSpell());
        Assert.True(ObjMonExtinguishFireCore.CapLineBeforeAbsorbLine());
        Assert.True(ObjMonExtinguishFireCore.HealBeforeDamageSpell());

        // **管线十二段、首尾已核对**
        Assert.Equal(12, ObjMonExtinguishFireCore.Pipeline.Length);
        Assert.Equal("GetMagStruckDamage", ObjMonExtinguishFireCore.Pipeline[0]);
        Assert.Equal("DamageReboundPower", ObjMonExtinguishFireCore.Pipeline[11]);

        // **下标：封顶 5 在吸收 6 之前**
        Assert.Equal(5, Array.IndexOf(ObjMonExtinguishFireCore.Pipeline, "GetAttackPowerMax"));
        Assert.Equal(6, Array.IndexOf(ObjMonExtinguishFireCore.Pipeline, "absorb"));
        Assert.Equal(7, Array.IndexOf(ObjMonExtinguishFireCore.Pipeline, "heal"));
        Assert.Equal(9, Array.IndexOf(ObjMonExtinguishFireCore.Pipeline, "DamageSpell"));
        Assert.Equal(10, Array.IndexOf(ObjMonExtinguishFireCore.Pipeline, "paralysis"));
    }

    [Fact]
    public void HealAndDamageSpellFacts()
    {
        // **回血（第六次出现）**
        Assert.Equal(0, ObjMonExtinguishFireCore.HealAmount(1000, 0));
        Assert.Equal(100, ObjMonExtinguishFireCore.HealAmount(1000, 10));
        Assert.True(ObjMonExtinguishFireCore.ZeroMpNoHeal());
        Assert.True(ObjMonExtinguishFireCore.TenMpTenthHeal());

        // **减蓝：全文件只有两处**
        Assert.True(ObjMonExtinguishFireCore.HasDamageSpell());
        Assert.True(ObjMonExtinguishFireCore.AbsentInSiblings());
        Assert.True(ObjMonExtinguishFireCore.OnlyTwoSitesInFile());
        Assert.True(ObjMonExtinguishFireCore.PairedWithAbsorbMpClass());
        Assert.True(ObjMonExtinguishFireCore.DamageSpellLinesExtracted());
        Assert.True(ObjMonExtinguishFireCore.OtherSiteIsLater());
    }

    [Fact]
    public void MasterDiscountFacts()
    {
        Assert.True(ObjMonExtinguishFireCore.MasterDiscount());
        Assert.True(ObjMonExtinguishFireCore.HalfRateHalves());
        Assert.True(ObjMonExtinguishFireCore.DiscountAfterPower());

        Assert.Equal(50, ObjMonExtinguishFireCore.SlavePower(100, 50));
        Assert.Equal(100, ObjMonExtinguishFireCore.SlavePower(100, 100));
    }

    // ===================== 五、整体与跨度 =====================

    [Fact]
    public void OverallAndSpanFacts()
    {
        Assert.True(ObjMonExtinguishFireCore.PureInheritedShellAgain());
        Assert.True(ObjMonExtinguishFireCore.FourthConsecutive());
        Assert.True(ObjMonExtinguishFireCore.TwelfthOccurrence());
        Assert.True(ObjMonExtinguishFireCore.TwoMethodsOnly());
        Assert.True(ObjMonExtinguishFireCore.NoCreate());
        Assert.True(ObjMonExtinguishFireCore.SameShapeAsJ207J208());
        Assert.True(ObjMonExtinguishFireCore.FifteenClassesCovered());
        Assert.True(ObjMonExtinguishFireCore.RemainingApprox());

        // **修正后的体量对照：本批嵌套短 30 行、总长也短 30 行**
        Assert.True(ObjMonExtinguishFireCore.NestedThirtyShorter());
        Assert.True(ObjMonExtinguishFireCore.TotalThirtyShorterThanJ207());
        Assert.True(ObjMonExtinguishFireCore.BothDifferencesAgree());
    }
}
