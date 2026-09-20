using System;
using System.Drawing;
using System.Windows.Forms;

namespace GXX.RunGate;

// =====================================================================================
// uFrmAntiPlugUpdateSetting.pas 1:1 转换（Source\RunGate\uFrmAntiPlugUpdateSetting.pas，73 行 / LF 72）。
// 布局真源：Source\RunGate\uFrmAntiPlugUpdateSetting.dfm（**同名 .dfm 存在**）。
//
// 窗体职责：网关插件自动更新设置（勾选开关 + 检测间隔分钟 + 配置地址），
// 确定时写回 3 个全局量并落盘 Config.ini 的 [GameGate] 三键。
//
// 接缝（不依赖 WinForms 的纯逻辑侧）：
//   * AntiPlugUpdateSettingLogic.Open()      —— 全局量 → 界面值包（原 :45-50 FormCreate）
//   * AntiPlugUpdateSettingLogic.Save(...)   —— 界面值包 → 全局量 + INI（原 :52-70 btnOKClick）
//   * MessageBoxSeam                          —— 本窗体原文**没有**弹窗，保留接缝以免将来加分支时直接 MessageBox
// =====================================================================================

/// <summary>原 :45-70 的界面值载体（`chkAntiPlugAutoUpdateCheck.Checked` / `seAntiPlugUpdateCheckInterval.Value` / `edtAntiPlugUpdateConfigUrl.Text`）。</summary>
public class AntiPlugUpdateSettingValues
{
    /// <summary>原 :47 / :56 `chkAntiPlugAutoUpdateCheck.Checked` → `g_boAntiPlugAutoUpdateCheck`。</summary>
    public bool AntiPlugAutoUpdateCheck;

    /// <summary>原 :48 / :57 `seAntiPlugUpdateCheckInterval.Value` → `g_wAntiPlugUpdateCheckInterval`。</summary>
    public int AntiPlugUpdateCheckInterval;

    /// <summary>原 :49 / :58 `edtAntiPlugUpdateConfigUrl.Text` → `g_sAntiPlugUpdateConfigUrl`。</summary>
    public string AntiPlugUpdateConfigUrl = "";
}

/// <summary>uFrmAntiPlugUpdateSetting.pas 的非 UI 逻辑（可单测）。</summary>
public static class AntiPlugUpdateSettingLogic
{
    /// <summary>
    /// 原 uFrmAntiPlugUpdateSetting.pas:45-50 `TFrmAntiPlugUpdateSetting.FormCreate`：
    ///   chkAntiPlugAutoUpdateCheck.Checked := g_boAntiPlugAutoUpdateCheck;
    ///   seAntiPlugUpdateCheckInterval.Value := g_wAntiPlugUpdateCheckInterval;
    ///   edtAntiPlugUpdateConfigUrl.Text     := g_sAntiPlugUpdateConfigUrl;
    /// 注意：**没有** Trim，**没有**范围裁剪（原 DFM 的 SpinEditEx 是 MaxValue=120 MinValue=1，
    /// 但 FormCreate 直接赋全局值，若配置里是 0 或 999 也会原样显示 —— 与 GXX.DBServer 的
    /// TSpinEdit「编程赋值不裁剪」语义一致）。
    /// </summary>
    public static AntiPlugUpdateSettingValues Open()
    {
        var v = new AntiPlugUpdateSettingValues();
        v.AntiPlugAutoUpdateCheck = FormGlobals.g_boAntiPlugAutoUpdateCheck;               // 原 :47
        v.AntiPlugUpdateCheckInterval = FormGlobals.g_wAntiPlugUpdateCheckInterval;        // 原 :48
        v.AntiPlugUpdateConfigUrl = FormGlobals.g_sAntiPlugUpdateConfigUrl;                // 原 :49
        return v;
    }

    /// <summary>
    /// 原 uFrmAntiPlugUpdateSetting.pas:52-70 `TFrmAntiPlugUpdateSetting.btnOKClick`：
    ///   1) 三个控件 → 三个全局量（原 :56-58，**顺序即原文顺序**）；
    ///   2) `TIniFile.Create(g_sIniFileName)` → 写 `[GameGate]` 三键（原 :60-67）；
    ///   3) `ModalResult := mrOk`（托管侧由窗体置 DialogResult.OK）。
    /// 键名逐字照抄，注意第三个键是 `AntiPlugUpdateConfigUrl5`（**带尾随数字 5**，原文如此，不是笔误可改）。
    /// </summary>
    public static void Save(AntiPlugUpdateSettingValues v, string iniFileName)
    {
        FormGlobals.g_boAntiPlugAutoUpdateCheck = v.AntiPlugAutoUpdateCheck;                       // 原 :56
        FormGlobals.g_wAntiPlugUpdateCheckInterval = v.AntiPlugUpdateCheckInterval;                // 原 :57
        FormGlobals.g_sAntiPlugUpdateConfigUrl = v.AntiPlugUpdateConfigUrl;                        // 原 :58

        var ini = new TIniFileEx(iniFileName);                                                     // 原 :60
        // 原 :62：IniFile.WriteBool(GateClass, 'AntiPlugAutoUpdateCheck', g_boAntiPlugAutoUpdateCheck);
        // 注意 TIniFileEx.WriteBool 的形参是 byte（Delphi Boolean 为 1 字节），不是 bool。
        ini.WriteBool(RunGateConst.GateClass, "AntiPlugAutoUpdateCheck", v.AntiPlugAutoUpdateCheck ? (byte)1 : (byte)0);
        // 原 :63：IniFile.WriteInteger(GateClass, 'AntiPlugUpdateCheckInterval', g_wAntiPlugUpdateCheckInterval);
        ini.WriteInteger(RunGateConst.GateClass, "AntiPlugUpdateCheckInterval", v.AntiPlugUpdateCheckInterval);
        // 原 :64：IniFile.WriteString(GateClass, 'AntiPlugUpdateConfigUrl5', g_sAntiPlugUpdateConfigUrl);
        //        ^^^ 键名末尾的 '5' 是原文原样，不可"顺手统一"为无 5 版本
        ini.WriteString(RunGateConst.GateClass, "AntiPlugUpdateConfigUrl5", v.AntiPlugUpdateConfigUrl);
        ini.Dispose();   // 原 :66 finally IniFile.Free → TIniFileEx.Dispose 落盘
    }
}

/// <summary>原 :33-43 `procedure ShowFrmAntiPlugUpdateSetting;`。</summary>
public static class AntiPlugUpdateSettingUnit
{
    /// <summary>原 :33-43：Create → ShowModal → Free（**返回值被丢弃**，与同族 ShowFrm* 不同）。</summary>
    public static void ShowFrmAntiPlugUpdateSetting()
    {
        using var form = new FrmAntiPlugUpdateSetting();
        form.ShowDialog();
    }
}

/// <summary>原 uFrmAntiPlugUpdateSetting.pas:10-25 `TFrmAntiPlugUpdateSetting`（DFM: uFrmAntiPlugUpdateSetting.dfm）。</summary>
public class FrmAntiPlugUpdateSetting : Form
{
    // DFM: FrmAntiPlugUpdateSetting Left=384 Top=383 BorderStyle=bsDialog Caption='网关插件自动更新设置'
    //      ClientHeight=145 ClientWidth=432 Font.Charset=GB2312_CHARSET Font.Height=-12 Font.Name='宋体'
    //      Position=poMainFormCenter OnCreate=FormCreate PixelsPerInch=96
    public GroupBox grpMain;                        // DFM: grpMain Left=8 Top=8 Width=417 Height=99 Caption='自动更新设置' TabOrder=0
    public Label lbl1;                              // DFM: lbl1 Left=19 Top=74 Width=108 Height=12 Caption='更新配置文件地址：'
    public Label lbl2;                              // DFM: lbl2 Left=19 Top=48 Width=108 Height=12 Caption='自动更新检测间隔：'
    public Label Label1;                            // DFM: Label1 Left=195 Top=48 Width=24 Height=12 Caption='分钟'
    public CheckBox chkAntiPlugAutoUpdateCheck;     // DFM: chkAntiPlugAutoUpdateCheck Left=8 Top=22 Width=129 Height=17 Caption='自动检测插件更新' TabOrder=0
    public TextBox edtAntiPlugUpdateConfigUrl;      // DFM: edtAntiPlugUpdateConfigUrl Left=125 Top=70 Width=285 Height=20 TabOrder=2
    public TSpinEditEx seAntiPlugUpdateCheckInterval; // DFM: seAntiPlugUpdateCheckInterval Left=125 Top=43 Width=68 Height=21 MaxValue=120 MinValue=1 TabOrder=1 Value=1
    public Button btnOK;                            // DFM: btnOK Left=350 Top=112 Width=75 Height=25 Caption='确定' TabOrder=1 OnClick=btnOKClick

    public FrmAntiPlugUpdateSetting()
    {
        // DFM: FrmAntiPlugUpdateSetting Caption='网关插件自动更新设置' BorderStyle=bsDialog
        Text = "网关插件自动更新设置";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;         // Position=poMainFormCenter
        Location = new Point(384, 383);
        ClientSize = new Size(432, 145);
        MaximizeBox = false;
        MinimizeBox = false;
        Font = new Font("宋体", 9F);                            // Font.Name='宋体' Font.Height=-12

        // DFM: grpMain Left=8 Top=8 Width=417 Height=99 Caption='自动更新设置' TabOrder=0
        grpMain = new GroupBox { Left = 8, Top = 8, Width = 417, Height = 99, Text = "自动更新设置", TabIndex = 0 };
        // DFM: lbl1 Left=19 Top=74 Width=108 Height=12 Caption='更新配置文件地址：'
        lbl1 = new Label { Left = 19, Top = 74, Width = 108, Height = 12, Text = "更新配置文件地址：" };
        // DFM: lbl2 Left=19 Top=48 Width=108 Height=12 Caption='自动更新检测间隔：'
        lbl2 = new Label { Left = 19, Top = 48, Width = 108, Height = 12, Text = "自动更新检测间隔：" };
        // DFM: Label1 Left=195 Top=48 Width=24 Height=12 Caption='分钟'
        Label1 = new Label { Left = 195, Top = 48, Width = 24, Height = 12, Text = "分钟" };
        // DFM: chkAntiPlugAutoUpdateCheck Left=8 Top=22 Width=129 Height=17 Caption='自动检测插件更新' TabOrder=0
        chkAntiPlugAutoUpdateCheck = new CheckBox { Left = 8, Top = 22, Width = 129, Height = 17, Text = "自动检测插件更新", TabIndex = 0 };
        // DFM: edtAntiPlugUpdateConfigUrl Left=125 Top=70 Width=285 Height=20 TabOrder=2
        edtAntiPlugUpdateConfigUrl = new TextBox { Left = 125, Top = 70, Width = 285, Height = 20, TabIndex = 2 };
        // DFM: seAntiPlugUpdateCheckInterval Left=125 Top=43 Width=68 Height=21 MaxValue=120 MinValue=1 TabOrder=1 Value=1
        seAntiPlugUpdateCheckInterval = new TSpinEditEx { Left = 125, Top = 43, Width = 68, Height = 21, TabIndex = 1 };
        seAntiPlugUpdateCheckInterval.SetDfmRange(1, 120);
        seAntiPlugUpdateCheckInterval.Value = 1;

        // DFM: btnOK Left=350 Top=112 Width=75 Height=25 Caption='确定' TabOrder=1 OnClick=btnOKClick
        btnOK = new Button { Left = 350, Top = 112, Width = 75, Height = 25, Text = "确定", TabIndex = 1 };

        grpMain.Controls.Add(lbl1);
        grpMain.Controls.Add(lbl2);
        grpMain.Controls.Add(Label1);
        grpMain.Controls.Add(chkAntiPlugAutoUpdateCheck);
        grpMain.Controls.Add(edtAntiPlugUpdateConfigUrl);
        grpMain.Controls.Add(seAntiPlugUpdateCheckInterval);

        Controls.Add(grpMain);
        Controls.Add(btnOK);

        // DFM: OnCreate = FormCreate
        Load += (s, e) => FormCreate(s, e);
        // DFM: btnOK OnClick = btnOKClick
        btnOK.Click += (s, e) => btnOK_Click(s, e);
    }

    /// <summary>原 uFrmAntiPlugUpdateSetting.pas:45-50 `TFrmAntiPlugUpdateSetting.FormCreate`。</summary>
    public void FormCreate(object sender, EventArgs e)
    {
        var v = AntiPlugUpdateSettingLogic.Open();
        chkAntiPlugAutoUpdateCheck.Checked = v.AntiPlugAutoUpdateCheck;              // 原 :47
        seAntiPlugUpdateCheckInterval.Value = v.AntiPlugUpdateCheckInterval;         // 原 :48
        edtAntiPlugUpdateConfigUrl.Text = v.AntiPlugUpdateConfigUrl;                 // 原 :49
    }

    /// <summary>原 uFrmAntiPlugUpdateSetting.pas:52-70 `TFrmAntiPlugUpdateSetting.btnOKClick`。</summary>
    public void btnOK_Click(object sender, EventArgs e)
    {
        var v = new AntiPlugUpdateSettingValues
        {
            AntiPlugAutoUpdateCheck = chkAntiPlugAutoUpdateCheck.Checked,                              // 原 :56
            AntiPlugUpdateCheckInterval = seAntiPlugUpdateCheckInterval.Value,                         // 原 :57
            AntiPlugUpdateConfigUrl = edtAntiPlugUpdateConfigUrl.Text                                  // 原 :58
        };
        AntiPlugUpdateSettingLogic.Save(v, FormGlobals.g_sIniFileName);
        DialogResult = DialogResult.OK;   // 原 :69 ModalResult := mrOk
    }
}
