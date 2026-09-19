using System;
using System.Windows.Forms;
using GXX.Core.Rtl;

namespace GXX.DBServer;

// RouteManage.pas (1-141) → RouteManage.cs
// 网关路由表窗体：列出 g_RouteInfo（遇到第一条空路由即停），支持增/改/删。

/// <summary>RouteManage.pas 的非 UI 逻辑（可单测）。</summary>
public static class RouteManageLogic
{
    /// <summary>
    /// RouteManage.pas:48-74 `RefShowRoute`：
    ///   for i := Low(g_RouteInfo) to High(g_RouteInfo)：nGateCount = 0 时 **break**（不是 continue，尾部空路由不显示）；
    ///   行 = [Caption=IntToStr(i), SubItems: sSelGateIP, IntToStr(nGateCount), 拼接的游戏网关串]。
    ///   拼接串原文 `format('%s %s:%d ', [...])`：**以空格开头、以空格结尾**，首项为空串。
    /// 返回显示出来的路由条数（等价 `ListViewRoute.Items.Count`）。
    /// </summary>
    public static int RefShowRoute(IListViewSink sink)
    {
        int i, ii;
        TRouteInfo RouteInfo;
        string sGameGate;

        sink.Clear();
        for (i = 0; i <= DBShareSeam.g_RouteInfo.Length - 1; i++)
        {
            RouteInfo = DBShareSeam.g_RouteInfo[i];
            if (RouteInfo.nGateCount == 0) break;
            sGameGate = "";
            var subItems = new System.Collections.Generic.List<string>
            {
                RouteInfo.sSelGateIP,                       // ListItem.SubItems.Add(RouteInfo.sSelGateIP)
                DelphiRTL.IntToStr(RouteInfo.nGateCount)    // ListItem.SubItems.Add(IntToStr(RouteInfo.nGateCount))
            };
            for (ii = 0; ii <= RouteInfo.nGateCount - 1; ii++)
            {
                sGameGate = DelphiRTL.Format("%s %s:%d ", sGameGate, RouteInfo.sGameGateIP[ii], RouteInfo.nGameGatePort[ii]);
            }
            subItems.Add(sGameGate);                        // ListItem.SubItems.Add(sGameGate)
            sink.AddRow(DelphiRTL.IntToStr(i), RouteInfo, subItems.ToArray());   // ListItem.Data := RouteInfo
        }
        return sink.Count;
    }

    /// <summary>
    /// RouteManage.pas:87-106 `ButtonDeleteClick`：就地清零该路由。
    /// 原文如此：只清 nGateCount / sSelGateIP / sGameGateIP[0..7] / nGameGatePort[0..7] / RunGate2List，
    /// **不清** nGameGateDBPort[] 与 dwGameGateConnectTick[]（残留值保留）。
    /// </summary>
    public static void ButtonDelete(TRouteInfo RouteInfo)
    {
        RouteInfo.nGateCount = 0;
        RouteInfo.sSelGateIP = "";

        for (int ii = 0; ii <= RouteInfo.sGameGateIP.Length - 1; ii++)
        {
            RouteInfo.sGameGateIP[ii] = "";
            RouteInfo.nGameGatePort[ii] = 0;
        }
        RouteInfo.RunGate2List.Clear();
    }

    /// <summary>RouteManage.pas:123-139 `ButtonAddRouteClick`：条数达 20 时提示并返回 false。</summary>
    public static bool ButtonAddRoute(int listViewItemCount, out TRouteInfo RouteInfo)
    {
        RouteInfo = null;
        int nNulIdx = listViewItemCount;
        if (nNulIdx >= 20)
        {
            UiSeam.MessageBox("路由条数已经达到指定数量,不能再增加路由！！！", "提示信息", TMsgBox.MB_OK + TMsgBox.MB_ICONINFORMATION);
            return false;
        }
        RouteInfo = DBShareSeam.g_RouteInfo[nNulIdx];
        return true;
    }
}

/// <summary>RouteManage.pas:31 `var frmRouteManage: TfrmRouteManage;`。</summary>
public static class RouteManageGlobal
{
    public static FrmRouteManage frmRouteManage;
}

/// <summary>RouteManage.pas:10-28 `TfrmRouteManage`（DFM: RouteManage.dfm）。</summary>
public class FrmRouteManage : Form
{
    // DFM: frmRouteManage Left=494 Top=377 BorderIcons=[biSystemMenu] BorderStyle=bsSingle
    //      Caption='网关路由配置' ClientHeight=223 ClientWidth=481 Position=poMainFormCenter ShowHint=True
    public GroupBox GroupBox1;
    public ListView ListViewRoute;
    public Button ButtonEdit;
    public Button ButtonDelete;
    public Button ButtonOK;
    public Button ButtonAddRoute;

    public FrmRouteManage()
    {
        // DFM: frmRouteManage Left=494 Top=377 BorderStyle=bsSingle Caption='网关路由配置'
        //      ClientHeight=223 ClientWidth=481 Position=poMainFormCenter ShowHint=True
        Text = "网关路由配置";
        StartPosition = FormStartPosition.CenterParent;    // Position=poMainFormCenter
        Location = new System.Drawing.Point(494, 377);
        ClientSize = new System.Drawing.Size(481, 223);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;

        // DFM: GroupBox1 Left=8 Top=8 Width=465 Height=209 Caption='网关路由表' TabOrder=0
        GroupBox1 = new GroupBox { Left = 8, Top = 8, Width = 465, Height = 209, Text = "网关路由表", TabIndex = 0 };

        // DFM: ListViewRoute Left=8 Top=16 Width=449 Height=153
        //      Columns = 序号(40) / 角色网关(80) / 网关数量(60) / 游戏网关(1000)
        //      GridLines=True ReadOnly=True RowSelect=True TabOrder=0 ViewStyle=vsReport OnClick=ListViewRouteClick
        ListViewRoute = new ListView
        {
            Left = 8,
            Top = 16,
            Width = 449,
            Height = 153,
            GridLines = true,
            FullRowSelect = true,
            MultiSelect = false,
            View = View.Details,
            TabIndex = 0
        };
        ListViewRoute.Columns.Add("序号", 40);
        ListViewRoute.Columns.Add("角色网关", 80);
        ListViewRoute.Columns.Add("网关数量", 60);
        ListViewRoute.Columns.Add("游戏网关", 1000);

        // DFM: ButtonEdit Left=88 Top=176 Width=73 Height=25 Hint='修改选定的网关路由' Caption='编辑(&E)' TabOrder=1 OnClick=ButtonEditClick
        ButtonEdit = new Button { Left = 88, Top = 176, Width = 73, Height = 25, Text = "编辑(&E)", TabIndex = 1 };
        // DFM: ButtonDelete Left=168 Top=176 Width=73 Height=25 Hint='删除选定的网关路由' Caption='删除(&D)' TabOrder=2 OnClick=ButtonDeleteClick
        ButtonDelete = new Button { Left = 168, Top = 176, Width = 73, Height = 25, Text = "删除(&D)", TabIndex = 2 };
        // DFM: ButtonOK Left=384 Top=176 Width=73 Height=25 Hint='保存网关路由设置退出' Caption='确定(&O)' ModalResult=1 TabOrder=3
        ButtonOK = new Button { Left = 384, Top = 176, Width = 73, Height = 25, Text = "确定(&O)", TabIndex = 3, DialogResult = DialogResult.OK };
        // DFM: ButtonAddRoute Left=8 Top=176 Width=73 Height=25 Hint='修改选定的网关路由'（原文如此，与 ButtonEdit 同一提示）
        //      Caption='增加(&A)' TabOrder=4 OnClick=ButtonAddRouteClick
        ButtonAddRoute = new Button { Left = 8, Top = 176, Width = 73, Height = 25, Text = "增加(&A)", TabIndex = 4 };

        GroupBox1.Controls.Add(ListViewRoute);
        GroupBox1.Controls.Add(ButtonEdit);
        GroupBox1.Controls.Add(ButtonDelete);
        GroupBox1.Controls.Add(ButtonOK);
        GroupBox1.Controls.Add(ButtonAddRoute);
        Controls.Add(GroupBox1);

        ListViewRoute.SelectedIndexChanged += (s, e) => ListViewRouteClick(s, e);
        ListViewRoute.Click += (s, e) => ListViewRouteClick(s, e);
        ButtonDelete.Click += (s, e) => ButtonDeleteClick(s, e);
        ButtonEdit.Click += (s, e) => ButtonEditClick(s, e);
        ButtonAddRoute.Click += (s, e) => ButtonAddRouteClick(s, e);
    }

    /// <summary>RouteManage.pas:41-46 `Open`：RefShowRoute(); ShowModal。</summary>
    public void Open()
    {
        RefShowRoute();

        ShowDialog();
    }

    /// <summary>RouteManage.pas:48-74 `RefShowRoute`（另负责两个按钮的可用状态）。</summary>
    public void RefShowRoute()
    {
        var sink = new ListViewSink(ListViewRoute);
        RouteManageLogic.RefShowRoute(sink);
        ButtonEdit.Enabled = false;
        ButtonDelete.Enabled = false;
    }

    /// <summary>RouteManage.pas:77-85 `ListViewRouteClick`。</summary>
    public void ListViewRouteClick(object Sender, EventArgs e)
    {
        ListViewItem ListItem = ListViewRoute.SelectedItems.Count > 0 ? ListViewRoute.SelectedItems[0] : null;
        if (ListItem == null) return;
        ButtonEdit.Enabled = true;
        ButtonDelete.Enabled = true;
    }

    /// <summary>RouteManage.pas:87-106 `ButtonDeleteClick`。</summary>
    public void ButtonDeleteClick(object Sender, EventArgs e)
    {
        ListViewItem ListItem = ListViewRoute.SelectedItems.Count > 0 ? ListViewRoute.SelectedItems[0] : null;
        if (ListItem == null) return;
        TRouteInfo RouteInfo = (TRouteInfo)ListItem.Tag;
        RouteManageLogic.ButtonDelete(RouteInfo);
        RefShowRoute();
    }

    /// <summary>RouteManage.pas:109-121 `ButtonEditClick`。</summary>
    public void ButtonEditClick(object Sender, EventArgs e)
    {
        ListViewItem ListItem = ListViewRoute.SelectedItems.Count > 0 ? ListViewRoute.SelectedItems[0] : null;
        if (ListItem == null) return;
        TRouteInfo RouteInfo = (TRouteInfo)ListItem.Tag;
        if (RouteEditUnit.ShowFrmRouteEdit(RouteInfo, true))
        {
            RefShowRoute();
        }
    }

    /// <summary>RouteManage.pas:123-139 `ButtonAddRouteClick`。</summary>
    public void ButtonAddRouteClick(object Sender, EventArgs e)
    {
        TRouteInfo RouteInfo;
        if (!RouteManageLogic.ButtonAddRoute(ListViewRoute.Items.Count, out RouteInfo)) return;
        if (RouteEditUnit.ShowFrmRouteEdit(RouteInfo, false))
        {
            RefShowRoute();
        }
    }
}
