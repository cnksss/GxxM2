using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.M2Server;

/// <summary>`pTProcessMessage` / `TProcessMessage` 的消费端引用（原文以指针传出）。</summary>
public sealed class TProcessMessage
{
    public ushort wIdent;
    public nint wParam;
    public nint nParam1;
    public nint nParam2;
    public nint nParam3;
    public nint BaseObject;
    public uint dwTimeTick;
    public uint dwDeliveryTime;
    public bool boLateDelivery;
    public string sMsg = "";
}

/// <summary>
/// ObjBase.pas 消息队列**消费端**两个函数 1:1 移植（批次J99）：
/// `TBaseObject.GetStruckMessage`(30725-30777) 与 `TBaseObject.GetMessage`(30779-30863)。
///
/// 至此 `m_MsgList` 的**投递侧 6 个函数**（J96/J97/J98）与**消费侧 2 个函数**（本批次）在 C# 侧齐备。
///
/// **两函数的关键差异**：
/// | | `GetStruckMessage` | `GetMessage` |
/// |---|---|---|
/// | 取哪条 | **仅 `RM_STRUCK`**（其余跳过） | **队列中第一条可用的**（任意 ident） |
/// | 延时检查 | **无**——取到即删 | **有**——未到点则跳过并 `Inc(I)` |
/// | 失败清空 | `Msg.wIdent := 0`（30737） | `Msg.wIdent := 0`（30793） |
/// | 循环条件 | `while m_MsgList.Count > I` | `while m_MsgList.Count > I`（同） |
///
/// **`GetMessage` 的复合延时门（30809-30811）极易写错**：
/// ```
/// (((SendMessage.wIdent = RM_10101) and (m_WAbil.HP > 0)) or (SendMessage.wIdent <> RM_10101))
///   and (SendMessage.boLateDelivery) and (MyGetTickCount < SendMessage.dwDeliveryTime)
/// ```
/// 该式等价于 **"非 `RM_10101` 的消息，或 `RM_10101` 且自身 HP > 0 的消息" 才参与延时等待**；
/// 换言之 **`RM_10101` 且 HP ≤ 0（即已死）时，延时门整体为假 → 该消息被立即取出、不等待**。
/// 原文注释解释了原因：`RM_10101` 是秒杀类消息，若死亡后仍按延时等待，
/// 会出现 `m_ExpHitter = nil` 的秒杀异常。若把它简化为"只看 boLateDelivery 与到点时刻"，
/// 就会重新引入该秒杀 bug。
///
/// 另注：`GetStruckMessage` 的 `if (SendMessage.wIdent <> RM_STRUCK) then Inc(I); Continue;`
/// （30741-30745）——**跳过所有非 `RM_STRUCK` 消息但不删除它们**，只删除并返回第一条 `RM_STRUCK`，
/// 随后 `Break`（30770）。`GetMessage` 亦为取一条即 `Break`（30853）。
/// </summary>
public static class MsgQueueConsumeCore
{
    // ===================== GetStruckMessage（30725-30777） =====================

    /// <summary>
    /// `GetStruckMessage` 1:1：从队列中取出**第一条** `RM_STRUCK` 并删除之；
    /// 其余消息一律跳过、**保留**。
    /// **本函数不做任何延时判定**——取到即删，无论是否到点。
    /// </summary>
    public static bool GetStruckMessage(List<TSendMessage> msgList, TProcessMessage msg)
    {
        // 30737：无论是否取到，先清零 wIdent
        msg.wIdent = 0;

        int i = 0;
        while (msgList.Count > i)
        {
            var sendMessage = msgList[i];

            // 30741-30745：非 RM_STRUCK 一律跳过（Inc + Continue），**不删除**
            if (sendMessage.wIdent != (ushort)Grobal2Const.RM_STRUCK)
            {
                i++;
                continue;
            }

            msgList.RemoveAt(i);       // 30746

            CopyTo(sendMessage, msg);
            ReleaseBuff(sendMessage, msg);

            // 30770：取到一条即止
            return true;
        }

        return false;
    }

    /// <summary>
    /// 30756-30766：有 Buff 且长度 > 0 时按该长度取出并释放；否则 `sMsg := ''`。
    /// </summary>
    private static void ReleaseBuff(TSendMessage sendMessage, TProcessMessage msg)
    {
        if (sendMessage.Buff != null && sendMessage.Buff.Length > 0)
        {
            // 30759-30760：按 BuffLen 精确还原
            msg.sMsg = System.Text.Encoding.ASCII.GetString(sendMessage.Buff, 0, sendMessage.Buff.Length);
            sendMessage.Buff = null;
        }
        else
        {
            msg.sMsg = "";
        }
    }

    /// <summary>30747-30755 / 30821-30829：逐字段搬运（两函数此处完全一致）。</summary>
    private static void CopyTo(TSendMessage sendMessage, TProcessMessage msg)
    {
        msg.wIdent = sendMessage.wIdent;
        msg.wParam = sendMessage.wParam;
        msg.nParam1 = sendMessage.nParam1;
        msg.nParam2 = sendMessage.nParam2;
        msg.nParam3 = sendMessage.nParam3;
        msg.BaseObject = sendMessage.BaseObject is null ? 0 : 1;
        msg.dwTimeTick = sendMessage.dwTimeTick;
        msg.dwDeliveryTime = sendMessage.dwDeliveryTime;
        msg.boLateDelivery = sendMessage.boLateDelivery;
    }

    // ===================== GetMessage（30779-30863） =====================

    /// <summary>
    /// 30809-30811：复合延时门。**为真表示"该消息尚在延时中，应跳过等待"**。
    ///
    /// 展开式：`(非 RM_10101，或 RM_10101 且 m_WAbil.HP > 0) 且 标记为迟到投递 且 尚未到点`。
    /// 即 **`RM_10101` 且 HP ≤ 0 时门为假 → 立即取出，不等待**（原文防秒杀处理）。
    /// </summary>
    public static bool ShouldWaitForDelivery(
        int wIdent, int m_WAbilHP, bool boLateDelivery, uint now, uint dwDeliveryTime)
    {
        bool identGate = (wIdent == Grobal2Const.RM_10101 && m_WAbilHP > 0)
                         || wIdent != Grobal2Const.RM_10101;

        return identGate
               && boLateDelivery
               && now < dwDeliveryTime;
    }

    /// <summary>
    /// `GetMessage` 1:1：取出队列中第一条**可用**消息并删除之。
    /// 遇到 `nil` 条目直接删除（30801-30806，`Continue` 不 `Inc`）；
    /// 遇到仍在延时中的消息则**跳过并 `Inc(I)`**（30813），保留在队列中。
    /// </summary>
    public static bool GetMessage(List<TSendMessage> msgList, TProcessMessage msg, uint now, int m_WAbilHP)
    {
        msg.wIdent = 0;       // 30793

        int i = 0;
        while (msgList.Count > i)
        {
            var sendMessage = msgList[i];

            // 30801-30806：nil 条目删除后 Continue（不 Inc）
            if (sendMessage is null)
            {
                msgList.RemoveAt(i);
                continue;
            }

            // 30809-30815：延时未到则跳过（保留，Inc）
            if (ShouldWaitForDelivery(sendMessage.wIdent, m_WAbilHP,
                    sendMessage.boLateDelivery, now, sendMessage.dwDeliveryTime))
            {
                i++;
                continue;
            }

            msgList.RemoveAt(i);      // 30818

            CopyTo(sendMessage, msg);
            ReleaseBuff(sendMessage, msg);

            return true;              // 30853
        }

        return false;
    }
}
