using GXX.M2Server.Engine;

namespace GXX.M2Server.Forms;

/// <summary>
/// ViewList2.pas TFrmViewList2 第一片（批次J56）：商店列表（六页签 g_SndaShopList 装载/增/删/改/上下移/回填）
/// 与消息过滤（g_FilterTexts 增/删/改/回填）处理器 1:1。其余子列表（箱子/物品规则/用户命令/套装/锻造/
/// 自定义货币/WIL 名单）随后续片接入。
/// </summary>
public sealed partial class ViewList2Form : System.Windows.Forms.Form
{
    // ---- 商店（PageControlShop 六页签 + ListViewShop1..6）----

    public System.Windows.Forms.TabControl PageControlShop = null!;
    public readonly System.Windows.Forms.ListView[] ListViewShop = new System.Windows.Forms.ListView[6];
    public System.Windows.Forms.TextBox EditShopItemName = null!;
    public System.Windows.Forms.NumericUpDown EditShopItemPrice = null!;
    public System.Windows.Forms.TextBox EditItemMemo1 = null!;
    public System.Windows.Forms.ComboBox ComboBoxGameMoney = null!;
    public System.Windows.Forms.ComboBox ComboBoxShopType = null!;
    public System.Windows.Forms.NumericUpDown EditImageIndex = null!;
    public System.Windows.Forms.NumericUpDown EditImageCount = null!;
    public System.Windows.Forms.TextBox MemoShop = null!;
    public System.Windows.Forms.NumericUpDown seItemCount = null!;
    public System.Windows.Forms.NumericUpDown seBulkBuyCount = null!;
    public System.Windows.Forms.CheckBox chkBulkBuy = null!;
    public System.Windows.Forms.Button ButtonShopRefresh = null!;
    public System.Windows.Forms.Button ButtonAddShopItem = null!;
    public System.Windows.Forms.Button ButtonDelShopItem = null!;
    public System.Windows.Forms.Button ButtonShopChgItem = null!;
    public System.Windows.Forms.Button ButtonShopSaveItem = null!;
    public System.Windows.Forms.Button ButtonShopItemUP = null!;
    public System.Windows.Forms.Button ButtonShopItemDOWN = null!;

    public TShopItem? SelShopItem;
    public System.Windows.Forms.ListView? SelShopListView;

    /// <summary>UserEngine.GetStdItem 接缝。</summary>
    public Func<string, TShopStdItemView?>? GetStdItemHandler;

    /// <summary>g_SndaShopList（Delphi 单元全局；窗体直接持有便于测试）。</summary>
    public readonly TSndaShopList g_SndaShopList = new();

    // ---- 消息过滤 ----

    public System.Windows.Forms.ListView ListViewMsgFilter = null!;
    public System.Windows.Forms.TextBox EditFilterMsg = null!;
    public System.Windows.Forms.TextBox EditNewMsg = null!;
    public System.Windows.Forms.Button ButtonMsgFilterAdd = null!;
    public System.Windows.Forms.Button ButtonMsgFilterDel = null!;
    public System.Windows.Forms.Button ButtonMsgFilterChg = null!;
    public System.Windows.Forms.Button ButtonMsgFilterSave = null!;

    /// <summary>g_FilterTexts（Delphi 单元全局）。</summary>
    public readonly TFilterTexts g_FilterTexts = new();

    public ViewList2Form()
    {
        // 商店页
        PageControlShop = new System.Windows.Forms.TabControl();
        for (int i = 0; i < 6; i++)
        {
            var page = new System.Windows.Forms.TabPage($"Page{i}");
            ListViewShop[i] = new System.Windows.Forms.ListView { View = System.Windows.Forms.View.Details };
            ListViewShop[i].Columns.Add("商店");
            ListViewShop[i].Columns.Add("物品");
            ListViewShop[i].Columns.Add("数量");
            ListViewShop[i].Columns.Add("货币");
            ListViewShop[i].Columns.Add("价格");
            ListViewShop[i].Columns.Add("图片");
            ListViewShop[i].Columns.Add("帧数");
            ListViewShop[i].Columns.Add("功能1");
            ListViewShop[i].Columns.Add("功能2");
            var idx = i;
            ListViewShop[i].Click += (_, _) => ListViewShopClick(ListViewShop[idx]);
            page.Controls.Add(ListViewShop[i]);
            PageControlShop.TabPages.Add(page);
        }

        EditShopItemName = new System.Windows.Forms.TextBox();
        EditShopItemPrice = NewSpinEdit();
        EditItemMemo1 = new System.Windows.Forms.TextBox();
        ComboBoxGameMoney = new System.Windows.Forms.ComboBox();
        ComboBoxShopType = new System.Windows.Forms.ComboBox();
        EditImageIndex = NewSpinEdit();
        EditImageCount = NewSpinEdit();
        MemoShop = new System.Windows.Forms.TextBox { Multiline = true };
        seItemCount = NewSpinEdit();
        seBulkBuyCount = NewSpinEdit();
        chkBulkBuy = new System.Windows.Forms.CheckBox();
        chkBulkBuy.CheckedChanged += (_, _) => seBulkBuyCount.Enabled = chkBulkBuy.Checked;

        ButtonShopRefresh = new System.Windows.Forms.Button();
        ButtonShopRefresh.Click += (_, _) => ButtonShopRefreshClick();
        ButtonAddShopItem = new System.Windows.Forms.Button();
        ButtonAddShopItem.Click += (_, _) => ButtonAddShopItemClick();
        ButtonDelShopItem = new System.Windows.Forms.Button { Enabled = false };
        ButtonDelShopItem.Click += (_, _) => ButtonDelShopItemClick();
        ButtonShopChgItem = new System.Windows.Forms.Button { Enabled = false };
        ButtonShopChgItem.Click += (_, _) => ButtonShopChgItemClick();
        ButtonShopSaveItem = new System.Windows.Forms.Button { Enabled = false };
        ButtonShopSaveItem.Click += (_, _) => g_SndaShopList.SaveToFile();
        ButtonShopItemUP = new System.Windows.Forms.Button { Enabled = false };
        ButtonShopItemUP.Click += (_, _) => ShopMoveItem(up: true);
        ButtonShopItemDOWN = new System.Windows.Forms.Button { Enabled = false };
        ButtonShopItemDOWN.Click += (_, _) => ShopMoveItem(up: false);

        // 消息过滤页
        ListViewMsgFilter = new System.Windows.Forms.ListView { View = System.Windows.Forms.View.Details };
        ListViewMsgFilter.Columns.Add("过滤消息");
        ListViewMsgFilter.Columns.Add("替换为");
        ListViewMsgFilter.Click += (_, _) => ListViewMsgFilterClick();
        EditFilterMsg = new System.Windows.Forms.TextBox();
        EditNewMsg = new System.Windows.Forms.TextBox();
        ButtonMsgFilterAdd = new System.Windows.Forms.Button();
        ButtonMsgFilterAdd.Click += (_, _) => ButtonMsgFilterAddClick();
        ButtonMsgFilterDel = new System.Windows.Forms.Button { Enabled = false };
        ButtonMsgFilterDel.Click += (_, _) => ButtonMsgFilterDelClick();
        ButtonMsgFilterChg = new System.Windows.Forms.Button { Enabled = false };
        ButtonMsgFilterChg.Click += (_, _) => ButtonMsgFilterChgClick();
        ButtonMsgFilterSave = new System.Windows.Forms.Button { Enabled = false };
        ButtonMsgFilterSave.Click += (_, _) => g_FilterTexts.SaveToFile();

        Controls.Add(PageControlShop);

        // 批次J57：物品规则/用户命令/宝箱/套装组/WIL 名单/装备技能威力/过滤备注七页
        InitializeResidualSections();
    }

    private int ActivePageIndex => PageControlShop.SelectedIndex;

    // ---- 商店 ----

    /// <summary>ViewList2.pas GetShopType（901）。</summary>
    public static string GetShopType(int nType) => nType switch
    {
        0 => "装饰",
        1 => "补给",
        2 => "强化",
        3 => "好友",
        4 => "限量",
        5 => "奇珍",
        _ => "",
    };

    /// <summary>ViewList2.pas GetGameMoney（919）：元宝/金币/游戏点/金刚石/灵符。</summary>
    public static string GetGameMoney(int nType) => nType switch
    {
        0 => M2Config.sGameGoldName,
        1 => "金币",
        2 => M2Config.sGamePointName,
        3 => M2Config.sGameDiamondName,
        4 => M2Config.sGameGirdName,
        _ => "",
    };

    /// <summary>RefShopList（1099-1153）：六个页签列表按 ShopItem.ShopType 分流装填。</summary>
    public void RefShopList()
    {
        for (int i = 0; i < 6; i++)
            ListViewShop[i].Items.Clear();

        for (int i = 0; i <= 5; i++)
        {
            var itemList = g_SndaShopList.GetList(i);
            if (itemList == null)
                continue;
            foreach (var shopItem in itemList)
            {
                var listView = shopItem.ShopType switch
                {
                    0 => ListViewShop[0],
                    1 => ListViewShop[1],
                    2 => ListViewShop[2],
                    3 => ListViewShop[3],
                    4 => ListViewShop[4],
                    5 => ListViewShop[5],
                    _ => null,
                };
                if (listView == null)
                    continue;
                var item = listView.Items.Add(GetShopType(i));
                item.Tag = shopItem;
                item.SubItems.Add(shopItem.StdItem.Name);
                item.SubItems.Add(shopItem.ItemCount.ToString());
                item.SubItems.Add(GetGameMoney(shopItem.GameMoney));
                item.SubItems.Add(shopItem.StdItem.Price.ToString());
                item.SubItems.Add(shopItem.ImageIndex.ToString());
                item.SubItems.Add(shopItem.ImageCount.ToString());
                item.SubItems.Add(shopItem.Memo1);
                item.SubItems.Add(shopItem.Memo2);
            }
        }
    }

    public void ButtonShopRefreshClick()
    {
        g_SndaShopList.LoadFromFile();
        RefShopList();
    }

    /// <summary>ButtonAddShopItemClick（1637-1717）：六重校验后入表。</summary>
    public void ButtonAddShopItemClick()
    {
        string sItemName = EditShopItemName.Text.Trim();
        int nPrice = (int)EditShopItemPrice.Value;
        string sMemo = EditItemMemo1.Text.Trim();

        if (ActivePageIndex is < 0 or > 5)
        {
            M2Forms.ErrorBox("请选择物品类别！");
            return;
        }
        var stdItem = GetStdItemHandler?.Invoke(sItemName);
        if (sItemName.Length == 0 || stdItem == null)
        {
            M2Forms.ErrorBox("请选择一个正确的物品！");
            return;
        }
        if (ComboBoxGameMoney.SelectedIndex < 0)
        {
            M2Forms.ErrorBox("请选择一个正确的交易货币！");
            return;
        }
        if (sMemo.Length == 0)
        {
            M2Forms.ErrorBox("请输入物品功能！");
            return;
        }
        if (nPrice <= 0)
        {
            M2Forms.ErrorBox("请输入正确的物品价格！");
            return;
        }
        if (g_SndaShopList.Get(sItemName) != null)
        {
            M2Forms.ErrorBox("该物品已经在列表中了！");
            return;
        }

        var shopItem = new TShopItem
        {
            ShopType = ActivePageIndex,
            // Delphi ShopItem.StdItem := StdItem^ 值拷贝后覆写价格
            StdItem = new TShopStdItemView { Name = stdItem.Name, Looks = stdItem.Looks, Price = nPrice },
            GameMoney = ComboBoxGameMoney.SelectedIndex,
            ImageIndex = (int)EditImageIndex.Value,
            ImageCount = (int)EditImageCount.Value,
            Memo1 = sMemo,
            ItemCount = (int)seItemCount.Value,
            boBulkBuy = chkBulkBuy.Checked,
            nBulkBuyCount = (int)seBulkBuyCount.Value,
        };
        seBulkBuyCount.Enabled = shopItem.boBulkBuy;

        int nCount = Math.Min(MemoShop.Lines.Length, 6);
        string memo2 = "";
        for (int i = 0; i < nCount; i++)
            memo2 += MemoShop.Lines[i] + "\r\n";
        shopItem.Memo2 = memo2;

        if (g_SndaShopList.Add(shopItem))
        {
            RefShopList();
            ButtonShopSaveItem.Enabled = true;
        }
        else
        {
            M2Forms.ErrorBox("增加失败！");
        }
    }

    public void ButtonDelShopItemClick()
    {
        if (SelShopItem != null)
        {
            if (g_SndaShopList.Delete(SelShopItem))
            {
                SelShopItem = null;
                ButtonShopSaveItem.Enabled = true;
                ButtonShopChgItem.Enabled = false;
                RefShopList();
            }
        }
    }

    /// <summary>ButtonShopChgItemClick（2729-2799）：按编辑区当前值改写选中项（不改 ShopType），Memo2 取前 min(6, 行数) 行。</summary>
    public void ButtonShopChgItemClick()
    {
        if (SelShopItem == null)
        {
            M2Forms.ErrorBox("请选择一个商品！");
            return;
        }
        string sItemName = EditShopItemName.Text.Trim();
        int nShopType = ActivePageIndex;
        int nPrice = (int)EditShopItemPrice.Value;
        string sMemo = EditItemMemo1.Text.Trim();

        if (nShopType is < 0 or > 5)
        {
            M2Forms.ErrorBox("请选择物品类别！");
            return;
        }
        var stdItem = GetStdItemHandler?.Invoke(sItemName);
        if (sItemName.Length == 0 || stdItem == null)
        {
            M2Forms.ErrorBox("请选择一个正确的物品！");
            return;
        }
        if (ComboBoxGameMoney.SelectedIndex < 0)
        {
            M2Forms.ErrorBox("请选择一个正确的交易货币！");
            return;
        }
        if (sMemo.Length == 0)
        {
            M2Forms.ErrorBox("请输入物品功能！");
            return;
        }
        if (nPrice <= 0)
        {
            M2Forms.ErrorBox("请输入正确的物品价格！");
            return;
        }

        int nCount = Math.Min(MemoShop.Lines.Length, 6);
        SelShopItem.StdItem.Price = nPrice;
        SelShopItem.GameMoney = ComboBoxGameMoney.SelectedIndex;
        SelShopItem.ImageIndex = (int)EditImageIndex.Value;
        SelShopItem.ImageCount = (int)EditImageCount.Value;
        SelShopItem.Memo1 = sMemo;
        SelShopItem.ItemCount = (int)seItemCount.Value;
        SelShopItem.boBulkBuy = chkBulkBuy.Checked;
        SelShopItem.nBulkBuyCount = (int)seBulkBuyCount.Value;
        seBulkBuyCount.Enabled = SelShopItem.boBulkBuy;

        string memo2 = "";
        for (int i = 0; i < nCount; i++)
            memo2 += MemoShop.Lines[i] + "\r\n";
        SelShopItem.Memo2 = memo2;

        RefShopList();
        ButtonShopSaveItem.Enabled = true;
    }

    /// <summary>ListViewShopClick（ListViewShop1Click 共用体，1732-1768）：选中回填编辑区与按钮态。</summary>
    public void ListViewShopClick(System.Windows.Forms.ListView sender)
    {
        SelShopListView = sender;
        var listItem = GetSelectedItem(sender);
        if (listItem != null && listItem.Tag is TShopItem shopItem)
        {
            SelShopItem = shopItem;
            ComboBoxShopType.SelectedIndex = shopItem.ShopType;
            ComboBoxGameMoney.SelectedIndex = shopItem.GameMoney;
            EditShopItemName.Text = shopItem.StdItem.Name;
            EditShopItemPrice.Value = Clamp(shopItem.StdItem.Price, EditShopItemPrice);
            EditImageIndex.Value = Clamp(shopItem.ImageIndex, EditImageIndex);
            EditImageCount.Value = Clamp(shopItem.ImageCount, EditImageCount);
            EditItemMemo1.Text = shopItem.Memo1;
            MemoShop.Lines = SplitLines(shopItem.Memo2);
            seItemCount.Value = Clamp(shopItem.ItemCount, seItemCount);
            chkBulkBuy.Checked = shopItem.boBulkBuy;
            seBulkBuyCount.Value = Clamp(shopItem.nBulkBuyCount, seBulkBuyCount);
            seBulkBuyCount.Enabled = shopItem.boBulkBuy;

            ButtonDelShopItem.Enabled = true;
            ButtonShopChgItem.Enabled = true;
            int itemIndex = listItem.Index;
            ButtonShopItemDOWN.Enabled = itemIndex >= 0 && itemIndex < sender.Items.Count - 1;
            ButtonShopItemUP.Enabled = itemIndex > 0;
        }
        else
        {
            SelShopItem = null;
            SelShopListView = null;
            ButtonDelShopItem.Enabled = false;
            ButtonShopChgItem.Enabled = false;
        }
    }

    /// <summary>ButtonShopItemUP/DOWNClick：g_SndaShopList.Up/Down 后刷新。</summary>
    public void ShopMoveItem(bool up)
    {
        if (SelShopItem == null)
            return;
        if (up)
            g_SndaShopList.Up(SelShopItem);
        else
            g_SndaShopList.Down(SelShopItem);
        RefShopList();
    }

    private static decimal Clamp(int value, System.Windows.Forms.NumericUpDown control)
        => Math.Clamp(value, control.Minimum, control.Maximum);

    /// <summary>无句柄时 SelectedItems 不更新——回退扫描 Items 的 Selected 标志（headless 测试路径）。</summary>
    private static System.Windows.Forms.ListViewItem? GetSelectedItem(System.Windows.Forms.ListView listView)
    {
        if (listView.SelectedItems.Count > 0)
            return listView.SelectedItems[0];
        foreach (System.Windows.Forms.ListViewItem item in listView.Items)
        {
            if (item.Selected)
                return item;
        }
        return null;
    }

    /// <summary>TSpinEdit 等价：宽范围（默认 NumericUpDown 上限 100 不足以容纳 ImageIndex=380 等）。</summary>
    private static System.Windows.Forms.NumericUpDown NewSpinEdit()
        => new() { Minimum = -1000000000, Maximum = 1000000000 };

    private static string[] SplitLines(string text)
        => text.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');

    // ---- 消息过滤 ----

    /// <summary>RefFilterMsgList（1062-1080）：重装列表并保持选中索引。</summary>
    public void RefFilterMsgList()
    {
        int nIndex = GetSelectedItem(ListViewMsgFilter)?.Index ?? -1;
        ListViewMsgFilter.Items.Clear();
        for (int i = 0; i < g_FilterTexts.Count; i++)
        {
            var filterMsg = g_FilterTexts.GetItems(i);
            var item = ListViewMsgFilter.Items.Add(filterMsg.Msg);
            item.Tag = filterMsg;
            item.SubItems.Add(filterMsg.Replace);
        }
        if (nIndex >= 0 && nIndex < ListViewMsgFilter.Items.Count)
            ListViewMsgFilter.Items[nIndex].Selected = true;
    }

    /// <summary>ButtonMsgFilterAddClick（2013-2040）。</summary>
    public void ButtonMsgFilterAddClick()
    {
        string sFilterMsg = EditFilterMsg.Text.Trim();
        string sNewFilterMsg = EditNewMsg.Text.Trim();
        if (sFilterMsg.Length == 0)
        {
            M2Forms.ErrorBox("请输入过滤消息！");
            return;
        }
        if (g_FilterTexts.Find(sFilterMsg))
        {
            M2Forms.ErrorBox("此过滤消息已经存在！");
            return;
        }
        if (g_FilterTexts.Add(sFilterMsg, sNewFilterMsg))
        {
            RefFilterMsgList();
            ButtonMsgFilterSave.Enabled = true;
        }
        else
        {
            M2Forms.ErrorBox("增加失败！");
        }
    }

    /// <summary>ButtonMsgFilterDelClick（2042-2063）。</summary>
    public void ButtonMsgFilterDelClick()
    {
        if (GetSelectedItem(ListViewMsgFilter) is { Tag: TFilterMsg filterMsg })
        {
            if (g_FilterTexts.Delete(filterMsg.Msg))
            {
                RefFilterMsgList();
                ButtonMsgFilterChg.Enabled = false;
                ButtonMsgFilterDel.Enabled = false;
                ButtonMsgFilterSave.Enabled = true;
            }
            else
            {
                M2Forms.ErrorBox("删除失败！");
            }
        }
    }

    /// <summary>ButtonMsgFilterChgClick（2065-2084）：查得过滤词且有选中行 → 替换词回写。</summary>
    public void ButtonMsgFilterChgClick()
    {
        string sFilterMsg = EditFilterMsg.Text.Trim();
        string sNewFilterMsg = EditNewMsg.Text.Trim();
        if (g_FilterTexts.Find(sFilterMsg))
        {
            if (GetSelectedItem(ListViewMsgFilter) is { Tag: TFilterMsg filterMsg })
            {
                filterMsg.Replace = sNewFilterMsg;
                RefFilterMsgList();
                ButtonMsgFilterSave.Enabled = true;
            }
        }
    }

    /// <summary>ListViewMsgFilterClick（2086-2105）：选中回填编辑区，未选中禁用改/删。</summary>
    public void ListViewMsgFilterClick()
    {
        if (GetSelectedItem(ListViewMsgFilter) is { Tag: TFilterMsg filterMsg })
        {
            EditFilterMsg.Text = filterMsg.Msg;
            EditNewMsg.Text = filterMsg.Replace;
            ButtonMsgFilterChg.Enabled = true;
            ButtonMsgFilterDel.Enabled = true;
        }
        else
        {
            ButtonMsgFilterChg.Enabled = false;
            ButtonMsgFilterDel.Enabled = false;
        }
    }
}
