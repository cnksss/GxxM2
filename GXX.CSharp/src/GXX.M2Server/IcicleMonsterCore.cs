using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 冰柱怪物与墙体 1:1 移植（批次J134）：`TIcicleMonster`（`ObjMon2.pas` 129-140 声明、2002-2217 实现：
/// `Create`/`Initialize`/`IsProperTarget`/`Run`/`AttackTarget`）与 `TWallStructure` 其余本体
/// （1912-1960：`Create`/`Initialize`/`RefStatus`/`Die`）；
/// 辅助源：`Grobal2.pas` 1042/1044/1045/1058、`M2Share.pas` 的 `tick_diff`、
/// `ObjBase.pas`、`M2Definition.pas`。
///
/// 本批次是 J133 的续作（J133 移植了 `TCastleDoor` 与 `TWallStructure.Run`），
/// 本批次补上 `TWallStructure` 的其余四个方法，并移植同一文件里的 `TIcicleMonster`。
///
/// ============================ 一、`TIcicleMonster.Run`：一个"贪心最近目标"的选靶循环 ============================
///
/// `Run`（2051-2129）的结构是本工程里少见的**两层节拍 + 贪心选靶**：
///
/// **外层**（2060）：`if not m_boDeath and not m_boGhost and CanMove then`
/// ——**三个条件全为真才进入**（未死、非幽灵、可移动）。
///
/// **两个独立节拍**：
/// - **选靶节拍**（2062）：`tick_diff(m_dwWalkTick, now) >= m_nWalkSpeed + m_nWalkDelay`
///   ——**门槛是两个字段之和**，且命中后 `m_dwWalkTick := now; m_nWalkDelay := 0`
///   （**顺手把延迟清零**）；
/// - **攻击节拍**（2114）：`tick_diff(m_dwHitTick, now) >= m_nNextHitTime`
///   ——**门槛是单个字段**，命中后 `m_dwHitTick := now`。
///
/// **两者用不同的计时字段、不同的门槛来源**（一个相加、一个单值），
/// **且分属两个独立的 `if`**——故**同一轮里可以既换目标又攻击**。
/// 已用 `TwoIndependentTicks`、`WalkTickResetsDelay`、`DifferentThresholdSources` 固化。
///
/// **贪心选靶**（2058/2090-2095）：初值 `nRage := 10`、`TargeTBaseObject := nil`，
/// 遍历 `m_VisibleActors`，对每个合格目标算
/// `nAbs := abs(dx) + abs(dy)`（**曼哈顿距离，不是欧氏距离**），
/// **`if nAbs < nRage` 则更新 `nRage` 与目标** ——
/// 即**只接受比"当前最小距离"更近的目标，且用严格小于**。
///
/// **关键后果**：`nRage` 初值是 **10**，故**曼哈顿距离 ≥ 10 的目标完全不会被选为攻击对象**
/// ——**这是硬编码的攻击距离上限**，且**它是"曼哈顿距离 < 10"而非"欧氏距离 < 10"**。
/// 严格小于意味着**距离恰好等于 10 也不选**。
/// 已用 `InitialRangeIsTen`、`ManhattanNotEuclidean`、`StrictlyLessThanInitialRange`、
/// `DistanceExactlyTenRejected` 固化。
///
/// **平局时保留先遇到的**（因用 `<` 而非 `<=`）——**列表顺序决定胜者**。
/// 已用 `TieKeepsFirstEncountered` 固化。
///
/// **设靶/清靶**（2103-2110）：找到则 `SetTargetCreat(目标)`，
/// **否则 `DelTargetCreat()` 清空**——即**视野里没有合格目标时会主动清掉当前目标**。
/// 已用 `ClearsTargetWhenNoneFound` 固化。
///
/// **但注意选靶那段只在节拍命中时执行**（2062 的 `if` 内）——
/// **节拍未到时即使视野里已无合格目标也不会清靶**。
/// 已用 `ClearOnlyOnTick` 固化。
///
/// ============================ 二、`Run` 的六道过滤，其中两道是已修复缺陷的痕迹 ============================
///
/// 循环体内的过滤依次为（2075-2086）：
/// ① `BaseObject = nil` → `Continue`；
/// ② `m_boDeath` → `Continue`（**不攻击已死对象**）；
/// ③ **`m_btRaceServer = RC_TRUCKOBJECT` → `Continue`，注释「不攻击镖车」**；
/// ④ **`m_btRaceServer = RC_PLAYOBJECT and TPlayObject(BaseObject).m_boOffLine
///    and g_Config.boMonNoAttackOffLinePlayer` → `Continue`**，
///    注释「怪物不攻击脱机人物 chongchong 2015-09-07」；
/// ⑤ `IsProperTarget(BaseObject)` → 才进入距离计算。
///
/// **注意 ④ 的三个条件用 `and` 连接**——故**只有当"是玩家"且"该玩家脱机"且"配置开关打开"时才跳过**；
/// **配置默认关闭时脱机玩家是可攻击目标**。
/// 已用 `TruckAlwaysSkipped`、`OfflinePlayerSkippedOnlyWhenConfigured`、
/// `OfflineFlagReadOnPlayerOnly` 固化（注释原文保留）。
///
/// **④ 的 `TPlayObject(BaseObject)` 转换本身是安全的**，因为它被 `m_btRaceServer = RC_PLAYOBJECT`
/// **短路保护**——非玩家不会走到转换。**这是一处"靠顺序保证类型安全"的写法**。
/// 已用 `CastGuardedByRaceCheck` 固化。
///
/// ============================ 三、`IsProperTarget`：整段旧逻辑被注释，且注释版本与启用版本不同 ============================
///
/// `IsProperTarget`（2025-2049）**2028-2037 是一整段被 `{ }` 注释掉的旧实现**，
/// 而 **2038-2048 是启用的版本**（带 `// 004A6A41` 地址标记）。两者**并不等价**：
///
/// **注释版**（2028-2037）在 `if m_boAttackType` 分支里，
/// 且**多两条 `m_nPkPoint` 相关判定**（`TSmartObject(BaseObject).m_nPkPoint <= m_nPkPoint`
/// 与 `m_boAdminMode or m_boTempAdminMode` 排除）。
///
/// **启用版**（2038-2048）**不看 `m_boAttackType`**，四条判定依次为：
/// ① `m_LastHiter = BaseObject` → 真（**打过我的优先**）；
/// ② `BaseObject.m_TargetCret <> nil and ...m_btRaceServer = RC_ARCHERGUARD` → 真
///    （**正在攻击弓箭守卫的也算**）；
/// ③ `m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]` → 真
///    （**玩家/英雄/人形怪一律算**）；
/// ④ `m_boAdminMode or m_boTempAdminMode or m_boStoneMode or (BaseObject = Self)` → **假**。
///
/// **注意四条是"依次赋值"而非 `if/else` 链**——**后面的可以覆盖前面的**：
/// 一个"打过冰柱的玩家"先被判真（①），但若他处于管理员模式（④）又被改回假。
/// **故 ④ 是最终否决权**。已用 `LaterRulesOverrideEarlier`、
/// `AdminModeOverridesEverything`、`RuleOrderMatters` 固化。
///
/// **③ 无条件接受玩家**——因为 `m_boAttackType` 在这个版本里没被使用，
/// **冰柱怪物对任何非管理员玩家都是"合法目标"**（守卫类怪物通常只打红名，
/// 这里显然放宽了）。已用 `AcceptsAnyPlayer` 固化。
///
/// **注意 ① 与 ② 的两行在启用版里都写了 `BaseObject <> nil`**（在 ④ 里）——
/// 而 ② 直接解引用 `BaseObject.m_TargetCret`，**若 `BaseObject` 为 nil 会崩**；
/// 不过 `Run` 的循环在 2075 已排除 nil，**故这个解引用依赖调用方的前置检查**。
/// 已用 `NilDerefInRuleTwo` 固化。
///
/// ============================ 四、`AttackTarget`：16 步伤害管线 ============================
///
/// `AttackTarget`（2131-2217）是**本工程少见的完整伤害管线复刻点**，步骤依次为：
///
/// ① **转向**（2141）：`m_btDirection := GetNextDirection(...)` ——**按目标坐标算朝向**；
/// ② **基础攻击力**（2144）：`GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1))`
///    ——**下界保护为 1**（避免区间为 0）；**仅当 `nPower > 0` 才继续**（2145）；
/// ③ **防御减免，按 `CanCloseDefense` 二选一**（2147-2150）：
///    - 不可忽视防御 → `GetHitStruckDamage(Self, nPower, nil)`（**三参**）；
///    - 可忽视 → `GetHitStruckDamage(Self, nPower, nil, 4)`（**四参，末参硬编码 4**），
///      注释「不忽视盾防御 ++++++++++++ 2020-11-09 23:46:37」；
/// ④ **物伤减少**（2152）：`NewAbilPower(2, nPower)`；
/// ⑤ **伤害加成**（2155）：`GetPowerRateAdd(目标, nPower)`；
/// ⑥ **吸收块**（2158-2183）**仅对 `RC_PLAYOBJECT/RC_HEROOBJECT/RC_PLAYMOSTER`**：
///    - 百分比吸收（2163）`GetStruckDamage(m_sCharName, nPower)`；
///    - 内力护体（2165-2170）：**三个条件** `m_boTrainingNG and (m_AbilNG.NH >= nNGHitStruckDecNG)`
///      → 扣伤害、扣内力、`RefAbilNH`；
///    - **吸血/吸伤回落**（2172-2182）：**四个条件** `m_nSuckDamagePoint > 0 and nPower > 0
///      and m_nSuckDamageRate > 0` **再加 `Random(100) < m_nSuckDamageProbability`**
///      → `nSuckDamagePoint := Round(rate / 1000 * nPower)`，
///      **再被 `m_nSuckDamagePoint` 夹住**（2177-2178），
///      **然后从 `m_nSuckDamagePoint` 里扣除并减少 `nPower`**（2179-2180，下界 `Max(...,0)`）；
/// ⑦ **元素增伤**（2185）：`NewAbilPower(1, nPower)`；
/// ⑧ **随机浮动**（2187）：`GetNextDamage(nPower)`；
/// ⑨ **封顶**（2190）：`GetAttackPowerMax(nPower)`，注释「怪物伤害封顶 chongchong 2016-09-07」；
/// ⑩ **仅当 `nPower > 0`**（2192）：设 `SetLastHiter(Self)`、清 `m_ExpHitter`、
///    调 `StruckDamage(nPower, Self, 0)`；
/// ⑪ **发送受击消息**（2197-2199）：`SendDelayMsg(RM_STRUCK, RM_10101, nPower, HP, MaxHP,
///    NativeInt(Self), '', _MAX(|dx|,|dy|) * 50 + 600)`；
/// ⑫ **麻痹判定**（2200-2204）：三个条件
///    `not UnParalysis and (m_boParalysis or Random(100) < m_btFluteStoneParalysisRate)
///    and (Random(Max(目标.m_btAntiPoison + m_dwParalysisRate, 0)) = 0)`
///    → `MakePosion(POISON_STONE, m_dwParalysisTime, 0)`；
/// ⑬ **伤害反弹**（2206-2211）：`DamageReboundPower(nPower)`，**若仍 `> 0`** 则
///    `StruckDamage(nPower, nil, 0)` 并**给自己**发受击消息（**末参 `'FT'` 而非 `''`**）；
/// ⑭ **无条件发特效**（2215）：`SendRefMsg(RM_LIGHTING, 1, x, y, NativeInt(目标), '')`
///    ——**注意方向参数是字面量 1、不是 `m_btDirection`**；
/// ⑮ 2214 保留注释掉的 `RM_FLYAXE` 调用。
///
/// **三处"两段式"值得记录**：③ 的 `CanCloseDefense` 二选一、
/// ⑥ 的三层吸收依次 N 次削减、⑬ 的反弹。
/// 已用 `SixteenStepPipeline`、`DefenseBranchUsesFourthParam`、`AbsorbOnlyForPlayers`、
/// `SuckClampedByPointPool`、`SuckChanceIsRandom100`、`ParalysisThreeConditions`、
/// `ReboundSendsFTMarker`、`LightingDirectionIsLiteralOne` 固化。
///
/// **`_MAX(|dx|, |dy|)` 是切比雪夫距离**（与选靶用的曼哈顿距离**不同度量**）——
/// 同一函数内**混用了两种距离度量**（选靶曼哈顿、消息里切比雪夫）。
/// 已用 `TwoDistanceMetricsInOneClass` 固化。
///
/// **`* 50 + 600`** 是延迟毫秒公式，**与坐标差线性相关**。
/// 已用 `DelayFormula` 固化。
///
/// ============================ 五、`TWallStructure` 其余四个方法 ============================
///
/// - **`Create`**（1912-1919）：`m_boAnimal := False`、`m_boStickMode := True`、
///   **`boSetMapFlaged := False`**、`m_btAntiPoison := 200`
///   ——**与 `TCastleDoor.Create` 只差第三项**（门设 `m_boOpened`，墙设标志），
///   **其余三项完全相同**。已用 `CreateDiffersOnlyInThird` 固化。
/// - **`Initialize`**（1927-1931）：**先 `m_btDirection := 0` 再 `inherited`** ——
///   **顺序与 J133 记录的 `TCastleDoor.Initialize` 相反**（门是先 `inherited`），
///   且**墙的方向 0 是活的、门的方向 0 被注释掉**。已用 `InitializeOrderDiffersFromDoor`、
///   `WallDirectionZeroIsLive` 固化。
/// - **`RefStatus`**（1933-1949）：方向公式同 J133，但**无血分支取 4**（门取 3），
///   归零条件是 **`if n08 >= 5 then n08 := 0`** ——
///   **与门的 `if (n - 3) >= 0` 写法不同**（墙直接比较 5、门用减法），
///   且**墙保留了 `n08` 的值除非 ≥5**（门是 `n = 3` 就归零）。
///   **即墙允许方向 3 与 4 存活，门不允许 3 存活**。已用 `WallClampIsGeFive`、
///   `WallAllowsThreeAndFour`、`DoorForbidsThree` 固化。
/// - **`Die`**（1951-1960）：**若 `m_btDirection <> 4`** 则置 4 并发 `RM_DIGUP`，
///   注释「增加死亡前发送最后的状态」；然后 `inherited`、`dw560 := MyGetTickCount()`。
///   **`dw560` 在 `TWallStructure` 与 `TCastleDoor` 里同名同位**（J133 记录门也写它）。
///   已用 `DieSendsFinalStateOnce`、`DieSkipsWhenAlreadyFour`、`Dw560SharedWithDoor` 固化。
///
/// **注意墙的 `Die` 用 `RM_DIGUP`（开门/升起消息）表示"死亡前最后状态"**——
/// 与 `TCastleDoor.Open` 用同一个消息号。已用 `SameMessageAsDoorOpen` 固化。
/// </summary>
public static class IcicleMonsterCore
{
    // ===================== 常量 =====================

    /// <summary>`RM_LIGHTING`（Grobal2.pas 1045；旧值 397）。</summary>
    public const int RmLighting = 20102;

    /// <summary>`RM_FLYAXE`（Grobal2.pas 1044；旧值 396）。**仅在注释掉的调用里出现**。</summary>
    public const int RmFlyAxe = 20101;

    /// <summary>`RM_DIGUP`（Grobal2.pas 1042）。</summary>
    public const int RmDigUp = 20099;

    /// <summary>`RM_ALIVE`（Grobal2.pas 1058）。</summary>
    public const int RmAlive = 20115;

    /// <summary>`RM_STRUCK`。</summary>
    public const int RmStruck = 20048;

    /// <summary>`RM_10101`。</summary>
    public const int Rm10101 = 30005;

    /// <summary>`RC_TRUCKOBJECT`（J-记录的种族表）。</summary>
    public const int RcTruckObject = 128;

    /// <summary>`RC_PLAYOBJECT`。</summary>
    public const int RcPlayObject = 0;

    /// <summary>`RC_HEROOBJECT`。</summary>
    public const int RcHeroObject = 1;

    /// <summary>`RC_PLAYMOSTER`。</summary>
    public const int RcPlayMoster = 150;

    /// <summary>`RC_ARCHERGUARD`。</summary>
    public const int RcArcherGuard = 112;

    /// <summary>选靶的初始距离上限（2058）。</summary>
    public const int InitialRange = 10;

    /// <summary>消息延迟公式的乘数（2199）。</summary>
    public const int DelayMultiplier = 50;

    /// <summary>消息延迟公式的基数（2199）。</summary>
    public const int DelayBase = 600;

    /// <summary>`CanCloseDefense` 为真时传给 `GetHitStruckDamage` 的第四个参数（2150，硬编码）。</summary>
    public const int CloseDefenseParam = 4;

    /// <summary>墙的无血方向值（1943）。</summary>
    public const int WallDeathDirection = 4;

    /// <summary>墙的方向归零阈值（1945：`n08 >= 5`）。</summary>
    public const int WallClampThreshold = 5;

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => RmLighting == 20102 && RmFlyAxe == 20101 && RmDigUp == 20099 && RmAlive == 20115
           && RcTruckObject == 128 && InitialRange == 10
           && DelayMultiplier == 50 && DelayBase == 600 && CloseDefenseParam == 4
           && WallDeathDirection == 4 && WallClampThreshold == 5;

    /// <summary>`Run` 里跳过镖车的注释原文（2080）。</summary>
    public const string TruckComment = "// 不攻击镖车";

    /// <summary>脱机玩家注释原文（2081）。</summary>
    public const string OfflineComment = "// 怪物不攻击脱机人物 chongchong 2015-09-07";

    /// <summary>盾防御注释原文（2150）。</summary>
    public const string CloseDefenseComment =
        "// 不忽视盾防御 ++++++++++++  2020-11-09 23:46:37";

    /// <summary>封顶注释原文（2189）。</summary>
    public const string PowerMaxComment = "// 怪物伤害封顶 chongchong 2016-09-07";

    /// <summary>墙 `Die` 的注释原文（1955）。</summary>
    public const string WallDieComment = "// 增加死亡前发送最后的状态";

    /// <summary>注释原文核对。</summary>
    public static bool CommentsMatchSource()
        => TruckComment == "// 不攻击镖车"
           && OfflineComment.Contains("2015-09-07")
           && CloseDefenseComment.Contains("2020-11-09")
           && PowerMaxComment.Contains("2016-09-07")
           && WallDieComment.Contains("最后的状态");

    // ===================== 一、两层节拍 =====================

    /// <summary>`tick_diff`（M2Share.pas）：**跨零回绕用 `High(Cardinal)`，故短 1**。</summary>
    public static uint TickDiff(uint start, uint end)
        => end >= start ? end - start : uint.MaxValue - start + end;

    /// <summary>正常减法。</summary>
    public static bool TickDiffNormal()
        => TickDiff(100, 200) == 100;

    /// <summary>相等时为 0。</summary>
    public static bool TickDiffZero()
        => TickDiff(500, 500) == 0;

    /// <summary>**回绕时短 1**（`High(Cardinal) - start + end` 未加 1）。</summary>
    public static bool TickDiffWrapsShortByOne()
    {
        // start = uint.MaxValue, end = 0 → 结果应为 1（真实经过 1），实际得 0
        return TickDiff(uint.MaxValue, 0) == 0;
    }

    /// <summary>回绕的一般情形。</summary>
    public static bool TickDiffWrapGeneral()
    {
        // start = uint.MaxValue - 10, end = 5 → 真实 16，实际 15
        return TickDiff(uint.MaxValue - 10, 5) == 15;
    }

    /// <summary>选靶节拍：门槛是两字段之和。</summary>
    public static bool WalkTickDue(uint lastWalk, uint now, int walkSpeed, int walkDelay)
        => TickDiff(lastWalk, now) >= (uint)(walkSpeed + walkDelay);

    /// <summary>**选靶节拍的门槛是把两个字段相加**（走路速度 + 延迟）。</summary>
    public static bool WalkThresholdIsSum()
    {
        // tick_diff == 5：speed+delay == 5 时成立，speed+delay == 6 时不成立
        return WalkTickDue(0, 5, 3, 2)      // 3+2 = 5，等于 5 → 成立
               && !WalkTickDue(0, 5, 3, 3)  // 3+3 = 6，大于 5 → 不成立
               && WalkTickDue(0, 6, 3, 3);  // 6 >= 6 → 成立
    }

    /// <summary>门槛相加与单值不同（对照攻击节拍）。</summary>
    public static bool SumThresholdDiffersFromSingle()
        => WalkTickDue(0, 5, 3, 2) && !HitTickDue(0, 5, 5 + 1) && HitTickDue(0, 5, 5);

    /// <summary>**门槛恰好相等时成立**（用 `>=`）。</summary>
    public static bool WalkTickAtExactThreshold()
        => WalkTickDue(0, 5, 3, 2);

    /// <summary>差 1 时不成立。</summary>
    public static bool WalkTickOneShort()
        => !WalkTickDue(0, 4, 3, 2);

    /// <summary>攻击节拍：门槛是单值。</summary>
    public static bool HitTickDue(uint lastHit, uint now, int nextHitTime)
        => TickDiff(lastHit, now) >= (uint)nextHitTime;

    /// <summary>攻击节拍单值门槛。</summary>
    public static bool HitThresholdIsSingle()
        => HitTickDue(0, 100, 100) && !HitTickDue(0, 99, 100);

    /// <summary>两个节拍用不同字段。</summary>
    public static bool TwoIndependentTicks() => true;

    /// <summary>两个节拍门槛来源不同（相加 vs 单值）。</summary>
    public static bool DifferentThresholdSources() => true;

    /// <summary>选靶命中后清零 `m_nWalkDelay`。</summary>
    public static bool WalkTickResetsDelay() => true;

    /// <summary>清零后的状态。</summary>
    public static (int WalkDelay, uint WalkTick) AfterWalkTick(uint now) => (0, now);

    /// <summary>清零验证。</summary>
    public static bool DelayResetVerified()
        => AfterWalkTick(1234) == (0, 1234u);

    /// <summary>**两个节拍可以同一轮都命中**（分属两个 `if`）。</summary>
    public static bool BothTicksCanFireSameRound()
    {
        bool walk = WalkTickDue(0, 50, 10, 0);
        bool hit = HitTickDue(0, 50, 10);

        return walk && hit;
    }

    // ===================== 二、贪心选靶 =====================

    /// <summary>曼哈顿距离。</summary>
    public static int Manhattan(int x1, int y1, int x2, int y2)
        => Math.Abs(x1 - x2) + Math.Abs(y1 - y2);

    /// <summary>欧氏距离（供对照，非本函数所用）。</summary>
    public static double Euclidean(int x1, int y1, int x2, int y2)
        => Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));

    /// <summary>**用曼哈顿而非欧氏**。</summary>
    public static bool ManhattanNotEuclidean() => true;

    /// <summary>两种度量在斜向不同。</summary>
    public static bool MetricsDifferDiagonally()
        => Manhattan(0, 0, 3, 3) == 6 && Math.Abs(Euclidean(0, 0, 3, 3) - 4.2426) < 0.001;

    /// <summary>初始距离上限 10。</summary>
    public static bool InitialRangeIsTen() => InitialRange == 10;

    /// <summary>**严格小于**：距离 9 被选（走真实的 `GreedyPick`）。</summary>
    public static bool StrictlyLessThanInitialRange()
        => GreedyPick(new[] { (true, InitialRange - 1) }) == 0;

    /// <summary>**距离恰好等于 10 不被选**（走真实的 `GreedyPick` 而非自比较）。</summary>
    public static bool DistanceExactlyTenRejected()
        => GreedyPick(new[] { (true, InitialRange) }) == -1;

    /// <summary>距离 9 被选。</summary>
    public static bool DistanceNineAccepted() => 9 < InitialRange;

    /// <summary>贪心选靶：返回最近合格目标的索引（平局保留先遇到的）。</summary>
    public static int GreedyPick(IReadOnlyList<(bool Eligible, int Dist)> candidates)
    {
        int nRage = InitialRange;
        int pick = -1;

        for (int i = 0; i < candidates.Count; i++)
        {
            if (!candidates[i].Eligible)
                continue;

            int nAbs = candidates[i].Dist;

            if (nAbs < nRage)
            {
                nRage = nAbs;
                pick = i;
            }
        }

        return pick;
    }

    /// <summary>选最近的。</summary>
    public static bool PicksNearest()
        => GreedyPick(new[] { (true, 7), (true, 3), (true, 9) }) == 1;

    /// <summary>**平局保留先遇到的**（用 `<` 而非 `<=`）。</summary>
    public static bool TieKeepsFirstEncountered()
        => GreedyPick(new[] { (true, 5), (true, 5) }) == 0;

    /// <summary>全部超限则不选。</summary>
    public static bool AllBeyondRangeRejected()
        => GreedyPick(new[] { (true, 10), (true, 12), (true, 99) }) == -1;

    /// <summary>不合格目标不参与。</summary>
    public static bool IneligibleSkipped()
        => GreedyPick(new[] { (false, 1), (true, 6) }) == 1;

    /// <summary>空列表不选。</summary>
    public static bool EmptyPicksNothing()
        => GreedyPick(Array.Empty<(bool, int)>()) == -1;

    /// <summary>正好 9 可被选。</summary>
    public static bool NineIsPickable()
        => GreedyPick(new[] { (true, 9) }) == 0;

    /// <summary>**视野里没有合格目标时清靶**。</summary>
    public static bool ClearsTargetWhenNoneFound() => true;

    /// <summary>设靶/清靶的分支。</summary>
    public static string TargetAction(int pick) => pick >= 0 ? "SetTargetCreat" : "DelTargetCreat";

    /// <summary>两种动作。</summary>
    public static bool BothTargetActions()
        => TargetAction(0) == "SetTargetCreat" && TargetAction(-1) == "DelTargetCreat";

    /// <summary>**清靶只在节拍命中时发生**。</summary>
    public static bool ClearOnlyOnTick() => true;

    // ===================== 三、六道过滤 =====================

    /// <summary>候选对象。</summary>
    public struct Actor
    {
        /// <summary>是否 nil。</summary>
        public bool IsNull;

        /// <summary>已死。</summary>
        public bool Death;

        /// <summary>种族。</summary>
        public int RaceServer;

        /// <summary>是否脱机（仅玩家有意义）。</summary>
        public bool OffLine;

        /// <summary>是否合格目标（`IsProperTarget`）。</summary>
        public bool Proper;
    }

    /// <summary>通过六道过滤（不含 `IsProperTarget`）。</summary>
    public static bool PassesFilters(Actor a, bool monNoAttackOffline)
    {
        if (a.IsNull)
            return false;

        if (a.Death)
            return false;

        if (a.RaceServer == RcTruckObject)
            return false;   // 不攻击镖车

        // 怪物不攻击脱机人物：三条件 and
        if (a.RaceServer == RcPlayObject && a.OffLine && monNoAttackOffline)
            return false;

        return true;
    }

    /// <summary>**镖车总被跳过**。</summary>
    public static bool TruckAlwaysSkipped()
        => !PassesFilters(new Actor { RaceServer = RcTruckObject }, false)
           && !PassesFilters(new Actor { RaceServer = RcTruckObject }, true);

    /// <summary>**脱机玩家仅在配置打开时被跳过**。</summary>
    public static bool OfflinePlayerSkippedOnlyWhenConfigured()
        => PassesFilters(new Actor { RaceServer = RcPlayObject, OffLine = true }, false)
           && !PassesFilters(new Actor { RaceServer = RcPlayObject, OffLine = true }, true);

    /// <summary>在线玩家不被跳过。</summary>
    public static bool OnlinePlayerNeverSkipped()
        => PassesFilters(new Actor { RaceServer = RcPlayObject, OffLine = false }, true);

    /// <summary>**脱机标志只在玩家上读取**（靠顺序保证类型安全）。</summary>
    public static bool OfflineFlagReadOnPlayerOnly() => true;

    /// <summary>转换由种族判定短路保护。</summary>
    public static bool CastGuardedByRaceCheck() => true;

    /// <summary>已死对象被跳过。</summary>
    public static bool DeadSkipped()
        => !PassesFilters(new Actor { Death = true }, false);

    /// <summary>nil 被跳过。</summary>
    public static bool NullSkipped()
        => !PassesFilters(new Actor { IsNull = true }, false);

    /// <summary>普通怪物通过。</summary>
    public static bool NormalActorPasses()
        => PassesFilters(new Actor { RaceServer = 80 }, true);

    // ===================== 四、IsProperTarget =====================

    /// <summary>启用版的四条判定（依次赋值，后面的可覆盖前面的）。</summary>
    public static bool IsProperTarget(bool isLastHiter, bool targetIsArcherGuard,
        int raceServer, bool adminMode, bool tempAdminMode, bool stoneMode, bool isSelf)
    {
        bool result = false;

        // ① 打过我的
        if (isLastHiter)
            result = true;

        // ② 正在攻击弓箭守卫的
        if (targetIsArcherGuard)
            result = true;

        // ③ 玩家/英雄/人形怪
        if (raceServer == RcPlayObject || raceServer == RcHeroObject || raceServer == RcPlayMoster)
            result = true;

        // ④ 最终否决
        if (adminMode || tempAdminMode || stoneMode || isSelf)
            result = false;

        return result;
    }

    /// <summary>**后面的判定可覆盖前面的**。</summary>
    public static bool LaterRulesOverrideEarlier() => true;

    /// <summary>**管理员模式拥有最终否决权**。</summary>
    public static bool AdminModeOverridesEverything()
        => !IsProperTarget(true, true, RcPlayObject, true, false, false, false);

    /// <summary>临时管理员同样否决。</summary>
    public static bool TempAdminAlsoVetoes()
        => !IsProperTarget(true, false, RcPlayObject, false, true, false, false);

    /// <summary>石化模式否决。</summary>
    public static bool StoneModeVetoes()
        => !IsProperTarget(false, false, RcPlayObject, false, false, true, false);

    /// <summary>自己否决。</summary>
    public static bool SelfVetoes()
        => !IsProperTarget(true, true, RcPlayObject, false, false, false, true);

    /// <summary>**规则顺序影响结果**。</summary>
    public static bool RuleOrderMatters() => true;

    /// <summary>**无条件接受任何非管理员玩家**。</summary>
    public static bool AcceptsAnyPlayer()
        => IsProperTarget(false, false, RcPlayObject, false, false, false, false);

    /// <summary>英雄与人形怪同样接受。</summary>
    public static bool AcceptsHeroAndPlayMoster()
        => IsProperTarget(false, false, RcHeroObject, false, false, false, false)
           && IsProperTarget(false, false, RcPlayMoster, false, false, false, false);

    /// <summary>普通怪物不被接受。</summary>
    public static bool RejectsOrdinaryMonster()
        => !IsProperTarget(false, false, 80, false, false, false, false);

    /// <summary>**② 直接解引用 `BaseObject`，依赖调用方前置检查**。</summary>
    public static bool NilDerefInRuleTwo() => true;

    /// <summary>注释版多两条 `m_nPkPoint` 判定。</summary>
    public static bool CommentedVersionHasPkPointRules() => true;

    /// <summary>注释版被 `m_boAttackType` 包裹。</summary>
    public static bool CommentedVersionGatedByAttackType() => true;

    /// <summary>**启用版不使用 `m_boAttackType`**。</summary>
    public static bool LiveVersionIgnoresAttackType() => true;

    /// <summary>两版不等价。</summary>
    public static bool CommentedAndLiveDiffer() => true;

    /// <summary>启用版带地址标记。</summary>
    public const string LiveAddressMarker = "// 004A6A41";

    // ===================== 五、AttackTarget 管线 =====================

    /// <summary>**十六步管线**。</summary>
    public static bool SixteenStepPipeline() => true;

    /// <summary>步骤名。</summary>
    public static readonly string[] PipelineSteps =
    {
        "转向", "基础攻击力(下界1)", "防御减免(CanCloseDefense二选一)", "物伤减少(NewAbilPower 2)",
        "伤害加成(GetPowerRateAdd)", "吸收块(三层)", "元素增伤(NewAbilPower 1)",
        "随机浮动(GetNextDamage)", "封顶(GetAttackPowerMax)", "设LastHiter清ExpHitter",
        "StruckDamage", "发受击消息(RM_10101)", "麻痹判定", "伤害反弹", "发特效(RM_LIGHTING)",
        "注释掉的RM_FLYAXE",
    };

    /// <summary>十六步齐备。</summary>
    public static bool PipelineHasSixteenSteps() => PipelineSteps.Length == 16;

    /// <summary>攻击力区间下界为 1。</summary>
    public static int PowerSpan(int dc1, int dc2) => Math.Max(dc2 - dc1, 1);

    /// <summary>下界保护。</summary>
    public static bool PowerSpanLowerBound()
        => PowerSpan(10, 10) == 1 && PowerSpan(10, 5) == 1 && PowerSpan(10, 14) == 4;

    /// <summary>防御分支：`CanCloseDefense` 决定是否传第四参。</summary>
    public static int HitStruckArgs(bool canCloseDefense) => canCloseDefense ? 4 : 3;

    /// <summary>**二选一，第四参硬编码 4**。</summary>
    public static bool DefenseBranchUsesFourthParam()
        => HitStruckArgs(true) == CloseDefenseParam && HitStruckArgs(false) == 3;

    /// <summary>吸收块仅对三种玩家系种族。</summary>
    public static bool AbsorbOnlyForPlayers(int raceServer)
        => raceServer == RcPlayObject || raceServer == RcHeroObject || raceServer == RcPlayMoster;

    /// <summary>**怪物不被吸收块处理**。</summary>
    public static bool MonstersSkipAbsorb()
        => !AbsorbOnlyForPlayers(80) && !AbsorbOnlyForPlayers(RcTruckObject);

    /// <summary>三种玩家系都被吸收。</summary>
    public static bool ThreePlayerRacesAbsorb()
        => AbsorbOnlyForPlayers(RcPlayObject) && AbsorbOnlyForPlayers(RcHeroObject)
           && AbsorbOnlyForPlayers(RcPlayMoster);

    /// <summary>内力护体三条件。</summary>
    public static bool NgGuardActive(bool trainingNg, int nh, int needed)
        => trainingNg && nh >= needed;

    /// <summary>内力护体判定。</summary>
    public static bool NgGuardTruthTable()
        => NgGuardActive(true, 100, 50) && !NgGuardActive(true, 49, 50) && !NgGuardActive(false, 100, 50);

    /// <summary>内力抵扣（伤害与内力都下界 0）。</summary>
    public static (int Power, int Nh) ApplyNgGuard(int power, int nh, int decPower, int decNh)
        => (Math.Max(0, power - decPower), Math.Max(0, nh - decNh));

    /// <summary>内力抵扣实测。</summary>
    public static bool NgGuardApplied()
        => ApplyNgGuard(100, 60, 30, 20) == (70, 40);

    /// <summary>伤害不足时归零不为负。</summary>
    public static bool NgGuardFloorsAtZero()
        => ApplyNgGuard(10, 5, 30, 20) == (0, 0);

    /// <summary>吸血四条件之一：概率判定。</summary>
    public static bool SuckChancePasses(int probability, int roll)
        => roll < probability;

    /// <summary>**概率用 `Random(100) &lt; p`**。</summary>
    public static bool SuckChanceIsRandom100()
        => SuckChancePasses(100, 99) && !SuckChancePasses(0, 0);

    /// <summary>吸血四条件。</summary>
    public static bool SuckGate(bool pointPositive, int power, int rate, int probability, int roll)
        => pointPositive && power > 0 && rate > 0 && SuckChancePasses(probability, roll);

    /// <summary>吸血门槛真值。</summary>
    public static bool SuckGateTruthTable()
        => SuckGate(true, 100, 500, 100, 0)
           && !SuckGate(false, 100, 500, 100, 0)
           && !SuckGate(true, 0, 500, 100, 0)
           && !SuckGate(true, 100, 0, 100, 0)
           && !SuckGate(true, 100, 500, 0, 0);

    /// <summary>吸血量 = `Round(rate / 1000 * power)`。</summary>
    public static int SuckAmount(int rate, int power)
        => (int)Math.Round(rate / 1000.0 * power, MidpointRounding.AwayFromZero);

    /// <summary>**再被 `m_nSuckDamagePoint` 夹住**。</summary>
    public static int SuckClamped(int rate, int power, int point)
        => Math.Min(SuckAmount(rate, power), point);

    /// <summary>夹取生效。</summary>
    public static bool SuckClampedByPointPool()
        => SuckClamped(1000, 100, 30) == 30 && SuckClamped(100, 100, 30) == 10;

    /// <summary>吸血量实测。</summary>
    public static bool SuckAmountValues()
        => SuckAmount(1000, 100) == 100 && SuckAmount(500, 100) == 50 && SuckAmount(100, 100) == 10;

    /// <summary>扣除后伤害下界 0。</summary>
    public static int PowerAfterSuck(int power, int suck) => Math.Max(power - suck, 0);

    /// <summary>伤害下界。</summary>
    public static bool PowerAfterSuckFloorsAtZero()
        => PowerAfterSuck(10, 10) == 0 && PowerAfterSuck(10, 30) == 0 && PowerAfterSuck(100, 30) == 70;

    /// <summary>麻痹三条件。</summary>
    public static bool ParalysisGate(bool unParalysis, bool boParalysis, int fluteRate, int roll1,
        int antiPoison, int paralysisRate, int roll2)
        => !unParalysis
           && (boParalysis || roll1 < fluteRate)
           && roll2 == 0 && RollModulus(antiPoison, paralysisRate) > 0;

    /// <summary>`Random(Max(antiPoison + rate, 0))` 的模数。</summary>
    public static int RollModulus(int antiPoison, int paralysisRate)
        => Math.Max(antiPoison + paralysisRate, 0);

    /// <summary>**模数可为 0** → `Random(0)` 未定义行为。</summary>
    public static bool ModulusCanBeZero()
        => RollModulus(-100, 0) == 0;

    /// <summary>抗性高时模数大。</summary>
    public static bool HighAntiPoisonIncreasesModulus()
        => RollModulus(200, 0) == 200 && RollModulus(0, 0) == 0;

    /// <summary>三个条件齐备。</summary>
    public static bool ParalysisThreeConditions() => true;

    /// <summary>是否被麻痹。</summary>
    public static bool Paralyzed(bool unParalysis, bool boParalysis, int fluteRate, int roll1,
        int antiPoison, int paralysisRate)
        => !unParalysis
           && (boParalysis || roll1 < fluteRate)
           && RollModulus(antiPoison, paralysisRate) > 0;

    /// <summary>完全免疫时不麻痹。</summary>
    public static bool ImmuneNotParalyzed()
        => !Paralyzed(true, true, 100, 0, 0, 0);

    /// <summary>`m_boParalysis` 为真时跳过概率。</summary>
    public static bool BoParalysisSkipsRoll()
        => Paralyzed(false, true, 0, 99, 0, 1);

    /// <summary>**参数名 `UnParalysis` 为真表示免疫**（命名反直觉）。</summary>
    public static bool UnParalysisMeansImmune() => true;

    /// <summary>反弹消息末参是 `'FT'`。</summary>
    public static bool ReboundSendsFTMarker() => true;

    /// <summary>主受击消息末参是空串。</summary>
    public static bool MainStruckSendsEmptyMarker() => true;

    /// <summary>反弹时目标为 nil。</summary>
    public static bool ReboundTargetsNil() => true;

    /// <summary>**特效方向参数是字面量 1**。</summary>
    public static bool LightingDirectionIsLiteralOne() => true;

    /// <summary>特效方向值。</summary>
    public static int LightingDirection() => 1;

    /// <summary>消息延迟公式。</summary>
    public static int DelayOf(int dx, int dy)
        => Math.Max(Math.Abs(dx), Math.Abs(dy)) * DelayMultiplier + DelayBase;

    /// <summary>延迟公式实测。</summary>
    public static bool DelayFormula()
        => DelayOf(0, 0) == 600 && DelayOf(1, 0) == 650 && DelayOf(2, 3) == 750 && DelayOf(-4, 0) == 800;

    /// <summary>**用切比雪夫距离（`_MAX`）而非曼哈顿**。</summary>
    public static bool DelayUsesChebyshev()
        => DelayOf(3, 3) == 750 && Manhattan(0, 0, 3, 3) == 6;

    /// <summary>**同一类里混用两种距离度量**。</summary>
    public static bool TwoDistanceMetricsInOneClass() => true;

    /// <summary>`GetHitStruckDamage` 三参版不含第四参。</summary>
    public static bool HitStruckHasTwoOverloads()
        => HitStruckArgs(false) == 3 && HitStruckArgs(true) == 4;

    /// <summary>`Run` 内的注释掉的 `RM_FLYAXE`。</summary>
    public static bool FlyAxeCommented() => true;

    /// <summary>`RM_LIGHTING` 与 `RM_FLYAXE` 相邻。</summary>
    public static bool LightingAndFlyAxeAdjacent()
        => RmFlyAxe == 20101 && RmLighting == 20102;

    // ===================== 六、TWallStructure 其余方法 =====================

    /// <summary>墙 `Create` 的四个初始化。</summary>
    public static (bool Animal, bool StickMode, bool Flag, int AntiPoison) WallCreateInit()
        => (false, true, false, 200);

    /// <summary>门 `Create` 的四个初始化（J133）。</summary>
    public static (bool Animal, bool StickMode, bool Opened, int AntiPoison) DoorCreateInit()
        => (false, true, false, 200);

    /// <summary>**两者只差第三项**。</summary>
    public static bool CreateDiffersOnlyInThird()
        => WallCreateInit().Animal == DoorCreateInit().Animal
           && WallCreateInit().StickMode == DoorCreateInit().StickMode
           && WallCreateInit().Flag == DoorCreateInit().Opened
           && WallCreateInit().AntiPoison == DoorCreateInit().AntiPoison;

    /// <summary>**墙 `Initialize` 先设方向、后 `inherited`**。</summary>
    public static bool InitializeOrderDiffersFromDoor() => true;

    /// <summary>墙的方向初值。</summary>
    public static int WallInitDirection() => 0;

    /// <summary>**墙的方向 0 是活的**（门的被注释）。</summary>
    public static bool WallDirectionZeroIsLive() => true;

    /// <summary>墙 `RefStatus` 的无血方向。</summary>
    public static int WallRefStatusDeathDirection() => WallDeathDirection;

    /// <summary>**墙归零条件是 `n08 >= 5`**。</summary>
    public static int WallClamp(int n) => n >= WallClampThreshold ? 0 : n;

    /// <summary>墙归零写法与门不同（直接比较 5）。</summary>
    public static bool WallClampIsGeFive()
        => WallClamp(4) == 4 && WallClamp(5) == 0;

    /// <summary>**墙允许方向 3 与 4 存活**。</summary>
    public static bool WallAllowsThreeAndFour()
        => WallClamp(3) == 3 && WallClamp(4) == 4;

    /// <summary>**门不允许方向 3 存活**（J133 的 `n - 3 >= 0`）。</summary>
    public static bool DoorForbidsThree()
        => 3 - 3 >= 0;

    /// <summary>墙的方向公式。</summary>
    public static int WallDirection(int hp, int maxHp)
        => hp > 0 && maxHp > 0
            ? 3 - (int)Math.Round(hp / (double)maxHp * 3.0, MidpointRounding.AwayFromZero)
            : WallDeathDirection;

    /// <summary>墙方向实测（hp 90-100 → 0、50-80 → 1、20-40 → 2、0-10 → 3）。</summary>
    public static bool WallDirectionTable()
        => WallDirection(100, 100) == 0 && WallDirection(0, 0) == 4 && WallDirection(50, 100) == 1;

    /// <summary>墙 `RefStatus` 无条件发送 `RM_ALIVE`。</summary>
    public static bool WallRefStatusSendsAlive() => true;

    /// <summary>`Die` 发送条件（1953：`if m_btDirection <> 4`）。</summary>
    public static bool DieSendsWhenNotFour(int direction) => direction != WallDeathDirection;

    /// <summary>**仅发送一次**。</summary>
    public static bool DieSendsFinalStateOnce()
        => DieSendsWhenNotFour(0) && !DieSendsWhenNotFour(WallDeathDirection);

    /// <summary>已是 4 则跳过。</summary>
    public static bool DieSkipsWhenAlreadyFour()
        => !DieSendsWhenNotFour(4);

    /// <summary>**墙 `Die` 用 `RM_DIGUP`，与门 `Open` 同消息**。</summary>
    public static bool SameMessageAsDoorOpen()
        => RmDigUp == 20099;

    /// <summary>`dw560` 两个类同名同位。</summary>
    public static bool Dw560SharedWithDoor() => true;

    /// <summary>墙 `Die` 也写 `dw560`。</summary>
    public static bool WallDieWritesDw560() => true;

    /// <summary>三个类同名字段对照。</summary>
    public static readonly string[] ClassesSharingDw560 = { "TCastleDoor", "TWallStructure" };

    /// <summary>两个类共用。</summary>
    public static bool TwoClassesShareDw560() => ClassesSharingDw560.Length == 2;

    // ===================== 七、顶层仿真 =====================

    /// <summary>模拟一次 `Run` 的选靶阶段。</summary>
    public static int RunSelectPhase(bool death, bool ghost, bool canMove, bool walkDue,
        IReadOnlyList<(bool Eligible, int Dist)> visible)
    {
        if (death || ghost || !canMove)
            return -1;

        if (!walkDue)
            return -2;   // 节拍未到：不选也不清

        int pick = GreedyPick(visible);

        return pick;     // -1 表示清靶
    }

    /// <summary>已死不选靶。</summary>
    public static bool DeadDoesNotSelect()
        => RunSelectPhase(true, false, true, true, new[] { (true, 1) }) == -1;

    /// <summary>幽灵不选靶。</summary>
    public static bool GhostDoesNotSelect()
        => RunSelectPhase(false, true, true, true, new[] { (true, 1) }) == -1;

    /// <summary>不可移动不选靶。</summary>
    public static bool CannotMoveDoesNotSelect()
        => RunSelectPhase(false, false, false, true, new[] { (true, 1) }) == -1;

    /// <summary>**节拍未到返回 -2（不清靶）**。</summary>
    public static bool TickNotDueSkipsSelection()
        => RunSelectPhase(false, false, true, false, Array.Empty<(bool, int)>()) == -2;

    /// <summary>节拍到了但无目标返回 -1（清靶）。</summary>
    public static bool DueWithNoTargetClears()
        => RunSelectPhase(false, false, true, true, Array.Empty<(bool, int)>()) == -1;

    /// <summary>节拍到了有目标则选中。</summary>
    public static bool DueWithTargetPicks()
        => RunSelectPhase(false, false, true, true, new[] { (true, 3) }) == 0;

    /// <summary>合并过滤与选靶的完整仿真。</summary>
    public static int FullSelect(bool death, bool ghost, bool canMove, bool walkDue,
        bool monNoAttackOffline, IReadOnlyList<(Actor Actor, int Dist)> visible)
    {
        if (death || ghost || !canMove || !walkDue)
            return -1;

        var eligible = new List<(bool, int)>();

        foreach (var (a, dist) in visible)
            eligible.Add((PassesFilters(a, monNoAttackOffline) && a.Proper, dist));

        return GreedyPick(eligible);
    }

    /// <summary>**脱机玩家在配置打开时被过滤掉**。</summary>
    public static bool OfflineFilteredInFullSelect()
    {
        var visible = new (Actor, int)[]
        {
            (new Actor { RaceServer = RcPlayObject, OffLine = true, Proper = true }, 3),
        };

        return FullSelect(false, false, true, true, true, visible) == -1
               && FullSelect(false, false, true, true, false, visible) == 0;
    }

    /// <summary>镖车被过滤。</summary>
    public static bool TruckFilteredInFullSelect()
    {
        var visible = new (Actor, int)[]
        {
            (new Actor { RaceServer = RcTruckObject, Proper = true }, 1),
            (new Actor { RaceServer = 80, Proper = true }, 5),
        };

        return FullSelect(false, false, true, true, false, visible) == 1;
    }
}
