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
  Windows, Classes, SysUtils, Graphics, Math, UnitPath, Envir, ObjBase, ObjPlayer,
  M2Locker, M2Threads, Grobal2, System.Types;

type

  TPathMap = class                                                                                  // 寻路类
  public
    PathMapArray: TPathMapArray;
    Height: Integer;
    Width: Integer;
    GetCostFunc: TGetCostFunc;
    ClientRect: TRect;
    ScopeValue: Integer;                                                                            // 寻找范围
    StartFind: Boolean;
    constructor Create;
    procedure GetClientRect;
    function FindPathOnMap(X, Y: Integer; Run: Boolean): TPath;
    function WalkToRun(Path: TPath): TPath;                                                         // 把WALK合并成RUN
    function MapX(X: Integer): Integer;
    function MapY(Y: Integer): Integer;
    function LoaclX(X: Integer): Integer;
    function LoaclY(Y: Integer): Integer;
  private
    function DirToDX(Direction: Integer): Integer;
    function DirToDY(Direction: Integer): Integer;

  protected
    function GetCost(X, Y, Direction: Integer; boFlag: Boolean): Integer; virtual;
    function FillPathMap(X1, Y1, X2, Y2: Integer; boFlag: Boolean; MinRange: Boolean): TPathMapArray;
  end;

  TFindPath = class(TPathMap)                                                                       // 传奇地图读取及寻路类
  private
    FEnvir: TEnvirnoment;
    FBaseObject: TBaseObject;
    FCriticalSection: TM2CriticalSection;
  public
    Title: string;
    BeginX, BeginY, EndX, EndY: Integer;
    constructor Create;
    destructor Destroy; override;
    function FindPath1(Envir: TEnvirnoment; StartX, StartY, StopX, StopY: Integer; Run: Boolean; boFlag: Boolean; MinRange: Boolean = True): TPath;
    function FindPath2(StopX, StopY: Integer; Run: Boolean; boFlag: Boolean; MinRange: Boolean = True): TPath;
    function FindPath3(Envir: TEnvirnoment; StartX, StartY, StopX, StopY: Integer; boFlag: Boolean; MinRange: Boolean = True): Boolean;
    procedure SetStartPos(StartX, StartY: Integer);
    procedure Stop;

    property Envir: TEnvirnoment read FEnvir write FEnvir;
    property BaseObject: TBaseObject read FBaseObject write FBaseObject;
  protected
    function GetCost(X, Y, Direction: Integer; boFlag: Boolean): Integer; override;
  end;

  TWaveCell = record                                                                                // 路线点
    X, Y: Integer;                                                                                  //
    Cost: Integer;                                                                                  //
    Direction: Integer;
  end;

  TWave = class                                                                                     // 路线类
  private
    FData: array of TWaveCell;
    FPos: Integer;                                                                                  //
    FCount: Integer;                                                                                //
    FMinCost: Integer;
    function GetItem: TWaveCell;
  public
    property Item: TWaveCell read GetItem;                                                          //
    property MinCost: Integer read FMinCost;                                                        // Cost

    constructor Create;
    destructor Destroy; override;
    procedure Add(NewX, NewY, NewCost, NewDirection: Integer);                                      //
    procedure Clear;                                                                                // FCount
    function Start: Boolean;                                                                        //
    function Next: Boolean;                                                                         //
  end;



implementation

uses M2Share;

constructor TWave.Create;
begin
  Clear;                                                                                            //
end;

destructor TWave.Destroy;
begin
  FData := nil;                                                                                     //
  inherited Destroy;
end;

function TWave.GetItem: TWaveCell;
begin
  Result := FData[FPos];                                                                            //
end;

procedure TWave.Add(NewX, NewY, NewCost, NewDirection: Integer);
begin
  if FCount >= Length(FData) then                                                                   //
    SetLength(FData, Length(FData) + 30);                                                           //
  with FData[FCount] do
  begin
    X := NewX;
    Y := NewY;
    Cost := NewCost;
    Direction := NewDirection;
  end;
  if NewCost < FMinCost then                                                                        // NewCost
    FMinCost := NewCost;
  Inc(FCount);                                                                                      //
end;

procedure TWave.Clear;
begin
  FPos := 0;
  FCount := 0;
  FData := nil;
  FMinCost := High(Integer);
end;

function TWave.Start: Boolean;
begin
  FPos := 0;                                                                                        //
  Result := (FCount > 0);                                                                           //
end;

function TWave.Next: Boolean;
begin
  Inc(FPos);                                                                                        //
  Result := (FPos < FCount);                                                                        // false,
end;

{------------------------------------------------------------------------------}

constructor TPathMap.Create;
begin
  inherited;
  ScopeValue := 24 * 4;                                                                             // 寻路范围
  GetCostFunc := nil;
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
// 从TPathMap中找出 TPath
// *************************************************************


function TPathMap.FindPathOnMap(X, Y: Integer; Run: Boolean): TPath;
var
  I: Integer;
  nX, nY: Integer;
  Direction: Integer;
  nCount: Integer;
begin
  Result := nil;
  nCount := 0;

  nX := LoaclX(X);
  nY := LoaclY(Y);
  if (nX < 0) or (nY < 0) or
    (nX >= ClientRect.Right - ClientRect.Left) or (nY >= ClientRect.Bottom - ClientRect.Top)
    then
  begin
    SetLength(PathMapArray, 0, 0);
    PathMapArray := nil;
    Exit;
  end;

  if (Length(PathMapArray) <= 0) or (PathMapArray[nY, nX].Distance < 0) then
  begin
    SetLength(PathMapArray, 0, 0);
    PathMapArray := nil;
    Exit;
  end;

  SetLength(Result, PathMapArray[nY, nX].Distance + 1);
  while PathMapArray[nY, nX].Distance > 0 do
  begin
    if nCount >= Length(Result) * 2 then Break;
    if not StartFind then Break;
    Result[PathMapArray[nY, nX].Distance] := Point(nX, nY);
    Direction := PathMapArray[nY, nX].Direction;
    nX := nX - DirToDX(Direction);
    nY := nY - DirToDY(Direction);
    Inc(nCount);
  end;

  SetLength(PathMapArray, 0, 0);
  PathMapArray := nil;

  Result[0] := Point(nX, nY);
  for I := 0 to Length(Result) - 1 do
    Result[I] := Point(MapX(Result[I].X), MapY(Result[I].Y));

  if Run then
    Result := WalkToRun(Result);
end;

function TPathMap.WalkToRun(Path: TPath): TPath;                                                    // 把WALK合并成RUN

  function GetNextDirection(sx, sy, dx, dy: Integer): Byte;
  var
    flagx, flagy: Integer;
  const
    DR_UP = 0;
    DR_UPRIGHT = 1;
    DR_RIGHT = 2;
    DR_DOWNRIGHT = 3;
    DR_DOWN = 4;
    DR_DOWNLEFT = 5;
    DR_LEFT = 6;
    DR_UPLEFT = 7;
  begin
    Result := DR_DOWN;
    if sx < dx then
      flagx := 1
    else if sx = dx then
      flagx := 0
    else
      flagx := -1;
    if abs(sy - dy) > 2
      then
      if (sx >= dx - 1) and (sx <= dx + 1) then flagx := 0;

    if sy < dy then
      flagy := 1
    else if sy = dy then
      flagy := 0
    else
      flagy := -1;
    if abs(sx - dx) > 2 then
      if (sy > dy - 1) and (sy <= dy + 1) then flagy := 0;

    if (flagx = 0) and (flagy = -1) then Result := DR_UP;
    if (flagx = 1) and (flagy = -1) then Result := DR_UPRIGHT;
    if (flagx = 1) and (flagy = 0) then Result := DR_RIGHT;
    if (flagx = 1) and (flagy = 1) then Result := DR_DOWNRIGHT;
    if (flagx = 0) and (flagy = 1) then Result := DR_DOWN;
    if (flagx = -1) and (flagy = 1) then Result := DR_DOWNLEFT;
    if (flagx = -1) and (flagy = 0) then Result := DR_LEFT;
    if (flagx = -1) and (flagy = -1) then Result := DR_UPLEFT;
  end;
var
  nDir1, nDir2: Integer;
  I, n01: Integer;
  WalkPath: TPath;
  nStep: Integer;
begin
  Result := nil;
  WalkPath := nil;
  if (Path <> nil) and (Length(Path) > 1) then
  begin
    SetLength(WalkPath, Length(Path));

    for I := 0 to Length(Path) - 1 do
      WalkPath[I] := Path[I];

    // WalkPath := Path;
    nStep := 0;
    I := 0;

    while True do
    begin
      if not StartFind then Break;
      if I >= Length(WalkPath) then Break;
      if nStep >= 2 then
      begin
        nDir1 := GetNextDirection(WalkPath[I - 2].x, WalkPath[I - 2].y, WalkPath[I - 1].x, WalkPath[I - 1].y);
        nDir2 := GetNextDirection(WalkPath[I - 1].x, WalkPath[I - 1].y, WalkPath[I].x, WalkPath[I].y);
        if nDir1 = nDir2 then
        begin
          WalkPath[I - 1].x := -1;
          WalkPath[I - 1].y := -1;
          nStep := 0;
        end
        else
        begin                                                                                     // 需要转向不能合并
          Dec(I);
          nStep := 0;
          Continue;
        end;
      end;
      Inc(nStep);
      Inc(I);
    end;

    n01 := 0;
    for I := 1 to Length(WalkPath) - 1 do
    begin
      if (WalkPath[I].x <> -1) and (WalkPath[I].y <> -1) then
      begin
        Inc(n01);
        SetLength(Result, n01);
        Result[n01 - 1] := WalkPath[I];
      end;
    end;
    Exit;
  end;
  if (Path <> nil) and (Length(Path) > 0) then
  begin
    SetLength(Result, Length(Path) - 1);
    for I := 1 to Length(Path) - 1 do
      Result[I - 1] := Path[I];
  end
  else
  begin
    SetLength(Result, 0);
    Result := nil;
  end;
end;

// *************************************************************
// 寻路算法
// X1,Y1为路径运算起点，X2，Y2为路径运算终点
// *************************************************************


function TPathMap.MapX(X: Integer): Integer;
begin
  Result := X + ClientRect.Left;
end;

function TPathMap.MapY(Y: Integer): Integer;
begin
  Result := Y + ClientRect.Top;
end;

function TPathMap.LoaclX(X: Integer): Integer;
begin
  Result := X - ClientRect.Left;
end;

function TPathMap.LoaclY(Y: Integer): Integer;
begin
  Result := Y - ClientRect.Top;
end;

procedure TPathMap.GetClientRect;
begin
  ClientRect := Bounds(0, 0, Width, Height);
end;

function TPathMap.FillPathMap(X1, Y1, X2, Y2: Integer; boFlag: Boolean; MinRange: Boolean): TPathMapArray;
var
  OldWave, NewWave: TWave;
  Finished: Boolean;
  I: TWaveCell;
  nX1, nY1, nX2, nY2: Integer;
  nCount: Integer;
  nLoopCount: Integer;

  procedure PreparePathMap;                                                                         // 初始化PathMapArray
  var
    X, Y: Integer;                                                                                  //
  begin
    SetLength(Result, ClientRect.Bottom - ClientRect.Top, ClientRect.Right - ClientRect.Left);
    for Y := 0 to (ClientRect.Bottom - ClientRect.Top - 1) do
    begin
      for X := 0 to (ClientRect.Right - ClientRect.Left - 1) do
      begin
        Result[Y, X].Distance := -1;
      end;
    end;
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
      C := GetCost(X, Y, D, boFlag);
      if (C >= 0) and (Result[Y, X].Distance < 0) then
        NewWave.Add(X, Y, C, D);                                                                    //
    end;
  end;

  procedure ExchangeWaves;                                                                          //
  var
    W: TWave;
  begin
    W := OldWave;
    OldWave := NewWave;
    NewWave := W;
    NewWave.Clear;
  end;
begin
  GetClientRect;

  nX1 := LoaclX(X1);
  nY1 := LoaclY(Y1);
  nX2 := LoaclX(X2);
  nY2 := LoaclY(Y2);

  if X2 < 0 then nX2 := X2;
  if Y2 < 0 then nY2 := Y2;

  if (X2 >= 0) and (Y2 >= 0) then
  begin
    if (abs(nX1 - nX2) > (ClientRect.Right - ClientRect.Left)) or
      (abs(nY1 - nY2) > (ClientRect.Bottom - ClientRect.Top)) then
    begin
      SetLength(Result, 0, 0);
      Exit;
    end;
  end;

  PreparePathMap;                                                                                   // 初始化PathMapArray ,Distance:=-1

  if (Result = nil) or (Length(Result) = 0) then Exit;

  OldWave := TWave.Create;
  NewWave := TWave.Create;

  if nX1 >= Length(Result) then
  begin
    SetLength(Result, 0, 0);
    Exit;
  end
  else if (Length(Result) > 0) and (nY1 >= Length(Result[0])) then
  begin
    SetLength(Result, 0, 0);
    Exit;
  end;

  Result[nY1, nX1].Distance := 0;                                                                   // 起点Distance:=0
  OldWave.Add(nX1, nY1, 0, 0);                                                                      // 将起点加入OldWave
  TestNeighbours;                                                                                   //

  nCount := 0;
  Finished := ((nX1 = nX2) and (nY1 = nY2));                                                        // 检验是否到达终点
  while not Finished do
  begin
    Inc(nCount);
    if MinRange then
    begin
      if (nCount >= 100) then Break;
    end
    else
    begin
      if (nCount >= 2000) then Break;
    end;

    ExchangeWaves;                                                                                  //
    nLoopCount := 0;

    if not StartFind then Break;
    if not OldWave.Start then
      Break;
    repeat
      Inc(nLoopCount);
      if MinRange then
      begin
        if (nLoopCount >= 300) then Break;
      end
      else
      begin
        if (nLoopCount >= 2000) then Break;
      end;
      if not StartFind then Break;
      I := OldWave.Item;
      I.Cost := I.Cost - OldWave.MinCost;                                                           // 如果大于MinCost
      if I.Cost > 0 then                                                                            // 加入NewWave
        NewWave.Add(I.X, I.Y, I.Cost, I.Direction)                                                  // 更新Cost= cost-MinCost
      else
      begin                                                                                         // 处理最小COST的点
        if Result[I.Y, I.X].Distance >= 0 then
          Continue;

        Result[I.Y, I.X].Distance := Result[I.Y - DirToDY(I.Direction), I.X -
          DirToDX(I.Direction)].Distance + 1;                                                       // 此点 Distance:=上一个点Distance+1

        Result[I.Y, I.X].Direction := I.Direction;
          //
        Finished := ((I.X = nX2) and (I.Y = nY2));                                                  // 检验是否到达终点
        if Finished then
          Break;
        TestNeighbours;
      end;
    until not OldWave.Next;                                                                         //
  end;                                                                                              // OldWave;
  NewWave.Free;
  OldWave.Free;
end;

function TPathMap.GetCost(X, Y, Direction: Integer; boFlag: Boolean): Integer;
begin
  Result := 0;
end;

constructor TFindPath.Create;
begin
  inherited;
  FEnvir := nil;
  FBaseObject := nil;
  StartFind := False;
  FCriticalSection := TM2CriticalSection.Create('TFindPath.FCriticalSection');
end;

procedure TFindPath.Stop;
begin
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then FCriticalSection.LockW(3);
  try
{$IFEND}

    StartFind := False;
    BeginX := -1;
    BeginY := -1;
    EndX := -1;
    EndY := -1;
    SetLength(PathMapArray, 0, 0);
    PathMapArray := nil;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then FCriticalSection.UnLockW;
  end;
{$IFEND}
end;

function TFindPath.FindPath2(StopX, StopY: Integer; Run: Boolean; boFlag: Boolean; MinRange: Boolean): TPath;
begin
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then FCriticalSection.LockW(1);
  try
{$IFEND}
    EndX := StopX;
    EndY := StopY;
    Result := FindPathOnMap(StopX, StopY, Run);
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then FCriticalSection.UnLockW;
  end;
{$IFEND}
end;

function TFindPath.FindPath1(Envir: TEnvirnoment; StartX, StartY, StopX, StopY: Integer; Run: Boolean; boFlag: Boolean; MinRange: Boolean): TPath;
begin
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then FCriticalSection.LockW(2);
  try
{$IFEND}
    if Envir <> nil then
    begin
      FEnvir := Envir;
      Width := Envir.m_nWidth;
      Height := Envir.m_nHeight;
      BeginX := StartX;
      BeginY := StartY;
      EndX := StopX;
      EndY := StopY;
      StartFind := True;
      PathMapArray := FillPathMap(StartX, StartY, StopX, StopY, boFlag, MinRange);
      Result := FindPathOnMap(StopX, StopY, Run);
    end
    else
      Result := nil;
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then FCriticalSection.UnLockW;
  end;
{$IFEND}
end;

function TFindPath.FindPath3(Envir: TEnvirnoment; StartX, StartY, StopX, StopY: Integer; boFlag: Boolean; MinRange: Boolean): Boolean;
var
  Path: TPath;
begin
  Path := FindPath1(Envir, StartX, StartY, StopX, StopY, False, boFlag, MinRange);
  Result := Length(Path) > 0;
  SetLength(Path, 0);
end;

procedure TFindPath.SetStartPos(StartX, StartY: Integer);
begin
{$IF MULTI_THREAD = 1}
  if g_MultiThreadRun then FCriticalSection.LockW(5);
  try
{$IFEND}
    BeginX := StartX;
    BeginY := StartY;
    PathMapArray := FillPathMap(StartX, StartY, -1, -1, False, False);
{$IF MULTI_THREAD = 1}
  finally
    if g_MultiThreadRun then FCriticalSection.UnLockW;
  end;
{$IFEND}
end;

function TFindPath.GetCost(X, Y, Direction: Integer; boFlag: Boolean): Integer;
var
  // cost: Integer;
  nX, nY: Integer;
begin
  if FEnvir <> nil then
  begin
    Direction := (Direction and 7);
    if (X < 0) or (X >= ClientRect.Right - ClientRect.Left) or (Y < 0) or (Y >= ClientRect.Bottom - ClientRect.Top) then
      Result := -1
    else                                 
    begin
      nX := MapX(X);
      nY := MapY(Y);

      if boFlag then
      begin
        if FEnvir.CanWalkEx2(FBaseObject, nX, nY, boFlag) then
          Result := 4
        else
          Result := -1;
      end
      else
      begin
        if FBaseObject.m_btRaceServer = RC_HEROOBJECT then
        begin
          if FEnvir.CanWalkEx(FBaseObject, nX, nY, g_Config.boDiableHeroRun) or (g_Config.boHeroSafeAreaLimited and TBaseObject(FBaseObject).InSafeZone) then
            Result := 4
          else
            Result := -1;
        end
        else if (FBaseObject.m_btRaceServer = RC_PLAYOBJECT) and (TPlayObject(FBaseObject).m_boDummyObject) then
        begin
          if FEnvir.CanWalkEx(FBaseObject, nX, nY, g_Config.boDiableDummyRun) or (g_Config.boDummySafeAreaLimited and TBaseObject(FBaseObject).InSafeZone) then
            Result := 4
          else
            Result := -1;
        end
        else if (FBaseObject.m_btRaceServer = RC_PLAYOBJECT) then
        begin
          if FEnvir.CanWalkEx(FBaseObject, nX, nY, g_Config.boDiableHumanRun or ((TBaseObject(FBaseObject).m_btPermission > 9) and g_Config.boGMRunAll)) or (g_Config.boSafeAreaLimited and TBaseObject(FBaseObject).InSafeZone) then
            Result := 4
          else
            Result := -1;
        end
        else
        begin
          if FEnvir.CanWalkEx(FBaseObject, nX, nY, False) then
            Result := 4
          else
            Result := -1;
        end;
      end;
      if ((Direction and 1) = 1) and (Result > 0) then                                              // 如果是斜方向,则COST增加
        Result := Result + (Result shr 1);                                                          // 应为Result*sqt(2),此处近似为1.5
    end;
  end
  else
    Result := -1;
end;

destructor TFindPath.Destroy;
begin
  FCriticalSection.Free;
  inherited;
end;

end.
