using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J233：`ObjMon.pas` 中 `TMon38_0Monster`（Mon38-0 不能移动的怪物）
/// 四个方法的 1:1 测试（213 行）。
/// **本批最有价值的发现**：
/// ① 外层体是一个**新形状**（26 行、无 `SetTargetXY`），且里面有一处
///    **由"取反重述"造成的恒真 `if`**（8294 是 8283 的德摩根取反、
///    而中间隔着两个 `Exit`）—— 与 J219/J221/J229/J230 的**阈值型恒真成因不同**；
/// ② `m_boFixedHideMode` 在本类出现五次、**五次全部无效**（三处 `//` + 两处 `{ }` 块），
///    已被 `m_boStoneMode` + `STATE_STONE_MODE` 取代；
/// ③ `NowDigUP` 17 行里只有 **4 行是活的**、另 13 行是**两代**被禁实现；
/// ④ 本批记下**第五种禁用方式 —— 散落的行注释**（关 `if`/`begin` 在 8204-8205、
///    再在 **60 行之外**的 8264 补关 `end`）。
/// </summary>
public sealed class ObjMonMon38_0CoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(8173, ObjMonMon38_0Core.AttackStart);
        Assert.Equal(8299, ObjMonMon38_0Core.AttackEnd);
        Assert.Equal(127, ObjMonMon38_0Core.AttackLines);
        Assert.Equal(8175, ObjMonMon38_0Core.NestedStart);
        Assert.Equal(8272, ObjMonMon38_0Core.NestedEnd);
        Assert.Equal(98, ObjMonMon38_0Core.NestedLines);
        Assert.Equal(8274, ObjMonMon38_0Core.OuterStart);
        Assert.Equal(8299, ObjMonMon38_0Core.OuterEnd);
        Assert.Equal(26, ObjMonMon38_0Core.OuterLines);
        Assert.Equal(8301, ObjMonMon38_0Core.DigStart);
        Assert.Equal(8317, ObjMonMon38_0Core.DigEnd);
        Assert.Equal(17, ObjMonMon38_0Core.DigLines);
        Assert.Equal(8319, ObjMonMon38_0Core.CreateStart);
        Assert.Equal(8327, ObjMonMon38_0Core.CreateEnd);
        Assert.Equal(9, ObjMonMon38_0Core.CreateLines);
        Assert.Equal(8329, ObjMonMon38_0Core.RunStart);
        Assert.Equal(8388, ObjMonMon38_0Core.RunEnd);
        Assert.Equal(60, ObjMonMon38_0Core.RunLines);
        Assert.Equal(213, ObjMonMon38_0Core.TotalLines);
        Assert.Equal(4, ObjMonMon38_0Core.MethodCount);

        Assert.Equal(8282, ObjMonMon38_0Core.RangeCommentLine);
        Assert.Equal(8283, ObjMonMon38_0Core.RangeGateLine);
        Assert.Equal(8285, ObjMonMon38_0Core.AttackCallLine);
        Assert.Equal(8286, ObjMonMon38_0Core.ResultTrueLine);
        Assert.Equal(8287, ObjMonMon38_0Core.GateExitLine);
        Assert.Equal(8289, ObjMonMon38_0Core.InvertedMapLine);
        Assert.Equal(8291, ObjMonMon38_0Core.MapDiscardLine);
        Assert.Equal(8292, ObjMonMon38_0Core.MapDiscardExitLine);
        Assert.Equal(8294, ObjMonMon38_0Core.TautologicalLine);
        Assert.Equal(8296, ObjMonMon38_0Core.FarDiscardLine);
        Assert.Equal(8275, ObjMonMon38_0Core.ResultFalseLine);
        Assert.Equal(8276, ObjMonMon38_0Core.NilGuardLine);
        Assert.Equal(8278, ObjMonMon38_0Core.CooldownLine);
        Assert.Equal(8280, ObjMonMon38_0Core.HitTickLine);
        Assert.Equal(8281, ObjMonMon38_0Core.HitDelayLine);
        Assert.Equal("m_nAttackRage", ObjMonMon38_0Core.RageFieldName);
        Assert.Equal(77, ObjMonMon38_0Core.RageDeclLine);
        Assert.Equal(4, ObjMonMon38_0Core.RageValue);

        Assert.Equal("m_boFixedHideMode", ObjMonMon38_0Core.RetiredFieldName);
        Assert.Equal(5, ObjMonMon38_0Core.RetiredFieldOccurrences);
        Assert.Equal("m_boStoneMode", ObjMonMon38_0Core.ReplacementFieldName);
        Assert.Equal(8324, ObjMonMon38_0Core.StoneModeSetLine);
        Assert.Equal(8325, ObjMonMon38_0Core.CharStatusExLine);
        Assert.Equal(1, ObjMonMon38_0Core.STATE_STONE_MODE);
        Assert.Equal(181, ObjMonMon38_0Core.STATE_STONE_MODE_Line);

        Assert.Equal(8313, ObjMonMon38_0Core.DigLiveStart);
        Assert.Equal(8316, ObjMonMon38_0Core.DigLiveEnd);
        Assert.Equal(4, ObjMonMon38_0Core.DigLiveLines);
        Assert.Equal(8302, ObjMonMon38_0Core.DigVarCommentStart);
        Assert.Equal(8305, ObjMonMon38_0Core.DigBlock1Start);
        Assert.Equal(8309, ObjMonMon38_0Core.DigBlock1End);
        Assert.Equal(8310, ObjMonMon38_0Core.DigBlock2Start);
        Assert.Equal(8312, ObjMonMon38_0Core.DigBlock2End);
        Assert.Equal(13, ObjMonMon38_0Core.DigDisabledLines);
        Assert.Equal(8313, ObjMonMon38_0Core.DigClearLine);
        Assert.Equal(8314, ObjMonMon38_0Core.DigRecalcLine);
        Assert.Equal(8315, ObjMonMon38_0Core.DigSendLine);
        Assert.Equal(8316, ObjMonMon38_0Core.DigUnstoneLine);
        Assert.Equal(20099, ObjMonMon38_0Core.RM_DIGUP);
        Assert.Equal(1042, ObjMonMon38_0Core.RM_DIGUP_Line);
        Assert.Equal(394, ObjMonMon38_0Core.RM_DIGUP_OldValue);

        Assert.Equal(8335, ObjMonMon38_0Core.GuardStart);
        Assert.Equal(8338, ObjMonMon38_0Core.GuardEnd);
        Assert.Equal(8340, ObjMonMon38_0Core.CommentedIfLine);
        Assert.Equal(8341, ObjMonMon38_0Core.StoneBranchLine);
        Assert.Equal(8343, ObjMonMon38_0Core.LockLine);
        Assert.Equal(8344, ObjMonMon38_0Core.TryLine);
        Assert.Equal(8345, ObjMonMon38_0Core.LoopLine);
        Assert.Equal(8347, ObjMonMon38_0Core.ItemLine);
        Assert.Equal(8348, ObjMonMon38_0Core.NullCheckLine);
        Assert.Equal(8354, ObjMonMon38_0Core.DeathContinueLine);
        Assert.Equal(8355, ObjMonMon38_0Core.ProperTargetLine);
        Assert.Equal(8357, ObjMonMon38_0Core.HideFilterLine);
        Assert.Equal(8359, ObjMonMon38_0Core.RevealRangeLine);
        Assert.Equal(3, ObjMonMon38_0Core.RevealRange);
        Assert.Equal(8361, ObjMonMon38_0Core.DigCallLine);
        Assert.Equal(8362, ObjMonMon38_0Core.WalkTickLine);
        Assert.Equal(8363, ObjMonMon38_0Core.WalkDelayLine);
        Assert.Equal(1000, ObjMonMon38_0Core.WalkDelayValue);
        Assert.Equal(8364, ObjMonMon38_0Core.BreakLine);
        Assert.Equal(8370, ObjMonMon38_0Core.FinallyLine);
        Assert.Equal(8371, ObjMonMon38_0Core.UnlockLine);
        Assert.Equal(8374, ObjMonMon38_0Core.ElseLine);
        Assert.Equal(8376, ObjMonMon38_0Core.ThrottleLine);
        Assert.Equal(8000, ObjMonMon38_0Core.SearchWithTargetMs);
        Assert.Equal(1000, ObjMonMon38_0Core.SearchWithoutTargetMs);
        Assert.Equal(8380, ObjMonMon38_0Core.SearchTargetLine);
        Assert.Equal(8383, ObjMonMon38_0Core.AttackCallRunLine);
        Assert.Equal(8386, ObjMonMon38_0Core.FinalInheritedLine);

        Assert.Equal(8250, ObjMonMon38_0Core.DisabledParalysisStart);
        Assert.Equal(8255, ObjMonMon38_0Core.DisabledParalysisEnd);
        Assert.Equal(6, ObjMonMon38_0Core.DisabledParalysisLines);
        Assert.Equal(8120, ObjMonMon38_0Core.J232DisabledStart);
        Assert.Equal(2, ObjMonMon38_0Core.DisabledParalysisOccurrences);

        Assert.Equal(8204, ObjMonMon38_0Core.CommentedResistIfLine);
        Assert.Equal(8205, ObjMonMon38_0Core.CommentedBeginLine);
        Assert.Equal(8264, ObjMonMon38_0Core.CommentedEndLine);
        Assert.Equal(60, ObjMonMon38_0Core.ScatteredDistance);
        Assert.Equal(6414, ObjMonMon38_0Core.J219ResistLine);

        Assert.Equal(8190, ObjMonMon38_0Core.ListCreateLine);
        Assert.Equal(8191, ObjMonMon38_0Core.MagicTryLine);
        Assert.Equal(8268, ObjMonMon38_0Core.MagicFinallyLine);
        Assert.Equal(8269, ObjMonMon38_0Core.FreeLine);
        Assert.Equal(8192, ObjMonMon38_0Core.GetMapLine);
        Assert.Equal(8196, ObjMonMon38_0Core.SelfFilterLine);
        Assert.Equal(8271, ObjMonMon38_0Core.EffectLine);
        Assert.Equal(0, ObjMonMon38_0Core.EffectId);
        Assert.Equal(20102, ObjMonMon38_0Core.RM_LIGHTING);
        Assert.Equal(310, ObjMonMon38_0Core.VisibleActorsDeclLine);
        Assert.Equal("TKeyItemList", ObjMonMon38_0Core.VisibleActorsType);
        Assert.Equal("0x408", ObjMonMon38_0Core.OffsetComment);

        Assert.Equal(75, ObjMonMon38_0Core.ClassDeclLine);
        Assert.Equal(77, ObjMonMon38_0Core.RageFieldLine);
        Assert.Equal(78, ObjMonMon38_0Core.DigDeclLine);
        Assert.Equal(80, ObjMonMon38_0Core.CreateDeclLine);
        Assert.Equal(81, ObjMonMon38_0Core.AttackDeclLine);
        Assert.Equal(82, ObjMonMon38_0Core.RunDeclLine);
        Assert.Equal(85, ObjMonMon38_0Core.NextClassLine);
        Assert.Equal(8390, ObjMonMon38_0Core.NextImplLine);
        Assert.Equal(36, ObjMonMon38_0Core.ClassesCovered);
        Assert.Equal(18, ObjMonMon38_0Core.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonMon38_0Core.SpanMatches());
        Assert.True(ObjMonMon38_0Core.TotalLinesAddUp());
        Assert.True(ObjMonMon38_0Core.DecompositionAddsUp());
        Assert.True(ObjMonMon38_0Core.MethodsAscending());
        Assert.True(ObjMonMon38_0Core.MethodsContiguous());
        Assert.True(ObjMonMon38_0Core.WithinUnit());
        Assert.True(ObjMonMon38_0Core.NoInstrumentation());
    }

    // ===================== 一、外层体与恒真 =====================

    [Fact]
    public void OuterShapeFacts()
    {
        Assert.True(ObjMonMon38_0Core.NewOuterShape());
        Assert.True(ObjMonMon38_0Core.NotTheSharedTemplate());
        Assert.True(ObjMonMon38_0Core.NoSetTargetXYAtAll());
        Assert.True(ObjMonMon38_0Core.AttackOrDiscardOnly());
        Assert.True(ObjMonMon38_0Core.ConsistentWithCannotMove());
    }

    [Fact]
    public void TautologyFacts()
    {
        Assert.True(ObjMonMon38_0Core.TautologicalIfAt8294());
        Assert.True(ObjMonMon38_0Core.ExactDeMorganComplement());
        Assert.True(ObjMonMon38_0Core.TwoExitsBetweenThem());
        Assert.True(ObjMonMon38_0Core.RedundantStructure());
        Assert.True(ObjMonMon38_0Core.NewCauseOfTautology());
        Assert.True(ObjMonMon38_0Core.DifferentFromThresholdTautology());
        Assert.True(ObjMonMon38_0Core.ComplementsAlways());
        Assert.True(ObjMonMon38_0Core.OutOfRangeAlwaysTrueWhenReached());
        Assert.True(ObjMonMon38_0Core.IfCouldBeOmitted());
    }

    [Fact]
    public void RangeComplementBoundaries()
    {
        // **门：两轴都在 4 格内**
        Assert.True(ObjMonMon38_0Core.InRange(4, 4));
        Assert.False(ObjMonMon38_0Core.InRange(5, 0));

        // **取反：任一轴超 4 格**
        Assert.True(ObjMonMon38_0Core.OutOfRange(5, 0));
        Assert.False(ObjMonMon38_0Core.OutOfRange(4, 4));

        // **两者恒互补（已穷举 −8..8 全网格）**
        Assert.True(ObjMonMon38_0Core.ComplementsAlways());
    }

    [Fact]
    public void AxisCorrectnessFacts()
    {
        Assert.True(ObjMonMon38_0Core.BothAxesCorrectHere());
        Assert.True(ObjMonMon38_0Core.XThenY());
        Assert.True(ObjMonMon38_0Core.ContrastWithJ231WhichDuplicatedX());
        Assert.True(ObjMonMon38_0Core.BothAxesCorrectAgain());
    }

    [Fact]
    public void MapAndDiscardFacts()
    {
        Assert.True(ObjMonMon38_0Core.InvertedMapCheck());
        Assert.True(ObjMonMon38_0Core.HasOwnExit());
        Assert.True(ObjMonMon38_0Core.SplitIntoTwoStatements());
        Assert.True(ObjMonMon38_0Core.NoElseBranch());
        Assert.True(ObjMonMon38_0Core.DelTargetCreatTwice());
        Assert.True(ObjMonMon38_0Core.TwoDiscardPaths());
        Assert.True(ObjMonMon38_0Core.EffectivelyAlwaysDiscards());
    }

    [Fact]
    public void OuterOutcomeBoundaries()
    {
        Assert.True(ObjMonMon38_0Core.InRangeAttacks());
        Assert.True(ObjMonMon38_0Core.DifferentMapDiscards());
        Assert.True(ObjMonMon38_0Core.FarDiscards());
        Assert.True(ObjMonMon38_0Core.OnlyTwoOutcomes());

        Assert.Equal("attacked", ObjMonMon38_0Core.OuterOutcome(0, 0, true, true));
        Assert.Equal("discard", ObjMonMon38_0Core.OuterOutcome(9, 9, false, false));
        Assert.Equal("discard", ObjMonMon38_0Core.OuterOutcome(9, 9, true, false));
    }

    // ===================== 二、彻底退役的字段 =====================

    [Fact]
    public void RetiredFieldFacts()
    {
        Assert.True(ObjMonMon38_0Core.FixedHideFullyRetired());
        Assert.True(ObjMonMon38_0Core.FiveOccurrencesAllDisabled());
        Assert.True(ObjMonMon38_0Core.ThreeCommentOuts());
        Assert.True(ObjMonMon38_0Core.TwoInsideDisabledBlocks());
        Assert.True(ObjMonMon38_0Core.ReplacedByStoneMode());
        Assert.True(ObjMonMon38_0Core.DisguiseMechanismChanged());
        Assert.True(ObjMonMon38_0Core.ThreeTracksOfOneChange());
        Assert.True(ObjMonMon38_0Core.RetiredFieldLinesChecked());
        Assert.True(ObjMonMon38_0Core.CommentLinesChecked());
        Assert.True(ObjMonMon38_0Core.CommentsInCreateAndRun());
        Assert.True(ObjMonMon38_0Core.StoneModeFollowsComment());
        Assert.True(ObjMonMon38_0Core.CharStatusExAlsoSet());

        Assert.Equal(new[] { 8307, 8311, 8323, 8336, 8340 },
            ObjMonMon38_0Core.RetiredFieldLines);
        Assert.Equal(new[] { 8323, 8336, 8340 },
            ObjMonMon38_0Core.RetiredFieldCommentLines);
        Assert.Equal(new[] { 8307, 8311 },
            ObjMonMon38_0Core.RetiredFieldDisabledLines);
    }

    [Fact]
    public void StoneModeFacts()
    {
        Assert.True(ObjMonMon38_0Core.StoneModeIsOne());
        Assert.True(ObjMonMon38_0Core.StoneModeDeclChecked());
    }

    // ---------- NowDigUP ----------

    [Fact]
    public void NowDigUpFacts()
    {
        Assert.True(ObjMonMon38_0Core.OnlyFourLiveLines());
        Assert.True(ObjMonMon38_0Core.ThirteenDisabledLines());
        Assert.True(ObjMonMon38_0Core.TwoGenerationsDisabled());
        Assert.True(ObjMonMon38_0Core.LiveUnstonesAndSendsDigUp());
        Assert.True(ObjMonMon38_0Core.MultipleGenerationsStacked());
        Assert.True(ObjMonMon38_0Core.DigLinesAddUp());
        Assert.True(ObjMonMon38_0Core.DisabledAboveLive());
        Assert.True(ObjMonMon38_0Core.VarDeclDisabled());
        Assert.True(ObjMonMon38_0Core.BothGenerationsUsedRetiredField());
        Assert.True(ObjMonMon38_0Core.LiveLinesDontUseIt());
    }

    [Fact]
    public void DigUpConstantFacts()
    {
        Assert.True(ObjMonMon38_0Core.DigUpIs20099());
        Assert.True(ObjMonMon38_0Core.OldValueResidueComment());
        Assert.True(ObjMonMon38_0Core.OldDiffersFromNew());
        Assert.True(ObjMonMon38_0Core.DigUpDeclChecked());
        Assert.True(ObjMonMon38_0Core.SameFamilyAsJ214J220());
    }

    // ===================== 三、Run 的两态 =====================

    [Fact]
    public void GuardFacts()
    {
        Assert.True(ObjMonMon38_0Core.TwoGuardTermsCommented());
        Assert.True(ObjMonMon38_0Core.RequiredByTheMechanismChange());
        Assert.True(ObjMonMon38_0Core.OtherwiseNeverReveals());
        Assert.True(ObjMonMon38_0Core.AllFalseRuns());
        Assert.True(ObjMonMon38_0Core.DeathBlocks());
        Assert.True(ObjMonMon38_0Core.StoneDoesNotBlockGuard());

        Assert.Equal(new[] { 8336, 8337 }, ObjMonMon38_0Core.CommentedGuardLines);
        Assert.True(ObjMonMon38_0Core.CanRun(false, false, true));
        Assert.False(ObjMonMon38_0Core.CanRun(false, true, true));
    }

    [Fact]
    public void TwoModeFacts()
    {
        Assert.True(ObjMonMon38_0Core.TwoModeStateMachine());
        Assert.True(ObjMonMon38_0Core.StoneModeReveals());
        Assert.True(ObjMonMon38_0Core.NormalModeFights());
        Assert.True(ObjMonMon38_0Core.StatesAreComplements());
        Assert.True(ObjMonMon38_0Core.StonePicksReveal());
        Assert.True(ObjMonMon38_0Core.NotStonePicksFight());
        Assert.True(ObjMonMon38_0Core.ModesDiffer());

        Assert.Equal("reveal-scan", ObjMonMon38_0Core.PickMode(true));
        Assert.Equal("normal-fight", ObjMonMon38_0Core.PickMode(false));
    }

    [Fact]
    public void LockFacts()
    {
        Assert.True(ObjMonMon38_0Core.UsesLockTryFinally());
        Assert.True(ObjMonMon38_0Core.MostRegularResourcePattern());
        Assert.True(ObjMonMon38_0Core.TKeyItemListHasOwnLock());
        Assert.True(ObjMonMon38_0Core.LockBlockStructure());
    }

    [Fact]
    public void CascadeFacts()
    {
        Assert.True(ObjMonMon38_0Core.FourLevelCascade());
        Assert.True(ObjMonMon38_0Core.TwoContinuesOneNestedIf());
        Assert.True(ObjMonMon38_0Core.RangeThreeTiles());
        Assert.True(ObjMonMon38_0Core.ShouldReveal(3, 3));
        Assert.False(ObjMonMon38_0Core.ShouldReveal(4, 0));
        Assert.True(ObjMonMon38_0Core.ThreeReveals());
        Assert.True(ObjMonMon38_0Core.FourDoesNotReveal());
    }

    [Fact]
    public void RevealBoundaries()
    {
        Assert.True(ObjMonMon38_0Core.ShouldReveal(0, 0));
        Assert.True(ObjMonMon38_0Core.ShouldReveal(3, 0));
        Assert.True(ObjMonMon38_0Core.ShouldReveal(0, 3));
        Assert.False(ObjMonMon38_0Core.ShouldReveal(4, 0));
        Assert.False(ObjMonMon38_0Core.ShouldReveal(0, 4));
    }

    [Fact]
    public void RunTailFacts()
    {
        Assert.True(ObjMonMon38_0Core.WalkDelaySetToThousand());
        Assert.True(ObjMonMon38_0Core.DelayInsteadOfCooldown());
        Assert.True(ObjMonMon38_0Core.InheritedUnconditional());
        Assert.True(ObjMonMon38_0Core.OutsideAllBranches());
        Assert.True(ObjMonMon38_0Core.SameAsJ220());
        Assert.True(ObjMonMon38_0Core.BreakAfterCall());
        Assert.True(ObjMonMon38_0Core.BreakStopsScan());
    }

    [Fact]
    public void ThrottleBoundaries()
    {
        Assert.True(ObjMonMon38_0Core.TwoTierThrottle());
        Assert.True(ObjMonMon38_0Core.SearchAfterEightWithTarget());
        Assert.True(ObjMonMon38_0Core.SearchAfterOneWithoutTarget());
        Assert.True(ObjMonMon38_0Core.ExactlyEightBlocks());

        Assert.True(ObjMonMon38_0Core.ShouldSearch(8001, true));
        Assert.False(ObjMonMon38_0Core.ShouldSearch(8000, true));
        Assert.True(ObjMonMon38_0Core.ShouldSearch(1001, false));
        Assert.False(ObjMonMon38_0Core.ShouldSearch(1000, false));
    }

    // ---------- 被禁的麻痹块 ----------

    [Fact]
    public void DisabledParalysisFacts()
    {
        Assert.True(ObjMonMon38_0Core.SameDisabledBlockAsJ232());
        Assert.True(ObjMonMon38_0Core.ObjectNameSwapped());
        Assert.True(ObjMonMon38_0Core.TwoOccurrencesTotal());
        Assert.True(ObjMonMon38_0Core.CompletesJ232Foreshadow());
        Assert.True(ObjMonMon38_0Core.DisabledBlocksChecked());
    }

    // ---------- 散落的行注释禁令 ----------

    [Fact]
    public void ScatteredDisableFacts()
    {
        Assert.True(ObjMonMon38_0Core.ThreeScatteredLineComments());
        Assert.True(ObjMonMon38_0Core.SixtyLinesApart());
        Assert.True(ObjMonMon38_0Core.FifthKindOfDisable());
        Assert.True(ObjMonMon38_0Core.NeededToKeepStructureBalanced());
        Assert.True(ObjMonMon38_0Core.SameFixedTenFaceFormAsJ219());
        Assert.True(ObjMonMon38_0Core.NoResistFilterNow());
    }

    [Fact]
    public void ScatteredDistanceArithmetic()
    {
        Assert.Equal(60, ObjMonMon38_0Core.CommentedEndLine
            - ObjMonMon38_0Core.CommentedResistIfLine);
        Assert.True(ObjMonMon38_0Core.CommentedBeginLine
            == ObjMonMon38_0Core.CommentedResistIfLine + 1);
    }

    // ===================== 四、其余 =====================

    [Fact]
    public void ResourceFacts()
    {
        Assert.True(ObjMonMon38_0Core.HasTryFinally());
        Assert.True(ObjMonMon38_0Core.RuleHoldsHere());
        Assert.True(ObjMonMon38_0Core.ContrastWithJ230());
        Assert.True(ObjMonMon38_0Core.FreeInFinally());
    }

    [Fact]
    public void RadiusFacts()
    {
        Assert.True(ObjMonMon38_0Core.RadiusFromField());
        Assert.True(ObjMonMon38_0Core.TargetCentered());
        Assert.True(ObjMonMon38_0Core.SelfCenteredSecondFilter());
        Assert.True(ObjMonMon38_0Core.SameFieldUsedTwice());
        Assert.True(ObjMonMon38_0Core.FieldSetInCreate());
        Assert.True(ObjMonMon38_0Core.CommentFourByFourIsAccurate());
        Assert.True(ObjMonMon38_0Core.RageCommentChecked());
    }

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonMon38_0Core.EffectZero());
        Assert.True(ObjMonMon38_0Core.BaseIsTAnimalObject());
        Assert.True(ObjMonMon38_0Core.NoOverrideIsCorrect());
        Assert.True(ObjMonMon38_0Core.ChainRoot());
        Assert.True(ObjMonMon38_0Core.DeclLinesChecked());
        Assert.True(ObjMonMon38_0Core.OneCommentOutInCreate());
        Assert.True(ObjMonMon38_0Core.FixedHideIntendedThenStone());
        Assert.True(ObjMonMon38_0Core.CreateSetsFourFields());
        Assert.True(ObjMonMon38_0Core.ViewRangeIsSeven());
        Assert.True(ObjMonMon38_0Core.VisibleActorsTypeChecked());
        Assert.True(ObjMonMon38_0Core.HasOffsetAndTodoComment());
        Assert.True(ObjMonMon38_0Core.ThirtySixClassesCovered());
        Assert.True(ObjMonMon38_0Core.RemainingApprox());
        Assert.True(ObjMonMon38_0Core.NextClassIsMon38_11());
        Assert.True(ObjMonMon38_0Core.Mon38Family());
    }
}
