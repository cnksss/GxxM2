using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J136：巡回弓箭手与魔王岭弓箭手 —— `TMoveArcherGuard` 与 `TDevilkingArcherGuard`
/// （ObjMon2.pas 100-126、216-433、2221-2559）1:1 测试。
/// </summary>
public sealed class MoveArcherGuardCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(MoveArcherGuardCore.ConstantsMatchSource());
        Assert.Equal(11, MoveArcherGuardCore.RcGuard);
        Assert.Equal(50, MoveArcherGuardCore.RcAnimal);
        Assert.Equal(108, MoveArcherGuardCore.DevilkingTargetRace);
        Assert.Equal(109, MoveArcherGuardCore.DevilkingOwnRace);
        Assert.Equal(154, MoveArcherGuardCore.DevilkingAltRace);
        Assert.Equal(156, MoveArcherGuardCore.DevilkingAltImage);
    }

    [Fact]
    public void ItemHideValue()
    {
        Assert.True(MoveArcherGuardCore.ItemHideValue());
        Assert.Equal(20082, MoveArcherGuardCore.RmItemHide);
    }

    [Fact]
    public void ThreeSourceComments()
    {
        Assert.True(MoveArcherGuardCore.ThreeSourceComments());
        Assert.Equal("// 不攻击卫士", MoveArcherGuardCore.GuardComment);
        Assert.Contains("魔王岭", MoveArcherGuardCore.DevilkingComment);
    }

    [Fact]
    public void PickItemDefaultsFalse()
    {
        // **配置默认 False**
        Assert.True(MoveArcherGuardCore.PickItemDefaultsFalse());
    }

    // ===================== 一、四份管线变体 =====================

    [Fact]
    public void FourCopiesShareSkeleton()
    {
        Assert.True(MoveArcherGuardCore.FourCopiesShareSkeleton());
        Assert.Equal(12, MoveArcherGuardCore.SharedSkeleton.Length);
        Assert.True(MoveArcherGuardCore.FourCopiesExist());
        Assert.Equal(4, MoveArcherGuardCore.FourCopies.Length);
    }

    [Fact]
    public void MoveArcherGateBoundaryDiffers()
    {
        // **巡回版的门只包防御减免与物伤减少；弓箭手版还包住元素增伤**
        Assert.True(MoveArcherGuardCore.MoveArcherGateBoundaryDiffers());
        Assert.True(MoveArcherGuardCore.GateOnlyWrapsDefenseAndReduction());
        Assert.Equal(MoveArcherGuardCore.GateScope.DefenseAndReduction, MoveArcherGuardCore.MoveArcherGate());
        Assert.Equal(MoveArcherGuardCore.GateScope.DefenseReductionAndElement, MoveArcherGuardCore.ArcherGate());
    }

    [Fact]
    public void PowerRateAddOrderDiffers()
    {
        // **同一调用在两份拷贝里的相对次序相反**
        Assert.True(MoveArcherGuardCore.PowerRateAddOrderDiffers());
        Assert.True(MoveArcherGuardCore.MoveArcherRateAddAfterRandomRoll());
        Assert.True(MoveArcherGuardCore.IcicleRateAddBeforeRandomRoll());
    }

    [Fact]
    public void TwoSequencesDiffer()
    {
        Assert.True(MoveArcherGuardCore.TwoSequencesDiffer());
        Assert.True(MoveArcherGuardCore.RateAddPositionInMoveArcher());
        Assert.Equal(5, MoveArcherGuardCore.MoveArcherRateAddIndex());
    }

    [Fact]
    public void FourCopiesExistNames()
    {
        Assert.Contains("TIcicleMonster.AttackTarget (J134)", MoveArcherGuardCore.FourCopies);
        Assert.Contains("TDevilkingArcherGuard.sub_4A6B30 (J136)", MoveArcherGuardCore.FourCopies);
    }

    // ===================== 二、恶魔弓箭手攻击力 =====================

    [Fact]
    public void DevilkingSmallIntTruncation()
    {
        // **40000 → -25536，跨度为 -25535**
        Assert.True(MoveArcherGuardCore.DevilkingSmallIntTruncation());
        Assert.Equal(-25535, MoveArcherGuardCore.SpanSmallInt(0, 40000));
    }

    [Fact]
    public void TruncationValues()
    {
        Assert.True(MoveArcherGuardCore.TruncationValues());
        Assert.Equal(-25536, MoveArcherGuardCore.TruncateShort(40000));
        Assert.Equal(-32768, MoveArcherGuardCore.TruncateShort(32768));
        Assert.Equal(0, MoveArcherGuardCore.TruncateShort(65536));
    }

    [Fact]
    public void CLanguageRejectsConstantOverflow()
    {
        // **C# 常量越界转换是编译错误、需 unchecked；Delphi 静默截断**
        Assert.True(MoveArcherGuardCore.CLanguageRejectsConstantOverflow());
    }

    [Fact]
    public void TruncationLeadsToNegativeSpan()
    {
        Assert.True(MoveArcherGuardCore.TruncationLeadsToNegativeSpan());
    }

    [Fact]
    public void SmallIntNormalRange()
    {
        Assert.True(MoveArcherGuardCore.SmallIntNormalRange());
        Assert.Equal(11, MoveArcherGuardCore.SpanSmallInt(10, 20));
        Assert.Equal(101, MoveArcherGuardCore.SpanSmallInt(0, 100));
    }

    [Fact]
    public void DevilkingRollUpperIsExclusive()
    {
        // **上限是 span - 1（开区间）**
        Assert.True(MoveArcherGuardCore.DevilkingRollUpperIsExclusive());
        Assert.Equal(20, MoveArcherGuardCore.DevilkingPower(10, 11, 10));
    }

    [Fact]
    public void DevilkingUsesRandomSpan()
    {
        Assert.True(MoveArcherGuardCore.DevilkingUsesRandomSpan());
        Assert.True(MoveArcherGuardCore.DevilkingSkipsRollWhenSpanNonPositive());
    }

    [Fact]
    public void AbsorbPositionDiffers()
    {
        // **恶魔版吸收在防御之前；另外三份在封顶之后**
        Assert.True(MoveArcherGuardCore.DevilkingAbsorbBeforeDefense());
        Assert.True(MoveArcherGuardCore.OthersAbsorbAfterCap());
        Assert.True(MoveArcherGuardCore.AbsorbPositionDiffers());
        Assert.True(MoveArcherGuardCore.DevilkingAbsorbIndexBeforeDefense());
    }

    [Fact]
    public void DevilkingLacksPowerRateAdd()
    {
        Assert.True(MoveArcherGuardCore.DevilkingLacksPowerRateAdd());
    }

    [Fact]
    public void DevilkingReboundSubtractionReversed()
    {
        // **减数顺序相反，数值等价**
        Assert.True(MoveArcherGuardCore.DevilkingReboundSubtractionReversed());
        Assert.True(MoveArcherGuardCore.SubtractionOrderIsValueEquivalent());
    }

    [Fact]
    public void DevilkingElementOutsideGate()
    {
        Assert.True(MoveArcherGuardCore.DevilkingElementOutsideGate());
    }

    [Fact]
    public void DevilkingStepsDiffer()
    {
        Assert.True(MoveArcherGuardCore.DevilkingStepsDiffer());
        Assert.Equal(8, MoveArcherGuardCore.DevilkingSteps.Length);
        Assert.Equal(8, MoveArcherGuardCore.MoveArcherSteps.Length);
    }

    // ===================== 三、TMoveArcherGuard.Run =====================

    [Fact]
    public void WalkTickResetDiffers()
    {
        // **巡回版的 m_dwWalkTick 赋值被注释掉**
        Assert.True(MoveArcherGuardCore.WalkTickResetCommentedOut());
        Assert.True(MoveArcherGuardCore.ArcherWalkTickResetLive());
        Assert.True(MoveArcherGuardCore.WalkTickResetDiffers());
        Assert.True(MoveArcherGuardCore.SelectionDoesNotConsumeWalkTick());
    }

    [Fact]
    public void AlsoSkipsGuard()
    {
        // **额外排除 RC_GUARD（大刀守卫）**
        Assert.True(MoveArcherGuardCore.AlsoSkipsGuard());
        Assert.True(MoveArcherGuardCore.FourRacesExcluded());
    }

    [Fact]
    public void MoveArcherLoopFilterValues()
    {
        Assert.False(MoveArcherGuardCore.MoveArcherLoopFilter(128));
        Assert.False(MoveArcherGuardCore.MoveArcherLoopFilter(11));
        Assert.False(MoveArcherGuardCore.MoveArcherLoopFilter(112));
        Assert.False(MoveArcherGuardCore.MoveArcherLoopFilter(142));
        Assert.True(MoveArcherGuardCore.MoveArcherLoopFilter(80));
    }

    [Fact]
    public void LacksGhostFilter()
    {
        // **巡回版没有 m_boGhost 过滤（弓箭手版有）**
        Assert.True(MoveArcherGuardCore.LacksGhostFilter());
        Assert.True(MoveArcherGuardCore.ExclusionSetsDiffer());
    }

    [Fact]
    public void AssignsTargetFieldDirectly()
    {
        // **直接赋值 m_TargetCret 而非调用 SetTargetCreat**
        Assert.True(MoveArcherGuardCore.AssignsTargetFieldDirectly());
        Assert.True(MoveArcherGuardCore.SetTargetStylesDiffer());
        Assert.Equal("m_TargetCret := obj / nil", MoveArcherGuardCore.SetTargetStyle(false));
        Assert.Equal("SetTargetCreat/DelTargetCreat", MoveArcherGuardCore.SetTargetStyle(true));
    }

    [Fact]
    public void TwoComparisonsInOneRun()
    {
        // **同一函数里选靶用 >=、移动用 >**
        Assert.True(MoveArcherGuardCore.TwoComparisonsInOneRun());
        Assert.True(MoveArcherGuardCore.SelectDue(0, 5, 3, 2));
        Assert.False(MoveArcherGuardCore.MoveDue(0, 5, 3, 2));
        Assert.True(MoveArcherGuardCore.MoveDue(0, 6, 3, 2));
    }

    // ===================== 四、巡逻路径状态机 =====================

    [Fact]
    public void PatrolOnlyWhenNoTargetAndPath()
    {
        Assert.True(MoveArcherGuardCore.PatrolOnlyWhenNoTargetAndPath());
    }

    [Fact]
    public void SinglePointNoPatrol()
    {
        // **恰好 1 个路径点不巡逻（Length > 1）**
        Assert.True(MoveArcherGuardCore.SinglePointNoPatrol());
        Assert.True(MoveArcherGuardCore.TwoPointsPatrol());
    }

    [Fact]
    public void WaitLockStrictGreater()
    {
        Assert.True(MoveArcherGuardCore.WaitLockStrictGreater());
        Assert.False(MoveArcherGuardCore.WaitUnlocks(0, 100, 100));
        Assert.True(MoveArcherGuardCore.WaitUnlocks(0, 101, 100));
    }

    [Fact]
    public void PickItemTruthTable()
    {
        Assert.True(MoveArcherGuardCore.PickItemTruthTable());
        Assert.True(MoveArcherGuardCore.PickItemNeedsConfig());
        Assert.True(MoveArcherGuardCore.PickItemGhostsAndHides());
    }

    [Fact]
    public void ItemHideParamOrder()
    {
        // **第 2 参是 0、第 3 参才是对象指针**
        Assert.True(MoveArcherGuardCore.ItemHideParamOrder());
        Assert.Equal(20082, MoveArcherGuardCore.ItemHideArgs().Msg);
        Assert.Equal(0, MoveArcherGuardCore.ItemHideArgs().Arg1);
        Assert.Equal("NativeInt(ItemObj)", MoveArcherGuardCore.ItemHideArgs().Arg2);
    }

    [Fact]
    public void StepLockTriggers()
    {
        Assert.True(MoveArcherGuardCore.StepLockTriggers());
        Assert.True(MoveArcherGuardCore.StepLockIsStrict());
        Assert.False(MoveArcherGuardCore.AfterStep(3, 3).Locked);
        Assert.True(MoveArcherGuardCore.AfterStep(4, 3).Locked);
    }

    [Fact]
    public void TargetResetOnArrivalOrExhaustion()
    {
        Assert.True(MoveArcherGuardCore.TargetResetOnArrivalOrExhaustion());
        Assert.True(MoveArcherGuardCore.ExhaustionIsStrict());
    }

    [Fact]
    public void IndexWrapsAndGuardsNegative()
    {
        // **越界归零 + 负数归零双重保护**
        Assert.True(MoveArcherGuardCore.IndexWrapsAndGuardsNegative());
        Assert.Equal(0, MoveArcherGuardCore.NextIndex(1, 2));
        Assert.Equal(1, MoveArcherGuardCore.NextIndex(0, 3));
        Assert.Equal(0, MoveArcherGuardCore.NextIndex(-5, 3));
    }

    [Fact]
    public void IndexUpperIsHigh()
    {
        Assert.True(MoveArcherGuardCore.IndexUpperIsHigh());
    }

    [Fact]
    public void KeepMaxIsOneAndHalfChebyshev()
    {
        // **切比雪夫距离的 1.5 倍**
        Assert.True(MoveArcherGuardCore.KeepMaxIsOneAndHalfChebyshev());
        Assert.Equal(15, MoveArcherGuardCore.KeepMax(0, 0, 10, 10));
        Assert.Equal(6, MoveArcherGuardCore.KeepMax(0, 0, 4, 0));
        Assert.Equal(0, MoveArcherGuardCore.KeepMax(0, 0, 0, 0));
    }

    [Fact]
    public void KeepMaxUsesChebyshev()
    {
        Assert.True(MoveArcherGuardCore.KeepMaxUsesChebyshev());
        Assert.Equal(5, MoveArcherGuardCore.KeepMax(0, 0, 3, 3));
    }

    [Fact]
    public void FiftyPercentSlack()
    {
        // **允许 50% 绕路余量**
        Assert.True(MoveArcherGuardCore.FiftyPercentSlack());
        Assert.True(MoveArcherGuardCore.KeepMaxIsAttemptBudget());
    }

    // ===================== 五、恶魔弓箭手规则集 =====================

    [Fact]
    public void DevilkingOnlyTwoRaces()
    {
        // **只打种族 108、或种族 154 且图像 156**
        Assert.True(MoveArcherGuardCore.DevilkingOnlyTwoRaces());
        Assert.True(MoveArcherGuardCore.DevilkingIsProperTarget(108, 0));
        Assert.True(MoveArcherGuardCore.DevilkingIsProperTarget(154, 156));
    }

    [Fact]
    public void AltRaceRequiresImage()
    {
        Assert.True(MoveArcherGuardCore.AltRaceRequiresImage());
        Assert.False(MoveArcherGuardCore.DevilkingIsProperTarget(154, 0));
    }

    [Fact]
    public void OwnRaceNotTarget()
    {
        Assert.True(MoveArcherGuardCore.OwnRaceNotTarget());
    }

    [Fact]
    public void DevilkingIgnoresAdminAndLastHiter()
    {
        Assert.True(MoveArcherGuardCore.DevilkingIgnoresAdminAndLastHiter());
    }

    [Fact]
    public void CommentedVetoIsImpossible()
    {
        // **注释块里的否决条件恒为假**
        Assert.True(MoveArcherGuardCore.CommentedVetoIsImpossible());
        Assert.True(MoveArcherGuardCore.CommentedVetoRequiresThreeRacesAtOnce());
        Assert.True(MoveArcherGuardCore.CommentedVetoHasFiveClauses());
        Assert.Equal(5, MoveArcherGuardCore.CommentedVetoFragment.Length);
    }

    [Fact]
    public void ShouldGhostTruthTable()
    {
        // **跟主人不在同一地图时变幽灵**
        Assert.True(MoveArcherGuardCore.ShouldGhostTruthTable());
        Assert.True(MoveArcherGuardCore.GhostsWhenMasterOnOtherMap());
        Assert.True(MoveArcherGuardCore.ExitsBeforeInherited());
    }

    [Fact]
    public void DevilkingAttacksTruckAndOwnKind()
    {
        // **不排除镖车与同类**
        Assert.True(MoveArcherGuardCore.DevilkingAttacksTruckAndOwnKind());
    }

    [Fact]
    public void DevilkingLoopFilterValues()
    {
        Assert.True(MoveArcherGuardCore.DevilkingHasOfflineFilter());
        Assert.False(MoveArcherGuardCore.DevilkingLoopFilter(0, true, true));
        Assert.True(MoveArcherGuardCore.DevilkingLoopFilter(0, true, false));
        Assert.True(MoveArcherGuardCore.TruckPassesDevilkingFilter());
    }

    [Fact]
    public void NoPatrolForDevilking()
    {
        Assert.True(MoveArcherGuardCore.NoPatrolForDevilking());
        Assert.True(MoveArcherGuardCore.DevilkingTurnsWhenNoTarget());
        Assert.True(MoveArcherGuardCore.ShouldTurnValues());
    }

    // ===================== 六、构造与析构 =====================

    [Fact]
    public void MoveArcherCreateTwelve()
    {
        Assert.True(MoveArcherGuardCore.MoveArcherCreateTwelve());
        Assert.Equal(-1, MoveArcherGuardCore.MoveArcherCreateInit().TargetX);
        Assert.Equal(12, MoveArcherGuardCore.MoveArcherCreateInit().ViewRange);
    }

    [Fact]
    public void OnlyTargetXInitialized()
    {
        // **只设 m_nTargetX，未设 m_nTargetY**
        Assert.True(MoveArcherGuardCore.OnlyTargetXInitialized());
        Assert.True(MoveArcherGuardCore.TargetYReadOnlyAfterTargetXSet());
    }

    [Fact]
    public void DestroyClearsPathFirst()
    {
        Assert.True(MoveArcherGuardCore.DestroyClearsPathFirst());
        Assert.True(MoveArcherGuardCore.PathClearedBeforeInherited());
    }

    [Fact]
    public void DevilkingRaceIsLiteral109()
    {
        // **种族是字面量 109**
        Assert.True(MoveArcherGuardCore.DevilkingRaceIsLiteral109());
        Assert.True(MoveArcherGuardCore.Race109HasNoConstant());
    }

    [Fact]
    public void DevilkingOmitsPkFields()
    {
        Assert.True(MoveArcherGuardCore.DevilkingOmitsPkFields());
        Assert.True(MoveArcherGuardCore.DevilkingCreateFive());
        Assert.Equal(109, MoveArcherGuardCore.DevilkingCreateInit().Race);
    }

    [Fact]
    public void AllThreeViewRangeTwelve()
    {
        Assert.True(MoveArcherGuardCore.AllThreeViewRangeTwelve());
        Assert.True(MoveArcherGuardCore.FirstSevenIdentical());
    }

    // ===================== 七、行走字段偏移 =====================

    [Fact]
    public void WalkFieldOffsetsContiguous()
    {
        // **五个字段严格 4 字节连续**
        Assert.True(MoveArcherGuardCore.WalkFieldOffsetsContiguous());
        Assert.True(MoveArcherGuardCore.WalkFieldOffsetValues());
        Assert.Equal((0x500, 0x504, 0x508, 0x50C, 0x510), MoveArcherGuardCore.WalkFieldOffsets());
    }

    [Fact]
    public void FiveWalkFields()
    {
        Assert.True(MoveArcherGuardCore.FiveWalkFields());
    }

    // ===================== 八、仿真 =====================

    [Fact]
    public void FirstPatrolAdvancesIndex()
    {
        // **Inc 先于读取，故首次巡逻取索引 1**
        Assert.True(MoveArcherGuardCore.FirstPatrolAdvancesIndex());
    }

    [Fact]
    public void PatrolIndexWraps()
    {
        Assert.True(MoveArcherGuardCore.PatrolIndexWraps());
    }

    [Fact]
    public void PatrolLockedDoesNotMove()
    {
        Assert.True(MoveArcherGuardCore.PatrolLockedDoesNotMove());
        Assert.True(MoveArcherGuardCore.PatrolNotDueDoesNotMove());
    }

    [Fact]
    public void PatrolResetsOnArrival()
    {
        Assert.True(MoveArcherGuardCore.PatrolResetsOnArrival());
        Assert.True(MoveArcherGuardCore.PatrolResetsOnExhaustion());
    }

    [Fact]
    public void SelectExcludesFourRaces()
    {
        // **四种族全被排除**
        Assert.True(MoveArcherGuardCore.SelectExcludesFourRaces());
    }

    [Fact]
    public void SelectExcludesOfflineWhenConfigured()
    {
        Assert.True(MoveArcherGuardCore.SelectExcludesOfflineWhenConfigured());
    }

    [Fact]
    public void SelectGuards()
    {
        var v = new (bool, bool, int, bool, bool, int)[]
        {
            (false, false, 80, false, true, 100),
        };

        Assert.Equal(-1, MoveArcherGuardCore.MoveArcherSelect(true, false, true, true, false, v));
        Assert.Equal(-1, MoveArcherGuardCore.MoveArcherSelect(false, true, true, true, false, v));
        Assert.Equal(-1, MoveArcherGuardCore.MoveArcherSelect(false, false, false, true, false, v));
        Assert.Equal(-1, MoveArcherGuardCore.MoveArcherSelect(false, false, true, false, false, v));
        Assert.Equal(0, MoveArcherGuardCore.MoveArcherSelect(false, false, true, true, false, v));
    }

    [Fact]
    public void SelectPicksNearest()
    {
        var v = new (bool, bool, int, bool, bool, int)[]
        {
            (false, false, 80, false, true, 800),
            (false, false, 80, false, true, 300),
        };

        Assert.Equal(1, MoveArcherGuardCore.MoveArcherSelect(false, false, true, true, false, v));
    }

    [Fact]
    public void SelectKeepsGhostsWhenPresent()
    {
        // **不可移动/幽灵门仍在**
        Assert.True(MoveArcherGuardCore.SelectKeepsGhostsWhenPresent());
    }
}
