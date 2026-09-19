using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 副本地图**人数统计与回收** 1:1 移植（批次J113）。
/// 主源：`UsrEngn.pas` 10455-10603（`ProcessFBMapObjectCount` 与 `ProcessFBMap` 前半），
/// 另含字段声明 `Envir.pas` 304-316 与构造初值 3564-3574。
///
/// J112 移植了实例的**分配与占用**，本批次补上**归还**：副本何时被回收、如何清怪。
/// 至此副本生命周期完整：**声明 → 注册 → 分配 → 占用 → 统计 → 回收 → 清怪 → 可再分配**。
///
/// **`ProcessFBMapObjectCount`（10456-10483）是"先全清零、再重新数"的两段式**：
/// ① 遍历 `g_FBMapManager` 的**所有**实例，把 `m_dwFBPlayObjectCount` 置 **0**（10467）；
/// ② 遍历 `m_PlayObjectList`，对每个满足三条件的玩家 `Inc` 其所在地图的人数（10477-10478）。
/// 三条件是 `(not m_boGhost) and (m_PEnvir &lt;&gt; nil) and (m_PEnvir.m_boFB)`——
/// **注意第三项判的是"地图是副本地图"，而不是"玩家在某个副本里"**，
/// 故非副本地图上的人数不会被计入任何实例。`m_boGhost` 排除幽灵玩家。
/// **这是一个 O(实例数 + 玩家数) 的全量重算，而非增量维护**——
/// 故 `Inc`/`Dec` 的遗漏不会累积错误，下一轮就被纠正；但也意味着
/// **任何在两次统计之间读取人数的地方看到的都是上一轮的快照**。
///
/// **`ProcessFBMap` 有两条独立的释放路径，条件完全不同，不可合并**：
///
/// **路径一（10516-10521）：失败超时释放**。
/// 条件仅 `m_boFBFail and (MyGetTickCount &gt; m_dwFBFailTime)`——**严格的 `&gt;`**。
/// 动作三项：`m_FBMasterObject := nil`、`m_boFBCreate := False`、`m_dwFBCheckMonsterTick := 0`。
/// **注意它清零了 `m_dwFBCheckMonsterTick`**，从而**允许紧随其后的路径二循环体立即执行**
/// （因 10565 的条件是 `MyGetTickCount &gt; m_dwFBCheckMonsterTick`，
/// 归零后必然成立）——这是一个刻意的"释放后立刻走一次检查"的连锁。
/// 路径一**不清怪**（无 `MakeGhost`/`Clear`），清怪交给随后的路径二块。
///
/// **路径二（10522-10563）：到期/无人释放**。外层条件两选一（**或**）：
/// `(MyGetTickCount &gt;= m_dwFBCreateTime + 60000 + m_dwFBEnterDelayMin)`——**`&gt;=` 闭区间**，
/// 即"创建满 1 分钟**加上**延时进入分钟数"；或 `m_boFBPlayObjectEnter`（有人已进入过）。
/// 内层再分：
/// - `m_dwFBPlayObjectCount &gt; 0` → 把 `m_dwFBNOPlayObjectTick := MyGetTickCount + 1000`
///   （**前推 1 秒**，作为"还有人，暂不回收"的续期）；
/// - 否则（无人）：**三岔判断**（10529-10532）——
///   ① `MyGetTickCount &lt; m_dwFBNOPlayObjectTick` → **回拨**为当前时刻
///      （处理计时器回绕或时钟倒退）；
///   ② `MyGetTickCount &gt; m_dwFBNOPlayObjectTick` **且** `差值 &gt;= m_dwFBNoHumClearMin`
///      → 真正回收（**`&gt;=` 闭区间**）；
///   ③ 两者相等 → **什么都不做**（既不回拨也不回收），下一轮再看。
///   **③ 这一情况极易被忽略**：`MyGetTickCount = m_dwFBNOPlayObjectTick` 时静默跳过。
///
/// **回收动作为两项**：`m_FBMasterObject := nil`、`m_boFBCreate := False`
/// （10534-10535）——**置 `m_boFBCreate := False` 正是让实例回到"可再分配"状态的关键**，
/// 因为 J112 的空闲判定要求 `not m_boFBCreate` **且** 人数 `&lt;= 0`。
///
/// **清怪块（10537-10560）与（10579-10602）是同一段代码的两次出现**（原文重复），
/// 语义为**按 `m_boFBCreate` 分支的两种相反处理**——这是本批次最反直觉之处：
/// - **若 `not m_boFBCreate`（已回收）**：对每个怪，若**不是幽灵且无主人** → `MakeGhost`
///   （让它消失）；循环后再 `m_FBMonsterList.Clear` **整体清空**；
/// - **若 `m_boFBCreate`（仍在创建中/未回收）**：只删除**已经是幽灵或已死亡**的怪
///   （`Delete(d)`），**不**对活怪做任何动作。
/// 即**同一段代码在"已回收"时是"全部变幽灵并清空"，在"未回收"时只是"清理尸体"**。
/// 循环用 `for d := Count - 1 downto 0`（**倒序**），故 `Delete(d)` 不会漏项。
/// **注意清空列表用的是 `Clear` 而非 `Free`**，与 J112 的 `m_FBMonsterList.Clear`（23187）一致。
///
/// **`m_dwFBCheckMonsterTick` 门控（10565-10567）**：`MyGetTickCount &gt; m_dwFBCheckMonsterTick`
/// 才进入，进入后立刻把它设为 `MyGetTickCount + 60 * 1000`——即**每分钟最多执行一次**。
/// 原文此处有一行被注释掉的 `// + Random(60 * 1000);`，保持原样不复原。
///
/// **内部释放条件（10569-10570）**：`m_boFBCreate` **且** 三者之一（**或**）——
/// `m_FBMasterObject = nil`、`m_dwFBPlayObjectCount &lt;= 0`、`MyGetTickCount &gt; m_dwFBTime`。
/// 动作三项：`m_FBMasterObject := nil`、`m_boFBCreate := False`、`m_nGuardinaLevelMonCount := 0`。
/// **注意它比路径二多了 `m_nGuardinaLevelMonCount := 0`**（守卫等级怪物计数），
/// 且条件里的 `&gt; m_dwFBTime`（**严格 `&gt;`**）与路径二的 `&gt;=` 形成对照。
/// </summary>
public static class FbRecycleCore
{
    /// <summary>10520：路径一归零 `m_dwFBCheckMonsterTick`，使 10565 门控立即打开。</summary>
    public const int CheckMonsterTickReset = 0;

    /// <summary>10567：检查间隔 60 秒。</summary>
    public const int CheckMonsterIntervalMs = 60_000;

    /// <summary>10526：有人时把无人物计时前推 1 秒。</summary>
    public const int NoPlayObjectRenewMs = 1_000;

    /// <summary>10522：创建后满 1 分钟才允许常规回收。</summary>
    public const int CreateGraceMs = 60_000;

    /// <summary>10623：创建人离线判定的 1 分钟门槛。</summary>
    public const int CreaterOfflineGraceMs = 60_000;

    /// <summary>副本实例的回收相关字段（对应 `TEnvirnoment` 子集）。</summary>
    public sealed class FbRecycleState
    {
        /// <summary>316：人数（由 `ProcessFBMapObjectCount` 每轮全量重算）。</summary>
        public int PlayObjectCount;

        /// <summary>309：地图失败标志。</summary>
        public bool BoFBFail;

        /// <summary>310：失败后的到期时刻。</summary>
        public int FbFailTime;

        /// <summary>是否已被创建占用。</summary>
        public bool BoFBCreate;

        /// <summary>10520/10567：怪物检查门控时刻。</summary>
        public int FbCheckMonsterTick;

        /// <summary>10522：创建时刻。</summary>
        public int FbCreateTime;

        /// <summary>2242：延时进入（毫秒）。</summary>
        public int FbEnterDelayMs;

        /// <summary>10523：是否有人已进入过。</summary>
        public bool BoFBPlayObjectEnter;

        /// <summary>10526/10530：无人物计时基准。</summary>
        public int FbNoPlayObjectTick;

        /// <summary>2243：无人多久后回收（毫秒）。</summary>
        public int FbNoHumClearMs;

        /// <summary>23159：副本到期时刻。</summary>
        public int FbTime;

        /// <summary>创建者。</summary>
        public object? FbMasterObject;

        /// <summary>10575：守卫等级怪物计数。</summary>
        public int GuardinaLevelMonCount;

        /// <summary>怪物列表（含幽灵标记与主人信息）。</summary>
        public List<FbMonster> FbMonsterList = new();
    }

    /// <summary>怪物在清怪块中的相关字段。</summary>
    public sealed class FbMonster
    {
        public string Name = "";

        /// <summary>是否已是幽灵（不可见/待回收）。</summary>
        public bool BoGhost;

        /// <summary>是否已死亡。</summary>
        public bool BoDeath;

        /// <summary>主人（非 nil 表示是召唤物/宝宝）。</summary>
        public object? Master;

        /// <summary>清怪块是否对其调用了 `MakeGhost`。</summary>
        public bool MadeGhost;

        /// <summary>是否被 `Delete` 出列表。</summary>
        public bool Deleted;
    }

    // ===================== ProcessFBMapObjectCount（10456-10483） =====================

    /// <summary>
    /// 10467：第一段——把所有实例的人数**清零**。
    /// </summary>
    public static void ResetAllPlayObjectCounts<T>(IEnumerable<IList<T>> allFbLists, Func<T, FbRecycleState> state)
    {
        foreach (var list in allFbLists)
        {
            for (int k = 0; k < list.Count; k++)
                state(list[k]).PlayObjectCount = 0;
        }
    }

    /// <summary>10477：计数三条件——非幽灵、有地图、**地图是副本地图**。</summary>
    public static bool ShouldCountPlayObject(bool boGhost, bool hasEnvir, bool envirBoFB)
        => !boGhost && hasEnvir && envirBoFB;

    /// <summary>
    /// 10470-10482：第二段——遍历玩家列表，对满足三条件者给其**所在地图**加一。
    /// `getEnvirState` 返回玩家所在地图的状态；无地图或非副本地图时返回 `null`。
    /// </summary>
    public static void CountPlayObjects<TP>(
        IReadOnlyList<TP?> playObjects, Func<TP, bool> isGhost,
        Func<TP, FbRecycleState?> getEnvirState, Func<TP, bool> envirIsFb)
    {
        for (int i = 0; i < playObjects.Count; i++)
        {
            var po = playObjects[i];
            if (po is null)
                continue;   // 10475-10476

            var st = getEnvirState(po);
            if (ShouldCountPlayObject(isGhost(po), st is not null, st is not null && envirIsFb(po)))
                st!.PlayObjectCount++;
        }
    }

    // ===================== 两条释放路径 =====================

    /// <summary>10516：路径一条件——`m_boFBFail and (now &gt; m_dwFBFailTime)`，**严格 `&gt;`**。</summary>
    public static bool ShouldReleaseOnFail(FbRecycleState s, int now)
        => s.BoFBFail && now > s.FbFailTime;

    /// <summary>
    /// 10518-10520：路径一动作三连。**第三项归零 `m_dwFBCheckMonsterTick`**
    /// 会立刻打开 10565 的门控，使路径二块在同一轮内执行。
    /// </summary>
    public static void ApplyFailRelease(FbRecycleState s)
    {
        s.FbMasterObject = null;
        s.BoFBCreate = false;
        s.FbCheckMonsterTick = CheckMonsterTickReset;
    }

    /// <summary>
    /// 10522-10523：路径二外层条件——**或**关系。
    /// ① `now &gt;= FbCreateTime + 60000 + FbEnterDelayMs`（**闭区间**）；
    /// ② `BoFBPlayObjectEnter`。
    /// </summary>
    public static bool ShouldEnterExpiryBlock(FbRecycleState s, int now)
        => (now >= s.FbCreateTime + CreateGraceMs + s.FbEnterDelayMs) || s.BoFBPlayObjectEnter;

    /// <summary>无人时三岔判断的结果（10529-10532）。</summary>
    public enum ExpiryAction
    {
        /// <summary>10525-10526：有人 → 把无人物计时前推 1 秒。</summary>
        RenewBecauseOccupied,

        /// <summary>10530：`now &lt; tick` → 把 tick **回拨**为 now。</summary>
        ClampTickBackToNow,

        /// <summary>10531-10532：`now &gt; tick` 且差值 `&gt;= NoHumClearMs` → 真正回收。</summary>
        Recycle,

        /// <summary>10529/10531 均不成立（即 `now = tick`）→ **什么都不做**。</summary>
        DoNothing,
    }

    /// <summary>10525-10532：决定到期块内部动作。**顺序即原文顺序**。</summary>
    public static ExpiryAction SelectExpiryAction(FbRecycleState s, int now)
    {
        if (s.PlayObjectCount > 0)
            return ExpiryAction.RenewBecauseOccupied;

        if (now < s.FbNoPlayObjectTick)
            return ExpiryAction.ClampTickBackToNow;

        if (now > s.FbNoPlayObjectTick && now - s.FbNoPlayObjectTick >= s.FbNoHumClearMs)
            return ExpiryAction.Recycle;

        return ExpiryAction.DoNothing;
    }

    /// <summary>10526：有人时的续期。</summary>
    public static void ApplyRenew(FbRecycleState s, int now)
        => s.FbNoPlayObjectTick = now + NoPlayObjectRenewMs;

    /// <summary>10530：回拨。</summary>
    public static void ApplyClampTick(FbRecycleState s, int now)
        => s.FbNoPlayObjectTick = now;

    /// <summary>10534-10535：真正回收——**只清创建者与创建标志**（清怪在清怪块）。</summary>
    public static void ApplyRecycle(FbRecycleState s)
    {
        s.FbMasterObject = null;
        s.BoFBCreate = false;
    }

    // ===================== 清怪块（10537-10560 / 10579-10602） =====================

    /// <summary>
    /// 10537-10560：清怪块。**按 `m_boFBCreate` 走两种相反处理**。
    /// - `not m_boFBCreate`（已回收）：对"非幽灵且无主人"者 `MakeGhost`；随后 `Clear` 整表；
    /// - `m_boFBCreate`（未回收）：只 `Delete` 掉"幽灵或已死亡"者。
    /// **倒序遍历**，故 `Delete` 安全。
    /// </summary>
    public static void ApplyMonsterCleanup(FbRecycleState s)
    {
        if (s.FbMonsterList.Count == 0)
            return;

        for (int d = s.FbMonsterList.Count - 1; d >= 0; d--)
        {
            FbMonster m = s.FbMonsterList[d];

            if (!s.BoFBCreate)
            {
                // 10545-10550：已回收 → 让"活着的、无主人的"怪变幽灵
                if (!m.BoGhost && m.Master is null)
                {
                    m.MadeGhost = true;
                    m.BoGhost = true;
                }
            }
            else
            {
                // 10553-10556：未回收 → 只清掉已经是幽灵或已死亡的
                if (m.BoGhost || m.BoDeath)
                {
                    m.Deleted = true;
                    s.FbMonsterList.RemoveAt(d);
                }
            }
        }

        // 10558-10559：仅已回收时整体清空
        if (!s.BoFBCreate)
            s.FbMonsterList.Clear();
    }

    /// <summary>10537/10579：只有列表非空才进入清怪块。</summary>
    public static bool ShouldRunMonsterCleanup(FbRecycleState s) => s.FbMonsterList.Count > 0;

    /// <summary>10559/10601：`Clear` 而非 `Free`。</summary>
    public static bool UsesClearNotFree() => true;

    // ===================== check monster 门控（10565-10576） =====================

    /// <summary>10565：`MyGetTickCount &gt; m_dwFBCheckMonsterTick`（**严格 `&gt;`**）。</summary>
    public static bool ShouldCheckMonsters(FbRecycleState s, int now)
        => now > s.FbCheckMonsterTick;

    /// <summary>10567：进入后把门控前推 60 秒（每分钟至多一次）。</summary>
    public static void AdvanceCheckMonsterTick(FbRecycleState s, int now)
        => s.FbCheckMonsterTick = now + CheckMonsterIntervalMs;

    /// <summary>
    /// 10569-10570：内部释放条件三选一（**或**），前置 `m_boFBCreate`。
    /// `now &gt; m_dwFBTime` 用的是**严格 `&gt;`**（与路径二的 `&gt;=` 对照）。
    /// </summary>
    public static bool ShouldReleaseInsideCheck(FbRecycleState s, int now)
        => s.BoFBCreate
        && (s.FbMasterObject is null || s.PlayObjectCount <= 0 || now > s.FbTime);

    /// <summary>10573-10575：内部释放动作——**比路径二多清零 `m_nGuardinaLevelMonCount`**。</summary>
    public static void ApplyInsideRelease(FbRecycleState s)
    {
        s.FbMasterObject = null;
        s.BoFBCreate = false;
        s.GuardinaLevelMonCount = 0;
    }

    /// <summary>10620：`g_Config.boFBExitCreaterOffline` 配置项名（用于说明依赖）。</summary>
    public const string ConfigExitCreaterOffline = "boFBExitCreaterOffline";

    /// <summary>
    /// 10620-10628：创建人离线释放。
    /// 条件：配置开启 **且** `m_FBEnterLimit &lt;&gt; fbel_OnlyCreater`（**只有创建者能进的副本不适用**）
    /// **且** `now - FbCreateTime &gt;= 60000`（**闭区间**）**且** `m_boFBCreate`
    /// **且**（创建人**不在**玩家列表 **或** 其所在地图 ≠ 本实例）。
    /// </summary>
    public static bool ShouldReleaseOnCreaterOffline(
        bool configExitCreaterOffline, FbMapDeclareCore.FbEnterLimit enterLimit,
        FbRecycleState s, int now, bool createrFound, bool createrInThisEnvir)
    {
        if (!configExitCreaterOffline)
            return false;

        if (enterLimit == FbMapDeclareCore.FbEnterLimit.OnlyCreater)
            return false;

        return now - s.FbCreateTime >= CreaterOfflineGraceMs
            && s.BoFBCreate
            && (!createrFound || !createrInThisEnvir);
    }

    /// <summary>10626-10627：创建人离线释放动作（只清创建标志与创建者）。</summary>
    public static void ApplyCreaterOfflineRelease(FbRecycleState s)
    {
        s.BoFBCreate = false;
        s.FbMasterObject = null;
    }

    /// <summary>
    /// 10631-10635：创建者**已不在玩家列表或其地图不是本实例** → 清 `m_FBMasterObject`。
    /// **注意这一支只清创建者、不动 `m_boFBCreate`**（与上面的释放不同）。
    /// </summary>
    public static bool ShouldClearMasterOnly(bool createrFound, bool createrInThisEnvir)
        => !createrFound || !createrInThisEnvir;

    // ===================== 构造初值（Envir.pas 3564-3574） =====================

    /// <summary>3564：`m_dwFBNoHumClearMin := 10`。**注意这是秒数常量 10，不是毫秒**。</summary>
    public const int DefaultNoHumClearMin = 10;

    /// <summary>3567-3568：`m_boFBFail := false; m_dwFBFailTime := 0;`。</summary>
    public static (bool Fail, int FailTime) DefaultFailState() => (false, 0);

    /// <summary>3574：`m_dwFBPlayObjectCount := 0`。</summary>
    public const int DefaultPlayObjectCount = 0;
}
