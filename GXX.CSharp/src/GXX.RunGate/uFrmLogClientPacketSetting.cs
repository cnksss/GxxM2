using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GXX.Core.Rtl;

namespace GXX.RunGate;

// =====================================================================================
// uFrmLogClientPacketSetting.pas 1:1 转换（Source\RunGate\uFrmLogClientPacketSetting.pas，146 行 / LF 145）。
// 布局真源：Source\RunGate\uFrmLogClientPacketSetting.dfm（**同名 .dfm 存在**）。
//
// 窗体职责：客户端封包记录设置 —— 8 个封包类型勾选（位或成掩码）+ 记录指定人物名单。
//
// ★ 原文缺陷（照抄 + 差异断言）：
//   D1. DFM 里 `chkLogOther` 是 `Checked=True Enabled=False`（**禁用恒真**），
//       而 `btnOKClick`（原 :64-70）**根本不读 chkLogOther**：
//       掩码只由 chkLogMove/Hit/Spell/Query/Team/Guild/Shop 七个位组成。
//       即"其他操作"这一项在 UI 上恒被勾选，但对 g_nLogClientPacketType 没有任何贡献 —— 照抄。
//   D2. 掩码常量 CPT_MOVE/CPT_HIT/... 定义在 GateShare.pas，本移植取 **Grobal2_Ex.pas 的位序**
//       惯例（见 RunGateConst.Cpt*），若后续 GateShare 移植给出不同位值，需以那边为准并更新此表；
//       已在测试中登记该依赖。
//   D3. `btnDelUserClick`（原 :116-122）判 `lstLogUser.Count > 0`（**TListBox.Count**），
//       而 `FormCreate`/`lstLogUserClick` 判同一个表达式；测试断言三处语义一致。
//   D4. `FormCreate`（原 :136）`lstLogUser.Items.Text := g_LogClientPacketUser.Text;`
//       用的是 TStrings.Text 的**按行拆分**语义（不是单行字符串）。
// =====================================================================================

/// <summary>uFrmLogClientPacketSetting.pas 的界面值载体。</summary>
public class LogClientPacketSettingValues
{
    public bool LogMove;        // chkLogMove
    public bool LogHit;         // chkLogHit
    public bool LogSpell;       // chkLogSpell
    public bool LogQuery;       // chkLogQuery
    public bool LogTeam;        // chkLogTeam
    public bool LogGuild;       // chkLogGuild
    public bool LogShop;        // chkLogShop
    public bool LogOther;       // chkLogOther（DFM: Checked=True Enabled=False；**不参与掩码**）
    public bool LogClientPacket; // chkLogClientPacket
    public List<string> LogUsers = new List<string>();   // lstLogUser.Items
}

/// <summary>uFrmLogClientPacketSetting.pas 的非 UI 逻辑。</summary>
public static class LogClientPacketSettingLogic
{
    /// <summary>原 :63-72 `btnOKClick` 的掩码拼接（**7 个位，chkLogOther 不参与**，顺序即原文顺序）。</summary>
    public static int BuildPacketTypeMask(LogClientPacketSettingValues v)
    {
        int mask = 0;                                                       // 原 :63
        if (v.LogMove) mask |= RunGateConst.CptMove;                        // 原 :64
        if (v.LogHit) mask |= RunGateConst.CptHit;                          // 原 :65
        if (v.LogSpell) mask |= RunGateConst.CptSpell;                      // 原 :66
        if (v.LogQuery) mask |= RunGateConst.CptQuery;                      // 原 :67
        if (v.LogTeam) mask |= RunGateConst.CptTeam;                        // 原 :68
        if (v.LogGuild) mask |= RunGateConst.CptGuild;                      // 原 :69
        if (v.LogShop) mask |= RunGateConst.CptShop;                        // 原 :70
        return mask;
    }

    /// <summary>原 :126-132 `FormCreate` 的位拆解（`and &lt;&gt; 0` 逐位判断）。</summary>
    public static LogClientPacketSettingValues Open()
    {
        var v = new LogClientPacketSettingValues();
        int mask = FormGlobals.g_nLogClientPacketType;
        v.LogMove = (mask & RunGateConst.CptMove) != 0;                     // 原 :126
        v.LogHit = (mask & RunGateConst.CptHit) != 0;                       // 原 :127
        v.LogSpell = (mask & RunGateConst.CptSpell) != 0;                   // 原 :128
        v.LogQuery = (mask & RunGateConst.CptQuery) != 0;                   // 原 :129
        v.LogTeam = (mask & RunGateConst.CptTeam) != 0;                     // 原 :130
        v.LogGuild = (mask & RunGateConst.CptGuild) != 0;                   // 原 :131
        v.LogShop = (mask & RunGateConst.CptShop) != 0;                     // 原 :132
        // chkLogOther：DFM Checked=True Enabled=False，代码**从不读写**
        v.LogOther = true;
        v.LogClientPacket = FormGlobals.g_boLogClientPacket;                // 原 :134
        v.LogUsers = new List<string>(FormGlobals.g_LogClientPacketUser.Lines);   // 原 :136 Items.Text := ...Text
        return v;
    }

    /// <summary>
    /// 原 :58-92 `btnOKClick`：
    ///   控件 → 掩码 + bool → `g_LogClientPacketUser.Text := lstLogUser.Items.Text` → SaveToFile
    ///   → INI 写 2 键（**没有 try..finally**，与 uFrmReadFileIP 同）。
    /// </summary>
    public static void ButtonOK(LogClientPacketSettingValues v, string iniFileName, string userFileName)
    {
        FormGlobals.g_nLogClientPacketType = BuildPacketTypeMask(v);        // 原 :72
        FormGlobals.g_boLogClientPacket = v.LogClientPacket;                // 原 :73

        FormGlobals.g_LogClientPacketUser.Lock();                           // 原 :75
        try
        {
            FormGlobals.g_LogClientPacketUser.Text = string.Join("\r\n", v.LogUsers);   // 原 :77
        }
        finally
        {
            FormGlobals.g_LogClientPacketUser.UnLock();                     // 原 :79
        }
        FormGlobals.g_LogClientPacketUser.SaveToFile(userFileName);         // 原 :81

        var ini = new TIniFileEx(iniFileName);                              // 原 :83
        ini.WriteBool(RunGateConst.GateClass, "LogClientPacket", FormGlobals.g_boLogClientPacket ? (byte)1 : (byte)0);   // 原 :85
        ini.WriteInteger(RunGateConst.GateClass, "LogClientPacketType", FormGlobals.g_nLogClientPacketType);             // 原 :86
        ini.Dispose();                                                      // 原 :88 IniFile.Free
    }

    /// <summary>原 :94-114 `btnAddUserClick` 的校验。
    /// 返回 (成功, 失败提示文本)。提示标题统一 '提示'，图标 MB_ICONINFORMATION。</summary>
    public static bool ValidateAddUser(string userNameText, IReadOnlyList<string> existing,
                                       out string errorMessage)
    {
        string userName = DelphiRTL.Trim(userNameText);                     // 原 :98
        if (userName.Length == 0)                                           // 原 :99
        {
            errorMessage = "人物名称不能为空";                               // 原 :101
            return false;
        }
        // 原 :106 `if lstLogUser.Items.IndexOf(UserName) >= 0 then`
        foreach (string s in existing)
        {
            if (s == userName)
            {
                errorMessage = "人物名称已经在列表中存在";                   // 原 :108
                return false;
            }
        }
        errorMessage = "";
        return true;
    }
}

/// <summary>原 :46-56 `procedure ShowFrmLogClientPacketSetting;`。</summary>
public static class LogClientPacketSettingUnit
{
    public static void ShowFrmLogClientPacketSetting()
    {
        using var form = new FrmLogClientPacketSetting();
        form.ShowDialog();                                             // 原 :52（返回值丢弃）
    }
}

/// <summary>原 uFrmLogClientPacketSetting.pas:10-38 `TFrmLogClientPacketSetting`（DFM: uFrmLogClientPacketSetting.dfm）。</summary>
public class FrmLogClientPacketSetting : Form
{
    // DFM: FrmLogClientPacketSetting Left=403 Top=335 BorderStyle=bsDialog Caption='封包记录设置'
    //      ClientHeight=238 ClientWidth=339 Font.Charset=GB2312_CHARSET Font.Height=-12 Font.Name='宋体'
    //      Position=poMainFormCenter OnCreate=FormCreate PixelsPerInch=96
    public GroupBox grpPacketType;   // DFM: grpPacketType Left=8 Top=8 Width=87 Height=191 Caption='封包类型' TabOrder=0
    public GroupBox grpLogUser;      // DFM: grpLogUser Left=101 Top=8 Width=231 Height=192 Caption='记录指定人物（为空表示记录所有）' TabOrder=1
    public CheckBox chkLogMove;      // DFM: chkLogMove Left=8 Top=20 Width=74 Height=17 Caption='移动相关' TabOrder=0
    public CheckBox chkLogSpell;     // DFM: chkLogSpell Left=8 Top=62 Width=74 Height=17 Caption='魔法相关' TabOrder=1
    public CheckBox chkLogQuery;     // DFM: chkLogQuery Left=8 Top=83 Width=74 Height=17 Caption='查询相关' TabOrder=2
    public CheckBox chkLogTeam;      // DFM: chkLogTeam Left=8 Top=104 Width=74 Height=17 Caption='组队相关' TabOrder=3
    public CheckBox chkLogHit;       // DFM: chkLogHit Left=8 Top=41 Width=74 Height=17 Caption='攻击相关' TabOrder=4
    public CheckBox chkLogGuild;     // DFM: chkLogGuild Left=8 Top=125 Width=74 Height=17 Caption='行会相关' TabOrder=5
    public CheckBox chkLogShop;      // DFM: chkLogShop Left=8 Top=146 Width=74 Height=17 Caption='商铺摆摊' TabOrder=6
    public CheckBox chkLogOther;     // DFM: chkLogOther Left=8 Top=167 Width=74 Height=17 Caption='其他操作' Checked=True Enabled=False State=cbChecked TabOrder=7
    public Label lbl1;               // DFM: lbl1 Left=7 Top=168 Width=24 Height=12 Caption='人物'
    public ListBox lstLogUser;       // DFM: lstLogUser Left=8 Top=20 Width=216 Height=141 ItemHeight=12 TabOrder=0 OnClick=lstLogUserClick
    public TextBox edtUserName;      // DFM: edtUserName Left=35 Top=165 Width=100 Height=20 TabOrder=1
    public Button btnAddUser;        // DFM: btnAddUser Left=138 Top=164 Width=42 Height=22 Caption='添加' TabOrder=2
    public Button btnDelUser;        // DFM: btnDelUser（推断：grpLogUser 右下）TabOrder=3
    public Button btnOK;             // DFM: btnOK Caption='确定' OnClick=btnOKClick
    public Button btnCancel;         // DFM: btnCancel Caption='取消' ModalResult=2
    public CheckBox chkLogClientPacket; // DFM: chkLogClientPacket Caption='记录客户端封包'

    public FrmLogClientPacketSetting()
    {
        // DFM: FrmLogClientPacketSetting Caption='封包记录设置' BorderStyle=bsDialog Position=poMainFormCenter
        Text = "封包记录设置";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        Location = new Point(403, 335);
        ClientSize = new Size(339, 238);
        Font = new Font("宋体", 9F);                            // Font.Height=-12 Font.Name='宋体'
        MaximizeBox = false;
        MinimizeBox = false;

        // DFM: grpPacketType Left=8 Top=8 Width=87 Height=191 Caption='封包类型' TabOrder=0
        grpPacketType = new GroupBox { Left = 8, Top = 8, Width = 87, Height = 191, Text = "封包类型", TabIndex = 0 };
        // DFM: chkLogMove Left=8 Top=20 Width=74 Height=17 Caption='移动相关' TabOrder=0
        chkLogMove = new CheckBox { Left = 8, Top = 20, Width = 74, Height = 17, Text = "移动相关", TabIndex = 0 };
        // DFM: chkLogSpell Left=8 Top=62 Width=74 Height=17 Caption='魔法相关' TabOrder=1
        chkLogSpell = new CheckBox { Left = 8, Top = 62, Width = 74, Height = 17, Text = "魔法相关", TabIndex = 1 };
        // DFM: chkLogQuery Left=8 Top=83 Width=74 Height=17 Caption='查询相关' TabOrder=2
        chkLogQuery = new CheckBox { Left = 8, Top = 83, Width = 74, Height = 17, Text = "查询相关", TabIndex = 2 };
        // DFM: chkLogTeam Left=8 Top=104 Width=74 Height=17 Caption='组队相关' TabOrder=3
        chkLogTeam = new CheckBox { Left = 8, Top = 104, Width = 74, Height = 17, Text = "组队相关", TabIndex = 3 };
        // DFM: chkLogHit Left=8 Top=41 Width=74 Height=17 Caption='攻击相关' TabOrder=4
        chkLogHit = new CheckBox { Left = 8, Top = 41, Width = 74, Height = 17, Text = "攻击相关", TabIndex = 4 };
        // DFM: chkLogGuild Left=8 Top=125 Width=74 Height=17 Caption='行会相关' TabOrder=5
        chkLogGuild = new CheckBox { Left = 8, Top = 125, Width = 74, Height = 17, Text = "行会相关", TabIndex = 5 };
        // DFM: chkLogShop Left=8 Top=146 Width=74 Height=17 Caption='商铺摆摊' TabOrder=6
        chkLogShop = new CheckBox { Left = 8, Top = 146, Width = 74, Height = 17, Text = "商铺摆摊", TabIndex = 6 };
        // DFM: chkLogOther Left=8 Top=167 Width=74 Height=17 Caption='其他操作' Checked=True Enabled=False State=cbChecked TabOrder=7
        chkLogOther = new CheckBox { Left = 8, Top = 167, Width = 74, Height = 17, Text = "其他操作", TabIndex = 7,
                                     Checked = true, Enabled = false };

        // DFM: grpLogUser Left=101 Top=8 Width=231 Height=192 Caption='记录指定人物（为空表示记录所有）' TabOrder=1
        grpLogUser = new GroupBox { Left = 101, Top = 8, Width = 231, Height = 192,
                                    Text = "记录指定人物（为空表示记录所有）", TabIndex = 1 };
        // DFM: lbl1 Left=7 Top=168 Width=24 Height=12 Caption='人物'
        lbl1 = new Label { Left = 7, Top = 168, Width = 24, Height = 12, Text = "人物" };
        // DFM: lstLogUser Left=8 Top=20 Width=216 Height=141 ItemHeight=12 TabOrder=0 OnClick=lstLogUserClick
        lstLogUser = new ListBox { Left = 8, Top = 20, Width = 216, Height = 141, TabIndex = 0 };
        // DFM: edtUserName Left=35 Top=165 Width=100 Height=20 TabOrder=1
        edtUserName = new TextBox { Left = 35, Top = 165, Width = 100, Height = 20, TabIndex = 1 };
        // DFM: btnAddUser Left=138 Top=164 Width=42 Height=22 Caption='添加' TabOrder=2
        btnAddUser = new Button { Left = 138, Top = 164, Width = 42, Height = 22, Text = "添加", TabIndex = 2 };
        // DFM: btnDelUser Left=183 Top=164 Width=42 Height=22 Caption='删除' TabOrder=3 OnClick=btnDelUserClick
        btnDelUser = new Button { Left = 183, Top = 164, Width = 42, Height = 22, Text = "删除", TabIndex = 3 };

        // DFM: chkLogClientPacket Left=8 Top=210 Width=105 Height=17 Caption='开启封包记录'
        //      Font.Style=[fsBold] ParentFont=False TabOrder=4
        chkLogClientPacket = new CheckBox { Left = 8, Top = 210, Width = 105, Height = 17,
                                            Text = "开启封包记录", TabIndex = 4,
                                            Font = new Font("宋体", 9F, FontStyle.Bold) };
        // DFM: btnOK Left=176 Top=206 Width=75 Height=25 Caption='确定' TabOrder=2 OnClick=btnOKClick
        btnOK = new Button { Left = 176, Top = 206, Width = 75, Height = 25, Text = "确定", TabIndex = 2 };
        // DFM: btnCancel Left=256 Top=206 Width=75 Height=25 Caption='取消' ModalResult=2 TabOrder=3
        btnCancel = new Button { Left = 256, Top = 206, Width = 75, Height = 25, Text = "取消", TabIndex = 3,
                                 DialogResult = DialogResult.Cancel };

        grpPacketType.Controls.AddRange(new Control[] { chkLogMove, chkLogSpell, chkLogQuery, chkLogTeam,
                                                        chkLogHit, chkLogGuild, chkLogShop, chkLogOther });
        grpLogUser.Controls.AddRange(new Control[] { lbl1, lstLogUser, edtUserName, btnAddUser, btnDelUser });
        Controls.AddRange(new Control[] { grpPacketType, grpLogUser, chkLogClientPacket, btnOK, btnCancel });

        // DFM 的事件接线
        Load += (s, e) => FormCreate(s, e);                                      // OnCreate=FormCreate
        btnOK.Click += (s, e) => btnOK_Click(s, e);                              // OnClick=btnOKClick
        btnAddUser.Click += (s, e) => btnAddUser_Click(s, e);
        btnDelUser.Click += (s, e) => btnDelUser_Click(s, e);
        lstLogUser.Click += (s, e) => lstLogUser_Click(s, e);
    }

    /// <summary>原 uFrmLogClientPacketSetting.pas:124-138 `FormCreate`。</summary>
    public void FormCreate(object sender, EventArgs e)
    {
        var v = LogClientPacketSettingLogic.Open();
        chkLogMove.Checked = v.LogMove;                             // 原 :126
        chkLogHit.Checked = v.LogHit;                               // 原 :127
        chkLogSpell.Checked = v.LogSpell;                           // 原 :128
        chkLogQuery.Checked = v.LogQuery;                           // 原 :129
        chkLogTeam.Checked = v.LogTeam;                             // 原 :130
        chkLogGuild.Checked = v.LogGuild;                           // 原 :131
        chkLogShop.Checked = v.LogShop;                             // 原 :132
        chkLogClientPacket.Checked = v.LogClientPacket;             // 原 :134

        lstLogUser.Items.Clear();
        foreach (string s in v.LogUsers) lstLogUser.Items.Add(s);   // 原 :136 Items.Text := ...Text
        btnDelUser.Enabled = lstLogUser.Items.Count > 0 && lstLogUser.SelectedIndex >= 0;   // 原 :137
    }

    /// <summary>原 uFrmLogClientPacketSetting.pas:58-92 `btnOKClick`。</summary>
    public void btnOK_Click(object sender, EventArgs e)
    {
        var v = new LogClientPacketSettingValues
        {
            LogMove = chkLogMove.Checked,                    // 原 :64
            LogHit = chkLogHit.Checked,                      // 原 :65
            LogSpell = chkLogSpell.Checked,                  // 原 :66
            LogQuery = chkLogQuery.Checked,                  // 原 :67
            LogTeam = chkLogTeam.Checked,                    // 原 :68
            LogGuild = chkLogGuild.Checked,                  // 原 :69
            LogShop = chkLogShop.Checked,                    // 原 :70
            LogOther = chkLogOther.Checked,                  // （原文不读，仅保留取值以记录"看起来一样实则不同"）
            LogClientPacket = chkLogClientPacket.Checked     // 原 :73
        };
        foreach (object o in lstLogUser.Items) v.LogUsers.Add(o.ToString());   // 原 :77 Items.Text

        LogClientPacketSettingLogic.ButtonOK(v, FormGlobals.g_sIniFileName, FormGlobals.g_sLogClientPakcetUserFile);
        DialogResult = DialogResult.OK;                      // 原 :91 ModalResult := mrOK
    }

    /// <summary>原 uFrmLogClientPacketSetting.pas:94-114 `btnAddUserClick`。</summary>
    public void btnAddUser_Click(object sender, EventArgs e)
    {
        var existing = new List<string>();
        foreach (object o in lstLogUser.Items) existing.Add(o.ToString());

        if (!LogClientPacketSettingLogic.ValidateAddUser(edtUserName.Text, existing, out string error))
        {
            MessageBoxSeam.ShowInformation(error, "提示");        // 原 :101 / :108
            edtUserName.Focus();                                  // 原 :102 / :109
            return;                                              // 原 :103 / :110 Exit
        }

        lstLogUser.Items.Add(DelphiRTL.Trim(edtUserName.Text));   // 原 :113（Add 的是 Trim 后的 UserName）
    }

    /// <summary>原 uFrmLogClientPacketSetting.pas:116-122 `btnDelUserClick`。</summary>
    public void btnDelUser_Click(object sender, EventArgs e)
    {
        if (lstLogUser.Items.Count > 0 && lstLogUser.SelectedIndex >= 0)   // 原 :118
        {
            lstLogUser.Items.RemoveAt(lstLogUser.SelectedIndex);           // 原 :120 DeleteSelected
        }
    }

    /// <summary>原 uFrmLogClientPacketSetting.pas:140-143 `lstLogUserClick`。</summary>
    public void lstLogUser_Click(object sender, EventArgs e)
        => btnDelUser.Enabled = lstLogUser.Items.Count > 0 && lstLogUser.SelectedIndex >= 0;   // 原 :142
}
