using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.M2Server;

/// <summary>
/// `M2Definition.pas` 262-274 的 `TSendMessage` 1:1（C# 侧以 class 表达，
/// 因原文以 `pTSendMessage` 指针 + `FreeMem` 手工管理生命周期）。
/// </summary>
public sealed class TSendMessage
{
    public ushort wIdent;
    public nint wParam;
    public nint nParam1;
    public nint nParam2;
    public nint nParam3;
    public object? BaseObject;
    public uint dwTimeTick;
    public uint dwDeliveryTime;
    public bool boLateDelivery;
    public byte[]? Buff;
}

/// <summary>
/// `TBaseObject.SendUpdateMsgA`（ObjBase.pas 30495-30564）与
/// `TBaseObject.SendUpdateMsg`（30566-30606）的 **队列排空 + 迟到判定** 1:1 移植（批次J96）。
///
/// 两个函数结构相同，差异在**迟到时间的累积方式**：
/// - `SendUpdateMsgA`（30522）：`dwDeliveryTime := Max(Now - SendMessage.dwDeliveryTime, dwDeliveryTime)`
///   —— 取**所有**被删同标识消息中的**最大**延迟时长，随后按消息类型取上限并 `Min` 钳制（30559），
///   再以该延迟走 `SendDelayMsg`（30560）；
/// - `SendUpdateMsg`（30587-30588）：**只置 `boLateDelivery := True`**，不累积时长，
///   最终以 **0** 延迟走 `SendDelayMsg`（30603）。
///
/// **逐字保留的原文语义**：
/// - 两处 `while True` 循环在**删除元素后 `Continue` 而不 `Inc(I)`**（30529/30593）——这样
///   同一位置的后续元素会被重新检查，是"删净所有同标识消息"的关键；若写成 `for` 循环会漏删。
/// - `SendUpdateMsgA` 的**删除顺序**是"先判 `boLateDelivery` 并取 Max，再 `Delete`"（30519-30524）；
///   而 `SendUpdateMsg` 是"**先 `Delete`**，再判 `boLateDelivery`"（30586-30588）——顺序相反，
///   但两者都在删除前读过了 `SendMessage` 的字段，故行为等价；此处逐字保留各自顺序。
/// - `Buff` 释放条件是 `(Buff <> nil) and (BuffLen > 0)`（30525/30589）——**两个条件都要满足**。
/// - 循环退出条件是 `m_MsgList.Count <= I`（30514/30581），即 `Break` 而非异常。
/// </summary>
public static class SendUpdateMsgCore
{
    /// <summary>
    /// 30540-30558 + 30559：`SendUpdateMsgA` 的迟到时长上限表与 `Min` 钳制。
    /// `nMaxDeliveryTime` 默认 **0**（30506），故未列出的消息类型会把延迟钳到 0。
    /// </summary>
    public static int ResolveMaxDeliveryTime(int wIdent, DeliveryTimeConfig cfg)
    {
        int nMaxDeliveryTime = 0;

        switch (wIdent)
        {
            case Grobal2Const.CM_SPELL:
                nMaxDeliveryTime = cfg.nMaxMagicHitDeliveryTime;
                break;

            case Grobal2Const.CM_HORSERUN:
            case Grobal2Const.CM_RUN:
                nMaxDeliveryTime = cfg.nMaxRunDeliveryTime;
                break;

            case Grobal2Const.CM_TURN:
                nMaxDeliveryTime = cfg.nMaxTurnDeliveryTime;
                break;

            case Grobal2Const.CM_WALK:
                nMaxDeliveryTime = cfg.nMaxWalkDeliveryTime;
                break;

            case Grobal2Const.CM_SITDOWN:
                nMaxDeliveryTime = cfg.nMaxDigUpDeliveryTime;
                break;

            default:
                // 30551-30557：攻击族（**含 CM_CRSHIT**，也含自定义范围）
                if (HitActionMsgSet.IsRunGateActionIdent(wIdent)
                    || wIdent == Grobal2Const.CM_CRSHIT)
                {
                    nMaxDeliveryTime = cfg.nMaxHitDeliveryTime;
                }
                break;
        }

        return nMaxDeliveryTime;
    }

    /// <summary>
    /// 30551-30557：`SendUpdateMsgA` 中归入"攻击族"的消息集合。
    /// 该列表**含 `CM_CRSHIT`**、含 `CM_101HIT..CM_103HIT`、**不含 `CM_100HIT`**，
    /// 与 `HitActionMsgSet` 的 RunGate 集合一致 —— 故直接复用该集合，避免第四份拷贝走样。
    /// </summary>
    public static bool IsHitFamilyForDelivery(int wIdent)
        => HitActionMsgSet.IsRunGateActionIdent(wIdent);

    /// <summary>30522：`Max(now - dwDeliveryTime, accumulated)`（无符号相减后取最大）。</summary>
    public static uint AccumulateDeliveryTime(uint now, uint msgDeliveryTime, uint accumulated)
    {
        uint diff = unchecked(now - msgDeliveryTime);
        return diff > accumulated ? diff : accumulated;
    }

    /// <summary>排空的结果：被删除的消息数、是否出现迟到、累积延迟。</summary>
    public readonly record struct DrainResult(int RemovedCount, bool BoLateDelivery, uint DwDeliveryTime);

    /// <summary>
    /// `SendUpdateMsgA` 的队列排空（30511-30532）1:1。
    /// 从 `m_MsgList` 中**删除所有** `wIdent` 相同的消息，并累积最大迟到时长。
    /// </summary>
    public static DrainResult DrainForUpdateMsgA(
        List<TSendMessage> msgList, ushort wIdent, uint now)
    {
        bool boLateDelivery = false;
        uint dwDeliveryTime = 0;
        int removed = 0;

        int i = 0;
        while (true)
        {
            if (msgList.Count <= i)
                break;

            var sendMessage = msgList[i];

            if (sendMessage.wIdent == wIdent)
            {
                if (sendMessage.boLateDelivery)
                {
                    boLateDelivery = true;
                    dwDeliveryTime = AccumulateDeliveryTime(now, sendMessage.dwDeliveryTime, dwDeliveryTime);
                }

                msgList.RemoveAt(i);
                removed++;
                // 30529：Continue —— 不 Inc(I)，继续检查同一位置
                continue;
            }

            i++;
        }

        return new DrainResult(removed, boLateDelivery, dwDeliveryTime);
    }

    /// <summary>
    /// `SendUpdateMsg` 的队列排空（30577-30596）1:1。
    /// 与 A 版的差异：**只置标志、不累积时长**，且**先删再判**。
    /// </summary>
    public static DrainResult DrainForUpdateMsg(
        List<TSendMessage> msgList, ushort wIdent)
    {
        bool boLateDelivery = false;
        int removed = 0;

        int i = 0;
        while (true)
        {
            if (msgList.Count <= i)
                break;

            var sendMessage = msgList[i];

            if (sendMessage.wIdent == wIdent)
            {
                msgList.RemoveAt(i);
                removed++;

                // 30587-30588：删除之后再判迟到（与 A 版顺序相反）
                if (sendMessage.boLateDelivery)
                    boLateDelivery = true;

                continue;
            }

            i++;
        }

        return new DrainResult(removed, boLateDelivery, 0);
    }

    /// <summary>
    /// 30538-30563：`SendUpdateMsgA` 的最终下发决策。
    /// 迟到 → `SendDelayMsg`（延迟经 `Min` 钳制）；否则 → 普通 `SendMsg`。
    /// </summary>
    public static SendUpdateOutcome FinishUpdateMsgA(
        ushort wIdent, bool boLateDelivery, uint dwDeliveryTime, DeliveryTimeConfig cfg)
    {
        if (!boLateDelivery)
            return new SendUpdateOutcome(SendUpdateKind.SendMsg, 0);

        int nMaxDeliveryTime = ResolveMaxDeliveryTime(wIdent, cfg);

        // 30559：Min 钳制（无符号与有符号比较，nMaxDeliveryTime 可能为 0）
        uint clamped = dwDeliveryTime < (uint)nMaxDeliveryTime
            ? dwDeliveryTime
            : (uint)nMaxDeliveryTime;

        return new SendUpdateOutcome(SendUpdateKind.SendDelayMsg, clamped);
    }

    /// <summary>30602-30605：`SendUpdateMsg` 的最终下发决策（迟到时延迟恒为 0）。</summary>
    public static SendUpdateOutcome FinishUpdateMsg(bool boLateDelivery)
        => boLateDelivery
            ? new SendUpdateOutcome(SendUpdateKind.SendDelayMsg, 0)
            : new SendUpdateOutcome(SendUpdateKind.SendMsg, 0);
}

/// <summary>30542-30550 引用的 `g_Config` 延迟上限字段。</summary>
public sealed class DeliveryTimeConfig
{
    public int nMaxMagicHitDeliveryTime;
    public int nMaxRunDeliveryTime;
    public int nMaxTurnDeliveryTime;
    public int nMaxWalkDeliveryTime;
    public int nMaxDigUpDeliveryTime;
    public int nMaxHitDeliveryTime;
}

/// <summary>最终下发方式。</summary>
public enum SendUpdateKind
{
    /// <summary>普通 `SendMsg`。</summary>
    SendMsg = 0,

    /// <summary>`SendDelayMsg`（带延迟）。</summary>
    SendDelayMsg = 1,
}

/// <summary>下发决策结果：方式 + 延迟毫秒。</summary>
public readonly record struct SendUpdateOutcome(SendUpdateKind Kind, uint DeliveryTime);
