unit GfxFonts;

interface
//---------------------------------------------------------------------------
uses
  Windows, Types, Classes, SysUtils, Contnrs, Forms,
  Math, HGE, DxCanvas, HGECanvas, Graphics, HashTable, TextureImages;
type
  TGfxFontTexture = class
  private
    FTexture: TTexture;
    FText: string;
    //FStyle: TFontStyles;
    FOutTimeTick: LongWord;
    FOutTimeTime: LongWord;
    FX, FY: Integer;
  public
    constructor Create();
    destructor Destroy; override;
    property Texture: TTexture read FTexture write FTexture;
    property Text: string read FText write FText;
    //property Style: TFontStyles read FStyle write FStyle;
    property OutTimeTick: LongWord read FOutTimeTick write FOutTimeTick;
    property OutTimeTime: LongWord read FOutTimeTime write FOutTimeTime;
    property X: Integer read FX write FX;
    property Y: Integer read FY write FY;
  end;

  TGfxFontTextureArray = array of TGfxFontTexture;

  TGfxFontTextures = class
  private
    FList: THashTable;
    FOutTimeTick: LongWord;
    FOutTimeTime: LongWord;
    FProcIdx: Integer;
    function GetTexture(Index: Integer): TGfxFontTexture;
    function GetTextureCount(): Integer;
  public
    constructor Create();
    destructor Destroy; override;
    procedure FreeIdleMemory;
    procedure Clear;

    procedure Add(const Text: string; Texture: TGfxFontTexture);
    property List: THashTable read FList;
    property Textures[Index: Integer]: TGfxFontTexture read GetTexture;
    property TextureCount: Integer read GetTextureCount;
    property OutTimeTick: LongWord read FOutTimeTick write FOutTimeTick;
    property OutTimeTime: LongWord read FOutTimeTime write FOutTimeTime;
  end;

  TGfxTextureFonts = class;
  TGfxTextureFont = class(TFont)
  private
    FOwner: TGfxTextureFonts;
    FFontWidth: Integer;
    FFontHeight: Integer;
    FFontWidthBold: Integer;

    FDoubleFontWidth: Integer;
    FDoubleFontHeight: Integer;
    FDoubleFontWidthBold: Integer;
    FTimeOutIdx: Integer;

    FFontTextures: TGfxFontTextures;

    procedure NewBitmapFile(const AWidth, AHeight, ABitCount: Integer; var FileData: Pointer; var FileSize: Integer);
    procedure GetFontSize;

    procedure DrawText(X, Y: Integer;
      const Text: string;
      FColor: TColor = clWhite);
  public
    nIndex: Integer;
    constructor Create(AOwner: TGfxTextureFonts);
    destructor Destroy; override;
    procedure Clear;
    procedure FreeIdleMemory;
    procedure Initialize;
    procedure Finalize;
    function GetFontTextureArray(const Text: string): TGfxFontTextureArray;
    function GetFontTexture(const Text: string): TGfxFontTexture;
    function GetTextTexture(const Text: string): TTexture;
    function TextHeight(const Text: string): Integer;
    function TextWidth(const Text: string): Integer;

    procedure TextOut(X, Y: Integer;
      const Text: string;
      FColor: TColor = clWhite); overload;

    procedure TextOut(X, Y: Integer;
      const Text: string;
      FColor: TColor;
      BColor: TColor); overload;

    procedure TextOutA(X, Y: Integer;
      const Text: string;
      FColor: Cardinal = $FFFFFFFF
      );

    procedure TextRect(Rect: TRect;
      X, Y: Integer;
      const Text: string;
      FColor: TColor = clWhite); overload;

    procedure TextRect(Dest, Rect: TRect;
      X, Y: Integer;
      const Text: string;
      FColor: TColor = clWhite); overload;

    procedure TextRect(Rect: TRect;
      X, Y: Integer;
      const Text: string;
      FColor: TColor = clWhite;
      BColor: TColor = clBlack); overload;

    property Owner: TGfxTextureFonts read FOwner;
    property FontTextures: TGfxFontTextures read FFontTextures;
  end;

  TGfxTextureFonts = class
  private
    //FCanvas: THGECanvas;
    Fonts: array of TGfxTextureFont;
    FD3DFormat: Boolean;
    FCriticalSection: TRTLCriticalSection;
    function GetCount(): Integer;
    function GetFont(Num: Integer): TGfxTextureFont;
  public
    constructor Create();
    destructor Destroy; override;
    procedure FreeIdleMemory;
    procedure Initialize;
    procedure Finalize;
    procedure RemoveAll();
    procedure RemoveFont(Num: Integer);
    procedure Lock;
    procedure UnLock;

    procedure Add(FontName: TFontName = '宋体'; FontSize: Integer = 9; FontStyles: TFontStyles = []);
    procedure SetFont(FontName: TFontName = '宋体'; FontSize: Integer = 9; FontStyles: TFontStyles = []);
    property D3DFormat: Boolean read FD3DFormat write FD3DFormat;
    property Font[Num: Integer]: TGfxTextureFont read GetFont; default;
    property Count: Integer read GetCount;
  end;
var
  GfxTextureFont: TGfxTextureFont = nil;
  GfxTextureFonts: TGfxTextureFonts = nil;
const
  WideNull = WideChar(#0);
  WideCR = WideChar(#13);
  WideLF = WideChar(#10);
  WideCRLF: WideString = #13#10;
implementation


constructor TGfxFontTexture.Create();
begin
  inherited;
  FTexture := nil;
  FText := '';
  //FStyle := [];
  FOutTimeTick := GetTickCount;
  FOutTimeTime := 1000 * 60 * 1;
end;

destructor TGfxFontTexture.Destroy;
begin
  if FTexture <> nil then
    FreeAndNil(FTexture);
  inherited;
end;
{-------------------------------------------------------------------------------}

constructor TGfxFontTextures.Create();
begin
  inherited;
  FList := THashTable.Create(1000);
  FOutTimeTick := GetTickCount;
  FOutTimeTime := 1000;
  FProcIdx := 0;
end;

destructor TGfxFontTextures.Destroy;
var
  I: Integer;
  Texture: TGfxFontTexture;
begin
  for I := 0 to FList.Count - 1 do begin
    Texture := FList.Items[I];
    if Texture <> nil then
      FreeAndNil(Texture);
  end;
  FList.Free;
  inherited;
end;

procedure TGfxFontTextures.Add(const Text: string; Texture: TGfxFontTexture);
begin
  FList.Add(Text, Text, Texture);
end;

procedure TGfxFontTextures.Clear;
var
  I: Integer;
  Texture: TGfxFontTexture;
begin
  FProcIdx := 0;
  for I := 0 to FList.Count - 1 do begin
    Texture := FList.Items[I];
    if Texture <> nil then
      FreeAndNil(Texture);
  end;
  FList.Clear;
end;

procedure TGfxFontTextures.FreeIdleMemory;
var
  Texture: TGfxFontTexture;
  nIdx: Integer;
  dwTimeTick: longword;
  boCheckTimeLimit: Boolean;
begin
  if GetTickCount - FOutTimeTick > FOutTimeTime then begin
    FOutTimeTick := GetTickCount;
    dwTimeTick := GetTickCount;

    nIdx := FProcIdx;
    boCheckTimeLimit := False;
    while True do begin
      if FList.Count <= nIdx then Break;
      Texture := FList.Items[nIdx];
      if Texture <> nil then begin
        if GetTickCount - Texture.OutTimeTick > Texture.OutTimeTime then begin
          FreeAndNil(Texture);
          FList.Delete(nIdx);
          Inc(nIdx);
          Continue;
        end;
      end;

      Inc(nIdx);
      if (GetTickCount - dwTimeTick) > 10 then begin
        boCheckTimeLimit := True;
        FProcIdx := nIdx;
        Break;
      end;
    end;
    if not boCheckTimeLimit then FProcIdx := 0;
  end;
end;

function TGfxFontTextures.GetTexture(Index: Integer): TGfxFontTexture;
begin
  if (Index >= 0) and (Index < FList.Count) then
    Result := FList.Items[Index] else Result := nil;
end;

function TGfxFontTextures.GetTextureCount(): Integer;
begin
  Result := FList.Count;
end;

{
procedure DebugOutStr(Msg: string);
var
  flname: string;
  fhandle: TextFile;
begin
//DScreen.AddChatBoardString(msg,clWhite, clBlack);
  //exit;
  flname := '.\!debug.txt';
  if FileExists(flname) then begin
    AssignFile(fhandle, flname);
    Append(fhandle);
  end else begin
    AssignFile(fhandle, flname);
    Rewrite(fhandle);
  end;
  Writeln(fhandle, Msg);
  CloseFile(fhandle);
end; }

{------------------------------TGfxTextureFont------------------------------}

constructor TGfxTextureFont.Create(AOwner: TGfxTextureFonts);
begin
  inherited Create;
  FOwner := AOwner;
  Name := '宋体';
  Size := 9;
  Charset := GB2312_CHARSET;
  Style := [];
  FFontWidth := 6;
  FFontHeight := 12;
  FFontWidthBold := 7;

  FDoubleFontWidth := 12;
  FDoubleFontHeight := 12;
  FDoubleFontWidthBold := 13;

  FFontTextures := TGfxFontTextures.Create;
  FTimeOutIdx := 0;
end;

destructor TGfxTextureFont.Destroy;
begin
  FFontTextures.Free;
  inherited;
end;

procedure TGfxTextureFont.Clear;
begin
  FFontTextures.Clear;
end;

const
  TextChars = [#32..#255];

function TGfxTextureFont.GetFontTextureArray(const Text: string): TGfxFontTextureArray;
var
  I: Integer;
  sText: WideString;
  S: string;
  cChar: Char;
  AsphyreFontTexture: TGfxFontTexture;
begin
  SetLength(Result, 0);
  if Text = '' then Exit;
  sText := Text;
  for I := 1 to Length(sText) do begin
    S := sText[I];
    cChar := S[1];
    if cChar in TextChars then begin
      AsphyreFontTexture := GetFontTexture(S);
      if AsphyreFontTexture <> nil then begin
        SetLength(Result, Length(Result) + 1);
        Result[Length(Result) - 1] := AsphyreFontTexture;
      end;
    end else begin
      if Length(S) = 2 then begin
        SetLength(Result, Length(Result) + 2);
      end else begin
        SetLength(Result, Length(Result) + 1);
      end;
    end;
  end;
end;

procedure TGfxTextureFont.DrawText(X, Y: Integer;
  const Text: string;
  FColor: TColor);
var
  I, II, nX, nY: Integer;
  TextList: TStringList;
  Texture: TGfxFontTexture;
  AsphyreFontTextureArray: TGfxFontTextureArray;
begin
  if Pos(WideCR, Text) > 0 then begin
    TextList := TStringList.Create;
    try
      TextList.Text := Text;
      nY := Y;
      for I := 0 to TextList.Count - 1 do begin
        nX := X;
        AsphyreFontTextureArray := GetFontTextureArray(TextList.Strings[I]);
        for II := 0 to Length(AsphyreFontTextureArray) - 1 do begin
          Texture := AsphyreFontTextureArray[II];
          if Texture <> nil then begin
            GameCanvas.DrawColor(nX, nY, Texture.Texture, FColor);
            nX := nX + Texture.Texture.Width;
          end else begin
            nX := nX + TextWidth('0');
          end;
        end;
        nY := nY + TextHeight('0');
      end;
    finally
      TextList.Free;
    end;
  end else begin
    nY := Y;
    nX := X;
    AsphyreFontTextureArray := GetFontTextureArray(Text);
    for II := 0 to Length(AsphyreFontTextureArray) - 1 do begin
      Texture := AsphyreFontTextureArray[II];
      if Texture <> nil then begin
        GameCanvas.DrawColor(nX, nY, Texture.Texture, FColor);
        nX := nX + Texture.Texture.Width;
      end else begin
        nX := nX + TextWidth('0');
      end;
    end;
  end;
end;

procedure TGfxTextureFont.GetFontSize;
var
  //TextMetric: TTextMetric;
  BitmapInfo: TBitmapInfo;
  HHBitmap: HBitmap;
  HHDC: HDC;
  TextSize: TSize;
  OldStyle: TFontStyles;
begin
  OldStyle := Style;
  Style := [];
  HHDC := CreateCompatibleDC(0);

  SelectObject(HHDC, Handle);

  Windows.GetTextExtentPoint32W(HHDC, '0', 1, TextSize);
  FFontWidth := abs(TextSize.cx);
  FFontHeight := abs(TextSize.cy);

  Windows.GetTextExtentPoint32W(HHDC, '一', 1, TextSize);
  FDoubleFontWidth := abs(TextSize.cx);
  FDoubleFontHeight := abs(TextSize.cy);

  DeleteDC(HHDC);

  Style := [fsBold];
  HHDC := CreateCompatibleDC(0);

  SelectObject(HHDC, Handle);

  Windows.GetTextExtentPoint32W(HHDC, '0', 1, TextSize);
  FFontWidthBold := abs(TextSize.cx);

  Windows.GetTextExtentPoint32W(HHDC, '一', 1, TextSize);
  FDoubleFontWidthBold := abs(TextSize.cx);
  Style := OldStyle;
end;

function TGfxTextureFont.GetTextTexture(const Text: string): TTexture;
var
  FontTexture: TGfxFontTexture;
begin
  Result := nil;
  FontTexture := GetFontTexture(Text);
  if FontTexture <> nil then
    Result := FontTexture.Texture;
end;

procedure TGfxTextureFont.NewBitmapFile(const AWidth, AHeight, ABitCount: Integer; var FileData: Pointer; var FileSize: Integer);
var
  FileHeader: PBitmapFileHeader;
  InfoHeader: PBitmapInfoHeader;

  Buffer: Pointer;
begin
  FileSize := SizeOf(TBitmapFileHeader) + SizeOf(TBitmapInfoHeader) + AWidth * AHeight * (ABitCount div 8);

  FileData := AllocMem(FileSize);

  // 位图文件头
  Buffer := FileData;
  FileHeader := PBitmapFileHeader(Buffer);
  FileHeader.bfType := 19778; //MakeWord(Ord('B'), Ord('M'));
  FileHeader.bfSize := FileSize; //SizeOf(TBitmapFileHeader) + SizeOf(TBitmapInfoHeader) + AWidth * AHeight * (ABitCount div 8);
  FileHeader.bfOffBits := FileHeader.bfSize - AWidth * AHeight * (ABitCount div 8);
  FileHeader.bfReserved1 := 0;
  FileHeader.bfReserved2 := 0;

  Buffer := Pointer(Integer(Buffer) + SizeOf(TBitmapFileHeader));

  // 位图信息头
  InfoHeader := PBitmapInfoHeader(Buffer);
  InfoHeader.biSize := SizeOf(TBitmapInfoHeader);
  InfoHeader.biWidth := AWidth;
  InfoHeader.biHeight := -AHeight;
  InfoHeader.biPlanes := 1;
  InfoHeader.biBitCount := ABitCount;

  InfoHeader.biCompression := BI_RGB; //BI_RGB;
  InfoHeader.biSizeImage := AWidth * AHeight * (ABitCount div 8);
  InfoHeader.biXPelsPerMeter := 0;
  InfoHeader.biYPelsPerMeter := 0;
  InfoHeader.biClrUsed := 0;
  InfoHeader.biClrImportant := 0;
end;

procedure DebugOutStr(Msg: string);
var
  flname: string;
  fhandle: TextFile;
begin
  flname := ExtractFilePath(Application.ExeName) + 'DebugOutStr.txt';

  if FileExists(flname) then begin
    AssignFile(fhandle, flname);
    Append(fhandle);
  end else begin
    AssignFile(fhandle, flname);
    Rewrite(fhandle);
  end;
  //Writeln(fhandle, TimeToStr(Time) + ' ' + Msg);
  Writeln(fhandle, FormatDateTime('yyyy-mm-dd hh:mm:ss', Now) + ' ' + Msg);

  CloseFile(fhandle);
end;

function TGfxTextureFont.GetFontTexture(
  const Text: string): TGfxFontTexture;
var
  I, II, III, nWidth, nHeight, X, Y: Integer;
  Texture: TGfxFontTexture;
  PBitmapBits: Pointer; // PIntegerArray;
  //TextMetric: TTextMetric;
  BitmapInfo: TBitmapInfo;
  HHBitmap: HBitmap;
  HHDC: HDC;

  FontTexture: TGfxFontTexture;

  ACol, ARow: Integer;
  P: PInteger;
  Bits: Pointer;
  Pitch: Integer;

  SrcP: Pointer;
  DesP: Pointer;
  Pix: Cardinal;
  RGBQuad: PRGBQuad;

  TextList: TStringList;

  FileHeader: PBitmapFileHeader;
  InfoHeader: PBitmapInfoHeader;

  Buffer: Pointer;

  nCode: Integer;
begin
  Result := nil;

  //try
  nCode := 0;
  if Text = '' then Exit;
  nCode := 1;
  FontTexture := FFontTextures.List.Datas[Text];
  nCode := 2;
  if FontTexture <> nil then begin
    nCode := 3;
    FontTexture.OutTimeTick := GetTickCount;
    nCode := 4;
    Result := FontTexture;
    nCode := 5;
    Exit;
  end;
  nCode := 6;
  {for I := 0 to FFontTextures.TextureCount - 1 do begin
    FontTexture := FFontTextures.Textures[I];
    if (FontTexture.Style = FontStyles) and
      (CompareStr(FontTexture.Text, Text) = 0) then begin
      FontTexture.OutTimeTick := GetTickCount;
      Result := FontTexture;
      Exit;
    end;
  end;}

  TextList := TStringList.Create;
  TextList.Text := Text;
  nCode := 7;

  nWidth := 0;
  nHeight := 0;

  for I := 0 to TextList.Count - 1 do begin
    nWidth := Max(nWidth, TextWidth(TextList.Strings[I]));
  end;
  nCode := 8;

  nHeight := TextHeight('pP') * TextList.Count;
  nCode := 9;
  FillChar(BitmapInfo, SizeOf(BitmapInfo), 0);
  nCode := 10;

  with BitmapInfo.bmiHeader do begin
  //位图信息头
    biSize := SizeOf(TBitmapInfoHeader);
    biWidth := nWidth;
    biHeight := -nHeight;
    biPlanes := 1;
    biBitCount := 32;
    biCompression := BI_RGB;
  end;

  nCode := 11;
  HHDC := CreateCompatibleDC(0);
  nCode := 12;
  PBitmapBits := nil;
  HHBitmap := CreateDIBSection(HHDC, BitmapInfo, DIB_RGB_COLORS, PBitmapBits, 0, 0);
  if (HHBitmap <> 0) and (PBitmapBits <> nil) then begin
    nCode := 13;
    SelectObject(HHDC, Handle);
    nCode := 14;
    SelectObject(HHDC, HHBitmap);
    nCode := 15;
    SetTextColor(HHDC, RGB(255, 255, 255)); //设文字颜色为白色
    nCode := 16;
    SetBkColor(HHDC, RGB(0, 0, 0)); //设背景颜色为黑色
    nCode := 17;
    Y := 0;
    for I := 0 to TextList.Count - 1 do begin
      Windows.TextOut(HHDC, 0, Y, PChar(TextList.Strings[I]), Length(TextList.Strings[I]));
      Inc(Y, TextHeight('pP'));
    end;
    nCode := 18;

    nCode := 19;
    Texture := TGfxFontTexture.Create;
    Texture.Text := Text;
    //Texture.Style := Style;
    nCode := 20;
    Texture.Texture := NewTexture(PBitmapBits, nWidth, nHeight);
    nCode := 21;
    if Texture.Texture <> nil then begin
      nCode := 22;
      FFontTextures.Add(Text, Texture);
      nCode := 23;
      Result := Texture;
    end else begin
      nCode := 24;
      FreeAndNil(Texture);
      nCode := 25;
    end;

    nCode := 26;
    DeleteObject(HHBitmap);
  end;
  nCode := 27;
  DeleteDC(HHDC);
  TextList.Free;
  nCode := 28;
  {except
    on E: Exception do begin
      Result := nil;
      DebugOutStr('GetFontTexture:' + IntToStr(nCode));
      DebugOutStr('GetFontTexture ' + E.Message);
    end;
  end;}
end;

procedure TGfxTextureFont.TextOutA(X, Y: Integer;
  const Text: string;
  FColor: Cardinal
  );
//var
//  Texture: TTexture;
begin
  DrawText(X, Y, Text, FColor);
  //if Text = '' then Exit;
 // Texture := GetTextTexture(Text, FontStyles);
  //if Texture = nil then Exit;
  //GameCanvas.DrawColor(X, Y, Texture.ClientRect, Texture, FColor); //DisplaceRB(FColor)  ,deSrcAlphaAdd deSrcAlphaAdd or $FF000000
end;

procedure TGfxTextureFont.TextOut(X, Y: Integer;
  const Text: string;
  FColor: TColor);
//var
//  Texture: TTexture;
begin
  DrawText(X, Y, Text, FColor);
  //if Text = '' then Exit;
  //Texture := GetTextTexture(Text, FontStyles);
 // if Texture = nil then Exit;
 // GameCanvas.DrawColor(X, Y, Texture.ClientRect, Texture, FColor); //DisplaceRB(FColor)  ,deSrcAlphaAdd deSrcAlphaAdd or $FF000000
end;

procedure TGfxTextureFont.TextOut(X, Y: Integer;
  const Text: string;
  FColor: TColor; // Cardinal;
  BColor: TColor);
//var
 // Texture: TTexture;
begin
  //if Text = '' then Exit;
  //Texture := GetTextTexture(Text, FontStyles);
  //if Texture = nil then Exit;
  GameCanvas.FillRect(Bounds(X, Y, TextWidth(Text), TextHeight(Text)), BColor); //Color4(DisplaceRB(BColor))
  DrawText(X, Y, Text, FColor);
 // GameCanvas.DrawColor(X, Y, Texture.ClientRect, Texture, FColor); //DisplaceRB(FColor)  ,deSrcAlphaAdd deSrcAlphaAdd or $FF000000
end;

procedure TGfxTextureFont.TextRect(Rect: TRect;
  X, Y: Integer;
  const Text: string;
  FColor: TColor);
var
  SourceRect: TRect;
  Texture: TTexture;
begin
 // if Text = '' then Exit;
 // Texture := GetTextTexture(Text, FontStyle);
 // if Texture = nil then Exit;
  //GameCanvas.DrawColor(X, Y, Rect, Texture, FColor);
  {
  SourceRect := Bounds(Rect.Left - X, Rect.Top - Y, Rect.Right - Rect.Left, Rect.Bottom - Rect.Top);
  if ClipRect(SourceRect, Texture.ClientRect) then begin
    Canvas.DrawColor(X, Y, SourceRect, Texture, FColor);
  end;}
end;

procedure TGfxTextureFont.TextRect(Dest, Rect: TRect;
  X, Y: Integer;
  const Text: string;
  FColor: TColor = clWhite);
//var
//  SourceRect: TRect;
 // Texture: TTexture;
begin
  {if Text = '' then Exit;
  Texture := GetTextTexture(Text, FontStyle);
  if Texture = nil then Exit;
  SourceRect := GameCanvas.ClientRect;

  GameCanvas.ClientRect := Dest;
  GameCanvas.DrawColor(X, Y, Rect, Texture, FColor);
  GameCanvas.ClientRect := SourceRect;      }
  {
  SourceRect := Bounds(Rect.Left - X, Rect.Top - Y, Rect.Right - Rect.Left, Rect.Bottom - Rect.Top);
  if ClipRect(SourceRect, Texture.ClientRect) then begin
    Canvas.DrawColor(X, Y, SourceRect, Texture, FColor);
  end;}
end;

procedure TGfxTextureFont.TextRect(Rect: TRect;
  X, Y: Integer;
  const Text: string;
  FColor: TColor;
  BColor: TColor);
//var
//  SourceRect: TRect;
//  Texture: TTexture;
begin
 { if Text = '' then Exit;

  Texture := GetTextTexture(Text, FontStyle);
  if Texture = nil then Exit;

  SourceRect := Bounds(Rect.Left - X, Rect.Top - Y, Rect.Right - Rect.Left, Rect.Bottom - Rect.Top);
  if ClipRect(SourceRect, Texture.ClientRect) then begin
    GameCanvas.FillRect(Bounds(X, Y, SourceRect.Right - SourceRect.Left, SourceRect.Bottom - SourceRect.Top), cColor4(cColor1(BColor)));
    GameCanvas.DrawColor(X, Y, SourceRect, Texture, FColor);
  end;   }
end;

procedure TGfxTextureFont.FreeIdleMemory;
begin
  FFontTextures.FreeIdleMemory;
end;

procedure TGfxTextureFont.Initialize;
begin
  GetFontSize;
end;

procedure TGfxTextureFont.Finalize;
begin
  FFontTextures.Clear;
end;

function TGfxTextureFont.TextHeight(const Text: string): Integer;
var
  sText: WideString;
begin
  sText := Text;
  if Length(Text) = Length(sText) then begin
    Result := FFontHeight;
  end else begin
    Result := Max(FFontHeight, FDoubleFontHeight);
  end;
end;

function TGfxTextureFont.TextWidth(const Text: string): Integer;
var
  nCount, nsCount, nwCount: Integer;
  sText: WideString;
begin
  sText := Text;
  nsCount := Length(Text);
  nwCount := Length(sText);
  if nsCount = nwCount then begin
    if fsBold in Style then
      Result := FFontWidthBold * Length(Text)
    else
      Result := FFontWidth * Length(Text);
  end else begin
    nCount := nsCount - nwCount; //双字节字符数
    if fsBold in Style then
      Result := FDoubleFontWidthBold * Max(nCount, 0) + Max((nwCount - nCount), 0) * FFontWidthBold
    else
      Result := FDoubleFontWidth * Max(nCount, 0) + Max((nwCount - nCount), 0) * FFontWidth;
  end;
end;
//---------------------------------------------------------------------------

constructor TGfxTextureFonts.Create();
begin
  inherited;
  InitializeCriticalSection(FCriticalSection);
  FD3DFormat := False;
  Add();
end;

//---------------------------------------------------------------------------

destructor TGfxTextureFonts.Destroy();
begin
  RemoveAll();
  DeleteCriticalSection(FCriticalSection);
  inherited;
end;

//---------------------------------------------------------------------------

function TGfxTextureFonts.GetCount(): Integer;
begin
  Result := Length(Fonts);
end;

//---------------------------------------------------------------------------

function TGfxTextureFonts.GetFont(Num: Integer): TGfxTextureFont;
begin
  if (Num >= 0) and (Num < Length(Fonts)) then
    Result := Fonts[Num] else Result := nil;
end;

procedure TGfxTextureFonts.Lock;
begin
  EnterCriticalSection(FCriticalSection);
end;

procedure TGfxTextureFonts.UnLock;
begin
  LeaveCriticalSection(FCriticalSection);
end;
//---------------------------------------------------------------------------

procedure TGfxTextureFonts.RemoveAll();
var
  I: Integer;
begin
  Lock;
  try
    for I := 0 to Length(Fonts) - 1 do
      if (Fonts[i] <> nil) then
        FreeAndNil(Fonts[I]);

    SetLength(Fonts, 0);
  finally
    UnLock;
  end;

end;

//---------------------------------------------------------------------------

procedure TGfxTextureFonts.Add(FontName: TFontName; FontSize: Integer; FontStyles: TFontStyles);
var
  nIndex: Integer;
begin
  for nIndex := 0 to Length(Fonts) - 1 do begin
    if (Fonts[nIndex].Size = FontSize) and
      (CompareText(Fonts[nIndex].Name, FontName) = 0) and (Fonts[nIndex].Style = FontStyles) then begin
      if GfxTextureFont = nil then
        GfxTextureFont := Fonts[nIndex];
      Exit;
    end;
  end;
  nIndex := Length(Fonts);
  SetLength(Fonts, nIndex + 1);
  Fonts[nIndex] := TGfxTextureFont.Create(Self);
  Fonts[nIndex].nIndex := nIndex;
  Fonts[nIndex].Name := FontName;
  Fonts[nIndex].Size := FontSize;
  Fonts[nIndex].Style := FontStyles;
  Fonts[nIndex].Initialize;
  if GfxTextureFont = nil then
    GfxTextureFont := Fonts[nIndex];
end;

//---------------------------------------------------------------------------

procedure TGfxTextureFonts.RemoveFont(Num: Integer);
var
  I: Integer;
begin
  if (Num < 0) or (Num >= Length(Fonts)) then Exit;

  Lock;
  try
    FreeAndNil(Fonts[Num]);

    for I := Num to Length(Fonts) - 2 do begin
      Fonts[I] := Fonts[I + 1];
      Fonts[I].nIndex := I;
    end;
    SetLength(Fonts, Length(Fonts) - 1);
  finally
    UnLock;
  end;
end;

procedure TGfxTextureFonts.FreeIdleMemory;
var
  I: Integer;
begin
  for I := 0 to Length(Fonts) - 1 do
    Fonts[I].FreeIdleMemory;
end;

procedure TGfxTextureFonts.Initialize;
var
  I: Integer;
begin
  for I := 0 to Length(Fonts) - 1 do
    Fonts[I].Initialize;
  SetFont('宋体', 9);
end;

procedure TGfxTextureFonts.Finalize;
var
  I: Integer;
begin
  GfxTextureFont := nil;
  for I := 0 to Length(Fonts) - 1 do
    Fonts[I].Finalize;
  SetFont('宋体', 9);
end;

procedure TGfxTextureFonts.SetFont(FontName: TFontName; FontSize: Integer; FontStyles: TFontStyles);
var
  nIndex: Integer;
begin
  Lock;
  try
    if (GfxTextureFont <> nil) and (GfxTextureFont.Size = FontSize) and
      ((CompareText(GfxTextureFont.Name, FontName) = 0) or (FontName = '')) and (GfxTextureFont.Style = FontStyles) then begin
      Exit;
    end;

    for nIndex := 0 to Length(Fonts) - 1 do begin
      if (Fonts[nIndex].Size = FontSize) and
        ((CompareText(Fonts[nIndex].Name, FontName) = 0) or (FontName = '')) and (Fonts[nIndex].Style = FontStyles) then begin
        GfxTextureFont := Fonts[nIndex];
        Exit;
      end;
    end;
    GfxTextureFont := nil;
    if FontName = '' then FontName := '宋体';
    Add(FontName, FontSize, FontStyles);
  finally
    UnLock;
  end;
end;

end.

