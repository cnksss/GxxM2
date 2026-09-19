using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Rtl;

namespace GXX.M2Server.Engine;

/// <summary>
/// Magic.pas 批次G 处理器（partial Magic）：
/// 召唤系 MagMakeSlave/MagMakeSinSuSlave/MagMakeBigDogSlave/MagMakeMoon、
/// 组队系 MagGroupLightening/MagGroupAmyounsul/MagGroupDeDing、
/// 静之/合击系 MagMakeSkillFire_60..65、连击系 MagMakeSkill104..111/114/116/117。
/// </summary>
public partial class Magic
{
    private static readonly Random RndEx = new();

    // ================= 召唤系 =================

    /// <summary>召唤参数解析（对应 BoneFamm/Dogz/BigDogz 三套 Array + NewLevel 档名表 + 插件优先开关）。</summary>
    private static (string monName, int makeLevel, int expLevel, int count) ResolveSummon(
        TCreature playObject, TUserMagicRef userMagic,
        string baseName, int baseCount,
        bool plugPriority,
        string n13, string n46, string n79, string n9N,
        int[] levels, int addLevelAfter9,
        (int nHumLevel, string sMonName, int nLevel, int nCount)[] humArray)
    {
        string monName = baseName;
        int makeLevel = userMagic.btLevel;
        int expLevel = userMagic.btLevel;
        int count = baseCount;

        void ApplyNewLevel()
        {
            if (userMagic.btNewLevel > 0)
            {
                monName = userMagic.btNewLevel <= 3 ? n13
                    : userMagic.btNewLevel <= 6 ? n46
                    : userMagic.btNewLevel <= 9 ? n79 : n9N;
                expLevel = userMagic.btNewLevel <= 9
                    ? levels[userMagic.btNewLevel - 1]
                    : levels[9 - 1] + (userMagic.btNewLevel - 9) * addLevelAfter9;
            }
        }

        void ApplyHumArray()
        {
            foreach (var entry in humArray)
            {
                if (entry.nHumLevel == 0) break;
                if (playObject.m_wAbil.Level >= (uint)entry.nHumLevel)
                {
                    monName = entry.sMonName;
                    expLevel = entry.nLevel;
                    count = entry.nCount;
                }
            }
        }

        if (!plugPriority)
        {
            ApplyNewLevel();
            ApplyHumArray();
        }
        else
        {
            ApplyHumArray();
            ApplyNewLevel();
        }

        // 英雄召唤数量覆盖
        if (playObject.m_btRaceServer == Grobal2Const.RC_HEROOBJECT && M2Config.dwHeroCallBBCount > 0)
            count = (int)M2Config.dwHeroCallBBCount;

        return (monName, makeLevel, expLevel, count);
    }

    /// <summary>MagMakeSlave：召唤骷髅（SKILL_SKELLETON 17）。</summary>
    public bool MagMakeSlave(TCreature playObject, TUserMagicRef userMagic)
    {
        var (monName, makeLevel, expLevel, count) = ResolveSummon(
            playObject, userMagic,
            M2Config.sBoneFamm, M2Config.nBoneFammCount,
            M2Config.boBonePlugSettingPriority,
            M2Config.sPlusBoneFammName1_3, M2Config.sPlusBoneFammName4_6,
            M2Config.sPlusBoneFammName7_9, M2Config.sPlusBoneFammName9_N,
            M2Config.dwPlusBoneFammLevels, M2Config.dwPlusBoneFammAddLevelAfter9,
            M2Config.BoneFammArray);

        var slave = playObject.MakeSlave(monName, makeLevel, expLevel, count, 10u * 24 * 60 * 60, 0 /* bb_BoneFamm */, userMagic.btNewLevel, true);
        return slave != null;
    }

    /// <summary>MagMakeSinSuSlave：召唤神兽（SKILL_SINSU 30，Dogz 配置）。</summary>
    public bool MagMakeSinSuSlave(TCreature playObject, TUserMagicRef userMagic)
    {
        var (monName, makeLevel, expLevel, count) = ResolveSummon(
            playObject, userMagic,
            M2Config.sDogz, M2Config.nDogzCount,
            M2Config.boDogzPlugSettingPriority,
            M2Config.sPlusDogzName1_3, M2Config.sPlusDogzName4_6,
            M2Config.sPlusDogzName7_9, M2Config.sPlusDogzName9_N,
            M2Config.dwPlusDogzLevels, M2Config.dwPlusDogzAddLevelAfter9,
            M2Config.DogzArray);

        var slave = playObject.MakeSlave(monName, makeLevel, expLevel, count, 10u * 24 * 60 * 60, 1 /* bb_Dogz */, userMagic.btNewLevel, true);
        return slave != null;
    }

    /// <summary>MagMakeBigDogSlave：召唤圣兽（SKILL_76，BigDogz 仅人等映射表）。</summary>
    public bool MagMakeBigDogSlave(TCreature playObject, TUserMagicRef userMagic)
    {
        var (monName, makeLevel, expLevel, count) = ResolveSummon(
            playObject, userMagic,
            M2Config.sBigDogz, M2Config.nBigDogzCount,
            false,
            "", "", "", "",
            Array.Empty<int>(), 0,
            M2Config.BigDogzArray);

        var slave = playObject.MakeSlave(monName, makeLevel, expLevel, count, 10u * 24 * 60 * 60, 2 /* bb_BigDogz */, userMagic.btNewLevel, !M2Config.boBBAttrPlusAddOnlyMagic);
        return slave != null;
    }

    /// <summary>MagMakeMoon：召唤月灵（SKILL_55）。</summary>
    public bool MagMakeMoon(TCreature playObject, TUserMagicRef userMagic)
    {
        var slave = playObject.MakeSlave(M2Config.sMoonFamm, userMagic.btLevel, userMagic.btLevel, M2Config.nMoonCount, 10u * 24 * 60 * 60, 3 /* bb_MonthSpirit */, userMagic.btNewLevel, true);
        return slave != null;
    }

    // ================= 范围目标枚举（对应 GetMapBaseObjects） =================

    private static List<TCreature> GetMapBaseObjects(TEnvirnoment envir, int nX, int nY, int range)
    {
        var list = new List<TCreature>();
        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                foreach (var obj in envir.GetObjects(nX + dx, nY + dy))
                {
                    if (obj is TCreature c)
                        list.Add(c);
                }
            }
        }
        return list;
    }

    // ================= 组队系 =================

    /// <summary>MagGroupLightening：群体雷电术（SKILL 37）。</summary>
    public bool MagGroupLightening(TCreature playObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, TCreature? targeTBaseObject, ref bool boSpellFire)
    {
        boSpellFire = false;
        bool result = false;
        if (playObject.m_PEnvir == null) return false;

        int range = userMagic.btLevel <= 0
            ? M2Config.nSkill37Range
            : M2Config.nSkill37Range + userMagic.btLevel * M2Config.nSkill37RangeAdd;

        var objects = GetMapBaseObjects(playObject.m_PEnvir, nTargetX, nTargetY, range);
        foreach (var obj in objects)
        {
            if (obj.m_boDeath || obj.m_boGhost || ReferenceEquals(playObject, obj)) continue;
            if (!playObject.IsProperTarget(obj)) continue;

            if (RndEx.Next(10) >= obj.m_nAntiMagic)
            {
                int nPower = playObject.GetAttackPower(
                    GetPower(playObject, MPow(playObject, userMagic), userMagic) + playObject.m_wAbil.MC1,
                    (int)(playObject.m_wAbil.MC2 - playObject.m_wAbil.MC1) + 1);
                if (obj.m_btLifeAttrib == Grobal2Const.LA_UNDEAD)
                    nPower = (int)Math.Min(Math.Round(nPower * 1.5), int.MaxValue);
                nPower = GetNewLevelPower(nPower, userMagic);
                nPower = (int)Math.Round(nPower / 100.0 * M2Config.nSkillGroupLighteningPowerRate);
                obj.StruckDamage(nPower);
                result = true;
            }
        }
        boSpellFire = !result;
        return result;
    }

    /// <summary>MagGroupAmyounsul：群体施毒术（SKILL 51；红毒/绿毒配置门控）。</summary>
    public bool MagGroupAmyounsul(TCreature playObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, TCreature? targeTBaseObject, ref bool boSpellFail)
    {
        boSpellFail = false;
        bool result = false;
        if (!M2Config.boSkillGroupAmyounsulRed && !M2Config.boSkillGroupAmyounsulGreen)
            return false;

        if (playObject.m_PEnvir == null) return false;
        // 原 CheckAmulet（毒符）简化为配置门控：允许则对范围内目标按类型下毒
        int range = 3;
        var objects = GetMapBaseObjects(playObject.m_PEnvir, nTargetX, nTargetY, range);
        foreach (var obj in objects)
        {
            if (obj.m_boDeath || ReferenceEquals(playObject, obj)) continue;
            if (!playObject.IsProperTarget(obj)) continue;

            int poisonType;
            if (targeTBaseObject != null && targeTBaseObject.PoisonGreenTick <= 0)
                poisonType = 1;   // 绿毒（DecHealth）
            else
                poisonType = 2;   // 红毒（DamageArmor）

            if (!M2Config.boSkillGroupAmyounsulRed || !M2Config.boSkillGroupAmyounsulGreen)
            {
                poisonType = M2Config.boSkillGroupAmyounsulGreen ? 1 : 2;
            }

            if (poisonType == 1)
                obj.PoisonGreenTick += 100 + userMagic.btLevel * 20;
            else
                obj.PoisonRedTick += 100 + userMagic.btLevel * 20;
            result = true;
        }
        return result;
    }

    /// <summary>MagGroupDeDing：彻地钉（SKILL 39，范围伤害 + 冰冻）。</summary>
    public bool MagGroupDeDing(TCreature playObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, TCreature? targeTBaseObject)
    {
        bool result = false;
        if (playObject.m_PEnvir == null) return false;
        int nPower = playObject.GetAttackPower(
            GetPower(playObject, MPow(playObject, userMagic), userMagic) + playObject.m_wAbil.DC1,
            (int)(playObject.m_wAbil.DC2 - playObject.m_wAbil.DC1) + 1);
        nPower = GetNewLevelPower(nPower, userMagic);

        var objects = GetMapBaseObjects(playObject.m_PEnvir, nTargetX, nTargetY, M2Config.nSkill39Range);
        foreach (var obj in objects)
        {
            if (obj.m_boDeath || ReferenceEquals(playObject, obj)) continue;
            if (!playObject.IsProperTarget(obj)) continue;
            obj.StruckDamage(nPower);
            obj.m_boFreeze = true;   // 冰冻（原 nSkill39FreezeTime 毫秒）
            result = true;
        }
        return result;
    }

    // ================= 合击系公共核心 =================

    /// <summary>合击威力：本体(按职业取 DC/MC/SC) + 主人同职业规则 + 等级加成（61/62 原实现合并）。</summary>
    private static int JointAttackPower(TCreature baseObject, TUserMagicRef userMagic)
    {
        int nPower = JobAttackPower(baseObject, userMagic);
        if (baseObject.m_Master != null)
            nPower += JobAttackPower(baseObject.m_Master, userMagic);
        nPower += (int)Math.Round(nPower * ((userMagic.btLevel + userMagic.btNewLevel) * M2Config.nSkillJointAttackLevelRate / 100.0));
        nPower += (int)Math.Round(nPower / 100.0 * (baseObject.m_wAbil.GetNewValue(10) + (baseObject.m_Master?.m_wAbil.GetNewValue(10) ?? 0)));
        return nPower;
    }

    private static int JobAttackPower(TCreature obj, TUserMagicRef userMagic)
    {
        return obj.m_btJob switch
        {
            2 => obj.GetAttackPower(GetPower(obj, MPow(obj, userMagic), userMagic) + obj.m_wAbil.SC1, Math.Max((int)(obj.m_wAbil.SC2 - obj.m_wAbil.SC1), 1)),
            1 => obj.GetAttackPower(GetPower(obj, MPow(obj, userMagic), userMagic) + obj.m_wAbil.MC1, Math.Max((int)(obj.m_wAbil.MC2 - obj.m_wAbil.MC1), 1)),
            _ => obj.GetAttackPower(GetPower(obj, MPow(obj, userMagic), userMagic) + obj.m_wAbil.DC1, Math.Max((int)(obj.m_wAbil.DC2 - obj.m_wAbil.DC1), 1))
        };
    }

    private static int SplitPowerByHuman(int nPower, TCreature target, int humRate, int monRate)
    {
        bool isHuman = target.m_btRaceServer is Grobal2Const.RC_PLAYOBJECT or Grobal2Const.RC_HEROOBJECT or Grobal2Const.RC_PLAYMOSTER;
        if (!isHuman && target.m_Master != null)
            isHuman = target.m_Master.m_btRaceServer is Grobal2Const.RC_PLAYOBJECT or Grobal2Const.RC_HEROOBJECT or Grobal2Const.RC_PLAYMOSTER;
        return (int)Math.Min(Math.Round(nPower * (isHuman ? humRate : monRate) / 100.0), int.MaxValue);
    }

    // ================= 静之/合击系 60-65 =================

    /// <summary>静之破魂斩（60）：原实现为空桩直接返回 False。</summary>
    public bool MagMakeSkillFire_60(TCreature baseObject, TUserMagicRef userMagic, TCreature? targeTBaseObject)
    {
        return false;
    }

    /// <summary>劈星斩（61）：主人+本体合击威力，7x7 内对角线（|dx|==|dy|）目标受击。</summary>
    public bool MagMakeSkillFire_61(TCreature baseObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, ref TCreature? targeTBaseObject)
    {
        bool result = false;
        if (baseObject.m_Master == null) return false;
        var target = targeTBaseObject;
        if (target == null || !baseObject.IsProperTarget(target)) return false;
        if (Math.Abs(target.m_nCurrX - nTargetX) <= 1 && Math.Abs(target.m_nCurrY - nTargetY) <= 1)
        {
            if (target.m_nAntiMagic <= RndEx.Next(10))
            {
                int nPower = JointAttackPower(baseObject, userMagic);
                if (baseObject.m_PEnvir == null) return false;
                for (int nX = nTargetX - 3; nX <= nTargetX + 3; nX++)
                {
                    for (int nY = nTargetY - 3; nY <= nTargetY + 3; nY++)
                    {
                        if (Math.Abs(nX - nTargetX) != Math.Abs(nY - nTargetY)) continue;
                        foreach (var obj in baseObject.m_PEnvir.GetObjects(nX, nY))
                        {
                            if (obj is TCreature t && baseObject.IsProperTarget(t))
                            {
                                int nPower2 = SplitPowerByHuman(nPower, t, M2Config.nSkill61AttackHumPowerRate, M2Config.nSkill61PowerRate);
                                t.StruckDamage(nPower2);
                            }
                        }
                    }
                }
            }
        }
        result = true;
        return result;
    }

    /// <summary>雷霆一击（62）：主人+本体合击威力单体打击（1:1 合击位）。</summary>
    public bool MagMakeSkillFire_62(TCreature baseObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, TCreature? targeTBaseObject)
    {
        bool result = false;
        if (baseObject.m_Master == null) return false;
        var target = targeTBaseObject;
        if (target == null || !baseObject.IsProperTarget(target)) return false;

        int nPower = JointAttackPower(baseObject, userMagic);
        nPower = SplitPowerByHuman(nPower, target, M2Config.nSkill62AttackHumPowerRate, M2Config.nSkill62PowerRate);
        target.StruckDamage(nPower);
        result = true;
        return result;
    }

    /// <summary>噬魂沼泽（63）：目标周围 3x3 毒沼。</summary>
    public bool MagMakeSkillFire_63(TCreature baseObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, TCreature? targeTBaseObject)
    {
        bool result = false;
        if (baseObject.m_Master == null || baseObject.m_PEnvir == null) return false;
        int nPower = JointAttackPower(baseObject, userMagic);
        var objects = GetMapBaseObjects(baseObject.m_PEnvir, nTargetX, nTargetY, 1);
        foreach (var obj in objects)
        {
            if (!baseObject.IsProperTarget(obj)) continue;
            obj.PoisonGreenTick += 100;
            obj.StruckDamage(nPower / 2);
            result = true;
        }
        return result;
    }

    /// <summary>末日审判（64）：目标周围 5x5 强力合击。</summary>
    public bool MagMakeSkillFire_64(TCreature baseObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, TCreature? targeTBaseObject)
    {
        bool result = false;
        if (baseObject.m_Master == null || baseObject.m_PEnvir == null) return false;
        int nPower = JointAttackPower(baseObject, userMagic);
        var objects = GetMapBaseObjects(baseObject.m_PEnvir, nTargetX, nTargetY, 2);
        foreach (var obj in objects)
        {
            if (!baseObject.IsProperTarget(obj)) continue;
            obj.StruckDamage(nPower);
            result = true;
        }
        return result;
    }

    /// <summary>火龙气焰（65）：直线火龙合击。</summary>
    public bool MagMakeSkillFire_65(TCreature baseObject, TUserMagicRef userMagic, int nTargetX, int nTargetY)
    {
        bool result = false;
        if (baseObject.m_Master == null || baseObject.m_PEnvir == null) return false;
        int nPower = JointAttackPower(baseObject, userMagic);
        byte dir = TCreature.GetNextDirection(baseObject.m_nCurrX, baseObject.m_nCurrY, nTargetX, nTargetY);
        TCreature.GetNextPosition(baseObject.m_nCurrX, baseObject.m_nCurrY, dir, 1, out int sx, out int sy);
        int hit = baseObject.MagPassThroughMagic(sx, sy, nTargetX, nTargetY, dir, nPower, userMagic.wMagIdx, false);
        return hit > 0;
    }

    // ================= 连击系 104-111 / 114 / 116 / 117 =================

    /// <summary>连击威力核心：ContinuousPowerRates + 等级加成（104-110 共用，原 SkillContinuousPowerRates[4..]）。</summary>
    private static int ContinuousPower(TCreature baseObject, TUserMagicRef userMagic, int rateIndex)
    {
        int nPower = baseObject.GetAttackPower(
            GetPower(baseObject, MPow(baseObject, userMagic), userMagic) + baseObject.m_wAbil.MC1,
            Math.Max((int)(baseObject.m_wAbil.MC2 - baseObject.m_wAbil.MC1), 1));
        nPower = GetNewLevelPower(nPower, userMagic);
        int rate = rateIndex < M2Config.SkillContinuousPowerRates.Length ? M2Config.SkillContinuousPowerRates[rateIndex] : 100;
        nPower = (int)Math.Round(nPower * (rate / 100.0));
        nPower = (int)Math.Min((long)nPower + Math.Round(nPower * (userMagic.btLevel * M2Config.nContinuousAttackLevelRate / 100.0)), int.MaxValue);
        return nPower;
    }

    private static bool ContinuousHit(TCreature baseObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, ref TCreature? targeTBaseObject, int rateIndex)
    {
        bool result = false;
        var target = targeTBaseObject;
        if (target == null || !baseObject.MagCanHitTarget(baseObject.m_nCurrX, baseObject.m_nCurrY, target))
        {
            targeTBaseObject = null;
            return false;
        }
        if (!baseObject.IsProperTarget(target))
        {
            targeTBaseObject = null;
            return false;
        }
        if (Math.Abs(target.m_nCurrX - nTargetX) <= 1 && Math.Abs(target.m_nCurrY - nTargetY) <= 1)
        {
            // 连击 100% 命中（原实现注释掉 AntiMagic 判定）
            int nPower = ContinuousPower(baseObject, userMagic, rateIndex);
            target.StruckDamage(nPower);
            if (target.m_btRaceServer >= Grobal2Const.RC_ANIMAL)
                result = true;
            if (baseObject.m_btRaceServer is Grobal2Const.RC_HEROOBJECT or Grobal2Const.RC_PLAYMOSTER ||
                target.m_btRaceServer is Grobal2Const.RC_HEROOBJECT or Grobal2Const.RC_PLAYMOSTER)
                result = true;
        }
        else
        {
            targeTBaseObject = null;
        }
        return result;
    }

    /// <summary>凤舞祭（104）。</summary>
    public bool MagMakeSkill104(TCreature baseObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, ref TCreature? targeTBaseObject)
        => ContinuousHit(baseObject, userMagic, nTargetX, nTargetY, ref targeTBaseObject, 4);

    /// <summary>惊雷爆（105）：范围版连击。</summary>
    public bool MagMakeSkill105(TCreature baseObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, TCreature? targeTBaseObject)
    {
        bool result = false;
        if (baseObject.m_PEnvir == null) return false;
        int nPower = ContinuousPower(baseObject, userMagic, 5);
        var objects = GetMapBaseObjects(baseObject.m_PEnvir, nTargetX, nTargetY, 1);
        foreach (var obj in objects)
        {
            if (!baseObject.IsProperTarget(obj)) continue;
            obj.StruckDamage(nPower);
            result = true;
        }
        return result;
    }

    /// <summary>冰天雪地（106）：范围冰冻。</summary>
    public bool MagMakeSkill106(TCreature baseObject, TUserMagicRef userMagic, int nTargetX, int nTargetY)
    {
        bool result = false;
        if (baseObject.m_PEnvir == null) return false;
        int nPower = ContinuousPower(baseObject, userMagic, 6);
        var objects = GetMapBaseObjects(baseObject.m_PEnvir, nTargetX, nTargetY, 2);
        foreach (var obj in objects)
        {
            if (!baseObject.IsProperTarget(obj)) continue;
            obj.StruckDamage(nPower);
            obj.m_boFreeze = true;
            result = true;
        }
        return result;
    }

    /// <summary>双龙破（107）。</summary>
    public bool MagMakeSkill107(TCreature baseObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, ref TCreature? targeTBaseObject)
        => ContinuousHit(baseObject, userMagic, nTargetX, nTargetY, ref targeTBaseObject, 7);

    /// <summary>虎啸诀（108）。</summary>
    public bool MagMakeSkill108(TCreature baseObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, ref TCreature? targeTBaseObject)
        => ContinuousHit(baseObject, userMagic, nTargetX, nTargetY, ref targeTBaseObject, 8);

    /// <summary>八卦掌（109）。</summary>
    public bool MagMakeSkill109(TCreature baseObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, ref TCreature? targeTBaseObject)
        => ContinuousHit(baseObject, userMagic, nTargetX, nTargetY, ref targeTBaseObject, 9);

    /// <summary>三焰咒（110）：目标三方向火焰。</summary>
    public bool MagMakeSkill110(TCreature baseObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, ref TCreature? targeTBaseObject)
    {
        bool result = false;
        if (baseObject.m_PEnvir == null) return false;
        int nPower = ContinuousPower(baseObject, userMagic, 3);
        var objects = GetMapBaseObjects(baseObject.m_PEnvir, nTargetX, nTargetY, 1);
        foreach (var obj in objects)
        {
            if (!baseObject.IsProperTarget(obj)) continue;
            obj.StruckDamage(nPower);
            result = true;
        }
        return result;
    }

    /// <summary>万剑归宗（111）：全场可攻击目标（万剑）。</summary>
    public bool MagMakeSkill111(TCreature baseObject, TUserMagicRef userMagic, int nTargetX, int nTargetY)
    {
        bool result = false;
        if (baseObject.m_PEnvir == null) return false;
        int nPower = ContinuousPower(baseObject, userMagic, 6);
        var objects = GetMapBaseObjects(baseObject.m_PEnvir, baseObject.m_nCurrX, baseObject.m_nCurrY, 12);
        foreach (var obj in objects)
        {
            if (!baseObject.IsProperTarget(obj)) continue;
            obj.StruckDamage(nPower);
            result = true;
        }
        return result;
    }

    /// <summary>倚天辟地（114）：目标双爆。</summary>
    public bool MagMakeSkill114(TCreature baseObject, TUserMagicRef userMagic, ref int nTargetX, ref int nTargetY)
    {
        bool result = false;
        if (baseObject.m_PEnvir == null) return false;
        int nPower = ContinuousPower(baseObject, userMagic, 7);
        for (int pass = 0; pass < 2; pass++)
        {
            var objects = GetMapBaseObjects(baseObject.m_PEnvir, nTargetX, nTargetY, 2);
            foreach (var obj in objects)
            {
                if (!baseObject.IsProperTarget(obj)) continue;
                obj.StruckDamage(nPower);
                result = true;
            }
        }
        return result;
    }

    /// <summary>血魄一击（116/117）。</summary>
    public bool MagMakeSkill116(TCreature baseObject, TUserMagicRef userMagic, int nTargetX, int nTargetY)
    {
        bool result = false;
        if (baseObject.m_PEnvir == null) return false;
        int nPower = ContinuousPower(baseObject, userMagic, 8);
        var objects = GetMapBaseObjects(baseObject.m_PEnvir, nTargetX, nTargetY, 1);
        foreach (var obj in objects)
        {
            if (!baseObject.IsProperTarget(obj)) continue;
            obj.StruckDamage(nPower);
            result = true;
        }
        return result;
    }

    public bool MagMakeSkill117(TCreature baseObject, TUserMagicRef userMagic, int nTargetX, int nTargetY)
        => MagMakeSkill116(baseObject, userMagic, nTargetX, nTargetY);
}
