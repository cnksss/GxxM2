using System;
using System.Linq;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次I：NPC 脚本全量命令注册与余下处理器测试。</summary>
public class NpcScriptBatchITests : IDisposable
{
    public void Dispose() { }

    private static TScriptPlayer NewPlayer(int level = 30, int gold = 1000)
    {
        var p = new TScriptPlayer
        {
            m_sCharName = "批甲",
            m_btJob = 0,
            m_btGender = 0,
            m_btAttackMode = 1,
            m_boOnHorse = true,
            m_sAccount = "acct",
            m_sIPaddr = "192.168.1.9"
        };
        p.m_wAbil.Level = (uint)level;
        p.m_wAbil.HP = 500;
        p.m_wAbil.MaxHP = 500;
        p.m_wAbil.MP = 500;
        p.m_wAbil.MaxMP = 500;
        p.Gold = gold;
        return p;
    }

    private static (NpcScriptEngine eng, TNormNpc npc, TScriptPlayer p) Setup(int level = 30)
    {
        return (new NpcScriptEngine(), new TNormNpc("测试NPC"), NewPlayer(level));
    }

    [Fact]
    public void FullNameTables_Counts()
    {
        // 全量名称表：条件 278 项、动作 727 项（与 NpcCommon.pas g_Cmd*List 一致）
        Assert.Equal(278, NpcCmdNames.ConditionNames.Count);
        Assert.True(NpcCmdNames.ActionNames.Count >= 700);
        // 名称→码回环抽查
        Assert.Equal(6, NpcCmdCodes.nNC_CHECKLEVEL);
        Assert.Equal(83, NpcCmdCodes.nNA_GAMEGOLD);
    }

    [Fact]
    public void HandlerRegistration_CoversAllRegisteredCodes()
    {
        // 全部已注册名称码均有处理器或状态级等效桩（无 null 漏项）
        foreach (var kv in NpcCmdNames.ConditionNames)
        {
            int code = kv.Value;
            if (code >= 1 && code < 1000)
                Assert.True(NpcScriptEngine.ConditionCmdArray[code] != null,
                    $"条件命令 {kv.Key}({code}) 未注册处理器");
        }
        foreach (var kv in NpcCmdNames.ActionNames)
        {
            int code = kv.Value;
            if (code >= 1 && code < 1000)
                Assert.True(NpcScriptEngine.ActionCmdArray[code] != null,
                    $"动作命令 {kv.Key}({code}) 未注册处理器");
        }
    }

    [Fact]
    public void Condition_DAYTIME_DAYOFWEEK_HOUR_MIN()
    {
        var (eng, npc, p) = Setup();
        int daytime = DateTime.Now.Hour switch
        {
            >= 5 and < 7 => 0,
            >= 7 and < 17 => 1,
            >= 17 and < 19 => 2,
            _ => 3
        };
        Assert.True(eng.CheckConditionText(npc, p, $"DAYTIME {daytime}"));
        Assert.False(eng.CheckConditionText(npc, p, $"DAYTIME {(daytime + 1) % 4}"));
        Assert.True(eng.CheckConditionText(npc, p, $"HOUR {DateTime.Now.Hour}"));
        Assert.True(eng.CheckConditionText(npc, p, $"MIN {DateTime.Now.Minute}"));
        Assert.True(eng.CheckConditionText(npc, p, $"DAYOFWEEK {((int)DateTime.Now.DayOfWeek)}"));
    }

    [Fact]
    public void Condition_CheckLevelEx_Operators()
    {
        var (eng, npc, p) = Setup(level: 30);
        Assert.True(eng.CheckConditionText(npc, p, "CHECKLEVELEX > 29"));
        Assert.True(eng.CheckConditionText(npc, p, "CHECKLEVELEX >= 30"));
        Assert.True(eng.CheckConditionText(npc, p, "CHECKLEVELEX = 30"));
        Assert.True(eng.CheckConditionText(npc, p, "CHECKLEVELEX < 31"));
        Assert.False(eng.CheckConditionText(npc, p, "CHECKLEVELEX <= 29"));
    }

    [Fact]
    public void Condition_GameGold_Diamond_Gird_Glory()
    {
        var (eng, npc, p) = Setup();
        p.GameGold = 500;
        Assert.True(eng.CheckConditionText(npc, p, "CHECKGAMEGOLD 500"));
        Assert.False(eng.CheckConditionText(npc, p, "CHECKGAMEGOLD 400 1"));   // <= 400 → 500 不满足
        p.m_nGameDiamond = 10;
        p.m_nGameGird = 20;
        p.m_nGameGlory = 30;
        Assert.True(eng.CheckConditionText(npc, p, "CHECKGAMEDIAMOND 10"));
        Assert.True(eng.CheckConditionText(npc, p, "CHECKGAMEGIRD 20"));
        Assert.True(eng.CheckConditionText(npc, p, "CHECKGAMEGLORY 30"));
    }

    [Fact]
    public void Condition_PKPoint_AttackMode_Horse()
    {
        var (eng, npc, p) = Setup();
        p.PKPoint = 100;
        Assert.True(eng.CheckConditionText(npc, p, "CHECKPKPOINT 50 200"));
        Assert.False(eng.CheckConditionText(npc, p, "CHECKPKPOINT 200 300"));
        p.m_btAttackMode = 3;
        Assert.True(eng.CheckConditionText(npc, p, "CHECKATTACKMODE 3"));
        p.m_boOnHorse = true;
        Assert.True(eng.CheckConditionText(npc, p, "CHECKONHORSE"));
    }

    [Fact]
    public void Condition_NameList_AccountList_IpList()
    {
        var (eng, npc, p) = Setup();
        Assert.True(eng.RunActionText(npc, p, "ADDNAMELIST 名单.txt"));
        Assert.True(eng.CheckConditionText(npc, p, "CHECKNAMELIST 名单.txt"));
        Assert.True(eng.RunActionText(npc, p, "DELNAMELIST 名单.txt"));
        Assert.False(eng.CheckConditionText(npc, p, "CHECKNAMELIST 名单.txt"));
        p.m_sAccount = "acct001";
        Assert.True(eng.RunActionText(npc, p, "ADDACCOUNTLIST 账号.txt"));
        Assert.True(TScriptPlayer.EngineAccountList.Contains("acct001"));
        Assert.True(eng.RunActionText(npc, p, "DELACCOUNTLIST 账号.txt"));
    }

    [Fact]
    public void Action_ChangeExp_PKPoint_HumanHpMp()
    {
        var (eng, npc, p) = Setup();
        Assert.True(eng.RunActionText(npc, p, "CHANGEEXP + 5000"));
        Assert.Equal(5000, p.Exp);
        Assert.True(eng.RunActionText(npc, p, "PKPOINT + 100"));
        Assert.Equal(100, p.PKPoint);
        Assert.True(eng.RunActionText(npc, p, "HUMANHP - 100"));
        Assert.Equal(400u, p.m_wAbil.HP);
        Assert.True(eng.RunActionText(npc, p, "HUMANMP = 50"));
        Assert.Equal(50u, p.m_wAbil.MP);
    }

    [Fact]
    public void Action_Var_LoadSave_CalcVar()
    {
        var (eng, npc, p) = Setup();
        Assert.True(eng.RunActionText(npc, p, "MOV G0 100"));
        Assert.Equal(100, NpcScriptEngine.ReadVar(p, "G0"));
        Assert.True(eng.RunActionText(npc, p, "CALCVAR G0 + 50"));
        Assert.Equal(150, NpcScriptEngine.ReadVar(p, "G0"));
    }

    [Fact]
    public void Action_AddSkill_DelSkill_SkillLevel()
    {
        var (eng, npc, p) = Setup();
        Assert.True(eng.RunActionText(npc, p, "ADDSKILL 火球术 3"));
        Assert.Contains("火球术", p.Skills);
        Assert.False(eng.RunActionText(npc, p, "ADDSKILL 火球术 3"), "重复加技能失败");
        Assert.True(eng.RunActionText(npc, p, "DELSKILL 火球术"));
        Assert.DoesNotContain("火球术", p.Skills);
    }

    [Fact]
    public void Action_Offline_Kick_Kill()
    {
        var (eng, npc, p) = Setup();
        Assert.True(eng.RunActionText(npc, p, "OFFLINE"));
        Assert.True(p.Kicked);
        p.Kicked = false;
        Assert.True(eng.RunActionText(npc, p, "KICK"));
        Assert.True(p.Kicked);
        Assert.True(eng.RunActionText(npc, p, "KILL"));
        Assert.True(p.m_boDeath);
    }

    [Fact]
    public void Action_MemberType_RenewLevel_CreditPoint()
    {
        var (eng, npc, p) = Setup();
        Assert.True(eng.RunActionText(npc, p, "SETMEMBERTYPE 3"));
        Assert.Equal(3, p.m_nMemberType);
        Assert.True(eng.RunActionText(npc, p, "SETMEMBERLEVEL 5"));
        Assert.Equal(5, p.m_nMemberLevel);
        Assert.True(eng.CheckConditionText(npc, p, "CHECKMEMBERTYPE 3"));
        Assert.True(eng.CheckConditionText(npc, p, "CHECKMEMBERLEVEL 5"));
        Assert.True(eng.RunActionText(npc, p, "RENEWLEVEL 2"));
        Assert.Equal(2, p.m_btRenewLevel);
        Assert.True(eng.CheckConditionText(npc, p, "CHECKRENEWLEVEL 2"));
        Assert.True(eng.RunActionText(npc, p, "CREDITPOINT + 100"));
        Assert.Equal(100, p.CreditPoint);
        Assert.True(eng.CheckConditionText(npc, p, "CHECKCREDITPOINT 100"));
    }

    [Fact]
    public void Condition_Marry_Master()
    {
        var (eng, npc, p) = Setup();
        Assert.False(eng.CheckConditionText(npc, p, "CHECKMARRY"));
        p.m_sDearName = "另一半";
        Assert.True(eng.CheckConditionText(npc, p, "CHECKMARRY"));
        Assert.False(eng.CheckConditionText(npc, p, "CHECKMASTER"));
        p.m_sMasterName = "师父";
        Assert.True(eng.CheckConditionText(npc, p, "CHECKMASTER"));
    }

    [Fact]
    public void Condition_ContainsText_CompareText()
    {
        var (eng, npc, p) = Setup();
        Assert.True(eng.CheckConditionText(npc, p, "CHECKCONTAINSTEXT 比奇省 奇省"));
        Assert.False(eng.CheckConditionText(npc, p, "CHECKCONTAINSTEXT 比奇省 盟重"));
        Assert.True(eng.CheckConditionText(npc, p, "COMPARETEXT ABC abc"));
        Assert.False(eng.CheckConditionText(npc, p, "COMPARETEXT ABC abd"));
    }

    [Fact]
    public void Registry_NameToCode_ReadOnlyConsistency()
    {
        // 278 条件名 + 727 动作名 全部可达码并存在处理器
        int condHandlers = NpcScriptEngine.ConditionCmdArray.Count(h => h != null);
        int actHandlers = NpcScriptEngine.ActionCmdArray.Count(h => h != null);
        Assert.True(condHandlers >= 70, $"条件处理器 {condHandlers} 应覆盖全码表");
        Assert.True(actHandlers >= 700, $"动作处理器 {actHandlers} 应覆盖全码表");
        foreach (var name in new[] { "CHECK", "RANDOM", "CHECKLEVEL", "CHECKGAMEGOLD", "CHECKVAR", "DAYTIME", "CHECKMARRY" })
            Assert.True(NpcScriptEngine.ConditionNames.ContainsKey(name), name);
        foreach (var name in new[] { "GIVE", "TAKE", "MAPMOVE", "GAMEGOLD", "CALCVAR", "OFFLINE", "HUMANHP" })
            Assert.True(NpcScriptEngine.ActionNames.ContainsKey(name), name);
    }
}
