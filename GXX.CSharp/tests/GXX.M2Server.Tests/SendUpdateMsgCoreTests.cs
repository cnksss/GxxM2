using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J96：ObjBase.pas SendUpdateMsgA(30495-30564) / SendUpdateMsg(30566-30606)
/// 的队列排空与迟到判定 1:1 测试。
/// </summary>
public sealed class SendUpdateMsgCoreTests
{
    private static TSendMessage Msg(ushort ident, bool late = false, uint deliveryTime = 0)
        => new() { wIdent = ident, boLateDelivery = late, dwDeliveryTime = deliveryTime };

    private static DeliveryTimeConfig Cfg(
        int magic = 0, int run = 0, int turn = 0, int walk = 0, int digUp = 0, int hit = 0)
        => new()
        {
            nMaxMagicHitDeliveryTime = magic,
            nMaxRunDeliveryTime = run,
            nMaxTurnDeliveryTime = turn,
            nMaxWalkDeliveryTime = walk,
            nMaxDigUpDeliveryTime = digUp,
            nMaxHitDeliveryTime = hit,
        };

    // ===================== TSendMessage 结构 =====================

    [Fact]
    public void SendMessageDefaultsAreZeroLikeRecord()
    {
        var m = new TSendMessage();
        Assert.Equal(0, m.wIdent);
        Assert.False(m.boLateDelivery);
        Assert.Null(m.Buff);
    }

    // ===================== 排空：核心 Continue 语义 =====================

    [Fact]
    public void DrainRemovesAllMatchingMessages()
    {
        // 30529 的 Continue（不 Inc）是"删净"的关键
        var list = new List<TSendMessage>
        {
            Msg(Grobal2Const.CM_HIT), Msg(Grobal2Const.CM_HIT), Msg(Grobal2Const.CM_HIT),
        };

        var r = SendUpdateMsgCore.DrainForUpdateMsgA(list, Grobal2Const.CM_HIT, 0);

        Assert.Equal(3, r.RemovedCount);
        Assert.Empty(list);
    }

    [Fact]
    public void DrainRemovesAdjacentDuplicatesWithoutSkipping()
    {
        // 若误写成 for + Inc，则相邻的两条会被跳过
        var list = new List<TSendMessage>
        {
            Msg(Grobal2Const.CM_WALK), Msg(Grobal2Const.CM_HIT), Msg(Grobal2Const.CM_HIT),
            Msg(Grobal2Const.CM_WALK),
        };

        var r = SendUpdateMsgCore.DrainForUpdateMsgA(list, Grobal2Const.CM_HIT, 0);

        Assert.Equal(2, r.RemovedCount);
        Assert.Equal(2, list.Count);
        Assert.All(list, m => Assert.Equal(Grobal2Const.CM_WALK, m.wIdent));
    }

    [Fact]
    public void DrainKeepsOtherIdents()
    {
        var list = new List<TSendMessage>
        {
            Msg(Grobal2Const.CM_SPELL), Msg(Grobal2Const.CM_HIT), Msg(Grobal2Const.CM_RUN),
        };

        var r = SendUpdateMsgCore.DrainForUpdateMsgA(list, Grobal2Const.CM_HIT, 0);

        Assert.Equal(1, r.RemovedCount);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void DrainEmptyListIsNoOp()
    {
        var list = new List<TSendMessage>();
        var r = SendUpdateMsgCore.DrainForUpdateMsgA(list, Grobal2Const.CM_HIT, 0);

        Assert.Equal(0, r.RemovedCount);
        Assert.False(r.BoLateDelivery);
    }

    [Fact]
    public void DrainNoMatchIsNoOp()
    {
        var list = new List<TSendMessage> { Msg(Grobal2Const.CM_SPELL) };
        var r = SendUpdateMsgCore.DrainForUpdateMsgA(list, Grobal2Const.CM_HIT, 0);

        Assert.Equal(0, r.RemovedCount);
        Assert.Single(list);
    }

    // ===================== 迟到时长累积（A 版特有） =====================

    [Fact]
    public void UpdateMsgAFlagsLateWhenAnyMatchingIsLate()
    {
        var list = new List<TSendMessage>
        {
            Msg(Grobal2Const.CM_HIT, late: false),
            Msg(Grobal2Const.CM_HIT, late: true, deliveryTime: 100),
        };

        var r = SendUpdateMsgCore.DrainForUpdateMsgA(list, Grobal2Const.CM_HIT, now: 150);

        Assert.True(r.BoLateDelivery);
        Assert.Equal(50u, r.DwDeliveryTime);      // 150 - 100
    }

    [Fact]
    public void UpdateMsgATakesMaximumAcrossMessages()
    {
        // 30522：Max(now - dwDeliveryTime, accumulated) —— 取最大
        var list = new List<TSendMessage>
        {
            Msg(Grobal2Const.CM_HIT, late: true, deliveryTime: 100),   // 50
            Msg(Grobal2Const.CM_HIT, late: true, deliveryTime: 10),    // 140
            Msg(Grobal2Const.CM_HIT, late: true, deliveryTime: 90),    // 60
        };

        var r = SendUpdateMsgCore.DrainForUpdateMsgA(list, Grobal2Const.CM_HIT, now: 150);

        Assert.Equal(140u, r.DwDeliveryTime);
    }

    [Fact]
    public void UpdateMsgAOrdersMaxRegardlessOfListOrder()
    {
        var asc = SendUpdateMsgCore.DrainForUpdateMsgA(new List<TSendMessage>
        {
            Msg(Grobal2Const.CM_HIT, true, 10), Msg(Grobal2Const.CM_HIT, true, 100),
        }, Grobal2Const.CM_HIT, 150);

        var desc = SendUpdateMsgCore.DrainForUpdateMsgA(new List<TSendMessage>
        {
            Msg(Grobal2Const.CM_HIT, true, 100), Msg(Grobal2Const.CM_HIT, true, 10),
        }, Grobal2Const.CM_HIT, 150);

        Assert.Equal(asc.DwDeliveryTime, desc.DwDeliveryTime);
        Assert.Equal(140u, asc.DwDeliveryTime);
    }

    [Fact]
    public void UpdateMsgAIgnoresNonLateMessagesForTime()
    {
        // 非迟到消息不参与时长累积（30519 的 if 包住 30522）
        var list = new List<TSendMessage>
        {
            Msg(Grobal2Const.CM_HIT, late: false, deliveryTime: 0),
            Msg(Grobal2Const.CM_HIT, late: true, deliveryTime: 100),
        };

        var r = SendUpdateMsgCore.DrainForUpdateMsgA(list, Grobal2Const.CM_HIT, 150);

        Assert.Equal(50u, r.DwDeliveryTime);
    }

    [Fact]
    public void AccumulateUsesUnsignedSubtraction()
    {
        // 原文为 LongWord 相减，回绕是定义行为
        uint v = SendUpdateMsgCore.AccumulateDeliveryTime(now: 10, msgDeliveryTime: 100, accumulated: 0);
        Assert.Equal(unchecked(10u - 100u), v);
    }

    [Fact]
    public void AccumulateKeepsLargerAccumulated()
    {
        uint v = SendUpdateMsgCore.AccumulateDeliveryTime(now: 150, msgDeliveryTime: 100, accumulated: 999);
        Assert.Equal(999u, v);
    }

    // ===================== SendUpdateMsg（非 A 版）差异 =====================

    [Fact]
    public void UpdateMsgFlagsLateWithoutAccumulatingTime()
    {
        // 30587-30588：只置标志，**不累积时长**
        var list = new List<TSendMessage>
        {
            Msg(Grobal2Const.CM_HIT, late: true, deliveryTime: 100),
        };

        var r = SendUpdateMsgCore.DrainForUpdateMsg(list, Grobal2Const.CM_HIT);

        Assert.True(r.BoLateDelivery);
        Assert.Equal(0u, r.DwDeliveryTime);
    }

    [Fact]
    public void UpdateMsgAlsoRemovesAllMatching()
    {
        var list = new List<TSendMessage>
        {
            Msg(Grobal2Const.CM_HIT, true), Msg(Grobal2Const.CM_HIT, true), Msg(Grobal2Const.CM_HIT, true),
        };

        var r = SendUpdateMsgCore.DrainForUpdateMsg(list, Grobal2Const.CM_HIT);

        Assert.Equal(3, r.RemovedCount);
        Assert.Empty(list);
    }

    [Fact]
    public void UpdateMsgFalseWhenNoneLate()
    {
        var list = new List<TSendMessage> { Msg(Grobal2Const.CM_HIT, false) };
        var r = SendUpdateMsgCore.DrainForUpdateMsg(list, Grobal2Const.CM_HIT);

        Assert.False(r.BoLateDelivery);
    }

    // ===================== 最终下发决策 =====================

    [Fact]
    public void NotLateUsesPlainSendMsg()
    {
        var o = SendUpdateMsgCore.FinishUpdateMsgA(Grobal2Const.CM_HIT, false, 500, Cfg(hit: 100));
        Assert.Equal(SendUpdateKind.SendMsg, o.Kind);
        Assert.Equal(0u, o.DeliveryTime);
    }

    [Fact]
    public void LateUsesSendDelayMsg()
    {
        var o = SendUpdateMsgCore.FinishUpdateMsgA(Grobal2Const.CM_HIT, true, 50, Cfg(hit: 100));
        Assert.Equal(SendUpdateKind.SendDelayMsg, o.Kind);
        Assert.Equal(50u, o.DeliveryTime);
    }

    [Fact]
    public void LateDeliveryIsClampedToMax()
    {
        // 30559：Min 钳制
        var o = SendUpdateMsgCore.FinishUpdateMsgA(Grobal2Const.CM_HIT, true, 500, Cfg(hit: 100));
        Assert.Equal(100u, o.DeliveryTime);
    }

    [Fact]
    public void UnlistedIdentClampsToZero()
    {
        // nMaxDeliveryTime 默认 0 → 未列出的类型延迟钳为 0
        var o = SendUpdateMsgCore.FinishUpdateMsgA(Grobal2Const.CM_SAY, true, 500, Cfg(hit: 100));
        Assert.Equal(0u, o.DeliveryTime);
    }

    [Fact]
    public void UpdateMsgAlwaysUsesZeroDelay()
    {
        // 30603：非 A 版即使迟到也用 0 延迟
        var late = SendUpdateMsgCore.FinishUpdateMsg(true);
        Assert.Equal(SendUpdateKind.SendDelayMsg, late.Kind);
        Assert.Equal(0u, late.DeliveryTime);

        var notLate = SendUpdateMsgCore.FinishUpdateMsg(false);
        Assert.Equal(SendUpdateKind.SendMsg, notLate.Kind);
    }

    // ===================== 上限表 30540-30558 =====================

    [Fact]
    public void SpellUsesMagicHitLimit()
    {
        Assert.Equal(11, SendUpdateMsgCore.ResolveMaxDeliveryTime(
            Grobal2Const.CM_SPELL, Cfg(magic: 11)));
    }

    [Theory]
    [InlineData(Grobal2Const.CM_HORSERUN)]
    [InlineData(Grobal2Const.CM_RUN)]
    public void HorseRunAndRunShareLimit(int ident)
    {
        Assert.Equal(22, SendUpdateMsgCore.ResolveMaxDeliveryTime(ident, Cfg(run: 22)));
    }

    [Fact]
    public void TurnUsesTurnLimit()
    {
        Assert.Equal(33, SendUpdateMsgCore.ResolveMaxDeliveryTime(Grobal2Const.CM_TURN, Cfg(turn: 33)));
    }

    [Fact]
    public void WalkUsesWalkLimit()
    {
        Assert.Equal(44, SendUpdateMsgCore.ResolveMaxDeliveryTime(Grobal2Const.CM_WALK, Cfg(walk: 44)));
    }

    [Fact]
    public void SitDownUsesDigUpLimit()
    {
        // 30549-30550：CM_SITDOWN 用的是 nMaxDigUpDeliveryTime（名字不同，原文如此）
        Assert.Equal(55, SendUpdateMsgCore.ResolveMaxDeliveryTime(Grobal2Const.CM_SITDOWN, Cfg(digUp: 55)));
    }

    [Fact]
    public void HitFamilyUsesHitLimit()
    {
        foreach (int h in new[]
                 {
                     Grobal2Const.CM_HIT, Grobal2Const.CM_HEAVYHIT, Grobal2Const.CM_BIGHIT,
                     Grobal2Const.CM_POWERHIT, Grobal2Const.CM_LONGHIT, Grobal2Const.CM_CRSHIT,
                     Grobal2Const.CM_TWNHIT, Grobal2Const.CM_WIDEHIT, Grobal2Const.CM_FIREHIT,
                     Grobal2Const.CM_43HIT, Grobal2Const.CM_66HIT, Grobal2Const.CM_66HIT1,
                     Grobal2Const.CM_SWORDHIT, Grobal2Const.CM_101HIT, Grobal2Const.CM_102HIT,
                     Grobal2Const.CM_103HIT, Grobal2Const.CM_113HIT, Grobal2Const.CM_115HIT,
                 })
            Assert.Equal(66, SendUpdateMsgCore.ResolveMaxDeliveryTime(h, Cfg(hit: 66)));
    }

    [Fact]
    public void CustomHitRangeUsesHitLimit()
    {
        Assert.Equal(66, SendUpdateMsgCore.ResolveMaxDeliveryTime(6000, Cfg(hit: 66)));
        Assert.Equal(66, SendUpdateMsgCore.ResolveMaxDeliveryTime(6299, Cfg(hit: 66)));
        Assert.Equal(0, SendUpdateMsgCore.ResolveMaxDeliveryTime(6300, Cfg(hit: 66)));
    }

    [Fact]
    public void HundredHitIsNotInDeliveryHitFamily()
    {
        // 30551-30557 **没有 CM_100HIT** —— 与 HitActionMsgSet 的 RunGate 集合一致
        Assert.Equal(0, SendUpdateMsgCore.ResolveMaxDeliveryTime(Grobal2Const.CM_100HIT, Cfg(hit: 66)));
    }

    [Fact]
    public void DeliveryHitFamilyMatchesRunGateSet()
    {
        // 避免第四份拷贝走样：本表应与 J95 的 RunGate 集合完全一致
        for (int id = 3000; id < 6400; id++)
            Assert.Equal(
                HitActionMsgSet.IsRunGateActionIdent(id),
                SendUpdateMsgCore.IsHitFamilyForDelivery(id));
    }

    [Fact]
    public void UnlistedIdentIsZero()
    {
        Assert.Equal(0, SendUpdateMsgCore.ResolveMaxDeliveryTime(Grobal2Const.CM_SAY, Cfg(hit: 66, walk: 44)));
    }
}
