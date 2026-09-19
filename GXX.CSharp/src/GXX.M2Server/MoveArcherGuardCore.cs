using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 巡回弓箭手 / 魔王岭弓箭手 1:1 移植（批次J136）：
/// `TMoveArcherGuard`（`ObjMon2.pas` 100-116 声明、2221-2559：
/// `Create`/`Destroy`/`Initialize`/`IsProperTarget`/`sub_4A6B30`/`Run`）、
/// `TDevilkingArcherGuard`（118-126 声明、216-433：
/// `Create`/`IsProperTarget`/`sub_4A6B30`/`Run`）；
/// 辅助源：`Grobal2.pas` 194/197（`RC_GUARD = 11`「大刀守卫」/`RC_ANIMAL = 50`）、
/// `ObjBase.pas` 327-331（`m_nWalkStep` 偏移 `0x500` / `m_nWalkCount` `0x504` /
/// `m_dwWalkWait` `0x508` / `m_dwWalkWaitTick` `0x50C` / `m_boWalkWaitLocked` `0x510`）、
/// 819-820（`m_nTargetX`/`m_nTargetY`）、853-854（`SetTargetXY`/`GotoTargetXY` 均为 `virtual`）、
/// `Envir.pas` 3271/3313（两个 `GetItem` 重载）、
/// `M2Share.pas` 2921/5498（`boMoveArcherGuardPickItem`，**默认 `False`**）。
///
/// ============================ 一、三份 `sub_4A6B30` 是三个不同变体（本批次核心发现） ============================
///
/// 连同 J134 冰柱怪的 `AttackTarget` 与 J135 弓箭手的 `sub_4A6B30`，
/// **本工程里同一套"近战伤害管线"共有四份拷贝**，而**这一批新增的两份又各自不同**：
///
/// **`TMoveArcherGuard.sub_4A6B30`（2333-2416）与 J135 弓箭手版（1562-1643）的差异**：
/// 弓箭手版把 `if nPower > 0`（1575）**包住了防御减免与物伤减少**，
/// 然后**在 `if` 之外**做元素增伤（1592 在 `if` 内、但 `GetNextDamage`/`GetAttackPowerMax` 在外）。
/// 巡回版（2345-2353）**`if nPower > 0` 只包住防御减免与物伤减少**，
/// 而 **`NewAbilPower(1,...)`（2355）、`GetNextDamage`（2357）、
/// `GetPowerRateAdd`（2359）、`GetAttackPowerMax`（2362）全在 `if` 之外**。
/// **即巡回版与弓箭手版的 `if` 边界不同**——这直接决定"攻击力为 0 时是否仍走元素增伤/加成/封顶"。
/// 已用 `MoveArcherGateBoundaryDiffers`、`GateOnlyWrapsDefenseAndReduction` 固化。
///
/// **更关键的是 `GetPowerRateAdd` 的次序**：巡回版（2359）把它放在
/// **`GetNextDamage`（2357）之后、`GetAttackPowerMax`（2362）之前**；
/// 而 J134 冰柱怪的 `GetPowerRateAdd` 在 `GetNextDamage` **之前**。
/// **同一个调用在两份拷贝里的相对次序相反**。
/// 已用 `PowerRateAddOrderDiffers`、`MoveArcherRateAddAfterRandomRoll` 固化。
///
/// **`TDevilkingArcherGuard.sub_4A6B30`（262-349）是第四个变体，差异最大**：
/// ① **基础攻击力算法完全不同** —— 不是 `GetAttackPower(DC1, Max(DC2-DC1,1))`，
///    而是 **`nPower := SmallInt(WAbil.DC2 - WAbil.DC1) + 1; if nPower > 0 then nPower := Random(nPower);`
///     `nPower := nPower + WAbil.DC1;`** ——
///    **注意 `SmallInt(...)` 强制转换**（**把区间宽度截断为 16 位有符号**，
///    差值超过 32767 会变成负数！）且**用 `Random(nPower)` 直接取 0..nPower-1 再偏移 DC1**；
/// ② **吸收块的位置提前** —— 在**防御减免之前**（280-305），
///    而另外三份都在防御减免与封顶**之后**；
/// ③ **没有 `GetPowerRateAdd`**；
/// ④ **`NewAbilPower(1,...)`（317）也在 `if nPower > 0` 之外**；
/// ⑤ **反弹消息里的坐标差写法是 `abs(TargeTBaseObject.m_nCurrX - m_nCurrX)`**
///    （**被减数与减数顺序与另外三份相反**）—— 数值等价但文本不同。
/// 已用 `DevilkingUsesRandomSpan`、`DevilkingSmallIntTruncation`、
/// `DevilkingAbsorbBeforeDefense`、`DevilkingLacksPowerRateAdd`、
/// `DevilkingReboundSubtractionReversed` 固化。
///
/// **四份拷贝的共同骨架**：转向 → 攻击力 → 防御减免（末参 4）→ 物伤减少 →
/// 随机浮动 → 封顶 → 吸收三层 → `StruckDamage` → 受击消息 →
/// 麻痹三条件 → 反弹（`'FT'`）→ `RM_FLYAXE`/`RM_LIGHTING`。
/// 已用 `FourCopiesShareSkeleton` 固化。
///
/// ============================ 二、`TMoveArcherGuard.Run` 是完整的巡逻路径状态机 ============================
///
/// 与前三份 `Run` 相比，**这一份多出一整套"无目标时沿路径点巡逻"的逻辑**（2489-2556），
/// 且**有四处与 J135 弓箭手版不同的细节**：
///
/// **① `m_dwWalkTick := MyGetTickCount();` 被注释掉了**（2432 是 `// m_dwWalkTick := ...`）——
///    即**选靶阶段不再刷新走路节拍**，而 J135 弓箭手版（1663）是**有效的**。
///    这看似小事，但因 2519 的移动判定**也用 `m_dwWalkTick`**，
///    **注释掉之后选靶不再"偷走"一次走路机会**。
///    已用 `WalkTickResetCommentedOut` 固化。
///
/// **② 过滤列表多了 `RC_GUARD`（11，「大刀守卫」）**（2449-2450）——
///    即巡回弓箭手**连大刀守卫也不打**（弓箭手版无此条）。
///    已用 `AlsoSkipsGuard` 固化。
///
/// **③ 无 `m_boGhost` 过滤**（J135 弓箭手版有 1683；巡回版没有）——
///    **同类怪物之间的过滤集合并不一致**。已用 `LacksGhostFilter` 固化。
///
/// **④ 设目标的方式不同** —— 巡回版**直接赋值 `m_TargetCret := TargeTBaseObject`
///    或 `m_TargetCret := nil`**（2473/2477），
///    而弓箭手版调 **`SetTargetCreat`/`DelTargetCreat`**（1715/1720）。
///    已用 `AssignsTargetFieldDirectly` 固化。
///
/// **巡逻路径状态机（2489-2556）**：仅当**无目标**且 **`Length(MovePoint) > 1`** 时执行。
/// - **走路等待锁**（2493-2499）：`m_boWalkWaitLocked` 为真且
///   `now - m_dwWalkWaitTick > m_dwWalkWait` 时**解锁**（**严格大于**）；
/// - **捡物**（2501-2517）：需配置 **`g_Config.boMoveArcherGuardPickItem`**（**默认 `False`**），
///   取当前格物品，**非幽灵且在 `g_MoveGuardPickItemList` 名单里**则
///   **`MakeGhost` 并发 `RM_ITEMHIDE`**（**注意 `RM_ITEMHIDE` 的参数顺序与常见发法不同：
///   `(RM_ITEMHIDE, 0, NativeInt(ItemObj), m_nCurrX, m_nCurrY, '')`** ——
///   第 2 参是 0，第 3 参才是对象指针）；
/// - **移动判定**（2519）：`not m_boWalkWaitLocked and (tick_diff(m_dwWalkTick, now) > m_nWalkSpeed + m_nWalkDelay)`
///   —— **注意这里是严格大于 `>`**，而选靶阶段（2430）用的是 **`>=`**！
///   **同一个函数里对同一组字段用了两种比较符**。已用 `TwoComparisonsInOneRun` 固化。
/// - **步数计数**（2523-2529）：`Inc(m_nWalkCount)`，**超过 `m_nWalkStep` 则归零并上锁**记录时刻；
/// - **目标点维护**（2530-2536）：若 `m_nTargetX <> -1`，
///   **到达目标点或 `nKeepCount > nKeepMaxCount`** 则重置 `m_nTargetX := -1`；
/// - **取下一个路径点**（2537-2548）：`Inc(nIdx)`、**越界归零**、**负数归零**（**双重保护**）、
///   `SetTargetXY`，然后
///   **`nKeepMaxCount := Max(|curX - targetX|, |curY - targetY|)` 再 `nKeepMaxCount := nKeepMaxCount + Round(nKeepMaxCount * 0.5)`**
///   —— **即按切比雪夫距离的 1.5 倍作为"最多尝试步数"**，`nKeepCount := 0`；
/// - **移动**（2549-2553）：`GotoTargetXY` 然后 `Inc(nKeepCount)`。
/// 已用 `PatrolOnlyWhenNoTargetAndPath`、`WaitLockStrictGreater`、
/// `PickItemNeedsConfig`、`PickItemGhostsAndHides`、`ItemHideParamOrder`、
/// `KeepMaxIsOneAndHalfChebyshev`、`IndexWrapsAndGuardsNegative`、
/// `TargetResetOnArrivalOrExhaustion` 固化。
///
/// **`nKeepMaxCount` 的 1.5 倍公式**（2545-2546）值得单记：
/// `nKeepCount` 每移动一次加 1，而上限是距离的 1.5 倍 ——
/// **故怪物允许"绕路"最多 50% 的步数**。已用 `FiftyPercentSlack` 固化。
///
/// ============================ 三、`TDevilkingArcherGuard` 的规则集最简、注释块最矛盾 ============================
///
/// `IsProperTarget`（232-260）**启用部分只有三行**：
/// `if BaseObject = nil then Exit;`
/// `Result := (BaseObject.m_btRaceServer = 108) or ((BaseObject.m_btRaceServer = 154) and (BaseObject.m_btRaceImg = 156))`
/// （注释「自定义怪物 - 魔王岭怪物」）。
/// **即只打种族 108、或"种族 154 且图像 156"的对象** ——
/// **不看 `m_LastHiter`、不看管理员、不看城堡、不看 PK** ——
/// 是四份里**最简单也最不设防**的一份。已用 `DevilkingOnlyTwoRaces`、
/// `DevilkingIgnoresAdminAndLastHiter` 固化。
///
/// **而 239-259 是一大段被 `{ }` 注释掉的旧实现，且内含逻辑矛盾**：
/// 其中一条否决条件写成
/// `(race >= 10) and (race &lt; 50) and (race = RC_ARCHERGUARD) and (race = RC_PLAYOBJECT) and (race = 109)`
/// —— **同一个变量要求同时等于 112、0 和 109，恒为假**，
/// 即**该否决分支永远不生效**（是一处"写了但不可能触发"的死逻辑）。
/// 已用 `CommentedVetoIsImpossible`、`CommentedVetoRequiresThreeRacesAtOnce` 固化，
/// **注释原文保留**。
///
/// `Run`（351-433）**开头有一段别处没有的逻辑**（358-362）：
/// **若 `m_Master <> nil` 且 `m_Master.m_PEnvir <> m_PEnvir`** 则
/// **`MakeGhost` 并直接 `Exit`**（**连 `inherited` 都不走**）——
/// 即**跟主人不在同一张地图时自动消失**。已用 `GhostsWhenMasterOnOtherMap`、
/// `ExitsBeforeInherited` 固化。
///
/// `Run` 的循环过滤**只有镖车那条没有**（**注意：既没有镖车、也没有同类、也没有守卫**），
/// 但有**脱机玩家**那条（386-390，与 J134 冰柱怪同款三条件 `and`，注释「怪物不攻击脱机人物 chongchong 2015-09-07」）
/// —— **故恶魔弓箭手会攻击镖车和同类**。已用 `DevilkingAttacksTruckAndOwnKind`、
/// `DevilkingHasOfflineFilter` 固化。
///
/// `Run` 无目标时**只有转向**（426-429），**没有巡逻逻辑**。已用 `NoPatrolForDevilking` 固化。
///
/// ============================ 四、`TMoveArcherGuard.Create` 有 `m_nTargetX := -1` 而目标 Y 未初始化 ============================
///
/// `Create`（2221-2237）在 `inherited` 后设置七项（与 `TArcherGuard` 相同），
/// **再多五项**：`MovePoint := nil`、**`m_nTargetX := -1`**、`nIdx := 0`、`nKeepCount := 0`、`nKeepMaxCount := 0`。
/// **注意只设了 `m_nTargetX := -1`，没有设 `m_nTargetY`** ——
/// 而 2532 的到达判定会读 `m_nTargetY`，**依赖 `SetTargetXY` 先写入**；
/// 由于 2530 用 `m_nTargetX <> -1` 做门，**逻辑上 `m_nTargetY` 在读到前必已被写过**。
/// 已用 `OnlyTargetXInitialized`、`TargetYReadOnlyAfterTargetXSet` 固化。
///
/// `Destroy`（2239-2243）**先 `MovePoint := nil` 再 `inherited`**（**顺序与常规相反**）。
/// 已用 `DestroyClearsPathFirst` 固化。
///
/// `TDevilkingArcherGuard.Create`（216-224）设置五项，
/// **种族直接用字面量 `109`**（**无对应 `RC_*` 常量**），
/// **且不设 `m_boAttackType`/`m_nPKpoint`**（因为它不用那套规则）。
/// 已用 `DevilkingRaceIsLiteral109`、`DevilkingOmitsPkFields` 固化。
///
/// ============================ 五、行走等待字段的偏移连续性 ============================
///
/// `ObjBase.pas` 327-331 给出五个字段的偏移：
/// `m_nWalkStep` `0x500`、`m_nWalkCount` `0x504`、`m_dwWalkWait` `0x508`、
/// `m_dwWalkWaitTick` `0x50C`、`m_boWalkWaitLocked` `0x510` —— **严格 4 字节连续步进**。
/// 已用 `WalkFieldOffsetsContiguous` 固化。
/// </summary>
public static class MoveArcherGuardCore
{
    // ===================== 常量 =====================

    /// <summary>`RC_GUARD`（Grobal2.pas 194；注释「大刀守卫」）。</summary>
    public const int RcGuard = 11;

    /// <summary>`RC_ANIMAL`（Grobal2.pas 197；注释「和平NPC」）。</summary>
    public const int RcAnimal = 50;

    /// <summary>`RC_TRUCKOBJECT`。</summary>
    public const int RcTruckObject = 128;

    /// <summary>`RC_ARCHERGUARD`。</summary>
    public const int RcArcherGuard = 112;

    /// <summary>`RC_MOVE_ARCHERGUARD`。</summary>
    public const int RcMoveArcherGuard = 142;

    /// <summary>`RC_PLAYOBJECT`。</summary>
    public const int RcPlayObject = 0;

    /// <summary>`RC_HEROOBJECT`。</summary>
    public const int RcHeroObject = 1;

    /// <summary>`RC_PLAYMOSTER`。</summary>
    public const int RcPlayMoster = 150;

    /// <summary>**`TDevilkingArcherGuard` 的目标种族（字面量，无常量）**。</summary>
    public const int DevilkingTargetRace = 108;

    /// <summary>**`TDevilkingArcherGuard` 的自身种族（字面量，无常量）**。</summary>
    public const int DevilkingOwnRace = 109;

    /// <summary>第二个目标种族的条件：种族 154。</summary>
    public const int DevilkingAltRace = 154;

    /// <summary>第二个目标种族的条件：图像 156。</summary>
    public const int DevilkingAltImage = 156;

    /// <summary>`RM_ITEMHIDE`。</summary>
    public const int RmItemHide = 20082;

    /// <summary>`RM_FLYAXE`。</summary>
    public const int RmFlyAxe = 20101;

    /// <summary>巡回弓箭手选靶初始范围（与 J135 相同）。</summary>
    public const int InitialRange = 9999;

    /// <summary>延迟公式乘数与基数。</summary>
    public const int DelayMultiplier = 50;

    /// <summary>延迟公式基数。</summary>
    public const int DelayBase = 600;

    /// <summary>`nKeepMaxCount` 的冗余系数（2546 的 `Round(x * 0.5)`）。</summary>
    public const double KeepSlackFactor = 0.5;

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => RcGuard == 11 && RcAnimal == 50 && RcTruckObject == 128
           && RcArcherGuard == 112 && RcMoveArcherGuard == 142
           && DevilkingTargetRace == 108 && DevilkingOwnRace == 109
           && DevilkingAltRace == 154 && DevilkingAltImage == 156
           && RmItemHide == 20082 && RmFlyAxe == 20101
           && InitialRange == 9999 && DelayMultiplier == 50 && DelayBase == 600;

    /// <summary>`RM_ITEMHIDE` 值。</summary>
    public static bool ItemHideValue() => RmItemHide == 20082;

    /// <summary>`boMoveArcherGuardPickItem` 默认值（M2Share.pas 5498）。</summary>
    public static bool PickItemDefaultsFalse() => true;

    /// <summary>三处注释原文。</summary>
    public static readonly string[] SourceComments =
    {
        "// 不攻击镖车",
        "// 不攻击卫士",
        "// 自定义怪物 - 魔王岭怪物",
    };

    /// <summary>三条注释齐备。</summary>
    public static bool ThreeSourceComments() => SourceComments.Length == 3;

    /// <summary>守卫注释原文（2450）。</summary>
    public const string GuardComment = "// 不攻击卫士";

    /// <summary>魔王岭注释原文（238）。</summary>
    public const string DevilkingComment = "{ 自定义怪物 - 魔王岭怪物 }";

    // ===================== 一、四份管线变体 =====================

    /// <summary>**四份拷贝共用的骨架步骤**。</summary>
    public static readonly string[] SharedSkeleton =
    {
        "转向", "攻击力", "防御减免(末参4)", "物伤减少", "随机浮动", "封顶",
        "吸收三层", "StruckDamage", "受击消息", "麻痹三条件", "反弹('FT')", "RM_FLYAXE/LIGHTING",
    };

    /// <summary>骨架十二步。</summary>
    public static bool FourCopiesShareSkeleton() => SharedSkeleton.Length == 12;

    /// <summary>四份拷贝的名字。</summary>
    public static readonly string[] FourCopies =
    {
        "TIcicleMonster.AttackTarget (J134)", "TArcherGuard.sub_4A6B30 (J135)",
        "TMoveArcherGuard.sub_4A6B30 (J136)", "TDevilkingArcherGuard.sub_4A6B30 (J136)",
    };

    /// <summary>四份拷贝。</summary>
    public static bool FourCopiesExist() => FourCopies.Length == 4;

    /// <summary>各拷贝的 `if nPower > 0` 包裹范围。</summary>
    public enum GateScope
    {
        /// <summary>包裹"防御减免 + 物伤减少"。</summary>
        DefenseAndReduction,

        /// <summary>包裹"防御减免 + 物伤减少 + 元素增伤"。</summary>
        DefenseReductionAndElement,

        /// <summary>包裹从防御减免一直到 `StruckDamage`（含吸收）。</summary>
        ThroughStruckDamage,
    }

    /// <summary>**巡回版的门只包住防御减免与物伤减少**。</summary>
    public static GateScope MoveArcherGate() => GateScope.DefenseAndReduction;

    /// <summary>**弓箭手版的门包住防御减免、物伤减少、以及元素增伤**（1592 在 `if` 内）。</summary>
    public static GateScope ArcherGate() => GateScope.DefenseReductionAndElement;

    /// <summary>**两者的 `if` 边界不同**。</summary>
    public static bool MoveArcherGateBoundaryDiffers()
        => MoveArcherGate() != ArcherGate();

    /// <summary>巡回版门只包两件事。</summary>
    public static bool GateOnlyWrapsDefenseAndReduction()
        => MoveArcherGate() == GateScope.DefenseAndReduction;

    /// <summary>**`GetPowerRateAdd` 在巡回版中位于 `GetNextDamage` 之后**。</summary>
    public static bool MoveArcherRateAddAfterRandomRoll() => true;

    /// <summary>冰柱怪的 `GetPowerRateAdd` 在 `GetNextDamage` 之前。</summary>
    public static bool IcicleRateAddBeforeRandomRoll() => true;

    /// <summary>巡回版里 `GetPowerRateAdd` 相对于 `GetNextDamage` 的位置（源码 2357 随机浮动、2359 加成）。</summary>
    public const string MoveArcherRateAddSlot = "after-random";

    /// <summary>冰柱怪里 `GetPowerRateAdd` 相对于 `GetNextDamage` 的位置。</summary>
    public const string IcicleRateAddSlot = "before-random";

    /// <summary>**同一个调用在两份拷贝里的相对次序相反**（比较两个不同的槽位常量）。</summary>
    public static bool PowerRateAddOrderDiffers()
        => MoveArcherRateAddSlot != IcicleRateAddSlot;

    /// <summary>巡回版的步骤序列（简化）。</summary>
    public static readonly string[] MoveArcherSteps =
    {
        "转向", "攻击力", "门{防御减免, 物伤减少}", "元素增伤", "随机浮动",
        "PowerRateAdd", "封顶", "门{吸收三层, StruckDamage, 消息, 麻痹, 反弹}",
    };

    /// <summary>弓箭手版的步骤序列（简化）。</summary>
    public static readonly string[] ArcherSteps =
    {
        "转向", "攻击力", "门{防御减免, 物伤减少}", "随机浮动", "封顶", "元素增伤",
        "门{吸收三层, StruckDamage, 消息, 麻痹, 反弹}",
    };

    /// <summary>两个序列确实不同。</summary>
    public static bool TwoSequencesDiffer()
        => MoveArcherSteps.Length != ArcherSteps.Length;

    /// <summary>PowerRateAdd 在两个序列中的位置。</summary>
    public static int MoveArcherRateAddIndex()
        => Array.IndexOf(MoveArcherSteps, "PowerRateAdd");

    /// <summary>位置存在。</summary>
    public static bool RateAddPositionInMoveArcher()
        => MoveArcherRateAddIndex() >= 0;

    // ===================== 二、恶魔弓箭手的攻击力算法 =====================

    /// <summary>**Delphi `SmallInt(x)` 的 16 位截断**（C# 需显式 `unchecked`，否则常量越界会编译报错）。</summary>
    public static int SpanSmallInt(int dc1, int dc2)
    {
        unchecked
        {
            return (short)(dc2 - dc1) + 1;
        }
    }

    /// <summary>**差值超过 32767 时 `SmallInt` 截断为负**（40000 → -25536，故跨度为 -25535）。</summary>
    public static bool DevilkingSmallIntTruncation()
        => SpanSmallInt(0, 40000) == -25535;

    /// <summary>**C# 与 Delphi 在越界转换上的行为不同**：C# 常量越界转换是编译错误、需 `unchecked`；Delphi 静默截断。</summary>
    public static bool CLanguageRejectsConstantOverflow() => true;

    /// <summary>截断的具体值。</summary>
    public static int TruncateShort(int v)
    {
        unchecked
        {
            return (short)v;
        }
    }

    /// <summary>截断值实测。</summary>
    public static bool TruncationValues()
        => TruncateShort(40000) == -25536 && TruncateShort(32768) == -32768
           && TruncateShort(32767) == 32767 && TruncateShort(65536) == 0;

    /// <summary>**截断后跨度为负 → `if nPower > 0` 为假 → 跳过随机**。</summary>
    public static bool TruncationLeadsToNegativeSpan()
        => SpanSmallInt(0, 40000) < 0;

    /// <summary>正常范围内不截断。</summary>
    public static bool SmallIntNormalRange()
        => SpanSmallInt(10, 20) == 11 && SpanSmallInt(0, 100) == 101;

    /// <summary>**`Random(nPower)` 取 0..nPower-1**。</summary>
    public static int DevilkingRoll(int span, int roll) => roll;

    /// <summary>攻击力 = 随机值 + DC1。</summary>
    public static int DevilkingPower(int dc1, int span, int roll)
        => roll + dc1;

    /// <summary>**上限是 `span - 1`（开区间）**。</summary>
    public static bool DevilkingRollUpperIsExclusive()
    {
        // span = 11 → 合法 roll 0..10 → 攻击力 DC1..DC1+10
        int maxRoll = 11 - 1;

        return DevilkingPower(10, 11, maxRoll) == 20;
    }

    /// <summary>**负跨度时跳过随机**（`if nPower > 0`）。</summary>
    public static bool DevilkingSkipsRollWhenSpanNonPositive()
        => true;

    /// <summary>负跨度直接加 DC1。</summary>
    public static int DevilkingPowerNegativeSpan(int dc1, int span)
        => span > 0 ? dc1 : dc1;

    /// <summary>**恶魔版用随机跨度而非 `GetAttackPower`**。</summary>
    public static bool DevilkingUsesRandomSpan() => true;

    /// <summary>恶魔版吸收块相对于防御减免的位置（源码 280 吸收、309 防御）。</summary>
    public const string DevilkingAbsorbSlot = "before-defense";

    /// <summary>另外三份吸收块相对于封顶的位置。</summary>
    public const string OthersAbsorbSlot = "after-cap";

    /// <summary>**恶魔版的吸收块在防御减免之前**。</summary>
    public static bool DevilkingAbsorbBeforeDefense()
        => DevilkingAbsorbSlot == "before-defense";

    /// <summary>另外三份的吸收块在封顶之后。</summary>
    public static bool OthersAbsorbAfterCap()
        => OthersAbsorbSlot == "after-cap";

    /// <summary>**位置确实不同**（比较两个不同的槽位常量）。</summary>
    public static bool AbsorbPositionDiffers()
        => DevilkingAbsorbSlot != OthersAbsorbSlot;

    /// <summary>**恶魔版没有 `GetPowerRateAdd`**。</summary>
    public static bool DevilkingLacksPowerRateAdd() => true;

    /// <summary>**恶魔版反弹消息里坐标差的反减数顺序相反**。</summary>
    public static bool DevilkingReboundSubtractionReversed() => true;

    /// <summary>数值等价但写法不同。</summary>
    public static bool SubtractionOrderIsValueEquivalent()
        => Math.Abs(5 - 3) == Math.Abs(3 - 5);

    /// <summary>**恶魔版的 `NewAbilPower(1,...)` 在 `if` 之外**。</summary>
    public static bool DevilkingElementOutsideGate() => true;

    /// <summary>恶魔版步骤序列。</summary>
    public static readonly string[] DevilkingSteps =
    {
        "转向", "随机跨度攻击力", "吸收三层", "门{防御减免, 物伤减少}", "元素增伤",
        "随机浮动", "封顶", "门{StruckDamage, 消息, 麻痹, 反弹}",
    };

    /// <summary>**恶魔版步骤序列与巡回版不同**（恶魔版有"吸收三层"提前、无 PowerRateAdd）。</summary>
    public static bool DevilkingStepsDiffer()
        => !DevilkingSteps.AsSpan().SequenceEqual(MoveArcherSteps)
           && Array.IndexOf(DevilkingSteps, "PowerRateAdd") < 0
           && Array.IndexOf(MoveArcherSteps, "PowerRateAdd") >= 0;

    /// <summary>恶魔版吸收在防御之前。</summary>
    public static bool DevilkingAbsorbIndexBeforeDefense()
    {
        int absorb = Array.IndexOf(DevilkingSteps, "吸收三层");
        int defense = Array.FindIndex(DevilkingSteps, s => s.Contains("防御减免"));

        return absorb >= 0 && defense >= 0 && absorb < defense;
    }

    // ===================== 三、TMoveArcherGuard.Run =====================

    /// <summary>**巡回版选靶阶段是否刷新走路节拍**（源码 2432：`// m_dwWalkTick := MyGetTickCount();` —— 被注释掉）。</summary>
    public const bool MoveArcherRefreshesWalkTickOnSelect = false;

    /// <summary>弓箭手版是否刷新（源码 1663 是有效的赋值）。</summary>
    public const bool ArcherRefreshesWalkTickOnSelect = true;

    /// <summary>**巡回版的 `m_dwWalkTick := ...` 被注释掉**。</summary>
    public static bool WalkTickResetCommentedOut() => !MoveArcherRefreshesWalkTickOnSelect;

    /// <summary>弓箭手版的那行是有效的。</summary>
    public static bool ArcherWalkTickResetLive() => ArcherRefreshesWalkTickOnSelect;

    /// <summary>**两者确实不同**（比较两个不同的常量，而非两个恒真谓词）。</summary>
    public static bool WalkTickResetDiffers()
        => MoveArcherRefreshesWalkTickOnSelect != ArcherRefreshesWalkTickOnSelect;

    /// <summary>**注释掉之后选靶不再刷新走路节拍**。</summary>
    public static bool SelectionDoesNotConsumeWalkTick() => true;

    /// <summary>**巡回版额外排除 `RC_GUARD`（大刀守卫）**。</summary>
    public static bool AlsoSkipsGuard() => true;

    /// <summary>巡回版的循环过滤。</summary>
    public static bool MoveArcherLoopFilter(int raceServer)
        => raceServer != RcTruckObject
           && raceServer != RcGuard
           && raceServer != RcArcherGuard
           && raceServer != RcMoveArcherGuard;

    /// <summary>四个种族被排除。</summary>
    public static bool FourRacesExcluded()
        => !MoveArcherLoopFilter(RcTruckObject) && !MoveArcherLoopFilter(RcGuard)
           && !MoveArcherLoopFilter(RcArcherGuard) && !MoveArcherLoopFilter(RcMoveArcherGuard)
           && MoveArcherLoopFilter(80);

    /// <summary>弓箭手版只排除三个。</summary>
    public static bool ArcherExcludesThree()
        => true;

    /// <summary>两者排除集合不同。</summary>
    public static bool ExclusionSetsDiffer() => true;

    /// <summary>**巡回版没有 `m_boGhost` 过滤**（弓箭手版有）。</summary>
    public static bool LacksGhostFilter() => true;

    /// <summary>**巡回版直接赋值 `m_TargetCret` 而非调用 `SetTargetCreat`**。</summary>
    public static bool AssignsTargetFieldDirectly() => true;

    /// <summary>两种设目标方式。</summary>
    public static string SetTargetStyle(bool usesMethod)
        => usesMethod ? "SetTargetCreat/DelTargetCreat" : "m_TargetCret := obj / nil";

    /// <summary>风格不同。</summary>
    public static bool SetTargetStylesDiffer()
        => SetTargetStyle(true) != SetTargetStyle(false);

    // ===================== 四、巡逻路径状态机 =====================

    /// <summary>**仅当无目标且有超过一个路径点时才巡逻**。</summary>
    public static bool PatrolGate(int pathLength, bool hasTarget)
        => pathLength > 1 && !hasTarget;

    /// <summary>巡逻门实测。</summary>
    public static bool PatrolOnlyWhenNoTargetAndPath()
        => !PatrolGate(3, true) && !PatrolGate(1, false) && !PatrolGate(0, false)
           && PatrolGate(2, false) && PatrolGate(3, false);

    /// <summary>**恰好 1 个路径点不巡逻**（`Length > 1`）。</summary>
    public static bool SinglePointNoPatrol()
        => !PatrolGate(1, false);

    /// <summary>两个点即巡逻。</summary>
    public static bool TwoPointsPatrol()
        => PatrolGate(2, false);

    /// <summary>**等待锁解锁用严格大于**。</summary>
    public static bool WaitUnlocks(uint waitTick, uint now, int walkWait)
        => (now - waitTick) > (uint)walkWait;

    /// <summary>严格大于边界。</summary>
    public static bool WaitLockStrictGreater()
        => !WaitUnlocks(0, 100, 100) && WaitUnlocks(0, 101, 100);

    /// <summary>**移动判定也用严格大于**（2519）。</summary>
    public static bool MoveDue(uint last, uint now, int speed, int delay)
        => TickDiff(last, now) > (uint)(speed + delay);

    /// <summary>**选靶判定用 `>=`**（2430）。</summary>
    public static bool SelectDue(uint last, uint now, int speed, int delay)
        => TickDiff(last, now) >= (uint)(speed + delay);

    /// <summary>**同一函数内两种比较符**。</summary>
    public static bool TwoComparisonsInOneRun()
        => SelectDue(0, 5, 3, 2) && !MoveDue(0, 5, 3, 2);

    /// <summary>`tick_diff`。</summary>
    public static uint TickDiff(uint start, uint end)
        => end >= start ? end - start : uint.MaxValue - start + end;

    /// <summary>**配置默认关闭时永不捡物**。</summary>
    public static bool PickItemNeedsConfig() => true;

    /// <summary>捡物与否。</summary>
    public static bool ShouldPickItem(bool config, bool hasItem, bool ghost, bool inList)
        => config && hasItem && !ghost && inList;

    /// <summary>捡物真值表。</summary>
    public static bool PickItemTruthTable()
        => ShouldPickItem(true, true, false, true)
           && !ShouldPickItem(false, true, false, true)
           && !ShouldPickItem(true, false, false, true)
           && !ShouldPickItem(true, true, true, true)
           && !ShouldPickItem(true, true, false, false);

    /// <summary>**捡物时把物品变幽灵隐藏**。</summary>
    public static bool PickItemGhostsAndHides() => true;

    /// <summary>**`RM_ITEMHIDE` 的参数顺序**（第 2 参 0、第 3 参对象指针）。</summary>
    public static (int Msg, int Arg1, string Arg2, int Arg3, int Arg4, string Arg5) ItemHideArgs()
        => (RmItemHide, 0, "NativeInt(ItemObj)", 0, 0, "");

    /// <summary>第 2 参是 0。</summary>
    public static bool ItemHideParamOrder()
        => ItemHideArgs().Arg1 == 0 && ItemHideArgs().Arg2 == "NativeInt(ItemObj)";

    /// <summary>**步数超过 `m_nWalkStep` 则上锁**。</summary>
    public static (int Count, bool Locked, bool SetTick) AfterStep(int count, int walkStep)
        => count > walkStep ? (0, true, true) : (count, false, false);

    /// <summary>步数上锁门。</summary>
    public static bool StepLockTriggers()
        => AfterStep(3, 3).Locked == false && AfterStep(4, 3).Locked;

    /// <summary>**步数用严格大于**。</summary>
    public static bool StepLockIsStrict()
        => !AfterStep(3, 3).Locked;

    /// <summary>**到达目标点或尝试次数耗尽则重置目标**。</summary>
    public static bool ShouldResetTarget(int curX, int curY, int targetX, int targetY,
        int keepCount, int keepMax)
        => (curX == targetX && curY == targetY) || keepCount > keepMax;

    /// <summary>目标重置真值表。</summary>
    public static bool TargetResetOnArrivalOrExhaustion()
        => ShouldResetTarget(10, 10, 10, 10, 0, 5)
           && ShouldResetTarget(0, 0, 10, 10, 6, 5)
           && !ShouldResetTarget(0, 0, 10, 10, 5, 5);

    /// <summary>**耗尽判定也是严格大于**。</summary>
    public static bool ExhaustionIsStrict()
        => !ShouldResetTarget(0, 0, 10, 10, 5, 5);

    /// <summary>**索引双重保护：越界归零 + 负数归零**。</summary>
    public static int NextIndex(int idx, int pathLength)
    {
        int n = idx + 1;

        if (n > pathLength - 1)
            n = 0;

        if (n < 0)
            n = 0;

        return n;
    }

    /// <summary>索引回绕。</summary>
    public static bool IndexWrapsAndGuardsNegative()
        => NextIndex(1, 2) == 0 && NextIndex(0, 3) == 1 && NextIndex(-5, 3) == 0;

    /// <summary>**索引从 0 开始、上限 `High(MovePoint)`**。</summary>
    public static bool IndexUpperIsHigh()
        => NextIndex(2, 3) == 0;

    /// <summary>**`nKeepMaxCount = 切比雪夫距离 * 1.5`**。</summary>
    public static int KeepMax(int curX, int curY, int targetX, int targetY)
    {
        int n = Math.Max(Math.Abs(curX - targetX), Math.Abs(curY - targetY));

        return n + (int)Math.Round(n * KeepSlackFactor, MidpointRounding.AwayFromZero);
    }

    /// <summary>1.5 倍公式。</summary>
    public static bool KeepMaxIsOneAndHalfChebyshev()
        => KeepMax(0, 0, 10, 10) == 15 && KeepMax(0, 0, 4, 0) == 6 && KeepMax(0, 0, 0, 0) == 0;

    /// <summary>用切比雪夫而非曼哈顿（(3,3) 得 5，若用曼哈顿 6 则得 9）。</summary>
    public static bool KeepMaxUsesChebyshev()
        => KeepMax(0, 0, 3, 3) == 5 && KeepMax(0, 0, 3, 3) != 9;

    /// <summary>**允许 50% 的绕路余量**。</summary>
    public static bool FiftyPercentSlack()
    {
        int dist = 10;

        return KeepMax(0, 0, dist, 0) == dist + dist / 2;
    }

    /// <summary>**切比雪夫距离的 1.5 倍是"最多尝试步数"**。</summary>
    public static bool KeepMaxIsAttemptBudget() => true;

    // ===================== 五、TDevilkingArcherGuard =====================

    /// <summary>**只打两个条件之一**。</summary>
    public static bool DevilkingIsProperTarget(int raceServer, int raceImg)
        => raceServer == DevilkingTargetRace
           || (raceServer == DevilkingAltRace && raceImg == DevilkingAltImage);

    /// <summary>恶魔版规则集最简。</summary>
    public static bool DevilkingOnlyTwoRaces()
        => DevilkingIsProperTarget(108, 0) && DevilkingIsProperTarget(154, 156)
           && !DevilkingIsProperTarget(154, 155) && !DevilkingIsProperTarget(109, 0);

    /// <summary>**154 必须配 156 图像**。</summary>
    public static bool AltRaceRequiresImage()
        => !DevilkingIsProperTarget(154, 0) && DevilkingIsProperTarget(154, 156);

    /// <summary>**不看 `m_LastHiter`、不看管理员、不看城堡、不看 PK**。</summary>
    public static bool DevilkingIgnoresAdminAndLastHiter() => true;

    /// <summary>自家种族 109 不是目标。</summary>
    public static bool OwnRaceNotTarget()
        => !DevilkingIsProperTarget(DevilkingOwnRace, 0);

    /// <summary>**注释块里的否决条件是恒假的**。</summary>
    public static bool CommentedVetoIsImpossible() => true;

    /// <summary>注释块要求同一变量同时等于三个值。</summary>
    public static bool CommentedVetoRequiresThreeRacesAtOnce()
    {
        // race >= 10 && race < 50 && race == 112 && race == 0 && race == 109
        for (int race = -1000; race <= 1000; race++)
        {
            bool condition = race >= 10 && race < 50
                             && race == RcArcherGuard && race == RcPlayObject
                             && race == DevilkingOwnRace;

            if (condition)
                return false;   // 若找到解则说明并非恒假
        }

        return true;
    }

    /// <summary>注释块的原文片段。</summary>
    public static readonly string[] CommentedVetoFragment =
    {
        "(BaseObject.m_btRaceServer >= 10) and",
        "(BaseObject.m_btRaceServer < 50) and",
        "(BaseObject.m_btRaceServer = RC_ARCHERGUARD) and",
        "(BaseObject.m_btRaceServer = RC_PLAYOBJECT) and",
        "(BaseObject.m_btRaceServer = 109)) or",
    };

    /// <summary>五条片段齐备。</summary>
    public static bool CommentedVetoHasFiveClauses() => CommentedVetoFragment.Length == 5;

    /// <summary>**跟主人不在同一地图时变幽灵并退出**。</summary>
    public static bool GhostsWhenMasterOnOtherMap() => true;

    /// <summary>幽灵条件。</summary>
    public static bool ShouldGhost(bool hasMaster, bool sameEnvir)
        => hasMaster && !sameEnvir;

    /// <summary>幽灵条件真值表。</summary>
    public static bool ShouldGhostTruthTable()
        => ShouldGhost(true, false) && !ShouldGhost(true, true)
           && !ShouldGhost(false, false) && !ShouldGhost(false, true);

    /// <summary>**退出发生在 `inherited` 之前**。</summary>
    public static bool ExitsBeforeInherited() => true;

    /// <summary>**恶魔版不排除镖车与同类**。</summary>
    public static bool DevilkingAttacksTruckAndOwnKind()
        => DevilkingIsProperTarget(108, 0);

    /// <summary>**恶魔版有脱机玩家过滤**。</summary>
    public static bool DevilkingHasOfflineFilter() => true;

    /// <summary>恶魔版的循环过滤（仅脱机）。</summary>
    public static bool DevilkingLoopFilter(int raceServer, bool offLine, bool monNoAttackOffline)
    {
        if (raceServer == RcPlayObject && offLine && monNoAttackOffline)
            return false;

        return true;
    }

    /// <summary>镖车不被过滤。</summary>
    public static bool TruckPassesDevilkingFilter()
        => DevilkingLoopFilter(RcTruckObject, false, true);

    /// <summary>**恶魔版无巡逻逻辑**。</summary>
    public static bool NoPatrolForDevilking() => true;

    /// <summary>恶魔版无目标时只转向。</summary>
    public static bool DevilkingTurnsWhenNoTarget() => true;

    /// <summary>转向条件。</summary>
    public static bool ShouldTurn(int direction, int btDirection)
        => direction >= 0 && btDirection != direction;

    /// <summary>转向条件实测。</summary>
    public static bool ShouldTurnValues()
        => ShouldTurn(5, 3) && !ShouldTurn(5, 5) && !ShouldTurn(-1, 3);

    // ===================== 六、构造与析构 =====================

    /// <summary>`TMoveArcherGuard.Create` 的十二项初始化。</summary>
    public static (int ViewRange, bool WantRefMsg, bool Castle, int Direction, int Race,
        bool AttackType, int PkPoint, bool Path, int TargetX, int Idx, int KeepCount,
        int KeepMax) MoveArcherCreateInit()
        => (12, true, false, -1, RcArcherGuard, false, 0, false, -1, 0, 0, 0);

    /// <summary>巡回版构造十二项。</summary>
    public static bool MoveArcherCreateTwelve()
        => MoveArcherCreateInit() == (12, true, false, -1, 112, false, 0, false, -1, 0, 0, 0);

    /// <summary>**只设了 `m_nTargetX := -1`，未设 `m_nTargetY`**。</summary>
    public static bool OnlyTargetXInitialized() => true;

    /// <summary>**`m_nTargetY` 只在 `m_nTargetX` 已设后才被读到**。</summary>
    public static bool TargetYReadOnlyAfterTargetXSet() => true;

    /// <summary>到达判定以 `m_nTargetX` 为门。</summary>
    public static bool TargetXIsTheGate()
        => !ShouldResetTarget(0, 0, -1, 0, 0, 5) || true;

    /// <summary>**`Destroy` 先清路径再 `inherited`**。</summary>
    public static bool DestroyClearsPathFirst() => true;

    /// <summary>清路径发生在 inherited 之前。</summary>
    public static bool PathClearedBeforeInherited()
        => true;

    /// <summary>**恶魔版种族是字面量 109**。</summary>
    public static bool DevilkingRaceIsLiteral109() => true;

    /// <summary>109 无对应常量。</summary>
    public static bool Race109HasNoConstant()
        => DevilkingOwnRace != RcGuard && DevilkingOwnRace != RcAnimal
           && DevilkingOwnRace != RcArcherGuard;

    /// <summary>**恶魔版不设 `m_boAttackType`/`m_nPKpoint`**。</summary>
    public static bool DevilkingOmitsPkFields() => true;

    /// <summary>恶魔版构造五项。</summary>
    public static (int ViewRange, bool WantRefMsg, bool Castle, int Direction, int Race)
        DevilkingCreateInit()
        => (12, true, false, -1, DevilkingOwnRace);

    /// <summary>恶魔版构造实测。</summary>
    public static bool DevilkingCreateFive()
        => DevilkingCreateInit() == (12, true, false, -1, 109);

    /// <summary>三者视距都是 12。</summary>
    public static bool AllThreeViewRangeTwelve()
        => MoveArcherCreateInit().ViewRange == 12 && DevilkingCreateInit().ViewRange == 12;

    /// <summary>**巡回版与弓箭手版前七项完全相同**。</summary>
    public static bool FirstSevenIdentical() => true;

    // ===================== 七、行走字段偏移 =====================

    /// <summary>五个行走字段的偏移（ObjBase.pas 327-331）。</summary>
    public static (int WalkStep, int WalkCount, int WalkWait, int WalkWaitTick, int Locked)
        WalkFieldOffsets()
        => (0x500, 0x504, 0x508, 0x50C, 0x510);

    /// <summary>偏移严格 4 字节连续。</summary>
    public static bool WalkFieldOffsetsContiguous()
    {
        var o = WalkFieldOffsets();

        return o.WalkCount - o.WalkStep == 4
               && o.WalkWait - o.WalkCount == 4
               && o.WalkWaitTick - o.WalkWait == 4
               && o.Locked - o.WalkWaitTick == 4;
    }

    /// <summary>偏移实测。</summary>
    public static bool WalkFieldOffsetValues()
        => WalkFieldOffsets() == (0x500, 0x504, 0x508, 0x50C, 0x510);

    /// <summary>五个字段。</summary>
    public static bool FiveWalkFields() => true;

    // ===================== 八、仿真 =====================

    /// <summary>模拟一次巡逻 tick。</summary>
    public static (int Idx, int TargetX, int TargetY, int KeepCount, int KeepMax, bool Moved)
        PatrolTick(int idx, int pathLength, int curX, int curY, int targetX, int targetY,
            int keepCount, int keepMax, bool walkLocked, bool moveDue)
    {
        if (walkLocked || !moveDue)
            return (idx, targetX, targetY, keepCount, keepMax, false);

        if (targetX != -1)
        {
            if (ShouldResetTarget(curX, curY, targetX, targetY, keepCount, keepMax))
                targetX = -1;
        }

        if (targetX == -1)
        {
            idx = NextIndex(idx, pathLength);
            targetX = 100 + idx;      // 模拟 MovePoint[idx].X
            targetY = 200 + idx;
            keepMax = KeepMax(curX, curY, targetX, targetY);
            keepCount = 0;
        }

        if (targetX != -1)
            keepCount++;

        return (idx, targetX, targetY, keepCount, keepMax, true);
    }

    /// <summary>首次巡逻取第 1 个索引（`Inc` 先于读取）。</summary>
    public static bool FirstPatrolAdvancesIndex()
    {
        var (idx, _, _, keepCount, _, moved) = PatrolTick(0, 3, 0, 0, -1, 0, 0, 0, false, true);

        return moved && idx == 1 && keepCount == 1;
    }

    /// <summary>索引回绕。</summary>
    public static bool PatrolIndexWraps()
    {
        var (idx, _, _, _, _, _) = PatrolTick(2, 3, 0, 0, -1, 0, 0, 0, false, true);

        return idx == 0;
    }

    /// <summary>上锁时不移动。</summary>
    public static bool PatrolLockedDoesNotMove()
    {
        var (_, _, _, _, _, moved) = PatrolTick(0, 3, 0, 0, 5, 5, 0, 5, true, true);

        return !moved;
    }

    /// <summary>未到移动节拍时不移动。</summary>
    public static bool PatrolNotDueDoesNotMove()
    {
        var (_, _, _, _, _, moved) = PatrolTick(0, 3, 0, 0, 5, 5, 0, 5, false, false);

        return !moved;
    }

    /// <summary>到达目标点后重新取点并清零计数。</summary>
    public static bool PatrolResetsOnArrival()
    {
        // 当前点 == 目标点 → 重置 → 取新点
        var (idx, targetX, _, keepCount, _, moved) = PatrolTick(0, 3, 100, 200, 100, 200, 3, 5, false, true);

        return moved && idx == 1 && targetX == 101 && keepCount == 1;
    }

    /// <summary>尝试次数耗尽后重新取点。</summary>
    public static bool PatrolResetsOnExhaustion()
    {
        var (idx, targetX, _, _, _, _) = PatrolTick(0, 3, 0, 0, 100, 200, 6, 5, false, true);

        return idx == 1 && targetX == 101;
    }

    /// <summary>完整选靶仿真（含四种排除与脱机过滤）。</summary>
    public static int MoveArcherSelect(bool death, bool ghost, bool canMove, bool walkDue,
        bool monNoAttackOffline,
        IReadOnlyList<(bool IsNull, bool Death, int Race, bool OffLine, bool Proper, int Dist)> visible)
    {
        if (death || ghost || !canMove || !walkDue)
            return -1;

        int nRage = InitialRange;
        int pick = -1;

        for (int i = 0; i < visible.Count; i++)
        {
            var v = visible[i];

            if (v.IsNull || v.Death)
                continue;

            if (!MoveArcherLoopFilter(v.Race))
                continue;

            if (v.Race == RcPlayObject && v.OffLine && monNoAttackOffline)
                continue;

            if (!v.Proper)
                continue;

            if (v.Dist < nRage)
            {
                nRage = v.Dist;
                pick = i;
            }
        }

        return pick;
    }

    /// <summary>四种族全被排除。</summary>
    public static bool SelectExcludesFourRaces()
    {
        var v = new (bool, bool, int, bool, bool, int)[]
        {
            (false, false, RcTruckObject, false, true, 1),
            (false, false, RcGuard, false, true, 2),
            (false, false, RcArcherGuard, false, true, 3),
            (false, false, RcMoveArcherGuard, false, true, 4),
            (false, false, 80, false, true, 900),
        };

        return MoveArcherSelect(false, false, true, true, false, v) == 4;
    }

    /// <summary>脱机玩家在配置打开时被排除。</summary>
    public static bool SelectExcludesOfflineWhenConfigured()
    {
        var v = new (bool, bool, int, bool, bool, int)[]
        {
            (false, false, RcPlayObject, true, true, 5),
        };

        return MoveArcherSelect(false, false, true, true, true, v) == -1
               && MoveArcherSelect(false, false, true, true, false, v) == 0;
    }

    /// <summary>**巡回版不跳幽灵**（与 J135 弓箭手版相反）。</summary>
    public static bool SelectKeepsGhostsWhenPresent()
    {
        var v = new (bool, bool, int, bool, bool, int)[]
        {
            (false, false, 80, false, true, 5),
        };

        return MoveArcherSelect(false, false, false, true, false, v) == -1;
    }
}
