using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J232：`ObjMon.pas` 中 `TTortoiseMonster`（乌龟怪物）两个方法的 1:1 测试（106 行）。
/// **本批最有价值的发现**：
/// ① 外层体是共享模板第 9 次确认、而且**三十行里只差一个数字**（范围门 6→10）；
/// ② 本批**修正**了 J229/J230 那条规律 ——
///    真正决定末尾判据死活的是 **`Exit` 的必然性**、而不是门的类型；
/// ③ `m_boUnParalysis` 这个名字在本文件里**只出现在被注释的代码**里；
/// ④ 那段 `{ }` 禁用的是一个**整语句块**（第四种花括号禁用、也是目前最大的单位），
///    且被禁的版本**更早更简单**（缺 `or (Random(100) < m_btFluteStoneParalysisRate)` 一项）。
/// </summary>
public sealed class ObjMonTortoiseCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(8064, ObjMonTortoiseCore.Start);
        Assert.Equal(8165, ObjMonTortoiseCore.End);
        Assert.Equal(102, ObjMonTortoiseCore.Lines);
        Assert.Equal(8066, ObjMonTortoiseCore.NestedStart);
        Assert.Equal(8134, ObjMonTortoiseCore.NestedEnd);
        Assert.Equal(69, ObjMonTortoiseCore.NestedLines);
        Assert.Equal(8136, ObjMonTortoiseCore.OuterStart);
        Assert.Equal(8165, ObjMonTortoiseCore.OuterEnd);
        Assert.Equal(30, ObjMonTortoiseCore.OuterLines);
        Assert.Equal(8167, ObjMonTortoiseCore.RunStart);
        Assert.Equal(8170, ObjMonTortoiseCore.RunEnd);
        Assert.Equal(4, ObjMonTortoiseCore.RunLines);
        Assert.Equal(106, ObjMonTortoiseCore.TotalLines);
        Assert.Equal(1, ObjMonTortoiseCore.NestedCount);

        Assert.Equal(5651, ObjMonTortoiseCore.TemplateStart);
        Assert.Equal(5680, ObjMonTortoiseCore.TemplateEnd);
        Assert.Equal(30, ObjMonTortoiseCore.TemplateLines);
        Assert.Equal(1, ObjMonTortoiseCore.TemplateDiffLines);
        Assert.Equal(5659, ObjMonTortoiseCore.TemplateRangeLine);
        Assert.Equal(6, ObjMonTortoiseCore.TemplateRangeThreshold);
        Assert.Equal(8144, ObjMonTortoiseCore.RangeGateLine);
        Assert.Equal(10, ObjMonTortoiseCore.RangeThreshold);
        Assert.Equal(9, ObjMonTortoiseCore.TemplateConfirmations);
        Assert.Equal(8146, ObjMonTortoiseCore.GateLine);
        Assert.Equal(2, ObjMonTortoiseCore.GateBound);
        Assert.Equal(8148, ObjMonTortoiseCore.AttackCallLine);
        Assert.Equal(8149, ObjMonTortoiseCore.ResultTrueLine);
        Assert.Equal(8150, ObjMonTortoiseCore.GateExitLine);
        Assert.Equal(8153, ObjMonTortoiseCore.SameMapLine);
        Assert.Equal(8155, ObjMonTortoiseCore.TailCheckLine);
        Assert.Equal(6, ObjMonTortoiseCore.TailThreshold);
        Assert.Equal(8157, ObjMonTortoiseCore.SetTargetXYLine);
        Assert.Equal(8162, ObjMonTortoiseCore.DiscardLine);
        Assert.Equal(8140, ObjMonTortoiseCore.CooldownLine);
        Assert.Equal(8142, ObjMonTortoiseCore.HitTickLine);
        Assert.Equal(8143, ObjMonTortoiseCore.HitDelayLine);
        Assert.Equal(8138, ObjMonTortoiseCore.NilGuardLine);

        Assert.Equal(8120, ObjMonTortoiseCore.DisabledStart);
        Assert.Equal(8125, ObjMonTortoiseCore.DisabledEnd);
        Assert.Equal(6, ObjMonTortoiseCore.DisabledLines);
        Assert.Equal(8121, ObjMonTortoiseCore.DisabledCondLine);
        Assert.Equal(8123, ObjMonTortoiseCore.DisabledApplyLine);
        Assert.Equal("m_boUnParalysis", ObjMonTortoiseCore.DisabledFieldName);
        Assert.Equal("UnParalysis", ObjMonTortoiseCore.LivePropertyName);
        Assert.Equal(807, ObjMonTortoiseCore.PropertyDeclLine);
        Assert.Equal(24795, ObjMonTortoiseCore.GetterImplLine);
        Assert.Equal(6, ObjMonTortoiseCore.FieldNameOccurrences);
        Assert.Equal(2, ObjMonTortoiseCore.FieldNameInObjMon);
        Assert.Equal(7909, ObjMonTortoiseCore.LiveParalysisLine);

        Assert.Equal(8133, ObjMonTortoiseCore.EffectLine);
        Assert.Equal(8112, ObjMonTortoiseCore.PositiveGuardLine);
        Assert.Equal(8132, ObjMonTortoiseCore.PositiveGuardEndLine);
        Assert.Equal(8083, ObjMonTortoiseCore.PowerRateAddLine);
        Assert.Equal(8085, ObjMonTortoiseCore.NextDamageLine);
        Assert.Equal(8087, ObjMonTortoiseCore.PowerMaxLine);
        Assert.Equal(20102, ObjMonTortoiseCore.RM_LIGHTING);

        Assert.Equal(196, ObjMonTortoiseCore.ClassDeclLine);
        Assert.Equal(195, ObjMonTortoiseCore.GroupCommentLine);
        Assert.Equal(198, ObjMonTortoiseCore.AttackDeclLine);
        Assert.Equal(199, ObjMonTortoiseCore.RunDeclLine);
        Assert.Equal(2, ObjMonTortoiseCore.ClassMethodCount);
        Assert.Equal(8173, ObjMonTortoiseCore.NextImplLine);
        Assert.Equal(8172, ObjMonTortoiseCore.NextSectionLine);
        Assert.Equal(35, ObjMonTortoiseCore.ClassesCovered);
        Assert.Equal(19, ObjMonTortoiseCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonTortoiseCore.SpanMatches());
        Assert.True(ObjMonTortoiseCore.TotalLinesAddUp());
        Assert.True(ObjMonTortoiseCore.DecompositionAddsUp());
        Assert.True(ObjMonTortoiseCore.OuterSpanMatches());
        Assert.True(ObjMonTortoiseCore.TemplateSpanMatches());
        Assert.True(ObjMonTortoiseCore.RunSpanMatches());
        Assert.True(ObjMonTortoiseCore.NestedBeforeOuter());
        Assert.True(ObjMonTortoiseCore.MethodsAscending());
        Assert.True(ObjMonTortoiseCore.MethodsContiguous());
        Assert.True(ObjMonTortoiseCore.WithinUnit());
        Assert.True(ObjMonTortoiseCore.NoInstrumentation());
    }

    // ===================== 一、模板只差一个常数 =====================

    [Fact]
    public void TemplateFacts()
    {
        Assert.True(ObjMonTortoiseCore.TemplateDiffIsOneLine());
        Assert.True(ObjMonTortoiseCore.OnlyThresholdDiffers());
        Assert.True(ObjMonTortoiseCore.SixToTen());
        Assert.True(ObjMonTortoiseCore.NinthConfirmation());
        Assert.True(ObjMonTortoiseCore.OnlyConstantChanged());
        Assert.True(ObjMonTortoiseCore.ThreeKindsOfVariation());
        Assert.True(ObjMonTortoiseCore.TemplateInstancesExtracted());
        Assert.True(ObjMonTortoiseCore.TwoPristineGroupings());
        Assert.True(ObjMonTortoiseCore.OnlyOneIsThreshold());
        Assert.True(ObjMonTortoiseCore.ThresholdDelta());
    }

    [Fact]
    public void TemplateInstanceTable()
    {
        Assert.Equal(6, ObjMonTortoiseCore.TemplateInstances.Length);
        Assert.Equal("J232", ObjMonTortoiseCore.TemplateInstances[5].Batch);
        Assert.Equal(30, ObjMonTortoiseCore.TemplateInstances[5].OuterLines);
        Assert.Contains("6 -> 10", ObjMonTortoiseCore.TemplateInstances[5].Variation);

        // **J219 是唯一少三行的**
        Assert.Equal(27, ObjMonTortoiseCore.TemplateInstances[2].OuterLines);
        Assert.Contains("deleted", ObjMonTortoiseCore.TemplateInstances[2].Variation);

        // **J222 换的是动作**
        Assert.Contains("approach", ObjMonTortoiseCore.TemplateInstances[3].Variation);
    }

    [Fact]
    public void RangeBoundaries()
    {
        Assert.True(ObjMonTortoiseCore.TenIsInRange());
        Assert.True(ObjMonTortoiseCore.ElevenIsOut());
        Assert.True(ObjMonTortoiseCore.TemplateWouldRejectSeven());

        Assert.True(ObjMonTortoiseCore.InRange(10, 10));
        Assert.True(ObjMonTortoiseCore.InRange(0, 10));
        Assert.False(ObjMonTortoiseCore.InRange(11, 0));
        Assert.False(ObjMonTortoiseCore.InRange(0, 11));

        // **模板版在 7 格时就已出界**
        Assert.True(Math.Abs(7) > ObjMonTortoiseCore.TemplateRangeThreshold);
    }

    // ---------- 末尾判据仍有意义 ----------

    [Fact]
    public void TailLivenessFacts()
    {
        Assert.True(ObjMonTortoiseCore.TailCheckStillMeaningful());
        Assert.True(ObjMonTortoiseCore.ExitInsideProbabilityGate());
        Assert.True(ObjMonTortoiseCore.TwoFallthroughCases());
        Assert.True(ObjMonTortoiseCore.SameAsJ222J228());
        Assert.True(ObjMonTortoiseCore.CorrectsTheRuleFromJ229J230());
        Assert.True(ObjMonTortoiseCore.ExitCertaintyIsWhatMatters());
        Assert.True(ObjMonTortoiseCore.J229StyleIsTautological());
        Assert.True(ObjMonTortoiseCore.ThisIsNotTautological());
        Assert.True(ObjMonTortoiseCore.WouldBeTautologicalIfExitCertain());
        Assert.True(ObjMonTortoiseCore.TailCheckCanBeFalse());
    }

    [Fact]
    public void TailLivenessTable()
    {
        // **`Exit` 必然 + 门 >= 判据 => 恒真（J229/J230 的形态）**
        Assert.True(ObjMonTortoiseCore.CheckIsTautological(7, 6, true));
        Assert.True(ObjMonTortoiseCore.CheckIsTautological(10, 6, true));

        // **`Exit` 有条件 => 不恒真（本类的形态）**
        Assert.False(ObjMonTortoiseCore.CheckIsTautological(10, 6, false));

        // **门 < 判据时即使 `Exit` 必然也不恒真（J215 模板）**
        Assert.False(ObjMonTortoiseCore.CheckIsTautological(5, 6, true));
    }

    [Fact]
    public void TailBoundaries()
    {
        Assert.True(ObjMonTortoiseCore.SixNoApproach());
        Assert.True(ObjMonTortoiseCore.SevenApproaches());

        Assert.False(ObjMonTortoiseCore.TailFires(6, 6));
        Assert.True(ObjMonTortoiseCore.TailFires(7, 0));
        Assert.True(ObjMonTortoiseCore.TailFires(0, 7));
        Assert.True(ObjMonTortoiseCore.TailFires(10, 10));
    }

    [Fact]
    public void RunShellFacts()
    {
        Assert.True(ObjMonTortoiseCore.RunIsPureShell());
        Assert.True(ObjMonTortoiseCore.NineteenthOccurrence());
    }

    [Fact]
    public void OrderFacts()
    {
        Assert.True(ObjMonTortoiseCore.NilGuardFirst());
        Assert.True(ObjMonTortoiseCore.CooldownOrder());
    }

    // ===================== 二、只出现在被注释代码里的名字 =====================

    [Fact]
    public void DisabledNameFacts()
    {
        Assert.True(ObjMonTortoiseCore.BraceBlockDisablesParalysis());
        Assert.True(ObjMonTortoiseCore.NameOnlyInDisabledCode());
        Assert.True(ObjMonTortoiseCore.SixOccurrencesTotal());
        Assert.True(ObjMonTortoiseCore.TwoInThisFile());
        Assert.True(ObjMonTortoiseCore.BothDisabled());
        Assert.True(ObjMonTortoiseCore.NoDeclarationFoundInMirror());
        Assert.True(ObjMonTortoiseCore.LiveCodeUsesPropertyOnly());
        Assert.True(ObjMonTortoiseCore.LegacyOrExternalName());
        Assert.True(ObjMonTortoiseCore.FourAssignmentsInFireDragon());
        Assert.True(ObjMonTortoiseCore.FieldNameDiffersFromProperty());
        Assert.True(ObjMonTortoiseCore.FireDragonLinesChecked());

        Assert.Equal(new[] { 8121, 8251 }, ObjMonTortoiseCore.FieldNameLines);
        Assert.Equal(new[] { 47, 128, 311, 367 }, ObjMonTortoiseCore.FireDragonLines);
    }

    [Fact]
    public void BraceDisableKindFacts()
    {
        Assert.True(ObjMonTortoiseCore.BraceDisablesWholeBlock());
        Assert.True(ObjMonTortoiseCore.SixLineBlock());
        Assert.True(ObjMonTortoiseCore.FourthKindOfBraceDisable());
        Assert.True(ObjMonTortoiseCore.LargestUnitSoFar());
        Assert.True(ObjMonTortoiseCore.BraceDisablesExtracted());
        Assert.True(ObjMonTortoiseCore.OnlyThisOneIsABlock());
        Assert.True(ObjMonTortoiseCore.AllUnitsDistinct());
    }

    [Fact]
    public void BraceDisableTable()
    {
        Assert.Equal(5, ObjMonTortoiseCore.BraceDisables.Length);
        Assert.Equal("J221", ObjMonTortoiseCore.BraceDisables[0].Batch);
        Assert.Equal("a condition", ObjMonTortoiseCore.BraceDisables[0].Unit);
        Assert.Equal("J223", ObjMonTortoiseCore.BraceDisables[1].Batch);
        Assert.Equal("a case branch", ObjMonTortoiseCore.BraceDisables[1].Unit);
        Assert.Equal("J229", ObjMonTortoiseCore.BraceDisables[2].Batch);
        Assert.Equal("an assignment", ObjMonTortoiseCore.BraceDisables[2].Unit);
        Assert.Equal("J230", ObjMonTortoiseCore.BraceDisables[3].Batch);
        Assert.Equal("J232", ObjMonTortoiseCore.BraceDisables[4].Batch);
    }

    // ---------- 被禁版本更早 ----------

    [Fact]
    public void EvolutionFacts()
    {
        Assert.True(ObjMonTortoiseCore.DisabledFormIsOlder());
        Assert.True(ObjMonTortoiseCore.MissingFluteRateTerm());
        Assert.True(ObjMonTortoiseCore.LiveFormHasTwoWays());
        Assert.True(ObjMonTortoiseCore.CommentRecordsEvolution());
        Assert.True(ObjMonTortoiseCore.SameWhenSwitchOn());
    }

    [Fact]
    public void ParalysisVersionBoundaries()
    {
        // **活动版：开关为假时靠笛声率仍可能生效**
        Assert.True(ObjMonTortoiseCore.LiveCanFireWithoutSwitch());
        Assert.True(ObjMonTortoiseCore.LiveParalysis(true, false, 100, 0, 0));

        // **被禁版：开关为假就一定不生效**
        Assert.True(ObjMonTortoiseCore.DisabledCannot());
        Assert.False(ObjMonTortoiseCore.DisabledParalysis(true, false, 0));

        // **开关为真且掷中时两者一致**
        Assert.True(ObjMonTortoiseCore.SameWhenSwitchOn());

        // **抗性掷骰未中时两者都不生效**
        Assert.False(ObjMonTortoiseCore.LiveParalysis(true, true, 0, 0, 1));
        Assert.False(ObjMonTortoiseCore.DisabledParalysis(true, true, 1));
    }

    [Fact]
    public void DicePropertyFacts()
    {
        Assert.True(ObjMonTortoiseCore.FieldVersusProperty());
        Assert.True(ObjMonTortoiseCore.DicePropertyVersusPlainField());
        Assert.True(ObjMonTortoiseCore.RestoringWouldChangeBehaviour());
        Assert.True(ObjMonTortoiseCore.DiceChangesPerRead());

        Assert.True(ObjMonTortoiseCore.DicePropertyFires(50, 10));
        Assert.False(ObjMonTortoiseCore.DicePropertyFires(50, 90));
    }

    // ===================== 三、其余 =====================

    [Fact]
    public void DamagePipelineFacts()
    {
        Assert.True(ObjMonTortoiseCore.EffectOutsideDamageGuard());
        Assert.True(ObjMonTortoiseCore.ZeroDamageStillSends());
        Assert.True(ObjMonTortoiseCore.SameAsJ228J229());
        Assert.True(ObjMonTortoiseCore.FiveStepPipeline());
        Assert.True(ObjMonTortoiseCore.SameAsJ228());
        Assert.True(ObjMonTortoiseCore.PipelineOrdered());
    }

    [Fact]
    public void ClassShapeFacts()
    {
        Assert.True(ObjMonTortoiseCore.SameBaseAsJ217J218J228());
        Assert.True(ObjMonTortoiseCore.FifthSubclass());
        Assert.True(ObjMonTortoiseCore.OnlyTwoOverrides());
        Assert.True(ObjMonTortoiseCore.AttackTargetNotOverridden());
        Assert.True(ObjMonTortoiseCore.BaseProvidesEntry());
        Assert.True(ObjMonTortoiseCore.TortoiseClosed());
        Assert.True(ObjMonTortoiseCore.ClassClosedInOneBatch());
        Assert.True(ObjMonTortoiseCore.ClassDeclChecked());
        Assert.True(ObjMonTortoiseCore.NextClassIsMon38_0());
        Assert.True(ObjMonTortoiseCore.NameWillRecurAt8251());
        Assert.True(ObjMonTortoiseCore.NextSectionChecked());
        Assert.True(ObjMonTortoiseCore.ThirtyFiveClassesCovered());
        Assert.True(ObjMonTortoiseCore.RemainingApprox());
    }
}
