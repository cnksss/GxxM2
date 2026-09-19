unit DxEdit;

interface
uses
  Windows,
  Types,
  Classes,
  Controls,
  SysUtils,
  Graphics,
  Forms,
  Clipbrd,
  HGE,
  HGEFontEx,
  DxComponents,
  DxControls,
  DxPopupMenu;
type
  TEditHistory = record
    Text:string;
    Selection:TSelection;
  end;
  pTEditHistory = ^TEditHistory;

  TDxEdit = class(TDxControl)
  private
    Ticks:Integer;
    FBlinkTicks:Integer;
    FReadOnly:Boolean;
    FSelection:TSelection;
    FOnChange:TNotifyEvent;
    FOnEnter:TNotifyEvent;

    FWideChar:array[0..1] of Char;

    FText:string;
    FPasswordText:string;
    FShowPasswordText:Boolean;
    FAllowSelect:Boolean;
    FAllowPaste:Boolean;
    FPasswordChar:Char;
    FViewPos:Integer;

    FSelectedColor:TColor;
    FSelBackColor:TColor;
    FSelFontColor:TColor;
    FInValue:TInValue;
    FFont:TDxFont;

    FDisableBackgroundColor:TColor;
    FHintTextFont:TDxFont;
    FHintText:string;
    FHintTextAlignment:TAlignment;

    FMaxLength:Integer;

    FSelIndex:Integer;
    FPoint:TPoint;

    FOldText:string;
    FOldSelection:TSelection;
    FEditHistorys:array of TEditHistory;

    FOnUnFocused:TNotifyEvent;

    procedure EnterKey(var Key:Word);
    procedure Change;
    procedure SetText(Value:string);
    procedure SetSelText(Value:string);
    procedure SetSelStart(Value:Integer);
    procedure SetMaxLength(Value:Integer);
    procedure SetSelLength(Value:Integer);
    function GetValue:Integer;
    procedure SetValue(Value:Integer);
    function GetSelText:string;
    function GetSelStart:Integer;
    function GetSelLength:Integer;
    function GetNextEdit:TDxEdit;
    function GetMaxTabOrder:Integer;
    procedure SetViewPos(const Value:Integer);
    function GetText:string;
    procedure SelectPos(Pt:TPoint; vRect:TRect);
    procedure SelectChar(Pt:TPoint; vRect:TRect);
    procedure PopupMenuClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DrawText(vtRect, vbRect:TRect);
    procedure DrawHintText(vtRect, vbRect:TRect);
    procedure DrawSelector(vtRect, vbRect:TRect);
    procedure MoveCursor(Index:Integer);

    procedure RecoveryEditHistory;
  protected
    procedure SaveEditHistory; overload;
    procedure SaveEditHistory(const AText:string; const ASelection:TSelection); overload;

    procedure DoUpdate(); override;
    procedure DoMouseEnter; override;
    procedure DoMouseLeave; override;

    procedure DoShow(); override;
    procedure DoHide(); override;
    procedure DoFocused(); override;
    procedure DoUnFocused(); override;

  public
    function InRange(X, Y:Integer):Boolean; override;

    procedure KeyPress(var Key:Char); override;
    procedure KeyDown(var Key:Word; Shift:TShiftState); override;
    procedure MouseMove(Shift:TShiftState; X, Y:Integer); override;
    procedure MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); override;
    procedure MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer); override;
  public
    constructor Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND}); override;
    destructor Destroy(); override;
    procedure Paint; override;
    procedure SetFocus(); override;
    procedure SelectAll;
    procedure DeleteText;
    procedure BackText;
    procedure CopyText;
    procedure CutText;
    procedure PasteText;
    property ViewPos:Integer read FViewPos write SetViewPos;
    property SelLength:Integer read GetSelLength write SetSelLength;
    property SelStart:Integer read GetSelStart write SetSelStart;
    property SelText:string read GetSelText write SetSelText;
    property BlinkTicks:Integer read FBlinkTicks write FBlinkTicks;
    property PopupMenu;
    property OnChange:TNotifyEvent read FOnChange write FOnChange;
    property OnEnter:TNotifyEvent read FOnEnter write FOnEnter;
    property OnUnFocused:TNotifyEvent read FOnUnFocused write FOnUnFocused;

    {$MESSAGE HINT '此处可能会影响，插件API的结构和GuiEdit.dll的操作，故标记'}
    //HZQ 消除警告 20230519，ClientExe = 1 时继承自TObject, ClientExe = 0时继承自TComponent
    {$IF CLIENTEXE = 1}
  public
    {$ELSE}
  published
    {$IFEND}
    property BackgroundColor;

    property DisableBackgroundColor:TColor read FDisableBackgroundColor write FDisableBackgroundColor;
    property HintTextFont:TDxFont read FHintTextFont write FHintTextFont;
    property HintText:string read FHintText write FHintText;

    property DrawBorder;
    property Font:TDxFont read FFont write FFont;
    property Text:string read FText write SetText;
    property Value:Integer read GetValue write SetValue;
    property ReadOnly:Boolean read FReadOnly write FReadOnly;
    property MaxLength:Integer read FMaxLength write SetMaxLength;
    property SelectedColor:TColor read FSelectedColor write FSelectedColor;
    property SelBackColor:TColor read FSelBackColor write FSelBackColor;
    property SelFontColor:TColor read FSelFontColor write FSelFontColor;
    property PasswordChar:Char read FPasswordChar write FPasswordChar;
    property AllowSelect:Boolean read FAllowSelect write FAllowSelect;
    property AllowPaste:Boolean read FAllowPaste write FAllowPaste;
    property InValue:TInValue read FInValue write FInValue;

  end;

implementation
uses Math,
  HGECanvas;
const
  TextChars = [#32..#255];

function CopyEx(Source:string; StartPos, Len:Integer):string;
begin
  if StartPos < 1 then StartPos := 1;
  if StartPos > Length(Source) then StartPos := Length(Source);
  if Len < 0 then Len := 0;
  if Len > Length(Source) then Len := Length(Source);

  if ByteType(Source, StartPos) = mbTrailByte then
    Inc(StartPos);

  if ByteType(Source, Len) = mbTrailByte then begin // 汉字的第二个字节
    Result := Copy(Source, StartPos, Len);
  end
  else if ByteType(Source, Len) = mbLeadByte then begin // 汉字的第一个字节
    Result := Copy(Source, StartPos, Len - 1);
  end
  else begin
    Result := Copy(Source, StartPos, Len);
  end;
end;

function IsStringNumber(const Str:string):Boolean;
var
  I:Integer;
begin
  Result := True;
  for I := 1 to Length(Str) do
    if (Byte(Str[I]) < Byte('0')) or (Byte(Str[I]) > Byte('9')) then begin
      Result := False;
      Break;
    end;
end;

procedure StripWrong(var Text:string);
var
  i:Integer;
begin
  // Text:= Trim(Text);

  for i := Length(Text) downto 1 do
    if (not (Text[i] in TextChars)) then Delete(Text, i, 1);
end;

constructor TDxEdit.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
begin
  inherited Create(AOwner);
  Transparent := False;
  EnableFocus := True;

  Width := 100;
  Height := 20;

  FOnChange := nil;
  FOnEnter := nil;
  FInValue := vString;

  FText := '';
  Ticks := 0;

  FReadOnly := False;
  FMaxLength := 0;

  FillChar(FWideChar, SizeOf(FWideChar), #0);

  FPasswordText := '';
  FShowPasswordText := False;

  FFont := TDxFont.Create;
  FFont.Color := clWhite;
  FFont.Bold := False;

  BackgroundColor := clBlack;

  FDisableBackgroundColor := clBlack;
  FHintTextFont := TDxFont.Create;
  FHintTextFont.Color := clWhite;
  FHintTextFont.Bold := False;
  FHintText := '';
  FHintTextAlignment := taLeftJustify;

  DrawBorder := True;

  FSelectedColor := clWhite;
  FSelBackColor := clBlue;
  FSelFontColor := clWhite;

  FBlinkTicks := 16;

  FAllowSelect := True;
  FAllowPaste := False;

  FSelection.StartPos := 0;
  FSelection.EndPos := 0;

  FPasswordChar := #0;

  TabOrder := GetMaxTabOrder + 1;

  FEditHistorys := nil;
  FOldText := FText;
  FOldSelection := FSelection;
end;

destructor TDxEdit.Destroy();
begin
  FFont.Free;
  FHintTextFont.Free;
  SetLength(FEditHistorys, 0);
  FEditHistorys := nil;
  inherited Destroy();
end;
// ------------------------------------------------------------------------------

// ------------------------------------------------------------------------------

function TDxEdit.GetMaxTabOrder:Integer;
var
  I, nTabOrder:Integer;
  DxEdit:TDxEdit;
begin
  nTabOrder := 0;
  if Owner <> nil then begin
    for I := 0 to TDxControl(Owner).ControlCount - 1 do begin
      if (TDxControl(Owner).Control[I] is TDxEdit) then begin
        DxEdit := TDxEdit(TDxControl(Owner).Control[I]);
        if DxEdit.TabOrder >= nTabOrder then begin
          nTabOrder := DxEdit.TabOrder;
        end;
      end;
    end;
  end;
  Result := nTabOrder;
end;

function NumberSort_2(List:TStringList; Index1, Index2:Integer):Integer;
var
  Value1, Value2:Integer;
begin
  Result := 0;
  try
    Value1 := StrToInt(List[Index1]);
    Value2 := StrToInt(List[Index2]);
    if Value1 > Value2 then
      Result := 1
    else if Value1 < Value2 then
      Result := -1
    else
      Result := 0;
  except
  end;
end;

function TDxEdit.GetNextEdit:TDxEdit;
var
  I:Integer;
  ControlList:TStringList;
  DxEdit:TDxEdit;
begin
  Result := Self;
  if Owner <> nil then begin
    ControlList := TStringList.Create;
    for I := 0 to TDxControl(Owner).ControlCount - 1 do begin
      if (TDxControl(Owner).Control[I] is TDxEdit) then begin
        DxEdit := TDxEdit(TDxControl(Owner).Control[I]);
        if DxEdit.Visible and DxEdit.Enabled then
          ControlList.AddObject(IntToStr(DxEdit.TabOrder), DxEdit);
      end;
    end;
    ControlList.CustomSort(NumberSort_2);

    for I := 0 to ControlList.Count - 1 do begin
      if ControlList.Objects[I] = Self then begin
        if I >= ControlList.Count - 1 then begin
          Result := TDxEdit(ControlList.Objects[0]);
        end
        else begin
          Result := TDxEdit(ControlList.Objects[I + 1]);
        end;
        break;
      end;
    end;
    ControlList.Free;
  end;
end;

// ------------------------------------------------------------------------------

procedure TDxEdit.SetViewPos(const Value:Integer);
var
  PaintRect:TRect;
  HGEFont:THGEFont;
begin
  HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);
  if HGEFont <> nil then begin
    FViewPos := Value;
    PaintRect := ShrinkRect(VirtualRect, 2, 0);

    if HGEFont.TextWidth(Text) < (PaintRect.Right - PaintRect.Left) then
      FViewPos := 0;

    if HGEFont.TextWidth(Text) - FViewPos < (PaintRect.Right - PaintRect.Left) - HGEFont.TextWidth('0') then
      FViewPos := HGEFont.TextWidth(Text) - ((PaintRect.Right - PaintRect.Left) - HGEFont.TextWidth('0'));

    if (FViewPos < 0) then FViewPos := 0;
  end;
end;

procedure TDxEdit.Change;
begin
  Repaint;
  if (Assigned(FOnChange)) then FOnChange(Self);
end;

function TDxEdit.GetSelLength:Integer;
var
  AText:WideString;
  AMin, AMax:Integer;
begin
  AText := Text;
  AMin := Min(FSelection.StartPos, FSelection.EndPos);
  AMax := Max(FSelection.StartPos, FSelection.EndPos);
  Result := Max(AMax - AMin, 0);
end;

procedure TDxEdit.SetSelLength(Value:Integer);
var
  AText:WideString;
begin
  SaveEditHistory;
  AText := Text;
  FSelection.EndPos := FSelection.StartPos + Value;

  if FSelection.EndPos > Length(AText) then
    FSelection.EndPos := Length(AText);

  if FSelection.EndPos < 0 then
    FSelection.EndPos := 0;
end;

procedure TDxEdit.SetSelStart(Value:Integer);
begin
  SaveEditHistory;
  if not ((Value < 1) and (Value > Length(Text))) then begin
    FSelection.StartPos := Value;
    FSelection.EndPos := FSelection.StartPos;
  end;
end;

function TDxEdit.GetSelStart:Integer;
begin
  Result := Min(FSelection.StartPos, FSelection.EndPos);
end;

function TDxEdit.GetSelText:string;
var
  AMin, AMax, ALength:Integer;
  AText:WideString;
begin
  AMin := Min(FSelection.StartPos, FSelection.EndPos);
  AMax := Max(FSelection.StartPos, FSelection.EndPos);
  ALength := AMax - AMin;
  AText := Text;
  Result := Copy(AText, AMin + 1, ALength);
end;

procedure TDxEdit.SetMaxLength(Value:Integer);
begin
  if FMaxLength <> Value then begin
    FMaxLength := Value;
    if (FMaxLength < Length(Text)) and (FMaxLength > 0) then begin
      FText := CopyEx(FText, 1, FMaxLength);
    end;
  end;
end;

// ---------------------------------------------------------------------------

function TDxEdit.GetValue:Integer;
begin
  Result := StrToIntDef(Text, 0);
end;

procedure TDxEdit.SetValue(Value:Integer);
begin
  if (Value = 0) and (FInValue = vInteger) then
    Text := ''
  else
    Text := IntToStr(Value);
end;

function TDxEdit.GetText:string;
var
  I:Integer;
  P:PChar;
begin
  Result := '';
  if FPasswordChar <> #0 then begin
    SetLength(Result, Length(Text));
    P := PChar(Result);
    for I := 1 to Length(Text) do begin
      P^ := FPasswordChar;
      Inc(P);
    end;
  end
  else begin
    Result := FText;
  end;
end;

procedure TDxEdit.SetSelText(Value:string);
var
  AText:WideString;
  AMin, AMax, ALength:Integer;
begin
  SaveEditHistory;
  StripWrong(Value);
  AText := FText;

  AMin := Min(FSelection.StartPos, FSelection.EndPos);
  AMax := Max(FSelection.StartPos, FSelection.EndPos);

  ALength := AMax - AMin;

  // Insert Key
  if ALength > 0 then
    Delete(AText, AMin + 1, ALength);

  Inc(AMin);
  Insert(Value, AText, AMin + 1);

  Text := AText;
  // Change;

  if AMin > Length(Text) then
    AMin := Length(Text);

  FSelection.StartPos := AMin;
  FSelection.EndPos := AMin + Length(Value);
end;

procedure TDxEdit.RecoveryEditHistory;
var
  EditHistory:pTEditHistory;
begin
  if Length(FEditHistorys) > 0 then begin
    EditHistory := @FEditHistorys[Length(FEditHistorys) - 1];
    FText := EditHistory.Text;
    FSelection := EditHistory.Selection;
    MoveCursor(FSelection.EndPos);
    SetLength(FEditHistorys, Length(FEditHistorys) - 1);
  end;
end;

procedure TDxEdit.SaveEditHistory;
var
  I:Integer;
  EditHistory:pTEditHistory;
begin
  SetLength(FEditHistorys, Length(FEditHistorys) + 1);
  EditHistory := @FEditHistorys[Length(FEditHistorys) - 1];
  EditHistory.Text := Text;
  EditHistory.Selection := FSelection;
  if Length(FEditHistorys) > 100 then begin
    for I := 50 to Length(FEditHistorys) - 1 do begin
      FEditHistorys[I - 50] := FEditHistorys[I];
    end;
    SetLength(FEditHistorys, Length(FEditHistorys) - 50);
  end;
end;

procedure TDxEdit.SaveEditHistory(const AText:string; const ASelection:TSelection);
var
  I:Integer;
  EditHistory:pTEditHistory;
begin
  SetLength(FEditHistorys, Length(FEditHistorys) + 1);
  EditHistory := @FEditHistorys[Length(FEditHistorys) - 1];
  EditHistory.Text := AText;
  EditHistory.Selection := ASelection;
  if Length(FEditHistorys) > 100 then begin
    for I := 50 to Length(FEditHistorys) - 1 do begin
      FEditHistorys[I - 50] := FEditHistorys[I];
    end;
    SetLength(FEditHistorys, Length(FEditHistorys) - 50);
  end;
end;

procedure TDxEdit.SetText(Value:string);
var
  PaintRect:TRect;
  HGEFont:THGEFont;
  // AText: WideString;
begin
  SaveEditHistory;
  StripWrong(Value);
  if (FMaxLength > 0) and (FMaxLength < Length(Value)) then begin
    Value := CopyEx(Value, 1, FMaxLength);
  end;
  // /nPos := Pos(#13#10, Value);
  // if nPos > 0 then
    // Value := Copy(Value, 1, nPos - 1);
  FText := Value;
  // AText := FText;
  FSelection.StartPos := 0; // Length(AText);
  FSelection.EndPos := 0; // Length(AText);

  HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);
  if HGEFont <> nil then begin
    PaintRect := ShrinkRect(VirtualRect, 2, 0);

    if HGEFont.TextWidth(FText) < (PaintRect.Right - PaintRect.Left) then
      FViewPos := 0;
    // ViewPos := FViewPos;
    // SelStart := Length(FText);
  end;
end;

procedure TDxEdit.KeyPress(var Key:Char);
var
  AText:WideString;
  sText:string;
  sData:string;
  AMin, AMax, ALength:Integer;
  DxEdit:TDxEdit;

  HGEFont:THGEFont;
begin
  inherited KeyPress(Key);
  if (FReadOnly) or (not Enabled) then Exit;

  AText := Text;

  AMin := Min(FSelection.StartPos, FSelection.EndPos);
  AMax := Max(FSelection.StartPos, FSelection.EndPos);

  ALength := AMax - AMin;
  // Insert Key
  if (Key > #31) then begin
    if (ALength > 0) then begin
      sText := SelText;
      // AText := Text;
      Delete(AText, AMin + 1, ALength);
      Text := AText;

      FSelection.StartPos := AMin;
      FSelection.EndPos := AMin;
      HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);
      if HGEFont <> nil then
        ViewPos := ViewPos - HGEFont.TextWidth(sText);
      Change;
    end;

    sData := '';

    {
    if IsDBCSLeadByte(Ord(Key)) then
    begin
      if FWideChar[0] = #0 then
      begin
        FWideChar[0] := Key;
        FWideChar[1] := #0;
      end
      else
      begin
        FWideChar[1] := Key;
        sData := FWideChar;
        FillChar(FWideChar, SizeOf(FWideChar), #0);
      end;
    end
    else
    begin
      FillChar(FWideChar, SizeOf(FWideChar), #0);
      sData := Key;
    end;
    }

    // 修正DxEdit控件输入时错误，比如输入瞭 chongchong 2014-05-29
    if Ord(FWideChar[0]) <> 0 then begin
      FWideChar[1] := Key;
      sData := FWideChar;
      FillChar(FWideChar, SizeOf(FWideChar), #0);
    end
    else begin
      // IsDBCSLeadByte用于判断一个指定字节是否为一个双字节字符的头一个字符
      if IsDBCSLeadByte(Ord(Key)) then begin
        FWideChar[0] := Key;
        FWideChar[1] := #0;
        Exit;
      end
      else begin
        FillChar(FWideChar, SizeOf(FWideChar), #0);
        sData := Key;
      end;
    end;

    if Length(sData) > 0 then begin
      case FInValue of
        vString:begin
            Inc(AMin);
            Insert(sData, AText, AMin);

            Text := AText;

            HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);
            if HGEFont <> nil then
              ViewPos := ViewPos + HGEFont.TextWidth(sData);

            Change;
          end;
        vInteger:begin
            if IsStringNumber(sData) then begin
              Inc(AMin);
              Insert(sData, AText, AMin);

              Text := AText;

              HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);
              if HGEFont <> nil then
                ViewPos := ViewPos + HGEFont.TextWidth(sData);

              Change;
            end;
          end;
      end;
    end;

    if AMin >= Length(Text) then
      AMin := Length(Text);

    FSelection.StartPos := AMin;
    FSelection.EndPos := AMin;

  end;
  if Key = #9 then begin
    DxEdit := GetNextEdit;
    if DxEdit <> nil then begin
      if DxEdit.AllowSelect then
        DxEdit.SelectAll;
      DxEdit.SetFocus;
    end;
  end;
end;

procedure TDxEdit.EnterKey(var Key:Word);
begin
  case Key of
    VK_BACK,
      Byte('D'):BackText;
    Byte('C'):CopyText;
    Byte('X'):CutText;
    Byte('Z'):RecoveryEditHistory;
    Byte('V'):PasteText;
    Byte('A'):SelectAll;

  end;
  // Key := 0;
end;

procedure TDxEdit.KeyDown(var Key:Word; Shift:TShiftState);
var
  AText:WideString;
begin
  inherited KeyDown(Key, Shift);
  if (FReadOnly) or (not Enabled) then Exit;

  // DebugOut('KeyDown:'+IntToStr(Key));
  case Key of
    VK_RIGHT:begin
        AText := Text;
        if FAllowSelect and (ssShift in Shift) then begin
          if FSelection.EndPos < Length(AText) then begin
            Inc(FSelection.EndPos);
            MoveCursor(FSelection.EndPos);
          end;
        end
        else begin
          if FSelection.EndPos < Length(AText) then
            Inc(FSelection.EndPos);
          FSelection.StartPos := FSelection.EndPos;

          MoveCursor(FSelection.EndPos);
        end;
      end;
    VK_LEFT:begin
        if FAllowSelect and (ssShift in Shift) then begin
          if FSelection.EndPos > 0 then begin
            Dec(FSelection.EndPos);
            MoveCursor(FSelection.EndPos);
          end;
        end
        else begin
          if FSelection.EndPos > 0 then
            Dec(FSelection.EndPos);
          FSelection.StartPos := FSelection.EndPos;
          MoveCursor(FSelection.EndPos);
        end;
      end;
    VK_BACK:EnterKey(Key);

    VK_DELETE:
      if (not FReadOnly) then begin
        DeleteText;
      end;

    VK_HOME:begin
        if FAllowSelect and (ssShift in Shift) then begin
          FSelection.EndPos := 0;
          MoveCursor(FSelection.EndPos);
        end
        else begin
          FSelection.StartPos := 0;
          FSelection.EndPos := 0;
          MoveCursor(FSelection.EndPos);
        end;
      end;

    VK_END:begin
        AText := Text;
        if FAllowSelect and (ssShift in Shift) then begin
          FSelection.EndPos := Length(AText);
          MoveCursor(FSelection.EndPos);
        end
        else begin
          FSelection.StartPos := Length(AText);
          FSelection.EndPos := Length(AText);
          MoveCursor(FSelection.EndPos);
        end;
      end;
    VK_RETURN:;
  end;
  if (ssCtrl in Shift) and Enabled then EnterKey(Key);
end;

procedure TDxEdit.BackText;
var
  AMin:Integer;
  AText:WideString;
  sText:string;
  HGEFont:THGEFont;
begin
  if (not FReadOnly) and Enabled then begin
    HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);
    if HGEFont <> nil then begin
      AMin := Min(FSelection.StartPos, FSelection.EndPos);
      if (SelLength > 0) then begin
        sText := SelText;
        AText := Text;
        Delete(AText, AMin + 1, SelLength);
        Text := AText;

        FSelection.StartPos := AMin;
        FSelection.EndPos := AMin;

        ViewPos := ViewPos - HGEFont.TextWidth(sText);
        Change;
      end
      else begin
        if AMin > 0 then begin
          AText := Text;
          if Length(AText) = AMin then begin
            Delete(AText, AMin, 1);
            Dec(AMin);
          end
          else begin
            Delete(AText, AMin, 1);
            Dec(AMin);
          end;
          Text := AText;
          FSelection.StartPos := AMin;
          FSelection.EndPos := AMin;

          if (HGEFont.TextWidth(Text) <= ViewPos) then
            ViewPos := ViewPos - Width;

          Change;
        end;
      end;
    end;
  end;
end;

procedure TDxEdit.DeleteText;
var
  AMin, AMax:Integer;
  AText:WideString;
  sText:string;
  HGEFont:THGEFont;
begin
  if (not FReadOnly) and Enabled then begin
    HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);
    if HGEFont <> nil then begin
      AMin := Min(FSelection.StartPos, FSelection.EndPos);
      AMax := Max(FSelection.StartPos, FSelection.EndPos);
      if (SelLength > 0) then begin
        sText := SelText;
        AText := Text;
        Delete(AText, AMin + 1, SelLength);
        Text := AText;

        FSelection.StartPos := AMin;
        FSelection.EndPos := AMin;

        ViewPos := ViewPos - HGEFont.TextWidth(sText);
        Change;
      end
      else begin
        AText := Text;
        if AMax < Length(AText) then begin
          Delete(AText, AMax + 1, 1);
          Text := AText;
          FSelection.StartPos := AMax;
          FSelection.EndPos := AMax;

          if (HGEFont.TextWidth(Text) <= ViewPos) then
            ViewPos := ViewPos - Width;

          Change;
        end;
      end;
    end;
  end;
end;

procedure TDxEdit.CopyText;
begin
  if (SelLength > 0) then begin
    SetClipboardText(SelText);
  end;
end;

procedure TDxEdit.CutText;
var
  AMin:Integer;
  AText:WideString;
  sText:string;
  HGEFont:THGEFont;
begin
  if (not FReadOnly) and Enabled then begin
    if (SelLength > 0) then begin
      AMin := Min(FSelection.StartPos, FSelection.EndPos);
      sText := SelText;

      SetClipboardText(sText);

      AText := Text;

      Delete(AText, AMin + 1, SelLength);
      Text := AText;

      FSelection.StartPos := AMin;
      FSelection.EndPos := AMin;
      HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);
      if HGEFont <> nil then
        ViewPos := ViewPos - HGEFont.TextWidth(sText);
      Change;
    end;
  end;
end;

procedure TDxEdit.PasteText;
var
  AText:WideString;
  AddTx:WideString;
  sText:string;
  AMin:Integer;
  HGEFont:THGEFont;
begin
  if FReadOnly or (not FAllowPaste) or (not Enabled) then Exit;
  HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);
  if HGEFont <> nil then begin
    AMin := Min(FSelection.StartPos, FSelection.EndPos);
    AText := Text;
    if (SelLength > 0) then begin
      sText := SelText;
      Delete(AText, AMin + 1, SelLength);
      Text := AText;
      HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);
      ViewPos := ViewPos - HGEFont.TextWidth(sText);
    end;
    AMin := Min(AMin, Length(AText));

    // DebugOut('PasteText:' + IntToStr(AMin)+' Length(AText):'+IntToStr(Length(AText)));

    sText := GetClipboardText;

    StripWrong(sText);

    AddTx := sText;
    // if (FInValue = vInteger) and (not IsStringNumber(AddTx)) then
    // AddTx := '0';

    Insert(AddTx, AText, AMin + 1);

    Text := AText;

    FSelection.StartPos := AMin;
    FSelection.EndPos := AMin;
    // DebugOut('PasteText FSelection.StartPos 1 :' + IntToStr(FSelection.StartPos));
  // DebugOut('PasteText FSelection.EndPos 1 :' + IntToStr(FSelection.EndPos));
    FSelection.StartPos := FSelection.StartPos + Length(AddTx);
    FSelection.EndPos := FSelection.StartPos;
    // DebugOut('PasteText FSelection.StartPos 2 :' + IntToStr(FSelection.StartPos));
     // DebugOut('PasteText FSelection.EndPos2 :' + IntToStr(FSelection.EndPos));

    ViewPos := ViewPos + HGEFont.TextWidth(AddTx);

    SetFocus;
    Change;
  end;
end;

procedure TDxEdit.SelectPos(Pt:TPoint; vRect:TRect);
var
  I, TextWidth, nLen:Integer;
  AText:WideString;
  sText:string;
  PaintRect:TRect;
  HGEFont:THGEFont;
begin
  SaveEditHistory;
  FOldText := FText;
  FOldSelection := FSelection;
  TextWidth := 0;
  FSelection.StartPos := 0;
  FSelection.EndPos := 0;
  FSelIndex := 0;
  AText := GetText;
  PaintRect := ShrinkRect(vRect, 2, 0);
  HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);

  if HGEFont <> nil then begin
    for I := 1 to Length(AText) do begin
      sText := AText[I];
      if TextWidth + HGEFont.TextWidth(sText) - FViewPos > Pt.X then begin
        nLen := TextWidth + HGEFont.TextWidth(sText) - FViewPos - Pt.X;

        if nLen > HGEFont.TextWidth(sText) div 2 then begin // 小于应该字符的一半宽度
          FSelection.StartPos := I - 1;
        end
        else begin
          FSelection.StartPos := I;
        end;
        FSelection.EndPos := FSelection.StartPos;
        FSelIndex := FSelection.StartPos;
        Exit;
      end
      else if TextWidth - FViewPos >= Pt.X then begin
        nLen := TextWidth - FViewPos - Pt.X;

        if nLen > HGEFont.TextWidth(sText) div 2 then begin // 小于应该字符的一半宽度
          FSelection.StartPos := I - 1;
        end
        else begin
          FSelection.StartPos := I;
        end;

        FSelection.EndPos := FSelection.StartPos;
        FSelIndex := FSelection.StartPos;
        Exit;
      end;
      TextWidth := TextWidth + HGEFont.TextWidth(sText); //
    end;
    FSelection.StartPos := Length(AText);
    FSelection.EndPos := FSelection.StartPos;
    FSelIndex := FSelection.StartPos;
  end;
end;

procedure TDxEdit.MoveCursor(Index:Integer);
var
  nX:Integer;
  AText:WideString;
  sText:string;
  PaintRect:TRect;
  vRect:TRect;
  HGEFont:THGEFont;
begin
  vRect := VirtualRect;
  AText := Text;
  HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);
  if HGEFont <> nil then begin
    PaintRect := ShrinkRect(vRect, 2, 0);

    if Index < 0 then Index := 0;
    if Index > Length(AText) then Index := Length(AText);

    // FSelection.StartPos := Index;
    // FSelection.EndPos := Index;

    sText := Copy(AText, 1, Index);
    nX := HGEFont.TextWidth(sText) - ViewPos;

    if (nX < 0) or (nX > (PaintRect.Right - PaintRect.Left) - HGEFont.TextWidth('0')) then begin
      if nX < 0 then begin
        ViewPos := ViewPos + nX;
      end
      else begin
        ViewPos := ViewPos + (nX - ((PaintRect.Right - PaintRect.Left) - HGEFont.TextWidth('0')));
      end;
    end;
  end;
end;

procedure TDxEdit.SelectChar(Pt:TPoint; vRect:TRect);
var
  I, TextWidth, nIndex, nLen:Integer;
  AText:WideString;
  PaintRect:TRect;
  sText:string;
  HGEFont:THGEFont;
begin
  HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);
  if HGEFont <> nil then begin
    TextWidth := 0;
    AText := GetText;
    PaintRect := ShrinkRect(vRect, 2, 0);

    if Pt.X <= 0 then
      Pt.X := 0
    else if Pt.X >= (PaintRect.Right - PaintRect.Left) then
      Pt.X := (PaintRect.Right - PaintRect.Left);

    if Pt.X >= PaintRect.Right - PaintRect.Left then begin
      ViewPos := ViewPos + (PaintRect.Right - PaintRect.Left) div 3;
    end
    else if Pt.X <= 1 then begin
      ViewPos := ViewPos - (PaintRect.Right - PaintRect.Left) div 3;
    end;

    nIndex := Length(AText);
    for I := 1 to Length(AText) do begin
      sText := AText[I];
      if TextWidth + HGEFont.TextWidth(sText) - ViewPos > Pt.X then begin

        nLen := TextWidth + HGEFont.TextWidth(sText) - ViewPos - Pt.X;

        if nLen > HGEFont.TextWidth(sText) div 2 then begin // 小于应该字符的一半宽度
          nIndex := I - 1;
        end
        else begin
          nIndex := I;
        end;

        Break;
      end
      else if TextWidth - ViewPos >= Pt.X - 1 then begin
        nLen := TextWidth - ViewPos - Pt.X;

        if nLen > HGEFont.TextWidth(sText) div 2 then begin // 小于应该字符的一半宽度
          nIndex := I - 1;
        end
        else begin
          nIndex := I;
        end;
        Break;
      end;
      TextWidth := TextWidth + HGEFont.TextWidth(sText);
    end;

    FSelection.StartPos := FSelIndex;
    FSelection.EndPos := nIndex;
  end;
end;

procedure TDxEdit.DoMouseEnter;
var
  I:Integer;
begin
  if MouseMoveed then begin
    for I := 0 to Application.MainForm.ControlCount - 1 do
      if (Application.MainForm.Controls[I] is TWinControl) and (TWinControl(Application.MainForm.Controls[I]).Handle = GameCanvas.Handle) then begin
        Application.MainForm.Controls[I].Cursor := crIBeam;
        Exit;
      end;
    Application.MainForm.Cursor := crIBeam;
  end;
  inherited;
end;

procedure TDxEdit.DoMouseLeave;
var
  I:Integer;
begin
  for I := 0 to Application.MainForm.ControlCount - 1 do
    if (Application.MainForm.Controls[I] is TWinControl) and (TWinControl(Application.MainForm.Controls[I]).Handle = GameCanvas.Handle) then begin
      Application.MainForm.Controls[I].Cursor := GetDefaultCursor;
      Exit;
    end;
  Application.MainForm.Cursor := GetDefaultCursor;
  inherited;
end;

procedure TDxEdit.DoShow();
begin
  FillChar(FWideChar, SizeOf(FWideChar), #0);
  inherited;
end;

procedure TDxEdit.DoHide();
begin
  FillChar(FWideChar, SizeOf(FWideChar), #0);
  inherited;
end;

procedure TDxEdit.DoFocused();
begin
  FillChar(FWideChar, SizeOf(FWideChar), #0);
  inherited;
end;

procedure TDxEdit.DoUnFocused();
begin
  inherited;
  if Assigned(FOnUnFocused) then
    FOnUnFocused(Self);
end;

procedure TDxEdit.MouseMove(Shift:TShiftState; X, Y:Integer);
var
  vRect:TRect;
begin
  inherited MouseMove(Shift, X, Y);
  if MouseDowned and Focused and (ssLeft in Shift) then begin
    vRect := VirtualRect;
    if abs(SpotX - X) > 0 then begin
      if FAllowSelect then begin
        SelectChar(Point(X - vRect.Left, Y - vRect.Top), vRect);
      end
      else begin
        SelectPos(Point(X - vRect.Left, Y - vRect.Top), vRect);
      end;
    end;
    SpotX := X;
    SpotY := Y;
  end;
end;

procedure TDxEdit.MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
var
  vRect:TRect;
begin
  inherited MouseDown(Button, Shift, X, Y);
  if (Button = mbLeft) then begin
    vRect := VirtualRect;
    FPoint := Point(X - vRect.Left, Y - vRect.Top);
    SelectPos(Point(X - vRect.Left, Y - vRect.Top), vRect);
  end;
end;

procedure TDxEdit.MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
begin
  inherited MouseUp(Button, Shift, X, Y);

  if Assigned(PopupMenu) and (Button = mbRight) then begin
    PopupMenu.PopupMenu := Self;
    TDxPopupMenu(PopupMenu).ItemIndex := -1;
    if TDxPopupMenu(PopupMenu).Items.Count >= 5 then begin
      if SelText = '' then begin
        TDxPopupMenu(PopupMenu).Items.Enabled[0] := False;
        TDxPopupMenu(PopupMenu).Items.Enabled[1] := False;
        TDxPopupMenu(PopupMenu).Items.Enabled[3] := False;
      end
      else begin
        TDxPopupMenu(PopupMenu).Items.Enabled[0] := True;
        TDxPopupMenu(PopupMenu).Items.Enabled[1] := True;
        TDxPopupMenu(PopupMenu).Items.Enabled[3] := True;
      end;
      TDxPopupMenu(PopupMenu).Items.Enabled[5] := Text <> '';

      if GetClipboardText <> '' then
        TDxPopupMenu(PopupMenu).Items.Enabled[2] := True
      else
        TDxPopupMenu(PopupMenu).Items.Enabled[2] := False;
    end;
    if RootCtrl <> nil then begin
      if X + PopupMenu.Width > RootCtrl.Width then
        PopupMenu.Left := X - PopupMenu.Width
      else
        PopupMenu.Left := X;
      if Y + PopupMenu.Height > RootCtrl.Height then
        PopupMenu.Top := Y - PopupMenu.Height
      else
        PopupMenu.Top := Y;
    end;
    PopupMenu.OnClick := PopupMenuClick;
    PopupMenu.Show;
  end;
end;

procedure TDxEdit.SetFocus();
begin
  inherited SetFocus();
  if Focused then begin
    Repaint;
    if Assigned(FOnEnter) then
      FOnEnter(Self);

    OpenIme;
  end;
end;

procedure TDxEdit.PopupMenuClick(Sender:TObject; X, Y:Integer);
begin
  if Assigned(PopupMenu) then
    case TDxPopupMenu(PopupMenu).ItemIndex of
      0:CutText; // 剪切
      1:CopyText; // 复制
      2:PasteText; // 粘贴
      3:BackText; // 删除
      4:; // -
      5:SelectAll; // 全选
    end;
end;

procedure TDxEdit.SelectAll;
var
  AText:WideString;
begin
  if not FAllowSelect then Exit;
  AText := Text;
  FSelection.StartPos := 0;
  FSelection.EndPos := Length(AText);
  Repaint;
end;

function TDxEdit.InRange(X, Y:Integer):Boolean;
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

procedure TDxEdit.DrawText(vtRect, vbRect:TRect);
var
  X, Y:Integer;
  AMin, AMax, ALength, nY:Integer;
  nTextLen, nDrawLen, nLen, nWidth, nHeight:Integer;
  PaintRect:TRect;
  SText:WideString;
  AText:WideString;
  BText:WideString;
  CText:WideString;
  DestRect, SrcRect:TRect;
  HGEFont:THGEFont;
  ImageInfo:TImageInfo;
begin
  SText := GetText;
  HGEFont := TextureFonts.FindFont(Font.Name, Font.Size, Font.Style);
  if HGEFont <> nil then begin
    if (FInValue = vInteger) and (SText = '') and (FHintText = '') then begin
      ImageInfo := HGEFont.GetImageInfo('0');
      if Length(ImageInfo.ImageIndexs) > 0 then begin
        nY := vtRect.Top + ((vtRect.Bottom - vtRect.Top) - ImageInfo.Height) div 2;
        nWidth := vtRect.Right - vtRect.Left;
        nHeight := vtRect.Bottom - vtRect.Top;
        DestRect := Bounds(vtRect.Left, nY, Min(ImageInfo.Width, nWidth), Min(ImageInfo.Height, nHeight));
        SrcRect := Bounds(ViewPos, 0, ImageInfo.Width, ImageInfo.Height);
        PaintRect := ReallyPaintRect(DestRect, SrcRect, vtRect, vbRect, X, Y);
        HGEFont.TextRect(X, Y, PaintRect, ImageInfo.ImageIndexs, FFont.Color);
      end;
      Exit;
    end;

    ImageInfo := HGEFont.GetImageInfo(string(SText));
    if Length(ImageInfo.ImageIndexs) > 0 then begin
      nY := vtRect.Top + ((vtRect.Bottom - vtRect.Top) - ImageInfo.Height) div 2;
      if FSelection.StartPos <> FSelection.EndPos then begin
        AMin := Min(FSelection.StartPos, FSelection.EndPos);
        AMax := Max(FSelection.StartPos, FSelection.EndPos);
        ALength := AMax - AMin;

        nDrawLen := 0;
        nWidth := vtRect.Right - vtRect.Left;
        nHeight := vtRect.Bottom - vtRect.Top;

        AText := Copy(SText, 1, AMin);
        nTextLen := HGEFont.TextWidth(string(AText));
        if nTextLen > ViewPos then begin
          DestRect := Bounds(vtRect.Left, nY, Min(nTextLen, nWidth), Min(ImageInfo.Height, nHeight));
          SrcRect := Bounds(ViewPos, 0, Min(nTextLen - ViewPos, nWidth), Min(ImageInfo.Height, nHeight));
          PaintRect := ReallyPaintRect(DestRect, SrcRect, vtRect, vbRect, X, Y);
          nLen := (PaintRect.Right - PaintRect.Left);
          nDrawLen := nLen;
          if nLen > 0 then begin
            nWidth := nWidth - nLen;
            HGEFont.TextRect(X, Y, PaintRect, ImageInfo.ImageIndexs, FFont.Color);
          end;
        end;

        BText := Copy(SText, AMin + 1, ALength);
        nTextLen := HGEFont.TextWidth(string(BText));
        DestRect := Bounds(vtRect.Left + nDrawLen, nY, Min(nTextLen, nWidth), Min(ImageInfo.Height, nHeight));
        SrcRect := Bounds(ViewPos + nDrawLen, 0, Min(nTextLen, nWidth), Min(ImageInfo.Height, nHeight));
        PaintRect := ReallyPaintRect(DestRect, SrcRect, vtRect, vbRect, X, Y);
        nLen := (PaintRect.Right - PaintRect.Left);
        nDrawLen := nDrawLen + nLen;
        if nLen > 0 then begin
          nWidth := nWidth - nLen;
          GameCanvas.FillRect(DestRect, FSelBackColor);
          HGEFont.TextRect(X, Y, PaintRect, ImageInfo.ImageIndexs, FSelFontColor);
        end;

        if nWidth > 0 then begin
          CText := Copy(SText, AMax + 1, Length(SText));
          nTextLen := HGEFont.TextWidth(string(CText));
          if nTextLen > 0 then begin
            DestRect := Bounds(vtRect.Left + nDrawLen, nY, Min(nTextLen, nWidth), Min(ImageInfo.Height, nHeight));
            SrcRect := Bounds(ViewPos + nDrawLen, 0, Min(nTextLen, nWidth), Min(ImageInfo.Height, nHeight));
            PaintRect := ReallyPaintRect(DestRect, SrcRect, vtRect, vbRect, X, Y);
            nLen := PaintRect.Right - PaintRect.Left;
            if nLen > 0 then begin
              HGEFont.TextRect(X, Y, PaintRect, ImageInfo.ImageIndexs, FFont.Color);
            end;
          end;
        end;
      end
      else begin
        nWidth := vtRect.Right - vtRect.Left;
        nHeight := vtRect.Bottom - vtRect.Top;
        DestRect := Bounds(vtRect.Left, nY, Min(ImageInfo.Width, nWidth), Min(ImageInfo.Height, nHeight));
        SrcRect := Bounds(ViewPos, 0, ImageInfo.Width, ImageInfo.Height);
        PaintRect := ReallyPaintRect(DestRect, SrcRect, vtRect, vbRect, X, Y);
        HGEFont.TextRect(X, Y, PaintRect, ImageInfo.ImageIndexs, FFont.Color);
      end;
    end;
  end;
end;

procedure TDxEdit.DrawHintText(vtRect, vbRect:TRect);
var
  X, Y:Integer;
  nY:Integer;
  nWidth, nHeight:Integer;
  PaintRect:TRect;
  SText:WideString;
  DestRect, SrcRect:TRect;
  HGEFont:THGEFont;
  ImageInfo:TImageInfo;
begin
  if Length(Text) > 0 then Exit;
  SText := FHintText;
  HGEFont := TextureFonts.FindFont(FHintTextFont.Name, FHintTextFont.Size, FHintTextFont.Style);
  if HGEFont <> nil then begin
    ImageInfo := HGEFont.GetImageInfo(string(SText));
    if Length(ImageInfo.ImageIndexs) > 0 then begin
      nY := vtRect.Top + ((vtRect.Bottom - vtRect.Top) - ImageInfo.Height) div 2;

      if FHintTextAlignment = taLeftJustify then begin
        nWidth := vtRect.Right - vtRect.Left;
        nHeight := vtRect.Bottom - vtRect.Top;
        DestRect := Bounds(vtRect.Left, nY, Min(ImageInfo.Width, nWidth), Min(ImageInfo.Height, nHeight));
        SrcRect := Bounds(ViewPos, 0, ImageInfo.Width, ImageInfo.Height);
        PaintRect := ReallyPaintRect(DestRect, SrcRect, vtRect, vbRect, X, Y);
        HGEFont.TextRect(X, Y, PaintRect, ImageInfo.ImageIndexs, FHintTextFont.Color);
      end
      else if FHintTextAlignment = taCenter then begin
        nWidth := Min(ImageInfo.Width, vtRect.Right - vtRect.Left);
        nHeight := vtRect.Bottom - vtRect.Top;
        DestRect := Bounds(vtRect.Left + (vtRect.Right - vtRect.Left - nWidth) div 2, nY, nWidth, Min(ImageInfo.Height, nHeight));
        SrcRect := Bounds(ViewPos, 0, ImageInfo.Width, ImageInfo.Height);
        PaintRect := ReallyPaintRect(DestRect, SrcRect, vtRect, vbRect, X, Y);
        HGEFont.TextRect(X, Y, PaintRect, ImageInfo.ImageIndexs, FHintTextFont.Color);
      end
      else if FHintTextAlignment = taRightJustify then begin
        nWidth := Min(ImageInfo.Width, vtRect.Right - vtRect.Left);
        nHeight := vtRect.Bottom - vtRect.Top;
        DestRect := Bounds(vtRect.Right - nWidth, nY, nWidth, Min(ImageInfo.Height, nHeight));
        SrcRect := Bounds(ViewPos, 0, ImageInfo.Width, ImageInfo.Height);
        PaintRect := ReallyPaintRect(DestRect, SrcRect, vtRect, vbRect, X, Y);
        HGEFont.TextRect(X, Y, PaintRect, ImageInfo.ImageIndexs, FHintTextFont.Color);
      end
    end;
  end;
end;

procedure TDxEdit.DrawSelector(vtRect, vbRect:TRect);
var
  TextWidth:Integer;
  PaintRect:TRect;
  AText:WideString;
  sText:string;
  HGEFont:THGEFont;
begin
  AText := Text;
  sText := Copy(AText, 1, FSelection.EndPos);
  HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);
  if HGEFont <> nil then
    TextWidth := HGEFont.TextWidth(sText);
  PaintRect := Bounds(vtRect.Left + (TextWidth - ViewPos), vtRect.Top, 1, vtRect.Bottom - vtRect.Top);
  FillRect(PaintRect, vtRect, vbRect, SelectedColor);
  // GameCanvas.FillRect(PaintRect, SelectedColor);
end;

procedure TDxEdit.DoUpdate();
begin
  inherited;
end;

procedure TDxEdit.Paint;
var
  AFont:TDxFont;
  vtRect:TRect;
  vbRect:TRect;
  PaintRect:TRect;
begin
  vbRect := VisibleRect;
  if (vbRect.Bottom <= vbRect.Top) or (vbRect.Right <= vbRect.Left) then Exit;
  vtRect := VirtualRect;
  DoPaint();

  if not Transparent then begin
    if not Enabled then
      GameCanvas.FillRect(vbRect, FDisableBackgroundColor)
    else
      GameCanvas.FillRect(vbRect, BackgroundColor);
  end;

  if Assigned(OnStartPaint) then
    OnStartPaint(Self);

  if Assigned(OnPaint) then
    OnPaint(Self);

  if DrawBorder then begin
    if Enabled then begin
      if MouseDowned then
        AFont := BorderColor.Down
      else if MouseMoveed then
        AFont := BorderColor.Hot
      else
        AFont := BorderColor.Up;
    end
    else
      AFont := BorderColor.Disabled;
    PaintRect := vtRect;
    FrameRect(PaintRect, vtRect, vbRect, AFont.Color);
    if AFont.Bold then begin
      PaintRect := ShrinkRect(PaintRect, 1, 1);
      FrameRect(PaintRect, vtRect, vbRect, AFont.Color);
    end;
  end;

  PaintRect := ShrinkRect(vtRect, 2, 0);

  DrawText(PaintRect, ShortRect(PaintRect, vbRect));

  if Focused and Enabled then begin
    Inc(Ticks);
    if (FBlinkTicks = 0) or ((Ticks div FBlinkTicks) and $01 = 0) then
      DrawSelector(ShrinkRect(vtRect, 2, 1), ShortRect(ShrinkRect(vtRect, 2, 1), vbRect));
  end
  else begin
    if not (Enabled and (PopupMenu <> nil) and PopupMenu.Visible and PopupMenu.Focused and (PopupMenu.PopupMenu = Self)) then begin
      if FSelection.StartPos < FSelection.EndPos then
        FSelection.StartPos := FSelection.EndPos
      else
        FSelection.EndPos := FSelection.StartPos;
    end;

    if Length(FHintText) > 0 then begin
      DrawHintText(PaintRect, ShortRect(PaintRect, vbRect));
    end;
  end;
  if Assigned(OnStopPaint) then
    OnStopPaint(Self);
end;

end.
