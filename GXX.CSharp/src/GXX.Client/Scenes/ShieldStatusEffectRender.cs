using System;
using System.Collections.Generic;

namespace GXX.Client.Scenes;

/// <summary>护盾类状态特效的一次绘制（GameCanvas.DrawBlend 的 headless 产物）。</summary>
public sealed record ShieldEffectDrawOp(
    int X, int Y, int ImageIndex, string Kind, bool Gray);

/// <summary>
/// Actor.pas THumActor.DrawChr 尾段的**护盾/状态特效族**（批次J89，无纹理 headless 化）——
/// 破碎盾(16853-16864)、魔法盾 3 级两版(16868-16903，原文 `boSkill31UseNewEffect` 分支)、
/// 4 级魔法盾(16906-16930)。
/// 所有分支共同点：**骑马时 (`m_btHorse = 0` 不成立) 一律不绘**、取图按观察者死亡走灰度、
/// 最终均以 **DrawBlend** 绘制、位置 = `dx + 图原点X + m_nShiftX` / `dy + 图原点Y + m_nShiftY`。
/// </summary>
public static class ShieldStatusEffectRender
{
    /// <summary>Actor.pas 49：魔法盾效果图位置。</summary>
    public const int MAGBUBBLEBASE = 3890;

    /// <summary>Actor.pas 50：被攻击时魔法盾效果图位置。</summary>
    public const int MAGBUBBLESTRUCKBASE = 3900;

    /// <summary>16871/16881：受击变体仅在 `m_nCurBubbleStruck < 3` 时启用。</summary>
    public const int BubbleStruckMax = 3;

    /// <summary>16873/16884：非受击变体按 `m_nGenAniCount mod 3` 取帧。</summary>
    public const int BubbleAniMod = 3;

    /// <summary>16854：破碎盾图号基址。</summary>
    public const int BrokenShieldBase = 4100;

    /// <summary>16868：`STATE_BUBBLEDEFENCEUP`（3 级魔法盾）。</summary>
    public const int STATE_BUBBLEDEFENCEUP = 0x00100000;

    /// <summary>16906：4 级魔法盾的状态位（原文注释记为 STATE_16）。</summary>
    public const int STATE_16 = 0x00008000;

    /// <summary>16929：新武力盾（`STATE_NEWHITBUBBLEDEFENCEUP`）。</summary>
    public const int STATE_NEWHITBUBBLEDEFENCEUP = 0x00040000;

    /// <summary>16963：新道力盾（原文注释亦记为 `STATE_NEWHITBUBBLEDEFENCEUP`，与上一项注释重名）。</summary>
    public const int STATE_NEWHITBUBBLEDEFENCEUP_2 = 0x00020000;

    /// <summary>16930：新武力盾的**固定**图号（原文注释掉的分支才是 `480 + m_nGenNewHitAniCount mod 7`）。</summary>
    public const int NewHitBubbleFixedIndex = 494;

    /// <summary>16996：新道力盾的纵坐标修正（`+ 2`）。</summary>
    public const int NewMagBubbleOffsetY = 2;

    /// <summary>16882/16884：新魔法盾效果（`boSkill31UseNewEffect`）的图号基址。</summary>
    public const int NewBubbleStruckBase = 920;
    public const int NewBubbleBase = 910;

    /// <summary>16908/16910：4 级魔法盾的图号基址。</summary>
    public const int Magic6StruckBase = 723;
    public const int Magic6Base = 720;

    /// <summary>16893：新魔法盾的纵坐标修正（`ay := ay - 6`）。</summary>
    public const int NewBubbleOffsetY = -6;

    /// <summary>SObjectType 枚举（决定取哪个图库）。</summary>
    public enum ShieldImageSet
    {
        None,           // 不绘制
        CboEffect,      // g_cboEffect
        WMagic,         // g_WMagicImages
        WMagicre,       // g_WMagicreImages（新魔法盾）
        WMagic6,        // g_WMagic6Images（4 级）
    }

    /// <summary>一次护盾特效绘制的规划结果。</summary>
    public sealed record ShieldPlan(
        ShieldImageSet ImageSet, int ImageIndex, int OffsetX, int OffsetY, string Kind);

    /// <summary>
    /// 破碎盾（16853-16864）：`m_boBrokenShield and m_btHorse = 0` 时取 `4100 + m_nBrokenShieldEffect`。
    /// 该分支与魔法盾分支是 **if/else 互斥**关系（16864 的 else）——破碎盾时不再绘魔法盾。
    /// </summary>
    public static ShieldPlan? PlanBrokenShield(bool boBrokenShield, int m_btHorse, int nBrokenShieldEffect)
    {
        if (!boBrokenShield || m_btHorse != 0)
            return null;

        return new ShieldPlan(ShieldImageSet.CboEffect, BrokenShieldBase + nBrokenShieldEffect, 0, 0, "BrokenShield");
    }

    /// <summary>
    /// 3 级魔法盾（16868-16903）：需 `STATE_BUBBLEDEFENCEUP` 置位且 `m_btHorse = 0`；
    /// 按 `boSkill31UseNewEffect` 走新旧两版：
    /// 旧版图号 `MAGBUBBLESTRUCKBASE + 受击数` 或 `MAGBUBBLEBASE + (m_nGenAniCount mod 3)`，取 `g_WMagicImages`；
    /// 新版图号 `920 + 受击数` 或 `910 + (m_nGenAniCount mod 3)`，取 `g_WMagicreImages`，
    /// **且把图原点纵坐标上移 6**（16893，原文另有一句 `ax := ax + 0` 为无操作，逐字保留其含义）。
    /// </summary>
    public static ShieldPlan? PlanBubbleDefence(
        int m_nState, int m_btHorse, bool boSkill31UseNewEffect,
        int m_nCurrentAction, int m_nCurBubbleStruck, int m_nGenAniCount)
    {
        if ((m_nState & STATE_BUBBLEDEFENCEUP) == 0 || m_btHorse != 0)
            return null;

        bool struck = m_nCurrentAction == TActorCore.SM_STRUCK && m_nCurBubbleStruck < BubbleStruckMax;

        if (!boSkill31UseNewEffect)
        {
            int idx = struck
                ? MAGBUBBLESTRUCKBASE + m_nCurBubbleStruck
                : MAGBUBBLEBASE + (m_nGenAniCount % BubbleAniMod);

            return new ShieldPlan(ShieldImageSet.WMagic, idx, 0, 0, "BubbleDefenceOld");
        }

        int nidx = struck
            ? NewBubbleStruckBase + m_nCurBubbleStruck
            : NewBubbleBase + (m_nGenAniCount % BubbleAniMod);

        // 16901 的 `ax := ax + 0` 为无操作；16902 的 `ay := ay - 6` 才是修正
        return new ShieldPlan(ShieldImageSet.WMagicre, nidx, 0, NewBubbleOffsetY, "BubbleDefenceNew");
    }

    /// <summary>
    /// 4 级魔法盾（16906-16930）：需 `STATE_16` 置位且 `m_btHorse = 0`；
    /// 受击时 `723 + m_nCurBubbleStruck`，否则 `720 + (m_nGenAniCount mod 3)`，取 `g_WMagic6Images`。
    /// **该分支无新旧两版**，也无纵坐标修正。
    /// </summary>
    public static ShieldPlan? PlanMagic6(
        int m_nState, int m_btHorse,
        int m_nCurrentAction, int m_nCurBubbleStruck, int m_nGenAniCount)
    {
        if ((m_nState & STATE_16) == 0 || m_btHorse != 0)
            return null;

        bool struck = m_nCurrentAction == TActorCore.SM_STRUCK && m_nCurBubbleStruck < BubbleStruckMax;

        int idx = struck
            ? Magic6StruckBase + m_nCurBubbleStruck
            : Magic6Base + (m_nGenAniCount % BubbleAniMod);

        return new ShieldPlan(ShieldImageSet.WMagic6, idx, 0, 0, "Magic6");
    }

    /// <summary>
    /// 新武力盾（16929-16959）：需 `STATE_NEWHITBUBBLEDEFENCEUP` 且 `m_btHorse = 0`；
    /// 图号是**固定 494**（16930），取 `g_WMagic6Images`；无受击/动画变体
    /// （16944-16958 那段 `480 + m_nGenNewHitAniCount mod 7` 的动画版**已被整段注释掉**）。
    /// </summary>
    public static ShieldPlan? PlanNewHitBubble(int m_nState, int m_btHorse)
    {
        if ((m_nState & STATE_NEWHITBUBBLEDEFENCEUP) == 0 || m_btHorse != 0)
            return null;

        return new ShieldPlan(ShieldImageSet.WMagic6, NewHitBubbleFixedIndex, 0, 0, "NewHitBubble");
    }

    /// <summary>
    /// 新道力盾（16963-16999）：需 `STATE_NEWHITBUBBLEDEFENCEUP_2`(0x00020000) 且 `m_btHorse = 0`；
    /// 受击时 `723 + m_nCurNewMagBubbleStruck`，否则 `720 + (m_nGenNewMagAniCount mod 3)`，
    /// 取 `g_WMagic6Images`，**位置额外下移 2**（16996 的 `+ 2`）。
    /// 16964-16981 那段用 `g_WMagicImages` + `3890/3900` 的旧版**已被整段注释掉**。
    /// 注意其受击/动画计数用的是 `m_nCurNewMagBubbleStruck` / `m_nGenNewMagAniCount`（与 4 级盾**不同的字段**）。
    /// </summary>
    public static ShieldPlan? PlanNewMagBubble(
        int m_nState, int m_btHorse,
        int m_nCurrentAction, int m_nCurNewMagBubbleStruck, int m_nGenNewMagAniCount)
    {
        if ((m_nState & STATE_NEWHITBUBBLEDEFENCEUP_2) == 0 || m_btHorse != 0)
            return null;

        bool struck = m_nCurrentAction == TActorCore.SM_STRUCK && m_nCurNewMagBubbleStruck < BubbleStruckMax;

        int idx = struck
            ? Magic6StruckBase + m_nCurNewMagBubbleStruck
            : Magic6Base + (m_nGenNewMagAniCount % BubbleAniMod);

        return new ShieldPlan(ShieldImageSet.WMagic6, idx, 0, NewMagBubbleOffsetY, "NewMagBubble");
    }

    /// <summary>
    /// 组合调度 1:1（16853-17000）：**破碎盾与魔法盾族互斥**（16864 的 else），
    /// else 段内 3 级/4 级/新武力盾/新道力盾是**四个独立 if，可同时满足**，故可能产出多条 op。
    /// 顺序即原文书写顺序。
    /// </summary>
    public static List<ShieldPlan> Plan(
        bool boBrokenShield, int nBrokenShieldEffect,
        int m_nState, int m_btHorse, bool boSkill31UseNewEffect,
        int m_nCurrentAction, int m_nCurBubbleStruck, int m_nGenAniCount,
        int m_nCurNewMagBubbleStruck = 0, int m_nGenNewMagAniCount = 0)
    {
        var list = new List<ShieldPlan>();

        var broken = PlanBrokenShield(boBrokenShield, m_btHorse, nBrokenShieldEffect);
        if (broken != null)
        {
            list.Add(broken);
            return list;                            // 16864 else：破碎盾时不走魔法盾
        }

        var b3 = PlanBubbleDefence(m_nState, m_btHorse, boSkill31UseNewEffect,
            m_nCurrentAction, m_nCurBubbleStruck, m_nGenAniCount);
        if (b3 != null)
            list.Add(b3);

        var b4 = PlanMagic6(m_nState, m_btHorse, m_nCurrentAction, m_nCurBubbleStruck, m_nGenAniCount);
        if (b4 != null)
            list.Add(b4);

        var nh = PlanNewHitBubble(m_nState, m_btHorse);
        if (nh != null)
            list.Add(nh);

        var nm = PlanNewMagBubble(m_nState, m_btHorse, m_nCurrentAction,
            m_nCurNewMagBubbleStruck, m_nGenNewMagAniCount);
        if (nm != null)
            list.Add(nm);

        return list;
    }

    /// <summary>
    /// 依规划取图并产出绘制 op 1:1（16856-16900）：按图库选择接缝取图（观察者死亡走灰度），
    /// 命中则以 DrawBlend 绘于 `dx + 图原点X + 偏移X + m_nShiftX` 等位置。
    /// </summary>
    public static List<ShieldEffectDrawOp> Draw(
        List<ShieldPlan> plans,
        int dx, int dy, int m_nShiftX, int m_nShiftY, bool selfDead,
        Func<ShieldImageSet, int, bool, FxImage?> resolve)
    {
        var ops = new List<ShieldEffectDrawOp>();

        foreach (var p in plans)
        {
            var d = resolve(p.ImageSet, p.ImageIndex, selfDead);
            if (d == null)
                continue;

            ops.Add(new ShieldEffectDrawOp(
                dx + d.OriginX + p.OffsetX + m_nShiftX,
                dy + d.OriginY + p.OffsetY + m_nShiftY,
                d.ImageIndex, p.Kind, selfDead));
        }

        return ops;
    }
}
