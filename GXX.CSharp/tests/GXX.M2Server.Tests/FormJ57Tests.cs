using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Forms;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J57：ViewList2.pas 第二片（物品规则/用户命令/宝箱/套装组/WIL 名单/技能威力/过滤备注）
/// 与 Boxs.pas / UserCmds.pas / ItemEffects.pas / GroupItems.pas 数据层 1:1 测试。
/// </summary>
public sealed class FormJ57Tests : IDisposable
{
    private readonly string _tempDir;
    private readonly ViewList2Form _form;

    public FormJ57Tests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "j57_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
        M2ShareState.ResetForTests(_tempDir);
        M2Config.sEnvirDir = _tempDir + Path.DirectorySeparatorChar;
        // sBoxsDir/sBoxsFile 是跨测试类共享的静态配置（FormGeneralConfigTests 会改写），此处显式钉住
        M2Config.sBoxsDir = M2Config.sEnvirDir + "Boxs" + Path.DirectorySeparatorChar;
        M2Config.sBoxsFile = M2Config.sBoxsDir + "BoxsList.txt";
        M2Config.ResetViewList2ConfigDefaults();
        M2ShareGlobals.ResetNames();
        SndaShopEnv.Reset();
        ViewList2State.ResetForTests();
        M2Forms.MessageBoxHandler = (_, _, _) => M2Forms.IDOK;

        _form = StaRunner.New(() =>
        {
            var f = new ViewList2Form();
            f.GetStdItemIdxHandler = name => name switch
            {
                "木剑" => 0,
                "屠龙" => 1,
                "金创药" => 2,
                _ => -1,
            };
            f.WriteBoolHandler = (_, _, _) => { };
            f.SendServerConfigHandler = () => { };
            return f;
        });
    }

    public void Dispose()
    {
        StaRunner.New(() => _form.Dispose());
        M2Forms.MessageBoxHandler = null;
        ViewList2State.ResetForTests();
        SndaShopEnv.Reset();
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_tempDir, true); } catch { }
    }

    // ================= UserCmds.pas =================

    [Fact]
    public void UserCmds_AddFindDelete_SaveLoadRoundTrip()
    {
        var cmds = new TUserCmds();
        Assert.True(cmds.Add("回城", 1));
        Assert.True(cmds.Add("PK", 2));
        Assert.Equal(2, cmds.Count);
        Assert.Equal(2, cmds.RecordCount);

        Assert.True(cmds.Find("回城"));
        Assert.True(cmds.Find("pk"));                 // CompareText
        Assert.Equal(2, cmds.Get("pk"));
        Assert.False(cmds.Find("不存在"));
        Assert.True(cmds.Find(1));
        Assert.False(cmds.Find(9));

        cmds.SaveToFile();
        var loaded = new TUserCmds();
        loaded.LoadFromFile();
        Assert.Equal(2, loaded.Count);
        Assert.Equal("回城", loaded.GetStrings(0));
        Assert.Equal(1, loaded.GetObjects(0));

        Assert.True(loaded.Delete("回城"));
        Assert.False(loaded.Delete("回城"));
        Assert.True(loaded.Delete(2));
        Assert.Equal(0, loaded.Count);
    }

    [Fact]
    public void UserCmds_GotoLable_RequiresFunctionNpc()
    {
        var cmds = new TUserCmds();
        cmds.Add("回城", 3);
        var visited = new List<string>();
        cmds.GotoLableHandler = (_, label) => visited.Add(label);

        Assert.False(cmds.GotoLable(new object(), "回城"));   // g_FunctionNPC = nil
        Assert.Empty(visited);

        cmds.FunctionNPCExists = () => true;
        Assert.True(cmds.GotoLable(new object(), "回城"));
        Assert.Equal(new[] { "@UserCmd3" }, visited);

        cmds.GotoLable(new object(), 7);
        Assert.Equal(new[] { "@UserCmd3", "@UserCmd7" }, visited);
    }

    // ================= ItemEffects.pas =================

    [Fact]
    public void ItemEffects_LoadFromFile_ParsesFiveLibrariesAndDefaults()
    {
        // 字段顺序 = LoadFromFile 的 GetValidStr3 读取顺序（EffectList.txt 落盘列顺序）：
        // 0 Index / 1-3 FileIndex1-3 / 4-6 StartIndex1-3 / 7-9 ImageCount1-3 / 10-12 Time1-3 /
        // 13-14 OffSetX1,Y1 / 15-16 OffSetX2,Y2 / 17-18 OffSetX3,Y3 / 19 NoBlendMode2 / 20 NoSex2 /
        // 21 DrawCenter1 / 22 DrawCenter3 / 23 FileIndex4 / 24 AddEffectDrawOrder / 25 StartIndex4 /
        // 26 ImageCount4 / 27 Time4 / 28 AddEffectNoBlendMode / 29 AddEffectDrawCenter /
        // 30 NoBlendMode1 / 31 NoBlendMode3 / 32 boEfectBelowItem1 / 33 FileIndex5 / 34 StartIndex5 /
        // 35 ImageCount5 / 36 Time5 / 37 OffSetX5 / 38 OffSetY5 / 39 DrawCenter5 / 40 NoBlendMode5 /
        // 41 boEfectBelowItem5 / 42 EffectDesc
        File.WriteAllText(Path.Combine(_tempDir, "EffectList.txt"),
            "7\t100\t200\t300\t5\t6\t7\t3\t4\t5\t10\t11\t12\t1\t2\t3\t4\t5\t6\t1\t0\t1\t1\t9\t2\t8\t7\t6\t1\t1\t0\t0\t1\t400\t10\t20\t30\t50\t60\t1\t0\t1\t地面火" + Environment.NewLine
            + "0\t1\t1\t1\t0\t0\t0\t0\t0\t0\t1\t1\t1\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t0\t1\t1\t0\t0\t0\t0\t0\t0\t0\t0\t应被 nIndex>0 过滤" + Environment.NewLine,
            GXX.Core.EncodingInit.GBK);

        var effects = new TItemEffects();
        effects.LoadFromFile();
        Assert.Equal(1, effects.Count);
        var e = effects.Get(7)!;
        Assert.Equal(100, e.FileIndex1);
        Assert.Equal(200, e.FileIndex2);
        Assert.Equal(300, e.FileIndex3);
        Assert.Equal(9, e.FileIndex4);
        Assert.Equal(400, e.FileIndex5);
        Assert.Equal(5, e.StartIndex1);
        Assert.Equal(10, e.Time1);
        Assert.Equal(20, e.ImageCount5);
        Assert.Equal(30, e.Time5);
        Assert.Equal(50, e.OffSetX5);
        Assert.Equal(60, e.OffSetY5);
        Assert.True(e.NoBlendMode2);
        Assert.False(e.NoSex2);
        Assert.True(e.DrawCenter1);
        Assert.True(e.DrawCenter5);
        Assert.True(e.AddEffectDrawCenter);
        Assert.True(e.boEfectBelowItem5);
        Assert.Equal("地面火", e.EffectDesc);
    }

    [Fact]
    public void ItemEffects_AddRejectsDuplicateIndex_SaveRoundTrip()
    {
        var effects = new TItemEffects();
        Assert.True(effects.Add(new TItemEffect { Index = 3, FileIndex1 = 11 }));
        Assert.False(effects.Add(new TItemEffect { Index = 3, FileIndex1 = 22 }));
        Assert.Equal(1, effects.RecordCount);
        Assert.NotNull(effects.Get(3));
        Assert.True(effects.Find(3));
        Assert.False(effects.Find(4));

        effects.SaveToFile();
        var loaded = new TItemEffects();
        loaded.LoadFromFile();
        Assert.Single(loaded.All());
        Assert.Equal(11, loaded.Get(3)!.FileIndex1);

        Assert.True(loaded.DeleteIndex(0));
        Assert.Equal(0, loaded.RecordCount);
        Assert.False(loaded.DeleteIndex(0));
    }

    // ================= Boxs.pas =================

    [Fact]
    public void BoxsList_LoadFromFile_ParsesBoxSetAndFourLists()
    {
        Directory.CreateDirectory(M2Config.sBoxsDir);
        File.WriteAllText(M2Config.sBoxsFile, "0" + Environment.NewLine, GXX.Core.EncodingInit.GBK);
        File.WriteAllText(M2Config.sBoxsDir + "0.txt",
            "1\t100\t200\t10\t20\t30\t40\t5" + Environment.NewLine
            + ";注释行" + Environment.NewLine
            + "金创药\t0\t3" + Environment.NewLine
            + "屠龙\t1\t1" + Environment.NewLine
            + "木剑\t2\t1" + Environment.NewLine
            + "经验(500)\t3\t500" + Environment.NewLine,
            GXX.Core.EncodingInit.GBK);

        var boxs = new TBoxsList { GetStdItemHandler = name => name is "金创药" or "屠龙" or "木剑" ? new TBoxStdItemView { Name = name } : null };
        boxs.LoadFromFile();

        Assert.Equal(1, boxs.Count);
        var box = boxs.GetBox(0);
        Assert.True(box.BoxSet.boNext);
        Assert.Equal(100, box.BoxSet.nGold);
        Assert.Equal(200, box.BoxSet.nGameGold);
        Assert.Equal(10, box.BoxSet.nAddGold);
        Assert.Equal(20, box.BoxSet.nAddGameGold);
        Assert.Equal(30, box.BoxSet.nEndGold);
        Assert.Equal(40, box.BoxSet.nEndGameGold);
        Assert.Equal(5, box.BoxSet.nCount);

        Assert.Equal(1, box.Give.Count);
        Assert.Equal("金创药", box.Give.GetItems(0).ItemName);
        Assert.Equal(3, box.Give.GetItems(0).ItemCount);
        Assert.Equal("屠龙", box.NoGive.GetItems(0).ItemName);
        Assert.Equal("木剑", box.Center.GetItems(0).ItemName);
        Assert.Equal("经验", box.EndNoGive.GetItems(0).ItemName);   // 经验(500) → 名 经验 数量 500
        Assert.Equal(500, box.EndNoGive.GetItems(0).ItemCount);
    }

    [Fact]
    public void BoxsList_SaveToFile_RoundTripsAndMakesNewNameForSpecials()
    {
        var boxs = new TBoxsList();
        var box = new TBox { Name = "赤金宝箱", Source = 4 };
        box.BoxSet.boNext = false;
        box.BoxSet.nGold = 7;
        box.BoxSet.nGameGold = 8;
        box.BoxSet.nAddGold = 9;
        box.BoxSet.nAddGameGold = 10;
        box.BoxSet.nEndGold = 11;
        box.BoxSet.nEndGameGold = 12;
        box.BoxSet.nCount = 3;
        box.Give.Add(new TBoxItem { ItemName = "金创药", ItemCount = 2 });
        box.NoGive.Add(new TBoxItem { ItemName = "声望", ItemCount = 50 });
        box.Center.Add(new TBoxItem { ItemName = "金刚石", ItemCount = 5 });
        box.EndNoGive.Add(new TBoxItem { ItemName = "经验", ItemCount = 100 });
        boxs.AddBox(box);
        boxs.SaveToFile();

        string text = File.ReadAllText(M2Config.sBoxsDir + "4.txt", GXX.Core.EncodingInit.GBK);
        Assert.Contains("声望(50)", text);
        Assert.Contains("金刚石(5)", text);
        Assert.Contains("经验(100)", text);
        Assert.Contains("金创药\t0\t2", text);
        Assert.Contains("声望(50)\t1\t50", text);
        Assert.Contains("金刚石(5)\t2\t5", text);
        Assert.Contains("经验(100)\t3\t100", text);
    }

    [Fact]
    public void BoxsList_GetBoxsItem_SpecialItemsAndOverlap()
    {
        var boxs = new TBoxsList
        {
            RandomHandler = _ => 0,
            GetStdItemHandler = name => name == "屠龙" ? new TBoxStdItemView { Name = "屠龙", StdMode = 5, Looks = 100, Color = 3, Price = 999, DuraMax = 33 } : null,
        };
        var box = new TBox { Source = 0 };
        box.BoxSet.nGold = 11;
        box.Give.Add(new TBoxItem { ItemName = "经验", ItemCount = 1234 });
        box.Give.Add(new TBoxItem { ItemName = "屠龙", ItemCount = 7 });
        box.Give.Add(new TBoxItem { ItemName = "声望", ItemCount = 55 });
        box.Give.Add(new TBoxItem { ItemName = "金刚石", ItemCount = 66 });
        boxs.AddBox(box);

        // nIdx=0 → Give 首项 经验：StdMode 255 / Looks 1186 / Color 255 / Price = 数量
        var clientItem = new TClientItemView();
        var boxSet = boxs.GetBoxsItem(0, 0, 4242, clientItem);
        Assert.Equal(11, boxSet.nGold);
        Assert.Equal(4242, clientItem.MakeIndex);
        Assert.Equal("经验", clientItem.Name);
        Assert.Equal(255, clientItem.StdMode);
        Assert.Equal(1186, clientItem.Looks);
        Assert.Equal(255, clientItem.Color);
        Assert.Equal(1234, clientItem.Price);

        // 声望 / 金刚石 分支（RandomHandler 契约：[0, count)）
        boxs.RandomHandler = _ => 0;
        var box2 = new TBox { Source = 1 };
        box2.NoGive.Add(new TBoxItem { ItemName = "声望", ItemCount = 55 });
        box2.Center.Add(new TBoxItem { ItemName = "金刚石", ItemCount = 66 });
        box2.EndNoGive.Add(new TBoxItem { ItemName = "屠龙", ItemCount = 7 });
        boxs.AddBox(box2);
        boxs.GetBoxsItem(1, 1, 1, clientItem);
        Assert.Equal(M2Config.sCreditPointName, clientItem.Name);
        Assert.Equal(1185, clientItem.Looks);
        boxs.GetBoxsItem(1, 2, 1, clientItem);
        Assert.Equal(M2Config.sGameDiamondName, clientItem.Name);
        Assert.Equal(1187, clientItem.Looks);

        // 普通物品：CheckOverLapItem true → Dura=0/DuraMax=原值；false → Dura=DuraMax
        boxs.GetBoxsItem(1, 3, 1, clientItem);
        Assert.Equal("屠龙", clientItem.Name);
        Assert.Equal(33, clientItem.DuraMax);
        Assert.Equal(33, clientItem.Dura);
        Assert.Equal(7, clientItem.Price);   // 数量统一覆盖价格

        boxs.CheckOverLapItemHandler = _ => true;
        boxs.GetBoxsItem(1, 3, 1, clientItem);
        Assert.Equal(0, clientItem.Dura);
        Assert.Equal(33, clientItem.DuraMax);
    }

    // ================= GroupItems.pas =================

    [Fact]
    public void GroupItems_LoadFromFile_ParsesAllEightSegments()
    {
        File.WriteAllText(Path.Combine(_tempDir, "GroupItemList.txt"),
            "3\t2\t祖玛套装\t祖玛头盔|祖玛项链\t1|1|0\t5|10\t100|200\t提示串" + Environment.NewLine
            + ";注释" + Environment.NewLine,
            GXX.Core.EncodingInit.GBK);

        var groups = new TGroupItems();
        groups.LoadFromFile();
        Assert.Equal(1, groups.Count);
        var g = groups.GetItems(0);
        Assert.Equal(3, g.FLD_INDEX);
        Assert.Equal(2, g.FLD_COUNT);
        Assert.Equal("祖玛套装", g.FLD_DESC);
        Assert.Equal(new[] { "祖玛头盔", "祖玛项链" }, g.FLD_ITEMNAMES);
        Assert.True(g.FLD_FLAG[0]);
        Assert.True(g.FLD_FLAG[1]);
        Assert.False(g.FLD_FLAG[2]);
        Assert.Equal(5, g.FLD_RATE[0]);
        Assert.Equal(10, g.FLD_RATE[1]);
        Assert.Equal(100, g.FLD_VALUE[0]);
        Assert.Equal(200, g.FLD_VALUE[1]);
        Assert.Equal("提示串", g.FLD_HINTMSG);
    }

    [Fact]
    public void GroupItems_SaveToFile_RoundTripKeepsSkillPercents()
    {
        var groups = new TGroupItems();
        var g = new TGroupItemModel { FLD_INDEX = 9, FLD_COUNT = 1, FLD_DESC = "测试套", FLD_HINTMSG = "提示" };
        g.FLD_ITEMNAMES.Add("木剑");
        g.FLD_FLAG[27] = true;
        g.FLD_RATE[3] = 15;
        g.FLD_VALUE[4] = 25;
        g.AttackSkillPercent[1] = 30;
        g.DefenseSkillPercent[1] = 40;
        g.AttackSkillPercent[2] = 99;    // 无效技能 → 不落盘
        groups.Add(g);
        groups.SaveToFile();

        var loaded = new TGroupItems();
        loaded.LoadFromFile();
        Assert.Equal(1, loaded.Count);
        var r = loaded.GetItems(0);
        Assert.Equal(9, r.FLD_INDEX);
        Assert.Equal("测试套", r.FLD_DESC);
        Assert.Equal("木剑", r.FLD_ITEMNAMES[0]);
        Assert.True(r.FLD_FLAG[27]);
        Assert.Equal(15, r.FLD_RATE[3]);
        Assert.Equal(25, r.FLD_VALUE[4]);
        Assert.Equal(30, r.AttackSkillPercent[1]);
        Assert.Equal(40, r.DefenseSkillPercent[1]);
        Assert.Equal(0, r.AttackSkillPercent[2]);   // IsValidMagicInSkillPowerItem(2)=false 不写
    }

    [Fact]
    public void GroupItems_IsValidMagicAndRateValues()
    {
        Assert.False(TGroupItems.IsValidMagicInSkillPowerItem(2));
        Assert.False(TGroupItems.IsValidMagicInSkillPowerItem(20));
        Assert.False(TGroupItems.IsValidMagicInSkillPowerItem(75));
        Assert.True(TGroupItems.IsValidMagicInSkillPowerItem(1));
        Assert.True(TGroupItems.IsValidMagicInSkillPowerItem(100));

        // 窗体 115 行版本额外含 3
        Assert.False(TGroupItems.IsValidMagicInSkillPowerItemForm(3));
        Assert.True(TGroupItems.IsValidMagicInSkillPowerItem(3));

        Assert.Equal(110, TGroupItems.RateValue(10, 100));       // 100 + round(10) = 110
        Assert.Equal(100, TGroupItems.RateValue(0, 100));        // 比率 0 → 原值
        Assert.Equal(int.MaxValue, TGroupItems.RateValue(100, int.MaxValue));  // Int64 后钳制
        Assert.Equal(0, TGroupItems.RateValue(1, -100));         // 负值钳到 0
        Assert.Equal(110L, TGroupItems.RateValue2(10, 100));
        Assert.Equal(100L, TGroupItems.RateValue2(0, 100));
    }

    [Fact]
    public void GroupItems_Get_MatchesAcrossContainersAndExcludesUsedSlots()
    {
        var groups = new TGroupItems
        {
            GetStdItemHandler = idx => idx switch
            {
                1 => new TStdItemView { Name = "木剑" },
                2 => new TStdItemView { Name = "布衣" },
                3 => new TStdItemView { Name = "金创药" },
                4 => new TStdItemView { Name = "封号令", AniCount = 1 },
                _ => null,
            },
            GetTZSupportRenameItem = () => false,
            GetJewelryCalcGroupAbilitys = () => true,
        };

        var g = new TGroupItemModel { FLD_INDEX = 1, FLD_COUNT = 2, FLD_DESC = "两件套" };
        g.FLD_ITEMNAMES.Add("木剑");
        g.FLD_ITEMNAMES.Add("布衣");
        groups.Add(g);

        var g2 = new TGroupItemModel { FLD_INDEX = 2, FLD_COUNT = 1, FLD_DESC = "单件" };
        g2.FLD_ITEMNAMES.Add("木剑");
        groups.Add(g2);

        var useItems = new TUserItem[30];
        useItems[0].wIndex = 1;   // 木剑
        var jewelry = new TUserItem[6];
        jewelry[0].wIndex = 2;    // 布衣（首饰盒）
        var godBless = new TUserItem[12];
        var fengHao = new TUserItem[60];

        var hits = new List<TGroupItemModel>();
        Assert.Equal(2, groups.Get(useItems, jewelry, godBless, fengHao, 0, hits));
        Assert.Equal(new[] { 1, 2 }, hits.Select(x => x.FLD_INDEX));

        // 仅剩首饰盒里的 布衣：两件套（木剑+布衣）再凑不齐（木剑已卸下），
        // 但单件套 g2（木剑）也不在首饰盒 → 都取不到
        useItems[0].wIndex = 0;
        hits.Clear();
        Assert.Equal(0, groups.Get(useItems, jewelry, godBless, fengHao, 0, hits));
        Assert.Empty(hits);

        // 神佑盒路径：布衣放进神佑盒也能命中
        jewelry[0].wIndex = 0;
        var godBless2 = new TUserItem[12];
        godBless2[0].wIndex = 2;
        var g3 = new TGroupItemModel { FLD_INDEX = 3, FLD_COUNT = 1 };
        g3.FLD_ITEMNAMES.Add("布衣");
        groups.Add(g3);
        hits.Clear();
        // g3（布衣）命中；g2（木剑）在神佑盒无对应物品 → 仅 1 项
        Assert.Equal(1, groups.Get(new TUserItem[30], new TUserItem[6], godBless2, fengHao, 0, hits));
        Assert.Equal(3, hits[0].FLD_INDEX);
    }

    [Fact]
    public void GroupItems_Get_FengHaoRequiresAniCountOrActive()
    {
        var groups = new TGroupItems
        {
            GetStdItemHandler = idx => idx switch
            {
                4 => new TStdItemView { Name = "封号令", AniCount = 1 },
                5 => new TStdItemView { Name = "普通封号", AniCount = 0 },
                _ => null,
            },
            GetTZSupportRenameItem = () => false,
            GetJewelryCalcGroupAbilitys = () => false,
        };
        var g = new TGroupItemModel { FLD_INDEX = 5, FLD_COUNT = 1 };
        g.FLD_ITEMNAMES.Add("封号令");
        groups.Add(g);

        var fengHao = new TUserItem[60];
        fengHao[0].wIndex = 4;
        var hits = new List<TGroupItemModel>();
        Assert.Equal(1, groups.Get(new TUserItem[30], new TUserItem[6], new TUserItem[12], fengHao, 0, hits));

        // AniCount=0 且非激活槽 → 跳过
        var g2 = new TGroupItemModel { FLD_INDEX = 6, FLD_COUNT = 1 };
        g2.FLD_ITEMNAMES.Add("普通封号");
        groups.Add(g2);
        var fengHao2 = new TUserItem[60];
        fengHao2[3].wIndex = 5;
        hits.Clear();
        Assert.Equal(0, groups.Get(new TUserItem[30], new TUserItem[6], new TUserItem[12], fengHao2, 0, hits));
        // ActiveFengHao = 3 → 命中
        Assert.Equal(1, groups.Get(new TUserItem[30], new TUserItem[6], new TUserItem[12], fengHao2, 3, hits));
    }

    // ================= 窗体层：物品规则 =================

    [Fact]
    public void ItemRule_AddWithFlagsAndPrices_SavesTabSeparatedRow()
    {
        _form.ItemRuleFormCreate();   // 拍卖货币 5 项（Delphi FormCreate）
        _form.ListBoxItemList2.Items.Add("木剑");
        _form.ListBoxItemList2.SetSelected(0, true);
        _form.ItemRuleChecks[0]!.Checked = true;
        _form.ItemRuleChecks[27]!.Checked = true;   // 允许拍卖
        _form.cbbAuctionPricesType.SelectedIndex = 0;
        _form.seAuctionPrices_Min.Value = 10;
        _form.seAuctionPrices_Max.Value = 99;
        _form.SeAuctionPricesMinChange();   // 无句柄时 ValueChanged 不触发
        _form.SeAuctionPricesMaxChange();

        _form.ButtonItemRuleAddClick();

        Assert.Equal(1, ViewList2State.g_ItemRules.Count);
        var rule = ViewList2State.g_ItemRules.GetItems(0);
        Assert.Equal("木剑", rule.ItemName);
        Assert.True(rule.FlagArray[0]);
        Assert.True(rule.FlagArray[27]);
        Assert.Equal(10u, rule.PricesMin[0]);
        Assert.Equal(99u, rule.PricesMax[0]);
        Assert.True(_form.ButtonItemRuleSave.Enabled);

        ViewList2State.g_ItemRules.SaveToEnvirFile();
        string text = File.ReadAllText(Path.Combine(_tempDir, "ItemRuleList.txt"), GXX.Core.EncodingInit.GBK);
        Assert.StartsWith("木剑\t", text);
        Assert.Contains("|", text);
        Assert.Contains("10 99", text);
    }

    [Fact]
    public void ItemRule_AddRejectsUnknownItem_AndInvertedPrices()
    {
        // 不可解析为物品索引 → Add 早退（Delphi GetStdItemIdx(sItemName) < 0 → Exit）
        _form.GetStdItemIdxHandler = name => name == "木剑" ? 0 : -1;
        _form.ListBoxItemList2.Items.Add("不存在物品");
        _form.ListBoxItemList2.SetSelected(0, true);
        _form.ButtonItemRuleAddClick();
        Assert.Equal(0, ViewList2State.g_ItemRules.Count);       // Delphi GetStdItemIdx < 0 → Add 返回 nil，不入表
        Assert.Equal("增加失败！", M2Forms.LastMessage);
        Assert.NotNull(_form.GetStdItemIdxHandler);              // 接缝已注入（未被清空）

        _form.ItemRuleFormCreate();
        _form.ListBoxItemList2.Items.Add("木剑");
        _form.ListBoxItemList2.SetSelected(1, true);
        _form.seAuctionPrices_Min.Value = 50;
        _form.seAuctionPrices_Max.Value = 5;
        _form.SeAuctionPricesMinChange();
        _form.SeAuctionPricesMaxChange();
        _form.ButtonItemRuleAddClick();
        Assert.Equal(0, ViewList2State.g_ItemRules.Count);
        Assert.Equal("拍卖最低价不能大于最高价！", M2Forms.LastMessage);
        Assert.Equal(50m, _form.seAuctionPrices_Min.Value);   // 回填
        Assert.Equal(5m, _form.seAuctionPrices_Max.Value);
    }

    [Fact]
    public void ItemRule_SelectAllSkipsDisabled_AndBatchSetting()
    {
        _form.ItemRuleChecks[0]!.Checked = false;
        _form.ItemRuleChecks[0]!.Enabled = false;
        _form.ItemRuleChecks[1]!.Checked = false;
        _form.ButtonItemRuleSelAllClick();
        Assert.False(_form.ItemRuleChecks[0]!.Checked);   // Enabled=False 跳过
        Assert.True(_form.ItemRuleChecks[1]!.Checked);

        _form.ButtonItemRuleNotSelAllClick();
        Assert.False(_form.ItemRuleChecks[1]!.Checked);

        // 批量设置：先备一条规则
        _form.ListBoxItemList2.Items.Add("木剑");
        _form.ListBoxItemList2.SetSelected(0, true);
        _form.ButtonItemRuleAddClick();
        Assert.Equal(1, _form.ListBoxItemRuleList.Items.Count);
        _form.SelectItemRuleRow(0);

        _form.OnItemRuleBathSettingClick(0 * 1000 + 1);   // 索引 0 置真
        Assert.True(ViewList2State.g_ItemRules.GetItems(0).FlagArray[0],
            $"选中数={_form.ListBoxItemRuleList.SelectedItems.Count} 消息={M2Forms.LastMessage}");
        _form.SelectItemRuleRow(0);                      // RefItemRuleList 后重选（VCL 保持选中的等效）
        _form.OnItemRuleBathSettingClick(0 * 1000 + 0);   // 索引 0 置假
        Assert.False(ViewList2State.g_ItemRules.GetItems(0).FlagArray[0],
            $"选中数={_form.ListBoxItemRuleList.SelectedItems.Count} 消息={M2Forms.LastMessage}");

        _form.OnItemRuleBathSettingClick(99 * 1000);      // 越界索引早退
        Assert.Equal(1, ViewList2State.g_ItemRules.Count);
    }

    [Fact]
    public void ItemRule_AddAllDelAllAndClickBackfill()
    {
        _form.ListBoxItemList2.Items.AddRange(new object[] { "木剑", "屠龙", "金创药" });
        _form.ItemRuleChecks[1]!.Checked = true;
        _form.ButtonItemRuleAddAllClick();
        Assert.Equal(3, ViewList2State.g_ItemRules.Count);
        Assert.True(ViewList2State.g_ItemRules.GetItems(0).FlagArray[1]);
        Assert.Equal(3, _form.ListBoxItemRuleList.Items.Count);

        // 点击回填
        _form.ItemRuleChecks[0]!.Checked = false;
        _form.ItemRuleChecks[27]!.Checked = false;
        ViewList2State.g_ItemRules.GetItems(0).FlagArray[0] = true;
        ViewList2State.g_ItemRules.GetItems(0).FlagArray[1] = false;   // 取消标志1，验证回填逐位对齐
        ViewList2State.g_ItemRules.GetItems(0).PricesMin[2] = 7;
        ViewList2State.g_ItemRules.GetItems(0).PricesMax[2] = 8;
        _form.SelectItemRuleRow(0);
        Assert.True(_form.ItemRuleChecks[0]!.Checked);
        Assert.False(_form.ItemRuleChecks[1]!.Checked);
        Assert.Equal("木剑", _form.EditRuleItemName.Text);
        Assert.Equal(7u, _form.FAcutionPricesLimeMin[2]);
        Assert.Equal(8u, _form.FAcutionPricesLimeMax[2]);
        Assert.True(_form.ButtonItemRuleChg.Enabled);

        // 修改
        _form.ItemRuleChecks[3]!.Checked = true;
        _form.ButtonItemRuleChgClick();
        Assert.True(ViewList2State.g_ItemRules.GetItems(0).FlagArray[3]);

        // 全部删除
        _form.ButtonItemRuleDelAllClick();
        Assert.Equal(0, ViewList2State.g_ItemRules.Count);
        Assert.Equal(0, _form.ListBoxItemRuleList.Items.Count);

        // 未选中时修改 → 弹窗
        _form.ButtonItemRuleChgClick();
        Assert.Equal("请选择一个需要修改的物品！", M2Forms.LastMessage);
    }

    // ================= 窗体层：用户命令 =================

    [Fact]
    public void UserCommand_AddListClickDeleteAndSave()
    {
        _form.EditCommandName.Text = "回城";
        _form.EditCommandIdx.Value = 1;
        _form.ButtonUserCommandAddClick();
        Assert.Equal(1, _form.ListBoxUserCommand.Items.Count);
        Assert.True(_form.ButtonUserCommandSave.Enabled);

        // 重名拒绝
        _form.EditCommandIdx.Value = 2;
        _form.ButtonUserCommandAddClick();
        Assert.Equal("此命令名称已经存在！", M2Forms.LastMessage);
        // 重号拒绝
        _form.EditCommandName.Text = "其他";
        _form.EditCommandIdx.Value = 1;
        _form.ButtonUserCommandAddClick();
        Assert.Equal("此命令编号已经存在！", M2Forms.LastMessage);

        // 点击回填 + 提示文本
        _form.ListBoxUserCommand.SelectedIndex = 0;
        Assert.Equal("回城", _form.EditCommandName.Text);
        Assert.Equal(1m, _form.EditCommandIdx.Value);
        Assert.True(_form.ButtonUserCommandDel.Enabled);
        Assert.Contains("[@UserCmd1]", _form.LabelMsg.Text);

        // 删除
        _form.ButtonUserCommandDelClick();
        Assert.Equal(0, _form.ListBoxUserCommand.Items.Count);
        Assert.False(_form.ButtonUserCommandDel.Enabled);

        // 落盘
        _form.EditCommandName.Text = "PK";
        _form.EditCommandIdx.Value = 9;
        _form.ButtonUserCommandAddClick();
        ViewList2State.g_UserCmds.SaveToFile();
        string text = File.ReadAllText(Path.Combine(_tempDir, "UserCmd.txt"), GXX.Core.EncodingInit.GBK);
        Assert.Contains("PK\t9", text);
    }

    // ================= 窗体层：宝箱 =================

    private void SeedBoxForm()
    {
        var boxs = ViewList2State.g_BoxsList;
        var box = new TBox { Name = "赤金宝箱", Source = 0 };
        box.BoxSet.nGold = 5;
        box.BoxSet.nCount = 2;
        box.Give.Add(new TBoxItem { ItemName = "金创药", ItemCount = 1 });
        boxs.AddBox(box);
        _form.ListBoxBoxItem.Items.Add("赤金宝箱");
        _form.ListBoxitemList1.Items.Add("木剑");
    }

    [Fact]
    public void Box_SelectBoxBackfillAddDelAndConfigChange()
    {
        SeedBoxForm();
        _form.ListBoxBoxItem.SelectedIndex = 0;
        Assert.Equal("赤金宝箱", _form.GroupBoxBoxItem.Text);
        Assert.True(_form.btnAddBoxItem.Enabled);
        Assert.Equal(5m, _form.seGold.Value);
        Assert.Equal(2m, _form.seCount.Value);
        Assert.Equal(1, _form.ListBoxGiveItem.Items.Count);

        // 添加（选定物品列表 + 类型 0）
        _form.ListBoxitemList1.SelectedIndex = 0;
        _form.ComboBoxBoxItemType.SelectedIndex = 0;
        _form.seBoxItemCount.Value = 3;
        _form.BtnAddBoxItemClick();
        Assert.Equal(2, _form.ListBoxGiveItem.Items.Count);
        Assert.True(_form.btnSaveBoxItem.Enabled);

        // 重复添加拒绝
        _form.BtnAddBoxItemClick();
        Assert.Equal("该物品已经在列表中了！", M2Forms.LastMessage);

        // 类型未选择
        _form.ComboBoxBoxItemType.SelectedIndex = -1;
        _form.BtnAddBoxItemClick();
        Assert.Equal("请选择物品种类！", M2Forms.LastMessage);

        // 配置变更写回
        _form.ComboBoxBoxItemType.SelectedIndex = 0;
        _form.seGold.Value = 88;
        _form.BoxSetFieldChanged("Gold");
        Assert.Equal(88, ViewList2State.g_BoxsList.GetBox(0).BoxSet.nGold);

        _form.chkNext.Checked = true;   // 触发 ChkNextClick → seCount 使能
        Assert.True(_form.seCount.Enabled);

        // 删除
        _form.ListBoxGiveItem.SelectedIndex = 1;
        _form.btnDelBoxItem.Enabled = true;
        _form.BtnDelBoxItemClick();
        Assert.Equal(1, _form.ListBoxGiveItem.Items.Count);
    }

    [Fact]
    public void Box_SpecialItemNameFromOtherCombo_AndSave()
    {
        ViewList2State.g_BoxsList.Reset();
        _form.OpenResidualSections(Array.Empty<(string, byte, ushort)>());   // 填 cbbOtherItem（Open 1:1）
        SeedBoxForm();                                                       // Open 会清空宝箱列表，之后垫入
        _form.ListBoxBoxItem.SelectedIndex = 0;
        _form.cbbOtherItem.SelectedIndex = 0;   // 经验
        _form.CbbOtherItemChange();              // 无句柄时 SelectedIndexChanged 不触发，显式调用
        Assert.Equal("经验", _form.edtBoxItemName.Text);
        _form.ComboBoxBoxItemType.SelectedIndex = 3;   // 永不可得
        _form.seBoxItemCount.Value = 500;
        _form.BtnAddBoxItemClick();
        Assert.Equal(1, _form.ListBoxEndNoGiveItem.Items.Count);
        _form.ListBoxEndNoGiveItem.SelectedIndex = 0;
        _form.BoxItemListClick(3);                     // 回填并把数量写回（Delphi 选中项 → BoxItem.ItemCount）

        _form.BtnSaveBoxItemClick();
        Assert.False(_form.btnSaveBoxItem.Enabled);
        string text = File.ReadAllText(M2Config.sBoxsDir + "0.txt", GXX.Core.EncodingInit.GBK);
        Assert.Contains("经验(500)\t3\t500", text);
    }

    // ================= 窗体层：套装组 =================

    [Fact]
    public void GroupItem_AddBackfillModifyDelete()
    {
        _form.EditGroupItemIndex.Value = 5;
        _form.EditGroupItemDesc.Text = "测试套装";
        _form.EditGroupItemName.Text = "木剑|屠龙";
        _form.EditGroupItemCount.Value = 2;
        _form.EditGroupItemHint.Text = "  提示  ";
        _form.GroupItemChecks[1]!.Checked = true;
        _form.GroupItemRates[3]!.Value = 15;
        _form.GroupItemValues[4]!.Value = 25;
        _form.ButtonGroupItemAddClick();

        Assert.Equal(1, ViewList2State.g_GroupItems.Count);
        var g = ViewList2State.g_GroupItems.GetItems(0);
        Assert.Equal(5, g.FLD_INDEX);
        Assert.Equal(2, g.FLD_COUNT);
        Assert.Equal("测试套装", g.FLD_DESC);
        Assert.Equal("提示", g.FLD_HINTMSG);
        Assert.Equal(new[] { "木剑", "屠龙" }, g.FLD_ITEMNAMES);
        Assert.True(g.FLD_FLAG[1]);
        Assert.Equal(15, g.FLD_RATE[3]);
        Assert.Equal(25, g.FLD_VALUE[4]);
        Assert.Same(g, ViewList2State.SelGroupItem);
        Assert.True(_form.ButtonGroupItemSave.Enabled);
        Assert.Equal(1, _form.ListViewGroupItemList.Items.Count);

        // 重复编号
        _form.ButtonGroupItemAddClick();
        Assert.Equal("套装编号已经存在，请重新输入！", M2Forms.LastMessage);

        _form.EditGroupItemIndex.Value = 6;
        _form.EditGroupItemDesc.Text = "";
        _form.ButtonGroupItemAddClick();
        Assert.Equal("请输入套装说明！", M2Forms.LastMessage);

        _form.EditGroupItemDesc.Text = "测试套装";
        _form.EditGroupItemName.Text = "";
        _form.ButtonGroupItemAddClick();
        Assert.Equal("请输入套装物品！", M2Forms.LastMessage);

        _form.EditGroupItemName.Text = "木剑";
        _form.EditGroupItemCount.Value = 0;
        _form.ButtonGroupItemAddClick();
        Assert.Equal("套装数量输入不正确！", M2Forms.LastMessage);

        // 回填
        _form.SelectGroupItemRow(0);
        Assert.Equal(5m, _form.EditGroupItemIndex.Value);
        Assert.Equal("测试套装", _form.EditGroupItemDesc.Text);
        Assert.Equal("木剑|屠龙|", _form.EditGroupItemName.Text);
        Assert.True(_form.GroupItemChecks[1]!.Checked);
        Assert.Equal(15m, _form.GroupItemRates[3]!.Value);
        Assert.Equal(25m, _form.GroupItemValues[4]!.Value);
        Assert.True(_form.ButtonGroupItemChg.Enabled);

        // 修改
        _form.EditGroupItemDesc.Text = "改名套装";
        _form.EditGroupItemCount.Value = 1;
        _form.ButtonGroupItemChgClick();
        Assert.Equal("改名套装", ViewList2State.g_GroupItems.GetItems(0).FLD_DESC);
        Assert.Equal(1, ViewList2State.g_GroupItems.GetItems(0).FLD_COUNT);
        Assert.Equal("改名套装", _form.ListViewGroupItemList.Items[0].SubItems[0].Text);

        // 技能威力回写
        ViewList2State.SelAttackSkillPercent[1] = 30;
        ViewList2State.SelDefenseSkillPercent[1] = 40;
        _form.ShowGroupItemSkillPowerHandler = () => true;
        _form.ButtonGroupItemSkillPowerClick();
        Assert.Equal(30, ViewList2State.g_GroupItems.GetItems(0).AttackSkillPercent[1]);
        Assert.Equal(40, ViewList2State.g_GroupItems.GetItems(0).DefenseSkillPercent[1]);

        // 保存 + 删除
        _form.ButtonGroupItemSaveClick();
        Assert.False(_form.ButtonGroupItemSave.Enabled);
        _form.ButtonGroupItemDelClick();
        Assert.Equal(0, ViewList2State.g_GroupItems.Count);
        Assert.Null(ViewList2State.SelGroupItem);
    }

    // ================= 窗体层：WIL 名单 =================

    [Fact]
    public void WilList_AddDupeMoveDeleteEditSaveAndSend()
    {
        _form.EditWilName.Text = "A.wil";
        _form.BtnWilAddClick();
        Assert.Equal(1, _form.ListBoxWilNameList.Items.Count);
        Assert.True(_form.btnWilSave.Enabled);

        // CompareText 重名
        _form.EditWilName.Text = "a.WIL";
        _form.BtnWilAddClick();
        Assert.Equal("此WIL文件名称已经在列表中了！", M2Forms.LastMessage);

        // 空名
        _form.EditWilName.Text = "   ";
        _form.BtnWilAddClick();
        Assert.Equal("请输入WIL文件名称！", M2Forms.LastMessage);

        _form.EditWilName.Text = "B.wil";
        _form.BtnWilAddClick();
        _form.EditWilName.Text = "C.wil";
        _form.BtnWilAddClick();
        Assert.Equal(3, _form.ListBoxWilNameList.Items.Count);

        // 上移/下移
        _form.ListBoxWilNameList.SelectedIndex = 1;
        _form.ListBoxWilNameListClick();
        Assert.Equal("B.wil 编号:1", _form.LabelFileIndex.Text);
        _form.BtnWilNameUpClick();
        Assert.Equal(new[] { "B.wil", "A.wil", "C.wil" }, _form.ListBoxWilNameList.Items.Cast<string>());
        Assert.Equal(0, _form.ListBoxWilNameList.SelectedIndex);
        Assert.False(_form.btnWilNameUP.Enabled);
        Assert.True(_form.btnWilNameDown.Enabled);

        _form.BtnWilNameDownClick();
        Assert.Equal(new[] { "A.wil", "B.wil", "C.wil" }, _form.ListBoxWilNameList.Items.Cast<string>());

        // 删除
        _form.ListBoxWilNameList.SelectedIndex = 2;
        _form.BtnWilDelClick();
        Assert.Equal(2, _form.ListBoxWilNameList.Items.Count);

        // 就地编辑
        _form.ListBoxWilNameList.SelectedIndex = 0;
        _form.EditWilName.Text = "D.wil";
        _form.BtnWilEditClick();
        Assert.Equal("D.wil", _form.ListBoxWilNameList.Items[0]);

        // 保存（同步 g_EffectImageList + 5 个下拉 + 落盘接缝）
        int saved = 0;
        _form.SaveEffectImageListHandler = () => saved++;
        _form.BtnWilSaveClick();
        Assert.Equal(1, saved);
        Assert.Equal(new[] { "D.wil", "B.wil" }, ViewList2State.g_EffectImageList);
        Assert.Equal("关闭特效", _form.EffectFileIndexCombos[0].Items[0]);
        Assert.Equal(3, _form.EffectFileIndexCombos[0].Items.Count);
        Assert.False(_form.btnWilSave.Enabled);

        int sent = 0;
        _form.SendEffectImageListHandler = () => sent++;
        _form.ButtonSendEffectImageListClick();
        Assert.Equal(1, sent);
    }

    // ================= 窗体层：技能威力物品 =================

    [Fact]
    public void SkillPowerItem_AddFromItemListDuplicateRejectChangeDeleteSave()
    {
        _form.ListBoxitemList5.Items.Add("木剑");
        _form.ListBoxitemList5.SelectedIndex = 0;
        Assert.True(_form.ButtonAddSkillPowerItem.Enabled);

        _form.SkillPowerGrid[1, 1] = "30";
        _form.SkillPowerGrid[2, 1] = "20";
        _form.ButtonAddSkillPowerItemClick();
        Assert.Equal(1, _form.g_SkillPowerItemList.Count);
        Assert.Equal(1, _form.ListBoxSkillPowerItem.Items.Count);
        Assert.True(_form.ButtonSaveSkillPowerItem.Enabled);

        // 重复添加
        _form.ButtonAddSkillPowerItemClick();
        Assert.Equal("该物品已经在装备技能威力列表中了！", M2Forms.LastMessage);

        // 点击回填
        _form.ListBoxSkillPowerItem.SelectedIndex = 0;
        Assert.Equal("木剑的技能威力百分比设置", _form.GroupBoxSkillPowerItem.Text);
        Assert.Equal("30", _form.SkillPowerGrid[1, 1]);
        Assert.Equal("20", _form.SkillPowerGrid[2, 1]);
        Assert.True(_form.ButtonDelSkillPowerItem.Enabled);

        // 修改
        _form.SkillPowerGrid[1, 3] = "55";
        _form.ButtonChgSkillPowerItemClick();
        Assert.Equal(55, _form.g_SkillPowerItemList.GetSkillPowerItem("木剑")!.AttackSkillPercent[3]);

        // 保存
        _form.ButtonSaveSkillPowerItemClick();
        Assert.False(_form.ButtonSaveSkillPowerItem.Enabled);
        string ini = File.ReadAllText(Path.Combine(_tempDir, "SkillPowerItemList.txt"), GXX.Core.EncodingInit.GBK);
        Assert.Contains("[木剑]", ini);
        Assert.Contains("Attack1=30", ini);
        Assert.Contains("Attack3=55", ini);
        Assert.Contains("Defense1=20", ini);
        Assert.DoesNotContain("Attack1=0", ini);

        // 删除
        _form.ButtonDelSkillPowerItemClick();
        Assert.Equal(0, _form.g_SkillPowerItemList.Count);
        Assert.False(_form.ButtonDelSkillPowerItem.Enabled);

        // 开关写配置
        _form.chkSkillPowerItemUseHum.Checked = true;
        _form.chkSkillPowerItemUseMon.Checked = false;
        Assert.True(M2Config.boSkillPowerItemUseHum);
        Assert.False(M2Config.boSkillPowerItemUseMon);
    }

    // ================= 窗体层：Open 与配置复选框 =================

    [Fact]
    public void OpenResidualSections_FillsListsAndDisablesButtons()
    {
        // Open 会清空并重填宝箱列表，故先垫入两只 StdMode=31 宝箱
        ViewList2State.g_BoxsList.AddBox(new TBox { Name = "檀木宝箱", Source = 0 });
        ViewList2State.g_BoxsList.AddBox(new TBox { Name = "紫铜宝箱", Source = 1 });

        M2Config.boGroupItemRule = true;
        M2Config.boSkillPowerItemUseHum = true;
        M2Config.boSkillPowerItemUseMon = false;
        M2Config.boEnablePlayerUseClientPickItems = true;   // {$IF NEED_KEY <> 2} 且 g_nKey=1 才回显
        M2Config.boEnableHeroUseClientPickItems = true;
        M2Config.boTZSupportRenameItem = true;
        M2Config.boSendFilterItemList = true;
        M2Config.boSingleHint = true;
        M2Config.boEnabledBuyShopItemGive = true;

        _form.OpenResidualSections(new (string, byte, ushort)[]
        {
            ("木剑", 5, 0),
            ("檀木宝箱", 31, 15),
            ("紫铜宝箱", 31, 16),
            ("普通物品", 0, 0),
        });

        Assert.Equal(4, _form.ListBoxitemList1.Items.Count);
        Assert.Equal(2, _form.ListBoxBoxItem.Items.Count);   // 仅 StdMode=31 且 Shape 15..49
        Assert.False(_form.btnAddBoxItem.Enabled);
        Assert.False(_form.btnSaveBoxItem.Enabled);
        Assert.False(_form.ButtonAddSkillPowerItem.Enabled);
        Assert.False(_form.ButtonItemRuleSave.Enabled);
        Assert.False(_form.ButtonUserCommandSave.Enabled);
        Assert.True(_form.RadioButtonValue.Checked);         // boGroupItemRule = True
        Assert.True(_form.chkSkillPowerItemUseHum.Checked);
        Assert.False(_form.chkSkillPowerItemUseMon.Checked);
        Assert.True(_form.chkEnablePlayerUseClientPickItems.Enabled);
        Assert.True(_form.chkEnablePlayerUseClientPickItems.Checked);
        Assert.True(_form.chkEnableHeroUseClientPickItems.Enabled);
        Assert.True(_form.chkEnableHeroUseClientPickItems.Checked);
        Assert.True(_form.chkTZSupportRenameItem.Checked);
        Assert.True(_form.chkSendFilterItemList.Checked);
        Assert.True(_form.chkSingleHint.Checked);
        Assert.True(_form.chkEnabledBuyShopItemGive.Checked);
        Assert.Equal("技能名称", _form.SkillPowerGrid[0, 0]);
        Assert.Equal("增加技能防御百分比", _form.SkillPowerGrid[2, 0]);
        Assert.Equal(5, _form.cbbAuctionPricesType.Items.Count);   // FormCreate 拍卖货币 5 项
        Assert.Equal(0, _form.cbbAuctionPricesType.SelectedIndex);
        Assert.True(_form.boOpened);
    }

    [Fact]
    public void OpenResidualSections_ClientPickItemsDisabledWhenKeyMissing()
    {
        _form.g_nKey_UseClientPickItems = 0;
        M2Config.boEnablePlayerUseClientPickItems = true;
        M2Config.boEnableHeroUseClientPickItems = true;
        _form.OpenResidualSections(Array.Empty<(string, byte, ushort)>());

        Assert.False(_form.chkEnablePlayerUseClientPickItems.Enabled);
        Assert.False(_form.chkEnableHeroUseClientPickItems.Enabled);
        Assert.False(_form.chkEnablePlayerUseClientPickItems.Checked);   // 未进入赋值分支
    }

    [Fact]
    public void ToggleHandlers_WriteConfigAndSendServerConfig()
    {
        int writes = 0;
        int sends = 0;
        _form.WriteBoolHandler = (section, key, value) =>
        {
            writes++;
            Assert.Equal("Setup", section);
        };
        _form.SendServerConfigHandler = () => sends++;
        _form.boOpened = true;

        _form.chkSendFilterItemList.Checked = true;
        Assert.True(M2Config.boSendFilterItemList);
        _form.chkSendItemDescList.Checked = true;
        Assert.True(M2Config.boSendItemDescList);
        _form.chkSendTzItemDescList.Checked = true;
        Assert.True(M2Config.boSendTzItemDescList);
        _form.chkSingleHint.Checked = true;
        Assert.Equal(1, sends);   // chkSingleHint 单独下发
        _form.chkEnabledBuyShopItemGive.Checked = true;
        Assert.Equal(2, sends);
        Assert.Equal(5, writes);   // 过滤/备注/套装备注/单件提示/购买赠送 五次写盘

        // boOpened 门控：客户端物品列表开关不写盘
        // boOpened=false：Delphi "if not boOpened then Exit" 早退，字段与 INI 均不动
        _form.boOpened = false;
        int before = writes;
        _form.chkEnablePlayerUseClientPickItems.Checked = true;
        _form.ChkEnablePlayerUseClientPickItemsClick();
        Assert.Equal(before, writes);
        Assert.False(M2Config.boEnablePlayerUseClientPickItems);

        _form.boOpened = true;
        _form.chkEnablePlayerUseClientPickItems.Checked = false;   // CheckedChanged 同步触发一次写盘
        Assert.False(M2Config.boEnablePlayerUseClientPickItems);
        Assert.Equal(before + 1, writes);

        _form.chkEnableHeroUseClientPickItems.Checked = true;   // CheckedChanged 同步触发一次写盘
        Assert.True(M2Config.boEnableHeroUseClientPickItems);
        Assert.Equal(before + 2, writes);

        // 改名三开关
        _form.chkTZSupportRenameItem.Checked = false;
        _form.chkDescSupportRenamItem.Checked = true;
        _form.chkNoRenameDescReadDefault.Checked = true;
        Assert.False(M2Config.boTZSupportRenameItem);
        Assert.True(M2Config.boDescSupportRenamItem);
        Assert.True(M2Config.boNoRenameDescReadDefault);
    }

    [Fact]
    public void FillSkillPowerGrid_DefAndCustomMagicRows()
    {
        _form.FillSkillPowerGrid(
            magicId => magicId switch
            {
                1 => "火球术",
                2 => "治愈术",       // 无效集合 → [无效]
                1000 => "自定义技能",
                _ => null,
            },
            _ => true);              // 自定义保护模式 → [无效]

        Assert.Equal("火球术", _form.SkillPowerGrid[0, 1]);
        Assert.Equal("治愈术[无效]", _form.SkillPowerGrid[0, 2]);
        Assert.Equal("", _form.SkillPowerGrid[0, 3]);
        Assert.Equal("0", _form.SkillPowerGrid[1, 1]);
        // 行号 = 技能 ID（自定义技能 ID = 1000 + 行内偏移）：行 251 → 1000 + 1
        Assert.Equal("自定义技能[无效]", _form.SkillPowerGrid[0, 251]);
        Assert.Equal("", _form.SkillPowerGrid[0, 300]);
    }

    [Fact]
    public void ItemRuleFormCreate_BuildsMenuEntriesAndAuctionCurrencies()
    {
        _form.ItemRuleFormCreate();
        // Delphi 循环全部 Tag>=0 的复选框：本窗体共 33 个（含 Visible=False 的 Tag=0）
        Assert.Equal(33, _form.ItemRuleMenuItems.Count);
        Assert.Equal("禁止丢弃", _form.ItemRuleMenuItems[0].Caption);
        Assert.Equal(0, _form.ItemRuleMenuItems[0].Tag);
        Assert.Equal(5, _form.cbbAuctionPricesType.Items.Count);
        Assert.Equal(M2Config.sGameGoldName, _form.cbbAuctionPricesType.Items[0]);
        Assert.Equal(M2Config.sGameGirdName, _form.cbbAuctionPricesType.Items[4]);
    }

    [Fact]
    public void InputQuery_CtrlFLocatesItemExactly()
    {
        _form.ListBoxitemList1.Items.AddRange(new object[] { "木剑", "屠龙" });
        _form.InputQueryHandler = (_, _, _) => (true, "屠龙");
        _form.ListBoxitemListKeyDown(_form.ListBoxitemList1, (int)System.Windows.Forms.Keys.F, true);
        Assert.Equal(1, _form.ListBoxitemList1.SelectedIndex);

        // 取消 → 不改变
        _form.InputQueryHandler = (_, _, _) => (false, "");
        _form.ListBoxitemList1.SelectedIndex = 0;
        _form.ListBoxitemListKeyDown(_form.ListBoxitemList1, (int)System.Windows.Forms.Keys.F, true);
        Assert.Equal(0, _form.ListBoxitemList1.SelectedIndex);

        // 非 Ctrl+F 不响应
        _form.InputQueryHandler = (_, _, _) => (true, "屠龙");
        _form.ListBoxitemListKeyDown(_form.ListBoxitemList1, (int)System.Windows.Forms.Keys.F, false);
        Assert.Equal(0, _form.ListBoxitemList1.SelectedIndex);
    }
}
