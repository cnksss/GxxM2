using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J8：RecalcAbilitys 组件（AddGroupItemValue 套装组值聚合 + AddAbilitysByCode 特戒开关）。</summary>
public sealed class RecalcAbilitysTests : IDisposable
{
    public RecalcAbilitysTests()
    {
        M2Config.ResetServerValueDefaults(); // btMaxAC=0 / btMaxHitPoint=1
    }

    public void Dispose()
    {
        M2Config.ResetServerValueDefaults();
    }

    private static TPlayObject NewPlayer() => new() { m_btJob = 0 };

    [Fact]
    public void AddGroupItemValue_AccumulatesAllFields()
    {
        var p = NewPlayer();
        p.m_wAbil.MaxHP = 100;
        p.m_wAbil.MaxMP = 50;
        p.m_wAbil.DC1 = 1;
        p.m_wAbil.DC2 = 8;

        p.AddGroupItemValue(65535, new[] { 10, 5, 1, 2, 3, 4, 5, 7, 9, 2, 3, 4, 5, 6, 2, 3, 4, 5, 6 });

        Assert.Equal(110u, p.m_wAbil.MaxHP);
        Assert.Equal(55u, p.m_wAbil.MaxMP);
        Assert.Equal(1, p.m_wAbil.AC1);
        Assert.Equal(2, p.m_wAbil.AC2);
        Assert.Equal(2, p.m_wAbil.MAC1);
        Assert.Equal(3, p.m_wAbil.MAC2);
        Assert.Equal(4, p.m_wAbil.DC1);
        Assert.Equal(12, p.m_wAbil.DC2);
        Assert.Equal(4, p.m_wAbil.MC1);
        Assert.Equal(5, p.m_wAbil.MC2);
        Assert.Equal(5, p.m_wAbil.SC1);
        Assert.Equal(6, p.m_wAbil.SC2);
        Assert.Equal(7, p.m_btHitPoint);     // 准确（基础 0 + 7）
        Assert.Equal(9, p.m_btSpeedPoint);   // 敏捷
        Assert.Equal(2, p.m_nAntiMagic);
        Assert.Equal(3, p.m_btAntiPoison);
        Assert.Equal(4, p.m_nPoisonRecover);
        Assert.Equal(5, p.m_nHealthRecover);
        Assert.Equal(6, p.m_nSpellRecover);
    }

    [Fact]
    public void AddGroupItemValue_ClampsMaxAcToWord_WhenBtMaxACZero()
    {
        var p = NewPlayer();
        p.m_wAbil.DC2 = 65000;
        p.AddGroupItemValue(65535, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1000, 0, 0 });
        Assert.Equal(65535, p.m_wAbil.DC2); // High(Word) 上限
    }

    [Fact]
    public void AddGroupItemValue_ClampsToHighInteger_WhenBtMaxACOne()
    {
        M2Config.btMaxAC = 1;
        var p = NewPlayer();
        p.m_wAbil.DC2 = 65000;
        p.AddGroupItemValue(65535, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1000, 0, 0 });
        Assert.Equal(66000, p.m_wAbil.DC2); // High(Integer) 上限，不截断
    }

    [Fact]
    public void AddGroupItemValue_HitPointCap_ByConfig()
    {
        var p = NewPlayer();
        p.m_btHitPoint = 250;
        p.m_btSpeedPoint = 250;

        // btMaxHitPoint=1（默认）→ Word 上限 65535
        p.AddGroupItemValue(65535, new[] { 0, 0, 0, 0, 0, 0, 0, 300, 300, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        Assert.Equal(550, p.m_btHitPoint);

        // btMaxHitPoint=0 → Byte 上限 255
        M2Config.btMaxHitPoint = 0;
        p.AddGroupItemValue(65535, new[] { 0, 0, 0, 0, 0, 0, 0, 300, 300, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        Assert.Equal(255, p.m_btHitPoint);
        Assert.Equal(255, p.m_btSpeedPoint);
    }

    [Fact]
    public void MaxHpClamp_ByNMaxValue()
    {
        var p = NewPlayer();
        p.m_wAbil.MaxHP = 60000;
        p.AddGroupItemValue(60100, new[] { 500, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        Assert.Equal(60100u, p.m_wAbil.MaxHP); // nMaxValue 钳制
    }

    [Fact]
    public void SpecialItemCodes_SetFlags()
    {
        var p = NewPlayer();
        p.ApplySpecialItemCode(111); // 隐身戒指
        Assert.True(p.m_boHideMode);

        p.ApplySpecialItemCode(112); // 传送
        Assert.True(p.m_boTeleport);

        p.ApplySpecialItemCode(113); // 麻痹
        Assert.True(p.m_boParalysis);

        p.ApplySpecialItemCode(118); // 魔法盾
        Assert.True(p.m_boMagicShield);

        p.ApplySpecialItemCode(139); // 防麻
        Assert.True(p.UnParalysis);

        p.ApplySpecialItemCode(141); // 经验
        Assert.True(p.m_boExpItem);

        p.ApplySpecialItemCode(150); // 麻痹护身（组合）
        Assert.True(p.m_boParalysis);
        Assert.True(p.m_boMagicShield);

        p.ApplySpecialItemCode(153); // 麻痹负载（组合）
        Assert.True(p.m_boMuscleRing);
    }

    [Fact]
    public void HideRing_ClearsTransparent()
    {
        var p = NewPlayer();
        p.m_boTransparent = true;
        p.ApplySpecialItemCode(111);
        Assert.True(p.m_boHideMode);
        Assert.False(p.m_boTransparent); // Delphi 修复分支 1:1
    }
}
