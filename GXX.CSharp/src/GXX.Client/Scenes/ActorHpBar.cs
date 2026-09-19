using System;
using System.Collections.Generic;
using GXX.Core.Protocol;

namespace GXX.Client.Scenes;

/// <summary>血条贴图索引（g_NewopUI170TextureArray 的下标语义，原文以数字字面量出现）。</summary>
public static class HpBarTextureIndex
{
    /// <summary>底图（各种颜色血条图像的数组第 0 张）。</summary>
    public const int Background = 0;

    /// <summary>人物/英雄血量（原文索引 1）。</summary>
    public const int HumHP = 1;

    /// <summary>人物高亮显血（原文索引 2）。</summary>
    public const int HighlightHP = 2;

    /// <summary>内功黄条（原文索引 3）。</summary>
    public const int NgYellow = 3;

    /// <summary>NPC 血量（原文索引 4）。</summary>
    public const int NpcHP = 4;

    /// <summary>怪物血量（原文索引 5）。</summary>
    public const int MonsterHP = 5;

    /// <summary>守卫血量（原文索引 6）。</summary>
    public const int GuardHP = 6;

    /// <summary>护身蓝条 / MP（原文索引 7）。</summary>
    public const int MagicShieldMP = 7;
}

/// <summary>DrawActorLabel 中的一次绘制（GameCanvas.Draw 的 headless 产物）。</summary>
public sealed record HpBarDrawOp(int X, int Y, int TexIndex, int SrcLeft, int SrcRight, int SrcBottom, string Kind);

/// <summary>自定义 NPC/怪物血条配置（PClientBaseConfig 中血条相关字段）。</summary>
public sealed class HpBarBaseConfig
{
    public int HPFile = -1;
    public int HPStartIndex = -1;
    public int HPBgOffsetX;
    public int HPBgOffsetY;
    public int HPOffsetX;
    public int HPOffsetY;
}

/// <summary>自定义 NPC 配置（PClientCustomNpcConfig 中血条相关字段）。</summary>
public sealed class HpBarNpcConfig
{
    public HpBarBaseConfig BaseConfig = new();
}

/// <summary>取图接缝：按 (文件索引, 图索引) 返回贴图（null = 无）。</summary>
public interface IHpBarImageSource
{
    LabelSurface? GetEffectImage(int fileIndex, int imageIndex);

    /// <summary>g_WMonImages.Images[appearance]。</summary>
    LabelSurface? GetMonImage(int appearance);

    /// <summary>g_WMonImages.Images[appearance] 与 其 [+1]，用于自定义怪物的背景+血条对。</summary>
    LabelSurface? GetMonImagePair(int appearance, int offset);
}

/// <summary>
/// TPlayScene.DrawActorLabel（PlayScn.pas 3581-3938）血条绘制族（批次J79）——
/// 纯几何 + 配色 + 守卫判定，与真实画布无关。
/// </summary>
public partial class TPlaySceneCore
{
    /// <summary>g_NewopUI170TextureArray[0..7]。</summary>
    public static readonly LabelSurface?[] NewopUI170TextureArray = new LabelSurface?[8];

    /// <summary>取图接缝。</summary>
    public static IHpBarImageSource? HpBarImages;

    /// <summary>g_EffectImageList.Count（自定义血条资源越界判定）。</summary>
    public static Func<int> EffectImageListCountFn = () => 0;

    /// <summary>GetCustomNpcConfig(appearance)。</summary>
    public static Func<int, HpBarNpcConfig?> GetCustomNpcConfigFn = _ => null;

    /// <summary>MyGetTickCount。</summary>
    public static Func<long> HpBarTickFn = () => SceneTime.TickNow();

    /// <summary>g_boAppExit（全局退出标志，绘制入口即判）。</summary>
    public static bool AppExit;

    // ---- g_ClientConfig ----
    public static int nHumHPBarOffsetX, nHumHPBarOffsetY;
    public static int nNpcHPBarOffsetX, nNpcHPBarOffsetY;
    public static int nMonHPBarOffsetX, nMonHPBarOffsetY;
    public static bool boShowHPLabel;
    public static bool boHumStruckShowNumber;
    public static bool boMonStruckShowNumber;
    public static bool boPetNoShowHPProgress;
    public static bool boShowMagicShieldHP;
    public static bool boShowHighlightHPLabel;
    public static bool boShowNGLabel;

    // ---- g_ConfigDlg.ConfigCheckeds ----
    public static bool ckShowHPLabel;
    public static bool ckShowNpcHPLabel;
    public static bool ckShowHighlightHPLabel;
    public static bool ckShowNGLabel;

    /// <summary>PlugInEnabled。</summary>
    public static bool PlugInEnabled = true;

    /// <summary>g_nRenderCode（原文异常诊断用；headless 仅记录）。</summary>
    public static int g_nRenderCode;

    /// <summary>DrawActorLabel 上下文（主角/英雄/焦点引用）。</summary>
    public TActor? MySelf;
    public TActor? MyHero;
    public TActor? FocusCret;
    public bool boSelectMyself;

    /// <summary>IsValidActorEx 接缝（默认恒真）。</summary>
    public static Func<TActor, bool> IsValidActorExFn = _ => true;

    /// <summary>DrawPreviewItem 接缝（原文在该函数末尾调用）。</summary>
    public Action? DrawPreviewItemFn;

    /// <summary>
    /// 绘制单个角色的血条（DrawActorLabel 循环体 3626-3835 的 1:1 抽取）。
    /// 所有 GameCanvas.Draw 调用转成 HpBarDrawOp 返回，供调用方/测试核对。
    /// </summary>
    public List<HpBarDrawOp> PlanActorHpBar(TActor actor)
    {
        var ops = new List<HpBarDrawOp>();
        if (actor.m_boDeath || !actor.m_boCanDraw || actor.m_boShopStall)
            return ops;

        // 3627-3629：开启显血超时自动关闭
        if (actor.m_noInstanceOpenHealth)
        {
            if (HpBarTickFn() - actor.m_dwOpenHealthStart > actor.m_dwOpenHealthTime)
                actor.m_noInstanceOpenHealth = false;
        }

        // 3631-3640：种族 HP 偏移三支
        int hpBarOffsetX, hpBarOffsetY;
        if (actor.m_btRace is ActorLabelConsts.RC_PLAYOBJECT or ActorLabelConsts.RC_HEROOBJECT)
        {
            hpBarOffsetX = nHumHPBarOffsetX;
            hpBarOffsetY = nHumHPBarOffsetY;
        }
        else if (actor.m_btRace == ActorLabelConsts.RC_MERCHANT)
        {
            hpBarOffsetX = nNpcHPBarOffsetX;
            hpBarOffsetY = nNpcHPBarOffsetY;
        }
        else
        {
            hpBarOffsetX = nMonHPBarOffsetX;
            hpBarOffsetY = nMonHPBarOffsetY;
        }

        // 3642-3643：总开关
        if ((PlugInEnabled && boShowHPLabel && ckShowHPLabel)
            || actor.m_noInstanceOpenHealth || actor.m_boOpenHealth)
        {
            g_nRenderCode = 51;
            // 3645：Abil := Actor.m_Abil 是**值拷贝**——后续所有对 Abil 的改写
            // （MaxHP/MaxMP 夹紧、商人强制 1、HP/MP 还原）都只作用于这个局部副本，
            // 不会写回 actor.m_Abil。逐字保留此语义。
            var abil = actor.m_Abil;

            // 3647-3651：夹紧上限
            if (abil.MaxHP < abil.HP)
                abil.MaxHP = abil.HP;
            if (abil.MaxMP < abil.MP)
                abil.MaxMP = abil.MP;

            // 3653-3660：贴图矩形
            var d = NewopUI170TextureArray[HpBarTextureIndex.Background];
            int rcLeft = 0, rcTop = 0, rcRight, rcBottom;
            if (d != null)
            {
                rcRight = d.Width;
                rcBottom = d.Height;
            }
            else
            {
                rcRight = 32;
                rcBottom = 3;
            }

            int hpImgWidth = rcRight;
            int hpImgHeigh = rcBottom;

            int hpOffsetX = hpImgWidth / 2;
            int hpOffsetY = hpImgHeigh * 2 + 4;

            g_nRenderCode = 54;

            // 3667-3672：商人/和平NPC 血量为 0 时强制 1
            if (actor.m_btRace == ActorLabelConsts.RC_MERCHANT || actor.m_btRace == ActorLabelConsts.RC_PEACENPC)
            {
                if (abil.MaxHP <= 0)
                {
                    abil.HP = 1;
                    abil.MaxHP = 1;
                }
            }

            uint hp = abil.HP;
            uint mp = abil.MP;
            abil.HP = abil.MaxHP;
            abil.MP = abil.MaxMP;

            bool boShowHPLabel = true;

            // 3681-3711：按种族修正 boShowHPLabel 与 HP/MP 显示值
            if (actor.m_btRace is ActorLabelConsts.RC_PLAYOBJECT or ActorLabelConsts.RC_HEROOBJECT)
            {
                if ((boHumStruckShowNumber && actor.m_boStruckShowNumber)
                    || !boHumStruckShowNumber
                    || ReferenceEquals(actor, MySelf)
                    || ReferenceEquals(actor, MyHero)
                    || actor.m_boOpenHealth)
                {
                    // 原文此处 ckShowNumberLable 那段被注释掉，仅保留 (MaxHP > 0)
                    if (abil.MaxHP > 0)
                    {
                        abil.HP = hp;
                        abil.MP = mp;
                    }
                }
            }
            else if (actor.m_btRace == ActorLabelConsts.RC_MERCHANT)
            {
                if (ckShowNpcHPLabel && !IsNpcAppearanceExcluded(actor.m_wAppearance))
                    boShowHPLabel = true;
                else
                    boShowHPLabel = false;
            }
            else if (abil.MaxHP > 0)
            {
                if (boPetNoShowHPProgress && actor.m_HumsBBType == THumBBType.bbGamePet)
                {
                    boShowHPLabel = false;
                }
                else if ((boMonStruckShowNumber && actor.m_boStruckShowNumber)
                    || !boMonStruckShowNumber || actor.m_boOpenHealth)
                {
                    abil.HP = hp;
                    abil.MP = mp;
                }
            }
            else
            {
                if (boPetNoShowHPProgress && actor.m_HumsBBType == THumBBType.bbGamePet)
                    boShowHPLabel = false;
            }

            if (boShowHPLabel)
            {
                // 3714-3717：按 HP/MaxHP 比例收缩右边界
                if (abil.MaxHP > 0)
                {
                    if (abil.HP < abil.MaxHP)
                        rcRight = rcLeft + (int)Math.Round((double)(rcRight - rcLeft) / abil.MaxHP * abil.HP);
                }

                // 3719-3743：护身属性 → 新资源（蓝条）
                if (boShowMagicShieldHP && actor.m_boMagicShield)
                {
                    int rc2Left = 0, rc2Right, rc2Bottom = hpImgHeigh;

                    if (abil.Level == 0 && !ReferenceEquals(actor, MySelf))
                    {
                        // rc2 := rc（保留已收缩的 rc）；rc := Rect(0,0,0,HpImgHeigh)
                        rc2Left = rcLeft;
                        rc2Right = rcRight;
                        rcRight = 0;
                    }
                    else if (actor.m_btJob == 0 && abil.Level < 28)
                    {
                        rc2Right = 0;
                    }
                    else
                    {
                        rc2Right = hpImgWidth;
                        if (abil.MaxMP > 0)
                        {
                            if (abil.MP < abil.MaxMP)
                                rc2Right = rc2Left + (int)Math.Round((double)(rc2Right - rc2Left) / abil.MaxMP * abil.MP);
                        }
                    }

                    int bx = actor.m_nSayX - hpOffsetX + hpBarOffsetX;
                    int by = actor.m_nSayY - hpOffsetY + hpBarOffsetY;

                    ops.Add(new HpBarDrawOp(bx, by, HpBarTextureIndex.Background, 0, 0, 0, "ShieldBg"));

                    if (rcRight - rcLeft > 0)
                        ops.Add(new HpBarDrawOp(bx, by, HpBarTextureIndex.HumHP, rcLeft, rcRight, rcBottom, "ShieldHP"));

                    if (rc2Right - rc2Left > 0)
                        ops.Add(new HpBarDrawOp(bx, by, HpBarTextureIndex.MagicShieldMP, rc2Left, rc2Right, rc2Bottom, "ShieldMP"));
                }
                else
                {
                    // 3744-3832：NPC/怪物分支
                    int nX = actor.m_nSayX - hpOffsetX;
                    int nY = actor.m_nSayY - hpOffsetY;

                    if (actor.m_btRace == ActorLabelConsts.RC_MERCHANT)
                    {
                        PlanMerchantHpBar(actor, ops, nX, nY, hpBarOffsetX, hpBarOffsetY, rcLeft, rcRight, rcBottom);
                    }
                    else if (actor.m_btRealRace == PlaySceneConsts.RC_GUARD
                        || actor.m_btRealRace == PlaySceneConsts.RC_ARCHERGUARD)
                    {
                        ops.Add(new HpBarDrawOp(nX + hpBarOffsetX, nY + hpBarOffsetY,
                            HpBarTextureIndex.Background, 0, 0, 0, "GuardBg"));
                        ops.Add(new HpBarDrawOp(nX + hpBarOffsetX, nY + hpBarOffsetY,
                            HpBarTextureIndex.GuardHP, rcLeft, rcRight, rcBottom, "GuardHP"));
                    }
                    else if (actor.IsCustomActor && actor.CustomBaseConfig != null)
                    {
                        PlanCustomMonsterHpBar(actor, ops, ref nX, nY, hpBarOffsetX, hpBarOffsetY, rcLeft, rcRight, rcBottom);
                    }
                    else
                    {
                        ops.Add(new HpBarDrawOp(nX + hpBarOffsetX, nY + hpBarOffsetY,
                            HpBarTextureIndex.Background, 0, 0, 0, "MonBg"));

                        bool highlight = actor.m_btRace is ActorLabelConsts.RC_PLAYOBJECT or ActorLabelConsts.RC_HEROOBJECT
                            && boShowHighlightHPLabel && ckShowHighlightHPLabel
                            && (ReferenceEquals(actor, MySelf) || ReferenceEquals(actor, MyHero));

                        if (highlight)
                        {
                            ops.Add(new HpBarDrawOp(nX + hpBarOffsetX, nY + hpBarOffsetY,
                                HpBarTextureIndex.HighlightHP, rcLeft, rcRight, rcBottom, "HighlightHP"));
                        }
                        else if (actor.m_btRace is ActorLabelConsts.RC_PLAYOBJECT or ActorLabelConsts.RC_HEROOBJECT)
                        {
                            ops.Add(new HpBarDrawOp(nX + hpBarOffsetX, nY + hpBarOffsetY,
                                HpBarTextureIndex.HumHP, rcLeft, rcRight, rcBottom, "HumHP"));
                        }
                        else
                        {
                            ops.Add(new HpBarDrawOp(nX + hpBarOffsetX, nY + hpBarOffsetY,
                                HpBarTextureIndex.MonsterHP, rcLeft, rcRight, rcBottom, "MonHP"));
                        }
                    }
                }
            }
        }

        return ops;
    }

    /// <summary>3693：商人血条排除的外观区间。</summary>
    public static bool IsNpcAppearanceExcluded(int appearance)
        => appearance is (>= 54 and <= 58) or (>= 60 and <= 68) or (>= 90 and <= 92) or (>= 94 and <= 98);

    /// <summary>3747-3780：商人（RC_MERCHANT）血条。</summary>
    private void PlanMerchantHpBar(TActor actor, List<HpBarDrawOp> ops, int nX, int nY,
        int hpBarOffsetX, int hpBarOffsetY, int rcLeft, int rcRight, int rcBottom)
    {
        HpBarNpcConfig? npcConfig = null;
        if (actor.m_wAppearance >= 10000)
            npcConfig = GetCustomNpcConfigFn(actor.m_wAppearance);

        var bc = npcConfig?.BaseConfig;

        if (npcConfig == null || bc!.HPFile < 0 || bc.HPFile >= EffectImageListCountFn() || bc.HPStartIndex < 0)
        {
            ops.Add(new HpBarDrawOp(nX + hpBarOffsetX, nY + hpBarOffsetY,
                HpBarTextureIndex.Background, 0, 0, 0, "NpcBg"));
            ops.Add(new HpBarDrawOp(nX + hpBarOffsetX, nY + hpBarOffsetY,
                HpBarTextureIndex.NpcHP, rcLeft, rcRight, rcBottom, "NpcHP"));
            return;
        }

        var bg = HpBarImages?.GetEffectImage(bc.HPFile, bc.HPStartIndex);
        var dhp = HpBarImages?.GetEffectImage(bc.HPFile, bc.HPStartIndex + 1);

        if (bg == null)
        {
            // GameImage = nil → 默认怪物绘制（用 BaseConfig 偏移，无全局 HpBarOffset）
            ops.Add(new HpBarDrawOp(nX + bc.HPOffsetX, nY + bc.HPOffsetY,
                HpBarTextureIndex.Background, 0, 0, 0, "NpcDefBg"));
            ops.Add(new HpBarDrawOp(nX + bc.HPOffsetX, nY + bc.HPOffsetY,
                HpBarTextureIndex.NpcHP, rcLeft, rcRight, rcBottom, "NpcDefHP"));
            return;
        }

        // 3769：nX 被重算为以背景宽居中
        int cx = actor.m_nSayX - bg.Width / 2;
        ops.Add(new HpBarDrawOp(cx + bc.HPBgOffsetX, nY + bc.HPBgOffsetY, -1, 0, 0, 0, "NpcCustomBg"));

        if (dhp != null)
        {
            int l = 0, r = dhp.Width, b = dhp.Height;
            if (actor.m_Abil.MaxHP > 0)
                r = l + (int)Math.Round((double)(r - l) / actor.m_Abil.MaxHP * actor.m_Abil.HP);
            ops.Add(new HpBarDrawOp(cx + bc.HPOffsetX, nY + bc.HPOffsetY, -2, l, r, b, "NpcCustomHP"));
        }
    }

    /// <summary>3784-3818：自定义怪物血条。</summary>
    private void PlanCustomMonsterHpBar(TActor actor, List<HpBarDrawOp> ops, ref int nX, int nY,
        int hpBarOffsetX, int hpBarOffsetY, int rcLeft, int rcRight, int rcBottom)
    {
        var bc = actor.CustomBaseConfig!;

        if (bc.HPStartIndex < 0)
        {
            ops.Add(new HpBarDrawOp(nX + bc.HPOffsetX + hpBarOffsetX, nY + bc.HPOffsetY + hpBarOffsetY,
                HpBarTextureIndex.Background, 0, 0, 0, "CustomMonBg"));
            ops.Add(new HpBarDrawOp(nX + bc.HPOffsetX + hpBarOffsetX, nY + bc.HPOffsetY + hpBarOffsetY,
                HpBarTextureIndex.MonsterHP, rcLeft, rcRight, rcBottom, "CustomMonHP"));
            return;
        }

        // 3792-3795：HPFile 有效取特效图，否则回落 g_WMonImages
        LabelSurface? bg, dhp;
        if (bc.HPFile >= 0 && bc.HPFile < EffectImageListCountFn())
        {
            bg = HpBarImages?.GetEffectImage(bc.HPFile, bc.HPStartIndex);
            dhp = HpBarImages?.GetEffectImage(bc.HPFile, bc.HPStartIndex + 1);
        }
        else
        {
            bg = HpBarImages?.GetMonImagePair(actor.m_wAppearance, 0);
            dhp = HpBarImages?.GetMonImagePair(actor.m_wAppearance, 1);
        }

        if (bg == null)
        {
            ops.Add(new HpBarDrawOp(nX + bc.HPOffsetX + hpBarOffsetX, nY + bc.HPOffsetY + hpBarOffsetY,
                HpBarTextureIndex.Background, 0, 0, 0, "CustomDefBg"));
            ops.Add(new HpBarDrawOp(nX + bc.HPOffsetX + hpBarOffsetX, nY + bc.HPOffsetY + hpBarOffsetY,
                HpBarTextureIndex.MonsterHP, rcLeft, rcRight, rcBottom, "CustomDefHP"));
            return;
        }

        int cx = actor.m_nSayX - bg.Width / 2;
        ops.Add(new HpBarDrawOp(cx + bc.HPBgOffsetX, nY + bc.HPBgOffsetY, -1, 0, 0, 0, "CustomMonCustomBg"));

        if (dhp != null)
        {
            int l = 0, r = dhp.Width, b = dhp.Height;
            if (actor.m_Abil.MaxHP > 0)
                r = l + (int)Math.Round((double)(r - l) / actor.m_Abil.MaxHP * actor.m_Abil.HP);
            ops.Add(new HpBarDrawOp(cx + bc.HPOffsetX, nY + bc.HPOffsetY, -2, l, r, b, "CustomMonCustomHP"));
        }
    }

    /// <summary>3839-3867：内功黄条（仅人物/英雄，且训练内功中）。</summary>
    public List<HpBarDrawOp> PlanNgLabel(TActor actor, int hpBarOffsetX, int hpBarOffsetY)
    {
        var ops = new List<HpBarDrawOp>();

        if (!(actor.m_btRace is ActorLabelConsts.RC_PLAYOBJECT or ActorLabelConsts.RC_HEROOBJECT))
            return ops;
        if (!actor.m_boTrainingNG || actor.m_AbilNG.MaxNH <= 0)
            return ops;
        if (!boShowNGLabel || !ckShowNGLabel)
            return ops;
        if (actor.m_boShopStall)
            return ops;

        var d = NewopUI170TextureArray[HpBarTextureIndex.Background];
        int rcRight, rcBottom;
        if (d != null) { rcRight = d.Width; rcBottom = d.Height; }
        else { rcRight = 32; rcBottom = 3; }

        int hpImgWidth = rcRight;
        int hpImgHeigh = rcBottom;
        int hpOffsetX = hpImgWidth / 2;
        int hpOffsetY = hpImgHeigh * 2 + 4;

        int x = actor.m_nSayX - hpOffsetX + hpBarOffsetX;
        int y = actor.m_nSayY - hpOffsetY + hpImgHeigh + hpBarOffsetY;

        int rc2Right = 0;
        rc2Right = rc2Right + (int)Math.Round((double)hpImgWidth * actor.m_AbilNG.NH / actor.m_AbilNG.MaxNH);

        ops.Add(new HpBarDrawOp(x, y, HpBarTextureIndex.Background, 0, 0, 0, "NgBg"));
        ops.Add(new HpBarDrawOp(x, y, HpBarTextureIndex.NgYellow, 0, rc2Right, hpImgHeigh, "NgBar"));
        return ops;
    }
}
