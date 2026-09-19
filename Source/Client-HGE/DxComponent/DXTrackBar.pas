unit DXTrackBar;

interface

uses
  Windows,
  Types,
  Classes,
  Controls,
  SysUtils,
  StdCtrls,
  Graphics,
  Forms,
  HGE,
  HGEFontEx,
  DxControls,
  DxComponents,
  GameImages,
  HUtil32;

type
  TDXTrackBar = class(TDxControl)
  private
    FSliderIndex:TDxImageIndex;
    FMin:Integer;
    FMax:Integer;
    FPosition:Integer;

    FIsHotSlider:Boolean;
    FIsDownSlider:Boolean;

    FOnChanggingPosition:TNotifyEvent;
    FOnChangedPosition:TNotifyEvent;

    procedure SetMax(const Value:Integer);
    procedure SetMin(const Value:Integer);
    procedure SetPosition(const Value:Integer);
  protected
    procedure SetOnGetImage(Value:TOnGetImage); override;
    function CanMove:Boolean; override;
    procedure DoMouseUp(); override;
  public
    constructor Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}); override;
    destructor Destroy(); override;

    procedure Paint; override;

    procedure MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); override;
    procedure MouseMove(Shift:TShiftState; X, Y:Integer); override;
    procedure MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); override;

    property OnChanggingPosition:TNotifyEvent read FOnChanggingPosition write FOnChanggingPosition;
    property OnChangedPosition:TNotifyEvent read FOnChangedPosition write FOnChangedPosition;
  published
    property AutoSize;
    property SliderIndex:TDxImageIndex read FSliderIndex write FSliderIndex;
    property Min:Integer read FMin write SetMin;
    property Max:Integer read FMax write SetMax;
    property Position:Integer read FPosition write SetPosition;
  end;

implementation

uses
  HGECanvas;

constructor TDXTrackBar.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
begin
  inherited Create(AOwner);
  AutoSize := True;
  Width := 100;
  Height := 20;

  FMin := 0;
  FMax := 10;

  FSliderIndex := TDxImageIndex.Create;
end;

destructor TDXTrackBar.Destroy();
begin
  FSliderIndex.Free;
  inherited Destroy();
end;

procedure TDXTrackBar.SetOnGetImage(Value:TOnGetImage);
begin
  inherited;
  FSliderIndex.OnGetImage := Value;
end;

procedure TDXTrackBar.Paint;
var
  vtRect:TRect;
  vbRect:TRect;
  vRect:TRect;

  nW, nH, nX, nY:Integer;

  Step:Single;

  FaceIndex:Integer;
  TextureTick:TTexture;
  TextureSlider:TTexture;
begin
  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := VirtualRect;

  DoPaint();

  if Designing then begin
    FrameRect(vtRect, vtRect, vbRect, BorderColor.Up.Color);
  end;

  if Assigned(OnStartPaint) then
    OnStartPaint(Self);

  TextureSlider := nil;

  if Assigned(OnPaint) then
    OnPaint(Self)
  else begin
    if FSliderIndex.Image <> nil then begin
      FaceIndex := FSliderIndex.Up;

      if FaceIndex >= 0 then begin
        if Enabled then
          TextureSlider := FSliderIndex.Image.Images[FaceIndex]
        else
          TextureSlider := FSliderIndex.Image.Grays[FaceIndex];
      end;
    end;

    if ImageIndex.Image <> nil then begin
      FaceIndex := ImageIndex.Up;

      if FaceIndex >= 0 then begin
        if Enabled then
          TextureTick := ImageIndex.Image.Images[FaceIndex]
        else
          TextureTick := ImageIndex.Image.Grays[FaceIndex];

        if TextureTick <> nil then begin
          vRect := vtRect;
          // 加入透明模式绘制 chongchong 2014-09-15
          DrawRect(vRect, vtRect, vbRect, TextureTick, BlendMode);
        end;
      end;

      FaceIndex := ImageIndex.Down;
      if FaceIndex >= 0 then begin
        if Enabled then
          TextureTick := ImageIndex.Image.Images[FaceIndex]
        else
          TextureTick := ImageIndex.Image.Grays[FaceIndex];

        if TextureTick <> nil then begin
          vRect := vtRect;

          {
          if TextureSlider <> nil then
            nW := vRect.Right - vRect.Left - TextureSlider.Width
          else
            nW := vRect.Right - vRect.Left;
          }

          nW := vRect.Right - vRect.Left;
          if FMax - FMin > 0 then begin
            Step := nW / (FMax - FMin);
            nX := Round(Step * (FPosition - FMin));
            vbRect.Right := vbRect.Left + nX;

            DrawRect(vRect, vtRect, vbRect, TextureTick, BlendMode);
          end;
        end;
      end;
    end;

    if TextureSlider <> nil then begin
      vRect := vtRect;
      if TextureSlider <> nil then
        nW := vRect.Right - vRect.Left - TextureSlider.Width
      else
        nW := vRect.Right - vRect.Left;

      if FMax - FMin > 0 then begin
        Step := nW / (FMax - FMin);
        nX := Round(Step * (FPosition - FMin));
        nH := TextureSlider.Height;

        nY := (vRect.Bottom - vRect.Top - nH) div 2;
        GameCanvas.Draw(vRect.Left + nX, vRect.Top + nY, TextureSlider, BlendMode);
      end;
    end;
  end;
end;

procedure TDXTrackBar.SetMax(const Value:Integer);
begin
  if FMax <> Value then begin
    FMax := Value;
    if FPosition > FMax then
      FPosition := Max;
  end;
end;

procedure TDXTrackBar.SetMin(const Value:Integer);
begin
  if FMin <> Value then begin
    FMin := Value;
    if FPosition < FMin then
      FPosition := FMin;
  end;
end;

procedure TDXTrackBar.SetPosition(const Value:Integer);
var
  V:Integer;
begin
  if Value < FMin then
    V := FMin
  else if Value > FMax then
    V := FMax
  else
    V := Value;

  if FPosition <> V then begin
    FPosition := Value;
  end;
end;

procedure TDXTrackBar.MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
begin
  inherited;
  FIsDownSlider := FIsHotSlider;
end;

procedure TDXTrackBar.MouseMove(Shift:TShiftState; X, Y:Integer);
var
  FaceIndex:Integer;
  TextureSlider:TTexture;

  vtRect:TRect;
  vbRect:TRect;
  vRect:TRect;

  nW, nH:Integer;
  Step:Single;
  nX, nY, nPos:Integer;
begin
  if not FIsDownSlider then inherited;

  //TextureSlider := nil;
  if FSliderIndex.Image <> nil then begin
    vbRect := VisibleRect;
    if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
    vtRect := VirtualRect;

    FaceIndex := FSliderIndex.Up;
    if FaceIndex >= 0 then begin
      if Enabled then
        TextureSlider := FSliderIndex.Image.Images[FaceIndex]
      else
        TextureSlider := FSliderIndex.Image.Grays[FaceIndex];

      if TextureSlider <> nil then begin
        vRect := vtRect;

        if TextureSlider <> nil then
          nW := vRect.Right - vRect.Left - TextureSlider.Width
        else
          nW := vRect.Right - vRect.Left;

        if FMax - FMin > 0 then begin
          Step := nW / (FMax - FMin);

          if FIsDownSlider then begin
            nPos := Round((X - vtRect.Left) / Step) + FMin;
            if nPos < FMin then
              nPos := FMin
            else if nPos > FMax then
              nPos := FMax;

            FPosition := nPos;

            if Assigned(FOnChanggingPosition) then
              FOnChanggingPosition(Self);
          end
          else begin
            nX := Round(Step * (FPosition - FMin));
            nH := TextureSlider.Height;

            nY := (vRect.Bottom - vRect.Top - nH) div 2;

            vtRect := vRect;
            vtRect.Left := vRect.Left + nX;
            vtRect.Top := vtRect.Top + nY;
            vtRect.Right := vtRect.Left + TextureSlider.Width;
            vtRect.Bottom := vtRect.Top + TextureSlider.Height;

            FIsHotSlider := PtInRect(vtRect, Point(X, Y));

            Exit;
          end;
        end;
      end;
    end;
  end;

  FIsHotSlider := False;
end;

procedure TDXTrackBar.MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
begin
  inherited;
  FIsDownSlider := False;

  if Assigned(FOnChangedPosition) then
    FOnChangedPosition(Self);
end;

function TDXTrackBar.CanMove:Boolean;
begin
  Result := not FIsDownSlider;
end;

procedure TDXTrackBar.DoMouseUp();
begin
  inherited;
  FIsDownSlider := False;
  if Assigned(FOnChangedPosition) then
    FOnChangedPosition(Self);
end;

end.
