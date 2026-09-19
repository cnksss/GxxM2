unit Wil;

interface

uses
  Windows,
  Classes,
  Graphics,
  SysUtils,
  MapFiles,
  HGE,
  GameImages,
  DIB,
  DxCanvas,
  GlobalString;

type
  TWMImageHeader = record
    Title:string[40]; // 'WEMADE Entertainment inc.'
    ImageCount:Integer;
    ColorCount:Integer;
    PaletteSize:Integer;
    VerFlag:Integer;
  end;

  PTWMImageHeader = ^TWMImageHeader;
  TWMImageInfo = record
    nWidth:SmallInt;
    nHeight:SmallInt;
    px:SmallInt;
    py:SmallInt;
    bits:PByte;
  end;
  PTWMImageInfo = ^TWMImageInfo;

  TWMIndexHeader = record
    Title:string[40]; // 'WEMADE Entertainment inc.'
    IndexCount:integer;
    VerFlag:integer;
  end;

  PTWMIndexHeader = ^TWMIndexHeader;

  TWMIndexInfo = record
    Position:Integer;
    Size:Integer;
  end;
  PTWMIndexInfo = ^TWMIndexInfo;

  pTWMImages = ^TWMImages;
  TWMImages = class(TGameImages)

  private
    btVersion:Byte;

    FHeader:TWMImageHeader;

    procedure LoadPalette;
    procedure LoadDxImage(Position:Integer; DXImage:pTDXImage);
    procedure LoadDxGrayImage(Position:Integer; DXImage:pTDXImage);
    procedure LoadDxBrightImage(Position:Integer; DXImage:pTDXImage);
    procedure LoadIndex(sIdxFile:string);
    procedure LoadDxBitmap(Position:Integer; DXImage:pTDXImage);
  protected
    function GetCachedSurface(Index:Integer):TTexture; override;
    function GetCachedGray(Index:Integer):TTexture; override;
    function GetCachedBright(Index:Integer):TTexture; override;

    function GetCachedBitmap(Index:Integer):TBitmap; override;
  public
    {$IF USEMAPSTREAM = 0}
    m_FileStream:TFileStream;
    {$ELSE}
    m_FileStream:TMapStream;
    {$IFEND}

    MainPalette:TRGBQuads;

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

constructor TWMImages.Create();
begin
  inherited Create;
  btVersion := 0;
  m_FileStream := nil;
  m_IndexList := TList.Create;

end;

destructor TWMImages.Destroy;
begin
  m_IndexList.Free;
  inherited;
end;

procedure TextOutStr(Msg:string);
var
  flname:string;
  fhandle:TextFile;
begin
  flname := '.\Text.txt';
  if FileExists(flname) then begin
    AssignFile(fhandle, flname);
    Append(fhandle);
  end
  else begin
    AssignFile(fhandle, flname);
    Rewrite(fhandle);
  end;
  Writeln(fhandle, TimeToStr(Time) + ' ' + Msg);
  CloseFile(fhandle);
end;

procedure TWMImages.Initialize;
var
  idxfile:string;
begin
  if not Initialized then begin
    if FileExists(FileName) then begin
      {$IF USEMAPSTREAM = 0}
      try
        m_FileStream := TFileStream.Create(FileName, fmOpenRead or fmShareDenyNone);
      except
        m_FileStream := nil;
      end;
      {$ELSE}
      m_FileStream := TMapStream.Create;
      m_FileStream.LoadFromFile(FileName);
      {$IFEND}
      if m_FileStream = nil then begin

        Exit;
      end;

      m_FileStream.Read(FHeader, SizeOf(TWMImageHeader));

      ////////// 强行修复骑马资源 chongchong 2013-10-17
      if FHeader.VerFlag <> 0 then begin
        FHeader.VerFlag := 0;
        FHeader.ColorCount := 256;
      end;

      if (FHeader.VerFlag = 0) or (FHeader.ColorCount = 65536) then begin
        btVersion := 1;
        m_FileStream.Seek(-4, soFromCurrent);
      end;

      case FHeader.ColorCount of
        256:BitCount := 8;
        65536:BitCount := 16;
        16777216:BitCount := 24;
        else
          BitCount := 32;
      end;

      ImageCount := FHeader.ImageCount;

      m_ImgArr := AllocMem(SizeOf(TDXImage) * ImageCount);

      if m_ImgArr = nil then begin
        ImageCount := 0;
        if m_FileStream <> nil then
          FreeAndNil(m_FileStream);
        Exit;
      end;

      idxfile := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.wix';
      LoadPalette;

      LoadIndex(idxfile);
      Initialized := True;
    end;
  end;
end;

procedure TWMImages.Finalize;
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
    ImageCount := 0;
    if m_FileStream <> nil then
      FreeAndNil(m_FileStream);
  finally
    UnLock;
  end;
end;

procedure TWMImages.LoadPalette;
begin
  if btVersion <> 0 then
    m_FileStream.Seek(SizeOf(TWMImageHeader) - 4, 0)
  else
    m_FileStream.Seek(SizeOf(TWMImageHeader), 0);

  m_FileStream.Read(MainPalette, SizeOf(TRGBQuad) * 256);
end;

procedure TWMImages.LoadIndex(sIdxFile:string);
var
  FHandle, I, Value:integer;
  Header:TWMIndexHeader;
  PValue:PInteger;
begin
  m_IndexList.Clear;
  if FileExists(sIdxFile) then begin
    FHandle := FileOpen(sIdxFile, fmOpenRead or fmShareDenyNone);
    if FHandle > 0 then begin
      if btVersion <> 0 then
        FileRead(FHandle, Header, SizeOf(TWMIndexHeader) - 4)
      else
        FileRead(FHandle, Header, SizeOf(TWMIndexHeader));

      if Header.IndexCount > 0 then begin
        PValue := AllocMem(4 * Header.IndexCount);
        FileRead(FHandle, PValue^, 4 * Header.IndexCount);
        for I := 0 to Header.IndexCount - 1 do begin
          Value := PInteger(Integer(PValue) + 4 * I)^;
          m_IndexList.Add(Pointer(Value));
        end;
        FreeMem(PValue);
      end;

      FileClose(FHandle);
    end;
  end;
end;

{----------------- Private Variables ---------------------}

procedure TWMImages.LoadDxBrightImage(Position:Integer; DXImage:pTDXImage);
var
  {$IF USEMAPSTREAM = 0}
  ImageInfo:TWMImageInfo;
  {$ELSE}
  ImageInfo:pTWMImageInfo;
  {$IFEND}
  nSize:Integer;

  Source:TDIB;

  FileData:Pointer;
  FileSize:Integer;
begin
  if (Position > 0) and (Position < m_FileStream.Size) and (DXImage.Bright = nil) then begin
    {$IF USEMAPSTREAM = 0}
    m_FileStream.Position := Position;
    if btVersion <> 0 then
      m_FileStream.Read(ImageInfo, SizeOf(TWMImageInfo) - 4)
    else
      m_FileStream.Read(ImageInfo, SizeOf(TWMImageInfo));
    {$ELSE}
    ImageInfo := pTWMImageInfo(Integer(m_FileStream.Memory) + Position);
    if btVersion <> 0 then
      FileData := Pointer(Integer(ImageInfo) + SizeOf(TWMImageInfo) - 4)
    else
      FileData := Pointer(Integer(ImageInfo) + SizeOf(TWMImageInfo));
    {$IFEND}

    if ((Abs(ImageInfo.px) > MAX_IMAGE_WIDTH) or (Abs(ImageInfo.py) > MAX_IMAGE_HEIGHT)) then begin
      OutMessage(Format(DecodeResStr(SWilLoadDxBrightImageErr), [FileName, ImageInfo.px, ImageInfo.py]));
      Exit;
    end;

    if ((ImageInfo.nWidth <= 0) or (ImageInfo.nHeight <= 0)) then begin
      OutMessage(Format(DecodeResStr(SWilLoadDxBrightImageSizeErr), [FileName, ImageInfo.nWidth, ImageInfo.nHeight]));
      Exit;
    end;

    if ((ImageInfo.nWidth >= MAX_IMAGE_WIDTH) or (ImageInfo.nHeight >= MAX_IMAGE_HEIGHT)) then begin
      OutMessage(Format(DecodeResStr(SWilLoadDxBrightImageSizeErr), [FileName, ImageInfo.nWidth, ImageInfo.nHeight]));
      Exit;
    end;

    if (ImageInfo.nWidth * ImageInfo.nHeight > 4) then begin
      DXImage.dwLatestBrightTime := MyGetTickCount;
      DXImage.nWidth := ImageInfo.nWidth;
      DXImage.nHeight := ImageInfo.nHeight;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;

      nSize := WidthBytes(BitCount, ImageInfo.nWidth) * ImageInfo.nHeight;
      Source := MakeDibByBitCount(Bitcount, ImageInfo.nWidth, ImageInfo.nHeight);
      if Source <> nil then begin
        {$IF USEMAPSTREAM = 0}
        m_FileStream.Read(Source.PBits^, nSize);
        {$ELSE}
        Move(FileData^, Source.PBits^, nSize);
        {$IFEND}
        if D3DFormat and (Source.Width >= 400) and (Source.Height >= 400) then begin
          FileDataBright32(Source, FileData, FileSize);
          if (FileSize > 0) and (FileData <> nil) then begin
            DXImage.Bright := NewTexture(FileData, FileSize, Source.Width, Source.Height, TransparentColor, D3DFormat);
            FreeMem(FileData);
          end;
        end else begin
          DXImage.Bright := NewTextureBright(Source);
        end;
        Source.Free;
      end;
    end;
    if DXImage.Bright = nil then begin
      DXImage.dwLatestBrightTime := MyGetTickCount;
      DXImage.nWidth := ImageInfo.nWidth;
      DXImage.nHeight := ImageInfo.nHeight;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;
      DXImage.Bright := NULLTexture;
    end;
  end;
end;

procedure TWMImages.LoadDxGrayImage(Position:Integer; DXImage:pTDXImage);
var
  {$IF USEMAPSTREAM = 0}
  ImageInfo:TWMImageInfo;
  {$ELSE}
  ImageInfo:pTWMImageInfo;
  {$IFEND}
  nSize:Integer;
  Source:TDIB;
  FileData:Pointer;
  FileSize:Integer;
begin
  if (Position > 0) and (Position < m_FileStream.Size) and (DXImage.Gray = nil) then begin
    {$IF USEMAPSTREAM = 0}
    m_FileStream.Position := Position;
    if btVersion <> 0 then
      m_FileStream.Read(ImageInfo, SizeOf(TWMImageInfo) - 4)
    else
      m_FileStream.Read(ImageInfo, SizeOf(TWMImageInfo));
    {$ELSE}
    ImageInfo := pTWMImageInfo(Integer(m_FileStream.Memory) + Position);
    if btVersion <> 0 then
      FileData := Pointer(Integer(ImageInfo) + SizeOf(TWMImageInfo) - 4)
    else
      FileData := Pointer(Integer(ImageInfo) + SizeOf(TWMImageInfo));
    {$IFEND}

    if ((Abs(ImageInfo.px) > MAX_IMAGE_WIDTH) or (Abs(ImageInfo.py) > MAX_IMAGE_HEIGHT)) then begin
      OutMessage(Format(DecodeResStr(SWilLoadDxGrayImageErr), [FileName, ImageInfo.px, ImageInfo.py]));
      Exit;
    end;

    if ((ImageInfo.nWidth <= 0) or (ImageInfo.nHeight <= 0)) then begin
      OutMessage(Format(DecodeResStr(SWilLoadDxGrayImageSizeErr), [FileName, ImageInfo.nWidth, ImageInfo.nHeight]));
      Exit;
    end;

    if ((ImageInfo.nWidth >= MAX_IMAGE_WIDTH) or (ImageInfo.nHeight >= MAX_IMAGE_HEIGHT)) then begin
      OutMessage(Format(DecodeResStr(SWilLoadDxGrayImageSizeErr), [FileName, ImageInfo.nWidth, ImageInfo.nHeight]));
      Exit;
    end;

    if (ImageInfo.nWidth * ImageInfo.nHeight > 4) then begin
      DXImage.dwLatestGrayTime := MyGetTickCount;
      DXImage.nWidth := ImageInfo.nWidth;
      DXImage.nHeight := ImageInfo.nHeight;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;
      nSize := WidthBytes(BitCount, ImageInfo.nWidth) * ImageInfo.nHeight;
      Source := MakeDibByBitCount(Bitcount, ImageInfo.nWidth, ImageInfo.nHeight);
      if Source <> nil then begin
        {$IF USEMAPSTREAM = 0}
        m_FileStream.Read(Source.PBits^, nSize);
        {$ELSE}
        Move(FileData^, Source.PBits^, nSize);
        {$IFEND}
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
    end;

    if DXImage.Gray = nil then begin
      DXImage.dwLatestGrayTime := MyGetTickCount;
      DXImage.nWidth := ImageInfo.nWidth;
      DXImage.nHeight := ImageInfo.nHeight;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;
      DXImage.Gray := NULLTexture;
    end;
  end;
end;

procedure TWMImages.LoadDxImage(Position:Integer; DXImage:pTDXImage);
var
  {$IF USEMAPSTREAM = 0}
  ImageInfo:TWMImageInfo;
  {$ELSE}
  ImageInfo:pTWMImageInfo;
  {$IFEND}
  nSize:Integer;
  Source:TDIB;
  FileData:Pointer;
  FileSize:Integer;
begin
  if (Position > 0) and (Position < m_FileStream.Size) and (DXImage.Surface = nil) then begin
    {$IF USEMAPSTREAM = 0}
    m_FileStream.Position := Position;
    if btVersion <> 0 then
      m_FileStream.Read(ImageInfo, SizeOf(TWMImageInfo) - 4)
    else
      m_FileStream.Read(ImageInfo, SizeOf(TWMImageInfo));
    {$ELSE}
    ImageInfo := pTWMImageInfo(Integer(m_FileStream.Memory) + Position);
    if btVersion <> 0 then
      FileData := Pointer(Integer(ImageInfo) + SizeOf(TWMImageInfo) - 4)
    else
      FileData := Pointer(Integer(ImageInfo) + SizeOf(TWMImageInfo));
    {$IFEND}

    if ((Abs(ImageInfo.px) > MAX_IMAGE_WIDTH) or (Abs(ImageInfo.py) > MAX_IMAGE_HEIGHT)) then begin
      OutMessage(Format(DecodeResStr(SWilLoadDxImageErr), [FileName, ImageInfo.px, ImageInfo.py]));
      Exit;
    end;

    if ((ImageInfo.nWidth <= 0) or (ImageInfo.nHeight <= 0)) then begin
      OutMessage(Format(DecodeResStr(SWilLoadDxImageSizeErr), [FileName, ImageInfo.nWidth, ImageInfo.nHeight]));
      Exit;
    end;

    if ((ImageInfo.nWidth >= MAX_IMAGE_WIDTH) or (ImageInfo.nHeight >= MAX_IMAGE_HEIGHT)) then begin
      OutMessage(Format(DecodeResStr(SWilLoadDxImageSizeErr), [FileName, ImageInfo.nWidth, ImageInfo.nHeight]));
      Exit;
    end;

    if (ImageInfo.nWidth * ImageInfo.nHeight > 4) then begin
      DXImage.dwLatestTime := MyGetTickCount;
      DXImage.nWidth := ImageInfo.nWidth;
      DXImage.nHeight := ImageInfo.nHeight;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;
      nSize := WidthBytes(BitCount, ImageInfo.nWidth) * ImageInfo.nHeight;
      Source := MakeDibByBitCount(Bitcount, ImageInfo.nWidth, ImageInfo.nHeight);
      if Source <> nil then begin
        {$IF USEMAPSTREAM = 0}
        m_FileStream.Read(Source.PBits^, nSize);
        {$ELSE}
        Move(FileData^, Source.PBits^, nSize);
        {$IFEND}
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
    end;

    if DXImage.Surface = nil then begin
      DXImage.dwLatestTime := MyGetTickCount;
      DXImage.nWidth := ImageInfo.nWidth;
      DXImage.nHeight := ImageInfo.nHeight;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;
      DXImage.Surface := NULLTexture;
    end;
  end;
end;

procedure TWMImages.LoadDxBitmap(Position:Integer; DXImage:pTDXImage);
const
  RGB565_MASK_RED = $F800;
  RGB555_MASK_RED = $07C0;
var
  ImageInfo:TWMImageInfo;
  nHeight, nSize:Integer;

  I:Integer;
  S:Pointer;
  SrcP:PByte;
  DesP:Pointer;

  Source:TDIB;
begin
  if (Position > 0) and (Position < m_FileStream.Size) then begin
    m_FileStream.Position := Position;
    if btVersion <> 0 then
      m_FileStream.Read(ImageInfo, SizeOf(TWMImageInfo) - 4)
    else
      m_FileStream.Read(ImageInfo, SizeOf(TWMImageInfo));
    if (ImageInfo.nWidth * ImageInfo.nHeight > 4) and (ImageInfo.nWidth * ImageInfo.nHeight < 2999999) then begin
      nHeight := ImageInfo.nHeight;
      nSize := WidthBytes(BitCount, ImageInfo.nWidth) * nHeight;

      GetMem(S, nSize);
      m_FileStream.Read(S^, nSize);
      SrcP := S;

      DXImage.nWidth := ImageInfo.nWidth;
      DXImage.nHeight := ImageInfo.nHeight;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;

      case BitCount of
        8:begin
            Source := TDIB.Create;
            try
              Source.ColorTable := MainPalette;
              Source.UpdatePalette;
              Source.SetSize(ImageInfo.nWidth, ImageInfo.nHeight, 8);
              Source.Canvas.Brush.Color := clblack;
              Source.Canvas.FillRect(Source.Canvas.ClipRect);

              DesP := Source.PBits;
              Move(SrcP^, DesP^, nSize);

              Source.PixelFormat := MakeDIBPixelFormat(5, 6, 5);

              DXImage.Bitmap := TBitmap.Create;
              DXImage.Bitmap.Width := Source.Width;
              DXImage.Bitmap.Height := Source.Height;
              DXImage.Bitmap.PixelFormat := pf16bit;

              for I := DXImage.Bitmap.Height - 1 downto 0 do begin
                DesP := DXImage.Bitmap.ScanLine[I];
                SrcP := Source.ScanLine[I];
                Move(SrcP^, DesP^, Source.WidthBytes);
              end;

            finally
              Source.Free;
            end;

          end;
        16:begin
            DXImage.Bitmap := TBitmap.Create;
            DXImage.Bitmap.Width := ImageInfo.nWidth;
            DXImage.Bitmap.Height := ImageInfo.nHeight;
            DXImage.Bitmap.PixelFormat := pf16bit;
            for I := DXImage.Bitmap.Height - 1 downto 0 do begin
              DesP := DXImage.Bitmap.ScanLine[I];
              Move(SrcP^, DesP^, ImageInfo.nWidth * 2);
              Inc(SrcP, ImageInfo.nWidth * 2);
            end;
          end;
        24:begin
            DXImage.Bitmap := TBitmap.Create;
            DXImage.Bitmap.Width := ImageInfo.nWidth;
            DXImage.Bitmap.Height := ImageInfo.nHeight;
            DXImage.Bitmap.PixelFormat := pf24bit;

            for I := DXImage.Bitmap.Height - 1 downto 0 do begin
              DesP := DXImage.Bitmap.ScanLine[I];
              Move(SrcP^, DesP^, ImageInfo.nWidth * 3);
              Inc(SrcP, ImageInfo.nWidth * 3);
            end;
          end;
        32:begin
            DXImage.Bitmap := TBitmap.Create;
            DXImage.Bitmap.Width := ImageInfo.nWidth;
            DXImage.Bitmap.Height := ImageInfo.nHeight;
            DXImage.Bitmap.PixelFormat := pf32bit;
            for I := DXImage.Bitmap.Height - 1 downto 0 do begin
              DesP := DXImage.Bitmap.ScanLine[I];
              Move(SrcP^, DesP^, ImageInfo.nWidth * 4);
              Inc(SrcP, ImageInfo.nWidth * 4);
            end;
          end;
      end;
      FreeMem(S);
    end;
  end;
end;

function TWMImages.GetCachedBitmap(Index:Integer):TBitmap;
var
  nPosition:integer;
begin
  Result := nil;
  Lock;
  try
    if (Index >= 0) and (Index < ImageCount) and (Index < m_IndexList.Count) and (m_FileStream <> nil) and (Initialized) then begin
      if m_ImgArr[Index].Bitmap = nil then begin
        nPosition := Integer(m_IndexList[Index]);
        LoadDxBitmap(nPosition, @m_ImgArr[Index]);
        m_ImgArr[Index].dwLatestTime := MyGetTickCount;

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

function TWMImages.GetCachedSurface(Index:Integer):TTexture;
var
  nPosition:Integer;
begin
  Result := nil;
  Lock;
  try
    if (Index >= 0) and (Index < ImageCount) and (Index < m_IndexList.Count) and (m_FileStream <> nil) and (Initialized) then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Surface = nil then begin
        nPosition := Integer(m_IndexList[Index]);
        LoadDxImage(nPosition, @m_ImgArr[Index]);
        m_ImgArr[Index].dwLatestTime := MyGetTickCount;

        if m_ImgArr[Index].Surface <> nil then
          IndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Surface;
        if Result = nil then
          Result := g_NullImage;
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

function TWMImages.GetCachedGray(Index:Integer):TTexture;
var
  nPosition:Integer;
begin
  Result := nil;
  Lock;
  try
    if (Index >= 0) and (Index < ImageCount) and (Index < m_IndexList.Count) and (m_FileStream <> nil) and (Initialized) then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Gray = nil then begin
        nPosition := Integer(m_IndexList[Index]);
        LoadDxGrayImage(nPosition, @m_ImgArr[Index]);
        m_ImgArr[Index].dwLatestGrayTime := MyGetTickCount;

        if m_ImgArr[Index].Gray <> nil then
          GrayIndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Gray;

        if Result = nil then
          Result := g_NullImage;
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

function TWMImages.GetCachedBright(Index:Integer):TTexture;
var
  nPosition:Integer;
begin
  Result := nil;
  Lock;
  try
    if (Index >= 0) and (Index < ImageCount) and (Index < m_IndexList.Count) and (m_FileStream <> nil) and (Initialized) then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Bright = nil then begin
        nPosition := Integer(m_IndexList[Index]);
        LoadDxBrightImage(nPosition, @m_ImgArr[Index]);
        m_ImgArr[Index].dwLatestBrightTime := MyGetTickCount;

        if m_ImgArr[Index].Bright <> nil then
          BrightIndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Bright;

        if Result = nil then
          Result := g_NullImage;
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

function TWMImages.GetCachedImage(Index:Integer; var PX, PY:Integer):TTexture;
var
  nPosition:Integer;
begin
  Result := nil;
  Lock;
  try
    if (Index >= 0) and (Index < ImageCount) and (Index < m_IndexList.Count) and (m_FileStream <> nil) and (Initialized) then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Surface = nil then begin
        nPosition := Integer(m_IndexList[Index]);
        LoadDxImage(nPosition, @m_ImgArr[Index]);

        m_ImgArr[Index].dwLatestTime := MyGetTickCount;
        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;

        if m_ImgArr[Index].Surface <> nil then
          IndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Surface;

        if Result = nil then
          Result := g_NullImage;
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

function TWMImages.GetCachedImageSize(AIndex:Integer; var ASize:TSize; var APoint:TPoint):Boolean;
var
  nPosition:Integer;

  {$IF USEMAPSTREAM = 0}
  ImageInfo:TWMImageInfo;
  {$ELSE}
  ImageInfo:pTWMImageInfo;
  {$IFEND}
begin
  Result := False;
  if (AIndex >= 0) and (AIndex < ImageCount) and (AIndex < m_IndexList.Count) and (m_FileStream <> nil) and (Initialized) then begin
    if m_ImgArr[AIndex].nWidth * m_ImgArr[AIndex].nHeight = 0 then begin
      nPosition := Integer(m_IndexList[AIndex]);

      if (nPosition > 0) and (nPosition < m_FileStream.Size) then begin
        {$IF USEMAPSTREAM = 0}
        //m_FileStream.Position := nPosition;
        m_FileStream.Seek(nPosition, soBeginning);
        if btVersion <> 0 then
          m_FileStream.Read(ImageInfo, SizeOf(TWMImageInfo) - 4)
        else
          m_FileStream.Read(ImageInfo, SizeOf(TWMImageInfo));
        {$ELSE}
        ImageInfo := pTWMImageInfo(Integer(m_FileStream.Memory) + nPosition);
        {$IFEND}
        m_ImgArr[AIndex].nWidth := ImageInfo.nWidth;
        m_ImgArr[AIndex].nHeight := ImageInfo.nHeight;
        m_ImgArr[AIndex].nPx := ImageInfo.px;
        m_ImgArr[AIndex].nPy := ImageInfo.py;

        ASize.cx := ImageInfo.nWidth;
        ASize.cy := ImageInfo.nHeight;
        APoint.X := ImageInfo.px;
        APoint.Y := ImageInfo.py;

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

function TWMImages.GetCachedGrayImage(Index:Integer; var px, py:Integer):TTexture;
var
  nPosition:Integer;
begin
  Result := nil;
  Lock;
  try
    if (Index >= 0) and (Index < ImageCount) and (Index < m_IndexList.Count) and (m_FileStream <> nil) and (Initialized) then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Gray = nil then begin
        nPosition := Integer(m_IndexList[Index]);
        LoadDxGrayImage(nPosition, @m_ImgArr[Index]);
        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;
        m_ImgArr[Index].dwLatestGrayTime := MyGetTickCount;

        if m_ImgArr[Index].Gray <> nil then
          GrayIndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Gray;

        if Result = nil then
          Result := g_NullImage;
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

function TWMImages.GetCachedBrightImage(Index:Integer; var px, py:Integer):TTexture;
var
  nPosition:Integer;
begin
  Result := nil;
  Lock;
  try
    if (Index >= 0) and (Index < ImageCount) and (Index < m_IndexList.Count) and (m_FileStream <> nil) and (Initialized) then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Bright = nil then begin
        nPosition := Integer(m_IndexList[Index]);
        LoadDxBrightImage(nPosition, @m_ImgArr[Index]);
        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;
        m_ImgArr[Index].dwLatestBrightTime := MyGetTickCount;

        if m_ImgArr[Index].Bright <> nil then
          BrightIndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Bright;

        if Result = nil then
          Result := g_NullImage;
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

function TWMImages.GetBitmap(Index:Integer; var PX, PY:Integer):TBitmap;
var
  nPosition:integer;
begin
  Result := nil;
  Lock;
  try
    if Initialized and (Index >= 0) and (Index < ImageCount) and (Index < m_IndexList.Count) then begin
      if m_ImgArr[Index].Bitmap = nil then begin
        nPosition := Integer(m_IndexList[Index]);
        LoadDxBitmap(nPosition, @m_ImgArr[Index]);
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

end.
