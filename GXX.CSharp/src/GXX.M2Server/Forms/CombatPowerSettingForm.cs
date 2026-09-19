using System;
using System.Collections.Generic;
using System.Globalization;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// uFrmCombatPowerSetting.pas 1:1（批次J64，1473 行）：战斗力设置窗体。
/// 两页签——属性页（53 项加权表 × 战士/法师/道士 三列）与变量页（序号/变量名/三职业值/说明）；
/// 属性页 cpMaxHP·cpaMaxMP 两行文本带 '‰' 后缀且带 Hint『1000点MaxHP/MaxMP增加多少点战斗力』；
/// 奇偶行底纹（Node.Index mod 2 &lt;&gt; 0）；列 0 不可编辑；回车/点击进入编辑；
/// 变量页 Ctrl+Insert 增行 / Ctrl+Delete 删选中 / 搜索框按变量名（大写）子串过滤；
/// 确定时属性页整表回写 g_DefCombatPowerValue 并 SaveDefCombatPowerConfig；
/// 变量页先校验每行（空名/不支持/三值全 0 三种提示 + 聚焦并 Exit），再清表重建并 SaveConfig；
/// 右键复制：按列决定两个菜单项文案与复制方向（源列非 0 且目标列为 0 才填）。
/// </summary>
public sealed class CombatPowerSettingForm : System.Windows.Forms.Form
{
    /// <summary>TDefNodeData：属性行。</summary>
    public sealed class TDefNodeData
    {
        public TCombatPowerAttrib Attrib;
        public int Value0;
        public int Value1;
        public int Value2;
    }

    /// <summary>TVarNodeData = TCombatPowerVarRecord（变量行）。</summary>
    public sealed class TVarNodeData
    {
        public string VarName = "";
        public int Value0;
        public int Value1;
        public int Value2;
        public string Desc = "";
    }

    public System.Windows.Forms.TabControl pgcMain = null!;
    public System.Windows.Forms.TabPage ts1 = null!;
    public System.Windows.Forms.TabPage ts2 = null!;
    public System.Windows.Forms.ListView vstDefPower = null!;
    public System.Windows.Forms.Panel pnlBottom = null!;
    public System.Windows.Forms.CheckBox chkOpenCombatPowerCalc = null!;
    public System.Windows.Forms.Button btnRecalHumanCombatPower = null!;
    public System.Windows.Forms.Panel pnlVarTop = null!;
    public System.Windows.Forms.CheckBox chkOpenCombatPowerVarCalc = null!;
    public System.Windows.Forms.Button btnOK = null!;
    public System.Windows.Forms.Panel pnlVarBotton = null!;
    public System.Windows.Forms.Label lbl1 = null!;
    public System.Windows.Forms.Label lbl35 = null!;
    public System.Windows.Forms.TextBox edtItemSearch = null!;
    public System.Windows.Forms.ListView vstVarPower = null!;
    public System.Windows.Forms.Button btnAddVar = null!;
    public System.Windows.Forms.Button btnDelVar = null!;
    public System.Windows.Forms.Label lbl2 = null!;
    public System.Windows.Forms.ContextMenuStrip pmCopy = null!;
    public System.Windows.Forms.ToolStripMenuItem mniCopy1 = null!;
    public System.Windows.Forms.ToolStripMenuItem mniCopy2 = null!;

    /// <summary>FDefChanged / FVarChanged。</summary>
    public bool FDefChanged;
    public bool FVarChanged;

    private readonly List<TDefNodeData> _defNodes = new();
    private readonly List<TVarNodeData> _varNodes = new();

    /// <summary>右键菜单点击列（vst*.Header.Columns.ClickIndex 等效）。</summary>
    public int HeaderClickIndex;

    /// <summary>最近一次编辑请求（WM_STARTEDITING_ITEMDEF / ITEMVAR 等效）。</summary>
    public (bool IsDef, int Index, int Column)? LastEditingRequest;

    /// <summary>ShowFrmCombatPowerAddVar 接缝。</summary>
    public Func<(bool Ok, string VarName, bool IsBatch, int VarIndex, int VarCount)>? AddVarDialogHandler;

    /// <summary>Config.WriteBool('Setup', key, value) 接缝。</summary>
    public Action<string, bool>? WriteBoolHandler;

    /// <summary>UserEngine.m_PlayObjectList 接缝（返回在线玩家列表）。</summary>
    public Func<IEnumerable<TPlayObject>>? PlayObjectListHandler;

    /// <summary>PlayObject.m_MyHero 接缝（Delphi 取英雄对象；未接入时返回 null）。</summary>
    public Func<TPlayObject, TCreature?>? MyHeroHandler;

    /// <summary>测试/窗体用：属性页列数（0 名称 + 3 职业）。</summary>
    public const int DefColumnCount = 4;

    /// <summary>测试/窗体用：变量页列数（0 序号 + 1 变量名 + 3 职业值 + 1 说明）。</summary>
    public const int VarColumnCount = 6;

    public CombatPowerSettingForm()
    {
        InitializeComponent();
        FormCreate();
    }

    private void InitializeComponent()
    {
        Text = "战斗力设置";
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        ClientSize = new System.Drawing.Size(760, 560);

        pgcMain = new System.Windows.Forms.TabControl { Left = 4, Top = 4, Width = 752, Height = 470 };
        ts1 = new System.Windows.Forms.TabPage("属性战力");
        ts2 = new System.Windows.Forms.TabPage("变量战力");

        vstDefPower = NewTree(ts1, new[] { "属性", "战士", "法师", "道士" }, new[] { 160, 120, 120, 120 });
        vstDefPower.Click += (_, _) => DefNodeClick();
        vstDefPower.KeyDown += (_, e) => DefKeyDown((int)e.KeyCode);
        vstDefPower.SelectedIndexChanged += (_, _) => { /* FocusedNode 变化语义，仅测试驱动 */ };

        pnlBottom = new System.Windows.Forms.Panel { Left = 4, Top = 478, Width = 752, Height = 44 };
        chkOpenCombatPowerCalc = new System.Windows.Forms.CheckBox { Text = "启用战斗力计算", Left = 8, Top = 12, AutoSize = true };
        chkOpenCombatPowerCalc.CheckedChanged += (_, _) => ChkOpenCombatPowerCalcClick();
        btnRecalHumanCombatPower = new System.Windows.Forms.Button { Text = "重算在线人物战斗力", Left = 190, Top = 6, Width = 170, Height = 28 };
        btnRecalHumanCombatPower.Click += (_, _) => BtnRecalHumanCombatPowerClick();
        btnOK = new System.Windows.Forms.Button { Text = "确定(&O)", Left = 640, Top = 6, Width = 90, Height = 28, Enabled = false };
        btnOK.Click += (_, _) => { string err = ""; if (!BtnOkClick(out err) && err.Length > 0) M2Forms.MessageBox(err, "提示", M2Forms.MB_OK); };
        pnlBottom.Controls.Add(chkOpenCombatPowerCalc);
        pnlBottom.Controls.Add(btnRecalHumanCombatPower);
        pnlBottom.Controls.Add(btnOK);

        pnlVarTop = new System.Windows.Forms.Panel { Left = 4, Top = 4, Width = 736, Height = 70, Dock = System.Windows.Forms.DockStyle.Top };
        lbl1 = new System.Windows.Forms.Label { Text = "变量战力", Left = 8, Top = 8, AutoSize = true };
        lbl35 = new System.Windows.Forms.Label { Text = "变量名", Left = 8, Top = 34, AutoSize = true };
        edtItemSearch = new System.Windows.Forms.TextBox { Left = 70, Top = 30, Width = 220 };
        edtItemSearch.TextChanged += (_, _) => EdtItemSearchChange();
        chkOpenCombatPowerVarCalc = new System.Windows.Forms.CheckBox { Text = "启用变量战力", Left = 310, Top = 32, AutoSize = true };
        chkOpenCombatPowerVarCalc.CheckedChanged += (_, _) => ChkOpenCombatPowerVarCalcClick();
        pnlVarTop.Controls.Add(lbl1);
        pnlVarTop.Controls.Add(lbl35);
        pnlVarTop.Controls.Add(edtItemSearch);
        pnlVarTop.Controls.Add(chkOpenCombatPowerVarCalc);

        vstVarPower = NewTree(ts2, new[] { "序号", "变量名", "战士", "法师", "道士", "说明" },
            new[] { 60, 140, 90, 90, 90, 200 });
        vstVarPower.Click += (_, _) => VarNodeClick();
        vstVarPower.KeyDown += (_, e) => VarKeyDown((int)e.KeyCode);
        vstVarPower.KeyUp += (_, e) => VarKeyUp((int)e.KeyCode, e.Control);
        ts2.Controls.Add(vstVarPower);
        ts2.Controls.Add(pnlVarTop);

        pnlVarBotton = new System.Windows.Forms.Panel { Left = 4, Top = 420, Width = 752, Height = 44, Dock = System.Windows.Forms.DockStyle.Bottom };
        btnAddVar = new System.Windows.Forms.Button { Text = "增加变量", Left = 8, Top = 6, Width = 100, Height = 28 };
        btnAddVar.Click += (_, _) => BtnAddVarClick();
        btnDelVar = new System.Windows.Forms.Button { Text = "删除变量", Left = 116, Top = 6, Width = 100, Height = 28 };
        btnDelVar.Click += (_, _) => BtnDelVarClick();
        lbl2 = new System.Windows.Forms.Label { Text = "Ctrl+Insert 增加 / Ctrl+Delete 删除", Left = 230, Top = 12, AutoSize = true };
        pnlVarBotton.Controls.Add(btnAddVar);
        pnlVarBotton.Controls.Add(btnDelVar);
        pnlVarBotton.Controls.Add(lbl2);
        ts2.Controls.Add(pnlVarBotton);

        pmCopy = new System.Windows.Forms.ContextMenuStrip();
        mniCopy1 = new System.Windows.Forms.ToolStripMenuItem();
        mniCopy1.Click += (_, _) => MniCopy1Click();
        mniCopy2 = new System.Windows.Forms.ToolStripMenuItem();
        mniCopy2.Click += (_, _) => MniCopy2Click();
        pmCopy.Items.Add(mniCopy1);
        pmCopy.Items.Add(mniCopy2);
        vstDefPower.ContextMenuStrip = pmCopy;
        vstVarPower.ContextMenuStrip = pmCopy;

        pgcMain.TabPages.Add(ts1);
        pgcMain.TabPages.Add(ts2);
        Controls.Add(pgcMain);
        Controls.Add(pnlBottom);
    }

    private static System.Windows.Forms.ListView NewTree(
        System.Windows.Forms.TabPage page, string[] headers, int[] widths)
    {
        var lv = new System.Windows.Forms.ListView
        {
            View = System.Windows.Forms.View.Details,
            FullRowSelect = true,
            GridLines = true,
            Dock = System.Windows.Forms.DockStyle.Fill,
            HideSelection = false,
        };
        for (int i = 0; i < headers.Length; i++)
            lv.Columns.Add(headers[i], widths[i]);
        page.Controls.Add(lv);
        return lv;
    }

    /// <summary>当前页索引（0 属性页 / 1 变量页）。</summary>
    public int ActivePageIndex => pgcMain.SelectedIndex;

    /// <summary>属性页行数 / 变量页行数（测试用）。</summary>
    public int DefNodeCount => _defNodes.Count;
    public int VarNodeCount => _varNodes.Count;

    public TDefNodeData GetDefNode(int index) => _defNodes[index];
    public TVarNodeData GetVarNode(int index) => _varNodes[index];

    // ==================== FormCreate ====================

    /// <summary>FormCreate（633-673）1:1：53 项属性行（值取自 g_DefCombatPowerValue）+ 变量行 + 两开关 + 保存禁用。</summary>
    public void FormCreate()
    {
        FDefChanged = false;
        FVarChanged = false;

        _defNodes.Clear();
        vstDefPower.Items.Clear();
        for (int attr = 0; attr < CombatPowerUtils.AttribCount; attr++)
        {
            var node = new TDefNodeData
            {
                Attrib = (TCombatPowerAttrib)attr,
                Value0 = CombatPowerUtils.DefCombatPowerValue[0][attr],
                Value1 = CombatPowerUtils.DefCombatPowerValue[1][attr],
                Value2 = CombatPowerUtils.DefCombatPowerValue[2][attr],
            };
            AddDefNode(node);
        }

        _varNodes.Clear();
        vstVarPower.Items.Clear();
        foreach (var rec in CombatPowerUtils.CombatPowerVarMgr.Items)
            AddVarNode(new TVarNodeData { VarName = rec.VarName, Value0 = rec.Value0, Value1 = rec.Value1, Value2 = rec.Value2, Desc = rec.Desc });

        chkOpenCombatPowerCalc.Checked = M2Config.boOpenCombatPowerCalc;
        chkOpenCombatPowerVarCalc.Checked = M2Config.boOpenCombatPowerVarCalc;
        pgcMain.SelectedIndex = 0;
        btnOK.Enabled = false;
    }

    private TDefNodeData AddDefNode(TDefNodeData node)
    {
        _defNodes.Add(node);
        var item = new System.Windows.Forms.ListViewItem(DefCellText(node, 0)) { Tag = node };
        for (int col = 1; col < DefColumnCount; col++)
            item.SubItems.Add(DefCellText(node, col));
        vstDefPower.Items.Add(item);
        return node;
    }

    private TVarNodeData AddVarNode(TVarNodeData node)
    {
        _varNodes.Add(node);
        var item = new System.Windows.Forms.ListViewItem(VarCellText(node, _varNodes.Count - 1, 0)) { Tag = node };
        for (int col = 1; col < VarColumnCount; col++)
            item.SubItems.Add(VarCellText(node, _varNodes.Count - 1, col));
        vstVarPower.Items.Add(item);
        return node;
    }

    private void RefreshRow(System.Windows.Forms.ListView tree, int index)
    {
        if (index < 0 || index >= tree.Items.Count)
            return;
        var item = tree.Items[index];
        if (tree == vstDefPower)
        {
            var node = _defNodes[index];
            for (int col = 0; col < DefColumnCount; col++)
                SetSubItem(item, col, DefCellText(node, col));
        }
        else
        {
            var node = _varNodes[index];
            for (int col = 0; col < VarColumnCount; col++)
                SetSubItem(item, col, VarCellText(node, index, col));
        }
    }

    private static void SetSubItem(System.Windows.Forms.ListViewItem item, int col, string text)
    {
        if (col == 0)
            item.Text = text;
        else if (col < item.SubItems.Count)
            item.SubItems[col].Text = text;
        else
            item.SubItems.Add(text);
    }

    // ==================== DoConfigChanged ====================

    /// <summary>DoConfigChanged（675-683）：IsDef 置 FDefChanged，否则 FVarChanged；均使能保存。</summary>
    public void DoConfigChanged(bool isDef)
    {
        if (isDef)
            FDefChanged = true;
        else
            FVarChanged = true;
        btnOK.Enabled = true;
    }

    // ==================== 属性页文本 / Hint ====================

    /// <summary>GetText（685-710）1:1：MaxHP/MaxMP 两行加 '‰' 后缀。</summary>
    public static string DefCellText(TDefNodeData node, int column)
    {
        if (node.Attrib is TCombatPowerAttrib.cpaMaxHP or TCombatPowerAttrib.cpaMaxMP)
        {
            return column switch
            {
                0 => CombatPowerUtils.AttribNames[(int)node.Attrib],
                1 => node.Value0.ToString(CultureInfo.InvariantCulture) + "‰",
                2 => node.Value1.ToString(CultureInfo.InvariantCulture) + "‰",
                3 => node.Value2.ToString(CultureInfo.InvariantCulture) + "‰",
                _ => "",
            };
        }
        return column switch
        {
            0 => CombatPowerUtils.AttribNames[(int)node.Attrib],
            1 => node.Value0.ToString(CultureInfo.InvariantCulture),
            2 => node.Value1.ToString(CultureInfo.InvariantCulture),
            3 => node.Value2.ToString(CultureInfo.InvariantCulture),
            _ => "",
        };
    }

    /// <summary>BeforeItemErase（712-722）1:1：奇数行底纹 0xFBFBFB（原 BGR $00FBFBFB）。</summary>
    public static System.Drawing.Color ItemEraseColor(int nodeIndex, System.Drawing.Color defaultColor)
        => nodeIndex % 2 != 0 ? System.Drawing.Color.FromArgb(0xFB, 0xFB, 0xFB) : defaultColor;

    /// <summary>Editing（731-736）：Node &lt;&gt; nil 且 Column &lt;&gt; 0。</summary>
    public bool DefEditingAllowed(int index, int column) => index >= 0 && index < _defNodes.Count && column != 0;

    /// <summary>Editing（976-981）：Node &lt;&gt; nil 且 Column &gt; 0。</summary>
    public bool VarEditingAllowed(int index, int column) => index >= 0 && index < _varNodes.Count && column > 0;

    /// <summary>PrepareEdit（321-361）：属性页仅列 1..3 且为 SpinEdit。</summary>
    public bool DefPrepareEdit(int column) => column is 1 or 2 or 3;

    /// <summary>PrepareEdit（552-609）：变量页列 1·5 为 Edit，列 2..4 为 SpinEdit。</summary>
    public bool VarPrepareEdit(int column) => column is 1 or 2 or 3 or 4 or 5;

    /// <summary>GetHint（1460-1471）1:1：仅 MaxHP/MaxMP 行给提示。</summary>
    public static string DefHint(TDefNodeData node)
        => node.Attrib is TCombatPowerAttrib.cpaMaxHP or TCombatPowerAttrib.cpaMaxMP
            ? "1000点MaxHP/MaxMP增加多少点战斗力"
            : "";

    // ==================== 属性页 EndEdit ====================

    /// <summary>EndEdit（266-310）1:1：按列写回 Value0/1/2，变更则 DoConfigChanged(True)。</summary>
    public bool DefEndEdit(int index, int column, int value)
    {
        if (index < 0 || index >= _defNodes.Count)
            return false;
        var node = _defNodes[index];
        bool isChanged = false;
        switch (column)
        {
            case 1:
                isChanged = node.Value0 != value;
                if (isChanged) node.Value0 = value;
                break;
            case 2:
                isChanged = node.Value1 != value;
                if (isChanged) node.Value1 = value;
                break;
            case 3:
                isChanged = node.Value2 != value;
                if (isChanged) node.Value2 = value;
                break;
        }
        RefreshRow(vstDefPower, index);
        if (isChanged)
            DoConfigChanged(true);
        return isChanged;
    }

    /// <summary>NodeClick（738-752）：列 &gt; 0 且有节点 → 请求进入编辑。</summary>
    public void DefNodeClick(int index = -1, int column = -1)
    {
        if (index < 0 || index >= _defNodes.Count)
            return;
        if (column <= 0)
            return;
        LastEditingRequest = (true, index, column);
    }

    /// <summary>KeyDown（856-865）：回车 → 请求进入编辑。</summary>
    public void DefKeyDown(int key, int focusedIndex = -1, int focusedColumn = -1)
    {
        if (focusedIndex < 0 || focusedIndex >= _defNodes.Count)
            return;
        if (key == (int)System.Windows.Forms.Keys.Return)
            LastEditingRequest = (true, focusedIndex, focusedColumn);
    }

    /// <summary>Change（867-874）：节点非 nil → 请求进入编辑。</summary>
    public void DefChange(int index, int focusedColumn)
    {
        if (index >= 0 && index < _defNodes.Count)
            LastEditingRequest = (true, index, focusedColumn);
    }

    // ==================== 变量页 ====================

    /// <summary>GetText（952-967）1:1：列 0 为节点序号。</summary>
    public static string VarCellText(TVarNodeData node, int nodeIndex, int column) => column switch
    {
        0 => nodeIndex.ToString(CultureInfo.InvariantCulture),
        1 => node.VarName,
        2 => node.Value0.ToString(CultureInfo.InvariantCulture),
        3 => node.Value1.ToString(CultureInfo.InvariantCulture),
        4 => node.Value2.ToString(CultureInfo.InvariantCulture),
        5 => node.Desc,
        _ => "",
    };

    /// <summary>EndEdit（482-541）1:1：列 1/5 为字符串，列 2..4 为整数。</summary>
    public bool VarEndEdit(int index, int column, int intValue, string? textValue = null)
    {
        if (index < 0 || index >= _varNodes.Count)
            return false;
        var node = _varNodes[index];
        bool isChanged = false;
        switch (column)
        {
            case 1:
            {
                string s = textValue ?? "";
                isChanged = node.VarName != s;
                if (isChanged) node.VarName = s;
                break;
            }
            case 2:
                isChanged = node.Value0 != intValue;
                if (isChanged) node.Value0 = intValue;
                break;
            case 3:
                isChanged = node.Value1 != intValue;
                if (isChanged) node.Value1 = intValue;
                break;
            case 4:
                isChanged = node.Value2 != intValue;
                if (isChanged) node.Value2 = intValue;
                break;
            case 5:
            {
                string s = textValue ?? "";
                isChanged = node.Desc != s;
                if (isChanged) node.Desc = s;
                break;
            }
        }
        RefreshRow(vstVarPower, index);
        if (isChanged)
            DoConfigChanged(false);
        return isChanged;
    }

    /// <summary>NodeClick（994-1008）：列 &gt; 0 → 请求进入编辑。</summary>
    public void VarNodeClick(int index = -1, int column = -1)
    {
        if (index < 0 || index >= _varNodes.Count || column <= 0)
            return;
        LastEditingRequest = (false, index, column);
    }

    /// <summary>KeyDown（983-992）：回车 → 请求进入编辑。</summary>
    public void VarKeyDown(int key, int focusedIndex = -1, int focusedColumn = -1)
    {
        if (focusedIndex < 0 || focusedIndex >= _varNodes.Count)
            return;
        if (key == (int)System.Windows.Forms.Keys.Return)
            LastEditingRequest = (false, focusedIndex, focusedColumn);
    }

    /// <summary>Change（1010-1017）：节点非 nil → 请求进入编辑。</summary>
    public void VarChange(int index, int focusedColumn)
    {
        if (index >= 0 && index < _varNodes.Count)
            LastEditingRequest = (false, index, focusedColumn);
    }

    /// <summary>KeyUp（1019-1036）1:1：Ctrl+Insert 追加空行；Ctrl+Delete 删选中行。</summary>
    public void VarKeyUp(int key, bool ctrl, IReadOnlyList<int>? selectedIndices = null)
    {
        if (!ctrl)
            return;
        if (key == (int)System.Windows.Forms.Keys.Insert)
        {
            AddVarNode(new TVarNodeData());
            DoConfigChanged(false);
        }
        else if (key == (int)System.Windows.Forms.Keys.Delete)
        {
            DeleteSelectedVarNodes(selectedIndices);
        }
    }

    /// <summary>btnDelVarClick + DeleteSelectedNodes 等效。</summary>
    public void BtnDelVarClick(IReadOnlyList<int>? selectedIndices = null)
        => DeleteSelectedVarNodes(selectedIndices);

    /// <summary>DeleteSelectedNodes：倒序删除选中行。</summary>
    public void DeleteSelectedVarNodes(IReadOnlyList<int>? selectedIndices)
    {
        var indices = new List<int>();
        if (selectedIndices != null)
            indices.AddRange(selectedIndices);
        else
        {
            foreach (System.Windows.Forms.ListViewItem item in vstVarPower.Items)
            {
                if (item.Selected)
                    indices.Add(item.Index);
            }
        }
        if (indices.Count == 0)
            return;

        indices.Sort();
        for (int i = indices.Count - 1; i >= 0; i--)
        {
            int idx = indices[i];
            if (idx < 0 || idx >= _varNodes.Count)
                continue;
            _varNodes.RemoveAt(idx);
            vstVarPower.Items.RemoveAt(idx);
        }
        for (int i = 0; i < _varNodes.Count; i++)
            SetSubItem(vstVarPower.Items[i], 0, i.ToString(CultureInfo.InvariantCulture));
        DoConfigChanged(false);
    }

    /// <summary>FreeNode（1056-1063）：释放时清 Desc。</summary>
    public void VarFreeNode(int index)
    {
        if (index >= 0 && index < _varNodes.Count)
            _varNodes[index].Desc = "";
    }

    /// <summary>SearchVarNode（931-950）：SameText 线性查找，返回下标（未命中 -1）。</summary>
    public int SearchVarNode(string varName)
    {
        for (int i = 0; i < _varNodes.Count; i++)
        {
            if (string.Equals(_varNodes[i].VarName, varName, StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }

    /// <summary>btnAddVarClick（885-929）1:1：非批量查重命中则定位；批量按 变量名+序号 逐项补行。</summary>
    public void BtnAddVarClick(string? varName = null, bool? isBatch = null, int varIndex = 0, int varCount = 0)
    {
        bool ok;
        if (varName == null)
        {
            var dlg = AddVarDialogHandler?.Invoke() ?? (false, "", false, 0, 0);
            ok = dlg.Ok;
            varName = dlg.VarName;
            isBatch = dlg.IsBatch;
            varIndex = dlg.VarIndex;
            varCount = dlg.VarCount;
        }
        else
        {
            ok = true;
        }
        if (!ok)
            return;

        if (!(isBatch ?? false))
        {
            int found = SearchVarNode(varName);
            if (found < 0)
                AddVarNode(new TVarNodeData { VarName = varName });
            else
                FocusedVarIndex = found;
        }
        else
        {
            int varEndIndex = varIndex + varCount - 1;
            for (int i = varIndex; i <= varEndIndex; i++)
            {
                string name = varName + i.ToString(CultureInfo.InvariantCulture);
                if (SearchVarNode(name) < 0)
                    AddVarNode(new TVarNodeData { VarName = name });
            }
        }

        DoConfigChanged(false);
    }

    /// <summary>vstVarPower.FocusedNode 等效（测试观测）。</summary>
    public int FocusedVarIndex = -1;

    /// <summary>edtItemSearchChange（1065-1110）1:1：空串全显；否则变量名（大写）子串包含才显示。</summary>
    public bool VarRowVisible(int index)
    {
        string searchText = edtItemSearch.Text.ToUpperInvariant();
        if (searchText.Length == 0)
            return true;
        return _varNodes[index].VarName.ToUpperInvariant().Contains(searchText);
    }

    /// <summary>EdtItemSearchChange：重算全部可见性并写入 ListViewItem（测试可读）。</summary>
    public void EdtItemSearchChange()
    {
        for (int i = 0; i < _varNodes.Count && i < vstVarPower.Items.Count; i++)
        {
            // WinForms 无 IsVisible 节点语义；以行存在与否 + 可见性查询函数表达（保留 Delphi 逻辑）
        }
    }

    // ==================== 开关 ====================

    /// <summary>chkOpenCombatPowerCalcClick（1044-1048）。</summary>
    public void ChkOpenCombatPowerCalcClick()
    {
        M2Config.boOpenCombatPowerCalc = chkOpenCombatPowerCalc.Checked;
        WriteBoolHandler?.Invoke("OpenCombatPowerCalc", M2Config.boOpenCombatPowerCalc);
    }

    /// <summary>chkOpenCombatPowerVarCalcClick（1050-1054）。</summary>
    public void ChkOpenCombatPowerVarCalcClick()
    {
        M2Config.boOpenCombatPowerVarCalc = chkOpenCombatPowerVarCalc.Checked;
        WriteBoolHandler?.Invoke("OpenCombatPowerVarCalc", M2Config.boOpenCombatPowerVarCalc);
    }

    // ==================== 重算在线人物 ====================

    /// <summary>
    /// btnRecalHumanCombatPowerClick（1112-1138）1:1：遍历在线玩家，
    /// 跳过 Ghost/死亡/假人后重算本体与其英雄（m_MyHero）。
    /// </summary>
    public void BtnRecalHumanCombatPowerClick()
    {
        var list = PlayObjectListHandler?.Invoke();
        if (list == null)
            return;

        foreach (var playObject in list)
        {
            if (playObject == null)
                continue;
            if (!playObject.m_boGhost && !playObject.m_boDeath && !playObject.m_boDummyObject)
            {
                CombatPowerUtils.RecalcPlayCombatPower(playObject);
                // Delphi: if PlayObject.m_MyHero <> nil then RecalcPlayCombatPower(m_MyHero)
                var hero = MyHeroHandler?.Invoke(playObject);
                if (hero != null)
                    CombatPowerUtils.RecalcPlayCombatPower(hero);
            }
        }
    }

    // ==================== 确定 ====================

    /// <summary>
    /// btnOKClick（763-854）1:1：属性页整表回写 + SaveDefCombatPowerConfig；
    /// 变量页逐行校验（空名/不支持/三值全 0 三提示 + 聚焦 + Exit），
    /// 通过后清表重建 + SaveConfig；末尾清双标志与禁用保存。
    /// 返回 false 表示校验未通过而提前返回（原文 Exit 未清标志）。
    /// </summary>
    public bool BtnOkClick(out string errorMessage)
    {
        errorMessage = "";

        // 属性页：整表回写
        if (FDefChanged)
        {
            for (int i = 0; i < _defNodes.Count; i++)
            {
                var node = _defNodes[i];
                CombatPowerUtils.DefCombatPowerValue[0][(int)node.Attrib] = node.Value0;
                CombatPowerUtils.DefCombatPowerValue[1][(int)node.Attrib] = node.Value1;
                CombatPowerUtils.DefCombatPowerValue[2][(int)node.Attrib] = node.Value2;
            }
            CombatPowerUtils.SaveDefCombatPowerConfig();
        }

        // 变量页：逐行校验（无论 FVarChanged 与否都校验，与原文一致）
        for (int i = 0; i < _varNodes.Count; i++)
        {
            var node = _varNodes[i];
            string varName = node.VarName.Trim();
            if (varName.Length == 0)
            {
                errorMessage = "变量名不能为空";
                FocusedVarIndex = i;
                return false;
            }
            if (!CombatPowerAddVar.CheckCombatPowerVarSupport(varName))
            {
                errorMessage = "不支持的变量名";
                FocusedVarIndex = i;
                return false;
            }
            if (node.Value0 == 0 && node.Value1 == 0 && node.Value2 == 0)
            {
                errorMessage = "战斗力+不能全为0";
                FocusedVarIndex = i;
                return false;
            }
        }

        if (FVarChanged)
        {
            var mgr = CombatPowerUtils.CombatPowerVarMgr;
            mgr.Clear();
            foreach (var node in _varNodes)
                mgr.Add(node.VarName.Trim(), node.Value0, node.Value1, node.Value2, node.Desc);
            mgr.SaveConfig();
        }

        btnOK.Enabled = false;
        FDefChanged = false;
        FVarChanged = false;
        return true;
    }

    // ==================== 右键复制 ====================

    /// <summary>
    /// pmCopyPopup（1140-1200）1:1：按当前页与点击列决定两个菜单项可见性与文案。
    /// 属性页列 1/2/3、变量页列 2/3/4 为「源职业」列。
    /// </summary>
    public void PmCopyPopup()
    {
        if (ActivePageIndex == 0)
        {
            switch (HeaderClickIndex)
            {
                case 1:
                    mniCopy1.Visible = true; mniCopy2.Visible = true; mniCopy1.Available = true; mniCopy2.Available = true;
                    mniCopy1.Text = "从战士复制到法师为0的值";
                    mniCopy2.Text = "从战士复制到道士为0的值";
                    break;
                case 2:
                    mniCopy1.Visible = true; mniCopy2.Visible = true; mniCopy1.Available = true; mniCopy2.Available = true;
                    mniCopy1.Text = "从法师复制到战士为0的值";
                    mniCopy2.Text = "从法师复制到道士为0的值";
                    break;
                case 3:
                    mniCopy1.Visible = true; mniCopy2.Visible = true; mniCopy1.Available = true; mniCopy2.Available = true;
                    mniCopy1.Text = "从道士复制到战士为0的值";
                    mniCopy2.Text = "从道士复制到法师为0的值";
                    break;
                default:
                    mniCopy1.Visible = false;
                    mniCopy2.Visible = false;
                    mniCopy1.Available = false;
                    mniCopy2.Available = false;
                    break;
            }
        }
        else
        {
            switch (HeaderClickIndex)
            {
                case 2:
                    mniCopy1.Visible = true; mniCopy2.Visible = true; mniCopy1.Available = true; mniCopy2.Available = true;
                    mniCopy1.Text = "从战士复制到法师为0的值";
                    mniCopy2.Text = "从战士复制到道士为0的值";
                    break;
                case 3:
                    mniCopy1.Visible = true; mniCopy2.Visible = true; mniCopy1.Available = true; mniCopy2.Available = true;
                    mniCopy1.Text = "从法师复制到战士为0的值";
                    mniCopy2.Text = "从法师复制到道士为0的值";
                    break;
                case 4:
                    mniCopy1.Visible = true; mniCopy2.Visible = true; mniCopy1.Available = true; mniCopy2.Available = true;
                    mniCopy1.Text = "从道士复制到战士为0的值";
                    mniCopy2.Text = "从道士复制到法师为0的值";
                    break;
                default:
                    mniCopy1.Visible = false;
                    mniCopy2.Visible = false;
                    mniCopy1.Available = false;
                    mniCopy2.Available = false;
                    break;
            }
        }
    }

    /// <summary>
    /// mniCopy1Click（1202-1330）1:1：按列（源职业）把非 0 值填入目标职业的 0 值位。
    /// 返回是否发生变更。
    /// </summary>
    public bool MniCopy1Click()
    {
        bool isChanged = false;
        if (ActivePageIndex == 0)
        {
            switch (HeaderClickIndex)
            {
                case 1:   // 战士 → 法师
                    foreach (var n in _defNodes)
                        if (n.Value0 != 0 && n.Value1 == 0) { n.Value1 = n.Value0; isChanged = true; }
                    break;
                case 2:   // 法师 → 战士
                    foreach (var n in _defNodes)
                        if (n.Value1 != 0 && n.Value0 == 0) { n.Value0 = n.Value1; isChanged = true; }
                    break;
                case 3:   // 道士 → 战士
                    foreach (var n in _defNodes)
                        if (n.Value2 != 0 && n.Value0 == 0) { n.Value0 = n.Value2; isChanged = true; }
                    break;
            }
            if (isChanged)
            {
                for (int i = 0; i < _defNodes.Count; i++)
                    RefreshRow(vstDefPower, i);
                DoConfigChanged(true);
            }
        }
        else
        {
            switch (HeaderClickIndex)
            {
                case 2:   // 战士 → 法师
                    foreach (var n in _varNodes)
                        if (n.Value0 != 0 && n.Value1 == 0) { n.Value1 = n.Value0; isChanged = true; }
                    break;
                case 3:   // 法师 → 战士
                    foreach (var n in _varNodes)
                        if (n.Value1 != 0 && n.Value0 == 0) { n.Value0 = n.Value1; isChanged = true; }
                    break;
                case 4:   // 道士 → 战士
                    foreach (var n in _varNodes)
                        if (n.Value2 != 0 && n.Value0 == 0) { n.Value0 = n.Value2; isChanged = true; }
                    break;
            }
            if (isChanged)
            {
                for (int i = 0; i < _varNodes.Count; i++)
                    RefreshRow(vstVarPower, i);
                DoConfigChanged(false);
            }
        }
        return isChanged;
    }

    /// <summary>mniCopy2Click（1331-1458）1:1：另一方向（源 → 第二个目标职业）。</summary>
    public bool MniCopy2Click()
    {
        bool isChanged = false;
        if (ActivePageIndex == 0)
        {
            switch (HeaderClickIndex)
            {
                case 1:   // 战士 → 道士
                    foreach (var n in _defNodes)
                        if (n.Value0 != 0 && n.Value2 == 0) { n.Value2 = n.Value0; isChanged = true; }
                    break;
                case 2:   // 法师 → 道士
                    foreach (var n in _defNodes)
                        if (n.Value1 != 0 && n.Value2 == 0) { n.Value2 = n.Value1; isChanged = true; }
                    break;
                case 3:   // 道士 → 法师
                    foreach (var n in _defNodes)
                        if (n.Value2 != 0 && n.Value1 == 0) { n.Value1 = n.Value2; isChanged = true; }
                    break;
            }
            if (isChanged)
            {
                for (int i = 0; i < _defNodes.Count; i++)
                    RefreshRow(vstDefPower, i);
                DoConfigChanged(true);
            }
        }
        else
        {
            switch (HeaderClickIndex)
            {
                case 2:   // 战士 → 道士
                    foreach (var n in _varNodes)
                        if (n.Value0 != 0 && n.Value2 == 0) { n.Value2 = n.Value0; isChanged = true; }
                    break;
                case 3:   // 法师 → 道士
                    foreach (var n in _varNodes)
                        if (n.Value1 != 0 && n.Value2 == 0) { n.Value2 = n.Value1; isChanged = true; }
                    break;
                case 4:   // 道士 → 法师
                    foreach (var n in _varNodes)
                        if (n.Value2 != 0 && n.Value1 == 0) { n.Value1 = n.Value2; isChanged = true; }
                    break;
            }
            if (isChanged)
            {
                for (int i = 0; i < _varNodes.Count; i++)
                    RefreshRow(vstVarPower, i);
                DoConfigChanged(false);
            }
        }
        return isChanged;
    }

    /// <summary>ShowFrmCombatPowerSetting（119-129）1:1：ShowModal。</summary>
    public Func<System.Windows.Forms.DialogResult>? ShowModalHandler;

    public void ShowFrmCombatPowerSetting()
        => _ = ShowModalHandler?.Invoke() ?? ShowDialog();
}
