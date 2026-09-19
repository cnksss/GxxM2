using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J127：自定义特效事件 `TCustomEffectEvent`
/// （GameEvent.pas 133-153 声明、756-982 构造 + `Run`；uCustomMonsterUtils.pas 26-32 附加伤害记录）1:1 测试。
/// </summary>
public sealed class CustomEffectEventCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.Equal(300u, CustomEffectEventCore.BaseDelay);
        Assert.Equal(1, CustomEffectEventCore.MinAttackInterval);
        Assert.Equal(12, CustomEffectEventCore.AdditionalDamageSlots);
        Assert.Equal(3, CustomEffectEventCore.NewAbilPowerIndex);
        Assert.Equal(22, CustomEffectEventCore.SendMsgMagicIndex);
        Assert.Equal(50, CustomEffectEventCore.HealthSpellChangedArg);
    }

    [Fact]
    public void BaseDelayIsDeadConstant()
    {
        // 784 声明 BASE_DELAY = 300 但 Run 中从未使用
        Assert.True(CustomEffectEventCore.BaseDelayIsDeadConstant());
    }

    // ===================== 一、构造参数校验 =====================

    [Fact]
    public void OnlyAttackIntervalIsClamped()
    {
        Assert.True(CustomEffectEventCore.OnlyAttackIntervalIsClamped());
    }

    [Fact]
    public void AttackIntervalClampBoundaries()
    {
        Assert.True(CustomEffectEventCore.AttackIntervalClampBoundaries());
        Assert.Equal(1, CustomEffectEventCore.ClampAttackInterval(-5));
        Assert.Equal(1, CustomEffectEventCore.ClampAttackInterval(0));
        Assert.Equal(1, CustomEffectEventCore.ClampAttackInterval(1));
        Assert.Equal(2, CustomEffectEventCore.ClampAttackInterval(2));
    }

    [Fact]
    public void OtherElevenParamsUnchecked()
    {
        Assert.True(CustomEffectEventCore.OtherElevenParamsUnchecked());
        Assert.Equal(8, CustomEffectEventCore.UncheckedParams.Length);
    }

    [Fact]
    public void AdditionalDamagesIsValueCopy()
    {
        Assert.True(CustomEffectEventCore.AdditionalDamagesIsValueCopy());
    }

    [Fact]
    public void ValueCopyIsIsolated()
    {
        // 与 J104/J116 的指针共享相反：此处改原数组不影响事件
        Assert.True(CustomEffectEventCore.ValueCopyIsIsolated());
    }

    [Fact]
    public void ByteAndWordRanges()
    {
        Assert.True(CustomEffectEventCore.ByteAndWordRanges());
    }

    // ===================== 二、槽位使用 =====================

    [Fact]
    public void SlotsFourAndElevenUnused()
    {
        // **12 个槽只用 10 个，槽 4 与槽 11 从未被引用**
        Assert.True(CustomEffectEventCore.SlotsFourAndElevenUnused());
        Assert.True(CustomEffectEventCore.TenSlotsUsedTwoIdle());
    }

    [Fact]
    public void UsedSlotsList()
    {
        Assert.Equal(new[] { 0, 1, 2, 3, 5, 6, 7, 8, 9, 10 }, CustomEffectEventCore.UsedSlots);
        Assert.DoesNotContain(4, CustomEffectEventCore.UsedSlots);
        Assert.DoesNotContain(11, CustomEffectEventCore.UsedSlots);
    }

    [Fact]
    public void SlotSemantics()
    {
        Assert.Contains("绿毒", CustomEffectEventCore.EffectForSlot(0));
        Assert.Contains("红毒", CustomEffectEventCore.EffectForSlot(1));
        Assert.Contains("麻痹", CustomEffectEventCore.EffectForSlot(2));
        Assert.Contains("冰冻", CustomEffectEventCore.EffectForSlot(3));
        Assert.Contains("吸血", CustomEffectEventCore.EffectForSlot(5));
        Assert.Contains("吸蓝", CustomEffectEventCore.EffectForSlot(6));
        Assert.Contains("蛛网", CustomEffectEventCore.EffectForSlot(7));
        Assert.Contains("零防御", CustomEffectEventCore.EffectForSlot(8));
        Assert.Contains("零魔御", CustomEffectEventCore.EffectForSlot(9));
        Assert.Contains("冰封", CustomEffectEventCore.EffectForSlot(10));
    }

    [Fact]
    public void UnusedSlotsHaveNoSemantics()
    {
        Assert.Equal("", CustomEffectEventCore.EffectForSlot(4));
        Assert.Equal("", CustomEffectEventCore.EffectForSlot(11));
    }

    [Fact]
    public void SlotOrderDiffersFromExecutionOrder()
    {
        // 槽 5/6（吸血吸蓝）写在槽 2/3（麻痹冰冻）之前
        Assert.True(CustomEffectEventCore.SlotOrderDiffersFromExecutionOrder());
        Assert.True(CustomEffectEventCore.DrainExecutesBeforeParalysis());
    }

    // ===================== 三、四条件门 =====================

    [Fact]
    public void RateIsDenominatorNotPercentage()
    {
        Assert.True(CustomEffectEventCore.RateIsDenominatorNotPercentage());
        Assert.Equal("1/5", CustomEffectEventCore.ProbabilityDescription(5));
        Assert.Equal("1/100", CustomEffectEventCore.ProbabilityDescription(100));
    }

    [Fact]
    public void RateIsDenominator()
    {
        // Rate = 5 → 只有 1/5 命中（roll 在 [0,4]）
        Assert.True(CustomEffectEventCore.RateIsDenominator(5, 0));
        Assert.True(CustomEffectEventCore.RateIsDenominator(5, 4));
        Assert.False(CustomEffectEventCore.RateIsDenominator(5, 5));
    }

    [Fact]
    public void ZeroRateMeansAlwaysFires()
    {
        // **与直觉相反：Rate = 0 表示必定触发**
        Assert.True(CustomEffectEventCore.ZeroRateMeansAlwaysFires());
        Assert.True(CustomEffectEventCore.ZeroRateHits());
        Assert.Equal("必定触发", CustomEffectEventCore.ProbabilityDescription(0));
    }

    [Fact]
    public void ZeroTimeNeverApplies()
    {
        // **Time = 0 的附加效果永不生效**（Time > 0 是统一必要条件）
        Assert.True(CustomEffectEventCore.ZeroTimeNeverApplies());
        Assert.False(CustomEffectEventCore.GateRequiresCheckedAndTime(true, 0));
    }

    [Fact]
    public void UncheckedNeverApplies()
    {
        Assert.True(CustomEffectEventCore.UncheckedNeverApplies());
        Assert.False(CustomEffectEventCore.GateRequiresCheckedAndTime(false, 10));
    }

    [Fact]
    public void GateRequiresBoth()
    {
        Assert.True(CustomEffectEventCore.GateRequiresCheckedAndTime(true, 10));
    }

    // ===================== 四、吸血 =====================

    [Fact]
    public void TimeIsPercentageForDrain()
    {
        // **同一字段在此处是百分比、在其它槽是时长**
        Assert.True(CustomEffectEventCore.TimeIsPercentageForDrain());
    }

    [Fact]
    public void DrainAmountCalculation()
    {
        Assert.Equal(10, CustomEffectEventCore.DrainAmount(100, 10));   // 100/100*10
        Assert.Equal(50, CustomEffectEventCore.DrainAmount(1000, 5));   // 1000/100*5
        Assert.Equal(0, CustomEffectEventCore.DrainAmount(50, 0));
    }

    [Fact]
    public void DrainRequiresPositiveGain()
    {
        Assert.True(CustomEffectEventCore.DrainRequiresPositiveGain(1));
        Assert.False(CustomEffectEventCore.DrainRequiresPositiveGain(0));
        Assert.False(CustomEffectEventCore.DrainRequiresPositiveGain(-1));
    }

    [Fact]
    public void HpDrainUsesInt64AndClampsToMax()
    {
        Assert.True(CustomEffectEventCore.HpDrainUsesInt64AndClampsToMax());
        Assert.Equal(150, CustomEffectEventCore.ClampHpAfterDrain(100, 50, 1000));
        Assert.Equal(1000, CustomEffectEventCore.ClampHpAfterDrain(900, 500, 1000));
    }

    [Fact]
    public void FullHpAssignedMaxHp()
    {
        Assert.True(CustomEffectEventCore.FullHpAssignedMaxHp());
        Assert.Equal(100, CustomEffectEventCore.ClampHpAfterDrain(100, 50, 100));
    }

    [Fact]
    public void HpDrainDoesNotReduceTarget()
    {
        Assert.True(CustomEffectEventCore.HpDrainDoesNotReduceTarget());
    }

    // ===================== 五、吸蓝 =====================

    [Fact]
    public void MpDrainClampsByTargetMp()
    {
        // **用目标当前 MP 限制施法者回蓝量**（语义可疑，但为原文行为）
        Assert.True(CustomEffectEventCore.MpDrainClampsByTargetMp());
    }

    [Fact]
    public void TargetMpClampBoundaries()
    {
        Assert.True(CustomEffectEventCore.TargetMpClampBoundaries());
    }

    [Fact]
    public void DeductTargetMpClampsToZero()
    {
        Assert.Equal(0, CustomEffectEventCore.DeductTargetMp(30, 100));
        Assert.Equal(70, CustomEffectEventCore.DeductTargetMp(100, 30));
        Assert.Equal(0, CustomEffectEventCore.DeductTargetMp(0, 10));
    }

    [Fact]
    public void MpActuallyDeductedFromTarget()
    {
        Assert.True(CustomEffectEventCore.MpActuallyDeductedFromTarget());
        Assert.True(CustomEffectEventCore.MpDrainUsesInt64AndClampsToMax());
    }

    [Fact]
    public void DeductionUsesClampedGain()
    {
        Assert.True(CustomEffectEventCore.DeductionUsesClampedGain());
    }

    [Fact]
    public void DrainSidesAreAsymmetric()
    {
        // **HP 只加自己、MP 加自己且减目标**
        Assert.True(CustomEffectEventCore.DrainSidesAreAsymmetric());
    }

    [Fact]
    public void HealthSpellChangedArgIsFifty()
    {
        Assert.True(CustomEffectEventCore.HealthSpellChangedArgIsFifty());
    }

    // ===================== 六、伤害计算 =====================

    [Fact]
    public void ComputeBaseDamageBranches()
    {
        Assert.Equal(999, CustomEffectEventCore.ComputeBaseDamage(true, 999, 100));
        Assert.Equal(100, CustomEffectEventCore.ComputeBaseDamage(false, 999, 100));
    }

    [Fact]
    public void GetMagStruckDamagePassesNull()
    {
        Assert.True(CustomEffectEventCore.GetMagStruckDamagePassesNull());
    }

    [Fact]
    public void NewAbilPowerIndexHardcodedToThree()
    {
        Assert.True(CustomEffectEventCore.NewAbilPowerIndexHardcodedToThree());
    }

    [Fact]
    public void SendMsgMagicIndexIsTwentyTwo()
    {
        Assert.True(CustomEffectEventCore.SendMsgMagicIndexIsTwentyTwo());
        Assert.Equal(20060, CustomEffectEventCore.RmMagStruckMine);
    }

    [Fact]
    public void NewAbilPowerAppliedUnconditionally()
    {
        Assert.True(CustomEffectEventCore.NewAbilPowerAppliedUnconditionally());
    }

    [Fact]
    public void TwoCommentedDamageBlocks()
    {
        Assert.True(CustomEffectEventCore.TwoCommentedDamageBlocks());
        Assert.Contains("元素增加攻击伤害", CustomEffectEventCore.CommentedElementalBonus);
        Assert.Contains("StruckDamage", CustomEffectEventCore.CommentedStruckDamage);
    }

    [Fact]
    public void CommentedBonusLacksObjectPrefix()
    {
        Assert.True(CustomEffectEventCore.CommentedBonusLacksObjectPrefix());
    }

    [Fact]
    public void DrainGatedOnPositiveDamage()
    {
        Assert.True(CustomEffectEventCore.DrainGatedOnPositiveDamage());
        Assert.True(CustomEffectEventCore.DrainGateOpen(1, true));
        Assert.False(CustomEffectEventCore.DrainGateOpen(0, true));
        Assert.False(CustomEffectEventCore.DrainGateOpen(-5, true));
        Assert.False(CustomEffectEventCore.DrainGateOpen(1, false));
    }

    [Fact]
    public void ZeroDamageNoDrain()
    {
        Assert.True(CustomEffectEventCore.ZeroDamageNoDrain());
    }

    // ===================== 七、812 的优先级缺陷 =====================

    [Fact]
    public void AndBindsTighterThanOr()
    {
        Assert.True(CustomEffectEventCore.AndBindsTighterThanOr());
    }

    [Fact]
    public void OwnerNilCheckParenthesizedWrong()
    {
        Assert.True(CustomEffectEventCore.OwnerNilCheckParenthesizedWrong());
        Assert.Contains("or (m_OwnBaseObject.m_boGhost)", CustomEffectEventCore.OwnerNilCheckSource);
    }

    [Fact]
    public void NullOwnerGhostReturnsTrueViaBug()
    {
        // **owner 为 nil 且 ghost 为真时，错误写法仍返回真 → nil 检查无效**
        Assert.True(CustomEffectEventCore.NullOwnerGhostReturnsTrueViaBug());
    }

    [Fact]
    public void BuggyOwnerCheckTruthTable()
    {
        // owner 非 nil 时行为正确
        Assert.True(CustomEffectEventCore.BuggyOwnerCheck(false, true, false));
        Assert.True(CustomEffectEventCore.BuggyOwnerCheck(false, false, true));
        Assert.False(CustomEffectEventCore.BuggyOwnerCheck(false, false, false));
    }

    [Fact]
    public void CorrectFormNullSafe()
    {
        Assert.True(CustomEffectEventCore.CorrectFormNullSafe());
        Assert.False(CustomEffectEventCore.CorrectOwnerCheck(true, false, true));
        Assert.False(CustomEffectEventCore.CorrectOwnerCheck(true, true, true));
    }

    [Fact]
    public void TwoFormsAgreeWhenOwnerNonNull()
    {
        Assert.True(CustomEffectEventCore.TwoFormsAgreeWhenOwnerNonNull());
    }

    [Fact]
    public void TwoFormsDifferOnlyWhenOwnerNull()
    {
        // 分歧只在 owner 为 nil 时出现——正是缺陷所在
        Assert.True(CustomEffectEventCore.TwoFormsDifferOnlyWhenOwnerNull());
    }

    [Fact]
    public void SameNilBugRepeatedAt974()
    {
        Assert.True(CustomEffectEventCore.SameNilBugRepeatedAt974());
        Assert.Equal(new[] { 812, 974 }, CustomEffectEventCore.OwnerNilBugLines);
    }

    [Fact]
    public void TargetLoopOwnershipCheckIsCorrect()
    {
        // **同一函数内两种写法、一处对一处错**
        Assert.True(CustomEffectEventCore.TargetLoopOwnershipCheckIsCorrect());
        Assert.True(CustomEffectEventCore.TwoStylesInOneFunction());
        Assert.DoesNotContain(" or ", CustomEffectEventCore.TargetLoopSource);
    }

    [Fact]
    public void OwnerDeathPatchCommentHasDate()
    {
        Assert.True(CustomEffectEventCore.OwnerDeathPatchCommentHasDate());
        Assert.Contains("特效发起者死亡", CustomEffectEventCore.OwnerDeathPatchComment);
    }

    // ===================== 八、两处发起者死亡处理 =====================

    [Fact]
    public void TwoOwnerDeathHandlingsDiffer()
    {
        // 812 只解引用；974 关闭事件
        Assert.True(CustomEffectEventCore.TwoOwnerDeathHandlingsDiffer());
        Assert.Equal(2, CustomEffectEventCore.OwnerDeathHandlings.Length);
    }

    [Fact]
    public void OwnerDeathHandlingDetails()
    {
        Assert.Equal(812, CustomEffectEventCore.OwnerDeathHandlings[0].Line);
        Assert.Contains("仅解引用", CustomEffectEventCore.OwnerDeathHandlings[0].Effect);
        Assert.Equal(974, CustomEffectEventCore.OwnerDeathHandlings[1].Line);
        Assert.Contains("关闭事件", CustomEffectEventCore.OwnerDeathHandlings[1].Effect);
    }

    [Fact]
    public void OwnerDeathCloseIsConfigGated()
    {
        Assert.True(CustomEffectEventCore.OwnerDeathCloseIsConfigGated());
    }

    [Fact]
    public void ConfigBlockExtendsFireBurnPattern()
    {
        Assert.True(CustomEffectEventCore.ConfigBlockExtendsFireBurnPattern());
        Assert.Equal("boDisableChangeMapFireCross", CustomEffectEventCore.ConfigKey);
    }

    [Fact]
    public void SuperManExemptionAlsoCommentedHere()
    {
        Assert.True(CustomEffectEventCore.SuperManExemptionAlsoCommentedHere());
        Assert.True(CustomEffectEventCore.SuperManCommentIdenticalToFireBurn());
        Assert.Equal(new[] { 607, 967 }, CustomEffectEventCore.SuperManCommentLines);
    }

    // ===================== 九、节流 =====================

    [Fact]
    public void AttackIntervalThrottle()
    {
        // 间隔 1 秒：1500-500=1000 不触发；1501-500=1001 触发
        Assert.True(CustomEffectEventCore.AttackIntervalThrottle());
    }

    [Fact]
    public void AttackDueBoundaries()
    {
        // **严格 `>`**：恰好等于间隔时不到期
        Assert.False(CustomEffectEventCore.AttackDue(3000, 0, 3));
        Assert.True(CustomEffectEventCore.AttackDue(3001, 0, 3));
        Assert.False(CustomEffectEventCore.AttackDue(2999, 0, 3));
        Assert.False(CustomEffectEventCore.AttackDue(3000, 0, 4));
        Assert.True(CustomEffectEventCore.AttackDue(4001, 0, 4));
    }

    [Fact]
    public void FirstRunDependsOnRunTickInitial()
    {
        Assert.True(CustomEffectEventCore.FirstRunDependsOnRunTickInitial());
    }

    [Fact]
    public void TwoLayersOfThrottling()
    {
        Assert.True(CustomEffectEventCore.TwoLayersOfThrottling());
        Assert.Equal(250u, CustomEffectEventCore.ManagerThrottleMs);
    }

    [Fact]
    public void OuterThrottleLimitsPrecision()
    {
        // 内部间隔 1 秒 > 外层 250ms → 外层更细，不构成瓶颈
        Assert.True(CustomEffectEventCore.OuterThrottleLimitsPrecision(1));
    }

    // ===================== 十、取目标分派 =====================

    [Fact]
    public void ZeroRangeMeansSingleCell()
    {
        Assert.True(CustomEffectEventCore.ZeroRangeMeansSingleCell());
        Assert.Equal("GetMovingObject", CustomEffectEventCore.TargetApiFor(0));
    }

    [Fact]
    public void TwoTargetApis()
    {
        Assert.True(CustomEffectEventCore.TwoTargetApis());
        Assert.Equal("GetRangeBaseObject", CustomEffectEventCore.TargetApiFor(1));
        Assert.Equal("GetRangeBaseObject", CustomEffectEventCore.TargetApiFor(10));
    }

    [Fact]
    public void NegativeRangeFallsToRangeApi()
    {
        Assert.True(CustomEffectEventCore.NegativeRangeFallsToRangeApi());
    }

    [Fact]
    public void TwoPathsHandleNilDifferently()
    {
        Assert.True(CustomEffectEventCore.TwoPathsHandleNilDifferently());
        Assert.True(CustomEffectEventCore.GetMovingObjectThirdArgTrue());
    }

    // ===================== 十一、m_Envir = nil =====================

    [Fact]
    public void NullEnvirSkipsDamage()
    {
        Assert.True(CustomEffectEventCore.NullEnvirSkipsDamage());
    }

    [Fact]
    public void NullEnvirStillRefreshesRunTick()
    {
        // **空跑一轮但仍然重置了节流**
        Assert.True(CustomEffectEventCore.NullEnvirStillRefreshesRunTick());
        Assert.True(CustomEffectEventCore.NullEnvirBurnsOneThrottleWindow());
    }

    [Fact]
    public void ListCreatedEvenWhenEnvirNull()
    {
        Assert.True(CustomEffectEventCore.ListCreatedEvenWhenEnvirNull());
    }

    [Fact]
    public void RunRoundIdleWhenEnvirNull()
    {
        uint runTick = 0;

        var r = CustomEffectEventCore.RunRound(
            now: 2000, ref runTick, attackInterval: 1,
            targetIsProper: new List<bool> { true },
            envirNull: true,
            targetHp: new List<int> { 100 },
            targetMp: new List<int> { 100 },
            ignoreDefence: false, rawDamage: 100, magStruckResult: 0);

        Assert.True(r.Throttled);
        Assert.True(r.IdleDueToNullEnvir);
        Assert.Equal(2000u, runTick);   // **仍被刷新**
        Assert.Equal(0, r.TargetCount);
    }

    [Fact]
    public void RunRoundThrottledSkips()
    {
        uint runTick = 0;

        var r = CustomEffectEventCore.RunRound(
            now: 500, ref runTick, attackInterval: 1,
            targetIsProper: new List<bool> { true },
            envirNull: false,
            targetHp: new List<int> { 100 },
            targetMp: new List<int> { 100 },
            ignoreDefence: false, rawDamage: 100, magStruckResult: 0);

        Assert.False(r.Throttled);
        Assert.Equal(0u, runTick);
    }

    [Fact]
    public void RunRoundRunsNormally()
    {
        uint runTick = 0;

        var r = CustomEffectEventCore.RunRound(
            now: 2000, ref runTick, attackInterval: 1,
            targetIsProper: new List<bool> { true, false, true },
            envirNull: false,
            targetHp: new List<int> { 100, 100, 100 },
            targetMp: new List<int> { 100, 100, 100 },
            ignoreDefence: false, rawDamage: 100, magStruckResult: 0);

        Assert.True(r.Throttled);
        Assert.False(r.IdleDueToNullEnvir);
        Assert.Equal(3, r.TargetCount);
        Assert.Equal(2000u, runTick);
    }
}
