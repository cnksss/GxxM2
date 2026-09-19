using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J143：`_Attack`（35831-36051）、`GetAttackDir` 两重载（27050-27111）、
/// `GetAttackPowerMax`（2886-2924）、`GetNextDamage`（2926-2936）、`GetAttackPower()`（2938-2941）1:1 测试。
/// </summary>
public sealed class AttackCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(AttackCore.ConstantsMatchSource());
        Assert.True(AttackCore.MessageValues());
    }

    [Fact]
    public void ConstantValues()
    {
        Assert.Equal(20048, AttackCore.RmStruck);
        Assert.Equal(30005, AttackCore.Rm10101);
        Assert.Equal(99, AttackCore.RcMoonObject);
        Assert.Equal(150, AttackCore.RcPlayMoster);
        Assert.Equal(5, AttackCore.PoisonStone);
        Assert.Equal(6, AttackCore.DrLeft);
        Assert.Equal(3, AttackCore.DrDownRight);
        Assert.Equal(200, AttackCore.StruckDelay);
        Assert.Equal("FT", AttackCore.ReboundSeventhParam);
    }

    // ===================== 一、wHitMode → nMagicID =====================

    [Fact]
    public void HitModeTableComplete()
    {
        Assert.True(AttackCore.HitModeTableComplete());
        Assert.True(AttackCore.HitModeTableValues());
        Assert.Equal(15, AttackCore.HitModeMagic.Count);
        Assert.True(AttackCore.FifteenAttackSkills());
    }

    [Fact]
    public void HitModeTableValuesExplicit()
    {
        Assert.Equal(7, AttackCore.MagicIdFor(3));
        Assert.Equal(12, AttackCore.MagicIdFor(4));
        Assert.Equal(25, AttackCore.MagicIdFor(5));
        Assert.Equal(26, AttackCore.MagicIdFor(7));
        Assert.Equal(40, AttackCore.MagicIdFor(8));
        Assert.Equal(42, AttackCore.MagicIdFor(9));
        Assert.Equal(43, AttackCore.MagicIdFor(10));
        Assert.Equal(56, AttackCore.MagicIdFor(15));
        Assert.Equal(60, AttackCore.MagicIdFor(12));
        Assert.Equal(61, AttackCore.MagicIdFor(13));
        Assert.Equal(62, AttackCore.MagicIdFor(14));
        Assert.Equal(66, AttackCore.MagicIdFor(11));
        Assert.Equal(101, AttackCore.MagicIdFor(16));
        Assert.Equal(102, AttackCore.MagicIdFor(17));
        Assert.Equal(103, AttackCore.MagicIdFor(18));
    }

    [Fact]
    public void HitModeGaps()
    {
        // **取值不连续：6 缺席、0/1/2/19+ 全落 else**
        Assert.True(AttackCore.HitModeGaps());
        Assert.Equal(0, AttackCore.MagicIdFor(6));
        Assert.Equal(0, AttackCore.MagicIdFor(19));
        Assert.Equal(0, AttackCore.MagicIdFor(999));
        Assert.True(AttackCore.ElseReturnsZero());
    }

    [Fact]
    public void HitModeOrderIsScrambled()
    {
        Assert.True(AttackCore.HitModeOrderIsScrambled());
        Assert.True(AttackCore.SourceOrderHasFifteenBeforeTwelve());
    }

    [Fact]
    public void GuardAlwaysUsesZero()
    {
        // **J140 护卫 wHitMode 恒 0 → 技能编号恒 0**
        Assert.True(AttackCore.GuardAlwaysUsesZero());
        Assert.Equal(0, AttackCore.MagicIdFor(0));
    }

    // ===================== 二、奴隶折扣 =====================

    [Fact]
    public void SlaveDiscountTruthTable()
    {
        // **三条件缺一不可**
        Assert.True(AttackCore.SlaveDiscountTruthTable());
        Assert.True(AttackCore.HeroExemptFromDiscount());
        Assert.True(AttackCore.CopyMonExemptFromDiscount());
        Assert.True(AttackCore.ApplySlaveDiscountValues());
    }

    [Fact]
    public void SlaveDiscountValues()
    {
        Assert.True(AttackCore.SlaveDiscount(true, 80, false));
        Assert.False(AttackCore.SlaveDiscount(false, 80, false));
        Assert.False(AttackCore.SlaveDiscount(true, 1, false));
        Assert.False(AttackCore.SlaveDiscount(true, 80, true));
    }

    [Fact]
    public void NullTargetExitsEarly()
    {
        Assert.True(AttackCore.NullTargetExitsEarly());
    }

    [Fact]
    public void CommentedSuperShieldBlock()
    {
        Assert.True(AttackCore.CommentedSuperShieldBlock());
        Assert.True(AttackCore.SuperShieldHadFiveSkillSlots());
        Assert.True(AttackCore.SuperShieldSlotValues());
        Assert.True(AttackCore.LastSlotIsRange());
        Assert.Equal(5, AttackCore.SuperShieldSlots.Length);
    }

    // ===================== 三、nCheckCode =====================

    [Fact]
    public void CheckCodeStages()
    {
        Assert.True(AttackCore.CheckCodeStageValues());
        Assert.True(AttackCore.ThreeFamilies());
        Assert.True(AttackCore.FortyTwoIsImmediatelyOverwritten());
        Assert.True(AttackCore.FortyTwoThenFortyThree());
        Assert.Equal(10, AttackCore.CheckCodeStages.Length);
    }

    [Fact]
    public void CheckCodeStageValuesExplicit()
    {
        Assert.Equal(new[] { 0, 4, 41, 42, 43, 5, 600, 601, 602, 603 }, AttackCore.CheckCodeStages);
    }

    [Fact]
    public void ExceptionFormatHasNameAndCode()
    {
        Assert.True(AttackCore.ExceptionFormatHasNameAndCode());
        Assert.True(AttackCore.FormatExceptionValue());
        Assert.True(AttackCore.TwoMessagesOnException());
        Assert.Equal("[Exception] TBaseObject._Attack Name:= 测试 Code:=602",
            AttackCore.FormatException("测试", 602));
    }

    [Fact]
    public void FifthExceptionStyle()
    {
        // **第五种异常定位风格**
        Assert.True(AttackCore.FifthExceptionStyle());
        Assert.True(AttackCore.FiveStylesPresent());
        Assert.True(AttackCore.ExceptionStylesCount());
        Assert.Equal(5, AttackCore.ExceptionStyles.Length);
        Assert.Contains("HookObjectAttack", AttackCore.HookError);
        Assert.True(AttackCore.HookErrorUnusedHere());
    }

    // ===================== 四、命中判定 =====================

    [Fact]
    public void HitRollTruthTable()
    {
        Assert.True(AttackCore.HitRollTruthTable());
        Assert.True(AttackCore.RandomUpperBoundIsNMinusOne());
        Assert.True(AttackCore.HitMisses(10, 5, 5));
        Assert.False(AttackCore.HitMisses(10, 5, 4));
    }

    [Fact]
    public void ZeroHitPointAlwaysMisses()
    {
        // **命中为 0 时必然失手**
        Assert.True(AttackCore.ZeroHitPointAlwaysMisses());
    }

    [Fact]
    public void ZeroSpeedRandomIsZero()
    {
        Assert.True(AttackCore.ZeroSpeedRandomIsZero());
    }

    [Fact]
    public void ImproperTargetZeroesPower()
    {
        Assert.True(AttackCore.ImproperTargetZeroesPower());
        Assert.True(AttackCore.ImproperTargetDoesNotExit());
    }

    // ===================== 五、防御减免 =====================

    [Fact]
    public void CanCloseDefenseBranches()
    {
        Assert.True(AttackCore.CanCloseDefenseBranches());
        Assert.True(AttackCore.MatchesJ139OpposesJ134());
        Assert.False(AttackCore.UsesFourArgVersion(false));
        Assert.True(AttackCore.UsesFourArgVersion(true));
    }

    [Fact]
    public void FourArgLiteralIsFour()
    {
        Assert.True(AttackCore.FourArgLiteralIsFour());
        Assert.Equal(4, AttackCore.FourArgLiteral());
    }

    [Fact]
    public void MagicAcInfoHardcodedNil()
    {
        Assert.True(AttackCore.MagicAcInfoHardcodedNil());
        Assert.True(AttackCore.CommentedMagicAcListLookup());
        Assert.Contains("m_MagicACList", AttackCore.CommentedMagicAcLookup);
    }

    [Fact]
    public void TwoNewAbilPowerCalls()
    {
        Assert.True(AttackCore.TwoNewAbilPowerCalls());
        Assert.True(AttackCore.FirstOnTargetSecondOnSelf());
        Assert.Equal(2, AttackCore.NewAbilPowerCalls.Length);
        Assert.Equal(2, AttackCore.NewAbilPowerReceivers.Length);

        // 序号与注释
        Assert.Equal(2, AttackCore.NewAbilPowerCalls[0].Which);
        Assert.Equal("物伤减少", AttackCore.NewAbilPowerCalls[0].Comment);
        Assert.Equal(1, AttackCore.NewAbilPowerCalls[1].Which);
        Assert.Equal("1元素增加攻击伤害", AttackCore.NewAbilPowerCalls[1].Comment);
    }

    [Fact]
    public void PowerRateAddHasDate()
    {
        Assert.True(AttackCore.PowerRateAddHasDate());
        Assert.Contains("2020-09-12", AttackCore.PowerRateAddDate);
    }

    // ===================== 六、人类目标减免链 =====================

    [Fact]
    public void HumanTargetTriple()
    {
        Assert.True(AttackCore.HumanTargetTriple());
        Assert.True(AttackCore.IsHumanTargetRace(0));
        Assert.True(AttackCore.IsHumanTargetRace(1));
        Assert.True(AttackCore.IsHumanTargetRace(150));
        Assert.False(AttackCore.IsHumanTargetRace(80));
    }

    [Fact]
    public void PlayMosterCommentedOut()
    {
        // **RC_PLAYMOSTER 在 IsHuman 判断里被注释掉**
        Assert.True(AttackCore.PlayMosterCommentedOut());
        Assert.True(AttackCore.PlayMosterCommentForm());
        Assert.True(AttackCore.IsHumanValues());
        Assert.False(AttackCore.IsHumanByRace(150));
    }

    [Fact]
    public void MasterCountsAsHuman()
    {
        Assert.True(AttackCore.MasterCountsAsHuman());
        Assert.True(AttackCore.HumanCheck(false, true, 0));
        Assert.False(AttackCore.HumanCheck(false, true, 150));
    }

    [Fact]
    public void AbsorbMgrSemantics()
    {
        // **人类不吃吸收表、非人类吃（探针确认极性）**
        Assert.True(AttackCore.AbsorbMgrPolarity());
        Assert.False(AttackCore.AbsorbMgrOnlyForNonHuman(true));
        Assert.True(AttackCore.AbsorbMgrOnlyForNonHuman(false));
        Assert.True(AttackCore.AbsorbMgrUsesAttackerName());
        Assert.True(AttackCore.AbsorbCommentHasDate());
    }

    [Fact]
    public void TrainingNgTruthTable()
    {
        // **`>=` 而非 `>`、三处 Max(0,...) 保护**
        Assert.True(AttackCore.TrainingNgTruthTable());
        Assert.True(AttackCore.TrainingNgUsesGreaterOrEqual());
        Assert.True(AttackCore.TrainingNgThreeClamps());
        Assert.True(AttackCore.TrainingNgClampValues());
        Assert.Equal((0, 0), AttackCore.ApplyTrainingNg(50, 100, 100, 80));
        Assert.Equal((70, 200), AttackCore.ApplyTrainingNg(100, 300, 100, 30));
    }

    [Fact]
    public void SuckGateTriple()
    {
        Assert.True(AttackCore.SuckGateTriple());
        Assert.True(AttackCore.SuckGate(1, 1, 1));
        Assert.False(AttackCore.SuckGate(0, 1, 1));
    }

    [Fact]
    public void SuckProbabilityIsLessOrEqual()
    {
        // **`<=`，与 J139 的 `=` 不同**
        Assert.True(AttackCore.SuckProbabilityIsLessOrEqual());
        Assert.True(AttackCore.SuckProbabilityHit(0, 0));
        Assert.False(AttackCore.SuckProbabilityHit(6, 5));
    }

    [Fact]
    public void SuckDividesByThousand()
    {
        Assert.True(AttackCore.SuckDividesByThousand());
        Assert.Equal(100, AttackCore.SuckPoint(1000, 100));
        Assert.Equal(50, AttackCore.SuckPoint(500, 100));
    }

    [Fact]
    public void SuckClampedToPool()
    {
        Assert.True(AttackCore.SuckClampedToPool());
        Assert.True(AttackCore.SuckMissDoesNothing());
        Assert.Equal((30, 0, 70), AttackCore.ApplySuck(1000, 100, 30, 100, 0));
        Assert.Equal((100, 400, 0), AttackCore.ApplySuck(1000, 100, 500, 100, 0));
    }

    [Fact]
    public void ContinuousGateTruthTable()
    {
        Assert.True(AttackCore.ContinuousGateTruthTable());
        Assert.True(AttackCore.ContinuousCommentHasDate());
    }

    [Fact]
    public void ContinuousProtectUsesMax()
    {
        Assert.True(AttackCore.ContinuousProtectUsesMax());
        Assert.True(AttackCore.ProtRandomPlusOne());
        Assert.True(AttackCore.AbsorbRateCappedAt100());
        Assert.True(AttackCore.ApplyContinuousValues());

        // 探针实测值：取较大者 → 20 与 50 取 50
        Assert.Equal(50, AttackCore.AbsorbRate(10, 50, 20));
        Assert.Equal(80, AttackCore.AbsorbRate(10, 50, 80));
    }

    // ===================== 七、插件与结算 =====================

    [Fact]
    public void PluginHookOutsideHumanBranch()
    {
        Assert.True(AttackCore.PluginHookOutsideHumanBranch());
    }

    [Fact]
    public void AttackRateAppliedLast()
    {
        Assert.True(AttackCore.AttackRateAppliedLast());
        Assert.True(AttackCore.GuardRateIsOne());
        Assert.True(AttackCore.AttackRateCommentHasDate());
        Assert.Equal(150, AttackCore.ApplyAttackRate(100, 1.5f));
    }

    [Fact]
    public void PowerPipelineFourSteps()
    {
        Assert.True(AttackCore.GetNextDamageThenPowerMax());
        Assert.True(AttackCore.PowerPipelineFourSteps());
        Assert.Equal(4, AttackCore.PowerPipeline.Length);
        Assert.True(AttackCore.PowerMaxCommentHasXs());
    }

    [Fact]
    public void MpChangeTruthTable()
    {
        // **MP 掉了也算受到打击**
        Assert.True(AttackCore.MpChangeTruthTable());
        Assert.True(AttackCore.TypoInVarName());
        Assert.True(AttackCore.TypoIsChagne());
        Assert.Equal("IsChagneMP", AttackCore.TypoVarName);
    }

    [Fact]
    public void SendsStruckTruthTable()
    {
        Assert.True(AttackCore.SendsStruckTruthTable());
        Assert.True(AttackCore.StruckMsgDelayIs200());
        Assert.True(AttackCore.SeventhParamIsMagicIdString());
        Assert.True(AttackCore.ZeroDamageStillSendsStruck());
        Assert.True(AttackCore.StruckCommentHasDate());
        Assert.True(AttackCore.SuckStruckCommentHasDate());
    }

    // ===================== 八、三种状态附着 =====================

    [Fact]
    public void ThreeStatusEffects()
    {
        Assert.True(AttackCore.ThreeStatusEffects());
        Assert.True(AttackCore.ThreeStatusMutuallySymmetric());
        Assert.True(AttackCore.ThreeStatusCalls());
        Assert.Equal(3, AttackCore.StatusEffects.Length);
        Assert.Equal(3, AttackCore.StatusCalls.Length);
    }

    [Fact]
    public void ParalysisUsesPoisonStone()
    {
        Assert.True(AttackCore.ParalysisUsesPoisonStone());
        Assert.True(AttackCore.ParalysisThirdArgZero());
        Assert.True(AttackCore.ParalysisConfigCommented());
        Assert.Equal(5, AttackCore.PoisonStone);
    }

    [Fact]
    public void TriggerRateBoundary()
    {
        // **`Random(100) < 触发率` 严格小于**
        Assert.True(AttackCore.TriggerRateBoundary());
        Assert.True(AttackCore.TriggerRateStrictLess(4, 5));
        Assert.False(AttackCore.TriggerRateStrictLess(5, 5));
    }

    [Fact]
    public void NegativeResistanceBackfires()
    {
        // **抗性为负按 0 处理 → Random(0) 恒 0 → 必然命中**
        Assert.True(AttackCore.NegativeResistanceBackfires());
        Assert.True(AttackCore.ResistModulusValues());
        Assert.Equal(0, AttackCore.ResistModulus(-10, 0));
        Assert.Equal(15, AttackCore.ResistModulus(10, 5));
    }

    [Fact]
    public void SetsPoisonHitterFirst()
    {
        Assert.True(AttackCore.SetsPoisonHitterFirst());
        Assert.True(AttackCore.FreezeCobwebSameDate());
    }

    // ===================== 九、伤害反弹 =====================

    [Fact]
    public void ReboundSemantics()
    {
        Assert.True(AttackCore.ReboundCallsSelfWithNil());
        Assert.True(AttackCore.ReboundThreeArgs());
        Assert.Equal(3, AttackCore.ReboundCallArgs.Length);
        Assert.Equal(new[] { "Self", "nil", "0" }, AttackCore.ReboundCallArgs);
    }

    [Fact]
    public void ReboundSeventhParamIsFt()
    {
        // **第 7 参是字面量 'FT'，不是技能编号**
        Assert.True(AttackCore.ReboundSeventhParamIsFt());
        Assert.True(AttackCore.ReboundParamIsNotMagicId());
        Assert.True(AttackCore.ReboundDelayIs200());
        Assert.True(AttackCore.ReboundCommentPresent());
    }

    [Fact]
    public void ReboundGateBoundary()
    {
        Assert.True(AttackCore.ReboundGateBoundary());
        Assert.False(AttackCore.ReboundGate(0));
        Assert.True(AttackCore.ReboundGate(1));
        Assert.True(AttackCore.ResultTrueOnlyInsidePowerBranch());
    }

    // ===================== 十、收尾广播 =====================

    [Fact]
    public void NonPlayerGetsExtraTruthTable()
    {
        Assert.True(AttackCore.NonPlayerGetsExtraTruthTable());
        Assert.True(AttackCore.PlayerOnlyGetsDelayed());
        Assert.True(AttackCore.TwoSendStyles());
        Assert.Equal(2, AttackCore.SendStyles.Length);
    }

    // ===================== 十一、GetAttackDir =====================

    [Fact]
    public void RangeOverloadSemantics()
    {
        Assert.True(AttackCore.RangeOverloadUsesRangeWalk());
        Assert.True(AttackCore.RangeOverloadFourSteps());
        Assert.Equal(4, AttackCore.RangeOverloadSteps.Length);
    }

    [Fact]
    public void EightNeighbourGateTruthTable()
    {
        Assert.True(AttackCore.EightNeighbourGateTruthTable());
        Assert.True(AttackCore.EightNeighbourGateIsChebyshevOne());
        Assert.True(AttackCore.EightNeighboursCoverAll());
    }

    [Fact]
    public void EightNeighbourPriority()
    {
        Assert.True(AttackCore.EightNeighbourPriorityEight());
        Assert.True(AttackCore.FirstFourCardinalThenDiagonal());
        Assert.True(AttackCore.PriorityMatchesJ137());
        Assert.True(AttackCore.ChooseAttackDirValues());
        Assert.Equal(8, AttackCore.EightNeighbourPriority.Length);
    }

    [Fact]
    public void ChooseAttackDirValues()
    {
        Assert.Equal(6, AttackCore.ChooseAttackDir(-1, 0));
        Assert.Equal(2, AttackCore.ChooseAttackDir(1, 0));
        Assert.Equal(0, AttackCore.ChooseAttackDir(0, -1));
        Assert.Equal(4, AttackCore.ChooseAttackDir(0, 1));
        Assert.Equal(7, AttackCore.ChooseAttackDir(-1, -1));
        Assert.Equal(1, AttackCore.ChooseAttackDir(1, -1));
        Assert.Equal(5, AttackCore.ChooseAttackDir(-1, 1));
        Assert.Equal(3, AttackCore.ChooseAttackDir(1, 1));
    }

    [Fact]
    public void FallbackZeroUnreachable()
    {
        // **末尾 btDir := 0 兜底不可达（穷举验证）**
        Assert.True(AttackCore.FallbackZeroUnreachable());
    }

    // ===================== 十二、威力辅助函数 =====================

    [Fact]
    public void ChangeAbilityTwoModes()
    {
        // **正向增量被 High(m_Abil.MP) 钳回原值 → 不产生效果**
        Assert.True(AttackCore.ChangeAbilityTwoModes());
        Assert.True(AttackCore.OnlyNegativeDeltaTakesEffect());
        Assert.True(AttackCore.NegativeDeltaModesDiffer());
        Assert.True(AttackCore.PercentageModeScalesByCurrentValue());

        // 探针实测：绝对值 200-50=150；比例 200+round(2×-50)=100
        Assert.Equal(150, AttackCore.ComputeMaxMp(200, true, -50, false));
        Assert.Equal(100, AttackCore.ComputeMaxMp(200, true, -50, true));
    }

    [Fact]
    public void ChangeAbilityValues()
    {
        Assert.Equal(100, AttackCore.ComputeMaxMp(100, true, 50, true));
        Assert.Equal(70, AttackCore.ComputeMaxMp(100, true, -30, false));
        Assert.Equal(100, AttackCore.ComputeMaxMp(100, false, 50, true));
        Assert.Equal(100, AttackCore.ComputeMaxMp(100, true, 0, true));
    }

    [Fact]
    public void Int64ClampToZeroAndHigh()
    {
        Assert.True(AttackCore.Int64ClampToZeroAndHigh());
        Assert.Equal(0, AttackCore.ComputeMaxMp(100, true, -500, false));
        Assert.Equal(100, AttackCore.ComputeMaxMp(100, true, 99999, false));
    }

    [Fact]
    public void PowerMaxFourGates()
    {
        Assert.True(AttackCore.PowerMaxFourGates());
        Assert.True(AttackCore.PlayerHeroMoonExempt());
        Assert.True(AttackCore.PlayMosterRequiresSwitch());
        Assert.True(AttackCore.NormalMonsterAlwaysCapped());
        Assert.True(AttackCore.ApplyPowerMaxValues());
    }

    [Fact]
    public void PowerMaxValues()
    {
        Assert.Equal(5, AttackCore.ApplyPowerMax(10, 5, 80, false));
        Assert.Equal(3, AttackCore.ApplyPowerMax(3, 5, 80, false));
        Assert.Equal(10, AttackCore.ApplyPowerMax(10, 5, 0, false));
    }

    [Fact]
    public void CommentedGmScriptText()
    {
        // **注释里留着 GM 脚本文本**
        Assert.True(AttackCore.CommentedGmScriptText());
        Assert.True(AttackCore.GmScriptFiveLines());
        Assert.True(AttackCore.GmScriptHasDate());
        Assert.Equal(5, AttackCore.CommentedGmScript.Length);
        Assert.Contains("ChangeMonAbility 3 半兽人 2 = 10 0 325 323 20", AttackCore.CommentedGmScript);
    }

    [Fact]
    public void PowerMaxFixComments()
    {
        Assert.True(AttackCore.PowerMaxFixHasAuthor());
        Assert.True(AttackCore.DamageLimitSwitchHasDate());
        Assert.Contains("一支笔", AttackCore.PowerMaxFixComment);
        Assert.Contains("Cursor", AttackCore.DamageLimitSwitchComment);
    }

    [Fact]
    public void NextDamageIntegerDivisionFirst()
    {
        // **先整数除法再乘 → 丢精度**
        Assert.True(AttackCore.NextDamageIntegerDivisionFirst());
        Assert.True(AttackCore.NextDamageDiffersFromExact());
        Assert.Equal(100, AttackCore.NextDamage(150, 100));   // 150/100=1 → 100
        Assert.Equal(200, AttackCore.NextDamage(250, 100));   // 250/100=2 → 200
    }

    [Fact]
    public void NextDamageResetsRateTo100()
    {
        // **比率用完无条件复位为 100**
        Assert.True(AttackCore.NextDamageResetsRateTo100());
        Assert.True(AttackCore.NextDamageResetValues());
        Assert.True(AttackCore.NextDamageHighGuard());
        Assert.Equal((50, 100), AttackCore.NextDamageAndReset(100, 50));
        Assert.Equal((200, 100), AttackCore.NextDamageAndReset(100, 200));
    }

    [Fact]
    public void AttackPowerSecondArgAtLeastOne()
    {
        // **Max(..., 1) 而非 Max(..., 0)**
        Assert.True(AttackCore.AttackPowerSecondArgAtLeastOne());
        Assert.True(AttackCore.DegeneratesToOne());
        Assert.True(AttackCore.DiffersFromJ139ZeroGuard());
        Assert.Equal(1, AttackCore.AttackPowerSpan(10, 10));
        Assert.Equal(10, AttackCore.AttackPowerSpan(10, 20));
    }

    // ===================== 顶层仿真 =====================

    [Fact]
    public void PipelinePlain()
    {
        Assert.True(AttackCore.PipelinePlain());
    }

    [Fact]
    public void PipelineWithDiscount()
    {
        Assert.True(AttackCore.PipelineWithDiscount());
    }

    [Fact]
    public void PipelineWithCap()
    {
        Assert.True(AttackCore.PipelineWithCap());
    }

    [Fact]
    public void PipelinePlayerUncapped()
    {
        Assert.True(AttackCore.PipelinePlayerUncapped());
    }
}
