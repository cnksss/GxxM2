using System;

namespace GXX.M2Server;

/// <summary>
/// `TUserEngine.ProcessMonsters` 的**外层骨架** 1:1 移植（批次J107），
/// 主源 `UsrEngn.pas` 3974-4199，另含统计量的消费端 `svMain.pas` 767-774。
///
/// J106 移植了候选表构建（4001-4104）、J105 移植了节流消费（4138-4163），
/// 本批次补上把它们**包起来**的三段：
/// ① 开头的准备段（3989-4000）；
/// ② 结尾的统计段（4165-4174）；
/// ③ 异常处理与"异常怪物"诊断段（4175-4198）。
/// 至此 `ProcessMonsters` 全过程（3974-4199）在 C# 侧完整覆盖。
///
/// **原文过程名是 `TUserEngine.ProcessMonsters`（3974），不是 `RunMonster`**——
/// J105/J106 叙述中的 "RunMonster 主循环" 实为该过程内部，
/// 本批次按原文名归档，以免后续检索时对不上。
///
/// **统计段（4165-4174）有三处刻意的不对称，均按 1:1 保留**：
///
/// | 统计量 | 计算 | 更新条件 |
/// |---|---|---|
/// | `g_nMonProcTime` | `now - dwMonProcTick`（**本轮开始时刻**） | 无条件赋值 |
/// | `g_nMonProcTimeMin` | 无 | `if g_nMonProcTime > Min` → **取更大值**（**名为 Min 却存最大值**） |
/// | `g_nMonProcTimeMax` | 无 | `if g_nMonProcTime > Max` → 取更大值 |
/// | `g_nMonTimeMin` | `now - dwRunTick`（**同为本轮开始时刻**） | 无条件赋值 |
/// | `g_nMonTimeMax` | 无 | `if Max < Min` → 即 `if g_nMonTimeMin > Max` → 取更大值 |
///
/// **`g_nMonProcTimeMin` 名为 "Min" 却在 `g_nMonProcTime > Min` 时被赋成更大的值**（4167-4168）——
/// 它的语义实际是"**历史峰值**"，与名字相反。这不是笔误导致的偶发错误，
/// 而是稳定存在的行为：`svMain.pas` 773-774 会每帧把它 `Dec` 2（下限 1，见 `if > 1`），
/// 776-778 同理对 `g_nMonTimeMax` 每帧 `Dec` 1 —— **即这两个量是被"缓慢衰减的峰值"**，
/// 用于界面显示 `MonP:当前/峰值/峰值`（svMain.pas 699）。
/// 由于它们每帧被衰减，故"取更大值"是合理的峰值维护写法，只是命名让人误以为是最小值。
///
/// **`g_nMonTimeMin` / `g_nMonProcTime` 每轮无条件覆盖**（4172/4166），
/// 故它们表示"**最近一轮**的耗时"；而两个 Max/Min 表示衰减峰值。
/// **`dwMonProcTick` 与 `dwRunTick` 都在本轮开始处取值**（3992/3998），
/// 故 `g_nMonProcTime` 与 `g_nMonTimeMin` 的**起点相同、终点相同**，
/// 二者在无异常时**必然相等**——它们只是在不同位置被赋值，语义上是一个量的两份拷贝。
///
/// **异常段（4175-4198）的两处细节**：
/// ① `IsError` 只在 `Monster <> nil` 时才置 `True`（4181）；
/// ② 故若异常发生在 `Monster` 仍为 `nil` 的早期阶段，**只打印消息、不置 IsError**，
///    进而 4189 的"异常怪物"诊断段也不会执行。
/// ③ 4189 `if IsError and (Monster <> nil)` 用 `m_boGamePet` 区分打印"异常宠物"/"异常怪物"；
///    其内部 `try...except` **空处理**（4196-4197），即诊断打印本身失败也被吞掉。
/// **4180/4185 用 `E.Message`，而 4178 判断的是 `Monster <> nil`**——
/// 即"是哪个怪物出错"取决于**异常抛出那一刻** `Monster` 变量的值，
/// 而该变量在 4009/4024/4114 三处被赋值，故它可能是"最后处理的那个"。
/// </summary>
public static class ProcessMonstersEnvelopeCore
{
    /// <summary>3987：`'[Exception] TUserEngine.ProcessMonsters %d; %s'`。</summary>
    public static string ExceptionMsg(int tCode, string message)
        => $"[Exception] TUserEngine.ProcessMonsters {tCode}; {message}";

    /// <summary>4178/4185：`Monster = nil` 时消息里的第二参退化为字面量 `'nil'`。</summary>
    public const string NilPlaceholder = "nil";

    /// <summary>4193/4195：异常对象诊断串。</summary>
    public static string AbnormalObjectMsg(bool boGamePet, string charName)
        => boGamePet ? $"异常宠物: {charName}" : $"异常怪物: {charName}";

    /// <summary>
    /// 4178-4186：异常发生时的消息第二参——`Monster &lt;&gt; nil` 时用真实消息，否则用 `'nil'`。
    /// </summary>
    public static string SelectExceptionDetail(bool monsterIsNull, string exceptionMessage)
        => monsterIsNull ? NilPlaceholder : exceptionMessage;

    /// <summary>
    /// 4178-4182：`IsError` **仅**在 `Monster &lt;&gt; nil` 时置真（4181）。
    /// 故早期阶段的异常不会触发 4189 的诊断段。
    /// </summary>
    public static bool ShouldSetIsError(bool monsterIsNull) => !monsterIsNull;

    /// <summary>4189：是否打印"异常怪物/宠物"诊断（需 `IsError` 且 `Monster <> nil`）。</summary>
    public static bool ShouldPrintAbnormalObject(bool isError, bool monsterIsNull)
        => isError && !monsterIsNull;

    /// <summary>
    /// 统计量快照。字段名按原文，语义差异见类注释。
    /// </summary>
    public sealed class ProcTimeStats
    {
        /// <summary>3857：名为 Min，实为**衰减峰值**。</summary>
        public int MonProcTimeMin;

        /// <summary>3858：峰值。</summary>
        public int MonProcTimeMax;

        /// <summary>3851：最近一轮怪物处理耗时。</summary>
        public int MonTimeMin;

        /// <summary>3852：峰值。</summary>
        public int MonTimeMax;
    }

    /// <summary>
    /// 4165-4174：统计段。
    /// 返回本轮的两个耗时值 `(g_nMonProcTime, g_nMonTimeMin)`——
    /// 二者起点（3992/3998）与终点相同，故无异常时**必然相等**。
    /// </summary>
    public static (int MonProcTime, int MonTime) UpdateTimingStats(
        ProcTimeStats stats, uint now, uint monProcTick, uint runTick)
    {
        // 4166：无条件赋值
        int monProcTime = unchecked((int)(now - monProcTick));
        stats.MonProcTimeMinUpdate(monProcTime);
        stats.MonProcTimeMaxUpdate(monProcTime);

        // 4172：无条件赋值
        int monTime = unchecked((int)(now - runTick));
        stats.MonTimeMaxUpdate(monTime);

        return (monProcTime, monTime);
    }

    /// <summary>
    /// svMain.pas 767-778：这两个统计量每帧被**衰减**——
    /// `if g_nMonTimeMax > 0 then Dec(g_nMonTimeMax)`、
    /// `if g_nMonProcTimeMin > 1 then Dec(g_nMonProcTimeMin, 2)`。
    /// **注意两者的下限与步长都不同**（0/步长 1 与 1/步长 2）。
    /// </summary>
    public static void DecayStats(ProcTimeStats stats)
    {
        // svMain.pas 767-768
        if (stats.MonTimeMax > 0)
            stats.MonTimeMax -= 1;

        // svMain.pas 773-774：下限 1、步长 2
        if (stats.MonProcTimeMin > 1)
            stats.MonProcTimeMin -= 2;
    }

    /// <summary>
    /// svMain.pas 699：界面显示的 `MonP:当前/Min/Max` 串。
    /// </summary>
    public static string FormatMonProcDisplay(int monProcTime, int monProcTimeMin, int monProcTimeMax)
        => $" MonP:{monProcTime}/{monProcTimeMin}/{monProcTimeMax}";

    /// <summary>
    /// 4201-4218：`GetGenMonCount`——统计某刷怪点中**既未死亡也非幽灵**的怪物数。
    /// **注意条件是两个否定同时成立**（`not m_boDeath and not m_boGhost`），
    /// 且 `nil` 条目直接跳过（不计入）。
    /// </summary>
    public static int GetGenMonCount(ReadOnlySpan<(bool IsNull, bool BoDeath, bool BoGhost)> certs)
    {
        int count = 0;

        foreach (var (isNull, boDeath, boGhost) in certs)
        {
            if (isNull)
                continue;

            if (!boDeath && !boGhost)
                count++;
        }

        return count;
    }
}

/// <summary>批次J107：`ProcTimeStats` 的峰值维护扩展（放在独立文件以免污染核心类型）。</summary>
public static class ProcTimeStatsExtensions
{
    /// <summary>4167-4168：`if g_nMonProcTime > g_nMonProcTimeMin then g_nMonProcTimeMin := g_nMonProcTime`。</summary>
    public static void MonProcTimeMinUpdate(this ProcessMonstersEnvelopeCore.ProcTimeStats s, int value)
    {
        if (value > s.MonProcTimeMin)
            s.MonProcTimeMin = value;
    }

    /// <summary>4169-4170：`if g_nMonProcTime > g_nMonProcTimeMax then g_nMonProcTimeMax := g_nMonProcTime`。</summary>
    public static void MonProcTimeMaxUpdate(this ProcessMonstersEnvelopeCore.ProcTimeStats s, int value)
    {
        if (value > s.MonProcTimeMax)
            s.MonProcTimeMax = value;
    }

    /// <summary>4173-4174：`if g_nMonTimeMax &lt; g_nMonTimeMin then g_nMonTimeMax := g_nMonTimeMin`。</summary>
    public static void MonTimeMaxUpdate(this ProcessMonstersEnvelopeCore.ProcTimeStats s, int value)
    {
        if (s.MonTimeMax < value)
            s.MonTimeMax = value;
    }

    /// <summary>
    /// 3967-3968（`ProcessRegenMonsters`）：`if g_nMonGenTime &gt; g_nMonGenTimeMin then g_nMonGenTimeMin := g_nMonGenTime`。
    /// **字段名为 `Min`，比较方向却是"更大才更新"**——与 4173 的 `MonTimeMaxUpdate`
    /// 同为"峰值"语义，但写成了不同的比较方向（`&lt;` 与 `&gt;`）。
    /// 两者在初值同为 0 且取值非负时结果一致，故**不可据结果推断原文写法**；
    /// 移植时按原文逐字保留 `&gt;`。
    /// </summary>
    public static void MonTimeMinUpdate(this ProcessMonstersEnvelopeCore.ProcTimeStats s, int value)
    {
        if (value > s.MonTimeMin)
            s.MonTimeMin = value;
    }
}
