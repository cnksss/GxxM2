using System;
using System.Collections.Generic;
using GXX.Core.Rtl;
using GXX.SelGate;
using Xunit;

namespace GXX.SelGate.Tests;

/// <summary>Misc.pas:162-177 SendGameCenterMsg 的 WM_COPYDATA 通道桩。</summary>
public sealed class FakeGameCenterChannel : ISelGameCenterChannel
{
    public readonly List<(IntPtr H, int Param, string Msg)> Calls = new();

    public void SendCopyData(IntPtr hWnd, int nParam, string sSendMsg) => Calls.Add((hWnd, nParam, sSendMsg));
}

/// <summary>
/// SelGate Misc.pas → CSelGateMisc 的 1:1 断言。
/// 覆盖：CloseIPConnect（:41-66）、KickUser 两个重载（:68-92、:94-111）、BlockUser（:113-129）、
/// AnsiStrToVal（:139-160）、SendGameCenterMsg（:162-177）、CheckAccountName（:179-209）、
/// 常量表（:10-21）、ReverseIP（:131-137）。
/// </summary>
public class SelGateMiscTests
{
    private static (CConfigMgr cfg, CSelGateIPFilter ipf) Setup(TempDir dir)
    {
        var cfg = new CConfigMgr(dir.File("Config.ini"));
        var ipf = new CSelGateIPFilter { BaseDirectory = dir.Path, Config = cfg };
        return (cfg, ipf);
    }

    // =====================================================================================
    // 1. 常量表
    // =====================================================================================

    [Fact]
    public void Constants_MatchProtocolPasAndMiscPas()
    {
        Assert.Equal(1000, CSelGateMisc.SG_FORMHANDLE);   // Misc.pas:11
        Assert.Equal(1001, CSelGateMisc.SG_STARTNOW);     // :12
        Assert.Equal(1002, CSelGateMisc.SG_STARTOK);      // :13
        Assert.Equal(1003, CSelGateMisc.SG_ACTIVE);       // :14
        Assert.Equal(2000, CSelGateMisc.GS_QUIT);         // :16

        Assert.Equal("网关", SelGateProtocol._STR_GRID_INDEX);
        Assert.Equal("网关地址", SelGateProtocol._STR_GRID_IP);
        Assert.Equal("端口", SelGateProtocol._STR_GRID_PORT);
        Assert.Equal("连接状态", SelGateProtocol._STR_GRID_CONNECT_STATUS);
        Assert.Equal("通讯", SelGateProtocol._STR_GRID_ONLINE_USER);
        Assert.Equal("正在启动角色网关...", SelGateProtocol._STR_NOW_START);
        Assert.Equal("角色网关启动完成...", SelGateProtocol._STR_STARTED);
        Assert.Equal("在线", SelGateProtocol._STR_NOW_STOP);      // 原文如此：名为 NOW_STOP 值却是"在线"
        Assert.Equal(@".\Config.ini", SelGateProtocol._STR_CONFIG_FILE);
        Assert.Equal(@".\BlockIPList.txt", SelGateProtocol._STR_BLOCK_FILE);
        Assert.Equal(@".\BlockIPAreaList.txt", SelGateProtocol._STR_BLOCK_AREA_FILE);
        Assert.Equal(@".\NewChrNameFilter.txt", SelGateProtocol._STR_USER_NAME_FILTER_FILE);

        // Protocol.pas:25-29：_IDM_TIMER_* = WM_USER(1024) + 1000 + n
        Assert.Equal(2024, SelGateProtocol._IDM_SERVERSOCK_MSG);
        Assert.Equal(2025, SelGateProtocol._IDM_TIMER_STARTSERVICE);
        Assert.Equal(2026, SelGateProtocol._IDM_TIMER_STOPSERVICE);
        Assert.Equal(2027, SelGateProtocol._IDM_TIMER_KEEP_ALIVE);
        Assert.Equal(2028, SelGateProtocol._IDM_TIMER_THREAD_INFO);

        // Protocol.pas:32 FIRST_PAKCET_MAX_LEN = 0080（八进制）= 64（原文如此）
        Assert.Equal(64, SelGateProtocol.FIRST_PAKCET_MAX_LEN);
        Assert.Equal(1024, SelGateProtocol.MAX_FUNC_COUNT);
        Assert.Equal(16 * 1024, SelGateProtocol.MAX_SERVER_FUNC_SIZE);
        Assert.Equal(16 * 1024, SelGateProtocol.MAX_CLIENT_FUNC_SIZE);
    }

    [Fact]
    public void TProgamType_SelGateOrdinalsMatchDelphiDeclarationOrder()
    {
        // Misc.pas:19-21 声明顺序；SendGameCenterMsg 依赖 tSelGate=6 / tSelGate1=7
        Assert.Equal(6, (int)CSelGateMisc.TProgamType.tSelGate);
        Assert.Equal(7, (int)CSelGateMisc.TProgamType.tSelGate1);
        Assert.Equal(0, (int)CSelGateMisc.TProgamType.tDBServer);
        Assert.Equal(15, (int)CSelGateMisc.TProgamType.tRunGate7);
    }

    // =====================================================================================
    // 2. KickUser(Integer)：三条分支 + 总开关
    // =====================================================================================

    [Fact]
    public void KickUser_ByIP_WhenKickOverPacketSizeOff_ReturnsTrueAndDoesNothing()
    {
        using var dir = new TempDir();
        var (cfg, ipf) = Setup(dir);
        cfg.m_fKickOverPacketSize = false;                          // :71
        cfg.m_tBlockIPMethod = (int)TBlockIPMethod.mBlock;
        var users = new List<ISelSessionHost?>();

        Assert.True(CSelGateMisc.KickUser(1234, cfg, ipf, users, (_, _) => { }, _ => { }));
        Assert.Empty(ipf.g_TempBlockIPList.AsEnumerable());
        Assert.Empty(ipf.g_BlockIPList.AsEnumerable());
    }

    [Fact]
    public void KickUser_ByIP_DisconnectMethod_ReturnsFalseWithoutBlocking()
    {
        using var dir = new TempDir();
        var (cfg, ipf) = Setup(dir);
        cfg.m_fKickOverPacketSize = true;
        cfg.m_tBlockIPMethod = (int)TBlockIPMethod.mDisconnect;

        Assert.False(CSelGateMisc.KickUser(1234, cfg, ipf, new List<ISelSessionHost?>(), (_, _) => { }, _ => { }));
        Assert.Empty(ipf.g_TempBlockIPList.AsEnumerable());          // :74-77 不做任何封禁
        Assert.Empty(ipf.g_BlockIPList.AsEnumerable());
    }

    [Fact]
    public void KickUser_ByIP_TempBlockMethod_AddsToTempListAndClosesConnections()
    {
        using var dir = new TempDir();
        var (cfg, ipf) = Setup(dir);
        cfg.m_fKickOverPacketSize = true;
        cfg.m_tBlockIPMethod = (int)TBlockIPMethod.mBlock;

        int ip = CSelGateIPFilter.InetAddr("4.4.4.4");
        var host = new FakeSessionHost { IPAddr = ip, Active = true, HandleLogin = 0 };
        var users = new List<ISelSessionHost?> { host };
        SelGateGlobals.g_fServiceStarted = true;
        try
        {
            Assert.False(CSelGateMisc.KickUser(ip, cfg, ipf, users, (h, _) => ((FakeSessionHost)h).SendOutOfConnectionCalls++, h => ((FakeSessionHost)h).FreeSocketCalls++));
        }
        finally { SelGateGlobals.g_fServiceStarted = false; }

        Assert.Equal(new[] { "4.4.4.4" }, ipf.g_TempBlockIPList.AsEnumerable()); // :80
        Assert.Equal(1, host.FreeSocketCalls);                                   // HandleLogin < 2 ⇒ FreeSocket（:63）
        Assert.Equal(0, host.SendOutOfConnectionCalls);
    }

    [Fact]
    public void KickUser_ByIP_PermanentBlockMethod_AddsToBlockList()
    {
        using var dir = new TempDir();
        var (cfg, ipf) = Setup(dir);
        cfg.m_fKickOverPacketSize = true;
        cfg.m_tBlockIPMethod = (int)TBlockIPMethod.mBlockList;

        int ip = CSelGateIPFilter.InetAddr("5.5.5.5");
        Assert.False(CSelGateMisc.KickUser(ip, cfg, ipf, new List<ISelSessionHost?>(), (_, _) => { }, _ => { }));
        Assert.Equal(new[] { "5.5.5.5" }, ipf.g_BlockIPList.AsEnumerable());     // :86
        Assert.Empty(ipf.g_TempBlockIPList.AsEnumerable());
    }

    // =====================================================================================
    // 3. KickUser(TSessionObj)：先 FreeSocket 再置标志，且 mDisconnect 分支缺失
    // =====================================================================================

    [Fact]
    public void KickUser_BySession_FreeSocketThenKickFlag_AndBlocksByMethod()
    {
        using var dir = new TempDir();
        var (cfg, ipf) = Setup(dir);
        cfg.m_fKickOverPacketSize = true;
        cfg.m_tBlockIPMethod = (int)TBlockIPMethod.mBlock;

        var host = new FakeSessionHost { IPAddr = CSelGateIPFilter.InetAddr("6.6.6.6") };
        CSelGateMisc.KickUser(host, cfg, ipf, h => ((FakeSessionHost)h).FreeSocketCalls++);

        Assert.Equal(1, host.FreeSocketCalls);                                   // :98
        Assert.True(host.KickFlag);                                              // :99
        Assert.Equal(new[] { "6.6.6.6" }, ipf.g_TempBlockIPList.AsEnumerable()); // :103
    }

    [Fact]
    public void KickUser_BySession_DisconnectMethod_StillFreesSocketAndSetsFlag()
    {
        using var dir = new TempDir();
        var (cfg, ipf) = Setup(dir);
        cfg.m_fKickOverPacketSize = true;
        cfg.m_tBlockIPMethod = (int)TBlockIPMethod.mDisconnect;

        var host = new FakeSessionHost { IPAddr = 7 };
        CSelGateMisc.KickUser(host, cfg, ipf, h => ((FakeSessionHost)h).FreeSocketCalls++);

        Assert.Equal(1, host.FreeSocketCalls);       // 与 KickUser(Integer) 的 mDisconnect 分支不同
        Assert.True(host.KickFlag);
        Assert.Empty(ipf.g_TempBlockIPList.AsEnumerable());
        Assert.Empty(ipf.g_BlockIPList.AsEnumerable());
    }

    [Fact]
    public void KickUser_BySession_WhenSwitchOff_IsANoOp()
    {
        using var dir = new TempDir();
        var (cfg, ipf) = Setup(dir);
        cfg.m_fKickOverPacketSize = false;
        var host = new FakeSessionHost { IPAddr = 8 };
        CSelGateMisc.KickUser(host, cfg, ipf, h => ((FakeSessionHost)h).FreeSocketCalls++);
        Assert.Equal(0, host.FreeSocketCalls);
        Assert.False(host.KickFlag);
    }

    // =====================================================================================
    // 4. BlockUser：不 FreeSocket
    // =====================================================================================

    [Fact]
    public void BlockUser_SetsFlagAndBlocksButNeverFreesSocket()
    {
        using var dir = new TempDir();
        var (cfg, ipf) = Setup(dir);
        cfg.m_fKickOverPacketSize = true;
        cfg.m_tBlockIPMethod = (int)TBlockIPMethod.mBlockList;

        var host = new FakeSessionHost { IPAddr = CSelGateIPFilter.InetAddr("8.8.8.8") };
        CSelGateMisc.BlockUser(host, cfg, ipf);

        Assert.True(host.KickFlag);                                              // :117
        Assert.Equal(0, host.FreeSocketCalls);                                   // 原文不 FreeSocket
        Assert.Equal(new[] { "8.8.8.8" }, ipf.g_BlockIPList.AsEnumerable());     // :125
    }

    [Fact]
    public void BlockUser_WhenSwitchOff_IsANoOp()
    {
        using var dir = new TempDir();
        var (cfg, ipf) = Setup(dir);
        cfg.m_fKickOverPacketSize = false;
        var host = new FakeSessionHost { IPAddr = 9 };
        CSelGateMisc.BlockUser(host, cfg, ipf);
        Assert.False(host.KickFlag);
        Assert.Empty(ipf.g_BlockIPList.AsEnumerable());
    }

    [Fact]
    public void BlockUser_DisconnectMethod_OnlySetsFlag()
    {
        using var dir = new TempDir();
        var (cfg, ipf) = Setup(dir);
        cfg.m_fKickOverPacketSize = true;
        cfg.m_tBlockIPMethod = (int)TBlockIPMethod.mDisconnect;
        var host = new FakeSessionHost { IPAddr = 10 };
        CSelGateMisc.BlockUser(host, cfg, ipf);
        Assert.True(host.KickFlag);
        Assert.Empty(ipf.g_TempBlockIPList.AsEnumerable());
        Assert.Empty(ipf.g_BlockIPList.AsEnumerable());
    }

    // =====================================================================================
    // 5. CloseIPConnect
    // =====================================================================================

    [Fact]
    public void CloseIPConnect_RequiresServiceStarted()
    {
        var host = new FakeSessionHost { IPAddr = 1, Active = true, HandleLogin = 0 };
        var users = new List<ISelSessionHost?> { host };
        SelGateGlobals.g_fServiceStarted = false;
        CSelGateMisc.CloseIPConnect(1, users, (h, _) => ((FakeSessionHost)h).SendOutOfConnectionCalls++, h => ((FakeSessionHost)h).FreeSocketCalls++);
        Assert.Equal(0, host.FreeSocketCalls);
    }

    [Fact]
    public void CloseIPConnect_HandleLoginAtLeastTwo_SendsOutOfConnectionInsteadOfFreeSocket()
    {
        var host = new FakeSessionHost { IPAddr = 1, Active = true, HandleLogin = 2, SvrObject = 77 };
        var users = new List<ISelSessionHost?> { host };
        int sentSvrObject = -1;
        SelGateGlobals.g_fServiceStarted = true;
        try
        {
            CSelGateMisc.CloseIPConnect(1, users,
                (h, svr) => { ((FakeSessionHost)h).SendOutOfConnectionCalls++; sentSvrObject = svr; },
                h => ((FakeSessionHost)h).FreeSocketCalls++);
        }
        finally { SelGateGlobals.g_fServiceStarted = false; }

        Assert.Equal(1, host.SendOutOfConnectionCalls);      // :58 SM_OUTOFCONNECTION
        Assert.Equal(77, sentSvrObject);                     // UserObj.m_nSvrObject
        Assert.Equal(0, host.FreeSocketCalls);
        Assert.True(host.KickFlag);                          // :60
    }

    [Fact]
    public void CloseIPConnect_OnlyTouchesMatchingActiveSessions()
    {
        var a = new FakeSessionHost { IPAddr = 1, Active = true, HandleLogin = 0 };
        var b = new FakeSessionHost { IPAddr = 2, Active = true, HandleLogin = 0 };
        var c = new FakeSessionHost { IPAddr = 1, Active = false, HandleLogin = 0 };
        var users = new List<ISelSessionHost?> { a, null, b, c };
        SelGateGlobals.g_fServiceStarted = true;
        try
        {
            CSelGateMisc.CloseIPConnect(1, users, (_, _) => { }, h => ((FakeSessionHost)h).FreeSocketCalls++);
        }
        finally { SelGateGlobals.g_fServiceStarted = false; }

        Assert.Equal(1, a.FreeSocketCalls);
        Assert.Equal(0, b.FreeSocketCalls);
        Assert.Equal(0, c.FreeSocketCalls);   // m_tLastGameSvr.Active = False
    }

    // =====================================================================================
    // 6. AnsiStrToVal
    // =====================================================================================

    [Fact]
    public void AnsiStrToVal_ParsesLeadingDigitsAndReportsConsumedCount()
    {
        Assert.Equal(1234, CSelGateMisc.AnsiStrToVal("1234", out int pos));
        Assert.Equal(4, pos);
    }

    [Fact]
    public void AnsiStrToVal_StopsAtFirstNonDigit()
    {
        Assert.Equal(12, CSelGateMisc.AnsiStrToVal("12abc", out int pos));
        Assert.Equal(2, pos);
    }

    [Fact]
    public void AnsiStrToVal_NullOrNoDigits_ReturnsZero()
    {
        Assert.Equal(0, CSelGateMisc.AnsiStrToVal(null, out int p1));
        Assert.Equal(0, p1);
        Assert.Equal(0, CSelGateMisc.AnsiStrToVal("", out int p2));
        Assert.Equal(0, p2);
        Assert.Equal(0, CSelGateMisc.AnsiStrToVal("abc", out int p3));
        Assert.Equal(0, p3);
        Assert.Equal(0, CSelGateMisc.AnsiStrToVal("-5", out int p4)); // '-' 不是数字 ⇒ 立即停
        Assert.Equal(0, p4);
    }

    [Fact]
    public void AnsiStrToVal_LeadingZeroesAreKeptInNumericValue()
    {
        Assert.Equal(7, CSelGateMisc.AnsiStrToVal("0007x", out int pos));
        Assert.Equal(4, pos);
    }

    // =====================================================================================
    // 7. SendGameCenterMsg
    // =====================================================================================

    [Fact]
    public void SendGameCenterMsg_UsesSelGateProgramTypeWhenNotNetComGate()
    {
        var ch = new FakeGameCenterChannel();
        SelGateGlobals.g_boNetComGate = false;
        SelGateGlobals.g_hGameCenterHandle = new IntPtr(0x1234);
        try
        {
            CSelGateMisc.SendGameCenterMsg((ushort)CSelGateMisc.SG_STARTNOW, "正在启动角色网关...", ch);
        }
        finally { SelGateGlobals.g_hGameCenterHandle = IntPtr.Zero; }

        Assert.Single(ch.Calls);
        Assert.Equal(new IntPtr(0x1234), ch.Calls[0].H);
        // nParam = MakeLong(Word(tSelGate), wIdent) = wIdent << 16 | 6
        Assert.Equal(DelphiRTL.MakeLong(6, CSelGateMisc.SG_STARTNOW), ch.Calls[0].Param);
        Assert.Equal("正在启动角色网关...", ch.Calls[0].Msg);
    }

    [Fact]
    public void SendGameCenterMsg_UsesSelGate1ProgramTypeWhenNetComGate()
    {
        var ch = new FakeGameCenterChannel();
        SelGateGlobals.g_boNetComGate = true;
        try
        {
            CSelGateMisc.SendGameCenterMsg((ushort)CSelGateMisc.SG_STARTOK, "x", ch);
        }
        finally { SelGateGlobals.g_boNetComGate = false; }

        Assert.Equal(DelphiRTL.MakeLong(7, CSelGateMisc.SG_STARTOK), ch.Calls[0].Param); // tSelGate1 = 7
    }

    // =====================================================================================
    // 8. CheckAccountName（'0'..'z' 宽松白名单 + GBK 双字节回退）
    // =====================================================================================

    [Theory]
    [InlineData("abc", true)]
    [InlineData("ABC", true)]      // 'A'(65) 在 '0'(48)..'z'(122) 内 ⇒ 放行（原文范围极宽松）
    [InlineData("a1_", true)]      // '_'(95) 仍在 '0'..'z' 内 ⇒ 放行
    [InlineData("abc123", true)]
    [InlineData("~", false)]       // '~'(126) > 'z'(122) ⇒ 越界，且非 GBK 前导 ⇒ False
    [InlineData("a~", false)]
    public void CheckAccountName_RangeIsWideAndCounterIntuitive(string name, bool expected)
    {
        Assert.Equal(expected, CSelGateMisc.CheckAccountName(name));
    }

    [Fact]
    public void CheckAccountName_AsciiRangeBoundaries_AreInclusive()
    {
        Assert.True(CSelGateMisc.CheckAccountName("0"));    // :194 下界（'0' 不小于 '0'）
        Assert.True(CSelGateMisc.CheckAccountName("z"));    // :194 上界（'z' 不大于 'z'）
        Assert.False(CSelGateMisc.CheckAccountName("\u007B")); // '{'(123) 已越界
        Assert.False(CSelGateMisc.CheckAccountName("/"));   // '/'(47) < '0'
    }

    [Fact]
    public void CheckAccountName_EmptyIsFalse_AndAsciiControlIsFalse()
    {
        Assert.False(CSelGateMisc.CheckAccountName(""));            // :185-186
        Assert.False(CSelGateMisc.CheckAccountName("\u0001"));      // 控制字符 < '0'
        Assert.False(CSelGateMisc.CheckAccountName(" "));           // 空格(32) < '0'(48)
    }

    [Fact]
    public void CheckAccountName_GbkTwoByteSequenceIsAccepted()
    {
        // 0xB0..0xC8 前导 + 0xA1..0xFE 尾字节 ⇒ 命中 :197-203 的 GBK 分支
        string gbk = "\u00B0\u00A1";
        Assert.True(CSelGateMisc.CheckAccountName(gbk));
    }

    [Fact]
    public void CheckAccountName_GbkLeadByteWithoutValidTailIsFalse()
    {
        string bad = "\u00B0\u0041";   // 前导合法但尾字节 'A'(0x41) 不在 0xA1..0xFE
        Assert.False(CSelGateMisc.CheckAccountName(bad));
    }
}
