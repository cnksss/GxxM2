// ============================================================================
// GrobalSession.pas（93 行）→ Forms/GrobalSession.cs
// 单元：LoginSrv/GrobalSession.pas；DFM：Source/LoginSrv/GrobalSession.dfm（**文本 DFM**，GBK）
//
// DFM 对账（Objects=4 / Events=1）：
//   object frmGrobalSession: TfrmGrobalSession        ← 窗体根（Caption='查看全局会话'）
//     object ButtonRefGrid: TButton                   ← 无 ONCLICK！★ 原文缺陷（见下）
//     object PanelStatus: TPanel                      ← GridSession 的父容器
//       object GridSession: TStringGrid               ← ColCount=6
//   events: OnCreate = FormCreate                      ← 唯一 1 条绑定
//
// ★★ 原文缺陷（照抄 + 断言锁死）：
//   1. `ButtonRefGrid`（'刷新(&R)'）在 DFM 里**没有** `OnClick`，`.pas` 里也**没有**任何
//      Click 处理器（全单元 procedure 清单实测：FormCreate/RefGridSession/Open 三条）⇒
//      **刷新按钮永久失效**，用户点它什么都不会发生。用"DFM 绑定数 vs 托管 += 数"计数取证
//      （§37.3 否定性断言必须计数取证），见 P10GrobalSessionTests。
//   2. `PanelStatus.Caption := '正在取得数据...'` 在 `RefGridSession` 里**只设不复位** ⇒
//      取完数据后状态栏**永远停在"正在取得数据..."**。
//   3. `GridSession.FixedRows := 1` 只在"会话数为 0"分支里写；非空分支不写（靠 DFM 默认值 1）。
//
// 命名空间：本车道 `Forms/**` 分区下的窗体统一放 `GXX.LoginSrv.Forms`
// （与既有 `GXX.M2Server.Forms` 的约定一致，见 p9-m2-forms 车道的 Sweep9/Forms）。
// 理由（D-P10-15）：根命名空间 `GXX.LoginSrv` 里已有/将有可能同名的接缝类型
// （实测：`LoginSrvShare.TMsgServerInfo` 与 MasSock.pas 的同名记录**不同**；
//   且 LSShare.pas 的 `TConnInfo` 一旦落地就会与本文件的 `TConnInfo` 撞名）。
// 落在子命名空间后，外层命名空间成员**先于** using 被解析 ⇒ 结构上不可能撞名。
// ============================================================================

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GXX.LoginSrv.Forms;

/// <summary>
/// 接缝：LSShare.pas:53-64 `TConnInfo`（本窗体实际只读 5 个字段：
/// `sAccount` / `sIPaddr` / `sServerName` / `nSessionID` / `boPayCost`）。
/// <para>
/// 托管侧 `GXX.LoginSrv/LoginSrvShare.cs`（**本车道分区之外，不得修改**）目前只有
/// `TConfig`（LSShare.pas:133-275）而**没有** `TConnInfo`，也没有 `TConfig.SessionList`。
/// 故本区按窗体读取面声明最小等价类型；LSShare 整单元落地后**必须去重**（B-P10-04）。
/// </para>
/// </summary>
public sealed class TConnInfo
{
    public string sAccount = "";        // LSShare.pas:54
    public string sIPaddr = "";         // :55
    public string sServerName = "";     // :56
    public int nSessionID;              // :57
    public bool boPayCost;              // :58
}

/// <summary>
/// 接缝宿主：原文 `Config := @g_Config; ... Config.SessionList`（LSShare.pas:206
/// `SessionList: TGList`）。
/// <para>
/// ⚠ 台账 §25.2：接缝的默认实现**不得**静默返回中性值（否则"字段语义错"会被伪装成
/// "分支没命中"）。故 <see cref="RequireSessionList"/> 在未接线时**显式抛**。
/// </para>
/// </summary>
public static class GrobalSessionHost
{
    /// <summary>
    /// 宿主注入点：`g_Config.SessionList`（托管类型复用既有
    /// <see cref="GXX.Core.Protocol.SDK.TGList"/>，其 `Lock`/`UnLock`/`Count`/索引器与原文同形）。
    /// </summary>
    public static GXX.Core.Protocol.SDK.TGList? SessionList;

    /// <summary>未接线即抛（不返回空表）。</summary>
    public static GXX.Core.Protocol.SDK.TGList RequireSessionList =>
        SessionList ?? throw new InvalidOperationException(
            "未接线：g_Config.SessionList（LSShare.pas:206）尚未注入（见报告 B-P10-04）。");
}

/// <summary>
/// GrobalSession.pas `TfrmGrobalSession` 1:1（全局会话查看窗体）。
/// </summary>
public sealed class TfrmGrobalSession : Form
{
    // ---- DFM 控件（名称与 DFM 1:1） ----
    public Button ButtonRefGrid = null!;
    public Panel PanelStatus = null!;
    public DataGridView GridSession = null!;

    /// <summary>原文 `ShowModal` 接缝（默认不弹窗，避免无头环境挂死）。</summary>
    public static Func<TfrmGrobalSession, bool>? ShowModalHandler;

    public TfrmGrobalSession()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        // DFM 窗体属性
        Text = "查看全局会话";                                     // Caption = #26597#30475#20840#23616#20250#35805
        ClientSize = new System.Drawing.Size(405, 208);
        StartPosition = FormStartPosition.CenterParent;             // Position = poMainFormCenter
        FormBorderStyle = FormBorderStyle.FixedSingle;              // BorderStyle = bsSingle
        MaximizeBox = false;                                        // BorderIcons = [biSystemMenu, biMinimize]
        MinimizeBox = true;

        // object ButtonRefGrid: TButton（Left=8 Top=176 Width=73 Height=25 Caption='刷新(&R)'）
        // ★ 原文缺陷：DFM 无 OnClick ⇒ 这里**故意不绑任何事件**（见文件头 §1）
        ButtonRefGrid = new Button
        {
            Name = "ButtonRefGrid",
            Left = 8,
            Top = 176,
            Width = 73,
            Height = 25,
            Text = "刷新(&R)",
            TabIndex = 0,
        };

        // object PanelStatus: TPanel（Left=4 Top=8 Width=393 Height=161）
        PanelStatus = new Panel
        {
            Name = "PanelStatus",
            Left = 4,
            Top = 8,
            Width = 393,
            Height = 161,
            TabIndex = 1,
        };

        // object GridSession: TStringGrid（ColCount=6, DefaultRowHeight=18, FixedCols=0,
        //   Options 含 goEditing/goRangeSelect —— 原文代码从不写回网格，托管侧保留可编辑）
        // 托管等价：DataGridView，并把列头关掉（TStringGrid 的"表头"是**固定行** 0，不是列头）
        GridSession = new DataGridView
        {
            Name = "GridSession",
            Left = 0,
            Top = 0,
            Width = 393,
            Height = 161,
            TabIndex = 0,
            AllowUserToAddRows = false,
            ColumnHeadersVisible = false,       // TStringGrid 的表头是"固定行"，不是列头
            RowHeadersVisible = false,
            RowTemplate = { Height = 18 },      // DefaultRowHeight = 18
        };
        for (int i = 0; i < 6; i++)
            GridSession.Columns.Add("col" + i, "");
        GridSession.RowCount = 1;               // VCL TStringGrid 默认 RowCount=1

        PanelStatus.Controls.Add(GridSession);
        Controls.Add(ButtonRefGrid);
        Controls.Add(PanelStatus);

        // DFM: OnCreate = FormCreate
        // 本工程既有窗体统一约定（见 GXX.DBServer/AddrEdit.cs:272、Ranking.cs:397、
        // RouteEdit.cs:683 等）：`OnCreate` → `Load += (s, e) => FormCreate(s, e);`
        Load += (s, e) => FormCreate(s);
    }

    /// <summary>
    /// VCL `TStringGrid.FixedRows`（DFM 未写 ⇒ VCL 默认 1）。
    /// 托管 `DataGridView` 无对应属性，故以同名字段承载，供对账与断言（D-P10-13）。
    /// </summary>
    public int FixedRows = 1;

    /// <summary>
    /// VCL `TControl.Visible` 的**属性值**。
    /// <para>
    /// WinForms 的 `Control.Visible` getter 返回的是**有效可见性**（父级不可见时恒为 false），
    /// 而 VCL 返回的是写入值 ⇒ 无头环境（窗体未 Show）下用 `GridSession.Visible` 读回永远是
    /// false，无法复刻原文 `GridSession.Visible := False/True` 的语义。故单独承载该属性值；
    /// 写入时**同时**设置 `GridSession.Visible`，生产行为不变（D-P10-13）。
    /// </para>
    /// </summary>
    public bool GridSessionVisible { get; private set; } = true;

    private void SetGridSessionVisible(bool Value)
    {
        GridSessionVisible = Value;
        GridSession.Visible = Value;
    }

    /// <summary>
    /// VCL `TStringGrid.RowCount`（= DataGridView.RowCount）。
    /// </summary>
    public int RowCount
    {
        get => GridSession.RowCount;
        set => GridSession.RowCount = value;
    }

    /// <summary>
    /// VCL `TStringGrid.Cells[Col, Row]` 读。
    /// <para>
    /// ⚠ 越界口径（D-P10-14）：原文首行清空写的是 `Cells[x, 1]`，而 DFM **没有** `RowCount`
    /// （VCL 默认 1 行）⇒ 首次调用时第 1 行并不存在。VCL `TStringGrid` 的
    /// `GetCells/SetCells` 对越界下标是**静默忽略**（TStringGrid 的稀疏行实现不抛），
    /// 托管若 1:1 直译成 `Rows[Row]` 会抛 `ArgumentOutOfRangeException`。
    /// 故此处按 VCL 口径"越界即忽略/空串"实现（不是新增语义）。
    /// </para>
    /// </summary>
    public string Cells(int Col, int Row)
    {
        if (Col < 0 || Col >= GridSession.Columns.Count) return "";
        if (Row < 0 || Row >= GridSession.RowCount) return "";
        return GridSession.Rows[Row].Cells[Col].Value as string ?? "";
    }

    /// <summary>VCL `TStringGrid.Cells[Col, Row] := Value` 写（越界静默忽略，见 <see cref="Cells"/>）。</summary>
    public void SetCells(int Col, int Row, string Value)
    {
        if (Col < 0 || Col >= GridSession.Columns.Count) return;
        if (Row < 0 || Row >= GridSession.RowCount) return;
        GridSession.Rows[Row].Cells[Col].Value = Value;
    }

    /// <summary>GrobalSession.pas:34-42 `procedure TfrmGrobalSession.FormCreate(Sender: TObject);`</summary>
    public void FormCreate(object? Sender)
    {
        SetCells(0, 0, "序号");
        SetCells(1, 0, "登录帐号");
        SetCells(2, 0, "登录地址");
        SetCells(3, 0, "服务器名");
        SetCells(4, 0, "会话ID");
        SetCells(5, 0, "是否充值");
    }

    /// <summary>GrobalSession.pas:44-48 `procedure TfrmGrobalSession.Open;`</summary>
    public void Open()
    {
        RefGridSession();
        SelfShowModal();
    }

    /// <summary>原文 `Self.ShowModal`。</summary>
    public bool SelfShowModal() => ShowModalHandler?.Invoke(this) ?? false;

    /// <summary>
    /// GrobalSession.pas:50-90 `procedure TfrmGrobalSession.RefGridSession;`（原文 private）
    /// </summary>
    private void RefGridSession()
    {
        int I;

        // 原文 Config := @g_Config;（本窗体只用到 Config.SessionList）
        GXX.Core.Protocol.SDK.TGList Config_SessionList = GrobalSessionHost.RequireSessionList;

        // ★ 原文如此：这一行**没有**复位点 ⇒ 取完数据后状态栏仍显示"正在取得数据..."
        PanelStatus.Text = "正在取得数据...";
        SetGridSessionVisible(false);
        SetCells(0, 1, "");
        SetCells(1, 1, "");
        SetCells(2, 1, "");
        SetCells(3, 1, "");
        SetCells(4, 1, "");
        SetCells(5, 1, "");

        Config_SessionList.Lock();
        try
        {
            if (Config_SessionList.Count <= 0)
            {
                RowCount = 2;
                FixedRows = 1;
            }
            else
            {
                RowCount = Config_SessionList.Count + 1;
            }

            for (I = 0; I <= Config_SessionList.Count - 1; I++)
            {
                TConnInfo ConnInfo = (TConnInfo)Config_SessionList[I];
                SetCells(0, I + 1, I.ToString());                          // IntToStr(I)
                SetCells(1, I + 1, ConnInfo.sAccount);
                SetCells(2, I + 1, ConnInfo.sIPaddr);
                SetCells(3, I + 1, ConnInfo.sServerName);
                SetCells(4, I + 1, ConnInfo.nSessionID.ToString());        // IntToStr(ConnInfo.nSessionID)
                SetCells(5, I + 1, GXX.Core.Util.HUtil32.BoolToCStr(ConnInfo.boPayCost));
            }
        }
        finally
        {
            Config_SessionList.UnLock();
        }

        SetGridSessionVisible(true);
    }
}
