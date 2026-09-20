using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// RunGateUtilsHeartbeat.cs 测试 —— 覆盖 RunGateUtils.pas:4259-4311（OnTimerCheckConnect 活分支）、
/// 1837-1979（TRunGate.Run 的清列表/还原防御/挑战应答）、GateShare.pas:1458-1464（tick_diff）。
/// </summary>
public class RunGateUtilsHeartbeatTests
{
    // ---------------- tick_diff ----------------

    [Fact]
    public void TickDiff_PlainSubtraction()
    {
        Assert.Equal(0u, RunGateTiming.TickDiff(100, 100));
        Assert.Equal(50u, RunGateTiming.TickDiff(100, 150));
    }

    [Fact]
    public void TickDiff_WrapsAround32Bits()
    {
        // 原 GateShare.pas:1460-1463：end < start 时走 High(Cardinal) - start + end
        Assert.Equal(10u, RunGateTiming.TickDiff(0xFFFFFFF6u, 0u));      // 10 ms 后回绕
        Assert.Equal(1u, RunGateTiming.TickDiff(uint.MaxValue, 0u));
        Assert.Equal(5u, RunGateTiming.TickDiff(uint.MaxValue - 4, 0u));
    }

    // ---------------- 心跳 / 重连 ----------------

    [Fact]
    public void ReconnectInterval_Is8000_AndBoundaryIsInclusive()
    {
        Assert.Equal(8000u, RunGateTiming.ReconnectIntervalMs);
        Assert.True(RunGateTiming.ShouldTryReconnect(1000, 9000));
        Assert.False(RunGateTiming.ShouldTryReconnect(1000, 8999));
        Assert.True(RunGateTiming.ShouldTryReconnect(uint.MaxValue - 3999, 4000));   // 回绕后正好 8000
    }

    [Fact]
    public void CheckClientInterval_IsQuarterOfTimeoutWith45sFloor()
    {
        // 原 4299：Max(g_dwCheckServerTimeOutTime div 4 * 1000, 45000)
        Assert.Equal(75000u, RunGateTiming.CheckClientIntervalMs(300));   // 默认值 → 75s
        Assert.Equal(45000u, RunGateTiming.CheckClientIntervalMs(180));   // 45s 正好触底
        Assert.Equal(45000u, RunGateTiming.CheckClientIntervalMs(100));   // 25s < 45s → 取 45s
        Assert.Equal(45000u, RunGateTiming.CheckClientIntervalMs(0));     // 0 -> 0 -> 45000
        Assert.Equal(46000u, RunGateTiming.CheckClientIntervalMs(184));   // 46s
        Assert.Equal(250000u, RunGateTiming.CheckClientIntervalMs(1000)); // 250s
    }

    [Fact]
    public void CheckClientInterval_IntegerDivisionBeforeMultiply()
    {
        // 是 `div 4 * 1000` 而不是 `* 1000 / 4`：303 秒 → 75*1000 = 75000；299 秒 → 74*1000 = 74000
        Assert.Equal(75000u, RunGateTiming.CheckClientIntervalMs(303));
        Assert.Equal(74000u, RunGateTiming.CheckClientIntervalMs(299));
    }

    [Fact]
    public void ShouldSendCheckClient_UsesInclusiveComparison()
    {
        Assert.True(RunGateTiming.ShouldSendCheckClient(0, 75000, 300));
        Assert.False(RunGateTiming.ShouldSendCheckClient(0, 74999, 300));
    }

    [Fact]
    public void ServerCheckTimeout_IsStrictlyGreater()
    {
        // 原 4305 用的是 `>`（而心跳用的是 `>=`）—— 差异断言
        Assert.Equal(300000u, RunGateTiming.DefaultCheckServerTimeOutSeconds * 1000);
        Assert.False(RunGateTiming.IsServerCheckTimeout(0, 300000, 300));
        Assert.True(RunGateTiming.IsServerCheckTimeout(0, 300001, 300));

        // 对比：同一个 tick 差值下心跳判据会先触发
        Assert.True(RunGateTiming.ShouldSendCheckClient(0, 300000, 300));
    }

    [Fact]
    public void AutoClearTemp_IsStrictlyGreaterAndGatedByFlag()
    {
        Assert.False(RunGateTiming.ShouldAutoClearTemp(true, 0, 120000, 120));
        Assert.True(RunGateTiming.ShouldAutoClearTemp(true, 0, 120001, 120));
        Assert.False(RunGateTiming.ShouldAutoClearTemp(false, 0, 999999999, 120));
    }

    [Fact]
    public void RestoreDefense_IsStrictlyGreaterAndGatedByFlag()
    {
        Assert.False(RunGateTiming.ShouldRestoreDefense(true, 0, 120000, 120));
        Assert.True(RunGateTiming.ShouldRestoreDefense(true, 0, 120001, 120));
        Assert.False(RunGateTiming.ShouldRestoreDefense(false, 0, 120001, 120));
    }

    [Fact]
    public void NeedsDefenseLevelRestore_OnlyWhenDifferent()
    {
        Assert.False(RunGateTiming.NeedsDefenseLevelRestore(1, 1));
        Assert.True(RunGateTiming.NeedsDefenseLevelRestore(3, 1));
        Assert.True(RunGateTiming.NeedsDefenseLevelRestore(1, 3));
        Assert.False(RunGateTiming.NeedsDefenseLevelRestore(0, 0));
    }

    [Fact]
    public void Defaults_MatchGateShare()
    {
        Assert.Equal(300u, RunGateTiming.DefaultCheckServerTimeOutSeconds);
        Assert.Equal(45000u, RunGateTiming.MinCheckClientIntervalMs);
        Assert.Equal(120u, RunGateTiming.DefaultAutoClearTempSeconds);
        Assert.Equal(120u, RunGateTiming.DefaultRestoreDefenseSeconds);
        Assert.Equal(1u, RunGateTiming.DefaultDefenseLevel);
    }

    // ---------------- 挑战应答状态机 ----------------

    [Fact]
    public void ChallengeState_InitiallyInactive_AndNeverResponds()
    {
        var st = new RunGateChallengeState(1000);
        Assert.False(st.Active);
        Assert.Equal(0u, st.ResponseTime);
        Assert.Equal(0, st.ResponseCount);
        Assert.False(st.ShouldRespond(999999));
    }

    [Fact]
    public void ChallengeState_OnChallenge_ArmsWithGivenResponseTime()
    {
        var st = new RunGateChallengeState();
        st.OnChallenge(now: 5000, responseTime: 2000, data1: 0xAABBCCDD, data2: 0x11223344);
        Assert.True(st.Active);
        Assert.Equal(5000u, st.ChallengeTick);
        Assert.Equal(2000u, st.ResponseTime);
        Assert.Equal(0xAABBCCDDu, st.Data1);
        Assert.Equal(0x11223344u, st.Data2);
        Assert.Equal(0, st.ResponseCount);
    }

    [Fact]
    public void ChallengeState_RespondBoundary()
    {
        var st = new RunGateChallengeState();
        st.OnChallenge(1000, 500, 1, 2);
        Assert.False(st.ShouldRespond(1499));
        Assert.True(st.ShouldRespond(1500));
        Assert.True(st.ShouldRespond(2000));
    }

    [Fact]
    public void ChallengeState_TwoResponsesThenStops_AndEachAdds60s()
    {
        var st = new RunGateChallengeState();
        st.OnChallenge(1000, 500, 1, 2);

        Assert.True(st.ShouldRespond(1500));
        st.AfterRespond();
        Assert.Equal(1, st.ResponseCount);
        Assert.Equal(60500u, st.ResponseTime);
        Assert.True(st.Active);

        // 下一次要等到 1000 + 60500
        Assert.False(st.ShouldRespond(60000));
        Assert.True(st.ShouldRespond(61500));
        st.AfterRespond();
        Assert.Equal(2, st.ResponseCount);
        Assert.False(st.Active);                                  // 原 1974-1977
        Assert.False(st.ShouldRespond(99999999));
    }

    [Fact]
    public void ChallengeState_ReChallengeResetsCountAndReArms()
    {
        var st = new RunGateChallengeState();
        st.OnChallenge(1000, 100, 1, 2);
        st.AfterRespond();
        st.AfterRespond();
        Assert.False(st.Active);

        st.OnChallenge(50000, 100, 9, 8);
        Assert.True(st.Active);
        Assert.Equal(0, st.ResponseCount);
        Assert.Equal(100u, st.ResponseTime);
        Assert.True(st.ShouldRespond(50100));
    }

    [Fact]
    public void ChallengeState_ResponseTimeWrapsWithoutThrowing()
    {
        var st = new RunGateChallengeState();
        uint start = uint.MaxValue - 10000;                       // 4294957295
        st.OnChallenge(0, start, 1, 2);
        st.AfterRespond();                                        // +60000 → 越过 2^32 回绕
        Assert.Equal(unchecked(start + 60000u), st.ResponseTime);
        Assert.Equal(49999u, st.ResponseTime);                    // 4295017295 - 2^32
    }

    [Fact]
    public void ChallengeState_Reset_ClearsEverything()
    {
        var st = new RunGateChallengeState();
        st.OnChallenge(1000, 100, 7, 8);
        st.Reset();
        Assert.False(st.Active);
        Assert.Equal(0, st.ResponseCount);
        Assert.Equal(0u, st.ResponseTime);
        Assert.Equal(0u, st.Data1);
        Assert.Equal(0u, st.Data2);
    }
}
