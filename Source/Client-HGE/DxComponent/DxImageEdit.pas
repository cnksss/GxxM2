unit DxImageEdit;

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
  DxPopupMenu,
  dxMemo,
  GameImages;

type
  TEditHistory = record
    Text:string;
    Selection:TSelection;
  end;
  pTEditHistory = ^TEditHistory;

  TDxImageEdit = class;

  TDxBackgroundImage = class(TInterfacedPersistent)
  private
    FOwner:TDxImageEdit;
    FOnChange:TNotifyEvent;
    FOnGetImage:TOnGetImage;
    FImageType:TImageType; // 使用WIL类型
    FImage:TGameImages;

    FImageIndex:Integer; // 图片序号
    FOffsetX:Integer;
    FOffsetY:Integer;
    FBlendDraw:Boolean;
    FOutsideAreaDraw:Boolean;

    procedure SetOnGetImage(const Value:TOnGetImage);
    procedure SetImageType(const Value:TImageType);
    procedure SetOffsetX(const Value:Integer);
    procedure SetOffsetY(const Value:Integer);
    procedure SetImageIndex(const Value:Integer);
    procedure SetBlendDraw(const Value:Boolean);
    procedure SetOutsideAreaDraw(const Value:Boolean);

    procedure Changed;
    procedure Paint;
  public
    constructor Create(AOnwer:TDxImageEdit);
    destructor Destroy; override;
    procedure Assign(Source:TPersistent); override;
    property Image:TGameImages read FImage;
    property OnChange:TNotifyEvent read FOnChange write FOnChange;
    property OnGetImage:TOnGetImage read FOnGetImage write SetOnGetImage;
  published
    property ImageType:TImageType read FImageType write SetImageType;

    property ImageIndex:Integer read FImageIndex write SetImageIndex;
    property OffsetX:Integer read FOffsetX write SetOffsetX;
    property OffsetY:Integer read FOffsetY write SetOffsetY;
    property BlendDraw:Boolean read FBlendDraw write SetBlendDraw;
    property OutsideAreaDraw:Boolean read FOutsideAreaDraw write SetOutsideAreaDraw;
  end;

  TDxImageEdit = class(TDxControl)
  private
    Ticks:Integer;
    FBlinkTicks:Integer;
    FReadOnly:Boolean;
    FSelection:TSelection;
    FOnChange:TNotifyEvent;
    FOnEnter:TNotifyEvent;

    FWideChar:array[0..1] of Char;

    FTokens:TStringLineEx;

    FAllowSelect:Boolean;
    FAllowPaste:Boolean;
    FPasswordChar:Char;
    FViewPos:Integer;

    FSelectedColor:TColor;
    FSelBackColor:TColor;
    FSelFontColor:TColor;
    FInValue:TInValue;
    FFont:TDxFont;

    FBackgroundColorAlpha:Byte;
    FBackgroundImage:TDxBackgroundImage;

    FDisableHideCtrl:Boolean;
    FDisableBackgroundTransparent:Boolean;
    FDisableBackgroundColor:TColor;
    FDisableBackgroundAlpha:Byte;
    FDisableBackgroundImage:TDxBackgroundImage;

    FHintTextFont:TDxFont;
    FHintText:string;
    FHintTextAlignment:TAlignment;

    FMaxLength:Integer;

    FSelIndex:Integer;
    FPoint:TPoint;

    FOldSelection:TSelection;
    FEditHistorys:array of TEditHistory;

    procedure EnterKey(var Key:Word);
    procedure Change;
    procedure SetText(Value:string);
    //procedure SetSelText(Value: string);
    procedure SetSelStart(Value:Integer);
    procedure SetMaxLength(Value:Integer);
    procedure SetSelLength(Value:Integer);
    function GetValue:Integer;
    procedure SetValue(Value:Integer);
    function GetSelText:string;
    function GetSelStart:Integer;
    function GetSelLength:Integer;
    function GetNextEdit:TDxImageEdit;
    function GetMaxTabOrder:Integer;
    procedure SetViewPos(const Value:Integer);
    function GetText:string;
    function GetText2:string;
    procedure SelectPos(Pt:TPoint; vRect:TRect);
    procedure SelectChar(Pt:TPoint; vRect:TRect);
    procedure PopupMenuClick(Sender:TObject; X, Y:Integer); stdcall;
    procedure DrawText(vtRect, vbRect:TRect);
    procedure DrawHintText(vtRect, vbRect:TRect);
    procedure DrawSelector(vtRect, vbRect:TRect);
    procedure MoveCursor(Index:Integer);
    procedure SaveEditHistory; overload;
    procedure RecoveryEditHistory;

    function SelectionPosToRealPos(Pos:Integer):Integer;
    function GetBeforePosItem(Pos:Integer):Integer;
    function GetPosInItem(Pos:Integer):Integer;
  protected
    procedure SetOnGetImage(Value:TOnGetImage); override;
    procedure DoUpdate(); override;
    procedure DoMouseEnter; override;
    procedure DoMouseLeave; override;

    procedure DoShow(); override;
    procedure DoHide(); override;
    procedure DoFocused(); override;

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
    property SelText:string read GetSelText; // write SetSelText;
    property BlinkTicks:Integer read FBlinkTicks write FBlinkTicks;
    property PopupMenu;
    property OnChange:TNotifyEvent read FOnChange write FOnChange;
    property OnEnter:TNotifyEvent read FOnEnter write FOnEnter;
  published
    // 编辑状态
    property BackgroundColor;
    property BackgroundColorAlpha:Byte read FBackgroundColorAlpha write FBackgroundColorAlpha;
    property BackgroundImage:TDxBackgroundImage read FBackgroundImage write FBackgroundImage;

    property DisableHideCtrl:Boolean read FDisableHideCtrl write FDisableHideCtrl;
    property DisableBackgroundTransparent:Boolean read FDisableBackgroundTransparent write FDisableBackgroundTransparent;
    property DisableBackgroundColor:TColor read FDisableBackgroundColor write FDisableBackgroundColor;
    property DisableBackgroundAlpha:Byte read FDisableBackgroundAlpha write FDisableBackgroundAlpha;
    property DisableBackgroundImage:TDxBackgroundImage read FDisableBackgroundImage write FDisableBackgroundImage;

    property HintTextFont:TDxFont read FHintTextFont write FHintTextFont;
    property HintText:string read FHintText write FHintText;
    property HintTextAlignment:TAlignment read FHintTextAlignment write FHintTextAlignment;

    property DrawBorder;
    property Font:TDxFont read FFont write FFont;
    property Text:string read GetText write SetText;
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

{---------------------------------------------------------------------------------}
{ TDxBackgroundImage }

procedure TDxBackgroundImage.Assign(Source:TPersistent);
begin
  if Source is TDxBackgroundImage then begin
    OnGetImage := TDxBackgroundImage(Source).OnGetImage;
    ImageType := TDxBackgroundImage(Source).ImageType;

    FImageIndex := TDxBackgroundImage(Source).FImageIndex; // 图片序号
    FOffsetX := TDxBackgroundImage(Source).FOffsetX;
    FOffsetY := TDxBackgroundImage(Source).FOffsetY;
    FBlendDraw := TDxBackgroundImage(Source).FBlendDraw;
    FOutsideAreaDraw := TDxBackgroundImage(Source).FOutsideAreaDraw;
    Changed;
  end;
end;

procedure TDxBackgroundImage.Changed;
begin
  if Assigned(FOnChange) then
    FOnChange(Self);
end;

constructor TDxBackgroundImage.Create(AOnwer:TDxImageEdit);
begin
  FOwner := AOnwer;
  FImageType := Prguse_wil; // 图库

  FImageIndex := -1; // 图片序号
  FOffsetX := 0;
  FOffsetY := 0;
  FBlendDraw := False;
  FOutsideAreaDraw := False;
end;

destructor TDxBackgroundImage.Destroy;
begin

  inherited;
end;

procedure TDxBackgroundImage.Paint;
var
  D:TTexture;
  nX, nY:Integer;
  R, SrcRect, ParentRect:TRect;
  BlendMode:Integer;
begin
  if FImage = nil then Exit;
  if (FImageIndex < 0) then Exit;

  D := FImage.GetCachedImage(FImageIndex, nX, nY);
  if D <> nil then begin
    ParentRect := FOwner.VirtualRect;
    SrcRect := D.ClientRect;

    R.Left := ParentRect.Left + FOffsetX;
    R.Top := ParentRect.Top + FOffsetY;

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

procedure TDxBackgroundImage.SetImageType(const Value:TImageType);
begin
  if FImageType <> Value then begin
    FImageType := Value;
    if Assigned(FOnGetImage) then
      FOnGetImage(Self, FImageType, TObject(FImage));
    Changed;
  end;
end;

procedure TDxBackgroundImage.SetOffsetX(const Value:Integer);
begin
  FOffsetX := Value;
end;

procedure TDxBackgroundImage.SetOffsetY(const Value:Integer);
begin
  FOffsetY := Value;
end;

procedure TDxBackgroundImage.SetOnGetImage(const Value:TOnGetImage);
begin
  FOnGetImage := Value;
  if Assigned(FOnGetImage) then
    FOnGetImage(Self, FImageType, TObject(FImage));
  Changed;
end;

procedure TDxBackgroundImage.SetImageIndex(const Value:Integer);
begin
  FImageIndex := Value;
end;

procedure TDxBackgroundImage.SetBlendDraw(const Value:Boolean);
begin
  FBlendDraw := Value;
end;

procedure TDxBackgroundImage.SetOutsideAreaDraw(const Value:Boolean);
begin
  if FOutsideAreaDraw <> Value then
    FOutsideAreaDraw := Value;
end;

{---------------------------------------------------------------------------------}

constructor TDxImageEdit.Create(AOwner:{$IF  CLIENTEXE = 1}TDxControl{$ELSE}TComponent{$IFEND});
begin
  inherited Create(AOwner);
  Transparent := False;
  EnableFocus := True;

  Width := 100;
  Height := 20;

  FOnChange := nil;
  FOnEnter := nil;
  FInValue := vString;

  Ticks := 0;

  FTokens := TStringLineEx.Create;

  FReadOnly := False;
  FMaxLength := 0;

  FillChar(FWideChar, SizeOf(FWideChar), #0);

  FFont := TDxFont.Create;
  FFont.Color := clWhite;
  FFont.Bold := False;

  BackgroundColor := clBlack;
  FBackgroundColorAlpha := 255;
  FBackgroundImage := TDxBackgroundImage.Create(Self);

  FDisableHideCtrl := False;
  FDisableBackgroundTransparent := False;
  FDisableBackgroundColor := clBlack;
  FDisableBackgroundAlpha := 255;
  FDisableBackgroundImage := TDxBackgroundImage.Create(Self);

  FHintTextFont := TDxFont.Create;
  FHintTextFont.Color := clWhite;
  FHintTextFont.Bold := False;
  FHintText := '';
  FHintTextAlignment := taLeftJustify;

  DrawBorder := True;

  FSelectedColor := clWhite;
  FSelBackColor := clBlue;
  FSelFontColor := clWhite;

  FBlinkTicks := 20;

  FAllowSelect := True;
  FAllowPaste := False;

  FSelection.StartPos := 0;
  FSelection.EndPos := 0;

  FPasswordChar := #0;

  TabOrder := GetMaxTabOrder + 1;

  FEditHistorys := nil;
  FOldSelection := FSelection;
end;

destructor TDxImageEdit.Destroy();
begin
  FFont.Free;
  FHintTextFont.Free;
  SetLength(FEditHistorys, 0);
  FEditHistorys := nil;
  FTokens.Free;
  FBackgroundImage.Free;
  FDisableBackgroundImage.Free;
  inherited Destroy();
end;

// ------------------------------------------------------------------------------

function TDxImageEdit.GetMaxTabOrder:Integer;
var
  I, nTabOrder:Integer;
  DxEdit:TDxImageEdit;
begin
  nTabOrder := 0;
  if Owner <> nil then begin
    for I := 0 to TDxControl(Owner).ControlCount - 1 do begin
      if (TDxControl(Owner).Control[I] is TDxImageEdit) then begin
        DxEdit := TDxImageEdit(TDxControl(Owner).Control[I]);
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

function TDxImageEdit.GetNextEdit:TDxImageEdit;
var
  I:Integer;
  ControlList:TStringList;
  DxEdit:TDxImageEdit;
begin
  Result := Self;
  if Owner <> nil then begin
    ControlList := TStringList.Create;
    for I := 0 to TDxControl(Owner).ControlCount - 1 do begin
      if (TDxControl(Owner).Control[I] is TDxImageEdit) then begin
        DxEdit := TDxImageEdit(TDxControl(Owner).Control[I]);
        if DxEdit.Visible then
          ControlList.AddObject(IntToStr(DxEdit.TabOrder), DxEdit);
      end;
    end;
    ControlList.CustomSort(NumberSort_2);

    for I := 0 to ControlList.Count - 1 do begin
      if ControlList.Objects[I] = Self then begin
        if I >= ControlList.Count - 1 then begin
          Result := TDxImageEdit(ControlList.Objects[0]);
        end
        else begin
          Result := TDxImageEdit(ControlList.Objects[I + 1]);
        end;
        break;
      end;
    end;
    ControlList.Free;
  end;
end;

// ------------------------------------------------------------------------------

procedure TDxImageEdit.SetViewPos(const Value:Integer);
var
  PaintRect:TRect;
  HGEFont:THGEFont;
begin
  HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);
  if HGEFont <> nil then begin
    FViewPos := Value;
    PaintRect := ShrinkRect(VirtualRect, 2, 0);

    if HGEFont.TextWidth(GetText2) < (PaintRect.Right - PaintRect.Left) then
      FViewPos := 0;

    if HGEFont.TextWidth(GetText2) - FViewPos < (PaintRect.Right - PaintRect.Left) - HGEFont.TextWidth('0') then
      FViewPos := HGEFont.TextWidth(GetText2) - ((PaintRect.Right - PaintRect.Left) - HGEFont.TextWidth('0'));

    if (FViewPos < 0) then FViewPos := 0;
  end;
end;

procedure TDxImageEdit.Change;
begin
  Repaint;
  if (Assigned(FOnChange)) then FOnChange(Self);
end;

function TDxImageEdit.GetSelLength:Integer;
var
  AText:WideString;
  AMin, AMax:Integer;
begin
  AText := Text;
  AMin := Min(FSelection.StartPos, FSelection.EndPos);
  AMax := Max(FSelection.StartPos, FSelection.EndPos);
  Result := Max(AMax - AMin, 0);
end;

procedure TDxImageEdit.SetSelLength(Value:Integer);
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

procedure TDxImageEdit.SetSelStart(Value:Integer);
var
  S:WideString;
begin
  SaveEditHistory;
  if Value < 1 then
    Value := 1
  else begin
    S := GetText2;
    if Value > Length(S) then
      Value := Length(S);
  end;
  if not ((Value < 1) and (Value > Length(Text))) then begin
    FSelection.StartPos := Value;
    FSelection.EndPos := FSelection.StartPos;
  end;
end;

function TDxImageEdit.GetSelStart:Integer;
begin
  Result := Min(FSelection.StartPos, FSelection.EndPos);
end;

function TDxImageEdit.GetSelText:string;
var
  AMin, AMax, ALength:Integer;
  AText:WideString;
begin
  AMin := Min(FSelection.StartPos, FSelection.EndPos);
  AMax := Max(FSelection.StartPos, FSelection.EndPos);
  ALength := AMax - AMin;
  AText := GetText2;
  Result := Copy(AText, AMin + 1, ALength);
end;

procedure TDxImageEdit.SetMaxLength(Value:Integer);
var
  S:string;
begin
  if FMaxLength <> Value then begin
    FMaxLength := Value;
    if (FMaxLength < Length(Text)) and (FMaxLength > 0) then begin
      S := Text;
      S := CopyEx(S, 1, FMaxLength);
      FTokens.Clear;
      GetTextListEx(S, Font.Color, BackgroundColor, FTokens);
    end;
  end;
end;

// ---------------------------------------------------------------------------

function TDxImageEdit.GetValue:Integer;
begin
  Result := StrToIntDef(Text, 0);
end;

procedure TDxImageEdit.SetValue(Value:Integer);
begin
  if (Value = 0) and (FInValue = vInteger) then
    Text := ''
  else
    Text := IntToStr(Value);
end;

function TDxImageEdit.GetText:string;
var
  I:Integer;
  Token:PStringToken;
begin
  Result := '';
  for I := 0 to FTokens.Count - 1 do begin
    Token := FTokens.Tokens[I];
    if Token.TokenType = tt_Item then
      Result := Result + Format('{%s/%d}', [Token.Text, Token.Flag])
    else
      Result := Result + Token.Text;
  end;

  {
  if FPasswordChar <> #0 then
  begin
    for I := 1 to Length(Result) do
    begin
      Result[I] := FPasswordChar;
    end;
  end;
  }
end;

function TDxImageEdit.GetText2:string;
var
  I:Integer;
  Token:PStringToken;
begin
  Result := '';

  for I := 0 to FTokens.Count - 1 do begin
    Token := FTokens.Tokens[I];
    Result := Result + Token.Text;
  end;

  if FPasswordChar <> #0 then begin
    for I := 1 to Length(Result) do begin
      Result[I] := FPasswordChar;
    end;
  end;
end;

(*
procedure TDxImageEdit.SetSelText(Value: string);
var
  AText: WideString;
  AMin, AMax, ALength: Integer;
begin
  SaveEditHistory;
  StripWrong(Value);
  AText := Text;

  AMin := Min(FSelection.StartPos, FSelection.EndPos);
  AMax := Max(FSelection.StartPos, FSelection.EndPos);

  ALength := AMax - AMin;

  // Insert Key
  if ALength > 0 then
    Delete(AText, AMin + 1, ALength);

  Inc(AMin);
  Insert(Value, AText, AMin + 1);

  FTokens.Clear;
  GetTextListEx(AText, Font.Color, BackgroundColor, FTokens);;

  if AMin > Length(Text) then
    AMin := Length(Text);

  FSelection.StartPos := AMin;
  FSelection.EndPos := AMin + Length(Value);
end;
*)

procedure TDxImageEdit.RecoveryEditHistory;
var
  EditHistory:pTEditHistory;
  S:string;
begin
  if Length(FEditHistorys) > 0 then begin
    EditHistory := @FEditHistorys[Length(FEditHistorys) - 1];
    S := EditHistory.Text;

    FTokens.Clear;
    GetTextListEx(S, Font.Color, self.BackgroundColor, FTokens);

    FSelection := EditHistory.Selection;
    MoveCursor(FSelection.EndPos);
    SetLength(FEditHistorys, Length(FEditHistorys) - 1);
  end;
end;

function TDxImageEdit.SelectionPosToRealPos(Pos:Integer):Integer;
var
  I, Index, StartPos, RealPos, TokenPos:Integer;
  S:WideString;
begin
  if Pos <= 0 then
    Result := 0
  else begin
    if Pos > Length(WideString(GetText2)) then
      Pos := Length(WideString(GetText2));
    Result := 0;
    StartPos := 0;
    TokenPos := Pos;
    Index := -1;
    for I := 0 to FTokens.Count - 1 do begin
      S := FTokens[I].Text;
      StartPos := StartPos + Length(S);
      if StartPos = Pos then begin
        Index := I + 1;
        TokenPos := 0;
        Break;
      end
      else if StartPos > Pos then begin
        Index := I;
        Break;
      end
      else begin
        Dec(TokenPos, Length(S));
      end;
    end;

    RealPos := 0;
    if Index >= 0 then begin
      for I := 0 to Index - 1 do begin
        if FTokens[I].TokenType = tt_Item then
          S := Format('{%s/%d}', [FTokens[I].Text, FTokens[I].Flag])
        else
          S := FTokens[I].Text;
        RealPos := RealPos + Length(S);
      end;

      RealPos := RealPos + TokenPos;
      Result := RealPos;
    end;
  end;
end;

function TDxImageEdit.GetBeforePosItem(Pos:Integer):Integer;
var
  I, StartPos:Integer;
  S:WideString;
begin
  Result := -1;
  if Pos > 0 then begin
    StartPos := 0;
    for I := 0 to FTokens.Count - 1 do begin
      S := FTokens[I].Text;
      StartPos := StartPos + Length(S);
      if StartPos = Pos then begin
        if FTokens[I].TokenType = tt_item then
          Result := I;
        Break;
      end
      else if StartPos > Pos then begin
        Break;
      end;
    end;
  end;
end;

function TDxImageEdit.GetPosInItem(Pos:Integer):Integer;
var
  I, StartPos:Integer;
  S:WideString;
begin
  Result := -1;
  if Pos > 0 then begin
    StartPos := 0;
    for I := 0 to FTokens.Count - 1 do begin
      S := FTokens[I].Text;
      StartPos := StartPos + Length(S);
      if StartPos = Pos then begin
        if FTokens[I].TokenType = tt_item then
          Result := I + 1;
        Break;
      end
      else if StartPos > Pos then begin
        if FTokens[I].TokenType = tt_item then
          Result := I;
        Break;
      end;
    end;
  end;
end;

procedure TDxImageEdit.SaveEditHistory;
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

procedure TDxImageEdit.SetText(Value:string);
var
  PaintRect:TRect;
  HGEFont:THGEFont;
begin
  SaveEditHistory;
  StripWrong(Value);
  if (FMaxLength > 0) and (FMaxLength < Length(Value)) then begin
    Value := CopyEx(Value, 1, FMaxLength);
  end;

  FTokens.Clear;
  GetTextListEx(Value, Font.Color, BackgroundColor, FTokens);

  FSelection.StartPos := 0; // Length(AText);
  FSelection.EndPos := 0; // Length(AText);

  HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);
  if HGEFont <> nil then begin
    PaintRect := ShrinkRect(VirtualRect, 2, 0);

    if HGEFont.TextWidth(GetText2) < (PaintRect.Right - PaintRect.Left) then
      FViewPos := 0;
  end;
end;

procedure TDxImageEdit.KeyPress(var Key:Char);
var
  AText:WideString;
  sText:string;
  sData:string;
  AMin, AMax, ARealMin, ARealMax, ALength:Integer;
  DxEdit:TDxImageEdit;

  HGEFont:THGEFont;
begin
  inherited KeyPress(Key);
  if (FReadOnly) or (not Enabled) then Exit;

  AText := Text;

  AMin := Min(FSelection.StartPos, FSelection.EndPos);
  AMax := Max(FSelection.StartPos, FSelection.EndPos);

  ARealMin := SelectionPosToRealPos(AMin);
  ARealMax := SelectionPosToRealPos(AMax);

  ALength := ARealMax - ARealMin;
  // Insert Key
  if (Key > #31) then begin
    if (ALength > 0) then begin
      sText := SelText;
      Delete(AText, ARealMin + 1, ALength);
      Text := AText;

      FSelection.StartPos := AMin;
      FSelection.EndPos := AMin;
      HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);
      if HGEFont <> nil then
        ViewPos := ViewPos - HGEFont.TextWidth(sText);
      Change;
    end;

    sData := '';

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
            Insert(sData, AText, ARealMin + 1);

            Text := AText;

            HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);
            if HGEFont <> nil then
              ViewPos := ViewPos + HGEFont.TextWidth(sData);

            Change;
          end;
        vInteger:begin
            if IsStringNumber(sData) then begin
              Inc(AMin);
              Insert(sData, AText, ARealMin + 1);

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

procedure TDxImageEdit.EnterKey(var Key:Word);
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

procedure TDxImageEdit.KeyDown(var Key:Word; Shift:TShiftState);
var
  AText:WideString;
  I, Index, nLen:Integer;
begin
  inherited KeyDown(Key, Shift);
  if (FReadOnly) or (not Enabled) then Exit;

  // DebugOut('KeyDown:'+IntToStr(Key));
  case Key of
    VK_RIGHT:begin
        AText := GetText2;
        if FAllowSelect and (ssShift in Shift) then begin
          if FSelection.EndPos < Length(AText) then begin
            Index := GetPosInItem(FSelection.EndPos + 1);
            if (Index >= 0) and (Index < FTokens.Count) then begin
              nLen := 0;
              for I := 0 to Index do
                nLen := nLen + Length(FTokens[I].Text);
              FSelection.EndPos := nLen;
            end
            else
              Inc(FSelection.EndPos);
            MoveCursor(FSelection.EndPos);
          end;
        end
        else begin
          if FSelection.EndPos < Length(AText) then begin
            Index := GetPosInItem(FSelection.EndPos + 1);
            if (Index >= 0) and (Index < FTokens.Count) then begin
              nLen := 0;
              for I := 0 to Index do
                nLen := nLen + Length(FTokens[I].Text);
              FSelection.EndPos := nLen;
            end
            else
              Inc(FSelection.EndPos);
          end;

          FSelection.StartPos := FSelection.EndPos;
          MoveCursor(FSelection.EndPos);
        end;
      end;
    VK_LEFT:begin
        if FAllowSelect and (ssShift in Shift) then begin
          if FSelection.EndPos > 0 then begin
            Index := GetPosInItem(FSelection.EndPos - 1);
            if (Index >= 0) and (Index < FTokens.Count) then begin
              nLen := 0;
              for I := 0 to Index - 1 do
                nLen := nLen + Length(FTokens[I].Text);
              FSelection.EndPos := nLen;
            end
            else
              Dec(FSelection.EndPos);

            MoveCursor(FSelection.EndPos);
          end;
        end
        else begin
          if FSelection.EndPos > 0 then begin
            Index := GetPosInItem(FSelection.EndPos - 1);
            if (Index >= 0) and (Index < FTokens.Count) then begin
              nLen := 0;
              for I := 0 to Index - 1 do
                nLen := nLen + Length(FTokens[I].Text);
              FSelection.EndPos := nLen;
            end
            else
              Dec(FSelection.EndPos);
          end;

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
        AText := GetText2;
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

procedure TDxImageEdit.BackText;
var
  AMin, AMax, ARealMin, ARealMax, Index, TokenLen:Integer;
  AText:WideString;
  sText:string;
  HGEFont:THGEFont;
begin
  if (not FReadOnly) and Enabled then begin
    HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);
    if HGEFont <> nil then begin
      AMin := Min(FSelection.StartPos, FSelection.EndPos);
      AMax := Max(FSelection.StartPos, FSelection.EndPos);

      ARealMin := SelectionPosToRealPos(AMin);
      ARealMax := SelectionPosToRealPos(AMax);

      if (ARealMax - ARealMin > 0) then begin
        sText := SelText;
        AText := Text;
        Delete(AText, ARealMin + 1, ARealMax - ARealMin);
        Text := AText;

        FSelection.StartPos := AMin;
        FSelection.EndPos := AMin;

        ViewPos := ViewPos - HGEFont.TextWidth(sText);
        Change;
      end
      else begin
        if AMin > 0 then begin
          Index := GetBeforePosItem(AMin);
          if (Index >= 0) and (Index < FTokens.Count) then begin
            TokenLen := Length(FTokens[Index].Text);
            FTokens.Delete(Index);

            Dec(AMin, TokenLen);
            FSelection.StartPos := AMin;
            FSelection.EndPos := AMin;
          end
          else begin
            AText := Text;

            Delete(AText, ARealMin, 1);
            Dec(AMin);
            Text := AText;

            FSelection.StartPos := AMin;
            FSelection.EndPos := AMin;
          end;
          if (HGEFont.TextWidth(Text) <= ViewPos) then
            ViewPos := ViewPos - Width;

          Change;
        end;
      end;
    end;
  end;
end;

procedure TDxImageEdit.DeleteText;
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

procedure TDxImageEdit.CopyText;
begin
  if (SelLength > 0) then begin
    SetClipboardText(SelText);
  end;
end;

procedure TDxImageEdit.CutText;
var
  AMin, AMax, AMinReal, AMaxReal:Integer;
  AText:WideString;
  sText:string;
  HGEFont:THGEFont;
begin
  if (not FReadOnly) and Enabled then begin
    if (SelLength > 0) then begin
      AMin := Min(FSelection.StartPos, FSelection.EndPos);
      AMax := Max(FSelection.StartPos, FSelection.EndPos);

      AMinReal := SelectionPosToRealPos(AMin);
      AMaxReal := SelectionPosToRealPos(AMax);

      if AMaxReal - AMinReal > 0 then begin
        sText := SelText;

        SetClipboardText(sText);

        AText := Text;

        Delete(AText, AMinReal + 1, AMaxReal - AMinReal);
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
end;

procedure TDxImageEdit.PasteText;
var
  AText:WideString;
  AddTx:WideString;
  sText:string;
  AMin, AMax, AMinReal, AMaxReal:Integer;
  HGEFont:THGEFont;
begin
  if FReadOnly or (not FAllowPaste) or (not Enabled) then Exit;
  HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);
  if HGEFont <> nil then begin
    AMin := Min(FSelection.StartPos, FSelection.EndPos);
    AMax := Max(FSelection.StartPos, FSelection.EndPos);
    AMinReal := SelectionPosToRealPos(AMin);
    AMaxReal := SelectionPosToRealPos(AMax);
    AText := Text;
    if (AMaxReal - AMinReal > 0) then begin
      sText := SelText;
      Delete(AText, AMinReal + 1, AMaxReal - AMinReal);
      Text := AText;
      HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);
      ViewPos := ViewPos - HGEFont.TextWidth(sText);
    end;
    AMin := Min(AMin, Length(AText));

    sText := GetClipboardText;

    StripWrong(sText);

    AddTx := sText;

    Insert(AddTx, AText, AMinReal + 1);

    Text := AText;

    FSelection.StartPos := AMin;
    FSelection.EndPos := AMin;
    FSelection.StartPos := FSelection.StartPos + Length(AddTx);
    FSelection.EndPos := FSelection.StartPos;

    ViewPos := ViewPos + HGEFont.TextWidth(AddTx);

    SetFocus;
    Change;
  end;
end;

procedure TDxImageEdit.SelectPos(Pt:TPoint; vRect:TRect);
var
  I, J, TextWidth, nLen, Index:Integer;
  AText:WideString;
  sText:string;
  PaintRect:TRect;
  HGEFont:THGEFont;
begin
  SaveEditHistory;
  FOldSelection := FSelection;
  TextWidth := 0;
  FSelection.StartPos := 0;
  FSelection.EndPos := 0;
  FSelIndex := 0;
  AText := GetText2;
  PaintRect := ShrinkRect(vRect, 2, 0);
  HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);

  if HGEFont <> nil then begin
    for I := 1 to Length(AText) do begin
      sText := AText[I];
      if TextWidth + HGEFont.TextWidth(sText) - FViewPos > Pt.X then begin
        nLen := TextWidth + HGEFont.TextWidth(sText) - FViewPos - Pt.X;

        if nLen > HGEFont.TextWidth(sText) div 2 then {// 小于应该字符的一半宽度} begin
          Index := GetPosInItem(I - 1);
          if Index = -1 then
            FSelection.StartPos := I - 1
          else if (Index >= 0) and (Index < FTokens.Count) then begin
            nLen := 0;
            for J := 0 to Index - 1 do begin
              nLen := nLen + Length(FTokens[J].Text);
            end;
            FSelection.StartPos := nLen;
          end;
        end
        else begin
          Index := GetPosInItem(I);
          if Index = -1 then
            FSelection.StartPos := I
          else if (Index >= 0) and (Index < FTokens.Count) then begin
            nLen := 0;
            for J := 0 to Index - 1 do begin
              nLen := nLen + Length(FTokens[J].Text);
            end;
            FSelection.StartPos := nLen;
          end;
        end;

        FSelection.EndPos := FSelection.StartPos;
        FSelIndex := FSelection.StartPos;
        Exit;
      end
      else if TextWidth - FViewPos >= Pt.X then begin
        nLen := TextWidth - FViewPos - Pt.X;

        if nLen > HGEFont.TextWidth(sText) div 2 then {// 小于应该字符的一半宽度} begin
          Index := GetPosInItem(I - 1);
          if Index = -1 then
            FSelection.StartPos := I - 1
          else if (Index >= 0) and (Index < FTokens.Count) then begin
            nLen := 0;
            for J := 0 to Index - 1 do begin
              nLen := nLen + Length(FTokens[J].Text);
            end;
            FSelection.StartPos := nLen;
          end;
        end
        else begin
          Index := GetPosInItem(I);
          if Index = -1 then
            FSelection.StartPos := I
          else if (Index >= 0) and (Index < FTokens.Count) then begin
            nLen := 0;
            for J := 0 to Index - 1 do begin
              nLen := nLen + Length(FTokens[J].Text);
            end;
            FSelection.StartPos := nLen;
          end;
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

procedure TDxImageEdit.MoveCursor(Index:Integer);
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

procedure TDxImageEdit.SelectChar(Pt:TPoint; vRect:TRect);
var
  I, J, TextWidth, nIndex, nLen, Index2:Integer;
  AText:WideString;
  PaintRect:TRect;
  sText:string;
  HGEFont:THGEFont;
begin
  HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);
  if HGEFont <> nil then begin
    TextWidth := 0;
    AText := GetText2;
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
          Index2 := GetPosInItem(I - 1);
          if Index2 = -1 then
            nIndex := I - 1
          else if (Index2 >= 0) and (Index2 < FTokens.Count) then begin
            nLen := 0;
            for J := 0 to Index2 - 1 do begin
              nLen := nLen + Length(FTokens[J].Text);
            end;
            nIndex := nLen;
          end;
        end
        else begin
          Index2 := GetPosInItem(I);
          if Index2 = -1 then
            nIndex := I
          else if (Index2 >= 0) and (Index2 < FTokens.Count) then begin
            nLen := 0;
            for J := 0 to Index2 - 1 do begin
              nLen := nLen + Length(FTokens[J].Text);
            end;
            nIndex := nLen;
          end;
        end;

        Break;
      end
      else if TextWidth - ViewPos >= Pt.X - 1 then begin
        nLen := TextWidth - ViewPos - Pt.X;

        if nLen > HGEFont.TextWidth(sText) div 2 then begin // 小于应该字符的一半宽度
          Index2 := GetPosInItem(I - 1);
          if Index2 = -1 then
            nIndex := I - 1
          else if (Index2 >= 0) and (Index2 < FTokens.Count) then begin
            nLen := 0;
            for J := 0 to Index2 - 1 do begin
              nLen := nLen + Length(FTokens[J].Text);
            end;
            nIndex := nLen;
          end;
        end
        else begin
          Index2 := GetPosInItem(I);
          if Index2 = -1 then
            nIndex := I
          else if (Index2 >= 0) and (Index2 < FTokens.Count) then begin
            nLen := 0;
            for J := 0 to Index2 - 1 do begin
              nLen := nLen + Length(FTokens[J].Text);
            end;
            nIndex := nLen;
          end;
        end;
        Break;
      end;
      TextWidth := TextWidth + HGEFont.TextWidth(sText);
    end;

    FSelection.StartPos := FSelIndex;
    FSelection.EndPos := nIndex;
  end;
end;

procedure TDxImageEdit.DoMouseEnter;
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

procedure TDxImageEdit.DoMouseLeave;
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

procedure TDxImageEdit.DoShow();
begin
  FillChar(FWideChar, SizeOf(FWideChar), #0);
  inherited;
end;

procedure TDxImageEdit.DoHide();
begin
  FillChar(FWideChar, SizeOf(FWideChar), #0);
  inherited;
end;

procedure TDxImageEdit.DoFocused();
begin
  FillChar(FWideChar, SizeOf(FWideChar), #0);
  inherited;
end;

procedure TDxImageEdit.MouseMove(Shift:TShiftState; X, Y:Integer);
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

procedure TDxImageEdit.MouseDown(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
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

procedure TDxImageEdit.MouseUp(Button:TMouseButton; Shift:TShiftState; X, Y:Integer);
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

procedure TDxImageEdit.SetFocus();
begin
  inherited SetFocus();
  if Focused then begin
    Repaint;
    if Assigned(FOnEnter) then
      FOnEnter(Self);

    OpenIme;
  end;
end;

procedure TDxImageEdit.PopupMenuClick(Sender:TObject; X, Y:Integer);
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

procedure TDxImageEdit.SelectAll;
var
  AText:WideString;
begin
  if not FAllowSelect then Exit;
  AText := Text;
  FSelection.StartPos := 0;
  FSelection.EndPos := Length(AText);
  Repaint;
end;

function TDxImageEdit.InRange(X, Y:Integer):Boolean;
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

procedure TDxImageEdit.DrawText(vtRect, vbRect:TRect);
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
  SText := GetText2;
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

procedure TDxImageEdit.DrawHintText(vtRect, vbRect:TRect);
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

procedure TDxImageEdit.DrawSelector(vtRect, vbRect:TRect);
var
  TextWidth:Integer;
  PaintRect:TRect;
  AText:WideString;
  sText:string;
  HGEFont:THGEFont;
begin
  AText := GetText2;
  sText := Copy(AText, 1, FSelection.EndPos);
  HGEFont := TextureFonts.FindFont(Font.Name, Font.Size);
  if HGEFont <> nil then
    TextWidth := HGEFont.TextWidth(sText);
  PaintRect := Bounds(vtRect.Left + (TextWidth - ViewPos), vtRect.Top, 1, vtRect.Bottom - vtRect.Top);
  FillRect(PaintRect, vtRect, vbRect, SelectedColor);
end;

procedure TDxImageEdit.SetOnGetImage(Value:TOnGetImage);
begin
  inherited;
  FBackgroundImage.OnGetImage := Value;
  FDisableBackgroundImage.OnGetImage := Value;
end;

procedure TDxImageEdit.DoUpdate();
begin
  inherited;
end;

procedure TDxImageEdit.Paint;
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

  if not Enabled then begin
    if not FDisableBackgroundTransparent then
      GameCanvas.FillRectAlpha(vbRect, FDisableBackgroundColor, FDisableBackgroundAlpha);

    FDisableBackgroundImage.Paint;
  end
  else begin
    if not Transparent then
      GameCanvas.FillRectAlpha(vbRect, BackgroundColor, FBackgroundColorAlpha);

    FBackgroundImage.Paint;
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

    if (Length(FHintText) > 0) and (Length(GetText2) <= 20) then begin
      DrawHintText(PaintRect, ShortRect(PaintRect, vbRect));
    end;
  end
  else begin
    if not (Enabled and (PopupMenu <> nil) and PopupMenu.Visible and PopupMenu.Focused and (PopupMenu.PopupMenu = Self)) then begin
      if FSelection.StartPos < FSelection.EndPos then
        FSelection.StartPos := FSelection.EndPos
      else
        FSelection.EndPos := FSelection.StartPos;
    end;
  end;
  if Assigned(OnStopPaint) then
    OnStopPaint(Self);
end;

end.
