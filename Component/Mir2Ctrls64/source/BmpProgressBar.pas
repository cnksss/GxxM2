unit BmpProgressBar;

interface

uses SysUtils, Windows, Messages, Classes, Graphics, Controls, Forms, StdCtrls;

type

  TGaugeKind = (gkHorizontalBar, gkVerticalBar);

  TProgressBarBitmaps = class(TPersistent)
  private
    FBorder: TBitmap;
    FProgress: TBitmap;
    FTransparentColor: TColor;
    FOnChange: TNotifyEvent;

    procedure SetBorder(Value: TBitmap);
    procedure SetProgress(Value: TBitmap);
    procedure SetTransparentColor(Value: TColor);

    procedure BitmapsChanged(Sender: TObject);
  public
    constructor Create;
    destructor Destroy; override;
  published
    property Border: TBitmap read FBorder write SetBorder;
    property Progress: TBitmap read FProgress write SetProgress;
    property TransparentColor: TColor read FTransparentColor write SetTransparentColor;

    property OnChange: TNotifyEvent read FOnChange write FOnChange;
  end;


  TBmpProgressBar = class(TGraphicControl)
  private
    FMinValue: Longint;
    FMaxValue: Longint;
    FCurValue: Longint;
    FKind: TGaugeKind;
    FShowText: Boolean;
    FBorderStyle: TBorderStyle;
    FForeColor: TColor;
    FBackColor: TColor;
    procedure PaintProgressText(PaintRect: TRect);
    procedure PaintProgressBar(PaintRect: TRect);
    procedure SetGaugeKind(Value: TGaugeKind);
    procedure SetShowText(Value: Boolean);
    procedure SetBorderStyle(Value: TBorderStyle);
    procedure SetForeColor(Value: TColor);
    procedure SetBackColor(Value: TColor);
    procedure SetMinValue(Value: Longint);
    procedure SetMaxValue(Value: Longint);
    procedure SetProgress(Value: Longint);
    function GetPercentDone: Longint;
  private
    FBitmapCache: TBitmap;

    FBitmaps: TProgressBarBitmaps;
    procedure BitmapChanged(Sender: TObject);
  protected
    procedure Paint; override;
    procedure Resize; override;
    procedure Loaded; override;
    procedure WMSize(var Msg: TWMSize); message WM_SIZE;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    procedure AddProgress(Value: Longint);
    property PercentDone: Longint read GetPercentDone;
  published
    property Align;
    property Anchors;
    property BackColor: TColor read FBackColor write SetBackColor default clWhite;
    property BorderStyle: TBorderStyle read FBorderStyle write SetBorderStyle default bsSingle;
    property Color;
    property Constraints;
    property Enabled;
    property ForeColor: TColor read FForeColor write SetForeColor default clBlack;
    property Font;
    property Kind: TGaugeKind read FKind write SetGaugeKind default gkHorizontalBar;
    property MinValue: Longint read FMinValue write SetMinValue default 0;
    property MaxValue: Longint read FMaxValue write SetMaxValue default 100;
    property ParentColor;
    property ParentFont;
    property ParentShowHint;
    property PopupMenu;
    property Progress: Longint read FCurValue write SetProgress;
    property ShowHint;
    property ShowText: Boolean read FShowText write SetShowText default True;
    property Visible;

    property Bitmaps: TProgressBarBitmaps read FBitmaps write FBitmaps;
  end;

implementation

uses Consts;

{ This function solves for x in the equation "x is y% of z". }
function SolveForX(Y, Z: Longint): Longint;
begin
  Result := Longint(Trunc( Z * (Y * 0.01) ));
end;

{ This function solves for y in the equation "x is y% of z". }
function SolveForY(X, Z: Longint): Longint;
begin
  if Z = 0 then Result := 0
  else Result := Longint(Trunc( (X * 100.0) / Z ));
end;

{ TProgressBarBitmaps }

constructor TProgressBarBitmaps.Create;
begin
  FBorder := TBitmap.Create;
  FBorder.OnChange := BitmapsChanged;

  FProgress := TBitmap.Create;
  FProgress.OnChange := BitmapsChanged;
end;

destructor TProgressBarBitmaps.Destroy;
begin
  FBorder.Free;
  FProgress.Free;
  inherited;
end;

procedure TProgressBarBitmaps.BitmapsChanged(Sender: TObject);
begin
  if Assigned(FOnChange) then
    FOnChange(Self);
end;

procedure TProgressBarBitmaps.SetBorder(Value: TBitmap);
begin
  FBorder.Assign(Value);
end;

procedure TProgressBarBitmaps.SetProgress(Value: TBitmap);
begin
  FProgress.Assign(Value);
end;

procedure TProgressBarBitmaps.SetTransparentColor(Value: TColor);
begin
  if FTransparentColor <> Value then
  begin
    FTransparentColor := Value;
    BitmapsChanged(Self);
  end;
end;

{ TGauge }

constructor TBmpProgressBar.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
  ControlStyle := ControlStyle + [csFramed, csOpaque];
  { default values }
  FMinValue := 0;
  FMaxValue := 100;
  FCurValue := 0;
  FKind := gkHorizontalBar;
  FShowText := True;
  FBorderStyle := bsSingle;
  FForeColor := clBlack;
  FBackColor := clWhite;

  FBitmapCache := TBitmap.Create;
  FBitmapCache.Width := 100;
  FBitmapCache.Height := 100;

  Width := FBitmapCache.Width;
  Height := FBitmapCache.Height;

  FBitmaps := TProgressBarBitmaps.Create;
  FBitmaps.OnChange := BitmapChanged;
end;

destructor TBmpProgressBar.Destroy;
begin
  FBitmapCache.Free;
  FBitmaps.Free;
  inherited;
end;

procedure TBmpProgressBar.Resize;
begin
  inherited;
  FBitmapCache.Width := Width;
  FBitmapCache.Height := Height;
end;

procedure TBmpProgressBar.Loaded;
var
  Msg: TWMSize;
begin
  inherited;
  WMSize(Msg);

  FBitmapCache.Width := Width;
  FBitmapCache.Height := Height;
end;

function TBmpProgressBar.GetPercentDone: Longint;
begin
  Result := SolveForY(FCurValue - FMinValue, FMaxValue - FMinValue);
end;

procedure TBmpProgressBar.Paint;
var
  PaintRect: TRect;
begin
  with Canvas do
  begin
    PaintRect := ClientRect;

    case FKind of
      gkHorizontalBar, gkVerticalBar: PaintProgressBar(PaintRect);
    end;

    if ShowText then PaintProgressText(PaintRect);

    FBitmapCache.Canvas.CopyMode := cmSrcCopy;
    if FBorderStyle = bsSingle then
    begin
      FBitmapCache.Canvas.Brush.Style := bsClear;
      FBitmapCache.Canvas.Pen.Color := Color;
      FBitmapCache.Canvas.Rectangle(PaintRect);
    end;

    Canvas.CopyMode := cmSrcCopy;
    Canvas.Draw(0, 0, FBitmapCache);
  end;
end;

procedure TBmpProgressBar.PaintProgressText(PaintRect: TRect);
var
  S: string;
  X, Y: Integer;
begin
  //FShowTextBitmap.Canvas.Brush.Color := clBlack;
  //FShowTextBitmap.Canvas.FillRect(PaintRect);
  if PercentDone = 0 then Exit;
  
  S := Format('%d%%', [PercentDone]);
  with FBitmapCache.Canvas do
  begin
    Brush.Style := bsClear;
    Font := Self.Font;
    Font.Color := Self.Font.Color;
    with PaintRect do
    begin
      X := (Right - Left + 1 - TextWidth(S)) div 2;
      Y := (Bottom - Top + 1 - TextHeight(S)) div 2;
    end;
    TextRect(PaintRect, X, Y, S);
  end;
end;


procedure TBmpProgressBar.PaintProgressBar(PaintRect: TRect);
var
  R: TRect;
  FillSize: Longint;
  W, H: Integer;
begin
  W := PaintRect.Right - PaintRect.Left + 1;
  H := PaintRect.Bottom - PaintRect.Top + 1;


  with FBitmapCache.Canvas do
  begin
    Brush.Color := BackColor;

    if (FBitmaps.FBorder.Width > 0) and (FBitmaps.FBorder.Height > 0) then
    begin
      Draw(PaintRect.Left, PaintRect.Top, FBitmaps.FBorder);
    end
    else
    begin
      FillRect(PaintRect);
    end;

    Pen.Color := ForeColor;
    Pen.Width := 1;
    Brush.Color := ForeColor;
    case FKind of
      gkHorizontalBar:
        begin
          FillSize := SolveForX(PercentDone, W);
          if FillSize > W then FillSize := W;
          if FillSize > 0 then
          begin
            if (FBitmaps.FProgress.Width > 0) and (FBitmaps.FProgress.Height > 0) then
            begin
              R := Rect(PaintRect.Left, PaintRect.Top, FillSize, H);
              CopyRect(R, FBitmaps.FProgress.Canvas, R);
            end
            else
              FillRect(Rect(PaintRect.Left, PaintRect.Top, FillSize, H));
          end;
        end;
      gkVerticalBar:
        begin
          FillSize := SolveForX(PercentDone, H);
          if FillSize >= H then FillSize := H - 1;

          if (FBitmaps.FProgress.Width > 0) and (FBitmaps.FProgress.Height > 0) then
          begin
            R := Rect(PaintRect.Left, PaintRect.Top, W, H);
            CopyRect(R, FBitmaps.FProgress.Canvas, R);
          end
          else
            FillRect(Rect(PaintRect.Left, H - FillSize, W, H));
        end;
    end;
  end;
end;

procedure TBmpProgressBar.SetGaugeKind(Value: TGaugeKind);
begin
  if Value <> FKind then
  begin
    FKind := Value;
    Refresh;
  end;
end;

procedure TBmpProgressBar.SetShowText(Value: Boolean);
begin
  if Value <> FShowText then
  begin
    FShowText := Value;
    Refresh;
  end;
end;

procedure TBmpProgressBar.SetBorderStyle(Value: TBorderStyle);
begin
  if Value <> FBorderStyle then
  begin
    FBorderStyle := Value;
    Refresh;
  end;
end;

procedure TBmpProgressBar.SetForeColor(Value: TColor);
begin
  if Value <> FForeColor then
  begin
    FForeColor := Value;
    Refresh;
  end;
end;

procedure TBmpProgressBar.SetBackColor(Value: TColor);
begin
  if Value <> FBackColor then
  begin
    FBackColor := Value;
    Refresh;
  end;
end;

procedure TBmpProgressBar.SetMinValue(Value: Longint);
begin
  if Value <> FMinValue then
  begin
    if Value > FMaxValue then
      if not (csLoading in ComponentState) then
        raise EInvalidOperation.CreateFmt(SOutOfRange, [-MaxInt, FMaxValue - 1]);
    FMinValue := Value;
    if FCurValue < Value then FCurValue := Value;
    Refresh;
  end;
end;

procedure TBmpProgressBar.SetMaxValue(Value: Longint);
begin
  if Value <> FMaxValue then
  begin
    if Value < FMinValue then
      if not (csLoading in ComponentState) then
        raise EInvalidOperation.CreateFmt(SOutOfRange, [FMinValue + 1, MaxInt]);
    FMaxValue := Value;
    if FCurValue > Value then FCurValue := Value;
    Refresh;
  end;
end;

procedure TBmpProgressBar.SetProgress(Value: Longint);
var
  TempPercent: Longint;
begin
  TempPercent := GetPercentDone;  { remember where we were }
  if Value < FMinValue then
    Value := FMinValue
  else if Value > FMaxValue then
    Value := FMaxValue;
  if FCurValue <> Value then
  begin
    FCurValue := Value;
    if TempPercent <> GetPercentDone then { only refresh if percentage changed }
      Refresh;
  end;
end;

procedure TBmpProgressBar.AddProgress(Value: Longint);
begin
  Progress := FCurValue + Value;
  Refresh;
end;


procedure TBmpProgressBar.BitmapChanged(Sender: TObject);
var
  Msg: TWMSize;
begin
  WMSize(Msg);
  Invalidate;
end;

procedure TBmpProgressBar.WMSize(var Msg: TWMSize);
begin
  if csLoading in ComponentState then
    Exit;

  if (FBitmaps.Border.Width > 0) and (FBitmaps.Border.Height > 0) then
  begin
    Width := FBitmaps.Border.Width;
    Height := FBitmaps.Border.Height;
    Invalidate;
  end;
end;

end.
