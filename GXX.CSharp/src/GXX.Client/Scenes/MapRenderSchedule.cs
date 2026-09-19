using System;
using System.Collections.Generic;

namespace GXX.Client.Scenes;

/// <summary>
/// PlayScn.pas RenderTileMap（1466-1583）地图分块渲染调度核心（批次J46，无纹理 headless 化）：
/// 背景大图块层（偶偶格、wBkImg 非 EN 地图 & $7FFF 掩码、imgNumber-1、屏幕边界过滤）与
/// 中间层（wMidImg、旧图 nY=-UNITY 起新图 -UNITY*2、无边界过滤），行 nY += UNITY、列 nX += UNITX。
/// 常量：UNITX=48/UNITY=32（MShare 107-108）、LOGICALMAPUNIT=40（MShare 111）、
/// LONGHEIGHT_IMAGE=32/AAX=16（PlayScn 40-42）、SCREENWIDTH=1024/MAPSURFACEHEIGHT=768（SDK 324-335）。
/// </summary>
public sealed class MapRenderSchedule
{
    public const int UNITX = 48;
    public const int UNITY = 32;
    public const int LOGICALMAPUNIT = 40;
    public const int LONGHEIGHT_IMAGE = 32;
    public const int AAX = 16;
    public const int SCREENWIDTH = 1024;
    public const int MAPSURFACEWIDTH = 1024;
    public const int MAPSURFACEHEIGHT = 768;
    public const int BK_MASK = 0x7FFF;

    public enum MapLayer
    {
        BkTile,   // 背景大图块（偶偶格）
        MidTile,  // 地图中间层
    }

    /// <summary>一条绘制调度（像素坐标 + 图号；headless 不含纹理本体）。</summary>
    public struct MapDrawOp
    {
        public MapLayer Layer;
        public int CellI;
        public int CellJ;
        public int PixelX;
        public int PixelY;
        public int ImgNumber;
    }

    /// <summary>地图单元格数据（m_MArr[I,J] 切片）。</summary>
    public struct MapCell
    {
        public ushort wBkImg;
        public ushort wMidImg;
        public byte btUnitBkImg;
        public byte btUnitMidImg;
    }

    /// <summary>图块尺寸探测接缝（GameImages.Images[n] 的 Width/Height 等效；null = 纹理未就绪）。</summary>
    public Func<byte, int, (int Width, int Height)?>? TextureProbe;

    /// <summary>相机可视块区域（Map.m_ClientRect 与 m_nBlockLeft/Top）。</summary>
    public int ClientLeft, ClientTop, ClientRight, ClientBottom;
    public int BlockLeft, BlockTop;
    public bool boENMap;
    public bool boNewMap;
    public bool MapLoadOk = true;
    public bool SelfDead;

    /// <summary>单元格取值接缝（m_MArr[I,J] 等效）。</summary>
    public Func<int, int, MapCell> Cell = (_, _) => default;

    /// <summary>
    /// RenderTileMap 背景 + 中间层调度 1:1：行 J 从 ClientRect.Top-BlockTop-1 到
    /// Bottom-BlockTop+10（MapYExt=10），列 I 从 Left-BlockLeft-2 到 Right-BlockLeft+1；
    /// 背景层 nY 起点 -UNITY*2 且仅偶偶格、Bk 图号 $7FFF 掩码（EN 地图不掩）后 -1、
    /// 屏幕边界过滤（nX+w>0 且 nX<=SCREENWIDTH 且 nY+h>0 且 nY<MAPSURFACEHEIGHT）；
    /// 中间层起点旧图 -UNITY/新图 -UNITY*2，图号 wMidImg-1，无边界过滤。
    /// </summary>
    public List<MapDrawOp> RenderTileMapSchedule()
    {
        var ops = new List<MapDrawOp>();
        if (!MapLoadOk)
            return ops;

        const int mapYExt = 10;
        var probe = TextureProbe;

        // ---- 地图背景（背景大图块层） ----
        int rowJ = ClientTop - BlockTop - 1;
        int colI = ClientLeft - BlockLeft - 2;
        int nY = -UNITY * 2;
        for (int j = rowJ; j <= ClientBottom - BlockTop + mapYExt; j++)
        {
            int nX = AAX + 14 - UNITX;
            for (int i = colI; i <= ClientRight - BlockLeft + 1; i++)
            {
                if (i >= 0 && i < LOGICALMAPUNIT * 3 && j >= 0 && j < LOGICALMAPUNIT * 3)
                {
                    var cell = Cell(i, j);
                    int nImgNumber = boENMap ? cell.wBkImg : (cell.wBkImg & BK_MASK);
                    if (nImgNumber > 0 && i % 2 == 0 && j % 2 == 0)
                    {
                        nImgNumber -= 1;
                        (int, int)? size = probe?.Invoke(cell.btUnitBkImg, nImgNumber);
                        if (size != null)
                        {
                            var (width, height) = size.Value;
                            if (width * height > 4)
                            {
                                if (nX + width > 0 && nX <= SCREENWIDTH && nY + height > 0 && nY < MAPSURFACEHEIGHT)
                                    ops.Add(new MapDrawOp { Layer = MapLayer.BkTile, CellI = i, CellJ = j, PixelX = nX, PixelY = nY, ImgNumber = nImgNumber });
                            }
                        }
                    }
                }
                nX += UNITX;
            }
            nY += UNITY;
        }

        // ---- 地图中间层（旧图 nY=-UNITY；新图 nY=-UNITY*2） ----
        nY = boNewMap ? -UNITY * 2 : -UNITY;
        for (int j = rowJ; j <= ClientBottom - BlockTop + mapYExt; j++)
        {
            int nX = AAX + 14 - UNITX;
            for (int i = colI; i <= ClientRight - BlockLeft + 1; i++)
            {
                if (i >= 0 && i < LOGICALMAPUNIT * 3 && j >= 0 && j < LOGICALMAPUNIT * 3)
                {
                    var cell = Cell(i, j);
                    int nImgNumber = cell.wMidImg;
                    if (nImgNumber > 0)
                    {
                        nImgNumber -= 1;
                        (int, int)? size = probe?.Invoke(cell.btUnitMidImg, nImgNumber);
                        if (size != null)
                        {
                            var (width, height) = size.Value;
                            if (width * height > 4)
                                ops.Add(new MapDrawOp { Layer = MapLayer.MidTile, CellI = i, CellJ = j, PixelX = nX, PixelY = nY, ImgNumber = nImgNumber });
                        }
                    }
                }
                nX += UNITX;
            }
            nY += UNITY;
        }

        return ops;
    }
}
