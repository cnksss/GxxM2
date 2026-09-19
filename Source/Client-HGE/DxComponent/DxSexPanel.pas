unit DxSexPanel;

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
  TDxSexPanel = class;

  TSexImageSetting = class(TPersistent)
  private
    FOwner:TDxSexPanel;

    FOnChange:TNotifyEvent;
    FOnGetImage:TOnGetImage;

    FImageType:TImageType; // 使用WIL类型
    FImage:TGameImages;

    FMale:Integer;
    FFemale:Integer;

    procedure SetImageType(Value:TImageType);
    procedure SetOnGetImage(Value:TOnGetImage);
    procedure SetMale(const Value:Integer);
    procedure SetFemale(const Value:Integer);
  protected
    procedure Changed; // dynamic;
  public
    constructor Create(AOwner:TDxSexPanel);
    procedure Assign(Source:TPersistent); override;

    property Image:TGameImages read FImage;
    property OnChange:TNotifyEvent read FOnChange write FOnChange;
    property OnGetImage:TOnGetImage read FOnGetImage write SetOnGetImage;
  published
    property ImageType:TImageType read FImageType write SetImageType;
    property Male:Integer read FMale write SetMale;
    property Female:Integer read FFemale write SetFemale;
  end;

  TDxSexPanel = class(TDxControl)
  private
    FIsMale:Boolean;
    FUseSetting2:Boolean;

    FSexImageSetting:TSexImageSetting;
    FSexImageSetting2:TSexImageSetting;

    procedure SetIsMale(const Value:Boolean);
    procedure SetUseSetting2(const Value:Boolean);

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
    property IsMale:Boolean read FIsMale write SetIsMale;
    property UseSetting2:Boolean read FUseSetting2 write SetUseSetting2;
    property SexImageSetting:TSexImageSetting read FSexImageSetting write FSexImageSetting;
    property SexImageSetting2:TSexImageSetting read FSexImageSetting2 write FSexImageSetting2;
    property AutoSize;
  end;

implementation

constructor TSexImageSetting.Create(AOwner:TDxSexPanel);
begin
  inherited Create;
  FOwner := AOwner;
  FImageType := Prguse_wil;

  FOwner := AOwner;
  FMale := -1;
  FFemale := -1;

  FImage := nil;
  FOnChange := nil;
  FOnGetImage := nil;
end;

procedure TSexImageSetting.SetImageType(Value:TImageType);
begin
  if FImageType <> Value then begin
    FImageType := Value;
    if Assigned(FOnGetImage) then
      FOnGetImage(Self, FImageType, TObject(FImage));
    Changed;
  end;
end;

procedure TSexImageSetting.SetOnGetImage(Value:TOnGetImage);
begin
  FOnGetImage := Value;
  if Assigned(FOnGetImage) then
    FOnGetImage(Self, FImageType, TObject(FImage));
  Changed;
end;

procedure TSexImageSetting.SetMale(const Value:Integer);
begin
  if FMale <> Value then begin
    FMale := Value;
    Changed;
  end;
end;

procedure TSexImageSetting.SetFemale(const Value:Integer);
begin
  if FFemale <> Value then begin
    FFemale := Value;
    Changed;
  end;
end;

procedure TSexImageSetting.Changed;
begin
  FOwner.RecallAutoSize;

  if Assigned(FOnChange) then
    FOnChange(Self);
end;

procedure TSexImageSetting.Assign(Source:TPersistent);
begin
  if Source is TSexImageSetting then begin
    OnGetImage := TSexImageSetting(Source).OnGetImage;
    FImageType := TSexImageSetting(Source).FImageType;

    FMale := TSexImageSetting(Source).FMale;
    FFemale := TSexImageSetting(Source).FFemale;

    Changed;
  end;
end;

{------------------------------------------------------------------------------}

constructor TDxSexPanel.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
begin
  inherited Create(AOwner);

  FIsMale := True;
  FUseSetting2 := False;

  FSexImageSetting := TSexImageSetting.Create(Self);
  FSexImageSetting.OnGetImage := OnGetImage;

  FSexImageSetting2 := TSexImageSetting.Create(Self);
  FSexImageSetting2.OnGetImage := OnGetImage;

  Width := 90;
  Height := 90;
end;

destructor TDxSexPanel.Destroy;
begin
  FSexImageSetting.Free;
  FSexImageSetting2.Free;
  inherited Destroy;
end;

procedure TDxSexPanel.Paint;
var
  I:Integer;
  D:TTexture;
  vtRect, vbRect, SrcRect:TRect;
  Setting:TSexImageSetting;
begin
  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := VirtualRect;

  if FUseSetting2 then
    Setting := FSexImageSetting2
  else
    Setting := FSexImageSetting;

  if Setting.Image <> nil then begin
    D := nil;

    //人物状态底图绘制， 分男女
    if IsMale then begin
      if Setting.FMale >= 0 then begin
        D := Setting.Image.Images[Setting.FMale];
      end;
    end else if (Setting.FFemale >= 0) then begin
      D := Setting.Image.Images[Setting.FFemale];
    end;

    if D <> nil then begin
      SrcRect := Rect(0, 0, Min(D.Width, Width), Min(D.Height, Height));
      GameCanvas.Draw(vtRect.Left, vtRect.Top, SrcRect, D);
    end;
  end;

  if Assigned(OnStartSubPaint) then
    OnStartSubPaint(Self); //衣服和武器在这绘制

  for I := ControlCount - 1 downto 0 do begin
    if not Visible then break;
    if (Control[I].Visible) then begin
      Control[I].Paint; //衣服和武器的绘制不在此处
    end;
  end;

  if Visible and Assigned(OnStopPaint) then
    OnStopPaint(Self);

  if Designing then begin
    FrameRect(vtRect, vtRect, vbRect, BorderColor.Up.Color);
  end;
end;

procedure TDxSexPanel.SetOnGetImage(Value:TOnGetImage);
begin
  inherited;
  FSexImageSetting.OnGetImage := Value;
  FSexImageSetting2.OnGetImage := Value;
end;

procedure TDxSexPanel.DoOnInRealArea(X, Y:Integer; var IsRealArea:Boolean);
begin
  inherited;
end;

procedure TDxSexPanel.SetIsMale(const Value:Boolean);
begin
  if FIsMale <> Value then begin
    FIsMale := Value;
    RecallAutoSize;
  end;
end;

procedure TDxSexPanel.SetUseSetting2(const Value:Boolean);
begin
  if FUseSetting2 <> Value then begin
    FUseSetting2 := Value;
    RecallAutoSize;
  end;
end;

procedure TDxSexPanel.RecallAutoSize;
var
  D:TTexture;
  Setting:TSexImageSetting;
begin
  if not AutoSize then Exit;

  if FUseSetting2 then
    Setting := FSexImageSetting2
  else
    Setting := FSexImageSetting;

  if Setting.Image <> nil then begin
    D := nil;
    if IsMale then begin
      if Setting.FMale >= 0 then begin
        D := Setting.Image.Images[Setting.FMale];
      end;
    end
    else if (Setting.FFemale >= 0) then begin
      D := Setting.Image.Images[Setting.FFemale];
    end;

    if (D <> nil) and (D.Width * D.Height > 4) then begin
      Width := D.Width;
      Height := D.Height;
    end;
  end;
end;

procedure TDxSexPanel.Assign(Source:TDxControl);
begin
  if Source is TDxSexPanel then begin
    Left := Source.Left;
    Top := Source.Top;
    Width := Source.Width;
    Height := Source.Height;
    Enabled := Source.Enabled;
    Visible := Source.Visible;

    Transparent := Source.Transparent; // 是否透明
    EnableFocus := Source.EnableFocus; // 是否允许设置焦点
    Floating := Source.Floating; // 是否可以拖动
    OwnerMove := Source.OwnerMove;

    MouseEvents := Source.MouseEvents;

    //Designing := Source.Designing;                                                                    // 是否在设计期
    ReferenceX := Source.ReferenceX;
    AdjustYByHeight := Source.AdjustYByHeight;
    TopAlignment := Source.TopAlignment;

    Center := Source.Center;
    Align := Source.Align;
    AutoSize := Source.AutoSize;
    BackgroundColor := Source.BackgroundColor;

    ImageIndex.Assign(Source.ImageIndex);
    BorderColor.Assign(Source.BorderColor);

    FSexImageSetting.Assign(TDxSexPanel(Source).FSexImageSetting);
    FSexImageSetting2.Assign(TDxSexPanel(Source).FSexImageSetting2);
  end;
end;

end.
