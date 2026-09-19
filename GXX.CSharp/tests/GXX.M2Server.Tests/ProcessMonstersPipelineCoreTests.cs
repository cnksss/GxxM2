using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J119：怪物处理两阶段流水线（UsrEngn.pas 3994-4001 / 4007-4088 / 4111-4164 /
/// 4165-4198）1:1 测试。
/// </summary>
public sealed class ProcessMonstersPipelineCoreTests
{
    private static ProcessMonstersPipelineCore.PipeMonGen Mg(string name, params ProcessMonstersPipelineCore.PipeCert[] certs)
        => new() { Name = name, Certs = new List<ProcessMonstersPipelineCore.PipeCert>(certs) };

    private static ProcessMonstersPipelineCore.PipeCert C(uint runTick = 0, int runTime = 0, string name = "m")
        => ProcessMonstersPipelineCore.PipeCert.Normal(runTick, runTime, name);

    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(ProcessMonstersPipelineCore.RunListClearedEachRound);
        Assert.Equal(0, ProcessMonstersPipelineCore.ProcessCountReset);
        Assert.False(ProcessMonstersPipelineCore.InitialBoProcessLimit);
        Assert.Equal(0, ProcessMonstersPipelineCore.InitialTCode);
        Assert.False(ProcessMonstersPipelineCore.InitialIsError);
    }

    [Fact]
    public void TickOriginOrder()
    {
        // 3992 → 3995 → 3998
        Assert.Equal(new[]
        {
            ProcessMonstersPipelineCore.TickOrigin.RunTick,
            ProcessMonstersPipelineCore.TickOrigin.CurrentTick,
            ProcessMonstersPipelineCore.TickOrigin.MonProcTick,
        }, ProcessMonstersPipelineCore.TickOriginOrder);
    }

    [Fact]
    public void CurrentTickTakenBeforePhaseOne()
    {
        Assert.True(ProcessMonstersPipelineCore.CurrentTickTakenBeforePhaseOne());
        Assert.True(ProcessMonstersPipelineCore.RunTickTakenBeforeMonProcTick());
    }

    // ===================== 入队条件（4039） =====================

    [Fact]
    public void EnqueueRequiresStrictlyGreater()
    {
        // tick_diff(0, 100) = 100 > 100 为假
        Assert.False(ProcessMonstersPipelineCore.ShouldEnqueue(0, 100, 100));
        Assert.True(ProcessMonstersPipelineCore.ShouldEnqueue(0, 101, 100));
    }

    [Fact]
    public void EnqueueAtZeroRunTime()
    {
        // m_nRunTime = 0 → 只要差值 > 0 即入队
        Assert.False(ProcessMonstersPipelineCore.ShouldEnqueue(500, 500, 0));
        Assert.True(ProcessMonstersPipelineCore.ShouldEnqueue(500, 501, 0));
    }

    [Fact]
    public void EnqueueUsesTickDiffNotPlainSubtract()
    {
        // 两处减法在回绕时不同——同一对输入结果不一致
        var (diff, plain) = ProcessMonstersPipelineCore.CompareSubtractions(4294967290, 5);

        Assert.Equal(10u, diff);    // tick_diff 回绕：比真实差值 11 小 1
        Assert.Equal(11u, plain);   // 直接相减
        Assert.NotEqual(diff, plain);
    }

    [Fact]
    public void EnqueueWraparoundIsTolerated()
    {
        // 用 tick_diff 故回绕不产生巨大差值
        Assert.True(ProcessMonstersPipelineCore.ShouldEnqueue(4294967290, 5, 0));
    }

    [Fact]
    public void IncrementProcessCount()
    {
        Assert.Equal(1, ProcessMonstersPipelineCore.IncrementProcessCount(0));
        Assert.Equal(6, ProcessMonstersPipelineCore.IncrementProcessCount(5));
    }

    [Fact]
    public void BuildMonGenInfo2Format()
    {
        Assert.Equal("怪/3/7", ProcessMonstersPipelineCore.BuildMonGenInfo2("怪", 3, 7));
    }

    [Fact]
    public void GhostDeleteExplicitlyNullsMonster()
    {
        Assert.True(ProcessMonstersPipelineCore.GhostDeleteExplicitlyNullsMonster());
    }

    // ===================== 候选表每轮重建（4001） =====================

    [Fact]
    public void RunListRebuiltEachRound()
    {
        Assert.True(ProcessMonstersPipelineCore.RunListClearedEachRound);
    }

    [Fact]
    public void RunListDoesNotGrowAcrossRounds()
    {
        // **结构关键**：三轮的候选表大小恒为 1（不累积）
        var gens = new List<ProcessMonstersPipelineCore.PipeMonGen>
        {
            Mg("a", C(runTick: 0, runTime: 0, name: "x")),
        };

        var sizes = ProcessMonstersPipelineCore.RunListSizesAcrossRounds(gens, 3, 100, 0);

        Assert.Equal(new[] { 1, 1, 1 }, sizes);
    }

    [Fact]
    public void RunListEmptyWhenNothingQualifies()
    {
        var gens = new List<ProcessMonstersPipelineCore.PipeMonGen>
        {
            Mg("a", C(runTick: 100, runTime: 9999, name: "x")),
        };

        var sizes = ProcessMonstersPipelineCore.RunListSizesAcrossRounds(gens, 2, 100, 0);

        Assert.Equal(new[] { 0, 0 }, sizes);
    }

    // ===================== 第二阶段前置（4115） =====================

    [Fact]
    public void SearchRequiresStrictlyGreaterAndPenvir()
    {
        Assert.True(ProcessMonstersPipelineCore.ShouldProcessSearch(200, 0, 100, false));

        Assert.False(ProcessMonstersPipelineCore.ShouldProcessSearch(100, 0, 100, false)); // 相等
        Assert.False(ProcessMonstersPipelineCore.ShouldProcessSearch(200, 0, 100, true));  // Penvir nil
    }

    [Fact]
    public void SearchUsesPlainSubtractNotTickDiff()
    {
        // 4115 是直接相减（与 4039 的 tick_diff 不同）
        var (diff, plain) = ProcessMonstersPipelineCore.CompareSubtractions(4294967290, 5);

        Assert.True(ProcessMonstersPipelineCore.ShouldProcessSearch(5, 4294967290, 0, false));
        Assert.NotEqual(diff, plain);
    }

    [Fact]
    public void EnqueuedDoesNotImplySearched()
    {
        // 两个阶段用不同字段与不同减法
        Assert.True(ProcessMonstersPipelineCore.EnqueuedDoesNotImplySearched());
    }

    [Fact]
    public void EnqueuedButNotSearchedInPractice()
    {
        // 入队（runTick 满足）但视野条件不满足 → 只在候选表里、不被搜索
        var gens = new List<ProcessMonstersPipelineCore.PipeMonGen>
        {
            Mg("a", C(runTick: 0, runTime: 0, name: "x")),
        };

        var round = ProcessMonstersPipelineCore.RunRound(
            gens, currentTick: 100, runTick: 0, now: 100,
            searchTickOf: _ => 100, searchTimeOf: _ => 100,   // 差值 0，不 > 100
            penvirIsNull: _ => false,
            boIsVisibleActive: _ => false, processMonsterInterval: 0,
            humCount: _ => 1, ghostNow: _ => false, deathNow: _ => false, poisonTime: _ => 0,
            clearHumOrBBTick: 0, interruptEverything: false);

        Assert.Equal(new[] { "x" }, round.RunListNames);
        Assert.Empty(round.SearchedNames);
    }

    [Fact]
    public void SearchedWhenConditionsMet()
    {
        var gens = new List<ProcessMonstersPipelineCore.PipeMonGen>
        {
            Mg("a", C(runTick: 0, runTime: 0, name: "x")),
        };

        var round = ProcessMonstersPipelineCore.RunRound(
            gens, currentTick: 1000, runTick: 0, now: 1000,
            searchTickOf: _ => 0, searchTimeOf: _ => 100,
            penvirIsNull: _ => false,
            boIsVisibleActive: _ => false, processMonsterInterval: 0,
            humCount: _ => 1, ghostNow: _ => false, deathNow: _ => false, poisonTime: _ => 0,
            clearHumOrBBTick: 0, interruptEverything: false);

        Assert.Equal(new[] { "x" }, round.SearchedNames);
    }

    [Fact]
    public void SearchSkippedWhenPenvirNull()
    {
        var gens = new List<ProcessMonstersPipelineCore.PipeMonGen>
        {
            Mg("a", C(runTick: 0, runTime: 0, name: "x")),
        };

        var round = ProcessMonstersPipelineCore.RunRound(
            gens, currentTick: 1000, runTick: 0, now: 1000,
            searchTickOf: _ => 0, searchTimeOf: _ => 100,
            penvirIsNull: _ => true,
            boIsVisibleActive: _ => false, processMonsterInterval: 0,
            humCount: _ => 1, ghostNow: _ => false, deathNow: _ => false, poisonTime: _ => 0,
            clearHumOrBBTick: 0, interruptEverything: false);

        Assert.Empty(round.SearchedNames);
    }

    [Fact]
    public void SearchOnEmptyMapDoesSilentClear()
    {
        // 4119 不成立（无人、无毒）→ 4128-4134 静默清理，**不刷新 tick**
        var gens = new List<ProcessMonstersPipelineCore.PipeMonGen>
        {
            Mg("a", C(runTick: 0, runTime: 0, name: "x")),
        };

        var round = ProcessMonstersPipelineCore.RunRound(
            gens, currentTick: 1000, runTick: 0, now: 1000,
            searchTickOf: _ => 0, searchTimeOf: _ => 100,
            penvirIsNull: _ => false,
            boIsVisibleActive: _ => false, processMonsterInterval: 0,
            humCount: _ => 0, ghostNow: _ => false, deathNow: _ => false, poisonTime: _ => 0,
            clearHumOrBBTick: 0, interruptEverything: false);

        Assert.Contains("x", round.SilentClearedNames);
        Assert.Empty(round.SearchedNames);
    }

    [Fact]
    public void SearchWhenPoisonedOnEmptyMap()
    {
        // 4119 的第三项：自己中毒也算"有人"
        var gens = new List<ProcessMonstersPipelineCore.PipeMonGen>
        {
            Mg("a", C(runTick: 0, runTime: 0, name: "x")),
        };

        var round = ProcessMonstersPipelineCore.RunRound(
            gens, currentTick: 1000, runTick: 0, now: 1000,
            searchTickOf: _ => 0, searchTimeOf: _ => 100,
            penvirIsNull: _ => false,
            boIsVisibleActive: _ => false, processMonsterInterval: 0,
            humCount: _ => 0, ghostNow: _ => false, deathNow: _ => false, poisonTime: _ => 1,
            clearHumOrBBTick: 0, interruptEverything: false);

        Assert.Equal(new[] { "x" }, round.SearchedNames);
    }

    // ===================== 两阶段串联 =====================

    [Fact]
    public void PhaseTwoIteratesPhaseOneList()
    {
        Assert.True(ProcessMonstersPipelineCore.PhaseTwoIteratesPhaseOneList());
    }

    [Fact]
    public void PhaseTwoOnlySeesEnqueued()
    {
        // **关键**：不入队的怪物第二阶段完全看不到
        var gens = new List<ProcessMonstersPipelineCore.PipeMonGen>
        {
            Mg("a",
                C(runTick: 0, runTime: 0, name: "in"),
                C(runTick: 999, runTime: 9999, name: "out")),
        };

        var round = ProcessMonstersPipelineCore.RunRound(
            gens, currentTick: 1000, runTick: 0, now: 1000,
            searchTickOf: _ => 0, searchTimeOf: _ => 100,
            penvirIsNull: _ => false,
            boIsVisibleActive: _ => false, processMonsterInterval: 0,
            humCount: _ => 1, ghostNow: _ => false, deathNow: _ => false, poisonTime: _ => 0,
            clearHumOrBBTick: 0, interruptEverything: false);

        Assert.Equal(new[] { "in" }, round.RunListNames);
        Assert.DoesNotContain("out", round.SearchedNames);
        Assert.DoesNotContain("out", round.RanNames);
    }

    [Fact]
    public void PhaseTwoIterationsMatchRunListSize()
    {
        var gens = new List<ProcessMonstersPipelineCore.PipeMonGen>
        {
            Mg("a",
                C(runTick: 0, runTime: 0, name: "1"),
                C(runTick: 0, runTime: 0, name: "2"),
                C(runTick: 0, runTime: 0, name: "3")),
        };

        var round = ProcessMonstersPipelineCore.RunRound(
            gens, currentTick: 1000, runTick: 0, now: 1000,
            searchTickOf: _ => 0, searchTimeOf: _ => 100,
            penvirIsNull: _ => false,
            boIsVisibleActive: _ => false, processMonsterInterval: 0,
            humCount: _ => 1, ghostNow: _ => false, deathNow: _ => false, poisonTime: _ => 0,
            clearHumOrBBTick: 0, interruptEverything: false);

        Assert.Equal(3, round.RunListNames.Count);
        Assert.Equal(ProcessMonstersPipelineCore.PhaseTwoIterations(3), round.RunListNames.Count);
    }

    [Fact]
    public void MonstersAcrossMultipleMonGensAllEnqueue()
    {
        var gens = new List<ProcessMonstersPipelineCore.PipeMonGen>
        {
            Mg("a", C(0, 0, "a1"), C(0, 0, "a2")),
            Mg("b", C(0, 0, "b1")),
        };

        var round = ProcessMonstersPipelineCore.RunRound(
            gens, currentTick: 1000, runTick: 0, now: 1000,
            searchTickOf: _ => 0, searchTimeOf: _ => 100,
            penvirIsNull: _ => false,
            boIsVisibleActive: _ => false, processMonsterInterval: 0,
            humCount: _ => 1, ghostNow: _ => false, deathNow: _ => false, poisonTime: _ => 0,
            clearHumOrBBTick: 0, interruptEverything: false);

        Assert.Equal(3, round.RunListNames.Count);
        Assert.Equal(3, round.MonsterProcessCount);
    }

    [Fact]
    public void ProcessLimitStopsFirstPhase()
    {
        var gens = new List<ProcessMonstersPipelineCore.PipeMonGen>
        {
            Mg("a", C(0, 0, "x")),
        };

        var round = ProcessMonstersPipelineCore.RunRound(
            gens, currentTick: 1000, runTick: 0, now: 1000,
            searchTickOf: _ => 0, searchTimeOf: _ => 100,
            penvirIsNull: _ => false,
            boIsVisibleActive: _ => false, processMonsterInterval: 0,
            humCount: _ => 1, ghostNow: _ => false, deathNow: _ => false, poisonTime: _ => 0,
            clearHumOrBBTick: 0, interruptEverything: true);

        Assert.True(round.ProcessLimit);
        Assert.Empty(round.RunListNames);
    }

    [Fact]
    public void NilCertSkippedInPhaseOne()
    {
        var gens = new List<ProcessMonstersPipelineCore.PipeMonGen>
        {
            Mg("a", ProcessMonstersPipelineCore.PipeCert.Nil, C(0, 0, "real")),
        };

        var round = ProcessMonstersPipelineCore.RunRound(
            gens, currentTick: 1000, runTick: 0, now: 1000,
            searchTickOf: _ => 0, searchTimeOf: _ => 100,
            penvirIsNull: _ => false,
            boIsVisibleActive: _ => false, processMonsterInterval: 0,
            humCount: _ => 1, ghostNow: _ => false, deathNow: _ => false, poisonTime: _ => 0,
            clearHumOrBBTick: 0, interruptEverything: false);

        Assert.Equal(new[] { "real" }, round.RunListNames);
    }

    [Fact]
    public void EmptyMonGenListGivesEmptyRound()
    {
        var round = ProcessMonstersPipelineCore.RunRound(
            new List<ProcessMonstersPipelineCore.PipeMonGen>(),
            1000, 0, 1000,
            _ => 0, _ => 100, _ => false, _ => false, 0,
            _ => 1, _ => false, _ => false, _ => 0, 0, false);

        Assert.Empty(round.RunListNames);
        Assert.Equal(0, round.MonsterProcessCount);
        Assert.False(round.ProcessLimit);
    }

    // ===================== 收尾统计（4165-4174） =====================

    [Fact]
    public void MonProcTimeFromMonProcTick()
    {
        Assert.Equal(50, ProcessMonstersPipelineCore.ComputeMonProcTime(150, 100));
    }

    [Fact]
    public void MonTimeFromRunTick()
    {
        Assert.Equal(80, ProcessMonstersPipelineCore.ComputeMonTime(180, 100));
    }

    [Fact]
    public void BothUseSameNow()
    {
        // 两行相邻、同一个 MyGetTickCount → monTime >= monProcTime（起点更早）
        uint now = 500;
        int proc = ProcessMonstersPipelineCore.ComputeMonProcTime(now, 300);
        int time = ProcessMonstersPipelineCore.ComputeMonTime(now, 200);

        Assert.True(time >= proc);
        Assert.True(ProcessMonstersPipelineCore.MonTimeAtLeastMonProcTime());
    }

    [Fact]
    public void UpdateStatsTracksBothPeaks()
    {
        var stats = new ProcessMonstersEnvelopeCore.ProcTimeStats();

        ProcessMonstersPipelineCore.UpdateStats(stats, 10, 20);
        ProcessMonstersPipelineCore.UpdateStats(stats, 30, 5);

        Assert.Equal(30, stats.MonProcTimeMin);
        Assert.Equal(30, stats.MonProcTimeMax);
        Assert.Equal(20, stats.MonTimeMax);   // 5 未超过 20
    }

    [Fact]
    public void ProcessCountIsWriteOnly()
    {
        // 3999 清零、4044 累加，但 4165-4174 **完全不用它**
        Assert.True(ProcessMonstersPipelineCore.ProcessCountIsWriteOnly());
    }

    [Fact]
    public void ProcessCountNotUsedInFinalizeStats()
    {
        // 对照：收尾只用两个耗时值，与 nMonsterProcessCount 无关
        var stats = new ProcessMonstersEnvelopeCore.ProcTimeStats();
        ProcessMonstersPipelineCore.UpdateStats(stats, 1, 2);

        Assert.Equal(1, stats.MonProcTimeMin);
        Assert.Equal(2, stats.MonTimeMax);
    }

    // ===================== 异常收尾（4175-4198） =====================

    [Fact]
    public void ExceptionUsesNilPlaceholderWhenMonsterNull()
    {
        string msg = ProcessMonstersPipelineCore.SelectExceptionMessage(true, 12, "boom");

        Assert.Contains("nil", msg);
        Assert.DoesNotContain("boom", msg);
    }

    [Fact]
    public void ExceptionUsesRealMessageWhenMonsterNotNull()
    {
        string msg = ProcessMonstersPipelineCore.SelectExceptionMessage(false, 12, "boom");

        Assert.Contains("boom", msg);
        Assert.Contains("12", msg);
    }

    [Fact]
    public void ExceptionMessageFormat()
    {
        string msg = ProcessMonstersPipelineCore.SelectExceptionMessage(false, 200, "x");

        Assert.Equal("[Exception] TUserEngine.ProcessMonsters 200; x", msg);
    }

    [Fact]
    public void IsErrorOnlyWhenMonsterNotNull()
    {
        Assert.True(ProcessMonstersPipelineCore.SetsIsErrorOnlyWhenMonsterNotNull(false));
        Assert.False(ProcessMonstersPipelineCore.SetsIsErrorOnlyWhenMonsterNotNull(true));
    }

    [Fact]
    public void AbnormalObjectPrintedOnlyWhenIsErrorAndNotNull()
    {
        Assert.True(ProcessMonstersPipelineCore.ShouldPrintAbnormal(true, false));
        Assert.False(ProcessMonstersPipelineCore.ShouldPrintAbnormal(true, true));
        Assert.False(ProcessMonstersPipelineCore.ShouldPrintAbnormal(false, false));
    }

    [Fact]
    public void ExceptionDetailUsesStaleMonster()
    {
        // 4114 之前异常 → 读到第一阶段残留值
        Assert.True(ProcessMonstersPipelineCore.ExceptionDetailUsesStaleMonster());
    }

    [Fact]
    public void MonsterStartsNullByAssumption()
    {
        // 建模假设（原文未显式初始化局部对象变量）
        Assert.True(ProcessMonstersPipelineCore.MonsterStartsNullByAssumption());
    }

    // ===================== tCode 序列 =====================

    [Fact]
    public void TCodesAreShortAndContainKeyMarkers()
    {
        var codes = ProcessMonstersPipelineCore.TCodes;

        Assert.Contains(11, codes);    // 4010 第一阶段开始
        Assert.Contains(121, codes);   // 4028 幽灵检查
        Assert.Contains(150, codes);   // 4064 内层次数推进
        Assert.Contains(200, codes);   // 4111 第二阶段开始
        Assert.Contains(149, codes);   // 4137 节流前
        Assert.Contains(1600, codes);  // 4165 统计
        Assert.Contains(1601, codes);  // 4171 统计
    }

    [Fact]
    public void TCodesAreUnique()
    {
        var seen = new HashSet<int>(ProcessMonstersPipelineCore.TCodes);
        Assert.Equal(ProcessMonstersPipelineCore.TCodes.Length, seen.Count);
    }
}
