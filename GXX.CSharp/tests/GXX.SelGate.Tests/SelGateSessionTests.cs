using System;
using System.Collections.Generic;
using System.Text;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.SelGate;
using Xunit;

namespace GXX.SelGate.Tests;

/// <summary>
/// ClientSession.pas（SelGate 特有会话逻辑）→ CSelSessionObj 的 1:1 断言。
/// 覆盖：构造/ReCreate（:56-69、:337-345）、SendDefMessage 帧格式（:94-124）、
/// ProcessCltData 全部反攻击与转发分支（:126-267）、ProcessSvrData（:269-279）、
/// UserEnter/UserLeave（:281-335）、FillUserList（:347-364）、上游帧构造（:253-254）。
/// </summary>
public class SelGateSessionTests
{
    /// <summary>构造一个接入真实 EDcode（GXX.Core 已 1:1 移植）的会话。</summary>
    private static CSelSessionObj NewSession(TempDir dir, FakeClientThread? server = null)
    {
        var cfg = new CConfigMgr(dir.File("Config.ini"));
        var log = new CLogMgr(IntPtr.Zero) { Config = cfg };
        var s = new CSelSessionObj
        {
            Codec = SelEDcodeAdapter.Instance,
            Config = cfg,
            LogMgr = log,
            IPFilter = new CSelGateIPFilter { BaseDirectory = dir.Path, Config = cfg },
            SendToGameSvr = (b) => server?.SendBuffer(b, b.Length),
        };
        if (server != null) s.m_tLastGameSvr = server;   // ClientSession.pas:17 m_tLastGameSvr
        return s;
    }

    private static byte[] EncodeMsg(ushort ident, long recog = 0, ushort param = 0, ushort tag = 0, ushort series = 0)
    {
        var m = default(TDefaultMessage);
        m.Recog = recog;
        m.Ident = ident;
        m.Param = param;
        m.Tag = tag;
        m.Series = series;
        byte[] raw = StructBytes.BytesOf(m);
        return EDcode.EncodeBuffer(raw, raw.Length);
    }

    /// <summary>ProcessCltData 的最小合法入参：已编码帧 + 填充到 DEF_BLOCK_SIZE(22) 的缓冲。</summary>
    private static byte[] MakeClientBuffer(params byte[] encoded)
    {
        byte[] buf = new byte[Math.Max(encoded.Length, Grobal2Const.DEF_BLOCK_SIZE)];
        Array.Copy(encoded, buf, encoded.Length);
        return buf;
    }

    // =====================================================================================
    // 1. 构造 / ReCreate
    // =====================================================================================

    [Fact]
    public void Ctor_InitialisesFlagsAndTicks()
    {
        var s = new CSelSessionObj();
        Assert.False(s.m_fKickFlag);        // :63
        Assert.Equal(0, s.m_nSvrObject);    // :64
        Assert.Equal(0, s.m_fHandleLogin);  // :67
        Assert.Equal(0, s.m_nSvrListIdx);   // :68
        Assert.Equal(0u, s.m_dwSessionID);
        Assert.Equal(0, (int)s.m_wRandKey);
    }

    [Fact]
    public void ReCreate_ResetsStateAndRefreshesTimeoutTick()
    {
        var s = new CSelSessionObj
        {
            m_fKickFlag = true,
            m_nSvrObject = 5,
            m_fHandleLogin = 3,
        };
        s.ReCreate();                                   // :337-345
        Assert.False(s.m_fKickFlag);
        Assert.Equal(0, s.m_nSvrObject);
        Assert.Equal(0, s.m_fHandleLogin);
        Assert.Equal(s.m_dwClientTimeOutTick, s.m_dwClientConnectTick); // :343
    }

    // =====================================================================================
    // 2. SendDefMessage：'#' + 6Bit(TCmdPack) + '!'
    //    差异/缺陷断言：param 被 series 覆盖（ClientSession.pas:107-108）
    // =====================================================================================

    [Fact]
    public void SendDefMessage_NoServer_DoesNothing()
    {
        using var dir = new TempDir();
        var s = NewSession(dir);
        var sent = new List<byte[]>();
        s.SendRaw = sent.Add;
        s.SendDefMessage(Grobal2Const.SM_OUTOFCONNECTION, 0, 0, 0, 0, "");   // :101-102 m_tLastGameSvr = nil
        Assert.Empty(sent);
    }

    [Fact]
    public void SendDefMessage_EmptyText_FrameIsHashPlusEncoded16BytesPlusBang()
    {
        using var dir = new TempDir();
        var s = NewSession(dir);
        s.m_tLastGameSvr = new FakeClientThread();
        var sent = new List<byte[]>();
        s.SendRaw = sent.Add;

        s.SendDefMessage(100, 0x1122334455667788L, 11, 22, 33, "");

        Assert.Single(sent);
        byte[] frame = sent[0];
        Assert.Equal((byte)'#', frame[0]);                                  // :110
        Assert.Equal((byte)'!', frame[frame.Length - 1]);                   // :122
        // 期望载荷：原文 :104-108 依次赋值 Recog/ident/param/tag，再以 param := nSeries 覆盖。
        // 用显式 wire 拼字节（SelGateWireLayoutTests 已锁定其与 Marshal 布局一致），再经 6-Bit 编码，
        // 避免与被测代码共用同一序列化/编码路径。
        var expected = default(TDefaultMessage);
        expected.Recog = 0x1122334455667788L;
        expected.Ident = 100;
        expected.Param = 33;   // :106 写 11，:108 被 nSeries(33) 覆盖 —— 原文如此
        expected.Tag = 22;
        expected.Series = 33;
        byte[] expectRaw = CSelSessionObj.BytesOfWire(expected);
        byte[] expectEnc = EDcode.EncodeBuffer(expectRaw, 16);
        Assert.Equal(expectEnc.Length + 2, frame.Length);
        Assert.Equal(expectEnc, frame.AsSpan(1, expectEnc.Length).ToArray());

        // 逐字段回读
        byte[] decoded = EDcode.DecodeBuffer(expectEnc, expectEnc.Length);
        Assert.Equal(expectRaw.AsSpan(0, 16).ToArray(), decoded.AsSpan(0, 16).ToArray());
        var back = StructBytes.FromBytes<TDefaultMessage>(decoded, 0);
        Assert.Equal(0x1122334455667788L, back.Recog);
        Assert.Equal(100, back.Ident);
        Assert.Equal(22, back.Tag);
        Assert.Equal(33, back.Param);   // nParam(11) 被 nSeries(33) 覆盖
    }

    [Fact]
    public void SendDefMessage_WithText_AppendsGbkMessageBytesBeforeEncoding()
    {
        using var dir = new TempDir();
        var s = NewSession(dir);
        s.m_tLastGameSvr = new FakeClientThread();
        var sent = new List<byte[]>();
        s.SendRaw = sent.Add;

        s.SendDefMessage(200, 0, 0, 0, 0, "测试abc");

        byte[] msgBytes = EncodingInit.GBK.GetBytes("测试abc");
        byte[] raw = new byte[16 + msgBytes.Length];
        Array.Copy(StructBytes.BytesOf(TDefaultMessage.Make(200, 0, 0, 0, 0)), raw, 16);
        Array.Copy(msgBytes, 0, raw, 16, msgBytes.Length);
        byte[] expectEnc = EDcode.EncodeBuffer(raw, raw.Length);

        Assert.Equal(expectEnc.Length + 2, sent[0].Length);
        Assert.Equal(expectEnc, sent[0].AsSpan(1, expectEnc.Length).ToArray());
    }

    // =====================================================================================
    // 3. ProcessCltData：反攻击分支
    // =====================================================================================

    [Fact]
    public void ProcessCltData_KickFlagSet_ResetsFlagAndFails()
    {
        using var dir = new TempDir();
        var s = NewSession(dir);
        s.m_fKickFlag = true;

        Assert.False(s.ProcessCltData(MakeClientBuffer(EncodeMsg(Grobal2Const.CM_QUERYCHR)), 22)); // :142-147
        Assert.False(s.m_fKickFlag);                                                                // :144 反向翻转
    }

    [Fact]
    public void ProcessCltData_PacketExceedsNomClientPacketSize_Kicks()
    {
        using var dir = new TempDir();
        var s = NewSession(dir);
        s.Config!.m_nNomClientPacketSize = 700;
        var kicked = new List<int>();
        s.KickUser = kicked.Add;

        byte[] buf = new byte[701];
        Assert.False(s.ProcessCltData(buf, 701));                        // :149-157
        Assert.Equal(new[] { s.m_pUserOBJ.nIPAddr }, kicked);
    }

    [Fact]
    public void ProcessCltData_HttpAttack_OnlyWhenDefenceEnabledAndLenAtLeastFive()
    {
        using var dir = new TempDir();
        var server = new FakeClientThread();
        var s = NewSession(dir, server);
        var kicked = new List<int>();
        s.KickUser = kicked.Add;

        // 30 字节：前 22 字节是合法的 CM_QUERYCHR 编码帧（会走转发分支、不踢），
        // 尾部 8 字节放 "HTTP/" 作探测（偏移 22..26）
        byte[] buf = new byte[30];
        Array.Copy(EncodeMsg(Grobal2Const.CM_QUERYCHR), buf, 22);
        Array.Copy(Encoding.ASCII.GetBytes("HTTP/"), 0, buf, 22, 5);

        // 未开启 DefenceCCPacket ⇒ 'HTTP/' 不参与判定，正常转发
        Assert.True(s.ProcessCltData(buf, 30));
        Assert.Empty(kicked);
        Assert.Single(server.Sent);

        // 开启后 :161-171 命中 ⇒ CC Attack, Kick
        s.Config!.m_fDefenceCCPacket = true;
        server.Sent.Clear();
        Assert.False(s.ProcessCltData(buf, 30));
        Assert.Single(kicked);
        Assert.Empty(server.Sent);
    }

    [Fact]
    public void ProcessCltData_HttpAttack_RequiresLenAtLeastFive()
    {
        using var dir = new TempDir();
        var s = NewSession(dir);
        var kicked = new List<int>();
        s.KickUser = kicked.Add;
        s.Config!.m_fDefenceCCPacket = true;

        // Len = 4 < 5 ⇒ :161 条件不成立；随后 :185 Len < 22 由 Attack2 分支踢
        byte[] buf = Encoding.ASCII.GetBytes("HTTP");
        Assert.False(s.ProcessCltData(buf, 4));
        Assert.Single(kicked);
    }

    [Fact]
    public void ProcessCltData_ShortJunkBuffer_IsKickedByAttack2()
    {
        using var dir = new TempDir();
        var s = NewSession(dir);
        var kicked = new List<int>();
        s.KickUser = kicked.Add;

        // 22 字节全 'A'（无 '$'、无 HTTP/、长度达标）：能走到解码，
        // 但解码结果 Ident=0x4141 不在白名单 ⇒ 命中 :258 else 分支被踢。
        // 说明"长度达标"并不等于"安全" —— 原版仍有命令白名单兜底。
        byte[] buf = new byte[22];
        for (int i = 0; i < 22; i++) buf[i] = (byte)'A';
        Assert.False(s.ProcessCltData(buf, 22));
        Assert.Single(kicked);
    }

    [Fact]
    public void ProcessCltData_DollarSignInPayload_AlwaysKicks()
    {
        using var dir = new TempDir();
        var s = NewSession(dir);
        var kicked = new List<int>();
        s.KickUser = kicked.Add;

        byte[] buf = new byte[64];
        buf[3] = (byte)'$';
        Assert.False(s.ProcessCltData(buf, 64));                          // :175-181 '$' Attack, Kick
        Assert.Single(kicked);
    }

    [Fact]
    public void ProcessCltData_LenBelowDefBlockSize_Kicks()
    {
        using var dir = new TempDir();
        var s = NewSession(dir);
        var kicked = new List<int>();
        s.KickUser = kicked.Add;

        Assert.False(s.ProcessCltData(new byte[21], 21));                 // :185-191 Len < 22
        Assert.Single(kicked);
    }

    [Fact]
    public void ProcessCltData_gDenyKicksEveryone()
    {
        using var dir = new TempDir();
        var s = NewSession(dir);
        var kicked = new List<int>();
        s.KickUser = kicked.Add;
        CSelSessionObj.gDeny = true;
        try
        {
            byte[] buf = MakeClientBuffer(EncodeMsg(Grobal2Const.CM_QUERYCHR));
            Assert.False(s.ProcessCltData(buf, 22));                      // :194-198
            Assert.Single(kicked);
        }
        finally { CSelSessionObj.gDeny = false; }
    }

    // =====================================================================================
    // 4. ProcessCltData：上游转发（SelGate 差异断言）
    // =====================================================================================

    [Theory]
    [InlineData(Grobal2Const.CM_QUERYCHR)]
    [InlineData(Grobal2Const.CM_NEWCHR)]
    [InlineData(Grobal2Const.CM_DELCHR)]
    [InlineData(Grobal2Const.CM_SELCHR)]
    [InlineData(Grobal2Const.CM_QUERYDELCHR)]
    [InlineData(Grobal2Const.CM_RANDOMNAME)]
    [InlineData(Grobal2Const.CM_GETBACKDELCHR)]
    public void ProcessCltData_CharacterSelectCommands_AreForwarded(int ident)
    {
        using var dir = new TempDir();
        var server = new FakeClientThread();
        var s = NewSession(dir, server);
        s.m_pUserOBJ._SendObj.Socket = 4321;

        byte[] buf = MakeClientBuffer(EncodeMsg((ushort)ident));
        Assert.True(s.ProcessCltData(buf, 22));                           // :244-257
        Assert.Single(server.Sent);

        byte[] frame = server.Sent[0];
        byte[] expected = CSelSessionObj.BuildUpstreamFrame(4321, buf, 22);
        Assert.Equal(expected, frame);
        Assert.Equal((byte)'%', frame[0]);
        Assert.Equal((byte)'!', frame[^2]);
        Assert.Equal((byte)'$', frame[^1]);
    }

    [Fact]
    public void BuildUpstreamFrame_HasExactLayoutPercentASocketHashOne()
    {
        byte[] addr = new byte[22];
        for (int i = 0; i < 22; i++) addr[i] = (byte)i;
        byte[] frame = CSelSessionObj.BuildUpstreamFrame(9876, addr, 22);

        byte[] head = Encoding.ASCII.GetBytes("%A9876/#1");
        Assert.Equal(head.Length + 22 + 2, frame.Length);
        Assert.Equal(head, frame.AsSpan(0, head.Length).ToArray());       // :253 pszBuf[0]='%' + 'A%d/#1%s!$'
        Assert.Equal(addr, frame.AsSpan(head.Length, 22).ToArray());
        Assert.Equal((byte)'!', frame[^2]);
        Assert.Equal((byte)'$', frame[^1]);
    }

    [Fact]
    public void BuildUpstreamFrame_TruncatesAtLen()
    {
        byte[] addr = new byte[64];
        byte[] frame = CSelSessionObj.BuildUpstreamFrame(1, addr, 22);
        Assert.Equal("%A1/#1".Length + 22 + 2, frame.Length);
    }

    [Fact]
    public void ProcessCltData_UnknownCommand_Kicks()
    {
        using var dir = new TempDir();
        var server = new FakeClientThread();
        var s = NewSession(dir, server);
        var kicked = new List<int>();
        s.KickUser = kicked.Add;

        byte[] buf = MakeClientBuffer(EncodeMsg(9999));
        Assert.False(s.ProcessCltData(buf, 22));                          // :258-264 else 分支
        Assert.Single(kicked);
        Assert.Empty(server.Sent);
    }

    [Fact]
    public void ProcessCltData_WhenHandleLoginNotZero_SkipsTheCommandSwitch()
    {
        using var dir = new TempDir();
        var server = new FakeClientThread();
        var s = NewSession(dir, server);
        var kicked = new List<int>();
        s.KickUser = kicked.Add;
        s.m_fHandleLogin = 1;                                             // :244 if m_fHandleLogin = 0

        byte[] buf = MakeClientBuffer(EncodeMsg(9999));
        Assert.True(s.ProcessCltData(buf, 22));                           // 不进入 case ⇒ 不踢
        Assert.Empty(kicked);
        Assert.Empty(server.Sent);
    }

    [Fact]
    public void ProcessCltData_ForwardedCommand_RefreshesTimeoutTick()
    {
        using var dir = new TempDir();
        var server = new FakeClientThread();
        var s = NewSession(dir, server);
        s.m_dwClientTimeOutTick = 1;

        s.ProcessCltData(MakeClientBuffer(EncodeMsg(Grobal2Const.CM_QUERYCHR)), 22);
        Assert.NotEqual(1u, s.m_dwClientTimeOutTick);                     // :250
    }

    // =====================================================================================
    // 5. ProcessSvrData
    // =====================================================================================

    [Fact]
    public void ProcessSvrData_PassesThroughPayload()
    {
        using var dir = new TempDir();
        var s = NewSession(dir);
        var sent = new List<byte[]>();
        s.SendRaw = sent.Add;

        byte[] payload = { 1, 2, 3, 4 };
        s.ProcessSvrData(new FakeClientThread(), payload, 4);             // :278

        Assert.Single(sent);
        Assert.Equal(payload, sent[0]);
    }

    [Fact]
    public void ProcessSvrData_WhenKickPending_FreesSocketAndDropsPayload()
    {
        using var dir = new TempDir();
        var s = NewSession(dir);
        var sent = new List<byte[]>();
        int freed = 0;
        s.SendRaw = sent.Add;
        s.FreeSocket = () => freed++;
        s.m_fKickFlag = true;

        s.ProcessSvrData(new FakeClientThread(), new byte[] { 9 }, 1);     // :272-277

        Assert.Equal(1, freed);
        Assert.Empty(sent);
        Assert.False(s.m_fKickFlag);
    }

    // =====================================================================================
    // 6. UserEnter / UserLeave
    // =====================================================================================

    [Fact]
    public void UserEnter_SendsOSocketIpLocalIpFrame()
    {
        using var dir = new TempDir();
        var server = new FakeClientThread();
        var s = NewSession(dir, server);
        s.m_pUserOBJ._SendObj.Socket = 555;
        s.m_pUserOBJ.pszIPAddr = "1.2.3.4";
        s.m_pUserOBJ.pszLocalIPAddr = "127.0.0.1";
        var added = new List<CSelSessionObj>();
        s.ProcMsgAddSession = added.Add;
        int before = CSelSessionObj.enterCount;

        s.UserEnter();                                                    // :281-292

        Assert.Equal(1, CSelSessionObj.enterCount - before);              // :286
        Assert.Single(added);                                             // :288
        Assert.Equal(0, s.m_fHandleLogin);                                // :287
        Assert.Equal(EncodingInit.GBK.GetBytes("%O555/1.2.3.4/127.0.0.1$"), server.Sent[0]);
    }

    [Fact]
    public void UserLeave_SendsXSocketFrameAndDeletesConnectOfIP()
    {
        using var dir = new TempDir();
        var server = new FakeClientThread();
        var s = NewSession(dir, server);
        s.m_pUserOBJ._SendObj.Socket = 556;
        s.m_pUserOBJ.nIPAddr = CSelGateIPFilter.InetAddr("10.0.0.9");
        var removed = new List<CSelSessionObj>();
        s.ProcMsgDelSession = removed.Add;
        s.DrainSendQueue = _ => { };
        int ip = s.m_pUserOBJ.nIPAddr;
        s.IPFilter!.Config!.m_fCheckNullSession = true;
        s.IPFilter.OverConnectOfIP(ip);
        Assert.Single(s.IPFilter.g_ConnectOfIPList);

        s.UserLeave();                                                    // :294-335

        Assert.Equal(EncodingInit.GBK.GetBytes("%X556$"), server.Sent[0]); // :304
        Assert.Empty(s.IPFilter.g_ConnectOfIPList);                       // :308 DeleteConnectOfIP
        Assert.Single(removed);                                           // :330
        Assert.Equal(0, s.m_fHandleLogin);
    }

    [Fact]
    public void UserLeave_SwallowsExceptionsAndLogsWithCode()
    {
        using var dir = new TempDir();
        var s = NewSession(dir);
        var logs = new List<string>();
        s.LogMgr!.OnAppend += logs.Add;
        s.m_pUserOBJ._SendObj.Socket = 1;
        s.m_pUserOBJ.nIPAddr = 0;
        s.DrainSendQueue = _ => throw new InvalidOperationException("boom"); // :311-326 步骤抛异常
        s.ProcMsgDelSession = _ => { };

        s.UserLeave();                                                    // :331-334 except ⇒ 只记日志

        Assert.Single(logs);
        Assert.Contains("TSessionObj.UserLeave: 3 boom", logs[0]);        // nCode = 3（:310）
    }

    [Fact]
    public void UserLeave_WithoutProcMsgThread_SkipsDelSession()
    {
        using var dir = new TempDir();
        var server = new FakeClientThread();
        var s = NewSession(dir, server);
        s.m_pUserOBJ._SendObj.Socket = 9;
        s.IPFilter!.Config!.m_fCheckNullSession = false;                  // DeleteConnectOfIP 直接 Exit
        s.ProcMsgDelSession = null;                                       // :329 g_ProcMsgThread = nil
        s.DrainSendQueue = _ => { };

        s.UserLeave();                                                    // 不得抛异常

        Assert.Single(server.Sent);
    }

    // =====================================================================================
    // 7. 全局用户表
    // =====================================================================================

    [Fact]
    public void FillUserList_UsesSingleSharedFillObject()
    {
        CSelUserList.FillUserList();                                      // :347-358
        try
        {
            Assert.NotNull(CSelUserList.g_pFillUserObj);
            Assert.Equal(CSelUserList.USER_ARRAY_COUNT, CSelUserList.g_UserList.Count);
            Assert.Equal(1048, CSelUserList.USER_ARRAY_COUNT);            // MAX_GAME_USER(1000) + 48
            Assert.Same(CSelUserList.g_UserList[0], CSelUserList.g_UserList[CSelUserList.USER_ARRAY_COUNT - 1]);
            Assert.Null(CSelUserList.g_pFillUserObj!.m_tLastGameSvr);     // :353
        }
        finally { CSelUserList.CleanupUserList(); }
    }

    // =====================================================================================
    // 8. 协议帧字节布局（wire 兼容，禁止漂移）
    // =====================================================================================

    [Fact]
    public void ProtocolFrameLayouts_MatchDelphiPackedRecords()
    {
        Assert.Equal(16, SelGateProtocol.SizeOfTCmdPack);      // Protocol.pas:43-49
        Assert.Equal(20, SelGateProtocol.SizeOfTSvrCmdPack);   // :55-62
        Assert.Equal(12, SelGateProtocol.SizeOfTCmdHeader);    // :65-73
        Assert.Equal(12, SelGateProtocol.SizeOfTEnDeInfo);     // :75-81
        Assert.Equal(16, TDefaultMessage.SizeOf);              // Grobal2.pas TDefaultMessage
        Assert.Equal(22, Grobal2Const.DEF_BLOCK_SIZE);
        Assert.Equal(0xAA55AA55u, Grobal2Const.RUNGATECODE);

        // TSvrCmdPack 小端序逐字节
        var pack = new SelGateProtocol.TSvrCmdPack
        {
            Flag = 0xAA55AA55, SockID = 0x11223344, Seq = 0x0102, Cmd = 3, GGSock = -1, DataLen = 5
        };
        byte[] b = GXX.Core.Protocol.StructBytes.BytesOf(pack);
        Assert.Equal(20, b.Length);
        Assert.Equal(new byte[] { 0x55, 0xAA, 0x55, 0xAA }, b.AsSpan(0, 4).ToArray());
        Assert.Equal(new byte[] { 0x44, 0x33, 0x22, 0x11 }, b.AsSpan(4, 4).ToArray());
        Assert.Equal(new byte[] { 0x02, 0x01 }, b.AsSpan(8, 2).ToArray());
        Assert.Equal(new byte[] { 0x03, 0x00 }, b.AsSpan(10, 2).ToArray());
        Assert.Equal(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }, b.AsSpan(12, 4).ToArray());
        Assert.Equal(new byte[] { 0x05, 0x00, 0x00, 0x00 }, b.AsSpan(16, 4).ToArray());
    }

    [Fact]
    public void TBlockIPMethod_OrdinalsMatchProtocolPas()
    {
        Assert.Equal(0, (int)TBlockIPMethod.mDisconnect);   // Protocol.pas:83
        Assert.Equal(1, (int)TBlockIPMethod.mBlock);
        Assert.Equal(2, (int)TBlockIPMethod.mBlockList);
        Assert.Equal(0, (int)TSockThreadStutas.stConnecting); // :84
        Assert.Equal(1, (int)TSockThreadStutas.stConnected);
        Assert.Equal(2, (int)TSockThreadStutas.stTimeOut);
    }

    [Fact]
    public void LogMgr_FormatsWithBracketedTimeAndCrLf()
    {
        using var dir = new TempDir();
        var cfg = new CConfigMgr(dir.File("Config.ini"));
        cfg.m_nShowLogLevel = 3;
        var log = new CLogMgr(IntPtr.Zero) { Config = cfg };
        var lines = new List<string>();
        log.OnAppend += lines.Add;

        log.Add("hello");                                                  // LogManager.pas:41-53

        Assert.Single(lines);
        Assert.EndsWith("] hello\r\n", lines[0]);
        Assert.StartsWith("[", lines[0]);
        Assert.True(log.CheckLevel(3));                                     // :38 >= nShowLv
        Assert.False(log.CheckLevel(4));
        Assert.True(log.CheckLevel(1));
    }

    [Fact]
    public void LogMgr_TimeToStr_UsesHHmmss()
    {
        Assert.Equal("00:00:00", CLogMgr.TimeToStr(new DateTime(2024, 1, 1, 0, 0, 0)));
        Assert.Equal("13:45:07", CLogMgr.TimeToStr(new DateTime(2024, 1, 1, 13, 45, 7)));
    }
}
