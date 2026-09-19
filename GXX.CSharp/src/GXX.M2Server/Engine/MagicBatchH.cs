using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Rtl;

namespace GXX.M2Server.Engine;

/// <summary>Magic.pas 批次H 配置扩展（g_Config 字段子集）。</summary>
public static partial class M2Config
{
    // ---- 灭天火（SKILL_45）----
    public static int nMakeFireDayPowerRate = 100;
    public static bool boPlayObjectReduceMP = true;
    public static uint dwHeroGotoLV4 = 100000;
    public static uint dwHeroPowerLV4 = 50;
    public static uint dwHumSkill45PowerLV4 = 30;

    // ---- 流星火雨（SKILL_58）----
    public static int nSkill58AttackRange = 3;
    public static int nSkill58PowerRate = 100;
    public static bool boSkill58PowerTwoAttack;

    // ---- 禁锢（SKILL_69）----
    public static bool boSkill69SameLevel = true;

    public static void ResetMagicBatchHDefaults()
    {
        nMakeFireDayPowerRate = 100;
        boPlayObjectReduceMP = true;
        nSkill58AttackRange = 3;
        nSkill58PowerRate = 100;
        boSkill58PowerTwoAttack = false;
        boSkill69SameLevel = true;
        CustomMagicConfigs.Clear();
    }
}

/// <summary>自定义技能目标图案（TCustomMagicConfig.ServerConfig 对应子集）。</summary>
public enum CustomTargetPattern : byte
{
    SingleTarget = 0,          // 单体
    GroupAroundTarget = 1,     // 以目标点为中心范围
    GroupAroundSelf = 2,       // 以自身为中心范围
    None = 3
}

/// <summary>自定义技能服务端配置（TCustomMagicConfig.ServerConfig 子集）。</summary>
public class TCustomMagicServerConfig
{
    public bool DisableInSafeZone;
    public CustomTargetPattern TargetPattern = CustomTargetPattern.SingleTarget;
    public int AttackRange = 2;
    public int PowerRate = 100;
    public int AddHP;
    public int AddMP;
    public bool IsTeleport;
    public bool PoisonApply;
    public int PoisonTime = 100;
}

/// <summary>TCustomMagicConfig：自定义技能定义（M2Definition.pas GetCustomMagicConfig 对应）。</summary>
public class TCustomMagicConfig
{
    public ushort wMagicId;
    public string sMagicName = "";
    public TCustomMagicServerConfig ServerConfig = new();

    public TCustomMagicConfig(ushort magicId) { wMagicId = magicId; }

    // ---- ServerConfig 便捷转发（与 Magic.pas 中 ServerConfig.<field> 直取习惯一致） ----
    public bool DisableInSafeZone
    {
        get => ServerConfig.DisableInSafeZone;
        set => ServerConfig.DisableInSafeZone = value;
    }
    public CustomTargetPattern TargetPattern
    {
        get => ServerConfig.TargetPattern;
        set => ServerConfig.TargetPattern = value;
    }
    public int AttackRange
    {
        get => ServerConfig.AttackRange;
        set => ServerConfig.AttackRange = value;
    }
    public int PowerRate
    {
        get => ServerConfig.PowerRate;
        set => ServerConfig.PowerRate = value;
    }
    public int AddHP
    {
        get => ServerConfig.AddHP;
        set => ServerConfig.AddHP = value;
    }
    public int AddMP
    {
        get => ServerConfig.AddMP;
        set => ServerConfig.AddMP = value;
    }
    public bool IsTeleport
    {
        get => ServerConfig.IsTeleport;
        set => ServerConfig.IsTeleport = value;
    }
    public bool PoisonApply
    {
        get => ServerConfig.PoisonApply;
        set => ServerConfig.PoisonApply = value;
    }
    public int PoisonTime
    {
        get => ServerConfig.PoisonTime;
        set => ServerConfig.PoisonTime = value;
    }
}

/// <summary>M2Config 扩展：自定义技能注册表（GetCustomMagicConfig）。</summary>
public static partial class M2Config
{
    public static readonly Dictionary<int, TCustomMagicConfig> CustomMagicConfigs = new();

    public static void RegisterCustomMagic(TCustomMagicConfig cfg)
        => CustomMagicConfigs[cfg.wMagicId] = cfg;

    public static TCustomMagicConfig? GetCustomMagicConfig(ushort magicId)
        => CustomMagicConfigs.TryGetValue(magicId, out var cfg) ? cfg : null;
}

/// <summary>TCreature 状态扩展（批次H：灭天火扣蓝/捆魔/禁锢/安全区）。</summary>
public partial class TCreature
{
    public bool m_boHolySeize;           // 捆魔咒定身
    public uint m_dwHolySeizeTime;
    public bool m_boImprison;            // 禁锢
    public uint m_dwImprisonTick;
    public uint m_dwImprisonTime;
    public int m_nImprisonRange;
    public int m_nImprisonX;
    public int m_nImprisonY;
    public bool m_boUnImprison;          // 免疫禁锢
    public bool m_boInSafeZone;          // 简化安全区标记（由引擎安全区管理器维护）
    public double m_rLoyalPoint;         // 英雄忠诚度（THeroObject 子集）

    /// <summary>DamageSpell：扣魔法值（灭天火同步扣蓝）。</summary>
    public void DamageSpell(int nDamage)
    {
        m_wAbil.MP = (uint)Math.Max(0, m_wAbil.MP - Math.Max(nDamage, 0));
    }

    /// <summary>OpenHolySeizeMode：捆魔定身（原 ObjBase.OpenHolySeizeMode）。</summary>
    public void OpenHolySeizeMode(uint dwTime)
    {
        m_boHolySeize = true;
        m_dwHolySeizeTime = dwTime;
    }
}

/// <summary>
/// Magic.pas 批次H 处理器（partial Magic）：
/// 灭天火（MagMakeFireDay 45）、流星火雨（MagMeteoriteRain 58）、
/// 捆魔咒（MagMakeHolyCurtain）、禁锢（MagMakeImprison）、
/// 自定义技能配置驱动框架（MagCustomSkillFull）。
/// </summary>
public partial class Magic
{
    // ================= 灭天火（SKILL_45） =================

    /// <summary>灭天火：单体魔法伤害 + 扣蓝（不死 1.5x / 英雄4级忠诚加成 / nMakeFireDayPowerRate）。</summary>
    public bool MagMakeFireDay(TCreature playObject, TUserMagicRef userMagic, int nTargetX, int nTargetY, ref TCreature? targeTBaseObject)
    {
        bool result = false;
        var target = targeTBaseObject;
        if (target == null || !playObject.IsProperTarget(target))
        {
            targeTBaseObject = null;
            return false;
        }
        if (Rnd.Next(10) >= target.m_nAntiMagic)
        {
            int nPower = playObject.GetAttackPower(
                GetPower(playObject, MPow(playObject, userMagic), userMagic) + playObject.m_wAbil.MC1,
                (int)(playObject.m_wAbil.MC2 - playObject.m_wAbil.MC1) + 1);
            if (target.m_btLifeAttrib == Grobal2Const.LA_UNDEAD)
                nPower = (int)Math.Min(Math.Round(nPower * 1.5), int.MaxValue);
            nPower = GetNewLevelPower(nPower, userMagic);

            // 英雄 4 级忠诚加成 / 人物 4 级加成（dwHeroPowerLV4 / dwHumSkill45PowerLV4）
            if (playObject.m_btRaceServer == Grobal2Const.RC_HEROOBJECT && userMagic.btLevel == 4 &&
                playObject.m_rLoyalPoint >= M2Config.dwHeroGotoLV4 / 100.0)
            {
                nPower += (int)Math.Round(nPower * (M2Config.dwHeroPowerLV4 / 100.0));
            }
            else if (playObject.m_btRaceServer == Grobal2Const.RC_PLAYOBJECT && userMagic.btLevel == 4)
            {
                nPower += (int)Math.Round(nPower * (M2Config.dwHumSkill45PowerLV4 / 100.0));
            }

            // nMakeFireDayPowerRate 倍率
            nPower = (int)Math.Round(nPower * (M2Config.nMakeFireDayPowerRate / 100.0));

            target.StruckDamage(nPower);
            if (M2Config.boPlayObjectReduceMP)
                target.DamageSpell(nPower);

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
        return result;
    }

    // ================= 流星火雨（SKILL_58） =================

    /// <summary>流星火雨：以落点为中心范围伤害（nSkill58Range/PowerRate/双段攻击选项）。</summary>
    public bool MagMeteoriteRain(TCreature baseObject, TUserMagicRef userMagic, int nX, int nY)
    {
        bool result = false;
        if (baseObject.m_PEnvir == null) return false;

        // 原实现的 Min(AttackRange + level, AttackRange) 恒等于 AttackRange（保真保留）
        int nRage = Math.Min(M2Config.nSkill58AttackRange + userMagic.btLevel, M2Config.nSkill58AttackRange);
        int nValue = baseObject.GetAttackPower(
            GetPower(baseObject, MPow(baseObject, userMagic), userMagic) + baseObject.m_wAbil.MC1,
            (int)(baseObject.m_wAbil.MC2 - baseObject.m_wAbil.MC1) + 1);
        nValue = (int)Math.Min(Math.Round(nValue * (M2Config.nSkill58PowerRate / 100.0)), int.MaxValue);
        nValue = GetNewLevelPower(nValue, userMagic);

        var objects = GetMapBaseObjects(baseObject.m_PEnvir, nX, nY, nRage);
        foreach (var target in objects)
        {
            if (target.m_boDeath || target.m_boGhost || !baseObject.IsProperTarget(target)) continue;

            if (RndEx.Next(10) >= target.m_nAntiMagic)
            {
                int nPower = nValue;
                if (target.m_btLifeAttrib == Grobal2Const.LA_UNDEAD)
                    nPower = (int)Math.Min(Math.Round(nPower * 1.5), int.MaxValue);
                target.StruckDamage(nPower);
                result = true;
            }
        }
        return result;
    }

    // ================= 捆魔咒（SKILL_HOLYSHIELD 分支调用） =================

    /// <summary>MagMakeHolyCurtain：捆魔咒——3x3 内低等级无主怪定身，返回成功数。</summary>
    public int MagMakeHolyCurtain(TCreature baseObject, int nPower, int nX, int nY)
    {
        int result = 0;
        if (baseObject.m_PEnvir == null) return 0;
        if (!baseObject.m_PEnvir.CanWalk(nX, nY)) return 0;

        var objects = GetMapBaseObjects(baseObject.m_PEnvir, nX, nY, 1);
        foreach (var obj in objects)
        {
            // 恶魔蝙蝠(127) 不判等级；普通怪需 (Random(4)+我方等级-1) > 目标等级 且无主人
            bool canSeize = obj.m_btRaceServer == 127 ||
                ((obj.m_btRaceServer >= Grobal2Const.RC_ANIMAL) &&
                 (RndEx.Next(4) + (baseObject.m_wAbil.Level - 1) > obj.m_wAbil.Level));
            if (canSeize && obj.m_Master == null)
            {
                obj.OpenHolySeizeMode((uint)nPower * 1000);
                result++;
            }
            else
                result = 0;
        }
        return result;
    }

    // ================= 禁锢（SKILL_69） =================

    /// <summary>MagMakeImprison：禁锢——范围内等级低于施法者的目标定身 nTime 秒，返回成功数。</summary>
    public int MagMakeImprison(TCreature baseObject, int nTime, int nRange, int nX, int nY)
    {
        int result = 0;
        if (baseObject.m_PEnvir == null) return 0;
        if (!baseObject.m_PEnvir.CanWalk(nX, nY)) return 0;

        var objects = GetMapBaseObjects(baseObject.m_PEnvir, nX, nY, nRange);
        foreach (var target in objects)
        {
            if (target.m_boDeath) continue;
            if (target.m_boUnImprison) continue;
            if (!baseObject.IsProperTarget(target)) continue;
            bool levelOk = target.m_wAbil.Level < baseObject.m_wAbil.Level ||
                           (M2Config.boSkill69SameLevel && target.m_wAbil.Level == baseObject.m_wAbil.Level);
            if (!levelOk) continue;

            target.m_dwImprisonTick = DelphiRTL.GetTickCount();
            target.m_dwImprisonTime = (uint)(nTime * 1000);
            target.m_nImprisonRange = nRange;
            target.m_nImprisonX = nX;
            target.m_nImprisonY = nY;
            target.m_boImprison = true;
            result++;
        }
        return result;
    }

    // ================= 自定义技能（MagCustomSkill 配置驱动框架） =================

    /// <summary>
    /// MagCustomSkillFull：自定义技能配置驱动框架（Magic.pas MagCustomSkill 对应）。
    /// 按注册的 TCustomMagicConfig 执行：安全区禁用检查 → 目标图案伤害 → 回血/回蓝/传送 → 毒状态。
    /// </summary>
    public bool MagCustomSkillFull(TCreature playObject, TUserMagicRef userMagic, int nTargetX, int nTargetY,
        ref TCreature? targeTBaseObject, ref string sMsg, ref bool boMoved, byte btNpcReleaseMagic, ref bool boSpellFire)
    {
        sMsg = "";
        boMoved = false;
        var cfg = M2Config.GetCustomMagicConfig(userMagic.wMagIdx);
        if (cfg == null)
            return false;    // 对应原 GetCustomMagicConfig = nil → Exit

        var server = cfg.ServerConfig;

        // 安全区禁用（施法点与目标点都检查）
        if (server.DisableInSafeZone)
        {
            if (playObject.m_boInSafeZone ||
                (targeTBaseObject != null && targeTBaseObject.m_boInSafeZone))
            {
                sMsg = "安全区禁止使用技能：" + userMagic.MagicInfo?.sMagicName;
                boSpellFire = false;
                return false;
            }
        }

        bool result = false;

        // 目标图案伤害
        if (server.TargetPattern != CustomTargetPattern.None && playObject.m_PEnvir != null)
        {
            int nPower = basePower(playObject, userMagic);
            int cx = server.TargetPattern == CustomTargetPattern.GroupAroundSelf ? playObject.m_nCurrX : nTargetX;
            int cy = server.TargetPattern == CustomTargetPattern.GroupAroundSelf ? playObject.m_nCurrY : nTargetY;
            int range = server.TargetPattern == CustomTargetPattern.SingleTarget ? 1 : server.AttackRange;

            var objects = server.TargetPattern == CustomTargetPattern.SingleTarget
                ? (targeTBaseObject != null ? new List<TCreature> { targeTBaseObject } : new List<TCreature>())
                : GetMapBaseObjects(playObject.m_PEnvir, cx, cy, range);

            foreach (var target in objects)
            {
                if (target.m_boDeath || target.m_boGhost) continue;
                if (!playObject.IsProperTarget(target)) continue;
                if (RndEx.Next(10) < target.m_nAntiMagic) continue;

                int dmg = (int)Math.Round(nPower * (server.PowerRate / 100.0));
                dmg = GetNewLevelPower(dmg, userMagic);
                target.StruckDamage(dmg);
                if (server.PoisonApply)
                    target.PoisonGreenTick += server.PoisonTime;
                result = true;
            }
        }

        // 恢复效果
        if (server.AddHP != 0)
            playObject.m_wAbil.HP = (uint)Math.Clamp(playObject.m_wAbil.HP + server.AddHP, 0, (long)playObject.m_wAbil.MaxHP);
        if (server.AddMP != 0)
            playObject.m_wAbil.MP = (uint)Math.Clamp(playObject.m_wAbil.MP + server.AddMP, 0, (long)playObject.m_wAbil.MaxMP);
        if (server.AddHP != 0 || server.AddMP != 0)
            result = true;

        // 传送
        if (server.IsTeleport && playObject.m_PEnvir.CanWalk(nTargetX, nTargetY))
        {
            playObject.m_PEnvir.DeleteFromMap(playObject.m_nCurrX, playObject.m_nCurrY, playObject);
            playObject.m_nCurrX = nTargetX;
            playObject.m_nCurrY = nTargetY;
            playObject.m_PEnvir.AddToMap(nTargetX, nTargetY, playObject);
            boMoved = true;
            result = true;
        }

        return result;

        static int basePower(TCreature obj, TUserMagicRef um)
            => MPow(obj, um);
    }
}
