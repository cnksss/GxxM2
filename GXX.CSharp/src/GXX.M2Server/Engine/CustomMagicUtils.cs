using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.M2Server.Engine;

// ============================================================================
// uCustomMagicUtils.pas 名称表与枚举子集 1:1 移植（批次J61）
// 供 uFrmCustomMagicCopySetting / uFrmHeroMagicSetting / uFrmCustomItemProperty /
// uFrmCustomMagic 等窗体使用。顺序即数组下标，与 Delphi 逐一对应，勿改。
// ============================================================================

/// <summary>uCustomMagicUtils.pas TCheckVarType 1:1。</summary>
public enum TCheckVarType
{
    cvtMoreThan,
    cvtLessThan,
    cvtEqual,
    cvtNotEqual,
    cvtQreaterEqual,
    cvtLessEqual,
}

/// <summary>uCustomMagicUtils.pas TMagicAttackDecAttributesType 1:1。</summary>
public enum TMagicAttackDecAttributesType
{
    daAC, daMAC, daDC, daMC, daSC, daHitPoint, daSpeedPoint, daAntiMagic, daAntiPoison,
}

/// <summary>uCustomMagicUtils.pas TMagicProtectAddAttributesType 1:1。</summary>
public enum TMagicProtectAddAttributesType
{
    aaAC, aaMAC, aaDC, aaMC, aaSC, aaHitPoint, aaSpeedPoint, aaAntiMagic, aaAntiPoison,
    aaNGDamage, aaNGDefense, aaHP, aaMP, aaMaxHP, aaMaxMP, aaHide,
}

/// <summary>uCustomMagicUtils.pas TItemElementsType 1:1（下标与 TAbility.NewValue 对齐）。</summary>
public enum TItemElementsType
{
    ietBlastHit, ietDamageAdd, ietDamageDec, ietSpellDamageDec, ietCloseDefense, ietDamageRebound,
    ietMonDropRate, ietMaxHPAdd, ietMaxMPAdd, ietAngryValueTimAdd, ietGroupDamageAdd, ietHuamDropRate,
    ietUndropRate, ietUnParalysis, ietUnMagicShield, ietUnRevival, ietUnPosion, ietUnTamming,
    ietUnFireCross, ietUnFrozen, ietUnCobwebWinding, ietFatalBlowRate, ietFatalBlowPower,
    ietFatalBlowDefense, ietUnBlastHit,
}

/// <summary>uCustomMagicUtils.pas TBreakDefenseType 1:1。</summary>
public enum TBreakDefenseType
{
    bdtHumDefense, bdtMonDefense, bdtHeroDefense, bdtHumMagDefense, bdtMonMagDefense, bdtHeroMagDefense,
}

/// <summary>
/// uCustomMagicUtils.pas 单元级常量与名称表（批次J61）。
/// MaxCustomMagicLevel=9；CustomMagicLevelNames[0..10]；其余表下标与枚举对齐。
/// </summary>
public static class CustomMagicUtils
{
    /// <summary>MaxCustomMagicLevel。</summary>
    public const int MaxCustomMagicLevel = 9;

    /// <summary>MagicWarrNGOptionNames（17 项：无 + 8 内置 + 8 自定义槽）。</summary>
    public static readonly string[] MagicWarrNGOptionNames =
    {
        "无", "半月弯弓", "烈火剑法", "逐日剑法", "龙影剑法", "刺杀剑术", "开天斩", "双龙斩",
        "断空斩", "自定义技能1", "自定义技能2", "自定义技能3", "自定义技能4",
        "自定义技能5", "自定义技能6", "自定义技能7", "自定义技能8",
    };

    /// <summary>CustomMagicLevelNames[0..10]（无强化 + 强化 1..9 重 + 9 重后每重增加）。</summary>
    public static readonly string[] CustomMagicLevelNames =
    {
        "无强化", "强化1重", "强化2重", "强化3重", "强化4重", "强化5重", "强化6重",
        "强化7重", "强化8重", "强化9重", "9重后每重增加",
    };

    /// <summary>MagicPlusLevelNames[TMagicPlusLevel]（4 项）。</summary>
    public static readonly string[] MagicPlusLevelNames = { "无强化", "强化1-3重", "强化4-6重", "强化7-9重" };

    /// <summary>MagicActionTypeNames[TMagicActionType]（5 项）。</summary>
    public static readonly string[] MagicActionTypeNames =
        { "魔法动作", "普通砍动作", "跳跃砍动作", "无动作", "自定义动作" };

    /// <summary>MagicSoundTypeNames[TMagicSoundType]（6 项）。</summary>
    public static readonly string[] MagicSoundTypeNames =
        { "ManWarr", "WomanWarr", "UseMagic", "MagicFly", "MagicExplosion", "MagicFail" };

    /// <summary>MagicSwitchModeNames[TMagicSwitchMode]（3 项）。</summary>
    public static readonly string[] MagicSwitchModeNames = { "无模式", "开关模式", "攻杀模式" };

    /// <summary>CheckVarTypeNames[TCheckVarType]（6 项）。</summary>
    public static readonly string[] CheckVarTypeNames = { ">", "<", "=", "<>", ">=", "<=" };

    /// <summary>MagicAttackDecAttributesTypeIniNames[TMagicAttackDecAttributesType]（9 项）。</summary>
    public static readonly string[] MagicAttackDecAttributesTypeIniNames =
        { "AC", "MAC", "DC", "MC", "SC", "HitPoint", "SpeedPoint", "AntiMagic", "AntiPoison" };

    /// <summary>MagicAttackDecAttributesTypeNames[TMagicAttackDecAttributesType]（9 项）。</summary>
    public static readonly string[] MagicAttackDecAttributesTypeNames =
        { "减防御", "减魔御", "减攻击", "减魔法", "减道术", "减准确", "减敏捷", "减魔法躲避", "减毒物躲避" };

    /// <summary>MagicProtectAddAttributesTypeIniNames[TMagicProtectAddAttributesType]（16 项）。</summary>
    public static readonly string[] MagicProtectAddAttributesTypeIniNames =
        { "AC", "MAC", "DC", "MC", "SC", "HitPoint", "SpeedPoint", "AntiMagic", "AntiPoison",
          "NGDamage", "NGDefense", "HP", "MP", "MaxHP", "MaxMP", "Hide" };

    /// <summary>MagicProtectAddAttributesTypeNames[TMagicProtectAddAttributesType]（16 项）。</summary>
    public static readonly string[] MagicProtectAddAttributesTypeNames =
        { "加防御", "加魔御", "加攻击", "加魔法", "加道术", "加准确", "加敏捷", "加魔法躲避", "加毒物躲避",
          "加内功伤害", "加内功防御", "单次加HP", "单次加MP", "加MaxHP", "加MaxMP", "加隐身" };

    /// <summary>ItemElementsTypeIniNames[TItemElementsType]（25 项）。</summary>
    public static readonly string[] ItemElementsTypeIniNames =
        { "BlastHit", "DamageAdd", "DamageDec", "SpellDamageDec", "CloseDefense", "DamageRebound",
          "MonDropRate", "MaxHPAdd", "MaxMPAdd", "AngryValueTimAdd", "GroupDamageAdd", "HuamDropRate",
          "UndropRate", "UnParalysis", "UnMagicShield", "UnRevival", "UnPosion", "UnTamming",
          "UnFireCross", "UnFrozen", "UnCobwebWinding", "FatalBlowRate", "FatalBlowPower",
          "FatalBlowDefense", "UnBlastHit" };

    /// <summary>ItemElementsTypeNames[TItemElementsType]（25 项）。</summary>
    public static readonly string[] ItemElementsTypeNames =
        { "暴击几率", "攻击伤害", "伤害吸收", "魔法防御", "忽视防御", "伤害反弹", "怪物爆率", "体力增加",
          "魔力增加", "怒气恢复", "合击伤害", "人物爆率", "防爆出率", "防止麻痹", "防止护身", "防止复活",
          "防止全毒", "防止诱惑", "防止火墙", "防止冰冻", "防止蛛网", "致命一击几率", "致命一击伤害",
          "致命一击防御", "暴击抗性" };

    /// <summary>MagicAttackDecValueTypeNames[Boolean]（False='%'、True='点'）。</summary>
    public static readonly string[] MagicAttackDecValueTypeNames = { "%", "点" };

    /// <summary>MagicAttackDecTimeTypeNames[Boolean]（False='%'、True='秒'）。</summary>
    public static readonly string[] MagicAttackDecTimeTypeNames = { "%", "秒" };

    /// <summary>BreakDefenseTypeNames[TBreakDefenseType]（6 项）。</summary>
    public static readonly string[] BreakDefenseTypeNames =
        { "BreakHumDefense", "BreakMonDefense", "BreakHeroDefense",
          "BreakHumMagDefense", "BreakMonMagDefense", "BreakHeroMagDefense" };
}
