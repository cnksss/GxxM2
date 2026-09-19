using System;
using GXX.Core.Protocol;

namespace GXX.M2Server;

/// <summary>
/// `TUserEngine.RunMonster` 的**怪物调度节流** 1:1 移植（批次J105），主源两处：
/// `UsrEngn.pas` 4112-4164（`FRunMonsterList` 主循环）与
/// `ObjBase.pas` 11375-11376（`m_boIsVisibleActive` / `m_nProcessRunCount` 的初值）。
///
/// **这是本项目"性能优化改变了游戏行为"最典型的一段代码**，注释即写明
/// `优化刷怪CPU占用 (添加下面的代码) chongchong 2013-12-24`。
///
/// **每个怪物每轮依次经过三道判定**：
///
/// **第一道：是否重新搜索视野（4115-4136）**
/// 前置条件 `(dwCurrentTick - m_dwSearchTick) > m_dwSearchTime` **且** `m_PEnvir &lt;&gt; nil`；
/// 满足后再看"地图上是否有人"：
/// - 有人（`HumCount > 0` 或 `HumBBCount > 0` 或中毒 `POISON_DECHEALTH > 0`）
///   → 刷新 `m_dwSearchTick` 为当前时刻，并调用 `SearchViewRange()`；
/// - **无人** → **清空六项战斗引用**（`m_CurrTarget`/`m_TargetCret`/`m_LastHiter`/
///   `m_ExpHitter`/`m_PoisonHitter`/`m_CurrTargetEx`）并 `ClearObject`，
///   **但`m_dwSearchTick` 不刷新**——故下一轮仍会再次进入本分支反复清理。
///
/// **第二道：节流计数（4138-4142）** —— 本批次的核心
/// `if not m_boIsVisibleActive and (m_nProcessRunCount &lt; g_Config.nProcessMonsterInterval) then
///    Inc(m_nProcessRunCount)`
/// 即 **"视野内没有玩家/宝宝" 且 "计数未达上限" 时，本轮不执行 `Run`，只把计数 +1**。
/// `nProcessMonsterInterval` 默认值为 **2**（M2Share.pas 4576），另在 `FSrvValue.pas` 329
/// 出现过硬编码 `3`（配置界面重置路径），实际值由配置读写决定。
/// **`m_nProcessRunCount` 声明为 `ShortInt`（ObjBase.pas 321，即**有符号 8 位**）**——
/// 上限虽受 `< interval` 约束（interval 正常为正），但类型选得有符号这一点按原文保留。
///
/// **第三道：真正运行或静默（4144-4163）**
/// 未进入第二道（即**视野内有人**，或计数已达上限）时，再判"是否值得运行"：
/// `m_PEnvir &lt;&gt; nil` 且（地图有人 **或** 距 `ClearHumOrBBTick` **不超过 600000ms**
/// **或** 自己 `m_boGhost` **或** 自己 `m_boDeath` **或** 中毒）；
/// 满足 → `m_nProcessRunCount := 0`（**计数清零**）并 `Monster.Run`；
/// 不满足但 `m_PEnvir &lt;&gt; nil` → 同样清空六项战斗引用并 `ClearObject`（**但不清零计数**）。
///
/// **注意"计数清零"只发生在真正 `Run` 的那条路径上**（4150），
/// 静默路径（4154-4163）**不清零**。这意味着计数会一直累积到 interval 上限，
/// 之后每轮都走第三道——即在无人地图上，怪物从"每 N 轮动一次"退化为"每轮都检查但基本不做事"。
/// 这正是"超过人物退出 10 分钟才不运行"（4144 注释）的语义：
/// 前 10 分钟仍会 `Run`（因为 `ClearHumOrBBTick` 在窗口内），10 分钟后才彻底静默。
/// </summary>
public static class MonsterScheduleThrottleCore
{
    /// <summary>4138：`g_Config.nProcessMonsterInterval` 的默认值（M2Share.pas 4576）。</summary>
    public const int DefaultProcessMonsterInterval = 2;

    /// <summary>4146：`ClearHumOrBBTick` 的宽限窗口 600000ms（10 分钟）。</summary>
    public const uint ClearHumOrBBTickGraceMs = 600_000;

    /// <summary>怪物本轮应采取的动作。</summary>
    public enum MonsterStep
    {
        /// <summary>4138-4142：本轮跳过 `Run`，仅把 `m_nProcessRunCount` 加一。</summary>
        IncrementAndSkip,

        /// <summary>4149-4152：计数清零并执行 `Monster.Run`。</summary>
        ResetAndRun,

        /// <summary>4154-4163：清空战斗引用并 `ClearObject`（**不清零计数**）。</summary>
        SilentClear,

        /// <summary>4136/4142 之外：`m_PEnvir = nil`，什么都不做。</summary>
        DoNothing,
    }

    /// <summary>怪物本轮的第一道动作：是否重搜视野 / 静默清理 / 不处理。</summary>
    public enum SearchStep
    {
        /// <summary>4115 前置条件不满足，本次不做视野处理。</summary>
        Skip,

        /// <summary>4122-4124：刷新 `m_dwSearchTick` 并调用 `SearchViewRange()`。</summary>
        RefreshTickAndSearch,

        /// <summary>4128-4134：清空六项战斗引用并 `ClearObject`（不刷新 tick）。</summary>
        ClearAndReset,
    }

    /// <summary>
    /// 4115-4136：第一道——视野重搜判定。
    /// 前置：`(dwCurrentTick - m_dwSearchTick) &gt; m_dwSearchTime` **且** `m_PEnvir &lt;&gt; nil`。
    /// **注意是严格大于 `>`**，非 `&gt;=`。
    /// 随后按"地图上是否有人或自己中毒"二选一。
    /// </summary>
    public static SearchStep SelectSearchStep(
        uint dwCurrentTick, uint searchTick, uint searchTime,
        bool penvirIsNull,
        int humCount, int humBBCount, int poisonDechealthTime)
    {
        // 4115：严格大于
        if (!((dwCurrentTick - searchTick) > searchTime))
            return SearchStep.Skip;

        // 4117：m_PEnvir <> nil
        if (penvirIsNull)
            return SearchStep.Skip;

        // 4119：有人或中毒
        if (humCount > 0 || humBBCount > 0 || poisonDechealthTime > 0)
            return SearchStep.RefreshTickAndSearch;

        // 4126-4134：无人 → 清理（**不刷新 tick**）
        return SearchStep.ClearAndReset;
    }

    /// <summary>
    /// 4138：第二道——节流判定。
    /// `not m_boIsVisibleActive and (m_nProcessRunCount &lt; nProcessMonsterInterval)`。
    /// 为真则本轮**跳过 `Run`**、只把计数加一。
    /// </summary>
    public static bool ShouldThrottle(bool boIsVisibleActive, int processRunCount, int processMonsterInterval)
        => !boIsVisibleActive && processRunCount < processMonsterInterval;

    /// <summary>
    /// 4144-4147：第三道的"是否值得运行"判定。
    /// 需 `m_PEnvir &lt;&gt; nil`，且以下任一成立：地图有人 / 距 `ClearHumOrBBTick`
    /// **不超过** 600000ms / 自己是 Ghost / 自己已死亡 / 中毒。
    /// **注意 4146 用的是 `&lt;=`（不超过），即恰好 600000 仍在窗口内。**
    /// </summary>
    public static bool ShouldRunMonster(
        bool penvirIsNull,
        int humCount, int humBBCount,
        uint now, uint clearHumOrBBTick,
        bool boGhost, bool boDeath, int poisonDechealthTime)
    {
        if (penvirIsNull)
            return false;      // 4144：m_PEnvir <> nil

        return humCount > 0
               || humBBCount > 0
               || (now - clearHumOrBBTick) <= ClearHumOrBBTickGraceMs
               || boGhost
               || boDeath
               || poisonDechealthTime > 0;
    }

    /// <summary>
    /// 4138-4163：综合决策。**按原文顺序短路**：
    /// 先判第二道节流（命中即 `IncrementAndSkip`，**不再看第三道**）；
    /// 否则若 `m_PEnvir = nil` → `DoNothing`（4163 的 `else if` 也要求非 nil）；
    /// 否则若值得运行 → `ResetAndRun`；否则 → `SilentClear`。
    /// </summary>
    public static MonsterStep SelectMonsterStep(
        bool boIsVisibleActive, int processRunCount, int processMonsterInterval,
        bool penvirIsNull,
        int humCount, int humBBCount,
        uint now, uint clearHumOrBBTick,
        bool boGhost, bool boDeath, int poisonDechealthTime)
    {
        // 第二道优先：命中则本轮只加计数，**与第三道无关**
        if (ShouldThrottle(boIsVisibleActive, processRunCount, processMonsterInterval))
            return MonsterStep.IncrementAndSkip;

        // 4154/4163：两道 else 都要求 m_PEnvir <> nil
        if (penvirIsNull)
            return MonsterStep.DoNothing;

        if (ShouldRunMonster(penvirIsNull, humCount, humBBCount,
                now, clearHumOrBBTick, boGhost, boDeath, poisonDechealthTime))
        {
            return MonsterStep.ResetAndRun;
        }

        return MonsterStep.SilentClear;
    }

    /// <summary>
    /// 4138-4142 / 4150：套用决策后的计数新值。
    /// **仅 `IncrementAndSkip` 加一、仅 `ResetAndRun` 清零**；
    /// `SilentClear` 与 `DoNothing` **保持原值**（这是 4154-4163 未清零的历史行为）。
    /// </summary>
    public static int ApplyProcessRunCount(MonsterStep step, int current)
        => step switch
        {
            MonsterStep.IncrementAndSkip => current + 1,
            MonsterStep.ResetAndRun => 0,
            _ => current,
        };

    /// <summary>
    /// 4128-4133 / 4156-4161：被清空的六项战斗引用（两处出现**完全相同**，顺序一致）。
    /// 本方法只返回字段名，供调用方按序置 `null`。
    /// </summary>
    public static readonly string[] ClearedCombatRefs =
    {
        "m_CurrTarget",
        "m_TargetCret",
        "m_LastHiter",
        "m_ExpHitter",
        "m_PoisonHitter",
        "m_CurrTargetEx",
    };

    /// <summary>
    /// 4112-4164：模拟一个怪物在多轮调度下的计数轨迹，
    /// 用于固化"无人地图上计数累积到上限后每轮都走第三道"的行为。
    /// 返回每轮结束时的 `m_nProcessRunCount`。
    /// </summary>
    public static int[] SimulateRunCount(
        int rounds, bool boIsVisibleActive, int processMonsterInterval,
        bool penvirIsNull, bool shouldRunConditions)
    {
        var result = new int[rounds];
        int count = 0;

        for (int i = 0; i < rounds; i++)
        {
            var step = SelectMonsterStep(
                boIsVisibleActive, count, processMonsterInterval,
                penvirIsNull,
                humCount: shouldRunConditions ? 1 : 0,
                humBBCount: 0,
                now: 0, clearHumOrBBTick: 0,
                boGhost: false, boDeath: false, poisonDechealthTime: 0);

            count = ApplyProcessRunCount(step, count);
            result[i] = count;
        }

        return result;
    }
}
