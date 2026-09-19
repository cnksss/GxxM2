using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J125：事件管理器 `TEventManager`（GameEvent.pas 119-131 声明、224-346 `Run`、
/// 347-386 注释掉的旧版、388-430 `GetGameEvent`、432-443 `AddEvent`、445-476 构造/析构）1:1 测试。
/// </summary>
public sealed class EventManagerCoreTests
{
    private static EventManagerCore.ManagedEvent Ev(
        bool active = true, uint runStart = 0, uint addTime = 0,
        object? envir = null, int x = 0, int y = 0, int type = 0)
        => new()
        {
            BoActive = active,
            DwRunStart = runStart,
            DwAddTime = addTime,
            Envir = envir,
            NX = x,
            NY = y,
            NEventType = type,
        };

    private static IEnumerator<uint> Seq(params uint[] values) => ((IEnumerable<uint>)values).GetEnumerator();

    /// <summary>重复同一时刻 N 次（预算不超、节流通过）。</summary>
    private static IEnumerator<uint> Repeat(uint value, int count)
    {
        var arr = new uint[count];

        for (int i = 0; i < count; i++)
            arr[i] = value;

        return ((IEnumerable<uint>)arr).GetEnumerator();
    }

    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.Equal(250u, EventManagerCore.RunThrottleMs);
        Assert.Equal(10u, EventManagerCore.TimeBudgetMs);
        Assert.Equal(300_000u, EventManagerCore.ClosedReleaseGraceMs);
    }

    [Fact]
    public void StoneMineConstantEvaluatesToOneHour()
    {
        // **表达式形态 `60 * 1000 * 60` 求值为 3,600,000**
        Assert.True(EventManagerCore.StoneMineConstantEvaluatesToOneHour());
        Assert.Equal(3_600_000u, EventManagerCore.StoneMineLifetimeMs);
    }

    [Fact]
    public void StoneMineExpiryIsOneHourWithGreaterOrEqual()
    {
        // **`>=` 而非严格 `>`**
        Assert.True(EventManagerCore.StoneMineExpiryIsOneHourWithGreaterOrEqual());
    }

    [Fact]
    public void StoneMineExpiryBoundary()
    {
        Assert.True(EventManagerCore.StoneMineExpiryReached(3_600_000));
        Assert.False(EventManagerCore.StoneMineExpiryReached(3_599_999));
    }

    [Fact]
    public void StoneMineExpiryViaTickDiff()
    {
        Assert.True(EventManagerCore.StoneMineExpiryReached(0, 3_600_000));
        Assert.False(EventManagerCore.StoneMineExpiryReached(0, 3_599_999));
    }

    // ===================== 三种时间判据写法 =====================

    [Fact]
    public void ManagerUsesTickDiffWhileEventRunUsesPlainSubtract()
    {
        Assert.True(EventManagerCore.ManagerUsesTickDiffWhileEventRunUsesPlainSubtract());
    }

    [Fact]
    public void ClosedReleaseUsesPlainSubtract()
    {
        Assert.True(EventManagerCore.ClosedReleaseUsesPlainSubtract());

        // 回绕输入：直接相减与 tick_diff 结果不同
        uint plain = unchecked(5u - 4294967290u);
        uint diff = EventManagerCore.TickDiff(4294967290, 5);

        Assert.NotEqual(plain, diff);
    }

    [Fact]
    public void ThreeSubtractionStylesInOneFile()
    {
        // tick_diff（244/256）、直接相减（336）、表达式常量（287）
        Assert.True(EventManagerCore.ManagerUsesTickDiffWhileEventRunUsesPlainSubtract());
        Assert.True(EventManagerCore.ClosedReleaseUsesPlainSubtract());
        Assert.True(EventManagerCore.StoneMineConstantEvaluatesToOneHour());
    }

    // ===================== 调度节流 =====================

    [Fact]
    public void RunThrottleIsStrictGreater()
    {
        Assert.True(EventManagerCore.RunThrottleIsStrictGreater());
    }

    [Fact]
    public void ShouldRunEventBoundaries()
    {
        Assert.True(EventManagerCore.ShouldRunEvent(true, 0, 251));
        Assert.False(EventManagerCore.ShouldRunEvent(true, 0, 250));
        Assert.False(EventManagerCore.ShouldRunEvent(true, 0, 100));
    }

    [Fact]
    public void InactiveNeverRuns()
    {
        Assert.True(EventManagerCore.InactiveNeverRuns());
        Assert.False(EventManagerCore.ShouldRunEvent(false, 0, 999_999));
    }

    [Fact]
    public void RunStartUpdatedBeforeRunCall()
    {
        Assert.True(EventManagerCore.RunStartUpdatedBeforeRunCall());
    }

    [Fact]
    public void RunTickReadTwiceAtGate()
    {
        Assert.True(EventManagerCore.RunTickReadTwiceAtGate());
    }

    // ===================== 主循环 =====================

    [Fact]
    public void MainLoopRunsDueEvent()
    {
        var e = Ev(runStart: 0);
        e.OnRun = _ => false;

        var list = new List<EventManagerCore.ManagedEvent> { e };
        var closed = new List<EventManagerCore.ManagedEvent>();
        int cursor = 0;

        // 235 取检查时刻、244 取判定时刻、246 取赋值时刻、256 取预算时刻
        // **注意 256 的预算是与 235 的 dwCheckTime 比较**，故这里第 4 个值须 ≤ 10
        var r = EventManagerCore.RunMainLoop(
            list, closed, ref cursor, Seq(0, 251, 251, 5));

        Assert.Equal(1, r.Ran);
        Assert.True(e.RunCalled);
        Assert.Equal(251u, e.DwRunStart);
    }

    [Fact]
    public void MainLoopSkipsBeforeThrottle()
    {
        var e = Ev(runStart: 0);
        e.OnRun = _ => false;

        var list = new List<EventManagerCore.ManagedEvent> { e };
        var closed = new List<EventManagerCore.ManagedEvent>();
        int cursor = 0;

        var r = EventManagerCore.RunMainLoop(
            list, closed, ref cursor, Seq(0, 250, 250));

        Assert.Equal(0, r.Ran);
        Assert.False(e.RunCalled);
    }

    [Fact]
    public void MainLoopMovesClosedEventToClosedList()
    {
        var e = Ev(runStart: 0);
        e.OnRun = _ => true;   // 立即到期

        var list = new List<EventManagerCore.ManagedEvent> { e };
        var closed = new List<EventManagerCore.ManagedEvent>();
        int cursor = 0;

        var r = EventManagerCore.RunMainLoop(
            list, closed, ref cursor, Seq(0, 251, 251, 5));

        Assert.Empty(list);
        Assert.Single(closed);
        Assert.Equal(1, r.Closed);
    }

    [Fact]
    public void ClosedEventContinueSkipsIncrement()
    {
        // **250 的 Continue 跳过 Inc** —— 故两个相邻的到期事件都被处理
        Assert.True(EventManagerCore.ClosedEventContinueSkipsIncrement());

        var a = Ev(runStart: 0);
        a.OnRun = _ => true;
        var b = Ev(runStart: 0);
        b.OnRun = _ => true;

        var list = new List<EventManagerCore.ManagedEvent> { a, b };
        var closed = new List<EventManagerCore.ManagedEvent>();
        int cursor = 0;

        var r = EventManagerCore.RunMainLoop(
            list, closed, ref cursor, Repeat(300u, 12));

        Assert.Equal(2, r.Closed);
        Assert.Empty(list);
    }

    [Fact]
    public void SkippedIncrementWouldMissNeighbour()
    {
        // 反证：若 Continue 前自增，会**跳过**紧邻的下一个事件
        var a = Ev(runStart: 0);
        a.OnRun = _ => true;
        var b = Ev(runStart: 0);
        b.OnRun = _ => true;
        var c = Ev(runStart: 0);
        c.OnRun = _ => true;

        var list = new List<EventManagerCore.ManagedEvent> { a, b, c };
        var closed = new List<EventManagerCore.ManagedEvent>();
        int cursor = 0;

        var r = EventManagerCore.RunMainLoop(
            list, closed, ref cursor, Repeat(300u, 16));

        Assert.Equal(3, r.Closed);
        Assert.Empty(list);
    }

    [Fact]
    public void MainLoopReturnsWhenListEmpty()
    {
        var list = new List<EventManagerCore.ManagedEvent>();
        var closed = new List<EventManagerCore.ManagedEvent>();
        int cursor = 0;

        var r = EventManagerCore.RunMainLoop(list, closed, ref cursor, Seq(0));

        Assert.Equal(0, r.Processed);
        Assert.False(r.TimeLimited);
        Assert.Equal(0, cursor);
    }

    [Fact]
    public void BoundsCheckedBeforeFetch()
    {
        Assert.True(EventManagerCore.BoundsCheckedBeforeFetch());
    }

    [Fact]
    public void MainLoopResumesFromCursor()
    {
        var a = Ev(runStart: 0);
        a.OnRun = _ => false;
        var b = Ev(runStart: 0);
        b.OnRun = _ => false;

        var list = new List<EventManagerCore.ManagedEvent> { a, b };
        var closed = new List<EventManagerCore.ManagedEvent>();
        int cursor = 1;   // **从 1 开始**

        var r = EventManagerCore.RunMainLoop(
            list, closed, ref cursor, Seq(0, 251, 251, 5));

        Assert.Equal(1, r.Processed);
        Assert.True(b.RunCalled);
        Assert.False(a.RunCalled);
    }

    [Fact]
    public void MainLoopResetsCursorWhenFinished()
    {
        var e = Ev(runStart: 0);
        e.OnRun = _ => false;

        var list = new List<EventManagerCore.ManagedEvent> { e };
        var closed = new List<EventManagerCore.ManagedEvent>();
        int cursor = 0;

        var r = EventManagerCore.RunMainLoop(
            list, closed, ref cursor, Seq(0, 251, 251, 5));

        Assert.False(r.TimeLimited);
        Assert.Equal(0, cursor);   // 266
    }

    [Fact]
    public void MainLoopTimeLimitedKeepsCursor()
    {
        var a = Ev(runStart: 0);
        a.OnRun = _ => false;
        var b = Ev(runStart: 0);
        b.OnRun = _ => false;

        var list = new List<EventManagerCore.ManagedEvent> { a, b };
        var closed = new List<EventManagerCore.ManagedEvent>();
        int cursor = 0;

        // 第 4 次取时刻（256 的预算判定）返回 11 → 超 10ms → 中断
        var r = EventManagerCore.RunMainLoop(
            list, closed, ref cursor, Seq(0, 251, 251, 11));

        Assert.True(r.TimeLimited);
        Assert.Equal(1, cursor);   // 保留续跑点
    }

    [Fact]
    public void CursorResetOnlyWhenNotTimeLimited()
    {
        Assert.True(EventManagerCore.CursorResetOnlyWhenNotTimeLimited());
    }

    [Fact]
    public void AlwaysProcessesAtLeastOne()
    {
        Assert.True(EventManagerCore.AlwaysProcessesAtLeastOne());

        var e = Ev(runStart: 0);
        e.OnRun = _ => false;

        var list = new List<EventManagerCore.ManagedEvent> { e };
        var closed = new List<EventManagerCore.ManagedEvent>();
        int cursor = 0;

        // 第 4 次取时刻已超预算，但元素已被处理
        var r = EventManagerCore.RunMainLoop(
            list, closed, ref cursor, Seq(0, 0, 0, 999_999));

        Assert.Equal(1, r.Processed);
    }

    [Fact]
    public void MainLoopHasOneBudgetCheck()
    {
        Assert.True(EventManagerCore.MainLoopHasOneBudgetCheck());
    }

    // ===================== 挖矿循环 =====================

    [Fact]
    public void StoneMineExpiresAfterOneHour()
    {
        var e = Ev(addTime: 0);

        var list = new List<EventManagerCore.ManagedEvent> { e };
        var closed = new List<EventManagerCore.ManagedEvent>();
        int cursor = 0;

        // 272 检查时刻、281 预算判定、287 到期判定
        var r = EventManagerCore.RunStoneMineLoop(
            list, closed, ref cursor, Seq(0, 0, 3_600_000));

        Assert.Equal(1, r.Expired);
        Assert.Empty(list);
        Assert.Single(closed);
        Assert.True(e.RemovedFromMap);
        Assert.True(e.BoClosed);
    }

    [Fact]
    public void StoneMineKeepsFreshEvent()
    {
        var e = Ev(addTime: 0);

        var list = new List<EventManagerCore.ManagedEvent> { e };
        var closed = new List<EventManagerCore.ManagedEvent>();
        int cursor = 0;

        var r = EventManagerCore.RunStoneMineLoop(
            list, closed, ref cursor, Seq(0, 0, 3_599_999, 0));

        Assert.Equal(0, r.Expired);
        Assert.Single(list);
    }

    [Fact]
    public void StoneMineLoopDoesNotCallRun()
    {
        Assert.True(EventManagerCore.StoneMineLoopDoesNotCallRun());

        var e = Ev(addTime: 0);

        var list = new List<EventManagerCore.ManagedEvent> { e };
        var closed = new List<EventManagerCore.ManagedEvent>();
        int cursor = 0;

        EventManagerCore.RunStoneMineLoop(list, closed, ref cursor, Seq(0, 0, 0, 0));

        Assert.False(e.RunCalled);
    }

    [Fact]
    public void StoneMineMayProcessZero()
    {
        // **预算判定在开头**（281）→ 可能零个元素就中断（与主循环相反）
        Assert.True(EventManagerCore.StoneMineMayProcessZero());

        var e = Ev(addTime: 0);

        var list = new List<EventManagerCore.ManagedEvent> { e };
        var closed = new List<EventManagerCore.ManagedEvent>();
        int cursor = 0;

        var r = EventManagerCore.RunStoneMineLoop(
            list, closed, ref cursor, Seq(0, 999_999));

        Assert.True(r.TimeLimited);
        Assert.Equal(0, r.Processed);
    }

    [Fact]
    public void StoneMineHasTwoBudgetChecks()
    {
        Assert.True(EventManagerCore.StoneMineHasTwoBudgetChecks());
    }

    [Fact]
    public void TwoLoopsHaveDifferentCloseResponsibilities()
    {
        Assert.True(EventManagerCore.TwoLoopsHaveDifferentCloseResponsibilities());
    }

    [Fact]
    public void StoneMineCloseStepsOrder()
    {
        Assert.Equal(4, EventManagerCore.StoneMineCloseSteps.Length);
        Assert.Contains("DeleteFromMap", EventManagerCore.StoneMineCloseSteps[0]);
        Assert.Equal("Event.Close", EventManagerCore.StoneMineCloseSteps[1]);
        Assert.Contains("m_ClosedEventList.Add", EventManagerCore.StoneMineCloseSteps[2]);
        Assert.Contains("m_StoneMineEventList.Delete", EventManagerCore.StoneMineCloseSteps[3]);
    }

    [Fact]
    public void StoneMineResumesFromCursor()
    {
        var a = Ev(addTime: 0);
        var b = Ev(addTime: 0);

        var list = new List<EventManagerCore.ManagedEvent> { a, b };
        var closed = new List<EventManagerCore.ManagedEvent>();
        int cursor = 1;

        // 272、281、287、298
        var r = EventManagerCore.RunStoneMineLoop(
            list, closed, ref cursor, Seq(0, 0, 3_600_000, 0));

        Assert.Equal(1, r.Expired);
        Assert.Single(list);
        Assert.Same(a, list[0]);
    }

    [Fact]
    public void StoneMineResetsCursorWhenFinished()
    {
        var e = Ev(addTime: 0);

        var list = new List<EventManagerCore.ManagedEvent> { e };
        var closed = new List<EventManagerCore.ManagedEvent>();
        int cursor = 0;

        var r = EventManagerCore.RunStoneMineLoop(
            list, closed, ref cursor, Seq(0, 0, 0, 0));

        Assert.False(r.TimeLimited);
        Assert.Equal(0, cursor);
    }

    // ===================== 延迟释放 =====================

    [Fact]
    public void ClosedListIsDeferredFreeQueue()
    {
        Assert.True(EventManagerCore.ClosedListIsDeferredFreeQueue());
    }

    [Fact]
    public void ReleaseGraceIsStrictGreater()
    {
        Assert.True(EventManagerCore.ReleaseGraceIsStrictGreater());
    }

    [Fact]
    public void ReleaseFreesExpiredOnly()
    {
        var fresh = Ev();
        fresh.DwCloseTick = 0;
        var old = Ev();
        old.DwCloseTick = 0;

        var closed = new List<EventManagerCore.ManagedEvent> { fresh, old };

        int freed = EventManagerCore.ReleaseClosedEvents(closed, 300_001);

        Assert.Equal(2, freed);
        Assert.Empty(closed);
    }

    [Fact]
    public void ReleaseKeepsWithinGrace()
    {
        var e = Ev();
        e.DwCloseTick = 0;

        var closed = new List<EventManagerCore.ManagedEvent> { e };

        int freed = EventManagerCore.ReleaseClosedEvents(closed, 300_000);

        Assert.Equal(0, freed);
        Assert.Single(closed);
    }

    [Fact]
    public void ReleaseIsFullScan()
    {
        Assert.True(EventManagerCore.ClosedReleaseIsFullScan());
    }

    [Fact]
    public void ReleaseHandlesMixedAges()
    {
        var old = Ev();
        old.DwCloseTick = 0;
        var young = Ev();
        young.DwCloseTick = 250_000;

        var closed = new List<EventManagerCore.ManagedEvent> { old, young };

        int freed = EventManagerCore.ReleaseClosedEvents(closed, 400_000);

        Assert.Equal(1, freed);
        Assert.Single(closed);
        Assert.Same(young, closed[0]);
    }

    [Fact]
    public void CommentedBreakRetained()
    {
        Assert.True(EventManagerCore.CommentedBreakRetained());
        Assert.Contains("break", EventManagerCore.CommentedBreak);
    }

    [Fact]
    public void FiveMinuteGraceForClientAnimation()
    {
        Assert.True(EventManagerCore.FiveMinuteGraceForClientAnimation());
    }

    // ===================== GetGameEvent =====================

    [Fact]
    public void GetGameEventMatchesAllFourFields()
    {
        var envir = new object();
        var target = Ev(envir: envir, x: 5, y: 6, type: 4);

        var list = new List<EventManagerCore.ManagedEvent>
        {
            Ev(envir: new object(), x: 5, y: 6, type: 4),
            target,
        };

        Assert.Same(target, EventManagerCore.GetGameEvent(list, envir, 5, 6, 4));
    }

    [Fact]
    public void GetGameEventRequiresAllFour()
    {
        var envir = new object();
        var list = new List<EventManagerCore.ManagedEvent>
        {
            Ev(envir: envir, x: 5, y: 6, type: 4),
        };

        Assert.Null(EventManagerCore.GetGameEvent(list, envir, 5, 6, 5));  // type 不符
        Assert.Null(EventManagerCore.GetGameEvent(list, envir, 5, 7, 4));  // y 不符
        Assert.Null(EventManagerCore.GetGameEvent(list, envir, 6, 6, 4));  // x 不符
        Assert.Null(EventManagerCore.GetGameEvent(list, new object(), 5, 6, 4));
    }

    [Fact]
    public void MatchRequiresAllFourFields()
    {
        Assert.True(EventManagerCore.MatchRequiresAllFourFields());
    }

    [Fact]
    public void GetGameEventReturnsNullWhenNotFound()
    {
        Assert.True(EventManagerCore.GetGameEventReturnsNullWhenNotFound());

        var list = new List<EventManagerCore.ManagedEvent>();
        Assert.Null(EventManagerCore.GetGameEvent(list, new object(), 0, 0, 0));
    }

    [Fact]
    public void GetGameEventReturnsFirstMatch()
    {
        var envir = new object();
        var first = Ev(envir: envir, x: 1, y: 1, type: 4);
        var second = Ev(envir: envir, x: 1, y: 1, type: 4);

        var list = new List<EventManagerCore.ManagedEvent> { first, second };

        Assert.Same(first, EventManagerCore.GetGameEvent(list, envir, 1, 1, 4));
    }

    [Fact]
    public void GetGameEventIteratesForward()
    {
        Assert.True(EventManagerCore.GetGameEventIteratesForward());
    }

    [Fact]
    public void GetGameEventSkipsNullElements()
    {
        Assert.True(EventManagerCore.GetGameEventSkipsNullElements());

        var envir = new object();
        var target = Ev(envir: envir, x: 1, y: 1, type: 4);

        var list = new List<EventManagerCore.ManagedEvent> { null!, target };

        Assert.Same(target, EventManagerCore.GetGameEvent(list, envir, 1, 1, 4));
    }

    [Fact]
    public void GetGameEventDoesNotSearchStoneMineList()
    {
        Assert.True(EventManagerCore.GetGameEventDoesNotSearchStoneMineList());
        Assert.True(EventManagerCore.StoneMineSearchIsCommentedOut());
        Assert.Equal(413, EventManagerCore.StoneMineSearchCommentLine);
    }

    // ===================== AddEvent =====================

    [Fact]
    public void AddEventDefaultGoesToMainList()
    {
        Assert.True(EventManagerCore.AddEventDefaultGoesToMainList());
        Assert.True(EventManagerCore.AddEventNotMineDefault);
        Assert.True(EventManagerCore.AddEventGoesToMainList(true));
    }

    [Fact]
    public void NotMineIsInvertedSemantics()
    {
        // **NotMine = True → 主列表**（不是挖矿列表）
        Assert.True(EventManagerCore.NotMineIsInvertedSemantics());
        Assert.False(EventManagerCore.AddEventGoesToMainList(false));
    }

    [Fact]
    public void AddEventIsExclusive()
    {
        Assert.True(EventManagerCore.AddEventIsExclusive(true));
        Assert.True(EventManagerCore.AddEventIsExclusive(false));
    }

    // ===================== 构造/析构 =====================

    [Fact]
    public void ThreeListsAndTwoCursors()
    {
        Assert.Equal(3, EventManagerCore.ManagerLists.Length);
        Assert.Equal(2, EventManagerCore.ManagerCursors.Length);
        Assert.Contains("m_ClosedEventList", EventManagerCore.ManagerLists);
        Assert.Contains("m_StoneMineEventList", EventManagerCore.ManagerLists);
    }

    [Fact]
    public void ConstructorZeroesBothCursors()
    {
        Assert.True(EventManagerCore.ConstructorZeroesBothCursors());
    }

    [Fact]
    public void DestructorOrder()
    {
        Assert.Equal(3, EventManagerCore.DestructorOrder.Length);
        Assert.Equal("m_EventList", EventManagerCore.DestructorOrder[0]);
        Assert.Equal("m_StoneMineEventList", EventManagerCore.DestructorOrder[1]);
        Assert.Equal("m_ClosedEventList", EventManagerCore.DestructorOrder[2]);
    }

    [Fact]
    public void ClosedListFreedLast()
    {
        Assert.True(EventManagerCore.ClosedListFreedLast());
    }

    [Fact]
    public void DestructorFreesAllWithoutGrace()
    {
        Assert.True(EventManagerCore.DestructorFreesAllWithoutGrace());
    }

    [Fact]
    public void DestructorForwardOrderIsSafe()
    {
        Assert.True(EventManagerCore.DestructorForwardOrderIsSafe());
    }

    [Fact]
    public void EachListFreesElementsThenList()
    {
        Assert.True(EventManagerCore.EachListFreesElementsThenList());
    }

    // ===================== 旧版 Run / 锁 =====================

    [Fact]
    public void OldRunRetainedAsComment()
    {
        Assert.True(EventManagerCore.OldRunRetainedAsComment());
        Assert.Equal((347, 386), EventManagerCore.OldRunCommentRange);
    }

    [Fact]
    public void OldRunWasReverseFullScan()
    {
        Assert.True(EventManagerCore.OldRunWasReverseFullScan());
    }

    [Fact]
    public void OldRunOrderWasDeleteThenAdd()
    {
        Assert.True(EventManagerCore.OldRunOrderWasDeleteThenAdd());
        Assert.True(EventManagerCore.CurrentOrderIsAddThenDelete());
    }

    [Fact]
    public void OldRunRetainedLockCalls()
    {
        Assert.True(EventManagerCore.OldRunRetainedLockCalls());
    }

    [Fact]
    public void AllLockCallsCommentedOut()
    {
        Assert.True(EventManagerCore.AllLockCallsCommentedOut());
        Assert.Equal(15, EventManagerCore.CommentedLockLines.Length);
    }

    // ===================== 异常处理 =====================

    [Fact]
    public void TwoDistinctExceptionMessages()
    {
        Assert.True(EventManagerCore.TwoDistinctExceptionMessages());
        Assert.Contains("1", EventManagerCore.ExceptionMsg1);
        Assert.Contains("2", EventManagerCore.ExceptionMsg2);
    }

    [Fact]
    public void ExceptionMessagesPerLoop()
    {
        Assert.Equal(EventManagerCore.ExceptionMsg1, EventManagerCore.ExceptionMsgForMainLoop());
        Assert.Equal(EventManagerCore.ExceptionMsg2, EventManagerCore.ExceptionMsgForStoneMineLoop());
    }

    [Fact]
    public void ExceptionsAreSwallowed()
    {
        Assert.True(EventManagerCore.ExceptionsAreSwallowed());
    }

    [Fact]
    public void ExceptionSkipsCursorReset()
    {
        Assert.True(EventManagerCore.ExceptionSkipsCursorReset());
    }
}
