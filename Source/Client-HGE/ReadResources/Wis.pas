unit Wis;

interface
uses
  Windows,
  Classes,
  Graphics,
  SysUtils,
  DIB,
  HGE,
  DxCanvas,
  GameImages,
  MapFiles;
type
  TWisFileHeaderInfo = packed record
    nTitle:Integer; // 04 $41534957 = WISA
    VerFlag:Integer; // 01
    Reserve1:Integer; // 00
    DateTime:TDateTime;
    Reserve2:Integer;
    Reserve3:Integer;
    CopyRight:string[20];
    aTemp1:array[1..107] of Char;
    nHeaderEncrypt:Integer; // 0XA0
    nHeaderLen:Integer; // 0XA4
    nImageCount:Integer; // 0XA8
    nHeaderData:Integer; // 0XAC

    aTemp2:array[1..$200 - $AC] of Char;
  end;
  PWisFileHeaderInfo = ^TWisFileHeaderInfo;

  TWisHeader = packed record
    OffSet:Integer;
    Length:Integer;
    temp3:Integer;
  end;
  PTWisHeader = ^TWisHeader;
  TWisFileHeaderArray = array of TWisHeader;

  TImgInfo = packed record
    btEncr0:Byte; // 0X00
    btEncr1:Byte; // 0X01
    bt2:Byte; // 0X02
    bt3:Byte; // 0X03
    wW:Smallint; // 0X04
    wH:Smallint; // 0X06
    wPx:Smallint; // 0X08
    wPy:Smallint; // 0X0A
  end;
  PTImgInfo = ^TImgInfo;

  TWisImages = class(TGameImages)
  private
    FIndexOffset:Integer;
    {$IF USEMAPSTREAM = 0}
    FileStream:TFileStream;
    {$ELSE}
    FileStream:TMapStream;
    {$IFEND}
    IndexArray:TWisFileHeaderArray;
    //function Decode(ASrc: PByte; DIB: TDIB; ASrcSize: Integer): Integer;
    function DecodeWis(ASrc, ADst:PByte; ASrcSize, ADstSize:Integer):Boolean; overload;
    function DecodeWis(ASrc:PByte; ASrcSize, Awidth, AHeight:Integer; Source:TDIB):Boolean; overload;
    function LoadIndex:Boolean;

    procedure LoadDxImage(WisHeader:PTWisHeader; DXImage:pTDXImage);
    procedure LoadDxGrayImage(WisHeader:PTWisHeader; DXImage:pTDXImage);
    procedure LoadDxBrightImage(WisHeader:PTWisHeader; DXImage:pTDXImage);

    procedure LoadDxBitmap(WisHeader:PTWisHeader; DXImage:pTDXImage);
  protected
    function GetCachedSurface(Index:Integer):TTexture; override;
    function GetCachedGray(Index:Integer):TTexture; override;
    function GetCachedBright(Index:Integer):TTexture; override;

    function GetCachedBitmap(Index:Integer):TBitmap; override;
  public
    constructor Create();
    destructor Destroy; override;

    procedure Initialize; override;
    procedure Finalize; override;
    function GetCachedImage(Index:Integer; var PX, PY:Integer):TTexture; override;
    function GetCachedGrayImage(Index:Integer; var px, py:Integer):TTexture; override;
    function GetCachedBrightImage(Index:Integer; var px, py:Integer):TTexture; override;
    function GetBitmap(Index:Integer; var PX, PY:Integer):TBitmap; override;

    function GetCachedImageSize(AIndex:Integer; var ASize:TSize; var APoint:TPoint):Boolean; override;
  end;

implementation
uses Math; // , MShare;

function SGL_RLE8_Decode(ASrc, ADst:PByte; ASrcSize,
  ADstSize:Integer):Boolean;
var
  L, I:Byte;
begin
  while (ASrcSize > 0) and (ADstSize > 0) do begin
    if (PByte(ASrc)^ and $80) = 0 then begin // 0..127
      L := ASrc^;
      Inc(ASrc);
      Dec(ASrcSize, 2);
      if L > ADstSize then L := ADstSize;
      Dec(ADstSize, L);
      for I := 1 to L do begin
        ADst^ := ASrc^;
        Inc(ADst);
      end;
      Inc(ASrc);
    end
    else begin
      L := PByte(ASrc)^ and $7F;
      Inc(PByte(ASrc));
      Dec(ASrcSize, L + 1);
      if L > ADstSize then L := ADstSize;
      Dec(ADstSize, L);
      for I := 1 to L do begin
        ADst^ := ASrc^;
        Inc(ADst);
        Inc(ASrc);
      end;
    end;
  end;
  Result := True;
end;

{
function TWisImages.Decode(ASrc: PByte; DIB: TDIB; ASrcSize: Integer): Integer;
var
  V, Len, Len1, I: Byte;
  ADstSize: Integer;
  boSkip: Boolean;
  X, Y: Integer;

  function WritePixel(Color: Byte): Boolean;
  var
    Row: PByteArray;
  begin
    if X = DIB.Height then
    begin
      Result := True;
      Exit;
    end;
    Row := DIB.ScanLine[X];
    Row[Y] := Color;
    Y := Y + 1;
    Result := False;
    if Y = DIB.Width then
    begin
      Y := 0;
      X := X + 1;
      Result := True;
    end;
  end;
begin
  Result := 0;
  Len := 0;
  X := 0;
  Y := 0;
  ADstSize := 0;
  boSkip := False;
  while ADstSize < ASrcSize do
  begin
    if not boSkip then
    begin
      V := ASrc^;
      Inc(PByte(ASrc));
      Inc(Result);
    end;
    if (V = 0) and (Len <= 0) then
    begin
      V := ASrc^;
      Len := V;
      Inc(PByte(ASrc));
      Inc(Result);
      boSkip := True;
    end;
    if boSkip then
    begin
      if Len <> 0 then
      begin
        Inc(ADstSize, Len);
        Inc(Result, Len);
        // Move(ASrc^, ADst^, Len);
        for I := 1 to Len do
        begin
          WritePixel(ASrc^);
          Inc(PByte(ASrc));
        end;
        // Inc(PByte(ASrc), Len);
        // Inc(PByte(ADst), Len);
        Len := 0;
      end
      else
      begin
        boSkip := False;
      end;
    end
    else
    begin
      Len1 := V;
      Inc(Result);
      Inc(ADstSize, Len1);
      for I := 1 to Len1 do
      begin
        WritePixel(ASrc^);
        // ADst^ := ASrc^;
        // Inc(PByte(ADst));
      end;
      Inc(PByte(ASrc));
    end;
  end;
end;
}

function TWisImages.DecodeWis(ASrc, ADst:PByte; ASrcSize,
  ADstSize:Integer):Boolean;
var
  V, Len, L, I:Byte;
  boSkip:Boolean;
begin
  boSkip := False;
  Len := 0;
  while (ASrcSize > 0) and (ADstSize > 0) do begin
    if not boSkip then begin
      V := ASrc^;
      Dec(ASrcSize);
      Inc(PByte(ASrc));
    end;
    if (V = 0) and (Len <= 0) then begin
      V := ASrc^;
      Len := V;
      Dec(ASrcSize);
      Inc(PByte(ASrc));
      boSkip := True;
    end;
    if boSkip then begin
      if Len <> 0 then begin
        Dec(ADstSize, Len);
        Dec(ASrcSize, Len);
        Move(ASrc^, ADst^, Len);
        Inc(PByte(ADst), Len);
        Inc(PByte(ASrc), Len);
      end
      else begin
        boSkip := False;
      end;
      Len := 0;
    end
    else begin
      L := V;
      Dec(ASrcSize);
      Dec(ADstSize, L);
      V := ASrc^;
      Inc(PByte(ASrc));
      for I := 1 to L do begin
        ADst^ := V;
        Inc(ADst);
      end;
    end;
  end;

  Result := True;
end;

function TWisImages.DecodeWis(ASrc:PByte; ASrcSize, AWidth, AHeight:Integer; Source:TDIB):Boolean;
var
  V, Len, L, I:Byte;
  boSkip:Boolean;
  ADst:PByte;
  ADstSize:Integer;
  nWidth, nHeight:Integer;

  procedure WriteData(PBits:PByte; WriteLen:Integer);
  begin
    if nHeight < AHeight then begin
      if nWidth + WriteLen <= AWidth then begin
        Move(PBits^, ADst^, WriteLen);
        Inc(PByte(ADst), WriteLen);
        Inc(nWidth, WriteLen);
      end;
      if nWidth >= AWidth then begin
        Inc(nHeight);
        nWidth := 0;
        if nHeight < AHeight then
          ADst := Source.ScanLine[nHeight];
      end;
    end;
  end;

  procedure WritePixel(Color:Byte);
  begin
    if nHeight < AHeight then begin
      if nWidth + 1 <= AWidth then begin
        ADst^ := Color;
        Inc(PByte(ADst));
        Inc(nWidth);
      end;
      if nWidth >= AWidth then begin
        Inc(nHeight);
        nWidth := 0;
        if nHeight < AHeight then
          ADst := Source.ScanLine[nHeight];
      end;
    end;
  end;

begin
  boSkip := False;
  Len := 0;

  nWidth := 0;
  nHeight := 0;
  ADstSize := AWidth * AHeight;
  ADst := Source.ScanLine[nHeight];
  while (ASrcSize > 0) and (ADstSize > 0) do begin
    if not boSkip then begin
      V := ASrc^;
      Dec(ASrcSize);
      Inc(PByte(ASrc));
    end;
    if (V = 0) and (Len <= 0) then begin
      V := ASrc^;
      Len := V;
      Dec(ASrcSize);
      Inc(PByte(ASrc));
      boSkip := True;
    end;
    if boSkip then begin
      if Len <> 0 then begin
        Dec(ADstSize, Len);
        Dec(ASrcSize, Len);
        WriteData(ASrc, Len);
        // Move(ASrc^, ADst^, Len);
        // Inc(PByte(ADst), Len);
        Inc(PByte(ASrc), Len);
      end
      else begin
        boSkip := False;
      end;
      Len := 0;
    end
    else begin
      L := V;
      Dec(ASrcSize);
      Dec(ADstSize, L);
      V := ASrc^;
      Inc(PByte(ASrc));
      for I := 1 to L do begin
        // ADst^ := V;
        WritePixel(V);
        // Inc(ADst);
      end;
    end;
  end;

  Result := True;
end;

constructor TWisImages.Create();
begin
  inherited Create;
  FileStream := nil;
  IndexArray := nil;
end;

destructor TWisImages.Destroy;
begin
  inherited;
end;

procedure TWisImages.Initialize;
begin
  if not Initialized then begin
    if FileExists(FileName) then begin
      if LoadIndex then begin
        {$IF USEMAPSTREAM = 0}
        FileStream := TFileStream.Create(FileName, fmOpenRead or fmShareDenyNone);
        {$ELSE}
        FileStream := TMapStream.Create;
        FileStream.LoadFromFile(FileName);
        {$IFEND}
        m_ImgArr := AllocMem(SizeOf(TDXImage) * ImageCount);
        Initialized := True;
      end;
    end;
  end;
end;

procedure TWisImages.Finalize;
var
  I:Integer;
begin
  Initialized := False;

  Lock;
  try
    IndexList.Clear;
    GrayIndexList.Clear;
    BrightIndexList.Clear;
    if m_ImgArr <> nil then begin
      for I := 0 to ImageCount - 1 do begin
        if m_ImgArr[I].Surface <> nil then begin
          try
            FreeAndNil(m_ImgArr[I].Surface);
          except
            DebugTextOut('[Exception] Texture.Free');
          end;
        end;

        if m_ImgArr[I].Gray <> nil then begin
          try
            FreeAndNil(m_ImgArr[I].Gray);
          except
            DebugTextOut('[Exception] Texture.Free');
          end;
        end;

        if m_ImgArr[I].Bright <> nil then begin
          try
            FreeAndNil(m_ImgArr[I].Bright);
          except
            DebugTextOut('[Exception] Texture.Free');
          end;
        end;

        if m_ImgArr[I].Bitmap <> nil then begin
          FreeAndNil(m_ImgArr[I].Bitmap);
        end;
      end;
      FreeMem(m_ImgArr);
    end;
    m_ImgArr := nil;
    IndexArray := nil;
    ImageCount := 0;
    if FileStream <> nil then
      FreeAndNil(FileStream);
  finally
    UnLock;
  end;
end;

function TWisImages.LoadIndex:Boolean;

  function DecPointer(P:Pointer; Size:Integer):Pointer;
  begin
    Result := Pointer(Integer(P) - Size);
  end;
var
  iFileOffset, nIndex, nIndexOffset:Integer;

  WisHeader:pTWisHeader;
  WisIndexArray:TWisFileHeaderArray;
  MapStream:TMapStream;
begin
  Result := False;

  MapStream := TMapStream.Create;
  if MapStream.LoadFromFile(FileName) then begin
    iFileOffset := 512;
    nIndexOffset := MapStream.Size;
    WisHeader := Pointer(Integer(MapStream.Memory) + MapStream.Size);
    while True do begin
      if nIndexOffset > iFileOffset then begin
        Dec(nIndexOffset, SizeOf(TWisHeader));
        WisHeader := DecPointer(WisHeader, SizeOf(TWisHeader));
        if (WisHeader.OffSet >= iFileOffset) and (WisHeader.Length >= 1) then begin
          SetLength(WisIndexArray, Length(WisIndexArray) + 1);
          WisIndexArray[Length(WisIndexArray) - 1] := WisHeader^;
          if (WisHeader.OffSet <= iFileOffset) then begin
            FIndexOffset := nIndexOffset;
            break;
          end;
        end
        else
          break;
      end
      else begin
        FIndexOffset := nIndexOffset;
        break;
      end;
    end;

    SetLength(IndexArray, Length(WisIndexArray));
    for nIndex := 0 to Length(WisIndexArray) - 1 do begin
      IndexArray[nIndex] := WisIndexArray[Length(WisIndexArray) - nIndex - 1];
    end;
    ImageCount := Length(IndexArray);

    Result := True;
  end;
  MapStream.Free;
end;

function TWisImages.GetCachedImage(Index:Integer; var PX, PY:Integer):TTexture;
begin
  Result := nil;
  Lock;
  try
    if Initialized and (Index >= 0) and (Index < ImageCount) then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Surface = nil then begin
        LoadDxImage(@IndexArray[Index], @m_ImgArr[Index]);
        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;
        m_ImgArr[Index].dwLatestTime := MyGetTickCount;

        if m_ImgArr[Index].Surface <> nil then
          IndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Surface;
      end
      else begin
        m_ImgArr[Index].dwLatestTime := MyGetTickCount;
        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;
        Result := m_ImgArr[Index].Surface;
      end;
    end;
  finally
    UnLock;
  end;
end;

function TWisImages.GetCachedImageSize(AIndex:Integer; var ASize:TSize; var APoint:TPoint):Boolean;
var
  {$IF USEMAPSTREAM = 0}
  ImgInfo:TImgInfo;
  {$ELSE}
  ImgInfo:pTImgInfo;
  {$IFEND}
begin
  Result := False;
  if (AIndex >= 0) and (AIndex < ImageCount) and (AIndex < m_IndexList.Count) and (FileStream <> nil) and (Initialized) then begin
    if m_ImgArr[AIndex].nWidth * m_ImgArr[AIndex].nHeight = 0 then begin
      if (IndexArray[AIndex].OffSet > 0) and (IndexArray[AIndex].OffSet < FileStream.Size) then begin
        {$IF USEMAPSTREAM = 0}
        FileStream.Position := IndexArray[AIndex].OffSet;
        FileStream.Read(ImgInfo, SizeOf(ImgInfo));
        {$ELSE}
        ImgInfo := pTImgInfo(Integer(FileStream.Memory) + WisHeader.OffSet);
        FileData := Pointer(Integer(ImgInfo) + SizeOf(TImgInfo));
        {$IFEND}
        m_ImgArr[AIndex].nWidth := ImgInfo.wW;
        m_ImgArr[AIndex].nHeight := ImgInfo.wH;
        m_ImgArr[AIndex].nPx := ImgInfo.wPx;
        m_ImgArr[AIndex].nPy := ImgInfo.wPy;

        ASize.cx := ImgInfo.wW;
        ASize.cy := ImgInfo.wH;
        APoint.X := ImgInfo.wPx;
        APoint.Y := ImgInfo.wPy;
        Result := True;
      end;
    end
    else begin
      ASize.cx := m_ImgArr[AIndex].nWidth;
      ASize.cy := m_ImgArr[AIndex].nHeight;
      APoint.X := m_ImgArr[AIndex].nPx;
      APoint.Y := m_ImgArr[AIndex].nPy;
      Result := True;
    end;
  end;
end;

function TWisImages.GetCachedBrightImage(Index:Integer; var PX, PY:Integer):TTexture;
begin
  Result := nil;
  Lock;
  try
    if Initialized and (Index >= 0) and (Index < ImageCount) then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Bright = nil then begin
        LoadDxBrightImage(@IndexArray[Index], @m_ImgArr[Index]);
        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;
        m_ImgArr[Index].dwLatestBrightTime := MyGetTickCount;

        if m_ImgArr[Index].Bright <> nil then
          BrightIndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Bright;
      end
      else begin
        m_ImgArr[Index].dwLatestBrightTime := MyGetTickCount;
        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;
        Result := m_ImgArr[Index].Bright;
      end;
    end;
  finally
    UnLock;
  end;
end;

function TWisImages.GetCachedGrayImage(Index:Integer; var PX, PY:Integer):TTexture;
begin
  Result := nil;
  Lock;
  try
    if Initialized and (Index >= 0) and (Index < ImageCount) then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Gray = nil then begin
        LoadDxGrayImage(@IndexArray[Index], @m_ImgArr[Index]);
        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;
        m_ImgArr[Index].dwLatestGrayTime := MyGetTickCount;

        if m_ImgArr[Index].Gray <> nil then
          GrayIndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Gray;
      end
      else begin
        m_ImgArr[Index].dwLatestGrayTime := MyGetTickCount;
        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;
        Result := m_ImgArr[Index].Gray;
      end;
    end;
  finally
    UnLock;
  end;
end;

function TWisImages.GetBitmap(Index:Integer; var PX, PY:Integer):TBitmap;
begin
  Result := nil;
  Lock;
  try
    if Initialized and (Index >= 0) and (Index < ImageCount) then begin
      if m_ImgArr[Index].Bitmap = nil then begin
        LoadDxBitmap(@IndexArray[Index], @m_ImgArr[Index]);
        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;

        if m_ImgArr[Index].Bitmap <> nil then
          IndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Bitmap;
      end
      else begin
        m_ImgArr[Index].dwLatestTime := MyGetTickCount;
        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;
        Result := m_ImgArr[Index].Bitmap;
      end;
    end;
  finally
    UnLock;
  end;
end;

function TWisImages.GetCachedSurface(Index:Integer):TTexture;
begin
  Result := nil;
  Lock;
  try
    if Initialized and (Index >= 0) and (Index < ImageCount) then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Surface = nil then begin
        LoadDxImage(@IndexArray[Index], @m_ImgArr[Index]);
        m_ImgArr[Index].dwLatestTime := MyGetTickCount;
        if m_ImgArr[Index].Surface <> nil then
          IndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Surface;
      end
      else begin
        m_ImgArr[Index].dwLatestTime := MyGetTickCount;
        Result := m_ImgArr[Index].Surface;
      end;
    end;
  finally
    UnLock;
  end;
end;

function TWisImages.GetCachedBitmap(Index:Integer):TBitmap;
begin
  Result := nil;
  Lock;
  try
    if Initialized and (Index >= 0) and (Index < ImageCount) then begin
      if m_ImgArr[Index].Bitmap = nil then begin
        LoadDxBitmap(@IndexArray[Index], @m_ImgArr[Index]);

        if m_ImgArr[Index].Bitmap <> nil then
          IndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Bitmap;
      end
      else begin
        m_ImgArr[Index].dwLatestTime := MyGetTickCount;
        Result := m_ImgArr[Index].Bitmap;
      end;
    end;
  finally
    UnLock;
  end;
end;

function TWisImages.GetCachedBright(Index:Integer):TTexture;
begin
  Result := nil;
  Lock;
  try
    if Initialized and (Index >= 0) and (Index < ImageCount) then begin
      FreeOldMemorys_Ex;

      if (m_ImgArr[Index].Bright = nil) then begin
        LoadDxBrightImage(@IndexArray[Index], @m_ImgArr[Index]);
        m_ImgArr[Index].dwLatestBrightTime := MyGetTickCount;

        if (m_ImgArr[Index].Bright <> nil) then
          BrightIndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Bright;
      end
      else begin
        m_ImgArr[Index].dwLatestBrightTime := MyGetTickCount;
        Result := m_ImgArr[Index].Bright;
      end;
    end;
  finally
    UnLock;
  end;
end;

function TWisImages.GetCachedGray(Index:Integer):TTexture;
begin
  Result := nil;
  Lock;
  try
    if Initialized and (Index >= 0) and (Index < ImageCount) then begin
      FreeOldMemorys_Ex;

      if (m_ImgArr[Index].Gray = nil) then begin
        LoadDxGrayImage(@IndexArray[Index], @m_ImgArr[Index]);
        m_ImgArr[Index].dwLatestGrayTime := MyGetTickCount;
        if (m_ImgArr[Index].Gray <> nil) then
          GrayIndexList.Add(Pointer(Index));
        Result := m_ImgArr[Index].Gray;
      end
      else begin
        m_ImgArr[Index].dwLatestGrayTime := MyGetTickCount;
        Result := m_ImgArr[Index].Gray;
      end;
    end;
  finally
    UnLock;
  end;
end;

procedure TWisImages.LoadDxBitmap(WisHeader:PTWisHeader; DXImage:pTDXImage);
var
  ImgInfo:TImgInfo;
  I, nWidth, nHeight:Integer;
  S:Pointer;
  SrcP:PByte;
  nSize:Integer;
  Source:TDIB;
begin
  if (WisHeader.OffSet > 0) and (WisHeader.OffSet < FileStream.Size) then begin
    FileStream.Position := WisHeader.OffSet;
    FileStream.Read(ImgInfo, SizeOf(ImgInfo));
    DXImage.nWidth := ImgInfo.wW;
    DXImage.nHeight := ImgInfo.wH;
    DXImage.nPx := ImgInfo.wPx;
    DXImage.nPy := ImgInfo.wPy;
    DXImage.dwLatestTime := MyGetTickCount;
    nSize := ImgInfo.wW * ImgInfo.wH;

    nWidth := ImgInfo.wW;
    nHeight := ImgInfo.wH;
    if (nSize > 4) and (nSize < 999999) then begin
      Source := TDIB.Create;
      Move(g_DefColorTable, Source.ColorTable, SizeOf(g_DefColorTable));
      Source.UpdatePalette;
      Source.SetSize(nWidth, nHeight, 8);
      Source.Canvas.Brush.Color := clBlack;
      Source.Canvas.FillRect(Source.Canvas.ClipRect);
      if ImgInfo.btEncr0 = 1 then begin
        GetMem(S, WisHeader.Length);
        SrcP := S;
        FileStream.Read(SrcP^, WisHeader.Length);
        DecodeWis(SrcP, WisHeader.Length, nWidth, nHeight, Source);
        FreeMem(S);
        // Source.SaveToFile(IntToStr(Integer(Source)) + '.bmp');
      end
      else begin
        for I := 0 to Source.Height - 1 do begin
          FileStream.Read(Source.ScanLine[I]^, nWidth); // Source.Height - 1 - I
        end;
      end;

      DXImage.Bitmap := TBitmap.Create;
      DXImage.Bitmap.Width := Source.Width;
      DXImage.Bitmap.Height := Source.Height;
      DXImage.Bitmap.Canvas.Draw(0, 0, Source);

      Source.Free;
    end;
  end;
end;

procedure TWisImages.LoadDxImage(WisHeader:PTWisHeader; DXImage:pTDXImage);
var
  {$IF USEMAPSTREAM = 0}
  ImgInfo:TImgInfo;
  {$ELSE}
  ImgInfo:pTImgInfo;
  {$IFEND}

  I, nWidth, nHeight:Integer;

  S:Pointer;
  SrcP:PByte;
  nSize:Integer;
  Source:TDIB;

  FileData:Pointer;
  FileSize:Integer;
begin
  if (WisHeader.OffSet > 0) and (WisHeader.OffSet < FileStream.Size) then begin
    {$IF USEMAPSTREAM = 0}
    FileStream.Position := WisHeader.OffSet;
    FileStream.Read(ImgInfo, SizeOf(ImgInfo));
    {$ELSE}
    ImgInfo := pTImgInfo(Integer(FileStream.Memory) + WisHeader.OffSet);
    FileData := Pointer(Integer(ImgInfo) + SizeOf(TImgInfo));
    {$IFEND}
    DXImage.nWidth := ImgInfo.wW;
    DXImage.nHeight := ImgInfo.wH;
    DXImage.nPx := ImgInfo.wPx;
    DXImage.nPy := ImgInfo.wPy;
    DXImage.dwLatestTime := MyGetTickCount;
    nSize := ImgInfo.wW * ImgInfo.wH;

    nWidth := ImgInfo.wW;
    nHeight := ImgInfo.wH;
    if (nSize > 4) and (nSize < 999999) then begin
      Source := TDIB.Create;
      Move(g_DefColorTable, Source.ColorTable, SizeOf(g_DefColorTable));
      Source.UpdatePalette;
      Source.SetSize(nWidth, nHeight, 8);
      Source.Canvas.Brush.Color := clBlack;
      Source.Canvas.FillRect(Source.Canvas.ClipRect);
      if ImgInfo.btEncr0 = 1 then begin
        GetMem(S, WisHeader.Length);
        SrcP := S;
        {$IF USEMAPSTREAM = 0}
        FileStream.Read(SrcP^, WisHeader.Length);
        {$ELSE}
        Move(FileData^, SrcP^, WisHeader.Length);
        {$IFEND}

        DecodeWis(SrcP, WisHeader.Length, nWidth, nHeight, Source);
        FreeMem(S);
      end
      else begin
        {$IF USEMAPSTREAM = 0}
        for I := 0 to Source.Height - 1 do begin
          Move(SrcP^, Source.ScanLine[I]^, nWidth);
          FileStream.Read(Source.ScanLine[I]^, nWidth); // Source.Height - 1 - I
        end;
        {$ELSE}
        SrcP := FileData;
        for I := 0 to Source.Height - 1 do begin
          Move(SrcP^, Source.ScanLine[I]^, nWidth);
          Inc(SrcP, nWidth);
        end;
        {$IFEND}
      end;

      if D3DFormat and (Source.Width >= 400) and (Source.Height >= 400) then begin
        FileData32(Source, FileData, FileSize);
        if (FileSize > 0) and (FileData <> nil) then begin
          DXImage.Surface := NewTexture(FileData, FileSize, Source.Width, Source.Height, TransparentColor, D3DFormat);
          FreeMem(FileData);
        end;
      end
      else begin
        DXImage.Surface := NewTexture(Source);
      end;
      Source.Free;
    end;

    if DXImage.Surface = nil then begin
      DXImage.dwLatestTime := MyGetTickCount;
      DXImage.nPx := ImgInfo.wPx;
      DXImage.nPy := ImgInfo.wpy;
      DXImage.Surface := NULLTexture;
    end;
  end;
end;

procedure TWisImages.LoadDxGrayImage(WisHeader:PTWisHeader; DXImage:pTDXImage);
var
  {$IF USEMAPSTREAM = 0}
  ImgInfo:TImgInfo;
  {$ELSE}
  ImgInfo:pTImgInfo;
  {$IFEND}

  I, nWidth, nHeight:Integer;

  S:Pointer;
  SrcP:PByte;
  nSize:Integer;
  Source:TDIB;

  FileData:Pointer;
  FileSize:Integer;
begin
  if (WisHeader.OffSet > 0) and (WisHeader.OffSet < FileStream.Size) then begin
    {$IF USEMAPSTREAM = 0}
    FileStream.Position := WisHeader.OffSet;
    FileStream.Read(ImgInfo, SizeOf(ImgInfo));
    {$ELSE}
    ImgInfo := pTImgInfo(Integer(FileStream.Memory) + WisHeader.OffSet);
    FileData := Pointer(Integer(ImgInfo) + SizeOf(TImgInfo));
    {$IFEND}

    DXImage.nWidth := ImgInfo.wW;
    DXImage.nHeight := ImgInfo.wH;
    DXImage.nPx := ImgInfo.wPx;
    DXImage.nPy := ImgInfo.wPy;
    DXImage.dwLatestGrayTime := MyGetTickCount;
    nSize := ImgInfo.wW * ImgInfo.wH;

    nWidth := ImgInfo.wW;
    nHeight := ImgInfo.wH;
    if (nSize > 4) and (nSize < 999999) then begin
      Source := TDIB.Create;
      Move(g_DefColorTable, Source.ColorTable, SizeOf(g_DefColorTable));
      Source.UpdatePalette;
      Source.SetSize(nWidth, nHeight, 8);
      Source.Canvas.Brush.Color := clBlack;
      Source.Canvas.FillRect(Source.Canvas.ClipRect);
      if ImgInfo.btEncr0 = 1 then begin
        GetMem(S, WisHeader.Length);
        SrcP := S;
        {$IF USEMAPSTREAM = 0}
        FileStream.Read(SrcP^, WisHeader.Length);
        {$ELSE}
        Move(FileData^, SrcP^, WisHeader.Length);
        {$IFEND}

        DecodeWis(SrcP, WisHeader.Length, nWidth, nHeight, Source);
        FreeMem(S);
        // Source.SaveToFile(IntToStr(Integer(Source)) + '.bmp');
      end
      else begin
        {$IF USEMAPSTREAM = 0}
        for I := 0 to Source.Height - 1 do begin
          Move(SrcP^, Source.ScanLine[I]^, nWidth);
          FileStream.Read(Source.ScanLine[I]^, nWidth); // Source.Height - 1 - I
        end;
        {$ELSE}
        SrcP := FileData;
        for I := 0 to Source.Height - 1 do begin
          Move(SrcP^, Source.ScanLine[I]^, nWidth);
          Inc(SrcP, nWidth);
          // FileStream.Read(Source.ScanLine[I]^, nWidth); //Source.Height - 1 - I
        end;
        {$IFEND}

      end;
      if D3DFormat and (Source.Width >= 400) and (Source.Height >= 400) then begin
        FileDataGray32(Source, FileData, FileSize);
        if (FileSize > 0) and (FileData <> nil) then begin
          DXImage.Gray := NewTexture(FileData, FileSize, Source.Width, Source.Height, TransparentColor, D3DFormat);
          FreeMem(FileData);
        end;
      end
      else begin
        DXImage.Gray := NewTextureGray(Source);
      end;
      Source.Free;
    end;

    if DXImage.Gray = nil then begin
      DXImage.dwLatestGrayTime := MyGetTickCount;
      DXImage.nPx := ImgInfo.wPx;
      DXImage.nPy := ImgInfo.wpy;
      DXImage.Gray := NULLTexture;
    end;
  end;
end;

procedure TWisImages.LoadDxBrightImage(WisHeader:PTWisHeader; DXImage:pTDXImage);
var
  {$IF USEMAPSTREAM = 0}
  ImgInfo:TImgInfo;
  {$ELSE}
  ImgInfo:pTImgInfo;
  {$IFEND}

  I, nWidth, nHeight:Integer;

  S:Pointer;
  SrcP:PByte;
  nSize:Integer;
  Source:TDIB;
  FileData:Pointer;
  FileSize:Integer;
begin
  if (WisHeader.OffSet > 0) and (WisHeader.OffSet < FileStream.Size) then begin
    {$IF USEMAPSTREAM = 0}
    FileStream.Position := WisHeader.OffSet;
    FileStream.Read(ImgInfo, SizeOf(ImgInfo));
    {$ELSE}
    ImgInfo := pTImgInfo(Integer(FileStream.Memory) + WisHeader.OffSet);
    FileData := Pointer(Integer(ImgInfo) + SizeOf(TImgInfo));
    {$IFEND}

    DXImage.nWidth := ImgInfo.wW;
    DXImage.nHeight := ImgInfo.wH;
    DXImage.nPx := ImgInfo.wPx;
    DXImage.nPy := ImgInfo.wPy;
    DXImage.dwLatestBrightTime := MyGetTickCount;
    nSize := ImgInfo.wW * ImgInfo.wH;

    nWidth := ImgInfo.wW;
    nHeight := ImgInfo.wH;
    if (nSize > 4) and (nSize < 999999) then begin
      Source := TDIB.Create;
      Move(g_DefColorTable, Source.ColorTable, SizeOf(g_DefColorTable));
      Source.UpdatePalette;
      Source.SetSize(nWidth, nHeight, 8);
      Source.Canvas.Brush.Color := clBlack;
      Source.Canvas.FillRect(Source.Canvas.ClipRect);
      if ImgInfo.btEncr0 = 1 then begin
        GetMem(S, WisHeader.Length);
        SrcP := S;
        {$IF USEMAPSTREAM = 0}
        FileStream.Read(SrcP^, WisHeader.Length);
        {$ELSE}
        Move(FileData^, SrcP^, WisHeader.Length);
        {$IFEND}

        DecodeWis(SrcP, WisHeader.Length, nWidth, nHeight, Source);
        FreeMem(S);
        // Source.SaveToFile(IntToStr(Integer(Source)) + '.bmp');
      end
      else begin
        {$IF USEMAPSTREAM = 0}
        for I := 0 to Source.Height - 1 do begin
          Move(SrcP^, Source.ScanLine[I]^, nWidth);
          FileStream.Read(Source.ScanLine[I]^, nWidth); // Source.Height - 1 - I
        end;
        {$ELSE}
        SrcP := FileData;
        for I := 0 to Source.Height - 1 do begin
          Move(SrcP^, Source.ScanLine[I]^, nWidth);
          Inc(SrcP, nWidth);
          // FileStream.Read(Source.ScanLine[I]^, nWidth); //Source.Height - 1 - I
        end;
        {$IFEND}

      end;

      if D3DFormat and (Source.Width >= 400) and (Source.Height >= 400) then begin
        FileDataBright32(Source, FileData, FileSize);
        if (FileSize > 0) and (FileData <> nil) then begin
          DXImage.Bright := NewTexture(FileData, FileSize, Source.Width, Source.Height, TransparentColor, D3DFormat);
          FreeMem(FileData);
        end;
      end
      else begin
        DXImage.Bright := NewTextureBright(Source);
      end;
      Source.Free;
    end;

    if DXImage.Bright = nil then begin
      DXImage.dwLatestBrightTime := MyGetTickCount;
      DXImage.nPx := ImgInfo.wPx;
      DXImage.nPy := ImgInfo.wpy;
      DXImage.Bright := NULLTexture;
    end;

  end;
end;

end.
