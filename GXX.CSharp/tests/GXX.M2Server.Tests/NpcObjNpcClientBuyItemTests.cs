// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas
// 测试对象：TMerchant.ClientBuyItem（原文 3367-3688，323 行）+ 其两个抽出私有方法
//           （`BuildNextGoodsDisplay` = 原文 :3486-3531/:3621-3666 的逐字重复块；
//            `ChargeCastleTax`   = 原文 :3445-3455/:3588-3598 的逐字重复块）
//           + `sub_4A1C84` 同族的 `CheckOverLap`（复用 `VisibleItemLifecycleCore`）
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Npc;
using Xunit;

namespace GXX.M2Server.Tests;

public sealed class NpcObjNpcClientBuyItemTests : IDisposable
{
    private readonly List<string> _sent = new();
    private readonly List<string> _logs = new();
    private readonly List<string> _sysMsgs = new();
    private readonly List<string> _addItem = new();

    public NpcObjNpcClientBuyItemTests()
    {
        NpcSeams.ResetDefaults();
        OnlineMsgControl.ResetDefaults();
        NpcSeams.SendTo = (sender, target, ident, wParam, p1, p2, p3, sMsg) =>
            _sent.Add($"{ident}|{wParam}|{p1}|{p2}|{p3}|{sMsg}");
        NpcSeams.AddGameDataLog = (a1, a2, actor, item, makeIdx, target, d1, d2, desc) =>
            _logs.Add($"{a1}/{a2}/{item}/{makeIdx}/{d1}/{d2}/{desc}");
        NpcSeams.SysMsg = (t, msg, color, type) => _sysMsgs.Add($"{msg}/{color}/{type}");
        NpcSeams.MainOutMessage = _ => { };
        NpcSeams.GetVariableText = (n, p, sMsg, sVar, nPos) => (false, sMsg, false);
        NpcSeams.CopyToUserItemFromName = (string name, ref TUserItem item) =>
        {
            item.MakeIndex = 4242;   // 原文 :3555 要求"新分配的 MakeIndex"可读
            return true;
        };
    }

    public void Dispose()
    {
        NpcSeams.ResetDefaults();
        OnlineMsgControl.ResetDefaults();
    }

    private static TStdItem Std(byte stdMode = 5, ushort duraMax = 100, int price = 100, byte weight = 10,
        ushort overLap = 0, byte needIdentify = 0, string name = "物品")
    {
        var s = new TStdItem
        {
            StdMode = stdMode,
            DuraMax = duraMax,
            Price = price,
            Weight = weight,
            OverLap = overLap,
            NeedIdentify = needIdentify,
        };
        s.NameStr = name;
        return s;
    }

    private static TUserItem Item(ushort wIndex, ushort dura = 0, ushort duraMax = 0, int makeIndex = 1,
        string name = "")
    {
        var it = new TUserItem { wIndex = wIndex, Dura = dura, DuraMax = duraMax, MakeIndex = makeIndex };
        if (name != "") it.NameStr = name;
        return it;
    }

    private static TPlayObject Player(uint gold = 100000)
    {
        var p = new TPlayObject { m_sCharName = "买家", m_nGold = gold };
        // 原文 3428 的重量门：`(m_WAbil.Weight + nWeight) <= m_WAbil.MaxWeight`；
        // 默认全零会让**任何正重量**都被拒（n1C = 2），故显式给足上限。
        p.m_wAbil.MaxWeight = 30000;
        return p;
    }

    /// <summary>搭一个"单组单件"的商店；返回该商品件以便断言。</summary>
    private static TMerchant NewMerchant(TUserItem goodsItem, TStdItem std, int price = 100, int weightLimit = 1000)
    {
        var m = new TMerchant { m_sScript = "NPC", m_sMapName = "0", m_sCharName = "商人" };
        m.AddItemPrice(goodsItem.wIndex, price);
        var group = new List<object> { goodsItem };
        m.m_GoodsList.Add(group);
        NpcSeams.GetStdItem = _ => std;
        NpcSeams.GetStdItemName = _ => std.NameStr;
        PlayerSurfaceBaseSeams.GetMaxBagCount = _ => 10;
        _ = weightLimit;
        return m;
    }

    /// <summary>让 `IsAddWeightAvailable` 恒真（重量门与本组用例无关）。</summary>
    private static void AllowWeight() => PlayerSurfaceBaseSeams.WeightChanged = _ => { };

    // -----------------------------------------------------------------------
    // 门槛分支（3386-3395）
    // -----------------------------------------------------------------------

    [Fact]
    public void ClientBuyItem_BuyDisabled_SendsNothing()
    {
        var m = NewMerchant(Item(5), Std());
        OnlineMsgControl.g_OnlineMsgControl.boDisableBuy = true;
        m.ClientBuyItem(Player(), "物品", 1, 1, false);
        Assert.Empty(_sent);
    }

    [Fact]
    public void ClientBuyItem_ItemNameMismatch_FailsWithCode1()
    {
        var m = NewMerchant(Item(5), Std());
        m.ClientBuyItem(Player(), "别的物品", 1, 1, false);
        Assert.Single(_sent);
        // 原文 `n1C := 1` 是**初值**：完全没有命中时保持 1（3/2 只在命中后才被赋值）
        Assert.StartsWith($"{Grobal2Const.RM_BUYITEM_FAIL}|0|1|0|0|", _sent[0]);
    }

    [Fact]
    public void ClientBuyItem_MakeIndexMismatch_FailsWithCode1()
    {
        var m = NewMerchant(Item(5, makeIndex: 1), Std());
        m.ClientBuyItem(Player(), "物品", 1, 999, false);   // nInt 与 MakeIndex 不符
        Assert.StartsWith($"{Grobal2Const.RM_BUYITEM_FAIL}|0|1|0|0|", _sent[0]);
    }

    [Fact]
    public void ClientBuyItem_CustomItemNameIsPreferred()
    {
        // 原文 3412-3413：btValue[13]=1 且 Name <> '' → 用自定义名比较
        var goods = Item(5, name: "自定义名");
        goods.SetBtValue(13, 1);
        var m = NewMerchant(goods, Std(name: "标准名"));
        m.ClientBuyItem(Player(), "自定义名", 1, 1, false);
        Assert.StartsWith($"{Grobal2Const.RM_BUYITEM_SUCCESS}|", _sent[0]);
    }

    [Fact]
    public void ClientBuyItem_NotEnoughGold_FailsWithCode3()
    {
        // 原文 3439：`(m_nGold >= nPrice) and (nPrice > 0)`
        var m = NewMerchant(Item(5), Std(price: 1000));
        m.ClientBuyItem(Player(gold: 10), "物品", 1, 1, false);
        Assert.StartsWith($"{Grobal2Const.RM_BUYITEM_FAIL}|0|3|0|0|", _sent[0]);
    }

    [Fact]
    public void ClientBuyItem_ZeroPrice_FailsWithCode3()
    {
        // `nPrice > 0` 门：标准物品价 0 且无价目表 → GetItemPrice 走 -1，GetUserPrice 得负
        var m = new TMerchant { m_sScript = "NPC", m_sMapName = "0", m_sCharName = "商人" };
        var group = new List<object> { Item(5) };
        m.m_GoodsList.Add(group);
        NpcSeams.GetStdItem = _ => Std(price: 0);
        NpcSeams.GetStdItemName = _ => "物品";
        var p = Player();
        m.ClientBuyItem(p, "物品", 1, 1, false);
        Assert.StartsWith($"{Grobal2Const.RM_BUYITEM_FAIL}|0|3|0|0|", _sent[0]);
    }

    [Fact]
    public void ClientBuyItem_NegativeOrZeroCount_IsForcedToOne()
    {
        // 原文 3394-3395：`nCount <= 0` → 1。非叠加物品 nCount 不参与价格 → 与 nCount=1 同结果
        var m = NewMerchant(Item(5, dura: 0, duraMax: 0), Std(price: 100));
        m.ClientBuyItem(Player(), "物品", 0, 1, false);
        Assert.StartsWith($"{Grobal2Const.RM_BUYITEM_SUCCESS}|", _sent[0]);
    }

    [Fact]
    public void ClientBuyItem_Bo574_BlocksPurchase()
    {
        var m = NewMerchant(Item(5), Std());
        m.bo574 = true;
        m.ClientBuyItem(Player(), "物品", 1, 1, false);
        // bo574 使 for 首轮即 Break → n1C 保持 1 → FAIL(1)
        Assert.StartsWith($"{Grobal2Const.RM_BUYITEM_FAIL}|0|1|0|0|", _sent[0]);
    }

    // -----------------------------------------------------------------------
    // 非叠加成功路径（3581-3687）
    // -----------------------------------------------------------------------

    [Fact]
    public void ClientBuyItem_NonOverlapSuccess_MovesItemToBagAndDeductsGold()
    {
        var m = NewMerchant(Item(5, makeIndex: 7), Std(stdMode: 5, price: 100), price: 100);
        var p = Player(gold: 1000);

        m.ClientBuyItem(p, "物品", 1, 7, false);

        // 售价 = GetUserPrice(GetUserItemPrice(...)) = 100
        Assert.Equal(900u, p.m_nGold);
        Assert.Single(p.Bag);
        Assert.Equal((ushort)5, p.Bag[0]!.Value.wIndex);
        // 商品被移出商品表（原文 3583 List20.Delete(II) → 组空 → 3479/3618 删组）
        Assert.Empty(m.m_GoodsList);
        // 成功后发包：wParam = Integer(IsFromTradingDlg) = 0
        Assert.Single(_sent);
        // ⚠ 原文易错点：**非叠加**路径（:3581-3585）**不设** `nItemCount`，故它保持 3392 的初值 0
        //    （`nItemCount` 只在叠加分支 :3469/:3562 被赋值）→ nParam3 = 0。
        Assert.StartsWith($"{Grobal2Const.RM_BUYITEM_SUCCESS}|0|900|7|0|", _sent[0]);
    }

    [Fact]
    public void ClientBuyItem_FromTradingDlg_SuccessParamIsOne()
    {
        var m = NewMerchant(Item(5, makeIndex: 7), Std(price: 100), price: 100);
        m.ClientBuyItem(Player(gold: 1000), "物品", 1, 7, true);
        // 原文 3685：`Integer(IsFromTradingDlg)` → True = 1
        Assert.StartsWith($"{Grobal2Const.RM_BUYITEM_SUCCESS}|1|", _sent[0]);
    }

    [Fact]
    public void ClientBuyItem_SetsItemFromToShopBuy_AndSyncsIntoBag_D36()
    {
        // ★ D36：原文靠指针别名让 3603-3608 的 ItemFrom 修改自动进背包；托管必须显式同步。
        var m = NewMerchant(Item(5, makeIndex: 7), Std(price: 100), price: 100);
        var p = Player(gold: 1000);

        m.ClientBuyItem(p, "物品", 1, 7, false);

        Assert.Single(p.Bag);
        Assert.Equal(TItemFormType.ifShopBuy, p.Bag[0]!.Value.ItemFrom.ItemForm);
        // ⚠ 原文 3606 用的是 **`PlayObject.m_sCharName`（买家名）**，不是 NPC 名 —— 照抄
        Assert.Equal("买家", p.Bag[0]!.Value.ItemFrom.MakerName);
        Assert.True(p.Bag[0]!.Value.ItemFrom.DateTime > 0);
    }

    [Fact]
    public void ClientBuyItem_ExistingIfShopBuy_KeepsMakerName()
    {
        // 原文 3603：只有 `ifUnknow` 才改写；已是 ifShopBuy 则 MakerName 不被覆盖
        var goods = Item(5, makeIndex: 7);
        goods.ItemFrom.ItemForm = TItemFormType.ifShopBuy;
        goods.ItemFrom.MakerName = "原主人";
        var m = NewMerchant(goods, Std(price: 100), price: 100);
        var p = Player(gold: 1000);

        m.ClientBuyItem(p, "物品", 1, 7, false);

        Assert.Equal("原主人", p.Bag[0]!.Value.ItemFrom.MakerName);
    }

    [Fact]
    public void ClientBuyItem_NeedIdentify_WritesBuyLogWithMinusPrice()
    {
        var m = NewMerchant(Item(5, makeIndex: 7), Std(price: 100, needIdentify: 1), price: 100);
        var p = Player(gold: 1000);

        m.ClientBuyItem(p, "物品", 1, 7, false);

        Assert.Single(_logs);
        // LOG_ItemBuy=11 / LOG_GoldChange=50 / Data1=m_nGold / Data2=-nPrice
        Assert.Equal($"11/50/物品/7/900/-100/NPC购买 [金币]", _logs[0]);
    }

    [Fact]
    public void ClientBuyItem_NoNeedIdentify_WritesNoLog()
    {
        var m = NewMerchant(Item(5, makeIndex: 7), Std(price: 100, needIdentify: 0), price: 100);
        m.ClientBuyItem(Player(gold: 1000), "物品", 1, 7, false);
        Assert.Empty(_logs);
    }

    [Fact]
    public void ClientBuyItem_NotEnoughBag_FailsWithCode2()
    {
        // 原文 3671：`IsEnoughBag` 为假 → n1C := 2（**与门槛失败的 3 不同**）
        var m = NewMerchant(Item(5, makeIndex: 7), Std(price: 100), price: 100);
        PlayerSurfaceBaseSeams.GetMaxBagCount = _ => 0;   // 背包恒满

        m.ClientBuyItem(Player(gold: 1000), "物品", 1, 7, false);

        Assert.StartsWith($"{Grobal2Const.RM_BUYITEM_FAIL}|0|2|0|0|", _sent[0]);
    }

    // -----------------------------------------------------------------------
    // 叠加路径（3423-3437 / 3538-3577）
    // -----------------------------------------------------------------------

    [Fact]
    public void ClientBuyItem_OverlapInBag_MergesIntoExistingStack()
    {
        // 原文 3441-3442：`OverLapItems` 命中 → 直接并入背包已有的那叠，**不新增格子**
        var m = NewMerchant(Item(5, dura: 9, duraMax: 100, makeIndex: 7), Std(stdMode: 0, overLap: 1, price: 100),
            price: 100);
        var p = Player(gold: 1000);
        var existing = Item(5, dura: 0, makeIndex: 8);
        p.AddToBag(existing);
        NpcSeams.OverLapItems = (_, _, _) => existing;

        m.ClientBuyItem(p, "物品", 1, 7, false);

        Assert.Single(p.Bag);                       // 没有新增格子
        Assert.Equal(900u, p.m_nGold);
        Assert.StartsWith($"{Grobal2Const.RM_BUYITEM_SUCCESS}|", _sent[0]);
    }

    [Fact]
    public void ClientBuyItem_OverlapFullStack_RemovesGoodsEntry()
    {
        // 原文 3458-3463：`nDura <= 0` → nCount=0 且商品被移出（这叠卖光了）
        var m = NewMerchant(Item(5, dura: 0, duraMax: 100, makeIndex: 7),
            Std(stdMode: 0, overLap: 1, price: 100), price: 100);
        var p = Player(gold: 1000);
        var existing = Item(5, makeIndex: 8);
        p.AddToBag(existing);
        NpcSeams.OverLapItems = (_, _, _) => existing;

        // nCount = 1，UserItem.Dura + 1 = 1 → nDura = 1 - 1 = 0 → <= 0
        m.ClientBuyItem(p, "物品", 1, 7, false);

        Assert.Empty(m.m_GoodsList);
        Assert.StartsWith($"{Grobal2Const.RM_BUYITEM_SUCCESS}|", _sent[0]);
    }

    [Fact]
    public void ClientBuyItem_NoOverlapAndOverlapItem_AddsNewStack()
    {
        // 原文 3552-3570：背包没有同类 → 新建一件（保留 `CopyToUserItemFromName` 分配的 MakeIndex）
        var m = NewMerchant(Item(5, dura: 9, duraMax: 100, makeIndex: 7),
            Std(stdMode: 0, overLap: 1, price: 100), price: 100);
        var p = Player(gold: 1000);
        NpcSeams.OverLapItems = (_, _, _) => null;   // 背包里没有同类

        m.ClientBuyItem(p, "物品", 3, 7, false);

        Assert.Single(p.Bag);
        // ★ 新件的 MakeIndex 来自 CopyToUserItemFromName（本测试注入 4242），**不是**商品那件的 7
        Assert.Equal(4242, p.Bag[0]!.Value.MakeIndex);
        // 部分购买：nCount=3 → 新件 Dura = nCount-1 = 2
        Assert.Equal((ushort)2, p.Bag[0]!.Value.Dura);
    }

    [Fact]
    public void ClientBuyItem_CopyToUserItemFromNameFails_BreaksWithoutAdding()
    {
        // 原文 3572-3576：复制失败 → Dispose + Break（不 AddItemToBag、不扣钱）
        var m = NewMerchant(Item(5, dura: 9, duraMax: 100, makeIndex: 7),
            Std(stdMode: 0, overLap: 1, price: 100), price: 100);
        var p = Player(gold: 1000);
        NpcSeams.OverLapItems = (_, _, _) => null;
        NpcSeams.CopyToUserItemFromName = (string _, ref TUserItem _) => false;

        m.ClientBuyItem(p, "物品", 3, 7, false);

        Assert.Empty(p.Bag);
        Assert.Equal(1000u, p.m_nGold);
        // n1C 保持 1（未设为 0）→ FAIL(1)
        Assert.StartsWith($"{Grobal2Const.RM_BUYITEM_FAIL}|0|1|0|0|", _sent[0]);
    }

    [Fact]
    public void ClientBuyItem_OverlapCountGreaterThanStack_IsClamped()
    {
        // 原文 3436-3437：`nCount > UserItem.Dura + 1` → 夹到 `UserItem.Dura + 1`（防刷）
        // 这里让 OverLapItems 返回 null（走"新占一格"路），nCount 被夹后新件 Dura = nCount-1
        var m = NewMerchant(Item(5, dura: 1, duraMax: 100, makeIndex: 7),
            Std(stdMode: 0, overLap: 1, price: 100), price: 100);
        var p = Player(gold: 100000);
        NpcSeams.OverLapItems = (_, _, _) => null;

        m.ClientBuyItem(p, "物品", 99, 7, false);   // Dura+1 = 2 → nCount 夹到 2

        Assert.Single(p.Bag);
        Assert.Equal((ushort)1, p.Bag[0]!.Value.Dura);   // nCount-1 = 1
    }

    // -----------------------------------------------------------------------
    // 税收（ChargeCastleTax = 原文两处逐字重复块，D33 同型）
    // -----------------------------------------------------------------------

    [Fact]
    public void ClientBuyItem_CastleTax_ChargedOnCastleInstance()
    {
        var m = NewMerchant(Item(5, makeIndex: 7), Std(price: 100), price: 100);
        m.m_boCastle = true;
        object castle = new object();
        NpcSeams.GetNpcCastle = _ => castle;
        int taxed = 0;
        object taxedOn = null;
        NpcSeams.IncRateGoldOnCastle = (c, g) => { taxedOn = c; taxed = g; };
        int managerTaxed = 0;
        NpcSeams.IncRateGoldOnCastleManager = g => managerTaxed = g;

        m.ClientBuyItem(Player(gold: 1000), "物品", 1, 7, false);

        Assert.Same(castle, taxedOn);
        Assert.Equal(100, taxed);          // 本次成交价
        Assert.Equal(0, managerTaxed);     // 有城堡时**不**走管理器
    }

    [Fact]
    public void ClientBuyItem_NoCastleButGetAllNpcTax_ManagerChargedUpgradeWeaponPrice()
    {
        // ★ D33 同型差异断言：原文 3453/3596 传的是 `nUpgradeWeaponPrice`，**不是**成交价
        var m = NewMerchant(Item(5, makeIndex: 7), Std(price: 100), price: 100);
        m.m_boCastle = true;
        NpcSeams.GetNpcCastle = _ => null;

        bool oldTax = M2Config.boGetAllNpcTax;
        int oldPrice = M2Config.nUpgradeWeaponPrice;
        try
        {
            M2Config.boGetAllNpcTax = true;
            M2Config.nUpgradeWeaponPrice = 555;
            int managerTaxed = -1;
            NpcSeams.IncRateGoldOnCastleManager = g => managerTaxed = g;

            m.ClientBuyItem(Player(gold: 1000), "物品", 1, 7, false);

            Assert.Equal(555, managerTaxed);   // 而不是 100（成交价）
        }
        finally
        {
            M2Config.boGetAllNpcTax = oldTax;
            M2Config.nUpgradeWeaponPrice = oldPrice;
        }
    }

    [Fact]
    public void ClientBuyItem_NoCastleNoTaxFlag_NoTax()
    {
        var m = NewMerchant(Item(5, makeIndex: 7), Std(price: 100), price: 100);
        m.m_boCastle = false;
        int taxed = -1;
        NpcSeams.IncRateGoldOnCastle = (_, g) => taxed = g;
        m.ClientBuyItem(Player(gold: 1000), "物品", 1, 7, false);
        Assert.Equal(-1, taxed);
    }

    // -----------------------------------------------------------------------
    // 下一件展示（BuildNextGoodsDisplay = 原文 3486-3531/3621-3666）
    // -----------------------------------------------------------------------

    [Fact]
    public void ClientBuyItem_OverlapFullStack_ShowsNextItemInSuccessText()
    {
        // 组里放两件同名同 MakeIndex 的（第二件用于"卖光后展示下一件"）：
        // 倒序遍历 → 先处理下标 1，卖光后 List20[0] 成为下一件
        var goods2 = Item(5, dura: 0, duraMax: 100, makeIndex: 7);
        var m = new TMerchant { m_sScript = "NPC", m_sMapName = "0", m_sCharName = "商人" };
        m.AddItemPrice(5, 100);
        var group = new List<object> { Item(5, dura: 0, duraMax: 100, makeIndex: 7), goods2 };
        m.m_GoodsList.Add(group);
        NpcSeams.GetStdItem = _ => Std(stdMode: 0, overLap: 1, price: 100);
        NpcSeams.GetStdItemName = _ => "物品";
        PlayerSurfaceBaseSeams.GetMaxBagCount = _ => 10;
        var p = Player(gold: 1000);
        NpcSeams.OverLapItems = (_, _, _) => Item(5, makeIndex: 9);

        m.ClientBuyItem(p, "物品", 1, 7, false);

        // 展示串非空（nSubMenu=0 分支 = `名/0/nPrice/nStock/nCount`）；
        // ⚠ 原文 3514/3648：`StdMode <= 4` 时 **`nStock` 被复用成 `UserItem.MakeIndex`** → 7 而不是 1
        Assert.StartsWith($"{Grobal2Const.RM_BUYITEM_SUCCESS}|0|", _sent[0]);
        Assert.EndsWith("|物品/0/100/7/1", _sent[0]);
    }
}
