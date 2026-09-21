// ============================================================================
// 车道 p9-m2-forms 共用：VCL `TStringGrid` → WinForms 垫片控件
//
// 为什么不是裸 `DataGridView`：
//   `TStringGrid.Cells[Col, Row] := X` 在 Delphi 里**越界会自动扩容**
//   （`TStringGrid.SetCells`：`if ACol >= ColCount then ColCount := ACol + 1;`
//    `if ARow >= RowCount then RowCount := ARow + 1;`）。
//   ViewHeroRcd 的 DFM 就依赖这一行为：`UserItemGrid1/2` 的 DFM `RowCount = 14`
//   而 `InitUserItemGrid` 会写到 `Cells[0, 15]`（⇒ 涨到 16）；
//   `UseMagicGrid1/2` 的 DFM `ColCount = 4` 而 `sub_49AB10` 写到 `Cells[4, 0]`（⇒ 涨到 5）。
//   `DataGridView` 越界**抛异常** ⇒ 直译必崩。故本垫片保留 Delphi 的自动扩容语义
//   （《并行派发台账》§21.3 同款陷阱的同类处置）。
//
// DFM 属性留证：`ColCount/RowCount/ColWidths/RowHeights/FixedCols/DefaultRowHeight/
//   DefaultColWidth/Options` 全部按 DFM 原文值记录在 `Dfm*` 字段上（便于对账用例断言），
//   并尽力映射到 `DataGridView` 的对应属性。
// ============================================================================

namespace GXX.M2Server.Sweep9.Forms;

/// <summary>
/// VCL `TStringGrid` 的托管等价（`DataGridView` + Delphi 的 `Cells` 自动扩容语义）。
/// </summary>
public sealed class Sweep9FormsStringGrid : System.Windows.Forms.DataGridView
{
    // ------------------------------------------------------------------
    // DFM 原始属性留证（`TStringGrid` 专有，DataGridView 无一一对应者）
    // ------------------------------------------------------------------

    /// <summary>DFM `ColCount`（Delphi 默认 5）。</summary>
    public int DfmColCount = 5;
    /// <summary>DFM `RowCount`（Delphi 默认 5）。</summary>
    public int DfmRowCount = 5;
    /// <summary>DFM `FixedCols`（Delphi 默认 1；0 = 无固定列）。</summary>
    public int DfmFixedCols = 1;
    /// <summary>DFM `FixedRows`（Delphi 默认 1；本单元 DFM 未写 ⇒ 1）。</summary>
    public int DfmFixedRows = 1;
    /// <summary>DFM `DefaultRowHeight`（Delphi 默认 24）。</summary>
    public int DfmDefaultRowHeight = 24;
    /// <summary>DFM `DefaultColWidth`（Delphi 默认 64）。</summary>
    public int DfmDefaultColWidth = 64;
    /// <summary>DFM `Options` 集合原文（如 `goFixedVertLine`…），原样保留供对账。</summary>
    public readonly List<string> DfmOptions = new();
    /// <summary>DFM `ColWidths` 数列（原样）。</summary>
    public readonly List<int> DfmColWidths = new();
    /// <summary>DFM `RowHeights` 数列（原样）。</summary>
    public readonly List<int> DfmRowHeights = new();

    /// <summary>构造（`AllowUserToAddRows = false` 对齐 Delphi：没有"新行占位符"）。</summary>
    public Sweep9FormsStringGrid()
    {
        Cells = new Sweep9FormsGridCells(this);
        AllowUserToAddRows = false;
        AllowUserToDeleteRows = false;
        AllowUserToResizeRows = false;
        RowHeadersVisible = false;
        ColumnHeadersVisible = false;
        // Delphi 默认 goRowSelect（本单元 DFM Options 含 goRowSelect）⇒ WinForms FullRowSelect
        SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        MultiSelect = false;
        EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
    }

    /// <summary>
    /// Delphi `TStringGrid.ColCount`（**可写**；DataGridView 的 `ColumnCount` 即等价物）。
    /// </summary>
    public int ColCount
    {
        get => ColumnCount;
        set => ColumnCount = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Delphi `TStringGrid.RowCount`（**有意隐藏** `DataGridView.RowCount`：Delphi 语义下
    /// 没有"新行占位符"，且本垫片已置 `AllowUserToAddRows = false`，两者数值一致）。
    /// </summary>
    public new int RowCount
    {
        get => base.RowCount;
        set => base.RowCount = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Delphi `TStringGrid.Cells[Col, Row]`（**0-based**；越界**自动扩容**，与 Delphi 一致）。
    /// <para>
    /// C# 无"具名索引器" ⇒ 用具名属性 + 索引器垫片复刻，调用形态与原文逐字一致：
    /// <c>HumanGrid.Cells[0, 1] := '索引号'</c> → <c>HumanGrid.Cells[0, 1] = "索引号";</c>
    /// </para>
    /// </summary>
    public Sweep9FormsGridCells Cells { get; }

    /// <summary>Delphi `TStringGrid.Cells[Col, Row]` 的**直取形态**（与 <see cref="Cells"/> 等价）。</summary>
    public string this[int col, int row]
    {
        get => GetCells(col, row);
        set => SetCells(col, row, value);
    }

    /// <summary>Delphi `TStringGrid.GetCells(ACol, ARow): string`。</summary>
    public string GetCells(int col, int row) => Rows[row].Cells[col].Value as string ?? "";

    /// <summary>
    /// Delphi `TStringGrid.SetCells(ACol, ARow: Integer; const Value: string)`（Grids.pas）：
    /// <code>
    ///   if (ACol >= ColCount) then ColCount := ACol + 1;
    ///   if (ARow >= RowCount) then RowCount := ARow + 1;
    ///   ...
    /// </code>
    /// </summary>
    public void SetCells(int col, int row, string value)
    {
        if (col >= ColumnCount) ColumnCount = col + 1;
        if (row >= base.RowCount) base.RowCount = row + 1;
        Rows[row].Cells[col].Value = value;
    }

    /// <summary>按 DFM 原文值把 `ColCount/RowCount/ColWidths/RowHeights` 落到控件上（构造期调用一次）。</summary>
    public void ApplyDfmCounts()
    {
        ColumnCount = DfmColCount < 1 ? 1 : DfmColCount;
        base.RowCount = DfmRowCount < 1 ? 1 : DfmRowCount;
        for (int i = 0; i < DfmColWidths.Count && i < ColumnCount; i++)
            if (DfmColWidths[i] > 0) Columns[i].Width = DfmColWidths[i];
        for (int i = 0; i < DfmRowHeights.Count && i < base.RowCount; i++)
            if (DfmRowHeights[i] > 0) Rows[i].Height = DfmRowHeights[i];
        RowTemplate.Height = DfmDefaultRowHeight;
        foreach (DataGridViewColumn c in Columns)
            if (c.Width == 100) c.Width = DfmDefaultColWidth;   // DataGridView 默认列宽 100 ⇒ 非 DFM 显式值时才用 DFM 默认宽
    }
}

/// <summary>
/// `TStringGrid.Cells[Col, Row]` 的具名索引器垫片（C# 无具名索引器）
/// —— 让托管代码能逐字写 `Grid.Cells[Col, Row] := X` 的对应形态。
/// </summary>
public sealed class Sweep9FormsGridCells
{
    private readonly Sweep9FormsStringGrid _grid;

    /// <summary>绑定到宿主表格。</summary>
    public Sweep9FormsGridCells(Sweep9FormsStringGrid grid) => _grid = grid;

    /// <summary>Delphi `Cells[Col, Row]`。</summary>
    public string this[int col, int row]
    {
        get => _grid.GetCells(col, row);
        set => _grid.SetCells(col, row, value);
    }
}
