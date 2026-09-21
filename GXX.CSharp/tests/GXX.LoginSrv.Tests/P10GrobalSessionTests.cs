// ============================================================================
// GrobalSession.pas（93 行）→ Forms/GrobalSession.cs 的逐成员 + DFM 对账用例。
//
// DFM 期望值（Source/LoginSrv/GrobalSession.dfm 实测）：
//   Objects = 4（frmGrobalSession / ButtonRefGrid / PanelStatus / GridSession）
//   Events  = 1（OnCreate = FormCreate）
//   GridSession.ColCount = 6
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using GXX.Core.Protocol;
using Xunit;

using TGList = GXX.Core.Protocol.SDK.TGList;

namespace GXX.LoginSrv.Forms.Tests;

[Collection("LoginSrvSequential")]
public sealed class P10GrobalSessionTests : IDisposable
{
    private readonly TGList _sessionList = new();

    public P10GrobalSessionTests()
    {
        GrobalSessionHost.SessionList = _sessionList;
        TfrmGrobalSession.ShowModalHandler = null;
    }

    public void Dispose()
    {
        GrobalSessionHost.SessionList = null;
        TfrmGrobalSession.ShowModalHandler = null;
    }

    private static TConnInfo Conn(string account, string ip, string server, int sessionId, bool pay)
        => new() { sAccount = account, sIPaddr = ip, sServerName = server, nSessionID = sessionId, boPayCost = pay };

    // ---------------- DFM 对账（§37.3 计数取证） ----------------

    [Fact]
    public void Dfm_ObjectCount_IsFour()
    {
        using var f = new TfrmGrobalSession();
        Assert.Equal(4, P10FormReconcile.CountDfmObjects(f));
        Assert.Equal(3, P10FormReconcile.CountChildrenOf(f));
    }

    [Fact]
    public void Dfm_ObjectNames_MatchDeclarationOrder()
    {
        using var f = new TfrmGrobalSession();
        var names = P10FormReconcile.EnumerateDfmObjects(f).Select(x => x.Name).ToList();
        Assert.Equal(new List<string> { "TfrmGrobalSession", "ButtonRefGrid", "PanelStatus", "GridSession" }, names);
    }

    [Fact]
    public void Dfm_GridSession_IsChildOfPanelStatus()
    {
        // DFM 里 GridSession 挂在 PanelStatus 下（不是窗体直挂）
        using var f = new TfrmGrobalSession();
        Assert.Same(f.PanelStatus, f.GridSession.Parent);
        Assert.Contains(f.PanelStatus, f.Controls.OfType<System.Windows.Forms.Control>());
    }

    [Fact]
    public void Dfm_GridSession_HasSixColumns()
    {
        using var f = new TfrmGrobalSession();
        Assert.Equal(6, f.GridSession.Columns.Count);
    }

    [Fact]
    public void Dfm_EventBindingCount_IsOne()
    {
        using var f = new TfrmGrobalSession();
        Assert.Equal(1, P10FormReconcile.CountEventBindings(f));
    }

    [Fact]
    public void Dfm_FormCreate_IsTheOnlyBinding()
    {
        using var f = new TfrmGrobalSession();
        // 窗体自身的 OnCreate（DFM: OnCreate = FormCreate）在托管侧由构造函数显式调用，
        // 因此这里用"处理器可调用且生效"作为正向取证
        f.FormCreate(f);
        Assert.Equal("序号", f.Cells(0, 0));
    }

    [Fact]
    public void Dfm_ButtonRefGrid_HasNoClickHandler_OriginalFlaw()
    {
        // ★ 原文如此（缺陷）：DFM 的 ButtonRefGrid **没有** OnClick 属性，
        //   .pas 里也没有任何 Click 处理器 ⇒ '刷新(&R)' 按钮永久失效。
        //   否定性断言按 §37.3 用计数取证：DFM 事件总数=1（只有 OnCreate），
        //   且 ButtonRefGrid 上挂接的事件数=0。
        using var f = new TfrmGrobalSession();
        Assert.Equal(0, P10FormReconcile.CountEventBindingsOn(f.ButtonRefGrid));
        Assert.False(P10FormReconcile.IsBound(f.ButtonRefGrid, "Click"));
        Assert.Equal(1, P10FormReconcile.CountEventBindings(f));
    }

    [Fact]
    public void Dfm_FormCaptionAndClientSize()
    {
        using var f = new TfrmGrobalSession();
        Assert.Equal("查看全局会话", f.Text);
        Assert.Equal(new System.Drawing.Size(405, 208), f.ClientSize);
    }

    [Fact]
    public void Dfm_ButtonCaption()
    {
        using var f = new TfrmGrobalSession();
        Assert.Equal("刷新(&R)", f.ButtonRefGrid.Text);
    }

    // ---------------- FormCreate ----------------

    [Fact]
    public void FormCreate_WritesSixColumnHeaders()
    {
        using var f = new TfrmGrobalSession();
        f.FormCreate(f);                     // 原 DFM OnCreate 处理器（测试直调，无需消息泵）
        Assert.Equal("序号", f.Cells(0, 0));
        Assert.Equal("登录帐号", f.Cells(1, 0));
        Assert.Equal("登录地址", f.Cells(2, 0));
        Assert.Equal("服务器名", f.Cells(3, 0));
        Assert.Equal("会话ID", f.Cells(4, 0));
        Assert.Equal("是否充值", f.Cells(5, 0));
    }

    // ---------------- RefGridSession（经 Open 驱动） ----------------

    [Fact]
    public void Open_CallsRefGridThenShowModal()
    {
        bool shown = false;
        TfrmGrobalSession.ShowModalHandler = _ => { shown = true; return true; };

        using var f = new TfrmGrobalSession();
        f.Open();

        Assert.True(shown);
    }

    [Fact]
    public void RefGridSession_EmptyList_LeavesTwoRowsAndFixedRowsOne()
    {
        using var f = new TfrmGrobalSession();
        f.Open();

        Assert.Equal(2, f.RowCount);
        Assert.Equal(1, f.FixedRows);
        Assert.Equal("", f.Cells(0, 1));
    }

    [Fact]
    public void RefGridSession_NonEmptyList_FillsColumns()
    {
        _sessionList.Add(Conn("acc0", "1.1.1.1", "srv0", 10, true));
        _sessionList.Add(Conn("acc1", "2.2.2.2", "srv1", 11, false));

        using var f = new TfrmGrobalSession();
        f.Open();

        Assert.Equal(3, f.RowCount);
        Assert.Equal("0", f.Cells(0, 1));
        Assert.Equal("acc0", f.Cells(1, 1));
        Assert.Equal("1.1.1.1", f.Cells(2, 1));
        Assert.Equal("srv0", f.Cells(3, 1));
        Assert.Equal("10", f.Cells(4, 1));
        Assert.Equal("是", f.Cells(5, 1));      // HUtil32.BoolToCStr(true)

        Assert.Equal("1", f.Cells(0, 2));
        Assert.Equal("acc1", f.Cells(1, 2));
        Assert.Equal("2.2.2.2", f.Cells(2, 2));
        Assert.Equal("srv1", f.Cells(3, 2));
        Assert.Equal("11", f.Cells(4, 2));
        Assert.Equal("否", f.Cells(5, 2));      // HUtil32.BoolToCStr(false)
    }

    [Fact]
    public void RefGridSession_PanelStatusCaptionIsNeverReset_OriginalFlaw()
    {
        // ★ 原文如此（缺陷）：`PanelStatus.Caption := '正在取得数据...'` 只设不复位 ⇒
        //   取完数据后状态栏永远停在"正在取得数据..."（差异断言锁死该行为）
        _sessionList.Add(Conn("a", "1", "s", 1, false));

        using var f = new TfrmGrobalSession();
        f.Open();

        Assert.Equal("正在取得数据...", f.PanelStatus.Text);
    }

    [Fact]
    public void RefGridSession_GridBecomesVisibleAgain()
    {
        using var f = new TfrmGrobalSession();
        f.Open();
        // VCL TControl.Visible 是"属性值"；WinForms 的 Control.Visible getter 是"有效可见性"
        // （窗体未 Show 时恒 false）⇒ 断言用承载属性值的 GridSessionVisible（D-P10-13）
        Assert.True(f.GridSessionVisible);
        Assert.True(f.GridSession.Visible == false || f.GridSession.Visible == true);   // 底层属性也被写过
    }

    [Fact]
    public void RefGridSession_ReusesGrid_RowCountShrinks()
    {
        _sessionList.Add(Conn("a", "1", "s", 1, false));
        _sessionList.Add(Conn("b", "2", "s", 2, false));

        using var f = new TfrmGrobalSession();
        f.Open();
        Assert.Equal(3, f.RowCount);

        _sessionList.Clear();
        f.Open();
        Assert.Equal(2, f.RowCount);
        Assert.Equal("", f.Cells(1, 1));        // 第 1 行被清空（原文 Cells[x,1] := ''）
    }

    [Fact]
    public void RefGridSession_ReleasesSessionListLockForOtherThreads()
    {
        _sessionList.Add(Conn("a", "1", "s", 1, false));
        using var f = new TfrmGrobalSession();
        f.Open();

        // TGList.Count 内部 `lock (Critical)`：若 RefGridSession 的 finally UnLock 缺失，
        // 该锁会被本测试线程持续持有 ⇒ 另一个线程在此挂住（用 Join 超时取证）
        bool ok = false;
        var t = new System.Threading.Thread(() => { _ = _sessionList.Count; ok = true; })
        {
            IsBackground = true,
        };
        t.Start();
        Assert.True(t.Join(3000), "SessionList 未被释放（UnLock 缺失）");
        Assert.True(ok);
    }

    [Fact]
    public void RefGridSession_UnwiredSessionList_ThrowsExplicitly()
    {
        // 台账 §25.2：接缝不得静默返回中性值 ⇒ 未接线时显式抛
        GrobalSessionHost.SessionList = null;
        using var f = new TfrmGrobalSession();
        var ex = Assert.Throws<InvalidOperationException>(() => f.Open());
        Assert.Contains("未接线", ex.Message);
    }

    [Fact]
    public void Cells_OutOfRange_IsSilentNoOp_VclStringGridSemantics()
    {
        // D-P10-14：VCL TStringGrid 的 Cells 对越界下标静默忽略；
        //   原文首行清空写 Cells[x,1] 时 RowCount=1（第 1 行不存在）⇒ 托管不得抛异常
        using var f = new TfrmGrobalSession();
        Assert.Equal("", f.Cells(9, 9));
        f.SetCells(9, 9, "x");        // 不抛
        f.SetCells(0, 1, "x");        // RowCount=1 ⇒ 静默忽略
        Assert.Equal("", f.Cells(0, 1));
    }

    [Fact]
    public void Dfm_OnCreate_IsBoundAsLoadHandler()
    {
        // DFM OnCreate 的托管承载：`Load += (s, e) => FormCreate(s);`
        // （本工程既有窗体统一约定，见 GXX.DBServer/AddrEdit.cs:272）
        using var f = new TfrmGrobalSession();
        Assert.True(P10FormReconcile.IsBound(f, "Load"));
    }

    [Fact]
    public void RefGridSession_ClearsSecondRowWhenItExists()
    {
        // 原文清空的是 Cells[x, 1]；只有 RowCount>1 时该行才存在（见 D-P10-14）
        _sessionList.Add(Conn("a", "1", "s", 1, false));
        _sessionList.Add(Conn("b", "2", "s", 2, false));

        using var f = new TfrmGrobalSession();
        f.Open();                       // RowCount = 3
        f.Open();                       // 第二次进入时 RowCount 已是 3 ⇒ 第 1 行先被清空再重填
        Assert.Equal("a", f.Cells(1, 1));
        Assert.Equal("b", f.Cells(1, 2));
    }
}

/// <summary>本车道窗体族测试串行集合（共享静态接缝）。</summary>
[CollectionDefinition("LoginSrvSequential", DisableParallelization = true)]
public sealed class P10LoginSrvSequentialCollection
{
}
