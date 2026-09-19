using System.Text;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J56：ViewList2.pas 第一片（商店列表 TSndaShopList + 消息过滤 TFilterTexts）1:1 测试。</summary>
public sealed class FormJ56Tests : IDisposable
{
    private readonly string _tempDir;
    private readonly ViewList2Form _form;

    public FormJ56Tests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "j56_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
        M2ShareState.ResetForTests(_tempDir);
        M2Config.sEnvirDir = _tempDir + Path.DirectorySeparatorChar;
        M2ShareGlobals.ResetNames();
        SndaShopEnv.Reset();

        _form = StaRunner.New(() =>
        {
            var f = new ViewList2Form();
            f.ComboBoxGameMoney.Items.AddRange(new object[] { "元宝", "金币", "游戏点", "金刚石", "灵符" });
            f.ComboBoxShopType.Items.AddRange(new object[] { "装饰", "补给", "强化", "好友", "限量", "奇珍" });
            return f;
        });
    }

    public void Dispose()
    {
        StaRunner.New(() => _form.Dispose());
        SndaShopEnv.Reset();
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_tempDir, true); } catch { }
    }

    private static TShopStdItemView Item(string name, int looks = 0, int price = 0)
        => new() { Name = name, Looks = looks, Price = price };

    // ---- TSndaShopList 数据层 ----

    [Fact]
    public void ShopList_Add_DupRejectsCaseInsensitive()
    {
        var list = new TSndaShopList();
        Assert.True(list.Add(new TShopItem { ShopType = 0, StdItem = Item("木剑") }));
        Assert.False(list.Add(new TShopItem { ShopType = 3, StdItem = Item("木剑") }));  // 跨页签同名拒绝
        Assert.False(list.Add(new TShopItem { ShopType = 1, StdItem = Item("木剑".ToUpper()) }));
        Assert.False(list.Add(new TShopItem { ShopType = 9, StdItem = Item("屠刀") }));  // ShopType 越界
        Assert.Equal(1, list.RecordCount);
    }

    [Fact]
    public void ShopList_GetGetExDelete()
    {
        var list = new TSndaShopList();
        var a = new TShopItem { ShopType = 1, StdItem = Item("金创药"), GameMoney = 2 };
        list.Add(a);
        Assert.Same(a, list.Get("金创药"));
        Assert.Null(list.GetEx("金创药", 0)); // a 只有 money=2
        Assert.Same(a, list.GetEx("金创药", 2));

        // Add 全表查重（Delphi 原义）：同名不同页签/不同货币同样拒绝
        Assert.False(list.Add(new TShopItem { ShopType = 2, StdItem = Item("金创药"), GameMoney = 0 }));

        Assert.True(list.Delete(a));
        Assert.False(list.Delete(a)); // 已移除
        Assert.Null(list.Get("金创药"));
    }

    [Fact]
    public void ShopList_UpDown_Move()
    {
        var list = new TSndaShopList();
        var a = new TShopItem { ShopType = 0, StdItem = Item("A") };
        var b = new TShopItem { ShopType = 0, StdItem = Item("B") };
        var c = new TShopItem { ShopType = 0, StdItem = Item("C") };
        list.Add(a);
        list.Add(b);
        list.Add(c);

        list.Up(c);   // C 到中间
        Assert.Equal(new[] { "A", "C", "B" }, list.GetList(0)!.Select(x => x.StdItem.Name).ToArray());
        list.Down(a); // A 到中间
        Assert.Equal(new[] { "C", "A", "B" }, list.GetList(0)!.Select(x => x.StdItem.Name).ToArray());

        list.Up(a);   // a@1 移回首 位 → A,C,B
        Assert.Equal(new[] { "A", "C", "B" }, list.GetList(0)!.Select(x => x.StdItem.Name).ToArray());

        var solo = new TSndaShopList();
        var s = new TShopItem { ShopType = 4, StdItem = Item("S") };
        solo.Add(s);
        solo.Up(s);   // 单元素不动
        solo.Down(s);
        Assert.Single(solo.GetList(4)!);
    }

    [Fact]
    public void ShopList_UpdateStdItem_KeepsPrice()
    {
        var list = new TSndaShopList();
        var shopItem = new TShopItem { ShopType = 0, StdItem = Item("木剑", looks: 5, price: 888) };
        list.Add(shopItem);

        list.UpdateStdItem(Item("木剑", looks: 9, price: 1));
        Assert.Equal(888, shopItem.StdItem.Price); // 价格保留
        Assert.Equal(9, shopItem.StdItem.Looks);
    }

    [Fact]
    public void ShopList_LoadFromFile_ParsesDefaultsAndGates()
    {
        var path = Path.Combine(_tempDir, "ShopItemList.txt");
        File.WriteAllLines(path, new[]
        {
            "0\t木剑\t10\t100|1\t0\t0\t回复HP|可叠加|第二行\t1\t1\t99",   // 第4列 price|money；ImageIndex=0→380、ImageCount=0→1、memo2 含 |
            "1\t金创药\t20\t50|0\t381\t2\t恢复\t1\t1\t5",                // 常规
            "6\t越界\t1\t1|1\t1\t1\t备注\t1\t0\t1",                      // ShopType=6 丢弃
            "2\t无价格\t1\t-1|1\t1\t1\t备注\t1\t0\t1",                    // Price=-1 丢弃
            "4\t未知物品\t1\t1|1\t1\t1\t备注\t1\t0\t1",                   // GetStdItem=nil 丢弃
        }, GXX.Core.EncodingInit.GBK);

        SndaShopEnv.GetStdItemFn = name => name == "未知物品" ? null : Item(name, looks: 1);

        var list = new TSndaShopList();
        list.LoadFromFile();

        Assert.Equal(2, list.RecordCount);
        var mujian = list.Get("木剑")!;
        Assert.Equal(0, mujian.ShopType);
        Assert.Equal(380, mujian.ImageIndex); // 0 → 380 缺省
        Assert.Equal(1, mujian.ImageCount);
        Assert.Equal(100, mujian.StdItem.Price);
        Assert.Equal(1, mujian.GameMoney);
        Assert.Equal("回复HP", mujian.Memo1);
        Assert.Equal("可叠加\r\n第二行", mujian.Memo2); // '|' → CRLF
        Assert.Equal(1, mujian.ItemCount);

        var yaodao = list.Get("金创药")!;
        Assert.Equal(1, yaodao.ShopType);
        Assert.Equal(381, yaodao.ImageIndex);
        Assert.Equal(50, yaodao.StdItem.Price);
    }

    [Fact]
    public void ShopList_SaveLoad_RoundTrip()
    {
        var list = new TSndaShopList();
        list.Add(new TShopItem
        {
            ShopType = 2,
            StdItem = Item("修罗", looks: 7, price: 1234),
            GameMoney = 3,
            ImageIndex = 400,
            ImageCount = 5,
            Memo1 = "攻杀",
            Memo2 = "行一\r\n行二",
            ItemCount = 2,
            boBulkBuy = true,
            nBulkBuyCount = 10,
        });
        list.SaveToFile();

        var path = Path.Combine(_tempDir, "ShopItemList.txt");
        Assert.True(File.Exists(path));
        var line = File.ReadAllLines(path, GXX.Core.EncodingInit.GBK).Single();
        Assert.Equal("2\t修罗\t7\t1234|3\t400\t5\t攻杀|行一|行二\t2\t1\t10", line);

        var reloaded = new TSndaShopList();
        SndaShopEnv.GetStdItemFn = name => Item(name);
        reloaded.LoadFromFile();
        var item = reloaded.Get("修罗")!;
        Assert.Equal("行一\r\n行二", item.Memo2); // '|' → CRLF 往返
        Assert.Equal(1234, item.StdItem.Price);
        Assert.True(item.boBulkBuy);
        Assert.Equal(10, item.nBulkBuyCount);
    }

    // ---- 窗体：商店页 ----

    [Fact]
    public void GetShopType_And_GetGameMoney_Tables()
    {
        Assert.Equal("装饰", ViewList2Form.GetShopType(0));
        Assert.Equal("奇珍", ViewList2Form.GetShopType(5));
        Assert.Equal("", ViewList2Form.GetShopType(9));

        Assert.Equal("元宝", ViewList2Form.GetGameMoney(0));   // sGameGoldName
        Assert.Equal("金币", ViewList2Form.GetGameMoney(1));   // sSTRING_GOLDNAME
        Assert.Equal("游戏点", ViewList2Form.GetGameMoney(2));
        Assert.Equal("金刚石", ViewList2Form.GetGameMoney(3));
        Assert.Equal("灵符", ViewList2Form.GetGameMoney(4));
        Assert.Equal("", ViewList2Form.GetGameMoney(7));
    }

    [Fact]
    public void RefShopList_RoutesByShopType()
    {
        StaRunner.New(() =>
        {
            _form.g_SndaShopList.Add(new TShopItem { ShopType = 0, StdItem = Item("木剑", price: 100), GameMoney = 1, ItemCount = 1 });
            _form.g_SndaShopList.Add(new TShopItem { ShopType = 5, StdItem = Item("金创药", price: 50), GameMoney = 0, ItemCount = 3 });
            _form.RefShopList();

            Assert.Single(_form.ListViewShop[0].Items);
            Assert.Single(_form.ListViewShop[5].Items);
            Assert.Empty(_form.ListViewShop[1].Items);

            var row = _form.ListViewShop[0].Items[0];
            Assert.Equal("装饰", row.Text); // Caption = GetShopType(页签 I)
            Assert.Equal("木剑", row.SubItems[1].Text);
            Assert.Equal("金币", row.SubItems[3].Text); // GetGameMoney(1)
            Assert.Equal("100", row.SubItems[4].Text);
        });
    }

    [Fact]
    public void AddShopItem_SixGateMessages_ThenSuccess()
    {
        StaRunner.New(() =>
        {
            _form.GetStdItemHandler = name => name == "木剑" ? Item("木剑", price: 0) : null;
            string? lastMsg = null;
            M2Forms.MessageBoxHandler = (text, _, _) => { lastMsg = text; return 1; };

            // 1 无选中页签（ActivePageIndex=-1）
            _form.PageControlShop.SelectedIndex = -1;
            _form.ButtonAddShopItemClick();
            Assert.Equal("请选择物品类别！", lastMsg);

            // 2 物品名无效
            _form.PageControlShop.SelectedIndex = 0;
            _form.EditShopItemName.Text = "不存在的";
            _form.ButtonAddShopItemClick();
            Assert.Equal("请选择一个正确的物品！", lastMsg);

            // 3 货币未选
            _form.EditShopItemName.Text = "木剑";
            _form.ButtonAddShopItemClick();
            Assert.Equal("请选择一个正确的交易货币！", lastMsg);

            // 4 功能为空
            _form.ComboBoxGameMoney.SelectedIndex = 1;
            _form.ButtonAddShopItemClick();
            Assert.Equal("请输入物品功能！", lastMsg);

            // 5 价格 ≤ 0
            _form.EditItemMemo1.Text = "砍人";
            _form.ButtonAddShopItemClick();
            Assert.Equal("请输入正确的物品价格！", lastMsg);

            // 6 重名
            _form.EditShopItemPrice.Value = 100;
            _form.g_SndaShopList.Add(new TShopItem { ShopType = 1, StdItem = Item("木剑") });
            _form.ButtonAddShopItemClick();
            Assert.Equal("该物品已经在列表中了！", lastMsg);

            // 成功：Memo2 取前 min(6, 行数) 行
            _form.g_SndaShopList.Delete(_form.g_SndaShopList.Get("木剑")!);
            lastMsg = null;
            _form.MemoShop.Lines = new[] { "甲", "乙", "丙", "丁", "戊", "己", "庚" }; // 7 行截取 6
            _form.ButtonAddShopItemClick();
            Assert.Null(lastMsg); // 无错误弹窗
            var added = _form.g_SndaShopList.Get("木剑")!;
            Assert.Equal("甲\r\n乙\r\n丙\r\n丁\r\n戊\r\n己\r\n", added.Memo2);
            Assert.True(_form.ButtonShopSaveItem.Enabled);
        });
    }

    [Fact]
    public void Shop_ClickBackfill_Chg_Del()
    {
        StaRunner.New(() =>
        {
            _form.GetStdItemHandler = name => Item(name);
            _form.PageControlShop.SelectedIndex = 0;
            _form.g_SndaShopList.Add(new TShopItem
            {
                ShopType = 0,
                StdItem = Item("木剑", looks: 3, price: 100),
                GameMoney = 2,
                ImageIndex = 380,
                ImageCount = 1,
                Memo1 = "砍",
                Memo2 = "说明\r\n第二行",
                ItemCount = 1,
                boBulkBuy = false,
                nBulkBuyCount = 99,
            });
            _form.RefShopList();

            // 模拟点击回填
            _form.ListViewShop[0].Items[0].Selected = true;
            _form.ListViewShopClick(_form.ListViewShop[0]);
            Assert.NotNull(_form.SelShopItem);
            Assert.Equal("木剑", _form.EditShopItemName.Text);
            Assert.Equal(100m, _form.EditShopItemPrice.Value);
            Assert.Equal(2, _form.ComboBoxGameMoney.SelectedIndex);
            Assert.True(_form.ButtonDelShopItem.Enabled);
            Assert.True(_form.ButtonShopChgItem.Enabled);
            Assert.False(_form.ButtonShopItemUP.Enabled);   // 首行
            Assert.False(_form.ButtonShopItemDOWN.Enabled); // 单行

            // 修改：价格/功能/Memo2（ShopType 不改）
            _form.EditShopItemPrice.Value = 250;
            _form.EditItemMemo1.Text = "重砍";
            _form.MemoShop.Lines = new[] { "新说明" };
            _form.ButtonShopChgItemClick();
            Assert.Equal(250, _form.SelShopItem.StdItem.Price);
            Assert.Equal("重砍", _form.SelShopItem.Memo1);
            Assert.Equal("新说明\r\n", _form.SelShopItem.Memo2);
            Assert.True(_form.ButtonShopSaveItem.Enabled);

            // 删除
            _form.ButtonDelShopItemClick();
            Assert.Null(_form.SelShopItem);
            Assert.Empty(_form.ListViewShop[0].Items);
            Assert.False(_form.ButtonShopChgItem.Enabled);
        });
    }

    // ---- 窗体：消息过滤页 ----

    [Fact]
    public void Filter_AddDupDelChgFlow()
    {
        StaRunner.New(() =>
        {
            string? lastMsg = null;
            M2Forms.MessageBoxHandler = (text, _, _) => { lastMsg = text; return 1; };

            // 空过滤词
            _form.ButtonMsgFilterAddClick();
            Assert.Equal("请输入过滤消息！", lastMsg);

            _form.EditFilterMsg.Text = "  广播  ";
            _form.EditNewMsg.Text = "新广播";
            _form.ButtonMsgFilterAddClick();
            Assert.Single(_form.ListViewMsgFilter.Items);
            Assert.True(_form.ButtonMsgFilterSave.Enabled);
            Assert.Equal("广播", _form.g_FilterTexts.GetItems(0).Msg); // TrimAll 入表

            // 重复
            _form.EditFilterMsg.Text = "广播";
            _form.ButtonMsgFilterAddClick();
            Assert.Equal("此过滤消息已经存在！", lastMsg);

            // 点击回填 + 按钮态
            _form.ListViewMsgFilter.Items[0].Selected = true;
            _form.ListViewMsgFilterClick();
            Assert.Equal("广播", _form.EditFilterMsg.Text);
            Assert.True(_form.ButtonMsgFilterChg.Enabled);
            Assert.True(_form.ButtonMsgFilterDel.Enabled);

            // 修改替换词
            _form.EditNewMsg.Text = "新词";
            _form.ButtonMsgFilterChgClick();
            Assert.Equal("新词", _form.g_FilterTexts.GetItems(0).Replace);

            // 删除
            _form.ButtonMsgFilterDelClick();
            Assert.Empty(_form.ListViewMsgFilter.Items);
            Assert.False(_form.ButtonMsgFilterChg.Enabled);
            Assert.False(_form.ButtonMsgFilterDel.Enabled);
        });
    }

    // ---- TFilterTexts 数据层 ----

    [Fact]
    public void FilterTexts_TrimAll_StripsControlChars()
    {
        // Delphi TextChars=[#32..#255] 基于 ANSI 字节流：GBK 字节 128..255（汉字）保留，仅删控制符
        Assert.Equal("abc", TFilterTexts.TrimAll("  a\u0001b\tc  "));
        Assert.Equal("中文", TFilterTexts.TrimAll("中文"));       // GBK 字节 ≥128 保留
        Assert.Equal("a b", TFilterTexts.TrimAll("a\u0002 b"));  // 空格与可见字符保留
    }

    [Fact]
    public void FilterTexts_Filter_ReplacementCore()
    {
        var ft = new TFilterTexts();
        ft.Add("外挂", "");          // 空 Replace → 清空
        ft.Add("管理员", "客服");
        ft.Add("黄金", "白金");
        ft.Add("GM", "管理员");      // 英文词测 SameText 大小写不敏感

        // 全等（SameText 大小写不敏感）→ 整句替换
        Assert.True(ft.Filter("管理员", out var s));
        Assert.Equal("客服", s);
        Assert.True(ft.Filter("gm", out s)); // SameText 不区分大小写
        Assert.Equal("管理员", s);

        // 包含 → 局部大小写不敏感替换
        Assert.True(ft.Filter("找黄金装备", out s));
        Assert.Equal("找白金装备", s);

        // 空 Replace → 清空且立即停止
        Assert.True(ft.Filter("开外挂了", out s));
        Assert.Equal("", s);

        // '$' 特例：包含 $ → 清除
        Assert.True(ft.Filter("低价$金币", out s));
        Assert.Equal("低价金币", s);
        Assert.True(ft.Filter("$", out s));
        Assert.Equal("", s);

        // 未命中 → false 且原文（Trim 后）返回
        Assert.False(ft.Filter("正常喊话", out s));
        Assert.Equal("正常喊话", s);
    }

    [Fact]
    public void FilterTexts_LoadSave_RoundTrip()
    {
        var path = Path.Combine(_tempDir, "FilterMsgList.txt");
        File.WriteAllLines(path, new[]
        {
            "; 注释行",
            "脏话\t***",
            "广告\t正规信息",
        }, GXX.Core.EncodingInit.GBK);

        var ft = new TFilterTexts();
        ft.LoadFromFile();
        Assert.Equal(2, ft.Count);
        Assert.Equal("正规信息", ft.GetItems(1).Replace);

        ft.Add("诈骗", "");
        ft.SaveToFile();
        var lines = File.ReadAllLines(path, GXX.Core.EncodingInit.GBK);
        Assert.Equal(new[] { "脏话\t***", "广告\t正规信息", "诈骗\t" }, lines);
    }
}
