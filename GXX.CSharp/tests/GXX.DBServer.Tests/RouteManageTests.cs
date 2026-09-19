using System;
using GXX.Core.Rtl;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>RouteManage.pas:1-141（网关路由表）逻辑与窗体测试。</summary>
public class RouteManageTests : TempDirTest
{
    private static TRouteInfo AddRoute(int idx, string selGate, params (string ip, int port)[] gates)
    {
        var r = DBShareSeam.g_RouteInfo[idx];
        r.sSelGateIP = selGate;
        r.nGateCount = gates.Length;
        for (int i = 0; i < gates.Length; i++)
        {
            r.sGameGateIP[i] = gates[i].ip;
            r.nGameGatePort[i] = gates[i].port;
        }
        return r;
    }

    [Fact]
    public void RefShowRoute_标题为索引_子项为角色网关数量与拼接串()
    {
        AddRoute(0, "127.0.0.1", ("10.0.0.1", 7200), ("10.0.0.2", 7201));
        AddRoute(1, "127.0.0.2", ("10.0.0.3", 7300));

        var sink = new MemoryListViewSink();
        int count = RouteManageLogic.RefShowRoute(sink);

        Assert.Equal(2, count);
        Assert.Equal("0", sink.Rows[0].Caption);
        Assert.Equal("127.0.0.1", sink.Rows[0].SubItems[0]);
        Assert.Equal("2", sink.Rows[0].SubItems[1]);
        // format('%s %s:%d ', ...) → 首项空串，故以空格开头、项间两个空格、结尾一个空格
        Assert.Equal(" 10.0.0.1:7200  10.0.0.2:7201 ", sink.Rows[0].SubItems[2]);
        Assert.Same(DBShareSeam.g_RouteInfo[0], sink.Rows[0].Tag);

        Assert.Equal("1", sink.Rows[1].Caption);
        Assert.Equal(" 10.0.0.3:7300 ", sink.Rows[1].SubItems[2]);
    }

    [Fact]
    public void RefShowRoute_遇到第一条空路由即break_后面的路由不显示_原文如此()
    {
        AddRoute(0, "127.0.0.1", ("10.0.0.1", 7200));
        AddRoute(2, "127.0.0.3", ("10.0.0.3", 7300));   // 索引 1 为空 → break

        var sink = new MemoryListViewSink();
        int count = RouteManageLogic.RefShowRoute(sink);

        Assert.Equal(1, count);
    }

    [Fact]
    public void RefShowRoute_先清空列表()
    {
        AddRoute(0, "127.0.0.1", ("10.0.0.1", 7200));
        var sink = new MemoryListViewSink();
        sink.AddRow("stale", null, "x");

        RouteManageLogic.RefShowRoute(sink);

        Assert.Equal(1, sink.Count);
        Assert.Equal("0", sink.Rows[0].Caption);
    }

    [Fact]
    public void RefShowRoute_无路由时返回0行()
    {
        var sink = new MemoryListViewSink();
        Assert.Equal(0, RouteManageLogic.RefShowRoute(sink));
    }

    [Fact]
    public void ButtonDelete_清网关与备用列表但保留DBPort与连接时钟_原文如此()
    {
        var r = AddRoute(0, "127.0.0.1", ("10.0.0.1", 7200), ("10.0.0.2", 7201));
        r.nGameGateDBPort[0] = 7300;
        r.nGameGateDBPort[1] = 7301;
        r.dwGameGateConnectTick[0] = 12345;
        r.RunGate2List.Add(1, "10.0.0.9", 8000, 8100, 5);

        RouteManageLogic.ButtonDelete(r);

        Assert.Equal(0, r.nGateCount);
        Assert.Equal("", r.sSelGateIP.Value);
        Assert.Equal("", r.sGameGateIP[0].Value);
        Assert.Equal(0, r.nGameGatePort[0]);
        Assert.Equal(0, r.RunGate2List.Count);
        // 原文缺陷：DBPort 与 dwGameGateConnectTick 未清
        Assert.Equal(7300, r.nGameGateDBPort[0]);
        Assert.Equal(7301, r.nGameGateDBPort[1]);
        Assert.Equal(12345u, r.dwGameGateConnectTick[0]);
    }

    [Fact]
    public void ButtonAddRoute_达到20条时提示并拒绝()
    {
        using var ui = new UiRecorder();
        bool ok = RouteManageLogic.ButtonAddRoute(20, out TRouteInfo r);

        Assert.False(ok);
        Assert.Null(r);
        Assert.Equal("路由条数已经达到指定数量,不能再增加路由！！！", ui.MessageBoxes[0].Text);
        Assert.Equal("提示信息", ui.MessageBoxes[0].Caption);
        Assert.Equal(TMsgBox.MB_OK + TMsgBox.MB_ICONINFORMATION, ui.MessageBoxes[0].Flags);
    }

    [Fact]
    public void ButtonAddRoute_按列表条数取下一个空槽位()
    {
        using var ui = new UiRecorder();
        bool ok = RouteManageLogic.ButtonAddRoute(3, out TRouteInfo r);

        Assert.True(ok);
        Assert.Same(DBShareSeam.g_RouteInfo[3], r);
        Assert.Empty(ui.MessageBoxes);
    }

    [Fact]
    public void ButtonAddRoute_第19条仍可增加第20条()
    {
        using var ui = new UiRecorder();
        Assert.True(RouteManageLogic.ButtonAddRoute(19, out TRouteInfo r));
        Assert.Same(DBShareSeam.g_RouteInfo[19], r);
    }

    [Fact]
    public void 窗体RefShowRoute_重建行并禁用编辑删除按钮()
    {
        AddRoute(0, "127.0.0.1", ("10.0.0.1", 7200));

        using var form = new FrmRouteManage();
        form.ButtonEdit.Enabled = true;
        form.ButtonDelete.Enabled = true;

        form.RefShowRoute();

        Assert.Equal(1, form.ListViewRoute.Items.Count);
        Assert.Equal("0", form.ListViewRoute.Items[0].Text);   // Caption
        Assert.Equal("127.0.0.1", form.ListViewRoute.Items[0].SubItems[1].Text);   // SubItems[0] = Caption
        Assert.False(form.ButtonEdit.Enabled);
        Assert.False(form.ButtonDelete.Enabled);
    }

    [Fact]
    public void 窗体ButtonEditClick_走ShowFrmRouteEdit接缝_返回真则刷新()
    {
        AddRoute(0, "127.0.0.1", ("10.0.0.1", 7200));
        using var form = new FrmRouteManage();
        form.RefShowRoute();

        TRouteInfo passed = null;
        bool isEdit = false;
        Func<TRouteInfo, bool, bool> saved = RouteEditUnit.ShowFrmRouteEdit;
        try
        {
            RouteEditUnit.ShowFrmRouteEdit = (r, edit) => { passed = r; isEdit = edit; return true; };
            form.ButtonEditClick(null, EventArgs.Empty);
            // 未选中行时不触发（原文 `if ListItem = nil then Exit`）
            Assert.Null(passed);

            form.ListViewRoute.Items[0].Selected = true;
            form.ListViewRoute.Select();
            form.ButtonEditClick(null, EventArgs.Empty);
        }
        finally
        {
            RouteEditUnit.ShowFrmRouteEdit = saved;
        }
    }

    [Fact]
    public void 窗体DFM_控件几何与列头宽度对齐()
    {
        using var form = new FrmRouteManage();
        Assert.Equal("网关路由配置", form.Text);
        Assert.Equal(new System.Drawing.Size(481, 223), form.ClientSize);
        Assert.Equal("网关路由表", form.GroupBox1.Text);
        Assert.Equal(new System.Drawing.Point(8, 16), form.ListViewRoute.Location);
        Assert.Equal(4, form.ListViewRoute.Columns.Count);
        Assert.Equal("序号", form.ListViewRoute.Columns[0].Text);
        Assert.Equal(40, form.ListViewRoute.Columns[0].Width);
        Assert.Equal("游戏网关", form.ListViewRoute.Columns[3].Text);
        Assert.Equal(1000, form.ListViewRoute.Columns[3].Width);
        Assert.Equal("编辑(&E)", form.ButtonEdit.Text);
        Assert.Equal("删除(&D)", form.ButtonDelete.Text);
        Assert.Equal("增加(&A)", form.ButtonAddRoute.Text);
        Assert.Equal("确定(&O)", form.ButtonOK.Text);
        Assert.Equal(System.Windows.Forms.DialogResult.OK, form.ButtonOK.DialogResult);
        Assert.Equal(new System.Drawing.Point(8, 176), form.ButtonAddRoute.Location);
    }

    [Fact]
    public void GateRouteIP_配合路由表使用_随机分配()
    {
        AddRoute(0, "127.0.0.1", ("10.0.0.1", 7200), ("10.0.0.2", 7201));
        DelphiRandom.Next = _ => 1;

        Assert.Equal("10.0.0.2", DBShareSeam.GateRouteIP("127.0.0.1", out int port));
        Assert.Equal(7201, port);
    }
}
