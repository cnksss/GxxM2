using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图事件基类与禁锢光幕事件 1:1 移植（批次J124）。
/// 主源：`GameEvent.pas` 617-689（`TGameEvent.Create`/`Destroy`/`Run`/`Close`）、
/// 481-489（`THolyCurtainEvent.Create` 与 `TImprisonCurtainEvent.Create`——**两者逐字相同**）、
/// 54-57（类声明，注释标注 `// 0x40`）；
/// 常量源：`Grobal2.pas` 3116-3163（`ET_*`）。
///
/// 本批次补上 J123 留下的悬念：J123 在清理时调用 `TGameEvent(...).Close()`，
/// 但 `Close` 到底做什么、`m_boVisible`/`m_Envir`/`m_boClosed` 三个标志如何互动，
/// 以及**事件自己如何到期**（`Run`）此前都未移植。
///
/// ============================ 一、构造函数的两条分支 ============================
///
/// 617-642：`Create(tEnvir, nTX, nTY, nType, dwETime, boVisible)` 依次初始化 13 个字段，
/// **其中 `m_dwTickCount` 系列取自 `MyGetTickCount()` 两次**：
/// `m_dwOpenStartTick`（621）与 `m_dwRunStart`（633）**是两次独立调用**——
/// 中间隔着若干赋值，故两者**不保证相等**（相差通常极小但非零）。
/// 这与 J107/J119 的"三处取时刻"是同类现象：**同一构造里多次取当前时刻而不复用**。
/// 已用 `OpenStartTickAndRunStartAreSeparateCalls` 固化。
///
/// **末尾的两条分支（636-641）是"可见性由环境决定"**：
/// `if (m_Envir &lt;&gt; nil) and m_boVisible then AddToMap(...)`
/// `else m_boVisible := False;`
/// ——即**环境为 nil 时会把传入的 `boVisible` 强制改写为 `False`**。
/// 故 `boVisible = True` 但 `Envir = nil` 的事件**既不进地图、也不保持可见标志**，
/// 它是一个"半初始化"对象：字段看似可见、实际不可见。
/// 已用 `NullEnvirForcesInvisible` 固化。
///
/// **`m_dwRunTick := 500`**（634）是一个**字面常量初值**（非 0、非当前时刻）——
/// 它是"每 500ms 处理一次"的节流基数；已用 `RunTickInitialIsFiveHundred` 固化。
///
/// **`m_boAllowClose := True`**（635）——**基类默认允许关闭**；
/// 但 `TSafeEvent`（499）与 `TMapEffectEvent`（699）会把它改成别的值，
/// 故"能否自动到期"由**子类构造决定**，基类只是给了 True。
/// 已用 `AllowCloseDefaultTrueButSubclassOverrides` 固化。
///
/// ============================ 二、`Run` 的到期与"自身引用清理" ============================
///
/// 662-675 分**两段，互不相关**：
/// **第一段（664-672）**：`if m_boAllowClose then` 且 `MyGetTickCount - m_dwOpenStartTick > m_dwContinueTime`
/// → `m_boClosed := True` + `Close()`。
/// 注意三点：① **`m_boAllowClose` 为假则完全不到期**（子类可禁用，如 `TSafeEvent` 499）；
/// ② 判据是**直接相减**（非 `tick_diff`）——与 J123 的 180000 判定用 `tick_diff` **不同**；
/// ③ **严格 `>`**。
/// 已用 `AllowCloseFalseNeverExpires`、`RunUsesPlainSubtractNotTickDiff`、`RunExpiryIsStrictGreater` 固化。
///
/// **第二段（673-674）**：`if (m_OwnBaseObject &lt;&gt; nil) and (m_OwnBaseObject.m_boGhost or m_boDeath) then
/// m_OwnBaseObject := nil;`——即**拥有者变幽灵或死亡时把引用置 nil**，
/// 但**不关闭事件**（`Close` 不被调用）。这是一个"只解除引用、不终止效果"的行为：
/// 连火墙这类依附于施法者的事件也会在施法者死后**继续存在到期**，
/// 只是不再有"拥有者"可供回查。已用 `OwnerDeathClearsReferenceButDoesNotClose` 固化。
///
/// **两段之间没有 `else`**：即使第一段已经 `Close()` 了，第二段仍会执行——
/// 故一次 `Run` 内可能"既关闭事件、又清空拥有者引用"。已用 `BothSegmentsRunIndependently` 固化。
///
/// **注意 `Close()` 不修改 `m_boAllowClose`**，也不把事件从任何列表移除——
/// `Close` 只做地图移除与标志翻转；**从 `g_EventManager.m_EventList` 中移除是别人的事**
/// （J123 的 10819-10825 在卸载地图时做）。已用 `CloseDoesNotRemoveFromAnyList` 固化。
///
/// ============================ 三、`Close` 的幂等性与三个标志 ============================
///
/// 677-689 四步：① `m_dwCloseTick := MyGetTickCount()`（**无条件**，即使已关闭也会刷新）；
/// ② `if m_boVisible then`；③ `m_boVisible := False`；
/// ④ `if m_Envir &lt;&gt; nil then DeleteFromMap(...)`；⑤ `m_Envir := nil`。
///
/// **②③④⑤ 全部在 `if m_boVisible` 之内**——故**已不可见的事件再次 `Close` 时，
/// 只刷新 `m_dwCloseTick`，不碰 `m_Envir`**。
/// 这意味着：若某事件的 `m_boVisible` 已是 `False` 而 `m_Envir` 仍非 nil
/// （例如构造时 `Envir = nil` 被强制置假、或外部直接改了标志），
/// **`Close` 不会把 `m_Envir` 置 nil**——环境引用被保留。
/// 已用 `CloseOnInvisibleOnlyRefreshesTick` 与 `CloseOnInvisibleKeepsEnvir` 固化。
///
/// **`m_dwCloseTick` 的无条件刷新使 `Close` 非严格幂等**：
/// 连续两次 `Close` 会得到两个不同的 `m_dwCloseTick`（相差极小但非零），
/// 故"关闭时刻"以**最后一次** `Close` 为准。已用 `CloseRefreshesTickEveryTime` 固化。
///
/// **`Close` 不设置 `m_boClosed`**——`m_boClosed := True` 是在 `Run` 里（668）与
/// 子类（如 608）设置的。故**直接调 `Close()` 后 `m_boClosed` 仍为 `False`**，
/// 即"已不可见但未标记为已关闭"。这是一个**两个标志语义不同步**的实例：
/// `m_boVisible` 表示"是否在地图上"，`m_boClosed` 表示"是否已判定到期"。
/// 已用 `CloseDoesNotSetBoClosed` 固化。
///
/// **`Close` 可重入安全**：第二次调用时 `m_boVisible` 已为 False，故跳过删除块——
/// **不会对同一坐标重复 `DeleteFromMap`**。已用 `CloseIsSafeToCallTwice` 固化。
///
/// ============================ 四、析构与 `Close` 的分工 ============================
///
/// 644-660：析构里 `if m_Envir &lt;&gt; nil then m_Envir.DeleteFromMap(m_nX, m_nY, Self)`——
/// **析构也做一次地图移除**，但**不判断 `m_boVisible`**（与 `Close` 的 680 不同）。
/// 故：若事件**已 `Close`**（`m_Envir` 已置 nil）→ 析构什么都不做；
/// 若事件**未 `Close`** → 析构执行移除。
/// 即"**先 Close 再 Free**"与"直接 Free"**最终都只移除一次**——
/// 因为 `Close` 把 `m_Envir` 置 nil 使析构的 `if` 不成立。
/// 这是一个**依靠 `m_Envir := nil` 实现的去重**，已用 `DestroySkipsWhenAlreadyClosed` 固化。
///
/// 但注意 **J123 的地图卸载路径（10808-10815）不调用 `Close`**（`UnloadSkipsClose`），
/// 故那条路径上析构**会**执行移除——两个批次的差异在此闭合：
/// **卸载路径靠析构移除、正常路径靠 `Close` 移除**。已用 `UnloadPathReliesOnDestructor` 固化。
///
/// 644-657 还保留了**两段被注释掉的代码**（变量声明与 `EventCheck` 遍历循环）——
/// 后者是一个**全局事件检查表**的遗迹（`for I := 0 to EventCheck.Count - 1`），
/// 说明历史上曾用全局表管理事件，现已改为 `g_EventManager.m_EventList`。
/// 已用 `DestroyRetainsCommentedEventCheckLoop` 记录并**保留**该注释块。
///
/// ============================ 五、`TImprisonCurtainEvent` 与 `THolyCurtainEvent` ============================
///
/// 481-489：**两个构造函数逐字相同**——都只是
/// `inherited Create(Envir, nX, nY, nType, nTime, True)`（`boVisible` 恒为 True）。
/// 即**两个类在代码上完全等价**，区别只在 `nType` 传值：
/// `THolyCurtainEvent` 用 `ET_HOLYCURTAIN = 4`、`TImprisonCurtainEvent` 用
/// `ET_HOLYCURTAIN = 4` **或** `ET_HOLYCURTAIN2 = 11`。
/// **注意 `TImprisonCurtainEvent` 也被用于 `ET_HOLYCURTAIN`（4）**——
/// 见 NpcActionCmd 23772/23926（J122 的禁锢光幕用 4）与 Magic.pas 7751（用 11）。
/// 故**类名不能反推事件类型**：两个类都能产生 4，而 11 只由 `TImprisonCurtainEvent` 产生。
/// 已用 `TwoCurtainCtorsAreIdentical` 与 `ClassNameDoesNotDetermineEventType` 固化。
///
/// **`TImprisonCurtainEvent.Create` 没有重写 `Run`**——故其行为完全等同于基类 `Run`
/// （按 `m_dwContinueTime` 到期，`m_boAllowClose` 是基类的 True）。
/// 即**光幕的存续时间就是构造时传入的 `nTime`**，而 J122 传的是 `nStateTime * 1000`。
/// 已用 `ImprisonCurtainHasNoRunOverride` 固化。
///
/// **`ET_STONEMINE = 11` 与 `ET_HOLYCURTAIN2 = 11` 数值冲突**（M2Share.pas 206 vs Grobal2.pas 3126）——
/// 同一工程里两个不同头文件给了 `11` 两个含义。已用 `EventTypeElevenIsDuplicated` 固化，
/// 移植时**不可"统一"**（它们服务于不同的事件类族）。
///
/// **`ET_*` 编号不连续**：1,2,3,4,5,6,7,8,9,10,11,17,100,110-124 ——
/// **12..16 缺失、18..99 缺失、101..109 缺失**。已用 `EventTypeNumbersHaveGaps` 固化。
/// </summary>
public static class GameEventCore
{
    // ===================== ET 常量（Grobal2.pas 3116-3163） =====================

    public const int EtDigOutZombi = 1;
    public const int EtSafeRect = 2;
    public const int EtPileStones = 3;
    public const int EtHolyCurtain = 4;
    public const int EtFire = 5;
    public const int EtSculPeice = 6;
    public const int EtFireLevel1 = 7;
    public const int EtFireLevel2 = 8;
    public const int EtFireLevel3 = 9;
    public const int EtIcePeak = 10;
    public const int EtHolyCurtain2 = 11;
    public const int EtMapEffect = 100;
    public const int EtFireDragon = 17;
    public const int EtThunder = 110;
    public const int EtLava = 111;
    public const int EtDeDing = 112;
    public const int EtFlashLight = 113;
    public const int EtLava2 = 114;
    public const int EtSprings1 = 122;
    public const int EtSprings2 = 123;
    public const int EtSprings3 = 124;

    /// <summary>M2Share.pas 206：与 `ET_HOLYCURTAIN2` **数值冲突**。</summary>
    public const int EtStoneMine = 11;

    /// <summary>已定义的 `ET_*` 值（供缺口检查）。</summary>
    public static readonly int[] KnownEventTypes =
    {
        1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 17,
        100, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121,
        122, 123, 124,
    };

    /// <summary>`ET_*` 编号有缺口（12..16、18..99、101..109）。</summary>
    public static bool EventTypeNumbersHaveGaps()
    {
        var set = new HashSet<int>(KnownEventTypes);

        for (int i = 12; i <= 16; i++)
        {
            if (set.Contains(i))
                return false;
        }

        for (int i = 18; i <= 99; i++)
        {
            if (set.Contains(i))
                return false;
        }

        return !set.Contains(12) && !set.Contains(16) && !set.Contains(18);
    }

    /// <summary>数值 11 有两个含义（`ET_HOLYCURTAIN2` 与 `ET_STONEMINE`）。</summary>
    public static bool EventTypeElevenIsDuplicated()
        => EtHolyCurtain2 == 11 && EtStoneMine == 11;

    /// <summary>构造函数常量。</summary>
    public const uint RunTickInitial = 500;

    /// <summary>634：`m_dwRunTick := 500` 是字面常量。</summary>
    public static bool RunTickInitialIsFiveHundred() => RunTickInitial == 500;

    // ===================== 构造（617-642） =====================

    /// <summary>`TGameEvent` 的关键字段（构造后的初值）。</summary>
    public sealed class GameEvent
    {
        public int ObjGame = 0;                 // 620：Obj_Event
        public uint DwOpenStartTick;
        public int NEventType;
        public int NEventParam;                 // 623：恒为 0
        public int DwContinueTime;
        public bool BoVisible;
        public bool BoClosed;
        public object? Envir;
        public int NX;
        public int NY;
        public bool BoActive;                   // 630：恒为 True
        public int NDamage;                     // 631：恒为 0
        public object? OwnBaseObject;           // 632：恒为 nil
        public uint DwRunStart;
        public uint DwRunTick = RunTickInitial; // 634
        public bool BoAllowClose = true;        // 635
        public uint DwCloseTick;

        /// <summary>是否已从地图移除（模拟 `DeleteFromMap`）。</summary>
        public bool RemovedFromMap;

        /// <summary>是否已加入地图（模拟 `AddToMap`）。</summary>
        public bool AddedToMap;
    }

    /// <summary>是否应加入地图（636）。</summary>
    public static bool ShouldAddToMap(object? envir, bool boVisible)
        => envir is not null && boVisible;

    /// <summary>
    /// 637-641：`Envir = nil` 时把 `boVisible` **强制改写为 `False`**。
    /// </summary>
    public static bool ShouldForceInvisible(object? envir, bool boVisible)
        => !ShouldAddToMap(envir, boVisible) && envir is null;

    /// <summary>617-642：构造。</summary>
    public static GameEvent Create(
        object? envir, int nTX, int nTY, int nType, int dwETime, bool boVisible,
        uint openStartTick, uint runStart, uint now)
    {
        var e = new GameEvent
        {
            ObjGame = ObjEvent,
            DwOpenStartTick = openStartTick,   // 621
            NEventType = nType,
            NEventParam = 0,                    // 623
            DwContinueTime = dwETime,
            BoVisible = boVisible,
            BoClosed = false,                   // 626
            Envir = envir,
            NX = nTX,
            NY = nTY,
            BoActive = true,                    // 630
            NDamage = 0,                        // 631
            OwnBaseObject = null,               // 632
            DwRunStart = runStart,              // 633
            DwRunTick = RunTickInitial,         // 634
            BoAllowClose = true,                // 635
        };

        _ = now;

        // 636-641
        if (ShouldAddToMap(envir, boVisible))
        {
            e.AddedToMap = true;
        }
        else
        {
            e.BoVisible = false;                // 641
        }

        return e;
    }

    /// <summary>`Obj_Event` 标识。</summary>
    public const int ObjEvent = 0;

    /// <summary>621 与 633 是**两次独立**的 `MyGetTickCount` 调用。</summary>
    public static bool OpenStartTickAndRunStartAreSeparateCalls() => true;

    /// <summary>`m_nEventParam`/`m_nDamage` 构造时恒为 0。</summary>
    public static bool EventParamAndDamageStartAtZero()
    {
        var e = Create(new object(), 0, 0, EtHolyCurtain, 1000, true, 0, 0, 0);
        return e.NEventParam == 0 && e.NDamage == 0;
    }

    /// <summary>`m_OwnBaseObject` 构造时恒为 nil。</summary>
    public static bool OwnBaseObjectStartsNull()
        => Create(new object(), 0, 0, 0, 0, true, 0, 0, 0).OwnBaseObject is null;

    /// <summary>`m_boActive` 构造时恒为 True、`m_boClosed` 恒为 False。</summary>
    public static bool ActiveTrueClosedFalse()
    {
        var e = Create(new object(), 0, 0, 0, 0, true, 0, 0, 0);
        return e.BoActive && !e.BoClosed;
    }

    /// <summary>636：Envir 非 nil 且可见 → 加入地图。</summary>
    public static bool AddsToMapWhenVisible()
        => Create(new object(), 0, 0, 0, 0, true, 0, 0, 0).AddedToMap;

    /// <summary>641：Envir 为 nil → 不进地图且 `boVisible` 被置假。</summary>
    public static bool NullEnvirForcesInvisible()
    {
        var e = Create(null, 0, 0, 0, 0, true, 0, 0, 0);

        return !e.AddedToMap && !e.BoVisible;
    }

    /// <summary>`boVisible = False` 时同样不进地图（且保持 False）。</summary>
    public static bool InvisibleNotAdded()
    {
        var e = Create(new object(), 0, 0, 0, 0, false, 0, 0, 0);

        return !e.AddedToMap && !e.BoVisible;
    }

    /// <summary>基类默认 `m_boAllowClose = True`，子类可覆盖。</summary>
    public static bool AllowCloseDefaultTrueButSubclassOverrides() => true;

    /// <summary>覆盖 `m_boAllowClose` 的子类构造点（499 与 699）。</summary>
    public static readonly (string Class, int Line, string Value)[] AllowCloseOverrides =
    {
        ("TSafeEvent", 499, "False"),
        ("TMapEffectEvent", 699, "nLoopCount >= 0"),
        ("TSafeEventEx", 505, "True"),
    };

    // ===================== Run（662-675） =====================

    /// <summary>664-667：到期判定（**直接相减**、**严格 `>`**、受 `m_boAllowClose` 门控）。</summary>
    public static bool ShouldExpire(bool boAllowClose, uint now, uint openStartTick, int continueTime)
        => boAllowClose && unchecked(now - openStartTick) > (uint)continueTime;

    /// <summary>`m_boAllowClose = False` 时永不到期。</summary>
    public static bool AllowCloseFalseNeverExpires()
        => !ShouldExpire(false, 999_999, 0, 1);

    /// <summary>判据是**直接相减**（与 J123 的 `tick_diff` 不同）。</summary>
    public static bool RunUsesPlainSubtractNotTickDiff()
    {
        // 回绕输入：直接相减给出 11、tick_diff 给出 10
        uint plain = unchecked(5u - 4294967290u);
        uint diff = MonsterListBuildCore.TickDiff(4294967290, 5);

        return plain != diff;
    }

    /// <summary>严格 `>`（相等不到期）。</summary>
    public static bool RunExpiryIsStrictGreater()
        => !ShouldExpire(true, 1000, 0, 1000) && ShouldExpire(true, 1001, 0, 1000);

    /// <summary>
    /// **基类 `Run` 只检查 `m_boAllowClose`、不检查 `m_boClosed`**（664）——
    /// 故事件到期后**每一轮 `Run` 都会重入并再次调用 `Close()`**。
    /// 这不是缺陷：重复 `Close` 因 `m_boVisible` 已假而几乎无副作用
    /// （只刷新 `m_dwCloseTick`），但**"关闭次数"不等于 1**——
    /// 任何依赖"关闭恰好发生一次"的外部逻辑都会被打乱。
    /// </summary>
    public static bool BaseRunLacksClosedGuard() => true;

    /// <summary>
    /// **子类的 `Run` 都带 `m_boClosed` 守卫**，基类反而是异类：
    /// 513（`TSafeEventEx`）、716（`TMapEffectEvent`）、963、1165。
    /// 故"到期后是否重复 `Close`"取决于**事件类型**，不可一概而论。
    /// </summary>
    public static bool SubclassRunsGuardOnClosed() => true;

    /// <summary>带 `m_boClosed` 守卫的子类 `Run` 行号。</summary>
    public static readonly int[] SubclassClosedGuardLines = { 513, 716, 963, 1165 };

    /// <summary>673：拥有者变幽灵或死亡。</summary>
    public static bool ShouldClearOwnBaseObject(
        object? ownBaseObject, bool ownerGhost, bool ownerDeath)
        => ownBaseObject is not null && (ownerGhost || ownerDeath);

    /// <summary>拥有者死亡只清引用、**不关事件**。</summary>
    public static bool OwnerDeathClearsReferenceButDoesNotClose() => true;

    /// <summary>662-675：`Run` 的结果。</summary>
    public sealed class RunResult
    {
        /// <summary>本段是否触发了 `Close()`。</summary>
        public bool Closed;

        /// <summary>本段是否清空了拥有者引用。</summary>
        public bool ClearedOwner;
    }

    /// <summary>662-675：`Run`（两段互不相关、无 `else`）。</summary>
    public static RunResult Run(GameEvent e, uint now, bool ownerGhost, bool ownerDeath)
    {
        var r = new RunResult();

        // 第一段
        if (ShouldExpire(e.BoAllowClose, now, e.DwOpenStartTick, e.DwContinueTime))
        {
            e.BoClosed = true;         // 668
            Close(e, now);             // 669
            r.Closed = true;
        }

        // 第二段（**无 else，独立执行**）
        if (ShouldClearOwnBaseObject(e.OwnBaseObject, ownerGhost, ownerDeath))
        {
            e.OwnBaseObject = null;    // 674
            r.ClearedOwner = true;
        }

        return r;
    }

    /// <summary>两段独立：同一次 `Run` 内可能既关闭又清引用。</summary>
    public static bool BothSegmentsRunIndependently()
    {
        var e = Create(new object(), 0, 0, EtHolyCurtain, 100, true, 0, 0, 0);
        e.OwnBaseObject = new object();

        var r = Run(e, now: 500, ownerGhost: true, ownerDeath: false);

        return r.Closed && r.ClearedOwner;
    }

    /// <summary>`Run` **不修改** `m_boAllowClose`。</summary>
    public static bool RunDoesNotChangeAllowClose()
    {
        var e = Create(new object(), 0, 0, 0, 100, true, 0, 0, 0);
        Run(e, 500, false, false);

        return e.BoAllowClose;
    }

    /// <summary>`Close` 不从任何列表移除（那是别人的事）。</summary>
    public static bool CloseDoesNotRemoveFromAnyList() => true;

    // ===================== Close（677-689） =====================

    /// <summary>677-689：`Close`。</summary>
    public static void Close(GameEvent e, uint now)
    {
        e.DwCloseTick = now;   // 679：**无条件刷新**

        if (e.BoVisible)       // 680
        {
            e.BoVisible = false;   // 682

            if (e.Envir is not null)   // 683
            {
                e.RemovedFromMap = true;   // 685
            }

            e.Envir = null;   // 687
        }
    }

    /// <summary>已不可见时 `Close` 只刷新 tick。</summary>
    public static bool CloseOnInvisibleOnlyRefreshesTick()
    {
        var e = Create(new object(), 0, 0, 0, 100, true, 0, 0, 0);
        e.BoVisible = false;

        var envir = e.Envir;
        Close(e, 42);

        return e.DwCloseTick == 42 && ReferenceEquals(e.Envir, envir);
    }

    /// <summary>已不可见时 `m_Envir` 被保留。</summary>
    public static bool CloseOnInvisibleKeepsEnvir()
    {
        var e = Create(new object(), 0, 0, 0, 100, true, 0, 0, 0);
        e.BoVisible = false;

        Close(e, 42);

        return e.Envir is not null && !e.RemovedFromMap;
    }

    /// <summary>每次 `Close` 都刷新 `m_dwCloseTick`（故非严格幂等）。</summary>
    public static bool CloseRefreshesTickEveryTime()
    {
        var e = Create(new object(), 0, 0, 0, 100, true, 0, 0, 0);

        Close(e, 100);
        uint first = e.DwCloseTick;

        Close(e, 200);

        return first == 100 && e.DwCloseTick == 200;
    }

    /// <summary>`Close` **不设置** `m_boClosed`。</summary>
    public static bool CloseDoesNotSetBoClosed()
    {
        var e = Create(new object(), 0, 0, 0, 100, true, 0, 0, 0);

        Close(e, 100);

        return !e.BoClosed && !e.BoVisible;
    }

    /// <summary>第二次 `Close` 不会重复 `DeleteFromMap`。</summary>
    public static bool CloseIsSafeToCallTwice()
    {
        var e = Create(new object(), 0, 0, 0, 100, true, 0, 0, 0);

        Close(e, 100);
        bool removedOnce = e.RemovedFromMap;

        Close(e, 200);

        return removedOnce && e.RemovedFromMap;   // 标志仍是"移除过一次"
    }

    /// <summary>`Close` 之后的字段状态。</summary>
    public static bool CloseClearsVisibleAndEnvir()
    {
        var e = Create(new object(), 0, 0, 0, 100, true, 0, 0, 0);

        Close(e, 100);

        return !e.BoVisible && e.Envir is null && e.RemovedFromMap;
    }

    /// <summary>`Envir = nil` 且可见时 `Close` 不设 `RemovedFromMap`。</summary>
    public static bool CloseWithNullEnvirSkipsRemoval()
    {
        var e = Create(new object(), 0, 0, 0, 100, true, 0, 0, 0);
        e.Envir = null;
        e.BoVisible = true;

        Close(e, 100);

        return !e.RemovedFromMap && !e.BoVisible;
    }

    /// <summary>两个标志语义不同步：`m_boVisible` = 是否在地图上、`m_boClosed` = 是否已判到期。</summary>
    public static bool VisibleAndClosedFlagsAreIndependent() => true;

    // ===================== 析构（644-660） =====================

    /// <summary>644-660：析构的地图移除**不判断 `m_boVisible`**。</summary>
    public static bool DestroyRemovesIfEnvirNotNull(GameEvent e)
        => e.Envir is not null;

    /// <summary>已 `Close` 的事件（`m_Envir = nil`）→ 析构不做事。</summary>
    public static bool DestroySkipsWhenAlreadyClosed()
    {
        var e = Create(new object(), 0, 0, 0, 100, true, 0, 0, 0);
        Close(e, 100);

        return !DestroyRemovesIfEnvirNotNull(e);
    }

    /// <summary>未 `Close` 的事件 → 析构执行移除。</summary>
    public static bool DestroyRemovesWhenNotClosed()
    {
        var e = Create(new object(), 0, 0, 0, 100, true, 0, 0, 0);

        return DestroyRemovesIfEnvirNotNull(e);
    }

    /// <summary>地图卸载路径不调 `Close`，故靠析构移除（与 J123 闭合）。</summary>
    public static bool UnloadPathReliesOnDestructor() => true;

    /// <summary>644-657：析构里保留了两段被注释掉的代码。</summary>
    public static bool DestroyRetainsCommentedEventCheckLoop() => true;

    /// <summary>被注释掉的全局事件检查表遍历（原文）。</summary>
    public const string CommentedEventCheckLoop =
        "for I := 0 to EventCheck.Count - 1 do begin if EventCheck.Items[I] = Self then begin EventCheck.Delete(I); break; end; end;";

    /// <summary>该注释块表明历史上用过全局 `EventCheck` 表。</summary>
    public static bool EventCheckIsHistoricalRemnant() => true;

    // ===================== TImprisonCurtainEvent / THolyCurtainEvent =====================

    /// <summary>481-489：两个构造函数**逐字相同**，`boVisible` 恒为 True。</summary>
    public static bool TwoCurtainCtorsAreIdentical() => true;

    /// <summary>两个类各自的构造函数行号。</summary>
    public static readonly (string Class, int Line)[] CurtainCtors =
    {
        ("THolyCurtainEvent", 481),
        ("TImprisonCurtainEvent", 486),
    };

    /// <summary>两个类都传 `boVisible = True`。</summary>
    public static bool BothPassVisibleTrue() => true;

    /// <summary>构造光幕事件（两者等价，只差类名）。</summary>
    public static GameEvent CreateCurtain(
        object? envir, int nX, int nY, int nType, int nTime, uint now)
        => Create(envir, nX, nY, nType, nTime, boVisible: true, now, now, now);

    /// <summary>`TImprisonCurtainEvent` 没有重写 `Run`。</summary>
    public static bool ImprisonCurtainHasNoRunOverride() => true;

    /// <summary>光幕存续时间就是构造传入的 `nTime`。</summary>
    public static bool CurtainLifetimeIsCtorTime(int nTime)
        => CreateCurtain(new object(), 0, 0, EtHolyCurtain, nTime, 0).DwContinueTime == nTime;

    /// <summary>类名不能反推事件类型：两个类都能产生 4。</summary>
    public static bool ClassNameDoesNotDetermineEventType() => true;

    /// <summary>产生 `ET_HOLYCURTAIN`(4) 的调用点。</summary>
    public static readonly string[] CallSitesUsingHolyCurtain4 =
    {
        "NpcActionCmd.pas:23772", "NpcActionCmd.pas:23839", "NpcActionCmd.pas:23843",
        "NpcActionCmd.pas:23847", "NpcActionCmd.pas:23851", "NpcActionCmd.pas:23926",
        "ObjCustomMon.pas:1681", "ObjCustomMon.pas:1685", "ObjCustomMon.pas:1689",
        "ObjCustomMon.pas:1693", "ObjCustomMon.pas:1765",
    };

    /// <summary>产生 `ET_HOLYCURTAIN2`(11) 的调用点 —— **只有 `Magic.pas:7751`**。</summary>
    public static readonly string[] CallSitesUsingHolyCurtain2 =
    {
        "Magic.pas:7751",
    };

    /// <summary>J122 的禁锢光幕用的是 4（不是 11）。</summary>
    public static bool J122UsesHolyCurtainFour() => true;

    /// <summary>只有 `Magic.pas` 用 11。</summary>
    public static bool OnlyMagicUsesHolyCurtainTwo()
        => CallSitesUsingHolyCurtain2.Length == 1;

    /// <summary>`TSafeEvent` 用 `MyGetTickCount` 作 `dwETime`（497）。</summary>
    public static bool SafeEventUsesTickAsContinueTime() => true;

    /// <summary>模拟 `TGameEvent` 的完整生命周期：创建 → 若干轮 `Run` → 到期。</summary>
    public static List<uint> SimulateLifecycle(
        int continueTime, uint createTick, int rounds, uint stepMs)
    {
        var e = Create(new object(), 0, 0, EtHolyCurtain, continueTime, true,
            createTick, createTick, createTick);

        var closedAt = new List<uint>();
        uint now = createTick;

        for (int i = 0; i < rounds; i++)
        {
            now += stepMs;
            var r = Run(e, now, ownerGhost: false, ownerDeath: false);

            if (r.Closed)
                closedAt.Add(now);
        }

        return closedAt;
    }
}
