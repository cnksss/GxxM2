using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J152：`EnterAnotherMap`（ObjBase.pas 33189-33381，193 行）与
/// `DeleteFromMap`（Envir.pas 1772-1931）1:1 测试。
/// **检查码序列、删除压缩行为与所有门限均由临时探针实测后写入。**
/// </summary>
public sealed class EnterAnotherMapCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(EnterAnotherMapCore.ConstantsMatchSource());
        Assert.True(EnterAnotherMapCore.MessageIdsMatchSource());
        Assert.True(EnterAnotherMapCore.SecretFlagsAreDistinctBits());
    }

    [Fact]
    public void MessageIds()
    {
        Assert.Equal(20053, EnterAnotherMapCore.RmUserName);
        Assert.Equal(20088, EnterAnotherMapCore.RmClearObjects);
        Assert.Equal(20089, EnterAnotherMapCore.RmChangeMap);
        Assert.Equal(656, EnterAnotherMapCore.SmChangeNameColor);
        Assert.Equal(8900, EnterAnotherMapCore.SmFbTime);
        Assert.Equal(50, EnterAnotherMapCore.RcAnimal);
    }

    [Fact]
    public void SecretFlags()
    {
        // **两个掩码位互不重叠**
        Assert.Equal(2, EnterAnotherMapCore.SecretFlagNoChangNameColor);
        Assert.Equal(8, EnterAnotherMapCore.SecretFlagShowEqualName);
        Assert.True(EnterAnotherMapCore.SecretFlagsAreDistinctBits());
    }

    // ===================== 一、nCode 序列 =====================

    [Fact]
    public void CheckCodeSequence()
    {
        Assert.True(EnterAnotherMapCore.ThirtyCheckCodes());
        Assert.True(EnterAnotherMapCore.EnterCodeSequenceLength());
        Assert.Equal(30, EnterAnotherMapCore.EnterCodeAssignments.Length);
    }

    [Fact]
    public void SequenceHasDuplicateAndGap()
    {
        // **`28` 出现两次、`29` 从未出现**
        Assert.True(EnterAnotherMapCore.TwentyEightTwice());
        Assert.True(EnterAnotherMapCore.TwentyNineSkipped());
        Assert.True(EnterAnotherMapCore.SequenceHasBothDefects());
        Assert.True(EnterAnotherMapCore.EnterCodeStartsAtZero());
    }

    [Fact]
    public void CheckCodeValues()
    {
        int[] c = EnterAnotherMapCore.EnterCodeAssignments;

        Assert.Equal(1, c[0]);
        Assert.Equal(28, c[27]);
        Assert.Equal(28, c[28]);
        Assert.Equal(30, c[29]);
    }

    [Fact]
    public void ExceptionFormat()
    {
        // **分号 + `Code = ` 形式、手工拼接、只输出一条**
        Assert.True(EnterAnotherMapCore.ExceptionIsConcatStyle());
        Assert.True(EnterAnotherMapCore.ExceptionEmitsOnlyOne());
        Assert.True(EnterAnotherMapCore.FormatEnterExceptionValues());

        Assert.Equal("[Exception] TBaseObject.EnterAnotherMap; Code = 12",
            EnterAnotherMapCore.FormatEnterException(12));
    }

    // ===================== 二、十道门 =====================

    [Fact]
    public void OnlyOneTruePath()
    {
        Assert.True(EnterAnotherMapCore.OnlyOneTruePathValues());
        Assert.True(EnterAnotherMapCore.OnlyOneTruePath(true));
        Assert.False(EnterAnotherMapCore.OnlyOneTruePath(false));
    }

    [Fact]
    public void QuestNpcIsSideEffectNotGate()
    {
        // **条件成立就点、不成立就跳过，两者都继续 —— 不是门**
        Assert.True(EnterAnotherMapCore.QuestNpcSideEffectValues());
        Assert.True(EnterAnotherMapCore.QuestNpcClickTruthTable());
    }

    [Fact]
    public void NeedSetOnGate()
    {
        Assert.True(EnterAnotherMapCore.NeedSetOnGateTruthTable());
    }

    [Fact]
    public void CellInfoAndCastleGates()
    {
        Assert.True(EnterAnotherMapCore.CellInfoFailureExitsValues());
        Assert.True(EnterAnotherMapCore.CastleGateTruthTable());

        // **城堡校验用的是旧坐标**
        Assert.True(EnterAnotherMapCore.CastleUsesOldCoords());
    }

    [Fact]
    public void HorseCarryGate()
    {
        Assert.True(EnterAnotherMapCore.HorseCarryIsRecursive());
        Assert.True(EnterAnotherMapCore.HorseCarryGateTruthTable());
    }

    [Fact]
    public void HeroTargetClear()
    {
        Assert.True(EnterAnotherMapCore.HeroTargetClearTruthTable());
    }

    [Fact]
    public void Rollback()
    {
        Assert.True(EnterAnotherMapCore.RollbackOnAddToMapFailure());
        Assert.True(EnterAnotherMapCore.FailureBranchHasCommentedLegacy());
        Assert.True(EnterAnotherMapCore.CommentedLegacyCount());

        Assert.Equal(("0", 5, 5),
            EnterAnotherMapCore.RollbackOnFailure(false, "1", 10, 20, "0", 5, 5));
    }

    // ===================== 三、AddToMap 成功支 =====================

    [Fact]
    public void TempAdminParensAsymmetry()
    {
        // **同一函数内两行之隔、一处少括号一处不少（第四次）**
        Assert.True(EnterAnotherMapCore.TempAdminMissingParens());
        Assert.True(EnterAnotherMapCore.SiblingHasParens());
        Assert.True(EnterAnotherMapCore.TempAdminParensAsymmetry());
    }

    [Fact]
    public void TruckSlaveConditions()
    {
        Assert.True(EnterAnotherMapCore.TruckSlaveFourConditions());
        Assert.True(EnterAnotherMapCore.ViewRangeInclusive());
        Assert.True(EnterAnotherMapCore.TruckUsesOldCoords());

        // **视距是闭区间：差 12 通过、差 13 不通过**
        Assert.True(EnterAnotherMapCore.TruckSlaveCondition(128, true, 10, 10, 22, 10, 12));
        Assert.False(EnterAnotherMapCore.TruckSlaveCondition(128, true, 10, 10, 23, 10, 12));
    }

    [Fact]
    public void SelfTruckClearsFlag()
    {
        Assert.True(EnterAnotherMapCore.SelfTruckClearsFlagValues());
        Assert.False(EnterAnotherMapCore.SelfTruckClearsFlag(128, true));
        Assert.True(EnterAnotherMapCore.SelfTruckClearsFlag(80, true));
    }

    // ===================== 四、收尾分支 =====================

    [Fact]
    public void FiveTicksReset()
    {
        Assert.True(EnterAnotherMapCore.FiveTicksReset());
        Assert.Equal(5, EnterAnotherMapCore.ResetTicks.Length);
    }

    [Fact]
    public void SayAdvertiseParensAsymmetry()
    {
        // **本批次第二次"同块内一处少括号"（第五次）**
        Assert.True(EnterAnotherMapCore.SayAdvertiseMissingParens());
        Assert.True(EnterAnotherMapCore.SayAdvertiseParensAsymmetry());
    }

    [Fact]
    public void CombatPowerSwitches()
    {
        Assert.True(EnterAnotherMapCore.CombatPowerNeedsBothSwitchesTruthTable());
        Assert.True(EnterAnotherMapCore.CombatPowerNeedsBothSwitches(true, true));
        Assert.False(EnterAnotherMapCore.CombatPowerNeedsBothSwitches(true, false));
    }

    [Fact]
    public void TimeMapGate()
    {
        Assert.True(EnterAnotherMapCore.TimeMapUsesOldMap());
        Assert.True(EnterAnotherMapCore.TimeMapGateTruthTable());
        Assert.True(EnterAnotherMapCore.TimeMapBoundaryAtOneMinute());

        // **边界恰在 60000 毫秒**
        Assert.True(EnterAnotherMapCore.TimeMapGate(60000, 0, 1, "@x"));
        Assert.False(EnterAnotherMapCore.TimeMapGate(59999, 0, 1, "@x"));
    }

    [Fact]
    public void FbTimeUsesNewMap()
    {
        // **同一段里两处取的地图不同：限时用旧地图、FB 时间用新地图**
        Assert.True(EnterAnotherMapCore.FbTimeUsesNewMap());
        Assert.True(EnterAnotherMapCore.TimeMapSourcesDiffer());
        Assert.True(EnterAnotherMapCore.FbTimeParamFourAndFive());
        Assert.True(EnterAnotherMapCore.FbTimeArgsValues());
        Assert.True(EnterAnotherMapCore.FbTimeCommentedGate());
        Assert.True(EnterAnotherMapCore.FbTimeIsUnconditional());

        Assert.Equal(600000, EnterAnotherMapCore.FbTimeValue(10));
    }

    [Fact]
    public void HeroBranch()
    {
        Assert.True(EnterAnotherMapCore.HeroBranchUsesMaster());
        Assert.True(EnterAnotherMapCore.HeroBranchGateTruthTable());
        Assert.True(EnterAnotherMapCore.TwoGotoLabels());
    }

    [Fact]
    public void Fight3ZoneAsymmetry()
    {
        // **只能由"假 → 真"触发；离开战争地图不刷新**
        Assert.True(EnterAnotherMapCore.Fight3ZoneCrossingOnly());
        Assert.True(EnterAnotherMapCore.LeavingDoesNotRefresh());

        Assert.True(EnterAnotherMapCore.Fight3ZoneGate(true, false));
        Assert.False(EnterAnotherMapCore.Fight3ZoneGate(false, true));
    }

    // ===================== 五、秘密标志 =====================

    [Fact]
    public void SecretFlagComparison()
    {
        Assert.True(EnterAnotherMapCore.SecretFlagMasked());
        Assert.True(EnterAnotherMapCore.SecondFlagFieldMatters());
        Assert.True(EnterAnotherMapCore.TwoMasksIndependent());
        Assert.True(EnterAnotherMapCore.SecretFlagMessageIds());
        Assert.True(EnterAnotherMapCore.ChangeNameColorCarriesColor());
    }

    [Fact]
    public void SecretFlagBitValues()
    {
        // 只有 showEqual 位变化时，showEqual 掩码命中、noColor 掩码不命中
        Assert.True(EnterAnotherMapCore.SecretFlagChanged(0, 8, 0, 0, 8));
        Assert.False(EnterAnotherMapCore.SecretFlagChanged(0, 8, 0, 0, 2));
    }

    // ===================== 六、DeleteFromMap =====================

    [Fact]
    public void DeleteCodes()
    {
        Assert.True(EnterAnotherMapCore.TwentyFourCodes());
        Assert.True(EnterAnotherMapCore.DeleteCodeCountValues());
        Assert.Equal(25, EnterAnotherMapCore.DeleteCodeCount());
    }

    [Fact]
    public void DeleteExceptionFormat()
    {
        // **三个具名字段**
        Assert.True(EnterAnotherMapCore.ExceptionFieldsThree());
        Assert.True(EnterAnotherMapCore.FormatDeleteExceptionValues());
        Assert.True(EnterAnotherMapCore.NameCachePurpose());

        Assert.Equal("[Exception] TEnvirnoment.DeleteFromMap, Code: 5; MapName: 0; MapDesc: 比奇省",
            EnterAnotherMapCore.FormatDeleteException(5, "0", "比奇省"));
    }

    [Fact]
    public void InvalidMapAndNesting()
    {
        Assert.True(EnterAnotherMapCore.InvalidMapExitsFalseValues());
        Assert.True(EnterAnotherMapCore.ThreeNestingLayers());
        Assert.True(EnterAnotherMapCore.ThreeLayerGateTruthTable());
    }

    [Fact]
    public void DeleteScanBehaviour()
    {
        Assert.True(EnterAnotherMapCore.SelfMatchSetsTrue());
        Assert.True(EnterAnotherMapCore.NotFoundReturnsFalse());
        Assert.True(EnterAnotherMapCore.DeleteLastEmptiesList());
        Assert.True(EnterAnotherMapCore.AllNullsEmptiesList());
    }

    [Fact]
    public void NullCompaction()
    {
        // **探针实测：`{0,0,5}` 删 `5` 得空表（索引不递增，目标落到 0 后立刻被删）**
        Assert.True(EnterAnotherMapCore.NullCompactionContinues());
        Assert.True(EnterAnotherMapCore.CompactionLandsOnTarget());
        Assert.True(EnterAnotherMapCore.TargetBeforeNullLeavesNull());

        var (found, rest) = EnterAnotherMapCore.DeleteScan(new List<int> { 0, 0, 5 }, 5);

        Assert.True(found);
        Assert.Empty(rest);
    }

    [Fact]
    public void BreakCommentRemnant()
    {
        // **源码把 `Continue` 改成了 `Break`，旧注释还留着**
        Assert.True(EnterAnotherMapCore.BreakCommentRemnant());
        Assert.True(EnterAnotherMapCore.BreakCommentRemnantPresent());
        Assert.Equal("Break; // Continue;", EnterAnotherMapCore.BreakCommentRemnantText);
    }

    [Fact]
    public void CounterBranches()
    {
        Assert.True(EnterAnotherMapCore.ThreeCounterBranches());
        Assert.True(EnterAnotherMapCore.PlayerBeatsSlaveBranch());
        Assert.True(EnterAnotherMapCore.SlaveBeatsMonsterBranch());
        Assert.True(EnterAnotherMapCore.CounterNeverNegative());

        // **计数不会被减成负数**
        Assert.Equal(0, EnterAnotherMapCore.DecrementCounter(0));
        Assert.Equal(4, EnterAnotherMapCore.DecrementCounter(5));
    }

    [Fact]
    public void BothZeroTickGate()
    {
        Assert.True(EnterAnotherMapCore.BothZeroSetsTickTruthTable());
        Assert.True(EnterAnotherMapCore.BothZeroSetsTick(0, 0));
        Assert.False(EnterAnotherMapCore.BothZeroSetsTick(1, 0));
        Assert.False(EnterAnotherMapCore.BothZeroSetsTick(0, 1));
    }

    [Fact]
    public void MonsterBranchHasNoTick()
    {
        Assert.True(EnterAnotherMapCore.MonsterBranchNoTick());
        Assert.True(EnterAnotherMapCore.ClearTickMissingParens());
        Assert.True(EnterAnotherMapCore.ClearTickParensAsymmetry());
    }

    [Fact]
    public void DeleteFlags()
    {
        // **门是"尚未删除"，块内恒置真、并把"已加入"恒置假**
        Assert.True(EnterAnotherMapCore.DeleteFlagsValues());
        Assert.True(EnterAnotherMapCore.AlreadyDeletedSkips());

        Assert.Equal((true, false), EnterAnotherMapCore.DeleteFlags(false));
        Assert.Equal((true, false), EnterAnotherMapCore.DeleteFlags(true));
    }

    [Fact]
    public void CounterOnlyForActor()
    {
        Assert.True(EnterAnotherMapCore.CounterOnlyForActorValues());
        Assert.True(EnterAnotherMapCore.CounterOnlyForActor(1));
        Assert.False(EnterAnotherMapCore.CounterOnlyForActor(2));
    }

    [Fact]
    public void NumericReturnRemnants()
    {
        // **三处注释残留说明"以前用的是数值返回码"**
        Assert.True(EnterAnotherMapCore.ThreeNumericReturnRemnants());
        Assert.Equal(3, EnterAnotherMapCore.DeleteCommentRemnants.Length);
    }
}
