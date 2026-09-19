using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>Magic.pas 批次H：灭天火/流星火雨/捆魔咒/禁锢/自定义技能配置驱动框架。</summary>
public class MagicBatchHTests : IDisposable
{
    private readonly TEnvirnoment _env = MakeEnv(48, 48);

    public void Dispose() => M2Config.ResetMagicExDefaults();

    private static TEnvirnoment MakeEnv(int w, int h)
    {
        var env = new TEnvirnoment
        {
            nWidth = w,
            nHeight = h,
            MapCellArray = new TMapCellinfo[w, h],
            MapData = new TMapUnitInfo[w, h]
        };
        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                env.MapCellArray[x, y] = new TMapCellinfo();
        return env;
    }

    private TSpellCaster MakeCaster(int x = 10, int y = 10, int level = 40, byte job = 1)
    {
        var c = new TSpellCaster
        {
            m_PEnvir = _env,
            m_nCurrX = x,
            m_nCurrY = y,
            m_btRaceServer = Grobal2Const.RC_PLAYOBJECT,
            m_btJob = job
        };
        c.m_wAbil.Level = (uint)level;
        c.m_wAbil.MC1 = 20;
        c.m_wAbil.MC2 = 40;
        c.m_wAbil.HP = 500;
        c.m_wAbil.MaxHP = 500;
        c.m_wAbil.MP = 500;
        c.m_wAbil.MaxMP = 500;
        return c;
    }

    private TSpellTarget MakeMonster(int x, int y, int hp = 100000, byte lifeAttrib = 0, int level = 20)
    {
        var m = new TSpellTarget
        {
            m_PEnvir = _env,
            m_nCurrX = x,
            m_nCurrY = y,
            m_btRaceServer = Grobal2Const.RC_MONSTER,
            m_btLifeAttrib = lifeAttrib
        };
        m.m_wAbil.Level = (uint)level;
        m.m_wAbil.HP = (uint)hp;
        m.m_wAbil.MaxHP = (uint)hp;
        m.m_wAbil.MP = (uint)hp;
        m.m_wAbil.MaxMP = (uint)hp;
        return m;
    }

    private static TUserMagicRef MakeUserMagic(int magicId, int power = 100, int maxPower = 100, int trainLv = 3)
    {
        return new TUserMagicRef
        {
            wMagIdx = (ushort)magicId,
            btLevel = 3,
            btNewLevel = 0,
            MagicInfo = new TMagicDef
            {
                wMagicId = (ushort)magicId,
                wPower = (ushort)power,
                wMaxPower = (ushort)maxPower,
                btTrainLv = (byte)trainLv
            }
        };
    }

    // ================= 灭天火（SKILL_45） =================

    [Fact]
    public void FireDay_DamagesTarget_AndBurnsMp()
    {
        M2Config.boPlayObjectReduceMP = true;
        M2Config.nMakeFireDayPowerRate = 100;
        var caster = MakeCaster();
        var target = MakeMonster(12, 10);
        _env.AddToMap(12, 10, target);

        var mgr = new Magic();
        TCreature? t = target;
        bool ok = mgr.MagMakeFireDay(caster, MakeUserMagic(MagicConst.SKILL_45), 12, 10, ref t);
        Assert.True(ok);
        Assert.True(target.m_wAbil.HP < 100000, "灭天火应扣血");
        Assert.True(target.m_wAbil.MP < 100000, "boPlayObjectReduceMP 开启时同步扣蓝");
    }

    [Fact]
    public void FireDay_UndeadBonus()
    {
        M2Config.boPlayObjectReduceMP = false;
        M2Config.nMakeFireDayPowerRate = 100;
        var caster = MakeCaster();
        // 不死系比普通怪掉血更多（1.5x）——用两个同血量怪对比
        var normal = MakeMonster(12, 10, hp: 1000000);
        var undead = MakeMonster(14, 10, hp: 1000000, lifeAttrib: Grobal2Const.LA_UNDEAD);
        _env.AddToMap(12, 10, normal);
        _env.AddToMap(14, 10, undead);

        var mgr = new Magic();
        TCreature? t1 = normal;
        TCreature? t2 = undead;
        mgr.MagMakeFireDay(caster, MakeUserMagic(MagicConst.SKILL_45), 12, 10, ref t1);
        mgr.MagMakeFireDay(caster, MakeUserMagic(MagicConst.SKILL_45), 14, 10, ref t2);
        long normalLost = 1000000 - normal.m_wAbil.HP;
        long undeadLost = 1000000 - undead.m_wAbil.HP;
        Assert.True(undeadLost > normalLost, "不死系受 1.5 倍伤害");
    }

    // ================= 流星火雨（SKILL_58） =================

    [Fact]
    public void MeteoriteRain_RangeHit()
    {
        M2Config.nSkill58AttackRange = 3;
        M2Config.nSkill58PowerRate = 100;
        M2Config.boSkill58PowerTwoAttack = false;
        var caster = MakeCaster();
        var inRange1 = MakeMonster(12, 10);
        var inRange2 = MakeMonster(10, 12);
        var outRange = MakeMonster(40, 40);
        foreach (var m in new[] { inRange1, inRange2, outRange })
            _env.AddToMap(m.m_nCurrX, m.m_nCurrY, m);

        var mgr = new Magic();
        bool ok = mgr.MagMeteoriteRain(caster, MakeUserMagic(MagicConst.SKILL_58), 11, 11);
        Assert.True(ok);
        Assert.True(inRange1.m_wAbil.HP < 100000);
        Assert.True(inRange2.m_wAbil.HP < 100000);
        Assert.Equal(100000u, outRange.m_wAbil.HP);
    }

    // ================= 捆魔咒（HolyCurtain） =================

    [Fact]
    public void HolyCurtain_CapturesLowLevelMonsters()
    {
        var caster = MakeCaster(level: 50);
        var lowMon = MakeMonster(12, 10, level: 20);
        _env.AddToMap(12, 10, lowMon);

        var mgr = new Magic();
        int captured = mgr.MagMakeHolyCurtain(caster, 5, 12, 10);
        Assert.Equal(1, captured);
        Assert.True(lowMon.m_boHolySeize, "低等级怪应被捆魔定身");
        Assert.True(lowMon.m_dwHolySeizeTime > 0);
    }

    [Fact]
    public void HolyCurtain_SkipsHighLevel()
    {
        var caster = MakeCaster(level: 20);
        var highMon = MakeMonster(12, 10, level: 50);
        _env.AddToMap(12, 10, highMon);

        var mgr = new Magic();
        int captured = mgr.MagMakeHolyCurtain(caster, 5, 12, 10);
        Assert.Equal(0, captured);
        Assert.False(highMon.m_boHolySeize);
    }

    // ================= 禁锢（SKILL_69） =================

    [Fact]
    public void Imprison_LocksLowerLevelTargets()
    {
        M2Config.boSkill69SameLevel = true;
        var caster = MakeCaster(level: 50);
        var lowMon = MakeMonster(12, 10, level: 20);
        _env.AddToMap(12, 10, lowMon);

        var mgr = new Magic();
        int count = mgr.MagMakeImprison(caster, 10, 2, 12, 10);
        Assert.Equal(1, count);
        Assert.True(lowMon.m_boImprison);
        Assert.Equal(10000u, lowMon.m_dwImprisonTime); // 10 秒 × 1000
    }

    [Fact]
    public void Imprison_SkipsHigherLevel()
    {
        M2Config.boSkill69SameLevel = true;
        var caster = MakeCaster(level: 20);
        var highMon = MakeMonster(12, 10, level: 50);
        _env.AddToMap(12, 10, highMon);

        var mgr = new Magic();
        int count = mgr.MagMakeImprison(caster, 10, 2, 12, 10);
        Assert.Equal(0, count);
        Assert.False(highMon.m_boImprison);
    }

    // ================= 自定义技能（MagCustomSkill 配置驱动） =================

    [Fact]
    public void CustomSkill_ConfigDriven_GroupDamage_Heal()
    {
        // 注册自定义技能 5001：组队攻击 + 给自己回血
        var cfg = new TCustomMagicConfig(5001)
        {
            DisableInSafeZone = false,
            TargetPattern = CustomTargetPattern.GroupAroundTarget,
            AttackRange = 2,
            PowerRate = 150,
            AddHP = 100,
            AddMP = 50
        };
        M2Config.RegisterCustomMagic(cfg);

        var caster = MakeCaster();
        caster.m_wAbil.HP = 100;
        caster.m_wAbil.MP = 100;
        var m1 = MakeMonster(12, 10);
        var m2 = MakeMonster(12, 12);
        _env.AddToMap(12, 10, m1);
        _env.AddToMap(12, 12, m2);

        var mgr = new Magic();
        string msg = "";
        bool moved = false, spellFire = false;
        TCreature? target = m1;
        bool ok = mgr.MagCustomSkillFull(caster, MakeUserMagic(5001), 12, 11, ref target, ref msg, ref moved, 0, ref spellFire);
        Assert.True(ok);
        Assert.True(m1.m_wAbil.HP < 100000, "组队范围内目标受击");
        Assert.Equal(200u, caster.m_wAbil.HP);  // AddHP=100
        Assert.Equal(150u, caster.m_wAbil.MP);  // AddMP=50
    }

    [Fact]
    public void CustomSkill_SafeZoneDisabled()
    {
        var cfg = new TCustomMagicConfig(5002) { DisableInSafeZone = true };
        M2Config.RegisterCustomMagic(cfg);

        var caster = MakeCaster();
        caster.m_boInSafeZone = true; // 处于安全区
        var mgr = new Magic();
        string msg = "";
        bool moved = false, spellFire = false;
        TCreature? target = null;
        Assert.False(mgr.MagCustomSkillFull(caster, MakeUserMagic(5002), 12, 10, ref target, ref msg, ref moved, 0, ref spellFire));
        Assert.False(spellFire);
        Assert.Contains("安全区", msg);
    }

    [Fact]
    public void CustomSkill_Unregistered_ReturnsFalse()
    {
        var caster = MakeCaster();
        var mgr = new Magic();
        string msg = "";
        bool moved = false, spellFire = false;
        TCreature? target = null;
        Assert.False(mgr.MagCustomSkillFull(caster, MakeUserMagic(9999), 12, 10, ref target, ref msg, ref moved, 0, ref spellFire));
    }
}
