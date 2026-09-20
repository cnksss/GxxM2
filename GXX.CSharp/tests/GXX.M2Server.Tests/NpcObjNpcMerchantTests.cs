// ============================================================================
// 源单元：Source\M2Engine\ObjNpc.pas
// 测试对象：GXX.M2Server.Npc 切片 4（TMerchant 价格/货物族）
//   · AddItemPrice                 原文 1446-1455
//   · CheckItemPrice               原文 1457-1486
//   · GetRefillList                原文 1488-1510
//   · CheckItemType                原文 1630-1643
//   · GetItemPrice                 原文 1645-1672
//   · GetUserPrice                 原文 2052-2085
//   · ClearExpreUpgradeListData    原文 3160-3178
//   · GetUserItemPrice             原文 3272-3365
//   · GetSellItemPrice             原文 3793-3796
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Npc;
using Xunit;

namespace GXX.M2Server.Tests;

public sealed class NpcObjNpcMerchantTests : IDisposable
{
    public NpcObjNpcMerchantTests() => NpcSeams.ResetDefaults();

    public void Dispose() => NpcSeams.ResetDefaults();

    private static TStdItem Std(
        byte stdMode = 5, ushort duraMax = 100, int price = 100, ushort overLap = 0)
        => new() { StdMode = stdMode, DuraMax = duraMax, Price = price, OverLap = overLap };

    private static TUserItem Item(ushort wIndex, ushort dura = 0, ushort duraMax = 0)
        => new() { wIndex = wIndex, Dura = dura, DuraMax = duraMax };

    // -----------------------------------------------------------------------
    // CheckItemType（1630-1643）
    // -----------------------------------------------------------------------

    [Fact]
    public void CheckItemType_EmptyList_ReturnsFalse()
    {
        var m = new TMerchant();
        Assert.False(m.CheckItemType(5));
    }

    [Fact]
    public void CheckItemType_FindsBoxedInteger()
    {
        var m = new TMerchant();
        m.m_ItemTypeList.Add(5);
        m.m_ItemTypeList.Add(-1);
        m.m_ItemTypeList.Add(30);
        Assert.True(m.CheckItemType(5));
        Assert.True(m.CheckItemType(-1));
        Assert.True(m.CheckItemType(30));
        Assert.False(m.CheckItemType(6));
    }

    // -----------------------------------------------------------------------
    // AddItemPrice（1446-1455）/ CheckItemPrice（1457-1486）
    // -----------------------------------------------------------------------

    [Fact]
    public void AddItemPrice_AppendsAndPersists()
    {
        var m = new TMerchant { m_sScript = "TESTNPC", m_sMapName = "0" };
        string saved = null;
        NpcSeams.SaveGoodPriceRecord = (npc, key) => { saved = key; Assert.Same(m, npc); };

        m.AddItemPrice(7, 250);

        Assert.Single(m.m_ItemPriceList);
        Assert.Equal((ushort)7, ((TItemPrice)m.m_ItemPriceList[0]).wIndex);
        Assert.Equal(250, ((TItemPrice)m.m_ItemPriceList[0]).nPrice);
        Assert.Equal("TESTNPC-0", saved);
    }

    [Fact]
    public void AddItemPrice_DoesNotDeduplicate()
    {
        var m = new TMerchant();
        m.AddItemPrice(7, 250);
        m.AddItemPrice(7, 300);
        Assert.Equal(2, m.m_ItemPriceList.Count);
    }

    [Fact]
    public void CheckItemPrice_ExistingEntry_LeavesPriceUntouchedAndDoesNotPersist()
    {
        var m = new TMerchant();
        m.AddItemPrice(7, 250);
        int saves = 0;
        NpcSeams.SaveGoodPriceRecord = (_, _) => saves++;
        NpcSeams.GetStdItem = _ => Std(price: 999);

        m.CheckItemPrice(7);

        Assert.Single(m.m_ItemPriceList);
        Assert.Equal(250, ((TItemPrice)m.m_ItemPriceList[0]).nPrice);
        Assert.Equal(0, saves);
    }

    [Fact]
    public void CheckItemPrice_MissingStdItem_AddsNothing()
    {
        var m = new TMerchant();
        NpcSeams.GetStdItem = _ => null;
        m.CheckItemPrice(7);
        Assert.Empty(m.m_ItemPriceList);
    }

    [Fact]
    public void CheckItemPrice_UsesRoundPriceTimesOnePointOne()
    {
        var m = new TMerchant();
        NpcSeams.GetStdItem = _ => Std(price: 100);
        m.CheckItemPrice(7);
        Assert.Single(m.m_ItemPriceList);
        // Round(100 * 1.1) = Round(110.00000000000001) = 110
        Assert.Equal(110, ((TItemPrice)m.m_ItemPriceList[0]).nPrice);
    }

    [Fact]
    public void CheckItemPrice_ZeroPriceStdItem_StillAddsZero()
    {
        // 原文不判 Price > 0；GetStdItem 非 nil 即追加（价格可为 0）。
        var m = new TMerchant();
        NpcSeams.GetStdItem = _ => Std(price: 0);
        m.CheckItemPrice(7);
        Assert.Single(m.m_ItemPriceList);
        Assert.Equal(0, ((TItemPrice)m.m_ItemPriceList[0]).nPrice);
    }

    // -----------------------------------------------------------------------
    // GetRefillList（1488-1510）
    // -----------------------------------------------------------------------

    [Fact]
    public void GetRefillList_NonPositiveIndex_ReturnsNull()
    {
        var m = new TMerchant();
        Assert.Null(m.GetRefillList(0));
        Assert.Null(m.GetRefillList(-3));
    }

    [Fact]
    public void GetRefillList_MatchesByFirstItemIndexOnly()
    {
        var m = new TMerchant();
        var group = new List<object> { Item(5), Item(9) };
        m.m_GoodsList.Add(group);
        Assert.Same(group, m.GetRefillList(5));
        // 组内第 1 个元素不参与匹配
        Assert.Null(m.GetRefillList(9));
    }

    [Fact]
    public void GetRefillList_EmptyGroup_IsSkipped()
    {
        var m = new TMerchant();
        m.m_GoodsList.Add(new List<object>());
        Assert.Null(m.GetRefillList(5));
    }

    [Fact]
    public void GetRefillList_ReturnsFirstMatchingGroup()
    {
        var m = new TMerchant();
        var g1 = new List<object> { Item(5) };
        var g2 = new List<object> { Item(5) };
        m.m_GoodsList.Add(g1);
        m.m_GoodsList.Add(g2);
        Assert.Same(g1, m.GetRefillList(5));
    }

    // -----------------------------------------------------------------------
    // GetItemPrice（1645-1672）
    // -----------------------------------------------------------------------

    [Fact]
    public void GetItemPrice_PriceListHit_WinsOverStdItem()
    {
        var m = new TMerchant();
        m.AddItemPrice(5, 77);
        NpcSeams.GetStdItem = _ => Std(price: 999);
        Assert.Equal(77, m.GetItemPrice(5));
    }

    [Fact]
    public void GetItemPrice_FallsBackToStdItemOnlyWhenItemTypeAllowed()
    {
        var m = new TMerchant();
        NpcSeams.GetStdItem = _ => Std(stdMode: 5, price: 999);
        m.m_ItemTypeList.Add(5);
        Assert.Equal(999, m.GetItemPrice(5));

        var m2 = new TMerchant();
        NpcSeams.GetStdItem = _ => Std(stdMode: 5, price: 999);
        Assert.Equal(-1, m2.GetItemPrice(5));   // CheckItemType 为假 → 保持哨兵 -1
    }

    [Fact]
    public void GetItemPrice_NotFound_ReturnsMinusOneSentinel()
    {
        var m = new TMerchant();
        NpcSeams.GetStdItem = _ => null;
        Assert.Equal(-1, m.GetItemPrice(5));
    }

    [Fact]
    public void GetItemPrice_ZeroPriceStdItem_IsDistinctFromSentinel()
    {
        var m = new TMerchant();
        m.m_ItemTypeList.Add(5);
        NpcSeams.GetStdItem = _ => Std(stdMode: 5, price: 0);
        Assert.Equal(0, m.GetItemPrice(5));   // 0 ≠ -1
    }

    [Fact]
    public void GetItemPrice_NegativePriceInList_TriggersStdItemFallback()
    {
        // `if Result < 0` 不看来源 → 列表里存了负数也会再去查标准物品（原文缺陷/易错点，照抄）。
        var m = new TMerchant();
        m.AddItemPrice(5, -1);
        m.m_ItemTypeList.Add(5);
        NpcSeams.GetStdItem = _ => Std(stdMode: 5, price: 500);
        Assert.Equal(500, m.GetItemPrice(5));
    }

    // -----------------------------------------------------------------------
    // GetUserPrice（2052-2085）
    // -----------------------------------------------------------------------

    [Fact]
    public void GetUserPrice_NoCastle_AppliesPriceRate()
    {
        var m = new TMerchant { m_boCastle = false, m_nPriceRate = 100 };
        Assert.Equal(123, m.GetUserPrice(new GXX.M2Server.Engine.TPlayObject(), 123));
    }

    [Fact]
    public void GetUserPrice_NoCastle_HalfRate()
    {
        var m = new TMerchant { m_boCastle = false, m_nPriceRate = 50 };
        Assert.Equal(50, m.GetUserPrice(new GXX.M2Server.Engine.TPlayObject(), 100));
    }

    [Fact]
    public void GetUserPrice_CastleButNotMasterGuild_UsesFullRate()
    {
        var m = new TMerchant { m_boCastle = true, m_nPriceRate = 100 };
        NpcSeams.GetNpcCastle = _ => new object();
        NpcSeams.IsMasterGuild = (_, _) => false;
        Assert.Equal(100, m.GetUserPrice(new GXX.M2Server.Engine.TPlayObject(), 100));
    }

    [Fact]
    public void GetUserPrice_CastleMasterGuild_UsesIntegerDivisionQuirk()
    {
        // 原文 2073：`Max(60, Round(m_nPriceRate * (g_Config.nCastleMemberPriceRate / 100)))`
        // 括号内是**整数除法** → 80 / 100 = 0 → n14 恒为 60（而不是 80% × rate）。
        var m = new TMerchant { m_boCastle = true, m_nPriceRate = 100 };
        NpcSeams.GetNpcCastle = _ => new object();
        NpcSeams.IsMasterGuild = (_, _) => true;
        Assert.Equal(60, m.GetUserPrice(new GXX.M2Server.Engine.TPlayObject(), 100));
    }

    [Fact]
    public void GetUserPrice_CastleNull_SkipsMasterGuildCheck()
    {
        var m = new TMerchant { m_boCastle = true, m_nPriceRate = 100 };
        NpcSeams.GetNpcCastle = _ => null;
        bool called = false;
        NpcSeams.IsMasterGuild = (_, _) => { called = true; return true; };
        Assert.Equal(100, m.GetUserPrice(new GXX.M2Server.Engine.TPlayObject(), 100));
        Assert.False(called);
    }

    [Fact]
    public void GetUserPrice_ZeroPrice_IsZero()
    {
        var m = new TMerchant { m_nPriceRate = 100 };
        Assert.Equal(0, m.GetUserPrice(new GXX.M2Server.Engine.TPlayObject(), 0));
    }

    // -----------------------------------------------------------------------
    // ClearExpreUpgradeListData（3160-3178）
    // -----------------------------------------------------------------------

    [Fact]
    public void ClearExpreUpgradeListData_EmptyList_IsNoOp()
    {
        var m = new TMerchant();
        m.ClearExpreUpgradeListData();
        Assert.Empty(m.m_UpgradeWeaponList);
    }

    [Fact]
    public void ClearExpreUpgradeListData_RemovesExpiredKeepsFresh()
    {
        var m = new TMerchant();
        var expired = new TUpgradeInfo { dtTime = DateTime.Now.AddDays(-100) };
        var fresh = new TUpgradeInfo { dtTime = DateTime.Now };
        m.m_UpgradeWeaponList.Add(expired);
        m.m_UpgradeWeaponList.Add(fresh);

        m.ClearExpreUpgradeListData();

        Assert.Single(m.m_UpgradeWeaponList);
        Assert.Same(fresh, m.m_UpgradeWeaponList[0]);
    }

    [Fact]
    public void ClearExpreUpgradeListData_NullEntryIsSkipped()
    {
        var m = new TMerchant();
        m.m_UpgradeWeaponList.Add(null);
        m.ClearExpreUpgradeListData();
        Assert.Single(m.m_UpgradeWeaponList);   // Continue（不删除）
    }

    [Fact]
    public void ClearExpreUpgradeListData_RemovesAllWhenAllExpired()
    {
        var m = new TMerchant();
        for (int i = 0; i < 5; i++)
            m.m_UpgradeWeaponList.Add(new TUpgradeInfo { dtTime = DateTime.Now.AddDays(-30) });
        m.ClearExpreUpgradeListData();
        Assert.Empty(m.m_UpgradeWeaponList);
    }

    // -----------------------------------------------------------------------
    // GetSellItemPrice（3793-3796）
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]    // Round(0.5) = 0（银行家舍入）
    [InlineData(3, 2)]    // Round(1.5) = 2
    [InlineData(5, 2)]    // Round(2.5) = 2（银行家舍入，不是 3）
    [InlineData(100, 50)]
    [InlineData(-3, -2)]  // Round(-1.5) = -2
    public void GetSellItemPrice_BankerRounding(int input, int expected)
    {
        var m = new TMerchant();
        Assert.Equal(expected, m.GetSellItemPrice(input));
    }

    // -----------------------------------------------------------------------
    // GetUserItemPrice（3272-3365）
    // -----------------------------------------------------------------------

    [Fact]
    public void GetUserItemPrice_NonPositiveBasePrice_ReturnsItUnchanged()
    {
        var m = new TMerchant();
        NpcSeams.GetStdItem = _ => null;
        var item = Item(5);
        Assert.Equal(-1, m.GetUserItemPrice(ref item, false));
    }

    [Fact]
    public void GetUserItemPrice_StdModeBelowFive_SkipsDurabilityMath()
    {
        var m = new TMerchant();
        m.AddItemPrice(5, 40);
        NpcSeams.GetStdItem = _ => Std(stdMode: 3, duraMax: 10);
        var item = Item(5, dura: 1, duraMax: 10);
        Assert.Equal(40, m.GetUserItemPrice(ref item, false));
    }

    [Fact]
    public void GetUserItemPrice_StdMode40_MeatBranch()
    {
        var m = new TMerchant();
        m.AddItemPrice(5, 100);
        NpcSeams.GetStdItem = _ => Std(stdMode: 40, duraMax: 100);
        var item = Item(5, dura: 50, duraMax: 100);
        // 肉分支：n20 = 100/2/100*(100-50) = 25 → n10 = Max(2, 75) = 75
        // 后续 StdMode>4：加属性 n14 = 0（btValue 全 0）→ 不变
        // 非叠加（OverLap=0）→ n10 = Round(75/100*100) = 75；n20 = 75/2/100*50 = 18.75
        //            → n10 = Max(2, Round(56.25)) = 56
        Assert.Equal(56, m.GetUserItemPrice(ref item, false));
        Assert.Equal((ushort)100, item.DuraMax);   // 非 43 分支不改 DuraMax
    }

    [Fact]
    public void GetUserItemPrice_StdMode43_ForcesDuraMaxTo10000()
    {
        var m = new TMerchant();
        m.AddItemPrice(5, 100);
        NpcSeams.GetStdItem = _ => Std(stdMode: 43, duraMax: 100);
        var item = Item(5, dura: 5000, duraMax: 5000);

        int price = m.GetUserItemPrice(ref item, false);

        // ⚠ 原文 3302-3303 就地改写入参：DuraMax < 10000 → 置 10000。
        Assert.Equal((ushort)10000, item.DuraMax);
        // 43 分支：n20 = 100/2/10000*(10000-5000) = 25 → n10 = Max(2, 75) = 75
        // 非叠加（OverLap=0）→ n10 = Round(n10 / **StdItem.DuraMax(100)** * UserItem.DuraMax(10000))
        //            = Round(7500) = 7500；n20 = 7500/2/10000*(10000-5000) = 1875
        //            → n10 = Max(2, Round(7500-1875)) = 5625
        Assert.Equal(5625, price);
    }

    [Fact]
    public void GetUserItemPrice_OverlapItem_MultipliesByDuraPlusOne()
    {
        var m = new TMerchant();
        m.AddItemPrice(5, 10);
        // OverLap > 0 且 StdMode ∈ {0,2,3,31,40,41,42,46,47} 且 DuraMax > 1 → 可叠加
        NpcSeams.GetStdItem = _ => Std(stdMode: 0, duraMax: 5, overLap: 1);
        var item = Item(5, dura: 2, duraMax: 5);
        Assert.Equal(10 * (2 + 1), m.GetUserItemPrice(ref item, false));
    }

    [Fact]
    public void GetUserItemPrice_ZeroDuraMax_SkipsDurabilityMath()
    {
        var m = new TMerchant();
        m.AddItemPrice(5, 100);
        NpcSeams.GetStdItem = _ => Std(stdMode: 5, duraMax: 100);
        var item = Item(5, dura: 0, duraMax: 0);   // UserItem.DuraMax = 0 → 整个 if 不进
        Assert.Equal(100, m.GetUserItemPrice(ref item, false));
    }

    [Fact]
    public void GetUserItemPrice_AddPropertyBonus_AppliedWhenNotSellToNpc()
    {
        var m = new TMerchant();
        m.AddItemPrice(5, 100);
        NpcSeams.GetStdItem = _ => Std(stdMode: 5, duraMax: 100);
        var item = Item(5, dura: 100, duraMax: 100);
        item.SetBtValue(0, 20);   // StdMode ∈ {5,6,68,69} 且 nC != 6 → n14 += 20

        // 加属性：n14 = 20 → n10 = 100 + Round(100*20/200) = 100 + 10 = 110
        // 非叠加（OverLap=0）→ n10 = Round(110/100*100) = 110；n20 = 110/2/100*(100-100) = 0
        //            → n10 = Max(2, 110) = 110
        Assert.Equal(110, m.GetUserItemPrice(ref item, false));
    }

    [Fact]
    public void GetUserItemPrice_SellToNpcWithNoCalcFlag_SkipsAddPropertyBonus()
    {
        // 原文 3315：`(not IsSellToNpc) or (not g_Config.boSellItemToNpcShopNoCalcAddProperty)`
        // → IsSellToNpc = True 且开关 = True 时为假 → 跳过加成。
        var m = new TMerchant();
        m.AddItemPrice(5, 100);
        NpcSeams.GetStdItem = _ => Std(stdMode: 5, duraMax: 100);
        var item = Item(5, dura: 100, duraMax: 100);
        item.SetBtValue(0, 20);

        bool old = GXX.M2Server.Engine.M2Config.boSellItemToNpcShopNoCalcAddProperty;
        try
        {
            GXX.M2Server.Engine.M2Config.boSellItemToNpcShopNoCalcAddProperty = true;
            Assert.Equal(100, m.GetUserItemPrice(ref item, true));
        }
        finally
        {
            GXX.M2Server.Engine.M2Config.boSellItemToNpcShopNoCalcAddProperty = old;
        }
    }

    [Fact]
    public void GetUserItemPrice_StdMode5_Slot6HasDoubledExcessBonus()
    {
        var m = new TMerchant();
        m.AddItemPrice(5, 200);
        NpcSeams.GetStdItem = _ => Std(stdMode: 5, duraMax: 100);
        var item = Item(5, dura: 100, duraMax: 100);
        item.SetBtValue(6, 15);   // nC = 6 且 > 10 → +（15-10)*2 = 10

        // n14 = 10 → n10 = 200 + Round(200*10/200) = 200 + 10 = 210
        // 非叠加 → Round(210/100*100) = 210；n20 = 0 → 210
        Assert.Equal(210, m.GetUserItemPrice(ref item, false));
    }
}
