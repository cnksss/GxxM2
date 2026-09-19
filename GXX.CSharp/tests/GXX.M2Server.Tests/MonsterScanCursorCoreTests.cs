using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J118：怪物处理主循环的跨轮游标状态机（UsrEngn.pas 4007-4104）1:1 测试。
/// </summary>
public sealed class MonsterScanCursorCoreTests
{
    private static MonsterScanCursorCore.CursorState St(
        int pos = 0, int cert = 0, int mc = 0, int mpp = 0)
        => new()
        {
            MonGenListPosition = pos,
            MonGenCertListPosition = cert,
            MonsterCount = mc,
            MonsterProcessPosition = mpp,
        };

    /// <summary>永不中断。</summary>
    private static Func<int, int, bool> NoInterrupt() => (_, _) => false;

    /// <summary>在第 mgIdx 个刷怪点、处理完第 afterPos 条后中断。</summary>
    private static Func<int, int, bool> InterruptAt(int mgIdx, int afterPos)
        => (i, p) => i == mgIdx && p >= afterPos;

    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.Equal(0, MonsterScanCursorCore.CertListPositionResetOnEntry);
        Assert.Equal(0, MonsterScanCursorCore.CertListPositionFallback);
        Assert.True(MonsterScanCursorCore.ZeroIterationPossible);
    }

    // ===================== 内层起点（4011-4019） =====================

    [Fact]
    public void CertStartUsesSavedWhenInRange()
    {
        Assert.Equal(3, MonsterScanCursorCore.SelectCertStartPosition(3, 10));
    }

    [Fact]
    public void CertStartFallsBackAtBoundary()
    {
        // **严格 <**：相等即取 0
        Assert.Equal(0, MonsterScanCursorCore.SelectCertStartPosition(10, 10));
        Assert.Equal(0, MonsterScanCursorCore.SelectCertStartPosition(11, 10));
    }

    [Fact]
    public void CertStartZeroWhenListEmpty()
    {
        Assert.Equal(0, MonsterScanCursorCore.SelectCertStartPosition(0, 0));
        Assert.Equal(0, MonsterScanCursorCore.SelectCertStartPosition(5, 0));
    }

    [Fact]
    public void EnterInnerLoopClearsSavedPosition()
    {
        // **4019 读后即清**
        var (start, saved) = MonsterScanCursorCore.EnterInnerLoop(4, 10);

        Assert.Equal(4, start);      // 起点采用了保存值
        Assert.Equal(0, saved);      // 但保存位被清零
    }

    [Fact]
    public void EnterInnerLoopClearsEvenWhenFallingBack()
    {
        var (start, saved) = MonsterScanCursorCore.EnterInnerLoop(99, 10);

        Assert.Equal(0, start);
        Assert.Equal(0, saved);
    }

    [Fact]
    public void ResetOnEntryIsZero()
    {
        Assert.Equal(0, MonsterScanCursorCore.ResetCertListPositionOnEntry());
    }

    // ===================== 单轮：正常跑完 =====================

    [Fact]
    public void NormalRoundVisitsAllMonGens()
    {
        var s = St();
        var round = MonsterScanCursorCore.RunScan(s, new[] { 2, 3, 1 }, NoInterrupt());

        Assert.Equal(new[] { 0, 1, 2 }, round.VisitedMonGenIndices);
        Assert.False(round.Interrupted);
    }

    [Fact]
    public void NormalRoundInnerStartsAllZero()
    {
        // 每轮进入前保存位都是 0
        var s = St();
        var round = MonsterScanCursorCore.RunScan(s, new[] { 2, 3, 1 }, NoInterrupt());

        Assert.All(round.InnerStartPositions, p => Assert.Equal(0, p));
    }

    [Fact]
    public void NormalCompletionResetsOuterCursor()
    {
        var s = St(pos: 0);
        MonsterScanCursorCore.RunScan(s, new[] { 1, 1 }, NoInterrupt());

        Assert.Equal(0, s.MonGenListPosition);
    }

    [Fact]
    public void NormalCompletionClearsSavedCertPosition()
    {
        // **关键**：正常跑完不会留下续扫位置
        var s = St(cert: 5);
        var round = MonsterScanCursorCore.RunScan(s, new[] { 10 }, NoInterrupt());

        Assert.Equal(0, s.MonGenCertListPosition);
        Assert.False(round.Interrupted);
    }

    [Fact]
    public void PresetPositionIsHonouredOnFirstRound()
    {
        // **注意语义**：4019 的"读后即清"只防止**后续轮**继续沿用；
        // 首轮进入时若保存位是 5，仍会被采用为起点（4011-4013）。
        var s = St(cert: 5);
        var round = MonsterScanCursorCore.RunScan(s, new[] { 10 }, NoInterrupt());

        Assert.Equal(5, round.InnerStartPositions[0]);
        Assert.Equal(0, s.MonGenCertListPosition);   // 读后已清
    }

    [Fact]
    public void NormalCompletionDoesNotResume()
    {
        // 正常跑完两轮：第一轮从 0 开始、清掉保存位；第二轮**仍从 0**（不会续扫）
        var s = St();
        var r1 = MonsterScanCursorCore.RunScan(s, new[] { 10 }, NoInterrupt());
        Assert.Equal(0, r1.InnerStartPositions[0]);

        var r2 = MonsterScanCursorCore.RunScan(s, new[] { 10 }, NoInterrupt());
        Assert.Equal(0, r2.InnerStartPositions[0]);
    }

    [Fact]
    public void PresetIsConsumedOnlyOnce()
    {
        // 预设的 5 只在首轮被采用，第二轮回到 0
        var s = St(cert: 5);
        var r1 = MonsterScanCursorCore.RunScan(s, new[] { 10 }, NoInterrupt());
        var r2 = MonsterScanCursorCore.RunScan(s, new[] { 10 }, NoInterrupt());

        Assert.Equal(5, r1.InnerStartPositions[0]);
        Assert.Equal(0, r2.InnerStartPositions[0]);
    }

    // ===================== 单轮：统计 =====================

    [Fact]
    public void FullRoundUpdatesMonsterCount()
    {
        var s = St();
        MonsterScanCursorCore.RunScan(s, new[] { 2, 3 }, NoInterrupt());

        // 累计处理 5 条 → nMonsterCount = 5
        Assert.Equal(5, s.MonsterCount);
    }

    [Fact]
    public void FullRoundZeroesProcessPosition()
    {
        var s = St();
        MonsterScanCursorCore.RunScan(s, new[] { 2, 3 }, NoInterrupt());

        Assert.Equal(0, s.MonsterProcessPosition);
    }

    [Fact]
    public void FullRoundFlagSet()
    {
        var s = St();
        var round = MonsterScanCursorCore.RunScan(s, new[] { 1 }, NoInterrupt());

        Assert.True(round.Finalized.FullRound);
    }

    [Fact]
    public void EmptyMonGenListZeroesStats()
    {
        // 零次迭代 → 保守解释使 `Count <= I` 成立 → 统计归零
        var s = St(mc: 99, mpp: 77);
        var round = MonsterScanCursorCore.RunScan(s, Array.Empty<int>(), NoInterrupt());

        Assert.Empty(round.VisitedMonGenIndices);
        Assert.True(round.Finalized.FullRound);
        Assert.Equal(0, s.MonsterProcessPosition);
    }

    [Fact]
    public void StartPositionBeyondEndSkipsAll()
    {
        // 起点 >= 终点 → 零次迭代
        var s = St(pos: 5);
        var round = MonsterScanCursorCore.RunScan(s, new[] { 1, 1 }, NoInterrupt());

        Assert.Empty(round.VisitedMonGenIndices);
    }

    [Fact]
    public void StartPositionAtLastElementVisitsOnlyIt()
    {
        // 4007：`for I := pos to Count - 1` —— pos=2、Count=3 时**只访问下标 2**
        var s = St(pos: 2);
        var round = MonsterScanCursorCore.RunScan(s, new[] { 1, 1, 1 }, NoInterrupt());

        Assert.Equal(new[] { 2 }, round.VisitedMonGenIndices);
    }

    [Fact]
    public void StartPositionVisitsThroughEnd()
    {
        // 与上条对照：pos=1 → 访问 1 与 2（跑到 Count-1 结束，不是只访问 pos）
        var s = St(pos: 1);
        var round = MonsterScanCursorCore.RunScan(s, new[] { 1, 1, 1 }, NoInterrupt());

        Assert.Equal(new[] { 1, 2 }, round.VisitedMonGenIndices);
    }

    // ===================== 单轮：限时中断 =====================

    [Fact]
    public void InterruptStopsScan()
    {
        var s = St();
        var round = MonsterScanCursorCore.RunScan(s, new[] { 3, 3, 3 }, InterruptAt(0, 2));

        Assert.True(round.Interrupted);
        Assert.Equal(new[] { 0 }, round.VisitedMonGenIndices);
    }

    [Fact]
    public void InterruptSavesAdvancedPosition()
    {
        // 4074 保存的是**递增后**的值
        var s = St();
        var round = MonsterScanCursorCore.RunScan(s, new[] { 5 }, InterruptAt(0, 2));

        Assert.Equal(2, s.MonGenCertListPosition);
        Assert.Equal(2, round.SavedCertPosition);
    }

    [Fact]
    public void SavedPositionIsReadableBeforeNextScan()
    {
        Assert.True(MonsterScanCursorCore.SavedPositionPointsToNextUnprocessed());

        // 处理完第 1、2 条后中断 → 保存 2；该值在下一轮进入内层时被读取并采用
        var s = St();
        MonsterScanCursorCore.RunScan(s, new[] { 5 }, InterruptAt(0, 2));

        var (nextStart, cleared) = MonsterScanCursorCore.EnterInnerLoop(s.MonGenCertListPosition, 5);
        Assert.Equal(2, nextStart);   // 采用保存值
        Assert.Equal(0, cleared);     // 同时清零（4019）
    }

    [Fact]
    public void NextRoundResumesAtSavedPosition()
    {
        // 完整两轮：第一轮中断、第二轮从保存处继续
        var s = St();

        var r1 = MonsterScanCursorCore.RunScan(s, new[] { 5 }, InterruptAt(0, 2));
        Assert.True(r1.Interrupted);
        Assert.Equal(2, s.MonGenCertListPosition);
        Assert.Equal(0, s.MonGenListPosition);   // 留在同一刷怪点

        var r2 = MonsterScanCursorCore.RunScan(s, new[] { 5 }, NoInterrupt());
        Assert.Equal(0, r2.VisitedMonGenIndices[0]);
        Assert.Equal(2, r2.InnerStartPositions[0]);   // **从 2 继续**
    }

    [Fact]
    public void InterruptKeepsSameMonGenIndex()
    {
        // 4103：留在同一个 I，不是 I + 1
        Assert.Equal(3, MonsterScanCursorCore.InterruptKeepsSameMonGenIndex(3));

        var s = St();
        var round = MonsterScanCursorCore.RunScan(s, new[] { 1, 1, 1 }, InterruptAt(1, 1));

        Assert.Equal(1, s.MonGenListPosition);   // 同一个刷怪点
        Assert.Equal(new[] { 0, 1 }, round.VisitedMonGenIndices);
    }

    [Fact]
    public void InterruptLeavesStatsUntouched()
    {
        // 中断轮不更新 nMonsterCount、不归零 nMonsterProcessPostion
        // 预设 mpp = 7，本轮处理 2 条 → 累计 9，但**不归零**
        var s = St(mc: 42, mpp: 7);
        MonsterScanCursorCore.RunScan(s, new[] { 5, 5 }, InterruptAt(0, 2));

        Assert.True(MonsterScanCursorCore.InterruptLeavesStatsUntouched());
        Assert.Equal(42, s.MonsterCount);          // 保持旧值，未被 4093 更新
        Assert.Equal(9, s.MonsterProcessPosition); // 已累计但**未归零**
    }

    [Fact]
    public void InterruptStatsContrastWithFullRound()
    {
        // 对照：同样预设 mpp = 7，但整轮跑完 → nMonsterCount 被更新、mpp 归零
        var s = St(mc: 42, mpp: 7);
        MonsterScanCursorCore.RunScan(s, new[] { 5 }, NoInterrupt());

        Assert.Equal(12, s.MonsterCount);          // 7 + 5，被 4093 更新
        Assert.Equal(0, s.MonsterProcessPosition); // 被 4094 归零
    }

    [Fact]
    public void InterruptRoundFullRoundFlagFalse()
    {
        var s = St();
        var round = MonsterScanCursorCore.RunScan(s, new[] { 5 }, InterruptAt(0, 1));

        Assert.False(round.Finalized.FullRound);
    }

    [Fact]
    public void InterruptAtLastMonGen()
    {
        var s = St();
        var round = MonsterScanCursorCore.RunScan(s, new[] { 1, 5 }, InterruptAt(1, 3));

        Assert.Equal(new[] { 0, 1 }, round.VisitedMonGenIndices);
        Assert.True(round.Interrupted);
        Assert.Equal(3, s.MonGenCertListPosition);
    }

    [Fact]
    public void GhostDeletePathCannotTriggerInterrupt()
    {
        // 4060 的 Continue 跳过 4067 的限时判定
        Assert.True(MonsterScanCursorCore.GhostDeletePathCannotTriggerInterrupt());
    }

    // ===================== 收尾（4089-4104） =====================

    [Fact]
    public void FullRoundFlagBoundary()
    {
        Assert.True(MonsterScanCursorCore.IsFullRoundCompleted(3, 3));
        Assert.True(MonsterScanCursorCore.IsFullRoundCompleted(3, 4));
        Assert.False(MonsterScanCursorCore.IsFullRoundCompleted(3, 2));
    }

    [Fact]
    public void FinalizeNonInterruptSetsZero()
    {
        var r = MonsterScanCursorCore.Finalize(3, 3, false, 5, 5);
        Assert.Equal(0, r.MonGenListPosition);
    }

    [Fact]
    public void FinalizeInterruptKeepsIndex()
    {
        var r = MonsterScanCursorCore.Finalize(5, 2, true, 5, 5);
        Assert.Equal(2, r.MonGenListPosition);
    }

    [Fact]
    public void FinalizeFullRoundUpdatesStats()
    {
        var r = MonsterScanCursorCore.Finalize(3, 3, false, 99, 7);

        Assert.Equal(7, r.MonsterCount);            // 4093
        Assert.Equal(0, r.MonsterProcessPosition);  // 4094
    }

    [Fact]
    public void FinalizeInterruptKeepsStats()
    {
        var r = MonsterScanCursorCore.Finalize(5, 2, true, 99, 7);

        Assert.Equal(99, r.MonsterCount);
        Assert.Equal(7, r.MonsterProcessPosition);
    }

    [Fact]
    public void BothFinalizeBranchesAssignZero()
    {
        // 4092 与 4099 在"整轮跑完且未中断"时都写 0——冗余但保留
        Assert.True(MonsterScanCursorCore.BothFinalizeBranchesAssignZero());

        var r = MonsterScanCursorCore.Finalize(2, 2, false, 0, 0);
        Assert.Equal(0, r.MonGenListPosition);
    }

    // ===================== 多轮稳定性 =====================

    [Fact]
    public void RepeatedFullRoundsKeepCursorAtZero()
    {
        var s = St();

        for (int i = 0; i < 5; i++)
        {
            var r = MonsterScanCursorCore.RunScan(s, new[] { 2, 2 }, NoInterrupt());
            Assert.False(r.Interrupted);
            Assert.Equal(0, s.MonGenListPosition);
            Assert.Equal(0, s.MonGenCertListPosition);
        }
    }

    [Fact]
    public void RepeatedInterruptsEventuallyProgress()
    {
        // 每次中断后保存位递增 → 最终跑完
        var s = St();
        int guard = 0;

        while (guard++ < 10)
        {
            var r = MonsterScanCursorCore.RunScan(s, new[] { 4 }, InterruptAt(0, s.MonGenCertListPosition + 1));
            if (!r.Interrupted)
                break;
        }

        Assert.True(guard < 10);
        Assert.Equal(0, s.MonGenCertListPosition);
    }

    [Fact]
    public void SavedPositionIsConsumedExactlyOnce()
    {
        // 第二轮消费掉保存值后立即清零；第三轮又从 0 开始
        var s = St();
        MonsterScanCursorCore.RunScan(s, new[] { 5 }, InterruptAt(0, 2));
        Assert.Equal(2, s.MonGenCertListPosition);

        var r2 = MonsterScanCursorCore.RunScan(s, new[] { 5 }, NoInterrupt());
        Assert.Equal(2, r2.InnerStartPositions[0]);
        Assert.Equal(0, s.MonGenCertListPosition);   // 已消费

        var r3 = MonsterScanCursorCore.RunScan(s, new[] { 5 }, NoInterrupt());
        Assert.Equal(0, r3.InnerStartPositions[0]);  // **不再续扫**
    }

    [Fact]
    public void TwoMonGensEachGetIndependentStart()
    {
        // 第一个刷怪点中断 → 只在它身上续扫，其它点仍从 0 开始
        var s = St();
        MonsterScanCursorCore.RunScan(s, new[] { 5, 5 }, InterruptAt(0, 3));
        Assert.Equal(3, s.MonGenCertListPosition);

        var r2 = MonsterScanCursorCore.RunScan(s, new[] { 5, 5 }, NoInterrupt());

        Assert.Equal(3, r2.InnerStartPositions[0]);   // 第一个点续扫
        Assert.Equal(0, r2.InnerStartPositions[1]);   // 第二个点从 0
    }

    [Fact]
    public void ProcessPositionAccumulatesAcrossMonGens()
    {
        var s = St();
        MonsterScanCursorCore.RunScan(s, new[] { 2, 3, 4 }, NoInterrupt());

        Assert.Equal(9, s.MonsterCount);   // 2 + 3 + 4
    }

    [Fact]
    public void StatsResetEachFullRound()
    {
        var s = St();

        MonsterScanCursorCore.RunScan(s, new[] { 2 }, NoInterrupt());
        Assert.Equal(2, s.MonsterCount);

        MonsterScanCursorCore.RunScan(s, new[] { 5 }, NoInterrupt());
        Assert.Equal(5, s.MonsterCount);   // 不是 7——每轮归零后重算
    }
}
