using System;
using System.IO;
using System.Text;
using GXX.Core.Protocol;
using GXX.Core.Util;
using GXX.LoginSrv;
using Xunit;

namespace GXX.LoginSrv.Tests;

/// <summary>BasicSet.pas TFrmBasicSet 1:1 测试（INI 键序/默认值/校验分支）。</summary>
public sealed class BasicSetTests : IDisposable
{
    private readonly string _dir;

    public BasicSetTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_lane6_basicset_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        LoginSrvShare.ResetForTests();
        LoginSrvForms.MessageBoxHandler = (_, _, _) => LoginSrvForms.IDOK;
        LoginSrvForms.NextAnswer = null;
        LoginSrvForms.LastMessage = null;
        LoginSrvForms.LastCaption = null;
        LoginSrvInputQuery.Handler = null;
    }

    public void Dispose()
    {
        LoginSrvForms.MessageBoxHandler = null;
        LoginSrvForms.NextAnswer = null;
        LoginSrvForms.LastMessage = null;
        LoginSrvForms.LastCaption = null;
        LoginSrvInputQuery.Handler = null;
        LoginSrvShare.ResetForTests();
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    private string IniPath => Path.Combine(_dir, "!LoginSrv.ini");
    private string DisablePwdFile => Path.Combine(_dir, "DisablePassword.txt");
    private string ControlIpFile => Path.Combine(_dir, "ControlIP.txt");

    // ------------------------------------------------------------------
    // 1) WriteConfig：键序 + 默认值 + 完整文本
    // ------------------------------------------------------------------

    /// <summary>BasicSet.pas:565 WriteConfig 的键序（Server 段 49 键 → DB 段 5 键）。</summary>
    [Fact]
    public void WriteConfig_WritesAllKeysInDelphiOrder()
    {
        var ini = new TFastIniFile(IniPath);
        LoginSrvShare.g_Config.IniConf = ini;

        TFrmBasicSet.WriteConfig(LoginSrvShare.g_Config);

        string text = File.ReadAllText(IniPath, GXX.Core.EncodingInit.GBK);

        string[] server = new[]
        {
            "DBServer=127.0.0.1",
            "FeeServer=127.0.0.1",
            "LogServer=127.0.0.1",
            "GateAddr=0.0.0.0",
            "GatePort=5500",
            "ServerAddr=0.0.0.0",
            "ServerPort=5600",
            "MonAddr=0.0.0.0",
            "MonPort=3000",
            "ControlPassword=bmm2",
            "ControlPort=0",
            "ShowBlockIPLog=0",
            "DBSPort=16300",
            "FeePort=16301",
            "LogPort=16301",
            "ReadyServers=0",
            "EnableMakingID=-1",
            "TestServer=-1",
            "GetbackPassword=-1",
            "GetbackPasswordCheckAll=0",
            "DisableIDSamePassword=0",
            "DisableQuizSameAnswer=0",
            "DisableIDSameL2Password=0",
            // ★ 原文笔误（BasicSet.pas:198）：boDisableL2SamePassword 的键名被写成了常量名，
            //   因此 INI 中**永远不会出现** DisableL2SamePassword 这个键。
            "sIdentDisableL2SamePassword=0",
            "DisablePasswordSameChr=0",
            "DisablePasswordAllNum=0",
            "DisablePasswordAllLetter=0",
            "AutoClearID=0",
            "AutoClearTime=0",
            "UnLockAccount=-1",
            "UnLockAccountTime=0",
            "DynamicIPMode=0",
            "RandCodeLogin=0",
            "RandCodeLoginReg=0",
            "RandCodePwdGetback=0",
            "RandCodePwdChange=0",
            "LoginWaveValue=8",
            "OtherWaveValue=4",
            "RandomCodeErrorMaxCount=3",
            "RandomCodeRefreshMaxCount=5",
            "EnabledL2Password=0",
            "ChangedMACCheckL2=0",
            "ChangedIPCheckL2=0",
            "AlwaysCheckL2=0",
            "NewLoginDlg=0",
            "NewLoginInto=-1",
            "NewLoginPhone=-1",
            "NewLoginMustHasPhone=-1",
        };
        string[] db = new[]
        {
            @"IdDir=.\DB\",
            @"WebLogDir=.\Share\",
            @"CountLogDir=.\CountLog\",
            @"FeedIDList=.\FeedIDList.txt",
            @"FeedIPList=.\FeedIPList.txt",
        };

        var sb = new StringBuilder();
        sb.Append("[Server]\r\n");
        foreach (string line in server) sb.Append(line).Append("\r\n");
        sb.Append("\r\n");
        sb.Append("[DB]\r\n");
        foreach (string line in db) sb.Append(line).Append("\r\n");
        sb.Append("\r\n");

        Assert.Equal(sb.ToString(), text);
        // 键序断言（对照 Server 段在 DB 段之前）
        Assert.True(text.IndexOf("[Server]", StringComparison.Ordinal) < text.IndexOf("[DB]", StringComparison.Ordinal));
    }

    /// <summary>布尔写 '-1'/'0'（Delphi TIniFile.WriteBool → SysUtils.BoolToStr），1:1。</summary>
    [Fact]
    public void WriteConfig_BooleanUsesDelphiBoolToStr()
    {
        var ini = new TFastIniFile(IniPath);
        LoginSrvShare.g_Config.IniConf = ini;
        LoginSrvShare.g_Config.boTestServer = true;
        LoginSrvShare.g_Config.boDynamicIPMode = false;

        TFrmBasicSet.WriteConfig(LoginSrvShare.g_Config);

        var read = new TFastIniFile(IniPath);
        Assert.Equal("-1", read.ReadString("Server", "TestServer", "?"));
        Assert.Equal("0", read.ReadString("Server", "DynamicIPMode", "?"));
    }

    /// <summary>WriteConfig 写的是 g_Config.boShowBlockIPLog（而非入参 Config 的同名字段）。</summary>
    [Fact]
    public void WriteConfig_ShowBlockIPLogComesFromGlobalConfig()
    {
        var ini = new TFastIniFile(IniPath);
        var local = TConfig.CreateDefault();
        local.IniConf = ini;
        local.boShowBlockIPLog = false;
        LoginSrvShare.g_Config.boShowBlockIPLog = true;

        TFrmBasicSet.WriteConfig(local);

        var read = new TFastIniFile(IniPath);
        Assert.Equal("-1", read.ReadString("Server", "ShowBlockIPLog", "?"));
    }

    // ------------------------------------------------------------------
    // 2) OpenBasicSet：装载 + SpinEdit 夹取
    // ------------------------------------------------------------------

    [StaFact]
    public void OpenBasicSet_LoadsConfigIntoControls()
    {
        StaRunner.New(() =>
        {
            var cfg = LoginSrvShare.g_Config;
            cfg.boTestServer = false;
            cfg.boEnableMakingID = false;
            cfg.sGateAddr = "10.0.0.1";
            cfg.nGatePort = 6001;
            cfg.sServerAddr = "10.0.0.2";
            cfg.nServerPort = 6002;
            cfg.sMonAddr = "10.0.0.3";
            cfg.nMonPort = 6003;
            cfg.nControlPort = 6100;
            cfg.sControlPassword = "pw";
            cfg.boDynamicIPMode = true;
            cfg.boShowBlockIPLog = true;
            cfg.dwAutoClearTime = 33;
            cfg.dwUnLockAccountTime = 44;
            cfg.boRandomCode[(int)TRandCodeType.rctRegister] = true;
            cfg.btLoginWaveValue = 7;
            cfg.btOtherWaveValue = 5;
            cfg.nRandomCodeErrorMaxCount = 9;
            cfg.nRandomCodeRefreshMaxCount = 8;
            cfg.boEnabledL2Password = true;
            cfg.boChangedMACCheckL2 = true;
            cfg.boChangedIPCheckL2 = true;
            cfg.boAlwaysCheckL2 = true;
            cfg.boDisableIDSameL2Password = true;
            cfg.boDisableL2SamePassword = true;
            cfg.boDisablePwdSameChr = true;
            cfg.boDisablePwdAllNum = true;
            cfg.boDisablePwdAllLetter = true;
            cfg.boNewLoginDlg = true;
            cfg.boNewLoginInto = false;
            cfg.boNewLoginPhone = false;
            LoginSrvShare.g_DisablePasswordList.Add("bad1");
            LoginSrvShare.g_DisablePasswordList.Add("bad2");
            LoginSrvShare.g_ControlIPList.Add("1.2.3.4");

            using var form = new TFrmBasicSet();
            form.OpenBasicSet(showModal: false);

            Assert.False(form.CheckBoxTestServer.Checked);
            Assert.False(form.CheckBoxEnableMakingID.Checked);
            Assert.Equal("10.0.0.1", form.EditGateAddr.Text);
            Assert.Equal("6001", form.EditGatePort.Text);
            Assert.Equal("10.0.0.2", form.EditServerAddr.Text);
            Assert.Equal("6002", form.EditServerPort.Text);
            Assert.Equal("10.0.0.3", form.EditMonAddr.Text);
            Assert.Equal("6003", form.EditMonPort.Text);
            Assert.Equal("6100", form.edtControlPort.Text);
            Assert.Equal("pw", form.edtControlPassword.Text);
            Assert.True(form.CheckBoxDynamicIPMode.Checked);
            Assert.True(form.chkShowBlockIPLog.Checked);
            Assert.Equal(33m, form.SpinEditAutoClearTime.Value);
            Assert.Equal(44m, form.SpinEditUnLockAccountTime.Value);
            Assert.True(form.chkRandomCodeReg.Checked);
            Assert.False(form.chkRandomCodeLogin.Checked);
            Assert.Equal(7m, form.seLoginWaveValue.Value);
            Assert.Equal(5m, form.seOtherWaveValue.Value);
            Assert.Equal(9m, form.EditRandomCodeErrorMaxCount.Value);
            Assert.Equal(8m, form.seRandomCodeRefreshMaxCount.Value);
            Assert.True(form.chkEnabledL2Password.Checked);
            Assert.True(form.chkAlwaysCheckL2.Checked);
            Assert.True(form.chkDisablePwdAllLetter.Checked);
            Assert.True(form.chkNewLoginDlg.Checked);
            Assert.False(form.chkNewLoginInto.Checked);
            Assert.False(form.chkNewLoginPhone.Checked);
            Assert.Equal("bad1\r\nbad2\r\n", form.mmoDisablePassword.Text);
            Assert.Equal(1, form.lstControlIPList.Items.Count);
            Assert.Equal("1.2.3.4", form.lstControlIPList.Items[0]);
            Assert.False(form.ButtonSave.Enabled);
            Assert.Equal(0, form.PageControl1.SelectedIndex);
        });
    }

    /// <summary>
    /// TSpinEdit.SetValue 越界夹取：低于 MinValue 的配置只夹控件值；
    /// 若夹取结果恰好等于控件初值，则**不触发** OnChange（Delphi 同样只在值变化时 Change），
    /// 于是 g_Config 里仍保留越界的原值 —— 原文行为如此，差异断言。
    /// </summary>
    [StaFact]
    public void OpenBasicSet_ClampsSpinEditToMinValue_OnlyWritesBackWhenValueChanged()
    {
        StaRunner.New(() =>
        {
            LoginSrvShare.g_Config.dwAutoClearTime = 0;      // 低于 MinValue=1，夹到 1 == 控件初值
            LoginSrvShare.g_Config.dwUnLockAccountTime = 0;  // 同上
            LoginSrvShare.g_Config.btLoginWaveValue = 0;     // 同上
            LoginSrvShare.g_Config.btOtherWaveValue = 9;     // 高于 MaxValue=6，夹到 6 != 初值 1

            using var form = new TFrmBasicSet();
            form.OpenBasicSet(showModal: false);

            Assert.Equal(1m, form.SpinEditAutoClearTime.Value);
            Assert.Equal(1m, form.SpinEditUnLockAccountTime.Value);
            Assert.Equal(1m, form.seLoginWaveValue.Value);
            Assert.Equal(6m, form.seOtherWaveValue.Value);

            // 只有"夹取后与初值不同"的 seOtherWaveValue 触发了 OnChange 回写
            Assert.Equal(0, LoginSrvShare.g_Config.dwAutoClearTime);
            Assert.Equal(0, LoginSrvShare.g_Config.dwUnLockAccountTime);
            Assert.Equal(0, LoginSrvShare.g_Config.btLoginWaveValue);
            Assert.Equal(6, LoginSrvShare.g_Config.btOtherWaveValue);
        });
    }

    /// <summary>OpenBasicSet 末尾 LockSaveButtonEnabled（装载过程引发的 OnChange 被它盖掉）。</summary>
    [StaFact]
    public void OpenBasicSet_EndsWithSaveButtonLocked()
    {
        StaRunner.New(() =>
        {
            using var form = new TFrmBasicSet();
            form.OpenBasicSet(showModal: false);
            Assert.False(form.ButtonSave.Enabled);
        });
    }

    // ------------------------------------------------------------------
    // 3) 各 Click / Change 处理器（表格驱动，覆盖全部公开处理器）
    // ------------------------------------------------------------------

    public static TheoryData<string> AllBooleanHandlers => new()
    {
        "TestServer", "EnableMakingID", "EnableGetbackPassword", "GetbackPasswordCheckAll",
        "DisableIDSamePassword", "DisableQuizSameAnswer", "DisableIDSameL2Password", "DisableL2SamePassword",
        "AutoClear", "AutoUnLockAccount", "DynamicIPMode", "ShowBlockIPLog",
        "RandomCodeLogin", "RandomCodeReg", "RandomCodePwdGetback", "RandomCodePwdChange",
        "ChangedMACCheckL2", "ChangedIPCheckL2", "AlwaysCheckL2", "EnabledL2Password",
        "DisablePwdSameChr", "DisablePwdAllNum", "DisablePwdAllLetter",
        "NewLoginDlg", "NewLoginInto", "NewLoginPhone",
    };

    /// <summary>每个复选框处理器：勾选写 True，取消写 False，并解锁保存按钮。</summary>
    [StaTheory]
    [MemberData(nameof(AllBooleanHandlers))]
    public void CheckBoxHandlers_WriteConfigAndUnlockSave(string which)
    {
        StaRunner.New(() =>
        {
            using var form = new TFrmBasicSet();
            form.OpenBasicSet(showModal: false);
            Assert.False(form.ButtonSave.Enabled);

            SetCheck(form, which, true);
            Assert.True(GetBoolConfig(which));
            Assert.True(form.ButtonSave.Enabled);

            form.LockSaveButtonEnabled();
            SetCheck(form, which, false);
            Assert.False(GetBoolConfig(which));
            Assert.True(form.ButtonSave.Enabled);
        });
    }

    private static void SetCheck(TFrmBasicSet form, string which, bool value)
    {
        switch (which)
        {
            case "TestServer": form.CheckBoxTestServer.Checked = value; form.CheckBoxTestServerClick(form); break;
            case "EnableMakingID": form.CheckBoxEnableMakingID.Checked = value; form.CheckBoxEnableMakingIDClick(form); break;
            case "EnableGetbackPassword": form.CheckBoxEnableGetbackPassword.Checked = value; form.CheckBoxEnableGetbackPasswordClick(form); break;
            case "GetbackPasswordCheckAll": form.CheckBoxGetbackPasswordCheckAll.Checked = value; form.CheckBoxGetbackPasswordCheckAllClick(form); break;
            case "DisableIDSamePassword": form.chkDisableIDSamePassword.Checked = value; form.chkDisableIDSamePasswordClick(form); break;
            case "DisableQuizSameAnswer": form.chkDisableQuizSameAnswer.Checked = value; form.chkDisableQuizSameAnswerClick(form); break;
            case "DisableIDSameL2Password": form.chkDisableIDSameL2Password.Checked = value; form.chkDisableIDSameL2PasswordClick(form); break;
            case "DisableL2SamePassword": form.chkDisableL2SamePassword.Checked = value; form.chkDisableL2SamePasswordClick(form); break;
            case "AutoClear": form.CheckBoxAutoClear.Checked = value; form.CheckBoxAutoClearClick(form); break;
            case "AutoUnLockAccount": form.CheckBoxAutoUnLockAccount.Checked = value; form.CheckBoxAutoUnLockAccountClick(form); break;
            case "DynamicIPMode": form.CheckBoxDynamicIPMode.Checked = value; form.CheckBoxDynamicIPModeClick(form); break;
            case "ShowBlockIPLog": form.chkShowBlockIPLog.Checked = value; form.chkShowBlockIPLogClick(form); break;
            case "RandomCodeLogin": form.chkRandomCodeLogin.Checked = value; form.chkRandomCodeLoginClick(form); break;
            case "RandomCodeReg": form.chkRandomCodeReg.Checked = value; form.chkRandomCodeRegClick(form); break;
            case "RandomCodePwdGetback": form.chkRandomCodePwdGetback.Checked = value; form.chkRandomCodePwdGetbackClick(form); break;
            case "RandomCodePwdChange": form.chkRandomCodePwdChange.Checked = value; form.chkRandomCodePwdChangeClick(form); break;
            case "ChangedMACCheckL2": form.chkChangedMACCheckL2.Checked = value; form.chkChangedMACCheckL2Click(form); break;
            case "ChangedIPCheckL2": form.chkChangedIPCheckL2.Checked = value; form.chkChangedIPCheckL2Click(form); break;
            case "AlwaysCheckL2": form.chkAlwaysCheckL2.Checked = value; form.chkAlwaysCheckL2Click(form); break;
            case "EnabledL2Password": form.chkEnabledL2Password.Checked = value; form.chkEnabledL2PasswordClick(form); break;
            case "DisablePwdSameChr": form.chkDisablePwdSameChr.Checked = value; form.chkDisablePwdSameChrClick(form); break;
            case "DisablePwdAllNum": form.chkDisablePwdAllNum.Checked = value; form.chkDisablePwdAllNumClick(form); break;
            case "DisablePwdAllLetter": form.chkDisablePwdAllLetter.Checked = value; form.chkDisablePwdAllLetterClick(form); break;
            case "NewLoginDlg": form.chkNewLoginDlg.Checked = value; form.chkNewLoginDlgClick(form); break;
            case "NewLoginInto": form.chkNewLoginInto.Checked = value; form.chkNewLoginIntoClick(form); break;
            case "NewLoginPhone": form.chkNewLoginPhone.Checked = value; form.chkNewLoginPhoneClick(form); break;
            default: throw new ArgumentOutOfRangeException(nameof(which), which);
        }
    }

    private static bool GetBoolConfig(string which)
    {
        var c = LoginSrvShare.g_Config;
        return which switch
        {
            "TestServer" => c.boTestServer,
            "EnableMakingID" => c.boEnableMakingID,
            "EnableGetbackPassword" => c.boEnableGetbackPassword,
            "GetbackPasswordCheckAll" => c.boGetbackPasswordCheckAll,
            "DisableIDSamePassword" => c.boDisableIDSamePassword,
            "DisableQuizSameAnswer" => c.boDisableQuizSameAnswer,
            "DisableIDSameL2Password" => c.boDisableIDSameL2Password,
            "DisableL2SamePassword" => c.boDisableL2SamePassword,
            "AutoClear" => c.boAutoClearID,
            "AutoUnLockAccount" => c.boUnLockAccount,
            "DynamicIPMode" => c.boDynamicIPMode,
            "ShowBlockIPLog" => c.boShowBlockIPLog,
            "RandomCodeLogin" => c.boRandomCode[(int)TRandCodeType.rctLogin],
            "RandomCodeReg" => c.boRandomCode[(int)TRandCodeType.rctRegister],
            "RandomCodePwdGetback" => c.boRandomCode[(int)TRandCodeType.rctPwdGetback],
            "RandomCodePwdChange" => c.boRandomCode[(int)TRandCodeType.rctPwdChange],
            "ChangedMACCheckL2" => c.boChangedMACCheckL2,
            "ChangedIPCheckL2" => c.boChangedIPCheckL2,
            "AlwaysCheckL2" => c.boAlwaysCheckL2,
            "EnabledL2Password" => c.boEnabledL2Password,
            "DisablePwdSameChr" => c.boDisablePwdSameChr,
            "DisablePwdAllNum" => c.boDisablePwdAllNum,
            "DisablePwdAllLetter" => c.boDisablePwdAllLetter,
            "NewLoginDlg" => c.boNewLoginDlg,
            "NewLoginInto" => c.boNewLoginInto,
            "NewLoginPhone" => c.boNewLoginPhone,
            _ => throw new ArgumentOutOfRangeException(nameof(which), which),
        };
    }

    /// <summary>chkNewLoginMustHasPhoneClick 方法体原文整体被注释 → 不改配置、不解锁。</summary>
    [StaFact]
    public void chkNewLoginMustHasPhoneClick_IsNoOp()
    {
        StaRunner.New(() =>
        {
            LoginSrvShare.g_Config.boNewLoginMustHasPhone = true;
            using var form = new TFrmBasicSet();
            form.OpenBasicSet(showModal: false);

            form.chkNewLoginMustHasPhoneClick(form);

            Assert.True(LoginSrvShare.g_Config.boNewLoginMustHasPhone);
            Assert.False(form.ButtonSave.Enabled);
        });
    }

    /// <summary>mmoDisablePasswordChange 只解锁保存按钮，不写配置。</summary>
    [StaFact]
    public void mmoDisablePasswordChange_OnlyUnlocksSave()
    {
        StaRunner.New(() =>
        {
            using var form = new TFrmBasicSet();
            form.OpenBasicSet(showModal: false);
            form.mmoDisablePassword.Text = "abc";
            form.mmoDisablePasswordChange(form);
            Assert.True(form.ButtonSave.Enabled);
        });
    }

    // ------------------------------------------------------------------
    // 4) 端口/地址处理器：Str_ToInt 与 StrToIntDef 的差异
    // ------------------------------------------------------------------

    /// <summary>
    /// ★ 差异断言：EditGatePortChange 用 HUtil32.Str_ToInt（首个字符是数字→TryParse 失败时结果为 0），
    /// 而 EditServerPortChange 用 SysUtils.StrToIntDef（失败回落到 5600）。原文两处写法不同。
    /// </summary>
    [StaFact]
    public void PortHandlers_StrToIntVsStrToIntDef_BehaveDifferently()
    {
        StaRunner.New(() =>
        {
            using var form = new TFrmBasicSet();
            form.OpenBasicSet(showModal: false);

            form.EditGatePort.Text = "12x";
            form.EditGatePortChange(form);
            form.EditMonPort.Text = "12x";
            form.EditMonPortChange(form);
            form.EditServerPort.Text = "12x";
            form.EditServerPortChange(form);

            Assert.Equal(0, LoginSrvShare.g_Config.nGatePort);      // Str_ToInt：TryParse 失败 → 0
            Assert.Equal(0, LoginSrvShare.g_Config.nMonPort);
            Assert.Equal(5600, LoginSrvShare.g_Config.nServerPort); // StrToIntDef → 默认值
        });
    }

    [StaTheory]
    [InlineData("5501", 5501)]
    [InlineData("", 5500)]
    [InlineData("abc", 5500)]
    public void EditGatePortChange_ParsesOrDefaults(string text, int expected)
    {
        StaRunner.New(() =>
        {
            using var form = new TFrmBasicSet();
            form.OpenBasicSet(showModal: false);
            form.EditGatePort.Text = text;
            form.EditGatePortChange(form);
            Assert.Equal(expected, LoginSrvShare.g_Config.nGatePort);
            Assert.True(form.ButtonSave.Enabled);
        });
    }

    [StaFact]
    public void AddrHandlers_TrimAndWrite()
    {
        StaRunner.New(() =>
        {
            using var form = new TFrmBasicSet();
            form.OpenBasicSet(showModal: false);
            form.EditGateAddr.Text = "  1.1.1.1  ";
            form.EditGateAddrChange(form);
            form.EditMonAddr.Text = "  2.2.2.2  ";
            form.EditMonAddrChange(form);
            form.EditServerAddr.Text = "  3.3.3.3  ";
            form.EditServerAddrChange(form);
            Assert.Equal("1.1.1.1", LoginSrvShare.g_Config.sGateAddr);
            Assert.Equal("2.2.2.2", LoginSrvShare.g_Config.sMonAddr);
            Assert.Equal("3.3.3.3", LoginSrvShare.g_Config.sServerAddr);
        });
    }

    [StaFact]
    public void ControlPortAndPasswordHandlers()
    {
        StaRunner.New(() =>
        {
            using var form = new TFrmBasicSet();
            form.OpenBasicSet(showModal: false);
            form.edtControlPort.Text = "7000";
            form.edtControlPortChange(form);
            form.edtControlPassword.Text = " pw ";
            form.edtControlPasswordChange(form);
            Assert.Equal(7000, LoginSrvShare.g_Config.nControlPort);
            Assert.Equal("pw", LoginSrvShare.g_Config.sControlPassword);
        });
    }

    [StaFact]
    public void SpinHandlers_WriteConfigAndUnlockSave()
    {
        StaRunner.New(() =>
        {
            using var form = new TFrmBasicSet();
            form.OpenBasicSet(showModal: false);

            form.SpinEditAutoClearTime.Value = 12; form.SpinEditAutoClearTimeChange(form);
            form.SpinEditUnLockAccountTime.Value = 13; form.SpinEditUnLockAccountTimeChange(form);
            form.EditRandomCodeErrorMaxCount.Value = 7; form.EditRandomCodeErrorMaxCountChange(form);
            form.seRandomCodeRefreshMaxCount.Value = 6; form.seRandomCodeRefreshMaxCountChange(form);
            form.seLoginWaveValue.Value = 4; form.seLoginWaveValueChange(form);
            form.seOtherWaveValue.Value = 3; form.seOtherWaveValueChange(form);

            Assert.Equal(12, LoginSrvShare.g_Config.dwAutoClearTime);
            Assert.Equal(13, LoginSrvShare.g_Config.dwUnLockAccountTime);
            Assert.Equal(7, LoginSrvShare.g_Config.nRandomCodeErrorMaxCount);
            Assert.Equal(6, LoginSrvShare.g_Config.nRandomCodeRefreshMaxCount);
            Assert.Equal(4, LoginSrvShare.g_Config.btLoginWaveValue);
            Assert.Equal(3, LoginSrvShare.g_Config.btOtherWaveValue);
            Assert.True(form.ButtonSave.Enabled);
        });
    }

    // ------------------------------------------------------------------
    // 5) 恢复默认值按钮（差异断言）
    // ------------------------------------------------------------------

    /// <summary>
    /// ButtonRestoreBasicClick 只改控件、不直接写 g_Config；
    /// 但 SpinEdit 的 Value 变化会触发 OnChange → 间接写入 dwAutoClearTime/dwUnLockAccountTime 并解锁保存按钮。
    /// </summary>
    [StaFact]
    public void ButtonRestoreBasicClick_SetsControlsAndIndirectlyWritesSpinConfig()
    {
        StaRunner.New(() =>
        {
            LoginSrvShare.g_Config.dwAutoClearTime = 99;
            LoginSrvShare.g_Config.dwUnLockAccountTime = 99;
            using var form = new TFrmBasicSet();
            form.OpenBasicSet(showModal: false);
            Assert.False(form.ButtonSave.Enabled);

            form.ButtonRestoreBasicClick(form);

            Assert.True(form.CheckBoxTestServer.Checked);
            Assert.True(form.CheckBoxEnableMakingID.Checked);
            Assert.True(form.CheckBoxEnableGetbackPassword.Checked);
            Assert.True(form.CheckBoxAutoClear.Checked);
            Assert.Equal(1m, form.SpinEditAutoClearTime.Value);
            Assert.False(form.CheckBoxAutoUnLockAccount.Checked);
            Assert.Equal(10m, form.SpinEditUnLockAccountTime.Value);
            Assert.False(form.chkRandomCodeLogin.Checked);
            Assert.False(form.chkRandomCodePwdChange.Checked);
            Assert.Equal(3m, form.EditRandomCodeErrorMaxCount.Value);

            // Spin 变化触发的 OnChange 副作用（1:1）
            Assert.Equal(1, LoginSrvShare.g_Config.dwAutoClearTime);
            Assert.Equal(10, LoginSrvShare.g_Config.dwUnLockAccountTime);
            Assert.True(form.ButtonSave.Enabled);
        });
    }

    /// <summary>ButtonRestoreNetClick 只改控件；文本变化经 Edit OnChange 间接写回 g_Config 的网络字段。</summary>
    [StaFact]
    public void ButtonRestoreNetClick_SetsNetworkDefaults()
    {
        StaRunner.New(() =>
        {
            LoginSrvShare.g_Config.sGateAddr = "9.9.9.9";
            LoginSrvShare.g_Config.nGatePort = 9999;
            LoginSrvShare.g_Config.sServerAddr = "9.9.9.9";
            LoginSrvShare.g_Config.nServerPort = 9999;
            LoginSrvShare.g_Config.sMonAddr = "9.9.9.9";
            LoginSrvShare.g_Config.nMonPort = 9999;
            LoginSrvShare.g_Config.boDynamicIPMode = true;

            using var form = new TFrmBasicSet();
            form.OpenBasicSet(showModal: false);
            form.ButtonRestoreNetClick(form);

            Assert.Equal("0.0.0.0", form.EditGateAddr.Text);
            Assert.Equal("5500", form.EditGatePort.Text);
            Assert.Equal("0.0.0.0", form.EditServerAddr.Text);
            Assert.Equal("5600", form.EditServerPort.Text);
            Assert.Equal("0.0.0.0", form.EditMonAddr.Text);
            Assert.Equal("3000", form.EditMonPort.Text);
            Assert.False(form.CheckBoxDynamicIPMode.Checked);

            // 经 OnChange 间接回写（原文如此）
            Assert.Equal("0.0.0.0", LoginSrvShare.g_Config.sGateAddr);
            Assert.Equal(5500, LoginSrvShare.g_Config.nGatePort);
            Assert.Equal(5600, LoginSrvShare.g_Config.nServerPort);
            Assert.Equal(3000, LoginSrvShare.g_Config.nMonPort);
        });
    }

    // ------------------------------------------------------------------
    // 6) ButtonSave / ButtonClose
    // ------------------------------------------------------------------

    [StaFact]
    public void ButtonSaveClick_WritesIniAndDisablePasswordFile_ThenLocks()
    {
        StaRunner.New(() =>
        {
            LoginSrvShare.g_Config.IniConf = new TFastIniFile(IniPath);
            LoginSrvShare.g_DisablePasswordFile = DisablePwdFile;
            using var form = new TFrmBasicSet();
            form.OpenBasicSet(showModal: false);
            form.mmoDisablePassword.Text = "abc\r\ndef\r\n";
            form.ButtonSaveClick(form);

            Assert.True(File.Exists(IniPath));
            Assert.Equal(new[] { "abc", "def" }, LoginSrvShare.g_DisablePasswordList.AsEnumerable());
            string saved = File.ReadAllText(DisablePwdFile, GXX.Core.EncodingInit.GBK);
            Assert.Equal("abc\r\ndef\r\n", saved);
            Assert.False(form.ButtonSave.Enabled);
        });
    }

    /// <summary>ButtonSaveClick 在 ini 未配置时会 NRE（对应原文字典对象为 nil）—— 断言抛异常。</summary>
    [StaFact]
    public void ButtonSaveClick_WithoutIniConf_Throws()
    {
        StaRunner.New(() =>
        {
            using var form = new TFrmBasicSet();
            LoginSrvShare.g_Config.IniConf = null;
            Assert.Throws<NullReferenceException>(() => form.ButtonSaveClick(form));
        });
    }

    [StaFact]
    public void ButtonCloseClick_ClosesForm()
    {
        StaRunner.New(() =>
        {
            var form = new TFrmBasicSet();
            form.ButtonCloseClick(form);
            form.Dispose();
        });
    }

    // ------------------------------------------------------------------
    // 7) IP 列表菜单
    // ------------------------------------------------------------------

    [StaFact]
    public void mniIPAddClick_ValidIp_AddsAndSaves()
    {
        StaRunner.New(() =>
        {
            LoginSrvShare.g_ControlIPFile = ControlIpFile;
            LoginSrvInputQuery.Handler = (_, _, _, _) => (true, "202.103.100.20");
            using var form = new TFrmBasicSet();
            form.OpenBasicSet(showModal: false);

            form.mniIPAddClick(form);

            Assert.Equal(1, LoginSrvShare.g_ControlIPList.Count);
            Assert.Equal("202.103.100.20", LoginSrvShare.g_ControlIPList[0]);
            Assert.Equal(1, form.lstControlIPList.Items.Count);
            Assert.True(File.Exists(ControlIpFile));
        });
    }

    [StaFact]
    public void mniIPAddClick_InvalidIp_ShowsErrorAndDoesNotAdd()
    {
        StaRunner.New(() =>
        {
            LoginSrvShare.g_ControlIPFile = ControlIpFile;
            LoginSrvInputQuery.Handler = (_, _, _, _) => (true, "300.1.1.1");
            using var form = new TFrmBasicSet();
            form.OpenBasicSet(showModal: false);

            form.mniIPAddClick(form);

            Assert.Equal(0, LoginSrvShare.g_ControlIPList.Count);
            Assert.Equal(0, form.lstControlIPList.Items.Count);
            Assert.Equal("输入的地址格式错误！", LoginSrvForms.LastMessage);
            Assert.Equal("错误", LoginSrvForms.LastCaption);
        });
    }

    [StaFact]
    public void mniIPAddClick_Cancelled_DoesNothing()
    {
        StaRunner.New(() =>
        {
            LoginSrvShare.g_ControlIPFile = ControlIpFile;
            LoginSrvInputQuery.Handler = (_, _, _, _) => (false, "");
            using var form = new TFrmBasicSet();
            form.OpenBasicSet(showModal: false);

            form.mniIPAddClick(form);

            Assert.Equal(0, LoginSrvShare.g_ControlIPList.Count);
            Assert.Null(LoginSrvForms.LastMessage);
        });
    }

    [StaFact]
    public void mniIPAddClick_DuplicateIp_NotAddedTwice()
    {
        StaRunner.New(() =>
        {
            LoginSrvShare.g_ControlIPFile = ControlIpFile;
            LoginSrvShare.g_ControlIPList.Add("1.1.1.1");
            LoginSrvInputQuery.Handler = (_, _, _, _) => (true, "1.1.1.1");
            using var form = new TFrmBasicSet();
            form.OpenBasicSet(showModal: false);
            Assert.Equal(1, form.lstControlIPList.Items.Count);

            form.mniIPAddClick(form);

            Assert.Equal(1, LoginSrvShare.g_ControlIPList.Count);
            Assert.Equal(1, form.lstControlIPList.Items.Count);
        });
    }

    [StaFact]
    public void mniIPDeleteClick_RemovesSelected()
    {
        StaRunner.New(() =>
        {
            LoginSrvShare.g_ControlIPFile = ControlIpFile;
            LoginSrvShare.g_ControlIPList.Add("1.1.1.1");
            LoginSrvShare.g_ControlIPList.Add("2.2.2.2");
            LoginSrvShare.g_ControlIPList.Add("3.3.3.3");
            using var form = new TFrmBasicSet();
            form.OpenBasicSet(showModal: false);

            form.lstControlIPList.SelectedIndex = 1;
            form.mniIPDeleteClick(form);

            Assert.Equal(new[] { "1.1.1.1", "3.3.3.3" }, LoginSrvShare.g_ControlIPList.AsEnumerable());
            Assert.Equal(2, form.lstControlIPList.Items.Count);
        });
    }

    /// <summary>未选中（ItemIndex &lt; 0）时删除分支不执行。</summary>
    [StaFact]
    public void mniIPDeleteClick_NoSelection_DoesNothing()
    {
        StaRunner.New(() =>
        {
            LoginSrvShare.g_ControlIPList.Add("1.1.1.1");
            using var form = new TFrmBasicSet();
            form.OpenBasicSet(showModal: false);
            form.lstControlIPList.SelectedIndex = -1;

            form.mniIPDeleteClick(form);

            Assert.Equal(1, LoginSrvShare.g_ControlIPList.Count);
        });
    }

    [StaFact]
    public void mniIPClearClick_ClearsBothLists()
    {
        StaRunner.New(() =>
        {
            LoginSrvShare.g_ControlIPFile = ControlIpFile;
            LoginSrvShare.g_ControlIPList.Add("1.1.1.1");
            LoginSrvShare.g_ControlIPList.Add("2.2.2.2");
            using var form = new TFrmBasicSet();
            form.OpenBasicSet(showModal: false);

            form.mniIPClearClick(form);

            Assert.Equal(0, LoginSrvShare.g_ControlIPList.Count);
            Assert.Equal(0, form.lstControlIPList.Items.Count);
        });
    }

    // ------------------------------------------------------------------
    // 8) 常量表
    // ------------------------------------------------------------------

    [Fact]
    public void RandomCodeIdentNames_MatchDelphi()
    {
        // BasicSet.pas:233 sIdentRandomCode 的 4 个键名
        var ini = new TFastIniFile(IniPath);
        LoginSrvShare.g_Config.IniConf = ini;
        TFrmBasicSet.WriteConfig(LoginSrvShare.g_Config);
        var read = new TFastIniFile(IniPath);
        Assert.True(read.ValueExists("Server", "RandCodeLogin"));
        Assert.True(read.ValueExists("Server", "RandCodeLoginReg"));
        Assert.True(read.ValueExists("Server", "RandCodePwdGetback"));
        Assert.True(read.ValueExists("Server", "RandCodePwdChange"));
    }
}
