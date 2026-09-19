unit DxImageForm;

interface
uses
  Windows,
  Types,
  Classes,
  Controls,
  SysUtils,
  Forms,
  HGE,
  DxControls,
  Dialogs,
  DxComponents,
  GameImages,
  Graphics,
  DxCanvas,
  Messages;

type
  TDxFormShapeImage = class(TInterfacedPersistent)
  private
    FOnChange:TNotifyEvent;
    FOnGetImage:TOnGetImage;
    FAlign:TAlignEx;
    FImageType:TImageType; // 使用WIL类型
    FImage:TGameImages;
    FImageIndex:Integer; // 图片序号
    FDraw:Boolean; // 是否绘制

    FStretch:Boolean; // 缩放绘制模式
    FCenter:Boolean; // 绘制在中间
    FSourceRect:TRect; // 需要绘制图片矩形
    FDestRect:TRect; // 绘制在目标矩形
    FBlendMode:Integer;
    FAutoDestRectSize:Boolean;
    procedure SetOnGetImage(Value:TOnGetImage);
    procedure SetImage(Value:TGameImages);
    procedure SetImageType(Value:TImageType);
    procedure SetImageIndex(Value:Integer);
    procedure SetDraw(Value:Boolean);
    procedure SetStretch(Value:Boolean);
    procedure SetCenter(Value:Boolean);
    procedure SetAlign(Value:TAlignEx);
    procedure SetAutoDestRectSize(Value:Boolean);
    function GetSourcePosition(const Index:Integer):Integer;
    procedure SetSourcePosition(const Index, Value:Integer);
    function GetDestPosition(const Index:Integer):Integer;
    procedure SetDestPosition(const Index, Value:Integer);
    procedure Changed;
  protected

  public
    constructor Create;
    destructor Destroy; override;
    procedure Assign(Source:TPersistent); override;
    property Image:TGameImages read FImage write SetImage;
    property OnChange:TNotifyEvent read FOnChange write FOnChange;
    property OnGetImage:TOnGetImage read FOnGetImage write SetOnGetImage;
    property SourceRect:TRect read FSourceRect write FSourceRect;
    property DestRect:TRect read FDestRect write FDestRect;
  published

    property ImageType:TImageType read FImageType write SetImageType;
    property ImageIndex:Integer read FImageIndex write SetImageIndex;
    property Align:TAlignEx read FAlign write SetAlign;
    property Draw:Boolean read FDraw write SetDraw;
    property Stretch:Boolean read FStretch write SetStretch;
    property Center:Boolean read FCenter write SetCenter;
    property AutoDestRectSize:Boolean read FAutoDestRectSize write SetAutoDestRectSize;
    property BlendMode:Integer read FBlendMode write FBlendMode;

    property SourceLeft:Integer index 0 read FSourceRect.Left write SetSourcePosition;
    property SourceTop:Integer index 1 read FSourceRect.Top write SetSourcePosition;
    property SourceWidth:Integer index 2 read GetSourcePosition write SetSourcePosition;
    property SourceHeight:Integer index 3 read GetSourcePosition write SetSourcePosition;

    property DestLeft:Integer index 0 read FDestRect.Left write SetDestPosition;
    property DestTop:Integer index 1 read FDestRect.Top write SetDestPosition;
    property DestWidth:Integer index 2 read GetDestPosition write SetDestPosition;
    property DestHeight:Integer index 3 read GetDestPosition write SetDestPosition;
  end;

  TDxImageForm = class;

  TAnimation = class(TInterfacedPersistent)
  private
    FOwner:TDxImageForm;
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
    constructor Create(AOnwer:TDxImageForm);
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
  end;

  TDxImageForm = class(TDxControl)
  private
    FIsBringToFront:Boolean;
    FBackgroundAlpha:Byte;

    FNoMove:Boolean;

    FOnAnimationFrameChanged:TAnimationFrameChangedEvent;
    FAnimation1:TAnimation;
    FAnimation2:TAnimation;
    FAnimation3:TAnimation;

    FOnBringToFront:TNotifyEvent;
    FOnAfterBringToFront:TNotifyEvent;

    procedure SetBackgroundAlpha(Value:Byte);

    function IsKeyMsg(var Msg:TMsg):Boolean;
    procedure ProcessMessages;
    function ProcessMessage(var Msg:TMsg):Boolean;
  protected
    procedure SetOnGetImage(Value:TOnGetImage); override;
    procedure DoMouseDown(); override;
    procedure DoMouseMove(); override;
    procedure DoMouseUp(); override;
    procedure DoShow(); override;
    procedure DoHide(); override;
    function CanMove:Boolean; override;

  public
    function InRange(X, Y:Integer):Boolean; override;
    procedure MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); override;
    procedure MouseMove(Shift:TShiftState; X, Y:Integer); override;
    procedure Assign(Source:TDxControl); override;
  public
    DialogResult:TModalResult;
    constructor Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}); override;
    destructor Destroy; override;
    procedure Paint; override;
    function ShowModalEx:Integer; overload;
    function ShowModal:Integer; overload;
    function ShowModal(Process:TNotifyEvent):Integer; overload;
    procedure BringToFront(); override;
    procedure BringToFrontEx();
    property IsBringToFront:Boolean read FIsBringToFront write FIsBringToFront;

    property OnBringToFront:TNotifyEvent read FOnBringToFront write FOnBringToFront;
    property OnAfterBringToFront:TNotifyEvent read FOnAfterBringToFront write FOnAfterBringToFront;

    property OnAnimationFrameChanged:TAnimationFrameChangedEvent read FOnAnimationFrameChanged write FOnAnimationFrameChanged;
  published
    property Align;
    property AutoSize;
    property Center;
    property BackgroundColor;
    property BackgroundAlpha:Byte read FBackgroundAlpha write SetBackgroundAlpha;

    property Animation1:TAnimation read FAnimation1 write FAnimation1;
    property Animation2:TAnimation read FAnimation2 write FAnimation2;
    property Animation3:TAnimation read FAnimation3 write FAnimation3;

    property NoMove:Boolean read FNoMove write FNoMove;
  end;

  TDxImageFormShape = class(TDxImageForm)
  private
    FDraw1:TDxFormShapeImage;
    FDraw2:TDxFormShapeImage;
    FDraw3:TDxFormShapeImage;
    FDraw4:TDxFormShapeImage;
    FDraw5:TDxFormShapeImage;
    FDraw6:TDxFormShapeImage;
    FDraw7:TDxFormShapeImage;
    FDraw8:TDxFormShapeImage;
    FDxFormShapeInfo:array[0..8 - 1] of TDxFormShapeImage;
    function GetImageCount:Integer;
    function GetShapeImages(Index:Integer):TDxFormShapeImage;
    procedure ImageIndexChange(Sender:TObject); stdcall;
  protected
    procedure SetOnGetImage(Value:TOnGetImage); override;
  public
    constructor Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}); override;
    destructor Destroy; override;
    procedure Paint; override;
    property Items[Index:Integer]:TDxFormShapeImage read GetShapeImages;
    property ImageCount:Integer read GetImageCount;
  published
    property Draw1:TDxFormShapeImage read FDraw1 write FDraw1;
    property Draw2:TDxFormShapeImage read FDraw2 write FDraw2;
    property Draw3:TDxFormShapeImage read FDraw3 write FDraw3;
    property Draw4:TDxFormShapeImage read FDraw4 write FDraw4;
    property Draw5:TDxFormShapeImage read FDraw5 write FDraw5;
    property Draw6:TDxFormShapeImage read FDraw6 write FDraw6;
    property Draw7:TDxFormShapeImage read FDraw7 write FDraw7;
    property Draw8:TDxFormShapeImage read FDraw8 write FDraw8;
  end;

implementation

uses Math,
  HGECanvas;

{---------------------------------------------------------------------------------}

{ TAnimation }

procedure TAnimation.Assign(Source:TPersistent);
begin
  if Source is TAnimation then begin
    OnGetImage := TAnimation(Source).OnGetImage;
    ImageType := TAnimation(Source).ImageType;

    FStartIndex := TAnimation(Source).FStartIndex; // 图片序号
    FEndIndex := TAnimation(Source).FEndIndex;
    FFrameTime := TAnimation(Source).FFrameTime;
    FPlayCount := TAnimation(Source).FPlayCount;

    FUseImageOffset := TAnimation(Source).FUseImageOffset;
    FOffsetX := TAnimation(Source).FOffsetX;
    FOffsetY := TAnimation(Source).FOffsetY;

    FOutsideAreaDraw := TAnimation(Source).FOutsideAreaDraw;
    FBlendDraw := TAnimation(Source).FBlendDraw;
    FDraw := TAnimation(Source).FDraw; // 是否绘制
    FDrawBeforeDef := TAnimation(Source).FDrawBeforeDef; // 是否绘制

    FCurrentFrame := TAnimation(Source).FCurrentFrame;
    FLastFrameTick := TAnimation(Source).FLastFrameTick;
    FCurrentCount := TAnimation(Source).FCurrentCount;

    Changed;
  end;
end;

procedure TAnimation.Changed;
begin
  if Assigned(FOnChange) then
    FOnChange(Self);
end;

constructor TAnimation.Create(AOnwer:TDxImageForm);
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
end;

destructor TAnimation.Destroy;
begin

  inherited;
end;

procedure TAnimation.Paint;
var
  D:TTexture;
  nX, nY:Integer;
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

  if MyGetTickCount - FLastFrameTick >= FFrameTime then begin
    Inc(FCurrentFrame);

    if Assigned(FOwner.FOnAnimationFrameChanged) then
      FOwner.FOnAnimationFrameChanged(FOwner, AnimationIndex, FCurrentCount, FCurrentFrame);

    if (FCurrentFrame > FEndIndex) then begin
      FCurrentFrame := FStartIndex;

      if FPlayCount > 0 then begin
        Inc(FCurrentCount);
      end;
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
    end
    else begin
      if FBlendDraw then
        BlendMode := Blend_SrcAlphaColor
      else
        BlendMode := 2;

      GameCanvas.Draw(R.Left, R.Top, D, BlendMode);
    end;
  end;
end;

procedure TAnimation.SetOutsideAreaDraw(const Value:Boolean);
begin
  if FOutsideAreaDraw <> Value then
    FOutsideAreaDraw := Value;
end;

procedure TAnimation.SetDraw(const Value:Boolean);
begin
  if FDraw <> Value then begin
    FDraw := Value;

    FCurrentFrame := FStartIndex;
    FLastFrameTick := MyGetTickCount;
    FCurrentCount := 0;
  end;
end;

procedure TAnimation.SetBlendDraw(const Value:Boolean);
begin
  FBlendDraw := Value;
end;

procedure TAnimation.SetEndIndex(const Value:Integer);
begin
  FEndIndex := Value;
end;

procedure TAnimation.SetFrameTime(const Value:Integer);
begin
  FFrameTime := Value;
end;

procedure TAnimation.SetImageType(const Value:TImageType);
begin
  if FImageType <> Value then begin
    FImageType := Value;
    if Assigned(FOnGetImage) then
      FOnGetImage(Self, FImageType, TObject(FImage));
    Changed;
  end;
end;

procedure TAnimation.SetUseImageOffset(const Value:Boolean);
begin
  FUseImageOffset := Value;
end;

procedure TAnimation.SetOffsetX(const Value:Integer);
begin
  FOffsetX := Value;
end;

procedure TAnimation.SetOffsetY(const Value:Integer);
begin
  FOffsetY := Value;
end;

procedure TAnimation.SetOnGetImage(const Value:TOnGetImage);
begin
  FOnGetImage := Value;
  if Assigned(FOnGetImage) then
    FOnGetImage(Self, FImageType, TObject(FImage));
  Changed;
end;

procedure TAnimation.SetPlayCount(const Value:Integer);
begin
  FPlayCount := Value;
end;

procedure TAnimation.SetStartIndex(const Value:Integer);
begin
  FStartIndex := Value;
end;

{---------------------------------------------------------------------------------}

constructor TDxImageForm.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
begin
  inherited Create(AOwner);
  // InitializeCriticalSection(FCriticalSection);
  // Center := True;
  Floating := True;
  DrawBorder := False;
  Width := 200;
  Height := 100;

  FBackgroundAlpha := 0;
  //FBackgroundColor := clBlack;

  FIsBringToFront := True;

  FAnimation1 := TAnimation.Create(Self);
  FAnimation1.FAnimationIndex := 0;

  FAnimation2 := TAnimation.Create(Self);
  FAnimation2.FAnimationIndex := 1;

  FAnimation3 := TAnimation.Create(Self);
  FAnimation3.FAnimationIndex := 2;

  FNoMove := False;
end;

destructor TDxImageForm.Destroy;
begin
  // DeleteCriticalSection(FCriticalSection);

  FAnimation1.Free;
  FAnimation2.Free;
  FAnimation3.Free;

  inherited;
end;

procedure TDxImageForm.DoShow();
begin
  BringToFront;
  inherited;
end;

procedure TDxImageForm.Assign(Source:TDxControl);
begin
  if Source is TDxImageForm then begin
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
    BackgroundAlpha := TDxImageForm(Source).BackgroundAlpha;

    ImageIndex.Assign(Source.ImageIndex);
    BorderColor.Assign(Source.BorderColor);

    Animation1.Assign(TDxImageForm(Source).Animation1);
    Animation2.Assign(TDxImageForm(Source).Animation2);
    Animation3.Assign(TDxImageForm(Source).Animation3);
  end;
end;

procedure TDxImageForm.DoHide();
begin
  RootCtrl.DeleteModalForm(Self);
  inherited;
end;

procedure TDxImageForm.BringToFront();
begin
  if FIsBringToFront then begin
    if Assigned(FOnBringToFront) then begin
      FOnBringToFront(Self);
    end;
    inherited;

    if Assigned(FOnAfterBringToFront) then begin
      FOnAfterBringToFront(Self);
    end;
  end;
end;

procedure TDxImageForm.BringToFrontEx();
begin
  if FIsBringToFront then begin
    inherited BringToFront();
  end;
end;

procedure TDxImageForm.SetOnGetImage(Value:TOnGetImage);
begin
  inherited;
  FAnimation1.OnGetImage := Value;
  FAnimation2.OnGetImage := Value;
  FAnimation3.OnGetImage := Value;
end;

procedure TDxImageForm.DoMouseDown();
begin

end;

procedure TDxImageForm.DoMouseMove();
begin

end;

procedure TDxImageForm.DoMouseUp();
begin

end;

procedure TDxImageForm.MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
begin
  inherited MouseDown(Button, Shift, X, Y);
  BringToFront;
end;

function TDxImageForm.InRange(X, Y:Integer):Boolean;
begin
  if not Designing then
    Result := inherited InRange(X, Y)
  else if PointInRect(Point(X, Y), VisibleRect) then
    Result := True
  else
    Result := False;
end;

procedure TDxImageForm.MouseMove(Shift:TShiftState; X, Y:Integer);
var
  Texture:TTexture;
  FaceIndex:Integer;
  vRect:TRect;
begin
  inherited MouseMove(Shift, X, Y);
  if PointInRect(Point(X, Y), VisibleRect) then begin

    if Designing and (ImageIndex.Image <> nil) then begin
      FaceIndex := -1;

      if ImageIndex.Up >= 0 then
        FaceIndex := ImageIndex.Up
      else if ImageIndex.Hot >= 0 then
        FaceIndex := ImageIndex.Hot
      else if ImageIndex.Down >= 0 then
        FaceIndex := ImageIndex.Down;

      if FaceIndex >= 0 then begin
        Texture := ImageIndex.Image.Images[FaceIndex];
        if (Texture <> nil) and (Texture.Width * Texture.Height > 4) then begin
          vRect := VirtualRect;
        end;
      end;
    end;
  end;
end;

function TDxImageForm.IsKeyMsg(var Msg:TMsg):Boolean;
var
  Wnd:HWND;
begin
  Result := False;
  with Msg do
    if (Message >= WM_KEYFIRST) and (Message <= WM_KEYLAST) then begin
      Wnd := GetCapture;
      if Wnd = 0 then begin
        Wnd := HWnd;
        if (Application.MainForm <> nil) and (Wnd = Application.MainForm.ClientHandle) then
          Wnd := Application.MainForm.Handle
        else begin
          // Find the nearest VCL component.  Non-VCL windows wont know what
          // to do with CN_BASE offset messages anyway.
          // TOleControl.WndProc needs this for TranslateAccelerator
          while (FindControl(Wnd) = nil) and (Wnd <> 0) do
            Wnd := GetParent(Wnd);
          if Wnd = 0 then Wnd := HWnd;
        end;
        if SendMessage(Wnd, CN_BASE + Message, WParam, LParam) <> 0 then
          Result := True;
      end
      else if (LongWord(GetWindowLong(Wnd, GWL_HINSTANCE)) = HInstance) then begin
        if SendMessage(Wnd, CN_BASE + Message, WParam, LParam) <> 0 then
          Result := True;
      end;
    end;
end;

procedure TDxImageForm.ProcessMessages;
var
  Msg:TMsg;
begin
  while ProcessMessage(Msg) do {loop};
end;

function TDxImageForm.ProcessMessage(var Msg:TMsg):Boolean;
var
  Handled:Boolean;
begin
  Result := False;

  // 补上这个，不然在CPU占用较高时，技能设置一直PeekMessage返回True 2020-10-30
  if not Visible then Exit;

  if PeekMessage(Msg, 0, 0, 0, PM_REMOVE) then begin
    Result := True;
    if Msg.Message <> WM_QUIT then begin
      Handled := False;
      if Assigned(Application.OnMessage) then Application.OnMessage(Msg, Handled);
      if not Handled and not IsKeyMsg(Msg) then begin
        TranslateMessage(Msg);
        DispatchMessage(Msg);
      end;
    end;
  end;
end;

function TDxImageForm.ShowModalEx:Integer;
begin
  DialogResult := mrNone;
  Visible := True;
  RootCtrl.ModalForm := Self;
  Result := 0;
end;

function TDxImageForm.ShowModal(Process:TNotifyEvent):Integer;
begin
  DialogResult := mrNone;
  Visible := True;
  RootCtrl.ModalForm := Self;
  while True do begin
    if (not Visible) or Application.Terminated then break;
    ProcessMessages;
    if (not Visible) or Application.Terminated then break;
    Sleep(1);
    if Assigned(Process) then
      Process(Self);
  end;
  Result := 0;
end;

procedure TDxImageForm.SetBackgroundAlpha(Value:Byte);
begin
  if FBackgroundAlpha <> Value then
    FBackgroundAlpha := Value;
end;

function TDxImageForm.ShowModal:Integer;
begin
  DialogResult := mrNone;
  Visible := True;
  RootCtrl.ModalForm := Self;
  while True do begin
    if (not Visible) or Application.Terminated then break;
    ProcessMessages;
    Sleep(1);
  end;
  Result := 0;
end;

function TDxImageForm.CanMove:Boolean;
begin
  Result := not FNoMove;
end;

procedure TDxImageForm.Paint;
var
  I:Integer;
  Texture:TTexture;
  vtRect, vtRect2:TRect;
  vbRect:TRect;
begin
  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := VirtualRect;

  if FAnimation1.DrawBeforeDef then
    FAnimation1.Paint;

  if FAnimation2.DrawBeforeDef then
    FAnimation2.Paint;

  if FAnimation3.DrawBeforeDef then
    FAnimation3.Paint;

  if Assigned(OnStartPaint) then
    OnStartPaint(Self);

  DoPaint();

  if Assigned(OnStartPaint) then
    OnStartPaint(Self);

  if Assigned(OnPaint) then begin
    OnPaint(Self);
  end else if (ImageIndex.Image <> nil) and (ImageIndex.Up >= 0) then begin
    Texture := ImageIndex.Image.Images[ImageIndex.Up];
    if Texture <> nil then begin
      vtRect2 := vtRect;
      vtRect2.Left := vtRect.Left + ImageIndex.OffsetX;
      vtRect2.Top := vtRect.Top + ImageIndex.OffsetY;
      DrawRect(vtRect2, vtRect, vbRect, Texture);
    end;
  end
  else if FBackgroundAlpha > 0 then begin
    GameCanvas.FillRectAlpha(vbRect, BackgroundColor, FBackgroundAlpha);
  end;

  {
  if Assigned(OnStartPaint) then
    OnStartPaint(Self);
  }

  if not FAnimation1.DrawBeforeDef then
    FAnimation1.Paint;

  if not FAnimation2.DrawBeforeDef then
    FAnimation2.Paint;

  if not FAnimation3.DrawBeforeDef then
    FAnimation3.Paint;

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
{-------------------------------------------------------------------------------}

procedure TDxFormShapeImage.SetOnGetImage(Value:TOnGetImage);
begin
  FOnGetImage := Value;
  if Assigned(FOnGetImage) then
    FOnGetImage(Self, FImageType, TObject(FImage));
  Changed;
end;

procedure TDxFormShapeImage.SetImage(Value:TGameImages);
begin
  if FImage <> Value then begin
    FImage := Value;
    Changed;
  end;
end;

procedure TDxFormShapeImage.SetImageType(Value:TImageType);
begin
  if FImageType <> Value then begin
    FImageType := Value;
    if Assigned(FOnGetImage) then
      FOnGetImage(Self, FImageType, TObject(FImage));
    Changed;
  end;
end;

procedure TDxFormShapeImage.SetImageIndex(Value:Integer);
var
  Texture:TTexture;
  nIndex:Integer;
begin
  if FImageIndex <> Value then begin
    FImageIndex := Value;
    if (FImage <> nil) then begin
      nIndex := FImageIndex;
      if nIndex >= 0 then begin
        Texture := FImage.Images[nIndex];
        if (Texture <> nil) and (Texture.Width * Texture.Height > 4) then begin
          // FSourceRect := ShortRect(FSourceRect, Texture.ClientRect);
          FSourceRect := Texture.ClientRect;
          if (FSourceRect.Left = 0) and (FSourceRect.Top = 0) and (FSourceRect.Right = 0) and (FSourceRect.Bottom = 0) then begin
            FSourceRect := Texture.ClientRect;
            FDestRect := Texture.ClientRect;
          end;
          if FAutoDestRectSize then
            FDestRect := Bounds(FDestRect.Left, FDestRect.Top, Texture.Width, Texture.Height);
        end;
      end;
    end;
    Changed;
  end;
end;

procedure TDxFormShapeImage.SetAlign(Value:TAlignEx);
begin
  if FAlign <> Value then begin
    FAlign := Value;
    Changed;
  end;
end;

procedure TDxFormShapeImage.SetDraw(Value:Boolean);
begin
  if FDraw <> Value then begin
    FDraw := Value;
    Changed;
  end;
end;

procedure TDxFormShapeImage.SetStretch(Value:Boolean);
begin
  if FStretch <> Value then begin
    FStretch := Value;
    Changed;
  end;
end;

procedure TDxFormShapeImage.SetCenter(Value:Boolean);
begin
  if FCenter <> Value then begin
    FCenter := Value;
    Changed;
  end;
end;

function TDxFormShapeImage.GetSourcePosition(const Index:Integer):Integer;
begin
  case Index of
    2:Result := FSourceRect.Right - FSourceRect.Left;
    3:Result := FSourceRect.Bottom - FSourceRect.Top;
    else
      Result := 0;
  end;
end;

procedure TDxFormShapeImage.SetSourcePosition(const Index, Value:Integer);
var
  Aux:Integer;
  NewRect:TRect;
begin
  NewRect := FSourceRect;
  case Index of
    0:begin
        Aux := NewRect.Right - NewRect.Left;
        NewRect.Left := Value;
        NewRect.Right := Value + Aux;
      end;
    1:begin
        Aux := NewRect.Bottom - NewRect.Top;
        NewRect.Top := Value;
        NewRect.Bottom := Value + Aux;
      end;
    2:NewRect.Right := NewRect.Left + Value;
    3:NewRect.Bottom := NewRect.Top + Value;
  end;
  FSourceRect := NewRect;
end;

function TDxFormShapeImage.GetDestPosition(const Index:Integer):Integer;
begin
  case Index of
    2:Result := FDestRect.Right - FDestRect.Left;
    3:Result := FDestRect.Bottom - FDestRect.Top;
    else
      Result := 0;
  end;
end;

procedure TDxFormShapeImage.SetDestPosition(const Index, Value:Integer);
var
  Aux:Integer;
  NewRect:TRect;
begin
  NewRect := FDestRect;
  case Index of
    0:begin
        Aux := NewRect.Right - NewRect.Left;
        NewRect.Left := Value;
        NewRect.Right := Value + Aux;
      end;
    1:begin
        Aux := NewRect.Bottom - NewRect.Top;
        NewRect.Top := Value;
        NewRect.Bottom := Value + Aux;
      end;
    2:NewRect.Right := NewRect.Left + Value;
    3:NewRect.Bottom := NewRect.Top + Value;
  end;
  FDestRect := NewRect;
end;

procedure TDxFormShapeImage.Changed;
begin
  if Assigned(FOnChange) then
    FOnChange(Self);
end;

constructor TDxFormShapeImage.Create;
begin
  inherited;
  FAlign := alxNone;
  FImage := nil;
  FImageType := Prguse_wil; // 图库
  FImageIndex := -1; // 图片序号
  FDraw := False; // 是否绘制
  FStretch := False; // 缩放绘制模式
  FCenter := False; // 绘制在中间
  FAutoDestRectSize := False;
  FSourceRect := Rect(0, 0, 0, 0); // 需要绘制图片矩形
  FDestRect := Rect(0, 0, 0, 0); // 绘制在目标矩形
  FBlendMode := Blend_Default;
end;

destructor TDxFormShapeImage.Destroy;
begin
  inherited;
end;

procedure TDxFormShapeImage.SetAutoDestRectSize(Value:Boolean);
begin
  if FAutoDestRectSize <> Value then begin
    FAutoDestRectSize := Value;
    Changed;
  end;
end;

procedure TDxFormShapeImage.Assign(Source:TPersistent);
begin
  if Source is TDxFormShapeImage then begin
    Image := TDxFormShapeImage(Source).Image;
    OnGetImage := TDxFormShapeImage(Source).OnGetImage;
    ImageType := TDxFormShapeImage(Source).ImageType;

    ImageIndex := TDxFormShapeImage(Source).ImageIndex;

    Draw := TDxFormShapeImage(Source).Draw;
    Stretch := TDxFormShapeImage(Source).Stretch;
    Center := TDxFormShapeImage(Source).Center;

    SourceLeft := TDxFormShapeImage(Source).SourceLeft;

    SourceTop := TDxFormShapeImage(Source).SourceTop;
    SourceWidth := TDxFormShapeImage(Source).SourceWidth;
    SourceHeight := TDxFormShapeImage(Source).SourceHeight;

    DestLeft := TDxFormShapeImage(Source).DestLeft;
    DestTop := TDxFormShapeImage(Source).DestTop;
    DestWidth := TDxFormShapeImage(Source).DestWidth;
    DestHeight := TDxFormShapeImage(Source).DestHeight;
    AutoDestRectSize := TDxFormShapeImage(Source).AutoDestRectSize;
    Changed;
  end;
end;

{---------------------------------------------------------------------------}

function TDxImageFormShape.GetImageCount:Integer;
begin
  Result := Length(FDxFormShapeInfo);
end;

function TDxImageFormShape.GetShapeImages(Index:Integer):TDxFormShapeImage;
begin
  Result := FDxFormShapeInfo[Index];
end;

procedure TDxImageFormShape.ImageIndexChange(Sender:TObject);
var
  Texture:TTexture;
  nIndex:Integer;
  ShapeImage:TDxFormShapeImage;
begin
  ShapeImage := TDxFormShapeImage(Sender);
  if (ShapeImage.Image <> nil) then begin
    nIndex := ShapeImage.ImageIndex;
    if nIndex >= 0 then begin
      Texture := ShapeImage.Image.Images[nIndex];
      if (Texture <> nil) and (Texture.Width * Texture.Height > 4) then begin
        // ShapeImage.SourceRect := ShortRect(ShapeImage.SourceRect, Texture.ClientRect);
        ShapeImage.SourceRect := Texture.ClientRect;
        if (ShapeImage.SourceRect.Left = 0) and (ShapeImage.SourceRect.Top = 0) and (ShapeImage.SourceRect.Right = 0) and (ShapeImage.SourceRect.Bottom = 0) then begin
          ShapeImage.SourceRect := Texture.ClientRect;
          ShapeImage.DestRect := Texture.ClientRect;
        end;
        if ShapeImage.AutoDestRectSize then
          ShapeImage.DestRect := Bounds(ShapeImage.DestRect.Left, ShapeImage.DestRect.Top, Texture.Width, Texture.Height);
      end;
    end;
  end;
end;

constructor TDxImageFormShape.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
var
  I:Integer;
  P:Pointer;
begin
  inherited Create(AOwner);
  P := @FDraw1;
  for I := 0 to Length(FDxFormShapeInfo) - 1 do begin
    FDxFormShapeInfo[I] := TDxFormShapeImage.Create;
    FDxFormShapeInfo[I].OnChange := ImageIndexChange;
    FDxFormShapeInfo[I].OnGetImage := OnGetImage;
    TDxFormShapeImage(P^) := FDxFormShapeInfo[I];
    Inc(Integer(P), 4);
  end;
end;

destructor TDxImageFormShape.Destroy;
var
  I:Integer;
begin
  for I := 0 to Length(FDxFormShapeInfo) - 1 do
    FDxFormShapeInfo[I].Free;
  inherited;
end;

procedure TDxImageFormShape.SetOnGetImage(Value:TOnGetImage);
var
  I:Integer;
begin
  for I := 0 to Length(FDxFormShapeInfo) - 1 do
    FDxFormShapeInfo[I].OnGetImage := Value;
  inherited;
end;

procedure TDxImageFormShape.Paint;
var
  I, nLeft, nTop, nWidth, nHeight:Integer;
  Texture:TTexture;
  vtRect:TRect;
  vbRect:TRect;
  SourceRect:TRect;
  DestRect:TRect;
begin
  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := VirtualRect;

  DoPaint();

  if Assigned(OnStartPaint) then
    OnStartPaint(Self);

  if Assigned(OnPaint) then
    OnPaint(Self)
  else if ImageIndex.Image <> nil then begin
    if ImageIndex.Up >= 0 then begin
      Texture := ImageIndex.Image.Images[ImageIndex.Up];
      if Texture <> nil then begin
        // Canvas.Draw(0,0, Texture, Transparent);
        DrawRect(vtRect, vtRect, vbRect, Texture);
      end;
    end;
  end;

  for I := 0 to Length(FDxFormShapeInfo) - 1 do begin
    if FDxFormShapeInfo[I].Draw and (FDxFormShapeInfo[I].Image <> nil) then begin
      Texture := FDxFormShapeInfo[I].Image.Images[FDxFormShapeInfo[I].ImageIndex];
      if (Texture <> nil) and (Texture.Width * Texture.Height > 4) then begin
        SourceRect := ShortRect(Texture.ClientRect, FDxFormShapeInfo[I].SourceRect);
        DestRect := ShortRect(Bounds(0, 0, Width, Height), FDxFormShapeInfo[I].DestRect);
        if (SourceRect.Bottom > SourceRect.Top) and (SourceRect.Right > SourceRect.Left) and
          (DestRect.Bottom > DestRect.Top) and (DestRect.Right > DestRect.Left)
          then begin

          case FDxFormShapeInfo[I].Align of
            alxNone:begin
                nLeft := vtRect.Left + DestRect.Left;
                nTop := vtRect.Top + DestRect.Top;
                if FDxFormShapeInfo[I].Center then begin
                  nWidth := SourceRect.Right - SourceRect.Left;
                  nHeight := SourceRect.Bottom - SourceRect.Top;
                  nLeft := vtRect.Left + ((vtRect.Right - vtRect.Left) - nWidth) div 2;
                  nTop := vtRect.Top + ((vtRect.Bottom - vtRect.Top) - nHeight) div 2;
                  // GameCanvas.Draw(vtRect.Left + nLeft, vtRect.Top + nTop, SourceRect, Texture);
                {end else begin
                  if FDxFormShapeInfo[I].Stretch then begin
                    DestRect := Bounds(vtRect.Left + DestRect.Left, vtRect.Top + DestRect.Top, SourceRect.Right - SourceRect.Left, SourceRect.Bottom - SourceRect.Top);
                    GameCanvas.StretchDraw(DestRect, SourceRect, Texture, FDxFormShapeInfo[I].BlendMode)
                  end else
                    GameCanvas.Draw(vtRect.Left + DestRect.Left, vtRect.Top + DestRect.Top, SourceRect, Texture, FDxFormShapeInfo[I].BlendMode); }
                end;
              end;
            alxTop:begin
                nLeft := vtRect.Left;
                nTop := vtRect.Top;
              end;
            alxBottom:begin
                nLeft := vtRect.Left;
                nTop := vtRect.Top + (vtRect.Bottom - vtRect.Top) - (SourceRect.Bottom - SourceRect.Top);

              end;
            alxLeft:begin
                nLeft := vtRect.Left;
                nTop := vtRect.Top;
              end;
            alxRight:begin
                nLeft := vtRect.Left + (vtRect.Right - vtRect.Left) - (SourceRect.Right - SourceRect.Left);
                nTop := vtRect.Top;
              end;
            alxClient:begin
                nLeft := vtRect.Left;
                nTop := vtRect.Top;
                if FDxFormShapeInfo[I].Stretch then
                  DestRect := Bounds(nLeft, nTop, vtRect.Right - vtRect.Left, vtRect.Bottom - vtRect.Top)
              end;
            alxTopLeft:begin
                nLeft := vtRect.Left;
                nTop := vtRect.Top;
              end;
            alxTopRight:begin
                nLeft := vtRect.Left + (vtRect.Right - vtRect.Left) - (SourceRect.Right - SourceRect.Left);
                nTop := vtRect.Top;
              end;
            alxBottomLeft:begin
                nLeft := vtRect.Left;
                nTop := vtRect.Top + (vtRect.Bottom - vtRect.Top) - (SourceRect.Bottom - SourceRect.Top);
              end;
            alxBottomRight:begin
                nLeft := vtRect.Left + (vtRect.Right - vtRect.Left) - (SourceRect.Right - SourceRect.Left);
                nTop := vtRect.Top + (vtRect.Bottom - vtRect.Top) - (SourceRect.Bottom - SourceRect.Top);
              end;
          end;
          if FDxFormShapeInfo[I].Stretch then begin
            DestRect := Bounds(nLeft, nTop, DestRect.Right - DestRect.Left, DestRect.Bottom - DestRect.Top);
            GameCanvas.StretchDraw(DestRect, SourceRect, Texture, FDxFormShapeInfo[I].BlendMode);
          end
          else
            GameCanvas.Draw(nLeft, nTop, SourceRect, Texture, FDxFormShapeInfo[I].BlendMode);
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

end.
