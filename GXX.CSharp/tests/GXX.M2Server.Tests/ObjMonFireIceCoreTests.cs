using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J211：`ObjMon.pas` 中 `TFireIceAttackMonster`（寒冰掌怪物）
/// 两个方法 1:1 测试（合计 123 行）。
/// **本批最有价值的发现**：嵌套过程的第一件事是幽灵/死亡守卫（5163），
/// 而特效发送（5238）在守卫之内 ——
/// **目标已死或为幽灵时连特效都不播**；
/// 对照 J203-J210，它们的特效发送都在所有守卫之外。
/// 另：本类独有的"推动目标"段，其服务端循环被注释掉且引用未声明标识符。
/// </summary>
public sealed class ObjMonFireIceCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(5152, ObjMonFireIceCore.MagicStart);
        Assert.Equal(5270, ObjMonFireIceCore.MagicEnd);
        Assert.Equal(119, ObjMonFireIceCore.MagicLines);
        Assert.Equal(5154, ObjMonFireIceCore.NestedStart);
        Assert.Equal(5239, ObjMonFireIceCore.NestedEnd);
        Assert.Equal(86, ObjMonFireIceCore.NestedLines);
        Assert.Equal(5241, ObjMonFireIceCore.OuterStart);
        Assert.Equal(5270, ObjMonFireIceCore.OuterEnd);
        Assert.Equal(30, ObjMonFireIceCore.OuterLines);
        Assert.Equal(5272, ObjMonFireIceCore.RunStart);
        Assert.Equal(5275, ObjMonFireIceCore.RunEnd);
        Assert.Equal(4, ObjMonFireIceCore.RunLines);
        Assert.Equal(123, ObjMonFireIceCore.TotalLines);

        Assert.Equal(5163, ObjMonFireIceCore.GhostGuardLine);
        Assert.Equal(5164, ObjMonFireIceCore.GhostGuardExitLine);
        Assert.Equal(5165, ObjMonFireIceCore.DirectionLine);
        Assert.Equal(5243, ObjMonFireIceCore.OuterNilGuardLine);
        Assert.Equal(5254, ObjMonFireIceCore.OuterResultTrueLine);

        Assert.Equal(5169, ObjMonFireIceCore.HealTestLine);
        Assert.Equal(5172, ObjMonFireIceCore.HealCallLine);
        Assert.Equal(5173, ObjMonFireIceCore.HealEffectLine);
        Assert.Equal(5171, ObjMonFireIceCore.OldHealCommentLine);
        Assert.Equal(2, ObjMonFireIceCore.HealEffectId);
        Assert.Equal(800, ObjMonFireIceCore.OldHealDelayMs);
        Assert.Equal(4748, ObjMonFireIceCore.J207HealTestLine);
        Assert.Equal(231, ObjMonFireIceCore.J207ApprGate);

        Assert.Equal(5229, ObjMonFireIceCore.PushTestLine);
        Assert.Equal(5231, ObjMonFireIceCore.PushCountLine);
        Assert.Equal(5232, ObjMonFireIceCore.PushSendLine);
        Assert.Equal(5234, ObjMonFireIceCore.CommentedLoopLine1);
        Assert.Equal(5235, ObjMonFireIceCore.CommentedLoopLine2);
        Assert.Equal(600, ObjMonFireIceCore.PushDelayMs);
        Assert.Equal(200, ObjMonFireIceCore.DamageDelayMs);
        Assert.Equal(30008, ObjMonFireIceCore.RM_DELAYPUSHED);

        Assert.Equal(44, ObjMonFireIceCore.IcePalmEffectId);
        Assert.Equal(5238, ObjMonFireIceCore.EffectSendLine);
        Assert.Equal(5185, ObjMonFireIceCore.CapLine);
        Assert.Equal(5191, ObjMonFireIceCore.AbsorbLine);
        Assert.Equal(5, ObjMonFireIceCore.POISON_STONE);
        Assert.Equal(6, ObjMonFireIceCore.HealOccurrence);
        Assert.Equal(5, ObjMonFireIceCore.CapBeforeCount);
        Assert.Equal(1, ObjMonFireIceCore.CapAfterCount);
        Assert.Equal(16, ObjMonFireIceCore.ClassesCovered);
        Assert.Equal(38, ObjMonFireIceCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonFireIceCore.SpanMatches());
        Assert.True(ObjMonFireIceCore.TotalLinesAddUp());
        Assert.True(ObjMonFireIceCore.DecompositionAddsUp());
        Assert.True(ObjMonFireIceCore.NestedBeforeOuter());
        Assert.True(ObjMonFireIceCore.RunAfterMagic());
        Assert.True(ObjMonFireIceCore.WithinUnit());
        Assert.True(ObjMonFireIceCore.NoInstrumentation());
        Assert.True(ObjMonFireIceCore.NestedIsEightySix());
        Assert.True(ObjMonFireIceCore.BetweenJ207AndJ210());
    }

    // ===================== 一、幽灵/死亡守卫 =====================

    [Fact]
    public void GuardFacts()
    {
        Assert.True(ObjMonFireIceCore.GuardIsFirstThing());
        Assert.True(ObjMonFireIceCore.EffectInsideGuard());
        Assert.True(ObjMonFireIceCore.NoEffectForDeadTarget());
        Assert.True(ObjMonFireIceCore.OuterHasNoGhostCheck());
        Assert.True(ObjMonFireIceCore.ResultTrueDespiteNoop());
        Assert.True(ObjMonFireIceCore.SameFamilyAsJ209());
        Assert.True(ObjMonFireIceCore.GuardPlacementTooBroad());
        Assert.True(ObjMonFireIceCore.SuppressesCosmeticToo());
        Assert.True(ObjMonFireIceCore.DiffersFromJ203ToJ210());
        Assert.True(ObjMonFireIceCore.ResultTrueOutsideGuard());
        Assert.True(ObjMonFireIceCore.GuardBeforeDirection());
        Assert.True(ObjMonFireIceCore.OuterGuardIsNilOnly());
    }

    [Fact]
    public void GuardBoundaries()
    {
        Assert.True(ObjMonFireIceCore.NormalTargetActs());
        Assert.True(ObjMonFireIceCore.GhostBlocked());
        Assert.True(ObjMonFireIceCore.DeathBlocked());
        Assert.True(ObjMonFireIceCore.BothBlocked());

        Assert.True(ObjMonFireIceCore.CanAct(false, false));
        Assert.False(ObjMonFireIceCore.CanAct(true, false));
        Assert.False(ObjMonFireIceCore.CanAct(false, true));
        Assert.False(ObjMonFireIceCore.CanAct(true, true));
    }

    // ===================== 二、自愈 =====================

    [Fact]
    public void HealFacts()
    {
        Assert.True(ObjMonFireIceCore.SameThresholdAsJ207());
        Assert.True(ObjMonFireIceCore.SameRollBound());
        Assert.True(ObjMonFireIceCore.NoApprTestHere());
        Assert.True(ObjMonFireIceCore.UniversalNotExclusive());
        Assert.True(ObjMonFireIceCore.OmittedConditionNotJustValue());
        Assert.True(ObjMonFireIceCore.AttackPowerAsHeal());
        Assert.True(ObjMonFireIceCore.VerbatimSameAsJ207());
        Assert.True(ObjMonFireIceCore.SecondOccurrence());
        Assert.True(ObjMonFireIceCore.HealEffectIsTwo());
        Assert.True(ObjMonFireIceCore.CenteredOnSelf());
        Assert.True(ObjMonFireIceCore.CommentBeforeCall());
    }

    [Fact]
    public void HealBoundaries()
    {
        Assert.True(ObjMonFireIceCore.LowHpRollsZeroHeals());
        Assert.True(ObjMonFireIceCore.ExactlyHalfBlocks());
        Assert.True(ObjMonFireIceCore.MissedRollNoHeal());

        Assert.True(ObjMonFireIceCore.CanHeal(40, 100, 0));
        Assert.True(ObjMonFireIceCore.CanHeal(49, 100, 0));
        Assert.False(ObjMonFireIceCore.CanHeal(50, 100, 0));
        Assert.False(ObjMonFireIceCore.CanHeal(40, 100, 1));
        Assert.False(ObjMonFireIceCore.CanHeal(40, 100, 2));
    }

    [Fact]
    public void HealRoundingFacts()
    {
        Assert.True(ObjMonFireIceCore.FloatDivisionThenRound());
        Assert.True(ObjMonFireIceCore.BankersRounding());
        Assert.True(ObjMonFireIceCore.OddMaxHpFavoursLower());
        Assert.True(ObjMonFireIceCore.EvenMaxHpExact());
        Assert.True(ObjMonFireIceCore.SameAsJ207Rounding());

        // **银行家舍入：101/2 = 50.5 -> 50（偶数端）**
        Assert.Equal(50, ObjMonFireIceCore.HealThreshold(101));
        Assert.Equal(50, ObjMonFireIceCore.HealThreshold(100));
        Assert.Equal(52, ObjMonFireIceCore.HealThreshold(103));
    }

    [Fact]
    public void OldHealFormFacts()
    {
        Assert.True(ObjMonFireIceCore.OldHealWasDelayMsg());
        Assert.True(ObjMonFireIceCore.DelayWas800());
        Assert.True(ObjMonFireIceCore.DroppedTheAmount());
        Assert.True(ObjMonFireIceCore.DowngradedToCosmetic());
    }

    // ===================== 三、推动 =====================

    [Fact]
    public void PushFacts()
    {
        Assert.True(ObjMonFireIceCore.PushIsUniqueToThisClass());
        Assert.True(ObjMonFireIceCore.SixBatchesLackIt());
        Assert.True(ObjMonFireIceCore.SecondRollAfterDamage());
        Assert.True(ObjMonFireIceCore.NeverZero());
        Assert.True(ObjMonFireIceCore.TwoIndependentRolls());
        Assert.True(ObjMonFireIceCore.PushViaClientMessage());
        Assert.True(ObjMonFireIceCore.ServerLoopCommentedOut());
        Assert.True(ObjMonFireIceCore.TwoUndeclaredIdentifiers());
        Assert.True(ObjMonFireIceCore.PushDelay600());
        Assert.True(ObjMonFireIceCore.DamageDelay200());
        Assert.True(ObjMonFireIceCore.PushAfterDamage());
        Assert.True(ObjMonFireIceCore.ClientSideInterpretation());
        Assert.True(ObjMonFireIceCore.NoServerSideValidation());
    }

    [Fact]
    public void PushCountBoundaries()
    {
        Assert.True(ObjMonFireIceCore.RollZeroGivesOne());
        Assert.True(ObjMonFireIceCore.RollOneGivesOne());
        Assert.True(ObjMonFireIceCore.RollTwoGivesTwo());
        Assert.True(ObjMonFireIceCore.ValueRangeIsOneTwo());
        Assert.True(ObjMonFireIceCore.P1IsTwoThirds());
        Assert.True(ObjMonFireIceCore.P2IsOneThird());

        // **`Max(Random(3), 1)`：0 被抬成 1，故值域 {1,2}**
        Assert.Equal(1, ObjMonFireIceCore.PushCount(0));
        Assert.Equal(1, ObjMonFireIceCore.PushCount(1));
        Assert.Equal(2, ObjMonFireIceCore.PushCount(2));
    }

    [Fact]
    public void JointDistributionFacts()
    {
        Assert.True(ObjMonFireIceCore.JointDistributionIsSixTwoOne());
        Assert.True(ObjMonFireIceCore.TotalCombinationsAreNine());
        Assert.True(ObjMonFireIceCore.JointProbabilityTwo());

        var (noPush, p1, p2) = ObjMonFireIceCore.JointDistribution();

        // **完整九宫格：不推 6、推 1 格 2、推 2 格 1**
        Assert.Equal(6, noPush);
        Assert.Equal(2, p1);
        Assert.Equal(1, p2);
        Assert.Equal(9, noPush + p1 + p2);
    }

    [Fact]
    public void UndeclaredIdentifierFacts()
    {
        Assert.True(ObjMonFireIceCore.VarBlockExtracted());
        Assert.True(ObjMonFireIceCore.VarBlockLacksBoth());
        Assert.True(ObjMonFireIceCore.WouldNotCompile());
        Assert.True(ObjMonFireIceCore.HalfFinishedWork());

        Assert.Equal(2, ObjMonFireIceCore.UndeclaredIdentifiers.Length);
        Assert.Equal("I", ObjMonFireIceCore.UndeclaredIdentifiers[0]);
        Assert.Equal("nStep", ObjMonFireIceCore.UndeclaredIdentifiers[1]);

        // **`var` 块恰六项，且以 nPush 开头、SmartObject 结尾**
        Assert.Equal(6, ObjMonFireIceCore.NestedVarBlock.Length);
        Assert.StartsWith("nPush", ObjMonFireIceCore.NestedVarBlock[0]);
        Assert.StartsWith("SmartObject", ObjMonFireIceCore.NestedVarBlock[5]);
    }

    [Fact]
    public void CrossBranchSideEffectFacts()
    {
        Assert.True(ObjMonFireIceCore.DirectionSetBeforeHealBranch());
        Assert.True(ObjMonFireIceCore.HealAlsoTurns());
        Assert.True(ObjMonFireIceCore.CrossBranchSideEffect());
    }

    // ===================== 四、与 J210 的对比 =====================

    [Fact]
    public void ContrastWithJ210Facts()
    {
        Assert.True(ObjMonFireIceCore.NoDualRoleVariable());
        Assert.True(ObjMonFireIceCore.HardcodedEffect44());
        Assert.True(ObjMonFireIceCore.UnconditionalSend());
        Assert.True(ObjMonFireIceCore.ContrastWithJ210());
        Assert.True(ObjMonFireIceCore.OnlyParalysisNoPoison());
        Assert.True(ObjMonFireIceCore.ThreeBatchesThreeChoices());
        Assert.True(ObjMonFireIceCore.PoisonDimensionVaries());
        Assert.True(ObjMonFireIceCore.PoisonChoicesExtracted());
        Assert.True(ObjMonFireIceCore.AllThreeDiffer());
        Assert.True(ObjMonFireIceCore.ParalysisVerbatimThreeBatches());
        Assert.True(ObjMonFireIceCore.HasMaxGuard());
        Assert.True(ObjMonFireIceCore.SharedFragment());
        Assert.True(ObjMonFireIceCore.ParalysisSlotIsFive());
    }

    [Fact]
    public void PoisonChoiceTable()
    {
        Assert.Equal(3, ObjMonFireIceCore.PoisonChoices.Length);
        Assert.Equal("J207", ObjMonFireIceCore.PoisonChoices[0].Batch);
        Assert.Contains("green", ObjMonFireIceCore.PoisonChoices[0].PoisonKind);
        Assert.Equal("J210", ObjMonFireIceCore.PoisonChoices[1].Batch);
        Assert.Contains("red", ObjMonFireIceCore.PoisonChoices[1].PoisonKind);
        Assert.Equal("J211", ObjMonFireIceCore.PoisonChoices[2].Batch);
        Assert.Contains("none", ObjMonFireIceCore.PoisonChoices[2].PoisonKind);
    }

    [Fact]
    public void DamagePipelineFacts()
    {
        Assert.True(ObjMonFireIceCore.HasHealIdiom());
        Assert.True(ObjMonFireIceCore.SixthOccurrence());
        Assert.True(ObjMonFireIceCore.SixOfSevenHaveHeal());
        Assert.True(ObjMonFireIceCore.CapBeforeAbsorb());
        Assert.True(ObjMonFireIceCore.FiveToOneMajority());
        Assert.True(ObjMonFireIceCore.J203IsTheOnlyException());
    }

    // ===================== 五、整体与跨度 =====================

    [Fact]
    public void OuterTemplateFacts()
    {
        Assert.True(ObjMonFireIceCore.OuterTemplateThirdConfirmation());
        Assert.True(ObjMonFireIceCore.VerbatimSameAsJ207J208J210());
        Assert.True(ObjMonFireIceCore.TemplateStable());
        Assert.True(ObjMonFireIceCore.TemplateExtracted());
        Assert.True(ObjMonFireIceCore.OuterIsThirtyLines());
        Assert.True(ObjMonFireIceCore.FixedSize());
    }

    [Fact]
    public void RunAndNamingFacts()
    {
        Assert.True(ObjMonFireIceCore.PureInheritedShellAgain());
        Assert.True(ObjMonFireIceCore.FifthConsecutive());
        Assert.True(ObjMonFireIceCore.ThirteenthOccurrence());
        Assert.True(ObjMonFireIceCore.TwoChineseNames());
        Assert.True(ObjMonFireIceCore.FireIceVsIceOnly());
        Assert.True(ObjMonFireIceCore.NamingMismatch());
        Assert.True(ObjMonFireIceCore.SameFamilyAsJ202());
        Assert.True(ObjMonFireIceCore.SixteenClassesCovered());
        Assert.True(ObjMonFireIceCore.RemainingApprox());
    }

    [Fact]
    public void PathsTable()
    {
        Assert.True(ObjMonFireIceCore.PathsExtracted());
        Assert.True(ObjMonFireIceCore.HealAndDamageEffectsDiffer());
        Assert.True(ObjMonFireIceCore.DamageAndPushShareEffect());

        Assert.Equal(3, ObjMonFireIceCore.Paths.Length);
        Assert.Equal("self-heal", ObjMonFireIceCore.Paths[0].Path);
        Assert.Equal(2, ObjMonFireIceCore.Paths[0].EffectId);
        Assert.Equal(44, ObjMonFireIceCore.Paths[1].EffectId);
        Assert.Equal(44, ObjMonFireIceCore.Paths[2].EffectId);
    }
}
