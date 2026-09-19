using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J106：怪物候选表构建（UsrEngn.pas 3990-4110）+ tick_diff + MakeGhost 1:1 测试。
/// </summary>
public sealed class MonsterListBuildCoreTests
{
    private const uint High = uint.MaxValue;   // High(Cardinal)

    // ===================== tick_diff（M2Share.pas 32953-32959） =====================

    [Fact]
    public void TickDiffNormalForward()
    {
        Assert.Equal(100u, MonsterListBuildCore.TickDiff(1000, 1100));
    }

    [Fact]
    public void TickDiffEqualIsZero()
    {
        Assert.Equal(0u, MonsterListBuildCore.TickDiff(500, 500));
    }

    [Fact]
    public void TickDiffBackwardWraps()
    {
        // 32958：High(Cardinal) - tick_start + tick_end
        // 用 4294967290 → 5 验证：真实间隔 11，但原文公式给出 10
        uint r = MonsterListBuildCore.TickDiff(4294967290u, 5u);
        Assert.Equal(10u, r);
    }

    [Fact]
    public void TickDiffWraparoundIsOneLessThanTrue()
    {
        // **原文既有偏差**：回绕分支比真实差值小 1（用 High 而非 High+1）。
        // 本移植按 1:1 保留，**不"修正"**。
        uint start = 4294967290u;
        uint end = 5u;

        uint asDelphi = MonsterListBuildCore.TickDiff(start, end);
        // 真实经过时间 = (2^32 - start) + end
        ulong trueElapsed = (4294967296UL - start) + end;

        Assert.Equal(trueElapsed - 1, (ulong)asDelphi);
    }

    [Fact]
    public void TickDiffIsNotTwosComplementWrap()
    {
        // 若误用 unchecked(end - start) 会得到 11（真实值），与原文 10 不同。
        uint start = 4294967290u;
        uint end = 5u;

        uint uncheckedForm = unchecked(end - start);
        Assert.NotEqual(uncheckedForm, MonsterListBuildCore.TickDiff(start, end));
        Assert.Equal(11u, uncheckedForm);
    }

    [Fact]
    public void TickDiffZeroStart()
    {
        Assert.Equal(7u, MonsterListBuildCore.TickDiff(0, 7));
    }

    // ===================== 游标起点（4011-4018） =====================

    [Fact]
    public void CertStartUsesSavedPositionWhenInRange()
    {
        Assert.Equal(3, MonsterListBuildCore.SelectCertStartPosition(3, 10));
    }

    [Fact]
    public void CertStartResetsWhenOutOfRange()
    {
        // 4015-4017：m_nMonGenCertListPosition >= Count → 0
        Assert.Equal(0, MonsterListBuildCore.SelectCertStartPosition(10, 10));
        Assert.Equal(0, MonsterListBuildCore.SelectCertStartPosition(99, 10));
    }

    [Fact]
    public void CertStartBoundaryIsStrictLessThan()
    {
        Assert.Equal(9, MonsterListBuildCore.SelectCertStartPosition(9, 10));
        Assert.Equal(0, MonsterListBuildCore.SelectCertStartPosition(10, 10));
    }

    [Fact]
    public void CertStartZeroWhenEmpty()
    {
        Assert.Equal(0, MonsterListBuildCore.SelectCertStartPosition(0, 0));
    }

    // ===================== 智能刷怪（4029-4033） =====================

    [Fact]
    public void MakeGhostRequiresAllThree()
    {
        Assert.True(MonsterListBuildCore.ShouldMakeGhost(false, true, true));
    }

    [Fact]
    public void NoMakeGhostWhenAlreadyGhost()
    {
        Assert.False(MonsterListBuildCore.ShouldMakeGhost(true, true, true));
    }

    [Fact]
    public void NoMakeGhostWhenMonGenDoesNotClear()
    {
        Assert.False(MonsterListBuildCore.ShouldMakeGhost(false, false, true));
    }

    [Fact]
    public void NoMakeGhostWhenEnvirDoesNotClear()
    {
        Assert.False(MonsterListBuildCore.ShouldMakeGhost(false, true, false));
    }

    [Fact]
    public void MakeGhostLifetimeIsFiveMinutes()
    {
        // 4052 注释"5分钟"
        Assert.Equal(300_000u, MonsterListBuildCore.GhostLifetimeMs);
    }

    // ===================== 加入候选表门槛（4039） =====================

    [Fact]
    public void AddWhenRunIntervalExceeded()
    {
        // tick_diff(1000, 1400) = 400 > 250
        Assert.True(MonsterListBuildCore.ShouldAddToRunList(1000, 1400, 250));
    }

    [Fact]
    public void AddBoundaryIsStrictGreater()
    {
        // 恰好相等不加入
        Assert.True(MonsterListBuildCore.ShouldAddToRunList(1000, 1251, 250));
        Assert.False(MonsterListBuildCore.ShouldAddToRunList(1000, 1250, 250));
    }

    [Fact]
    public void AddWhenRunTimeZeroAndTickDiffers()
    {
        Assert.True(MonsterListBuildCore.ShouldAddToRunList(1000, 1001, 0));
        Assert.False(MonsterListBuildCore.ShouldAddToRunList(1000, 1000, 0));
    }

    [Fact]
    public void AddUsesTickDiffSoItWraps()
    {
        // 回绕时 tick_diff 比真实小 1，故 250 的门槛更易被跨过
        uint r = MonsterListBuildCore.TickDiff(4294967290u, 5u);
        Assert.True(MonsterListBuildCore.ShouldAddToRunList(4294967290u, 5u, 5));
        Assert.False(MonsterListBuildCore.ShouldAddToRunList(4294967290u, 5u, 10));
        Assert.Equal(10u, r);
    }

    [Fact]
    public void DefaultMonLimitIsThirty()
    {
        // M2Share.pas 3865
        Assert.Equal(30u, MonsterListBuildCore.DefaultMonLimit);
    }

    [Fact]
    public void MonLimitBoundaryIsStrictGreater()
    {
        Assert.True(MonsterListBuildCore.IsMonLimitReached(1031, 1000, 30));
        Assert.False(MonsterListBuildCore.IsMonLimitReached(1030, 1000, 30));
    }

    // ===================== 幽灵回收（4052） =====================

    [Fact]
    public void GhostExpiryBoundaryIsStrictGreater()
    {
        Assert.True(MonsterListBuildCore.IsGhostExpired(300_001, 0));
        Assert.False(MonsterListBuildCore.IsGhostExpired(300_000, 0));
    }

    [Fact]
    public void GhostExpiryUsesPlainSubtractionNotTickDiff()
    {
        // **差异保护**：4052 用直接相减，4042/4052 之外用 tick_diff。
        // 在回绕场景下两者结论不同，故不可统一。
        uint now = 5u;
        uint ghostTick = 4294967290u;

        // 直接相减：5 - 4294967290 (uint 回绕) = 11 → 未超期
        Assert.False(MonsterListBuildCore.IsGhostExpired(now, ghostTick));

        // 若改用 tick_diff 会得到 10，同样未超期——但二者数值不同，故仍不可互换
        Assert.NotEqual(MonsterListBuildCore.TickDiff(ghostTick, now), unchecked(now - ghostTick));
    }

    [Fact]
    public void GhostExpiryNumericallyDiffersFromTickDiffOnWrap()
    {
        uint now = 5u;
        uint ghostTick = 4294967290u;

        Assert.Equal(11u, unchecked(now - ghostTick));                       // 直接相减
        Assert.Equal(10u, MonsterListBuildCore.TickDiff(ghostTick, now));    // tick_diff
    }

    // ===================== 诊断串（4072） =====================

    [Fact]
    public void MonGenInfoFormat()
    {
        Assert.Equal("测试怪/3/7", MonsterListBuildCore.BuildMonGenInfo("测试怪", 3, 7));
    }

    // ===================== 游标收尾（4088-4104） =====================

    [Fact]
    public void FullSweepResetsPositionAndSetsCount()
    {
        // m_MonGenList.Count (5) <= I (5) → 整轮跑完
        var (pos, count, procPos) = MonsterListBuildCore.FinalizeScanCursor(
            monGenListCount: 5, loopIndex: 5, processLimit: false,
            monsterCount: 0, monsterProcessPostion: 42);

        Assert.Equal(0, pos);       // 未中断 → 0
        Assert.Equal(42, count);    // nMonsterCount := nMonsterProcessPostion
        Assert.Equal(0, procPos);   // 且清零
    }

    [Fact]
    public void LimitedScanKeepsPositionOnSameMonGen()
    {
        // 中断 → m_nMonGenListPosition := I
        var (pos, count, procPos) = MonsterListBuildCore.FinalizeScanCursor(
            monGenListCount: 100, loopIndex: 7, processLimit: true,
            monsterCount: 0, monsterProcessPostion: 42);

        Assert.Equal(7, pos);       // 留在同一刷怪点
        Assert.Equal(0, count);     // 未跑完 → 不更新总数
        Assert.Equal(42, procPos);  // 且不清零
    }

    [Fact]
    public void FullSweepIsNotLimitCase()
    {
        // **差异保护**：整轮跑完与限时中断对三项游标的影响完全不同。
        // 必须用"未跑完"（count > index）的场景对比——否则两者都触发
        // `monsterCount := procPos; procPos := 0`，看不出差异。
        var done = MonsterListBuildCore.FinalizeScanCursor(5, 5, processLimit: false, 0, 42);
        var limited = MonsterListBuildCore.FinalizeScanCursor(
            monGenListCount: 100, loopIndex: 7, processLimit: true, 0, 42);

        // 整轮跑完：游标归零、总数更新、过程计数清零
        Assert.Equal(0, done.MonGenListPosition);
        Assert.Equal(42, done.MonsterCount);
        Assert.Equal(0, done.MonsterProcessPostion);

        // 限时中断：游标留在原刷怪点、总数不变、过程计数保留
        Assert.Equal(7, limited.MonGenListPosition);
        Assert.Equal(0, limited.MonsterCount);
        Assert.Equal(42, limited.MonsterProcessPostion);
    }

    [Fact]
    public void BoundaryIsCountLessOrEqualIndex()
    {
        // 4090：m_MonGenList.Count <= I
        var at = MonsterListBuildCore.FinalizeScanCursor(5, 5, false, 0, 1);
        var below = MonsterListBuildCore.FinalizeScanCursor(6, 5, false, 0, 1);

        Assert.Equal(1, at.MonsterCount);      // 5 <= 5 成立
        Assert.Equal(0, below.MonsterCount);   // 6 <= 5 不成立
    }

    // ===================== 单条凭证决策 =====================

    [Fact]
    public void NilMonsterAction()
    {
        Assert.Equal(MonsterListBuildCore.CertAction.NilMonster,
            MonsterListBuildCore.DecideCertAction(true, false, 0, 0, 0, 0, 0));
    }

    [Fact]
    public void AddedWhenIntervalExceeded()
    {
        Assert.Equal(MonsterListBuildCore.CertAction.AddedToRunList,
            MonsterListBuildCore.DecideCertAction(false, false, 0, 0, 1000, 1400, 250));
    }

    [Fact]
    public void RetainedWhenIntervalNotExceeded()
    {
        Assert.Equal(MonsterListBuildCore.CertAction.GhostRetained,
            MonsterListBuildCore.DecideCertAction(false, false, 0, 0, 1000, 1100, 250));
    }

    [Fact]
    public void GhostRetainedWhenNotExpired()
    {
        Assert.Equal(MonsterListBuildCore.CertAction.GhostRetained,
            MonsterListBuildCore.DecideCertAction(false, true, 0, 100_000, 0, 0, 0));
    }

    [Fact]
    public void GhostRemovedWhenExpired()
    {
        Assert.Equal(MonsterListBuildCore.CertAction.GhostRemovedNoAdvance,
            MonsterListBuildCore.DecideCertAction(false, true, 0, 300_001, 0, 0, 0));
    }

    [Fact]
    public void GhostNeverAddedToRunList()
    {
        // 4035：幽灵不进候选表，即使运行间隔已过
        Assert.Equal(MonsterListBuildCore.CertAction.GhostRetained,
            MonsterListBuildCore.DecideCertAction(false, true, 0, 1000, 1000, 99999, 250));
    }

    [Fact]
    public void OnlyGhostRemovalSkipsAdvance()
    {
        // 4065：只有 GhostRemovedNoAdvance 不递增下标
        Assert.False(MonsterListBuildCore.ShouldAdvancePosition(
            MonsterListBuildCore.CertAction.GhostRemovedNoAdvance));

        Assert.True(MonsterListBuildCore.ShouldAdvancePosition(MonsterListBuildCore.CertAction.NilMonster));
        Assert.True(MonsterListBuildCore.ShouldAdvancePosition(MonsterListBuildCore.CertAction.AddedToRunList));
        Assert.True(MonsterListBuildCore.ShouldAdvancePosition(MonsterListBuildCore.CertAction.GhostRetained));
    }

    // ===================== 内层循环模拟 =====================

    [Fact]
    public void LoopWalksAllCerts()
    {
        var certs = new List<MonsterListBuildCore.SimCert>
        {
            MonsterListBuildCore.SimCert.Normal(1000),
            MonsterListBuildCore.SimCert.Normal(1000),
            MonsterListBuildCore.SimCert.Normal(1000),
        };

        var (added, endPos) = MonsterListBuildCore.SimulateCertLoop(
            certs, 0, currentTick: 2000, runTick: 2000, now: 0,
            monProcTick: 0, monLimit: 30, limitEnabled: false);

        // runTime = 0，tick_diff(1000,2000)=1000 > 0 → 三个都加入
        Assert.Equal(3, added.Count);
        Assert.Equal(3, endPos);
    }

    [Fact]
    public void LoopSkipsNilEntries()
    {
        var certs = new List<MonsterListBuildCore.SimCert>
        {
            MonsterListBuildCore.SimCert.Nil,
            MonsterListBuildCore.SimCert.Normal(1000),
        };

        var (added, endPos) = MonsterListBuildCore.SimulateCertLoop(
            certs, 0, 2000, 2000, 0, 0, 30, false);

        Assert.Single(added);
        Assert.Equal(1, added[0]);   // 只有下标 1 被加入
        Assert.Equal(2, endPos);
    }

    [Fact]
    public void LoopRemovesExpiredGhostWithoutAdvancing()
    {
        var certs = new List<MonsterListBuildCore.SimCert>
        {
            MonsterListBuildCore.SimCert.Ghosted(0),          // 超期 → 删除
            MonsterListBuildCore.SimCert.Normal(1000),
        };

        var (added, endPos) = MonsterListBuildCore.SimulateCertLoop(
            certs, 0, currentTick: 400_000, runTick: 400_000, now: 400_000,
            monProcTick: 0, monLimit: 30, limitEnabled: false);

        // 幽灵被删后，原下标 1 的普通怪落到下标 0，被访问并加入
        Assert.Single(added);
        Assert.Equal(0, added[0]);
    }

    [Fact]
    public void LoopRetainsUnexpiredGhost()
    {
        var certs = new List<MonsterListBuildCore.SimCert>
        {
            MonsterListBuildCore.SimCert.Ghosted(100_000),
        };

        var (added, endPos) = MonsterListBuildCore.SimulateCertLoop(
            certs, 0, 0, 0, now: 100_000, monProcTick: 0, monLimit: 30, limitEnabled: false);

        Assert.Empty(added);
        Assert.Equal(1, endPos);
    }

    [Fact]
    public void LoopStartsFromSavedPosition()
    {
        var certs = new List<MonsterListBuildCore.SimCert>
        {
            MonsterListBuildCore.SimCert.Normal(1000),
            MonsterListBuildCore.SimCert.Normal(1000),
            MonsterListBuildCore.SimCert.Normal(1000),
        };

        var (added, _) = MonsterListBuildCore.SimulateCertLoop(
            certs, startPosition: 2, 2000, 2000, 0, 0, 30, false);

        Assert.Single(added);
        Assert.Equal(2, added[0]);
    }

    [Fact]
    public void LoopBreaksWhenMonLimitReached()
    {
        var certs = new List<MonsterListBuildCore.SimCert>
        {
            MonsterListBuildCore.SimCert.Normal(1000),
            MonsterListBuildCore.SimCert.Normal(1000),
            MonsterListBuildCore.SimCert.Normal(1000),
        };

        // now = 100, monProcTick = 0, limit = 30 → 100 > 30 → 第一条后即中断
        var (added, endPos) = MonsterListBuildCore.SimulateCertLoop(
            certs, 0, currentTick: 2000, runTick: 2000, now: 100,
            monProcTick: 0, monLimit: 30, limitEnabled: true);

        Assert.Single(added);
        Assert.Equal(1, endPos);
    }

    [Fact]
    public void LoopDoesNotBreakWhenLimitDisabled()
    {
        var certs = new List<MonsterListBuildCore.SimCert>
        {
            MonsterListBuildCore.SimCert.Normal(1000),
            MonsterListBuildCore.SimCert.Normal(1000),
        };

        var (added, endPos) = MonsterListBuildCore.SimulateCertLoop(
            certs, 0, 2000, 2000, now: 999_999, monProcTick: 0, monLimit: 30, limitEnabled: false);

        Assert.Equal(2, added.Count);
        Assert.Equal(2, endPos);
    }

    [Fact]
    public void LimitNotCheckedForNilEntries()
    {
        // 4067：if Monster <> nil then —— nil 条目后不判限时
        var certs = new List<MonsterListBuildCore.SimCert>
        {
            MonsterListBuildCore.SimCert.Nil,
            MonsterListBuildCore.SimCert.Normal(1000),
        };

        var (_, endPos) = MonsterListBuildCore.SimulateCertLoop(
            certs, 0, 2000, 2000, now: 100, monProcTick: 0, monLimit: 30, limitEnabled: true);

        // nil 处不中断，普通怪处才中断 → 停在 1（普通怪处理完递增后）
        Assert.Equal(2, endPos);
    }

    [Fact]
    public void LoopHandlesEmptyList()
    {
        var (added, endPos) = MonsterListBuildCore.SimulateCertLoop(
            new List<MonsterListBuildCore.SimCert>(), 0, 0, 0, 0, 0, 30, false);

        Assert.Empty(added);
        Assert.Equal(0, endPos);
    }
}
