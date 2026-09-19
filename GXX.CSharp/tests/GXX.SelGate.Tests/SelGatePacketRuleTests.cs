using System;
using System.Collections.Generic;
using GXX.Core.Util;
using GXX.SelGate;
using Xunit;

namespace GXX.SelGate.Tests;

/// <summary>
/// PacketRuleConfig.pas（封包规则/黑白名单配置窗体）的纯逻辑部分 → CSelPacketRule 的 1:1 断言。
/// 覆盖：IP 段解析与校验（:254-324 / :376-454）、Int64 视图查重（:307-313）、
/// 列表搬迁（:168-188、:492-512、:769-798、:1013-1043）、删除缺陷（:800-825、:1045-1071）、
/// 活跃连接加入黑名单（:598-668）、刷新活跃列表（:190-206）、UI 值映射（:555-580）。
/// </summary>
public class SelGatePacketRuleTests
{
    private static TIPArea LastArea(IReadOnlyList<object> objs) => (TIPArea)objs[objs.Count - 1];

    // =====================================================================================
    // 1. IP 段解析
    // =====================================================================================

    [Fact]
    public void TryParseIPArea_TypicalRange_ProducesReverseIPBounds()
    {
        Assert.True(CSelPacketRule.TryParseIPArea("192.168.1.1-192.168.1.255", out TIPArea area,
            out bool hasDash, out bool lowBad, out bool highBad));

        Assert.True(hasDash);
        Assert.False(lowBad);
        Assert.False(highBad);
        Assert.Equal(CSelGateIPFilter.ReverseIP(unchecked((uint)CSelGateIPFilter.InetAddr("192.168.1.1"))), area.Low);
        Assert.Equal(CSelGateIPFilter.ReverseIP(unchecked((uint)CSelGateIPFilter.InetAddr("192.168.1.255"))), area.High);
        Assert.True(area.Low < area.High);
    }

    [Fact]
    public void TryParseIPArea_MissingDash_FailsWithoutFlags()
    {
        // :283 if Pos('-', szIPArea) = 0 ⇒ MessageBox『输入格式错误，正确格式如下：192.168.1.1-192.168.1.255』
        Assert.False(CSelPacketRule.TryParseIPArea("192.168.1.1", out _, out bool hasDash, out bool lowBad, out bool highBad));
        Assert.False(hasDash);
        Assert.False(lowBad);
        Assert.False(highBad);
    }

    [Fact]
    public void TryParseIPArea_EmptyString_Fails()
    {
        Assert.False(CSelPacketRule.TryParseIPArea("", out _, out bool hasDash, out _, out _));
        Assert.False(hasDash);
    }

    [Fact]
    public void TryParseIPArea_BadLowEnd_SetsLowBad()
    {
        Assert.False(CSelPacketRule.TryParseIPArea("999.1.1.1-1.1.1.1", out _, out _, out bool lowBad, out bool highBad));
        Assert.True(lowBad);     // :293-297 『输入的低位IP格式错误』
        Assert.False(highBad);
    }

    [Fact]
    public void TryParseIPArea_BadHighEnd_SetsHighBad()
    {
        Assert.False(CSelPacketRule.TryParseIPArea("1.1.1.1-999.1.1.1", out _, out _, out bool lowBad, out bool highBad));
        Assert.False(lowBad);
        Assert.True(highBad);    // :298-302 『输入的高位IP格式错误』
    }

    [Fact]
    public void TryParseIPArea_SwapsWhenLowAboveHigh()
    {
        Assert.True(CSelPacketRule.TryParseIPArea("10.0.0.255-10.0.0.1", out TIPArea area, out _, out _, out _));
        Assert.True(area.Low < area.High);                                    // :300-305 交换
        Assert.Equal(CSelGateIPFilter.ReverseIP(unchecked((uint)CSelGateIPFilter.InetAddr("10.0.0.1"))), area.Low);
        Assert.Equal(CSelGateIPFilter.ReverseIP(unchecked((uint)CSelGateIPFilter.InetAddr("10.0.0.255"))), area.High);
    }

    [Fact]
    public void TryParseIPArea_SameLowAndHigh_IsAccepted()
    {
        Assert.True(CSelPacketRule.TryParseIPArea("5.5.5.5-5.5.5.5", out TIPArea area, out _, out _, out _));
        Assert.Equal(area.Low, area.High);
    }

    [Fact]
    public void TryParseIPArea_TrailingContentAfterSecondDash_IsIgnoredLikeGetValidStr3()
    {
        // GetValidStr3 只按第一个 '-' 切分，剩余段落整体作为"高位"字符串 ⇒ 含 '-' 时 inet_addr 失败
        Assert.False(CSelPacketRule.TryParseIPArea("1.1.1.1-2.2.2.2-3.3.3.3", out _, out _, out bool lowBad, out bool highBad));
        Assert.False(lowBad);
        Assert.True(highBad);
    }

    [Fact]
    public void TryParseIPArea_ShorthandNotationAcceptedByInetAddr()
    {
        // inet_addr 接受 a.b.c 简写，故 "10.1.2-10.1.3" 合法（原文行为，非笔误）
        Assert.True(CSelPacketRule.TryParseIPArea("10.1.2-10.1.3", out TIPArea area, out _, out _, out _));
        Assert.Equal(CSelGateIPFilter.ReverseIP(unchecked((uint)CSelGateIPFilter.InetAddr("10.1.2"))), area.Low);
    }

    // =====================================================================================
    // 2. 查重（Int64 内存视图）
    // =====================================================================================

    [Fact]
    public void CombineToInt64_PacksLowInLowWordHighInHighWord()
    {
        var a = new TIPArea { Low = 0x03020100u, High = 0x07060504u };
        Assert.Equal(0x0706050403020100L, CSelPacketRule.CombineToInt64(a));
        Assert.Equal(CSelPacketRule.CombineToInt64(a), CSelPacketRule.CombineToInt64(a));
    }

    [Fact]
    public void SameAreaKey_RequiresBothBoundsEqual()
    {
        var a = new TIPArea { Low = 1, High = 2 };
        Assert.True(CSelPacketRule.SameAreaKey(a, new TIPArea { Low = 1, High = 2 }));
        Assert.False(CSelPacketRule.SameAreaKey(a, new TIPArea { Low = 1, High = 3 }));
        Assert.False(CSelPacketRule.SameAreaKey(a, new TIPArea { Low = 0, High = 2 }));
        // 跨字段别名的 Collision（Low=2,High=1 vs Low=1,High=2 在 64 位视图下不同，但 Low/High 交换后相同）
        Assert.False(CSelPacketRule.SameAreaKey(new TIPArea { Low = 2, High = 1 }, new TIPArea { Low = 1, High = 2 }));
    }

    [Fact]
    public void ContainsArea_FindsExistingEntry()
    {
        var objs = new List<object>
        {
            new TIPArea { Low = 1, High = 2 },
            new TIPArea { Low = 3, High = 4 },
        };
        Assert.True(CSelPacketRule.ContainsArea(objs, new TIPArea { Low = 3, High = 4 }));
        Assert.False(CSelPacketRule.ContainsArea(objs, new TIPArea { Low = 3, High = 5 }));
        Assert.False(CSelPacketRule.ContainsArea(new List<object>(), new TIPArea { Low = 1, High = 2 }));
    }

    // =====================================================================================
    // 3. 列表搬迁
    // =====================================================================================

    [Fact]
    public void AllBlockToTempList_MovesAllAndDedupsByText()
    {
        var blk = new TStringList();
        var tmp = new TStringList();
        blk.AddObject("1.1.1.1", 1);
        blk.AddObject("2.2.2.2", 2);
        tmp.AddObject("2.2.2.2", 99);      // 已存在 ⇒ 不覆盖
        tmp.AddObject("3.3.3.3", 3);

        CSelPacketRule.AllBlockToTempList(blk, tmp);   // :168-188

        Assert.Empty(blk.AsEnumerable());               // g_BlockIPList.Clear()
        Assert.Equal(new[] { "2.2.2.2", "3.3.3.3", "1.1.1.1" }, tmp.AsEnumerable());
        Assert.Equal(99, (int)tmp.GetObject(0));        // 原有 objects 保留
        Assert.Equal(1, (int)tmp.GetObject(2));         // 搬来的保留原 objects
    }

    [Fact]
    public void AllTempToBlockList_MovesAllAndDedupsByText()
    {
        var blk = new TStringList();
        var tmp = new TStringList();
        blk.AddObject("9.9.9.9", 9);
        tmp.AddObject("9.9.9.9", 90);
        tmp.AddObject("8.8.8.8", 8);

        CSelPacketRule.AllTempToBlockList(blk, tmp);   // :492-512

        Assert.Empty(tmp.AsEnumerable());
        Assert.Equal(new[] { "9.9.9.9", "8.8.8.8" }, blk.AsEnumerable());
        Assert.Equal(9, (int)blk.GetObject(0));        // 已存在项不被覆盖
        Assert.Equal(8, (int)blk.GetObject(1));
    }

    [Fact]
    public void AllBlockToTempList_EmptySourceIsANoOp()
    {
        var blk = new TStringList();
        var tmp = new TStringList();
        tmp.Add("keep");
        CSelPacketRule.AllBlockToTempList(blk, tmp);
        Assert.Equal(new[] { "keep" }, tmp.AsEnumerable());
    }

    [Fact]
    public void TempToBlockList_MovesSingleEntryAndRemovesItFromSource()
    {
        var blk = new TStringList();
        var tmp = new TStringList();
        tmp.AddObject("1.1.1.1", 11);
        tmp.AddObject("2.2.2.2", 22);

        CSelPacketRule.TempToBlockList(blk, tmp, 1);   // :769-798

        Assert.Equal(new[] { "1.1.1.1" }, tmp.AsEnumerable());
        Assert.Equal(new[] { "2.2.2.2" }, blk.AsEnumerable());
        Assert.Equal(22, (int)blk.GetObject(0));
    }

    [Fact]
    public void TempToBlockList_OutOfRangeIndexIsIgnored()
    {
        var blk = new TStringList();
        var tmp = new TStringList();
        tmp.Add("1.1.1.1");
        CSelPacketRule.TempToBlockList(blk, tmp, -1);
        CSelPacketRule.TempToBlockList(blk, tmp, 5);
        Assert.Single(tmp.AsEnumerable());
        Assert.Empty(blk.AsEnumerable());
    }

    [Fact]
    public void BlockToTempList_MovesSingleEntryAndRemovesItFromSource()
    {
        var blk = new TStringList();
        var tmp = new TStringList();
        blk.AddObject("1.1.1.1", 11);
        blk.AddObject("2.2.2.2", 22);

        CSelPacketRule.BlockToTempList(blk, tmp, 0);   // :1013-1043

        Assert.Equal(new[] { "2.2.2.2" }, blk.AsEnumerable());
        Assert.Equal(new[] { "1.1.1.1" }, tmp.AsEnumerable());
        Assert.Equal(11, (int)tmp.GetObject(0));
    }

    // =====================================================================================
    // 4. 删除（原文缺陷照抄）
    // =====================================================================================

    [Fact]
    public void DeleteTempListItem_RemovesEntryAndReturnsTrue()
    {
        var tmp = new TStringList();
        tmp.Add("1.1.1.1");
        tmp.Add("2.2.2.2");

        bool deleted = CSelPacketRule.DeleteTempListItem(tmp, 0);   // :800-825

        Assert.True(deleted);
        Assert.Equal(new[] { "2.2.2.2" }, tmp.AsEnumerable());
    }

    [Fact]
    public void DeleteTempListItem_OutOfRangeIndexReturnsFalse()
    {
        var tmp = new TStringList();
        tmp.Add("1.1.1.1");
        Assert.False(CSelPacketRule.DeleteTempListItem(tmp, -1));
        Assert.False(CSelPacketRule.DeleteTempListItem(tmp, 1));
        Assert.Single(tmp.AsEnumerable());
    }

    [Fact]
    public void DeleteTempListItem_DuplicateTexts_RemovesAllMatchingBecauseScanBlockStillRuns()
    {
        // 原文 :808 删同下标项后**未 return**，:810 的扫描块继续执行并命中剩下那条 ⇒ 重复项被全部删除
        var tmp = new TStringList();
        tmp.Add("dup");
        tmp.Add("dup");

        Assert.True(CSelPacketRule.DeleteTempListItem(tmp, 0));
        Assert.Empty(tmp.AsEnumerable());
    }

    [Fact]
    public void DeleteBlockListItem_RemovesEntryAndReturnsTrue()
    {
        var blk = new TStringList();
        blk.Add("1.1.1.1");
        blk.Add("2.2.2.2");
        Assert.True(CSelPacketRule.DeleteBlockListItem(blk, 1));      // :1045-1071
        Assert.Equal(new[] { "1.1.1.1" }, blk.AsEnumerable());
        Assert.False(CSelPacketRule.DeleteBlockListItem(blk, 9));
    }

    [Fact]
    public void DeleteBlockListItem_DuplicateTexts_LeavesTheShiftedTailEntry()
    {
        // 原文 :1053 删除后由 UI 层 `ListBoxBlockList.Items.Delete(ItemIndex)` 删列表项；
        // 复刻只覆盖 g_BlockIPList：命中同下标时删掉一条即结束（:1053-1055 无后置扫描块），
        // 因此重复文本会残留一条（与 DeleteTempListItem 的"后置扫描块"结构不同）。
        var blk = new TStringList();
        blk.Add("dup");
        blk.Add("dup");
        Assert.True(CSelPacketRule.DeleteBlockListItem(blk, 0));
        Assert.Equal(new[] { "dup" }, blk.AsEnumerable());
    }

    // =====================================================================================
    // 5. 活跃连接加入黑名单 / 刷新活跃列表
    // =====================================================================================

    [Fact]
    public void AddActiveIPToBlockList_ParsesListBoxTextAndClosesConnections()
    {
        var list = new TStringList();
        var closed = new List<int>();

        Assert.True(CSelPacketRule.AddActiveIPToBlockList(list, "1.2.3.4", closed.Add));  // :598-632
        Assert.Equal(new[] { "1.2.3.4" }, list.AsEnumerable());
        Assert.Equal(CSelGateIPFilter.InetAddr("1.2.3.4"), (int)list.GetObject(0));
        Assert.Equal(new[] { CSelGateIPFilter.InetAddr("1.2.3.4") }, closed);
    }

    [Fact]
    public void AddActiveIPToBlockList_TextWithCharacterName_FallsBackToSecondToken()
    {
        // ListBox 项形如 "<IP> <角色名>"：GetValidStr3(' ') 取第一段；若为空或 Char(15) 则回退到第二段
        var list = new TStringList();
        // ListBox 项形如 "<IP> <角色名>"：GetValidStr3(' ') 用第一段作 szChrName，余下部分作 szIPaddr
        var parsed = new SelPacketRuleActiveProbe("1.2.3.4 测试角色");
        bool ok = CSelPacketRule.AddActiveIPToBlockList(list, "1.2.3.4 测试角色", _ => { });
        Assert.True(ok && list.Count == 1,
            $"ok={ok} count={list.Count} first=[{parsed.First}] rest=[{parsed.Rest}] ip=[{parsed.IpText}]");
        Assert.Equal("1.2.3.4", list[0]);
        Assert.Equal(CSelGateIPFilter.InetAddr("1.2.3.4"), (int)list.GetObject(0));
    }

    [Fact]
    public void AddActiveIPToBlockList_Char15Placeholder_UsesCharacterName()
    {
        var list = new TStringList();
        var closed = new List<int>();

        // 首段为 Char(15) ⇒ :606-607 szIPaddr := szChrName（第二段）
        Assert.True(CSelPacketRule.AddActiveIPToBlockList(list, "\u000F 5.6.7.8", closed.Add));
        Assert.Equal("5.6.7.8", list[0]);
    }

    [Fact]
    public void AddActiveIPToBlockList_InvalidAddress_DoesNothing()
    {
        var list = new TStringList();
        var closed = new List<int>();
        Assert.False(CSelPacketRule.AddActiveIPToBlockList(list, "not-an-ip", closed.Add));
        Assert.Empty(list.AsEnumerable());
        Assert.Empty(closed);
    }

    [Fact]
    public void AddActiveIPToBlockList_DedupsByStoredInteger_ButStillClosesConnections()
    {
        var list = new TStringList();
        var closed = new List<int>();
        CSelPacketRule.AddActiveIPToBlockList(list, "1.2.3.4", closed.Add);
        Assert.True(CSelPacketRule.AddActiveIPToBlockList(list, "1.2.3.4", closed.Add)); // :621 已存在 ⇒ 不重复加
        Assert.Single(list.AsEnumerable());
        Assert.Equal(2, closed.Count);   // :627 CloseIPConnect 仍然执行
    }

    [Fact]
    public void RefreshActiveList_RequiresServiceStarted()
    {
        var users = new List<ISelSessionHost?> { new FakeSessionHost { IPText = "1.1.1.1", Active = true } };
        SelGateGlobals.g_fServiceStarted = false;
        try
        {
            Assert.Empty(CSelPacketRule.RefreshActiveList(users));      // :194 未启动 ⇒ 空
            SelGateGlobals.g_fServiceStarted = true;
            Assert.Equal(new[] { "1.1.1.1" }, CSelPacketRule.RefreshActiveList(users));
        }
        finally { SelGateGlobals.g_fServiceStarted = false; }
    }

    [Fact]
    public void RefreshActiveList_SkipsNullInactiveAndKickedSessions_AndTrims()
    {
        var users = new List<ISelSessionHost?>
        {
            null,                                                                       // :197 m_tLastGameSvr <> nil 前提
            new FakeSessionHost { IPText = " 1.1.1.1 ", Active = true },                 // Trim
            new FakeSessionHost { IPText = "2.2.2.2", Active = false },                  // not Active
            new FakeSessionHost { IPText = "3.3.3.3", Active = true, KickFlag = true },  // m_fKickFlag
        };
        SelGateGlobals.g_fServiceStarted = true;
        try
        {
            Assert.Equal(new[] { "1.1.1.1" }, CSelPacketRule.RefreshActiveList(users));
        }
        finally { SelGateGlobals.g_fServiceStarted = false; }
    }

    // =====================================================================================
    // 6. UI 值映射
    // =====================================================================================

    [Fact]
    public void SetBlockIPMethod_MapsThreeRadioButtons()
    {
        using var dir = new TempDir();
        var cfg = new CConfigMgr(dir.File("Config.ini"));

        CSelPacketRule.SetBlockIPMethod(cfg, 0);   // rdDisConnect
        Assert.Equal((int)TBlockIPMethod.mDisconnect, cfg.m_tBlockIPMethod);
        CSelPacketRule.SetBlockIPMethod(cfg, 1);   // rdAddBlockList
        Assert.Equal((int)TBlockIPMethod.mBlockList, cfg.m_tBlockIPMethod);
        CSelPacketRule.SetBlockIPMethod(cfg, 2);   // rdAddTempList
        Assert.Equal((int)TBlockIPMethod.mBlock, cfg.m_tBlockIPMethod);
        CSelPacketRule.SetBlockIPMethod(cfg, 77);  // 未知 Tag ⇒ 不改
        Assert.Equal((int)TBlockIPMethod.mBlock, cfg.m_tBlockIPMethod);
    }

    [Fact]
    public void ApplySpinEdit_MapsTags20To24_Skipping23()
    {
        using var dir = new TempDir();
        var cfg = new CConfigMgr(dir.File("Config.ini"));

        CSelPacketRule.ApplySpinEdit(cfg, 20, 33);
        Assert.Equal(33, cfg.m_nMaxConnectOfIP);          // :560
        CSelPacketRule.ApplySpinEdit(cfg, 21, 45);
        Assert.Equal(45 * 1000, cfg.m_nClientTimeOutTime); // :561 Value * 1000
        CSelPacketRule.ApplySpinEdit(cfg, 22, 900);
        Assert.Equal(900, cfg.m_nNomClientPacketSize);    // :562
        CSelPacketRule.ApplySpinEdit(cfg, 24, 7);
        Assert.Equal(7, cfg.m_nMaxClientPacketCount);     // :563

        int before = cfg.m_nMaxClientPacketCount;
        CSelPacketRule.ApplySpinEdit(cfg, 23, 999);       // Tag 23 原文没有分支
        Assert.Equal(before, cfg.m_nMaxClientPacketCount);
    }
}
