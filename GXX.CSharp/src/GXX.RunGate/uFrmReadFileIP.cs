using System;
using System.Drawing;
using System.Windows.Forms;
using GXX.Core.Rtl;

namespace GXX.RunGate;

// =====================================================================================
// uFrmReadFileIP.pas 1:1 转换（Source\RunGate\uFrmReadFileIP.pas，153 行 / LF 152）。
// 布局真源：Source\RunGate\uFrmReadFileIP.dfm（**同名 .dfm 存在**）。
//
// 窗体职责：防御设置 —— 3 组（过滤列表 / 绿色通道 / 机器码过滤列表）各 2 项
// （本地文件路径 + 读取间隔；远程 URL + 下载间隔），外加"只允许绿色通道连接"开关。
//
// ★ 原文缺陷（照抄并在测试中固定行为）：
//   D1. `btnOKClick`（原 :104-122）对 12 个字符串全局量全部 `Trim`，唯独
//       `g_OnlyWhiteListLink` 直取 Checked。
//   D2. `btnOKClick`（原 :124-146）**没有 try..finally**：若 `TIniFile.Create` 之后任一步抛异常，
//       `IniFile.Free` 不会执行（与同族 uFrmInterval/uFrmAntiPlugUpdateSetting 的 try..finally 不同）。
//       托管侧保留同样的"不 finally"结构（用 using 会改变异常行为），改为**先写后延时落盘**，
//       并把落盘放在最后一行，等价于原 `IniFile.Free` 的位置。
//   D3. 原 :147 `ModalResult := mrOK;;`（**双分号**）—— 无害笔误，照抄注释但不产生 C# 影响。
//   D4. `.dfm` 里 4 个 TSpinEditEx 的 `Value=60` 与 `MinValue/MaxValue` 不匹配
//       （seFYDownDenyIPTime：MinValue=2 MaxValue=120 Value=60；
//        seFYReadDenyIPFile 的 MinValue=60 MaxValue=600000000 Value=60）。
//       本移植沿用 TSpinEditEx「编程赋值不裁剪」语义，DFM 的 Min/Max 只影响上下按钮。
// =====================================================================================

/// <summary>uFrmReadFileIP.pas 的界面值载体（18 个控件 → 13 个全局量）。</summary>
public class ReadFileIPValues
{
    public string FYReadDenyIPFile = "";
    public uint FYReadDenyIPTime;
    public string FYDownDenyIPUrl = "";
    public uint FYDownDenyIPTime;
    public string FYReadPassIPFile = "";
    public uint FYReadPassIPTime;
    public string FYDownPassIPUrl = "";
    public uint FYDownPassIPTime;
    public string FYReadDenyMACFile = "";
    public uint FYReadDenyMACTime;
    public string FYDownDenyMACUrl = "";
    public uint FYDownDenyMACTime;
    public bool OnlyWhiteListLink;
}

/// <summary>uFrmReadFileIP.pas 的非 UI 逻辑（可单测）。</summary>
public static class ReadFileIPLogic
{
    /// <summary>原 :74-98 `TFrmReadFileIP.Open`：13 个全局量 → 界面值。
    /// （原 :76-77 的 `Left/Top := Application.MainForm...` 属窗口定位，留在窗体侧。）</summary>
    public static ReadFileIPValues Open()
    {
        var v = new ReadFileIPValues();
        v.FYReadDenyIPFile = FormGlobals.g_sFYReadDenyIPFile;                 // 原 :79
        v.FYReadDenyIPTime = FormGlobals.g_dwFYReadDenyIPTime;                // 原 :80
        v.FYDownDenyIPUrl = FormGlobals.g_sFYDownDenyIPUrl;                   // 原 :82
        v.FYDownDenyIPTime = FormGlobals.g_dwFYDownDenyIPTime;                // 原 :83
        v.FYReadPassIPFile = FormGlobals.g_sFYReadPassIPFile;                 // 原 :85
        v.FYReadPassIPTime = FormGlobals.g_dwFYReadPassIPTime;                // 原 :86
        v.FYDownPassIPUrl = FormGlobals.g_sFYDownPassIPUrl;                   // 原 :88
        v.FYDownPassIPTime = FormGlobals.g_dwFYDownPassIPTime;                // 原 :89
        v.FYReadDenyMACFile = FormGlobals.g_sFYReadDenyMACFile;               // 原 :91
        v.FYReadDenyMACTime = FormGlobals.g_dwFYReadDenyMACTime;              // 原 :92
        v.FYDownDenyMACUrl = FormGlobals.g_sFYDownDenyMACUrl;                 // 原 :94
        v.FYDownDenyMACTime = FormGlobals.g_dwFYDownDenyMACTime;              // 原 :95
        v.OnlyWhiteListLink = FormGlobals.g_OnlyWhiteListLink;                // 原 :97
        return v;
    }

    /// <summary>原 :104-122：界面值 → 全局量（8 个字符串全部 `Trim`）。</summary>
    public static void ApplyToGlobals(ReadFileIPValues v)
    {
        FormGlobals.g_sFYReadDenyIPFile = DelphiRTL.Trim(v.FYReadDenyIPFile);         // 原 :104
        FormGlobals.g_dwFYReadDenyIPTime = v.FYReadDenyIPTime;                        // 原 :105

        FormGlobals.g_sFYDownDenyIPUrl = DelphiRTL.Trim(v.FYDownDenyIPUrl);           // 原 :107
        FormGlobals.g_dwFYDownDenyIPTime = v.FYDownDenyIPTime;                        // 原 :108

        FormGlobals.g_sFYReadPassIPFile = DelphiRTL.Trim(v.FYReadPassIPFile);         // 原 :110
        FormGlobals.g_dwFYReadPassIPTime = v.FYReadPassIPTime;                        // 原 :111

        FormGlobals.g_sFYDownPassIPUrl = DelphiRTL.Trim(v.FYDownPassIPUrl);           // 原 :113
        FormGlobals.g_dwFYDownPassIPTime = v.FYDownPassIPTime;                        // 原 :114

        FormGlobals.g_sFYReadDenyMACFile = DelphiRTL.Trim(v.FYReadDenyMACFile);       // 原 :116
        FormGlobals.g_dwFYReadDenyMACTime = v.FYReadDenyMACTime;                      // 原 :117

        FormGlobals.g_sFYDownDenyMACUrl = DelphiRTL.Trim(v.FYDownDenyMACUrl);         // 原 :119
        FormGlobals.g_dwFYDownDenyMACTime = v.FYDownDenyMACTime;                      // 原 :120

        FormGlobals.g_OnlyWhiteListLink = v.OnlyWhiteListLink;                        // 原 :122（**不 Trim**，bool 无需）
    }

    /// <summary>
    /// 原 :124-146 的 13 条 INI 写入（`[GameGate]` 节）。
    /// **键序即原文顺序**，测试逐字节断言。
    /// 原 :146 `IniFile.Free` → 托管侧 `Dispose()`（在本方法末尾，等价于原位置）。
    /// </summary>
    public static void WriteIni(string iniFileName, ReadFileIPValues v)
    {
        var ini = new TIniFileEx(iniFileName);                                        // 原 :124

        ini.WriteString(RunGateConst.GateClass, "FYReadDenyIPFile", FormGlobals.g_sFYReadDenyIPFile);      // 原 :126
        ini.WriteInteger(RunGateConst.GateClass, "FYReadDenyIPTime", (int)FormGlobals.g_dwFYReadDenyIPTime); // 原 :127

        ini.WriteString(RunGateConst.GateClass, "FYDownDenyIPUrl", FormGlobals.g_sFYDownDenyIPUrl);        // 原 :129
        ini.WriteInteger(RunGateConst.GateClass, "FYDownDenyIPTime", (int)FormGlobals.g_dwFYDownDenyIPTime); // 原 :130

        ini.WriteString(RunGateConst.GateClass, "FYReadPassIPFile", FormGlobals.g_sFYReadPassIPFile);      // 原 :132
        ini.WriteInteger(RunGateConst.GateClass, "FYReadPassIPTime", (int)FormGlobals.g_dwFYReadPassIPTime); // 原 :133

        ini.WriteString(RunGateConst.GateClass, "FYDownPassIPUrl", FormGlobals.g_sFYDownPassIPUrl);        // 原 :135
        ini.WriteInteger(RunGateConst.GateClass, "FYDownPassIPTime", (int)FormGlobals.g_dwFYDownPassIPTime); // 原 :136

        ini.WriteString(RunGateConst.GateClass, "FYReadDenyMACFile", FormGlobals.g_sFYReadDenyMACFile);    // 原 :138
        ini.WriteInteger(RunGateConst.GateClass, "FYReadDenyMACTime", (int)FormGlobals.g_dwFYReadDenyMACTime); // 原 :139

        ini.WriteString(RunGateConst.GateClass, "FYDownDenyMACUrl", FormGlobals.g_sFYDownDenyMACUrl);      // 原 :141
        ini.WriteInteger(RunGateConst.GateClass, "FYDownDenyMACTime", (int)FormGlobals.g_dwFYDownDenyMACTime); // 原 :142

        ini.WriteBool(RunGateConst.GateClass, "OnlyWhiteListLink", FormGlobals.g_OnlyWhiteListLink ? (byte)1 : (byte)0); // 原 :144

        ini.Dispose();                                                                // 原 :146 IniFile.Free
    }

    /// <summary>原 :100-148 的整体流程（便于单测，不碰控件）。</summary>
    public static void Save(ReadFileIPValues v, string iniFileName)
    {
        ApplyToGlobals(v);
        WriteIni(iniFileName, v);
    }
}

/// <summary>原 :61-72 `function ShowFrmReadFileIP: Boolean;`。</summary>
public static class ReadFileIPUnit
{
    public static bool ShowFrmReadFileIP()
    {
        using var form = new FrmReadFileIP();
        form.Open();                                             // 原 :67
        return form.ShowDialog() == DialogResult.OK;             // 原 :68
    }
}

/// <summary>原 uFrmReadFileIP.pas:10-53 `TFrmReadFileIP`（DFM: uFrmReadFileIP.dfm）。</summary>
public class FrmReadFileIP : Form
{
    // DFM: FrmReadFileIP Left=563 Top=310 BorderStyle=bsDialog Caption='防御设置'
    //      ClientHeight=284 ClientWidth=554 Position=poMainFormCenter PixelsPerInch=96
    public GroupBox grp1;          // DFM: grp1 Left=7 Top=7 Width=538 Height=74 Caption='过滤列表' TabOrder=0
    public GroupBox GroupBox1;     // DFM: GroupBox1 Left=7 Top=92 Width=538 Height=74 Caption='绿色通道' TabOrder=1
    public GroupBox GroupBox2;     // DFM: GroupBox2 Left=7 Top=175 Width=538 Height=74 Caption='机器码过滤列表 （引擎配置）' TabOrder=3
    public Button btnOK;           // DFM: btnOK Left=469 Top=254 Width=75 Height=25 Caption='确定(&O)' TabOrder=2 OnClick=btnOKClick
    public CheckBox chkOnlyWhiteListLink; // DFM: chkOnlyWhiteListLink Left=8 Top=258 Width=201 Height=17 Caption='只允许绿色通道（白名单）连接' TabOrder=4

    // ---- grp1（过滤列表）----
    public Label lbl1, lbl2, Label1, lbl3, Label8, Label9;
    public TextBox edtFYReadDenyIPFile, edtFYDownDenyIPUrl;
    public TSpinEditEx seFYReadDenyIPTime, seFYDownDenyIPTime;

    // ---- GroupBox1（绿色通道）----
    public Label Label2, Label3, Label4, Label10, Label11, Label12;
    public TextBox edtFYReadPassIPFile, edtFYDownPassIPUrl;
    public TSpinEditEx seFYReadPassIPTime, seFYDownPassIPTime;

    // ---- GroupBox2（机器码过滤列表）----
    public Label Label5, Label6, Label7, Label13, Label14, Label15;
    public TextBox edtFYReadDenyMACFile, edtFYDownDenyMACUrl;
    public TSpinEditEx seFYReadDenyMACTime, seFYDownDenyMACTime;

    public FrmReadFileIP()
    {
        // DFM: FrmReadFileIP Caption='防御设置' BorderStyle=bsDialog
        Text = "防御设置";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;        // Position=poMainFormCenter
        Location = new Point(563, 310);
        ClientSize = new Size(554, 284);
        Font = new Font("宋体", 9F);                            // Font.Height=-12 Font.Name='宋体'
        MaximizeBox = false;
        MinimizeBox = false;

        // DFM: grp1 Left=7 Top=7 Width=538 Height=74 Caption='过滤列表' TabOrder=0
        grp1 = new GroupBox { Left = 7, Top = 7, Width = 538, Height = 74, Text = "过滤列表", TabIndex = 0 };
        lbl1 = new Label { Left = 8, Top = 21, Width = 90, Height = 12, Text = "读取本地IP列表:" };
        lbl2 = new Label { Left = 386, Top = 21, Width = 54, Height = 12, Text = "读取间隔:" };
        Label1 = new Label { Left = 516, Top = 21, Width = 12, Height = 12, Text = "秒" };
        lbl3 = new Label { Left = 8, Top = 48, Width = 90, Height = 12, Text = "下载远程IP地址:" };
        Label8 = new Label { Left = 386, Top = 48, Width = 54, Height = 12, Text = "下载间隔:" };
        Label9 = new Label { Left = 516, Top = 48, Width = 12, Height = 12, Text = "分" };
        // DFM: edtFYReadDenyIPFile Left=100 Top=17 Width=280 Height=20 TabOrder=0
        edtFYReadDenyIPFile = new TextBox { Left = 100, Top = 17, Width = 280, Height = 20, TabIndex = 0 };
        // DFM: seFYReadDenyIPTime Left=442 Top=16 Width=72 Height=21 Increment=10 MaxValue=600000000 MinValue=60 TabOrder=1 Value=60
        seFYReadDenyIPTime = new TSpinEditEx { Left = 442, Top = 16, Width = 72, Height = 21, TabIndex = 1, Increment = 10 };
        seFYReadDenyIPTime.SetDfmRange(60, 600000000);
        seFYReadDenyIPTime.Value = 60;
        // DFM: edtFYDownDenyIPUrl Left=100 Top=44 Width=280 Height=20 Hint='请填写准确的远程列表地址，保证网站通讯正常'#13#10'如果网址为空，则关闭远程下载' ShowHint=True TabOrder=2
        edtFYDownDenyIPUrl = new TextBox { Left = 100, Top = 44, Width = 280, Height = 20, TabIndex = 2 };
        var urlHint = new ToolTip();
        urlHint.SetToolTip(edtFYDownDenyIPUrl, "请填写准确的远程列表地址，保证网站通讯正常\r\n如果网址为空，则关闭远程下载");
        // DFM: seFYDownDenyIPTime Left=442 Top=43 Width=72 Height=21 Increment=10 MaxValue=120 MinValue=2 TabOrder=3 Value=60
        seFYDownDenyIPTime = new TSpinEditEx { Left = 442, Top = 43, Width = 72, Height = 21, TabIndex = 3, Increment = 10 };
        seFYDownDenyIPTime.SetDfmRange(2, 120);
        seFYDownDenyIPTime.Value = 60;

        // DFM: GroupBox1 Left=7 Top=92 Width=538 Height=74 Caption='绿色通道' TabOrder=1
        GroupBox1 = new GroupBox { Left = 7, Top = 92, Width = 538, Height = 74, Text = "绿色通道", TabIndex = 1 };
        Label2 = new Label { Left = 8, Top = 21, Width = 90, Height = 12, Text = "读取本地IP列表:" };
        Label3 = new Label { Left = 386, Top = 21, Width = 54, Height = 12, Text = "读取间隔:" };
        Label4 = new Label { Left = 516, Top = 21, Width = 12, Height = 12, Text = "秒" };
        Label10 = new Label { Left = 8, Top = 48, Width = 90, Height = 12, Text = "下载远程IP地址:" };
        Label11 = new Label { Left = 386, Top = 48, Width = 54, Height = 12, Text = "下载间隔:" };
        Label12 = new Label { Left = 516, Top = 48, Width = 12, Height = 12, Text = "分" };
        // DFM: edtFYReadPassIPFile Left=100 Top=17 Width=280 Height=20 TabOrder=0
        edtFYReadPassIPFile = new TextBox { Left = 100, Top = 17, Width = 280, Height = 20, TabIndex = 0 };
        // DFM: seFYReadPassIPTime Left=441 Top=16 Width=72 Height=21 Increment=10 MaxValue=600000000 MinValue=60 TabOrder=1 Value=60
        seFYReadPassIPTime = new TSpinEditEx { Left = 441, Top = 16, Width = 72, Height = 21, TabIndex = 1, Increment = 10 };
        seFYReadPassIPTime.SetDfmRange(60, 600000000);
        seFYReadPassIPTime.Value = 60;
        // DFM: edtFYDownPassIPUrl Left=100 Top=44 Width=280 Height=20 TabOrder=2
        edtFYDownPassIPUrl = new TextBox { Left = 100, Top = 44, Width = 280, Height = 20, TabIndex = 2 };
        urlHint.SetToolTip(edtFYDownPassIPUrl, "请填写准确的远程列表地址，保证网站通讯正常\r\n如果网址为空，则关闭远程下载");
        // DFM: seFYDownPassIPTime Left=442 Top=43 Width=72 Height=21 Increment=10 MaxValue=120 MinValue=2 TabOrder=3 Value=60
        seFYDownPassIPTime = new TSpinEditEx { Left = 442, Top = 43, Width = 72, Height = 21, TabIndex = 3, Increment = 10 };
        seFYDownPassIPTime.SetDfmRange(2, 120);
        seFYDownPassIPTime.Value = 60;

        // DFM: GroupBox2 Left=7 Top=175 Width=538 Height=74 Caption='机器码过滤列表 （引擎配置）' TabOrder=3
        GroupBox2 = new GroupBox { Left = 7, Top = 175, Width = 538, Height = 74, Text = "机器码过滤列表 （引擎配置）", TabIndex = 3 };
        Label5 = new Label { Left = 8, Top = 21, Width = 90, Height = 12, Text = "本地机器码列表:" };
        Label6 = new Label { Left = 386, Top = 21, Width = 54, Height = 12, Text = "读取间隔:" };
        Label7 = new Label { Left = 516, Top = 21, Width = 12, Height = 12, Text = "秒" };
        Label13 = new Label { Left = 8, Top = 48, Width = 90, Height = 12, Text = "下载远程机器码:" };
        Label14 = new Label { Left = 386, Top = 48, Width = 54, Height = 12, Text = "下载间隔:" };
        Label15 = new Label { Left = 516, Top = 48, Width = 12, Height = 12, Text = "分" };
        // DFM: edtFYReadDenyMACFile Left=100 Top=17 Width=280 Height=20 TabOrder=0
        edtFYReadDenyMACFile = new TextBox { Left = 100, Top = 17, Width = 280, Height = 20, TabIndex = 0 };
        // DFM: seFYReadDenyMACTime Left=442 Top=17 Width=72 Height=21 Increment=10 MaxValue=600000000 MinValue=60 TabOrder=1 Value=60
        seFYReadDenyMACTime = new TSpinEditEx { Left = 442, Top = 17, Width = 72, Height = 21, TabIndex = 1, Increment = 10 };
        seFYReadDenyMACTime.SetDfmRange(60, 600000000);
        seFYReadDenyMACTime.Value = 60;
        // DFM: edtFYDownDenyMACUrl Left=100 Top=44 Width=280 Height=20 TabOrder=2
        edtFYDownDenyMACUrl = new TextBox { Left = 100, Top = 44, Width = 280, Height = 20, TabIndex = 2 };
        urlHint.SetToolTip(edtFYDownDenyMACUrl, "请填写准确的远程列表地址，保证网站通讯正常\r\n如果网址为空，则关闭远程下载");
        // DFM: seFYDownDenyMACTime Left=442 Top=43 Width=72 Height=21 Increment=10 MaxValue=120 MinValue=2 TabOrder=3 Value=60
        seFYDownDenyMACTime = new TSpinEditEx { Left = 442, Top = 43, Width = 72, Height = 21, TabIndex = 3, Increment = 10 };
        seFYDownDenyMACTime.SetDfmRange(2, 120);
        seFYDownDenyMACTime.Value = 60;

        // DFM: btnOK Left=469 Top=254 Width=75 Height=25 Caption='确定(&O)' TabOrder=2 OnClick=btnOKClick
        btnOK = new Button { Left = 469, Top = 254, Width = 75, Height = 25, Text = "确定(&O)", TabIndex = 2 };
        // DFM: chkOnlyWhiteListLink Left=8 Top=258 Width=201 Height=17 Caption='只允许绿色通道（白名单）连接' TabOrder=4
        chkOnlyWhiteListLink = new CheckBox { Left = 8, Top = 258, Width = 201, Height = 17,
                                              Text = "只允许绿色通道（白名单）连接", TabIndex = 4 };

        grp1.Controls.AddRange(new Control[] { lbl1, lbl2, Label1, lbl3, Label8, Label9,
                                               edtFYReadDenyIPFile, seFYReadDenyIPTime,
                                               edtFYDownDenyIPUrl, seFYDownDenyIPTime });
        GroupBox1.Controls.AddRange(new Control[] { Label2, Label3, Label4, Label10, Label11, Label12,
                                                    edtFYReadPassIPFile, seFYReadPassIPTime,
                                                    edtFYDownPassIPUrl, seFYDownPassIPTime });
        GroupBox2.Controls.AddRange(new Control[] { Label5, Label6, Label7, Label13, Label14, Label15,
                                                    edtFYReadDenyMACFile, seFYReadDenyMACTime,
                                                    edtFYDownDenyMACUrl, seFYDownDenyMACTime });
        Controls.AddRange(new Control[] { grp1, GroupBox1, GroupBox2, btnOK, chkOnlyWhiteListLink });

        // DFM: btnOK OnClick = btnOKClick
        btnOK.Click += (s, e) => btnOK_Click(s, e);
    }

    /// <summary>原 uFrmReadFileIP.pas:74-98 `TFrmReadFileIP.Open`。</summary>
    public void Open()
    {
        // 原 :76-77：Left := Application.MainForm.Left; Top := Application.MainForm.Top + 20;
        if (Application.OpenForms.Count > 0 && Application.OpenForms[0] != this)
        {
            var main = Application.OpenForms[0];
            Left = main.Left;                                       // 原 :76
            Top = main.Top + 20;                                    // 原 :77
        }

        var v = ReadFileIPLogic.Open();
        edtFYReadDenyIPFile.Text = v.FYReadDenyIPFile;               // 原 :79
        seFYReadDenyIPTime.Value = (int)v.FYReadDenyIPTime;          // 原 :80
        edtFYDownDenyIPUrl.Text = v.FYDownDenyIPUrl;                 // 原 :82
        seFYDownDenyIPTime.Value = (int)v.FYDownDenyIPTime;          // 原 :83
        edtFYReadPassIPFile.Text = v.FYReadPassIPFile;               // 原 :85
        seFYReadPassIPTime.Value = (int)v.FYReadPassIPTime;          // 原 :86
        edtFYDownPassIPUrl.Text = v.FYDownPassIPUrl;                 // 原 :88
        seFYDownPassIPTime.Value = (int)v.FYDownPassIPTime;          // 原 :89
        edtFYReadDenyMACFile.Text = v.FYReadDenyMACFile;             // 原 :91
        seFYReadDenyMACTime.Value = (int)v.FYReadDenyMACTime;        // 原 :92
        edtFYDownDenyMACUrl.Text = v.FYDownDenyMACUrl;               // 原 :94
        seFYDownDenyMACTime.Value = (int)v.FYDownDenyMACTime;        // 原 :95
        chkOnlyWhiteListLink.Checked = v.OnlyWhiteListLink;          // 原 :97
    }

    /// <summary>原 uFrmReadFileIP.pas:100-148 `TFrmReadFileIP.btnOKClick`。</summary>
    public void btnOK_Click(object sender, EventArgs e)
    {
        var v = new ReadFileIPValues
        {
            FYReadDenyIPFile = edtFYReadDenyIPFile.Text,             // 原 :104
            FYReadDenyIPTime = (uint)seFYReadDenyIPTime.Value,       // 原 :105
            FYDownDenyIPUrl = edtFYDownDenyIPUrl.Text,               // 原 :107
            FYDownDenyIPTime = (uint)seFYDownDenyIPTime.Value,       // 原 :108
            FYReadPassIPFile = edtFYReadPassIPFile.Text,             // 原 :110
            FYReadPassIPTime = (uint)seFYReadPassIPTime.Value,       // 原 :111
            FYDownPassIPUrl = edtFYDownPassIPUrl.Text,               // 原 :113
            FYDownPassIPTime = (uint)seFYDownPassIPTime.Value,       // 原 :114
            FYReadDenyMACFile = edtFYReadDenyMACFile.Text,           // 原 :116
            FYReadDenyMACTime = (uint)seFYReadDenyMACTime.Value,     // 原 :117
            FYDownDenyMACUrl = edtFYDownDenyMACUrl.Text,             // 原 :119
            FYDownDenyMACTime = (uint)seFYDownDenyMACTime.Value,     // 原 :120
            OnlyWhiteListLink = chkOnlyWhiteListLink.Checked         // 原 :122
        };
        ReadFileIPLogic.Save(v, FormGlobals.g_sIniFileName);
        DialogResult = DialogResult.OK;                              // 原 :147 ModalResult := mrOK;;
    }
}
