using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J99：ObjBase.pas GetStruckMessage(30725-30777) / GetMessage(30779-30863) 1:1 测试。
/// </summary>
public sealed class MsgQueueConsumeCoreTests
{
    private static TSendMessage M(int ident, bool late = false, uint deliver = 0, byte[]? buff = null)
        => new() { wIdent = (ushort)ident, boLateDelivery = late, dwDeliveryTime = deliver, Buff = buff };

    private static TProcessMessage P() => new();

    // ===================== GetStruckMessage =====================

    [Fact]
    public void StruckReturnsFirstStruck()
    {
        var list = new List<TSendMessage> { M(Grobal2Const.RM_STRUCK) };
        var p = P();

        Assert.True(MsgQueueConsumeCore.GetStruckMessage(list, p));
        Assert.Equal(Grobal2Const.RM_STRUCK, p.wIdent);
        Assert.Empty(list);
    }

    [Fact]
    public void StruckSkipsNonStruckButKeepsThem()
    {
        // 30741-30745：跳过非 RM_STRUCK 但**不删除**
        var list = new List<TSendMessage>
        {
            M(Grobal2Const.RM_POISON), M(Grobal2Const.RM_STRUCK), M(Grobal2Const.RM_MAGSTRUCK),
        };
        var p = P();

        Assert.True(MsgQueueConsumeCore.GetStruckMessage(list, p));

        Assert.Equal(2, list.Count);
        Assert.DoesNotContain(list, m => m.wIdent == Grobal2Const.RM_STRUCK);
        Assert.Contains(list, m => m.wIdent == Grobal2Const.RM_POISON);
        Assert.Contains(list, m => m.wIdent == Grobal2Const.RM_MAGSTRUCK);
    }

    [Fact]
    public void StruckReturnsFalseWhenAbsent()
    {
        var list = new List<TSendMessage> { M(Grobal2Const.RM_POISON) };
        var p = P();

        Assert.False(MsgQueueConsumeCore.GetStruckMessage(list, p));
        Assert.Equal(0, p.wIdent);          // 30737：仍清零
        Assert.Single(list);                 // 未取到则不删任何消息
    }

    [Fact]
    public void StruckClearsIdentEvenOnFailure()
    {
        var p = P();
        p.wIdent = 999;

        MsgQueueConsumeCore.GetStruckMessage(new List<TSendMessage>(), p);

        Assert.Equal(0, p.wIdent);
    }

    [Fact]
    public void StruckTakesOnlyOnePerCall()
    {
        var list = new List<TSendMessage> { M(Grobal2Const.RM_STRUCK), M(Grobal2Const.RM_STRUCK) };

        Assert.True(MsgQueueConsumeCore.GetStruckMessage(list, P()));
        Assert.Single(list);      // 30770：Break，只取一条
    }

    [Fact]
    public void StruckIgnoresLateDeliveryGate()
    {
        // 本函数**无**延时判定：即使未到点也照取
        var list = new List<TSendMessage> { M(Grobal2Const.RM_STRUCK, late: true, deliver: 99999) };
        var p = P();

        Assert.True(MsgQueueConsumeCore.GetStruckMessage(list, p));
        Assert.Empty(list);
    }

    [Fact]
    public void StruckCopiesAllFields()
    {
        var list = new List<TSendMessage>
        {
            new()
            {
                wIdent = Grobal2Const.RM_STRUCK, wParam = 11, nParam1 = 22, nParam2 = 33, nParam3 = 44,
                dwTimeTick = 55, dwDeliveryTime = 66, boLateDelivery = true,
            },
        };
        var p = P();

        MsgQueueConsumeCore.GetStruckMessage(list, p);

        Assert.Equal(11, p.wParam);
        Assert.Equal(22, p.nParam1);
        Assert.Equal(33, p.nParam2);
        Assert.Equal(44, p.nParam3);
        Assert.Equal(55u, p.dwTimeTick);
        Assert.Equal(66u, p.dwDeliveryTime);
        Assert.True(p.boLateDelivery);
    }

    [Fact]
    public void StruckRestoresBuffString()
    {
        var buff = System.Text.Encoding.ASCII.GetBytes("hello");
        var list = new List<TSendMessage> { M(Grobal2Const.RM_STRUCK, buff: buff) };
        var p = P();

        MsgQueueConsumeCore.GetStruckMessage(list, p);

        Assert.Equal("hello", p.sMsg);
    }

    [Fact]
    public void StruckEmptyBuffYieldsEmptyString()
    {
        var list = new List<TSendMessage> { M(Grobal2Const.RM_STRUCK) };
        var p = P();
        p.sMsg = "stale";

        MsgQueueConsumeCore.GetStruckMessage(list, p);

        Assert.Equal("", p.sMsg);       // 30765
    }

    [Fact]
    public void StruckZeroLengthBuffTreatedAsEmpty()
    {
        // 30756：需 BuffLen > 0，故长度 0 走 else
        var list = new List<TSendMessage> { M(Grobal2Const.RM_STRUCK, buff: Array.Empty<byte>()) };
        var p = P();
        p.sMsg = "stale";

        MsgQueueConsumeCore.GetStruckMessage(list, p);

        Assert.Equal("", p.sMsg);
    }

    // ===================== GetMessage：延时门（核心） =====================

    [Fact]
    public void WaitGateTrueWhenNotYetDue()
    {
        Assert.True(MsgQueueConsumeCore.ShouldWaitForDelivery(
            Grobal2Const.RM_POISON, 100, boLateDelivery: true, now: 50, dwDeliveryTime: 100));
    }

    [Fact]
    public void WaitGateFalseWhenDue()
    {
        // now >= dwDeliveryTime → 到点，不再等待
        Assert.False(MsgQueueConsumeCore.ShouldWaitForDelivery(
            Grobal2Const.RM_POISON, 100, true, now: 100, dwDeliveryTime: 100));
        Assert.False(MsgQueueConsumeCore.ShouldWaitForDelivery(
            Grobal2Const.RM_POISON, 100, true, now: 200, dwDeliveryTime: 100));
    }

    [Fact]
    public void WaitGateFalseWhenNotMarkedLate()
    {
        Assert.False(MsgQueueConsumeCore.ShouldWaitForDelivery(
            Grobal2Const.RM_POISON, 100, false, 50, 100));
    }

    [Fact]
    public void Rm10101WithPositiveHpWaits()
    {
        // (ident = RM_10101 and HP > 0) → 门为真
        Assert.True(MsgQueueConsumeCore.ShouldWaitForDelivery(
            Grobal2Const.RM_10101, m_WAbilHP: 1, true, 50, 100));
    }

    [Fact]
    public void Rm10101WithZeroHpDoesNotWait()
    {
        // **核心防秒杀语义**：RM_10101 且 HP <= 0 时门为假 → 立即取出
        Assert.False(MsgQueueConsumeCore.ShouldWaitForDelivery(
            Grobal2Const.RM_10101, m_WAbilHP: 0, true, 50, 100));
    }

    [Fact]
    public void Rm10101WithNegativeHpDoesNotWait()
    {
        Assert.False(MsgQueueConsumeCore.ShouldWaitForDelivery(
            Grobal2Const.RM_10101, -10, true, 50, 100));
    }

    [Fact]
    public void NonRm10101WaitsRegardlessOfHp()
    {
        // ident <> RM_10101 → 门为真，与 HP 无关
        foreach (int hp in new[] { -10, 0, 1, 999 })
            Assert.True(MsgQueueConsumeCore.ShouldWaitForDelivery(
                Grobal2Const.RM_POISON, hp, true, 50, 100), $"HP {hp} 应不影响非 RM_10101");
    }

    [Fact]
    public void WaitGateMatchesSourceExpressionExhaustively()
    {
        // 与原文表达式逐值对照：
        // ((ident = RM_10101) and (HP > 0)) or (ident <> RM_10101)
        //   and boLateDelivery and (now < dwDeliveryTime)
        foreach (int ident in new[] { Grobal2Const.RM_10101, Grobal2Const.RM_STRUCK, Grobal2Const.RM_POISON })
        foreach (int hp in new[] { -1, 0, 1 })
        foreach (bool late in new[] { false, true })
        foreach (uint now in new[] { 50u, 100u, 150u })
        foreach (uint due in new[] { 0u, 100u, 200u })
        {
            bool expected = ((ident == Grobal2Const.RM_10101 && hp > 0) || ident != Grobal2Const.RM_10101)
                            && late
                            && now < due;

            Assert.Equal(expected, MsgQueueConsumeCore.ShouldWaitForDelivery(ident, hp, late, now, due));
        }
    }

    // ===================== GetMessage：行为 =====================

    [Fact]
    public void GetMessageTakesFirstAvailable()
    {
        var list = new List<TSendMessage> { M(Grobal2Const.RM_POISON), M(Grobal2Const.CM_HIT) };
        var p = P();

        Assert.True(MsgQueueConsumeCore.GetMessage(list, p, now: 0, m_WAbilHP: 100));
        Assert.Equal(Grobal2Const.RM_POISON, p.wIdent);
        Assert.Single(list);
    }

    [Fact]
    public void GetMessageSkipsNotYetDueAndKeepsIt()
    {
        // 30813：未到点则 Inc 跳过、**保留在队列**
        var list = new List<TSendMessage>
        {
            M(Grobal2Const.RM_POISON, late: true, deliver: 500),
            M(Grobal2Const.CM_HIT),
        };
        var p = P();

        Assert.True(MsgQueueConsumeCore.GetMessage(list, p, now: 100, m_WAbilHP: 100));

        Assert.Equal(Grobal2Const.CM_HIT, p.wIdent);      // 取到后面那条
        Assert.Single(list);                               // 延时的仍在
        Assert.Equal(Grobal2Const.RM_POISON, list[0].wIdent);
    }

    [Fact]
    public void GetMessageReturnsFalseWhenAllWaiting()
    {
        var list = new List<TSendMessage> { M(Grobal2Const.RM_POISON, late: true, deliver: 500) };
        var p = P();

        Assert.False(MsgQueueConsumeCore.GetMessage(list, p, now: 100, m_WAbilHP: 100));
        Assert.Equal(0, p.wIdent);
        Assert.Single(list);
    }

    [Fact]
    public void GetMessageTakesDueMessage()
    {
        var list = new List<TSendMessage> { M(Grobal2Const.RM_POISON, late: true, deliver: 100) };
        var p = P();

        Assert.True(MsgQueueConsumeCore.GetMessage(list, p, now: 100, m_WAbilHP: 100));
        Assert.Empty(list);
    }

    [Fact]
    public void GetMessageRm10101EscapesWaitWhenDead()
    {
        // 端到端验证防秒杀语义：RM_10101 + HP 0 → 即使未到点也立即取出
        var list = new List<TSendMessage> { M(Grobal2Const.RM_10101, late: true, deliver: 9999) };
        var p = P();

        Assert.True(MsgQueueConsumeCore.GetMessage(list, p, now: 0, m_WAbilHP: 0));
        Assert.Equal(Grobal2Const.RM_10101, p.wIdent);
        Assert.Empty(list);
    }

    [Fact]
    public void GetMessageRm10101WaitsWhenAlive()
    {
        var list = new List<TSendMessage> { M(Grobal2Const.RM_10101, late: true, deliver: 9999) };
        var p = P();

        Assert.False(MsgQueueConsumeCore.GetMessage(list, p, now: 0, m_WAbilHP: 100));
        Assert.Single(list);
    }

    [Fact]
    public void GetMessageDeletesNullEntry()
    {
        // 30801-30806：nil 条目删除后 Continue（不 Inc）
        var list = new List<TSendMessage> { null!, M(Grobal2Const.CM_HIT) };
        var p = P();

        Assert.True(MsgQueueConsumeCore.GetMessage(list, p, 0, 100));
        Assert.Equal(Grobal2Const.CM_HIT, p.wIdent);
        Assert.Empty(list);      // null 被删且 CM_HIT 被取
    }

    [Fact]
    public void GetMessageHandlesMultipleNullsThenTakes()
    {
        var list = new List<TSendMessage> { null!, null!, null!, M(Grobal2Const.CM_HIT) };
        var p = P();

        Assert.True(MsgQueueConsumeCore.GetMessage(list, p, 0, 100));
        Assert.Empty(list);
    }

    [Fact]
    public void GetMessageOnlyNullsReturnsFalse()
    {
        var list = new List<TSendMessage> { null!, null! };
        var p = P();

        Assert.False(MsgQueueConsumeCore.GetMessage(list, p, 0, 100));
        Assert.Empty(list);      // null 仍被清除
    }

    [Fact]
    public void GetMessageEmptyReturnsFalse()
    {
        var p = P();
        Assert.False(MsgQueueConsumeCore.GetMessage(new List<TSendMessage>(), p, 0, 100));
        Assert.Equal(0, p.wIdent);
    }

    [Fact]
    public void GetMessageTakesOnlyOnePerCall()
    {
        var list = new List<TSendMessage> { M(Grobal2Const.CM_HIT), M(Grobal2Const.CM_WALK) };

        Assert.True(MsgQueueConsumeCore.GetMessage(list, P(), 0, 100));
        Assert.Single(list);
    }

    [Fact]
    public void GetMessageRestoresBuff()
    {
        var buff = System.Text.Encoding.ASCII.GetBytes("abc");
        var list = new List<TSendMessage> { M(Grobal2Const.CM_HIT, buff: buff) };
        var p = P();

        MsgQueueConsumeCore.GetMessage(list, p, 0, 100);

        Assert.Equal("abc", p.sMsg);
    }

    [Fact]
    public void GetMessageEmptyBuffYieldsEmptyString()
    {
        var list = new List<TSendMessage> { M(Grobal2Const.CM_HIT) };
        var p = P();
        p.sMsg = "stale";

        MsgQueueConsumeCore.GetMessage(list, p, 0, 100);

        Assert.Equal("", p.sMsg);
    }

    // ===================== 两函数差异 =====================

    [Fact]
    public void StruckSelectiveVsMessageGeneral()
    {
        // 同一队列：GetStruckMessage 跳过非 STRUCK 取 STRUCK；
        // GetMessage 直接取队首
        var a = new List<TSendMessage> { M(Grobal2Const.RM_POISON), M(Grobal2Const.RM_STRUCK) };
        var b = new List<TSendMessage> { M(Grobal2Const.RM_POISON), M(Grobal2Const.RM_STRUCK) };

        var pa = P();
        var pb = P();

        MsgQueueConsumeCore.GetStruckMessage(a, pa);
        MsgQueueConsumeCore.GetMessage(b, pb, 0, 100);

        Assert.Equal(Grobal2Const.RM_STRUCK, pa.wIdent);
        Assert.Equal(Grobal2Const.RM_POISON, pb.wIdent);
    }

    [Fact]
    public void OnlyGetMessageHonoursDelayGate()
    {
        // 同一条未到点的 RM_STRUCK：GetStruckMessage 取走，GetMessage 保留
        var a = new List<TSendMessage> { M(Grobal2Const.RM_STRUCK, late: true, deliver: 9999) };
        var b = new List<TSendMessage> { M(Grobal2Const.RM_STRUCK, late: true, deliver: 9999) };

        Assert.True(MsgQueueConsumeCore.GetStruckMessage(a, P()));
        Assert.Empty(a);

        Assert.False(MsgQueueConsumeCore.GetMessage(b, P(), 0, 100));
        Assert.Single(b);
    }
}
