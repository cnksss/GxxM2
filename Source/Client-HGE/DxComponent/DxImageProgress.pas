unit DxImageProgress;

interface

uses
  Windows,
  Types,
  Classes,
  Controls,
  SysUtils,
  Graphics,
  HGE,
  HGECanvas,
  DxComponents,
  DxControls,
  GameImages,
  Math,
  HGEFontEx;

type
  TDxImageProgress = class;

  TProgressSetting = class(TPersistent)
  private
    FOwner:TDxImageProgress;

    FFont:TDxFont;

    FOnChange:TNotifyEvent;
    FOnGetImage:TOnGetImage;

    FImageType:TImageType; // ??WIL??
    FImage:TGameImages;

    FImageBG:Integer;
    FImageProgress:Integer;
    FImageProgressX:Integer;
    FImageProgressY:Integer;

    FValueType:TProgressValueType;
    FValueSplite:string;

    FValueAlignment:TAlignment;
    FValuePrefix:string;
    FValueSuffix:string;

    FMax:LongWord;
    FMin:LongWord;
    FValue:LongWord;

    procedure SetImageType(Value:TImageType);
    procedure SetOnGetImage(Value:TOnGetImage);
    procedure SetImageBG(const Value:Integer);
    procedure SetImageProgress(const Value:Integer);

    procedure SetFont(Value:TDxFont);
    procedure FontChange(Sender:TObject); stdcall;
  protected
    procedure Changed; // dynamic;
  public
    constructor Create(AOwner:TDxImageProgress);
    destructor Destroy; override;
    procedure Assign(Source:TPersistent); override;

    property Image:TGameImages read FImage;
    property OnChange:TNotifyEvent read FOnChange write FOnChange;
    property OnGetImage:TOnGetImage read FOnGetImage write SetOnGetImage;
  published
    property Font:TDxFont read FFont write SetFont;

    property ImageType:TImageType read FImageType write SetImageType;
    property ImageBG:Integer read FImageBG write SetImageBG;
    property ImageProgress:Integer read FImageProgress write SetImageProgress;

    property ImageProgressX:Integer read FImageProgressX write FImageProgressX;
    property ImageProgressY:Integer read FImageProgressY write FImageProgressY;

    property ValueType:TProgressValueType read FValueType write FValueType;
    property ValueSplite:string read FValueSplite write FValueSplite;
    property ValueAlignment:TAlignment read FValueAlignment write FValueAlignment;
    property ValuePrefix:string read FValuePrefix write FValuePrefix;
    property ValueSuffix:string read FValueSuffix write FValueSuffix;

    property Max:LongWord read FMax write FMax;
    property Min:LongWord read FMin write FMin;
    property Value:LongWord read FValue write FValue;
  end;

  TDxImageProgress = class(TDxControl)
  private
    FProgressSetting:TProgressSetting;

    procedure RecallAutoSize;
  protected
    procedure DoOnInRealArea(X, Y:Integer; var IsRealArea:Boolean); override;
    procedure SetOnGetImage(Value:TOnGetImage); override;
  public
    constructor Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}); override;
    destructor Destroy; override;
    procedure Assign(Source:TDxControl); override;
    procedure Paint; override;
  published
    property ProgressSetting:TProgressSetting read FProgressSetting write FProgressSetting;
    property AutoSize;
  end;

implementation

constructor TProgressSetting.Create(AOwner:TDxImageProgress);
begin
  inherited Create;
  FOwner := AOwner;

  FFont := TDxFont.Create;
  FFont.Bold := True;
  FFont.Color := clWhite;
  FFont.OnChange := FontChange;

  FImageType := Prguse_wil;

  FOwner := AOwner;
  FImageBG := -1;
  FImageProgress := -1;

  FImageProgressX := 0;
  FImageProgressY := 0;

  FValueType := vtValue;
  FValueSplite := '-';
  FValueAlignment := taCenter;
  FValuePrefix := '';
  FValueSuffix := '';

  FMax := 100;
  FMin := 0;
  FValue := 50;

  FImage := nil;
  FOnChange := nil;
  FOnGetImage := nil;
end;

destructor TProgressSetting.Destroy;
begin
  FFont.Free;
  inherited;
end;

procedure TProgressSetting.SetImageType(Value:TImageType);
begin
  if FImageType <> Value then begin
    FImageType := Value;
    if Assigned(FOnGetImage) then
      FOnGetImage(Self, FImageType, TObject(FImage));
    Changed;
  end;
end;

procedure TProgressSetting.SetOnGetImage(Value:TOnGetImage);
begin
  FOnGetImage := Value;
  if Assigned(FOnGetImage) then
    FOnGetImage(Self, FImageType, TObject(FImage));
  Changed;
end;

procedure TProgressSetting.SetFont(Value:TDxFont);
begin
  FFont.Assign(Value);
end;

procedure TProgressSetting.FontChange(Sender:TObject); stdcall;
begin
  if Assigned(FOnChange) then
    FOnChange(Self);
end;

procedure TProgressSetting.SetImageBG(const Value:Integer);
begin
  if FImageBG <> Value then begin
    FImageBG := Value;
    Changed;
  end;
end;

procedure TProgressSetting.SetImageProgress(const Value:Integer);
begin
  if FImageProgress <> Value then begin
    FImageProgress := Value;
    Changed;
  end;
end;

procedure TProgressSetting.Changed;
begin
  FOwner.RecallAutoSize;

  if Assigned(FOnChange) then
    FOnChange(Self);
end;

procedure TProgressSetting.Assign(Source:TPersistent);
begin
  if Source is TProgressSetting then begin
    OnGetImage := TProgressSetting(Source).OnGetImage;
    FImageType := TProgressSetting(Source).FImageType;

    FFont.Assign(TProgressSetting(Source).FFont);

    FImageBG := TProgressSetting(Source).FImageBG;
    FImageProgress := TProgressSetting(Source).FImageProgress;

    FValueType := TProgressSetting(Source).FValueType;
    FValueSplite := TProgressSetting(Source).FValueSplite;
    FValueAlignment := TProgressSetting(Source).FValueAlignment;
    FValuePrefix := TProgressSetting(Source).FValuePrefix;
    FValueSuffix := TProgressSetting(Source).FValueSuffix;

    FMax := TProgressSetting(Source).FMax;
    FMin := TProgressSetting(Source).FMin;
    FValue := TProgressSetting(Source).FValue;

    Changed;
  end;
end;

{------------------------------------------------------------------------------}

constructor TDxImageProgress.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
begin
  inherited Create(AOwner);

  FProgressSetting := TProgressSetting.Create(Self);
  FProgressSetting.OnGetImage := OnGetImage;

  Width := 90;
  Height := 20;
end;

destructor TDxImageProgress.Destroy;
begin
  FProgressSetting.Free;
  inherited Destroy;
end;

procedure TDxImageProgress.Paint;
var
  I, nWidth, nHeight:Integer;
  D:TTexture;
  vtRect, vbRect, SrcRect:TRect;
  S:string;
  HGEFont:THGEFont;
  TextImages:TImageInfos;
begin
  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := VirtualRect;

  if FProgressSetting.Image <> nil then begin
    if (FProgressSetting.FImageBG >= 0) then begin
      D := FProgressSetting.Image.Images[FProgressSetting.FImageBG];

      if D <> nil then begin
        SrcRect := Rect(0, 0, Min(D.Width, Width), Min(D.Height, Height));
        GameCanvas.Draw(vtRect.Left, vtRect.Top, SrcRect, D);
      end;
    end;

    if (FProgressSetting.FImageProgress >= 0) and
      (FProgressSetting.FMax > FProgressSetting.FMin) and
      (FProgressSetting.FValue >= FProgressSetting.Min) then begin
      D := FProgressSetting.Image.Images[FProgressSetting.FImageProgress];

      if D <> nil then begin
        SrcRect := Rect(0, 0, D.Width, D.Height);
        SrcRect.Right := SrcRect.Left + Round((SrcRect.Right - SrcRect.Left) / (FProgressSetting.FMax - FProgressSetting.FMin) * Min(FProgressSetting.FValue, FProgressSetting.FMax));
        GameCanvas.Draw(vtRect.Left + FProgressSetting.FImageProgressX, vtRect.Top + FProgressSetting.FImageProgressY, SrcRect, D);
      end;
    end;

    if (FProgressSetting.FImageBG >= 0) and
      (FProgressSetting.FMax > FProgressSetting.FMin) and
      (FProgressSetting.FValue >= FProgressSetting.Min) then begin
      if FProgressSetting.FValueType = vtValueAndMax then
        S := FProgressSetting.ValuePrefix + IntToStr(FProgressSetting.FValue) + FProgressSetting.FValueSplite + IntToStr(FProgressSetting.FMax) + FProgressSetting.FValueSuffix
      else if FProgressSetting.FValueType = vtValue then
        S := FProgressSetting.ValuePrefix + IntToStr(FProgressSetting.FValue) + FProgressSetting.FValueSuffix
      else if FProgressSetting.FValueType = vtPercentage then
        S := FProgressSetting.ValuePrefix + Format('%d%%', [Round(FProgressSetting.FValue / (FProgressSetting.FMax - FProgressSetting.FMin) * 100)]) + FProgressSetting.FValueSuffix
      else
        S := '';

      if Length(S) > 0 then begin
        HGEFont := TextureFonts.FindFont(FProgressSetting.FFont.Name, FProgressSetting.FFont.Size, FProgressSetting.FFont.Style);

        if HGEFont <> nil then begin
          TextImages := HGEFont.GetImageInfos(S);

          nWidth := 0;
          nHeight := 0;
          for I := 0 to Length(TextImages) - 1 do begin
            if nWidth < TextImages[I].Width then
              nWidth := TextImages[I].Width;

            if TextImages[I].Height <= 0 then
              Inc(nHeight, HGEFont.TextHeight('0'))
            else
              Inc(nHeight, TextImages[I].Height);
          end;

          if FProgressSetting.Font.Bold then begin
            Inc(nWidth, 2);
            Inc(nHeight, 2);
          end;

          DrawCaption(HGEFont, FProgressSetting.Font, FProgressSetting.FValueAlignment, TextImages, Bounds(vtRect.Left, vtRect.Top, nWidth, nHeight), vbRect, vtRect, 0, 0);
        end;
      end;
    end;
  end;

  if Assigned(OnStartSubPaint) then
    OnStartSubPaint(Self);

  for I := ControlCount - 1 downto 0 do begin
    if not Visible then break;
    if (Control[I].Visible) then
      Control[I].Paint;
  end;

  if Visible and Assigned(OnStopPaint) then
    OnStopPaint(Self);

  if Designing then begin
    FrameRect(vtRect, vtRect, vbRect, BorderColor.Up.Color);
  end;
end;

procedure TDxImageProgress.SetOnGetImage(Value:TOnGetImage);
begin
  inherited;
  FProgressSetting.OnGetImage := Value;
end;

procedure TDxImageProgress.DoOnInRealArea(X, Y:Integer; var IsRealArea:Boolean);
begin
  inherited;
end;

procedure TDxImageProgress.RecallAutoSize;
var
  D:TTexture;
begin
  if not AutoSize then Exit;

  if FProgressSetting.Image <> nil then begin
    if (FProgressSetting.FImageBG >= 0) then begin
      D := FProgressSetting.Image.Images[FProgressSetting.FImageBG];

      if (D <> nil) and (D.Width * D.Height > 4) then begin
        Width := D.Width;
        Height := D.Height;
      end;
    end;
  end;
end;

procedure TDxImageProgress.Assign(Source:TDxControl);
begin
  if Source is TDxImageProgress then begin
    Left := Source.Left;
    Top := Source.Top;
    Width := Source.Width;
    Height := Source.Height;
    Enabled := Source.Enabled;
    Visible := Source.Visible;

    Transparent := Source.Transparent; // ????
    EnableFocus := Source.EnableFocus; // ????????
    Floating := Source.Floating; // ??????
    OwnerMove := Source.OwnerMove;

    MouseEvents := Source.MouseEvents;

    //Designing := Source.Designing;                                                                    // ??????
    ReferenceX := Source.ReferenceX;
    AdjustYByHeight := Source.AdjustYByHeight;
    TopAlignment := Source.TopAlignment;

    Center := Source.Center;
    Align := Source.Align;
    AutoSize := Source.AutoSize;
    BackgroundColor := Source.BackgroundColor;

    ImageIndex.Assign(Source.ImageIndex);
    BorderColor.Assign(Source.BorderColor);

    FProgressSetting.Assign(TDxImageProgress(Source).FProgressSetting);
  end;
end;

end.
