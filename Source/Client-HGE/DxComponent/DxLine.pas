unit DxLine;

interface
uses
  Windows,
  Types,
  Classes,
  Controls,
  SysUtils,
  StdCtrls,
  Graphics,
  HGE,
  DxControls,
  DxComponents,
  GameImages;
type
  TDxLine = class(TDxControl)

  private
    FLineStyle:TLineStyle;
    FLineColor:TDxBorderColor;
    procedure SetStyle(Value:TLineStyle);
  public
    function InRange(X, Y:Integer):Boolean; override;
  public
    constructor Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}); override;
    procedure Paint; override;
    destructor Destroy; override;
  published
    property Style:TLineStyle read FLineStyle write SetStyle;
    property LineColor:TDxBorderColor read FLineColor write FLineColor;
  end;
implementation
uses Math,
  HGECanvas;

constructor TDxLine.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
begin
  inherited Create(AOwner);
  AutoSize := False;
  Floating := False;
  DrawBorder := False;
  Width := 200;
  Height := 12;
  FLineStyle := lsNone;
  FLineColor := TDxBorderColor.Create;
  FLineColor.Up.Color := clGray;
  FLineColor.Hot.Color := clGray;
  FLineColor.Down.Color := clGray;
  FLineColor.Disabled.Color := clGray;
end;

destructor TDxLine.Destroy;
begin
  FLineColor.Free;
  inherited;
end;

procedure TDxLine.SetStyle(Value:TLineStyle);
begin
  if FLineStyle <> Value then begin
    {if Designing and (FLineStyle in [lsHorizontal, lsVertical]) and (Value in [lsHorizontal, lsVertical]) then begin
      nSize := Width;
      Width := Height;
      Height := nSize;
    end;}
    FLineStyle := Value;
  end;
end;

function TDxLine.InRange(X, Y:Integer):Boolean;
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

procedure TDxLine.Paint;
var
  vtRect:TRect;
  vbRect:TRect;
  Font:TDxFont;
  Pt1, Pt2, Pt3:TPoint;
begin
  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := VirtualRect;

  DoPaint();

  if Designing then begin
    // Canvas.FillRectAlpha(vbRect, BackgroundColor, 150);
    FrameRect(vtRect, vtRect, vbRect, BorderColor.Up.Color);
  end;

  if Enabled then begin
    if MouseDowned then begin
      Font := FLineColor.Down;
    end
    else if MouseMoveed then
      Font := FLineColor.Hot
    else
      Font := FLineColor.Up;
  end
  else
    Font := FLineColor.Disabled;
  if Font <> nil then begin
    case FLineStyle of
      lsHorizontal:GameCanvas.Line(Point(vtRect.Left, vtRect.Top + (Height - 1) div 2), Point(vtRect.Right, vtRect.Top + (Height - 1) div 2), Font.Color);
      lsVertical:GameCanvas.Line(Point(vtRect.Left + (Width - 1) div 2, vtRect.Top), Point(vtRect.Left + (Width - 1) div 2, vtRect.Bottom), Font.Color);
      lsCircle:GameCanvas.Circle(vtRect.Left + Width div 2, vtRect.Top + Height div 2, Max(Width div 2, Height div 2), Font.Color);
      lsTriangle:begin
          Pt1 := Point(vtRect.Left + (Width - 1) div 2, vtRect.Top);
          Pt2 := Point(vtRect.Left, vtRect.Bottom);
          Pt3 := Point(vtRect.Right, vtRect.Bottom);
          GameCanvas.Line(Pt1, Pt2, Font.Color);
          GameCanvas.Line(Pt1, Pt3, Font.Color);
          GameCanvas.Line(Pt2, Pt3, Font.Color);
        end;
    end;
  end;
end;

end.
