using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J5 前置：g_BaseAbilConfig 属性配置数据层（1:1 结构锁定）。</summary>
public sealed class BaseAbilConfigTests
{
    [Fact]
    public void UseDefault_DefaultsTrue_AndResetRestores()
    {
        Assert.True(M2ShareAbilConfig.g_BaseAbilConfig.UseDefault);
        M2ShareAbilConfig.g_BaseAbilConfig.UseDefault = false;
        M2ShareAbilConfig.ResetDefaults();
        Assert.True(M2ShareAbilConfig.g_BaseAbilConfig.UseDefault);
    }

    [Fact]
    public void Structure_JobsAnd1000Levels()
    {
        var cfg = new TBaseAbilConfig();
        Assert.Equal(3, cfg.HumAbil.Items.Length);   // JOB_WARR..JOB_TAOS
        Assert.Equal(3, cfg.HeroAbil.Items.Length);
        Assert.Equal(1000, cfg.HumAbil[0].Base.Length); // 0..999 级
        Assert.Equal(0u, cfg.HumAbil[2].Base[999].MaxHP);
        Assert.False(cfg.HeroAbil[1].AutoCalcLevel1000);
    }

    [Fact]
    public void BaseAbilInfo_FieldsWriteRead()
    {
        var info = new TBaseAbilInfo { DC1 = 3, DC2 = 8, MaxHP = 100, MaxWeight = 500 };
        Assert.Equal(3u, info.DC1);
        Assert.Equal(8u, info.DC2);
        Assert.Equal(100u, info.MaxHP);
        Assert.Equal(500, info.MaxWeight);
    }
}
