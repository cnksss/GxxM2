using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J205：`ObjMon.pas` 中 `TTwoKindAttackMonster` 四个方法 1:1 测试
/// （合计 517 行，含嵌套函数 23 行）。
/// **本批两个重点**：
/// ① J204 判定的"`GetRangeTargetCount` 是死代码"**只针对那一份** ——
///    本类的同名嵌套函数**真的在 4256 被调用**、且**多一个脱机过滤条件**；
/// ② `TwoAttack` 的伤害管线**表面复制五遍、实则产生五种行为**。
/// </summary>
public sealed class ObjMonTwoKindCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(4074, ObjMonTwoKindCore.OneAttackStart);
        Assert.Equal(4105, ObjMonTwoKindCore.OneAttackEnd);
        Assert.Equal(32, ObjMonTwoKindCore.OneAttackLines);
        Assert.Equal(4108, ObjMonTwoKindCore.TwoAttackStart);
        Assert.Equal(4580, ObjMonTwoKindCore.TwoAttackEnd);
        Assert.Equal(473, ObjMonTwoKindCore.TwoAttackLines);
        Assert.Equal(4582, ObjMonTwoKindCore.CreateStart);
        Assert.Equal(5, ObjMonTwoKindCore.CreateLines);
        Assert.Equal(4588, ObjMonTwoKindCore.AttackTargetStart);
        Assert.Equal(7, ObjMonTwoKindCore.AttackTargetLines);
        Assert.Equal(517, ObjMonTwoKindCore.TotalLines);
        Assert.Equal(4110, ObjMonTwoKindCore.NestedFnStart);
        Assert.Equal(4132, ObjMonTwoKindCore.NestedFnEnd);
        Assert.Equal(23, ObjMonTwoKindCore.NestedFnLines);
        Assert.Equal(4256, ObjMonTwoKindCore.NestedCallLine);
        Assert.Equal(4255, ObjMonTwoKindCore.OldCriterionLine);
        Assert.Equal(8, ObjMonTwoKindCore.AttackRangeVar);
        Assert.Equal(255, ObjMonTwoKindCore.Appr255);
        Assert.Equal(250, ObjMonTwoKindCore.Appr250);
        Assert.Equal(251, ObjMonTwoKindCore.Appr251);
        Assert.Equal(256, ObjMonTwoKindCore.Appr256);
        Assert.Equal(262, ObjMonTwoKindCore.Appr262);
        Assert.Equal(267, ObjMonTwoKindCore.SlaveTemplateAppr);
        Assert.Equal(123, ObjMonTwoKindCore.SlaveTemplateRace);
        Assert.Equal(4, ObjMonTwoKindCore.MagicDenominator);
        Assert.Equal(4, ObjMonTwoKindCore.SummonCount);
        Assert.Equal(10, ObjMonTwoKindCore.ClassesCovered);
        Assert.Equal(44, ObjMonTwoKindCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonTwoKindCore.SpanMatches());
        Assert.True(ObjMonTwoKindCore.TotalLinesAddUp());
        Assert.True(ObjMonTwoKindCore.NestedFnSpanMatches());
        Assert.True(ObjMonTwoKindCore.NestedInsideTwoAttack());
        Assert.True(ObjMonTwoKindCore.CallAfterDefinition());
        Assert.True(ObjMonTwoKindCore.OldCriterionAdjacent());
        Assert.True(ObjMonTwoKindCore.StartsAscending());
        Assert.True(ObjMonTwoKindCore.WithinUnit());
        Assert.True(ObjMonTwoKindCore.NoInstrumentation());
        Assert.True(ObjMonTwoKindCore.StagesAscending());
        Assert.True(ObjMonTwoKindCore.StageSpansMatch());
    }

    // ===================== 一、嵌套函数 =====================

    [Fact]
    public void NestedFunctionFacts()
    {
        Assert.True(ObjMonTwoKindCore.NestedCalledHere());
        Assert.True(ObjMonTwoKindCore.CalledAt4256());
        Assert.True(ObjMonTwoKindCore.SameShapeAsJ204ButLive());
        Assert.True(ObjMonTwoKindCore.ThreeFoldHere());
        Assert.True(ObjMonTwoKindCore.TwoFoldInJ204());
        Assert.True(ObjMonTwoKindCore.SameNameDifferentSemantics());
        Assert.True(ObjMonTwoKindCore.NoTryFinallyHere());
        Assert.True(ObjMonTwoKindCore.CountsAfterDeleting());
        Assert.True(ObjMonTwoKindCore.SameDefectsButLive());
        Assert.True(ObjMonTwoKindCore.OldCriterionCommented());
        Assert.True(ObjMonTwoKindCore.NewIsCountDependent());
        Assert.True(ObjMonTwoKindCore.MoreTargetsHigherChance());
        Assert.True(ObjMonTwoKindCore.ClampPreventsRandomZero());
        Assert.True(ObjMonTwoKindCore.GuardsAgainstRandomZero());
        Assert.True(ObjMonTwoKindCore.DifferentPurposeThanEarlier());
        Assert.True(ObjMonTwoKindCore.FiveTargetsStill1());
        Assert.True(ObjMonTwoKindCore.BoundNeverZero());
        Assert.True(ObjMonTwoKindCore.RollZeroTriggers());
        Assert.True(ObjMonTwoKindCore.RollNonZeroDoesNot());
        Assert.True(ObjMonTwoKindCore.BoundDecreasesWithCount());
    }

    [Fact]
    public void ChanceBoundBoundaries()
    {
        // **目标数从 0 到 4 时参数递减：5 / 4 / 3 / 2 / 1**
        Assert.Equal(5, ObjMonTwoKindCore.ChanceBound(0));
        Assert.Equal(4, ObjMonTwoKindCore.ChanceBound(1));
        Assert.Equal(3, ObjMonTwoKindCore.ChanceBound(2));
        Assert.Equal(2, ObjMonTwoKindCore.ChanceBound(3));
        Assert.Equal(1, ObjMonTwoKindCore.ChanceBound(4));

        // **恰好 5 个及以上被钳在 1（不会到 0、避免 Random(0)）**
        Assert.Equal(1, ObjMonTwoKindCore.ChanceBound(5));
        Assert.Equal(1, ObjMonTwoKindCore.ChanceBound(99));

        Assert.True(ObjMonTwoKindCore.ZeroTargetsBound5());
        Assert.True(ObjMonTwoKindCore.OneTargetBound4());
        Assert.True(ObjMonTwoKindCore.FourTargetsBound1());
    }

    [Fact]
    public void OldVsNewCriterion()
    {
        // **修正后的真反例：旧判据的 roll5=0 路径会触发、新判据不认**
        Assert.True(ObjMonTwoKindCore.OldAndNewDiffer());
        Assert.True(ObjMonTwoKindCore.OldRollFiveTriggers());
        Assert.True(ObjMonTwoKindCore.NewDoesNotAtThatRoll());

        // **反向反例：零目标时新判据可触发、而旧判据在该点不触发**
        Assert.True(ObjMonTwoKindCore.NewTriggersAtZeroTargets());
        Assert.True(ObjMonTwoKindCore.OldDoesNotThere());

        // **旧判据要求"人数 >= 2"这条硬条件**
        Assert.True(ObjMonTwoKindCore.OldRequiresTwoTargets());
        Assert.True(ObjMonTwoKindCore.OldNeedsTwoTargets());

        // **新判据没有人数下限**
        Assert.True(ObjMonTwoKindCore.NewHasNoTargetFloor());
    }

    // ===================== 二、六段链 =====================

    [Fact]
    public void SixStageChainFacts()
    {
        Assert.True(ObjMonTwoKindCore.SixStageChain());
        Assert.True(ObjMonTwoKindCore.OrderedShortCircuit());
        Assert.True(ObjMonTwoKindCore.EveryStageMayExit());
        Assert.True(ObjMonTwoKindCore.TableExtracted());
        Assert.True(ObjMonTwoKindCore.SameApprTwoModes());
        Assert.True(ObjMonTwoKindCore.FrozenThenSmall());
        Assert.True(ObjMonTwoKindCore.MutuallyExclusive());
        Assert.True(ObjMonTwoKindCore.Range8UsedOnlyOnce());
        Assert.True(ObjMonTwoKindCore.OthersHardcoded());
        Assert.True(ObjMonTwoKindCore.VariableNameOverpromises());
        Assert.True(ObjMonTwoKindCore.RawSubtraction());
        Assert.True(ObjMonTwoKindCore.PrecedenceWorksByLuck());
        Assert.True(ObjMonTwoKindCore.CoexistsWithTickDiff());

        Assert.Equal(6, ObjMonTwoKindCore.Stages.Length);
        Assert.Equal(1, ObjMonTwoKindCore.Stages[0].Stage);
        Assert.Equal(6, ObjMonTwoKindCore.Stages[5].Stage);
    }

    [Fact]
    public void FrozenCooldownBoundaries()
    {
        // **血量恰好 80% 不触发（严格小于）**
        Assert.True(ObjMonTwoKindCore.LowHpAllows());
        Assert.True(ObjMonTwoKindCore.ExactlyEightyBlocks());
        Assert.False(ObjMonTwoKindCore.FrozenReady(80, 100, 20000, 0));

        // **冷却恰好 15 秒不触发（严格大于）**
        Assert.True(ObjMonTwoKindCore.ExactlyFifteenSecondsBlocks());
        Assert.False(ObjMonTwoKindCore.FrozenReady(50, 100, 15000, 0));
        Assert.True(ObjMonTwoKindCore.PastFifteenSecondsAllows());

        // **高血量即使冷却够也不触发**
        Assert.True(ObjMonTwoKindCore.HighHpBlocks());
    }

    // ===================== 三、FFrozenTick 与召唤 =====================

    [Fact]
    public void FrozenTickFacts()
    {
        Assert.True(ObjMonTwoKindCore.SingleField());
        Assert.True(ObjMonTwoKindCore.ThreeSitesPlusInit());
        Assert.True(ObjMonTwoKindCore.CrossCallState());
        Assert.True(ObjMonTwoKindCore.StampBeforeWork());
        Assert.True(ObjMonTwoKindCore.CooldownConsumedOnAttempt());
        Assert.True(ObjMonTwoKindCore.EmptyAttackStillCools());
        Assert.True(ObjMonTwoKindCore.FrozenTickSitesExtracted());
        Assert.True(ObjMonTwoKindCore.InitToNow());
        Assert.True(ObjMonTwoKindCore.FifteenSecondGraceOnSpawn());
        Assert.True(ObjMonTwoKindCore.PreventsImmediateBurst());

        Assert.Equal(4, ObjMonTwoKindCore.FrozenTickSites.Length);
        Assert.Equal(67, ObjMonTwoKindCore.FrozenTickSites[0].Line);
        Assert.Equal(4175, ObjMonTwoKindCore.FrozenTickSites[1].Line);
        Assert.Equal(4178, ObjMonTwoKindCore.FrozenTickSites[2].Line);
        Assert.Equal(4585, ObjMonTwoKindCore.FrozenTickSites[3].Line);
    }

    [Fact]
    public void SummonFacts()
    {
        Assert.True(ObjMonTwoKindCore.TwoBranchesFor256());
        Assert.True(ObjMonTwoKindCore.ThreeConditionsForSummon());
        Assert.True(ObjMonTwoKindCore.NoSummonWhenSlavesExist());
        Assert.True(ObjMonTwoKindCore.AllThreeSummons());
        Assert.True(ObjMonTwoKindCore.ExactlyThirdBlocks());
        Assert.True(ObjMonTwoKindCore.ExistingSlaveBlocks());
        Assert.True(ObjMonTwoKindCore.MissedRollBlocks());
        Assert.True(ObjMonTwoKindCore.SummonFourTimes());
        Assert.True(ObjMonTwoKindCore.RepositionsEachTime());
        Assert.True(ObjMonTwoKindCore.MayOverlap());
        Assert.True(ObjMonTwoKindCore.OnlyNilChecked());

        // **恰好三分之一不召唤（整数截断：100/3 = 33）**
        Assert.False(ObjMonTwoKindCore.CanSummon(33, 100, 0, 0));
        Assert.True(ObjMonTwoKindCore.CanSummon(32, 100, 0, 0));
    }

    [Fact]
    public void SlaveTemplateFacts()
    {
        Assert.True(ObjMonTwoKindCore.NameDefaultsToSelf());
        Assert.True(ObjMonTwoKindCore.OverriddenBySlaveTemplate());
        Assert.True(ObjMonTwoKindCore.BreaksOnFirstMatch());
        Assert.True(ObjMonTwoKindCore.DanglingAppr267());
        Assert.True(ObjMonTwoKindCore.OnlyQueriedNeverSet());
        Assert.True(ObjMonTwoKindCore.SingleSiteInEngine());
        Assert.True(ObjMonTwoKindCore.MatchesBoth());
        Assert.True(ObjMonTwoKindCore.WrongApprNoMatch());
        Assert.True(ObjMonTwoKindCore.WrongRaceNoMatch());

        Assert.True(ObjMonTwoKindCore.IsSlaveTemplate(123, 267));
        Assert.False(ObjMonTwoKindCore.IsSlaveTemplate(123, 255));
        Assert.False(ObjMonTwoKindCore.IsSlaveTemplate(80, 267));
    }

    [Fact]
    public void SummonInitFacts()
    {
        Assert.True(ObjMonTwoKindCore.ConditionalCompilationSplit());
        Assert.True(ObjMonTwoKindCore.TryAndFinallySeparatelyGuarded());
        Assert.True(ObjMonTwoKindCore.MacroGeneratedPairing());
        Assert.True(ObjMonTwoKindCore.SixInitSteps());
        Assert.True(ObjMonTwoKindCore.RoyaltyOneHour());
        Assert.True(ObjMonTwoKindCore.HealToMidpointNotFull());
        Assert.True(ObjMonTwoKindCore.IntegerDivisionTruncates());
        Assert.True(ObjMonTwoKindCore.FullHpNoHeal());
        Assert.True(ObjMonTwoKindCore.EmptyHealsToHalf());
        Assert.True(ObjMonTwoKindCore.LowHealsToMidpoint());
        Assert.True(ObjMonTwoKindCore.AlwaysMidpoint());

        Assert.Equal(3600000, ObjMonTwoKindCore.RoyaltyMs);
        Assert.Equal(1, ObjMonTwoKindCore.SlaveMakeLevel);
        Assert.Equal(1, ObjMonTwoKindCore.SlaveExpLevel);

        // **补血是 (hp + maxHp) / 2，不是补满**
        Assert.Equal(50, ObjMonTwoKindCore.AfterHeal(0, 100));
        Assert.Equal(60, ObjMonTwoKindCore.AfterHeal(20, 100));
        Assert.Equal(100, ObjMonTwoKindCore.AfterHeal(100, 100));
    }

    // ===================== 四、五遍管线 =====================

    [Fact]
    public void PipelineVariantFacts()
    {
        Assert.True(ObjMonTwoKindCore.PipelineCopiedFiveTimes());
        Assert.True(ObjMonTwoKindCore.OnlyFirstHasRateAdd());
        Assert.True(ObjMonTwoKindCore.OnlyFirstHasFrozen());
        Assert.True(ObjMonTwoKindCore.OnlyLaterHaveParalysis());
        Assert.True(ObjMonTwoKindCore.FiveVariantsNotOne());
        Assert.True(ObjMonTwoKindCore.VariantMatrixExtracted());
        Assert.True(ObjMonTwoKindCore.ExactlyOneRateAdd());
        Assert.True(ObjMonTwoKindCore.ExactlyOneFrozen());
        Assert.True(ObjMonTwoKindCore.ExactlyFourParalysis());
        Assert.True(ObjMonTwoKindCore.BothExtrasOnFirstStage());
        Assert.True(ObjMonTwoKindCore.ParalysisIsComplementary());

        Assert.Equal(6, ObjMonTwoKindCore.PipelineVariants.Length);

        // **第 1 段是唯一同时有 RateAdd 与 Frozen 的**
        Assert.True(ObjMonTwoKindCore.PipelineVariants[0].RateAdd);
        Assert.True(ObjMonTwoKindCore.PipelineVariants[0].Frozen);
        Assert.False(ObjMonTwoKindCore.PipelineVariants[0].Paralysis);

        // **第 2 段三者皆无**
        Assert.False(ObjMonTwoKindCore.PipelineVariants[1].RateAdd);
        Assert.False(ObjMonTwoKindCore.PipelineVariants[1].Frozen);
        Assert.False(ObjMonTwoKindCore.PipelineVariants[1].Paralysis);
    }

    [Fact]
    public void ContinueAndResourceFacts()
    {
        Assert.True(ObjMonTwoKindCore.OfflineOrderConsistent());
        Assert.True(ObjMonTwoKindCore.AllFiveSameOrder());
        Assert.True(ObjMonTwoKindCore.TwoSeparateContinues());
        Assert.True(ObjMonTwoKindCore.SameAsJ204());
        Assert.True(ObjMonTwoKindCore.DiffersFromJ203());
        Assert.True(ObjMonTwoKindCore.NoTryFinallyInStage3());
        Assert.True(ObjMonTwoKindCore.FreeAfterSend());
        Assert.True(ObjMonTwoKindCore.TwoModesInsideOneMethod());
        Assert.True(ObjMonTwoKindCore.SummonBranchAlsoUnprotected());
    }

    [Fact]
    public void FreezeFormsFacts()
    {
        Assert.True(ObjMonTwoKindCore.FixedThreeSeconds());
        Assert.True(ObjMonTwoKindCore.FourFrozenFormsInFile());
        Assert.True(ObjMonTwoKindCore.ThisOneIsFixed());
        Assert.True(ObjMonTwoKindCore.OtherThreeAreRanges());
        Assert.True(ObjMonTwoKindCore.LongestFreezeInComment());
        Assert.True(ObjMonTwoKindCore.SixLineBraceCommentOldLoop());
        Assert.True(ObjMonTwoKindCore.WasFreezeOnly());
        Assert.True(ObjMonTwoKindCore.BecameDamageOnly());

        Assert.Equal(4, ObjMonTwoKindCore.FreezeForms.Length);

        // **四种冰冻写法：2..4 / 3 固定 / 3..14 / 3..5**
        Assert.Equal(2, ObjMonTwoKindCore.FreezeForms[0].Min);
        Assert.Equal(4, ObjMonTwoKindCore.FreezeForms[0].Max);
        Assert.Equal(3, ObjMonTwoKindCore.FreezeForms[1].Min);
        Assert.Equal(3, ObjMonTwoKindCore.FreezeForms[1].Max);
        Assert.Equal(14, ObjMonTwoKindCore.FreezeForms[2].Max);
        Assert.Equal(5, ObjMonTwoKindCore.FreezeForms[3].Max);
    }

    // ===================== 五、段 4/5/6 与整体 =====================

    [Fact]
    public void StageFourFiveSixFacts()
    {
        Assert.True(ObjMonTwoKindCore.RandomEffectType());
        Assert.True(ObjMonTwoKindCore.OnlyPlaceEffectRandom());
        Assert.True(ObjMonTwoKindCore.UsesLightingEx());
        Assert.True(ObjMonTwoKindCore.OnlyStage5());
        Assert.True(ObjMonTwoKindCore.SameAsJ204SixZeroSeven());
        Assert.True(ObjMonTwoKindCore.SingleTargetFallback());
        Assert.True(ObjMonTwoKindCore.NoListNoLoop());
        Assert.True(ObjMonTwoKindCore.SameTailAsGroupStages());
        Assert.True(ObjMonTwoKindCore.ComputedEarlyUsedLate());
        Assert.True(ObjMonTwoKindCore.WastedIfEarlierExit());
        Assert.True(ObjMonTwoKindCore.Appr255ForcesOne());
        Assert.True(ObjMonTwoKindCore.HitCmdOneFor255());
        Assert.True(ObjMonTwoKindCore.HitCmdRandomOtherwise());

        // **`nHitCmd`：255 恒为 1、其余用掷值**
        Assert.Equal(1, ObjMonTwoKindCore.HitCmd(255, 0));
        Assert.Equal(1, ObjMonTwoKindCore.HitCmd(255, 1));
        Assert.Equal(0, ObjMonTwoKindCore.HitCmd(250, 0));
        Assert.Equal(1, ObjMonTwoKindCore.HitCmd(250, 1));
    }

    [Fact]
    public void DispatchFacts()
    {
        Assert.True(ObjMonTwoKindCore.OneInFourChance());
        Assert.True(ObjMonTwoKindCore.TwentyFivePercentMagic());
        Assert.True(ObjMonTwoKindCore.SeventyFivePercentPhysical());
        Assert.True(ObjMonTwoKindCore.RollZeroMagic());
        Assert.True(ObjMonTwoKindCore.RollOnePhysical());
        Assert.True(ObjMonTwoKindCore.RollThreePhysical());
        Assert.True(ObjMonTwoKindCore.ExactlyOneMagicValue());
        Assert.True(ObjMonTwoKindCore.SameShapeAsJ202());
        Assert.True(ObjMonTwoKindCore.UsesPlainGetAttackDir());
        Assert.True(ObjMonTwoKindCore.GenericVersion());
        Assert.True(ObjMonTwoKindCore.PrologueDuplicated());
        Assert.True(ObjMonTwoKindCore.TenPlusOccurrencesInFile());
        Assert.True(ObjMonTwoKindCore.TenClassesCovered());
        Assert.True(ObjMonTwoKindCore.RemainingApprox());

        Assert.Equal("two-attack", ObjMonTwoKindCore.Dispatch(0));
        Assert.Equal("one-attack", ObjMonTwoKindCore.Dispatch(1));
        Assert.Equal("one-attack", ObjMonTwoKindCore.Dispatch(3));
    }
}
