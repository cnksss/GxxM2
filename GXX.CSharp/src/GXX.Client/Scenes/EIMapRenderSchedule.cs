using System;
using System.Collections.Generic;

namespace GXX.Client.Scenes;

/// <summary>MapUnit.pas 121-124 TEIMapTileInfo（packed record）镜像。</summary>
public sealed class EIMapTileInfo
{
    public byte btFileIdx;
    public ushort wTileIdx;
}

/// <summary>MapUnit.pas 127-138 TEIMapInfo（packed record）镜像。</summary>
public sealed class EIMapInfo
{
    public byte btFlag;
    public byte btObj1Ani;
    public byte btObj2Ani;
    public byte btFileIdx1;
    public byte btFileIdx2;
    public ushort wObj1;
    public ushort wObj2;
    public byte btDoorIdx;
    public byte btDoorOffset;
    public ushort wLigntNEvent;
    public byte btLigntNEvent1;
}

/// <summary>EI 地图一次绘制（GameCanvas.Draw 的 headless 产物）。</summary>
public sealed record EIMapDrawOp(
    int X, int Y, int FileIdx, int ImageNumber,
    int SrcLeft, int SrcTop, int SrcRight, int SrcBottom,
    string Kind, bool Gray);

/// <summary>
/// PlayScn.pas DrawTileEIMap（2082-2381）传奇3（EI）地图渲染 1:1 移植（批次J84，无纹理 headless 化）。
/// 三段结构：
/// ① 地图背景格（偶偶格、`btFileIdx &lt; 73`、裁剪矩形与屏幕边界双重过滤）；
/// ② 下层对象（`btFileIdx2` / `wObj1` / `btObj1Ani`）；
/// ③ 上层对象（`btFileIdx1` / `wObj2` / `btObj2Ani`）。
/// ②③ 结构完全相同（仅字段与读取顺序不同）。
/// 常量复用 MShare 107-108 的 UNITX=48/UNITY=32 与 PlayScn 40 的 LONGHEIGHT_IMAGE=32、AAX=16。
/// </summary>
public sealed class EIMapRenderSchedule
{
    public const int UNITX = 48;
    public const int UNITY = 32;
    public const int AAX = 16;
    public const int LONGHEIGHT_IMAGE = 32;
    public const int SCREENWIDTH = 1024;
    public const int MAPSURFACEHEIGHT = 768;
    public const int LOGICALMAPUNIT = 40;

    /// <summary>背景格文件号上限（2141：`nFileIdx < 73`）。</summary>
    public const int BgFileIdxLimit = 73;

    /// <summary>对象文件号上限（2224/2299：`nFileIdx < 75`）。</summary>
    public const int ObjFileIdxLimit = 75;

    /// <summary>2111：MapYExt 恒为 10（原文的 boFullScreenDrawMap 分支已被注释掉）。</summary>
    public const int MapYExt = 10;

    /// <summary>需要逐帧动画的文件号集合（2226/2302）。</summary>
    public static readonly int[] AnimatedFileIdxs = { 11, 26, 41, 56, 71 };

    /// <summary>GetCachedImageSize 返回的尺寸 ≤ 4 像素时视为无效（2157/2171）。</summary>
    public const int MinPixelArea = 4;

    /// <summary>对象只有恰好 48×32 时才绘制（2284/2360）。</summary>
    public const int ObjectWidth = 48;
    public const int ObjectHeight = 32;

    /// <summary>m_MArrEIMapTiteInfo[I div 2, J div 2] 取值接缝。</summary>
    public Func<int, int, EIMapTileInfo> TileCell = (_, _) => new EIMapTileInfo();

    /// <summary>m_MArrEIMapInfo[I, J] 取值接缝；返回 null 表示越界（2218-2219 的 Continue）。</summary>
    public Func<int, int, EIMapInfo?> ObjectCell;

    /// <summary>GetCachedImageSize(imgNumber) → 宽高；null 表示未就绪（走 else 分支直取纹理）。</summary>
    public Func<int, int, (int Width, int Height)?>? ObjectTextureProbe;

    /// <summary>g_EIMapTitleArr[fileIdx].Images / .Grays 的取图接缝（返回 null = 该图为空）。</summary>
    public Func<int, int, bool, EIMapTextureInfo?> Texture;

    /// <summary>取图产物（宽高 + 是否走灰度）。</summary>
    public sealed record EIMapTextureInfo(int Width, int Height);

    public int ClientLeft, ClientTop, ClientRight, ClientBottom;
    public int BlockLeft, BlockTop;
    public int DefXX, DefYY;
    public int ShakeX, ShakeY;
    public int AniCount;
    public bool SelfDead;
    public bool AutoUpdate;
    public bool MapLoadOk = true;
    public bool HasMySelf = true;

    /// <summary>g_boCanDrawTileMap（2172/2196 需要重绘整图时置真）。</summary>
    public bool CanDrawTileMap;

    /// <summary>GetNeedUpdate（2149/2171 的图库更新检测）。</summary>
    public Func<int, bool>? NeedUpdate;

    /// <summary>
    /// 单格动画图像号偏移（2226-2263 / 2302-2339）：
    /// 仅在 fileIdx ∈ {11,26,41,56,71} 且 `ani ∈ (0, 255)` 时生效；
    /// `ani and $80` 非 0 → blend；随后按文件号分派取模步长。
    /// 注意 fileIdx = 11 的 `ani > 10` 与 `ani > 0` 两支**结果相同**（都除以 ani 取模）——
    /// 原文如此，第一支的 `mod 5` 只出现在 2235 的形式下，逐字保留其分支结构。
    /// </summary>
    public static (int ImageNumber, bool Blend) ApplyAnimation(
        int fileIdx, int imageNumber, int ani, int aniCount)
    {
        bool blend = false;

        if (Array.IndexOf(AnimatedFileIdxs, fileIdx) < 0)
            return (imageNumber, false);

        if (ani == 255 || ani <= 0)
            return (imageNumber, false);

        if ((ani & 0x80) > 0)
            blend = true;

        int half = aniCount / 2;

        switch (fileIdx)
        {
            case 11:
                if (ani > 10)
                    imageNumber += half % 5;
                else
                    imageNumber += half % ani;
                break;

            case 26:
                if (ani == 190)
                    imageNumber += half % 14;
                else
                    imageNumber += half % 10;
                break;

            case 41:
                if (ani == 184)
                    imageNumber += half % 16;
                else
                    imageNumber += half % 10;
                break;

            case 56:
                if (ani >= 136)
                    imageNumber += half % 10;
                else if (ani >= 69)
                    imageNumber += half % 5;
                else
                    imageNumber += half % 6;
                break;

            case 71:
                imageNumber += half % 6;
                break;
        }

        return (imageNumber, blend);
    }

    /// <summary>
    /// ① 地图背景格（2101-2206）：行从 `ClientTop - BlockTop - 1` 到 `+ MapYExt`，
    /// 起点 `nY = -UNITY*2` 且每行 `+= UNITY`、起点 `nX = AAX + 14 - UNITX` 且每格 `+= UNITX`；
    /// `nPaintY &gt; MAPSURFACEHEIGHT` 或 `nPaintX &gt; SCREENWIDTH` 时 **Break**（跳出整层）；
    /// 仅 `I mod 2 = 0 and J mod 2 = 0` 且 `nImgNumber ∈ [0, 65535)` 且 `nFileIdx ∈ [0, 73)` 才处理。
    /// </summary>
    public List<EIMapDrawOp> ScheduleBackground()
    {
        var ops = new List<EIMapDrawOp>();
        if (!HasMySelf || !MapLoadOk)
            return ops;

        int nY = -UNITY * 2;

        for (int j = ClientTop - BlockTop - 1; j <= ClientBottom - BlockTop + MapYExt; j++)
        {
            int nX = AAX + 14 - UNITX;

            int nOffsetY = nY - ClientTop;
            int nPaintY = Math.Max(nOffsetY, 0) + ShakeY;
            if (nPaintY > MAPSURFACEHEIGHT)
                break;

            for (int i = ClientLeft - BlockLeft - 2; i <= ClientRight - BlockLeft + MapYExt; i++)
            {
                int nOffsetX = nX - ClientLeft;
                int nPaintX = Math.Max(nOffsetX, 0) + ShakeX;
                if (nPaintX > SCREENWIDTH)
                    break;

                if (i >= 0 && i < LOGICALMAPUNIT * 3 && j >= 0 && j < LOGICALMAPUNIT * 3)
                {
                    var cell = TileCell(i / 2, j / 2);
                    int nFileIdx = cell.btFileIdx;
                    int nImgNumber = cell.wTileIdx;

                    // 2139：nImgNumber >= 0（Word 恒真，保留原文判断形式）
                    if (nImgNumber >= 0
                        && i % 2 == 0 && j % 2 == 0
                        && nImgNumber != 65535
                        && nFileIdx < BgFileIdxLimit && nFileIdx >= 0)
                    {
                        EIMapTextureInfo? d = null;
                        bool boNeedUpdate = false;

                        if (Texture != null)
                        {
                            if (AutoUpdate && NeedUpdate != null)
                                boNeedUpdate = NeedUpdate(nImgNumber);

                            nOffsetX = nX - ClientLeft;
                            nOffsetY = nY - ClientTop;
                            nPaintX = Math.Max(nOffsetX, 0) + ShakeX;
                            nPaintY = Math.Max(nOffsetY, 0) + ShakeY;

                            var size = ObjectTextureProbe?.Invoke(nImgNumber, nFileIdx);
                            if (size != null)
                            {
                                if (size.Value.Width * size.Value.Height > MinPixelArea)
                                {
                                    int left = 0, top = 0;
                                    int right = size.Value.Width, bottom = size.Value.Height;
                                    if (nOffsetX < 0) left = -nOffsetX;
                                    if (nOffsetY < 0) top = -nOffsetY;

                                    if (left < right && top < bottom
                                        && nPaintX + size.Value.Width >= 0 && nPaintX <= SCREENWIDTH
                                        && nPaintY + size.Value.Height >= 0 && nPaintY <= MAPSURFACEHEIGHT)
                                    {
                                        d = Texture(nFileIdx, nImgNumber, SelfDead);
                                    }
                                }
                                else if (AutoUpdate && boNeedUpdate)
                                {
                                    CanDrawTileMap = true;
                                }
                            }
                            else
                            {
                                d = Texture(nFileIdx, nImgNumber, SelfDead);
                            }
                        }

                        if (d != null)
                        {
                            if (d.Width * d.Height > MinPixelArea)
                            {
                                int left = 0, top = 0;
                                int right = d.Width, bottom = d.Height;
                                if (nOffsetX < 0) left = -nOffsetX;
                                if (nOffsetY < 0) top = -nOffsetY;

                                if (left < right && top < bottom
                                    && nPaintX + d.Width >= 0 && nPaintX <= SCREENWIDTH
                                    && nPaintY + d.Height >= 0 && nPaintY <= MAPSURFACEHEIGHT)
                                {
                                    ops.Add(new EIMapDrawOp(nPaintX, nPaintY, nFileIdx, nImgNumber,
                                        left, top, right, bottom, "BgTile", SelfDead));
                                }
                            }
                            else if (AutoUpdate && boNeedUpdate)
                            {
                                CanDrawTileMap = true;
                            }
                        }
                    }
                }

                nX += UNITX;
            }

            nY += UNITY;
        }

        return ops;
    }

    /// <summary>
    /// ②③ 对象层（2208-2380；两层的 <paramref name="useObj1"/> 决定读 btFileIdx2/wObj1/btObj1Ani
    /// 还是 btFileIdx1/wObj2/btObj2Ani）。
    /// 行从 `ClientTop - BlockTop` 到 `+ LONGHEIGHT_IMAGE`，起点 `m = DefYY - UNITY`；
    /// 负行 **Continue**（仍推进 m）；列起点 `n = DefXX - UNITX*2`；
    /// 越界格 Continue（2218-2219）；`wObj != 65535 且 fileIdx ∈ (0, 75)` 才处理；
    /// 对象尺寸探针命中且面积 ≥ 4 时先做屏幕边界剔除，
    /// 最终仅当纹理恰为 48×32 **且非 blend** 时绘制（`mmm = m + UNITY - h + ShakeY`）。
    /// </summary>
    public List<EIMapDrawOp> ScheduleObjects(bool useObj1)
    {
        var ops = new List<EIMapDrawOp>();
        if (!HasMySelf || !MapLoadOk || ObjectCell == null)
            return ops;

        int m = DefYY - UNITY;

        for (int j = ClientTop - BlockTop; j <= ClientBottom - BlockTop + LONGHEIGHT_IMAGE; j++)
        {
            if (j < 0)
            {
                m += UNITY;
                continue;
            }

            int n = DefXX - UNITX * 2;

            for (int i = ClientLeft - BlockLeft - 2; i <= ClientRight - BlockLeft + 2; i++)
            {
                var cell = ObjectCell(i, j);
                if (cell == null)                      // 2218-2219：越界 Continue
                    continue;

                int nFileIdx = useObj1 ? cell.btFileIdx2 : cell.btFileIdx1;
                int nImgNumber = useObj1 ? cell.wObj1 : cell.wObj2;
                int ani = useObj1 ? cell.btObj1Ani : cell.btObj2Ani;

                if (nImgNumber != 65535 && nFileIdx < ObjFileIdxLimit && nFileIdx > 0
                    && (nFileIdx != 0 || nImgNumber != 0))
                {
                    var (img, blend) = ApplyAnimation(nFileIdx, nImgNumber, ani, AniCount);
                    nImgNumber = img;

                    EIMapTextureInfo? d = null;

                    if (Texture != null)
                    {
                        var size = ObjectTextureProbe?.Invoke(nImgNumber, nFileIdx);
                        if (size != null)
                        {
                            if (size.Value.Width * size.Value.Height >= MinPixelArea)
                            {
                                int mmm = m + UNITY - size.Value.Height + ShakeY;
                                if (n + ShakeX + size.Value.Width >= 0 && n + ShakeX <= SCREENWIDTH
                                    && mmm + size.Value.Height >= 0 && mmm <= MAPSURFACEHEIGHT)
                                {
                                    d = Texture(nFileIdx, nImgNumber, false);   // 对象层恒 Images（无灰度分支）
                                }
                            }
                        }
                        else
                        {
                            d = Texture(nFileIdx, nImgNumber, false);
                        }
                    }

                    if (d != null && d.Width == ObjectWidth && d.Height == ObjectHeight && !blend)
                    {
                        int mmm = m + UNITY - d.Height + ShakeY;
                        if (n + ShakeX + d.Width >= 0 && n + ShakeX <= SCREENWIDTH
                            && mmm + d.Height >= 0 && mmm <= MAPSURFACEHEIGHT)
                        {
                            ops.Add(new EIMapDrawOp(n + ShakeX, mmm, nFileIdx, nImgNumber,
                                0, 0, d.Width, d.Height, useObj1 ? "Obj1" : "Obj2", false));
                        }
                    }
                }

                n += UNITX;
            }

            m += UNITY;
        }

        return ops;
    }

    /// <summary>三段的统一调度顺序：背景格 → 下层对象（Obj1）→ 上层对象（Obj2）。</summary>
    public List<EIMapDrawOp> ComposeSchedule()
    {
        var ops = new List<EIMapDrawOp>();
        if (!HasMySelf || !MapLoadOk)
            return ops;

        ops.AddRange(ScheduleBackground());
        ops.AddRange(ScheduleObjects(true));
        ops.AddRange(ScheduleObjects(false));
        return ops;
    }
}
