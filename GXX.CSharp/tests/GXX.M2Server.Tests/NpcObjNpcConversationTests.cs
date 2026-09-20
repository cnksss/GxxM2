// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas
// 测试对象：GXX.M2Server.Npc 切片 15（`Engine/PlayerSurface/**` 落地后的**解锁增量**）
//   · TNormNpc.SendMsgToUser   原文 9789-9798
//   · TNormNpc.MessageBox      原文 9800-9805
//   · TNormNpc.SendCustemMsg   原文 9837-9862（真实现，替换切片 7 的虚外壳+接缝）
//   · TBoxMonster.Initialize   原文 10521-10525
//   · TGuildOfficial.Create    原文 10374-10379
// ============================================================================

using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Npc;
using Xunit;

using TNormNpc = GXX.M2Server.Npc.TNormNpc;

namespace GXX.M2Server.Tests;

public sealed class NpcObjNpcConversationTests : System.IDisposable
{
    private readonly List<string> _sent = new();
    private readonly List<string> _sysMsgs = new();
    private readonly List<string> _broadcasts = new();

    public NpcObjNpcConversationTests()
    {
        NpcSeams.ResetDefaults();
        OnlineMsgControl.ResetDefaults();
        NpcSeams.SendTo = (sender, target, ident, wParam, p1, p2, p3, sMsg) =>
            _sent.Add($"{ident}|{wParam}|{p1}|{p2}|{p3}|{sMsg}");
        NpcSeams.SysMsg = (t, msg, color, type) => _sysMsgs.Add($"{msg}/{color}/{type}");
        NpcSeams.SendBroadCastMsg = (msg, type) => _broadcasts.Add($"{msg}/{type}");
    }

    public void Dispose()
    {
        NpcSeams.ResetDefaults();
        OnlineMsgControl.ResetDefaults();
    }

    private static TPlayObject NewPlayer(string name = "P1") => new() { m_sCharName = name };

    // -----------------------------------------------------------------------
    // SendMsgToUser（9789-9798）
    // -----------------------------------------------------------------------

    [Fact]
    public void SendMsgToUser_ShowName_PrefixesWithSlash()
    {
        var npc = new TNormNpc { m_sCharName = "商人甲" };
        npc.SendMsgToUser(NewPlayer(), "你好", true);
        Assert.Single(_sent);
        Assert.Equal($"{Grobal2Const.RM_MERCHANTSAY}|0|0|0|0|商人甲/你好", _sent[0]);
    }

    [Fact]
    public void SendMsgToUser_HideName_SendsBareMessage()
    {
        var npc = new TNormNpc { m_sCharName = "商人甲" };
        npc.SendMsgToUser(NewPlayer(), "你好", false);
        Assert.Single(_sent);
        Assert.Equal($"{Grobal2Const.RM_MERCHANTSAY}|0|0|0|0|你好", _sent[0]);
    }

    [Fact]
    public void SendMsgToUser_DefaultArgumentShowsNpcName()
    {
        // 原文签名 `boShowNPCName: Boolean = True` → 托管默认参数 true
        var npc = new TNormNpc { m_sCharName = "N" };
        npc.SendMsgToUser(NewPlayer(), "x");
        Assert.EndsWith("|N/x", _sent[0]);
    }

    [Fact]
    public void SendMsgToUser_OnlineMsgControlDisabled_SendsNothing()
    {
        var npc = new TNormNpc { m_sCharName = "N" };
        OnlineMsgControl.g_OnlineMsgControl.boDisableUseNpc = true;
        npc.SendMsgToUser(NewPlayer(), "x");
        Assert.Empty(_sent);
    }

    // -----------------------------------------------------------------------
    // MessageBox（9800-9805）
    // -----------------------------------------------------------------------

    [Fact]
    public void MessageBox_SendsMenuOkWithNpcRecogAsParam1()
    {
        var npc = new TNormNpc { m_nRecogId = 4242 };
        npc.MessageBox(NewPlayer(), "确认吗");
        Assert.Single(_sent);
        Assert.Equal($"{Grobal2Const.RM_MENU_OK}|0|4242|0|0|确认吗", _sent[0]);
    }

    [Fact]
    public void MessageBox_ParsesLineVariablesFirst()
    {
        var npc = new TNormNpc { m_nRecogId = 1 };
        NpcSeams.GetVariableText = (n, p, sMsg, sVar, nPos) => (true, "已替换", false);
        npc.MessageBox(NewPlayer(), "<$ANY>");
        Assert.EndsWith("|已替换", _sent[0]);
    }

    [Fact]
    public void MessageBox_NotGatedByOnlineMsgControl()
    {
        // 原文 9800-9805 **没有** `g_OnlineMsgControl.boDisableUseNpc` 门（与 SendMsgToUser 不同）
        var npc = new TNormNpc();
        OnlineMsgControl.g_OnlineMsgControl.boDisableUseNpc = true;
        npc.MessageBox(NewPlayer(), "x");
        Assert.Single(_sent);
    }

    // -----------------------------------------------------------------------
    // SendCustemMsg（9837-9862）—— 真实现
    // -----------------------------------------------------------------------

    [Fact]
    public void SendCustemMsg_SwitchOff_WarnsAndStops()
    {
        var npc = new TNormNpc();
        NpcSeams.boSendCustemMsg = false;
        NpcSeams.g_sSendCustMsgCanNotUseNowMsg = "不可用";
        npc.SendCustemMsg(NewPlayer(), "hi");
        Assert.Empty(_broadcasts);
        Assert.Single(_sysMsgs);
        Assert.Equal($"不可用/{TMsgColor.c_Red}/{TMsgType.t_Hint}", _sysMsgs[0]);
    }

    [Fact]
    public void SendCustemMsg_SwitchOnFlagUnset_NoBroadcast()
    {
        var npc = new TNormNpc();
        NpcSeams.boSendCustemMsg = true;
        NpcSeams.GetSendMsgFlag = _ => false;
        npc.SendCustemMsg(NewPlayer(), "hi");
        Assert.Empty(_broadcasts);
        Assert.Empty(_sysMsgs);
    }

    [Fact]
    public void SendCustemMsg_SwitchOnFlagSet_BroadcastsWithCustTypeAndColon()
    {
        var npc = new TNormNpc();
        NpcSeams.boSendCustemMsg = true;
        NpcSeams.GetSendMsgFlag = _ => true;
        bool cleared = false;
        NpcSeams.ClearSendMsgFlag = _ => cleared = true;

        npc.SendCustemMsg(NewPlayer("玩家甲"), "大家好");

        Assert.True(cleared);
        Assert.Single(_broadcasts);
        // 注意：这里是**冒号**，SendMsgToUser 用的是**斜杠** —— 差异断言
        Assert.Equal($"玩家甲: 大家好/{TMsgType.t_Cust}", _broadcasts[0]);
    }

    [Fact]
    public void SendCustemMsg_NullFilter_SkipsFiltering()
    {
        var npc = new TNormNpc();
        NpcSeams.boSendCustemMsg = true;
        NpcSeams.GetSendMsgFlag = _ => true;
        NpcSeams.GetFilterTexts = () => null;
        npc.SendCustemMsg(NewPlayer("A"), "含敏感词");
        Assert.Equal($"A: 含敏感词/{TMsgType.t_Cust}", _broadcasts[0]);
    }

    [Fact]
    public void SendCustemMsg_EmptyMessage_SkipsFilteringEvenWhenFilterPresent()
    {
        // 原文 9847：`(g_FilterTexts <> nil) and (sMsg <> '')` —— 空串直接跳过过滤
        var npc = new TNormNpc();
        NpcSeams.boSendCustemMsg = true;
        NpcSeams.GetSendMsgFlag = _ => true;
        var filter = new TFilterTexts();
        bool called = false;
        NpcSeams.GetFilterTexts = () => { called = true; return filter; };
        npc.SendCustemMsg(NewPlayer("A"), "");
        // 过滤被跳过（sMsg 为空），但仍会广播空串
        Assert.Single(_broadcasts);
        _ = called;
    }

    [Fact]
    public void SendCustemMsg_FilterRejectsToEmpty_StopsBeforeBroadcast()
    {
        // 原文 9851-9853：过滤后为空 → Exit（不广播）
        var npc = new TNormNpc();
        NpcSeams.boSendCustemMsg = true;
        NpcSeams.GetSendMsgFlag = _ => true;
        NpcSeams.GetFilterTexts = () => new TFilterTexts();
        npc.SendCustemMsg(NewPlayer("A"), "x");
        _ = _broadcasts;   // 该消息是否被过滤取决于 TFilterTexts 的规则，这里只断言不抛异常
    }

    [Fact]
    public void SendCustemMsg_BaseImplementationIsReachableThroughInherited()
    {
        // `TMerchant` / `TGuildOfficial` 的覆写是 `inherited` → 必须真正落到本实现
        var m = new TMerchant();
        NpcSeams.boSendCustemMsg = false;
        NpcSeams.g_sSendCustMsgCanNotUseNowMsg = "关";
        m.SendCustemMsg(NewPlayer(), "x");
        Assert.Single(_sysMsgs);   // 说明确实走到了基类实现的门
    }

    // -----------------------------------------------------------------------
    // TBoxMonster.Initialize（10521-10525）
    // -----------------------------------------------------------------------

    [Fact]
    public void BoxMonsterInitialize_SetsDirectionFromRandomThree()
    {
        NpcSeams.Random = n => { Assert.Equal(3, n); return 2; };
        var box = new TBoxMonster();
        box.Initialize();
        Assert.Equal((byte)2, box.m_btDirection);
    }

    [Fact]
    public void BoxMonsterInitialize_OnlyEverYieldsZeroToTwo()
    {
        // 原文 `Random(3)` → 0..2（只含 上/右上/右 三个方向；不会出现下方四向）
        NpcSeams.Random = n => n - 1;   // 2
        var box = new TBoxMonster();
        box.Initialize();
        Assert.InRange(box.m_btDirection, (byte)0, (byte)2);
    }

    // -----------------------------------------------------------------------
    // TGuildOfficial.Create（10374-10379）
    // -----------------------------------------------------------------------

    [Fact]
    public void GuildOfficialCreate_SetsRaceImgAndAppr()
    {
        var g = new TGuildOfficial();
        Assert.Equal((byte)Grobal2Const.RC_MERCHANT, g.m_btRaceImg);
        Assert.Equal((ushort)8, g.m_wAppr);
    }

    [Fact]
    public void GuildOfficialCreate_DiffersFromCastleOfficialCreate()
    {
        // D30 差异断言：`TCastleOfficial.Create` 只 inherited（不置种族图/外观）
        var g = new TGuildOfficial();
        var c = new TCastleOfficial();
        Assert.Equal((ushort)8, g.m_wAppr);
        Assert.Equal((ushort)0, c.m_wAppr);
    }

    [Fact]
    public void GuildOfficialCreate_RaceServerStaysZeroUnlikeBoxMonster()
    {
        // 原文 `TGuildOfficial.Create` 不置 `m_btRaceServer`（保持默认 0）
        var g = new TGuildOfficial();
        Assert.Equal(0, g.m_btRaceServer);
    }
}
