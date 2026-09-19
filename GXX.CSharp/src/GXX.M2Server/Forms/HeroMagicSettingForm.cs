using System;
using System.Collections.Generic;
using System.Globalization;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// uFrmHeroMagicSetting.pas 1:1（批次J63）：英雄技能自动施法设置窗体。
/// 三页签（战士 Tag=0 / 法师 Tag=1 / 道士 Tag=2）+ 每行 7 列（0 勾选 / 1 技能名 / 2 类型 /
/// 3 使用率 / 4 攻击范围 / 5 攻击目标 / 6 条件文本 / 7 设置…）；
/// Editing 列白名单（普通技能仅列 3；自定义技能战士页 1,3,4、其余页 1,3,4,5）；
/// Ctrl+Insert 新增自定义条目 / Ctrl+Delete 仅删自定义；
/// 列 7 点击 → HeroMagicConditionForm，变更时置 IsChanged 并 SetCanSave；
/// GetHeroMagicUseConditionText 逐项拼接（含原文 boForeverFrozen 判两次的瑕疵）。
/// </summary>
public sealed class HeroMagicSettingForm : System.Windows.Forms.Form
{
    /// <summary>TNodeData（HeroMagic + MagicName）。</summary>
    public sealed class TNodeData
    {
        public THeroMagic HeroMagic = new();
        public string MagicName = "";
    }

    public System.Windows.Forms.TabControl pgcMain = null!;
    public System.Windows.Forms.TabPage ts1 = null!;
    public System.Windows.Forms.TabPage ts2 = null!;
    public System.Windows.Forms.TabPage ts3 = null!;
    public System.Windows.Forms.ListView vstWarrMagic = null!;
    public System.Windows.Forms.ListView vstWizardMagic = null!;
    public System.Windows.Forms.ListView vstTaosMagic = null!;
    public System.Windows.Forms.Panel pnl1 = null!;
    public System.Windows.Forms.Button btnSave = null!;
    public System.Windows.Forms.Label lbl34 = null!;

    /// <summary>g_CustomHeroMagicMgr（Delphi 单元全局）。</summary>
    public TCustomHeroMagicMgr Mgr = CustomHeroMagicState.CustomHeroMagicMgr;

    /// <summary>UserEngine.FindHeroMagic(MagicID) / (MagicID, mtHero) 接缝（返回技能名，null = 未找到）。</summary>
    public Func<int, string?>? FindHeroMagicNameAnyHandler;
    public Func<int, string?>? FindHeroMagicNameHandler;

    /// <summary>UserEngine.m_MagicList 接缝（列 1 下拉候选：wMagicId + sMagicName，按 mtype 过滤）。</summary>
    public Func<IEnumerable<(int MagicId, string MagicName)>>? HeroMagicCandidatesHandler;

    /// <summary>ShowFrmHeroMagicCondition 接缝（返回是否确认）。</summary>
    public Func<THeroMagicUseCondition, (bool Ok, THeroMagicUseCondition Result)>? ConditionDialogHandler;

    /// <summary>ShowModal 接缝。</summary>
    public Func<System.Windows.Forms.DialogResult>? ShowModalHandler;

    /// <summary>列 0..7 表头（Delphi DFM 列序）。</summary>
    public static readonly string[] ColumnHeaders =
        { "启用", "技能名称", "类型", "使用率", "攻击范围", "攻击目标", "使用条件", "设置" };

    private readonly System.Windows.Forms.ListView[] _trees;
    private readonly Dictionary<System.Windows.Forms.ListView, List<TNodeData>> _nodeData = new();

    public HeroMagicSettingForm() : this(CustomHeroMagicState.CustomHeroMagicMgr) { }

    public HeroMagicSettingForm(TCustomHeroMagicMgr mgr)
    {
        Mgr = mgr;
        InitializeComponent();
        _trees = new[] { vstWarrMagic, vstWizardMagic, vstTaosMagic };
        foreach (var tree in _trees)
            _nodeData[tree] = new List<TNodeData>();
        FormCreate();
    }

    private void InitializeComponent()
    {
        Text = "英雄技能设置";
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        ClientSize = new System.Drawing.Size(760, 500);

        pgcMain = new System.Windows.Forms.TabControl { Left = 4, Top = 4, Width = 752, Height = 440 };
        ts1 = new System.Windows.Forms.TabPage("战士");
        ts2 = new System.Windows.Forms.TabPage("法师");
        ts3 = new System.Windows.Forms.TabPage("道士");
        vstWarrMagic = NewTree(ts1);
        vstWizardMagic = NewTree(ts2);
        vstTaosMagic = NewTree(ts3);
        pgcMain.TabPages.Add(ts1);
        pgcMain.TabPages.Add(ts2);
        pgcMain.TabPages.Add(ts3);

        pnl1 = new System.Windows.Forms.Panel { Left = 4, Top = 448, Width = 752, Height = 44 };
        btnSave = new System.Windows.Forms.Button { Text = "保存(&S)", Left = 8, Top = 8, Width = 90, Height = 28, Enabled = false };
        btnSave.Click += (_, _) => BtnSaveClick();
        lbl34 = new System.Windows.Forms.Label
        {
            Text = "Ctrl+Insert 新增自定义技能；Ctrl+Delete 删除自定义技能；点击「设置」列编辑使用条件",
            Left = 110, Top = 14, AutoSize = true,
        };
        pnl1.Controls.Add(btnSave);
        pnl1.Controls.Add(lbl34);

        Controls.Add(pgcMain);
        Controls.Add(pnl1);
    }

    private static System.Windows.Forms.ListView NewTree(System.Windows.Forms.TabPage page)
    {
        var lv = new System.Windows.Forms.ListView
        {
            View = System.Windows.Forms.View.Details,
            CheckBoxes = true,
            FullRowSelect = true,
            GridLines = true,
            Dock = System.Windows.Forms.DockStyle.Fill,
            HideSelection = false,
        };
        var widths = new[] { 46, 160, 90, 70, 80, 90, 220, 60 };
        for (int i = 0; i < ColumnHeaders.Length; i++)
            lv.Columns.Add(ColumnHeaders[i], widths[i]);
        page.Controls.Add(lv);
        return lv;
    }

    /// <summary>当前页索引（0 战士 / 1 法师 / 2 道士）。</summary>
    public int ActivePageIndex => pgcMain.SelectedIndex;

    private System.Windows.Forms.ListView ActiveTree => _trees[Math.Max(0, Math.Min(2, ActivePageIndex))];

    /// <summary>窗体 Tag 等效（0 战士 / 1 法师 / 2 道士）——列 1/5 候选集与 Editing 白名单依赖。</summary>
    public static int TreeTag(System.Windows.Forms.ListView tree) => tree == null ? 0
        : tree.Tag is int t ? t : 0;

    // ==================== 列文本（GetText 1:1） ====================

    /// <summary>GetHeroMagicUseConditionText（427-582）1:1：逐项拼接，尾部裁剪 3 字符。</summary>
    public static string GetHeroMagicUseConditionText(THeroMagicUseCondition condition)
    {
        string result = "";
        if (condition.HeroLevelCheck.boChecked)
        {
            if (condition.HeroLevelCheck.CompareType == THeroLevelCompareType.hlctTargetLevel)
            {
                result += string.Format(CultureInfo.InvariantCulture, "(英雄等级 {0} 目标等级) + ",
                    CustomHeroMagicState.CompareSymbolNames[(int)condition.HeroLevelCheck.CompareSymbol]);
            }
            else if (condition.HeroLevelCheck.CompareType == THeroLevelCompareType.hlctLevelNumber)
            {
                result += string.Format(CultureInfo.InvariantCulture, "(英雄等级 {0} {1}) + ",
                    CustomHeroMagicState.CompareSymbolNames[(int)condition.HeroLevelCheck.CompareSymbol],
                    condition.HeroLevelCheck.CompareValue);
            }
        }

        result += HpLikeText("英雄HP", condition.HeroHPCheck);
        result += HpLikeText("英雄MP", condition.HeroMPCheck);
        result += HpLikeText("目标HP", condition.TargetHPCheck);
        result += HpLikeText("目标MP", condition.TargetMPCheck);

        if (condition.TargetStatusCheck.boPoisonDamageArmor) result += "红毒 + ";
        if (condition.TargetStatusCheck.boPoisonDecHealth) result += "绿毒 + ";
        if (condition.TargetStatusCheck.boPoisoning) result += "中毒 + ";
        if (condition.TargetStatusCheck.boPoisonStone) result += "麻痹 + ";
        if (condition.TargetStatusCheck.boFrozen) result += "冰冻 + ";
        if (condition.TargetStatusCheck.boForeverFrozen) result += "冰封 + ";
        // 原文瑕疵保留：此处再次判断 boForeverFrozen（本意应为 boCobwebWinding）
        if (condition.TargetStatusCheck.boForeverFrozen) result += "蛛网 + ";

        if (condition.TargetStatusCheck.boUnPoisonDamageArmor) result += "无红毒 + ";
        if (condition.TargetStatusCheck.boUnPoisonDecHealth) result += "无绿毒 + ";
        if (condition.TargetStatusCheck.boUnPoisoning) result += "无毒 + ";
        if (condition.TargetStatusCheck.boUnPoisonStone) result += "无麻痹 + ";
        if (condition.TargetStatusCheck.boUnFrozen) result += "无冰冻 + ";
        if (condition.TargetStatusCheck.boUnForeverFrozen) result += "无冰封 + ";
        // 原文瑕疵保留：同上传入 boUnForeverFrozen
        if (condition.TargetStatusCheck.boUnForeverFrozen) result += "无蛛网 + ";

        if (condition.FriendCountCheck.boChecked)
        {
            result += string.Format(CultureInfo.InvariantCulture, "(目标周围{0}格朋友数量 > {1}) + ",
                condition.FriendCountCheck.nCheckRange, condition.FriendCountCheck.nCheckValue);
        }
        if (condition.EnemyCountCheck.boChecked)
        {
            result += string.Format(CultureInfo.InvariantCulture, "(目标周围{0}格敌人数量 > {1}) + ",
                condition.EnemyCountCheck.nCheckRange, condition.EnemyCountCheck.nCheckValue);
        }

        if (condition.boStraightLineCheck)
            result += "[直线] + ";

        if (result.Length > 0)
            result = result.Substring(0, result.Length - 3);

        return result;
    }

    private static string HpLikeText(string prefix, THeroHPCheck check)
    {
        if (!check.boChecked)
            return "";
        string symbol = CustomHeroMagicState.CompareSymbolNames[(int)check.CompareSymbol];
        return check.CompareType == THeroHPCompareType.hhpctNumber
            ? string.Format(CultureInfo.InvariantCulture, "({0} {1} {2}) + ", prefix, symbol, check.CompareValue)
            : string.Format(CultureInfo.InvariantCulture, "({0} {1} {2}%) + ", prefix, symbol, check.CompareValue);
    }

    /// <summary>GetText（661-684）1:1：按列返回文本（列 0 为勾选框不返文本）。</summary>
    public static string GetCellText(TNodeData nodeData, int column) => column switch
    {
        1 => nodeData.MagicName,
        2 => nodeData.HeroMagic.IsCustomMagic ? "自定义技能" : "普通技能",
        3 => nodeData.HeroMagic.UseRate.ToString(CultureInfo.InvariantCulture),
        4 => nodeData.HeroMagic.AttackRange.ToString(CultureInfo.InvariantCulture),
        5 => CustomHeroMagicState.MagicAttackTargetNames[(int)nodeData.HeroMagic.AttackTarget],
        6 => GetHeroMagicUseConditionText(nodeData.HeroMagic.Condition),
        7 => nodeData.HeroMagic.IsCustomMagic ? "设置..." : "",
        _ => "",
    };

    // ==================== FormCreate ====================

    /// <summary>FormCreate（584-650）1:1：三树 Tag 0/1/2 + 按 MagicType 分树 + 勾选态 + 技能名解析。</summary>
    public void FormCreate()
    {
        vstWarrMagic.Tag = 0;
        vstWizardMagic.Tag = 1;
        vstTaosMagic.Tag = 2;
        foreach (var tree in _trees)
            _nodeData[tree].Clear();
        pgcMain.SelectedIndex = 0;

        for (int i = 0; i < Mgr.Count; i++)
        {
            var heroMagic = Mgr[i];
            if (heroMagic == null)
                continue;

            var tree = heroMagic.MagicType switch
            {
                THeroMagicType.mtWarrAttack => vstWarrMagic,
                THeroMagicType.mtWizardAttack => vstWizardMagic,
                _ => vstTaosMagic,
            };

            AddNode(tree, heroMagic, nodeData =>
            {
                nodeData.MagicName = ResolveMagicName(heroMagic.MagicID);
            });
        }

        btnSave.Enabled = false;
    }

    /// <summary>技能名解析（623-646）：自定义走无 mtype 重载，其余走 mtHero；未找到为 '-'。</summary>
    public string ResolveMagicName(int magicId)
    {
        string? name;
        if (Mgr.CheckIsCustomMagicHandler?.Invoke(magicId)
            ?? magicId >= Grobal2Const.CUSTOM_MAGIC_START_ID)
        {
            name = FindHeroMagicNameAnyHandler?.Invoke(magicId);
        }
        else
        {
            name = FindHeroMagicNameHandler?.Invoke(magicId);
        }
        return string.IsNullOrEmpty(name) ? "-" : name;
    }

    private TNodeData AddNode(System.Windows.Forms.ListView tree, THeroMagic heroMagic, Action<TNodeData>? fill = null)
    {
        var nodeData = new TNodeData { HeroMagic = heroMagic };
        fill?.Invoke(nodeData);
        _nodeData[tree].Add(nodeData);

        var item = new System.Windows.Forms.ListViewItem("") { Checked = heroMagic.Checked, Tag = nodeData };
        for (int col = 1; col < ColumnHeaders.Length; col++)
            item.SubItems.Add(GetCellText(nodeData, col));

        tree.Items.Add(item);
        return nodeData;
    }

    private void RefreshRow(System.Windows.Forms.ListView tree, System.Windows.Forms.ListViewItem item)
    {
        if (item.Tag is not TNodeData nodeData)
            return;
        for (int col = 1; col < ColumnHeaders.Length && col < item.SubItems.Count; col++)
            item.SubItems[col].Text = GetCellText(nodeData, col);
        item.Checked = nodeData.HeroMagic.Checked;
    }

    /// <summary>节点数据快照（测试用）：按树与行号取 TNodeData。</summary>
    public TNodeData GetNodeData(System.Windows.Forms.ListView tree, int index) => _nodeData[tree][index];

    /// <summary>指定树当前行数。</summary>
    public int NodeCount(System.Windows.Forms.ListView tree) => _nodeData[tree].Count;

    // ==================== 编辑门控 ====================

    /// <summary>Editing（731-757）1:1：普通技能仅列 3；自定义技能战士页 1,3,4、其余页 1,3,4,5。</summary>
    public bool EditingAllowed(System.Windows.Forms.ListView tree, int index, int column)
    {
        if (index < 0 || index >= _nodeData[tree].Count)
            return false;
        var nodeData = _nodeData[tree][index];
        if (!nodeData.HeroMagic.IsCustomMagic)
            return column == 3;
        return TreeTag(tree) == 0
            ? column is 1 or 3 or 4
            : column is 1 or 3 or 4 or 5;
    }

    /// <summary>PrepareEdit（259-403）1:1：列 1 候选（战士页仅 IsMagicWarr，其余页取反）；列 5 目标候选（战士页仅到 matMaster）。</summary>
    public List<(int Value, string Text)> PrepareEdit(System.Windows.Forms.ListView tree, int column)
    {
        var result = new List<(int, string)>();
        switch (column)
        {
            case 1:
            {
                bool warrPage = TreeTag(tree) == 0;
                foreach (var (magicId, magicName) in HeroMagicCandidatesHandler?.Invoke()
                             ?? Array.Empty<(int, string)>())
                {
                    if (warrPage == IsMagicWarr(magicId))
                        result.Add((magicId, magicName));
                }
                break;
            }
            case 3:
            case 4:
                break;   // 纯 SpinEdit，无候选
            case 5:
            {
                // Delphi: if FTree.Tag = 1 then Low..matMaster else Low..High
                // 战士页 Tag=0 → 落 else（全 4 项）；法师页 Tag=1 → 仅到 matMaster（3 项）
                int high = TreeTag(tree) == 1 ? (int)THeroMagicAttackTarget.matMaster
                                              : (int)THeroMagicAttackTarget.matPartner;
                for (int i = 0; i <= high; i++)
                    result.Add((i, CustomHeroMagicState.MagicAttackTargetNames[i]));
                break;
            }
        }
        return result;
    }

    /// <summary>CustomMagicConfig.IsMagicWarr 接缝（默认非战士）。</summary>
    public Func<int, bool>? IsMagicWarrHandler;

    private bool IsMagicWarr(int magicId) => IsMagicWarrHandler?.Invoke(magicId) ?? false;

    /// <summary>
    /// EndEdit（180-248）1:1：按列写回并按需置 IsChanged + SetCanSave。
    /// 返回是否发生变更。
    /// </summary>
    public bool EndEdit(System.Windows.Forms.ListView tree, int index, int column, int value, string? comboText = null)
    {
        if (index < 0 || index >= _nodeData[tree].Count)
            return false;
        var nodeData = _nodeData[tree][index];
        bool isChanged = false;

        switch (column)
        {
            case 1:
                if (value >= 0)
                {
                    isChanged = nodeData.HeroMagic.MagicID != value;
                    if (isChanged)
                    {
                        nodeData.HeroMagic.MagicID = value;
                        nodeData.MagicName = comboText ?? ResolveMagicName(value);
                    }
                }
                break;
            case 3:
                isChanged = nodeData.HeroMagic.UseRate != value;
                if (isChanged)
                    nodeData.HeroMagic.UseRate = value;
                break;
            case 4:
                isChanged = nodeData.HeroMagic.AttackRange != value;
                if (isChanged)
                    nodeData.HeroMagic.AttackRange = value;
                break;
            case 5:
                isChanged = (int)nodeData.HeroMagic.AttackTarget != value;
                if (isChanged)
                    nodeData.HeroMagic.AttackTarget = (THeroMagicAttackTarget)value;
                break;
        }

        if (isChanged)
        {
            nodeData.HeroMagic.IsChanged = true;
            SetCanSave();
        }

        var item = tree.Items.Count > index ? tree.Items[index] : null;
        if (item != null)
            RefreshRow(tree, item);

        return isChanged;
    }

    // ==================== 节点点击 / 勾选 / 键盘 ====================

    /// <summary>NodeClick（701-729）1:1：列 7 且自定义 → 条件窗体；否则进入该列编辑。</summary>
    public void NodeClick(System.Windows.Forms.ListView tree, int index, int column)
    {
        if (index < 0 || index >= _nodeData[tree].Count || column < 0)
            return;

        var nodeData = _nodeData[tree][index];
        if (column == 7)
        {
            if (!nodeData.HeroMagic.IsCustomMagic)
                return;

            var condition = nodeData.HeroMagic.Condition;
            var (ok, result) = ConditionDialogHandler?.Invoke(condition) ?? (false, condition);
            if (!ok)
                return;

            if (!ConditionEquals(nodeData.HeroMagic.Condition, result))
            {
                nodeData.HeroMagic.Condition = result;
                nodeData.HeroMagic.IsChanged = true;
                SetCanSave();
                var item = tree.Items.Count > index ? tree.Items[index] : null;
                if (item != null)
                    RefreshRow(tree, item);
            }
        }
        else
        {
            // Delphi PostMessage(WM_STARTEDITING_MAGIC) → EditNode(Node, Column)
            LastEditingRequest = (tree, index, column);
        }
    }

    /// <summary>最近一次编辑请求（WM_STARTEDITING_MAGIC 等效，测试用）。</summary>
    public (System.Windows.Forms.ListView Tree, int Index, int Column)? LastEditingRequest;

    /// <summary>Checked（857-878）1:1：勾选态写回 Checked + IsChanged + SetCanSave。</summary>
    public void NodeChecked(System.Windows.Forms.ListView tree, int index, bool isChecked)
    {
        if (index < 0 || index >= _nodeData[tree].Count)
            return;
        var nodeData = _nodeData[tree][index];
        nodeData.HeroMagic.Checked = isChecked;
        nodeData.HeroMagic.IsChanged = true;
        SetCanSave();
    }

    /// <summary>KeyUp（766-837）1:1：Ctrl+Insert 新增自定义；Ctrl+Delete 删自定义。</summary>
    public void KeyUp(System.Windows.Forms.ListView tree, int key, bool ctrl, int focusedIndex)
    {
        if (!ctrl)
            return;

        if (key == (int)System.Windows.Forms.Keys.Insert)
        {
            var heroMagic = Mgr.Add();
            heroMagic.Checked = true;
            heroMagic.MagicType = tree == vstWarrMagic ? THeroMagicType.mtWarrAttack
                : tree == vstWizardMagic ? THeroMagicType.mtWizardAttack
                : THeroMagicType.mtTaosAttack;
            heroMagic.MagicID = 0;
            heroMagic.IsCustomMagic = true;
            heroMagic.UseRate = 0;
            heroMagic.Checked = true;
            heroMagic.AttackTarget = THeroMagicAttackTarget.matEnemy;

            var nodeData = AddNode(tree, heroMagic);
            nodeData.MagicName = "";   // 原文新增后未设 MagicName（首次编辑列 1 才填）
            var item = tree.Items[tree.Items.Count - 1];
            RefreshRow(tree, item);
            if (tree.Items.Count > 0)
            {
                foreach (System.Windows.Forms.ListViewItem it in tree.Items)
                    it.Selected = false;
                item.Selected = true;
                tree.FocusedItem = item;
            }
            LastEditingRequest = (tree, tree.Items.Count - 1, 0);
            SetCanSave();
        }
        else if (key == (int)System.Windows.Forms.Keys.Delete)
        {
            if (focusedIndex < 0 || focusedIndex >= _nodeData[tree].Count)
                return;
            var nodeData = _nodeData[tree][focusedIndex];
            if (!nodeData.HeroMagic.IsCustomMagic)
                return;

            int neighbor = focusedIndex + 1 < _nodeData[tree].Count ? focusedIndex + 1
                : focusedIndex - 1 >= 0 ? focusedIndex - 1 : -1;

            if (Mgr.Remove(nodeData.HeroMagic))
            {
                _nodeData[tree].RemoveAt(focusedIndex);
                tree.Items.RemoveAt(focusedIndex);
                if (neighbor >= 0 && neighbor < tree.Items.Count)
                {
                    foreach (System.Windows.Forms.ListViewItem it in tree.Items)
                        it.Selected = false;
                    tree.Items[neighbor].Selected = true;
                    tree.FocusedItem = tree.Items[neighbor];
                }
                SetCanSave();
            }
        }
    }

    /// <summary>DrawText（880-897）1:1：IsChanged 行标红（WinForms 用 ForeColor 等效）。</summary>
    public System.Drawing.Color RowColor(System.Windows.Forms.ListView tree, int index, bool selectedAndFocused)
    {
        var nodeData = _nodeData[tree][index];
        if (nodeData.HeroMagic.IsChanged)
            return System.Drawing.Color.Red;
        if (selectedAndFocused)
            return System.Drawing.SystemColors.HighlightText;
        return tree.ForeColor;
    }

    // ==================== 保存 ====================

    /// <summary>btnSaveClick（839-850）1:1：落盘 + 禁用保存 + 当前页刷新。</summary>
    public void BtnSaveClick()
    {
        Mgr.SaveToFile();
        btnSave.Enabled = false;
        ActiveTree.Invalidate();
    }

    /// <summary>SetCanSave（852-855）。</summary>
    public void SetCanSave() => btnSave.Enabled = true;

    /// <summary>CompareMem 等效：条件记录逐字段比较。</summary>
    public static bool ConditionEquals(in THeroMagicUseCondition a, in THeroMagicUseCondition b)
        => a.HeroLevelCheck.boChecked == b.HeroLevelCheck.boChecked
           && a.HeroLevelCheck.CompareSymbol == b.HeroLevelCheck.CompareSymbol
           && a.HeroLevelCheck.CompareType == b.HeroLevelCheck.CompareType
           && a.HeroLevelCheck.CompareValue == b.HeroLevelCheck.CompareValue
           && HpEquals(a.HeroHPCheck, b.HeroHPCheck)
           && HpEquals(a.HeroMPCheck, b.HeroMPCheck)
           && HpEquals(a.TargetHPCheck, b.TargetHPCheck)
           && HpEquals(a.TargetMPCheck, b.TargetMPCheck)
           && StatusEquals(a.TargetStatusCheck, b.TargetStatusCheck)
           && a.FriendCountCheck.boChecked == b.FriendCountCheck.boChecked
           && a.FriendCountCheck.nCheckRange == b.FriendCountCheck.nCheckRange
           && a.FriendCountCheck.nCheckValue == b.FriendCountCheck.nCheckValue
           && a.EnemyCountCheck.boChecked == b.EnemyCountCheck.boChecked
           && a.EnemyCountCheck.nCheckRange == b.EnemyCountCheck.nCheckRange
           && a.EnemyCountCheck.nCheckValue == b.EnemyCountCheck.nCheckValue
           && a.boStraightLineCheck == b.boStraightLineCheck;

    private static bool HpEquals(in THeroHPCheck a, in THeroHPCheck b)
        => a.boChecked == b.boChecked && a.CompareSymbol == b.CompareSymbol
           && a.CompareType == b.CompareType && a.CompareValue == b.CompareValue;

    private static bool StatusEquals(in TTargetStatusCheck a, in TTargetStatusCheck b)
        => a.boPoisonDamageArmor == b.boPoisonDamageArmor
           && a.boPoisonDecHealth == b.boPoisonDecHealth
           && a.boPoisoning == b.boPoisoning
           && a.boPoisonStone == b.boPoisonStone
           && a.boFrozen == b.boFrozen
           && a.boForeverFrozen == b.boForeverFrozen
           && a.boCobwebWinding == b.boCobwebWinding
           && a.boUnPoisonDamageArmor == b.boUnPoisonDamageArmor
           && a.boUnPoisonDecHealth == b.boUnPoisonDecHealth
           && a.boUnPoisoning == b.boUnPoisoning
           && a.boUnPoisonStone == b.boUnPoisonStone
           && a.boUnFrozen == b.boUnFrozen
           && a.boUnForeverFrozen == b.boUnForeverFrozen
           && a.boUnCobwebWinding == b.boUnCobwebWinding;

    /// <summary>
    /// ShowFrmHeroMagicSetting（70-80）1:1：ShowModal = mrOK。
    /// </summary>
    public bool ShowFrmHeroMagicSetting()
    {
        var result = ShowModalHandler?.Invoke() ?? ShowDialog();
        return result == System.Windows.Forms.DialogResult.OK;
    }
}
