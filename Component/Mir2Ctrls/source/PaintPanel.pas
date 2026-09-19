unit PaintPanel;

interface

uses
  Windows, Controls, Forms, Classes, Messages, Graphics, ExtCtrls, Themes, Mir2Common;

type
  TCustomPaintPanel = class(TCustomPanel)
  private
    FOnPaint: TNotifyEvent;
    FPicture: TPicture;

    FAutoHeight: Boolean;
    FAutoWidth: Boolean;

    FHorzStretchType: THorzStretchType;
    FVertStretchType: TVertStretchType;

    FStretchPoint: TPoint;

    FBevelColor: TColor;
    FBevelStyle: TBorderStyle;
    FEnabledEraseBkgnd: Boolean;

    FDrawStyle: TDrawStyle;

    FCaptionOffset: TPoint;

    FWorkAreaParentColor: Boolean;
    FWorkAreaColor: TColor;

{$IF CompilerVersion < 18.5}
    FPadding: TMargins;
    FVerticalAlignment: TVerticalAlignment;
{$IFEND}
    procedure SetAutoHeight(const Value: Boolean);
    procedure SetAutoWidth(const Value: Boolean);
    procedure SetHorzStretchType(const Value: THorzStretchType);
    procedure SetVertStretchType(const Value: TVertStretchType);
    procedure SetPicture(Value: TPicture);
    procedure WMEraseBkgnd(var Message: TWmEraseBkgnd); message WM_ERASEBKGND;
    procedure CMColorChanged(var Message: TMessage); message CM_COLORCHANGED;
    procedure PictureChanged(Sender: TObject);

    procedure SetBevelColor(const Value: TColor);
    procedure SetBevelStyle(const Value: TBorderStyle);
    procedure SetEnabledEraseBkgnd(const Value: Boolean);
    procedure SetPicHorzStretchPoint(const Value: Integer);
    procedure SetPicVertStretchPoint(const Value: Integer);

    procedure SetCaptionOffsetX(const Value: Integer);
    procedure SetCaptionOffsetY(const Value: Integer);

{$IF CompilerVersion < 18.5}
    procedure SetVerticalAlignment(Value: TVerticalAlignment);
    procedure PaddingChanged(Sender: TObject);
    procedure SetPadding(const Value: TMargins);
{$IFEND}
    procedure SetDrawStyle(const Value: TDrawStyle);
    procedure SetWorkAreaColor(const Value: TColor);
    procedure SetWorkAreaParentColor(const Value: Boolean);
  protected
    procedure Paint; override;
    procedure CreateParams(var Params: TCreateParams); override;
{$IF CompilerVersion < 18.5}
    procedure AdjustClientRect(var Rect: TRect); override;
    procedure AlignControls(AControl: TControl; var Rect: TRect); override;
{$IFEND}
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    property Canvas;
  public
{$IF CompilerVersion < 18.5}
    property VerticalAlignment: TVerticalAlignment read FVerticalAlignment write SetVerticalAlignment default taVerticalCenter;
    property Padding: TMargins read FPadding write SetPadding;
{$IFEND}
    property WorkAreaParentColor: Boolean read FWorkAreaParentColor write SetWorkAreaParentColor default True;
    property WorkAreaColor: TColor read FWorkAreaColor write SetWorkAreaColor default clBtnFace;
    property EnabledEraseBkgnd: Boolean read FEnabledEraseBkgnd write SetEnabledEraseBkgnd default True;
    property BevelStyle: TBorderStyle read FBevelStyle write SetBevelStyle default bsNone;
    property BevelColor: TColor read FBevelColor write SetBevelColor default clBlack;
    property OnPaint: TNotifyEvent read FOnPaint write FOnPaint;
    property Picture: TPicture read FPicture write SetPicture;
    property AutoWidth: Boolean read FAutoWidth write SetAutoWidth;
    property AutoHeight: Boolean read FAutoHeight write SetAutoHeight;
    property PicHorzStretch: THorzStretchType read FHorzStretchType write SetHorzStretchType default hstNone;
    property PicVertStretch: TVertStretchType read FVertStretchType write SetVertStretchType default vstNone;

    property PicHorzStretchPoint: Integer read FStretchPoint.X write SetPicHorzStretchPoint default 0;
    property PicVertStretchPoint: Integer read FStretchPoint.Y write SetPicVertStretchPoint default 0;
    property DrawStyle: TDrawStyle read FDrawStyle write SetDrawStyle default dsStretch;

    property CaptionOffsetX: Integer read FCaptionOffset.X write SetCaptionOffsetX default 0;
    property CaptionOffsetY: Integer read FCaptionOffset.Y write SetCaptionOffsetY default 0;
  end;

  TPaintPanel = class(TCustomPaintPanel)
  published
    property Align;
    property Alignment;
    property Anchors;
    property AutoSize;
    property BevelWidth;
    property BiDiMode;
    property BorderWidth;

    property Caption;
    property Color;
    property Constraints;
    property Ctl3D;
    property UseDockManager default True;
    property DockSite;
    property DragCursor;
    property DragKind;
    property DragMode;
    property Enabled;
    property FullRepaint;
    property Font;
    property Locked;

    property VerticalAlignment;
    property Padding;
{$IF CompilerVersion >= 18.5}
    property OnAlignInsertBefore;
    property OnAlignPosition;
    property OnMouseActivate;
    property OnMouseEnter;
    property OnMouseLeave;
{$IFEND}
    property ParentBiDiMode;
    property ParentBackground;
    property ParentColor default True;
    property ParentCtl3D;
    property ParentFont;
    property ParentShowHint;
    property PopupMenu;
    property ShowHint;
    property TabOrder;
    property TabStop;
    property Visible;
    property OnCanResize;
    property OnClick;
    property OnConstrainedResize;
    property OnContextPopup;
    property OnDockDrop;
    property OnDockOver;
    property OnDblClick;
    property OnDragDrop;
    property OnDragOver;
    property OnEndDock;
    property OnEndDrag;
    property OnEnter;
    property OnExit;
    property OnGetSiteInfo;
    property OnMouseDown;
    property OnMouseMove;
    property OnMouseUp;
    property OnResize;
    property OnStartDock;
    property OnStartDrag;
    property OnUnDock;

    property DoubleBuffered;
    property EnabledEraseBkgnd;
    property BevelStyle;
    property BevelColor;
    property OnPaint;
    property Picture;
    property AutoWidth;
    property AutoHeight;
    property PicHorzStretch;
    property PicVertStretch;

    property PicHorzStretchPoint;
    property PicVertStretchPoint;

    property CaptionOffsetX;
    property CaptionOffsetY;

    property WorkAreaParentColor;
    property WorkAreaColor;
    
    property DrawStyle;
  end;

implementation

{ TPaintPanel }

constructor TCustomPaintPanel.Create(AOwner: TComponent);
begin
  inherited;
  FPicture := TPicture.Create;
  FHorzStretchType := hstNone;
  FVertStretchType := vstNone;
  FStretchPoint.X := 0;
  FStretchPoint.Y := 0;
  BevelOuter := bvNone;
  FPicture.OnChange := PictureChanged;
  //ControlStyle := ControlStyle - [csOpaque];
  Ctl3D := False;
  BorderStyle := bsNone;
  ParentFont := True;
  ParentColor := True;

  FBevelStyle := bsNone;
  FBevelColor := clBlack;

  FEnabledEraseBkgnd := True;

  FCaptionOffset.X := 0;
  FCaptionOffset.Y := 0;

  FWorkAreaParentColor := True;
  FWorkAreaColor := Color;

{$IF CompilerVersion < 18.5}
  FVerticalAlignment := taVerticalCenter;
  ParentBackground := False;
  ControlStyle := ControlStyle - [csParentBackground];

  FPadding := TMargins.Create(Self);
  FPadding.OnChange := PaddingChanged;
{$IFEND}
end;

procedure TCustomPaintPanel.CreateParams(var Params: TCreateParams);
begin
  inherited;
  Params.WindowClass.style := Params.WindowClass.style or CS_HREDRAW or CS_VREDRAW;
  Params.Style := Params.Style or BS_OWNERDRAW;
end;

destructor TCustomPaintPanel.Destroy;
begin
  FPicture.Free;
{$IF CompilerVersion < 18.5}
  FPadding.Free;
{$IFEND}
  inherited;
end;

procedure TCustomPaintPanel.Paint;
const
  Alignments: array[TAlignment] of Longint = (DT_LEFT, DT_RIGHT, DT_CENTER);
  VerticalAlignments: array[TVerticalAlignment] of Longint = (DT_TOP, DT_BOTTOM, DT_VCENTER);
var
  Rect, WorkRect: TRect;
  Size: TPoint;
  Flags: Longint;
  TopColor, BottomColor: TColor;
  X, Y, W, H, I, J: Integer;
  PicW, PicH: Integer;

  procedure AdjustColors(Bevel: TPanelBevel);
  begin
    TopColor := clBtnHighlight;
    if Bevel = bvLowered then TopColor := clBtnShadow;
    BottomColor := clBtnShadow;
    if Bevel = bvLowered then BottomColor := clBtnHighlight;
  end;
begin
  Rect := GetClientRect;
  Canvas.Lock;
  try
    if Assigned(FPicture.Graphic) then
    begin
      Size.X := Width;
      Size.Y := Height;
      if FDrawStyle = dsStretch then
        StretchPicture(FPicture.Bitmap, Canvas.Handle, Size, Point(0, 0), FHorzStretchType, FVertStretchType, FStretchPoint)
      else
      begin
        PicW := FPicture.Graphic.Width;
        PicH := FPicture.Graphic.Height;

        W := (Width + PicW - 1) div PicW;
        H := (Height + PicH - 1) div PicH;

        for I := 0 to W - 1 do
        begin
          for J := 0 to H - 1 do
            Canvas.Draw(I * PicW, J * PicH, FPicture.Graphic);
        end;
      end;
    end
    else
    begin
      with Canvas do
      begin
        if not ThemeServices.ThemesEnabled or not ParentBackground then
        begin
          Brush.Color := Color;
          FillRect(Rect);
        end;
      end;

      if BevelOuter <> bvNone then
      begin
        AdjustColors(BevelOuter);
        Frame3D(Canvas, Rect, TopColor, BottomColor, BevelWidth);
      end;

      Frame3D(Canvas, Rect, Color, Color, BorderWidth);

      if BevelInner <> bvNone then
      begin
        AdjustColors(BevelInner);
        Frame3D(Canvas, Rect, TopColor, BottomColor, BevelWidth);
      end;

      if not FWorkAreaParentColor then
      begin
        WorkRect.Left := Rect.Left + Padding.Left;
        WorkRect.Right := Rect.Right - Padding.Right;
        WorkRect.Top := Rect.Top + Padding.Top;
        WorkRect.Bottom := Rect.Bottom - Padding.Bottom;

        Canvas.Brush.Style := bsSolid;
        Canvas.Brush.Color := FWorkAreaColor;
        Canvas.FillRect(WorkRect);
      end;
    end;

    Canvas.Brush.Style := bsClear;
    Font := Self.Font;

    if Length(Caption) > 0 then
    begin
      Canvas.Font := Font;

    {$IF CompilerVersion <= 18.5}
      OffsetRect(Rect, CaptionOffsetX, CaptionOffsety);
    {$ELSE}
      OffsetRect(Rect, MulDiv(CaptionOffsetX, 96, FCurrentPPI), MulDiv(CaptionOffsety, 96, FCurrentPPI));
    {$IFEND}
      Flags := DT_EXPANDTABS or DT_SINGLELINE or
        VerticalAlignments[VerticalAlignment] or Alignments[Alignment];
      Flags := DrawTextBiDiModeFlags(Flags);
      DrawText(Canvas.Handle, PChar(Caption), -1, Rect, Flags);
    end;

    if FBevelStyle <> bsNone then
    begin
      Canvas.Pen.Color := FBevelColor;
      Canvas.Pen.Width := BevelWidth;

      X := Canvas.Pen.Width div 2;
      Y := X;
      W := Width - Canvas.Pen.Width + 1;
      H := Height - Canvas.Pen.Width + 1;
      if Canvas.Pen.Width = 0 then
      begin
        Dec(W);
        Dec(H);
      end;
      Canvas.Brush.Style := bsClear;
      Canvas.Rectangle(X, Y, W, H);
      {
      if BevelWidth mod 2 = 0 then
      begin
        Rect.Left := Rect.Left + 1;
        Rect.Top := Rect.Top + 1;
      end;
      Canvas.Rectangle(Rect);
      }
    end;

    if Assigned(FOnPaint) then FOnPaint(Self);
  finally
    Canvas.Unlock;
  end;
end;

procedure TCustomPaintPanel.PictureChanged(Sender: TObject);
begin
  if Assigned(FPicture.Graphic) then
  begin
    if FAutoWidth then Width := FPicture.Bitmap.Width;
    if FAutoHeight then Height := FPicture.Bitmap.Height;
  end;
end;

procedure TCustomPaintPanel.SetAutoHeight(const Value: Boolean);
begin
  if (FAutoHeight <> Value) and (Assigned(FPicture.Graphic)) then
  begin
    FAutoHeight := Value;
    if Value then Height := FPicture.Bitmap.Height;
  end;
end;

procedure TCustomPaintPanel.SetAutoWidth(const Value: Boolean);
begin
  if (FAutoWidth <> Value) and (Assigned(FPicture.Graphic)) then
  begin
    FAutoWidth := Value;
    if Value then Width := FPicture.Bitmap.Width;
  end;
end;

procedure TCustomPaintPanel.SetBevelColor(const Value: TColor);
begin
  if FBevelColor <> Value then
  begin
    FBevelColor := Value;
    Invalidate;
  end;
end;

procedure TCustomPaintPanel.SetBevelStyle(const Value: TBorderStyle);
begin
  if FBevelStyle <> Value then
  begin
    FBevelStyle := Value;
    Invalidate;
  end;
end;

procedure TCustomPaintPanel.SetEnabledEraseBkgnd(const Value: Boolean);
begin
  if FEnabledEraseBkgnd <> Value then
  begin
    FEnabledEraseBkgnd := Value;
    Invalidate;
  end;
end;

procedure TCustomPaintPanel.SetHorzStretchType(const Value: THorzStretchType);
begin
  if FHorzStretchType <> Value then
  begin
    FHorzStretchType := Value;
    Invalidate;
  end;
end;

procedure TCustomPaintPanel.SetPicture(Value: TPicture);
begin
  FPicture.Assign(Value);
  Invalidate;
end;

procedure TCustomPaintPanel.SetPicHorzStretchPoint(const Value: Integer);
begin
  if FStretchPoint.X <> Value then
  begin
    FStretchPoint.X := Value;
    Invalidate;
  end;
end;

procedure TCustomPaintPanel.SetPicVertStretchPoint(const Value: Integer);
begin
  if FStretchPoint.Y <> Value then
  begin
    FStretchPoint.Y := Value;
    INvalidate;
  end;
end;

procedure TCustomPaintPanel.SetCaptionOffsetX(const Value: Integer);
begin
  if FCaptionOffset.X <> Value then
  begin
    FCaptionOffset.X := Value;
    Invalidate;
  end;
end;

procedure TCustomPaintPanel.SetCaptionOffsetY(const Value: Integer);
begin
  if FCaptionOffset.Y <> Value then
  begin
    FCaptionOffset.Y := Value;
    INvalidate;
  end;
end;


{$IF CompilerVersion < 18.5}
procedure TCustomPaintPanel.SetVerticalAlignment(Value: TVerticalAlignment);
begin
  if FVerticalAlignment <> Value then
  begin
    FVerticalAlignment := Value;
    Invalidate;
  end;
end;

procedure TCustomPaintPanel.PaddingChanged(Sender: TObject);
begin
  Realign;
end;

procedure TCustomPaintPanel.SetPadding(const Value: TMargins);
begin
  FPadding.Assign(Value);
end;

procedure TCustomPaintPanel.AdjustClientRect(var Rect: TRect);
begin
  inherited AdjustClientRect(Rect);
  Inc(Rect.Left, Padding.Left);
  Inc(Rect.Top, Padding.Top);
  Dec(Rect.Right, Padding.Right);
  Dec(Rect.Bottom, Padding.Bottom);
end;

procedure TCustomPaintPanel.AlignControls(AControl: TControl; var Rect: TRect);
begin
  inherited;

end;

{$IFEND}

procedure TCustomPaintPanel.SetVertStretchType(const Value: TVertStretchType);
begin
  if FVertStretchType <> Value then
  begin
    FVertStretchType := Value;
    Invalidate;
  end;
end;

procedure TCustomPaintPanel.SetWorkAreaColor(const Value: TColor);
begin
  FWorkAreaColor := Value;
  FWorkAreaParentColor := False;
end;

procedure TCustomPaintPanel.SetWorkAreaParentColor(const Value: Boolean);
begin
  FWorkAreaParentColor := Value;
  if FWorkAreaParentColor then
    FWorkAreaColor := Color;
end;

procedure TCustomPaintPanel.WMEraseBkgnd(var Message: TWmEraseBkgnd);
begin
  if Assigned(FPicture.Graphic) or (not FEnabledEraseBkgnd) then
    Message.Result := 1
  else
    Inherited;
end;

procedure TCustomPaintPanel.CMColorChanged(var Message: TMessage);
begin
  if FWorkAreaParentColor then
    FWorkAreaColor := Color;
end;

procedure TCustomPaintPanel.SetDrawStyle(const Value: TDrawStyle);
begin
  if FDrawStyle <> Value then
  begin
    FDrawStyle := Value;
    Invalidate;
  end;
end;

end.
