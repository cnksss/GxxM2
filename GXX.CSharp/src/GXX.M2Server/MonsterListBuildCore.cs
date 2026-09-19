using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// `TUserEngine.RunMonster` **前半段：怪物候选表构建** 1:1 移植（批次J106），
/// 主源 `UsrEngn.pas` 3990-4110，另含两处依赖：
/// `tick_diff`(M2Share.pas 32953-32959) 与 `TBaseObject.MakeGhost`(ObjBase.pas 33697-33706)。
///
/// J105 移植了 4138-4163 的**节流消费段**（拿到 `FRunMonsterList` 之后怎么跑），
/// 本批次补上 `FRunMonsterList` **如何被填出来**（4001-4104）——两段合起来
/// 才是完整的 `RunMonster` 主循环。
///
/// **本段的核心是"不把全部怪物一次跑完"的两级游标**：
/// - `m_nMonGenListPosition`（UsrEngn.pas 124）：刷怪点列表的**进度游标**，
///   下次从该刷怪点继续（4088 的 `for I := m_nMonGenListPosition to ...`）；
/// - `m_nMonGenCertListPosition`（125）：**当前刷怪点内**凭证列表的进度游标（4011-4019）。
/// 每次外层循环开始时，若"上次中断点仍在本次刷怪点的范围内"则从该点续跑，否则归零（4011-4019）；
/// 且**读取后立即把 `m_nMonGenCertListPosition` 清零**（4019）——故该游标只在
/// "本轮被 `g_dwMonLimit` 限时中断"（4074）这一刻才被重新写入。
///
/// **限时中断机制（4067-4078，原文注释 `修复引擎报错 piaoyun 2013-12-26`）**：
/// 每处理完一个凭证后，若 `(MyGetTickCount - dwMonProcTick) > g_dwMonLimit`
/// （默认 **30**，M2Share.pas 3865 `g_dwMonLimit: LongWord = 30`）则：
/// 记录 `g_sMonGenInfo2` 诊断串、置 `boProcessLimit := True`、
/// **保存 `m_nMonGenCertListPosition := nProcessPosition`** 并 `Break` 出 while。
/// 回到外层后 `boProcessLimit` 为真则 `Break` 出 for（4080-4081），
/// 且**把 `m_nMonGenListPosition := I`**（4103）——即在**同一个刷怪点**上留下断点。
/// 若未中断，`m_nMonGenListPosition := 0`（4099），下一轮从列表头重新扫描。
///
/// **两处"计数"语义不同，易混**（4090-4094）：
/// - `nMonsterProcessPostion`（132 注释"处理怪物总数位置，用于计算怪物总数"）在每个凭证上 `Inc`（4047）；
/// - `nMonsterProcessCount`（3999 清零）只在**真正加入 `FRunMonsterList`** 时 `Inc`（4044）。
/// 当 `m_MonGenList.Count &lt;= I`（即本轮把刷怪点列表**整轮跑完**）时，
/// 才把 `nMonsterCount := nMonsterProcessPostion` 并**把 `nMonsterProcessPostion := 0`**（4093-4094）。
/// 原文 4095 有一行注释掉的折中写法 `// n84 := (n84 + nMonsterProcessCount) div 2;`，保留痕迹。
///
/// **智能刷怪（4029-4033，原文注释 `智能刷怪 chongchong 2014-01-07`）**：
/// 当 `(not m_boGhost) and MonGen.boNoManNoMon and m_PEnvir.m_boClearMon` 三者同时成立时
/// 调用 `MakeGhost()`——把怪物**转为幽灵态**而非立即删除，使其不再参与后续逻辑但仍留在列表中。
/// **幽灵化的怪物不会进入 `FRunMonsterList`**（4035 的 `if not m_boGhost`），
/// 且**走到 4050 的 else 分支**——在那里若已幽灵超过 **300000ms（5 分钟）**
/// 则从 `CertList` 中 `Delete` 并 `FreeAndNil`，随后 `Continue`
/// （**注意 `Continue` 跳过 4064 的 `Inc(nProcessPosition)`**，
/// 因为 `Delete` 已使后一个元素落到当前下标，这正是"删除后不递增"的标准写法）。
///
/// **加入候选表的门槛（4039）**：`tick_diff(m_dwRunTick, dwCurrentTick) > m_nRunTime`
/// ——即距上次运行超过该怪物自身的 `m_nRunTime`（ObjBase.pas 3200/11417 处为 350/250，
/// ObjAxeMon.pas 173 为 250）才加入；加入时把 `m_dwRunTick := dwRunTick`
/// （**注意 `dwRunTick` 是 3992 在本轮开始时取的快照，而非当前时刻**）。
///
/// **`tick_diff`（M2Share.pas 32953-32959）处理了 32 位回绕**：
/// `if tick_end >= tick_start then tick_end - tick_start
///  else High(Cardinal) - tick_start + tick_end`——
/// 注意回绕分支用的是 `High(Cardinal)`（4294967295）而**不是** `High(Cardinal) + 1`，
/// 故回绕时结果**比真实差值小 1**。这是原文的既有偏差（`tick_diff` 在本项目 12 个单元中
/// 各有独立实现），本移植按 1:1 保留，**不"修正"为 `uint.MaxValue + 1`**。
/// </summary>
public static class MonsterListBuildCore
{
    /// <summary>3865：`g_dwMonLimit: LongWord = 30`（毫秒）。</summary>
    public const uint DefaultMonLimit = 30;

    /// <summary>4052：幽灵怪物存活上限 300000ms（原文注释"5分钟"）。</summary>
    public const uint GhostLifetimeMs = 300_000;

    /// <summary>
    /// 32953-32959：`tick_diff` 1:1。
    /// **回绕分支用 `High(Cardinal)`（4294967295）而非 4294967296**，故比真实差值小 1——
    /// 按原文保留该偏差。
    /// </summary>
    public static uint TickDiff(uint tickStart, uint tickEnd)
        => tickEnd >= tickStart ? tickEnd - tickStart : uint.MaxValue - tickStart + tickEnd;

    /// <summary>当前刷怪点内，本轮从哪个下标开始（4011-4018）。</summary>
    public static int SelectCertStartPosition(int monGenCertListPosition, int certListCount)
        => monGenCertListPosition < certListCount ? monGenCertListPosition : 0;

    /// <summary>
    /// 4029-4033：是否应把怪物转为幽灵。
    /// 三者同时成立：怪物非幽灵、刷怪点开了"无人不刷"、地图开了清怪。
    /// </summary>
    public static bool ShouldMakeGhost(bool monsterGhost, bool monGenNoManNoMon, bool envirClearMon)
        => !monsterGhost && monGenNoManNoMon && envirClearMon;

    /// <summary>
    /// 4039：是否把该怪物加入本轮候选表（`tick_diff(m_dwRunTick, dwCurrentTick) > m_nRunTime`）。
    /// **严格大于**。
    /// </summary>
    public static bool ShouldAddToRunList(uint monsterRunTick, uint currentTick, int monsterRunTime)
        => TickDiff(monsterRunTick, currentTick) > (uint)monsterRunTime;

    /// <summary>
    /// 4052：幽灵态是否已超期可回收（`(MyGetTickCount - m_dwGhostTick) > 300000`，**严格大于**）。
    /// 注意此处用的是**直接相减**（`MyGetTickCount - m_dwGhostTick`），
    /// **而非 `tick_diff`**——原文两处写法不同，按 1:1 保留。
    /// </summary>
    public static bool IsGhostExpired(uint now, uint ghostTick)
        => (now - ghostTick) > GhostLifetimeMs;

    /// <summary>
    /// 4069：是否已达本轮限时（`(MyGetTickCount - dwMonProcTick) > g_dwMonLimit`，**严格大于**）。
    /// </summary>
    public static bool IsMonLimitReached(uint now, uint monProcTick, uint monLimit)
        => (now - monProcTick) > monLimit;

    /// <summary>
    /// 4072：限时中断时记录到 `g_sMonGenInfo2` 的诊断串
    /// `Monster.m_sCharName + '/' + IntToStr(I) + '/' + IntToStr(nProcessPosition)`。
    /// </summary>
    public static string BuildMonGenInfo(string charName, int monGenIndex, int processPosition)
        => $"{charName}/{monGenIndex}/{processPosition}";

    /// <summary>
    /// 4088-4104：一轮扫描结束后的游标收尾。
    /// 返回 `(monGenListPosition, monsterCount, monsterProcessPostion)` 三元组的新值。
    /// - 若 `m_MonGenList.Count &lt;= I`（整轮跑完）→ `monsterCount := nMonsterProcessPostion`、
    ///   `nMonsterProcessPostion := 0`；
    /// - 未中断 → `m_nMonGenListPosition := 0`；中断 → `:= I`（留在同一刷怪点）。
    /// </summary>
    public static (int MonGenListPosition, int MonsterCount, int MonsterProcessPostion)
        FinalizeScanCursor(
            int monGenListCount, int loopIndex, bool processLimit,
            int monsterCount, int monsterProcessPostion)
    {
        if (monGenListCount <= loopIndex)
        {
            monsterCount = monsterProcessPostion;
            monsterProcessPostion = 0;
        }

        int position = processLimit ? loopIndex : 0;

        return (position, monsterCount, monsterProcessPostion);
    }

    /// <summary>单条凭证在本轮内的处置结果。</summary>
    public enum CertAction
    {
        /// <summary>4026：凭证为 nil，跳过。</summary>
        NilMonster,

        /// <summary>4043：加入候选表并 `Inc(nMonsterProcessCount)`，随后 4047 递增下标。</summary>
        AddedToRunList,

        /// <summary>4035 为假（已是幽灵）但未超期：仅 4047 递增下标。</summary>
        GhostRetained,

        /// <summary>4055-4060：幽灵超期，从 CertList 删除并释放，**`Continue` 不递增下标**。</summary>
        GhostRemovedNoAdvance,
    }

    /// <summary>
    /// 4020-4079：处理**一条**凭证的决策（不含限时中断判定，后者由调用方在 4067 处判定）。
    /// 调用方需按返回值决定是否 `Inc(nProcessPosition)`：
    /// **仅 `GhostRemovedNoAdvance` 不递增**，其余三种都递增（4065）。
    /// </summary>
    public static CertAction DecideCertAction(
        bool monsterIsNull, bool monsterGhost,
        uint ghostTick, uint now,
        uint monsterRunTick, uint currentTick, int monsterRunTime)
    {
        if (monsterIsNull)
            return CertAction.NilMonster;

        if (!monsterGhost)
        {
            // 4035-4045
            if (ShouldAddToRunList(monsterRunTick, currentTick, monsterRunTime))
                return CertAction.AddedToRunList;

            return CertAction.GhostRetained;   // 未达运行间隔：不加入，仅递增下标
        }

        // 4050-4061：已是幽灵
        if (IsGhostExpired(now, ghostTick))
            return CertAction.GhostRemovedNoAdvance;

        return CertAction.GhostRetained;
    }

    /// <summary>
    /// 4065：该动作之后是否应 `Inc(nProcessPosition)`。
    /// **只有幽灵超期删除不递增**（因 `Delete` 已使后续元素前移）。
    /// </summary>
    public static bool ShouldAdvancePosition(CertAction action)
        => action != CertAction.GhostRemovedNoAdvance;

    /// <summary>
    /// 4020-4079 的完整内层循环模拟：在给定凭证序列上跑一遍，
    /// 返回加入候选表的怪物下标与最终 `nProcessPosition`。
    /// `certList` 中的 `null` 元素代表凭证为 nil（保留占位，模拟原文"不递增"之外的情形）。
    /// </summary>
    public static (List<int> AddedIndices, int EndPosition) SimulateCertLoop(
        IReadOnlyList<SimCert> certList,
        int startPosition,
        uint currentTick, uint runTick, uint now,
        uint monProcTick, uint monLimit, bool limitEnabled)
    {
        var added = new List<int>();
        var list = new List<SimCert>(certList);
        int position = startPosition;

        while (true)
        {
            // 4022-4023
            if (position >= list.Count)
                break;

            var m = list[position];

            var action = DecideCertAction(
                monsterIsNull: m.IsNull,
                monsterGhost: m.Ghost,
                ghostTick: m.GhostTick,
                now: now,
                monsterRunTick: m.RunTick,
                currentTick: currentTick,
                monsterRunTime: m.RunTime);

            if (action == CertAction.AddedToRunList)
                added.Add(position);

            // 4055-4060：删除 + Continue（不递增）
            if (action == CertAction.GhostRemovedNoAdvance)
            {
                list.RemoveAt(position);
                continue;
            }

            // 4065
            position++;

            // 4067-4078：限时中断（仅在 Monster <> nil 时判定，即非 NilMonster）
            if (limitEnabled && !m.IsNull && IsMonLimitReached(now, monProcTick, monLimit))
                break;
        }

        return (added, position);
    }

    /// <summary>模拟用的凭证条目。</summary>
    public readonly record struct SimCert(
        bool IsNull, bool Ghost, uint GhostTick, uint RunTick, int RunTime)
    {
        public static SimCert Normal(uint runTick, int runTime = 0)
            => new(false, false, 0, runTick, runTime);

        public static SimCert Ghosted(uint ghostTick)
            => new(false, true, ghostTick, 0, 0);

        public static SimCert Nil => new(true, false, 0, 0, 0);
    }
}
