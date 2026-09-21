using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GXX.Core.Rtl;
using GXX.GatewayKit;
using GXX.GatewayKit.Rest11;
using Xunit;

namespace GXX.GatewayKit.Tests;

/// <summary>`SendMessage(hWnd, WM_COPYDATA, ...)` 的记录替身。</summary>
internal sealed class FakeGameCenterChannel : IRest11GameCenterChannel
{
    public readonly List<(IntPtr hWnd, int nParam, string msg)> Calls = new();
    public void SendCopyData(IntPtr hWnd, int nParam, string sSendMsg) => Calls.Add((hWnd, nParam, sSendMsg));
}

public class Rest11LoginGateMiscTests : IDisposable
{
    private readonly string _dir;

    public Rest11LoginGateMiscTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "rest11misc-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, true); } catch { }
    }

    private Rest11LoginGateConfig NewConfig(Rest11TBlockIPMethod method, bool kickOverPacketSize = true,
                                            bool kickOverSpeed = false, int maxConnOfIP = 20)
        => new(Path.Combine(_dir, "c.ini"))
        {
            m_tBlockIPMethod = (int)method,
            m_fKickOverPacketSize = kickOverPacketSize,
            m_fKickOverSpeed = kickOverSpeed,
            m_nMaxConnectOfIP = maxConnOfIP
        };

    // =====================================================================================
    // Misc.pas:244-250 ReverseIP —— 1:1 逐字节反转
    // =====================================================================================
    [Fact]
    public void ReverseIP_MatchesOriginalBitAssembly()
    {
        // Result := (LOBYTE(LOWORD) shl 24) or (HIBYTE(LOWORD) shl 16) or (LOBYTE(HIWORD) shl 8) or HIBYTE(HIWORD)
        Assert.Equal(0xDDCCBBAAu, Rest11LoginGateMisc.ReverseIP(0xAABBCCDDu));
        Assert.Equal(0u, Rest11LoginGateMisc.ReverseIP(0u));
    }

    // =====================================================================================
    // Misc.pas:252-273 AnsiStrToVal
    // =====================================================================================
    [Fact]
    public void AnsiStrToVal_ParsesLeadingDigits_AndCountsConsumed()
    {
        int n = Rest11LoginGateMisc.AnsiStrToVal("123abc", out int nPos);
        Assert.Equal(123, n);
        Assert.Equal(3, nPos);

        Assert.Equal(0, Rest11LoginGateMisc.AnsiStrToVal("abc", out nPos));
        Assert.Equal(0, nPos);

        Assert.Equal(0, Rest11LoginGateMisc.AnsiStrToVal(null, out nPos));
        Assert.Equal(0, nPos);

        Assert.Equal(0, Rest11LoginGateMisc.AnsiStrToVal("", out nPos));   // 原文靠 #0 终止
        Assert.Equal(0, nPos);
    }

    // =====================================================================================
    // Misc.pas:288-318 CheckAccountName
    // =====================================================================================
    [Theory]
    [InlineData("", false)]           // :294 空串直接 False
    [InlineData("AB", true)]          // 大写字母在 '0'..'z' 内（原文如此：范围极宽松）
    [InlineData("abc123", true)]
    [InlineData("a:b", true)]         // ':' 在 '0'..'z' 内
    [InlineData("a@b", true)]         // '@' 在 '0'..'z' 内
    [InlineData("a/b", false)]        // '/' = 0x2F < '0'，且非 GBK 双字节首字节 ⇒ False
    public void CheckAccountName_RangeIsLoose(string name, bool expected)
        => Assert.Equal(expected, Rest11LoginGateMisc.CheckAccountName(name));

    [Fact]
    public void CheckAccountName_GbkDoubleByteCharsAreRejectedBecauseUtf16CodePointExceedsZ()
    {
        // '中' 的 GBK 字节 = 0xD6 0xD0（首字节落在 #$B0..#$C8）
        string gbkDecoded = GXX.Core.EncodingInit.GBK.GetString(new byte[] { 0xD6, 0xD0 });
        Assert.Equal("中", gbkDecoded);
        Assert.Equal(1, gbkDecoded.Length);       // GBK → UTF-16 后是 1 个代码单元
        // ★ 原文按 **AnsiString 字节**逐字节判定；托管按 **UTF-16 代码单元**判定
        //   ⇒ U+4E2D > 'z' 且 < #$B0 的上界判断也走不到双字节回退（#$B0..#$C8 是 GBK 首字节区间）。
        //   偏差登记见报告 §6 D-P11-02（未修，原文照抄的边界）。
        Assert.False(Rest11LoginGateMisc.CheckAccountName(gbkDecoded));
    }

    [Fact]
    public void CheckAccountName_Latin1FallbackRangeAcceptsSecondByteInA1Fe()
    {
        // 直接构造 UTF-16 代码单元以命中 :306-312 的双字节回退分支：
        // 首字符在两个范围之外（'/'），第二个字符落在 #$A1..#$FE
        string s = "/" + '\u00A5';
        Assert.False(Rest11LoginGateMisc.CheckAccountName(s));   // 首字符 '/' < '0' 且不在 #$B0..#$C8 ⇒ False

        string s2 = "\u00B5\u00A5";                              // 首字符在 #$B0..#$C8 内
        Assert.True(Rest11LoginGateMisc.CheckAccountName(s2));   // 第二字符在 #$A1..#$FE 内 ⇒ 置 True
    }

    // =====================================================================================
    // Misc.pas:141-175 SendGameCenterMsg —— LoginGate 副本 nParam 无分支
    // =====================================================================================
    [Fact]
    public void SendGameCenterMsg_UsesLoginGateTypeAndIdent()
    {
        var ch = new FakeGameCenterChannel();
        Rest11LoginGateMisc.SendGameCenterMsg(Rest11LoginGateMisc.SG_STARTNOW, "开始", ch);

        Assert.Single(ch.Calls);
        // MakeLong(Word(tLoginGate), wIdent) = (wIdent << 16) | 4
        Assert.Equal(DelphiRTL.MakeLong((int)Rest11LoginGateMisc.TProgamType.tLoginGate, Rest11LoginGateMisc.SG_STARTNOW),
                     ch.Calls[0].nParam);
        Assert.Equal("开始", ch.Calls[0].msg);
    }

    // =====================================================================================
    // Misc.pas:181-205 KickUser(nRemoteIP): Boolean
    // =====================================================================================
    [Fact]
    public void KickUser_ByIP_ReturnsTrueWhenSwitchOff()
    {
        var ch = new FakeEnforcementChannel();
        var cfg = NewConfig(Rest11TBlockIPMethod.mBlock, kickOverPacketSize: false);
        Assert.True(Rest11LoginGateMisc.KickUser(0x0A000001, cfg, new List<IRest11SessionObj?>(), ch));
        Assert.Empty(ch.TempBlockCalls);
        Assert.Empty(ch.FreeSocketCalls);
    }

    [Fact]
    public void KickUser_ByIP_mDisconnect_ReturnsFalseWithoutBlocking()
    {
        var ch = new FakeEnforcementChannel();
        var cfg = NewConfig(Rest11TBlockIPMethod.mDisconnect);
        Assert.False(Rest11LoginGateMisc.KickUser(0x0A000001, cfg, new List<IRest11SessionObj?>(), ch));
        Assert.Empty(ch.TempBlockCalls);   // :186-189 原文如此：mDisconnect 分支不封禁
        Assert.Empty(ch.BlockListCalls);
    }

    [Fact]
    public void KickUser_ByIP_mBlock_AddsTempBlockAndClosesMatchingSessions()
    {
        var ch = new FakeEnforcementChannel();
        var cfg = NewConfig(Rest11TBlockIPMethod.mBlock);
        int ip = Rest11LoginGateNet.InetAddr("10.0.0.1");

        var match = new FakeSession { IPAddr = ip, Socket = 11, HandleLogin = 0, LastGameSvrActive = true };
        var other = new FakeSession { IPAddr = 0x0A000002, Socket = 12, HandleLogin = 0 };
        var list = new List<IRest11SessionObj?> { null, match, other };

        Assert.False(Rest11LoginGateMisc.KickUser(ip, cfg, list, ch));
        Assert.Equal(new[] { ip }, ch.TempBlockCalls);
        Assert.Equal(new[] { 11 }, ch.FreeSocketCalls);     // HandleLogin=0 < 2 ⇒ FreeSocket（:176）
        Assert.Empty(ch.OutOfConnectionCalls);
        Assert.False(match.KickFlag);                       // CloseIPConnect 的 FreeSocket 分支**不**置 KickFlag
    }

    [Fact]
    public void KickUser_ByIP_mBlockList_AddsPermanentBlock()
    {
        var ch = new FakeEnforcementChannel();
        var cfg = NewConfig(Rest11TBlockIPMethod.mBlockList);
        int ip = Rest11LoginGateNet.InetAddr("10.0.0.2");
        var s = new FakeSession { IPAddr = ip, Socket = 21, HandleLogin = 5 };
        Assert.False(Rest11LoginGateMisc.KickUser(ip, cfg, new List<IRest11SessionObj?> { s }, ch));
        Assert.Equal(new[] { ip }, ch.BlockListCalls);
        Assert.Equal(new[] { 21 }, ch.OutOfConnectionCalls.Select(c => c.ident).ToArray()); // HandleLogin>=2 ⇒ 发消息
        Assert.True(s.KickFlag);                            // :173
    }

    // =====================================================================================
    // Misc.pas:207-224 KickUser(const UserObj) —— 先 FreeSocket 再置 KickFlag
    // =====================================================================================
    [Fact]
    public void KickUser_ByObject_FreeSocketThenFlag_ThenBlockByMethod()
    {
        var ch = new FakeEnforcementChannel();
        var cfg = NewConfig(Rest11TBlockIPMethod.mBlock);
        var s = new FakeSession { IPAddr = 0x0A000003, Socket = 31 };
        Rest11LoginGateMisc.KickUser(s, cfg, ch);
        Assert.Equal(new[] { 31 }, ch.FreeSocketCalls);
        Assert.True(s.KickFlag);
        Assert.Equal(new[] { 0x0A000003 }, ch.TempBlockCalls);
    }

    [Fact]
    public void KickUser_ByObject_mDisconnect_OnlyFreesAndFlags()
    {
        var ch = new FakeEnforcementChannel();
        var cfg = NewConfig(Rest11TBlockIPMethod.mDisconnect);
        var s = new FakeSession { IPAddr = 0x0A000004, Socket = 41 };
        Rest11LoginGateMisc.KickUser(s, cfg, ch);
        Assert.Equal(new[] { 41 }, ch.FreeSocketCalls);
        Assert.True(s.KickFlag);
        Assert.Empty(ch.TempBlockCalls);   // :213-222 原文如此：mDisconnect 未列出
        Assert.Empty(ch.BlockListCalls);
    }

    [Fact]
    public void KickUser_ByObject_NoOpWhenSwitchOff()
    {
        var ch = new FakeEnforcementChannel();
        var cfg = NewConfig(Rest11TBlockIPMethod.mBlock, kickOverPacketSize: false);
        var s = new FakeSession { IPAddr = 0x0A000005, Socket = 51 };
        Rest11LoginGateMisc.KickUser(s, cfg, ch);
        Assert.Empty(ch.FreeSocketCalls);
        Assert.False(s.KickFlag);
    }

    // =====================================================================================
    // Misc.pas:226-242 BlockUser —— 不 FreeSocket
    // =====================================================================================
    [Fact]
    public void BlockUser_FlagsAndBlocksWithoutFreeingSocket()
    {
        var ch = new FakeEnforcementChannel();
        var cfg = NewConfig(Rest11TBlockIPMethod.mBlockList);
        var s = new FakeSession { IPAddr = 0x0A000006, Socket = 61 };
        Rest11LoginGateMisc.BlockUser(s, cfg, ch);
        Assert.True(s.KickFlag);
        Assert.Equal(new[] { 0x0A000006 }, ch.BlockListCalls);
        Assert.Empty(ch.FreeSocketCalls);
    }

    [Fact]
    public void BlockUser_NoOpWhenSwitchOff_And_mDisconnectDoesNotBlock()
    {
        var ch = new FakeEnforcementChannel();
        var s = new FakeSession { IPAddr = 0x0A000007, Socket = 71 };

        Rest11LoginGateMisc.BlockUser(s, NewConfig(Rest11TBlockIPMethod.mBlock, kickOverPacketSize: false), ch);
        Assert.False(s.KickFlag);

        Rest11LoginGateMisc.BlockUser(s, NewConfig(Rest11TBlockIPMethod.mDisconnect), ch);
        Assert.True(s.KickFlag);
        Assert.Empty(ch.TempBlockCalls);
        Assert.Empty(ch.BlockListCalls);
    }

    // =====================================================================================
    // Misc.pas:155-179 CloseIPConnect
    // =====================================================================================
    [Fact]
    public void CloseIPConnect_EarlyExitWhenServiceNotStarted()
    {
        var ch = new FakeEnforcementChannel { ServiceStarted = false };
        var s = new FakeSession { IPAddr = 5, Socket = 1 };
        Rest11LoginGateMisc.CloseIPConnect(5, new List<IRest11SessionObj?> { s }, ch);
        Assert.Empty(ch.FreeSocketCalls);
    }

    [Fact]
    public void CloseIPConnect_RequiresLastGameSvrActive()
    {
        var ch = new FakeEnforcementChannel();
        var s = new FakeSession { IPAddr = 5, Socket = 1, LastGameSvrActive = false };
        Rest11LoginGateMisc.CloseIPConnect(5, new List<IRest11SessionObj?> { s }, ch);
        Assert.Empty(ch.FreeSocketCalls);
    }

    [Fact]
    public void CloseIPConnect_SkipsNullSlots()
    {
        var ch = new FakeEnforcementChannel();
        Rest11LoginGateMisc.CloseIPConnect(5, new List<IRest11SessionObj?> { null, null }, ch);
        Assert.Empty(ch.FreeSocketCalls);
    }
}
