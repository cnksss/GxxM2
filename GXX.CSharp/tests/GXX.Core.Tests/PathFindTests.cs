using System;
using System.IO;
using GXX.Core.Util;
using Xunit;

namespace GXX.Core.Tests;

/// <summary>PathFind.pas（TPathMap 波扩散寻路 + TLegendMap 地图寻路）测试。</summary>
public class PathFindTests
{
    /// <summary>合成 TLegendMap.LoadMap 格式地图（列主序，TMapBlock 12 字节）。</summary>
    private static string CreateLegendMap(string path, int width, int height, Func<int, int, bool> isObstacle)
    {
        using var fs = new FileStream(path, FileMode.Create);
        using var bw = new BinaryWriter(fs);
        // TMapHeader：Width u16 + Height u16 + Title[16] + UpdateDate f64 + Reserved[24] = 52
        bw.Write((ushort)width);
        bw.Write((ushort)height);
        bw.Write(new byte[16]);
        bw.Write(0.0);
        bw.Write(new byte[24]);
        // 列主序：for x(宽度) for y(高度)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                ushort bkImg = isObstacle(x, y) ? (ushort)0x8000 : (ushort)1;
                bw.Write(bkImg);        // BkImg（$8000 位 = 障碍）
                bw.Write((ushort)0);    // MidImg
                bw.Write((ushort)0);    // FrImg
                bw.Write(new byte[6]);  // flag/offset/framecount/delaytime/objgroup/unused
            }
        }
        return path;
    }

    [Fact]
    public void DirToDX_DY_MatchesDelphiLayout()
    {
        // 布局：7 0 1 / 6 X 2 / 5 4 3
        var map = new TPathMap();
        Assert.Equal(0, map.DirToDX(0));
        Assert.Equal(1, map.DirToDX(1));
        Assert.Equal(1, map.DirToDX(2));
        Assert.Equal(1, map.DirToDX(3));
        Assert.Equal(0, map.DirToDX(4));
        Assert.Equal(-1, map.DirToDX(5));
        Assert.Equal(-1, map.DirToDX(6));
        Assert.Equal(-1, map.DirToDX(7));

        Assert.Equal(-1, map.DirToDY(0));
        Assert.Equal(-1, map.DirToDY(1));
        Assert.Equal(0, map.DirToDY(2));
        Assert.Equal(1, map.DirToDY(3));
        Assert.Equal(1, map.DirToDY(4));
        Assert.Equal(1, map.DirToDY(5));
        Assert.Equal(0, map.DirToDY(6));
        Assert.Equal(-1, map.DirToDY(7));
    }

    [Fact]
    public void TPathMap_StraightLinePath()
    {
        // 10x10 全通地图：成本函数恒 4
        var map = new TPathMap();
        var path = map.FindPath(10, 10, 2, 2, 6, 2, (x, y, d) => 4);
        Assert.NotNull(path);
        // 路径含起点与终点
        Assert.Equal(2, path[0].X);
        Assert.Equal(2, path[0].Y);
        var last = path[path.Length - 1];
        Assert.Equal(6, last.X);
        Assert.Equal(2, last.Y);
        // 直线路径步数 = 4
        Assert.Equal(4, path.Length - 1);
        // 每步都是相邻格
        for (int i = 1; i < path.Length; i++)
        {
            int dx = Math.Abs(path[i].X - path[i - 1].X);
            int dy = Math.Abs(path[i].Y - path[i - 1].Y);
            Assert.True(dx <= 1 && dy <= 1);
        }
    }

    [Fact]
    public void TPathMap_AroundObstacle()
    {
        // 7x7，中间竖墙 x=3 (y=1..5) 阻挡，只留上下口
        var map = new TPathMap();
        var path = map.FindPath(7, 7, 1, 3, 5, 3, (x, y, d) =>
            x == 3 && y >= 1 && y <= 5 ? -1 : 4);
        Assert.NotNull(path);
        var last = path[path.Length - 1];
        Assert.Equal(5, last.X);
        Assert.Equal(3, last.Y);
        // 必须绕行：步数 > 曼哈顿距离 4
        Assert.True(path.Length - 1 > 4);
        // 不得穿越墙
        foreach (var p in path)
            Assert.False(p.X == 3 && p.Y >= 1 && p.Y <= 5);
    }

    [Fact]
    public void TPathMap_Unreachable_ReturnsNull()
    {
        var map = new TPathMap();
        var path = map.FindPath(5, 5, 0, 0, 4, 4, (x, y, d) => -1); // 全阻挡
        Assert.Null(path);
    }

    [Fact]
    public void TLegendMap_LoadMap_AndTerrain()
    {
        string mapFile = Path.Combine(Path.GetTempPath(), "pf_" + Guid.NewGuid().ToString("N") + ".map");
        try
        {
            CreateLegendMap(mapFile, 20, 15, (x, y) => x == 10 && y == 7);
            var legend = new TLegendMap();
            Assert.True(legend.LoadMap(mapFile));
            Assert.Equal(20, legend.MapWidth);
            Assert.Equal(15, legend.MapHeight);
            Assert.Equal(TerrainType.ttNormal, legend.MapData[0][0].TerrainType);
            Assert.Equal(TerrainType.ttObstacle, legend.MapData[10][7].TerrainType);
        }
        finally
        {
            File.Delete(mapFile);
        }
    }

    [Fact]
    public void TLegendMap_FindPath_AroundObstacle()
    {
        string mapFile = Path.Combine(Path.GetTempPath(), "pf_" + Guid.NewGuid().ToString("N") + ".map");
        try
        {
            // 全图可走，但 (5,3..6) 一列障碍
            CreateLegendMap(mapFile, 16, 16, (x, y) => x == 5 && y >= 3 && y <= 6);
            var legend = new TLegendMap();
            Assert.True(legend.LoadMap(mapFile));
            var path = legend.FindPath(3, 5, 7, 5);
            Assert.NotNull(path);
            Assert.Equal(7, path[path.Length - 1].X);
            foreach (var p in path)
                Assert.False(p.X == 5 && p.Y >= 3 && p.Y <= 6, "路径不得穿越障碍列");
        }
        finally
        {
            File.Delete(mapFile);
        }
    }

    [Fact]
    public void TLegendMap_SetStartPos_ThenFindPathOnMap()
    {
        string mapFile = Path.Combine(Path.GetTempPath(), "pf_" + Guid.NewGuid().ToString("N") + ".map");
        try
        {
            CreateLegendMap(mapFile, 16, 16, (x, y) => false);
            var legend = new TLegendMap();
            legend.LoadMap(mapFile);
            legend.SetStartPos(4, 4);
            var path = legend.FindPath(8, 4);
            Assert.NotNull(path);
            Assert.Equal(4, path.Length - 1); // 直线 4 步
        }
        finally
        {
            File.Delete(mapFile);
        }
    }

    [Fact]
    public void TerrainParams_MoveCost_MatchesDelphi()
    {
        Assert.Equal(4, PathFindConst.TerrainMoveCost(TerrainType.ttNormal));
        Assert.Equal(6, PathFindConst.TerrainMoveCost(TerrainType.ttSand));
        Assert.Equal(10, PathFindConst.TerrainMoveCost(TerrainType.ttForest));
        Assert.Equal(2, PathFindConst.TerrainMoveCost(TerrainType.ttRoad));
        Assert.Equal(-1, PathFindConst.TerrainMoveCost(TerrainType.ttObstacle));
        Assert.Equal(0, PathFindConst.TerrainMoveCost(TerrainType.ttPath));
    }
}
