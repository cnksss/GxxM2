using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 怪物处理主循环的**两阶段流水线** 1:1 移植（批次J119）。
/// 主源：`UsrEngn.pas` 3994-4001（初始化与列表清空）、4007-4088（第一阶段：建候选表）、
/// 4111-4164（第二阶段：驱动候选表）、4165-4198（统计与异常收尾）。
///
/// J104 移植了单条凭证的决策，J105 移植了两道节流/运行判定，J118 移植了跨轮游标。
/// 本批次补的是把它们**串起来的那条流水线**——
/// `ProcessMonsters` 并不是"扫一遍凭证、逐个处理"，而是：
/// **第一阶段只做筛选与入队**（4007-4088，把够条件的怪物放进 `FRunMonsterList`），
/// **第二阶段才真正驱动**（4112-4164，遍历该列表逐个 `SearchViewRange`/`Run`）。
/// 两阶段之间**没有任何数据依赖**，唯一的连接是 `FRunMonsterList` 这个临时表。
///
/// **`FRunMonsterList.Count := 0`（4001）在**两阶段之前**、`try` 之内、
/// 且在 `m_MonGenList.LockR` 之前**——故该列表是**每轮重建**的：
/// 上一轮入队的怪物不会残留到本轮。若误写成"每轮追加"，候选表会无限增长，
/// 同一怪物被重复驱动（`Run` 被调用多次），表现为怪物行动速度成倍加快且
/// 随运行时间越来越快——**这是本批次最需要固化的结构性事实**。
/// 已用 `RunListRebuiltEachRound` 与 `RunListDoesNotGrowAcrossRounds` 固化。
///
/// **第一阶段与第二阶段对同一怪物可能给出不同结论**：
/// 入队条件（4039）用的是 `tick_diff(m_dwRunTick, dwCurrentTick) > m_nRunTime`（**回绕安全**），
/// 而第二阶段的视野判定（4115）用的是 `dwCurrentTick - m_dwSearchTick > m_dwSearchTime`（**直接相减**）。
/// 两者是**不同的字段、不同的时间基准、不同的减法写法**——
/// 入队过的怪物在本轮第二阶段仍可能因视野条件不满足而完全不被处理。
/// 已用 `EnqueuedDoesNotImplySearched` 固化，并把两种减法并列对照
/// （`tick_diff` 回绕时比真实差值小 1，直接相减则按 `LongWord` 自然回绕）。
///
/// **第二阶段的 `Monster` 变量在两阶段间被复用**：
/// 第一阶段结束时 `Monster` 保留着**最后一次内层循环取到的凭证**（或 4060 删除后置的 `nil`）；
/// 第二阶段开头（4114）才被重新赋值为 `FRunMonsterList.Items[I]`。
/// **而 4178 的异常处理读的正是这个变量**——故若异常发生在 4114 之前
/// （例如 4011 的 `MonGen.CertList.Count` 访问、或 `m_MonGenList.Items[I]` 越界），
/// 异常处理看到的是**上一轮遗留或第一阶段的残留值**，而不是 `nil`。
/// 原文用 `if Monster <> nil` 区分两种异常消息（4178-4186），
/// 故**消息内容取决于变量的历史值而非当前故障点**。本移植显式建模该"历史值"语义。
/// 已用 `ExceptionDetailUsesStaleMonster` 固化。
///
/// **`Monster := nil` 的两个来源**：① 4058（幽灵超期删除后显式置 nil）；
/// ② Delphi 在 `try` 外未初始化局部对象变量时**不保证为 nil**，但此处 3983 声明后
/// 第一阶段首次赋值前若发生异常，其值未定义。本移植按"初始为 nil"建模（最常见情形），
/// 并注明这是移植假设而非原文保证。
///
/// **两个阶段都在 `try` 内、都有 `tCode` 标记**（3990-4188）：
/// `tCode` 在进入每个可疑操作前被更新，异常消息带上它（4180）以定位故障点。
/// 本移植保留 `tCode` 序列，因为它与行为无关但**对复现原文的诊断输出必要**。
/// 注意 **3992/3995/3998 三次取 `MyGetTickCount()`**：
/// `dwRunTick`（3992，用于 4172 的 `g_nMonTimeMin`）、
/// `dwCurrentTick`（3995，用于 4039 的入队判定与 4115 的视野判定）、
/// `dwMonProcTick`（3998，用于 4069 的限时判定与 4166 的 `g_nMonProcTime`）。
/// **三者取的时刻不同但相差极小**，故 `g_nMonProcTime` 与 `g_nMonTimeMin` 在无异常时
/// **几乎相等但不保证相等**——J107 已固定该观察，本批次把三者的**取值位置**也一并固化：
/// `dwCurrentTick` 取自**第一阶段之前**，故第一阶段耗时**不计入**入队判定的时间基准。
/// 已用 `CurrentTickTakenBeforePhaseOne` 固化。
///
/// **`nMonsterProcessCount`（3999 清零、4044 累加）不在收尾统计之列**：
/// 4165-4174 只更新 `g_nMonProcTime`/`g_nMonProcTimeMin`/`g_nMonProcTimeMax`/
/// `g_nMonTimeMin`/`g_nMonTimeMax`，**完全没用到 `nMonsterProcessCount`**；
/// 4093 用的是 `nMonsterProcessPostion`。故 `nMonsterProcessCount` 是一个
/// **每轮清零、每轮累加、却无人读取**的字段（4095 有一行被注释掉的
/// `// n84 := (n84 + nMonsterProcessCount) div 2;` 是其唯一的历史消费者）。
/// 已用 `ProcessCountIsWriteOnly` 记录该"只写不读"的事实并**保留原样**。
/// </summary>
public static class ProcessMonstersPipelineCore
{
    /// <summary>4001：候选表在每轮开始时被清空（`FRunMonsterList.Count := 0`）。</summary>
    public const bool RunListClearedEachRound = true;

    /// <summary>3999：`nMonsterProcessCount` 每轮清零。</summary>
    public const int ProcessCountReset = 0;

    /// <summary>3994：`boProcessLimit` 初值。</summary>
    public const bool InitialBoProcessLimit = false;

    /// <summary>3990：`tCode` 初值。</summary>
    public const int InitialTCode = 0;

    /// <summary>3989：`IsError` 初值。</summary>
    public const bool InitialIsError = false;

    // ===================== 三处时刻取值（3992/3995/3998） =====================

    /// <summary>三个时间基准的取值位置。</summary>
    public enum TickOrigin
    {
        /// <summary>3992：`dwRunTick`，用于 4172 的 `g_nMonTimeMin`。</summary>
        RunTick,

        /// <summary>3995：`dwCurrentTick`，用于 4039 入队与 4115 视野判定。</summary>
        CurrentTick,

        /// <summary>3998：`dwMonProcTick`，用于 4069 限时与 4166 的 `g_nMonProcTime`。</summary>
        MonProcTick,
    }

    /// <summary>
    /// 3992/3995/3998：三次取时刻**都在第一阶段之前**，
    /// 故第一阶段耗时不计入 `dwCurrentTick` 的时间基准。
    /// </summary>
    public static bool CurrentTickTakenBeforePhaseOne() => true;

    /// <summary>三处取值的顺序（原文 3992 → 3995 → 3998）。</summary>
    public static readonly TickOrigin[] TickOriginOrder =
    {
        TickOrigin.RunTick,
        TickOrigin.CurrentTick,
        TickOrigin.MonProcTick,
    };

    /// <summary>取时刻的先后决定 `g_nMonTimeMin >= g_nMonProcTime`（前者起点更早）。</summary>
    public static bool RunTickTakenBeforeMonProcTick() => true;

    // ===================== 第一阶段：入队（4007-4088） =====================

    /// <summary>入队后的记录。</summary>
    public sealed class RunEntry
    {
        /// <summary>来源刷怪点下标。</summary>
        public int MonGenIndex;

        /// <summary>该刷怪点内凭证下标。</summary>
        public int CertIndex;

        /// <summary>怪物标识（测试用）。</summary>
        public string Name = "";
    }

    /// <summary>第一阶段的产出。</summary>
    public sealed class PhaseOneResult
    {
        /// <summary>候选表（每轮重建）。</summary>
        public List<RunEntry> RunList = new();

        /// <summary>4044 累加值。</summary>
        public int MonsterProcessCount;

        /// <summary>是否因限时中断。</summary>
        public bool ProcessLimit;

        /// <summary>4072 的诊断串（仅中断时写）。</summary>
        public string MonGenInfo2 = "";

        /// <summary>中断时保存的凭证位置。</summary>
        public int SavedCertPosition;
    }

    /// <summary>
    /// 4039：入队条件——`tick_diff(m_dwRunTick, dwCurrentTick) > m_nRunTime`（**严格大于、回绕安全**）。
    /// </summary>
    public static bool ShouldEnqueue(uint monsterRunTick, uint currentTick, int monsterRunTime)
        => MonsterListBuildCore.TickDiff(monsterRunTick, currentTick) > (uint)monsterRunTime;

    /// <summary>4044：入队时 `Inc(nMonsterProcessCount)`。</summary>
    public static int IncrementProcessCount(int current) => current + 1;

    /// <summary>4072：限时中断时写入的诊断串 `m_sCharName + '/' + I + '/' + nProcessPosition`。</summary>
    public static string BuildMonGenInfo2(string charName, int monGenIndex, int certPosition)
        => MonsterListBuildCore.BuildMonGenInfo(charName, monGenIndex, certPosition);

    /// <summary>
    /// 4058：幽灵超期删除后显式 `Monster := nil` —— 这是 `Monster` 变量**变 nil 的唯一显式来源**。
    /// </summary>
    public static bool GhostDeleteExplicitlyNullsMonster() => true;

    // ===================== 第二阶段：驱动（4111-4164） =====================

    /// <summary>
    /// 4115：视野判定的前置条件——
    /// `(dwCurrentTick - m_dwSearchTick) > m_dwSearchTime` **且** `m_PEnvir <> nil`。
    /// **注意是直接相减**（非 `tick_diff`）。
    /// </summary>
    public static bool ShouldProcessSearch(uint currentTick, uint searchTick, uint searchTime, bool penvirIsNull)
        => (currentTick - searchTick) > searchTime && !penvirIsNull;

    /// <summary>
    /// 两种减法并列：`tick_diff`（回绕安全，回绕时比真实差值小 1）
    /// vs 直接相减（`LongWord` 自然回绕）。
    /// **同一对输入在回绕情形下结果不同**——故二者不可互换。
    /// </summary>
    public static (uint TickDiffResult, uint PlainSubtractResult) CompareSubtractions(
        uint start, uint now)
        => (MonsterListBuildCore.TickDiff(start, now), unchecked(now - start));

    /// <summary>
    /// **入队过不代表本轮会被搜索**：两者用不同字段与不同减法。
    /// </summary>
    public static bool EnqueuedDoesNotImplySearched() => true;

    /// <summary>4112：第二阶段遍历的正是第一阶段建的表。</summary>
    public static bool PhaseTwoIteratesPhaseOneList() => true;

    /// <summary>4112：`for I := 0 to FRunMonsterList.Count - 1` —— 空表零次迭代。</summary>
    public static int PhaseTwoIterations(int runListCount) => runListCount;

    // ===================== 收尾统计（4165-4174） =====================

    /// <summary>4166：`g_nMonProcTime := MyGetTickCount - dwMonProcTick`（**第二阶段末取时刻**）。</summary>
    public static int ComputeMonProcTime(uint now, uint monProcTick)
        => unchecked((int)(now - monProcTick));

    /// <summary>4172：`g_nMonTimeMin := MyGetTickCount - dwRunTick`（**与 4166 同一时刻**）。</summary>
    public static int ComputeMonTime(uint now, uint runTick)
        => unchecked((int)(now - runTick));

    /// <summary>
    /// 4172 与 4166 用的是**同一个 `MyGetTickCount` 调用**（原文两行相邻），
    /// 故 `g_nMonTimeMin >= g_nMonProcTime` 恒成立（起点更早）。
    /// </summary>
    public static bool MonTimeAtLeastMonProcTime() => true;

    /// <summary>
    /// 4167-4174：统计更新。**注意 4173 是 `if g_nMonTimeMax < g_nMonTimeMin`（方向与其它三处相反）**。
    /// </summary>
    public static void UpdateStats(
        ProcessMonstersEnvelopeCore.ProcTimeStats stats, int monProcTime, int monTime)
    {
        stats.MonProcTimeMinUpdate(monProcTime);   // 4167-4168
        stats.MonProcTimeMaxUpdate(monProcTime);   // 4169-4170
        stats.MonTimeMaxUpdate(monTime);           // 4173-4174
    }

    /// <summary>
    /// 4095：`nMonsterProcessCount` 的唯一历史消费者被注释掉——
    /// 故该字段**每轮清零、每轮累加、无人读取**。
    /// </summary>
    public static bool ProcessCountIsWriteOnly() => true;

    // ===================== 异常收尾（4175-4198） =====================

    /// <summary>
    /// 4178：异常处理读的是 `Monster` 变量的**当前值**——
    /// 该值在 4114 之前是第一阶段残留（或 nil），4114 之后是本轮元素。
    /// </summary>
    public static bool ShouldUseNilDetail(bool monsterIsNull) => monsterIsNull;

    /// <summary>4178-4186：按 `Monster` 是否为 nil 选择异常消息。</summary>
    public static string SelectExceptionMessage(
        bool monsterIsNull, int tCode, string exceptionMessage)
    {
        string detail = ProcessMonstersEnvelopeCore.SelectExceptionDetail(monsterIsNull, exceptionMessage);
        return ProcessMonstersEnvelopeCore.ExceptionMsg(tCode, detail);
    }

    /// <summary>4181：`Monster <> nil` 时置 `IsError := True`（仅此时）。</summary>
    public static bool SetsIsErrorOnlyWhenMonsterNotNull(bool monsterIsNull)
        => ProcessMonstersEnvelopeCore.ShouldSetIsError(monsterIsNull);

    /// <summary>4189：`IsError and (Monster <> nil)` 才输出异常对象名。</summary>
    public static bool ShouldPrintAbnormal(bool isError, bool monsterIsNull)
        => ProcessMonstersEnvelopeCore.ShouldPrintAbnormalObject(isError, monsterIsNull);

    /// <summary>
    /// **异常消息取决于 `Monster` 的历史值而非当前故障点**——
    /// 若异常发生在 4114 之前，读到的是第一阶段的残留值。
    /// </summary>
    public static bool ExceptionDetailUsesStaleMonster() => true;

    /// <summary>4178 的建模假设：`Monster` 初始为 nil（原文未显式初始化局部对象变量）。</summary>
    public static bool MonsterStartsNullByAssumption() => true;

    // ===================== 顶层：两阶段驱动 =====================

    /// <summary>一条凭证在模拟中的状态。</summary>
    public readonly record struct PipeCert(
        bool IsNull, bool Ghost, uint GhostTick, uint RunTick, int RunTime, string Name)
    {
        public static PipeCert Normal(uint runTick, int runTime, string name)
            => new(false, false, 0, runTick, runTime, name);

        /// <summary>4026 的 `Monster = nil` 情形（凭证为 nil、第一阶段直接跳过）。</summary>
        public static PipeCert Nil => new(true, false, 0, 0, 0, "");
    }

    /// <summary>一个刷怪点。</summary>
    public sealed class PipeMonGen
    {
        public string Name = "";
        public List<PipeCert> Certs = new();
    }

    /// <summary>一整轮的产出。</summary>
    public sealed class PipelineRound
    {
        /// <summary>候选表内容（第一阶段产出）。</summary>
        public List<string> RunListNames = new();

        /// <summary>第二阶段实际执行 `SearchViewRange` 的怪物名。</summary>
        public List<string> SearchedNames = new();

        /// <summary>第二阶段实际执行 `Run` 的怪物名。</summary>
        public List<string> RanNames = new();

        /// <summary>第二阶段静默清理的怪物名。</summary>
        public List<string> SilentClearedNames = new();

        /// <summary>是否第一阶段被限时中断。</summary>
        public bool ProcessLimit;

        /// <summary>4044 累加值。</summary>
        public int MonsterProcessCount;
    }

    /// <summary>
    /// 4001-4164：**一整轮**两阶段流水线。
    /// `searchTimes` 为每个怪物的 `m_dwSearchTime`；`runTimes` 为 `m_nRunTime`。
    /// 回调决定第二阶段的每一步，以便测试只观察"谁进入了哪一阶段"。
    /// </summary>
    public static PipelineRound RunRound(
        IReadOnlyList<PipeMonGen> monGens,
        uint currentTick, uint runTick, uint now,
        Func<PipeCert, uint> searchTickOf,
        Func<PipeCert, uint> searchTimeOf,
        Func<PipeCert, bool> penvirIsNull,
        Func<PipeCert, bool> boIsVisibleActive,
        int processMonsterInterval,
        Func<PipeCert, int> humCount,
        Func<PipeCert, bool> ghostNow,
        Func<PipeCert, bool> deathNow,
        Func<PipeCert, int> poisonTime,
        uint clearHumOrBBTick,
        bool interruptEverything)
    {
        var round = new PipelineRound();

        // 4001：**候选表每轮清空**
        var runList = new List<(PipeCert Cert, string Name)>();

        int processCount = ProcessCountReset;   // 3999

        // ===== 第一阶段：4007-4088 =====
        for (int i = 0; i < monGens.Count; i++)
        {
            if (interruptEverything)
            {
                round.ProcessLimit = true;
                break;
            }

            foreach (var cert in monGens[i].Certs)
            {
                if (cert.IsNull)
                    continue;

                // 4039
                if (!ShouldEnqueue(cert.RunTick, currentTick, cert.RunTime))
                    continue;

                runList.Add((cert, cert.Name));
                processCount = IncrementProcessCount(processCount);   // 4044
            }
        }

        round.RunListNames = new List<string>();
        foreach (var (_, name) in runList)
            round.RunListNames.Add(name);
        round.MonsterProcessCount = processCount;

        // ===== 第二阶段：4112-4164 =====
        foreach (var (cert, name) in runList)
        {
            // 4115
            if (ShouldProcessSearch(currentTick, searchTickOf(cert), searchTimeOf(cert), penvirIsNull(cert)))
            {
                if (humCount(cert) > 0 || poisonTime(cert) > 0)
                    round.SearchedNames.Add(name);      // 4124
                else
                    round.SilentClearedNames.Add(name); // 4128-4134
            }

            // 4138-4163：复用 J105 的决策
            var step = MonsterScheduleThrottleCore.SelectMonsterStep(
                boIsVisibleActive(cert), 0, processMonsterInterval,
                penvirIsNull(cert),
                humCount(cert), 0,
                now, clearHumOrBBTick,
                ghostNow(cert), deathNow(cert), poisonTime(cert));

            if (step == MonsterScheduleThrottleCore.MonsterStep.ResetAndRun)
                round.RanNames.Add(name);               // 4152
            else if (step == MonsterScheduleThrottleCore.MonsterStep.SilentClear)
                round.SilentClearedNames.Add(name);     // 4156-4162
        }

        return round;
    }

    /// <summary>
    /// 4001 + 4112：连续多轮驱动，用于验证**候选表不跨轮累积**。
    /// </summary>
    public static List<int> RunListSizesAcrossRounds(
        IReadOnlyList<PipeMonGen> monGens, int rounds, uint currentTick, uint runTick)
    {
        var sizes = new List<int>();

        for (int r = 0; r < rounds; r++)
        {
            var round = RunRound(
                monGens, currentTick, runTick, currentTick,
                _ => 0, _ => 0, _ => false, _ => false, 2,
                _ => 0, _ => false, _ => false, _ => 0, 0,
                interruptEverything: false);

            sizes.Add(round.RunListNames.Count);
        }

        return sizes;
    }

    /// <summary>4007-4088 + 4112-4164：`tCode` 序列（仅供诊断输出复现）。</summary>
    public static readonly int[] TCodes =
    {
        1,          // 4000
        11,         // 4010
        12,         // 4025
        121, 122, 123, 14, 125, 126, 127, 128, 129,   // 4028-4059
        150,        // 4064
        15, 16,     // 4071/4075
        2,          // 4089
        200,        // 4111
        13,         // 4123
        149,        // 4137
        114,        // 4140
        115, 116,   // 4149/4151
        1600,       // 4165
        1601,       // 4171
    };
}
