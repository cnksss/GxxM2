using System;
using System.Collections.Generic;
using System.Linq;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J122：更改人物状态脚本命令 `CHANGESTATE`
/// （NpcActionCmd.pas 23598-23967 + 相邻首饰盒命令 23969-24000+）1:1 测试。
/// </summary>
public sealed class ChangeStateCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.Equal(329, ChangeStateCore.NaChangeState);
        Assert.Equal("CHANGESTATE", ChangeStateCore.CommandName);
        Assert.Equal(1, ChangeStateCore.MinStateType);
        Assert.Equal(14, ChangeStateCore.MaxStateType);
        Assert.Equal("ET_HOLYCURTAIN", ChangeStateCore.EtHolyCurtain);
    }

    [Fact]
    public void PoisonConstantsMatch()
    {
        Assert.Equal(0, ChangeStateCore.PoisonDechealth);
        Assert.Equal(1, ChangeStateCore.PoisonDamageArmor);
        Assert.Equal(2, ChangeStateCore.PoisonLockSpell);
        Assert.Equal(4, ChangeStateCore.PoisonDontMove);
        Assert.Equal(5, ChangeStateCore.PoisonStone);
    }

    [Fact]
    public void PoisonHasGapAtThree()
    {
        // **POISON_DONTMOVE = 4，编号 3 缺失**
        var known = new[]
        {
            ChangeStateCore.PoisonDechealth,
            ChangeStateCore.PoisonDamageArmor,
            ChangeStateCore.PoisonLockSpell,
            ChangeStateCore.PoisonDontMove,
            ChangeStateCore.PoisonStone,
        };

        Assert.DoesNotContain(3, known);
    }

    // ===================== 第一道校验 =====================

    [Fact]
    public void StateRangeIsOneToFourteen()
    {
        Assert.True(ChangeStateCore.IsValidStateType(1));
        Assert.True(ChangeStateCore.IsValidStateType(14));
        Assert.False(ChangeStateCore.IsValidStateType(0));
        Assert.False(ChangeStateCore.IsValidStateType(15));
        Assert.False(ChangeStateCore.IsValidStateType(-1));
    }

    [Fact]
    public void FirstCheckFailsOutsideRange()
    {
        Assert.True(ChangeStateCore.FirstCheckFails(0));
        Assert.True(ChangeStateCore.FirstCheckFails(15));
        Assert.False(ChangeStateCore.FirstCheckFails(1));
        Assert.False(ChangeStateCore.FirstCheckFails(14));
    }

    [Fact]
    public void FirstErrorUsesBaseObjectNotPlayObject()
    {
        Assert.True(ChangeStateCore.FirstErrorUsesBaseObjectNotPlayObject());
    }

    // ===================== 第二道校验 =====================

    [Fact]
    public void SecondCheckRejectsNegative()
    {
        Assert.True(ChangeStateCore.SecondCheckFails(-1, 0));
        Assert.True(ChangeStateCore.SecondCheckFails(0, -1));
        Assert.True(ChangeStateCore.SecondCheckFails(-1, -1));
        Assert.False(ChangeStateCore.SecondCheckFails(0, 0));
        Assert.False(ChangeStateCore.SecondCheckFails(5, 5));
    }

    [Fact]
    public void OmittedEffectParamsPass()
    {
        Assert.True(ChangeStateCore.OmittedEffectParamsPass(0));
        Assert.True(ChangeStateCore.OmittedEffectParamsPass(99));
    }

    [Fact]
    public void SecondCheckBlocksStatesThatIgnoreEffects()
    {
        Assert.True(ChangeStateCore.SecondCheckBlocksStatesThatIgnoreEffects());

        // 状态 13 不用特效，但 nParam5 = -1 时整条命令被拒
        Assert.True(ChangeStateCore.IsValidStateType(13));
        Assert.True(ChangeStateCore.SecondCheckFails(-1, 0));
    }

    [Fact]
    public void StatesIgnoringEffectsListed()
    {
        Assert.Equal(new[] { 6, 7, 13, 14 }, ChangeStateCore.StatesIgnoringEffects);
    }

    // ===================== 自定义特效 =====================

    [Fact]
    public void EffectGateFourConditions()
    {
        Assert.True(ChangeStateCore.ShouldSendCustomEffect(0, 0, 1, 1));

        Assert.False(ChangeStateCore.ShouldSendCustomEffect(-1, 0, 1, 1));  // file < 0
        Assert.False(ChangeStateCore.ShouldSendCustomEffect(0, -1, 1, 1));  // index < 0
        Assert.False(ChangeStateCore.ShouldSendCustomEffect(0, 0, 0, 1));   // count = 0
        Assert.False(ChangeStateCore.ShouldSendCustomEffect(0, 0, 1, 0));   // playtime = 0
    }

    [Fact]
    public void EffectGateDiffersFromValidation()
    {
        // 校验用 < 0（0 通过），门要求 > 0 —— 方向不一致
        Assert.True(ChangeStateCore.EffectGateDiffersFromValidation());
    }

    [Fact]
    public void BlendDrawParsing()
    {
        Assert.True(ChangeStateCore.ParseBlendDraw(1));
        Assert.False(ChangeStateCore.ParseBlendDraw(0));
        Assert.False(ChangeStateCore.ParseBlendDraw(2));   // **等值判断，非 0 即真**
    }

    [Fact]
    public void BlendDrawEncodedAsString()
    {
        Assert.True(ChangeStateCore.BlendDrawEncodedAsString());
        Assert.Equal("1", ChangeStateCore.BoolToIntStr(true));
        Assert.Equal("0", ChangeStateCore.BoolToIntStr(false));
    }

    // ===================== 状态 1..5 镜像结构 =====================

    [Fact]
    public void ImmunityFlagsMap()
    {
        Assert.Equal("UnParalysis", ChangeStateCore.ImmunityFlagFor(1));
        Assert.Equal("UnFrozen", ChangeStateCore.ImmunityFlagFor(2));
        Assert.Equal("UnCobwebWinding", ChangeStateCore.ImmunityFlagFor(3));
        Assert.Equal("UnPosion", ChangeStateCore.ImmunityFlagFor(4));
        Assert.Equal("UnPosion", ChangeStateCore.ImmunityFlagFor(5));
        Assert.Equal("", ChangeStateCore.ImmunityFlagFor(6));
    }

    [Fact]
    public void StatesFourAndFiveShareImmunityFlag()
    {
        Assert.True(ChangeStateCore.StatesFourAndFiveShareImmunityFlag());
    }

    [Fact]
    public void StatesOneTwoThreeHaveDistinctFlags()
    {
        Assert.True(ChangeStateCore.StatesOneTwoThreeHaveDistinctFlags());
    }

    [Fact]
    public void PoisonTypesMap()
    {
        Assert.Equal(5, ChangeStateCore.PoisonTypeFor(1));   // POISON_STONE
        Assert.Null(ChangeStateCore.PoisonTypeFor(2));       // MakeFrozen
        Assert.Null(ChangeStateCore.PoisonTypeFor(3));       // OpenCobwebWinding
        Assert.Equal(1, ChangeStateCore.PoisonTypeFor(4));   // POISON_DAMAGEARMOR
        Assert.Equal(0, ChangeStateCore.PoisonTypeFor(5));   // POISON_DECHEALTH
    }

    [Fact]
    public void ForcedStateApplicationLogic()
    {
        // nParam3 = 0 → 无条件施加
        Assert.True(ChangeStateCore.ShouldApplyForcedState(0, immuneFlag: true));
        Assert.True(ChangeStateCore.ShouldApplyForcedState(0, immuneFlag: false));

        // nParam3 <> 0 → 仅未免疫时施加
        Assert.False(ChangeStateCore.ShouldApplyForcedState(1, immuneFlag: true));
        Assert.True(ChangeStateCore.ShouldApplyForcedState(1, immuneFlag: false));
    }

    [Fact]
    public void StatesSixAndSevenHaveNoParam3Branch()
    {
        Assert.True(ChangeStateCore.StatesSixAndSevenHaveNoParam3Branch());

        // 6/7 不使用 nParam3
        Assert.False(ChangeStateCore.UsesParam3(6));
        Assert.False(ChangeStateCore.UsesParam3(7));
    }

    [Fact]
    public void MakePosionLastParamAlwaysFalse()
    {
        Assert.True(ChangeStateCore.MakePosionLastParamAlwaysFalse());
    }

    // ===================== 状态 8：范围钳制与模式选择 =====================

    [Fact]
    public void RangeClampedToNonNegative()
    {
        Assert.Equal(0, ChangeStateCore.ClampRange(-5));
        Assert.Equal(0, ChangeStateCore.ClampRange(0));
        Assert.Equal(3, ChangeStateCore.ClampRange(3));
    }

    [Fact]
    public void RangeModeSelection()
    {
        Assert.True(ChangeStateCore.IsRangeMode(1));
        Assert.False(ChangeStateCore.IsRangeMode(0));
    }

    [Fact]
    public void NegativeRangeFallsToTargetMode()
    {
        // nParam3 = -5 → nRange = 0 → **单目标模式**（而非范围模式）
        Assert.Equal(0, ChangeStateCore.ClampRange(-5));
        Assert.False(ChangeStateCore.IsRangeMode(ChangeStateCore.ClampRange(-5)));
    }

    // ===================== 状态 8：六道过滤 =====================

    [Fact]
    public void FilterOrderIsSourceOrder()
    {
        // 全部条件同时为真 → 命中**第一个**（nil）
        Assert.Equal(ChangeStateCore.ImprisonFilter.Null,
            ChangeStateCore.FilterImprisonTarget(true, true, true, true, true, true));

        Assert.Equal(ChangeStateCore.ImprisonFilter.Death,
            ChangeStateCore.FilterImprisonTarget(false, true, true, true, true, true));

        Assert.Equal(ChangeStateCore.ImprisonFilter.Ghost,
            ChangeStateCore.FilterImprisonTarget(false, false, true, true, true, true));

        Assert.Equal(ChangeStateCore.ImprisonFilter.UnImprison,
            ChangeStateCore.FilterImprisonTarget(false, false, false, true, true, true));

        Assert.Equal(ChangeStateCore.ImprisonFilter.Self,
            ChangeStateCore.FilterImprisonTarget(false, false, false, false, true, true));

        Assert.Equal(ChangeStateCore.ImprisonFilter.PlayerPet,
            ChangeStateCore.FilterImprisonTarget(false, false, false, false, false, true));
    }

    [Fact]
    public void FilterPassesCleanTarget()
    {
        Assert.Equal(ChangeStateCore.ImprisonFilter.Pass,
            ChangeStateCore.FilterImprisonTarget(false, false, false, false, false, false));
    }

    [Fact]
    public void SelfAndPetAreExcludedInRangeMode()
    {
        Assert.Equal(ChangeStateCore.ImprisonFilter.Self,
            ChangeStateCore.FilterImprisonTarget(false, false, false, false, isSelf: true, masterIsPlayer: false));

        Assert.Equal(ChangeStateCore.ImprisonFilter.PlayerPet,
            ChangeStateCore.FilterImprisonTarget(false, false, false, false, isSelf: false, masterIsPlayer: true));
    }

    // ===================== 状态 8：字段与标志 =====================

    [Fact]
    public void ImprisonFieldsWrittenInOrder()
    {
        Assert.Equal(new[]
        {
            "m_dwImprisonTick", "m_dwImprisonTime", "m_nImprisonRange",
            "m_nImprisonPos.X", "m_nImprisonPos.Y", "m_boImprison",
        }, ChangeStateCore.ImprisonFieldsWritten);
    }

    [Fact]
    public void ImprisonFlagFollowsStateTime()
    {
        Assert.True(ChangeStateCore.ImprisonFlagFor(1));
        Assert.False(ChangeStateCore.ImprisonFlagFor(0));
        Assert.False(ChangeStateCore.ImprisonFlagFor(-1));
    }

    [Fact]
    public void ZeroTimeWritesFieldsButFlagFalse()
    {
        Assert.True(ChangeStateCore.ZeroTimeWritesFieldsButFlagFalse());

        // **nStateTime = 0 时字段仍写、标志为假**
        Assert.False(ChangeStateCore.ImprisonFlagFor(0));
        Assert.Equal(0, ChangeStateCore.ImprisonTimeMs(0));
        Assert.Equal(6, ChangeStateCore.ImprisonFieldsWritten.Length);
    }

    [Fact]
    public void ImprisonTimeIsMilliseconds()
    {
        Assert.Equal(5000, ChangeStateCore.ImprisonTimeMs(5));
    }

    [Fact]
    public void MagicEventCreatedOnlyWhenPositiveTime()
    {
        Assert.True(ChangeStateCore.CreatesMagicEvent(1));
        Assert.False(ChangeStateCore.CreatesMagicEvent(0));
        Assert.False(ChangeStateCore.CreatesMagicEvent(-1));
    }

    [Fact]
    public void MagicEventFields()
    {
        Assert.Equal(5000, ChangeStateCore.MagicEventDwTime(5));
        Assert.True(ChangeStateCore.MagicEventFormNpcIsTrue());
    }

    [Fact]
    public void EmptyTargetListDisposesEvent()
    {
        Assert.True(ChangeStateCore.EmptyTargetListDisposesEvent());
    }

    [Fact]
    public void StateEightSendsEffectPerTargetAnyway()
    {
        Assert.True(ChangeStateCore.StateEightSendsEffectPerTargetAnyway());
    }

    [Fact]
    public void RangeModeUsesGetMapBaseObjects()
    {
        Assert.True(ChangeStateCore.RangeModeUsesGetMapBaseObjects());
    }

    // ===================== 边框坐标算法 =====================

    [Fact]
    public void BorderCellConditionContainsFourTerms()
    {
        // 右下角：由 (nX = nMaxX) 与 (nY = nMaxY) 同时命中
        Assert.True(ChangeStateCore.IsBorderCell(2, 2, 0, 2, 0, 2));

        // 中心：不是边框
        Assert.False(ChangeStateCore.IsBorderCell(1, 1, 0, 2, 0, 2));

        // 右下角外侧
        Assert.False(ChangeStateCore.IsBorderCell(0, 0, 1, 3, 1, 3));
    }

    [Fact]
    public void BorderCellCountEqualsPerimeter()
    {
        Assert.True(ChangeStateCore.BorderCellCountEqualsPerimeter());
    }

    [Fact]
    public void BorderCellCountForRangeOne()
    {
        // nRange = 1 → 3x3 的边框 = 8 格
        Assert.Equal(8, ChangeStateCore.BorderCellCount(1));
    }

    [Fact]
    public void BorderCellCountForRangeThree()
    {
        // nRange = 3 → 7x7 的边框 = 24 格
        Assert.Equal(24, ChangeStateCore.BorderCellCount(3));
    }

    [Fact]
    public void BorderCellCountAtZeroRange()
    {
        // nRange = 0 → 只有中心一格（条件中 nMinX = nMaxX 等，四条件之一恒真）
        Assert.Equal(1, ChangeStateCore.BorderCellCountAtZeroRange());
    }

    [Fact]
    public void BorderCellSetIsExactlyTheRing()
    {
        var cells = ChangeStateCore.EnumerateBorder(0, 0, 2);

        // 恰为 |x| = 2 或 |y| = 2 的格
        var expected = new HashSet<(int, int)>();
        for (int x = -2; x <= 2; x++)
        {
            for (int y = -2; y <= 2; y++)
            {
                if (Math.Abs(x) == 2 || Math.Abs(y) == 2)
                    expected.Add((x, y));
            }
        }

        Assert.Equal(expected, new HashSet<(int, int)>(cells));
    }

    [Fact]
    public void BorderCellsAreUnique()
    {
        // 四条件互有重叠，但每个 (nX,nY) 只创建一个 Event（if 只判一次）
        var cells = ChangeStateCore.EnumerateBorder(0, 0, 4);

        Assert.Equal(cells.Count, cells.Distinct().Count());
    }

    [Fact]
    public void BorderIsOffsetByCenter()
    {
        var cells = ChangeStateCore.EnumerateBorder(100, 200, 1);

        Assert.Contains((99, 199), cells);
        Assert.Contains((101, 201), cells);
        Assert.Contains((99, 200), cells);    // 左边
        Assert.Contains((100, 201), cells);   // 下边
        // **中心不在环上**（nRange=1 时环是 8 格、中心是第 9 格）
        Assert.DoesNotContain((100, 200), cells);
        Assert.Equal(8, cells.Count);
    }

    // ===================== 状态 8：单目标模式 =====================

    [Fact]
    public void TargetModeFourNegations()
    {
        Assert.True(ChangeStateCore.ShouldImprisonTarget(false, false, false, false));

        Assert.False(ChangeStateCore.ShouldImprisonTarget(true, false, false, false));
        Assert.False(ChangeStateCore.ShouldImprisonTarget(false, true, false, false));
        Assert.False(ChangeStateCore.ShouldImprisonTarget(false, false, true, false));
        Assert.False(ChangeStateCore.ShouldImprisonTarget(false, false, false, true));
    }

    [Fact]
    public void TargetModeLacksSelfAndPetExclusion()
    {
        Assert.True(ChangeStateCore.TargetModeLacksSelfAndPetExclusion());
    }

    [Fact]
    public void PosSourceDiffersBetweenModes()
    {
        Assert.True(ChangeStateCore.RangeModePosUsesCasterTargetModeUsesTarget());
        Assert.True(ChangeStateCore.RangeModePosUsesCasterCoordinates());
        Assert.True(ChangeStateCore.TargetModePosUsesTargetCoordinates());
    }

    [Fact]
    public void TargetModeCreatesCrossOfFour()
    {
        Assert.True(ChangeStateCore.TargetModeCreatesCrossOfFour());

        var cross = ChangeStateCore.CrossOfFour(10, 20);

        Assert.Equal(4, cross.Count);
        Assert.Contains((9, 20), cross);
        Assert.Contains((11, 20), cross);
        Assert.Contains((10, 19), cross);
        Assert.Contains((10, 21), cross);
    }

    [Fact]
    public void CrossDiffersFromBorder()
    {
        Assert.True(ChangeStateCore.CrossDiffersFromBorder());
        Assert.Equal(4, ChangeStateCore.CrossOfFour(0, 0).Count);
        Assert.Equal(8, ChangeStateCore.BorderCellCount(1));
    }

    [Fact]
    public void EnqueueOrderDiffersBetweenModes()
    {
        Assert.True(ChangeStateCore.TargetModeEnqueuesBeforeWritingFields());
        Assert.True(ChangeStateCore.RangeModeEnqueuesInsideFinally());
    }

    // ===================== 状态 9 =====================

    [Fact]
    public void StateNineFlagAlwaysTrue()
    {
        Assert.True(ChangeStateCore.StateNineFlagAlwaysTrue());
        Assert.True(ChangeStateCore.StateNineIgnoresRange());
    }

    [Fact]
    public void StateNineTimeIsMilliseconds()
    {
        Assert.Equal(3000, ChangeStateCore.UnImprisonTimeMs(3));
    }

    [Fact]
    public void StateNineFlagDiffersFromStateEight()
    {
        // 状态 9 无条件 True；状态 8 跟随 nStateTime
        Assert.True(ChangeStateCore.StateNineFlagAlwaysTrue());
        Assert.False(ChangeStateCore.ImprisonFlagFor(0));
    }

    // ===================== 状态 10 / 11 =====================

    [Fact]
    public void AbsorbRateClampOrder()
    {
        Assert.True(ChangeStateCore.AbsorbRateClampOrder());
        Assert.Equal(0, ChangeStateCore.ClampAbsorbRate(-5));
        Assert.Equal(0, ChangeStateCore.ClampAbsorbRate(0));
        Assert.Equal(50, ChangeStateCore.ClampAbsorbRate(50));
        Assert.Equal(100, ChangeStateCore.ClampAbsorbRate(100));
        Assert.Equal(100, ChangeStateCore.ClampAbsorbRate(500));
    }

    [Fact]
    public void AbsorbValueClampsOnlyLowerBound()
    {
        Assert.True(ChangeStateCore.AbsorbValueClampsOnlyLowerBound());
        Assert.Equal(0, ChangeStateCore.ClampAbsorbValue(-1));
        Assert.Equal(999999, ChangeStateCore.ClampAbsorbValue(999999));
    }

    [Fact]
    public void AbsorbHpMpAreSymmetric()
    {
        Assert.True(ChangeStateCore.AbsorbHpMpAreSymmetric());
        Assert.True(ChangeStateCore.AbsorbFlagsAlwaysTrue());
    }

    // ===================== 状态 12 =====================

    [Fact]
    public void StateTwelveDifferences()
    {
        Assert.True(ChangeStateCore.StateTwelveHasNoTargetLoop());
        Assert.True(ChangeStateCore.StateTwelveCreatesEventBeforeFields());
        Assert.True(ChangeStateCore.StateTwelveHasNoEmptyCleanup());
        Assert.True(ChangeStateCore.StateTwelveAddsSelf());
    }

    [Fact]
    public void StateTwelveNeedsPositiveRange()
    {
        Assert.True(ChangeStateCore.StateTwelveNeedsPositiveRange(1));
        Assert.False(ChangeStateCore.StateTwelveNeedsPositiveRange(0));
    }

    [Fact]
    public void StateTwelveGatesEffectOnPositiveTime()
    {
        // **状态 12 把 SendCustomEffect 门控在 nStateTime > 0**；状态 8 不门控
        Assert.True(ChangeStateCore.StateTwelveGatesEffectOnPositiveTime());
        Assert.True(ChangeStateCore.StateEightSendsEffectPerTargetAnyway());

        // 两者方向相反
        Assert.False(ChangeStateCore.CreatesMagicEvent(0));   // 都用同一门控建事件
    }

    // ===================== 状态 13 =====================

    [Fact]
    public void StateThirteenFields()
    {
        Assert.Equal(5, ChangeStateCore.BloodLossTimeLeft(5));
        Assert.Equal(7, ChangeStateCore.BloodLossPoint(7));
        Assert.Equal(3000, ChangeStateCore.BloodLossIntervalMs(3));
    }

    [Fact]
    public void StateThirteenMixesTimeUnits()
    {
        // TimeLeft 用**秒**、TimeInterval 用**毫秒**
        Assert.True(ChangeStateCore.StateThirteenMixesTimeUnits());
        Assert.NotEqual(
            ChangeStateCore.BloodLossTimeLeft(5) * 1000,
            ChangeStateCore.BloodLossTimeLeft(5));   // 5 vs 5000
    }

    [Fact]
    public void BloodLossPointAcceptsNegative()
    {
        Assert.True(ChangeStateCore.BloodLossPointAcceptsNegative());
        Assert.Equal(-10, ChangeStateCore.BloodLossPoint(-10));
    }

    [Fact]
    public void StateThirteenSendsNoEffect()
    {
        Assert.True(ChangeStateCore.StateThirteenSendsNoEffect());
        Assert.False(ChangeStateCore.HasStaticEffectCall(13));
    }

    // ===================== 状态 14 =====================

    [Fact]
    public void StateFourteenOnlyForPlayersAndHeroes()
    {
        Assert.True(ChangeStateCore.StateFourteenAppliesTo(0));    // RC_PLAYOBJECT
        Assert.True(ChangeStateCore.StateFourteenAppliesTo(1));    // RC_HEROOBJECT
        Assert.False(ChangeStateCore.StateFourteenAppliesTo(80));  // RC_MONSTER
        Assert.False(ChangeStateCore.StateFourteenAppliesTo(10));  // RC_NPC
    }

    [Fact]
    public void StateFourteenOnlyForPlayers()
    {
        Assert.True(ChangeStateCore.StateFourteenOnlyForPlayers());
    }

    [Fact]
    public void CanSpellStateTickIsNowPlusTime()
    {
        Assert.Equal(5000u, ChangeStateCore.CanSpellStateTick(0, 5));
        Assert.Equal(15000u, ChangeStateCore.CanSpellStateTick(10000, 5));
    }

    [Fact]
    public void StateFourteenZeroMeansEnabled()
    {
        Assert.True(ChangeStateCore.StateFourteenZeroMeansEnabled());

        // nStateTime = 0 → **恢复可用**（True）
        Assert.True(ChangeStateCore.CanSpellStateFlag(0));
        Assert.False(ChangeStateCore.CanSpellStateFlag(1));
    }

    [Fact]
    public void FlagDirectionInvertedVersusStateEight()
    {
        // 状态 8：0 → False（不启用）；状态 14：0 → True（解除限制）
        Assert.True(ChangeStateCore.FlagDirectionIsInvertedVersusStateEight());
    }

    [Fact]
    public void StateFourteenSendsNoEffect()
    {
        Assert.True(ChangeStateCore.StateFourteenSendsNoEffect());
        Assert.False(ChangeStateCore.HasStaticEffectCall(14));
    }

    // ===================== 分派表 =====================

    [Fact]
    public void StaticEffectCallTable()
    {
        for (int s = 1; s <= 12; s++)
            Assert.True(ChangeStateCore.HasStaticEffectCall(s), $"state {s} should call effect");

        Assert.False(ChangeStateCore.HasStaticEffectCall(13));
        Assert.False(ChangeStateCore.HasStaticEffectCall(14));
    }

    [Fact]
    public void UsesParam3Table()
    {
        foreach (int s in new[] { 1, 2, 3, 4, 5, 8, 10, 11, 12, 13 })
            Assert.True(ChangeStateCore.UsesParam3(s), $"state {s} uses nParam3");

        foreach (int s in new[] { 6, 7, 9, 14 })
            Assert.False(ChangeStateCore.UsesParam3(s), $"state {s} ignores nParam3");
    }

    [Fact]
    public void UsesParam4Table()
    {
        foreach (int s in new[] { 10, 11, 13 })
            Assert.True(ChangeStateCore.UsesParam4(s), $"state {s} uses nParam4");

        foreach (int s in new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 12, 14 })
            Assert.False(ChangeStateCore.UsesParam4(s), $"state {s} ignores nParam4");
    }

    [Fact]
    public void StatesIgnoringParam3And4Listed()
    {
        Assert.Equal(new[] { 6, 7, 9, 14 }, ChangeStateCore.StatesIgnoringParam3And4);
    }

    [Fact]
    public void ParamIgnoringStatesAreConsistent()
    {
        foreach (int s in ChangeStateCore.StatesIgnoringParam3And4)
        {
            Assert.False(ChangeStateCore.UsesParam3(s));
            Assert.False(ChangeStateCore.UsesParam4(s));
        }
    }

    // ===================== 首饰盒 =====================

    [Fact]
    public void ActivateCasketOnlyFromNoActive()
    {
        var (s1, sent1) = ChangeStateCore.ActivateCasket(
            ChangeStateCore.JewelryBoxStatus.NoActive, 0);

        Assert.Equal(ChangeStateCore.JewelryBoxStatus.Active, s1);
        Assert.True(sent1);

        var (s2, sent2) = ChangeStateCore.ActivateCasket(
            ChangeStateCore.JewelryBoxStatus.Active, 0);

        Assert.Equal(ChangeStateCore.JewelryBoxStatus.Active, s2);
        Assert.False(sent2);
    }

    [Fact]
    public void CloseCasketOnlyFromActive()
    {
        var (s1, sent1) = ChangeStateCore.CloseCasket(
            ChangeStateCore.JewelryBoxStatus.Active, 0);

        Assert.Equal(ChangeStateCore.JewelryBoxStatus.NoActive, s1);
        Assert.True(sent1);

        var (s2, sent2) = ChangeStateCore.CloseCasket(
            ChangeStateCore.JewelryBoxStatus.NoActive, 0);

        Assert.Equal(ChangeStateCore.JewelryBoxStatus.NoActive, s2);
        Assert.False(sent2);
    }

    [Fact]
    public void CasketCommandsAreIdempotent()
    {
        Assert.True(ChangeStateCore.CasketCommandsAreIdempotent());
    }

    [Fact]
    public void CasketSendParamIsHeroFlag()
    {
        Assert.True(ChangeStateCore.CasketSendParamIsHeroFlag(1));    // RC_HEROOBJECT
        Assert.False(ChangeStateCore.CasketSendParamIsHeroFlag(0));   // RC_PLAYOBJECT
        Assert.False(ChangeStateCore.CasketSendParamIsHeroFlag(80));
    }

    [Fact]
    public void CasketIgnoresNonPlayers()
    {
        Assert.True(ChangeStateCore.CasketIgnoresNonPlayers());
    }

    [Fact]
    public void CasketWorksForHero()
    {
        var (s, sent) = ChangeStateCore.ActivateCasket(
            ChangeStateCore.JewelryBoxStatus.NoActive, 1);

        Assert.Equal(ChangeStateCore.JewelryBoxStatus.Active, s);
        Assert.True(sent);
        Assert.True(ChangeStateCore.CasketSendParamIsHeroFlag(1));
    }
}
