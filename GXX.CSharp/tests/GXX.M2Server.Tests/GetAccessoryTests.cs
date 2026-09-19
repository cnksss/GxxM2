using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using Xunit;
using TAddAbility = GXX.M2Server.Engine.TAddAbility;

namespace GXX.M2Server.Tests;

/// <summary>批次J9：ObjBase.pas TSmartObject.GetAccessory 1:1 移植测试（装备配件属性聚合）。</summary>
public sealed class GetAccessoryTests : IDisposable
{
    public GetAccessoryTests()
    {
        M2Config.ResetServerValueDefaults();
    }

    public void Dispose()
    {
        M2Config.ResetServerValueDefaults();
    }

    private static TStdItemView MakeStd(byte stdMode, int dc1 = 0, int dc2 = 0, int ac1 = 0, int ac2 = 0,
        int mac1 = 0, int mac2 = 0, int mc2 = 0, int sc2 = 0, uint hp = 0, uint mp = 0)
        => new()
        {
            StdMode = stdMode,
            DC1 = dc1,
            DC2 = dc2,
            AC1 = ac1,
            AC2 = ac2,
            MAC1 = mac1,
            MAC2 = mac2,
            MC2 = mc2,
            SC2 = sc2,
            HP = hp,
            MP = mp
        };

    [Fact]
    public void WeaponStdMode_AccumulatesDCAndSpeed()
    {
        var player = new TPlayObject { m_btRace = Grobal2Const.RC_PLAYOBJECT };
        var add = new TAddAbility();
        var all = new TAddAbility();
        var item = new TUserItemView();
        var std = MakeStd(5, dc1: 3, dc2: 7, ac1: 2, ac2: 4, mac1: 1, mac2: 12);
        GetAccessory.Apply(player, item, std, add, all);
        Assert.Equal(3, add.nDC1);
        Assert.Equal(7, add.nDC2);
        Assert.Equal(2, add.btLuck);    // AC1
        Assert.Equal(1, add.btUnLuck);  // MAC1
        Assert.Equal(2, add.nHitSpeed); // MAC2 - 10
    }

    [Fact]
    public void WeaponStdMode_SlowWeaponDecreasesSpeed()
    {
        var player = new TPlayObject { m_btRace = Grobal2Const.RC_PLAYOBJECT };
        var add = new TAddAbility();
        var all = new TAddAbility();
        var item = new TUserItemView();
        var std = MakeStd(5, mac2: 8); // MAC2 ≤ 10 → 减速
        GetAccessory.Apply(player, item, std, add, all);
        Assert.Equal(-8, add.nHitSpeed);
    }

    [Fact]
    public void NecklaceStdMode_LuckAndAntiMagic()
    {
        var player = new TPlayObject { m_btRace = Grobal2Const.RC_PLAYOBJECT };
        var add = new TAddAbility();
        var all = new TAddAbility();
        var item = new TUserItemView();
        var std = MakeStd(19, ac2: 5, mac1: 2, mac2: 3);
        GetAccessory.Apply(player, item, std, add, all);
        Assert.Equal(5, add.wAntiMagic);
        Assert.Equal(2, add.btUnLuck);
        Assert.Equal(3, add.btLuck);
        Assert.Equal(0, add.nHitSpeed); // 项链不计攻速
    }

    [Fact]
    public void BraceletStdMode_HitAndSpeed()
    {
        var player = new TPlayObject { m_btRace = Grobal2Const.RC_PLAYOBJECT };
        var add = new TAddAbility();
        var all = new TAddAbility();
        var item = new TUserItemView();
        var std = MakeStd(20, ac2: 6, mac2: 4);
        GetAccessory.Apply(player, item, std, add, all);
        Assert.Equal(6, add.wHitPoint);
        Assert.Equal(4, add.wSpeedPoint);
    }

    [Fact]
    public void BeltStdMode_HealthSpellRecover()
    {
        var player = new TPlayObject { m_btRace = Grobal2Const.RC_PLAYOBJECT };
        var add = new TAddAbility();
        var all = new TAddAbility();
        var item = new TUserItemView();
        var std = MakeStd(21, ac1: 2, ac2: 5, mac1: 1, mac2: 3);
        GetAccessory.Apply(player, item, std, add, all);
        Assert.Equal(5, add.wHealthRecover);
        Assert.Equal(3, add.wSpellRecover);
        Assert.Equal(1, add.nHitSpeed); // AC1 2 - MAC1 1 = 1
    }

    [Fact]
    public void DefaultStdMode_GenericACMAC()
    {
        var player = new TPlayObject { m_btRace = Grobal2Const.RC_PLAYOBJECT };
        var add = new TAddAbility();
        var all = new TAddAbility();
        var item = new TUserItemView();
        var std = MakeStd(10, dc2: 6, ac2: 9, mac2: 1); // 盔甲类走 else 通用分支
        GetAccessory.Apply(player, item, std, add, all);
        Assert.Equal(9, add.nAC2);
        Assert.Equal(1, add.nMAC2);
        Assert.Equal(6, add.nDC2);
    }

    [Fact]
    public void CommonAccumulation_HpMpAndDcMcSc()
    {
        var player = new TPlayObject { m_btRace = Grobal2Const.RC_PLAYOBJECT };
        var add = new TAddAbility();
        var all = new TAddAbility();
        var item = new TUserItemView();
        var std = MakeStd(15, dc2: 5, mc2: 6, sc2: 7, hp: 100, mp: 50);
        GetAccessory.Apply(player, item, std, add, all);
        Assert.Equal(100u, add.wHP);
        Assert.Equal(50u, add.wMP);
        Assert.Equal(5, add.nDC2);
        Assert.Equal(6, add.nMC2);
        Assert.Equal(7, add.nSC2);
    }

    [Fact]
    public void CustomProperty_BindType3DC2_FixedAndPercent()
    {
        var player = new TPlayObject { m_btRace = Grobal2Const.RC_PLAYOBJECT };
        var all = new TAddAbility();
        var std = MakeStd(10, dc2: 100);

        var add = new TAddAbility();
        var item = new TUserItemView();
        item.CustomProperties.Add((3, 0, 15));
        GetAccessory.Apply(player, item, std, add, all);
        Assert.Equal(115, add.nDC2); // 固定值

        var add2 = new TAddAbility();
        var item2 = new TUserItemView();
        item2.CustomProperties.Add((3, 1, 50));
        GetAccessory.Apply(player, item2, std, add2, all);
        Assert.Equal(150, add2.nDC2); // 本级百分比 100/100*50

        var item3 = new TUserItemView();
        item3.CustomProperties.Add((3, 2, 30));
        GetAccessory.Apply(player, item3, std, add2, all);
        Assert.Equal(30, all.nDC2); // 全件百分比 → AllAddAbility
    }

    [Fact]
    public void NewValueStdMode53_RespectsConfigSwitch()
    {
        var player = new TPlayObject { m_btRace = Grobal2Const.RC_PLAYOBJECT };
        var all = new TAddAbility();
        var item = new TUserItemView();
        var std = MakeStd(53, ac1: 4, ac2: 5);

        M2Config.boAddUserItemNewValue = true;
        var add = new TAddAbility();
        GetAccessory.Apply(player, item, std, add, all);
        Assert.Equal(4, add.nAC1);
        Assert.Equal(5, add.nAC2);

        M2Config.boAddUserItemNewValue = false;
        var add2 = new TAddAbility();
        GetAccessory.Apply(player, item, std, add2, all);
        Assert.Equal(5, add2.wAntiMagic); // 旧路径 AC2 → 魔躲
        Assert.Equal(0, add2.nAC1);
    }
}
