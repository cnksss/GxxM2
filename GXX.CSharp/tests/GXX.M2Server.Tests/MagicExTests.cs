using System;
using System.Linq;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>Magic.pas 批次G：召唤系 / 组队系 / 静之系 / 合击连击系技能测试。</summary>
public class MagicExTests : IDisposable
{
    private readonly TEnvirnoment _env = MakeEnv(48, 48);

    public MagicExTests()
    {
        M2Config.ResetMagicExDefaults();
    }

    public void Dispose()
    {
        M2Config.ResetMagicExDefaults();
    }

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

    private TSpellCaster MakeCaster(int x = 10, int y = 10, int level = 40, byte job = 0)
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
        c.m_wAbil.DC1 = 10;
        c.m_wAbil.DC2 = 20;
        c.m_wAbil.MC1 = 10;
        c.m_wAbil.MC2 = 20;
        c.m_wAbil.SC1 = 10;
        c.m_wAbil.SC2 = 20;
        c.m_wAbil.HP = 500;
        c.m_wAbil.MaxHP = 500;
        return c;
    }

    private TSpellTarget MakeMonster(int x, int y, int hp = 10000, byte lifeAttrib = 0)
    {
        var m = new TSpellTarget
        {
            m_PEnvir = _env,
            m_nCurrX = x,
            m_nCurrY = y,
            m_btRaceServer = Grobal2Const.RC_MONSTER,
            m_btLifeAttrib = lifeAttrib
        };
        m.m_wAbil.HP = (uint)hp;
        m.m_wAbil.MaxHP = (uint)hp;
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

    // ================= 召唤系 =================

    [Fact]
    public void MagMakeSlave_CreatesBoneFammSlave()
    {
        var caster = MakeCaster();
        var mgr = new Magic();
        bool ok = mgr.MagMakeSlave(caster, MakeUserMagic(MagicConst.SKILL_SKELLETON));
        Assert.True(ok);
        Assert.Equal(M2Config.nBoneFammCount, caster.m_SlaveList.Count);
        var slave = caster.m_SlaveList[0];
        Assert.Equal(M2Config.sBoneFamm, slave.m_sCharName);
        Assert.Same(caster, slave.m_Master);
        Assert.NotNull(slave.m_PEnvir);
    }

    [Fact]
    public void MagMakeSinSuSlave_DogzNaming()
    {
        var caster = MakeCaster();
        var um = MakeUserMagic(MagicConst.SKILL_SINSU);
        um.btNewLevel = 5; // 4-6 档名

        var mgr = new Magic();
        Assert.True(mgr.MagMakeSinSuSlave(caster, um));
        var slave = caster.m_SlaveList[0];
        Assert.Equal(M2Config.sPlusDogzName4_6, slave.m_sCharName);
        Assert.Equal((uint)M2Config.dwPlusDogzLevels[4], slave.m_wAbil.Level);
    }

    [Fact]
    public void MagMakeBigDogSlave_HumLevelArray()
    {
        M2Config.BigDogzArray[0] = (nHumLevel: 40, sMonName: "圣兽王", nLevel: 60, nCount: 2);
        var caster = MakeCaster(level: 50);

        var mgr = new Magic();
        Assert.True(mgr.MagMakeBigDogSlave(caster, MakeUserMagic(MagicConst.SKILL_76)));
        var slave = caster.m_SlaveList[0];
        Assert.Equal("圣兽王", slave.m_sCharName);
        Assert.Equal(2, caster.m_SlaveList.Count);
    }

    [Fact]
    public void MagMakeMoon_CreatesMoonSlave()
    {
        var caster = MakeCaster();
        var mgr = new Magic();
        Assert.True(mgr.MagMakeMoon(caster, MakeUserMagic(MagicConst.SKILL_55)));
        Assert.Single(caster.m_SlaveList);
        Assert.Equal(M2Config.sMoonFamm, caster.m_SlaveList[0].m_sCharName);
    }

    // ================= 组队系 =================

    [Fact]
    public void MagGroupLightening_RangeDamage_UndeadBonus()
    {
        var caster = MakeCaster();
        var m1 = MakeMonster(12, 10);
        var m2 = MakeMonster(10, 12, lifeAttrib: Grobal2Const.LA_UNDEAD);
        var m3 = MakeMonster(40, 40); // 范围外
        foreach (var m in new[] { m1, m2, m3 }) _env.AddToMap(m.m_nCurrX, m.m_nCurrY, m);

        var mgr = new Magic();
        bool spellFire = true;
        bool ok = mgr.MagGroupLightening(caster, MakeUserMagic(MagicConst.SKILL_GROUPLIGHTENING), 11, 11, null, ref spellFire);
        Assert.True(ok);
        Assert.False(spellFire);
        Assert.True(m1.m_wAbil.HP < 10000);
        Assert.True(m2.m_wAbil.HP < 10000);
        Assert.Equal(10000u, m3.m_wAbil.HP); // 范围外不受
    }

    [Fact]
    public void MagGroupDeDing_RangeDamage()
    {
        var caster = MakeCaster();
        var m1 = MakeMonster(11, 10);
        _env.AddToMap(11, 10, m1);

        var mgr = new Magic();
        bool ok = mgr.MagGroupDeDing(caster, MakeUserMagic(MagicConst.SKILL_GROUPDEDING), 11, 10, m1);
        Assert.True(ok);
        Assert.True(m1.m_wAbil.HP < 10000);
    }

    [Fact]
    public void MagGroupAmyounsul_RequiresConfig_AndPoisons()
    {
        var caster = MakeCaster();
        var m1 = MakeMonster(11, 10);
        _env.AddToMap(11, 10, m1);

        var mgr = new Magic();
        bool spellFail = false;
        // 红毒/绿毒配置均关闭 → 直接失败
        M2Config.boSkillGroupAmyounsulRed = false;
        M2Config.boSkillGroupAmyounsulGreen = false;
        Assert.False(mgr.MagGroupAmyounsul(caster, MakeUserMagic(MagicConst.SKILL_GROUPAMYOUNSUL), 11, 10, m1, ref spellFail));

        // 开启绿毒 → 目标中绿毒
        M2Config.boSkillGroupAmyounsulGreen = true;
        Assert.True(mgr.MagGroupAmyounsul(caster, MakeUserMagic(MagicConst.SKILL_GROUPAMYOUNSUL), 11, 10, m1, ref spellFail));
        Assert.True(m1.PoisonGreenTick > 0, "目标应带上绿毒状态");
    }

    // ================= 静之系 / 合击连击系 =================

    [Fact]
    public void Skill60_Stub_ReturnsFalse()
    {
        var caster = MakeCaster();
        caster.m_Master = MakeCaster();
        var target = MakeMonster(11, 10);

        var mgr = new Magic();
        Assert.False(mgr.MagMakeSkillFire_60(caster, MakeUserMagic(60), target));
    }

    [Fact]
    public void Skill61_JointAttack_DiagonalPattern_RequiresMaster()
    {
        var caster = MakeCaster(job: 2); // 道士 SC
        var master = MakeCaster(10, 10, job: 0); // 战士 DC
        caster.m_Master = master;

        // 同行目标（|dx|≠|dy|）不在对角线图案上
        var diag = MakeMonster(13, 13);
        var offDiag = MakeMonster(14, 13);
        _env.AddToMap(13, 13, diag);
        _env.AddToMap(14, 13, offDiag);

        var mgr = new Magic();
        TCreature? target = diag;
        bool ok = mgr.MagMakeSkillFire_61(caster, MakeUserMagic(61), 13, 13, ref target);
        Assert.True(ok);
        Assert.True(diag.m_wAbil.HP < 10000, "对角线目标应受伤害");
        Assert.Equal(10000u, offDiag.m_wAbil.HP); // 非对角线不受

        // 无主人 → 直接失败
        var lonely = MakeCaster(job: 2);
        TCreature? t2 = MakeMonster(13, 13);
        Assert.False(mgr.MagMakeSkillFire_61(lonely, MakeUserMagic(61), 13, 13, ref t2));
    }

    [Fact]
    public void Skill62_JointAttack_SingleTarget()
    {
        var caster = MakeCaster(job: 0);
        caster.m_Master = MakeCaster(10, 10, job: 1);
        var target = MakeMonster(12, 10, hp: 100000);
        _env.AddToMap(12, 10, target);

        var mgr = new Magic();
        bool ok = mgr.MagMakeSkillFire_62(caster, MakeUserMagic(62), 12, 10, target);
        Assert.True(ok);
        Assert.True(target.m_wAbil.HP < 100000);
    }

    [Fact]
    public void Skill104_ContinuousAttack_HundredPercentHit()
    {
        var caster = MakeCaster();
        var target = MakeMonster(12, 10);
        _env.AddToMap(12, 10, target);

        var mgr = new Magic();
        TCreature? t = target;
        bool ok = mgr.MagMakeSkill104(caster, MakeUserMagic(104), 12, 10, ref t);
        Assert.True(ok);
        Assert.True(target.m_wAbil.HP < 10000, "连击 100% 命中应造成伤害");
    }

    [Fact]
    public void Skill111_MultiTarget_SwordRain()
    {
        var caster = MakeCaster();
        var t1 = MakeMonster(11, 10);
        var t2 = MakeMonster(12, 12);
        _env.AddToMap(11, 10, t1);
        _env.AddToMap(12, 12, t2);

        var mgr = new Magic();
        bool ok = mgr.MagMakeSkill111(caster, MakeUserMagic(111), 11, 10);
        Assert.True(ok);
        Assert.True(t1.m_wAbil.HP < 10000);
        Assert.True(t2.m_wAbil.HP < 10000);
    }

    // ================= DoSpell 分发接入 =================

    [Fact]
    public void DoSpell_DispatchesSummonFamily()
    {
        var caster = MakeCaster();
        var mgr = new Magic();
        // SKILL_SKELLETON(17) → MagMakeSlave（nBoneFammCount=2）
        Assert.True(mgr.DoSpell(caster, MakeUserMagic(MagicConst.SKILL_SKELLETON), 0, 0, null));
        Assert.Equal(2, caster.m_SlaveList.Count);
        // SKILL_SINSU(30) → MagMakeSinSuSlave
        Assert.True(mgr.DoSpell(caster, MakeUserMagic(MagicConst.SKILL_SINSU), 0, 0, null));
        Assert.Equal(3, caster.m_SlaveList.Count);
        // SKILL_76 → MagMakeBigDogSlave
        Assert.True(mgr.DoSpell(caster, MakeUserMagic(MagicConst.SKILL_76), 0, 0, null));
        Assert.Equal(4, caster.m_SlaveList.Count);
        // SKILL_55 → MagMakeMoon
        Assert.True(mgr.DoSpell(caster, MakeUserMagic(MagicConst.SKILL_55), 0, 0, null));
        Assert.Equal(5, caster.m_SlaveList.Count);
    }

    [Fact]
    public void DoSpell_DispatchesGroupFamily()
    {
        var caster = MakeCaster();
        var m1 = MakeMonster(12, 10);
        _env.AddToMap(12, 10, m1);

        var mgr = new Magic();
        // SKILL_GROUPLIGHTENING(37)
        Assert.True(mgr.DoSpell(caster, MakeUserMagic(MagicConst.SKILL_GROUPLIGHTENING), 11, 11, null));
        Assert.True(m1.m_wAbil.HP < 10000);
    }
}
