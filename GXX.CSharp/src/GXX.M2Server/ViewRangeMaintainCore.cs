using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.M2Server;

/// <summary>
/// ObjBase.pas 视野维护三函数 1:1 移植（批次J103）：
/// `TBaseObject.ClearObject`(31537-31592)、`TBaseObject.CheckMasterViewRange`(31594-31612)、
/// `TBaseObject.SearchViewRange`(31614-31992) 的**决策层**。
///
/// `SearchViewRange` 是每帧为每个玩家对象执行的视野维护主循环（约 380 行），
/// 本批次移植其：**离线闸门**、**奴隶拾取开关判定**、**扫描边界**、
/// **宽限期跳过**、以及**帧末两类清理**。扫描内层的"发现新对象"动作
/// （`AddVisibleObject`/`UpdateVisibleItem` 等）由 J102 的状态机承担。
///
/// **`ClearObject`（31537-31592）逐字要点**：三类表各自**先判 Count > 0 才加锁**
/// （31545/31553/31573），然后 `for I := 0 to Count - 1` 逐个 `Dispose`，最后 `Clear`。
/// **遍历用的是正序 `to` 而非倒序 `downto`**（31558/31578）——因为这里只释放元素、
/// 不在循环中删除元素，故正序是安全的；而 31944/31972 的**清理**是在循环中删除，
/// 必须倒序。同一文件内两种遍历方向各有其因，不可互换。
/// 三个 `except` 分别打印序号 **1 / 2 / 3**（31550/31567/31586）。
/// `TPlayObject.ClearObject`(ObjPlayer.pas 11366-11387) 先调 `inherited` 再做自己的
/// `m_VisibleEvents` 清理（用 `HashList` 正序游标遍历、逐个 `Dispose`、最后 `Clear`）。
///
/// **`CheckMasterViewRange`（31594-31612）逐字要点**：**默认返回 `True`**（31601）——
/// 即"没有主人"或"主人不在同一地图"时视为**在**可视范围内。
/// 仅当 `m_Master <> nil` 且**同一地图**时，才按主人的 `m_nViewRange` 计算矩形并判定。
/// **两个方向的门限都用主人的 `m_nViewRange`，没有 `m_nViewRangeExtY`**
/// （31604-31607）——这与 `SearchViewRange`（31736 的 Y 侧**加了** `ExtY`）
/// 和 `IsInViewRange`（用对方的 `ExtY`）都不同，三者是三套独立规则。
/// 31609/31610 两行是**被注释掉的等价改写**（其中 31609 那行其实是**取反**的旧写法），
/// 本移植保留其注释痕迹以示行为曾变过。
///
/// **`SearchViewRange` 的三道离线闸门（31643-31671）**：① 自身 `m_PEnvir = nil` → 直接 `Exit`；
/// ② 自身 `m_boOffLine` → `ClearObject` 后 `Exit`；
/// ③ 自己是英雄且主人离线 → `ClearObject` 后 `Exit`；
/// ④ 自己有主人、主人是玩家、且主人离线 → `ClearObject` 后 `Exit`。
/// **注意 ②③④ 都清空可见列表**——即"离线者不保留任何可见缓存"，
/// 这样重新上线时从零重建，避免显示离线期间残留的对象。
///
/// **扫描边界有一处不对称（31733-31736）**：X 为 `±m_nViewRange`，
/// **Y 的下界是 `-m_nViewRange`，上界却是 `+m_nViewRange + m_nViewRangeExtY`**
/// ——即 Y 方向**只向上多扩展**，向下不扩展。这与 `SendRefMsgCore.ComputeScanBounds`
/// （X/Y 都按 `g_nSendRefMsgRange`，Y 两侧都加 `C_VIEWRANGEEXTY`）**不同**。
/// 当前 `ExtY` 未在客户端配置中放大时该差异可能不显，但语义上二者不可互套。
/// </summary>
public static class ViewRangeMaintainCore
{
    // ===================== ClearObject（31537-31592） =====================

    /// <summary>31550/31567/31586：`ClearObject` 三个 except 的序号。</summary>
    public const int ClearObjectErrorHumanList = 1;
    public const int ClearObjectErrorActors = 2;
    public const int ClearObjectErrorItems = 3;

    /// <summary>`ClearObject` 的异常消息格式：`'[Exception] TBaseObject.ClearObject %d'`。</summary>
    public static string ClearObjectErrorMsg(int code) => $"[Exception] TBaseObject.ClearObject {code}";

    /// <summary>
    /// 31545-31552：`m_VisibleHumanList` 清理。**先判 Count > 0 才动**（31545），
    /// 仅 `Clear`，无需逐个释放（31648）。
    /// </summary>
    public static void ClearVisibleHumanList<T>(IList<T> list, Action<string>? onError = null)
    {
        if (list.Count <= 0) return;

        try
        {
            list.Clear();
        }
        catch (Exception)
        {
            onError?.Invoke(ClearObjectErrorMsg(ClearObjectErrorHumanList));
        }
    }

    /// <summary>
    /// 31553-31572：`m_VisibleActors` 清理。**正序 `to` 遍历**逐个释放再 `Clear`
    /// （31558 用的是 `to` 而非 `downto`，因不删元素故安全）。
    /// </summary>
    public static void ClearVisibleActors<T>(
        IList<T> list, Action<T> dispose, Action<string>? onError = null)
    {
        if (list.Count <= 0) return;

        try
        {
            for (int i = 0; i <= list.Count - 1; i++)
            {
                var v = list[i];
                if (v is not null)
                    dispose(v);
            }

            list.Clear();
        }
        catch (Exception)
        {
            onError?.Invoke(ClearObjectErrorMsg(ClearObjectErrorActors));
        }
    }

    /// <summary>31573-31591：`m_VisibleItems` 清理，规则同 Actors，序号为 3。</summary>
    public static void ClearVisibleItems<T>(
        IList<T> list, Action<T> dispose, Action<string>? onError = null)
    {
        if (list.Count <= 0) return;

        try
        {
            for (int i = 0; i <= list.Count - 1; i++)
            {
                var v = list[i];
                if (v is not null)
                    dispose(v);
            }

            list.Clear();
        }
        catch (Exception)
        {
            onError?.Invoke(ClearObjectErrorMsg(ClearObjectErrorItems));
        }
    }

    /// <summary>11371：`TPlayObject.ClearObject` 的异常消息。</summary>
    public const string PlayObjectClearObjectErrorMsg = "[Exception] TBaseObject.ClearObject";

    /// <summary>
    /// ObjPlayer.pas 11374-11386：`m_VisibleEvents`（哈希表）清理——
    /// **正序游标遍历**逐个释放，最后 `Clear`。
    /// </summary>
    public static void ClearVisibleEvents<T>(
        IList<T> events, Action<T> dispose, Action<string>? onError = null)
    {
        try
        {
            foreach (var v in events)
            {
                if (v is not null)
                    dispose(v);
            }

            events.Clear();
        }
        catch (Exception)
        {
            onError?.Invoke(PlayObjectClearObjectErrorMsg);
        }
    }

    // ===================== CheckMasterViewRange（31594-31612） =====================

    /// <summary>
    /// 31594-31612：检测自身是否在主人可视范围内。
    /// **默认 `True`**——无主人或不同地图时视为在范围内。
    /// 门限**两侧都用主人的 `m_nViewRange`，不加 `ExtY`**。
    /// </summary>
    public static bool CheckMasterViewRange(
        bool hasMaster, bool sameEnvir,
        int masterX, int masterY, int masterViewRange,
        int selfX, int selfY)
    {
        if (!hasMaster || !sameEnvir)
            return true;      // 31601：默认 True

        int nStartX = masterX - masterViewRange;
        int nEndX = masterX + masterViewRange;
        int nStartY = masterY - masterViewRange;
        int nEndY = masterY + masterViewRange;

        return selfX >= nStartX && selfX <= nEndX
               && selfY >= nStartY && selfY <= nEndY;
    }

    // ===================== SearchViewRange（31614-31992） =====================

    /// <summary>31635-31638：四条异常消息格式。</summary>
    public static string SearchExceptionMsg1(int code) => $"[Exception] TBaseObject.SearchViewRange Code:{code}";

    public static string SearchExceptionMsg2(int code, string name, string map, int x, int y, int c)
        => $"[Exception] TBaseObject.SearchViewRange 1-{code} {name} {map} {x} {y} {c}";

    public static string SearchExceptionMsg3(int code, string name, string map, int x, int y, int c)
        => $"[Exception] TBaseObject.SearchViewRange 2-{code} {name} {map} {x} {y} {c}";

    public static string SearchExceptionMsg4(int code, string name, string map, int x, int y, int c)
        => $"[Exception] TBaseObject.SearchViewRange 3-{code} {name} {map} {x} {y} {c}";

    /// <summary>`SearchViewRange` 应执行的动作。</summary>
    public enum ViewRangeAction
    {
        /// <summary>31646：`m_PEnvir = nil`，打印后直接退出（**不清空**）。</summary>
        ExitNilEnvir,

        /// <summary>31653/31661/31669：离线，先 `ClearObject` 再退出。</summary>
        ClearAndExit,

        /// <summary>正常继续扫描。</summary>
        Proceed,
    }

    /// <summary>
    /// 31643-31671：三道离线闸门。
    /// ① 自身 `m_PEnvir = nil` → `ExitNilEnvir`（**注意此分支不清空可见列表**）；
    /// ② 自身离线 → `ClearAndExit`；
    /// ③ 自己是英雄且主人离线 → `ClearAndExit`；
    /// ④ 有主人、主人是玩家、主人离线 → `ClearAndExit`。
    /// **判定按 ①②③④ 顺序短路**，故自身离线优先于主人离线。
    /// </summary>
    public static ViewRangeAction SelectStartupAction(
        bool penvirIsNull,
        bool selfOffline, int selfRaceServer,
        bool hasMaster, bool masterOffline, int masterRaceServer)
    {
        if (penvirIsNull)
            return ViewRangeAction.ExitNilEnvir;      // 31643-31647

        if (selfOffline)
            return ViewRangeAction.ClearAndExit;      // 31650-31655

        if (selfRaceServer == Grobal2Const.RC_HEROOBJECT && hasMaster && masterOffline)
            return ViewRangeAction.ClearAndExit;      // 31658-31663

        if (hasMaster && masterRaceServer == Grobal2Const.RC_PLAYOBJECT && masterOffline)
            return ViewRangeAction.ClearAndExit;      // 31666-31671

        return ViewRangeAction.Proceed;
    }

    /// <summary>
    /// 31685-31694：`IsSlavePickItem` 判定。两分支互斥：
    /// **`m_boGamePet` 为真** → 主人必须是玩家，且
    /// `(m_btGamePetEnablePick = 0 且 g_Config.boEnabledPetPickup)` **或** `m_btGamePetEnablePick = 1`；
    /// **为假** → `not m_PEnvir.m_boNoAutoRangePickItem` 且 `m_boSlaveAutoPickItem`
    /// 且 `g_nKey_UseClientPickItems &lt;&gt; 0`。
    /// 两者都要求"有主人且主人种族属于 {玩家, 英雄, 宠物主人}"这一**外层前提**（31682）。
    /// </summary>
    public static bool ComputeIsSlavePickItem(
        bool hasMaster, int masterRaceServer,
        bool boGamePet, int btGamePetEnablePick, bool boEnabledPetPickup,
        bool envirNoAutoRangePickItem, bool boSlaveAutoPickItem, int nKeyUseClientPickItems)
    {
        // 31682：外层前提
        bool masterEligible = hasMaster
            && (masterRaceServer == Grobal2Const.RC_PLAYOBJECT
                || masterRaceServer == Grobal2Const.RC_HEROOBJECT
                || masterRaceServer == Grobal2Const.RC_PLAYMOSTER);

        if (!masterEligible)
            return false;

        if (boGamePet)
        {
            // 31686-31687
            return masterRaceServer == Grobal2Const.RC_PLAYOBJECT
                   && ((btGamePetEnablePick == 0 && boEnabledPetPickup) || btGamePetEnablePick == 1);
        }

        // 31692-31693
        return !envirNoAutoRangePickItem
               && boSlaveAutoPickItem
               && nKeyUseClientPickItems != 0;
    }

    /// <summary>
    /// 31698：是否需要维护"物品可见性"。
    /// 自身种族属于 {玩家, 英雄, 宠物主人}，**或** `IsSlavePickItem` 为真。
    /// </summary>
    public static bool ShouldMaintainVisibleItems(int selfRaceServer, bool isSlavePickItem)
        => selfRaceServer == Grobal2Const.RC_PLAYOBJECT
           || selfRaceServer == Grobal2Const.RC_HEROOBJECT
           || selfRaceServer == Grobal2Const.RC_PLAYMOSTER
           || isSlavePickItem;

    /// <summary>
    /// 31733-31736：扫描边界。**Y 方向只向上扩展**——
    /// 下界 `-m_nViewRange`，上界 `+m_nViewRange + m_nViewRangeExtY`。
    /// 与 <see cref="SendRefMsgCore.ComputeScanBounds"/>（Y 两侧都加）**不同**。
    /// </summary>
    public static (int StartX, int EndX, int StartY, int EndY) ComputeSearchBounds(
        int currX, int currY, int nViewRange, int nViewRangeExtY)
    {
        int startX = currX - nViewRange;
        int endX = currX + nViewRange;
        int startY = currY - nViewRange;
        int endY = currY + nViewRange + nViewRangeExtY;

        return (startX, endX, startY, endY);
    }

    /// <summary>
    /// 31796：宽限期判定——`(not m_boDenyRefStatus) and ((now - m_dwAddTime) >= 60000)`。
    /// **与 <see cref="SendRefMsgCore.IsInRefStatusGrace"/> 同名同义**（两处原文一致），
    /// 此处独立暴露以便 SearchViewRange 侧使用并单独固化。
    /// </summary>
    public static bool IsInRefStatusGrace(bool boDenyRefStatus, uint now, uint addTime)
        => !boDenyRefStatus && (now - addTime) >= SendRefMsgCore.RefStatusGraceMs;

    /// <summary>31705/31721：帧首把 Actors 与 Items 的条目全部置为 `FlagStale`(0)。</summary>
    public static void MarkVisibleAllStale<TActor, TItem>(
        IEnumerable<TActor> actors, Action<TActor, byte> setA,
        IEnumerable<TItem> items, Action<TItem, byte> setI)
    {
        foreach (var a in actors)
            if (a is not null) setA(a, VisibleItemLifecycleCore.FlagStale);

        foreach (var i in items)
            if (i is not null) setI(i, VisibleItemLifecycleCore.FlagStale);
    }

    /// <summary>
    /// 31944-31955：帧末清理 `m_VisibleActors`——**倒序 `downto 0`**，
    /// 凡 `nVisibleFlag = 0` 者删除并释放。
    /// </summary>
    public static void FinishVisibleActors<T>(
        IList<T> list, Func<T, byte> getFlag, Action<int> removeAt, Action<T> dispose)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            var v = list[i];
            if (v is not null && getFlag(v) == VisibleItemLifecycleCore.FlagStale)
            {
                removeAt(i);
                dispose(v);
            }
        }
    }

    /// <summary>
    /// 31967-31984：帧末清理 `m_VisibleItems`，**仅当** `ShouldMaintainVisibleItems`
    /// 为真时执行（31968）；同样**倒序**。
    /// </summary>
    public static void FinishVisibleItems<T>(
        bool shouldMaintain, IList<T> list,
        Func<T, byte> getFlag, Action<int> removeAt, Action<T> dispose)
    {
        if (!shouldMaintain)
            return;

        for (int i = list.Count - 1; i >= 0; i--)
        {
            var v = list[i];
            if (v is not null && getFlag(v) == VisibleItemLifecycleCore.FlagStale)
            {
                removeAt(i);
                dispose(v);
            }
        }
    }

    /// <summary>31679：进入扫描前先把 `m_boIsVisibleActive` 置 False。</summary>
    public const bool BoIsVisibleActiveOnEntry = false;
}
