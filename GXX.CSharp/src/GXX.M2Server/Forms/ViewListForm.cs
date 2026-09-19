using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// ViewList.pas TfrmViewList 核心转换（批次J18，3023 行巨片第一片）：
/// 物品总表 + 禁止制造/允许制造两组名单的 增/全增/删/全删/保存 处理器 1:1，
/// 其余十组名单（GameLog/DisableTakeOff/DisableMoveMap/EnablePickUp/PriorityPickUp 等）
/// 处理器结构相同，随后续巨片拆分逐组接入。
/// </summary>
public sealed class ViewListForm : System.Windows.Forms.Form
{
    private bool boOpened;
    private bool boModValued;

    /// <summary>批次J21：ViewList 其余名单组（源表 + 全局表 + 保存接缝），共用通用五钮处理器。</summary>
    public sealed class NameListGroup
    {
        public required string Name;
        public required GXX.Core.Util.TStringList Source;
        public required GXX.Core.Util.TStringList Target;
        public Action? SaveHandler;
    }

    /// <summary>其余名单组（J21 接入）：GameLog/DisableMoveMap/EnablePickUp/PriorityPickUp/
    /// MoveGuardPick/DisableRangePick/DisableDropToBag/NameFilter。</summary>
    public readonly List<NameListGroup> Groups = new()
    {
        new() { Name = "GameLog", Source = new(), Target = new() },
        new() { Name = "DisableMoveMap", Source = new(), Target = new() },
        new() { Name = "EnablePickUp", Source = new(), Target = new() },
        new() { Name = "PriorityPickUp", Source = new(), Target = new() },
        new() { Name = "MoveGuardPick", Source = new(), Target = new() },
        new() { Name = "DisableRangePick", Source = new(), Target = new() },
        new() { Name = "DisableDropToBag", Source = new(), Target = new() },
        new() { Name = "NameFilter", Source = new(), Target = new() },
    };

    public NameListGroup Group(string name) => Groups.First(g => g.Name == name);

    public System.Windows.Forms.ListBox ListBoxItemList = null!;
    public System.Windows.Forms.ListBox ListBoxEnableMakeList = null!;
    public System.Windows.Forms.ListBox ListBoxDisableMakeList = null!;
    public System.Windows.Forms.Button btnAddEnableMakeItem = null!;
    public System.Windows.Forms.Button btnAddAllEnableMakeItem = null!;
    public System.Windows.Forms.Button btnDelEnableMakeItem = null!;
    public System.Windows.Forms.Button btnDelAllEnableMakeItem = null!;
    public System.Windows.Forms.Button btnSaveEnableMakeItem = null!;
    public System.Windows.Forms.Button btnAddDisableMakeItem = null!;
    public System.Windows.Forms.Button btnDelDisableMakeItem = null!;
    public System.Windows.Forms.Button btnSaveDisableMakeItem = null!;

    /// <summary>UserEngine.StdItemList 名称表接缝。</summary>
    public Func<List<string>>? StdItemNamesHandler;

    /// <summary>地图名列表接缝（MapManager 批次接入真实数据）。</summary>
    public Func<List<string>>? MapNamesHandler;

    /// <summary>怪物名列表接缝（UserEngine.MonsterList 批次接入真实数据）。</summary>
    public Func<List<string>>? MonsterNamesHandler;

    /// <summary>SaveEnableMakeItem 接缝（Delphi 落盘函数）。</summary>
    public Action? SaveEnableMakeItemHandler;

    // ---- 批次J38：剩余 12 组列表框映射 ----

    /// <summary>单组：源列表框 + 目标列表框 + 目标全局表（Sorted 语义按组）。</summary>
    public sealed class Group2
    {
        public string Title = "";
        public System.Windows.Forms.ListBox SourceListBox = null!;
        public System.Windows.Forms.ListBox TargetListBox = null!;
        public GXX.Core.Util.TStringList Target = null!;
        public bool IntMode;
        public bool Sorted;

        public void AddSelected(string text)
        {
            if (!TargetListBox.Items.Contains(text))
                TargetListBox.Items.Add(text);
        }

        public void AddSelectedInt(int idx, string name)
        {
            var text = idx + "  " + name;
            if (!TargetListBox.Items.Contains(text))
                TargetListBox.Items.Add(text);
        }

        public void AddSelectedFromSource()
        {
            if (SourceListBox.SelectedIndex >= 0)
                AddSelected(SourceListBox.Items[SourceListBox.SelectedIndex]?.ToString() ?? "");
        }

        public void ClearTarget() => TargetListBox.Items.Clear();

        /// <summary>保存：目标列表框 → 全局表（Sorted 组排序；IntMode 按前导整数解析）。</summary>
        public void Save()
        {
            Target.Clear();
            foreach (var item in TargetListBox.Items)
            {
                var s = item?.ToString() ?? "";
                if (IntMode)
                {
                    var sp = s.IndexOf(' ');
                    Target.Add(sp > 0 ? s[..sp] : s);
                }
                else
                {
                    Target.Add(s);
                }
            }
            if (Sorted)
                Target.Sort();
        }
    }

    public List<Group2> Groups2 { get; } = new();

    /// <summary>构建剩余 12 组列表框映射（Delphi Open 组加载 1:1 键名）。</summary>
    public void BuildGroups2()
    {
        void AddGroup(string title, GXX.Core.Util.TStringList target, Func<List<string>>? source, bool intMode, bool sorted)
        {
            var g = new Group2
            {
                Title = title,
                Target = target,
                Sorted = sorted,
                SourceListBox = new System.Windows.Forms.ListBox { Width = 200, Height = 200 },
                TargetListBox = new System.Windows.Forms.ListBox { Width = 200, Height = 200 },
            };
            foreach (var name in source?.Invoke() ?? new List<string>())
                g.SourceListBox.Items.Add(name);
            foreach (var s in target.AsEnumerable())
                g.TargetListBox.Items.Add(s);
            Groups2.Add(g);
        }

        AddGroup("禁止取下", ViewListState.g_DisableTakeOffList, StdItemNamesHandler, intMode: true, sorted: true);
        AddGroup("禁止显示来源", ViewListState.g_DisableShowItemFromList, StdItemNamesHandler, intMode: false, sorted: true);
        AddGroup("禁止传送地图", ViewListState.g_DisableMoveMapList, MapNamesHandler, intMode: false, sorted: false);
        AddGroup("允许拾取", ViewListState.g_EnablePickUpItemList, StdItemNamesHandler, intMode: false, sorted: true);
        AddGroup("行卫拾取", ViewListState.g_MoveGuardPickItemList, StdItemNamesHandler, intMode: false, sorted: true);
        AddGroup("优先拾取", ViewListState.g_PriorityPickUpItemList, StdItemNamesHandler, intMode: false, sorted: true);
        AddGroup("禁止范围拾取", ViewListState.g_DisableRangePickItemList, StdItemNamesHandler, intMode: false, sorted: true);
        AddGroup("禁止入包掉落", ViewListState.g_DisableDropToBagItemList, StdItemNamesHandler, intMode: false, sorted: true);
        AddGroup("不清理怪物", ViewListState.g_NoClearMonList, MonsterNamesHandler, intMode: false, sorted: false);
        AddGroup("怪物列表", ViewListState.g_MonList, MonsterNamesHandler, intMode: false, sorted: false);
        AddGroup("管理员列表", ViewListState.g_AdminList, MonsterNamesHandler, intMode: false, sorted: false);
        AddGroup("预览物品怪物", ViewListState.g_PreviewItemMonList, MonsterNamesHandler, intMode: false, sorted: true);
    }

    public ViewListForm()
    {
        InitializeComponent();
        WireGroupSaveHandlers();
    }

    /// <summary>批次J21：各组保存按钮接对应全局表（btnSaveXxxClick 1:1：写回 + Sorted）。</summary>
    private void WireGroupSaveHandlers()
    {
        Group("GameLog").SaveHandler = () =>
            ViewListGroups.SaveLogItemClick(Group("GameLog").Target, ViewListState.g_GameLogItemNameList);
        Group("DisableMoveMap").SaveHandler = () =>
            ViewListGroups.Save(Group("DisableMoveMap").Target, ViewListState.g_DisableMoveMapList);
        Group("EnablePickUp").SaveHandler = () =>
            ViewListGroups.Save(Group("EnablePickUp").Target, ViewListState.g_EnablePickUpItemList);
        Group("PriorityPickUp").SaveHandler = () =>
            ViewListGroups.Save(Group("PriorityPickUp").Target, ViewListState.g_PriorityPickUpItemList);
        Group("MoveGuardPick").SaveHandler = () =>
            ViewListGroups.Save(Group("MoveGuardPick").Target, ViewListState.g_MoveGuardPickItemList);
        Group("DisableRangePick").SaveHandler = () =>
            ViewListGroups.Save(Group("DisableRangePick").Target, ViewListState.g_DisableRangePickItemList);
        Group("DisableDropToBag").SaveHandler = () =>
            ViewListGroups.Save(Group("DisableDropToBag").Target, ViewListState.g_DisableDropToBagItemList);
        Group("NameFilter").SaveHandler = () =>
            ViewListGroups.Save(Group("NameFilter").Target, ViewListState.g_NameFilterList);
    }

    /// <summary>组名 → Delphi 全局表（Open 载入与 Save 写回共用；GameLog 组经 SaveLogItemClick 写回）。</summary>
    private static GXX.Core.Util.TStringList? GroupGlobalList(string name) => name switch
    {
        "GameLog" => ViewListState.g_GameLogItemNameList,
        "DisableMoveMap" => ViewListState.g_DisableMoveMapList,
        "EnablePickUp" => ViewListState.g_EnablePickUpItemList,
        "PriorityPickUp" => ViewListState.g_PriorityPickUpItemList,
        "MoveGuardPick" => ViewListState.g_MoveGuardPickItemList,
        "DisableRangePick" => ViewListState.g_DisableRangePickItemList,
        "DisableDropToBag" => ViewListState.g_DisableDropToBagItemList,
        "NameFilter" => ViewListState.g_NameFilterList,
        _ => null
    };

    private void InitializeComponent()
    {
        Text = "列表管理";
        Width = 640;
        Height = 460;

        ListBoxItemList = new System.Windows.Forms.ListBox { Left = 8, Top = 8, Width = 180, Height = 300, SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended };
        Controls.Add(ListBoxItemList);

        ListBoxEnableMakeList = new System.Windows.Forms.ListBox { Left = 210, Top = 8, Width = 180, Height = 140 };
        Controls.Add(ListBoxEnableMakeList);
        ListBoxDisableMakeList = new System.Windows.Forms.ListBox { Left = 210, Top = 156, Width = 180, Height = 140 };
        Controls.Add(ListBoxDisableMakeList);

        btnAddEnableMakeItem = new System.Windows.Forms.Button { Text = "增加(&A)", Left = 400, Top = 8, Width = 70, Height = 24 };
        btnAddEnableMakeItem.Click += (s, e) => btnAddEnableMakeItemClick(s);
        btnAddAllEnableMakeItem = new System.Windows.Forms.Button { Text = "全加(&L)", Left = 400, Top = 38, Width = 70, Height = 24 };
        btnAddAllEnableMakeItem.Click += (s, e) => btnAddAllEnableMakeItemClick(s);
        btnDelEnableMakeItem = new System.Windows.Forms.Button { Text = "删除(&D)", Left = 400, Top = 68, Width = 70, Height = 24 };
        btnDelEnableMakeItem.Click += (s, e) => btnDelEnableMakeItemClick(s);
        btnDelAllEnableMakeItem = new System.Windows.Forms.Button { Text = "全删(&C)", Left = 400, Top = 98, Width = 70, Height = 24 };
        btnDelAllEnableMakeItem.Click += (s, e) => btnDelAllEnableMakeItemClick(s);
        btnSaveEnableMakeItem = new System.Windows.Forms.Button { Text = "保存允许(&S)", Left = 400, Top = 128, Width = 90, Height = 24, Enabled = false };
        btnSaveEnableMakeItem.Click += (s, e) => btnSaveEnableMakeItemClick(s);
        Controls.Add(btnAddEnableMakeItem);
        Controls.Add(btnAddAllEnableMakeItem);
        Controls.Add(btnDelEnableMakeItem);
        Controls.Add(btnDelAllEnableMakeItem);
        Controls.Add(btnSaveEnableMakeItem);

        btnAddDisableMakeItem = new System.Windows.Forms.Button { Text = "增加禁止(&B)", Left = 400, Top = 160, Width = 90, Height = 24 };
        btnAddDisableMakeItem.Click += (s, e) => btnAddDisableMakeItemClick(s);
        btnDelDisableMakeItem = new System.Windows.Forms.Button { Text = "删除禁止(&E)", Left = 400, Top = 190, Width = 90, Height = 24 };
        btnDelDisableMakeItem.Click += (s, e) => btnDelDisableMakeItemClick(s);
        btnSaveDisableMakeItem = new System.Windows.Forms.Button { Text = "保存禁止(&F)", Left = 400, Top = 220, Width = 90, Height = 24, Enabled = false };
        btnSaveDisableMakeItem.Click += (s, e) => btnSaveDisableMakeItemClick(s);
        Controls.Add(btnAddDisableMakeItem);
        Controls.Add(btnDelDisableMakeItem);
        Controls.Add(btnSaveDisableMakeItem);
    }

    // ================= Delphi 1:1 =================

    public bool IsModValued => boModValued;

    private void ModValue()
    {
        boModValued = true;
        btnSaveEnableMakeItem.Enabled = true;
        btnSaveDisableMakeItem.Enabled = true;
    }

    private void uModValue()
    {
        boModValued = false;
        btnSaveEnableMakeItem.Enabled = false;
        btnSaveDisableMakeItem.Enabled = false;
    }

    /// <summary>Open 1:1 核心子集（物品总表 + 两组名单加载；地图/怪物等列表随巨片接入）。</summary>
    public void Open(bool showModal = true)
    {
        boOpened = false;
        uModValue();

        ListBoxItemList.Items.Clear();
        ListBoxEnableMakeList.Items.Clear();
        ListBoxDisableMakeList.Items.Clear();

        var names = StdItemNamesHandler?.Invoke() ?? new List<string>();
        foreach (var g in Groups)
        {
            g.Source.Clear();
            foreach (var n in names) g.Source.Add(n);
            g.Target.Clear();
            // Delphi Open 1:1：全局表内容载入各名单列表（GameLog→GameLogList、DisableMoveMap→…）
            var global = GroupGlobalList(g.Name);
            if (global == null)
                continue;
            System.Threading.Monitor.Enter(ViewListState.LockObj);
            try
            {
                foreach (var s in global.AsEnumerable())
                    g.Target.Add(s);
            }
            finally { System.Threading.Monitor.Exit(ViewListState.LockObj); }
        }
        foreach (var name in names)
        {
            ListBoxItemList.Items.Add(name);
            ListBoxEnableMakeList.Items.Add(name); // Delphi 1:1：初始把全部物品加入允许制造表
        }

        System.Threading.Monitor.Enter(ViewListState.LockObj);
        try
        {
            foreach (var s in ViewListState.g_EnableMakeItemList.AsEnumerable())
                ListBoxEnableMakeList.Items.Add(s);
        }
        finally { System.Threading.Monitor.Exit(ViewListState.LockObj); }

        System.Threading.Monitor.Enter(ViewListState.LockObj);
        try
        {
            foreach (var s in ViewListState.g_DisableMakeItemList.AsEnumerable())
                ListBoxDisableMakeList.Items.Add(s);
        }
        finally { System.Threading.Monitor.Exit(ViewListState.LockObj); }

        BuildGroups2(); // 批次J38：剩余 12 组列表框映射

        boOpened = true;
        if (showModal)
            ShowDialog();
    }

    public void ListBoxEnableMakeListClick(object? sender)
    {
        if (!boOpened)
            return;
        btnDelEnableMakeItem.Enabled = ListBoxEnableMakeList.SelectedIndex >= 0;
    }

    /// <summary>btnAddEnableMakeItemClick 1:1（把总表中选中项追加到允许制造表，去重）。</summary>
    public void btnAddEnableMakeItemClick(object? sender, int[]? selectedIndices = null)
    {
        var indices = selectedIndices ?? System.Linq.Enumerable.Range(0, ListBoxItemList.Items.Count).ToArray();
        foreach (var i in indices)
        {
            var sItemName = ListBoxItemList.Items[i]?.ToString() ?? "";
            if (ListBoxEnableMakeList.Items.IndexOf(sItemName) < 0)
                ListBoxEnableMakeList.Items.Add(sItemName);
        }
        ModValue();
    }

    /// <summary>btnAddAllEnableMakeItemClick 1:1。</summary>
    public void btnAddAllEnableMakeItemClick(object? sender)
    {
        ListBoxEnableMakeList.Items.Clear();
        for (int i = 0; i < ListBoxItemList.Items.Count; i++)
            ListBoxEnableMakeList.Items.Add(ListBoxItemList.Items[i]);
        ModValue();
    }

    /// <summary>btnDelAllEnableMakeItemClick 1:1。</summary>
    public void btnDelAllEnableMakeItemClick(object? sender)
    {
        ListBoxEnableMakeList.Items.Clear();
        btnDelEnableMakeItem.Enabled = false;
        ModValue();
    }

    /// <summary>btnDelEnableMakeItemClick 1:1。</summary>
    public void btnDelEnableMakeItemClick(object? sender, int? itemIndex = null)
    {
        if (itemIndex.HasValue)
            ListBoxEnableMakeList.SelectedIndex = itemIndex.Value;
        if (ListBoxEnableMakeList.SelectedIndex >= 0)
        {
            ListBoxEnableMakeList.Items.RemoveAt(ListBoxEnableMakeList.SelectedIndex);
            ModValue();
        }
        if (ListBoxEnableMakeList.SelectedIndex < 0)
            btnDelEnableMakeItem.Enabled = false;
    }

    /// <summary>btnSaveEnableMakeItemClick 1:1（写回 g_EnableMakeItemList 并排序 + SaveEnableMakeItem 落盘接缝）。</summary>
    public void btnSaveEnableMakeItemClick(object? sender)
    {
        var list = ViewListState.g_EnableMakeItemList;
        Monitor.Enter(list);
        try
        {
            list.Clear();
            for (int i = 0; i < ListBoxEnableMakeList.Items.Count; i++)
                list.Add(ListBoxEnableMakeList.Items[i]?.ToString() ?? "");
            list.Sorted = true;
        }
        finally { Monitor.Exit(list); }
        SaveEnableMakeItemHandler?.Invoke();
        uModValue();
    }

    // ---- DisableMake 组（与 EnableMake 处理器结构相同） ----

    public void btnAddDisableMakeItemClick(object? sender, int[]? selectedIndices = null)
    {
        var indices = selectedIndices ?? System.Linq.Enumerable.Range(0, ListBoxItemList.Items.Count).ToArray();
        foreach (var i in indices)
        {
            var sItemName = ListBoxItemList.Items[i]?.ToString() ?? "";
            if (ListBoxDisableMakeList.Items.IndexOf(sItemName) < 0)
                ListBoxDisableMakeList.Items.Add(sItemName);
        }
        ModValue();
    }

    public void btnDelDisableMakeItemClick(object? sender, int? itemIndex = null)
    {
        if (itemIndex.HasValue)
            ListBoxDisableMakeList.SelectedIndex = itemIndex.Value;
        if (ListBoxDisableMakeList.SelectedIndex >= 0)
        {
            ListBoxDisableMakeList.Items.RemoveAt(ListBoxDisableMakeList.SelectedIndex);
            ModValue();
        }
        if (ListBoxDisableMakeList.SelectedIndex < 0)
            btnDelDisableMakeItem.Enabled = false;
    }

    /// <summary>btnSaveDisableMakeItemClick 1:1（写回 g_DisableMakeItemList 并排序）。</summary>
    public void btnSaveDisableMakeItemClick(object? sender)
    {
        var list = ViewListState.g_DisableMakeItemList;
        Monitor.Enter(list);
        try
        {
            list.Clear();
            for (int i = 0; i < ListBoxDisableMakeList.Items.Count; i++)
                list.Add(ListBoxDisableMakeList.Items[i]?.ToString() ?? "");
            list.Sorted = true;
        }
        finally { Monitor.Exit(list); }
        uModValue();
    }
}
