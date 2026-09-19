using System;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>ItemRules.pas / ItmUnit.pas 测试：物品规则表 + 随机升级。</summary>
public unsafe class ItemRuleTests
{
    [Fact]
    public void ItemRules_AddFindDelete()
    {
        var rules = new TItemRules();
        var flags = new TFlagArray();
        flags[0] = true; // 禁止丢弃
        flags[1] = true; // 禁止交易

        Assert.True(rules.Add("屠龙", flags));
        Assert.False(rules.Add("屠龙", flags), "重复添加应失败");
        Assert.Equal(1, rules.Count);

        Assert.NotNull(rules.Find("屠龙"));
        Assert.Null(rules.Find("无极棍"));

        Assert.True(rules.Get(0, 0), "规则 0：禁止丢弃");
        Assert.True(rules.Get(0, 1), "规则 1：禁止交易");
        Assert.False(rules.Get(0, 5), "规则 5 未启用");

        Assert.True(rules.Delete("屠龙"));
        Assert.Equal(0, rules.Count);
        Assert.False(rules.Get(0, 0));
    }
    [Fact]
    public void ItemRules_Persistence()
    {
        string file = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "rule_" + Guid.NewGuid().ToString("N") + ".txt");
        try
        {
            var rules = new TItemRules();
            var flags = new TFlagArray();
            flags[3] = true;
            flags[6] = true;
            rules.Add("骨玉权杖", flags);
            rules.SaveToFile(file);

            var rules2 = new TItemRules();
            rules2.LoadFromFile(file);
            Assert.Equal(1, rules2.Count);
            Assert.True(rules2.Get(0, 3));
            Assert.True(rules2.Get(0, 6));
        }
        finally
        {
            System.IO.File.Delete(file);
        }
    }

    [Fact]
    public void ItemUnit_RandomUpgradeWeapon_Bounded()
    {
        // 极限概率配置：必出、且不超上限
        M2Config.nWeaponDCAddRate = 1;              // 100% 触发
        M2Config.nWeaponDCAddValueRate = 10;        // GetRandomRange 概率
        M2Config.nWeaponDCAddValueMaxLimit = 20;
        M2Config.nWeaponMCAddRate = 0;
        M2Config.nWeaponSCAddRate = 0;

        for (int i = 0; i < 50; i++)
        {
            var item = new TUserItem();
            item.wIndex = 42;
            item.Dura = 1000;
            item.DuraMax = 1000;
            unsafe { ItemUnit.RandomUpgradeWeapon(ref item); }
            Assert.InRange(item.btValue[0], 1, 20);
        }
    }

    [Fact]
    public void ItemUnit_RandomUpgradeDress_Bounded()
    {
        M2Config.nDressACAddRate = 1;
        M2Config.nDressACAddValueRate = 10;
        M2Config.nDressACAddValueMaxLimit = 15;
        M2Config.nDressMACAddRate = 0;
        M2Config.nDressDCAddRate = 0;
        M2Config.nDressMCAddRate = 0;
        M2Config.nDressSCAddRate = 0;

        for (int i = 0; i < 50; i++)
        {
            var item = new TUserItem();
            item.Dura = 500;
            item.DuraMax = 500;
            unsafe { ItemUnit.RandomUpgradeDress(ref item); }
            Assert.InRange(item.btValue[0], 1, 15);
            Assert.InRange(item.DuraMax, 500, 65000);
        }
    }

    [Fact]
    public void ItemUnit_GetRandomRange()
    {
        // GetRandomRange(count, rate)：rate 内随机 <1 时 +count 递推
        Assert.Equal(0, ItemUnit.GetRandomRange(0, 10));
        for (int i = 0; i < 100; i++)
        {
            int v = ItemUnit.GetRandomRange(5, 100);
            Assert.InRange(v, 0, 5);
        }
    }
}
