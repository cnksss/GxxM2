using System;
using System.Collections.Generic;
using System.Linq;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using Xunit;

using static GXX.RunGate.GateShareSeam;
using static GXX.RunGate.FormGlobals;
using static GXX.RunGate.RunGateConst;
using static GXX.RunGate.RunGateUtilsConst;
using static GXX.Core.Protocol.Grobal2Const;

namespace GXX.RunGate.Tests;

// =====================================================================================
// MirClientContext.pas（Source/RunGate/MirClientContext.pas）移植的测试。
//
// 串行化：本组测试与窗体族共享 FormGlobals / GateShareSeam 的静态全局量
//   （g_Config / g_boSayMsgControl / g_LockUserList / IocpTransport.Current ...），
//   必须挂 RunGateFormLane 集合，否则并行执行会互相污染
//   （依据 docs/并行报告-p2-rungate-impl.md §5.25-3）。
// =====================================================================================
[Collection("RunGateFormLane")]
public class MirClientContextFixture
{
    internal CapturingTransport Transport;
    internal FakeTcpClient TcpClient;
    internal TRunGate RunGate;
    internal TIocpCore Core;
    internal TMirClientContext Context;

    public MirClientContextFixture()
    {
        FormGlobals.ResetForTest();
        FormGlobals.ResetConfigForTest();
        GateShareSeam.ResetForTest();
        FrmMainSeam.FrmMain = null;

        Transport = new CapturingTransport();
        IocpTransport.Current = Transport;

        TcpClient = new FakeTcpClient { Active = true };
        RunGate = new TRunGate { TcpClient = TcpClient };
        Core = new TIocpCore { Owner = new TIocpTcpServer { BindObject = RunGate } };

        Context = new TMirClientContext(Core, 0x1234);
        Context.RemoteAddr = "10.0.0.1";
        Context.ContextID = 7;
        Context.InvokeDoReset(false);
        Transport.Sent.Clear();
        Transport.SentText.Clear();
        Transport.CloseCount = 0;
    }
}

/// <summary>捕获 PostSendText / PostSendBuffer / Close 的接缝替身。</summary>
public sealed class CapturingTransport : IIocpTransportSeam
{
    public readonly List<byte[]> Sent = new();
    public readonly List<string> SentText = new();
    public int CloseCount;

    public void PostSendText(TIocpClientContext context, string text) => SentText.Add(text);

    public void PostSendBuffer(TIocpClientContext context, byte[] buffer, int len)
    {
        byte[] copy = new byte[len];
        Array.Copy(buffer, 0, copy, 0, Math.Min(len, buffer.Length));
        Sent.Add(copy);
    }

    public void Close(TIocpClientContext context) { CloseCount++; context.MarkClosed(); }
}

/// <summary>ITcpClientSeam 替身（UseIocpClient = 1 的 RunGate.TcpClient）。</summary>
public sealed class FakeTcpClient : ITcpClientSeam
{
    public bool Active { get; set; }
    public readonly List<(int Ident, ushort SockIdx, int Socket, int UserListIndex, byte[] Data)> Sent = new();

    public void SendServerMsg(int nIdent, ushort wSocketIndex, int nSocket, int nUserListIndex,
                              byte[] buffer, int bufferLen)
    {
        byte[] copy = buffer == null ? Array.Empty<byte>() : new byte[bufferLen];
        if (buffer != null && bufferLen > 0) Array.Copy(buffer, 0, copy, 0, Math.Min(bufferLen, buffer.Length));
        Sent.Add((nIdent, wSocketIndex, nSocket, nUserListIndex, copy));
    }
}

/// <summary>原文 FrmMain 的两个回调的捕获替身。</summary>
public sealed class FakeFrmMain : IFrmMainSeam
{
    public readonly List<(TMirClientContext Ctx, bool IsProcessList)> ProcessListCalls = new();
    public readonly List<string> StatusTexts = new();

    public void RefreshContextProcessList(TMirClientContext context, bool isProcessList)
        => ProcessListCalls.Add((context, isProcessList));

    public void RefreshContextStatusText(string text) => StatusTexts.Add(text);
}

// =====================================================================================
// 单元级函数（原文 :308-412）
// =====================================================================================
[Collection("RunGateFormLane")]
public class MirClientContextUnitTests
{
    public MirClientContextUnitTests()
    {
        FormGlobals.ResetForTest();
        FormGlobals.ResetConfigForTest();
        GateShareSeam.ResetForTest();
    }

    // ---- SubStringOccurences 原文 :389-412 ----

    [Fact]
    public void SubStringOccurences_PlainCount()
    {
        Assert.Equal(3, MirClientContextUnit.SubStringOccurences("/", "a/b/c/d", false));
        Assert.Equal(0, MirClientContextUnit.SubStringOccurences("/", "abcd", false));
        Assert.Equal(1, MirClientContextUnit.SubStringOccurences("ab", "xxabyy", false));
    }

    [Fact]
    public void SubStringOccurences_NonOverlapping()
    {
        // 原文 :410 是 `pEx + Length(sub)` → 非重叠计数（"aaa" 里找 "aa" 得 1，不是 2）
        Assert.Equal(1, MirClientContextUnit.SubStringOccurences("aa", "aaa", false));
        Assert.Equal(2, MirClientContextUnit.SubStringOccurences("aa", "aaaa", false));
    }

    [Fact]
    public void SubStringOccurences_CaseInsensitiveFold()
    {
        Assert.Equal(1, MirClientContextUnit.SubStringOccurences("AB", "abAB", true));   // 区分大小写只命中 1 次
        Assert.Equal(2, MirClientContextUnit.SubStringOccurences("AB", "abAB", false));  // 折叠后 2 次
        Assert.Equal(0, MirClientContextUnit.SubStringOccurences("AB", "abab", true));
    }

    [Fact]
    public void SubStringOccurences_EmptySourceIsZero()
    {
        Assert.Equal(0, MirClientContextUnit.SubStringOccurences("/", "", false));
    }

    [Fact]
    public void SubStringOccurences_EmptySubString_DoesNotReproduceTheDelphiInfiniteLoop()
    {
        // 原文缺陷（MirClientContext.pas:410）：sub 为空串时 Length(sub) = 0，pEx 不前进 → 死循环。
        // **但** 托管侧不会复现，因为 RTL 垫片 `DelphiRTL.Pos("")` 返回 0
        // （Delphi 的 Pos('', S) 返回 1）—— 见 docs/并行报告-p2-rungate-impl.md §5.25-1。
        // 本断言把这个"行为被无意修好"的差异钉死，防止有人以为这里是死循环。
        Assert.Equal(0, MirClientContextUnit.SubStringOccurencesBounded("", "abc", false, 50));
        Assert.Equal(0, DelphiRTL.Pos("", "abc"));       // 垫片语义（与 Delphi 不同）
    }

    [Fact]
    public void SubStringOccurences_LoginPacketSlashCount_DrivesTheTwoBranches()
    {
        // DoCheckRecvBuffer:2061-2062 的分水岭：`/` 数 <= 5 走"老客户端"分支。
        // 登录串形态（原文 :2076/:2087 会先 Copy(sDataText, 3, Len-2) 去掉前两字符）
        string newClient = "#1/2/acc/chr/9/ver/key/ck/rlc/mac/umac/800/600/md5/";   // 14 个 '/'
        Assert.Equal(14, MirClientContextUnit.SubStringOccurences("/", newClient, false));
        Assert.True(MirClientContextUnit.SubStringOccurences("/", newClient, false) > 5);

        string oldClient = "#1/2/acc/chr/9/";            // 恰好 5 个 '/' → 走老客户端分支
        Assert.Equal(5, MirClientContextUnit.SubStringOccurences("/", oldClient, false));
        Assert.True(MirClientContextUnit.SubStringOccurences("/", oldClient, false) <= 5);

        // 边界差异断言：刚好 6 个就翻到"新客户端"分支
        Assert.False(MirClientContextUnit.SubStringOccurences("/", "#1/2/acc/chr/9/ver/", false) <= 5);
    }

    // ---- UpdateLockUserList / GetUserLockTime 原文 :308-387 ----

    private static TMirClientContext NewContext()
    {
        var core = new TIocpCore { Owner = new TIocpTcpServer { BindObject = new TRunGate() } };
        var ctx = new TMirClientContext(core, 1);
        ctx.sChrName = "测试角色";
        return ctx;
    }

    [Fact]
    public void UpdateLockUserList_SaveDisabled_ClearsListAndExits()
    {
        g_Config.boSaveLockStatus = false;
        g_LockUserList.AddObject("别的角色", 5);

        TMirClientContext ctx = NewContext();
        ctx.boLocked = true;
        ctx.dwUnLockTick = MyGetTickCount() + 60_000;

        MirClientContextUnit.UpdateLockUserList(ctx);

        Assert.Equal(0, g_LockUserList.Count);           // :318-319 整体清空
    }

    [Fact]
    public void UpdateLockUserList_LockedUserIsAddedWithRemainingSeconds()
    {
        g_Config.boSaveLockStatus = true;
        uint now = MyGetTickCount();
        TMirClientContext ctx = NewContext();
        ctx.boLocked = true;
        ctx.dwUnLockTick = now + 5_000;                  // 余 5 秒

        MirClientContextUnit.UpdateLockUserList(ctx);

        Assert.Equal(0, g_LockUserList.IndexOf("测试角色"));
        int stored = (int)g_LockUserList.GetObject(0);
        Assert.InRange(stored, 4, 5);                    // div 1000 截断 → 4 或 5
    }

    [Fact]
    public void UpdateLockUserList_UnlockExpired_RemovesEntry()
    {
        g_Config.boSaveLockStatus = true;
        g_LockUserList.AddObject("测试角色", 9);

        TMirClientContext ctx = NewContext();
        ctx.boLocked = false;                            // 已解锁
        ctx.dwUnLockTick = 0;

        MirClientContextUnit.UpdateLockUserList(ctx);

        Assert.Equal(-1, g_LockUserList.IndexOf("测试角色"));   // :340 Delete
    }

    [Fact]
    public void GetUserLockTime_SaveDisabled_ReturnsZeroAndClears()
    {
        g_Config.boSaveLockStatus = false;
        g_LockUserList.AddObject("测试角色", 42);

        Assert.Equal(0, MirClientContextUnit.GetUserLockTime(NewContext()));
        Assert.Equal(0, g_LockUserList.Count);
    }

    [Fact]
    public void GetUserLockTime_ClampsToConfigLockTime()
    {
        g_Config.boSaveLockStatus = true;
        g_Config.nLockTime = 10;
        g_LockUserList.AddObject("测试角色", 999);

        Assert.Equal(10, MirClientContextUnit.GetUserLockTime(NewContext()));   // :380-381 夹到 nLockTime
    }

    [Fact]
    public void GetUserLockTime_ZeroOrNegativeStoredValue_IsTreatedAsUnlocked()
    {
        g_Config.boSaveLockStatus = true;
        g_Config.nLockTime = 100;

        g_LockUserList.AddObject("测试角色", 0);          // 原文 TObject(0) = nil
        Assert.Equal(0, MirClientContextUnit.GetUserLockTime(NewContext()));

        g_LockUserList.Clear();
        g_LockUserList.AddObject("测试角色", -3);         // 负值（TObject 当整数用）
        Assert.Equal(0, MirClientContextUnit.GetUserLockTime(NewContext()));
    }

    [Fact]
    public void GetUserLockTime_AbsentUser_ReturnsZero()
    {
        g_Config.boSaveLockStatus = true;
        Assert.Equal(0, MirClientContextUnit.GetUserLockTime(NewContext()));
    }
}

// =====================================================================================
// GetExVersionNO（原文 :1888-1901）—— 静态纯函数，无需夹具
// =====================================================================================
public class MirClientContextVersionTests
{
    [Fact]
    public void GetExVersionNO_AtOrBelowThreshold_ReturnsZeroAndKeepsDate()
    {
        Assert.Equal(0, TMirClientContext.GetExVersionNO(100000000, out int old1));
        Assert.Equal(100000000, old1);

        Assert.Equal(0, TMirClientContext.GetExVersionNO(0, out int old2));
        Assert.Equal(0, old2);

        Assert.Equal(0, TMirClientContext.GetExVersionNO(-5, out int old3));
        Assert.Equal(-5, old3);
    }

    [Fact]
    public void GetExVersionNO_SingleStep()
    {
        // 100000001 → 结果 100000000、余 1
        Assert.Equal(100000000, TMirClientContext.GetExVersionNO(100000001, out int old));
        Assert.Equal(1, old);
    }

    [Fact]
    public void GetExVersionNO_MultiStep()
    {
        // 500000123 → 5 步，余 123
        Assert.Equal(500000000, TMirClientContext.GetExVersionNO(500000123, out int old));
        Assert.Equal(123, old);
    }

    [Fact]
    public void GetExVersionNO_ExactMultiple_StopsOneStepEarly()
    {
        // 原文 :1894 `while (nVersionDate > 100000000)` —— 严格大于。
        // 300000000 → 2 步后剩 100000000（**不大于**阈值）→ 停。
        // 差异断言：结果不是 300000000 而是 200000000，余数停在 100000000。
        Assert.Equal(200000000, TMirClientContext.GetExVersionNO(300000000, out int old));
        Assert.Equal(100000000, old);
    }

    [Fact]
    public void GetExVersionNO_MaxInt_CannotOverflow()
    {
        // 原文 :1897 `Inc(Result, 100000000)` 看似会溢出，实际**不可能**：
        // 循环条件保证 Result <= floor((int.MaxValue - 1e8) / 1e8) * 1e8 = 2.1e9 < int.MaxValue。
        // 照抄实现并把这个"溢出分支不可达"钉死（防止有人擅自加 checked 或改 long）。
        int result = TMirClientContext.GetExVersionNO(int.MaxValue, out int old);
        Assert.Equal(2100000000, result);
        Assert.Equal(47483647, old);
        Assert.True(result > 0);                         // 恒为正 —— 与"会回绕成负"的直觉相反
    }

    [Fact]
    public void GetExVersionNO_IsUsedToDecideOldClient_BoIsOldClient()
    {
        // 原文 :2211 `boIsOldClient := GetExVersionNO(StrToIntDef(sClientVersion, 0), IntValue) = 0;`
        Assert.True(TMirClientContext.GetExVersionNO(20240101) == 0);      // 新版 → 非旧客户端
        Assert.False(TMirClientContext.GetExVersionNO(100000123) == 0);    // 有进位 → 旧客户端
    }
}

// =====================================================================================
// 骨架 / 生命周期 / 发送族 / 限速判定（原文 416-1901、2947-3463、9689-9965）
// =====================================================================================
[Collection("RunGateFormLane")]
public class MirClientContextCoreTests
{
    private readonly CapturingTransport _t = new();
    private readonly FakeTcpClient _tcp = new();
    private readonly TRunGate _runGate;
    private readonly TMirClientContext _ctx;

    public MirClientContextCoreTests()
    {
        FormGlobals.ResetForTest();
        FormGlobals.ResetConfigForTest();
        GateShareSeam.ResetForTest();
        FrmMainSeam.FrmMain = null;

        IocpTransport.Current = _t;
        _runGate = new TRunGate { TcpClient = _tcp };
        var core = new TIocpCore { Owner = new TIocpTcpServer { BindObject = _runGate } };
        _ctx = new TMirClientContext(core, 0x99);
        _ctx.RemoteAddr = "1.2.3.4";
        _ctx.ContextID = 3;
        _ctx.InvokeDoReset(false);      // boSendGmClose := True（原文 :605），DoDisconnect 依赖它
        _t.Sent.Clear();
    }

    // ---- Create / DoReset（原文 416-443 / 473-679）----

    [Fact]
    public void Create_InitialisesFieldsLikeOriginal()
    {
        Assert.NotNull(_ctx.GameSpeed);
        Assert.Equal(10, _ctx.MagicCDSpeed.Length);
        Assert.Equal(30, _ctx.RecordActionArr.Length);
        Assert.Equal(ActionModeCount, _ctx.nCompensationArr.Length);
        Assert.Equal(300u, _ctx.nContextDataLen);
        Assert.NotNull(_ctx.pContextData);
        Assert.Equal(300, _ctx.pContextData.Length);
        Assert.Equal(46, _ctx.HumBagItems.Capacity);
        Assert.Equal(46, _ctx.HeroBagItems.Capacity);
    }

    [Fact]
    public void DoReset_NotClose_RestoresAllDefaults()
    {
        _ctx.nUserListIndex = 77;
        _ctx.sChrName = "张三";
        _ctx.boLocked = true;
        _ctx.nMoveSpeed = 5;
        _ctx.GameSpeed.dwAttackTick = 123;

        _ctx.InvokeDoReset(false);

        Assert.Equal(-1, _ctx.nUserListIndex);
        Assert.Equal(-1, _ctx.nPacketIndex);
        Assert.Equal(0, _ctx.nPacketErrCount);
        Assert.True(_ctx.boStartLogon);
        Assert.True(_ctx.boIsOldClient);          // 原文 :497 boIsOldClient := True
        Assert.Equal("", _ctx.sChrName);
        Assert.False(_ctx.boLocked);
        Assert.Equal(0u, _ctx.dwUnLockTick);
        Assert.Equal(0, _ctx.nMoveSpeed);
        Assert.Equal(0u, _ctx.GameSpeed.dwAttackTick);
        Assert.Equal(TAntiPlugActionMode.amHit, _ctx.LastLockAntiPlugActionMode);
        Assert.True(_ctx.boSendGmClose);
        // 原文 :589 `RunThreadIndex := Random(MIRCONTEXT_RUN_THREAD_COUNT)`（0..7），不是 0
        Assert.InRange(_ctx.RunThreadIndex, 0, MirClientContextConst.MIRCONTEXT_RUN_THREAD_COUNT - 1);
    }

    [Fact]
    public void DoReset_IsCloseOnlyClearsBuffersNotIdentity()
    {
        _ctx.sChrName = "保留";
        _ctx.nUserListIndex = 42;
        _ctx.InvokeDoReset(true);

        Assert.Equal("保留", _ctx.sChrName);       // IsClose 分支不重置身份
        Assert.Equal(42, _ctx.nUserListIndex);
        Assert.True(_ctx.boSendGmClose);           // 但 :605 仍执行
    }

    [Fact]
    public void DoReset_VerifyIntervalIsSeededWithinConfiguredWindow()
    {
        g_dwVerifyCodeInterval1 = 30;
        g_dwVerifyCodeInterval2 = 50;
        GateShareSeam.RandomSink = range => 0;      // 固定随机源，便于断言下界
        _ctx.InvokeDoReset(false);

        Assert.Equal(30u * 60000, _ctx.dwVerifyInterval);   // :668 下界
    }

    // ---- CheckRecvPacketSize（原文 1904-1918）----

    [Fact]
    public void CheckRecvPacketSize_UnderLimit_ReturnsTrueAndDoesNotClose()
    {
        g_boKickOverPacketSize = true;
        Assert.True(_ctx.CheckRecvPacketSize(10, 10, 3011));
        Assert.False(_ctx.boDelayClose);
        Assert.Equal(0, _t.CloseCount);
    }

    [Fact]
    public void CheckRecvPacketSize_OverLimit_KicksByConfigBlockMethod()
    {
        g_boKickOverPacketSize = true;
        g_BlockMethod = TBlockIPMethod.bmBlockList;
        var blocked = new List<string>();
        AddBlockIPSink = ip => blocked.Add(ip);

        Assert.False(_ctx.CheckRecvPacketSize(11, 10, 3011));
        Assert.Equal(new[] { "1.2.3.4" }, blocked);
        Assert.True(_ctx.boDelayClose);                     // :1915 DelayClose(100)
    }

    [Fact]
    public void CheckRecvPacketSize_TempBlockMethod_AddsTempNotPermanent()
    {
        g_boKickOverPacketSize = true;
        g_BlockMethod = TBlockIPMethod.bmTempBlock;
        var perm = new List<string>();
        var temp = new List<string>();
        AddBlockIPSink = ip => perm.Add(ip);
        AddTempBlockIPSink = ip => temp.Add(ip);

        Assert.False(_ctx.CheckRecvPacketSize(9999, 10, 1));
        Assert.Empty(perm);
        Assert.Equal(new[] { "1.2.3.4" }, temp);
    }

    [Fact]
    public void CheckRecvPacketSize_DisconnectMethod_MatchesNoCaseSoNoBlockCall()
    {
        // 原文 :1910 的 case **无 else** → bmDisconnect 两个分支都不执行（照抄的差异断言）
        g_boKickOverPacketSize = true;
        g_BlockMethod = TBlockIPMethod.bmDisconnect;
        var perm = new List<string>();
        var temp = new List<string>();
        AddBlockIPSink = ip => perm.Add(ip);
        AddTempBlockIPSink = ip => temp.Add(ip);

        Assert.False(_ctx.CheckRecvPacketSize(9999, 10, 1));
        Assert.Empty(perm);
        Assert.Empty(temp);
        Assert.True(_ctx.boDelayClose);
    }

    [Fact]
    public void CheckRecvPacketSize_KickSwitchOff_NeverKicksEvenOnHugePacket()
    {
        g_boKickOverPacketSize = false;
        Assert.True(_ctx.CheckRecvPacketSize(int.MaxValue, 1, 1));
        Assert.False(_ctx.boDelayClose);
    }

    // ---- 消息队列（原文 2947-3120）----

    [Fact]
    public void ProcessClientMessage_ThenGetClientMessage_RoundTripsPayload()
    {
        var def = TDefaultMessage.Make(3011, 123, 4, 5, 6);
        byte[] payload = { 1, 2, 3, 4 };

        _ctx.ProcessClientMessage(def, payload);

        var msg = new TProcessMsg();
        Assert.True(_ctx.GetClientMessage(msg));
        Assert.Equal((ushort)3011, msg.DefMessage.Ident);
        Assert.Equal(123L, msg.DefMessage.Recog);
        Assert.Equal(payload, msg.sMessage);
        Assert.Null(_ctx.ClientMsgListProbe.Count == 0 ? null : (object)1);   // 出队后为空
    }

    [Fact]
    public void ProcessClientMessage_EmptyPayload_YieldsEmptyMessage()
    {
        _ctx.ProcessClientMessage(TDefaultMessage.Make(1, 0, 0, 0, 0), Array.Empty<byte>());
        var msg = new TProcessMsg();
        Assert.True(_ctx.GetClientMessage(msg));
        Assert.Empty(msg.sMessage);
    }

    [Fact]
    public void ProcessClientMessage_DelayCloseOrClosing_IsIgnored()
    {
        _ctx.boDelayClose = true;
        _ctx.ProcessClientMessage(TDefaultMessage.Make(1, 0, 0, 0, 0), Array.Empty<byte>());
        Assert.Equal(0, _ctx.ClientMsgListProbe.Count);

        _ctx.boDelayClose = false;
        _ctx.IsPostedCloseQuest = true;
        _ctx.ProcessClientMessage(TDefaultMessage.Make(1, 0, 0, 0, 0), Array.Empty<byte>());
        Assert.Equal(0, _ctx.ClientMsgListProbe.Count);
    }

    [Fact]
    public void GetClientMessage_EmptyQueue_ReturnsFalse()
    {
        Assert.False(_ctx.GetClientMessage(new TProcessMsg()));
    }

    [Fact]
    public void DelayClientMessage_PushesFrontAndShiftsExistingTicks()
    {
        _ctx.ProcessClientMessage(TDefaultMessage.Make(3011, 0, 0, 0, 0), Array.Empty<byte>());
        // 让 FDelayTick 变成过去时间，否则 ProcessClientMessage 会被 :2957 挡掉
        _ctx.DelayClientMessage(new TProcessMsg { DefMessage = TDefaultMessage.Make(9999, 0, 0, 0, 0) }, 0);

        var msg = new TProcessMsg();
        Assert.True(_ctx.GetClientMessage(msg));
        Assert.Equal((ushort)9999, msg.DefMessage.Ident);   // Insert(0) 的延时消息排在队首
    }

    [Fact]
    public void DelayClientMessage_ShiftsEveryQueuedTickByDelayTime()
    {
        g_nMaxClientPacketCount = 100;
        _ctx.ProcessClientMessage(TDefaultMessage.Make(1, 0, 0, 0, 0), Array.Empty<byte>());

        _ctx.DelayClientMessage(new TProcessMsg { DefMessage = TDefaultMessage.Make(2, 0, 0, 0, 0) }, 1);

        var first = (TClientMsg)_ctx.ClientMsgListProbe[0];
        var second = (TClientMsg)_ctx.ClientMsgListProbe[1];
        Assert.Equal((ushort)2, first.DefMessage.Ident);
        Assert.True(first.dwTimeTick >= second.dwTimeTick);   // :3032 旧条目被整体延后
    }

    [Fact]
    public void ClearClientMsgList_EmptiesQueueAndReleasesBuffers()
    {
        _ctx.ProcessClientMessage(TDefaultMessage.Make(1, 0, 0, 0, 0), new byte[] { 9, 9 });
        Assert.Equal(1, _ctx.ClientMsgListProbe.Count);

        _ctx.ClearClientMsgList();
        Assert.Equal(0, _ctx.ClientMsgListProbe.Count);
    }

    [Fact]
    public void GetConcurrentPacketCount_CountsOnlyMatchingIdent()
    {
        for (int i = 0; i < 3; i++)
            _ctx.ProcessClientMessage(TDefaultMessage.Make(3011, 0, 0, 0, 0), Array.Empty<byte>());
        _ctx.ProcessClientMessage(TDefaultMessage.Make(3017, 0, 0, 0, 0), Array.Empty<byte>());

        Assert.Equal(3, _ctx.GetConcurrentPacketCount(TDefaultMessage.Make(3011, 0, 0, 0, 0)));
        Assert.Equal(1, _ctx.GetConcurrentPacketCount(TDefaultMessage.Make(3017, 0, 0, 0, 0)));
        Assert.Equal(0, _ctx.GetConcurrentPacketCount(TDefaultMessage.Make(1234, 0, 0, 0, 0)));
    }

    [Fact]
    public void ClearConcurrentPacket_RemovesMatchingAndDropsOthers()
    {
        _ctx.ProcessClientMessage(TDefaultMessage.Make(3011, 0, 0, 0, 0), Array.Empty<byte>());
        _ctx.ProcessClientMessage(TDefaultMessage.Make(3017, 0, 0, 0, 0), Array.Empty<byte>());

        _ctx.ClearConcurrentPacket(TDefaultMessage.Make(3011, 0, 0, 0, 0));

        Assert.Equal(1, _ctx.ClientMsgListProbe.Count);
        Assert.Equal(0, _ctx.GetConcurrentPacketCount(TDefaultMessage.Make(3011, 0, 0, 0, 0)));
    }

    [Fact]
    public void ClearConcurrentPacket_AlwaysReturnsZero_UnlikeGetConcurrentPacketCount()
    {
        // 原文缺陷（MirClientContext.pas:9715）：Result 声明后从未赋值 → 恒 0。
        // 差异断言：同一状态下 Get 返回真实计数，Clear 返回 0。
        for (int i = 0; i < 4; i++)
            _ctx.ProcessClientMessage(TDefaultMessage.Make(3011, 0, 0, 0, 0), Array.Empty<byte>());

        Assert.Equal(4, _ctx.GetConcurrentPacketCount(TDefaultMessage.Make(3011, 0, 0, 0, 0)));
        Assert.Equal(0, _ctx.ClearConcurrentPacket(TDefaultMessage.Make(3011, 0, 0, 0, 0)));
    }

    // ---- 发送族（原文 3123-3262、9737-9745）----

    [Fact]
    public void SendMessaggeToClient_EmptyMessage_SendsNothing()
    {
        _ctx.SendMessaggeToClient("", 0, 1, 2);
        Assert.Empty(_t.Sent);
    }

    [Fact]
    public void SendMessaggeToClient_SysMessageAndMenuOk_ProduceDifferentFrames()
    {
        _ctx.SendMessaggeToClient("hello", 0, 0x12, 0x34);
        _ctx.SendMessaggeToClient("hello", 1, 0x12, 0x34);

        Assert.Equal(2, _t.Sent.Count);
        Assert.NotEqual(_t.Sent[0], _t.Sent[1]);     // SM_SYSMESSAGE vs SM_MENU_OK 两条路径
        // SM_MENU_OK 分支会先 EncodeString(Msg) 再编码，负载更长
        Assert.True(_t.Sent[1].Length > 0 && _t.Sent[0].Length > 0);
    }

    [Fact]
    public void SendWarnMsg_UsesFixedColors38AndFF()
    {
        _ctx.SendWarnMsg("warn");
        _ctx.SendMessaggeToClient("warn", 0, 0x38, 0xFF);
        Assert.Equal(_t.Sent[0], _t.Sent[1]);        // 同色同负载 → 同帧
    }

    [Fact]
    public void SendActionRet_EncodesIsGoodIntoParam()
    {
        _ctx.SendActionRet(true);
        _ctx.SendActionRet(false);
        Assert.Equal(2, _t.Sent.Count);
        Assert.NotEqual(_t.Sent[0], _t.Sent[1]);
    }

    [Fact]
    public void LockUser_ZeroTime_IsNoOp()
    {
        _ctx.LockUser(0);
        Assert.Empty(_t.Sent);
        Assert.False(_ctx.boLocked);
    }

    [Fact]
    public void LockUser_PositiveTime_SendsAndArmsUnlockTick()
    {
        g_Config.boShowLockLog = false;
        _ctx.sChrName = "锁定者";
        _ctx.LockUser(3);

        Assert.True(_ctx.boLocked);
        Assert.Equal(1, _t.Sent.Count);
        Assert.True(_ctx.dwUnLockTick > MyGetTickCount());
    }

    [Fact]
    public void UnLockUser_NotLocked_IsNoOp()
    {
        _ctx.UnLockUser();
        Assert.Empty(_t.Sent);
    }

    [Fact]
    public void UnLockUser_Locked_SendsAndUpdatesLockList()
    {
        g_Config.boSaveLockStatus = true;
        _ctx.sChrName = "解锁者";
        _ctx.LockUser(5);
        _t.Sent.Clear();

        _ctx.UnLockUser();

        Assert.False(_ctx.boLocked);
        Assert.Equal(1, _t.Sent.Count);
        Assert.Equal(-1, g_LockUserList.IndexOf("解锁者"));   // 已解锁 → :340 删除
    }

    [Fact]
    public void SendMessageToServer_NoRunGate_DoesNothing()
    {
        var core = new TIocpCore();                 // Owner = null
        var ctx = new TMirClientContext(core, 1);
        ctx.SendMessageToServer(TDefaultMessage.Make(3011, 1, 2, 3, 4), Array.Empty<byte>());
        Assert.Empty(_tcp.Sent);
    }

    [Fact]
    public void SendMessageToServer_EmptyBody_SendsHeaderOnly()
    {
        _ctx.SendMessageToServer(TDefaultMessage.Make(3011, 1, 2, 3, 4), Array.Empty<byte>());
        Assert.Single(_tcp.Sent);
        Assert.Equal(GM_DATA, _tcp.Sent[0].Ident);
        Assert.Equal(TDefaultMessage.SizeOf, _tcp.Sent[0].Data.Length);
    }

    [Fact]
    public void SendMessageToServer_WithBody_HeaderThenBody()
    {
        byte[] body = { 0xAA, 0xBB, 0xCC };
        _ctx.SendMessageToServer(TDefaultMessage.Make(3011, 1, 2, 3, 4), body);
        Assert.Single(_tcp.Sent);
        Assert.Equal(TDefaultMessage.SizeOf + body.Length, _tcp.Sent[0].Data.Length);
        Assert.Equal(body, _tcp.Sent[0].Data.Skip(TDefaultMessage.SizeOf).ToArray());
    }

    [Fact]
    public void SendMessageToServer_PointerOverload_RespectsMsgLen()
    {
        byte[] body = { 1, 2, 3, 4, 5 };
        _ctx.SendMessageToServer(TDefaultMessage.Make(1, 0, 0, 0, 0), body, 2);
        Assert.Equal(TDefaultMessage.SizeOf + 2, _tcp.Sent[0].Data.Length);

        _tcp.Sent.Clear();
        _ctx.SendMessageToServer(TDefaultMessage.Make(1, 0, 0, 0, 0), body, 0);
        Assert.Equal(TDefaultMessage.SizeOf, _tcp.Sent[0].Data.Length);
    }

    [Fact]
    public void SendMessageToServer_RawOverload_PassesThroughUnchanged()
    {
        byte[] raw = { 7, 7, 7 };
        _ctx.SendMessageToServer(raw, raw.Length);
        Assert.Single(_tcp.Sent);
        Assert.Equal(raw, _tcp.Sent[0].Data);
    }

    // ---- GetSpeedText / ContinuousSpeed / ProcessAssasinate（原文 3410-3463）----

    [Theory]
    [InlineData(0, "+0")]
    [InlineData(5, "+5")]
    [InlineData(-1, "-1")]
    [InlineData(-200, "-200")]
    [InlineData(int.MinValue, "-2147483648")]
    public void GetSpeedText_SignsNonNegativeValues(int value, string expected)
        => Assert.Equal(expected, TMirClientContext.GetSpeedText(value));

    [Fact]
    public void ContinuousSpeed_WalkRunAndSpellUseNames3_HitUsesNames()
    {
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        g_Config.btMsgFColor = 1; g_Config.btMsgBColor = 2;
        _ctx.nMoveSpeed = -3; _ctx.nAttackSpeed = 4; _ctx.nSpellSpeed = 0;

        _ctx.ContinuousSpeed(TAntiPlugActionMode.amWalk, 100);
        _ctx.ContinuousSpeed(TAntiPlugActionMode.amHit, 100);
        _ctx.ContinuousSpeed(TAntiPlugActionMode.amSpell, 100);
        _ctx.ContinuousSpeed(TAntiPlugActionMode.amTurn, 100);

        Assert.Equal(4, logs.Count);
        Assert.Contains("移动速度-3", logs[0]);              // amWalk 用 names3（:3425）
        Assert.Contains("攻击速度+4", logs[1]);              // amHit 用 names（:3431）
        Assert.Contains("魔法速度+0", logs[2]);
        // default 分支（:3446）只有"【速度异常】%s:%d; 用户:%s"，**不含**具体速度项
        Assert.DoesNotContain("移动速度", logs[3]);
        Assert.DoesNotContain("攻击速度", logs[3]);
        Assert.DoesNotContain("魔法速度", logs[3]);
        Assert.Contains("【速度异常】", logs[3]);
        Assert.True(_ctx.boDelayClose);                      // :3454 DelayClose(1000)
    }

    [Fact]
    public void ProcessAssasinate_LogsAndClosesAfter100ms()
    {
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        _ctx.ProcessAssasinate();

        Assert.Single(logs);
        Assert.True(_ctx.boDelayClose);
        Assert.True(_ctx.dwDelayCloseTick <= MyGetTickCount() + 100);
    }

    // ---- FilterSayMsg（原文 3281-3408）----

    [Fact]
    public void FilterSayMsg_NoWordFilterAndNoSayControl_PassesThrough()
    {
        g_boSayMsgControl = false;
        g_boFilterSayMsg = false;
        string msg = "自由发言";
        Assert.False(_ctx.FilterSayMsg(ref msg));
        Assert.Equal("自由发言", msg);
    }

    [Fact]
    public void FilterSayMsg_DisableWindow_BlocksAndWarns()
    {
        g_boSayMsgControl = true;
        g_sDisableSayMsg = "禁止聊天";
        _ctx.dwDisableSayMsgTick = MyGetTickCount() + 60_000;    // 仍在禁言期

        string msg = "试图发言";
        Assert.True(_ctx.FilterSayMsg(ref msg));
        Assert.Equal("", msg);                                   // :3307 清空
        Assert.Single(_t.Sent);                                  // SendWarnMsg
    }

    [Fact]
    public void FilterSayMsg_SayTooFastBeyondMaxCount_TriggersDisableWindow()
    {
        g_boSayMsgControl = true;
        g_dwSayTime = 10_000;
        g_dwSayMaxCount = 2;
        g_dwSayDisableTime = 30;
        g_sDisableSayMsgBegin = "由于您说话太快，%d秒内禁止聊天！！！";

        _ctx.dwSayMsgTick = MyGetTickCount();
        _ctx.dwSayMsgCount = 2;                                   // 已达上限

        string msg = "第三句";
        Assert.True(_ctx.FilterSayMsg(ref msg));
        Assert.Equal("", msg);
        Assert.True(_ctx.dwDisableSayMsgTick > MyGetTickCount());
    }

    [Fact]
    public void FilterSayMsg_OutsideSayTimeWindow_ResetsCounter()
    {
        g_boSayMsgControl = true;
        g_dwSayTime = 100;
        g_dwSayMaxCount = 5;
        _ctx.dwSayMsgTick = MyGetTickCount() - 10_000;             // 远超窗口
        _ctx.dwSayMsgCount = 4;

        string msg = "hi";
        Assert.False(_ctx.FilterSayMsg(ref msg));
        Assert.Equal(0u, _ctx.dwSayMsgCount);                     // :3328 归零（不再自增）
    }

    [Fact]
    public void FilterSayMsg_MaxLenTruncatesByAnsiBytes()
    {
        g_boSayMsgControl = true;
        g_dwSayTime = 0;
        g_dwSayMaxLen = 4;                                        // 4 个 GBK 字节 = 2 个汉字
        _ctx.dwSayMsgTick = MyGetTickCount();

        string msg = "一二三四";
        _ctx.FilterSayMsg(ref msg);
        Assert.Equal("一二", msg);
    }

    [Fact]
    public void FilterSayMsg_AllBlockMode_ReplacesWithWarnText()
    {
        g_boSayMsgControl = false;
        g_boFilterSayMsg = true;
        g_FilterSayMsgMode = TFilterSayMsgMode.fsmmAllBlock;
        g_WarnSayMsg = "含非法字符";
        g_WordFilterList.Clear();
        g_WordFilterList.Add("外挂");

        string msg = "我卖外挂";
        Assert.False(_ctx.FilterSayMsg(ref msg));
        Assert.Equal("含非法字符", msg);                            // :3373
    }

    [Fact]
    public void FilterSayMsg_AllBlockModeWithEmptyWarnText_ReturnsTrue()
    {
        // 差异断言：:3374 `if Length(g_WarnSayMsg) = 0 then Result := True;`
        g_boSayMsgControl = false;
        g_boFilterSayMsg = true;
        g_FilterSayMsgMode = TFilterSayMsgMode.fsmmAllBlock;
        g_WarnSayMsg = "";
        g_WordFilterList.Clear();
        g_WordFilterList.Add("外挂");

        string msg = "我卖外挂";
        Assert.True(_ctx.FilterSayMsg(ref msg));
        Assert.Equal("", msg);
    }

    [Fact]
    public void FilterSayMsg_SelfBlockMode_ReplacesWithStarsOfAnsiLength()
    {
        g_boSayMsgControl = false;
        g_boFilterSayMsg = true;
        g_FilterSayMsgMode = TFilterSayMsgMode.fsmmSelfBolck;
        g_sReplaceWord = '*';
        g_WordFilterList.Clear();
        g_WordFilterList.Add("外挂");                              // GBK 4 字节

        string msg = "我卖外挂啦";
        Assert.False(_ctx.FilterSayMsg(ref msg));
        Assert.Equal("我卖****啦", msg);                            // :3379-3380
    }

    [Fact]
    public void FilterSayMsg_DisMsgMode_EmptiesMessageAndReturnsTrue()
    {
        g_boSayMsgControl = false;
        g_boFilterSayMsg = true;
        g_FilterSayMsgMode = TFilterSayMsgMode.fsmmDisMsg;
        g_WordFilterList.Clear();
        g_WordFilterList.Add("bad");

        string msg = "a bad word";
        Assert.True(_ctx.FilterSayMsg(ref msg));
        Assert.Equal("", msg);
    }

    [Fact]
    public void FilterSayMsg_CloseMode_ClosesContext()
    {
        g_boSayMsgControl = false;
        g_boFilterSayMsg = true;
        g_FilterSayMsgMode = TFilterSayMsgMode.fsmmClose;
        g_WordFilterList.Clear();
        g_WordFilterList.Add("bad");

        string msg = "bad";
        Assert.True(_ctx.FilterSayMsg(ref msg));
        Assert.Equal(1, _t.CloseCount);
    }

    [Fact]
    public void FilterSayMsg_DisMsgOrSysMode_SendsWarnOnlyWhenTextPresent()
    {
        g_boSayMsgControl = false;
        g_boFilterSayMsg = true;
        g_FilterSayMsgMode = TFilterSayMsgMode.fsmmDisMsgorSys;
        g_WarnSayMsg = "警告文本";
        g_WordFilterList.Clear();
        g_WordFilterList.Add("bad");

        string msg = "bad";
        Assert.True(_ctx.FilterSayMsg(ref msg));
        Assert.Single(_t.Sent);

        _t.Sent.Clear();
        g_WarnSayMsg = "";
        msg = "bad";
        Assert.True(_ctx.FilterSayMsg(ref msg));
        Assert.Empty(_t.Sent);                                     // 空文本不发
    }

    [Fact]
    public void FilterSayMsg_FilterModeClose_DoesNotTriggerScriptCallback()
    {
        // 差异断言（:3359-3360）：`g_FilterSayMsgMode <> fsmmClose` 才回调 M2
        _tcp.Sent.Clear();
        g_boSayMsgControl = false;
        g_boFilterSayMsg = true;
        g_boFilterSayTriggerScript = true;
        g_FilterSayMsgMode = TFilterSayMsgMode.fsmmClose;
        g_WordFilterList.Clear();
        g_WordFilterList.Add("bad");

        string msg = "bad";
        _ctx.FilterSayMsg(ref msg);
        Assert.Empty(_tcp.Sent);                                   // Close 模式不触发脚本

        g_FilterSayMsgMode = TFilterSayMsgMode.fsmmDisMsg;
        msg = "bad";
        _ctx.FilterSayMsg(ref msg);
        Assert.Single(_tcp.Sent);
        Assert.Equal(GM_DATA, _tcp.Sent[0].Ident);
    }

    // ---- AddServerMsg / AddServerText / GetRunGate（原文 9747-9844）----

    [Fact]
    public void GetRunGate_WithoutOwner_ReturnsNull()
    {
        var ctx = new TMirClientContext(new TIocpCore(), 0);
        Assert.Null(ctx.GetRunGate());
    }

    [Fact]
    public void GetRunGate_WithTcpServerOwner_ReturnsBindObject()
    {
        Assert.Same(_runGate, _ctx.GetRunGate());
    }

    [Fact]
    public void GetRunGate_WithForeignOwner_ReturnsNull()
    {
        var core = new TIocpCore { Owner = "not a server" };
        var ctx = new TMirClientContext(core, 0);
        Assert.Null(ctx.GetRunGate());
    }

    [Fact]
    public void AddServerMsg_AccumulatesEncodedFrame()
    {
        _ctx.AddServerMsg(TDefaultMessage.Make(SM_SYSMESSAGE, 0, 0, 0, 0), null, 0);
        Assert.True(_ctx.ServerMsgStrProbe.Length > 0);
    }

    [Fact]
    public void AddServerMsg_WhenClosing_IsIgnored()
    {
        _ctx.IsPostedCloseQuest = true;
        _ctx.AddServerMsg(TDefaultMessage.Make(1, 0, 0, 0, 0), null, 0);
        Assert.Equal(0, _ctx.ServerMsgStrProbe.Length);
    }

    [Fact]
    public void AddServerText_AccumulatesRawBytes()
    {
        _ctx.AddServerText(new byte[] { 1, 2, 3 });
        Assert.Equal(3, _ctx.ServerMsgStrProbe.Length);
    }

    [Fact]
    public void AddServerMsg_OverAccumulateLimit_ClosesContextSocket()
    {
        // 原文 :9774 `Length(FServerMsgStr) + Length(S) >= g_dwClientAccumulateMaxSize shl 10`
        // 超限走 :9792 CloseContextSocket(ERROR_SUCCESS, cfOther) —— 注意它**不是** Close()，
        // 所以传输层接缝的 Close 不会被调用，只有上下文被标记关闭（差异断言）。
        g_dwClientAccumulateMaxSize = 1;                     // 1KB 上限
        _ctx.AddServerText(new byte[2048]);                  // 直接超限

        Assert.True(_ctx.IsClosed);
        Assert.Equal(0, _t.CloseCount);                      // Close() 未被调用
    }

    [Fact]
    public void AddServerMsg_UnderLimit_DoesNotClose()
    {
        g_dwClientAccumulateMaxSize = 64;
        _ctx.AddServerText(new byte[8]);
        Assert.False(_ctx.IsClosed);
        Assert.Equal(8, _ctx.ServerMsgStrProbe.Length);
    }

    // ---- CloseContextSocket / DoConnect / DoDisconnect（原文 9846-10249）----

    [Fact]
    public void DoConnect_AddsSelfToOnlineListAndSendsGmOpen()
    {
        _tcp.Sent.Clear();
        _ctx.InvokeDoConnect();

        Assert.Equal(1, _runGate.OnlineUser.Count);
        Assert.Equal(1, _runGate.nMaxOnlineUserCount);
        Assert.Equal(0, _ctx.nUserListIndex);
        Assert.Contains(_tcp.Sent, s => s.Ident == GM_OPEN);
    }

    [Fact]
    public void DoConnect_OpenCheckClient_SendsCheckCode()
    {
        g_boOpenCheckClient = true;
        GateShareSeam.RandomSink = r => 12345;
        _tcp.Sent.Clear();

        _ctx.InvokeDoConnect();

        Assert.True(_ctx.boSendCheckCode);
        Assert.Equal(12345, _ctx.dwSendCheckCode);
        Assert.NotEmpty(_t.Sent);                            // SM_SENDRUNGATE_CHECKCODE 落到 PostSendBuffer
    }

    [Fact]
    public void DoConnect_WithoutRunGate_DoesNothing()
    {
        var ctx = new TMirClientContext(new TIocpCore(), 0);
        ctx.InvokeDoConnect();
        Assert.Empty(_tcp.Sent);
    }

    [Fact]
    public void DoDisconnect_SendsGmCloseAndLogsAddress()
    {
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        _ctx.nSessionID = 11;
        _ctx.nUserListIndex = 2;
        _tcp.Sent.Clear();

        _ctx.InvokeDoDisconnect(0x99);

        Assert.Single(_tcp.Sent);
        Assert.Equal(GM_CLOSE, _tcp.Sent[0].Ident);
        Assert.Equal((ushort)11, _tcp.Sent[0].SockIdx);
        Assert.Contains(logs, m => m.StartsWith("断开连接: "));
    }

    [Fact]
    public void DoDisconnect_DelayedCloseMode_SendsGmDelayCloseWithSeconds()
    {
        _ctx.boDelayClientClose = true;
        _ctx.nClientCloseDelay = 7;
        _ctx.boFirstClientQueryBagItems = true;
        _tcp.Sent.Clear();

        _ctx.InvokeDoDisconnect(0x99);

        Assert.Equal(GM_DELAY_CLOSE, _tcp.Sent[0].Ident);
        Assert.Equal(sizeof(int), _tcp.Sent[0].Data.Length);
        Assert.Equal(7, BitConverter.ToInt32(_tcp.Sent[0].Data, 0));
    }

    [Fact]
    public void DoDisconnect_ClearsMacCounter()
    {
        g_LoginMACPlayerList.AddObject("MAC-1", 2);
        _ctx.sMachineID = "MAC-1";

        _ctx.InvokeDoDisconnect(0);

        Assert.Equal(1, (int)g_LoginMACPlayerList.GetObject(0));    // :10072 递减
    }

    [Fact]
    public void DoDisconnect_MacCounterAtOne_RemovesEntry()
    {
        g_LoginMACPlayerList.AddObject("MAC-2", 1);
        _ctx.sMachineID = "MAC-2";

        _ctx.InvokeDoDisconnect(0);

        Assert.Equal(-1, g_LoginMACPlayerList.IndexOf("MAC-2"));     // :10077
    }

    [Fact]
    public void DoDisconnect_CurrIpList_DecrementsCount()
    {
        TAddressInfo info = g_CurrIPList.Add("1.2.3.4");
        info.nCount = 2;

        _ctx.InvokeDoDisconnect(0);

        Assert.Equal(1, info.nCount);
    }

    [Fact]
    public void DoDisconnect_CurrIpListCountReachesZero_RemovesEntry()
    {
        TAddressInfo info = g_CurrIPList.Add("1.2.3.4");
        info.nCount = 1;

        _ctx.InvokeDoDisconnect(0);

        Assert.Equal(0, g_CurrIPList.Count);
    }

    [Fact]
    public void CloseContextSocket_RemovesFromOnlineList()
    {
        _runGate.OnlineUser.Add(_ctx);
        Assert.Equal(1, _runGate.OnlineUser.Count);

        _ctx.InvokeCloseContextSocket(0, TCloseFrom.cfOther);

        Assert.Equal(0, _runGate.OnlineUser.Count);
    }

    [Fact]
    public void CloseContextSocket_WithError_LogsCloseSource()
    {
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        _runGate.OnlineUser.Add(_ctx);

        _ctx.InvokeCloseContextSocket(10054, TCloseFrom.cfPostWSASendCache1);

        Assert.Contains(logs, m => m.Contains("PostWSASendCache1"));
    }

    [Fact]
    public void CloseContextSocket_UnknownCloseFrom_UsesChineseNone()
    {
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        _runGate.OnlineUser.Add(_ctx);

        _ctx.InvokeCloseContextSocket(10054, TCloseFrom.cfOther);

        Assert.Contains(logs, m => m.Contains("来源:无"));
    }

    [Fact]
    public void DelayClose_ArmTickAndFlag()
    {
        _ctx.DelayClose(250);
        Assert.True(_ctx.boDelayClose);
        Assert.True(_ctx.dwDelayCloseTick >= MyGetTickCount());
    }

    // ---- 消息堆积拦截（原文 2947-3020 的 :3006-3019）----

    [Fact]
    public void ProcessClientMessage_TooManyPackets_ClosesConnection()
    {
        g_boKickOverPacketSize = true;
        g_nMaxClientPacketCount = 3;
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);

        for (int i = 0; i < 3; i++)
            _ctx.ProcessClientMessage(TDefaultMessage.Make(3011, 0, 0, 0, 0), Array.Empty<byte>());

        Assert.Equal(1, _t.CloseCount);
        Assert.Contains(logs, m => m.Contains("包堆积太多"));
    }

    [Fact]
    public void ProcessClientMessage_TooManyPacketsButKickDisabled_KeepsConnection()
    {
        g_boKickOverPacketSize = false;
        g_nMaxClientPacketCount = 1;
        for (int i = 0; i < 3; i++)
            _ctx.ProcessClientMessage(TDefaultMessage.Make(3011, 0, 0, 0, 0), Array.Empty<byte>());

        Assert.Equal(0, _t.CloseCount);
    }
}
