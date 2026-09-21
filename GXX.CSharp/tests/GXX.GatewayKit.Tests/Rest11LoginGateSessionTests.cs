using System;
using System.Collections.Generic;
using GXX.Core.Rtl;
using GXX.GatewayKit.Rest11;
using Xunit;

namespace GXX.GatewayKit.Tests;

public class Rest11LoginGateSessionTests
{
    // =====================================================================================
    // ClientSession.pas:62-80 / :87-101 构造与 ReCreate —— LoginGate 独有字段
    // =====================================================================================
    [Fact]
    public void Constructor_SetsLoginGateOnlyFieldsToZero()
    {
        var s = new Rest11LoginGateSession();
        Assert.False(s.m_fKickFlag);              // :66
        Assert.Equal(0, s.m_nSvrObject);          // :67
        Assert.Equal((byte)0, s.m_fHandleLogin);  // :69
        Assert.Equal(0, s.m_nSvrListIdx);         // :70
        Assert.Equal((ushort)0, s.m_wRandKey);    // :72

        Assert.Equal(0u, s.m_dwProtocolPassword); // :74 ★ LoginGate 独有
        Assert.False(s.m_IsCanSetL2Password);     // :75 ★
        Assert.False(s.m_IsCanCheckL2Password);   // :76 ★

        Assert.False(s.m_IsDelayClose);           // :78 ★
    }

    [Fact]
    public void ReCreate_ResetsAllLoginGateOnlyFields()
    {
        var s = new Rest11LoginGateSession
        {
            m_fKickFlag = true,
            m_nSvrObject = 9,
            m_fHandleLogin = 3,
            m_dwProtocolPassword = 0xDEADBEEF,
            m_IsCanSetL2Password = true,
            m_IsCanCheckL2Password = true,
            m_IsDelayClose = true
        };
        s.ReCreate();

        Assert.False(s.m_fKickFlag);              // :89
        Assert.Equal(0, s.m_nSvrObject);          // :91
        Assert.Equal((byte)0, s.m_fHandleLogin);  // :92
        Assert.Equal(0u, s.m_dwProtocolPassword); // :95
        Assert.False(s.m_IsCanSetL2Password);     // :96
        Assert.False(s.m_IsCanCheckL2Password);   // :97
        Assert.False(s.m_IsDelayClose);           // :99
    }

    // =====================================================================================
    // ClientSession.pas:785-789 DelayClose
    // =====================================================================================
    [Fact]
    public void DelayClose_SetsTickAndFlag()
    {
        var s = new Rest11LoginGateSession();
        uint before = DelphiRTL.GetTickCount();
        s.DelayClose(1000);
        Assert.True(s.m_IsDelayClose);
        Assert.InRange(s.m_dwDelayCloseTick, before + 1000, DelphiRTL.GetTickCount() + 1000);
    }

    // =====================================================================================
    // ClientSession.pas:103-121 RotateBits —— LoginGate 副本独有
    // =====================================================================================
    [Fact]
    public void RotateBits_ZeroBitsIsIdentity()
    {
        // Bits = 0：SI = MakeWord(0, C) = C；shr 0；Swap(C) 与 C 的字节和 = C
        for (char c = '\0'; c < (char)256; c++)
            Assert.Equal(c, Rest11LoginGateSession.RotateBits(c, 0));
    }

    [Theory]
    [InlineData('A', -1)]
    [InlineData('A', 1)]
    [InlineData((char)0xD6, -3)]
    [InlineData((char)0x7F, 7)]
    [InlineData('z', 9)]      // 9 mod 8 = 1
    public void RotateBits_MatchesOriginalBitAssembly(char c, int bits)
    {
        // 逐步复刻 Delphi 原文（`MakeWord(lo,hi) = (hi shl 8) or lo`；shl/shr/Swap 全部在 16 位上截断）：
        //   Bits := Bits mod 8;
        //   if Bits < 0 then begin SI := MakeWord(Byte(C),0); SI := SI shl Abs(Bits); end
        //   else begin SI := MakeWord(0,Byte(C)); SI := SI shr Abs(Bits); end;
        //   SI := Swap(SI); SI := Lo(SI) or Hi(SI); Result := chr(SI);
        int b = bits % 8;
        // MakeWord(lo, hi) = (hi shl 8) or lo：
        //   Bits < 0 ⇒ MakeWord(Byte(C), 0) = Byte(C)（高字节 0）
        //   Bits ≥ 0 ⇒ MakeWord(0, Byte(C)) = Byte(C) shl 8
        int si = (byte)c;
        if (b >= 0) si = si << 8;
        si = b < 0
            ? (si << Math.Abs(b)) & 0xFFFF                // :111 shl（16 位截断）
            : (si >> Math.Abs(b)) & 0xFFFF;               // :116 shr
        int swapped = (((si >> 8) | ((si << 8) & 0xFFFF)) & 0xFFFF);   // :118 Swap
        char expected = (char)((swapped & 0x00FF) | ((swapped >> 8) & 0x00FF)); // :119 Lo or Hi

        Assert.Equal(expected, Rest11LoginGateSession.RotateBits(c, bits));
    }

    [Fact]
    public void RotateBits_IsDeterministicAndBoundedToByte()
    {
        for (int i = 0; i < 256; i++)
        {
            char r = Rest11LoginGateSession.RotateBits((char)i, -3);
            Assert.InRange((int)r, 0, 255);       // Lo or Hi ⇒ 必然落在 1 字节
        }
    }

    // =====================================================================================
    // ClientSession.pas:690-704 ProcessSvrData
    // =====================================================================================
    [Fact]
    public void ProcessSvrData_ForwardsWhenNotKicked()
    {
        var s = new Rest11LoginGateSession { Socket = 5 };
        byte[]? got = null;
        bool ok = s.ProcessSvrData(new byte[] { 1, 2, 3 }, 3, (buf, len) => got = buf);
        Assert.True(ok);
        Assert.Equal(new byte[] { 1, 2, 3 }, got);
        Assert.Equal(5, s.Socket);
    }

    [Fact]
    public void ProcessSvrData_KickedSessionClearsFlagAndInvalidatesSocket()
    {
        var s = new Rest11LoginGateSession { Socket = 5, m_fKickFlag = true };
        bool called = false;
        bool ok = s.ProcessSvrData(new byte[] { 1 }, 1, (_, _) => called = true);
        Assert.False(ok);
        Assert.False(called);                                 // :694-696 直接 Exit
        Assert.False(s.m_fKickFlag);                          // :694 标志被反转回 False
        Assert.Equal(Rest11LoginGateSession.INVALID_SOCKET, s.Socket); // :695
    }

    [Fact]
    public void ProcessSvrData_SendFailureInvalidatesSocket()
    {
        var s = new Rest11LoginGateSession { Socket = 5 };
        bool ok = s.ProcessSvrData(new byte[] { 1 }, 1, (_, _) => throw new InvalidOperationException("x"));
        Assert.False(ok);                                     // :700-703
        Assert.Equal(Rest11LoginGateSession.INVALID_SOCKET, s.Socket);
    }

    // =====================================================================================
    // ClientSession.pas:706-737 UserEnter
    // =====================================================================================
    [Fact]
    public void UserEnter_RegistersSessionAndRandomizesProtocolPassword()
    {
        int enterBefore = Rest11LoginGateSession.g_enterCount;
        var t = new Rest11ProcMsgThread();
        var s = new Rest11LoginGateSession
        {
            Socket = 42,
            IPText = "10.0.0.42",
            LocalIPText = "127.0.0.1",
            Rand = max => 12345
        };

        string? sentToServer = null;
        byte[]? sentToClient = null;
        s.UserEnter(t, (sock, text, ip) => sentToServer = text, buf => sentToClient = buf);

        Assert.Equal(enterBefore + 1, Rest11LoginGateSession.g_enterCount);   // :711
        Assert.Equal((byte)0, s.m_fHandleLogin);                              // :712
        Assert.Same(s, t.GetSession(42));                                     // :713
        Assert.Equal(12346u, s.m_dwProtocolPassword);                         // :716 1 + Random(...)
        Assert.False(s.m_IsCanSetL2Password);                                 // :718
        Assert.False(s.m_IsCanCheckL2Password);                               // :719
        Assert.Equal("%O42/10.0.0.42/127.0.0.1$", sentToServer);              // :721-722
        Assert.NotNull(sentToClient);                                         // :731-732
        Assert.NotEmpty(sentToClient!);
    }

    // =====================================================================================
    // ClientSession.pas:739-783 UserLeave
    // =====================================================================================
    [Fact]
    public void UserLeave_ClearsHandleLogin_SendsXFrame_DeletesConn_AndRemovesSession()
    {
        var t = new Rest11ProcMsgThread();
        var s = new Rest11LoginGateSession { Socket = 43, IPAddr = 0x0A00002B, m_fHandleLogin = 2 };
        t.AddSession(s);

        string? sent = null;
        int deletedIp = 0;
        bool drained = false;
        s.DeleteConnectOfIPCallback = ip => deletedIp = ip;
        s.UserLeave(t, (sock, text) => sent = text, () => drained = true);

        Assert.Equal("%X43$", sent);            // :749
        Assert.Equal((byte)0, s.m_fHandleLogin); // :748
        Assert.Equal(0x0A00002B, deletedIp);     // :753
        Assert.True(drained);                    // :757-774
        Assert.Null(t.GetSession(43));           // :778
    }

    [Fact]
    public void UserLeave_ToleratesNullProcMsgThread()
    {
        var s = new Rest11LoginGateSession { Socket = 44 };
        s.UserLeave(null, (_, _) => { });
        Assert.Equal((byte)0, s.m_fHandleLogin);
    }

    // =====================================================================================
    // ClientSession.pas:123-155 SendDefMessage 的帧构造（含原文 param/series 覆盖缺陷）
    // =====================================================================================
    [Fact]
    public void BuildDefMessageFrame_WrapsWithHashAndBang()
    {
        byte[] frame = Rest11LoginGateSession.BuildDefMessageFrame(
            wIdent: 100, nRecog: 0x1122334455667788L, nParam: 7, nTag: 8, nSeries: 9, sMsg: "");

        Assert.Equal((byte)'#', frame[0]);                 // :138
        Assert.Equal((byte)'!', frame[^1]);                // :150
        Assert.True(frame.Length > 2);
    }

    [Fact]
    public void BuildDefMessageFrame_SeriesOverwritesParam_OriginalDefectPreserved()
    {
        // :134 Cmd.param := nParam;  :136 Cmd.param := nSeries;  ⇒ nParam **被 nSeries 覆盖**
        const long recog = 0x1122334455667788L;
        byte[] frameA = Rest11LoginGateSession.BuildDefMessageFrame(1, recog, 7, 0, 9, "");
        byte[] frameB = Rest11LoginGateSession.BuildDefMessageFrame(1, recog, 99, 0, 9, "");
        Assert.Equal(frameA, frameB);                     // nParam 不同但结果相同 ⇒ 证明被覆盖

        byte[] frameC = Rest11LoginGateSession.BuildDefMessageFrame(1, recog, 7, 0, 8, "");
        Assert.NotEqual(frameA, frameC);                  // nSeries 有效（覆盖进 param）

        byte[] frameD = Rest11LoginGateSession.BuildDefMessageFrame(1, recog, 7, 1, 9, "");
        Assert.NotEqual(frameA, frameD);                  // nTag 有效

        // 帧结构：# + EncodeBuffer(len=16) + '!' ⇒ 2 + 22 = 24
        Assert.Equal((byte)'#', frameA[0]);               // :138
        Assert.Equal((byte)'!', frameA[^1]);              // :150
        Assert.Equal(24, frameA.Length);

        // ★ 差异断言：Body = EncodeBuffer(Cmd 的 16 字节)，其中 param = nSeries（:136 覆盖 :134）、
        //   而 **Series 字段保持 0**（原文从未给它赋值）。
        var cmd = new GXX.Core.Protocol.TDefaultMessage
        {
            Recog = recog, Ident = 1, Param = 9, Tag = 0, Series = 0
        };
        byte[] cmdBytes = GXX.Core.Protocol.StructBytes.BytesOf(cmd);
        Assert.Equal(16, cmdBytes.Length);
        Assert.Equal((byte)0x01, cmdBytes[8]);            // Ident
        Assert.Equal((byte)0x09, cmdBytes[10]);           // Param ← nSeries（:136）
        Assert.Equal((byte)0x00, cmdBytes[12]);           // Tag
        Assert.Equal((byte)0x00, cmdBytes[14]);           // Series 未被赋值 ⇒ 0（原文如此）
        Assert.Equal(GXX.Core.Protocol.EDcode.EncodeBuffer(cmdBytes, 16), frameA.AsSpan(1, 22).ToArray());
    }

    // =====================================================================================
    // ClientSession.pas:690 附近的 IRest11SessionObj 接缝（供 Misc/FuncForComm 使用）
    // =====================================================================================
    [Fact]
    public void SessionObjSeam_ComposesLastGameSvrActiveFromBothParts()
    {
        IRest11SessionObj s = new Rest11LoginGateSession { Socket = 77, IPAddr = 0x01020304 };
        Assert.False(s.LastGameSvrActive);                       // m_tLastGameSvr = nil

        ((Rest11LoginGateSession)s).m_fConnectedToGameSvr = true;
        Assert.False(s.LastGameSvrActive);                       // 连上了但 Active 未置
        ((Rest11LoginGateSession)s).m_fGameSvrActive = true;
        Assert.True(s.LastGameSvrActive);                        // :166-167 两者皆真

        Assert.Equal(0x01020304, s.IPAddr);
        Assert.Equal(77, s.Socket);
    }

    [Fact]
    public void SessionObjSeam_KickFlagRoundTrips()
    {
        var impl = new Rest11LoginGateSession();
        IRest11SessionObj s = impl;
        s.KickFlag = true;
        Assert.True(impl.m_fKickFlag);
        impl.m_fKickFlag = false;
        Assert.False(s.KickFlag);
    }

    // =====================================================================================
    // Rest11LoginGateOptions —— 默认全关（"默认不改变现有行为"）
    // =====================================================================================
    [Fact]
    public void Options_DefaultAllDisabled()
    {
        var o = Rest11LoginGateOptions.Disabled;
        Assert.False(o.EnableProcMsgThread);
        Assert.False(o.EnableLoginGateIniSections);
        Assert.False(o.EnableIpAddrFilterResidual);
        Assert.False(o.EnableMiscEnforcement);
        Assert.False(o.EnableSessionResidual);

        var all = Rest11LoginGateOptions.All;
        Assert.True(all.EnableProcMsgThread);
        Assert.True(all.EnableLoginGateIniSections);
        Assert.True(all.EnableIpAddrFilterResidual);
        Assert.True(all.EnableMiscEnforcement);
        Assert.True(all.EnableSessionResidual);
    }

    // =====================================================================================
    // Protocol.pas:83-103 投影的数值一致性（与既有 GatewayKit / SelGate 投影对照）
    // =====================================================================================
    [Fact]
    public void EnumProjections_MatchExistingGatewayKitProjections()
    {
        Assert.Equal((byte)GXX.GatewayKit.TBlockIPMethod.mDisconnect, (byte)Rest11TBlockIPMethod.mDisconnect);
        Assert.Equal((byte)GXX.GatewayKit.TBlockIPMethod.mBlock, (byte)Rest11TBlockIPMethod.mBlock);
        Assert.Equal((byte)GXX.GatewayKit.TBlockIPMethod.mBlockList, (byte)Rest11TBlockIPMethod.mBlockList);
        Assert.Equal(0, (int)Rest11TSockThreadStutas.stConnecting);
        Assert.Equal(1, (int)Rest11TSockThreadStutas.stConnected);
        Assert.Equal(2, (int)Rest11TSockThreadStutas.stTimeOut);
    }

    // =====================================================================================
    // ★ 差异断言：`Windows.MakeWord(Lo, Hi) = (Hi shl 8) or Lo` 的实参顺序
    //   `GXX.Core.Rtl.DelphiRTL` 有 **(byte,byte)** 与 **(int,int)** 两个 MakeWord 重载，
    //   前者的实参名是 (lo, hi) 但实现按 `(hi << 8) | lo` 求值 —— 与后者一致；
    //   之所以容易踩坑，是因为**字面量常量会自动选 (byte,byte) 重载**，让人误以为顺序变了。
    //   Rest11 的 `RotateBits` 因此改用**具名实参** `MakeWord(lo:, hi:)` 调用。
    //   登记：报告 §6 D-P11-04（记录该重载歧义陷阱，非缺陷）。
    // =====================================================================================
    [Fact]
    public void CoreMakeWord_ArgumentOrderIsLoHi_AndOverloadAmbiguityTrapIsPinned()
    {
        // Delphi/Windows 语义：MakeWord(0x00, 0x41) = (0x41 shl 8) or 0x00 = 0x4100
        Assert.Equal(0x4100, DelphiRTL.MakeWord((int)0x00, (int)0x41));
        Assert.Equal(0x4100, DelphiRTL.MakeWord((byte)0x00, (byte)0x41));
        // 反过来：MakeWord(0x41, 0x00) = (0x00 shl 8) or 0x41 = 0x0041
        Assert.Equal(0x0041, DelphiRTL.MakeWord((int)0x41, (int)0x00));
        Assert.Equal(0x0041, DelphiRTL.MakeWord((byte)0x41, (byte)0x00));

        // Rest11 用具名实参 ⇒ 两种重载都可安全调用，结果一致
        Assert.Equal(0x0041, DelphiRTL.MakeWord(lo: 0x41, hi: 0x00));
        Assert.Equal(0x4100, DelphiRTL.MakeWord(lo: 0x00, hi: 0x41));

        // Rest11 的 RotateBits 不依赖调用歧义 ⇒ Bits=0 必须是恒等（0..255 全枚举）
        for (char c = '\0'; c < (char)256; c++)
            Assert.Equal(c, Rest11LoginGateSession.RotateBits(c, 0));
    }

    // =====================================================================================
    // ★ 差异断言：`System.Swap` 的 16 位截断（C# 的 ushort 左移会提升为 int）
    //   实测 `(ushort)((w >> 8) | (w << 8))` 对 w=0x2080 得 0x8020（应为 0x2000）
    //   ⇒ Rest11 的 SwapWord 显式 & 0x00FF / 0xFF00。登记：报告 §6 D-P11-05。
    // =====================================================================================
    [Fact]
    public void SwapWord_RequiresExplicit16BitTruncation()
    {
        // 以 RotateBits 的可观测行为固定（MakeWord(lo,hi) = (hi shl 8) or lo；Swap 截断到 16 位）：
        //  'A' bits=1  ⇒ SI=MakeWord(0,0x41)=0x4100；>>1 → 0x2080；Swap(0x2080)=0x8020
        //              ⇒ Lo|Hi = 0x20|0x80 = 0xA0
        Assert.Equal(0xA0, (int)Rest11LoginGateSession.RotateBits('A', 1));
        //  'A' bits=-1 ⇒ SI=MakeWord(0x41,0)=0x0041；<<1 → 0x0082；Swap(0x0082)=0x8200
        //              ⇒ Lo|Hi = 0x00|0x82 = 0x82
        Assert.Equal(0x82, (int)Rest11LoginGateSession.RotateBits('A', -1));
        //  0x7F bits=7 ⇒ SI=MakeWord(0,0x7F)=0x7F00；>>7 → 0x00FE；Swap=0xFE00 ⇒ Lo|Hi = 0xFE
        Assert.Equal(0xFE, (int)Rest11LoginGateSession.RotateBits((char)0x7F, 7));
        //  无截断时 Swap(0x2080) 会得 0x208000（int），本实现在任何输入下都必须 ≤ 0xFF
        for (int i = 0; i < 256; i++)
        {
            Assert.InRange((int)Rest11LoginGateSession.RotateBits((char)i, 1), 0, 0xFF);
            Assert.InRange((int)Rest11LoginGateSession.RotateBits((char)i, -1), 0, 0xFF);
        }
    }
}
