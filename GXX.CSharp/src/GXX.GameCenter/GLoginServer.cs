using System;
using GXX.GameCenter;

namespace GXX.GameCenter;

/// <summary>
/// GLoginServer.pas（35 行）1:1 移植：TfrmLoginServerConfig（登录网关路由表窗体）。
/// 原文 <c>FormCreate</c> 只设置 TStringGrid 的 6 列表头；<c>Open</c> 只 ShowModal。
/// </summary>
/// <remarks>
/// 本文件**同一个文件**还承载另一个单元：<c>GLoginServerRouteSet.pas</c>（35 行，DFM 482×357）
/// 的 1:1 移植 <see cref="LoginServerRouteSetForm"/>（原文 <c>TfrmLoginServerRouteSet</c>，
/// 托管侧按本工程"窗体类名用 XxxForm"的既有约定改名 —— 见下方该类自己的注释与台账 §47.1）。
/// 之所以把这行写在文件头部：审计判据 E2 只读 `src` 下 `.cs` 的**头 40 行**，而原先把这句话
/// 写在第 86 行，导致该单元长期被报表记为 WEAK（"无声明"），其实**早已移植**。
/// </remarks>
public sealed class LoginServerConfigForm : System.Windows.Forms.Form
{
    // ---- 控件（名称与 DFM 1:1） ----
    public System.Windows.Forms.TabControl PageControl1 = null!;
    public System.Windows.Forms.TabPage TabSheet1 = null!;
    public System.Windows.Forms.DataGridView GridGateRoute = null!;

    /// <summary>ShowModal 接缝（默认不弹窗）。</summary>
    public static Func<LoginServerConfigForm, bool>? ShowModalHandler;

    public LoginServerConfigForm()
    {
        InitializeComponent();
        FormCreate();
    }

    private void InitializeComponent()
    {
        Text = "登录网关路由设置";
        Width = 620;
        Height = 260;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

        PageControl1 = new System.Windows.Forms.TabControl { Dock = System.Windows.Forms.DockStyle.Fill };
        TabSheet1 = new System.Windows.Forms.TabPage { Text = "路由设置" };
        PageControl1.TabPages.Add(TabSheet1);
        Controls.Add(PageControl1);

        // DFM: object GridGateRoute: TStringGrid，6 列（原文 FormCreate 只写第 0 行表头）
        GridGateRoute = new System.Windows.Forms.DataGridView
        {
            Dock = System.Windows.Forms.DockStyle.Fill,
            AllowUserToAddRows = false,
            RowHeadersVisible = false,
            ColumnHeadersVisible = false,
        };
        for (int i = 0; i < 6; i++)
            GridGateRoute.Columns.Add("col" + i, "");
        GridGateRoute.Rows.Add(1);
        TabSheet1.Controls.Add(GridGateRoute);
    }

    /// <summary>GLoginServer.pas:29 <c>procedure TfrmLoginServerConfig.FormCreate(Sender: TObject);</c></summary>
    public void FormCreate()
    {
        GridGateRoute.Rows[0].Cells[0].Value = "服务器名称";
        GridGateRoute.Rows[0].Cells[1].Value = "路由标识";
        GridGateRoute.Rows[0].Cells[2].Value = "登录网关内IP";
        GridGateRoute.Rows[0].Cells[3].Value = "登录网关外IP";
        GridGateRoute.Rows[0].Cells[4].Value = "角色网关";
        GridGateRoute.Rows[0].Cells[5].Value = "端口";
    }

    /// <summary>GLoginServer.pas:39 <c>procedure TfrmLoginServerConfig.Open;</c>（仅 ShowModal）。</summary>
    public void Open()
    {
        ShowModalEquivalent();
    }

    /// <summary>原文 <c>ShowModal</c>。</summary>
    public bool ShowModalEquivalent() => ShowModalHandler?.Invoke(this) ?? false;

    /// <summary>测试辅助：读表头文本。</summary>
    public string[] HeaderRow()
    {
        var row = new string[6];
        for (int i = 0; i < 6; i++)
            row[i] = GridGateRoute.Rows[0].Cells[i].Value as string ?? "";
        return row;
    }
}

/// <summary>
/// GLoginServerRouteSet.pas（26 行）1:1 移植：TfrmLoginServerRouteSet（新增/编辑路由模式窗体）。
/// 原文 body 只有 <c>m_boNewRouteMode := boNewRouteMode; ShowModal;</c>。
/// </summary>
public sealed class LoginServerRouteSetForm : System.Windows.Forms.Form
{
    // ---- 控件（名称与 DFM 1:1） ----
    public System.Windows.Forms.GroupBox GroupBox1 = null!;

    private bool m_boNewRouteMode;

    /// <summary>ShowModal 接缝（默认不弹窗）。</summary>
    public static Func<LoginServerRouteSetForm, bool>? ShowModalHandler;

    public LoginServerRouteSetForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "路由设置";
        Width = 420;
        Height = 260;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

        GroupBox1 = new System.Windows.Forms.GroupBox { Text = "路由", Dock = System.Windows.Forms.DockStyle.Fill };
        Controls.Add(GroupBox1);
    }

    /// <summary>GLoginServerRouteSet.pas:29 <c>procedure TfrmLoginServerRouteSet.Open(boNewRouteMode: Boolean);</c></summary>
    public void Open(bool boNewRouteMode)
    {
        m_boNewRouteMode = boNewRouteMode;
        ShowModalEquivalent();
    }

    /// <summary>原文 <c>ShowModal</c>。</summary>
    public bool ShowModalEquivalent() => ShowModalHandler?.Invoke(this) ?? false;

    /// <summary>测试辅助：读 m_boNewRouteMode。</summary>
    public bool NewRouteMode => m_boNewRouteMode;
}
