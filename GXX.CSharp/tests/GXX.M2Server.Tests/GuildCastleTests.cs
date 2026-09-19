using System;
using System.Linq;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>Guild.pas 行会系统测试：成员/职位/战争/联盟/行会管理器。</summary>
public class GuildTests
{
    [Fact]
    public void GuildManager_AddFindDel()
    {
        var mgr = new GuildManager();
        Assert.True(mgr.AddGuild("战神行会", "盟主甲"));
        Assert.False(mgr.AddGuild("战神行会", "重复建"), "重名行会应失败");
        var g = mgr.FindGuild("战神行会");
        Assert.NotNull(g);
        Assert.Equal("战神行会", g!.sGuildName);
        Assert.True(mgr.DelGuild("战神行会", out bool found));
        Assert.True(found);
        Assert.Null(mgr.FindGuild("战神行会"));
    }

    [Fact]
    public void Guild_Member_AddRemove_Count()
    {
        var g = new Guild("行会甲");
        Assert.True(g.AddMember2("成员一"));
        Assert.True(g.AddMember2("成员二"));
        Assert.Equal(2, g.Count);
        Assert.True(g.IsMember("成员一"));
        Assert.False(g.IsMember("路人"));
        Assert.True(g.DelMember("成员一"));
        Assert.False(g.IsMember("成员一"));
        Assert.Equal(1, g.Count);
        Assert.False(g.DelMember("不存在"));
    }

    [Fact]
    public void Guild_CancelGuld_OnlySingleChief()
    {
        var g = new Guild("行会乙");
        // 空行会（无职位表）不能取消
        Assert.False(g.CancelGuld("甲"));
        // 掌门 + 成员 → 不能取消
        g.UpdateRank("#1 掌门\r\n甲\r\n成员一");
        Assert.False(g.CancelGuld("甲"));
        // 只剩掌门一人 → 可取消
        g.DelMember("成员一");
        Assert.True(g.CancelGuld("甲"));
    }

    [Fact]
    public void Guild_UpdateRank_ParsesRankFile()
    {
        var g = new Guild("行会丙");
        int rc = g.UpdateRank("#1 掌门<甲>\r\n甲\r\n#2 长老<乙>\r\n长老一 长老二\r\n#99 成员<丙>\r\n小一,小二 小三");
        Assert.Equal(0, rc);
        Assert.Equal(3, g.RankCount);
        Assert.Equal("掌门", g.RankList[0].sRankName);
        Assert.Equal("甲", g.GetChiefName());
        int rankNo = 0;
        var rankName = g.GetRankName("长老一", ref rankNo);
        Assert.Equal(2, rankNo);
        Assert.Contains("长老", rankName);
        // 未变化再次提交 → -1
        Assert.Equal(-1, g.UpdateRank("#1 掌门<甲>\r\n甲\r\n#2 长老<乙>\r\n长老一 长老二\r\n#99 成员<丙>\r\n小一,小二 小三"));
    }

    [Fact]
    public void Guild_UpdateRank_NoChiefRank_Rejected()
    {
        var g = new Guild("行会丁");
        // 无 #1 职位 → -2
        Assert.Equal(-2, g.UpdateRank("#99 成员<丙>\r\n小一"));
    }

    [Fact]
    public void Guild_War_Ally_Relations()
    {
        var mgr = new GuildManager();
        mgr.AddGuild("行会A", "A主");
        mgr.AddGuild("行会B", "B主");
        mgr.AddGuild("行会C", "C主");
        var a = mgr.FindGuild("行会A")!;
        var b = mgr.FindGuild("行会B")!;
        var c = mgr.FindGuild("行会C")!;

        Assert.False(a.IsWarGuild(b));
        Assert.NotNull(a.AddWarGuild(b));
        Assert.True(a.IsWarGuild(b));
        // 战争持续期内有效；模拟超时后失效
        a.StopWarGuild(b);
        Assert.False(a.IsWarGuild(b));

        Assert.False(a.IsAllyGuild(c));
        Assert.True(a.AllyGuild(c));
        Assert.True(a.IsAllyGuild(c));
        Assert.True(a.DelAllyGuild(c));
        Assert.False(a.IsAllyGuild(c));
    }

    [Fact]
    public void GuildManager_MemberOfGuild()
    {
        var mgr = new GuildManager();
        mgr.AddGuild("行会X", "X主");
        var x = mgr.FindGuild("行会X")!;
        x.AddMember2("小甲");
        Assert.Same(x, mgr.MemberOfGuild("小甲"));
        Assert.Null(mgr.MemberOfGuild("无门无派"));
    }
}

/// <summary>Castle.pas 沙巴克城堡测试：战争状态机 / 占城 / 收入。</summary>
public class CastleTests
{
    private static TUserCastle NewCastle()
    {
        var mgr = new GuildManager();
        mgr.AddGuild("守城行会", "守主");
        mgr.AddGuild("攻城行会", "攻主");
        var defender = mgr.FindGuild("守城行会")!;
        var castle = new TUserCastle("沙巴克")
        {
            m_sHomeMap = "0151",
            m_nHomeX = 100,
            m_nHomeY = 100,
            m_nWarRangeX = 100,
            m_nWarRangeY = 100
        };
        castle.SetMasterGuild(defender);
        return castle;
    }

    [Fact]
    public void Castle_InitialOwner()
    {
        var castle = NewCastle();
        Assert.Equal("守城行会", castle.m_sOwnGuild);
        Assert.False(castle.m_boUnderWar);
        Assert.Equal(0, castle.m_nTotalGold);
    }

    [Fact]
    public void Castle_WarLifecycle()
    {
        var mgr = new GuildManager();
        mgr.AddGuild("守城行会", "守主");
        mgr.AddGuild("攻城行会", "攻主");
        var defender = mgr.FindGuild("守城行会")!;
        var attacker = mgr.FindGuild("攻城行会")!;
        var castle = new TUserCastle("沙巴克");
        castle.SetMasterGuild(defender);

        castle.RequestCastleWar(attacker, DateTime.Today);
        // 当日请求 → StartWar 开战
        castle.StartWar(DateTime.Today);
        Assert.True(castle.m_boUnderWar);
        Assert.True(castle.IsAttackGuild(attacker));
        Assert.True(attacker.IsWarGuild(defender), "攻城战期间双方自动进入行会战争");

        // 战争时长耗尽 → Run 结束战争
        castle.ForceWarElapsedForTest();
        castle.Run();
        Assert.False(castle.m_boUnderWar);
    }

    [Fact]
    public void Castle_GetCastle_ChangesOwner()
    {
        var mgr = new GuildManager();
        mgr.AddGuild("守城行会", "守主");
        mgr.AddGuild("攻城行会", "攻主");
        var defender = mgr.FindGuild("守城行会")!;
        var attacker = mgr.FindGuild("攻城行会")!;
        var castle = new TUserCastle("沙巴克");
        castle.SetMasterGuild(defender);

        castle.GetCastle(attacker);
        Assert.Equal("攻城行会", castle.m_sOwnGuild);
        Assert.True(castle.IsMasterGuild(attacker));
    }

    [Fact]
    public void Castle_InCastleWarArea()
    {
        var castle = new TUserCastle("沙巴克")
        {
            m_sHomeMap = "0151",
            m_nHomeX = 100,
            m_nHomeY = 100,
            m_nWarRangeX = 50,
            m_nWarRangeY = 50
        };
        var env = new TEnvirnoment { sMapName = "0151" };
        Assert.True(castle.InCastleWarArea(env, 120, 130));
        Assert.False(castle.InCastleWarArea(env, 200, 130));
    }

    [Fact]
    public void Castle_Gold_Income_Withdraw()
    {
        var castle = NewCastle();
        castle.IncRateGold(10000);
        Assert.Equal(10000, castle.m_nTotalGold);
        // 非成员提取失败
        var stranger = new TScriptPlayer { m_sCharName = "路人" };
        Assert.Equal(-1, castle.WithDrawalGolds(stranger, 100, null));
    }
}
