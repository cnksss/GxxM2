using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>魔法/攻击特效族的一次绘制（GameCanvas.DrawBlend 的 headless 产物）。</summary>
public sealed record MagicEffectDrawOp(int X, int Y, MagicImgLib Lib, int ImageIndex, string Kind);

/// <summary>
/// Actor.pas THumActor.DrawChr 的**魔法/攻击特效派发族**（批次J90，headless 化）——
/// ① `m_boUseEffect` 跟随人物动作效果(17002-17013)；
/// ② 施法起手特效(17017-17139)：自定义技能走 `DrawSelfMagicEffect`，否则 `m_boUseMagic` 分支；
/// ③ 魔法结束特效(17143-17161)；④ 攻击特效族(17171-17240)。
/// </summary>
public static class MagicEffectDispatchRender
{
    /// <summary>17021：`m_CurMagic.EffectNumber in [100..111]` 的技能**不走本派发**（原为 107-111 的分支已整段注释）。</summary>
    public const int EffectNumberExcludedLow = 100;
    public const int EffectNumberExcludedHigh = 111;

    /// <summary>17024：魔法盾起手动作在连击时**直接 Exit（退出整个 DrawChr）**。</summary>
    public const int ShieldSpellEffectNumber = 29;

    /// <summary>17029：4 级灭天火（人物起始动作）特例。</summary>
    public const int FireOfHeavenSerial = 45;
    public const int FireOfHeavenBaseIndex = 80;

    /// <summary>17049-17062：新魔法盾/道力盾/武力盾起手（`boSkill31UseNewEffect`）。</summary>
    public const int NewShieldEffectBase4Level = 900;
    public const int NewShieldEffectBaseNormal = 880;
    public const int NewShieldOffsetX = 2;
    public const int NewShieldOffsetY = -6;

    /// <summary>17069-17071：4 级新武力盾用强化盾效果。</summary>
    public const int NewPhysicalShieldEffectNumber = 68;
    public const int NewPhysicalShieldBaseIndex = 1608;

    /// <summary>17074-17076：4 级灵魂火符起手动作。</summary>
    public const int SoulFireSymbolEffectNumber = 10;
    public const int SoulFireSymbolBaseIndex = 120;

    /// <summary>17100：新武力盾只有 5 帧，超过则钉在 +4。</summary>
    public const int NewPhysicalShieldFrameCap = 4;

    /// <summary>17106-17107：施毒术（红毒/绿毒）未持物品施放时图号 +210。</summary>
    public const int PoisonEffectNumber = 4;
    public const int PoisonItemOffset = 210;

    /// <summary>17174-17222：攻击特效按编号的**方向步长**（20 或 10）与特殊处理。</summary>
    public static int? HitEffectDirStride(int hitEffectNumber) => hitEffectNumber switch
    {
        7 => 20,    // 雷霆剑法
        8 => 20,    // 龙影剑法
        9 => 20,    // 破魂斩
        20 => 20,   // 三绝杀
        26 => 20,   // 断空斩
        10 => 10,   // 劈星斩
        12 => 10,   // 开天斩重击
        25 => 10,   // 开天斩轻击
        14 => 10,   // 逐日剑法
        27 => 10,   // 血魄一击
        23 => 10,   // 追心刺
        _ => null,  // 走 else 分支（同样 +10）
    };

    /// <summary>4 级烈火特例（17202）：`编号 = 4 and Level2 = 4 and Level = 0`。</summary>
    public const int FireHitEffectNumber = 4;

    /// <summary>17208：追心刺在 `m_nCurrentAction = 0` 时把 idx 置 -1（即不绘）。</summary>
    public const int ChasingHeartHitEffectNumber = 23;

    // ==================================================================
    // ① m_boUseEffect 跟随人物动作效果（17002-17013）
    // ==================================================================

    /// <summary>
    /// 17002：`m_boUseEffect and (m_UseEffectImage &lt;&gt; nil)` 时按 `m_nEffectFrame` 取图，
    /// 恒以 DrawBlend 绘于 `dx + 图原点 + m_nShift`。**无帧范围守卫、无方向调整**。
    /// </summary>
    public static bool UseEffectGate(bool m_boUseEffect, bool hasUseEffectImage)
        => m_boUseEffect && hasUseEffectImage;

    // ==================================================================
    // ② 施法起手特效（17017-17139）
    // ==================================================================

    /// <summary>施法起手特效的规划结果。`Index &lt; 0` 表示不绘。</summary>
    public sealed record SpellEffectPlan(
        MagicImgLib Lib, int Index, int OffsetX, int OffsetY, string Kind);

    /// <summary>
    /// 17024-17026：**魔法盾起手在连击时直接 Exit 整个 DrawChr**——
    /// 返回值表示"应立刻退出，且后续魔法结束/攻击特效都不绘"。
    /// 这是本族中唯一会中断整个 DrawChr 的路径。
    /// </summary>
    public static bool ShouldExitForContinuousShield(
        int effectNumber, bool isMySelf, bool isInContinuous)
        => effectNumber == ShieldSpellEffectNumber && isMySelf && isInContinuous;

    /// <summary>17021：施法起手特效的进入条件。</summary>
    public static bool SpellGate(bool m_boUseMagic, int effectNumber)
        => m_boUseMagic && effectNumber > 0
           && !(effectNumber >= EffectNumberExcludedLow && effectNumber <= EffectNumberExcludedHigh);

    /// <summary>
    /// 施法起手特效的取图与图号修正 1:1（17028-17107）。
    /// <paramref name="getEffectBase"/> 为 `GetEffectBase(EffectNumber-1, 0, wimg, idx, NewLevel)` 接缝。
    /// </summary>
    public static SpellEffectPlan PlanSpellEffect(
        int magicSerial, int effectNumber, int magicLevel, int newLevel,
        int btJob, int curEffFrame, bool boSkill31UseNewEffect,
        bool magicItemType,
        Func<int, int, EffectBaseImage> getEffectBase)
    {
        // 17029：4 级灭天火（人物起始动作）——独立分支，不参与后续修正
        if (magicSerial == FireOfHeavenSerial && magicLevel == 4 && newLevel == 0)
        {
            return new SpellEffectPlan(MagicImgLib.WMagic6Images,
                FireOfHeavenBaseIndex + curEffFrame, 0, 0, "FireOfHeaven");
        }

        int nX = 0, nY = 0;
        MagicImgLib lib;
        int idx;

        if (boSkill31UseNewEffect && effectNumber == ShieldSpellEffectNumber)
        {
            // 17051：4 级技能、强化技能效果
            lib = MagicImgLib.WMagicreImages;
            idx = (newLevel > 0 || magicLevel == 4) ? NewShieldEffectBase4Level : NewShieldEffectBaseNormal;

            nX = NewShieldOffsetX;
            nY = NewShieldOffsetY;
        }
        else if (effectNumber == ShieldSpellEffectNumber && (newLevel > 0 || magicLevel == 4))
        {
            // 17065：4 级魔法盾/道力盾/武力盾用强化盾效果（NewLevel 传 1）
            var r = MagicEffectBaseLookup.Resolve(effectNumber - 1, 0, 1, 0, 0);
            lib = r.Lib;
            idx = r.BaseIndex;
        }
        else if (effectNumber == NewPhysicalShieldEffectNumber && (newLevel > 0 || magicLevel == 4))
        {
            lib = MagicImgLib.WMagic10Images;
            idx = NewPhysicalShieldBaseIndex;
        }
        else if (effectNumber == SoulFireSymbolEffectNumber && magicLevel >= 4)
        {
            lib = MagicImgLib.WMagic6Images;
            idx = SoulFireSymbolBaseIndex;
        }
        else
        {
            var r = getEffectBase(effectNumber - 1, newLevel);
            lib = r.Lib;
            idx = r.BaseIndex;
        }

        if (idx < 0)
            return new SpellEffectPlan(lib, -1, nX, nY, "Spell");

        // 17082-17086：职业相关的图号偏移（107-111 的分支已被整段注释）
        switch (effectNumber)
        {
            case 61:
                if (btJob == 2) idx -= 20;
                break;
            case 62:
                idx -= btJob * 20;
                break;
            case 64:
                if (btJob == 2) idx += 20;      // 末日审判起手动作
                break;
        }

        // 17100-17103：新武力盾只有 5 帧（>4 时钉在 +4）
        if (effectNumber == NewPhysicalShieldEffectNumber && curEffFrame > NewPhysicalShieldFrameCap)
            idx += NewPhysicalShieldFrameCap;
        else
            idx += curEffFrame;

        // 17106-17107：施毒术未持物品时所放技能用另一套效果
        if (effectNumber == PoisonEffectNumber && newLevel > 0 && !magicItemType)
            idx += PoisonItemOffset;

        return new SpellEffectPlan(lib, idx, nX, nY, "Spell");
    }

    // ==================================================================
    // ③ 魔法结束特效（17143-17161）
    // ==================================================================

    /// <summary>
    /// 17143-17147：`m_boMagicEndEffect and (EffectNumber &gt; 0)` 且帧在 `[0..m_nSpellFrame-1]`，
    /// 图号 = `GetEffectBase 基址 + (m_nCurrentMagicFrame - m_nStartMagicFrame + m_nStartPosMagicFrame)`。
    /// **注意与本族其它成员不同：此处不叠加 `m_nCurEffFrame`，而是用三个 Magic 帧字段的组合。**
    /// </summary>
    public static SpellEffectPlan? PlanMagicEndEffect(
        bool m_boMagicEndEffect, int effectNumber, int newLevel, int curEffFrame, int spellFrame,
        int currentMagicFrame, int startMagicFrame, int startPosMagicFrame,
        Func<int, int, EffectBaseImage> getEffectBase)
    {
        if (!m_boMagicEndEffect || effectNumber <= 0)
            return null;
        if (curEffFrame < 0 || curEffFrame > spellFrame - 1)
            return null;

        var r = getEffectBase(effectNumber - 1, newLevel);
        if (r.BaseIndex < 0)
            return null;

        int idx = r.BaseIndex + (currentMagicFrame - startMagicFrame + startPosMagicFrame);
        return new SpellEffectPlan(r.Lib, idx, 0, 0, "MagicEnd");
    }

    // ==================================================================
    // ④ 攻击特效族（17171-17240）
    // ==================================================================

    /// <summary>4 级烈火特例的参数（17202）。</summary>
    public sealed record FireHitSpecial(bool Active, int Index, MagicImgLib Lib);

    /// <summary>
    /// 攻击特效图号修正 1:1（17174-17222）。
    /// 返回索引 &lt; 0 表示不绘（追心刺在 `m_nCurrentAction = 0` 时置 -1）。
    /// **`else` 分支与多数具名分支等价（同为 `+dir*10+帧差`），但原文如此分别书写，逐字保留其结构。**
    /// </summary>
    public static SpellEffectPlan? PlanHitEffect(
        int hitEffectNumber, int hitEffectLevel, int hitEffectLevel2,
        int btDir, int currentFrame, int startFrame, int currentAction,
        int baseIndex, MagicImgLib baseLib)
    {
        int frameDelta = currentFrame - startFrame;
        int idx;

        // 17202：4 级烈火（无强化）——从 0 重新计数并换图库
        bool fireSpecial = hitEffectNumber == FireHitEffectNumber
            && hitEffectLevel2 == 4 && hitEffectLevel == 0;

        if (fireSpecial)
        {
            idx = 0 + btDir * 10 + frameDelta;
            return new SpellEffectPlan(MagicImgLib.WMagic6Images, idx, 0, 0, "HitFire4");
        }

        // 17207-17212：追心刺在无动作时不绘
        if (hitEffectNumber == ChasingHeartHitEffectNumber && currentAction == 0)
            return null;

        int stride = HitEffectDirStride(hitEffectNumber) ?? 10;
        idx = baseIndex + btDir * stride + frameDelta;

        return new SpellEffectPlan(baseLib, idx, 0, 0, "Hit");
    }

    // ==================================================================
    // 绘制
    // ==================================================================

    /// <summary>
    /// 以灰度/彩色取图并产出绘制 op。所有成员均 DrawBlend 且位置含 `m_nShift`，
    /// 施法起手额外叠加 `nX`/`nY` 修正（17116-17117）。
    /// </summary>
    public static MagicEffectDrawOp? Draw(
        SpellEffectPlan plan, int dx, int dy, int shiftX, int shiftY, bool selfDead, int originX, int originY,
        string kindOverride = null)
    {
        if (plan.Index < 0)
            return null;

        return new MagicEffectDrawOp(
            dx + originX + shiftX + plan.OffsetX,
            dy + originY + shiftY + plan.OffsetY,
            plan.Lib, plan.Index, kindOverride ?? plan.Kind);
    }
}
