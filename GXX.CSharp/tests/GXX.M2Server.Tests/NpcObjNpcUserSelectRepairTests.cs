// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas
// 测试对象：TMerchant.UserSelect 的**已移植 case 分支**（逐片累积）
//   · @s_repair       2089-2092 SuperRepairItem    （2702-2706，守卫 m_boS_repair）
//   · @sell           2257-2260 SellItem           （2732-2736，守卫 m_boSell）
//   · @repair         2262-2265 RepairItem         （2737-2741，守卫 m_boRepair）
//   · @armremovestone 2267-2270 ArmRemoveStoneItem （2742-2746，守卫 m_boArmRemoveStone）
// ★ 相邻分支"看起来一样实则不同"：三个纯发包过程**只有包号与守卫不同** → 逐一互相对照的差异断言。
// ⚠ 派发体（2597-2899）未移植 → `UserSelect` 整体仍是 Missing；本文件只锁已移植分支的语义。
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
    public void UserSelectPortedArms_SuperRepairFlagOn_SendsSuperRepair()
    {
        var m = Merchant();
        m.m_boS_repair = true;
        Assert.True(m.UserSelectPortedArms(Player(), NpcProcessCmd.nNF_SuperRepair));
        Assert.StartsWith($"{Grobal2Const.RM_SENDUSERSREPAIR}|", Assert.Single(_sent));
    }

    [Fact]
    public void UserSelectPortedArms_SuperRepairFlagOff_SendsNothing()
    {
        // 原文 2704 的守卫 `if m_boS_repair then`
        var m = Merchant();
        m.m_boS_repair = false;
        Assert.True(m.UserSelectPortedArms(Player(), NpcProcessCmd.nNF_SuperRepair));
        Assert.Empty(_sent);
    }

    [Fact]
    public void UserSelectPortedArms_RepairFlagOn_SendsRepair()
    {
        var m = Merchant();
        m.m_boRepair = true;
        Assert.True(m.UserSelectPortedArms(Player(), NpcProcessCmd.nNF_Repair));
        Assert.StartsWith($"{Grobal2Const.RM_SENDUSERREPAIR}|", Assert.Single(_sent));
    }

    [Fact]
    public void UserSelectPortedArms_RepairFlagOff_SendsNothing()
    {
        var m = Merchant();
        m.m_boRepair = false;
        Assert.True(m.UserSelectPortedArms(Player(), NpcProcessCmd.nNF_Repair));
        Assert.Empty(_sent);
    }

    [Fact]
    public void UserSelectPortedArms_FlagsAreIndependent()
    {
        // ★ 两个守卫是**各自独立**的字段：只开 m_boS_repair 不该放行 nNF_Repair
        var m = Merchant();
        m.m_boS_repair = true;
        m.m_boRepair = false;

        m.UserSelectPortedArms(Player(), NpcProcessCmd.nNF_Repair);
        Assert.Empty(_sent);

        m.UserSelectPortedArms(Player(), NpcProcessCmd.nNF_SuperRepair);
        Assert.Single(_sent);
    }

    [Fact]
    public void UserSelectPortedArms_OtherCommandId_ReturnsFalseAndSendsNothing()
    {
        // ★ 关键：不属 @repair 族时必须返回 false（**不能**当静默兜底）
        var m = Merchant();
        m.m_boS_repair = true;
        m.m_boRepair = true;

        // 用 `14`（原文 `nNF_Buy`，NpcCommon.pas:36）作"非 @repair 族"的见证 ——
        // 本分片只声明了自用的两个命令号常量，其余 30+ 个待对应分片再登记。
        Assert.False(m.UserSelectPortedArms(Player(), 14));
        Assert.Empty(_sent);
    }

    [Fact]
    public void UserSelectPortedArms_TotalConstantValuesAreOriginal()
    {
        // 原文 NpcCommon.pas:26 / :32 —— 这两个值错了会静默派发到别的分支
        Assert.Equal(9, NpcProcessCmd.nNF_SuperRepair);
        Assert.Equal(12, NpcProcessCmd.nNF_Repair);
        // 且二者不相等（否则两条分支会撞在一起）
        Assert.NotEqual(NpcProcessCmd.nNF_SuperRepair, NpcProcessCmd.nNF_Repair);
    }

    // -----------------------------------------------------------------------
    // SellItem（2257-2260）+ ArmRemoveStoneItem（2267-2270）
    // -----------------------------------------------------------------------

    [Fact]
    public void SellItem_SendsRmSendUserSellWithSelfId()
    {
        var m = Merchant();
        m.SellItem(Player());
        Assert.Single(_sent);
        Assert.StartsWith($"{Grobal2Const.RM_SENDUSERSELL}|0|4242|0|0|", _sent[0]);
    }

    [Fact]
    public void ArmRemoveStoneItem_SendsRmArmRemoveStoneWithSelfId()
    {
        var m = Merchant();
        m.ArmRemoveStoneItem(Player());
        Assert.Single(_sent);
        Assert.StartsWith($"{Grobal2Const.RM_ARMREMOVESTONE}|0|4242|0|0|", _sent[0]);
    }

    [Fact]
    public void ThreePacketSenders_UseThreeDistinctPacketIds()
    {
        // ★ 相邻分支互相对照：三个纯发包过程只有包号不同 —— 必须两两不同
        Assert.NotEqual(Grobal2Const.RM_SENDUSERSELL, Grobal2Const.RM_SENDUSERREPAIR);
        Assert.NotEqual(Grobal2Const.RM_SENDUSERREPAIR, Grobal2Const.RM_ARMREMOVESTONE);
        Assert.NotEqual(Grobal2Const.RM_SENDUSERSELL, Grobal2Const.RM_ARMREMOVESTONE);
        Assert.NotEqual(Grobal2Const.RM_SENDUSERSREPAIR, Grobal2Const.RM_SENDUSERREPAIR);
    }

    [Fact]
    public void SellItem_DoesNotSendRepairOrArmRemovePackets()
    {
        // ★ 差异断言：卖 不该发出 修 / 卸装 的包（防"互相抄"）
        var m = Merchant();
        m.SellItem(Player());
        Assert.DoesNotContain(_sent, s => s.StartsWith($"{Grobal2Const.RM_SENDUSERREPAIR}|"));
        Assert.DoesNotContain(_sent, s => s.StartsWith($"{Grobal2Const.RM_ARMREMOVESTONE}|"));
    }

    [Fact]
    public void ArmRemoveStoneItem_DoesNotSendSellOrRepairPackets()
    {
        var m = Merchant();
        m.ArmRemoveStoneItem(Player());
        Assert.DoesNotContain(_sent, s => s.StartsWith($"{Grobal2Const.RM_SENDUSERSELL}|"));
        Assert.DoesNotContain(_sent, s => s.StartsWith($"{Grobal2Const.RM_SENDUSERREPAIR}|"));
    }

    [Fact]
    public void PortedArms_SellFlagOn_SendsSell()
    {
        var m = Merchant();
        m.m_boSell = true;
        Assert.True(m.UserSelectPortedArms(Player(), NpcProcessCmd.nNF_Sell));
        Assert.StartsWith($"{Grobal2Const.RM_SENDUSERSELL}|", Assert.Single(_sent));
    }

    [Fact]
    public void PortedArms_SellFlagOff_SendsNothing()
    {
        var m = Merchant();
        m.m_boSell = false;
        Assert.True(m.UserSelectPortedArms(Player(), NpcProcessCmd.nNF_Sell));
        Assert.Empty(_sent);
    }

    [Fact]
    public void PortedArms_ArmRemoveStoneFlagOn_SendsArmRemoveStone()
    {
        var m = Merchant();
        m.m_boArmRemoveStone = true;
        Assert.True(m.UserSelectPortedArms(Player(), NpcProcessCmd.nNF_ArmRemoveStone));
        Assert.StartsWith($"{Grobal2Const.RM_ARMREMOVESTONE}|", Assert.Single(_sent));
    }

    [Fact]
    public void PortedArms_ArmRemoveStoneFlagOff_SendsNothing()
    {
        var m = Merchant();
        m.m_boArmRemoveStone = false;
        Assert.True(m.UserSelectPortedArms(Player(), NpcProcessCmd.nNF_ArmRemoveStone));
        Assert.Empty(_sent);
    }

    [Fact]
    public void PortedArms_FourGuardsAreIndependent()
    {
        // ★ 四个守卫互不串台：只开 m_boSell 时其余三条分支不得放行
        var m = Merchant();
        m.m_boSell = true;
        m.m_boS_repair = false;
        m.m_boRepair = false;
        m.m_boArmRemoveStone = false;

        Assert.True(m.UserSelectPortedArms(Player(), NpcProcessCmd.nNF_SuperRepair));
        Assert.True(m.UserSelectPortedArms(Player(), NpcProcessCmd.nNF_Repair));
        Assert.True(m.UserSelectPortedArms(Player(), NpcProcessCmd.nNF_ArmRemoveStone));
        Assert.Empty(_sent);                                   // 三条都没发货

        m.UserSelectPortedArms(Player(), NpcProcessCmd.nNF_Sell);
        Assert.Single(_sent);                                  // 只有卖发货
    }

    [Fact]
    public void PortedArms_CommandIdsAreFourDistinctOriginalValues()
    {
        // NpcCommon.pas:26/32/36/70
        Assert.Equal(9, NpcProcessCmd.nNF_SuperRepair);
        Assert.Equal(12, NpcProcessCmd.nNF_Repair);
        Assert.Equal(15, NpcProcessCmd.nNF_Sell);
        Assert.Equal(31, NpcProcessCmd.nNF_ArmRemoveStone);
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
