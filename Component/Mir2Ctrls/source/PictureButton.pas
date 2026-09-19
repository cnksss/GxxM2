{

  最后修改: 2011-4-19

      1、增加 Click 方法
      2、绘制时支持 &hotkey
      3、增加 DropdownMenu
}

unit PictureButton;

interface

uses
  Windows, Classes, SysUtils, Controls, Messages, Forms, Graphics,
  ActiveX, Menus;

type
  TImgCtrlStatus = (icsNormal = 0, icsEnter = 1, icsDown = 2, icsCheck = 3);

  TPictures = class(TPersistent)
  private
    FUp: TPicture;
    FHot: TPicture;
    FDown: TPicture;
    FDisabled: TPicture;

    FOnChange: TNotifyEvent;
    procedure SetDisabled(const Value: TPicture);
    procedure SetDown(const Value: TPicture);
    procedure SetHot(const Value: TPicture);
    procedure SetUp(const Value: TPicture);

    procedure PictureChanged(Sender: TObject);
  public
    constructor Create;
    Destructor Destroy; override;
  published
    property Up: TPicture read FUp write SetUp;
    property Hot: TPicture read FHot write SetHot;
    property Down: TPicture read FDown write SetDown;
    property Disabled: TPicture read FDisabled write SetDisabled;
    property OnChange: TNotifyEvent read FOnChange write FOnChange;
  end;

  TImgStatusChangedEvent = procedure(Sender: TObject; Status: TImgCtrlStatus) of object;
  TPictureCtrl = class(TGraphicControl)
  private
    FDropdownMenu: TPopupMenu;

    FAlignment: TAlignment;

    FPictures: TPictures;

    FAutoRepaintParent: Boolean;

    FIsMouseDown: Boolean;          //是否鼠标按下状态

    FOnStatusChanged: TImgStatusChangedEvent;

    { 鼠标移入消息 }
    procedure CMMouseEnter(var Msg: Tmessage); message CM_MOUSEENTER;

    { 鼠标移出消息}
    procedure CMMouseLeave(var Msg:Tmessage); message CM_MOUSELEAVE;

    { 字体更改消息 }
    procedure CMFontChanged(var Message: TMessage); message CM_FONTCHANGED;

    { 标题改变 }
    procedure CMTextChanged(var Message: TMessage); message CM_TEXTCHANGED;

     { 禁止/非禁止状态改变 }
    procedure CMEnabledChanged(var Message: TMessage); message CM_ENABLEDCHANGED;

    { 热键 }
    procedure CMDialogChar(var Message: TCMDialogChar); message CM_DIALOGCHAR;

    procedure WMSize(var Msg: TWMSize); message WM_SIZE;

    procedure OnPicturesChange(Sender: TObject);
    procedure SetDropdownMenu(const Value: TPopupMenu);
  protected
    procedure SetAutoRepaintParent(const Value: Boolean);
    procedure SetAlignment(const Value: TAlignment);

    function CheckDropdownMenu: Boolean;
  protected
    FStatus: TImgCtrlStatus;
    FCheckOffset: TPoint;
    procedure DoStatusChanaged; virtual;

    procedure Loaded; override;

    procedure Paint; override;
    procedure MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y: Integer); override;
    procedure MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); override;
    procedure MouseMove(Shift: TShiftState; X, Y: Integer); override;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;

    procedure Click; override;
    procedure Invalidate; override;

    property Pictures: TPictures read FPictures write FPictures;

    property AutoRepaintParent: Boolean read FAutoRepaintParent write
      SetAutoRepaintParent default False;

    property DropdownMenu: TPopupMenu read FDropdownMenu write SetDropdownMenu;

    property OnStatusChanged: TImgStatusChangedEvent read FOnStatusChanged write FOnStatusChanged;
  end;

  TPictureButton = class(TPictureCtrl)
  published
    property Align;
    property Anchors;
    property AutoSize;
    property Enabled;
    property Visible;
    property Font;
    property ParentFont;
    property OnClick;
    property OnDblClick;
    property Hint;
    property ParentShowHint;
    property ShowHint;
    property Cursor;

    property Pictures;

    property Caption;
    property AutoRepaintParent;

    property DropdownMenu;

    property OnStatusChanged;
  end;

implementation

{ TPictures }

constructor TPictures.Create;
begin
  FUp := TPicture.Create;
  FUp.OnChange := PictureChanged;

  FHot := TPicture.Create;
  FHot.OnChange := PictureChanged;

  FDown := TPicture.Create;
  FDown.OnChange := PictureChanged;

  FDisabled := TPicture.Create;
  FDisabled.OnChange := PictureChanged;
end;

destructor TPictures.Destroy;
begin
  FUp.Free;
  FHot.Free;
  FDown.Free;
  FDisabled.Free;
  inherited;
end;

procedure TPictures.PictureChanged(Sender: TObject);
begin
  if Assigned(FOnChange) then
    FOnChange(Self);
end;

procedure TPictures.SetUp(const Value: TPicture);
begin
  FUp.Assign(Value);
end;

procedure TPictures.SetHot(const Value: TPicture);
begin
  FHot.Assign(Value);
end;

procedure TPictures.SetDown(const Value: TPicture);
begin
  FDown.Assign(Value);
end;

procedure TPictures.SetDisabled(const Value: TPicture);
begin
  FDisabled.Assign(Value);
end;

{ TImgCtrl }

constructor TPictureCtrl.Create(AOwner: TComponent);
begin
  inherited;
  FPictures := TPictures.Create;
  FPictures.OnChange := OnPicturesChange;
end;

destructor TPictureCtrl.Destroy;
begin
  FPictures.Free;
  inherited;
end;

procedure TPictureCtrl.CMEnabledChanged(var Message: TMessage);
begin
  inherited;
  FStatus := icsNormal;
  DoStatusChanaged;
  FIsMouseDown := False;
  Invalidate;
end;

procedure TPictureCtrl.CMFontChanged(var Message: TMessage);
begin
  inherited;
  Canvas.Font.Assign(Font);
  Invalidate;
end;

procedure TPictureCtrl.CMMouseEnter(var Msg: Tmessage);
begin
  inherited;
  if (csDesigning in ComponentState) or (not Enabled) or FIsMouseDown or
     (FStatus = icsCheck) then Exit;

  FStatus := icsEnter;
  DoStatusChanaged;
  Invalidate;
end;

procedure TPictureCtrl.CMMouseLeave(var Msg: Tmessage);
begin
  inherited;
  if (csDesigning in ComponentState) or (not Enabled) or FIsMouseDown or
     (FStatus = icsCheck) then
    Exit;

  FStatus := icsNormal;
  DoStatusChanaged;
  Invalidate;
end;

procedure TPictureCtrl.CMTextChanged(var Message: TMessage);
begin
  inherited;
  Invalidate;
end;

procedure TPictureCtrl.Invalidate;
var
  R: TRect;
begin
  if FAutoRepaintParent and (not (csDesigning in ComponentState)) then
  begin
    R.Left := Left;
    R.Top := Top;
    R.Right := R.Left + Width;
    R.Bottom := R.Top + Height;
    InvalidateRect(Parent.Handle, @R, True);
  end;

  inherited;
end;

procedure TPictureCtrl.MouseMove(Shift: TShiftState; X, Y: Integer);
begin
  inherited;

end;

procedure TPictureCtrl.MouseDown(Button: TMouseButton; Shift: TShiftState; X,
  Y: Integer);
begin
  inherited;
  if (csDesigning in ComponentState) or (not Enabled) or (Button <> mbLeft) or
     (FStatus = icsCheck) then Exit;
  FStatus := icsDown;
  DoStatusChanaged;
  FIsMouseDown := True;
  Invalidate;
end;

procedure TPictureCtrl.MouseUp(Button: TMouseButton; Shift: TShiftState; X,
  Y: Integer);
var
  Pt: TPoint;
begin
  inherited;
  if (csDesigning in ComponentState)
    or (not Enabled) or
    (Button <> mbLeft) then Exit;     // or (FStatus = icsCheck)

  CheckDropdownMenu;

  if (FStatus <> icsCheck) then
  begin
    GetCursorPos(Pt);
    
    if (FindDragTarget(Pt, True) = Self) then
      FStatus := icsEnter
    else
      FStatus := icsNormal;
    DoStatusChanaged;
  end;

  FIsMouseDown := False;
  Invalidate;
end;

procedure TPictureCtrl.Paint;
var
  Picture: TPicture;
begin
  inherited;

  Canvas.Pen.Style := psDash;
  Canvas.Brush.Style := bsClear;
  Canvas.Rectangle(0, 0, Width, Height);

  Picture := nil;
  if not Enabled then
    Picture := Pictures.Disabled
  else
  begin
    case FStatus of
      icsNormal: Picture := Pictures.Up;
      icsEnter: Picture := Pictures.Hot;
      icsDown: Picture := Pictures.Down;
    end;
  end;

  if (Picture <> nil) and (Picture.Graphic <> nil) then
  begin
    Canvas.Draw(0, 0, Picture.Graphic);
  end;
end;

procedure TPictureCtrl.SetAlignment(const Value: TAlignment);
begin
  if Value <> FAlignment then
  begin
    FAlignment := Value;
    Invalidate;
  end;
end;

procedure TPictureCtrl.SetAutoRepaintParent(const Value: Boolean);
begin
  if FAutoRepaintParent <> Value then
  begin
    FAutoRepaintParent := Value;
    Invalidate;
  end;
end;

procedure TPictureCtrl.OnPicturesChange(Sender: TObject);
var
  Msg: TWMSize;
begin
  WMSize(Msg);
  Invalidate;
end;


procedure TPictureCtrl.Click;
begin
  inherited;

end;

procedure TPictureCtrl.CMDialogChar(var Message: TCMDialogChar);
begin
  with Message do
  begin
    if IsAccel(CharCode, Caption) and Enabled and Visible and
      (Parent <> nil) and Parent.Showing then
    begin
      Click;
      CheckDropdownMenu;
      Result := 1;
    end
    else
      inherited;
  end;
end;

procedure TPictureCtrl.SetDropdownMenu(const Value: TPopupMenu);
begin
  if Value <> FDropdownMenu then
  begin
    FDropdownMenu := Value;
    if Value <> nil then Value.FreeNotification(Self);
  end;
end;

function TPictureCtrl.CheckDropdownMenu: Boolean;
var
  APoint: TPoint;
begin
  Result := False;
  if FDropdownMenu <> nil then
  begin
    APoint := Self.ClientToScreen(Point(0, Self.ClientHeight));
    if FDropdownMenu.IsRightToLeft then Inc(APoint.X, Self.Width);

    if (GetSystemMetrics(SM_CYMENU) * FDropdownMenu.Items.Count) + APoint.Y >
       Screen.MonitorFromPoint(APoint).Height then
      Dec(APoint.Y, Self.Height);

    FDropdownMenu.Popup(APoint.X, APoint.Y);

    Result := True;
  end;
end;

procedure TPictureCtrl.DoStatusChanaged;
begin
  if Assigned(FOnStatusChanged) then
    FOnStatusChanged(Self, FStatus);
end;

procedure TPictureCtrl.WMSize(var Msg: TWMSize);
begin
  if csLoading in ComponentState then Exit;

  if FPictures.FUp.Graphic <> nil then
  begin
    Width := FPictures.FUp.Graphic.Width;
    Height := FPictures.FUp.Graphic.Height;
  end
  else if FPictures.FHot.Graphic <> nil then
  begin
    Width := FPictures.FHot.Graphic.Width;
    Height := FPictures.FHot.Graphic.Height;
  end
  else if FPictures.FDown.Graphic <> nil then
  begin
    Width := FPictures.FDown.Graphic.Width;
    Height := FPictures.FDown.Graphic.Height;
  end
  else if FPictures.FDisabled.Graphic <> nil then
  begin
    Width := FPictures.FDisabled.Graphic.Width;
    Height := FPictures.FDisabled.Graphic.Height;
  end;
  Invalidate;
end;

procedure TPictureCtrl.Loaded;
var
  Msg: TWMSize;
begin
  inherited Loaded;
  WMSize(Msg);
  Invalidate;
end;

end.
