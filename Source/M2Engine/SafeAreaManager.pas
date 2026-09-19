unit SafeAreaManager;

interface

uses
  Windows, Classes, SysUtils;

type
  TSafeArea = class(TObject)
  private
    FMapName: string;
  protected
    function DoInSafeArea(nX, nY: Integer): Boolean; virtual; abstract; // 检测是否在安全区
    function DoGetCenterX: Integer; virtual; abstract; // 获取一个中间点
    function DoGetCenterY: Integer; virtual; abstract;
    procedure DoRecall; virtual; abstract;
  public
    property MapName: string read FMapName write FMapName;
    function InSafeArea(nX, nY: Integer): Boolean;
    function GetCenterX: Integer;
    function GetCenterY: Integer;
    procedure Recall;
  end;

  TRangeSafeArea = class(TSafeArea)
  private
    FCenterX: Integer;
    FCenterY: Integer;
    FRange: Integer;
    FShowType: Integer;
    FIsDisableSay: Boolean;
    FIsPKZone: Boolean;
    FIsPKFire: Boolean;

    FArea: TRect;
  protected
    function DoInSafeArea(nX: Integer; nY: Integer): Boolean; override; // 检测是否在安全区
    function DoGetCenterX: Integer; override; // 获取一个中间点
    function DoGetCenterY: Integer; override;
    procedure DoRecall; override;
  public
    property CenterX: Integer read FCenterX write FCenterX;
    property CenterY: Integer read FCenterY write FCenterY;
    property Range: Integer read FRange write FRange;
    property ShowType: Integer read FShowType write FShowType;
    property IsDisableSay: Boolean read FIsDisableSay write FIsDisableSay;
    property IsPKZone: Boolean read FIsPKZone write FIsPKZone;
    property IsPKFire: Boolean read FIsPKFire write FIsPKFire;
  end;

  PAllotypePoint = ^TAllotypePoint;

  TAllotypePoint = record
    PointX: Integer;
    ShowType: Integer;
    Dir: Integer;
  end;

  TAllotypeRow = class(TObject)
  private
    FPointY: Integer;
    FPoints: TList;
    function GetCount: Integer;
    function GetPoints(Index: Integer): PAllotypePoint;
    function Search(PointX: Integer; var Index: Integer): Boolean;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Add(PointX, ShowType, Dir: Integer);
    procedure Clear;
    function PointInRow(PointX: Integer): Boolean;
    property Count: Integer read GetCount;
    property Points[Index: Integer]: PAllotypePoint read GetPoints;
    property PointY: Integer read FPointY;
  end;

  TAllotypeSafeArea = class(TSafeArea)
  private
    FID: Integer;
    FRows: TList;

    FCenterX, FCenterY: Integer;
    FArea: TRect;

    function GetCount: Integer;
    function GetRows(Index: Integer): TAllotypeRow;
    function Search(PointY: Integer; var Index: Integer): Boolean;
  protected
    function DoInSafeArea(nX, nY: Integer): Boolean; override; // 检测是否在安全区
    function DoGetCenterX: Integer; override; // 获取一个中间点
    function DoGetCenterY: Integer; override;
    procedure DoRecall; override;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Add(PointX, PointY, ShowType, Dir: Integer);
    function GetRow(PointY: Integer): TAllotypeRow;
    procedure Clear;
    property ID: Integer read FID write FID;
    property Count: Integer read GetCount;
    property Rows[Index: Integer]: TAllotypeRow read GetRows;
  end;

  TSafeAreaManager = class(TObject)
  private
    FList: TList;
    function GetCount: Integer;
    function GetItems(Index: Integer): TSafeArea;
  public
    constructor Create;
    destructor Destroy; override;

    procedure Recall;

    procedure Clear;
    procedure Add(SafeArea: TSafeArea);

    property Count: Integer read GetCount;
    property Items[Index: Integer]: TSafeArea read GetItems; default;

    function MapSafeArea(MapName: string): TSafeArea;
    function PointInSafeArea(MapName: string; nX, nY: Integer): Boolean;

    function GetAllotypeSafeArea(MapName: string; ID: Integer): TAllotypeSafeArea;
  end;

implementation

{ TSafeArea }

function TSafeArea.GetCenterX: Integer;
begin
  Result := DoGetCenterX;
end;

function TSafeArea.GetCenterY: Integer;
begin
  Result := DoGetCenterY;
end;

function TSafeArea.InSafeArea(nX, nY: Integer): Boolean;
begin
  Result := DoInSafeArea(nX, nY);
end;

procedure TSafeArea.Recall;
begin
  DoRecall;
end;

{ TRangeSafeArea }

function TRangeSafeArea.DoGetCenterX: Integer;
begin
  Result := FCenterX;
end;

function TRangeSafeArea.DoGetCenterY: Integer;
begin
  Result := FCenterY;
end;

function TRangeSafeArea.DoInSafeArea(nX, nY: Integer): Boolean;
begin
  Result := (nX >= FArea.Left) and (nX <= FArea.Right) and (nY >= FArea.Top) and (nY <= FArea.Bottom);
end;

procedure TRangeSafeArea.DoRecall;
begin
  FArea := Rect(FCenterX - FRange, FCenterY - FRange, FCenterX + FRange, FCenterY + FRange);
end;

{ TAllotypeRow }

constructor TAllotypeRow.Create;
begin
  FPoints := TList.Create;
end;

destructor TAllotypeRow.Destroy;
begin
  Clear;
  FPoints.Free;
  inherited;
end;

procedure TAllotypeRow.Add(PointX, ShowType, Dir: Integer);
var
  I: Integer;
  Point: PAllotypePoint;
begin
  if not Search(PointX, I) then
  begin
    New(Point);
    Point.PointX := PointX;
    Point.ShowType := ShowType;
    Point.Dir := Dir;

    FPoints.Insert(I, Point);
  end
  else
  begin
    Point := FPoints.Items[I];
    Point.ShowType := ShowType;
    Point.Dir := Dir;
  end;
end;

procedure TAllotypeRow.Clear;
var
  I: Integer;
  Point: PAllotypePoint;
begin
  for I := 0 to FPoints.Count - 1 do
  begin
    Point := FPoints.Items[I];
    Dispose(Point);
  end;
  FPoints.Clear;
end;

function TAllotypeRow.GetCount: Integer;
begin
  Result := FPoints.Count;
end;

function TAllotypeRow.GetPoints(Index: Integer): PAllotypePoint;
begin
  Result := FPoints.Items[Index];
end;

function TAllotypeRow.Search(PointX: Integer; var Index: Integer): Boolean;
var
  L, H, C, I: Integer;
  Point: PAllotypePoint;
begin
  Result := False;

  L := 0;
  H := FPoints.Count - 1;
  while L <= H do
  begin
    I := (L + H) shr 1;
    Point := FPoints.Items[I];
    C := Point.PointX - PointX;
    if C < 0 then
      L := I + 1
    else
    begin
      H := I - 1;
      if C = 0 then
      begin
        Result := True;
        L := I;
      end;
    end;
  end;
  Index := L;
end;

function TAllotypeRow.PointInRow(PointX: Integer): Boolean;
var
  Point1, Point2: PAllotypePoint;
begin
  Result := False;
  if FPoints.Count > 0 then
  begin
    Point1 := FPoints.Items[0];
    Point2 := FPoints.Items[FPoints.Count - 1];
    Result := (PointX >= Point1.PointX) and (PointX <= Point2.PointX);
  end;
end;

{ TAllotypeSafeArea }

constructor TAllotypeSafeArea.Create;
begin
  FRows := TList.Create;
end;

destructor TAllotypeSafeArea.Destroy;
begin
  Clear;
  FRows.Free;
  inherited;
end;

procedure TAllotypeSafeArea.Add(PointX, PointY, ShowType, Dir: Integer);
var
  I: Integer;
  Row: TAllotypeRow;
begin
  if not Search(PointY, I) then
  begin
    Row := TAllotypeRow.Create;
    Row.FPointY := PointY;
    Row.Add(PointX, ShowType, Dir);
    FRows.Insert(I, Row)
  end
  else
  begin
    Row := FRows.Items[I];
    Row.Add(PointX, ShowType, Dir);
  end;
end;

procedure TAllotypeSafeArea.Clear;
var
  I: Integer;
  Row: TAllotypeRow;
begin
  for I := 0 to FRows.Count - 1 do
  begin
    Row := FRows.Items[I];
    Row.Free;
  end;
  FRows.Clear;
end;

function TAllotypeSafeArea.GetCount: Integer;
begin
  Result := FRows.Count;
end;

function TAllotypeSafeArea.GetRow(PointY: Integer): TAllotypeRow;
var
  I: Integer;
begin
  if Search(PointY, I) then
    Result := FRows.Items[I]
  else
    Result := nil;
end;

function TAllotypeSafeArea.GetRows(Index: Integer): TAllotypeRow;
begin
  Result := FRows.Items[Index];
end;

function TAllotypeSafeArea.Search(PointY: Integer; var Index: Integer): Boolean;
var
  L, H, C, I: Integer;
  Row: TAllotypeRow;
begin
  Result := False;

  L := 0;
  H := FRows.Count - 1;
  while L <= H do
  begin
    I := (L + H) shr 1;
    Row := FRows.Items[I];
    C := Integer(Row.FPointY) - Integer(PointY);
    if C < 0 then
      L := I + 1
    else
    begin
      H := I - 1;
      if C = 0 then
      begin
        Result := True;
        L := I;
      end;
    end;
  end;
  Index := L;
end;

function TAllotypeSafeArea.DoGetCenterX: Integer;
begin
  Result := FCenterX;
end;

function TAllotypeSafeArea.DoGetCenterY: Integer;
begin
  Result := FCenterY;
end;

function TAllotypeSafeArea.DoInSafeArea(nX, nY: Integer): Boolean;
var
  Row: TAllotypeRow;
begin
  Result := (nX >= FArea.Left) and (nX <= FArea.Right) and (nY >= FArea.Top) and (nY <= FArea.Bottom);
  if not Result then
    Exit;

  Row := GetRow(nY);
  if Row = nil then
  begin
    Result := False;
    Exit;
  end;

  Result := Row.PointInRow(nX);
end;

procedure TAllotypeSafeArea.DoRecall;
var
  I, II: Integer;
  Row: TAllotypeRow;
  APoint: PAllotypePoint;
begin
  if FRows.Count > 0 then
  begin
    I := FRows.Count shr 1;
    Row := FRows.Items[I];

    FCenterY := Row.FPointY;
    if Row.Count = 1 then
    begin
      FCenterX := PAllotypePoint(Row.FPoints[0]).PointX;
    end
    else if Row.Count >= 2 then
    begin
      FCenterX := (PAllotypePoint(Row.FPoints[0]).PointX + PAllotypePoint(Row.FPoints[Row.FPoints.Count - 1]).PointX) shr 1;
    end;

    FArea.Left := High(Integer);
    FArea.Top := High(Integer);
    FArea.Right := -1;
    FArea.Bottom := -1;

    for I := 0 to FRows.Count - 1 do
    begin
      Row := FRows.Items[I];

      if Row.FPointY < FArea.Top then
        FArea.Top := Row.FPointY;

      if Row.FPointY > FArea.Bottom then
        FArea.Bottom := Row.FPointY;

      for II := 0 to Row.Count - 1 do
      begin
        APoint := Row.GetPoints(II);

        if APoint.PointX < FArea.Left then
          FArea.Left := APoint.PointX;

        if APoint.PointX > FArea.Right then
          FArea.Right := APoint.PointX;
      end;
    end;

    // OutputDebugString(PChar(IntToStr(FArea.Left)));
  end;
end;

{ TSafeAreaManager }

constructor TSafeAreaManager.Create;
begin
  FList := TList.Create;
end;

procedure TSafeAreaManager.Add(SafeArea: TSafeArea);
begin
  FList.Add(SafeArea);
end;

procedure TSafeAreaManager.Clear;
var
  I: Integer;
  SafeArea: TSafeArea;
begin
  for I := 0 to FList.Count - 1 do
  begin
    SafeArea := FList.Items[I];
    SafeArea.Free;
  end;
  FList.Clear;
end;

destructor TSafeAreaManager.Destroy;
begin
  Clear;
  FList.Free;
  inherited;
end;

function TSafeAreaManager.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TSafeAreaManager.GetItems(Index: Integer): TSafeArea;
begin
  if (Index >= 0) and (Index <= FList.Count - 1) then
    Result := FList.Items[Index]
  else
    Result := nil;
end;

procedure TSafeAreaManager.Recall;
var
  I: Integer;
  SafeArea: TSafeArea;
begin
  for I := 0 to FList.Count - 1 do
  begin
    SafeArea := FList.Items[I];
    SafeArea.Recall;
  end;
end;

function TSafeAreaManager.MapSafeArea(MapName: string): TSafeArea;
var
  I: Integer;
  SafeArea: TSafeArea;
begin
  Result := nil;
  for I := 0 to FList.Count - 1 do
  begin
    SafeArea := FList.Items[I];
    if SameText(SafeArea.MapName, MapName) then
    begin
      Result := SafeArea;
      Exit;
    end;
  end;
end;

function TSafeAreaManager.PointInSafeArea(MapName: string; nX, nY: Integer): Boolean;
var
  I: Integer;
  SafeArea: TSafeArea;
begin
  Result := False;
  for I := 0 to FList.Count - 1 do
  begin
    SafeArea := FList.Items[I];
    if SameText(SafeArea.MapName, MapName) then
    begin
      if SafeArea.InSafeArea(nX, nY) then
      begin
        Result := True;
        Break;
      end;
    end;
  end;
end;

function TSafeAreaManager.GetAllotypeSafeArea(MapName: string; ID: Integer): TAllotypeSafeArea;
var
  I: Integer;
  SafeArea: TSafeArea;
  AllotypeArea: TAllotypeSafeArea;
begin
  Result := nil;
  for I := 0 to FList.Count - 1 do
  begin
    SafeArea := FList.Items[I];
    if SafeArea is TAllotypeSafeArea then
    begin
      AllotypeArea := SafeArea as TAllotypeSafeArea;
      if SameText(AllotypeArea.MapName, MapName) and (AllotypeArea.FID = ID) then
      begin
        Result := AllotypeArea;
        Break;
      end;
    end;
  end;
end;

end.
