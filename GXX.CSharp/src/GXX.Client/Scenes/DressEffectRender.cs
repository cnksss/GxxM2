using System;
using System.Collections.Generic;

namespace GXX.Client.Scenes;

/// <summary>时装/护盾特效类绘制的一次产物（GameCanvas.Draw 或 DrawBlend）。</summary>
public sealed record DressEffectDrawOp(int X, int Y, string Kind, bool Blend);

/// <summary>
/// Actor.pas `TActor.DrawDressEffect`（5974-5987）+ `THumActor.DrawDressEffect`（11260-11304）
/// + `TActor.DrawShieldEffect`（5915-5933）1:1 移植（批次J91）。
///
/// **本族的命名约定并不统一，是原文的真实样貌，不可"统一"**：
/// - `m_boDressEffectDrawNoBlend` / `m_boShieldEffectDrawNoBlend` /
///   `m_boHeroM2DressNoBlend` / `m_boMedalEffectDrawNoBlend`：
///   **为真 → `Draw`（不混合）**，为假 → `DrawBlend`；
/// - 但 `m_boEffectNormalDraw` / `m_boEffect_30NormalDraw`：
///   **为真 → `Draw`**，为假 → `DrawBlend`（名字是"NormalDraw"而非"NoBlend"，语义方向相同但命名相反）。
/// 两者最终行为一致（真 → Draw），但字段名与判据写法在原文中不同，故此处分别保留两个入口。
/// </summary>
public static class DressEffectRender
{
    // ==================================================================
    // TActor.DrawDressEffect（5974-5987）：基类，只画 m_DressEffectSurface
    // ==================================================================

    /// <summary>
    /// 5915-5933 `TActor.DrawShieldEffect`：需
    /// `(not ConfigCheckeds[ckHideWeaponEffect]) or (not PlugInEnabled)`
    /// —— 即**隐藏武器特效开启且插件可用时不绘制盾特效**（注意是 or 不是 and）。
    /// </summary>
    public static bool ShieldEffectGate(bool hideWeaponEffectChecked, bool plugInEnabled)
        => !hideWeaponEffectChecked || !plugInEnabled;

    /// <summary>
    /// 5915-5933：`m_ShieldEffectSurface` 非空且过门时绘制，
    /// `m_boShieldEffectDrawNoBlend` 为真走 `Draw`，否则 `DrawBlend`。
    /// </summary>
    public static DressEffectDrawOp? DrawShieldEffect(
        int ddx, int ddy,
        bool hasShieldEffectSurface, bool shieldEffectDrawNoBlend,
        int shieldEffectX, int shieldEffectY, int shiftX, int shiftY,
        bool hideWeaponEffectChecked, bool plugInEnabled)
    {
        if (!ShieldEffectGate(hideWeaponEffectChecked, plugInEnabled))
            return null;
        if (!hasShieldEffectSurface)
            return null;

        return new DressEffectDrawOp(
            ddx + shieldEffectX + shiftX,
            ddy + shieldEffectY + shiftY,
            "ShieldEffect",
            !shieldEffectDrawNoBlend);
    }

    /// <summary>
    /// 5974-5987 `TActor.DrawDressEffect`：只处理 `m_DressEffectSurface`，
    /// `m_boDressEffectDrawNoBlend` 为真走 `Draw`。
    /// </summary>
    public static DressEffectDrawOp? DrawBaseDressEffect(
        int ddx, int ddy,
        bool hasDressEffectSurface, bool dressEffectDrawNoBlend,
        int dressEffectX, int dressEffectY, int shiftX, int shiftY)
    {
        if (!hasDressEffectSurface)
            return null;

        return new DressEffectDrawOp(
            ddx + dressEffectX + shiftX,
            ddy + dressEffectY + shiftY,
            "DressEffect",
            !dressEffectDrawNoBlend);
    }

    // ==================================================================
    // THumActor.DrawDressEffect（11260-11304）：四个独立块 + inherited
    // ==================================================================

    /// <summary>
    /// 11260-11304 `THumActor.DrawDressEffect` 1:1。
    /// 四个块**各自独立**（非 else 链），故可能同时产出多张图，顺序即原文顺序：
    /// ① `m_HumWinSurface`（`m_boEffectNormalDraw` **为真走 Draw**）；
    /// ② `m_HumWinSurface_30`（`m_boEffect_30NormalDraw` **为真走 Draw**）；
    /// ③ `m_HeroM2DressEffect`（`m_boHeroM2DressNoBlend` **为真走 Draw**）；
    /// ④ `m_MedalEffectSurface`（`m_boMedalEffectDrawNoBlend` **为真走 Draw**）；
    /// 最后 `inherited`（11303）→ 基类块画 `m_DressEffectSurface`，**位于四条之后**。
    /// 注意 `blend` 与 `ceff` 两个参数在 `THumActor` 版本中**完全未被使用**。
    /// </summary>
    public static List<DressEffectDrawOp> DrawHumDressEffect(
        int ddx, int ddy, int shiftX, int shiftY,
        bool hasHumWinSurface, int spX, int spY, bool effectNormalDraw,
        bool hasHumWinSurface30, int spX30, int spY30, bool effect30NormalDraw,
        bool hasHeroM2DressEffect, int heroM2X, int heroM2Y, bool heroM2DressNoBlend,
        bool hasMedalEffectSurface, int medalEffectX, int medalEffectY, bool medalEffectDrawNoBlend,
        bool hasDressEffectSurface, int dressEffectX, int dressEffectY, bool dressEffectDrawNoBlend)
    {
        var ops = new List<DressEffectDrawOp>();

        if (hasHumWinSurface)
        {
            ops.Add(new DressEffectDrawOp(
                ddx + spX + shiftX, ddy + spY + shiftY, "HumWin", !effectNormalDraw));
        }

        if (hasHumWinSurface30)
        {
            ops.Add(new DressEffectDrawOp(
                ddx + spX30 + shiftX, ddy + spY30 + shiftY, "HumWin30", !effect30NormalDraw));
        }

        if (hasHeroM2DressEffect)
        {
            ops.Add(new DressEffectDrawOp(
                ddx + heroM2X + shiftX, ddy + heroM2Y + shiftY, "HeroM2Dress", !heroM2DressNoBlend));
        }

        if (hasMedalEffectSurface)
        {
            ops.Add(new DressEffectDrawOp(
                ddx + medalEffectX + shiftX, ddy + medalEffectY + shiftY, "MedalEffect", !medalEffectDrawNoBlend));
        }

        // 11303：inherited → 基类块（m_DressEffectSurface）在最后
        var baseOp = DrawBaseDressEffect(ddx, ddy, hasDressEffectSurface, dressEffectDrawNoBlend,
            dressEffectX, dressEffectY, shiftX, shiftY);
        if (baseOp != null)
            ops.Add(baseOp);

        return ops;
    }

    // ==================================================================
    // 已注释掉的 DrawDressEffectEx（11260/5991 两处）
    // ==================================================================

    /// <summary>
    /// `THumActor.DrawDressEffectEx`（11306-11336）与 `TActor.DrawDressEffectEx`（5991-6004）
    /// **在原文中均被整段 `{...}` 注释掉**，故本移植不提供实现。
    /// 记录其被注释的行为以免日后误"补全"：若启用，各块会以
    /// `GameCanvas.DrawColorAlpha(nX, nY, 图, LightColor, LightAlpha, Blend_SrcAlphaColor)`
    /// 代替 `DrawBlend`/`Draw`（用光照色与透明度做加法式混合）；
    /// 且 `THumActor` 版本第 ③ 块存在**原文笔误**——判据用 `m_HeroM2DressEffect`
    /// 但传入的是 `m_HumWinSurface`（11332），若照抄会画错图。
    /// </summary>
    public const bool DrawDressEffectExIsCommentedOut = true;
}
