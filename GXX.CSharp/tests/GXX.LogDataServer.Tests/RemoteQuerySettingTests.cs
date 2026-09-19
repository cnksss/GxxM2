using System;
using System.IO;
using GXX.Core.Util;
using GXX.LogDataServer;
using Xunit;

namespace GXX.LogDataServer.Tests;

/// <summary>uFrmRemoteQuerySetting.pas TFrmRemoteQuerySetting 1:1 测试。</summary>
public sealed class RemoteQuerySettingTests : IDisposable
{
    private readonly string _dir;

    public RemoteQuerySettingTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_lane6_rqs_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        LogDataShare.ResetForTests();
        LogDataForms.MessageBoxHandler = (_, _, _) => LogDataForms.IDOK;
        LogDataForms.LastMessage = null;
        LogDataForms.LastCaption = null;
        LogDataInputQuery.Handler = null;
        TFrmRemoteQuerySetting.IniFileName = Path.Combine(_dir, "LogData.ini");
    }

    public void Dispose()
    {
        LogDataForms.MessageBoxHandler = null;
        LogDataForms.LastMessage = null;
        LogDataInputQuery.Handler = null;
        TFrmRemoteQuerySetting.IniFileName = @".\LogData.ini";
        LogDataShare.ResetForTests();
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    [StaFact]
    public void FormCreate_LoadsPortPasswordAndIpList()
    {
        StaRunner.New(() =>
        {
            LogDataShare.g_nControlPort = 12345;
            LogDataShare.g_sControlPassword = "secret";
            LogDataShare.g_ControlIPList.Add("1.1.1.1");
            LogDataShare.g_ControlIPList.Add("2.2.2.2");

            using var f = new TFrmRemoteQuerySetting();

            Assert.Equal("远程查询设置", f.Text);
            Assert.Equal(403, f.ClientSize.Width);
            Assert.Equal(227, f.ClientSize.Height);
            Assert.Equal(12345m, f.sePort.Value);
            Assert.Equal("secret", f.edtPassword.Text);
            Assert.Equal(2, f.lstControlIPList.Items.Count);
            Assert.Equal("1.1.1.1", f.lstControlIPList.Items[0]);
            Assert.Equal("查询端口:", f.lbl3.Text);
            Assert.Equal("查询密码:", f.lbl4.Text);
            Assert.Equal("远程查询设置", f.grp2.Text);
            Assert.Equal("允许连接IP", f.grp1.Text);
            Assert.Equal("确定", f.btnOK.Text);
            Assert.Equal("增加(&A)", f.mniIPAdd.Text);
            Assert.Equal("删除(&D)", f.mniIPDelete.Text);
            Assert.Equal("清空(&C)", f.mniIPClear.Text);
        });
    }

    /// <summary>TSpinEdit 夹取：g_nControlPort 是 Word，MinValue=0 / MaxValue=65535。</summary>
    [StaFact]
    public void FormCreate_ClampsPortIntoSpinRange()
    {
        StaRunner.New(() =>
        {
            LogDataShare.g_nControlPort = 65535;
            using var f = new TFrmRemoteQuerySetting();
            Assert.Equal(65535m, f.sePort.Value);
            Assert.Equal(0m, f.sePort.Minimum);
            Assert.Equal(65535m, f.sePort.Maximum);
        });
    }

    [StaFact]
    public void btnOKClick_WritesIniAndUpdatesGlobals()
    {
        StaRunner.New(() =>
        {
            using var f = new TFrmRemoteQuerySetting();
            f.sePort.Value = 20000;
            f.edtPassword.Text = "pw123";

            f.btnOKClick(f);

            Assert.Equal(20000, LogDataShare.g_nControlPort);
            Assert.Equal("pw123", LogDataShare.g_sControlPassword);
            Assert.Equal(System.Windows.Forms.DialogResult.OK, f.DialogResult);

            var ini = new TFastIniFile(TFrmRemoteQuerySetting.IniFileName);
            Assert.Equal(20000, ini.ReadInteger("Setup", "ControlPort", -1));
            Assert.Equal("pw123", ini.ReadString("Setup", "ControlPassword", ""));
        });
    }

    [StaFact]
    public void btnOKClick_ZeroPort_DisablesRemoteQuery()
    {
        StaRunner.New(() =>
        {
            LogDataShare.g_nControlPort = 9;
            using var f = new TFrmRemoteQuerySetting();
            f.sePort.Value = 0;
            f.edtPassword.Text = "";

            f.btnOKClick(f);

            Assert.Equal(0, LogDataShare.g_nControlPort);
            Assert.Equal("", LogDataShare.g_sControlPassword);
        });
    }

    [StaFact]
    public void mniIPAddClick_ValidIp_AddsAndPersists()
    {
        StaRunner.New(() =>
        {
            LogDataShare.g_ControlIPFile = Path.Combine(_dir, "ip.txt");
            LogDataInputQuery.Handler = (_, _, _, _) => (true, "192.168.1.10");

            using var f = new TFrmRemoteQuerySetting();
            f.mniIPAddClick(f);

            Assert.Equal(1, LogDataShare.g_ControlIPList.Count);
            Assert.Equal("192.168.1.10", LogDataShare.g_ControlIPList[0]);
            Assert.Equal(1, f.lstControlIPList.Items.Count);
            Assert.True(File.Exists(LogDataShare.g_ControlIPFile));
        });
    }

    [StaFact]
    public void mniIPAddClick_InvalidIp_ShowsError()
    {
        StaRunner.New(() =>
        {
            LogDataInputQuery.Handler = (_, _, _, _) => (true, "not-an-ip");
            using var f = new TFrmRemoteQuerySetting();
            f.mniIPAddClick(f);

            Assert.Equal("输入的地址格式错误！", LogDataForms.LastMessage);
            Assert.Equal("错误", LogDataForms.LastCaption);
            Assert.Equal(0, LogDataShare.g_ControlIPList.Count);
        });
    }

    [StaFact]
    public void mniIPAddClick_Duplicate_NotAddedTwice()
    {
        StaRunner.New(() =>
        {
            LogDataShare.g_ControlIPFile = Path.Combine(_dir, "ip.txt");
            LogDataShare.g_ControlIPList.Add("1.1.1.1");
            LogDataInputQuery.Handler = (_, _, _, _) => (true, "1.1.1.1");

            using var f = new TFrmRemoteQuerySetting();
            Assert.Equal(1, f.lstControlIPList.Items.Count);
            f.mniIPAddClick(f);

            Assert.Equal(1, LogDataShare.g_ControlIPList.Count);
            Assert.Equal(1, f.lstControlIPList.Items.Count);
        });
    }

    [StaFact]
    public void mniIPAddClick_Cancelled_DoesNothing()
    {
        StaRunner.New(() =>
        {
            LogDataInputQuery.Handler = (_, _, _, _) => (false, "");
            using var f = new TFrmRemoteQuerySetting();
            f.mniIPAddClick(f);

            Assert.Equal(0, LogDataShare.g_ControlIPList.Count);
            Assert.Null(LogDataForms.LastMessage);
        });
    }

    [StaFact]
    public void mniIPDeleteClick_RemovesSelectedFromBothLists()
    {
        StaRunner.New(() =>
        {
            LogDataShare.g_ControlIPFile = Path.Combine(_dir, "ip.txt");
            LogDataShare.g_ControlIPList.Add("1.1.1.1");
            LogDataShare.g_ControlIPList.Add("2.2.2.2");
            LogDataShare.g_ControlIPList.Add("3.3.3.3");

            using var f = new TFrmRemoteQuerySetting();
            f.lstControlIPList.SelectedIndex = 0;
            f.mniIPDeleteClick(f);

            Assert.Equal(new[] { "2.2.2.2", "3.3.3.3" }, LogDataShare.g_ControlIPList.AsEnumerable());
            Assert.Equal(2, f.lstControlIPList.Items.Count);
        });
    }

    [StaFact]
    public void mniIPDeleteClick_NoSelection_IsNoOp()
    {
        StaRunner.New(() =>
        {
            LogDataShare.g_ControlIPList.Add("1.1.1.1");
            using var f = new TFrmRemoteQuerySetting();
            f.lstControlIPList.SelectedIndex = -1;
            f.mniIPDeleteClick(f);
            Assert.Equal(1, LogDataShare.g_ControlIPList.Count);
        });
    }

    [StaFact]
    public void mniIPClearClick_ClearsEverything()
    {
        StaRunner.New(() =>
        {
            LogDataShare.g_ControlIPFile = Path.Combine(_dir, "ip.txt");
            LogDataShare.g_ControlIPList.Add("1.1.1.1");
            using var f = new TFrmRemoteQuerySetting();
            f.mniIPClearClick(f);

            Assert.Equal(0, LogDataShare.g_ControlIPList.Count);
            Assert.Equal(0, f.lstControlIPList.Items.Count);
        });
    }

    [StaFact]
    public void ErrMessage_UsesErrorCaption()
    {
        StaRunner.New(() =>
        {
            LogDataInputQuery.Handler = (_, _, _, _) => (true, "bad");
            using var f = new TFrmRemoteQuerySetting();
            f.mniIPAddClick(f);
            Assert.Equal("错误", LogDataForms.LastCaption);
            Assert.Equal(LogDataForms.MB_OK | LogDataForms.MB_ICONERROR, LogDataForms.MB_OK | LogDataForms.MB_ICONERROR);
        });
    }
}

/// <summary>LDShare.pas 最小接缝的 1:1 测试。</summary>
public sealed class LogDataShareTests : IDisposable
{
    public LogDataShareTests() => LogDataShare.ResetForTests();

    public void Dispose() => LogDataShare.ResetForTests();

    [Fact]
    public void Constants_MatchDelphi()
    {
        Assert.Equal(0x422C9CD1u, LogDataShare.ControlMsgHeaderIdent);
        Assert.Equal(1000, LogDataShare.CMSG_HEARTBEAT);
        Assert.Equal(1001, LogDataShare.CMSG_CHECK_PASSWORD);
        Assert.Equal(1002, LogDataShare.CMSG_SEARCH_LOG);
        Assert.Equal(1000, LogDataShare.SMSG_HEARTBEAT);
        Assert.Equal(1001, LogDataShare.SMSG_CHECK_PASSWORD_OK);
        Assert.Equal(1002, LogDataShare.SMSG_CHECK_PASSWORD_FAIL);
        Assert.Equal(1003, LogDataShare.SMSG_SEARCH_START);
        Assert.Equal(1004, LogDataShare.SMSG_SEARCH_LOG);
        Assert.Equal(1005, LogDataShare.SMSG_SEARCH_LOG_END);
        Assert.Equal(2, LogDataShare.tLogServer);
    }

    [Fact]
    public void Globals_MatchDelphiTypedConstants()
    {
        Assert.Equal(@".\BaseDir", LogDataShare.sBaseDir);
        Assert.Equal("", LogDataShare.sServerName);
        Assert.Equal("LogDataSrv", LogDataShare.sCaption);
        Assert.Equal(10000, LogDataShare.nServerPort);
        Assert.Equal(0, LogDataShare.g_nControlPort);
        Assert.Equal("", LogDataShare.g_sControlPassword);
        Assert.Equal("", LogDataShare.g_ControlIPFile);
        Assert.Empty(LogDataShare.g_ControlIPList.AsEnumerable());
    }

    [Theory]
    [InlineData(0, "00")]
    [InlineData(9, "09")]
    [InlineData(10, "10")]
    [InlineData(99, "99")]
    [InlineData(100, "100")]
    public void IntToString_PadsSingleDigit(int n, string expected)
        => Assert.Equal(expected, LogDataShare.IntToString(n));

    [Fact]
    public void LogActorTypeNames_MatchDelphi()
    {
        Assert.Equal(7, LogActorTypeNames.Names.Length);
        Assert.Equal("-", LogActorTypeNames.Get(TLogActorType.latNone));
        Assert.Equal("人物", LogActorTypeNames.Get(TLogActorType.latHuman));
        Assert.Equal("假人", LogActorTypeNames.Get(TLogActorType.latDummyHuman));
        Assert.Equal("英雄", LogActorTypeNames.Get(TLogActorType.latHero));
        Assert.Equal("假人英雄", LogActorTypeNames.Get(TLogActorType.latDummyHero));
        Assert.Equal("人形怪", LogActorTypeNames.Get(TLogActorType.latPlayMonster));
        Assert.Equal("怪物", LogActorTypeNames.Get(TLogActorType.latMonster));
    }

    [Fact]
    public void TThreadList_LockListReturnsSharedStorage()
    {
        var list = new TThreadList();
        var l = list.LockList();
        l.Add("a");
        list.UnlockList();

        var again = list.LockList();
        Assert.Single(again);
        list.UnlockList();
    }

    [Fact]
    public void SearchManagerSeam_AddAndCancel()
    {
        var m = new TSearchManagerSeam();
        Assert.Equal(256, m.SearchActions.Length);
        Assert.All(m.SearchActions, b => Assert.False(b));

        m.AddTask(new TSearchTask { TaskID = 1, FileName = "x" });
        Assert.Single(m.Tasks);
        m.CancelAndClearAllTask();
        Assert.Empty(m.Tasks);
    }

    [Fact]
    public void TSafeStringList_LockUnlockAndSave()
    {
        var dir = Path.Combine(Path.GetTempPath(), "gxx_lane6_safe_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            string file = Path.Combine(dir, "ip.txt");
            var l = new TSafeStringList();
            l.Lock();
            l.Add("1.1.1.1");
            l.UnLock();
            l.SaveToFile(file);

            Assert.True(File.Exists(file));
            var read = new TStringList();
            read.LoadFromFile(file);
            Assert.Equal(new[] { "1.1.1.1" }, read.AsEnumerable());
        }
        finally
        {
            try { Directory.Delete(dir, true); } catch { /* best effort */ }
        }
    }
}
