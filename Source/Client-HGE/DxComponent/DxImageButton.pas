unit DxImageButton;

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
  GameImages,
  DxComponents,
  DxControls;

type
  TDxImageButton = class;

  TButtonAnimation = class(TInterfacedPersistent)
  private
    FOwner:TDxImageButton;

    FOnChange:TNotifyEvent;
    FOnGetImage:TOnGetImage;
    FImageType:TImageType; // 使用WIL类型
    FImage:TGameImages;

    FShowType:TButtonAnimationShowType;

    FStartIndex:Integer; // 图片序号
    FEndIndex:Integer;
    FFrameTime:Integer;
    FPlayCount:Integer;

    FUseImageOffset:Boolean;
    FOffsetX:Integer;
    FOffsetY:Integer;

    FBlendDraw:Boolean;
    FDraw:Boolean; // 是否绘制
    FOutsideAreaDraw:Boolean;
    FDrawBeforeDef:Boolean;

    FCurrentFrame:Integer;
    FLastFrameTick:LongWord;
    FCurrentCount:Integer;

    procedure SetOnGetImage(const Value:TOnGetImage);
    procedure SetImageType(const Value:TImageType);
    procedure SetImage(const Value:TGameImages);
    procedure SetOutsideAreaDraw(const Value:Boolean);
    procedure SetDraw(const Value:Boolean);
    procedure SetBlendDraw(const Value:Boolean);
    procedure SetEndIndex(const Value:Integer);
    procedure SetFrameTime(const Value:Integer);
    procedure SetUseImageOffset(const Value:Boolean);
    procedure SetOffsetX(const Value:Integer);
    procedure SetOffsetY(const Value:Integer);
    procedure SetPlayCount(const Value:Integer);
    procedure SetStartIndex(const Value:Integer);
    procedure SetShowType(const Value:TButtonAnimationShowType);

    procedure Changed;
    procedure Paint;
  public
    constructor Create(AOnwer:TDxImageButton);
    destructor Destroy; override;
    procedure Assign(Source:TPersistent); override;
    property Image:TGameImages read FImage write SetImage;
    property OnChange:TNotifyEvent read FOnChange write FOnChange;
    property OnGetImage:TOnGetImage read FOnGetImage write SetOnGetImage;
  published
    property ImageType:TImageType read FImageType write SetImageType;

    property ShowType:TButtonAnimationShowType read FShowType write SetShowType;
    property StartIndex:Integer read FStartIndex write SetStartIndex;
    property EndIndex:Integer read FEndIndex write SetEndIndex;
    property FrameTime:Integer read FFrameTime write SetFrameTime;
    property PlayCount:Integer read FPlayCount write SetPlayCount;
    property OffsetX:Integer read FOffsetX write SetOffsetX;
    property OffsetY:Integer read FOffsetY write SetOffsetY;
    property UseImageOffset:Boolean read FUseImageOffset write SetUseImageOffset;

    property OutsideAreaDraw:Boolean read FOutsideAreaDraw write SetOutsideAreaDraw;
    property Draw:Boolean read FDraw write SetDraw;
    property BlendDraw:Boolean read FBlendDraw write SetBlendDraw;
    property DrawBeforeDef:Boolean read FDrawBeforeDef write FDrawBeforeDef;

    //property AnimationIndex: Integer read FAnimationIndex;
  end;

  TDxImageButton = class(TDxControl)
  private
    FClickSound:TClickSound;
    FOnClickSound:TOnClickSound;
    FButtonStyle:TButtonStyle;
    FChecked:Boolean;
    FCaptionColor:TDxCaptionColor;
    FCaptionDownOffsetX:Integer;
    FCaptionDownOffsetY:Integer;
    FButtonDownOffsetX:Integer;
    FButtonDownOffsetY:Integer;

    FCaptionOffsetX:Integer;
    FCaptionOffsetY:Integer;
    FDrawAligment:TDrawAligment;

    FExpandWidth:Integer;

    FAnimation:TButtonAnimation;

    FOnAnimationFrameChanged:TAnimationFrameChangedEvent;

    procedure SetChecked(Value:Boolean);
    procedure SetCaptionOffsetX(Value:Integer);
    procedure SetCaptionOffsetY(Value:Integer);
    procedure SetExpandWidth(Value:Integer);
  protected
    procedure CheckAutoSize(); override; //HZQ 20230609 测试用于解决微端模式下，Texture延迟加载导致的控件Size问题，
    procedure SetOnGetImage(Value:TOnGetImage); override;

    procedure DoMouseDown(); override;
    procedure DoMouseMove(); override;
    procedure DoMouseUp(); override;
    procedure DoClick(X, Y:Integer); override;
    procedure DoCaptionChange(); override;
    procedure DoDrawCaption; virtual;
  public
    function InRange(X, Y:Integer):Boolean; override;
    procedure MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); override;

  public
    constructor Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}); override;
    destructor Destroy(); override;
    procedure Paint; override;
    procedure Initialize; override;
    procedure Finalize; override;
    property OnClickSound:TOnClickSound read FOnClickSound write FOnClickSound;

    procedure Assign(Source:TDxControl); override;

    property OnAnimationFrameChanged:TAnimationFrameChangedEvent read FOnAnimationFrameChanged write FOnAnimationFrameChanged;
  published
    property Align;
    property Alignment;
    property Caption;
    property AutoSize;
    property BlendMode;
    property MouseDownBlendMode;
    property MouseMoveBlendMode;

    property ClickCount:TClickSound read FClickSound write FClickSound;
    property CaptionColor:TDxCaptionColor read FCaptionColor write FCaptionColor;
    property Style:TButtonStyle read FButtonStyle write FButtonStyle;
    property Checked:Boolean read FChecked write SetChecked;
    property CaptionDownOffsetX:Integer read FCaptionDownOffsetX write FCaptionDownOffsetX;
    property CaptionDownOffsetY:Integer read FCaptionDownOffsetY write FCaptionDownOffsetY;
    property ButtonDownOffsetX:Integer read FButtonDownOffsetX write FButtonDownOffsetX;
    property ButtonDownOffsetY:Integer read FButtonDownOffsetY write FButtonDownOffsetY;

    property CaptionOffsetX:Integer read FCaptionOffsetX write SetCaptionOffsetX;
    property CaptionOffsetY:Integer read FCaptionOffsetY write SetCaptionOffsetY;
    property DrawAligment:TDrawAligment read FDrawAligment write FDrawAligment;
    property ExpandWidth:Integer read FExpandWidth write SetExpandWidth;

    property Animation:TButtonAnimation read FAnimation write FAnimation;
  end;

implementation

uses Math,
  DxPopupMenu,
  HGEFontEx;

{ TButtonAnimation }

procedure TButtonAnimation.Assign(Source:TPersistent);
begin
  if Source is TButtonAnimation then begin
    OnGetImage := TButtonAnimation(Source).OnGetImage;
    ImageType := TButtonAnimation(Source).ImageType;

    FShowType := TButtonAnimation(Source).FShowType;

    FStartIndex := TButtonAnimation(Source).FStartIndex; // 图片序号
    FEndIndex := TButtonAnimation(Source).FEndIndex;
    FFrameTime := TButtonAnimation(Source).FFrameTime;
    FPlayCount := TButtonAnimation(Source).FPlayCount;

    FUseImageOffset := TButtonAnimation(Source).FUseImageOffset;
    FOffsetX := TButtonAnimation(Source).FOffsetX;
    FOffsetY := TButtonAnimation(Source).FOffsetY;

    FOutsideAreaDraw := TButtonAnimation(Source).FOutsideAreaDraw;
    FBlendDraw := TButtonAnimation(Source).FBlendDraw;
    FDraw := TButtonAnimation(Source).FDraw; // 是否绘制
    FDrawBeforeDef := TButtonAnimation(Source).DrawBeforeDef;

    FCurrentFrame := TButtonAnimation(Source).FCurrentFrame;
    FLastFrameTick := TButtonAnimation(Source).FLastFrameTick;
    FCurrentCount := TButtonAnimation(Source).FCurrentCount;

    Changed;
  end;
end;

procedure TButtonAnimation.Changed;
begin
  if Assigned(FOnChange) then
    FOnChange(Self);
end;

constructor TButtonAnimation.Create(AOnwer:TDxImageButton);
begin
  FOwner := AOnwer;
  FImageType := Prguse_wil; // 图库

  FShowType := astAlwaysShow;

  FStartIndex := -1; // 图片序号
  FEndIndex := -1;
  FFrameTime := 200;
  FPlayCount := 0;

  FUseImageOffset := True;
  FOffsetX := 0;
  FOffsetY := 0;

  FOutsideAreaDraw := False;
  FBlendDraw := False;
  FDraw := False; // 是否绘制
  FDrawBeforeDef := False;

  FCurrentFrame := 0;
  FLastFrameTick := MyGetTickCount;
  FCurrentCount := 0;
end;

destructor TButtonAnimation.Destroy;
begin

  inherited;
end;

procedure TButtonAnimation.Paint;
var
  D:TTexture;
  nX, nY:Integer;
  R, SrcRect, ParentRect:TRect;
  BlendMode:Integer;

  IsShow:Boolean;
begin  
  if FImage = nil then Exit;
  if (FStartIndex < 0) or (FEndIndex < 0) or (FStartIndex > FEndIndex) then Exit;
  if not FDraw then Exit;

  // stAlwaysShow, astNormalShow, astHotShow, astDownShow, astCheckShow, astEnableShow, astDisableShow
  if FShowType = astAlwaysShow then begin
    IsShow := True;
  end else if FShowType = astEnableShow then begin
    IsShow := FOwner.Enabled;
  end else if FShowType = astDisableShow then begin
    IsShow := not FOwner.Enabled;
  end else if FOwner.FButtonStyle = bsButton then begin
    if FOwner.MouseDowned then begin
      IsShow := FShowType = astDownShow;
    end else if FOwner.MouseMoveed then begin
      IsShow := FShowType = astHotShow;
    end else begin
      IsShow := FShowType = astNormalShow;
    end;
  end else begin
    if FOwner.MouseMoveed then begin
      if FOwner.Checked then begin
        IsShow := FShowType = astCheckShow;
      end
      else if FOwner.MouseDowned then begin
        IsShow := FShowType = astDownShow;
      end
      else begin
        IsShow := FShowType = astHotShow;
      end;
    end else begin
      if FOwner.Checked then begin
        IsShow := FShowType = astCheckShow;
      end else begin
        IsShow := FShowType = astNormalShow;
      end;
    end;
  end;

  if not IsShow then Exit;

  if FPlayCount > 0 then begin
    if FCurrentCount >= FPlayCount then Exit;
  end;

  if (FCurrentFrame < FStartIndex) or (FCurrentFrame > FEndIndex) then
    FCurrentFrame := FStartIndex;

  if MyGetTickCount - FLastFrameTick >= FFrameTime then begin
    Inc(FCurrentFrame);

    if (FCurrentFrame > FEndIndex) then begin
      FCurrentFrame := FStartIndex;

      if FPlayCount > 0 then begin
        Inc(FCurrentCount);
      end;

      if Assigned(FOwner.FOnAnimationFrameChanged) then
        FOwner.FOnAnimationFrameChanged(FOwner, 0, FCurrentCount, FCurrentFrame);
    end;

    FLastFrameTick := MyGetTickCount;
  end;

  D := FImage.GetCachedImage(FCurrentFrame, nX, nY);
  if D <> nil then begin
    ParentRect := FOwner.VirtualRect;
    SrcRect := D.ClientRect;

    if FUseImageOffset then begin
      R.Left := ParentRect.Left + FOffsetX + nX;
      R.Top := ParentRect.Top + FOffsetY + nY;
    end
    else begin
      R.Left := ParentRect.Left + FOffsetX;
      R.Top := ParentRect.Top + FOffsetY;
    end;

    if not FOutsideAreaDraw then begin
      R.Right := R.Left + D.Width;
      R.Bottom := R.Top + D.Height;

      if R.Left < ParentRect.Left then begin
        SrcRect.Left := SrcRect.Left + (ParentRect.Left - R.Left);
        R.Left := ParentRect.Left;
      end;

      if R.Right > ParentRect.Right then begin
        SrcRect.Right := SrcRect.Right - (R.Right - ParentRect.Right);
        R.Right := ParentRect.Right;
      end;

      if R.Top < ParentRect.Top then begin
        SrcRect.Top := SrcRect.Top + (ParentRect.Top - r.Top);
        R.Top := ParentRect.Top;
      end;

      if R.Bottom > ParentRect.Bottom then begin
        SrcRect.Bottom := SrcRect.Bottom - (R.Bottom - ParentRect.Bottom);
        R.Bottom := ParentRect.Bottom;
      end;

      if FBlendDraw then
        BlendMode := Blend_SrcAlphaColor
      else
        BlendMode := 2;

      GameCanvas.Draw(R.Left, R.Top, SrcRect, D, BlendMode);
    end else begin
      if FBlendDraw then
        BlendMode := Blend_SrcAlphaColor
      else
        BlendMode := 2;

      GameCanvas.Draw(R.Left, R.Top, D, BlendMode);
    end;
  end;
end;

procedure TButtonAnimation.SetOutsideAreaDraw(const Value:Boolean);
begin
  if FOutsideAreaDraw <> Value then
    FOutsideAreaDraw := Value;
end;

procedure TButtonAnimation.SetDraw(const Value:Boolean);
begin
  if FDraw <> Value then begin
    FDraw := Value;

    FCurrentFrame := FStartIndex;
    FLastFrameTick := MyGetTickCount;
    FCurrentCount := 0;
  end;
end;

procedure TButtonAnimation.SetBlendDraw(const Value:Boolean);
begin
  FBlendDraw := Value;
end;

procedure TButtonAnimation.SetEndIndex(const Value:Integer);
begin
  FEndIndex := Value;
end;

procedure TButtonAnimation.SetFrameTime(const Value:Integer);
begin
  FFrameTime := Value;
end;

procedure TButtonAnimation.SetImageType(const Value:TImageType);
begin
  if FImageType <> Value then begin
    FImageType := Value;
    if Assigned(FOnGetImage) then
      FOnGetImage(Self, FImageType, TObject(FImage));
    Changed;
  end;
end;

procedure TButtonAnimation.SetImage(const Value:TGameImages);
begin
  if FImage <> Value then begin
    FImage := Value;
    Changed;
  end;
end;

procedure TButtonAnimation.SetUseImageOffset(const Value:Boolean);
begin
  FUseImageOffset := Value;
end;

procedure TButtonAnimation.SetOffsetX(const Value:Integer);
begin
  FOffsetX := Value;
end;

procedure TButtonAnimation.SetOffsetY(const Value:Integer);
begin
  FOffsetY := Value;
end;

procedure TButtonAnimation.SetOnGetImage(const Value:TOnGetImage);
begin
  FOnGetImage := Value;
  if Assigned(FOnGetImage) then
    FOnGetImage(Self, FImageType, TObject(FImage));
  Changed;
end;

procedure TButtonAnimation.SetPlayCount(const Value:Integer);
begin
  FPlayCount := Value;
end;

procedure TButtonAnimation.SetStartIndex(const Value:Integer);
begin
  FStartIndex := Value;
end;

procedure TButtonAnimation.SetShowType(const Value:TButtonAnimationShowType);
begin
  FShowType := Value;
end;

//-----------------------------------------------------------------------------------------

constructor TDxImageButton.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
begin
  inherited Create(AOwner);
  FAnimation := TButtonAnimation.Create(Self);

  AutoSize := True;
  Width := 100;
  Height := 20;
  Alignment := taCenter;
  FClickSound := csNone;
  FButtonStyle := bsButton;
  FChecked := False;
  FCaptionColor := TDxCaptionColor.Create;
  FCaptionDownOffsetX := 1;
  FCaptionDownOffsetY := 1;
  FButtonDownOffsetX := 0;
  FButtonDownOffsetY := 0;

  FDrawAligment := daFill;
  FCaptionOffsetX := 0;
  FCaptionOffsetY := 0;
  FExpandWidth := 0;
  GuiType := t_Button;
end;

destructor TDxImageButton.Destroy();
begin
  FCaptionColor.Free;
  FAnimation.Free;
  inherited Destroy();
end;

procedure TDxImageButton.SetOnGetImage(Value:TOnGetImage);
begin
  inherited;
  FAnimation.OnGetImage := Value;
end;

procedure TDxImageButton.CheckAutoSize();
begin
    if Self.Style = bsButton then begin //TButtonStyle = (bsButton, bsRadio, bsCheckBox);
        inherited CheckAutoSize;
    end;
end;

procedure TDxImageButton.MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
var
  vRect:TRect;
begin
  if (PopupMenu <> nil) then begin
    if PopupMenu.RootCtrl <> nil then begin
      vRect := VirtualRect;
      if vRect.Bottom + PopupMenu.Height > PopupMenu.RootCtrl.Height then
        PopupMenu.Top := vRect.Top - PopupMenu.Height
      else
        PopupMenu.Top := vRect.Bottom;
    end;
    TDxPopupMenu(PopupMenu).ItemIndex := -1;
    PopupMenu.Left := vRect.Left;
    PopupMenu.Width := Width;
    PopupMenu.Visible := True;
  end;
  inherited MouseDown(Button, Shift, X, Y);
end;

procedure TDxImageButton.DoDrawCaption;
var
  X, Y:Integer;
  Font:TDxFont;
  HGEFont:THGEFont;
  TextImages:TImageInfos;
  I, nWidth, nHeight:Integer;
  vtRect:TRect;
  vbRect:TRect;
begin
  if Caption = '' then Exit;

  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := VirtualRect;
  X := 0;
  Y := 0;
  if Enabled then begin
    if MouseDowned {or Checked} then begin
      X := FCaptionOffsetX + FCaptionDownOffsetX;
      Y := FCaptionOffsetY + FCaptionDownOffsetY;
      Font := CaptionColor.Down;
    end
    else begin
      X := FCaptionOffsetX;
      Y := FCaptionOffsetY;

      if MouseMoveed then
        Font := CaptionColor.Hot
      else
        Font := CaptionColor.Up;
    end;
  end
  else
    Font := CaptionColor.Disabled;
  // vtRect := VirtualRect;

  {
  if FButtonStyle <> bsButton then
  begin
    X := 0;
    Y := 0;
  end;
  }

  HGEFont := TextureFonts.FindFont(Font.Name, Font.Size, Font.Style);

  if HGEFont <> nil then begin
    TextImages := HGEFont.GetImageInfos(Caption);

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

    DrawCaption(HGEFont,
      Font, TextImages,
      Bounds(vtRect.Left, vtRect.Top, nWidth, nHeight), vbRect, vtRect, X, Y, 3);
  end;
end;

procedure TDxImageButton.DoMouseDown();
begin

end;

procedure TDxImageButton.DoMouseMove();
begin

end;

procedure TDxImageButton.DoMouseUp();
begin

end;

procedure TDxImageButton.Assign(Source:TDxControl);
begin
  if Source is TDxImageButton then begin
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
    BlendMode := TDxImageButton(Source).BlendMode;

    ImageIndex.Assign(Source.ImageIndex);
    BorderColor.Assign(Source.BorderColor);

    ButtonDownOffsetX := TDxImageButton(Source).ButtonDownOffsetX;
    ButtonDownOffsetY := TDxImageButton(Source).ButtonDownOffsetY;

    Caption := TDxImageButton(Source).Caption;
    Alignment := TDxImageButton(Source).Alignment;

    CaptionColor.Assign(TDxImageButton(Source).CaptionColor);
    CaptionDownOffsetX := TDxImageButton(Source).CaptionDownOffsetX;
    CaptionDownOffsetY := TDxImageButton(Source).CaptionDownOffsetY;

    CaptionOffsetX := TDxImageButton(Source).CaptionOffsetX;
    CaptionOffsetY := TDxImageButton(Source).CaptionOffsetY;

    Checked := TDxImageButton(Source).Checked;
    ClickCount := TDxImageButton(Source).ClickCount;
    DrawBorder := TDxImageButton(Source).DrawBorder;
    Hint := TDxImageButton(Source).Hint;
    ModalControl := TDxImageButton(Source).ModalControl;
    MouseDownBlendMode := TDxImageButton(Source).MouseDownBlendMode;
    MouseMoveBlendMode := TDxImageButton(Source).MouseMoveBlendMode;
    Style := TDxImageButton(Source).Style;

    Animation.Assign(TDxImageButton(Source).Animation);
  end;
end;

procedure TDxImageButton.DoCaptionChange();
var
  I, FaceIndex, nWidth, nHeight:Integer;
  D:TTexture;
  HGEFont:THGEFont;
  TextImages:TImageInfos;
begin
  if AutoSize then begin
    D := nil;
    if ImageIndex.Image <> nil then begin
      if FButtonStyle = bsButton then begin
        if MouseDowned then begin
          FaceIndex := ImageIndex.Down;
          if (FaceIndex < 0) and (ImageIndex.Up >= 0) then
            FaceIndex := ImageIndex.Up;
        end
        else if MouseMoveed then begin
          FaceIndex := ImageIndex.Hot;
          if (FaceIndex < 0) and (ImageIndex.Up >= 0) then
            FaceIndex := ImageIndex.Up;
        end
        else
          FaceIndex := ImageIndex.Up;
      end
      else begin
        // boDrawBorder := ImageIndex.Up < 0;
        if MouseMoveed then begin
          if Checked then begin
            if ImageIndex.Checked >= 0 then
              FaceIndex := ImageIndex.Checked
            else
              FaceIndex := ImageIndex.Down
          end

            // add chongchong 2015-08-13
          else if MouseDowned and (ImageIndex.Down >= 0) and (ImageIndex.Checked >= 0) then
            FaceIndex := ImageIndex.Down

          else if ImageIndex.Hot >= 0 then
            FaceIndex := ImageIndex.Hot
          else
            FaceIndex := ImageIndex.Up;
        end
        else begin
          if Checked then begin
            if ImageIndex.Checked >= 0 then
              FaceIndex := ImageIndex.Checked
            else
              FaceIndex := ImageIndex.Down
          end
          else
            FaceIndex := ImageIndex.Up;
        end;
      end;

      if FaceIndex >= 0 then begin
        D := ImageIndex.Image.Images[FaceIndex];
        {if Texture <> nil then begin
          vRect := vtRect;
          if Alignment = taLeftJustify then begin
            vRect.Left := vRect.Right - Texture.Width;
          end;

        end; }
      end;
    end;

    if Caption <> '' then begin
      if (FButtonStyle = bsButton) and (D <> nil) and (D.Width >= 2) and (D.Height >= 2) then begin
        nWidth := D.Width;
        nHeight := D.Height;
      end
      else begin
        nWidth := 0;
        nHeight := 0;
        HGEFont := TextureFonts.FindFont(CaptionColor.Up.Name, CaptionColor.Up.Size, CaptionColor.Up.Style);
        if HGEFont <> nil then begin
          TextImages := HGEFont.GetImageInfos(Caption);
          for I := 0 to Length(TextImages) - 1 do begin
            if nWidth < TextImages[I].Width then
              nWidth := TextImages[I].Width;

            if nHeight < TextImages[I].Height then
              nHeight := TextImages[I].Height;
          end;

          if CaptionColor.Up.Bold then begin
            Inc(nWidth, 2);
            Inc(nHeight, 2);
          end;
        end;

        if (D <> nil) then begin
          Inc(nWidth, 1);
          Inc(nWidth, D.Width);
          nHeight := Max(nHeight, D.Height);
        end;

        Inc(nWidth, FExpandWidth);
      end;
      Width := nWidth;
      Height := nHeight;
    end
    else begin
      if D <> nil then begin
        Width := D.Width;
        Height := D.Height;
      end;
    end;
  end;
end;

procedure TDxImageButton.Initialize;
begin

end;

procedure TDxImageButton.Finalize;
begin

end;

procedure TDxImageButton.SetChecked(Value:Boolean);
var
  I:Integer;
  D:TDxControl;
begin
  case FButtonStyle of
    bsRadio:begin // 单选按钮
        if Value then begin
          if (Owner <> nil) then begin
            for I := 0 to TDxControl(Owner).ControlCount - 1 do begin
              D := TDxControl(Owner).Control[I];
              if (D is TDxImageButton) and (TDxImageButton(D).Style = bsRadio) then begin
                TDxImageButton(D).Checked := False;
              end;
            end;
          end;
          FChecked := True;
        end
        else
          FChecked := Value;
      end;
    else
      FChecked := Value;
  end;
end;

procedure TDxImageButton.SetCaptionOffsetX(Value:Integer);
begin
  if FCaptionOffsetX <> Value then begin
    FCaptionOffsetX := Value;
    DoCaptionChange;
  end;
end;

procedure TDxImageButton.SetCaptionOffsetY(Value:Integer);
begin
  if FCaptionOffsetY <> Value then begin
    FCaptionOffsetY := Value;
    DoCaptionChange;
  end;
end;

procedure TDxImageButton.SetExpandWidth(Value:Integer);
begin
  if FExpandWidth <> Value then begin
    FExpandWidth := Value;
    DoCaptionChange;
  end;
end;

function TDxImageButton.InRange(X, Y:Integer):Boolean;
var
  boInrange:Boolean;
  vRect:TRect;
begin
  if FButtonStyle = bsButton then begin
    Result := inherited InRange(X, Y);
  end
  else begin
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
end;

procedure TDxImageButton.DoClick(X, Y:Integer);
var
  I:Integer;
  D:TDxControl;
begin
  if CanMouse then begin
    if not Designing then begin
      case FButtonStyle of
        bsRadio:begin // 单选按钮
            if (Owner <> nil) then begin
              for I := 0 to TDxControl(Owner).ControlCount - 1 do begin
                D := TDxControl(Owner).Control[I];
                if (D is TDxImageButton) and (TDxImageButton(D).Style = bsRadio) then begin
                  TDxImageButton(D).Checked := False;
                end;
              end;
            end;
            if not Checked then begin
              Checked := True;
              if Assigned(FOnClickSound) then FOnClickSound(Self, FClickSound);
              inherited;
            end;

          end;
        bsCheckBox:begin // 多选按钮
            Checked := not Checked;
            if Assigned(FOnClickSound) then FOnClickSound(Self, FClickSound);
            inherited;
          end;
        else begin
            if Assigned(FOnClickSound) then FOnClickSound(Self, FClickSound);
            inherited;
          end;
      end;
    end
    else
      inherited;
  end;
end;

procedure TDxImageButton.Paint;
var
  FaceIndex:Integer;
  Texture:TTexture;
  vRect:TRect;
  vtRect:TRect;
  vbRect:TRect;
begin
  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := VirtualRect;
  DoPaint();

  if Designing then begin
    // Canvas.FillRectAlpha(vbRect, BackgroundColor, 150);
    FrameRect(vtRect, vtRect, vbRect, BorderColor.Up.Color);
    // CurrentFont.TextOut(vtRect.Left + 1, vtRect.Top + 1, Format('Left:%d Top:%d', [vtRect.Left, vtRect.Top]));
  end;

  if FAnimation.DrawBeforeDef then
    FAnimation.Paint;

  if Assigned(OnStartPaint) then
    OnStartPaint(Self);

  if Assigned(OnPaint) then
    OnPaint(Self)
  else if ImageIndex.Image <> nil then begin
    // if Enabled then begin
    if FButtonStyle = bsButton then begin
      if MouseDowned then begin
        FaceIndex := ImageIndex.Down;
        if (FaceIndex < 0) and (ImageIndex.Up >= 0) then
          FaceIndex := ImageIndex.Up;
      end else if MouseMoveed then begin
        FaceIndex := ImageIndex.Hot;
        if (FaceIndex < 0) and (ImageIndex.Up >= 0) then
          FaceIndex := ImageIndex.Up;
      end else begin
        FaceIndex := ImageIndex.Up;
      end;
    end else begin
      // boDrawBorder := ImageIndex.Up < 0;
      if MouseMoveed then begin
        if Checked then begin
          if ImageIndex.Checked >= 0 then
            FaceIndex := ImageIndex.Checked
          else
            FaceIndex := ImageIndex.Down
        end else if MouseDowned and (ImageIndex.Down >= 0) and (ImageIndex.Checked >= 0) then // add chongchong 2015-08-13
          FaceIndex := ImageIndex.Down

        else if ImageIndex.Hot >= 0 then
          FaceIndex := ImageIndex.Hot
        else
          FaceIndex := ImageIndex.Up;
      end else begin
        if Checked then begin
          if ImageIndex.Checked >= 0 then
            FaceIndex := ImageIndex.Checked
          else
            FaceIndex := ImageIndex.Down
        end else begin
          FaceIndex := ImageIndex.Up;
        end;
      end;
    end;
    // end else begin
    // FaceIndex := ImageIndex.Disabled;
    // end;
    if FaceIndex >= 0 then begin
      if Enabled then
        Texture := ImageIndex.Image.Images[FaceIndex]
      else
        Texture := ImageIndex.Image.Grays[FaceIndex];

      if Texture <> nil then begin
        vRect := vtRect;
        if Alignment = taLeftJustify then begin
          vRect.Left := vRect.Right - Texture.Width;
        end;

        if (FButtonStyle = bsButton) and MouseDowned then begin
          vRect.Left := vRect.Left + FCaptionOffsetX + FButtonDownOffsetX;
          vRect.Top := vRect.Top + FCaptionOffsetY + FButtonDownOffsetY;
        end;

        // 加入透明模式绘制 chongchong 2014-09-15
        if FDrawAligment = daFill then
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

  DoDrawCaption;

  if not FAnimation.DrawBeforeDef then
    FAnimation.Paint;

  if Assigned(OnStopPaint) then
    OnStopPaint(Self);
end;

end.
