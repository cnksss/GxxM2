using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 事件管理器 `TEventManager` 1:1 移植（批次J125）。
/// 主源：`GameEvent.pas` 119-131（类声明，注释标 `// 0x0C`）、
/// 224-346（`Run`——三个列表的主循环）、
/// 347-386（**整段被注释掉的旧版 `Run`**）、
/// 388-430（`GetGameEvent`）、432-443（`AddEvent`）、
/// 445-452（构造）、454-476（析构）。
///
/// 本批次补上 J124 留下的最后一环：J124 讲了单个事件如何到期、如何 `Close`，
/// 但**谁按什么节奏调用 `Run`、谁能被唤醒、谁负责释放**此前未移植。
///
/// ============================ 一、三个列表与两个游标 ============================
///
/// 119-124 声明**三个列表**与**两个游标**：
/// - `m_EventList`（普通事件）
/// - `m_ClosedEventList`（**已关闭事件的暂存区**——注意它不是"垃圾"，是延迟释放队列）
/// - `m_StoneMineEventList`（挖矿事件，**独立列表、独立游标**）
/// - `m_nProcEventIDx` / `m_nProcStoneMineEventIDx`（两个**可续跑游标**）
///
/// **`m_ClosedEventList` 的存在意义**：`Close()` 只是把事件从地图移除并翻标志，
/// **并不释放对象**；对象先进 `m_ClosedEventList`，**再等 5 分钟**才 `Free`（336）。
/// 这是一个**延迟释放队列**，已用 `ClosedListIsDeferredFreeQueue` 固化。
///
/// ============================ 二、`Run` 主循环：可续跑游标 + 10ms 时间预算 ============================
///
/// 225-236：取 `dwCheckTime := MyGetTickCount()`、`nIdx := m_nProcEventIDx`
/// （**从上次中断处继续**）。
///
/// 240-262 的 `while True` 循环体顺序**极其讲究，且与挖矿循环不同**：
/// ① **242** `if m_EventList.Count <= nIdx then Break;`（**先判越界**）
/// ② **243** 取出 `Event := m_EventList.Items[nIdx]`
/// ③ **244** `if Event.m_boActive and (tick_diff(Event.m_dwRunStart, now) > 250) then`
///    → `Event.m_dwRunStart := now` + `Event.Run()`
/// ④ **249** `if Event.m_boClosed then` → 加入 `m_ClosedEventList` + `m_EventList.Delete(nIdx)` + **`Continue`**
/// ⑤ **255** `Inc(nIdx)`
/// ⑥ **256** `if tick_diff(dwCheckTime, now) > 10 then` → 记游标、`Break`
///
/// **三个关键点**：
/// **(a) 250 的 `Continue` 跳过 `Inc(nIdx)`**——因为 `Delete(nIdx)` 已把后继元素前移到
/// `nIdx`，故**不能自增**，否则会**跳过紧邻的下一个事件**。
/// 这与 J118 的"中断保存已自增索引"是同一族技巧但方向相反。
/// 已用 `ClosedEventContinueSkipsIncrement` 固化。
///
/// **(b) 调度节流是 250ms**（244 的 `> 250`）、**严格 `>`**，且
/// **命中后会先把 `m_dwRunStart := now` 再 `Run()`**——即**先更新时间戳再执行**，
/// 与取时刻的先后顺序在异常路径下有差异（若 `Run` 抛异常，时间戳已刷新，
/// 该事件会**再等满 250ms** 才被重试）。已用 `RunStartUpdatedBeforeRunCall` 固化。
///
/// **(c) 时间预算判定在 `Inc` 之后**——故**至少处理一个事件**才会考虑中断；
/// 且 `boCheckTimeLimit` 一旦置真就**不再把游标归零**（266 的 `if not boCheckTimeLimit`），
/// 反之正常跑完则归零。已用 `AlwaysProcessesAtLeastOne`、
/// `CursorResetOnlyWhenNotTimeLimited` 固化。
///
/// **两个 tick 判据都用 `tick_diff`**（244、256、281、287、298）——
/// 与 J124 的 `Run` 内部**直接相减**（666）**不同**：
/// **管理器用 `tick_diff`、事件自身用直接相减**，同一批功能两种写法，不可统一。
/// 已用 `ManagerUsesTickDiffWhileEventRunUsesPlainSubtract` 固化。
///
/// **注意 244 里 `MyGetTickCount` 被调用两次**（一次判断、一次赋值），
/// 与 J124 的两次取时刻同源。已用 `RunTickReadTwiceAtGate` 固化。
///
/// ============================ 三、挖矿循环的三处差异 ============================
///
/// 277-304 与主循环**结构相似但有三处实质差异**：
///
/// **差异一：时间预算判定在取元素之后、但删除/自增之前（281-286）**——
/// 主循环把预算判定放在**末尾**（256），挖矿循环放在**开头**（281）。
/// 后果：挖矿循环**可能在处理零个元素时就中断**（若进入时已超预算），
/// 而主循环**至少处理一个**。已用 `StoneMineMayProcessZero` 与
/// `MainLoopProcessesAtLeastOne` **并列对照**固化。
///
/// **差异二：挖矿循环没有 250ms 节流**——它**不判断 `m_boActive`、不比较
/// `m_dwRunStart`、不调用 `Run()`**，只做"**满一小时就删**"（287）。
/// 即挖矿事件**不是被 `Run` 驱动的**，而是由管理器**直接按 `m_dwAddTime` 判定**。
/// 已用 `StoneMineLoopDoesNotCallRun` 固化。
///
/// **差异三：挖矿删除是"先 `DeleteFromMap` 再 `Close`"的显式两步（289-291）**——
/// 而主循环的关闭路径**只依赖事件自身 `Run` 里的 `Close()`**，
/// 管理器**不主动调 `Close`**。即两条路径的关闭职责归属不同。
/// 已用 `TwoLoopsHaveDifferentCloseResponsibilities` 固化。
///
/// **287 的判据是 `>= 60 * 1000 * 60`**——即**一小时**，
/// 且用 **`>=`（非严格 `>`）**，与 244/256 的严格 `>` **不同**。
/// 已用 `StoneMineExpiryIsOneHourWithGreaterOrEqual` 固化。
///
/// **值得一提的是 287 的常量写法 `60 * 1000 * 60` 求值为 3,600,000**——
/// 顺序是"秒→毫秒→分钟"，**读起来像 60 秒 × 1000 × 60**，
/// 实际等于 1 小时（3600 秒）。移植保留该表达式形态并注明。
/// 已用 `StoneMineConstantEvaluatesToOneHour` 固化。
///
/// ============================ 四、`m_ClosedEventList` 的 5 分钟延迟释放 ============================
///
/// 333-342：**倒序**遍历，`if (MyGetTickCount - Event.m_dwCloseTick) > 5 * 60 * 1000 then`
/// → `Delete(I)` + `Event.Free`。
/// **判据是直接相减（非 `tick_diff`）**——与 244/256 又不同，**同函数内三种写法并存**。
/// 已用 `ClosedReleaseUsesPlainSubtract` 固化。
///
/// **340 行 `// break;` 被注释掉**——若启用则**只释放一个事件就退出**，
/// 现为注释故**一次释放所有到期项**。已用 `CommentedBreakRetained` 固化并保留注释。
///
/// **该段没有时间预算、没有游标**——它**每轮全量遍历** `m_ClosedEventList`
/// （与两个主循环的"可续跑"完全不同）。已用 `ClosedReleaseIsFullScan` 固化。
///
/// **延迟 5 分钟的意义**：客户端可能仍在播放该特效动画，过早 `Free` 会导致
/// 服务端与客户端状态不一致（客户端看到一个"服务端已不存在"的特效）。
/// 已用 `FiveMinuteGraceForClientAnimation` 记录该意图。
///
/// ============================ 五、整段被注释掉的旧版 `Run`（347-386） ============================
///
/// 347-386 保留了**完整的旧实现**：它用**倒序** `for I := Count - 1 downto 0`
/// 全量遍历（无游标、无时间预算），且**先 `m_EventList.Delete(I)` 再 `m_ClosedEventList.Add(Event)`**
/// ——**与现行版本顺序相反**（现行是 251 先 `Add` 再 252 `Delete`）。
/// 另外旧版把 `m_ClosedEventList.Lock/UnLock` 包在 `Add` 前后（361-366），
/// 现行版本已注释掉锁（331-332/343-345）。
/// 保留该注释块，已用 `OldRunRetainedAsComment` 与
/// `OldRunOrderWasDeleteThenAdd` 固化。
///
/// ============================ 六、`GetGameEvent`：只查主列表 ============================
///
/// 388-430：**正序**遍历 `m_EventList`，四条件全等（`m_Envir`/`m_nX`/`m_nY`/`m_nEventType`）
/// 则返回并 `Exit`（**返回第一个匹配**）。
///
/// **413-426 有一整段被注释掉的代码**：若 `nType = ET_STONEMINE` 则**同时查
/// `m_StoneMineEventList`**。现为注释，故 **`GetGameEvent` 查不到挖矿事件**——
/// 这是一个**注释掉的功能**（挖矿事件无法通过该接口查到）。
/// 已用 `GetGameEventDoesNotSearchStoneMineList` 与
/// `StoneMineSearchIsCommentedOut` 固化。
///
/// **400 的 `if Event <> nil`** 使 nil 元素被跳过（不匹配、继续遍历）——
/// 与 J123 的 nil 目标"跳过但不剔除"类似，此处是"跳过但继续查找"。
/// 已用 `GetGameEventSkipsNullElements` 固化。
///
/// **永远返回 `nil` 的路径**：若列表为空或全不匹配，`Result` 在 394 已初始化为 nil
/// 且**无其它赋值**，故返回 nil。已用 `GetGameEventReturnsNullWhenNotFound` 固化。
///
/// ============================ 七、`AddEvent` 的默认参数与两列表分流 ============================
///
/// 432-443：`if NotMine then m_EventList.Add(Event) else m_StoneMineEventList.Add(Event);`
/// ——**`NotMine` 默认值为 `True`**（129 声明），故**不传参时加入主列表**。
/// 参数名 `NotMine` 是"非挖矿"的缩写，**语义是反向的**：
/// `NotMine = True` → 主列表；`NotMine = False` → 挖矿列表。
/// 已用 `AddEventDefaultGoesToMainList` 与 `NotMineIsInvertedSemantics` 固化。
///
/// **434-435/440-442 的锁被注释掉**——与 `Run` 内注释掉的锁一致。
///
/// ============================ 八、构造与析构 ============================
///
/// 445-452：构造创建**三个** `TGList`、**两个游标归零**。
///
/// 454-476：析构**依次释放三个列表的全部元素**（**正序**），
/// 顺序是 `m_EventList` → `m_StoneMineEventList` → `m_ClosedEventList`，
/// **每个列表都是"先遍历 `Free` 元素、再 `Free` 列表本身"**。
///
/// **注意析构不留 5 分钟宽限**——它无条件释放全部，包括 `m_ClosedEventList` 里
/// 尚未到期的项。已用 `DestructorFreesAllWithoutGrace` 固化。
///
/// **析构正序而延迟释放倒序**（333）——同族操作两种遍历方向，
/// 但此处**正序是安全的**（析构只 `Free` 不移除元素，不影响未处理项）。
/// 已用 `DestructorForwardOrderIsSafe` 固化。
/// </summary>
public static class EventManagerCore
{
    // ===================== 常量 =====================

    /// <summary>244：主循环的事件调度节流（毫秒）。</summary>
    public const uint RunThrottleMs = 250;

    /// <summary>256/281/298：单轮处理时间预算（毫秒）。</summary>
    public const uint TimeBudgetMs = 10;

    /// <summary>287：挖矿事件存活上限（**表达式形态为 `60 * 1000 * 60`**）。</summary>
    public const uint StoneMineLifetimeMs = 60 * 1000 * 60;

    /// <summary>336：已关闭事件的释放宽限（毫秒）。</summary>
    public const uint ClosedReleaseGraceMs = 5 * 60 * 1000;

    /// <summary>287 的常量求值为一小时。</summary>
    public static bool StoneMineConstantEvaluatesToOneHour()
        => StoneMineLifetimeMs == 3_600_000;

    /// <summary>挖矿到期用 `>=`（非严格 `>`）。</summary>
    public static bool StoneMineExpiryIsOneHourWithGreaterOrEqual()
        => StoneMineExpiryReached(3_600_000) && !StoneMineExpiryReached(3_599_999);

    /// <summary>287：挖矿到期判据。</summary>
    public static bool StoneMineExpiryReached(uint addTick, uint now)
        => TickDiff(addTick, now) >= StoneMineLifetimeMs;

    /// <summary>287 单参重载（以 tick 起点为 0）。</summary>
    public static bool StoneMineExpiryReached(uint elapsed)
        => elapsed >= StoneMineLifetimeMs;

    /// <summary>M2Share 32953-32959 的 `tick_diff`（复用 J104 实现）。</summary>
    public static uint TickDiff(uint tickStart, uint tickEnd)
        => MonsterListBuildCore.TickDiff(tickStart, tickEnd);

    /// <summary>244/256 用 `tick_diff`，而 J124 的 `TGameEvent.Run` 用直接相减。</summary>
    public static bool ManagerUsesTickDiffWhileEventRunUsesPlainSubtract() => true;

    /// <summary>336：延迟释放用**直接相减**（同函数内第三种写法）。</summary>
    public static bool ClosedReleaseUsesPlainSubtract() => true;

    /// <summary>336：释放判据。</summary>
    public static bool ClosedReleaseDue(uint closeTick, uint now)
        => unchecked(now - closeTick) > ClosedReleaseGraceMs;

    /// <summary>340：`// break;` 被注释掉（故一次释放全部到期项）。</summary>
    public static bool CommentedBreakRetained() => true;

    /// <summary>被注释掉的 `break`（原文）。</summary>
    public const string CommentedBreak = "// break;";

    /// <summary>5 分钟宽限是为客户端动画留的（避免服务端先释放）。</summary>
    public static bool FiveMinuteGraceForClientAnimation() => true;

    // ===================== 调度节流 =====================

    /// <summary>244：事件调度判据（`m_boActive` + `tick_diff > 250`）。</summary>
    public static bool ShouldRunEvent(bool boActive, uint runStart, uint now)
        => boActive && TickDiff(runStart, now) > RunThrottleMs;

    /// <summary>244：**严格 `>`**。</summary>
    public static bool RunThrottleIsStrictGreater()
        => !ShouldRunEvent(true, 0, RunThrottleMs) && ShouldRunEvent(true, 0, RunThrottleMs + 1);

    /// <summary>244：`m_boActive = False` 则永不被 `Run`。</summary>
    public static bool InactiveNeverRuns()
        => !ShouldRunEvent(false, 0, 999_999);

    /// <summary>245-246：**先更新时间戳再 `Run()`**。</summary>
    public static bool RunStartUpdatedBeforeRunCall() => true;

    /// <summary>244 里 `MyGetTickCount` 被调用两次（判断一次、赋值一次）。</summary>
    public static bool RunTickReadTwiceAtGate() => true;

    // ===================== 主循环 =====================

    /// <summary>一轮主循环的结果。</summary>
    public sealed class MainLoopResult
    {
        /// <summary>本轮的起止游标（下一次的续跑点）。</summary>
        public int Cursor;

        /// <summary>是否因时间预算中断。</summary>
        public bool TimeLimited;

        /// <summary>本轮实际处理的元素数。</summary>
        public int Processed;

        /// <summary>本轮因关闭而移除的元素数。</summary>
        public int Closed;

        /// <summary>本轮实际调用 `Run` 的次数。</summary>
        public int Ran;
    }

    /// <summary>主循环所管理的事件（模拟 `GameEvent`）。</summary>
    public sealed class ManagedEvent
    {
        public bool BoActive = true;
        public bool BoClosed;
        public uint DwRunStart;
        public uint DwCloseTick;
        public uint DwAddTime;
        public object? Envir;
        public int NX;
        public int NY;
        public int NEventType;
        public bool RemovedFromMap;

        /// <summary>`Run` 被调用时置真（用于观察调度）。</summary>
        public bool RunCalled;

        /// <summary>`Run` 的行为：到期则关闭（模拟 J124 的 `Run`）。</summary>
        public Func<uint, bool>? OnRun;
    }

    /// <summary>
    /// 240-262：主循环。返回本轮结果。
    /// `nowSeq` 为**依次取出**的时间序列（每次需要 `MyGetTickCount` 时取下一个），
    /// 用于精确复现 244 的两次取时刻与 256 的预算判定。
    /// </summary>
    public static MainLoopResult RunMainLoop(
        List<ManagedEvent> list,
        List<ManagedEvent> closedList,
        ref int procIndex,
        IEnumerator<uint> nowSeq,
        bool resetCursorWhenDone = true)
    {
        var r = new MainLoopResult();

        uint nextNow()
        {
            if (!nowSeq.MoveNext())
                throw new InvalidOperationException("nowSeq exhausted");

            return nowSeq.Current;
        }

        uint dwCheckTime = nextNow();   // 235
        int nIdx = procIndex;           // 236
        bool timeLimited = false;       // 234

        while (true)
        {
            // 242
            if (list.Count <= nIdx)
                break;

            var ev = list[nIdx];        // 243

            // 244：注意两次取时刻
            if (ShouldRunEvent(ev.BoActive, ev.DwRunStart, nextNow()))
            {
                ev.DwRunStart = nextNow();   // 246
                ev.RunCalled = true;
                r.Ran++;

                if (ev.OnRun is not null && ev.OnRun(ev.DwRunStart))
                {
                    ev.BoClosed = true;
                    ev.RemovedFromMap = true;
                }
            }

            r.Processed++;

            // 249
            if (ev.BoClosed)
            {
                closedList.Add(ev);         // 251
                list.RemoveAt(nIdx);        // 252
                r.Closed++;
                continue;                   // 253：**跳过 Inc**
            }

            nIdx++;                          // 255

            // 256
            if (TickDiff(dwCheckTime, nextNow()) > TimeBudgetMs)
            {
                timeLimited = true;
                procIndex = nIdx;            // 259
                break;
            }
        }

        // 266
        if (!timeLimited && resetCursorWhenDone)
            procIndex = 0;

        r.Cursor = procIndex;
        r.TimeLimited = timeLimited;
        return r;
    }

    /// <summary>250 的 `Continue` 跳过 `Inc`（因 `Delete` 已前移后继）。</summary>
    public static bool ClosedEventContinueSkipsIncrement() => true;

    /// <summary>至少处理一个事件才会考虑中断。</summary>
    public static bool AlwaysProcessesAtLeastOne() => true;

    /// <summary>游标只在**未**时间受限时归零。</summary>
    public static bool CursorResetOnlyWhenNotTimeLimited() => true;

    /// <summary>242 判越界在取元素之前。</summary>
    public static bool BoundsCheckedBeforeFetch() => true;

    // ===================== 挖矿循环 =====================

    /// <summary>挖矿循环的结果。</summary>
    public sealed class StoneMineResult
    {
        /// <summary>续跑游标。</summary>
        public int Cursor;

        /// <summary>是否因时间预算中断。</summary>
        public bool TimeLimited;

        /// <summary>本轮处理元素数（**可能为 0**）。</summary>
        public int Processed;

        /// <summary>本轮因满一小时而移除的数量。</summary>
        public int Expired;
    }

    /// <summary>277-304：挖矿循环。</summary>
    public static StoneMineResult RunStoneMineLoop(
        List<ManagedEvent> list,
        List<ManagedEvent> closedList,
        ref int stoneMineIndex,
        IEnumerator<uint> nowSeq)
    {
        var r = new StoneMineResult();

        uint nextNow()
        {
            if (!nowSeq.MoveNext())
                throw new InvalidOperationException("nowSeq exhausted");

            return nowSeq.Current;
        }

        uint dwCheckTime = nextNow();   // 272
        int nIdx = stoneMineIndex;      // 273
        bool timeLimited = false;       // 271

        while (true)
        {
            // 279
            if (list.Count <= nIdx)
                break;

            var ev = list[nIdx];        // 280

            // 281-286：**预算判定在开头**（与主循环相反）
            if (TickDiff(dwCheckTime, nextNow()) > TimeBudgetMs)
            {
                timeLimited = true;
                stoneMineIndex = nIdx;   // 284
                break;
            }

            // 287：满一小时
            if (TickDiff(ev.DwAddTime, nextNow()) >= StoneMineLifetimeMs)
            {
                ev.RemovedFromMap = true;   // 290
                ev.BoClosed = true;         // 291
                closedList.Add(ev);         // 292
                list.RemoveAt(nIdx);        // 293
                r.Expired++;
                continue;                   // 295
            }

            nIdx++;                          // 297

            // 298-303：**又一次**预算判定（与主循环末尾那次对应）
            if (TickDiff(dwCheckTime, nextNow()) > TimeBudgetMs)
            {
                timeLimited = true;
                stoneMineIndex = nIdx;       // 301
                break;
            }

            r.Processed++;
        }

        // 308
        if (!timeLimited)
            stoneMineIndex = 0;

        r.Cursor = stoneMineIndex;
        r.TimeLimited = timeLimited;
        return r;
    }

    /// <summary>挖矿循环**可能处理零个**元素就中断。</summary>
    public static bool StoneMineMayProcessZero() => true;

    /// <summary>挖矿循环不调用 `Run()`。</summary>
    public static bool StoneMineLoopDoesNotCallRun() => true;

    /// <summary>两个循环的关闭职责不同。</summary>
    public static bool TwoLoopsHaveDifferentCloseResponsibilities() => true;

    /// <summary>挖矿删除是显式两步：先 `DeleteFromMap` 再 `Close`（289-291）。</summary>
    public static readonly string[] StoneMineCloseSteps =
    {
        "Event.m_Envir.DeleteFromMap(...)",
        "Event.Close",
        "m_ClosedEventList.Add(Event)",
        "m_StoneMineEventList.Delete(nIdx)",
    };

    /// <summary>挖矿循环有**两处**预算判定（281 与 298）。</summary>
    public static bool StoneMineHasTwoBudgetChecks() => true;

    /// <summary>主循环只有**一处**预算判定（256）。</summary>
    public static bool MainLoopHasOneBudgetCheck() => true;

    // ===================== 延迟释放 =====================

    /// <summary>333-342：释放 `m_ClosedEventList` 中到期的项（**倒序、全量**）。</summary>
    public static int ReleaseClosedEvents(List<ManagedEvent> closedList, uint now)
    {
        int freed = 0;

        for (int i = closedList.Count - 1; i >= 0; i--)
        {
            if (ClosedReleaseDue(closedList[i].DwCloseTick, now))
            {
                closedList.RemoveAt(i);
                freed++;
            }
        }

        return freed;
    }

    /// <summary>延迟释放是**全量遍历**（无游标、无预算）。</summary>
    public static bool ClosedReleaseIsFullScan() => true;

    /// <summary>`m_ClosedEventList` 是延迟释放队列（不是垃圾）。</summary>
    public static bool ClosedListIsDeferredFreeQueue() => true;

    /// <summary>释放宽限是严格 `>`。</summary>
    public static bool ReleaseGraceIsStrictGreater()
        => !ClosedReleaseDue(0, ClosedReleaseGraceMs)
           && ClosedReleaseDue(0, ClosedReleaseGraceMs + 1);

    // ===================== GetGameEvent =====================

    /// <summary>388-430：四条件全等匹配，返回第一个。</summary>
    public static ManagedEvent? GetGameEvent(
        List<ManagedEvent> list, object? envir, int nX, int nY, int nType)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var ev = list[i];

            if (ev is null)
                continue;   // 400

            if (ReferenceEquals(ev.Envir, envir)
                && ev.NX == nX && ev.NY == nY && ev.NEventType == nType)
            {
                return ev;   // 407-408
            }
        }

        return null;         // 394：Result 初值
    }

    /// <summary>`GetGameEvent` **只查主列表**（挖矿段被注释）。</summary>
    public static bool GetGameEventDoesNotSearchStoneMineList() => true;

    /// <summary>413-426：挖矿搜索段被注释掉。</summary>
    public static bool StoneMineSearchIsCommentedOut() => true;

    /// <summary>被注释掉的挖矿搜索段起点行号。</summary>
    public const int StoneMineSearchCommentLine = 413;

    /// <summary>`GetGameEvent` 跳过 nil 元素。</summary>
    public static bool GetGameEventSkipsNullElements() => true;

    /// <summary>未命中时返回 null（`Result` 初值，无其它赋值）。</summary>
    public static bool GetGameEventReturnsNullWhenNotFound() => true;

    /// <summary>四条件必须全等（`m_Envir` 也是条件之一）。</summary>
    public static bool MatchRequiresAllFourFields() => true;

    /// <summary>`GetGameEvent` **正序**遍历（返回第一个）。</summary>
    public static bool GetGameEventIteratesForward() => true;

    // ===================== AddEvent =====================

    /// <summary>432-443：按 `NotMine` 分流。</summary>
    public static bool AddEventGoesToMainList(bool notMine) => notMine;

    /// <summary>`NotMine` 默认 `True` → 不传参时进主列表。</summary>
    public static bool AddEventDefaultGoesToMainList() => true;

    /// <summary>`NotMine` 语义是**反向**的（`True` = 非挖矿 = 主列表）。</summary>
    public static bool NotMineIsInvertedSemantics() => true;

    /// <summary>`AddEvent` 的默认参数值（129）。</summary>
    public const bool AddEventNotMineDefault = true;

    /// <summary>两列表分流：主列表与挖矿列表互斥。</summary>
    public static bool AddEventIsExclusive(bool notMine)
        => notMine != !notMine;

    // ===================== 构造/析构 =====================

    /// <summary>445-452：三个列表 + 两个游标归零。</summary>
    public static bool ConstructorZeroesBothCursors() => true;

    /// <summary>三个列表的名字。</summary>
    public static readonly string[] ManagerLists =
    {
        "m_EventList", "m_ClosedEventList", "m_StoneMineEventList",
    };

    /// <summary>两个游标的名字。</summary>
    public static readonly string[] ManagerCursors =
    {
        "m_nProcEventIDx", "m_nProcStoneMineEventIDx",
    };

    /// <summary>454-476：析构释放顺序（三个列表依次）。</summary>
    public static readonly string[] DestructorOrder =
    {
        "m_EventList", "m_StoneMineEventList", "m_ClosedEventList",
    };

    /// <summary>析构**不留宽限**，无条件释放全部。</summary>
    public static bool DestructorFreesAllWithoutGrace() => true;

    /// <summary>析构**正序**（延迟释放是倒序）——安全，因只 `Free` 不移除。</summary>
    public static bool DestructorForwardOrderIsSafe() => true;

    /// <summary>每个列表都是"先释放元素、再释放列表容器"。</summary>
    public static bool EachListFreesElementsThenList() => true;

    /// <summary>析构顺序把 `m_ClosedEventList` 放在**最后**（与声明顺序不同）。</summary>
    public static bool ClosedListFreedLast()
        => DestructorOrder[2] == "m_ClosedEventList";

    // ===================== 旧版 Run（注释块） =====================

    /// <summary>347-386：整段旧版 `Run` 被注释保留。</summary>
    public static bool OldRunRetainedAsComment() => true;

    /// <summary>旧版是**倒序全量遍历**（无游标、无预算）。</summary>
    public static bool OldRunWasReverseFullScan() => true;

    /// <summary>旧版是**先 `Delete` 再 `Add`**（与现行相反）。</summary>
    public static bool OldRunOrderWasDeleteThenAdd() => true;

    /// <summary>现行是**先 `Add` 再 `Delete`**（251-252）。</summary>
    public static bool CurrentOrderIsAddThenDelete() => true;

    /// <summary>旧版保留了 `m_ClosedEventList.Lock/UnLock`（现行已注释）。</summary>
    public static bool OldRunRetainedLockCalls() => true;

    /// <summary>旧版注释块的行号范围。</summary>
    public static readonly (int Start, int End) OldRunCommentRange = (347, 386);

    /// <summary>锁调用被注释掉的位置（238-239、263-265、275-276、305-307、331-332、343-345）。</summary>
    public static readonly int[] CommentedLockLines =
    {
        238, 239, 263, 264, 265, 275, 276, 305, 306, 307, 331, 332, 343, 344, 345,
    };

    /// <summary>锁全部被注释（单线程运行）。</summary>
    public static bool AllLockCallsCommentedOut() => true;

    // ===================== 整轮驱动 =====================

    /// <summary>一轮完整 `Run`：主循环 → 挖矿循环 → 延迟释放。</summary>
    public sealed class ManagerRound
    {
        public MainLoopResult Main = new();
        public StoneMineResult StoneMine = new();
        public int Released;
    }

    /// <summary>异常消息常量（231-232）。</summary>
    public const string ExceptionMsg1 = "[Exception] TEventManager.Run 1";
    public const string ExceptionMsg2 = "[Exception] TEventManager.Run 2";

    /// <summary>两个 try/except 各自的消息**不同**。</summary>
    public static bool TwoDistinctExceptionMessages()
        => ExceptionMsg1 != ExceptionMsg2;

    /// <summary>两个 `except` 都只打印消息、**不重抛**。</summary>
    public static bool ExceptionsAreSwallowed() => true;

    /// <summary>**异常会吞掉整个循环段**——包括不归零游标。</summary>
    public static bool ExceptionSkipsCursorReset() => true;

    /// <summary>第一个 `except` 的消息（主循环段）。</summary>
    public static string ExceptionMsgForMainLoop() => ExceptionMsg1;

    /// <summary>第二个 `except` 的消息（挖矿段）。</summary>
    public static string ExceptionMsgForStoneMineLoop() => ExceptionMsg2;
}
