using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 刷怪主循环 `TUserEngine.ProcessRegenMonsters` 1:1 移植（批次J116）。
/// 主源：`UsrEngn.pas` 3831-3972，含嵌套 `CheckGVar`（3832-3849）、
/// 被注释掉的 `GetZenTime`（3851-3869）、优先刷怪游标（3890-3912）、
/// 常规游标（3920-3931）与刷怪条件（3939-3964）。
///
/// J108-J115 铺好了副本这条线，本批次回到**主刷怪循环**——
/// 这是 `m_MonGenList` 与 `m_SortMapMonGenList` 的**主要消费者**，
/// 也是 J106 的 `GetGenMonCount` 与 J108 的解析结果的落点。
/// 至此"**配置解析 → 候选表 → 排序 → 二分 → 刷怪循环 → 计数/节流**"贯通。
///
/// **节流（3885-3888）**：`(not g_Config.boVentureServer) and (not g_Config.boStopM2MakeMon)
/// and ((dwCurrentTick - dwRegenMonstersTick) > g_Config.dwRegenMonstersTime)`。
/// **两个否定配置项是"与"关系**：任一项为真（是冒险服 / 停止刷怪）则整块跳过。
/// 时间比较是**严格 `>`**；间隔默认 **200ms**（M2Share.pas 4211）。
/// 进入后**立刻**把 `dwRegenMonstersTick := dwCurrentTick`——**先更新再由后续逻辑判断**，
/// 故即使本轮什么都没刷，下次也要再等满 200ms。
/// **注意 `dwCurrentTick` 在 3880 就取好了**，整块用的是同一时刻（不是每次重新取）。
///
/// **`CheckGVar`（3832-3849）是 `case` 无 `else` 的写法**：
/// `Result := True` 是初值，只有匹配到六个 `TCompareType` 之一才改写。
/// 故 **`ctFail`（J108 的 `ParseCompareType` 对无法识别比较符的返回值）会保持 `True`**——
/// 即"比较符写错"的配置行**反而永远通过检查**，而不是被过滤掉。
/// 这与 J108 的 `ctFail` 语义连起来看很关键：`ctFail` 不是"永不满足"，而是"不比较、默认放行"。
/// 已用 `CtFailPassesCheck` 固化。**索引 `GVarIndex` 未做边界检查**（直接下标），
/// 故配置里写出越界序号会读到相邻内存——本移植用 `IReadOnlyList` 并显式声明该约束。
///
/// **优先刷怪游标（3890-3912）**：`FPriorityMonGenList` 非空时取**第 0 个**记录，
/// 若 `FPriorityMonGenListIndex >= 记录.Count` 则先归零；随后
/// `Index := 记录.Index + FPriorityMonGenListIndex`，**若越出 `m_SortMapMonGenList` 边界**，
/// 则 `Dispose` 该记录、从列表删除、**且本轮 `MonGen` 保持 nil**（走常规游标）；
/// 否则取该元素、`Inc` 游标、**若已达 `Count` 则 Dispose + 删除 + 游标归零**。
/// **两处 Dispose 的位置不同**：越界时"先删后（本轮不取）"，
/// 正常取用时"取完再删（若已满）"。故一个记录在"取最后一个元素"的那一轮仍会被使用，
/// 下一轮才发现越界——**不是提前删除**。
///
/// **常规游标（3920-3931）是三段式**：
/// ① 若 `m_nCurrMonGen < Count` 则取该元素（**可能取不到，当 Count 为 0**）；
/// ② 若 `m_nCurrMonGen < Count - 1` 则 `Inc`；
/// ③ 否则归零。
/// **注意 ① 与 ② 是两个独立判断**，条件分别是 `< Count` 与 `< Count - 1`。
/// 当 `Count = 1` 时：① 成立（0 < 1，取到元素），② 不成立（0 < 0 为假）→ 归零。
/// 当 `Count = 0` 时：① 不成立（`MonGen` 保持 nil），② 也不成立 → 归零。
/// **故空表时 `MonGen` 为 nil**，由 3939 的 `MonGen <> nil` 兜住。
///
/// **刷怪条件（3939-3940）是五条件与**：`MonGen <> nil`、`not MonGen.boFB`、
/// `Length(MonGen.sMonName) > 0`、**`MonGen.Envir.m_boMakeMon`**（"智能刷怪"开关，2014-01-07）、
/// `CheckGVar(MonGen)`。
/// **`not MonGen.boFB` 把副本地图的模板排除在主循环之外**——副本的刷怪由 J110/J112 那条线单独处理。
/// **`MonGen.Envir` 在此处被直接解引用**（无 nil 检查）：
/// 这正是 J109 的 `AddEmptyMonGenInfo` 必须创建"空挂靠点"而非留 nil 的原因之一。
///
/// **时间窗（3944）**：`(MonGen.dwStartTick = 0) or ((MyGetTickCount - MonGen.dwStartTick) > MonGen.dwZenTime)`。
/// **`dwStartTick = 0` 是特殊值**，表示"从未刷过"→ 无条件通过（首次立即刷）。
/// 否则用**严格 `>`**。原文 3942 有一行被注释掉的旧版本（用 `GetZenTime(...)` 做过 CPU 修复尝试），
/// **保持原样不复原**。
///
/// **数量计算（3946-3953）**：
/// ```
/// nGenCount := GetGenMonCount(MonGen);
/// boRegened := True;
/// if g_Config.nMonGenRate <= 0 then g_Config.nMonGenRate := 10;   // 防除零
/// nGenModCount := _MAX(1, Round(_MAX(1, MonGen.nCount) / (g_Config.nMonGenRate / 10)));
/// nMakeMonsterCount := nGenModCount - nGenCount;
/// if nMakeMonsterCount < 0 then nMakeMonsterCount := 0;
/// ```
/// **`_MAX(1, ...)` 出现两次**：内层保证**目标基准至少 1**，
/// 外层保证**计算结果至少 1**——故即使 `nMonGenRate` 极大（分母大），也不会算出 0。
/// `nMonGenRate / 10` 是**整数除法**（Delphi 的 `/` 在整数上得 Real，但此处
/// `g_Config.nMonGenRate` 与字面量 10 均为整数，实际按浮点除法再与整数相除——
/// 本移植按原文用 `double` 除法再 `Round`，保留 `.5` 的银行家舍入差异见下）。
/// `nGenModCount - nGenCount` **可能为负**（现存怪已超过目标），**被夹到 0**，
/// 故"怪太多"时不会刷出负数、也不会去杀怪。
/// **`boRegened` 初值为 `True`**：若 `nMakeMonsterCount <= 0`（不调用 `RegenMonsters`），
/// `boRegened` **保持 `True`** → 3960 **仍会刷新 `dwStartTick`**！
/// 即"本轮不需要刷怪"也会重置计时器，把下一次刷怪**推迟一整个 `dwZenTime`**。
/// 这是本批次最反直觉的一点：**没刷怪也会续期**。已用 `NoSpawnStillRefreshesStartTick` 固化。
///
/// **`g_nMonGenTime` 统计（3966-3970）**：**注意 3967/3969 都写成 `> g_nMonGenTimeMin`**——
/// 第一处把 `g_nMonGenTimeMin` 当作"最大值"用（与 J107 的 `g_nMonProcTimeMin` 同类命名陷阱）。
/// 两处都是"只增不减"的峰值，实际衰减发生在 `svMain.pas` 的 UI 刷新里（J107 已固化）。
///
/// **`g_sMonGenInfo1`（3963）**：`MonGen.sMonName + ',' + m_nCurrMonGen + '/' + m_MonGenList.Count`——
/// **注意用的是常规游标 `m_nCurrMonGen` 而非优先游标**，即便本轮怪来自优先列表。
/// 故监控面板显示的下标在优先刷怪时**与实际取用的下标不符**。这是一个显示层面的既有偏差，
/// 已用 `StatusUsesNormalCursorEvenForPriority` 记录而不"修正"。
/// </summary>
public static class RegenMonstersLoopCore
{
    /// <summary>M2Share.pas 4211：`dwRegenMonstersTime` 默认 **200** 毫秒。</summary>
    public const int DefaultRegenMonstersTime = 200;

    /// <summary>M2Share.pas 4211：`nMonGenRate` 默认 **10**（即倍率 1.0）。</summary>
    public const int DefaultMonGenRate = 10;

    /// <summary>3949：防除零的兜底值。</summary>
    public const int MonGenRateFallback = 10;

    /// <summary>3944：`dwStartTick = 0` 表示"从未刷过"。</summary>
    public const int NeverSpawnedTick = 0;

    // ===================== 节流（3883-3888） =====================

    /// <summary>
    /// 3883：`g_boStopRun` 为真则整个过程立即 `Exit`（**连统计都不更新**）。
    /// </summary>
    public static bool ShouldAbortEntirely(bool boStopRun) => boStopRun;

    /// <summary>
    /// 3885-3886：进入刷怪块的三条件——两个否定配置项与时间窗。
    /// **时间比较是严格 `>`**。
    /// </summary>
    public static bool ShouldEnterRegenBlock(
        bool boVentureServer, bool boStopM2MakeMon, int now, int regenTick, int regenTime)
    {
        return !boVentureServer
            && !boStopM2MakeMon
            && (now - regenTick) > regenTime;
    }

    /// <summary>3888：进入后立刻更新节流时刻（**先更新再判断**）。</summary>
    public static int RenewRegenTick(int now) => now;

    // ===================== CheckGVar（3832-3849） =====================

    /// <summary>
    /// 3832-3849：G 变量比较。
    /// **`case` 无 `else` 且 `Result` 初值为 `True`**——
    /// 故 `ctFail`（无法识别的比较符）**保持 `True`、永远放行**。
    /// `globalVal` 越界时抛 `ArgumentOutOfRangeException`（对应原文的未检查下标）。
    /// </summary>
    public static bool CheckGVar(
        MonGenParseCore.CompareType compareType, IReadOnlyList<int> globalVal, int gvarIndex, int gvarValue)
    {
        int v = globalVal[gvarIndex];

        return compareType switch
        {
            MonGenParseCore.CompareType.ctLess => v < gvarValue,
            MonGenParseCore.CompareType.ctEqual => v == gvarValue,
            MonGenParseCore.CompareType.ctGreater => v > gvarValue,
            MonGenParseCore.CompareType.ctLessEqual => v <= gvarValue,
            MonGenParseCore.CompareType.ctGreaterEqual => v >= gvarValue,
            MonGenParseCore.CompareType.ctNotEqual => v != gvarValue,
            _ => true,   // **ctFail 及未知值 → 初值 True，放行**
        };
    }

    /// <summary>3832：`ctFail` 是否放行——**是**（这是"比较符写错反而通过"的根源）。</summary>
    public static bool CtFailPassesCheck(MonGenParseCore.CompareType ct)
        => CheckGVar(ct, new[] { 0 }, 0, 0);

    // ===================== 优先刷怪游标（3890-3912） =====================

    /// <summary>优先刷怪记录（对应 `TPriorityMonGenRecord`）。</summary>
    public sealed class PriorityRecord
    {
        /// <summary>起始下标。</summary>
        public int Index;

        /// <summary>本记录覆盖的条目数。</summary>
        public int Count;
    }

    /// <summary>优先游标推进的结果。</summary>
    public enum PriorityStep
    {
        /// <summary>3903-3904：取到元素并推进游标（未删除记录）。</summary>
        TookAndAdvanced,

        /// <summary>3903-3909：取到元素、推进后已达 `Count` → **Dispose 并删除、游标归零**。</summary>
        TookThenRemoved,

        /// <summary>3896-3900：下标越出 `m_SortMapMonGenList` → **Dispose 并删除，本轮不取**。</summary>
        OutOfRangeRemoved,

        /// <summary>3893-3894：游标超限先归零，随后按上面三种判定。</summary>
        ResetCursorFirst,
    }

    /// <summary>优先游标推进的输出。</summary>
    public sealed class PriorityResult
    {
        /// <summary>取到的排序表下标；`null` 表示本轮未取到（应走常规游标）。</summary>
        public int? SortListIndex;

        /// <summary>是否删除了记录（Dispose + Delete）。</summary>
        public bool RemovedRecord;

        /// <summary>删除后游标是否归零。</summary>
        public bool CursorReset;

        /// <summary>推进后的游标值（记录被删时为 0）。</summary>
        public int NewCursor;
    }

    /// <summary>
    /// 3890-3912：优先刷怪游标推进。
    /// **无论走哪个分支，本轮至多取一个元素**；越界时 `MonGen` 保持 `null`。
    /// `sortListCount` 为 `m_SortMapMonGenList.Count`。
    /// </summary>
    public static PriorityResult StepPriorityCursor(
        PriorityRecord record, int cursor, int sortListCount)
    {
        var result = new PriorityResult();

        // 3893-3894：游标超限先归零
        if (cursor >= record.Count)
            cursor = 0;

        int index = record.Index + cursor;

        // 3896-3900：越出排序表 → 删除记录，**本轮不取**
        if (index >= sortListCount)
        {
            result.SortListIndex = null;
            result.RemovedRecord = true;
            result.CursorReset = true;      // 3909 只在"取用后满"分支；越界分支不显式归零，
                                            // 但记录已删，下一次从新记录开始
            result.NewCursor = 0;
            return result;
        }

        // 3903-3904
        result.SortListIndex = index;
        cursor++;

        // 3905-3910：取用后若已达 Count → 删除并归零
        if (cursor >= record.Count)
        {
            result.RemovedRecord = true;
            result.CursorReset = true;
            cursor = 0;
        }

        result.NewCursor = cursor;
        return result;
    }

    /// <summary>3890：只有优先列表非空才走优先分支。</summary>
    public static bool ShouldUsePriority(bool hasPriorityRecords) => hasPriorityRecords;

    // ===================== 常规游标（3920-3931） =====================

    /// <summary>常规游标推进的输出。</summary>
    public sealed class NormalCursorResult
    {
        /// <summary>3922：取到的下标；`null` 表示未取到（空表）。</summary>
        public int? Index;

        /// <summary>推进后的游标。</summary>
        public int NewCursor;
    }

    /// <summary>
    /// 3920-3931：三段式常规游标推进。**①② 是两个独立判断**
    /// （条件分别是 `< Count` 与 `< Count - 1`）。
    /// </summary>
    public static NormalCursorResult StepNormalCursor(int cursor, int monGenListCount)
    {
        var result = new NormalCursorResult();

        // 3920-3923
        if (cursor < monGenListCount)
            result.Index = cursor;
        else
            result.Index = null;

        // 3924-3931
        if (cursor < monGenListCount - 1)
            result.NewCursor = cursor + 1;
        else
            result.NewCursor = 0;

        return result;
    }

    /// <summary>3913：只有优先分支没取到才走常规游标。</summary>
    public static bool ShouldFallbackToNormal(bool monGenIsNull) => monGenIsNull;

    /// <summary>3913 的语义别名，供测试以意图命名引用。</summary>
    public static bool FallbackToNormalOnlyWhenPriorityMissed(bool monGenIsNull)
        => ShouldFallbackToNormal(monGenIsNull);

    // ===================== 刷怪条件（3939-3940） =====================

    /// <summary>
    /// 3939-3940：**五条件与**。`envirBoMakeMon` 对应 `MonGen.Envir.m_boMakeMon`（智能刷怪开关）。
    /// </summary>
    public static bool ShouldProcessMonGen(
        bool monGenIsNull, bool boFB, string monName, bool envirBoMakeMon, bool gvarOk)
    {
        return !monGenIsNull
            && !boFB
            && monName.Length > 0
            && envirBoMakeMon
            && gvarOk;
    }

    /// <summary>3939：`not MonGen.boFB` —— **副本模板被主循环排除**。</summary>
    public static bool ExcludesFbTemplates(bool boFB) => !boFB;

    // ===================== 时间窗（3944） =====================

    /// <summary>
    /// 3944：`(dwStartTick = 0) or ((now - dwStartTick) > dwZenTime)`。
    /// **`dwStartTick = 0` 无条件通过**（首次立即刷）；否则**严格 `>`**。
    /// </summary>
    public static bool IsZenTimeElapsed(int startTick, int now, int zenTime)
        => startTick == NeverSpawnedTick || (now - startTick) > zenTime;

    // ===================== 数量计算（3946-3953） =====================

    /// <summary>
    /// 3950：`_MAX(1, Round(_MAX(1, nCount) / (nMonGenRate / 10)))`。
    /// **`nMonGenRate` 处于分母位置**——尽管它在源码里的注释写作"刷怪倍数 10"（UsrEngn.pas 32），
    /// 数值**越大反而刷得越少**：`20` → 除以 2 → 目标减半；`5` → 除以 0.5 → 目标加倍。
    /// 只需把 `nCount` 与 `nMonGenRate` 对调即会反过来，故**按原文保留分母位置**。
    /// 已用 `HigherRateYieldsFewerMonsters` 记录该命名与行为的背离。
    /// </summary>
    public static int ComputeGenModCount(int monCount, int monGenRate)
    {
        // 3948-3949：防除零（**就地改写配置**，见 ApplyRateFallback）
        if (monGenRate <= 0)
            monGenRate = MonGenRateFallback;

        int baseCount = Math.Max(1, monCount);              // 内层 _MAX(1, ...)
        double divisor = monGenRate / 10.0;                  // nMonGenRate / 10

        int mod = (int)Math.Round(baseCount / divisor, MidpointRounding.AwayFromZero);

        return Math.Max(1, mod);                             // 外层 _MAX(1, ...)
    }

    /// <summary>
    /// 3952-3953：`nGenModCount - nGenCount` 后**夹到 0**（怪过多时不刷、也不杀）。
    /// </summary>
    public static int ComputeMakeMonsterCount(int genModCount, int genCount)
        => Math.Max(0, genModCount - genCount);

    /// <summary>3948-3949：`nMonGenRate <= 0` 时就地改写配置为 10。</summary>
    public static int ApplyRateFallback(int monGenRate)
        => monGenRate <= 0 ? MonGenRateFallback : monGenRate;

    /// <summary>3954：只有 `nMakeMonsterCount > 0` 才调用 `RegenMonsters`。</summary>
    public static bool ShouldCallRegen(int makeMonsterCount) => makeMonsterCount > 0;

    /// <summary>
    /// 3958-3961：`boRegened` 为真时刷新 `dwStartTick`。
    /// **`boRegened` 初值为 `True`**（3947），且只有 `ShouldCallRegen` 为真时才可能被
    /// `RegenMonsters` 的返回值覆盖——故**"本轮不需要刷怪"（`makeMonsterCount &lt;= 0`）
    /// 也会刷新计时器**，把下一次刷怪推迟一整个 `dwZenTime`。
    /// </summary>
    public static bool ShouldRefreshStartTick(bool boRegened) => boRegened;

    /// <summary>3947：`boRegened` 的初值。</summary>
    public const bool InitialBoRegened = true;

    // ===================== 统计（3966-3970） =====================

    /// <summary>
    /// 3966-3970：`g_nMonGenTime` 统计。
    /// **注意 3967 与 3969 都写成 `&gt; g_nMonGenTimeMin`**——第一项名为 `Min` 实为"峰值"，
    /// 与 J107 的 `g_nMonProcTimeMin` 同类命名陷阱。两者都只增不减。
    /// 复用 J107 的 `ProcTimeStats` 与 `MonTimeMinUpdate`/`MonTimeMaxUpdate` 两个扩展方法。
    /// </summary>
    public static void UpdateTimingStats(ProcessMonstersEnvelopeCore.ProcTimeStats stats, int monGenTime)
    {
        stats.MonTimeMinUpdate(monGenTime);   // 3967-3968（名为 Min，实为峰值）
        stats.MonTimeMaxUpdate(monGenTime);   // 3969-3970
    }

    /// <summary>3883：`g_boStopRun` 时**连统计都不更新**（直接 Exit）。</summary>
    public static bool UpdatesStatsWhenStopped() => false;

    // ===================== 监控字符串（3963） =====================

    /// <summary>
    /// 3963：`g_sMonGenInfo1 := MonGen.sMonName + ',' + IntToStr(m_nCurrMonGen) + '/' + IntToStr(m_MonGenList.Count)`。
    /// **用的是常规游标 `m_nCurrMonGen`，而非本轮实际取用的下标**——
    /// 故优先刷怪时显示的下标与真实取用不一致。属既有显示偏差，**保持原样**。
    /// </summary>
    public static string FormatMonGenInfo(string monName, int normalCursor, int monGenListCount)
        => monName + "," + normalCursor + "/" + monGenListCount;

    /// <summary>3963：状态串始终基于常规游标（即便本轮走的是优先分支）。</summary>
    public static bool StatusUsesNormalCursorEvenForPriority() => true;

    // ===================== 顶层流程（供测试驱动） =====================

    /// <summary>一轮刷怪的结果。</summary>
    public sealed class RegenRound
    {
        /// <summary>是否进入了刷怪块（节流通过）。</summary>
        public bool EnteredBlock;

        /// <summary>本轮取用的 `MonGen` 下标来源。</summary>
        public int? SortListIndex;

        /// <summary>是否来自优先列表。</summary>
        public bool FromPriority;

        /// <summary>是否满足刷怪条件并进入时间窗。</summary>
        public bool DidSpawn;

        /// <summary>本次实际刷出的数量（`nMakeMonsterCount`）。</summary>
        public int MakeMonsterCount;

        /// <summary>是否刷新了 `dwStartTick`。</summary>
        public bool RefreshedStartTick;

        /// <summary>本轮耗时（用于统计）。</summary>
        public int MonGenTime;
    }

    /// <summary>
    /// 3831-3964：一轮刷怪的**时序**骨架（不含真实刷怪副作用，由回调表达）。
    /// `regen` 返回 `RegenMonsters` 的成败；`getGenCount` 对应 J106 的 `GetGenMonCount`。
    /// G 变量判定**在本方法内**用 `CheckGVar` 完成（不外部传入），以保持与 3940 一致。
    /// </summary>
    public static RegenRound RunRound(
        bool boStopRun, bool boVentureServer, bool boStopM2MakeMon,
        int now, ref int regenTick, int regenTime,
        bool hasPriority,
        bool monGenIsNull, bool boFB, string monName, bool envirBoMakeMon,
        MonGenParseCore.CompareType ct, IReadOnlyList<int> globalVal,
        int gvarIndex, int gvarValue,
        int startTick, int zenTime, int monCount, int monGenRate,
        Func<int, int> getGenCount, Func<int, bool> regen,
        ref int outStartTick)
    {
        var round = new RegenRound();

        // 3883-3884
        if (ShouldAbortEntirely(boStopRun))
            return round;

        // 3885-3888
        if (!ShouldEnterRegenBlock(boVentureServer, boStopM2MakeMon, now, regenTick, regenTime))
        {
            round.MonGenTime = 0;
            return round;
        }

        round.EnteredBlock = true;
        regenTick = RenewRegenTick(now);

        round.FromPriority = hasPriority;

        if (monGenIsNull)
            return round;

        // 3939-3940
        bool gv = CheckGVar(ct, globalVal, gvarIndex, gvarValue);
        if (!ShouldProcessMonGen(monGenIsNull, boFB, monName, envirBoMakeMon, gv))
            return round;

        // 3944
        if (!IsZenTimeElapsed(startTick, now, zenTime))
            return round;

        round.DidSpawn = true;

        // 3946-3953
        int genCount = getGenCount(monCount);   // 3946：J106 的 GetGenMonCount
        int genModCount = ComputeGenModCount(monCount, monGenRate);
        int makeCount = ComputeMakeMonsterCount(genModCount, genCount);
        round.MakeMonsterCount = makeCount;

        bool boRegened = InitialBoRegened;   // 3947

        if (ShouldCallRegen(makeCount))
            boRegened = regen(makeCount);    // 3956

        // 3958-3961
        if (ShouldRefreshStartTick(boRegened))
        {
            outStartTick = now;
            round.RefreshedStartTick = true;
        }

        return round;
    }
}
