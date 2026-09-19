using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GXX.GameCenter;
using Xunit;

namespace GXX.GameCenter.Tests;

/// <summary>
/// GMain.pas 声明段（第 15..17 行 const + 第 19..383 行 TfrmMain DFM 字段 + 第 503..570 行成员）
/// 覆盖率与语义测试。字段清单由 _recon/gen_gmain_fields.ps1 从原文抽取，测试再回读比对。
/// </summary>
[Collection("GameCenterSequential")]
public sealed class MainFormDeclTests : GameCenterTestBase
{
    /// <summary>GMain.pas:19-383 的 DFM 字段期望表（字段名 → 托管类型），由原文脚本抽取后固化。</summary>
    private static readonly Dictionary<string, string> ExpectedFields = new()
    {
        { "PageControl1", "TabControl" },
        { "TabSheet1", "TabPage" },
        { "TabSheet2", "TabPage" },
        { "PageControl2", "TabControl" },
        { "PageControl3", "TabControl" },
        { "TabSheet4", "TabPage" },
        { "TabSheet5", "TabPage" },
        { "TabSheet6", "TabPage" },
        { "GroupBox1", "GroupBox" },
        { "Label1", "Label" },
        { "EditGameDir", "TextBox" },
        { "ButtonNext1", "Button" },
        { "ButtonNext2", "Button" },
        { "GroupBox2", "GroupBox" },
        { "ButtonPrv2", "Button" },
        { "EditGameName", "TextBox" },
        { "Label3", "Label" },
        { "Label4", "Label" },
        { "EditGameExtIPaddr", "TextBox" },
        { "GroupBox5", "GroupBox" },
        { "ButtonStartGame", "Button" },
        { "CheckBoxM2Server", "CheckBox" },
        { "CheckBoxDBServer", "CheckBox" },
        { "CheckBoxLoginServer", "CheckBox" },
        { "CheckBoxLogServer", "CheckBox" },
        { "CheckBoxLoginGate", "CheckBox" },
        { "CheckBoxSelGate", "CheckBox" },
        { "CheckBoxRunGate", "CheckBox" },
        { "CheckBoxRunGate1", "CheckBox" },
        { "CheckBoxRunGate2", "CheckBox" },
        { "TimerStartGame", "Timer" },
        { "TimerStopGame", "Timer" },
        { "TimerCheckRun", "Timer" },
        { "MemoLog", "TextBox" },
        { "ButtonReLoadConfig", "Button" },
        { "GroupBox3", "GroupBox" },
        { "GroupBox8", "GroupBox" },
        { "Label11", "Label" },
        { "Label12", "Label" },
        { "EditSelGate_MainFormX", "NumericUpDown" },
        { "EditSelGate_MainFormY", "NumericUpDown" },
        { "TabSheet7", "TabPage" },
        { "GroupBox9", "GroupBox" },
        { "GroupBox10", "GroupBox" },
        { "Label13", "Label" },
        { "Label14", "Label" },
        { "EditLoginServer_MainFormX", "NumericUpDown" },
        { "EditLoginServer_MainFormY", "NumericUpDown" },
        { "TabSheet8", "TabPage" },
        { "GroupBox11", "GroupBox" },
        { "GroupBox12", "GroupBox" },
        { "Label15", "Label" },
        { "Label16", "Label" },
        { "EditDBServer_MainFormX", "NumericUpDown" },
        { "EditDBServer_MainFormY", "NumericUpDown" },
        { "TabSheet9", "TabPage" },
        { "GroupBox13", "GroupBox" },
        { "GroupBox14", "GroupBox" },
        { "Label17", "Label" },
        { "Label18", "Label" },
        { "EditLogServer_MainFormX", "NumericUpDown" },
        { "EditLogServer_MainFormY", "NumericUpDown" },
        { "TabSheet10", "TabPage" },
        { "GroupBox15", "GroupBox" },
        { "GroupBox16", "GroupBox" },
        { "Label19", "Label" },
        { "Label20", "Label" },
        { "EditM2Server_MainFormX", "NumericUpDown" },
        { "EditM2Server_MainFormY", "NumericUpDown" },
        { "TabSheet11", "TabPage" },
        { "ButtonSave", "Button" },
        { "ButtonGenGameConfig", "Button" },
        { "ButtonPrv3", "Button" },
        { "ButtonNext3", "Button" },
        { "TabSheet12", "TabPage" },
        { "ButtonPrv4", "Button" },
        { "ButtonNext4", "Button" },
        { "ButtonPrv5", "Button" },
        { "ButtonNext5", "Button" },
        { "ButtonPrv6", "Button" },
        { "ButtonNext6", "Button" },
        { "ButtonPrv7", "Button" },
        { "ButtonNext7", "Button" },
        { "ButtonPrv8", "Button" },
        { "ButtonNext8", "Button" },
        { "ButtonPrv9", "Button" },
        { "GroupBox17", "GroupBox" },
        { "GroupBox18", "GroupBox" },
        { "Label21", "Label" },
        { "Label22", "Label" },
        { "EditRunGate_MainFormX", "NumericUpDown" },
        { "EditRunGate_MainFormY", "NumericUpDown" },
        { "GroupBox19", "GroupBox" },
        { "Label23", "Label" },
        { "EditRunGate_Connt", "NumericUpDown" },
        { "TabSheet13", "TabPage" },
        { "ButtonLoginServerConfig", "Button" },
        { "chkDoubleLineMode", "CheckBox" },
        { "ServerSocket", "Socket" },
        { "Timer", "Timer" },
        { "GroupBox22", "GroupBox" },
        { "LabelRunGate_GatePort1", "Label" },
        { "EditRunGate_GatePort1", "TextBox" },
        { "LabelLabelRunGate_GatePort2", "Label" },
        { "EditRunGate_GatePort2", "TextBox" },
        { "LabelRunGate_GatePort3", "Label" },
        { "EditRunGate_GatePort3", "TextBox" },
        { "LabelRunGate_GatePort4", "Label" },
        { "EditRunGate_GatePort4", "TextBox" },
        { "LabelRunGate_GatePort5", "Label" },
        { "EditRunGate_GatePort5", "TextBox" },
        { "LabelRunGate_GatePort6", "Label" },
        { "EditRunGate_GatePort6", "TextBox" },
        { "LabelRunGate_GatePort7", "Label" },
        { "EditRunGate_GatePort7", "TextBox" },
        { "EditRunGate_GatePort8", "TextBox" },
        { "LabelRunGate_GatePort78", "Label" },
        { "ButtonRunGateDefault", "Button" },
        { "ButtonSelGateDefault", "Button" },
        { "ButtonGeneralDefalult", "Button" },
        { "ButtonLoginGateDefault", "Button" },
        { "ButtonLoginSrvDefault", "Button" },
        { "ButtonDBServerDefault", "Button" },
        { "ButtonLogServerDefault", "Button" },
        { "ButtonM2ServerDefault", "Button" },
        { "GroupBox24", "GroupBox" },
        { "Label29", "Label" },
        { "EditSelGate_GatePort", "TextBox" },
        { "TabSheet15", "TabPage" },
        { "GroupBox27", "GroupBox" },
        { "CheckBoxboLoginGate_GetStart", "CheckBox" },
        { "GroupBoxSelGate_GetStart", "GroupBox" },
        { "CheckBoxboSelGate_GetStart", "CheckBox" },
        { "GroupBox32", "GroupBox" },
        { "Label61", "Label" },
        { "Label62", "Label" },
        { "EditM2Server_TestLevel", "NumericUpDown" },
        { "EditM2Server_TestGold", "NumericUpDown" },
        { "Label49", "Label" },
        { "EditSelGate_GatePort1", "TextBox" },
        { "GroupBox33", "GroupBox" },
        { "Label50", "Label" },
        { "Label51", "Label" },
        { "EditLoginServerGatePort", "TextBox" },
        { "EditLoginServerServerPort", "TextBox" },
        { "GroupBox34", "GroupBox" },
        { "CheckBoxboLoginServer_GetStart", "CheckBox" },
        { "GroupBox35", "GroupBox" },
        { "CheckBoxDBServerGetStart", "CheckBox" },
        { "GroupBox36", "GroupBox" },
        { "Label52", "Label" },
        { "Label53", "Label" },
        { "EditDBServerGatePort", "TextBox" },
        { "EditDBServerServerPort", "TextBox" },
        { "GroupBox37", "GroupBox" },
        { "CheckBoxLogServerGetStart", "CheckBox" },
        { "GroupBox38", "GroupBox" },
        { "Label54", "Label" },
        { "EditLogServerPort", "TextBox" },
        { "GroupBox39", "GroupBox" },
        { "Label55", "Label" },
        { "EditM2ServerGatePort", "TextBox" },
        { "GroupBox40", "GroupBox" },
        { "CheckBoxM2ServerGetStart", "CheckBox" },
        { "Label56", "Label" },
        { "EditM2ServerMsgSrvPort", "TextBox" },
        { "GroupBox41", "GroupBox" },
        { "LabelVersion", "Label" },
        { "Label60", "Label" },
        { "CheckBoxRunGate3", "CheckBox" },
        { "CheckBoxRunGate4", "CheckBox" },
        { "CheckBoxRunGate5", "CheckBox" },
        { "CheckBoxRunGate6", "CheckBox" },
        { "CheckBoxRunGate7", "CheckBox" },
        { "GroupBox44", "GroupBox" },
        { "CheckBoxboRunGate_GetMinimize", "CheckBox" },
        { "TimerStart", "Timer" },
        { "TabSheet66", "TabPage" },
        { "GroupBox21", "GroupBox" },
        { "ListViewDataBackup", "ListView" },
        { "GroupBox29", "GroupBox" },
        { "lbl1", "Label" },
        { "lbl2", "Label" },
        { "lbl3", "Label" },
        { "lbl4", "Label" },
        { "lbl5", "Label" },
        { "lbl6", "Label" },
        { "RadioButtonBackMode1", "RadioButton" },
        { "EditSource", "TextBox" },
        { "EditDest", "TextBox" },
        { "RadioButtonBackMode2", "RadioButton" },
        { "EditHour1", "NumericUpDown" },
        { "EditHour2", "NumericUpDown" },
        { "EditMin1", "NumericUpDown" },
        { "EditMin2", "NumericUpDown" },
        { "ButtonBackChg", "Button" },
        { "ButtonBackDel", "Button" },
        { "ButtonBackAdd", "Button" },
        { "ButtonBackSave", "Button" },
        { "ButtonBackStart", "Button" },
        { "LabelBackMsg", "Label" },
        { "TimerClose", "Timer" },
        { "chkAutoStartServer", "CheckBox" },
        { "lbl7", "Label" },
        { "EditAutoStartDelayTime", "NumericUpDown" },
        { "lbl8", "Label" },
        { "TimerAutoStartServer", "Timer" },
        { "LabelNetComIPaddr", "Label" },
        { "EditGameExtNetComIPaddr", "TextBox" },
        { "CheckBoxSelGate1", "CheckBox" },
        { "CheckBoxboSelGate_GetStart1", "CheckBox" },
        { "TabSheet3", "TabPage" },
        { "EditEnvirFilePath", "TextBox" },
        { "Label5", "Label" },
        { "Label6", "Label" },
        { "EditDBName", "TextBox" },
        { "MemoLog1", "TextBox" },
        { "ButtonStdMode", "Button" },
        { "ButtonUnbindItem", "Button" },
        { "ButtonUnTakeOffItem", "Button" },
        { "ButtonChangeItemNameColorWhite", "Button" },
        { "ButtonMapEvent", "Button" },
        { "TabSheet14", "TabPage" },
        { "pgc1", "TabControl" },
        { "ts1", "TabPage" },
        { "ts2", "TabPage" },
        { "CheckGroupClear", "CheckedListBox" },
        { "grp1", "GroupBox" },
        { "lbl9", "Label" },
        { "btnMyGetTxtDel", "Button" },
        { "btnMyGetTxtAdd", "Button" },
        { "btnMyGetTxtOpen", "Button" },
        { "edtMyGetTXT", "TextBox" },
        { "lstMyGetTXT", "ListBox" },
        { "grp2", "GroupBox" },
        { "lbl10", "Label" },
        { "btnMyGetFileOpen", "Button" },
        { "btnMyGetFileAdd", "Button" },
        { "btnMyGetFileDel", "Button" },
        { "edtMyGetFile", "TextBox" },
        { "lstMyGetFile", "ListBox" },
        { "grp3", "GroupBox" },
        { "lbl11", "Label" },
        { "btnMyGetDirOpen", "Button" },
        { "btnMyGetDirAdd", "Button" },
        { "btnMyGetDirDel", "Button" },
        { "edtMyGetDir", "TextBox" },
        { "lstMyGetDir", "ListBox" },
        { "img1", "PictureBox" },
        { "ClearServerOpenDialog", "OpenFileDialog" },
        { "btnStartClear", "Button" },
        { "btnClearSave", "Button" },
        { "chkDynamicIPMode", "CheckBox" },
        { "chkIsCompress", "CheckBox" },
        { "chkAutoStart", "CheckBox" },
        { "GroupBox28", "GroupBox" },
        { "Label7", "Label" },
        { "Label8", "Label" },
        { "Label24", "Label" },
        { "Label25", "Label" },
        { "Label26", "Label" },
        { "Label27", "Label" },
        { "Label45", "Label" },
        { "Label46", "Label" },
        { "edtRunGate_DBPort1", "TextBox" },
        { "edtRunGate_DBPort2", "TextBox" },
        { "edtRunGate_DBPort3", "TextBox" },
        { "edtRunGate_DBPort4", "TextBox" },
        { "edtRunGate_DBPort5", "TextBox" },
        { "edtRunGate_DBPort6", "TextBox" },
        { "edtRunGate_DBPort7", "TextBox" },
        { "edtRunGate_DBPort8", "TextBox" },
        { "grp4", "GroupBox" },
        { "CheckBoxboRunGate_GetMultiThread", "CheckBox" },
        { "lbl14", "Label" },
        { "edtRunGate_DBPortMulThread", "TextBox" },
        { "grp5", "GroupBox" },
        { "lbl15", "Label" },
        { "sePortInc", "NumericUpDown" },
        { "btn2", "Button" },
        { "lbl16", "Label" },
        { "ts3", "TabPage" },
        { "grp6", "GroupBox" },
        { "chkTimerStart", "CheckBox" },
        { "chkEmbeddedWindow", "CheckBox" },
        { "GroupBox26", "GroupBox" },
        { "Label31", "Label" },
        { "Label32", "Label" },
        { "Label33", "Label" },
        { "Label34", "Label" },
        { "Label35", "Label" },
        { "Label36", "Label" },
        { "Label37", "Label" },
        { "Label38", "Label" },
        { "Label39", "Label" },
        { "Label40", "Label" },
        { "Label41", "Label" },
        { "Label42", "Label" },
        { "Label43", "Label" },
        { "Label44", "Label" },
        { "edtLoginAccount", "TextBox" },
        { "edtLoginAccountPasswd", "TextBox" },
        { "edtLoginAccountUserName", "TextBox" },
        { "edtLoginAccountSSNo", "TextBox" },
        { "edtLoginAccountBirthDay", "TextBox" },
        { "edtLoginAccountQuiz", "TextBox" },
        { "edtLoginAccountAnswer", "TextBox" },
        { "edtLoginAccountQuiz2", "TextBox" },
        { "edtLoginAccountAnswer2", "TextBox" },
        { "edtLoginAccountMobilePhone", "TextBox" },
        { "edtLoginAccountMemo", "TextBox" },
        { "edtLoginAccountEMail", "TextBox" },
        { "edtLoginAccountMemo2", "TextBox" },
        { "chkFullEditMode", "CheckBox" },
        { "ButtonLoginAccountOK", "Button" },
        { "edtLoginAccountPhone", "TextBox" },
        { "Panel1", "Panel" },
        { "Label30", "Label" },
        { "edtSearchLoginAccount", "TextBox" },
        { "ButtonSearchLoginAccount", "Button" },
        { "lbl17", "Label" },
        { "EditLoginGate_GatePort", "TextBox" },
        { "Label9", "Label" },
        { "Label10", "Label" },
        { "EditLoginGate_MainFormX", "NumericUpDown" },
        { "EditLoginGate_MainFormY", "NumericUpDown" },
        { "CheckBoxboLoginGate_GetMinimize", "CheckBox" },
        { "CheckBoxboSelGate_GetMinimize", "CheckBox" },
        { "CheckBoxboLoginServer_GetMinimize", "CheckBox" },
        { "CheckBoxDBServerGetMinimize", "CheckBox" },
        { "CheckBoxLogServerGetMinimize", "CheckBox" },
        { "CheckBoxM2ServerGetMinimize", "CheckBox" },
        { "pnlProgramWindow", "Panel" },
        { "grp7", "GroupBox" },
        { "EditHeroDB", "TextBox" },
        { "rbBDE", "RadioButton" },
        { "rbSqlite", "RadioButton" },
        { "edtSqliteDB", "TextBox" },
        { "Label2", "Label" },
        { "EditLoginServerControlPort", "TextBox" },
        { "btnSetPath", "Button" },
        { "grp8", "GroupBox" },
        { "rbDataSaveSqlite", "RadioButton" },
        { "grpDataSaveMySql", "GroupBox" },
        { "rbDataSaveMySql", "RadioButton" },
        { "lblSrcDBPort", "Label" },
        { "lblSrcDBServer", "Label" },
        { "edtDataSaveDBServer", "TextBox" },
        { "seDataSaveDBPort", "NumericUpDown" },
        { "lblSrcDBUser", "Label" },
        { "lblSrcDBPassword", "Label" },
        { "edtDataSaveDBUser", "TextBox" },
        { "edtDataSaveDBPassword", "TextBox" },
        { "Label28", "Label" },
        { "edtDataSaveDataBase", "TextBox" },
        { "lblMySqlLinkTest", "Label" },
        { "lblMySqlDoInit", "Label" },
        { "chkSelGate_GetMultiThread", "CheckBox" },
        { "Button1", "Button" },
        { "Button2", "Button" },
        { "dtpDate", "DateTimePicker" },
        { "dtpTime", "DateTimePicker" },
    };

    // ---------------- 常量 ----------------

    [Fact]
    public void ProgramName_MatchesPas()
    {
        Assert.Equal("引擎控制台", MainForm.sProgramName);
    }

    // ---------------- 字段覆盖 ----------------

    [Fact]
    public void FormFields_All362PresentWithExpectedType()
    {
        Assert.Equal(362, ExpectedFields.Count);
        var problems = new List<string>();
        var t = typeof(MainForm);
        foreach (var kv in ExpectedFields)
        {
            FieldInfo? f = t.GetField(kv.Key, BindingFlags.Public | BindingFlags.Instance);
            if (f == null) { problems.Add("missing: " + kv.Key); continue; }
            if (f.FieldType.Name != kv.Value)
                problems.Add($"type mismatch: {kv.Key} expected {kv.Value} actual {f.FieldType.Name}");
        }
        Assert.Empty(problems);
    }

    [Fact]
    public void FormFields_PublicInstanceFieldsCountIsExactly362()
    {
        var fields = new List<FieldInfo>();
        foreach (var f in typeof(MainForm).GetFields(BindingFlags.Public | BindingFlags.Instance))
            fields.Add(f);

        var extra = new List<string>();
        foreach (var f in fields)
            if (!ExpectedFields.ContainsKey(f.Name)) extra.Add(f.Name);

        Assert.True(extra.Count == 0, "unexpected public fields: " + string.Join(",", extra) + " total=" + fields.Count);
        Assert.Equal(362, fields.Count);
    }

    [Fact]
    public void FormFields_NoDuplicateNames()
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var f in typeof(MainForm).GetFields(BindingFlags.Public | BindingFlags.Instance))
            Assert.True(seen.Add(f.Name), "duplicate field: " + f.Name);
    }

    // ---------------- 私有成员 ----------------

    [Fact]
    public void PrivateMembers_FourFieldsDeclared()
    {
        var t = typeof(MainForm);
        Assert.NotNull(t.GetField("m_boOpen", BindingFlags.NonPublic | BindingFlags.Instance));
        Assert.NotNull(t.GetField("m_nStartStatus", BindingFlags.NonPublic | BindingFlags.Instance));
        Assert.NotNull(t.GetField("m_dwShowTick", BindingFlags.NonPublic | BindingFlags.Instance));
        Assert.NotNull(t.GetField("m_StartTime", BindingFlags.NonPublic | BindingFlags.Instance));
    }

    [Fact]
    public void PrivateMembers_DefaultsMatchPas()
    {
        Sta(() =>
        {
            using var f = new MainForm();
            Assert.False(f.Get_m_boOpen());
            Assert.Equal(0, f.Get_m_nStartStatus());
            Assert.Equal((uint)0, f.Get_m_dwShowTick());
            Assert.Equal(default, f.Get_m_StartTime());
        });
    }

    [Fact]
    public void PrivateMembers_RoundTripThroughAccessors()
    {
        Sta(() =>
        {
            using var f = new MainForm();
            f.Set_m_boOpen(true);
            f.Set_m_nStartStatus(2);
            f.Set_m_dwShowTick(1234);
            f.Set_m_StartTime(new DateTime(2026, 9, 19, 1, 2, 3));
            Assert.True(f.Get_m_boOpen());
            Assert.Equal(2, f.Get_m_nStartStatus());
            Assert.Equal((uint)1234, f.Get_m_dwShowTick());
            Assert.Equal(new DateTime(2026, 9, 19, 1, 2, 3), f.Get_m_StartTime());
        });
    }

    // ---------------- 例行程序清单 ----------------

    [Fact]
    public void RoutineIndex_Contains160Methods()
    {
        Assert.Equal(177, MainForm.MainPasRoutineCount);
        Assert.Equal(177, MainForm.MainPasRoutineIndex.Length);
    }

    [Fact]
    public void RoutineIndex_FirstAndLastEntriesMatchPas()
    {
        var r = MainForm.MainPasRoutineIndex;
        // GMain.pas:581 ListBoxAdd（实现段第一个例行程序）/ GMain.pas:7046 Button1Click
        Assert.Equal((581, "procedure", "ListBoxAdd"), (r[0].Line, r[0].Kind, r[0].Name));
        Assert.Equal((7046, "procedure", "TfrmMain.Button1Click"), (r[r.Length - 1].Line, r[r.Length - 1].Kind, r[r.Length - 1].Name));
    }

    [Fact]
    public void RoutineIndex_LinesAreStrictlyIncreasing()
    {
        var r = MainForm.MainPasRoutineIndex;
        for (int i = 1; i < r.Length; i++)
            Assert.True(r[i].Line > r[i - 1].Line, $"line order broken at {i}: {r[i - 1].Line} -> {r[i].Line}");
    }

    [Fact]
    public void RoutineIndex_ContainsKeyPortedRoutines()
    {
        var names = new HashSet<string>();
        foreach (var e in MainForm.MainPasRoutineIndex) names.Add(e.Name);

        foreach (string expected in new[]
        {
            "ListBoxAdd", "ListBoxDel", "ClearModValue", "ClearTxt", "TfrmMain.MainOutMessage",
            "Clear_SaveConfig", "Clear_LoadConfig", "ClearSetupIni", "ClearGlobal",
            "TfrmMain.GenGameConfig", "TfrmMain.GenDBServerConfig", "TfrmMain.GenBackupConfig",
            "TfrmMain.SaveBackList", "TfrmMain.LoadBackList", "TfrmMain.RefBackListToView",
            "TfrmMain.MainOutMessage",
            "GetClearAccountDBSql", "GetClearRoleDataDBSql", "GetClearUserShopSql",
            "GetClearStorageExSql", "GetClearAuctionDataSql",
        })
            Assert.Contains(expected, names);
    }

    // ---------------- MainOutMessage（GMain.pas:866-870） ----------------

    [Fact]
    public void MainOutMessage_PrefixesWithBracketedTimestamp()
    {
        Sta(() =>
        {
            using var f = new MainForm();
            f.MainOutMessage("开始");
            Assert.Single(f.MemoLogLines);
            Assert.StartsWith("[", f.MemoLogLines[0]);
            Assert.EndsWith("] 开始", f.MemoLogLines[0]);
        });
    }

    [Fact]
    public void MainOutMessage_UsesDateTimeToStrFormat()
    {
        string s = MainForm.DateTimeToStr(new DateTime(2026, 9, 19, 7, 5, 3));
        Assert.Equal("2026/9/19 7:05:03", s);
    }

    [Fact]
    public void MainOutMessage_HostHandlerTakesPrecedence()
    {
        Sta(() =>
        {
            using var f = new MainForm();
            string? captured = null;
            f.SetMemoLogAddHandler(s => captured = s);
            f.MainOutMessage("X");
            Assert.NotNull(captured);
            Assert.EndsWith("] X", captured);
            Assert.Empty(f.MemoLogLines);
        });
    }

    [Fact]
    public void MainOutMessage_EmptyMessageStillPrefixed()
    {
        Sta(() =>
        {
            using var f = new MainForm();
            f.MainOutMessage("");
            Assert.EndsWith("] ", f.MemoLogLines[0]);
        });
    }

    // ---------------- STA 执行器（与 GXX.M2Server.Tests 同约定） ----------------

    private static void Sta(Action action)
    {
        Exception? caught = null;
        var t = new System.Threading.Thread(() =>
        {
            try { action(); }
            catch (Exception ex) { caught = ex; }
        });
        t.SetApartmentState(System.Threading.ApartmentState.STA);
        t.Start();
        t.Join();
        if (caught != null)
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(caught).Throw();
    }
}
