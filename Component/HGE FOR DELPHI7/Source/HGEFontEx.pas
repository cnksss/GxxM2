unit HGEFontEx;

interface
uses
  Windows,
  Classes,
  SysUtils,
  Graphics,
  HGE,
  WideCharList {, IGDIPlus};
const
  NullRect:TRect = (Left:0; Top:0; Right:0; Bottom:0);
type
  TImageIndexs = array of Integer;

  TImageInfo = record
    Width:Integer;
    Height:Integer;
    ImageIndexs:TImageIndexs;
  end;
  pTImageInfo = ^TImageInfo;

  TImageInfos = array of TImageInfo;

  TImageRect = record
    Active:Boolean;
    X, Y:SmallInt;
    Rect:TRect;
    Index:Integer;
    Time:LongWord;
    S:WideChar;
  end;
  pTImageRect = ^TImageRect;

  THGEFont = class
  private
    FTexture:TTexture;

    FImageRect:array of TImageRect;
    //FreeMemCheckTick: LongWord;

    FRowCount:Integer;
    FColCount:Integer;

    FInitialized:Boolean;
    FCriticalSection:TRTLCriticalSection;

    FList:TWideCharList;
    FFont:TFont;

    FFontWidth:Integer;
    FFontHeight:Integer;

    FDoubleFontWidth:Integer;
    FDoubleFontHeight:Integer;

    FProcIdx:Integer;
    FTextCount:Integer;
    procedure SetFont(Value:TFont);
    function GetImages(Index:Integer):pTImageRect;
    function GetCount:Integer;

    procedure InitEnglishString;
    procedure GetFontSize;

    function Add(const Text:WideChar):pTImageRect;
  public
    m_dwFreeMemCheckTime:Cardinal; //Integer; HZQ 20230519
    constructor Create(ATextCount:Integer);
    destructor Destroy; override;
    procedure Lock;
    procedure UnLock;

    procedure Initialize;
    procedure Finalize;
    //procedure Clear;
    procedure FreeOldMemorys;

    procedure TextRect(const X, Y:Integer; SrcRect:TRect; const ImageIndexs:TImageIndexs; Color:TColor = clWhite; BlendMode:Integer = Blend_Default; Alpha:Byte = 255); overload;
    procedure TextRect(const X, Y:Integer; SrcRect:TRect; const Text:string; Color:TColor = clWhite; BlendMode:Integer = Blend_Default; Alpha:Byte = 255); overload;
    procedure TextRect(const X, Y:Integer; SrcRect:TRect; ImageInfos:TImageInfos; Color:TColor = clWhite; BlendMode:Integer = Blend_Default; Alpha:Byte = 255; ExpandLineHeight:Integer = 0); overload;

    procedure TextOut(const X, Y:Integer; const Text:string; Color:TColor = clWhite; BlendMode:Integer = Blend_Default; Alpha:Byte = 255); overload;
    procedure TextOut(const X, Y:Integer; ImageIndexs:TImageIndexs; Color:TColor = clWhite; BlendMode:Integer = Blend_Default; Alpha:Byte = 255); overload;

    function TextWidth(const Text:string):Integer;
    function TextHeight(const Text:string):Integer;
    function GetImageInfo(const Text:string):TImageInfo;
    function GetImageInfos(const Text:string):TImageInfos;
    property Font:TFont read FFont write SetFont;
    //property List: THashList read FList;

    property Count:Integer read GetCount;
    //property Texture: TTexture read FTexture;

    function GetActive:pTImageRect;

    property Initialized:Boolean read FInitialized;

    property RowCount:Integer read FRowCount;
    property ColCount:Integer read FColCount;

    property Images[Index:Integer]:pTImageRect read GetImages;

    procedure SaveTextureToStream(Stream:TStream);
  end;

  THGEFonts = class
  private
    FList:TList;
    FCriticalSection:TRTLCriticalSection;
  public
    constructor Create();
    destructor Destroy; override;
    procedure Lock;
    procedure UnLock;
    function TryLock:Boolean;
    procedure Initialize;
    procedure Finalize;
    procedure FreeOldMemorys;
    procedure Add(AFont:THGEFont);
    //procedure Clear;
    function FindFont(AFontName:string = '宋体'; AFontSize:Integer = 9; AFontStyles:TFontStyles = []):THGEFont;

    //property List: TList read FList;
  end;

implementation

uses HGECanvas,
  Math,
  DIB {, MShare};

function Is2n(N:Integer):Boolean; //检测是否是2次幂
begin
  if (N = 0) then
    Result := False
  else
    Result := ((N and (N - 1)) = 0) and (N <> 0) and (N <> 1);
end;

function ANS(N:Integer):Integer; //2次幂
var
  nR, nT:Integer;
begin
  if Is2n(N) then
    Result := N
  else begin
    nR := 1;
    nT := N;
    while (nT <> 0) do begin
      nT := nT shr 1;
      nR := nR shl 1;
    end;
    if (N = 0) or ((N shl 1) = nR) then
      nR := nR shr 1;

    Result := nR;
    if Result in [0, 1] then Result := 2;
  end;
end;

constructor THGEFont.Create(ATextCount:Integer);
begin
  InitializeCriticalSection(FCriticalSection);
  FTexture := nil;

  FColCount := 1;
  FRowCount := 1;
  FInitialized := False;

  FFont := TFont.Create;
  FFont.Name := '宋体';
  FFont.Size := 9;
  FFont.Charset := GB2312_CHARSET;
  FFont.Style := [];

  FFontWidth := 6;
  FFontHeight := 12;
  FDoubleFontWidth := 12;
  FDoubleFontHeight := 12;
  FTextCount := ATextCount;
  FProcIdx := 0;
  FList := TWideCharList.Create(ATextCount); //ATextCount
  m_dwFreeMemCheckTime := 1000 * 60 * 5;
end;

destructor THGEFont.Destroy;
begin
  if FTexture <> nil then
    FTexture.Free;
  FList.Free;
  FFont.Free;
  DeleteCriticalSection(FCriticalSection);
  inherited;
end;

procedure THGEFont.Lock;
begin
  EnterCriticalSection(FCriticalSection);
end;

procedure THGEFont.UnLock;
begin
  LeaveCriticalSection(FCriticalSection);
end;

procedure THGEFont.SetFont(Value:TFont);
begin
  FFont.Assign(Value);
end;

procedure THGEFont.Initialize;
var
  I, nX, nY, AWidth, AHeight:Integer;
begin
  Lock;
  try
    GetFontSize;

    FProcIdx := 0;
    AWidth := 512;
    AHeight := Ans(Min(FDoubleFontWidth * FDoubleFontHeight * FTextCount div AWidth + FDoubleFontHeight, 512));
    FColCount := AWidth div FDoubleFontWidth;
    FRowCount := AHeight div FDoubleFontHeight;

    SetLength(FImageRect, FColCount * FRowCount);

    for I := 0 to Length(FImageRect) - 1 do begin
      FImageRect[I].Active := False;
      FImageRect[I].Index := I;
      FImageRect[I].Time := MyGetTickCount;
      nX := I mod FColCount * FDoubleFontWidth;
      nY := I div FColCount * FDoubleFontHeight;
      FImageRect[I].Rect := Bounds(nX, nY, FDoubleFontWidth, FDoubleFontHeight);
    end;

    if FTexture = nil then
      FTexture := GameCanvas.HGE.Texture_Create(AWidth, AHeight);
    FInitialized := FTexture <> nil;
  finally
    UnLock;
  end;
  if FInitialized then
    InitEnglishString;
end;

procedure THGEFont.Finalize;
begin
  Lock;
  try
    FInitialized := False;
    SetLength(FImageRect, 0);
    FList.Clear;
    if FTexture <> nil then
      FreeAndNil(FTexture);
  finally
    UnLock;
  end;
end;

{
procedure THGEFont.Clear;
var
  I, nX, nY, AWidth, AHeight: Integer;
begin
  Lock;
  try
    FInitialized := False;
    SetLength(FImageRect, 0);
    FList.Clear;
    if FTexture <> nil then
      FreeAndNil(FTexture);

    GetFontSize;

    FProcIdx := 0;
    AWidth := 512;
    AHeight := Ans(Min(FDoubleFontWidth * FDoubleFontHeight * FTextCount div AWidth + FDoubleFontHeight, 512));
    FColCount := AWidth div FDoubleFontWidth;
    FRowCount := AHeight div FDoubleFontHeight;

    SetLength(FImageRect, FColCount * FRowCount);

    for I := 0 to Length(FImageRect) - 1 do
    begin
      FImageRect[I].Active := False;
      FImageRect[I].Index := I;
      FImageRect[I].Time := MyGetTickCount;
      nX := I mod FColCount * FDoubleFontWidth;
      nY := I div FColCount * FDoubleFontHeight;
      FImageRect[I].Rect := Bounds(nX, nY, FDoubleFontWidth, FDoubleFontHeight);
    end;

    if FTexture = nil then
      FTexture := GameCanvas.HGE.Texture_Create(AWidth, AHeight);
    FInitialized := FTexture <> nil;
  finally
    UnLock;
  end;
  if FInitialized then
    InitEnglishString;
end;
}

function THGEFont.GetCount:Integer;
begin
  Result := Length(FImageRect);
end;

function THGEFont.GetImages(Index:Integer):pTImageRect;
begin
  //Lock;
 // try
  if (Index >= 0) and (Index < Length(FImageRect)) and FImageRect[Index].Active then begin
    Result := @FImageRect[Index];
    Result.Time := MyGetTickCount;
  end
  else
    Result := nil;
  // finally
   //  UnLock;
   //end;
end;

function THGEFont.GetActive:pTImageRect;
var
  I, nX, nY:Integer;
begin
  Result := nil;
  for I := 0 to Length(FImageRect) - 1 do begin
    if not FImageRect[I].Active then begin
      FImageRect[I].Active := True;
      FImageRect[I].Time := MyGetTickCount;
      nX := I mod FColCount * FDoubleFontWidth;
      nY := I div FColCount * FDoubleFontHeight;

      FImageRect[I].X := I mod FColCount;
      FImageRect[I].Y := I div FColCount;

      FImageRect[I].Rect := Bounds(nX, nY, FDoubleFontWidth, FDoubleFontHeight);
      Result := @FImageRect[I];
      break;
    end;
  end;

  if Result = nil then begin
    for I := 0 to Length(FImageRect) - 1 do begin
      if MyGetTickCount - FImageRect[I].Time >= 1000 then begin
        FList.Remove(FImageRect[I].S);
        FImageRect[I].Active := True;
        FImageRect[I].Time := MyGetTickCount;
        nX := I mod FColCount * FDoubleFontWidth;
        nY := I div FColCount * FDoubleFontHeight;

        FImageRect[I].X := I mod FColCount;
        FImageRect[I].Y := I div FColCount;

        FImageRect[I].Rect := Bounds(nX, nY, FDoubleFontWidth, FDoubleFontHeight);
        Result := @FImageRect[I];
        break;
      end;
    end;
  end;
end;

procedure THGEFont.FreeOldMemorys;
var
  nIdx:Integer;
  ImageRect:pTImageRect;
  dwTimeTick:longword;
  boCheckTimeLimit:Boolean;
begin
  Lock;
  try
    nIdx := FProcIdx;
    boCheckTimeLimit := False;
    dwTimeTick := MyGetTickCount;
    while True do begin
      if (Count <= nIdx) {or (Count <= 94)} then Break;
      //if (Index >= 0) and (Index < Length(FImageRect)) and FImageRect[Index].Active then
     // ImageRect := Images[nIdx];
      ImageRect := @FImageRect[nIdx];
      if (ImageRect <> nil) and ImageRect.Active then begin
        if MyGetTickCount - ImageRect.Time > m_dwFreeMemCheckTime then begin
          ImageRect.Active := False;
          FList.Remove(ImageRect.S);
        end;
      end;
      Inc(nIdx);
      if (MyGetTickCount - dwTimeTick) > 10 then begin
        boCheckTimeLimit := True;
        FProcIdx := nIdx;
        Break;
      end;
    end;
    if not boCheckTimeLimit then FProcIdx := 0;
  finally
    UnLock;
  end;
end;

procedure THGEFont.GetFontSize;
var
  HHDC:HDC;
  TextSize:TSize;
  OldStyle:TFontStyles;
  abc1:TABC;
begin
  OldStyle := FFont.Style;

  if fsItalic in OldStyle then
    FFont.Style := [fsItalic]
  else
    FFont.Style := [];

  HHDC := CreateCompatibleDC(0);
  SelectObject(HHDC, FFont.Handle);

  Windows.GetTextExtentPoint32(HHDC, '8', 1, TextSize);
  FFontWidth := abs(TextSize.cx);
  FFontHeight := abs(TextSize.cy);

  // 斜体宽度修正
  if fsItalic in OldStyle then begin
    GetCharABCWidths(HHDC, Ord('8'), Ord('8'), abc1);
    FFontWidth := FFontWidth + abc1.abcA - abc1.abcC;
  end;

  Windows.GetTextExtentPoint32(HHDC, '字', 2, TextSize);
  FDoubleFontWidth := abs(TextSize.cx);
  FDoubleFontHeight := abs(TextSize.cy);

  // 斜体宽度修正
  if fsItalic in OldStyle then begin
    GetCharABCWidthsW(HHDC, Ord('8'), Ord('8'), abc1);
    FDoubleFontWidth := FDoubleFontWidth + abc1.abcA - abc1.abcC;
  end;

  DeleteDC(HHDC);

  FFont.Style := OldStyle;

  //DebugOutStr(Format('(1) FFontWidth:%d, FFontHeight:%d, FFontWidthBold:%d, FDoubleFontWidth:%d, FDoubleFontHeight:%d, FDoubleFontWidthBold:%d',
  //[FFontWidth, FFontHeight, FFontWidthBold, FDoubleFontWidth, FDoubleFontHeight, FDoubleFontWidthBold]));
end;

function THGEFont.TextHeight(const Text:string):Integer;
{var
  HHDC: HDC;
  TextSize: TSize;
begin
  HHDC := CreateCompatibleDC(0);

  SelectObject(HHDC, FFont.Handle);

  Windows.GetTextExtentPoint32(HHDC, PChar(Text), Length(Text), TextSize);
  Result := abs(TextSize.cy);

  DeleteDC(HHDC);
end;}
var
  sText:WideString;
begin
  sText := Text;
  if Length(Text) = Length(sText) then begin
    Result := FFontHeight;
  end
  else begin
    Result := Max(FFontHeight, FDoubleFontHeight);
  end;
end;

function THGEFont.TextWidth(const Text:string):Integer;
//var
//  HHDC: HDC;
//  TextSize: TSize;
//begin
//  HHDC := CreateCompatibleDC(0);
//
//  SelectObject(HHDC, FFont.Handle);
//
//  Windows.GetTextExtentPoint32(HHDC, PChar(Text), Length(Text), TextSize);
//  Result := abs(TextSize.cx);
//
//  DeleteDC(HHDC);
//end;
var
  nCount, nsCount, nwCount:Integer;
  sText:WideString;
begin
  sText := Text;
  nsCount := Length(Text);
  nwCount := Length(sText);
  if nsCount = nwCount then begin
    Result := FFontWidth * Length(Text);
  end
  else begin
    nCount := nsCount - nwCount; //双字节字符数
    Result := FDoubleFontWidth * Max(nCount, 0) + Max((nwCount - nCount), 0) * FFontWidth;
  end;
end;

function THGEFont.Add(const Text:WideChar):pTImageRect;
var
  nWidth, nHeight, X, Y:Integer;

  PBitmapBits:Pointer; // PIntegerArray;

  BitmapInfo:TBitmapInfo;
  HHBitmap:HBitmap;
  HHDC:HDC;

  Bits:Pointer;
  Pitch:Integer;

  SrcP:PByte;
  DesP:PByte;

  SrcRect:TRect;
  ImageRect:pTImageRect;

  {AGraphics: IGPGraphics;
  AFont: IGPFont;
  ABrush: IGPSolidBrush;}
begin
  Result := nil;
  if (not FInitialized) or (not GameCanvas.Active) then begin
    //DebugOutStr('(not FInitialized) or (not GameCanvas.Active):' + Text);
    Exit;
  end;

  ImageRect := GetActive;
  if ImageRect <> nil then begin
    // DebugOutStr(Format('(1) Left:%d, Top:%d, AWidth:%d, AHeight:%d', [ImageRect.Rect.Left, ImageRect.Rect.Top, ImageRect.Rect.Right, ImageRect.Rect.Bottom]));
    ImageRect.S := Text;
    nWidth := TextWidth(Text); //FDoubleFontWidth; //TextWidth(Text);
    nHeight := FDoubleFontHeight; //TextHeight(Text);
    //DebugOutStr(Format('AWidth:%d, AHeight:%d', [nWidth, nHeight]));
    FillChar(BitmapInfo, SizeOf(BitmapInfo), 0);

    with BitmapInfo.bmiHeader do begin
      //位图信息头
      biSize := SizeOf(TBitmapInfoHeader);
      biWidth := nWidth;
      biHeight := -nHeight;
      biPlanes := 1;
      biBitCount := 32;
      biCompression := BI_RGB;
    end;

    HHDC := CreateCompatibleDC(0);
    PBitmapBits := nil;
    HHBitmap := CreateDIBSection(HHDC, BitmapInfo, DIB_RGB_COLORS, PBitmapBits, 0, 0);
    if (HHBitmap <> 0) and (PBitmapBits <> nil) then begin
      //DebugOutStr(Format('(2) Left:%d, Top:%d, AWidth:%d, AHeight:%d', [ImageRect.Rect.Left, ImageRect.Rect.Top, ImageRect.Rect.Right, ImageRect.Rect.Bottom]));
      SelectObject(HHDC, FFont.Handle);
      SelectObject(HHDC, HHBitmap);
      SetTextColor(HHDC, RGB(255, 255, 255)); //设文字颜色为白色
      SetBkColor(HHDC, RGB(0, 0, 0)); //设背景颜色为黑色

      Windows.TextOutW(HHDC, 0, 0, PWideChar(@Text), Length(Text));

      {AGraphics := TGPGraphics.Create(HHDC);
      AGraphics.SmoothingMode := SmoothingModeAntiAlias8x8; //SmoothingModeAntiAlias;
      AGraphics.TextRenderingHint := TextRenderingHintAntiAlias;
      AFont := TGPFont.Create(FFont.Name, FFont.Size, FFont.Style);
      ABrush := TGPSolidBrush.Create($FFFFFFFF);
      AGraphics.DrawString(Text, AFont, Point(0, 0), ABrush);
            }

      SrcRect := ImageRect.Rect;

      nWidth := Min(SrcRect.Right - SrcRect.Left, nWidth);
      SrcRect := Bounds(SrcRect.Left, SrcRect.Top, nWidth, nHeight);
      ImageRect.Rect := SrcRect;
      //GameCanvas.HGE.Lock;
      //try
      try
        if FTexture.Lock(SrcRect, Bits, Pitch, False) then begin
          if (Bits <> nil) and (Pitch > 0) then begin
            for Y := 0 to nHeight - 1 do begin
              SrcP := PByte(Integer(PBitmapBits) + Y * nWidth * 4);
              DesP := PByte(Integer(Bits) + Y * Pitch);
              //FillChar(DesP^, Pitch, 0);
              for X := 0 to nWidth - 1 do begin
                if PCardinal(SrcP)^ <> 0 then
                  PCardinal(DesP)^ := PCardinal(SrcP)^ or $FF000000 //$FF000000;
                else
                  PCardinal(DesP)^ := $000000FF;

                Inc(SrcP, 4);
                Inc(DesP, 4);
              end;
            end;
            //DebugOutStr(Format('(3)Text:%s Left:%d, Top:%d, Right:%d, Bottom:%d', [Text, SrcRect.Left, SrcRect.Top, SrcRect.Right, SrcRect.Bottom]));

            FList.Add(Text, ImageRect);
            { ImageRect1 := FList.Find(Text);
             if ImageRect1 <> nil then
               DebugOutStr(Format('(4)Text:%s Left:%d, Top:%d, Right:%d, Bottom:%d', [Text, ImageRect1.Rect.Left, ImageRect1.Rect.Top, ImageRect1.Rect.Right, ImageRect1.Rect.Bottom]))
             else
               DebugOutStr(Format('(5)Text:%s Left:%d, Top:%d, Right:%d, Bottom:%d', [Text, SrcRect.Left, SrcRect.Top, SrcRect.Right, SrcRect.Bottom]));
             }
          end
          else begin
            ImageRect.Active := False;
            //DebugOutStr(Format('(6)Text:%s Left:%d, Top:%d, Right:%d, Bottom:%d', [Text, SrcRect.Left, SrcRect.Top, SrcRect.Right, SrcRect.Bottom]));
          end;
        end
        else begin
          ImageRect.Active := False;
          // DebugOutStr(Format('(7)Text:%s Left:%d, Top:%d, Right:%d, Bottom:%d', [Text, SrcRect.Left, SrcRect.Top, SrcRect.Right, SrcRect.Bottom]));
        end;
      finally
        FTexture.Unlock;
      end;
      //finally
        //GameCanvas.HGE.UnLock;
      //end;
      DeleteObject(HHBitmap);
    end
    else begin
      ImageRect.Active := False;
      //DebugOutStr(Format('(8)Text:%s Left:%d, Top:%d, Right:%d, Bottom:%d', [Text, SrcRect.Left, SrcRect.Top, SrcRect.Right, SrcRect.Bottom]));
    end;
    DeleteDC(HHDC);
  end
  else begin
    //DebugOutStr('(10)Text:' + Text);
  end;
  if (ImageRect <> nil) and (not ImageRect.Active) then begin
    ImageRect := nil;
    //DebugOutStr(Format('(9)Text:%s Left:%d, Top:%d, Right:%d, Bottom:%d', [Text, SrcRect.Left, SrcRect.Top, SrcRect.Right, SrcRect.Bottom]));
  end;
  Result := ImageRect;
end;

procedure THGEFont.InitEnglishString;
var
  I:Integer;
  C:Char;
  WS:WideString;
begin
  for I := 0 to 9 do {//0~9} begin
    WS := IntToStr(I);
    Add(WS[1]);
  end;

  for I := 97 to 122 do {//a~z      //10~25} begin
    C := Chr(I);
    Add(WideChar(C));
  end;

  for I := 65 to 90 do {//A~Z     //26~51} begin
    C := Chr(I);
    Add(WideChar(C));
  end;
  Add('~'); //52
  Add('!'); //52
  Add('@'); //52
  Add('#'); //52
  Add('$'); //52
  Add('%'); //52
  Add('^');
  Add('&');
  Add('^');
  Add('&');
  Add('*');
  Add('(');
  Add(')');
  Add('_');
  Add('+');
  Add('-');
  Add('=');
  Add('{');
  Add('}');
  Add('[');
  Add(']');
  Add('\');
  Add('|');
  Add('<');
  Add('>');
  Add('/');
  Add('?');
  Add(':');
  Add('.');
  Add(',');
  Add(';');
  Add(' ');
end;

function THGEFont.GetImageInfos(const Text:string):TImageInfos;
var
  I, II:Integer;
  ImageRect:pTImageRect;
  ImageInfo:pTImageInfo;
  sText:WideString;
  S:WideChar;
  LineText:string;
  TextList:TStringList;
begin
  SetLength(Result, 0);
  if (not FInitialized) or (not GameCanvas.Active) then Exit;
  Lock;
  try
    TextList := TStringList.Create;
    try
      TextList.Text := Text;
      for I := 0 to TextList.Count - 1 do begin
        SetLength(Result, Length(Result) + 1);
        LineText := TextList.Strings[I];
        if Length(LineText) > 0 then begin
          ImageInfo := @Result[Length(Result) - 1];

          sText := LineText;
          for II := 1 to Length(sText) do begin
            S := sText[II];
            ImageRect := FList.Find(S);
            //if ImageRect <> nil then
            //  DebugOutStr(Format('GetImageInfos Text:%s Left:%d, Top:%d, Right:%d, Bottom:%d', [S, ImageRect.Rect.Left, ImageRect.Rect.Top, ImageRect.Rect.Right, ImageRect.Rect.Bottom]))

           // else
            //  DebugOutStr(Format('GetImageInfos Text:%s', [S]));

            // 修改 chongchong 2015-02-13
            // if ImageRect <> nil) and (ImageRect.Active) then
            if (ImageRect <> nil) {and (ImageRect.Active)} then begin
              // ++++ chongchong 2015-02-13
              ImageRect.Active := True;

              ImageRect.Time := MyGetTickCount;
              ImageInfo.Width := ImageInfo.Width + Max(ImageRect.Rect.Right - ImageRect.Rect.Left, 0);
              ImageInfo.Height := Max(ImageInfo.Height, (ImageRect.Rect.Bottom - ImageRect.Rect.Top));
              SetLength(ImageInfo.ImageIndexs, Length(ImageInfo.ImageIndexs) + 1);
              ImageInfo.ImageIndexs[Length(ImageInfo.ImageIndexs) - 1] := ImageRect.Index;
            end
            else begin
              // ----- chongchong 2015-02-13
              //if (ImageRect <> nil) and (not ImageRect.Active) then
              //  FList.Remove(ImageRect.S);

              ImageRect := Add(S);
              if ImageRect <> nil then begin

                ImageInfo.Width := ImageInfo.Width + Max(ImageRect.Rect.Right - ImageRect.Rect.Left, 0);
                ImageInfo.Height := Max(ImageInfo.Height, (ImageRect.Rect.Bottom - ImageRect.Rect.Top));
                SetLength(ImageInfo.ImageIndexs, Length(ImageInfo.ImageIndexs) + 1);
                ImageInfo.ImageIndexs[Length(ImageInfo.ImageIndexs) - 1] := ImageRect.Index;
              end
              else begin
                //DebugOutStr(Format('GetImageInfos Fail Text:%s', [S]));

                ImageInfo.Width := ImageInfo.Width + TextWidth('0');
                // Result.Height := Max(Result.Height, TextHeight('0'));
                SetLength(ImageInfo.ImageIndexs, Length(ImageInfo.ImageIndexs) + 1);
                ImageInfo.ImageIndexs[Length(ImageInfo.ImageIndexs) - 1] := -1;
              end;
            end;
          end;
        end;
      end;
    finally
      TextList.Free;
    end;
  finally
    UnLock;
  end;
end;

function THGEFont.GetImageInfo(const Text:string):TImageInfo;
var
  I:Integer;
  ImageRect:pTImageRect;
  sText:WideString;
  S:WideChar;
  //ImageInfo:TImageInfo;
begin
  Result.Width := 0;
  Result.Height := 0;
  SetLength(Result.ImageIndexs, 0);
  if (not FInitialized) or (not GameCanvas.Active) then Exit;
  Lock;
  try
    if Length(Text) > 0 then begin
      sText := Text;
      for I := 1 to Length(sText) do begin
        S := sText[I];
        ImageRect := FList.Find(S);

        if (ImageRect <> nil) and (ImageRect.Active) then begin
          ImageRect.Time := MyGetTickCount;
          Result.Width := Result.Width + Max(ImageRect.Rect.Right - ImageRect.Rect.Left, 0);
          Result.Height := Max(Result.Height, (ImageRect.Rect.Bottom - ImageRect.Rect.Top));
          SetLength(Result.ImageIndexs, Length(Result.ImageIndexs) + 1);
          Result.ImageIndexs[Length(Result.ImageIndexs) - 1] := ImageRect.Index;
        end
        else begin
          if (ImageRect <> nil) and (not ImageRect.Active) then
            FList.Remove(ImageRect.S);

          ImageRect := Add(S);
          if ImageRect <> nil then begin
            Result.Width := Result.Width + Max(ImageRect.Rect.Right - ImageRect.Rect.Left, 0);
            Result.Height := Max(Result.Height, (ImageRect.Rect.Bottom - ImageRect.Rect.Top));
            SetLength(Result.ImageIndexs, Length(Result.ImageIndexs) + 1);
            Result.ImageIndexs[Length(Result.ImageIndexs) - 1] := ImageRect.Index;
          end
          else begin
            Result.Width := Result.Width + TextWidth('0');
            // Result.Height := Max(Result.Height, TextHeight('0'));
            SetLength(Result.ImageIndexs, Length(Result.ImageIndexs) + 1);
            Result.ImageIndexs[Length(Result.ImageIndexs) - 1] := -1;
          end;
        end;
      end;
    end;
  finally
    UnLock;
  end;
end;

procedure THGEFont.TextRect(const X, Y:Integer; SrcRect:TRect; const Text:string; Color:TColor; BlendMode:Integer; Alpha:Byte);
var
  ImageInfos:TImageInfos;
begin
  if (not FInitialized) or (not GameCanvas.Active) then Exit;
  ImageInfos := GetImageInfos(Text);
  TextRect(X, Y, SrcRect, ImageInfos, Color, BlendMode, Alpha);
end;

//IntersectRect

procedure THGEFont.TextRect(const X, Y:Integer; SrcRect:TRect; ImageInfos:TImageInfos; Color:TColor; BlendMode:Integer; Alpha:Byte; ExpandLineHeight:Integer);
var
  I, II:Integer;
  nX, nY, nOffsetX, nOffsetY, nWidth, nHeight, nH:Integer;
  ImageRect:pTImageRect;
  PaintRect:TRect;
begin
  if (not FInitialized) or (not GameCanvas.Active) then Exit;
  if (SrcRect.Right > SrcRect.Left) and (SrcRect.Bottom > SrcRect.Top) then begin
    Lock;
    try
      nY := 0;
      nOffsetY := SrcRect.Top;
      nHeight := SrcRect.Bottom - SrcRect.Top;
      for I := 0 to Length(ImageInfos) - 1 do begin
        if nHeight <= 0 then break;
        if ImageInfos[I].Height <= 0 then begin
          if nOffsetY >= TextHeight('0') then begin
            Dec(nOffsetY, TextHeight('0'));
            Continue;
          end;

          if nOffsetY > 0 then begin
            Inc(nY, TextHeight('0') - nOffsetY);
            Dec(nHeight, TextHeight('0') - nOffsetY);
            nOffsetY := 0;
            Continue;
          end;

          Inc(nY, TextHeight('0'));
          Dec(nHeight, TextHeight('0'));
          Continue;
        end;

        if (nOffsetY > 0) and (nOffsetY >= ImageInfos[I].Height) then begin
          Dec(nOffsetY, ImageInfos[I].Height);
          Continue;
        end;

        nX := 0;
        nWidth := SrcRect.Right - SrcRect.Left;
        nOffsetX := SrcRect.Left;
        nH := ImageInfos[I].Height - nOffsetY;

        for II := 0 to Length(ImageInfos[I].ImageIndexs) - 1 do begin
          if nWidth <= 0 then break;
          ImageRect := Images[ImageInfos[I].ImageIndexs[II]];
          if (ImageRect <> nil) and (ImageRect.Rect.Right > ImageRect.Rect.Left) then begin
            if (nOffsetX > 0) then begin
              if (nOffsetX >= ImageRect.Rect.Right - ImageRect.Rect.Left) then begin
                nOffsetX := nOffsetX - (ImageRect.Rect.Right - ImageRect.Rect.Left);
                Continue;
              end;
            end;

            PaintRect := Bounds(ImageRect.Rect.Left + nOffsetX, ImageRect.Rect.Top + nOffsetY, Min(ImageRect.Rect.Right - ImageRect.Rect.Left - nOffsetX, nWidth), Min(ImageRect.Rect.Bottom - ImageRect.Rect.Top - nOffsetY, nHeight));
            if (PaintRect.Right > PaintRect.Left) and (PaintRect.Bottom > PaintRect.Top) then begin
              ImageRect.Time := MyGetTickCount;
              GameCanvas.DrawColorEx(X + nX, Y + nY, PaintRect, FTexture, Color, Alpha, BlendMode);

              {
              if (PaintRect.Right - PaintRect.Left > 200) or (PaintRect.Bottom - PaintRect.Top > 200) then
              begin
                DebugOutStr(Format('TextRect Size Error %s(%d, %d),(%d, %d)', [ImageRect.S, PaintRect.Left, PaintRect.Top, PaintRect.Right, PaintRect.Bottom]));
              end;
              }
            end;
            Inc(nX, PaintRect.Right - PaintRect.Left);
            Dec(nWidth, PaintRect.Right - PaintRect.Left);
            nH := Min(nH, PaintRect.Bottom - PaintRect.Top);
            nOffsetX := 0;

          end
          else begin
            if (nOffsetX > 0) then begin
              if nOffsetX >= TextWidth('0') then begin
                nOffsetX := nOffsetX - TextWidth('0');
                Continue;
              end;
              Inc(nX, TextWidth('0') - nOffsetX);
              Dec(nWidth, TextWidth('0') - nOffsetX);
              nOffsetX := 0;
              Continue;
            end;
            Inc(nX, TextWidth('0'));
            Dec(nWidth, TextWidth('0'));
          end;
        end; //for II := 0 to Length(ImageInfos[I].ImageIndexs) - 1 do begin
        Inc(nY, nH + ExpandLineHeight);
        Dec(nHeight, nH);
        nOffsetY := 0;
      end;
    finally
      UnLock;
    end;
  end;
end;

procedure THGEFont.TextRect(const X, Y:Integer; SrcRect:TRect; const ImageIndexs:TImageIndexs; Color:TColor; BlendMode:Integer; Alpha:Byte);
var
  I:Integer;
  nX, nOffsetX, nOffsetY, nWidth, nHeight:Integer;
  ImageRect:pTImageRect;
  PaintRect:TRect;
begin
  if (not FInitialized) or (not GameCanvas.Active) then Exit;
  if (SrcRect.Right > SrcRect.Left) and (SrcRect.Bottom > SrcRect.Top) then begin
    Lock;
    try
      nX := 0;

      nOffsetY := SrcRect.Top;
      nHeight := SrcRect.Bottom - SrcRect.Top;

      nWidth := SrcRect.Right - SrcRect.Left;
      nOffsetX := SrcRect.Left;

      for I := 0 to Length(ImageIndexs) - 1 do begin
        if nWidth <= 0 then break;
        ImageRect := Images[ImageIndexs[I]];
        if (ImageRect <> nil) and (ImageRect.Rect.Right > ImageRect.Rect.Left) then begin
          if (nOffsetX > 0) then begin
            if (nOffsetX >= ImageRect.Rect.Right - ImageRect.Rect.Left) then begin
              nOffsetX := nOffsetX - (ImageRect.Rect.Right - ImageRect.Rect.Left);
              Continue;
            end;
          end;
          ImageRect.Time := MyGetTickCount;

          PaintRect := Bounds(ImageRect.Rect.Left + nOffsetX, ImageRect.Rect.Top + nOffsetY, Min(ImageRect.Rect.Right - ImageRect.Rect.Left - nOffsetX, nWidth), Min(ImageRect.Rect.Bottom - ImageRect.Rect.Top - nOffsetY, nHeight));

          if (PaintRect.Right > PaintRect.Left) and (PaintRect.Bottom > PaintRect.Top) then begin
            GameCanvas.DrawColorEx(X + nX, Y, PaintRect, FTexture, Color, Alpha, BlendMode);

            {
            if (PaintRect.Right - PaintRect.Left > 200) or (PaintRect.Bottom - PaintRect.Top > 200) then
            begin
              DebugOutStr(Format('TextRect2 Size Error %s(%d, %d),(%d, %d)', [ImageRect.S, PaintRect.Left, PaintRect.Top, PaintRect.Right, PaintRect.Bottom]));
            end;
            }
            Inc(nX, PaintRect.Right - PaintRect.Left);
            Dec(nWidth, PaintRect.Right - PaintRect.Left);
          end;

          nOffsetX := 0;

        end
        else begin
          if (nOffsetX > 0) then begin
            if nOffsetX >= TextWidth('0') then begin
              nOffsetX := nOffsetX - TextWidth('0');
              Continue;
            end;
            Inc(nX, TextWidth('0') - nOffsetX);
            Dec(nWidth, TextWidth('0') - nOffsetX);
            nOffsetX := 0;
            Continue;
          end;
          Inc(nX, TextWidth('0'));
          Dec(nWidth, TextWidth('0'));
        end;
      end;
    finally
      UnLock;
    end;
  end;
end;

procedure THGEFont.TextOut(const X, Y:Integer; const Text:string; Color:TColor; BlendMode:Integer; Alpha:Byte);
var
  I, nWidth, nHeight:Integer;
  ImageInfos:TImageInfos;
begin
  if (not FInitialized) or (not GameCanvas.Active) then Exit;
  try
    Lock;

    nWidth := 0;
    nHeight := 0;
    ImageInfos := GetImageInfos(Text);
    for I := 0 to Length(ImageInfos) - 1 do begin
      if nWidth < ImageInfos[I].Width then
        nWidth := ImageInfos[I].Width;

      if ImageInfos[I].Height <= 0 then
        Inc(nHeight, TextHeight('0'))
      else
        Inc(nHeight, ImageInfos[I].Height);
    end;
    if (nWidth > 0) and (nHeight > 0) then
      TextRect(X, Y, Rect(0, 0, nWidth, nHeight), ImageInfos, Color, BlendMode, Alpha);
  finally
    UnLock;
  end;
end;

procedure THGEFont.TextOut(const X, Y:Integer; ImageIndexs:TImageIndexs; Color:TColor; BlendMode:Integer; Alpha:Byte);
var
  I, nX, nIndex:Integer;
  ImageRect:pTImageRect;
begin
  if (not FInitialized) or (not GameCanvas.Active) then Exit;
  Lock;
  //try
  nX := X;
  for I := 0 to Length(ImageIndexs) - 1 do begin
    nIndex := ImageIndexs[I];
    ImageRect := Images[nIndex];
    if (ImageRect <> nil) then begin
      ImageRect.Time := MyGetTickCount;
      if (ImageRect.Rect.Right > ImageRect.Rect.Left) and (ImageRect.Rect.Bottom > ImageRect.Rect.Top) then begin
        GameCanvas.DrawColorEx(nX, Y, ImageRect.Rect, FTexture, Color, Alpha, BlendMode);

        {
        if (ImageRect.Rect.Right - ImageRect.Rect.Left > 200) or (ImageRect.Rect.Bottom - ImageRect.Rect.Top > 200) then
        begin
          DebugOutStr(Format('TextOut Size Error %s(%d, %d),(%d, %d)', [ImageRect.S, ImageRect.Rect.Left, ImageRect.Rect.Top, ImageRect.Rect.Right, ImageRect.Rect.Bottom]));
        end;
        }
        Inc(nX, ImageRect.Rect.Right - ImageRect.Rect.Left);
      end
      else begin
        Inc(nX, TextWidth('0'));
      end;
    end
    else begin
      Inc(nX, TextWidth('0'));
    end;
  end;
  // finally
  UnLock;
  // end;
end;
{------------------------------------------------------------------------------}

constructor THGEFonts.Create();
begin
  InitializeCriticalSection(FCriticalSection);
  FList := TList.Create;
end;

destructor THGEFonts.Destroy;
var
  I:Integer;
begin
  for I := 0 to FList.Count - 1 do
    THGEFont(FList.Items[I]).Free;
  FList.Free;
  DeleteCriticalSection(FCriticalSection);
end;

function THGEFonts.TryLock:Boolean;
begin
  Result := TryEnterCriticalSection(FCriticalSection);
end;

procedure THGEFonts.Lock;
begin
  EnterCriticalSection(FCriticalSection);
end;

procedure THGEFonts.UnLock;
begin
  LeaveCriticalSection(FCriticalSection);
end;

procedure THGEFonts.Initialize;
var
  I:Integer;
begin
  Lock;
  try
    for I := 0 to FList.Count - 1 do
      THGEFont(FList.Items[I]).Initialize;
  finally
    UnLock;
  end;
end;

procedure THGEFonts.Finalize;
var
  I:Integer;
begin
  Lock;
  try
    for I := 0 to FList.Count - 1 do
      THGEFont(FList.Items[I]).Finalize;
  finally
    UnLock;
  end;
end;

procedure THGEFonts.FreeOldMemorys;
var
  I:Integer;
begin
  if TryLock then begin
    try
      for I := 0 to FList.Count - 1 do
        THGEFont(FList.Items[I]).FreeOldMemorys;
    finally
      UnLock;
    end;
  end;
end;

procedure THGEFonts.Add(AFont:THGEFont);
begin
  Lock;
  try
    FList.Add(AFont);
  finally
    UnLock;
  end;
end;

{
procedure THGEFonts.Clear;
var
  I: Integer;
begin
  Lock;
  try
    for I := 0 to FList.Count - 1 do
      THGEFont(FList.Items[I]).Clear;
  finally
    UnLock;
  end;
end;
}

function THGEFonts.FindFont(AFontName:string; AFontSize:Integer; AFontStyles:TFontStyles):THGEFont;
var
  I:Integer;
  HGEFont:THGEFont;
  boFind:Boolean;
begin
  Lock;
  // try
  Result := nil;
  if AFontName = '' then AFontName := '宋体';
  boFind := False;
  for I := 0 to FList.Count - 1 do begin
    HGEFont := THGEFont(FList.Items[I]);
    if (CompareText(HGEFont.Font.Name, AFontName) = 0) and
      (HGEFont.Font.Size = AFontSize) and
      (HGEFont.Font.Style = AFontStyles) then begin
      Result := HGEFont;
      boFind := True;
      break;
    end;
  end;

  if not boFind then begin
    HGEFont := THGEFont.Create(300);
    HGEFont.Font.Name := AFontName;
    HGEFont.Font.Size := AFontSize;
    HGEFont.Font.Style := AFontStyles;
    HGEFont.m_dwFreeMemCheckTime := 1000 * 60 * 2;
    HGEFont.Initialize;
    FList.Add(HGEFont);
    Result := HGEFont;
  end;
  // finally
  UnLock;
  //end;
end;

procedure THGEFont.SaveTextureToStream(Stream:TStream);
var
  DIB:TDIB;
  Bits:Pointer;
  Pitch, nY:Integer;
  SrcP, DestP:PByte;
begin
  DIB := TDIB.Create;
  try
    DIB.PixelFormat := MakeDIBPixelFormat(8, 8, 8);
    DIB.SetSize(FTexture.Width, FTexture.Height, 32);

    if FTexture.Lock(Bits, Pitch, False) and (Bits <> nil) and (Pitch > 0) then begin
      for nY := 0 to FTexture.Height - 1 do begin
        SrcP := PByte(Integer(Bits) + nY * Pitch);
        DestP := DIB.ScanLine[nY];

        Move(SrcP^, DestP^, FTexture.Width * 4);
      end;
      FTexture.Unlock;
    end;

    DIB.SaveToStream(Stream);
  finally
    DIB.Free;
  end;
end;

end.
