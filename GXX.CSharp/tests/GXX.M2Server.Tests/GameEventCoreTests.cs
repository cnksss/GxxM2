using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J124：地图事件基类 `TGameEvent` 与禁锢光幕事件
/// （GameEvent.pas 617-689 构造/析构/Run/Close、481-489 两个光幕构造、ET_* 常量）1:1 测试。
/// </summary>
public sealed class GameEventCoreTests
{
    private static GameEventCore.GameEvent Make(
        int continueTime = 1000, bool visible = true, object? envir = null)
        => GameEventCore.Create(
            envir ?? new object(), 10, 20, GameEventCore.EtHolyCurtain,
            continueTime, visible, openStartTick: 0, runStart: 0, now: 0);

    // ===================== ET 常量 =====================

    [Fact]
    public void EventTypeConstants()
    {
        Assert.Equal(1, GameEventCore.EtDigOutZombi);
        Assert.Equal(2, GameEventCore.EtSafeRect);
        Assert.Equal(3, GameEventCore.EtPileStones);
        Assert.Equal(4, GameEventCore.EtHolyCurtain);
        Assert.Equal(5, GameEventCore.EtFire);
        Assert.Equal(6, GameEventCore.EtSculPeice);
        Assert.Equal(7, GameEventCore.EtFireLevel1);
        Assert.Equal(8, GameEventCore.EtFireLevel2);
        Assert.Equal(9, GameEventCore.EtFireLevel3);
        Assert.Equal(10, GameEventCore.EtIcePeak);
        Assert.Equal(11, GameEventCore.EtHolyCurtain2);
        Assert.Equal(17, GameEventCore.EtFireDragon);
        Assert.Equal(100, GameEventCore.EtMapEffect);
        Assert.Equal(110, GameEventCore.EtThunder);
        Assert.Equal(124, GameEventCore.EtSprings3);
    }

    [Fact]
    public void EventTypeNumbersHaveGaps()
    {
        Assert.True(GameEventCore.EventTypeNumbersHaveGaps());
    }

    [Fact]
    public void EventTypeGapRanges()
    {
        var set = new HashSet<int>(GameEventCore.KnownEventTypes);

        for (int i = 12; i <= 16; i++)
            Assert.DoesNotContain(i, set);

        for (int i = 18; i <= 99; i++)
            Assert.DoesNotContain(i, set);
    }

    [Fact]
    public void EventTypeElevenIsDuplicated()
    {
        // **ET_HOLYCURTAIN2 = 11 与 ET_STONEMINE = 11 数值冲突**
        Assert.True(GameEventCore.EventTypeElevenIsDuplicated());
        Assert.Equal(11, GameEventCore.EtStoneMine);
    }

    [Fact]
    public void RunTickInitialIsFiveHundred()
    {
        Assert.True(GameEventCore.RunTickInitialIsFiveHundred());
        Assert.Equal(500u, GameEventCore.RunTickInitial);
    }

    // ===================== 构造 =====================

    [Fact]
    public void ConstructorInitializesFields()
    {
        var e = Make();

        Assert.Equal(0, e.NEventParam);
        Assert.Equal(0, e.NDamage);
        Assert.True(e.BoActive);
        Assert.False(e.BoClosed);
        Assert.Null(e.OwnBaseObject);
        Assert.Equal(500u, e.DwRunTick);
        Assert.True(e.BoAllowClose);
        Assert.Equal(10, e.NX);
        Assert.Equal(20, e.NY);
    }

    [Fact]
    public void ConstructorStoresTypeAndTime()
    {
        var e = Make(continueTime: 5000);

        Assert.Equal(GameEventCore.EtHolyCurtain, e.NEventType);
        Assert.Equal(5000, e.DwContinueTime);
    }

    [Fact]
    public void EventParamAndDamageStartAtZero()
    {
        Assert.True(GameEventCore.EventParamAndDamageStartAtZero());
    }

    [Fact]
    public void OwnBaseObjectStartsNull()
    {
        Assert.True(GameEventCore.OwnBaseObjectStartsNull());
    }

    [Fact]
    public void ActiveTrueClosedFalse()
    {
        Assert.True(GameEventCore.ActiveTrueClosedFalse());
    }

    [Fact]
    public void OpenStartTickAndRunStartAreSeparateCalls()
    {
        // 621 与 633 是两次独立的 MyGetTickCount 调用，**不保证相等**
        Assert.True(GameEventCore.OpenStartTickAndRunStartAreSeparateCalls());

        var e = GameEventCore.Create(
            new object(), 0, 0, 0, 0, true, openStartTick: 100, runStart: 107, now: 0);

        Assert.Equal(100u, e.DwOpenStartTick);
        Assert.Equal(107u, e.DwRunStart);
    }

    [Fact]
    public void ShouldAddToMapRequiresEnvirAndVisible()
    {
        Assert.True(GameEventCore.ShouldAddToMap(new object(), true));
        Assert.False(GameEventCore.ShouldAddToMap(null, true));
        Assert.False(GameEventCore.ShouldAddToMap(new object(), false));
        Assert.False(GameEventCore.ShouldAddToMap(null, false));
    }

    [Fact]
    public void AddsToMapWhenVisible()
    {
        Assert.True(GameEventCore.AddsToMapWhenVisible());
    }

    [Fact]
    public void NullEnvirForcesInvisible()
    {
        // **boVisible 传入 True 但被强制改写为 False**
        Assert.True(GameEventCore.NullEnvirForcesInvisible());

        var e = GameEventCore.Create(
            null, 0, 0, 0, 100, boVisible: true, 0, 0, 0);

        Assert.False(e.BoVisible);
        Assert.False(e.AddedToMap);
    }

    [Fact]
    public void ShouldForceInvisibleOnlyWhenEnvirNull()
    {
        Assert.True(GameEventCore.ShouldForceInvisible(null, true));
        Assert.False(GameEventCore.ShouldForceInvisible(new object(), true));
    }

    [Fact]
    public void InvisibleNotAdded()
    {
        Assert.True(GameEventCore.InvisibleNotAdded());
    }

    [Fact]
    public void AllowCloseDefaultTrueButSubclassOverrides()
    {
        Assert.True(GameEventCore.AllowCloseDefaultTrueButSubclassOverrides());
        Assert.Equal(3, GameEventCore.AllowCloseOverrides.Length);
    }

    [Fact]
    public void SafeEventDisablesAllowClose()
    {
        var safe = Array.Find(
            GameEventCore.AllowCloseOverrides, o => o.Class == "TSafeEvent");

        Assert.Equal(499, safe.Line);
        Assert.Equal("False", safe.Value);
    }

    // ===================== Run：到期 =====================

    [Fact]
    public void RunExpiryIsStrictGreater()
    {
        Assert.True(GameEventCore.RunExpiryIsStrictGreater());
    }

    [Fact]
    public void ShouldExpireBoundaries()
    {
        Assert.False(GameEventCore.ShouldExpire(true, 1000, 0, 1000));
        Assert.True(GameEventCore.ShouldExpire(true, 1001, 0, 1000));
        Assert.False(GameEventCore.ShouldExpire(true, 999, 0, 1000));
    }

    [Fact]
    public void AllowCloseFalseNeverExpires()
    {
        Assert.True(GameEventCore.AllowCloseFalseNeverExpires());
        Assert.False(GameEventCore.ShouldExpire(false, 999_999, 0, 1));
    }

    [Fact]
    public void RunUsesPlainSubtractNotTickDiff()
    {
        Assert.True(GameEventCore.RunUsesPlainSubtractNotTickDiff());
    }

    [Fact]
    public void RunClosesOnExpiry()
    {
        var e = Make(continueTime: 1000);

        var r = GameEventCore.Run(e, now: 1001, ownerGhost: false, ownerDeath: false);

        Assert.True(r.Closed);
        Assert.True(e.BoClosed);
        Assert.False(e.BoVisible);
        Assert.Null(e.Envir);
    }

    [Fact]
    public void RunDoesNotCloseBeforeExpiry()
    {
        var e = Make(continueTime: 1000);

        var r = GameEventCore.Run(e, now: 1000, ownerGhost: false, ownerDeath: false);

        Assert.False(r.Closed);
        Assert.False(e.BoClosed);
        Assert.True(e.BoVisible);
    }

    [Fact]
    public void RunDoesNotChangeAllowClose()
    {
        Assert.True(GameEventCore.RunDoesNotChangeAllowClose());
    }

    // ===================== Run：拥有者引用 =====================

    [Fact]
    public void OwnerDeathClearsReferenceButDoesNotClose()
    {
        var e = Make(continueTime: 999_999);
        e.OwnBaseObject = new object();

        var r = GameEventCore.Run(e, now: 1, ownerGhost: false, ownerDeath: true);

        Assert.True(r.ClearedOwner);
        Assert.Null(e.OwnBaseObject);
        Assert.False(r.Closed);      // **不关闭事件**
        Assert.True(e.BoVisible);
    }

    [Fact]
    public void OwnerGhostAlsoClearsReference()
    {
        var e = Make(continueTime: 999_999);
        e.OwnBaseObject = new object();

        var r = GameEventCore.Run(e, now: 1, ownerGhost: true, ownerDeath: false);

        Assert.True(r.ClearedOwner);
    }

    [Fact]
    public void NullOwnerNotCleared()
    {
        var e = Make(continueTime: 999_999);
        e.OwnBaseObject = null;

        var r = GameEventCore.Run(e, now: 1, ownerGhost: true, ownerDeath: true);

        Assert.False(r.ClearedOwner);
    }

    [Fact]
    public void HealthyOwnerNotCleared()
    {
        var e = Make(continueTime: 999_999);
        var owner = new object();
        e.OwnBaseObject = owner;

        var r = GameEventCore.Run(e, now: 1, ownerGhost: false, ownerDeath: false);

        Assert.False(r.ClearedOwner);
        Assert.Same(owner, e.OwnBaseObject);
    }

    [Fact]
    public void ShouldClearOwnBaseObjectGuard()
    {
        Assert.True(GameEventCore.ShouldClearOwnBaseObject(new object(), true, false));
        Assert.True(GameEventCore.ShouldClearOwnBaseObject(new object(), false, true));
        Assert.False(GameEventCore.ShouldClearOwnBaseObject(null, true, true));
        Assert.False(GameEventCore.ShouldClearOwnBaseObject(new object(), false, false));
    }

    [Fact]
    public void BothSegmentsRunIndependently()
    {
        // **无 else**：同一次 Run 内既关闭事件、又清空拥有者引用
        Assert.True(GameEventCore.BothSegmentsRunIndependently());
    }

    [Fact]
    public void BothSegmentsFromScratch()
    {
        var e = Make(continueTime: 100);
        e.OwnBaseObject = new object();

        var r = GameEventCore.Run(e, now: 500, ownerGhost: true, ownerDeath: false);

        Assert.True(r.Closed);
        Assert.True(r.ClearedOwner);
        Assert.Null(e.Envir);
        Assert.Null(e.OwnBaseObject);
    }

    // ===================== Close =====================

    [Fact]
    public void CloseClearsVisibleAndEnvir()
    {
        Assert.True(GameEventCore.CloseClearsVisibleAndEnvir());
    }

    [Fact]
    public void CloseRemovesFromMap()
    {
        var e = Make();

        GameEventCore.Close(e, 100);

        Assert.True(e.RemovedFromMap);
        Assert.False(e.BoVisible);
        Assert.Null(e.Envir);
    }

    [Fact]
    public void CloseSetsCloseTick()
    {
        var e = Make();

        GameEventCore.Close(e, 4242);

        Assert.Equal(4242u, e.DwCloseTick);
    }

    [Fact]
    public void CloseRefreshesTickEveryTime()
    {
        Assert.True(GameEventCore.CloseRefreshesTickEveryTime());
    }

    [Fact]
    public void CloseDoesNotSetBoClosed()
    {
        // **两个标志语义不同步**：Close 后 m_boVisible=False 但 m_boClosed 仍为 False
        Assert.True(GameEventCore.CloseDoesNotSetBoClosed());

        var e = Make();
        GameEventCore.Close(e, 100);

        Assert.False(e.BoClosed);
        Assert.False(e.BoVisible);
    }

    [Fact]
    public void CloseOnInvisibleOnlyRefreshesTick()
    {
        Assert.True(GameEventCore.CloseOnInvisibleOnlyRefreshesTick());
    }

    [Fact]
    public void CloseOnInvisibleKeepsEnvir()
    {
        // **已不可见时 Close 不把 m_Envir 置 nil**
        Assert.True(GameEventCore.CloseOnInvisibleKeepsEnvir());

        var e = Make();
        e.BoVisible = false;

        GameEventCore.Close(e, 100);

        Assert.NotNull(e.Envir);
        Assert.False(e.RemovedFromMap);
    }

    [Fact]
    public void CloseIsSafeToCallTwice()
    {
        Assert.True(GameEventCore.CloseIsSafeToCallTwice());
    }

    [Fact]
    public void CloseWithNullEnvirSkipsRemoval()
    {
        Assert.True(GameEventCore.CloseWithNullEnvirSkipsRemoval());
    }

    [Fact]
    public void CloseDoesNotRemoveFromAnyList()
    {
        Assert.True(GameEventCore.CloseDoesNotRemoveFromAnyList());
    }

    [Fact]
    public void VisibleAndClosedFlagsAreIndependent()
    {
        Assert.True(GameEventCore.VisibleAndClosedFlagsAreIndependent());
    }

    // ===================== 析构 =====================

    [Fact]
    public void DestroyRemovesWhenNotClosed()
    {
        Assert.True(GameEventCore.DestroyRemovesWhenNotClosed());
    }

    [Fact]
    public void DestroySkipsWhenAlreadyClosed()
    {
        // **依靠 m_Envir := nil 实现去重**：Close 后析构不再移除
        Assert.True(GameEventCore.DestroySkipsWhenAlreadyClosed());
    }

    [Fact]
    public void DestroyGuardIgnoresVisibleFlag()
    {
        // 析构的 if 只看 m_Envir，**不看 m_boVisible**（与 Close 的 680 不同）
        var e = Make();
        e.BoVisible = false;
        e.Envir = new object();

        Assert.True(GameEventCore.DestroyRemovesIfEnvirNotNull(e));
    }

    [Fact]
    public void UnloadPathReliesOnDestructor()
    {
        Assert.True(GameEventCore.UnloadPathReliesOnDestructor());
    }

    [Fact]
    public void DestroyRetainsCommentedEventCheckLoop()
    {
        Assert.True(GameEventCore.DestroyRetainsCommentedEventCheckLoop());
        Assert.True(GameEventCore.EventCheckIsHistoricalRemnant());
        Assert.Contains("EventCheck", GameEventCore.CommentedEventCheckLoop);
    }

    // ===================== 光幕事件 =====================

    [Fact]
    public void TwoCurtainCtorsAreIdentical()
    {
        Assert.True(GameEventCore.TwoCurtainCtorsAreIdentical());
        Assert.Equal(2, GameEventCore.CurtainCtors.Length);
    }

    [Fact]
    public void CurtainCtorLineNumbers()
    {
        Assert.Equal(481, GameEventCore.CurtainCtors[0].Line);
        Assert.Equal(486, GameEventCore.CurtainCtors[1].Line);
    }

    [Fact]
    public void BothPassVisibleTrue()
    {
        Assert.True(GameEventCore.BothPassVisibleTrue());
    }

    [Fact]
    public void CreateCurtainIsVisible()
    {
        var e = GameEventCore.CreateCurtain(new object(), 5, 6, GameEventCore.EtHolyCurtain, 3000, 0);

        Assert.True(e.BoVisible);
        Assert.True(e.AddedToMap);
        Assert.Equal(GameEventCore.EtHolyCurtain, e.NEventType);
        Assert.Equal(5, e.NX);
        Assert.Equal(6, e.NY);
    }

    [Fact]
    public void CurtainLifetimeIsCtorTime()
    {
        Assert.True(GameEventCore.CurtainLifetimeIsCtorTime(3000));
    }

    [Fact]
    public void CurtainExpiresAfterItsTime()
    {
        var e = GameEventCore.CreateCurtain(
            new object(), 0, 0, GameEventCore.EtHolyCurtain, 3000, 0);

        Assert.Equal(3000, e.DwContinueTime);
        Assert.False(GameEventCore.ShouldExpire(
            e.BoAllowClose, 3000, e.DwOpenStartTick, e.DwContinueTime));
        Assert.True(GameEventCore.ShouldExpire(
            e.BoAllowClose, 3001, e.DwOpenStartTick, e.DwContinueTime));
    }

    [Fact]
    public void ImprisonCurtainHasNoRunOverride()
    {
        Assert.True(GameEventCore.ImprisonCurtainHasNoRunOverride());
    }

    [Fact]
    public void ClassNameDoesNotDetermineEventType()
    {
        Assert.True(GameEventCore.ClassNameDoesNotDetermineEventType());
    }

    [Fact]
    public void HolyCurtain4HasManyCallSites()
    {
        Assert.True(GameEventCore.CallSitesUsingHolyCurtain4.Length > 5);
        Assert.Contains("NpcActionCmd.pas:23772", GameEventCore.CallSitesUsingHolyCurtain4);
    }

    [Fact]
    public void HolyCurtain2OnlyFromMagic()
    {
        Assert.True(GameEventCore.OnlyMagicUsesHolyCurtainTwo());
        Assert.Single(GameEventCore.CallSitesUsingHolyCurtain2);
        Assert.Equal("Magic.pas:7751", GameEventCore.CallSitesUsingHolyCurtain2[0]);
    }

    [Fact]
    public void J122UsesHolyCurtainFour()
    {
        Assert.True(GameEventCore.J122UsesHolyCurtainFour());
    }

    [Fact]
    public void SafeEventUsesTickAsContinueTime()
    {
        Assert.True(GameEventCore.SafeEventUsesTickAsContinueTime());
    }

    [Fact]
    public void FourImprisonCallsUseCrossOfFour()
    {
        // J122 单目标模式创建十字四格（23839/23843/23847/23851 连续四行）
        var cross = new[]
        {
            "NpcActionCmd.pas:23839", "NpcActionCmd.pas:23843",
            "NpcActionCmd.pas:23847", "NpcActionCmd.pas:23851",
        };

        Assert.All(cross, c => Assert.Contains(c, GameEventCore.CallSitesUsingHolyCurtain4));
        Assert.Equal(4, cross.Length);
    }

    // ===================== 生命周期仿真 =====================

    [Fact]
    public void LifecycleClosesOnEveryRoundAfterExpiry()
    {
        // **基类 Run 不检查 m_boClosed** —— 到期后**每一轮**都重入并再调 Close()
        var closedAt = GameEventCore.SimulateLifecycle(
            continueTime: 1000, createTick: 0, rounds: 10, stepMs: 200);

        Assert.Equal(new[] { 1200u, 1400u, 1600u, 1800u, 2000u }, closedAt);
    }

    [Fact]
    public void BaseRunHasNoClosedGuard()
    {
        // 基类 664 只看 m_boAllowClose，**不看 m_boClosed**（子类都看）
        Assert.True(GameEventCore.BaseRunLacksClosedGuard());
    }

    [Fact]
    public void SubclassRunsDoGuardOnClosed()
    {
        Assert.True(GameEventCore.SubclassRunsGuardOnClosed());
        Assert.Equal(4, GameEventCore.SubclassClosedGuardLines.Length);
    }

    [Fact]
    public void FirstExpiryIsAtNextRoundBoundary()
    {
        var closedAt = GameEventCore.SimulateLifecycle(
            continueTime: 1000, createTick: 0, rounds: 10, stepMs: 200);

        Assert.Equal(1200u, closedAt[0]);   // 首次超过 1000 的轮次是 1200
    }

    [Fact]
    public void LifecycleWithLargeStep()
    {
        var closedAt = GameEventCore.SimulateLifecycle(
            continueTime: 1000, createTick: 0, rounds: 3, stepMs: 1000);

        Assert.Equal(new[] { 2000u, 3000u }, closedAt);
    }

    [Fact]
    public void LifecycleNeverClosesWhenTimeHuge()
    {
        var closedAt = GameEventCore.SimulateLifecycle(
            continueTime: 999_999, createTick: 0, rounds: 5, stepMs: 100);

        Assert.Empty(closedAt);
    }

    [Fact]
    public void LifecycleZeroTimeClosesEveryRound()
    {
        // dwTime = 0 → 第一轮即刻到期，之后每轮都再次 Close
        var closedAt = GameEventCore.SimulateLifecycle(
            continueTime: 0, createTick: 0, rounds: 3, stepMs: 100);

        Assert.Equal(new[] { 100u, 200u, 300u }, closedAt);
    }

    [Fact]
    public void RepeatedCloseIsHarmlessAfterFirst()
    {
        // 重复 Close 除刷新 m_dwCloseTick 外无副作用（因 m_boVisible 已假）
        var e = Make(continueTime: 0);

        GameEventCore.Run(e, now: 100, ownerGhost: false, ownerDeath: false);
        bool removed = e.RemovedFromMap;
        var envir = e.Envir;

        GameEventCore.Run(e, now: 200, ownerGhost: false, ownerDeath: false);

        Assert.True(removed);
        Assert.Null(envir);
        Assert.Null(e.Envir);
        Assert.Equal(200u, e.DwCloseTick);
    }
}
