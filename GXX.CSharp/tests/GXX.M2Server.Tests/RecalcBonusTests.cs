using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J11：RecalcAbilitys 主循环扩展分支测试（GetItemAddValue/RecalcAdjusBonus/宠物加成）。</summary>
public sealed class RecalcBonusTests : IDisposable
{
    public RecalcBonusTests()
    {
        M2Config.ResetServerValueDefaults();
        M2ShareAbilConfig.ResetDefaults();
        ResetAbilCfg();
    }

    public void Dispose()
    {
        M2ShareAbilConfig.ResetDefaults();
        M2Config.ResetServerValueDefaults();
        ResetAbilCfg();
    }

    private static void ResetAbilCfg()
    {
        foreach (var b in new[] { M2Config.BonusAbilofWarr, M2Config.BonusAbilofWizard, M2Config.BonusAbilofTaos })
        {
            b.DC = b.MC = b.SC = b.AC = b.MAC = 0;
            b.HP = b.MP = 0;
        }
        foreach (var n in new[] { M2Config.NakedAbilofWarr, M2Config.NakedAbilofWizard, M2Config.NakedAbilofTaos })
        {
            n.DC = n.MC = n.SC = n.AC = n.MAC = 0;
            n.HP = n.MP = 0;
        }
    }

    private static TUserItemView Item(params byte[] btValue)
    {
        var item = new TUserItemView { wIndex = 1 };
        for (int i = 0; i < btValue.Length; i++)
            item.BtValue[i] = btValue[i];
        return item;
    }

    [Fact]
    public void GetItemAddValue_Weapon_UpgradeValues()
    {
        var std = new TStdItemView { StdMode = 5, DC1 = 2, DC2 = 9, MC2 = 8, AC1 = 1, AC2 = 3, MAC1 = 2, MAC2 = 12 };
        var item = Item(7, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        // btValue[0]=7 → DC2+7；btValue[9]=1 → DC1+1；btValue[4]=0 → 幸运+0；btValue[6]=0 → 攻速不变

        RecalcBonus.GetItemAddValue(item, std);
        Assert.Equal(3, std.DC1);
        Assert.Equal(16, std.DC2);
        Assert.Equal(12, std.MAC2); // 攻速不变（btValue[6]=0）
    }

    [Fact]
    public void GetItemAddValue_Weapon_SpeedBranches()
    {
        // Mac2=0 → 10 + btValue[6]
        var std1 = new TStdItemView { StdMode = 5, MAC2 = 0 };
        RecalcBonus.GetItemAddValue(Item(0, 0, 0, 0, 0, 0, 5), std1);
        Assert.Equal(15, std1.MAC2);

        // Mac2=8 (<10) → 8 - 6 = 2 → <0? 否 → 2
        var std2 = new TStdItemView { StdMode = 5, MAC2 = 8 };
        RecalcBonus.GetItemAddValue(Item(0, 0, 0, 0, 0, 0, 6), std2);
        Assert.Equal(2, std2.MAC2);

        // Mac2=8 (<10) → 8 - 12 = -4 → 10 + 4 = 14
        var std3 = new TStdItemView { StdMode = 5, MAC2 = 8 };
        RecalcBonus.GetItemAddValue(Item(0, 0, 0, 0, 0, 0, 12), std3);
        Assert.Equal(14, std3.MAC2);

        // Mac2=15 (>=10) → 15 + 3
        var std4 = new TStdItemView { StdMode = 5, MAC2 = 15 };
        RecalcBonus.GetItemAddValue(Item(0, 0, 0, 0, 0, 0, 3), std4);
        Assert.Equal(18, std4.MAC2);
    }

    [Fact]
    public void GetItemAddValue_ArmorStdModes()
    {
        var std = new TStdItemView { StdMode = 10, DC2 = 5, AC2 = 2 };
        var item = Item(6, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        RecalcBonus.GetItemAddValue(item, std);
        Assert.Equal(8, std.AC2);  // btValue[0]
        Assert.Equal(8, std.DC2);  // btValue[2]
    }

    [Fact]
    public void RecalcAdjusBonus_DistributesPointsToLoAndHi()
    {
        var player = new TPlayObject { m_btJob = 0, m_wAbil = { Level = 1 } };
        // 战士裸体成长 DC=0x00010001（1/1），加点 4 点 DC → adc=4 → 全部进上限（lo+1 < hi 不成立）
        M2Config.NakedAbilofWarr.DC = 0x00010001;
        M2Config.BonusAbilofWarr.DC = 1; // BonusTick：每 1 点
        player.m_BonusAbil.DC = 4;

        player.RecalcAdjusBonus();
        Assert.Equal(1, player.m_wAbil.DC1); // lo/hi 交替：4 点 = ldc 1 + hdc 3
        Assert.Equal(3, player.m_wAbil.DC2);
        // 基础 DC2 = max(1, 1 div 5)=1（RecalcLevelAbilitys 已在链外）→ 此处只验证加点增量
    }

    [Fact]
    public void RecalcAdjusBonus_LoFirst_WhenGapExists()
    {
        var player = new TPlayObject { m_btJob = 1 };
        // 法师裸体 MC = lo=1, hi=5（0x00050001）→ 有间隙，加点优先补下限
        M2Config.NakedAbilofWizard.MC = 0x00050001;
        M2Config.BonusAbilofWizard.MC = 1; // BonusTick：每 1 点
        player.m_BonusAbil.MC = 2;
        player.m_wAbil.MC1 = 1;
        player.m_wAbil.MC2 = 5;

        player.RecalcAdjusBonus();
        // amc = 2/1 = 2 点：lo(1)+1<hi(5) → 第1点进 lo（1→2），第2点 2+1<5 → lo（2→3）
        Assert.Equal(1 + 2, player.m_wAbil.MC1); // amc = 2/1 = 2 点，lo 1→3
        Assert.Equal(5, player.m_wAbil.MC2);
    }

    [Fact]
    public void ApplyPetBonus_HPAndDC_ByRate()
    {
        M2Config.nPetAbilToMasterRate = 10;
        M2Config.boPetHPToMaster = true;
        M2Config.boPetDCToMaster = true;

        var master = new TPlayObject();
        master.m_wAbil.MaxHP = 1000;
        master.m_wAbil.DC1 = 10;
        master.m_wAbil.DC2 = 50;
        var pet = new TPlayObject();
        pet.m_wAbil.MaxHP = 500;
        pet.m_wAbil.DC1 = 20;
        pet.m_wAbil.DC2 = 60;

        master.ApplyPetBonus(pet);
        Assert.Equal(1050u, master.m_wAbil.MaxHP);  // 1000 + 500/100*10
        Assert.Equal(12, master.m_wAbil.DC1);       // 10 + 20/100*10
        Assert.Equal(56, master.m_wAbil.DC2);       // 50 + 60/100*10
    }

    [Fact]
    public void ApplyPetBonus_ZeroRate_Noop()
    {
        var master = new TPlayObject { m_MyGamePet = new TPlayObject() };
        master.m_wAbil.MaxHP = 100;
        master.ApplyPetBonus(master.m_MyGamePet);
        Assert.Equal(100u, master.m_wAbil.MaxHP);
    }
}
