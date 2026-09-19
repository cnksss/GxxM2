using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J113：副本人数统计与回收（UsrEngn.pas 10455-10603 + Envir.pas 304-316/3564-3574）1:1 测试。
/// </summary>
public sealed class FbRecycleCoreTests
{
    private static FbRecycleCore.FbRecycleState St(
        int count = 0, bool created = false, int noHumMs = 10_000)
        => new()
        {
            PlayObjectCount = count,
            BoFBCreate = created,
            FbNoHumClearMs = noHumMs,
        };

    private static FbRecycleCore.FbMonster Mon(
        string name = "怪", bool ghost = false, bool death = false, object? master = null)
        => new() { Name = name, BoGhost = ghost, BoDeath = death, Master = master };

    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.Equal(0, FbRecycleCore.CheckMonsterTickReset);
        Assert.Equal(60_000, FbRecycleCore.CheckMonsterIntervalMs);
        Assert.Equal(1_000, FbRecycleCore.NoPlayObjectRenewMs);
        Assert.Equal(60_000, FbRecycleCore.CreateGraceMs);
        Assert.Equal(10, FbRecycleCore.DefaultNoHumClearMin);
        Assert.Equal(0, FbRecycleCore.DefaultPlayObjectCount);
    }

    [Fact]
    public void ConstructDefaults()
    {
        // 3567-3568 / 3574
        var (fail, failTime) = FbRecycleCore.DefaultFailState();
        Assert.False(fail);
        Assert.Equal(0, failTime);
    }

    [Fact]
    public void CreaterOfflineGraceIsOneMinute()
    {
        Assert.Equal(60_000, FbRecycleCore.CreaterOfflineGraceMs);
        Assert.Equal(FbRecycleCore.CreateGraceMs, FbRecycleCore.CreaterOfflineGraceMs);
    }

    // ===================== 人数统计（10456-10483） =====================

    [Fact]
    public void ResetZeroesAllInstances()
    {
        var a = St(count: 5);
        var b = St(count: 9);
        var lists = new List<IList<FbRecycleCore.FbRecycleState>>
        {
            new List<FbRecycleCore.FbRecycleState> { a },
            new List<FbRecycleCore.FbRecycleState> { b },
        };

        FbRecycleCore.ResetAllPlayObjectCounts(lists, x => x);

        Assert.Equal(0, a.PlayObjectCount);
        Assert.Equal(0, b.PlayObjectCount);
    }

    [Fact]
    public void ResetAcrossMultiplePools()
    {
        var a = St(count: 1);
        var b = St(count: 2);
        var c = St(count: 3);
        var lists = new List<IList<FbRecycleCore.FbRecycleState>>
        {
            new List<FbRecycleCore.FbRecycleState> { a, b },
            new List<FbRecycleCore.FbRecycleState> { c },
        };

        FbRecycleCore.ResetAllPlayObjectCounts(lists, x => x);

        Assert.Equal(0, a.PlayObjectCount);
        Assert.Equal(0, b.PlayObjectCount);
        Assert.Equal(0, c.PlayObjectCount);
    }

    [Fact]
    public void ResetEmptyIsNoOp()
    {
        FbRecycleCore.ResetAllPlayObjectCounts(
            new List<IList<FbRecycleCore.FbRecycleState>>(), x => x);
    }

    [Fact]
    public void CountThreeConditions()
    {
        // 10477：(not ghost) and (envir <> nil) and envir.boFB
        Assert.True(FbRecycleCore.ShouldCountPlayObject(false, true, true));
        Assert.False(FbRecycleCore.ShouldCountPlayObject(true, true, true));    // 幽灵
        Assert.False(FbRecycleCore.ShouldCountPlayObject(false, false, true));  // 无地图
        Assert.False(FbRecycleCore.ShouldCountPlayObject(false, true, false));  // **非副本地图**
    }

    [Fact]
    public void NonFbMapPlayersAreNotCounted()
    {
        // 第三条件是"地图是副本地图"，故普通地图上的人不计入任何实例
        var fb = St();
        var players = new List<Player?>
        {
            new() { Ghost = false, State = fb, IsFb = false },
        };

        FbRecycleCore.CountPlayObjects(players, p => p.Ghost, p => p.State, p => p.IsFb);

        Assert.Equal(0, fb.PlayObjectCount);
    }

    [Fact]
    public void CountsPlayersInFbMap()
    {
        var fb = St();
        var players = new List<Player?>
        {
            new() { Ghost = false, State = fb, IsFb = true },
            new() { Ghost = false, State = fb, IsFb = true },
        };

        FbRecycleCore.CountPlayObjects(players, p => p.Ghost, p => p.State, p => p.IsFb);

        Assert.Equal(2, fb.PlayObjectCount);
    }

    [Fact]
    public void GhostPlayersNotCounted()
    {
        var fb = St();
        var players = new List<Player?>
        {
            new() { Ghost = true, State = fb, IsFb = true },
            new() { Ghost = false, State = fb, IsFb = true },
        };

        FbRecycleCore.CountPlayObjects(players, p => p.Ghost, p => p.State, p => p.IsFb);

        Assert.Equal(1, fb.PlayObjectCount);
    }

    [Fact]
    public void NullPlayerSkipped()
    {
        // 10475-10476
        var fb = St();
        var players = new List<Player?> { null, new() { Ghost = false, State = fb, IsFb = true } };

        FbRecycleCore.CountPlayObjects(players, p => p.Ghost, p => p.State, p => p.IsFb);

        Assert.Equal(1, fb.PlayObjectCount);
    }

    [Fact]
    public void PlayersInDifferentInstancesCountedSeparately()
    {
        var a = St();
        var b = St();
        var players = new List<Player?>
        {
            new() { Ghost = false, State = a, IsFb = true },
            new() { Ghost = false, State = b, IsFb = true },
            new() { Ghost = false, State = b, IsFb = true },
        };

        FbRecycleCore.CountPlayObjects(players, p => p.Ghost, p => p.State, p => p.IsFb);

        Assert.Equal(1, a.PlayObjectCount);
        Assert.Equal(2, b.PlayObjectCount);
    }

    [Fact]
    public void ResetThenCountGivesFreshSnapshot()
    {
        // 两段式：先清零再重算，故旧值不残留
        var fb = St(count: 99);
        var lists = new List<IList<FbRecycleCore.FbRecycleState>>
        {
            new List<FbRecycleCore.FbRecycleState> { fb },
        };

        FbRecycleCore.ResetAllPlayObjectCounts(lists, x => x);

        var players = new List<Player?> { new() { Ghost = false, State = fb, IsFb = true } };
        FbRecycleCore.CountPlayObjects(players, p => p.Ghost, p => p.State, p => p.IsFb);

        Assert.Equal(1, fb.PlayObjectCount);   // 不是 99 + 1
    }

    private sealed class Player
    {
        public bool Ghost;
        public FbRecycleCore.FbRecycleState? State;
        public bool IsFb;
    }

    // ===================== 路径一：失败超时（10516-10521） =====================

    [Fact]
    public void FailReleaseRequiresStrictGreater()
    {
        // 10516：`MyGetTickCount > m_dwFBFailTime` —— **严格大于**
        var s = St();
        s.BoFBFail = true;
        s.FbFailTime = 1000;

        Assert.False(FbRecycleCore.ShouldReleaseOnFail(s, 1000));   // 相等不释放
        Assert.True(FbRecycleCore.ShouldReleaseOnFail(s, 1001));
    }

    [Fact]
    public void FailReleaseRequiresFailFlag()
    {
        var s = St();
        s.FbFailTime = 1000;

        Assert.False(FbRecycleCore.ShouldReleaseOnFail(s, 9999));
    }

    [Fact]
    public void FailReleaseClearsThreeFields()
    {
        // 10518-10520
        var s = St(created: true);
        s.FbMasterObject = new object();
        s.FbCheckMonsterTick = 55555;

        FbRecycleCore.ApplyFailRelease(s);

        Assert.Null(s.FbMasterObject);
        Assert.False(s.BoFBCreate);
        Assert.Equal(0, s.FbCheckMonsterTick);
    }

    [Fact]
    public void FailReleaseReopensCheckGate()
    {
        // **关键连锁**：归零 tick 后 10565 的门控立即成立
        var s = St();
        s.FbCheckMonsterTick = 50_000;

        Assert.False(FbRecycleCore.ShouldCheckMonsters(s, 1000));   // 归零前不成立

        FbRecycleCore.ApplyFailRelease(s);

        Assert.True(FbRecycleCore.ShouldCheckMonsters(s, 1000));    // 归零后成立
    }

    [Fact]
    public void FailReleaseDoesNotClearMonsters()
    {
        // 路径一自身不清怪，交给清怪块
        var s = St();
        s.FbMonsterList.Add(Mon("a"));

        FbRecycleCore.ApplyFailRelease(s);

        Assert.Single(s.FbMonsterList);
    }

    // ===================== 路径二外层条件（10522-10523） =====================

    [Fact]
    public void ExpiryBlockOpensAfterGracePlusDelay()
    {
        // 10522：now >= FbCreateTime + 60000 + FbEnterDelayMs
        var s = St();
        s.FbCreateTime = 10_000;
        s.FbEnterDelayMs = 300_000;   // 5 分

        Assert.False(FbRecycleCore.ShouldEnterExpiryBlock(s, 10_000 + 60_000 + 300_000 - 1));
        Assert.True(FbRecycleCore.ShouldEnterExpiryBlock(s, 10_000 + 60_000 + 300_000));    // **闭区间**
    }

    [Fact]
    public void ExpiryBlockOpensImmediatelyIfSomeoneEntered()
    {
        // 10523：或 m_boFBPlayObjectEnter
        var s = St();
        s.FbCreateTime = 10_000;
        s.BoFBPlayObjectEnter = true;

        Assert.True(FbRecycleCore.ShouldEnterExpiryBlock(s, 10_000));
    }

    [Fact]
    public void ExpiryBlockClosedWhenNeitherCondition()
    {
        var s = St();
        s.FbCreateTime = 10_000;
        s.FbEnterDelayMs = 0;

        Assert.False(FbRecycleCore.ShouldEnterExpiryBlock(s, 10_000 + 59_999));
    }

    // ===================== 路径二内层三岔（10525-10532） =====================

    [Fact]
    public void OccupiedRenewsTick()
    {
        // 10525-10526
        var s = St(count: 1);
        s.FbNoPlayObjectTick = 5000;

        Assert.Equal(FbRecycleCore.ExpiryAction.RenewBecauseOccupied,
            FbRecycleCore.SelectExpiryAction(s, 8000));

        FbRecycleCore.ApplyRenew(s, 8000);
        Assert.Equal(9000, s.FbNoPlayObjectTick);   // now + 1000
    }

    [Fact]
    public void RecycleWhenElapsedEnough()
    {
        // 10531-10532
        var s = St(count: 0, noHumMs: 10_000);
        s.FbNoPlayObjectTick = 1000;

        Assert.Equal(FbRecycleCore.ExpiryAction.Recycle, FbRecycleCore.SelectExpiryAction(s, 11_000));
    }

    [Fact]
    public void RecycleBoundaryIsInclusive()
    {
        // 差值 **>= NoHumClearMs**
        var s = St(count: 0, noHumMs: 10_000);
        s.FbNoPlayObjectTick = 1000;

        Assert.Equal(FbRecycleCore.ExpiryAction.Recycle, FbRecycleCore.SelectExpiryAction(s, 11_000));
        Assert.NotEqual(FbRecycleCore.ExpiryAction.Recycle, FbRecycleCore.SelectExpiryAction(s, 10_999));
    }

    [Fact]
    public void TickAheadOfNowIsClampedBack()
    {
        // 10530：now < tick → 回拨
        var s = St(count: 0, noHumMs: 10_000);
        s.FbNoPlayObjectTick = 50_000;

        Assert.Equal(FbRecycleCore.ExpiryAction.ClampTickBackToNow,
            FbRecycleCore.SelectExpiryAction(s, 1000));

        FbRecycleCore.ApplyClampTick(s, 1000);
        Assert.Equal(1000, s.FbNoPlayObjectTick);
    }

    [Fact]
    public void EqualTickDoesNothing()
    {
        // **10529/10531 均不成立**：now = tick → 静默跳过
        var s = St(count: 0, noHumMs: 10_000);
        s.FbNoPlayObjectTick = 5000;

        Assert.Equal(FbRecycleCore.ExpiryAction.DoNothing, FbRecycleCore.SelectExpiryAction(s, 5000));
    }

    [Fact]
    public void OccupiedTakesPrecedenceOverClamp()
    {
        // 有人时**先**走续期，不回拨
        var s = St(count: 1);
        s.FbNoPlayObjectTick = 50_000;

        Assert.Equal(FbRecycleCore.ExpiryAction.RenewBecauseOccupied,
            FbRecycleCore.SelectExpiryAction(s, 1000));
    }

    [Fact]
    public void RecycleClearsOnlyTwoFields()
    {
        // 10534-10535：**只清创建者与创建标志**（守护计数不在其中）
        var s = St(created: true);
        s.FbMasterObject = new object();
        s.GuardinaLevelMonCount = 7;

        FbRecycleCore.ApplyRecycle(s);

        Assert.Null(s.FbMasterObject);
        Assert.False(s.BoFBCreate);
        Assert.Equal(7, s.GuardinaLevelMonCount);   // **未被清零**
    }

    [Fact]
    public void RecycledInstanceBecomesAllocatable()
    {
        // 回收后满足 J112 的空闲判定
        var s = St(count: 0, created: true);

        var pool = new FbInstancePoolCore.FbInstance { PlayObjectCount = 0, BoFBCreate = true };
        Assert.False(FbInstancePoolCore.IsFreeInstance(pool));

        pool.BoFBCreate = false;   // 回收动作
        Assert.True(FbInstancePoolCore.IsFreeInstance(pool));
    }

    // ===================== 清怪块（10537-10560） =====================

    [Fact]
    public void CleanupSkippedWhenListEmpty()
    {
        var s = St();
        Assert.False(FbRecycleCore.ShouldRunMonsterCleanup(s));
    }

    [Fact]
    public void CleanupRunsWhenListNonEmpty()
    {
        var s = St();
        s.FbMonsterList.Add(Mon());
        Assert.True(FbRecycleCore.ShouldRunMonsterCleanup(s));
    }

    [Fact]
    public void RecycledInstancesTurnLiveMonstersIntoGhosts()
    {
        // **`not m_boFBCreate` 分支**：活怪、无主人 → MakeGhost
        var s = St(created: false);
        var m1 = Mon("a");
        var m2 = Mon("b");
        s.FbMonsterList.Add(m1);
        s.FbMonsterList.Add(m2);

        FbRecycleCore.ApplyMonsterCleanup(s);

        Assert.True(m1.MadeGhost);
        Assert.True(m2.MadeGhost);
        Assert.Empty(s.FbMonsterList);   // 随后 Clear
    }

    [Fact]
    public void RecycledInstanceKeepsMasterOwnedMonstersAlive()
    {
        // **有主人（召唤物）的不变幽灵**，但最后仍被 Clear 清出列表
        var s = St(created: false);
        var m = Mon("宝宝", master: new object());
        s.FbMonsterList.Add(m);

        FbRecycleCore.ApplyMonsterCleanup(s);

        Assert.False(m.MadeGhost);
        Assert.Empty(s.FbMonsterList);
    }

    [Fact]
    public void RecycledInstanceDoesNotReghostExistingGhost()
    {
        var s = St(created: false);
        var m = Mon("已幽灵", ghost: true);
        s.FbMonsterList.Add(m);

        FbRecycleCore.ApplyMonsterCleanup(s);

        Assert.False(m.MadeGhost);   // 本来就是幽灵，不重复 MakeGhost
    }

    [Fact]
    public void ActiveInstanceOnlyDeletesGhostOrDead()
    {
        // **`m_boFBCreate` 分支**：只删幽灵/已死亡的，活怪保留
        var s = St(created: true);
        var live = Mon("活");
        var ghost = Mon("幽灵", ghost: true);
        var dead = Mon("死", death: true);
        s.FbMonsterList.Add(live);
        s.FbMonsterList.Add(ghost);
        s.FbMonsterList.Add(dead);

        FbRecycleCore.ApplyMonsterCleanup(s);

        Assert.Single(s.FbMonsterList);
        Assert.Equal("活", s.FbMonsterList[0].Name);
        Assert.False(live.Deleted);
        Assert.True(ghost.Deleted);
        Assert.True(dead.Deleted);
    }

    [Fact]
    public void ActiveInstanceDoesNotClearList()
    {
        var s = St(created: true);
        s.FbMonsterList.Add(Mon("活"));

        FbRecycleCore.ApplyMonsterCleanup(s);

        Assert.Single(s.FbMonsterList);   // **未 Clear**
    }

    [Fact]
    public void ActiveInstanceNeverMakesGhost()
    {
        var s = St(created: true);
        var m = Mon("活");
        s.FbMonsterList.Add(m);

        FbRecycleCore.ApplyMonsterCleanup(s);

        Assert.False(m.MadeGhost);
    }

    [Fact]
    public void TwoBranchesAreOpposite()
    {
        // **同一段代码在两种状态下行为相反**——差异保护
        var recycled = St(created: false);
        var rm = Mon("活");
        recycled.FbMonsterList.Add(rm);

        var active = St(created: true);
        var am = Mon("活");
        active.FbMonsterList.Add(am);

        FbRecycleCore.ApplyMonsterCleanup(recycled);
        FbRecycleCore.ApplyMonsterCleanup(active);

        Assert.True(rm.MadeGhost);
        Assert.False(am.MadeGhost);
        Assert.Empty(recycled.FbMonsterList);
        Assert.Single(active.FbMonsterList);
    }

    [Fact]
    public void PushBackLoopHandlesAllElements()
    {
        // 倒序遍历，Delete 不漏项
        var s = St(created: true);
        for (int i = 0; i < 5; i++)
            s.FbMonsterList.Add(Mon($"g{i}", ghost: true));
        s.FbMonsterList.Add(Mon("live"));

        FbRecycleCore.ApplyMonsterCleanup(s);

        Assert.Single(s.FbMonsterList);
        Assert.Equal("live", s.FbMonsterList[0].Name);
    }

    [Fact]
    public void UsesClearNotFree()
    {
        Assert.True(FbRecycleCore.UsesClearNotFree());
    }

    // ===================== check monster 门控（10565-10576） =====================

    [Fact]
    public void CheckGateIsStrictlyGreater()
    {
        var s = St();
        s.FbCheckMonsterTick = 1000;

        Assert.False(FbRecycleCore.ShouldCheckMonsters(s, 1000));   // 相等不进入
        Assert.True(FbRecycleCore.ShouldCheckMonsters(s, 1001));
    }

    [Fact]
    public void CheckGateAdvancesOneMinute()
    {
        var s = St();
        FbRecycleCore.AdvanceCheckMonsterTick(s, 5000);
        Assert.Equal(65_000, s.FbCheckMonsterTick);
    }

    [Fact]
    public void CheckGateLimitsToOncePerMinute()
    {
        var s = St();
        s.FbCheckMonsterTick = 1000;

        Assert.True(FbRecycleCore.ShouldCheckMonsters(s, 2000));
        FbRecycleCore.AdvanceCheckMonsterTick(s, 2000);

        Assert.False(FbRecycleCore.ShouldCheckMonsters(s, 30_000));   // 未到 60 秒
        Assert.False(FbRecycleCore.ShouldCheckMonsters(s, 62_000));   // **恰好相等仍不成立**（严格 >）
        Assert.True(FbRecycleCore.ShouldCheckMonsters(s, 62_001));
    }

    [Fact]
    public void InsideReleaseRequiresCreateFlag()
    {
        var s = St(created: false);
        Assert.False(FbRecycleCore.ShouldReleaseInsideCheck(s, 999_999));
    }

    [Fact]
    public void InsideReleaseOnMasterNil()
    {
        var s = St(created: true, count: 5);
        Assert.True(FbRecycleCore.ShouldReleaseInsideCheck(s, 0));
    }

    [Fact]
    public void InsideReleaseOnZeroCount()
    {
        var s = St(created: true, count: 0);
        s.FbMasterObject = new object();
        Assert.True(FbRecycleCore.ShouldReleaseInsideCheck(s, 0));
    }

    [Fact]
    public void InsideReleaseOnExpiredTime()
    {
        var s = St(created: true, count: 5);
        s.FbMasterObject = new object();
        s.FbTime = 10_000;

        Assert.False(FbRecycleCore.ShouldReleaseInsideCheck(s, 10_000));   // **严格 >**
        Assert.True(FbRecycleCore.ShouldReleaseInsideCheck(s, 10_001));
    }

    [Fact]
    public void InsideReleaseDoesNotTriggerWhenAllGood()
    {
        var s = St(created: true, count: 3);
        s.FbMasterObject = new object();
        s.FbTime = 10_000;

        Assert.False(FbRecycleCore.ShouldReleaseInsideCheck(s, 5_000));
    }

    [Fact]
    public void InsideReleaseClearsGuardinaCount()
    {
        // **比路径二多一项**
        var s = St(created: true);
        s.FbMasterObject = new object();
        s.GuardinaLevelMonCount = 9;

        FbRecycleCore.ApplyInsideRelease(s);

        Assert.Null(s.FbMasterObject);
        Assert.False(s.BoFBCreate);
        Assert.Equal(0, s.GuardinaLevelMonCount);
    }

    // ===================== 创建人离线（10620-10635） =====================

    [Fact]
    public void CreaterOfflineRequiresConfig()
    {
        var s = St(created: true);
        Assert.False(FbRecycleCore.ShouldReleaseOnCreaterOffline(
            configExitCreaterOffline: false, FbMapDeclareCore.FbEnterLimit.Job3, s, 999_999, false, false));
    }

    [Fact]
    public void CreaterOfflineSkippedForOnlyCreaterLimit()
    {
        // **只有创建者能进的副本不适用**
        var s = St(created: true);
        Assert.False(FbRecycleCore.ShouldReleaseOnCreaterOffline(
            true, FbMapDeclareCore.FbEnterLimit.OnlyCreater, s, 999_999, false, false));
    }

    [Fact]
    public void CreaterOfflineRequiresGraceElapsed()
    {
        var s = St(created: true);
        s.FbCreateTime = 100_000;

        Assert.False(FbRecycleCore.ShouldReleaseOnCreaterOffline(
            true, FbMapDeclareCore.FbEnterLimit.Job3, s, 100_000 + 59_999, false, false));

        Assert.True(FbRecycleCore.ShouldReleaseOnCreaterOffline(
            true, FbMapDeclareCore.FbEnterLimit.Job3, s, 100_000 + 60_000, false, false));
    }

    [Fact]
    public void CreaterOfflineTriggersWhenCreaterMissing()
    {
        var s = St(created: true);
        s.FbCreateTime = 0;

        Assert.True(FbRecycleCore.ShouldReleaseOnCreaterOffline(
            true, FbMapDeclareCore.FbEnterLimit.Job3, s, 60_000, createrFound: false, createrInThisEnvir: false));
    }

    [Fact]
    public void CreaterOfflineTriggersWhenCreaterElsewhere()
    {
        var s = St(created: true);
        s.FbCreateTime = 0;

        Assert.True(FbRecycleCore.ShouldReleaseOnCreaterOffline(
            true, FbMapDeclareCore.FbEnterLimit.Job3, s, 60_000, true, false));
    }

    [Fact]
    public void CreaterOfflineNotTriggeredWhenCreaterPresent()
    {
        var s = St(created: true);
        s.FbCreateTime = 0;

        Assert.False(FbRecycleCore.ShouldReleaseOnCreaterOffline(
            true, FbMapDeclareCore.FbEnterLimit.Job3, s, 60_000, true, true));
    }

    [Fact]
    public void CreaterOfflineReleaseClearsTwoFields()
    {
        var s = St(created: true);
        s.FbMasterObject = new object();

        FbRecycleCore.ApplyCreaterOfflineRelease(s);

        Assert.False(s.BoFBCreate);
        Assert.Null(s.FbMasterObject);
    }

    [Fact]
    public void ClearMasterOnlyDoesNotClearCreateFlag()
    {
        // 10631-10635：只清创建者
        Assert.True(FbRecycleCore.ShouldClearMasterOnly(createrFound: false, createrInThisEnvir: false));
        Assert.True(FbRecycleCore.ShouldClearMasterOnly(true, false));
        Assert.False(FbRecycleCore.ShouldClearMasterOnly(true, true));
    }

    [Fact]
    public void ConfigNameConstant()
    {
        Assert.Equal("boFBExitCreaterOffline", FbRecycleCore.ConfigExitCreaterOffline);
    }
}
