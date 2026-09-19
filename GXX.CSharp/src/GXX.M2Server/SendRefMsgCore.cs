using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.M2Server;

/// <summary>
/// ObjBase.pas `TBaseObject.SendRefMsg`(30980-31428) 的**决策层** 1:1 移植（批次J101）。
///
/// 该函数是服务端最大、最热的广播函数（整个 449 行），本批次移植其中**纯决策部分**：
/// 谁应该收到这条引用消息、以什么方式发出、以及"可见人类列表"(`m_VisibleHumanList`) 的
/// 重建与复用策略。实际的 `SendMsg`/`SendDelayMsg` 落地由调用方通过委托完成
/// （原文经由 socket 发送，C# 侧为可注入接缝）。
///
/// **本函数是三段式结构（原文用 `Exit` 分隔），三段的可见性规则各不相同**：
/// 1. **隐身段（31043-31102）**：`m_boObMode` / `m_boFixedHideMode` / `boTempFixedHideMode` 之一为真时，
///    只发给"自己"（限 `RC_PLAYOBJECT`）与"主人"（限 `RC_HEROOBJECT`），并在 `ClientConfigs[73]`
///    开启时补发给视野里看不到自己的队友——**本段直接 `Exit`，不走视野扫描**。
/// 2. **视野扫描段（31108-31275）**：当"距上次重建 ≥ 100ms"或"可见列表为空"或"广播范围已变"时，
///    重建 `m_VisibleHumanList`；扫描范围 X 为 `±g_nSendRefMsgRange`，Y 为
///    `±(g_nSendRefMsgRange + C_VIEWRANGEEXTY)`（**Y 比 X 多一个扩展量**）。
/// 3. **列表复用段（31331-31422）**：未触发重建时，直接遍历上次缓存下来的 `m_VisibleHumanList`。
///
/// **`C_VIEWRANGEEXTY` 当前取值为 0**（M2Share.pas 中原文写作 `C_VIEWRANGEEXTY = 0; // 9`），
/// 即所有 `+ m_nViewRangeExtY` / `+ C_VIEWRANGEEXTY` 项**目前均不产生实际偏移**，
/// 但本移植**保留这些项的结构**——原文注释表明它曾为 9，一旦改回即需生效。
///
/// **`g_Config.ClientConfigs[73]` 在原文中即 `boShowNewGroupInfo`**（M2Share.pas），
/// 本移植以此为参数名，避免调用方误读为"某个编号 73 的开关"。
/// </summary>
public static class SendRefMsgCore
{
    // ===================== 常量（M2Share.pas） =====================

    /// <summary>`C_VIEWRANGEEXTY = 0`（原文注释 `// 9`）。</summary>
    public const int C_VIEWRANGEEXTY = 0;

    /// <summary>31108：可见列表重建的最小间隔（毫秒）。</summary>
    public const int VisibleListRebuildInterval = 100;

    /// <summary>31164：对象加入地图后多久内不参与引用状态广播（毫秒）。</summary>
    public const uint RefStatusGraceMs = 60 * 1000;

    // ===================== 成员集合（脚本自原文独立抽取核对） =====================

    /// <summary>
    /// 31070-31075 / 31297-31302 / 31392-31397：**队伍补充广播**的消息集合，
    /// 三处出现**完全相同**，共 6 个码。
    /// 命中且 `boShowNewGroupInfo` 开启时，会把消息补发给"看不到自己"的队友。
    /// </summary>
    public static readonly int[] GroupNotifySet =
    {
        Grobal2Const.RM_HEALTHSPELLCHANGED,
        Grobal2Const.RM_HEALTHSPELLCHANGED_STRUCK,
        Grobal2Const.RM_HPMPCHANGED_FORM_STONE,
        Grobal2Const.RM_MAGICSHIELD_STRUCK,
        Grobal2Const.RM_STRUCK,
        Grobal2Const.RM_STRUCK_MAG,
    };

    /// <summary>
    /// 31237-31241 / 31373-31377：**非玩家对象**（`m_boWantRefMsg` 为真）愿意接收的消息集合，
    /// 两处出现**完全相同**，共 5 个码。原文注释"增加分身的魔法盾效果"。
    /// </summary>
    public static readonly int[] WantRefMsgSet =
    {
        Grobal2Const.RM_STRUCK,
        Grobal2Const.RM_HEAR,
        Grobal2Const.RM_DEATH,
        Grobal2Const.RM_CHARSTATUSCHANGED,
        Grobal2Const.RM_RUSH,
    };

    public static bool IsGroupNotifyIdent(int wIdent) => Array.IndexOf(GroupNotifySet, wIdent) >= 0;

    public static bool IsWantRefMsgIdent(int wIdent) => Array.IndexOf(WantRefMsgSet, wIdent) >= 0;

    // ===================== 隐身判定（31036-31043） =====================

    /// <summary>
    /// 31039-31040：**只有** `RC_PLAYOBJECT` / `RC_HEROOBJECT` / `RC_PLAYMOSTER` 三种种族
    /// 才读取"NPC 命令限时隐身"标记；其余种族该标记恒为 `false`（31036 的初值）。
    /// </summary>
    public static bool IsTempHideModeRace(int raceServer)
        => raceServer == Grobal2Const.RC_PLAYOBJECT
           || raceServer == Grobal2Const.RC_HEROOBJECT
           || raceServer == Grobal2Const.RC_PLAYMOSTER;

    /// <summary>
    /// 31036-31040：综合隐身标记。`ChangeModeExTick[1] > 0` 表示限时隐身生效中。
    /// </summary>
    public static bool ComputeTempFixedHideMode(int raceServer, uint changeModeExTick1)
        => IsTempHideModeRace(raceServer) && changeModeExTick1 > 0;

    /// <summary>31043：三种隐身模式任一为真即进入"只发给自己"分支。</summary>
    public static bool IsSelfOnlyMode(bool boObMode, bool boFixedHideMode, bool boTempFixedHideMode)
        => boObMode || boFixedHideMode || boTempFixedHideMode;

    // ===================== 可见性判定 =====================

    /// <summary>
    /// 31196-31199 / 31229-31232 / 31337-31339：**扫描段与复用段共用的可见判定**——
    /// 同一地图，且 X 偏移 ≤ 对方 `m_nViewRange`，Y 偏移 ≤ 对方 `m_nViewRange + m_nViewRangeExtY`。
    /// **注意 Y 的门限含对方的 `m_nViewRangeExtY`（而非自己的）**。
    /// </summary>
    public static bool IsInViewRange(
        bool sameEnvir, int otherX, int otherY, int selfX, int selfY,
        int otherViewRange, int otherViewRangeExtY)
    {
        if (!sameEnvir) return false;

        return Math.Abs(otherX - selfX) <= otherViewRange
               && Math.Abs(otherY - selfY) <= otherViewRange + otherViewRangeExtY;
    }

    /// <summary>
    /// 31248-31251：非玩家对象的**专用**可见判定——门限取
    /// `Max(对方 m_nViewRange, 自己 m_nViewRange)`，且 Y 侧用的是**自己的** `m_nViewRangeExtY`。
    /// **与 <see cref="IsInViewRange"/> 不同**（后者用对方的 ExtY），二者不可互换。
    /// </summary>
    public static bool IsInViewRangeForWantRef(
        bool sameEnvir, int otherX, int otherY, int selfX, int selfY,
        int otherViewRange, int selfViewRange, int selfViewRangeExtY)
    {
        if (!sameEnvir) return false;

        int nViewRange = Math.Max(otherViewRange, selfViewRange);
        return Math.Abs(otherX - selfX) <= nViewRange
               && Math.Abs(otherY - selfY) <= nViewRange + selfViewRangeExtY;
    }

    /// <summary>
    /// 31164：`(not m_boDenyRefStatus) and ((now - obj.m_dwAddTime) >= 60000)`。
    /// 为真表示该对象**尚未满 60 秒**（或用 `m_boDenyRefStatus` 关掉了宽限），
    /// 此时走 31186 的 `else` 分支正常广播；为假则**跳过广播**。
    /// 原文该分支体内是整段被注释掉的死代码，故此处只表达条件。
    /// </summary>
    public static bool IsInRefStatusGrace(bool boDenyRefStatus, uint now, uint objAddTime)
        => !boDenyRefStatus && (now - objAddTime) >= RefStatusGraceMs;

    // ===================== 掉落物品的二次判定（31203-31218） =====================

    /// <summary>
    /// 31203-31218：`RM_ITEMSHOW` / `RM_ITEMHIDE` 时**不按消息参数里的物品坐标去比对方视野**，
    /// 而是把**观察者自身坐标** `otherX`/`otherY` 与**物品坐标** `nParam2`/`nParam3` 相减判定；
    /// 其他消息恒为 `true`。
    /// 原文注释：修正掉落装备会发 2 次 `RM_ITEMSHOW` 的问题。
    ///
    /// **即比较的是"观察者 vs 物品"，而非"物品 vs 物品"或"观察者 vs 观察者"。**
    /// 注意此处与 <see cref="IsInViewRange"/> 的差别：后者比较的是两个对象坐标，
    /// 本函数则把一个对象坐标换成了消息携带的物品坐标。
    /// </summary>
    public static bool ShouldSendItemMessage(
        int wIdent, int otherX, int otherY, nint nParam2, nint nParam3,
        int otherViewRange, int otherViewRangeExtY)
    {
        if (wIdent != Grobal2Const.RM_ITEMSHOW && wIdent != Grobal2Const.RM_ITEMHIDE)
            return true;

        return Math.Abs(otherX - (int)nParam2) <= otherViewRange
               && Math.Abs(otherY - (int)nParam3) <= otherViewRange + otherViewRangeExtY;
    }

    // ===================== ProcessRMMsg（30988-31009） =====================

    /// <summary>`ProcessRMMsg` 的改写结果。</summary>
    public readonly record struct RmMsgRewrite(
        int WIdent, nint WParam, nint NParam1, nint NParam2, nint NParam3, string SMsg);

    /// <summary>
    /// 30988-31009：发送前对参数的就地改写。**三条分支互斥且按序判定**：
    /// ① `RM_FEATURECHANGED` → `wParam` 换成预计算的 `Feature_wParam`，三个 nParam 清零，
    ///    `sMsg` 换成预计算的 `Feature_sMsg`；
    /// ② `RM_ITEMSHOW` → `sMsg` 恢复为备份的原串（兼容老端，piaoyun 2013-09-07）；
    /// ③ `RM_HEROLOGON` 或 `RM_MYHEROLOGON` → 按"是否为我的主人"改写为
    ///    `RM_MYHEROLOGON` 或 `RM_HEROLOGON`。
    ///
    /// **注意 ① 会清零 nParam1/2/3**：若调用方在 ① 之后仍需要原参数（如 `RM_ITEMSHOW` 的物品坐标），
    /// 会拿到 0——但原文三分支互斥，`RM_FEATURECHANGED` 与 `RM_ITEMSHOW` 不会同时命中，故安全。
    /// </summary>
    public static RmMsgRewrite ProcessRMMsg(
        int wIdent, nint wParam, nint nParam1, nint nParam2, nint nParam3, string sMsg,
        int featureWParam, string featureSMsg, string backupMsg, bool isMasterIsPlayer)
    {
        if (wIdent == Grobal2Const.RM_FEATURECHANGED)
        {
            return new RmMsgRewrite(wIdent, featureWParam, 0, 0, 0, featureSMsg);
        }

        if (wIdent == Grobal2Const.RM_ITEMSHOW)
        {
            return new RmMsgRewrite(wIdent, wParam, nParam1, nParam2, nParam3, backupMsg);
        }

        if (wIdent == Grobal2Const.RM_HEROLOGON || wIdent == Grobal2Const.RM_MYHEROLOGON)
        {
            int rewritten = isMasterIsPlayer
                ? Grobal2Const.RM_MYHEROLOGON
                : Grobal2Const.RM_HEROLOGON;
            return new RmMsgRewrite(rewritten, wParam, nParam1, nParam2, nParam3, sMsg);
        }

        return new RmMsgRewrite(wIdent, wParam, nParam1, nParam2, nParam3, sMsg);
    }

    /// <summary>
    /// 31048-31049：**英雄**在隐身模式下收到 `RM_DISAPPEAR` 且 `wParam = 65535` 时**直接返回**，
    /// 连自己都不发。原文注释：`H.ChangeModeEx 2 1000 主人看不到英雄`。
    /// </summary>
    public static bool ShouldExitOnDisappear(int raceServer, int wIdent, nint wParam)
        => raceServer == Grobal2Const.RC_HEROOBJECT
           && wIdent == Grobal2Const.RM_DISAPPEAR
           && wParam == 65535;

    // ===================== 可见列表重建判定（31108-31114） =====================

    /// <summary>
    /// 31108-31110：三个条件任一成立即重建 `m_VisibleHumanList`：
    /// ① 距上次重建 **≥ 100ms**；② 可见列表**为空**；③ **广播范围已变化**。
    /// 否则复用上次的列表（31331 段）。
    /// </summary>
    public static bool ShouldRebuildVisibleList(
        uint now, uint sendRefMsgTick, int visibleCount, int nSendRefMsgRange, int gSendRefMsgRange)
        => (now - sendRefMsgTick) >= VisibleListRebuildInterval
           || visibleCount == 0
           || nSendRefMsgRange != gSendRefMsgRange;

    /// <summary>
    /// 31115-31118：扫描范围。X 为 `±g_nSendRefMsgRange`；
    /// **Y 为 `±(g_nSendRefMsgRange + C_VIEWRANGEEXTY)`**——Y 侧多一个扩展量（当前为 0）。
    /// </summary>
    public static (int LX, int HX, int LY, int HY) ComputeScanBounds(
        int currX, int currY, int gSendRefMsgRange)
    {
        int lx = currX - gSendRefMsgRange;
        int hx = currX + gSendRefMsgRange;
        int ly = currY - gSendRefMsgRange - C_VIEWRANGEEXTY;
        int hy = currY + gSendRefMsgRange + C_VIEWRANGEEXTY;

        return (lx, hx, ly, hy);
    }

    // ===================== 队伍补充广播（31066-31100 / 31293-31327 / 31388-31422） =====================

    /// <summary>
    /// 31066-31075（及 31293、31388 两处完全相同的判定）：是否执行"补发给看不到自己的队友"。
    /// 五个条件缺一不可，其中 `groupOwner` 与自身都必须是 `RC_PLAYOBJECT`。
    /// </summary>
    public static bool ShouldSendToGroupOwnerMembers(
        bool boShowNewGroupInfo, int selfRaceServer, bool hasGroupOwner,
        int groupOwnerRaceServer, int wIdent)
        => boShowNewGroupInfo
           && selfRaceServer == Grobal2Const.RC_PLAYOBJECT
           && hasGroupOwner
           && groupOwnerRaceServer == Grobal2Const.RC_PLAYOBJECT
           && IsGroupNotifyIdent(wIdent);

    /// <summary>
    /// 31086 / 31313 / 31408：队友是否应收到补充广播——
    /// **非空、非自己、且其 `m_VisibleHumanList` 中不含自己**（即对方看不到我）。
    /// 原文判断顺序即 `(PlayObject &lt;&gt; nil) and (PlayObject &lt;&gt; Self) and (IndexOf(Self) = -1)`。
    /// </summary>
    public static bool ShouldSendToGroupMember(bool isNull, bool isSelf, bool seesSelf)
        => !isNull && !isSelf && !seesSelf;

    // ===================== 英雄主人在远处的补偿广播（31279-31291） =====================

    /// <summary>
    /// 31279-31284：英雄在其主人**看不到自己**时，仍需把自己的状态发给主人。
    /// 条件：自己是英雄、有主人、主人是玩家，且三者之一成立——
    /// 不同地图、X 距离超主人视野、或 Y 距离超主人视野+自己的 ExtY。
    /// **注意 Y 侧用的是英雄自己的 `m_nViewRangeExtY`**（31284）。
    /// </summary>
    public static bool ShouldSendHeroStateToMaster(
        int selfRaceServer, bool hasMaster, int masterRaceServer,
        bool sameEnvir, int masterX, int masterY, int selfX, int selfY,
        int masterViewRange, int selfViewRangeExtY)
    {
        if (selfRaceServer != Grobal2Const.RC_HEROOBJECT) return false;
        if (!hasMaster) return false;
        if (masterRaceServer != Grobal2Const.RC_PLAYOBJECT) return false;

        return !sameEnvir
               || Math.Abs(masterX - selfX) > masterViewRange
               || Math.Abs(masterY - selfY) > masterViewRange + selfViewRangeExtY;
    }
}
