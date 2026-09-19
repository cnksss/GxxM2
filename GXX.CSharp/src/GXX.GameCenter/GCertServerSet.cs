using System;
using System.IO;
using GXX.Core.Util;
using GXX.GameCenter;

namespace GXX.GameCenter;

/// <summary>
/// GCertServerSet.pas（131 行）1:1 移植：TfrmCertServerSet（验证服务器地址/端口设置窗体）。
/// <para>
/// <b>原文缺陷登记</b>：GCertServerSet.pas 第 78-144 行引用了 6 个 GShare.pas 中**不存在**的全局变量
/// （<c>g_sRunGate_Config_RegServerAddr</c> / <c>g_nRunGate_Config_RegServerPort</c> /
/// <c>g_sDBServer_Config_RegServerAddr</c> / <c>g_nDBServer_Config_RegServerPort</c> /
/// <c>g_sM2Server_Config_RegServerAddr</c> / <c>g_nM2Server_Config_RegServerPort</c>），
/// 且该单元未列入 GameCenter.dpr 的 uses（compiler 不编译它）。
/// 托管侧按原文名声明这些全局量并标注"原文未定义"，保持 1:1 与可编译。
/// </para>
/// <para>
/// <b>原文缺陷登记 2</b>：ButtonOKClick 的第三段校验（GMain.pas 等价位置 GCertServerSet.pas:129-138）
/// 重复校验了 RunGate 而不是 M2Server —— 即 <c>g_sM2Server_Config_RegServerAddr/Port</c>
/// 只赋值、从不校验（原文如此）。逐字保留该顺序。
/// </para>
/// </summary>
public sealed class CertServerSetForm : System.Windows.Forms.Form
{
    // ---- 控件（名称与 DFM 1:1） ----
    public System.Windows.Forms.GroupBox GroupBox1 = null!;
    public System.Windows.Forms.Label Label1 = null!;
    public System.Windows.Forms.TextBox EditRunGate_Config_RegServerAddr = null!;
    public System.Windows.Forms.Label Label2 = null!;
    public System.Windows.Forms.TextBox EditRunGate_Config_RegServerPort = null!;
    public System.Windows.Forms.GroupBox GroupBox2 = null!;
    public System.Windows.Forms.Label Label3 = null!;
    public System.Windows.Forms.Label Label4 = null!;
    public System.Windows.Forms.TextBox EditDBServer_Config_RegServerAddr = null!;
    public System.Windows.Forms.TextBox EditDBServer_Config_RegServerPort = null!;
    public System.Windows.Forms.GroupBox GroupBox3 = null!;
    public System.Windows.Forms.Label Label5 = null!;
    public System.Windows.Forms.Label Label6 = null!;
    public System.Windows.Forms.TextBox EditM2Server_Config_RegServerAddr = null!;
    public System.Windows.Forms.TextBox EditM2Server_Config_RegServerPort = null!;
    public System.Windows.Forms.Button ButtonOK = null!;

    private bool m_boOpened;
    private bool m_boModValued;

    /// <summary>最近的校验失败焦点控件名（测试断言用；对应原文 SetFocus）。</summary>
    public string? LastFocus { get; private set; }

    public CertServerSetForm()
    {
        InitializeComponent();
        FormCreate();
    }

    // ================= 布局（DFM: GCertServerSet.dfm 结构等效） =================

    private void InitializeComponent()
    {
        Text = "验证服务器设置";
        Width = 400;
        Height = 300;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

        GroupBox1 = new System.Windows.Forms.GroupBox { Text = "游戏网关", Left = 10, Top = 8, Width = 370, Height = 80 };
        Label1 = new System.Windows.Forms.Label { Text = "地址:", Left = 14, Top = 24, Width = 60 };
        EditRunGate_Config_RegServerAddr = new System.Windows.Forms.TextBox { Left = 80, Top = 21, Width = 270 };
        Label2 = new System.Windows.Forms.Label { Text = "端口:", Left = 14, Top = 50, Width = 60 };
        EditRunGate_Config_RegServerPort = new System.Windows.Forms.TextBox { Left = 80, Top = 47, Width = 270 };
        GroupBox1.Controls.AddRange(new System.Windows.Forms.Control[]
        { Label1, EditRunGate_Config_RegServerAddr, Label2, EditRunGate_Config_RegServerPort });
        Controls.Add(GroupBox1);

        GroupBox2 = new System.Windows.Forms.GroupBox { Text = "数据库", Left = 10, Top = 92, Width = 370, Height = 80 };
        Label3 = new System.Windows.Forms.Label { Text = "地址:", Left = 14, Top = 24, Width = 60 };
        EditDBServer_Config_RegServerAddr = new System.Windows.Forms.TextBox { Left = 80, Top = 21, Width = 270 };
        Label4 = new System.Windows.Forms.Label { Text = "端口:", Left = 14, Top = 50, Width = 60 };
        EditDBServer_Config_RegServerPort = new System.Windows.Forms.TextBox { Left = 80, Top = 47, Width = 270 };
        GroupBox2.Controls.AddRange(new System.Windows.Forms.Control[]
        { Label3, EditDBServer_Config_RegServerAddr, Label4, EditDBServer_Config_RegServerPort });
        Controls.Add(GroupBox2);

        GroupBox3 = new System.Windows.Forms.GroupBox { Text = "游戏引擎", Left = 10, Top = 176, Width = 370, Height = 80 };
        Label5 = new System.Windows.Forms.Label { Text = "地址:", Left = 14, Top = 24, Width = 60 };
        EditM2Server_Config_RegServerAddr = new System.Windows.Forms.TextBox { Left = 80, Top = 21, Width = 270 };
        Label6 = new System.Windows.Forms.Label { Text = "端口:", Left = 14, Top = 50, Width = 60 };
        EditM2Server_Config_RegServerPort = new System.Windows.Forms.TextBox { Left = 80, Top = 47, Width = 270 };
        GroupBox3.Controls.AddRange(new System.Windows.Forms.Control[]
        { Label5, EditM2Server_Config_RegServerAddr, Label6, EditM2Server_Config_RegServerPort });
        Controls.Add(GroupBox3);

        ButtonOK = new System.Windows.Forms.Button { Text = "确定(&O)", Left = 290, Top = 262, Width = 90, Height = 26 };
        ButtonOK.Click += (s, e) => ButtonOKClick(s);
        Controls.Add(ButtonOK);

        EditRunGate_Config_RegServerAddr.TextChanged += (s, e) => EditChange(s);
        EditRunGate_Config_RegServerPort.TextChanged += (s, e) => EditChange(s);
        EditDBServer_Config_RegServerAddr.TextChanged += (s, e) => EditChange(s);
        EditDBServer_Config_RegServerPort.TextChanged += (s, e) => EditChange(s);
        EditM2Server_Config_RegServerAddr.TextChanged += (s, e) => EditChange(s);
        EditM2Server_Config_RegServerPort.TextChanged += (s, e) => EditChange(s);
    }

    // ================= 原文方法 =================

    /// <summary>GCertServerSet.pas:51 <c>procedure TfrmCertServerSet.ModValue;</c></summary>
    public void ModValue()
    {
        m_boModValued = true;
        ButtonOK.Enabled = true;
    }

    /// <summary>GCertServerSet.pas:57 <c>procedure TfrmCertServerSet.uModValue;</c></summary>
    public void uModValue()
    {
        m_boModValued = false;
        ButtonOK.Enabled = false;
    }

    /// <summary>GCertServerSet.pas:63 <c>procedure TfrmCertServerSet.FormCreate(Sender: TObject);</c></summary>
    public void FormCreate()
    {
        m_boOpened = false;
        ButtonOK.Enabled = false;
    }

    /// <summary>GCertServerSet.pas:69 <c>procedure TfrmCertServerSet.Open;</c></summary>
    public void Open()
    {
        RefInfo();
        ShowModalEquivalent();
    }

    /// <summary>原文 <c>ShowModal</c>（测试不弹真实模态框）。</summary>
    public bool ShowModalEquivalent() => ShowModalHandler?.Invoke(this) ?? false;

    /// <summary>ShowModal 接缝（默认不弹窗）。</summary>
    public static Func<CertServerSetForm, bool>? ShowModalHandler;

    /// <summary>GCertServerSet.pas:75 <c>procedure TfrmCertServerSet.RefInfo;</c></summary>
    public void RefInfo()
    {
        m_boOpened = false;
        EditRunGate_Config_RegServerAddr.Text = GCertServerSetGlobals.g_sRunGate_Config_RegServerAddr;
        EditRunGate_Config_RegServerPort.Text = DelphiSystem.IntToStr(GCertServerSetGlobals.g_nRunGate_Config_RegServerPort);

        EditDBServer_Config_RegServerAddr.Text = GCertServerSetGlobals.g_sDBServer_Config_RegServerAddr;
        EditDBServer_Config_RegServerPort.Text = DelphiSystem.IntToStr(GCertServerSetGlobals.g_nDBServer_Config_RegServerPort);

        EditM2Server_Config_RegServerAddr.Text = GCertServerSetGlobals.g_sM2Server_Config_RegServerAddr;
        EditM2Server_Config_RegServerPort.Text = DelphiSystem.IntToStr(GCertServerSetGlobals.g_nM2Server_Config_RegServerPort);
        m_boOpened = true;
    }

    /// <summary>
    /// GCertServerSet.pas:91 <c>procedure TfrmCertServerSet.ButtonOKClick(Sender: TObject);</c>
    /// 返回 <c>true</c> 表示校验通过并写回全局量；<c>false</c> 表示中途 exit（原文 <c>exit</c>）。
    /// </summary>
    public bool ButtonOKClick(object? sender)
    {
        string sRunGate_Config_RegServerAddr;
        int nRunGate_Config_RegServerPort;
        string sDBServer_Config_RegServerAddr;
        int nDBServer_Config_RegServerPort;
        string sM2Server_Config_RegServerAddr;
        int nM2Server_Config_RegServerPort;

        sRunGate_Config_RegServerAddr = DelphiSystem.Trim(EditRunGate_Config_RegServerAddr.Text);
        // 原文 Str_ToInt(Edit..., -1)（HUtil32 版：仅当首字符为数字/±时才尝试解析）
        nRunGate_Config_RegServerPort = HUtil32.Str_ToInt(EditRunGate_Config_RegServerPort.Text, -1);
        sDBServer_Config_RegServerAddr = DelphiSystem.Trim(EditDBServer_Config_RegServerAddr.Text);
        nDBServer_Config_RegServerPort = HUtil32.Str_ToInt(EditDBServer_Config_RegServerPort.Text, -1);
        sM2Server_Config_RegServerAddr = DelphiSystem.Trim(EditM2Server_Config_RegServerAddr.Text);
        nM2Server_Config_RegServerPort = HUtil32.Str_ToInt(EditM2Server_Config_RegServerPort.Text, -1);

        if (!HUtil32.IsIPaddr(sRunGate_Config_RegServerAddr))
        {
            GameCenterDialogs.MessageBox("游戏网关验证服务器地址设置错误！！！", "错误信息",
                GameCenterDialogs.MB_OK | GameCenterDialogs.MB_ICONERROR);
            LastFocus = "EditRunGate_Config_RegServerAddr";
            EditRunGate_Config_RegServerAddr.Focus();
            return false;
        }
        if ((nRunGate_Config_RegServerPort < 0) || (nRunGate_Config_RegServerPort > 65535))
        {
            GameCenterDialogs.MessageBox("游戏网关验证服务器端口设置错误！！！", "错误信息",
                GameCenterDialogs.MB_OK | GameCenterDialogs.MB_ICONERROR);
            LastFocus = "EditRunGate_Config_RegServerPort";
            EditRunGate_Config_RegServerPort.Focus();
            return false;
        }

        if (!HUtil32.IsIPaddr(sDBServer_Config_RegServerAddr))
        {
            GameCenterDialogs.MessageBox("数据库验证服务器地址设置错误！！！", "错误信息",
                GameCenterDialogs.MB_OK | GameCenterDialogs.MB_ICONERROR);
            LastFocus = "EditDBServer_Config_RegServerAddr";
            EditDBServer_Config_RegServerAddr.Focus();
            return false;
        }
        if ((nDBServer_Config_RegServerPort < 0) || (nDBServer_Config_RegServerPort > 65535))
        {
            GameCenterDialogs.MessageBox("数据库验证服务器端口设置错误！！！", "错误信息",
                GameCenterDialogs.MB_OK | GameCenterDialogs.MB_ICONERROR);
            LastFocus = "EditDBServer_Config_RegServerPort";
            EditDBServer_Config_RegServerPort.Focus();
            return false;
        }

        // 原文如此（GCertServerSet.pas:129-138）：第三段重复校验 RunGate，M2Server 从不校验。
        if (!HUtil32.IsIPaddr(sRunGate_Config_RegServerAddr))
        {
            GameCenterDialogs.MessageBox("游戏网关验证服务器地址设置错误！！！", "错误信息",
                GameCenterDialogs.MB_OK | GameCenterDialogs.MB_ICONERROR);
            LastFocus = "EditRunGate_Config_RegServerAddr";
            EditRunGate_Config_RegServerAddr.Focus();
            return false;
        }
        if ((nRunGate_Config_RegServerPort < 0) || (nRunGate_Config_RegServerPort > 65535))
        {
            GameCenterDialogs.MessageBox("游戏网关验证服务器端口设置错误！！！", "错误信息",
                GameCenterDialogs.MB_OK | GameCenterDialogs.MB_ICONERROR);
            LastFocus = "EditRunGate_Config_RegServerPort";
            EditRunGate_Config_RegServerPort.Focus();
            return false;
        }

        GCertServerSetGlobals.g_sRunGate_Config_RegServerAddr = sRunGate_Config_RegServerAddr;
        GCertServerSetGlobals.g_nRunGate_Config_RegServerPort = nRunGate_Config_RegServerPort;
        GCertServerSetGlobals.g_sDBServer_Config_RegServerAddr = sDBServer_Config_RegServerAddr;
        GCertServerSetGlobals.g_nDBServer_Config_RegServerPort = nDBServer_Config_RegServerPort;
        GCertServerSetGlobals.g_sM2Server_Config_RegServerAddr = sM2Server_Config_RegServerAddr;
        GCertServerSetGlobals.g_nM2Server_Config_RegServerPort = nM2Server_Config_RegServerPort;

        uModValue();
        return true;
    }

    /// <summary>GCertServerSet.pas:149 <c>procedure TfrmCertServerSet.EditChange(Sender: TObject);</c></summary>
    public void EditChange(object? sender)
    {
        if (m_boOpened) ModValue();
    }

    /// <summary>测试辅助：读取 m_boModValued。</summary>
    public bool IsModValued => m_boModValued;

    /// <summary>测试辅助：读取 m_boOpened。</summary>
    public bool IsOpened => m_boOpened;
}

/// <summary>
/// GCertServerSet.pas 引用但 **GShare.pas 从未声明** 的 6 个全局量
/// （见 <see cref="CertServerSetForm"/> 类注释的缺陷登记）。原文缺省值不可知，
/// 这里按类型零值（""/0）声明并标注"原文未定义"。
/// </summary>
public static class GCertServerSetGlobals
{
    /// <summary>GCertServerSet.pas:78（原文未定义）。</summary>
    public static string g_sRunGate_Config_RegServerAddr = "";

    /// <summary>GCertServerSet.pas:79（原文未定义）。</summary>
    public static int g_nRunGate_Config_RegServerPort = 0;

    /// <summary>GCertServerSet.pas:81（原文未定义）。</summary>
    public static string g_sDBServer_Config_RegServerAddr = "";

    /// <summary>GCertServerSet.pas:82（原文未定义）。</summary>
    public static int g_nDBServer_Config_RegServerPort = 0;

    /// <summary>GCertServerSet.pas:84（原文未定义）。</summary>
    public static string g_sM2Server_Config_RegServerAddr = "";

    /// <summary>GCertServerSet.pas:85（原文未定义）。</summary>
    public static int g_nM2Server_Config_RegServerPort = 0;

    /// <summary>测试隔离。</summary>
    public static void ResetForTests()
    {
        g_sRunGate_Config_RegServerAddr = "";
        g_nRunGate_Config_RegServerPort = 0;
        g_sDBServer_Config_RegServerAddr = "";
        g_nDBServer_Config_RegServerPort = 0;
        g_sM2Server_Config_RegServerAddr = "";
        g_nM2Server_Config_RegServerPort = 0;
    }
}
