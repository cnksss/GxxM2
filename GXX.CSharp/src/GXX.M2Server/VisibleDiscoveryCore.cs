using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.M2Server;

/// <summary>
/// ObjBase.pas `SearchViewRange` **内层发现动作** 1:1 移植（批次J104），主源三处：
/// `TBaseObject.UpdateVisibleGay`(31430-31474)、
/// `SearchViewRange` 的**角色准入判定**（31845-31869）、
/// 以及**物品准入判定**（31871-31886）。
///
/// J103 移植了 `SearchViewRange` 的外层（离线闸门、扫描边界、帧末清理），
/// 本批次补上扫描命中一个对象时"**要不要把它记为可见**"的判定，
/// 以及两个关键的**状态副作用**：`m_boIsVisibleActive` 的置位与 `UpdateVisibleGay`/`UpdateVisibleItem` 的调用。
///
/// **`UpdateVisibleGay`（31430-31474）是 J102 `UpdateVisibleItem` 的"对象侧孪生"，两处有一处关键不对称**：
/// | | `UpdateVisibleGay`（对象） | `UpdateVisibleItem`（物品） |
/// |---|---|---|
/// | 新建时初值 | **恒为 2**（31452） | `boSendItemShow` ? **1** : **2**（31488-31490） |
/// | 已存在时 | 引用相等 → **1**（31464） | 引用相等 → **1**（31527） |
/// | 唯一键 | `NativeInt(BaseObject)` | `NativeInt(MapItem)` |
/// 即**对象从不像物品那样"随本次发送而建立"**——因为对象可见性由扫描本身决定，
/// 而物品可能由 `RM_ITEMSHOW` 广播先行送达。
///
/// **`m_boIsVisibleActive` 的全部语义（31439-31440 + 31679）**：
/// - 每次 `SearchViewRange` 入口**先置 `False`**（31679）；
/// - 仅当扫到**玩家**（`RC_PLAYOBJECT`）**或"有主人的宝宝"**（`m_Master <> nil`）时置 `True`（31440）。
/// 故该字段最终回答的是"**本次视野内是否至少有一个玩家或宝宝**"。
///
/// **角色准入判定（31851-31863）是一段嵌套极深的条件，顺序不可换**：
/// 外层（31851-31852）四个条件**同时**成立：
/// ① 非 `m_boGhost`；② 非 `m_boFixedHideMode`；
/// ③ 同地图；④ `((非 m_boObMode 且 非 boTempFixedHideMode) 或 对方的主人就是我)`。
/// **④ 的括号是关键**：隐身对象**只对其主人**可见（原文注释 `H.ChangeModeEx 2 1000 主人看不到英雄`）。
/// 内层（31855-31863）五个条件**任一**成立即调用 `UpdateVisibleGay`：
/// ① `m_btRaceServer < RC_ANIMAL`（自身比动物"低阶"——即自身是玩家/英雄/NPC 等）；
/// ② `m_Master <> nil`（自身是宝宝）；
/// ③ `m_boCrazyMode`；④ `m_boWantRefMsg`；
/// ⑤ 对方有主人 **且** 与自身 X 距离 ≤ `m_nViewRange` **且** Y 距离 ≤ `m_nViewRange + m_nViewRangeExtY`；
/// ⑥ 对方是玩家；⑦ **国战规则**——双方 `m_btNation > 0` 且 `m_boNoSameNationMonPK` 且两国号不同。
/// **注意 ⑤ 的距离门限用的是"自身"的 `m_nViewRange` 与 `m_nViewRangeExtY`**（31858-31859），
/// 与 J101/J103 中出现的其他视野门限又不同（那是第 4 套规则）。
/// 原文 31856-31857 的注释解释了 ⑤ 的来由（"怪物攻击有主人的宝宝必须相距 3 格以内"与赤月恶魔的修正）。
///
/// **物品准入判定（31874-31884）比角色简单得多**：
/// 仅当自身种族属于 `{RC_HEROOBJECT, RC_PLAYMOSTER}` **或** `IsSlavePickItem` 为真时才处理，
/// 且物品非 `m_boGhost` 即调用 `UpdateVisibleItem(n18, n1C, ItemObject)`——
/// **注意传的是坐标 `n18`/`n1C`（即 X/Y 循环变量），且第三参 `boSendItemShow` 省略即取默认 `False`**
/// （故新建物品初值为 **2**）。
/// **玩家自己（`RC_PLAYOBJECT`）不在该集合内**——玩家不走这里的物品发现，
/// 而是靠 `RM_ITEMSHOW` 广播（J101 的 `SendRefMsg`）来建立物品可见性；这正是两者初值规则不同的原因。
/// </summary>
public static class VisibleDiscoveryCore
{
    // ===================== UpdateVisibleGay（31430-31474） =====================

    /// <summary>31435：异常消息格式。</summary>
    public static string UpdateVisibleGayErrorMsg(int errCode)
        => $"[Exception] TBaseObject.UpdateVisibleGay-->ErrCode={errCode}";

    /// <summary>
    /// 31439-31440：`m_boIsVisibleActive` 的置位条件——
    /// 对象是**玩家**，**或**"有主人的宝宝"（`m_Master <> nil`）。
    /// 注意本函数**只置 `True`、从不置 `False`**；`False` 由 31679 在每次入口统一置入。
    /// </summary>
    public static bool ShouldSetVisibleActive(int baseObjectRaceServer, bool baseObjectHasMaster)
        => baseObjectRaceServer == Grobal2Const.RC_PLAYOBJECT || baseObjectHasMaster;

    /// <summary>`UpdateVisibleGay` 的执行结果（含是否因置位而改变了 `m_boIsVisibleActive`）。</summary>
    public readonly record struct GayUpdateResult(
        bool Created, TVisibleBaseObject Entry, bool VisibleActiveSet);

    /// <summary>
    /// `UpdateVisibleGay`(31436-31474) 1:1。
    /// 条目不存在则**新建并置 `nVisibleFlag := 2`**（31452）；已存在且**引用相等**则置 **1**（31464）。
    /// 副作用：满足 31439 条件时把 `m_boIsVisibleActive` 置 `True`。
    /// </summary>
    public static GayUpdateResult UpdateVisibleGay(
        Dictionary<nint, TVisibleBaseObject> visibleActors,
        nint key, object baseObject, int baseObjectRaceServer, bool baseObjectHasMaster)
    {
        bool visibleActiveSet = false;

        // 31439-31440：先置位，再动表
        if (ShouldSetVisibleActive(baseObjectRaceServer, baseObjectHasMaster))
            visibleActiveSet = true;

        visibleActors.TryGetValue(key, out var existing);

        if (existing is null)
        {
            // 31450-31456：新建，**恒为 2**
            var entry = new TVisibleBaseObject
            {
                nVisibleFlag = VisibleItemLifecycleCore.FlagNewUnsent,
                BaseObject = baseObject,
            };
            visibleActors[key] = entry;

            return new GayUpdateResult(true, entry, visibleActiveSet);
        }

        // 31461-31464：引用相等才置 1
        if (ReferenceEquals(existing.BaseObject, baseObject))
            existing.nVisibleFlag = VisibleItemLifecycleCore.FlagVisible;

        return new GayUpdateResult(false, existing, visibleActiveSet);
    }

    // ===================== 角色准入判定（31845-31869） =====================

    /// <summary>`SearchViewRange` 中待判定对象的属性快照。</summary>
    public sealed class ActorViewArgs
    {
        public int RaceServer;
        public bool BoGhost;
        public bool BoFixedHideMode;
        public bool BoObMode;

        /// <summary>`TSmartObject.m_dwChangeModeExTick[1] > 0`，仅对三种限定种族读取。</summary>
        public uint ChangeModeExTick1;

        /// <summary>对象的 `m_Master` 是否**就是**判定者自己（`BaseObject.m_Master = Self`）。</summary>
        public bool MasterIsSelf;

        /// <summary>对象是否有主人。</summary>
        public bool HasMaster;

        /// <summary>31858-31859：对象的坐标，用于 ⑤ 的距离判定。</summary>
        public int CurX;
        public int CurY;

        /// <summary>31862-31863：对象国号。</summary>
        public int Nation;
    }

    /// <summary>`SearchViewRange` 判定者（自己）的属性快照。</summary>
    public sealed class SelfViewArgs
    {
        public int RaceServer;
        public int CurX;
        public int CurY;
        public int ViewRange;
        public int ViewRangeExtY;
        public int Nation;

        public bool HasMaster;
        public bool BoCrazyMode;
        public bool BoWantRefMsg;
        public bool BoNoSameNationMonPK;
    }

    /// <summary>
    /// 31846-31848：`boTempFixedHideMode` 的取值——**仅三种限定种族**读取限时隐身标记，
    /// 其余种族恒为 `False`（与 J101 `SendRefMsgCore.ComputeTempFixedHideMode` 同规则）。
    /// </summary>
    public static bool ComputeTempFixedHideMode(int raceServer, uint changeModeExTick1)
        => SendRefMsgCore.IsTempHideModeRace(raceServer) && changeModeExTick1 > 0;

    /// <summary>
    /// 31851-31852：**外层**准入——四个条件同时成立。
    /// 其中第 ④ 项的括号至关重要：**隐身对象只对其主人可见**。
    /// 参数 `sameEnvir` 对应 `BaseObject.m_PEnvir = m_PEnvir`。
    /// </summary>
    public static bool IsVisibleActorOuterGate(
        bool boGhost, bool boFixedHideMode, bool sameEnvir,
        bool boObMode, bool boTempFixedHideMode, bool masterIsSelf)
    {
        if (boGhost) return false;
        if (boFixedHideMode) return false;
        if (!sameEnvir) return false;

        // 31852：((not m_boObMode) and (not boTempFixedHideMode)) or (BaseObject.m_Master = Self)
        return (!boObMode && !boTempFixedHideMode) || masterIsSelf;
    }

    /// <summary>
    /// 31855-31863：**内层**准入——五个（实为七个）条件**任一**成立即可。
    /// `RC_ANIMAL` 为 50，故 `m_btRaceServer &lt; RC_ANIMAL` 涵盖玩家(0)/英雄(1)/守卫(11,12)/
    /// 和平NPC(15)/盒子(30) 等一切"低于动物"的种族。
    /// </summary>
    public static bool IsVisibleActorInnerGate(ActorViewArgs other, SelfViewArgs self)
    {
        // ① 31855：自身比动物"低阶"
        if (self.RaceServer < Grobal2Const.RC_ANIMAL)
            return true;

        // ② 31855：自身是宝宝
        if (self.HasMaster)
            return true;

        // ③ 31855：自身狂暴
        if (self.BoCrazyMode)
            return true;

        // ④ 31855：自身需要引用消息
        if (self.BoWantRefMsg)
            return true;

        // ⑤ 31858-31859：对方有主人，且**用自身的**视野门限判定距离
        if (other.HasMaster
            && Math.Abs(other.CurX - self.CurX) <= self.ViewRange
            && Math.Abs(other.CurY - self.CurY) <= self.ViewRange + self.ViewRangeExtY)
        {
            return true;
        }

        // ⑥ 31860：对方是玩家
        if (other.RaceServer == Grobal2Const.RC_PLAYOBJECT)
            return true;

        // ⑦ 31862-31863：不同国家怪物相互 PK
        if (self.Nation > 0 && other.Nation > 0 && self.BoNoSameNationMonPK
            && self.Nation != other.Nation)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 31845-31869：完整准入判定 = 外层四条件 **且** 内层任一条件。
    /// 命中则应调用 `UpdateVisibleGay`。
    /// </summary>
    public static bool ShouldRegisterVisibleActor(
        SelfViewArgs self, ActorViewArgs other, bool sameEnvir)
    {
        bool tempHide = ComputeTempFixedHideMode(other.RaceServer, other.ChangeModeExTick1);

        if (!IsVisibleActorOuterGate(
                other.BoGhost, other.BoFixedHideMode, sameEnvir,
                other.BoObMode, tempHide, other.MasterIsSelf))
        {
            return false;
        }

        return IsVisibleActorInnerGate(other, self);
    }

    // ===================== 物品准入判定（31871-31886） =====================

    /// <summary>
    /// 31874：物品发现的门槛——自身种族属于 `{RC_HEROOBJECT, RC_PLAYMOSTER}`
    /// **或** `IsSlavePickItem`。
    /// **`RC_PLAYOBJECT`（玩家自己）不在其中**：玩家的物品可见性由 J101 的
    /// `RM_ITEMSHOW` 广播建立，而非视野扫描——这正是 J102 中物品初值有 1/2 分支、
    /// 而对象初值恒为 2 的原因。
    /// </summary>
    public static bool ShouldScanItems(int selfRaceServer, bool isSlavePickItem)
        => selfRaceServer == Grobal2Const.RC_HEROOBJECT
           || selfRaceServer == Grobal2Const.RC_PLAYMOSTER
           || isSlavePickItem;

    /// <summary>
    /// 31879-31884：物品被登记的条件——**仅需非 `m_boGhost`**
    /// （比角色准入简单得多：不查同地图、不查隐身、不查国战）。
    /// 登记时第三参 `boSendItemShow` 取默认 `False`，故新建物品初值为 **2**。
    /// </summary>
    public static bool ShouldRegisterVisibleItem(bool itemGhost) => !itemGhost;
}
