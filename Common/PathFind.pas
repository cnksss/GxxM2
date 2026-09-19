(******************************************************************************
  关于TLegendMap(位于PathFind.pas)的用法
  1、FLegendMap:=TLegendMap.Create;
     FLegendMap.LoadMap('mapfile')
        --成功返回后生成地图数据FLegendMap.MapData[i, j]:TMapData
     FLegendMap.SetStartPos(StartX, StartY,PathSpace)
     Path:=FLegendMap.FindPath(StopX, StopY)
  2、FLegendMap:=TLegendMap.Create;
     FLegendMap.LoadMap('mapfile')
     Path:=FLegendMap.FindPath(StartX,StartY,StopX, StopY,PathSpace)

     其中
     Path为TPath = array of TPoint 为nil时表示不能到达
     第一个值为起点，最后一个值为终点
     High(Path)即路径需要的步数

     PathSpace为离开障碍物多少个象素
******************************************************************************)

(*****************************************************************************
  关于TPathMap的特点
  1、不需要传递地图数据，节省内存的频繁拷贝
  2、可自定义估价函数，根据自己需要产生不同路径

  关于TPathMap的用法
  1、定义估价函数MovingCost(X, Y, Direction: Integer)
     只需根据自定义的地图格式编写)
  2、FPathMap:=TPathMap.Create;
     FPathMap.MakePathMap(MapHeader.width, MapHeader.height, StartX, StartY,MovingCost);
     Path:=FPathMap.FindPathOnMap( EndX, EndY)
     其中Path为TPath = array of TPoint;

  如果不喜欢在TPathMap外部定义估价函数，可继承TPathMap，
  将地图数据的读取和估价函数封装成一个类使用。
*******************************************************************************)
unit PathFind;

interface

uses
  Windows, Classes, SysUtils, Graphics;

type
   // 地图元素分类
  TTerrainTypes = (ttNormal, ttSand, ttForest, ttRoad, ttObstacle, ttPath);
  TTerrainParam = record
    CellColor: TColor;
    CellLabel: string[16];
    MoveCost: Integer;
  end;

  TPath = array of TPoint; // 路径数组

  TPathMapCell = record // 路径图元
    Distance: Integer; // 离起点的距离
    Direction: Integer;
  end;
  TPathMapArray = array of array of TPathMapCell; // 路径图存储数组

  TMapHeader = packed record // 传奇地图数据头结构 52
    Width: Word; // 2
    Height: Word; // 2
    Title: array[1..16] of char; // 标题      16
    UpdateDate: TDateTime; // 8
    Reserved: array[0..23] of char; // 保留
  end;

  TMapBlock = record // 传奇地图数据体结构
    BkImg: Word;
    MidImg: Word;
    FrImg: Word;
    flag: Byte;
    offset: Byte;
    framecount: Byte;
    delaytime: Byte;
    objgroup: Byte;
    unused: Byte;
  end;
  TMapBuf = array of array of TMapBlock; // 传奇地图存储数组

  TCellParams = record
    TerrainType: TTerrainTypes;
    OnPath: Boolean;
  end;
  TMapData = array of array of TCellParams; // 地图存储数组(算法可识别格式)

  TGetCostFunc = function(X, Y, Direction: Integer; PathWidth: Integer = 0): Integer;

  TPathMap = class // 寻路类
  public
    PathMapArray: TPathMapArray;
    MapHeight: Integer;
    MapWidth: Integer;
    GetCostFunc: TGetCostFunc;
    // PathWidth: Integer;

    constructor Create;
    function FindPath(MapWidthin, MapHeightin: Integer; StartX, StartY, StopX, StopY: Integer;
      pGetCostFunc: TGetCostFunc): TPath;
    function FindPathOnMap(X, Y: Integer): TPath;
    procedure MakePathMap(MapWidthin, MapHeightin: Integer; StartX, StartY: Integer;
      pGetCostFunc: TGetCostFunc);

  private
    function DirToDX(Direction: Integer): Integer;
    function DirToDY(Direction: Integer): Integer;

  protected
    function GetCost(X, Y, Direction: Integer): Integer; virtual;
    function FillPathMap(X1, Y1, X2, Y2: Integer): TPathMapArray;
  end;

  TLegendMap = class(TPathMap) // 传奇地图读取及寻路类
  private
    FPath: TPath;
    MapBuf: TMapBuf;
    MapHeader: TMapHeader;
    FCX: Integer;
    FCY: Integer;
    FHX: Integer;
    FHY: Integer;
    function XYToCurrXY(X, Y: Integer; var nX, nY: Integer): Boolean;
  public
    MapData: TMapData;
    Title: string;
    property Path: TPath read FPath write FPath;

    function LoadMap(MapFile: string): Boolean;
    function FindPath(StartX, StartY, StopX, StopY: Integer): TPath; overload;
    function FindPath(StopX, StopY: Integer): TPath; overload;
    procedure SetStartPos(StartX, StartY: Integer);

  protected
    function GetCost(X, Y, Direction: Integer): Integer; override;
  end;

  TWaveCell = record // 路线点
    X, Y: Integer; // 
    Cost: Integer; // 
    Direction: Integer;
  end;

  TWave = class // 路线类
  private
    FData: array of TWaveCell;
    FPos: Integer; // 
    FCount: Integer; // 
    FMinCost: Integer;
    function GetItem: TWaveCell;
  public
    property Item: TWaveCell read GetItem; // 
    property MinCost: Integer read FMinCost; // Cost

    constructor Create;
    destructor Destroy; override;
    procedure Add(NewX, NewY, NewCost, NewDirection: Integer); // 
    procedure Clear; // FCount
    function Start: Boolean; // 
    function Next: Boolean; // 
  end;

const
  TerrainParams: array[TTerrainTypes] of TTerrainParam = (
    (CellColor: clWhite; CellLabel: '平地'; MoveCost: 4),
    (CellColor: clOlive; CellLabel: '沙地'; MoveCost: 6),
    (CellColor: clGreen; CellLabel: '树林'; MoveCost: 10),
    (CellColor: clSilver; CellLabel: '马路'; MoveCost: 2),
    (CellColor: clBlack; CellLabel: '障碍物'; MoveCost: - 1),
    (CellColor: clRed; CellLabel: '路径'; MoveCost: 0));


implementation
uses ClMain;
constructor TWave.Create;
begin
  Clear; // 
end;

destructor TWave.Destroy;
begin
  FData := nil; // 
  inherited Destroy;
end;

function TWave.GetItem: TWaveCell;
begin
  Result := FData[FPos]; // 
end;

procedure TWave.Add(NewX, NewY, NewCost, NewDirection: Integer);
begin
  if FCount >= Length(FData) then // 
    SetLength(FData, Length(FData) + 30); // 
  with FData[FCount] do
  begin
    X := NewX;
    Y := NewY;
    Cost := NewCost;
    Direction := NewDirection;
  end;
  if NewCost < FMinCost then // NewCost
    FMinCost := NewCost;
  Inc(FCount); // 
end;

procedure TWave.Clear;
begin
  FPos := 0;
  FCount := 0;
  FMinCost := High(Integer);
end;

function TWave.Start: Boolean;
begin
  FPos := 0; // 
  Result := (FCount > 0); // 
end;

function TWave.Next: Boolean;
begin
  Inc(FPos); // 
  Result := (FPos < FCount); // false,
end;

constructor TPathMap.Create;
begin
  inherited Create;
end;

function TPathMap.FindPath(MapWidthin, MapHeightin: Integer; StartX, StartY, StopX, StopY: Integer;
  pGetCostFunc: TGetCostFunc): TPath;
begin
  MapWidth := MapWidthin; // 
  MapHeight := MapHeightin; // 
  GetCostFunc := pGetCostFunc;
  PathMapArray := FillPathMap(StartX, StartY, StopX, StopY);
  Result := FindPathOnMap(StopX, StopY);
end;

// *************************************************************
// 从TPathMap中找出 TPath
// *************************************************************
function TPathMap.FindPathOnMap(X, Y: Integer): TPath;
var
  Direction: Integer;
begin
  Result := nil;
  if (X >= MapWidth) or (Y >= MapHeight) then
    Exit;
  if PathMapArray[Y, X].Distance < 0 then
    Exit;
  SetLength(Result, PathMapArray[Y, X].Distance + 1); // 
  while PathMapArray[Y, X].Distance > 0 do
  begin
    Result[PathMapArray[Y, X].Distance] := Point(X, Y);
    Direction := PathMapArray[Y, X].Direction;
    X := X - DirToDX(Direction);
    Y := Y - DirToDY(Direction);
  end;
  Result[0] := Point(X, Y);
end;

procedure TPathMap.MakePathMap(MapWidthin, MapHeightin: Integer; StartX, StartY: Integer;
  pGetCostFunc: TGetCostFunc);
begin
  MapWidth := MapWidthin;
  MapHeight := MapHeightin; // 
  GetCostFunc := pGetCostFunc;
  PathMapArray := FillPathMap(StartX, StartY, -1, -1);
end;

// *************************************************************
// 方向编号转为X方向符号
// 7  0  1
// 6  X  2
// 5  4  3
// *************************************************************
function TPathMap.DirToDX(Direction: Integer): Integer;
begin
  case Direction of
    0, 4: Result := 0;
    1..3: Result := 1;
  else
    Result := -1;
  end;
end;

function TPathMap.DirToDY(Direction: Integer): Integer;
begin
  case Direction of
    2, 6: Result := 0;
    3..5: Result := 1;
  else
    Result := -1;
  end;
end;

// *************************************************************
// 寻路算法
// X1,Y1为路径运算起点，X2，Y2为路径运算终点
// *************************************************************
function TPathMap.FillPathMap(X1, Y1, X2, Y2: Integer): TPathMapArray;
var
  OldWave, NewWave: TWave;
  Finished: Boolean;
  I: TWaveCell;

  procedure PreparePathMap; // 初始化PathMapArray
  var
    X, Y: Integer; // 
  begin
    SetLength(Result, MapHeight, MapWidth);
    for Y := 0 to (MapHeight - 1) do
      for X := 0 to (MapWidth - 1) do
        Result[Y, X].Distance := -1;
  end;

// 计算相邻8个节点的权cost，并合法点加入NewWave(),并更新最小cost
// 合法点是指非障碍物且Result[X，Y]中未访问的点
  procedure TestNeighbours;
  var
    X, Y, C, D: Integer;
  begin
    for D := 0 to 7 do
    begin
      X := OldWave.Item.X + DirToDX(D);
      Y := OldWave.Item.Y + DirToDY(D);
      C := GetCost(X, Y, D);
      if (C >= 0) and (Result[Y, X].Distance < 0) then
        NewWave.Add(X, Y, C, D); // 
    end;
  end;

  procedure ExchangeWaves; // 
  var
    W: TWave;
  begin
    W := OldWave;
    OldWave := NewWave;
    NewWave := W;
    NewWave.Clear;
  end;

begin
  PreparePathMap; // 初始化PathMapArray ,Distance:=-1
  OldWave := TWave.Create;
  NewWave := TWave.Create;
  Result[Y1, X1].Distance := 0; // 起点Distance:=0
  OldWave.Add(X1, Y1, 0, 0); // 将起点加入OldWave
  TestNeighbours; // 

  Finished := ((X1 = X2) and (Y1 = Y2)); // 检验是否到达终点
  while not Finished do
  begin
    ExchangeWaves; // 
    if not OldWave.Start then
      Break;
    repeat
      I := OldWave.Item;
      I.Cost := I.Cost - OldWave.MinCost; // 如果大于MinCost
      if I.Cost > 0 then // 加入NewWave
        NewWave.Add(I.X, I.Y, I.Cost, I.Direction) // 更新Cost= cost-MinCost
      else
      begin // 处理最小COST的点
        if Result[I.Y, I.X].Distance >= 0 then
          Continue;

        Result[I.Y, I.X].Distance := Result[I.Y - DirToDY(I.Direction), I.X -
          DirToDX(I.Direction)].Distance + 1; // 此点 Distance:=上一个点Distance+1

        Result[I.Y, I.X].Direction := I.Direction;
          // 
        Finished := ((I.X = X2) and (I.Y = Y2)); // 检验是否到达终点
        if Finished then
          Break;
        TestNeighbours;
      end;
    until not OldWave.Next; // 
  end; // OldWave;
  NewWave.Free;
  OldWave.Free;
end;

function TPathMap.GetCost(X, Y, Direction: Integer): Integer;
begin
  Direction := (Direction and 7);
  if (X < 0) or (X >= MapWidth) or (Y < 0) or (Y >= MapHeight) then
    Result := -1
  else
    Result := GetCostFunc(X, Y, Direction);
end;


function TLegendMap.LoadMap(MapFile: string): Boolean;
var
  aMapFile: TFileStream;
  I, j: Integer;
  hh: widestring;
begin
  aMapFile := TFileStream.Create(MapFile, fmOpenRead);
  try
    aMapFile.Read(MapHeader, SizeOf(TMapHeader)); // 
    SetLength(MapBuf, MapHeader.width, MapHeader.height);
    for I := 0 to MapHeader.width - 1 do
      for j := 0 to MapHeader.height - 1 do
        aMapFile.Read(MapBuf[I, j], SizeOf(MapBuf[I, j]));
  except
    aMapFile.Free;
    Result := false;
  end;
  aMapFile.Free;

  MapWidth := MapHeader.Width;
  MapHeight := MapHeader.Height;
// Title:=copy(MapHeader.Title,2,16);
  Title := MapHeader.Title + FormatDateTime('yy-mm-dd', MapHeader.UpdateDate);

 // 将MapBuf转为算法使用的地图格式MapData
  SetLength(MapData, MapWidth, MapHeight);
  for I := 0 to MapWidth - 1 do
    for j := 0 to MapHeight - 1 do
    begin
      if ((MapBuf[I, j].BkImg and $8000) = 0) then
        MapData[I, j].TerrainType := ttNormal // 标识为平地
      else
        MapData[I, j].TerrainType := ttObstacle; // 标识为障碍物
    end;
  Result := true;
end;

function TLegendMap.FindPath(StopX, StopY: Integer): TPath;
begin
  Result := FindPathOnMap(StopX, StopY);
end;

function TLegendMap.FindPath(StartX, StartY, StopX, StopY: Integer): TPath;
begin
  PathMapArray := FillPathMap(StartX, StartY, StopX, StopY);
  Result := FindPathOnMap(StopX, StopY);
end;

procedure TLegendMap.SetStartPos(StartX, StartY: Integer);
begin
  PathMapArray := FillPathMap(StartX, StartY, -1, -1);
end;

function TLegendMap.XYToCurrXY(X, Y: Integer; var nX, nY: Integer): Boolean;
begin
  nX := X - FHX;
  nY := Y - FHY;
  Result := True;
  if nX >= MapWidth then begin
    nX := MapWidth - 1;
    Result := False;
  end;
  if nY >= MapHeight then begin
    nY := MapHeight - 1;
    Result := False;
  end;
  if nX < 0 then begin
    nX := 0;
    Result := False;
  end;
  if nY < 0 then begin
    nY := 0;
    Result := False;
  end;
end;

function TLegendMap.GetCost(X, Y, Direction: Integer): Integer;
var
  cost: Integer;
begin
  Direction := (Direction and 7);
  if (X < 0) or (X >= MapWidth) or (Y < 0) or (Y >= MapHeight) then
    Result := -1
  else
  begin
    if PlayScene.CanWalkEx(X, Y) then Result := 4 else Result := -1;
    if (X < MapWidth - PathWidth) and (X > PathWidth) and
      (Y < MapHeight - PathWidth) and (Y > PathWidth) then
    begin
      cost := TerrainParams[MapData[X - PathWidth, Y].TerrainType].MoveCost
        + TerrainParams[MapData[X + PathWidth, Y].TerrainType].MoveCost
        + TerrainParams[MapData[X, Y - PathWidth].TerrainType].MoveCost
        + TerrainParams[MapData[X, Y + PathWidth].TerrainType].MoveCost;
      if cost < 4 * TerrainParams[ttNormal].MoveCost then
        Result := -1;
    end;
    {Result := TerrainParams[MapData[X, Y].TerrainType].MoveCost;
    if (X < MapWidth - PathWidth) and (X > PathWidth) and
      (Y < MapHeight - PathWidth) and (Y > PathWidth) then
    begin
      cost := TerrainParams[MapData[X - PathWidth, Y].TerrainType].MoveCost
        + TerrainParams[MapData[X + PathWidth, Y].TerrainType].MoveCost
        + TerrainParams[MapData[X, Y - PathWidth].TerrainType].MoveCost
        + TerrainParams[MapData[X, Y + PathWidth].TerrainType].MoveCost;
      if cost < 4 * TerrainParams[ttNormal].MoveCost then
        Result := -1;
    end; }

    if ((Direction and 1) = 1) and (Result > 0) then // 如果是斜方向,则COST增加
      Result := Result + (Result shr 1); // 应为Result*sqt(2),此处近似为1.5
  end;

end;

end.

