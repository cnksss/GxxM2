using GXX.Core.Protocol;
using GXX.Core.Util;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>批次J7：千级经验默认表 + LoadExp 装载 + GetLevelExp 三路分派测试。</summary>
public sealed class ExpTableTests : IDisposable
{
    private readonly string _dir;

    public ExpTableTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "gxx_j7_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        M2ShareState.ResetForTests(_dir);
        M2Config.ResetServerValueDefaults();
    }

    public void Dispose()
    {
        M2ShareState.ResetForTests(null);
        try { Directory.Delete(_dir, true); } catch { /* best effort */ }
    }

    [Fact]
    public void DefaultTables_MatchDelphiConstants()
    {
        // M2Share.pas g_dwOldNeedExps 前四级与 46-48 级边界（typed constant 1:1）
        Assert.Equal(100u, M2Config.OldNeedExps[1]);
        Assert.Equal(600u, M2Config.OldNeedExps[5]);
        Assert.Equal(480000000u, M2Config.OldNeedExps[46]);
        Assert.Equal(1000000000u, M2Config.OldNeedExps[47]);
        Assert.Equal(3000000000u, M2Config.OldNeedExps[48]);
        Assert.Equal(4200000000u, M2Config.OldNeedExps[49]);
        Assert.Equal(4200000000u, M2Config.OldNeedExps[1000]);
        Assert.Equal(100u, M2Config.OldHeroNeedExps[1]);
        Assert.Equal(0u, M2Config.OldNeedExps[0]); // 索引 0 未用
    }

    [Fact]
    public void LoadExp_DefaultFillsFromOldTables_AndPersistsIni()
    {
        var iniPath = Path.Combine(_dir, "ExpConfig.txt");
        var ini = new TFastIniFile(iniPath);
        M2Config.LoadExp(ini);

        Assert.Equal(100u, M2Config.dwNeedExps[1]);
        Assert.Equal(4200000000u, M2Config.dwNeedExps[1000]);
        Assert.Equal(100u, M2Config.dwHeroNeedExps[1]);
        Assert.Equal(100u, M2Config.dwPetNeedExps[1]); // Delphi 原文：宠物缺省落英雄表

        var persisted = new TFastIniFile(iniPath);
        Assert.Equal("100", persisted.ReadString("Exp", "Level1", ""));
        Assert.Equal("4200000000", persisted.ReadString("Exp", "Level1000", ""));
        Assert.Equal("100", persisted.ReadString("HeroExp", "Level1", ""));
    }

    [Fact]
    public void LoadExp_CustomLevelValue_OverridesTable()
    {
        var iniPath = Path.Combine(_dir, "ExpConfig.txt");
        var ini = new TFastIniFile(iniPath);
        ini.WriteString("Exp", "Level1", "555");
        ini.UpdateFile();

        M2Config.LoadExp(new TFastIniFile(iniPath));
        Assert.Equal(555u, M2Config.dwNeedExps[1]);
        Assert.Equal(200u, M2Config.dwNeedExps[2]); // 其余仍走缺省表
    }

    [Fact]
    public void GetLevelExp_ThreePathsAndOver1000Extrapolation()
    {
        M2Config.LoadExp(new TFastIniFile(Path.Combine(_dir, "ExpConfig.txt")));

        var player = new TPlayObject { m_btRace = Grobal2Const.RC_PLAYOBJECT };
        Assert.Equal(100u, AbilRecalc.GetLevelExp(player, 1));
        Assert.Equal(200u, AbilRecalc.GetLevelExp(player, 2));

        // 超千级固定经验：尾部表值 + 超出级数 × 1000 万
        var over = AbilRecalc.GetLevelExp(player, 1002);
        Assert.Equal(4200000000u + 2u * 10000000u, over);

        // 英雄路
        var hero = new TPlayObject { m_btRace = Grobal2Const.RC_HEROOBJECT };
        Assert.Equal(100u, AbilRecalc.GetLevelExp(hero, 1));

        // 宠物路（缺省表 = 英雄表）
        var pet = new TPlayObject { m_btRace = Grobal2Const.RC_PLAYOBJECT, m_boGamePet = true };
        Assert.Equal(100u, AbilRecalc.GetLevelExp(pet, 1));
    }
}
