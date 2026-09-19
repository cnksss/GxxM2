using System;

namespace GXX.Core.Util;

/// <summary>
/// UnitPath.pas 1:1 类型声明（PathFind.pas 与本单元共用的地图/路径类型）。
/// </summary>
public enum TerrainType
{
    ttNormal,    // 平地
    ttSand,      // 沙地
    ttForest,    // 树林
    ttRoad,      // 马路
    ttObstacle,  // 障碍物
    ttPath       // 路径
}

/// <summary>TTerrainParam。</summary>
public struct TerrainParam
{
    public string CellLabel;
    public int MoveCost;
}

/// <summary>TPath = array of TPoint。</summary>
public class PathPoint
{
    public int X;
    public int Y;

    public PathPoint() { }
    public PathPoint(int x, int y) { X = x; Y = y; }
}

/// <summary>TPathMapCell：路径图元。</summary>
public struct TPathMapCell
{
    public int Distance;    // 离起点的距离
    public int Direction;
}

/// <summary>TCellParams。</summary>
public struct TCellParams
{
    public TerrainType TerrainType;
    public bool OnPath;
}

/// <summary>TGetCostFunc(X, Y, Direction) → 成本；-1 = 不可通行。</summary>
public delegate int TGetCostFunc(int x, int y, int direction);

public static class PathFindConst
{
    /// <summary>TerrainParams[TTerrainTypes] of MoveCost（原常量数组）。</summary>
    public static int TerrainMoveCost(TerrainType t) => t switch
    {
        TerrainType.ttNormal => 4,     // 平地
        TerrainType.ttSand => 6,       // 沙地
        TerrainType.ttForest => 10,    // 树林
        TerrainType.ttRoad => 2,       // 马路
        TerrainType.ttObstacle => -1,  // 障碍物
        TerrainType.ttPath => 0,       // 路径
        _ => 4
    };
}
