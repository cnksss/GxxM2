using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J123：魔法事件列表消费侧（UsrEngn.pas 8444-8513 `ProcessEvents`、
/// 10802-10817 地图卸载清理、616-625 析构清理）1:1 测试。
/// </summary>
public sealed class MagicEventListCoreTests
{
    private static MagicEventListCore.MagicEventEntry Ev(
        bool formNPC = true, uint startTick = 0, uint dwTime = 1000,
        object? envir = null, params MagicEventListCore.MagicTarget[] targets)
    {
        var e = new MagicEventListCore.MagicEventEntry
        {
            FormNPC = formNPC,
            DwStartTick = startTick,
            DwTime = dwTime,
            Envir = envir,
            BaseObjectList2 = new List<MagicEventListCore.MagicTarget>(targets),
        };

        return e;
    }

    private static MagicEventListCore.MagicTarget T(
        bool death = false, bool ghost = false, bool holySeize = false, bool imprison = false)
        => MagicEventListCore.MagicTarget.Normal(death, ghost, holySeize, imprison);

    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.Equal(180_000u, MagicEventListCore.HardCapMs);
    }

    [Fact]
    public void ExpiryConditionOrderMatchesSource()
    {
        Assert.Equal(3, MagicEventListCore.ExpiryConditionOrder.Length);
        Assert.Contains("Count <= 0", MagicEventListCore.ExpiryConditionOrder[0]);
        Assert.Contains("dwTime", MagicEventListCore.ExpiryConditionOrder[1]);
        Assert.Contains("180000", MagicEventListCore.ExpiryConditionOrder[2]);
    }

    [Fact]
    public void CleanupStepsOrder()
    {
        Assert.Equal(5, MagicEventListCore.CleanupSteps.Length);
        Assert.StartsWith("BaseObjectList_2", MagicEventListCore.CleanupSteps[0]);
        Assert.Contains("Close", MagicEventListCore.CleanupSteps[1]);
        Assert.StartsWith("Events_2", MagicEventListCore.CleanupSteps[2]);
        Assert.StartsWith("Dispose", MagicEventListCore.CleanupSteps[3]);
        Assert.Contains("Delete", MagicEventListCore.CleanupSteps[4]);
    }

    // ===================== 两支判据 =====================

    [Fact]
    public void NpcBranchOnlyChecksImprison()
    {
        Assert.True(MagicEventListCore.ShouldRemoveNpcTarget(boImprison: false));
        Assert.False(MagicEventListCore.ShouldRemoveNpcTarget(boImprison: true));
    }

    [Fact]
    public void NpcBranchIgnoresDeathAndGhost()
    {
        // **死亡 + 仍在禁锢 → 不剔除**（NPC 支不看死亡）
        Assert.True(MagicEventListCore.NpcBranchIgnoresDeathAndGhost());

        var targets = new List<MagicEventListCore.MagicTarget>
        {
            T(death: true, ghost: true, imprison: true),
        };

        Assert.Equal(0, MagicEventListCore.PruneTargetsInPlace(targets, formNPC: true));
        Assert.Single(targets);
    }

    [Fact]
    public void NonNpcBranchChecksDeathGhostAndHold()
    {
        Assert.True(MagicEventListCore.NonNpcBranchChecksDeathGhostAndHold());
    }

    [Fact]
    public void NonNpcBranchRemovesOnDeath()
    {
        Assert.True(MagicEventListCore.ShouldRemoveNonNpcTarget(true, false, true, true));
    }

    [Fact]
    public void NonNpcBranchRemovesWhenNoHold()
    {
        // 既未擒获也未禁锢 → 剔除
        Assert.True(MagicEventListCore.ShouldRemoveNonNpcTarget(false, false, false, false));
    }

    [Fact]
    public void NonNpcBranchKeepsWhenHeld()
    {
        Assert.False(MagicEventListCore.ShouldRemoveNonNpcTarget(false, false, true, false));
        Assert.False(MagicEventListCore.ShouldRemoveNonNpcTarget(false, false, false, true));
    }

    [Fact]
    public void TwoBranchesUseDifferentPredicates()
    {
        Assert.True(MagicEventListCore.TwoBranchesUseDifferentPredicates());

        // 同一目标（死亡 + 禁锢）：NPC 支保留、非 NPC 支剔除
        Assert.False(MagicEventListCore.ShouldRemoveNpcTarget(true));
        Assert.True(MagicEventListCore.ShouldRemoveNonNpcTarget(true, false, false, true));
    }

    [Fact]
    public void ChangeStateEventsAllTakeNpcBranch()
    {
        Assert.True(MagicEventListCore.ChangeStateEventsAllTakeNpcBranch());
        Assert.Equal(3, MagicEventListCore.FormNpcAssignmentLines.Length);
        Assert.Equal(new[] { 23714, 23806, 23894 }, MagicEventListCore.FormNpcAssignmentLines);
    }

    [Fact]
    public void IsNpcEventFollowsFormNpc()
    {
        Assert.True(MagicEventListCore.IsNpcEvent(true));
        Assert.False(MagicEventListCore.IsNpcEvent(false));
    }

    // ===================== 剔除 =====================

    [Fact]
    public void PruneRemovesNonImprisonedInNpcBranch()
    {
        var targets = new List<MagicEventListCore.MagicTarget>
        {
            T(imprison: true),
            T(imprison: false),
            T(imprison: true),
        };

        int removed = MagicEventListCore.PruneTargetsInPlace(targets, formNPC: true);

        Assert.Equal(1, removed);
        Assert.Equal(2, targets.Count);
    }

    [Fact]
    public void PruneSkipsNullElements()
    {
        var targets = new List<MagicEventListCore.MagicTarget>
        {
            MagicEventListCore.MagicTarget.Nil,
            T(imprison: false),
        };

        int removed = MagicEventListCore.PruneTargetsInPlace(targets, formNPC: true);

        Assert.Equal(1, removed);           // 只删了 imprison=false 那个
        Assert.Single(targets);
        Assert.True(targets[0].IsNull);     // **nil 仍在**
    }

    [Fact]
    public void PruningIsReverseOrder()
    {
        Assert.True(MagicEventListCore.PruningIsReverseOrder());
    }

    [Fact]
    public void PruneKeepsOrderOfSurvivors()
    {
        var a = T(imprison: true);
        var b = T(imprison: true);

        var targets = new List<MagicEventListCore.MagicTarget>
        {
            a, T(imprison: false), b,
        };

        MagicEventListCore.PruneTargetsInPlace(targets, formNPC: true);

        Assert.Same(a, targets[0]);
        Assert.Same(b, targets[1]);
    }

    // ===================== tick_diff =====================

    [Fact]
    public void TickDiffNormalCase()
    {
        Assert.Equal(100u, MagicEventListCore.TickDiff(0, 100));
        Assert.Equal(0u, MagicEventListCore.TickDiff(50, 50));
    }

    [Fact]
    public void TickDiffUnderCountsByOneOnWrap()
    {
        Assert.True(MagicEventListCore.TickDiffUnderCountsByOneOnWrap());
    }

    // ===================== 到期判定 =====================

    [Fact]
    public void ExpiryNoneWhenFresh()
    {
        Assert.Equal(MagicEventListCore.ExpiryReason.None,
            MagicEventListCore.CheckExpiry(1, 0, 500, 1000));
    }

    [Fact]
    public void ExpiryOnEmptyTargetList()
    {
        Assert.Equal(MagicEventListCore.ExpiryReason.EmptyTargetList,
            MagicEventListCore.CheckExpiry(0, 0, 0, 999_000));
    }

    [Fact]
    public void ExpiryOnDwTime()
    {
        Assert.Equal(MagicEventListCore.ExpiryReason.DwTimeReached,
            MagicEventListCore.CheckExpiry(1, 0, 1001, 1000));
    }

    [Fact]
    public void DwTimeUsesStrictGreater()
    {
        Assert.True(MagicEventListCore.DwTimeUsesStrictGreater(100, 1000));
    }

    [Fact]
    public void ExpiryConditionOrderIsEmptyFirst()
    {
        // 目标为空**且**已超时 → 报 EmptyTargetList（条件①在前）
        Assert.Equal(MagicEventListCore.ExpiryReason.EmptyTargetList,
            MagicEventListCore.CheckExpiry(0, 0, 999_999, 1000));
    }

    [Fact]
    public void HardCapMsIs180Seconds()
    {
        // dwTime 极小（1）时条件②恒先成立，故改用**不影响条件②**的观测方式：
        // 让 dwTime 大于 180000，则 180000 时两条件皆假、180001 时条件③成立
        Assert.Equal(MagicEventListCore.ExpiryReason.None,
            MagicEventListCore.CheckExpiry(1, 0, 180_000, 500_000));

        Assert.Equal(MagicEventListCore.ExpiryReason.HardCap,
            MagicEventListCore.CheckExpiry(1, 0, 180_001, 500_000));
    }

    [Fact]
    public void HardCapFiresOnlyWhenDwTimeNotReached()
    {
        // **条件②写在③之前**：dwTime < 经过时间时先命中②，故报 DwTimeReached 而非 HardCap
        Assert.Equal(MagicEventListCore.ExpiryReason.DwTimeReached,
            MagicEventListCore.CheckExpiry(1, 0, 180_001, 180_000));

        // dwTime 很大 → 条件②不成立 → 落到条件③
        Assert.Equal(MagicEventListCore.ExpiryReason.HardCap,
            MagicEventListCore.CheckExpiry(1, 0, 180_001, 500_000));
    }

    [Fact]
    public void HardCapOverridesLongerDwTime()
    {
        // dwTime 很大（999 秒），但 180 秒后仍被清理（此处由条件③触发）
        var reason = MagicEventListCore.CheckExpiry(1, 0, 200_000, 999_000);

        Assert.Equal(MagicEventListCore.ExpiryReason.HardCap, reason);
    }

    [Fact]
    public void HardCapOnlyReachableWhenDwTimeAtLeastCap()
    {
        // 硬上限**只在条件② 不成立**（即 dwTime >= 经过时间）时才可能成为归因
        Assert.True(MagicEventListCore.HardCapOnlyReachableWhenDwTimeAtLeastCap());
    }

    [Fact]
    public void ExpiryCheckComesAfterTargetPruning()
    {
        Assert.True(MagicEventListCore.ExpiryCheckComesAfterTargetPruning());
    }

    [Fact]
    public void EmptyTargetListExpiresImmediately()
    {
        Assert.True(MagicEventListCore.EmptyTargetListExpiresImmediately());
    }

    [Fact]
    public void NilElementPreventsEarlyCleanup()
    {
        Assert.True(MagicEventListCore.NilElementPreventsEarlyCleanup());
    }

    // ===================== 清理顺序 =====================

    [Fact]
    public void CleanupOrderIsListThenCloseThenFree()
    {
        Assert.True(MagicEventListCore.CleanupOrderIsListThenCloseThenFree());
    }

    [Fact]
    public void EventsClosedInReverseOrder()
    {
        Assert.True(MagicEventListCore.EventsClosedInReverseOrder());
    }

    [Fact]
    public void CloseEventsSkipsNull()
    {
        var events = new List<object?> { new object(), null, new object() };

        Assert.Equal(2, MagicEventListCore.CloseEvents(events));
    }

    [Fact]
    public void EventsTwoIndexerAndItemsAreEquivalent()
    {
        Assert.True(MagicEventListCore.EventsTwoIndexerAndItemsAreEquivalent());
    }

    [Fact]
    public void NullBaseObjectListLeaksForever()
    {
        Assert.True(MagicEventListCore.NullBaseObjectListLeaksForever());
    }

    [Fact]
    public void IsProcessableGuard()
    {
        Assert.True(MagicEventListCore.IsProcessable(false, false));
        Assert.False(MagicEventListCore.IsProcessable(true, false));   // event nil
        Assert.False(MagicEventListCore.IsProcessable(false, true));   // list nil
    }

    [Fact]
    public void BaseObjectListAlwaysCreatedByChangeState()
    {
        Assert.True(MagicEventListCore.BaseObjectListAlwaysCreatedByChangeState());
    }

    // ===================== 8457 防御性 Break =====================

    [Fact]
    public void EmptyListBreakIsDefensiveRedundant()
    {
        Assert.True(MagicEventListCore.EmptyListBreakIsDefensiveRedundant());
        Assert.True(MagicEventListCore.BreakIsRedundantBecauseIndexDerivedFromCount());
        Assert.True(MagicEventListCore.BreakGuardsAgainstConcurrentShrink());
    }

    // ===================== 整轮处理 =====================

    [Fact]
    public void RoundClearsExpiredEvent()
    {
        var list = new List<MagicEventListCore.MagicEventEntry>
        {
            Ev(formNPC: true, startTick: 0, dwTime: 1000, targets: T(imprison: true)),
        };

        var round = MagicEventListCore.RunProcessEvents(list, now: 1001);

        Assert.Equal(1, round.Cleared);
        Assert.Empty(list);
        Assert.Equal(MagicEventListCore.ExpiryReason.DwTimeReached, round.Reasons[0]);
    }

    [Fact]
    public void RoundKeepsFreshEvent()
    {
        var list = new List<MagicEventListCore.MagicEventEntry>
        {
            Ev(formNPC: true, startTick: 0, dwTime: 1000, targets: T(imprison: true)),
        };

        var round = MagicEventListCore.RunProcessEvents(list, now: 500);

        Assert.Equal(0, round.Cleared);
        Assert.Single(list);
    }

    [Fact]
    public void RoundPrunesTargetsThenChecksExpiry()
    {
        // 唯一目标不再被禁锢 → 被剔除 → 列表空 → **同一轮清理**
        var list = new List<MagicEventListCore.MagicEventEntry>
        {
            Ev(formNPC: true, startTick: 0, dwTime: 999_000, targets: T(imprison: false)),
        };

        var round = MagicEventListCore.RunProcessEvents(list, now: 1);

        Assert.Equal(1, round.PrunedTargets);
        Assert.Equal(1, round.Cleared);
        Assert.Equal(MagicEventListCore.ExpiryReason.EmptyTargetList, round.Reasons[0]);
    }

    [Fact]
    public void RoundSkipsNullBaseObjectList()
    {
        var ev = Ev(formNPC: true, startTick: 0, dwTime: 1000, targets: T(imprison: true));
        ev.BaseObjectList2IsNull = true;

        var list = new List<MagicEventListCore.MagicEventEntry> { ev };

        var round = MagicEventListCore.RunProcessEvents(list, now: 999_999);

        Assert.Equal(0, round.Cleared);
        Assert.Single(list);   // **永久留下**
    }

    [Fact]
    public void RoundHandlesMultipleEvents()
    {
        var list = new List<MagicEventListCore.MagicEventEntry>
        {
            Ev(startTick: 0, dwTime: 1000, targets: T(imprison: true)),      // 到期
            Ev(startTick: 0, dwTime: 999_000, targets: T(imprison: true)),   // 未到期
            Ev(startTick: 0, dwTime: 500, targets: T(imprison: true)),       // 到期
        };

        var round = MagicEventListCore.RunProcessEvents(list, now: 1001);

        Assert.Equal(2, round.Cleared);
        Assert.Single(list);
        Assert.Equal(999_000u, list[0].DwTime);   // 存活的是 dwTime 最大的那个
        Assert.Equal(2, round.Reasons.Count);
        Assert.All(round.Reasons, r => Assert.Equal(MagicEventListCore.ExpiryReason.DwTimeReached, r));
    }

    [Fact]
    public void RoundOnEmptyList()
    {
        var list = new List<MagicEventListCore.MagicEventEntry>();

        var round = MagicEventListCore.RunProcessEvents(list, now: 1000);

        Assert.Equal(0, round.Cleared);
        Assert.Equal(0, round.Remaining);
    }

    [Fact]
    public void RoundPrunesAcrossMultipleEvents()
    {
        var list = new List<MagicEventListCore.MagicEventEntry>
        {
            Ev(startTick: 0, dwTime: 999_000, targets: new[] { T(imprison: false), T(imprison: true) }),
            Ev(startTick: 0, dwTime: 999_000, targets: T(imprison: false)),
        };

        var round = MagicEventListCore.RunProcessEvents(list, now: 1);

        Assert.Equal(2, round.PrunedTargets);   // 各剔除 1
        Assert.Equal(1, round.Cleared);          // 第二个列表变空
    }

    [Fact]
    public void NonNpcEventRemovedOnDeathSameRound()
    {
        // 非 NPC 支：目标死亡 → 剔除 → 列表空 → 清理
        var list = new List<MagicEventListCore.MagicEventEntry>
        {
            Ev(formNPC: false, startTick: 0, dwTime: 999_000, targets: T(death: true)),
        };

        var round = MagicEventListCore.RunProcessEvents(list, now: 1);

        Assert.Equal(1, round.Cleared);
        Assert.Equal(MagicEventListCore.ExpiryReason.EmptyTargetList, round.Reasons[0]);
    }

    [Fact]
    public void NpcEventSurvivesTargetDeath()
    {
        // NPC 支：目标死亡但仍在禁锢 → 不剔除 → 不清理
        var list = new List<MagicEventListCore.MagicEventEntry>
        {
            Ev(formNPC: true, startTick: 0, dwTime: 999_000, targets: T(death: true, imprison: true)),
        };

        var round = MagicEventListCore.RunProcessEvents(list, now: 1);

        Assert.Equal(0, round.Cleared);
        Assert.Single(list);
    }

    // ===================== 地图卸载清理 =====================

    [Fact]
    public void UnloadStepsOrder()
    {
        Assert.Equal(4, MagicEventListCore.UnloadSteps.Length);
        Assert.StartsWith("BaseObjectList_2", MagicEventListCore.UnloadSteps[0]);
        Assert.StartsWith("Events_2", MagicEventListCore.UnloadSteps[1]);
        Assert.Contains("Delete", MagicEventListCore.UnloadSteps[2]);
        Assert.StartsWith("Dispose", MagicEventListCore.UnloadSteps[3]);
    }

    [Fact]
    public void UnloadSkipsClose()
    {
        Assert.True(MagicEventListCore.UnloadSkipsClose());
    }

    [Fact]
    public void UnloadOrderIsDeleteThenDispose()
    {
        Assert.True(MagicEventListCore.UnloadOrderIsDeleteThenDispose());
    }

    [Fact]
    public void ProcessOrderIsDisposeThenDelete()
    {
        Assert.True(MagicEventListCore.ProcessOrderIsDisposeThenDelete());
    }

    [Fact]
    public void TwoCleanupOrdersAreReversed()
    {
        Assert.True(MagicEventListCore.TwoCleanupOrdersAreReversed());
    }

    [Fact]
    public void UnloadHasNoExpiryCheck()
    {
        Assert.True(MagicEventListCore.UnloadHasNoExpiryCheck());
    }

    [Fact]
    public void UnloadMatchesByEnvir()
    {
        Assert.True(MagicEventListCore.UnloadMatchesByEnvir());

        var envirA = new object();
        var envirB = new object();

        var list = new List<MagicEventListCore.MagicEventEntry>
        {
            Ev(envir: envirA), Ev(envir: envirB), Ev(envir: envirA),
        };

        int removed = MagicEventListCore.UnloadCleanup(list, envirA);

        Assert.Equal(2, removed);
        Assert.Single(list);
        Assert.Same(envirB, list[0].Envir);
    }

    [Fact]
    public void UnloadCleanupOnNoMatch()
    {
        var list = new List<MagicEventListCore.MagicEventEntry> { Ev(envir: new object()) };

        Assert.Equal(0, MagicEventListCore.UnloadCleanup(list, new object()));
        Assert.Single(list);
    }

    [Fact]
    public void UnloadFixCommentHasTimestamp()
    {
        Assert.True(MagicEventListCore.UnloadFixCommentHasTimestamp());
        Assert.Contains("ProcessEvents", MagicEventListCore.UnloadFixComment);
        Assert.Contains("困魔咒", MagicEventListCore.UnloadFixComment);
    }

    // ===================== 析构清理 =====================

    [Fact]
    public void DestructorIteratesForward()
    {
        Assert.True(MagicEventListCore.DestructorIteratesForward());
    }

    [Fact]
    public void DestructorDoesNotFreeInnerLists()
    {
        Assert.True(MagicEventListCore.DestructorDoesNotFreeInnerLists());
        Assert.True(MagicEventListCore.DestructorFreesOuterListOnly());
    }
}
