unit DxSwitchButton;

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
  GameImages;

type
  TDxSwitchButton = class;

  TButtonSetting = class(TPersistent)
  private
    FOwner:TDxSwitchButton;

    FImageIndex:TDxImageIndex;
    FCaption:TCaption;
    FAlignment:TAlignment;

    FClickSound:TClickSound;

    FCaptionColor:TDxCaptionColor;

    FCaptionOffsetX:Integer;
    FCaptionOffsetY:Integer;

    FCaptionDownOffsetX:Integer;
    FCaptionDownOffsetY:Integer;
    FButtonDownOffsetX:Integer;
    FButtonDownOffsetY:Integer;

    FDrawAligment:TDrawAligment;

    //FExpandWidth: Integer;

    procedure Changed(Sender:TObject); stdcall;
  protected
    procedure DoChanged; virtual;

    procedure SetCaptionOffsetX(Value:Integer);
    procedure SetCaptionOffsetY(Value:Integer);
    //procedure SetExpandWidth(Value: Integer);
  public
    constructor Create(AOwner:TDxSwitchButton);
    destructor Destroy(); override;
    procedure Assign(Source:TPersistent); override;
  published
    property ImageIndex:TDxImageIndex read FImageIndex write FImageIndex;
    property ClickSound:TClickSound read FClickSound write FClickSound;

    property Caption:TCaption read FCaption write FCaption;
    property Alignment:TAlignment read FAlignment write FAlignment;
    property CaptionColor:TDxCaptionColor read FCaptionColor;

    property CaptionOffsetX:Integer read FCaptionOffsetX write SetCaptionOffsetX;
    property CaptionOffsetY:Integer read FCaptionOffsetY write SetCaptionOffsetY;
    property CaptionDownOffsetX:Integer read FCaptionDownOffsetX write FCaptionDownOffsetX;
    property CaptionDownOffsetY:Integer read FCaptionDownOffsetY write FCaptionDownOffsetY;

    property ButtonDownOffsetX:Integer read FButtonDownOffsetX write FButtonDownOffsetX;
    property ButtonDownOffsetY:Integer read FButtonDownOffsetY write FButtonDownOffsetY;

    property DrawAligment:TDrawAligment read FDrawAligment write FDrawAligment;

    // property ExpandWidth: Integer read FExpandWidth write SetExpandWidth;
  end;

  TDxSwitchButton = class(TDxControl)
  private
    FIsOpen:Boolean;

    FCloseSetting:TButtonSetting;
    FOpenSetting:TButtonSetting;

    procedure SetIsOpen(const Value:Boolean);

    procedure RecallAutoSize;
  protected
    procedure DoClick(X, Y:Integer); override;

    procedure DoOnInRealArea(X, Y:Integer; var IsRealArea:Boolean); override;
    procedure SetOnGetImage(Value:TOnGetImage); override;
  public
    constructor Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}); override;
    destructor Destroy; override;
    procedure Assign(Source:TDxControl); override;
    procedure Paint; override;
  published
    property IsOpen:Boolean read FIsOpen write SetIsOpen;
    property CloseSetting:TButtonSetting read FCloseSetting write FCloseSetting;
    property OpenSetting:TButtonSetting read FOpenSetting write FOpenSetting;
    property AutoSize;
  end;

implementation

uses Math,
  DxPopupMenu,
  HGEFontEx;

{ TButtonSetting }

constructor TButtonSetting.Create(AOwner:TDxSwitchButton);
begin
  FOwner := AOwner;

  FImageIndex := TDxImageIndex.Create;
  FImageIndex.OnChange := Changed;

  FCaption := '';
  FAlignment := taCenter;
  FClickSound := csNone;

  FCaptionColor := TDxCaptionColor.Create;
  FCaptionColor.OnChange := Changed;

  FCaptionOffsetX := 0;
  FCaptionOffsetY := 0;
  FCaptionDownOffsetX := 1;
  FCaptionDownOffsetY := 1;

  FButtonDownOffsetX := 0;
  FButtonDownOffsetY := 0;

  FDrawAligment := daFill;
  //FExpandWidth := 0;
end;

destructor TButtonSetting.Destroy;
begin
  FImageIndex.Free;
  FCaptionColor.Free;
  inherited;
end;

procedure TButtonSetting.Assign(Source:TPersistent);
begin
  //if not (Source is InheritsFrom(TPersistent)) then begin
  //    inherited; //HZQ 20230621 白屏修复工具没有这句
  //end;

  if Source is TButtonSetting then begin
    FImageIndex.Assign(TButtonSetting(Source).FImageIndex);
    FCaption := TButtonSetting(Source).FCaption;
    FClickSound := TButtonSetting(Source).FClickSound;

    FCaptionColor.Assign(TButtonSetting(Source).FCaptionColor);

    FCaptionOffsetX := TButtonSetting(Source).FCaptionOffsetX;
    FCaptionOffsetY := TButtonSetting(Source).FCaptionOffsetY;

    FCaptionDownOffsetX := TButtonSetting(Source).CaptionDownOffsetX;
    FCaptionDownOffsetY := TButtonSetting(Source).CaptionDownOffsetY;

    FButtonDownOffsetX := TButtonSetting(Source).FButtonDownOffsetX;
    FButtonDownOffsetY := TButtonSetting(Source).FButtonDownOffsetY;

    FDrawAligment := TButtonSetting(Source).FDrawAligment;

    //FExpandWidth := TButtonSetting(Source).FExpandWidth;

    DoChanged;
  end;
end;

procedure TButtonSetting.SetCaptionOffsetX(Value:Integer);
begin
  if FCaptionOffsetX <> Value then begin
    FCaptionOffsetX := Value;
    DoChanged;
  end;
end;

procedure TButtonSetting.SetCaptionOffsetY(Value:Integer);
begin
  if FCaptionOffsetY <> Value then begin
    FCaptionOffsetY := Value;
    DoChanged;
  end;
end;

{
procedure TButtonSetting.SetExpandWidth(Value: Integer);
begin
  if FExpandWidth <> Value then
  begin
    FExpandWidth := Value;
    DoChanged;
  end;
end;
}

procedure TButtonSetting.Changed(Sender:TObject);
begin
  DoChanged;
end;

procedure TButtonSetting.DoChanged;
begin
  FOwner.RecallAutoSize;
end;

{------------------------------------------------------------------------------}

constructor TDxSwitchButton.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
begin
  inherited Create(AOwner);

  FIsOpen := False;

  FCloseSetting := TButtonSetting.Create(Self);
  FCloseSetting.FImageIndex.OnGetImage := OnGetImage;

  FOpenSetting := TButtonSetting.Create(Self);
  FOpenSetting.FImageIndex.OnGetImage := OnGetImage;

  Width := 100;
  Height := 20;
end;

destructor TDxSwitchButton.Destroy;
begin
  FCloseSetting.Free;
  FOpenSetting.Free;
  inherited Destroy;
end;

procedure TDxSwitchButton.Paint;
var
  I, nWidth, nHeight:Integer;
  FaceIndex:Integer;
  Texture:TTexture;
  Font:TDxFont;
  vRect:TRect;
  vtRect:TRect;
  vbRect:TRect;
  X, Y:Integer;
  HGEFont:THGEFont;
  TextImages:TImageInfos;

  Setting:TButtonSetting;
begin
  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := VirtualRect;

  DoPaint();

  if FIsOpen then
    Setting := FOpenSetting
  else
    Setting := FCloseSetting;

  if Designing then begin
    FrameRect(vtRect, vtRect, vbRect, BorderColor.Up.Color);
  end;

  if Assigned(OnStartPaint) then
    OnStartPaint(Self);

  if Assigned(OnPaint) then
    OnPaint(Self)
  else if Setting.ImageIndex.Image <> nil then begin

    if MouseDowned then begin
      FaceIndex := Setting.ImageIndex.Down;
      if (FaceIndex < 0) and (Setting.ImageIndex.Up >= 0) then
        FaceIndex := Setting.ImageIndex.Up;
    end
    else if MouseMoveed then begin
      FaceIndex := Setting.ImageIndex.Hot;
      if (FaceIndex < 0) and (Setting.ImageIndex.Up >= 0) then
        FaceIndex := Setting.ImageIndex.Up;
    end
    else
      FaceIndex := Setting.ImageIndex.Up;

    if FaceIndex >= 0 then begin
      if Enabled then
        Texture := Setting.ImageIndex.Image.Images[FaceIndex]
      else
        Texture := Setting.ImageIndex.Image.Grays[FaceIndex];
      if Texture <> nil then begin
        vRect := vtRect;
        if Alignment = taLeftJustify then begin
          vRect.Left := vRect.Right - Texture.Width;
        end;

        if MouseDowned then begin
          vRect.Left := vRect.Left + Setting.FCaptionOffsetX + Setting.FButtonDownOffsetX;
          vRect.Top := vRect.Top + Setting.FCaptionOffsetY + Setting.FButtonDownOffsetY;
        end;

        // 加入透明模式绘制 chongchong 2014-09-15
        if Setting.FDrawAligment = daFill then
          DrawRect(vRect, vtRect, vbRect, Texture, BlendMode)
        else begin
          if vRect.Bottom - vtRect.Top > Texture.Height then begin
            vRect.Top := vRect.Top + (vRect.Bottom - vRect.Top - Texture.Height);
          end;
          DrawRect(vRect, vtRect, vbRect, Texture, BlendMode)
        end;
      end;
    end;
  end;

  if Setting.Caption <> '' then begin
    X := 0;
    Y := 0;
    if Enabled then begin
      if MouseDowned {or Checked} then begin
        X := Setting.FCaptionOffsetX + Setting.FCaptionDownOffsetX;
        Y := Setting.FCaptionOffsetY + Setting.FCaptionDownOffsetY;
        Font := Setting.CaptionColor.Down;
      end
      else begin
        X := Setting.FCaptionOffsetX;
        Y := Setting.FCaptionOffsetY;

        if MouseMoveed then
          Font := Setting.CaptionColor.Hot
        else
          Font := Setting.CaptionColor.Up;
      end;
    end
    else
      Font := Setting.CaptionColor.Disabled;

    HGEFont := TextureFonts.FindFont(Font.Name, Font.Size, Font.Style);
    if HGEFont <> nil then begin
      TextImages := HGEFont.GetImageInfos(Setting.Caption);

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

      if Font.Bold then begin
        Inc(nWidth, 2);
        Inc(nHeight, 2);
      end;

      DrawCaption(HGEFont, Font, Setting.FAlignment, TextImages, Bounds(vtRect.Left, vtRect.Top, nWidth, nHeight), vbRect, vtRect, X, Y);
    end;
  end;

  if Assigned(OnStopPaint) then
    OnStopPaint(Self);
end;

procedure TDxSwitchButton.SetOnGetImage(Value:TOnGetImage);
begin
  inherited;
  FCloseSetting.ImageIndex.OnGetImage := Value;
  FOpenSetting.ImageIndex.OnGetImage := Value;
end;

procedure TDxSwitchButton.DoOnInRealArea(X, Y:Integer; var IsRealArea:Boolean);
begin
  inherited;
end;

procedure TDxSwitchButton.SetIsOpen(const Value:Boolean);
begin
  if FIsOpen <> Value then begin
    FIsOpen := Value;
    RecallAutoSize;
  end;
end;

procedure TDxSwitchButton.RecallAutoSize;
var
  D:TTexture;

  FaceIndex:Integer;
  Setting:TButtonSetting;
begin
  if not AutoSize then Exit;

  if FIsOpen then
    Setting := FOpenSetting
  else
    Setting := FCloseSetting;

  D := nil;
  if Setting.ImageIndex.Image <> nil then begin
    if MouseDowned then begin
      FaceIndex := Setting.ImageIndex.Down;
      if (FaceIndex < 0) and (Setting.ImageIndex.Up >= 0) then
        FaceIndex := Setting.ImageIndex.Up;
    end
    else if MouseMoveed then begin
      FaceIndex := Setting.ImageIndex.Hot;
      if (FaceIndex < 0) and (Setting.ImageIndex.Up >= 0) then
        FaceIndex := Setting.ImageIndex.Up;
    end
    else
      FaceIndex := Setting.ImageIndex.Up;

    if FaceIndex >= 0 then begin
      D := Setting.ImageIndex.Image.Images[FaceIndex];
    end;
  end;

  if D <> nil then begin
    Width := D.Width;
    Height := D.Height;
  end;
end;

procedure TDxSwitchButton.Assign(Source:TDxControl);
begin
  if Source is TDxSwitchButton then begin
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

    FCloseSetting.Assign(TDxSwitchButton(Source).FCloseSetting);
    FOpenSetting.Assign(TDxSwitchButton(Source).FOpenSetting);
  end;
end;

procedure TDxSwitchButton.DoClick(X, Y:Integer);
begin
  if CanMouse then begin
    if not Designing then begin
      FIsOpen := not FIsOpen;
      inherited;
    end
    else
      inherited;
  end;
end;

end.
