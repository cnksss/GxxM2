unit ButtonEdit;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Forms, Controls,
  StdCtrls, Menus, Themes, EditEx;

type
  TButtonStyle = (bsFind, bsCustom);     //按钮样式

type
  TButtonState = (bsNormal, bsHot, bsPushed);
  TButtonPosition = (bpLeft, bpRight);

  TButtonPaintEvent = procedure(Sender: TObject; Canvas: TCanvas; R: TRect;
    ButtonState: TButtonState) of object;

  TEditButton = class;
  TGlyph = class(TCustomControl)
  private
    FButton: TEditButton;
    FState: TButtonState;

    FOnButtonPaint: TButtonPaintEvent;
  protected
    procedure Click; override;
    procedure MouseDown(Button: TMouseButton; Shift: TShiftState;
      X, Y: Integer); override;
    procedure MouseMove(Shift: TShiftState; X, Y: Integer); override;
    procedure MouseUp(Button: TMouseButton; Shift: TShiftState;
      X, Y: Integer); override;

    procedure Paint; override;
    procedure WndProc(var Message: TMessage); override;
    procedure CreateWnd; override;
    procedure WMEraseBkgnd(var Message: TWmEraseBkgnd); message WM_ERASEBKGND;
  public
    constructor Create(AButton: TEditButton); reintroduce; virtual;

    property OnButtonPaint: TButtonPaintEvent read FOnButtonPaint write 
      FOnButtonPaint;
  end;

  TCustomButtonEdit = class;

  TEditButton = class(TPersistent)
  private
    FImage: TBitmap;
    FGlyph: TGlyph;
    FOneImgWidth: Integer;
    FPosition: TButtonPosition;
    FEditControl: TCustomButtonEdit;
    FDropDownMenu: TPopupMenu;
    FAutoWidth: Boolean;
    FButtonStyle: TButtonStyle;

    FMarginRect: TRect;
    FTransparent: Boolean;

    FImageCount: Integer;

    function GetWidth: Integer;
    function GetEnabled: Boolean;
    function GetVisible: Boolean;

    function GetMargin(const Index: Integer): Integer;
    
    procedure SetWidth(const Value: Integer);
    procedure SetEnabled(const Value: Boolean);
    procedure SetVisible(const Value: Boolean);

    procedure SetImage(const Value: TBitmap);
    procedure SetAutoWidth(const Value: Boolean);

    procedure PictureChanged(Sender: TObject);
    procedure SetButtonStyle(const Value: TButtonStyle);
    procedure SetMargin(const Index, Value: Integer);
    procedure SetTransparent(const Value: Boolean);
    function GetHint: string;
    function GetParentShowHint: Boolean;
    function GetShowHint: Boolean;
    procedure SetHint(const Value: string);
    procedure SetParentShowHint(const Value: Boolean);
    procedure SetShowHint(const Value: Boolean);
    procedure SetImageCount(const Value: Integer);
  protected
    function GetOwner: TPersistent; override;
    property EditControl: TCustomButtonEdit read FEditControl;
    property Glyph: TGlyph read FGlyph;
    property Position: TButtonPosition read FPosition;
    procedure UpdateBounds; dynamic;
  public
    constructor Create(EditControl: TCustomButtonEdit; APosition: TButtonPosition); reintroduce; virtual;
    destructor Destroy; override;
  published
    property Image: TBitmap read FImage write SetImage;
    property ImageCount: Integer read FImageCount write SetImageCount default 4;
    property Width: Integer read GetWidth write SetWidth default 20;
    property Enabled: Boolean read GetEnabled write SetEnabled default True;
    property Visible: Boolean read GetVisible write SetVisible default False;
    property AutoWidth: Boolean read FAutoWidth write SetAutoWidth default False;
    property DropDownMenu: TPopupMenu read FDropDownMenu write FDropDownMenu;
    property Style: TButtonStyle read FButtonStyle write SetButtonStyle default bsCustom;
    property LeftMargin: Integer index 0 read GetMargin write SetMargin default 0;
    property RightMargin: Integer index 1 read GetMargin write SetMargin default 0;
    property TopMargin: Integer index 2 read GetMargin write SetMargin default 0;
    property BottomMargin: Integer index 3 read GetMargin write SetMargin default 0;
    property Transparent: Boolean read FTransparent write SetTransparent default False;

    property Hint: string read GetHint write SetHint;
    property ShowHint: Boolean read GetShowHint write SetShowHint;
    property ParentShowHint: Boolean read GetParentShowHint write SetParentShowHint;
  end;

  TEditButtonClass = class of TEditButton;

  { TCustomButtonEdit }

  TCustomButtonEdit = class(TCustomEditEx)
  private
    FLeftButton: TEditButton;
    FRightButton: TEditButton;

    function GetOnLeftButtonClick: TNotifyEvent;
    function GetOnRightButtonClick: TNotifyEvent;

    procedure SetLeftButton(const Value: TEditButton);
    procedure SetRightButton(const Value: TEditButton);

    procedure SetOnLeftButtonClick(const Value: TNotifyEvent);
    procedure SetOnRightButtonClick(const Value: TNotifyEvent);
    function GetOnLeftButtonPaint: TButtonPaintEvent;
    function GetOnRightButtonPaint: TButtonPaintEvent;
    procedure SetOnLeftButtonPaint(const Value: TButtonPaintEvent);
    procedure SetOnRightButtonPaint(const Value: TButtonPaintEvent);
    function GetOnLeftButtonMouseDown: TMouseEvent;
    function GetOnLeftButtonMouseMove: TMouseMoveEvent;
    function GetOnLeftButtonMouseUp: TMouseEvent;
    function GetOnRightButtonMouseDown: TMouseEvent;
    function GetOnRightButtonMouseMove: TMouseMoveEvent;
    function GetOnRightButtonMouseUp: TMouseEvent;
    procedure SetOnLeftButtonMouseDown(const Value: TMouseEvent);
    procedure SetOnLeftButtonMouseMove(const Value: TMouseMoveEvent);
    procedure SetOnLeftButtonMouseUp(const Value: TMouseEvent);
    procedure SetOnRightButtonMouseDown(const Value: TMouseEvent);
    procedure SetOnRightButtonMouseMove(const Value: TMouseMoveEvent);
    procedure SetOnRightButtonMouseUp(const Value: TMouseEvent);
  protected
    function GetEditButtonClass: TEditButtonClass; dynamic;
    procedure UpdateEditMargins; override;
    procedure WndProc(var Message: TMessage); override;
    procedure KeyPress(var Key: Char); override;

    property LeftButton: TEditButton read FLeftButton write SetLeftButton;
    property RightButton: TEditButton read FRightButton write SetRightButton;

    property OnLeftButtonClick: TNotifyEvent read GetOnLeftButtonClick
      write SetOnLeftButtonClick;
    property OnRightButtonClick: TNotifyEvent read GetOnRightButtonClick
      write SetOnRightButtonClick;

    property OnLeftButtonMouseDown: TMouseEvent read GetOnLeftButtonMouseDown
      write SetOnLeftButtonMouseDown;
    property OnRightButtonMouseDown: TMouseEvent read GetOnRightButtonMouseDown
      write SetOnRightButtonMouseDown;

    property OnLeftButtonMouseMove: TMouseMoveEvent read GetOnLeftButtonMouseMove
      write SetOnLeftButtonMouseMove;
    property OnRightButtonMouseMove: TMouseMoveEvent read GetOnRightButtonMouseMove
      write SetOnRightButtonMouseMove;

    property OnLeftButtonMouseUp: TMouseEvent read GetOnLeftButtonMouseUp
      write SetOnLeftButtonMouseUp;
    property OnRightButtonMouseUp: TMouseEvent read GetOnRightButtonMouseUp
      write SetOnRightButtonMouseUp;

    property OnLeftButtonPaint: TButtonPaintEvent read GetOnLeftButtonPaint
      write SetOnLeftButtonPaint;
    property OnRightButtonPaint: TButtonPaintEvent read GetOnRightButtonPaint
      write SetOnRightButtonPaint;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;

    procedure RepaintLeftButton;
    procedure RepaintRightButton;
  end;

  TButtonEdit = class(TCustomButtonEdit)
    property LeftButton;
    property RightButton;
    property OnLeftButtonClick;
    property OnRightButtonClick;
    property OnLeftButtonPaint;
    property OnRightButtonPaint;

    property OnLeftButtonMouseDown;
    property OnRightButtonMouseDown;

    property OnLeftButtonMouseMove;
    property OnRightButtonMouseMove;

    property OnLeftButtonMouseUp;
    property OnRightButtonMouseUp;

    property HintText;
    property HintTextFont;
    property OnPaintHintText;
    property Alignment;
    property ValueSetting;
    property PaddingWidth;
    property HintParentFont;

    property Align;
    property Anchors;
    property AutoSelect;
    property AutoSize;
    property BevelEdges;
    property BevelInner;
    property BevelKind default bkNone;
    property BevelOuter;
    property BevelWidth;
    property BiDiMode;
    property BorderStyle;
    property CharCase;
    property Color;
    property Constraints;
    property Ctl3D;
    property DragCursor;
    property DragKind;
    property DragMode;
    property Enabled;
    property Font;
    property HideSelection;
    property ImeMode;
    property ImeName;
    property MaxLength;
    property OEMConvert;
    property ParentBiDiMode;
    property ParentColor;
    property ParentCtl3D;
    property ParentFont;
    property ParentShowHint;
    property PasswordChar;
    property PopupMenu;
    property ReadOnly;
    property ShowHint;
    property TabOrder;
    property TabStop;
    property Text;
    property Visible;
    property OnChange;
    property OnClick;
    property OnContextPopup;
    property OnDblClick;
    property OnDragDrop;
    property OnDragOver;
    property OnEndDock;
    property OnEndDrag;
    property OnEnter;
    property OnExit;
    property OnKeyDown;
    property OnKeyPress;
    property OnKeyUp;
    property OnMouseDown;
{$IFDEF DELPHI2007}
    property OnMouseActivate;
    property OnMouseEnter;
    property OnMouseLeave;
{$ENDIF}
    property OnMouseMove;
    property OnMouseUp;
    property OnStartDock;
    property OnStartDrag;
  end;

implementation

var
  refCount: Integer = 0;
  arrPic: array[TButtonStyle] of TBitmap;
  //arrPicName: array[TButtonStyle] of string = ('RESFIND', '');

{ TEditButton.TGlyph }
constructor TGlyph.Create(AButton: TEditButton);
begin
  inherited Create(AButton.FEditControl);
  FButton := AButton;
  FState := bsNormal;
  Parent := FButton.FEditControl;
  Visible := False;
  Canvas.Brush.Color := FButton.EditControl.Color;
end;

procedure TGlyph.CreateWnd;
begin
  inherited;
  if Visible then
    FButton.FEditControl.UpdateEditMargins;
end;

procedure TGlyph.MouseDown(Button: TMouseButton; Shift: TShiftState; X,
  Y: Integer);
begin
  //inherited;
  if Assigned(OnMouseDown) and (Action <> nil) then
    OnMouseDown(FButton.EditControl, Button, Shift, X, Y)
  else if not (csDesigning in ComponentState) and (ActionLink <> nil) then
    ActionLink.Execute(FButton.EditControl)
  else if Assigned(OnMouseDown) then
    OnMouseDown(FButton.EditControl, Button, Shift, X, Y);
end;

procedure TGlyph.MouseMove(Shift: TShiftState; X, Y: Integer);
begin
  //inherited;
  if Assigned(OnMouseMove) and (Action <> nil) then
    OnMouseMove(FButton.EditControl, Shift, X, Y)
  else if not (csDesigning in ComponentState) and (ActionLink <> nil) then
    ActionLink.Execute(FButton.EditControl)
  else if Assigned(OnMouseMove) then
    OnMouseMove(FButton.EditControl, Shift, X, Y);
end;

procedure TGlyph.MouseUp(Button: TMouseButton; Shift: TShiftState; X,
  Y: Integer);
begin
  //inherited;
  if Assigned(OnMouseUp) and (Action <> nil) then
    OnMouseUp(FButton.EditControl, Button, Shift, X, Y)
  else if not (csDesigning in ComponentState) and (ActionLink <> nil) then
    ActionLink.Execute(FButton.EditControl)
  else if Assigned(OnMouseUp) then
    OnMouseUp(FButton.EditControl, Button, Shift, X, Y);
end;

procedure TGlyph.Click;
begin
  if Assigned(OnClick) and (Action <> nil) then
    OnClick(FButton.EditControl)
  else if not (csDesigning in ComponentState) and (ActionLink <> nil) then
    ActionLink.Execute(FButton.EditControl)
  else if Assigned(OnClick) then
    OnClick(FButton.EditControl);
end;

procedure TGlyph.Paint;
var
  Index: Integer;
  R, DestR: TRect;
begin
  inherited;
  if Visible then
  begin
    if (FButton.FImage <> nil) then
    begin
      if not Enabled then
        Index := 3
      else
        Index := Integer(FState);

      if Index > FButton.FImageCount - 1 then
        Index := FButton.FImageCount - 1;

      R.Left := Index * FButton.FOneImgWidth;
      R.Right := (Index + 1) * FButton.FOneImgWidth;
      R.Top := 0;
      R.Bottom := FButton.FImage.Height;
      Canvas.Pen.Style := psClear;
      //Canvas.Brush.Color := clRed;
      Canvas.Rectangle(ClientRect);

      DestR.Left := (ClientWidth - (R.Right - R.Left)) div 2;
      DestR.Right := DestR.Left + R.Right - R.Left;
      DestR.Top := (ClientHeight - (R.Bottom - R.Top)) div 2;
      DestR.Bottom := DestR.Top + R.Bottom - R.Top;

      Canvas.CopyRect(DestR, FButton.Image.Canvas, R);
    end;

    if Assigned(FOnButtonPaint) then
      FOnButtonPaint(FButton.EditControl, Canvas, Rect(0, 0, Width, Height), FState);
  end;
end;

procedure TGlyph.WMEraseBkgnd(var Message: TWmEraseBkgnd);
begin
  Message.Result := 1;
end;

procedure TGlyph.WndProc(var Message: TMessage);
var
  LPoint: TPoint;
begin
  inherited;
  if (Message.Msg = WM_CONTEXTMENU) and (FButton.EditControl.PopupMenu = nil) then
    Exit;
  case Message.Msg of
    CM_MOUSEENTER: FState := bsHot;
    CM_MOUSELEAVE: FState := bsNormal;
    WM_LBUTTONDOWN:
      if FButton.FDropDownMenu <> nil then
      begin
        if not (csDesigning in Parent.ComponentState) then
        begin
          LPoint := ClientToScreen(Point(0, FButton.EditControl.Height));
          FButton.FDropDownMenu.Popup(LPoint.X, LPoint.Y);
        end;
      end
      else
        FState := bsPushed;
    WM_LBUTTONUP:
      begin
        GetCursorPos(LPoint);
        if (FindDragTarget(LPoint, True) = Self) then
          if not(csDesigning in ComponentState) and Enabled then
            FState := bsHot
        else
          FState := bsNormal;
      end;
    CM_VISIBLECHANGED: FButton.UpdateBounds;
  else
    Exit;
  end;
  Invalidate;
end;

{ TEditButton }

constructor TEditButton.Create(EditControl: TCustomButtonEdit; APosition: TButtonPosition);
var
  btnStyle: TButtonStyle;
begin
  inherited Create;
  FImageCount := 4;
  FOneImgWidth := 0;
  FPosition := APosition;
  FAutoWidth := False;
  FButtonStyle := bsCustom;
  FTransparent := False;
  FEditControl := EditControl;
  FImage := TBitmap.Create;
  FImage.OnChange := PictureChanged;
  FGlyph := TGlyph.Create(Self);
  FGlyph.Width := 20;
  FMarginRect := Rect(0, 0, 0, 0);

  {----------------------------}
  if refCount = 0 then
  begin
    for btnStyle := Low(TButtonStyle) to High(TButtonStyle) do
    begin
      arrPic[btnStyle] := TBitmap.Create;
      //if Length(arrPicName[btnStyle]) > 0 then
      //  arrPic[btnStyle].LoadFromResourceName(HInstance, arrPicName[btnStyle]);
    end;
  end;
  Inc(refCount);
end;

destructor TEditButton.Destroy;
var
  btnStyle: TButtonStyle;
begin
  FGlyph.Parent.RemoveControl(FGlyph);
  FGlyph.Free;
  FImage.Free;

  {----------------------------}
  Dec(refCount);
  if refCount = 0 then
  begin
    for btnStyle := Low(TButtonStyle) to High(TButtonStyle) do
      arrPic[btnStyle].Free;
  end;

  inherited;
end;

procedure TEditButton.SetMargin(const Index, Value: Integer);
begin
  case Index of
    0: FMarginRect.Left := Value;
    1: FMarginRect.Right := Value;
    2: FMarginRect.Top := Value;
    3: FMarginRect.Bottom := Value;
  end;
  UpdateBounds;
  FEditControl.UpdateEditMargins;
end;

procedure TEditButton.SetParentShowHint(const Value: Boolean);
begin
  FGlyph.ParentShowHint := Value;
end;

procedure TEditButton.SetShowHint(const Value: Boolean);
begin
  FGlyph.ShowHint := Value;
end;

procedure TEditButton.SetTransparent(const Value: Boolean);
begin
  if FTransparent <> Value then
  begin
    FTransparent := Value;
    if Assigned(FImage) then
    begin
      FImage.TransparentColor := FImage.Canvas.Pixels[0, 0];
      FImage.Transparent := FTransparent;
    end;
  end;
end;

function TEditButton.GetMargin(const Index: Integer): Integer;
begin
  case Index of
    0: Result := FMarginRect.Left;
    1: Result := FMarginRect.Right;
    2: Result := FMarginRect.Top;
    3: Result := FMarginRect.Bottom;
  else
    Result := 0;
  end;
end;

procedure TEditButton.UpdateBounds;
begin
  if Assigned(FGlyph) then
  begin
    FGlyph.Top := TopMargin;
    FGlyph.Height := EditControl.Height - TopMargin - BottomMargin;

    if FPosition = bpLeft then
      FGlyph.Left := LeftMargin
    else
      FGlyph.Left := FEditControl.Width - Width - RightMargin;
  end;
end;

function TEditButton.GetEnabled: Boolean;
begin
  Result := FGlyph.Enabled;
end;

function TEditButton.GetHint: string;
begin
  Result := FGlyph.Hint;
end;

function TEditButton.GetOwner: TPersistent;
begin
  Result := FEditControl;
end;

function TEditButton.GetParentShowHint: Boolean;
begin
  Result := FGlyph.ParentShowHint;
end;

function TEditButton.GetShowHint: Boolean;
begin
  Result := FGlyph.ShowHint;
end;

function TEditButton.GetVisible: Boolean;
begin
  Result := FGlyph.Visible;
end;

function TEditButton.GetWidth: Integer;
begin
  Result := FGlyph.Width;
end;

procedure TEditButton.PictureChanged(Sender: TObject);
begin
  if Assigned(FImage) then
  begin
    FOneImgWidth := FImage.Width div FImageCount;
    FImage.TransparentColor := FImage.Canvas.Pixels[0, 0];
    FImage.Transparent := FTransparent;
  end
  else
    FOneImgWidth := 0;

  if AutoWidth then
  begin
    Glyph.Width := FOneImgWidth;
    UpdateBounds;
    FEditControl.UpdateEditMargins;
  end;
end;

procedure TEditButton.SetAutoWidth(const Value: Boolean);
begin
  if FAutoWidth <> Value then
  begin
    FAutoWidth := Value;
    if FAutoWidth then
    begin
      Glyph.Width := FOneImgWidth;
      FEditControl.UpdateEditMargins;
    end;
  end;
end;

procedure TEditButton.SetButtonStyle(const Value: TButtonStyle);
begin
  if Value <> FButtonStyle then
  begin
    FButtonStyle := Value;
    if Assigned(arrPic[FButtonStyle]) then
      FImage.Assign(arrPic[FButtonStyle])
    else
      FImage.CleanupInstance;
    FGlyph.Repaint;
  end;
end;

procedure TEditButton.SetEnabled(const Value: Boolean);
begin
  if Value <> FGlyph.Enabled then
  begin
    FGlyph.Enabled := Value;
    FGlyph.Invalidate;
  end;
end;

procedure TEditButton.SetHint(const Value: string);
begin
  FGlyph.Hint := Value;
end;

procedure TEditButton.SetImage(const Value: TBitmap);
begin
  FImage.Assign(Value);
end;

procedure TEditButton.SetImageCount(const Value: Integer);
begin
  FImageCount := Value;
  PictureChanged(Self);
end;

procedure TEditButton.SetVisible(const Value: Boolean);
begin
  if Value <> FGlyph.Visible then
  begin
    FGlyph.Visible := Value;
    FEditControl.UpdateEditMargins;
  end;
end;

procedure TEditButton.SetWidth(const Value: Integer);
begin
  if (Glyph.Width <> Value) and (not FAutoWidth) then
  begin
    Glyph.Width := Value;
    UpdateBounds;
    FEditControl.UpdateEditMargins;
  end;
end;

{ TCustomButtonEdit }

constructor TCustomButtonEdit.Create(AOwner: TComponent);
begin
  inherited;
  FLeftButton := GetEditButtonClass.Create(Self, bpLeft);
  FRightButton := GetEditButtonClass.Create(Self, bpRight);
end;


destructor TCustomButtonEdit.Destroy;
begin
  FreeAndNil(FLeftButton);
  FreeAndNil(FRightButton);
  inherited;
end;

function TCustomButtonEdit.GetEditButtonClass: TEditButtonClass;
begin
  Result := TEditButton;
end;

function TCustomButtonEdit.GetOnLeftButtonClick: TNotifyEvent;
begin
  Result := LeftButton.Glyph.OnClick;
end;

function TCustomButtonEdit.GetOnLeftButtonMouseDown: TMouseEvent;
begin
  Result := LeftButton.Glyph.OnMouseDown;
end;

function TCustomButtonEdit.GetOnLeftButtonMouseMove: TMouseMoveEvent;
begin
  Result := LeftButton.Glyph.OnMouseMove;
end;

function TCustomButtonEdit.GetOnLeftButtonMouseUp: TMouseEvent;
begin
  Result := LeftButton.Glyph.OnMouseUp;
end;

function TCustomButtonEdit.GetOnLeftButtonPaint: TButtonPaintEvent;
begin
  Result := LeftButton.Glyph.OnButtonPaint;
end;

function TCustomButtonEdit.GetOnRightButtonClick: TNotifyEvent;
begin
  Result := RightButton.Glyph.OnClick;
end;

function TCustomButtonEdit.GetOnRightButtonMouseDown: TMouseEvent;
begin
  Result := RightButton.Glyph.OnMouseDown;
end;

function TCustomButtonEdit.GetOnRightButtonMouseMove: TMouseMoveEvent;
begin
  Result := RightButton.Glyph.OnMouseMove;
end;

function TCustomButtonEdit.GetOnRightButtonMouseUp: TMouseEvent;
begin
  Result := RightButton.Glyph.OnMouseUp;
end;

function TCustomButtonEdit.GetOnRightButtonPaint: TButtonPaintEvent;
begin
  Result := RightButton.Glyph.OnButtonPaint;
end;

procedure TCustomButtonEdit.SetLeftButton(const Value: TEditButton);
begin
  FLeftButton.Assign(Value);
end;

procedure TCustomButtonEdit.SetOnLeftButtonClick(const Value: TNotifyEvent);
begin
  LeftButton.Glyph.OnClick := Value;
end;

procedure TCustomButtonEdit.SetOnLeftButtonMouseDown(const Value: TMouseEvent);
begin
  LeftButton.Glyph.OnMouseDown := Value;
end;

procedure TCustomButtonEdit.SetOnLeftButtonMouseMove(
  const Value: TMouseMoveEvent);
begin
  LeftButton.Glyph.OnMouseMove := Value;
end;

procedure TCustomButtonEdit.SetOnLeftButtonMouseUp(const Value: TMouseEvent);
begin
  LeftButton.Glyph.OnMouseUp := Value;
end;

procedure TCustomButtonEdit.SetOnLeftButtonPaint(
  const Value: TButtonPaintEvent);
begin
  LeftButton.Glyph.OnButtonPaint := Value;
end;

procedure TCustomButtonEdit.SetOnRightButtonClick(const Value: TNotifyEvent);
begin
  RightButton.Glyph.OnClick := Value;
end;

procedure TCustomButtonEdit.SetOnRightButtonMouseDown(const Value: TMouseEvent);
begin
  RightButton.Glyph.OnMouseDown := Value;
end;

procedure TCustomButtonEdit.SetOnRightButtonMouseMove(
  const Value: TMouseMoveEvent);
begin
  RightButton.Glyph.OnMouseMove := Value;
end;

procedure TCustomButtonEdit.SetOnRightButtonMouseUp(const Value: TMouseEvent);
begin
  RightButton.Glyph.OnMouseUp := Value;
end;

procedure TCustomButtonEdit.SetOnRightButtonPaint(
  const Value: TButtonPaintEvent);
begin
  RightButton.Glyph.OnButtonPaint := Value;
end;

procedure TCustomButtonEdit.SetRightButton(const Value: TEditButton);
begin
  FRightButton.Assign(Value);
end;

procedure TCustomButtonEdit.KeyPress(var Key: Char);
begin
  inherited;
  if (Key = Char(VK_RETURN)) then Key := #0;
end;

procedure TCustomButtonEdit.RepaintLeftButton;
begin
  FLeftButton.FGlyph.Repaint;
end;

procedure TCustomButtonEdit.RepaintRightButton;
begin
  FRightButton.FGlyph.Repaint;
end;

{
//对于单行，本函数有效
procedure TCustomButtonEdit.UpdateEditMargins;
var
  LMargin, RMargin: Integer;
begin
  if HandleAllocated then
  begin
    LMargin := 0;
    RMargin := 0;

    if LeftButton.Visible then
      LMargin := LeftButton.Width + 2;
    if RightButton.Visible then
      RMargin := RightButton.Width + 2;

    SendMessage(Handle, EM_SETMARGINS, EC_LEFTMARGIN or EC_RIGHTMARGIN, MakeLong(LMargin, RMargin));
    Invalidate;
  end;
end;
}

procedure TCustomButtonEdit.UpdateEditMargins;
var
  R: TRect;
  H: Integer;
begin
  //if HandleAllocated then
  begin
    R := ClientRect;
    Inc(R.Left);
    Dec(R.Right);

    if LeftButton.Visible then
    begin
      R.Left := R.Left + BorderWidth + LeftButton.LeftMargin + LeftButton.Width + LeftButton.RightMargin;
      FHintTextLeftMargin := BorderWidth + LeftButton.LeftMargin + LeftButton.Width + 3;
    end;

    if RightButton.Visible then
      R.Right := R.Right - BorderWidth - RightButton.RightMargin - RightButton.Width - RightButton.LeftMargin;

    FCanvas.Font.Assign(Font);
    H := FCanvas.TextHeight('|');
    H := ((R.Bottom - R.Top) - H) div 2;

    InflateRect(R, 0, -H);

    SendMessage(Handle, EM_SETRECT, 0, Longint(@R));
    Invalidate;
  end;
end;

procedure TCustomButtonEdit.WndProc(var Message: TMessage);
begin
  inherited;
  case Message.Msg of
    CM_BORDERCHANGED,
    CM_CTL3DCHANGED,
    WM_SIZE:
      begin
        LeftButton.UpdateBounds;
        RightButton.UpdateBounds;
        UpdateEditMargins;
      end;
    CM_FONTCHANGED:
      if not (csLoading in ComponentState) then
        UpdateEditMargins;
    CN_CTLCOLOREDIT:
      begin
        //FLeftButton.Glyph.Invalidate;
        //FRightButton.Glyph.Invalidate;
      end;
  end;
end;

end.
