using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>自身魔法特效的一次绘制（GameCanvas.Draw / DrawBlend 的 headless 产物）。</summary>
public sealed record SelfMagicEffectDrawOp(
    int X, int Y, int ImageIndex, bool Blend, string Kind);

/// <summary>
/// THumActor.DrawChr 的嵌套过程 DrawSelfMagicEffect（Actor.pas 16503-16630）1:1 移植（批次J87）。
/// 该过程是自定义技能"自身特效"层的完整实现：
/// 门禁（战士路径 vs 魔法路径不同）→ 按 `m_CurMagic.NewLevel` 取 `TMagicPlusLevel` 档位
/// → 配置为空即退出 → 帧推进（同步人物动作 / 自定义播放时间两路）→ `PlayFrameCount = 0` 退出
/// → `IsDraw` 判定（**自身与旁人分支不同**）→ 写回 `m_nMagLight` → 方向计算
/// → 按 `Self_DirCalcType` 三分支取图 → 按 `Self_DrawMode` 决定混合绘制。
/// </summary>
public static class SelfMagicEffectRender
{
    /// <summary>16549/16557 的播放时间容差（原文 `Self_PlayTime - 10`）。</summary>
    public const int PlayTimeTolerance = 10;

    /// <summary>16574 的隐身状态位（`g_MySelf.m_nState and $00800000`）。</summary>
    public const int StateInvisible = 0x00800000;

    /// <summary>
    /// `m_CurMagic.NewLevel` → `TMagicPlusLevel`（16527-16534）：
    /// 0 → mplNone；1..3 → mpl1_3；4..6 → mpl4_6；7..9 → mpl7_9；**其余（含负数）→ mpl7_9**。
    /// </summary>
    public static TMagicPlusLevel MagicPlusLevelOf(int newLevel)
    {
        if (newLevel == 0)
            return TMagicPlusLevel.mplNone;
        if (newLevel >= 1 && newLevel <= 3)
            return TMagicPlusLevel.mpl1_3;
        if (newLevel >= 4 && newLevel <= 6)
            return TMagicPlusLevel.mpl4_6;
        return TMagicPlusLevel.mpl7_9;                 // 7..9 及所有其余值
    }

    /// <summary>
    /// 16570-16581 的 `IsDraw` 判定。**自身与旁人分支不同**：
    /// 自身额外要求 `Self_DrawOrder = mdoPriorMagic` 时 `BeforeDraw and boFlag`、
    /// `mdoPriorSelf` 时 `not BeforeDraw and (not boFlag or 隐身态)`；
    /// 旁人则只看 `BeforeDraw` / `not BeforeDraw`（无 `boFlag` 与隐身态参与）。
    /// 两者共同要求 `Self_File ∈ [0, g_EffectImageList.Count)` 且 `Self_DirCalcType <> mdctCenter`。
    /// </summary>
    public static bool IsDraw(
        bool isMySelf, TMagicClientConfig cfg, int effectImageListCount,
        bool beforeDraw, bool boFlag, bool selfInvisible)
    {
        if (cfg.Self_File < 0 || cfg.Self_File >= effectImageListCount)
            return false;
        if (cfg.Self_DirCalcType == TCustomDirCalcType.mdctCenter)
            return false;

        if (isMySelf)
        {
            return (cfg.Self_DrawOrder == TCustomDrawOrder.mdoPriorMagic && beforeDraw && boFlag)
                || (cfg.Self_DrawOrder == TCustomDrawOrder.mdoPriorSelf && !beforeDraw
                    && (!boFlag || selfInvisible));
        }

        return (cfg.Self_DrawOrder == TCustomDrawOrder.mdoPriorMagic && beforeDraw)
            || (cfg.Self_DrawOrder == TCustomDrawOrder.mdoPriorSelf && !beforeDraw);
    }

    /// <summary>
    /// 16603-16609 的图号计算：
    /// `mdctNone` → `Self_StartIndex + 当前帧`；
    /// `mdctNormal` → `Self_StartIndex + nDir * (Self_PlayCount + Self_EmptyCount) + 当前帧`；
    /// `mdctCenter` → 恒为 -1（不取图，对应原文既不进 if 也不进 else if 的第三分支）。
    /// </summary>
    public static int ImageIndex(TMagicClientConfig cfg, int nDir, int curFrame)
    {
        if (cfg.Self_DirCalcType == TCustomDirCalcType.mdctNone)
            return cfg.Self_StartIndex + curFrame;

        if (cfg.Self_DirCalcType == TCustomDirCalcType.mdctNormal)
            return cfg.Self_StartIndex + nDir * (cfg.Self_PlayCount + cfg.Self_EmptyCount) + curFrame;

        return -1;
    }

    /// <summary>
    /// 帧推进（16541-16565）。
    /// `Self_SyncHumAction`（Delphi Boolean = 1 字节，C# 侧为 byte）非 0 时**每帧无条件跟随人物动作**：
    /// `m_nCurSelfEffFrame := m_nCurrentFrame - m_nStartFrame`、`PlayFrameCount := m_nEndFrame - m_nStartFrame`
    /// （注意此处**不看 tick**，且该赋值无下限保护）。
    /// 否则 `PlayFrameCount := Self_PlayCount`，并在 `now - m_dwCurSelfEffFrameTick >= Self_PlayTime - 10`
    /// 时**仅当 `m_nCurSelfEffFrame < Self_PlayCount - 0`** 才递增，随后无条件刷新 tick。
    /// 返回值即最终的 `PlayFrameCount`。
    /// </summary>
    public static int AdvanceFrame(
        ref SelfMagicEffectState st, TMagicClientConfig cfg, bool isWarrDraw, uint now)
    {
        int playFrameCount;

        if (cfg.Self_SyncHumAction != 0)
        {
            st.m_nCurSelfEffFrame = st.m_nCurrentFrame - st.m_nStartFrame;
            playFrameCount = st.m_nEndFrame - st.m_nStartFrame;
        }
        else
        {
            playFrameCount = cfg.Self_PlayCount;

            // 16548/16556 的两个分支**代码完全相同**（原文如此，仅注释语境不同）
            if (unchecked(now - st.m_dwCurSelfEffFrameTick) >= cfg.Self_PlayTime - PlayTimeTolerance)
            {
                if (st.m_nCurSelfEffFrame < cfg.Self_PlayCount - 0)   // `- 0` 为原文冗余写法
                    st.m_nCurSelfEffFrame++;
                st.m_dwCurSelfEffFrameTick = now;
            }
        }

        return playFrameCount;
    }

    /// <summary>
    /// 应用自身魔法特效 1:1（16518-16629，不含外层 `ClientConfig = nil` 的取配置步骤）。
    /// <paramref name="customMagicConfigFound"/> 与 <paramref name="clientConfig"/> 对应
    /// 16526-16539 的两级可空检查（配置缺失即退出，且**不做任何帧推进**）。
    /// </summary>
    public static List<SelfMagicEffectDrawOp> Apply(
        ref SelfMagicEffectState st,
        bool customMagicConfigFound,
        TMagicClientConfig? clientConfig,
        int dx, int dy,
        bool isWarrDraw, bool beforeDraw, bool boFlag,
        bool isMySelf, bool selfInvisible,
        int effectImageListCount,
        int dir, int mrDir,
        uint now,
        Func<int, int, bool, FxImage?>? resolveImage)
    {
        var ops = new List<SelfMagicEffectDrawOp>();

        // 16518-16520：战士路径不要求 m_boUseMagic
        if (!isWarrDraw)
        {
            if (!st.m_boUseMagic)
                return ops;
        }

        if (!customMagicConfigFound || clientConfig == null)
            return ops;

        var cfg = clientConfig.Value;

        int playFrameCount = AdvanceFrame(ref st, cfg, isWarrDraw, now);

        if (playFrameCount == 0)
            return ops;

        bool isDraw = IsDraw(isMySelf, cfg, effectImageListCount, beforeDraw, boFlag, selfInvisible);

        st.m_nMagLight = cfg.Self_LightRange;          // 16583：**在 IsDraw 之外**写回

        if (!isDraw)
            return ops;

        // 16588-16590：Pascal 中 `and` 优先级高于 `or`，故该式解析为
        // `(PlayFrameCount > 0) and (frame in [0..PlayFrameCount-1]) and (not Self_PlayFailNoDraw)`
        // **`or (ServerMagicCode > 0) or IsWarrDraw`** —— 后两项为真的话，
        // 帧范围守卫会被**整体绕过**（原文如此，逐字保留其分组）
        bool frameRangeOk = playFrameCount > 0
            && st.m_nCurSelfEffFrame >= 0
            && st.m_nCurSelfEffFrame <= playFrameCount - 1;

        bool gate = (frameRangeOk && cfg.Self_PlayFailNoDraw == 0)
            || st.m_CurMagic_ServerMagicCode > 0
            || isWarrDraw;

        if (!gate)
            return ops;

        // 16594-16597：方向计算
        int nDir;
        if ((st.m_CurMagic_targx == -1 && st.m_CurMagic_targy == -1)
            || !isMySelf
            || st.CustomMagicConfigNoChangeDir)
            nDir = dir;
        else
            nDir = mrDir;

        if (resolveImage == null)
            return ops;

        int idx = ImageIndex(cfg, nDir, st.m_nCurSelfEffFrame);
        if (idx < 0)
            return ops;                                 // mdctCenter 不取图

        var d = resolveImage(idx, cfg.Self_File, cfg.Self_DrawMode == TCustomDrawMode.mdmBlend);
        if (d == null)
            return ops;

        ops.Add(new SelfMagicEffectDrawOp(
            dx + d.OriginX + st.m_nShiftX,
            dy + d.OriginY + st.m_nShiftY,
            d.ImageIndex,
            cfg.Self_DrawMode == TCustomDrawMode.mdmBlend,
            "SelfMagicEffect"));

        return ops;
    }
}

/// <summary>DrawSelfMagicEffect 需要读写的角色状态。</summary>
public sealed class SelfMagicEffectState
{
    public bool m_boUseMagic;

    public int m_nCurrentFrame;
    public int m_nStartFrame;
    public int m_nEndFrame;
    public int m_nCurSelfEffFrame;
    public uint m_dwCurSelfEffFrameTick;

    public bool m_CurMagic_ServerMagicCode_Positive;
    public int m_CurMagic_ServerMagicCode;
    public int m_CurMagic_targx = -1;
    public int m_CurMagic_targy = -1;

    public bool CustomMagicConfigNoChangeDir;
    public int m_nShiftX, m_nShiftY;

    /// <summary>16583：`m_nMagLight := ClientConfig.Self_LightRange`（在 IsDraw 之外无条件写回）。</summary>
    public int m_nMagLight;
}
