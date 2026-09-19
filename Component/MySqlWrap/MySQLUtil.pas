unit MySQLUtil;

interface

uses
  Windows, SysUtils, Classes;

{$IFDEF VER320}           // Delphi XE10.1
  {$DEFINE XE}
{$ENDIF}

{$IFDEF VER310}           // Delphi XE10.1
  {$DEFINE XE}
{$ENDIF}
{$IFDEF VER300}           // Delphi XE10
  {$DEFINE XE}
{$ENDIF}

{$IFDEF VER290}           // Delphi XE8
  {$DEFINE XE}
{$ENDIF}

{$IFDEF VER280}           // Delphi XE7
  {$DEFINE XE}
{$ENDIF}

{$IFDEF VER270}           // Delphi XE6
  {$DEFINE XE}
{$ENDIF}

{$IFDEF VER260}           // Delphi XE5
  {$DEFINE XE}
{$ENDIF}

{$IFDEF VER250}           // Delphi XE4
  {$DEFINE XE}
{$ENDIF}

{$IFDEF VER240}           // Delphi XE3
  {$DEFINE XE}
{$ENDIF}

{$IFDEF VER230}           // Delphi XE2
  {$DEFINE XE}
{$ENDIF}

{$IFDEF VER220}           // Delphi XE
  {$DEFINE XE}
{$ENDIF}


{$IFDEF XE}
  {$IFDEF WIN32}
    {$DEFINE FireDAC_32}
  {$ELSE}
    {$DEFINE FireDAC_64}
  {$ENDIF}
{$ELSE}
  {$DEFINE FireDAC_32}
{$ENDIF}


type
{$IFNDEF XE}
  IntPtr = Integer;
  PUInt64 = ^UInt64;
{$ENDIF}

  TMyVersion = UInt64;
  TMyCharSet = {$IFDEF NEXTGEN} TSysCharSet {$ELSE} set of AnsiChar {$ENDIF};
  TMyEncoding = (ecDefault, ecUTF8, ecUTF16, ecANSI);

type
  TMyMachineType = (mtUnknown, mt32Bit, mt64Bit, mtOther);

  TMyLibrary = class(TObject)
  private
    FDriverID: string;
    FProduct, FVersionName, FCopyright, FInfo: string;
    FDLLName: string;
    FFailedProcs: TStrings;
  protected
    FVersion: TMyVersion;
    FVersionStr: string;
    FDllHandle: THandle;
    procedure GetLibraryInfo; virtual;
    procedure LoadLibrary(const ADLLNames: array of string; ARequired: Boolean); virtual;
    function GetProc(const AProcName: string; ARequired: Boolean = True): Pointer; virtual;
    procedure LoadEntries; virtual; abstract;
  public
    constructor Create(const ADriverID: string; AOwningObj: TObject);
    destructor Destroy; override;
    procedure Load(const ADLLNames: array of string; ARequired: Boolean);
    procedure Unload; virtual;
    property DriverID: string read FDriverID;
    property DllHandle: THandle read FDllHandle;
    property DLLName: string read FDLLName;
    property Version: TMyVersion read FVersion;
    property Product: string read FProduct;
    property VersionStr: string read FVersionStr;
    property VersionName: string read FVersionName;
    property Copyright: string read FCopyright;
    property Info: string read FInfo;
  end;

var
  C_CodePage: Cardinal;

const
  C_MY_MaxUTF8Len = 3;

const
  // Some MySQL versions
  mvMySQL032000 = 0320000000;
  mvMySQL032300 = 0323000000;
  mvMySQL032306 = 0323060000;
  mvMySQL032314 = 0323140000;
  mvMySQL032321 = 0323210000;
  mvMySQL040000 = 0400000000;
  mvMySQL040002 = 0400020000;
  mvMySQL040018 = 0400180000;
  mvMySQL040100 = 0401000000;
  mvMySQL040101 = 0401010000;
  mvMySQL041000 = 0410000000;
  mvMySQL041100 = 0411000000;
  mvMySQL050000 = 0500000000;
  mvMySQL050002 = 0500020000;
  mvMySQL050006 = 0500060000;
  mvMySQL050007 = 0500070000;
  mvMySQL050010 = 0500100000;
  mvMySQL050023 = 0500230000;
  mvMySQL050060 = 0500600000;
  mvMySQL050100 = 0501000000;
  mvMySQL050134 = 0501340000;
  mvMySQL050200 = 0502000000;
  mvMySQL050500 = 0505000000;
  mvMySQL050503 = 0505030000;
  mvMySQL050600 = 0506000000;
  mvMySQL050700 = 0507000000;
  mvMySQL060200 = 0602000000;

  // Some MSSQL versions
  svMSSQL6    = 0600000000;
  svMSSQL7    = 0700000000;
  svMSSQL2000 = 0800000000;
  svMSSQL2005 = 0900000000;
  svMSSQL2008 = 1000000000;
  svMSSQL2012 = 1100000000;
  svMSSQL2014 = 1200000000;
  svMSSQL2016 = 1300000000;

  function MyVerStr2Int(const AVersion: string): TMyVersion;
  function MyVerInt2Str(AVersion: TMyVersion): string;

  function MyGetVersionInfo(const AFileName: string; out AProduct,
    AVersion, AVersionName, ACopyright, AInfo: string): Boolean;
  function MyGetLibMachineType(const AFileName: string): TMyMachineType;


{$IFNDEF XE}
  function GetDllDirectory(nBufferLength: DWORD; lpBuffer: LPSTR): DWORD; stdcall; external kernel32 name 'GetDllDirectoryA';
  function SetDllDirectory(lpPathName: LPCSTR): BOOL; stdcall; external kernel32 name 'SetDllDirectoryA';
{$ENDIF}

implementation

uses
  MySQLCli;

{-------------------------------------------------------------------------------}
{ TFDLibrary                                                                    }
{-------------------------------------------------------------------------------}
constructor TMyLibrary.Create(const ADriverID: string; AOwningObj: TObject);
begin
  inherited Create;
  FDriverID := ADriverID;
  FFailedProcs := TStringList.Create;
end;

{-------------------------------------------------------------------------------}
procedure TMyLibrary.Load(const ADLLNames: array of string; ARequired: Boolean);
begin
  FFailedProcs.Clear;
  LoadLibrary(ADLLNames, ARequired);
  if DllHandle <> 0 then
  begin
    GetLibraryInfo;
    LoadEntries;
  end;

  if FFailedProcs.Count > 0 then
    raise Exception.Create('CantGetLibraryEntry ' + Trim(FFailedProcs.Text));
  
  {
  if FFailedProcs.Count > 0 then
    FDException(FOwningObj, [S_FD_LPhys, FDriverID], er_FD_AccCantGetLibraryEntry,
      []);
  }
end;

{-------------------------------------------------------------------------------}
destructor TMyLibrary.Destroy;
begin
  Unload;
  FFailedProcs.Free;
  inherited Destroy;
end;

{-------------------------------------------------------------------------------}
procedure TMyLibrary.LoadLibrary(const ADLLNames: array of string; ARequired: Boolean);
var
  sLib, sLibs, sErr, sMsg: String;
  I: Integer;
{$IFDEF MSWINDOWS}
  sPath, sCurrDir: AnsiString;
{$ENDIF}
begin
  FDllHandle := 0;
  FDLLName := '';
  sMsg := '';

{$IFDEF MSWINDOWS}
  SetLength(sCurrDir, MAX_PATH);
  SetLength(sCurrDir, DWORD(GetDllDirectory(MAX_PATH, PAnsiChar(sCurrDir))));
  for I := Low(ADLLNames) to High(ADLLNames) do
  begin
    if ADLLNames[I] <> '' then
    begin
      sPath := ExtractFilePath(ADLLNames[I]);
      // If a full library path is specified, then force Windows to load the DLL's
      // at first from this folder, then from System32 or how else. This allows
      // to avoid the chances for getting a DLL version conflict.
      if sPath <> '' then
        SetDllDirectory(PAnsiChar(sPath));
      try
        FDllHandle := SafeLoadLibrary(ADLLNames[I]);
        sMsg := SysErrorMessage(GetLastError) + ' [' + ADLLNames[I] + ']';
      finally
        if sPath <> '' then
          if sCurrDir = '' then
            SetDllDirectory(nil)
          else
            SetDllDirectory(PAnsiChar(sCurrDir));
      end;
      
      if FDllHandle <> 0 then Break;
    end;
  end;
{$ENDIF}

{$IFDEF POSIX}
  for I := Low(ADLLNames) to High(ADLLNames) do
  begin
    if ADLLNames[I] <> '' then 
    begin
      FDllHandle := SafeLoadLibrary(ADLLNames[I]);
      if FDllHandle <> 0 then 
      begin
        FDLLName := ADLLNames[I];
        Break;
      end;
    end;
  end;
{$ENDIF}
       
  if (DllHandle = 0) and ARequired then
  begin
    sLibs := '';
    sErr := '';
    for I := Low(ADLLNames) to High(ADLLNames) do
    begin
      if Low(ADLLNames) <> High(ADLLNames) then
        if I = High(ADLLNames) then
          sLibs := sLibs + ' or '
        else if I > Low(ADLLNames) then
          sLibs := sLibs + ', ';
      sLibs := sLibs + ADLLNames[I];
      if MyGetLibMachineType(ADLLNames[I]) = {$IFDEF FireDAC_32} mt64Bit {$ELSE} mt32Bit {$ENDIF} then
      begin
        if Low(ADLLNames) = High(ADLLNames) then
          sLib := 'Library'
        else
          sLib := ADLLNames[I];
        sErr := sErr + Format('%s 只支持[%s]位架构，必须用[%s]架构的文件。', [sLib,
          {$IFDEF FireDAC_32} 'x64' {$ELSE} 'x86' {$ENDIF},
          {$IFDEF FireDAC_32} 'x86' {$ELSE} 'x64' {$ENDIF}]) + sLineBreak;
      end
      else
      begin
      
      end;
    end;
    sErr := sErr + sMsg;

    {
    if (sErr <> '') and not FDInSet(sErr[Length(sErr)], [#13, #10, '.']) then
      sErr := sErr + sLineBreak;

    FDException(OwningObj, [S_FD_LPhys, FDriverID], er_FD_AccCantLoadLibrary,
      [sLibs, sErr])
    }
    raise Exception.Create(sErr);
  end;
end;

{-------------------------------------------------------------------------------}
procedure TMyLibrary.Unload;
begin
  if DllHandle <> 0 then begin
    FreeLibrary(FDllHandle);
    FDllHandle := 0;
  end;
end;

{-------------------------------------------------------------------------------}
procedure TMyLibrary.GetLibraryInfo;
begin
  FProduct := '';
  FVersionStr := '';
  FVersionName := '';
  FCopyright := '';
  FInfo := '';
  FVersion := 0;
  if DllHandle = 0 then Exit;
  SetLength(FDLLName, 255);
  SetLength(FDLLName, GetModuleFileName(DllHandle, PChar(FDLLName), 255));
  MyGetVersionInfo(FDLLName, FProduct, FVersionStr, FVersionName, FCopyright, FInfo);
  if FVersionStr <> '' then
    FVersion := MyVerStr2Int(FVersionStr)
  else
    FVersion := 0;
  if FVersion <= 100000000 then
  begin
    if FVersionName <> '' then
      FVersion := MyVerStr2Int(FVersionName)
    else
      FVersion := 0;
  end;
end;

{-------------------------------------------------------------------------------}
function TMyLibrary.GetProc(const AProcName: string; ARequired: Boolean = True): Pointer;
begin
  if DllHandle <> 0 then begin
    Result := GetProcAddress(DllHandle, PChar(AProcName));
    if (Result = nil) and ARequired then
      FFailedProcs.Add(AProcName);
  end
  else
    Result := nil;
end;


function MyVerStr2Int(const AVersion: string): TMyVersion;
var
  I, iItemNo, iDot, iFirstNonDig, iVal: Integer;
  s, sItem: string;
begin
  I := 1;
  while I <= Length(AVersion) do
    if (AVersion[I] >= '0') and (AVersion[I] <= '9') then
      Break
    else
      Inc(I);
  s := Copy(AVersion, I, Length(AVersion));
  Result := 0;
  for iItemNo := 1 to 5 do
  begin
    if s = '' then
      Result := Result * 100
    else
    begin
      iDot := Pos('.', s);
      if iDot = 0 then
        iDot := Length(s) + 1;
      sItem := Trim(Copy(s, 1, iDot - 1));
      iFirstNonDig := 1;
      while (iFirstNonDig <= Length(sItem)) and
            (sItem[iFirstNonDig] >= '0') and (sItem[iFirstNonDig] <= '9') do
        Inc(iFirstNonDig);
      iVal := StrToIntDef(Copy(sItem, 1, iFirstNonDig - 1), 0);
      if (iVal > 99) and (iItemNo > 1) then
        iVal := 99;
      Result := Result * 100 + Cardinal(iVal);
      s := Copy(s, iDot + 1, Length(s));
    end;
  end;
end;

{-------------------------------------------------------------------------------}
function MyVerInt2Str(AVersion: TMyVersion): string;
var
  iVal: Integer;
begin
  Result := '';
  while AVersion > 0 do
  begin
    if Result <> '' then
      Result := '.' + Result;
    iVal := AVersion mod 100;
    AVersion := AVersion div 100;
    if iVal = 99 then
      iVal := 0;
    Result := IntToStr(iVal) + Result;
  end;
end;

function MyGetVersionInfo(const AFileName: string; out AProduct,
  AVersion, AVersionName, ACopyright, AInfo: string): Boolean;
type
  TTranslation = array[0 .. 1] of Word;
const
  CTrans: String = '\VarFileInfo\Translation';
  CProdName: String = '\StringFileInfo\%s\FileDescription';
  CFileVers: String = '\StringFileInfo\%s\FileVersion';
  CCopyright: String = '\StringFileInfo\%s\LegalCopyright';
  CComments: String = '\StringFileInfo\%s\Comments';
  CSlash: String = '\';
var
  iHndl, iSz, iLen: DWORD;
  pBuff: Pointer;
  pTranslation: ^TTranslation;
  sTranslation, sBeta: String;
  pStr: PChar;
  pFileInfo: ^TVSFixedFileInfo;
begin
  Result := False;
  AProduct := '';
  AVersion := '';
  AVersionName := '';
  ACopyright := '';
  AInfo := '';
  iHndl := 0;
  iLen := 0;
  pFileInfo := nil;
  pTranslation := nil;
  pStr := nil;
  iSz := GetFileVersionInfoSize(PChar(AFileName), iHndl);
  if iSz <= 0 then
    Exit;
  GetMem(pBuff, iSz);
  try
    if not GetFileVersionInfo(PChar(AFileName), iHndl, iSz, pBuff) then
      Exit;
    if VerQueryValue(pBuff, PChar(CSlash), Pointer(pFileInfo), iLen) then
      if (pFileInfo.dwFileFlags and VS_FF_PRERELEASE) <> 0 then
        sBeta := ' Beta';
    if VerQueryValue(pBuff, PChar(CTrans), Pointer(pTranslation), iLen) then
      sTranslation := IntToHex(pTranslation^[0], 4) + IntToHex(pTranslation^[1], 4)
    else
      sTranslation := '040904B0';
    if VerQueryValue(pBuff, PChar(Format(CProdName, [sTranslation])), Pointer(pStr), iLen) then
      AProduct := pStr;
    if VerQueryValue(pBuff, PChar(Format(CFileVers, [sTranslation])), Pointer(pStr), iLen) then begin
      AVersion := pStr;
      AVersionName := Format('%d.%d.%d (Build %d)%s', [pFileInfo^.dwFileVersionMS shr 16,
        pFileInfo^.dwFileVersionMS and $0000FFFF, pFileInfo^.dwFileVersionLS shr 16,
        pFileInfo^.dwFileVersionLS and $0000FFFF, sBeta]);
    end;
    if VerQueryValue(pBuff, PChar(Format(CCopyright, [sTranslation])), Pointer(pStr), iLen) then
      ACopyright := pStr;
    if VerQueryValue(pBuff, PChar(Format(CComments, [sTranslation])), Pointer(pStr), iLen) then
      AInfo := pStr;
    Result := True;
  finally
    FreeMem(pBuff, iSz);
  end;
end;

var
  FLastFileName: string = '';
  FLastMachineType: TMyMachineType = mtUnknown;

function MyGetLibMachineType(const AFileName: string): TMyMachineType;
var
  oFS: TFileStream;
  iPeOffset: Integer;
  iPeHead: LongWord;
  iMachineType: Word;
begin
  Result := mtUnknown;

  // http://download.microsoft.com/download/9/c/5/9c5b2167-8017-4bae-9fde-d599bac8184a/pecoff_v8.doc
  // Offset to PE header is always at 0x3C.
  // PE header starts with "PE\0\0" = 0x50 0x45 0x00 0x00,
  // followed by 2-byte machine type field (see document above for enum).

  //FLock.Enter;
  try
    if FLastFileName = AFileName then begin
      Result := FLastMachineType;
      Exit;
    end;
    try
      oFS := TFileStream.Create(AFileName, fmOpenRead or fmShareDenyNone);
      try
        oFS.Seek($3C, soFromBeginning);
        oFS.Read(iPeOffset, SizeOf(iPeOffset));
        oFS.Seek(iPeOffset, soFromBeginning);
        oFS.Read(iPeHead, SizeOf(iPeHead));
        // "PE\0\0", little-endian then
        if iPeHead <> $00004550 then
          Exit;
        oFS.Read(iMachineType, SizeOf(iMachineType));
        case iMachineType of
          $8664, // AMD64
          $0200: // IA64
            Result := mt64Bit;
          $014C: // I386
            Result := mt32Bit;
        else
          Result := mtOther;
        end;
      finally
        oFS.Free;
      end;
    except
      // none
    end;
    FLastFileName := AFileName;
    FLastMachineType := Result;
  finally
    //FLock.Leave;
  end;
end;

(*
{$WARNINGS OFF}
function Encode(const AStr: String; Encoding: TMyEncoding): TMyAnsiString;
{$IFDEF MSWINDOWS}
var
  iLen, iRes: Integer;
  pUTF8: PAnsiChar;
{$ENDIF}
begin
  SetLength(Result, 0);
  if AStr = '' then Exit;
{$IFDEF MSWINDOWS}
  case Encoding of
    ecANSI:
      begin
        SetLength(Result, (Length(AStr) + 1) * SizeOf(AnsiChar));
        iRes := WideCharToMultiByte(C_CodePage, 0, PWideChar(AStr),
          Length(AStr), LPSTR(PByte(Result)), Length(Result) - SizeOf(AnsiChar), nil, nil);
        if iRes = 0 then begin
          iRes := WideCharToMultiByte(C_CodePage, 0, PWideChar(AStr),
            Length(AStr), nil, 0, nil, nil);
          SetLength(Result, (iRes + 1) * SizeOf(AnsiChar));
          WideCharToMultiByte(C_CodePage, 0, PWideChar(AStr),
            Length(AStr), LPSTR(PByte(Result)), Length(Result), nil, nil);
        end;
        //SetTrailerBytes(Result, False, SizeOf(AnsiChar));
      end;
    ecUTF8:
      begin
        pUTF8 := Alloc(Length(AStr) * C_MY_MaxUTF8Len + 1);
        iLen := WideCharToMultiByte(CP_UTF8, 0, PWideChar(AStr),
          Length(AStr), pUTF8, Length(AStr) * C_MY_MaxUTF8Len + 1, nil, nil);
        SetBytes(Result, pUTF8, iLen, 1);
        Release(pUTF8);
      end;
    ecUTF16:
      SetBytes(Result, PByte(@AStr[1]), Length(AStr) * SizeOf(WideChar), 2);
  end;
{$ENDIF}
{$IFDEF POSIX}
  Result := Enco(AStr, ADestEncoding);
{$ENDIF}
end;
{$WARNINGS ON}

{-------------------------------------------------------------------------------}
function TFDEncoder.Decode(const AStr: TFDByteString; ASrcEncoding: TFDEncoding = ecDefault): String;
{$IFDEF MSWINDOWS}
var
  iLen: Integer;
  pWC: PWideChar;
{$ENDIF}
begin
  Result := '';
  if ASrcEncoding = ecDefault then
    ASrcEncoding := FEncoding;
  if Length(AStr) = 0 then
    Exit;
{$IFDEF MSWINDOWS}
  case ASrcEncoding of
    ecANSI:
      begin
        SetLength(Result, Length(AStr));
        MultiByteToWideChar(C_CodePage, 0, LPCSTR(PByte(AStr)),
          Length(AStr), PWideChar(Result), Length(Result));
      end;
    ecUTF8:
      begin
        pWC := Alloc((Length(AStr) + 1) * SizeOf(WideChar));
        iLen := MultiByteToWideChar(CP_UTF8, 0, LPCSTR(PByte(AStr)),
          Length(AStr), pWC, Length(AStr));
        SetString(Result, pWC, iLen);
        Release(pWC);
      end;
    ecUTF16:
      SetString(Result, PChar(PByte(AStr)), Length(AStr) div SizeOf(Char));
  end;
{$ENDIF}
{$IFDEF POSIX}
  Result := Deco(AStr, ASrcEncoding);
{$ENDIF}
end;
*)

initialization
  C_CodePage := GetACP;

end.
