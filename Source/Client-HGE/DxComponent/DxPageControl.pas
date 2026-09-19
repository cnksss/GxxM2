unit DxPageControl;

interface
uses
  Windows,
  Types,
  Classes,
  Controls,
  SysUtils,
  StdCtrls,
  Graphics,
  HGE,
  DxComponents,
  HGECanvas,
  ComCtrls,
  DxControls;

type
  TDxTabSheet = class(TDxControl)
  private
    FCaptionColor:TDxCaptionColor;
    FOffSetX:Integer;
    FOffSetY:Integer;

    FTabVisible:Boolean;
  protected
    function GetMaxTabOrder:Integer;
    procedure SetTabVisible(Value:Boolean);
    procedure DoResize(var NewRect:TRect); override;
    procedure Notification(AComponent:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}; Operation:TOperation); override;
  public
    function InRange(X, Y:Integer):Boolean; override;
    function CanDraw:Boolean; override;
  public
    constructor Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}); override;
    destructor Destroy; override;
    procedure Paint; override;
    procedure Assign(Source:TDxControl); override;
  published
    property Caption;
    property TabVisible:Boolean read FTabVisible write SetTabVisible default True;
    property CaptionColor:TDxCaptionColor read FCaptionColor write FCaptionColor;
    property OffSetX:Integer read FOffSetX write FOffSetX default 0;
    property OffSetY:Integer read FOffSetY write FOffSetY default 0;
  end;

  TDxPageControl = class(TDxControl)
  private
    FTabPosition:TTabPosition;

    FActivePage:TDxControl;
    FActivePageIndex:Integer;

    FButtonWidth:Integer;
    FButtonHeight:Integer;

    FClientLeft:Integer;
    FClientTop:Integer;
    FClientWidth:Integer;
    FClientHeight:Integer;

    FButtonDown:array of Boolean;
    FButtonMove:array of Boolean;

    FShowButton:Boolean;
    FDrawButton:Boolean;

    FOnTabSheetCreate:TNotifyEvent;
    FOnTabSheetDestroy:TNotifyEvent;
    FOnActivePageChange:TNotifyEvent;

    FOnTabButtonMouseDown:TMouseEvent;

    FOffSetX, FOffSetY:Integer;
    FCriticalSection:TRTLCriticalSection;

    FCaptionOffsetX:Integer;
    FCaptionOffsetY:Integer;
    FDownCaptionOffsetX:Integer;
    FDownCaptionOffsetY:Integer;

    FReverseDrawButton:Boolean;

    FVisiblePageList:TStringList;
    procedure SetClientLeft(Value:Integer);
    procedure SetClientTop(Value:Integer);
    procedure SetClientWidth(Value:Integer);
    procedure SetClientHeight(Value:Integer);

    function GetClientLeft:Integer;
    function GetClientTop:Integer;
    function GetClientWidth:Integer;
    function GetClientHeight:Integer;

    procedure SetButtonWidth(Value:Integer);
    procedure SetButtonHeight(Value:Integer);
    procedure SetTabPosition(Value:TTabPosition);

    procedure SetActivePageIndex(Value:Integer);
    procedure SetActivePage(Value:TDxControl);
    procedure Put(Index:Integer; Value:TDxControl);
    function Get(Index:Integer):TDxControl;

    procedure ReTabsize(AWidth, AHeight:Integer);

    function GetPageCount:Integer;

    procedure ClearButtonState;
    function SetButtonDown(X, Y:Integer; vRect:TRect):Integer;
    function SetButtonMove(X, Y:Integer; vRect:TRect):Integer;
    function ButtonDownState(Index:Integer):Boolean;
    function ButtonMoveState(Index:Integer):Boolean;
    function InButtonRange(X, Y:Integer; vRect:TRect):Boolean;
    procedure ButtonDown(Index:Integer);
    procedure ButtonMove(Index:Integer);
    procedure SetShowButton(Value:Boolean);

  protected
    procedure SetPageCount(Value:Integer);
    function GetTabSheet:TDxTabSheet;
    procedure SetTabSheet(Value:TDxTabSheet);
  protected
    procedure DoResize(var NewRect:TRect); override;
    procedure DrawTabSheetButton(TabSheet:TDxTabSheet; Index:Integer);
  public
    function InRange(X, Y:Integer):Boolean; override;
    procedure MouseMove(Shift:TShiftState; X, Y:Integer); override;
    procedure MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); override;
    procedure MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); override;
  public
    constructor Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}); override;
    destructor Destroy; override;
    procedure Paint; override;
    procedure Clear;
    function Add:TDxTabSheet;
    procedure AddTabSheet(TabSheet:TDxTabSheet);
    procedure RemoveTabSheet(TabSheet:TDxTabSheet);
    procedure Lock;
    procedure UnLock;
    property Pages[Index:Integer]:TDxControl read Get write Put;
    property OnActivePageChange:TNotifyEvent read FOnActivePageChange write FOnActivePageChange;
    property OnTabButtonMouseDown:TMouseEvent read FOnTabButtonMouseDown write FOnTabButtonMouseDown;

    property OnTabSheetCreate:TNotifyEvent read FOnTabSheetCreate write FOnTabSheetCreate;
    property OnTabSheetDestroy:TNotifyEvent read FOnTabSheetDestroy write FOnTabSheetDestroy;
  published
    property Align;
    property ShowButton:Boolean read FShowButton write SetShowButton;
    property DrawButton:Boolean read FDrawButton write FDrawButton;
    property ClientLeft:Integer read GetClientLeft write SetClientLeft;
    property ClientTop:Integer read GetClientTop write SetClientTop;
    property ClientWidth:Integer read GetClientWidth write SetClientWidth;
    property ClientHeight:Integer read GetClientHeight write SetClientHeight;

    property TabPosition:TTabPosition read FTabPosition write SetTabPosition;

    property PageCount:Integer read GetPageCount; // write SetPageCount;
    property ActivePageIndex:Integer read FActivePageIndex write SetActivePageIndex;
    property ActivePage:TDxControl read FActivePage write SetActivePage;

    property ButtonWidth:Integer read FButtonWidth write SetButtonWidth;
    property ButtonHeight:Integer read FButtonHeight write SetButtonHeight;
    property OffSetX:Integer read FOffSetX write FOffSetX;
    property OffSetY:Integer read FOffSetY write FOffSetY;

    property CaptionOffsetX:Integer read FCaptionOffsetX write FCaptionOffsetX default 0;
    property CaptionOffsetY:Integer read FCaptionOffsetY write FCaptionOffsetY default 0;
    property DownCaptionOffsetX:Integer read FDownCaptionOffsetX write FDownCaptionOffsetX default 0;
    property DownCaptionOffsetY:Integer read FDownCaptionOffsetY write FDownCaptionOffsetY default 0;
    property ReverseDrawButton:Boolean read FReverseDrawButton write FReverseDrawButton default False;
  end;

implementation
uses Math,
  HGEFontEx;

constructor TDxTabSheet.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
begin
  inherited Create(AOwner);
  FOffSetX := 0;
  FOffSetY := 0;
  AutoSize := False;
  Alignment := taCenter;
  Caption := Name;
  OwnerMove := True;
  FTabVisible := True;
  FCaptionColor := TDxCaptionColor.Create;
  TabOrder := GetMaxTabOrder + 1;
  if Owner <> nil then
    TDxPageControl(Owner).AddTabSheet(Self);
end;

destructor TDxTabSheet.Destroy;
begin
  FCaptionColor.Free;
  if Owner <> nil then
    TDxPageControl(Owner).RemoveTabSheet(Self);
  inherited Destroy;
end;

function TDxTabSheet.GetMaxTabOrder:Integer;
var
  I, nTabOrder:Integer;
  TabSheet:TDxTabSheet;
begin
  nTabOrder := 0;
  if Owner <> nil then begin
    for I := 0 to TDxControl(Owner).ControlCount - 1 do begin
      if (TDxControl(Owner).Control[I] is TDxTabSheet) then begin
        TabSheet := TDxTabSheet(TDxControl(Owner).Control[I]);
        if TabSheet.TabOrder >= nTabOrder then begin
          nTabOrder := TabSheet.TabOrder;
        end;
      end;
    end;
  end;
  Result := nTabOrder;
end;
// ------------------------------------------------------------------------------

procedure TDxTabSheet.Assign(Source:TDxControl);
begin
  inherited;
  if Source is TDxTabSheet then begin
    OffSetX := TDxTabSheet(Source).OffSetX;
    OffSetY := TDxTabSheet(Source).OffSetY;
    TabVisible := TDxTabSheet(Source).TabVisible;
    CaptionColor.Assign(TDxTabSheet(Source).CaptionColor);
  end;
end;

procedure TDxTabSheet.Paint;
var
  I:Integer;
  vtRect, vbRect:TRect;
begin
  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := VirtualRect;
  DoPaint();

  if Designing then
    FrameRect(vtRect, vtRect, vbRect, BorderColor.Up.Color);

  if Assigned(OnPaint) then
    OnPaint(Self);

  for I := ControlCount - 1 downto 0 do
    if (Control[I].Visible) then begin
      Control[I].Paint;
    end;
end;

procedure TDxTabSheet.SetTabVisible(Value:Boolean);
begin
  if FTabVisible <> Value then begin
    FTabVisible := Value;
    Visible := FTabVisible;
    if FTabVisible then begin
      TDxPageControl(Owner).AddTabSheet(Self);
    end else begin
      TDxPageControl(Owner).RemoveTabSheet(Self);
    end;
  end;
end;

procedure TDxTabSheet.Notification(AComponent:{$IF CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND};
  Operation:TOperation);
begin
  inherited;
  {
  case Operation of
    opRemove: begin
         // DebugOut('TDxTabSheet.Notification opRemove '+Name +' '+Owner.Name);
        if (Owner <> nil) then TDxPageControl(Owner).RemoveTabSheet(Self);
      end;
    opInsert: begin
      DebugOut('TDxTabSheet.Notification opInsert '+Name +' '+Owner.Name);
        // if (Owner <> nil) then TDxPageControl(Owner).AddTabSheet(Self);
      end;
  end;}
end;

function TDxTabSheet.CanDraw:Boolean;
begin
  if FTabVisible and (TDxPageControl(Owner).ActivePage = Self) then
    Result := inherited CanDraw
  else
    Result := False;
end;

function TDxTabSheet.InRange(X, Y:Integer):Boolean;
var
  boInrange:Boolean;
  vtRect:TRect;
begin
  if FTabVisible and PointInRect(Point(X, Y), VisibleRect) then begin
    boInrange := True;
    if Assigned(OnInRealArea) then begin
      vtRect := VirtualRect;
      OnInRealArea(Self, X - vtRect.Left, Y - vtRect.Top, boInrange);
    end;
    Result := boInrange;
  end
  else
    Result := False;
end;

procedure TDxTabSheet.DoResize(var NewRect:TRect);
begin
  with Owner as TDxPageControl do begin
    NewRect.Left := ClientLeft;
    NewRect.Top := ClientTop;
    NewRect.Right := NewRect.Left + ClientWidth;
    NewRect.Bottom := NewRect.Top + ClientHeight;
  end;
  inherited DoResize(NewRect);
end;

{------------------------------------------------------------------------------}

constructor TDxPageControl.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
begin
  inherited Create(AOwner);
  InitializeCriticalSection(FCriticalSection);

  FVisiblePageList := TStringList.Create;
  FButtonWidth := 48;
  FButtonHeight := 20;

  FTabPosition := tpTop;

  FActivePage := nil;
  FActivePageIndex := -1;

  FClientLeft := 0;
  FClientTop := 40;
  FClientWidth := 394;
  FClientHeight := 216;

  Width := 394;
  Height := 230;
  FShowButton := True;
  FDrawButton := True;
  FOnTabSheetCreate := nil;
  FOnTabSheetDestroy := nil;
  FOnActivePageChange := nil;
  FOnTabButtonMouseDown := nil;
  FOffSetX := 0;
  FOffSetY := 0;

  FCaptionOffsetX := 0;
  FCaptionOffsetY := 0;
  FDownCaptionOffsetX := 0;
  FDownCaptionOffsetY := 0;

  OwnerMove := True;

  FReverseDrawButton := False;
end;

destructor TDxPageControl.Destroy;
begin
  FVisiblePageList.Free;
  DeleteCriticalSection(FCriticalSection);
  inherited Destroy;
end;

// ------------------------------------------------------------------------------

procedure TDxPageControl.Lock;
begin
  EnterCriticalSection(FCriticalSection);
end;

procedure TDxPageControl.UnLock;
begin
  LeaveCriticalSection(FCriticalSection);
end;

procedure TDxPageControl.Clear;
begin
  Lock;
  try
    while ControlCount > 0 do
      Control[0].Free;
    SetLength(FButtonDown, 0);
    SetLength(FButtonMove, 0);
  finally
    UnLock;
  end;
end;

procedure TDxPageControl.ClearButtonState;
begin
  Lock;
  try
    SetLength(FButtonDown, 0);
    SetLength(FButtonMove, 0);
    SetLength(FButtonDown, FVisiblePageList.Count);
    SetLength(FButtonMove, FVisiblePageList.Count);
  finally
    UnLock;
  end;
end;

function TDxPageControl.InButtonRange(X, Y:Integer; vRect:TRect):Boolean;
begin
  if FShowButton then begin
    case FTabPosition of
      tpTop:Result := (Y <= vRect.Top + FButtonHeight) and (X >= vRect.Left + FOffSetX);
      tpBottom:Result := (Y >= vRect.Bottom - FButtonHeight) and (X >= vRect.Left + FOffSetX);
      tpLeft:Result := (X <= vRect.Left + FButtonWidth) and (Y >= vRect.Top + FOffSetY);
      tpRight:Result := (X >= vRect.Right - FButtonWidth) and (Y >= vRect.Top + FOffSetY);
    end;
  end else Result := False;
end;

procedure TDxPageControl.ButtonDown(Index:Integer);
var
  I:Integer;
begin
  if Length(FButtonDown) <> FVisiblePageList.Count then
    SetLength(FButtonDown, FVisiblePageList.Count);
  if (Index >= 0) and (Index < Length(FButtonDown)) then begin
    for I := 0 to Length(FButtonDown) - 1 do begin
      FButtonDown[I] := False;
    end;
    FButtonDown[Index] := True;
  end;
end;

procedure TDxPageControl.ButtonMove(Index:Integer);
var
  I:Integer;
begin
  if Length(FButtonMove) <> FVisiblePageList.Count then
    SetLength(FButtonMove, FVisiblePageList.Count);
  if (Index >= 0) and (Index < Length(FButtonMove)) then begin
    for I := 0 to Length(FButtonMove) - 1 do begin
      FButtonMove[I] := False;
    end;
    FButtonMove[Index] := True;
  end;
end;

function TDxPageControl.SetButtonDown(X, Y:Integer; vRect:TRect):Integer;
var
  Index:Integer;
begin
  Lock;
  try
    Index := -1;
    if InButtonRange(X, Y, vRect) then begin
      if Length(FButtonDown) <> FVisiblePageList.Count then
        SetLength(FButtonDown, FVisiblePageList.Count);
      case FTabPosition of
        tpTop, tpBottom:begin
            Index := (X - vRect.Left - FOffSetX) div FButtonWidth;
            if (Index >= 0) and (Index < FVisiblePageList.Count) then
              ButtonDown(Index)
            else
              Index := -1;
          end;
        tpLeft, tpRight:begin
            Index := (Y - vRect.Top - FOffSetY) div FButtonHeight;
            if (Index >= 0) and (Index < FVisiblePageList.Count) then
              ButtonDown(Index)
            else
              Index := -1;
          end;
      end;
    end; // else ClearButtonState;
  finally
    UnLock;
  end;
  Result := Index;
end;

function TDxPageControl.SetButtonMove(X, Y:Integer; vRect:TRect):Integer;
var
  Index:Integer;
begin
  Lock;
  try
    Index := -1;
    if InButtonRange(X, Y, vRect) then begin
      if Length(FButtonMove) <> FVisiblePageList.Count then
        SetLength(FButtonMove, FVisiblePageList.Count);
      case FTabPosition of
        tpTop, tpBottom:begin
            Index := (X - vRect.Left - FOffSetX) div FButtonWidth;
            if (Index >= 0) and (Index < FVisiblePageList.Count) then
              ButtonMove(Index)
            else
              Index := -1;
          end;
        tpLeft, tpRight:begin
            Index := (Y - vRect.Top - FOffSetY) div FButtonHeight;
            if (Index >= 0) and (Index < FVisiblePageList.Count) then
              ButtonMove(Index)
            else
              Index := -1;
          end;
      end;
    end; // else ClearButtonState;
  finally
    UnLock;
  end;
  Result := Index;
end;

function TDxPageControl.ButtonDownState(Index:Integer):Boolean;
begin
  if (Index >= 0) and (Index < Length(FButtonDown)) then
    Result := FButtonDown[Index]
  else
    Result := False;
end;

function TDxPageControl.ButtonMoveState(Index:Integer):Boolean;
begin
  if (Index >= 0) and (Index < Length(FButtonMove)) then
    Result := FButtonMove[Index]
  else
    Result := False;
end;

procedure TDxPageControl.MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
var
  Index:Integer;
begin
  if (mbLeft = Button) and FShowButton then begin
    Index := SetButtonDown(X, Y, VirtualRect);
    if Index >= 0 then begin
      ActivePageIndex := Index;
      if Assigned(FOnTabButtonMouseDown) then
        FOnTabButtonMouseDown(Self, Button, Shift, X, Y);
    end;
  end;

  inherited MouseDown(Button, Shift, X, Y);
end;

procedure TDxPageControl.MouseMove(Shift:TShiftState; X, Y:Integer);
begin
  if FShowButton then
    SetButtonMove(X, Y, VirtualRect);
  inherited MouseMove(Shift, X, Y);
end;

procedure TDxPageControl.MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
begin
  ClearButtonState;
  inherited MouseUp(Button, Shift, X, Y);
end;

function TDxPageControl.InRange(X, Y:Integer):Boolean;
var
  boInrange:Boolean;
  vRect:TRect;
begin
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

procedure TDxPageControl.DrawTabSheetButton(TabSheet:TDxTabSheet; Index:Integer);
var
  FaceIndex:Integer;
  TextRect:TRect;
  nOffSetX, nOffSetY:Integer;
  Font:TDxFont;
  HGEFont:THGEFont;
  TextImages:TImageInfos;

  Texture:TTexture;
  vtRect, PaintRect:TRect;
  II, nWidth, nHeight:Integer;
begin
  vtRect := VirtualRect;
  if TabSheet.ImageIndex.Image <> nil then begin
    if Index = FActivePageIndex then begin
      FaceIndex := TabSheet.ImageIndex.Down;
      if (FaceIndex < 0) and (TabSheet.ImageIndex.Up >= 0) then
        FaceIndex := TabSheet.ImageIndex.Up;
    end
    else begin
      FaceIndex := TabSheet.ImageIndex.Up;
    end;

    if FaceIndex >= 0 then begin
      Texture := TabSheet.ImageIndex.Image.Images[FaceIndex];
      if Texture <> nil then begin
        case FTabPosition of
          tpTop:begin
              PaintRect.Left := vtRect.Left + Index * FButtonWidth + FOffSetX + TabSheet.OffSetX + (FButtonWidth - Texture.Width) div 2;
              PaintRect.Right := PaintRect.Left + Texture.Width;
              PaintRect.Top := vtRect.Top + TabSheet.OffSetY + (FButtonHeight - Texture.Height) div 2;
              PaintRect.Bottom := PaintRect.Top + Texture.Height;
            end;
          tpBottom:begin
              PaintRect.Left := vtRect.Left + Index * FButtonWidth + FOffSetX + TabSheet.OffSetX + (FButtonWidth - Texture.Width) div 2;
              PaintRect.Right := PaintRect.Left + Texture.Width;
              PaintRect.Top := vtRect.Top + (Height - FButtonHeight) + TabSheet.OffSetY + (FButtonHeight - Texture.Height) div 2;
              PaintRect.Bottom := PaintRect.Top + Texture.Height;
            end;
          tpLeft:begin
              PaintRect.Left := vtRect.Left + TabSheet.OffSetX + (FButtonWidth - Texture.Width) div 2;
              PaintRect.Right := PaintRect.Left + Texture.Width;
              PaintRect.Top := vtRect.Top + Index * FButtonHeight + FOffSetY + TabSheet.OffSetY + (FButtonHeight - Texture.Height) div 2;
              PaintRect.Bottom := PaintRect.Top + Texture.Height;
            end;
          tpRight:begin
              PaintRect.Left := vtRect.Left + (Width - FButtonWidth) + TabSheet.OffSetX + (FButtonWidth - Texture.Width) div 2;
              PaintRect.Right := PaintRect.Left + Texture.Width;
              PaintRect.Top := vtRect.Top + Index * FButtonHeight + FOffSetY + TabSheet.OffSetY + (FButtonHeight - Texture.Height) div 2;
              PaintRect.Bottom := PaintRect.Top + Texture.Height;
            end;
        end;
        DrawRect(PaintRect, Texture);
      end;
    end;
  end; // if Image <> nil then

  if TabSheet.Caption <> '' then begin
    case FTabPosition of
      tpTop:TextRect := Bounds(Index * FButtonWidth + FOffSetX + TabSheet.OffSetX, TabSheet.OffSetY, FButtonWidth, FButtonHeight);
      tpBottom:TextRect := Bounds(Index * FButtonWidth + FOffSetX + TabSheet.OffSetX, Height - FButtonHeight + TabSheet.OffSetY, FButtonWidth, FButtonHeight);
      tpLeft:TextRect := Bounds(TabSheet.OffSetX, Index * FButtonHeight + FOffSetY + TabSheet.OffSetY, FButtonWidth, FButtonHeight);
      tpRight:TextRect := Bounds(Width - FButtonWidth + TabSheet.OffSetX, Index * FButtonHeight + FOffSetY + TabSheet.OffSetY, FButtonWidth, FButtonHeight);
    end;

    TextRect := MoveRect(TextRect, vtRect.TopLeft);

    if Index = FActivePageIndex then begin
      Font := TabSheet.CaptionColor.Down;

      nOffSetX := FDownCaptionOffsetX;
      nOffSetY := FDownCaptionOffsetY;
    end
    else begin
      if ButtonDownState(Index) then
        Font := TabSheet.CaptionColor.Down
      else if ButtonMoveState(Index) then
        Font := TabSheet.CaptionColor.Hot
      else
        Font := TabSheet.CaptionColor.Up;

      nOffSetX := FCaptionOffsetX;
      nOffSetY := FCaptionOffsetY;
    end;

    HGEFont := TextureFonts.FindFont(Font.Name, Font.Size, Font.Style);
    if HGEFont <> nil then begin
      TextImages := HGEFont.GetImageInfos(TabSheet.Caption);

      nWidth := 0;
      nHeight := 0;
      for II := 0 to Length(TextImages) - 1 do begin
        if nWidth < TextImages[II].Width then
          nWidth := TextImages[II].Width;

        if TextImages[II].Height <= 0 then
          Inc(nHeight, HGEFont.TextHeight('0'))
        else
          Inc(nHeight, TextImages[II].Height);
      end;

      if Font.Bold then begin
        Inc(nWidth, 2);
        Inc(nHeight, 2);
      end;

      TabSheet.DrawCaption(HGEFont,
        Font, TextImages,
        // TabSheet.Caption,
        Bounds(TextRect.Left, TextRect.Top, Min(nWidth, FButtonWidth), Min(nHeight, FButtonHeight)),
        vtRect,
        TextRect,
        nOffSetX, nOffSetY);
    end;
  end;
end;

procedure TDxPageControl.Paint;
var
  I, nIndex:Integer;
  vtRect, vbRect:TRect;
  TabSheet:TDxTabSheet;
  PaintPage:TDxControl;
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

  if Assigned(OnPaint) then
    OnPaint(Self)
  else if FShowButton and FDrawButton then begin
    Lock;
    try
      if not FReverseDrawButton then begin
        nIndex := 0;
        for I := 0 to FVisiblePageList.Count - 1 do begin
          TabSheet := TDxTabSheet(FVisiblePageList.Objects[I]);
          if nIndex <> ActivePageIndex then begin
            DrawTabSheetButton(TabSheet, nIndex);
          end;
          Inc(nIndex);
        end;
      end
      else begin
        nIndex := FVisiblePageList.Count - 1;
        for I := FVisiblePageList.Count - 1 downto 0 do begin
          TabSheet := TDxTabSheet(FVisiblePageList.Objects[I]);
          if nIndex <> ActivePageIndex then begin
            DrawTabSheetButton(TabSheet, nIndex);
          end;
          Dec(nIndex);
        end;
      end;

      nIndex := 0;
      for I := 0 to FVisiblePageList.Count - 1 do begin
        TabSheet := TDxTabSheet(FVisiblePageList.Objects[I]);
        if nIndex = FActivePageIndex then begin
          DrawTabSheetButton(TabSheet, nIndex);
          Break;
        end;
        Inc(nIndex);
      end;
    finally
      UnLock;
    end;
  end;

  if Assigned(OnStopPaint) then
    OnStopPaint(Self);

  Lock;
  try
    PaintPage := FActivePage;
  finally
    UnLock;
  end;

  if PaintPage <> nil then
    PaintPage.Paint;
end;

procedure TDxPageControl.SetShowButton(Value:Boolean);
begin
  if FShowButton <> Value then begin
    FShowButton := Value;
    ReTabsize(Width, Height);
  end;
end;

function TDxPageControl.GetClientLeft:Integer;
begin
  if FShowButton then
    Result := FClientLeft
  else
    Result := 0;
end;

function TDxPageControl.GetClientTop:Integer;
begin
  if FShowButton then
    Result := FClientTop
  else
    Result := 0;
end;

function TDxPageControl.GetClientWidth:Integer;
begin
  if FShowButton then
    Result := FClientWidth
  else
    Result := Width;
end;

function TDxPageControl.GetClientHeight:Integer;
begin
  if FShowButton then
    Result := FClientHeight
  else
    Result := Height;
end;

procedure TDxPageControl.SetClientLeft(Value:Integer);
begin
  if FClientLeft <> Value then begin
    FClientLeft := Value;
    ReTabsize(Width, Height);
  end;
end;

procedure TDxPageControl.SetClientTop(Value:Integer);
begin
  if FClientTop <> Value then begin
    FClientTop := Value;
    ReTabsize(Width, Height);
  end;
end;

procedure TDxPageControl.SetClientWidth(Value:Integer);
begin
  if FClientWidth <> Value then begin
    FClientWidth := Value;
    ReTabsize(Width, Height);
  end;
end;

procedure TDxPageControl.SetClientHeight(Value:Integer);
begin
  if FClientHeight <> Value then begin
    FClientHeight := Value;
    ReTabsize(Width, Height);
  end;
end;

procedure TDxPageControl.DoResize(var NewRect:TRect);
begin
  inherited;
  if ((NewRect.Right - NewRect.Left) <> Width) or
    ((NewRect.Bottom - NewRect.Top) <> Height) then
    ReTabsize(NewRect.Right - NewRect.Left, NewRect.Bottom - NewRect.Top);
end;

procedure TDxPageControl.ReTabsize(AWidth, AHeight:Integer);
var
  Index:Integer;
  TabSheet:TDxControl;
begin
  if FShowButton then begin
    case FTabPosition of
      tpTop:begin
          FClientLeft := 0;
          FClientTop := FButtonHeight;
          FClientWidth := AWidth;
          FClientHeight := AHeight - FButtonHeight;
        end;
      tpBottom:begin
          FClientLeft := 0;
          FClientTop := 0;
          FClientWidth := AWidth;
          FClientHeight := AHeight - FButtonHeight;
        end;
      tpLeft:begin
          FClientLeft := FButtonWidth;
          FClientTop := 0;
          FClientWidth := AWidth - FButtonWidth;
          FClientHeight := AHeight;
        end;
      tpRight:begin
          FClientLeft := 0;
          FClientTop := 0;
          FClientWidth := AWidth - FButtonWidth;
          FClientHeight := AHeight;
        end;
    end;
  end else begin
    FClientLeft := 0;
    FClientTop := 0;
    FClientWidth := AWidth;
    FClientHeight := AHeight;
  end;

  for Index := 0 to ControlCount - 1 do begin
    TabSheet := Control[Index];
    TabSheet.Left := FClientLeft;
    TabSheet.Top := FClientTop;
    TabSheet.Width := FClientWidth;
    TabSheet.Height := FClientHeight;
  end;

  ActivePageIndex := FActivePageIndex;
end;

procedure TDxPageControl.SetButtonWidth(Value:Integer);
begin
  if FButtonWidth <> Value then begin
    FButtonWidth := Value;
    ReTabsize(Width, Height);
  end;
end;

procedure TDxPageControl.SetButtonHeight(Value:Integer);
begin
  if FButtonHeight <> Value then begin
    FButtonHeight := Value;
    ReTabsize(Width, Height);
  end;
end;

procedure TDxPageControl.SetTabPosition(Value:TTabPosition);
begin
  if FTabPosition <> Value then begin
    FTabPosition := Value;
    ReTabsize(Width, Height);
  end;
end;

function TDxPageControl.GetPageCount:Integer;
begin
  Result := ControlCount;
end;

procedure TDxPageControl.AddTabSheet(TabSheet:TDxTabSheet);
var
  Index:Integer;
begin
  {if FVisiblePageList.IndexOfObject(TabSheet) < 0 then begin
    FVisiblePageList.AddObject(IntToStr(TabSheet.TabOrder), TabSheet);
    FVisiblePageList.CustomSort(NumberSort_2);
  end;}
  // DebugOut('AddTabSheet ' + TabSheet.Name);

  Lock;
  try
    FVisiblePageList.Clear;
    for Index := 0 to ControlCount - 1 do begin
      if TDxTabSheet(Control[Index]).TabVisible then
        FVisiblePageList.AddObject('', Control[Index]);
    end;
  finally
    UnLock;
  end;

  // if ActivePage = nil then
   // ActivePage := TabSheet;
end;

procedure TDxPageControl.RemoveTabSheet(TabSheet:TDxTabSheet);
var
  Index:Integer;
begin
  Lock;
  try
    Index := FVisiblePageList.IndexOfObject(TabSheet);
    if Index >= 0 then
      FVisiblePageList.Delete(Index);

    if ActivePage = TabSheet then
      ActivePageIndex := 0;
  finally
    UnLock;
  end;
  // DebugOut('RemoveTabSheet ' + TabSheet.Name);
end;

function TDxPageControl.Add:TDxTabSheet;
var
  TabSheet:TDxTabSheet;
begin
  TabSheet := TDxTabSheet.Create(Self);
  TabSheet.Designing := False;
  TabSheet.OnGetImage := OnGetImage;
  TabSheet.Left := FClientLeft;
  TabSheet.Top := FClientTop;
  TabSheet.Width := FClientWidth;
  TabSheet.Height := FClientHeight;
  Result := TabSheet;
end;

procedure TDxPageControl.SetPageCount(Value:Integer);
begin

end;

procedure TDxPageControl.SetActivePageIndex(Value:Integer);
var
  Index:Integer;
begin
  // if FActivePageIndex <> Value then begin
  if (Value >= 0) and (Value < FVisiblePageList.Count) then begin
    FActivePageIndex := Value;
    for Index := 0 to ControlCount - 1 do begin
      TDxTabSheet(Control[Index]).Visible := False;
    end;

    Lock;
    try
      ButtonDown(Value);
      FActivePage := TDxTabSheet(FVisiblePageList.Objects[Value]);
      FActivePage.Visible := True;
    finally
      UnLock;
    end;

    if Assigned(FOnActivePageChange) then
      FOnActivePageChange(Self);
  end
  else if ControlCount = 0 then begin
    FActivePageIndex := -1;
    FActivePage := nil;
  end;
  // end;
end;

procedure TDxPageControl.SetActivePage(Value:TDxControl);
var
  Index:Integer;
begin
  if (FActivePage <> Value) then begin
    if (Value <> nil) and (Value.Owner = Self) then begin
      Index := FVisiblePageList.IndexOfObject(Value);
      if Index >= 0 then begin
        ActivePageIndex := Index;
      end else begin
        Lock;
        try
          FActivePage := Value;
          for Index := 0 to ControlCount - 1 do begin
            TDxTabSheet(Control[Index]).Visible := False;
          end;
          FActivePage.Visible := True;
        finally
          UnLock;
        end;
      end;
    end
    else begin
      if ControlCount = 0 then begin
        FActivePage := Value;
      end;
    end;
  end;
end;

procedure TDxPageControl.Put(Index:Integer; Value:TDxControl);
begin
  {if (Index >= 0) and (Index < ControlCount) then
    Control[Index]}
end;

function TDxPageControl.Get(Index:Integer):TDxControl;
begin
  // Lock;
  // try
  // nIndex := PageIndex(Index);
  if (Index >= 0) and (Index < ControlCount) then
    Result := TDxTabSheet(Control[Index])
  else
    Result := nil;
  // finally
    // UnLock;
  // end;
end;

function TDxPageControl.GetTabSheet:TDxTabSheet;
begin
  // Lock;
  // try
  // nIndex := PageIndex(FActivePageIndex);
  if (FActivePageIndex >= 0) and (FActivePageIndex < ControlCount) then
    Result := TDxTabSheet(Control[FActivePageIndex])
  else
    Result := nil;
  // finally
    // UnLock;
  // end;
end;

procedure TDxPageControl.SetTabSheet(Value:TDxTabSheet);
begin

end;

end.
