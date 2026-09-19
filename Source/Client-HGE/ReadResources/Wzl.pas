unit Wzl;

interface

uses
  Windows,
  Classes,
  Graphics,
  SysUtils,
  MapFiles,
  HGE,
  HGECanvas,
  DxControls,
  GameImages,
  DIB,
  DxCanvas,
  GlobalString;

type
  TWzlImageHeader = record
    Title:string[40]; // 'WEMADE Entertainment inc.'
    ImageCount:Integer;
    ColorCount:Integer;
    PaletteSize:Integer;
    VerFlag:Integer;
    Flag:Integer;
  end;
  PTWzlImageHeader = ^TWzlImageHeader;

  TWzlImageInfo = record
    // bt1: Byte; //bt1=3 8位 bt1=5 16位
    PixelFormat:TPixelFormat;
    bt2:Byte; // bt2=1 是否压缩
    bt3:Byte;
    bt4:Byte; // ZIP 压缩等级
    nWidth:SmallInt;
    nHeight:SmallInt;
    px:SmallInt;
    py:SmallInt;
    Length:Integer;
  end;
  PTWzlImageInfo = ^TWzlImageInfo;

  TWzlIndexHeader = record
    Title:string[40]; // 'WEMADE Entertainment inc.'
    IndexCount:Integer;
  end;
  PTWzlIndexHeader = ^TWzlIndexHeader;

  pTWzlImages = ^TWzlImages;
  TWzlImages = class(TGameImages)
  private
    FHeader:TWzlImageHeader;
    procedure LoadDxImage(Position:Integer; DXImage:pTDXImage; Index:Integer);
    procedure LoadDxGrayImage(Position:Integer; DXImage:pTDXImage; Index:Integer);
    procedure LoadDxBrightImage(Position:Integer; DXImage:pTDXImage; Index:Integer);
    procedure LoadIndex(sIdxFile:string);
    procedure LoadDxBitmap(Position:Integer; DXImage:pTDXImage; Index:Integer);
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

    FCSFileStream:TRTLCriticalSection;

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

    procedure LockFileStream;
    procedure UnLockFileStream;
  end;

implementation
uses zlibex,
  Math{$IF CLIENTEXE = 1},
  Grobal2,
  UpdateEngine,
  MShare{$IFEND};

constructor TWzlImages.Create();
begin
  inherited Create;
  m_FileStream := nil;
  m_IndexList := TList.Create;
  ResetWZLAlpha := False;

  InitializeCriticalSection(FCSFileStream);
end;

destructor TWzlImages.Destroy;
begin
  m_IndexList.Free;
  DeleteCriticalSection(FCSFileStream);
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

procedure TWzlImages.Initialize;
var
  I:Integer;
begin
  try
    Finalize;
    if not Initialized then begin
      if FileExists(FileName) then begin
        Lock;
        try
          m_boUpdateIndex := True;
          m_boUpdateIndexing := False;
          m_boNeedUpdate := True;
          {$IF USEMAPSTREAM = 0}

          try
            m_FileStream := TFileStream.Create(FileName, fmOpenReadWrite or fmShareDenyNone);
          except
            m_FileStream := nil;
          end;
          {$ELSE}
          m_FileStream := TMapStream.Create;
          m_FileStream.LoadFromFile(FileName);
          {$IFEND}
          if m_FileStream = nil then begin
            //OutMessage('[Exception] TWzlImages::Initialize');
            OutMessage(Format(DecodeResStr(SWzlInitErr), [FileName]));
            Exit;
          end;

          m_FileStream.Read(FHeader, SizeOf(TWzlImageHeader));

          ImageCount := FHeader.ImageCount;

          m_ImgArr := AllocMem(SizeOf(TDXImage) * ImageCount);

          if m_ImgArr = nil then begin
            if m_FileStream <> nil then
              FreeAndNil(m_FileStream);
            Exit;
          end;  

          for I := 0 to ImageCount - 1 do begin
            m_ImgArr[I].nWidth := 0;
            m_ImgArr[I].nHeight := 0;
            m_ImgArr[I].nPx := 0;
            m_ImgArr[I].nPy := 0;
            m_ImgArr[I].boUpdateStop := False;
            m_ImgArr[I].boUpdateStart := False;
            m_ImgArr[I].dwUpdateStartTick := MyGetTickCount;
          end;

          // idxfile := ExtractFilePath(FileName) + ExtractFileNameOnly(FileName) + '.WZX';

          LoadIndex(IndexFileName);
          Initialized := True;

        finally
          UnLock;
        end;
      end;
    end;
  except
    on E:Exception do begin
      //DebugOutStr('[Exception]TWzlImages.Initialize; ' + E.Message + ' ' + FileName);
      raise Exception.Create(E.Message);
    end;
  end;
end;

procedure TWzlImages.Finalize;
var
  I:Integer;
begin

  Lock;
  try
    if Initialized then begin
      Initialized := False;
      IndexList.Clear;
      GrayIndexList.Clear;
      BrightIndexList.Clear;
      if m_ImgArr <> nil then begin
        for I := 0 to ImageCount - 1 do begin
          if m_ImgArr[I].Surface <> nil then begin
            try
              FreeAndNil(m_ImgArr[I].Surface);
            except
              //OutMessage('[Exception] Texture.Free');
            end;
          end;

          if m_ImgArr[I].Gray <> nil then begin
            try
              FreeAndNil(m_ImgArr[I].Gray);
            except
              //OutMessage('[Exception] Texture.Free');
            end;
          end;

          if m_ImgArr[I].Bright <> nil then begin
            try
              FreeAndNil(m_ImgArr[I].Bright);
            except
              //OutMessage('[Exception] Texture.Free');
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
    end;
  finally
    UnLock;
  end;
end;

procedure TWzlImages.LoadIndex(sIdxFile:string);
var
  I, Value:integer;
  Header:TWzlIndexHeader;
  PValue:PInteger;
  IndexStream:TFileStream;
begin
  m_IndexList.Clear;
  if FileExists(sIdxFile) then begin
    IndexStream := TFileStream.Create(sIdxFile, fmOpenReadWrite or fmShareDenyNone);
    if IndexStream = nil then begin
      //OutMessage('[Exception] TWzlImages::LoadIndex');
      OutMessage(Format(DecodeResStr(SWzlLoadIndexErr), [sIdxFile]));
      Exit;
    end;

    IndexStream.Read(Header, SizeOf(TWZLIndexHeader));

    if Header.IndexCount > 0 then begin
      PValue := AllocMem(SizeOf(Integer) * Header.IndexCount);
      IndexStream.Read(PValue^, SizeOf(Integer) * Header.IndexCount);
      for I := 0 to Header.IndexCount - 1 do begin
        Value := PInteger(Integer(PValue) + SizeOf(Integer) * I)^;
        m_IndexList.Add(Pointer(Value));
      end;
      FreeMem(PValue);
    end;
    IndexStream.Free;
  end
  else begin
    {$IF CLIENTEXE = 1}
    if m_boUpdateIndex and ((not m_boUpdateIndexing) or (MyGetTickCount - m_dwUpdateIndexingTick >= g_UpdateRetryTime)) and g_boAutoUpdate then begin
      if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtIndexWzl, -1, Self, nil) then begin
        m_boUpdateIndexing := True;
        m_dwUpdateIndexingTick := MyGetTickCount;
      end;
    end;
    {$IFEND}
  end;
end;

{----------------- Private Variables ---------------------}

procedure TWzlImages.LoadDxBrightImage(Position:Integer; DXImage:pTDXImage; Index:Integer);
var
  {$IF USEMAPSTREAM = 0}
  ImageInfo:TWzlImageInfo;
  {$ELSE}
  ImageInfo:pTWzlImageInfo;
  {$IFEND}
  nSize:Integer;

  Source:TDIB;
  FileData:Pointer;
  FileSize:Integer;

  InBuf:Pointer;

  OutBuf:Pointer;
  OutBytes:Integer;

  boDecompressError:Boolean;

  SourceAlphaSize:Integer;
  SourceAlphaBuf:PByte;
  LineData:PByte;
  pData:PByteArray;
  lsAlpha:TDIB;
  nX, nY, nX_2:Integer;
begin
  if (DXImage.Bright = nil) then begin
    if (Position = 0) or (Position > m_FileStream.Size - SizeOf(TWzlImageInfo)) then begin // 需要更新资源
      DXImage.dwLatestBrightTime := MyGetTickCount;
      DXImage.nPx := 0; //ImageInfo.px;
      DXImage.nPy := 0; //ImageInfo.py;
      // DXImage.Bright := NULLTexture;
      Exit;
    end;

    {$IF USEMAPSTREAM = 0}
    LockFileStream;
    try
      m_FileStream.Position := Position;
      m_FileStream.Read(ImageInfo, SizeOf(TWzlImageInfo));
    finally
      UnLockFileStream;
    end;
    {$ELSE}
    ImageInfo := pTWzlImageInfo(Integer(m_FileStream.Memory) + Position);
    FileData := Pointer(Integer(ImageInfo) + SizeOf(TWzlImageInfo));
    {$IFEND}

    if ((Abs(ImageInfo.px) > MAX_IMAGE_WIDTH) or (Abs(ImageInfo.nHeight) > MAX_IMAGE_HEIGHT)) then begin
      OutMessage(Format(DecodeResStr(SWzlBrightImageErr), [FileName, Index, ImageInfo.px, ImageInfo.py]));

      DXImage.dwLatestBrightTime := MyGetTickCount;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;
      DXImage.Bright := NULLTexture;

      Exit;
    end;

    if (ImageInfo.Length >= MAX_IMAGE_SIZE) then begin
      OutMessage(Format(DecodeResStr(SWzlBrightImageLenErr), [FileName, Index, ImageInfo.Length]));

      DXImage.dwLatestBrightTime := MyGetTickCount;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;
      DXImage.Bright := NULLTexture;

      Exit;
    end;

    if ((ImageInfo.nWidth >= MAX_IMAGE_WIDTH) or (ImageInfo.nHeight >= MAX_IMAGE_HEIGHT)) then begin
      OutMessage(Format(DecodeResStr(SWzlBrightImageSizeErr), [FileName, Index, ImageInfo.nWidth, ImageInfo.nHeight]));

      DXImage.dwLatestBrightTime := MyGetTickCount;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;
      DXImage.Bright := NULLTexture;

      Exit;
    end;

    if (ImageInfo.nWidth * ImageInfo.nHeight <= 4) then begin
      DXImage.Bright := NULLTexture;

      DXImage.dwLatestBrightTime := MyGetTickCount;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;
      DXImage.Bright := NULLTexture;

      Exit;
    end;

    if (ImageInfo.nWidth * ImageInfo.nHeight <= 0) or (ImageInfo.nWidth <= 0) then begin // 空图片
      DXImage.dwLatestBrightTime := MyGetTickCount;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;
      DXImage.Bright := NULLTexture;
      Exit;
    end;

    DXImage.dwLatestBrightTime := MyGetTickCount;
    DXImage.nWidth := ImageInfo.nWidth;
    DXImage.nHeight := ImageInfo.nHeight;
    DXImage.nPx := ImageInfo.px;
    DXImage.nPy := ImageInfo.py;

    //Source := nil;
    //nSize := 0; //HZQ 02230525 //预置解压缩估算值0，
    (*case ImageInfo.PixelFormat of
      pf8bit:begin
          Source := TDIB.Create;
          nSize := WidthBytes(8, ImageInfo.nWidth) * ImageInfo.nHeight;
          Move(g_DefColorTable, Source.ColorTable, SizeOf(g_DefColorTable));
          Source.UpdatePalette;
          Source.SetSize(ImageInfo.nWidth, ImageInfo.nHeight, 8);
        end;
      pf15bit:begin
          Source := TDIB.Create;
          nSize := WidthBytes(16, ImageInfo.nWidth) * ImageInfo.nHeight;
          Source.PixelFormat := MakeDIBPixelFormat(5, 5, 5);
          Source.SetSize(ImageInfo.nWidth, ImageInfo.nHeight, 16);
        end;
      pf16bit:begin
          Source := TDIB.Create;
          nSize := WidthBytes(16, ImageInfo.nWidth) * ImageInfo.nHeight;
          Source.PixelFormat := MakeDIBPixelFormat(5, 6, 5);
          Source.SetSize(ImageInfo.nWidth, ImageInfo.nHeight, 16);
        end;
      pf24bit:begin
          Source := TDIB.Create;
          nSize := WidthBytes(24, ImageInfo.nWidth) * ImageInfo.nHeight;
          Source.PixelFormat := MakeDIBPixelFormat(8, 8, 8);
          Source.SetSize(ImageInfo.nWidth, ImageInfo.nHeight, 24);
        end;
      pf32bit:begin
          Source := TDIB.Create;
          nSize := WidthBytes(32, ImageInfo.nWidth) * ImageInfo.nHeight;
          Source.PixelFormat := MakeDIBPixelFormat(8, 8, 8);
          Source.SetSize(ImageInfo.nWidth, ImageInfo.nHeight, 32);
        end;
    end;*)
    Source := MakeDibByPixelFormat(ImageInfo.PixelFormat, ImageInfo.nWidth, ImageInfo.nHeight);

    boDecompressError := False;
    lsAlpha := nil;

    if Source <> nil then begin
      nSize := Source.WidthBytes * Source.Height;
      Source.Canvas.Brush.Color := clBlack;
      Source.Canvas.FillRect(Source.Canvas.ClipRect);
      if ImageInfo.Length > 0 then begin
        GetMem(InBuf, ImageInfo.Length);
        {$IF USEMAPSTREAM = 0}
        LockFileStream;
        try
          m_FileStream.Position := Position + SizeOf(TWzlImageInfo); // add chongchong 2016-01-09
          m_FileStream.Read(InBuf^, ImageInfo.Length);
        finally
          UnLockFileStream;
        end;
        {$ELSE}
        InBuf := FileData;
        {$IFEND}
        // if ImageInfo.bt4>0 then    //ImageInfo.bt4
        // TextOutStr(inttostr(ImageInfo.bt4));
        try
          DecompressBuf(InBuf, ImageInfo.Length, nSize, OutBuf, OutBytes);
        except
          boDecompressError := True;
          //OutMessage('[Exception] TWzlImages::LoadDxBrightImage DecompressBuf');
          OutMessage(Format(DecodeResStr(SWzlBrightImageDecompErr), [FileName, Index]));
        end;

        if (OutBuf <> nil) and (OutBytes > 0) then begin
          Move(OutBuf^, Source.PBits^, nSize);

          if ResetWZLAlpha then begin
            SourceAlphaSize := OutBytes - Source.Size;
            if SourceAlphaSize = (ImageInfo.nWidth * ImageInfo.nHeight div 2) then begin
              SourceAlphaBuf := PByte(Integer(OutBuf) + Source.Size);

              lsAlpha := TDIB.Create;
              lsAlpha.SetSize(ImageInfo.nWidth, ImageInfo.nHeight, 8);

              for nY := 0 to lsAlpha.Height - 1 do begin
                LineData := lsAlpha.ScanLine[nY];
                Integer(pData) := Integer(SourceAlphaBuf) + nY * lsAlpha.Width div 2;

                for nX := 0 to lsAlpha.Width - 1 do begin
                  nX_2 := nX div 2;
                  if nX mod 2 = 0 then
                    LineData^ := pData[nX_2]
                  else
                    LineData^ := (pData[nX_2] + pData[nX_2 + 1]) div 2;

                  Inc(LineData);
                end;
              end;
            end;
          end;

          FreeMem(OutBuf);
        end;

        FreeMem(InBuf);
      end
      else begin

        LockFileStream;
        try
          m_FileStream.Position := Position + SizeOf(TWzlImageInfo); // add chongchong 2016-01-09
          m_FileStream.Read(Source.PBits^, nSize);
        finally
          UnLockFileStream;
        end;
      end;

      if D3DFormat and (Source.Width >= 400) and (Source.Height >= 400) then begin
        FileDataBright32(Source, FileData, FileSize);
        if (FileSize > 0) and (FileData <> nil) then begin
          DXImage.Bright := NewTexture(FileData, FileSize, Source.Width, Source.Height, TransparentColor, D3DFormat);
          FreeMem(FileData);
        end;
      end
      else begin
        // 修改chongchong 2015-07-30 14:05:59
        if not boDecompressError then begin
          DXImage.Bright := NewTextureBright(Source, lsAlpha);
        end
        else
          DXImage.Bright := NULLTexture;
      end;

      if lsAlpha <> nil then
        lsAlpha.Free;

      Source.Free;
    end;
  end;
end;

procedure TWzlImages.LoadDxGrayImage(Position:Integer; DXImage:pTDXImage; Index:Integer);
var
  {$IF USEMAPSTREAM = 0}
  ImageInfo:TWzlImageInfo;
  {$ELSE}
  ImageInfo:pTWzlImageInfo;
  {$IFEND}
  nSize:Integer;

  Source:TDIB;

  FileData:Pointer;
  FileSize:Integer;

  InBuf:Pointer;

  OutBuf:Pointer;
  OutBytes:Integer;

  boDecompressError:Boolean;

  SourceAlphaSize:Integer;
  SourceAlphaBuf:PByte;
  LineData:PByte;
  pData:PByteArray;
  lsAlpha:TDIB;
  nX, nY, nX_2:Integer;
begin
  if (DXImage.Gray = nil) then begin
    if (Position = 0) or (Position > m_FileStream.Size - SizeOf(TWzlImageInfo)) then begin // 需要更新资源
      DXImage.dwLatestGrayTime := MyGetTickCount;
      DXImage.nPx := 0; //ImageInfo.px;
      DXImage.nPy := 0; //ImageInfo.py;
      // DXImage.Gray := NULLTexture;
      Exit;
    end;

    {$IF USEMAPSTREAM = 0}
    LockFileStream;
    try
      m_FileStream.Position := Position;
      m_FileStream.Read(ImageInfo, SizeOf(TWzlImageInfo));
    finally
      UnLockFileStream;
    end;

    {$ELSE}
    ImageInfo := pTWzlImageInfo(Integer(m_FileStream.Memory) + Position);
    FileData := Pointer(Integer(ImageInfo) + SizeOf(TWzlImageInfo));
    {$IFEND}

    if ((Abs(ImageInfo.px) > MAX_IMAGE_WIDTH) or (Abs(ImageInfo.nHeight) > MAX_IMAGE_HEIGHT)) then begin
      OutMessage(Format(DecodeResStr(SWzlGrayImageErr), [FileName, Index, ImageInfo.px, ImageInfo.py]));

      DXImage.dwLatestGrayTime := MyGetTickCount;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;
      DXImage.Gray := NULLTexture;

      Exit;
    end;

    if (ImageInfo.Length >= MAX_IMAGE_SIZE) then begin
      OutMessage(Format(DecodeResStr(SWzlGrayImageLenErr), [FileName, Index, ImageInfo.Length]));

      DXImage.dwLatestGrayTime := MyGetTickCount;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;
      DXImage.Gray := NULLTexture;

      Exit;
    end;

    if ((ImageInfo.nWidth >= MAX_IMAGE_WIDTH) or (ImageInfo.nHeight >= MAX_IMAGE_HEIGHT)) then begin
      OutMessage(Format(DecodeResStr(SWzlGrayImageSizeErr), [FileName, Index, ImageInfo.nWidth, ImageInfo.nHeight]));

      DXImage.dwLatestGrayTime := MyGetTickCount;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;
      DXImage.Gray := NULLTexture;

      Exit;
    end;

    if (ImageInfo.nWidth * ImageInfo.nHeight <= 0) or (ImageInfo.nWidth <= 0) then begin // 空图片
      DXImage.dwLatestGrayTime := MyGetTickCount;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;
      DXImage.Gray := NULLTexture;
      Exit;
    end;

    DXImage.dwLatestGrayTime := MyGetTickCount;
    DXImage.nWidth := ImageInfo.nWidth;
    DXImage.nHeight := ImageInfo.nHeight;
    DXImage.nPx := ImageInfo.px;
    DXImage.nPy := ImageInfo.py;

    //Source := nil;
    //nSize := 0; //HZQ 02230525 //预置解压缩估算值0
    (*case ImageInfo.PixelFormat of
      pf8bit:begin
          Source := TDIB.Create;
          nSize := WidthBytes(8, ImageInfo.nWidth) * ImageInfo.nHeight;
          Move(g_DefColorTable, Source.ColorTable, SizeOf(g_DefColorTable));
          Source.UpdatePalette;
          Source.SetSize(ImageInfo.nWidth, ImageInfo.nHeight, 8);
        end;
      pf15bit:begin
          Source := TDIB.Create;
          nSize := WidthBytes(16, ImageInfo.nWidth) * ImageInfo.nHeight;
          Source.PixelFormat := MakeDIBPixelFormat(5, 5, 5);
          Source.SetSize(ImageInfo.nWidth, ImageInfo.nHeight, 16);
        end;
      pf16bit:begin
          Source := TDIB.Create;
          nSize := WidthBytes(16, ImageInfo.nWidth) * ImageInfo.nHeight;
          Source.PixelFormat := MakeDIBPixelFormat(5, 6, 5);
          Source.SetSize(ImageInfo.nWidth, ImageInfo.nHeight, 16);
        end;
      pf24bit:begin
          Source := TDIB.Create;
          nSize := WidthBytes(24, ImageInfo.nWidth) * ImageInfo.nHeight;
          Source.PixelFormat := MakeDIBPixelFormat(8, 8, 8);
          Source.SetSize(ImageInfo.nWidth, ImageInfo.nHeight, 24);
        end;
      pf32bit:begin
          Source := TDIB.Create;
          nSize := WidthBytes(32, ImageInfo.nWidth) * ImageInfo.nHeight;
          Source.PixelFormat := MakeDIBPixelFormat(8, 8, 8);
          Source.SetSize(ImageInfo.nWidth, ImageInfo.nHeight, 32);
        end;
    end; *)
    Source := MakeDibByPixelFormat(ImageInfo.PixelFormat, ImageInfo.nWidth, ImageInfo.nHeight);

    boDecompressError := False;
    lsAlpha := nil;

    if Source <> nil then begin
      nSize := Source.WidthBytes * Source.Height; 
      Source.Canvas.Brush.Color := clBlack;
      Source.Canvas.FillRect(Source.Canvas.ClipRect);

      if ImageInfo.Length > 0 then begin
        GetMem(InBuf, ImageInfo.Length);
        {$IF USEMAPSTREAM = 0}

        LockFileStream;
        try
          m_FileStream.Position := Position + SizeOf(TWzlImageInfo); // add chongchong 2016-01-09
          m_FileStream.Read(InBuf^, ImageInfo.Length);
        finally
          UnLockFileStream;
        end;

        {$ELSE}
        InBuf := FileData;
        {$IFEND}
        try
          DecompressBuf(InBuf, ImageInfo.Length, nSize, OutBuf, OutBytes);
        except
          boDecompressError := True;
          //OutMessage('[Exception] TWzlImages::LoadDxGrayImage DecompressBuf');
          OutMessage(Format(DecodeResStr(SWZLGrayImageDecompErr), [FileName, Index]));
        end;

        if (OutBuf <> nil) and (OutBytes > 0) then begin
          Move(OutBuf^, Source.PBits^, nSize);

          if ResetWZLAlpha then begin
            SourceAlphaSize := OutBytes - Source.Size;
            if SourceAlphaSize = (ImageInfo.nWidth * ImageInfo.nHeight div 2) then begin
              SourceAlphaBuf := PByte(Integer(OutBuf) + Source.Size);

              lsAlpha := TDIB.Create;
              lsAlpha.SetSize(ImageInfo.nWidth, ImageInfo.nHeight, 8);

              for nY := 0 to lsAlpha.Height - 1 do begin
                LineData := lsAlpha.ScanLine[nY];
                Integer(pData) := Integer(SourceAlphaBuf) + nY * lsAlpha.Width div 2;

                for nX := 0 to lsAlpha.Width - 1 do begin
                  nX_2 := nX div 2;
                  if nX mod 2 = 0 then
                    LineData^ := pData[nX_2]
                  else
                    LineData^ := (pData[nX_2] + pData[nX_2 + 1]) div 2;

                  Inc(LineData);
                end;
              end;
            end;
          end;

          FreeMem(OutBuf);
        end;

        FreeMem(InBuf);
      end
      else begin
        LockFileStream;
        try
          m_FileStream.Position := Position + SizeOf(TWzlImageInfo); // add chongchong 2016-01-09
          m_FileStream.Read(Source.PBits^, nSize);
        finally
          UnLockFileStream;
        end;
      end;

      if D3DFormat and (Source.Width >= 400) and (Source.Height >= 400) then begin
        FileDataGray32(Source, FileData, FileSize);
        if (FileSize > 0) and (FileData <> nil) then begin
          DXImage.Gray := NewTexture(FileData, FileSize, Source.Width, Source.Height, TransparentColor, D3DFormat);
          FreeMem(FileData);
        end;
      end
      else begin
        if not boDecompressError then
          DXImage.Gray := NewTextureGray(Source, lsAlpha)
        else
          DXImage.Gray := NULLTexture;
      end;

      if lsAlpha <> nil then
        lsAlpha.Free;

      Source.Free;
    end;

  end;
end;

procedure TWzlImages.LoadDxImage(Position:Integer; DXImage:pTDXImage; Index:Integer);
var
  {$IF USEMAPSTREAM = 0}
  ImageInfo:TWzlImageInfo;
  {$ELSE}
  ImageInfo:pTWzlImageInfo;
  {$IFEND}
  nSize:Integer;
  Source:TDIB;
  FileData:Pointer;
  FileSize:Integer;

  InBuf:Pointer;

  OutBuf:Pointer;
  OutBytes:Integer;
  boDecompressError:Boolean;

  SourceAlphaSize:Integer;
  SourceAlphaBuf:PByte;
  LineData:PByte;
  pData:PByteArray;
  lsAlpha:TDIB;
  nX, nY, nX_2:Integer;
begin
  if (DXImage.Surface = nil) then begin
    if (Position = 0) or (Position > m_FileStream.Size - SizeOf(TWzlImageInfo)) then begin // 可能需要更新资源，可能是空图片
      DXImage.dwLatestTime := MyGetTickCount;
      DXImage.nPx := 0; //ImageInfo.px;
      DXImage.nPy := 0; //ImageInfo.py;
      Exit;
    end;

    {$IF USEMAPSTREAM = 0}
    LockFileStream;
    try
      m_FileStream.Position := Position;
      m_FileStream.Read(ImageInfo, SizeOf(TWzlImageInfo));
    finally
      UnLockFileStream;
    end;

    {$ELSE}
    ImageInfo := pTWzlImageInfo(Integer(m_FileStream.Memory) + Position);
    FileData := Pointer(Integer(ImageInfo) + SizeOf(TWzlImageInfo));
    {$IFEND}

    if ((Abs(ImageInfo.px) > MAX_IMAGE_WIDTH) or (Abs(ImageInfo.nHeight) > MAX_IMAGE_HEIGHT)) then begin
      OutMessage(Format(DecodeResStr(SWzlImageErr), [FileName, Index, ImageInfo.px, ImageInfo.py]));

      DXImage.dwLatestTime := MyGetTickCount;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;
      DXImage.Surface := NULLTexture;

      Exit;
    end;

    if (ImageInfo.Length >= MAX_IMAGE_SIZE) then begin
      OutMessage(Format(DecodeResStr(SWzlImageLenErr), [FileName, Index, ImageInfo.Length]));

      DXImage.dwLatestTime := MyGetTickCount;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;
      DXImage.Surface := NULLTexture;

      Exit;
    end;

    if ((ImageInfo.nWidth >= MAX_IMAGE_WIDTH) or (ImageInfo.nHeight >= MAX_IMAGE_HEIGHT)) then begin
      OutMessage(Format(DecodeResStr(SWzlImageSizeErr), [FileName, Index, ImageInfo.nWidth, ImageInfo.nHeight]));

      DXImage.dwLatestTime := MyGetTickCount;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;
      DXImage.Surface := NULLTexture;

      Exit;
    end;

    if (ImageInfo.nWidth * ImageInfo.nHeight <= 0) or (ImageInfo.nWidth <= 0) then begin // 空图片
      DXImage.dwLatestTime := MyGetTickCount;
      DXImage.nPx := ImageInfo.px;
      DXImage.nPy := ImageInfo.py;
      DXImage.Surface := NULLTexture;
      Exit;
    end;

    DXImage.dwLatestTime := MyGetTickCount;
    DXImage.nWidth := ImageInfo.nWidth;
    DXImage.nHeight := ImageInfo.nHeight;
    DXImage.nPx := ImageInfo.px;
    DXImage.nPy := ImageInfo.py;

    //Source := nil;
    //nSize := 0;
      (*
      case ImageInfo.PixelFormat of
      pf8bit:begin
          //Source := TDIB.Create;
          //nSize := WidthBytes(8, ImageInfo.nWidth) * ImageInfo.nHeight;
          //Move(g_DefColorTable, Source.ColorTable, SizeOf(g_DefColorTable));
          //Source.UpdatePalette;
          //Source.SetSize(ImageInfo.nWidth, ImageInfo.nHeight, 8);
        end;
      pf15bit:begin
          //Source := TDIB.Create;
          //nSize := WidthBytes(16, ImageInfo.nWidth) * ImageInfo.nHeight;
          //Source.PixelFormat := MakeDIBPixelFormat(5, 5, 5);
          //Source.SetSize(ImageInfo.nWidth, ImageInfo.nHeight, 16);
        end;
      pf16bit:begin
          //Source := TDIB.Create;
          //nSize := WidthBytes(16, ImageInfo.nWidth) * ImageInfo.nHeight;
          //Source.PixelFormat := MakeDIBPixelFormat(5, 6, 5);
          //Source.SetSize(ImageInfo.nWidth, ImageInfo.nHeight, 16);
        end;
      pf24bit:begin
          //Source := TDIB.Create;
          //nSize := WidthBytes(24, ImageInfo.nWidth) * ImageInfo.nHeight;
          //Source.PixelFormat := MakeDIBPixelFormat(8, 8, 8);
          //Source.SetSize(ImageInfo.nWidth, ImageInfo.nHeight, 24);
        end;
      pf32bit:begin
          //Source := TDIB.Create;
          //nSize := WidthBytes(32, ImageInfo.nWidth) * ImageInfo.nHeight;
          //Source.PixelFormat := MakeDIBPixelFormat(8, 8, 8);
          //Source.SetSize(ImageInfo.nWidth, ImageInfo.nHeight, 32);
        end;
    end;  *)
    Source := MakeDibByPixelFormat(ImageInfo.PixelFormat, ImageInfo.nWidth, ImageInfo.nHeight);

    lsAlpha := nil;
    if Source <> nil then begin
      nSize := Source.WidthBytes * Source.Height;
      Source.Canvas.Brush.Color := clBlack;
      Source.Canvas.FillRect(Source.Canvas.ClipRect);

      boDecompressError := False;
      if ImageInfo.Length > 0 then begin // 需要解压
        GetMem(InBuf, ImageInfo.Length);
        {$IF USEMAPSTREAM = 0}
        LockFileStream;
        try
          m_FileStream.Position := Position + SizeOf(TWzlImageInfo); // add chongchong 2016-01-09
          m_FileStream.Read(InBuf^, ImageInfo.Length);
        finally
          UnLockFileStream;
        end;
        {$ELSE}
        InBuf := FileData;
        {$IFEND}

        OutBuf := nil;
        OutBytes := 0;
        try
          DecompressBuf(InBuf, ImageInfo.Length, nSize, OutBuf, OutBytes);
        except
          boDecompressError := True;
          //OutMessage('[Exception] TWzlImages::LoadDxImage DecompressBuf');
          OutMessage(Format(DecodeResStr(SWZLImageDecompressErr), [FileName, Index]));
        end;

        if (OutBuf <> nil) and (OutBytes > 0) then begin
          Move(OutBuf^, Source.PBits^, nSize);

          if ResetWZLAlpha then begin
            SourceAlphaSize := OutBytes - Source.Size;
            if SourceAlphaSize = (ImageInfo.nWidth * ImageInfo.nHeight div 2) then begin
              SourceAlphaBuf := PByte(Integer(OutBuf) + Source.Size);

              lsAlpha := TDIB.Create;
              lsAlpha.SetSize(ImageInfo.nWidth, ImageInfo.nHeight, 8);

              for nY := 0 to lsAlpha.Height - 1 do begin
                LineData := lsAlpha.ScanLine[nY];
                Integer(pData) := Integer(SourceAlphaBuf) + nY * lsAlpha.Width div 2;

                for nX := 0 to lsAlpha.Width - 1 do begin
                  nX_2 := nX div 2;

                  if nX mod 2 = 0 then
                    LineData^ := pData[nX_2]
                  else
                    LineData^ := (pData[nX_2] + pData[nX_2 + 1]) div 2;

                  Inc(LineData);
                end;
              end;
            end;
          end;

          FreeMem(OutBuf);
        end;
        FreeMem(InBuf);
      end else begin // 没有压缩
        LockFileStream;
        try
          m_FileStream.Position := Position + SizeOf(TWzlImageInfo); // add chongchong 2016-01-09
          m_FileStream.Read(Source.PBits^, nSize);
        finally
          UnLockFileStream;
        end;
      end;

      if D3DFormat and (Source.Width >= 400) and (Source.Height >= 400) then begin
        FileData32(Source, FileData, FileSize);
        if (FileSize > 0) and (FileData <> nil) then begin
          DXImage.Surface := NewTexture(FileData, FileSize, Source.Width, Source.Height, TransparentColor, D3DFormat);
          FreeMem(FileData);
        end;
      end else begin
        // 修改chongchong 2015-07-30 14:05:59
        if not boDecompressError then begin
          DXImage.Surface := NewTexture(Source, lsAlpha);
        end
        else
          DXImage.Surface := NULLTexture;
      end;

      if lsAlpha <> nil then
        lsAlpha.Free;

      Source.Free;
    end;
  end;
end;

procedure TWzlImages.LoadDxBitmap(Position:Integer; DXImage:pTDXImage; Index:Integer);
begin

end;

function TWzlImages.GetCachedBitmap(Index:Integer):TBitmap;
var
  nPosition:integer;
begin
  Result := nil;
  if (Index < 0) then Exit;

  Lock;
  try
    if (Index < ImageCount) and (Index < m_IndexList.Count) and (m_FileStream <> nil) and (Initialized) then begin
      if m_ImgArr[Index].Bitmap = nil then begin
        nPosition := Integer(m_IndexList[Index]);
        LoadDxBitmap(nPosition, @m_ImgArr[Index], Index);
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

function TWzlImages.GetCachedSurface(Index:Integer):TTexture;
var
  nPosition:Integer;
begin
  Result := nil;
  if (Index < 0) then Exit;

  Lock;
  try
    if (Index < ImageCount) and (Index < m_IndexList.Count) and Initialized then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Surface = nil then begin
        nPosition := Integer(m_IndexList[Index]);
        LoadDxImage(nPosition, @m_ImgArr[Index], Index);
        m_ImgArr[Index].dwLatestTime := MyGetTickCount;

        if m_ImgArr[Index].Surface <> nil then
          IndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Surface;

        {$IF CLIENTEXE = 1}
        if (Result = nil) and g_boAutoUpdate and m_boNeedUpdate and (not m_ImgArr[Index].boUpdateStop) and
          ((not m_ImgArr[Index].boUpdateStart) or (MyGetTickCount - m_ImgArr[Index].dwUpdateStartTick >= g_UpdateRetryTime)) and
          g_boDeviceInitializeOK then begin
          if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtImageWzl, Index, Self, nil) then begin
            m_ImgArr[Index].boUpdateStart := True;
            m_ImgArr[Index].dwUpdateStartTick := MyGetTickCount;
          end;
        end;
        {$IFEND}
        if Result = nil then
          Result := g_NullImage;
      end
      else begin
        m_ImgArr[Index].dwLatestTime := MyGetTickCount;
        Result := m_ImgArr[Index].Surface;
      end;
    end
    else begin
      {$IF CLIENTEXE = 1}
      if m_boUpdateIndex and ((not m_boUpdateIndexing) or (MyGetTickCount - m_dwUpdateIndexingTick >= g_UpdateRetryTime)) and g_boDeviceInitializeOK and g_boAutoUpdate and m_boNeedUpdate then begin
        if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtIndexWzl, -1, Self, nil) then begin
          m_boUpdateIndexing := True;
          m_dwUpdateIndexingTick := MyGetTickCount;
        end;
      end;
      {$IFEND}
    end;
  finally
    UnLock;
  end;
end;

function TWzlImages.GetCachedGray(Index:Integer):TTexture;
var
  nPosition:Integer;
begin
  Result := nil;
  if (Index < 0) then Exit;

  Lock;
  try
    if (Index < ImageCount) and (Index < m_IndexList.Count) and Initialized then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Gray = nil then begin
        nPosition := Integer(m_IndexList[Index]);
        LoadDxGrayImage(nPosition, @m_ImgArr[Index], Index);
        m_ImgArr[Index].dwLatestGrayTime := MyGetTickCount;

        if m_ImgArr[Index].Gray <> nil then
          GrayIndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Gray;

        {$IF CLIENTEXE = 1}
        if (Result = nil) and g_boAutoUpdate and m_boNeedUpdate and (not m_ImgArr[Index].boUpdateStop) and
          ((not m_ImgArr[Index].boUpdateStart) or (MyGetTickCount - m_ImgArr[Index].dwUpdateStartTick >= g_UpdateRetryTime)) and
          g_boDeviceInitializeOK then begin
          if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtImageWzl, Index, Self, nil) then begin
            m_ImgArr[Index].boUpdateStart := True;
            m_ImgArr[Index].dwUpdateStartTick := MyGetTickCount;
          end;
        end;
        {$IFEND}
        if Result = nil then
          Result := g_NullImage;
      end
      else begin
        m_ImgArr[Index].dwLatestGrayTime := MyGetTickCount;
        Result := m_ImgArr[Index].Gray;
      end;
    end
    else begin
      {$IF CLIENTEXE = 1}
      if m_boUpdateIndex and ((not m_boUpdateIndexing) or (MyGetTickCount - m_dwUpdateIndexingTick >= g_UpdateRetryTime)) and g_boDeviceInitializeOK and g_boAutoUpdate and m_boNeedUpdate then begin
        if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtIndexWzl, -1, Self, nil) then begin
          m_boUpdateIndexing := True;
          m_dwUpdateIndexingTick := MyGetTickCount;
        end;
      end;
      {$IFEND}
    end;
  finally
    UnLock;
  end;
end;

function TWzlImages.GetCachedBright(Index:Integer):TTexture;
var
  nPosition:Integer;
begin
  Result := nil;
  if (Index < 0) then Exit;

  Lock;
  try
    if (Index < ImageCount) and (Index < m_IndexList.Count) and Initialized then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Bright = nil then begin
        nPosition := Integer(m_IndexList[Index]);
        LoadDxBrightImage(nPosition, @m_ImgArr[Index], Index);
        m_ImgArr[Index].dwLatestBrightTime := MyGetTickCount;

        if m_ImgArr[Index].Bright <> nil then
          BrightIndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Bright;

        {$IF CLIENTEXE = 1}
        if (Result = nil) and g_boAutoUpdate and m_boNeedUpdate and (not m_ImgArr[Index].boUpdateStop) and
          ((not m_ImgArr[Index].boUpdateStart) or (MyGetTickCount - m_ImgArr[Index].dwUpdateStartTick >= g_UpdateRetryTime)) and
          g_boDeviceInitializeOK then begin
          if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtImageWzl, Index, Self, nil) then begin
            m_ImgArr[Index].boUpdateStart := True;
            m_ImgArr[Index].dwUpdateStartTick := MyGetTickCount;
          end;
        end;
        {$IFEND}
        if Result = nil then
          Result := g_NullImage;
      end
      else begin
        m_ImgArr[Index].dwLatestBrightTime := MyGetTickCount;
        Result := m_ImgArr[Index].Bright;
      end;
    end
    else begin
      {$IF CLIENTEXE = 1}
      if m_boUpdateIndex and ((not m_boUpdateIndexing) or (MyGetTickCount - m_dwUpdateIndexingTick >= g_UpdateRetryTime)) and g_boDeviceInitializeOK and g_boAutoUpdate and m_boNeedUpdate then begin
        if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtIndexWzl, -1, Self, nil) then begin
          m_boUpdateIndexing := True;
          m_dwUpdateIndexingTick := MyGetTickCount;
        end;
      end;
      {$IFEND}
    end;
  finally
    UnLock;
  end;
end;

function TWzlImages.GetCachedImage(Index:Integer; var PX, PY:Integer):TTexture;
var
  nPosition:Integer;
begin
  Result := nil;
  if (Index < 0) then Exit;

  Lock;
  try
    if (Index < ImageCount) and (Index < m_IndexList.Count) and Initialized then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Surface = nil then begin
        nPosition := Integer(m_IndexList[Index]);
        LoadDxImage(nPosition, @m_ImgArr[Index], Index);

        m_ImgArr[Index].dwLatestTime := MyGetTickCount;
        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;

        if m_ImgArr[Index].Surface <> nil then
          IndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Surface;
        {$IF CLIENTEXE = 1}
        if (Result = nil) and g_boAutoUpdate and m_boNeedUpdate and (not m_ImgArr[Index].boUpdateStop) and
          ((not m_ImgArr[Index].boUpdateStart) or (MyGetTickCount - m_ImgArr[Index].dwUpdateStartTick >= g_UpdateRetryTime)) and
          g_boDeviceInitializeOK then begin
          if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtImageWzl, Index, Self, nil) then begin
            m_ImgArr[Index].boUpdateStart := True;
            m_ImgArr[Index].dwUpdateStartTick := MyGetTickCount;
          end;
        end;
        {$IFEND}
        if Result = nil then
          Result := g_NullImage;
      end
      else begin
        m_ImgArr[Index].dwLatestTime := MyGetTickCount;
        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;
        Result := m_ImgArr[Index].Surface;
      end;
    end
    else begin
      {$IF CLIENTEXE = 1}
      if m_boUpdateIndex and ((not m_boUpdateIndexing) or (MyGetTickCount - m_dwUpdateIndexingTick >= g_UpdateRetryTime)) and g_boDeviceInitializeOK and g_boAutoUpdate and m_boNeedUpdate then begin
        if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtIndexWzl, -1, Self, nil) then begin
          m_boUpdateIndexing := True;
          m_dwUpdateIndexingTick := MyGetTickCount;
        end;
      end;
      {$IFEND}
    end;
  finally
    UnLock;
  end;
end;

function TWzlImages.GetCachedImageSize(AIndex:Integer; var ASize:TSize; var APoint:TPoint):Boolean;
var
  nPosition:Integer;
  {$IF USEMAPSTREAM = 0}
  ImageInfo:TWzlImageInfo;
  {$ELSE}
  ImageInfo:pTWzlImageInfo;
  {$IFEND}
begin
  Result := False;
  Lock;
  try
    if (AIndex >= 0) and (AIndex < ImageCount) and (AIndex < m_IndexList.Count) and (m_FileStream <> nil) and (Initialized) then begin
      if m_ImgArr[AIndex].nWidth * m_ImgArr[AIndex].nHeight = 0 then begin
        nPosition := Integer(m_IndexList[AIndex]);

        if (nPosition > 0) and (nPosition < m_FileStream.Size) then begin
          {$IF USEMAPSTREAM = 0}
          LockFileStream;
          try
            m_FileStream.Seek(nPosition, soBeginning);
            m_FileStream.Read(ImageInfo, SizeOf(TWzlImageInfo));
          finally
            UnLockFileStream;
          end;
          {$ELSE}
          ImageInfo := pTWzlImageInfo(Integer(m_FileStream.Memory) + nPosition);
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
  finally
    UnLock;
  end;
end;

function TWzlImages.GetCachedGrayImage(Index:Integer; var px, py:Integer):TTexture;
var
  nPosition:Integer;
begin
  Result := nil;
  if (Index < 0) then Exit;

  Lock;
  try
    if (Index < ImageCount) and (Index < m_IndexList.Count) and Initialized then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Gray = nil then begin
        nPosition := Integer(m_IndexList[Index]);
        LoadDxGrayImage(nPosition, @m_ImgArr[Index], Index);
        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;
        m_ImgArr[Index].dwLatestGrayTime := MyGetTickCount;

        if m_ImgArr[Index].Gray <> nil then
          GrayIndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Gray;
        {$IF CLIENTEXE = 1}
        if (Result = nil) and g_boAutoUpdate and m_boNeedUpdate and (not m_ImgArr[Index].boUpdateStop) and
          ((not m_ImgArr[Index].boUpdateStart) or (MyGetTickCount - m_ImgArr[Index].dwUpdateStartTick >= g_UpdateRetryTime)) and
          g_boDeviceInitializeOK then begin
          if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtImageWzl, Index, Self, nil) then begin
            m_ImgArr[Index].boUpdateStart := True;
            m_ImgArr[Index].dwUpdateStartTick := MyGetTickCount;
          end;
        end;
        {$IFEND}
        if Result = nil then
          Result := g_NullImage;
      end
      else begin
        m_ImgArr[Index].dwLatestGrayTime := MyGetTickCount;
        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;
        Result := m_ImgArr[Index].Gray;
      end;
    end
    else begin
      {$IF CLIENTEXE = 1}
      if m_boUpdateIndex and ((not m_boUpdateIndexing) or (MyGetTickCount - m_dwUpdateIndexingTick >= g_UpdateRetryTime)) and g_boDeviceInitializeOK and g_boAutoUpdate and m_boNeedUpdate then begin
        if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtIndexWzl, -1, Self, nil) then begin
          m_boUpdateIndexing := True;
          m_dwUpdateIndexingTick := MyGetTickCount;
        end;
      end;
      {$IFEND}
    end;
  finally
    UnLock;
  end;
end;

function TWzlImages.GetCachedBrightImage(Index:Integer; var px, py:Integer):TTexture;
var
  nPosition:Integer;
begin
  Result := nil;
  if (Index < 0) then Exit;

  Lock;
  try
    if (Index < ImageCount) and (Index < m_IndexList.Count) and (Initialized) then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Bright = nil then begin
        nPosition := Integer(m_IndexList[Index]);
        LoadDxBrightImage(nPosition, @m_ImgArr[Index], Index);
        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;
        m_ImgArr[Index].dwLatestBrightTime := MyGetTickCount;

        if m_ImgArr[Index].Bright <> nil then
          BrightIndexList.Add(Pointer(Index));

        Result := m_ImgArr[Index].Bright;
        {$IF CLIENTEXE = 1}
        if (Result = nil) and g_boAutoUpdate and m_boNeedUpdate and (not m_ImgArr[Index].boUpdateStop) and
          ((not m_ImgArr[Index].boUpdateStart) or (MyGetTickCount - m_ImgArr[Index].dwUpdateStartTick >= g_UpdateRetryTime)) and
          g_boDeviceInitializeOK then begin
          if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtImageWzl, Index, Self, nil) then begin
            m_ImgArr[Index].boUpdateStart := True;
            m_ImgArr[Index].dwUpdateStartTick := MyGetTickCount;
          end;
        end;
        {$IFEND}
        if Result = nil then
          Result := g_NullImage;
      end
      else begin
        m_ImgArr[Index].dwLatestBrightTime := MyGetTickCount;
        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;
        Result := m_ImgArr[Index].Bright;
      end;
    end
    else begin
      {$IF CLIENTEXE = 1}
      if m_boUpdateIndex and ((not m_boUpdateIndexing) or (MyGetTickCount - m_dwUpdateIndexingTick >= g_UpdateRetryTime)) and g_boDeviceInitializeOK and g_boAutoUpdate and m_boNeedUpdate then begin
        if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(FileName, udtIndexWzl, -1, Self, nil) then begin
          m_boUpdateIndexing := True;
          m_dwUpdateIndexingTick := MyGetTickCount;
        end;
      end;
      {$IFEND}
    end;
  finally
    UnLock;
  end;
end;

function TWzlImages.GetBitmap(Index:Integer; var PX, PY:Integer):TBitmap;
var
  nPosition:integer;
begin
  Result := nil;
  if (Index < 0) then Exit;

  Lock;
  try
    if Initialized and (Index < ImageCount) and (Index < m_IndexList.Count) and Initialized then begin
      if m_ImgArr[Index].Bitmap = nil then begin
        nPosition := Integer(m_IndexList[Index]);
        LoadDxBitmap(nPosition, @m_ImgArr[Index], Index);
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

procedure TWzlImages.LockFileStream;
begin
  EnterCriticalSection(FCSFileStream);
end;

procedure TWzlImages.UnLockFileStream;
begin
  LeaveCriticalSection(FCSFileStream);
end;

end.
