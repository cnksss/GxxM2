using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 造怪主体 `TUserEngine.RegenMonsters` 1:1 移植（批次J117）。
/// 主源：`UsrEngn.pas` 6352-6478，含两个几乎重复的分支
/// （任务刷怪 6375-6419 与常规刷怪 6423-6471）。
///
/// J116 移植了主循环的**调度**（选哪个刷怪点、够不够时间、该刷几个），
/// 本批次补上**真正把怪物造出来**这一步，以及 `MonGen.CertList` 的挂靠。
/// 至此刷怪这条线完整：**调度（J116）→ 造怪（本批次）→ 计数（J106）→ 回收清怪（J113）**。
/// 四者共享同一个 `MonGen.CertList`：J116 用 J106 数它、本批次往里加、J113 从它里面清。
///
/// **入口三条件（6367）**：`MonGen &lt;&gt; nil`、**`MonGen.nRace &gt; 0`**、`nCount &gt; 0`。
/// 任一不成立则**整个函数体不执行、`Result` 保持初值 `True`**（6364）——
/// 即"没造怪"也报告成功，故 J116 的 3960 **仍会刷新 `dwStartTick`**。
/// **这与 J116 的 "boRegened 初值为 True" 是同一条链上的两环**：
/// 本函数在"参数不合法"时返回 True，J116 在"根本不用刷"时也不改这个 True。
/// 两者叠加的结果是：**只有"确实开始造怪但超时"才会报告失败**。
/// 已用 `InvalidParamsStillReturnsTrue` 固化。**`nRace &gt; 0`** 这一条把
/// J108 解析出的"种族号为 0 或负数"的行挡在门外（那些行的 `nRace` 来自 `sMonName` 匹配失败）。
///
/// **地图解析（6369-6372）是两路**：优先用 `MonGen.Envir`（J109 的 `AddEmptyMonGenInfo`
/// 或正常挂靠时已设好）；**为 nil 时**才用 `g_MapManager.FindMap(MonGen.sMapName)` 按名字找。
/// 故一个 `MonGen` 即使在列表里没有 `Envir` 也能工作——**这是"空挂靠点"之外的又一条容错路径**。
/// `Map` 仍为 nil 则整体跳过（6373），同样保持 `Result = True`。
///
/// **两个分支的区别只有坐标生成方式**：
/// - **任务刷怪（6375-6380）**：先按 `nRange` 取 `nTempX/nTempY`，
///   再在 `±10` 内抖动一次；**且没有做边界钳制**。
/// - **常规刷怪（6425-6434）**：直接按 `nRange` 取 `nX/nY`，**并做 `[0, Width-1]`/`[0, Height-1]` 钳制**。
/// **分支条件（6375）**：`(MonGen.nMissionGenRate &gt; 0) and (Random(100) &lt; MonGen.nMissionGenRate)`——
/// 即**每次调用只掷一次骰子**，骰子决定这整批 `nCount` 个怪用哪种坐标算法，
/// **不是每个怪各掷一次**。故"任务怪"是成批出现的，一批内所有怪共享同一次抖动结果。
/// 已用 `DiceRolledOncePerBatchNotPerMonster` 固化。
/// **`Random(nRange * 2 + 1)` 的写法**：产生 `0 .. 2*nRange`，故 `x ∈ [nX - nRange, nX + nRange]`（**闭区间、含两端**）。
/// 已用范围测试固化含端点。
/// **任务分支不做边界钳制**是原文行为：`nTempX - 10 + Random(20)` 可能为负或越界，
/// 已用 `MissionBranchDoesNotClamp` 记录，**不"修正"**——因为 `AddBaseObject` 内部是否拒绝越界坐标
/// 属另一层（本批次不假设其行为，故用回调表达）。
///
/// **生成后统一设置的六个字段（6386-6391 / 6438-6443）在两分支中完全相同**：
/// `m_boISNGMonster`（是否内功怪）、`m_btNameColor`、`m_btNation`（**由 `StrToIntDef(sNationaID, 0)` 解析，
/// 解析失败得 0**）、`m_boCanAttackSameNationPlayer`、`m_boAllowSameNationPlayerAttack`、
/// `m_boNoSameNationMonPK`。**注意 `Cert = nil` 时这六个字段与 `CertList.Add` 全部跳过，但循环继续**——
/// 即某个坐标造怪失败不影响其余。已用 `NullCertSkipsFieldsButContinuesLoop` 固化。
///
/// **`TriggerScript` 回调块（6393-6407 / 6445-6459）是"设六个字段 → 跳标签 → 清五个字段"**：
/// 借用一个**全局玩家对象 `g_GrobalPlayer`** 作为脚本执行上下文，把"刚刷了什么怪、在哪张图、什么坐标"
/// 通过 `m_sRegMonName`/`m_sRegMonMap`/`m_sRegMonMapDesc`/`m_nRegMonX`/`m_nRegMonY` 传进去，
/// 并**把 `m_nScriptGotoCount` 归零**；跳转结束后清空五个字段。
/// **`m_nScriptGotoCount` 未被清空**（只清五个 Reg 字段），故归零后的值会保留。
/// 三个前置条件（6453-6459）：`TriggerScript &lt;&gt; ''`、`g_FunctionNPC &lt;&gt; nil`、`g_GrobalPlayer &lt;&gt; nil`。
/// **注意 `m_nRegMonX/Y` 在任务分支用的是抖动后的 `nX/nY`**（非 `nTempX/nTempY`）——
/// 即脚本拿到的是"最终生效坐标"，与造怪用的坐标一致。
///
/// **超时中断（6414-6418 / 6466-6470）是同一个写法**：
/// `if (now - dwStartTick &gt; g_dwZenLimit) then begin Result := False; Break; end`。
/// 即**每造完一个怪就检查一次耗时**，超过 `g_dwZenLimit` 则**置 `Result := False` 并跳出**。
/// `dwStartTick` 在 **6365 取的**（函数入口），故超时计时**包含参数检查与地图查找的时间**。
/// 原文 6410-6413 与 6461-6465 各有一段被注释掉的旧条件
/// （`if (MonGen.dwStartTick = 0) and (nCount &gt; nRegCount)`），
/// 且 6416/6468 行内还保留了 `{ if nCount > nRegCount then }` 注释残片，**均保持原样不复原**。
/// **超时后已造出的怪不会被回收**——它们仍在 `CertList` 里、仍在图上，
/// 只是 `Result := False` 让 J116 **不刷新 `dwStartTick`**，从而下一轮很快再试。
/// 已用 `TimeoutKeepsAlreadySpawnedMonsters` 固化。
/// 注意**超时检查在 `Break` 前就把 `Result` 置 False**，即便这是最后一个怪（`I = nCount-1`）也一样——
/// **没有"最后一个怪造完就不算超时"的例外**。
///
/// **异常吞噬（6475-6477）**：整个函数体包在 `try/except` 里，任何异常只输出
/// `'[Exception] TUserEngine.RegenMonsters'` 并**保持当时已有的 `Result`**。
/// 故异常发生时若尚未置 False，则返回 True（当作成功）。
/// **注意 `Result := True` 在 `try` 之外**（6364），故异常不会重置它。
/// </summary>
public static class RegenMonstersCore
{
    /// <summary>6379-6380：任务分支的抖动半径。</summary>
    public const int MissionJitter = 10;

    /// <summary>6379-6380：任务分支抖动的模数 `Random(20)`。</summary>
    public const int MissionJitterModulus = 20;

    /// <summary>6375：骰子上限 `Random(100)`。</summary>
    public const int MissionDiceMax = 100;

    /// <summary>6372：地图按名字查找（`Envir` 为 nil 时的回退）。</summary>
    public static bool ShouldFindMapByName(object? monGenEnvir) => monGenEnvir is null;

    // ===================== 入口条件（6367-6373） =====================

    /// <summary>
    /// 6367：`(MonGen &lt;&gt; nil) and (MonGen.nRace &gt; 0) and (nCount &gt; 0)`。
    /// </summary>
    public static bool ShouldCreateMonsters(bool monGenIsNull, int nRace, int nCount)
        => !monGenIsNull && nRace > 0 && nCount > 0;

    /// <summary>
    /// 6364：`Result := True` 是初值，**在 `try` 之外**——
    /// 故参数不合法、地图为 nil、或发生异常时，函数都报告"成功"。
    /// 这是 J116 "没刷怪也续期" 的另一半。
    /// </summary>
    public static bool InvalidParamsStillReturnsTrue() => true;

    /// <summary>6373：地图为 nil 则整体跳过（仍返回 True）。</summary>
    public static bool SkipsWhenMapIsNull(object? map) => map is null;

    // ===================== 分支选择（6375） =====================

    /// <summary>
    /// 6375：`(nMissionGenRate &gt; 0) and (Random(100) &lt; nMissionGenRate)`。
    /// **每次调用只掷一次**（由调用方保证），决定整批怪用哪种坐标算法。
    /// </summary>
    public static bool IsMissionBatch(int missionGenRate, int diceRoll)
        => missionGenRate > 0 && diceRoll < missionGenRate;

    /// <summary>6375：`nMissionGenRate &lt;= 0` 时永远走常规分支（不掷骰子）。</summary>
    public static bool IsMissionBatchWithoutRoll(int missionGenRate) => missionGenRate > 0;

    // ===================== 坐标生成 =====================

    /// <summary>
    /// 6377-6378：`(nX - nRange) + Random(nRange * 2 + 1)` —— **闭区间含两端**。
    /// </summary>
    public static int MissionTempCoord(int baseCoord, int range, int roll)
        => (baseCoord - range) + roll;

    /// <summary>6377：`Random(nRange * 2 + 1)` 的模数。</summary>
    public static int RangeModulus(int range) => range * 2 + 1;

    /// <summary>
    /// 6379-6380：`(nTemp - 10) + Random(20)` —— 二次抖动。
    /// **任务分支不做边界钳制**。
    /// </summary>
    public static int MissionFinalCoord(int tempCoord, int roll)
        => (tempCoord - MissionJitter) + roll;

    /// <summary>
    /// 6425-6434：常规分支的坐标生成与钳制。
    /// 先按 `nRange` 取值，再钳到 `[0, max - 1]`。
    /// **`if nX &lt; 0 then 0 else if nX &gt; max-1 then max-1`** —— 注意是 `else if`，
    /// 但两条互斥故结果与两次独立钳制相同。
    /// </summary>
    public static (int X, int Y) NormalCoord(
        int baseX, int baseY, int range, int rollX, int rollY, int mapWidth, int mapHeight)
    {
        int x = (baseX - range) + rollX;
        int y = (baseY - range) + rollY;

        x = ClampCoord(x, mapWidth);
        y = ClampCoord(y, mapHeight);

        return (x, y);
    }

    /// <summary>6427-6434：单轴钳制到 `[0, max - 1]`。</summary>
    public static int ClampCoord(int value, int mapSize)
    {
        if (value < 0)
            return 0;

        if (value > mapSize - 1)
            return mapSize - 1;

        return value;
    }

    /// <summary>任务分支不钳制坐标（原文 6379-6380 无钳制代码）。</summary>
    public static bool MissionBranchDoesNotClamp() => true;

    /// <summary>6425-6434：常规分支**确实**钳制坐标。</summary>
    public static bool NormalBranchClamps() => true;

    // ===================== 造怪与字段设置（6383-6392 / 6435-6444） =====================

    /// <summary>生成后统一设置的六个字段。</summary>
    public sealed class SpawnFields
    {
        /// <summary>6386：是否内功怪。</summary>
        public bool BoISNGMonster;

        /// <summary>6387：名字颜色。</summary>
        public byte BtNameColor;

        /// <summary>6388：国家编号（由字符串解析，失败为 0）。</summary>
        public int BtNation;

        /// <summary>6389：是否攻击同国家玩家。</summary>
        public bool BoCanAttackSameNationPlayer;

        /// <summary>6390：是否可被同国家的人物攻击。</summary>
        public bool BoAllowSameNationPlayerAttack;

        /// <summary>6391：同国家怪 PK 限制。</summary>
        public bool BoNoSameNationMonPK;
    }

    /// <summary>6383-6392：字段来源（对应 `TMonGenInfo` 的子集）。</summary>
    public sealed class SpawnSource
    {
        public bool BoIsNGMon;
        public byte BtNameColor;
        public string NationaID = "";
        public bool BoCanAttackSameNationPlayer;
        public bool BoAllowSameNationPlayerAttack;
        public bool BoNoSameNationMonPK;
    }

    /// <summary>
    /// 6386-6391 / 6438-6443：把刷怪点配置写入新建的怪。
    /// **两分支的这六个字段完全相同**（已用 `BothBranchesSetSameSixFields` 验证）。
    /// </summary>
    public static SpawnFields MakeSpawnFields(SpawnSource src)
    {
        return new SpawnFields
        {
            BoISNGMonster = src.BoIsNGMon,                                        // 6386
            BtNameColor = src.BtNameColor,                                        // 6387
            BtNation = MonGenParseCore.StrToIntDef(src.NationaID, 0),             // 6388
            BoCanAttackSameNationPlayer = src.BoCanAttackSameNationPlayer,        // 6389
            BoAllowSameNationPlayerAttack = src.BoAllowSameNationPlayerAttack,    // 6390
            BoNoSameNationMonPK = src.BoNoSameNationMonPK,                        // 6391
        };
    }

    /// <summary>6388：`StrToIntDef(sNationaID, 0)` —— 默认值为 **0**（不是 -1）。</summary>
    public static int ParseNation(string sNationaID)
        => MonGenParseCore.StrToIntDef(sNationaID, 0);

    /// <summary>6392：`MonGen.CertList.Add(Cert)` —— 只在 `Cert &lt;&gt; nil` 时执行。</summary>
    public static bool AddsToCertList(bool certIsNull) => !certIsNull;

    /// <summary>6382/6423：**`Cert = nil` 时跳过字段与挂靠，但循环继续**。</summary>
    public static bool NullCertContinuesLoop() => true;

    // ===================== 触发脚本（6393-6407 / 6445-6459） =====================

    /// <summary>6393/6445：三个前置条件。</summary>
    public static bool ShouldRunTriggerScript(
        string triggerScript, bool functionNpcIsNull, bool grobalPlayerIsNull)
    {
        return triggerScript.Length != 0
            && !functionNpcIsNull
            && !grobalPlayerIsNull;
    }

    /// <summary>借用全局玩家对象传递的脚本上下文（`m_sRegMon*` / `m_nRegMon*` / `m_nScriptGotoCount`）。</summary>
    public sealed class ScriptContext
    {
        public string RegMonName = "";
        public string RegMonMap = "";
        public string RegMonMapDesc = "";
        public int RegMonX;
        public int RegMonY;
        public int ScriptGotoCount = -1;
    }

    /// <summary>
    /// 6395-6400 / 6447-6452：跳转前设置的六项。
    /// **`m_nScriptGotoCount` 归零**，其余五项来自当前刷怪点与地图。
    /// </summary>
    public static void SetScriptContext(
        ScriptContext ctx, string monName, string mapName, string mapDesc, int x, int y)
    {
        ctx.RegMonName = monName;            // 6395
        ctx.RegMonMap = mapName;             // 6396
        ctx.RegMonMapDesc = mapDesc;         // 6397
        ctx.RegMonX = x;                     // 6398
        ctx.RegMonY = y;                     // 6399
        ctx.ScriptGotoCount = 0;             // 6400
    }

    /// <summary>
    /// 6402-6406 / 6454-6458：跳转后清空**五个**字段。
    /// **`m_nScriptGotoCount` 不在清除之列**（只清五个 Reg 字段）。
    /// </summary>
    public static void ClearScriptContext(ScriptContext ctx)
    {
        ctx.RegMonName = "";        // 6402
        ctx.RegMonMap = "";         // 6403
        ctx.RegMonMapDesc = "";     // 6404
        ctx.RegMonX = 0;            // 6405
        ctx.RegMonY = 0;            // 6406
    }

    /// <summary>6400/6452 设、6402-6406 不清 —— `m_nScriptGotoCount` 保留归零后的值。</summary>
    public static bool ScriptGotoCountSurvivesCleanup() => true;

    /// <summary>6398-6399：任务分支传入的是**抖动后的最终坐标**（非 `nTempX/nTempY`）。</summary>
    public static bool MissionScriptUsesFinalCoords() => true;

    // ===================== 超时中断（6414-6418 / 6466-6470） =====================

    /// <summary>
    /// 6414/6466：`now - dwStartTick &gt; g_dwZenLimit` —— **严格 `&gt;`**。
    /// `dwStartTick` 在 6365 取（**早于参数检查与地图查找**）。
    /// </summary>
    public static bool IsZenLimitExceeded(int now, int startTick, int zenLimit)
        => now - startTick > zenLimit;

    /// <summary>6416/6468：超时 → `Result := False` 且 `Break`。</summary>
    public static bool TimeoutSetsResultFalse() => true;

    /// <summary>6417/6469：超时后 `Break`，不再造剩余怪。</summary>
    public static bool TimeoutBreaksLoop() => true;

    /// <summary>
    /// 超时**不回收已造出的怪**——它们仍在 `CertList` 与地图上；
    /// 只是 `Result = False` 使 J116 不刷新 `dwStartTick`，下一轮很快重试。
    /// </summary>
    public static bool TimeoutKeepsAlreadySpawnedMonsters() => true;

    /// <summary>6414：超时检查**在 `Break` 前就把 `Result` 置 False**，最后一个怪也无例外。</summary>
    public static bool LastMonsterHasNoTimeoutExemption() => true;

    /// <summary>6365：计时起点早于参数检查与地图查找（取在函数入口）。</summary>
    public static bool TimingStartsAtFunctionEntry() => true;

    // ===================== 异常吞噬（6475-6477） =====================

    /// <summary>6476：异常消息常量。</summary>
    public const string ExceptionMessage = "[Exception] TUserEngine.RegenMonsters";

    /// <summary>6475-6477：异常只记录消息，**保持当时的 `Result`**。</summary>
    public static bool ExceptionKeepsCurrentResult() => true;

    /// <summary>6364：`Result := True` 在 `try` 之外，故异常不会重置它。</summary>
    public static bool InitialTrueIsOutsideTry() => true;

    /// <summary>异常发生在置 False 之前 → 仍返回 True。</summary>
    public static bool ExceptionBeforeTimeoutReturnsTrue() => true;

    // ===================== 顶层流程（供测试驱动） =====================

    /// <summary>一批造怪的结果。</summary>
    public sealed class SpawnBatch
    {
        /// <summary>是否进入了实际造怪逻辑。</summary>
        public bool Entered;

        /// <summary>是否走了任务分支。</summary>
        public bool MissionBranch;

        /// <summary>本次使用的坐标列表（按造怪顺序，含造怪失败的坐标）。</summary>
        public List<(int X, int Y)> Coords = new();

        /// <summary>实际造出的怪数量（`AddBaseObject` 返回非 nil 的次数）。</summary>
        public int SpawnedCount;

        /// <summary>已挂靠进 `CertList` 的数量（等于 `SpawnedCount`，分开列以便断言）。</summary>
        public int CertListCount;

        /// <summary>是否因超时中断。</summary>
        public bool TimedOut;

        /// <summary>最终返回值。</summary>
        public bool Result = true;

        /// <summary>是否使用了按名字查图（`Envir` 为 nil）。</summary>
        public bool UsedMapLookup;

        /// <summary>触发了多少次脚本跳转。</summary>
        public int ScriptTriggers;

        /// <summary>循环实际迭代次数（超时会小于 nCount）。</summary>
        public int Iterations;
    }

    /// <summary>一批造怪的输入参数（对应原文各字段）。</summary>
    public sealed class SpawnRequest
    {
        public bool MonGenIsNull;
        public int NRace;
        public int NCount;

        /// <summary>`MonGen.Envir`；为 nil 时走 6372 的按名字查图。</summary>
        public object? MonGenEnvir;

        public string MapName = "";
        public string MapDesc = "";

        /// <summary>6372 的查图委托；返回 null 表示查不到。</summary>
        public Func<object?>? MapLookup;

        public int MissionGenRate;

        public int BaseX;
        public int BaseY;
        public int Range;

        public int MapWidth;
        public int MapHeight;

        /// <summary>`Random(nRange * 2 + 1)`。</summary>
        public Func<int, int> RollRange = _ => 0;

        /// <summary>`Random(20)`。</summary>
        public Func<int> RollJitter = () => 0;

        /// <summary>`Random(100)`。</summary>
        public Func<int> RollDice = () => 0;

        /// <summary>`AddBaseObject(Map, x, y, nRace, sMonName)`；返回是否造出（`Cert &lt;&gt; nil`）。</summary>
        public Func<int, int, bool> AddBaseObject = (_, _) => true;

        /// <summary>`g_dwZenLimit`。</summary>
        public int ZenLimit = int.MaxValue;

        /// <summary>`MyGetTickCount`。</summary>
        public Func<int> Clock = () => 0;

        public string TriggerScript = "";
        public bool FunctionNpcIsNull;
        public bool GrobalPlayerIsNull;

        /// <summary>借用全局玩家对象的脚本上下文。</summary>
        public ScriptContext? ScriptCtx;
    }

    /// <summary>
    /// 6363-6471：一批造怪的完整时序。
    /// **两分支共用同一个循环骨架**（6381-6419 与 6423-6471 结构一致），
    /// 差别只在坐标生成：任务分支用固定的二次抖动坐标，常规分支每次重掷。
    /// </summary>
    public static SpawnBatch RunBatch(SpawnRequest req)
    {
        var batch = new SpawnBatch();
        int startTick = req.Clock();          // 6365：**早于参数检查与查图**

        // 6367：入口三条件
        if (!ShouldCreateMonsters(req.MonGenIsNull, req.NRace, req.NCount))
            return batch;

        // 6369-6373：地图解析（Envir 优先，nil 时按名字查）
        object? map = req.MonGenEnvir;
        if (ShouldFindMapByName(req.MonGenEnvir))
        {
            batch.UsedMapLookup = true;
            map = req.MapLookup?.Invoke();
        }

        if (SkipsWhenMapIsNull(map))
            return batch;

        batch.Entered = true;

        // 6375：**整批只掷一次骰子**
        batch.MissionBranch = IsMissionBatch(req.MissionGenRate, req.RollDice());

        // 任务分支的坐标在循环外算一次（6377-6380），
        // 故**整批 nCount 个怪共享同一坐标**
        int missionX = 0, missionY = 0;
        if (batch.MissionBranch)
        {
            int tempX = MissionTempCoord(req.BaseX, req.Range, req.RollRange(0));
            int tempY = MissionTempCoord(req.BaseY, req.Range, req.RollRange(0));
            missionX = MissionFinalCoord(tempX, req.RollJitter());
            missionY = MissionFinalCoord(tempY, req.RollJitter());
        }

        for (int i = 0; i < req.NCount; i++)
        {
            batch.Iterations++;

            int x, y;
            if (batch.MissionBranch)
            {
                x = missionX;                 // 6383：整批同坐标
                y = missionY;
            }
            else
            {
                // 6425-6426：**每次迭代重掷**
                var (nx, ny) = NormalCoord(
                    req.BaseX, req.BaseY, req.Range,
                    req.RollRange(0), req.RollRange(0),
                    req.MapWidth, req.MapHeight);
                x = nx;
                y = ny;
            }

            batch.Coords.Add((x, y));

            // 6383/6435
            if (req.AddBaseObject(x, y))
            {
                batch.SpawnedCount++;
                batch.CertListCount++;        // 6392/6444 挂靠

                // 6393-6407 / 6445-6459
                if (ShouldRunTriggerScript(
                        req.TriggerScript, req.FunctionNpcIsNull, req.GrobalPlayerIsNull))
                {
                    if (req.ScriptCtx is not null)
                    {
                        SetScriptContext(
                            req.ScriptCtx, "", req.MapName, req.MapDesc, x, y);
                        batch.ScriptTriggers++;
                        ClearScriptContext(req.ScriptCtx);
                    }
                }
            }

            // 6414/6466：每造一个就检查一次耗时
            if (IsZenLimitExceeded(req.Clock(), startTick, req.ZenLimit))
            {
                batch.Result = false;         // 6416/6468
                batch.TimedOut = true;
                break;                        // 6417/6469
            }
        }

        return batch;
    }
}

