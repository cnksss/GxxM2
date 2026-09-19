using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J137：钉刺怪物 —— `TStickMonster`（ObjMon2.pas 9-22、437-610）1:1 测试。
/// </summary>
public sealed class StickMonsterCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(StickMonsterCore.ConstantsMatchSource());
        Assert.Equal(0, StickMonsterCore.DrUp);
        Assert.Equal(7, StickMonsterCore.DrUpLeft);
        Assert.Equal(20099, StickMonsterCore.RmDigUp);
        Assert.Equal(20100, StickMonsterCore.RmDigDown);
        Assert.Equal(85, StickMonsterCore.StickRace);
        Assert.Equal(7, StickMonsterCore.ViewRange);
    }

    [Fact]
    public void DirectionsAreContiguous()
    {
        Assert.True(StickMonsterCore.DirectionsAreContiguous());
    }

    [Fact]
    public void DigMessagesArePaired()
    {
        Assert.True(StickMonsterCore.DigMessagesArePaired());
        Assert.True(StickMonsterCore.DigUpSharedWithDoorAndWall());
        Assert.True(StickMonsterCore.SameMessageFamilyAsDoorAndWall());
    }

    [Fact]
    public void MethodNameIsMisspelled()
    {
        // **方法名少了 i：VisbleActors**
        Assert.True(StickMonsterCore.MethodNameIsMisspelled());
        Assert.Equal("VisbleActors", StickMonsterCore.MisspelledMethodName);
        Assert.Equal("VisibleActors", StickMonsterCore.CorrectMethodName);
    }

    [Fact]
    public void ExceptionMsgIsResourceString()
    {
        Assert.True(StickMonsterCore.ExceptionMsgIsResourceString());
        Assert.True(StickMonsterCore.DifferentFromLocatorCodeStyle());
        Assert.Contains("Dispose", StickMonsterCore.ExceptionMsg);
    }

    // ===================== 一、Create =====================

    [Fact]
    public void CreateEleven()
    {
        Assert.True(StickMonsterCore.CreateEleven());
        Assert.Equal((false, 7, 250, 2500, 1500, 85, 4, 4, true, true, true),
            StickMonsterCore.CreateInit());
    }

    [Fact]
    public void RunTimeIsRedundant()
    {
        // **m_nRunTime := 250 与基类默认完全相同 → 冗余赋值**
        Assert.True(StickMonsterCore.RunTimeIsRedundant());
        Assert.Equal(StickMonsterCore.BaseRunTime, StickMonsterCore.RunTime);
    }

    [Fact]
    public void SearchTimeRangeDiffers()
    {
        // **m_dwSearchTime 与基类不同**
        Assert.True(StickMonsterCore.SearchTimeRangeDiffers());
        Assert.True(StickMonsterCore.SearchTimeNarrowedWithSameUpperBound());
        Assert.True(StickMonsterCore.SearchTimeRangeValues());
        Assert.Equal(((2500, 3999), (2000, 3999)), StickMonsterCore.SearchTimeRanges());
    }

    [Fact]
    public void StartsInFixedHideMode()
    {
        // **出生时处于潜伏状态**
        Assert.True(StickMonsterCore.StartsInFixedHideMode());
        Assert.True(StickMonsterCore.BaseDefaultIsFalse());
    }

    [Fact]
    public void SharesStickAndAnimalWithDoorAndWall()
    {
        Assert.True(StickMonsterCore.SharesStickAndAnimalWithDoorAndWall());
    }

    [Fact]
    public void Bo550IsDeadField()
    {
        // **bo550 全文再无读写**
        Assert.True(StickMonsterCore.Bo550IsDeadField());
        Assert.Equal(1, StickMonsterCore.DeadFieldCount());
        Assert.True(StickMonsterCore.Bo550InitFalse());
    }

    [Fact]
    public void RaceHasNoConstant()
    {
        Assert.True(StickMonsterCore.RaceHasNoConstant());
    }

    // ===================== 二、n554 与 n558 =====================

    [Fact]
    public void TwoFieldsSameInitDifferentUse()
    {
        // **两个字段初值都是 4，但用途完全不同**
        Assert.True(StickMonsterCore.TwoFieldsSameInitDifferentUse());
        Assert.True(StickMonsterCore.SameFieldDifferentMeaningPrecedent());
    }

    [Fact]
    public void TriggerUsesStrictLess()
    {
        // **潜伏解除用严格小于**
        Assert.True(StickMonsterCore.TriggerUsesStrictLess());
        Assert.True(StickMonsterCore.TriggerUnhides(3, 3, StickMonsterCore.TriggerRadius));
        Assert.False(StickMonsterCore.TriggerUnhides(4, 0, StickMonsterCore.TriggerRadius));
    }

    [Fact]
    public void TriggerCovers49Cells()
    {
        // **覆盖 -3..3 共 7x7 = 49 格**
        Assert.True(StickMonsterCore.TriggerCovers49Cells());
        Assert.Equal(49, StickMonsterCore.TriggerCellCount());
    }

    [Fact]
    public void LeashUsesStrictGreater()
    {
        // **脱缰用严格大于**
        Assert.True(StickMonsterCore.LeashUsesStrictGreater());
        Assert.True(StickMonsterCore.LeashBreaks(5, 0, StickMonsterCore.LeashRadius));
        Assert.False(StickMonsterCore.LeashBreaks(4, 0, StickMonsterCore.LeashRadius));
    }

    [Fact]
    public void DeadZoneAtExactlyFour()
    {
        // **恰好 4 时既不触发也不脱缰**
        Assert.True(StickMonsterCore.DeadZoneAtExactlyFour());
        Assert.True(StickMonsterCore.DeadZoneIsExactlyFour());
        Assert.Equal(new[] { 4 }, StickMonsterCore.DeadZoneOffsets());
    }

    [Fact]
    public void ComparisonOperatorsAreOpposite()
    {
        Assert.True(StickMonsterCore.ComparisonOperatorsAreOpposite());
    }

    [Fact]
    public void DeadZoneBoundarySummary()
    {
        // 0..3 触发解除；4 落死区；>=5 脱缰
        for (int d = 0; d <= 3; d++)
        {
            Assert.True(StickMonsterCore.TriggerUnhides(d, 0, 4));
            Assert.False(StickMonsterCore.LeashBreaks(d, 0, 4));
        }

        Assert.False(StickMonsterCore.TriggerUnhides(4, 0, 4));
        Assert.False(StickMonsterCore.LeashBreaks(4, 0, 4));

        for (int d = 5; d <= 8; d++)
        {
            Assert.False(StickMonsterCore.TriggerUnhides(d, 0, 4));
            Assert.True(StickMonsterCore.LeashBreaks(d, 0, 4));
        }
    }

    // ===================== 三、sub_FFE9 与 VisbleActors =====================

    [Fact]
    public void UnhideClearsFlagAndSendsDigUp()
    {
        Assert.True(StickMonsterCore.UnhideClearsFlagAndSendsDigUp());
        Assert.Equal((false, 20099), StickMonsterCore.Unhide());
    }

    [Fact]
    public void HideSetsFlagAndSendsDigDown()
    {
        Assert.True(StickMonsterCore.HideSetsFlagAndSendsDigDown());
        Assert.Equal((true, 20100), StickMonsterCore.Hide());
    }

    [Fact]
    public void TwoOperationsAreInverse()
    {
        Assert.True(StickMonsterCore.TwoOperationsAreInverse());
    }

    [Fact]
    public void DisposeBeforeClearIsNecessary()
    {
        // **Clear 不调用析构，故 Dispose 必须**
        Assert.True(StickMonsterCore.DisposeBeforeClearIsNecessary());
        Assert.True(StickMonsterCore.ClearDoesNotDispose());
    }

    [Fact]
    public void FlagSetOutsideTry()
    {
        // **异常也会置位**
        Assert.True(StickMonsterCore.FlagSetOutsideTry());
        Assert.True(StickMonsterCore.FlagSetEvenOnException());
    }

    [Fact]
    public void ClearsVisibleList()
    {
        Assert.True(StickMonsterCore.ClearsVisibleList());
    }

    // ===================== 四、sub_FFEA =====================

    [Fact]
    public void CoolEyeTruthTable()
    {
        // **隐身目标需要冷眼**
        Assert.True(StickMonsterCore.CoolEyeTruthTable());
        Assert.True(StickMonsterCore.HiddenTargetNeedsCoolEye());
    }

    [Fact]
    public void CanSeeTargetValues()
    {
        Assert.True(StickMonsterCore.CanSeeTarget(false, false));
        Assert.False(StickMonsterCore.CanSeeTarget(true, false));
        Assert.True(StickMonsterCore.CanSeeTarget(true, true));
    }

    [Fact]
    public void OrderOfFilters()
    {
        Assert.True(StickMonsterCore.OrderOfFilters());
        Assert.True(StickMonsterCore.DeathCheckedBeforeProper());
    }

    [Fact]
    public void FilterTruthTable()
    {
        Assert.True(StickMonsterCore.FilterTruthTable());
    }

    [Fact]
    public void PassesAllFiltersValues()
    {
        Assert.True(StickMonsterCore.PassesAllFilters(false, false, true, false, false, 1, 1));
        Assert.False(StickMonsterCore.PassesAllFilters(true, false, true, false, false, 1, 1));
        Assert.False(StickMonsterCore.PassesAllFilters(false, true, true, false, false, 1, 1));
        Assert.False(StickMonsterCore.PassesAllFilters(false, false, false, false, false, 1, 1));
        Assert.False(StickMonsterCore.PassesAllFilters(false, false, true, true, false, 1, 1));
        Assert.False(StickMonsterCore.PassesAllFilters(false, false, true, false, false, 10, 0));
    }

    [Fact]
    public void BreaksAfterUnhide()
    {
        // **最多解除一次**
        Assert.True(StickMonsterCore.BreaksAfterUnhide());
        Assert.True(StickMonsterCore.OnlyFirstMatchUnhides());
        Assert.True(StickMonsterCore.NoMatchNoUnhide());
        Assert.Equal(1, StickMonsterCore.UnhideCountFor(new[] { true, true, true }));
    }

    // ===================== 五、AttackTarget =====================

    [Fact]
    public void InAttackRangeTruthTable()
    {
        Assert.True(StickMonsterCore.InAttackRangeTruthTable());
        Assert.True(StickMonsterCore.SameCellRejected());
    }

    [Fact]
    public void InAttackRangeValues()
    {
        Assert.False(StickMonsterCore.InAttackRange(5, 5, 5, 5));
        Assert.True(StickMonsterCore.InAttackRange(5, 5, 6, 5));
        Assert.True(StickMonsterCore.InAttackRange(5, 5, 4, 6));
        Assert.False(StickMonsterCore.InAttackRange(5, 5, 7, 5));
    }

    [Fact]
    public void DirectionPriorityOrder()
    {
        Assert.True(StickMonsterCore.DirectionPriorityOrder());
        Assert.True(StickMonsterCore.DirectionPrioritySequence());
        Assert.True(StickMonsterCore.CardinalsBeforeDiagonals());
        Assert.Equal(8, StickMonsterCore.DirectionPriority.Length);
    }

    [Fact]
    public void DirectionPrioritySequenceValues()
    {
        var expected = new[]
        {
            StickMonsterCore.DrLeft, StickMonsterCore.DrRight,
            StickMonsterCore.DrUp, StickMonsterCore.DrDown,
            StickMonsterCore.DrUpLeft, StickMonsterCore.DrUpRight,
            StickMonsterCore.DrDownLeft, StickMonsterCore.DrDownRight,
        };

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i], StickMonsterCore.DirectionPriority[i].Dir);
        }
    }

    [Fact]
    public void ComputeDirectionValues()
    {
        Assert.True(StickMonsterCore.ComputeDirectionValues());
        Assert.Equal(StickMonsterCore.DrLeft, StickMonsterCore.ComputeDirection(5, 5, 4, 5));
        Assert.Equal(StickMonsterCore.DrUpLeft, StickMonsterCore.ComputeDirection(5, 5, 4, 4));
        Assert.Equal(StickMonsterCore.DrDownRight, StickMonsterCore.ComputeDirection(5, 5, 6, 6));
    }

    [Fact]
    public void EachBranchExits()
    {
        Assert.True(StickMonsterCore.EachBranchExits());
    }

    [Fact]
    public void FallbackZeroIsDeadCode()
    {
        // **btDir := 0 的回退实际走不到（穷举验证）**
        Assert.True(StickMonsterCore.FallbackZeroIsDeadCode());
    }

    [Fact]
    public void HitIntervalUsesSumStrictGreater()
    {
        // **和值、严格大于**
        Assert.True(StickMonsterCore.HitIntervalUsesSumStrictGreater());
        Assert.False(StickMonsterCore.HitIntervalDue(0, 100, 60, 40));
        Assert.True(StickMonsterCore.HitIntervalDue(0, 101, 60, 40));
    }

    [Fact]
    public void AttackTargetResultTruthTable()
    {
        // **返回"能打到"而非"打了"**
        Assert.True(StickMonsterCore.AttackTargetResultTruthTable());
        Assert.True(StickMonsterCore.ReturnsTrueEvenWhenIntervalNotDue());
        Assert.True(StickMonsterCore.AttackTargetResult(true, true));
    }

    [Fact]
    public void PursuesWhenSameMap()
    {
        Assert.True(StickMonsterCore.PursuesWhenSameMap());
        Assert.True(StickMonsterCore.DropsWhenDifferentMap());
        Assert.Equal("SetTargetXY", StickMonsterCore.OutsideRangeAction(true));
        Assert.Equal("DelTargetCreat", StickMonsterCore.OutsideRangeAction(false));
    }

    [Fact]
    public void ThreeRefreshedFields()
    {
        Assert.True(StickMonsterCore.AttackRefreshesThreeFields());
        Assert.Equal(3, StickMonsterCore.RefreshedOnAttack.Length);
        Assert.Contains("m_dwHitTick", StickMonsterCore.RefreshedOnAttack);
        Assert.Contains("m_nHitDelay", StickMonsterCore.RefreshedOnAttack);
        Assert.Contains("m_dwTargetFocusTick", StickMonsterCore.RefreshedOnAttack);
    }

    // ===================== 六、Run =====================

    [Fact]
    public void GuardOrderReversed()
    {
        // **先判幽灵后判死亡（与 J134/J135/J136 相反）**
        Assert.True(StickMonsterCore.GuardOrderReversed());
        Assert.True(StickMonsterCore.RunGateTruthTable());
    }

    [Fact]
    public void WalkTickUsesStrictGreater()
    {
        Assert.True(StickMonsterCore.WalkTickUsesStrictGreater());
        Assert.True(StickMonsterCore.WalkTickResetsDelay());
    }

    [Fact]
    public void TwoBranchesByFixedHide()
    {
        Assert.True(StickMonsterCore.TwoBranchesByFixedHide());
        Assert.Equal("sub_FFEA", StickMonsterCore.BranchByFixedHide(true));
    }

    [Fact]
    public void Bo05TruthTable()
    {
        Assert.True(StickMonsterCore.Bo05TruthTable());
        Assert.True(StickMonsterCore.NoTargetRehides());
    }

    [Fact]
    public void ComputeBo05Values()
    {
        Assert.False(StickMonsterCore.ComputeBo05(true, 1, 1));
        Assert.True(StickMonsterCore.ComputeBo05(true, 5, 0));
        Assert.True(StickMonsterCore.ComputeBo05(true, 0, 9));
        Assert.True(StickMonsterCore.ComputeBo05(false, 0, 0));
    }

    [Fact]
    public void TwoActionsByBo05()
    {
        Assert.True(StickMonsterCore.TwoActionsByBo05());
        Assert.Equal("VisbleActors", StickMonsterCore.ActionByBo05(true));
        Assert.Equal("AttackTarget", StickMonsterCore.ActionByBo05(false));
    }

    [Fact]
    public void HitTickNotRefreshedHere()
    {
        // **m_dwHitTick 在此处不刷新，刷新在 AttackTarget 内部**
        Assert.True(StickMonsterCore.HitTickNotRefreshedHere());
        Assert.True(StickMonsterCore.RefreshHappensInAttackTarget());
    }

    [Fact]
    public void InheritedOnceOnBothPaths()
    {
        // **两条路径都恰好一次 inherited**
        Assert.True(StickMonsterCore.InheritedOnceOnBothPaths());
        Assert.True(StickMonsterCore.BothPathsInheritExactlyOnce());
        Assert.True(StickMonsterCore.EarlyExitSkipsTrailingInherited());
        Assert.Equal(1, StickMonsterCore.InheritedCallCount(true));
        Assert.Equal(1, StickMonsterCore.InheritedCallCount(false));
    }

    [Fact]
    public void SearchTickUsesSumStrictGreater()
    {
        Assert.True(StickMonsterCore.SearchTickUsesSumStrictGreater());
        Assert.True(StickMonsterCore.SearchResetsHitDelay());
    }

    [Fact]
    public void OperateIsPureForward()
    {
        Assert.True(StickMonsterCore.OperateIsPureForward());
        Assert.True(StickMonsterCore.OperateAddsNothing());
    }

    // ===================== 七、仿真 =====================

    [Fact]
    public void RunInFixedHideCallsSubFfea()
    {
        Assert.True(StickMonsterCore.RunInFixedHideCallsSubFfea());
        Assert.Equal("sub_FFEA",
            StickMonsterCore.RunTick(false, false, true, true, true, false, false, 0, 0, false).Path);
    }

    [Fact]
    public void RunNoTargetHides()
    {
        Assert.True(StickMonsterCore.RunNoTargetHides());
    }

    [Fact]
    public void RunInRangeAttacks()
    {
        Assert.True(StickMonsterCore.RunInRangeAttacks());
        Assert.Equal("AttackTarget+Exit",
            StickMonsterCore.RunTick(false, false, true, true, false, false, true, 1, 1, true).Path);
    }

    [Fact]
    public void RunLeashedHides()
    {
        // **脱缰则重新潜伏**
        Assert.True(StickMonsterCore.RunLeashedHides());
        Assert.Equal("VisbleActors",
            StickMonsterCore.RunTick(false, false, true, true, false, false, true, 9, 0, false).Path);
    }

    [Fact]
    public void RunTickNotDueDoesNothing()
    {
        Assert.True(StickMonsterCore.RunTickNotDueDoesNothing());
    }

    [Fact]
    public void RunGateBlocks()
    {
        Assert.True(StickMonsterCore.RunGateBlocks());
    }

    [Fact]
    public void RunInheritsOnceOnEveryPath()
    {
        Assert.True(StickMonsterCore.RunInheritsOnceOnEveryPath());
    }

    [Fact]
    public void RunFallthroughPaths()
    {
        // 有目标、未脱缰、但不在射程 → 落到 inherited
        var r = StickMonsterCore.RunTick(false, false, true, true, false, false, true, 1, 1, false);
        Assert.StartsWith("fallthrough", r.Path);
        Assert.True(r.Inherited);

        // 搜索节拍命中时路径名带 -after-search
        var r2 = StickMonsterCore.RunTick(false, false, true, true, false, true, true, 1, 1, false);
        Assert.Equal("fallthrough-after-search", r2.Path);
    }
}
