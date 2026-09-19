using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>一次特效/图标绘制（GameCanvas.Draw 或 DrawBlend 的 headless 产物）。</summary>
public sealed record SelfEffectDrawOp(
    int X, int Y, int ImageIndex, bool Blend, string Kind, bool Gray);

/// <summary>TGetNearObjectHintInfo 的产物。</summary>
public sealed record NearObjectHintResult(bool Visible, int X, int Y, int FriendFlag);

/// <summary>
/// Actor.pas 自身特效族 1:1 移植（批次J85，无纹理 headless 化）——
/// DrawSelfEffect(8751-8892)、ShowIcons(8601-8665)、DrawExploreItemEffect(8926-8959)、
/// DrawLockTargetEffect(8894-8907)、GetNearObjectHintInfo(8910-8924)。
/// </summary>
public static class ActorSelfEffectRender
{
    /// <summary>8762：自定义状态特效的帧间隔。</summary>
    public const int StatusEffectTickMs = 120;

    /// <summary>8765/8767：索引与受击计数的回卷上限。</summary>
    public const int StatusAniIndexWrap = 100000;
    public const int StatusStruckWrap = 100000;

    /// <summary>8805/8807：状态特效的纵向偏移（原文硬编码 +2）。</summary>
    public const int StatusEffectOffsetY = 2;

    /// <summary>8938：死亡后延迟 800ms 才开始播探索图标。</summary>
    public const int ExploreItemDelayMs = 800;

    /// <summary>8954：探索图标的纵向基准偏移（原文硬编码 -20）。</summary>
    public const int ExploreItemOffsetY = -20;

    /// <summary>TActorIconArray 的元素数（Actor.pas IconArray 上下界）。</summary>
    public const int ActorIconCount = 3;

    /// <summary>每帧的取图接缝：GetCachedImage(idx) / GetCachedGrayImage(idx) → 尺寸/原点。</summary>
    public delegate FxImage? ImageLookup(int imageIndex, bool gray);

    /// <summary>
    /// DrawSelfEffect 1:1（8751-8892）。分三段：
    /// ① 非背面（`not IsBackActor`）时的自定义魔法状态特效 + 自身持续特效；
    /// ② `IsDraw` 判定（背面看 `mdoPriorMagic`、正面看 `mdoPriorSelf`）；
    /// ③ SelfKeepPlay 的两层（StartIndex / StartIndex2）。
    /// </summary>
    public static List<SelfEffectDrawOp> DrawSelfEffect(
        ActorSelfEffectState st,
        int dx, int dy, bool isBackActor,
        uint now,
        Func<int, int, bool, FxImage?>? resolveEffectFile,
        ImageLookup? resolveStatusImage,
        ImageLookup? resolveSelfEffectImage,
        ImageLookup? resolveSelfKeepImage,
        bool selfDead)
    {
        var ops = new List<SelfEffectDrawOp>();

        if (!isBackActor)
        {
            // ---- ① 自定义魔法状态特效（8761-8811）----
            if (st.CustomMagicStatusEffect.boShow && st.m_btHorse == 0)
            {
                if (now - st.CustomMagicStatusEffect.m_nGenAniTick > StatusEffectTickMs)
                {
                    st.CustomMagicStatusEffect.m_nGenAniTick = now;
                    st.CustomMagicStatusEffect.m_nGenAniIndex++;
                    if (st.CustomMagicStatusEffect.m_nGenAniIndex > StatusAniIndexWrap)
                        st.CustomMagicStatusEffect.m_nGenAniIndex = 0;
                    st.CustomMagicStatusEffect.m_nStruck++;
                    if (st.CustomMagicStatusEffect.m_nStruck > StatusStruckWrap)
                        st.CustomMagicStatusEffect.m_nStruck = StatusStruckWrap;
                }

                bool haveWimg = false;
                int idx = 0;
                bool isBlendDraw = false;
                var cms = st.CustomMagicStatusEffect;

                if (st.m_nCurrentAction == TActorCore.SM_STRUCK
                    && cms.m_nStruck < cms.Status2_PlayCount
                    && cms.Status2_File >= 0
                    && cms.Status2_File < st.EffectImageListCount)
                {
                    if (cms.Status2_CalcDir)
                        idx = cms.Status2_StartIndex + (cms.Status2_PlayCount + cms.Status2_EmptyCount) * st.m_btDir + cms.m_nStruck;
                    else
                        idx = cms.Status2_StartIndex + cms.m_nStruck;

                    haveWimg = true;
                    isBlendDraw = cms.Status2_DrawMode == (int)TCustomDrawMode.mdmBlend;
                }
                else
                {
                    if (cms.Status1_PlayCount > 0 && cms.Status1_File >= 0 && cms.Status1_File < st.EffectImageListCount)
                    {
                        if (cms.Status2_CalcDir)
                        {
                            // 8785：`Status2_CalcDir`（原文疑为笔误，逐字保留）
                            idx = cms.Status1_StartIndex + (cms.Status1_PlayCount + cms.Status1_EmptyCount) * st.m_btDir
                                + (cms.m_nGenAniIndex % cms.Status1_PlayCount);
                        }
                        else
                        {
                            idx = cms.Status1_StartIndex + (cms.m_nGenAniIndex % cms.Status1_PlayCount);
                        }

                        haveWimg = true;
                        isBlendDraw = cms.Status1_DrawMode == (int)TCustomDrawMode.mdmBlend;
                    }
                }

                if (haveWimg && resolveStatusImage != null)
                {
                    var d = resolveStatusImage(idx, selfDead);
                    if (d != null)
                    {
                        ops.Add(new SelfEffectDrawOp(
                            dx + d.OriginX + st.m_nShiftX,
                            dy + d.OriginY + st.m_nShiftY + StatusEffectOffsetY,
                            d.ImageIndex, isBlendDraw, "StatusEffect", selfDead));
                    }
                }
            }

            // ---- ③' 自身持续特效（8813-8832）----
            if (st.m_boSelfEffectRunning && st.HasSelfEffectGameImage)
            {
                if (now - st.m_dwSelfEffectLastTick >= st.m_nSelfEffectFrameTime)
                {
                    st.m_dwSelfEffectLastTick = now;
                    st.m_nSelfEffectCurrentFrame++;
                }

                if (st.m_nSelfEffectCurrentFrame > st.m_nSelfEffectEndFrame)
                {
                    st.m_boSelfEffectRunning = false;
                    return ops;                                  // 8821：直接 Exit，跳过 SelfKeepPlay
                }

                if (resolveSelfEffectImage != null)
                {
                    var d = resolveSelfEffectImage(st.m_nSelfEffectCurrentFrame, false);
                    if (d != null)
                    {
                        ops.Add(new SelfEffectDrawOp(
                            dx + d.OriginX + st.m_nShiftX,
                            dy + d.OriginY + st.m_nShiftY,
                            d.ImageIndex, st.m_boSelfEffectBlendDraw, "SelfEffect", false));
                    }
                }
            }
        }

        // ---- ② IsDraw 判定（8835-8838）----
        bool isDraw = isBackActor
            ? st.SelfKeepPlay.SelfPlay.SelfKeep_DrawOrder == TCustomDrawOrder.mdoPriorMagic
            : st.SelfKeepPlay.SelfPlay.SelfKeep_DrawOrder == TCustomDrawOrder.mdoPriorSelf;

        if (isDraw)
        {
            var sp = st.SelfKeepPlay.SelfPlay;

            if (now - st.SelfKeepPlay.SelfKeep_StartTime <= (uint)(sp.SelfKeep_KeepTime * 1000)
                && st.SelfKeepPlay.HasImages
                && sp.SelfKeep_PlayCount > 0
                && sp.SelfKeep_KeepTime > 0)
            {
                if (now - st.SelfKeepPlay.SelfKeep_LastTick >= sp.SelfKeep_PlayTime)
                {
                    st.SelfKeepPlay.SelfKeep_Index++;
                    if (st.SelfKeepPlay.SelfKeep_Index >= sp.SelfKeep_PlayCount)
                        st.SelfKeepPlay.SelfKeep_Index = 0;

                    st.SelfKeepPlay.SelfKeep_LastTick = now;
                }

                if (sp.SelfKeep_StartIndex >= 0 && resolveSelfKeepImage != null)
                {
                    var d = resolveSelfKeepImage(sp.SelfKeep_StartIndex + st.SelfKeepPlay.SelfKeep_Index, selfDead);
                    if (d != null)
                    {
                        ops.Add(new SelfEffectDrawOp(
                            dx + d.OriginX + st.m_nShiftX,
                            dy + d.OriginY + st.m_nShiftY,
                            d.ImageIndex, sp.SelfKeep_DrawMode == (int)TCustomDrawMode.mdmBlend, "SelfKeep1", selfDead));
                    }
                }

                if (sp.SelfKeep_StartIndex2 >= 0 && resolveSelfKeepImage != null)
                {
                    var d = resolveSelfKeepImage(sp.SelfKeep_StartIndex2 + st.SelfKeepPlay.SelfKeep_Index, selfDead);
                    if (d != null)
                    {
                        ops.Add(new SelfEffectDrawOp(
                            dx + d.OriginX + st.m_nShiftX,
                            dy + d.OriginY + st.m_nShiftY,
                            d.ImageIndex, sp.SelfKeep_DrawMode2 == (int)TCustomDrawMode.mdmBlend, "SelfKeep2", selfDead));
                    }
                }
            }
        }

        return ops;
    }

    /// <summary>
    /// ShowIcons 1:1（8601-8665）：`m_boShopStall` / 死亡 / 不可绘 三道退出；
    /// 再按 ckHideActorIcons（仅玩家与英雄）与 ckHideMonsterIcons（怪物）两道可选退出；
    /// HP 条偏移按种族三档（玩家/英雄 → Hum、商人 → Npc、其余 → Mon）；
    /// 逐图标要求 `Texture <> nil` 且 `nFileIndex ∈ [0, EffectImageList.Count)` 且 `nIconCount > 0`；
    /// `nX = m_nSayX - DefTextureWidth div 2 + nX + IconIndexX`、`nY = m_nSayY - 32 + nY + IconIndexY`；
    /// **绘制与否按 `btDrawOrder` 与 `IsBackActor` 的配对**（0 配正面、非 0 配背面）。
    /// </summary>
    public static List<SelfEffectDrawOp> ShowIcons(
        ActorIconRenderState st,
        bool isBackActor)
    {
        var ops = new List<SelfEffectDrawOp>();

        // 8608 的 `if m_btHorse > 0 then Exit;` 已被注释掉 —— 不生效
        if (st.m_boShopStall)
            return ops;
        if (st.m_boDeath || !st.m_boCanDraw)
            return ops;

        if (st.ckHideActorIcons)
        {
            if ((st.m_btRace == ActorLabelConsts.RC_PLAYOBJECT && !st.m_boPlayMoster)
                || st.m_btRace == ActorLabelConsts.RC_HEROOBJECT)
                return ops;
        }

        if (st.ckHideMonsterIcons)
        {
            if (IsMonster(st.m_btRace, st.m_boPlayMoster))
                return ops;
        }

        int hpBarOffsetX, hpBarOffsetY;
        if (st.m_btRace == ActorLabelConsts.RC_PLAYOBJECT || st.m_btRace == ActorLabelConsts.RC_HEROOBJECT)
        {
            hpBarOffsetX = st.nHumHPBarOffsetX;
            hpBarOffsetY = st.nHumHPBarOffsetY;
        }
        else if (st.m_btRace == ActorLabelConsts.RC_MERCHANT)
        {
            hpBarOffsetX = st.nNpcHPBarOffsetX;
            hpBarOffsetY = st.nNpcHPBarOffsetY;
        }
        else
        {
            hpBarOffsetX = st.nMonHPBarOffsetX;
            hpBarOffsetY = st.nMonHPBarOffsetY;
        }

        for (int i = 0; i < ActorIconCount; i++)
        {
            var ic = st.ActorIcons[i];
            var idxs = st.ActorIconIndexs[i];

            if (!idxs.HasTexture || ic.nFileIndex < 0 || ic.nFileIndex >= st.EffectImageListCount || ic.nIconCount <= 0)
                continue;

            if (!idxs.HasTexture)
                continue;

            // 8651：用 DefTextureWidth 而非 Texture.Width（原文注释掉的旧写法）
            int nX = st.m_nSayX - idxs.DefTextureWidth / 2 + ic.nX + idxs.nX;
            int nY = st.m_nSayY - 32 + ic.nY + idxs.nY;

            if ((ic.btDrawOrder == 0 && !isBackActor) || (ic.btDrawOrder != 0 && isBackActor))
            {
                ops.Add(new SelfEffectDrawOp(nX + hpBarOffsetX, nY + hpBarOffsetY,
                    ic.nFileIndex, ic.boBlend, "Icon", false));
            }
        }

        return ops;
    }

    /// <summary>
    /// IsMonster（Grobal2.pas 6545-6564）1:1：`RC_PLAYOBJECT` 时直接返回 `boHumMonster`；
    /// 否则判定是否属于 **NOT_MONSTER_RACES** 集合，**结果取反**（`Result := not bMonster`）。
    /// 该集合是「非怪物」种族清单，而非怪物清单——集合注释明确指出 `RC_PLAYOBJECT` 已在前面排除、
    /// `RC_PLAYMOSTER` 被注释掉。逐字保留集合内容与取反语义。
    /// </summary>
    public static bool IsMonster(int btRace, bool boHumMonster)
    {
        if (btRace == ActorLabelConsts.RC_PLAYOBJECT)
            return boHumMonster;

        bool bMonster = Array.IndexOf(NotMonsterRaces, btRace) >= 0;
        return !bMonster;
    }

    /// <summary>
    /// Grobal2.pas 6555-6561 的种族集合（**非怪物**）：
    /// `RC_PLAYOBJECT` 已被前面排除故不在集合内；`RC_PLAYMOSTER` 被原文注释掉；
    /// 23 = 变异骷髅、54 = 神兽为字面量。
    /// </summary>
    public static readonly int[] NotMonsterRaces =
    {
        ActorLabelConsts.RC_HEROOBJECT,
        ActorLabelConsts.RC_NPC,
        ActorLabelConsts.RC_GUARD,
        ActorLabelConsts.RC_GUARD2,
        ActorLabelConsts.RC_PEACENPC,
        ActorLabelConsts.RC_ANIMAL,
        ActorLabelConsts.RC_MOONOBJECT,
        ActorLabelConsts.RC_ARCHERGUARD,
        ActorLabelConsts.RC_TRUCKOBJECT,
        ActorLabelConsts.RC_MOVE_ARCHERGUARD,
        23,     // 变异骷髅
        54,     // 神兽
    };

    /// <summary>
    /// DrawExploreItemEffect 1:1（8926-8959）：三道退出（未死亡 / 非探索物品 / 骷髅）；
    /// `dwExploreItemIconCount <= 0` 退出；死亡不足 800ms 时**重置 tTick 与 frame 后 Exit**；
    /// 其后按 `dwExploreItemIconPlayTime` 推进帧、按 `dwExploreItemIconCount` 回卷；
    /// 图号 = `dwExploreItemIconIndex + frame`；
    /// 位置 = (`m_nSayX + nExploreItemIconOffsetX`, `m_nSayY - 20 + nExploreItemIconOffsetY`) + 图原点。
    /// </summary>
    public static List<SelfEffectDrawOp> DrawExploreItemEffect(
        ActorExploreItemState st, uint now, ImageLookup resolve)
    {
        var ops = new List<SelfEffectDrawOp>();

        if (!st.m_boDeath)
            return ops;
        if (!st.m_IsExploreItem)
            return ops;
        if (st.m_boSkeleton)
            return ops;
        if (st.dwExploreItemIconCount <= 0)
            return ops;

        if (TickDiff(st.m_dwDeathTick, now) < ExploreItemDelayMs)
        {
            st.m_dwExploreItemEffectTick = now;
            st.m_dwExploreItemEffectFrame = 0;
            return ops;
        }

        if (TickDiff(st.m_dwExploreItemEffectTick, now) >= st.dwExploreItemIconPlayTime)
        {
            st.m_dwExploreItemEffectFrame++;
            st.m_dwExploreItemEffectTick = now;
            if (st.m_dwExploreItemEffectFrame >= st.dwExploreItemIconCount)
                st.m_dwExploreItemEffectFrame = 0;
        }

        var d = resolve(st.dwExploreItemIconIndex + st.m_dwExploreItemEffectFrame, false);
        if (d == null)
            return ops;

        int nX = st.m_nSayX + st.nExploreItemIconOffsetX;
        int nY = st.m_nSayY + ExploreItemOffsetY + st.nExploreItemIconOffsetY;

        ops.Add(new SelfEffectDrawOp(nX + d.OriginX, nY + d.OriginY, d.ImageIndex, false, "ExploreItem", false));
        return ops;
    }

    /// <summary>
    /// DrawLockTargetEffect 1:1（8894-8907）：`ckShowTargetAperture` 未勾选则退出；
    /// 命中纹理后以 `(dx + 原点X + m_nShiftX, dy + 原点Y + m_nShiftY)` 绘制（无 blend、无灰度）。
    /// </summary>
    public static List<SelfEffectDrawOp> DrawLockTargetEffect(
        int frame, int dx, int dy, bool ckShowTargetAperture,
        bool hasImages, int shiftX, int shiftY, ImageLookup resolve)
    {
        var ops = new List<SelfEffectDrawOp>();

        if (!ckShowTargetAperture)
            return ops;
        if (!hasImages)
            return ops;

        var d = resolve(frame, false);
        if (d == null)
            return ops;

        ops.Add(new SelfEffectDrawOp(dx + d.OriginX + shiftX, dy + d.OriginY + shiftY,
            d.ImageIndex, false, "LockTarget", false));
        return ops;
    }

    /// <summary>
    /// GetNearObjectHintInfo 1:1（8910-8924）：**排除自身**、未死亡、且在 BossList 中；
    /// 命中后把 X/Y 各自加上 `m_nShiftX/Y`，并按好友列表决定 `nFriendFlag`（1/0）。
    /// </summary>
    public static NearObjectHintResult GetNearObjectHintInfo(
        bool isMySelf, bool m_boDeath, bool inBossList, bool inFriendList,
        int x, int y, int shiftX, int shiftY)
    {
        if (isMySelf || m_boDeath || !inBossList)
            return new NearObjectHintResult(false, x, y, 0);

        return new NearObjectHintResult(true, x + shiftX, y + shiftY, inFriendList ? 1 : 0);
    }

    /// <summary>Actor.pas tick_diff（无符号回绕差）。</summary>
    public static uint TickDiff(uint start, uint now) => now - start;
}

/// <summary>DrawSelfEffect 所需的角色状态（ActorCore 侧接缝）。</summary>
public sealed class ActorSelfEffectState
{
    public TCustomMagicStatusEffect CustomMagicStatusEffect;
    public int m_btHorse;
    public int m_btDir;
    public int m_nCurrentAction;
    public int EffectImageListCount;
    public int m_nShiftX, m_nShiftY;

    public bool m_boSelfEffectRunning;
    public bool HasSelfEffectGameImage;
    public uint m_dwSelfEffectLastTick;
    public int m_nSelfEffectFrameTime;
    public int m_nSelfEffectCurrentFrame;
    public int m_nSelfEffectEndFrame;
    public bool m_boSelfEffectBlendDraw;

    public SelfKeepPlayRuntime SelfKeepPlay = new();
}

/// <summary>m_SelfKeepPlay（Actor.pas 146-148）：记录 + 运行期字段。</summary>
public sealed class SelfKeepPlayRuntime
{
    public TSelfKeepPlay SelfPlay;
    public bool HasImages;
    public uint SelfKeep_StartTime;
    public uint SelfKeep_LastTick;
    public int SelfKeep_Index;
}

/// <summary>ShowIcons 所需的角色状态。</summary>
public sealed class ActorIconRenderState
{
    public int m_btRace;
    public bool m_boPlayMoster;
    public bool m_boDeath;
    public bool m_boCanDraw = true;
    public bool m_boShopStall;
    public int m_nSayX, m_nSayY;
    public bool ckHideActorIcons;
    public bool ckHideMonsterIcons;
    public int EffectImageListCount;

    public int nHumHPBarOffsetX, nHumHPBarOffsetY;
    public int nNpcHPBarOffsetX, nNpcHPBarOffsetY;
    public int nMonHPBarOffsetX, nMonHPBarOffsetY;

    public TActorIcon[] ActorIcons = new TActorIcon[ActorSelfEffectRender.ActorIconCount];
    public TActorIconIndex[] ActorIconIndexs = new TActorIconIndex[ActorSelfEffectRender.ActorIconCount];

    public ActorIconRenderState()
    {
        for (int i = 0; i < ActorIcons.Length; i++)
        {
            ActorIcons[i] = new TActorIcon();
            ActorIconIndexs[i] = new TActorIconIndex();
        }
    }
}

/// <summary>Actor.pas TActorIcon（图标定义）。</summary>
public sealed class TActorIcon
{
    public int nFileIndex = -1;
    public int nIconCount;
    public int nX, nY;
    public int btDrawOrder;
    public bool boBlend;
}

/// <summary>Actor.pas TActorIconIndex（图标索引与运行期纹理）。</summary>
public sealed class TActorIconIndex
{
    public bool HasTexture;
    public int DefTextureWidth;
    public int nX, nY;
}

/// <summary>DrawExploreItemEffect 所需的角色状态。</summary>
public sealed class ActorExploreItemState
{
    public bool m_boDeath;
    public bool m_IsExploreItem;
    public bool m_boSkeleton;
    public int m_nSayX, m_nSayY;
    public uint m_dwDeathTick;
    public uint m_dwExploreItemEffectTick;
    public int m_dwExploreItemEffectFrame;
    public int dwExploreItemIconCount;
    public int dwExploreItemIconPlayTime;
    public int dwExploreItemIconIndex;
    public int nExploreItemIconOffsetX;
    public int nExploreItemIconOffsetY;
}
