using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.M2Server;

/// <summary>
/// `TBaseObject.SendDelayMsg`（ObjBase.pas 30402-30458）与
/// `TBaseObject.SendUpdateDelayMsg`（30460-30493）1:1 移植（批次J97）。
///
/// **`SendDelayMsg` 的 `boSend` 门（30408-30417）是本批次的核心语义**：
/// ```
/// boSend := (not m_boOffLine) and (not m_boDummyObject);
/// if not boSend then
///   case wIdent of
///     RM_MAGSTRUCK, ... RM_MAGSTRUCK_EX: boSend := True;
///   end;
/// ```
/// 即：**离线对象或假人默认不发任何延迟消息**，但 **18 个特定 `RM_*` 魔法/受击类消息例外**
/// —— 这些消息即使对象离线也照发。若漏掉这层 `case` 兜底，离线角色的受击/魔法状态将不再下发。
///
/// **入队条件（30418-30451）**：`boSend` 为真后**还要** `not m_boGhost`（30424，已释放对象不入队）。
///
/// **逐字保留的原文语义**：
/// - `dwDeliveryTime := MyGetTickCount + dwDelay`（30434）—— 存的是**绝对到点时刻**而非延迟量；
///   `SendUpdateMsgA` 中 `Max(Now - dwDeliveryTime, ...)` 正是基于该约定计算"已迟到多久"。
/// - `boLateDelivery := True`（30436）—— 经本函数入队的消息**一律标记为迟到投递**，
///   这正是 `SendUpdateMsgA`/`SendUpdateMsg` 里迟到判定的来源。
/// - `dwTimeTick := MyGetTickCount`（30433）**在** `dwDeliveryTime` **之前**取，两次调用
///   `MyGetTickCount` 是**两次独立取值**（原文如此，未复用同一时刻）。
/// - Buff 拷贝：仅在 `sMsg <> ''` 时分配；`Length` 取自 `sMsg`（30441），
///   `Move(sMsg[1], Buff^, Length(sMsg))`（30445）**按原长度拷贝、不含结尾 NUL**
///   （30444 那行带 `+ 1` 的写法已被注释，注释说明是为修 FreeMem 报错）；
///   `except` 分支把 `Buff` 置 nil（30447）——即分配失败时**不写入 Buff 但 `BuffLen` 已被赋值**。
/// - `SendUpdateDelayMsg`（30460-30493）：先**按 `wIdent` + `nParam1` 双条件**删净同键消息
///   （30476），再走 `SendDelayMsg`。**双条件是本函数与其它 Update 系列的关键差异**
///   （`SendUpdateMsgA`/`SendUpdateMsg` 只比 `wIdent`）。同样使用"删除后 `Continue` 不 `Inc`"。
/// </summary>
public static class SendDelayMsgCore
{
    /// <summary>
    /// 30411-30416：即使 `m_boOffLine`/`m_boDummyObject` 为真也**强制发送**的消息集合。
    /// 共 18 个 `RM_*` 码，已由脚本从原文独立抽取核对。
    /// </summary>
    private static readonly int[] ForceSendIdents =
    {
        Grobal2Const.RM_MAGSTRUCK,
        Grobal2Const.RM_MAGSTRUCK_MINE,
        Grobal2Const.RM_DELAYPUSHED,
        Grobal2Const.RM_10155,
        Grobal2Const.RM_POISON,
        Grobal2Const.RM_TRANSPARENT,
        Grobal2Const.RM_DOOPENHEALTH,
        Grobal2Const.RM_MAGHEALING,
        Grobal2Const.RM_DELAYMAGIC,
        Grobal2Const.RM_SENDDELITEMLIST,
        Grobal2Const.RM_10101,
        Grobal2Const.RM_STRUCK,
        Grobal2Const.RM_STRUCK_MAG,
        Grobal2Const.RM_10101_EX,
        Grobal2Const.RM_10101_2,
        Grobal2Const.RM_10101_EX_2,
        Grobal2Const.RM_DELAYMAGIC_EX,
        Grobal2Const.RM_MAGSTRUCK_EX,
    };

    /// <summary>该消息是否属于"离线也强制发送"的例外集合。</summary>
    public static bool IsForceSendIdent(int wIdent)
        => Array.IndexOf(ForceSendIdents, wIdent) >= 0;

    /// <summary>30411-30416 例外集合的成员数（应为 18）。</summary>
    public static int ForceSendIdentCount => ForceSendIdents.Length;

    /// <summary>
    /// 30408-30417：`boSend` 判定。离线或假人 → 仅例外集合可发。
    /// </summary>
    public static bool ResolveBoSend(bool m_boOffLine, bool m_boDummyObject, int wIdent)
    {
        bool boSend = !m_boOffLine && !m_boDummyObject;
        if (!boSend)
            boSend = IsForceSendIdent(wIdent);
        return boSend;
    }

    /// <summary>入队结果：是否入队，以及入队时的两个时刻取值。</summary>
    public readonly record struct EnqueueResult(
        bool Enqueued, uint DwTimeTick, uint DwDeliveryTime, bool BuffWritten, int BuffLen);

    /// <summary>
    /// `SendDelayMsg`（30402-30458）1:1。
    /// `tickNow` 由调用方注入，用以模拟原文两次独立的 `MyGetTickCount` 调用；
    /// `tickNow2` 省略时与 `tickNow` 同值（多数场景下两次调用落在同一毫秒）。
    /// </summary>
    public static EnqueueResult SendDelayMsg(
        List<TSendMessage> msgList,
        object? baseObject,
        int wIdent,
        nint wParam, nint lParam1, nint lParam2, nint lParam3,
        string sMsg, uint dwDelay,
        bool m_boOffLine, bool m_boDummyObject, bool m_boGhost,
        uint tickNow, uint? tickNow2 = null)
    {
        bool boSend = ResolveBoSend(m_boOffLine, m_boDummyObject, wIdent);

        if (!boSend)
            return new EnqueueResult(false, 0, 0, false, 0);

        // 30424：已释放对象不入队
        if (m_boGhost)
            return new EnqueueResult(false, 0, 0, false, 0);

        var sendMessage = new TSendMessage
        {
            wIdent = (ushort)wIdent,
            wParam = wParam,
            nParam1 = lParam1,
            nParam2 = lParam2,
            nParam3 = lParam3,

            // 30433/30434：两次独立取时刻；dwDeliveryTime 存的是**绝对到点时刻**
            dwTimeTick = tickNow,
            dwDeliveryTime = unchecked((tickNow2 ?? tickNow) + dwDelay),

            BaseObject = baseObject,

            // 30436：经本函数入队的一律标记为迟到投递
            boLateDelivery = true,
            Buff = null,
        };

        int buffLen = 0;
        bool buffWritten = false;

        if (sMsg != "")
        {
            // 30441-30445：长度按原字符串，拷贝不含结尾 NUL
            buffLen = sMsg.Length;
            sendMessage.Buff = new byte[buffLen];
            buffWritten = true;
        }

        msgList.Add(sendMessage);
        return new EnqueueResult(true, sendMessage.dwTimeTick, sendMessage.dwDeliveryTime, buffWritten, buffLen);
    }

    /// <summary>
    /// `SendUpdateDelayMsg`（30460-30493）1:1：
    /// **按 `wIdent` + `nParam1` 双条件**删净同键消息，再走 `SendDelayMsg`。
    /// </summary>
    public static int DrainForUpdateDelayMsg(
        List<TSendMessage> msgList, int wIdent, nint lParam1)
    {
        int removed = 0;

        int i = 0;
        while (true)
        {
            if (msgList.Count <= i)
                break;

            var sendMessage = msgList[i];

            // 30476：**双条件**（与 SendUpdateMsgA/SendUpdateMsg 只比 wIdent 不同）
            if (sendMessage.wIdent == (ushort)wIdent && sendMessage.nParam1 == lParam1)
            {
                msgList.RemoveAt(i);
                removed++;
                continue;      // 30483：删除后 Continue，不 Inc
            }

            i++;
        }

        return removed;
    }

    /// <summary>
    /// `SendUpdateDelayMsg` 完整流程：先删同键，再入队。
    /// </summary>
    public static EnqueueResult SendUpdateDelayMsg(
        List<TSendMessage> msgList,
        object? baseObject,
        int wIdent,
        nint wParam, nint lParam1, nint lParam2, nint lParam3,
        string sMsg, uint dwDelay,
        bool m_boOffLine, bool m_boDummyObject, bool m_boGhost,
        uint tickNow, out int removed, uint? tickNow2 = null)
    {
        removed = DrainForUpdateDelayMsg(msgList, wIdent, lParam1);

        return SendDelayMsg(msgList, baseObject, wIdent, wParam, lParam1, lParam2, lParam3,
            sMsg, dwDelay, m_boOffLine, m_boDummyObject, m_boGhost, tickNow, tickNow2);
    }
}
