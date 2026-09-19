{...................................................................

Author : Per Linds?Larsen {lindsoe@post.tele.dk)

Christian Lovis for lib dynamic linking {christian.lovis@dim.hcuge.ch]

UPDATES : http:// www.euromind.com/iedelphi

Copyright :

source : IE CACHE Component v 1.02

First release : January 26, 2000

Last release : January 14, 2001

Unit Type : WinInet MS lib wrapper

Compiler :

Comment :

Dependances :

...................................................................}

unit IECache;

interface

uses

  wininet,
  Windows,
  Messages,
  SysUtils,
  Classes;

const

  CACHEGROUP_ATTRIBUTE_GET_ALL = $FFFFFFFF;

  CACHEGROUP_ATTRIBUTE_BASIC = $00000001;

  CACHEGROUP_ATTRIBUTE_FLAG = $00000002;

  CACHEGROUP_ATTRIBUTE_TYPE = $00000004;

  CACHEGROUP_ATTRIBUTE_QUOTA = $00000008;

  CACHEGROUP_ATTRIBUTE_GROUPNAME = $00000010;

  CACHEGROUP_ATTRIBUTE_STORAGE = $00000020;

  CACHEGROUP_FLAG_NONPURGEABLE = $00000001;

  CACHEGROUP_FLAG_GIDONLY = $00000004;

  CACHEGROUP_FLAG_FLUSHURL_ONDELETE = $00000002;

  CACHEGROUP_SEARCH_ALL = $00000000;

  CACHEGROUP_SEARCH_BYURL = $00000001;

  CACHEGROUP_TYPE_INVALID = $00000001;

  CACHEGROUP_READWRITE_MASK = CACHEGROUP_ATTRIBUTE_TYPE or

  CACHEGROUP_ATTRIBUTE_QUOTA or

  CACHEGROUP_ATTRIBUTE_GROUPNAME or

  CACHEGROUP_ATTRIBUTE_STORAGE;

  GROUPNAME_MAX_LENGTH = 120;

  GROUP_OWNER_STORAGE_SIZE = 4;

type

  PInternetCacheTimeStamps = ^TInternetCacheTimeStamps;

  TInternetCacheTimeStamps = record

    ftExpires:TFileTime;

    ftLastModified:TFileTime;

  end;

  PInternetCacheGroupInfo = ^TInternetCacheGroupInfo;

  TInternetCacheGroupInfo = record

    dwGroupSize:DWORD;

    dwGroupFlags:DWORD;

    dwGroupType:DWORD;

    dwDiskUsage:DWORD;

    dwDiskQuota:DWORD;

    dwOwnerStorage:array[0..GROUP_OWNER_STORAGE_SIZE - 1] of DWORD;

    szGroupName:array[0..GROUPNAME_MAX_LENGTH - 1] of AnsiChar;

  end;

  TEntryInfo = record

    SourceUrlName:string;

    LocalFileName:string;

    EntryType:DWORD;

    UseCount:DWORD;

    HitRate:DWORD;

    FSize:DWORD;

    LastModifiedTime:TDateTime;

    ExpireTime:TDateTime;

    LastAccessTime:TDateTime;

    LastSyncTime:TDateTime;

    HeaderInfo:string;

    FileExtension:string;

    ExemptDelta:DWORD;

  end;

  TGroupInfo = record

    DiskUsage:DWORD;

    DiskQuota:DWORD;

    OwnerStorage:array[0..GROUP_OWNER_STORAGE_SIZE - 1] of DWORD;

    GroupName:string;

  end;

  TContent = record

    Buffer:Pointer;

    BufferLength:Integer;

  end;

  TFilterOption = (NORMAL_ENTRY,

    STABLE_ENTRY,

    STICKY_ENTRY,

    COOKIE_ENTRY,

    URLHISTORY_ENTRY,

    TRACK_OFFLINE_ENTRY,

    TRACK_ONLINE_ENTRY,

    SPARSE_ENTRY,

    OCX_ENTRY);

  TFilterOptions = set of TFilterOption;

  TOnEntryEvent = procedure(Sender:TObject; var Cancel:Boolean) of object;

  TOnGroupEvent = procedure(Sender:TObject; GroupID:GROUPID; var Cancel:Boolean) of object;

  TSearchPattern = (spAll, spCookies, spHistory, spUrl);

  TIECache = class(TComponent)

  private

    FSearchPattern:TSearchPattern;

    FOnEntry:TOnEntryEvent;

    FOnGroup:TOnGroupEvent;

    GrpHandle:THandle;

    H:THandle;

    FCancel:Boolean;

    FFilterOptions:TFilterOptions;

    FFilterOptionValue:Cardinal;

    procedure SetFilterOptions(const Value:TFilterOptions);

    procedure UpdateFilterOptionValue;

    procedure GetEntryValues(Info:PInternetCacheEntryInfo);

    procedure ClearEntryValues;

  protected { Protected declarations }

  public

    GroupInfo:TGroupInfo;

    EntryInfo:TEntryInfo;

    Content:TContent;

    constructor Create(AOwner:TComponent); override;

    function CreateGroup:INT64;

    function DeleteGroup(GroupID:INT64):DWORD;

    function GetGroupInfo(GroupID:INT64):DWORD;

    function SetGroupInfo(GroupID:INT64):DWORD;

    function AddUrlToGroup(GroupID:INT64; Url:string):DWORD;

    function RemoveUrlFromGroup(GroupID:INT64; Url:string):DWORD;

    function FindFirstGroup(var GroupID:Int64):DWORD;

    function FindNextGroup(var GroupID:Int64):BOOL;

    function RetrieveGroups:DWORD;

    function CreateEntry(Url, FileExtension:string; ExpectedFileSize:DWORD; var FName:string):DWORD;

    function DeleteEntry(Url:string):DWORD;

    function FindFirstEntry(GroupID:INT64):DWORD;

    function FindNextEntry:DWORD;

    function CloseFindEntry:BOOL;

    procedure RetrieveEntries(GroupID:INT64);

    function GetEntryInfo(Url:string):DWORD;

    function GetEntryContent(Url:string):DWORD;

    function SetEntryInfo(Url:string):DWORD;

    function getLibraryFound:boolean;

    // function CopyFileToCache(UrlName, FileName: Pchar): string;

    function CopyFileToCache(Url, FileName:string; CacheType:DWORD; Expire:TDateTime):DWORD;

    procedure ClearAllEntries;

    { Public declarations }

  published

    property FilterOptions:TFilterOptions read FFilterOptions write SetFilterOptions;

    property SearchPattern:TSearchpattern read FSearchpattern write FSearchPattern;

    property LibraryFound:boolean read getLibraryFound;

    property OnEntry:TOnEntryEvent read FOnEntry write FOnEntry;

    property OnGroup:TOnGroupEvent read FOnGroup write FOnGroup;

    { Published declarations }

  end;

procedure Register;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

implementation

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

type

  tFindFirstUrlCacheGroup =

  function(dwFlags, dwFilter:DWORD;

    lpSearchCondition:Pointer; dwSearchCondition:DWORD;

    var Group:Int64; lpReserved:Pointer):THandle; stdcall;

  tFindNextUrlCacheGroup =

  function(hFind:THandle; var GroupID:Int64; lpReserved:Pointer):BOOL; stdcall;

  tSetUrlCacheGroupAttribute =

  function(gid:Int64; dwFlags, dwAttributes:DWORD; var lpGroupInfo:TInternetCacheGroupInfo;

    lpReserved:Pointer):BOOL; stdcall;

  tGetUrlCacheGroupAttribute =

  function(gid:Int64; dwFlags, dwAttributes:DWORD;

    var GroupInfo:TInternetCacheGroupInfo; var dwGroupInfo:DWORD; lpReserved:Pointer):BOOL; stdcall;

var

  FindFirstUrlCacheGroup:tFindFirstUrlCacheGroup;

  FindNextUrlCacheGroup:tFindNextUrlCacheGroup;

  GetUrlCacheGroupAttribute:tGetUrlCacheGroupAttribute;

  SetUrlCacheGroupAttribute:tSetUrlCacheGroupAttribute;

const

  winetdll = 'wininet.dll';

var

  winInetLibFound:boolean;

  // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

function initializeWinInet:boolean;
var
  fPointer:tFarProc;
  hInst:tHandle;
begin
  if winInetLibFound then
    result := true
  else begin
    result := false;
    hInst := loadLibrary(winetdll);
    if hInst > 0 then try
      fPointer := getProcAddress(hInst, 'FindFirstUrlCacheGroup');
      if fPointer <> nil then begin
        FindFirstUrlCacheGroup := tFindFirstUrlCacheGroup(fPointer);
        fPointer := getProcAddress(hInst, 'FindNextUrlCacheGroup');
        if fPointer <> nil then begin
          FindNextUrlCacheGroup := tFindNextUrlCacheGroup(fPointer);
          fPointer := getProcAddress(hInst, 'GetUrlCacheGroupAttributeA');
          if fPointer <> nil then begin
            GetUrlCacheGroupAttribute := tGetUrlCacheGroupAttribute(fPointer);
            fPointer := getProcAddress(hInst, 'SetUrlCacheGroupAttributeA');
            if fPointer <> nil then begin
              SetUrlCacheGroupAttribute := tSetUrlCacheGroupAttribute(fPointer);
              fPointer := getProcAddress(hInst, 'FindFirstUrlCacheEntryExA');
              if fPointer <> nil then
                result := true;
            end;
          end;
        end;
      end;
    except

    end;
  end;
end; // function initializeWinInet : boolean;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

function FileTimeToDateTime(Ft:TFileTime):TDateTime;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

var

  St:TSystemTime;

  lft:TFileTime;

begin

  Result := 0;

  if FileTimeToLocalFiletime(Ft, lft) then

    if FileTimeToSyStemTime(lft, st) then

      Result := SystemTimeTODateTime(st);

end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

function DateTimeToFileTime(Dt:TDateTime):TFileTime;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

var

  St:TSystemTime;

  lft:TFileTime;

begin

  DateTimeToSystemTime(Dt, ST);

  if SystemTimeToFileTime(st, lft) then LocalFileTimeToFileTime(lft, Result);

end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

// TIECache

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

constructor TIECache.Create(AOwner:TComponent);

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

begin

  inherited;

  Content.Buffer := nil;

  ClearEntryValues;

  // Identical to URLCACHE_FIND_DEFAULT_FILTER

  FFilterOptions := [NORMAL_ENTRY, COOKIE_ENTRY, URLHISTORY_ENTRY,

  TRACK_OFFLINE_ENTRY, TRACK_ONLINE_ENTRY, STICKY_ENTRY];

end; // constructor TIECache.Create(AOwner: TComponent);

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

function TIECache.getLibraryFound:boolean;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

begin

  result := initializeWinInet;

end; // function TIECache.getLibraryFound : boolean;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

function TIECache.RemoveUrlFromGroup(GroupID:INT64; Url:string):DWORD;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

begin

  Result := S_OK;

  if not initializeWinInet then
  begin

    Result := ERROR_FILE_NOT_FOUND;

    Exit;

  end;

  if not SetUrlCacheEntryGroup(Pchar(Url), INTERNET_CACHE_GROUP_REMOVE, GroupID, nil, 0, nil)

  then Result := GetLastError;

end;

function TIECache.AddUrlToGroup(GroupID:INT64; Url:string):DWORD;

begin

  Result := S_OK;

  if not initializeWinInet then
  begin

    Result := ERROR_FILE_NOT_FOUND;

    Exit;

  end;

  if not SetUrlCacheEntryGroup(Pchar(Url), INTERNET_CACHE_GROUP_ADD, GroupID, nil, 0, nil)

  then Result := GetLastError;

end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

function TIECache.CopyFileToCache(Url, FileName:string; CacheType:DWORD; Expire:TDateTime):DWORD;
var
  FName:string;
  Ext:string;
  F:file of Byte;
  Size:DWORD;
begin
  if not initializeWinInet then begin
    Result := ERROR_FILE_NOT_FOUND;
    Exit;
  end;
  if not FileExists(FileName) then begin
    Result := ERROR_FILE_NOT_FOUND;
    Exit;
  end;

  AssignFile(F, FileName);
  Reset(F);
  Size := FileSize(F);
  CloseFile(F);
  Ext := ExtractFileExt(FileName);
  Ext := Copy(Ext, 2, Length(ext));
  Result := CreateEntry(Url, Ext, Size, FName);
  if Result <> S_OK then Exit;
  if not windows.copyfile(PChar(FileName), Pchar(FName), FALSE) then begin
    Result := GetLastError;
    Exit;
  end;
  if not CommitUrlCacheEntry(Pchar(Url), Pchar(Fname), DateTimeToFileTime(Expire), DateTimeToFileTime(now), CacheType, nil, 0, Pchar(Ext), 0)
    then Result := GetLastError;
end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

function TIECache.CreateEntry(Url, FileExtension:string; ExpectedFileSize:DWORD; var FName:string):DWORD;
var
  PC:array[0..MAX_PATH] of Char;
begin
  PC := '';
  Result := S_OK;
  if not initializeWinInet then begin
    Result := ERROR_FILE_NOT_FOUND;
    Exit;
  end;
  if not CreateUrlCacheEntry(Pchar(url), ExpectedFileSize, Pchar(FileExtension), PC, 0) then
    result := GetLastError
  else
    FName := StrPas(PC);
end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

function TIECache.GetGroupInfo(GroupID:INT64):DWORD;
var

  info:TInternetCacheGroupInfo;

  dw:DWORD;

begin

  Result := S_OK;

  if not initializeWinInet then
  begin

    Result := ERROR_FILE_NOT_FOUND;

    Exit;

  end;

  dw := Sizeof(TInternetCacheGroupInfo);

  if not GetUrlCacheGroupAttribute(GroupID, 0, CACHEGROUP_ATTRIBUTE_GET_ALL, info, dw, nil)

  then
    Result := GetLastError
  else

    with GroupInfo do begin

      DiskUsage := info.dwDiskUsage;

      DiskQuota := info.dwDiskQuota;

      move(info.dwOwnerStorage, OwnerStorage, Sizeof(OwnerStorage));

      GroupName := info.szGroupName;

    end;

end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

function TIECache.SetGroupInfo(GroupID:INT64):DWORD;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

var

  info:TInternetCacheGroupInfo;

begin

  Result := S_OK;

  if not initializeWinInet then
  begin

    Result := ERROR_FILE_NOT_FOUND;

    Exit;

  end;

  info.dwGroupSize := SizeOf(Info);

  info.dwGroupFlags := CACHEGROUP_FLAG_NONPURGEABLE;

  info.dwGroupType := CACHEGROUP_TYPE_INVALID;

  info.dwDiskQuota := GroupInfo.DiskQuota;

  move(GroupInfo.OwnerStorage, info.dwOwnerStorage, Sizeof(info.dwOwnerStorage));

  move(GroupInfo.Groupname[1], info.szGroupName[0], length(GroupInfo.Groupname));

  if not SetUrlCacheGroupAttribute(GroupID, 0, CACHEGROUP_READWRITE_MASK, info, nil)

  then Result := GetLastError;

end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

function TIECache.CreateGroup:INT64;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

begin

  if not initializeWinInet then
  begin

    Result := ERROR_FILE_NOT_FOUND;

    Exit;

  end;

  Result := CreateUrlCacheGroup(0, nil);

end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

function TIECache.DeleteGroup(GroupID:INT64):DWORD;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

begin

  Result := S_OK;

  if not initializeWinInet then
  begin

    Result := ERROR_FILE_NOT_FOUND;

    Exit;

  end;

  if not DeleteUrlCacheGroup(GroupID, CACHEGROUP_FLAG_FLUSHURL_ONDELETE, nil)

  then Result := GetLastError;

end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

function TIECache.SetEntryInfo(Url:string):DWORD;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

var

  info:TInternetCacheEntryInfo;

  fc:DWORD;

begin

  Result := S_OK;

  if not initializeWinInet then
  begin

    Result := ERROR_FILE_NOT_FOUND;

    Exit;

  end;

  fc := CACHE_ENTRY_ATTRIBUTE_FC +

  CACHE_ENTRY_HITRATE_FC +

  CACHE_ENTRY_MODTIME_FC +

  CACHE_ENTRY_EXPTIME_FC +

  CACHE_ENTRY_ACCTIME_FC +

  CACHE_ENTRY_SYNCTIME_FC +

  CACHE_ENTRY_EXEMPT_DELTA_FC;

  with info do begin

    CacheEntryType := EntryInfo.EntryType;

    dwHitRate := EntryInfo.HitRate;

    LastModifiedTime := DateTimeToFileTime(EntryInfo.LastModifiedTime);

    ExpireTime := DateTimeToFileTime(EntryInfo.ExpireTime);

    LastAccessTime := DateTimeToFileTime(EntryInfo.LastAccessTime);

    LastSyncTime := DateTimeToFileTime(EntryInfo.LastSyncTime);

    dwReserved := EntryInfo.ExemptDelta;

  end;

  if not SetUrlCacheEntryInfo(Pchar(url), info, fc) then

    Result := GetLastError;

end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

function TIECache.GetEntryInfo(Url:string):DWORD;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

var

  D:DWORD;

  T:PInternetCacheEntryInfo;

begin

  Result := S_OK;

  if not initializeWinInet then
  begin

    Result := ERROR_FILE_NOT_FOUND;

    Exit;

  end;

  GetUrlCacheEntryInfoEx(Pchar(Url), nil, @D, nil, nil, nil, 0);

  GetMem(T, D);

  try

    if GetUrlCacheEntryInfoEx(Pchar(Url), T, @D, nil, nil, nil, 0)

    then
      GetEntryValues(t)
    else
      Result := GetLastError;

  finally

    Freemem(T, D);

  end;

end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

function TIECache.GetEntryContent(Url:string):DWORD;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

var

  Hr:THandle;

  D:Cardinal;

  T:PInternetCacheEntryInfo;

begin

  result := S_OK;

  if not initializeWinInet then
  begin

    Result := ERROR_FILE_NOT_FOUND;

    Exit;

  end;

  D := 0;

  T := nil;

  RetrieveUrlCacheEntryStream(PChar(Url), T^, D, TRUE, 0);

  Getmem(T, D);

  try

    hr := THandle(RetrieveUrlCacheEntryStream(PChar(Url), T^, D, TRUE, 0));

    if Hr <> 0 then begin

      Content.BufferLength := T^.dwSizeLow + 1;

      GetEntryValues(T);

      Getmem(Content.Buffer, Content.BufferLength);

      Fillchar(Content.Buffer^, Content.BufferLength, #0);

      if not ReadUrlCacheEntryStream(Hr, 0, Content.Buffer, T^.DwSizeLow, 0)

      then Result := GetLastError;

    end;

  finally

    Freemem(T, D);

  end;

  UnLockUrlCacheEntryStream(Hr, 0);

end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

function TIECache.FindNextGroup(var GroupID:Int64):BOOL;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

begin

  if not initializeWinInet then
  begin

    Result := false;

    Exit;

  end;

  Result := FindNextUrlCacheGroup(GrpHandle, GroupID, nil);

  GetGroupInfo(GroupID);

end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

function TIECache.FindFirstGroup(var GroupID:Int64):DWORD;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

begin

  if not initializeWinInet then
  begin

    Result := ERROR_FILE_NOT_FOUND;

    Exit;

  end;

  GrpHandle := FindFirstUrlCacheGroup(0, 0, nil, 0, GroupID, nil);

  if GrpHandle <> 0 then
    result := S_OK
  else

    Result := GetLastError;

  if result = S_OK then GetGroupInfo(GroupID);

end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

function TIECache.RetrieveGroups:DWORD;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

var

  GroupID:INT64;

  Res:DWORD;

  NewGroup, Cancel:Boolean;

begin

  Result := S_OK;

  if not initializeWinInet then
  begin

    Result := ERROR_FILE_NOT_FOUND;

    Exit;

  end;

  Cancel := False;

  NewGroup := True;

  Res := FindFirstGroup(GroupID);

  if Res = S_OK then begin

    GetGroupInfo(GroupID);

    if Assigned(FOngroup) then FOnGroup(self, GroupID, FCancel);

    while not Cancel and NewGroup do begin

      NewGroup := FindNextGroup(GroupID);

      GetGroupInfo(GroupID);

      if Assigned(FOngroup) and NewGroup then FOnGroup(self, GroupID, Cancel);

    end;

  end
  else

    result := GetLastError;

end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

function TIECache.DeleteEntry(Url:string):DWORD;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

begin

  Result := S_OK;

  if not initializeWinInet then exit;

  if not DeleteUrlCacheEntry(PChar(Url)) then

    Result := GetLastError

  else
    ClearEntryValues;

end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

procedure TIECache.ClearAllEntries;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

var

  hr:DWord;

begin

  if not initializeWinInet then Exit;

  if FindFirstEntry(0) = S_OK then begin

    repeat

      DeleteEntry(EntryInfo.SourceUrlName);

      hr := FindNextEntry;

    until hr = ERROR_NO_MORE_ITEMS;

  end;

  FindCloseUrlCache(H);

end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

procedure TIECache.ClearEntryValues;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

begin

  if not initializeWinInet then Exit;

  Content.Buffer := nil;

  Content.BufferLength := 0;

  with EntryInfo do begin

    sourceUrlName := '';

    localfilename := '';

    entryType := 0;

    UseCount := 0;

    Hitrate := 0;

    LastModifiedTime := 0;

    ExpireTime := 0;

    LastAccessTime := 0;

    LastSyncTime := 0;

    FileExtension := '';

    FSize := 0;

    HeaderInfo := '';

    ExemptDelta := 0;

  end;

end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

procedure TIECache.GetEntryValues(Info:PInternetCacheEntryInfo);

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

begin

  if not initializeWinInet then Exit;

  with entryInfo do
  begin

    sourceUrlName := info^.lpszSourceUrlname;

    localfilename := info^.lpszLocalFilename;

    entryType := info^.CacheEntryType;

    UseCount := info^.dwUseCount;

    Hitrate := info^.dwHitRate;

    LastModifiedTime := FileTimeToDateTime(info^.LastModifiedTime);

    ExpireTime := FileTimeToDateTime(info^.ExpireTime);

    LastAccessTime := FileTimeToDateTime(info^.LastAccessTime);

    LastSyncTime := FileTimeToDateTime(info^.LastSyncTime);

    FileExtension := info^.lpszFileExtension;

    FSize := (info^.dwSizeHigh shl 32) + info^.dwSizeLow;

    HeaderInfo := StrPas(PChar(info^.lpHeaderInfo));

    ExemptDelta := info^.dwReserved;

  end;

end;
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

function TIECache.FindFirstEntry(GroupID:INT64):DWORD;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

const

  Pattern:array[TSearchPattern] of PChar = (nil, 'Cookie:', 'Visited:', '');

var

  T:PInternetCacheEntryInfo;

  D:DWORD;

begin

  Result := S_OK;

  if not initializeWinInet then
  begin

    Result := ERROR_FILE_NOT_FOUND;

    Exit;

  end;

  H := 0;

  D := 0;

  FindFirstUrlCacheEntryEx(Pattern[SearchPattern], 0, FFilterOptionValue, GroupID, nil, @D, nil, nil, nil);

  GetMem(T, D);

  try

    H := FindFirstUrlCacheEntryEx(Pattern[SearchPattern], 0, FFilterOptionValue, GroupID, T, @D, nil, nil, nil);

    if (H = 0) then
      Result := GetLastError
    else
      GetEntryValues(T);

  finally

    FreeMem(T, D)

  end;

end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

function TIECache.FindNextEntry:DWORD;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

var

  T:PInternetCacheEntryInfo;

  D:DWORD;

begin

  Result := S_OK;

  if not initializeWinInet then
  begin

    Result := ERROR_FILE_NOT_FOUND;

    Exit;

  end;

  D := 0;

  FindnextUrlCacheEntryEx(H, nil, @D, nil, nil, nil);

  GetMem(T, D);

  try

    if not FindNextUrlCacheEntryEx(H, T, @D, nil, nil, nil)

    then
      Result := GetLastError
    else
      GetEntryValues(T);

  finally

    FreeMem(T, D)

  end;
end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

procedure TIECache.RetrieveEntries(GroupID:INT64);
var
  HR:DWORD;
begin
  if not initializeWinInet then Exit;

  FCancel := False;

  hr := FindFirstEntry(GroupID);

  if (hr = S_OK) then begin

    if Assigned(FOnEntry) then
      with EntryInfo do
        FOnEntry(self, FCancel);

    while (hr = S_OK) and not FCancel do
    begin

      hr := FindNextEntry;

      if (hr = S_OK) and Assigned(FOnEntry) then
        with EntryInfo do
          FOnEntry(self, FCancel);

    end;

  end;

  FindCloseUrlCache(H);
end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

function TIECache.CloseFindEntry:BOOL;
begin
  if not initializeWinInet then begin
    Result := false;
    Exit;
  end;
  Result := FindCloseUrlCache(H);
end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

procedure TIECache.SetFilterOptions(const Value:TFilterOptions);
begin
  FFilterOptions := Value;
  UpdateFilterOptionValue;
end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

procedure TIECache.UpdateFilterOptionValue;
const
  acardFilterOptionValues:array[TFilterOption] of Cardinal = (
    $00000001, $00000002, $00000004, $00100000, $00200000,
    $00000010, $00000020, $00010000, $00020000);
var
  I:TFilterOption;
begin
  FFilterOptionValue := 0;
  if (FFilterOptions <> []) then
    for i := Low(TFilterOption) to High(TFilterOption) do
      if (i in FFilterOptions) then
        Inc(FFilterOptionValue, acardFilterOptionValues[i]);
end;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

procedure Register;

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
begin
  RegisterComponents('Internet', [TIECache]);
end;

initialization

  wininetLibFound := initializeWinInet;

end.
