using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>uFrmStorageItemsView.pas TFrmStorageItemsView 1:1（仓库物品查看/删除/搜索）。
/// 仓储与在线玩家接缝经委托注入（UserEngine.GetPlayObject/g_M2DataDB.StorageDB 批次接出）。</summary>
public sealed class StorageItemsViewForm : System.Windows.Forms.Form
{
    private string FUserName = "";
    public string CurrentUserName => FUserName;

    public System.Windows.Forms.ListBox lstUsers = null!;
    public System.Windows.Forms.DataGridView lvItems = null!;
    public System.Windows.Forms.TextBox edtUser = null!;
    public System.Windows.Forms.Button btnDel = null!;
    public System.Windows.Forms.Button btnDelAll = null!;
    public System.Windows.Forms.Button btnSearch = null!;

    /// <summary>在线玩家仓库接缝：返回玩家背包物品列表（在线时）。</summary>
    public Func<string, List<StorageItemRow>?>? GetPlayObjectStorageHandler;
    /// <summary>仓储 DB 接缝：LoadStorageItems。</summary>
    public Func<string, List<StorageItemRow>?>? LoadStorageItemsHandler;
    /// <summary>仓储 DB 接缝：DeleteStorageItem。</summary>
    public Action<int, string, int, int>? DeleteStorageItemHandler;
    /// <summary>仓储 DB 接缝：ClearStorageItem。</summary>
    public Action<int, string>? ClearStorageItemHandler;
    /// <summary>UserEngine.GetPlayObject 接缝（在线判定）。</summary>
    public Func<string, bool>? GetPlayObjectExistsHandler;
    /// <summary>GetStdItem 接缝：wIndex → 物品名。</summary>
    public Func<ushort, string?>? GetStdItemNameHandler;

    /// <summary>仓库物品行（序号/名称/MakeIndex/wIndex+1/Dura/DuraMax）。</summary>
    public class StorageItemRow
    {
        public int No;
        public string Name = "";
        public int MakeIndex;
        public int WIndex;
        public int Dura;
        public int DuraMax;
    }

    public StorageItemsViewForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "仓库物品查看";
        Width = 620;
        Height = 480;

        lstUsers = new System.Windows.Forms.ListBox { Left = 8, Top = 8, Width = 180, Height = 380 };
        lstUsers.SelectedIndexChanged += (s, e) => lstUsersClick(s);
        Controls.Add(lstUsers);

        lvItems = new System.Windows.Forms.DataGridView
        {
            Left = 196,
            Top = 8,
            Width = 400,
            Height = 380,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false
        };
        lvItems.Columns.Add("no", "序号");
        lvItems.Columns.Add("name", "名称");
        lvItems.Columns.Add("make", "MakeIndex");
        lvItems.Columns.Add("idx", "Idx+1");
        lvItems.Columns.Add("dura", "持久");
        lvItems.Columns.Add("duramax", "最大持久");
        Controls.Add(lvItems);

        btnDel = new System.Windows.Forms.Button { Text = "删除(&D)", Left = 196, Top = 396, Width = 80, Height = 26, Enabled = false };
        btnDel.Click += (s, e) => btnDelClick(s);
        btnDelAll = new System.Windows.Forms.Button { Text = "删除全部(&A)", Left = 284, Top = 396, Width = 90, Height = 26, Enabled = false };
        btnDelAll.Click += (s, e) => btnDelAllClick(s);
        btnSearch = new System.Windows.Forms.Button { Text = "搜索(&F)", Left = 380, Top = 396, Width = 80, Height = 26 };
        btnSearch.Click += (s, e) => btnSearchClick(s);
        edtUser = new System.Windows.Forms.TextBox { Left = 196, Top = 430, Width = 200 };
        Controls.Add(edtUser);
        Controls.Add(btnDel);
        Controls.Add(btnDelAll);
        Controls.Add(btnSearch);
    }

    /// <summary>Delphi lvItems.ItemIndex 等效（无句柄环境用显式索引承载）。</summary>
    public int SelectedItemIndex = -1;
    private int lvItemsIndex => SelectedItemIndex;
    private int lvItemsCount => lvItems.Rows.Count;
    private bool CanDel => lvItemsIndex >= 0 && lvItemsIndex < lvItemsCount;

    private void UpdateButtonStates()
    {
        btnDel.Enabled = CanDel;
        btnDelAll.Enabled = lvItemsCount > 0;
    }

    /// <summary>ShowFrmStorageItemsView 入口 1:1（加载全部人物 + 按钮初始态）。</summary>
    public void Open(IEnumerable<string> allHumans, bool showModal = true)
    {
        FUserName = "";
        foreach (var human in allHumans)
            lstUsers.Items.Add(human);
        UpdateButtonStates();
        if (showModal)
            ShowDialog();
    }

    private List<StorageItemRow>? LoadItemsFor(string userName)
    {
        if (GetPlayObjectExistsHandler?.Invoke(userName) ?? false)
            return GetPlayObjectStorageHandler?.Invoke(userName);
        return LoadStorageItemsHandler?.Invoke(userName);
    }

    /// <summary>lstUsersClick 1:1（清表 → 在线/离线两路取仓库 → 填充六列）。</summary>
    public void lstUsersClick(object? sender, int? index = null)
    {
        lvItems.Rows.Clear();

        var selIdx = index ?? lstUsers.SelectedIndex;
        if (selIdx >= 0 && selIdx < lstUsers.Items.Count)
        {
            FUserName = lstUsers.Items[selIdx]?.ToString() ?? "";
            var tempList = LoadItemsFor(FUserName);

            if (tempList != null)
            {
                for (int i = 0; i < tempList.Count; i++)
                {
                    var userItem = tempList[i];
                    var name = GetStdItemNameHandler?.Invoke((ushort)userItem.WIndex) ?? "";
                    lvItems.Rows.Add((i + 1).ToString(), name, userItem.MakeIndex.ToString(),
                        (userItem.WIndex + 1).ToString(), userItem.Dura.ToString(), userItem.DuraMax.ToString());
                }
            }
        }

        UpdateButtonStates();
    }

    /// <summary>btnDelClick 1:1（按 MakeIndex 匹配删除 + DeleteStorageItem）。</summary>
    public void btnDelClick(object? sender)
    {
        if (!CanDel)
            return;
        var nMakeIndex = DelphiRTL_StrToIntDef(lvItems.Rows[lvItemsIndex].Cells[2].Value?.ToString());
        lvItems.Rows.RemoveAt(lvItemsIndex);

        if (FUserName != "")
        {
            var tempList = LoadItemsFor(FUserName);
            if (tempList != null)
            {
                foreach (var userItem in tempList)
                {
                    if (userItem.MakeIndex == nMakeIndex)
                    {
                        tempList.Remove(userItem);
                        DeleteStorageItemHandler?.Invoke(0, FUserName, userItem.MakeIndex, userItem.WIndex);
                        break;
                    }
                }
            }
        }

        UpdateButtonStates();
    }

    /// <summary>btnDelAllClick 1:1（确认后清空 + ClearStorageItem）。</summary>
    public void btnDelAllClick(object? sender, bool confirmed = true)
    {
        if (lvItemsCount <= 0)
            return;
        if (!confirmed)
            return;

        lvItems.Rows.Clear();
        if (FUserName != "")
        {
            var tempList = LoadItemsFor(FUserName);
            tempList?.Clear();
            ClearStorageItemHandler?.Invoke(0, FUserName);
        }

        UpdateButtonStates();
    }

    /// <summary>btnSearchClick 1:1（SameText 找人并触发 OnClick）。</summary>
    public void btnSearchClick(object? sender)
    {
        if (edtUser.Text.Length > 0)
        {
            for (int i = 0; i < lstUsers.Items.Count; i++)
            {
                if (string.Equals(lstUsers.Items[i]?.ToString(), edtUser.Text, StringComparison.OrdinalIgnoreCase))
                {
                    lstUsers.SelectedIndex = i;
                    lstUsersClick(lstUsers);
                    break;
                }
            }
        }
    }

    private static int DelphiRTL_StrToIntDef(string? s) => int.TryParse(s, out var v) ? v : 0;
}

/// <summary>仓储物品行（StorageDB 数据形态）。</summary>
public class StorageDBItem
{
    public int MakeIndex;
    public int WIndex;
    public int Dura;
    public int DuraMax;
}
