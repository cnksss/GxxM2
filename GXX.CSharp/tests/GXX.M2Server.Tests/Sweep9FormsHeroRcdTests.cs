// ============================================================================
// ViewHeroRcd.pas（453 行）1:1 测试
//   DFM 对账（§37.3 计数取证）：控件 **29** / 事件绑定 **1**（OnCreate↔Load）/ 处理器方法 **1**
//   DFM 数据来自 `Source/M2Engine/Forms/ViewHeroRcd.dfm`（**二进制**，手工解码）
// ============================================================================

using GXX.Core.Protocol;
using GXX.M2Server.Sweep9.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

[Collection("Sweep9FormsSerial")]
public sealed class Sweep9FormsHeroRcdTests : IDisposable
{
    private TFrmHeroFDBViewer Form = null!;

    public Sweep9FormsHeroRcdTests()
    {
        Sweep9FormsMessageBoxSeam.Reset();
        Sweep9FormsMessageBoxSeam.UiEnabled = false;
        Sweep9FormsHeroRcdGlobals.Reset();
        Form = new TFrmHeroFDBViewer();
    }

    public void Dispose()
    {
        Form.Dispose();
        Sweep9FormsHeroRcdGlobals.Reset();
        Sweep9FormsMessageBoxSeam.Reset();
    }

    private static TUserItem Item(ushort wIndex, int makeIndex = 0, ushort dura = 0, ushort duraMax = 0)
        => new() { wIndex = wIndex, MakeIndex = makeIndex, Dura = dura, DuraMax = duraMax };

    // ------------------------------------------------------------------
    // DFM 对账（计数取证）
    // ------------------------------------------------------------------

    [Fact]
    public void DfmReconcile_ControlCount_Is29()
    {
        // 二进制 DFM 手工解码：窗体下 29 个 object 节点（与 .pas:11-39 的 29 个控件字段逐名一致）
        Assert.Equal(29, Sweep9FormsReconcile.CountControlsExcludingForm(Form));
        Assert.Equal(30, Sweep9FormsReconcile.CountControlsIncludingForm(Form));
    }

    [Fact]
    public void DfmReconcile_ControlNames_MatchPasDeclarationsExactly()
    {
        // .pas:11-39 的控件字段名（顺序即 DFM 顺序）
        var expected = new[]
        {
            "PageControlHero", "TabSheet3", "HumanGrid", "TabSheet4", "PageControlHeroJob0",
            "TabSheet7", "UserItemGrid0", "TabSheet8", "BagItemGrid0", "TabSheet9", "UseMagicGrid0",
            "TabSheet5", "PageControlHeroJob1", "TabSheet10", "UserItemGrid1", "TabSheet11",
            "BagItemGrid1", "TabSheet12", "UseMagicGrid1", "TabSheet6", "PageControlHeroJob2",
            "TabSheet13", "UserItemGrid2", "TabSheet14", "BagItemGrid2", "TabSheet15", "UseMagicGrid2",
            "TabSheet1", "UseSuccessiveMagicGrid",
        };
        var got = Sweep9FormsReconcile.EnumerateControls(Form).Skip(1).Select(x => x.Name).ToArray();
        Assert.Equal(expected.OrderBy(x => x, StringComparer.Ordinal),
            got.OrderBy(x => x, StringComparer.Ordinal));
        Assert.Equal(29, expected.Length);
    }

    [Fact]
    public void DfmReconcile_ControlTypes_MatchDfmClasses()
    {
        Assert.Equal(typeof(System.Windows.Forms.TabControl), Form.PageControlHero.GetType());
        Assert.Equal(typeof(System.Windows.Forms.TabControl), Form.PageControlHeroJob0.GetType());
        Assert.Equal(typeof(System.Windows.Forms.TabControl), Form.PageControlHeroJob1.GetType());
        Assert.Equal(typeof(System.Windows.Forms.TabControl), Form.PageControlHeroJob2.GetType());
        Assert.Equal(typeof(System.Windows.Forms.TabPage), Form.TabSheet3.GetType());
        Assert.Equal(typeof(System.Windows.Forms.TabPage), Form.TabSheet1.GetType());
        Assert.Equal(typeof(Sweep9FormsStringGrid), Form.HumanGrid.GetType());
        Assert.Equal(typeof(Sweep9FormsStringGrid), Form.UseSuccessiveMagicGrid.GetType());
    }

    [Fact]
    public void DfmReconcile_EventBindingCount_Is1()
    {
        // DFM 绑定实测：仅根节点 `OnCreate = FormCreate`（29 个控件**全部无事件绑定**）
        Assert.Equal(1, Sweep9FormsReconcile.CountEventBindings(Form));
        Assert.True(Sweep9FormsReconcile.IsBound(Form, "Load"));      // = OnCreate 的 WinForms 等价
    }

    [Fact]
    public void DfmReconcile_NoControlHasAnyBoundEvent()
    {
        // ★ 否定性断言用计数取证：29 个 DFM 控件逐个查"是否挂了任何事件"，总数必须 = 0。
        //   （只数有名字的 DFM 节点：`DataGridView` 内部匿名 `ScrollBar` 的自带订阅不算 DFM 绑定。）
        int bound = 0;
        foreach (var c in Sweep9FormsReconcile.DfmControls(Form))
            bound += Sweep9FormsReconcile.CountEventBindingsOn(c);
        Assert.Equal(0, bound);
    }

    [Fact]
    public void DfmReconcile_HandlerMethodCount_Is1()
    {
        var handlers = typeof(TFrmHeroFDBViewer)
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.DeclaredOnly)
            .Select(m => m.Name)
            .Where(n => n is "FormCreate")
            .ToArray();
        Assert.Equal(new[] { "FormCreate" }, handlers);
    }

    [Fact]
    public void DfmReconcile_FormProperties_MatchBinaryDfm()
    {
        Assert.Equal("FrmHeroFDBViewer", Form.Name);
        Assert.Equal("查看英雄数据", Form.Text);
        Assert.Equal(851, Form.Left);
        Assert.Equal(295, Form.Top);
        Assert.Equal(918, Form.ClientSize.Width);
        Assert.Equal(317, Form.ClientSize.Height);
        Assert.Equal(System.Windows.Forms.FormBorderStyle.FixedSingle, Form.FormBorderStyle);  // bsSingle
        Assert.False(Form.MaximizeBox);      // BorderIcons 无 biMaximize
        Assert.True(Form.MinimizeBox);       // BorderIcons 含 biMinimize
        // DFM Font.Height = -11（vaInt8 有符号）⇒ -11 像素 = 8.25pt @96dpi；
        // ⚠ GDI+ 的 Font.Height 是"行高"（含行距）⇒ 不能直接比 -11，改比 em 尺寸。
        Assert.Equal(8.25F, Form.Font.Size);
        Assert.Equal("MS Sans Serif", Form.Font.OriginalFontName);   // Name 会被 GDI+ 字体替换成 "Microsoft Sans Serif"
    }

    [Fact]
    public void DfmReconcile_BoundsTable_Covers16Nodes()
    {
        Assert.Equal(16, TFrmHeroFDBViewer.DfmBounds.Count);
        Assert.Equal(new System.Drawing.Rectangle(851, 295, 918, 317), TFrmHeroFDBViewer.DfmBounds["FrmHeroFDBViewer"]);
        Assert.Equal(new System.Drawing.Rectangle(0, 0, 910, 289), TFrmHeroFDBViewer.DfmBounds["HumanGrid"]);
        Assert.Equal(new System.Drawing.Rectangle(0, 0, 902, 261), TFrmHeroFDBViewer.DfmBounds["UserItemGrid1"]);
        Assert.Equal(new System.Drawing.Rectangle(0, 0, 902, 261), TFrmHeroFDBViewer.DfmBounds["UseMagicGrid2"]);
        // ⚠ 控件上的 `Width/Height` 不能拿来比 DFM：这些控件全是 `Align = alClient`（Dock = Fill），
        //   一旦 `Controls.Add` 触发布局就被重算（无窗口句柄时塌成默认值）⇒ DFM 几何只以
        //   `DfmBounds` 表为准。这里只确认表与控件**一一对应**（名可查）。
        foreach (var name in TFrmHeroFDBViewer.DfmBounds.Keys)
        {
            if (name == "FrmHeroFDBViewer") continue;
            Assert.NotNull(Sweep9FormsReconcile.FindByName(Form, name));
        }
    }

    [Fact]
    public void DfmReconcile_TabSheetCaptionsAndImageIndexes()
    {
        Assert.Equal(14, TFrmHeroFDBViewer.DfmTabSheets.Count);
        var pages = new (System.Windows.Forms.TabPage Page, string Name)[]
        {
            (Form.TabSheet3, "TabSheet3"), (Form.TabSheet4, "TabSheet4"), (Form.TabSheet5, "TabSheet5"),
            (Form.TabSheet6, "TabSheet6"), (Form.TabSheet1, "TabSheet1"), (Form.TabSheet7, "TabSheet7"),
            (Form.TabSheet8, "TabSheet8"), (Form.TabSheet9, "TabSheet9"), (Form.TabSheet10, "TabSheet10"),
            (Form.TabSheet11, "TabSheet11"), (Form.TabSheet12, "TabSheet12"), (Form.TabSheet13, "TabSheet13"),
            (Form.TabSheet14, "TabSheet14"), (Form.TabSheet15, "TabSheet15"),
        };
        foreach (var (page, name) in pages)
        {
            var (caption, imageIndex) = TFrmHeroFDBViewer.DfmTabSheets[name];
            Assert.Equal(name, page.Name);
            Assert.Equal(caption, page.Text);
            if (imageIndex >= 0) Assert.Equal(imageIndex, page.ImageIndex);
        }
    }

    [Fact]
    public void DfmReconcile_ActivePages_MatchDfm_Asymmetric()
    {
        // ★ 原文三处不对称，逐字保留：
        Assert.Equal(4, Form.PageControlHero.SelectedIndex);          // ActivePage = TabSheet1（**最后一页**）
        Assert.Equal("TabSheet1", Form.PageControlHero.SelectedTab!.Name);
        Assert.Equal(2, Form.PageControlHeroJob0.SelectedIndex);      // = TabSheet9
        Assert.Equal("TabSheet9", Form.PageControlHeroJob0.SelectedTab!.Name);
        Assert.Equal(0, Form.PageControlHeroJob1.SelectedIndex);      // = TabSheet10
        Assert.Equal("TabSheet10", Form.PageControlHeroJob1.SelectedTab!.Name);
        Assert.Equal(2, Form.PageControlHeroJob2.SelectedIndex);      // = TabSheet15
        Assert.Equal("TabSheet15", Form.PageControlHeroJob2.SelectedTab!.Name);
    }

    [Fact]
    public void DfmReconcile_GridProperties_MatchBinaryDfm()
    {
        // HumanGrid：ColCount=12 / RowCount=15 / FixedCols=0 / DefaultRowHeight=20
        Assert.Equal(12, Form.HumanGrid.DfmColCount);
        Assert.Equal(15, Form.HumanGrid.DfmRowCount);
        Assert.Equal(0, Form.HumanGrid.DfmFixedCols);
        Assert.Equal(20, Form.HumanGrid.DfmDefaultRowHeight);
        Assert.Equal(new[] { 64, 69, 64, 64, 64, 64, 64, 64, 64, 64, 71, 98 }, Form.HumanGrid.DfmColWidths);
        Assert.Equal(new[] { 5, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 }, Form.HumanGrid.DfmRowHeights);

        // UserItemGrid0：DefaultColWidth=80、RowCount=16、无 ColCount（Delphi 默认 5）
        Assert.Equal(5, Form.UserItemGrid0.DfmColCount);
        Assert.Equal(16, Form.UserItemGrid0.DfmRowCount);
        Assert.Equal(80, Form.UserItemGrid0.DfmDefaultColWidth);
        Assert.Empty(Form.UserItemGrid0.DfmColWidths);

        // UseMagicGrid1/2：ColCount=4（DFM 显式）
        Assert.Equal(4, Form.UseMagicGrid1.DfmColCount);
        Assert.Equal(4, Form.UseMagicGrid2.DfmColCount);
        Assert.Equal(new[] { 117, 75, 123, 119 }, Form.UseMagicGrid1.DfmColWidths);

        // BagItemGrid 一族：RowCount=63 / FixedCols=0 / ColWidths = (48 88 63 125 124)
        Assert.Equal(63, Form.BagItemGrid0.DfmRowCount);
        Assert.Equal(0, Form.BagItemGrid1.DfmFixedCols);
        Assert.Equal(new[] { 48, 88, 63, 125, 124 }, Form.BagItemGrid2.DfmColWidths);

        // Options 10 张表逐字相同
        Assert.Equal(new[]
        {
            "goFixedVertLine", "goFixedHorzLine", "goVertLine", "goHorzLine",
            "goRangeSelect", "goColSizing", "goThumbTracking",
        }, Form.HumanGrid.DfmOptions);
        Assert.Equal(Form.HumanGrid.DfmOptions, Form.UseSuccessiveMagicGrid.DfmOptions);
    }

    // ------------------------------------------------------------------
    // FormCreate（:72-78）
    // ------------------------------------------------------------------

    [Fact]
    public void FormCreate_Writes48HumanGridHeaders()
    {
        Form.FormCreate();

        // 4 行 × 12 列 = 48 条（.pas:124-174）
        int nonEmpty = 0;
        for (int col = 0; col < 12; col++)
            foreach (int row in new[] { 1, 3, 5, 7 })
                if (Form.HumanGrid.Cells[col, row] != "") nonEmpty++;
        Assert.Equal(48, nonEmpty);

        Assert.Equal("索引号", Form.HumanGrid.Cells[0, 1]);
        Assert.Equal("Home", Form.HumanGrid.Cells[11, 1]);
        Assert.Equal("HomeX", Form.HumanGrid.Cells[0, 3]);
        Assert.Equal("SC/2", Form.HumanGrid.Cells[11, 3]);
        Assert.Equal("Reserved2", Form.HumanGrid.Cells[0, 5]);
        Assert.Equal("最后登录时间", Form.HumanGrid.Cells[11, 5]);
        Assert.Equal("修炼内功", Form.HumanGrid.Cells[0, 7]);
        Assert.Equal("醉酒度", Form.HumanGrid.Cells[11, 7]);
        // 第 0/2/4/6/8..14 行**未写**（原文只写 1/3/5/7 四行）
        Assert.Equal("", Form.HumanGrid.Cells[0, 0]);
        Assert.Equal("", Form.HumanGrid.Cells[0, 2]);
        Assert.Equal("", Form.HumanGrid.Cells[0, 8]);
    }

    [Fact]
    public void FormCreate_InitUserItemGrid_WritesAllThreeGrids()
    {
        Form.FormCreate();

        for (int job = 0; job <= 2; job++)
        {
            var g = Form.UserItemGrid(job);
            Assert.Equal("物品位置", g.Cells[0, 0]);
            Assert.Equal("物品ID", g.Cells[1, 0]);
            Assert.Equal("物品号", g.Cells[2, 0]);
            Assert.Equal("持久", g.Cells[3, 0]);
            Assert.Equal("物品名称", g.Cells[4, 0]);
            Assert.Equal("衣服", g.Cells[0, 1]);
            Assert.Equal("军鼓", g.Cells[0, 15]);
        }
    }

    [Fact]
    public void FormCreate_InitUserItemGrid_AutoGrowsRows14To16_OriginalBehaviour()
    {
        // ★ 原文行为：UserItemGrid1/2 的 DFM `RowCount = 14`，而 :202 写 `Cells[0, 15]`
        //   ⇒ TStringGrid 自动扩容 ⇒ 实到 16 行（UserItemGrid0 的 DFM 本来就是 16）。
        Assert.Equal(14, Form.UserItemGrid1.RowCount);
        Assert.Equal(14, Form.UserItemGrid2.RowCount);
        Assert.Equal(16, Form.UserItemGrid0.RowCount);

        Form.FormCreate();

        Assert.Equal(16, Form.UserItemGrid0.RowCount);
        Assert.Equal(16, Form.UserItemGrid1.RowCount);      // 14 → 16（自动扩容）
        Assert.Equal(16, Form.UserItemGrid2.RowCount);
    }

    [Fact]
    public void FormCreate_Sub49A9DC_WritesBagHeaders()
    {
        Form.FormCreate();
        for (int job = 0; job <= 2; job++)
        {
            var g = Form.BagItemGrid(job);
            Assert.Equal("物品号", g.Cells[0, 0]);
            Assert.Equal("物品ID", g.Cells[1, 0]);
            Assert.Equal("物品号", g.Cells[2, 0]);       // ★ 原文第 0/2 列**同为 '物品号'**（逐字保留）
            Assert.Equal("持久", g.Cells[3, 0]);
            Assert.Equal("物品名称", g.Cells[4, 0]);
            Assert.Equal("", g.Cells[0, 1]);             // 只写第 0 行
        }
    }

    [Fact]
    public void FormCreate_Sub49AB10_WritesMagicHeaders_AndGrowsCols4To5()
    {
        Assert.Equal(4, Form.UseMagicGrid1.ColCount);
        Assert.Equal(4, Form.UseMagicGrid2.ColCount);
        Assert.Equal(5, Form.UseMagicGrid0.ColCount);    // DFM 未写 ColCount ⇒ 默认 5

        Form.FormCreate();

        for (int job = 0; job <= 2; job++)
        {
            var g = Form.UseMagicGrid(job);
            Assert.Equal("技能ID", g.Cells[0, 0]);
            Assert.Equal("快捷键", g.Cells[1, 0]);
            Assert.Equal("修练状态", g.Cells[2, 0]);
            Assert.Equal("技能名称", g.Cells[3, 0]);
            Assert.Equal("技能类型", g.Cells[4, 0]);
            Assert.Equal(5, g.ColCount);                 // 4 → 5（自动扩容）
        }

        // 连击表在循环之外（:232-236），内容与循环体逐字相同
        Assert.Equal("技能ID", Form.UseSuccessiveMagicGrid.Cells[0, 0]);
        Assert.Equal("技能类型", Form.UseSuccessiveMagicGrid.Cells[4, 0]);
    }

    [Fact]
    public void FormCreate_IsIdempotent_ForHeaderCells()
    {
        Form.FormCreate();
        Form.FormCreate();
        Assert.Equal("军鼓", Form.UserItemGrid(1).Cells[0, 15]);
        Assert.Equal("技能类型", Form.UseMagicGrid(2).Cells[4, 0]);
    }

    // ------------------------------------------------------------------
    // GetUserItemGrid / GetBagItemGrid / GetUseMagicGrid（:92-120）
    // ------------------------------------------------------------------

    [Fact]
    public void GridGetters_MapJob1And2_ElseFallsBackToJob0()
    {
        Assert.Same(Form.UserItemGrid0, Form.GetUserItemGrid(0));
        Assert.Same(Form.UserItemGrid1, Form.GetUserItemGrid(1));
        Assert.Same(Form.UserItemGrid2, Form.GetUserItemGrid(2));
        // ★ else 吞掉一切其它值（含 0 / 负数 / 3）
        Assert.Same(Form.UserItemGrid0, Form.GetUserItemGrid(3));
        Assert.Same(Form.UserItemGrid0, Form.GetUserItemGrid(-1));
        Assert.Same(Form.UserItemGrid0, Form.GetUserItemGrid(int.MinValue));

        Assert.Same(Form.BagItemGrid1, Form.GetBagItemGrid(1));
        Assert.Same(Form.BagItemGrid2, Form.GetBagItemGrid(2));
        Assert.Same(Form.BagItemGrid0, Form.GetBagItemGrid(7));

        Assert.Same(Form.UseMagicGrid1, Form.GetUseMagicGrid(1));
        Assert.Same(Form.UseMagicGrid2, Form.GetUseMagicGrid(2));
        Assert.Same(Form.UseMagicGrid0, Form.GetUseMagicGrid(99));
    }

    [Fact]
    public void GridIndexerProperties_ForwardToGetters()
    {
        Assert.Same(Form.GetUserItemGrid(2), Form.UserItemGrid(2));
        Assert.Same(Form.GetBagItemGrid(1), Form.BagItemGrid(1));
        Assert.Same(Form.GetUseMagicGrid(0), Form.UseMagicGrid(0));
    }

    // ------------------------------------------------------------------
    // ShowBagItem（:239-257）/ ShowUserItem（:259-275）
    // ------------------------------------------------------------------

    [Fact]
    public void ShowBagItem_NonEmptyItem_WritesAllFiveColumns()
    {
        Form.ShowBagItem(3, 1, "项链", Item(wIndex: 42, makeIndex: 7, dura: 5, duraMax: 9));

        var g = Form.BagItemGrid(1);
        Assert.Equal("项链", g.Cells[0, 3]);
        Assert.Equal("7", g.Cells[1, 3]);
        Assert.Equal("42", g.Cells[2, 3]);
        Assert.Equal("5/9", g.Cells[3, 3]);
        Assert.Equal("", g.Cells[4, 3]);              // GetStdItemName 接缝默认返回空（未接线）
    }

    [Fact]
    public void ShowBagItem_UsesGetStdItemNameSeam()
    {
        Form.GetStdItemNameHandler = idx => "物品" + idx;
        Form.ShowBagItem(1, 0, "衣服", Item(wIndex: 5));
        Assert.Equal("物品5", Form.BagItemGrid(0).Cells[4, 1]);
        Assert.Equal("5", Form.BagItemGrid(0).Cells[2, 1]);
    }

    [Fact]
    public void ShowBagItem_EmptyItem_ClearsColumnsButKeepsName()
    {
        var g = Form.BagItemGrid(2);
        g.Cells[1, 4] = "旧值";
        g.Cells[4, 4] = "旧值";

        Form.ShowBagItem(4, 2, "戒指", Item(wIndex: 0));

        Assert.Equal("戒指", g.Cells[0, 4]);
        Assert.Equal("", g.Cells[1, 4]);
        Assert.Equal("", g.Cells[2, 4]);
        Assert.Equal("", g.Cells[3, 4]);
        Assert.Equal("", g.Cells[4, 4]);
    }

    [Fact]
    public void ShowUserItem_NonEmptyItem_WritesColumns1To4_Only()
    {
        Form.ShowUserItem(2, 0, "武器", Item(wIndex: 11, makeIndex: 3, dura: 1, duraMax: 2));

        var g = Form.UserItemGrid(0);
        Assert.Equal("3", g.Cells[1, 2]);
        Assert.Equal("11", g.Cells[2, 2]);
        Assert.Equal("1/2", g.Cells[3, 2]);
    }

    [Fact]
    public void ShowUserItem_NeverWritesColumn0_AsymmetricWithShowBagItem()
    {
        // ★★ 原文缺陷：`ShowUserItem`（:259-275）**从不写第 0 列**，
        //    而 `ShowBagItem`（:239-257）两个分支都写。
        //    取证方式：先在第 0 列埋哨兵，再走两个分支，哨兵必须**原封不动**。
        var g = Form.UserItemGrid(1);
        g.Cells[0, 6] = "哨兵A";
        Form.ShowUserItem(6, 1, "头盔", Item(wIndex: 9));
        Assert.Equal("哨兵A", g.Cells[0, 6]);          // 非空分支：第 0 列没被覆盖
        Assert.Equal("9", g.Cells[2, 6]);

        g.Cells[0, 7] = "哨兵B";
        Form.ShowUserItem(7, 1, "头盔", Item(wIndex: 0));
        Assert.Equal("哨兵B", g.Cells[0, 7]);          // 空分支：同样没被覆盖
        Assert.Equal("", g.Cells[1, 7]);

        // 对照：ShowBagItem 在**同一张表**的该位置会写第 0 列
        var bg = Form.BagItemGrid(1);
        bg.Cells[0, 8] = "哨兵C";
        Form.ShowBagItem(8, 1, "头盔", Item(wIndex: 0));
        Assert.Equal("头盔", bg.Cells[0, 8]);
    }

    // ------------------------------------------------------------------
    // ShowHumData（:80-90）＋ 四个空体方法
    // ------------------------------------------------------------------

    [Fact]
    public void ShowHumData_ResetsAllFourPageControlsTo0()
    {
        Form.ShowHumData();

        Assert.Equal(0, Form.PageControlHero.SelectedIndex);
        Assert.Equal(0, Form.PageControlHeroJob0.SelectedIndex);
        Assert.Equal(0, Form.PageControlHeroJob1.SelectedIndex);
        Assert.Equal(0, Form.PageControlHeroJob2.SelectedIndex);
    }

    [Fact]
    public void ShowHumData_DoesNotShowModal_OriginalBehaviour()
    {
        // ★ 原文 :80-90 **没有 ShowModal**（对比同族窗体）—— 逐字保留。
        Form.ShowHumData();
        Assert.Equal(0, Sweep9FormsMessageBoxSeam.ShowModalCount);
    }

    [Fact]
    public void ShowHumData_IsCallableWithEmptyBodies_NoSideEffectsBeyondPages()
    {
        // ShowHumanInfo / ShowUserItems / ShowBagItems / ShowUseMagic 四个方法体
        // **整段被 (* *) 注释掉** ⇒ Delphi 侧是空过程。取证：调它们不改变任何表格内容。
        Form.ShowHumData();
        var snapshot = Snapshot();
        Form.ShowHumData();
        Assert.Equal(snapshot, Snapshot());
    }

    private string Snapshot()
    {
        var sb = new System.Text.StringBuilder();
        var grids = new[]
        {
            Form.HumanGrid, Form.UserItemGrid0, Form.UserItemGrid1, Form.UserItemGrid2,
            Form.BagItemGrid0, Form.BagItemGrid1, Form.BagItemGrid2,
            Form.UseMagicGrid0, Form.UseMagicGrid1, Form.UseMagicGrid2, Form.UseSuccessiveMagicGrid,
        };
        foreach (var g in grids)
        {
            sb.Append(g.RowCount).Append('x').Append(g.ColCount).Append(':');
            for (int r = 0; r < Math.Min(g.RowCount, 3); r++)
                for (int c = 0; c < g.ColCount; c++)
                    sb.Append(g.Cells[c, r]).Append('|');
            sb.Append(';');
        }
        return sb.ToString();
    }

    [Fact]
    public void EmptyBodyMethods_ExistAndAreEmpty()
    {
        // 四个方法必须**存在**（原文有声明与实现），且调用无副作用
        Form.ShowHumanInfo();
        Form.ShowBagItems();
        Form.ShowUserItems();
        Form.ShowUseMagic();
        Assert.Equal(0, Sweep9FormsMessageBoxSeam.ShowModalCount);
    }

    [Fact]
    public void SetActivePageIndex_OutOfRange_IsSilentlySetToMinusOne()
    {
        // 复刻 Delphi `TPageControl.ActivePageIndex := X`（WinForms 会抛）
        TFrmHeroFDBViewer.SetActivePageIndex(Form.PageControlHero, 99);
        Assert.Equal(-1, Form.PageControlHero.SelectedIndex);
        TFrmHeroFDBViewer.SetActivePageIndex(Form.PageControlHero, -5);
        Assert.Equal(-1, Form.PageControlHero.SelectedIndex);
        TFrmHeroFDBViewer.SetActivePageIndex(Form.PageControlHero, 2);
        Assert.Equal(2, Form.PageControlHero.SelectedIndex);
    }

    // ------------------------------------------------------------------
    // 其它公开面
    // ------------------------------------------------------------------

    [Fact]
    public void PublicFields_DefaultLikeOriginal()
    {
        // 原文 :56-57 两个 public 字段全单元 0 写入点 ⇒ 保持 CLR 默认（0 / null→""）
        Assert.Equal(0, Form.n2F8);
        Assert.Equal("", Form.s2FC);
    }

    [Fact]
    public void GlobalFormVariable_IsNullUntilWired()
    {
        Assert.Null(Sweep9FormsHeroRcdGlobals.FrmHeroFDBViewer);
        var f = new TFrmHeroFDBViewer();
        Sweep9FormsHeroRcdGlobals.FrmHeroFDBViewer = f;
        Assert.Same(f, Sweep9FormsHeroRcdGlobals.FrmHeroFDBViewer);
        f.Dispose();
    }
}
