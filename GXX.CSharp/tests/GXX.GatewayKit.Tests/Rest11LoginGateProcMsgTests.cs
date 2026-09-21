using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GXX.GatewayKit.Rest11;
using Xunit;

namespace GXX.GatewayKit.Tests;

public class Rest11LoginGateProcMsgTests : IDisposable
{
    private readonly string _dir;
    private readonly Rest11LoginGateConfig _cfg;
    private uint _now = 50_000;

    public Rest11LoginGateProcMsgTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "rest11pm-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        _cfg = new Rest11LoginGateConfig(Path.Combine(_dir, "c.ini")) { m_nClientTimeOutTime = 10_000 };
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, true); } catch { }
    }

    private Rest11ProcMsgThread NewThread(out FakeEnforcementChannel ch)
    {
        ch = new FakeEnforcementChannel();
        return new Rest11ProcMsgThread { TickCount = () => _now };
    }

    // =====================================================================================
    // FuncForComm.pas:99-178 构造 / 增删查
    // =====================================================================================
    [Fact]
    public void Constructor_SetsIntervalAndEnabled()
    {
        var t = new Rest11ProcMsgThread();
        Assert.Equal(1, t.Interval);     // :106 Self.Interval := 1
        Assert.True(t.Enabled);          // :107 Self.Enabled := True
        Assert.Equal(0, t.m_nProcIdx);   // :103
    }

    [Fact]
    public void AddAndDelSession_AreIdempotentPerReference()
    {
        var t = new Rest11ProcMsgThread();
        var s = new FakeSession { Socket = 5 };
        t.AddSession(s);
        t.AddSession(s);
        Assert.Equal(2, t.m_xUserList.Count);

        t.DelSession(s);                 // IndexOf 只删第一个匹配（:145-148）
        Assert.Single(t.m_xUserList);
        t.DelSession(s);
        Assert.Empty(t.m_xUserList);
        t.DelSession(s);                 // :146 i < 0 ⇒ 不删
        Assert.Empty(t.m_xUserList);
    }

    [Fact]
    public void GetSession_MatchesBySocket_AndRejectsInvalidSocket()
    {
        var t = new Rest11ProcMsgThread();
        var a = new FakeSession { Socket = 7 };
        var b = new FakeSession { Socket = 8 };
        t.AddSession(a);
        t.AddSession(b);

        Assert.Same(a, t.GetSession(7));
        Assert.Same(b, t.GetSession(8));
        Assert.Null(t.GetSession(9));
        Assert.Null(t.GetSession(-1));   // :161 nSock = INVALID_SOCKET ⇒ 直接返回 nil
    }

    // =====================================================================================
    // FuncForComm.pas:180-257 Run —— 快照 + 三条踢线路径
    // =====================================================================================
    [Fact]
    public void Run_EarlyExitWhenServiceNotStarted()
    {
        var t = NewThread(out var ch);
        ch.ServiceStarted = false;
        var s = new FakeSession { Socket = 1, dwClientTimeOutTick = 0 };
        t.AddSession(s);
        t.Run(ch, _cfg);
        Assert.Empty(ch.FreeSocketCalls);
        Assert.Empty(t.m_xTempUserList);
    }

    [Fact]
    public void Run_SkipsSessionsWithoutActiveGameServer()
    {
        var t = NewThread(out var ch);
        var s = new FakeSession { Socket = 1, LastGameSvrActive = false, dwClientTimeOutTick = 0 };
        t.AddSession(s);
        t.Run(ch, _cfg);
        Assert.Empty(ch.FreeSocketCalls);
        Assert.Single(t.m_xTempUserList);   // :191-202 快照无条件复制
    }

    [Fact]
    public void Run_DelayCloseExpired_FlagsAndFreesWithoutBlocking()
    {
        var t = NewThread(out var ch);
        var s = new FakeSession
        {
            Socket = 10,
            HandleLogin = 1,                 // < 3 才进入该分支（:211）
            LastGameSvrActive = true,
            IsDelayClose = true,
            dwDelayCloseTick = _now - 1      // 已到期
        };
        t.AddSession(s);
        t.Run(ch, _cfg);

        Assert.True(s.KickFlag);             // :215
        Assert.False(s.IsDelayClose);        // :216
        Assert.Equal(new[] { 10 }, ch.FreeSocketCalls);   // :217
        Assert.Empty(ch.TempBlockCalls);     // 该分支**不** BlockUser
        Assert.Empty(ch.OutOfConnectionCalls);
    }

    [Fact]
    public void Run_DelayCloseNotYetExpired_FallsThroughToTimeoutCheck()
    {
        var t = NewThread(out var ch);
        var s = new FakeSession
        {
            Socket = 11,
            HandleLogin = 1,
            LastGameSvrActive = true,
            IsDelayClose = true,
            dwDelayCloseTick = _now + 1_000,  // 未到期
            dwClientTimeOutTick = _now         // 也没超时
        };
        t.AddSession(s);
        t.Run(ch, _cfg);

        Assert.False(s.KickFlag);
        Assert.True(s.IsDelayClose);
        Assert.Empty(ch.FreeSocketCalls);
    }

    [Fact]
    public void Run_ClientTimeOut_KicksBlocksAndRefreshesTick()
    {
        var t = NewThread(out var ch);
        var s = new FakeSession
        {
            Socket = 12,
            IPText = "10.0.0.12",
            SvrObject = 3,
            HandleLogin = 2,
            LastGameSvrActive = true,
            dwClientTimeOutTick = _now - 10_001     // 超过 m_nClientTimeOutTime = 10000
        };
        t.AddSession(s);
        t.Run(ch, _cfg);

        Assert.Equal(_now, s.dwClientTimeOutTick);                     // :227 先刷新 tick
        Assert.Equal(new[] { 12 }, ch.OutOfConnectionCalls.Select(c => c.ident).ToArray()); // :228
        Assert.Equal(3, ch.OutOfConnectionCalls[0].nSvrObject);
        Assert.True(s.KickFlag);                                        // :229
        Assert.Single(ch.Logs);                                         // :233 'Client Connect Time Out: '
        Assert.Contains("10.0.0.12", ch.Logs[0]);
        // BlockUser 按 m_tBlockIPMethod 分派，默认 mDisconnect ⇒ 不封禁
        Assert.Empty(ch.TempBlockCalls);
        Assert.Empty(ch.BlockListCalls);
    }

    [Fact]
    public void Run_ClientTimeOut_BlocksWhenMethodIsBlock()
    {
        _cfg.m_tBlockIPMethod = (int)Rest11TBlockIPMethod.mBlock;
        var t = NewThread(out var ch);
        var s = new FakeSession
        {
            Socket = 13,
            IPAddr = 0x0A00000D,
            HandleLogin = 2,
            LastGameSvrActive = true,
            dwClientTimeOutTick = _now - 10_001
        };
        t.AddSession(s);
        t.Run(ch, _cfg);
        Assert.Equal(new[] { 0x0A00000D }, ch.TempBlockCalls);
    }

    [Fact]
    public void Run_SkipsTimeOutBranchWhenHandleLoginAtLeastThree()
    {
        var t = NewThread(out var ch);
        var s = new FakeSession
        {
            Socket = 14,
            HandleLogin = 3,                 // :211 m_fHandleLogin < 3 为假
            LastGameSvrActive = true,
            dwClientTimeOutTick = _now - 999_999
        };
        t.AddSession(s);
        t.Run(ch, _cfg);
        Assert.Empty(ch.FreeSocketCalls);
        Assert.Empty(ch.OutOfConnectionCalls);
        Assert.False(s.KickFlag);
    }

    [Fact]
    public void Run_KickFlaggedSession_SecondTimeOutFreesSocketWithoutBlocking()
    {
        var t = NewThread(out var ch);
        var s = new FakeSession
        {
            Socket = 15,
            IPText = "10.0.0.15",
            KickFlag = true,                 // :209 为真 ⇒ 走 else 分支（:238-250）
            HandleLogin = 5,
            LastGameSvrActive = true,
            dwClientTimeOutTick = _now - 10_001
        };
        t.AddSession(s);
        t.Run(ch, _cfg);

        Assert.Equal(new[] { 15 }, ch.FreeSocketCalls);      // :248
        Assert.Single(ch.Logs);
        Assert.Contains("Client Connect Time Out 2", ch.Logs[0]);   // :244
        Assert.Empty(ch.TempBlockCalls);
        Assert.Empty(ch.OutOfConnectionCalls);
    }

    [Fact]
    public void Run_LogLevelGated()
    {
        var t = NewThread(out var ch);
        ch.CheckLevelFn = _ => false;        // :231-234 CheckLevel(5) 为假 ⇒ 不记日志
        var s = new FakeSession
        {
            Socket = 16,
            KickFlag = true,
            HandleLogin = 5,
            LastGameSvrActive = true,
            dwClientTimeOutTick = _now - 10_001
        };
        t.AddSession(s);
        t.Run(ch, _cfg);
        Assert.Empty(ch.Logs);
        Assert.Single(ch.FreeSocketCalls);
    }

    // =====================================================================================
    // FuncForComm.pas:261-356 TAddressListEx
    // =====================================================================================
    [Fact]
    public void TAddressListEx_AddZeroesAllFields_AndFindMatchesByInetAddr()
    {
        var l = new TAddressListEx();
        TAddressInfo a = l.Add("10.1.1.1");
        Assert.Equal(Rest11LoginGateNet.InetAddr("10.1.1.1"), a.nIPaddr);
        Assert.Equal("10.1.1.1", a.sIPaddr);
        Assert.Equal(0, a.nCount);          // :318 ZeroMemory ⇒ 全字段 0（**不是 1**）
        Assert.Equal(1, l.GetCount());

        Assert.Same(a, l.Find("10.1.1.1"));
        Assert.Null(l.Find("10.1.1.2"));
    }

    [Fact]
    public void TAddressListEx_GetItemsIsBoundsChecked()
    {
        var l = new TAddressListEx();
        l.Add("10.1.1.1");
        Assert.NotNull(l.GetItems(0));
        Assert.Null(l.GetItems(-1));        // :294 下界
        Assert.Null(l.GetItems(1));         // :294 上界
    }

    [Fact]
    public void TAddressListEx_DeleteByReference_AndClear()
    {
        var l = new TAddressListEx();
        var a = l.Add("10.1.1.1");
        var b = l.Add("10.1.1.2");
        l.Delete(a);                        // :349 引用相等
        Assert.Equal(1, l.GetCount());
        Assert.Same(b, l.GetItems(0));

        l.Delete(new TAddressInfo());       // 非表内引用 ⇒ 不删
        Assert.Equal(1, l.GetCount());

        l.Clear();
        Assert.Equal(0, l.GetCount());
    }

    // =====================================================================================
    // FuncForComm.pas:507-561 ShowThreadInfo / :537-554 字节格式化 / :529-533 状态文本
    // =====================================================================================
    private sealed class FakeServerInfo : IRest11ServerInfo
    {
        public bool ClientActive { get; set; } = true;
        public string ServerIP { get; set; } = "";
        public int Port { get; set; }
        public Rest11TSockThreadStutas SockThreadStutas { get; set; }
        public uint SendBytes { get; set; }
        public uint RecvBytes { get; set; }
    }

    private sealed class FakeThreadInfoSource : IRest11ThreadInfoSource
    {
        public bool Active { get; set; } = true;
        public int InUseBlock { get; set; } = 3;
        public int MaxInUseBlock { get; set; } = 10;
        public readonly List<IRest11ServerInfo?> Servers = new();
        public readonly Dictionary<(int col, int row), string> Cells = new();
        public string StatusPanel = "";
        public int ServerCount => Servers.Count;
        public IRest11ServerInfo? GetServerInfo(int index) => Servers[index];
        public void SetGridCell(int col, int row, string value) => Cells[(col, row)] = value;
        public void SetStatusPanel(string text) => StatusPanel = text;
    }

    [Theory]
    [InlineData(0u, "↑0B")]
    [InlineData(1024u, "↑1024B")]              // 原文是 > 1024（不是 >=）
    [InlineData(1025u, "↑1.00K")]
    [InlineData(1024000u, "↑1000.00K")]        // 原文是 > 1024*1000
    [InlineData(1024001u, "↑1.00M")]
    public void FormatSendBytes_MatchesOriginalThresholds(uint bytes, string expected)
        => Assert.Equal(expected, Rest11LoginGateProcMsgFormat.FormatSendBytes(bytes));

    [Theory]
    [InlineData(0u, "↓0B")]
    [InlineData(1025u, "↓1.00K")]
    [InlineData(2048000u, "↓2.00M")]
    public void FormatRecvBytes_MatchesOriginalThresholds(uint bytes, string expected)
        => Assert.Equal(expected, Rest11LoginGateProcMsgFormat.FormatRecvBytes(bytes));

    [Theory]
    [InlineData(Rest11TSockThreadStutas.stConnecting, "连接中..")]
    [InlineData(Rest11TSockThreadStutas.stConnected, "已连接")]
    [InlineData(Rest11TSockThreadStutas.stTimeOut, "超时")]
    public void StatusText_MatchesOriginal(Rest11TSockThreadStutas st, string expected)
        => Assert.Equal(expected, Rest11LoginGateProcMsgFormat.StatusText(st));

    [Fact]
    public void ShowThreadInfo_WritesRowStartingAtOne_AndResetsByteCounters()
    {
        var src = new FakeThreadInfoSource();
        var s0 = new FakeServerInfo
        {
            ServerIP = "10.0.0.1",
            Port = 5500,
            SockThreadStutas = Rest11TSockThreadStutas.stConnected,
            SendBytes = 2048,
            RecvBytes = 512
        };
        var s1 = new FakeServerInfo { ClientActive = false };   // :525 Continue
        src.Servers.Add(s0);
        src.Servers.Add(s1);

        Rest11LoginGateProcMsgFormat.ShowThreadInfo(src);

        Assert.Equal("10.0.0.1", src.Cells[(0, 1)]);            // :519 nRow 从 1 开始
        Assert.Equal("5500", src.Cells[(1, 1)]);
        Assert.Equal("已连接", src.Cells[(2, 1)]);
        Assert.Equal("↑2.00K  ↓512B", src.Cells[(3, 1)]);
        Assert.Equal("连接: 3/10", src.StatusPanel);            // :535
        Assert.Equal(0u, s0.SendBytes);                         // :543 读取后清零
        Assert.Equal(0u, s0.RecvBytes);                         // :554
        Assert.False(src.Cells.ContainsKey((0, 2)));            // 非 Active 的行被 Continue 跳过
    }

    [Fact]
    public void ShowThreadInfo_SkipsEntireLoopWhenAcceptExNotActive()
    {
        var src = new FakeThreadInfoSource { Active = false };
        src.Servers.Add(new FakeServerInfo { ServerIP = "1.1.1.1", Port = 1 });
        Rest11LoginGateProcMsgFormat.ShowThreadInfo(src);
        Assert.Empty(src.Cells);
        Assert.Equal("", src.StatusPanel);
    }

    // =====================================================================================
    // FuncForComm.pas:563-587 OnTimerProc
    // =====================================================================================
    [Fact]
    public void OnTimerProc_DispatchesAllFourTimerIds()
    {
        string hit = "";
        bool Handle(uint id) => Rest11LoginGateProcMsgFormat.OnTimerProc(
            id,
            () => hit = "start", () => hit = "stop", () => hit = "keep", () => hit = "info");

        Assert.True(Handle(Rest11LoginGateConstants._IDM_TIMER_STARTSERVICE));
        Assert.Equal("start", hit);
        Assert.True(Handle(Rest11LoginGateConstants._IDM_TIMER_STOPSERVICE));
        Assert.Equal("stop", hit);
        Assert.True(Handle(Rest11LoginGateConstants._IDM_TIMER_KEEP_ALIVE));
        Assert.Equal("keep", hit);
        Assert.True(Handle(Rest11LoginGateConstants._IDM_TIMER_THREAD_INFO));
        Assert.Equal("info", hit);
        Assert.False(Handle(999999));    // 未登记 id ⇒ 原文 case 无 else，什么都不做
    }
}
