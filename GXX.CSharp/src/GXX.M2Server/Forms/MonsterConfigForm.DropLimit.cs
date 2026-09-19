using System;
using System.Collections.Generic;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// MonsterConfig.pas 巨片余部第一片：掉落限制页（tsDropItem）——
/// vstDropLimitItems 物品树 / vstItemRules 规则树（11 列，列 0..5 与 8 可编辑）/ lstAllItems 全物品表 /
/// edtItemSearch 过滤 / chkRecordLog / Ctrl+Insert 增规则 / Ctrl+Delete 删规则 / Ctrl+F 查找 /
/// 删除物品（Button1）/ 右键日志菜单 / ShowFrmItemDropLog 查看接缝。
/// 数据层见 Engine/DropLimitState.cs（ItemDropLimit.pas 1:1）。
/// </summary>
public sealed partial class MonsterConfigForm
{
    // ---- 掉落限制页控件（Delphi tsDropItem） ----
    public System.Windows.Forms.ListBox lstAllItems = null!;
    public System.Windows.Forms.ListView VstDropLimitItems = null!;
    public System.Windows.Forms.ListView VstItemRules = null!;
    public System.Windows.Forms.TextBox edtItemSearch = null!;
    public System.Windows.Forms.CheckBox chkRecordLog = null!;
    public System.Windows.Forms.Button btnAddItemRule = null!;
    public System.Windows.Forms.Button btnClearItemRule = null!;
    public System.Windows.Forms.Button BtnDelLimitItem = null!;   // Delphi 控件名 Button1

    /// <summary>Delphi tsDropItem.TabVisible（g_nKey_DropLimitExt&lt;&gt;1 时 False）。</summary>
    public bool TsDropItemVisible = true;

    /// <summary>右键菜单 miItemRulesLog.Visible 等效（pmItemRulesPopup）。</summary>
    public bool MiItemRulesLogVisible;

    public TDropLimitItem? FCurrentLimitItem;
    public TDropItemRule? FCurrentItemRule;

    /// <summary>UserEngine.StdItemList 接缝（FormCreate 全物品表填充）。</summary>
    public Func<List<string>>? AllItemsHandler;

    /// <summary>g_MapManager 接缝（规则列 0 地图下拉；Mirror 跳过、FB 取主地图名）。</summary>
    public Func<List<DropLimitMapEntry>>? MapListHandler;

    /// <summary>ShowFrmItemDropLog 接缝（规则列 10 '查看'）。</summary>
    public Action<string, string>? ShowFrmItemDropLogHandler;

    /// <summary>vstItemRules 当前待编辑列（WM_STARTEDITING_ITEMRULE 等效）。</summary>
    public int PendingItemRuleEditColumn = -1;

    /// <summary>vstDropLimitItems 节点主表（Delphi IsVisible 过滤只隐藏不删除，节点常驻）。</summary>
    private readonly List<TDropLimitItem> _dropLimitNodes = new();

    /// <summary>Open/双击录入共用的节点添加（主表 + 可见列表）。</summary>
    private System.Windows.Forms.ListViewItem AddDropLimitNode(TDropLimitItem item)
    {
        _dropLimitNodes.Add(item);
        var node = new System.Windows.Forms.ListViewItem(item.Name) { Tag = item };
        VstDropLimitItems.Items.Add(node);
        return node;
    }

    private void InitializeComponentDropLimit()
    {
        var gb = new System.Windows.Forms.GroupBox { Text = "掉落限制", Left = 8, Top = 336, Width = 660, Height = 190 };
        Controls.Add(gb);
        gb.Controls.Add(new System.Windows.Forms.Label { Text = "全物品:", Left = 10, Top = 20, AutoSize = true });
        lstAllItems = new System.Windows.Forms.ListBox { Left = 70, Top = 18, Width = 150, Height = 120 };
        lstAllItems.DoubleClick += (_, _) => LstAllItemsDblClick(lstAllItems);
        lstAllItems.KeyDown += (s, e) => LstAllItemsKeyDown((int)e.KeyCode, e.Control);
        gb.Controls.Add(lstAllItems);
        gb.Controls.Add(new System.Windows.Forms.Label { Text = "限制物品:", Left = 230, Top = 20, AutoSize = true });
        VstDropLimitItems = new System.Windows.Forms.ListView
        {
            Left = 295, Top = 18, Width = 160, Height = 120, View = System.Windows.Forms.View.Details,
            FullRowSelect = true, HideSelection = false, MultiSelect = false,
        };
        VstDropLimitItems.Columns.Add("物品", 150);
        VstDropLimitItems.Click += (_, _) => VstDropLimitItemsNodeClick();
        gb.Controls.Add(VstDropLimitItems);
        VstItemRules = new System.Windows.Forms.ListView
        {
            Left = 462, Top = 18, Width = 190, Height = 120, View = System.Windows.Forms.View.Details,
            FullRowSelect = true, HideSelection = false, MultiSelect = false,
        };
        string[] ruleCols = { "地图", "清理间隔", "类型", "掉落间隔", "数量限制", "已掉", "剩余", "下次清理日期", "下次清理时间", "累计", "日志" };
        foreach (var col in ruleCols)
            VstItemRules.Columns.Add(col, 90);
        VstItemRules.Click += (_, _) => VstItemRulesNodeClick(VstItemRules.SelectedItems.Count > 0
            ? GetItemRuleColumn(VstItemRules)
            : -1);
        gb.Controls.Add(VstItemRules);
        gb.Controls.Add(new System.Windows.Forms.Label { Text = "查找:", Left = 230, Top = 145, AutoSize = true });
        edtItemSearch = new System.Windows.Forms.TextBox { Left = 272, Top = 142, Width = 100 };
        edtItemSearch.TextChanged += (_, _) => EdtItemSearchChange();
        gb.Controls.Add(edtItemSearch);
        chkRecordLog = new System.Windows.Forms.CheckBox { Text = "记录日志", Left = 462, Top = 142, AutoSize = true };
        chkRecordLog.Click += (_, _) => ChkRecordLogClick();
        gb.Controls.Add(chkRecordLog);
        btnAddItemRule = new System.Windows.Forms.Button { Text = "增加规则", Left = 10, Top = 145, Width = 70 };
        btnAddItemRule.Click += (_, _) => BtnAddItemRuleClick();
        gb.Controls.Add(btnAddItemRule);
        btnClearItemRule = new System.Windows.Forms.Button { Text = "清空规则", Left = 84, Top = 145, Width = 70 };
        btnClearItemRule.Click += (_, _) => BtnClearItemRuleClick();
        gb.Controls.Add(btnClearItemRule);
        BtnDelLimitItem = new System.Windows.Forms.Button { Text = "删除物品", Left = 158, Top = 145, Width = 70 };
        BtnDelLimitItem.Click += (_, _) => BtnDelLimitItemClick();
        gb.Controls.Add(BtnDelLimitItem);

        // Delphi FormCreate：g_nKey_DropLimitExt<>1 → 页签隐藏（填充在 Open 起始的
        // EnsureFormCreateDropLimitFill 一次性执行，等效 FormCreate 时序）
        if (DropLimitGlobals.g_nKey_DropLimitExt != 1)
            TsDropItemVisible = false;
    }

    /// <summary>Delphi FormCreate 掉落限制部分（全物品表填充 + 当前复位 + 按钮刷新；仅执行一次）。</summary>
    private bool _formCreateDropLimitDone;

    private void EnsureFormCreateDropLimitFill()
    {
        if (_formCreateDropLimitDone)
            return;
        _formCreateDropLimitDone = true;
        if (DropLimitGlobals.g_nKey_DropLimitExt != 1)
            return;
        lstAllItems.Items.Clear();
        foreach (var name in AllItemsHandler?.Invoke() ?? new List<string>())
            lstAllItems.Items.Add(name);
        FCurrentLimitItem = null;
        FCurrentItemRule = null;
        RefreshDropLimitItemButtons();
    }

    public sealed class DropLimitMapEntry
    {
        public string MapName = "";
        public string MainMapName = "";
        public bool Mirror;
        public bool FB;
    }

    private static int GetItemRuleColumn(System.Windows.Forms.ListView vst)
    {
        // WinForms 无列命中：以鼠标 X 折算列宽（与 Delphi HitColumn 等效的测试近似）
        var pt = vst.PointToClient(System.Windows.Forms.Cursor.Position);
        int x = 0;
        for (int i = 0; i < vst.Columns.Count; i++)
        {
            x += vst.Columns[i].Width;
            if (pt.X < x)
                return i;
        }
        return -1;
    }

    /// <summary>Delphi ListView/FocusedNode 等效取行（无句柄环境下 SelectedItems 为空，按 Selected 标志回退）。</summary>
    private static System.Windows.Forms.ListViewItem? GetSelected(System.Windows.Forms.ListView lv)
    {
        if (lv.SelectedItems.Count > 0)
            return lv.SelectedItems[0];
        foreach (System.Windows.Forms.ListViewItem it in lv.Items)
            if (it.Selected) return it;
        return null;
    }

    // ================= vstDropLimitItems（物品树） =================

    /// <summary>vstDropLimitItemsNodeClick 1:1（清规则树 → 取 FocusedNode → 回填规则 → 刷新按钮）。</summary>
    public void VstDropLimitItemsNodeClick()
    {
        VstItemRules.Items.Clear();
        var focused = GetSelected(VstDropLimitItems);
        if (focused == null)
        {
            FCurrentLimitItem = null;
            RefreshDropLimitItemButtons();
            return;
        }
        FCurrentLimitItem = (TDropLimitItem?)focused.Tag;
        FCurrentItemRule = null;
        if (FCurrentLimitItem != null)
        {
            for (int i = 0; i < FCurrentLimitItem.Count; i++)
            {
                var rule = FCurrentLimitItem.GetItemRule(i);
                if (rule != null)
                    AddItemRuleRow(rule);
            }
        }
        RefreshDropLimitItemButtons();
    }

    private void AddItemRuleRow(TDropItemRule rule)
    {
        var item = new System.Windows.Forms.ListViewItem(ItemRuleCellText(rule, 0)) { Tag = rule };
        for (int col = 1; col <= 10; col++)
            item.SubItems.Add(ItemRuleCellText(rule, col));
        VstItemRules.Items.Add(item);
    }

    /// <summary>vstItemRulesGetText 1:1（11 列文本：剩余/下次清理日期/时间为派生列，无间隔显示 '-'）。</summary>
    public static string ItemRuleCellText(TDropItemRule rule, int column)
    {
        switch (column)
        {
            case 0: return rule.MapName;
            case 1: return rule.ClearInterval.ToString();
            case 2: return DropLimitConsts.TIntervalTypeNames[(int)rule.IntervalType];
            case 3: return rule.DropInterval.ToString();
            case 4: return rule.LimitCount.ToString();
            case 5: return rule.DropedCount.ToString();
            case 6:
                return rule.LimitCount > 0
                    ? Math.Max(rule.LimitCount - rule.DropedCount, 0).ToString()
                    : "-";
            case 7:
                if (rule.ClearInterval > 0)
                {
                    var nextDate = NextClearDate(rule);
                    return DateTime.FromOADate(Math.Truncate(nextDate)).ToString("yyyy/MM/dd");
                }
                return "-";
            case 8:
                if (rule.ClearInterval > 0)
                    return DateTime.FromOADate(NextClearDate(rule)).ToString("HH:mm:ss");
                return "-";
            case 9: return rule.AllDropedCount.ToString();
            case 10: return "查看";
            default: return "";
        }
    }

    /// <summary>Delphi 下次清理时间：LastClearDate + ClearInterval（天加法/时/分增减）。</summary>
    public static double NextClearDate(TDropItemRule rule)
    {
        if (rule.IntervalType == TIntervalType.itDay)
            return rule.LastClearDate + rule.ClearInterval;
        if (rule.IntervalType == TIntervalType.itHour)
            return rule.LastClearDate + rule.ClearInterval / 24.0;
        return rule.LastClearDate + rule.ClearInterval / 1440.0;
    }

    /// <summary>lstAllItemsDblClick 1:1（先恢复全部可见 → 未录入则 AddItem+Save+选中触发点击，已录入则定位选中触发点击）。</summary>
    public void LstAllItemsDblClick(object? sender)
    {
        if (lstAllItems.SelectedIndex < 0)
        {
            FCurrentLimitItem = null;
            return;
        }
        RebuildVisibleNodes(_ => true);
        string itemName = lstAllItems.Items[lstAllItems.SelectedIndex]?.ToString() ?? "";
        if (!DropLimitGlobals.g_DropLimitMgr.Search(itemName, out _))
        {
            var limitItem = DropLimitGlobals.g_DropLimitMgr.AddItem(itemName);
            if (limitItem == null)
                return;
            var node = AddDropLimitNode(limitItem);
            limitItem.IsChanged = true;
            limitItem.Save();
            node.Selected = true;
            VstDropLimitItems.FocusedItem = node;
            VstDropLimitItemsNodeClick();
        }
        else
        {
            foreach (System.Windows.Forms.ListViewItem node in VstDropLimitItems.Items)
            {
                var limitItem = (TDropLimitItem?)node.Tag;
                if (limitItem != null && string.Equals(limitItem.Name, itemName, StringComparison.OrdinalIgnoreCase))
                {
                    node.Selected = true;
                    VstDropLimitItems.FocusedItem = node;
                    VstDropLimitItemsNodeClick();
                    return;
                }
            }
        }
    }

    /// <summary>IsVisible 恢复全部（Delphi BeginUpdate 遍历置 True）。</summary>
    private void RebuildVisibleNodes(Func<TDropLimitItem, bool> visible)
    {
        VstDropLimitItems.Items.Clear();
        foreach (var it in _dropLimitNodes)
        {
            if (visible(it))
                VstDropLimitItems.Items.Add(new System.Windows.Forms.ListViewItem(it.Name) { Tag = it });
        }
    }

    // ================= vstItemRules（规则树编辑） =================

    /// <summary>vstItemRulesEditing 1:1（Column &lt;= 5 或 = 8 允许编辑）。</summary>
    public static bool IsItemRuleColumnEditable(int column)
        => column <= 5 || column == 8;

    public enum ItemRuleEditKind { MapCombo, Spin, IntervalTypeCombo, Time }

    public sealed class ItemRuleEditor
    {
        public ItemRuleEditKind Kind;
        public List<string> Items = new();  // 组合框条目
        public string Text = "";            // 地图组合框当前文本
        public int ItemIndex;               // 类型组合框当前项
        public int MinValue, MaxValue, Value;
        public DateTime DateTime;
    }

    /// <summary>TItemRulePropertyEditLink.PrepareEdit 1:1（列 0 地图下拉；1 Min1/Max60；3/4/5 Min0/Max0 原文形态；2 类型下拉；8 时间）。</summary>
    public ItemRuleEditor? PrepareItemRuleEditor(int column, TDropItemRule rule)
    {
        switch (column)
        {
            case 0:
            {
                var ed = new ItemRuleEditor { Kind = ItemRuleEditKind.MapCombo };
                var sl = new List<string>();
                foreach (var envir in MapListHandler?.Invoke() ?? new List<DropLimitMapEntry>())
                {
                    if (envir.Mirror)
                        continue;
                    string mapName = envir.FB ? envir.MainMapName : envir.MapName;
                    if (!sl.Contains(mapName))
                        sl.Add(mapName);
                }
                sl.Sort(StringComparer.Ordinal);
                sl.Insert(0, "*");
                ed.Items = sl;
                ed.Text = rule.MapName;
                return ed;
            }
            case 1:
                return new ItemRuleEditor { Kind = ItemRuleEditKind.Spin, MinValue = 1, MaxValue = 60, Value = rule.ClearInterval };
            case 2:
            {
                var ed = new ItemRuleEditor { Kind = ItemRuleEditKind.IntervalTypeCombo };
                ed.Items.AddRange(DropLimitConsts.TIntervalTypeNames);
                ed.ItemIndex = (int)rule.IntervalType;
                return ed;
            }
            case 3:
                return new ItemRuleEditor { Kind = ItemRuleEditKind.Spin, MinValue = 0, MaxValue = 0, Value = rule.DropInterval };
            case 4:
                return new ItemRuleEditor { Kind = ItemRuleEditKind.Spin, MinValue = 0, MaxValue = 0, Value = rule.LimitCount };
            case 5:
                return new ItemRuleEditor { Kind = ItemRuleEditKind.Spin, MinValue = 0, MaxValue = 0, Value = rule.DropedCount };
            case 8:
                return new ItemRuleEditor { Kind = ItemRuleEditKind.Time, DateTime = DateTime.FromOADate(NextClearDate(rule)) };
            default:
                return null; // Delphi else Result := False
        }
    }

    /// <summary>TItemRulePropertyEditLink.EndEdit 1:1（写回 + 列 4/5 双向钳制 DropedCount≤LimitCount + 变更即 Save）。</summary>
    public void EndItemRuleEdit(int column, TDropItemRule rule, ItemRuleEditor ed)
    {
        bool isChanged = false;
        switch (column)
        {
            case 0:
                isChanged = !string.Equals(rule.MapName, ed.Text, StringComparison.OrdinalIgnoreCase);
                if (isChanged)
                    rule.MapName = ed.Text;
                break;
            case 1:
                isChanged = rule.ClearInterval != ed.Value;
                if (isChanged)
                    rule.ClearInterval = ed.Value;
                break;
            case 2:
                isChanged = ed.ItemIndex != (int)rule.IntervalType;
                if (isChanged)
                    rule.IntervalType = (TIntervalType)ed.ItemIndex;
                break;
            case 3:
                isChanged = rule.DropInterval != ed.Value;
                if (isChanged)
                    rule.DropInterval = ed.Value;
                break;
            case 4:
                isChanged = rule.LimitCount != ed.Value;
                if (isChanged)
                    rule.LimitCount = ed.Value;
                if (rule.DropedCount > rule.LimitCount)
                    rule.DropedCount = rule.LimitCount;
                break;
            case 5:
                isChanged = rule.DropedCount != ed.Value;
                if (isChanged)
                    rule.DropedCount = ed.Value;
                if (rule.DropedCount > rule.LimitCount)
                    rule.DropedCount = rule.LimitCount;
                break;
            case 8:
                if (rule.IntervalType == TIntervalType.itDay)
                    rule.LastClearDate = ed.DateTime.ToOADate() - rule.ClearInterval;
                else if (rule.IntervalType == TIntervalType.itHour)
                    rule.LastClearDate = ed.DateTime.ToOADate() - rule.ClearInterval / 24.0;
                else
                    rule.LastClearDate = ed.DateTime.ToOADate() - rule.ClearInterval / 1440.0;
                break;
        }
        if (isChanged)
            FCurrentLimitItem?.Save();
        RefreshItemRuleRow(rule);
    }

    /// <summary>EndEdit 后刷新规则行展示（VCL Invalidate 等效）。</summary>
    private void RefreshItemRuleRow(TDropItemRule rule)
    {
        foreach (System.Windows.Forms.ListViewItem row in VstItemRules.Items)
        {
            if (!ReferenceEquals(row.Tag, rule))
                continue;
            for (int col = 0; col <= 10; col++)
            {
                if (col == 0)
                    row.Text = ItemRuleCellText(rule, 0);
                else
                    row.SubItems[col].Text = ItemRuleCellText(rule, col);
            }
            break;
        }
    }

    /// <summary>vstItemRulesNodeClick 1:1（列 10 且有当前物品 → 查看掉落日志；否则记录 FCurrentItemRule 并进入编辑）。</summary>
    public void VstItemRulesNodeClick(int hitColumn)
    {
        var row = GetSelected(VstItemRules);
        if (row == null || hitColumn < 0)
            return;
        var rule = (TDropItemRule?)row.Tag;
        if (rule == null)
            return;
        if (hitColumn == 10 && FCurrentLimitItem != null)
        {
            ShowFrmItemDropLogHandler?.Invoke(FCurrentLimitItem.Name, rule.MapName);
        }
        else
        {
            FCurrentItemRule = rule;
            RefreshDropLimitItemButtons();
            PendingItemRuleEditColumn = hitColumn;
        }
    }

    /// <summary>vstItemRulesKeyUp 1:1（Ctrl+Insert 增默认规则；Ctrl+Delete 删除并聚焦邻行，双双 Save）。</summary>
    public void VstItemRulesKeyUp(int key, bool ctrl)
    {
        const int VK_INSERT = 0x2D;
        const int VK_DELETE = 0x2E;
        if (!ctrl)
            return;
        if (key == VK_INSERT)
        {
            if (FCurrentLimitItem == null)
                return;
            var itemRule = new TDropItemRule
            {
                MapName = "*",
                ClearInterval = 1,
                IntervalType = TIntervalType.itDay,
                DropInterval = 0,
                LimitCount = 1,
                DropedCount = 0,
                LastClearDate = DropLimitGlobals.NowFn().ToOADate(),
                AllDropedCount = 0,
                LastDropTime = 0,
            };
            var rule = FCurrentLimitItem.Add(itemRule);
            var row = AddItemRuleRow2(rule);
            row.Selected = true;
            VstItemRules.FocusedItem = row;
            FCurrentLimitItem.Save();
            PendingItemRuleEditColumn = 0;
        }
        else if (key == VK_DELETE)
        {
            var row = GetSelected(VstItemRules);
            if (row == null || FCurrentLimitItem == null)
                return;
            var rule = (TDropItemRule?)row.Tag;
            int rowIndex = row.Index;
            // Delphi GetNext/GetPrevious 返回节点引用（删除后仍有效），邻行按引用取
            var neighbor = rowIndex + 1 < VstItemRules.Items.Count ? VstItemRules.Items[rowIndex + 1]
                : rowIndex - 1 >= 0 ? VstItemRules.Items[rowIndex - 1] : null;
            if (rule != null && FCurrentLimitItem.Remove(rule))
            {
                VstItemRules.Items.Remove(row);
                if (neighbor != null)
                {
                    neighbor.Selected = true;
                    VstItemRules.FocusedItem = neighbor;
                    FCurrentItemRule = (TDropItemRule?)neighbor.Tag;
                    RefreshDropLimitItemButtons();
                }
                else
                {
                    FCurrentItemRule = null;
                    RefreshDropLimitItemButtons();
                }
                FCurrentLimitItem.Save();
            }
        }
    }

    private System.Windows.Forms.ListViewItem AddItemRuleRow2(TDropItemRule rule)
    {
        var item = new System.Windows.Forms.ListViewItem(ItemRuleCellText(rule, 0)) { Tag = rule };
        for (int col = 1; col <= 10; col++)
            item.SubItems.Add(ItemRuleCellText(rule, col));
        VstItemRules.Items.Add(item);
        return item;
    }

    // ================= 按钮/搜索/日志开关 =================

    /// <summary>RefreshDropLimitItemButtons 1:1（注释掉的 btnAddItemRule/btnEditItemRule/btnDelItemRule 行为保留原文形态）。</summary>
    public void RefreshDropLimitItemButtons()
    {
        // btnAddItemRule.Enabled := FCurrentLimitItem <> nil;
        btnClearItemRule.Enabled = FCurrentLimitItem != null;
        chkRecordLog.Enabled = FCurrentLimitItem != null;
        // btnEditItemRule.Enabled := FCurrentItemRule <> nil;
        // btnDelItemRule.Enabled := FCurrentItemRule <> nil;
        chkRecordLog.Checked = FCurrentLimitItem != null && FCurrentLimitItem.IsRecordLog;
    }

    /// <summary>btnAddItemRuleClick 1:1（原文仅有 FCurrentLimitItem 判空早退，无实际动作）。</summary>
    public void BtnAddItemRuleClick()
    {
        if (FCurrentLimitItem == null)
            return;
    }

    /// <summary>btnClearItemRuleClick 1:1（清规则树 + 清数据 + 落盘）。</summary>
    public void BtnClearItemRuleClick()
    {
        if (FCurrentLimitItem != null)
        {
            VstItemRules.Items.Clear();
            FCurrentLimitItem.Clear();
            FCurrentLimitItem.Save();
        }
    }

    /// <summary>Button1Click 1:1（删除物品：删节点 + 管理器移除 + 清规则树 + 复位当前）。</summary>
    public void BtnDelLimitItemClick()
    {
        if (FCurrentLimitItem != null)
        {
            var focused = GetSelected(VstDropLimitItems);
            if (focused != null)
                VstDropLimitItems.Items.Remove(focused);
            _dropLimitNodes.Remove(FCurrentLimitItem);
            DropLimitGlobals.g_DropLimitMgr.Remove(FCurrentLimitItem);
            VstItemRules.Items.Clear();
            FCurrentLimitItem = null;
            FCurrentItemRule = null;
            RefreshDropLimitItemButtons();
        }
    }

    /// <summary>lstAllItemsKeyDown 1:1（Ctrl+F InputQuery 精确等值查找；不命中保持原选中）。</summary>
    public void LstAllItemsKeyDown(int key, bool ctrl)
    {
        const int KEY_F = 0x46;
        if (!ctrl || key != KEY_F)
            return;
        if (InputQueryHandler == null || !InputQueryHandler("物品查找", "输入物品名称:"))
            return;
        string sItemName = LastInputQueryText;
        if (sItemName.Length == 0)
            return;
        for (int i = 0; i < lstAllItems.Items.Count; i++)
        {
            if ((lstAllItems.Items[i]?.ToString() ?? "") == sItemName)
            {
                lstAllItems.SelectedIndex = i;
                break;
            }
        }
    }

    /// <summary>InputQuery 返回文本接缝（M2Forms.InputQueryText 等效，测试注入查找关键字）。</summary>
    public string LastInputQueryText = "";

    /// <summary>edtItemSearchChange 1:1（空文本恢复全部可见；否则 Pos(关键字, 名) 子串过滤，大小写敏感原文保留；隐藏节点常驻主表）。</summary>
    public void EdtItemSearchChange()
    {
        string key = edtItemSearch.Text;
        RebuildVisibleNodes(key.Length == 0 ? _ => true : it => it.Name.Contains(key));
    }

    /// <summary>edtItemSearchKeyPress 1:1（回车触发过滤并吞键）。</summary>
    public bool EdtItemSearchKeyPress(char key)
    {
        if (key == '\r')
        {
            EdtItemSearchChange();
            return false; // Key := #0
        }
        return true;
    }

    /// <summary>chkRecordLogClick 1:1（写 IsRecordLog 并落盘）。</summary>
    public void ChkRecordLogClick()
    {
        if (FCurrentLimitItem != null)
        {
            FCurrentLimitItem.IsRecordLog = chkRecordLog.Checked;
            FCurrentLimitItem.Save();
        }
    }

    /// <summary>pmItemRulesPopup 1:1（有当前规则时显示日志菜单项）。</summary>
    public void PmItemRulesPopup()
    {
        MiItemRulesLogVisible = FCurrentItemRule != null;
    }
}
