using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GXX.Core.Rtl;

namespace GXX.RunGate;

// =====================================================================================
// uFrmProcessBlacklist.pas 1:1 转换（Source\RunGate\uFrmProcessBlacklist.pas，202 行 / LF 201）。
// 布局真源：Source\RunGate\uFrmProcessBlacklist.dfm（**同名 .dfm 存在**）。
//
// 窗体职责：进程黑名单列表（序号 / 进程名 / MD5），支持按字段搜索、搜索下一个、添加（转发
// uFrmAddProcessBlack）、右键删除，并在增删后 SaveProcessBlacklist + RebuildProcessBlacklist。
//
// 纯逻辑抽离（不依赖 WinForms）：
//   * ProcessBlacklistLogic.BuildRows(...)      —— 原 :50-59 的列表填充
//   * ProcessBlacklistLogic.FindIndex(...)      —— 原 :68-96 / :98-131 的搜索（两处**逻辑相同**，只有起点不同）
//   * ProcessBlacklistLogic.NormalizeCaption    —— 原 :193-196 的序号重排
//   * ProcessBlacklistLogic.NextSearchStart(...) —— 原 :104-108 的"搜索下一个"起点回绕
//
// ★ 原文缺陷（照抄 + 差异断言）：
//   D1. `ShowFrmProcessBlacklist`（原 :52）从 `g_ProcessBlacklist.Items[I]` 取数据，
//       而 `btnAddClick`/`mniDeleteClick` 用的是 `g_ProcessBlackList`（**大小写 B/L 不同**）。
//       Delphi 大小写不敏感 → 同一变量；C# 侧接缝只有一个 `FormGlobals.g_ProcessBlackList`，
//       故此差异在托管侧**不存在**（登记为"名义差异已消解"）。
//   D2. `btnSearchClick`（原 :68-96）与 `btnSearchNextClick`（原 :98-131）的搜索体**完全重复**，
//       唯一差异是起始下标（前者恒 0，后者取当前选中 +1 并回绕）。此处合并为一个纯函数 + 两个入口。
//   D3. `btnSearchClick`（原 :88-94）搜索**成功后没有 Exit 外层**（有 `Break`），
//       因此只定位到**第一个**匹配项；`btnSearchNextClick` 亦然。
//   D4. `btnAddClick`（原 :151）新行序号写 `IntToStr(Items.Count)`（**Add 之后的 Count**，
//       即等于行数本身，不是 I+1）；`mniDeleteClick`（原 :193-196）删除后**统一重排**为 I+1。
//       两者对"追加第 N 行"的结果恰好一致，但对"删除中间行后再添加"的序号规则不同 —— 差异断言见测试。
//   D5. `btnSearchClick` 用 `Pos(UpperCase(搜索文本), UpperCase(字段)) > 0`：
//       子串匹配（不是前缀），且 `UpperCase` 是 Delphi AnsiUpperCase（此处用 ToUpperInvariant 等价）。
//   D6. 搜索字段下拉：`cbbSearchField.ItemIndex` 为 0/1 之外时 `IsFound` 恒 False（case 无 else）。
// =====================================================================================

/// <summary>uFrmProcessBlacklist.pas 的列表行载体。</summary>
public class ProcessBlacklistRow
{
    public string Caption = "";        // 原 :55 Item.Caption := IntToStr(I + 1)
    public string ProcessName = "";    // 原 :57 SubItems.Add(ProcessInfo.ProcessName)
    public string ProcessMD5 = "";     // 原 :58 SubItems.Add(ProcessInfo.ProcessMD5)
    public TProcessInfo Data;          // 原 :56 Item.Data := ProcessInfo
}

/// <summary>uFrmProcessBlacklist.pas 的非 UI 逻辑。</summary>
public static class ProcessBlacklistLogic
{
    /// <summary>原 :50-59 `ShowFrmProcessBlacklist` 的列表填充（序号从 1 开始）。</summary>
    public static List<ProcessBlacklistRow> BuildRows(TProcessBlacklist list)
    {
        var rows = new List<ProcessBlacklistRow>();
        for (int i = 0; i < list.Count; i++)                       // 原 :50
        {
            var info = list.Items[i];                              // 原 :52
            rows.Add(new ProcessBlacklistRow
            {
                Caption = DelphiRTL.IntToStr(i + 1),               // 原 :55
                Data = info,                                       // 原 :56
                ProcessName = info.ProcessName,                    // 原 :57
                ProcessMD5 = info.ProcessMD5                       // 原 :58
            });
        }
        return rows;
    }

    /// <summary>原 :61 `btnAdd.Enabled := g_ProcessBlackList.Count &lt; g_ProcessBlackList.MaxCount;`。</summary>
    public static bool CanAdd(TProcessBlacklist list) => list.Count < list.MaxCount;

    /// <summary>
    /// 原 :68-96 `btnSearchClick` / :98-131 `btnSearchNextClick` 的**唯一实质逻辑**：
    ///   IsFound := Pos(UpperCase(edtSearchText.Text), UpperCase(SubItems[ItemIndex])) &gt; 0
    /// 其中 `fieldIndex` 为 cbbSearchField.ItemIndex（0=进程名，1=MD5，其它=不匹配）。
    /// 返回命中的行下标，未命中返回 -1。
    /// </summary>
    public static int FindIndex(IReadOnlyList<ProcessBlacklistRow> rows, int startIndex, int fieldIndex, string searchText)
    {
        // 原 :80/:115 `if Item.SubItems.Count >= 2 then` —— 我方行结构恒有 2 个 SubItem，等价恒真。
        string needle = DelphiRTL.UpperCase(searchText ?? "");
        for (int i = startIndex; i < rows.Count; i++)                                       // 原 :75 / :110
        {
            bool isFound = false;                                                           // 原 :77 / :112
            // ★ GXX.Core.Rtl.DelphiRTL.Pos 对空子串返回 0（Delphi 的 `Pos('', S)` 返回 1）。
            //   为保持原语义，这里显式把"空搜索串"视为命中（Pos 恒 > 0）。
            if (needle.Length == 0)
                isFound = true;
            else if (fieldIndex == 0)                                                       // 原 :83（进程）
                isFound = DelphiRTL.Pos(needle, DelphiRTL.UpperCase(rows[i].ProcessName)) > 0;
            else if (fieldIndex == 1)                                                       // 原 :84（MD5）
                isFound = DelphiRTL.Pos(needle, DelphiRTL.UpperCase(rows[i].ProcessMD5)) > 0;
            // 原 case 无 else → 其它 fieldIndex 时 isFound 保持 False（空串情形下 fieldIndex 也必须合法）
            if (needle.Length == 0 && fieldIndex != 0 && fieldIndex != 1)
                isFound = false;

            if (isFound) return i;                                                          // 原 :88-94 / :123-129
        }
        return -1;
    }

    /// <summary>
    /// 原 :104-108 `btnSearchNextClick` 的起始下标计算（含回绕）：
    ///   StartIndex := lvProcessBlacklist.ItemIndex + 1;
    ///   if StartIndex &lt; 0 then StartIndex := 0
    ///   else if StartIndex >= Items.Count then StartIndex := 0;
    /// ★ 注意：`ItemIndex = -1`（未选中）时 StartIndex = 0 → 从头搜；
    ///   `ItemIndex = Count - 1`（最后一行）时 StartIndex = Count → 回绕到 0。
    /// </summary>
    public static int NextSearchStart(int currentItemIndex, int itemCount)
    {
        int startIndex = currentItemIndex + 1;      // 原 :104
        if (startIndex < 0) startIndex = 0;         // 原 :105-106
        else if (startIndex >= itemCount) startIndex = 0;   // 原 :107-108
        return startIndex;
    }

    /// <summary>原 :142-160 `btnAddClick` 成功后追加行的序号：`IntToStr(Items.Count)`（**Add 之后**的 Count）。</summary>
    public static string NewRowCaptionAfterAdd(int itemCountAfterAdd)
        => DelphiRTL.IntToStr(itemCountAfterAdd);   // 原 :151

    /// <summary>原 :193-196 `mniDeleteClick` 删除后的序号重排：`Items[I].Caption := IntToStr(I + 1)`。</summary>
    public static void NormalizeCaptions(IReadOnlyList<ProcessBlacklistRow> rows)
    {
        for (int i = 0; i < rows.Count; i++)                                    // 原 :193
            rows[i].Caption = DelphiRTL.IntToStr(i + 1);                        // 原 :195
    }

    /// <summary>原 :162-171 `pmDeletePopup`：只有选中行且 Data 非 nil 时才显示"删除进程"。</summary>
    public static bool CanDelete(ProcessBlacklistRow selected)
        => selected != null && selected.Data != null;

    /// <summary>同上，但接受"选中行下标 + 行集合"（托管侧 ListView.SelectedItems 在无句柄时为空）。</summary>
    public static bool CanDelete(IReadOnlyList<ProcessBlacklistRow> rows, int selectedIndex)
        => selectedIndex >= 0 && selectedIndex < rows.Count && CanDelete(rows[selectedIndex]);
}

/// <summary>接缝：待 GateShare.pas 的 `SaveProcessBlacklist` / `RebuildProcessBlacklist` 移植后接入。
/// 原文（GateShare.pas:1943-1949 附近）会 zLib 压缩 + RSA 加密 + MD5，再清空缓存；此处只暴露触发点。</summary>
public interface IProcessBlacklistSink
{
    void SaveProcessBlacklist();
    void RebuildProcessBlacklist();
}

/// <summary>原 :41-66 `procedure ShowFrmProcessBlacklist;`。</summary>
public static class ProcessBlacklistUnit
{
    /// <summary>可注入的 Save/Rebuild 接缝（默认空实现，等价于原文那两步的副作用在别处）。</summary>
    public static IProcessBlacklistSink Sink;

    public static void ShowFrmProcessBlacklist()
    {
        using var form = new FrmProcessBlacklist();
        form.FillList();                                           // 原 :50-59
        form.btnAdd.Enabled = ProcessBlacklistLogic.CanAdd(FormGlobals.g_ProcessBlackList);   // 原 :61
        form.ShowDialog();                                         // 原 :62
    }
}

/// <summary>原 uFrmProcessBlacklist.pas:10-33 `TFrmProcessBlacklist`（DFM: uFrmProcessBlacklist.dfm）。</summary>
public class FrmProcessBlacklist : Form
{
    // DFM: FrmProcessBlacklist Left=350 Top=258 BorderStyle=bsDialog Caption='进程黑名单'
    //      ClientHeight=441 ClientWidth=622 Font.Charset=GB2312_CHARSET Font.Height=-13 Font.Name='宋体'
    //      Position=poMainFormCenter PixelsPerInch=96
    public Label lbl1;                  // DFM: lbl1 Left=7 Top=10 Width=65 Height=13 Caption='搜索字段：'
    public Label lbl2;                  // DFM: lbl2 Left=143 Top=10 Width=65 Height=13 Caption='搜索内容：'
    public Label lbl3;                  // DFM: lbl3 Left=5 Top=422 Width=183 Height=13 Caption='说明：进程列表最多只支持80个'
    /// <summary>原 :32-33 的列表控件。
    /// DFM: lvProcessBlacklist Left=6 Top=32 Width=610 Height=385
    ///      Columns=[序号,进程名(200),MD5(330)] GridLines=True ReadOnly=True RowSelect=True ViewStyle=vsReport
    ///      ColumnClick=False PopupMenu=pmDelete TabOrder=4</summary>
    public ListView lvProcessBlacklist;

    /// <summary>
    /// 测试可见的"当前选中行"镜像。WinForms `ListView.SelectedIndices` 需要控件已创建句柄
    /// （无消息循环的单元测试里恒为空），而原文 `lvProcessBlacklist.ItemIndex` 是纯数据。
    /// 故这里在每次改变选中时同步维护一个索引镜像，读取时优先用它。
    /// **不改变原有 UI 语义**：真实运行时它始终等于 `SelectedIndices[0]`。
    /// </summary>
    public int SelectedRowMirror = -1;

    /// <summary>测试辅助/内部使用：读当前选中行（优先控件的真实选中，其次镜像）。</summary>
    public int CurrentSelectedRow
        => lvProcessBlacklist.SelectedIndices.Count > 0 ? lvProcessBlacklist.SelectedIndices[0] : SelectedRowMirror;

    /// <summary>当前选中行（原文 `lvProcessBlacklist.ItemIndex`；无选中为 -1）。</summary>
    public int SelectedRow => CurrentSelectedRow;

    /// <summary>设置选中行（原文 `lvProcessBlacklist.ItemIndex := N`）。</summary>
    public void SetSelectedRow(int index)
    {
        SelectedRowMirror = index;
        if (index >= 0 && index < lvProcessBlacklist.Items.Count)
            lvProcessBlacklist.Items[index].Selected = true;
    }

    /// <summary>选中并确保可见（原 `Selected := Item; MakeVisible(True); SetFocus`）。</summary>
    public void SelectRowAndMakeVisible(int index)
    {
        SelectedRowMirror = index;                                   // 原 :90 / :125
        if (index >= 0 && index < lvProcessBlacklist.Items.Count)
        {
            lvProcessBlacklist.Items[index].Selected = true;
            lvProcessBlacklist.Items[index].Focused = true;
            lvProcessBlacklist.Items[index].EnsureVisible();          // 原 :91 MakeVisible(True)
        }
        lvProcessBlacklist.Focus();                                   // 原 :92 SetFocus
    }

    public ComboBox cbbSearchField;     // DFM: cbbSearchField Left=68 Top=6 Width=65 Height=21 Style=csDropDownList ItemIndex=0 Text='进程' Items=('进程','MD5') TabOrder=0
    public TextBox edtSearchText;       // DFM: edtSearchText Left=204 Top=6 Width=181 Height=21 TabOrder=1 OnKeyDown=edtSearchTextKeyDown
    public Button btnSearch;            // DFM: btnSearch Left=388 Top=5 Width=53 Height=22 Caption='搜索' TabOrder=2 OnClick=btnSearchClick
    public Button btnSearchNext;        // DFM: btnSearchNext Left=446 Top=5 Width=71 Height=22 Caption='搜索下一个' TabOrder=3 OnClick=btnSearchNextClick
    public Button btnAdd;               // DFM: btnAdd Left=545 Top=5 Width=71 Height=22 Caption='添加' TabOrder=5 OnClick=btnAddClick
    public ContextMenuStrip pmDelete;   // DFM: pmDelete (TPopupMenu) OnPopup=pmDeletePopup
    public ToolStripMenuItem mniDelete; // DFM: mniDelete Caption='删除进程' OnClick=mniDeleteClick

    public FrmProcessBlacklist()
    {
        // DFM: FrmProcessBlacklist Caption='进程黑名单' BorderStyle=bsDialog
        Text = "进程黑名单";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;     // Position=poMainFormCenter
        Location = new Point(350, 258);
        ClientSize = new Size(622, 441);
        Font = new Font("宋体", 9.75F);                      // Font.Height=-13 Font.Name='宋体'
        MaximizeBox = false;
        MinimizeBox = false;

        // DFM: lbl1 Left=7 Top=10 Width=65 Height=13 Caption='搜索字段：'
        lbl1 = new Label { Left = 7, Top = 10, Width = 65, Height = 13, Text = "搜索字段：" };
        // DFM: lbl2 Left=143 Top=10 Width=65 Height=13 Caption='搜索内容：'
        lbl2 = new Label { Left = 143, Top = 10, Width = 65, Height = 13, Text = "搜索内容：" };
        // DFM: lbl3 Left=5 Top=422 Width=183 Height=13 Caption='说明：进程列表最多只支持80个'
        lbl3 = new Label { Left = 5, Top = 422, Width = 183, Height = 13, Text = "说明：进程列表最多只支持80个" };

        // DFM: lvProcessBlacklist Left=6 Top=32 Width=610 Height=385 Columns=[序号,进程名 Width=200,MD5 Width=330]
        //      GridLines=True ReadOnly=True RowSelect=True ViewStyle=vsReport ColumnClick=False PopupMenu=pmDelete
        lvProcessBlacklist = new ListView
        {
            Left = 6, Top = 32, Width = 610, Height = 385, TabIndex = 4,
            View = View.Details,
            GridLines = true,
            MultiSelect = false,
            FullRowSelect = true,                 // RowSelect=True
            HideSelection = false
        };
        lvProcessBlacklist.Columns.Add("序号");                       // DFM: Columns[0] Caption='序号'
        lvProcessBlacklist.Columns.Add("进程名", 200);                // DFM: Columns[1] Caption='进程名' Width=200
        lvProcessBlacklist.Columns.Add("MD5", 330);                   // DFM: Columns[2] Caption='MD5' Width=330

        // DFM: cbbSearchField Left=68 Top=6 Width=65 Height=21 Style=csDropDownList ItemIndex=0 Text='进程'
        cbbSearchField = new ComboBox { Left = 68, Top = 6, Width = 65, Height = 21, TabIndex = 0,
                                        DropDownStyle = ComboBoxStyle.DropDownList };
        cbbSearchField.Items.AddRange(new object[] { "进程", "MD5" });
        cbbSearchField.SelectedIndex = 0;                             // DFM: ItemIndex=0
        // DFM: edtSearchText Left=204 Top=6 Width=181 Height=21 TabOrder=1 OnKeyDown=edtSearchTextKeyDown
        edtSearchText = new TextBox { Left = 204, Top = 6, Width = 181, Height = 21, TabIndex = 1 };
        // DFM: btnSearch Left=388 Top=5 Width=53 Height=22 Caption='搜索' TabOrder=2
        btnSearch = new Button { Left = 388, Top = 5, Width = 53, Height = 22, Text = "搜索", TabIndex = 2 };
        // DFM: btnSearchNext Left=446 Top=5 Width=71 Height=22 Caption='搜索下一个' TabOrder=3
        btnSearchNext = new Button { Left = 446, Top = 5, Width = 71, Height = 22, Text = "搜索下一个", TabIndex = 3 };
        // DFM: btnAdd Left=545 Top=5 Width=71 Height=22 Caption='添加' TabOrder=5
        btnAdd = new Button { Left = 545, Top = 5, Width = 71, Height = 22, Text = "添加", TabIndex = 5 };

        // DFM: pmDelete (TPopupMenu) OnPopup=pmDeletePopup；mniDelete Caption='删除进程'
        pmDelete = new ContextMenuStrip();
        mniDelete = new ToolStripMenuItem("删除进程");
        pmDelete.Items.Add(mniDelete);
        lvProcessBlacklist.ContextMenuStrip = pmDelete;               // DFM: lvProcessBlacklist PopupMenu=pmDelete

        Controls.AddRange(new Control[] { lbl1, lbl2, lbl3, lvProcessBlacklist, cbbSearchField,
                                          edtSearchText, btnSearch, btnSearchNext, btnAdd });

        // DFM 的事件接线
        btnSearch.Click += (s, e) => btnSearch_Click(s, e);
        btnSearchNext.Click += (s, e) => btnSearchNext_Click(s, e);
        edtSearchText.KeyDown += (s, e) => edtSearchText_KeyDown(s, e);
        btnAdd.Click += (s, e) => btnAdd_Click(s, e);
        pmDelete.Opening += (s, e) => pmDelete_Popup(s, e);
        mniDelete.Click += (s, e) => mniDelete_Click(s, e);
    }

    /// <summary>原 uFrmProcessBlacklist.pas:50-59 的列表填充。</summary>
    public void FillList()
    {
        lvProcessBlacklist.Items.Clear();
        SelectedRowMirror = -1;                                  // 重建列表后无选中（原 :52 的 I 循环不设 ItemIndex）
        foreach (var row in ProcessBlacklistLogic.BuildRows(FormGlobals.g_ProcessBlackList))
        {
            var item = new ListViewItem(row.Caption);       // 原 :54-55 Item.Caption
            item.Tag = row;                                  // 原 :56 Item.Data := ProcessInfo
            item.SubItems.Add(row.ProcessName);              // 原 :57
            item.SubItems.Add(row.ProcessMD5);               // 原 :58
            lvProcessBlacklist.Items.Add(item);
        }
    }

    /// <summary>测试辅助：把 ListView 读成纯逻辑行列表。</summary>
    public List<ProcessBlacklistRow> ReadRows()
    {
        var rows = new List<ProcessBlacklistRow>();
        foreach (ListViewItem it in lvProcessBlacklist.Items)
        {
            var row = it.Tag as ProcessBlacklistRow ?? new ProcessBlacklistRow();
            row.Caption = it.Text;
            row.ProcessName = it.SubItems.Count > 1 ? it.SubItems[1].Text : "";
            row.ProcessMD5 = it.SubItems.Count > 2 ? it.SubItems[2].Text : "";
            rows.Add(row);
        }
        return rows;
    }

    /// <summary>原 uFrmProcessBlacklist.pas:68-96 `btnSearchClick`（起始下标恒 0）。</summary>
    public void btnSearch_Click(object sender, EventArgs e)
    {
        var rows = ReadRows();
        int hit = ProcessBlacklistLogic.FindIndex(rows, 0, cbbSearchField.SelectedIndex, edtSearchText.Text);   // 原 :74-95
        if (hit >= 0) SelectRowAndMakeVisible(hit);
    }

    /// <summary>原 uFrmProcessBlacklist.pas:98-131 `btnSearchNextClick`（起点 = 当前选中 +1，回绕）。</summary>
    public void btnSearchNext_Click(object sender, EventArgs e)
    {
        var rows = ReadRows();
        int current = CurrentSelectedRow;   // 原 :104 lvProcessBlacklist.ItemIndex
        int start = ProcessBlacklistLogic.NextSearchStart(current, rows.Count);                                   // 原 :104-108
        int hit = ProcessBlacklistLogic.FindIndex(rows, start, cbbSearchField.SelectedIndex, edtSearchText.Text);  // 原 :110-130
        if (hit >= 0) SelectRowAndMakeVisible(hit);
    }

    /// <summary>原 uFrmProcessBlacklist.pas:133-140 `edtSearchTextKeyDown`：VK_RETURN 且文本非空 → 触发搜索。</summary>
    public void edtSearchText_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Return && edtSearchText.Text.Length > 0)     // 原 :136
        {
            btnSearch_Click(btnSearch, EventArgs.Empty);                  // 原 :138 btnSearch.Click
        }
    }

    /// <summary>原 uFrmProcessBlacklist.pas:142-160 `btnAddClick`。</summary>
    public void btnAdd_Click(object sender, EventArgs e)
    {
        if (AddProcessBlackUnit.ShowAddProcessBlack(out TProcessInfo processInfo))   // 原 :147
        {
            var row = new ProcessBlacklistRow
            {
                Data = processInfo,                                                   // 原 :150
                // 原 :151：Item.Caption := IntToStr(lvProcessBlacklist.Items.Count)（Add 之前的 Count，
                // 而 TListItem 是 Add 的返回值，故等价于"Add 之后的行数"）
                Caption = ProcessBlacklistLogic.NewRowCaptionAfterAdd(lvProcessBlacklist.Items.Count + 1),
                ProcessName = processInfo.ProcessName,                                // 原 :152
                ProcessMD5 = DelphiRTL.UpperCase(processInfo.ProcessMD5)              // 原 :153（**大写**，与 ShowFrm 不同）
            };
            var item = new ListViewItem(row.Caption);
            item.Tag = row;
            item.SubItems.Add(row.ProcessName);
            item.SubItems.Add(row.ProcessMD5);
            lvProcessBlacklist.Items.Add(item);

            ProcessBlacklistUnit.Sink?.SaveProcessBlacklist();                        // 原 :155
            ProcessBlacklistUnit.Sink?.RebuildProcessBlacklist();                     // 原 :156

            btnAdd.Enabled = ProcessBlacklistLogic.CanAdd(FormGlobals.g_ProcessBlackList);   // 原 :158
        }
    }

    /// <summary>原 :162-171 `pmDeletePopup` 的决策结果（托管侧镜像，便于单测；
    /// 因 Windows 的 `ToolStripItem.Visible` 在所属菜单未弹出时会被强制为 false，不能直接断言）。</summary>
    public bool DeleteMenuItemShouldBeVisible;

    /// <summary>原 uFrmProcessBlacklist.pas:162-171 `pmDeletePopup`。</summary>
    public void pmDelete_Popup(object sender, EventArgs e)
    {
        var rows = ReadRows();
        bool visible = ProcessBlacklistLogic.CanDelete(rows, SelectedRow);   // 原 :164
        DeleteMenuItemShouldBeVisible = visible;
        mniDelete.Visible = visible;                        // 原 :166 / :170
    }

    /// <summary>原 uFrmProcessBlacklist.pas:173-199 `mniDeleteClick`。</summary>
    public void mniDelete_Click(object sender, EventArgs e)
    {
        int idx = SelectedRow;
        if (idx < 0 || idx >= lvProcessBlacklist.Items.Count) return;                   // 原 :177
        var row = lvProcessBlacklist.Items[idx].Tag as ProcessBlacklistRow;
        if (row?.Data == null) return;                                                 // 原 :177

        // 原 :182-187
        FormGlobals.g_ProcessBlackList.Lock();
        try
        {
            FormGlobals.g_ProcessBlackList.Delete(row.Data);                           // 原 :184
        }
        finally
        {
            FormGlobals.g_ProcessBlackList.UnLock();                                   // 原 :186
        }

        ProcessBlacklistUnit.Sink?.SaveProcessBlacklist();                             // 原 :189
        ProcessBlacklistUnit.Sink?.RebuildProcessBlacklist();                          // 原 :190

        lvProcessBlacklist.Items.RemoveAt(idx);                                       // 原 :192 DeleteSelected
        SelectedRowMirror = -1;                                                        // 删除后无选中

        // 原 :193-196 序号重排
        for (int i = 0; i < lvProcessBlacklist.Items.Count; i++)
            lvProcessBlacklist.Items[i].Text = DelphiRTL.IntToStr(i + 1);

        btnAdd.Enabled = ProcessBlacklistLogic.CanAdd(FormGlobals.g_ProcessBlackList);   // 原 :198
    }
}
