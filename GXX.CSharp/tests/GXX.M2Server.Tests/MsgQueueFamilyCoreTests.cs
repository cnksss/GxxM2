using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J98：ObjBase.pas SendUpdateBaseObject(30608-30648) / SendUseItemMsg(30650-30685) /
/// SendActionMsg(30687-30723) 1:1 测试。
/// </summary>
public sealed class MsgQueueFamilyCoreTests
{
    private static TSendMessage M(int ident, object? bo = null, nint p1 = 0, bool late = false)
        => new() { wIdent = (ushort)ident, BaseObject = bo, nParam1 = p1, boLateDelivery = late };

    // ===================== SendUpdateBaseObject：wIdent + BaseObject =====================

    [Fact]
    public void BaseObjectDeletesSameIdentAndObject()
    {
        var a = new object();
        var list = new List<TSendMessage> { M(Grobal2Const.CM_HIT, a), M(Grobal2Const.CM_HIT, a) };

        var r = MsgQueueFamilyCore.DrainForUpdateBaseObject(list, Grobal2Const.CM_HIT, a);

        Assert.Equal(2, r.RemovedCount);
        Assert.Empty(list);
    }

    [Fact]
    public void BaseObjectKeepsSameIdentDifferentObject()
    {
        // 30626：对象用**引用相等**比较
        var a = new object();
        var b = new object();
        var list = new List<TSendMessage> { M(Grobal2Const.CM_HIT, a), M(Grobal2Const.CM_HIT, b) };

        var r = MsgQueueFamilyCore.DrainForUpdateBaseObject(list, Grobal2Const.CM_HIT, a);

        Assert.Equal(1, r.RemovedCount);
        Assert.Single(list);
        Assert.Same(b, list[0].BaseObject);
    }

    [Fact]
    public void BaseObjectKeepsDifferentIdentSameObject()
    {
        var a = new object();
        var list = new List<TSendMessage> { M(Grobal2Const.CM_HIT, a), M(Grobal2Const.CM_WALK, a) };

        var r = MsgQueueFamilyCore.DrainForUpdateBaseObject(list, Grobal2Const.CM_HIT, a);

        Assert.Equal(1, r.RemovedCount);
        Assert.Single(list);
        Assert.Equal(Grobal2Const.CM_WALK, list[0].wIdent);
    }

    [Fact]
    public void BaseObjectNullMatchesNull()
    {
        // 引用相等下 null == null
        var list = new List<TSendMessage> { M(Grobal2Const.CM_HIT, null), M(Grobal2Const.CM_HIT, new object()) };

        var r = MsgQueueFamilyCore.DrainForUpdateBaseObject(list, Grobal2Const.CM_HIT, null);

        Assert.Equal(1, r.RemovedCount);
        Assert.Single(list);
        Assert.NotNull(list[0].BaseObject);
    }

    [Fact]
    public void BaseObjectFlagsLateWithoutAccumulatingTime()
    {
        var a = new object();
        var list = new List<TSendMessage> { M(Grobal2Const.CM_HIT, a, late: true) };

        var r = MsgQueueFamilyCore.DrainForUpdateBaseObject(list, Grobal2Const.CM_HIT, a);

        Assert.True(r.BoLateDelivery);
        Assert.Equal(0u, r.DwDeliveryTime);      // 30629-30630 只置标志
    }

    [Fact]
    public void BaseObjectHandlesAdjacentDuplicates()
    {
        var a = new object();
        var list = new List<TSendMessage> { M(Grobal2Const.CM_HIT, a), M(Grobal2Const.CM_HIT, a), M(Grobal2Const.CM_HIT, a) };

        Assert.Equal(3, MsgQueueFamilyCore.DrainForUpdateBaseObject(list, Grobal2Const.CM_HIT, a).RemovedCount);
        Assert.Empty(list);
    }

    [Fact]
    public void BaseObjectFinishUsesZeroDelayWhenLate()
    {
        var late = MsgQueueFamilyCore.FinishUpdateBaseObject(true);
        Assert.Equal(SendUpdateKind.SendDelayMsg, late.Kind);
        Assert.Equal(0u, late.DeliveryTime);

        var ok = MsgQueueFamilyCore.FinishUpdateBaseObject(false);
        Assert.Equal(SendUpdateKind.SendMsg, ok.Kind);
    }

    // ===================== SendUseItemMsg：计数 + 动态延迟 =====================

    [Fact]
    public void UseItemSetHas16Members()
    {
        // 由脚本从 30665-30670 独立抽取核对
        int n = 0;
        foreach (int id in new[]
                 {
                     Grobal2Const.CM_EAT, Grobal2Const.CM_TURN, Grobal2Const.CM_WALK,
                     Grobal2Const.CM_SITDOWN, Grobal2Const.CM_HORSERUN, Grobal2Const.CM_RUN,
                     Grobal2Const.CM_HIT, Grobal2Const.CM_HEAVYHIT, Grobal2Const.CM_BIGHIT,
                     Grobal2Const.CM_POWERHIT, Grobal2Const.CM_LONGHIT, Grobal2Const.CM_WIDEHIT,
                     Grobal2Const.CM_FIREHIT, Grobal2Const.CM_SWORDHIT,
                     Grobal2Const.CM_113HIT, Grobal2Const.CM_115HIT,
                 })
            if (MsgQueueFamilyCore.IsUseItemCountIdent(id)) n++;

        Assert.Equal(16, n);
    }

    [Fact]
    public void UseItemSetIncludesEatAndMovement()
    {
        foreach (int id in new[]
                 {
                     Grobal2Const.CM_EAT, Grobal2Const.CM_TURN, Grobal2Const.CM_WALK,
                     Grobal2Const.CM_SITDOWN, Grobal2Const.CM_HORSERUN, Grobal2Const.CM_RUN,
                 })
            Assert.True(MsgQueueFamilyCore.IsUseItemCountIdent(id));
    }

    [Fact]
    public void UseItemSetExcludesCrsHitAnd43Etc()
    {
        // 30665-30671 确实不含这些
        foreach (int id in new[]
                 {
                     Grobal2Const.CM_CRSHIT, Grobal2Const.CM_43HIT, Grobal2Const.CM_66HIT,
                     Grobal2Const.CM_66HIT1, Grobal2Const.CM_101HIT, Grobal2Const.CM_102HIT,
                     Grobal2Const.CM_103HIT, Grobal2Const.CM_TWNHIT,
                 })
            Assert.False(MsgQueueFamilyCore.IsUseItemCountIdent(id), $"CM 码 {id} 不应计入");
    }

    [Fact]
    public void UseItemSetIncludesCustomRange()
    {
        Assert.True(MsgQueueFamilyCore.IsUseItemCountIdent(6000));
        Assert.True(MsgQueueFamilyCore.IsUseItemCountIdent(6299));
        Assert.False(MsgQueueFamilyCore.IsUseItemCountIdent(6300));
    }

    [Fact]
    public void UseItemCountsListWithoutDeleting()
    {
        // 30662 是 for 循环**只计数、不删除**
        var list = new List<TSendMessage>
        {
            M(Grobal2Const.CM_EAT), M(Grobal2Const.CM_SPELL), M(Grobal2Const.CM_HIT), M(6000),
        };

        Assert.Equal(3, MsgQueueFamilyCore.CountUseItemMsgs(list));
        Assert.Equal(4, list.Count);      // 未被删除
    }

    [Fact]
    public void UseItemCountEmptyIsZero()
    {
        Assert.Equal(0, MsgQueueFamilyCore.CountUseItemMsgs(new List<TSendMessage>()));
    }

    [Fact]
    public void UseItemDelayIsTwentyPerMsg()
    {
        // 30682：Min(500, nMsg * 20)
        Assert.Equal(20u, MsgQueueFamilyCore.FinishUseItemMsg(1).DeliveryTime);
        Assert.Equal(100u, MsgQueueFamilyCore.FinishUseItemMsg(5).DeliveryTime);
    }

    [Fact]
    public void UseItemDelayCapsAt500()
    {
        Assert.Equal(500u, MsgQueueFamilyCore.FinishUseItemMsg(25).DeliveryTime);
        Assert.Equal(500u, MsgQueueFamilyCore.FinishUseItemMsg(1000).DeliveryTime);
    }

    [Fact]
    public void UseItemZeroCountSendsImmediately()
    {
        // 30683：nMsg = 0 时走普通 SendMsg
        var o = MsgQueueFamilyCore.FinishUseItemMsg(0);
        Assert.Equal(SendUpdateKind.SendMsg, o.Kind);
        Assert.Equal(0u, o.DeliveryTime);
    }

    [Fact]
    public void UseItemNonZeroCountUsesDelay()
    {
        Assert.Equal(SendUpdateKind.SendDelayMsg, MsgQueueFamilyCore.FinishUseItemMsg(3).Kind);
    }

    [Fact]
    public void UseItemIsOnlyFamilyUsingComputedDelay()
    {
        // 与另外两个函数不同：本函数延迟随计数变化
        Assert.NotEqual(
            MsgQueueFamilyCore.FinishUseItemMsg(1).DeliveryTime,
            MsgQueueFamilyCore.FinishUseItemMsg(2).DeliveryTime);
    }

    // ===================== SendActionMsg：按动作集合清除 =====================

    [Fact]
    public void ActionMsgSetHas15Members()
    {
        int n = 0;
        foreach (int id in new[]
                 {
                     Grobal2Const.CM_TURN, Grobal2Const.CM_WALK, Grobal2Const.CM_SITDOWN,
                     Grobal2Const.CM_HORSERUN, Grobal2Const.CM_RUN, Grobal2Const.CM_HIT,
                     Grobal2Const.CM_HEAVYHIT, Grobal2Const.CM_BIGHIT, Grobal2Const.CM_POWERHIT,
                     Grobal2Const.CM_LONGHIT, Grobal2Const.CM_WIDEHIT, Grobal2Const.CM_FIREHIT,
                     Grobal2Const.CM_SWORDHIT, Grobal2Const.CM_113HIT, Grobal2Const.CM_115HIT,
                 })
            if (MsgQueueFamilyCore.IsActionMsgClearIdent(id)) n++;

        Assert.Equal(15, n);
    }

    [Fact]
    public void ActionMsgSetExcludesEatUnlikeUseItem()
    {
        // 两个集合高度相似但不同：SendActionMsg **不含 CM_EAT**
        Assert.True(MsgQueueFamilyCore.IsUseItemCountIdent(Grobal2Const.CM_EAT));
        Assert.False(MsgQueueFamilyCore.IsActionMsgClearIdent(Grobal2Const.CM_EAT));
    }

    [Fact]
    public void ActionMsgSetExcludesCrsHitAnd43Etc()
    {
        foreach (int id in new[]
                 {
                     Grobal2Const.CM_CRSHIT, Grobal2Const.CM_43HIT, Grobal2Const.CM_66HIT,
                     Grobal2Const.CM_66HIT1, Grobal2Const.CM_101HIT, Grobal2Const.CM_102HIT,
                     Grobal2Const.CM_103HIT, Grobal2Const.CM_TWNHIT,
                 })
            Assert.False(MsgQueueFamilyCore.IsActionMsgClearIdent(id));
    }

    [Fact]
    public void ActionMsgSetIncludesCustomRange()
    {
        Assert.True(MsgQueueFamilyCore.IsActionMsgClearIdent(6000));
        Assert.False(MsgQueueFamilyCore.IsActionMsgClearIdent(6300));
    }

    [Fact]
    public void TwoSetsDifferExactlyOnEat()
    {
        // 冗余保护：两集合的差异**应恰为 CM_EAT**（多为共有、EAT 只在 UseItem）
        int diff = 0;
        for (int id = 1000; id < 6400; id++)
        {
            bool u = MsgQueueFamilyCore.IsUseItemCountIdent(id);
            bool a = MsgQueueFamilyCore.IsActionMsgClearIdent(id);
            if (u != a) diff++;
        }

        Assert.Equal(1, diff);
    }

    [Fact]
    public void ActionMsgDrainsAllActionMessages()
    {
        var list = new List<TSendMessage>
        {
            M(Grobal2Const.CM_TURN), M(Grobal2Const.CM_HIT), M(Grobal2Const.CM_WALK), M(6000),
        };

        int removed = MsgQueueFamilyCore.DrainForActionMsg(list);

        Assert.Equal(4, removed);
        Assert.Empty(list);
    }

    [Fact]
    public void ActionMsgKeepsNonActionMessages()
    {
        var list = new List<TSendMessage>
        {
            M(Grobal2Const.CM_SPELL), M(Grobal2Const.CM_HIT), M(Grobal2Const.CM_SAY),
        };

        int removed = MsgQueueFamilyCore.DrainForActionMsg(list);

        Assert.Equal(1, removed);
        Assert.Equal(2, list.Count);
        Assert.DoesNotContain(list, m => m.wIdent == Grobal2Const.CM_HIT);
    }

    [Fact]
    public void ActionMsgKeepsEat()
    {
        // CM_EAT 不在本集合，故被保留
        var list = new List<TSendMessage> { M(Grobal2Const.CM_EAT), M(Grobal2Const.CM_HIT) };

        Assert.Equal(1, MsgQueueFamilyCore.DrainForActionMsg(list));
        Assert.Single(list);
        Assert.Equal(Grobal2Const.CM_EAT, list[0].wIdent);
    }

    [Fact]
    public void ActionMsgHandlesAdjacentDuplicates()
    {
        var list = new List<TSendMessage>
        {
            M(Grobal2Const.CM_HIT), M(Grobal2Const.CM_HIT), M(Grobal2Const.CM_HIT),
        };

        Assert.Equal(3, MsgQueueFamilyCore.DrainForActionMsg(list));
        Assert.Empty(list);
    }

    [Fact]
    public void ActionMsgAlwaysSendsPlainMsg()
    {
        // 30722：本函数**无条件** SendMsg，不因迟到改走延迟
        Assert.Equal(SendUpdateKind.SendMsg, MsgQueueFamilyCore.FinishActionMsg().Kind);
        Assert.Equal(0u, MsgQueueFamilyCore.FinishActionMsg().DeliveryTime);
    }

    // ===================== 家族差异固化 =====================

    [Fact]
    public void ThreeDeleteConditionsAreDistinct()
    {
        // 冗余保护：证明"只比 wIdent"、"wIdent+nParam1"、"wIdent+BaseObject" 三者不同
        var a = new object();
        var mk = () => new List<TSendMessage>
        {
            M(Grobal2Const.CM_HIT, a, p1: 1),
            M(Grobal2Const.CM_HIT, new object(), p1: 2),
        };

        // 仅 wIdent → 删 2
        var onlyIdent = mk();
        int byIdent = SendUpdateMsgCore.DrainForUpdateMsg(onlyIdent, Grobal2Const.CM_HIT).RemovedCount;

        // wIdent + nParam1 → 删 1
        int byParam = SendDelayMsgCore.DrainForUpdateDelayMsg(mk(), Grobal2Const.CM_HIT, 1);

        // wIdent + BaseObject → 删 1
        int byObject = MsgQueueFamilyCore.DrainForUpdateBaseObject(mk(), Grobal2Const.CM_HIT, a).RemovedCount;

        Assert.Equal(2, byIdent);
        Assert.Equal(1, byParam);
        Assert.Equal(1, byObject);
    }

    [Fact]
    public void ParamAndObjectConditionsDifferOnSameFixture()
    {
        // 构造让两种条件结果不同的队列
        var a = new object();
        var list1 = new List<TSendMessage> { M(Grobal2Const.CM_HIT, a, p1: 9) };
        var list2 = new List<TSendMessage> { M(Grobal2Const.CM_HIT, a, p1: 9) };

        int byParam = SendDelayMsgCore.DrainForUpdateDelayMsg(list1, Grobal2Const.CM_HIT, 1);
        int byObject = MsgQueueFamilyCore.DrainForUpdateBaseObject(list2, Grobal2Const.CM_HIT, a).RemovedCount;

        Assert.Equal(0, byParam);      // param1 不匹配
        Assert.Equal(1, byObject);     // 对象匹配
    }
}
