using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>绘制层（THumActor.DrawChr race 0/1 分支的分层顺序）。</summary>
public enum HumDrawLayer
{
    DressEffectBehind,   // 时装/勋章特效 - 身体下层（方向 3/4/5 时提前绘）
    WeaponBehind,        // 武器 - 身体后（m_nWpord=0）
    ShieldBehind,        // 盾牌 - 身体后（m_nWpord=1 且 dir≠5）
    Horse,               // 坐骑
    HorseWingsEffect,    // 坐骑翅膀特效
    HorseEffect,         // 坐骑特效
    HorseHum,            // 三方马上人物
    HorseHair,           // 三方马上人物发型
    DressAddEffectBelow, // 衣服附加特效 - 下（order≠0）
    Body,                // 身体
    DressAddEffectAbove, // 衣服附加特效 - 上（order=0）
    Hair,                // 发型
    WeaponFront,         // 武器 - 身体前（m_nWpord=1）
    ShieldFront,         // 盾牌 - 前（m_nWpord=0 或 (m_nWpord=1 且 dir=5)）
    DressEffectFront,    // 时装/勋章特效 - 身体上层（m_wEffect=50 或方向 0,1,2,6,7）
}

/// <summary>
/// Actor.pas THumActor.DrawChr（16501-16850）绘制排序核心（批次J50，headless 化）：
/// 以 WORDER[sex][frame]（1206-1292 机器提取表）决定武器在身体前/后层，
/// 并按原文固定层序产出：后层特效 → 后层武器 → 后层盾 → 坐骑族 → 下层衣服附加特效 →
/// 身体 → 上层衣服附加特效 → 发型 → 前层武器 → 前层盾 → 前层时装特效。
/// 守卫：m_btDir∈[0..7]、武器需 m_wWeapon≥2 且非隐藏、摆摊图按方向 4/5 前后置。
/// </summary>
public static class HumActorDrawPlanner
{
    public class HumDrawInput
    {
        public byte Sex;
        public int CurrentFrame;      // m_nCurrentFrame（0..599 有效区间才查 WORDER）
        public byte Dir;
        public bool Race0Or1 = true;  // m_btRace in [0,1]
        public int Weapon;            // m_wWeapon
        public bool HideWeapon;       // m_boHideWeapon
        public int Shield;            // m_wShield
        public bool HasBody = true;
        public bool HasHair = true;
        public bool HasWeapon = true;
        public bool HasShield = true;
        public bool HasHorse;         // m_HorseSurface ≠ nil
        public bool HasHorseWingsEffect;
        public bool HasHorseEffect;
        public bool HasHorseHum;
        public bool HasHorseHair;
        public bool HasDressAddEffect;
        public int DressAddEffectOrder; // 0=上层（原文默认），≠0=下层
        public int Effect;              // m_wEffect（天使等）
        public bool HasDressEffectSurface; // m_DressEffectSurface ≠ nil
        public bool HasMedalEffect;
        public bool HasHeroM2DressEffect;
        public bool HasActorEffects;
        public bool ShopStall;          // m_boShopStall（race 0）
        public bool Blend;
    }

    /// <summary>WORDER 查表（16675-16676：m_nCurrentFrame ∈ [0..599] 才查表，否则 m_nWpord 保持缺省 0 → 武器在身体后）。</summary>
    public static int WeaponOrder(byte sex, int currentFrame)
    {
        if (currentFrame >= 0 && currentFrame <= 599)
            return sex == 0
                ? HumActorDrawTables.WorderMale[currentFrame]
                : HumActorDrawTables.WorderFemale[currentFrame];
        return 0;
    }

    /// <summary>DrawChr race 0/1 分支的层序产出。</summary>
    public static List<HumDrawLayer> PlanLayers(HumDrawInput input)
    {
        var layers = new List<HumDrawLayer>();
        if (input.Dir is < 0 or > 7)
            return layers; // 16551：方向守卫

        int wpord = WeaponOrder(input.Sex, input.CurrentFrame);
        bool hasFrontEffectGroup = (input.Effect != 0) || input.HasDressEffectSurface ||
            input.HasMedalEffect || input.HasHeroM2DressEffect || input.HasActorEffects;

        // 16679-16683：下层层特效（非混合绘制且有效果组且方向 3/4/5）
        if (!input.Blend && hasFrontEffectGroup && input.Dir is 3 or 4 or 5)
            layers.Add(HumDrawLayer.DressEffectBehind);

        // 16685-16693：武器 - 身体后（m_nWpord=0 且武器≥2 且未隐藏）
        if (wpord == 0 && !input.Blend && input.Weapon >= 2 && input.HasWeapon && !input.HideWeapon)
            layers.Add(HumDrawLayer.WeaponBehind);

        // 16697：盾牌不透明（人后面）：m_nWpord=1 且非混合 且持盾 且 dir≠5
        if (input.HasShield && wpord == 1 && !input.Blend && input.Shield > 0 && input.Dir != 5)
            layers.Add(HumDrawLayer.ShieldBehind);

        // 16702-16722：坐骑族
        if (input.HasHorse) layers.Add(HumDrawLayer.Horse);
        if (input.HasHorseWingsEffect) layers.Add(HumDrawLayer.HorseWingsEffect);
        if (input.HasHorseEffect) layers.Add(HumDrawLayer.HorseEffect);
        if (input.HasHorseHum) layers.Add(HumDrawLayer.HorseHum);
        if (input.HasHorseHair) layers.Add(HumDrawLayer.HorseHair);

        // 16727-16744：衣服附加特效 - 衣服下层（order≠0）
        if (input.HasDressAddEffect && input.DressAddEffectOrder != 0)
            layers.Add(HumDrawLayer.DressAddEffectBelow);

        // 16746-16751：身体
        if (input.HasBody)
            layers.Add(HumDrawLayer.Body);

        // 16753-16771：衣服附加特效 - 衣服上层（order=0）
        if (input.HasDressAddEffect && input.DressAddEffectOrder == 0)
            layers.Add(HumDrawLayer.DressAddEffectAbove);

        // 16773-16778：发型
        if (input.HasHair)
            layers.Add(HumDrawLayer.Hair);

        // 16781-16789：武器 - 身体前（m_nWpord=1）
        if (wpord == 1 && input.Weapon >= 2 && input.HasWeapon && !input.HideWeapon)
            layers.Add(HumDrawLayer.WeaponFront);

        // 16791-16796：盾牌透明（人前面）：(m_nWpord=0 且持盾) 或 (m_nWpord=1 且 dir=5)
        if (input.HasShield && ((wpord == 0 && input.Shield > 0) || (wpord == 1 && input.Dir == 5)))
            layers.Add(HumDrawLayer.ShieldFront);

        // 16798-16806：前层时装特效：m_wEffect=50 恒绘；否则方向 0,1,2,6,7
        if (input.Effect == 50)
            layers.Add(HumDrawLayer.DressEffectFront);
        else if (hasFrontEffectGroup && input.Dir is 0 or 1 or 2 or 6 or 7)
            layers.Add(HumDrawLayer.DressEffectFront);

        return layers;
    }

    /// <summary>摆摊图位置守卫（16657/16842）：race 0 且摆摊，方向 4/5 时图片绘于最后，否则最先。</summary>
    public static bool ShopStallDrawsLast(byte dir) => dir is 4 or 5;
}
