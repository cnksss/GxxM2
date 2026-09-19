using System;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.LoginSrv;

/// <summary>
/// BasicSet.pas TFrmBasicSet 1:1（登录服基本设置）。
/// 源码：BasicSet.pas:350-886；DFM：BasicSet.dfm（Caption='基本设置', PageControl 4 页）。
/// TSpinEdit → NumericUpDown；TPageControl → TabControl；INI 键序/默认值/校验分支 1:1。
/// </summary>
public sealed class TFrmBasicSet : System.Windows.Forms.Form
{
    // ---- resourcestring（BasicSet.pas:160-231，1:1）----
    private const string sSectionServer = "Server";
    private const string sSectionDB = "DB";
    private const string sSectionDataSaveDB = "DataSaveDB";

    private const string sIdentDBServer = "DBServer";
    private const string sIdentFeeServer = "FeeServer";
    private const string sIdentLogServer = "LogServer";
    private const string sIdentGateAddr = "GateAddr";
    private const string sIdentGatePort = "GatePort";
    private const string sIdentServerAddr = "ServerAddr";
    private const string sIdentServerName = "ServerName";
    private const string sIdentServerPort = "ServerPort";
    private const string sIdentMonAddr = "MonAddr";
    private const string sIdentMonPort = "MonPort";
    private const string sIdentDBSPort = "DBSPort";
    private const string sIdentFeePort = "FeePort";
    private const string sIdentLogPort = "LogPort";

    private const string sIdentControlPort = "ControlPort";
    private const string sIdentControlPassword = "ControlPassword";

    private const string sIdentReadyServers = "ReadyServers";
    private const string sIdentTestServer = "TestServer";
    private const string sIdentDynamicIPMode = "DynamicIPMode";
    private const string sIdentIdDir = "IdDir";
    private const string sIdentWebLogDir = "WebLogDir";
    private const string sIdentCountLogDir = "CountLogDir";
    private const string sIdentFeedIDList = "FeedIDList";
    private const string sIdentFeedIPList = "FeedIPList";
    private const string sIdentShowBlockIPLog = "ShowBlockIPLog";

    private const string sIdentEnableMakingID = "EnableMakingID";
    private const string sIdentEnableGetbackPassword = "GetbackPassword";
    private const string sIdentGetbackPasswordCheckAll = "GetbackPasswordCheckAll";
    private const string sIdentDisableIDSamePassword = "DisableIDSamePassword";
    private const string sIdentDisableQuizSameAnswer = "DisableQuizSameAnswer";
    private const string sIdentDisableIDSameL2Password = "DisableIDSameL2Password";
    // ★ 原文如此（BasicSet.pas:198）：常量名是 DisableIDSameL2Password，值却写成 'sIdentDisableL2SamePassword'
    private const string sIdentDisableL2SamePassword = "sIdentDisableL2SamePassword";

    private const string sIdentDisablePwdSameChr = "DisablePasswordSameChr";
    private const string sIdentDisablePwdAllNum = "DisablePasswordAllNum";
    private const string sIdentDisablePwdAllLetter = "DisablePasswordAllLetter";

    private const string sIdentAutoClearID = "AutoClearID";
    private const string sIdentAutoClearTime = "AutoClearTime";
    private const string sIdentUnLockAccount = "UnLockAccount";
    private const string sIdentUnLockAccountTime = "UnLockAccountTime";
    private const string sIdentMinimize = "Minimize";

    private const string sIdentRandomCodeErrorMaxCount = "RandomCodeErrorMaxCount";
    private const string sIdentRandomCodeRefreshMaxCount = "RandomCodeRefreshMaxCount";

    private const string sIdentLoginWaveValue = "LoginWaveValue";
    private const string sIdentOtherWaveValue = "OtherWaveValue";

    private const string sIdentEnabledL2Password = "EnabledL2Password";
    private const string sIdentChangedMACCheckL2 = "ChangedMACCheckL2";
    private const string sIdentChangedIPCheckL2 = "ChangedIPCheckL2";
    private const string sIdentAlwaysCheckL2 = "AlwaysCheckL2";

    private const string sIdentDataSaveDBType = "DataSaveDBType";
    private const string sIdentDataSaveDBServer = "DataSaveDBServer";
    private const string sIdentDataSaveDBPort = "DataSaveDBPort";
    private const string sIdentDataSaveDBUser = "DataSaveDBUser";
    private const string sIdentDataSaveDBPassword = "DataSaveDBPassword";
    private const string sIdentDataSaveDataBase = "DataSaveDataBase";

    private const string sIdentNewLoginDlg = "NewLoginDlg";
    private const string sIdentNewLoginInto = "NewLoginInto";
    private const string sIdentNewLoginPhone = "NewLoginPhone";
    private const string sIdentNewLoginMustHasPhone = "NewLoginMustHasPhone";

    /// <summary>sIdentRandomCode: array[TRandCodeType] of string（BasicSet.pas:233）。</summary>
    private static readonly string[] sIdentRandomCode =
        { "RandCodeLogin", "RandCodeLoginReg", "RandCodePwdGetback", "RandCodePwdChange" };

    // ---- 控件（DFM 1:1）----
    public System.Windows.Forms.TabControl PageControl1 = null!;
    public System.Windows.Forms.TabPage TabSheet1 = null!;   // 普通设置
    public System.Windows.Forms.TabPage TabSheet2 = null!;   // 网络设置
    public System.Windows.Forms.TabPage TabSheet3 = null!;   // 远程管理设置
    public System.Windows.Forms.TabPage ts1 = null!;         // 密码扩展设置

    public System.Windows.Forms.GroupBox GroupBox1 = null!;  // 功能设置
    public System.Windows.Forms.GroupBox GroupBox2 = null!;  // 清理账号设置
    public System.Windows.Forms.CheckBox CheckBoxTestServer = null!;
    public System.Windows.Forms.CheckBox CheckBoxEnableMakingID = null!;
    public System.Windows.Forms.CheckBox CheckBoxEnableGetbackPassword = null!;
    public System.Windows.Forms.CheckBox CheckBoxAutoClear = null!;
    public System.Windows.Forms.Label Label1 = null!;
    public System.Windows.Forms.Label Label2 = null!;
    public System.Windows.Forms.Button ButtonSave = null!;
    public System.Windows.Forms.Button ButtonClose = null!;
    public System.Windows.Forms.NumericUpDown SpinEditAutoClearTime = null!;
    public System.Windows.Forms.Button ButtonRestoreBasic = null!;
    public System.Windows.Forms.Button ButtonRestoreNet = null!;

    public System.Windows.Forms.GroupBox GroupBox3 = null!;  // 网关设置
    public System.Windows.Forms.GroupBox GroupBox4 = null!;  // 远程监控设置
    public System.Windows.Forms.GroupBox GroupBox5 = null!;  // 服务器网络设置
    public System.Windows.Forms.Label Label3 = null!;
    public System.Windows.Forms.Label Label4 = null!;
    public System.Windows.Forms.TextBox EditGateAddr = null!;
    public System.Windows.Forms.TextBox EditGatePort = null!;
    public System.Windows.Forms.Label Label5 = null!;
    public System.Windows.Forms.Label Label6 = null!;
    public System.Windows.Forms.TextBox EditMonAddr = null!;
    public System.Windows.Forms.TextBox EditMonPort = null!;
    public System.Windows.Forms.Label Label7 = null!;
    public System.Windows.Forms.Label Label8 = null!;
    public System.Windows.Forms.TextBox EditServerAddr = null!;
    public System.Windows.Forms.TextBox EditServerPort = null!;

    public System.Windows.Forms.GroupBox GroupBox8 = null!;  // 开启验证码
    public System.Windows.Forms.Label Label11 = null!;
    public System.Windows.Forms.CheckBox chkRandomCodeLogin = null!;
    public System.Windows.Forms.NumericUpDown EditRandomCodeErrorMaxCount = null!;
    public System.Windows.Forms.CheckBox chkDisableIDSamePassword = null!;
    public System.Windows.Forms.CheckBox chkDisableQuizSameAnswer = null!;
    public System.Windows.Forms.CheckBox CheckBoxDynamicIPMode = null!;

    public System.Windows.Forms.GroupBox GroupBox9 = null!;  // 远程管理设置
    public System.Windows.Forms.Label Label12 = null!;
    public System.Windows.Forms.Label Label13 = null!;
    public System.Windows.Forms.TextBox edtControlPort = null!;
    public System.Windows.Forms.TextBox edtControlPassword = null!;
    public System.Windows.Forms.GroupBox grp1 = null!;       // 允许连接IP
    public System.Windows.Forms.ListBox lstControlIPList = null!;
    public System.Windows.Forms.ContextMenuStrip pmControlIPList = null!;
    public System.Windows.Forms.ToolStripMenuItem mniIPAdd = null!;
    public System.Windows.Forms.ToolStripMenuItem mniIPDelete = null!;
    public System.Windows.Forms.ToolStripMenuItem mniIPClear = null!;
    public System.Windows.Forms.Label lbl1 = null!;
    public System.Windows.Forms.Label Label14 = null!;
    public System.Windows.Forms.CheckBox CheckBoxGetbackPasswordCheckAll = null!;

    public System.Windows.Forms.GroupBox grpL2Password = null!;   // 二级密码校验选项
    public System.Windows.Forms.CheckBox chkChangedMACCheckL2 = null!;
    public System.Windows.Forms.CheckBox chkChangedIPCheckL2 = null!;
    public System.Windows.Forms.CheckBox chkAlwaysCheckL2 = null!;

    public System.Windows.Forms.GroupBox GroupBox7 = null!;       // 自动解除锁定账号
    public System.Windows.Forms.Label Label9 = null!;
    public System.Windows.Forms.Label Label10 = null!;
    public System.Windows.Forms.CheckBox CheckBoxAutoUnLockAccount = null!;
    public System.Windows.Forms.NumericUpDown SpinEditUnLockAccountTime = null!;
    public System.Windows.Forms.CheckBox chkDisableIDSameL2Password = null!;
    public System.Windows.Forms.CheckBox chkDisableL2SamePassword = null!;
    public System.Windows.Forms.CheckBox chkEnabledL2Password = null!;

    public System.Windows.Forms.CheckBox chkDisablePwdSameChr = null!;
    public System.Windows.Forms.CheckBox chkDisablePwdAllNum = null!;
    public System.Windows.Forms.Label Label15 = null!;
    public System.Windows.Forms.TextBox mmoDisablePassword = null!;
    public System.Windows.Forms.CheckBox chkDisablePwdAllLetter = null!;
    public System.Windows.Forms.Label Label16 = null!;
    public System.Windows.Forms.NumericUpDown seRandomCodeRefreshMaxCount = null!;
    public System.Windows.Forms.CheckBox chkRandomCodePwdGetback = null!;
    public System.Windows.Forms.CheckBox chkRandomCodePwdChange = null!;
    public System.Windows.Forms.CheckBox chkRandomCodeReg = null!;
    public System.Windows.Forms.Label lbl2 = null!;
    public System.Windows.Forms.NumericUpDown seLoginWaveValue = null!;
    public System.Windows.Forms.Label Label17 = null!;
    public System.Windows.Forms.NumericUpDown seOtherWaveValue = null!;
    public System.Windows.Forms.CheckBox chkShowBlockIPLog = null!;

    public System.Windows.Forms.GroupBox GroupBox6 = null!;   // 新版登陆
    public System.Windows.Forms.CheckBox chkNewLoginDlg = null!;
    public System.Windows.Forms.CheckBox chkNewLoginInto = null!;
    public System.Windows.Forms.CheckBox chkNewLoginPhone = null!;

    public TFrmBasicSet()
    {
        InitializeComponent();
        FormCreate(this);
    }

    private static System.Windows.Forms.NumericUpDown Spin(int left, int top, int width, int max, int min, int value, int tab)
        => new()
        {
            Left = left,
            Top = top,
            Width = width,
            Height = 21,
            Maximum = max,
            Minimum = min,
            Value = value,
            TabIndex = tab,
        };

    /// <summary>Delphi TSpinEdit.SetValue：越界先夹到 [MinValue, MaxValue] 再赋值（1:1）。</summary>
    private static void SetSpin(System.Windows.Forms.NumericUpDown spin, decimal v)
    {
        if (v < spin.Minimum) v = spin.Minimum;
        else if (v > spin.Maximum) v = spin.Maximum;
        spin.Value = v;
    }

    private static System.Windows.Forms.CheckBox Chk(string caption, int left, int top, int width, int tab = 0)
        => new() { Text = caption, Left = left, Top = top, Width = width, Height = 17, TabIndex = tab, AutoSize = false };

    private static System.Windows.Forms.Label Lbl(string caption, int left, int top, int width)
        => new() { Text = caption, Left = left, Top = top, Width = width, Height = 12, AutoSize = false };

    private void InitializeComponent()
    {
        // DFM: Caption = '基本设置'
        Text = "基本设置";
        // DFM: ClientWidth/ClientHeight 由 PageControl(401x329) + 按钮区推出
        ClientSize = new System.Drawing.Size(417, 376);

        PageControl1 = new System.Windows.Forms.TabControl { Left = 8, Top = 8, Width = 401, Height = 329, TabIndex = 0 };
        TabSheet1 = new System.Windows.Forms.TabPage { Text = "普通设置" };
        TabSheet2 = new System.Windows.Forms.TabPage { Text = "网络设置" };
        TabSheet3 = new System.Windows.Forms.TabPage { Text = "远程管理设置" };
        ts1 = new System.Windows.Forms.TabPage { Text = "密码扩展设置" };
        PageControl1.TabPages.Add(TabSheet1);
        PageControl1.TabPages.Add(TabSheet2);
        PageControl1.TabPages.Add(TabSheet3);
        PageControl1.TabPages.Add(ts1);

        // ---------------- TabSheet1 普通设置 ----------------
        GroupBox1 = new System.Windows.Forms.GroupBox { Text = "功能设置", Left = 6, Top = 2, Width = 167, Height = 159, TabIndex = 0 };
        CheckBoxTestServer = Chk("测试模式", 8, 15, 156, 0);
        CheckBoxTestServer.Click += (s, e) => CheckBoxTestServerClick(s);
        CheckBoxEnableMakingID = Chk("允许创建账号", 8, 32, 156, 1);
        CheckBoxEnableMakingID.Click += (s, e) => CheckBoxEnableMakingIDClick(s);
        CheckBoxEnableGetbackPassword = Chk("允许取回密码", 8, 50, 156, 2);
        CheckBoxEnableGetbackPassword.Click += (s, e) => CheckBoxEnableGetbackPasswordClick(s);
        chkDisableIDSamePassword = Chk("禁止'ID'密码相同", 8, 67, 156, 3);
        chkDisableIDSamePassword.Click += (s, e) => chkDisableIDSamePasswordClick(s);
        chkDisableQuizSameAnswer = Chk("禁止问题答案相同", 8, 84, 156, 4);
        chkDisableQuizSameAnswer.Click += (s, e) => chkDisableQuizSameAnswerClick(s);
        CheckBoxGetbackPasswordCheckAll = Chk("找回密码须密保完全正确", 8, 101, 156, 5);
        CheckBoxGetbackPasswordCheckAll.Click += (s, e) => CheckBoxGetbackPasswordCheckAllClick(s);
        chkDisableIDSameL2Password = Chk("禁止二级密码和'ID'相同", 8, 119, 156, 6);
        chkDisableIDSameL2Password.Click += (s, e) => chkDisableIDSameL2PasswordClick(s);
        chkDisableL2SamePassword = Chk("禁止二级密码和密码相同", 8, 136, 156, 7);
        chkDisableL2SamePassword.Click += (s, e) => chkDisableL2SamePasswordClick(s);
        GroupBox1.Controls.Add(CheckBoxTestServer);
        GroupBox1.Controls.Add(CheckBoxEnableMakingID);
        GroupBox1.Controls.Add(CheckBoxEnableGetbackPassword);
        GroupBox1.Controls.Add(chkDisableIDSamePassword);
        GroupBox1.Controls.Add(chkDisableQuizSameAnswer);
        GroupBox1.Controls.Add(CheckBoxGetbackPasswordCheckAll);
        GroupBox1.Controls.Add(chkDisableIDSameL2Password);
        GroupBox1.Controls.Add(chkDisableL2SamePassword);

        GroupBox2 = new System.Windows.Forms.GroupBox { Text = "清理账号设置", Left = 6, Top = 162, Width = 168, Height = 37, TabIndex = 1 };
        Label1 = Lbl("间隔", 79, 16, 24);
        Label2 = Lbl("秒", 149, 16, 12);
        CheckBoxAutoClear = Chk("自动清理", 8, 14, 73, 0);
        CheckBoxAutoClear.Click += (s, e) => CheckBoxAutoClearClick(s);
        SpinEditAutoClearTime = Spin(106, 12, 43, 1000000, 1, 1, 1);
        SpinEditAutoClearTime.ValueChanged += (s, e) => SpinEditAutoClearTimeChange(s);
        GroupBox2.Controls.Add(Label1);
        GroupBox2.Controls.Add(Label2);
        GroupBox2.Controls.Add(CheckBoxAutoClear);
        GroupBox2.Controls.Add(SpinEditAutoClearTime);

        ButtonRestoreBasic = new System.Windows.Forms.Button { Text = "恢复默认值(&D)", Left = 6, Top = 201, Width = 91, Height = 18, TabIndex = 2 };
        ButtonRestoreBasic.Click += (s, e) => ButtonRestoreBasicClick(s);

        GroupBox8 = new System.Windows.Forms.GroupBox { Text = "开启验证码", Left = 179, Top = 40, Width = 210, Height = 101, TabIndex = 3 };
        Label11 = Lbl("错误次数:", 7, 78, 54);
        Label16 = Lbl("更换次数:", 108, 78, 54);
        lbl2 = Lbl("登录扭曲:", 7, 55, 54);
        Label17 = Lbl("其他扭曲:", 108, 55, 54);
        chkRandomCodeLogin = Chk("登录验证码", 8, 16, 84, 0);
        chkRandomCodeLogin.Click += (s, e) => chkRandomCodeLoginClick(s);
        EditRandomCodeErrorMaxCount = Spin(63, 74, 41, 10, 1, 3, 1);
        EditRandomCodeErrorMaxCount.ValueChanged += (s, e) => EditRandomCodeErrorMaxCountChange(s);
        seRandomCodeRefreshMaxCount = Spin(164, 74, 41, 10, 1, 3, 2);
        seRandomCodeRefreshMaxCount.ValueChanged += (s, e) => seRandomCodeRefreshMaxCountChange(s);
        chkRandomCodePwdGetback = Chk("密码找回", 8, 34, 73, 3);
        chkRandomCodePwdGetback.Click += (s, e) => chkRandomCodePwdGetbackClick(s);
        chkRandomCodePwdChange = Chk("密码修改", 120, 34, 73, 4);
        chkRandomCodePwdChange.Click += (s, e) => chkRandomCodePwdChangeClick(s);
        chkRandomCodeReg = Chk("注册验证码", 120, 16, 84, 5);
        chkRandomCodeReg.Click += (s, e) => chkRandomCodeRegClick(s);
        seLoginWaveValue = Spin(63, 51, 41, 10, 1, 1, 6);
        seLoginWaveValue.ValueChanged += (s, e) => seLoginWaveValueChange(s);
        seOtherWaveValue = Spin(164, 51, 41, 6, 1, 1, 7);
        seOtherWaveValue.ValueChanged += (s, e) => seOtherWaveValueChange(s);
        GroupBox8.Controls.Add(Label11);
        GroupBox8.Controls.Add(Label16);
        GroupBox8.Controls.Add(lbl2);
        GroupBox8.Controls.Add(Label17);
        GroupBox8.Controls.Add(chkRandomCodeLogin);
        GroupBox8.Controls.Add(EditRandomCodeErrorMaxCount);
        GroupBox8.Controls.Add(seRandomCodeRefreshMaxCount);
        GroupBox8.Controls.Add(chkRandomCodePwdGetback);
        GroupBox8.Controls.Add(chkRandomCodePwdChange);
        GroupBox8.Controls.Add(chkRandomCodeReg);
        GroupBox8.Controls.Add(seLoginWaveValue);
        GroupBox8.Controls.Add(seOtherWaveValue);

        grpL2Password = new System.Windows.Forms.GroupBox { Text = "二级密码校验选项", Left = 179, Top = 144, Width = 211, Height = 73, TabIndex = 4 };
        chkChangedMACCheckL2 = Chk("机器码改变", 8, 33, 81, 0);
        chkChangedMACCheckL2.Click += (s, e) => chkChangedMACCheckL2Click(s);
        chkChangedIPCheckL2 = Chk("IP改变", 112, 33, 56, 1);
        chkChangedIPCheckL2.Click += (s, e) => chkChangedIPCheckL2Click(s);
        chkAlwaysCheckL2 = Chk("每次进入都需要效验", 8, 51, 137, 2);
        chkAlwaysCheckL2.Click += (s, e) => chkAlwaysCheckL2Click(s);
        chkEnabledL2Password = Chk("开户二级密码功能", 8, 16, 139, 3);
        chkEnabledL2Password.Click += (s, e) => chkEnabledL2PasswordClick(s);
        grpL2Password.Controls.Add(chkChangedMACCheckL2);
        grpL2Password.Controls.Add(chkChangedIPCheckL2);
        grpL2Password.Controls.Add(chkAlwaysCheckL2);
        grpL2Password.Controls.Add(chkEnabledL2Password);

        GroupBox7 = new System.Windows.Forms.GroupBox { Text = "自动解除锁定账号", Left = 179, Top = 3, Width = 210, Height = 37, TabIndex = 5 };
        Label9 = Lbl("等待时间", 59, 17, 48);
        Label10 = Lbl("分", 167, 17, 12);
        CheckBoxAutoUnLockAccount = Chk("开启", 8, 15, 49, 0);
        CheckBoxAutoUnLockAccount.Click += (s, e) => CheckBoxAutoUnLockAccountClick(s);
        SpinEditUnLockAccountTime = Spin(110, 13, 55, 1000000, 1, 1, 1);
        SpinEditUnLockAccountTime.ValueChanged += (s, e) => SpinEditUnLockAccountTimeChange(s);
        GroupBox7.Controls.Add(Label9);
        GroupBox7.Controls.Add(Label10);
        GroupBox7.Controls.Add(CheckBoxAutoUnLockAccount);
        GroupBox7.Controls.Add(SpinEditUnLockAccountTime);

        GroupBox6 = new System.Windows.Forms.GroupBox { Text = "新版登陆", Left = 8, Top = 224, Width = 169, Height = 73, TabIndex = 6 };
        chkNewLoginDlg = Chk("启用新版登陆", 8, 16, 97, 0);
        chkNewLoginDlg.Click += (s, e) => chkNewLoginDlgClick(s);
        chkNewLoginInto = Chk("创建账户直接进入游戏", 8, 32, 145, 1);
        chkNewLoginInto.Click += (s, e) => chkNewLoginIntoClick(s);
        chkNewLoginPhone = Chk("提示绑定手机号", 8, 48, 105, 2);
        chkNewLoginPhone.Click += (s, e) => chkNewLoginPhoneClick(s);
        GroupBox6.Controls.Add(chkNewLoginDlg);
        GroupBox6.Controls.Add(chkNewLoginInto);
        GroupBox6.Controls.Add(chkNewLoginPhone);

        TabSheet1.Controls.Add(GroupBox1);
        TabSheet1.Controls.Add(GroupBox2);
        TabSheet1.Controls.Add(ButtonRestoreBasic);
        TabSheet1.Controls.Add(GroupBox8);
        TabSheet1.Controls.Add(grpL2Password);
        TabSheet1.Controls.Add(GroupBox7);
        TabSheet1.Controls.Add(GroupBox6);

        // ---------------- TabSheet2 网络设置 ----------------
        ButtonRestoreNet = new System.Windows.Forms.Button { Text = "默认(&D)", Left = 318, Top = 159, Width = 67, Height = 25, TabIndex = 0 };
        ButtonRestoreNet.Click += (s, e) => ButtonRestoreNetClick(s);

        GroupBox3 = new System.Windows.Forms.GroupBox { Text = "网关设置", Left = 8, Top = 4, Width = 185, Height = 65, TabIndex = 1 };
        Label3 = Lbl("绑定地址:", 8, 18, 54);
        Label4 = Lbl("网关端口:", 8, 41, 54);
        EditGateAddr = new System.Windows.Forms.TextBox { Left = 72, Top = 14, Width = 105, Height = 20, TabIndex = 0 };
        EditGateAddr.TextChanged += (s, e) => EditGateAddrChange(s);
        EditGatePort = new System.Windows.Forms.TextBox { Left = 72, Top = 37, Width = 57, Height = 20, TabIndex = 1 };
        EditGatePort.TextChanged += (s, e) => EditGatePortChange(s);
        GroupBox3.Controls.Add(Label3);
        GroupBox3.Controls.Add(Label4);
        GroupBox3.Controls.Add(EditGateAddr);
        GroupBox3.Controls.Add(EditGatePort);

        GroupBox4 = new System.Windows.Forms.GroupBox { Text = "远程监控设置", Left = 200, Top = 4, Width = 185, Height = 65, TabIndex = 2 };
        Label5 = Lbl("绑定地址:", 8, 18, 54);
        Label6 = Lbl("网关端口:", 8, 41, 54);
        EditMonAddr = new System.Windows.Forms.TextBox { Left = 72, Top = 14, Width = 105, Height = 20, TabIndex = 0 };
        EditMonAddr.TextChanged += (s, e) => EditMonAddrChange(s);
        EditMonPort = new System.Windows.Forms.TextBox { Left = 72, Top = 37, Width = 57, Height = 20, TabIndex = 1 };
        EditMonPort.TextChanged += (s, e) => EditMonPortChange(s);
        GroupBox4.Controls.Add(Label5);
        GroupBox4.Controls.Add(Label6);
        GroupBox4.Controls.Add(EditMonAddr);
        GroupBox4.Controls.Add(EditMonPort);

        GroupBox5 = new System.Windows.Forms.GroupBox { Text = "服务器网络设置", Left = 8, Top = 75, Width = 185, Height = 65, TabIndex = 3 };
        Label7 = Lbl("绑定地址:", 8, 18, 54);
        Label8 = Lbl("使用端口:", 8, 41, 54);
        EditServerAddr = new System.Windows.Forms.TextBox { Left = 72, Top = 14, Width = 105, Height = 20, TabIndex = 0 };
        EditServerAddr.TextChanged += (s, e) => EditServerAddrChange(s);
        EditServerPort = new System.Windows.Forms.TextBox { Left = 72, Top = 37, Width = 57, Height = 20, TabIndex = 1 };
        EditServerPort.TextChanged += (s, e) => EditServerPortChange(s);
        GroupBox5.Controls.Add(Label7);
        GroupBox5.Controls.Add(Label8);
        GroupBox5.Controls.Add(EditServerAddr);
        GroupBox5.Controls.Add(EditServerPort);

        CheckBoxDynamicIPMode = Chk("动态域名模式", 201, 78, 97, 4);
        CheckBoxDynamicIPMode.Click += (s, e) => CheckBoxDynamicIPModeClick(s);
        chkShowBlockIPLog = Chk("显示非法请求内部端口日志", 8, 198, 169, 5);
        chkShowBlockIPLog.Click += (s, e) => chkShowBlockIPLogClick(s);

        TabSheet2.Controls.Add(ButtonRestoreNet);
        TabSheet2.Controls.Add(GroupBox3);
        TabSheet2.Controls.Add(GroupBox4);
        TabSheet2.Controls.Add(GroupBox5);
        TabSheet2.Controls.Add(CheckBoxDynamicIPMode);
        TabSheet2.Controls.Add(chkShowBlockIPLog);

        // ---------------- TabSheet3 远程管理设置 ----------------
        lbl1 = Lbl("端口为'0'关闭远程管理功能", 208, 79, 138);
        Label14 = Lbl("无连接'IP'限制时，可以任意'IP'连接", 208, 98, 180);
        GroupBox9 = new System.Windows.Forms.GroupBox { Text = "远程管理设置", Left = 210, Top = 7, Width = 177, Height = 65, TabIndex = 0 };
        Label12 = Lbl("管理端口:", 8, 18, 54);
        Label13 = Lbl("管理密码:", 8, 42, 54);
        edtControlPort = new System.Windows.Forms.TextBox { Left = 64, Top = 14, Width = 57, Height = 20, TabIndex = 0 };
        edtControlPort.TextChanged += (s, e) => edtControlPortChange(s);
        edtControlPassword = new System.Windows.Forms.TextBox { Left = 64, Top = 38, Width = 105, Height = 20, TabIndex = 1 };
        edtControlPassword.TextChanged += (s, e) => edtControlPasswordChange(s);
        GroupBox9.Controls.Add(Label12);
        GroupBox9.Controls.Add(Label13);
        GroupBox9.Controls.Add(edtControlPort);
        GroupBox9.Controls.Add(edtControlPassword);

        grp1 = new System.Windows.Forms.GroupBox { Text = "允许连接'IP'", Left = 6, Top = 3, Width = 195, Height = 214, TabIndex = 1 };
        lstControlIPList = new System.Windows.Forms.ListBox { Left = 8, Top = 16, Width = 177, Height = 191, TabIndex = 0, IntegralHeight = false };
        mniIPAdd = new System.Windows.Forms.ToolStripMenuItem("增加(&A)");
        mniIPAdd.Click += (s, e) => mniIPAddClick(s);
        mniIPDelete = new System.Windows.Forms.ToolStripMenuItem("删除(&D)");
        mniIPDelete.Click += (s, e) => mniIPDeleteClick(s);
        mniIPClear = new System.Windows.Forms.ToolStripMenuItem("清空(&C)");
        mniIPClear.Click += (s, e) => mniIPClearClick(s);
        pmControlIPList = new System.Windows.Forms.ContextMenuStrip();
        pmControlIPList.Items.Add(mniIPAdd);
        pmControlIPList.Items.Add(mniIPDelete);
        pmControlIPList.Items.Add(mniIPClear);
        lstControlIPList.ContextMenuStrip = pmControlIPList;
        grp1.Controls.Add(lstControlIPList);

        TabSheet3.Controls.Add(lbl1);
        TabSheet3.Controls.Add(Label14);
        TabSheet3.Controls.Add(GroupBox9);
        TabSheet3.Controls.Add(grp1);

        // ---------------- ts1 密码扩展设置 ----------------
        Label15 = Lbl("密码禁止包含以下字符('一行一个')", 8, 73, 180);
        chkDisablePwdSameChr = Chk("禁止密码为相同数字或字母", 8, 8, 169, 0);
        chkDisablePwdSameChr.Click += (s, e) => chkDisablePwdSameChrClick(s);
        chkDisablePwdAllNum = Chk("禁止密码全部是数字", 8, 29, 153, 1);
        chkDisablePwdAllNum.Click += (s, e) => chkDisablePwdAllNumClick(s);
        mmoDisablePassword = new System.Windows.Forms.TextBox
        {
            Left = 8,
            Top = 90,
            Width = 233,
            Height = 124,
            Multiline = true,
            ScrollBars = System.Windows.Forms.ScrollBars.Both,
            WordWrap = false,
            TabIndex = 2,
        };
        mmoDisablePassword.TextChanged += (s, e) => mmoDisablePasswordChange(s);
        chkDisablePwdAllLetter = Chk("禁止密码全部是字母", 8, 50, 153, 3);
        chkDisablePwdAllLetter.Click += (s, e) => chkDisablePwdAllLetterClick(s);
        ts1.Controls.Add(Label15);
        ts1.Controls.Add(chkDisablePwdSameChr);
        ts1.Controls.Add(chkDisablePwdAllNum);
        ts1.Controls.Add(mmoDisablePassword);
        ts1.Controls.Add(chkDisablePwdAllLetter);

        // ---------------- 按钮区（DFM: ButtonSave(248,341,75,25) ButtonClose(334,341,75,25)）----------------
        // DFM: Caption = '保存(&S)'
        ButtonSave = new System.Windows.Forms.Button { Text = "保存(&S)", Left = 248, Top = 341, Width = 75, Height = 25, TabIndex = 1, Enabled = false };
        ButtonSave.Click += (s, e) => ButtonSaveClick(s);
        // DFM: Caption = '确定(&O)'
        ButtonClose = new System.Windows.Forms.Button { Text = "确定(&O)", Left = 334, Top = 341, Width = 75, Height = 25, TabIndex = 2 };
        ButtonClose.Click += (s, e) => ButtonCloseClick(s);

        Controls.Add(PageControl1);
        Controls.Add(ButtonSave);
        Controls.Add(ButtonClose);
    }

    // ================= Delphi 1:1 私有辅助 =================

    /// <summary>BasicSet.pas:350 LockSaveButtonEnabled。</summary>
    public void LockSaveButtonEnabled()
    {
        ButtonSave.Enabled = false;
    }

    /// <summary>BasicSet.pas:355 UnLockSaveButtonEnabled。</summary>
    public void UnLockSaveButtonEnabled()
    {
        ButtonSave.Enabled = true;
    }

    /// <summary>BasicSet.pas:776 ErrMessage。</summary>
    private void ErrMessage(string MsgStr)
    {
        LoginSrvForms.MessageBox(MsgStr, "错误", LoginSrvForms.MB_OK | LoginSrvForms.MB_ICONERROR);
    }

    /// <summary>BasicSet.pas:781 FormCreate（PageControl1.ActivePageIndex := 0）。</summary>
    public void FormCreate(object? Sender)
    {
        PageControl1.SelectedIndex = 0;
    }

    /// <summary>BasicSet.pas:360 OpenBasicSet（装载 g_Config → 控件；showModal=false 供测试/宿主复用）。</summary>
    public bool OpenBasicSet(bool showModal = true)
    {
        CheckBoxTestServer.Checked = LoginSrvShare.g_Config.boTestServer;
        CheckBoxEnableMakingID.Checked = LoginSrvShare.g_Config.boEnableMakingID;
        CheckBoxEnableGetbackPassword.Checked = LoginSrvShare.g_Config.boEnableGetbackPassword;
        CheckBoxGetbackPasswordCheckAll.Checked = LoginSrvShare.g_Config.boGetbackPasswordCheckAll;
        chkDisableIDSamePassword.Checked = LoginSrvShare.g_Config.boDisableIDSamePassword;

        chkDisableQuizSameAnswer.Checked = LoginSrvShare.g_Config.boDisableQuizSameAnswer;

        CheckBoxAutoClear.Checked = LoginSrvShare.g_Config.boAutoClearID;
        SetSpin(SpinEditAutoClearTime, LoginSrvShare.g_Config.dwAutoClearTime);

        CheckBoxAutoUnLockAccount.Checked = LoginSrvShare.g_Config.boUnLockAccount;
        SetSpin(SpinEditUnLockAccountTime, LoginSrvShare.g_Config.dwUnLockAccountTime);

        EditGateAddr.Text = LoginSrvShare.g_Config.sGateAddr;
        EditGatePort.Text = DelphiRTL.IntToStr(LoginSrvShare.g_Config.nGatePort);

        EditServerAddr.Text = LoginSrvShare.g_Config.sServerAddr;
        EditServerPort.Text = DelphiRTL.IntToStr(LoginSrvShare.g_Config.nServerPort);

        EditMonAddr.Text = LoginSrvShare.g_Config.sMonAddr;
        EditMonPort.Text = DelphiRTL.IntToStr(LoginSrvShare.g_Config.nMonPort);

        edtControlPort.Text = DelphiRTL.IntToStr(LoginSrvShare.g_Config.nControlPort);
        edtControlPassword.Text = LoginSrvShare.g_Config.sControlPassword;

        CheckBoxDynamicIPMode.Checked = LoginSrvShare.g_Config.boDynamicIPMode;
        //CheckBoxMinimize.Checked := Config.boMinimize;      // 原文如此（BasicSet.pas:391 注释）

        chkShowBlockIPLog.Checked = LoginSrvShare.g_Config.boShowBlockIPLog;

        chkRandomCodeLogin.Checked = LoginSrvShare.g_Config.boRandomCode[(int)TRandCodeType.rctLogin];
        chkRandomCodeReg.Checked = LoginSrvShare.g_Config.boRandomCode[(int)TRandCodeType.rctRegister];
        chkRandomCodePwdGetback.Checked = LoginSrvShare.g_Config.boRandomCode[(int)TRandCodeType.rctPwdGetback];
        chkRandomCodePwdChange.Checked = LoginSrvShare.g_Config.boRandomCode[(int)TRandCodeType.rctPwdChange];

        SetSpin(seLoginWaveValue, LoginSrvShare.g_Config.btLoginWaveValue);
        SetSpin(seOtherWaveValue, LoginSrvShare.g_Config.btOtherWaveValue);

        SetSpin(EditRandomCodeErrorMaxCount, LoginSrvShare.g_Config.nRandomCodeErrorMaxCount);
        SetSpin(seRandomCodeRefreshMaxCount, LoginSrvShare.g_Config.nRandomCodeRefreshMaxCount);

        chkEnabledL2Password.Checked = LoginSrvShare.g_Config.boEnabledL2Password;
        chkChangedMACCheckL2.Checked = LoginSrvShare.g_Config.boChangedMACCheckL2;
        chkChangedIPCheckL2.Checked = LoginSrvShare.g_Config.boChangedIPCheckL2;
        chkAlwaysCheckL2.Checked = LoginSrvShare.g_Config.boAlwaysCheckL2;   // 原文小写 g_config（BasicSet.pas:409）

        chkDisableIDSameL2Password.Checked = LoginSrvShare.g_Config.boDisableIDSameL2Password;
        chkDisableL2SamePassword.Checked = LoginSrvShare.g_Config.boDisableL2SamePassword;

        chkDisablePwdSameChr.Checked = LoginSrvShare.g_Config.boDisablePwdSameChr;
        chkDisablePwdAllNum.Checked = LoginSrvShare.g_Config.boDisablePwdAllNum;
        chkDisablePwdAllLetter.Checked = LoginSrvShare.g_Config.boDisablePwdAllLetter;
        mmoDisablePassword.Text = DelphiStrings.GetText(LoginSrvShare.g_DisablePasswordList);

        chkNewLoginDlg.Checked = LoginSrvShare.g_Config.boNewLoginDlg;
        chkNewLoginInto.Checked = LoginSrvShare.g_Config.boNewLoginInto;
        chkNewLoginPhone.Checked = LoginSrvShare.g_Config.boNewLoginPhone;
        //  chkNewLoginMustHasPhone.Checked := g_Config.boNewLoginMustHasPhone;   // 原文如此（BasicSet.pas:422 注释）

        LockSaveButtonEnabled();

        LoginSrvShare.g_ControlIPList.Lock();
        try
        {
            for (int I = 0; I <= LoginSrvShare.g_ControlIPList.Count - 1; I++)
            {
                lstControlIPList.Items.Add(LoginSrvShare.g_ControlIPList[I]);
            }
        }
        finally
        {
            LoginSrvShare.g_ControlIPList.UnLock();
        }
        if (showModal)
            ShowDialog();
        return true;
    }

    // ================= 各事件处理器（BasicSet.pas 1:1） =================

    public void CheckBoxTestServerClick(object? Sender)
    {
        LoginSrvShare.g_Config.boTestServer = CheckBoxTestServer.Checked;
        UnLockSaveButtonEnabled();
    }

    public void CheckBoxEnableMakingIDClick(object? Sender)
    {
        LoginSrvShare.g_Config.boEnableMakingID = CheckBoxEnableMakingID.Checked;
        UnLockSaveButtonEnabled();
    }

    public void CheckBoxEnableGetbackPasswordClick(object? Sender)
    {
        LoginSrvShare.g_Config.boEnableGetbackPassword = CheckBoxEnableGetbackPassword.Checked;
        UnLockSaveButtonEnabled();
    }

    public void CheckBoxGetbackPasswordCheckAllClick(object? Sender)
    {
        LoginSrvShare.g_Config.boGetbackPasswordCheckAll = CheckBoxGetbackPasswordCheckAll.Checked;
        UnLockSaveButtonEnabled();
    }

    public void chkDisableIDSamePasswordClick(object? Sender)
    {
        LoginSrvShare.g_Config.boDisableIDSamePassword = chkDisableIDSamePassword.Checked;
        UnLockSaveButtonEnabled();
    }

    public void CheckBoxAutoClearClick(object? Sender)
    {
        LoginSrvShare.g_Config.boAutoClearID = CheckBoxAutoClear.Checked;
        UnLockSaveButtonEnabled();
    }

    public void SpinEditAutoClearTimeChange(object? Sender)
    {
        LoginSrvShare.g_Config.dwAutoClearTime = (int)SpinEditAutoClearTime.Value;
        UnLockSaveButtonEnabled();
    }

    public void CheckBoxAutoUnLockAccountClick(object? Sender)
    {
        LoginSrvShare.g_Config.boUnLockAccount = CheckBoxAutoUnLockAccount.Checked;
        UnLockSaveButtonEnabled();
    }

    public void SpinEditUnLockAccountTimeChange(object? Sender)
    {
        LoginSrvShare.g_Config.dwUnLockAccountTime = (int)SpinEditUnLockAccountTime.Value;
        UnLockSaveButtonEnabled();
    }

    /// <summary>BasicSet.pas:494 ButtonRestoreBasicClick（★ 原文不改 g_Config、也不解开保存按钮）。</summary>
    public void ButtonRestoreBasicClick(object? Sender)
    {
        CheckBoxTestServer.Checked = true;
        CheckBoxEnableMakingID.Checked = true;
        CheckBoxEnableGetbackPassword.Checked = true;
        CheckBoxAutoClear.Checked = true;
        SpinEditAutoClearTime.Value = 1;
        CheckBoxAutoUnLockAccount.Checked = false;
        SpinEditUnLockAccountTime.Value = 10;

        chkRandomCodeLogin.Checked = false;
        chkRandomCodeReg.Checked = false;
        chkRandomCodePwdGetback.Checked = false;
        chkRandomCodePwdChange.Checked = false;

        EditRandomCodeErrorMaxCount.Value = 3;
    }

    public void EditGateAddrChange(object? Sender)
    {
        LoginSrvShare.g_Config.sGateAddr = DelphiRTL.Trim(EditGateAddr.Text);
        UnLockSaveButtonEnabled();
    }

    public void EditGatePortChange(object? Sender)
    {
        LoginSrvShare.g_Config.nGatePort = HUtil32.Str_ToInt(DelphiRTL.Trim(EditGatePort.Text), 5500);
        UnLockSaveButtonEnabled();
    }

    public void EditMonAddrChange(object? Sender)
    {
        LoginSrvShare.g_Config.sMonAddr = DelphiRTL.Trim(EditMonAddr.Text);
        UnLockSaveButtonEnabled();
    }

    public void EditMonPortChange(object? Sender)
    {
        LoginSrvShare.g_Config.nMonPort = HUtil32.Str_ToInt(DelphiRTL.Trim(EditMonPort.Text), 3000);
        UnLockSaveButtonEnabled();
    }

    public void EditServerAddrChange(object? Sender)
    {
        LoginSrvShare.g_Config.sServerAddr = DelphiRTL.Trim(EditServerAddr.Text);
        UnLockSaveButtonEnabled();
    }

    /// <summary>★ 原文此处用的是 SysUtils.StrToIntDef，而 EditGatePort/EditMonPort 用 HUtil32.Str_ToInt（差异保留）。</summary>
    public void EditServerPortChange(object? Sender)
    {
        LoginSrvShare.g_Config.nServerPort = DelphiRTL.StrToIntDef(DelphiRTL.Trim(EditServerPort.Text), 5600);
        UnLockSaveButtonEnabled();
    }

    public void CheckBoxDynamicIPModeClick(object? Sender)
    {
        LoginSrvShare.g_Config.boDynamicIPMode = CheckBoxDynamicIPMode.Checked;
        UnLockSaveButtonEnabled();
    }

    /// <summary>BasicSet.pas:554 ButtonRestoreNetClick（★ 原文不改 g_Config、也不解开保存按钮）。</summary>
    public void ButtonRestoreNetClick(object? Sender)
    {
        EditGateAddr.Text = "0.0.0.0";
        EditGatePort.Text = "5500";
        EditServerAddr.Text = "0.0.0.0";
        EditServerPort.Text = "5600";
        EditMonAddr.Text = "0.0.0.0";
        EditMonPort.Text = "3000";
        CheckBoxDynamicIPMode.Checked = false;
    }

    /// <summary>
    /// BasicSet.pas:565 WriteConfig（嵌套过程 WriteConfigString/Integer/Boolean 1:1 展开）。
    /// 键序与原文完全一致；布尔写 '-1'/'0'（Delphi TIniFile.WriteBool → SysUtils.BoolToStr）。
    /// </summary>
    public static void WriteConfig(TConfig Config)
    {
        TFastIniFile ini = Config.IniConf!;

        void WriteConfigString(string sSection, string sIdent, string sDefault)
        {
            ini.WriteString(sSection, sIdent, sDefault);
        }
        void WriteConfigInteger(string sSection, string sIdent, int nDefault)
        {
            ini.WriteInteger(sSection, sIdent, nDefault);
        }
        void WriteConfigBoolean(string sSection, string sIdent, bool boDefault)
        {
            // Delphi TIniFile.WriteBool → SysUtils.BoolToStr：True='-1'，False='0'
            ini.WriteString(sSection, sIdent, boDefault ? "-1" : "0");
        }

        WriteConfigString(sSectionServer, sIdentDBServer, Config.sDBServer);
        WriteConfigString(sSectionServer, sIdentFeeServer, Config.sFeeServer);
        WriteConfigString(sSectionServer, sIdentLogServer, Config.sLogServer);

        WriteConfigString(sSectionServer, sIdentGateAddr, Config.sGateAddr);
        WriteConfigInteger(sSectionServer, sIdentGatePort, Config.nGatePort);
        WriteConfigString(sSectionServer, sIdentServerAddr, Config.sServerAddr);
        WriteConfigInteger(sSectionServer, sIdentServerPort, Config.nServerPort);
        WriteConfigString(sSectionServer, sIdentMonAddr, Config.sMonAddr);
        WriteConfigInteger(sSectionServer, sIdentMonPort, Config.nMonPort);

        WriteConfigString(sSectionServer, sIdentControlPassword, Config.sControlPassword);
        WriteConfigInteger(sSectionServer, sIdentControlPort, Config.nControlPort);

        WriteConfigBoolean(sSectionServer, sIdentShowBlockIPLog, LoginSrvShare.g_Config.boShowBlockIPLog);

        WriteConfigInteger(sSectionServer, sIdentDBSPort, Config.nDBSPort);
        WriteConfigInteger(sSectionServer, sIdentFeePort, Config.nFeePort);
        WriteConfigInteger(sSectionServer, sIdentLogPort, Config.nLogPort);
        WriteConfigInteger(sSectionServer, sIdentReadyServers, Config.nReadyServers);
        WriteConfigBoolean(sSectionServer, sIdentEnableMakingID, Config.boEnableMakingID);
        WriteConfigBoolean(sSectionServer, sIdentTestServer, Config.boTestServer);

        WriteConfigBoolean(sSectionServer, sIdentEnableGetbackPassword, Config.boEnableGetbackPassword);
        WriteConfigBoolean(sSectionServer, sIdentGetbackPasswordCheckAll, Config.boGetbackPasswordCheckAll);
        WriteConfigBoolean(sSectionServer, sIdentDisableIDSamePassword, Config.boDisableIDSamePassword);
        WriteConfigBoolean(sSectionServer, sIdentDisableQuizSameAnswer, Config.boDisableQuizSameAnswer);

        WriteConfigBoolean(sSectionServer, sIdentDisableIDSameL2Password, Config.boDisableIDSameL2Password);
        WriteConfigBoolean(sSectionServer, sIdentDisableL2SamePassword, Config.boDisableL2SamePassword);

        WriteConfigBoolean(sSectionServer, sIdentDisablePwdSameChr, Config.boDisablePwdSameChr);
        WriteConfigBoolean(sSectionServer, sIdentDisablePwdAllNum, Config.boDisablePwdAllNum);
        WriteConfigBoolean(sSectionServer, sIdentDisablePwdAllLetter, Config.boDisablePwdAllLetter);

        WriteConfigBoolean(sSectionServer, sIdentAutoClearID, Config.boAutoClearID);
        WriteConfigInteger(sSectionServer, sIdentAutoClearTime, Config.dwAutoClearTime);
        WriteConfigBoolean(sSectionServer, sIdentUnLockAccount, Config.boUnLockAccount);
        WriteConfigInteger(sSectionServer, sIdentUnLockAccountTime, Config.dwUnLockAccountTime);

        WriteConfigBoolean(sSectionServer, sIdentDynamicIPMode, Config.boDynamicIPMode);
        // WriteConfigBoolean(sSectionServer, sIdentMinimize, Config.boMinimize);   // 原文如此（BasicSet.pas:622 注释）

        for (TRandCodeType RandCodeType = TRandCodeType.rctLogin; RandCodeType <= TRandCodeType.rctPwdChange; RandCodeType++)
        {
            WriteConfigBoolean(sSectionServer, sIdentRandomCode[(int)RandCodeType], Config.boRandomCode[(int)RandCodeType]);
        }

        WriteConfigInteger(sSectionServer, sIdentLoginWaveValue, LoginSrvShare.g_Config.btLoginWaveValue);
        WriteConfigInteger(sSectionServer, sIdentOtherWaveValue, LoginSrvShare.g_Config.btOtherWaveValue);

        WriteConfigInteger(sSectionServer, sIdentRandomCodeErrorMaxCount, Config.nRandomCodeErrorMaxCount);
        WriteConfigInteger(sSectionServer, sIdentRandomCodeRefreshMaxCount, Config.nRandomCodeRefreshMaxCount);

        WriteConfigString(sSectionDB, sIdentIdDir, Config.sIdDir);
        WriteConfigString(sSectionDB, sIdentWebLogDir, Config.sWebLogDir);
        WriteConfigString(sSectionDB, sIdentCountLogDir, Config.sCountLogDir);
        WriteConfigString(sSectionDB, sIdentFeedIDList, Config.sFeedIDList);
        WriteConfigString(sSectionDB, sIdentFeedIPList, Config.sFeedIPList);

        WriteConfigBoolean(sSectionServer, sIdentEnabledL2Password, Config.boEnabledL2Password);
        WriteConfigBoolean(sSectionServer, sIdentChangedMACCheckL2, Config.boChangedMACCheckL2);
        WriteConfigBoolean(sSectionServer, sIdentChangedIPCheckL2, Config.boChangedIPCheckL2);
        WriteConfigBoolean(sSectionServer, sIdentAlwaysCheckL2, Config.boAlwaysCheckL2);

        WriteConfigBoolean(sSectionServer, sIdentNewLoginDlg, Config.boNewLoginDlg);
        WriteConfigBoolean(sSectionServer, sIdentNewLoginInto, Config.boNewLoginInto);
        WriteConfigBoolean(sSectionServer, sIdentNewLoginPhone, Config.boNewLoginPhone);
        WriteConfigBoolean(sSectionServer, sIdentNewLoginMustHasPhone, Config.boNewLoginMustHasPhone);

        // Delphi TIniFile.WriteString 每次调用即落盘；GXX.Core.TFastIniFile 为内存缓存 → 末尾统一落盘
        ini.UpdateFile();
    }

    /// <summary>BasicSet.pas:652 ButtonSaveClick。</summary>
    public void ButtonSaveClick(object? Sender)
    {
        WriteConfig(LoginSrvShare.g_Config);

        DelphiStrings.SetText(LoginSrvShare.g_DisablePasswordList, mmoDisablePassword.Text);
        LoginSrvShare.g_DisablePasswordList.SaveToFile(LoginSrvShare.g_DisablePasswordFile);

        LockSaveButtonEnabled();
    }

    /// <summary>BasicSet.pas:662 ButtonCloseClick。</summary>
    public void ButtonCloseClick(object? Sender)
    {
        Close();
    }

    public void chkRandomCodeLoginClick(object? Sender)
    {
        LoginSrvShare.g_Config.boRandomCode[(int)TRandCodeType.rctLogin] = chkRandomCodeLogin.Checked;
        UnLockSaveButtonEnabled();
    }

    public void seRandomCodeRefreshMaxCountChange(object? Sender)
    {
        LoginSrvShare.g_Config.nRandomCodeRefreshMaxCount = (int)seRandomCodeRefreshMaxCount.Value;
        UnLockSaveButtonEnabled();
    }

    public void chkRandomCodeRegClick(object? Sender)
    {
        LoginSrvShare.g_Config.boRandomCode[(int)TRandCodeType.rctRegister] = chkRandomCodeReg.Checked;
        UnLockSaveButtonEnabled();
    }

    public void chkRandomCodePwdGetbackClick(object? Sender)
    {
        LoginSrvShare.g_Config.boRandomCode[(int)TRandCodeType.rctPwdGetback] = chkRandomCodePwdGetback.Checked;
        UnLockSaveButtonEnabled();
    }

    public void chkRandomCodePwdChangeClick(object? Sender)
    {
        LoginSrvShare.g_Config.boRandomCode[(int)TRandCodeType.rctPwdChange] = chkRandomCodePwdChange.Checked;
        UnLockSaveButtonEnabled();
    }

    public void EditRandomCodeErrorMaxCountChange(object? Sender)
    {
        LoginSrvShare.g_Config.nRandomCodeErrorMaxCount = (int)EditRandomCodeErrorMaxCount.Value;
        UnLockSaveButtonEnabled();
    }

    public void chkDisableQuizSameAnswerClick(object? Sender)
    {
        LoginSrvShare.g_Config.boDisableQuizSameAnswer = chkDisableQuizSameAnswer.Checked;
        UnLockSaveButtonEnabled();
    }

    public void edtControlPortChange(object? Sender)
    {
        LoginSrvShare.g_Config.nControlPort = HUtil32.Str_ToInt(DelphiRTL.Trim(edtControlPort.Text), 0);
        UnLockSaveButtonEnabled();
    }

    public void edtControlPasswordChange(object? Sender)
    {
        LoginSrvShare.g_Config.sControlPassword = DelphiRTL.Trim(edtControlPassword.Text);
        UnLockSaveButtonEnabled();
    }

    /// <summary>BasicSet.pas:721 mniIPAddClick。</summary>
    public void mniIPAddClick(object? Sender)
    {
        string sIPaddress = "";
        if (!LoginSrvInputQuery.InputQueryEx("永久IP过滤", "请输入一个新的IP地址: ", "如：202.103.100.20", ref sIPaddress)) return;
        if (!HUtil32.IsIPaddr(sIPaddress))
        {
            ErrMessage("输入的地址格式错误！");
            return;
        }

        LoginSrvShare.g_ControlIPList.Lock();
        try
        {
            if (LoginSrvShare.g_ControlIPList.IndexOf(sIPaddress) < 0)
            {
                LoginSrvShare.g_ControlIPList.Add(sIPaddress);
                lstControlIPList.Items.Add(sIPaddress);

                LoginSrvShare.g_ControlIPList.SaveToFile(LoginSrvShare.g_ControlIPFile);
            }
        }
        finally
        {
            LoginSrvShare.g_ControlIPList.UnLock();
        }
    }

    /// <summary>BasicSet.pas:747 mniIPDeleteClick（★ 原文删除后不刷新 ItemIndex）。</summary>
    public void mniIPDeleteClick(object? Sender)
    {
        if ((lstControlIPList.SelectedIndex >= 0) && (lstControlIPList.SelectedIndex < lstControlIPList.Items.Count))
        {
            LoginSrvShare.g_ControlIPList.Lock();
            try
            {
                LoginSrvShare.g_ControlIPList.Delete(lstControlIPList.SelectedIndex);
            }
            finally
            {
                LoginSrvShare.g_ControlIPList.UnLock();
            }
            lstControlIPList.Items.RemoveAt(lstControlIPList.SelectedIndex);

            LoginSrvShare.g_ControlIPList.SaveToFile(LoginSrvShare.g_ControlIPFile);
        }
    }

    /// <summary>BasicSet.pas:763 mniIPClearClick。</summary>
    public void mniIPClearClick(object? Sender)
    {
        LoginSrvShare.g_ControlIPList.Lock();
        try
        {
            LoginSrvShare.g_ControlIPList.Clear();
        }
        finally
        {
            LoginSrvShare.g_ControlIPList.UnLock();
        }

        lstControlIPList.Items.Clear();
        LoginSrvShare.g_ControlIPList.SaveToFile(LoginSrvShare.g_ControlIPFile);
    }

    public void chkChangedMACCheckL2Click(object? Sender)
    {
        LoginSrvShare.g_Config.boChangedMACCheckL2 = chkChangedMACCheckL2.Checked;
        UnLockSaveButtonEnabled();
    }

    public void chkChangedIPCheckL2Click(object? Sender)
    {
        LoginSrvShare.g_Config.boChangedIPCheckL2 = chkChangedIPCheckL2.Checked;
        UnLockSaveButtonEnabled();
    }

    public void chkAlwaysCheckL2Click(object? Sender)
    {
        LoginSrvShare.g_Config.boAlwaysCheckL2 = chkAlwaysCheckL2.Checked;
        UnLockSaveButtonEnabled();
    }

    public void chkDisableIDSameL2PasswordClick(object? Sender)
    {
        LoginSrvShare.g_Config.boDisableIDSameL2Password = chkDisableIDSameL2Password.Checked;
        UnLockSaveButtonEnabled();
    }

    public void chkDisableL2SamePasswordClick(object? Sender)
    {
        LoginSrvShare.g_Config.boDisableL2SamePassword = chkDisableL2SamePassword.Checked;
        UnLockSaveButtonEnabled();
    }

    public void chkEnabledL2PasswordClick(object? Sender)
    {
        LoginSrvShare.g_Config.boEnabledL2Password = chkEnabledL2Password.Checked;
        UnLockSaveButtonEnabled();
    }

    public void chkDisablePwdSameChrClick(object? Sender)
    {
        LoginSrvShare.g_Config.boDisablePwdSameChr = chkDisablePwdSameChr.Checked;
        UnLockSaveButtonEnabled();
    }

    public void chkDisablePwdAllNumClick(object? Sender)
    {
        LoginSrvShare.g_Config.boDisablePwdAllNum = chkDisablePwdAllNum.Checked;
        UnLockSaveButtonEnabled();
    }

    public void chkDisablePwdAllLetterClick(object? Sender)
    {
        LoginSrvShare.g_Config.boDisablePwdAllLetter = chkDisablePwdAllLetter.Checked;
        UnLockSaveButtonEnabled();
    }

    /// <summary>BasicSet.pas:840 mmoDisablePasswordChange（只解锁保存按钮，不写配置）。</summary>
    public void mmoDisablePasswordChange(object? Sender)
    {
        UnLockSaveButtonEnabled();
    }

    public void seLoginWaveValueChange(object? Sender)
    {
        LoginSrvShare.g_Config.btLoginWaveValue = (byte)seLoginWaveValue.Value;
        UnLockSaveButtonEnabled();
    }

    public void seOtherWaveValueChange(object? Sender)
    {
        LoginSrvShare.g_Config.btOtherWaveValue = (byte)seOtherWaveValue.Value;
        UnLockSaveButtonEnabled();
    }

    public void chkShowBlockIPLogClick(object? Sender)
    {
        LoginSrvShare.g_Config.boShowBlockIPLog = chkShowBlockIPLog.Checked;
        UnLockSaveButtonEnabled();
    }

    public void chkNewLoginDlgClick(object? Sender)
    {
        LoginSrvShare.g_Config.boNewLoginDlg = chkNewLoginDlg.Checked;
        UnLockSaveButtonEnabled();
    }

    public void chkNewLoginIntoClick(object? Sender)
    {
        LoginSrvShare.g_Config.boNewLoginInto = chkNewLoginInto.Checked;
        UnLockSaveButtonEnabled();
    }

    public void chkNewLoginPhoneClick(object? Sender)
    {
        LoginSrvShare.g_Config.boNewLoginPhone = chkNewLoginPhone.Checked;
        UnLockSaveButtonEnabled();
    }

    /// <summary>BasicSet.pas:881 chkNewLoginMustHasPhoneClick（原文方法体整体被注释，保留空实现）。</summary>
    public void chkNewLoginMustHasPhoneClick(object? Sender)
    {
        //  g_Config.boNewLoginMustHasPhone := chkNewLoginMustHasPhone.Checked;
        //  UnLockSaveButtonEnabled();
    }
}
