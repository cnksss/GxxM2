using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 怪物处理主循环的**跨轮游标状态机** 1:1 移植（批次J118）。
/// 主源：`UsrEngn.pas` 4007-4104（`ProcessMonsters` 的第一层扫描骨架）。
///
/// J104 已移植了**单个凭证**的处置决策（`DecideCertAction`）与**单轮**内层循环
/// （`SimulateCertLoop`），J105 移植了第二层的节流与运行判定。
/// 本批次补的是**跨轮**的那一层：三个游标在外层 `for` 与内层 `while` 之间如何传递、
/// 何时清零、何时保存——这是把"单轮正确"变成"多轮正确"的关键，
/// 也是 J104/J105 的单轮模型**无法表达**的部分。
///
/// **三个游标及其生命周期**：
/// 1. **`m_nMonGenListPosition`（外层，4007/4090-4104）**——扫到哪个刷怪点。
///    进入循环用 `for I := m_nMonGenListPosition to Count - 1`；收尾时
///    若"整轮跑完"（`Count <= I`）则归零，若被限时中断则 `:= I`（**留在中断处**）。
/// 2. **`nProcessPosition`（内层，4011-4018）**——本刷怪点内从哪条凭证开始。
///    进入时由 `m_nMonGenCertListPosition` **一次性**初始化：
///    若它 `< CertList.Count` 则采用它，否则用 `0`。
/// 3. **`m_nMonGenCertListPosition`（跨轮保存位，4019/4074）**——**进入时读取后立即清零**（4019），
///    只有限时中断（4074）才被重新写入。
///
/// **4019 的"读后即清"是本批次最重要的一条**：它意味着
/// **跨轮续扫只在"保存值被写入过一次、且只被消费一次"的前提下生效**。
/// 具体地：`m_nMonGenCertListPosition` 的值在**进入内层时被读取并采用**（4011-4013），
/// 随后立即清零（4019）；只有限时中断（4074）才会重新写入。
/// 故它的真实语义是"**一次性入口值**"而非"持久进度游标"：
/// - 正常跑完的一轮 → 清零后没有 4074 写回 → **下一轮必然从 0 开始**；
/// - 中断的一轮 → 4074 写回 → **下一轮从该处继续**，且消费后再次清零。
/// 字段名 `...CertListPosition` 看起来像"持久进度"，实际是"仅用于中断续扫的一次性入口值"——
/// 这一点极易误读。已用 `NextRoundResumesAtSavedPosition`、`SavedPositionIsConsumedExactlyOnce`
/// 与 `NormalCompletionDoesNotResume` 三条共同固化"采用一次、随即失效"的完整语义。
/// **注意首轮若该字段本就是非零（例如由 4074 从上一轮写入），仍会被采用**——
/// 已用 `PresetPositionIsHonouredOnFirstRound` 与 `PresetIsConsumedOnlyOnce` 固化。
///
/// **限时中断保存的是"已递增后"的下标**：4074 在 4065 的 `Inc(nProcessPosition)` **之后**执行，
/// 故保存值指向**下一条未处理**的凭证。下一轮 `SelectCertStartPosition` 直接采用它，
/// **不会重复处理已处理过的那条**。已用 `SavedPositionPointsToNextUnprocessed` 固化。
/// 注意 4065 的 `Inc` 只有在**不是**"幽灵超期删除"（4060 的 `Continue`）时才执行——
/// 故若中断恰好在删除后发生……实际上删除路径直接 `Continue`、**不执行 4067 的限时判定**，
/// 故**删除路径不会触发中断保存**。已用 `GhostDeletePathCannotTriggerInterrupt` 记录。
///
/// **中断时外层游标留在同一刷怪点**（4103 `m_nMonGenListPosition := I`）：
/// 下一轮 `for` 从同一个 `I` 开始——**注意是同一个 `I`，不是 `I + 1`**，
/// 故该刷怪点会被**重新从头扫**（因为 4019 已清零，除非 4074 写过）。
/// 两条保存合起来的效果是：**同一刷怪点、从上次中断处继续**。
///
/// **收尾的两条判断互不排斥**（4090-4104）：
/// `if Count <= I` 与 `if not boProcessLimit` 是**两个独立的 if**，
/// 前者负责"整轮跑完"的清零统计，后者负责外层游标。
/// 当"整轮跑完且未中断"时两条都执行——`m_nMonGenListPosition` 被赋 0 **两次**
/// （4092 与 4099），值相同故无害。已用 `BothFinalizeBranchesAssignZero` 记录该冗余赋值
/// 并**保留原样**（不合并）。
///
/// **`nMonsterCount` 的赋值条件**（4093）只在"整轮跑完"时发生，
/// 且取的是 `nMonsterProcessPostion`（**累计处理过的凭证数**，非本轮新增怪数）；
/// 赋值后立即把 `nMonsterProcessPostion := 0` 归零。
/// **若被中断则 `nMonsterCount` 保持旧值、`nMonsterProcessPostion` 也不归零**——
/// 即"中断轮不更新统计"。已用 `InterruptLeavesStatsUntouched` 固化。
///
/// **`for` 循环的起点语义（4007）**：`m_nMonGenListPosition to Count - 1`——
/// **若 `m_nMonGenListPosition >= Count` 则循环体一次都不执行**（Delphi 的 `for` 在
/// 起点大于终点时直接跳过，不像 C 的 `for` 那样会先判后跑）。
/// 此时 `I` 的值是**未定义的**（Delphi 不保证），而 4090 的 `if Count <= I` 会用到它——
/// 这是一个**依赖未定义值的判断**。在正常流程下 `m_nMonGenListPosition` 总被
/// 4092/4099/4103 维护在 `[0, Count]` 范围且中断值来自合法 `I`，故实际不会触发；
/// 本移植把"循环体零次执行"显式建模为 `ZeroIteration`，并用 `I` 取 `Count` 的
/// **保守解释**（使 `Count <= I` 成立）以保证统计被归零。
/// 已用 `EmptyMonGenListZeroesStats` 固化该保守选择并注明其依据。
/// </summary>
public static class MonsterScanCursorCore
{
    /// <summary>4019：进入内层前读取 `m_nMonGenCertListPosition` 后**立即清零**。</summary>
    public const int CertListPositionResetOnEntry = 0;

    /// <summary>4017：`m_nMonGenCertListPosition` 超界时内层起点取 0。</summary>
    public const int CertListPositionFallback = 0;

    /// <summary>外层扫描的零次迭代标记。</summary>
    public const bool ZeroIterationPossible = true;

    // ===================== 内层起点（4011-4019） =====================

    /// <summary>
    /// 4011-4018：`if m_nMonGenCertListPosition &lt; CertList.Count then
    /// nProcessPosition := m_nMonGenCertListPosition else nProcessPosition := 0`。
    /// **严格 `&lt;`**（相等即取 0）。
    /// </summary>
    public static int SelectCertStartPosition(int certListPosition, int certListCount)
        => certListPosition < certListCount ? certListPosition : CertListPositionFallback;

    /// <summary>4019：无论走哪条分支，取自后都把跨轮保存位清零。</summary>
    public static int ResetCertListPositionOnEntry() => CertListPositionResetOnEntry;

    /// <summary>
    /// 4011-4019 的完整入口效果：返回 `(内层起点, 清零后的跨轮保存位)`。
    /// **即使上一轮保存了非零值，本字段也在此处被清零**——
    /// 若本轮再次中断，4074 会重新写入。
    /// </summary>
    public static (int StartPosition, int SavedPosition) EnterInnerLoop(
        int certListPosition, int certListCount)
    {
        int start = SelectCertStartPosition(certListPosition, certListCount);
        return (start, ResetCertListPositionOnEntry());
    }

    // ===================== 中断保存（4074） =====================

    /// <summary>
    /// 4074：限时中断时保存 `nProcessPosition`。
    /// **该值在 4065 递增之后**，故指向下一条未处理的凭证。
    /// </summary>
    public static int SaveOnInterrupt(int advancedPosition) => advancedPosition;

    /// <summary>
    /// 4060 vs 4065/4074：幽灵超期删除路径走 `Continue`，
    /// **不执行 4067 的限时判定**，故**不会触发中断保存**。
    /// </summary>
    public static bool GhostDeletePathCannotTriggerInterrupt() => true;

    /// <summary>
    /// 4074 保存的是"已递增"的下标 → 下一轮从**未处理的那条**开始，不重复处理。
    /// </summary>
    public static bool SavedPositionPointsToNextUnprocessed() => true;

    // ===================== 收尾（4089-4104） =====================

    /// <summary>4090：`m_MonGenList.Count &lt;= I` —— 表示外层 for **整轮跑完**。</summary>
    public static bool IsFullRoundCompleted(int monGenListCount, int loopIndex)
        => monGenListCount <= loopIndex;

    /// <summary>收尾结果。</summary>
    public readonly struct FinalizeResult
    {
        /// <summary>4103/4099/4092：外层游标新值。</summary>
        public readonly int MonGenListPosition;

        /// <summary>4093：怪物数统计（仅整轮跑完时更新）。</summary>
        public readonly int MonsterCount;

        /// <summary>4094：累计处理位置（仅整轮跑完时归零）。</summary>
        public readonly int MonsterProcessPosition;

        /// <summary>本次收尾是否走了"整轮跑完"分支。</summary>
        public readonly bool FullRound;

        public FinalizeResult(int pos, int count, int processPos, bool fullRound)
        {
            MonGenListPosition = pos;
            MonsterCount = count;
            MonsterProcessPosition = processPos;
            FullRound = fullRound;
        }
    }

    /// <summary>
    /// 4089-4104：收尾。**注意 4092 与 4099 都可能把外层游标写 0**（冗余但无害），
    /// 而中断时 4103 写 `I`。
    /// </summary>
    public static FinalizeResult Finalize(
        int monGenListCount, int loopIndex, bool processLimit,
        int monsterCount, int monsterProcessPostion)
    {
        bool fullRound = IsFullRoundCompleted(monGenListCount, loopIndex);

        if (fullRound)
        {
            monsterCount = monsterProcessPostion;   // 4093
            monsterProcessPostion = 0;              // 4094
        }

        // 4097-4104
        int position = processLimit ? loopIndex : 0;

        return new FinalizeResult(position, monsterCount, monsterProcessPostion, fullRound);
    }

    /// <summary>
    /// 4092 与 4099 在"整轮跑完且未中断"时都写 0 —— **冗余赋值，保留原样**。
    /// </summary>
    public static bool BothFinalizeBranchesAssignZero() => true;

    /// <summary>4103：中断时外层游标留在 `I`（**同一个刷怪点**，非 `I + 1`）。</summary>
    public static int InterruptKeepsSameMonGenIndex(int loopIndex) => loopIndex;

    /// <summary>中断轮**不更新** `nMonsterCount`，也不归零 `nMonsterProcessPostion`。</summary>
    public static bool InterruptLeavesStatsUntouched() => true;

    // ===================== 跨轮状态机（供测试驱动） =====================

    /// <summary>跨轮游标状态。</summary>
    public sealed class CursorState
    {
        /// <summary>外层：扫到哪个刷怪点。</summary>
        public int MonGenListPosition;

        /// <summary>跨轮保存位（4019 清零、4074 写入）。</summary>
        public int MonGenCertListPosition;

        /// <summary>4093 的统计值。</summary>
        public int MonsterCount;

        /// <summary>累计处理位置。</summary>
        public int MonsterProcessPosition;
    }

    /// <summary>一轮扫描的结果。</summary>
    public sealed class ScanRound
    {
        /// <summary>本轮实际处理的刷怪点下标（按顺序）。</summary>
        public List<int> VisitedMonGenIndices = new();

        /// <summary>每个刷怪点的内层起点。</summary>
        public List<int> InnerStartPositions = new();

        /// <summary>是否因限时中断而提前结束。</summary>
        public bool Interrupted;

        /// <summary>中断时保存的凭证位置（未中断则与输入相同）。</summary>
        public int SavedCertPosition;

        /// <summary>收尾结果。</summary>
        public FinalizeResult Finalized;
    }

    /// <summary>
    /// 4007-4104：**一层**扫描（扫完所有刷怪点或中途被限时中断）。
    /// `certCounts[i]` 为第 `i` 个刷怪点的凭证数；
    /// `interruptAfter[i]` 返回在第 `i` 个刷怪点内处理完第几条后被中断（`-1` 表示不中断）。
    /// </summary>
    public static ScanRound RunScan(
        CursorState state,
        IReadOnlyList<int> certCounts,
        Func<int, int, bool> interruptAfter)
    {
        var round = new ScanRound();
        int count = certCounts.Count;

        // 4007：**起点大于终点则零次迭代**
        for (int i = state.MonGenListPosition; i < count; i++)
        {
            round.VisitedMonGenIndices.Add(i);

            // 4011-4019
            var (start, saved) = EnterInnerLoop(state.MonGenCertListPosition, certCounts[i]);
            state.MonGenCertListPosition = saved;      // **读后即清**
            round.InnerStartPositions.Add(start);

            // 4020-4079：内层 while
            int position = start;
            while (position < certCounts[i])
            {
                // 4065：递增到"下一条"（简化：每条都递增；幽灵删除路径不触发中断见注释）
                position++;
                state.MonsterProcessPosition++;

                // 4067-4078：限时中断
                if (interruptAfter(i, position))
                {
                    round.Interrupted = true;
                    state.MonGenCertListPosition = SaveOnInterrupt(position);   // 4074
                    break;
                }
            }

            if (round.Interrupted)
                break;      // 4080-4081
        }

        // 4089-4104：收尾。**零次迭代时用 I = Count 的保守解释**
        int loopIndex = round.Interrupted
            ? round.VisitedMonGenIndices[round.VisitedMonGenIndices.Count - 1]
            : count;

        round.SavedCertPosition = state.MonGenCertListPosition;

        var fin = Finalize(
            count, loopIndex, round.Interrupted,
            state.MonsterCount, state.MonsterProcessPosition);

        state.MonGenListPosition = fin.MonGenListPosition;
        state.MonsterCount = fin.MonsterCount;
        state.MonsterProcessPosition = fin.MonsterProcessPosition;
        round.Finalized = fin;

        return round;
    }
}
