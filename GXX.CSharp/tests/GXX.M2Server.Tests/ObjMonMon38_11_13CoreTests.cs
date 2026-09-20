using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J234：`ObjMon.pas` 中两个类的 1:1 测试（237 行）：
/// `TMon38_11Monster.AttackTarget`（42 行）与
/// `TMon38_13Monster` 的 `AttackTarget`/`MagicAttack`/`MagicAttackGroup`（43+72+80）。
/// **本批最有价值的发现**：
/// ① 所谓"狂暴攻击"就是把 `AttackDir` 的 `AttackRate` 从 1 改成 2，
///    且特效号与之对应（1↔2、0↔1，即"倍率减一"）；
/// ② `m_wAppr <> 640` 是**唯一一次**用外观值做**排除**条件、且 `640` 全文件只此一见；
/// ③ `MagicAttackGroup` 的管线**缺 `GetPowerRateAdd`、多一个 `* 2`**；
/// ④ `boSelfRage` 选的是**圆心**（自己/目标）——"半径×圆心"表里第一次出现圆心可切换；
/// ⑤ J230 那条"两种过滤写法同函数并存"在本批**第二次**印证。
/// </summary>
public sealed class ObjMonMon38_11_13CoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(8390, ObjMonMon38_11_13Core.M11Start);
        Assert.Equal(8431, ObjMonMon38_11_13Core.M11End);
        Assert.Equal(42, ObjMonMon38_11_13Core.M11Lines);
        Assert.Equal(8434, ObjMonMon38_11_13Core.M13AttackStart);
        Assert.Equal(8476, ObjMonMon38_11_13Core.M13AttackEnd);
        Assert.Equal(43, ObjMonMon38_11_13Core.M13AttackLines);
        Assert.Equal(8478, ObjMonMon38_11_13Core.MagicStart);
        Assert.Equal(8549, ObjMonMon38_11_13Core.MagicEnd);
        Assert.Equal(72, ObjMonMon38_11_13Core.MagicLines);
        Assert.Equal(8550, ObjMonMon38_11_13Core.GroupStart);
        Assert.Equal(8629, ObjMonMon38_11_13Core.GroupEnd);
        Assert.Equal(80, ObjMonMon38_11_13Core.GroupLines);
        Assert.Equal(237, ObjMonMon38_11_13Core.TotalLines);
        Assert.Equal(4, ObjMonMon38_11_13Core.MethodCount);

        Assert.Equal(8392, ObjMonMon38_11_13Core.M11DirDeclLine);
        Assert.Equal(8397, ObjMonMon38_11_13Core.M11DirCheckLine);
        Assert.Equal(8399, ObjMonMon38_11_13Core.M11CooldownLine);
        Assert.Equal(8401, ObjMonMon38_11_13Core.M11HitTickLine);
        Assert.Equal(8402, ObjMonMon38_11_13Core.M11HitDelayLine);
        Assert.Equal(8403, ObjMonMon38_11_13Core.M11FocusTickLine);
        Assert.Equal(8404, ObjMonMon38_11_13Core.BerserkGateLine);
        Assert.Equal(3, ObjMonMon38_11_13Core.BerserkBound);
        Assert.Equal(640, ObjMonMon38_11_13Core.ApprExcluded);
        Assert.Equal(8406, ObjMonMon38_11_13Core.BerserkCommentLine);
        Assert.Equal(8407, ObjMonMon38_11_13Core.BerserkAttackLine);
        Assert.Equal(2, ObjMonMon38_11_13Core.BerserkRate);
        Assert.Equal(8408, ObjMonMon38_11_13Core.BerserkEffectLine);
        Assert.Equal(1, ObjMonMon38_11_13Core.BerserkEffectId);
        Assert.Equal(8412, ObjMonMon38_11_13Core.NormalAttackLine);
        Assert.Equal(1, ObjMonMon38_11_13Core.NormalRate);
        Assert.Equal(8413, ObjMonMon38_11_13Core.NormalEffectLine);
        Assert.Equal(0, ObjMonMon38_11_13Core.NormalEffectId);
        Assert.Equal(8415, ObjMonMon38_11_13Core.M11BreakSeizeLine);
        Assert.Equal(8417, ObjMonMon38_11_13Core.M11ResultLine);
        Assert.Equal(0, ObjMonMon38_11_13Core.AttackDirHitMode);
        Assert.Equal(1, ObjMonMon38_11_13Core.AttackDirDefaultRate);
        Assert.Equal(568, ObjMonMon38_11_13Core.AttackDirDeclLine);
        Assert.Equal(27466, ObjMonMon38_11_13Core.AttackDirImplLine);

        Assert.Equal(8448, ObjMonMon38_11_13Core.GroupRollLine);
        Assert.Equal(5, ObjMonMon38_11_13Core.GroupRollBound);
        Assert.Equal(8450, ObjMonMon38_11_13Core.GroupCallLine);
        Assert.True(ObjMonMon38_11_13Core.GroupSelfRage);
        Assert.Equal(8, ObjMonMon38_11_13Core.GroupRageArg);
        Assert.Equal(8452, ObjMonMon38_11_13Core.MagicRollLine);
        Assert.Equal(3, ObjMonMon38_11_13Core.MagicRollBound);
        Assert.Equal(8454, ObjMonMon38_11_13Core.MagicCallLine);
        Assert.Equal(8458, ObjMonMon38_11_13Core.M13MeleeLine);
        Assert.Equal(8459, ObjMonMon38_11_13Core.M13BreakSeizeLine);
        Assert.Equal(8462, ObjMonMon38_11_13Core.M13ResultLine);
        Assert.Equal(8625, ObjMonMon38_11_13Core.ThirdSemicolonOmitted);

        Assert.Equal(8487, ObjMonMon38_11_13Core.MagicDirLine);
        Assert.Equal(8496, ObjMonMon38_11_13Core.MagicRateAddLine);
        Assert.Equal(8498, ObjMonMon38_11_13Core.MagicNextDamageLine);
        Assert.Equal(8500, ObjMonMon38_11_13Core.MagicPowerMaxLine);
        Assert.Equal(8525, ObjMonMon38_11_13Core.MagicGuardLine);
        Assert.Equal(8536, ObjMonMon38_11_13Core.LineCommentLine);
        Assert.Equal(8537, ObjMonMon38_11_13Core.LineLoopLine);
        Assert.Equal(4, ObjMonMon38_11_13Core.LineTiles);
        Assert.Equal(8539, ObjMonMon38_11_13Core.LinePosLine);
        Assert.Equal(8540, ObjMonMon38_11_13Core.LineMoveLine);
        Assert.Equal(8541, ObjMonMon38_11_13Core.LineFilterLine);
        Assert.Equal(8544, ObjMonMon38_11_13Core.LineAttackLine);
        Assert.Equal(8547, ObjMonMon38_11_13Core.LineEffectLine);
        Assert.Equal(2, ObjMonMon38_11_13Core.LineEffectId);

        Assert.Equal(8558, ObjMonMon38_11_13Core.GroupListLine);
        Assert.Equal(8559, ObjMonMon38_11_13Core.GroupTryLine);
        Assert.Equal(8560, ObjMonMon38_11_13Core.SelfRageLine);
        Assert.Equal(8561, ObjMonMon38_11_13Core.SelfGetMapLine);
        Assert.Equal(8563, ObjMonMon38_11_13Core.TargetGetMapLine);
        Assert.Equal(8583, ObjMonMon38_11_13Core.MissingRateAddSlot);
        Assert.Equal(8584, ObjMonMon38_11_13Core.DoubleLine);
        Assert.Equal(2, ObjMonMon38_11_13Core.DoubleFactor);
        Assert.Equal(8585, ObjMonMon38_11_13Core.GroupNextDamageLine);
        Assert.Equal(8587, ObjMonMon38_11_13Core.GroupPowerMaxLine);
        Assert.Equal(8570, ObjMonMon38_11_13Core.RejectFilterLine);
        Assert.Equal(8573, ObjMonMon38_11_13Core.ConjunctFilterStart);
        Assert.Equal(8577, ObjMonMon38_11_13Core.ConjunctFilterEnd);
        Assert.Equal(8567, ObjMonMon38_11_13Core.GroupLoopLine);
        Assert.Equal(8617, ObjMonMon38_11_13Core.GroupReboundLine);
        Assert.Equal(8625, ObjMonMon38_11_13Core.GroupEffectLine);
        Assert.Equal(1, ObjMonMon38_11_13Core.GroupEffectId);
        Assert.Equal(8626, ObjMonMon38_11_13Core.GroupFinallyLine);
        Assert.Equal(8627, ObjMonMon38_11_13Core.GroupFreeLine);
        Assert.Equal(7853, ObjMonMon38_11_13Core.J230RejectFilterLine);

        Assert.Equal(85, ObjMonMon38_11_13Core.M11ClassDeclLine);
        Assert.Equal(87, ObjMonMon38_11_13Core.M11AttackDeclLine);
        Assert.Equal(98, ObjMonMon38_11_13Core.M13ClassDeclLine);
        Assert.Equal(100, ObjMonMon38_11_13Core.M13GroupDeclLine);
        Assert.Equal(101, ObjMonMon38_11_13Core.M13MagicDeclLine);
        Assert.Equal(103, ObjMonMon38_11_13Core.M13AttackDeclLine);
        Assert.Equal(8632, ObjMonMon38_11_13Core.NextImplLine);
        Assert.Equal(8698, ObjMonMon38_11_13Core.NextImpl2Line);
        Assert.Equal(38, ObjMonMon38_11_13Core.ClassesCovered);
        Assert.Equal(16, ObjMonMon38_11_13Core.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonMon38_11_13Core.SpanMatches());
        Assert.True(ObjMonMon38_11_13Core.TotalLinesAddUp());
        Assert.True(ObjMonMon38_11_13Core.MethodsAscending());
        Assert.True(ObjMonMon38_11_13Core.MethodsContiguous());
        Assert.True(ObjMonMon38_11_13Core.WithinUnit());
        Assert.True(ObjMonMon38_11_13Core.NoInstrumentation());
    }

    // ===================== 一、狂暴 = 双倍倍率 =====================

    [Fact]
    public void BerserkFacts()
    {
        Assert.True(ObjMonMon38_11_13Core.BerserkIsDoubleRate());
        Assert.True(ObjMonMon38_11_13Core.AttackRateFourthArg());
        Assert.True(ObjMonMon38_11_13Core.DefaultRateIsOne());
        Assert.True(ObjMonMon38_11_13Core.EffectIdCorrelatesWithRate());
        Assert.True(ObjMonMon38_11_13Core.RateMinusOneEqualsEffect());
        Assert.True(ObjMonMon38_11_13Core.CommentMatchesCode());
        Assert.True(ObjMonMon38_11_13Core.RateDoublesWhenBerserk());
        Assert.True(ObjMonMon38_11_13Core.EffectIncrementsWhenBerserk());
        Assert.True(ObjMonMon38_11_13Core.TwoDimensionsCorrespond());
        Assert.True(ObjMonMon38_11_13Core.AttackDirImplChecked());
        Assert.True(ObjMonMon38_11_13Core.HitModeIsZero());
    }

    [Fact]
    public void BerserkRateTable()
    {
        Assert.Equal(2, ObjMonMon38_11_13Core.AttackRate(true));
        Assert.Equal(1, ObjMonMon38_11_13Core.AttackRate(false));
        Assert.Equal(1, ObjMonMon38_11_13Core.EffectId(true));
        Assert.Equal(0, ObjMonMon38_11_13Core.EffectId(false));

        // **倍率减一 = 特效号**
        Assert.Equal(ObjMonMon38_11_13Core.AttackRate(true) - 1,
            ObjMonMon38_11_13Core.EffectId(true));
        Assert.Equal(ObjMonMon38_11_13Core.AttackRate(false) - 1,
            ObjMonMon38_11_13Core.EffectId(false));
    }

    [Fact]
    public void AppearanceFacts()
    {
        Assert.True(ObjMonMon38_11_13Core.AppearanceIsUsedNegatively());
        Assert.True(ObjMonMon38_11_13Core.OnlyOneOccurrenceFileWide());
        Assert.True(ObjMonMon38_11_13Core.ExclusionNotSelection());
        Assert.True(ObjMonMon38_11_13Core.FirstNegativeUse());
        Assert.True(ObjMonMon38_11_13Core.ContrastWith231And607());
        Assert.True(ObjMonMon38_11_13Core.ThreeDistinctApprValues());
        Assert.True(ObjMonMon38_11_13Core.OnlyThisOneIsNegative());

        Assert.Equal(3, ObjMonMon38_11_13Core.ApprValues.Length);
        Assert.Equal(231, ObjMonMon38_11_13Core.ApprValues[0].Value);
        Assert.Equal(607, ObjMonMon38_11_13Core.ApprValues[1].Value);
        Assert.Equal(640, ObjMonMon38_11_13Core.ApprValues[2].Value);
    }

    [Fact]
    public void BerserkBoundaries()
    {
        Assert.True(ObjMonMon38_11_13Core.AllTrueBerserks());
        Assert.True(ObjMonMon38_11_13Core.Appr640Excluded());
        Assert.True(ObjMonMon38_11_13Core.MissedRollGoesNormal());
        Assert.True(ObjMonMon38_11_13Core.BerserkPicksDouble());
        Assert.True(ObjMonMon38_11_13Core.OtherwiseNormal());

        Assert.True(ObjMonMon38_11_13Core.BerserkFires(0, 600));
        Assert.False(ObjMonMon38_11_13Core.BerserkFires(0, 640));
        Assert.False(ObjMonMon38_11_13Core.BerserkFires(1, 600));
        Assert.False(ObjMonMon38_11_13Core.BerserkFires(2, 600));

        Assert.Equal("berserk", ObjMonMon38_11_13Core.PickAttack(0, 600));
        Assert.Equal("normal", ObjMonMon38_11_13Core.PickAttack(0, 640));
        Assert.Equal("normal", ObjMonMon38_11_13Core.PickAttack(1, 600));
    }

    [Fact]
    public void SendAndBreakFacts()
    {
        Assert.True(ObjMonMon38_11_13Core.SendInsideEachBranch());
        Assert.True(ObjMonMon38_11_13Core.NotTheUnifiedSendForm());
        Assert.True(ObjMonMon38_11_13Core.TwoSendsOnePerBranch());
        Assert.True(ObjMonMon38_11_13Core.BreakSeizeSharedByBothBranches());
        Assert.True(ObjMonMon38_11_13Core.ContrastWithSiblingClass());
        Assert.True(ObjMonMon38_11_13Core.ResultOutsideCooldown());
        Assert.True(ObjMonMon38_11_13Core.M11CallOrder());
    }

    // ===================== 二、三段式概率分派 =====================

    [Fact]
    public void DispatchFacts()
    {
        Assert.True(ObjMonMon38_11_13Core.NestedProbabilityDispatch());
        Assert.True(ObjMonMon38_11_13Core.SecondRollOnlyIfFirstFails());
        Assert.True(ObjMonMon38_11_13Core.EffectiveProbabilities());
        Assert.True(ObjMonMon38_11_13Core.NotASingleRoll());
        Assert.True(ObjMonMon38_11_13Core.FirstZeroGroups());
        Assert.True(ObjMonMon38_11_13Core.SecondZeroMagic());
        Assert.True(ObjMonMon38_11_13Core.NeitherZeroMelee());
        Assert.True(ObjMonMon38_11_13Core.SecondRollIrrelevantWhenFirstZero());
    }

    [Fact]
    public void DispatchBoundaries()
    {
        Assert.Equal("group", ObjMonMon38_11_13Core.PickPath(0, 5));
        Assert.Equal("group", ObjMonMon38_11_13Core.PickPath(0, 0));
        Assert.Equal("magic", ObjMonMon38_11_13Core.PickPath(1, 0));
        Assert.Equal("melee", ObjMonMon38_11_13Core.PickPath(1, 1));
        Assert.Equal("melee", ObjMonMon38_11_13Core.PickPath(4, 2));
    }

    [Fact]
    public void ProbabilityArithmetic()
    {
        Assert.True(ObjMonMon38_11_13Core.ProbabilitiesSumToOne());
        Assert.True(ObjMonMon38_11_13Core.MeleeIsTheMajority());

        Assert.True(Math.Abs(ObjMonMon38_11_13Core.GroupProbability() - 0.2) < 1e-9);
        Assert.True(Math.Abs(ObjMonMon38_11_13Core.MagicProbability() - 4.0 / 15) < 1e-9);
        Assert.True(Math.Abs(ObjMonMon38_11_13Core.MeleeProbability() - 8.0 / 15) < 1e-9);
    }

    [Fact]
    public void SemicolonFacts()
    {
        Assert.True(ObjMonMon38_11_13Core.TrailingSemicolonOmitted());
        Assert.True(ObjMonMon38_11_13Core.LegalBeforeEnd());
        Assert.True(ObjMonMon38_11_13Core.MixedStyleInOneMethod());
        Assert.True(ObjMonMon38_11_13Core.ThreeSitesOmitIt());
        Assert.True(ObjMonMon38_11_13Core.SemicolonLinesChecked());

        Assert.Equal(new[] { 8450, 8454 },
            ObjMonMon38_11_13Core.SemicolonOmittedLines);
    }

    [Fact]
    public void BreakSeizePlacementFacts()
    {
        Assert.True(ObjMonMon38_11_13Core.BreakSeizeOnlyInMeleeBranch());
        Assert.True(ObjMonMon38_11_13Core.MagicPathsKeepSeize());
        Assert.True(ObjMonMon38_11_13Core.OppositeOfSibling());
    }

    [Fact]
    public void GroupFamilyFacts()
    {
        Assert.True(ObjMonMon38_11_13Core.CompletesTheGroupFamilyMember());
        Assert.True(ObjMonMon38_11_13Core.DeclAtLine100());
        Assert.True(ObjMonMon38_11_13Core.TwoParamVersion());
        Assert.True(ObjMonMon38_11_13Core.TwoTwoParamTwoFourParam());
        Assert.True(ObjMonMon38_11_13Core.GroupDeclLinesChecked());

        Assert.Equal(new[] { 54, 92, 100, 116 },
            ObjMonMon38_11_13Core.GroupDeclLines);
    }

    // ===================== 三、直线穿透 =====================

    [Fact]
    public void PipelineFacts()
    {
        Assert.True(ObjMonMon38_11_13Core.FiveStepPipeline());
        Assert.True(ObjMonMon38_11_13Core.SameAsJ228J232());
    }

    [Fact]
    public void LineAttackFacts()
    {
        Assert.True(ObjMonMon38_11_13Core.StraightLineFourTiles());
        Assert.True(ObjMonMon38_11_13Core.ExcludesPrimaryTarget());
        Assert.True(ObjMonMon38_11_13Core.ContrastWithJ229Beam());
        Assert.True(ObjMonMon38_11_13Core.NoPrimaryExclusionThere());
        Assert.True(ObjMonMon38_11_13Core.PrimaryNotHitAgain());
        Assert.True(ObjMonMon38_11_13Core.OthersHit());
        Assert.True(ObjMonMon38_11_13Core.NullSkipped());
        Assert.True(ObjMonMon38_11_13Core.LineCommentChecked());
        Assert.True(ObjMonMon38_11_13Core.LineCoversFour());
        Assert.True(ObjMonMon38_11_13Core.StartsAtOne());
    }

    [Fact]
    public void LineFilterBoundaries()
    {
        Assert.False(ObjMonMon38_11_13Core.IsHitByLine(true, true, true));
        Assert.True(ObjMonMon38_11_13Core.IsHitByLine(true, true, false));
        Assert.False(ObjMonMon38_11_13Core.IsHitByLine(false, true, false));
        Assert.False(ObjMonMon38_11_13Core.IsHitByLine(true, false, false));
    }

    [Fact]
    public void LineTileSet()
    {
        Assert.Equal(new[] { 1, 2, 3, 4 }, ObjMonMon38_11_13Core.LineTilesHit());
    }

    [Fact]
    public void CallFormFacts()
    {
        Assert.True(ObjMonMon38_11_13Core.PositionReturnDiscarded());
        Assert.True(ObjMonMon38_11_13Core.StatementForm());
        Assert.True(ObjMonMon38_11_13Core.J229UsedAsCondition());
        Assert.True(ObjMonMon38_11_13Core.NoCastOnGetMovingObject());
        Assert.True(ObjMonMon38_11_13Core.OthersDoCast());
        Assert.True(ObjMonMon38_11_13Core.WeakTypingReliedUpon());
    }

    [Fact]
    public void EffectIdFacts()
    {
        Assert.True(ObjMonMon38_11_13Core.EffectTwoForLine());
        Assert.True(ObjMonMon38_11_13Core.EffectOneForGroup());
        Assert.True(ObjMonMon38_11_13Core.NoneForMelee());
        Assert.True(ObjMonMon38_11_13Core.ThreeDistinctEffectIds());
    }

    // ===================== 四、boSelfRage 选圆心 =====================

    [Fact]
    public void CenterSelectionFacts()
    {
        Assert.True(ObjMonMon38_11_13Core.BooleanSelectsCenter());
        Assert.True(ObjMonMon38_11_13Core.TrueMeansSelf());
        Assert.True(ObjMonMon38_11_13Core.FalseMeansTarget());
        Assert.True(ObjMonMon38_11_13Core.RuntimeSelectableCenter());
        Assert.True(ObjMonMon38_11_13Core.NewCombinationInTheTable());
        Assert.True(ObjMonMon38_11_13Core.CallPassesTrue());
        Assert.True(ObjMonMon38_11_13Core.SelfCenteredHere());
        Assert.True(ObjMonMon38_11_13Core.RadiusFromArgument());
        Assert.True(ObjMonMon38_11_13Core.TwoCentersDiffer());
    }

    [Fact]
    public void CenterBoundaries()
    {
        Assert.Equal("self", ObjMonMon38_11_13Core.PickCenter(true));
        Assert.Equal("target", ObjMonMon38_11_13Core.PickCenter(false));
    }

    [Fact]
    public void GroupPipelineVariantFacts()
    {
        Assert.True(ObjMonMon38_11_13Core.MissingPowerRateAdd());
        Assert.True(ObjMonMon38_11_13Core.ExtraTimesTwo());
        Assert.True(ObjMonMon38_11_13Core.ThreeImplementationsThreePipelines());
        Assert.True(ObjMonMon38_11_13Core.TimesTwoBeforeCap());
        Assert.True(ObjMonMon38_11_13Core.SameSlotAsRateAdd());
        Assert.True(ObjMonMon38_11_13Core.DoubleActuallyDoubles());
        Assert.True(ObjMonMon38_11_13Core.NoDoubleKeepsValue());
    }

    [Fact]
    public void DoubleArithmetic()
    {
        Assert.Equal(200, ObjMonMon38_11_13Core.FinalDamage(100, false, true));
        Assert.Equal(100, ObjMonMon38_11_13Core.FinalDamage(100, false, false));
        Assert.Equal(2, ObjMonMon38_11_13Core.FinalDamage(1, false, true));
    }

    [Fact]
    public void SecondFilterConfirmationFacts()
    {
        Assert.True(ObjMonMon38_11_13Core.BothFilterFormsAgain());
        Assert.True(ObjMonMon38_11_13Core.NearlyIdenticalToJ230());
        Assert.True(ObjMonMon38_11_13Core.SecondConfirmation());
        Assert.True(ObjMonMon38_11_13Core.NotASingleOccurrence());
        Assert.True(ObjMonMon38_11_13Core.NoSecondFilter());
        Assert.True(ObjMonMon38_11_13Core.ParameterReplacesTheTwoLayerStructure());
    }

    [Fact]
    public void ResourceFacts()
    {
        Assert.True(ObjMonMon38_11_13Core.HasTryFinally());
        Assert.True(ObjMonMon38_11_13Core.FreeInFinally());
        Assert.True(ObjMonMon38_11_13Core.HasRebound());
        Assert.True(ObjMonMon38_11_13Core.ReboundUnderGuard());
    }

    // ===================== 五、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonMon38_11_13Core.BothDeriveFromTATMonster());
        Assert.True(ObjMonMon38_11_13Core.OnlyAttackTargetOverridden());
        Assert.True(ObjMonMon38_11_13Core.RunNotOverridden());
        Assert.True(ObjMonMon38_11_13Core.FamilyPattern());
        Assert.True(ObjMonMon38_11_13Core.TwoClassesClosedInOneBatch());
        Assert.True(ObjMonMon38_11_13Core.Mon38_13Closed());
        Assert.True(ObjMonMon38_11_13Core.Mon38_11Closed());
        Assert.True(ObjMonMon38_11_13Core.NextClassIsMon38_12());
        Assert.True(ObjMonMon38_11_13Core.HasFourParamVersion());
        Assert.True(ObjMonMon38_11_13Core.NextImpl2Checked());
        Assert.True(ObjMonMon38_11_13Core.DeclLinesChecked());
        Assert.True(ObjMonMon38_11_13Core.ThirtyEightClassesCovered());
        Assert.True(ObjMonMon38_11_13Core.RemainingApprox());
    }
}
