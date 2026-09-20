// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas
// 测试对象：GXX.M2Server.Npc 下的 1:1 移植（切片 1/2/3）
//   · ObjNpcUnitFuncs.LoadLevelScriptAction        原文 509-595
//   · ObjNpcUnitFuncs.LoadLevelScriptCondition     原文 596-681
//   · ObjNpcUnitFuncs.GetLevelBaseObjectCondition  原文 682-856
//   · ObjNpcUnitFuncs.GetLevelBaseObjectAction     原文 857-1106
//   · ObjNpcUnitFuncs.CheckStrIsVar                原文 10406-10509
//   · TNormNpc.GetVarValue ×4                      原文 4443-4493
//   · TNormNpc.SetVarValue                         原文 4495-4510
//   · TNormNpc.GetDynamicValue / SetDynamicValue   原文 4512-4633
//   · TNormNpc.GetValNameValue                     原文 5690-5877
//   · TNormNpc.GetLineVariableText                 原文 5981-6009
//   · TNormNpc.GetDynamicVarList                   原文 9877-9898
//   · TConditionList.Create                        原文 504-507
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Npc;
using Xunit;

// ⚠ 跨车道同名类型碰撞（第 5 次复发，已上报调度方）：
//   `GXX.M2Server.Engine.TNormNpc`（Engine/NpcScriptEngine.cs:10，会话 A 的 ObjNpc 前置最小模型）
//   与 `GXX.M2Server.Npc.TNormNpc`（本车道按 ObjNpc.pas:262 全量移植）同名不同命名空间。
//   只要一个文件同时 `using` 两个命名空间就触发 CS0104。
//   本车道**不改他人文件**，故在引用点用文件级别名消歧；
//   接入时应由调度方裁定：删除 Engine 侧最小模型，或把 ObjNpc 版并入 Engine。
using TNormNpc = GXX.M2Server.Npc.TNormNpc;

namespace GXX.M2Server.Tests;

public sealed class NpcObjNpcTests : IDisposable
{
    public NpcObjNpcTests() => NpcSeams.ResetDefaults();

    public void Dispose() => NpcSeams.ResetDefaults();

    private static TPlayObject NewPlayer(string name = "PLAYER1")
        => new() { m_sCharName = name, m_btRaceServer = Grobal2Const.RC_PLAYOBJECT };

    private static TCreature NewCreature(byte raceServer = Grobal2Const.RC_MONSTER)
        => new TCreatureProbe { m_btRaceServer = raceServer };

    /// <summary>测试用最小 TCreature 具体化（原文 TBaseObject 的接缝代表）。</summary>
    private sealed class TCreatureProbe : TCreature
    {
    }

    // -----------------------------------------------------------------------
    // LoadLevelScriptAction（509-595）
    // -----------------------------------------------------------------------

    [Fact]
    public void LoadLevelScriptAction_NoDot_ReturnsInputAndLeavesArraysEmpty()
    {
        var info = new TQuestActionInfo();
        string r = ObjNpcUnitFuncs.LoadLevelScriptAction(info, "CHECKITEM");
        Assert.Equal("CHECKITEM", r);
        Assert.Empty(info.ScriptCmd);
        Assert.Empty(info.ScriptList);
    }

    [Fact]
    public void LoadLevelScriptAction_EmptyInput_ReturnsEmpty()
    {
        var info = new TQuestActionInfo();
        string r = ObjNpcUnitFuncs.LoadLevelScriptAction(info, "");
        Assert.Equal("", r);
        Assert.Empty(info.ScriptCmd);
    }

    [Fact]
    public void LoadLevelScriptAction_SelfPrefix_DoesNotDuplicateSelf()
    {
        var info = new TQuestActionInfo();
        string r = ObjNpcUnitFuncs.LoadLevelScriptAction(info, "SELF.CHECKITEM");
        Assert.Equal("CHECKITEM", r);
        Assert.Equal(new[] { ObjNpcConst.CMD_RACE_0 }, info.ScriptCmd);
        Assert.Equal(new[] { "SELF" }, info.ScriptList);
    }

    [Fact]
    public void LoadLevelScriptAction_HeroPrefix_InsertsSelfAtFront()
    {
        var info = new TQuestActionInfo();
        string r = ObjNpcUnitFuncs.LoadLevelScriptAction(info, "HERO.CHECKITEM");
        Assert.Equal("CHECKITEM", r);
        Assert.Equal(new[] { ObjNpcConst.CMD_RACE_0, ObjNpcConst.CMD_RACE_1 }, info.ScriptCmd);
        Assert.Equal(new[] { "SELF", "HERO" }, info.ScriptList);
    }

    [Fact]
    public void LoadLevelScriptAction_LowercaseAndWhitespace_AreUppercasedAndTrimmed()
    {
        var info = new TQuestActionInfo();
        string r = ObjNpcUnitFuncs.LoadLevelScriptAction(info, " hero . checkitem ");
        Assert.Equal("CHECKITEM", r);
        Assert.Equal(new[] { "SELF", "HERO" }, info.ScriptList);
    }

    [Theory]
    [InlineData("M.PET.CHECKLEVEL", new[] { 0, 3, 9 })]
    [InlineData("O.CHECKLEVEL", new[] { 0, 2 })]
    [InlineData("P.CHECKLEVEL", new[] { 0, 4 })]
    [InlineData("L.CHECKLEVEL", new[] { 0, 6 })]
    [InlineData("BB.CHECKLEVEL", new[] { 0, 10 })]
    [InlineData("FS.CHECKLEVEL", new[] { 0, 11 })]
    [InlineData("BBR.CHECKLEVEL", new[] { 0, 12 })]
    [InlineData("UNKNOWN.CHECKLEVEL", new[] { 0, 5 })]
    public void LoadLevelScriptAction_TargetPrefixTable(string cmd, int[] expected)
    {
        var info = new TQuestActionInfo();
        ObjNpcUnitFuncs.LoadLevelScriptAction(info, cmd);
        Assert.Equal(expected, info.ScriptCmd);
    }

    [Fact]
    public void LoadLevelScriptAction_TrailingDot_IsGuardedAndLeavesArraysEmpty()
    {
        // 原文 518：Action 侧独有 `(sCmd[Length(sCmd)] <> '.')` 尾点保护。
        var info = new TQuestActionInfo();
        string r = ObjNpcUnitFuncs.LoadLevelScriptAction(info, "A.");
        Assert.Equal("A.", r);
        Assert.Empty(info.ScriptCmd);
        Assert.Empty(info.ScriptList);
    }

    [Fact]
    public void LoadLevelScriptAction_HeroExtOff_HmHlStayZeroWhileOtherPrefixesStillMap()
    {
        // 原文 558-568：`HM` / `HL` 只在 g_nKey_HeroExt = 1 时赋值；
        // 否则 SetLength 的零初始化保持 0 —— 而 0 **恰好等于 CMD_RACE_0（self）**。
        NpcSeams.g_nKey_HeroExt = 0;
        var info = new TQuestActionInfo();
        ObjNpcUnitFuncs.LoadLevelScriptAction(info, "HM.HL.CHECKLEVEL");
        Assert.Equal(new[] { ObjNpcConst.CMD_RACE_0, ObjNpcConst.CMD_RACE_0, ObjNpcConst.CMD_RACE_0 }, info.ScriptCmd);
        Assert.Equal(new[] { "SELF", "HM", "HL" }, info.ScriptList);
    }

    [Fact]
    public void LoadLevelScriptAction_HeroExtOn_HmHlMapToSevenAndEight()
    {
        NpcSeams.g_nKey_HeroExt = 1;
        var info = new TQuestActionInfo();
        ObjNpcUnitFuncs.LoadLevelScriptAction(info, "HM.HL.CHECKLEVEL");
        Assert.Equal(new[] { ObjNpcConst.CMD_RACE_0, ObjNpcConst.CMD_RACE_7, ObjNpcConst.CMD_RACE_8 }, info.ScriptCmd);
    }

    // -----------------------------------------------------------------------
    // LoadLevelScriptCondition（596-681）—— 与 Action 的三处真实差异
    // -----------------------------------------------------------------------

    [Fact]
    public void LoadLevelScriptCondition_Diff1_TrailingDot_ThrowsWhileActionDoesNot()
    {
        // 差异 1（原文 605 无尾点保护）：`"A."` → ExtractStrings 得 ['A'] → Delete(0) 后取 Strings[0] 越界。
        // Delphi 抛 EStringListError；托管侧 List<T> 抛 ArgumentOutOfRangeException。
        var con = new TQuestConditionInfo();
        Assert.Throws<ArgumentOutOfRangeException>(() => ObjNpcUnitFuncs.LoadLevelScriptCondition(con, "A."));

        var act = new TQuestActionInfo();
        string r = ObjNpcUnitFuncs.LoadLevelScriptAction(act, "A.");
        Assert.Equal("A.", r);
        Assert.Empty(act.ScriptCmd);
    }

    [Fact]
    public void LoadLevelScriptCondition_Diff1_SingleDot_Throws()
    {
        // `"."` → ExtractStrings 全为空段 → 列表为空 → Strings[-1] 越界。
        var con = new TQuestConditionInfo();
        Assert.Throws<ArgumentOutOfRangeException>(() => ObjNpcUnitFuncs.LoadLevelScriptCondition(con, "."));
    }

    [Theory]
    [InlineData("BB.CHECKLEVEL", 5)]   // 原文 662-671 把 BB 分支注释掉 → 落 else → CMD_RACE_5
    [InlineData("FS.CHECKLEVEL", 5)]   // 同上
    [InlineData("BBR.CHECKLEVEL", 5)]  // 原文根本没有 BBR 分支 → CMD_RACE_5
    [InlineData("PET.CHECKLEVEL", 9)]  // 未注释的分支仍生效
    [InlineData("HERO.CHECKLEVEL", 1)]
    public void LoadLevelScriptCondition_Diff2And3_BbFsBbrDegradeToVarName(string cmd, int expectedSecond)
    {
        var info = new TQuestConditionInfo();
        ObjNpcUnitFuncs.LoadLevelScriptCondition(info, cmd);
        Assert.Equal(new[] { ObjNpcConst.CMD_RACE_0, expectedSecond }, info.ScriptCmd);
    }

    [Fact]
    public void LoadLevelScriptCondition_MatchesActionOnCommonPrefixes()
    {
        foreach (string cmd in new[] { "SELF.CHECKITEM", "HERO.CHECKITEM", "M.PET.CHECKLEVEL", "O.CHECKLEVEL" })
        {
            var act = new TQuestActionInfo();
            var con = new TQuestConditionInfo();
            string ra = ObjNpcUnitFuncs.LoadLevelScriptAction(act, cmd);
            string rc = ObjNpcUnitFuncs.LoadLevelScriptCondition(con, cmd);
            Assert.Equal(ra, rc);
            Assert.Equal(act.ScriptCmd, con.ScriptCmd);
            Assert.Equal(act.ScriptList, con.ScriptList);
        }
    }

    // -----------------------------------------------------------------------
    // GetLevelBaseObjectCondition（682-855）/ GetLevelBaseObjectAction（857-1105）
    // -----------------------------------------------------------------------

    [Fact]
    public void GetLevelBaseObjectAction_NullQuestActionInfo_ReturnsNull()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        Assert.Null(ObjNpcUnitFuncs.GetLevelBaseObjectAction(npc, player, null));
    }

    [Fact]
    public void GetLevelBaseObject_EmptyScriptCmd_ReturnsPlayObject()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        Assert.Same(player, ObjNpcUnitFuncs.GetLevelBaseObjectCondition(npc, player, new TQuestConditionInfo()));
        Assert.Same(player, ObjNpcUnitFuncs.GetLevelBaseObjectAction(npc, player, new TQuestActionInfo()));
    }

    [Fact]
    public void GetLevelBaseObject_Self_ReturnsPlayObject()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        var con = new TQuestConditionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_0 } };
        var act = new TQuestActionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_0 } };
        Assert.Same(player, ObjNpcUnitFuncs.GetLevelBaseObjectCondition(npc, player, con));
        Assert.Same(player, ObjNpcUnitFuncs.GetLevelBaseObjectAction(npc, player, act));
    }

    [Fact]
    public void GetLevelBaseObject_UnknownCmdCode_BreaksAndKeepsCurrentObject()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        var con = new TQuestConditionInfo { ScriptCmd = new[] { 99 } };
        var act = new TQuestActionInfo { ScriptCmd = new[] { 99 } };
        Assert.Same(player, ObjNpcUnitFuncs.GetLevelBaseObjectCondition(npc, player, con));
        Assert.Same(player, ObjNpcUnitFuncs.GetLevelBaseObjectAction(npc, player, act));
    }

    [Fact]
    public void GetLevelBaseObject_Hero_ReturnsHeroWhenPlayerRaceMatches()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        var hero = NewCreature();
        player.m_MyHero = hero;   // ★ 已去接缝：直读 TPlayObject.m_MyHero（PlayerSurface.Hero.cs:31）

        var con = new TQuestConditionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_1 } };
        var act = new TQuestActionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_1 } };
        Assert.Same(hero, ObjNpcUnitFuncs.GetLevelBaseObjectCondition(npc, player, con));
        Assert.Same(hero, ObjNpcUnitFuncs.GetLevelBaseObjectAction(npc, player, act));
    }

    [Fact]
    public void GetLevelBaseObject_Hero_NoHero_ReturnsNull()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        player.m_MyHero = null;
        var con = new TQuestConditionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_1 } };
        Assert.Null(ObjNpcUnitFuncs.GetLevelBaseObjectCondition(npc, player, con));
    }

    [Fact]
    public void GetLevelBaseObject_Hero_NonPlayerRace_ReturnsNull()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        var monster = NewCreature(Grobal2Const.RC_MONSTER);
        var con = new TQuestConditionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_1 } };
        // BaseObject 起手即 PlayObject；用 CMD_RACE_0 + CMD_RACE_1 无法造非玩家起手，
        // 故用 CMD_RACE_2(master) 先把 BaseObject 换成怪物，再要 hero。
        player.m_Master = monster;
        con.ScriptCmd = new[] { ObjNpcConst.CMD_RACE_2, ObjNpcConst.CMD_RACE_1 };
        Assert.Null(ObjNpcUnitFuncs.GetLevelBaseObjectCondition(npc, player, con));
    }

    [Fact]
    public void GetLevelBaseObject_Master_ReturnsMaster()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        var master = NewCreature();
        player.m_Master = master;
        var act = new TQuestActionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_2 } };
        Assert.Same(master, ObjNpcUnitFuncs.GetLevelBaseObjectAction(npc, player, act));
    }

    [Fact]
    public void GetLevelBaseObject_CurrTargetAndLastHiterAndPoseCreate()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        var target = NewCreature();
        var hiter = NewCreature();
        var pose = NewCreature();
        // ★ 已去接缝：直读 TCreature.m_CurrTarget / m_LastHiter / GetPoseCreate()
        player.m_CurrTarget = target;
        player.m_LastHiter = hiter;
        // `GetPoseCreate()` 原文 = `GetFrontPosition` + `m_PEnvir.GetMovingObject(...)`，
        // 托管侧经 Engine 自己的 `PlayerSurfaceBaseSeams.GetMovingObject` 接缝返回（Engine 层已有）。
        player.m_PEnvir = new TEnvirnoment();
        PlayerSurfaceBaseSeams.GetMovingObject = (_, _, _, _, _) => pose;

        Assert.Same(target, ObjNpcUnitFuncs.GetLevelBaseObjectCondition(npc, player,
            new TQuestConditionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_3 } }));
        Assert.Same(hiter, ObjNpcUnitFuncs.GetLevelBaseObjectCondition(npc, player,
            new TQuestConditionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_6 } }));
        Assert.Same(pose, ObjNpcUnitFuncs.GetLevelBaseObjectCondition(npc, player,
            new TQuestConditionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_4 } }));
    }

    [Fact]
    public void GetLevelBaseObject_HeroExtOff_KeepsBaseObjectUntouched()
    {
        // 原文 801-834：CMD_RACE_7/8 的整个赋值块在 `if g_nKey_HeroExt = 1` 之内
        // → 关闭时**既不赋值也不 Break**，BaseObject 保持 PlayObject（与 CMD_RACE_1 不同！）
        NpcSeams.g_nKey_HeroExt = 0;
        var npc = new TNormNpc();
        var player = NewPlayer();
        player.m_MyHero = null;

        Assert.Same(player, ObjNpcUnitFuncs.GetLevelBaseObjectCondition(npc, player,
            new TQuestConditionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_7 } }));
        Assert.Same(player, ObjNpcUnitFuncs.GetLevelBaseObjectAction(npc, player,
            new TQuestActionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_8 } }));
    }

    [Fact]
    public void GetLevelBaseObject_HeroExtOn_Race7GoesThroughHeroCurrTarget()
    {
        NpcSeams.g_nKey_HeroExt = 1;
        var npc = new TNormNpc();
        var player = NewPlayer();
        var hero = NewCreature();
        var heroTarget = NewCreature();
        player.m_MyHero = hero;
        hero.m_CurrTarget = heroTarget;

        Assert.Same(heroTarget, ObjNpcUnitFuncs.GetLevelBaseObjectCondition(npc, player,
            new TQuestConditionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_7 } }));
    }

    [Fact]
    public void GetLevelBaseObject_Race9_GamePet()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        var pet = NewPlayer("PET1");
        player.m_MyGamePet = pet;
        Assert.Same(pet, ObjNpcUnitFuncs.GetLevelBaseObjectCondition(npc, player,
            new TQuestConditionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_9 } }));
    }

    [Fact]
    public void GetLevelBaseObject_Race9_NoPet_ReturnsNull()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        player.m_MyGamePet = null;
        Assert.Null(ObjNpcUnitFuncs.GetLevelBaseObjectCondition(npc, player,
            new TQuestConditionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_9 } }));
    }

    [Fact]
    public void GetLevelBaseObject_Diff_Race5NullPlayer_BreaksInActionButNotInCondition()
    {
        // 关键差异（原文 779 vs 959-961）：Condition 在 GetPlayObject 返回 nil 时**不 Break**，
        // 于是后续级别还能把 BaseObject 重新赋回 PlayObject（CMD_RACE_0）；
        // Action 立即 Break，结果保持 nil。
        var npc = new TNormNpc();
        var player = NewPlayer();
        NpcSeams.GetPlayObject = _ => null;

        var con = new TQuestConditionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_5, ObjNpcConst.CMD_RACE_0 } };
        con.ScriptList = new[] { "NOPE", "X" };
        var act = new TQuestActionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_5, ObjNpcConst.CMD_RACE_0 } };
        act.ScriptList = new[] { "NOPE", "X" };

        Assert.Same(player, ObjNpcUnitFuncs.GetLevelBaseObjectCondition(npc, player, con));
        Assert.Null(ObjNpcUnitFuncs.GetLevelBaseObjectAction(npc, player, act));
    }

    [Fact]
    public void GetLevelBaseObject_Race5_ResolvesVariableThenLooksUpPlayer()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        var other = NewPlayer("OTHER");
        string asked = null;
        NpcSeams.GetPlayObject = n => { asked = n; return other; };

        var con = new TQuestConditionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_5 } };
        con.ScriptList = new[] { "OTHER" };
        Assert.Same(other, ObjNpcUnitFuncs.GetLevelBaseObjectCondition(npc, player, con));
        Assert.Equal("OTHER", asked);
    }

    [Fact]
    public void GetLevelBaseObject_Race10_ReturnsFirstLivingSlave()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        var dead = NewCreature();
        dead.m_boDeath = true;
        var alive = NewCreature();
        player.m_SlaveList.Add(dead);
        player.m_SlaveList.Add(alive);

        var act = new TQuestActionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_10 } };
        Assert.Same(alive, ObjNpcUnitFuncs.GetLevelBaseObjectAction(npc, player, act));

        // Condition 侧**没有** CMD_RACE_10 分支 → else Break → 保持 PlayObject
        var con = new TQuestConditionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_10 } };
        Assert.Same(player, ObjNpcUnitFuncs.GetLevelBaseObjectCondition(npc, player, con));
    }

    [Fact]
    public void GetLevelBaseObject_Race11_RequiresCopyMonFlag()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        var slave = NewCreature();
        player.m_SlaveList.Add(slave);

        NpcSeams.IsCopyMon = _ => false;
        Assert.Null(ObjNpcUnitFuncs.GetLevelBaseObjectAction(npc, player,
            new TQuestActionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_11 } }));

        NpcSeams.IsCopyMon = _ => true;
        Assert.Same(slave, ObjNpcUnitFuncs.GetLevelBaseObjectAction(npc, player,
            new TQuestActionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_11 } }));
    }

    [Fact]
    public void GetLevelBaseObject_Race12_RandomPickup_LivingFirstTry()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        player.m_SlaveList.Add(NewCreature());
        player.m_SlaveList.Add(NewCreature());
        NpcSeams.Random = _ => 1;

        Assert.Same(player.m_SlaveList[1], ObjNpcUnitFuncs.GetLevelBaseObjectAction(npc, player,
            new TQuestActionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_12 } }));
    }

    [Fact]
    public void GetLevelBaseObject_Race12_AllDead_GivesUpAfter20TriesWithoutBreak()
    {
        // 原文 1082-1091：`Count < 20` 上限；全失败时 BaseObject 保持 nil **且不 Break**。
        var npc = new TNormNpc();
        var player = NewPlayer();
        var dead = NewCreature();
        dead.m_boDeath = true;
        player.m_SlaveList.Add(dead);

        int calls = 0;
        NpcSeams.Random = n => { calls++; return 0; };

        var act = new TQuestActionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_12 } };
        Assert.Null(ObjNpcUnitFuncs.GetLevelBaseObjectAction(npc, player, act));
        Assert.Equal(20, calls);
    }

    [Fact]
    public void GetLevelBaseObject_Race12_NoBreak_NextLevelCanRescue()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        player.m_SlaveList.Add(new TCreatureProbe { m_boDeath = true });
        NpcSeams.Random = _ => 0;

        // [12, 0]：12 全失败（不 Break）→ 0 把 BaseObject 赋回 PlayObject
        var act = new TQuestActionInfo { ScriptCmd = new[] { ObjNpcConst.CMD_RACE_12, ObjNpcConst.CMD_RACE_0 } };
        Assert.Same(player, ObjNpcUnitFuncs.GetLevelBaseObjectAction(npc, player, act));
    }

    // -----------------------------------------------------------------------
    // CheckStrIsVar（10406-10509）
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData("", false)]
    [InlineData("abc", false)]
    [InlineData("<$HUMAN(AA)>", true)]
    [InlineData("<$GUILD(AA)>", true)]
    [InlineData("<$GLOBAL(AA)>", true)]
    [InlineData("<$STR(P1)>", true)]
    [InlineData("<$STR(L$A)>", true)]
    [InlineData("<$STR(S$A)>", true)]
    [InlineData("<$STR(N$A)>", true)]
    [InlineData("<$STR(Q1)>", false)]
    // 长度必须 > 3 才进入（"<$A>" 长度 4 > 3，但仍要求含 '<' '$' '>'）
    [InlineData("<$>", false)]
    public void CheckStrIsVar_Table(string text, bool expected)
    {
        Assert.Equal(expected, ObjNpcUnitFuncs.CheckStrIsVar(text));
    }

    [Fact]
    public void CheckStrIsVar_ShortString_ReturnsFalse()
    {
        Assert.False(ObjNpcUnitFuncs.CheckStrIsVar("<$STR(P1)"));
    }

    // -----------------------------------------------------------------------
    // GetValNameValue（5690-5877）
    // -----------------------------------------------------------------------

    [Fact]
    public void GetValNameValue_EmptyVar_LeavesOutParamsUntouched()
    {
        // 原文 5697-5698：`if sVar = '' then Exit` —— 出参保持调用方原值（ref 而非 out）。
        var npc = new TNormNpc();
        var player = NewPlayer();
        string sValue = "KEEP";
        int nValue = 777;
        Assert.False(npc.GetValNameValue(player, "", ref sValue, ref nValue));
        Assert.Equal("KEEP", sValue);
        Assert.Equal(777, nValue);
    }

    [Fact]
    public void GetValNameValue_UnmatchedPrefix_ExitsWithSValueEqualsInputAndNValueZero()
    {
        // 原文 5699-5702：`sValue := sVar; nValue := 0;` 在判定之前无条件执行。
        var npc = new TNormNpc();
        var player = NewPlayer();
        string sValue = "KEEP";
        int nValue = 777;
        Assert.False(npc.GetValNameValue(player, "ZZZ", ref sValue, ref nValue));
        Assert.Equal("ZZZ", sValue);
        Assert.Equal(0, nValue);
    }

    [Fact]
    public void GetValNameValue_DVariable_ReadsPlayerDyVal()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        player.m_DyVal[7] = 1234;
        string sValue = "";
        int nValue = 0;
        Assert.True(npc.GetValNameValue(player, "D7", ref sValue, ref nValue));
        Assert.Equal(1234, nValue);
        Assert.Equal("1234", sValue);
    }

    [Fact]
    public void GetValNameValue_MNUVariables_ReadPlayerArrays()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        player.m_nMval[1] = 11;
        player.m_nInteger[2] = 22;
        player.m_UVal[3] = 33;
        player.m_JVal[4] = 44;
        player.m_ZVal[5] = "55";

        string sValue = "";
        int nValue = 0;
        Assert.True(npc.GetValNameValue(player, "M1", ref sValue, ref nValue));
        Assert.Equal(11, nValue);
        Assert.True(npc.GetValNameValue(player, "N2", ref sValue, ref nValue));
        Assert.Equal(22, nValue);
        Assert.True(npc.GetValNameValue(player, "U3", ref sValue, ref nValue));
        Assert.Equal(33, nValue);
        Assert.True(npc.GetValNameValue(player, "J4", ref sValue, ref nValue));
        Assert.Equal(44, nValue);
        Assert.True(npc.GetValNameValue(player, "Z5", ref sValue, ref nValue));
        Assert.Equal(55, nValue);
        Assert.Equal("55", sValue);
    }

    [Fact]
    public void GetValNameValue_ZValNullElement_TreatedAsEmptyString()
    {
        // ⚠ Engine.TPlayObject.m_ZVal 是 string[]，默认元素为 null；原文是 ShortString 默认 ''。
        var npc = new TNormNpc();
        var player = NewPlayer();
        string sValue = "";
        int nValue = 0;
        Assert.True(npc.GetValNameValue(player, "Z0", ref sValue, ref nValue));
        Assert.Equal("", sValue);
        Assert.Equal(0, nValue);
    }

    [Fact]
    public void GetValNameValue_PVariable_UsesSeam()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        player.m_nVal[3] = 103;   // 原文 `PlayObject.m_nVal[n01]`（n01 = 3）
        string sValue = "";
        int nValue = 0;
        Assert.True(npc.GetValNameValue(player, "P3", ref sValue, ref nValue));
        Assert.Equal(103, nValue);
        Assert.Equal("103", sValue);
    }

    [Fact]
    public void GetValNameValue_GlobalArrays_UseSeams()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        NpcSeams.GetGlobaDyMval = i => 4000 + i;
        NpcSeams.GetGlobalVal = i => 5000 + i;
        NpcSeams.GetGlobalAVal = i => (6000 + i).ToString();

        string sValue = "";
        int nValue = 0;
        Assert.True(npc.GetValNameValue(player, "I5", ref sValue, ref nValue));
        Assert.Equal(4005, nValue);
        Assert.True(npc.GetValNameValue(player, "G6", ref sValue, ref nValue));
        Assert.Equal(5006, nValue);
        Assert.True(npc.GetValNameValue(player, "A7", ref sValue, ref nValue));
        Assert.Equal(6007, nValue);
        Assert.Equal("6007", sValue);
    }

    [Fact]
    public void GetValNameValue_StringListAndIntegerList()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        player.m_StringList.Add("S$HELLO", "88");
        player.m_IntegerList.Add("N$HELLO", 99);

        string sValue = "";
        int nValue = 0;
        Assert.True(npc.GetValNameValue(player, "S$hello", ref sValue, ref nValue));
        Assert.Equal("88", sValue);
        Assert.Equal(88, nValue);

        Assert.True(npc.GetValNameValue(player, "N$hello", ref sValue, ref nValue));
        Assert.Equal(99, nValue);
        Assert.Equal("99", sValue);
    }

    [Fact]
    public void GetValNameValue_StringListMiss_YieldsEmptyAndZero()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        string sValue = "";
        int nValue = 0;
        Assert.True(npc.GetValNameValue(player, "S$NOPE", ref sValue, ref nValue));
        Assert.Equal("", sValue);
        Assert.Equal(0, nValue);
    }

    [Fact]
    public void GetValNameValue_LVariable_UsesArraySeam()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        player.m_ArrayList.Add("L$ARR", "66");
        string sValue = "";
        int nValue = 0;
        Assert.True(npc.GetValNameValue(player, "L$ARR", ref sValue, ref nValue));
        Assert.Equal("66", sValue);
        Assert.Equal(66, nValue);
    }

    [Fact]
    public void GetValNameValue_LVariableWithBrackets_StripsIndexBeforeLookup()
    {
        // 原文 5831-5834：带 `[i]` 时先用 GetValidStr3 剥掉下标，再按**剩余名字**查表。
        var npc = new TNormNpc();
        var player = NewPlayer();
        player.m_ArrayList.Add("L$ARR", "7");
        string sValue = "";
        int nValue = 0;
        Assert.True(npc.GetValNameValue(player, "L$ARR[3]", ref sValue, ref nValue));
        Assert.Equal(7, nValue);
    }

    [Fact]
    public void GetValNameValue_BoxItemPrefix_UsesSeam()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        NpcSeams.GetBoxItemValue = (sVar, p) => (true, "321");
        string sValue = "";
        int nValue = 0;
        Assert.True(npc.GetValNameValue(player, "<$BOXITEM[0]>", ref sValue, ref nValue));
        Assert.Equal("321", sValue);
        Assert.Equal(321, nValue);
    }

    [Fact]
    public void GetValNameValue_BoxItemPrefix_IsCaseInsensitive()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        bool called = false;
        NpcSeams.GetBoxItemValue = (sVar, p) => { called = true; return (false, ""); };
        string sValue = "";
        int nValue = 0;
        Assert.False(npc.GetValNameValue(player, "<$boxitem[0]>", ref sValue, ref nValue));
        Assert.True(called);
    }

    [Fact]
    public void GetValNameValue_StrWrapper_UnwrapsBeforeLookup()
    {
        // 原文 5703-5708：`<$STR(...)>` 先取括号内容作为 sName。
        var npc = new TNormNpc();
        var player = NewPlayer();
        player.m_DyVal[3] = 42;
        string sValue = "";
        int nValue = 0;
        Assert.True(npc.GetValNameValue(player, "<$STR(D3)>", ref sValue, ref nValue));
        Assert.Equal(42, nValue);
        Assert.Equal("42", sValue);
    }

    [Fact]
    public void GetValNameValue_StrWrapper_EmptyInner_ReturnsFalse()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        string sValue = "KEEP";
        int nValue = 1;
        Assert.False(npc.GetValNameValue(player, "<$STR()>", ref sValue, ref nValue));
    }

    [Fact]
    public void GetValNameValue_LPlainNumber_ReturnsFalseButStillWritesOutParams()
    {
        // 原文 GetValNameNo 对 `L123` 返回 10123（>=0），但 GetValNameValue 只列到 9999
        // → 无区间命中，Result 保持 False 且 sValue 已是 sVar、nValue 已是 0。
        var npc = new TNormNpc();
        var player = NewPlayer();
        string sValue = "KEEP";
        int nValue = 5;
        Assert.False(npc.GetValNameValue(player, "L123", ref sValue, ref nValue));
        Assert.Equal("L123", sValue);
        Assert.Equal(0, nValue);
    }

    // -----------------------------------------------------------------------
    // GetVarValue 四个重载（4443-4493）/ SetVarValue（4495-4510）
    // -----------------------------------------------------------------------

    [Fact]
    public void GetVarValue_IntOverload_NonVarTextZeroesValue()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        int nValue = 55;
        npc.GetVarValue(player, "PLAIN", ref nValue);
        Assert.Equal(0, nValue);
    }

    [Fact]
    public void GetVarValue_IntOverload_VarTextKeepsDefaultOnParseFailure()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        int nValue = 55;
        // 接缝 GetVariableText 默认返回 (false, sMsg, false) → sValue 仍是原文串 → 解析失败 → 保留 55
        npc.GetVarValue(player, "<$XYZ>", ref nValue);
        Assert.Equal(55, nValue);
    }

    [Fact]
    public void GetVarValue_StringOverload_NonVarTextPassesThrough()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        string sValue = "";
        npc.GetVarValue(player, "PLAIN", ref sValue);
        Assert.Equal("PLAIN", sValue);
    }

    [Fact]
    public void GetVarValue_FiveArgOverload_AlwaysResetsIsBreakParseVar()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        string sValue = "";
        int nValue = 9;
        bool brk = true;
        npc.GetVarValue(player, "PLAIN", ref sValue, ref nValue, ref brk);
        Assert.False(brk);
        Assert.Equal("PLAIN", sValue);
        Assert.Equal(0, nValue);   // 原文 4478：StrToInt64Def(sValue, **0**)
    }

    [Fact]
    public void GetVarValue_FourArgOverload_PriorityNameThenDynamicThenLine()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        player.m_DyVal[1] = 66;
        string sVar = "", sValue = "";
        int nValue = 0;
        npc.GetVarValue(player, "D1", ref sVar, ref sValue, ref nValue);
        Assert.Equal("D1", sVar);
        Assert.Equal("66", sValue);
        Assert.Equal(66, nValue);
    }

    [Fact]
    public void GetVarValue_FourArgOverload_FallsBackToLineVariableText()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        NpcSeams.GetVariableText = (n, p, sMsg, sVariable, nPos) => (true, "REPLACED", false);
        string sVar = "", sValue = "";
        int nValue = 0;
        npc.GetVarValue(player, "<$ANY>", ref sVar, ref sValue, ref nValue);
        Assert.Equal("REPLACED", sValue);
        Assert.Equal(0, nValue);
    }

    [Fact]
    public void GetVarValue_FourArgOverload_DynamicVARIsResolvedBeforeLineParse()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        player.m_sCharName = "HERO1";
        var list = new List<TDynamicVar>
        {
            new() { sName = "MYVAR", VarType = TVarType.vInteger, nInternet = 123 }
        };
        NpcSeams.GetPlayerDynamicVarList = _ => list;

        string sVar = "", sValue = "";
        int nValue = 0;
        npc.GetVarValue(player, "<$HUMAN(MYVAR)>", ref sVar, ref sValue, ref nValue);
        Assert.Equal("123", sValue);
        Assert.Equal(123, nValue);
        // 命中的动态变量必须**未被**改写
        Assert.Equal(123, list[0].nInternet);
    }

    [Fact]
    public void SetVarValue_EmptyData_ReturnsFalse()
    {
        var npc = new TNormNpc();
        Assert.False(npc.SetVarValue(NewPlayer(), "", "v", 1));
    }

    [Fact]
    public void SetVarValue_FallsThroughToDynamicValue()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        var list = new List<TDynamicVar>
        {
            new() { sName = "V1", VarType = TVarType.vString, sString = "old" }
        };
        NpcSeams.GetGlobalDynamicVarList = () => list;
        NpcSeams.SetValNameValue = (n, p, s, v, i) => false;

        Assert.True(npc.SetVarValue(player, "<$GLOBAL(V1)>", "new", 0));
        Assert.Equal("new", list[0].sString);
    }

    [Fact]
    public void SetVarValue_NameValueSeamShortCircuits()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        bool dynamicCalled = false;
        NpcSeams.SetValNameValue = (n, p, s, v, i) => true;
        NpcSeams.GetGlobalDynamicVarList = () => { dynamicCalled = true; return new List<TDynamicVar>(); };
        Assert.True(npc.SetVarValue(player, "<$GLOBAL(V1)>", "new", 0));
        Assert.False(dynamicCalled);
    }

    // -----------------------------------------------------------------------
    // GetDynamicValue（4512-4574）/ SetDynamicValue（4576-4633）
    // -----------------------------------------------------------------------

    [Fact]
    public void GetDynamicValue_NonWrappedVar_WritesOutParamsAndReturnsFalse()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        string sValue = "OLD";
        int nValue = 5;
        Assert.False(npc.GetDynamicValue(player, "NOTWRAPPED", ref sValue, ref nValue));
        Assert.Equal("NOTWRAPPED", sValue);
        Assert.Equal(0, nValue);
    }

    [Fact]
    public void GetDynamicValue_TooShort_ReturnsFalse()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        string sValue = "";
        int nValue = 0;
        // 原文 4526：要求 Length(sData) > 2
        Assert.False(npc.GetDynamicValue(player, "<$", ref sValue, ref nValue));
        Assert.False(npc.GetDynamicValue(player, "<$>", ref sValue, ref nValue));
    }

    [Fact]
    public void GetDynamicValue_UnknownScope_ReturnsFalse()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        string sValue = "";
        int nValue = 0;
        Assert.False(npc.GetDynamicValue(player, "<$NOSCOPE(A)>", ref sValue, ref nValue));
    }

    [Fact]
    public void GetDynamicValue_IntegerVar_ReturnsValue()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        NpcSeams.GetPlayerDynamicVarList = _ => new List<TDynamicVar>
        {
            new() { sName = "AA", VarType = TVarType.vInteger, nInternet = 7 }
        };
        string sValue = "";
        int nValue = 0;
        Assert.True(npc.GetDynamicValue(player, "<$HUMAN(AA)>", ref sValue, ref nValue));
        Assert.Equal("7", sValue);
        Assert.Equal(7, nValue);
    }

    [Fact]
    public void GetDynamicValue_StringVar_ParsesNumericPrefix()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        NpcSeams.GetPlayerDynamicVarList = _ => new List<TDynamicVar>
        {
            new() { sName = "AA", VarType = TVarType.vString, sString = "123" }
        };
        string sValue = "";
        int nValue = 0;
        Assert.True(npc.GetDynamicValue(player, "<$HUMAN(AA)>", ref sValue, ref nValue));
        Assert.Equal("123", sValue);
        Assert.Equal(123, nValue);
    }

    [Fact]
    public void GetDynamicValue_VNoneType_MatchesButYieldsFalseAndStopsSearch()
    {
        // 原文 4556-4570：case 只列 vInteger/vString；vNone 无分支 → Result 仍 False，
        // 但 4570 的 Break 仍执行 → 后面的同名项不再被考察（差异断言）。
        var npc = new TNormNpc();
        var player = NewPlayer();
        NpcSeams.GetPlayerDynamicVarList = _ => new List<TDynamicVar>
        {
            new() { sName = "AA", VarType = TVarType.vNone },
            new() { sName = "AA", VarType = TVarType.vInteger, nInternet = 99 }
        };
        string sValue = "";
        int nValue = 0;
        Assert.False(npc.GetDynamicValue(player, "<$HUMAN(AA)>", ref sValue, ref nValue));
        Assert.Equal(0, nValue);
    }

    [Fact]
    public void GetDynamicValue_NameMatchIsCaseInsensitive()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        NpcSeams.GetPlayerDynamicVarList = _ => new List<TDynamicVar>
        {
            new() { sName = "MixedCase", VarType = TVarType.vInteger, nInternet = 3 }
        };
        string sValue = "";
        int nValue = 0;
        Assert.True(npc.GetDynamicValue(player, "<$HUMAN(mixedcase)>", ref sValue, ref nValue));
        Assert.Equal(3, nValue);
    }

    [Fact]
    public void SetDynamicValue_IntegerAndString()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        var list = new List<TDynamicVar>
        {
            new() { sName = "IV", VarType = TVarType.vInteger },
            new() { sName = "SV", VarType = TVarType.vString }
        };
        NpcSeams.GetPlayerDynamicVarList = _ => list;

        Assert.True(npc.SetDynamicValue(player, "<$HUMAN(IV)>", "ignored", 42));
        Assert.Equal(42, list[0].nInternet);

        Assert.True(npc.SetDynamicValue(player, "<$HUMAN(SV)>", "text", 0));
        Assert.Equal("text", list[1].sString);
    }

    [Fact]
    public void SetDynamicValue_VNoneType_StillReturnsTrue()
    {
        // 与 GetDynamicValue 的差异：Set 版 `Result := True` 在 case 之后 → vNone 也返回 True。
        var npc = new TNormNpc();
        var player = NewPlayer();
        NpcSeams.GetPlayerDynamicVarList = _ => new List<TDynamicVar>
        {
            new() { sName = "AA", VarType = TVarType.vNone }
        };
        Assert.True(npc.SetDynamicValue(player, "<$HUMAN(AA)>", "x", 1));
    }

    [Fact]
    public void SetDynamicValue_NonWrapped_ReturnsFalse()
    {
        var npc = new TNormNpc();
        Assert.False(npc.SetDynamicValue(NewPlayer(), "PLAIN", "x", 1));
    }

    // -----------------------------------------------------------------------
    // GetDynamicVarList（9877-9898）
    // -----------------------------------------------------------------------

    [Fact]
    public void GetDynamicVarList_Human_SetsPlayerName()
    {
        var npc = new TNormNpc();
        var player = NewPlayer("NAMEX");
        var list = new List<TDynamicVar>();
        NpcSeams.GetPlayerDynamicVarList = _ => list;
        string sName = "";
        Assert.Same(list, npc.GetDynamicVarList(player, "HUMAN", ref sName));
        Assert.Equal("NAMEX", sName);
    }

    [Fact]
    public void GetDynamicVarList_PrefixMatchIsCaseInsensitiveAndPrefixOnly()
    {
        var npc = new TNormNpc();
        var player = NewPlayer("NAMEX");
        NpcSeams.GetPlayerDynamicVarList = _ => new List<TDynamicVar>();
        string sName = "";
        // 原文用 CompareLStr 做**前缀**比较 → "humanXYZ" 也命中 HUMAN 分支
        Assert.NotNull(npc.GetDynamicVarList(player, "humanXYZ", ref sName));
        Assert.Equal("NAMEX", sName);
    }

    [Fact]
    public void GetDynamicVarList_GuildWithoutGuild_ReturnsNullAndKeepsName()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        NpcSeams.GetPlayerGuildName = _ => null;
        string sName = "KEEP";
        Assert.Null(npc.GetDynamicVarList(player, "GUILD", ref sName));
        Assert.Equal("KEEP", sName);
    }

    [Fact]
    public void GetDynamicVarList_GuildWithGuild_SetsGuildName()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        var list = new List<TDynamicVar>();
        NpcSeams.GetPlayerGuildName = _ => "GUILDX";
        NpcSeams.GetGuildDynamicVarList = _ => list;
        string sName = "";
        Assert.Same(list, npc.GetDynamicVarList(player, "GUILD", ref sName));
        Assert.Equal("GUILDX", sName);
    }

    [Fact]
    public void GetDynamicVarList_Global_AlwaysSetsGlobalName()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        var list = new List<TDynamicVar>();
        NpcSeams.GetGlobalDynamicVarList = () => list;
        string sName = "";
        Assert.Same(list, npc.GetDynamicVarList(player, "GLOBAL", ref sName));
        Assert.Equal("GLOBAL", sName);
    }

    [Fact]
    public void GetDynamicVarList_UnknownScope_ReturnsNullAndKeepsName()
    {
        var npc = new TNormNpc();
        string sName = "KEEP";
        Assert.Null(npc.GetDynamicVarList(NewPlayer(), "OTHER", ref sName));
        Assert.Equal("KEEP", sName);
    }

    // -----------------------------------------------------------------------
    // GetLineVariableText（5981-6009）
    // -----------------------------------------------------------------------

    [Fact]
    public void GetLineVariableText_NoVariables_ReturnsInputUnchanged()
    {
        var npc = new TNormNpc();
        bool brk = false;
        Assert.Equal("PLAIN TEXT", npc.GetLineVariableText(NewPlayer(), "PLAIN TEXT", ref brk));
    }

    [Fact]
    public void GetLineVariableText_SingleVariable_CallsSeamOnceWithPosition()
    {
        var npc = new TNormNpc();
        int calls = 0;
        int seenPos = -1;
        string seenVar = null;
        NpcSeams.GetVariableText = (n, p, sMsg, sVariable, nPos) =>
        {
            calls++;
            seenPos = nPos;
            seenVar = sVariable;
            return (true, sMsg.Replace("<$V>", "X"), false);
        };
        bool brk = false;
        string r = npc.GetLineVariableText(NewPlayer(), "A<$V>B", ref brk);
        Assert.Equal("AXB", r);
        Assert.Equal(1, calls);
        // `ArrestVariable` 返回的是 `'<'` 的 **1-based 位置**（"A<$V>B" 里 `<` 在第 2 位），
        // 不是循环起点 nStartPos（起点恒为 1）。
        Assert.Equal(2, seenPos);
        // ⚠ 关键语义：`ArrestVariable` 取出的子串**含 `$`**（HUtil32.pas:1807 的 nLen 从 nPos+1 起算，
        // 而 nPos+1 正是 `$` 所在位）→ 传进 GetVariableText 的 sVariable 是 "$V"。
        // 这也与 GetVariableText 内部直接比较 `'$HUMAN('` 一致。
        Assert.Equal("$V", seenVar);
    }

    [Fact]
    public void GetLineVariableText_FailureAdvancesStartPosByTwo()
    {
        var npc = new TNormNpc();
        var positions = new List<int>();
        NpcSeams.GetVariableText = (n, p, sMsg, sVariable, nPos) =>
        {
            positions.Add(nPos);
            return (false, sMsg, false);
        };
        bool brk = false;
        // 两个变量都解析失败：第一次 nPos = 1（第 1 个 '<'），nStartPos := 3；
        // 第二次从 3 开始找下一个 '<' → 命中第 5 位。之后 nStartPos := 7 > 长度 → 循环结束。
        npc.GetLineVariableText(NewPlayer(), "<$A><$B>", ref brk);
        Assert.Equal(new[] { 1, 5 }, positions);
    }

    [Fact]
    public void GetLineVariableText_UnknownVariableRepeated_IsBoundedBy1001()
    {
        var npc = new TNormNpc();
        int calls = 0;
        // 每次失败都把 nStartPos 推进到下一个 `<$`，故 1500 个变量会一路走到 `nC >= 1001` 的强制退出。
        NpcSeams.GetVariableText = (n, p, sMsg, sVariable, nPos) => { calls++; return (false, sMsg, false); };
        bool brk = false;
        string msg = string.Concat(System.Linq.Enumerable.Repeat("<$A>", 1500));
        npc.GetLineVariableText(NewPlayer(), msg, ref brk);
        Assert.Equal(1001, calls);
    }

    [Fact]
    public void GetLineVariableText_SeamMaySetIsBreakParseVar()
    {
        var npc = new TNormNpc();
        NpcSeams.GetVariableText = (n, p, sMsg, sVariable, nPos) => (true, "done", true);
        bool brk = false;
        npc.GetLineVariableText(NewPlayer(), "<$A>", ref brk);
        Assert.True(brk);
    }

    // -----------------------------------------------------------------------
    // TConditionList.Create（504-507）
    // -----------------------------------------------------------------------

    [Fact]
    public void TConditionList_Create_DefaultsToAnd()
    {
        var list = new TConditionList();
        Assert.Equal(TConditionType.ct_and, list.ConditionType);
        Assert.Equal(0, list.TrueCount);
        Assert.Empty(list.Items);
    }

    [Fact]
    public void TConditionList_PropertiesRoundTrip()
    {
        var list = new TConditionList { ConditionType = TConditionType.ct_or, TrueCount = 7 };
        Assert.Equal(TConditionType.ct_or, list.FConditionType);
        Assert.Equal(7, list.FTrueCount);
    }
}
