using System;
using System.Collections.Generic;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>ObjNpc 脚本解释器测试：解析 / 条件 / 动作 / 变量 / SAY 替换。</summary>
public class NpcScriptTests
{
    private static TScriptPlayer NewPlayer(int level = 30, int gold = 1000)
    {
        var p = new TScriptPlayer
        {
            m_sCharName = "测试玩家",
            m_btJob = 0,     // 战士
            m_btGender = 0,  // 男
            m_boAdmin = false
        };
        p.m_wAbil.Level = (uint)level;
        p.Gold = gold;
        return p;
    }

    private static NpcScript Parse(string text) => NpcScriptEngine.ParseScript(text);

    [Fact]
    public void ParseScript_Sections()
    {
        var script = Parse(@"
; 注释行
[@main]
#IF
CHECKLEVEL 10
#SAY
你好，勇士！
#ACT
GIVE 金创药 2

[@shop]
#SAY
商店
");
        Assert.Contains("@main", script.Labels.Keys);
        Assert.Contains("@shop", script.Labels.Keys);
        var main = script.Labels["@main"];
        Assert.Single(main.Conditions);           // CHECKLEVEL 10
        Assert.Single(main.Actions);              // GIVE
        Assert.Single(main.SayLines);             // 你好，勇士！
        Assert.Equal("CHECKLEVEL", main.Conditions[0].Name);
        Assert.Equal("GIVE", main.Actions[0].Name);
    }

    [Fact]
    public void Condition_CheckLevel()
    {
        var eng = new NpcScriptEngine();
        var npc = new TNormNpc("比奇老兵");
        var p = NewPlayer(level: 35);
        Assert.True(eng.CheckConditionText(npc, p, "CHECKLEVEL 30"));
        Assert.False(eng.CheckConditionText(npc, p, "CHECKLEVEL 40"));
        // 区间形式 CHECKLEVEL 30 40
        Assert.True(eng.CheckConditionText(npc, p, "CHECKLEVEL 30 40"));
        Assert.False(eng.CheckConditionText(npc, p, "CHECKLEVEL 36 40"));
    }

    [Fact]
    public void Condition_CheckGold_And_Admin()
    {
        var eng = new NpcScriptEngine();
        var npc = new TNormNpc("掌柜");
        var p = NewPlayer(gold: 500);
        Assert.True(eng.CheckConditionText(npc, p, "CHECKGOLD 500"));
        Assert.False(eng.CheckConditionText(npc, p, "CHECKGOLD 501"));
        Assert.False(eng.CheckConditionText(npc, p, "ISADMIN"));
        p.m_boAdmin = true;
        Assert.True(eng.CheckConditionText(npc, p, "ISADMIN"));
    }

    [Fact]
    public void Condition_Gender_Job_Random()
    {
        var eng = new NpcScriptEngine();
        var npc = new TNormNpc("占卜师");
        var p = NewPlayer();
        Assert.True(eng.CheckConditionText(npc, p, "GENDER MAN"));
        Assert.False(eng.CheckConditionText(npc, p, "GENDER WOMAN"));
        Assert.True(eng.CheckConditionText(npc, p, "CHECKJOB WARRIOR"));
        Assert.False(eng.CheckConditionText(npc, p, "CHECKJOB WIZARD"));
        // RANDOM 1 恒真（Random(1)=0）
        Assert.True(eng.CheckConditionText(npc, p, "RANDOM 1"));
        // RANDOMEX A B：A/B 概率抽签，A>=B 恒真
        Assert.True(eng.CheckConditionText(npc, p, "RANDOMEX 2 2"));
    }

    [Fact]
    public void Action_Give_Take_Items()
    {
        var eng = new NpcScriptEngine();
        var npc = new TNormNpc("药店");
        var p = NewPlayer();
        Assert.True(eng.RunActionText(npc, p, "GIVE 金创药 3"));
        Assert.Equal(3, p.GetItemcount("金创药"));
        Assert.True(eng.RunActionText(npc, p, "TAKE 金创药 2"));
        Assert.Equal(1, p.GetItemcount("金创药"));
        // 数量不足 → 失败且不扣
        Assert.False(eng.RunActionText(npc, p, "TAKE 金创药 5"));
        Assert.Equal(1, p.GetItemcount("金创药"));
    }

    [Fact]
    public void Action_GameGold_And_ChangeLevel_Job()
    {
        var eng = new NpcScriptEngine();
        var npc = new TNormNpc("元宝使者");
        var p = NewPlayer();
        p.GameGold = 100;
        Assert.True(eng.RunActionText(npc, p, "GAMEGOLD + 50"));
        Assert.Equal(150, p.GameGold);
        Assert.True(eng.RunActionText(npc, p, "GAMEGOLD - 30"));
        Assert.Equal(120, p.GameGold);

        Assert.True(eng.RunActionText(npc, p, "CHANGELEVEL 40"));
        Assert.Equal(40u, p.m_wAbil.Level);
        Assert.True(eng.RunActionText(npc, p, "CHANGEJOB WIZARD"));
        Assert.Equal(1, p.m_btJob);
    }

    [Fact]
    public void Action_Vars_MOV_INC_Equal()
    {
        var eng = new NpcScriptEngine();
        var npc = new TNormNpc("变量师");
        var p = NewPlayer();
        Assert.True(eng.RunActionText(npc, p, "MOV P0 10"));
        Assert.True(eng.RunActionText(npc, p, "INC P0 5"));
        Assert.Equal(15, p.GetPVar(0));
        Assert.True(eng.RunActionText(npc, p, "DEC P0 3"));
        Assert.Equal(12, p.GetPVar(0));
        Assert.True(eng.CheckConditionText(npc, p, "EQUAL P0 12"));
        Assert.True(eng.CheckConditionText(npc, p, "LARGE P0 11"));
        Assert.True(eng.CheckConditionText(npc, p, "SMALL P0 13"));
        // MUL/DIV/PERCENT
        Assert.True(eng.RunActionText(npc, p, "MUL P0 2"));
        Assert.Equal(24, p.GetPVar(0));
        Assert.True(eng.RunActionText(npc, p, "DIV P0 4"));
        Assert.Equal(6, p.GetPVar(0));
        Assert.True(eng.RunActionText(npc, p, "PERCENT P0 50")); // 取 50% → 3
        Assert.Equal(3, p.GetPVar(0));
    }

    [Fact]
    public void Say_VariableSubstitution()
    {
        var eng = new NpcScriptEngine();
        var npc = new TNormNpc("长老");
        var p = NewPlayer(level: 42, gold: 777);
        p.GameGold = 9;
        string outText = eng.BuildSay(npc, p, new List<string>
        {
            "勇士 $USERNAME，你好。",
            "等级：$LEVEL 金币：$GOLD 元宝：$GAMEGOLD",
            "变量：<$STR(P0)>"
        });
        Assert.Contains("勇士 测试玩家", outText);
        Assert.Contains("等级：42 金币：777 元宝：9", outText);
        Assert.Contains("变量：0", outText);
    }

    [Fact]
    public void RunLabel_FullFlow()
    {
        var eng = new NpcScriptEngine();
        var npc = new TNormNpc("传送员");
        var p = NewPlayer(level: 35, gold: 2000);
        p.GameGold = 1500;
        var script = Parse(@"
[@main]
#IF
CHECKLEVEL 30
CHECKGOLD 1000
#ACT
GAMEGOLD - 1000
MAPMOVE 盟重 100 100
#SAY
已传送到盟重！

[@low]
#SAY
等级不足。
");
        var result = eng.RunLabel(script, "@main", npc, p);
        Assert.True(result.ConditionsPassed);
        Assert.True(result.ActionsAllOk);
        Assert.Equal(500, p.GameGold);
        Assert.Equal("盟重", p.RecallMapName);

        // 条件不满足 → 走 ELSE 分支
        var p2 = NewPlayer(level: 10, gold: 2000);
        var r2 = eng.RunLabel(script, "@main", npc, p2);
        Assert.False(r2.ConditionsPassed);
    }

    [Fact]
    public void RunLabel_ElseSay()
    {
        var eng = new NpcScriptEngine();
        var npc = new TNormNpc("守卫");
        var pLow = NewPlayer(level: 5);
        var script = Parse(@"
[@main]
#IF
CHECKLEVEL 30
#ACT
GIVE 奖励 1
#ELSEACT
SENDMSG 你等级太低
#ELSESAY
等你长大再来。
");
        var r = eng.RunLabel(script, "@main", npc, pLow);
        Assert.False(r.ConditionsPassed);
        Assert.Contains("等你长大再来。", r.SayText);
        Assert.False(pLow.HasItem("奖励"));
    }

    [Fact]
    public void Break_StopsActionChain()
    {
        var eng = new NpcScriptEngine();
        var npc = new TNormNpc("断路");
        var p = NewPlayer();
        Assert.True(eng.RunActionText(npc, p, "BREAK"));
        // BREAK 后续不执行：链式测试
        var script = Parse(@"
[@main]
#ACT
BREAK
GIVE 不得发放 1
");
        var r = eng.RunLabel(script, "@main", npc, p);
        Assert.False(p.HasItem("不得发放"));
    }

    [Fact]
    public void CmdCodeTables_Registered()
    {
        // 注册表与原 ConditionCmdArray/ActionCmdArray 对应
        Assert.Equal(1, NpcCmdCodes.nNC_CHECK);
        Assert.Equal(62, NpcCmdCodes.nNC_CHECKGAMEGOLD);
        Assert.Equal(3, NpcCmdCodes.nNA_GIVE);
        Assert.Equal(14, NpcCmdCodes.nNA_MAPMOVE);
        Assert.Equal(83, NpcCmdCodes.nNA_GAMEGOLD);
        // 名称→码表
        Assert.Equal(NpcCmdCodes.nNC_CHECKLEVEL, NpcScriptEngine.ConditionNameToCode("CHECKLEVEL"));
        Assert.Equal(NpcCmdCodes.nNA_MAPMOVE, NpcScriptEngine.ActionNameToCode("MAPMOVE"));
    }
}
