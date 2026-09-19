using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.LogDataServer;

/// <summary>
/// uFrmRemoteQuerySetting.pas:96/107 GetAveCharSize + InputQueryEx 1:1
/// （与 BasicSet.pas:241/252 同源重复实现，此处按 LogDataServer 侧独立保留）。
/// </summary>
public static class LogDataInputQuery
{
    public const int FORM_WIDTH = 280;

    /// <summary>测试注入：(ACaption, APrompt, AHint, Value) → true 表示确定并返回新值。</summary>
    public static System.Func<string, string, string, string, (bool ok, string value)>? Handler;

    /// <summary>uFrmRemoteQuerySetting.pas:96 GetAveCharSize（Result.X := Result.X div 52）。</summary>
    public static System.Drawing.Point GetAveCharSize(System.Drawing.Graphics canvas, System.Drawing.Font font)
    {
        var buf = new char[52];
        for (int I = 0; I <= 25; I++) buf[I] = (char)('A' + I);
        for (int I = 0; I <= 25; I++) buf[I + 26] = (char)('a' + I);
        System.Drawing.SizeF size = canvas.MeasureString(new string(buf), font);
        return new System.Drawing.Point((int)size.Width / 52, (int)size.Height);
    }

    private static int MulDiv(int number, int numerator, int denominator)
        => (int)System.Math.Round((double)number * numerator / denominator, System.MidpointRounding.AwayFromZero);

    /// <summary>uFrmRemoteQuerySetting.pas:107 InputQueryEx。</summary>
    public static bool InputQueryEx(string ACaption, string APrompt, string AHint, ref string Value, bool showModal = true)
    {
        if (!showModal || Handler != null)
        {
            var (ok, value) = Handler != null ? Handler(ACaption, APrompt, AHint, Value) : (false, Value);
            if (ok) Value = value;
            return ok;
        }

        bool Result = false;
        using var Form = new System.Windows.Forms.Form();
        Form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        Form.Text = ACaption;
        Form.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        Form.MinimizeBox = false;
        Form.MaximizeBox = false;
        Form.ShowInTaskbar = false;

        using var g = Form.CreateGraphics();
        System.Drawing.Point DialogUnits = GetAveCharSize(g, Form.Font);
        Form.ClientSize = new System.Drawing.Size(MulDiv(FORM_WIDTH, DialogUnits.X, 4), 120);

        var Prompt = new System.Windows.Forms.Label
        {
            Text = APrompt,
            Left = MulDiv(8, DialogUnits.X, 4),
            Top = MulDiv(8, DialogUnits.Y, 8),
            MaximumSize = new System.Drawing.Size(MulDiv(FORM_WIDTH - 16, DialogUnits.X, 4), 0),
            AutoSize = true,
        };
        Form.Controls.Add(Prompt);

        var Edit = new System.Windows.Forms.TextBox
        {
            Left = Prompt.Left,
            Top = Prompt.Top + Prompt.Height + 5,
            Width = MulDiv(FORM_WIDTH - 16, DialogUnits.X, 4),
            MaxLength = 255,
            Text = Value,
        };
        Edit.SelectAll();
        Form.Controls.Add(Edit);

        int ButtonTop = Edit.Top + Edit.Height + 8;
        int ButtonWidth = MulDiv(50, DialogUnits.X, 4);
        int ButtonHeight = MulDiv(14, DialogUnits.Y, 8);

        var OkBtn = new System.Windows.Forms.Button
        {
            Text = "确定",
            DialogResult = System.Windows.Forms.DialogResult.OK,
            Left = Edit.Left + Edit.Width - ButtonWidth * 2 - 6,
            Top = ButtonTop,
            Width = ButtonWidth,
            Height = ButtonHeight,
        };
        Form.Controls.Add(OkBtn);
        Form.AcceptButton = OkBtn;

        var CancelBtn = new System.Windows.Forms.Button
        {
            Text = "取消",
            DialogResult = System.Windows.Forms.DialogResult.Cancel,
            Left = Edit.Left + Edit.Width - ButtonWidth,
            Top = ButtonTop,
            Width = ButtonWidth,
            Height = ButtonHeight,
        };
        Form.Controls.Add(CancelBtn);
        Form.CancelButton = CancelBtn;
        Form.ClientSize = new System.Drawing.Size(Form.ClientSize.Width, CancelBtn.Top + CancelBtn.Height + 10);

        var Hint = new System.Windows.Forms.Label
        {
            Text = AHint,
            ForeColor = System.Drawing.Color.Blue,
            Left = Edit.Left,
            Top = ButtonTop + (ButtonHeight - 12) / 2,
            MaximumSize = new System.Drawing.Size(MulDiv(FORM_WIDTH - 16, DialogUnits.X, 4), 0),
            AutoSize = true,
        };
        Form.Controls.Add(Hint);

        if (Form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            Value = Edit.Text;
            Result = true;
        }
        return Result;
    }
}

/// <summary>
/// uFrmRemoteQuerySetting.pas TFrmRemoteQuerySetting 1:1（远程查询设置）。
/// 源码：uFrmRemoteQuerySetting.pas:43-257；DFM（文本）：uFrmRemoteQuerySetting.dfm
/// （Caption='远程查询设置', ClientWidth=403, ClientHeight=227, bsDialog）。
/// </summary>
public sealed class TFrmRemoteQuerySetting : System.Windows.Forms.Form
{
    public System.Windows.Forms.Label lbl1 = null!;
    public System.Windows.Forms.Label lbl2 = null!;
    public System.Windows.Forms.GroupBox grp2 = null!;
    public System.Windows.Forms.Label lbl3 = null!;
    public System.Windows.Forms.Label lbl4 = null!;
    public System.Windows.Forms.TextBox edtPassword = null!;
    public System.Windows.Forms.GroupBox grp1 = null!;
    public System.Windows.Forms.ListBox lstControlIPList = null!;
    public System.Windows.Forms.NumericUpDown sePort = null!;
    public System.Windows.Forms.Button btnOK = null!;
    public System.Windows.Forms.ContextMenuStrip pmControlIPList = null!;
    public System.Windows.Forms.ToolStripMenuItem mniIPAdd = null!;
    public System.Windows.Forms.ToolStripMenuItem mniIPDelete = null!;
    public System.Windows.Forms.ToolStripMenuItem mniIPClear = null!;

    /// <summary>uFrmRemoteQuerySetting.pas:62 的 '.\LogData.ini'（测试可改）。</summary>
    public static string IniFileName = @".\LogData.ini";

    public TFrmRemoteQuerySetting()
    {
        InitializeComponent();
        FormCreate(this);
    }

    private void InitializeComponent()
    {
        // DFM: Caption = '远程查询设置' / BorderStyle = bsDialog / Position = poMainFormCenter
        Text = "远程查询设置";
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        // DFM: ClientWidth = 403  ClientHeight = 227
        ClientSize = new System.Drawing.Size(403, 227);

        // DFM: lbl1 TLabel(213,87) Caption=端口为'0'关闭远程管理功能（蓝色宋体 12）
        lbl1 = new System.Windows.Forms.Label { Text = "端口为'0'关闭远程管理功能", Left = 213, Top = 87, Width = 138, Height = 12, ForeColor = System.Drawing.Color.Blue, AutoSize = false };
        // DFM: lbl2 TLabel(213,106) Caption=无连接'IP'限制时，可以任意'IP'连接
        lbl2 = new System.Windows.Forms.Label { Text = "无连接'IP'限制时，可以任意'IP'连接", Left = 213, Top = 106, Width = 180, Height = 12, ForeColor = System.Drawing.Color.Blue, AutoSize = false };

        // DFM: grp2 TGroupBox(214,6,180,77) Caption='远程查询设置' TabOrder=0
        grp2 = new System.Windows.Forms.GroupBox { Text = "远程查询设置", Left = 214, Top = 6, Width = 180, Height = 77, TabIndex = 0 };
        lbl3 = new System.Windows.Forms.Label { Text = "查询端口:", Left = 9, Top = 21, Width = 52, Height = 13, AutoSize = false };
        lbl4 = new System.Windows.Forms.Label { Text = "查询密码:", Left = 9, Top = 47, Width = 52, Height = 13, AutoSize = false };
        // DFM: edtPassword TEdit(65,43,105,21) MaxLength=30 TabOrder=0
        edtPassword = new System.Windows.Forms.TextBox { Left = 65, Top = 43, Width = 105, Height = 21, MaxLength = 30, TabIndex = 0 };
        // DFM: sePort TSpinEditEx(64,16,105,22) MaxValue=65535 MinValue=0 Value=0 TabOrder=1
        sePort = new System.Windows.Forms.NumericUpDown { Left = 64, Top = 16, Width = 105, Height = 22, Maximum = 65535, Minimum = 0, Value = 0, TabIndex = 1 };
        grp2.Controls.Add(lbl3);
        grp2.Controls.Add(lbl4);
        grp2.Controls.Add(edtPassword);
        grp2.Controls.Add(sePort);

        // DFM: grp1 TGroupBox(6,6,195,214) Caption='允许连接IP' TabOrder=1
        grp1 = new System.Windows.Forms.GroupBox { Text = "允许连接IP", Left = 6, Top = 6, Width = 195, Height = 214, TabIndex = 1 };
        // DFM: lstControlIPList TListBox(8,16,178,191) ItemHeight=13 TabOrder=0
        lstControlIPList = new System.Windows.Forms.ListBox { Left = 8, Top = 16, Width = 178, Height = 191, TabIndex = 0, IntegralHeight = false };
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

        // DFM: btnOK TButton(319,195,75,25) Caption='确定' TabOrder=2
        btnOK = new System.Windows.Forms.Button { Text = "确定", Left = 319, Top = 195, Width = 75, Height = 25, TabIndex = 2 };
        btnOK.Click += (s, e) => btnOKClick(s);

        Controls.Add(lbl1);
        Controls.Add(lbl2);
        Controls.Add(grp2);
        Controls.Add(grp1);
        Controls.Add(btnOK);
    }

    /// <summary>uFrmRemoteQuerySetting.pas:43 ShowFrmRemoteQuerySetting（全局过程）。</summary>
    public static void ShowFrmRemoteQuerySetting(bool showModal = true)
    {
        var FrmRemoteQuerySetting = new TFrmRemoteQuerySetting();
        try
        {
            if (showModal) FrmRemoteQuerySetting.ShowDialog();
        }
        finally
        {
            FrmRemoteQuerySetting.Dispose();
        }
    }

    /// <summary>uFrmRemoteQuerySetting.pas:55 btnOKClick（写 .\LogData.ini 的 Setup/ControlPort|ControlPassword）。</summary>
    public void btnOKClick(object? Sender)
    {
        LogDataShare.g_nControlPort = (ushort)sePort.Value;
        LogDataShare.g_sControlPassword = edtPassword.Text;

        TFastIniFile Conf = new(IniFileName);
        Conf.WriteInteger("Setup", "ControlPort", LogDataShare.g_nControlPort);
        Conf.WriteString("Setup", "ControlPassword", LogDataShare.g_sControlPassword);
        Conf.UpdateFile();

        DialogResult = System.Windows.Forms.DialogResult.OK;
    }

    /// <summary>uFrmRemoteQuerySetting.pas:73 ErrMessage。</summary>
    private void ErrMessage(string MsgStr)
    {
        LogDataForms.MessageBox(MsgStr, "错误", LogDataForms.MB_OK | LogDataForms.MB_ICONERROR);
    }

    /// <summary>uFrmRemoteQuerySetting.pas:78 FormCreate。</summary>
    public void FormCreate(object? Sender)
    {
        sePort.Value = ClampSpin(sePort, LogDataShare.g_nControlPort);
        edtPassword.Text = LogDataShare.g_sControlPassword;

        LogDataShare.g_ControlIPList.Lock();
        try
        {
            for (int I = 0; I <= LogDataShare.g_ControlIPList.Count - 1; I++)
            {
                lstControlIPList.Items.Add(LogDataShare.g_ControlIPList[I]);
            }
        }
        finally
        {
            LogDataShare.g_ControlIPList.UnLock();
        }
    }

    /// <summary>Delphi TSpinEdit.SetValue：越界先夹到 [MinValue, MaxValue]（1:1）。</summary>
    private static decimal ClampSpin(System.Windows.Forms.NumericUpDown spin, decimal v)
    {
        if (v < spin.Minimum) return spin.Minimum;
        if (v > spin.Maximum) return spin.Maximum;
        return v;
    }

    /// <summary>uFrmRemoteQuerySetting.pas:204 mniIPAddClick。</summary>
    public void mniIPAddClick(object? Sender)
    {
        string sIPaddress = "";
        if (!LogDataInputQuery.InputQueryEx("永久IP过滤", "请输入一个新的IP地址: ", "如：202.103.100.20", ref sIPaddress)) return;
        if (!HUtil32.IsIPaddr(sIPaddress))
        {
            ErrMessage("输入的地址格式错误！");
            return;
        }

        LogDataShare.g_ControlIPList.Lock();
        try
        {
            if (LogDataShare.g_ControlIPList.IndexOf(sIPaddress) < 0)
            {
                LogDataShare.g_ControlIPList.Add(sIPaddress);
                lstControlIPList.Items.Add(sIPaddress);

                LogDataShare.g_ControlIPList.SaveToFile(LogDataShare.g_ControlIPFile);
            }
        }
        finally
        {
            LogDataShare.g_ControlIPList.UnLock();
        }
    }

    /// <summary>uFrmRemoteQuerySetting.pas:230 mniIPDeleteClick（★ 原文删除后不刷新 ItemIndex）。</summary>
    public void mniIPDeleteClick(object? Sender)
    {
        if ((lstControlIPList.SelectedIndex >= 0) && (lstControlIPList.SelectedIndex < lstControlIPList.Items.Count))
        {
            LogDataShare.g_ControlIPList.Lock();
            try
            {
                LogDataShare.g_ControlIPList.Delete(lstControlIPList.SelectedIndex);
            }
            finally
            {
                LogDataShare.g_ControlIPList.UnLock();
            }
            lstControlIPList.Items.RemoveAt(lstControlIPList.SelectedIndex);

            LogDataShare.g_ControlIPList.SaveToFile(LogDataShare.g_ControlIPFile);
        }
    }

    /// <summary>uFrmRemoteQuerySetting.pas:246 mniIPClearClick。</summary>
    public void mniIPClearClick(object? Sender)
    {
        LogDataShare.g_ControlIPList.Lock();
        try
        {
            LogDataShare.g_ControlIPList.Clear();
        }
        finally
        {
            LogDataShare.g_ControlIPList.UnLock();
        }

        lstControlIPList.Items.Clear();
        LogDataShare.g_ControlIPList.SaveToFile(LogDataShare.g_ControlIPFile);
    }
}
