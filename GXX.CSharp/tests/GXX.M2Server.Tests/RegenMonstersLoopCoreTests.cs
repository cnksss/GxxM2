using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J116：刷怪主循环 `ProcessRegenMonsters`（UsrEngn.pas 3831-3972，含 `CheckGVar` 3832-3849）
/// 1:1 测试。
/// </summary>
public sealed class RegenMonstersLoopCoreTests
{
    private static readonly int[] Gvals = { 0, 5, 10, 100, -3 };

    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.Equal(200, RegenMonstersLoopCore.DefaultRegenMonstersTime);
        Assert.Equal(10, RegenMonstersLoopCore.DefaultMonGenRate);
        Assert.Equal(10, RegenMonstersLoopCore.MonGenRateFallback);
        Assert.Equal(0, RegenMonstersLoopCore.NeverSpawnedTick);
        Assert.True(RegenMonstersLoopCore.InitialBoRegened);
    }

    // ===================== 节流（3883-3888） =====================

    [Fact]
    public void StopRunAbortsEntirely()
    {
        Assert.True(RegenMonstersLoopCore.ShouldAbortEntirely(true));
        Assert.False(RegenMonstersLoopCore.ShouldAbortEntirely(false));
    }

    [Fact]
    public void RegenBlockRequiresBothConfigFlagsFalse()
    {
        // **两个否定项是"与"**
        Assert.True(RegenMonstersLoopCore.ShouldEnterRegenBlock(false, false, 1000, 0, 200));
        Assert.False(RegenMonstersLoopCore.ShouldEnterRegenBlock(true, false, 1000, 0, 200));
        Assert.False(RegenMonstersLoopCore.ShouldEnterRegenBlock(false, true, 1000, 0, 200));
        Assert.False(RegenMonstersLoopCore.ShouldEnterRegenBlock(true, true, 1000, 0, 200));
    }

    [Fact]
    public void RegenBlockTimeComparisonIsStrict()
    {
        // **严格 >**：恰好 200ms 不进
        Assert.False(RegenMonstersLoopCore.ShouldEnterRegenBlock(false, false, 200, 0, 200));
        Assert.True(RegenMonstersLoopCore.ShouldEnterRegenBlock(false, false, 201, 0, 200));
    }

    [Fact]
    public void RegenTickRenewedToNow()
    {
        Assert.Equal(1234, RegenMonstersLoopCore.RenewRegenTick(1234));
    }

    [Fact]
    public void RegenTickRenewalPostponesNextRound()
    {
        // 3888 **先更新再判断**：本轮更新后，下一轮必须重新等满 200ms
        int tick = 0;
        Assert.True(RegenMonstersLoopCore.ShouldEnterRegenBlock(false, false, 1000, tick, 200));
        tick = RegenMonstersLoopCore.RenewRegenTick(1000);

        Assert.False(RegenMonstersLoopCore.ShouldEnterRegenBlock(false, false, 1100, tick, 200));
        Assert.True(RegenMonstersLoopCore.ShouldEnterRegenBlock(false, false, 1201, tick, 200));
    }

    // ===================== CheckGVar（3832-3849） =====================

    [Fact]
    public void GVarLess()
    {
        Assert.True(RegenMonstersLoopCore.CheckGVar(MonGenParseCore.CompareType.ctLess, Gvals, 1, 10));
        Assert.False(RegenMonstersLoopCore.CheckGVar(MonGenParseCore.CompareType.ctLess, Gvals, 2, 10));
    }

    [Fact]
    public void GVarEqual()
    {
        Assert.True(RegenMonstersLoopCore.CheckGVar(MonGenParseCore.CompareType.ctEqual, Gvals, 2, 10));
        Assert.False(RegenMonstersLoopCore.CheckGVar(MonGenParseCore.CompareType.ctEqual, Gvals, 1, 10));
    }

    [Fact]
    public void GVarGreater()
    {
        Assert.True(RegenMonstersLoopCore.CheckGVar(MonGenParseCore.CompareType.ctGreater, Gvals, 3, 10));
        Assert.False(RegenMonstersLoopCore.CheckGVar(MonGenParseCore.CompareType.ctGreater, Gvals, 1, 10));
    }

    [Fact]
    public void GVarLessEqualBoundary()
    {
        Assert.True(RegenMonstersLoopCore.CheckGVar(MonGenParseCore.CompareType.ctLessEqual, Gvals, 2, 10));
        Assert.True(RegenMonstersLoopCore.CheckGVar(MonGenParseCore.CompareType.ctLessEqual, Gvals, 1, 10));
        Assert.False(RegenMonstersLoopCore.CheckGVar(MonGenParseCore.CompareType.ctLessEqual, Gvals, 3, 10));
    }

    [Fact]
    public void GVarGreaterEqualBoundary()
    {
        Assert.True(RegenMonstersLoopCore.CheckGVar(MonGenParseCore.CompareType.ctGreaterEqual, Gvals, 2, 10));
        Assert.True(RegenMonstersLoopCore.CheckGVar(MonGenParseCore.CompareType.ctGreaterEqual, Gvals, 3, 10));
        Assert.False(RegenMonstersLoopCore.CheckGVar(MonGenParseCore.CompareType.ctGreaterEqual, Gvals, 1, 10));
    }

    [Fact]
    public void GVarNotEqual()
    {
        Assert.True(RegenMonstersLoopCore.CheckGVar(MonGenParseCore.CompareType.ctNotEqual, Gvals, 1, 10));
        Assert.False(RegenMonstersLoopCore.CheckGVar(MonGenParseCore.CompareType.ctNotEqual, Gvals, 2, 10));
    }

    [Fact]
    public void NegativeValuesCompared()
    {
        Assert.True(RegenMonstersLoopCore.CheckGVar(MonGenParseCore.CompareType.ctLess, Gvals, 4, 0));
    }

    [Fact]
    public void CtFailPassesCheck()
    {
        // **无 else 分支 + Result 初值 True** → 比较符写错反而永远放行
        Assert.True(RegenMonstersLoopCore.CtFailPassesCheck(MonGenParseCore.CompareType.ctFail));
    }

    [Fact]
    public void CtFailPassesRegardlessOfValues()
    {
        // 任何取值下 ctFail 都放行
        foreach (int v in new[] { 0, 5, -3, 100 })
        {
            Assert.True(RegenMonstersLoopCore.CheckGVar(
                MonGenParseCore.CompareType.ctFail, new[] { v }, 0, 999));
        }
    }

    [Fact]
    public void AllSixCompareTypesHandled()
    {
        // 六个比较符都要有分支（ctFail 走 fallback）
        var types = new[]
        {
            MonGenParseCore.CompareType.ctLess,
            MonGenParseCore.CompareType.ctEqual,
            MonGenParseCore.CompareType.ctGreater,
            MonGenParseCore.CompareType.ctLessEqual,
            MonGenParseCore.CompareType.ctGreaterEqual,
            MonGenParseCore.CompareType.ctNotEqual,
        };

        foreach (var ct in types)
            RegenMonstersLoopCore.CheckGVar(ct, Gvals, 2, 10);

        Assert.Equal(6, types.Length);
    }

    // ===================== 优先刷怪游标（3890-3912） =====================

    [Fact]
    public void PriorityCursorTakesAndAdvances()
    {
        var rec = new RegenMonstersLoopCore.PriorityRecord { Index = 10, Count = 5 };
        var r = RegenMonstersLoopCore.StepPriorityCursor(rec, 0, 100);

        Assert.Equal(10, r.SortListIndex);
        Assert.False(r.RemovedRecord);
        Assert.Equal(1, r.NewCursor);
    }

    [Fact]
    public void PriorityCursorRemovesAfterLastElement()
    {
        // 3905-3909：**取到最后一个元素的那一轮仍会使用它**，取完才删
        var rec = new RegenMonstersLoopCore.PriorityRecord { Index = 10, Count = 3 };
        var r = RegenMonstersLoopCore.StepPriorityCursor(rec, 2, 100);

        Assert.Equal(12, r.SortListIndex);   // **仍然取到**
        Assert.True(r.RemovedRecord);
        Assert.True(r.CursorReset);
        Assert.Equal(0, r.NewCursor);
    }

    [Fact]
    public void PriorityCursorNotRemovedBeforeLast()
    {
        var rec = new RegenMonstersLoopCore.PriorityRecord { Index = 10, Count = 3 };
        var r = RegenMonstersLoopCore.StepPriorityCursor(rec, 1, 100);

        Assert.Equal(11, r.SortListIndex);
        Assert.False(r.RemovedRecord);
        Assert.Equal(2, r.NewCursor);
    }

    [Fact]
    public void PriorityCursorOutOfRangeRemovesWithoutTaking()
    {
        // 3896-3900：`record.Index + cursor >= m_SortMapMonGenList.Count` → Dispose + 删除，**本轮不取**
        // 比较的是 `Index + cursor` 与表长，故必须让二者之和达到表长
        var rec = new RegenMonstersLoopCore.PriorityRecord { Index = 98, Count = 5 };
        var r = RegenMonstersLoopCore.StepPriorityCursor(rec, 2, 100);   // 98 + 2 = 100 >= 100

        Assert.Null(r.SortListIndex);
        Assert.True(r.RemovedRecord);
        Assert.Equal(0, r.NewCursor);
    }

    [Fact]
    public void PriorityCursorInRangeNearEndStillTakes()
    {
        // 与上条对照：98 + 1 = 99 < 100 → 正常取用
        var rec = new RegenMonstersLoopCore.PriorityRecord { Index = 98, Count = 5 };
        var r = RegenMonstersLoopCore.StepPriorityCursor(rec, 1, 100);

        Assert.Equal(99, r.SortListIndex);
        Assert.False(r.RemovedRecord);
    }

    [Fact]
    public void PriorityCursorResetsWhenCursorExceedsCount()
    {
        // 3893-3894：游标超限先归零，再正常取用
        var rec = new RegenMonstersLoopCore.PriorityRecord { Index = 10, Count = 3 };
        var r = RegenMonstersLoopCore.StepPriorityCursor(rec, 99, 100);

        Assert.Equal(10, r.SortListIndex);   // 归零后取 Index+0
        Assert.Equal(1, r.NewCursor);
    }

    [Fact]
    public void PriorityCursorExactlyAtCountResets()
    {
        // 边界：cursor == Count 也归零（`>=` 而非 `>`）
        var rec = new RegenMonstersLoopCore.PriorityRecord { Index = 10, Count = 3 };
        var r = RegenMonstersLoopCore.StepPriorityCursor(rec, 3, 100);

        Assert.Equal(10, r.SortListIndex);
    }

    [Fact]
    public void PriorityIndexExactlyAtSortCountRemoves()
    {
        // 边界：index == sortListCount 即越界（`>=`）
        var rec = new RegenMonstersLoopCore.PriorityRecord { Index = 100, Count = 5 };
        var r = RegenMonstersLoopCore.StepPriorityCursor(rec, 0, 100);

        Assert.Null(r.SortListIndex);
        Assert.True(r.RemovedRecord);
    }

    [Fact]
    public void PrioritySingleElementRemovesImmediately()
    {
        var rec = new RegenMonstersLoopCore.PriorityRecord { Index = 5, Count = 1 };
        var r = RegenMonstersLoopCore.StepPriorityCursor(rec, 0, 100);

        Assert.Equal(5, r.SortListIndex);
        Assert.True(r.RemovedRecord);
        Assert.Equal(0, r.NewCursor);
    }

    [Fact]
    public void PriorityNeverTakesMoreThanOne()
    {
        var rec = new RegenMonstersLoopCore.PriorityRecord { Index = 0, Count = 10 };
        var r = RegenMonstersLoopCore.StepPriorityCursor(rec, 0, 100);

        Assert.NotNull(r.SortListIndex);
        Assert.Equal(1, r.NewCursor);
    }

    [Fact]
    public void ShouldUsePriorityOnlyWhenNonEmpty()
    {
        Assert.True(RegenMonstersLoopCore.ShouldUsePriority(true));
        Assert.False(RegenMonstersLoopCore.ShouldUsePriority(false));
    }

    // ===================== 常规游标（3920-3931） =====================

    [Fact]
    public void NormalCursorTakesAndAdvances()
    {
        var r = RegenMonstersLoopCore.StepNormalCursor(0, 5);
        Assert.Equal(0, r.Index);
        Assert.Equal(1, r.NewCursor);
    }

    [Fact]
    public void NormalCursorWrapsAtEnd()
    {
        var r = RegenMonstersLoopCore.StepNormalCursor(4, 5);
        Assert.Equal(4, r.Index);
        Assert.Equal(0, r.NewCursor);
    }

    [Fact]
    public void NormalCursorSingleElementTakesThenWraps()
    {
        // ① 成立（0 < 1）取到；② 不成立（0 < 0 假）→ 归零
        var r = RegenMonstersLoopCore.StepNormalCursor(0, 1);
        Assert.Equal(0, r.Index);
        Assert.Equal(0, r.NewCursor);
    }

    [Fact]
    public void NormalCursorEmptyListTakesNothing()
    {
        // Count = 0：① 不成立 → Index 为 null；② 也不成立 → 归零
        var r = RegenMonstersLoopCore.StepNormalCursor(0, 0);
        Assert.Null(r.Index);
        Assert.Equal(0, r.NewCursor);
    }

    [Fact]
    public void NormalCursorTwoIndependentConditions()
    {
        // ①② 条件不同：Count-1 与 Count
        int count = 3;
        var a = RegenMonstersLoopCore.StepNormalCursor(2, count);
        Assert.Equal(2, a.Index);          // ① 2 < 3 成立
        Assert.Equal(0, a.NewCursor);      // ② 2 < 2 不成立 → 归零

        var b = RegenMonstersLoopCore.StepNormalCursor(3, count);
        Assert.Null(b.Index);              // ① 3 < 3 不成立
        Assert.Equal(0, b.NewCursor);
    }

    [Fact]
    public void NormalCursorCyclesThroughAllIndices()
    {
        int cursor = 0;
        var seen = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            var r = RegenMonstersLoopCore.StepNormalCursor(cursor, 3);
            seen.Add(r.Index!.Value);
            cursor = r.NewCursor;
        }

        Assert.Equal(new[] { 0, 1, 2, 0 }, seen);
    }

    [Fact]
    public void FallbackToNormalOnlyWhenPriorityMissed()
    {
        Assert.True(RegenMonstersLoopCore.FallbackToNormalOnlyWhenPriorityMissed(true));
        Assert.False(RegenMonstersLoopCore.FallbackToNormalOnlyWhenPriorityMissed(false));
    }

    // ===================== 刷怪条件（3939-3940） =====================

    [Fact]
    public void AllFiveConditionsRequired()
    {
        Assert.True(RegenMonstersLoopCore.ShouldProcessMonGen(false, false, "鸡", true, true));

        Assert.False(RegenMonstersLoopCore.ShouldProcessMonGen(true, false, "鸡", true, true));   // nil
        Assert.False(RegenMonstersLoopCore.ShouldProcessMonGen(false, true, "鸡", true, true));   // boFB
        Assert.False(RegenMonstersLoopCore.ShouldProcessMonGen(false, false, "", true, true));    // 空名
        Assert.False(RegenMonstersLoopCore.ShouldProcessMonGen(false, false, "鸡", false, true)); // 智能刷怪关
        Assert.False(RegenMonstersLoopCore.ShouldProcessMonGen(false, false, "鸡", true, false)); // GVar 不过
    }

    [Fact]
    public void FbTemplatesExcludedFromMainLoop()
    {
        // **`not boFB`**：副本模板由 J110/J112 那条线单独处理
        Assert.True(RegenMonstersLoopCore.ExcludesFbTemplates(false));
        Assert.False(RegenMonstersLoopCore.ExcludesFbTemplates(true));
    }

    // ===================== 时间窗（3944） =====================

    [Fact]
    public void NeverSpawnedAlwaysElapsed()
    {
        // `dwStartTick = 0` **无条件通过**
        Assert.True(RegenMonstersLoopCore.IsZenTimeElapsed(0, 0, 999_999));
        Assert.True(RegenMonstersLoopCore.IsZenTimeElapsed(0, 12345, 999_999));
    }

    [Fact]
    public void ZenTimeComparisonIsStrict()
    {
        Assert.False(RegenMonstersLoopCore.IsZenTimeElapsed(1000, 1000 + 60_000, 60_000));
        Assert.True(RegenMonstersLoopCore.IsZenTimeElapsed(1000, 1000 + 60_001, 60_000));
    }

    [Fact]
    public void ZenTimeNegativeAlwaysElapsed()
    {
        // J108 的 dwZenTime 默认 -1 → 只要 startTick 非 0 也立即通过
        Assert.True(RegenMonstersLoopCore.IsZenTimeElapsed(1000, 1000, -1));
    }

    // ===================== 数量计算（3946-3953） =====================

    [Fact]
    public void GenModCountAtRateTen()
    {
        // 倍率 1.0（nMonGenRate/10 = 1）：目标等于配置数量
        Assert.Equal(10, RegenMonstersLoopCore.ComputeGenModCount(10, 10));
    }

    [Fact]
    public void GenModCountAtRateTwenty()
    {
        // **nMonGenRate 是除数**：20 → 除以 2 → **减半**（不是加倍）
        Assert.Equal(5, RegenMonstersLoopCore.ComputeGenModCount(10, 20));
    }

    [Fact]
    public void GenModCountAtRateFive()
    {
        // 5 → 除以 0.5 → **加倍**
        Assert.Equal(20, RegenMonstersLoopCore.ComputeGenModCount(10, 5));
    }

    [Fact]
    public void HigherRateYieldsFewerMonsters()
    {
        // 命名陷阱："刷怪倍数"数值越大反而刷得越少（它是分母）
        int atTen = RegenMonstersLoopCore.ComputeGenModCount(100, 10);
        int atTwenty = RegenMonstersLoopCore.ComputeGenModCount(100, 20);

        Assert.True(atTwenty < atTen);
        Assert.Equal(100, atTen);
        Assert.Equal(50, atTwenty);
    }

    [Fact]
    public void GenModCountOuterMaxKeepsAtLeastOne()
    {
        // **外层 _MAX(1, ...)**：倍率极大时结果仍为 1
        Assert.Equal(1, RegenMonstersLoopCore.ComputeGenModCount(1, 1000));
        Assert.Equal(1, RegenMonstersLoopCore.ComputeGenModCount(10, 100_000));
    }

    [Fact]
    public void GenModCountInnerMaxKeepsAtLeastOne()
    {
        // **内层 _MAX(1, nCount)**：配置 0 也按 1 计
        Assert.Equal(1, RegenMonstersLoopCore.ComputeGenModCount(0, 10));
        Assert.Equal(1, RegenMonstersLoopCore.ComputeGenModCount(-5, 10));
    }

    [Fact]
    public void GenModCountRateZeroFallsBack()
    {
        // 3948-3949：<= 0 时按 10 处理
        Assert.Equal(10, RegenMonstersLoopCore.ComputeGenModCount(10, 0));
        Assert.Equal(10, RegenMonstersLoopCore.ComputeGenModCount(10, -3));
    }

    [Fact]
    public void ApplyRateFallbackRewritesOnlyWhenNonPositive()
    {
        Assert.Equal(10, RegenMonstersLoopCore.ApplyRateFallback(0));
        Assert.Equal(10, RegenMonstersLoopCore.ApplyRateFallback(-1));
        Assert.Equal(7, RegenMonstersLoopCore.ApplyRateFallback(7));
    }

    [Fact]
    public void MakeMonsterCountSubtractsExisting()
    {
        Assert.Equal(7, RegenMonstersLoopCore.ComputeMakeMonsterCount(10, 3));
    }

    [Fact]
    public void MakeMonsterCountClampedAtZero()
    {
        // **怪过多时不刷、也不杀**
        Assert.Equal(0, RegenMonstersLoopCore.ComputeMakeMonsterCount(10, 10));
        Assert.Equal(0, RegenMonstersLoopCore.ComputeMakeMonsterCount(10, 999));
    }

    [Fact]
    public void ShouldCallRegenOnlyWhenPositive()
    {
        Assert.True(RegenMonstersLoopCore.ShouldCallRegen(1));
        Assert.False(RegenMonstersLoopCore.ShouldCallRegen(0));
        Assert.False(RegenMonstersLoopCore.ShouldCallRegen(-5));
    }

    // ===================== 最反直觉：没刷怪也续期 =====================

    [Fact]
    public void NoSpawnStillRefreshesStartTick()
    {
        // **boRegened 初值 True**：nMakeMonsterCount <= 0 时不调用 RegenMonsters，
        // boRegened 保持 True → 3960 仍刷新 dwStartTick
        Assert.True(RegenMonstersLoopCore.InitialBoRegened);
        Assert.True(RegenMonstersLoopCore.ShouldRefreshStartTick(RegenMonstersLoopCore.InitialBoRegened));
    }

    [Fact]
    public void OnlyExplicitRegenFailureSkipsRefresh()
    {
        // 唯一不刷新路径：调用了 RegenMonsters 且返回 False
        Assert.False(RegenMonstersLoopCore.ShouldRefreshStartTick(false));
    }

    [Fact]
    public void NoSpawnPostponesNextSpawn()
    {
        // 后果：本轮无需刷怪 → 计时器重置 → 下一次要再等一个 dwZenTime
        int startTick = 0;
        int now = 10_000;
        int zenTime = 60_000;

        // 首次（startTick=0）无条件通过
        Assert.True(RegenMonstersLoopCore.IsZenTimeElapsed(startTick, now, zenTime));

        // 本轮 makeCount = 0 → 不刷，但 boRegened 仍 True → 重置
        int makeCount = RegenMonstersLoopCore.ComputeMakeMonsterCount(10, 10);
        Assert.Equal(0, makeCount);
        Assert.False(RegenMonstersLoopCore.ShouldCallRegen(makeCount));

        bool boRegened = RegenMonstersLoopCore.InitialBoRegened;
        Assert.True(RegenMonstersLoopCore.ShouldRefreshStartTick(boRegened));
        startTick = now;

        // 下一次必须等满 zenTime
        Assert.False(RegenMonstersLoopCore.IsZenTimeElapsed(startTick, now + 59_999, zenTime));
        Assert.True(RegenMonstersLoopCore.IsZenTimeElapsed(startTick, now + 60_001, zenTime));
    }

    // ===================== 统计（3966-3970） =====================

    [Fact]
    public void TimingStatsTrackPeaks()
    {
        var stats = new ProcessMonstersEnvelopeCore.ProcTimeStats();

        RegenMonstersLoopCore.UpdateTimingStats(stats, 10);
        RegenMonstersLoopCore.UpdateTimingStats(stats, 30);
        RegenMonstersLoopCore.UpdateTimingStats(stats, 20);

        Assert.Equal(30, stats.MonTimeMin);   // **名为 Min，实为峰值**
        Assert.Equal(30, stats.MonTimeMax);
    }

    [Fact]
    public void TimingStatsNeverDecrease()
    {
        var stats = new ProcessMonstersEnvelopeCore.ProcTimeStats();

        RegenMonstersLoopCore.UpdateTimingStats(stats, 50);
        int min = stats.MonTimeMin;
        RegenMonstersLoopCore.UpdateTimingStats(stats, 1);

        Assert.Equal(min, stats.MonTimeMin);   // 只增不减
    }

    [Fact]
    public void StatsNotUpdatedWhenStopped()
    {
        // 3883-3884：`g_boStopRun` 时直接 Exit，连统计都不更新
        Assert.False(RegenMonstersLoopCore.UpdatesStatsWhenStopped());
    }

    // ===================== 监控字符串（3963） =====================

    [Fact]
    public void MonGenInfoFormat()
    {
        Assert.Equal("鸡,3/100", RegenMonstersLoopCore.FormatMonGenInfo("鸡", 3, 100));
    }

    [Fact]
    public void StatusUsesNormalCursorEvenForPriority()
    {
        // 3963 用 m_nCurrMonGen，**不是**本轮实际取用的下标
        Assert.True(RegenMonstersLoopCore.StatusUsesNormalCursorEvenForPriority());
    }

    [Fact]
    public void StatusStrDiffersFromActualIndexUnderPriority()
    {
        // 优先取到 42，但状态串显示常规游标 3
        string status = RegenMonstersLoopCore.FormatMonGenInfo("鸡", 3, 100);
        Assert.Contains(",3/", status);
        Assert.DoesNotContain(",42/", status);
    }

    // ===================== 一轮时序（RunRound） =====================

    private static RegenMonstersLoopCore.RegenRound RunSimple(
        bool stopRun = false, bool venture = false, bool stopMake = false,
        int now = 1000, int regenTick = 0, int regenTime = 200,
        bool monGenIsNull = false, bool boFB = false, string name = "鸡", bool makeMon = true,
        MonGenParseCore.CompareType ct = MonGenParseCore.CompareType.ctLess,
        int startTick = 0, int zenTime = 60_000, int monCount = 10, int monGenRate = 10,
        int existing = 0, bool regenOk = true, Action<int>? onRegen = null)
    {
        int tick = regenTick;
        int outStart = startTick;

        return RegenMonstersLoopCore.RunRound(
            stopRun, venture, stopMake, now, ref tick, regenTime,
            false, monGenIsNull, boFB, name, makeMon,
            ct, Gvals, 1, 10,
            startTick, zenTime, monCount, monGenRate,
            _ => existing,
            n => { onRegen?.Invoke(n); return regenOk; },
            ref outStart);
    }

    [Fact]
    public void RoundAbortsWhenStopped()
    {
        var r = RunSimple(stopRun: true);
        Assert.False(r.EnteredBlock);
    }

    [Fact]
    public void RoundSkippedByThrottle()
    {
        var r = RunSimple(now: 100, regenTick: 0, regenTime: 200);
        Assert.False(r.EnteredBlock);
    }

    [Fact]
    public void RoundEntersBlockAndSpawns()
    {
        var r = RunSimple(now: 1000, regenTick: 0, regenTime: 200);

        Assert.True(r.EnteredBlock);
        Assert.True(r.DidSpawn);
        Assert.Equal(10, r.MakeMonsterCount);
        Assert.True(r.RefreshedStartTick);
    }

    [Fact]
    public void RoundPassesCountToRegen()
    {
        int got = -1;
        RunSimple(now: 1000, existing: 3, onRegen: n => got = n);

        Assert.Equal(7, got);   // 10 - 3
    }

    [Fact]
    public void RoundSkipsRegenWhenNothingToMake()
    {
        bool called = false;
        var r = RunSimple(now: 1000, existing: 999, onRegen: _ => called = true);

        Assert.False(called);
        Assert.Equal(0, r.MakeMonsterCount);
        Assert.True(r.RefreshedStartTick);   // **仍然续期**
    }

    [Fact]
    public void RoundSkipsRefreshWhenRegenFails()
    {
        var r = RunSimple(now: 1000, existing: 0, regenOk: false);

        Assert.True(r.DidSpawn);
        Assert.False(r.RefreshedStartTick);   // 唯一不刷新的路径
    }

    [Fact]
    public void RoundBlockedByFbTemplate()
    {
        var r = RunSimple(now: 1000, boFB: true);
        Assert.True(r.EnteredBlock);
        Assert.False(r.DidSpawn);
    }

    [Fact]
    public void RoundBlockedByGVar()
    {
        // ctLess: Gvals[1]=5 < 10 → True；换成 ctGreater 则为 False
        var ok = RunSimple(now: 1000, ct: MonGenParseCore.CompareType.ctLess);
        Assert.True(ok.DidSpawn);

        var bad = RunSimple(now: 1000, ct: MonGenParseCore.CompareType.ctGreater);
        Assert.False(bad.DidSpawn);
    }

    [Fact]
    public void RoundBlockedByZenTime()
    {
        var r = RunSimple(now: 1000, startTick: 990, zenTime: 60_000);
        Assert.True(r.EnteredBlock);
        Assert.False(r.DidSpawn);
    }

    [Fact]
    public void RoundEntersBlockEvenWhenMonGenNull()
    {
        // 3913-3938：优先分支没取到且常规游标也空 → MonGen 为 nil，
        // 但**节流块已进入并已续期 regenTick**
        var r = RunSimple(now: 1000, monGenIsNull: true);
        Assert.True(r.EnteredBlock);
        Assert.False(r.DidSpawn);
    }

    [Fact]
    public void EveryRoundYieldsAtMostOneMonGen()
    {
        // 每轮至多处理一个刷怪点
        var r = RunSimple(now: 1000);
        Assert.True(r.DidSpawn);
    }
}
