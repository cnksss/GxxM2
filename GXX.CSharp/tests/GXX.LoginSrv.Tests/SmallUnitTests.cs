using System;
using GXX.Core.Rtl;
using GXX.LoginSrv;
using Xunit;

namespace GXX.LoginSrv.Tests;

/// <summary>FAccountView.pas TFrmAccountView 1:1 测试。</summary>
public sealed class FAccountViewTests
{
    [StaFact]
    public void FormCreate_ControlsMatchDfm()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmAccountView();
            Assert.Equal("充值记录", f.Text);
            Assert.Equal(402, f.ClientSize.Width);
            Assert.Equal(456, f.ClientSize.Height);
            Assert.Equal(8, f.EdFindID.Left);
            Assert.Equal(408, f.EdFindID.Top);
            Assert.Equal(193, f.EdFindID.Width);
            Assert.Equal(208, f.EdFindIP.Left);
            Assert.Equal(193, f.ListBox1.Width);
            Assert.Equal(193, f.ListBox2.Width);
            Assert.Equal(2, f.ListBox1.TabIndex);
            Assert.Equal(3, f.ListBox2.TabIndex);
        });
    }

    [StaTheory]
    [InlineData('x')]
    [InlineData('\t')]
    public void EdFindIDKeyPress_NonEnter_KeepsSelection(char key)
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmAccountView();
            f.ListBox1.Items.Add("alice");
            f.EdFindID.Text = "alice";
            char k = key;
            f.EdFindIDKeyPress(f, ref k);
            Assert.Equal(-1, f.ListBox1.SelectedIndex);
        });
    }

    [StaFact]
    public void EdFindIDKeyPress_Enter_SelectsMatchingItem()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmAccountView();
            f.ListBox1.Items.Add("alice");
            f.ListBox1.Items.Add("bob");
            f.EdFindID.Text = "bob";
            char k = '\r';
            f.EdFindIDKeyPress(f, ref k);
            Assert.Equal(1, f.ListBox1.SelectedIndex);
        });
    }

    /// <summary>★ 原文循环不 break：重复项时最终停在**最后一个**匹配项（差异断言）。</summary>
    [StaFact]
    public void EdFindIDKeyPress_Duplicates_SelectsLastMatch()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmAccountView();
            f.ListBox1.Items.Add("alice");
            f.ListBox1.Items.Add("bob");
            f.ListBox1.Items.Add("alice");
            f.EdFindID.Text = "alice";
            char k = '\r';
            f.EdFindIDKeyPress(f, ref k);
            Assert.Equal(2, f.ListBox1.SelectedIndex);
        });
    }

    [StaFact]
    public void EdFindIDKeyPress_NoMatch_SelectionUnchanged()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmAccountView();
            f.ListBox1.Items.Add("alice");
            f.ListBox1.SelectedIndex = 0;
            f.EdFindID.Text = "zzz";
            char k = '\r';
            f.EdFindIDKeyPress(f, ref k);
            Assert.Equal(0, f.ListBox1.SelectedIndex);
        });
    }

    [StaFact]
    public void EdFindIPKeyPress_Enter_SelectsLastMatchingItem()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmAccountView();
            f.ListBox2.Items.Add("1.1.1.1");
            f.ListBox2.Items.Add("2.2.2.2");
            f.ListBox2.Items.Add("1.1.1.1");
            f.EdFindIP.Text = "1.1.1.1";
            char k = '\r';
            f.EdFindIPKeyPress(f, ref k);
            Assert.Equal(2, f.ListBox2.SelectedIndex);
        });
    }

    [StaFact]
    public void EdFindIPKeyPress_NonEnter_KeepsSelection()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmAccountView();
            f.ListBox2.Items.Add("1.1.1.1");
            f.EdFindIP.Text = "1.1.1.1";
            char k = 'a';
            f.EdFindIPKeyPress(f, ref k);
            Assert.Equal(-1, f.ListBox2.SelectedIndex);
        });
    }
}

/// <summary>MonSoc.pas TFrmMonSoc 1:1 测试。</summary>
public sealed class MonSocTests : IDisposable
{
    public MonSocTests() => LoginSrvShare.ResetForTests();

    public void Dispose() => LoginSrvShare.ResetForTests();

    [StaFact]
    public void FormCreate_DeactivatesMonSocket_AndSetsDfmGeometry()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmMonSoc();
            Assert.False(f.MonSocket.Active);
            Assert.Equal("FrmMonSoc", f.Text);
            Assert.Equal(194, f.ClientSize.Width);
            Assert.Equal(124, f.ClientSize.Height);
            Assert.Equal(5000, f.MonTimer.Interval);
            Assert.True(f.FormCreated);
        });
    }

    /// <summary>★ MonSoc.pas:47 第一行即 `exit;` → StartService 是空操作（死代码保留）。</summary>
    [StaFact]
    public void StartService_IsDeadCode_DoesNotTouchSocket()
    {
        StaRunner.New(() =>
        {
            LoginSrvShare.g_Config.sMonAddr = "9.9.9.9";
            LoginSrvShare.g_Config.nMonPort = 9999;
            using var f = new TFrmMonSoc();
            f.StartService();

            Assert.False(f.MonSocket.Active);
            Assert.Equal("0.0.0.0", f.MonSocket.Address);   // DFM 初值，未被 StartService 改写
            Assert.Equal(0, f.MonSocket.Port);
        });
    }

    private static TMsgServerInfo Server(string name, int index, int online, uint keepAliveTick)
        => new() { sServerName = name, nServerIndex = index, nOnlineCount = online, dwKeepAliveTick = keepAliveTick };

    [StaFact]
    public void MonTimerTimer_FormatsNormalAndTimeout()
    {
        StaRunner.New(() =>
        {
            uint now = DelphiRTL.GetTickCount();
            LoginSrvShare.FrmMasSoc.m_ServerList.Add(Server("srv1", 1, 10, now));
            LoginSrvShare.FrmMasSoc.m_ServerList.Add(Server("srv2", 2, 20, unchecked(now - 40000)));

            using var f = new TFrmMonSoc();
            var sock = (TServerSocketSeam)f.MonSocket;
            sock.AddConnection();
            f.MonTimerTimer(f);

            Assert.Equal("2;srv1/1/10/正常 ;srv2/2/20/超时 ;", sock.SentOf(0)[0]);
        });
    }

    /// <summary>★ 少于 30000 视为正常（严格小于）。边界取远离 30000 的值以避免计时抖动。</summary>
    [StaTheory]
    [InlineData(0u, "正常 ;")]
    [InlineData(1000u, "正常 ;")]
    [InlineData(28000u, "正常 ;")]
    [InlineData(31000u, "超时 ;")]
    [InlineData(60000u, "超时 ;")]
    public void MonTimerTimer_KeepAliveBoundary(uint elapsed, string expectedSuffix)
    {
        StaRunner.New(() =>
        {
            uint now = DelphiRTL.GetTickCount();
            LoginSrvShare.FrmMasSoc.m_ServerList.Add(Server("srv", 1, 5, unchecked(now - elapsed)));

            using var f = new TFrmMonSoc();
            var sock = (TServerSocketSeam)f.MonSocket;
            sock.AddConnection();
            f.MonTimerTimer(f);

            Assert.Equal("1;srv/1/5/" + expectedSuffix, sock.SentOf(0)[0]);
        });
    }

    /// <summary>空服务器名 → 整体写 '-/-/-/-;'（覆盖已有累积内容）。</summary>
    [StaFact]
    public void MonTimerTimer_EmptyServerName_WritesPlaceholder()
    {
        StaRunner.New(() =>
        {
            LoginSrvShare.FrmMasSoc.m_ServerList.Add(Server("", 0, 0, 0));
            using var f = new TFrmMonSoc();
            var sock = (TServerSocketSeam)f.MonSocket;
            sock.AddConnection();
            f.MonTimerTimer(f);
            Assert.Equal("1;-/-/-/-;", sock.SentOf(0)[0]);
        });
    }

    [StaFact]
    public void MonTimerTimer_NoServers_SendsZeroCount()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmMonSoc();
            var sock = (TServerSocketSeam)f.MonSocket;
            sock.AddConnection();
            f.MonTimerTimer(f);
            Assert.Equal("0;", sock.SentOf(0)[0]);
        });
    }

    [StaFact]
    public void MonTimerTimer_SendsToAllConnections_NoneIsNoOp()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmMonSoc();
            f.MonTimerTimer(f);   // 无连接 → 不抛异常

            var sock = (TServerSocketSeam)f.MonSocket;
            sock.AddConnection();
            sock.AddConnection();
            f.MonTimerTimer(f);
            Assert.Single(sock.SentOf(0));
            Assert.Single(sock.SentOf(1));
        });
    }

    [StaFact]
    public void MonSocketClientError_ZeroesErrorCodeAndCloses()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmMonSoc();
            var sock = (TServerSocketSeam)f.MonSocket;
            var conn = sock.AddConnection();
            int errorCode = 10054;

            f.MonSocketClientError(f, conn, ref errorCode);

            Assert.Equal(0, errorCode);
            Assert.True(conn.Closed);
        });
    }
}

/// <summary>LSShare.pas 最小接缝函数的 1:1 测试（BasicSet/窗体族依赖）。</summary>
public sealed class LoginSrvShareTests : IDisposable
{
    public LoginSrvShareTests() => LoginSrvShare.ResetForTests();

    public void Dispose() => LoginSrvShare.ResetForTests();

    [Fact]
    public void ConfigDefaults_MatchDelphiTypedConstant()
    {
        var c = LoginSrvShare.g_Config;
        Assert.Equal("127.0.0.1", c.sDBServer);
        Assert.Equal(16300, c.nDBSPort);
        Assert.Equal(16301, c.nFeePort);
        Assert.Equal(16301, c.nLogPort);
        Assert.Equal("0.0.0.0", c.sGateAddr);
        Assert.Equal(5500, c.nGatePort);
        Assert.Equal("0.0.0.0", c.sServerAddr);
        Assert.Equal(5600, c.nServerPort);
        Assert.Equal("0.0.0.0", c.sMonAddr);
        Assert.Equal(3000, c.nMonPort);
        Assert.Equal(0, c.nControlPort);
        Assert.Equal("bmm2", c.sControlPassword);
        Assert.True(c.boTestServer);
        Assert.True(c.boEnableMakingID);
        Assert.True(c.boEnableGetbackPassword);
        Assert.True(c.boUnLockAccount);
        Assert.False(c.boDynamicIPMode);
        Assert.False(c.boGetbackPasswordCheckAll);
        Assert.Equal(8, c.btLoginWaveValue);
        Assert.Equal(4, c.btOtherWaveValue);
        Assert.Equal(3, c.nRandomCodeErrorMaxCount);
        Assert.Equal(5, c.nRandomCodeRefreshMaxCount);
        Assert.Equal(3306, c.wDataSaveDBPort);
        Assert.False(c.boNewLoginDlg);
        Assert.True(c.boNewLoginInto);
        Assert.True(c.boNewLoginPhone);
        Assert.True(c.boNewLoginMustHasPhone);
        Assert.Equal(".\\DB\\", c.sIdDir);
        Assert.Equal(".\\Share\\", c.sWebLogDir);
        Assert.Equal(".\\FeedIDList.txt", c.sFeedIDList);
        Assert.Equal(".\\FeedIPList.txt", c.sFeedIPList);
        Assert.Equal(".\\CountLog\\", c.sCountLogDir);
        Assert.Equal(".\\ChrLog\\", c.sChrLogDir);
        Assert.All(c.boRandomCode, b => Assert.False(b));
    }

    [Theory]
    [InlineData(2024, 1, 2, 20240102)]
    [InlineData(1999, 12, 31, 19991231)]
    [InlineData(2000, 1, 1, 20000101)]
    public void Date2MyDate_EncodesYyyyMmDd(int y, int m, int d, int expected)
        => Assert.Equal(expected, LoginSrvShare.Date2MyDate(new DateTime(y, m, d)));

    [Theory]
    [InlineData(20240102)]
    [InlineData(19991231)]
    [InlineData(20000101)]
    public void MyDate2Date_RoundTrips(int packed)
    {
        DateTime dt = LoginSrvShare.MyDate2Date(packed);
        Assert.Equal(packed, LoginSrvShare.Date2MyDate(dt));
    }

    [Theory]
    [InlineData(10000000)]
    [InlineData(0)]
    [InlineData(-5)]
    public void MyDate2Date_NotGreaterThan10000000_ReturnsZero(int packed)
        => Assert.Equal(default, LoginSrvShare.MyDate2Date(packed));

    [Theory]
    [InlineData(1.0, 1)]
    [InlineData(1.5, 2)]
    [InlineData(0.0, 0)]
    [InlineData(-1.5, -1)]
    public void GetCodeMsgSize_MatchesDelphiTruncRule(double x, int expected)
        => Assert.Equal(expected, LoginSrvShare.GetCodeMsgSize(x));

    [Theory]
    [InlineData(10u, 20u, 10u)]
    [InlineData(20u, 10u, 4294967285u)]
    [InlineData(5u, 5u, 0u)]
    public void tickDiff_WrapsAround(uint start, uint end, uint expected)
        => Assert.Equal(expected, LoginSrvShare.tick_diff(start, end));

    /// <summary>★ 即使 nSpaceCount &lt; length，也至少追加一个空格（原文 Result := sStr + ' ' 在前）。</summary>
    [Theory]
    [InlineData("ab", 5, "ab    ")]
    [InlineData("", 3, "    ")]
    [InlineData("abcdef", 2, "abcdef ")]
    [InlineData("x", 1, "x ")]
    public void GenSpaceString_AlwaysAppendsAtLeastOneSpace(string s, int n, string expected)
        => Assert.Equal(expected, LoginSrvShare.GenSpaceString(s, n));

    [Fact]
    public void MainOutMessage_AppendsTimestampedLine()
    {
        LoginSrvShare.MainOutMessage("hello");
        Assert.Single(LoginSrvShare.g_MainMsgList.AsEnumerable());
        Assert.EndsWith("] hello", LoginSrvShare.g_MainMsgList[0], StringComparison.Ordinal);
        Assert.StartsWith("[", LoginSrvShare.g_MainMsgList[0], StringComparison.Ordinal);
    }
}
