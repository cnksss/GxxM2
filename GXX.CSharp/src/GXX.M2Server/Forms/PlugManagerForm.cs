using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>uFrmPlugManager.pas TFrmPlugManager 核心逻辑 1:1（插件管理：列表/加载/卸载/信息刷新）。
/// g_PluginManager 经 PluginManagerState 注入接缝（插件批次接出前以桩承载 PlugList）。</summary>
public sealed class PlugManagerForm : System.Windows.Forms.Form
{
    /// <summary>Delphi TNodeData。</summary>
    public class NodeData
    {
        public int ID;
        public string PlugName = "";
        public PluginInfo? Plugin;
    }

    /// <summary>TPlugin 桩（IsSysDef/信息展示子集；插件批次接入后扩展）。</summary>
    public class PluginInfo
    {
        public bool IsSysDef;
        public string sFileName = "";
        public string sVersion = "";
        public string sNote = "";
    }

    /// <summary>g_PluginManager.PlugList 接缝（名称 + 可空插件对象）。</summary>
    public readonly List<(string Name, PluginInfo? Plugin)> PlugList = new();

    /// <summary>g_PluginManager.LoadPlugin 接缝。</summary>
    public Func<int, PluginInfo?>? LoadPluginHandler;

    public System.Windows.Forms.DataGridView vstPlug = null!;
    public System.Windows.Forms.TextBox mmoPlugInfo = null!;
    public System.Windows.Forms.Button btnLoadPlug = null!;
    public System.Windows.Forms.Button btnUnloadPlug = null!;
    private int FocusedIndex = -1;

    public PlugManagerForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "插件管理";
        Width = 520;
        Height = 460;

        vstPlug = new System.Windows.Forms.DataGridView
        {
            Left = 8,
            Top = 8,
            Width = 490,
            Height = 280,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false
        };
        vstPlug.Columns.Add("no", "序号");
        vstPlug.Columns.Add("name", "插件名");
        vstPlug.SelectionChanged += (s, e) => vstPlugFocusChanged(s);
        Controls.Add(vstPlug);

        mmoPlugInfo = new System.Windows.Forms.TextBox
        {
            Left = 8,
            Top = 294,
            Width = 490,
            Height = 90,
            Multiline = true,
            ReadOnly = true
        };
        Controls.Add(mmoPlugInfo);

        btnLoadPlug = new System.Windows.Forms.Button { Text = "加载(&L)", Left = 8, Top = 390, Width = 90, Height = 26, Enabled = false };
        btnLoadPlug.Click += (s, e) => mniLoadPlugClick(s);
        btnUnloadPlug = new System.Windows.Forms.Button { Text = "卸载(&U)", Left = 106, Top = 390, Width = 90, Height = 26, Enabled = false };
        btnUnloadPlug.Click += (s, e) => mniUnloadPlugClick(s);
        Controls.Add(btnLoadPlug);
        Controls.Add(btnUnloadPlug);
    }

    private NodeData? NodeAt(int index)
        => index >= 0 && index < PlugList.Count
            ? new NodeData { ID = index, PlugName = PlugList[index].Name, Plugin = PlugList[index].Plugin }
            : null;

    private void SetNodePlugin(int index, PluginInfo? plugin)
    {
        var (name, _) = PlugList[index];
        PlugList[index] = (name, plugin);
    }

    // ================= Delphi 1:1 =================

    /// <summary>FormCreate 1:1（PlugList 全量入表）。</summary>
    public void FormCreate()
    {
        vstPlug.Rows.Clear();
        for (int i = 0; i < PlugList.Count; i++)
            vstPlug.Rows.Add((i + 1).ToString(), PlugList[i].Name);
        mmoPlugInfo.Clear();
    }

    /// <summary>vstPlugFocusChanged 1:1（无焦点禁用；加载按 Plugin==nil，卸载按非 nil 且非系统）。</summary>
    public void vstPlugFocusChanged(object? sender)
    {
        mmoPlugInfo.Clear();
        if (FocusedIndex < 0)
        {
            btnLoadPlug.Enabled = false;
            btnUnloadPlug.Enabled = false;
            return;
        }

        var node = NodeAt(FocusedIndex);
        if (node == null)
        {
            btnLoadPlug.Enabled = false;
            btnUnloadPlug.Enabled = false;
            return;
        }

        btnLoadPlug.Enabled = node.Plugin == null;
        btnUnloadPlug.Enabled = node.Plugin != null && !node.Plugin.IsSysDef;

        if (node.Plugin == null)
            return;
        RefreshPlugInfo(node.Plugin);
    }

    public void RefreshPlugInfo(PluginInfo plugin)
    {
        mmoPlugInfo.Text = plugin.sFileName + "\r\n" + plugin.sVersion + "\r\n" + plugin.sNote;
    }

    /// <summary>mniLoadPlugClick 1:1（Plugin 为 nil 时经 g_PluginManager.LoadPlugin 加载）。</summary>
    public void mniLoadPlugClick(object? sender)
    {
        if (FocusedIndex < 0)
            return;
        var node = NodeAt(FocusedIndex);
        if (node == null || node.Plugin != null)
            return;

        var plugin = LoadPluginHandler?.Invoke(node.ID);
        SetNodePlugin(node.ID, plugin);
        node.Plugin = plugin;

        if (plugin == null)
            return;

        btnLoadPlug.Enabled = plugin == null;
        btnUnloadPlug.Enabled = plugin != null && !plugin.IsSysDef;

        if (plugin != null)
            RefreshPlugInfo(plugin);
        else
            mmoPlugInfo.Clear();
    }

    /// <summary>mniUnloadPlugClick 1:1（系统插件不可卸载；卸载后清 Plugin 与信息）。</summary>
    public void mniUnloadPlugClick(object? sender)
    {
        if (FocusedIndex < 0)
            return;
        var node = NodeAt(FocusedIndex);
        if (node == null)
            return;
        if (node.Plugin == null || node.Plugin.IsSysDef)
            return;

        SetNodePlugin(node.ID, null);
        btnLoadPlug.Enabled = true;
        btnUnloadPlug.Enabled = false;
        mmoPlugInfo.Clear();
    }

    /// <summary>pmPlugListPopup 1:1（右键菜单使能）。</summary>
    public (bool load, bool unload) pmPlugListPopup()
    {
        if (FocusedIndex < 0)
            return (false, false);
        var node = NodeAt(FocusedIndex);
        return (node?.Plugin == null, node?.Plugin != null && node.Plugin!.IsSysDef == false);
    }

    /// <summary>测试/宿主辅助：设置焦点行（Delphi vstPlug.FocusedNode）。</summary>
    public void SetFocusedIndex(int index)
    {
        FocusedIndex = index;
        if (index >= 0 && index < vstPlug.Rows.Count)
            vstPlug.Rows[index].Selected = true;
    }
}

/// <summary>g_PluginManager 桩状态（批次J17 引擎层）。</summary>
public static class PluginManagerState
{
    public static readonly List<(string Name, PlugManagerForm.PluginInfo? Plugin)> PlugList = new();

    public static void Reset()
    {
        PlugList.Clear();
    }

    /// <summary>g_PluginManager.LoadPlugin 桩（Delphi 返回 nil 表示加载失败/未提供 DLL）。</summary>
    public static PlugManagerForm.PluginInfo? LoadPlugin(int id)
        => id >= 0 && id < PlugList.Count ? PlugList[id].Plugin : null;
}
