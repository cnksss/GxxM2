{*******************************************************}
{                                                       }
{       创    建： PiaoYun                              }
{       版    本： 1.0                                  }
{       创建日期： 2013-07-20                           }
{                                                       }
{       单元说明： 扩展空值校验                         }
{                                                       }
{       版权所有 (C) 2013 GeeM2                         }
{                                                       }
{*******************************************************}

unit SpinEditEx;

interface

{$I HGVER.INC}

uses
  Windows, Classes, SysUtils, Controls, Messages, Spin, StdCtrls, Math;

type
  TSpinEditEx = class(TSpinEdit)
  private
    FDisableKeyChangeValue: Boolean;

    function CheckValue (NewValue: Integer): Integer;
    function GetValue: Integer;
    procedure SetValue(const Value: Integer);

    procedure CMExit(var Message: TCMExit); message CM_EXIT;
  protected
    procedure KeyDown(var Key: Word; Shift: TShiftState); override;
  public
    constructor Create(AOwner: TComponent); override;
  published
    property Value: Integer read GetValue write SetValue;
    property DisableKeyChangeValue: Boolean read FDisableKeyChangeValue write FDisableKeyChangeValue default False;
  end;

  TSpinEditLongWord = class(TCustomEdit)
  private
    FMinValue: LongWord;
    FMaxValue: LongWord;
    FIncrement: LongWord;
    FButton: TSpinButton;
    FEditorEnabled: Boolean;
    FDisableKeyChangeValue: Boolean;

    function GetMinHeight: Integer;
    function GetValue: LongWord;
    function CheckValue (NewValue: LongWord): LongWord;
    procedure SetValue (NewValue: LongWord);
    procedure SetEditRect;
    procedure WMSize(var Message: TWMSize); message WM_SIZE;
    procedure CMEnter(var Message: TCMGotFocus); message CM_ENTER;
    procedure CMExit(var Message: TCMExit);   message CM_EXIT;
    procedure WMPaste(var Message: TWMPaste);   message WM_PASTE;
    procedure WMCut(var Message: TWMCut);   message WM_CUT;
  protected
    procedure GetChildren(Proc: TGetChildProc; Root: TComponent); override;
    function IsValidChar(Key: Char): Boolean; virtual;
    procedure UpClick (Sender: TObject); virtual;
    procedure DownClick (Sender: TObject); virtual;
    procedure KeyDown(var Key: Word; Shift: TShiftState); override;
    procedure KeyPress(var Key: Char); override;
    procedure CreateParams(var Params: TCreateParams); override;
    procedure CreateWnd; override;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    property Button: TSpinButton read FButton;
  published
    property Anchors;
    property AutoSelect;
    property AutoSize;
    property Color;
    property Constraints;
    property Ctl3D;
    property DragCursor;
    property DragMode;
    property EditorEnabled: Boolean read FEditorEnabled write FEditorEnabled default True;
    property Enabled;
    property Font;
    property Increment: LongWord read FIncrement write FIncrement default 1;
    property MaxLength;
    property MaxValue: LongWord read FMaxValue write FMaxValue;
    property MinValue: LongWord read FMinValue write FMinValue;
    property DisableKeyChangeValue: Boolean read FDisableKeyChangeValue write FDisableKeyChangeValue default False;
    property ParentColor;
    property ParentCtl3D;
    property ParentFont;
    property ParentShowHint;
    property PopupMenu;
    property ReadOnly;
    property ShowHint;
    property TabOrder;
    property TabStop;
    property Value: LongWord read GetValue write SetValue;
    property Visible;
    property OnChange;
    property OnClick;
    property OnDblClick;
    property OnDragDrop;
    property OnDragOver;
    property OnEndDrag;
    property OnEnter;
    property OnExit;
    property OnKeyDown;
    property OnKeyPress;
    property OnKeyUp;
    property OnMouseDown;
    property OnMouseMove;
    property OnMouseUp;
    property OnStartDrag;

{$IFDEF XE}
    property StyleElements;
{$ENDIF}
  end;

implementation

{ TSpinEdit }

function TSpinEditEx.CheckValue(NewValue: Integer): Integer;
begin
  Result := NewValue;
  if (MaxValue <> MinValue) then
  begin
    if NewValue < MinValue then
      Result := MinValue
    else if NewValue > MaxValue then
      Result := MaxValue;
  end;
end;

procedure TSpinEditEx.CMExit(var Message: TCMExit);
begin
  if Length(Text) = 0 then
    Value := MinValue;
  inherited;
end;

constructor TSpinEditEx.Create(AOwner: TComponent);
begin
  inherited;
  FDisableKeyChangeValue := False;
end;

function TSpinEditEx.GetValue: Integer;
begin
  Result := StrToIntDef(Text, 0);
end;

procedure TSpinEditEx.KeyDown(var Key: Word; Shift: TShiftState);
{
type
  TKeyDownProc = procedure(var Key: Word; Shift: TShiftState) of Object;
var
  KeyDownProc: TKeyDownProc;
}
begin
  if ((ssAlt in Shift) or (not FDisableKeyChangeValue)) and (Key = VK_UP) then
    UpClick(Self)
  else if ((ssAlt in Shift) or (not FDisableKeyChangeValue)) and (Key = VK_DOWN) then
    DownClick(Self);

  {
  TMethod(KeyDownProc).Code := @TWinControl.KeyDown;
  TMethod(KeyDownProc).Data := Self;
  KeyDownProc(Key, Shift)
  }

  if Assigned(OnKeyDown) then OnKeyDown(Self, Key, Shift);
end;

procedure TSpinEditEx.SetValue(const Value: Integer);
begin
  Text := IntToStr(CheckValue(Value));
end;


{ TSpinEdit }

constructor TSpinEditLongWord.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
  FButton := TSpinButton.Create(Self);
  FButton.Width := 15;
  FButton.Height := 17;
  FButton.Visible := True;  
  FButton.Parent := Self;
  FButton.FocusControl := Self;
  FButton.OnUpClick := UpClick;
  FButton.OnDownClick := DownClick;
  Text := '0';
  ControlStyle := ControlStyle - [csSetCaption];
  FIncrement := 1;
  FEditorEnabled := True;
  ParentBackground := False;
  FDisableKeyChangeValue := False;
end;

destructor TSpinEditLongWord.Destroy;
begin
  FButton := nil;
  inherited Destroy;
end;

procedure TSpinEditLongWord.GetChildren(Proc: TGetChildProc; Root: TComponent);
begin
end;

procedure TSpinEditLongWord.KeyDown(var Key: Word; Shift: TShiftState);
begin
  if ((ssAlt in Shift) or (not FDisableKeyChangeValue)) and (Key = VK_UP) then
    UpClick(Self)
  else if ((ssAlt in Shift) or (not FDisableKeyChangeValue)) and (Key = VK_DOWN) then
    DownClick(Self);

  inherited KeyDown(Key, Shift);
end;

procedure TSpinEditLongWord.KeyPress(var Key: Char);
begin
  if not IsValidChar(Key) then
  begin
    Key := #0;
    MessageBeep(0)
  end;
  if Key <> #0 then inherited KeyPress(Key);
end;

function TSpinEditLongWord.IsValidChar(Key: Char): Boolean;
begin
{$IF CompilerVersion >= 20}
  Result := (Key in [FormatSettings.DecimalSeparator, '+', '-', '0'..'9']) or
    ((Key < #32) and (Key <> Chr(VK_RETURN)));
{$ELSE}
  Result := (Key in [DecimalSeparator, '+', '-', '0'..'9']) or
    ((Key < #32) and (Key <> Chr(VK_RETURN)));
{$IFEND}

  if not FEditorEnabled and Result and ((Key >= #32) or
      (Key = Char(VK_BACK)) or (Key = Char(VK_DELETE))) then
    Result := False;
end;

procedure TSpinEditLongWord.CreateParams(var Params: TCreateParams);
begin
  inherited CreateParams(Params);
{  Params.Style := Params.Style and not WS_BORDER;  }
  Params.Style := Params.Style or ES_MULTILINE or WS_CLIPCHILDREN;
end;

procedure TSpinEditLongWord.CreateWnd;
begin
  inherited CreateWnd;
  SetEditRect;
end;

procedure TSpinEditLongWord.SetEditRect;
var
  Loc: TRect;
begin
  SendMessage(Handle, EM_GETRECT, 0, LongWord(@Loc));
  Loc.Bottom := ClientHeight + 1;  {+1 is workaround for windows paint bug}
  Loc.Right := ClientWidth - FButton.Width - 2;
  Loc.Top := 0;  
  Loc.Left := 0;  
  SendMessage(Handle, EM_SETRECTNP, 0, LongWord(@Loc));
  SendMessage(Handle, EM_GETRECT, 0, LongWord(@Loc));  {debug}
end;

procedure TSpinEditLongWord.WMSize(var Message: TWMSize);
var
  MinHeight: Integer;
begin
  inherited;
  MinHeight := GetMinHeight;
    { text edit bug: if size to less than minheight, then edit ctrl does
      not display the text }
  if Height < MinHeight then   
    Height := MinHeight
  else if FButton <> nil then
  begin
    if NewStyleControls and Ctl3D then
      FButton.SetBounds(Width - FButton.Width - 5, 0, FButton.Width, Height - 5)
    else FButton.SetBounds (Width - FButton.Width, 1, FButton.Width, Height - 3);
    SetEditRect;
  end;
end;

function TSpinEditLongWord.GetMinHeight: Integer;
var
  DC: HDC;
  SaveFont: HFont;
  I: Integer;
  SysMetrics, Metrics: TTextMetric;
begin
  DC := GetDC(0);
  GetTextMetrics(DC, SysMetrics);
  SaveFont := SelectObject(DC, Font.Handle);
  GetTextMetrics(DC, Metrics);
  SelectObject(DC, SaveFont);
  ReleaseDC(0, DC);
  I := SysMetrics.tmHeight;
  if I > Metrics.tmHeight then I := Metrics.tmHeight;
  Result := Metrics.tmHeight + I div 4 + GetSystemMetrics(SM_CYBORDER) * 4 + 2;
end;

procedure TSpinEditLongWord.UpClick (Sender: TObject);
begin
  if ReadOnly then MessageBeep(0)
  else Value := Value + FIncrement;
end;

procedure TSpinEditLongWord.DownClick (Sender: TObject);
begin
  if ReadOnly then MessageBeep(0)
  else Value := Value - FIncrement;
end;

procedure TSpinEditLongWord.WMPaste(var Message: TWMPaste);   
begin
  if not FEditorEnabled or ReadOnly then Exit;
  inherited;
end;

procedure TSpinEditLongWord.WMCut(var Message: TWMPaste);   
begin
  if not FEditorEnabled or ReadOnly then Exit;
  inherited;
end;

procedure TSpinEditLongWord.CMExit(var Message: TCMExit);
begin
  inherited;
  if CheckValue (Value) <> Value then
    SetValue (Value);
end;

function TSpinEditLongWord.GetValue: LongWord;
var
  I64: Int64;
begin
  try
    I64 := StrToInt64(Text);
    Result := Max(FMinValue, Min(I64, High(LongWord)));
  except
    Result := FMinValue;
  end;
end;

procedure TSpinEditLongWord.SetValue (NewValue: LongWord);
begin
  Text := IntToStr (CheckValue (NewValue));
end;

function TSpinEditLongWord.CheckValue (NewValue: LongWord): LongWord;
begin
  Result := NewValue;
  if (FMaxValue <> FMinValue) then
  begin
    if NewValue < FMinValue then
      Result := FMinValue
    else if NewValue > FMaxValue then
      Result := FMaxValue;
  end;
end;

procedure TSpinEditLongWord.CMEnter(var Message: TCMGotFocus);
begin
  if AutoSelect and not (csLButtonDown in ControlState) then
    SelectAll;
  inherited;
end;

end.
