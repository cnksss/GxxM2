using System;

namespace GXX.Client.Scenes;

/// <summary>Delphi TColor 常量（BGR 布局，SDK/GDI 标准色）。</summary>
public static class DlColors
{
    public const int Black = 0x000000;
    public const int White = 0xFFFFFF;
    public const int Red = 0x0000FF;
    public const int Lime = 0x00FF00;
    public const int Green = 0x008000;
    public const int Blue = 0xFF0000;
    public const int Yellow = 0x00FFFF;
    public const int Fuchsia = 0xFF00FF;
    public const int Aqua = 0xFFFF00;
    public const int Silver = 0xC0C0C0;
    public const int Gray = 0x808080;
}

/// <summary>Actor.pas 绘制调度接缝：调色板查表 / 运行时音效 id / 时钟。</summary>
public static class ActorDrawEnv
{
    /// <summary>GetRGB(c256)（MShare 4111：g_DefColorTable[c256] → TColor）headless 接缝。</summary>
    public static Func<int, int> GetRgbFn = c => c;
    /// <summary>SoundUtil 运行时命名音效常量（s_cboFs1_start 等 = g_SoundList.Count + N，缺省 -1）。</summary>
    public static Func<string, int> NamedSoundFn = _ => -1;
    /// <summary>TimeGetTime。</summary>
    public static Func<uint> NowFn = () => 0;

    public static void Reset()
    {
        GetRgbFn = c => c;
        NamedSoundFn = _ => -1;
        NowFn = () => 0;
    }
}

public enum SurfaceDrawKind
{
    Draw,
    DrawColor,
    DrawColorAlpha,
    DrawBlend,
    StretchDraw,
}

/// <summary>一次表面绘制调用（GameCanvas.Draw* 的 headless 镜像）。</summary>
public sealed record SurfaceDrawOp(
    SurfaceDrawKind Kind,
    int X,
    int Y,
    int Color = 0,
    int Alpha = 0,
    int DestWidth = 0,
    int DestHeight = 0,
    string Lib = "",
    int ImageIndex = 0,
    bool Gray = false);

/// <summary>状态特效动画游标（TActor 内嵌状态字段的 headless 子集）。</summary>
public sealed class StateFxState
{
    public uint CobwebTick, ToxicTick, FrozenTick;
    public int CobwebIndex, ToxicIndex, FrozenIndex;
}

/// <summary>
/// Actor.pas 绘制调度族 1:1（批次J53）：
/// DrawEffSurface（5704-5795：state $00800000 强制混合、m_btBodyColor → GetRGB 调色板、
/// 13 色 ceff 分流——注意 ceGreen 非混合走 clLime、混合走 clGreen 的原版差异、混合恒 alpha 150）、
/// StretchDrawEffSurface（5797-5890：1.5× Round 银行家舍入、nX=12/nY=UNITY=32 原点、幻影 alpha）、
/// DrawStateEffSurface（5654-5702：蛛网 100ms×10 帧@NewopUI 320、毒烟 80ms×10 帧@cboEffect 4010、
/// 永恒冰冻 80ms×4 帧@NewopUI 330，Ghost/Death 门控与 SayX 居中定位）、
/// DrawWeaponGlimmer（5892-5910：ckHideWeaponEffect+PlugInEnabled 门、NoBlend 分流）、
/// DrawChr 施法特效层（6101-6118：m_boUseMagic 门、GetEffectBase 变参、死亡灰度、DrawBlend）、
/// SetMagicSound（6167-6350：10000+id×10+{0,1,2} 公式与 40 余条技能音效修正表）。
/// </summary>
public static class ActorDrawDispatch
{
    public const int StateBlendMask = 0x00800000;
    public const int Unity = 32;
    public const int HalftoneAlpha = 150;

    /// <summary>DrawEffSurface（5704）1:1：返回 null = Source 未命中不绘。</summary>
    public static SurfaceDrawOp DrawEffSurface(int state, int bodyColor, TColorEffect ceff, bool blend, int x, int y)
    {
        if ((state & StateBlendMask) != 0)
            blend = true;

        if (bodyColor != 0)
        {
            int rgb = ActorDrawEnv.GetRgbFn(bodyColor);
            return blend
                ? new SurfaceDrawOp(SurfaceDrawKind.DrawColorAlpha, x, y, rgb, HalftoneAlpha)
                : new SurfaceDrawOp(SurfaceDrawKind.DrawColor, x, y, rgb);
        }

        if (blend)
        {
            int color = ceff switch
            {
                TColorEffect.ceBlack => DlColors.Black,
                TColorEffect.ceWhite => DlColors.White,
                TColorEffect.ceRed => DlColors.Red,
                TColorEffect.ceGreen => DlColors.Green, // 混合分支用 clGreen
                TColorEffect.ceBlue => DlColors.Blue,
                TColorEffect.ceYellow => DlColors.Yellow,
                TColorEffect.ceFuchsia => DlColors.Fuchsia,
                TColorEffect.ceAqua => DlColors.Aqua,
                TColorEffect.ceSilver => DlColors.Silver,
                TColorEffect.ceGray => DlColors.Gray,
                _ => DlColors.White, // ceNone / ceGrayScale / ceGrayScale2 / ceBright
            };
            return new SurfaceDrawOp(SurfaceDrawKind.DrawColorAlpha, x, y, color, HalftoneAlpha);
        }

        return ceff switch
        {
            TColorEffect.ceBlack => new SurfaceDrawOp(SurfaceDrawKind.DrawColor, x, y, DlColors.Black),
            TColorEffect.ceWhite => new SurfaceDrawOp(SurfaceDrawKind.DrawColor, x, y, DlColors.White),
            TColorEffect.ceRed => new SurfaceDrawOp(SurfaceDrawKind.DrawColor, x, y, DlColors.Red),
            TColorEffect.ceGreen => new SurfaceDrawOp(SurfaceDrawKind.DrawColor, x, y, DlColors.Lime), // clGreen→clLime piaoyun 2013-06-28
            TColorEffect.ceBlue => new SurfaceDrawOp(SurfaceDrawKind.DrawColor, x, y, DlColors.Blue),
            TColorEffect.ceYellow => new SurfaceDrawOp(SurfaceDrawKind.DrawColor, x, y, DlColors.Yellow),
            TColorEffect.ceFuchsia => new SurfaceDrawOp(SurfaceDrawKind.DrawColor, x, y, DlColors.Fuchsia),
            TColorEffect.ceAqua => new SurfaceDrawOp(SurfaceDrawKind.DrawColor, x, y, DlColors.Aqua),
            TColorEffect.ceSilver => new SurfaceDrawOp(SurfaceDrawKind.DrawColor, x, y, DlColors.Silver),
            TColorEffect.ceGray => new SurfaceDrawOp(SurfaceDrawKind.DrawColor, x, y, DlColors.Gray),
            _ => new SurfaceDrawOp(SurfaceDrawKind.Draw, x, y), // ceNone / 灰度 / 高亮
        };
    }

    /// <summary>Delphi Round：银行家舍入（Round(64.5)=64）。</summary>
    private static int DelphiRound(double v) => (int)Math.Round(v, MidpointRounding.ToEven);

    /// <summary>StretchDrawEffSurface（5797）1:1：DestRect=(ddx−12, ddy−32, w×1.5, h×1.5)，幻影 alpha + ceff 色表。</summary>
    public static SurfaceDrawOp StretchDrawEffSurface(int bodyColor, TColorEffect ceff, int x, int y,
        int srcWidth, int srcHeight, int phantomAlpha)
    {
        int width = DelphiRound(srcWidth * 1.5);
        int height = DelphiRound(srcHeight * 1.5);
        int dx = x - 12;
        int dy = y - Unity;

        if (bodyColor != 0)
        {
            return new SurfaceDrawOp(SurfaceDrawKind.StretchDraw, dx, dy,
                ActorDrawEnv.GetRgbFn(bodyColor), phantomAlpha, width, height);
        }

        int color = ceff switch
        {
            TColorEffect.ceBlack => DlColors.Black,
            TColorEffect.ceWhite => DlColors.White,
            TColorEffect.ceRed => DlColors.Red,
            TColorEffect.ceGreen => DlColors.Green,
            TColorEffect.ceBlue => DlColors.Blue,
            TColorEffect.ceYellow => DlColors.Yellow,
            TColorEffect.ceFuchsia => DlColors.Fuchsia,
            TColorEffect.ceAqua => DlColors.Aqua,
            TColorEffect.ceSilver => DlColors.Silver,
            TColorEffect.ceGray => DlColors.Gray,
            _ => 0, // ceNone / ceGrayScale / ceGrayScale2 / ceBright 无色参
        };
        return new SurfaceDrawOp(SurfaceDrawKind.StretchDraw, dx, dy, color, phantomAlpha, width, height);
    }

    /// <summary>状态特效图库标识。</summary>
    public const string NewopUiLib = "WNewopUIImages";
    public const string CboEffectLib = "cboEffect";

    /// <summary>取图镜像：返回 null = d=nil 不绘。</summary>
    public static FxImage? GetCachedImage(string lib, int imageIndex, bool gray)
        => new(imageIndex, 0, 0, 0, 0, gray);

    /// <summary>
    /// DrawStateEffSurface（5654）1:1：三层状态各自推进/回卷/门控后返回绘制操作。
    /// 蛛网 y=ddy+pY−20、毒烟/冰冻 y=ddy+pY；x=m_nSayX−宽 div 2。
    /// </summary>
    public static (SurfaceDrawOp? Cobweb, SurfaceDrawOp? Smoke, SurfaceDrawOp? Frozen) DrawStateEffSurface(
        StateFxState st, bool cobweb, bool duanJin, bool toxicSmoke, bool foreverFrozen,
        bool ghost, bool death, uint now, int sayX, int ddy,
        Func<string, int, (int Width, int OriginY)?> resolve)
    {
        SurfaceDrawOp? cobwebOp = null, smokeOp = null, frozenOp = null;

        if ((cobweb && !duanJin) && !ghost && !death) // 蜘蛛网罩住
        {
            if (now - st.CobwebTick > 100)
            {
                st.CobwebTick = now;
                st.CobwebIndex++;
            }
            if (st.CobwebIndex < 0 || st.CobwebIndex > 9)
                st.CobwebIndex = 0;

            int idx = 320 + st.CobwebIndex;
            var d = resolve(NewopUiLib, idx);
            if (d != null)
                cobwebOp = new SurfaceDrawOp(SurfaceDrawKind.DrawBlend, sayX - d.Value.Width / 2, ddy + d.Value.OriginY - 20,
                    Lib: NewopUiLib, ImageIndex: idx);
        }

        if (toxicSmoke && !ghost && !death) // 毒烟 chongchong 2013-11-10
        {
            if (now - st.ToxicTick > 80)
            {
                st.ToxicTick = now;
                st.ToxicIndex++;
            }
            if (st.ToxicIndex < 0 || st.ToxicIndex > 9)
                st.ToxicIndex = 0;

            int idx = 4010 + st.ToxicIndex;
            var d = resolve(CboEffectLib, idx);
            if (d != null)
                smokeOp = new SurfaceDrawOp(SurfaceDrawKind.DrawBlend, sayX - d.Value.Width / 2, ddy + d.Value.OriginY,
                    Lib: CboEffectLib, ImageIndex: idx);
        }

        if (foreverFrozen && !ghost && !death) // 永恒冰冻 piaoyun 2013-12-05
        {
            if (now - st.FrozenTick > 80)
            {
                st.FrozenTick = now;
                st.FrozenIndex++;
            }
            if (st.FrozenIndex < 0 || st.FrozenIndex > 3)
                st.FrozenIndex = 0;

            int idx = 330 + st.FrozenIndex;
            var d = resolve(NewopUiLib, idx);
            if (d != null)
                frozenOp = new SurfaceDrawOp(SurfaceDrawKind.DrawBlend, sayX - d.Value.Width / 2, ddy + d.Value.OriginY,
                    Lib: NewopUiLib, ImageIndex: idx);
        }

        return (cobwebOp, smokeOp, frozenOp);
    }

    /// <summary>DrawWeaponGlimmer（5892）1:1：插件隐藏门 + NoBlend 分流。</summary>
    public static SurfaceDrawOp? WeaponGlimmer(bool hideChecked, bool pluginEnabled, bool noBlend,
        int weaponEffectX, int weaponEffectY, int shiftX, int shiftY, int x, int y, string lib, int imageIndex)
    {
        if (hideChecked && pluginEnabled)
            return null;
        var kind = noBlend ? SurfaceDrawKind.Draw : SurfaceDrawKind.DrawBlend;
        return new SurfaceDrawOp(kind, x + weaponEffectX + shiftX, y + weaponEffectY + shiftY, Lib: lib, ImageIndex: imageIndex);
    }

    /// <summary>GetEffectBase 变参结果的 headless 镜像（wimg=nil → null）。</summary>
    public sealed record EffectBaseRef(string Lib, int BaseIndex, int OriginX, int OriginY);

    /// <summary>DrawChr 施法特效层（6101）1:1。</summary>
    public static SurfaceDrawOp? SpellEffect(bool useMagic, int effectNumber, int curEffFrame, int spellFrame, int newLevel,
        bool selfDead, int dx, int dy, int shiftX, int shiftY,
        Func<int, int, int, EffectBaseRef?> getEffectBase)
    {
        if (!useMagic || effectNumber <= 0)
            return null;
        if (curEffFrame < 0 || curEffFrame > spellFrame - 1) // m_nCurEffFrame in [0..m_nSpellFrame-1]
            return null;
        var wimg = getEffectBase(effectNumber - 1, 0, newLevel);
        if (wimg == null)
            return null;
        int idx = wimg.BaseIndex + curEffFrame;
        return new SurfaceDrawOp(SurfaceDrawKind.DrawBlend, dx + wimg.OriginX + shiftX, dy + wimg.OriginY + shiftY,
            Lib: wimg.Lib, ImageIndex: idx, Gray: selfDead);
    }

    /// <summary>技能音效三元组（m_nMagicStartSound/FireSound/ExplosionSound）。</summary>
    public sealed record MagicSoundIds(int Start, int Fire, int Explosion);

    /// <summary>SetMagicSound（6167-6350）1:1：公式 + 修正表；wMagicID≤0 → null（不设值）。</summary>
    public static MagicSoundIds? SetMagicSound(int wMagicId)
    {
        if (wMagicId <= 0)
            return null;

        int start = 10000 + wMagicId * 10;
        int fire = 10000 + wMagicId * 10 + 1;
        int explosion = 10000 + wMagicId * 10 + 2;

        switch (wMagicId)
        {
            case 11: // 雷电术去掉起手声音 chongchong 2015-11-25
                start = -1;
                break;
            case 62:
                start = 10520; fire = 10521; explosion = 10522;
                break;
            case 63:
                start = 10530; fire = 10531; explosion = 10532;
                break;
            case 64:
                start = 10540; fire = 10541; explosion = 10542;
                break;
            case 65:
                start = 10550; fire = 10551; explosion = 10552;
                break;
            case 58: // 流星火雨
                start = Named("s_hit_Lxhy_0");
                fire = Named("s_hit_Lxhy_0");
                explosion = Named("s_hit_Lxhy_3");
                break;
            case 57: // 噬血术
                start = 10000 + 10 * 48; fire = 10000 + 10 * 48 + 1; explosion = 10000 + 10 * 48 + 2;
                break;
            case 48: // 气功波37
                start = 10000 + 10 * 37; fire = 10000 + 10 * 37 + 1; explosion = 10000 + 10 * 37 + 2;
                break;
            case 50: // 无极真气36
                start = 10000 + 10 * 36; fire = 10000 + 10 * 36 + 1; explosion = 10000 + 10 * 36 + 2;
                break;
            case 51: // 群体施毒术
                start = 10060; fire = 10061; explosion = 10062;
                break;
            case 52: // 飓风破
                start = 10000 + 10 * 47; fire = 10000 + 10 * 47 + 1; explosion = 10000 + 10 * 47 + 2;
                break;
            case 38 or 46: // 诅咒术{38}声音修复 piaoyun 2013-08-24（fire 不覆盖）
                start = 10520;
                explosion = 10522;
                break;
            case 71: // 擒龙手
                start = 10000 + 10 * 28; fire = 10000 + 10 * 28 + 1; explosion = 10000 + 10 * 28 + 2;
                break;
            case 72: // 乾坤大挪移
                start = 10000 + 10 * 21; fire = 10000 + 10 * 21 + 1; explosion = 10000 + 10 * 21 + 2;
                break;
            case 73 or 87 or 88 or 89: // 道/武力盾声音 piaoyun 2013-08-27
                start = 10000 + 10 * 31; fire = 10000 + 10 * 31 + 1; explosion = 10000 + 10 * 31 + 2;
                break;
            case 76: // 召唤圣兽
                start = 10000 + 10 * 30; fire = 10000 + 10 * 30 + 1; explosion = 10000 + 10 * 30 + 2;
                break;
            case 107: // 双龙破
                start = Named("s_cboFs1_start");
                explosion = Named("s_cboFs1_target");
                break;
            case 104: // 凤舞祭
                start = Named("s_cboFs2_start");
                explosion = Named("s_cboFs2_target");
                break;
            case 105: // 惊雷爆
                start = Named("s_cboFs3_start");
                explosion = Named("s_cboFs3_target");
                break;
            case 106: // 冰天雪地
                start = Named("s_cboFs4_start");
                explosion = Named("s_cboFs4_target");
                break;
            case 108: // 虎啸诀
                start = Named("s_cboDs1_start");
                explosion = Named("s_cboDs1_target");
                break;
            case 109: // 八卦掌
                start = Named("s_cboDs2_start");
                explosion = Named("s_cboDs2_target");
                break;
            case 110: // 三焰咒
                start = Named("s_cboDs3_start");
                explosion = Named("s_cboDs3_target");
                break;
            case 111: // 万剑归宗
                start = Named("s_cboDs4_start");
                explosion = Named("s_cboDs4_target");
                break;
            case 114: // 倚天辟地
                start = Named("s_xsls_death");
                explosion = Named("s_xsws_pbec");
                break;
            case 116: // 血魄一击(法)
                start = 11036;
                explosion = 11037;
                break;
            case 117: // 血魄一击(道)
                start = 11040;
                explosion = 11041;
                break;
            case 199:
                start = 11000; fire = 0; explosion = 11002;
                break;
            case 200:
                start = 11010; fire = 0; explosion = 11012;
                break;
            case 201: // 新技能声音 -- 2013-6-17
                start = 10330; fire = 10331; explosion = 10430;
                break;
            case 202: // 裂神符声音
                start = 10130; fire = 10131; explosion = 10132;
                break;
            case 203: // 死亡之眼声音
                start = 10490; fire = 10491; explosion = 10492;
                break;
            case 204: // 十步一杀声音
                start = 10461; fire = 0; explosion = 10522;
                break;
            case 205: // 冰霜雪雨声音
                start = Named("s_hit_Lxhy_0");
                fire = 0;
                explosion = Named("s_hit_Lxhy_3");
                break;
            case 206: // 冰霜群雨声音
                start = Named("s_xf");
                fire = 0;
                explosion = Named("s_xsws_pbec");
                break;
            case 208: // 旋风斩 chongchong 2014-09-14
                start = 11060; fire = 0; explosion = 0;
                break;
            case 34: // 解毒术声音 chongchong 2013-11-19
                start = 10020; fire = 0; explosion = 10492;
                break;
            case 41: // 狮子吼声音 chongchong 2013-12-10
                start = 10430; fire = 0; explosion = 0;
                break;
        }

        return new MagicSoundIds(start, fire, explosion);
    }

    private static int Named(string id) => ActorDrawEnv.NamedSoundFn(id);
}
