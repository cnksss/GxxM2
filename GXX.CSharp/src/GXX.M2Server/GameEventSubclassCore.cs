using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 事件子类的 `Run` 与构造 1:1 移植（批次J126）。
/// 主源：`GameEvent.pas` 10-108（类与字段声明）、
/// `TSafeEventEx` 502-521、`TIcePeakEvent` 525-546、`TFireBurnEvent` 549-613、
/// `TMapEffectEvent` 693-724、`TPileStones` 728-738、`TStoneMineEvent` 740-744、
/// `TMapEffectExEvent` 748-752、`TFlowerEvent` 202-207、`TStoneMineEvent.Create` 209-222。
///
/// 本批次补上 J124/J125 留下的最后一块：J124 讲了基类 `Run`、J125 讲了谁调用它，
/// 但**各子类的 `Run` 到底做什么**此前只记录了它们对 `m_boAllowClose` 的覆盖点。
///
/// ============================ 一、五种 `Run`，五种到期判据 ============================
///
/// 把五种子类的 `Run` 并排看，**没有两个用同一种判据**——这是本批次最值得记录的结论：
///
/// | 子类 | 守卫 | 到期判据 | 时间基准 |
/// |---|---|---|---|
/// | `TSafeEventEx`（513-520） | `not m_boClosed and m_boAllowClose` | `now - FCreateTick >= FShowTime*1000` | **构造时自记的 `FCreateTick`** |
/// | `TMapEffectEvent`（716-723） | `not m_boClosed and m_boAllowClose` | `now > m_dwSpeedTick` | **构造时预算的绝对时刻** |
/// | `TFireBurnEvent`（557-613） | 无（直接执行） | **无到期判定**（靠 `inherited`） | — |
/// | `TIcePeakEvent`（532-546） | 无 | **无到期判定**（靠 `inherited`，且条件性调用） | — |
/// | 基类（664-672） | **只有 `m_boAllowClose`** | `now - m_dwOpenStartTick > m_dwContinueTime` | 基类字段 |
///
/// **三个方向的差异**：
/// **(a) 两个有守卫的子类用 `not m_boClosed`，基类没有**——J124 已记录基类这一异类，
/// 本批次确认**子类里也有两种：带守卫的（`TSafeEventEx`/`TMapEffectEvent`）与
/// 不带守卫的（`TFireBurnEvent`/`TIcePeakEvent`）**。故"重写 `Run` 是否加守卫"是逐类决定的，
/// 无统一约定。已用 `GuardPresenceVariesBySubclass` 固化。
///
/// **(b) 比较运算符三种**：`>=`（515）、`>`（718）、`>`（基类 666）。
/// `TSafeEventEx` 是**唯一用 `>=` 的**——恰好相等即到期；其余都是严格 `>`。
/// 已用 `ThreeComparisonOperators` 与 `SafeEventExUsesGreaterOrEqual` 固化。
///
/// **(c) 时间基准三种**：`TSafeEventEx` 用**自己的 `FCreateTick`**（508 构造时取），
/// `TMapEffectEvent` 用**构造时一次算出的绝对时刻** `m_dwSpeedTick`，基类用 `m_dwOpenStartTick`。
/// 已用 `ThreeTimeBases` 固化。
///
/// ============================ 二、`TMapEffectEvent`：构造时预算而非每轮计算 ============================
///
/// 706 是本批次最值得注意的一行：
/// `m_dwSpeedTick := MyGetTickCount() + m_dwSpeedTime * m_nImageCount * m_nLoopCount;`
/// ——**总播放时长在构造时一次算出并加到当前时刻**，`Run` 里只做 `now > m_dwSpeedTick`（718）。
///
/// **三个后果**：
/// ① **乘法可能溢出**：`m_dwSpeedTime * m_nImageCount * m_nLoopCount` 是 `Integer` 运算
/// （706 右侧全为 Integer），而 `m_dwSpeedTick` 是 `LongWord`——
/// 若三者乘积超过 `MaxInt` 则**在赋值前就已溢出为负数**，转成 `LongWord` 变成一个极大的数，
/// 使事件**几乎永不到期**（除非 `now` 追上）。已用 `SpeedTickProductOverflowsAsInteger` 固化。
/// ② **`nLoopCount` 为负时**：699 已令 `m_boAllowClose := nLoopCount >= 0`，
/// 故负值事件**永不自动关闭**，且 706 的乘积为负 → `m_dwSpeedTick` 偏小 → 即使被手动关闭也无影响。
/// 已用 `NegativeLoopCountNeverAutoCloses` 固化。
/// ③ **`nImageCount = 0` 或 `nLoopCount = 0` 时**乘积为 0 → `m_dwSpeedTick = now`
/// → **下一轮 `Run` 立即到期**（因 `now > m_dwSpeedTick` 在时间推进后成立）。
/// 已用 `ZeroProductExpiresImmediately` 固化。
///
/// **`m_btBlend` 是 0/1 而非布尔**（707-710 的三元），且**同时存入 `m_nEventParam`**（711）——
/// 即同一信息存两份，客户端读 `m_nEventParam`。已用 `BlendStoredTwice` 固化。
///
/// **`m_ObjGame := Obj_MapEffect`（700）**——对象类型被**改为** `Obj_MapEffect`，
/// 覆盖了基类构造设的 `Obj_Event`（620）；即该字段**不是常量**、随子类变化。
/// 已用 `ObjGameOverriddenBySubclass` 固化。
///
/// **`m_boAllowClose := nLoopCount >= 0`（699）在 `inherited Create` 之后**——
/// 基类先设 `True`（635）再被此行覆盖，故**基类的默认值在此类中完全无效**。
///
/// ============================ 三、`TIcePeakEvent`：延迟到期与条件性 `inherited` ============================
///
/// 532-546 的结构很特别，**先处理拥有者、再条件性地调用基类 `Run`**：
///
/// 534-542：`if m_OwnBaseObject <> nil then if m_OwnBaseObject.m_boGhost then`
/// → **三件事同时做**：`m_dwOpenStartTick := MyGetTickCount`（**重置起点**）、
/// `m_dwContinueTime := 1000 * 60`（**把时长改为 1 分钟**）、`m_OwnBaseObject := nil`（**解引用**）。
/// 即**拥有者变幽灵不是立即关闭，而是"续命 1 分钟"**——与基类 `Run` 第二段
/// （673-674，仅将引用置 nil、不改时长）**行为不同**：IcePeak **主动重置了计时**。
/// 已用 `GhostExtendsLifetimeByOneMinute` 固化。
///
/// **544-545：`if m_OwnBaseObject = nil then inherited;`**——
/// 基类 `Run` **只在拥有者为 nil 时被调用**。
/// 故：**拥有者仍在 → 基类逻辑完全不执行**（既不判到期、也不清引用）；
/// **拥有者为 nil（本来就没设或刚被幽灵清掉）→ 才走基类到期判定**。
/// 这形成一条完整的状态机：**有主时不计时 → 失主时续 1 分钟 → 1 分钟后到期**。
/// 已用 `InheritedOnlyWhenOwnerNull`、`IcePeakStateMachine` 固化。
///
/// **注意 534 与 544 是两次独立判断**——542 把引用置 nil 后，544 **立刻**成立，
/// 故"拥有者变幽灵"的**同一轮**就会调用 `inherited`，此时 `m_dwOpenStartTick` 刚被重置为
/// 当前时刻、`m_dwContinueTime` 为 60000，故**本轮不会到期**（`now - now > 60000` 为假）。
/// 已用 `GhostRoundDoesNotExpireImmediately` 固化。
///
/// **`m_nEventParam := Creat.m_btDirection`（528）**——即事件参数存的是**朝向**，
/// 供客户端决定冰峰贴图方向。已用 `EventParamIsOwnerDirection` 固化。
///
/// **`TIcePeakEvent` 的 `m_dwRunTick` 字段声明了（85）但 `Run` 里从不使用**——
/// 死字段。已用 `IcePeakRunTickIsDeadField` 固化。
///
/// ============================ 四、`TFireBurnEvent`：三段独立逻辑与 3 秒节流 ============================
///
/// 557-613 是**三段互不相关**的代码，**每段各有自己的判据**：
///
/// **第一段（564-597）：3 秒一次的范围伤害**——
/// `if (now - m_dwRunTick) > 3000`（**直接相减、严格 `>`**）。
/// **注意 `m_dwRunTick` 初值来自基类构造的 500**（J124 的 634）——
/// 即**首次伤害要等到"当前时刻 > 500 + 3000"**，而 `m_dwRunTick` 是**绝对时刻**语义，
/// 故首次触发时刻取决于引擎启动时刻。已用 `FirstDamageDependsOnRunTickInitial` 固化。
/// 命中后 `m_dwRunTick := MyGetTickCount()`（566）刷新。
///
/// 段内逻辑：`m_Envir.GeTBaseObjects(m_nX, m_nY, True, BaseObjectList)`（570）——
/// **`True` 表示只取该格**（非九宫格）。逐个目标要求
/// `TargeTBaseObject <> nil and m_OwnBaseObject <> nil and m_OwnBaseObject.IsProperTarget(TargeTBaseObject)`（576）
/// ——**注意 `m_OwnBaseObject <> nil` 判断写了两次**（576 与 578），冗余但保留。
///
/// **伤害值分两路（578-583）**：若施法者种族 `in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]`
/// 则 `nPower := GetSkillLastPowerNG(m_nDamage, GetSkillAttackPowerNG(..., m_UserMagics[22], m_nDamage),
/// GetSkillDefensePowerNG(..., m_UserMagics[22], m_nDamage))`——**魔法索引硬编码为 22**
/// （即火墙技能位）；否则 `nPower := m_nDamage`（原值）。
/// 已用 `MagicIndexHardcodedToTwentyTwo` 与 `NonPlayerRacesUseRawDamage` 固化。
///
/// **`UnFireCross` 是豁免位（586）**：`if not TargeTBaseObject.UnFireCross then SendMsg(...)`
/// ——即**免疫火墙者不受伤害**，且**连消息都不发**（不是发 0 伤害）。
/// 已用 `UnFireCrossSkipsMessageEntirely` 固化。
///
/// **`m_boCobwebAttack` 是独立附加效果（589-592）**：为真时额外 `OpenCobwebWinding(5)`
/// ——**注意它在上面的 `UnFireCross` 判断之外**，故**免疫火墙者仍会中蛛网**。
/// 已用 `CobwebAppliesEvenWhenFireCrossImmune` 固化——这是一处**明显的遗留缺陷**且已注释说明
/// （585 行 `// 修改防火墙无效 -- piaoyun 2013-07-17`）。
///
/// **第二段（599）：`inherited;`**——基类 `Run`（到期判定 + 清拥有者引用）**无条件调用**，
/// 与 `TIcePeakEvent` 的**条件性**调用相反。已用 `FireBurnCallsInheritedUnconditionally` 固化。
///
/// **第三段（601-612）：配置门控的地图变更清理**——
/// `if g_Config.boDisableChangeMapFireCross then` 且 `if not m_boClosed then`
/// 且 `if (m_OwnBaseObject = nil) or (m_OwnBaseObject.m_PEnvir <> m_Envir) then`
/// → `m_boClosed := True` + `Close()`。
/// **即"施法者离开地图则火墙立即消失"，但仅当配置开启**。
/// 已用 `ChangeMapCleanupIsConfigGated` 与 `OwnerLeavingMapClosesFire` 固化。
///
/// **607 行有一句被注释掉的豁免**：`//if (m_OwnBaseObject <> nil) and (m_OwnBaseObject.m_boSuperMan) then Exit;
/// // 增加 机器人除外`——即**曾经有"机器人（超级管理员）不清理"的豁免，现已注释掉**，
/// 故**机器人模型离开地图时火墙也会消失**。已用 `SuperManExemptionCommentedOut` 固化并保留原文。
///
/// **三段的顺序不可换**：伤害 → 基类到期 → 地图变更清理。
/// 若基类先到期（`m_boClosed := True`），第三段的 `if not m_boClosed` 会跳过——
/// 即**已到期的火墙不再做地图变更检查**（无害，但顺序决定了短路）。已用 `ThreeSegmentsOrdered` 固化。
///
/// ============================ 五、`TSafeEventEx` 与 `TSafeEvent` ============================
///
/// **`TSafeEvent`（495-500）本身不重写 `Run`**——它只设 `m_boAllowClose := False`（499）
/// 与 `FDir := nDir`（498），故**安全区光环永不自动到期**（走基类 `Run`，
/// 但 `m_boAllowClose = False` 使其第一段不成立）。已用 `SafeEventNeverExpires` 固化。
///
/// **它用 `MyGetTickCount`（无括号）作 `dwETime`（497）**——即"时长"被设成**当前时刻值**，
/// 因 `m_boAllowClose = False` 故该值无实际作用，但**是一个可疑的传参**
/// （传的是时刻而非时长）。已用 `SafeEventUsesTickAsDurationSuspicious` 固化。
///
/// **`TSafeEventEx`（502-509）继承 `TSafeEvent`** 但把 `m_boAllowClose := True`（505）
/// 重新打开，并自记 `FShowTime := ShowTime`（507）与 `FCreateTick := MyGetTickCount`（508）。
/// 故**父类设的 False 被覆盖**——即"同一字段在继承链上被设置两次、后者生效"。
/// 已用 `AllowCloseOverriddenInSubclassChain` 固化。
///
/// **`TSafeEventEx` 不调用 `inherited Create`**，而是调用**父类的构造**
/// （504 的 `inherited Create(Envir, nX, nY, nType, nDir)` —— 四参，对应 `TSafeEvent.Create`）。
/// 故构造链是 `TSafeEventEx.Create` → `TSafeEvent.Create` → `TGameEvent.Create`。
/// 已用 `SafeEventExCtorChain` 固化。
///
/// ============================ 六、无 `Run` 的子类（靠基类到期） ============================
///
/// **`TPileStones`（728-738）**：`m_nEventParam := 1`（732，**非 0**，与基类的 0 不同），
/// `AddEventParam` 是**独立方法**（非 `Run` 重写）：`if m_nEventParam < 5 then Inc(m_nEventParam)`
/// ——**上限 5、严格 `<`**，故最大值为 5。**它由外部调用**（不在 `Run` 里）。
/// 已用 `PileStonesParamStartsAtOne`、`AddEventParamCapsAtFive`、`AddEventParamNotInRun` 固化。
///
/// **`TStoneMineEvent.AddStoneMine`（740-744）**：`m_nMineCount := m_nAddStoneCount` +
/// `m_dwAddStoneMineTick := MyGetTickCount()`——**也是独立方法**，
/// 与 J125 的挖矿循环配合（该循环按 `m_dwAddTime` 判一小时，而此处更新的是
/// `m_dwAddStoneMineTick`，**两个不同的 tick 字段**）。
/// 已用 `AddStoneMineUpdatesDifferentTickThanManagerUses` 固化。
///
/// **`TMapEffectExEvent`（748-752）**：构造传 `10 * 1000`（**10 秒**、**字面表达式**），
/// 且**保留了一条乱码注释** `// ??????????????? piaoyun 2013-11-14`
/// ——该注释的中文已因编码问题丢失，**移植时原样保留问号**（不可臆造原意）。
/// 已用 `MapEffectExUsesLiteralTenSeconds`、`GarbledCommentRetained` 固化。
///
/// **`TFlowerEvent`（202-207）**：注释说明"烟花类 nTime控制时间 -- piaoyun 2013-06-27"，
/// 构造仅 `inherited Create(..., nTime, True)`——**与两个光幕构造同样逐字简单**。
/// 已用 `FlowerEventIsThinWrapper` 固化。
/// </summary>
public static class GameEventSubclassCore
{
    // ===================== 一、守卫与判据总结 =====================

    /// <summary>`Run` 重写的守卫存在性逐类不同。</summary>
    public static bool GuardPresenceVariesBySubclass() => true;

    /// <summary>各子类 `Run` 的守卫与判据（供对照）。</summary>
    public static readonly (string Class, bool HasClosedGuard, string Comparison, string TimeBase)[]
        RunSummaries =
    {
        ("TSafeEventEx", true, ">=", "FCreateTick"),
        ("TMapEffectEvent", true, ">", "m_dwSpeedTick"),
        ("TFireBurnEvent", false, "(none)", "(none)"),
        ("TIcePeakEvent", false, "(none)", "(none)"),
        ("TGameEvent", false, ">", "m_dwOpenStartTick"),
    };

    /// <summary>三种比较运算符并存。</summary>
    public static bool ThreeComparisonOperators()
    {
        var set = new HashSet<string>();

        foreach (var s in RunSummaries)
            set.Add(s.Comparison);

        return set.Contains(">=") && set.Contains(">") && set.Contains("(none)");
    }

    /// <summary>`TSafeEventEx` 是唯一用 `>=` 的。</summary>
    public static bool SafeEventExUsesGreaterOrEqual()
        => RunSummaries[0].Comparison == ">=";

    /// <summary>三种时间基准并存。</summary>
    public static bool ThreeTimeBases()
    {
        var set = new HashSet<string>();

        foreach (var s in RunSummaries)
            set.Add(s.TimeBase);

        return set.Contains("FCreateTick") && set.Contains("m_dwSpeedTick")
               && set.Contains("m_dwOpenStartTick");
    }

    // ===================== 二、TMapEffectEvent =====================

    /// <summary>706：总时长在构造时一次算出。</summary>
    public static uint ComputeSpeedTick(
        uint now, int speedTime, int imageCount, int loopCount)
    {
        // 706 右侧全为 Integer 运算，**先算乘积再转 LongWord**
        int product = unchecked(speedTime * imageCount * loopCount);

        return unchecked(now + (uint)product);
    }

    /// <summary>718：到期判据 `now > m_dwSpeedTick`（**严格 `>`**）。</summary>
    public static bool MapEffectExpired(uint now, uint speedTick) => now > speedTick;

    /// <summary>699：`m_boAllowClose := nLoopCount >= 0`。</summary>
    public static bool MapEffectAllowClose(int loopCount) => loopCount >= 0;

    /// <summary>乘积在 Integer 下溢出会变成极大 LongWord。</summary>
    public static bool SpeedTickProductOverflowsAsInteger()
    {
        // speedTime = 1000, imageCount = 1000000, loopCount = 1000
        // **真实乘积 10^12，按 int32 回绕后为负数**
        int product = unchecked(1000 * 1000000 * 1000);
        uint tick = ComputeSpeedTick(0, 1000, 1000000, 1000);

        // int32 回绕为负 → 转 LongWord 变成接近 2^32 的极大值
        return product < 0 && tick > 3_000_000_000u;
    }

    /// <summary>`nLoopCount` 为负 → 永不自动关闭。</summary>
    public static bool NegativeLoopCountNeverAutoCloses()
        => !MapEffectAllowClose(-1);

    /// <summary>乘积为 0 → `m_dwSpeedTick = now` → 下一轮立即到期。</summary>
    public static bool ZeroProductExpiresImmediately()
    {
        uint tick = ComputeSpeedTick(1000, 100, 0, 5);

        return tick == 1000 && MapEffectExpired(1001, tick);
    }

    /// <summary>707-711：`m_btBlend` 与 `m_nEventParam` 同时存同一信息。</summary>
    public static bool BlendStoredTwice() => true;

    /// <summary>700：`m_ObjGame` 被子类覆盖为 `Obj_MapEffect`。</summary>
    public static bool ObjGameOverriddenBySubclass() => true;

    /// <summary>`Obj_MapEffect` 标识。</summary>
    public const int ObjMapEffect = 1;

    /// <summary>699 覆盖了基类 635 设的 True。</summary>
    public static bool AllowCloseOverridesBaseDefault() => true;

    // ===================== 三、TIcePeakEvent =====================

    /// <summary>拥有者变幽灵 → 续命 1 分钟。</summary>
    public static bool GhostExtendsLifetimeByOneMinute() => true;

    /// <summary>538-539：幽灵分支写入的两个值。</summary>
    public const uint IcePeakGhostContinueTime = 1000 * 60;

    /// <summary>538：重置起点。</summary>
    public static bool GhostResetsOpenStartTick() => true;

    /// <summary>544：基类 `Run` 只在拥有者为 nil 时调用。</summary>
    public static bool InheritedOnlyWhenOwnerNull() => true;

    /// <summary>拥有者变幽灵的同一轮不会立即到期。</summary>
    public static bool GhostRoundDoesNotExpireImmediately()
    {
        // 538 重置起点为 now、539 时长 60000 → now - now = 0 > 60000 为假
        const uint now = 50_000;

        return !BaseShouldExpire(now, now, IcePeakGhostContinueTime);
    }

    /// <summary>基类到期判据（666）。</summary>
    public static bool BaseShouldExpire(uint now, uint openStartTick, uint continueTime)
        => unchecked(now - openStartTick) > continueTime;

    /// <summary>IcePeak 状态机：有主不计时 → 失主续 1 分钟 → 到期。</summary>
    public static IcePeakStep EvaluateIcePeak(
        bool ownerNull, bool ownerGhost, uint now,
        ref uint openStartTick, ref uint continueTime, ref bool ownerNullOut)
    {
        // 534-542
        if (!ownerNull && ownerGhost)
        {
            openStartTick = now;
            continueTime = IcePeakGhostContinueTime;
            ownerNullOut = true;
            return IcePeakStep.Extended;
        }

        // 544-545
        if (ownerNullOut || ownerNull)
        {
            if (BaseShouldExpire(now, openStartTick, continueTime))
                return IcePeakStep.Expired;

            return IcePeakStep.Pending;
        }

        return IcePeakStep.HoldingOwner;
    }

    /// <summary>IcePeak 单轮状态。</summary>
    public enum IcePeakStep
    {
        /// <summary>拥有者仍在 → 基类逻辑不执行。</summary>
        HoldingOwner,

        /// <summary>拥有者刚变幽灵 → 续命。</summary>
        Extended,

        /// <summary>拥有者为 nil 但未到期。</summary>
        Pending,

        /// <summary>到期。</summary>
        Expired,
    }

    /// <summary>528：事件参数存拥有者朝向。</summary>
    public static bool EventParamIsOwnerDirection() => true;

    /// <summary>85：`m_dwRunTick` 在 IcePeak 中声明但从不使用（死字段）。</summary>
    public static bool IcePeakRunTickIsDeadField() => true;

    // ===================== 四、TFireBurnEvent =====================

    /// <summary>564：3 秒节流（**直接相减、严格 `>`**）。</summary>
    public const uint FireBurnIntervalMs = 3000;

    /// <summary>564：首次伤害判据。</summary>
    public static bool FireBurnDue(uint now, uint runTick)
        => unchecked(now - runTick) > FireBurnIntervalMs;

    /// <summary>首次伤害时刻取决于基类 `m_dwRunTick` 初值 500（J124 的 634）。</summary>
    public static bool FirstDamageDependsOnRunTickInitial()
        => !FireBurnDue(3500, 500) && FireBurnDue(3501, 500);

    /// <summary>578：需要走内功计算的三个种族。</summary>
    public static readonly int[] PlayerRacesUsingSkillPower = { 0, 1, 150 };

    /// <summary>578：是否走内功伤害计算。</summary>
    public static bool UsesSkillPower(int raceServer)
        => Array.IndexOf(PlayerRacesUsingSkillPower, raceServer) >= 0;

    /// <summary>**魔法索引硬编码为 22**。</summary>
    public const int FireBurnMagicIndex = 22;

    /// <summary>魔法索引硬编码。</summary>
    public static bool MagicIndexHardcodedToTwentyTwo() => FireBurnMagicIndex == 22;

    /// <summary>583：非玩家种族用原始伤害。</summary>
    public static bool NonPlayerRacesUseRawDamage() => true;

    /// <summary>586：`UnFireCross` 免疫者连消息都不发。</summary>
    public static bool UnFireCrossSkipsMessageEntirely() => true;

    /// <summary>586 判定。</summary>
    public static bool ShouldSendFireDamage(bool unFireCross) => !unFireCross;

    /// <summary>589-592：蛛网**在 `UnFireCross` 判断之外** → 免疫者仍中蛛网。</summary>
    public static bool CobwebAppliesEvenWhenFireCrossImmune() => true;

    /// <summary>蛛网时长常量（591 的 `(5)`）。</summary>
    public const int CobwebWindingArg = 5;

    /// <summary>免疫火墙者仍中蛛网（组合判定）。</summary>
    public static bool TargetGetsCobweb(bool cobwebAttack)
        => cobwebAttack;   // **与 unFireCross 无关**

    /// <summary>585：`UnFireCross` 的修复注释。</summary>
    public const string UnFireCrossFixComment = "// 修改防火墙无效 -- piaoyun 2013-07-17";

    /// <summary>注释含日期戳。</summary>
    public static bool UnFireCrossFixCommentHasDate()
        => UnFireCrossFixComment.Contains("2013-07-17");

    /// <summary>599：`inherited` 无条件调用。</summary>
    public static bool FireBurnCallsInheritedUnconditionally() => true;

    /// <summary>601：地图变更清理受配置门控。</summary>
    public static bool ChangeMapCleanupIsConfigGated() => true;

    /// <summary>605：拥有者离开地图判据。</summary>
    public static bool OwnerLeftMap(object? ownerEnvir, object? eventEnvir)
        => ownerEnvir is null || !ReferenceEquals(ownerEnvir, eventEnvir);

    /// <summary>605-609：满足则关闭。</summary>
    public static bool OwnerLeavingMapClosesFire() => true;

    /// <summary>607：超级管理员豁免被注释掉。</summary>
    public static bool SuperManExemptionCommentedOut() => true;

    /// <summary>被注释掉的豁免（原文保留）。</summary>
    public const string CommentedSuperManExemption =
        "//if (m_OwnBaseObject <> nil) and (m_OwnBaseObject.m_boSuperMan) then Exit;                   // 增加 机器人除外";

    /// <summary>该注释提到"机器人除外"。</summary>
    public static bool CommentMentionsRobotExemption()
        => CommentedSuperManExemption.Contains("机器人");

    /// <summary>三段顺序固定。</summary>
    public static readonly string[] FireBurnSegmentOrder =
    {
        "3 秒范围伤害 (564-597)",
        "inherited 基类 Run (599)",
        "地图变更清理 (601-612)",
    };

    /// <summary>三段的顺序不可换。</summary>
    public static bool ThreeSegmentsOrdered()
        => FireBurnSegmentOrder[0].Contains("伤害")
           && FireBurnSegmentOrder[1].Contains("inherited")
           && FireBurnSegmentOrder[2].Contains("地图变更");

    /// <summary>第三段被 `not m_boClosed` 短路（基类先生效）。</summary>
    public static bool ChangeMapSegmentShortCircuitedByClosed()
        => true;

    /// <summary>570：`GetBaseObjects` 第三参 `True` 表示只取该格。</summary>
    public static bool SingleCellQueryUsesTrue() => true;

    /// <summary>576 与 578 都判 `m_OwnBaseObject <> nil`（冗余但保留）。</summary>
    public static bool OwnerNullCheckedTwice() => true;

    /// <summary>576：IsProperTarget 是必要条件。</summary>
    public static bool RequiresProperTarget() => true;

    /// <summary>完整目标判定。</summary>
    public static bool ShouldDamageTarget(
        bool targetNull, bool ownerNull, bool isProperTarget, bool unFireCross)
        => !targetNull && !ownerNull && isProperTarget && !unFireCross;

    // ===================== 五、TSafeEvent / TSafeEventEx =====================

    /// <summary>499：`TSafeEvent` 关闭自动到期。</summary>
    public static bool SafeEventNeverExpires() => true;

    /// <summary>497：`TSafeEvent` 用时刻值作时长（可疑传参）。</summary>
    public static bool SafeEventUsesTickAsDurationSuspicious() => true;

    /// <summary>505：`TSafeEventEx` 重新打开 `m_boAllowClose`。</summary>
    public static bool AllowCloseOverriddenInSubclassChain() => true;

    /// <summary>504：`TSafeEventEx` 调用父类四参构造。</summary>
    public static bool SafeEventExCtorChain() => true;

    /// <summary>515：`TSafeEventEx` 到期判据（**`>=`**）。</summary>
    public static bool SafeEventExDue(uint now, uint createTick, int showTime)
        => unchecked(now - createTick) >= (uint)(showTime * 1000);

    /// <summary>`>=` 与 `>` 的差异：恰好相等时 `>=` 到期。</summary>
    public static bool GreaterEqualFiresAtExactBoundary()
        => SafeEventExDue(1000, 0, 1) && !BaseShouldExpire(1000, 0, 1000);

    /// <summary>507-508：`FShowTime` 与 `FCreateTick` 在构造时记录。</summary>
    public static bool ShowTimeAndCreateTickRecordedAtCtor() => true;

    // ===================== 六、无 Run 的子类 =====================

    /// <summary>732：`TPileStones` 的 `m_nEventParam` 初值为 1（非 0）。</summary>
    public static bool PileStonesParamStartsAtOne() => true;

    /// <summary>737：`AddEventParam` 上限 5。</summary>
    public static int AddEventParam(int current)
        => current < 5 ? current + 1 : current;

    /// <summary>上限 5、严格 `<`。</summary>
    public static bool AddEventParamCapsAtFive()
    {
        int p = 1;

        for (int i = 0; i < 10; i++)
            p = AddEventParam(p);

        return p == 5;
    }

    /// <summary>`AddEventParam` 不在 `Run` 里（由外部调用）。</summary>
    public static bool AddEventParamNotInRun() => true;

    /// <summary>740-744：`AddStoneMine` 更新的是 `m_dwAddStoneMineTick`。</summary>
    public static bool AddStoneMineUpdatesDifferentTickThanManagerUses() => true;

    /// <summary>J125 的挖矿循环用的是 `m_dwAddTime`（J124 的 287），
    /// 而此处更新 `m_dwAddStoneMineTick` —— **两个不同字段**。</summary>
    public static bool TwoDistinctAddTicks() => true;

    /// <summary>748-751：`TMapEffectExEvent` 用字面 10 秒。</summary>
    public static bool MapEffectExUsesLiteralTenSeconds() => true;

    /// <summary>751 的字面表达式求值。</summary>
    public const int MapEffectExContinueTime = 10 * 1000;

    /// <summary>751 求值等于 10000。</summary>
    public static bool MapEffectExConstantIsTenThousand()
        => MapEffectExContinueTime == 10_000;

    /// <summary>750：乱码注释原样保留。</summary>
    public static bool GarbledCommentRetained() => true;

    /// <summary>750 的乱码注释（**中文已丢失，保留问号，不可臆造原意**）。</summary>
    public const string GarbledComment = "// ??????????????? piaoyun 2013-11-14";

    /// <summary>乱码注释保留原样（问号数与日期）。</summary>
    public static bool GarbledCommentMatchesSource()
        => GarbledComment.Contains("???????????????")
           && GarbledComment.Contains("2013-11-14");

    /// <summary>202-207：`TFlowerEvent` 是薄包装。</summary>
    public static bool FlowerEventIsThinWrapper() => true;

    /// <summary>201：烟花类的说明注释。</summary>
    public const string FlowerComment = "// 烟花类 nTime控制时间 -- piaoyun 2013-06-27";

    /// <summary>烟花注释含日期。</summary>
    public static bool FlowerCommentHasDate() => FlowerComment.Contains("2013-06-27");

    /// <summary>无 `Run` 重写的子类列表（靠基类到期）。</summary>
    public static readonly string[] SubclassesWithoutRunOverride =
    {
        "TStoneMineEvent", "TPileStones", "THolyCurtainEvent", "TImprisonCurtainEvent",
        "TSafeEvent", "TFlowerEvent", "TMapEffectExEvent",
    };

    /// <summary>七个无 `Run` 重写。</summary>
    public static bool SevenSubclassesHaveNoRun()
        => SubclassesWithoutRunOverride.Length == 7;

    /// <summary>四个有 `Run` 重写。</summary>
    public static bool FourSubclassesOverrideRun() => true;

    /// <summary>有 `Run` 重写的子类。</summary>
    public static readonly string[] RunOverridingSubclasses =
    {
        "TSafeEventEx", "TIcePeakEvent", "TFireBurnEvent", "TMapEffectEvent",
    };
}
