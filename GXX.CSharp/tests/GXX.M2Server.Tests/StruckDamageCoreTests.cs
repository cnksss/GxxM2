using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J145：`GetHitStruckDamage`（38956-39244）、`GetMagStruckDamage`（39246-39446）1:1 测试。
/// </summary>
public sealed class StruckDamageCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(StruckDamageCore.ConstantsMatchSource());
        Assert.True(StruckDamageCore.Skill31RatesDefaults());
        Assert.True(StruckDamageCore.Skill31RatesAreArithmetic());
        Assert.True(StruckDamageCore.MessageIdsMatchSource());
    }

    [Fact]
    public void ConstantValues()
    {
        Assert.Equal(3, StruckDamageCore.StrongShieldLevelThreshold);
        Assert.Equal(9, StruckDamageCore.Skill31RateCount);
        Assert.Equal(8, StruckDamageCore.DefaultOrdinarySkill31Rate);
        Assert.Equal(30, StruckDamageCore.DefaultSuperShiledPowerRate);
        Assert.Equal(0, StruckDamageCore.DefaultSuperShiledLevelUpDecPowerRate);
        Assert.Equal(5, StruckDamageCore.DefaultOpenSuperShiledRate);
        Assert.Equal(12, StruckDamageCore.DefaultPosionDecMacRate);
        Assert.Equal(1000, StruckDamageCore.ScriptPosionThousand);
        Assert.Equal(3000, StruckDamageCore.MagicLvExpDelay);
    }

    [Fact]
    public void MessageIds()
    {
        // **源码核对：护体神盾是 20177，不是凭印象的 20252**
        Assert.Equal(20177, StruckDamageCore.RmSendSuperShiledEffect);
        Assert.Equal(20178, StruckDamageCore.RmSendBlastHit);
        Assert.Equal(20093, StruckDamageCore.RmMagicLvExp);
        Assert.True(StruckDamageCore.SuperShieldMsgIdPositive());
    }

    [Fact]
    public void DefaultSwitches()
    {
        Assert.True(StruckDamageCore.CloseDefenseUseScaleDefaultsOff());
        Assert.True(StruckDamageCore.SuperShieldDisplayDefaultsOn());
        Assert.True(StruckDamageCore.PosionDecMacDefaultsOff());
    }

    [Fact]
    public void RoundHalfUpValues()
    {
        Assert.True(StruckDamageCore.RoundHalfUpValues());
        Assert.True(StruckDamageCore.HighIntegerValue());
    }

    [Fact]
    public void FunctionLineCounts()
    {
        Assert.True(StruckDamageCore.FunctionLineCounts());
    }

    // ===================== 一、nType 模式 =====================

    [Fact]
    public void HitTypeModes()
    {
        Assert.True(StruckDamageCore.HitTypeModeCount());
        Assert.Equal(5, StruckDamageCore.HitTypeModes.Length);
    }

    [Fact]
    public void OuterBlockTruthTable()
    {
        // **只有 nType = 4 跳过外层**
        Assert.True(StruckDamageCore.OuterBlockTruthTable());
        Assert.True(StruckDamageCore.TypeFourSkipsOuterKeepsShields(4));
        Assert.False(StruckDamageCore.OuterBlockEntered(4));
        Assert.True(StruckDamageCore.OuterBlockEntered(0));
    }

    [Fact]
    public void TypeTwoAndOneExitEarly()
    {
        Assert.True(StruckDamageCore.TypeTwoExitsEarly(2));
        Assert.True(StruckDamageCore.TypeOneExitsEarly(1, true));
        Assert.False(StruckDamageCore.TypeOneExitsEarly(1, false));
    }

    [Fact]
    public void TypeThreeSkipsAcAndMagicShield()
    {
        Assert.True(StruckDamageCore.TypeThreeSkipsAc(3));
        Assert.True(StruckDamageCore.TypeThreeKeepsForceAndSuper());
    }

    [Fact]
    public void MagTypeSemanticsOpposeHit()
    {
        // **两个函数的 nType 方向相反**
        Assert.True(StruckDamageCore.MagTypeZeroIsFull(0));
        Assert.False(StruckDamageCore.MagTypeZeroIsFull(1));
        Assert.True(StruckDamageCore.HitAndMagTypeSemanticsDiffer());
    }

    [Fact]
    public void MagExitsOnMagicAcInfo()
    {
        Assert.True(StruckDamageCore.MagExitsOnMagicAcInfo(true));
        Assert.False(StruckDamageCore.MagExitsOnMagicAcInfo(false));
        Assert.True(StruckDamageCore.MagEarlyResult(-5) == 0);
    }

    [Fact]
    public void ShieldComments()
    {
        Assert.True(StruckDamageCore.SevenShieldComments());
        Assert.True(StruckDamageCore.ShieldCommentsHaveDates());
        Assert.Equal(7, StruckDamageCore.ShieldComments.Length);
    }

    // ===================== 二、四个盾块 =====================

    [Fact]
    public void FourShieldBlocks()
    {
        Assert.True(StruckDamageCore.FourShieldBlocks());
        Assert.Equal(4, StruckDamageCore.ShieldBlocks.Length);
    }

    [Fact]
    public void StrongShieldConditionIsOr()
    {
        // **`NewLevel > 0` 或 `Level > 3`**
        Assert.True(StruckDamageCore.StrongShieldConditionIsOr());
        Assert.True(StruckDamageCore.LevelThreeIsStillNormal());
        Assert.True(StruckDamageCore.StrongShield(1, 0));
        Assert.True(StruckDamageCore.StrongShield(0, 4));
        Assert.False(StruckDamageCore.StrongShield(0, 3));
    }

    [Fact]
    public void RateArrayIndexClamping()
    {
        Assert.True(StruckDamageCore.RateArrayIndexClamping());
        Assert.True(StruckDamageCore.NewLevelZeroStopsAtFirst());
        Assert.Equal(10, StruckDamageCore.StrongShieldRate(0, StruckDamageCore.DefaultSkill31Rates));
        Assert.Equal(90, StruckDamageCore.StrongShieldRate(99, StruckDamageCore.DefaultSkill31Rates));
    }

    [Fact]
    public void NormalShieldLevelPlusOne()
    {
        // **普通盾是"等级 + 1 后乘基础倍率"**
        Assert.True(StruckDamageCore.NormalShieldLevelPlusOne());
        Assert.Equal(8, StruckDamageCore.NormalShieldRate(0, 8));
        Assert.Equal(32, StruckDamageCore.NormalShieldRate(3, 8));
    }

    [Fact]
    public void StrongAndNormalDiffer()
    {
        Assert.True(StruckDamageCore.StrongAndNormalDifferAtLevelFour());
    }

    [Fact]
    public void ClampIsElseIf()
    {
        Assert.True(StruckDamageCore.ClampIsElseIf());
        Assert.True(StruckDamageCore.DeductNeverNegative());
        Assert.Equal(0, StruckDamageCore.ClampDefense(-5, 100));
        Assert.Equal(100, StruckDamageCore.ClampDefense(150, 100));
    }

    [Fact]
    public void ShieldDefenseValues()
    {
        Assert.Equal(10, StruckDamageCore.StrongDefense(100, 10));
        Assert.Equal(16, StruckDamageCore.NormalDefense(100, 1, 8));
    }

    [Fact]
    public void CommentedScaleBlocks()
    {
        Assert.True(StruckDamageCore.CommentedScaleBlocksDisabled());
        Assert.True(StruckDamageCore.SevenCommentedScaleBlocks());
        Assert.Equal(7, StruckDamageCore.CommentedScaleBlockCount());
        Assert.True(StruckDamageCore.CommentedScaleUsesNewValueFour());
    }

    [Fact]
    public void ShieldCallbacks()
    {
        Assert.True(StruckDamageCore.ThreeShieldCallbacks());
        Assert.Equal(3, StruckDamageCore.ShieldCallbacks.Length);
    }

    [Fact]
    public void ForceShieldMsgCommentedOut()
    {
        Assert.True(StruckDamageCore.ForceShieldMsgCommentedOut());
        Assert.True(StruckDamageCore.ForceShieldCommentHasDate());
    }

    // ===================== 三、护体神盾 =====================

    [Fact]
    public void SuperShieldRaceGate()
    {
        Assert.True(StruckDamageCore.SuperShieldRaceGateTruthTable());
        Assert.True(StruckDamageCore.NormalMonsterNoSuperShield());
        Assert.False(StruckDamageCore.SuperShieldRaceGate(80));
    }

    [Fact]
    public void SuperShieldRateIsEqualZero()
    {
        // **`Random(...) = 0` 而非 `<=`**
        Assert.True(StruckDamageCore.SuperShieldRateIsEqualZero());
        Assert.True(StruckDamageCore.SuperShieldHit(0, 5));
        Assert.False(StruckDamageCore.SuperShieldHit(1, 5));
    }

    [Fact]
    public void SuperShieldFormulaUsesLevelSum()
    {
        Assert.True(StruckDamageCore.SuperShieldFormulaUsesLevelSum());
        Assert.True(StruckDamageCore.DefaultPerLevelRateIsZero());
        Assert.Equal(30, StruckDamageCore.SuperShieldDefense(100, 30, 0, 0, 0));
        Assert.Equal(35, StruckDamageCore.SuperShieldDefense(100, 30, 3, 2, 1));
    }

    [Fact]
    public void SuperShieldBoolAsIntParams()
    {
        Assert.True(StruckDamageCore.SuperShieldBoolAsIntParams());
        Assert.True(StruckDamageCore.SuperShieldMsgGateTruthTable());
        Assert.Equal((1, 0), StruckDamageCore.SuperShieldMsgParams(true, false));
    }

    [Fact]
    public void SuperShieldTrainGates()
    {
        Assert.True(StruckDamageCore.SuperShieldTrainFourGates());
        Assert.True(StruckDamageCore.TrainExcludesPlayMoster());
        Assert.False(StruckDamageCore.SuperShieldTrainGate(150, true, 0, 3, 1, 10));
    }

    [Fact]
    public void SuperShieldTrainDetails()
    {
        // **练级点数 1..3、消息延迟 3000ms、失败才发**
        Assert.True(StruckDamageCore.TrainPointRange());
        Assert.Equal(1, StruckDamageCore.TrainPoint(0));
        Assert.Equal(3, StruckDamageCore.TrainPoint(2));
        Assert.True(StruckDamageCore.SuperShieldTrainMsgDelay());
        Assert.True(StruckDamageCore.SendsOnlyWhenNoLevelUp(false));
        Assert.False(StruckDamageCore.SendsOnlyWhenNoLevelUp(true));
    }

    // ===================== 四、AC / MAC 取值 =====================

    [Fact]
    public void AcRangeShape()
    {
        // **探针实测：random=10 得 20（区间长度 11）**
        Assert.True(StruckDamageCore.AcRangeShape());
        Assert.True(StruckDamageCore.ModuloReturnsToAc1());
        Assert.Equal(20, StruckDamageCore.AcValue(10, 20, 10));
        Assert.Equal(10, StruckDamageCore.AcValue(10, 20, 11));
    }

    [Fact]
    public void EqualRangeYieldsSingleValue()
    {
        Assert.True(StruckDamageCore.EqualRangeYieldsSingleValue());
        Assert.Equal(15, StruckDamageCore.AcValue(15, 15, 0));
    }

    [Fact]
    public void InvertedRangeYieldsAc2PlusOne()
    {
        // **上下限倒挂 → 退化成 AC2 + 1（比上限还大 1）**
        Assert.True(StruckDamageCore.InvertedRangeYieldsAc2PlusOne());
        Assert.True(StruckDamageCore.InvertedRangeNoRandom());
        Assert.True(StruckDamageCore.InvertedRangeValueIsAc2PlusOne());
        Assert.Equal(11, StruckDamageCore.AcValue(20, 10, 0));
    }

    [Fact]
    public void AcMagicInfoThreeRaces()
    {
        Assert.True(StruckDamageCore.AcMagicInfoThreeRaces());
        Assert.Equal(80, StruckDamageCore.AcAfterMagicAcInfo(100, 0, 20, 0, 0));
        Assert.Equal(70, StruckDamageCore.AcAfterMagicAcInfo(100, 1, 0, 30, 0));
        Assert.Equal(50, StruckDamageCore.AcAfterMagicAcInfo(100, 80, 0, 0, 50));
    }

    [Fact]
    public void MagAndHitOneDirectionsOppose()
    {
        Assert.True(StruckDamageCore.MagMagicAcUsesPhysicalAc());
        Assert.True(StruckDamageCore.HitTypeOneUsesAcOnly());
        Assert.True(StruckDamageCore.MagAndHitOneDirectionsOppose());
    }

    // ===================== 五、红毒与脚本中毒 =====================

    [Fact]
    public void PosionDecMacValues()
    {
        Assert.True(StruckDamageCore.PosionDecMacValues());
        Assert.Equal(88, StruckDamageCore.PosionDecMac(100, true, true, 12));
        Assert.Equal(100, StruckDamageCore.PosionDecMac(100, false, true, 12));
    }

    [Fact]
    public void ScriptPosionUsesThousandMinus()
    {
        Assert.True(StruckDamageCore.ScriptPosionUsesThousandMinus());
        Assert.Equal(100, StruckDamageCore.ScriptPosion(100, true, 1, 0));
        Assert.Equal(50, StruckDamageCore.ScriptPosion(100, true, 1, 500));
        Assert.Equal(0, StruckDamageCore.ScriptPosion(100, true, 1, 1000));
    }

    [Fact]
    public void ScriptPosionClampedToThousand()
    {
        Assert.True(StruckDamageCore.ScriptPosionClampedToThousand());
        Assert.True(StruckDamageCore.ScriptPosionNeedsTypeOne());
        Assert.Equal(0, StruckDamageCore.ScriptPosion(100, true, 1, 5000));
        Assert.Equal(100, StruckDamageCore.ScriptPosion(100, true, 1, -500));
    }

    [Fact]
    public void PosionPathsDuplicatedNotIdentical()
    {
        // **两处路径重复但不相同：物理函数缺"红毒减魔御"**
        Assert.True(StruckDamageCore.TwoPosionDecPaths());
        Assert.True(StruckDamageCore.HitLacksPosionDecMac());
        Assert.True(StruckDamageCore.PosionPathsDuplicatedNotIdentical());
        Assert.True(StruckDamageCore.TwoPosionComments());
    }

    // ===================== 六、不死系加成与收尾 =====================

    [Fact]
    public void UndeadBonusTruthTable()
    {
        Assert.True(StruckDamageCore.UndeadBonusThreeRaces());
        Assert.True(StruckDamageCore.UndeadBonusTruthTable());
        Assert.False(StruckDamageCore.UndeadBonus(1, true, 80));
    }

    [Fact]
    public void UndeadBonusAddsBack()
    {
        // **是"加回去"以抵消神圣属性减伤**
        Assert.True(StruckDamageCore.UndeadBonusAddsBack());
        Assert.True(StruckDamageCore.UndeadCommentPresent());
        Assert.Equal(120, StruckDamageCore.ApplyUndeadBonus(100, 20));
    }

    [Fact]
    public void UndeadGateDiffersBetweenFunctions()
    {
        // **物理带 nType 门、魔法没有**
        Assert.True(StruckDamageCore.UndeadBonusHitHasTypeGate());
        Assert.True(StruckDamageCore.UndeadBonusMagHasNoTypeGate());
        Assert.True(StruckDamageCore.UndeadGateDiffersBetweenFunctions());
    }

    [Fact]
    public void StruckHpAssignedInMagOnly()
    {
        Assert.True(StruckDamageCore.StruckHpAssignedInMagOnly());
        Assert.True(StruckDamageCore.StruckHpIsAcAmount(5));
    }

    [Fact]
    public void ResultClamping()
    {
        Assert.True(StruckDamageCore.HitResultClamped());
        Assert.True(StruckDamageCore.MagEarlyExitClampedValues());
        Assert.True(StruckDamageCore.BothResultsEquivalent());
        Assert.Equal(0, StruckDamageCore.HitResult(-5));
        Assert.Equal(50, StruckDamageCore.HitResult(50));
    }
}
