using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J217：`ObjMon.pas` 中两个类的 1:1 测试（合计 204 行）：
/// `TStoneFoxMonster.AttackTarget`（10 行）与
/// `TFoxMagicAttackMonster.MagicAttackTarget`（190 行，含两个嵌套过程）
/// + `Run`（4 行）。
/// **本批最有价值的发现**：`CanStone` 自己已检查 `not UnParalysis`，
/// 而调用方又显式检查了一次 ——
/// 因 `UnParalysis` 每次读取都重新掷骰（J210 已查明），
/// **这个重复检查把石化命中率打了平方折扣**
/// （抵抗率 50% 时从 1/2 降到 1/4）。
/// </summary>
public sealed class ObjMonStoneFoxCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(5936, ObjMonStoneFoxCore.StoneStart);
        Assert.Equal(5945, ObjMonStoneFoxCore.StoneEnd);
        Assert.Equal(10, ObjMonStoneFoxCore.StoneLines);
        Assert.Equal(5948, ObjMonStoneFoxCore.MagicStart);
        Assert.Equal(6137, ObjMonStoneFoxCore.MagicEnd);
        Assert.Equal(190, ObjMonStoneFoxCore.MagicLines);
        Assert.Equal(6139, ObjMonStoneFoxCore.RunStart);
        Assert.Equal(6142, ObjMonStoneFoxCore.RunEnd);
        Assert.Equal(4, ObjMonStoneFoxCore.RunLines);
        Assert.Equal(204, ObjMonStoneFoxCore.TotalLines);

        Assert.Equal(5950, ObjMonStoneFoxCore.NestedSingleStart);
        Assert.Equal(6017, ObjMonStoneFoxCore.NestedSingleEnd);
        Assert.Equal(68, ObjMonStoneFoxCore.NestedSingleLines);
        Assert.Equal(6020, ObjMonStoneFoxCore.NestedGroupStart);
        Assert.Equal(6097, ObjMonStoneFoxCore.NestedGroupEnd);
        Assert.Equal(78, ObjMonStoneFoxCore.NestedGroupLines);
        Assert.Equal(6018, ObjMonStoneFoxCore.GroupCommentLine);
        Assert.Equal(6099, ObjMonStoneFoxCore.OuterStart);
        Assert.Equal(6137, ObjMonStoneFoxCore.OuterEnd);
        Assert.Equal(39, ObjMonStoneFoxCore.OuterLines);
        Assert.Equal(30, ObjMonStoneFoxCore.TemplateLines);
        Assert.Equal(9, ObjMonStoneFoxCore.InsertLines);

        Assert.Equal(796, ObjMonStoneFoxCore.CanStoneDeclLine);
        Assert.Equal(11720, ObjMonStoneFoxCore.CanStoneImplLine);
        Assert.Equal(11722, ObjMonStoneFoxCore.CanStoneNotParalysisLine);
        Assert.Equal(0, ObjMonStoneFoxCore.CanStoneDefaultValue);
        Assert.Equal(807, ObjMonStoneFoxCore.UnParalysisDeclLine);
        Assert.Equal(24795, ObjMonStoneFoxCore.GetUnParalysisImpl);
        Assert.Equal(13, ObjMonStoneFoxCore.UnParalysisIndex);
        Assert.Equal(100, ObjMonStoneFoxCore.RollFaces);
        Assert.Equal(8, ObjMonStoneFoxCore.StoneRollBound);
        Assert.Equal(6, ObjMonStoneFoxCore.UnguardedOccurrences);

        Assert.Equal(5942, ObjMonStoneFoxCore.MakePosionLine);
        Assert.Equal(5, ObjMonStoneFoxCore.POISON_STONE);
        Assert.Equal(6, ObjMonStoneFoxCore.PoisonTimeBound);
        Assert.Equal(2, ObjMonStoneFoxCore.PoisonTimeBase);
        Assert.Equal(2, ObjMonStoneFoxCore.PoisonTimeMin);
        Assert.Equal(7, ObjMonStoneFoxCore.PoisonTimeMax);
        Assert.Equal(0, ObjMonStoneFoxCore.PoisonPower);
        Assert.Equal(3, ObjMonStoneFoxCore.J212PoisonTimeBase);
        Assert.Equal(5, ObjMonStoneFoxCore.PoisonConfigCount);

        Assert.Equal(202, ObjMonStoneFoxCore.StoneClassDeclLine);
        Assert.Equal(204, ObjMonStoneFoxCore.StoneAttackDeclLine);
        Assert.Equal(207, ObjMonStoneFoxCore.FoxMagicClassDeclLine);
        Assert.Equal(213, ObjMonStoneFoxCore.DamageSpellDeclLine);
        Assert.Equal(219, ObjMonStoneFoxCore.DamageArmorDeclLine);
        Assert.Equal(190, ObjMonStoneFoxCore.FoxAttackDeclLine);
        Assert.Equal(5939, ObjMonStoneFoxCore.InheritedExprLine);

        Assert.Equal(3842, ObjMonStoneFoxCore.Mon36GroupImplLine);
        Assert.Equal(5, ObjMonStoneFoxCore.GroupEntityCount);
        Assert.Equal(4, ObjMonStoneFoxCore.GroupSignatureCount);
        Assert.Equal(6115, ObjMonStoneFoxCore.NestedGroupCallLine);
        Assert.Equal(6029, ObjMonStoneFoxCore.GroupTryLine);
        Assert.Equal(6028, ObjMonStoneFoxCore.GroupListCreateLine);
        Assert.Equal(6031, ObjMonStoneFoxCore.GroupGetMapLine);
        Assert.Equal(2, ObjMonStoneFoxCore.GroupRadius);
        Assert.Equal(6038, ObjMonStoneFoxCore.GroupFilterLine);
        Assert.Equal(6094, ObjMonStoneFoxCore.GroupFinallyLine);
        Assert.Equal(6095, ObjMonStoneFoxCore.GroupFreeLine);
        Assert.Equal(6093, ObjMonStoneFoxCore.GroupEffectLine);
        Assert.Equal(6016, ObjMonStoneFoxCore.SingleEffectLine);
        Assert.Equal(1, ObjMonStoneFoxCore.SingleEffectId);
        Assert.Equal(0, ObjMonStoneFoxCore.GroupEffectId);

        Assert.Equal(6111, ObjMonStoneFoxCore.InsertStart);
        Assert.Equal(6119, ObjMonStoneFoxCore.InsertEnd);
        Assert.Equal(6111, ObjMonStoneFoxCore.ApprCheckLine);
        Assert.Equal(607, ObjMonStoneFoxCore.ApprValue);
        Assert.Equal(6113, ObjMonStoneFoxCore.InsertRollLine);
        Assert.Equal(5, ObjMonStoneFoxCore.InsertRollBound);
        Assert.Equal(6120, ObjMonStoneFoxCore.SingleCallLine);
        Assert.Equal(6109, ObjMonStoneFoxCore.TemplateRollLine);
        Assert.Equal(2, ObjMonStoneFoxCore.TemplateRollBound);
        Assert.Equal(22, ObjMonStoneFoxCore.ClassesCovered);
        Assert.Equal(32, ObjMonStoneFoxCore.RemainingClasses);
        Assert.Equal(14, ObjMonStoneFoxCore.ShellOccurrence);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonStoneFoxCore.SpanMatches());
        Assert.True(ObjMonStoneFoxCore.TotalLinesAddUp());
        Assert.True(ObjMonStoneFoxCore.MagicDecompositionAddsUp());
        Assert.True(ObjMonStoneFoxCore.NestedSpansMatch());
        Assert.True(ObjMonStoneFoxCore.StoneComesFirst());
        Assert.True(ObjMonStoneFoxCore.MethodsAscending());
        Assert.True(ObjMonStoneFoxCore.OuterAfterNested());
        Assert.True(ObjMonStoneFoxCore.WithinUnit());
        Assert.True(ObjMonStoneFoxCore.NoInstrumentation());
    }

    // ===================== 一、CanStone 的重复检查 =====================

    [Fact]
    public void CanStoneFacts()
    {
        Assert.True(ObjMonStoneFoxCore.CanStoneAlreadyChecksIt());
        Assert.True(ObjMonStoneFoxCore.RedundantExplicitCheck());
        Assert.True(ObjMonStoneFoxCore.DoubleCheck());
        Assert.True(ObjMonStoneFoxCore.CanStoneIncludesIt());
        Assert.True(ObjMonStoneFoxCore.CanStoneRolls());
        Assert.True(ObjMonStoneFoxCore.PropertyRerollsOnRead());
        Assert.True(ObjMonStoneFoxCore.TwoIndependentRolls());
        Assert.True(ObjMonStoneFoxCore.DoublesTheResistRequirement());
        Assert.True(ObjMonStoneFoxCore.SuccessRateSquared());
        Assert.True(ObjMonStoneFoxCore.NotMereRedundancy());
        Assert.True(ObjMonStoneFoxCore.NewDefectShape());
        Assert.True(ObjMonStoneFoxCore.CallerAndCalleeBothCheck());
        Assert.True(ObjMonStoneFoxCore.CompletesJ210AndJ212());
        Assert.True(ObjMonStoneFoxCore.NeitherFewerNorMore());
    }

    [Fact]
    public void CanStoneBoundaries()
    {
        // **`CanStone` 内含 `not UnParalysis`**
        Assert.True(ObjMonStoneFoxCore.CanStone(true, 100, 0));
        Assert.False(ObjMonStoneFoxCore.CanStone(false, 100, 0));

        // **`CanStone` 内含一次掷骰（roll 非 0 则不通过）**
        Assert.False(ObjMonStoneFoxCore.CanStone(true, 100, 1));
        Assert.False(ObjMonStoneFoxCore.CanStone(true, 100, 99));

        Assert.True(ObjMonStoneFoxCore.UnParalysisResists(50, 49));
        Assert.False(ObjMonStoneFoxCore.UnParalysisResists(50, 50));
    }

    [Fact]
    public void ResistGateFacts()
    {
        // **任何一次抵抗就挡下石化**
        Assert.True(ObjMonStoneFoxCore.AnyResistBlocks());
        Assert.True(ObjMonStoneFoxCore.TwoResistsAlsoBlock());
        Assert.True(ObjMonStoneFoxCore.TwoResistsBlock());

        // **只有零抵抗才放行**
        Assert.True(ObjMonStoneFoxCore.OnlyZeroResistPasses());
        Assert.True(ObjMonStoneFoxCore.NoResistAllows());
        Assert.True(ObjMonStoneFoxCore.WriteGuardAllows(false));
        Assert.False(ObjMonStoneFoxCore.WriteGuardAllows(true));
    }

    [Fact]
    public void ProbabilityHalvingFacts()
    {
        // **抵抗率 50% 时：单检 1/2、双检 1/4**
        Assert.True(ObjMonStoneFoxCore.AtFiftyPercentItQuarters());
        Assert.True(Math.Abs(ObjMonStoneFoxCore.StoneProbabilitySingle(50) - 0.5) < 1e-9);
        Assert.True(Math.Abs(ObjMonStoneFoxCore.StoneProbabilityDouble(50) - 0.25) < 1e-9);

        // **双检一律更小（中间值）**
        Assert.True(ObjMonStoneFoxCore.DoubleCheckIsLessLikely());
        Assert.True(ObjMonStoneFoxCore.DoubleAlwaysSmallerInBetween());

        // **两端点相同**
        Assert.True(ObjMonStoneFoxCore.ZeroResistBothCertain());
        Assert.True(ObjMonStoneFoxCore.FullResistBothNever());
    }

    [Fact]
    public void GuardParameterFacts()
    {
        Assert.True(ObjMonStoneFoxCore.DefaultParamZero());
        Assert.True(ObjMonStoneFoxCore.BecomesRandomAntiPoison());
        Assert.True(ObjMonStoneFoxCore.NoMaxGuardAgain());
        Assert.True(ObjMonStoneFoxCore.SixthOccurrence());
        Assert.True(ObjMonStoneFoxCore.LivesInObjBase());

        Assert.Equal(0, ObjMonStoneFoxCore.CanStoneDefaultValue);
        Assert.Equal(11720, ObjMonStoneFoxCore.CanStoneImplLine);
    }

    // ===================== 二、TStoneFoxMonster =====================

    [Fact]
    public void InheritanceFacts()
    {
        Assert.True(ObjMonStoneFoxCore.DerivesFromFox());
        Assert.True(ObjMonStoneFoxCore.OverrideIsCorrect());
        Assert.True(ObjMonStoneFoxCore.SameVirtualChain());
        Assert.True(ObjMonStoneFoxCore.InheritedAsExpression());
        Assert.True(ObjMonStoneFoxCore.StaticCallNotDispatch());
        Assert.True(ObjMonStoneFoxCore.NilCheckFirst());
        Assert.True(ObjMonStoneFoxCore.ResultMirrorsBase());
    }

    [Fact]
    public void ShortCircuitFacts()
    {
        // **空值检查与 `inherited` 调用在同一行（5939）**
        Assert.True(ObjMonStoneFoxCore.ShortCircuitProtectsBase());
        Assert.True(ObjMonStoneFoxCore.NilCheckSameLineOnLeft());
        Assert.Equal(ObjMonStoneFoxCore.NilCheckLine, ObjMonStoneFoxCore.InheritedExprLine);

        // **左边为假则右边不求值**
        Assert.True(ObjMonStoneFoxCore.LeftFalseSkipsRight());
        Assert.True(ObjMonStoneFoxCore.OrderMatters());
        Assert.False(ObjMonStoneFoxCore.ShortCircuitAnd(false, true));
        Assert.True(ObjMonStoneFoxCore.ShortCircuitAnd(true, true));
    }

    [Fact]
    public void ResultMirrorBoundaries()
    {
        Assert.True(ObjMonStoneFoxCore.BaseTrueGivesTrue());
        Assert.True(ObjMonStoneFoxCore.BaseFalseGivesFalse());
        Assert.True(ObjMonStoneFoxCore.NullTargetFalse());

        Assert.True(ObjMonStoneFoxCore.StoneAttackResult(true, true));
        Assert.False(ObjMonStoneFoxCore.StoneAttackResult(true, false));
        Assert.False(ObjMonStoneFoxCore.StoneAttackResult(false, true));
    }

    [Fact]
    public void StoningProbabilityFacts()
    {
        Assert.True(ObjMonStoneFoxCore.OneInEight());
        Assert.True(ObjMonStoneFoxCore.NestedUnderBaseSuccess());
        Assert.True(ObjMonStoneFoxCore.CompoundProbability());
        Assert.True(ObjMonStoneFoxCore.PracticallyLower());
        Assert.True(ObjMonStoneFoxCore.AllTrueStones());
        Assert.True(ObjMonStoneFoxCore.EitherResistBlocks());
        Assert.True(ObjMonStoneFoxCore.BaseFailBlocks());

        Assert.True(ObjMonStoneFoxCore.StonesFully(true, 0, false, false, true));
        Assert.False(ObjMonStoneFoxCore.StonesFully(true, 0, true, false, true));
        Assert.False(ObjMonStoneFoxCore.StonesFully(true, 0, false, true, true));
        Assert.False(ObjMonStoneFoxCore.StonesFully(false, 0, false, false, true));
        Assert.False(ObjMonStoneFoxCore.StonesFully(true, 1, false, false, true));
    }

    // ---------- MakePosion ----------

    [Fact]
    public void PoisonConfigFacts()
    {
        Assert.True(ObjMonStoneFoxCore.FifthPoisonConfig());
        Assert.True(ObjMonStoneFoxCore.DurationTwoToSeven());
        Assert.True(ObjMonStoneFoxCore.StrengthIsZero());
        Assert.True(ObjMonStoneFoxCore.SameBoundAsJ212DifferentBase());
        Assert.True(ObjMonStoneFoxCore.BaseDiffersByOne());
        Assert.True(ObjMonStoneFoxCore.ParalysisNeverCarriesPower());
        Assert.True(ObjMonStoneFoxCore.PoisonConfigsExtracted());
        Assert.True(ObjMonStoneFoxCore.FiveConfigsAllDiffer());
    }

    [Fact]
    public void PoisonDurationBoundaries()
    {
        Assert.True(ObjMonStoneFoxCore.MinPoisonTime());
        Assert.True(ObjMonStoneFoxCore.MaxPoisonTime());

        Assert.Equal(2, ObjMonStoneFoxCore.PoisonTime(0));
        Assert.Equal(7, ObjMonStoneFoxCore.PoisonTime(5));

        Assert.Equal(5, ObjMonStoneFoxCore.PoisonConfigs.Length);
        Assert.Equal("J217", ObjMonStoneFoxCore.PoisonConfigs[4].Batch);
        Assert.Equal("paralysis", ObjMonStoneFoxCore.PoisonConfigs[4].Kind);
        Assert.Equal("0", ObjMonStoneFoxCore.PoisonConfigs[4].Power);
    }

    // ===================== 三、两个嵌套过程 =====================

    [Fact]
    public void NestedProcedureFacts()
    {
        Assert.True(ObjMonStoneFoxCore.TwoNestedProcedures());
        Assert.True(ObjMonStoneFoxCore.FirstOfItsKind());
        Assert.True(ObjMonStoneFoxCore.SingleAndGroup());
        Assert.True(ObjMonStoneFoxCore.BothCalledFromOuter());
        Assert.True(ObjMonStoneFoxCore.SingleIsSixtyEight());
        Assert.True(ObjMonStoneFoxCore.GroupIsSeventyEight());
        Assert.True(ObjMonStoneFoxCore.NestedIsZeroArg());
        Assert.True(ObjMonStoneFoxCore.FourClassLevelDecls());
        Assert.True(ObjMonStoneFoxCore.FiveEntitiesFourSignatures());
        Assert.True(ObjMonStoneFoxCore.NoShadowingHere());
        Assert.True(ObjMonStoneFoxCore.DifferentSemanticsFromJ204());
        Assert.True(ObjMonStoneFoxCore.RecursShape21());
        Assert.True(ObjMonStoneFoxCore.GroupEntitiesExtracted());
        Assert.True(ObjMonStoneFoxCore.TwoSignaturesRepeated());
        Assert.True(ObjMonStoneFoxCore.TMon35IsMagicSubclass());
        Assert.True(ObjMonStoneFoxCore.ClassDeclLinesAscending());

        Assert.Equal(2, ObjMonStoneFoxCore.NestedCount);
        Assert.Equal(4, ObjMonStoneFoxCore.ClassLevelGroupDeclLines.Length);
        Assert.Equal(new[] { 54, 92, 100, 116 }, ObjMonStoneFoxCore.ClassLevelGroupDeclLines);
        Assert.Equal(5, ObjMonStoneFoxCore.GroupEntities.Length);
    }

    [Fact]
    public void GroupEntityTable()
    {
        Assert.Equal("TMon36_XMonster", ObjMonStoneFoxCore.GroupEntities[0].Site);
        Assert.Contains("boSelfRage", ObjMonStoneFoxCore.GroupEntities[0].Signature);
        Assert.Equal("TMon38_12Monster", ObjMonStoneFoxCore.GroupEntities[1].Site);
        Assert.Equal("TMon38_13Monster", ObjMonStoneFoxCore.GroupEntities[2].Site);
        Assert.Equal("TMon35_2Monster", ObjMonStoneFoxCore.GroupEntities[3].Site);
        Assert.Contains("nested", ObjMonStoneFoxCore.GroupEntities[4].Site);
        Assert.Equal("()", ObjMonStoneFoxCore.GroupEntities[4].Signature);
    }

    [Fact]
    public void ProtectionFacts()
    {
        Assert.True(ObjMonStoneFoxCore.GroupHasTryFinally());
        Assert.True(ObjMonStoneFoxCore.OuterAndSingleDoNot());
        Assert.True(ObjMonStoneFoxCore.MatchesJ209NotJ207J212());
        Assert.True(ObjMonStoneFoxCore.StructuralReasonNotInconsistency());
        Assert.True(ObjMonStoneFoxCore.OnlyListBuilderIsProtected());
        Assert.True(ObjMonStoneFoxCore.FreeInFinally());
        Assert.True(ObjMonStoneFoxCore.TryAfterCreate());
    }

    // ---------- 半径与过滤器 ----------

    [Fact]
    public void RadiusAndCenterFacts()
    {
        Assert.True(ObjMonStoneFoxCore.RadiusTwoTargetCentered());
        Assert.True(ObjMonStoneFoxCore.FourthCombination());
        Assert.True(ObjMonStoneFoxCore.NeverUnified());
        Assert.True(ObjMonStoneFoxCore.GroupCombosExtracted());
        Assert.True(ObjMonStoneFoxCore.TwoHardcodedTwoDifferentCenters());
        Assert.True(ObjMonStoneFoxCore.AllCombosDiffer());

        Assert.Equal(4, ObjMonStoneFoxCore.GroupCombos.Length);
        Assert.Equal("J217", ObjMonStoneFoxCore.GroupCombos[3].Batch);
        Assert.Equal("hardcoded 2", ObjMonStoneFoxCore.GroupCombos[3].Radius);
        Assert.Equal("target", ObjMonStoneFoxCore.GroupCombos[3].Center);
    }

    [Fact]
    public void RejectFormBoundaries()
    {
        Assert.True(ObjMonStoneFoxCore.RejectFormIdiom());
        Assert.True(ObjMonStoneFoxCore.Idiom2());
        Assert.True(ObjMonStoneFoxCore.ConsistentWithJ212());
        Assert.True(ObjMonStoneFoxCore.VisibleProperPasses());
        Assert.True(ObjMonStoneFoxCore.HiddenNoCoolEyeRejected());
        Assert.True(ObjMonStoneFoxCore.ImproperRejected());

        Assert.False(ObjMonStoneFoxCore.RejectForm(false, false, true));
        Assert.True(ObjMonStoneFoxCore.RejectForm(true, false, true));
        Assert.False(ObjMonStoneFoxCore.RejectForm(true, true, true));
        Assert.True(ObjMonStoneFoxCore.RejectForm(false, false, false));
    }

    [Fact]
    public void PrimaryTargetFacts()
    {
        Assert.True(ObjMonStoneFoxCore.NoPrimaryExclusion());
        Assert.True(ObjMonStoneFoxCore.WouldHitTwice());
        Assert.True(ObjMonStoneFoxCore.SavedByTheExit());
        Assert.True(ObjMonStoneFoxCore.ContrastWithJ209Exclusion());
    }

    // ---------- 特效消息 ----------

    [Fact]
    public void EffectFacts()
    {
        Assert.True(ObjMonStoneFoxCore.DifferentMessages());
        Assert.True(ObjMonStoneFoxCore.SingleUsesLightingGroupUsesLightingEx());
        Assert.True(ObjMonStoneFoxCore.NumbersOneAndZero());
        Assert.True(ObjMonStoneFoxCore.ContrastWithJ212SameMessage());
        Assert.True(ObjMonStoneFoxCore.SinglePicksLighting());
        Assert.True(ObjMonStoneFoxCore.GroupPicksLightingEx());

        Assert.Equal("RM_LIGHTING:1", ObjMonStoneFoxCore.PickEffect(false));
        Assert.Equal("RM_LIGHTINGEX:0", ObjMonStoneFoxCore.PickEffect(true));
        Assert.Equal(20102, ObjMonStoneFoxCore.RM_LIGHTING);
        Assert.Equal(20198, ObjMonStoneFoxCore.RM_LIGHTINGEX);
    }

    // ===================== 四、外层体与插入块 =====================

    [Fact]
    public void InsertionFacts()
    {
        Assert.True(ObjMonStoneFoxCore.TemplateWithInsertion());
        Assert.True(ObjMonStoneFoxCore.FirstInsertion());
        Assert.True(ObjMonStoneFoxCore.NineLineInsert());
        Assert.True(ObjMonStoneFoxCore.Appr607GetsGroupAttack());
        Assert.True(ObjMonStoneFoxCore.OneInFive());
        Assert.True(ObjMonStoneFoxCore.OtherwiseSingle());
        Assert.True(ObjMonStoneFoxCore.OuterIsThirtyNine());
        Assert.True(ObjMonStoneFoxCore.ThirtyPlusNine());
        Assert.True(ObjMonStoneFoxCore.RestMatchesTemplate());
        Assert.True(ObjMonStoneFoxCore.OuterDecompositionAddsUp());
    }

    [Fact]
    public void InsertDispatchBoundaries()
    {
        Assert.True(ObjMonStoneFoxCore.Appr607RollZeroGroups());
        Assert.True(ObjMonStoneFoxCore.Appr607RollNotZeroSingle());
        Assert.True(ObjMonStoneFoxCore.OtherApprAlwaysSingle());

        Assert.Equal("group", ObjMonStoneFoxCore.PickAttack(607, 0));
        Assert.Equal("single", ObjMonStoneFoxCore.PickAttack(607, 1));
        Assert.Equal("single", ObjMonStoneFoxCore.PickAttack(600, 0));
        Assert.Equal("single", ObjMonStoneFoxCore.PickAttack(231, 0));
    }

    [Fact]
    public void ApprOrderingFacts()
    {
        Assert.True(ObjMonStoneFoxCore.CheapCheckFirst());
        Assert.True(ObjMonStoneFoxCore.NoRollWhenApprDiffers());
        Assert.True(ObjMonStoneFoxCore.ContrastWithJ204Ordering());
        Assert.True(ObjMonStoneFoxCore.TemplateRollIsHalf());
    }

    [Fact]
    public void Appr607Census()
    {
        Assert.True(ObjMonStoneFoxCore.FourAppr607Sites());
        Assert.True(ObjMonStoneFoxCore.TwoSetForms());
        Assert.True(ObjMonStoneFoxCore.TwoSingleForms());
        Assert.True(ObjMonStoneFoxCore.ScatteredApprSpecialCasing());
        Assert.True(ObjMonStoneFoxCore.ApprTableExtracted());
        Assert.True(ObjMonStoneFoxCore.ThisBatchApprLine());

        Assert.Equal(4, ObjMonStoneFoxCore.Appr607Lines.Length);
        Assert.Equal(new[] { 3531, 3709, 4006, 6111 }, ObjMonStoneFoxCore.Appr607Lines);
        Assert.Equal(new[] { 3531, 4006 }, ObjMonStoneFoxCore.Appr607SetLines);
        Assert.Equal(new[] { 3709, 6111 }, ObjMonStoneFoxCore.Appr607SingleLines);
    }

    // ---------- Run ----------

    [Fact]
    public void RunShellFacts()
    {
        Assert.True(ObjMonStoneFoxCore.PureInheritedShellAgain());
        Assert.True(ObjMonStoneFoxCore.SixthConsecutive());
        Assert.True(ObjMonStoneFoxCore.FourteenthOccurrence());
        Assert.True(ObjMonStoneFoxCore.VerbatimSameAsJ207ToJ211());
        Assert.True(ObjMonStoneFoxCore.RunDecompositionAddsUp());
    }

    // ===================== 五、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonStoneFoxCore.TwentyTwoClassesCovered());
        Assert.True(ObjMonStoneFoxCore.RemainingApprox());
        Assert.True(ObjMonStoneFoxCore.ThreeShareCommentPrefix());
        Assert.True(ObjMonStoneFoxCore.OnlySuffixDiffers());
        Assert.True(ObjMonStoneFoxCore.DisambiguatedBySuffixOnly());
        Assert.True(ObjMonStoneFoxCore.SameFamilyAsJ213());
        Assert.True(ObjMonStoneFoxCore.DeclLinesChecked());
        Assert.True(ObjMonStoneFoxCore.DeclsSixApart());
        Assert.True(ObjMonStoneFoxCore.SameBaseSameShape());
        Assert.True(ObjMonStoneFoxCore.SiblingPair());
        Assert.True(ObjMonStoneFoxCore.DetailDiffersOnly());
    }
}
