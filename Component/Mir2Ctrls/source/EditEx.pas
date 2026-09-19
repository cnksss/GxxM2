(*
 *
 * 单元 : 虫虫 <huan_2004@163.com>
 * 网站 :
 *
 * 说明 : EditEx
 *
 * CHANGES:
 * v1.1 <2010-3-23>
 *   + PaddingWidth: 文字与左右边距的间隔
 *)

unit EditEx;

interface

{$I HGVER.INC}

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Forms, Controls,
  StdCtrls, Menus, Themes, Dialogs;

type
  TValueType = (vtString, vtNumberOnly, vtFloat, vtHex);
  TPaintHintTextEvent = procedure(Sender: TObject; Canvas: TCanvas;
    var EnableAfterPaint: Boolean) of object;

  TCustomEditEx = class;
  TValueSetting = class(TPersistent)
  private
    FEditControl: TCustomEditEx;
    FValueType: TValueType;             //数据类型

    FShowThousands: Boolean;            //千位分隔
    FAutoCutDecimal: Boolean;           //自动截取小数
    FAppectMinus: Boolean;              //接受负数

    FFormatStr: string;                 //文本格式化字串
    FMaxLength: Integer;

    FDecimalPlaces,                     //小数位数
    FIntegerLength: Byte;               //整数长度

    FHideZeroValue: Boolean;            //隐藏零值

    procedure UpdateEditText(Value: Extended);
    procedure UpdateMaxLength;
    procedure UpdateFormatStr;

    function  GetFloatValue: Extended;
    procedure SetFloatValue(const Value: Extended);

    procedure SetDecimalPlaces(const Value: Byte);
    procedure SetIntegerLength(const Value: Byte);

    procedure SetValueType(const Value: TValueType);
    procedure SetShowThousands(const Value: Boolean);
    procedure SetHideZeroValue(const Value: Boolean);
  public
    constructor Create(EditControl: TCustomEditEx); reintroduce; virtual;
  published
    property ValueType: TValueType read FValueType write SetValueType default vtString;
    property FloatValue: Extended read GetFloatValue write SetFloatValue;

    property ShowThousands: Boolean read FShowThousands write SetShowThousands default False;
    property AutoCutDecimal: Boolean read FAutoCutDecimal write FAutoCutDecimal default False;
    property AppectMinus: Boolean read FAppectMinus write FAppectMinus default False;

    property DecimalPlaces: Byte read FDecimalPlaces write SetDecimalPlaces default 2;
    property IntegerLength: Byte read FIntegerLength write SetIntegerLength default 8;
    property HideZeroValue: Boolean read FHideZeroValue write SetHideZeroValue default False;

    property FormatStr: string read FFormatStr;
    property MaxLength: Integer read FMaxLength;
  end;

  TCustomEditEx = class(TCustomEdit)
  private
    FHintTextFont: TFont;
    FHintText: string;
    FHideCaret: Boolean;
    FOnPaintHintText: TPaintHintTextEvent;

    FAlignment: TAlignment;
    FValueSetting: TValueSetting;
    FPaddingWidth: Word;

    FHintParentFont: Boolean;

    function IsTextEmpty: Boolean;
    function IsHintTextEmpty: Boolean;

    function IsHintFontStored: Boolean;

    procedure SetHintTextFont(const Value: TFont);
    procedure SetHintText(const Value: string);
    procedure SetAlignment(const Value: TAlignment);

    procedure WMSetFocus(var Message: TWMSetFocus); message WM_SETFOCUS;
    procedure WMKillFocus(var Message: TWMSetFocus); message WM_KILLFOCUS;
    procedure WMPaint(var Message: TWMPaint); message WM_PAINT;
    procedure WMGetDlgCode(var Message: TWMGetDlgCode); message WM_GETDLGCODE;
    procedure WMKeyDown(var Message: TWMKeyDown); message WM_KEYDOWN;
    procedure WMPaste(var Message: TMessage); message WM_PASTE;
    procedure CMParentFontChanged(var Message: TMessage); message CM_PARENTFONTCHANGED;

    procedure SetHideCaret(const Value: Boolean);

    procedure SetPaddingWidth(const Value: Word);
    procedure SetHintParentFont(const Value: Boolean);
  protected
    FHintTextLeftMargin: Integer;

    FCanvas: TCanvas;
    procedure CreateWnd; override;
    procedure CreateParams(var Params: TCreateParams); override;
    procedure UpdateEditMargins; dynamic;
    procedure WndProc(var Message: TMessage); override;
    procedure KeyPress(var Key: Char); override;

    property HintText: string read FHintText write SetHintText;
    property HintTextFont: TFont read FHintTextFont write SetHintTextFont stored IsHintFontStored;
    property HideCaret: Boolean read FHideCaret write SetHideCaret;
    property Alignment: TAlignment read FAlignment write SetAlignment default taLeftJustify;
    property OnPaintHintText: TPaintHintTextEvent read FOnPaintHintText write FOnPaintHintText;
    property ValueSetting: TValueSetting read FValueSetting write FValueSetting;

    property PaddingWidth: Word read FPaddingWidth write SetPaddingWidth default 0;

    property HintParentFont: Boolean read FHintParentFont write SetHintParentFont default True;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
  end;
         
type
  TEditEx = class(TCustomEditEx)
  published
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
    property HideCaret;
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

{$IFDEF XE}
    property StyleElements;
{$ENDIF}

    property OnMouseMove;
    property OnMouseUp;
    property OnStartDock;
    property OnStartDrag;
  end;

implementation

{ TValueSetting }

function StrToValue(S: string; CutComma: Boolean): Extended;
begin
  S := Trim(S);
  if Length(S) = 0 then
  begin
    Result := 0;
    Exit;
  end;

  if CutComma then
    S := StringReplace(S, ',', '', [rfReplaceAll]);
  try
    Result := StrToFloat(S);
  except
    Result := 0;
  end;
end;

function CheckHexDigital(Value: string): Boolean;
var
  I: Integer;
begin
  Result := False;
  Value := Trim(Value);
  for I := 1 to Length(Value) do
  begin
    if not (Value[I] in ['0'..'9', 'a'..'f', 'A'..'F']) then
      Exit;
  end;

  Result := True;
end;

function CheckDigital(Value: string): Boolean;
var
  I: Integer;
begin
  Result := False;
  Value := Trim(Value);
  for I := 1 to Length(Value) do
  begin
    if not (Value[I] in ['0'..'9']) then
      Exit;
  end;

  Result := True;
end;

constructor TValueSetting.Create(EditControl: TCustomEditEx);
begin
  FEditControl := EditControl;
  FValueType := vtString;
  FShowThousands := False;
  FAppectMinus   := False;
  FAutoCutDecimal := False;
  FDecimalPlaces := 2;
  FIntegerLength := 8;
  FFormatStr := '0.00';
  FHideZeroValue := False;
  UpdateMaxLength;
end;

procedure TValueSetting.UpdateEditText(Value: Extended);
begin
  if ValueType <> vtFloat then Exit;

  if FHideZeroValue and (Value = 0) then
    FEditControl.Text := ''
  else
  begin
    if FShowThousands then
      FEditControl.Text := FormatFloat(FFormatStr, Value)
    else
      FEditControl.Text := FormatFloat(FFormatStr, Value);
  end;
end;

procedure TValueSetting.UpdateFormatStr;
var
  I: Integer;
  IntegerFormat, DecimalFormat: string;
begin
  if FShowThousands then
    IntegerFormat := '#,0'
  else
    IntegerFormat := '0';

  if FDecimalPlaces > 0 then
  begin
    IntegerFormat := IntegerFormat + '.';

    DecimalFormat := '';
    for I := 1 to FDecimalPlaces do
      DecimalFormat := DecimalFormat + '0';
  end;
  FFormatStr := IntegerFormat + DecimalFormat;
end;

procedure TValueSetting.UpdateMaxLength;
begin
  FMaxLength := FIntegerLength;

  if FShowThousands then
  begin
    FMaxLength := FIntegerLength + FIntegerLength div 3;
    if FIntegerLength mod 3 = 0 then
      Dec(FMaxLength);
  end;

  if FDecimalPlaces > 0 then
    FMaxLength := FMaxLength + 1 + FDecimalPlaces;
end;

procedure TValueSetting.SetDecimalPlaces(const Value: Byte);
begin
  if FDecimalPlaces <> Value then
  begin
    FDecimalPlaces := Value;

    UpdateMaxLength;
    UpdateFormatStr;
    UpdateEditText(FloatValue);
  end;
end;

procedure TValueSetting.SetIntegerLength(const Value: Byte);
begin
  if FIntegerLength <> Value then
  begin
    FIntegerLength := Value;
    UpdateMaxLength;
  end;
end;

procedure TValueSetting.SetShowThousands(const Value: Boolean);
begin
  if FShowThousands <> Value then
  begin
    FShowThousands := Value;
    UpdateMaxLength;
    UpdateFormatStr;
    UpdateEditText(FloatValue);
  end;
end;

procedure TValueSetting.SetFloatValue(const Value: Extended);
begin
  FEditControl.Modified := False;
  if FValueType <> vtFloat then
    FEditControl.Text := FloatToStr(Value)
  else
    UpdateEditText(Value);
end;

procedure TValueSetting.SetHideZeroValue(const Value: Boolean);
begin
  if FHideZeroValue <> Value then
  begin
    FHideZeroValue := Value;
    UpdateEditText(FloatValue);
  end;
end;

function TValueSetting.GetFloatValue: Extended;
var
  s: string;
begin
  s := StringReplace(FEditControl.Text, ',', '', [rfReplaceAll]);
  Result := StrToValue(s, (ValueType = vtFloat) and FShowThousands)
end;

procedure TValueSetting.SetValueType(const Value: TValueType);
var
  OldHeight: Integer;
  OldType: TValueType;
  Style: Longint;
begin
  if FValueType <> Value then
  begin
    OldHeight := FEditControl.Height;
    OldType := FValueType;
    FValueType := Value;

    if OldType = vtNumberOnly then
    begin
      Style := GetWindowLong(FEditControl.Handle, GWL_STYLE);
      SetWindowLong(FEditControl.Handle, GWL_STYLE, Style and not ES_NUMBER);
      FEditControl.Height := OldHeight;
    end;

    if FValueType = vtNumberOnly  then
    begin
      Style := GetWindowLong(FEditControl.Handle, GWL_STYLE);
      SetWindowLong(FEditControl.Handle, GWL_STYLE, Style or ES_NUMBER);
      FEditControl.Height := OldHeight;
    end
    else if Value = vtFloat then
      UpdateEditText(FloatValue);
  end;
end;

{ TEditEx }

procedure TCustomEditEx.CMParentFontChanged(var Message: TMessage);
begin
  inherited;
  if FHintParentFont then
  begin
    if Message.wParam <> 0 then
      FHintTextFont.Assign(TFont(Message.lParam)) else
      FHintTextFont.Assign(Font);
    FHintParentFont := True;
  end;
end;

constructor TCustomEditEx.Create(AOwner: TComponent);
begin
  inherited;
  FHintTextLeftMargin := 0;
  FAlignment := taLeftJustify;
  FCanvas := TControlCanvas.Create;
  TControlCanvas(FCanvas).Control := Self;

  FValueSetting := TValueSetting.Create(Self);

  FHintTextFont := TFont.Create;
  FHintTextFont.Assign(Font);

  FHintParentFont := True;

  FPaddingWidth := 0;
end;

destructor TCustomEditEx.Destroy;
begin
  FreeAndNil(FHintTextFont);
  FreeAndNil(FCanvas);
  FreeAndNil(FValueSetting);
  inherited;
end;

function TCustomEditEx.IsHintFontStored: Boolean;
begin
  Result := not HintParentFont;
end;

function TCustomEditEx.IsHintTextEmpty: Boolean;
begin
  Result := Length(Trim(HintText)) = 0;
end;

procedure TCustomEditEx.CreateParams(var Params: TCreateParams);
const
  Alignments: array[Boolean, TAlignment] of DWORD =
    ((ES_LEFT, ES_RIGHT, ES_CENTER),(ES_RIGHT, ES_LEFT, ES_CENTER));
begin
  inherited;
  Params.Style := Params.Style or ES_MULTILINE or
    Alignments[UseRightToLeftAlignment, FAlignment];
  { if not set ES_MULTILINE style then
    SendMessage(Handle, EM_SETRECT, 0, Longint(@R)) not run }
end;

procedure TCustomEditEx.CreateWnd;
begin
  inherited;
  UpdateEditMargins;
end;

procedure TCustomEditEx.SetAlignment(const Value: TAlignment);
begin
  if FAlignment <> Value then
  begin
    FAlignment := Value;
    RecreateWnd;
  end;
end;

procedure TCustomEditEx.SetPaddingWidth(const Value: Word);
begin
  if FPaddingWidth <> Value then
  begin
    FPaddingWidth := Value;
    UpdateEditMargins;
  end;
end;

procedure TCustomEditEx.SetHideCaret(const Value: Boolean);
begin
  if FHideCaret <> Value then
  begin
    FHideCaret := Value;
    if Focused then
    begin
      if FHideCaret then
        Windows.HideCaret(Handle)
      else
        Windows.ShowCaret(Handle);
    end;
  end;
end;

procedure TCustomEditEx.SetHintParentFont(const Value: Boolean);
begin
  if FHintParentFont <> Value then
  begin
    FHintParentFont := Value;
    if (Parent <> nil) and not (csReading in ComponentState) then
      Perform(CM_PARENTFONTCHANGED, 0, 0);
  end;
end;

procedure TCustomEditEx.SetHintText(const Value: string);
begin
  FHintText := Value;
  if IsTextEmpty and (not IsHintTextEmpty) then
    Invalidate;
end;

procedure TCustomEditEx.SetHintTextFont(const Value: TFont);
begin
  FHintTextFont.Assign(Value);
  FHintParentFont := False;
  if IsTextEmpty and (not IsHintTextEmpty) then
    Invalidate;
end;

procedure TCustomEditEx.UpdateEditMargins;
var
  R: TRect;
  H: Integer;
begin
  //if HandleAllocated then
  R := ClientRect;
  Inc(R.Left);
  Dec(R.Right);

  FCanvas.Font.Assign(Font);

  H := FCanvas.TextHeight('|');
  H := ((R.Bottom - R.Top) - H) div 2;

  InflateRect(R, -1 * FPaddingWidth, -H);
  SendMessage(Handle, EM_SETRECT, 0, Longint(@R));
  Invalidate;
end;

procedure TCustomEditEx.WMGetDlgCode(var Message: TWMGetDlgCode);
begin
  inherited;
  Message.Result := Message.Result and not DLGC_WANTTAB;
  Message.Result := Message.Result and not DLGC_WANTALLKEYS;
end;

procedure TCustomEditEx.WMKillFocus(var Message: TWMSetFocus);
begin
  inherited;
  Invalidate;
end;

procedure TCustomEditEx.WMSetFocus(var Message: TWMSetFocus);
begin
  inherited;
  Invalidate;

  if FHideCaret then
    Windows.HideCaret(Handle);
end;

procedure TCustomEditEx.WndProc(var Message: TMessage);
begin
  inherited;
  case Message.Msg of
    WM_SIZE:        if not (csLoading in ComponentState) then UpdateEditMargins;
    CM_FONTCHANGED: if not (csLoading in ComponentState) then UpdateEditMargins;
  end;
end;

procedure TCustomEditEx.WMPaint(var Message: TWMPaint);
var
  EnableAfterPaint: Boolean;
  Y: Integer;
begin
  inherited;
  if {(not Focused) and} (not IsHintTextEmpty) and IsTextEmpty then
  begin
    FCanvas.Font.Assign(FHintTextFont);

  {$IF CompilerVersion >= 22.0} //delphi XE
    FCanvas.Font.Height := MulDiv(FHintTextFont.Height, FCurrentPPI, 96);
  {$IFEND}

    EnableAfterPaint := True;
    if Assigned(FOnPaintHintText) then
      FOnPaintHintText(Self, FCanvas, EnableAfterPaint);

    if EnableAfterPaint then
    begin
      Y := (Self.ClientHeight - FCanvas.TextHeight('|')) div 2;
      FCanvas.Brush.Style := bsClear;
      FCanvas.TextOut(3 + FHintTextLeftMargin, Y, FHintText);
    end;
  end;
end;

procedure TCustomEditEx.WMPaste(var Message: TMessage);
var
  HWND: THandle;
  Str: string;
  E: Double;
begin
  if ValueSetting.ValueType = vtString then
  begin
    inherited;
    Exit;
  end;

  if not IsClipboardFormatAvailable(CF_TEXT) then Exit;
  try
    OpenClipBoard(Handle);
    HWND := GetClipboardData(CF_TEXT);
    if HWND = 0 then Exit;
    Str := StrPas(PChar(GlobalLock(HWND)));
    GlobalUnlock(HWND);
  finally
    CloseClipBoard;
  end;


  if ValueSetting.ValueType = vtNumberOnly then
  begin
    if (Length(Str) > MaxLength) or (not CheckDigital(Str)) then
    begin
      Beep;
      Exit;
    end;

    Text := Str;
  end
  else if ValueSetting.ValueType = vtHex then
  begin
    if (Length(Str) > MaxLength) or (not CheckHexDigital(Str)) then
    begin
      Beep;
      Exit;
    end;

    Text := Str;
  end
  else
  begin
    try
      E := StrToFloat(Str);
    except
      Beep;
      Exit;
    end;

    if ValueSetting.HideZeroValue and (E = 0) then
      Text := ''
    else
    begin
      if ValueSetting.ShowThousands then
        Str := FormatFloat(ValueSetting.FormatStr, E)
      else
        Str := FormatFloat(ValueSetting.FormatStr, E);

      if Length(Str) > ValueSetting.MaxLength then
      begin
        Beep;
        Exit;
      end
      else
        Text := Str;
    end;
  end;
end;

function TCustomEditEx.IsTextEmpty: Boolean;
begin
  Result := Length(Trim(Text)) = 0;
end;

procedure TCustomEditEx.WMKeyDown(var Message: TWMKeyDown);
var
  strFront, strSel, strBack, strText: string;
  SelIndex: Integer;
  E: Extended;
begin
  if not ((ValueSetting.ValueType = vtFloat) and
    (Message.CharCode = VK_DELETE) and (Message.Unused = 0)) then
  begin
    inherited;
    Exit;
  end;

  if ReadOnly then
  begin
    inherited;
    Exit;
  end;

  // 拦截 delete 键
  strFront:= Copy(Text, 0, SelStart);
  strSel  := Copy(Text, SelStart + 1, SelLength);
  strBack := Copy(Text, SelStart + SelLength + 1, MAXINT);
  SelIndex := Length(Text) - SelStart - SelLength;

  if SelLength > 0 then
    strText := strFront + strBack
  else
  begin
    strText := strFront + Copy(strBack, 2, MAXINT);
    Dec(SelIndex);
  end;

  E := StrToValue(strText, ValueSetting.ShowThousands);

  if ValueSetting.ShowThousands then
    Text := FormatFloat(ValueSetting.FormatStr, E)
  else
    Text := FormatFloat(ValueSetting.FormatStr, E);

  SelStart := Length(Text) - SelIndex;
  Modified := True;
  Message.Result := 1;
end;

procedure TCustomEditEx.KeyPress(var Key: Char);
const
  CTRL_A = #1;
  CTRL_C = #3;
  CTRL_V = #22;
  CTRL_X = #24;
  CTRL_Z = #26;
var
  strFront, strSel, strBack, strText: string;
  Index, SelIndex: Integer;
  E: Extended;
  InputMinus: Boolean;
  SetKeys: set of Char;
begin
  inherited;
  if (Key = Char(VK_RETURN)) then Key := #0;

  if Key in [CTRL_A, CTRL_C, CTRL_V, CTRL_X, CTRL_Z] then Exit;


  if (ValueSetting.ValueType = vtHex) then
  begin
    if ReadOnly then
    begin
      Key := #0;
      Beep;
      Exit;
    end;
    
    SetKeys := [#8, '0'..'9', 'a'..'f', 'A'..'F'];

    //if not ValueSetting.AppectMinus then
    //  Exclude(SetKeys, '-');

    if not (Key in SetKeys) then
    begin
      Key := #0;
      Beep;
      Exit;
    end;
  end;

  if (ValueSetting.ValueType <> vtFloat) then Exit;

  if ReadOnly then
  begin
    Key := #0;
    Beep;
    Exit;
  end;

  SetKeys := ['-', '.', #8, '0'..'9'];
  if not ValueSetting.AppectMinus then
    Exclude(SetKeys, '-');
  if ValueSetting.DecimalPlaces = 0 then
    Exclude(SetKeys, '.');

  if not (Key in SetKeys) then
  begin
    Key := #0;
    Beep;
    Exit;
  end;

  InputMinus := False;   
  SelIndex := Length(Text) - SelStart - SelLength;

  strFront:= Copy(Text, 0, SelStart);
  strSel  := Copy(Text, SelStart + 1, SelLength);
  strBack := Copy(Text, SelStart + SelLength + 1, MAXINT);
  if Key = '-' then
  begin
    Key := #0;
    if SelStart = 0 then
    begin
      strText :=  '-' + strBack;
      InputMinus := True;
    end
    else
    begin
      Beep;
      Exit;
    end;
  end
  else if Key = '.' then
  begin
    Key := #0;
    if Pos('.', strFront) = 0 then
    begin
      Index := Pos('.', strBack);
      if Index = 0 then
        strText := strFront + '.' + strBack
      else
      begin
        if ValueSetting.AutoCutDecimal then
        begin
          strText := strFront + '.' + Copy(strBack, Index + 1, MAXINT);
          Dec(SelIndex, Index);
        end
        else
        begin
          Beep;
          Exit;
        end;
      end;
    end
    else
    begin
      Beep;
      Exit;
    end;
  end
  else if Key = #8 then
  begin
    Key := #0;
    if SelLength > 0 then
      strText := strFront + strBack
    else
      strText := Copy(strFront, 0, Length(strFront) - 1) + strBack;
  end
  else if Key in ['0'..'9'] then
  begin
    if SelLength <> 0 then
      strText := strFront + Key + strBack
    else
    begin
    begin
      strText := strFront + Key + strBack;
      Index := Pos('.', strFront);
      if SelStart >= Index then
      begin
        SelIndex := SelIndex - 1;
      end;
    end;

      if Length(strText) > ValueSetting.MaxLength then
      begin
        Index := Pos('.', strFront);
        if Index > 0 then
        begin
          strText[Length(strText)] := #0;
          Text := strText;
          Modified := True;
          SelStart := Length(Text) - SelIndex;
          Exit;
        end;

        Beep;
        Key := #0;
        Exit;
      end;
    end;
    Key := #0;
  end;

  E := StrToValue(strText, ValueSetting.ShowThousands);

  strText := FormatFloat(ValueSetting.FormatStr, E);

  if (E = 0) and InputMinus then
    strText := '-' + strText;
  Text := strText;
  SelStart := Length(Text) - SelIndex;
  Modified := True;
end;


end.
