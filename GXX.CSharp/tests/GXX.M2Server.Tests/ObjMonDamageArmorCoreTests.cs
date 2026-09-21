using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J249：`ObjMon.pas` 中 `TDamageArmorAttackMonster`（狐狸魔法攻击 · 减防御）
/// 两方法的 1:1 测试（112 行）。
/// **本批最有价值的发现**：
/// ① `MagicAttackTarget` 里有一个 **75 行的嵌套过程** `MagicAttack`（占全文七成），
///    外层只剩 30 行守门；它是本系列**第二个**嵌套过程、也是**活的**那个；
/// ② 本处是"别名 vs `Max`"相关性的**第四种组合** —— **无别名 + 有 `Max`**
///    （`GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1))`）
///    ⇒ 至此四种组合在文件里**全部**出现，
///    别名是"省 `Max`"的**必要痕迹但非充分条件**；
/// ③ **"这次有没有破防"被编码进特效编号 `wMagicID`**（开头 1、破防改 2、
///    而特效消息在整段伤害/中毒/反弹都算完之后**才发**）；
/// ④ 外层是共享模板的**第 13 次确认**，但**同图分支多了一层 `Abs > 6` 守卫**
///    （模板原版是无条件 `SetTargetXY`）。
/// **里程碑**：本批完成后 J241 覆盖率表的候选未移植类**清零**。
/// </summary>
public sealed class ObjMonDamageArmorCoreTests
{
    [Fact]
    public void Constants()
    {
        Assert.Equal(6261, ObjMonDamageArmorCore.AttackStart);
        Assert.Equal(6368, ObjMonDamageArmorCore.AttackEnd);
        Assert.Equal(108, ObjMonDamageArmorCore.AttackLines);
        Assert.Equal(6263, ObjMonDamageArmorCore.NestedStart);
        Assert.Equal(6337, ObjMonDamageArmorCore.NestedEnd);
        Assert.Equal(75, ObjMonDamageArmorCore.NestedLines);
        Assert.Equal(6339, ObjMonDamageArmorCore.OuterBeginLine);
        Assert.Equal(30, ObjMonDamageArmorCore.OuterLines);
        Assert.Equal(6370, ObjMonDamageArmorCore.RunStart);
        Assert.Equal(6373, ObjMonDamageArmorCore.RunEnd);
        Assert.Equal(4, ObjMonDamageArmorCore.RunLines);
        Assert.Equal(112, ObjMonDamageArmorCore.TotalLines);
        Assert.Equal(2, ObjMonDamageArmorCore.MethodCount);
        Assert.Equal(1, ObjMonDamageArmorCore.ClassCount);

        Assert.Equal(6351, ObjMonDamageArmorCore.CallSiteLine);
        Assert.Equal(6341, ObjMonDamageArmorCore.NilGuardLine);
        Assert.Equal(6274, ObjMonDamageArmorCore.PowerLine);
        Assert.Equal(6272, ObjMonDamageArmorCore.MagicIdInitLine);
        Assert.Equal(1, ObjMonDamageArmorCore.MagicIdInitial);
        Assert.Equal(6322, ObjMonDamageArmorCore.MagicIdSetLine);
        Assert.Equal(2, ObjMonDamageArmorCore.MagicIdBroken);
        Assert.Equal(6319, ObjMonDamageArmorCore.ArmorGateLine);
        Assert.Equal(3, ObjMonDamageArmorCore.ArmorGateBound);
        Assert.Equal(6321, ObjMonDamageArmorCore.ZeroArmorLine);
        Assert.Equal(3, ObjMonDamageArmorCore.ArmorReductionBound);
        Assert.Equal(1, ObjMonDamageArmorCore.ArmorReductionBase);
        Assert.Equal(6336, ObjMonDamageArmorCore.EffectLine);

        Assert.Equal(6278, ObjMonDamageArmorCore.PipelineStart);
        Assert.Equal(6310, ObjMonDamageArmorCore.PipelineEnd);
        Assert.Equal(6313, ObjMonDamageArmorCore.GetBackLine);
        Assert.Equal(200, ObjMonDamageArmorCore.FirstDelay);
        Assert.Equal(200, ObjMonDamageArmorCore.SecondDelay);
        Assert.Equal("FT", ObjMonDamageArmorCore.ReboundTag);
        Assert.Equal(6324, ObjMonDamageArmorCore.ParalysisLine);

        Assert.Equal(6343, ObjMonDamageArmorCore.CooldownLine);
        Assert.Equal(6347, ObjMonDamageArmorCore.RangeGateLine);
        Assert.Equal(6, ObjMonDamageArmorCore.RangeRadius);
        Assert.Equal(6349, ObjMonDamageArmorCore.ProbGateLine);
        Assert.Equal(6360, ObjMonDamageArmorCore.SetTargetLine);
        Assert.Equal(6358, ObjMonDamageArmorCore.SetTargetGuardLine);
        Assert.Equal(6365, ObjMonDamageArmorCore.DelTargetLine);
        Assert.Equal(13, ObjMonDamageArmorCore.TemplateConfirmations);
        Assert.Equal(6352, ObjMonDamageArmorCore.ResultTrueLine);

        Assert.Equal(6, ObjMonDamageArmorCore.SiblingRunLines);
        Assert.Equal(55, ObjMonDamageArmorCore.TotalRealClasses);
        Assert.Equal(54, ObjMonDamageArmorCore.PortedBefore);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonDamageArmorCore.SpanMatches());
        Assert.True(ObjMonDamageArmorCore.TotalLinesAddUp());
        Assert.True(ObjMonDamageArmorCore.MethodsAscending());
        Assert.True(ObjMonDamageArmorCore.MethodsContiguous());
        Assert.True(ObjMonDamageArmorCore.WithinUnit());
        Assert.True(ObjMonDamageArmorCore.NoInstrumentation());
    }

    // ===================== 一、嵌套过程 =====================

    [Fact]
    public void NestedProcedureFacts()
    {
        Assert.True(ObjMonDamageArmorCore.NestedProcedureIsTheBody());
        Assert.True(ObjMonDamageArmorCore.SeventyFiveOfOneOhEight());
        Assert.True(ObjMonDamageArmorCore.OuterIsJustGates());
        Assert.True(ObjMonDamageArmorCore.SecondNestedProcedureIsAlive());
        Assert.True(ObjMonDamageArmorCore.FirstWasDead());
        Assert.True(ObjMonDamageArmorCore.CapturesTargetByScope());
        Assert.True(ObjMonDamageArmorCore.CalledExactlyOnce());
        Assert.True(ObjMonDamageArmorCore.NoParameters());
        Assert.True(ObjMonDamageArmorCore.OuterGuardsNull());
        Assert.True(ObjMonDamageArmorCore.InnerHasNoOwnGuard());
        Assert.True(ObjMonDamageArmorCore.OrderDependencyByConvention());
        Assert.True(ObjMonDamageArmorCore.NestedInsideOuter());
        Assert.True(ObjMonDamageArmorCore.OuterFollowsNested());
    }

    [Fact]
    public void NestedProportionBoundaries()
    {
        // **75 / 108 ≈ 69%（整数除法）**
        Assert.Equal(69, ObjMonDamageArmorCore.NestedLines * 100
            / ObjMonDamageArmorCore.AttackLines);
        Assert.True(ObjMonDamageArmorCore.NestedLines > ObjMonDamageArmorCore.OuterLines);
    }

    // ===================== 二、第四种组合 =====================

    [Fact]
    public void FourthCombinationFacts()
    {
        Assert.True(ObjMonDamageArmorCore.NoAliasWithMax());
        Assert.True(ObjMonDamageArmorCore.FourthCombination());
        Assert.True(ObjMonDamageArmorCore.AllFourCombinationsPresent());
        Assert.True(ObjMonDamageArmorCore.AliasIsNecessaryNotSufficient());
        Assert.True(ObjMonDamageArmorCore.ClosesTheRefinement());
        Assert.True(ObjMonDamageArmorCore.FourCombinationsListed());
        Assert.True(ObjMonDamageArmorCore.ThisOneIsNoAliasWithMax());
        Assert.True(ObjMonDamageArmorCore.UsesDcHere());
        Assert.True(ObjMonDamageArmorCore.McElsewhere());
        Assert.True(ObjMonDamageArmorCore.TwoAbilityPairsAgain());

        Assert.Equal(4, ObjMonDamageArmorCore.Combinations.Length);
        Assert.False(ObjMonDamageArmorCore.Combinations[2].Alias);
        Assert.True(ObjMonDamageArmorCore.Combinations[2].HasMax);
    }

    [Fact]
    public void PowerBoundaries()
    {
        Assert.True(ObjMonDamageArmorCore.DifferWhenInverted());
        Assert.True(ObjMonDamageArmorCore.SameWhenNormal());

        Assert.Equal(5, ObjMonDamageArmorCore.PowerNoMax(10, 5));
        Assert.Equal(11, ObjMonDamageArmorCore.PowerWithMax(10, 5));
        Assert.Equal(ObjMonDamageArmorCore.PowerNoMax(5, 10),
            ObjMonDamageArmorCore.PowerWithMax(5, 10));
    }

    // ===================== 三、wMagicID =====================

    [Fact]
    public void MagicIdFacts()
    {
        Assert.True(ObjMonDamageArmorCore.MagicIdSetToOne());
        Assert.True(ObjMonDamageArmorCore.SetToTwoOnArmorBreak());
        Assert.True(ObjMonDamageArmorCore.MessageSentLast());
        Assert.True(ObjMonDamageArmorCore.EncodesWhetherArmorBroke());
        Assert.True(ObjMonDamageArmorCore.ClientFacingEffectId());
        Assert.True(ObjMonDamageArmorCore.BrokenGivesTwo());
        Assert.True(ObjMonDamageArmorCore.IntactGivesOne());
        Assert.True(ObjMonDamageArmorCore.TwoDistinctIds());

        Assert.Equal(2, ObjMonDamageArmorCore.MagicId(true));
        Assert.Equal(1, ObjMonDamageArmorCore.MagicId(false));
    }

    [Fact]
    public void ArmorBreakFacts()
    {
        Assert.True(ObjMonDamageArmorCore.OneInThreeToBreakArmor());
        Assert.True(ObjMonDamageArmorCore.ArmorReducedByOneToThree());
        Assert.True(ObjMonDamageArmorCore.SiblingsDifferByThisBlock());
        Assert.True(ObjMonDamageArmorCore.ArmorReductionRange());

        Assert.True(ObjMonDamageArmorCore.BreaksArmor(0));
        Assert.False(ObjMonDamageArmorCore.BreaksArmor(1));
        Assert.False(ObjMonDamageArmorCore.BreaksArmor(2));

        Assert.Equal(1, ObjMonDamageArmorCore.ArmorReduction(0));
        Assert.Equal(3, ObjMonDamageArmorCore.ArmorReduction(2));
    }

    // ---------- 伤害管线 ----------

    [Fact]
    public void PipelineFacts()
    {
        Assert.True(ObjMonDamageArmorCore.FifthCopyOfTheDamagePipeline());
        Assert.True(ObjMonDamageArmorCore.CommentsVerbatimToo());
        Assert.True(ObjMonDamageArmorCore.SixSteps());
        Assert.True(ObjMonDamageArmorCore.MpLowByteAsDivisor());
        Assert.True(ObjMonDamageArmorCore.SamePositionAsJ248());
        Assert.True(ObjMonDamageArmorCore.DelayTwoHundred());
        Assert.True(ObjMonDamageArmorCore.ContrastsWithJ242J245sThreeHundred());
        Assert.True(ObjMonDamageArmorCore.ReboundHasFtTag());
        Assert.True(ObjMonDamageArmorCore.CompleteParalysisForm());

        Assert.Equal(6, ObjMonDamageArmorCore.PipelineSteps.Length);
    }

    [Fact]
    public void RecoveryBoundaries()
    {
        Assert.True(ObjMonDamageArmorCore.ZeroMpNoRecovery());
        Assert.True(ObjMonDamageArmorCore.NonZeroMpRecovers());

        Assert.Equal(100, ObjMonDamageArmorCore.RecoveredHp(100, 100, 0));
        Assert.Equal(110, ObjMonDamageArmorCore.RecoveredHp(100, 100, 10));
        Assert.Equal(101, ObjMonDamageArmorCore.RecoveredHp(100, 15, 10));
    }

    [Fact]
    public void ParalysisBoundaries()
    {
        Assert.True(ObjMonDamageArmorCore.ZeroResistParalyses());
        Assert.True(ObjMonDamageArmorCore.NonZeroResistBlocks());
        Assert.True(ObjMonDamageArmorCore.ImmuneBlocks());

        Assert.True(ObjMonDamageArmorCore.Paralyses(true, true, 100, 0));
        Assert.False(ObjMonDamageArmorCore.Paralyses(true, true, 100, 1));
        Assert.False(ObjMonDamageArmorCore.Paralyses(false, true, 100, 0));
    }

    // ===================== 四、外层模板 =====================

    [Fact]
    public void TemplateFacts()
    {
        Assert.True(ObjMonDamageArmorCore.ThirteenthTemplateConfirmation());
        Assert.True(ObjMonDamageArmorCore.BothGatesPresent());
        Assert.True(ObjMonDamageArmorCore.ExtraDistanceGuardInSameMapBranch());
        Assert.True(ObjMonDamageArmorCore.TemplateSetsUnconditionally());
        Assert.True(ObjMonDamageArmorCore.FourthTemplateVariant());
        Assert.True(ObjMonDamageArmorCore.DuplicateAbsComputation());
        Assert.True(ObjMonDamageArmorCore.EquivalentButRedundant());
        Assert.True(ObjMonDamageArmorCore.ApproachInsideCooldown());
        Assert.True(ObjMonDamageArmorCore.BoundToAttackCadence());
        Assert.True(ObjMonDamageArmorCore.SameBindingStyleAsJ247());
        Assert.True(ObjMonDamageArmorCore.ReturnsTrueInsideTheGates());
        Assert.True(ObjMonDamageArmorCore.FallsThroughWhenGatesFail());
    }

    [Fact]
    public void RangeGateBoundaries()
    {
        Assert.True(ObjMonDamageArmorCore.SixInRange());
        Assert.True(ObjMonDamageArmorCore.SevenOut());

        Assert.True(ObjMonDamageArmorCore.InRange(6, 6));
        Assert.False(ObjMonDamageArmorCore.InRange(7, 0));
        Assert.False(ObjMonDamageArmorCore.InRange(0, 7));
    }

    [Fact]
    public void ProbGateBoundaries()
    {
        Assert.True(ObjMonDamageArmorCore.SentinleForcesAttack());
        Assert.True(ObjMonDamageArmorCore.HalfChanceOtherwise());

        Assert.True(ObjMonDamageArmorCore.ProbGate(-1, 1));
        Assert.True(ObjMonDamageArmorCore.ProbGate(5, 0));
        Assert.False(ObjMonDamageArmorCore.ProbGate(5, 1));
    }

    // ===================== 五、其余 =====================

    [Fact]
    public void SiblingFacts()
    {
        Assert.True(ObjMonDamageArmorCore.FourLineRunIsTheShortest());
        Assert.True(ObjMonDamageArmorCore.SiblingRunIsSix());
        Assert.True(ObjMonDamageArmorCore.SiblingRunChecked());
        Assert.True(ObjMonDamageArmorCore.AllBehaviourInMagicAttackTarget());
        Assert.True(ObjMonDamageArmorCore.ThreeIdenticalDeclarations());
        Assert.True(ObjMonDamageArmorCore.OnlyCommentsDiffer());
        Assert.True(ObjMonDamageArmorCore.TripletClasses());
        Assert.True(ObjMonDamageArmorCore.CommentsDistinct());
        Assert.True(ObjMonDamageArmorCore.DeclLinesChecked());
        Assert.True(ObjMonDamageArmorCore.ThisIsTheThird());

        Assert.Equal(new[] { 6255, 6260 }, ObjMonDamageArmorCore.SiblingRunSpan);
        Assert.Equal(new[] { "TFoxMagicAttackMonster", "TDamageSpellAttackMonster", "TDamageArmorAttackMonster" },
            ObjMonDamageArmorCore.SiblingClasses);
        Assert.Equal(new[] { 207, 213, 219 }, ObjMonDamageArmorCore.DeclLines);
        Assert.Equal(3, ObjMonDamageArmorCore.ClassComments.Length);
    }

    [Fact]
    public void SiblingLineCountBoundaries()
    {
        Assert.True(ObjMonDamageArmorCore.AttackLinesDifferByTwo());

        Assert.Equal(new[] { 110, 6, 108, 4 }, ObjMonDamageArmorCore.SiblingMethodLines);
        Assert.Equal(2, ObjMonDamageArmorCore.SiblingMethodLines[0]
            - ObjMonDamageArmorCore.SiblingMethodLines[2]);
        Assert.Equal(2, ObjMonDamageArmorCore.SiblingMethodLines[1]
            - ObjMonDamageArmorCore.SiblingMethodLines[3]);
    }

    // ---------- 里程碑 ----------

    [Fact]
    public void MilestoneFacts()
    {
        Assert.True(ObjMonDamageArmorCore.ConsistentWithJ244ToJ248());
        Assert.True(ObjMonDamageArmorCore.LastCandidatePorted());
        Assert.True(ObjMonDamageArmorCore.PendingListNowEmpty());
        Assert.True(ObjMonDamageArmorCore.OnlyProvenFalsePositivesRemain());
        Assert.True(ObjMonDamageArmorCore.FullReauditDue());
        Assert.True(ObjMonDamageArmorCore.TwoFalsePositivesRecorded());
        Assert.True(ObjMonDamageArmorCore.AllPortedAfterThis());

        Assert.Equal(new[] { ("TDevilBat", 521), ("TDevilkingMonster", 505) },
            ObjMonDamageArmorCore.FalsePositives);
        Assert.Equal(ObjMonDamageArmorCore.TotalRealClasses,
            ObjMonDamageArmorCore.PortedBefore + 1);
    }
}
