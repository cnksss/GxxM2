unit Uib;

interface
uses
  Windows,
  Classes,
  Graphics,
  SysUtils,
  DIB,
  HGE,
  DxCanvas,
  GameImages;
type
  TUibImages = class(TGameImages)
    m_FileList:TStringList;
  private
    procedure LoadDxImage(const AFileName:string; DXImage:pTDXImage);
    procedure LoadDxBrightImage(const AFileName:string; DXImage:pTDXImage);
    procedure LoadDxGrayImage(const AFileName:string; DXImage:pTDXImage);

    function GetSurfaceByName(Name:string):TTexture;
    function GetIndexByName(Name:string):Integer;
  protected
    function GetCachedSurface(Index:Integer):TTexture; override;
    function GetCachedGray(Index:Integer):TTexture; override;
    function GetCachedBright(Index:Integer):TTexture; override;
  public
    constructor Create();
    destructor Destroy; override;
    procedure Initialize; override;
    procedure Finalize; override;
    procedure StreamSaveToFile(Sender:Tobject; Stream:TMemoryStream; Index:Integer; const FileName:string);
    function GetCachedImage(Index:Integer; var PX, PY:Integer):TTexture; override;
    function GetCachedGrayImage(Index:Integer; var px, py:Integer):TTexture; override;
    function GetCachedBrightImage(Index:Integer; var px, py:Integer):TTexture; override;
    function GetCachedImageSize(AIndex:Integer; var ASize:TSize; var APoint:TPoint):Boolean; override;
    property Names[Name:string]:TTexture read GetSurfaceByName;
  end;

implementation

uses Math,
  MShare,
  UpdateEngine;

constructor TUibImages.Create();
begin
  inherited Create;
  m_FileList := TStringList.Create;
end;

destructor TUibImages.Destroy;
begin
  m_FileList.Free;
  inherited;
end;

procedure TUibImages.Initialize;
begin
  if not Initialized then begin
    BitCount := 8;
    ImageCount := m_FileList.Count;
    // FileName := 'Data\';
    m_ImgArr := AllocMem(SizeOf(TDXImage) * ImageCount);
    Initialized := True;
  end;
end;

procedure TUibImages.Finalize;
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
  finally
    UnLock;
  end;
end;

procedure TUibImages.LoadDxBrightImage(const AFileName:string; DXImage:pTDXImage);
var
  Source:TDIB;
  FileData:Pointer;
  FileSize:Integer;
begin
  if FileExists(AFileName) then begin
    Source := TDIB.Create;
    try
      Source.LoadFromFile(AFileName);
    except
      Source.Free;
      Exit;
    end;

    if (Source <> nil) and (Source.Width * Source.Height > 4) then begin
      DXImage.dwLatestBrightTime := MyGetTickCount;
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
    end;
    Source.Free;
  end;
end;

procedure TUibImages.LoadDxGrayImage(const AFileName:string; DXImage:pTDXImage);
var
  Source:TDIB;
  FileData:Pointer;
  FileSize:Integer;
begin
  if FileExists(AFileName) then begin
    Source := TDIB.Create;
    try
      Source.LoadFromFile(AFileName);
    except
      Source.Free;
      Exit;
    end;

    if (Source <> nil) and (Source.Width * Source.Height > 4) then begin
      DXImage.dwLatestGrayTime := MyGetTickCount;

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
    end;
    Source.Free;
  end;
end;

procedure TUibImages.LoadDxImage(const AFileName:string; DXImage:pTDXImage);
var
  Source:TDIB;
  FileData:Pointer;
  FileSize:Integer;
begin
  if FileExists(AFileName) then begin
    Source := TDIB.Create;
    try
      Source.LoadFromFile(AFileName);
    except
      Source.Free;
      Exit;
    end;

    if (Source <> nil) and (Source.Width * Source.Height > 4) then begin
      DXImage.dwLatestTime := MyGetTickCount;

      if D3DFormat and (Source.Width >= 400) and (Source.Height >= 400) then begin
        FileData32(Source, FileData, FileSize);
        if (FileSize > 0) and (FileData <> nil) then begin
          DXImage.Surface := NewTexture(FileData, FileSize, Source.Width, Source.Height, TransparentColor, D3DFormat);
          FreeMem(FileData);
        end;
      end else begin
        DXImage.Surface := NewTexture(Source);
      end;
    end;
    Source.Free;
  end;
end;

function TUibImages.GetIndexByName(Name:string):Integer;
var
  I:integer;
begin
  Result := -1;
  for I := 0 to m_FileList.Count - 1 do
    if (LowerCase(Name) = LowerCase(m_FileList.Strings[I])) then begin
      Result := I; // Integer(m_FileList.Objects[I]);
      Break;
    end;
  if Result < 0 then begin
    m_FileList.Add(Name);
    Result := m_FileList.Count - 1;
  end;
  ImageCount := m_FileList.Count;
end;

function TUibImages.GetCachedSurface(Index:Integer):TTexture;
var
  sFileName:string;
begin
  Result := nil;
  Lock;
  try
    if (Index >= 0) and (Index < ImageCount) and (Initialized) then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Surface = nil then begin
        IndexList.Add(Pointer(Index));
        LoadDxImage(m_FileList.Strings[Index], @m_ImgArr[Index]);
        Result := m_ImgArr[Index].Surface;
        if g_boAutoUpdate and (Result = nil) and (not m_ImgArr[Index].boUpdateStop) and (not m_ImgArr[Index].boUpdateStart) and
          g_boDeviceInitializeOK then begin
          sFileName := m_FileList.Strings[Index];
          if (sFileName <> '') then begin
            if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(sFileName, udtFileOther, Index, nil, StreamSaveToFile) then
              m_ImgArr[Index].boUpdateStart := True;
          end;
        end;
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

function TUibImages.GetCachedGray(Index:Integer):TTexture;
var
  sFileName:string;
begin
  Result := nil;
  Lock;
  try
    if (Index >= 0) and (Index < ImageCount) and (Initialized) then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Gray = nil then begin
        GrayIndexList.Add(Pointer(Index));
        LoadDxGrayImage(m_FileList.Strings[Index], @m_ImgArr[Index]);
        Result := m_ImgArr[Index].Gray;
        if g_boAutoUpdate and (Result = nil) and (not m_ImgArr[Index].boUpdateStop) and (not m_ImgArr[Index].boUpdateStart) and
          g_boDeviceInitializeOK then begin
          sFileName := m_FileList.Strings[Index];
          if (sFileName <> '') then begin
            if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(sFileName, udtFileOther, Index, nil, StreamSaveToFile) then
              m_ImgArr[Index].boUpdateStart := True;
          end;
        end;
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

function TUibImages.GetCachedBright(Index:Integer):TTexture;
var
  sFileName:string;
begin
  Result := nil;
  if (Index >= 0) and (Index < ImageCount) and (Initialized) then begin
    Lock;
    try
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Bright = nil then begin
        BrightIndexList.Add(Pointer(Index));
        LoadDxBrightImage(m_FileList.Strings[Index], @m_ImgArr[Index]);
        Result := m_ImgArr[Index].Bright;
        if g_boAutoUpdate and (Result = nil) and (not m_ImgArr[Index].boUpdateStop) and (not m_ImgArr[Index].boUpdateStart) and
          g_boDeviceInitializeOK then begin
          sFileName := m_FileList.Strings[Index];
          if (sFileName <> '') then begin
            if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(sFileName, udtFileOther, Index, nil, StreamSaveToFile) then
              m_ImgArr[Index].boUpdateStart := True;
          end;
        end;
      end
      else begin
        m_ImgArr[Index].dwLatestBrightTime := MyGetTickCount;
        Result := m_ImgArr[Index].Bright;
      end;
    finally
      UnLock;
    end;
  end;
end;

function TUibImages.GetCachedImage(Index:Integer; var PX, PY:Integer):TTexture;
var
  sFileName:string;
begin
  Result := nil;
  Lock;
  try
    if (Index >= 0) and (Index < ImageCount) and (Initialized) then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Surface = nil then begin
        IndexList.Add(Pointer(Index));
        LoadDxImage(m_FileList.Strings[Index], @m_ImgArr[Index]);
        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;
        Result := m_ImgArr[Index].Surface;
        if g_boAutoUpdate and (Result = nil) and (not m_ImgArr[Index].boUpdateStop)
          and (not m_ImgArr[Index].boUpdateStart) and g_boDeviceInitializeOK then begin
          sFileName := m_FileList.Strings[Index];
          if (sFileName <> '') then begin
            if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(sFileName, udtFileOther, Index, nil, StreamSaveToFile) then begin
              m_ImgArr[Index].boUpdateStart := True;
            end;
          end;
        end;
      end else begin
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

function TUibImages.GetCachedImageSize(AIndex:Integer; var ASize:TSize; var APoint:TPoint):Boolean;
var
  sFileName:string;
begin
  //HZQ 20230524尝试添加了这个函数，未经过验证，程序中也未使用
  Result := False;
  Lock;
  try
    if (AIndex >= 0) and (AIndex < ImageCount) and (Initialized) then begin
      FreeOldMemorys_Ex;
      if m_ImgArr[AIndex].Surface = nil then begin
        IndexList.Add(Pointer(AIndex));
        LoadDxImage(m_FileList.Strings[AIndex], @m_ImgArr[AIndex]);
        APoint.X := m_ImgArr[AIndex].nPx;
        APoint.Y := m_ImgArr[AIndex].nPy;
        ASize.cx := m_ImgArr[AIndex].nWidth;
        ASize.cy := m_ImgArr[AIndex].nHeight;
        Result := True;
        if g_boAutoUpdate and (m_ImgArr[AIndex].Surface = nil) and (not m_ImgArr[AIndex].boUpdateStop)
          and (not m_ImgArr[AIndex].boUpdateStart) and g_boDeviceInitializeOK then begin
          sFileName := m_FileList.Strings[AIndex];
          if (sFileName <> '') then begin
            if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(sFileName, udtFileOther, AIndex, nil, StreamSaveToFile) then begin
              m_ImgArr[AIndex].boUpdateStart := True;
            end;
          end;
        end;
      end else begin
        m_ImgArr[AIndex].dwLatestTime := MyGetTickCount;
        APoint.X := m_ImgArr[AIndex].nPx;
        APoint.Y := m_ImgArr[AIndex].nPy;
        ASize.cx := m_ImgArr[AIndex].nWidth;
        ASize.cy := m_ImgArr[AIndex].nHeight;
        Result := True;
      end;
    end;
  finally
    UnLock;
  end;
end;

function TUibImages.GetCachedGrayImage(Index:Integer; var px, py:Integer):TTexture;
var
  sFileName:string;
begin
  Result := nil;
  Lock;
  try
    if (Index >= 0) and (Index < ImageCount) and (Initialized) then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Gray = nil then begin
        GrayIndexList.Add(Pointer(Index));
        LoadDxGrayImage(m_FileList.Strings[Index], @m_ImgArr[Index]);
        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;
        Result := m_ImgArr[Index].Gray;
        if g_boAutoUpdate and (Result = nil) and (not m_ImgArr[Index].boUpdateStop) and (not m_ImgArr[Index].boUpdateStart) and
          g_boDeviceInitializeOK then begin
          sFileName := m_FileList.Strings[Index];
          if (sFileName <> '') then begin
            if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(sFileName, udtFileOther, Index, nil, StreamSaveToFile) then
              m_ImgArr[Index].boUpdateStart := True;
          end;
        end;
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

function TUibImages.GetCachedBrightImage(Index:Integer; var px, py:Integer):TTexture;
var
  sFileName:string;
begin
  Result := nil;
  Lock;
  try
    if (Index >= 0) and (Index < ImageCount) and (Initialized) then begin
      FreeOldMemorys_Ex;

      if m_ImgArr[Index].Bright = nil then begin
        BrightIndexList.Add(Pointer(Index));
        LoadDxBrightImage(m_FileList.Strings[Index], @m_ImgArr[Index]);
        PX := m_ImgArr[Index].nPx;
        PY := m_ImgArr[Index].nPy;
        Result := m_ImgArr[Index].Bright;

        if g_boAutoUpdate and (Result = nil) and (not m_ImgArr[Index].boUpdateStop) and (not m_ImgArr[Index].boUpdateStart) and
          g_boDeviceInitializeOK then begin
          sFileName := m_FileList.Strings[Index];
          if (sFileName <> '') then begin
            if (g_UpdateEngine <> nil) and g_UpdateEngine.Add(sFileName, udtFileOther, Index, nil, StreamSaveToFile) then
              m_ImgArr[Index].boUpdateStart := True;
          end;
        end;
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

function TUibImages.GetSurfaceByName(Name:string):TTexture;
var
  Index:integer;
begin
  Result := nil;
  if (m_FileList = nil) or (not Initialized) then Exit;
  Index := GetIndexByName(Name);
  if (Index < 0) then Exit;

  Lock;
  try
    if m_ImgArr[Index].Surface = nil then begin
      // IndexList.Add(Pointer(Index));
      LoadDxImage(Name, @m_ImgArr[Index]);
      m_ImgArr[Index].dwLatestTime := MyGetTickCount;
      Result := m_ImgArr[Index].Surface;
    end
    else begin
      m_ImgArr[Index].dwLatestTime := MyGetTickCount;
      Result := m_ImgArr[Index].Surface;
    end;
  finally
    UnLock;
  end;
end;

procedure TUibImages.StreamSaveToFile(Sender:TObject; Stream:TMemoryStream; Index:Integer; const FileName:string);
var
  FilePath:string;
begin
  Lock;
  try
    if Stream <> nil then begin
      FilePath := ExtractFilePath(FileName);

      if not DirectoryExists(FilePath) then begin
        ForceDirectories(FilePath);
      end;

      try
        Stream.SaveToFile(FileName);
      except

      end;

      if (Index >= 0) and (Index < ImageCount) then
        m_ImgArr[Index].boUpdateStart := False;

      Stream.Free;
    end
    else begin
      if (Index >= 0) and (Index < ImageCount) then begin
        m_ImgArr[Index].boUpdateStart := False;
        m_ImgArr[Index].boUpdateStop := True;
      end;
    end;
  finally
    UnLock;
  end;
end;

initialization

end.
