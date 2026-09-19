using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J10：RecalcAbilitys 主循环整合测试（等级基础 → 装备遍历 → 并入 → clamp 整链）。</summary>
public sealed class RecalcChainTests : IDisposable
{
    public RecalcChainTests()
    {
        M2Config.ResetServerValueDefaults();
        M2ShareAbilConfig.ResetDefaults();
        // 跨类静态残留隔离：清空属性点/宠物加成与成长配置（其它批次测试会修改）
        M2Config.NakedAbilofWarr.HP = 0;
        M2Config.NakedAbilofWarr.MP = 0;
        M2Config.BonusAbilofWarr.HP = 0;
        M2Config.BonusAbilofWarr.MP = 0;
        M2Config.NakedAbilofWarr.DC = 0;
        M2Config.BonusAbilofWarr.DC = 0;
        M2Config.nPetAbilToMasterRate = 0;
        M2Config.boPetHPToMaster = false;
        M2Config.boPetDCToMaster = false;
    }

    public void Dispose()
    {
        M2ShareAbilConfig.ResetDefaults();
        M2Config.ResetServerValueDefaults();
    }

    private static TStdItemView Std(byte stdMode, ushort index, int dc1 = 0, int dc2 = 0, int ac1 = 0, int ac2 = 0, uint hp = 0)
        => new()
        {
            StdMode = stdMode,
            DC1 = dc1,
            DC2 = dc2,
            AC1 = ac1,
            AC2 = ac2,
            HP = hp
        };

    [Fact]
    public void Chain_LevelBasePlusEquipment()
    {
        var player = new TPlayObject { m_btJob = 0, m_wAbil = { Level = 1 } }; // 战士 1 级：MaxHP 19 DC2 1

        // 武器（U_WEAPON）+ 衣服（U_DRESS）装备
        player.StdItemResolver = idx => idx switch
        {
            1 => Std(5, 1, dc1: 2, dc2: 9),   // 武器 DC 2-9
            2 => Std(10, 2, dc2: 3, ac2: 4),  // 衣服 DC2+3 AC2+4
            _ => null
        };
        player.m_UseItems[UseSlots.U_WEAPON] = new TUserItemView { wIndex = 1 };
        player.m_UseItems[UseSlots.U_DRESS] = new TUserItemView { wIndex = 2 };

        player.RecalcAbilitys();

        // 等级基础：DC1=0/DC2=1；装备：DC1+2/DC2+9+3
        Assert.Equal(2, player.m_AddAbil.nDC1);
        Assert.Equal(2, player.m_wAbil.DC1);
        Assert.Equal(13, player.m_wAbil.DC2);
        Assert.Equal(4, player.m_wAbil.AC2);
        Assert.Equal(0, player.m_wAbil.AC1);
        Assert.Equal(19u, player.m_wAbil.MaxHP); // 等级基础 MaxHP，装备无 HP 加成
    }

    [Fact]
    public void Chain_EquipmentHpAddsToMaxHp()
    {
        var player = new TPlayObject { m_btJob = 0, m_wAbil = { Level = 1 } };
        player.StdItemResolver = idx => idx switch
        {
            5 => Std(15, 5, hp: 50),   // 头盔 +50 HP（通用 StdMode）
            6 => Std(10, 6, hp: 30),   // 衣服 +30
            _ => null
        };
        player.m_UseItems[UseSlots.U_HELMET] = new TUserItemView { wIndex = 5 };
        player.m_UseItems[UseSlots.U_DRESS] = new TUserItemView { wIndex = 6 };

        player.RecalcAbilitys();
        Assert.Equal(19u + 50u + 30u, player.m_wAbil.MaxHP);
    }

    [Fact]
    public void Chain_EmptyEquipment_KeepsLevelBase()
    {
        var player = new TPlayObject { m_btJob = 0, m_wAbil = { Level = 1 } };
        player.RecalcAbilitys();
        Assert.Equal(19u, player.m_wAbil.MaxHP);
        Assert.Equal(1, player.m_wAbil.DC2);
        Assert.Equal(0, player.m_wAbil.DC1);
    }

    [Fact]
    public void Chain_HpClamp_WhenCurrentExceedsMax()
    {
        var player = new TPlayObject { m_btJob = 0, m_wAbil = { Level = 1, HP = 999, MP = 999 } };
        player.RecalcAbilitys();
        Assert.Equal(19u, player.m_wAbil.HP);  // clamp 到 MaxHP
        Assert.Equal(15u, player.m_wAbil.MP);
    }

    [Fact]
    public void Chain_SpecialRing_TriggersFlag()
    {
        var player = new TPlayObject { m_btJob = 0, m_wAbil = { Level = 1 } };
        player.StdItemResolver = idx => idx switch
        {
            _ => null
        };
        var ringStd = Std(22, 9);
        ringStd.Shape = 111;
        player.StdItemResolver = _ => ringStd;
        player.m_UseItems[UseSlots.U_RINGL] = new TUserItemView { wIndex = 9 };

        player.RecalcAbilitys();
        Assert.True(player.m_boHideMode); // AddAbilitysByCode 在遍历内触发
    }

    [Fact]
    public void Chain_NecklaceLuck_Route()
    {
        var player = new TPlayObject { m_btJob = 0, m_wAbil = { Level = 1 } };
        player.StdItemResolver = _ =>
        {
            var s = Std(19, 3, ac2: 5);
            s.MAC2 = 3;
            return s;
        };
        player.m_UseItems[UseSlots.U_NECKLACE] = new TUserItemView { wIndex = 3 };

        player.RecalcAbilitys();
        Assert.Equal(5, player.m_AddAbil.wAntiMagic);
        Assert.Equal(3, player.m_AddAbil.btLuck);
    }
}
