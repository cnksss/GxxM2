using GXX.Core.Rtl;
using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>uFrmItemDropLog.pas TFrmItemDropLog 1:1（物品掉落日志查看）。</summary>
public sealed class ItemDropLogForm : System.Windows.Forms.Form
{
    public System.Windows.Forms.DataGridView vstLogs = null!;

    public ItemDropLogForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "掉落日志";
        Width = 620;
        Height = 460;

        vstLogs = new System.Windows.Forms.DataGridView
        {
            Left = 8,
            Top = 8,
            Width = 590,
            Height = 380,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        };
        vstLogs.Columns.Add("c0", "掉落时间");
        vstLogs.Columns.Add("c1", "物品主人");
        vstLogs.Columns.Add("c2", "掉落怪物");
        vstLogs.Columns.Add("c3", "地图");
        vstLogs.Columns.Add("c4", "坐标");
        Controls.Add(vstLogs);
        KeyPress += (s, e) =>
        {
            if (e.KeyChar == (char)27)
                DialogResult = System.Windows.Forms.DialogResult.Cancel; // ModalResult := mrCancel
        };
    }

    /// <summary>LoadLog 1:1（解析到表格，逆序语义由解析器保证，列文本按 vstLogsGetText）。</summary>
    public void LoadLog(string fileName, string mapName)
    {
        var logs = ViewFormsData.LoadItemDropLog(fileName, mapName);
        vstLogs.Rows.Clear();
        foreach (var log in logs)
        {
            var row = vstLogs.Rows.Add(
                ViewFormsData.LogColumnText(log, 0),
                ViewFormsData.LogColumnText(log, 1),
                ViewFormsData.LogColumnText(log, 2),
                ViewFormsData.LogColumnText(log, 3),
                ViewFormsData.LogColumnText(log, 4));
            vstLogs.Rows[row].Tag = log;
        }
    }

    /// <summary>ShowFrmItemDropLog 1:1。</summary>
    public static ItemDropLogForm Show(string itemName, string fileName, string mapName)
    {
        var form = new ItemDropLogForm();
        form.Text = itemName + " 掉落日志";
        form.LoadLog(fileName, mapName);
        return form;
    }
}

/// <summary>uFrmUserShopGetMoneyTotal.pas TFrmUserShopGetMoneyTotal 1:1（寄售未取货币汇总）。</summary>
public sealed class UserShopGetMoneyTotalForm : System.Windows.Forms.Form
{
    public System.Windows.Forms.DataGridView lvMoney = null!;
    public System.Windows.Forms.TextBox edt1 = null!;
    public System.Windows.Forms.TextBox edt2 = null!;
    public System.Windows.Forms.TextBox edt3 = null!;
    public System.Windows.Forms.TextBox edt4 = null!;
    public System.Windows.Forms.TextBox edt5 = null!;

    public UserShopGetMoneyTotalForm()
    {
        Text = "寄售未取货币汇总";
        Width = 560;
        Height = 440;

        lvMoney = new System.Windows.Forms.DataGridView
        {
            Left = 8,
            Top = 8,
            Width = 530,
            Height = 280,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false
        };
        lvMoney.Columns.Add("c0", "物主");
        lvMoney.Columns.Add("c1", M2Config.sGameGoldName);   // 元宝
        lvMoney.Columns.Add("c2", M2Config.sGamePointName);  // 游戏点
        lvMoney.Columns.Add("c3", "金币");
        lvMoney.Columns.Add("c4", M2Config.sGameDiamondName); // 金刚石
        lvMoney.Columns.Add("c5", M2Config.sGameGirdName);   // 灵符
        Controls.Add(lvMoney);

        edt1 = new System.Windows.Forms.TextBox { Left = 120, Top = 300, Width = 180, ReadOnly = true };
        edt2 = new System.Windows.Forms.TextBox { Left = 120, Top = 326, Width = 180, ReadOnly = true };
        edt3 = new System.Windows.Forms.TextBox { Left = 120, Top = 352, Width = 180, ReadOnly = true };
        edt4 = new System.Windows.Forms.TextBox { Left = 120, Top = 378, Width = 180, ReadOnly = true };
        edt5 = new System.Windows.Forms.TextBox { Left = 120, Top = 404, Width = 180, ReadOnly = true };
        Controls.Add(edt1);
        Controls.Add(edt2);
        Controls.Add(edt3);
        Controls.Add(edt4);
        Controls.Add(edt5);
    }

    /// <summary>RefreshData 1:1（分组累计 + 合计写编辑框）。</summary>
    public void RefreshData(List<TSelledAndNoGetMoneyTotal> items)
    {
        lvMoney.Rows.Clear();
        var (rows, sums) = ViewFormsData.RefreshUserShopMoneyTotal(items);
        foreach (var (master, moneys) in rows)
            lvMoney.Rows.Add(master, moneys[0].ToString(), moneys[1].ToString(), moneys[2].ToString(),
                moneys[3].ToString(), moneys[4].ToString());

        edt1.Text = sums[0].ToString();
        edt2.Text = sums[1].ToString();
        edt3.Text = sums[2].ToString();
        edt4.Text = sums[3].ToString();
        edt5.Text = sums[4].ToString();
    }
}

/// <summary>uFrmUserShopView.pas TFrmUserShopView 1:1（用户商铺查看/改名/搜索）。</summary>
public sealed class UserShopViewForm : System.Windows.Forms.Form
{
    private int FSelectShopID = -1;
    private string FSelectUserName = "";
    private string FSelectShopName = "";
    private int FSelectItemIndex = -1;

    public System.Windows.Forms.DataGridView lvItems = null!;
    public System.Windows.Forms.TextBox edtShopName = null!;
    public System.Windows.Forms.TextBox edtUser = null!;
    public System.Windows.Forms.Button btnRename = null!;
    public System.Windows.Forms.Button btnSearch = null!;

    /// <summary>GetNameInFilterList 接缝（非法字符检测，M2Share 批次接入前默认无过滤）。</summary>
    public Func<string, bool>? GetNameInFilterListHandler;

    /// <summary>UserShopDB 接缝：名称占用检查/改名。</summary>
    public Func<string, bool>? ShopNameExistsHandler;
    public Func<int, string, bool>? ShopRenameHandler;

    public Action<string>? ShowMessageHandler;

    public UserShopViewForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "用户商铺";
        Width = 560;
        Height = 460;

        lvItems = new System.Windows.Forms.DataGridView
        {
            Left = 8,
            Top = 8,
            Width = 530,
            Height = 300,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false
        };
        lvItems.Columns.Add("ShopID", "商铺ID");
        lvItems.Columns.Add("Master", "物主");
        lvItems.Columns.Add("ShopName", "店铺名");
        lvItems.Columns.Add("Date", "创建日期");
        lvItems.SelectionChanged += (s, e) => lvItemsSelectItem(s);
        Controls.Add(lvItems);

        edtShopName = new System.Windows.Forms.TextBox { Left = 100, Top = 316, Width = 200, Enabled = false };
        btnRename = new System.Windows.Forms.Button { Text = "改名(&R)", Left = 308, Top = 314, Width = 80, Height = 24, Enabled = false };
        btnRename.Click += (s, e) => btnRenameClick(s);
        Controls.Add(edtShopName);
        Controls.Add(btnRename);

        edtUser = new System.Windows.Forms.TextBox { Left = 100, Top = 346, Width = 200 };
        btnSearch = new System.Windows.Forms.Button { Text = "搜索(&F)", Left = 308, Top = 344, Width = 80, Height = 24 };
        btnSearch.Click += (s, e) => btnSearchClick(s);
        Controls.Add(edtUser);
        Controls.Add(btnSearch);
    }

    /// <summary>FormCreate 1:1（填充商铺列表 + 选择态复位）。</summary>
    public void FormCreate(List<TUserShop> userShopList)
    {
        lvItems.Rows.Clear();
        foreach (var userShop in userShopList)
        {
            var row = lvItems.Rows.Add(userShop.ShopID.ToString(), userShop.sMasterName, userShop.sShopName,
                userShop.dCreateDate.ToString("yyyy/MM/dd"));
            lvItems.Rows[row].Tag = userShop;
        }

        FSelectShopID = -1;
        FSelectUserName = "";
        FSelectShopName = "";
        FSelectItemIndex = -1;
    }

    /// <summary>lvItemsSelectItem 1:1。</summary>
    public void lvItemsSelectItem(object? sender)
    {
        var row = lvItems.CurrentRow;
        if (row != null && row.Tag is TUserShop)
        {
            FSelectShopID = DelphiRTL.StrToIntDef(row.Cells[0].Value?.ToString() ?? "", 0);
            FSelectUserName = row.Cells[1].Value?.ToString() ?? "";
            FSelectShopName = row.Cells[2].Value?.ToString() ?? "";
            FSelectItemIndex = row.Index;

            edtShopName.Text = FSelectShopName;
            edtShopName.Enabled = true;
            btnRename.Enabled = true;
        }
        else
        {
            FSelectShopID = -1;
            FSelectUserName = "";
            FSelectShopName = "";
            FSelectItemIndex = -1;

            edtShopName.Text = "";
            edtShopName.Enabled = false;
            btnRename.Enabled = false;
        }
    }

    /// <summary>btnRenameClick 1:1（占用检查→非法字符→确认→改名）。</summary>
    public (bool ok, string? msg) btnRenameClick(object? sender, bool confirmed = true)
    {
        var newShopName = edtShopName.Text.Trim();

        if (ShopNameExistsHandler?.Invoke(newShopName) ?? false)
        {
            M2Forms.MessageBox(newShopName + " 店铺名已被占用", "提示", M2Forms.MB_OK);
            return (false, newShopName + " 店铺名已被占用");
        }

        if (GetNameInFilterListHandler?.Invoke(newShopName) ?? false)
        {
            M2Forms.MessageBox("商店名称包含禁止的字符", "提示", M2Forms.MB_OK);
            return (false, "商店名称包含禁止的字符");
        }

        if (confirmed)
        {
            var msg = $"是否将用户 {FSelectUserName} 的商铺 \"{FSelectShopName}\" 改名为 \"{newShopName}\"";
            if (M2Forms.MessageBox(msg, "确认改名", M2Forms.MB_YESNO | M2Forms.MB_ICONQUESTION) == M2Forms.IDYES)
            {
                if (ShopRenameHandler?.Invoke(FSelectShopID, newShopName) ?? false)
                {
                    lvItems.Rows[FSelectItemIndex].Cells[2].Value = newShopName;
                    FSelectShopName = newShopName;
                    M2Forms.MessageBox("店铺改名成功", "提示", M2Forms.MB_OK);
                    return (true, "店铺改名成功");
                }
            }
            else
            {
                return (false, null);
            }
        }
        return (false, null);
    }

    /// <summary>btnSearchClick 1:1（SameText 按物主搜索选中行）。</summary>
    public void btnSearchClick(object? sender)
    {
        if (edtUser.Text.Length == 0)
            return;

        foreach (System.Windows.Forms.DataGridViewRow row in lvItems.Rows)
        {
            if (string.Equals(row.Cells[1].Value?.ToString(), edtUser.Text, StringComparison.OrdinalIgnoreCase))
            {
                lvItems.CurrentCell = row.Cells[0];
                row.Selected = true;
                break;
            }
        }
    }
}
