using System;

// 源：Source/RunGate/RunGateUtils.pas
//   TRunGateManager.OnTimerCheckConnect   4059-4329（活分支 = 4271-4311 的 FIocpClient 循环）
//   TRunGate.Run                           1828-1981（自动清列表 / 还原防御 / 挑战应答）
//   tick_diff                              GateShare.pas:1458-1464
//   GateShare 配置默认值                    GateShare.pas:1204 / 1264 / 1272-1275
// 只抽判定逻辑；线程/定时器留薄外壳。

namespace GXX.RunGate;

/// <summary>
/// 定时判定原语（纯函数）。所有时间参数都是 <c>MyGetTickCount</c>（WinMM <c>timeGetTime</c>）风格
/// 的 32 位毫秒计数（会回绕），因此一律走 <see cref="TickDiff"/>。
/// </summary>
public static class RunGateTiming
{
    /// <summary>GateShare.pas:1204 —— g_dwCheckServerTimeOutTime 默认 300 秒。</summary>
    public const uint DefaultCheckServerTimeOutSeconds = 300;

    /// <summary>RunGateUtils.pas:4281 / 4259 —— 断线重连尝试间隔 8000 ms。</summary>
    public const uint ReconnectIntervalMs = 8000;

    /// <summary>RunGateUtils.pas:4259 —— 心跳间隔下限 45000 ms。</summary>
    public const uint MinCheckClientIntervalMs = 45000;

    /// <summary>GateShare.pas:1274-1275 —— 自动清动态过滤列表：默认开，间隔 120 秒。</summary>
    public const uint DefaultAutoClearTempSeconds = 120;

    /// <summary>GateShare.pas:1272-1273 —— 无攻击还原防御等级：默认开，间隔 120 秒。</summary>
    public const uint DefaultRestoreDefenseSeconds = 120;

    /// <summary>GateShare.pas:1264 —— 默认防御等级 1。</summary>
    public const uint DefaultDefenseLevel = 1;

    /// <summary>
    /// GateShare.pas:1458-1464 —— <c>tick_diff</c>。
    /// 原文分两支写（<c>end - start</c> / <c>High(Cardinal) - start + end</c>），
    /// 数学上等价于 32 位无符号减法；这里用 <c>unchecked</c> 一次表达，避免把"分支"当成语义。
    /// </summary>
    public static uint TickDiff(uint tickStart, uint tickEnd) => unchecked(tickEnd - tickStart);

    /// <summary>RunGateUtils.pas:4281 —— 断线时是否到了重连时机（<c>&gt;=</c> 8000）。</summary>
    public static bool ShouldTryReconnect(uint lastTryTick, uint now, uint intervalMs = ReconnectIntervalMs)
        => TickDiff(lastTryTick, now) >= intervalMs;

    /// <summary>
    /// RunGateUtils.pas:4299 —— 是否该向 M2 发 GM_CHECKCLIENT 心跳。
    /// 间隔 = <c>Max(超时秒数 div 4 * 1000, 45000)</c>；注意是**整数除法之后**再乘 1000。
    /// 默认 300 秒 → <c>Max(75000, 45000) = 75000</c>。
    /// </summary>
    public static uint CheckClientIntervalMs(uint checkServerTimeOutSeconds)
        => Math.Max(checkServerTimeOutSeconds / 4 * 1000, MinCheckClientIntervalMs);

    /// <summary>RunGateUtils.pas:4299 —— <c>&gt;=</c> 判据。</summary>
    public static bool ShouldSendCheckClient(uint lastSendTick, uint now, uint checkServerTimeOutSeconds)
        => TickDiff(lastSendTick, now) >= CheckClientIntervalMs(checkServerTimeOutSeconds);

    /// <summary>RunGateUtils.pas:4305 —— 服务器检测超时判据是**严格大于**（<c>&gt;</c>），勿改成 &gt;=。</summary>
    public static bool IsServerCheckTimeout(uint lastRecvTick, uint now, uint checkServerTimeOutSeconds)
        => TickDiff(lastRecvTick, now) > checkServerTimeOutSeconds * 1000;

    /// <summary>RunGateUtils.pas:1839 —— 自动清动态过滤列表：严格大于。</summary>
    public static bool ShouldAutoClearTemp(bool enabled, uint lastTick, uint now, uint intervalSeconds)
        => enabled && TickDiff(lastTick, now) > intervalSeconds * 1000;

    /// <summary>RunGateUtils.pas:1856 —— 无攻击还原防御等级：严格大于。</summary>
    public static bool ShouldRestoreDefense(bool enabled, uint lastTick, uint now, uint intervalSeconds)
        => enabled && TickDiff(lastTick, now) > intervalSeconds * 1000;

    /// <summary>
    /// RunGateUtils.pas:1858-1862 —— 是否需要真的改写当前防御等级。
    /// 只有当配置值 != 当前值时才写并打日志（<c>dwResotreDefenseTick</c> 无论是否改写都会刷新）。
    /// </summary>
    public static bool NeedsDefenseLevelRestore(uint configuredLevel, uint currentLevel)
        => configuredLevel != currentLevel;
}

/// <summary>
/// RunGateUtils.pas:1867-1979 —— M2→网关「合法性挑战」的应答状态机。
/// <para>
/// 生命周期：M2 在 GM_DATA 里发 <c>SM_CHECK_RUNGATE2</c>（ndIdent 命中）→
/// <see cref="OnChallenge"/> 记录 tick/等待时长/两个数据字并把计数清零、置 Active；
/// 之后 <see cref="ShouldRespond"/> 每满足一次就 <see cref="AfterRespond"/>，
/// 等待时长每次 +60 秒，应答 2 次后 <c>FRecvCheckToM2 := False</c>。
/// </para>
/// </summary>
public sealed class RunGateChallengeState
{
    /// <summary>RunGateUtils.pas:1439-1444 —— 构造初值：Tick=now, ResponseTime=0, Active=false。</summary>
    public uint ChallengeTick { get; private set; }
    public uint ResponseTime { get; private set; }
    public uint Data1 { get; private set; }
    public uint Data2 { get; private set; }
    public int ResponseCount { get; private set; }
    public bool Active { get; private set; }

    public RunGateChallengeState(uint now = 0)
    {
        ChallengeTick = now;
        ResponseTime = 0;
        Active = false;
    }

    /// <summary>RunGateUtils.pas:630-637 / 841-848 —— 收到 SM_CHECK_RUNGATE2。</summary>
    public void OnChallenge(uint now, uint responseTime, uint data1, uint data2)
    {
        ChallengeTick = now;
        ResponseTime = responseTime;
        Data1 = data1;
        Data2 = data2;
        ResponseCount = 0;
        Active = true;
    }

    /// <summary>RunGateUtils.pas:1868 —— <c>Active and (tick_diff(Tick, now) &gt;= ResponseTime)</c>。</summary>
    public bool ShouldRespond(uint now) => Active && RunGateTiming.TickDiff(ChallengeTick, now) >= ResponseTime;

    /// <summary>RunGateUtils.pas:1970-1977 —— 应答一次：计数 +1、等待 +60s、达到 2 次则停。</summary>
    public void AfterRespond()
    {
        ResponseCount++;
        ResponseTime = unchecked(ResponseTime + RunGateChallengeHash.ResponseTimeStep);
        if (ResponseCount >= RunGateChallengeHash.RequiredResponseCount)
            Active = false;
    }

    /// <summary>RunGateUtils.pas:1531（StopServices / 断线）—— 不显式复位；此处提供显式复位用于测试。</summary>
    public void Reset()
    {
        Active = false;
        ResponseCount = 0;
        ResponseTime = 0;
        Data1 = 0;
        Data2 = 0;
    }
}
