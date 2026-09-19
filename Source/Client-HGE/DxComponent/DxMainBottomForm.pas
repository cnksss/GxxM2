unit DxMainBottomForm;

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
  HGECanvas;

type
  TDxMainBottomForm = class;

  TMainBottonAnimation = class(TInterfacedPersistent)
  private
    FOwner:TDxMainBottomForm;
    FAnimationIndex:Integer;

    FOnChange:TNotifyEvent;
    FOnGetImage:TOnGetImage;
    FImageType:TImageType; // 使用WIL类型
    FImage:TGameImages;

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

    FHorzAlignment:TAlignment;
    FVertAlignment:TVerticalAlignment;
    FAdjustYByHeight:Boolean;

    procedure SetOnGetImage(const Value:TOnGetImage);
    procedure SetImageType(const Value:TImageType);
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

    procedure Changed;
    procedure Paint;
  public
    constructor Create(AOnwer:TDxMainBottomForm);
    destructor Destroy; override;
    procedure Assign(Source:TPersistent); override;
    property Image:TGameImages read FImage;
    property OnChange:TNotifyEvent read FOnChange write FOnChange;
    property OnGetImage:TOnGetImage read FOnGetImage write SetOnGetImage;

    property AnimationIndex:Integer read FAnimationIndex;
  published
    property ImageType:TImageType read FImageType write SetImageType;

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

    property HorzAlignment:TAlignment read FHorzAlignment write FHorzAlignment;
    property VertAlignment:TVerticalAlignment read FVertAlignment write FVertAlignment;
    property AdjustYByHeight:Boolean read FAdjustYByHeight write FAdjustYByHeight;
  end;

  TDxMainBottomImage = class(TPersistent)
  private
    FOnChange:TNotifyEvent;
    FOnGetImage:TOnGetImage;
    FImageType:TImageType; // 使用WIL类型
    FImage:TGameImages;
    FIndex:Integer;
    procedure SetImageType(Value:TImageType);
    procedure SetOnGetImage(Value:TOnGetImage);
    procedure SetImage(Value:TGameImages);
    procedure SetIndex(Value:Integer);
  protected
    procedure Changed; // dynamic;
  public
    constructor Create;
    procedure Assign(Source:TPersistent); override;
    property Image:TGameImages read FImage write SetImage;
    property OnChange:TNotifyEvent read FOnChange write FOnChange;
    property OnGetImage:TOnGetImage read FOnGetImage write SetOnGetImage;
  published
    property ImageType:TImageType read FImageType write SetImageType;
    property Index:Integer read FIndex write SetIndex;
  end;

  TDxMainBottomStretchImage = class(TPersistent)
  private
    FOwner:TDxMainBottomForm;

    FOnChange:TNotifyEvent;
    FOnGetImage:TOnGetImage;
    FImageType:TImageType; // 使用WIL类型
    FImage:TGameImages;

    FUpLeft:Integer;
    FUp:Integer;
    FUpRight:Integer;
    FLeft:Integer;
    FRight:Integer;
    FDownLeft:Integer;
    FDown:Integer;
    FDownRight:Integer;

    FFillCenterColor:TColor;
    FFillCenterAlpha:Byte;
    FFillCenterExpandHorz:Integer;
    FFillCenterExpandVert:Integer;

    FHeight:Integer;
    FMaxHeight:Integer;
    FMinHeight:Integer;

    FDragHeightOffsetY:Integer;
    FDragHeightSize:Integer;

    procedure SetImageType(Value:TImageType);
    procedure SetOnGetImage(Value:TOnGetImage);

    procedure SetUpLeft(Value:Integer);
    procedure SetUp(Value:Integer);
    procedure SetUpRight(Value:Integer);
    procedure SetLeft(Value:Integer);
    procedure SetRight(Value:Integer);
    procedure SetDownLeft(Value:Integer);
    procedure SetDown(Value:Integer);
    procedure SetDownRight(Value:Integer);

    procedure SetFillCenterColor(Value:TColor);
    procedure SetFillCenterAlpha(Value:Byte);
    procedure SetFillCenterExpandHorz(Value:Integer);
    procedure SetFillCenterExpandVert(Value:Integer);

    procedure SetHeight(Value:Integer);
    procedure SetMinHeight(Value:Integer);
    procedure SetMaxHeight(Value:Integer);

    procedure SetDragHeightOffsetY(Value:Integer);
    procedure SetDragHeightSize(Value:Integer);
  protected
    procedure Changed; // dynamic;
  public
    constructor Create(AOwner:TDxMainBottomForm);
    procedure Assign(Source:TPersistent); override;
    property Image:TGameImages read FImage;
    property OnChange:TNotifyEvent read FOnChange write FOnChange;
    property OnGetImage:TOnGetImage read FOnGetImage write SetOnGetImage;
  published
    property ImageType:TImageType read FImageType write SetImageType;
    property UpLeft:Integer read FUpLeft write SetUpLeft;
    property Up:Integer read FUp write SetUp;
    property UpRight:Integer read FUpRight write SetUpRight;
    property Left:Integer read FLeft write SetLeft;
    property Right:Integer read FRight write SetRight;
    property DownLeft:Integer read FDownLeft write SetDownLeft;
    property Down:Integer read FDown write SetDown;
    property DownRight:Integer read FDownRight write SetDownRight;

    property FillCenterColor:TColor read FFillCenterColor write SetFillCenterColor;
    property FillCenterAlpha:Byte read FFillCenterAlpha write SetFillCenterAlpha;
    property FillCenterExpandHorz:Integer read FFillCenterExpandHorz write SetFillCenterExpandHorz;
    property FillCenterExpandVert:Integer read FFillCenterExpandVert write SetFillCenterExpandVert;

    property Height:Integer read FHeight write SetHeight;
    property MinHeight:Integer read FMinHeight write SetMinHeight;
    property MaxHeight:Integer read FMaxHeight write SetMaxHeight;

    property DragHeightOffsetY:Integer read FDragHeightOffsetY write SetDragHeightOffsetY;
    property DragHeightSize:Integer read FDragHeightSize write SetDragHeightSize;
  end;

  TDxMainBottomCenter = class(TPersistent)
  private
    FOwner:TDxMainBottomForm;
    FStretchImage:TDxMainBottomStretchImage;
    FFillImage:TDxMainBottomImage;

    FOffsetLeft:Integer;
    FOffsetRight:Integer;

    FAutoStretchSize:Boolean;

    procedure SetOffsetLeft(Value:Integer);
    procedure SetOffsetRight(Value:Integer);

    procedure SetAutoStretchSize(Value:Boolean);
  public
    constructor Create(AOwner:TDxMainBottomForm);
    destructor Destroy; override;
  published
    property StretchImage:TDxMainBottomStretchImage read FStretchImage write FStretchImage;
    property FillImage:TDxMainBottomImage read FFillImage write FFillImage;

    property OffsetLeft:Integer read FOffsetLeft write SetOffsetLeft;
    property OffsetRight:Integer read FOffsetRight write SetOffsetRight;

    property AutoStretchSize:Boolean read FAutoStretchSize write SetAutoStretchSize;
  end;

  TDxMainBottomForm = class(TDxControl)
  private
    FLeftImage:TDxMainBottomImage;
    FCenterSetting:TDxMainBottomCenter;
    FRightImage:TDxMainBottomImage;
    FBottomImage:TDxMainBottomImage;

    FOnCenterHeightChanged:TNotifyEvent;
    FOnCenterHeightChangeQuery:TChangeQueryEvent;

    FIsChangeCenterHeight:Boolean;
    FNoChangeCenterHeight:Integer;

    FSaveCenterHeight:Integer;

    FAnimation1:TMainBottonAnimation;
    FAnimation2:TMainBottonAnimation;
    FAnimation3:TMainBottonAnimation;
    FAnimation4:TMainBottonAnimation;
  protected
    procedure SetOnGetImage(Value:TOnGetImage); override;
    procedure DoOnInRealArea(X, Y:Integer; var IsRealArea:Boolean); override;
    procedure DoCenterHeightChanged; virtual;
    procedure DoCenterHeightChangeQuery(var CanChange:Boolean); virtual;
  public
    procedure MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); override;
    procedure MouseMove(Shift:TShiftState; X, Y:Integer); override;
    procedure MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); override;
  public
    constructor Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}); override;
    destructor Destroy; override;
    procedure Paint; override;
    procedure StopChangeCenterHeight;
    property OnCenterHeightChanged:TNotifyEvent read FOnCenterHeightChanged write FOnCenterHeightChanged;
    property OnCenterHeightChangeQuery:TChangeQueryEvent read FOnCenterHeightChangeQuery write FOnCenterHeightChangeQuery;

    function GetLeftRect:TRect;
    function GetCenterRect:TRect;
    function GetRightRect:TRect;

    procedure SaveCenterHeight;
  published
    property LeftImage:TDxMainBottomImage read FLeftImage write FLeftImage;
    property CenterSetting:TDxMainBottomCenter read FCenterSetting write FCenterSetting;
    property RightImage:TDxMainBottomImage read FRightImage write FRightImage;
    property BottomImage:TDxMainBottomImage read FBottomImage write FBottomImage;

    property Animation1:TMainBottonAnimation read FAnimation1 write FAnimation1;
    property Animation2:TMainBottonAnimation read FAnimation2 write FAnimation2;
    property Animation3:TMainBottonAnimation read FAnimation3 write FAnimation3;
    property Animation4:TMainBottonAnimation read FAnimation4 write FAnimation4;
  end;

implementation

{---------------------------------------------------------------------------------}

{ TMainBottonAnimation }

procedure TMainBottonAnimation.Assign(Source:TPersistent);
begin
  if Source is TMainBottonAnimation then begin
    OnGetImage := TMainBottonAnimation(Source).OnGetImage;
    ImageType := TMainBottonAnimation(Source).ImageType;

    FStartIndex := TMainBottonAnimation(Source).FStartIndex; // 图片序号
    FEndIndex := TMainBottonAnimation(Source).FEndIndex;
    FFrameTime := TMainBottonAnimation(Source).FFrameTime;
    FPlayCount := TMainBottonAnimation(Source).FPlayCount;

    FUseImageOffset := TMainBottonAnimation(Source).FUseImageOffset;
    FOffsetX := TMainBottonAnimation(Source).FOffsetX;
    FOffsetY := TMainBottonAnimation(Source).FOffsetY;

    FOutsideAreaDraw := TMainBottonAnimation(Source).FOutsideAreaDraw;
    FBlendDraw := TMainBottonAnimation(Source).FBlendDraw;
    FDraw := TMainBottonAnimation(Source).FDraw; // 是否绘制
    FDrawBeforeDef := TMainBottonAnimation(Source).FDrawBeforeDef; // 是否绘制

    FCurrentFrame := TMainBottonAnimation(Source).FCurrentFrame;
    FLastFrameTick := TMainBottonAnimation(Source).FLastFrameTick;
    FCurrentCount := TMainBottonAnimation(Source).FCurrentCount;

    FHorzAlignment := TMainBottonAnimation(Source).FHorzAlignment;
    FVertAlignment := TMainBottonAnimation(Source).FVertAlignment;
    FAdjustYByHeight := TMainBottonAnimation(Source).FAdjustYByHeight;

    Changed;
  end;
end;

procedure TMainBottonAnimation.Changed;
begin
  if Assigned(FOnChange) then
    FOnChange(Self);
end;

constructor TMainBottonAnimation.Create(AOnwer:TDxMainBottomForm);
begin
  FOwner := AOnwer;
  FImageType := Prguse_wil; // 图库

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

  FHorzAlignment := taLeftJustify;
  FVertAlignment := taAlignTop;
  FAdjustYByHeight := False;
end;

destructor TMainBottonAnimation.Destroy;
begin

  inherited;
end;

procedure TMainBottonAnimation.Paint;
var
  D:TTexture;
  nX, nY:Integer;
  ImgSize:TSize;
  ImgPoint:TPoint;
  R, SrcRect, ParentRect:TRect;
  BlendMode:Integer;
begin
  if FImage = nil then Exit;
  if (FStartIndex < 0) or (FEndIndex < 0) or (FStartIndex > FEndIndex) then Exit;
  if not FDraw then Exit;

  if FPlayCount > 0 then begin
    if FCurrentCount >= FPlayCount then Exit;
  end;

  if (FCurrentFrame < FStartIndex) or (FCurrentFrame > FEndIndex) then
    FCurrentFrame := FStartIndex;

  if MyGetTickCount - FLastFrameTick >= Cardinal(FFrameTime) then {//HZQ 20230519 FFrameTime --> Cardinal(FFrameTime)} begin
    Inc(FCurrentFrame);

    if (FCurrentFrame > FEndIndex) then begin
      FCurrentFrame := FStartIndex;

      if FPlayCount > 0 then begin
        Inc(FCurrentCount);
      end;
    end;

    FLastFrameTick := MyGetTickCount;
  end;

  ParentRect := FOwner.VirtualRect;

  FImage.GetCachedImageSize(FStartIndex, ImgSize, ImgPoint);

  D := FImage.GetCachedImage(FCurrentFrame, nX, nY);
  if D <> nil then begin
    SrcRect := D.ClientRect;

    if FUseImageOffset then begin
      R.Left := ParentRect.Left + FOffsetX + nX;
      R.Top := ParentRect.Top + FOffsetY + nY;
    end
    else begin
      R.Left := ParentRect.Left + FOffsetX;
      R.Top := ParentRect.Top + FOffsetY;
    end;

    if FHorzAlignment = taRightJustify then begin
      R.Left := R.Left + (ParentRect.Right - ParentRect.Left - ImgSize.cx);
    end
    else if FHorzAlignment = taCenter then begin
      R.Left := R.Left + (ParentRect.Right - ParentRect.Left - ImgSize.cx) div 2;
    end;

    if FVertAlignment = taAlignBottom then begin
      R.Top := R.Top + (ParentRect.Bottom - ParentRect.Top - ImgSize.cy);
    end
    else if FVertAlignment = taVerticalCenter then begin
      R.Top := R.Top + (ParentRect.Bottom - ParentRect.Top - ImgSize.cy) div 2;
    end;

    if FAdjustYByHeight and FOwner.CenterSetting.AutoStretchSize then begin
      R.top := R.Top - (FOwner.CenterSetting.FStretchImage.Height - FOwner.FSaveCenterHeight);
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
    end
    else begin
      if FBlendDraw then
        BlendMode := Blend_SrcAlphaColor
      else
        BlendMode := 2;

      if FHorzAlignment = taRightJustify then begin
        R.Left := R.Left + (ParentRect.Right - ParentRect.Left - ImgSize.cx);
      end
      else if FHorzAlignment = taCenter then begin
        R.Left := R.Left + (ParentRect.Right - ParentRect.Left - ImgSize.cx) div 2;
      end;

      if FVertAlignment = taAlignBottom then begin
        R.Top := R.Top + (ParentRect.Bottom - ParentRect.Top - ImgSize.cy);
      end
      else if FVertAlignment = taVerticalCenter then begin
        R.Top := R.Top + (ParentRect.Bottom - ParentRect.Top - ImgSize.cy) div 2;
      end;

      if FAdjustYByHeight and FOwner.CenterSetting.AutoStretchSize then begin
        R.top := R.Top - (FOwner.CenterSetting.FStretchImage.Height - FOwner.FSaveCenterHeight);
      end;

      GameCanvas.Draw(R.Left, R.Top, D, BlendMode);
    end;
  end;
end;

procedure TMainBottonAnimation.SetOutsideAreaDraw(const Value:Boolean);
begin
  if FOutsideAreaDraw <> Value then
    FOutsideAreaDraw := Value;
end;

procedure TMainBottonAnimation.SetDraw(const Value:Boolean);
begin
  if FDraw <> Value then begin
    FDraw := Value;

    FCurrentFrame := FStartIndex;
    FLastFrameTick := MyGetTickCount;
    FCurrentCount := 0;
  end;
end;

procedure TMainBottonAnimation.SetBlendDraw(const Value:Boolean);
begin
  FBlendDraw := Value;
end;

procedure TMainBottonAnimation.SetEndIndex(const Value:Integer);
begin
  FEndIndex := Value;
end;

procedure TMainBottonAnimation.SetFrameTime(const Value:Integer);
begin
  FFrameTime := Value;
end;

procedure TMainBottonAnimation.SetImageType(const Value:TImageType);
begin
  if FImageType <> Value then begin
    FImageType := Value;
    if Assigned(FOnGetImage) then
      FOnGetImage(Self, FImageType, TObject(FImage));
    Changed;
  end;
end;

procedure TMainBottonAnimation.SetUseImageOffset(const Value:Boolean);
begin
  FUseImageOffset := Value;
end;

procedure TMainBottonAnimation.SetOffsetX(const Value:Integer);
begin
  FOffsetX := Value;
end;

procedure TMainBottonAnimation.SetOffsetY(const Value:Integer);
begin
  FOffsetY := Value;
end;

procedure TMainBottonAnimation.SetOnGetImage(const Value:TOnGetImage);
begin
  FOnGetImage := Value;
  if Assigned(FOnGetImage) then
    FOnGetImage(Self, FImageType, TObject(FImage));
  Changed;
end;

procedure TMainBottonAnimation.SetPlayCount(const Value:Integer);
begin
  FPlayCount := Value;
end;

procedure TMainBottonAnimation.SetStartIndex(const Value:Integer);
begin
  FStartIndex := Value;
end;

{---------------------------------------------------------------------------------}

constructor TDxMainBottomImage.Create;
begin
  inherited Create();
  FImageType := Prguse_wil;
  FIndex := -1;
  FOnChange := nil;
  FOnGetImage := nil;
  FImage := nil;
end;

procedure TDxMainBottomImage.Changed;
begin
  if Assigned(FOnChange) then
    FOnChange(Self);
end;

procedure TDxMainBottomImage.Assign(Source:TPersistent);
begin
  // inherited;
  if Source is TDxMainBottomImage then begin
    Image := TDxMainBottomImage(Source).Image;
    OnGetImage := TDxMainBottomImage(Source).OnGetImage;
    ImageType := TDxMainBottomImage(Source).ImageType;
    Index := TDxMainBottomImage(Source).Index;
    Changed;
  end;
end;

procedure TDxMainBottomImage.SetImageType(Value:TImageType);
begin
  if FImageType <> Value then begin
    FImageType := Value;
    if Assigned(FOnGetImage) then
      FOnGetImage(Self, FImageType, TObject(FImage));
    Changed;
  end;
end;

procedure TDxMainBottomImage.SetOnGetImage(Value:TOnGetImage);
begin
  FOnGetImage := Value;
  if Assigned(FOnGetImage) then
    FOnGetImage(Self, FImageType, TObject(FImage));
  Changed;
end;

procedure TDxMainBottomImage.SetImage(Value:TGameImages);
begin
  if FImage <> Value then begin
    FImage := Value;
    Changed;
  end;
end;

procedure TDxMainBottomImage.SetIndex(Value:Integer);
begin
  if FIndex <> Value then begin
    FIndex := Value;
    Changed;
  end;
end;

{------------------------------------------------------------------------------}

constructor TDxMainBottomStretchImage.Create(AOwner:TDxMainBottomForm);
begin
  inherited Create;
  FOwner := AOwner;
  FImageType := Prguse_wil;

  FUpLeft := -1;
  FUp := -1;
  FUpRight := -1;
  FLeft := -1;
  FRight := -1;
  FDownLeft := -1;
  FDown := -1;
  FDownRight := -1;

  FFillCenterAlpha := 0;
  FFillCenterColor := clWhite;
  FFillCenterExpandHorz := 0;
  FFillCenterExpandVert := 0;

  FHeight := 160;
  FMaxHeight := 180;

  FDragHeightOffsetY := 0;
  FDragHeightSize := 0;

  FOnChange := nil;
  FOnGetImage := nil;
  FImage := nil;
end;

procedure TDxMainBottomStretchImage.Changed;
begin
  if Assigned(FOnChange) then
    FOnChange(Self);
end;

procedure TDxMainBottomStretchImage.Assign(Source:TPersistent);
begin
  // inherited;
  if Source is TDxMainBottomStretchImage then begin
    OnGetImage := TDxMainBottomStretchImage(Source).OnGetImage;
    ImageType := TDxMainBottomStretchImage(Source).ImageType;

    FUpLeft := TDxMainBottomStretchImage(Source).FUpLeft;
    FUp := TDxMainBottomStretchImage(Source).FUp;
    FUpRight := TDxMainBottomStretchImage(Source).FUpRight;
    FLeft := TDxMainBottomStretchImage(Source).FLeft;
    FRight := TDxMainBottomStretchImage(Source).FRight;
    FDownLeft := TDxMainBottomStretchImage(Source).FDownLeft;
    FDown := TDxMainBottomStretchImage(Source).FDown;
    FDownRight := TDxMainBottomStretchImage(Source).FDownRight;

    FFillCenterAlpha := TDxMainBottomStretchImage(Source).FFillCenterAlpha;
    FFillCenterColor := TDxMainBottomStretchImage(Source).FFillCenterColor;

    Changed;
  end;
end;

procedure TDxMainBottomStretchImage.SetImageType(Value:TImageType);
begin
  if FImageType <> Value then begin
    FImageType := Value;
    if Assigned(FOnGetImage) then
      FOnGetImage(Self, FImageType, TObject(FImage));
    Changed;
  end;
end;

procedure TDxMainBottomStretchImage.SetOnGetImage(Value:TOnGetImage);
begin
  FOnGetImage := Value;
  if Assigned(FOnGetImage) then
    FOnGetImage(Self, FImageType, TObject(FImage));
  Changed;
end;

procedure TDxMainBottomStretchImage.SetUpLeft(Value:Integer);
begin
  if FUpLeft <> Value then begin
    FUpLeft := Value;
    Changed;
  end;
end;

procedure TDxMainBottomStretchImage.SetUp(Value:Integer);
begin
  if FUp <> Value then begin
    FUp := Value;
    Changed;
  end;
end;

procedure TDxMainBottomStretchImage.SetUpRight(Value:Integer);
begin
  if FUpRight <> Value then begin
    FUpRight := Value;
    Changed;
  end;
end;

procedure TDxMainBottomStretchImage.SetLeft(Value:Integer);
begin
  if FLeft <> Value then begin
    FLeft := Value;
    Changed;
  end;
end;

procedure TDxMainBottomStretchImage.SetRight(Value:Integer);
begin
  if FRight <> Value then begin
    FRight := Value;
    Changed;
  end;
end;

procedure TDxMainBottomStretchImage.SetDownLeft(Value:Integer);
begin
  if FDownLeft <> Value then begin
    FDownLeft := Value;
    Changed;
  end;
end;

procedure TDxMainBottomStretchImage.SetDown(Value:Integer);
begin
  if FDown <> Value then begin
    FDown := Value;
    Changed;
  end;
end;

procedure TDxMainBottomStretchImage.SetDownRight(Value:Integer);
begin
  if FDownRight <> Value then begin
    FDownRight := Value;
    Changed;
  end;
end;

procedure TDxMainBottomStretchImage.SetFillCenterColor(Value:TColor);
begin
  if FFillCenterColor <> Value then begin
    FFillCenterColor := Value;
    Changed;
  end;
end;

procedure TDxMainBottomStretchImage.SetFillCenterAlpha(Value:Byte);
begin
  if FFillCenterAlpha <> Value then begin
    FFillCenterAlpha := Value;
    Changed;
  end;
end;

procedure TDxMainBottomStretchImage.SetFillCenterExpandHorz(Value:Integer);
begin
  if FFillCenterExpandHorz <> Value then begin
    FFillCenterExpandHorz := Value;
  end;
end;

procedure TDxMainBottomStretchImage.SetFillCenterExpandVert(Value:Integer);
begin
  if FFillCenterExpandVert <> Value then begin
    FFillCenterExpandVert := Value;
  end;
end;

procedure TDxMainBottomStretchImage.SetHeight(Value:Integer);
var
  CanChange:Boolean;
begin
  CanChange := True;
  FOwner.DoCenterHeightChangeQuery(CanChange);

  if CanChange then begin
    if Value > FMaxHeight then
      Value := FMaxHeight
    else if Value < FMinHeight then
      Value := FMinHeight;

    if (FHeight <> Value) then begin
      FHeight := Value;

      if FOwner.Height < FHeight then
        FOwner.Height := FHeight;

      //FOwner.FSaveCenterHeight := FHeight;

      FOwner.DoCenterHeightChanged;
    end;
  end;
end;

procedure TDxMainBottomStretchImage.SetMinHeight(Value:Integer);
begin
  if (FMinHeight <> Value) then begin
    FMinHeight := Value;

    if FHeight < FMinHeight then
      FHeight := FMinHeight;

    if (FMaxHeight < FMinHeight) then
      FMaxHeight := FMinHeight;
  end;
end;

procedure TDxMainBottomStretchImage.SetMaxHeight(Value:Integer);
begin
  if (FMaxHeight <> Value) then begin
    FMaxHeight := Value;

    if FMaxHeight < FHeight then
      FHeight := FMaxHeight;

    if FMaxHeight < FMinHeight then
      FMinHeight := FMaxHeight;
  end;
end;

procedure TDxMainBottomStretchImage.SetDragHeightOffsetY(Value:Integer);
begin
  if FDragHeightOffsetY <> Value then begin
    FDragHeightOffsetY := Value;
  end;
end;

procedure TDxMainBottomStretchImage.SetDragHeightSize(Value:Integer);
begin
  if FDragHeightSize <> Value then begin
    FDragHeightSize := Value;
  end;
end;

{-----------------------------------------------------------------------------}

constructor TDxMainBottomCenter.Create(AOwner:TDxMainBottomForm);
begin
  FOwner := AOwner;

  FOffsetLeft := 0;
  FOffsetRight := 0;
  FAutoStretchSize := True;

  FStretchImage := TDxMainBottomStretchImage.Create(AOwner);
  FFillImage := TDxMainBottomImage.Create();
end;

destructor TDxMainBottomCenter.Destroy;
begin
  FStretchImage.Free;
  FFillImage.Free;
  inherited;
end;

procedure TDxMainBottomCenter.SetOffsetLeft(Value:Integer);
begin
  if FOffsetLeft <> Value then begin
    FOffsetLeft := Value;
  end;
end;

procedure TDxMainBottomCenter.SetOffsetRight(Value:Integer);
begin
  if FOffsetRight <> Value then begin
    FOffsetRight := Value;
  end;
end;

procedure TDxMainBottomCenter.SetAutoStretchSize(Value:Boolean);
begin
  if FAutoStretchSize <> Value then
    FAutoStretchSize := Value;
end;

{-----------------------------------------------------------------------------}

constructor TDxMainBottomForm.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
begin
  inherited Create(AOwner);

  AutoSize := False;

  Width := 800;
  Height := 260;

  FLeftImage := TDxMainBottomImage.Create;
  FCenterSetting := TDxMainBottomCenter.Create(Self);
  FRightImage := TDxMainBottomImage.Create;
  FBottomImage := TDxMainBottomImage.Create;

  FLeftImage.OnGetImage := OnGetImage;
  FCenterSetting.FStretchImage.OnGetImage := OnGetImage;
  FCenterSetting.FFillImage.OnGetImage := OnGetImage;
  FRightImage.OnGetImage := OnGetImage;
  FBottomImage.OnGetImage := OnGetImage;
  FIsChangeCenterHeight := False;
  FNoChangeCenterHeight := 0;

  FSaveCenterHeight := 0;

  FAnimation1 := TMainBottonAnimation.Create(Self);
  FAnimation2 := TMainBottonAnimation.Create(Self);
  FAnimation3 := TMainBottonAnimation.Create(Self);
  FAnimation4 := TMainBottonAnimation.Create(Self);
end;

destructor TDxMainBottomForm.Destroy;
begin
  FLeftImage.Free;
  FCenterSetting.Free;
  FRightImage.Free;
  FBottomImage.Free;

  FAnimation1.Free;
  FAnimation2.Free;
  FAnimation3.Free;
  FAnimation4.Free;

  inherited;
end;

procedure TDxMainBottomForm.SetOnGetImage(Value:TOnGetImage);
begin
  inherited;
  FLeftImage.OnGetImage := Value;
  FCenterSetting.FStretchImage.OnGetImage := Value;
  FCenterSetting.FFillImage.OnGetImage := Value;
  FRightImage.OnGetImage := Value;
  FBottomImage.OnGetImage := Value;

  FAnimation1.OnGetImage := Value;
  FAnimation2.OnGetImage := Value;
  FAnimation3.OnGetImage := Value;
  FAnimation4.OnGetImage := Value;
end;

procedure TDxMainBottomForm.MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
var
  R:TRect;
  DLeft:TTexture;
  DRight:TTexture;
  //DBottom: TTexture;
begin
  inherited MouseDown(Button, Shift, X, Y);

  if FCenterSetting.AutoStretchSize and (FCenterSetting.FStretchImage.FDragHeightSize > 0) then begin
    DLeft := nil;
    DRight := nil;
    R := VirtualRect;

    if (FLeftImage.Image <> nil) and (FLeftImage.Index >= 0) then begin
      DLeft := FLeftImage.Image.Images[FLeftImage.Index];
    end;

    if (FRightImage.Image <> nil) and (FRightImage.Index >= 0) then begin
      DRight := FRightImage.Image.Images[FRightImage.Index];
    end;

    //HZQ 20230519 DBottom未使用
    //if (FBottomImage.Image <> nil) and (FBottomImage.Index >= 0) then
    //begin
    //  DBottom := FBottomImage.Image.Images[FBottomImage.Index];
    //end;

    if DLeft <> nil then begin
      R.Left := R.Left + DLeft.Width - FCenterSetting.OffsetLeft;
    end;

    if DRight <> nil then begin
      R.Right := R.Right - DRight.Width + FCenterSetting.OffsetRight;
    end;

    R.Top := (R.Bottom - FCenterSetting.FStretchImage.Height);

    R.Top := R.Top + FCenterSetting.FStretchImage.FDragHeightOffsetY;
    R.Bottom := R.Top + FCenterSetting.FStretchImage.FDragHeightSize;

    FIsChangeCenterHeight := PtInRect(R, Point(X, Y));
    FNoChangeCenterHeight := FCenterSetting.FStretchImage.Height;
  end;
end;

procedure TDxMainBottomForm.MouseMove(Shift:TShiftState; X, Y:Integer);
var
  OffsetY:Integer;
begin
  inherited MouseMove(Shift, X, Y);

  if FIsChangeCenterHeight then begin
    OffsetY := SpotY - Y;
    FCenterSetting.FStretchImage.Height := FNoChangeCenterHeight + OffsetY;
  end;
end;

procedure TDxMainBottomForm.MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
begin
  inherited MouseUp(Button, Shift, X, Y);
  FIsChangeCenterHeight := False;
end;

procedure TDxMainBottomForm.DoCenterHeightChanged;
begin
  if Assigned(FOnCenterHeightChanged) then
    FOnCenterHeightChanged(Self);
end;

procedure TDxMainBottomForm.DoCenterHeightChangeQuery(var CanChange:Boolean);
begin
  if Assigned(FOnCenterHeightChangeQuery) then
    FOnCenterHeightChangeQuery(Self, CanChange);
end;

procedure TDxMainBottomForm.DoOnInRealArea(X, Y:Integer; var IsRealArea:Boolean);
var
  DLeft:TTexture;
  DRight:TTexture;
  DCenter:TTexture;
  //DBottom: TTexture;
  R, RectC, PaintRect:TRect;
  nWidth:Integer;

  vbRect:TRect;
begin
  if Designing then
    Exit;

  // 修正不勾选边框自适应模式，并且左，中，右三张图均无时，人物不能跑  2019-07-18
  IsRealArea := False;

  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;

  DLeft := nil;
  DRight := nil;
  DCenter := nil;

  R := VirtualRect;
  RectC := R;
  PaintRect := R;

  X := Left + X;
  Y := Top + Y;

  if (FLeftImage.Image <> nil) and (FLeftImage.Index >= 0) then begin
    DLeft := FLeftImage.Image.Images[FLeftImage.Index];
  end;

  if (FRightImage.Image <> nil) and (FRightImage.Index >= 0) then begin
    DRight := FRightImage.Image.Images[FRightImage.Index];
  end;

  //  if (FBottomImage.Image <> nil) and (FBottomImage.Index >= 0) then
  //  begin
  //    DBottom := FBottomImage.Image.Images[FBottomImage.Index];
  //  end;

  if not FCenterSetting.AutoStretchSize then begin
    if (FCenterSetting.FillImage.FImage <> nil) and (FCenterSetting.FillImage.Index >= 0) then begin
      DCenter := FCenterSetting.FillImage.Image.Images[FCenterSetting.FillImage.Index];
    end;

    if (DLeft <> nil) and (DCenter <> nil) and (DRight <> nil) then begin
      nWidth := DLeft.Width + DCenter.Width + DRight.Width + FCenterSetting.OffsetLeft + FCenterSetting.OffsetRight;

      PaintRect.Left := PaintRect.Left + (Width - nWidth) div 2;
      if PtInRect(PaintRect, Point(X, Y)) then begin
        IsRealArea := CheckTextureAlpha(DLeft, X - PaintRect.Left, Y - PaintRect.Top);
      end
      else begin
        PaintRect.Left := PaintRect.Left + DLeft.Width + FCenterSetting.OffsetLeft;

        if PtInRect(PaintRect, Point(X, Y)) then begin
          IsRealArea := CheckTextureAlpha(DCenter, X - PaintRect.Left, Y - PaintRect.Top);
        end
        else begin
          PaintRect.Left := PaintRect.Left + DCenter.Width + FCenterSetting.OffsetRight;

          if PtInRect(PaintRect, Point(X, Y)) then begin
            IsRealArea := CheckTextureAlpha(DRight, X - PaintRect.Left, Y - PaintRect.Top);
          end
        end;
      end;
    end;

    Exit;
  end;

  if DLeft <> nil then begin
    PaintRect.Left := R.Left;
    PaintRect.Top := R.Top + Height - DLeft.Height;
    PaintRect.Right := PaintRect.Left + DLeft.Width;
    PaintRect.Bottom := R.Top + Height;

    if PtInRect(PaintRect, Point(X, Y)) then begin
      IsRealArea := CheckTextureAlpha(DLeft, X - PaintRect.Left, Y - PaintRect.Top);
      Exit;
    end;

    RectC.Left := R.Left + DLeft.Width - FCenterSetting.OffsetLeft;
  end;

  if DRight <> nil then begin
    PaintRect.Left := R.Right - DRight.Width;
    PaintRect.Top := R.Top + Height - DRight.Height;
    PaintRect.Right := R.Right;
    PaintRect.Bottom := R.Top + Height;

    if PtInRect(PaintRect, Point(X, Y)) then begin
      IsRealArea := CheckTextureAlpha(DRight, X - PaintRect.Left, Y - PaintRect.Top);
      Exit;
    end;

    RectC.Right := RectC.Right - DRight.Width + FCenterSetting.OffsetRight;
  end;

  RectC.Top := (RectC.Bottom - FCenterSetting.StretchImage.Height);
  IsRealArea := PtInRect(RectC, Point(X, Y));
end;

procedure TDxMainBottomForm.Paint;
var
  DUpLeft:TTexture;
  DUp:TTexture;
  DUpRight:TTexture;
  DLeft:TTexture;
  DRight:TTexture;
  DDownLeft:TTexture;
  DDown:TTexture;
  DDownRight:TTexture;
  DCenter:TTexture;
  DBottom:TTexture;

  R, RectC, RectLeft, RectRight, RectUP, RectDown, PaintRect:TRect;
  nLeft, nTop, nStart, nEnd, nWidth, nHeight:Integer;

  GameImages:TGameImages;

  I:Integer;
  vbRect:TRect;
begin
  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;

  nHeight := 0;
  DLeft := nil;
  DRight := nil;
  DCenter := nil;
  DBottom := nil;
  R := VirtualRect;
  RectC := R;

  if FAnimation1.DrawBeforeDef then
    FAnimation1.Paint;

  if FAnimation2.DrawBeforeDef then
    FAnimation2.Paint;

  if FAnimation3.DrawBeforeDef then
    FAnimation3.Paint;

  if FAnimation4.DrawBeforeDef then
    FAnimation4.Paint;

  if Assigned(OnStartPaint) then
    OnStartPaint(Self);

  if (FLeftImage.Image <> nil) and (FLeftImage.Index >= 0) then begin
    DLeft := FLeftImage.Image.Images[FLeftImage.Index];
  end;

  if (FRightImage.Image <> nil) and (FRightImage.Index >= 0) then begin
    DRight := FRightImage.Image.Images[FRightImage.Index];
  end;

  if (FBottomImage.Image <> nil) and (FBottomImage.Index >= 0) then begin
    DBottom := FBottomImage.Image.Images[FBottomImage.Index];
  end;

  if not FCenterSetting.AutoStretchSize then begin
    if (FCenterSetting.FillImage.FImage <> nil) and (FCenterSetting.FillImage.Index >= 0) then begin
      DCenter := FCenterSetting.FillImage.Image.Images[FCenterSetting.FillImage.Index];
    end;
    if (DBottom <> nil) then begin
      nWidth := DBottom.Width;
      RectC := R;
      RectC.Left := RectC.Left + (Width - nWidth) div 2;
      //画底部
      GameCanvas.Draw(RectC.Left, RectC.Top + Height - DBottom.Height, DBottom);
      nHeight := nHeight - DBottom.Height;
    end;
    //    if (DLeft <> nil) and (DCenter <> nil) and (DRight <> nil) then
    //    begin
    //      nWidth := DLeft.Width + DCenter.Width + DRight.Width + FCenterSetting.OffsetLeft + FCenterSetting.OffsetRight;
    //
    //      RectC.Left := RectC.Left + (Width - nWidth) div 2;
    //
    //      GameCanvas.Draw(RectC.Left, RectC.Top + Height - DLeft.Height, DLeft);
    //      RectC.Left := RectC.Left + DLeft.Width + FCenterSetting.OffsetLeft;
    //
    //      GameCanvas.Draw(RectC.Left, RectC.Top + Height - DCenter.Height, DCenter);
    //      RectC.Left := RectC.Left + DCenter.Width + FCenterSetting.OffsetRight;
    //
    //      GameCanvas.Draw(RectC.Left, RectC.Top + Height - DRight.Height, DRight);
    //    end;
    nWidth := FCenterSetting.OffsetLeft + FCenterSetting.OffsetRight;
    if (DLeft <> nil) then nWidth := nWidth + DLeft.Width;
    if (DCenter <> nil) then nWidth := nWidth + DCenter.Width;
    if (DRight <> nil) then nWidth := nWidth + DRight.Width;

    RectC := R;
    RectC.Left := RectC.Left + (Width - nWidth) div 2;
    //画左边
    if (DLeft <> nil) then begin
      GameCanvas.Draw(RectC.Left, RectC.Top + Height - DLeft.Height + nHeight, DLeft);
      RectC.Left := RectC.Left + DLeft.Width + FCenterSetting.OffsetLeft;
    end;
    //画中间
    if (DCenter <> nil) then begin
      GameCanvas.Draw(RectC.Left, RectC.Top + Height - DCenter.Height + nHeight, DCenter);
      RectC.Left := RectC.Left + DCenter.Width + FCenterSetting.OffsetRight;
    end;
    //画右边
    if (DRight <> nil) then begin
      GameCanvas.Draw(RectC.Left, RectC.Top + Height - DRight.Height + nHeight, DRight);
    end;

    DoPaint();

    if not FAnimation1.DrawBeforeDef then
      FAnimation1.Paint;

    if not FAnimation2.DrawBeforeDef then
      FAnimation2.Paint;

    if not FAnimation3.DrawBeforeDef then
      FAnimation3.Paint;

    if not FAnimation4.DrawBeforeDef then
      FAnimation4.Paint;

    if Assigned(OnStartPaint) then
      OnStartPaint(Self);

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
      FrameRect(R, R, vbRect, BorderColor.Up.Color);
    end;

    Exit;
  end;

  nHeight := 0;

  if DBottom <> nil then begin
    GameCanvas.Draw(R.Left, R.Top + Height - DBottom.Height, DBottom);
    RectC.Left := R.Left + DBottom.Width;
    nHeight := nHeight - DBottom.Height;
  end;

  if DLeft <> nil then begin
    GameCanvas.Draw(R.Left, R.Top + Height - DLeft.Height + nHeight, DLeft);
    RectC.Left := R.Left + DLeft.Width - FCenterSetting.OffsetLeft;
  end;

  if DRight <> nil then begin
    GameCanvas.Draw(R.Right - DRight.Width, R.Top + Height - DRight.Height + nHeight, DRight);
    RectC.Right := RectC.Right - DRight.Width + FCenterSetting.OffsetRight;
  end;

  RectC.Top := (RectC.Bottom - FCenterSetting.StretchImage.Height);

  GameImages := FCenterSetting.StretchImage.FImage;

  if GameImages <> nil then begin
    DUpLeft := GameImages.Images[FCenterSetting.StretchImage.FUpLeft];
    DUp := GameImages.Images[FCenterSetting.StretchImage.FUp];
    DUpRight := GameImages.Images[FCenterSetting.StretchImage.FUpRight];
    DLeft := GameImages.Images[FCenterSetting.StretchImage.FLeft];
    DRight := GameImages.Images[FCenterSetting.StretchImage.FRight];
    DDownLeft := GameImages.Images[FCenterSetting.StretchImage.FDownLeft];
    DDown := GameImages.Images[FCenterSetting.StretchImage.FDown];
    DDownRight := GameImages.Images[FCenterSetting.StretchImage.FDownRight];

    RectLeft := RectC;
    RectRight := RectC;
    RectUP := RectC;
    RectDown := RectC;

    if (DUpLeft <> nil) then begin
      GameCanvas.Draw(RectC.Left, RectC.Top, DUpLeft);

      RectUP.Left := RectUP.Left + DUpLeft.Width;
      RectLeft.Top := RectLeft.Top + DUpLeft.Height;
    end;

    if DUpRight <> nil then begin
      GameCanvas.Draw(RectC.Right - DUpRight.Width, RectC.Top, DUpRight);

      RectUP.Right := RectUP.Right - DUpRight.Width;
      RectRight.Top := RectRight.Top + DUpRight.Height;
    end;

    if DDownLeft <> nil then begin
      GameCanvas.Draw(RectC.Left, RectC.Bottom - DDownLeft.Height, DDownLeft);

      RectDown.Left := RectDown.Left + DDownLeft.Width;
      RectLeft.Bottom := RectLeft.Bottom - DDownLeft.Height;
    end;

    if DDownRight <> nil then begin
      GameCanvas.Draw(RectC.Right - DUpRight.Width, RectC.Bottom - DDownRight.Height, DDownRight);

      RectDown.Right := RectDown.Right - DDownRight.Width;
      RectRight.Bottom := RectRight.Bottom - DDownRight.Height;
    end;

    if DUp <> nil then begin
      nStart := RectUP.Left;
      nEnd := RectUP.Right;
      while nStart < nEnd do begin
        if nStart + DUp.Width <= nEnd then
          PaintRect := DUp.ClientRect
        else
          PaintRect := Bounds(0, 0, nEnd - nStart, DUp.Height);

        GameCanvas.Draw(nStart, RectUP.Top, PaintRect, DUp);
        Inc(nStart, DUp.Width);
      end;
    end;

    if DDown <> nil then begin
      nStart := RectDown.Left;
      nEnd := RectDown.Right;
      RectDown.Top := RectDown.Bottom - DDown.Height;
      while nStart < nEnd do begin
        if nStart + DDown.Width <= nEnd then
          PaintRect := DDown.ClientRect
        else
          PaintRect := Bounds(0, 0, nEnd - nStart, DDown.Height);

        GameCanvas.Draw(nStart, RectDown.Top, PaintRect, DDown);
        Inc(nStart, DDown.Width);
      end;
    end;

    if DLeft <> nil then begin
      nStart := RectLeft.Top;
      nEnd := RectLeft.Bottom;
      while nStart < nEnd do begin
        if nStart + DLeft.Height <= nEnd then
          PaintRect := DLeft.ClientRect
        else
          PaintRect := Bounds(0, 0, Width, nEnd - nStart);

        GameCanvas.Draw(RectLeft.Left, nStart, PaintRect, DLeft);
        Inc(nStart, DLeft.Height);
      end;
    end;

    if DRight <> nil then begin
      RectRight.Left := RectRight.Right - DRight.Width;
      nStart := RectRight.Top;
      nEnd := RectRight.Bottom;
      while nStart < nEnd do begin
        if nStart + DRight.Height <= nEnd then
          PaintRect := DRight.ClientRect
        else
          PaintRect := Bounds(0, 0, Width, nEnd - nStart);

        GameCanvas.Draw(RectRight.Left, nStart, PaintRect, DRight);
        Inc(nStart, DRight.Height);
      end;
    end;

    if (DUpLeft <> nil) and (DDownRight <> nil) and (FCenterSetting.FStretchImage.FFillCenterAlpha > 0) then begin
      PaintRect.Left := RectC.Left + DUpLeft.Width;
      PaintRect.Top := RectC.Top + DUpLeft.Height;
      PaintRect.Right := RectC.Right - DDownRight.Width;
      PaintRect.Bottom := RectC.Bottom - DDownRight.Height;

      InflateRect(PaintRect, FCenterSetting.FStretchImage.FFillCenterExpandHorz, FCenterSetting.FStretchImage.FFillCenterExpandVert);

      GameCanvas.FillRectAlpha(PaintRect, FCenterSetting.FStretchImage.FFillCenterColor, FCenterSetting.FStretchImage.FFillCenterAlpha);
    end;
  end;

  DoPaint();

  if not FAnimation1.DrawBeforeDef then
    FAnimation1.Paint;

  if not FAnimation2.DrawBeforeDef then
    FAnimation2.Paint;

  if not FAnimation3.DrawBeforeDef then
    FAnimation3.Paint;

  if not FAnimation4.DrawBeforeDef then
    FAnimation4.Paint;

  if Assigned(OnStartPaint) then
    OnStartPaint(Self);

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
    FrameRect(R, R, vbRect, BorderColor.Up.Color);
  end;
end;

procedure TDxMainBottomForm.StopChangeCenterHeight;
begin
  FIsChangeCenterHeight := False;
end;

function TDxMainBottomForm.GetLeftRect:TRect;
var
  DLeft:TTexture;
  DRight:TTexture;
  DCenter:TTexture;
  //DBottom: TTexture;
  R, RectL:TRect;
  nWidth:Integer;
begin
  DLeft := nil;
  DRight := nil;
  DCenter := nil;

  R := VirtualRect;

  if (FLeftImage.Image <> nil) and (FLeftImage.Index >= 0) then begin
    DLeft := FLeftImage.Image.Images[FLeftImage.Index];
  end;

  if (FRightImage.Image <> nil) and (FRightImage.Index >= 0) then begin
    DRight := FRightImage.Image.Images[FRightImage.Index];
  end;

  //  if (FBottomImage.Image <> nil) and (FBottomImage.Index >= 0) then
  //  begin
  //    DBottom := FBottomImage.Image.Images[FBottomImage.Index];
  //  end;

  RectL := Rect(0, 0, 0, 0);

  if not FCenterSetting.AutoStretchSize then begin
    if (FCenterSetting.FillImage.FImage <> nil) and (FCenterSetting.FillImage.Index >= 0) then begin
      DCenter := FCenterSetting.FillImage.Image.Images[FCenterSetting.FillImage.Index];
    end;

    if (DLeft <> nil) and (DCenter <> nil) and (DRight <> nil) then begin
      nWidth := DLeft.Width + DCenter.Width + DRight.Width + FCenterSetting.OffsetLeft + FCenterSetting.OffsetRight;

      RectL.Left := R.Left + (Width - nWidth) div 2;
      RectL.Right := R.Left + DLeft.Width - FCenterSetting.OffsetLeft;
      RectL.Top := R.Top + Height - DLeft.Height;
      RectL.Bottom := R.Bottom;
    end;
  end
  else begin
    if DLeft <> nil then begin
      RectL.Left := R.Left;
      RectL.Top := R.Top + Height - DLeft.Height;
      RectL.Right := RectL.Left + DLeft.Width - FCenterSetting.OffsetLeft;
      RectL.Bottom := R.Top + Height;
    end;
  end;

  Result := RectL;
end;

function TDxMainBottomForm.GetCenterRect:TRect;
var
  DLeft:TTexture;
  DRight:TTexture;
  DCenter:TTexture;
  //DBottom: TTexture;
  R, RectC:TRect;
  nWidth:Integer;
begin
  DLeft := nil;
  DRight := nil;
  DCenter := nil;

  R := VirtualRect;

  if (FLeftImage.Image <> nil) and (FLeftImage.Index >= 0) then begin
    DLeft := FLeftImage.Image.Images[FLeftImage.Index];
  end;

  if (FRightImage.Image <> nil) and (FRightImage.Index >= 0) then begin
    DRight := FRightImage.Image.Images[FRightImage.Index];
  end;

  //  if (FBottomImage.Image <> nil) and (FBottomImage.Index >= 0) then
  //  begin
  //    DBottom := FBottomImage.Image.Images[FBottomImage.Index];
  //  end;

  RectC := Rect(0, 0, 0, 0);

  if not FCenterSetting.AutoStretchSize then begin
    if (FCenterSetting.FillImage.FImage <> nil) and (FCenterSetting.FillImage.Index >= 0) then begin
      DCenter := FCenterSetting.FillImage.Image.Images[FCenterSetting.FillImage.Index];
    end;

    if (DLeft <> nil) and (DCenter <> nil) and (DRight <> nil) then begin
      nWidth := DLeft.Width + DCenter.Width + DRight.Width + FCenterSetting.OffsetLeft + FCenterSetting.OffsetRight;

      RectC.Left := (R.Left + (Width - nWidth) div 2) + DLeft.Width - FCenterSetting.OffsetLeft;
      RectC.Right := RectC.Left + DCenter.Width + FCenterSetting.OffsetRight;
      RectC.Top := R.Top + Height - DCenter.Height;
      RectC.Bottom := R.Bottom;

      Result := RectC;
    end;
  end
  else begin
    RectC.Top := (R.Bottom - FCenterSetting.StretchImage.Height);
    RectC.Bottom := R.Bottom;

    RectC.Left := R.Left;
    RectC.Right := R.Right;

    if DLeft <> nil then begin
      RectC.Left := R.Left + DLeft.Width - FCenterSetting.OffsetLeft;
    end;

    if DRight <> nil then begin
      RectC.Right := R.Right - DRight.Width + FCenterSetting.OffsetRight;
    end;
  end;

  Result := RectC;
end;

function TDxMainBottomForm.GetRightRect:TRect;
var
  DLeft:TTexture;
  DRight:TTexture;
  DCenter:TTexture;
  //DBottom: TTexture;
  R, RectR:TRect;
  nWidth:Integer;
begin
  DLeft := nil;
  DRight := nil;
  DCenter := nil;

  R := VirtualRect;

  if (FLeftImage.Image <> nil) and (FLeftImage.Index >= 0) then begin
    DLeft := FLeftImage.Image.Images[FLeftImage.Index];
  end;

  if (FRightImage.Image <> nil) and (FRightImage.Index >= 0) then begin
    DRight := FRightImage.Image.Images[FRightImage.Index];
  end;

  //  if (FBottomImage.Image <> nil) and (FBottomImage.Index >= 0) then
  //  begin
  //    DBottom := FBottomImage.Image.Images[FBottomImage.Index];
  //  end;

  Result := Rect(0, 0, 0, 0);

  if not FCenterSetting.AutoStretchSize then begin
    if (FCenterSetting.FillImage.FImage <> nil) and (FCenterSetting.FillImage.Index >= 0) then begin
      DCenter := FCenterSetting.FillImage.Image.Images[FCenterSetting.FillImage.Index];
    end;

    if (DLeft <> nil) and (DCenter <> nil) and (DRight <> nil) then begin
      nWidth := DLeft.Width + DCenter.Width + DRight.Width + FCenterSetting.OffsetLeft + FCenterSetting.OffsetRight;

      RectR.Right := (R.Left + (Width - nWidth) div 2) + nWidth;
      RectR.Left := RectR.Right - DRight.Width + FCenterSetting.OffsetRight;
      RectR.Top := R.Top + Height - DRight.Height;
      RectR.Bottom := R.Bottom;
    end;
  end
  else begin
    if DRight <> nil then begin
      RectR.Top := (R.Bottom - DRight.Height);
      RectR.Bottom := R.Bottom;

      RectR.Left := R.Right - DRight.Width + FCenterSetting.OffsetRight;
      RectR.Right := R.Right;
    end;
  end;

  Result := RectR;
end;

procedure TDxMainBottomForm.SaveCenterHeight;
begin
  FSaveCenterHeight := FCenterSetting.StretchImage.FHeight;
end;

end.
