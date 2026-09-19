using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J105：怪物调度节流（UsrEngn.pas 4112-4164 + ObjBase.pas 11375-11376）1:1 测试。
/// </summary>
public sealed class MonsterScheduleThrottleCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void DefaultIntervalIsTwo()
    {
        // M2Share.pas 4576
        Assert.Equal(2, MonsterScheduleThrottleCore.DefaultProcessMonsterInterval);
    }

    [Fact]
    public void GraceIsTenMinutes()
    {
        // 4146：600000ms
        Assert.Equal(600_000u, MonsterScheduleThrottleCore.ClearHumOrBBTickGraceMs);
    }

    [Fact]
    public void SixCombatRefsAreCleared()
    {
        Assert.Equal(6, MonsterScheduleThrottleCore.ClearedCombatRefs.Length);
        Assert.Equal("m_CurrTarget", MonsterScheduleThrottleCore.ClearedCombatRefs[0]);
        Assert.Equal("m_CurrTargetEx", MonsterScheduleThrottleCore.ClearedCombatRefs[5]);
    }

    // ===================== 第一道：视野重搜（4115-4136） =====================

    [Fact]
    public void SearchSkippedWhenIntervalNotElapsed()
    {
        // 4115：严格大于，恰好相等时**不**重搜
        Assert.Equal(MonsterScheduleThrottleCore.SearchStep.Skip,
            MonsterScheduleThrottleCore.SelectSearchStep(
                dwCurrentTick: 1000, searchTick: 900, searchTime: 100,
                penvirIsNull: false, humCount: 1, humBBCount: 0, poisonDechealthTime: 0));
    }

    [Fact]
    public void SearchRunsWhenStrictlyGreater()
    {
        Assert.Equal(MonsterScheduleThrottleCore.SearchStep.RefreshTickAndSearch,
            MonsterScheduleThrottleCore.SelectSearchStep(1001, 900, 100, false, 1, 0, 0));
    }

    [Fact]
    public void SearchSkippedWhenEnvirNil()
    {
        // 4117
        Assert.Equal(MonsterScheduleThrottleCore.SearchStep.Skip,
            MonsterScheduleThrottleCore.SelectSearchStep(9999, 0, 100, penvirIsNull: true, 1, 0, 0));
    }

    [Fact]
    public void SearchWhenHumansPresent()
    {
        Assert.Equal(MonsterScheduleThrottleCore.SearchStep.RefreshTickAndSearch,
            MonsterScheduleThrottleCore.SelectSearchStep(9999, 0, 100, false, humCount: 1, 0, 0));
    }

    [Fact]
    public void SearchWhenBBHumansPresent()
    {
        // 4119：HumBBCount > 0 也算有人
        Assert.Equal(MonsterScheduleThrottleCore.SearchStep.RefreshTickAndSearch,
            MonsterScheduleThrottleCore.SelectSearchStep(9999, 0, 100, false, 0, humBBCount: 1, 0));
    }

    [Fact]
    public void SearchWhenPoisoned()
    {
        // 4119：中毒也算
        Assert.Equal(MonsterScheduleThrottleCore.SearchStep.RefreshTickAndSearch,
            MonsterScheduleThrottleCore.SelectSearchStep(9999, 0, 100, false, 0, 0, poisonDechealthTime: 1));
    }

    [Fact]
    public void ClearWhenNobodyAround()
    {
        // 4126-4134：无人 → 清理
        Assert.Equal(MonsterScheduleThrottleCore.SearchStep.ClearAndReset,
            MonsterScheduleThrottleCore.SelectSearchStep(9999, 0, 100, false, 0, 0, 0));
    }

    [Fact]
    public void SearchStepHasThreeOutcomes()
    {
        // 三种结果互不相同
        var skip = MonsterScheduleThrottleCore.SelectSearchStep(0, 0, 100, false, 1, 0, 0);
        var search = MonsterScheduleThrottleCore.SelectSearchStep(9999, 0, 100, false, 1, 0, 0);
        var clear = MonsterScheduleThrottleCore.SelectSearchStep(9999, 0, 100, false, 0, 0, 0);

        Assert.NotEqual(skip, search);
        Assert.NotEqual(search, clear);
        Assert.NotEqual(skip, clear);
    }

    // ===================== 第二道：节流（4138） =====================

    [Fact]
    public void ThrottleWhenInvisibleAndBelowLimit()
    {
        Assert.True(MonsterScheduleThrottleCore.ShouldThrottle(
            boIsVisibleActive: false, processRunCount: 0, processMonsterInterval: 2));
    }

    [Fact]
    public void NoThrottleWhenVisibleActive()
    {
        // 视野内有人 → 不节流
        Assert.False(MonsterScheduleThrottleCore.ShouldThrottle(true, 0, 2));
    }

    [Fact]
    public void NoThrottleWhenCountReachedLimit()
    {
        Assert.False(MonsterScheduleThrottleCore.ShouldThrottle(false, 2, 2));
        Assert.False(MonsterScheduleThrottleCore.ShouldThrottle(false, 3, 2));
    }

    [Fact]
    public void ThrottleBoundaryIsStrictLessThan()
    {
        Assert.True(MonsterScheduleThrottleCore.ShouldThrottle(false, 1, 2));
        Assert.False(MonsterScheduleThrottleCore.ShouldThrottle(false, 2, 2));
    }

    [Fact]
    public void IntervalZeroMeansNeverThrottle()
    {
        // interval = 0 → count < 0 永假
        Assert.False(MonsterScheduleThrottleCore.ShouldThrottle(false, 0, 0));
    }

    // ===================== 第三道：是否运行（4144-4147） =====================

    [Fact]
    public void RunWhenHumansPresent()
    {
        Assert.True(MonsterScheduleThrottleCore.ShouldRunMonster(
            false, humCount: 1, humBBCount: 0, now: 0, clearHumOrBBTick: 0, false, false, 0));
    }

    [Fact]
    public void RunWhenWithinGraceWindow()
    {
        // 4146：不超过 600000ms
        Assert.True(MonsterScheduleThrottleCore.ShouldRunMonster(
            false, 0, 0, now: 600_000, clearHumOrBBTick: 0, false, false, 0));
    }

    [Fact]
    public void GraceBoundaryIsInclusive()
    {
        // 恰好 600000 仍在窗口内（<=）
        Assert.True(MonsterScheduleThrottleCore.ShouldRunMonster(false, 0, 0, 600_000, 0, false, false, 0));
        Assert.False(MonsterScheduleThrottleCore.ShouldRunMonster(false, 0, 0, 600_001, 0, false, false, 0));
    }

    [Fact]
    public void RunWhenGhost()
    {
        Assert.True(MonsterScheduleThrottleCore.ShouldRunMonster(false, 0, 0, 999_999, 0, boGhost: true, false, 0));
    }

    [Fact]
    public void RunWhenDead()
    {
        Assert.True(MonsterScheduleThrottleCore.ShouldRunMonster(false, 0, 0, 999_999, 0, false, boDeath: true, 0));
    }

    [Fact]
    public void RunWhenPoisoned()
    {
        Assert.True(MonsterScheduleThrottleCore.ShouldRunMonster(false, 0, 0, 999_999, 0, false, false, poisonDechealthTime: 1));
    }

    [Fact]
    public void NoRunWhenEnvirNil()
    {
        // 4144：m_PEnvir <> nil 是前置
        Assert.False(MonsterScheduleThrottleCore.ShouldRunMonster(
            penvirIsNull: true, humCount: 99, 99, 0, 0, true, true, 99));
    }

    [Fact]
    public void NoRunWhenAloneAndGraceExpired()
    {
        // 无人、超 10 分钟、非 Ghost/死亡/中毒
        Assert.False(MonsterScheduleThrottleCore.ShouldRunMonster(false, 0, 0, 600_001, 0, false, false, 0));
    }

    // ===================== 综合决策（4138-4163） =====================

    [Fact]
    public void ThrottleTakesPriorityOverRun()
    {
        // **关键**：第二道命中时**不再看第三道**，即使地图有人也不 Run
        Assert.Equal(MonsterScheduleThrottleCore.MonsterStep.IncrementAndSkip,
            MonsterScheduleThrottleCore.SelectMonsterStep(
                boIsVisibleActive: false, processRunCount: 0, processMonsterInterval: 2,
                penvirIsNull: false, humCount: 100, humBBCount: 0,
                now: 0, clearHumOrBBTick: 0, boGhost: false, boDeath: false, poisonDechealthTime: 0));
    }

    [Fact]
    public void VisibleActiveSkipsThrottleAndRuns()
    {
        Assert.Equal(MonsterScheduleThrottleCore.MonsterStep.ResetAndRun,
            MonsterScheduleThrottleCore.SelectMonsterStep(
                true, 0, 2, false, 1, 0, 0, 0, false, false, 0));
    }

    [Fact]
    public void CountAtLimitGoesToRun()
    {
        Assert.Equal(MonsterScheduleThrottleCore.MonsterStep.ResetAndRun,
            MonsterScheduleThrottleCore.SelectMonsterStep(
                false, 2, 2, false, 1, 0, 0, 0, false, false, 0));
    }

    [Fact]
    public void SilentClearWhenNoReasonToRun()
    {
        Assert.Equal(MonsterScheduleThrottleCore.MonsterStep.SilentClear,
            MonsterScheduleThrottleCore.SelectMonsterStep(
                false, 2, 2, false, 0, 0, 600_001, 0, false, false, 0));
    }

    [Fact]
    public void DoNothingWhenEnvirNilAndNoThrottle()
    {
        // 4154/4163 都要求 m_PEnvir <> nil
        Assert.Equal(MonsterScheduleThrottleCore.MonsterStep.DoNothing,
            MonsterScheduleThrottleCore.SelectMonsterStep(
                true, 0, 2, penvirIsNull: true, 0, 0, 0, 0, false, false, 0));
    }

    [Fact]
    public void EnvirNilStillThrottlesWhenCountBelowLimit()
    {
        // 第二道不检查 m_PEnvir，故仍会加计数
        Assert.Equal(MonsterScheduleThrottleCore.MonsterStep.IncrementAndSkip,
            MonsterScheduleThrottleCore.SelectMonsterStep(
                false, 0, 2, penvirIsNull: true, 0, 0, 0, 0, false, false, 0));
    }

    // ===================== 计数轨迹 =====================

    [Fact]
    public void ApplyIncrementsOnlyOnThrottle()
    {
        Assert.Equal(1, MonsterScheduleThrottleCore.ApplyProcessRunCount(
            MonsterScheduleThrottleCore.MonsterStep.IncrementAndSkip, 0));
    }

    [Fact]
    public void ApplyResetsOnlyOnRun()
    {
        Assert.Equal(0, MonsterScheduleThrottleCore.ApplyProcessRunCount(
            MonsterScheduleThrottleCore.MonsterStep.ResetAndRun, 5));
    }

    [Fact]
    public void ApplyKeepsValueOnSilentClear()
    {
        // **关键**：静默清理**不清零**（4154-4163 未清零）
        Assert.Equal(5, MonsterScheduleThrottleCore.ApplyProcessRunCount(
            MonsterScheduleThrottleCore.MonsterStep.SilentClear, 5));
    }

    [Fact]
    public void ApplyKeepsValueOnDoNothing()
    {
        Assert.Equal(5, MonsterScheduleThrottleCore.ApplyProcessRunCount(
            MonsterScheduleThrottleCore.MonsterStep.DoNothing, 5));
    }

    [Fact]
    public void SilentClearDoesNotResetIsDistinctFromRun()
    {
        // **差异保护**：SilentClear 与 ResetAndRun 对计数的影响相反
        Assert.NotEqual(
            MonsterScheduleThrottleCore.ApplyProcessRunCount(MonsterScheduleThrottleCore.MonsterStep.SilentClear, 7),
            MonsterScheduleThrottleCore.ApplyProcessRunCount(MonsterScheduleThrottleCore.MonsterStep.ResetAndRun, 7));
    }

    [Fact]
    public void InvisibleMonsterCountClimbsToLimit()
    {
        // 无人地图、非 Ghost/死亡/中毒，且超宽限期：
        // 前 2 轮 IncrementAndSkip（0→1→2），此后转为 SilentClear（保持 2）
        var trace = MonsterScheduleThrottleCore.SimulateRunCount(
            rounds: 5, boIsVisibleActive: false, processMonsterInterval: 2,
            penvirIsNull: false, shouldRunConditions: true);   // humCount=1 → 有人

        // 有人时：第一道不影响计数；第二道节流两轮后第三道 Run 清零 → 循环
        Assert.Equal(new[] { 1, 2, 0, 1, 2 }, trace);
    }

    [Fact]
    public void VisibleMonsterNeverAccumulates()
    {
        // 视野内有人 → 永不节流 → 每轮 ResetAndRun → 计数恒为 0
        var trace = MonsterScheduleThrottleCore.SimulateRunCount(
            rounds: 4, boIsVisibleActive: true, processMonsterInterval: 2,
            penvirIsNull: false, shouldRunConditions: true);

        Assert.All(trace, v => Assert.Equal(0, v));
    }

    [Fact]
    public void CountCyclesWhenMonsterKeepsRunning()
    {
        // `SimulateRunCount` 固定 now=0、clearHumOrBBTick=0，故 0-0 <= 600000 恒成立
        // → 一旦通过节流就会走 ResetAndRun。于是形成 1,2,0 的循环。
        var trace = MonsterScheduleThrottleCore.SimulateRunCount(
            rounds: 5, boIsVisibleActive: false, processMonsterInterval: 2,
            penvirIsNull: false, shouldRunConditions: false);

        Assert.Equal(new[] { 1, 2, 0, 1, 2 }, trace);
    }

    [Fact]
    public void CountSaturatesWhenEnvirNil()
    {
        // m_PEnvir = nil：前 2 轮加计数到 2（第二道不检查 m_PEnvir），
        // 之后 DoNothing **保持** 2 —— 这才是真正的饱和
        var trace = MonsterScheduleThrottleCore.SimulateRunCount(
            rounds: 5, boIsVisibleActive: false, processMonsterInterval: 2,
            penvirIsNull: true, shouldRunConditions: false);

        Assert.Equal(new[] { 1, 2, 2, 2, 2 }, trace);
    }

    [Fact]
    public void IntervalThreeGivesLongerCycle()
    {
        // interval = 3 → 三轮加计数后 Run 清零
        var trace = MonsterScheduleThrottleCore.SimulateRunCount(
            rounds: 6, boIsVisibleActive: false, processMonsterInterval: 3,
            penvirIsNull: false, shouldRunConditions: true);

        Assert.Equal(new[] { 1, 2, 3, 0, 1, 2 }, trace);
    }

    [Fact]
    public void IntervalOneBehavesAsThrottleEveryOtherRound()
    {
        var trace = MonsterScheduleThrottleCore.SimulateRunCount(
            rounds: 4, boIsVisibleActive: false, processMonsterInterval: 1,
            penvirIsNull: false, shouldRunConditions: true);

        Assert.Equal(new[] { 1, 0, 1, 0 }, trace);
    }

    // ===================== 初值（ObjBase 11375-11376） =====================

    [Fact]
    public void FreshMonsterDoesNotRunYet()
    {
        // 11375-11376：构造后 m_boIsVisibleActive = False、m_nProcessRunCount = 0。
        // 故新怪物的第一轮**必然**先走一次节流（除非已有人看见它）。
        Assert.True(MonsterScheduleThrottleCore.ShouldThrottle(
            boIsVisibleActive: false, processRunCount: 0,
            processMonsterInterval: MonsterScheduleThrottleCore.DefaultProcessMonsterInterval));
    }

    [Fact]
    public void ClearObjectResetsBothFields()
    {
        // 与 J103 的 ClearObject 呼应：入口先把可见标志置 False（31679），
        // 而 m_nProcessRunCount 只在真正 Run 时清零
        Assert.False(ViewRangeMaintainCore.BoIsVisibleActiveOnEntry);
    }
}
