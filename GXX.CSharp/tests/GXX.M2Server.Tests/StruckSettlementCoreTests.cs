using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J146：`StruckDamage`（39448-39865，419 行）1:1 测试。
/// </summary>
public sealed class StruckSettlementCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(StruckSettlementCore.ConstantsMatchSource());
        Assert.True(StruckSettlementCore.ConfigDefaults());
        Assert.True(StruckSettlementCore.TwoSwitchesDefaultOff());
        Assert.True(StruckSettlementCore.MessageIdsHaveLegacyComments());
        Assert.True(StruckSettlementCore.SlotRanges());
    }

    [Fact]
    public void ConstantValues()
    {
        Assert.Equal(0, StruckSettlementCore.UDress);
        Assert.Equal(1, StruckSettlementCore.UWeapon);
        Assert.Equal(1, StruckSettlementCore.PoisonDamageArmor);
        Assert.Equal(6, StruckSettlementCore.JewelryBoxCount);
        Assert.Equal(20064, StruckSettlementCore.RmAbility);
        Assert.Equal(20095, StruckSettlementCore.RmDuraChange);
        Assert.Equal(20103, StruckSettlementCore.RmSubAbility);
        Assert.Equal(9, StruckSettlementCore.LogItemDisappear);
        Assert.Equal(0, StruckSettlementCore.LogActionNone);
        Assert.Equal(1000, StruckSettlementCore.DuraDisplayUnit);
        Assert.Equal(8, StruckSettlementCore.DuraRandomGate);
    }

    [Fact]
    public void FunctionLineCount()
    {
        Assert.True(StruckSettlementCore.FunctionLineCount());
        Assert.Equal(419, StruckSettlementCore.FunctionLines());
    }

    [Fact]
    public void RoundHalfUpValues()
    {
        Assert.True(StruckSettlementCore.RoundHalfUpValues());
        Assert.True(StruckSettlementCore.HighIntegerValue());
    }

    // ===================== 一、四道伤害修正 =====================

    [Fact]
    public void FourDamageAdjustments()
    {
        Assert.True(StruckSettlementCore.FourDamageAdjustments());
        Assert.Equal(4, StruckSettlementCore.DamageAdjustments.Length);
    }

    [Fact]
    public void ClampNegativeValues()
    {
        Assert.True(StruckSettlementCore.ClampNegativeValues());
        Assert.Equal(0, StruckSettlementCore.ClampNegative(-5));
        Assert.Equal(7, StruckSettlementCore.ClampNegative(7));
    }

    [Fact]
    public void PosionDamagarmorIntegerDivision()
    {
        // **整数除法：12 → 1**
        Assert.True(StruckSettlementCore.PosionDamagarmorIntegerDivision());
        Assert.Equal(1, StruckSettlementCore.PosionDamageRate(12));
        Assert.Equal(2, StruckSettlementCore.PosionDamageRate(20));
        Assert.Equal(0, StruckSettlementCore.PosionDamageRate(9));
    }

    [Fact]
    public void PosionRateEdges()
    {
        // **默认 12 → 乘 1、等于没做；配到 20 才放大；配到 9 会清零**
        Assert.True(StruckSettlementCore.DefaultPosionRateIsNoOp());
        Assert.True(StruckSettlementCore.PosionRateNeedsTwenty());
        Assert.True(StruckSettlementCore.PosionRateBelowTenZeroes());
        Assert.True(StruckSettlementCore.PosionAppliesToBothValues());

        Assert.Equal(100, StruckSettlementCore.ApplyPosionDamage(100, 12));
        Assert.Equal(200, StruckSettlementCore.ApplyPosionDamage(100, 20));
        Assert.Equal(0, StruckSettlementCore.ApplyPosionDamage(100, 9));
    }

    [Fact]
    public void ScriptPosionUsesTypeZero()
    {
        // **与 J145 的 Type = 1 相反 —— 同字段相反取值**
        Assert.True(StruckSettlementCore.ScriptPosionUsesTypeZero());
        Assert.True(StruckSettlementCore.J145UsedTypeOne());
        Assert.True(StruckSettlementCore.ScriptPosionGate(1, 0));
        Assert.False(StruckSettlementCore.ScriptPosionGate(1, 1));
    }

    [Fact]
    public void ScriptPosionAdds()
    {
        Assert.True(StruckSettlementCore.ScriptPosionAdds());
        Assert.Equal(150, StruckSettlementCore.ApplyScriptPosion(100, 50));
    }

    [Fact]
    public void ThunderPalsyAppends()
    {
        Assert.True(StruckSettlementCore.ThunderPalsyAppends());
        Assert.Equal(125, StruckSettlementCore.ApplyThunderPalsy(100, 25));
        Assert.Equal(100, StruckSettlementCore.ApplyThunderPalsy(100, 0));
    }

    [Fact]
    public void DamRange()
    {
        // **nDam 范围 5..14**
        Assert.True(StruckSettlementCore.DamRange());
        Assert.Equal(5, StruckSettlementCore.DamValue(0));
        Assert.Equal(14, StruckSettlementCore.DamValue(9));
    }

    [Fact]
    public void DuraLossFormula()
    {
        Assert.True(StruckSettlementCore.DuraLossFormula());
        Assert.True(StruckSettlementCore.DuraLossRateIsFloating());
        Assert.Equal(5, StruckSettlementCore.DuraLoss(5, 100));
        Assert.Equal(14, StruckSettlementCore.DuraLoss(14, 100));
    }

    // ===================== 二、五段扣持久块 =====================

    [Fact]
    public void FiveDuraBlocks()
    {
        Assert.True(StruckSettlementCore.FiveDuraBlocks());
        Assert.Equal(5, StruckSettlementCore.DuraBlocks.Length);
    }

    [Fact]
    public void RoundThousandValues()
    {
        // **探针实测：Round(5000/1000)=5、Round(4990/1000)=5、Round(5999/1000)=6**
        Assert.Equal(5, StruckSettlementCore.RoundThousand(5000));
        Assert.Equal(5, StruckSettlementCore.RoundThousand(4990));
        Assert.Equal(6, StruckSettlementCore.RoundThousand(5999));
    }

    [Fact]
    public void BroadcastOnlyOnThousandBoundary()
    {
        // **相等即未跨千位 → 不广播**
        Assert.True(StruckSettlementCore.BroadcastOnlyOnThousandBoundary());
        Assert.True(StruckSettlementCore.NoBroadcastWithinSameThousand());
        Assert.True(StruckSettlementCore.BroadcastOnCrossing());
        Assert.True(StruckSettlementCore.OldDuraIsTruncatedDivision());
    }

    [Fact]
    public void BroadcastTruthTable()
    {
        // **边界在 x500（四舍五入），不在 x000**
        Assert.False(StruckSettlementCore.ShouldBroadcast(5, 4990));
        Assert.False(StruckSettlementCore.ShouldBroadcast(5, 5000));
        Assert.True(StruckSettlementCore.ShouldBroadcast(6, 5499));
        Assert.False(StruckSettlementCore.ShouldBroadcast(6, 5500));
        Assert.True(StruckSettlementCore.BoundaryIsAtHalfThousand());
    }

    [Fact]
    public void DuraLossChain()
    {
        Assert.True(StruckSettlementCore.DuraLossChain());
        Assert.True(StruckSettlementCore.DeleteGateValues());
        Assert.Equal(990, StruckSettlementCore.ApplyDuraLoss(1000, 10));
        Assert.Equal(-5, StruckSettlementCore.ApplyDuraLoss(5, 10));
    }

    [Fact]
    public void RandomEightGate()
    {
        // **每格只有 1/8 概率被扣**
        Assert.True(StruckSettlementCore.OnlyOneInEight());
        Assert.True(StruckSettlementCore.GateSitsAfterZeroCheck());
        Assert.False(StruckSettlementCore.RandomEightGate(0));
        Assert.True(StruckSettlementCore.RandomEightGate(1));
    }

    [Fact]
    public void SixEquipmentFilters()
    {
        Assert.True(StruckSettlementCore.SixEquipmentFilters());
        Assert.Equal(6, StruckSettlementCore.EquipmentFilters.Length);
    }

    [Fact]
    public void FilterListsDiffer()
    {
        // **装备格要求 Anicount > 0，首饰盒无条件跳过**
        Assert.True(StruckSettlementCore.FilterListsDiffer());
        Assert.True(StruckSettlementCore.JewelryLacksAnicountGate());
        Assert.True(StruckSettlementCore.JewelryMergesShapeZero());
        Assert.True(StruckSettlementCore.FilterCountsDiffer());
        Assert.Equal((6, 4), StruckSettlementCore.FilterCounts());
    }

    [Fact]
    public void EquipmentAnicountGate()
    {
        Assert.True(StruckSettlementCore.EquipmentAnicountGate(1));
        Assert.False(StruckSettlementCore.EquipmentAnicountGate(0));
    }

    [Fact]
    public void FeatureRefreshRules()
    {
        // **只有衣服与武器刷新外观；首饰盒不刷新**
        Assert.True(StruckSettlementCore.OnlyDressAndWeaponRefresh());
        Assert.True(StruckSettlementCore.FeatureChangedPerBlock());
        Assert.True(StruckSettlementCore.JewelryDoesNotRefreshFeature());
        Assert.True(StruckSettlementCore.DressBlocksAssignTrue());

        Assert.True(StruckSettlementCore.FeatureRefreshSlot(0));
        Assert.True(StruckSettlementCore.FeatureRefreshSlot(1));
        Assert.False(StruckSettlementCore.FeatureRefreshSlot(2));
    }

    [Fact]
    public void TodoMarker()
    {
        // **TODO 出现五次，说明"删除特定技能"从未实现**
        Assert.True(StruckSettlementCore.TodoMarkerFiveTimes());
        Assert.True(StruckSettlementCore.TodoTextHasDate());
        Assert.True(StruckSettlementCore.TodoDescribesMissingStep());
        Assert.True(StruckSettlementCore.CommentedWIndexZeroPresent());
        Assert.Equal(5, StruckSettlementCore.TodoCount());
    }

    [Fact]
    public void LogTexts()
    {
        Assert.True(StruckSettlementCore.TwoLogTexts());
        Assert.True(StruckSettlementCore.LogTextsDiffer());
        Assert.Equal("持久为0", StruckSettlementCore.DisappearLogTexts[0]);
        Assert.Equal("0持久消失", StruckSettlementCore.DisappearLogTexts[1]);
    }

    [Fact]
    public void ItemDamageLabel()
    {
        Assert.True(StruckSettlementCore.ItemDamageLabelFormat());
        Assert.True(StruckSettlementCore.ResetsScriptGotoCount());
        Assert.Equal("@ItemDamage0", StruckSettlementCore.ItemDamageLabel(0));
    }

    // ===================== 三、收尾 =====================

    [Fact]
    public void RecalcNotifiesOnlyHumans()
    {
        // **人形怪重算但不通知**
        Assert.True(StruckSettlementCore.RecalcNotifiesOnlyHumans());
        Assert.True(StruckSettlementCore.RecalcNotifyRace(0));
        Assert.False(StruckSettlementCore.RecalcNotifyRace(150));
    }

    [Fact]
    public void TwoRecalcMessages()
    {
        Assert.True(StruckSettlementCore.TwoRecalcMessages());
        Assert.Equal(new[] { 20064, 20103 }, StruckSettlementCore.RecalcMessages);
    }

    [Fact]
    public void FeatureChangedCalled()
    {
        Assert.True(StruckSettlementCore.FeatureChangedCalled());
    }

    [Fact]
    public void StruckProtectRaceGate()
    {
        // **秒杀保护只对玩家/英雄/人形怪**
        Assert.True(StruckSettlementCore.StruckProtectRaceGateValues());
        Assert.True(StruckSettlementCore.StruckProtectCommentHasDate());
        Assert.False(StruckSettlementCore.StruckProtectRaceGate(80));
    }

    [Fact]
    public void DamageHealthTwoPaths()
    {
        // **第三参镜像"是否有来源"**
        Assert.True(StruckSettlementCore.DamageHealthTwoPaths());
        Assert.True(StruckSettlementCore.ThirdParamMirrorsStruckFrom());
        Assert.True(StruckSettlementCore.ContextSavedAndRestored());
        Assert.True(StruckSettlementCore.ThreeSavedContextItems());
        Assert.True(StruckSettlementCore.MagicIdWrittenToBoth());
        Assert.True(StruckSettlementCore.CurrTargetSetToAttacker());
    }

    [Fact]
    public void MagicShieldDecMpSemantics()
    {
        // **有来源时是"MP 下降量"，无来源时恒 0**
        Assert.True(StruckSettlementCore.MagicShieldDecMpSemantics());
        Assert.True(StruckSettlementCore.NilSourceZeroesShieldMp());
        Assert.Equal(20, StruckSettlementCore.MagicShieldDecMp(100, 80));
        Assert.Equal(0, StruckSettlementCore.MagicShieldDecMp(100, 120));
    }

    [Fact]
    public void PkPowerRules()
    {
        // **PK 值 = 掉血 + 魔法盾消耗 MP**
        Assert.True(StruckSettlementCore.PkPowerIsResultPlusShieldMp());
        Assert.True(StruckSettlementCore.PkPowerDefaultTrue());
        Assert.True(StruckSettlementCore.PkPowerGuardTruthTable());
        Assert.Equal(120, StruckSettlementCore.PkPower(100, 20));
        Assert.False(StruckSettlementCore.PkPowerNeedsBothGuards(false, true));
    }

    [Fact]
    public void CloseSuperShiledFormula()
    {
        // **探针实测：(5,3,2,10) = 8**
        Assert.True(StruckSettlementCore.CloseSuperShiledFormulaUsesLevelSum());
        Assert.True(StruckSettlementCore.DefaultPerLevelRateIsNoOp());
        Assert.Equal(5, StruckSettlementCore.CloseSuperShiledTemp(5, 0, 0, 0));
        Assert.Equal(8, StruckSettlementCore.CloseSuperShiledTemp(5, 3, 2, 10));
        Assert.Equal(30, StruckSettlementCore.CloseSuperShiledTemp(5, 3, 2, 100));
    }

    [Fact]
    public void CloseSuperShiledBreakCheck()
    {
        Assert.True(StruckSettlementCore.RandomizeCallPresent());
        Assert.True(StruckSettlementCore.BreakCheckIsEqualZero());
        Assert.True(StruckSettlementCore.NonPositiveShortCircuits());
        Assert.True(StruckSettlementCore.NonPositiveIgnoresRoll());
        Assert.True(StruckSettlementCore.NonPositiveTempAlwaysBreaks(0));
        Assert.True(StruckSettlementCore.CloseSuperShiledGates());
    }
}
