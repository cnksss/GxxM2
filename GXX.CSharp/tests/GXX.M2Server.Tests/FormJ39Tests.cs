using GXX.M2Server.Engine;
using System.Collections.Generic;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J39：NPC 脚本 Stub 逐条深化（条件/动作基于 TScriptPlayer 状态的 1:1 实现）。</summary>
public sealed class NpcStubDeepenTests
{
    private static TScriptPlayer NewPlayer(int level = 30, int gold = 1000)
    {
        var p = new TScriptPlayer
        {
            m_sCharName = "测试员",
            m_btJob = 0,
            m_btGender = 0,
            m_boAdmin = false,
            Gold = gold,
            GameGold = 500,
            GamePoint = 200,
            CreditPoint = 100,
            BonusPoint = 50,
            PKPoint = 10,
            Exp = 50000,
        };
        p.m_wAbil.Level = (uint)level;
        return p;
    }

    [Fact]
    public void Condition_CheckTextLength()
    {
        var eng = new NpcScriptEngine();
        var npc = new TNormNpc("测试NPC");
        var p = NewPlayer();
        // ConditionOfCheckStringLength：比较符 =,>,<，其余一律按 >= 处理
        Assert.True(eng.CheckConditionText(npc, p, "CHECKTEXTLENGTH HELLO = 5"));
        Assert.False(eng.CheckConditionText(npc, p, "CHECKTEXTLENGTH HELLO = 3"));
        Assert.True(eng.CheckConditionText(npc, p, "CHECKTEXTLENGTH HELLO < 6"));
        Assert.True(eng.CheckConditionText(npc, p, "CHECKTEXTLENGTH HELLO > 4"));
        Assert.True(eng.CheckConditionText(npc, p, "CHECKTEXTLENGTH HELLO X 5"));
        Assert.False(eng.CheckConditionText(npc, p, "CHECKTEXTLENGTH HELLO X 6"));
    }

    [Fact]
    public void Condition_CheckNameListPosition()
    {
        var eng = new NpcScriptEngine();
        var npc = new TNormNpc("测试NPC");
        var p = NewPlayer();
        // ConditionOfCheckNameListPostion：人物名 0 基行号 >= 参数 为真；';' 注释行跳过；名单不存在为假
        TScriptPlayer.NameListFiles.Clear();
        TScriptPlayer.NameListFiles["VIP名单"] = new List<string> { "; 注释行", "", "别人", "测试员" };
        Assert.True(eng.CheckConditionText(npc, p, "CHECKNAMELISTPOSITION VIP名单 0"));
        Assert.True(eng.CheckConditionText(npc, p, "CHECKNAMELISTPOSITION VIP名单 3"));
        Assert.False(eng.CheckConditionText(npc, p, "CHECKNAMELISTPOSITION VIP名单 4"));
        Assert.False(eng.CheckConditionText(npc, p, "CHECKNAMELISTPOSITION 不存在名单 0"));
    }

    [Fact]
    public void Condition_CheckContainText_And_CompareText()
    {
        var eng = new NpcScriptEngine();
        var npc = new TNormNpc("测试NPC");
        var p = NewPlayer();
        Assert.True(eng.CheckConditionText(npc, p, "CHECKCONTAINSTEXT 你好世界 你好"));
        Assert.False(eng.CheckConditionText(npc, p, "CHECKCONTAINSTEXT 你好世界 再见"));
        Assert.True(eng.CheckConditionText(npc, p, "COMPARETEXT ABC ABC"));
        Assert.True(eng.CheckConditionText(npc, p, "COMPARETEXT ABC abc"));
        Assert.False(eng.CheckConditionText(npc, p, "COMPARETEXT ABC DEF"));
    }

    [Fact]
    public void Action_AddSkill_DelSkill_ClearSkill()
    {
        var eng = new NpcScriptEngine();
        var npc = new TNormNpc("测试NPC");
        var p = NewPlayer();
        eng.RunActionText(npc, p, "ADDSKILL 火球术");
        Assert.Contains("火球术", p.Skills);
        eng.RunActionText(npc, p, "DELSKILL 火球术");
        Assert.DoesNotContain("火球术", p.Skills);
        p.Skills.Add("治愈术");
        eng.RunActionText(npc, p, "CLEARSKILL");
        Assert.Empty(p.Skills);
    }

    [Fact]
    public void Action_Give_Take_Items()
    {
        var eng = new NpcScriptEngine();
        var npc = new TNormNpc("测试NPC");
        var p = NewPlayer();
        eng.RunActionText(npc, p, "GIVE 木剑 1");
        Assert.Equal(1, p.Items["木剑"]);
        eng.RunActionText(npc, p, "TAKE 木剑 1");
        Assert.Equal(0, p.Items["木剑"]);
    }

    [Fact]
    public void Action_GameGold_Ops()
    {
        var eng = new NpcScriptEngine();
        var npc = new TNormNpc("测试NPC");
        var p = NewPlayer();
        eng.RunActionText(npc, p, "GAMEGOLD + 100");
        Assert.Equal(600, p.GameGold);
        eng.RunActionText(npc, p, "GAMEGOLD - 50");
        Assert.Equal(550, p.GameGold);
    }

    [Fact]
    public void Action_ChangeLevel_ChangePkPoint_ChangeExp()
    {
        var eng = new NpcScriptEngine();
        var npc = new TNormNpc("测试NPC");
        var p = NewPlayer();
        eng.RunActionText(npc, p, "CHANGELEVEL 40");
        Assert.Equal(40u, p.Level);
        // ActionOfChangePkPoint / ActionOfChangeExp：方法符 =,-,+ 绝对/减/增
        eng.RunActionText(npc, p, "CHANGEPKPOINT = 0");
        Assert.Equal(0, p.PKPoint);
        eng.RunActionText(npc, p, "CHANGEPKPOINT + 7");
        Assert.Equal(7, p.PKPoint);
        eng.RunActionText(npc, p, "CHANGEPKPOINT - 10");
        Assert.Equal(0, p.PKPoint);
        Assert.False(eng.RunActionText(npc, p, "CHANGEPKPOINT + -5"));
        eng.RunActionText(npc, p, "CHANGEEXP = 99999");
        Assert.Equal(99999, (long)p.Exp);
        eng.RunActionText(npc, p, "CHANGEEXP - 100000");
        Assert.Equal(0, (long)p.Exp);
    }

    [Fact]
    public void Action_ChangeJob_ChangeGender()
    {
        var eng = new NpcScriptEngine();
        var npc = new TNormNpc("测试NPC");
        var p = NewPlayer();
        // ActionOfChangeJob：数值优先，WAR/WIZ/TAO 前缀或中文覆盖；非法 → 错误且职业不变
        eng.RunActionText(npc, p, "CHANGEJOB 1");
        Assert.Equal(1, (int)p.m_btJob);
        Assert.True(eng.RunActionText(npc, p, "CHANGEJOB 道士"));
        Assert.Equal(2, (int)p.m_btJob);
        Assert.True(eng.RunActionText(npc, p, "CHANGEJOB WIZARD"));
        Assert.Equal(1, (int)p.m_btJob);
        Assert.False(eng.RunActionText(npc, p, "CHANGEJOB 9"));
        Assert.Equal(1, (int)p.m_btJob);
        // ActionOfChangeGender：0/1 绝对设定 + 发型缺陷规则
        eng.RunActionText(npc, p, "CHANGEGENDER 1");
        Assert.Equal(1, (int)p.m_btGender);
        p.m_btHair = 2;
        eng.RunActionText(npc, p, "CHANGEGENDER 0");
        Assert.Equal(0, (int)p.m_btGender);
        Assert.Equal(2, (int)p.m_btHair);
        eng.RunActionText(npc, p, "CHANGEGENDER 1");
        Assert.Equal(1, (int)p.m_btGender);
        Assert.Equal(1, (int)p.m_btHair);
        Assert.False(eng.RunActionText(npc, p, "CHANGEGENDER 5"));
        Assert.Equal(1, (int)p.m_btGender);
    }

    [Fact]
    public void Action_Mov_Inc_Dec_On_PVar()
    {
        var eng = new NpcScriptEngine();
        var npc = new TNormNpc("测试NPC");
        var p = NewPlayer();
        eng.RunActionText(npc, p, "MOV P1 100");
        Assert.Equal(100, p.GetPVar(1));
        eng.RunActionText(npc, p, "INC P1 50");
        Assert.Equal(150, p.GetPVar(1));
        eng.RunActionText(npc, p, "DEC P1 30");
        Assert.Equal(120, p.GetPVar(1));
    }
}
