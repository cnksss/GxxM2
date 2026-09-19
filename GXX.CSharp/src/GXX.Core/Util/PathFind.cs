using System;
using System.IO;
using GXX.Core.Rtl;

namespace GXX.Core.Util;

/// <summary>
/// PathFind.pas 1:1 转换：波扩散（Dijkstra 变体）寻路。
/// 方向布局（与原实现一致，注意与 Grobal2 DR_ 布局不同）：
///   7 0 1
///   6 X 2
///   5 4 3
/// PathMapArray 按 [Y, X] 索引；TLegendMap.MapData 按 [X, Y] 索引（保持原实现的差异）。
/// </summary>
public class TPathMap
{
    public TPathMapCell[][] PathMapArray = Array.Empty<TPathMapCell[]>();
    public int MapHeight;
    public int MapWidth;
    public TGetCostFunc? GetCostFunc;

    public TPathMap() { }

    public virtual int DirToDX(int direction)
    {
        if (direction == 0 || direction == 4) return 0;
        if (direction >= 1 && direction <= 3) return 1;
        return -1;
    }

    public virtual int DirToDY(int direction)
    {
        if (direction == 2 || direction == 6) return 0;
        if (direction >= 3 && direction <= 5) return 1;
        return -1;
    }

    public PathPoint[]? FindPath(int mapWidthin, int mapHeightin, int startX, int startY, int stopX, int stopY, TGetCostFunc pGetCostFunc)
    {
        MapWidth = mapWidthin;
        MapHeight = mapHeightin;
        GetCostFunc = pGetCostFunc;
        PathMapArray = FillPathMap(startX, startY, stopX, stopY);
        return FindPathOnMap(stopX, stopY);
    }

    /// <summary>从路径图回溯出 TPath：首元素=起点，末元素=终点；nil=不可达。</summary>
    public PathPoint[]? FindPathOnMap(int x, int y)
    {
        if (x >= MapWidth || y >= MapHeight)
            return null;
        if (PathMapArray[y][x].Distance < 0)
            return null;
        var result = new PathPoint[PathMapArray[y][x].Distance + 1];
        while (PathMapArray[y][x].Distance > 0)
        {
            result[PathMapArray[y][x].Distance] = new PathPoint(x, y);
            int direction = PathMapArray[y][x].Direction;
            x -= DirToDX(direction);
            y -= DirToDY(direction);
        }
        result[0] = new PathPoint(x, y);
        return result;
    }

    public void MakePathMap(int mapWidthin, int mapHeightin, int startX, int startY, TGetCostFunc pGetCostFunc)
    {
        MapWidth = mapWidthin;
        MapHeight = mapHeightin;
        GetCostFunc = pGetCostFunc;
        PathMapArray = FillPathMap(startX, startY, -1, -1);
    }

    protected virtual int GetCost(int x, int y, int direction)
    {
        direction &= 7;
        if (x < 0 || x >= MapWidth || y < 0 || y >= MapHeight)
            return -1;
        return GetCostFunc != null ? GetCostFunc(x, y, direction) : -1;
    }

    /// <summary>波扩散寻路核心（FillPathMap）：成本递减波 + 8 邻域。</summary>
    protected TPathMapCell[][] FillPathMap(int x1, int y1, int x2, int y2)
    {
        // PreparePathMap：全部 Distance := -1
        var result = new TPathMapCell[MapHeight][];
        for (int yy = 0; yy < MapHeight; yy++)
        {
            result[yy] = new TPathMapCell[MapWidth];
            for (int xx = 0; xx < MapWidth; xx++)
                result[yy][xx].Distance = -1;
        }

        var oldWave = new TWave();
        var newWave = new TWave();

        result[y1][x1].Distance = 0;           // 起点距离 0
        oldWave.Add(x1, y1, 0, 0);
        TestNeighbours(result, oldWave, newWave);

        bool finished = x1 == x2 && y1 == y2;
        while (!finished)
        {
            // ExchangeWaves
            var w = oldWave;
            oldWave = newWave;
            newWave = w;
            newWave.Clear();
            if (!oldWave.Start())
                break;
            do
            {
                var cell = oldWave.Item;
                int cost = cell.Cost - oldWave.MinCost;
                if (cost > 0)
                {
                    // 成本未耗尽：折减后加入新波
                    newWave.Add(cell.X, cell.Y, cost, cell.Direction);
                }
                else
                {
                    // 处理最小成本点
                    if (result[cell.Y][cell.X].Distance >= 0)
                        continue;
                    result[cell.Y][cell.X].Distance =
                        result[cell.Y - DirToDY(cell.Direction)][cell.X - DirToDX(cell.Direction)].Distance + 1;
                    result[cell.Y][cell.X].Direction = cell.Direction;
                    finished = cell.X == x2 && cell.Y == y2;
                    if (finished)
                        break;
                    TestNeighbours(result, oldWave, newWave);
                }
            } while (oldWave.Next());
        }
        return result;
    }

    private void TestNeighbours(TPathMapCell[][] result, TWave oldWave, TWave newWave)
    {
        for (int d = 0; d < 8; d++)
        {
            int x = oldWave.Item.X + DirToDX(d);
            int y = oldWave.Item.Y + DirToDY(d);
            int c = GetCost(x, y, d);
            // Delphi {$B-} 短路：出界(GetCost=-1)时不访问 result
            if (c >= 0 && x >= 0 && x < MapWidth && y >= 0 && y < MapHeight && result[y][x].Distance < 0)
                newWave.Add(x, y, c, d);
        }
    }
}

/// <summary>TWaveCell：路线点。</summary>
public struct TWaveCell
{
    public int X;
    public int Y;
    public int Cost;
    public int Direction;
}

/// <summary>TWave：波容器（Add/Start/Next/MinCost 语义 1:1）。</summary>
public class TWave
{
    private TWaveCell[] _data = new TWaveCell[30];
    private int _pos;
    private int _count;
    private int _minCost = int.MaxValue;

    public TWaveCell Item => _data[_pos];
    public int MinCost => _minCost;

    public void Add(int newX, int newY, int newCost, int newDirection)
    {
        if (_count >= _data.Length)
            Array.Resize(ref _data, _data.Length + 30);
        _data[_count].X = newX;
        _data[_count].Y = newY;
        _data[_count].Cost = newCost;
        _data[_count].Direction = newDirection;
        if (newCost < _minCost)
            _minCost = newCost;
        _count++;
    }

    public void Clear()
    {
        _pos = 0;
        _count = 0;
        _minCost = int.MaxValue;
    }

    public bool Start()
    {
        _pos = 0;
        return _count > 0;
    }

    public bool Next()
    {
        _pos++;
        return _pos < _count;
    }
}

/// <summary>
/// TLegendMap：传奇 .map 地图读取 + 寻路（TLegendMap.GetCost）。
/// TMapHeader 52 字节：Width u16, Height u16, Title[16], UpdateDate f64, Reserved[24]。
/// TMapBlock 12 字节：BkImg/MidImg/FrImg u16 + 6 字节标志；BkImg $8000 位 = 障碍。
/// 原 GetCost 依赖客户端 PlayScene.CanWalkEx → 此处为虚方法 CanWalkEx（默认按地形），客户端覆写。
/// </summary>
public class TLegendMap : TPathMap
{
    public struct TMapBlock
    {
        public ushort BkImg;
        public ushort MidImg;
        public ushort FrImg;
        public byte flag;
        public byte offset;
        public byte framecount;
        public byte delaytime;
        public byte objgroup;
        public byte unused;
    }

    private TMapBlock[][] _mapBuf = Array.Empty<TMapBlock[]>();
    private string _title = "";

    /// <summary>MapData[X, Y]（与原实现一致：按 X,Y 索引）。</summary>
    public TCellParams[][] MapData = Array.Empty<TCellParams[]>();
    public string Title => _title;

    public bool LoadMap(string mapFile)
    {
        try
        {
            using var fs = new FileStream(mapFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var br = new BinaryReader(fs);
            // TMapHeader
            ushort width = br.ReadUInt16();
            ushort height = br.ReadUInt16();
            byte[] titleBytes = br.ReadBytes(16);
            double updateDate = br.ReadDouble();
            br.ReadBytes(24); // Reserved

            MapWidth = width;
            MapHeight = height;
            _title = DelphiRTL.StrPas(titleBytes) + updateDate.ToDelphiDateString();

            // MapBuf[width, height]（列主序读取）
            _mapBuf = new TMapBlock[width][];
            for (int i = 0; i < width; i++)
            {
                _mapBuf[i] = new TMapBlock[height];
                for (int j = 0; j < height; j++)
                {
                    var block = new TMapBlock
                    {
                        BkImg = br.ReadUInt16(),
                        MidImg = br.ReadUInt16(),
                        FrImg = br.ReadUInt16(),
                        flag = br.ReadByte(),
                        offset = br.ReadByte(),
                        framecount = br.ReadByte(),
                        delaytime = br.ReadByte(),
                        objgroup = br.ReadByte(),
                        unused = br.ReadByte()
                    };
                    _mapBuf[i][j] = block;
                }
            }

            // MapData[X, Y]
            MapData = new TCellParams[width][];
            for (int i = 0; i < MapWidth; i++)
            {
                MapData[i] = new TCellParams[height];
                for (int j = 0; j < MapHeight; j++)
                {
                    MapData[i][j].TerrainType =
                        (_mapBuf[i][j].BkImg & 0x8000) == 0 ? TerrainType.ttNormal : TerrainType.ttObstacle;
                }
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public PathPoint[]? FindPath(int startX, int startY, int stopX, int stopY)
    {
        PathMapArray = FillPathMap(startX, startY, stopX, stopY);
        return FindPathOnMap(stopX, stopY);
    }

    public new PathPoint[]? FindPath(int stopX, int stopY)
        => FindPathOnMap(stopX, stopY);

    public void SetStartPos(int startX, int startY)
    {
        PathMapArray = FillPathMap(startX, startY, -1, -1);
    }

    /// <summary>原依赖 PlayScene.CanWalkEx（客户端）；默认按地形判定，客户端可覆写。</summary>
    protected virtual bool CanWalkEx(int x, int y)
        => x >= 0 && x < MapWidth && y >= 0 && y < MapHeight &&
           MapData[x][y].TerrainType != TerrainType.ttObstacle;

    protected override int GetCost(int x, int y, int direction)
    {
        direction &= 7;
        if (x < 0 || x >= MapWidth || y < 0 || y >= MapHeight)
            return -1;

        int result;
        if (CanWalkEx(x, y))
            result = 4;
        else
            result = -1;

        // PathWidth = 0：四邻地形成本和 < 4×平地成本 → 障碍
        const int pathWidth = 0;
        if (x < MapWidth - pathWidth && x > pathWidth && y < MapHeight - pathWidth && y > pathWidth)
        {
            int cost = PathFindConst.TerrainMoveCost(MapData[x - pathWidth][y].TerrainType)
                     + PathFindConst.TerrainMoveCost(MapData[x + pathWidth][y].TerrainType)
                     + PathFindConst.TerrainMoveCost(MapData[x][y - pathWidth].TerrainType)
                     + PathFindConst.TerrainMoveCost(MapData[x][y + pathWidth].TerrainType);
            if (cost < 4 * PathFindConst.TerrainMoveCost(TerrainType.ttNormal))
                result = -1;
        }

        if ((direction & 1) == 1 && result > 0)  // 斜方向成本 ×1.5（近似）
            result += result >> 1;

        return result;
    }
}

internal static class LegendMapDateExt
{
    /// <summary>原 Title 追加 FormatDateTime('yy-mm-dd', UpdateDate)。</summary>
    public static string ToDelphiDateString(this double delphiDate)
    {
        if (delphiDate <= 0) return "00-01-01";
        try
        {
            var dt = DateTime.FromOADate(delphiDate);
            return dt.ToString("yy-MM-dd");
        }
        catch
        {
            return "00-01-01";
        }
    }
}
