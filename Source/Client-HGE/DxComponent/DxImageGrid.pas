unit DxImageGrid;

interface
uses
  Types,
  Classes,
  Controls,
  SysUtils,
  DxControls,
  DxComponents,
  Grids;
type
  TDxImageGrid = class(TDxControl)
  private
    FColCount, FRowCount:Integer;
    FColWidth, FRowHeight:Integer;
    FViewTopLine:Integer;
    SelectCell:TPoint;
    DownPos:TPoint;
    FOnGridSelect:TOnGridSelect;
    FOnGridMouseMove:TOnGridMove;
    FOnGridPaint:TOnGridPaint;

    procedure SetColCount(Value:Integer);
    procedure SetRowCount(Value:Integer);
    procedure SetColWidth(Value:Integer);
    procedure SetRowHeight(Value:Integer);
  protected
    procedure DoResize(var NewRect:TRect); override;
    procedure DoClick(X, Y:Integer); override;
  public
    function InRange(X, Y:Integer):Boolean; override;
    procedure MouseMove(Shift:TShiftState; X, Y:Integer); override;
    procedure MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); override;
    procedure MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); override;
  public
    cx, cy:Integer;
    Col, Row:Integer;
    constructor Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}); override;
    procedure Paint; override;
    procedure ClearSelect;
    property OnGridSelect:TOnGridSelect read FOnGridSelect write FOnGridSelect;
    property OnGridMouseMove:TOnGridMove read FOnGridMouseMove write FOnGridMouseMove;
    property OnGridPaint:TOnGridPaint read FOnGridPaint write FOnGridPaint;

    function GetColRow(X, Y:Integer; var ACol, ARow:Integer):Boolean;
    function GetCellRect(ACol, ARow:Integer):TRect;
  published
    property ColCount:Integer read FColCount write SetColCount;
    property RowCount:Integer read FRowCount write SetRowCount;
    property ColWidth:Integer read FColWidth write SetColWidth;
    property RowHeight:Integer read FRowHeight write SetRowHeight;
    property ViewTopLine:Integer read FViewTopLine write FViewTopLine;
  end;
implementation
{------------------------- TDxImageGrid --------------------------}

constructor TDxImageGrid.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
begin
  inherited Create(AOwner);
  EnableFocus := False;

  MouseEvents := [mbLeft];

  FColCount := 8;
  FRowCount := 5;
  FColWidth := 36;
  FRowHeight := 32;

  Width := FColWidth * FColCount;
  Height := FRowHeight * FRowCount;

  FOnGridSelect := nil;
  FOnGridMouseMove := nil;
  FOnGridPaint := nil;

  SelectCell.X := -1;
  SelectCell.Y := -1;
end;
// ------------------------------------------------------------------------------

procedure TDxImageGrid.SetColCount(Value:Integer); // 设置列
begin
  if FColCount <> Value then begin
    FColCount := Value;
    Width := FColCount * FColWidth;
  end;
end;

procedure TDxImageGrid.SetRowCount(Value:Integer); // 设置组
begin
  if FRowCount <> Value then begin
    FRowCount := Value;
    Height := FRowCount * FRowHeight;
  end;
end;

procedure TDxImageGrid.SetColWidth(Value:Integer); // 设置列宽
begin
  if FColWidth <> Value then begin
    FColWidth := Value;
    Width := FColCount * FColWidth;
  end;
end;

procedure TDxImageGrid.SetRowHeight(Value:Integer); // 设置组高
begin
  if FRowHeight <> Value then begin
    FRowHeight := Value;
    Height := FRowCount * FRowHeight;
  end;
end;

procedure TDxImageGrid.DoResize(var NewRect:TRect);
// var
 // Aux: Integer;
begin
  inherited;
  NewRect.Right := NewRect.Left + FColCount * FColWidth;
  NewRect.Bottom := NewRect.Top + FRowCount * FRowHeight;
  {Aux := NewRect.Right - NewRect.Left;
  if FColCount * FColWidth <> Aux then
    NewRect.Right := NewRect.Left + FColCount * FColWidth;

  Aux := NewRect.Bottom - NewRect.Top;
  if FRowCount * FRowHeight <> Aux then
    NewRect.Bottom := NewRect.Top + FRowCount * FRowHeight;}
end;

function TDxImageGrid.InRange(X, Y:Integer):Boolean;
var
  boInrange:Boolean;
  vRect:TRect;
begin
  if PointInRect(Point(X, Y), VisibleRect) then begin
    boInrange := True;
    if Assigned(OnInRealArea) then begin
      vRect := VirtualRect;
      OnInRealArea(Self, X - vRect.Left, Y - vRect.Top, boInrange);
    end;
    Result := boInrange;
  end
  else
    Result := False;
end;

function TDxImageGrid.GetColRow(X, Y:Integer; var ACol, ARow:Integer):Boolean; // 根据坐标 获取点击的格子
var
  vRect:TRect;
begin
  Result := False;
  if InRange(X, Y) then begin
    vRect := VirtualRect;
    ACol := (X - vRect.Left) div FColWidth;
    ARow := (Y - vRect.Top) div FRowHeight;

    {
    if ARow >= RowCount then
      ARow := RowCount - 1;
    if ACol >= ColCount then
      ACol := ColCount - 1;
    }

    Result := (ARow < RowCount) and (ACol < ColCount);
    ;
  end;
end;

procedure TDxImageGrid.MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
var
  ACol, ARow:Integer;
begin
  if GetColRow(X, Y, ACol, ARow) then begin
    SelectCell.X := ACol;
    SelectCell.Y := ARow;
    DownPos.X := X;
    DownPos.Y := Y;
  end;
  inherited MouseDown(Button, Shift, X, Y);
end;

procedure TDxImageGrid.MouseMove(Shift:TShiftState; X, Y:Integer);
var
  ACol, ARow:Integer;
begin
  if InRange(X, Y) then begin
    if GetColRow(X, Y, ACol, ARow) then begin
      if Assigned(FOnGridMouseMove) then
        FOnGridMouseMove(Self, ACol, ARow, Shift);
    end;
  end;
  inherited MouseMove(Shift, X, Y);
end;

procedure TDxImageGrid.MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
var
  ACol, ARow:Integer;
begin
  inherited MouseUp(Button, Shift, X, Y);
  if GetColRow(X, Y, ACol, ARow) then begin
    if (SelectCell.X = ACol) and (SelectCell.Y = ARow) then begin
      Col := ACol;
      Row := ARow;
      if Assigned(FOnGridSelect) then
        FOnGridSelect(Self, ACol, ARow, Button, Shift);
    end;
  end;
end;

procedure TDxImageGrid.ClearSelect;
begin
  SelectCell.X := -1;
  SelectCell.Y := -1;
end;

procedure TDxImageGrid.DoClick(X, Y:Integer);
begin
  inherited;
  {if GetColRow(X, Y, ACol, ARow) then begin
    if (SelectCell.X = ACol) and (SelectCell.Y = ARow) then begin
      Col := ACol;
      Row := ARow;
      if Assigned(FOnGridSelect) then
        FOnGridSelect(Self, ACol, ARow, []);
    end;
  end;}
end;

function TDxImageGrid.GetCellRect(ACol, ARow:Integer):TRect;
var
  vtRect:TRect;
begin
  vtRect := VirtualRect;
  Result := Rect(vtRect.Left + ACol * FColWidth, vtRect.Top + ARow * FRowHeight, vtRect.Left + (ACol + 1) * FColWidth - 1, vtRect.Top + (ARow + 1) * FRowHeight - 1)
end;

procedure TDxImageGrid.Paint;
var
  I, j:Integer;
  rc:TRect;

  vtRect:TRect;
  vbRect:TRect;
begin
  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := VirtualRect;
  DoPaint();

  if Assigned(FOnGridPaint) then begin
    for I := 0 to FRowCount - 1 do
      for j := 0 to FColCount - 1 do begin
        rc := Rect(vtRect.Left + j * FColWidth, vtRect.Top + I * FRowHeight, vtRect.Left + (j + 1) * FColWidth - 1, vtRect.Top + (I + 1) * FRowHeight - 1);
        if (SelectCell.Y = I) and (SelectCell.X = j) then
          FOnGridPaint(Self, j, I, rc, [gdSelected])
        else
          FOnGridPaint(Self, j, I, rc, []);
      end;
  end;

  if Designing then begin
    for I := 0 to FRowCount - 1 do
      for j := 0 to FColCount - 1 do begin
        rc := Bounds(j * FColWidth + vtRect.Left, I * FRowHeight + vtRect.Top, FColWidth + 1, FRowHeight + 1);
        rc := ShortRect(vtRect, rc);
        FrameRect(rc, vtRect, vbRect, BorderColor.Up.Color);
      end;
  end;

  for I := ControlCount - 1 downto 0 do
    if Control[I].Visible then
      Control[I].Paint;
end;

end.
