unit DxGroupAttackProgress;

interface

uses
  Windows,
  Types,
  Classes,
  Controls,
  SysUtils,
  Forms,
  HGE,
  DxCanvas,
  DxControls,
  Dialogs,
  DxComponents,
  GameImages,
  Graphics,
  HGECanvas,
  Math;

type
  TDxGroupAttackProgress = class;

  // ¡¨ª˜≈‰÷√
  TContinueAttackSetting = class(TPersistent)
  private
    FOwner:TDxGroupAttackProgress;

    FOnChange:TNotifyEvent;
    FOnGetImage:TOnGetImage;

    FImageType:TImageType; //  π”√WIL¿‡–Õ
    FImage:TGameImages;

    FBackground:Integer; // ±≥æ∞
    FFlashStart:Integer; // …¡À∏ø™ ºÕº∆¨
    FFlashEnd:Integer; // …¡À∏Ω· ¯Õº∆¨
    FFlashInterval:Integer; // …¡À∏º‰∏Ù

    FBgOffsetX:Integer;
    FBgOffsetY:Integer;

    FFlashOffsetX:Integer;
    FFlashOffsetY:Integer;

    procedure SetImageType(Value:TImageType);
    procedure SetOnGetImage(Value:TOnGetImage);
  protected
    procedure Changed; // dynamic;
  public
    constructor Create(AOwner:TDxGroupAttackProgress);
    procedure Assign(Source:TPersistent); override;

    property Image:TGameImages read FImage;
    property OnChange:TNotifyEvent read FOnChange write FOnChange;
    property OnGetImage:TOnGetImage read FOnGetImage write SetOnGetImage;
  published
    property ImageType:TImageType read FImageType write SetImageType;
    property Background:Integer read FBackground write FBackground;
    property FlashStart:Integer read FFlashStart write FFlashStart;
    property FlashEnd:Integer read FFlashEnd write FFlashEnd;
    property FlashInterval:Integer read FFlashInterval write FFlashInterval;
    property BgOffsetX:Integer read FBgOffsetX write FBgOffsetX;
    property BgOffsetY:Integer read FBgOffsetY write FBgOffsetY;
    property FlashOffsetX:Integer read FFlashOffsetX write FFlashOffsetX;
    property FlashOffsetY:Integer read FFlashOffsetY write FFlashOffsetY;
  end;

  TGroupAttackSetting = class(TPersistent)
  private
    FOwner:TDxGroupAttackProgress;

    FOnChange:TNotifyEvent;
    FOnGetImage:TOnGetImage;

    FImageType:TImageType; //  π”√WIL¿‡–Õ
    FImage:TGameImages;

    FBackground:Integer; // ±≥æ∞
    FProgress:Integer;

    FFlashStart:Integer; // …¡À∏ø™ ºÕº∆¨
    FFlashEnd:Integer; // …¡À∏Ω· ¯Õº∆¨
    FFlashInterval:Integer; // …¡À∏º‰∏Ù

    FBgOffsetX:Integer;
    FBgOffsetY:Integer;

    FPgOffsetX:Integer;
    FPgOffsetY:Integer;

    FContinueOffsetX:Integer;
    FContinueOffsetY:Integer;

    procedure SetImageType(Value:TImageType);
    procedure SetOnGetImage(Value:TOnGetImage);
  protected
    procedure Changed; // dynamic;
  public
    constructor Create(AOwner:TDxGroupAttackProgress);
    procedure Assign(Source:TPersistent); override;

    property Image:TGameImages read FImage;
    property OnChange:TNotifyEvent read FOnChange write FOnChange;
    property OnGetImage:TOnGetImage read FOnGetImage write SetOnGetImage;
  published
    property ImageType:TImageType read FImageType write SetImageType;
    property Background:Integer read FBackground write FBackground;
    property Progress:Integer read FProgress write FProgress;
    property FlashStart:Integer read FFlashStart write FFlashStart;
    property FlashEnd:Integer read FFlashEnd write FFlashEnd;
    property FlashInterval:Integer read FFlashInterval write FFlashInterval;

    property BgOffsetX:Integer read FBgOffsetX write FBgOffsetX;
    property BgOffsetY:Integer read FBgOffsetY write FBgOffsetY;
    property PgOffsetX:Integer read FPgOffsetX write FPgOffsetX;
    property PgOffsetY:Integer read FPgOffsetY write FPgOffsetY;
    property ContinueOffsetX:Integer read FContinueOffsetX write FContinueOffsetX;
    property ContinueOffsetY:Integer read FContinueOffsetY write FContinueOffsetY;
  end;

  TDxGroupAttackProgress = class(TDxControl)
  private
    FProgressAlignment:TMagicBallValueAlignment;

    FContinueSetting:TContinueAttackSetting;
    FGroupSetting:TGroupAttackSetting;
    FContinueAndGroupSetting:TGroupAttackSetting;

    FOnGetGroupAttackProgress:TGetGroupAttackProgressEvent;

    FShowContinueAttack:Boolean;
    FShowGroupAttack:Boolean;
    FGroupAttackProgress:Integer;

    FContinueFlashTick:LongWord;
    FContinueFlashFrame:Integer;

    FGroupFlashTick:LongWord;
    FGroupFlashFrame:Integer;

  protected
    procedure DoOnInRealArea(X, Y:Integer; var IsRealArea:Boolean); override;
    procedure SetOnGetImage(Value:TOnGetImage); override;
  public
    constructor Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}); override;
    destructor Destroy; override;
    procedure Assign(Source:TDxControl); override;
    procedure Paint; override;
  published
    property ProgressAlignment:TMagicBallValueAlignment read FProgressAlignment write FProgressAlignment;
    property ContinueSetting:TContinueAttackSetting read FContinueSetting write FContinueSetting;
    property GroupSetting:TGroupAttackSetting read FGroupSetting write FGroupSetting;
    property ContinueAndGroupSetting:TGroupAttackSetting read FContinueAndGroupSetting write FContinueAndGroupSetting;

    property ShowContinueAttack:Boolean read FShowContinueAttack write FShowContinueAttack;
    property ShowGroupAttack:Boolean read FShowGroupAttack write FShowGroupAttack;
    property GroupAttackProgress:Integer read FGroupAttackProgress write FGroupAttackProgress;

    property OnGetGroupAttackProgress:TGetGroupAttackProgressEvent read FOnGetGroupAttackProgress write FOnGetGroupAttackProgress;
  end;

implementation

constructor TContinueAttackSetting.Create(AOwner:TDxGroupAttackProgress);
begin
  inherited Create;
  FOwner := AOwner;
  FImageType := Prguse_wil;

  FOwner := AOwner;

  FBackground := -1; // ±≥æ∞
  FFlashStart := -1; // …¡À∏ø™ ºÕº∆¨
  FFlashEnd := -1; // …¡À∏Ω· ¯Õº∆¨
  FFlashInterval := 200; // …¡À∏º‰∏Ù
  FBgOffsetX := 0;
  FBgOffsetY := 0;
  FFlashOffsetX := 0;
  FFlashOffsetY := 0;

  FImage := nil;
  FOnChange := nil;
  FOnGetImage := nil;
end;

procedure TContinueAttackSetting.SetImageType(Value:TImageType);
begin
  if FImageType <> Value then begin
    FImageType := Value;
    if Assigned(FOnGetImage) then
      FOnGetImage(Self, FImageType, TObject(FImage));
    Changed;
  end;
end;

procedure TContinueAttackSetting.SetOnGetImage(Value:TOnGetImage);
begin
  FOnGetImage := Value;
  if Assigned(FOnGetImage) then
    FOnGetImage(Self, FImageType, TObject(FImage));
  Changed;
end;

procedure TContinueAttackSetting.Changed;
begin
  if Assigned(FOnChange) then
    FOnChange(Self);
end;

procedure TContinueAttackSetting.Assign(Source:TPersistent);
begin
  if Source is TContinueAttackSetting then begin
    OnGetImage := TContinueAttackSetting(Source).OnGetImage;
    FImageType := TContinueAttackSetting(Source).FImageType;

    FBackground := TContinueAttackSetting(Source).FBackground;
    FFlashStart := TContinueAttackSetting(Source).FFlashStart;
    FFlashEnd := TContinueAttackSetting(Source).FFlashEnd;
    FFlashInterval := TContinueAttackSetting(Source).FFlashInterval;
    FBgOffsetX := TContinueAttackSetting(Source).FBgOffsetX;
    FBgOffsetY := TContinueAttackSetting(Source).FBgOffsetY;
    FFlashOffsetX := TContinueAttackSetting(Source).FFlashOffsetX;
    FFlashOffsetY := TContinueAttackSetting(Source).FFlashOffsetY;
    Changed;
  end;
end;

{------------------------------------------------------------------------------}

constructor TGroupAttackSetting.Create(AOwner:TDxGroupAttackProgress);
begin
  inherited Create;
  FOwner := AOwner;
  FImageType := Prguse_wil;

  FOwner := AOwner;

  FBackground := -1; // ±≥æ∞
  FProgress := -1;
  FFlashStart := -1; // …¡À∏ø™ ºÕº∆¨
  FFlashEnd := -1; // …¡À∏Ω· ¯Õº∆¨
  FFlashInterval := 200; // …¡À∏º‰∏Ù

  FBgOffsetX := 0;
  FBgOffsetY := 0;
  FPgOffsetX := 0;
  FPgOffsetY := 0;

  FContinueOffsetX := 0;
  FContinueOffsetY := 0;

  FImage := nil;
  FOnChange := nil;
  FOnGetImage := nil;
end;

procedure TGroupAttackSetting.SetImageType(Value:TImageType);
begin
  if FImageType <> Value then begin
    FImageType := Value;
    if Assigned(FOnGetImage) then
      FOnGetImage(Self, FImageType, TObject(FImage));
    Changed;
  end;
end;

procedure TGroupAttackSetting.SetOnGetImage(Value:TOnGetImage);
begin
  FOnGetImage := Value;
  if Assigned(FOnGetImage) then
    FOnGetImage(Self, FImageType, TObject(FImage));
  Changed;
end;

procedure TGroupAttackSetting.Changed;
begin
  if Assigned(FOnChange) then
    FOnChange(Self);
end;

procedure TGroupAttackSetting.Assign(Source:TPersistent);
begin
  if Source is TGroupAttackSetting then begin
    OnGetImage := TGroupAttackSetting(Source).OnGetImage;
    FImageType := TGroupAttackSetting(Source).FImageType;

    FBackground := TGroupAttackSetting(Source).FBackground;
    FProgress := TGroupAttackSetting(Source).FProgress;
    FFlashStart := TGroupAttackSetting(Source).FFlashStart;
    FFlashEnd := TGroupAttackSetting(Source).FFlashEnd;
    FFlashInterval := TGroupAttackSetting(Source).FFlashInterval;
    FBgOffsetX := TGroupAttackSetting(Source).FBgOffsetX;
    FBgOffsetY := TGroupAttackSetting(Source).FBgOffsetY;
    FPgOffsetX := TGroupAttackSetting(Source).FPgOffsetX;
    FPgOffsetY := TGroupAttackSetting(Source).FPgOffsetY;
    FContinueOffsetX := TGroupAttackSetting(Source).FContinueOffsetX;
    FContinueOffsetY := TGroupAttackSetting(Source).FContinueOffsetY;

    Changed;
  end;
end;

{------------------------------------------------------------------------------}

constructor TDxGroupAttackProgress.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
begin
  inherited Create(AOwner);

  FProgressAlignment := mbaBottom;

  FContinueSetting := TContinueAttackSetting.Create(Self);
  FGroupSetting := TGroupAttackSetting.Create(Self);
  FContinueAndGroupSetting := TGroupAttackSetting.Create(Self);

  FContinueSetting.OnGetImage := OnGetImage;
  FGroupSetting.OnGetImage := OnGetImage;
  FContinueAndGroupSetting.OnGetImage := OnGetImage;

  FShowContinueAttack := True;
  FShowGroupAttack := True;
  FGroupAttackProgress := 100;

  FContinueFlashTick := MyGetTickCount;
  FContinueFlashFrame := 0;

  FGroupFlashTick := MyGetTickCount;
  FGroupFlashFrame := 0;

  Width := 90;
  Height := 90;
end;

destructor TDxGroupAttackProgress.Destroy;
begin
  FContinueSetting.Free;
  FGroupSetting.Free;
  FContinueAndGroupSetting.Free;
  inherited Destroy;
end;

procedure TDxGroupAttackProgress.SetOnGetImage(Value:TOnGetImage);
begin
  inherited;
  FContinueSetting.OnGetImage := Value;
  FGroupSetting.OnGetImage := Value;
  FContinueAndGroupSetting.OnGetImage := Value;
end;

procedure TDxGroupAttackProgress.DoOnInRealArea(X, Y:Integer; var IsRealArea:Boolean);
begin
  inherited;
end;

procedure TDxGroupAttackProgress.Assign(Source:TDxControl);
begin
  if Source is TDxGroupAttackProgress then begin
    Left := Source.Left;
    Top := Source.Top;
    Width := Source.Width;
    Height := Source.Height;
    Enabled := Source.Enabled;
    Visible := Source.Visible;

    Transparent := Source.Transparent; //  «∑ÒÕ∏√˜
    EnableFocus := Source.EnableFocus; //  «∑Ò‘ –Ì…Ë÷√Ωπµ„
    Floating := Source.Floating; //  «∑Òø…“‘Õœ∂Ø
    OwnerMove := Source.OwnerMove;

    MouseEvents := Source.MouseEvents;

    //Designing := Source.Designing;                                                                    //  «∑Ò‘⁄…Ëº∆∆⁄
    ReferenceX := Source.ReferenceX;
    AdjustYByHeight := Source.AdjustYByHeight;
    TopAlignment := Source.TopAlignment;

    Center := Source.Center;
    Align := Source.Align;
    AutoSize := Source.AutoSize;
    BackgroundColor := Source.BackgroundColor;

    ImageIndex.Assign(Source.ImageIndex);
    BorderColor.Assign(Source.BorderColor);

    FContinueSetting.Assign(TDxGroupAttackProgress(Source).FContinueSetting);
    FGroupSetting.Assign(TDxGroupAttackProgress(Source).FGroupSetting);
    FContinueAndGroupSetting.Assign(TDxGroupAttackProgress(Source).FContinueAndGroupSetting);
  end;
end;

procedure TDxGroupAttackProgress.Paint;
var
  I:Integer;
  D:TTexture;
  vtRect, vbRect, PaintRect:TRect;
  boShowContinue, boShowGroup, boContinueFlash:Boolean;
  GroupProgress:Integer;

  ContinueOffsetX, ContinueOffsetY:Integer;
begin
  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := VirtualRect;

  boShowContinue := FShowContinueAttack;
  boShowGroup := FShowGroupAttack;
  GroupProgress := FGroupAttackProgress;
  boContinueFlash := True;

  if Assigned(FOnGetGroupAttackProgress) then
    FOnGetGroupAttackProgress(Self, boShowGroup, boShowContinue, GroupProgress, boContinueFlash);

  if GroupProgress < 0 then
    GroupProgress := 0
  else if GroupProgress > 100 then
    GroupProgress := 100;

  if (not boShowContinue) and (not boShowGroup) then Exit;

  ContinueOffsetX := 0;
  ContinueOffsetY := 0;

  if boShowContinue and boShowGroup then begin
    if (FContinueAndGroupSetting.Image <> nil) then begin
      ContinueOffsetX := FContinueAndGroupSetting.ContinueOffsetX;
      ContinueOffsetY := FContinueAndGroupSetting.ContinueOffsetY;

      if (FContinueAndGroupSetting.FBackground >= 0) then begin
        D := FContinueAndGroupSetting.Image.Images[FContinueAndGroupSetting.FBackground];

        if D <> nil then
          GameCanvas.Draw(vtRect.Left + FContinueAndGroupSetting.FBgOffsetX, vtRect.Top + FContinueAndGroupSetting.FBgOffsetY, D);
      end;

      if GroupProgress = 100 then begin
        if (FContinueAndGroupSetting.FFlashStart >= 0) and (FContinueAndGroupSetting.FFlashEnd >= FContinueAndGroupSetting.FFlashStart) then begin
          if FGroupFlashFrame < FContinueAndGroupSetting.FFlashStart then
            FGroupFlashFrame := FContinueAndGroupSetting.FFlashStart
          else if FGroupFlashFrame > FContinueAndGroupSetting.FFlashEnd then
            FGroupFlashFrame := FContinueAndGroupSetting.FFlashStart;

          if MyGetTickCount - FGroupFlashTick >= FContinueAndGroupSetting.FFlashInterval then begin
            FGroupFlashTick := MyGetTickCount;
            Inc(FGroupFlashFrame);

            if FGroupFlashFrame > FContinueAndGroupSetting.FFlashEnd then
              FGroupFlashFrame := FContinueAndGroupSetting.FFlashStart;
          end;

          D := FContinueAndGroupSetting.Image.Images[FGroupFlashFrame];
          if D <> nil then
            GameCanvas.Draw(vtRect.Left + FContinueAndGroupSetting.FPgOffsetX, vtRect.Top + FContinueAndGroupSetting.FPgOffsetY, D);
        end;
      end
      else if FContinueAndGroupSetting.FProgress >= 0 then begin
        D := FContinueAndGroupSetting.Image.Images[FContinueAndGroupSetting.FProgress];

        if D <> nil then begin
          PaintRect := D.ClientRect;

          case FProgressAlignment of
            mbaLeft:PaintRect.Right := MAX(Round(D.Width / 100 * GroupProgress), 0);
            mbaRight:PaintRect.Left := MAX(Round(D.Width / 100 * (100 - GroupProgress)), 0);
            mbaTop:PaintRect.Bottom := MAX(Round(D.Height / 100 * GroupProgress), 0);
            else //mbaBottom:
              PaintRect.Top := MAX(Round(D.Height / 100 * (100 - GroupProgress)), 0);
          end;

          GameCanvas.Draw(vtRect.Left + PaintRect.Left + FContinueAndGroupSetting.FPgOffsetX, vtRect.Top + PaintRect.Top + FContinueAndGroupSetting.FPgOffsetY, PaintRect, d);
        end;
      end;
    end;
  end
  else if boShowGroup then begin
    ContinueOffsetX := FGroupSetting.ContinueOffsetX;
    ContinueOffsetY := FGroupSetting.ContinueOffsetY;

    if (FGroupSetting.Image <> nil) then begin
      if (FGroupSetting.FBackground >= 0) then begin
        D := FGroupSetting.Image.Images[FGroupSetting.FBackground];

        if D <> nil then
          GameCanvas.Draw(vtRect.Left + FGroupSetting.FBgOffsetX, vtRect.Top + FGroupSetting.FBgOffsetY, D);
      end;

      if GroupProgress = 100 then begin
        if (FGroupSetting.FFlashStart >= 0) and (FGroupSetting.FFlashEnd >= FGroupSetting.FFlashStart) then begin
          if FGroupFlashFrame < FGroupSetting.FFlashStart then
            FGroupFlashFrame := FGroupSetting.FFlashStart
          else if FGroupFlashFrame > FGroupSetting.FFlashEnd then
            FGroupFlashFrame := FGroupSetting.FFlashStart;

          if MyGetTickCount - FGroupFlashTick >= FGroupSetting.FFlashInterval then begin
            FGroupFlashTick := MyGetTickCount;
            Inc(FGroupFlashFrame);

            if FGroupFlashFrame > FGroupSetting.FFlashEnd then
              FGroupFlashFrame := FGroupSetting.FFlashStart;
          end;

          D := FGroupSetting.Image.Images[FGroupFlashFrame];
          if D <> nil then
            GameCanvas.Draw(vtRect.Left + FGroupSetting.FPgOffsetX, vtRect.Top + FGroupSetting.FPgOffsetY, D);
        end;
      end
      else if FGroupSetting.FProgress >= 0 then begin
        D := FGroupSetting.Image.Images[FGroupSetting.FProgress];

        if D <> nil then begin
          PaintRect := D.ClientRect;

          case FProgressAlignment of
            mbaLeft:PaintRect.Right := MAX(Round(D.Width / 100 * GroupProgress), 0);
            mbaRight:PaintRect.Left := MAX(Round(D.Width / 100 * (100 - GroupProgress)), 0);
            mbaTop:PaintRect.Bottom := MAX(Round(D.Height / 100 * GroupProgress), 0);
            else //mbaBottom:
              PaintRect.Top := MAX(Round(D.Height / 100 * (100 - GroupProgress)), 0);
          end;

          GameCanvas.Draw(vtRect.Left + PaintRect.Left + FGroupSetting.FPgOffsetX, vtRect.Top + PaintRect.Top + FGroupSetting.FPgOffsetY, PaintRect, d);
        end;
      end;
    end;
  end;

  if boShowContinue and (FContinueSetting.Image <> nil) then begin
    if (FContinueSetting.FBackground >= 0) then begin
      D := FContinueSetting.Image.Images[FContinueSetting.FBackground];

      if D <> nil then
        GameCanvas.Draw(vtRect.Left + FContinueSetting.FBgOffsetX + ContinueOffsetX, vtRect.Top + FContinueSetting.FBgOffsetY + ContinueOffsetY, D);
    end;

    if boContinueFlash and (FContinueSetting.FFlashStart >= 0) and (FContinueSetting.FFlashEnd >= FGroupSetting.FFlashStart) then begin
      if FContinueFlashFrame < FContinueSetting.FFlashStart then
        FContinueFlashFrame := FContinueSetting.FFlashStart
      else if FContinueFlashFrame > FContinueSetting.FFlashEnd then
        FContinueFlashFrame := FContinueSetting.FFlashStart;

      if MyGetTickCount - FContinueFlashTick >= FContinueSetting.FFlashInterval then begin
        FContinueFlashTick := MyGetTickCount;
        Inc(FContinueFlashFrame);

        if FContinueFlashFrame > FContinueSetting.FFlashEnd then
          FContinueFlashFrame := FContinueSetting.FFlashStart;
      end;

      D := FContinueSetting.Image.Images[FContinueFlashFrame];
      if D <> nil then
        GameCanvas.Draw(vtRect.Left + FContinueSetting.FFlashOffsetX + ContinueOffsetX, vtRect.Top + FContinueSetting.FFlashOffsetY + ContinueOffsetY, D);
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

end.
