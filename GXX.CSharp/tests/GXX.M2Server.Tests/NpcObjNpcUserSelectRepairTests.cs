// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas
// 测试对象：TMerchant.UserSelect 的 **@repair 分片**
//   2089-2092 `SuperRepairItem` / 2262-2265 `RepairItem`
//   2702-2706 `case nNF_SuperRepair`（守卫 m_boS_repair）
//   2737-2741 `case nNF_Repair`（守卫 m_boRepair）
// ⚠ 派发体（2547-2899）未移植 → `UserSelect` 整体仍是 Missing；本文件只锁本分片语义。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Npc;
using Xunit;

namespace GXX.M2Server.Tests;

public sealed class NpcObjNpcUserSelectRepairTests : IDisposable
{
    private readonly List<string> _sent = new();

    public NpcObjNpcUserSelectRepairTests()
    {
        NpcSeams.ResetDefaults();
        NpcSeams.SendTo = (sender, target, ident, wParam, p1, p2, p3, sMsg) =>
            _sent.Add($"{ident}|{wParam}|{p1}|{p2}|{p3}|{sMsg}");
    }

    public void Dispose() => NpcSeams.ResetDefaults();

    private static TMerchant Merchant() => new() { m_sCharName = "商人", m_nRecogId = 4242 };
    private static TPlayObject Player() => new() { m_sCharName = "玩家" };

    // -----------------------------------------------------------------------
    // SuperRepairItem（2089-2092）
    // -----------------------------------------------------------------------

    [Fact]
    public void SuperRepairItem_SendsRmSendUsersRepairWithSelfId()
    {
        var m = Merchant();
        m.SuperRepairItem(Player());

        Assert.Single(_sent);
        // RM_SENDUSERSREPAIR=20119? 否 —— 用常量本身断言，避免硬编码
        Assert.StartsWith($"{Grobal2Const.RM_SENDUSERSREPAIR}|0|4242|0|0|", _sent[0]);
    }

    [Fact]
    public void SuperRepairItem_UsesSuperRepairPacket_NotRepairPacket()
    {
        // ★ 差异断言：RM_SENDUSERSREPAIR(913) 与 RM_SENDUSERREPAIR(916) 是**两个不同**的包
        var m = Merchant();
        m.SuperRepairItem(Player());
        Assert.DoesNotContain(_sent, s => s.StartsWith($"{Grobal2Const.RM_SENDUSERREPAIR}|"));
    }

    // -----------------------------------------------------------------------
    // RepairItem（2262-2265）
    // -----------------------------------------------------------------------

    [Fact]
    public void RepairItem_SendsRmSendUserRepairWithSelfId()
    {
        var m = Merchant();
        m.RepairItem(Player());

        Assert.Single(_sent);
        Assert.StartsWith($"{Grobal2Const.RM_SENDUSERREPAIR}|0|4242|0|0|", _sent[0]);
    }

    [Fact]
    public void RepairItem_UsesRepairPacket_NotSuperRepairPacket()
    {
        var m = Merchant();
        m.RepairItem(Player());
        Assert.DoesNotContain(_sent, s => s.StartsWith($"{Grobal2Const.RM_SENDUSERSREPAIR}|"));
    }

    [Fact]
    public void TwoRepairPackets_AreDistinctConstants()
    {
        // 两个包号在原文里确实不同（Grobal2.Const.g.cs:913 vs :916）
        Assert.NotEqual(Grobal2Const.RM_SENDUSERSREPAIR, Grobal2Const.RM_SENDUSERREPAIR);
    }

    // -----------------------------------------------------------------------
    // 派发的两条 case 分支（2702-2706 / 2737-2741）
    // -----------------------------------------------------------------------

    [Fact]
    public void UserSelectRepairCommands_SuperRepairFlagOn_SendsSuperRepair()
    {
        var m = Merchant();
        m.m_boS_repair = true;
        Assert.True(m.UserSelectRepairCommands(Player(), NpcProcessCmd.nNF_SuperRepair));
        Assert.StartsWith($"{Grobal2Const.RM_SENDUSERSREPAIR}|", Assert.Single(_sent));
    }

    [Fact]
    public void UserSelectRepairCommands_SuperRepairFlagOff_SendsNothing()
    {
        // 原文 2704 的守卫 `if m_boS_repair then`
        var m = Merchant();
        m.m_boS_repair = false;
        Assert.True(m.UserSelectRepairCommands(Player(), NpcProcessCmd.nNF_SuperRepair));
        Assert.Empty(_sent);
    }

    [Fact]
    public void UserSelectRepairCommands_RepairFlagOn_SendsRepair()
    {
        var m = Merchant();
        m.m_boRepair = true;
        Assert.True(m.UserSelectRepairCommands(Player(), NpcProcessCmd.nNF_Repair));
        Assert.StartsWith($"{Grobal2Const.RM_SENDUSERREPAIR}|", Assert.Single(_sent));
    }

    [Fact]
    public void UserSelectRepairCommands_RepairFlagOff_SendsNothing()
    {
        var m = Merchant();
        m.m_boRepair = false;
        Assert.True(m.UserSelectRepairCommands(Player(), NpcProcessCmd.nNF_Repair));
        Assert.Empty(_sent);
    }

    [Fact]
    public void UserSelectRepairCommands_FlagsAreIndependent()
    {
        // ★ 两个守卫是**各自独立**的字段：只开 m_boS_repair 不该放行 nNF_Repair
        var m = Merchant();
        m.m_boS_repair = true;
        m.m_boRepair = false;

        m.UserSelectRepairCommands(Player(), NpcProcessCmd.nNF_Repair);
        Assert.Empty(_sent);

        m.UserSelectRepairCommands(Player(), NpcProcessCmd.nNF_SuperRepair);
        Assert.Single(_sent);
    }

    [Fact]
    public void UserSelectRepairCommands_OtherCommandId_ReturnsFalseAndSendsNothing()
    {
        // ★ 关键：不属 @repair 族时必须返回 false（**不能**当静默兜底）
        var m = Merchant();
        m.m_boS_repair = true;
        m.m_boRepair = true;

        // 用 `14`（原文 `nNF_Buy`，NpcCommon.pas:36）作"非 @repair 族"的见证 ——
        // 本分片只声明了自用的两个命令号常量，其余 30+ 个待对应分片再登记。
        Assert.False(m.UserSelectRepairCommands(Player(), 14));
        Assert.Empty(_sent);
    }

    [Fact]
    public void UserSelectRepairCommands_TotalConstantValuesAreOriginal()
    {
        // 原文 NpcCommon.pas:26 / :32 —— 这两个值错了会静默派发到别的分支
        Assert.Equal(9, NpcProcessCmd.nNF_SuperRepair);
        Assert.Equal(12, NpcProcessCmd.nNF_Repair);
        // 且二者不相等（否则两条分支会撞在一起）
        Assert.NotEqual(NpcProcessCmd.nNF_SuperRepair, NpcProcessCmd.nNF_Repair);
    }

    // -----------------------------------------------------------------------
    // 派发基础设施（g_NpcProcessCommand 表；原文 NpcCommon.pas:1899-1962）
    // -----------------------------------------------------------------------

    [Fact]
    public void G_NpcProcessCommand_HasAll68Entries()
    {
        // 原文 1899-1962 共 68 条 AddObject
        Assert.Equal(68, NpcProcessCmd.g_NpcProcessCommand.Count);
    }

    [Theory]
    [InlineData("@repair", 12)]          // sNF_Repair → nNF_Repair
    [InlineData("@s_repair", 9)]         // sNF_SuperRepair → nNF_SuperRepair
    [InlineData("~@repair", 13)]         // sNF_RepairOK → nNF_RepairOK
    [InlineData("@buy", 14)]
    [InlineData("@sell", 15)]
    [InlineData("@main", 43)]
    [InlineData("~@main", 44)]
    [InlineData("@upgradenow", 32)]
    [InlineData("~@upgradenow_ok", 34)]
    public void G_NpcProcessCommand_MapsLabelToCommandId(string label, int expected)
        => Assert.Equal(expected, NpcProcessCmd.g_NpcProcessCommand.GetCommand(label));

    [Fact]
    public void G_NpcProcessCommand_UnknownLabel_ReturnsMinusOne()
    {
        // 原文 2692 `if nIndex >= 0 then` —— 不在表中即 -1
        Assert.Equal(-1, NpcProcessCmd.g_NpcProcessCommand.GetCommand("@not_a_command"));
        Assert.Equal(-1, NpcProcessCmd.g_NpcProcessCommand.IndexOf("@not_a_command"));
    }

    [Fact]
    public void G_NpcProcessCommand_IndexOfIsCaseInsensitive_LikeDelphiTStringList()
    {
        // Delphi `TStringList.IndexOf` 默认 `CaseSensitive = False`
        Assert.Equal(NpcProcessCmd.g_NpcProcessCommand.IndexOf("@REPAIR"),
            NpcProcessCmd.g_NpcProcessCommand.IndexOf("@repair"));
        Assert.Equal(NpcProcessCmd.nNF_Repair, NpcProcessCmd.g_NpcProcessCommand.GetCommand("@RePaIr"));
    }

    [Fact]
    public void G_NpcProcessCommand_RepairAndSuperRepairAreDistinctEntries()
    {
        // ★ 差异断言：`@repair`(12) 与 `@s_repair`(9) 是**两条**不同命令
        Assert.NotEqual(
            NpcProcessCmd.g_NpcProcessCommand.GetCommand(NpcProcessCmd.sNF_Repair),
            NpcProcessCmd.g_NpcProcessCommand.GetCommand(NpcProcessCmd.sNF_SuperRepair));
    }

    [Fact]
    public void G_NpcProcessCommand_TotalCommandIdsAreDistinct()
    {
        // 68 个命令号必须两两不同（否则 case 分支会互相遮蔽）
        var ids = new System.Collections.Generic.HashSet<int>();
        foreach (var label in NpcProcessCmd.g_NpcProcessCommand.Order)
            Assert.True(ids.Add(NpcProcessCmd.g_NpcProcessCommand.GetCommand(label)), $"重复命令号: {label}");
        Assert.Equal(68, ids.Count);
    }

    [Fact]
    public void RepairLabelConstants_MatchNpcCommon()
    {
        // NpcCommon.pas:33 / :35
        Assert.Equal("@repair", NpcProcessCmd.sNF_Repair);
        Assert.Equal("~@repair", NpcProcessCmd.sNF_RepairOK);
        Assert.NotEqual(NpcProcessCmd.sNF_Repair, NpcProcessCmd.sNF_RepairOK);
    }
}
