// ============================================================================
// ConfigMonGen.pas（60 行）1:1 测试
//   DFM 对账（§37.3 计数取证）：控件 **2** / 事件绑定 **1** / 处理器方法 **1**
//   Open（:38-57）＋ ListBoxMonGenDblClick（:32-36）
// ============================================================================

using GXX.M2Server.Sweep9.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

[Collection("Sweep9FormsSerial")]
public sealed class Sweep9FormsConfigMonGenTests : IDisposable
{
    private readonly System.Collections.Generic.List<string> Clipboard = new();
    private readonly System.Collections.Generic.List<int> LockIds = new();
    private readonly System.Collections.Generic.List<Sweep9FormsMonGenInfo> Table = new();
    private TfrmConfigMonGen Form = null!;

    public Sweep9FormsConfigMonGenTests()
    {
        Sweep9FormsMessageBoxSeam.Reset();
        Sweep9FormsMessageBoxSeam.UiEnabled = false;        // ★ 无头：否则挂死 testhost
        Sweep9FormsConfigMonGenGlobals.Reset();
        Form = new TfrmConfigMonGen();
        Wire();
    }

    private void Wire()
    {
        Form.MonGenListHandler = () => Table;
        Form.MonGenListLockR = id => LockIds.Add(id);
        Form.MonGenListUnLockR = () => LockIds.Add(-1);
        Form.SetClipboardText = Clipboard.Add;
    }

    public void Dispose()
    {
        Form.Dispose();
        Sweep9FormsConfigMonGenGlobals.Reset();
        Sweep9FormsMessageBoxSeam.Reset();
    }

    // ------------------------------------------------------------------
    // DFM 对账（计数取证）
    // ------------------------------------------------------------------

    [Fact]
    public void DfmReconcile_ControlCount_Is2()
    {
        // ConfigMonGen.dfm 里窗体内 object 节点 = ListBoxMonGen（:15）+ pnl（:26） = 2
        Assert.Equal(2, Sweep9FormsReconcile.CountControlsExcludingForm(Form));
        Assert.Equal(3, Sweep9FormsReconcile.CountControlsIncludingForm(Form));
    }

    [Fact]
    public void DfmReconcile_ControlNamesAndTypes_MatchDfmExactly()
    {
        var got = Sweep9FormsReconcile.EnumerateControls(Form)
            .Select(x => x.Name + ":" + x.Type.Name).ToArray();
        Assert.Equal(new[]
        {
            "frmConfigMonGen:TfrmConfigMonGen",
            "ListBoxMonGen:ListBox",
            "pnl:Panel",
        }, got);
    }

    [Fact]
    public void DfmReconcile_EventBindingCount_Is1()
    {
        // DFM 绑定实测：仅 `:23 OnDblClick = ListBoxMonGenDblClick`（窗体本身无 OnCreate）
        Assert.Equal(1, Sweep9FormsReconcile.CountEventBindings(Form));
    }

    [Fact]
    public void DfmReconcile_HandlerMethodCount_Is1()
    {
        // 原文只有 1 个事件处理器方法（ListBoxMonGenDblClick，:32）+ 1 个 public Open（:38）
        var handlers = typeof(TfrmConfigMonGen)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.DeclaredOnly)
            .Select(m => m.Name)
            .Where(n => n.EndsWith("Click", StringComparison.Ordinal))
            .ToArray();
        Assert.Equal(new[] { "ListBoxMonGenDblClick" }, handlers);
    }

    [Fact]
    public void DfmReconcile_FormProperties_MatchDfm()
    {
        Assert.Equal("刷怪配制", Form.Text);                       // DFM Caption
        Assert.Equal(989, Form.Left);
        Assert.Equal(614, Form.Top);
        Assert.Equal(578, Form.ClientSize.Width);
        Assert.Equal(326, Form.ClientSize.Height);
        Assert.Equal(System.Windows.Forms.FormStartPosition.CenterParent, Form.StartPosition);
        // DFM 未写 BorderStyle ⇒ Delphi bsSizeable
        Assert.Equal(System.Windows.Forms.FormBorderStyle.Sizable, Form.FormBorderStyle);
    }

    [Fact]
    public void DfmReconcile_ControlGeometry_MatchDfm()
    {
        Assert.Equal(new System.Drawing.Rectangle(0, 0, 578, 300), TfrmConfigMonGen.ListBoxMonGenDfmBounds);
        Assert.Equal(new System.Drawing.Rectangle(0, 300, 578, 26), TfrmConfigMonGen.pnlDfmBounds);
        Assert.Equal(System.Windows.Forms.DockStyle.Top, Form.ListBoxMonGen.Dock);      // Align = alTop
        Assert.Equal("双击条目复制到剪贴板", Form.pnlCaption);                            // pnl.Caption
        Assert.Equal(System.Windows.Forms.DockStyle.Fill, Form.pnl.Dock);               // Align = alClient
    }

    // ------------------------------------------------------------------
    // Open（:38-57）
    // ------------------------------------------------------------------

    [Fact]
    public void Open_PopulatesListFromMonGenList()
    {
        Table.Add(new Sweep9FormsMonGenInfo { sMapName = "0", nX = 330, nY = 330, sMonName = "鸡" });
        Table.Add(new Sweep9FormsMonGenInfo { sMapName = "3", nX = 100, nY = 200, sMonName = "稻草人" });

        Form.Open();

        Assert.Equal(2, Form.ListBoxMonGen.Items.Count);
        // 原文 :49 `sMapName + '(' + nX + ':' + nY + ')' + ' - ' + sMonName`
        Assert.Equal("0(330:330) - 鸡", Form.ListBoxMonGen.Items[0]);
        Assert.Equal("3(100:200) - 稻草人", Form.ListBoxMonGen.Items[1]);
    }

    [Fact]
    public void Open_KeepsObjectCarrierInParallel()
    {
        var info = new Sweep9FormsMonGenInfo { sMapName = "0", nX = 1, nY = 2, sMonName = "M" };
        Table.Add(info);

        Form.Open();

        Assert.Single(Form.ItemObjects);
        Assert.Same(info, Form.ItemObjects[0]);       // 原文 AddObject 的 TObject(MonGen)
    }

    [Fact]
    public void Open_DoesNotClearList_AccumulatesAcrossCalls_OriginalBehaviour()
    {
        // ★ 原文缺陷：`Open` **没有** `ListBoxMonGen.Clear` ⇒ 反复 Open 会累积（原文如此）。
        Table.Add(new Sweep9FormsMonGenInfo { sMapName = "0", nX = 1, nY = 1, sMonName = "A" });

        Form.Open();
        Form.Open();

        Assert.Equal(2, Form.ListBoxMonGen.Items.Count);
        Assert.Equal(2, Form.ItemObjects.Count);
        Assert.Equal(2, Sweep9FormsMessageBoxSeam.ShowModalCount);   // 每次都走 ShowModal
    }

    [Fact]
    public void Open_EmptyList_LeavesListUntouched()
    {
        Form.Open();
        Assert.Empty(Form.ListBoxMonGen.Items);
        Assert.Equal(1, Sweep9FormsMessageBoxSeam.ShowModalCount);
    }

    [Fact]
    public void Open_MultiThreadRunFalse_DoesNotLock()
    {
        Form.g_MultiThreadRun = false;
        Table.Add(new Sweep9FormsMonGenInfo());
        Form.Open();
        Assert.Empty(LockIds);
    }

    [Fact]
    public void Open_MultiThreadRunTrue_LocksWithId14_ThenUnlocks()
    {
        Form.g_MultiThreadRun = true;
        Table.Add(new Sweep9FormsMonGenInfo());
        Form.Open();
        // 原文 :44 LockR(14) / :54 UnLockR —— id 恒为 14
        Assert.Equal(new[] { 14, -1 }, LockIds);
    }

    [Fact]
    public void Open_UnwiredEngine_TreatedAsEmptyTable()
    {
        // 接缝：`UserEngine.m_MonGenList` 未接线 ⇒ 等价于"引擎里没有任何刷怪模板"（Count = 0）
        Form.MonGenListHandler = null;
        Form.Open();
        Assert.Empty(Form.ListBoxMonGen.Items);
        Assert.Equal(1, Sweep9FormsMessageBoxSeam.ShowModalCount);
    }

    // ------------------------------------------------------------------
    // ListBoxMonGenDblClick（:32-36）
    // ------------------------------------------------------------------

    [Fact]
    public void DblClick_NoSelection_DoesNotTouchClipboard()
    {
        Table.Add(new Sweep9FormsMonGenInfo { sMapName = "0", nX = 1, nY = 1, sMonName = "A" });
        Form.Open();
        Form.ListBoxMonGen.SelectedIndex = -1;          // ItemIndex < 0

        Form.ListBoxMonGenDblClick(Form.ListBoxMonGen);

        Assert.Empty(Clipboard);
    }

    [Fact]
    public void DblClick_WithSelection_CopiesItemText()
    {
        Table.Add(new Sweep9FormsMonGenInfo { sMapName = "0", nX = 330, nY = 331, sMonName = "鸡" });
        Form.Open();
        Form.ListBoxMonGen.SelectedIndex = 0;

        Form.ListBoxMonGenDblClick(Form.ListBoxMonGen);

        // 原文 :35 `Clipboard.AsText := ListBoxMonGen.Items[ListBoxMonGen.ItemIndex]`
        Assert.Equal(new[] { "0(330:331) - 鸡" }, Clipboard);
    }

    [Fact]
    public void GlobalFormVariable_IsNullUntilSvwired()
    {
        // 原文 :22 `var frmConfigMonGen: TfrmConfigMonGen;` —— 由 svMain.pas:2804 创建
        Assert.Null(Sweep9FormsConfigMonGenGlobals.frmConfigMonGen);
        var f = new TfrmConfigMonGen();
        Sweep9FormsConfigMonGenGlobals.frmConfigMonGen = f;
        Assert.Same(f, Sweep9FormsConfigMonGenGlobals.frmConfigMonGen);
        f.Dispose();
    }
}
