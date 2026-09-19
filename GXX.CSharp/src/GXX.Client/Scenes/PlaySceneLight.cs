using System;
using System.Collections.Generic;

namespace GXX.Client.Scenes;

/// <summary>一次光照贴图绘制的 headless 产物（Blend_SrcColorAdd 加色混合）。</summary>
public sealed record LightDrawOp(int X, int Y, int ImageIndex, int TexWidth, int TexHeight, string Kind);

/// <summary>引导框填充（GameCanvas.FillRect(clWhite, Blend_SrcColorAdd) 的 headless 产物）。</summary>
public sealed record GuideFillOp(int X, int Y, int Width, int Height);

/// <summary>TClEvent 的光照切片（m_nLight / m_nX / m_nY）。</summary>
public sealed class DrawEventLightEntry
{
    public int m_nLight;
    public int m_nX;
    public int m_nY;
}

/// <summary>
/// PlayScn.pas 光照与引导渲染（批次J81，无纹理 headless 化）——
/// RenderLight(1338-1454) 与 RenderGuide(1456-1464)。
/// 光照四源：地图格 btLight、m_DrawActorList 的 actor.Light、m_DrawEffectList 的 meff.Light、
/// m_DrawEventList 的 evn.m_nLight；统一取 g_WNewopUIImages.Images[210 + nLight]，
/// 并以 (ix - w/2, iy - h/2) 居中绘制。
/// 常量：LOGICALMAPUNIT=40（MShare 111）、LONGHEIGHT_IMAGE=32（PlayScn 40）。
/// </summary>
public sealed class PlaySceneLightRender
{
    public const int LONGHEIGHT_IMAGE = 32;
    public const int LOGICALMAPUNIT = 40;

    /// <summary>光照贴图基址（g_WNewopUIImages.Images[210 + nLight]）。</summary>
    public const int LightImageBase = 210;

    /// <summary>光照等级上限（_MIN(nLight, 5)）。</summary>
    public const int LightLevelMax = 5;

    /// <summary>SCREENWIDTH / SCREENHEIGHT（光照铺满整屏的填充范围）。</summary>
    public const int SCREENWIDTH = 1024;
    public const int SCREENHEIGHT = 768;

    /// <summary>g_nDarkValue（FillRect 的 RGB 通道灰度值；原文 ARGB(255, v, v, v)）。</summary>
    public int DarkValue;

    /// <summary>g_MySelf 是否为 nil（1348：为 nil 则整个 RenderLight 直接 Exit）。</summary>
    public bool HasMySelf;

    /// <summary>g_MySelf 的光照与坐标切片（1392-1405 的回落分支用）。</summary>
    public LightActorSlice? MySelf;

    /// <summary>Map.m_boLoadOk（1354）。</summary>
    public bool MapLoadOk = true;

    /// <summary>Map.m_ClientRect / m_nBlockLeft / m_nBlockTop 切片。</summary>
    public int ClientLeft, ClientTop, ClientRight, ClientBottom;
    public int BlockLeft, BlockTop;

    /// <summary>m_DrawActorList（1375-1391）。</summary>
    public readonly List<LightActorSlice> DrawActorList = new();

    /// <summary>m_DrawEffectList（1407-1435；TMagicEff 切片）。</summary>
    public readonly List<LightEffectSlice> DrawEffectList = new();

    /// <summary>m_DrawEventList（1437-1449）。</summary>
    public readonly List<DrawEventLightEntry> DrawEventList = new();

    /// <summary>地图单元格 btLight 取值接缝（m_MArr[I, J].btLight）。</summary>
    public Func<int, int, int> MapCellLight = (_, _) => 0;

    /// <summary>ScreenXYfromMCXY(5942-5968)：地图格 → 屏幕像素。</summary>
    public Func<int, int, (int Sx, int Sy)> ScreenXYfromMCXY =
        (x, y) => MagicEffEnv.ScreenXYfromMCXY(x, y);

    /// <summary>光照贴图探测接缝（g_WNewopUIImages.Images[210 + nLight]；null = 未就绪）。</summary>
    public Func<int, (int Width, int Height)?>? TextureProbe;

    /// <summary>g_GuideX / g_GuideY / g_GuideR / g_GuideB（R/B 为右/下边界）。</summary>
    public int GuideX, GuideY, GuideR, GuideB;

    /// <summary>Actor 的光照切片（actor.Light / m_nRx / m_nRy / m_nShiftX / m_nShiftY）。</summary>
    public sealed class LightActorSlice
    {
        public int Light;
        public int m_nRx;
        public int m_nRy;
        public int m_nShiftX;
        public int m_nShiftY;
    }

    /// <summary>TMagicEff 的光照切片（light / rx / ry / targetx / targety / TargetActor）。</summary>
    public sealed class LightEffectSlice
    {
        public int light;
        public int rx;
        public int ry;
        public int targetx;
        public int targety;

        /// <summary>TargetActor（TActor 时叠加其 shift；否则不叠加）。</summary>
        public LightActorSlice? TargetActor;
    }

    private LightDrawOp? Probe(int nLight, int ix, int iy, string kind)
    {
        var d = TextureProbe?.Invoke(LightImageBase + nLight);
        if (d == null)
            return null;
        return new LightDrawOp(ix - d.Value.Width / 2, iy - d.Value.Height / 2,
            LightImageBase + nLight, d.Value.Width, d.Value.Height, kind);
    }

    /// <summary>
    /// RenderLight 1:1（1338-1454）：g_MySelf 为 nil 直接 Exit；先以
    /// ARGB(255, g_nDarkValue³) 铺满整屏，然后按四源依次绘制加色混合光照。
    /// 原文整段被 try/except 包裹（except 仅 DebugOutStr('107')）。
    /// 注意 1380 与 1412/1441 的下界差异：actor 为 _MAX(nLight, 0)，effect/event 为 _MAX(nLight, 1)。
    /// </summary>
    public List<LightDrawOp> RenderLight()
    {
        var ops = new List<LightDrawOp>();

        if (!HasMySelf)
            return ops;

        // 1349：FillRect(Bounds(0,0,SCREENWIDTH,SCREENHEIGHT), ARGB(255, DarkValue³))
        // 这是画面暗色底，不是光照贴图 → 不产出 LightDrawOp。

        // ---- 源一：地图格 btLight（1354-1373） ----
        if (MapLoadOk)
        {
            int jTop = ClientTop - BlockTop - 4;
            int jBottom = ClientBottom - BlockTop + LONGHEIGHT_IMAGE;
            int iLeft = ClientLeft - BlockLeft - 5;
            int iRight = ClientRight - BlockLeft + 5;

            for (int j = jTop; j <= jBottom; j++)
            {
                if (j < 0)
                    continue;                          // 1356-1358：仅 continue，不 break

                for (int i = iLeft; i <= iRight; i++)
                {
                    if (i >= 0 && i < LOGICALMAPUNIT * 3 && j >= 0 && j < LOGICALMAPUNIT * 3)
                    {
                        int nLight = MapCellLight(i, j);
                        if (nLight > 0)
                        {
                            nLight = Math.Min(nLight, LightLevelMax);   // 无下界夹紧
                            var (ix, iy) = ScreenXYfromMCXY(i + BlockLeft, j + BlockTop);
                            var op = Probe(nLight, ix, iy, "MapCell");
                            if (op != null)
                                ops.Add(op);
                        }
                    }
                }
            }
        }

        // ---- 源二：m_DrawActorList 的 actor.Light（1375-1405） ----
        if (DrawActorList.Count > 0)
        {
            foreach (var actor in DrawActorList)
            {
                // 1378：自己恒绘制（即使 Light = 0），其余仅 Light > 0
                if (ReferenceEquals(actor, MySelf) || actor.Light > 0)
                {
                    int nLight = actor.Light;
                    nLight = Math.Max(nLight, 0);
                    nLight = Math.Min(nLight, LightLevelMax);
                    var (ix0, iy0) = ScreenXYfromMCXY(actor.m_nRx, actor.m_nRy);
                    var op = Probe(nLight, ix0 + actor.m_nShiftX, iy0 + actor.m_nShiftY, "Actor");
                    if (op != null)
                        ops.Add(op);
                }
            }
        }
        else if (MySelf != null)
        {
            // 1392-1405：列表为空时回落到 g_MySelf 自身（此处自身必绘，无 Light > 0 判断）
            int nLight = MySelf.Light;
            nLight = Math.Max(nLight, 0);
            nLight = Math.Min(nLight, LightLevelMax);
            var (ix0, iy0) = ScreenXYfromMCXY(MySelf.m_nRx, MySelf.m_nRy);
            var op = Probe(nLight, ix0 + MySelf.m_nShiftX, iy0 + MySelf.m_nShiftY, "SelfFallback");
            if (op != null)
                ops.Add(op);
        }

        // ---- 源三：m_DrawEffectList 的 meff.Light（1407-1435） ----
        foreach (var meff in DrawEffectList)
        {
            if (meff.light <= 0)
                continue;

            int nLight = Math.Min(Math.Max(meff.light, 1), LightLevelMax);

            int ix, iy;
            // 1419-1424：Rx/Ry 任一为正则用自身坐标，否则回落到 targetx/targety
            if (meff.rx > 0 || meff.ry > 0)
                (ix, iy) = ScreenXYfromMCXY(meff.rx, meff.ry);
            else
                (ix, iy) = ScreenXYfromMCXY(meff.targetx, meff.targety);

            // 1427-1430：TargetActor 为 TActor 时叠加其 shift
            if (meff.TargetActor != null)
            {
                ix += meff.TargetActor.m_nShiftX;
                iy += meff.TargetActor.m_nShiftY;
            }

            var op = Probe(nLight, ix, iy, "Effect");
            if (op != null)
                ops.Add(op);
        }

        // ---- 源四：m_DrawEventList 的 evn.m_nLight（1437-1449） ----
        foreach (var evn in DrawEventList)
        {
            if (evn.m_nLight <= 0)
                continue;

            int nLight = Math.Min(Math.Max(evn.m_nLight, 1), LightLevelMax);
            var (ix, iy) = ScreenXYfromMCXY(evn.m_nX, evn.m_nY);
            var op = Probe(nLight, ix, iy, "Event");
            if (op != null)
                ops.Add(op);
        }

        return ops;
    }

    /// <summary>
    /// RenderGuide 1:1（1456-1464）：g_MySelf 为 nil 直接 Exit；
    /// 否则以 clWhite + Blend_SrcColorAdd 填充
    /// Bounds(g_GuideX, g_GuideY, g_GuideR - g_GuideX, g_GuideB - g_GuideY)。
    /// 注：原文 1462 的灰色铺屏调用是**注释掉的**，故不执行。
    /// </summary>
    public List<GuideFillOp> RenderGuide()
    {
        var ops = new List<GuideFillOp>();

        if (!HasMySelf)
            return ops;

        ops.Add(new GuideFillOp(GuideX, GuideY, GuideR - GuideX, GuideB - GuideY));
        return ops;
    }
}
