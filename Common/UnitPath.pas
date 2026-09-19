unit UnitPath;

interface
uses
  Windows, Classes;
type
   // 地图元素分类
  TTerrainTypes = (ttNormal, ttSand, ttForest, ttRoad, ttObstacle, ttPath);
  TTerrainParam = record
    // CellColor: TColor;
    CellLabel: string[16];
    MoveCost: Integer;
  end;

  TPath = array of TPoint;                                                                          // 路径数组

  TPathMapCell = record                                                                             // 路径图元
    Distance: Integer;                                                                              // 离起点的距离
    Direction: Integer;
  end;
  TPathMapArray = array of array of TPathMapCell;                                                   // 路径图存储数组

  TCellParams = record
    TerrainType: TTerrainTypes;
    OnPath: Boolean;
  end;
  TMapData = array of array of TCellParams;                                                         // 地图存储数组(算法可识别格式)

  TGetCostFunc = procedure(Sender: Tobject; X, Y, Direction: Integer; var Result: Integer) of object;
implementation

end.
