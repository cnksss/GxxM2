using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.M2Server;

/// <summary>
/// ObjBase.pas 消息队列家族的**余下三个成员** 1:1 移植（批次J98）：
/// `SendUpdateBaseObject`(30608-30648)、`SendUseItemMsg`(30650-30685)、`SendActionMsg`(30687-30723)。
///
/// 加上 J96 的 `SendUpdateMsgA`/`SendUpdateMsg` 与 J97 的 `SendUpdateDelayMsg`，
/// 本项目已覆盖 `m_MsgList` 相关的全部 6 个操作函数。
///
/// **本家族的删除条件各不相同（原文如此，是刻意设计而非笔误）**：
/// | 函数 | 删除条件 |
/// |---|---|
/// | `SendUpdateMsgA`/`SendUpdateMsg` | 仅 `wIdent` |
/// | `SendUpdateDelayMsg` | `wIdent` + `nParam1` |
/// | `SendUpdateBaseObject` | **`wIdent` + `BaseObject`（对象标识）** |
/// | `SendActionMsg` | 按 **`case wIdent` 的动作集合**（不比对参数） |
///
/// 故**不可**把它们"统一"成一个带谓词参数的函数——各条件的语义不同。
/// </summary>
public static class MsgQueueFamilyCore
{
    // ===================== SendUpdateBaseObject（30608-30648） =====================

    /// <summary>
    /// 30621-30638：按 **`wIdent` + `BaseObject`** 双条件排空。
    /// `BaseObject` 用**引用相等**比较（原文 `SendMessage.BaseObject = BaseObject` 是对象指针比较）。
    /// 删除后置 `boLateDelivery`（30629-30630），**不累积时长**。
    /// </summary>
    public static SendUpdateMsgCore.DrainResult DrainForUpdateBaseObject(
        List<TSendMessage> msgList, int wIdent, object? baseObject)
    {
        bool boLateDelivery = false;
        int removed = 0;

        int i = 0;
        while (true)
        {
            if (msgList.Count <= i)
                break;

            var sendMessage = msgList[i];

            if (sendMessage.wIdent == (ushort)wIdent
                && ReferenceEquals(sendMessage.BaseObject, baseObject))
            {
                msgList.RemoveAt(i);
                removed++;

                if (sendMessage.boLateDelivery)
                    boLateDelivery = true;

                continue;      // 30635
            }

            i++;
        }

        return new SendUpdateMsgCore.DrainResult(removed, boLateDelivery, 0);
    }

    /// <summary>
    /// 30644-30647：迟到 → `SendDelayMsg`（**延迟恒 0**）；否则 → `SendMsg`。
    /// 与非 A 版 `SendUpdateMsg`（J96）行为一致。
    /// </summary>
    public static SendUpdateOutcome FinishUpdateBaseObject(bool boLateDelivery)
        => MsgQueueFamilyCoreShared.FinishZeroDelay(boLateDelivery);

    // ===================== SendUseItemMsg（30650-30685） =====================

    /// <summary>
    /// 30665-30671：`SendUseItemMsg` 计数的消息集合。
    /// **注意该集合与 J96 的"攻击族"不同**：它额外含 `CM_EAT`/`CM_TURN`/`CM_WALK`/
    /// `CM_SITDOWN`/`CM_HORSERUN`/`CM_RUN`（动作族），但**不含 `CM_CRSHIT`**、
    /// **不含 `CM_43HIT`/`CM_66HIT`/`CM_66HIT1`/`CM_101HIT`/`CM_102HIT`/`CM_103HIT`**，
    /// 而单列了 `CM_113HIT`/`CM_115HIT`。逐字保留。
    /// </summary>
    private static readonly int[] UseItemCountSet =
    {
        Grobal2Const.CM_EAT,
        Grobal2Const.CM_TURN,
        Grobal2Const.CM_WALK,
        Grobal2Const.CM_SITDOWN,
        Grobal2Const.CM_HORSERUN,
        Grobal2Const.CM_RUN,
        Grobal2Const.CM_HIT,
        Grobal2Const.CM_HEAVYHIT,
        Grobal2Const.CM_BIGHIT,
        Grobal2Const.CM_POWERHIT,
        Grobal2Const.CM_LONGHIT,
        Grobal2Const.CM_WIDEHIT,
        Grobal2Const.CM_FIREHIT,
        Grobal2Const.CM_SWORDHIT,
        Grobal2Const.CM_113HIT,
        Grobal2Const.CM_115HIT,
    };

    /// <summary>该消息是否计入 `SendUseItemMsg` 的 nMsg。</summary>
    public static bool IsUseItemCountIdent(int wIdent)
        => Array.IndexOf(UseItemCountSet, wIdent) >= 0
           || HitActionMsgSet.InCustomHitRange(wIdent);

    /// <summary>30661-30675：统计队列中上述消息条数。**本函数不删除任何消息。**</summary>
    public static int CountUseItemMsgs(IReadOnlyList<TSendMessage> msgList)
    {
        int nMsg = 0;
        for (int i = 0; i < msgList.Count; i++)
        {
            if (IsUseItemCountIdent(msgList[i].wIdent))
                nMsg++;
        }
        return nMsg;
    }

    /// <summary>
    /// 30681-30684：`nMsg > 0` 时按 `Min(500, nMsg * 20)` **动态延迟**；否则立即 `SendMsg`。
    /// 这是本家族唯一使用**计算延迟**而非 0 的函数。
    /// </summary>
    public static SendUpdateOutcome FinishUseItemMsg(int nMsg)
    {
        if (nMsg > 0)
        {
            int delay = Math.Min(500, nMsg * 20);
            return new SendUpdateOutcome(SendUpdateKind.SendDelayMsg, (uint)delay);
        }

        return new SendUpdateOutcome(SendUpdateKind.SendMsg, 0);
    }

    // ===================== SendActionMsg（30687-30723） =====================

    /// <summary>
    /// 30703-30705：`SendActionMsg` 要清除的消息集合。
    /// **与 `SendUseItemMsg` 的计数集合高度相似但不相同**：本集合**不含 `CM_EAT`**，
    /// 也**不含 `CM_CRSHIT`/`CM_43HIT`/`CM_66HIT`/`CM_66HIT1`/`CM_101HIT`/`CM_102HIT`/`CM_103HIT`**。
    /// 逐字保留。
    /// </summary>
    private static readonly int[] ActionMsgClearSet =
    {
        Grobal2Const.CM_TURN,
        Grobal2Const.CM_WALK,
        Grobal2Const.CM_SITDOWN,
        Grobal2Const.CM_HORSERUN,
        Grobal2Const.CM_RUN,
        Grobal2Const.CM_HIT,
        Grobal2Const.CM_HEAVYHIT,
        Grobal2Const.CM_BIGHIT,
        Grobal2Const.CM_POWERHIT,
        Grobal2Const.CM_LONGHIT,
        Grobal2Const.CM_WIDEHIT,
        Grobal2Const.CM_FIREHIT,
        Grobal2Const.CM_SWORDHIT,
        Grobal2Const.CM_113HIT,
        Grobal2Const.CM_115HIT,
    };

    /// <summary>30703-30705：该消息是否被 `SendActionMsg` 清除。</summary>
    public static bool IsActionMsgClearIdent(int wIdent)
        => Array.IndexOf(ActionMsgClearSet, wIdent) >= 0
           || HitActionMsgSet.InCustomHitRange(wIdent);

    /// <summary>
    /// 30697-30716：删除队列中所有"动作类"消息。
    /// **注意 `case` 未命中时会落到 `Inc(I)`（30715）而非 `Continue`** ——
    /// 即遇到非动作消息才前进，动作消息删完继续检查同一位置（与 J96/J97 同一手法）。
    /// </summary>
    public static int DrainForActionMsg(List<TSendMessage> msgList)
    {
        int removed = 0;

        int i = 0;
        while (true)
        {
            if (msgList.Count <= i)
                break;

            var sendMessage = msgList[i];

            if (IsActionMsgClearIdent(sendMessage.wIdent))
            {
                msgList.RemoveAt(i);
                removed++;
                continue;      // 30712
            }

            i++;               // 30715
        }

        return removed;
    }

    /// <summary>30722：`SendActionMsg` **无条件**走普通 `SendMsg`（不因迟到改走延迟）。</summary>
    public static SendUpdateOutcome FinishActionMsg()
        => new(SendUpdateKind.SendMsg, 0);
}

/// <summary>本家族共用的小工具（避免各处重复写同一种收尾逻辑）。</summary>
internal static class MsgQueueFamilyCoreShared
{
    /// <summary>迟到 → `SendDelayMsg`（延迟 0）；否则 → `SendMsg`。</summary>
    internal static SendUpdateOutcome FinishZeroDelay(bool boLateDelivery)
        => boLateDelivery
            ? new SendUpdateOutcome(SendUpdateKind.SendDelayMsg, 0)
            : new SendUpdateOutcome(SendUpdateKind.SendMsg, 0);
}
