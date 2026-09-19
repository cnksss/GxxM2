using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>播放特效的一次绘制（GameCanvas.Draw / DrawBlend 的 headless 产物）。</summary>
public sealed record PlayEffectDrawOp(
    int X, int Y, int ImageIndex, bool Blend, string Kind, int OriginX, int OriginY);

/// <summary>
/// Actor.pas 脚本播放特效族（m_ActorEffects）1:1 移植（批次J86，无纹理 headless 化）——
/// CheckLoadPlayEffect(7274-7315) 帧推进、LoadPlayEffectSurface(7317-7353) 取图、
/// DrawPlayEffect(5935-5970) 绘制，以及 THumActor.Finalize(11223-11270) 的队列清理。
/// 队列元素为 **TClientActorEffect 引用**（Delphi 侧是 `pTClientActorEffect` 指针），
/// 故推进/删除直接作用于同一实例。
/// </summary>
public sealed class ActorPlayEffectQueue
{
    /// <summary>队列（Delphi `m_ActorEffects`；元素为引用类型以承载可变播放状态）。</summary>
    public readonly List<TClientActorEffectRef> Items = new();

    /// <summary>g_EffectImageList.Count 接缝。</summary>
    public int EffectImageListCount;

    /// <summary>
    /// 取图接缝：按 `nEffectFileIndex` 与 `nCurrentFrame` 取纹理。
    /// 返回 null 表示该图库不存在或图号为 nil（对应 Delphi 的 `GameImages = nil` 与取图返 nil）。
    /// </summary>
    public Func<int, int, TColorEffect, FxImage?>? ResolveImage;

    /// <summary>m_ColorEffect（7337 的三分支依据）。</summary>
    public TColorEffect ColorEffect = TColorEffect.ceNone;

    /// <summary>
    /// CheckLoadPlayEffect 1:1（7274-7315）：**倒序**遍历（`for I := Count-1 downto 0`）。
    /// `nLoopCount = 0` 的元素被**删除并 Dispose**（倒序正是为了让 Delete 不影响后续下标）；
    /// 否则当 `now - dwEffectTick > wEffectFrameTime` 时推进 `nCurrentFrame`；
    /// 帧超出 `nEffectImageOffSet + wEffectImageCount - 1` 时回卷到 `nEffectImageOffSet`、
    /// 把 `nOldCurrentFrame` 置 **-1**、重置 tick，并在 `nLoopCount > 0` 时递减；
    /// 最后 `nOldCurrentFrame <> nCurrentFrame` 时同步并把返回值置真。
    /// </summary>
    public bool CheckLoadPlayEffect(uint now)
    {
        bool result = false;

        for (int i = Items.Count - 1; i >= 0; i--)
        {
            var e = Items[i];

            if (e.nLoopCount != 0)
            {
                if (now - e.dwEffectTick > e.wEffectFrameTime)
                {
                    e.dwEffectTick = now;
                    e.nCurrentFrame++;
                }

                if (e.nCurrentFrame > e.nEffectImageOffSet + e.wEffectImageCount - 1)
                {
                    e.nCurrentFrame = e.nEffectImageOffSet;
                    e.nOldCurrentFrame = -1;
                    e.dwEffectTick = now;

                    if (e.nLoopCount > 0)
                        e.nLoopCount--;
                }

                if (e.nOldCurrentFrame != e.nCurrentFrame)
                {
                    e.nOldCurrentFrame = e.nCurrentFrame;
                    result = true;
                }
            }
            else
            {
                // 7306-7310：循环耗尽即出队（倒序遍历使 RemoveAt 安全）
                Items.RemoveAt(i);
                result = true;
            }
        }

        return result;
    }

    /// <summary>
    /// LoadPlayEffectSurface 1:1（7317-7353）：**正序**遍历；每项先把纹理置 nil；
    /// 仅当 `nEffectFileIndex ∈ [0, g_EffectImageList.Count)` 且 `nLoopCount <> 0` 时取图，
    /// 并按 `m_ColorEffect` 三档走灰度 / 增亮 / 原色。
    /// 注意：**删除发生在 CheckLoadPlayEffect 而非此处**。
    /// </summary>
    public void LoadPlayEffectSurface()
    {
        foreach (var e in Items)
        {
            e.Texture = null;

            if (e.nEffectFileIndex >= 0 && e.nLoopCount != 0 && e.nEffectFileIndex < EffectImageListCount)
            {
                if (ResolveImage != null)
                    e.Texture = ResolveImage(e.nEffectFileIndex, e.nCurrentFrame, ColorEffect);
            }
        }
    }

    /// <summary>
    /// DrawPlayEffect 1:1（5935-5970）：**骑马时整体退出**（`m_btHorse > 0`）；
    /// 逐项要求 `nLoopCount <> 0`、纹理非空、`nEffectFileIndex ∈ [0, Count)`，
    /// 且 `btDrawOrder` 与 `IsBackActor` **配对**（0 配正面、非 0 配背面）；
    /// 位置 = `ddx + nX + nOffsetX + m_nShiftX`、`ddy + nY + nOffsetY + m_nShiftY`。
    /// </summary>
    public List<PlayEffectDrawOp> DrawPlayEffect(
        int ddx, int ddy, bool isBackActor, int m_btHorse, int m_nShiftX, int m_nShiftY)
    {
        var ops = new List<PlayEffectDrawOp>();

        if (m_btHorse > 0)
            return ops;

        for (int i = 0; i < Items.Count; i++)
        {
            var e = Items[i];

            if (e.nLoopCount == 0)
                continue;

            if (!((e.btDrawOrder == 0 && !isBackActor) || (e.btDrawOrder != 0 && isBackActor)))
                continue;

            if (e.Texture == null)
                continue;
            if (e.nEffectFileIndex < 0 || e.nEffectFileIndex >= EffectImageListCount)
                continue;

            var d = (FxImage)e.Texture;
            ops.Add(new PlayEffectDrawOp(
                ddx + e.nX + e.nOffsetX + m_nShiftX,
                ddy + e.nY + e.nOffsetY + m_nShiftY,
                d.ImageIndex, e.boBlendMode, "PlayEffect", d.OriginX, d.OriginY));
        }

        return ops;
    }

    /// <summary>
    /// THumActor.Finalize 的队列段（11248-11257）1:1：**仅把每项 Texture 置 nil**，
    /// 既不置 `boWantDelete` 也不清空队列（`boWantDelete` 由 ClMain.pas 21079 的另一条路径设置）。
    /// </summary>
    public void FinalizeEffects()
    {
        foreach (var e in Items)
            e.Texture = null;
    }

    /// <summary>
    /// TActor 析构时的队列释放（2956-2959）1:1：逐项 Dispose 后 Free 整个队列。
    /// </summary>
    public void DestroyEffects()
    {
        Items.Clear();
    }

    /// <summary>入队一个新特效（脚本命令路径；Delphi 侧为 New(pTClientActorEffect)）。</summary>
    public TClientActorEffectRef Add()
    {
        var e = new TClientActorEffectRef
        {
            nOldLoopCount = 0,
            nLoopCount = 0,
            nOldCurrentFrame = -1,
            nCurrentFrame = 0,
        };
        Items.Add(e);
        return e;
    }
}
