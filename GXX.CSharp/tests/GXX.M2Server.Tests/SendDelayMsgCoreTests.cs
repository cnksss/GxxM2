using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J97：ObjBase.pas SendDelayMsg(30402-30458) / SendUpdateDelayMsg(30460-30493) 1:1 测试。
/// </summary>
public sealed class SendDelayMsgCoreTests
{
    private static List<TSendMessage> List() => new();

    private static SendDelayMsgCore.EnqueueResult Send(
        List<TSendMessage> list, int ident,
        string sMsg = "", uint delay = 0,
        bool offLine = false, bool dummy = false, bool ghost = false,
        uint tick = 1000, uint? tick2 = null)
        => SendDelayMsgCore.SendDelayMsg(list, null, ident, 0, 0, 0, 0, sMsg, delay,
            offLine, dummy, ghost, tick, tick2);

    // ===================== boSend 门（30408-30417） =====================

    [Fact]
    public void NormalObjectSends()
    {
        Assert.True(SendDelayMsgCore.ResolveBoSend(false, false, Grobal2Const.CM_HIT));
    }

    [Fact]
    public void OffLineObjectBlocksOrdinaryMessage()
    {
        Assert.False(SendDelayMsgCore.ResolveBoSend(true, false, Grobal2Const.CM_HIT));
    }

    [Fact]
    public void DummyObjectBlocksOrdinaryMessage()
    {
        Assert.False(SendDelayMsgCore.ResolveBoSend(false, true, Grobal2Const.CM_HIT));
    }

    [Fact]
    public void BothOffLineAndDummyAlsoBlock()
    {
        Assert.False(SendDelayMsgCore.ResolveBoSend(true, true, Grobal2Const.CM_HIT));
    }

    [Fact]
    public void ForceSendSetHas18Members()
    {
        // 由脚本从原文 30412-30414 独立抽取核对
        Assert.Equal(18, SendDelayMsgCore.ForceSendIdentCount);
    }

    [Fact]
    public void ForceSendExceptionBypassesOffLine()
    {
        // 30411-30416：18 个 RM_* 码即使离线也强制发送
        foreach (int id in new[]
                 {
                     Grobal2Const.RM_MAGSTRUCK, Grobal2Const.RM_MAGSTRUCK_MINE,
                     Grobal2Const.RM_DELAYPUSHED, Grobal2Const.RM_10155,
                     Grobal2Const.RM_POISON, Grobal2Const.RM_TRANSPARENT,
                     Grobal2Const.RM_DOOPENHEALTH, Grobal2Const.RM_MAGHEALING,
                     Grobal2Const.RM_DELAYMAGIC, Grobal2Const.RM_SENDDELITEMLIST,
                     Grobal2Const.RM_10101, Grobal2Const.RM_STRUCK,
                     Grobal2Const.RM_STRUCK_MAG, Grobal2Const.RM_10101_EX,
                     Grobal2Const.RM_10101_2, Grobal2Const.RM_10101_EX_2,
                     Grobal2Const.RM_DELAYMAGIC_EX, Grobal2Const.RM_MAGSTRUCK_EX,
                 })
        {
            Assert.True(SendDelayMsgCore.IsForceSendIdent(id), $"RM 码 {id} 应在例外集合");
            Assert.True(SendDelayMsgCore.ResolveBoSend(true, false, id), "离线也应发送");
            Assert.True(SendDelayMsgCore.ResolveBoSend(false, true, id), "假人也应发送");
        }
    }

    [Fact]
    public void ForceSendExceptionAlsoAppliesToBothFlags()
    {
        Assert.True(SendDelayMsgCore.ResolveBoSend(true, true, Grobal2Const.RM_STRUCK));
    }

    [Fact]
    public void NonExceptionRmCodeIsBlockedWhenOffLine()
    {
        // 未列入例外的消息仍被拦下
        Assert.False(SendDelayMsgCore.IsForceSendIdent(Grobal2Const.RM_DELAYMAGIC + 1000));
        Assert.False(SendDelayMsgCore.ResolveBoSend(true, false, Grobal2Const.RM_DELAYMAGIC + 1000));
    }

    // ===================== 入队（30418-30451） =====================

    [Fact]
    public void BlockedSendEnqueuesNothing()
    {
        var list = List();
        var r = Send(list, Grobal2Const.CM_HIT, offLine: true);

        Assert.False(r.Enqueued);
        Assert.Empty(list);
    }

    [Fact]
    public void GhostBlocksEnqueueEvenWhenBoSendTrue()
    {
        // 30424：boSend 为真后**还要** not m_boGhost
        var list = List();
        var r = Send(list, Grobal2Const.CM_HIT, ghost: true);

        Assert.False(r.Enqueued);
        Assert.Empty(list);
    }

    [Fact]
    public void GhostDoesNotBlockForceSendIdent()
    {
        // 例外集合绕过的是 boSend 门；ghost 判定在其后，故 ghost 仍拦下
        var list = List();
        var r = Send(list, Grobal2Const.RM_STRUCK, offLine: true, ghost: true);

        Assert.False(r.Enqueued);
        Assert.Empty(list);
    }

    [Fact]
    public void ForceSendIdentEnqueuesWhenOffLine()
    {
        var list = List();
        var r = Send(list, Grobal2Const.RM_STRUCK, offLine: true);

        Assert.True(r.Enqueued);
        Assert.Single(list);
        Assert.Equal(Grobal2Const.RM_STRUCK, list[0].wIdent);
    }

    [Fact]
    public void EnqueuedMessageIsMarkedLateDelivery()
    {
        // 30436：经本函数入队的一律标记为迟到投递
        var list = List();
        Send(list, Grobal2Const.CM_HIT);

        Assert.True(list[0].boLateDelivery);
    }

    [Fact]
    public void DeliveryTimeIsAbsoluteDeadline()
    {
        // 30434：存的是"到点时刻"而非延迟量
        var list = List();
        var r = Send(list, Grobal2Const.CM_HIT, delay: 500, tick: 1000);

        Assert.Equal(1500u, r.DwDeliveryTime);
        Assert.Equal(1500u, list[0].dwDeliveryTime);
    }

    [Fact]
    public void DeliveryTimeWrapsOnOverflow()
    {
        var list = List();
        var r = Send(list, Grobal2Const.CM_HIT, delay: 500, tick: uint.MaxValue - 100);

        Assert.Equal(unchecked(uint.MaxValue - 100 + 500), r.DwDeliveryTime);
    }

    [Fact]
    public void TimeTickAndDeliveryTimeAreIndependentReadings()
    {
        // 30433/30434：两次独立调用 MyGetTickCount，故可传入不同值
        var list = List();
        var r = Send(list, Grobal2Const.CM_HIT, delay: 100, tick: 1000, tick2: 1005);

        Assert.Equal(1000u, r.DwTimeTick);
        Assert.Equal(1105u, r.DwDeliveryTime);      // 1005 + 100，非 1000 + 100
    }

    [Fact]
    public void TimeTickEqualsFirstReadingWhenNotSeparated()
    {
        var list = List();
        var r = Send(list, Grobal2Const.CM_HIT, delay: 100, tick: 1000);

        Assert.Equal(1000u, r.DwTimeTick);
        Assert.Equal(1100u, r.DwDeliveryTime);
    }

    // ===================== Buff =====================

    [Fact]
    public void EmptyStringAllocatesNoBuff()
    {
        var list = List();
        var r = Send(list, Grobal2Const.CM_HIT, sMsg: "");

        Assert.False(r.BuffWritten);
        Assert.Equal(0, r.BuffLen);
        Assert.Null(list[0].Buff);
    }

    [Fact]
    public void NonEmptyStringAllocatesBuffOfExactLength()
    {
        // 30445：按原长度拷贝，**不含结尾 NUL**（带 +1 的写法已被注释）
        var list = List();
        var r = Send(list, Grobal2Const.CM_HIT, sMsg: "hello");

        Assert.True(r.BuffWritten);
        Assert.Equal(5, r.BuffLen);
        Assert.Equal(5, list[0].Buff!.Length);
    }

    [Fact]
    public void SingleCharMessageHasBuffLenOne()
    {
        var list = List();
        var r = Send(list, Grobal2Const.CM_HIT, sMsg: "x");

        Assert.Equal(1, r.BuffLen);
        Assert.Single(list[0].Buff!);
    }

    // ===================== SendUpdateDelayMsg 双条件删除 =====================

    [Fact]
    public void UpdateDelayDeletesSameIdentAndParam1()
    {
        // 30476：**双条件** wIdent + nParam1
        var list = new List<TSendMessage>
        {
            new() { wIdent = Grobal2Const.RM_STRUCK, nParam1 = 7 },
            new() { wIdent = Grobal2Const.RM_STRUCK, nParam1 = 7 },
            new() { wIdent = Grobal2Const.RM_STRUCK, nParam1 = 8 },
        };

        int removed = SendDelayMsgCore.DrainForUpdateDelayMsg(list, Grobal2Const.RM_STRUCK, 7);

        Assert.Equal(2, removed);
        Assert.Single(list);
        Assert.Equal(8, list[0].nParam1);
    }

    [Fact]
    public void UpdateDelayKeepsSameIdentDifferentParam1()
    {
        var list = new List<TSendMessage>
        {
            new() { wIdent = Grobal2Const.RM_STRUCK, nParam1 = 1 },
            new() { wIdent = Grobal2Const.RM_STRUCK, nParam1 = 2 },
            new() { wIdent = Grobal2Const.RM_STRUCK, nParam1 = 3 },
        };

        int removed = SendDelayMsgCore.DrainForUpdateDelayMsg(list, Grobal2Const.RM_STRUCK, 2);

        Assert.Equal(1, removed);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void UpdateDelayKeepsDifferentIdentSameParam1()
    {
        var list = new List<TSendMessage>
        {
            new() { wIdent = Grobal2Const.RM_POISON, nParam1 = 5 },
            new() { wIdent = Grobal2Const.RM_STRUCK, nParam1 = 5 },
        };

        int removed = SendDelayMsgCore.DrainForUpdateDelayMsg(list, Grobal2Const.RM_STRUCK, 5);

        Assert.Equal(1, removed);
        Assert.Single(list);
        Assert.Equal(Grobal2Const.RM_POISON, list[0].wIdent);
    }

    [Fact]
    public void UpdateDelayHandlesAdjacentDuplicates()
    {
        // 同样使用"删除后 Continue 不 Inc"
        var list = new List<TSendMessage>
        {
            new() { wIdent = Grobal2Const.RM_STRUCK, nParam1 = 1 },
            new() { wIdent = Grobal2Const.RM_STRUCK, nParam1 = 1 },
            new() { wIdent = Grobal2Const.RM_STRUCK, nParam1 = 1 },
        };

        int removed = SendDelayMsgCore.DrainForUpdateDelayMsg(list, Grobal2Const.RM_STRUCK, 1);

        Assert.Equal(3, removed);
        Assert.Empty(list);
    }

    [Fact]
    public void UpdateDelayThenEnqueues()
    {
        var list = new List<TSendMessage>
        {
            new() { wIdent = Grobal2Const.RM_STRUCK, nParam1 = 4 },
        };

        var r = SendDelayMsgCore.SendUpdateDelayMsg(list, null, Grobal2Const.RM_STRUCK,
            0, 4, 0, 0, "", 200, false, false, false, 500, out int removed);

        Assert.Equal(1, removed);
        Assert.True(r.Enqueued);
        Assert.Single(list);
        Assert.Equal(700u, list[0].dwDeliveryTime);
    }

    [Fact]
    public void DoubleConditionDiffersFromSingleCondition()
    {
        // 冗余保护：证明双条件确实比单条件少删
        var mk = () => new List<TSendMessage>
        {
            new() { wIdent = Grobal2Const.RM_STRUCK, nParam1 = 1 },
            new() { wIdent = Grobal2Const.RM_STRUCK, nParam1 = 2 },
        };

        int dual = SendDelayMsgCore.DrainForUpdateDelayMsg(mk(), Grobal2Const.RM_STRUCK, 1);
        var single = mk();
        int singleCount = 0;
        for (int i = single.Count - 1; i >= 0; i--)
            if (single[i].wIdent == Grobal2Const.RM_STRUCK) { single.RemoveAt(i); singleCount++; }

        Assert.Equal(1, dual);
        Assert.Equal(2, singleCount);
        Assert.NotEqual(dual, singleCount);
    }

    [Fact]
    public void NoMatchRemovesNothing()
    {
        var list = new List<TSendMessage>
        {
            new() { wIdent = Grobal2Const.RM_POISON, nParam1 = 9 },
        };

        Assert.Equal(0, SendDelayMsgCore.DrainForUpdateDelayMsg(list, Grobal2Const.RM_STRUCK, 9));
        Assert.Single(list);
    }
}
