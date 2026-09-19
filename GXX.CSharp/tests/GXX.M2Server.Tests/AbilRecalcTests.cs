using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J5正片：ObjBase.pas TBaseObject.RecalcLevelAbilitys 1:1 移植测试。
/// 手工按 Delphi 银行家舍入推导向量（btMaxLevel=0 / 默认 nLevelValueOf* 参数）。
/// </summary>
public sealed class AbilRecalcTests : IDisposable
{
    public AbilRecalcTests()
    {
        M2ShareAbilConfig.ResetDefaults();
        M2Config.ResetServerValueDefaults();
    }

    public void Dispose()
    {
        M2ShareAbilConfig.ResetDefaults();
        M2Config.ResetServerValueDefaults();
    }

    private static TPlayObject NewPlayer(byte job, uint level)
        => new() { m_btJob = job, m_wAbil = { Level = level } };

    [Fact]
    public void Warrior_Level1_DefaultFormulas()
    {
        var p = NewPlayer(0, 1);
        p.RecalcLevelAbilitys(false);
        // MaxHP = 14 + Round(0.25+4.5+0.05) = 19
        Assert.Equal(19u, p.m_wAbil.MaxHP);
        // MaxMP = 11 + Round(3.5) = 11 + 4（银行家舍入 .5→偶）
        Assert.Equal(15u, p.m_wAbil.MaxMP);
        // MaxWeight = 50 + Round(0.25) = 50
        Assert.Equal(50, p.m_wAbil.MaxWeight);
        Assert.Equal(15, p.m_wAbil.MaxWearWeight);
        Assert.Equal(12, p.m_wAbil.MaxHandWeight);
        Assert.Equal(0, p.m_wAbil.DC1);
        Assert.Equal(1, p.m_wAbil.DC2);   // max(1, 1 div 5)
        Assert.Equal(0, p.m_wAbil.AC1);
        Assert.Equal(0, p.m_wAbil.AC2);   // 1 div 7
    }

    [Fact]
    public void Warrior_Level7_Divisions()
    {
        var p = NewPlayer(0, 7);
        p.RecalcLevelAbilitys(false);
        // MaxHP = 14 + Round((7/4 + 4.5 + 7/20)*7) = 14 + Round((1.75+4.5+0.35)*7) = 14 + Round(46.2) = 60
        Assert.Equal(60u, p.m_wAbil.MaxHP);
        // DC1 = 7 div 5 - 1 = 0；DC2 = max(1,1)；AC2 = 7 div 7 = 1
        Assert.Equal(0, p.m_wAbil.DC1);
        Assert.Equal(1, p.m_wAbil.DC2);
        Assert.Equal(1, p.m_wAbil.AC2);
    }

    [Fact]
    public void Taos_Level7_Formulas()
    {
        var p = NewPlayer(2, 7);
        p.RecalcLevelAbilitys(false);
        // MaxHP = 14 + Round((7/6 + 2.5)*7) = 14 + Round(25.6667) = 40
        Assert.Equal(40u, p.m_wAbil.MaxHP);
        // MaxMP = 13 + Round((7/8)*2.2*7) = 13 + Round(13.475) = 13 + 13 = 26
        Assert.Equal(26u, p.m_wAbil.MaxMP);
        // n = 7 div 7 = 1 → DC1/SC1 = max(0,0)=0；DC2/SC2 = 1
        Assert.Equal(0, p.m_wAbil.DC1);
        Assert.Equal(1, p.m_wAbil.DC2);
        Assert.Equal(0, p.m_wAbil.SC1);
        Assert.Equal(1, p.m_wAbil.SC2);
        Assert.Equal(0, p.m_wAbil.MC1);
        // MAC：n = Round(7/6) = 1 → MAC1 = 0；MAC2 = 2
        Assert.Equal(0, p.m_wAbil.MAC1);
        Assert.Equal(2, p.m_wAbil.MAC2);
        // MaxWeight = 50 + Round(1.75*7) = 62
        Assert.Equal(62, p.m_wAbil.MaxWeight);
    }

    [Fact]
    public void Wizard_Level5_Formulas()
    {
        var p = NewPlayer(1, 5);
        p.RecalcLevelAbilitys(false);
        // MaxHP = 14 + Round((5/15 + 1.8)*5) = 14 + Round(10.6667) = 25
        Assert.Equal(25u, p.m_wAbil.MaxHP);
        // MaxMP = 13 + Round((1+2)*2.2*5) = 13 + 33 = 46
        Assert.Equal(46u, p.m_wAbil.MaxMP);
        // n = 0 → DC1/DC2/MC1/MC2 同道士
        Assert.Equal(0, p.m_wAbil.DC1);
        Assert.Equal(1, p.m_wAbil.DC2);
        Assert.Equal(0, p.m_wAbil.SC1);
        Assert.Equal(0, p.m_wAbil.MAC1);
    }

    [Fact]
    public void CustomTable_LookupWithin1000()
    {
        M2ShareAbilConfig.g_BaseAbilConfig.UseDefault = false;
        M2ShareAbilConfig.g_BaseAbilConfig.HumAbil[0].Base[41] = new TBaseAbilInfo
        {
            DC1 = 5, DC2 = 9, MaxHP = 777, MaxWeight = 800
        };
        var p = NewPlayer(0, 42); // 级 42 → Base[41]
        p.RecalcLevelAbilitys(false);
        Assert.Equal(777u, p.m_wAbil.MaxHP);
        Assert.Equal(5, p.m_wAbil.DC1);
        Assert.Equal(9, p.m_wAbil.DC2);
        Assert.Equal(800, p.m_wAbil.MaxWeight);
    }

    [Fact]
    public void CustomTable_Over1000_WithoutAutoCalc_Extrapolates()
    {
        var cfg = M2ShareAbilConfig.g_BaseAbilConfig;
        cfg.UseDefault = false;
        cfg.HumAbil[1].Base[999] = new TBaseAbilInfo { MaxHP = 5000, DC2 = 100 };
        cfg.HumAbil[1].Add = new TBaseAbilInfo { MaxHP = 20, DC2 = 1 };
        cfg.HumAbil[1].AutoCalcLevel1000 = false;

        var p = NewPlayer(1, 1105); // nLevelSub = 1105-1000 = 105
        p.RecalcLevelAbilitys(false);
        Assert.Equal(5000u + 20u * 105, p.m_wAbil.MaxHP);
        Assert.Equal(100 + 1 * 105, p.m_wAbil.DC2);
    }

    [Fact]
    public void CustomTable_Over1000_WithAutoCalc_FallsBackToSystemFormula()
    {
        var cfg = M2ShareAbilConfig.g_BaseAbilConfig;
        cfg.UseDefault = false;
        cfg.HumAbil[2].AutoCalcLevel1000 = true;
        var p = NewPlayer(2, 1200); // 道士超 1000 且 AutoCalc → 系统公式
        p.RecalcLevelAbilitys(false);
        // 公式值 = 14 + Round((1200/6 + 2.5)*1200) = 243014，超 nMaxValue=65535 → Min 钳到 65535
        Assert.Equal(65535u, p.m_wAbil.MaxHP);
        // 超 1000 → 重量上限 High(Word)
        Assert.Equal(65535, p.m_wAbil.MaxWeight);
    }

    [Fact]
    public void TailClamp_MinHpMp_AndHpOverMax()
    {
        // 极低等级公式兜底不触发时：自定义表全 0 → MaxHP 落 15
        var cfg = M2ShareAbilConfig.g_BaseAbilConfig;
        cfg.UseDefault = false;
        var p = NewPlayer(0, 42);
        p.m_wAbil.HP = 999;
        p.m_wAbil.MP = 999;
        p.RecalcLevelAbilitys(false);
        Assert.Equal(15u, p.m_wAbil.MaxHP); // 全 0 表 → <=0 兜底 15
        Assert.Equal(15u, p.m_wAbil.MaxMP);
        Assert.Equal(15u, p.m_wAbil.HP);    // clamp
        Assert.Equal(15u, p.m_wAbil.MP);
    }

    [Fact]
    public void GamePet_SkipsRecalc()
    {
        var p = NewPlayer(0, 1);
        p.m_boGamePet = true;
        p.RecalcLevelAbilitys(false);
        Assert.Equal(0u, p.m_wAbil.MaxHP); // 未被改动
    }
}
