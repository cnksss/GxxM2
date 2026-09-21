// ============================================================================
// ViewHeroRcd.pas 同源二进制 DFM 的控件树实例化（29 个控件 + 窗体自身属性）
// 数据来源：手工解码 `Source/M2Engine/Forms/ViewHeroRcd.dfm`（4,510 字节，见报告 §0.2）
// ============================================================================

namespace GXX.M2Server.Sweep9.Forms;

public sealed partial class TFrmHeroFDBViewer
{
    /// <summary>DFM 全部表格共用的 `Options` 集合（原文 10 张表逐字相同）。</summary>
    private static readonly string[] DfmGridOptions =
    {
        "goFixedVertLine", "goFixedHorzLine", "goVertLine", "goHorzLine",
        "goRangeSelect", "goColSizing", "goThumbTracking",
    };

    /// <summary>DFM 控件树 1:1 实例化（属性逐条取自二进制 DFM 手工解码结果）。</summary>
    private void InitializeComponents()
    {
        // ---- 窗体自身（DFM 根节点 FrmHeroFDBViewer: TFrmHeroFDBViewer） ----
        Name = "FrmHeroFDBViewer";
        Text = "查看英雄数据";                                             // Caption（vaWString，6 个 UTF-16 码元）
        Left = 851;                                                       // Left
        Top = 295;                                                        // Top
        ClientSize = new System.Drawing.Size(918, 317);                    // ClientWidth/ClientHeight
        // BorderIcons = [biSystemMenu, biMinimize] ⇒ 有最小化、无最大化
        MaximizeBox = false;
        MinimizeBox = true;
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle; // BorderStyle = bsSingle
        // DFM 未写 Position ⇒ Delphi 默认 poDesigned ⇒ WinForms 默认 WindowsDefaultLocation（不设置）
        Font = new System.Drawing.Font("MS Sans Serif", 8.25F);            // Font.Name = 'MS Sans Serif' / Height = -11
        // DFM `OnCreate = FormCreate` ⇒ WinForms 等价事件 Load（绑定数对账：DFM 1 ↔ 托管 1）
        Load += (_, _) => FormCreate(this);

        // =================================================================
        // PageControlHero（DFM: TPageControl，ActivePage = TabSheet1 ⇒ SelectedIndex = 4）
        // =================================================================
        PageControlHero = new System.Windows.Forms.TabControl
        {
            Name = "PageControlHero",
            Left = 0, Top = 0, Width = 918, Height = 317,
            Dock = System.Windows.Forms.DockStyle.Fill,                    // Align = alClient
            TabIndex = 0,
        };

        // ---- 页 1：TabSheet3 '英雄信息' → HumanGrid ----
        TabSheet3 = MakePage("TabSheet3", "英雄信息");
        HumanGrid = MakeGrid("HumanGrid", colCount: 12, rowCount: 15, fixedCols: 0,
            defaultRowHeight: 20, defaultColWidth: 64,
            colWidths: new[] { 64, 69, 64, 64, 64, 64, 64, 64, 64, 64, 71, 98 },
            rowHeights: new[] { 5, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 },
            tabOrder: 0);
        TabSheet3.Controls.Add(HumanGrid);

        // ---- 页 2：TabSheet4 '战士英雄' → PageControlHeroJob0（ActivePage = TabSheet9） ----
        TabSheet4 = MakePage("TabSheet4", "战士英雄", imageIndex: 1);
        PageControlHeroJob0 = MakePageControl("PageControlHeroJob0");
        TabSheet7 = MakePage("TabSheet7", "身上装备");
        UserItemGrid0 = MakeGrid("UserItemGrid0", colCount: 5, rowCount: 16, fixedCols: 1,
            defaultRowHeight: 20, defaultColWidth: 80, colWidths: Array.Empty<int>(),
            rowHeights: new[] { 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20 },
            tabOrder: 0);
        TabSheet7.Controls.Add(UserItemGrid0);
        TabSheet8 = MakePage("TabSheet8", "背包物品", imageIndex: 1);
        BagItemGrid0 = MakeGrid("BagItemGrid0", colCount: 5, rowCount: 63, fixedCols: 0,
            defaultRowHeight: 20, defaultColWidth: 64, colWidths: new[] { 48, 88, 63, 125, 124 },
            rowHeights: Array.Empty<int>(), tabOrder: 0);
        TabSheet8.Controls.Add(BagItemGrid0);
        TabSheet9 = MakePage("TabSheet9", "修炼技能", imageIndex: 2);
        UseMagicGrid0 = MakeGrid("UseMagicGrid0", colCount: 5, rowCount: 27, fixedCols: 0,
            defaultRowHeight: 20, defaultColWidth: 64, colWidths: new[] { 117, 75, 123, 119, 108 },
            rowHeights: Array.Empty<int>(), tabOrder: 0);
        TabSheet9.Controls.Add(UseMagicGrid0);
        PageControlHeroJob0.TabPages.AddRange(new[] { TabSheet7, TabSheet8, TabSheet9 });
        PageControlHeroJob0.SelectedIndex = 2;                             // DFM ActivePage = TabSheet9
        TabSheet4.Controls.Add(PageControlHeroJob0);

        // ---- 页 3：TabSheet5 '法师英雄' → PageControlHeroJob1（ActivePage = TabSheet10） ----
        TabSheet5 = MakePage("TabSheet5", "法师英雄", imageIndex: 2);
        PageControlHeroJob1 = MakePageControl("PageControlHeroJob1");
        TabSheet10 = MakePage("TabSheet10", "身上装备");
        UserItemGrid1 = MakeGrid("UserItemGrid1", colCount: 5, rowCount: 14, fixedCols: 1,
            defaultRowHeight: 20, defaultColWidth: 80, colWidths: Array.Empty<int>(),
            rowHeights: Array.Empty<int>(), tabOrder: 0);
        TabSheet10.Controls.Add(UserItemGrid1);
        TabSheet11 = MakePage("TabSheet11", "背包物品", imageIndex: 1);
        BagItemGrid1 = MakeGrid("BagItemGrid1", colCount: 5, rowCount: 63, fixedCols: 0,
            defaultRowHeight: 20, defaultColWidth: 64, colWidths: new[] { 48, 88, 63, 125, 124 },
            rowHeights: Array.Empty<int>(), tabOrder: 0);
        TabSheet11.Controls.Add(BagItemGrid1);
        TabSheet12 = MakePage("TabSheet12", "修炼技能", imageIndex: 2);
        UseMagicGrid1 = MakeGrid("UseMagicGrid1", colCount: 4, rowCount: 27, fixedCols: 0,
            defaultRowHeight: 20, defaultColWidth: 64, colWidths: new[] { 117, 75, 123, 119 },
            rowHeights: Array.Empty<int>(), tabOrder: 0);
        TabSheet12.Controls.Add(UseMagicGrid1);
        PageControlHeroJob1.TabPages.AddRange(new[] { TabSheet10, TabSheet11, TabSheet12 });
        PageControlHeroJob1.SelectedIndex = 0;                             // DFM ActivePage = TabSheet10
        TabSheet5.Controls.Add(PageControlHeroJob1);

        // ---- 页 4：TabSheet6 '道士英雄' → PageControlHeroJob2（ActivePage = TabSheet15） ----
        TabSheet6 = MakePage("TabSheet6", "道士英雄", imageIndex: 3);
        PageControlHeroJob2 = MakePageControl("PageControlHeroJob2");
        TabSheet13 = MakePage("TabSheet13", "身上装备");
        UserItemGrid2 = MakeGrid("UserItemGrid2", colCount: 5, rowCount: 14, fixedCols: 1,
            defaultRowHeight: 20, defaultColWidth: 80, colWidths: Array.Empty<int>(),
            rowHeights: Array.Empty<int>(), tabOrder: 0);
        TabSheet13.Controls.Add(UserItemGrid2);
        TabSheet14 = MakePage("TabSheet14", "背包物品", imageIndex: 1);
        BagItemGrid2 = MakeGrid("BagItemGrid2", colCount: 5, rowCount: 63, fixedCols: 0,
            defaultRowHeight: 20, defaultColWidth: 64, colWidths: new[] { 48, 88, 63, 125, 124 },
            rowHeights: Array.Empty<int>(), tabOrder: 0);
        TabSheet14.Controls.Add(BagItemGrid2);
        TabSheet15 = MakePage("TabSheet15", "修炼技能", imageIndex: 2);
        UseMagicGrid2 = MakeGrid("UseMagicGrid2", colCount: 4, rowCount: 27, fixedCols: 0,
            defaultRowHeight: 20, defaultColWidth: 64, colWidths: new[] { 117, 75, 123, 119 },
            rowHeights: Array.Empty<int>(), tabOrder: 0);
        TabSheet15.Controls.Add(UseMagicGrid2);
        PageControlHeroJob2.TabPages.AddRange(new[] { TabSheet13, TabSheet14, TabSheet15 });
        PageControlHeroJob2.SelectedIndex = 2;                             // DFM ActivePage = TabSheet15
        TabSheet6.Controls.Add(PageControlHeroJob2);

        // ---- 页 5：TabSheet1 '连击技能' → UseSuccessiveMagicGrid ----
        TabSheet1 = MakePage("TabSheet1", "连击技能", imageIndex: 4);
        UseSuccessiveMagicGrid = MakeGrid("UseSuccessiveMagicGrid", colCount: 4, rowCount: 27, fixedCols: 0,
            defaultRowHeight: 20, defaultColWidth: 64, colWidths: new[] { 117, 75, 123, 119 },
            rowHeights: Array.Empty<int>(), tabOrder: 0);
        TabSheet1.Controls.Add(UseSuccessiveMagicGrid);

        // 页序 = DFM 子节点顺序（TabSheet3,4,5,6,1）
        PageControlHero.TabPages.AddRange(new[] { TabSheet3, TabSheet4, TabSheet5, TabSheet6, TabSheet1 });
        PageControlHero.SelectedIndex = 4;                                 // DFM ActivePage = TabSheet1
        Controls.Add(PageControlHero);
    }

    // ------------------------------------------------------------------
    // 构造辅助（仅用于把 DFM 值落到控件上；不含任何业务逻辑）
    // ------------------------------------------------------------------

    /// <summary>DFM `TTabSheet`（`Caption` / 可选 `ImageIndex`）。</summary>
    private static System.Windows.Forms.TabPage MakePage(string name, string caption, int imageIndex = -1)
    {
        var page = new System.Windows.Forms.TabPage
        {
            Name = name,
            Text = caption,
            UseVisualStyleBackColor = false,
        };
        if (imageIndex >= 0)
            page.ImageIndex = imageIndex;
        return page;
    }

    /// <summary>DFM `TPageControl`（`Align = alClient`，无其它非默认属性）。</summary>
    private static System.Windows.Forms.TabControl MakePageControl(string name)
    {
        var pc = new System.Windows.Forms.TabControl
        {
            Name = name,
            Dock = System.Windows.Forms.DockStyle.Fill,                    // Align = alClient
            TabIndex = 0,
        };
        ApplyDfmBounds(pc, name);
        return pc;
    }

    /// <summary>按手工解码出的 DFM 原始几何设置 `Left/Top/Width/Height`（表里没有则不动）。</summary>
    private static void ApplyDfmBounds(System.Windows.Forms.Control control, string dfmName)
    {
        if (!DfmBounds.TryGetValue(dfmName, out var r)) return;
        control.Left = r.Left;
        control.Top = r.Top;
        control.Width = r.Width;
        control.Height = r.Height;
    }

    /// <summary>DFM `TStringGrid`（`Align = alClient`，属性逐条落盘 + 留证）。</summary>
    private static Sweep9FormsStringGrid MakeGrid(string name, int colCount, int rowCount, int fixedCols,
        int defaultRowHeight, int defaultColWidth, int[] colWidths, int[] rowHeights, int tabOrder)
    {
        var grid = new Sweep9FormsStringGrid
        {
            Name = name,
            Dock = System.Windows.Forms.DockStyle.Fill,                    // Align = alClient
            TabIndex = tabOrder,
            DfmColCount = colCount,
            DfmRowCount = rowCount,
            DfmFixedCols = fixedCols,
            DfmFixedRows = 1,                                              // DFM 未写 ⇒ Delphi 默认 1
            DfmDefaultRowHeight = defaultRowHeight,
            DfmDefaultColWidth = defaultColWidth,
        };
        ApplyDfmBounds(grid, name);
        grid.DfmOptions.AddRange(DfmGridOptions);
        grid.DfmColWidths.AddRange(colWidths);
        grid.DfmRowHeights.AddRange(rowHeights);
        grid.ApplyDfmCounts();
        return grid;
    }
}
