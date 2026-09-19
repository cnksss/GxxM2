using GXX.Core.Util;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J4a：ViewSession.pas + FSrvValue.pas 1:1 转换测试。
/// </summary>
public sealed class FormViewSessionTests : IDisposable
{
    private readonly string _dir;

    public FormViewSessionTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j4a_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
        IdSocState.FrmIDSoc.m_SessionList.Clear();
    }

    public void Dispose()
    {
        IdSocState.FrmIDSoc.m_SessionList.Clear();
        M2Forms.MessageBoxHandler = null;
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    [StaFact]
    public void FormCreate_HeadersSixColumns()
    {
        StaRunner.New(() =>
        {
            using var form = new ViewSessionForm();
            Assert.Equal("序号", form.GridSession[0, 0].Value);
            Assert.Equal("登录帐号", form.GridSession[1, 0].Value);
            Assert.Equal("登录地址", form.GridSession[2, 0].Value);
            Assert.Equal("会话ID号", form.GridSession[3, 0].Value);
            Assert.Equal("充值", form.GridSession[4, 0].Value);
            Assert.Equal("充值模式", form.GridSession[5, 0].Value);
        });
    }

    [StaFact]
    public void RefGridSession_EmptyList_KeepsTwoRows()
    {
        StaRunner.New(() =>
        {
            using var form = new ViewSessionForm();
            form.RefGridSession();
            Assert.Equal(2, form.GridSession.RowCount);
            Assert.Equal("正在取得数据...", form.PanelStatus.Text);
        });
    }

    [StaFact]
    public void RefGridSession_FillsRows_SkipsClosedSessions()
    {
        IdSocState.FrmIDSoc.m_SessionList.Add(new TSessInfo
        {
            sAccount = "甲",
            sIPaddr = "127.0.0.1",
            nSessionID = 101,
            nPayMent = 30,
            nPayMode = 2
        });
        IdSocState.FrmIDSoc.m_SessionList.Add(new TSessInfo
        {
            sAccount = "乙",
            boClose = true
        });
        IdSocState.FrmIDSoc.m_SessionList.Add(new TSessInfo
        {
            sAccount = "丙",
            sIPaddr = "192.168.1.5",
            nSessionID = 303,
            nPayMent = 60,
            nPayMode = 3
        });

        StaRunner.New(() =>
        {
            using var form = new ViewSessionForm();
            form.RefGridSession();
            Assert.Equal(4, form.GridSession.RowCount); // 表头 + 3
            Assert.Equal("0", form.GridSession[0, 1].Value);
            Assert.Equal("甲", form.GridSession[1, 1].Value);
            Assert.Equal("127.0.0.1", form.GridSession[2, 1].Value);
            Assert.Equal("101", form.GridSession[3, 1].Value);
            Assert.Equal("30", form.GridSession[4, 1].Value);
            Assert.Equal("2", form.GridSession[5, 1].Value);
            // 乙 boClose=true → Delphi 跳过不写该行（单元格保持未赋值）
            Assert.Null(form.GridSession[1, 2].Value);
            // 丙 行号 2
            Assert.Equal("2", form.GridSession[0, 3].Value);
            Assert.Equal("丙", form.GridSession[1, 3].Value);
            Assert.Equal("192.168.1.5", form.GridSession[2, 3].Value);
        });
    }

    [StaFact]
    public void ButtonRefGrid_Refreshes()
    {
        StaRunner.New(() =>
        {
            using var form = new ViewSessionForm();
            IdSocState.FrmIDSoc.m_SessionList.Add(new TSessInfo { sAccount = "丁", nSessionID = 9 });
            form.ButtonRefGridClick(form);
            Assert.Equal("丁", form.GridSession[1, 1].Value);
        });
    }
}

/// <summary>FSrvValue.pas TfrmServerValue（引擎资源限额与处理参数设置）。</summary>
public sealed class FormFSrvValueTests : IDisposable
{
    private readonly string _dir;

    public FormFSrvValueTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j4b_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetServerValueDefaults();
        M2ShareLimits.ResetDefaults();
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;
    }

    public void Dispose()
    {
        M2Forms.MessageBoxHandler = null;
        M2Forms.NextAnswer = null;
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    private string IniPath => Path.Combine(_dir, "!Setup.txt");

    [StaFact]
    public void AdjuestServerConfig_LoadsAllValues()
    {
        M2ShareLimits.g_dwHumLimit = 33;
        M2Config.nSendBlock = 5000;
        M2Config.nMonGenRate = 12;
        using var _form = StaRunner.New(() => new FSrvValueForm());
        StaRunner.New(() => _form.AdjuestServerConfig(showModal: false));
        Assert.Equal(33, (int)_form.EHum.Value);
        Assert.Equal(5000, (int)_form.ESendBlock.Value);
        Assert.Equal(12, (int)_form.EditZenMonRate.Value);
        Assert.False(_form.BitBtn1.Enabled);
    }

    [StaFact]
    public void ChangeHandlers_WriteConfigWhenOpened()
    {
        using var form = StaRunner.New(() => new FSrvValueForm());
        StaRunner.New(() =>
        {
            form.AdjuestServerConfig(showModal: false);
            form.EHum.Value = 99; form.EHumChange(form);
            form.EMon.Value = 88; form.EMonChange(form);
            form.EZen.Value = 77; form.EZenChange(form);
            form.ESoc.Value = 66; form.ESocChange(form);
            form.ENpc.Value = 55; form.ENpcChange(form);
            // EDec 无 OnChange 处理器（Delphi 原文如此，仅展示）
            form.ESendBlock.Value = 1234; form.ESendBlockChange(form);
            form.ECheckBlock.Value = 2345; form.ECheckBlockChange(form);
            form.EAvailableBlock.Value = 3456; form.EAvailableBlockChange(form);
            form.EGateLoad.Value = 7; form.EGateLoadChange(form);
            form.EditZenMonRate.Value = 15; form.EditZenMonRateChange(form);
            form.EditZenMonTime.Value = 300; form.EditZenMonTimeChange(form);
            form.EditProcessTime.Value = 260; form.EditProcessTimeChange(form);
            form.EditProcessMonsterInterval.Value = 4; form.EditProcessMonsterIntervalChange(form);
            form.CheckBoxSendCompressDataToRunGate.Checked = true;
            form.CheckBoxSendCompressDataToRunGateClick(form);
        });
        Assert.Equal(99u, M2ShareLimits.g_dwHumLimit);
        Assert.Equal(88u, M2ShareLimits.g_dwMonLimit);
        Assert.Equal(77u, M2ShareLimits.g_dwZenLimit);
        Assert.Equal(66u, M2ShareLimits.g_dwSocLimit);
        Assert.Equal(55u, M2ShareLimits.g_dwNpcLimit);
        Assert.Equal(20, M2ShareLimits.nDecLimit); // EDec 无处理器，保持初值
        Assert.Equal(1234u, M2Config.nSendBlock);
        Assert.Equal(2345u, M2Config.nCheckBlock);
        Assert.Equal(3456u, M2Config.nAvailableBlock);
        Assert.Equal(7u, M2Config.nGateLoad);
        Assert.Equal(15u, M2Config.nMonGenRate);
        Assert.Equal(300u, M2Config.dwRegenMonstersTime);
        Assert.Equal(260u, M2Config.dwProcessMonstersTime);
        Assert.Equal(4u, M2Config.nProcessMonsterInterval);
        Assert.True(M2Config.boSendCompressDataToRunGate);
        Assert.True(form.BitBtn1.Enabled);
    }

    [StaFact]
    public void LimitHandlers_ClampValues()
    {
        using var form = StaRunner.New(() => new FSrvValueForm());
        StaRunner.New(() =>
        {
            form.AdjuestServerConfig(showModal: false);
            form.EHum.Value = 200; form.EHumChange(form);          // Min(150, v)
            form.EMon.Value = 500; form.EMonChange(form);          // Min(150, v)
            form.ESendBlock.Value = 3; form.ESendBlockChange(form);  // Max(10, v)
            form.EAvailableBlock.Value = 1; form.EAvailableBlockChange(form); // Max(10, v)
        });
        Assert.Equal(150u, M2ShareLimits.g_dwHumLimit);
        Assert.Equal(150u, M2ShareLimits.g_dwMonLimit);
        Assert.Equal(10u, M2Config.nSendBlock);
        Assert.Equal(10u, M2Config.nAvailableBlock);
    }

    [StaFact]
    public void BitBtn1Click_WritesIniKeys_WithFlaseTypoPreserved()
    {
        M2ShareLimits.g_dwHumLimit = 41;
        M2Config.boViewHackMessage = true;
        M2Config.boViewAdmissionFailure = false;
        using var form = StaRunner.New(() => new FSrvValueForm());
        StaRunner.New(() =>
        {
            form.AdjuestServerConfig(showModal: false);
            form.BitBtn1Click(form);
        });

        var ini = new TFastIniFile(IniPath);
        Assert.Equal(41, ini.ReadInteger("Server", "HumLimit", 0));
        Assert.Equal(30, ini.ReadInteger("Server", "MonLimit", 0));
        Assert.Equal("TRUE", ini.ReadString("Server", "ViewHackMessage", ""), ignoreCase: true);
        Assert.Equal("FLASE", ini.ReadString("Server", "ViewAdmissionFailure", "")); // Delphi 原文拼写 1:1
        Assert.Equal(10, ini.ReadInteger("Setup", "GenMonRate", 0));
        Assert.Equal(250, ini.ReadInteger("Server", "ProcessMonstersTime", 0));
        Assert.Equal(200, ini.ReadInteger("Server", "RegenMonstersTime", 0));
        Assert.Equal(2, ini.ReadInteger("Setup", "ProcessMonsterInterval", 0));
        Assert.False(ini.ReadBool("Setup", "SendCompressDataToRunGate2", true));
        Assert.False(form.BitBtn1.Enabled); // uModValue
    }

    [StaFact]
    public void ButtonDefault_ConfirmYes_ResetsDelphiDefaults()
    {
        M2ShareLimits.g_dwHumLimit = 100;
        M2Config.nSendBlock = 9999;
        M2Forms.NextAnswer = M2Forms.IDYES;
        using var form = StaRunner.New(() => new FSrvValueForm());
        StaRunner.New(() =>
        {
            form.AdjuestServerConfig(showModal: false);
            form.ButtonDefaultClick(form);
        });
        Assert.Equal(30u, M2ShareLimits.g_dwHumLimit);
        Assert.Equal(10u, M2ShareLimits.g_dwMonLimit);
        Assert.Equal(5u, M2ShareLimits.g_dwZenLimit);
        Assert.Equal(10u, M2ShareLimits.g_dwSocLimit);
        Assert.Equal(20, M2ShareLimits.nDecLimit);
        Assert.Equal(5u, M2ShareLimits.g_dwNpcLimit);
        Assert.Equal(4000u, M2Config.nSendBlock);
        Assert.Equal(10000u, M2Config.nCheckBlock);
        Assert.Equal(8000u, M2Config.nAvailableBlock);
        Assert.Equal(0u, M2Config.nGateLoad);
        Assert.False(M2Config.boViewHackMessage);
        Assert.False(M2Config.boViewAdmissionFailure);
        Assert.False(M2Config.boSendCompressDataToRunGate);
        Assert.True(M2Config.boIsOldClient);
        Assert.Equal(10u, M2Config.nMonGenRate);
        Assert.Equal(200u, M2Config.dwRegenMonstersTime);
        Assert.Equal(250u, M2Config.dwProcessMonstersTime);
        Assert.Equal(3u, M2Config.nProcessMonsterInterval);
        Assert.Equal(30, (int)form.EHum.Value); // RefShow 回填
    }

    [StaFact]
    public void ButtonDefault_ConfirmNo_NoChange()
    {
        M2ShareLimits.g_dwHumLimit = 100;
        M2Forms.NextAnswer = M2Forms.IDNO;
        using var form = StaRunner.New(() => new FSrvValueForm());
        StaRunner.New(() =>
        {
            form.AdjuestServerConfig(showModal: false);
            form.ButtonDefaultClick(form);
        });
        Assert.Equal(100u, M2ShareLimits.g_dwHumLimit);
    }

    [StaFact]
    public void ChangeIgnoredWhenNotOpened()
    {
        using var form = StaRunner.New(() => new FSrvValueForm());
        StaRunner.New(() =>
        {
            form.EHum.Value = 88;
            form.EHumChange(form); // boOpened=false → 不写
        });
        Assert.Equal(30u, M2ShareLimits.g_dwHumLimit);
        Assert.False(form.BitBtn1.Enabled);
    }
}
