using System;
using System.IO;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>Magic.pas 技能体系测试：威力系统 / CD / 战士技能表 / DoSpell 分发 / 具体技能效果。</summary>
public class MagicTests
{
    private static TEnvirnoment MakeEnv(int w = 32, int h = 32)
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

    private static TSpellCaster MakeCaster(TEnvirnoment env, int x = 5, int y = 5, int level = 30)
    {
        var c = new TSpellCaster
        {
            m_PEnvir = env,
            m_nCurrX = x,
            m_nCurrY = y,
            m_btRaceServer = Grobal2Const.RC_PLAYOBJECT
        };
        c.m_wAbil.Level = (uint)level;
        c.m_wAbil.MC1 = 5;
        c.m_wAbil.MC2 = 10;
        c.m_wAbil.SC1 = 4;
        c.m_wAbil.SC2 = 8;
        c.m_wAbil.HP = 100;
        c.m_wAbil.MaxHP = 100;
        return c;
    }

    // ---------------- 威力系统 ----------------

    [Fact]
    public void MPow_NoLuck_WithinPowerRange()
    {
        var caster = MakeCaster(MakeEnv());
        var um = MakeUserMagic(power: 10, maxPower: 20);
        for (int i = 0; i < 50; i++)
        {
            int p = Magic.MPow(caster, um);
            // 无幸运时：Power + Random(PowerSub) ∈ [10, 20)
            Assert.InRange(p, 10, 19);
        }
    }

    [Fact]
    public void MPow_HighLuck_AlwaysMaxPower()
    {
        var caster = MakeCaster(MakeEnv());
        caster.m_nLuck = M2Config.nMaxLuckMaxPower; // 幸运 ≥ 上限 → 恒最大
        var um = MakeUserMagic(power: 10, maxPower: 20);
        for (int i = 0; i < 20; i++)
            Assert.Equal(20, Magic.MPow(caster, um));
    }

    [Fact]
    public void GetPower_ScalesWithLevel()
    {
        var low = MakeCaster(MakeEnv(), level: 0);
        var high = MakeCaster(MakeEnv(), level: 30);
        // defMaxPower == defPower → DefPowerSub=0，结果确定（按 UserMagic.btLevel 缩放）
        var um = MakeUserMagic(trainLv: 3, defPower: 5, defMaxPower: 5);
        um.btLevel = 0;
        int pLow = Magic.GetPower(low, 12, um);
        Assert.Equal(12 / 4 * 1 + 5, pLow);
        var umHigh = MakeUserMagic(trainLv: 3, defPower: 5, defMaxPower: 5);
        umHigh.btLevel = 30;
        int pHigh = Magic.GetPower(high, 12, umHigh);
        Assert.Equal(12 / 4 * 31 + 5, pHigh);
        // 高幸运同值（DefPowerSub=0 时上下限一致）
        high.m_nLuck = M2Config.nMaxLuckMaxPower;
        Assert.Equal(12 / 4 * 31 + 5, Magic.GetPower(high, 12, umHigh));
    }

    [Fact]
    public void GetPower13_SplitsOneThird()
    {
        var caster = MakeCaster(MakeEnv(), level: 0);
        caster.m_nLuck = M2Config.nMaxLuckMaxPower;
        // nInt=30：d10=10, d18=20 → MagicInfo=nil → Round(20)=20
        Assert.Equal(20, Magic.GetPower13(caster, 30, null));
    }

    [Fact]
    public void GetRPow_RangeOrValue()
    {
        var caster = MakeCaster(MakeEnv());
        caster.m_nLuck = M2Config.nMaxLuckMaxPower;
        // 高幸运：Max(nInt2-nInt1+1,1) + nInt1 = 5+3 = 8
        Assert.Equal(8, Magic.GetRPow(caster, 3, 7));
        Assert.Equal(7, Magic.GetRPow(caster, 7, 3));    // nInt2<=nInt1 → nInt1
        Assert.Equal(7, Magic.GetRPow(caster, 7, 7));
    }

    [Fact]
    public void GetNewLevelPower_DefaultTable()
    {
        var caster = MakeCaster(MakeEnv());
        var um = MakeUserMagic(magicId: 999); // 非特定技能 → 默认表
        um.btNewLevel = 1;
        M2Config.NewLevelMagicPowerRates[0] = 120; // 1重 = 120%
        int p = Magic.GetNewLevelPower(100, um);
        Assert.Equal(120, p);
        um.btNewLevel = 10; // 超过 9 重：9 重威力 × (1 + (10-9)*After9/100)
        M2Config.NewLevelMagicPowerRates[8] = 200;
        M2Config.NewLevelMagicPowerRatesAfter9 = 10;
        p = Magic.GetNewLevelPower(100, um);
        Assert.Equal((int)Math.Round(200 * 1.1), p); // 220
        um.btNewLevel = 0; // 无强化 → 原威力
        Assert.Equal(100, Magic.GetNewLevelPower(100, um));
    }

    [Fact]
    public void GetNewLevelPower_SpecificTable_FireballId3()
    {
        var caster = MakeCaster(MakeEnv());
        var um = MakeUserMagic(magicId: 3); // 基本剑术 → Index 0
        um.btNewLevel = 5;
        M2Config.NewLevelMagicPowerRatesSpecific[0][4] = 150;
        Assert.Equal(150, Magic.GetNewLevelPower(100, um));
    }

    // ---------------- 战士技能表 / CD ----------------

    [Fact]
    public void IsWarrSkill_MatchesDelphiList()
    {
        Assert.True(Magic.IsWarrSkill(3));   // 基本剑术
        Assert.True(Magic.IsWarrSkill(4));   // 攻杀（ILKWANG 实为攻杀? 原注释）
        Assert.True(Magic.IsWarrSkill(7));   // 半月? YEDO
        Assert.True(Magic.IsWarrSkill(12));  // 刺杀
        Assert.True(Magic.IsWarrSkill(25));
        Assert.True(Magic.IsWarrSkill(26));
        Assert.True(Magic.IsWarrSkill(27));
        Assert.True(Magic.IsWarrSkill(40));
        Assert.True(Magic.IsWarrSkill(42));
        Assert.True(Magic.IsWarrSkill(43));
        Assert.True(Magic.IsWarrSkill(56));
        Assert.True(Magic.IsWarrSkill(60));
        Assert.True(Magic.IsWarrSkill(66));
        Assert.True(Magic.IsWarrSkill(100));
        Assert.True(Magic.IsWarrSkill(103));
        Assert.True(Magic.IsWarrSkill(113));
        Assert.False(Magic.IsWarrSkill(1));  // 火球
        Assert.False(Magic.IsWarrSkill(2));  // 治愈
        Assert.False(Magic.IsWarrSkill(11)); // 雷电
        Assert.False(Magic.IsWarrSkill(0));
    }

    [Fact]
    public void CheckCD_Works()
    {
        var caster = MakeCaster(MakeEnv());
        Assert.True(Magic.CheckCD(caster, 1, 1000, true));   // 跳过检查
        caster.m_SkillUseTick[1] = GXX.Core.Rtl.DelphiRTL.GetTickCount();
        Assert.False(Magic.CheckCD(caster, 1, 100000, false)); // 刚用过 → 冷却
        caster.m_SkillUseTick[1] = 0;                        // 久远 → 可用
        Assert.True(Magic.CheckCD(caster, 1, 1000, false));
        caster.m_SkillUseTick[1] = GXX.Core.Rtl.DelphiRTL.GetTickCount();
        Assert.False(Magic.CheckCD(caster, 1, 100000, false));
    }

    // ---------------- DoSpell 分发与技能效果 ----------------

    private static TUserMagicRef MakeUserMagic(int magicId = 1, int power = 10, int maxPower = 10, int trainLv = 3, int defPower = 0, int defMaxPower = 0)
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
                btTrainLv = (byte)trainLv,
                wDefPower = (ushort)defPower,
                wDefMaxPower = (ushort)defMaxPower
            }
        };
    }

    [Fact]
    public void DoSpell_WarrSkill_Rejected()
    {
        var env = MakeEnv();
        var caster = MakeCaster(env);
        var mgr = new Magic();
        Assert.False(mgr.DoSpell(caster, MakeUserMagic(3), 6, 5, null)); // 战士技能不能施展
    }

    [Fact]
    public void DoSpell_Fireball_DamagesTarget()
    {
        var env = MakeEnv();
        var caster = MakeCaster(env, 5, 5);
        var target = new TSpellTarget { m_PEnvir = env, m_nCurrX = 8, m_nCurrY = 5, m_btRaceServer = Grobal2Const.RC_MONSTER };
        target.m_wAbil.HP = 1000;
        target.m_wAbil.MaxHP = 1000;
        env.AddToMap(8, 5, target);

        var mgr = new Magic();
        TSpellTarget? outTarget = target;
        bool ok = mgr.DoSpell(caster, MakeUserMagic(MagicConst.SKILL_FIREBALL, 50, 50), 8, 5, outTarget);
        Assert.True(ok);
        Assert.True(target.m_wAbil.HP < 1000, "火球应造成伤害");
    }

    [Fact]
    public void DoSpell_Healing_HealsFriend()
    {
        var env = MakeEnv();
        var caster = MakeCaster(env, 5, 5);
        caster.m_wAbil.HP = 50;
        var friend = new TSpellTarget { m_PEnvir = env, m_nCurrX = 6, m_nCurrY = 5, m_btRaceServer = Grobal2Const.RC_PLAYOBJECT };
        friend.m_wAbil.HP = 40;
        friend.m_wAbil.MaxHP = 100;

        var mgr = new Magic();
        bool ok = mgr.DoSpell(caster, MakeUserMagic(MagicConst.SKILL_HEALLING, 30, 30), 6, 5, friend);
        Assert.True(ok);
        // 延时消息队列中应有 RM_MAGHEALING；此处直接检查延迟表被投递
        Assert.True(caster.DelayedMsgCount + friend.DelayedMsgCount > 0, "应有延时治疗消息");
    }

    [Fact]
    public void DoSpell_Firewind_PushesNearbyLowerLevel()
    {
        var env = MakeEnv();
        var caster = MakeCaster(env, 10, 10, level: 50);
        var enemy = new TSpellTarget { m_PEnvir = env, m_nCurrX = 11, m_nCurrY = 10, m_btRaceServer = Grobal2Const.RC_MONSTER };
        enemy.m_wAbil.Level = 10;
        enemy.m_nCurrX = 11;
        enemy.m_nCurrY = 10;
        env.AddToMap(11, 10, enemy);
        caster.AddVisibleActor(enemy);

        var mgr = new Magic();
        int pushed = mgr.MagPushArround(caster, MakeUserMagic(MagicConst.SKILL_FIREWIND), 3);
        Assert.Equal(1, pushed);
        // 被推离原位
        Assert.True(enemy.m_nCurrX != 11 || enemy.m_nCurrY != 10);
    }

    [Fact]
    public void DoSpell_TurnUndead_UndeadTakesBonus()
    {
        var env = MakeEnv();
        var caster = MakeCaster(env, 5, 5);
        caster.m_wAbil.MC1 = 50;
        caster.m_wAbil.MC2 = 60;
        var undead = new TSpellTarget { m_PEnvir = env, m_nCurrX = 6, m_nCurrY = 5, m_btLifeAttrib = Grobal2Const.LA_UNDEAD, m_btRaceServer = Grobal2Const.RC_MONSTER };
        undead.m_wAbil.HP = 10000;
        undead.m_wAbil.MaxHP = 10000;
        undead.m_nAntiMagic = 0;
        env.AddToMap(6, 5, undead);

        var mgr = new Magic();
        bool ok = mgr.MagTurnUndead(caster, undead, 6, 5, 3);
        Assert.True(ok);
        Assert.True(undead.m_wAbil.HP < 10000);
    }

    [Fact]
    public void DoSpell_BigExplosion_RangeDamage()
    {
        var env = MakeEnv();
        var caster = MakeCaster(env, 5, 5);
        var m1 = new TSpellTarget { m_PEnvir = env, m_nCurrX = 7, m_nCurrY = 5, m_btRaceServer = Grobal2Const.RC_MONSTER };
        var m2 = new TSpellTarget { m_PEnvir = env, m_nCurrX = 5, m_nCurrY = 7, m_btRaceServer = Grobal2Const.RC_MONSTER };
        foreach (var m in new[] { m1, m2 })
        {
            m.m_wAbil.HP = 500;
            m.m_wAbil.MaxHP = 500;
            env.AddToMap(m.m_nCurrX, m.m_nCurrY, m);
        }
        var mgr = new Magic();
        int hit = mgr.MagBigExplosion(caster, MakeUserMagic(MagicConst.SKILL_FIREBOOM, 100, 100), 100, 6, 6, 2, 100);
        Assert.Equal(2, hit);
        Assert.True(m1.m_wAbil.HP < 500);
        Assert.True(m2.m_wAbil.HP < 500);
    }

    [Fact]
    public void DoSpell_HellFire_LineDamage()
    {
        var env = MakeEnv();
        var caster = MakeCaster(env, 5, 5);
        // 目标在右侧 → 地狱火向右喷 5 格，(7,5) 处有怪
        var m1 = new TSpellTarget { m_PEnvir = env, m_nCurrX = 7, m_nCurrY = 5, m_btRaceServer = Grobal2Const.RC_MONSTER };
        var m2 = new TSpellTarget { m_PEnvir = env, m_nCurrX = 5, m_nCurrY = 7, m_btRaceServer = Grobal2Const.RC_MONSTER }; // 不在直线上
        foreach (var m in new[] { m1, m2 })
        {
            m.m_wAbil.HP = 500;
            m.m_wAbil.MaxHP = 500;
            env.AddToMap(m.m_nCurrX, m.m_nCurrY, m);
        }
        var mgr = new Magic();
        bool ok = mgr.DoSpell(caster, MakeUserMagic(MagicConst.SKILL_FIRE, 40, 40), 10, 5, null);
        Assert.True(ok);
        Assert.True(m1.m_wAbil.HP < 500, "直线上的怪应受伤害");
        Assert.Equal(500u, m2.m_wAbil.HP); // 线外不受
    }
}
