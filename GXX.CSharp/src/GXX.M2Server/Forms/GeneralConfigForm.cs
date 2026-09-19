using GXX.Core.Rtl;
using GXX.Core.Util;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// GeneralConfig.pas TfrmGeneralConfig 1:1 转换（基本设置窗体：游戏设置/目录设置/网络设置/操作界面四页）。
/// 保存处理器的校验顺序、原文消息、焦点目标、g_Config 赋值与 INI 键名全部按 Delphi 源 1:1。
/// </summary>
public sealed class GeneralConfigForm : System.Windows.Forms.Form
{
    private bool boOpened;
    private bool boModValued;

    // ---- 控件（名称与 Delphi 1:1） ----
    public System.Windows.Forms.TabControl PageControl = null!;
    public System.Windows.Forms.TabPage ServerInfoSheet = null!;
    public System.Windows.Forms.TabPage ShareSheet = null!;
    public System.Windows.Forms.TabPage NetWorkSheet = null!;
    public System.Windows.Forms.TabPage TabSheet1 = null!;

    // 网络设置
    public System.Windows.Forms.GroupBox GroupBoxNet = null!;
    public System.Windows.Forms.Label LabelGateIPaddr = null!;
    public System.Windows.Forms.Label LabelGatePort = null!;
    public System.Windows.Forms.TextBox EditGateAddr = null!;
    public System.Windows.Forms.TextBox EditGatePort = null!;
    public System.Windows.Forms.Button ButtonNetWorkSave = null!;
    public System.Windows.Forms.GroupBox GroupBox1 = null!; // 数据库服务器
    public System.Windows.Forms.TextBox EditDBPort = null!;
    public System.Windows.Forms.TextBox EditDBAddr = null!;
    public System.Windows.Forms.GroupBox GroupBox2 = null!; // 登录服务器
    public System.Windows.Forms.TextBox EditIDSPort = null!;
    public System.Windows.Forms.TextBox EditIDSAddr = null!;
    public System.Windows.Forms.GroupBox GroupBox3 = null!; // 日志服务器
    public System.Windows.Forms.TextBox EditLogServerPort = null!;
    public System.Windows.Forms.TextBox EditLogServerAddr = null!;
    public System.Windows.Forms.GroupBox GroupBox4 = null!; // 游戏主服务器
    public System.Windows.Forms.TextBox EditMsgSrvPort = null!;
    public System.Windows.Forms.TextBox EditMsgSrvAddr = null!;

    // 游戏设置
    public System.Windows.Forms.GroupBox GroupBoxInfo = null!; // 基本参数
    public System.Windows.Forms.TextBox EditGameName = null!;
    public System.Windows.Forms.TextBox EditServerIndex = null!;
    public System.Windows.Forms.TextBox EditServerNumber = null!;
    public System.Windows.Forms.CheckBox CheckBoxServiceMode = null!;
    public System.Windows.Forms.GroupBox GroupBox5 = null!; // 免费模式
    public System.Windows.Forms.TextBox EditTestLevel = null!;
    public System.Windows.Forms.TextBox EditTestGold = null!;
    public System.Windows.Forms.TextBox EditTestUserLimit = null!;
    public System.Windows.Forms.CheckBox CheckBoxTestServer = null!;
    public System.Windows.Forms.Button ButtonServerInfoSave = null!;
    public System.Windows.Forms.GroupBox GroupBox6 = null!; // 最高上线人数
    public System.Windows.Forms.TextBox EditUserFull = null!;
    public System.Windows.Forms.GroupBox GroupBoxDBSrc = null!; // 游戏数据源名称
    public System.Windows.Forms.RadioButton rbBDE = null!;
    public System.Windows.Forms.RadioButton rbSqlite = null!;
    public System.Windows.Forms.TextBox EditDBName = null!;
    public System.Windows.Forms.TextBox edtSqlite = null!;
    public System.Windows.Forms.CheckBox chkShowBlockIPLog = null!;

    // 目录设置
    public System.Windows.Forms.GroupBox GroupBox7 = null!;
    public System.Windows.Forms.TextBox EditGuildDir = null!;
    public System.Windows.Forms.TextBox EditGuildFile = null!;
    public System.Windows.Forms.TextBox EditConLogDir = null!;
    public System.Windows.Forms.TextBox EditCastleDir = null!;
    public System.Windows.Forms.TextBox EditEnvirDir = null!;
    public System.Windows.Forms.TextBox EditMapDir = null!;
    public System.Windows.Forms.TextBox EditNoticeDir = null!;
    public System.Windows.Forms.TextBox EditPlugDir = null!;
    public System.Windows.Forms.TextBox EditVentureDir = null!;
    public System.Windows.Forms.TextBox edtBoxsDir = null!;
    public System.Windows.Forms.Button ButtonShareDirSave = null!;

    // 操作界面
    public System.Windows.Forms.GroupBox GroupBox8 = null!;
    public System.Windows.Forms.ComboBox ColorBoxHint = null!;
    public System.Windows.Forms.Label lbl1 = null!;

    public GeneralConfigForm()
    {
        InitializeComponent();
    }

    // ================= 布局（DFM 结构等效） =================

    private void InitializeComponent()
    {
        Text = "基本设置";
        Width = 560;
        Height = 470;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

        PageControl = new System.Windows.Forms.TabControl { Dock = System.Windows.Forms.DockStyle.Fill };
        PageControl.Selecting += (s, e) => e.Cancel = !PageControlChangingConfirm();
        Controls.Add(PageControl);

        // ---- 游戏设置（ServerInfoSheet） ----
        ServerInfoSheet = new System.Windows.Forms.TabPage { Text = "游戏设置" };
        PageControl.TabPages.Add(ServerInfoSheet);

        GroupBoxInfo = new System.Windows.Forms.GroupBox { Text = "基本参数", Left = 8, Top = 6, Width = 260, Height = 140 };
        AddLabeledEdit(GroupBoxInfo, "游戏名称:", 20, out EditGameName);
        AddLabeledEdit(GroupBoxInfo, "服务器号:", 46, out EditServerIndex);
        AddLabeledEdit(GroupBoxInfo, "服务器数:", 72, out EditServerNumber);
        CheckBoxServiceMode = new System.Windows.Forms.CheckBox { Text = "免费模式", Left = 14, Top = 100, Width = 120, AutoSize = true };
        CheckBoxServiceMode.CheckedChanged += (s, e) => EditValueChange(s);
        GroupBoxInfo.Controls.Add(CheckBoxServiceMode);
        ServerInfoSheet.Controls.Add(GroupBoxInfo);

        GroupBox5 = new System.Windows.Forms.GroupBox { Text = "免费模式", Left = 8, Top = 150, Width = 260, Height = 100 };
        AddLabeledEdit(GroupBox5, "开始等级:", 18, out EditTestLevel);
        AddLabeledEdit(GroupBox5, "开始金币:", 44, out EditTestGold);
        AddLabeledEdit(GroupBox5, "测试人数:", 70, out EditTestUserLimit);
        CheckBoxTestServer = new System.Windows.Forms.CheckBox { Text = "测试模式", Left = 150, Top = 18, Width = 100, AutoSize = true };
        CheckBoxTestServer.CheckedChanged += (s, e) => CheckBoxTestServerClick(s);
        GroupBox5.Controls.Add(CheckBoxTestServer);
        ServerInfoSheet.Controls.Add(GroupBox5);

        GroupBox6 = new System.Windows.Forms.GroupBox { Text = "最高上线人数", Left = 8, Top = 254, Width = 260, Height = 50 };
        AddLabeledEdit(GroupBox6, "上限人数:", 18, out EditUserFull);
        ServerInfoSheet.Controls.Add(GroupBox6);

        GroupBoxDBSrc = new System.Windows.Forms.GroupBox { Text = "游戏数据源名称", Left = 276, Top = 6, Width = 260, Height = 160 };
        rbBDE = new System.Windows.Forms.RadioButton { Text = "BDE数据库", Left = 14, Top = 18, Width = 120, AutoSize = true, Checked = true };
        rbSqlite = new System.Windows.Forms.RadioButton { Text = "Sqlite数据库", Left = 14, Top = 40, Width = 120, AutoSize = true };
        EditDBName = new System.Windows.Forms.TextBox { Left = 90, Top = 66, Width = 150 };
        edtSqlite = new System.Windows.Forms.TextBox { Left = 90, Top = 92, Width = 150 };
        chkShowBlockIPLog = new System.Windows.Forms.CheckBox { Text = "显示非法请求内部端口日志", Left = 14, Top = 120, Width = 240, AutoSize = true };
        rbBDE.Click += (s, e) => rbBDEClick(s);
        rbSqlite.Click += (s, e) => rbSqliteClick(s);
        EditDBName.TextChanged += (s, e) => EditValueChange(s);
        edtSqlite.TextChanged += (s, e) => EditValueChange(s);
        chkShowBlockIPLog.CheckedChanged += (s, e) => EditValueChange(s);
        GroupBoxDBSrc.Controls.AddRange(new System.Windows.Forms.Control[] { rbBDE, rbSqlite, EditDBName, edtSqlite, chkShowBlockIPLog });
        ServerInfoSheet.Controls.Add(GroupBoxDBSrc);

        ButtonServerInfoSave = new System.Windows.Forms.Button { Text = "保存(&S)", Left = 276, Top = 254, Width = 90, Height = 26, Enabled = false };
        ButtonServerInfoSave.Click += (s, e) => ButtonServerInfoSaveClick();
        ServerInfoSheet.Controls.Add(ButtonServerInfoSave);

        // ---- 目录设置（ShareSheet） ----
        ShareSheet = new System.Windows.Forms.TabPage { Text = "目录设置" };
        PageControl.TabPages.Add(ShareSheet);

        GroupBox7 = new System.Windows.Forms.GroupBox { Text = "目录设置", Left = 8, Top = 6, Width = 300, Height = 300 };
        int dy = 0;
        dy = AddLabeledEdit(GroupBox7, "行会目录:", 18, out EditGuildDir);
        dy = AddLabeledEdit(GroupBox7, "行会文件:", dy, out EditGuildFile);
        dy = AddLabeledEdit(GroupBox7, "功能插件:", dy, out EditPlugDir);
        dy = AddLabeledEdit(GroupBox7, "公告目录:", dy, out EditNoticeDir);
        dy = AddLabeledEdit(GroupBox7, "地图目录:", dy, out EditMapDir);
        dy = AddLabeledEdit(GroupBox7, "配置目录:", dy, out EditEnvirDir);
        dy = AddLabeledEdit(GroupBox7, "城堡目录:", dy, out EditCastleDir);
        dy = AddLabeledEdit(GroupBox7, "登录日志:", dy, out EditConLogDir);
        dy = AddLabeledEdit(GroupBox7, "Venture:", dy, out EditVentureDir);
        dy = AddLabeledEdit(GroupBox7, "宝箱目录:", dy, out edtBoxsDir);
        ShareSheet.Controls.Add(GroupBox7);

        ButtonShareDirSave = new System.Windows.Forms.Button { Text = "保存(&S)", Left = 320, Top = 30, Width = 90, Height = 26, Enabled = false };
        ButtonShareDirSave.Click += (s, e) => ButtonShareDirSaveClick();
        ShareSheet.Controls.Add(ButtonShareDirSave);

        // ---- 网络设置（NetWorkSheet） ----
        NetWorkSheet = new System.Windows.Forms.TabPage { Text = "网络设置" };
        PageControl.TabPages.Add(NetWorkSheet);

        GroupBoxNet = new System.Windows.Forms.GroupBox { Text = "网络接口", Left = 8, Top = 6, Width = 250, Height = 60 };
        AddLabeledEdit(GroupBoxNet, "绑定地址:", 18, out EditGateAddr, 110);
        AddLabeledEdit(GroupBoxNet, "网关端口:", 44, out EditGatePort, 110);
        NetWorkSheet.Controls.Add(GroupBoxNet);

        GroupBox1 = new System.Windows.Forms.GroupBox { Text = "数据库服务器", Left = 8, Top = 70, Width = 250, Height = 60 };
        AddLabeledEdit(GroupBox1, "服务器地址:", 18, out EditDBAddr, 110);
        AddLabeledEdit(GroupBox1, "服务器端口:", 44, out EditDBPort, 110);
        NetWorkSheet.Controls.Add(GroupBox1);

        GroupBox2 = new System.Windows.Forms.GroupBox { Text = "登录服务器", Left = 270, Top = 6, Width = 250, Height = 60 };
        AddLabeledEdit(GroupBox2, "服务器地址:", 18, out EditIDSAddr, 110);
        AddLabeledEdit(GroupBox2, "服务器端口:", 44, out EditIDSPort, 110);
        NetWorkSheet.Controls.Add(GroupBox2);

        GroupBox3 = new System.Windows.Forms.GroupBox { Text = "日志服务器", Left = 270, Top = 70, Width = 250, Height = 60 };
        AddLabeledEdit(GroupBox3, "服务器地址:", 18, out EditLogServerAddr, 110);
        AddLabeledEdit(GroupBox3, "服务器端口:", 44, out EditLogServerPort, 110);
        NetWorkSheet.Controls.Add(GroupBox3);

        GroupBox4 = new System.Windows.Forms.GroupBox { Text = "游戏主服务器", Left = 8, Top = 134, Width = 250, Height = 60 };
        AddLabeledEdit(GroupBox4, "服务器地址:", 18, out EditMsgSrvAddr, 110);
        AddLabeledEdit(GroupBox4, "服务器端口:", 44, out EditMsgSrvPort, 110);
        NetWorkSheet.Controls.Add(GroupBox4);

        ButtonNetWorkSave = new System.Windows.Forms.Button { Text = "保存(&S)", Left = 270, Top = 134, Width = 90, Height = 26, Enabled = false };
        ButtonNetWorkSave.Click += (s, e) => ButtonNetWorkSaveClick();
        NetWorkSheet.Controls.Add(ButtonNetWorkSave);

        // ---- 操作界面（TabSheet1） ----
        TabSheet1 = new System.Windows.Forms.TabPage { Text = "操作界面" };
        PageControl.TabPages.Add(TabSheet1);

        GroupBox8 = new System.Windows.Forms.GroupBox { Text = "操作界面", Left = 8, Top = 6, Width = 300, Height = 70 };
        lbl1 = new System.Windows.Forms.Label { Text = "弹出说明颜色", Left = 14, Top = 26, AutoSize = true };
        ColorBoxHint = new System.Windows.Forms.ComboBox { Left = 110, Top = 24, Width = 160, DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList };
        ColorBoxHint.SelectedIndexChanged += (s, e) => ColorBoxHintChange(s);
        GroupBox8.Controls.Add(lbl1);
        GroupBox8.Controls.Add(ColorBoxHint);
        TabSheet1.Controls.Add(GroupBox8);

        foreach (System.Windows.Forms.TextBox tb in new[] { EditGateAddr, EditGatePort, EditDBAddr, EditDBPort, EditIDSAddr, EditIDSPort,
                     EditLogServerAddr, EditLogServerPort, EditMsgSrvAddr, EditMsgSrvPort, EditGameName, EditServerIndex, EditServerNumber,
                     EditTestLevel, EditTestGold, EditTestUserLimit, EditUserFull, EditGuildDir, EditGuildFile, EditConLogDir, EditCastleDir,
                     EditEnvirDir, EditMapDir, EditNoticeDir, EditPlugDir, EditVentureDir, edtBoxsDir })
            tb.TextChanged += (s, e) => EditValueChange(s);
    }

    private int AddLabeledEdit(System.Windows.Forms.GroupBox parent, string caption, int top, out System.Windows.Forms.TextBox edit, int editLeft = 130)
    {
        var lb = new System.Windows.Forms.Label { Text = caption, Left = 12, Top = top + 3, AutoSize = true };
        edit = new System.Windows.Forms.TextBox { Left = editLeft, Top = top, Width = 130 };
        parent.Controls.Add(lb);
        parent.Controls.Add(edit);
        return top + 26;
    }

    // ================= Delphi 1:1 私有方法 =================

    private void ModValue()
    {
        boModValued = true;
        ButtonNetWorkSave.Enabled = true;
        ButtonServerInfoSave.Enabled = true;
        ButtonShareDirSave.Enabled = true;
    }

    private void uModValue()
    {
        boModValued = false;
        ButtonNetWorkSave.Enabled = false;
        ButtonServerInfoSave.Enabled = false;
        ButtonShareDirSave.Enabled = false;
    }

    /// <summary>测试观测点（boModValued）。</summary>
    public bool IsModValued => boModValued;

    /// <summary>测试入口：EditValueChange 路径（boOpened 门槛）。</summary>
    public void SimulateEdit()
    {
        boOpened = true;
        EditValueChange(this);
        boOpened = false;
    }

    // ================= 网络设置保存（ButtonNetWorkSaveClick 1:1） =================

    public (bool ok, string? focus) ButtonNetWorkSaveClick()
    {
        string Gateaddr = EditGateAddr.Text.Trim();
        int GatePort = DelphiRTL.StrToIntDef(EditGatePort.Text.Trim(), -1);
        string IDSAddr = EditIDSAddr.Text.Trim();
        int IDSPort = DelphiRTL.StrToIntDef(EditIDSPort.Text.Trim(), -1);
        string DBAddr = EditDBAddr.Text.Trim();
        int DBPort = DelphiRTL.StrToIntDef(EditDBPort.Text.Trim(), -1);
        string LogServerAddr = EditLogServerAddr.Text.Trim();
        int LogServerPort = DelphiRTL.StrToIntDef(EditLogServerPort.Text.Trim(), -1);
        string MsgSrvAddr = EditMsgSrvAddr.Text.Trim();
        int MsgSrvPort = DelphiRTL.StrToIntDef(EditMsgSrvPort.Text.Trim(), -1);

        if (!HUtil32.IsIPaddr(Gateaddr))
        {
            M2Forms.ErrorBox("网关地址设置错误！");
            EditGateAddr.Focus();
            return (false, nameof(EditGateAddr));
        }
        if (GatePort < 0 || GatePort > 65535)
        {
            M2Forms.ErrorBox("网关端口设置错误！");
            EditGatePort.Focus();
            return (false, nameof(EditGatePort));
        }
        if (!HUtil32.IsIPaddr(IDSAddr))
        {
            M2Forms.ErrorBox("管理服务器地址设置错误！");
            EditIDSAddr.Focus();
            return (false, nameof(EditIDSAddr));
        }
        if (IDSPort < 0 || IDSPort > 65535)
        {
            M2Forms.ErrorBox("管理服务器端口设置错误！");
            EditIDSPort.Focus();
            return (false, nameof(EditIDSPort));
        }
        if (!HUtil32.IsIPaddr(DBAddr))
        {
            M2Forms.ErrorBox("数据库服务器地址设置错误！");
            EditDBAddr.Focus();
            return (false, nameof(EditDBAddr));
        }
        if (DBPort < 0 || DBPort > 65535)
        {
            M2Forms.ErrorBox("数据库服务器端口设置错误！");
            EditDBPort.Focus();
            return (false, nameof(EditDBPort));
        }
        if (!HUtil32.IsIPaddr(LogServerAddr))
        {
            M2Forms.ErrorBox("日志服务器地址设置错误！");
            EditLogServerAddr.Focus();
            return (false, nameof(EditLogServerAddr));
        }
        if (LogServerPort < 0 || LogServerPort > 65535)
        {
            M2Forms.ErrorBox("日志服务器端口设置错误！");
            EditLogServerPort.Focus();
            return (false, nameof(EditLogServerPort));
        }
        if (!HUtil32.IsIPaddr(MsgSrvAddr))
        {
            M2Forms.ErrorBox("游戏主服务器地址设置错误！");
            EditMsgSrvAddr.Focus();
            return (false, nameof(EditMsgSrvAddr));
        }
        if (MsgSrvPort < 0 || MsgSrvPort > 65535)
        {
            M2Forms.ErrorBox("游戏主服务器端口设置错误！");
            EditMsgSrvPort.Focus();
            return (false, nameof(EditMsgSrvPort));
        }

        M2Config.sGateAddr = Gateaddr;
        M2Config.nGatePort = GatePort;
        M2Config.sIDSAddr = IDSAddr;
        M2Config.nIDSPort = IDSPort;
        M2Config.sDBAddr = DBAddr;
        M2Config.nDBPort = DBPort;
        M2Config.sLogServerAddr = LogServerAddr;
        M2Config.nLogServerPort = LogServerPort;
        M2Config.sMsgSrvAddr = MsgSrvAddr;
        M2Config.nMsgSrvPort = MsgSrvPort;

        var Config = M2ShareState.ConfigIni;
        Config.WriteString("Server", "GateAddr", M2Config.sGateAddr);
        Config.WriteInteger("Server", "GatePort", M2Config.nGatePort);
        Config.WriteString("Server", "IDSAddr", M2Config.sIDSAddr);
        Config.WriteInteger("Server", "IDSPort", M2Config.nIDSPort);
        Config.WriteString("Server", "DBAddr", M2Config.sDBAddr);
        Config.WriteInteger("Server", "DBPort", M2Config.nDBPort);
        Config.WriteString("Server", "LogServerAddr", M2Config.sLogServerAddr);
        Config.WriteInteger("Server", "LogServerPort", M2Config.nLogServerPort);
        Config.WriteString("Server", "MsgSrvAddr", M2Config.sMsgSrvAddr);
        Config.WriteInteger("Server", "MsgSrvPort", M2Config.nMsgSrvPort);
        Config.UpdateFile();
        uModValue();
        return (true, null);
    }

    // ================= 游戏设置保存（ButtonServerInfoSaveClick 1:1） =================

    public (bool ok, string? focus) ButtonServerInfoSaveClick()
    {
        string GameName = EditGameName.Text.Trim();
        int ServerIndex = DelphiRTL.StrToIntDef(EditServerIndex.Text.Trim(), -1);
        int ServerNumber = DelphiRTL.StrToIntDef(EditServerNumber.Text.Trim(), -1);
        bool ServiceMode = CheckBoxServiceMode.Checked;
        bool TestServer = CheckBoxTestServer.Checked;
        int TestLevel = DelphiRTL.StrToIntDef(EditTestLevel.Text.Trim(), -1);
        int TestGold = DelphiRTL.StrToIntDef(EditTestGold.Text.Trim(), -1);
        int TestUserLimit = DelphiRTL.StrToIntDef(EditTestUserLimit.Text.Trim(), -1);
        int UserFull = DelphiRTL.StrToIntDef(EditUserFull.Text.Trim(), -1);

        string DBName = EditDBName.Text.Trim();
        string SqliteDBName = edtSqlite.Text.Trim();

        if (GameName == "")
        {
            M2Forms.ErrorBox("游戏名称设置错误！");
            EditGameName.Focus();
            return (false, nameof(EditGameName));
        }
        if (ServerIndex < 0 || ServerIndex > 255)
        {
            M2Forms.ErrorBox("服务器号设置错误！");
            EditServerIndex.Focus();
            return (false, nameof(EditServerIndex));
        }
        if (ServerNumber < 0 || ServerNumber > 255)
        {
            M2Forms.ErrorBox("服务器数设置错误！");
            EditServerNumber.Focus();
            return (false, nameof(EditServerNumber));
        }
        if (TestLevel < 0 || TestLevel > 65535)
        {
            M2Forms.ErrorBox("开始等级设置错误！");
            EditTestLevel.Focus();
            return (false, nameof(EditTestLevel));
        }
        if (TestGold < 0 || TestGold > int.MaxValue / 2)
        {
            M2Forms.ErrorBox("开始金币设置错误！");
            EditTestGold.Focus();
            return (false, nameof(EditTestGold));
        }
        if (TestUserLimit < 0 || TestUserLimit > 10000)
        {
            M2Forms.ErrorBox("测试人数设置错误！");
            EditTestUserLimit.Focus();
            return (false, nameof(EditTestUserLimit));
        }
        if (UserFull < 0 || UserFull > 10000)
        {
            M2Forms.ErrorBox("上限人数设置错误！");
            EditUserFull.Focus();
            return (false, nameof(EditUserFull));
        }
        if (rbBDE.Checked)
        {
            if (DBName == "")
            {
                M2Forms.ErrorBox("数据库名称设置错误！");
                EditDBName.Focus();
                return (false, nameof(EditDBName));
            }
        }
        else
        {
            if (!File.Exists(SqliteDBName))
            {
                M2Forms.ErrorBox("数据库文件设置错误！");
                edtSqlite.Focus();
                return (false, nameof(edtSqlite));
            }
        }

        M2Config.sServerName = GameName;
        // nServerIndex:=ServerIndex;（Delphi 原句被注释，仅校验不赋值）
        M2Config.nServerNumber = ServerNumber;
        M2Config.boServiceMode = ServiceMode;
        M2Config.boTestServer = TestServer;
        M2Config.nTestLevel = TestLevel;
        M2Config.nTestGold = TestGold;
        M2Config.nTestUserLimit = TestUserLimit;
        M2Config.nUserFull = UserFull;

        M2ShareState.g_boUseSqliteDB = rbSqlite.Checked;
        M2ShareState.g_sDBName = DBName;
        M2ShareState.g_sSqliteDBName = SqliteDBName;
        M2ShareState.g_boShowBlockIPLog = chkShowBlockIPLog.Checked;

        var Config = M2ShareState.ConfigIni;
        Config.WriteString("Server", "ServerName", M2Config.sServerName);
        Config.WriteInteger("Server", "ServerIndex", M2ShareState.nServerIndex);
        Config.WriteInteger("Server", "ServerNumber", M2Config.nServerNumber);
        Config.WriteString("Server", "TestServer", DelphiBoolToStr(M2Config.boTestServer));
        Config.WriteInteger("Server", "TestLevel", M2Config.nTestLevel);
        Config.WriteInteger("Server", "TestGold", M2Config.nTestGold);
        Config.WriteInteger("Server", "TestServerUserLimit", M2Config.nTestUserLimit);
        Config.WriteInteger("Server", "UserFull", M2Config.nUserFull);
        Config.WriteBool("Setup", "UseSqliteDB", M2ShareState.g_boUseSqliteDB);
        Config.WriteString("Server", "DBName", M2ShareState.g_sDBName);
        Config.WriteString("Server", "SqliteDBName", M2ShareState.g_sSqliteDBName);
        Config.WriteBool("Server", "ShowBlockIPLog", M2ShareState.g_boShowBlockIPLog);
        Config.UpdateFile();
        uModValue();
        return (true, null);
    }

    /// <summary>Delphi BoolToStr 默认形态（'-1'/'0'）。</summary>
    private static string DelphiBoolToStr(bool b) => b ? "-1" : "0";

    // ================= 目录设置保存（ButtonShareDirSaveClick 1:1） =================

    public (bool ok, string? focus) ButtonShareDirSaveClick()
    {
        string GuildDir = EditGuildDir.Text.Trim();
        string GuildFile = EditGuildFile.Text.Trim();
        string VentureDir = EditVentureDir.Text.Trim();
        string ConLogDir = EditConLogDir.Text.Trim();
        string CastleDir = EditCastleDir.Text.Trim();
        string EnvirDir = EditEnvirDir.Text.Trim();
        string MapDir = EditMapDir.Text.Trim();
        string NoticeDir = EditNoticeDir.Text.Trim();
        string PlugDir = EditPlugDir.Text.Trim();
        string BoxsDir = edtBoxsDir.Text.Trim();

        if (!Directory.Exists(GuildDir) || !GuildDir.EndsWith("\\"))
        {
            M2Forms.ErrorBox("行会目录设置错误！");
            EditGuildDir.Focus();
            return (false, nameof(EditGuildDir));
        }
        if (!File.Exists(GuildFile))
        {
            M2Forms.ErrorBox("行会文件设置错误！");
            EditGuildFile.Focus();
            return (false, nameof(EditGuildFile));
        }
        if (!Directory.Exists(VentureDir) || !VentureDir.EndsWith("\\"))
        {
            M2Forms.ErrorBox("Venture目录设置错误！");
            EditVentureDir.Focus();
            return (false, nameof(EditVentureDir));
        }
        if (!Directory.Exists(ConLogDir) || !ConLogDir.EndsWith("\\"))
        {
            M2Forms.ErrorBox("登录日志目录设置错误！");
            EditConLogDir.Focus();
            return (false, nameof(EditConLogDir));
        }
        if (!Directory.Exists(CastleDir) || !CastleDir.EndsWith("\\"))
        {
            M2Forms.ErrorBox("城堡目录设置错误！");
            EditCastleDir.Focus();
            return (false, nameof(EditCastleDir));
        }
        if (!Directory.Exists(EnvirDir) || !EnvirDir.EndsWith("\\"))
        {
            M2Forms.ErrorBox("配置目录设置错误！");
            EditEnvirDir.Focus();
            return (false, nameof(EditEnvirDir));
        }
        if (!Directory.Exists(MapDir) || !MapDir.EndsWith("\\"))
        {
            M2Forms.ErrorBox("地图目录设置错误！");
            EditMapDir.Focus();
            return (false, nameof(EditMapDir));
        }
        if (!Directory.Exists(NoticeDir) || !NoticeDir.EndsWith("\\"))
        {
            M2Forms.ErrorBox("公告目录设置错误！");
            EditNoticeDir.Focus();
            return (false, nameof(EditNoticeDir));
        }
        if (!Directory.Exists(PlugDir) || !PlugDir.EndsWith("\\"))
        {
            M2Forms.ErrorBox("插件目录设置错误！");
            EditPlugDir.Focus();
            return (false, nameof(EditPlugDir));
        }
        if (!Directory.Exists(BoxsDir) || !BoxsDir.EndsWith("\\"))
        {
            M2Forms.ErrorBox("宝箱目录设置错误！");
            edtBoxsDir.Focus();
            return (false, nameof(edtBoxsDir));
        }

        M2Config.sGuildDir = GuildDir;
        M2Config.sGuildFile = GuildFile;
        M2Config.sVentureDir = VentureDir;
        M2Config.sConLogDir = ConLogDir;
        M2Config.sCastleDir = CastleDir;
        M2Config.sEnvirDir = EnvirDir;
        M2Config.sMapDir = MapDir;
        M2Config.sNoticeDir = NoticeDir;
        M2Config.sPlugDir = PlugDir;
        M2Config.sBoxsDir = BoxsDir;

        var Config = M2ShareState.ConfigIni;
        Config.WriteString("Share", "GuildDir", M2Config.sGuildDir);
        Config.WriteString("Share", "GuildFile", M2Config.sGuildFile);
        Config.WriteString("Share", "VentureDir", M2Config.sVentureDir);
        Config.WriteString("Share", "ConLogDir", M2Config.sConLogDir);
        Config.WriteString("Share", "CastleDir", M2Config.sCastleDir);
        Config.WriteString("Share", "EnvirDir", M2Config.sEnvirDir);
        Config.WriteString("Share", "MapDir", M2Config.sMapDir);
        Config.WriteString("Share", "NoticeDir", M2Config.sNoticeDir);
        Config.WriteString("Share", "PlugDir", M2Config.sPlugDir);
        Config.WriteString("Share", "BoxsDir", M2Config.sBoxsDir);
        Config.UpdateFile();
        uModValue();
        return (true, null);
    }

    // ================= 打开 / 联动（1:1） =================

    /// <summary>Delphi Open()；showModal=false 供测试/宿主复用（Delphi 末行 ShowModal）。</summary>
    public void Open(bool showModal = true)
    {
        boOpened = false;
        uModValue();
        EditGateAddr.Text = M2Config.sGateAddr;
        EditGatePort.Text = M2Config.nGatePort.ToString();
        EditIDSAddr.Text = M2Config.sIDSAddr;
        EditIDSPort.Text = M2Config.nIDSPort.ToString();
        EditDBAddr.Text = M2Config.sDBAddr;
        EditDBPort.Text = M2Config.nDBPort.ToString();
        EditLogServerAddr.Text = M2Config.sLogServerAddr;
        EditLogServerPort.Text = M2Config.nLogServerPort.ToString();
        EditMsgSrvAddr.Text = M2Config.sMsgSrvAddr;
        EditMsgSrvPort.Text = M2Config.nMsgSrvPort.ToString();

        EditGameName.Text = M2Config.sServerName;
        EditServerIndex.Text = M2ShareState.nServerIndex.ToString();
        EditServerNumber.Text = M2Config.nServerNumber.ToString();
        CheckBoxServiceMode.Checked = M2Config.boServiceMode;
        CheckBoxTestServer.Checked = M2Config.boTestServer;
        EditTestLevel.Text = M2Config.nTestLevel.ToString();
        EditTestGold.Text = M2Config.nTestGold.ToString();
        EditTestUserLimit.Text = M2Config.nTestUserLimit.ToString();
        EditUserFull.Text = M2Config.nUserFull.ToString();
        CheckBoxTestServerClick(this);
        EditDBName.Text = M2ShareState.g_sDBName;
        edtSqlite.Text = M2ShareState.g_sSqliteDBName;
        chkShowBlockIPLog.Checked = M2ShareState.g_boShowBlockIPLog;

        rbBDE.Checked = !M2ShareState.g_boUseSqliteDB;
        rbSqlite.Checked = M2ShareState.g_boUseSqliteDB;

        EditDBName.Enabled = !M2ShareState.g_boUseSqliteDB;
        rbSqlite.Enabled = M2ShareState.g_boUseSqliteDB;

        EditGuildDir.Text = M2Config.sGuildDir;
        EditGuildFile.Text = M2Config.sGuildFile;
        EditConLogDir.Text = M2Config.sConLogDir;
        EditCastleDir.Text = M2Config.sCastleDir;
        EditEnvirDir.Text = M2Config.sEnvirDir;
        EditMapDir.Text = M2Config.sMapDir;
        EditNoticeDir.Text = M2Config.sNoticeDir;
        EditPlugDir.Text = M2Config.sPlugDir;
        EditVentureDir.Text = M2Config.sVentureDir;
        edtBoxsDir.Text = M2Config.sBoxsDir;

        RefDlgConf();

        boOpened = true;
        PageControl.SelectedIndex = 0;
        if (showModal)
            ShowDialog();
    }

    public void EditValueChange(object? sender)
    {
        if (!boOpened) return;
        ModValue();
    }

    /// <summary>PageControlChanging 1:1：修改未保存时确认；返回 AllowChange。测试注入 M2Forms.NextAnswer。</summary>
    public bool PageControlChangingConfirm()
    {
        if (boModValued)
        {
            if (M2Forms.MessageBox("参数设置已经被修改，是否确认不保存修改的设置？", "确认信息", M2Forms.MB_YESNO | M2Forms.MB_ICONQUESTION) == M2Forms.IDYES)
                uModValue();
            else
                return false;
        }
        return true;
    }

    public void CheckBoxTestServerClick(object? sender)
    {
        bool boStatue = CheckBoxTestServer.Checked;
        EditTestLevel.Enabled = boStatue;
        EditTestGold.Enabled = boStatue;
        EditTestUserLimit.Enabled = boStatue;
        EditValueChange(sender);
    }

    public void RefDlgConf()
    {
        ColorBoxHint.BackColor = M2Forms.HintColor;
    }

    public void ColorBoxHintChange(object? sender)
    {
        M2Forms.HintColor = ColorBoxHint.BackColor;
    }

    public void rbBDEClick(object? sender)
    {
        EditDBName.Enabled = rbBDE.Checked;
        edtSqlite.Enabled = !rbBDE.Checked;
    }

    public void rbSqliteClick(object? sender)
    {
        EditDBName.Enabled = rbBDE.Checked;
        edtSqlite.Enabled = !rbBDE.Checked;
    }
}
