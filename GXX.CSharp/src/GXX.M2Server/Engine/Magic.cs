using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Rtl;

namespace GXX.M2Server.Engine;

/// <summary>
/// Magic.pas TMagicManager 1:1 核心转换：
/// 威力系统（MPow/GetPower/GetPower13/GetRPow/GetNewLevelPower 含九重强化表）、
/// 战士技能表（IsWarrSkill）、CD 检查（CheckCD）、DoSpell 完整分发、
/// 以及主要技能处理（火球/治愈/群体治愈/地狱火/疾光/雷电/抗拒火环/圣言/爆裂/噬血/灵魂火符）。
/// 注：伤害类技能在托管版中即时结算（原版经 RM_DELAYMAGIC 延时，延时队列同样已实现于 TCreature）。
/// </summary>
public partial class Magic
{
    private static readonly Random Rnd = new();
    private byte FNpcReleaseMagic;

    // ================= 威力系统（Magic.pas 顶层函数） =================

    /// <summary>MPow：技能基础威力（幸运≥上限取 MaxPower，否则 Power+Random(Max-Power)）。</summary>
    public static int MPow(TCreature baseObject, TUserMagicRef userMagic)
    {
        int powerSub;
        var info = userMagic.MagicInfo;
        if (info != null && info.wMaxPower >= info.wPower)
            powerSub = info.wMaxPower - info.wPower;
        else
            powerSub = 0;

        if (baseObject.m_nLuck > 0 && baseObject.m_nLuck >= M2Config.nMaxLuckMaxPower)
            return Math.Min(info != null ? info.wPower + powerSub : 0, int.MaxValue);
        return info != null ? info.wPower + Rnd.Next(Math.Max(powerSub, 0)) : 0;
    }

    /// <summary>GetPower：按等级/修炼级缩放 + 防御威力。</summary>
    public static int GetPower(TCreature baseObject, int nPower, TUserMagicRef? userMagic)
    {
        var info = userMagic?.MagicInfo;
        int defPowerSub;
        if (info != null && info.wDefMaxPower > info.wDefPower)
            defPowerSub = info.wDefMaxPower - info.wDefPower;
        else
            defPowerSub = 0;

        int scaled = info != null
            ? (int)Math.Round((double)nPower / (info.btTrainLv + 1) * (userMagic!.btLevel + 1))
            : nPower;

        if (baseObject.m_nLuck > 0 && baseObject.m_nLuck >= M2Config.nMaxLuckMaxPower)
            return (int)Math.Min((long)scaled + (info != null ? info.wDefPower + defPowerSub : 0), int.MaxValue);
        return (int)Math.Min((long)scaled + (info != null ? info.wDefPower + Rnd.Next(Math.Max(defPowerSub, 0)) : 0), int.MaxValue);
    }

    /// <summary>GetPower13：1/3 固定 + 2/3 按等级缩放（辅助类技能时间/威力）。</summary>
    public static int GetPower13(TCreature baseObject, int nInt, TUserMagicRef? userMagic)
    {
        double d10 = nInt / 3.0;
        double d18 = nInt - d10;
        var info = userMagic?.MagicInfo;
        if (baseObject.m_nLuck > 0 && baseObject.m_nLuck >= M2Config.nMaxLuckMaxPower)
        {
            long v = info != null
                ? (long)Math.Round(d18 / (info.btTrainLv + 1) * (userMagic!.btLevel + 1) + d10 + (info.wDefPower + Math.Max(info.wDefMaxPower - info.wDefPower, 0)))
                : (long)Math.Round(d18);
            return (int)Math.Min(v, int.MaxValue);
        }
        long v2 = info != null
            ? (long)Math.Round(d18 / (info.btTrainLv + 1) * (userMagic!.btLevel + 1) + d10 + (info.wDefPower + Rnd.Next(Math.Max(info.wDefMaxPower - info.wDefPower, 0))))
            : (long)Math.Round(d18);
        return (int)Math.Min(v2, int.MaxValue);
    }

    /// <summary>GetRPow：区间随机威力。</summary>
    public static int GetRPow(TCreature baseObject, int nInt1, int nInt2)
    {
        if (nInt2 > nInt1)
        {
            if (baseObject.m_nLuck > 0 && baseObject.m_nLuck >= M2Config.nMaxLuckMaxPower)
                return (int)Math.Min(Math.Max(nInt2 - nInt1 + 1, 1) + nInt1, int.MaxValue);
            return (int)Math.Min((long)Rnd.Next(nInt2 - nInt1 + 1) + nInt1, int.MaxValue);
        }
        return nInt1;
    }

    /// <summary>GetNewLevelPower：九重强化威力（含特定技能 Index 映射表，Magic.pas 170-247 全表）。</summary>
    public static int GetNewLevelPower(int nPower, TUserMagicRef? userMagic)
    {
        if (userMagic != null && userMagic.btNewLevel is >= 1 and <= 99 && nPower > 0)
        {
            int index;
            switch (userMagic.MagicInfo?.wMagicId ?? 0)
            {
                case 3: index = 0; break;   // 基本剑术
                case 7: index = 1; break;   // 攻杀剑术
                case 12: index = 2; break;  // 刺杀剑术
                case 25: index = 3; break;  // 半月弯刀
                case 26: index = 4; break;  // 烈火剑法
                case 56: index = 5; break;  // 逐日剑法
                case 58: index = 20; break; // 流星火雨
                case 22: index = 21; break; // 火墙
                case 31: index = 22; break; // 魔法盾
                case 11: index = 23; break; // 雷电术
                case 45: index = 24; break; // 灭天火
                case 33: index = 25; break; // 冰咆哮
                case 6: index = 40; break;  // 施毒术
                case 51: index = 40; break; // 群体施毒术
                case 14: index = 41; break; // 幽灵盾
                case 15: index = 42; break; // 神圣战甲术
                case 13: index = 43; break; // 灵魂火符
                case 57: index = 44; break; // 噬血术
                case 52: index = 45; break; // 飓风破
                default: index = -1; break;
            }

            long i64;
            if (index == -1)
            {
                if (userMagic.btNewLevel <= 9)
                    i64 = (long)Math.Round(nPower * (M2Config.NewLevelMagicPowerRates[userMagic.btNewLevel - 1] / 100.0));
                else
                {
                    double level9Power = nPower * (M2Config.NewLevelMagicPowerRates[9 - 1] / 100.0);
                    double rate = 1 + (userMagic.btNewLevel - 9) * (M2Config.NewLevelMagicPowerRatesAfter9 / 100.0);
                    i64 = (long)Math.Round(level9Power * rate);
                }
            }
            else
            {
                var table = M2Config.NewLevelMagicPowerRatesSpecific[index];
                if (userMagic.btNewLevel <= 9)
                    i64 = (long)Math.Round(nPower * (table[userMagic.btNewLevel - 1] / 100.0));
                else
                {
                    double level9Power = nPower * (table[9 - 1] / 100.0);
                    double rate = 1 + (userMagic.btNewLevel - 9) * (M2Config.NewLevelMagicPowerRatesAfter9 / 100.0);
                    i64 = (long)Math.Round(level9Power * rate);
                }
            }
            return (int)Math.Min(i64, int.MaxValue);
        }
        return nPower;
    }

    // ================= 技能分类 / CD =================

    /// <summary>IsWarrSkill：战士技能列表（Magic.pas 4623 全集）。</summary>
    public static bool IsWarrSkill(int wMagIdx)
    {
        switch (wMagIdx)
        {
            case MagicConst.SKILL_ONESWORD:    // 3 基本剑术
            case MagicConst.SKILL_ILKWANG:     // 4
            case MagicConst.SKILL_YEDO:        // 7
            case MagicConst.SKILL_ERGUM:       // 12 刺杀
            case MagicConst.SKILL_BANWOL:      // 25 半月
            case MagicConst.SKILL_FIRESWORD:   // 26 烈火
            case MagicConst.SKILL_MOOTEBO:     // 27 野蛮冲撞
            case MagicConst.SKILL_40:          // 40 双龙斩
            case MagicConst.SKILL_42:          // 42 龙影剑法
            case MagicConst.SKILL_43:          // 43 雷霆剑法
            case MagicConst.SKILL_56:          // 56 逐日剑法
            case MagicConst.SKILL_60:          // 60
            case MagicConst.SKILL_66:          // 66 开天斩
            case MagicConst.SKILL_100:
            case MagicConst.SKILL_101:
            case MagicConst.SKILL_102:
            case MagicConst.SKILL_103:
            case MagicConst.SKILL_113:         // 断空斩
                return true;
            default:
                return false;
        }
    }

    /// <summary>CheckCD：技能冷却（未冷却 false 并提示；boDoNotCheck 跳过）。</summary>
    public static bool CheckCD(TCreature baseObject, uint skillId, uint cdTime, bool boDoNotCheck)
    {
        if (boDoNotCheck)
            return true;
        uint t = DelphiRTL.GetTickCount();
        if (skillId < baseObject.m_SkillUseTick.Length)
        {
            if (t - baseObject.m_SkillUseTick[skillId] >= cdTime)
            {
                baseObject.m_SkillUseTick[skillId] = t;
                return true;
            }
            return false;
        }
        return false;
    }

    // ================= 技能处理 =================

    /// <summary>MagPushArround：抗拒火环/气功波推挤周围目标，返回推动数。</summary>
    public int MagPushArround(TCreature playObject, TUserMagicRef userMagic, int nPushLevel)
    {
        int result = 0;
        bool boPushSameLevel = userMagic.wMagIdx == MagicConst.SKILL_FIREWIND
            ? M2Config.boFireWindPushSameLevel
            : M2Config.boQigongPushSameLevel;

        foreach (var baseObject in playObject.m_VisibleActors.ToArray())
        {
            if (baseObject == null) continue;
            if (Math.Abs(playObject.m_nCurrX - baseObject.m_nCurrX) <= 1 &&
                Math.Abs(playObject.m_nCurrY - baseObject.m_nCurrY) <= 1)
            {
                if (!baseObject.m_boDeath && !ReferenceEquals(baseObject, playObject) && !baseObject.m_boStickMode)
                {
                    if (playObject.m_wAbil.Level > baseObject.m_wAbil.Level ||
                        (boPushSameLevel && playObject.m_wAbil.Level == baseObject.m_wAbil.Level))
                    {
                        int levelgap = (int)(playObject.m_wAbil.Level - baseObject.m_wAbil.Level);
                        int nValue = boPushSameLevel && playObject.m_wAbil.Level == baseObject.m_wAbil.Level
                            ? Rnd.Next(10)
                            : Rnd.Next(20);
                        if (nValue < 6 + nPushLevel * 3 + levelgap)
                        {
                            if (playObject.IsProperTarget(baseObject))
                            {
                                int push = 1 + Math.Max(0, nPushLevel - 1) + Rnd.Next(2);
                                byte nDir = TCreature.GetNextDirection(playObject.m_nCurrX, playObject.m_nCurrY, baseObject.m_nCurrX, baseObject.m_nCurrY);
                                baseObject.CharPushed(nDir, push);
                                result++;
                            }
                        }
                    }
                }
            }
        }
        return result;
    }

    /// <summary>MagBigHealing：群体治疗（3x3 友方回血延时消息）。</summary>
    public bool MagBigHealing(TCreature playObject, int nPower, int nX, int nY)
    {
        bool result = false;
        if (playObject.m_PEnvir == null) return false;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                foreach (var obj in playObject.m_PEnvir.GetObjects(nX + dx, nY + dy))
                {
                    if (obj is TCreature baseObject && playObject.IsProperFriend(baseObject))
                    {
                        if (baseObject.m_wAbil.HP < baseObject.m_wAbil.MaxHP)
                        {
                            baseObject.SendDelayMsg(playObject, Grobal2Const.RM_MAGHEALING, 0, nPower, 0, MagicConst.SKILL_BIGHEALLING, "", 800);
                            result = true;
                        }
                    }
                }
            }
        }
        return result;
    }

    /// <summary>MagTreatment：治愈术（单体友方回血）。</summary>
    public bool MagTreatment(TCreature playObject, TUserMagicRef userMagic, ref int nTargetX, ref int nTargetY, ref TCreature? targeTBaseObject)
    {
        bool result = false;
        if (targeTBaseObject == null)
        {
            targeTBaseObject = playObject;
            nTargetX = playObject.m_nCurrX;
            nTargetY = playObject.m_nCurrY;
        }
        if (playObject.IsProperFriend(targeTBaseObject))
        {
            int nPower = playObject.GetAttackPower(
                GetPower(playObject, MPow(playObject, userMagic), userMagic) + playObject.m_wAbil.SC1 * 2,
                (int)(playObject.m_wAbil.SC2 - playObject.m_wAbil.SC1) * 2 + 1);
            nPower = GetNewLevelPower(nPower, userMagic);
            if (targeTBaseObject.m_wAbil.HP < targeTBaseObject.m_wAbil.MaxHP)
            {
                targeTBaseObject.SendDelayMsg(playObject, Grobal2Const.RM_MAGHEALING, 0, nPower, 0, userMagic.wMagIdx, "", 800);
                result = true;
            }
        }
        return result;
    }

    /// <summary>MagMakeFireball：火球术（单目标魔法伤害）。</summary>
    public bool MagMakeFireball(TCreature playObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, ref TCreature? targeTBaseObject)
    {
        bool result = false;
        if (!playObject.MagCanHitTarget(playObject.m_nCurrX, playObject.m_nCurrY, targeTBaseObject))
        {
            targeTBaseObject = null;
            return false;
        }
        var target = targeTBaseObject;
        if (target == null || !playObject.IsProperTarget(target))
        {
            targeTBaseObject = null;
            return false;
        }
        if (Math.Abs(target.m_nCurrX - nTargetX) <= 1 && Math.Abs(target.m_nCurrY - nTargetY) <= 1)
        {
            if (target.m_nAntiMagic <= Rnd.Next(10))
            {
                int nPower = playObject.GetAttackPower(
                    GetPower(playObject, MPow(playObject, userMagic), userMagic) + playObject.m_wAbil.MC1,
                    Math.Max((int)(playObject.m_wAbil.MC2 - playObject.m_wAbil.MC1), 1));
                nPower = GetNewLevelPower(nPower, userMagic);
                target.StruckDamage(nPower);   // 原版经 RM_DELAYMAGIC 延时结算
                if (target.m_btRaceServer >= Grobal2Const.RC_ANIMAL)
                    result = true;
                if (playObject.m_btRaceServer is Grobal2Const.RC_HEROOBJECT or Grobal2Const.RC_PLAYMOSTER ||
                    target.m_btRaceServer is Grobal2Const.RC_HEROOBJECT or Grobal2Const.RC_PLAYMOSTER)
                    result = true;
            }
            else
            {
                targeTBaseObject = null;
            }
        }
        else
        {
            targeTBaseObject = null;
        }
        return result;
    }

    /// <summary>MagMakeHellFire：地狱火（方向直线 5 格贯穿）。</summary>
    public bool MagMakeHellFire(TCreature playObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, TCreature? targeTBaseObject)
    {
        bool result = false;
        if (playObject.m_PEnvir == null) return false;
        byte n1C = TCreature.GetNextDirection(playObject.m_nCurrX, playObject.m_nCurrY, nTargetX, nTargetY);
        TCreature.GetNextPosition(playObject.m_nCurrX, playObject.m_nCurrY, n1C, 1, out int n14, out int n18);
        TCreature.GetNextPosition(playObject.m_nCurrX, playObject.m_nCurrY, n1C, 5, out nTargetX, out nTargetY);
        int nPower = playObject.GetAttackPower(
            GetPower(playObject, MPow(playObject, userMagic), userMagic) + playObject.m_wAbil.MC1,
            (int)(playObject.m_wAbil.MC2 - playObject.m_wAbil.MC1) + 1);
        nPower = GetNewLevelPower(nPower, userMagic);
        if (playObject.MagPassThroughMagic(n14, n18, nTargetX, nTargetY, n1C, nPower, userMagic.wMagIdx, false) > 0)
            result = true;
        return result;
    }

    /// <summary>MagMakeQuickLighting：疾光电影（方向直线 8 格贯穿）。</summary>
    public bool MagMakeQuickLighting(TCreature playObject, TUserMagicRef userMagic, ref int nTargetX, ref int nTargetY, TCreature? targeTBaseObject)
    {
        bool result = false;
        if (playObject.m_PEnvir == null) return false;
        byte n1C = TCreature.GetNextDirection(playObject.m_nCurrX, playObject.m_nCurrY, nTargetX, nTargetY);
        TCreature.GetNextPosition(playObject.m_nCurrX, playObject.m_nCurrY, n1C, 1, out int n14, out int n18);
        TCreature.GetNextPosition(playObject.m_nCurrX, playObject.m_nCurrY, n1C, 8, out nTargetX, out nTargetY);
        int nPower = playObject.GetAttackPower(
            GetPower(playObject, MPow(playObject, userMagic), userMagic) + playObject.m_wAbil.MC1,
            (int)(playObject.m_wAbil.MC2 - playObject.m_wAbil.MC1) + 1);
        nPower = GetNewLevelPower(nPower, userMagic);
        if (playObject.MagPassThroughMagic(n14, n18, nTargetX, nTargetY, n1C, nPower, userMagic.wMagIdx, true) > 0)
            result = true;
        return result;
    }

    /// <summary>MagMakeLighting：雷电术（单体；不死系 1.5 倍）。</summary>
    public bool MagMakeLighting(TCreature playObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, ref TCreature? targeTBaseObject)
    {
        bool result = false;
        var target = targeTBaseObject;
        if (target == null) return false;
        if (playObject.IsProperTarget(target))
        {
            if (Rnd.Next(10) >= target.m_nAntiMagic)
            {
                int nPower = playObject.GetAttackPower(
                    GetPower(playObject, MPow(playObject, userMagic), userMagic) + playObject.m_wAbil.MC1,
                    (int)(playObject.m_wAbil.MC2 - playObject.m_wAbil.MC1) + 1);
                if (target.m_btLifeAttrib == Grobal2Const.LA_UNDEAD)
                    nPower = (int)Math.Min(Math.Round(nPower * 1.5), int.MaxValue);
                nPower = GetNewLevelPower(nPower, userMagic);
                nPower = (int)Math.Round(nPower / 100.0 * M2Config.nSkillLighteningPowerRate);
                target.StruckDamage(nPower);
                result = true;
            }
        }
        return result;
    }

    /// <summary>MagAbsorbBlood：噬血术（伤害 + 按比例吸血）。</summary>
    public bool MagAbsorbBlood(TCreature baseObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, ref TCreature? targeTBaseObject)
    {
        bool result = false;
        var target = targeTBaseObject;
        if (target == null || !baseObject.IsProperTarget(target)) return false;
        if (Rnd.Next(10) >= target.m_nAntiMagic)
        {
            if ((Math.Abs(target.m_nCurrX - nTargetX) <= 1 && Math.Abs(target.m_nCurrY - nTargetY) <= 1) ||
                baseObject.m_btRaceServer == Grobal2Const.RC_HEROOBJECT)
            {
                int nPower = baseObject.GetAttackPower(
                    GetPower(baseObject, MPow(baseObject, userMagic), userMagic) + baseObject.m_wAbil.SC1 * 2,
                    (int)(baseObject.m_wAbil.SC2 - baseObject.m_wAbil.SC1) * 2 + 1);
                nPower = GetNewLevelPower(nPower, userMagic);
                nPower = (int)Math.Min((long)Math.Round(nPower / 100.0 * M2Config.nSkill57PowerRate), int.MaxValue);
                target.StruckDamage(nPower);
                // 吸血
                if (baseObject.m_wAbil.HP < baseObject.m_wAbil.MaxHP)
                {
                    int nAddHP = (int)Math.Min((long)Math.Round(nPower / 100.0 * M2Config.nSkill57AddHPRate), int.MaxValue);
                    baseObject.SendDelayMsg(null, Grobal2Const.RM_INCHEALTH, nAddHP, 0, 0, 0, "", 1000);
                    baseObject.ProcessDelayedMessages();
                }
                if (baseObject.m_btRaceServer is Grobal2Const.RC_PLAYMOSTER or Grobal2Const.RC_HEROOBJECT)
                    result = true;
                else if (baseObject.m_btRaceServer == Grobal2Const.RC_PLAYOBJECT && target.m_btRaceServer >= Grobal2Const.RC_ANIMAL)
                    result = true;
            }
        }
        return result;
    }

    /// <summary>MagTurnUndead：圣言术（不死系按等级即死/重击）。</summary>
    public bool MagTurnUndead(TCreature baseObject, TCreature? targeTBaseObject, int nTargetX, int nTargetY, int nLevel)
    {
        bool result = false;
        var target = targeTBaseObject;
        if (target == null || !baseObject.IsProperTarget(target)) return false;
        if (Rnd.Next(10) >= target.m_nAntiMagic)
        {
            if (target.m_btLifeAttrib == Grobal2Const.LA_UNDEAD)
            {
                // 原版：不死系按威力结算（此处即时结算）
                int nPower = baseObject.GetAttackPower(baseObject.m_wAbil.MC1 + nLevel * 5,
                    (int)(baseObject.m_wAbil.MC2 - baseObject.m_wAbil.MC1) + 1);
                target.StruckDamage(nPower);
                result = true;
            }
        }
        else if (baseObject.m_btRaceServer == Grobal2Const.RC_PLAYOBJECT)
        {
            // MISS
        }
        return result;
    }

    /// <summary>MagBigExplosion：爆裂火焰/范围伤害（GetMapBaseObjects(range) 全体），返回命中数。</summary>
    public int MagBigExplosion(TCreature baseObject, TUserMagicRef userMagic, int nPower, int nX, int nY, int nRange, int powerRate)
    {
        int result = 0;
        if (baseObject.m_PEnvir == null) return 0;
        for (int dx = -nRange; dx <= nRange; dx++)
        {
            for (int dy = -nRange; dy <= nRange; dy++)
            {
                foreach (var obj in baseObject.m_PEnvir.GetObjects(nX + dx, nY + dy))
                {
                    if (obj is TCreature target && baseObject.IsProperTarget(target))
                    {
                        int dmg = nPower * powerRate / 100;
                        target.StruckDamage(dmg);
                        result++;
                    }
                }
            }
        }
        return result;
    }

    /// <summary>MagElecBlizzard：地狱雷光（全屏随机目标）。</summary>
    public bool MagElecBlizzard(TCreature baseObject, TUserMagicRef userMagic, int nPower)
    {
        bool result = false;
        if (baseObject.m_PEnvir == null || baseObject.m_VisibleActors.Count == 0) return false;
        foreach (var target in baseObject.m_VisibleActors.ToArray())
        {
            if (baseObject.IsProperTarget(target))
            {
                target.StruckDamage(nPower);
                result = true;
            }
        }
        return result;
    }

    /// <summary>MagMakeFireCharm：灵魂火符（单体符伤害）。</summary>
    public bool MagMakeFireCharm(TCreature baseObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, ref TCreature? targeTBaseObject, bool boMove)
    {
        bool result = false;
        var target = targeTBaseObject;
        if (target == null || !baseObject.IsProperTarget(target)) return false;
        if (Math.Abs(target.m_nCurrX - nTargetX) <= 1 && Math.Abs(target.m_nCurrY - nTargetY) <= 1)
        {
            if (Rnd.Next(10) >= target.m_nAntiMagic)
            {
                int nPower = baseObject.GetAttackPower(
                    GetPower(baseObject, MPow(baseObject, userMagic), userMagic) + baseObject.m_wAbil.SC1 * 2,
                    (int)(baseObject.m_wAbil.SC2 - baseObject.m_wAbil.SC1) * 2 + 1);
                nPower = GetNewLevelPower(nPower, userMagic);
                target.StruckDamage(nPower);
                result = target.m_btRaceServer >= Grobal2Const.RC_ANIMAL;
            }
            else
            {
                targeTBaseObject = null;
            }
        }
        else
        {
            targeTBaseObject = null;
        }
        return result;
    }

    // ================= DoSpell 总分发（对应 Magic.pas 4665 起 if/else 链主干） =================

    public bool DoSpell(TCreature baseObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, TCreature? targeTBaseObject)
        => DoSpell(baseObject, userMagic, nTargetX, nTargetY, ref targeTBaseObject);

    public bool DoSpell(TCreature baseObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, ref TCreature? targeTBaseObject)
    {
        bool boTrain;
        if (IsWarrSkill(userMagic.wMagIdx))
            return false;

        FNpcReleaseMagic = 0;
        int nMagicAttackRage = M2Config.boViewRangeCanMagicAttack ? baseObject.m_VisibleActors.Count + 8 : M2Config.nMagicAttackRage;
        if (userMagic.wMagIdx == MagicConst.SKILL_204)
            nMagicAttackRage = Math.Max(nMagicAttackRage, M2Config.nSkill204Distance);

        if (userMagic.wMagIdx == MagicConst.SKILL_HEALLING && targeTBaseObject == null)
            targeTBaseObject = baseObject;

        boTrain = false;
        switch (userMagic.wMagIdx)
        {
            case MagicConst.SKILL_FIREBALL:      // 火球术
            case MagicConst.SKILL_FIREBALL2:     // 大火球
            {
                if (MagMakeFireball(baseObject, userMagic, nTargetX, nTargetY, ref targeTBaseObject))
                    boTrain = true;
                break;
            }
            case MagicConst.SKILL_HEALLING:      // 治愈术
            {
                int tx = nTargetX, ty = nTargetY;
                if (MagTreatment(baseObject, userMagic, ref tx, ref ty, ref targeTBaseObject))
                    boTrain = true;
                break;
            }
            case MagicConst.SKILL_AMYOUNSUL:     // 施毒术
            {
                if (CheckCD(baseObject, MagicConst.SKILL_AMYOUNSUL, baseObject.GetMagicCD(MagicConst.SKILL_AMYOUNSUL), false))
                {
                    TCreature? t = targeTBaseObject;
                    if (MagMakeLighting(baseObject, userMagic, nTargetX, nTargetY, ref t))
                        boTrain = true;
                }
                break;
            }
            case MagicConst.SKILL_FIREWIND:      // 抗拒火环
            {
                if (MagPushArround(baseObject, userMagic, userMagic.btLevel) > 0)
                    boTrain = true;
                break;
            }
            case MagicConst.SKILL_FIRE:          // 地狱火
            {
                if (MagMakeHellFire(baseObject, userMagic, nTargetX, nTargetY, targeTBaseObject))
                    boTrain = true;
                break;
            }
            case MagicConst.SKILL_SHOOTLIGHTEN:  // 疾光电影
            {
                int tx = nTargetX, ty = nTargetY;
                if (MagMakeQuickLighting(baseObject, userMagic, ref tx, ref ty, targeTBaseObject))
                    boTrain = true;
                break;
            }
            case MagicConst.SKILL_LIGHTENING:    // 雷电术
            {
                TCreature? t = targeTBaseObject;
                if (MagMakeLighting(baseObject, userMagic, nTargetX, nTargetY, ref t))
                    boTrain = true;
                break;
            }
            case MagicConst.SKILL_FIRECHARM:     // 灵魂火符
            {
                TCreature? t = targeTBaseObject;
                if (MagMakeFireCharm(baseObject, userMagic, nTargetX, nTargetY, ref t, false))
                    boTrain = true;
                break;
            }
            case MagicConst.SKILL_57:            // 噬血术
            {
                if (CheckCD(baseObject, MagicConst.SKILL_57, baseObject.GetMagicCD(MagicConst.SKILL_57), false))
                {
                    TCreature? t = targeTBaseObject;
                    if (MagAbsorbBlood(baseObject, userMagic, nTargetX, nTargetY, ref t))
                        boTrain = true;
                }
                break;
            }
            case MagicConst.SKILL_BIGHEALLING:   // 群体治疗
            {
                if (MagBigHealing(baseObject, GetPower13(baseObject, 60, userMagic) + baseObject.m_wAbil.SC1 * 2, nTargetX, nTargetY))
                    boTrain = true;
                break;
            }
            case MagicConst.SKILL_FIREBOOM:      // 爆裂火焰
            {
                int nPower = baseObject.GetAttackPower(
                    GetPower(baseObject, MPow(baseObject, userMagic), userMagic) + baseObject.m_wAbil.MC1,
                    (int)(baseObject.m_wAbil.MC2 - baseObject.m_wAbil.MC1) + 1);
                nPower = GetNewLevelPower(nPower, userMagic);
                if (MagBigExplosion(baseObject, userMagic, nPower, nTargetX, nTargetY, 1, 100) > 0)
                    boTrain = true;
                break;
            }
            case MagicConst.SKILL_LIGHTFLOWER:   // 地狱雷光
            {
                int nPower = baseObject.GetAttackPower(
                    GetPower(baseObject, MPow(baseObject, userMagic), userMagic) + baseObject.m_wAbil.MC1,
                    (int)(baseObject.m_wAbil.MC2 - baseObject.m_wAbil.MC1) + 1);
                nPower = GetNewLevelPower(nPower, userMagic);
                if (MagElecBlizzard(baseObject, userMagic, nPower))
                    boTrain = true;
                break;
            }
            case MagicConst.SKILL_45:            // 灭天火
            {
                TCreature? t45 = targeTBaseObject;
                if (MagMakeFireDay(baseObject, userMagic, nTargetX, nTargetY, ref t45))
                    boTrain = true;
                break;
            }
            case MagicConst.SKILL_58:            // 流星火雨
                boTrain = MagMeteoriteRain(baseObject, userMagic, nTargetX, nTargetY);
                break;
            case MagicConst.SKILL_69:            // 禁锢
                boTrain = MagMakeImprison(baseObject, 10, 2, nTargetX, nTargetY) > 0;
                break;
            case MagicConst.SKILL_SKELLETON:     // 召唤骷髅（原版经护身符分支，此处直呼）
            {
                if (MagMakeSlave(baseObject, userMagic))
                    boTrain = true;
                break;
            }
            case MagicConst.SKILL_SINSU:         // 召唤神兽
            {
                if (MagMakeSinSuSlave(baseObject, userMagic))
                    boTrain = true;
                break;
            }
            case MagicConst.SKILL_76:            // 召唤圣兽
            {
                if (MagMakeBigDogSlave(baseObject, userMagic))
                    boTrain = true;
                break;
            }
            case MagicConst.SKILL_55:            // 召唤月灵
            {
                if (MagMakeMoon(baseObject, userMagic))
                    boTrain = true;
                break;
            }
            case MagicConst.SKILL_GROUPLIGHTENING: // 群体雷电术
            {
                bool boSpellFire37 = false;
                if (MagGroupLightening(baseObject, userMagic, nTargetX, nTargetY, targeTBaseObject, ref boSpellFire37))
                    boTrain = true;
                break;
            }
            case MagicConst.SKILL_GROUPAMYOUNSUL: // 群体施毒术
            {
                bool boSpellFail51 = false;
                if (MagGroupAmyounsul(baseObject, userMagic, nTargetX, nTargetY, targeTBaseObject, ref boSpellFail51))
                    boTrain = true;
                break;
            }
            case MagicConst.SKILL_GROUPDEDING:   // 彻地钉
            {
                if (MagGroupDeDing(baseObject, userMagic, nTargetX, nTargetY, targeTBaseObject))
                    boTrain = true;
                break;
            }
            case 60: // 静之破魂斩（原版空桩）
                boTrain = MagMakeSkillFire_60(baseObject, userMagic, targeTBaseObject);
                break;
            case 61: // 劈星斩
            {
                TCreature? t61 = targeTBaseObject;
                if (MagMakeSkillFire_61(baseObject, userMagic, nTargetX, nTargetY, ref t61))
                    boTrain = true;
                break;
            }
            case 62: // 雷霆一击
                boTrain = MagMakeSkillFire_62(baseObject, userMagic, nTargetX, nTargetY, targeTBaseObject);
                break;
            case 63: // 噬魂沼泽
                boTrain = MagMakeSkillFire_63(baseObject, userMagic, nTargetX, nTargetY, targeTBaseObject);
                break;
            case 64: // 末日审判
                boTrain = MagMakeSkillFire_64(baseObject, userMagic, nTargetX, nTargetY, targeTBaseObject);
                break;
            case 65: // 火龙气焰
                boTrain = MagMakeSkillFire_65(baseObject, userMagic, nTargetX, nTargetY);
                break;
            case 104: // 凤舞祭
            {
                TCreature? t104 = targeTBaseObject;
                if (MagMakeSkill104(baseObject, userMagic, nTargetX, nTargetY, ref t104))
                    boTrain = true;
                break;
            }
            case 105: // 惊雷爆
            {
                TCreature? t105 = targeTBaseObject;
                boTrain = MagMakeSkill105(baseObject, userMagic, nTargetX, nTargetY, t105);
                break;
            }
            case 106: // 冰天雪地
                boTrain = MagMakeSkill106(baseObject, userMagic, nTargetX, nTargetY);
                break;
            case 107: // 双龙破
            {
                TCreature? t107 = targeTBaseObject;
                if (MagMakeSkill107(baseObject, userMagic, nTargetX, nTargetY, ref t107))
                    boTrain = true;
                break;
            }
            case 108: // 虎啸诀
            {
                TCreature? t108 = targeTBaseObject;
                if (MagMakeSkill108(baseObject, userMagic, nTargetX, nTargetY, ref t108))
                    boTrain = true;
                break;
            }
            case 109: // 八卦掌
            {
                TCreature? t109 = targeTBaseObject;
                if (MagMakeSkill109(baseObject, userMagic, nTargetX, nTargetY, ref t109))
                    boTrain = true;
                break;
            }
            case 110: // 三焰咒
            {
                TCreature? t110 = targeTBaseObject;
                if (MagMakeSkill110(baseObject, userMagic, nTargetX, nTargetY, ref t110))
                    boTrain = true;
                break;
            }
            case 111: // 万剑归宗
                boTrain = MagMakeSkill111(baseObject, userMagic, nTargetX, nTargetY);
                break;
            case 114: // 倚天辟地
            {
                int tx = nTargetX, ty = nTargetY;
                if (MagMakeSkill114(baseObject, userMagic, ref tx, ref ty))
                    boTrain = true;
                break;
            }
            case 116: // 血魄一击
                boTrain = MagMakeSkill116(baseObject, userMagic, nTargetX, nTargetY);
                break;
            case 117:
                boTrain = MagMakeSkill117(baseObject, userMagic, nTargetX, nTargetY);
                break;
            default:
                // 自定义技能（wMagicId >= CUSTOM_MAGIC_START_ID）→ 配置驱动框架
                if (userMagic.wMagIdx >= Grobal2Const.CUSTOM_MAGIC_START_ID)
                {
                    string sMsg = "";
                    bool boMoved = false, boSpellFire = false;
                    TCreature? t = targeTBaseObject;
                    boTrain = MagCustomSkillFull(baseObject, userMagic, nTargetX, nTargetY, ref t, ref sMsg, ref boMoved, 0, ref boSpellFire);
                }
                else
                {
                    // 其余基础技能由 MagCustomSkill 与后续批次处理器接管
                    boTrain = MagCustomSkill(baseObject, userMagic, nTargetX, nTargetY, ref targeTBaseObject);
                }
                break;
        }
        return boTrain;
    }

    /// <summary>MagCustomSkill：自定义/扩展技能入口（对应 MagCustomSkill 框架位）。</summary>
    public bool MagCustomSkill(TCreature baseObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, ref TCreature? targeTBaseObject)
    {
        // 自定义技能（wMagicId >= CUSTOM_MAGIC_START_ID）由自定义技能配置驱动；基础技能集之外的默认返回失败
        return false;
    }
}
