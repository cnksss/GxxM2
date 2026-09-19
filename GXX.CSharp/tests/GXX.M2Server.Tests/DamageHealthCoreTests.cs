using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J147：`DamageHealth`（13973-14220）、`MakePosion`（40222-40299）、
/// `MakeFrozen`（40301-40331）、`OpenCobwebWinding`（2438）、三个盾回调、
/// `GetStruckProtectHP`（43438）1:1 测试。
/// </summary>
public sealed class DamageHealthCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(DamageHealthCore.ConstantsMatchSource());
        Assert.True(DamageHealthCore.MessageIdsMatchSource());
        Assert.True(DamageHealthCore.StateIndexesDistinct());
        Assert.True(DamageHealthCore.HighValues());
    }

    [Fact]
    public void ConstantValues()
    {
        Assert.Equal(18, DamageHealthCore.MaxStatusAttr);
        Assert.Equal(0, DamageHealthCore.PoisonDecHealth);
        Assert.Equal(1, DamageHealthCore.PoisonDamageArmor);
        Assert.Equal(5, DamageHealthCore.PoisonStone);
        Assert.Equal(11, DamageHealthCore.StateBubbleDefenceUp);
        Assert.Equal(12, DamageHealthCore.StateFrozen);
        Assert.Equal(13, DamageHealthCore.StateNewHitBubbleDefenceUp);
        Assert.Equal(14, DamageHealthCore.StateNewMagBubbleDefenceUp);
        Assert.Equal(150, DamageHealthCore.MonsterShieldHardcoded);
        Assert.Equal(11, DamageHealthCore.AntiParalysisIndex);
    }

    [Fact]
    public void MessageIds()
    {
        Assert.Equal(20259, DamageHealthCore.RmHealthSpellChangedStruck);
        Assert.Equal(20258, DamageHealthCore.RmMagicShieldStruck);
        Assert.Equal(20201, DamageHealthCore.RmStopContinuousMagic);
        Assert.Equal(20406, DamageHealthCore.RmPoisonStruckHum);
        Assert.Equal(20196, DamageHealthCore.RmOpenCobwebWinding);
        Assert.Equal(20190, DamageHealthCore.RmRefAbilNg);
    }

    [Fact]
    public void StaleComment076()
    {
        // **三个盾回调注释里的 0x76 与真实常量都不符**
        Assert.True(DamageHealthCore.StaleComment076());
        Assert.Equal(118, DamageHealthCore.StaleCommentIndex);
        Assert.NotEqual(DamageHealthCore.StaleCommentIndex, DamageHealthCore.StateBubbleDefenceUp);
    }

    [Fact]
    public void RoundHalfUpValues()
    {
        Assert.True(DamageHealthCore.RoundHalfUpValues());
    }

    // ===================== 一、魔法盾初始化 =====================

    [Fact]
    public void MagicShieldInitPerRace()
    {
        Assert.True(DamageHealthCore.MagicShieldInitPerRace());
        Assert.True(DamageHealthCore.MonsterHasNoShield());
        Assert.True(DamageHealthCore.Hardcoded150Unreachable());
    }

    [Fact]
    public void InitShieldValues()
    {
        var player = DamageHealthCore.InitShield(0, true, 0, 30, 20);
        var monster = DamageHealthCore.InitShield(80, false, 0, 0, 0);

        Assert.True(player.HasShield);
        Assert.Equal(30, player.ShieldRate);
        Assert.Equal(20, player.AbsorbRate);
        Assert.False(monster.HasShield);
        Assert.Equal(150, monster.ShieldRate);
        Assert.Equal(0, monster.AbsorbRate);
    }

    [Fact]
    public void FluteStonePassive()
    {
        Assert.True(DamageHealthCore.FluteStoneTruthTable());
        Assert.True(DamageHealthCore.FluteStonePassive(0, 10));
        Assert.False(DamageHealthCore.FluteStonePassive(10, 10));
    }

    [Fact]
    public void UnMagicShieldTwoSources()
    {
        Assert.True(DamageHealthCore.UnMagicShieldTwoSources());
        Assert.True(DamageHealthCore.UnMagicShield(true, false, false));
        Assert.True(DamageHealthCore.UnMagicShield(false, true, true));
        Assert.False(DamageHealthCore.UnMagicShield(false, false, false));
    }

    [Fact]
    public void AbsorbScaling()
    {
        Assert.True(DamageHealthCore.AbsorbScalesThenMayExit());
        Assert.True(DamageHealthCore.AbsorbZeroExits());
        Assert.True(DamageHealthCore.AbsorbRateNotClamped());
        Assert.True(DamageHealthCore.AbsorbClampedToHigh());

        Assert.Equal(70, DamageHealthCore.ApplyAbsorb(100, 30));
        Assert.Equal(0, DamageHealthCore.ApplyAbsorb(100, 100));
        Assert.Equal(0, DamageHealthCore.ApplyAbsorb(100, 150));
    }

    // ===================== 二、两段镜像脚本钩子 =====================

    [Fact]
    public void TwoMirroredScriptHooks()
    {
        Assert.True(DamageHealthCore.TwoMirroredScriptHooks());
        Assert.True(DamageHealthCore.FourBranchesEach());
        Assert.True(DamageHealthCore.HooksNeedNpcAndAttackerTruthTable());
    }

    [Fact]
    public void FirstAsksAttackerIdentity()
    {
        Assert.True(DamageHealthCore.FirstAsksAttackerIdentity());
        Assert.Equal("@AttackDamage", DamageHealthCore.AttackerLabel(0, false, 0, false));
        Assert.Equal("@HeroAttackDamage", DamageHealthCore.AttackerLabel(1, true, 0, false));
        Assert.Equal("@GamePetAttackDamage", DamageHealthCore.AttackerLabel(80, true, 0, true));
        Assert.Equal("@SlaveAttackDamage", DamageHealthCore.AttackerLabel(80, true, 0, false));
    }

    [Fact]
    public void SecondAsksSelfIdentity()
    {
        Assert.True(DamageHealthCore.SecondAsksSelfIdentity());
        Assert.Equal("@StruckDamage", DamageHealthCore.SelfLabel(0, false, 0, false));
        Assert.Equal("@SlaveStruckDamage", DamageHealthCore.SelfLabel(80, true, 0, false));
    }

    [Fact]
    public void GamePetBeatsSlave()
    {
        Assert.True(DamageHealthCore.GamePetBeatsSlave());
        Assert.True(DamageHealthCore.HeroBranchNeedsPlayerMaster());
    }

    [Fact]
    public void DamageRebookedThroughField()
    {
        // **脚本可以改伤害**
        Assert.True(DamageHealthCore.DamageRebookedThroughField());
        Assert.Equal(50, DamageHealthCore.RoundTripDamage(100, 50, true));
        Assert.Equal(100, DamageHealthCore.RoundTripDamage(100, 50, false));
    }

    [Fact]
    public void SlaveBranchUsesMasterField()
    {
        Assert.True(DamageHealthCore.SlaveBranchUsesMasterFieldValues());
        Assert.True(DamageHealthCore.SlaveBranchUsesMasterField("@SlaveAttackDamage"));
        Assert.False(DamageHealthCore.SlaveBranchUsesMasterField("@AttackDamage"));
    }

    [Fact]
    public void ScriptLabels()
    {
        Assert.True(DamageHealthCore.TenScriptLabels());
        Assert.True(DamageHealthCore.ScriptLabelsDistinct());
        Assert.Equal(10, DamageHealthCore.ScriptLabels.Length);
    }

    // ===================== 三、PK 标记与掉蓝 =====================

    [Fact]
    public void PkFlagSevenGates()
    {
        Assert.True(DamageHealthCore.PkFlagSevenGates());
        Assert.True(DamageHealthCore.PkGoesToMasterValues());
    }

    [Fact]
    public void DrainMpFiveGates()
    {
        Assert.True(DamageHealthCore.DrainMpFiveGates());
    }

    [Fact]
    public void SpDamageValues()
    {
        Assert.True(DamageHealthCore.SpDamageValues());
        Assert.Equal(30, DamageHealthCore.SpDamage(100, 30));
        Assert.Equal(150, DamageHealthCore.SpDamage(100, 150));
    }

    [Fact]
    public void NoPkWhenSelfStruck()
    {
        Assert.True(DamageHealthCore.NoPkWhenSelfStruckValues());
        Assert.False(DamageHealthCore.NoPkWhenSelfStruck(true));
        Assert.True(DamageHealthCore.NoPkWhenSelfStruck(false));
    }

    [Fact]
    public void MpShortfallTruncates()
    {
        Assert.True(DamageHealthCore.MpShortfallTruncates());
        Assert.True(DamageHealthCore.ExactMpCountsAsDec());
        Assert.True(DamageHealthCore.ZeroMpNotDec());

        var short1 = DamageHealthCore.DrainMp(20, 30);

        Assert.Equal(0, short1.Mp);
        Assert.Equal(20, short1.SpDam);
    }

    [Fact]
    public void FullAbsorbZeroesDamage()
    {
        Assert.True(DamageHealthCore.FullAbsorbZeroesDamage());
        Assert.Equal(0, DamageHealthCore.DamageAfterShield(100, 100));
        Assert.Equal(70, DamageHealthCore.DamageAfterShield(100, 30));
    }

    [Fact]
    public void MpMessages()
    {
        Assert.True(DamageHealthCore.TwoMpMessages());
        Assert.True(DamageHealthCore.ShieldStruckNeedsThreeConditions());
        Assert.True(DamageHealthCore.HealthSpellChangedArgsValues());
        Assert.True(DamageHealthCore.ObjectPointerAsParam());
        Assert.True(DamageHealthCore.ShieldStruckDelayIs200());
    }

    // ===================== 四、扣血与吸血 =====================

    [Fact]
    public void DeductTwoPaths()
    {
        Assert.True(DamageHealthCore.DeductTwoPaths());
        Assert.True(DamageHealthCore.OverkillUsesRemainingHp());

        var overkill = DamageHealthCore.DeductHealth(100, 150);

        Assert.Equal(100, overkill.Result);
        Assert.Equal(0, overkill.NewHp);
    }

    [Fact]
    public void OverkillAttackPowerBug()
    {
        // **源码先置零血量再加，故实际加 0（原意应是加被打掉的量）**
        Assert.True(DamageHealthCore.OverkillAttackPowerBug());
        Assert.Equal(0, DamageHealthCore.DeductHealth(100, 150).AttackPowerAdd);
        Assert.Equal(30, DamageHealthCore.DeductHealth(100, 30).AttackPowerAdd);
    }

    [Fact]
    public void NegativeDamageHeals()
    {
        Assert.True(DamageHealthCore.NegativeDamageHeals());
        Assert.True(DamageHealthCore.HealOverflowReturned());

        Assert.Equal(-30, DamageHealthCore.HealResult(50, 100, -30));
        Assert.Equal(-10, DamageHealthCore.HealResult(90, 100, -30));
    }

    [Fact]
    public void AbsorbSections()
    {
        Assert.True(DamageHealthCore.TwoAbsorbSections());
        Assert.True(DamageHealthCore.AbsorbFiveGates());
        Assert.True(DamageHealthCore.AbsorbUsesAttackPower());
        Assert.True(DamageHealthCore.AbsorbClampedToMax());
        Assert.True(DamageHealthCore.AbsorbOverwritesDamageVar());
        Assert.Equal(2, DamageHealthCore.AbsorbSections.Length);
    }

    [Fact]
    public void AbsorbValues()
    {
        Assert.Equal(100, DamageHealthCore.AbsorbAmount(200, 50));
        Assert.Equal(150, DamageHealthCore.AbsorbClamp(50, 100, 200));
        Assert.True(DamageHealthCore.AbsorbNeedsPositiveAmount(1));
        Assert.False(DamageHealthCore.AbsorbNeedsPositiveAmount(0));
    }

    // ===================== 五、MakePosion =====================

    [Fact]
    public void AntiFullParalysisFourGates()
    {
        Assert.True(DamageHealthCore.AntiFullParalysisFourGates());
        Assert.True(DamageHealthCore.ChangeModeExValueEleven());
    }

    [Fact]
    public void TypeRangeSilentFail()
    {
        // **类型越界静默失败**
        Assert.True(DamageHealthCore.TypeRangeSilentFail());
        Assert.True(DamageHealthCore.TypeInRange(17));
        Assert.False(DamageHealthCore.TypeInRange(18));
    }

    [Fact]
    public void ClampTimeValues()
    {
        Assert.True(DamageHealthCore.ClampTimeValues());
        Assert.Equal(0, DamageHealthCore.ClampTime(-5));
    }

    [Fact]
    public void OldTimeOnlyIncreases()
    {
        // **`CheckOldTime` 为真时只增不减**
        Assert.True(DamageHealthCore.OldTimeOnlyIncreases());
        Assert.True(DamageHealthCore.UncheckedOverwrites());
        Assert.True(DamageHealthCore.TickAlwaysWritten());

        Assert.Equal(10, DamageHealthCore.MergeOldTime(10, 5, true));
        Assert.Equal(5, DamageHealthCore.MergeOldTime(10, 5, false));
    }

    [Fact]
    public void GreenPoisonClampMinusOne()
    {
        // **绿毒要减 1（因自动减血是 +1）**
        Assert.True(DamageHealthCore.GreenPoisonClampMinusOne());
        Assert.True(DamageHealthCore.GreenPoisonAppliesCapThenMinusOne());
        Assert.True(DamageHealthCore.GreenPoisonCommentPresent());

        Assert.Equal(99, DamageHealthCore.GreenPoisonPoint(100, v => v));
        Assert.Equal(199, DamageHealthCore.GreenPoisonPoint(500, _ => 200));
    }

    [Fact]
    public void StatusChangedOnlyOnDiff()
    {
        Assert.True(DamageHealthCore.StatusChangedOnlyOnDiffValues());
        Assert.False(DamageHealthCore.StatusChangedOnlyOnDiff(5, 5));
    }

    [Fact]
    public void PoisonHintOnlyThreeTypes()
    {
        Assert.True(DamageHealthCore.PoisonHintOnlyThreeTypes());
        Assert.True(DamageHealthCore.PoisonHintFormatTwoValues());
        Assert.False(DamageHealthCore.PoisonHint(2, true, 0, 5));
    }

    [Fact]
    public void HeroPosionTodo()
    {
        // **英雄中毒提示至今未实现**
        Assert.True(DamageHealthCore.HeroPosionTodoPresent());
        Assert.Contains("添加英雄中毒提示", DamageHealthCore.HeroPosionTodo);
    }

    [Fact]
    public void ParalysisBreaksContinuous()
    {
        Assert.True(DamageHealthCore.ParalysisBreaksContinuousValues());
        Assert.True(DamageHealthCore.ParalysisBreaksContinuous(5, 0, true));
    }

    [Fact]
    public void AutoOnlineGreenPoison()
    {
        Assert.True(DamageHealthCore.AutoOnlineGreenPoisonTruthTable());
        Assert.True(DamageHealthCore.AutoOnlineGreenPoison(0, true, 0, true, 80, true, 0));
        Assert.False(DamageHealthCore.AutoOnlineGreenPoison(1, true, 0, true, 0, false, 0));
    }

    [Fact]
    public void PosionComments()
    {
        Assert.True(DamageHealthCore.FourPosionComments());
        Assert.Equal(4, DamageHealthCore.PosionComments.Length);
    }

    // ===================== 六、MakeFrozen 与盾回调 =====================

    [Fact]
    public void FrozenDifferences()
    {
        // **冰冻缺"防全麻"与类型范围门，且恒返回真**
        Assert.True(DamageHealthCore.FrozenLacksTwoGates());
        Assert.True(DamageHealthCore.FrozenAlwaysTrue());
        Assert.True(DamageHealthCore.FrozenHintFormat());
    }

    [Fact]
    public void FrozenHintGateValues()
    {
        Assert.True(DamageHealthCore.FrozenHintGate(0, 5));
        Assert.True(DamageHealthCore.FrozenHintGate(1, 5));
        Assert.False(DamageHealthCore.FrozenHintGate(80, 5));
        Assert.False(DamageHealthCore.FrozenHintGate(0, 0));
    }

    [Fact]
    public void ThreeShieldCallbacks()
    {
        // **三者结构完全一致、都忽略入参**
        Assert.True(DamageHealthCore.ThreeShieldCallbacksIdentical());
        Assert.True(DamageHealthCore.IgnoreParameter());
    }

    [Fact]
    public void ShieldDecrement()
    {
        Assert.True(DamageHealthCore.DecrementByThree());
        Assert.True(DamageHealthCore.FloorAtOne());
        Assert.True(DamageHealthCore.ZeroStaysZero());

        Assert.Equal(7, DamageHealthCore.ShieldDec(10));
        Assert.Equal(1, DamageHealthCore.ShieldDec(3));
        Assert.Equal(0, DamageHealthCore.ShieldDec(0));
    }

    // ===================== 七、GetStruckProtectHP =====================

    [Fact]
    public void TwoEarlyExits()
    {
        Assert.True(DamageHealthCore.TwoEarlyExits());
        Assert.True(DamageHealthCore.OnlyLethalProtected());
    }

    [Fact]
    public void PercentageMode()
    {
        Assert.True(DamageHealthCore.PercentageModeTwoValues());
        Assert.True(DamageHealthCore.PercentageCheckUsesIntegerDivision());
        Assert.True(DamageHealthCore.AbsoluteModeTwoValues());

        var (check, protect) = DamageHealthCore.PercentageValues(1000, 30, 20);

        Assert.Equal(300, check);
        Assert.Equal(200, protect);
    }

    [Fact]
    public void ProtectOutcome()
    {
        // **两种模式收尾相同；保护量不小于血量时免死**
        Assert.True(DamageHealthCore.ProtectOutcomeTwoCases());
        Assert.True(DamageHealthCore.ProtectNeverPunchesThrough());
        Assert.True(DamageHealthCore.SecondMinCapsAtHpMinusProtect());

        Assert.Equal(0, DamageHealthCore.ProtectOutcome(100, 100, 100));
        Assert.Equal(50, DamageHealthCore.ProtectOutcome(100, 150, 50));
        Assert.Equal(70, DamageHealthCore.ProtectOutcome(1000, 100, 30));
    }

    [Fact]
    public void ProtectScriptLabel()
    {
        Assert.True(DamageHealthCore.ScriptLabelTwoWayDuplicate());
        Assert.True(DamageHealthCore.LabelWrittenTwice());
        Assert.Equal("@HeroProtectHP", DamageHealthCore.ProtectLabel(1, true, 0));
        Assert.Equal("@ProtectHP", DamageHealthCore.ProtectLabel(0, false, 0));
    }

    // ===================== 八、OpenCobwebWinding =====================

    [Fact]
    public void CobwebGates()
    {
        Assert.True(DamageHealthCore.CobwebPositiveOpensValues());
        Assert.True(DamageHealthCore.CobwebNonPositiveCloses());
        Assert.True(DamageHealthCore.CobwebPointerParam());
        Assert.True(DamageHealthCore.CobwebHintTwoRaces());
    }

    [Fact]
    public void CobwebTickIsMilliseconds()
    {
        Assert.True(DamageHealthCore.CobwebTickIsMilliseconds());
        Assert.Equal(6000, DamageHealthCore.CobwebTick(1000, 5));
    }

    // ===================== 九、内功 =====================

    [Fact]
    public void NgDecPower()
    {
        Assert.True(DamageHealthCore.RefAbilNgGate(true));
        Assert.False(DamageHealthCore.RefAbilNgGate(false));
        Assert.True(DamageHealthCore.NgDecPowerUsesIntegerDivision());
        Assert.True(DamageHealthCore.NgDecPowerClampsNegative());
        Assert.True(DamageHealthCore.NgAddAndDecSameShape());

        Assert.Equal(15, DamageHealthCore.NgDecPower(10, 3, 5, 0));
        Assert.Equal(0, DamageHealthCore.NgDecPower(2, 3, 5, 0));
    }
}
