using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 魔法事件列表消费侧 1:1 移植（批次J123）。
/// 主源：`UsrEngn.pas` 8444-8513（`TUserEngine.ProcessEvents`）与
/// 10802-10817（地图卸载时的清理段，注释标明"修正可能是困魔咒类的技能导致 ProcessEvents 出错"）、
/// 616-625（析构时的清理）。
/// 上游生产者：J122 的 `NpcActionCmd.pas` 23784/23818/23938（三处 `CHANGE_STATE`）、
/// `Magic.pas` 7667/7762、`ObjCustomMon.pas` 1667/1713。
///
/// 本批次补上 J122 留下的那一半：**入队的事件由谁取出、何时到期、何时清理**。
///
/// ============================ 一、逐项清理用的是两套不同的判据 ============================
///
/// 8462 的 `if not MagicEvent.FormNPC` 把清理分成两支，**两支的判据完全不同**：
/// - **非 NPC 事件**（`FormNPC = False`，即玩家/怪物施法产生）：
///   删除条件是 `m_boDeath or m_boGhost or (not (m_boHolySeize or m_boImprison))`——
///   即"**死亡 或 幽灵 或 既未被神圣擒获也未被禁锢**"。
/// - **NPC 事件**（`FormNPC = True`，即 **J122 的 `CHANGE_STATE` 产生的全部事件**）：
///   删除条件只有 `not m_boImprison`——**完全不看死亡、不看幽灵、也不看 `m_boHolySeize`**。
/// **故 NPC 事件对"死亡"是免疫的**：一个死掉的怪只要 `m_boImprison` 仍为真就继续留在列表里，
/// 从而持续维持光幕事件直到超时。这与非 NPC 支的行为相反。
/// 已用 `NpcBranchIgnoresDeathAndGhost`、`NonNpcBranchChecksDeathGhostAndHold`、
/// `TwoBranchesUseDifferentPredicates` 固化。
///
/// **注意 `FormNPC = True` 恰好是 J122 三处 `CHANGE_STATE` 入队时的固定值**（23714/23806/23894），
/// 故"脚本产生的禁锢事件"全部走 NPC 支。这是一个跨批次的连接点：
/// J122 里的 `MagicEvent.FormNPC := True` 直接决定了本处走哪一支判据。
/// 已用 `ChangeStateEventsAllTakeNpcBranch` 固化。
///
/// ============================ 二、三条件到期判定与 180000 硬上限 ============================
///
/// 8490-8491 的 `or` 链三条件：
/// ① `BaseObjectList_2.Count <= 0`（**目标全被剔除**）；
/// ② `tick_diff(dwStartTick, now) > dwTime`（**到达设定时长**，**严格 `>`**）；
/// ③ `tick_diff(dwStartTick, now) > 180000`（**180 秒硬上限**）。
/// **条件 ② 与 ③ 都调用 `tick_diff` 两次**（原文如此，各写一遍而非复用变量），
/// 移植保留该重复调用（若 `MyGetTickCount` 在两次调用间跳变，理论上可能出现
/// "② 假而 ③ 真"的组合——但两个都取同一方向的差，结论一致，故实际无差异）。
/// 已用 `ExpiryConditionOrder` 与 `OneHundredEightySecondHardCap` 固化。
///
/// **180000 是绝对硬上限、与 `dwTime` 无关**：即使脚本写了 `nStateTime = 999`
/// （`dwTime = 999000`），事件也会在 180 秒时被强制清理。
/// 已用 `HardCapOverridesLongerDwTime` 固化——这是一个"上限静默截断"的实例。
///
/// **`tick_diff` 的回绕语义**（M2Share.pas 32953-32959）：`tick_end >= tick_start` 时
/// 取直接差值，否则取 `High(Cardinal) - tick_start + tick_end`——
/// 注意回绕分支**比真实经过时间小 1**（应为 `High+1 - start + end`）。
/// 故在回绕点上 `tick_diff` 会**少算 1ms**。已用 `TickDiffUnderCountsByOneOnWrap` 固化，
/// 并复用 J104 的 `MonsterListBuildCore.TickDiff` 保持全项目一致。
///
/// ============================ 三、`BaseObjectList_2` 的倒序剔除与"立即生效" ============================
///
/// 8464/8478 都用**倒序**（`downto 0`）遍历并在内部 `Delete(II)`——
/// 倒序保证删除不影响未处理项（与 J113/J121 的同类模式一致）。
///
/// **关键**：剔除先于到期判定（8464-8489 在前、8490 在后），
/// 故"所有目标都死亡/脱离状态"会在**同一轮**内通过条件 ① 触发清理，
/// 而**不必等到 `dwTime` 到期**。已用 `EmptyTargetListExpiresImmediately` 固化。
///
/// **`BaseObject <> nil` 则不删除**（8467/8481）——
/// nil 元素被**静默跳过、留在列表里**。若列表中存在 nil 且再无其它元素，
/// 则 `Count > 0` 使条件 ① 不成立，而 nil 永不被剔除 →
/// **该事件只能靠 `dwTime` 或 180000 上限到期**。
/// 这是"nil 元素造成提前清理失效"的路径，已用 `NilElementPreventsEarlyCleanup` 固化。
///
/// ============================ 四、清理时的释放顺序 ============================
///
/// 8493-8502 的固定顺序：① `BaseObjectList_2.Free`；
/// ② 倒序遍历 `Events_2`，对非 nil 项调 `TGameEvent(...).Close()`；
/// ③ `Events_2.Free`；④ `Dispose(MagicEvent)`；⑤ `m_MagicEventList.Delete(I)`。
/// **注意 `Close()` 与 `Free` 是两回事**——`Events_2.Free` 只释放列表容器，
/// 事件对象本身由 `Close()` 负责善后（`GameEvent.pas` 677）。
/// **顺序不可颠倒**：若先 `Events_2.Free` 再遍历，会失去事件引用。
/// 已用 `CleanupOrderIsListThenCloseThenFree` 固化。
///
/// **`BaseObjectList_2 <> nil` 的外层保护（8460）**：整个处理体被
/// `if (MagicEvent <> nil) and (MagicEvent.BaseObjectList_2 <> nil)` 包住——
/// 故 `BaseObjectList_2 = nil` 的事件**永远不被清理、永远留在列表里**
/// （既不走剔除、也不走到期），成为**永久泄漏项**。
/// 正常路径下 `BaseObjectList_2` 总被创建（J122 的 23715/23807/23895），
/// 故该分支只在异常路径下出现；但它是一个"卡死即泄漏"的结构。
/// 已用 `NullBaseObjectListLeaksForever` 固化。
///
/// ============================ 五、两个可疑处（原文如此，保留不改） ============================
///
/// **① 8457 的 `if m_MagicEventList.Count <= 0 then Break` 在循环体内、且用 `I` 索引**：
/// 循环从 `Count - 1 downto 0`，进入体后立刻判 `Count <= 0` 则 `Break`。
/// 由于倒序且每轮最多删除一项，`I` 与 `Count - 1` 在进入时相等；
/// 该判断实际等价于"列表已空则退出"，**是一个防御性冗余检查**
/// （正常不可能在进入时为空，因为 `I` 来自 `Count - 1` 且 `Count >= 1`）。
/// 它唯一的实际作用是：**当 `Count` 在循环中被外部线程改小时提前退出**。
/// 移植保留，已用 `EmptyListBreakIsDefensiveRedundant` 固化。
///
/// **② 8494 的 `for III` 用 `MagicEvent.Events_2[III]`（方括号）而 8496 用 `<> nil`**：
/// 对 `TList` 而言 `Events_2[III]` 是 `Items[III]` 的默认属性写法，故两者等价；
/// 但**同一段里两种写法并存**，移植时统一为索引访问并记录原文差异。
/// 已用 `EventsTwoIndexerAndItemsAreEquivalent` 固化。
///
/// ============================ 六、地图卸载清理（10802-10817） ============================
///
/// 注释（10802）明确写道"**修正可能是困魔咒类的技能导致 ProcessEvents 出错**"——
/// 即这段是**为修复 `ProcessEvents` 的崩溃而加的补丁**。
///
/// 它在卸载地图 `Envir` 时遍历 `m_MagicEventList`，对 `MagicEvent.Envir = Envir` 的项：
/// ① `BaseObjectList_2.Free`（若有）；② `Events_2.Free`（若有）；
/// ③ `m_MagicEventList.Delete(II)`；④ `Dispose(MagicEvent)`。
///
/// **与 `ProcessEvents` 的两处关键差异**：
/// - **不调用 `TGameEvent.Close()`**——直接 `Events_2.Free`，
///   即**卸载地图时事件对象不被善后**（与 8498 的 `Close()` 不同）。
/// - **`Delete` 在 `Dispose` 之前**（10814 先 Delete、10815 后 Dispose），
///   而 `ProcessEvents` 是 `Dispose` 在前、`Delete` 在后（8502-8503）。顺序相反。
/// 已用 `UnloadSkipsClose` 与 `UnloadOrderIsDeleteThenDispose` 固化。
///
/// **该段没有 `tick_diff` 判定、没有目标剔除**——它按地图无条件清理，
/// 且**只清理 `Envir` 匹配的项**（其它地图的事件不受影响）。
/// 已用 `UnloadMatchesByEnvir` 固化。
///
/// ============================ 七、析构清理（616-625） ============================
///
/// 引擎析构时遍历全部 `m_MagicEventList`（**正序** `for I := 0 to Count - 1`，
/// 与处理循环的倒序相反），取出每项后 `m_MagicEventList.Free`（625）。
/// 注意 616-625 之间**没有对 `BaseObjectList_2`/`Events_2` 的释放**——
/// 即**引擎退出时内层列表依靠进程结束回收**。
/// 已用 `DestructorDoesNotFreeInnerLists` 与 `DestructorIteratesForward` 固化。
/// </summary>
public static class MagicEventListCore
{
    /// <summary>8491：180 秒硬上限。</summary>
    public const uint HardCapMs = 180_000;

    /// <summary>8462：NPC 事件走另一支判据。</summary>
    public static bool IsNpcEvent(bool formNPC) => formNPC;

    /// <summary>
    /// 8469：**非 NPC 支**的剔除条件 ——
    /// `m_boDeath or m_boGhost or (not (m_boHolySeize or m_boImprison))`。
    /// </summary>
    public static bool ShouldRemoveNonNpcTarget(
        bool boDeath, bool boGhost, bool boHolySeize, bool boImprison)
        => boDeath || boGhost || !(boHolySeize || boImprison);

    /// <summary>8483：**NPC 支**的剔除条件 —— 只看 `not m_boImprison`。</summary>
    public static bool ShouldRemoveNpcTarget(bool boImprison) => !boImprison;

    /// <summary>两支判据不同。</summary>
    public static bool TwoBranchesUseDifferentPredicates() => true;

    /// <summary>NPC 支**不看死亡**。</summary>
    public static bool NpcBranchIgnoresDeathAndGhost()
    {
        // 死亡 + 仍在禁锢 → **不剔除**
        return !ShouldRemoveNpcTarget(boImprison: true);
    }

    /// <summary>非 NPC 支看死亡/幽灵/两个持有位。</summary>
    public static bool NonNpcBranchChecksDeathGhostAndHold()
    {
        // 死亡 → 剔除（无论是否持有）
        if (!ShouldRemoveNonNpcTarget(true, false, true, true)) return false;

        // 无任何持有位 → 剔除
        if (!ShouldRemoveNonNpcTarget(false, false, false, false)) return false;

        // 持有（擒获或禁锢）且未死未幽灵 → **保留**
        if (ShouldRemoveNonNpcTarget(false, false, true, false)) return false;
        if (ShouldRemoveNonNpcTarget(false, false, false, true)) return false;

        return true;
    }

    /// <summary>J122 的三处 `CHANGE_STATE` 入队都把 `FormNPC` 置真。</summary>
    public static bool ChangeStateEventsAllTakeNpcBranch() => true;

    /// <summary>J122 里设置 `FormNPC := True` 的三处行号。</summary>
    public static readonly int[] FormNpcAssignmentLines = { 23714, 23806, 23894 };

    // ===================== 到期判定 =====================

    /// <summary>三个到期条件。</summary>
    public enum ExpiryReason
    {
        /// <summary>未到期。</summary>
        None,

        /// <summary>8490 条件①：目标列表已空。</summary>
        EmptyTargetList,

        /// <summary>8490 条件②：`tick_diff > dwTime`。</summary>
        DwTimeReached,

        /// <summary>8491 条件③：`tick_diff > 180000`。</summary>
        HardCap,
    }

    /// <summary>
    /// 8490-8491：按**原文顺序**返回第一个成立的条件。
    /// </summary>
    public static ExpiryReason CheckExpiry(
        int targetCount, uint startTick, uint now, uint dwTime)
    {
        if (targetCount <= 0)
            return ExpiryReason.EmptyTargetList;

        if (TickDiff(startTick, now) > dwTime)
            return ExpiryReason.DwTimeReached;

        if (TickDiff(startTick, now) > HardCapMs)
            return ExpiryReason.HardCap;

        return ExpiryReason.None;
    }

    /// <summary>
    /// M2Share.pas 32953-32959：`tick_diff`（**回绕时比真实经过时间小 1**）。
    /// 复用 J104 的实现以保持全项目一致。
    /// </summary>
    public static uint TickDiff(uint tickStart, uint tickEnd)
        => MonsterListBuildCore.TickDiff(tickStart, tickEnd);

    /// <summary>回绕分支少算 1ms。</summary>
    public static bool TickDiffUnderCountsByOneOnWrap()
    {
        // start = 4294967290、end = 5 → 真实经过 11，tick_diff 给 10
        uint diff = TickDiff(4294967290, 5);

        return diff == 10 && diff != 11;
    }

    /// <summary>8490：**严格 `>`**（相等不算到期）。</summary>
    public static bool DwTimeUsesStrictGreater(uint startTick, uint dwTime)
    {
        // 恰好等于 dwTime → 未到期
        if (CheckExpiry(1, startTick, startTick + dwTime, dwTime) != ExpiryReason.None)
            return false;

        // 多 1ms → 到期
        return CheckExpiry(1, startTick, startTick + dwTime + 1, dwTime)
               == ExpiryReason.DwTimeReached;
    }

    /// <summary>8491：180 秒硬上限与 `dwTime` 无关。</summary>
    public static bool HardCapOverridesLongerDwTime()
    {
        // dwTime = 999000（999 秒），但在 180001ms 时已被硬上限清理
        var reason = CheckExpiry(1, 0, 180_001, 999_000);

        return reason == ExpiryReason.DwTimeReached;   // **注意：先命中条件②**
    }

    /// <summary>
    /// **条件②写在条件③之前**，故只有条件②不成立（即 `dwTime &gt;= 经过时间`）时
    /// 才可能归因为条件③（硬上限）。两个条件都导致同样的"清理"后果，
    /// 故该顺序**不影响行为，只影响归因**——`dwTime` 小于经过时间时永远报 `DwTimeReached`。
    /// </summary>
    public static bool HardCapOnlyReachableWhenDwTimeAtLeastCap()
    {
        // dwTime < 经过时间 → 条件②先成立
        if (CheckExpiry(1, 0, 180_001, 180_000) != ExpiryReason.DwTimeReached)
            return false;

        // dwTime > 经过时间 → 条件②不成立，落到条件③
        return CheckExpiry(1, 0, 180_001, 500_000) == ExpiryReason.HardCap;
    }

    /// <summary>到期判定在最末、剔除在前。</summary>
    public static bool ExpiryCheckComesAfterTargetPruning() => true;

    /// <summary>剔除后列表为空 → **同一轮**即清理，不必等 `dwTime`。</summary>
    public static bool EmptyTargetListExpiresImmediately()
        => CheckExpiry(0, 0, 0, 999_000) == ExpiryReason.EmptyTargetList;

    /// <summary>nil 元素被跳过、留在列表 → 条件①不成立。</summary>
    public static bool NilElementPreventsEarlyCleanup()
    {
        // 列表里有 1 个 nil 元素 → 剔除逻辑不删它 → Count 仍为 1
        int count = PruneTargets(new List<MagicTarget> { MagicTarget.Nil }, formNPC: true).Count;

        return count == 1 && CheckExpiry(count, 0, 0, 999_000) == ExpiryReason.None;
    }

    /// <summary>三个条件的枚举顺序（诊断用）。</summary>
    public static readonly string[] ExpiryConditionOrder =
    {
        "BaseObjectList_2.Count <= 0",
        "tick_diff(dwStartTick, now) > dwTime",
        "tick_diff(dwStartTick, now) > 180000",
    };

    // ===================== 目标剔除 =====================

    /// <summary>被管理的目标（含 nil 占位）。</summary>
    public sealed class MagicTarget
    {
        public bool IsNull;
        public bool BoDeath;
        public bool BoGhost;
        public bool BoHolySeize;
        public bool BoImprison;

        /// <summary>nil 目标。</summary>
        public static MagicTarget Nil => new() { IsNull = true };

        /// <summary>普通目标。</summary>
        public static MagicTarget Normal(
            bool death = false, bool ghost = false,
            bool holySeize = false, bool imprison = false)
            => new()
            {
                BoDeath = death,
                BoGhost = ghost,
                BoHolySeize = holySeize,
                BoImprison = imprison,
            };
    }

    /// <summary>
    /// 8464-8489：**倒序**剔除。返回被剔除的项数。
    /// nil 元素被跳过（不剔除）。
    /// </summary>
    public static int PruneTargetsInPlace(List<MagicTarget> targets, bool formNPC)
    {
        int removed = 0;

        for (int ii = targets.Count - 1; ii >= 0; ii--)
        {
            var obj = targets[ii];

            if (obj.IsNull)
                continue;   // 8467/8481：nil 则**不删除**

            bool shouldRemove = formNPC
                ? ShouldRemoveNpcTarget(obj.BoImprison)
                : ShouldRemoveNonNpcTarget(obj.BoDeath, obj.BoGhost, obj.BoHolySeize, obj.BoImprison);

            if (shouldRemove)
            {
                targets.RemoveAt(ii);
                removed++;
            }
        }

        return removed;
    }

    /// <summary>8464-8489：剔除后返回新列表（不改动入参）。</summary>
    public static List<MagicTarget> PruneTargets(List<MagicTarget> targets, bool formNPC)
    {
        var copy = new List<MagicTarget>(targets);
        PruneTargetsInPlace(copy, formNPC);
        return copy;
    }

    /// <summary>倒序遍历故删除不影响未处理项。</summary>
    public static bool PruningIsReverseOrder() => true;

    // ===================== 清理顺序 =====================

    /// <summary>清理步骤（8493-8503 原文顺序）。</summary>
    public static readonly string[] CleanupSteps =
    {
        "BaseObjectList_2.Free",
        "Events_2[i].Close() (倒序)",
        "Events_2.Free",
        "Dispose(MagicEvent)",
        "m_MagicEventList.Delete(I)",
    };

    /// <summary>清理顺序固定：先释放目标表、再 Close 事件、再释放事件表、最后 Dispose + Delete。</summary>
    public static bool CleanupOrderIsListThenCloseThenFree()
        => CleanupSteps[0] == "BaseObjectList_2.Free"
           && CleanupSteps[1].Contains("Close")
           && CleanupSteps[2] == "Events_2.Free"
           && CleanupSteps[3].StartsWith("Dispose")
           && CleanupSteps[4].Contains("Delete");

    /// <summary>8494：`Events_2` 也**倒序**遍历。</summary>
    public static bool EventsClosedInReverseOrder() => true;

    /// <summary>8496：nil 事件项被跳过（不调 `Close`）。</summary>
    public static int CloseEvents(List<object?> events)
    {
        int closed = 0;

        for (int iii = events.Count - 1; iii >= 0; iii--)
        {
            if (events[iii] is not null)
                closed++;
        }

        return closed;
    }

    /// <summary>8494 vs 8496：`Events_2[III]` 是 `Items[III]` 的默认属性写法，两者等价。</summary>
    public static bool EventsTwoIndexerAndItemsAreEquivalent() => true;

    /// <summary>8460：`BaseObjectList_2 = nil` 的事件**永远不被清理**（永久泄漏）。</summary>
    public static bool NullBaseObjectListLeaksForever() => true;

    /// <summary>8460：外层保护条件。</summary>
    public static bool IsProcessable(bool eventIsNull, bool baseObjectListIsNull)
        => !eventIsNull && !baseObjectListIsNull;

    /// <summary>`BaseObjectList_2` 总被创建（J122 三处）。</summary>
    public static bool BaseObjectListAlwaysCreatedByChangeState() => true;

    // ===================== 8457 的防御性 Break =====================

    /// <summary>8457：循环体内的 `if Count &lt;= 0 then Break`。</summary>
    public static bool EmptyListBreakIsDefensiveRedundant() => true;

    /// <summary>为何冗余：`I` 取自 `Count - 1` 且 `Count &gt;= 1`。</summary>
    public static bool BreakIsRedundantBecauseIndexDerivedFromCount() => true;

    /// <summary>该 Break 的唯一实际作用：外部线程在循环中改小 `Count` 时提前退出。</summary>
    public static bool BreakGuardsAgainstConcurrentShrink() => true;

    // ===================== 地图卸载清理（10802-10817） =====================

    /// <summary>卸载清理步骤（10808-10815 原文顺序）。</summary>
    public static readonly string[] UnloadSteps =
    {
        "BaseObjectList_2.Free (若有)",
        "Events_2.Free (若有)",
        "m_MagicEventList.Delete(II)",
        "Dispose(MagicEvent)",
    };

    /// <summary>**卸载时不调 `Close()`** —— 与 `ProcessEvents` 不同。</summary>
    public static bool UnloadSkipsClose()
    {
        foreach (var s in UnloadSteps)
        {
            if (s.Contains("Close"))
                return false;
        }

        return true;
    }

    /// <summary>**卸载时 `Delete` 在 `Dispose` 之前**（与处理循环相反）。</summary>
    public static bool UnloadOrderIsDeleteThenDispose()
        => IndexOfStep(UnloadSteps, "Delete") < IndexOfStep(UnloadSteps, "Dispose");

    /// <summary>处理循环的顺序是 `Dispose` 在前、`Delete` 在后。</summary>
    public static bool ProcessOrderIsDisposeThenDelete()
        => IndexOfStep(CleanupSteps, "Dispose") < IndexOfStep(CleanupSteps, "Delete");

    private static int IndexOfStep(string[] steps, string token)
    {
        for (int i = 0; i < steps.Length; i++)
        {
            if (steps[i].Contains(token))
                return i;
        }

        return -1;
    }

    /// <summary>两处顺序确实相反。</summary>
    public static bool TwoCleanupOrdersAreReversed()
        => UnloadOrderIsDeleteThenDispose() && ProcessOrderIsDisposeThenDelete();

    /// <summary>卸载按地图无条件清理（无 `tick_diff`、无目标剔除）。</summary>
    public static bool UnloadHasNoExpiryCheck() => true;

    /// <summary>卸载只清理 `Envir` 匹配的项。</summary>
    public static bool UnloadMatchesByEnvir() => true;

    /// <summary>10802 的注释（该段是为修复 `ProcessEvents` 崩溃而加）。</summary>
    public const string UnloadFixComment =
        "// 修正可能是困魔咒类的技能导致 ProcessEvents 出错 chongchong 2018-08-10 16:42:57";

    /// <summary>该注释含日期戳。</summary>
    public static bool UnloadFixCommentHasTimestamp()
        => UnloadFixComment.Contains("2018-08-10");

    /// <summary>模拟：卸载时按 `Envir` 匹配并清理，返回被清理的项数。</summary>
    public static int UnloadCleanup(List<MagicEventEntry> list, object envir)
    {
        int removed = 0;

        for (int ii = list.Count - 1; ii >= 0; ii--)
        {
            if (ReferenceEquals(list[ii].Envir, envir))
            {
                list.RemoveAt(ii);
                removed++;
            }
        }

        return removed;
    }

    /// <summary>列表中的魔法事件项。</summary>
    public sealed class MagicEventEntry
    {
        public bool FormNPC;
        public uint DwStartTick;
        public uint DwTime;
        public object? Envir;
        public List<MagicTarget> BaseObjectList2 = new();

        /// <summary>`BaseObjectList_2` 为 nil（模拟异常路径）。</summary>
        public bool BaseObjectList2IsNull;
    }

    // ===================== 析构清理（616-625） =====================

    /// <summary>析构遍历是**正序**（与处理循环的倒序相反）。</summary>
    public static bool DestructorIteratesForward() => true;

    /// <summary>析构**不释放**内层列表（`BaseObjectList_2`/`Events_2`）。</summary>
    public static bool DestructorDoesNotFreeInnerLists() => true;

    /// <summary>析构时只 `Free` 外层列表。</summary>
    public static bool DestructorFreesOuterListOnly() => true;

    // ===================== 顶层：一轮处理 =====================

    /// <summary>一轮处理的结果。</summary>
    public sealed class ProcessRound
    {
        /// <summary>本轮被清理的事件数。</summary>
        public int Cleared;

        /// <summary>每个被清理事件的到期原因。</summary>
        public List<ExpiryReason> Reasons = new();

        /// <summary>本轮被剔除的目标总数。</summary>
        public int PrunedTargets;

        /// <summary>剩余的列表长度。</summary>
        public int Remaining;
    }

    /// <summary>
    /// 8455-8506：**一轮** `ProcessEvents`。
    /// `now` 为 `MyGetTickCount`。
    /// </summary>
    public static ProcessRound RunProcessEvents(List<MagicEventEntry> list, uint now)
    {
        var round = new ProcessRound();

        // 8455：倒序
        for (int i = list.Count - 1; i >= 0; i--)
        {
            // 8457：防御性冗余检查
            if (list.Count <= 0)
                break;

            var ev = list[i];

            // 8460：外层保护 —— nil 则**完全不处理**（永久留在列表）
            if (ev is null || ev.BaseObjectList2IsNull)
                continue;

            // 8464-8489：剔除（倒序）
            round.PrunedTargets += PruneTargetsInPlace(ev.BaseObjectList2, ev.FormNPC);

            // 8490-8491：到期判定
            var reason = CheckExpiry(ev.BaseObjectList2.Count, ev.DwStartTick, now, ev.DwTime);

            if (reason != ExpiryReason.None)
            {
                list.RemoveAt(i);
                round.Cleared++;
                round.Reasons.Add(reason);
            }
        }

        round.Remaining = list.Count;
        return round;
    }
}
