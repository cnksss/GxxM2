// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas
// 测试对象：GXX.M2Server.Npc 切片 10（公会官员 / 攻城官员 / 商人 Click）
//   · TCastleOfficial.Click          原文 1107-1116
//   · TCastleOfficial.Create         原文 10364-10367
//   · TCastleOfficial.SendCustemMsg  原文 10391-10404
//   · TGuildOfficial.Click           原文 10049-10053
//   · TGuildOfficial.GetVariableText 原文 10055-10089
//   · TGuildOfficial.SendCustemMsg   原文 10386-10389
//   · TMerchant.Click                原文 3228-3232
//   · TNormNpc.Click 虚外壳          原文 4431-4442（本体接缝）
// ============================================================================

using System.Collections.Generic;
using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Npc;
using Xunit;

using TNormNpc = GXX.M2Server.Npc.TNormNpc;

namespace GXX.M2Server.Tests;

public sealed class NpcObjNpcGuildCastleTests : System.IDisposable
{
    private readonly List<string> _sysMsgs = new();
    private readonly List<string> _broadcasts = new();
    private readonly List<TNormNpc> _clicked = new();

    public NpcObjNpcGuildCastleTests()
    {
        NpcSeams.ResetDefaults();
        NpcSeams.SysMsg = (target, msg, color, type) => _sysMsgs.Add($"{msg}/{color}/{type}");
        NpcSeams.SendBroadCastMsg = (msg, type) => _broadcasts.Add($"{msg}/{type}");
        NpcSeams.Click = (npc, player) => _clicked.Add(npc);
    }

    public void Dispose() => NpcSeams.ResetDefaults();

    private static TPlayObject NewPlayer(string name = "P1") => new() { m_sCharName = name };

    // -----------------------------------------------------------------------
    // TNormNpc.Click 虚外壳（4431-4442）
    // -----------------------------------------------------------------------

    [Fact]
    public void NormNpcClick_ForwardsToSeam()
    {
        var npc = new TNormNpc();
        var player = NewPlayer();
        npc.Click(player);
        Assert.Single(_clicked);
        Assert.Same(npc, _clicked[0]);
    }

    // -----------------------------------------------------------------------
    // TMerchant.Click（3228-3232）/ TGuildOfficial.Click（10049-10053）
    // -----------------------------------------------------------------------

    [Fact]
    public void MerchantClick_ForwardsToBase()
    {
        var m = new TMerchant();
        m.Click(NewPlayer());
        Assert.Single(_clicked);
        Assert.Same(m, _clicked[0]);
    }

    [Fact]
    public void GuildOfficialClick_ForwardsToBase()
    {
        var g = new TGuildOfficial();
        g.Click(NewPlayer());
        Assert.Single(_clicked);
        Assert.Same(g, _clicked[0]);
    }

    // -----------------------------------------------------------------------
    // TGuildOfficial.SendCustemMsg（10386-10389）
    // -----------------------------------------------------------------------

    [Fact]
    public void GuildOfficialSendCustemMsg_ForwardsToBaseSeam()
    {
        var g = new TGuildOfficial();
        string seen = null;
        NpcSeams.SendCustemMsg = (npc, player, msg) => { Assert.Same(g, npc); seen = msg; };
        g.SendCustemMsg(NewPlayer(), "HELLO");
        Assert.Equal("HELLO", seen);
    }

    // -----------------------------------------------------------------------
    // TCastleOfficial.Create（10364-10367）
    // -----------------------------------------------------------------------

    [Fact]
    public void CastleOfficialCreate_IsConstructibleAndInheritsMerchantDefaults()
    {
        var c = new TCastleOfficial();
        Assert.NotNull(c);
        Assert.Equal(100, c.m_nPriceRate);   // TMerchant 默认价率
        Assert.Equal(0, c.m_btRaceServer);   // 原文 Create 只调 inherited，不置种族值
    }

    // -----------------------------------------------------------------------
    // TCastleOfficial.Click（1107-1116）
    // -----------------------------------------------------------------------

    [Fact]
    public void CastleOfficialClick_NoCastle_WarnsAndDoesNotOpenDialog()
    {
        var c = new TCastleOfficial();
        NpcSeams.GetNpcCastle = _ => null;
        c.Click(NewPlayer());
        Assert.Empty(_clicked);
        Assert.Single(_sysMsgs);
        Assert.Equal($"NPC不属于城堡！/{TMsgColor.c_Red}/{TMsgType.t_Hint}", _sysMsgs[0]);
    }

    [Fact]
    public void CastleOfficialClick_NonMemberLowPermission_SilentlyDoesNothing()
    {
        var c = new TCastleOfficial();
        NpcSeams.GetNpcCastle = _ => new object();
        NpcSeams.IsMasterGuild = (_, _) => false;
        NpcSeams.GetPlayerPermission = _ => 2;
        c.Click(NewPlayer());
        Assert.Empty(_clicked);
        Assert.Empty(_sysMsgs);   // 原文此分支既不提示也不 inherited
    }

    [Fact]
    public void CastleOfficialClick_MasterGuild_OpensDialog()
    {
        var c = new TCastleOfficial();
        NpcSeams.GetNpcCastle = _ => new object();
        NpcSeams.IsMasterGuild = (_, _) => true;
        NpcSeams.GetPlayerPermission = _ => 0;
        c.Click(NewPlayer());
        Assert.Single(_clicked);
    }

    [Fact]
    public void CastleOfficialClick_PermissionThree_OpensDialogEvenIfNotMasterGuild()
    {
        var c = new TCastleOfficial();
        NpcSeams.GetNpcCastle = _ => new object();
        NpcSeams.IsMasterGuild = (_, _) => false;
        NpcSeams.GetPlayerPermission = _ => 3;
        c.Click(NewPlayer());
        Assert.Single(_clicked);
    }

    [Fact]
    public void CastleOfficialClick_NullCastleShortCircuitsMasterGuildCheck()
    {
        var c = new TCastleOfficial();
        NpcSeams.GetNpcCastle = _ => null;
        bool called = false;
        NpcSeams.IsMasterGuild = (_, _) => { called = true; return true; };
        c.Click(NewPlayer());
        Assert.False(called);
    }

    // -----------------------------------------------------------------------
    // TCastleOfficial.SendCustemMsg（10391-10404）
    // -----------------------------------------------------------------------

    [Fact]
    public void CastleOfficialSendCustemMsg_SwitchOff_WarnsAndStops()
    {
        var c = new TCastleOfficial();
        NpcSeams.boSubkMasterSendMsg = false;
        NpcSeams.g_sSubkMasterMsgCanNotUseNowMsg = "不可用";
        c.SendCustemMsg(NewPlayer(), "hi");
        Assert.Empty(_broadcasts);
        Assert.Single(_sysMsgs);
        Assert.Equal($"不可用/{TMsgColor.c_Red}/{TMsgType.t_Hint}", _sysMsgs[0]);
    }

    [Fact]
    public void CastleOfficialSendCustemMsg_SwitchOnButFlagUnset_NoBroadcast()
    {
        var c = new TCastleOfficial();
        NpcSeams.boSubkMasterSendMsg = true;
        NpcSeams.GetSendMsgFlag = _ => false;
        c.SendCustemMsg(NewPlayer(), "hi");
        Assert.Empty(_broadcasts);
        Assert.Empty(_sysMsgs);
    }

    [Fact]
    public void CastleOfficialSendCustemMsg_SwitchOnAndFlagSet_BroadcastsWithCastleType()
    {
        var c = new TCastleOfficial();
        NpcSeams.boSubkMasterSendMsg = true;
        NpcSeams.GetSendMsgFlag = _ => true;
        bool cleared = false;
        NpcSeams.ClearSendMsgFlag = p => cleared = true;

        c.SendCustemMsg(NewPlayer("城主"), "大家好");

        Assert.True(cleared);
        Assert.Single(_broadcasts);
        Assert.Equal($"城主: 大家好/{TMsgType.t_Castle}", _broadcasts[0]);
    }

    [Fact]
    public void CastleOfficialSendCustemMsg_DoesNotCallBaseSeam()
    {
        // 原文 10391-10404 **没有** inherited —— 与 TGuildOfficial 版不同。
        var c = new TCastleOfficial();
        NpcSeams.boSubkMasterSendMsg = true;
        NpcSeams.GetSendMsgFlag = _ => false;
        bool baseCalled = false;
        NpcSeams.SendCustemMsg = (_, _, _) => baseCalled = true;
        c.SendCustemMsg(NewPlayer(), "hi");
        Assert.False(baseCalled);
    }

    // -----------------------------------------------------------------------
    // TGuildOfficial.GetVariableText（10055-10089）
    // -----------------------------------------------------------------------

    [Fact]
    public void GuildOfficialGetVariableText_BaseHandled_SkipsGuildBranch()
    {
        var g = new TGuildOfficial();
        NpcSeams.GetCastleNameList = _ => throw new System.Exception("should not be called");
        NpcSeams.GetVariableText = (n, p, sMsg, sVar, nPos) => (true, "BASE", false);
        string sMsg = "<$REQUESTCASTLELIST>";
        bool brk = false;
        Assert.True(g.GetVariableText(NewPlayer(), ref sMsg, "$REQUESTCASTLELIST", ref brk, 1));
        Assert.Equal("BASE", sMsg);
    }

    [Fact]
    public void GuildOfficialGetVariableText_UnknownVariable_ReturnsFalse()
    {
        var g = new TGuildOfficial();
        string sMsg = "<$NOPE>";
        bool brk = false;
        Assert.False(g.GetVariableText(NewPlayer(), ref sMsg, "$NOPE", ref brk, 1));
        Assert.Equal("<$NOPE>", sMsg);
    }

    [Fact]
    public void GuildOfficialGetVariableText_CastleListFormatsRowsWithBackslashes()
    {
        var g = new TGuildOfficial();
        NpcSeams.GetCastleNameList = list => { list.Add("A"); list.Add("B"); list.Add("C"); };
        string sMsg = "<$REQUESTCASTLELIST>";
        bool brk = false;

        Assert.True(g.GetVariableText(NewPlayer(), ref sMsg, "$REQUESTCASTLELIST", ref brk, 1));

        // 原文 10074-10079：II = I+1；II 为**偶数**时插一个 '\'（每两项一行）
        //   I=0 → "<A/@requestcastlewarnow0> "
        //   I=1 → "<B/@requestcastlewarnow1> \"     ← 单反斜杠
        //   I=2 → "<C/@requestcastlewarnow2> "
        // 末尾 10082 追加 '\ \'（反斜杠 空格 反斜杠）
        string expected =
            "<A/@requestcastlewarnow0> <B/@requestcastlewarnow1> \\<C/@requestcastlewarnow2> \\ \\";
        Assert.Equal(expected, sMsg);
    }

    [Fact]
    public void GuildOfficialGetVariableText_EmptyCastleList_StillAppendsTail()
    {
        var g = new TGuildOfficial();
        NpcSeams.GetCastleNameList = _ => { };
        string sMsg = "<$REQUESTCASTLELIST>";
        bool brk = false;
        Assert.True(g.GetVariableText(NewPlayer(), ref sMsg, "$REQUESTCASTLELIST", ref brk, 1));
        Assert.Equal("\\ \\", sMsg);
    }

    [Fact]
    public void GuildOfficialGetVariableText_LowercaseVariableIsUppercased()
    {
        var g = new TGuildOfficial();
        NpcSeams.GetCastleNameList = list => list.Add("X");
        string sMsg = "<$requestcastlelist>";
        bool brk = false;
        Assert.True(g.GetVariableText(NewPlayer(), ref sMsg, "$requestcastlelist", ref brk, 1));
        Assert.Equal("<X/@requestcastlewarnow0> \\ \\", sMsg);
    }

    [Fact]
    public void GuildOfficialGetVariableText_ReceivesListOfCoreStringList()
    {
        var g = new TGuildOfficial();
        TStringList seen = null;
        NpcSeams.GetCastleNameList = list => { seen = list; };
        string sMsg = "<$REQUESTCASTLELIST>";
        bool brk = false;
        g.GetVariableText(NewPlayer(), ref sMsg, "$REQUESTCASTLELIST", ref brk, 1);
        Assert.NotNull(seen);
        Assert.IsType<TStringList>(seen);
    }
}
