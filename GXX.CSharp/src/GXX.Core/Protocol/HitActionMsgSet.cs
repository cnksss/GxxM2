using System;
using System.Collections.Generic;

namespace GXX.Core.Protocol;

/// <summary>
/// `CM_*` **攻击/动作消息集合**判定（批次J95）。
///
/// 该集合在服务端三处**以完全相同的成员列表**出现，是"这条动作消息是否算一次攻击/动作"的判据：
/// - `ObjPlayer.GetHitMsgCount`（14905-14930，统计 `m_MsgList` 中的攻击消息条数）；
/// - `ObjPlayer`（19901-19915，动作消息合并/去重时的分类）；
/// - `UsrEngn`（5729-5751，按限速配置选择 `SendMsg`/`SendUpdateMsg`/`SendUpdateMsgA` 下发）；
/// - `MirClientContext`（6684-6699，RunGate 侧限速计费时判定 `FLastAction`）。
///
/// **原文以"具名码列表 + 范围 case"两种写法表达同一集合**：`UsrEngn` 与 `MirClientContext`
/// 的 `$ELSE` 分支把范围 case 显式展开为
/// `(Ident >= CM_CUSTOM_HIT001) and (Ident < CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT)` 的
/// `or` 链——本类即按该展开式实现，故与两种写法都逐字等价。
///
/// **注意各调用点的成员列表并不完全一致**（原文如此，非笔误）：
/// - `GetHitMsgCount`(14919-14921) 含 `CM_HEAVYHIT`/`CM_BIGHIT`/`CM_POWERHIT`/`CM_LONGHIT`/
///   `CM_WIDEHIT`/`CM_FIREHIT`/`CM_TWNHIT`/`CM_43HIT`/`CM_66HIT`/`CM_66HIT1`/`CM_SWORDHIT`/
///   `CM_100HIT..CM_103HIT`/`CM_113HIT`/`CM_115HIT`，**不含** `CM_CRSHIT` 与 `CM_60HIT`/`CM_61HIT`/`CM_62HIT`；
/// - `MirClientContext`(6684-6687) **含** `CM_CRSHIT`，**含** `CM_101HIT..CM_103HIT` 但**不含** `CM_100HIT`；
/// - `UsrEngn`(5729-5736) **含** `CM_CRSHIT`，但**不含** `CM_100HIT`，且额外含移动族
///   `CM_HORSERUN`/`CM_TURN`/`CM_WALK`/`CM_RUN`。
/// 故本类提供 **三个独立集合**而非强行合并——合并会改变行为。
///
/// **本批次的一处自查修正**：`UsrEngn` 的成员表最初由我手工转录，**漏掉了 `CM_CRSHIT`**，
/// 且注释还断言"不含 CRSHIT"。随后用脚本从原文 `5729-5736` 独立抽取成员列表做交叉核对，
/// 才发现该错误并修正。教训：**大段成员表必须以脚本抽取 + 回读比对，不可依赖手工转录**。
/// </summary>
public static class HitActionMsgSet
{
    /// <summary>Grobal2.pas 60：自定义动作数量（范围宽度）。</summary>
    public const int CustomMagicCount = 300;

    /// <summary>Grobal2.pas 2215：自定义攻击动作起始码。</summary>
    public const int CM_CUSTOM_HIT001 = Grobal2Const.CM_CUSTOM_HIT001;

    /// <summary>
    /// 范围判据（`MirClientContext` 6699 的展开式）：
    /// `(Ident >= CM_CUSTOM_HIT001) and (Ident < CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT)`。
    /// 上界**不含**。
    /// </summary>
    public static bool InCustomHitRange(int ident)
        => ident >= CM_CUSTOM_HIT001 && ident < CM_CUSTOM_HIT001 + CustomMagicCount;

    // ===================== GetHitMsgCount（ObjPlayer 14919-14921） =====================

    /// <summary>
    /// `ObjPlayer.GetHitMsgCount`（14905-14930）的判据集合。
    /// **原文此处没有 `CM_CRSHIT`，也没有 `CM_60HIT`/`CM_61HIT`/`CM_62HIT`** —— 逐字保留。
    /// </summary>
    private static readonly int[] GetHitMsgCountSet =
    {
        Grobal2Const.CM_HIT,
        Grobal2Const.CM_HEAVYHIT,
        Grobal2Const.CM_BIGHIT,
        Grobal2Const.CM_POWERHIT,
        Grobal2Const.CM_LONGHIT,
        Grobal2Const.CM_WIDEHIT,
        Grobal2Const.CM_FIREHIT,
        Grobal2Const.CM_TWNHIT,
        Grobal2Const.CM_43HIT,
        Grobal2Const.CM_66HIT,
        Grobal2Const.CM_66HIT1,
        Grobal2Const.CM_SWORDHIT,
        Grobal2Const.CM_100HIT,
        Grobal2Const.CM_101HIT,
        Grobal2Const.CM_102HIT,
        Grobal2Const.CM_103HIT,
        Grobal2Const.CM_113HIT,
        Grobal2Const.CM_115HIT,
    };

    /// <summary>14918-14923：单条消息是否计入攻击消息数。</summary>
    public static bool IsHitMsgCountIdent(int ident)
        => Array.IndexOf(GetHitMsgCountSet, ident) >= 0 || InCustomHitRange(ident);

    /// <summary>
    /// `ObjPlayer.GetHitMsgCount`（14905-14930）1:1：统计消息列表中攻击消息条数。
    /// 原文在 `LockProcessMsg = 1` 下以临界区保护遍历，此处由调用方保证线程安全。
    /// </summary>
    public static int GetHitMsgCount(IReadOnlyList<int> msgIdents)
    {
        int result = 0;
        for (int i = 0; i < msgIdents.Count; i++)
        {
            if (IsHitMsgCountIdent(msgIdents[i]))
                result++;
        }
        return result;
    }

    // ===================== UsrEngn（5729-5751） =====================

    /// <summary>
    /// `UsrEngn` 5729-5736 的成员集合（用于选择下发方式）。
    /// **含 `CM_CRSHIT`，但不含 `CM_100HIT`** —— 逐字保留。
    /// </summary>
    private static readonly int[] UsrEngnSet =
    {
        Grobal2Const.CM_HORSERUN,
        Grobal2Const.CM_TURN,
        Grobal2Const.CM_WALK,
        Grobal2Const.CM_RUN,
        Grobal2Const.CM_HIT,
        Grobal2Const.CM_HEAVYHIT,
        Grobal2Const.CM_BIGHIT,
        Grobal2Const.CM_POWERHIT,
        Grobal2Const.CM_LONGHIT,
        Grobal2Const.CM_CRSHIT,
        Grobal2Const.CM_TWNHIT,
        Grobal2Const.CM_WIDEHIT,
        Grobal2Const.CM_FIREHIT,
        Grobal2Const.CM_43HIT,
        Grobal2Const.CM_66HIT,
        Grobal2Const.CM_66HIT1,
        Grobal2Const.CM_SWORDHIT,
        Grobal2Const.CM_101HIT,
        Grobal2Const.CM_102HIT,
        Grobal2Const.CM_103HIT,
        Grobal2Const.CM_113HIT,
        Grobal2Const.CM_115HIT,
    };

    /// <summary>5729-5737：该消息是否走"动作限速下发"支路。</summary>
    public static bool IsUsrEngnActionIdent(int ident)
        => Array.IndexOf(UsrEngnSet, ident) >= 0 || InCustomHitRange(ident);

    /// <summary>
    /// 5739-5750：按限速配置选择下发方式。
    /// 优先级：`boSpeedControl and boSendUpdateMsg` → `A` 式；
    /// 否则 `boSpeedControl and boActionSendActionMsg` → 普通 `UpdateMsg`；
    /// 否则回落普通 `SendMsg`。**两个开关都必须与 `boSpeedControl` 同时成立。**
    /// </summary>
    public static HitSendMode SelectUsrEngnSendMode(
        bool boSpeedControl, bool boSendUpdateMsg, bool boActionSendActionMsg)
    {
        if (boSpeedControl && boSendUpdateMsg)
            return HitSendMode.SendUpdateMsgA;
        if (boSpeedControl && boActionSendActionMsg)
            return HitSendMode.SendUpdateMsg;
        return HitSendMode.SendMsg;
    }

    // ===================== MirClientContext（6684-6688） =====================

    /// <summary>
    /// `MirClientContext` 6684-6687 的成员集合（RunGate 限速计费）。
    /// **含 `CM_CRSHIT`，含 `CM_101HIT..CM_103HIT`，但不含 `CM_100HIT`** —— 逐字保留。
    /// 也**不含** `CM_HORSERUN`/`CM_TURN`/`CM_WALK`/`CM_RUN`（那些在 6683 之前的其它分支处理）。
    /// </summary>
    private static readonly int[] RunGateSet =
    {
        Grobal2Const.CM_HIT,
        Grobal2Const.CM_HEAVYHIT,
        Grobal2Const.CM_BIGHIT,
        Grobal2Const.CM_POWERHIT,
        Grobal2Const.CM_LONGHIT,
        Grobal2Const.CM_WIDEHIT,
        Grobal2Const.CM_FIREHIT,
        Grobal2Const.CM_CRSHIT,
        Grobal2Const.CM_TWNHIT,
        Grobal2Const.CM_SWORDHIT,
        Grobal2Const.CM_43HIT,
        Grobal2Const.CM_66HIT,
        Grobal2Const.CM_66HIT1,
        Grobal2Const.CM_101HIT,
        Grobal2Const.CM_102HIT,
        Grobal2Const.CM_103HIT,
        Grobal2Const.CM_113HIT,
        Grobal2Const.CM_115HIT,
    };

    /// <summary>6684-6699：该消息在 RunGate 侧是否计入攻击限速。</summary>
    public static bool IsRunGateActionIdent(int ident)
        => Array.IndexOf(RunGateSet, ident) >= 0 || InCustomHitRange(ident);
}

/// <summary>5739-5750：动作消息的下发方式。</summary>
public enum HitSendMode
{
    /// <summary>普通 `SendMsg`。</summary>
    SendMsg = 0,

    /// <summary>`SendUpdateMsg`（防止消息队列里有多个操作）。</summary>
    SendUpdateMsg = 1,

    /// <summary>`SendUpdateMsgA`。</summary>
    SendUpdateMsgA = 2,
}
