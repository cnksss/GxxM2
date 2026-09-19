using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J144：`DamageReboundPower`（2272-2293）、`BlastHit`（2295-2353）、
/// `NewAbilPower`（2355-2373）、`GetPowerRateAdd`（2375-2436）1:1 测试。
/// </summary>
public sealed class DamageReduceCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(DamageReduceCore.ConstantsMatchSource());
        Assert.True(DamageReduceCore.DefaultThresholdsMatch());
        Assert.True(DamageReduceCore.IndexesAreDistinct());
    }

    [Fact]
    public void ConstantValues()
    {
        Assert.Equal(1, DamageReduceCore.LaUndead);
        Assert.Equal(5, DamageReduceCore.ReboundIndex);
        Assert.Equal(0, DamageReduceCore.CritRateIndex);
        Assert.Equal(24, DamageReduceCore.CritResistIndex);
        Assert.Equal(21, DamageReduceCore.FatalRateIndex);
        Assert.Equal(22, DamageReduceCore.FatalDamageIndex);
        Assert.Equal(23, DamageReduceCore.FatalDefenceIndex);
        Assert.Equal(100, DamageReduceCore.FatalBaseHundred);
        Assert.Equal(100, DamageReduceCore.DefaultCritAttackHurtRate);
        Assert.Equal(30, DamageReduceCore.DefaultDamageReboundRate);
        Assert.Equal(100, DamageReduceCore.DefaultFatalBlowBasePower);
        Assert.Equal(120, DamageReduceCore.DefaultFatalThresholds[0]);
        Assert.Equal(150, DamageReduceCore.DefaultFatalThresholds[1]);
        Assert.Equal(200, DamageReduceCore.DefaultFatalThresholds[2]);
    }

    [Fact]
    public void RoundHalfUpValues()
    {
        Assert.True(DamageReduceCore.RoundHalfUpValues());
        Assert.True(DamageReduceCore.HighIntegerValue());
    }

    // ===================== 一、DamageReboundPower =====================

    [Fact]
    public void ReboundGateTruthTables()
    {
        Assert.True(DamageReduceCore.ScaleModeAlwaysTriggers());
        Assert.True(DamageReduceCore.ProbabilityModeUsesLessOrEqual());
    }

    [Fact]
    public void FieldMeaningDependsOnMode()
    {
        // **同一字段两种模式下含义完全不同**
        Assert.True(DamageReduceCore.FieldMeaningDependsOnMode());
        Assert.True(DamageReduceCore.ReboundModeSwitch());
    }

    [Fact]
    public void ValueSourcesDiffer()
    {
        // **属性模式用自身字段、概率模式用配置倍率**
        Assert.True(DamageReduceCore.ScaleModeUsesOwnField());
        Assert.True(DamageReduceCore.ProbabilityModeUsesConfig());
        Assert.True(DamageReduceCore.ValueSourcesDiffer());

        Assert.Equal(50, DamageReduceCore.ReboundValue(true, 100, 50, 999));
        Assert.Equal(30, DamageReduceCore.ReboundValue(false, 100, 999, 30));
    }

    [Fact]
    public void NonPositiveExitTruthTable()
    {
        Assert.True(DamageReduceCore.NonPositiveExitTruthTable());
        Assert.False(DamageReduceCore.NonPositiveExits(1));
        Assert.True(DamageReduceCore.NonPositiveExits(0));
    }

    [Fact]
    public void NegativeClampIsDeadCode()
    {
        Assert.True(DamageReduceCore.NegativeClampIsDeadCode());
    }

    [Fact]
    public void ApplyReboundValues()
    {
        Assert.True(DamageReduceCore.ApplyReboundValues());

        Assert.Equal(0, DamageReduceCore.ApplyRebound(0, true, 50, 0, 30));
        Assert.Equal(50, DamageReduceCore.ApplyRebound(100, true, 50, 99, 30));
        Assert.Equal(0, DamageReduceCore.ApplyRebound(100, false, 50, 99, 30));
        Assert.Equal(30, DamageReduceCore.ApplyRebound(100, false, 50, 30, 30));
    }

    // ===================== 二、BlastHit =====================

    [Fact]
    public void BlastHitTwoSystems()
    {
        Assert.True(DamageReduceCore.BlastHitTwoSystems());
        Assert.True(DamageReduceCore.TwoBlastSystems());
        Assert.Equal(2, DamageReduceCore.BlastSystems.Length);
    }

    [Fact]
    public void FirstSystemThreeGates()
    {
        Assert.True(DamageReduceCore.FirstSystemThreeGates());
        Assert.True(DamageReduceCore.NormalMonsterCannotCrit());
        Assert.False(DamageReduceCore.CritGate(1, 1, 80));
    }

    [Fact]
    public void MinusCritResist()
    {
        Assert.True(DamageReduceCore.MinusCritResist());
        Assert.True(DamageReduceCore.ResistCanNegateCrit());
        Assert.Equal(30, DamageReduceCore.CritRate(50, 20));
        Assert.Equal(0, DamageReduceCore.CritRate(50, 50));
        Assert.Equal(-10, DamageReduceCore.CritRate(50, 60));
    }

    [Fact]
    public void CritRateLessOrEqualValues()
    {
        Assert.True(DamageReduceCore.CritRateLessOrEqualValues());
        Assert.True(DamageReduceCore.CritRateUsesLessOrEqual(5, 5));
        Assert.False(DamageReduceCore.CritRateUsesLessOrEqual(6, 5));
    }

    [Fact]
    public void AdditiveNotMultiplicative()
    {
        // **暴击是"原伤害 + 百分比附加值"**
        Assert.True(DamageReduceCore.AdditiveNotMultiplicative());
        Assert.Equal(200, DamageReduceCore.CritDamage(100, 100));
        Assert.Equal(150, DamageReduceCore.CritDamage(100, 50));
        Assert.Equal(100, DamageReduceCore.CritDamage(100, 0));
    }

    [Fact]
    public void SecondScaleCanShrink()
    {
        // **第二次缩放按结果百分比重算，< 100 时压小**
        Assert.True(DamageReduceCore.SecondScaleCanShrink());
        Assert.True(DamageReduceCore.SecondScaleIsNotAMultiplier());
        Assert.Equal(100, DamageReduceCore.SecondScale(200, true, 50));
        Assert.Equal(300, DamageReduceCore.SecondScale(200, true, 150));
    }

    [Fact]
    public void SecondSystemLosesRaceCheck()
    {
        // **第二套丢掉了种族限制（那行被注释掉）**
        Assert.True(DamageReduceCore.SecondSystemGateLosesRaceCheck(1, 1));
        Assert.True(DamageReduceCore.SecondSystemFiresOnMonsters());
        Assert.True(DamageReduceCore.FatalBlowCommentHasDate());
        Assert.Contains("2020-09-05", DamageReduceCore.FatalRaceComment);
    }

    [Fact]
    public void FatalBlowFormulaUsesHundredAsBase()
    {
        Assert.True(DamageReduceCore.FatalBlowFormulaUsesHundredAsBase());
        Assert.True(DamageReduceCore.FieldBelowBaseShrinks());
        Assert.Equal(100, DamageReduceCore.FatalAddPower(100, 100, 100));
        Assert.Equal(120, DamageReduceCore.FatalAddPower(100, 100, 120));
        Assert.Equal(50, DamageReduceCore.FatalAddPower(100, 100, 50));
    }

    [Fact]
    public void FatalBlowDefenceTwoModes()
    {
        Assert.True(DamageReduceCore.FatalBlowDefenceTwoModes());
        Assert.True(DamageReduceCore.FullDefenceCancelsAll());
        Assert.Equal(0, DamageReduceCore.FatalDecPower(100, 0));
        Assert.Equal(100, DamageReduceCore.FatalDecPower(100, 100));
        Assert.Equal(50, DamageReduceCore.FatalDecPower(100, 50));
    }

    [Fact]
    public void FatalResultValues()
    {
        Assert.True(DamageReduceCore.FatalResultValues());
        Assert.True(DamageReduceCore.DefenceCanUndershoot());
        Assert.Equal(10, DamageReduceCore.FatalResult(100, 10, 100));
    }

    [Fact]
    public void PowerRateUsesNPowerDenominator()
    {
        Assert.True(DamageReduceCore.PowerRateUsesNPowerDenominator());
        Assert.True(DamageReduceCore.DenominatorUnguarded());
        Assert.Equal(150, DamageReduceCore.PowerRate(150, 100));
    }

    [Fact]
    public void FourTiersThreeThresholds()
    {
        // **阈值恰等于 powerRate 时跳到下一档（探针实测）**
        Assert.True(DamageReduceCore.FourTiersThreeThresholds());
        Assert.True(DamageReduceCore.ThresholdEqualitySkipsTier());
        Assert.Equal("bhtFatalBlow3", DamageReduceCore.FatalTier(150, 120, 150, 200));
        Assert.Equal("bhtFatalBlow4", DamageReduceCore.FatalTier(200, 120, 150, 200));
    }

    [Fact]
    public void TierBoundaryIsStrictLess()
    {
        Assert.True(DamageReduceCore.TierBoundaryIsStrictLess());
        Assert.Equal("bhtFatalBlow1", DamageReduceCore.FatalTier(119, 120, 150, 200));
    }

    [Fact]
    public void FourTiers()
    {
        Assert.True(DamageReduceCore.FourTiers());
        Assert.True(DamageReduceCore.FourthTierIsFallback());
        Assert.Equal(4, DamageReduceCore.FatalTiers.Length);
    }

    [Fact]
    public void ResultWrittenBackBetweenSystems()
    {
        Assert.True(DamageReduceCore.ResultWrittenBackBetweenSystems());
    }

    // ---- 完整仿真 ----

    [Fact]
    public void BlastNoneCase()
    {
        Assert.True(DamageReduceCore.BlastNoneCase());
    }

    [Fact]
    public void BlastCritOnlyCase()
    {
        Assert.True(DamageReduceCore.BlastCritOnlyCase());
    }

    [Fact]
    public void BlastFatalOnlyCase()
    {
        Assert.True(DamageReduceCore.BlastFatalOnlyCase());

        var r = DamageReduceCore.ApplyBlastHit(100, 0, 0, 0, false, 0, 100,
            100, 10, 100, 0, 100, 120, 150, 200);

        Assert.Equal(200, r.Damage);
        Assert.Equal("bhtFatalBlow4", r.Type);
    }

    [Fact]
    public void BlastBothSystemsCase()
    {
        // 第一套触发 → 200；第二套也触发 → 再叠加
        var r = DamageReduceCore.ApplyBlastHit(100, 50, 0, 10, false, 0, 100,
            100, 10, 100, 0, 100, 120, 150, 200);

        // 第一套：200。第二套：nPower=200, add = 200/100*(100+100-100)=200 → result 400
        Assert.Equal(400, r.Damage);
        Assert.Equal("bhtFatalBlow4", r.Type);
    }

    [Fact]
    public void BlastResistNegatesCrit()
    {
        // 抗性超过暴击率 → 第一套不触发
        var r = DamageReduceCore.ApplyBlastHit(100, 50, 60, 0, false, 0, 100,
            0, 0, 0, 0, 100, 120, 150, 200);

        Assert.Equal(100, r.Damage);
        Assert.Equal("bhtNone", r.Type);
    }

    // ===================== 三、NewAbilPower =====================

    [Fact]
    public void ValidType()
    {
        Assert.True(DamageReduceCore.ValidType(1));
        Assert.True(DamageReduceCore.ValidType(3));
        Assert.False(DamageReduceCore.ValidType(0));
        Assert.False(DamageReduceCore.ValidType(4));
        Assert.True(DamageReduceCore.OutOfRangeReturnsUnchanged());
    }

    [Fact]
    public void TypeOneAdds()
    {
        Assert.True(DamageReduceCore.TypeOneAdds());
        Assert.Equal(150, DamageReduceCore.ApplyAbilPower(1, 100, 50));
        Assert.Equal(200, DamageReduceCore.ApplyAbilPower(1, 100, 100));
    }

    [Fact]
    public void TypesTwoThreeSubtract()
    {
        Assert.True(DamageReduceCore.TypesTwoThreeSubtract());
        Assert.Equal(50, DamageReduceCore.ApplyAbilPower(2, 100, 50));
        Assert.Equal(0, DamageReduceCore.ApplyAbilPower(3, 100, 200));
    }

    [Fact]
    public void ZeroClampOnlyForTwoThree()
    {
        Assert.True(DamageReduceCore.ZeroClampOnlyForTwoThree());
        Assert.True(DamageReduceCore.OnlyTypeOneUsesInt64());
    }

    [Fact]
    public void AbilGateEdges()
    {
        Assert.True(DamageReduceCore.OutOfRangeValues());
        Assert.True(DamageReduceCore.BothNeedPositivePower());
        Assert.True(DamageReduceCore.BothNeedPositiveField());

        Assert.Equal(100, DamageReduceCore.ApplyAbilPower(0, 100, 50));
        Assert.Equal(100, DamageReduceCore.ApplyAbilPower(4, 100, 50));
        Assert.Equal(0, DamageReduceCore.ApplyAbilPower(1, 0, 50));
        Assert.Equal(100, DamageReduceCore.ApplyAbilPower(1, 100, 0));
    }

    [Fact]
    public void MatchesJ143CallSites()
    {
        // **J143 的两次调用：2 减、1 加**
        Assert.True(DamageReduceCore.MatchesJ143CallSites());
    }

    [Fact]
    public void ThreeAbilTypeComments()
    {
        Assert.True(DamageReduceCore.ThreeAbilTypeComments());
        Assert.Equal(3, DamageReduceCore.AbilTypes.Length);
        Assert.True(DamageReduceCore.AbilFixCommentHasDate());
        Assert.Contains("2014-04-11", DamageReduceCore.AbilFixComment);
    }

    // ===================== 四、GetPowerRateAdd =====================

    [Fact]
    public void ThreeMutuallyExclusiveBranches()
    {
        Assert.True(DamageReduceCore.ThreeMutuallyExclusiveBranches());
        Assert.True(DamageReduceCore.HumanAttackerBeatsSlave());
        Assert.True(DamageReduceCore.SlaveBeatsMonsterVsHero());
        Assert.True(DamageReduceCore.OnlyHumanBranchStacks());
    }

    [Fact]
    public void BranchValues()
    {
        Assert.Equal("humanAttacker", DamageReduceCore.PowerRateBranch(0, false, 80));
        Assert.Equal("humanAttacker", DamageReduceCore.PowerRateBranch(1, false, 80));
        Assert.Equal("humanAttacker", DamageReduceCore.PowerRateBranch(150, false, 80));
        Assert.Equal("slave", DamageReduceCore.PowerRateBranch(80, true, 80));
        Assert.Equal("monsterVsHero", DamageReduceCore.PowerRateBranch(80, false, 1));
        Assert.Equal("none", DamageReduceCore.PowerRateBranch(80, false, 80));
    }

    [Fact]
    public void SameHumanCheckAsJ143()
    {
        // **人类判定与 J142/J143 同形，且 PLAYMOSTER 同样被注释掉**
        Assert.True(DamageReduceCore.SameHumanCheckValues());
        Assert.True(DamageReduceCore.PlayMosterCommentedOutAgain());
        Assert.True(DamageReduceCore.PlayMosterCommentForm());
        Assert.False(DamageReduceCore.SameHumanCheckAsJ143(150, false, 0));
    }

    [Fact]
    public void HumanCheckRepeatedThreeTimes()
    {
        // **这段七行判定在本工程已出现三次**
        Assert.True(DamageReduceCore.HumanCheckThreeTimes());
        Assert.True(DamageReduceCore.ThreeHumanCheckSites());
        Assert.True(DamageReduceCore.DuplicatedHumanCheck());
        Assert.Equal(3, DamageReduceCore.HumanCheckOccurrences());
        Assert.Equal(3, DamageReduceCore.HumanCheckSites.Length);
    }

    [Fact]
    public void RateGuarding()
    {
        // **两个 CEO 倍率带 >0 门，另三个没有**
        Assert.True(DamageReduceCore.HumMonRateGuarded());
        Assert.True(DamageReduceCore.SlaveAndConfigRatesUnguarded());
        Assert.True(DamageReduceCore.ZeroConfigZeroesDamage());
        Assert.True(DamageReduceCore.PowerRateItemCounts());
        Assert.Equal(100, DamageReduceCore.GuardedRate(0, 100));
        Assert.Equal(0, DamageReduceCore.UnguardedRate(0, 100));
    }

    [Fact]
    public void StackValues()
    {
        // **第一段内三道倍率可以叠加**
        Assert.True(DamageReduceCore.HumanBranchCanStackThreeMultipliers());
        Assert.True(DamageReduceCore.StackValues());
        Assert.Equal(200, DamageReduceCore.StackHumanBranch(100, 200, false, 100, false, 100));
        Assert.Equal(50, DamageReduceCore.StackHumanBranch(100, 100, true, 50, false, 100));
    }

    [Fact]
    public void OnlyUpperBoundClamped()
    {
        // **只限上界、负数可原样传出**
        Assert.True(DamageReduceCore.OnlyUpperBoundClamped());
        Assert.True(DamageReduceCore.NegativeSurvivesClamp());
        Assert.Equal(-500, DamageReduceCore.ClampPowerRate(-500));
        Assert.Equal(int.MaxValue, DamageReduceCore.ClampPowerRate(long.MaxValue));
    }

    [Fact]
    public void PowerRateComments()
    {
        Assert.True(DamageReduceCore.FourPowerRateComments());
        Assert.True(DamageReduceCore.PowerRateCommentsHaveDates());
        Assert.Equal(4, DamageReduceCore.PowerRateComments.Length);
    }
}
