unit VersionHelper;

interface

uses
  Windows, SysUtils;

  function IsWindowsXPOrGreater(): Boolean;
  function IsWindowsXPSP1OrGreater(): Boolean;
  function IsWindowsXPSP2OrGreater(): Boolean;
  function IsWindowsXPSP3OrGreater(): Boolean;
  function IsWindowsVistaOrGreater(): Boolean;
  function IsWindowsVistaSP1OrGreater(): Boolean;
  function IsWindowsVistaSP2OrGreater(): Boolean;
  function IsWindows7OrGreater(): Boolean;
  function IsWindows7SP1OrGreater(): Boolean;
  function IsWindows8OrGreater(): Boolean;
  function IsWindows8Point1OrGreater(): Boolean;
  function IsWindows10OrGreater(): Boolean;

  function IsWindowsServer(): Boolean;
  function IsWin64(): Boolean;

  function GetOSString(): string;

implementation

const
  _WIN32_WINNT_NT4        = $0400;
  _WIN32_WINNT_WIN2K      = $0500;
  _WIN32_WINNT_WINXP      = $0501;
  _WIN32_WINNT_WS03       = $0502;
  _WIN32_WINNT_WIN6       = $0600;
  _WIN32_WINNT_VISTA      = $0600;
  _WIN32_WINNT_WS08       = $0600;
  _WIN32_WINNT_LONGHORN   = $0600;
  _WIN32_WINNT_WIN7       = $0601;
  _WIN32_WINNT_WIN8       = $0602;
  _WIN32_WINNT_WINBLUE    = $0603;
  _WIN32_WINNT_WIN10      = $0A00;


const
//from winnt.h
  {$EXTERNALSYM VER_EQUAL}
  VER_EQUAL                       = 1;
  {$EXTERNALSYM VER_GREATER}
  VER_GREATER                     = 2;
  {$EXTERNALSYM VER_GREATER_EQUAL}
  VER_GREATER_EQUAL               = 3;
  {$EXTERNALSYM VER_LESS}
  VER_LESS                        = 4;
  {$EXTERNALSYM VER_LESS_EQUAL}
  VER_LESS_EQUAL                  = 5;
  {$EXTERNALSYM VER_AND}
  VER_AND                         = 6;
  {$EXTERNALSYM VER_OR}
  VER_OR                          = 7;

  {$EXTERNALSYM VER_CONDITION_MASK}
  VER_CONDITION_MASK              = 7;

  {$EXTERNALSYM VER_NUM_BITS_PER_CONDITION_MASK}
  VER_NUM_BITS_PER_CONDITION_MASK = 3;


  {$EXTERNALSYM VER_MINORVERSION}
  VER_MINORVERSION                = $0000001;
  {$EXTERNALSYM VER_MAJORVERSION}
  VER_MAJORVERSION                = $0000002;
  {$EXTERNALSYM VER_BUILDNUMBER}
  VER_BUILDNUMBER                 = $0000004;
  {$EXTERNALSYM VER_PLATFORMID}
  VER_PLATFORMID                  = $0000008;
  {$EXTERNALSYM VER_SERVICEPACKMINOR}
  VER_SERVICEPACKMINOR            = $0000010;
  {$EXTERNALSYM VER_SERVICEPACKMAJOR}
  VER_SERVICEPACKMAJOR            = $0000020;
  {$EXTERNALSYM VER_SUITENAME}
  VER_SUITENAME                   = $0000040;
  {$EXTERNALSYM VER_PRODUCT_TYPE}
  VER_PRODUCT_TYPE                = $0000080;

//
// RtlVerifyVersionInfo() os product type values
//

  {$EXTERNALSYM  VER_NT_WORKSTATION}
  VER_NT_WORKSTATION              = $0000001;
  {$EXTERNALSYM  VER_NT_DOMAIN_CONTROLLER}
  VER_NT_DOMAIN_CONTROLLER        = $0000002;
  {$EXTERNALSYM VER_NT_SERVER}
  VER_NT_SERVER                   = $0000003;

  {$EXTERNALSYM SM_STARTER}
  SM_STARTER                      = 88;
  {$EXTERNALSYM SM_SERVERR2}
  SM_SERVERR2                     = 89;

  PROCESSOR_ARCHITECTURE_AMD64    = 9;
  PROCESSOR_ARCHITECTURE_IA64     = 6;

type
 _OSVERSIONINFOEXA = record
    dwOSVersionInfoSize: DWORD;
    dwMajorVersion: DWORD;
    dwMinorVersion: DWORD;
    dwBuildNumber: DWORD;
    dwPlatformId: DWORD;
    szCSDVersion: array[0..127] of AnsiChar;
    wServicePackMajor: Word;
    wServicePackMinor: Word;
    wSuiteMask: Word;
    wProductType: Byte;
    wReserved: Byte;
   end;

  _OSVERSIONINFOEXW = record
    dwOSVersionInfoSize: DWORD;
    dwMajorVersion: DWORD;
    dwMinorVersion: DWORD;
    dwBuildNumber: DWORD;
    dwPlatformId: DWORD;
    szCSDVersion: array[0..127] of widechar;
    wServicePackMajor: Word;
    wServicePackMinor: Word;
    wSuiteMask: Word;
    wProductType: Byte;
    wReserved: Byte;
  end;

  TOSVersionInfoExW = _OSVERSIONINFOEXW;
  TOSVersionInfoEx = TOSVersionInfoEXW;

function IsWindowsVersionOrGreater(wMajorVersion, wMinorVersion, wServicePackMajor: Word): Boolean;
var
  VersionInfo: TOSVersionInfoExW;
  DllHandle: THandle;
  RtlGetVersion: function(var Version: TOSVersionInfoExW): Integer; stdcall;
begin
  Result := False;

  FillChar(VersionInfo, SizeOf(VersionInfo), 0);
  VersionInfo.dwOSVersionInfoSize := SizeOf(VersionInfo);

  DllHandle := LoadLibrary('ntdll.dll');
  if DllHandle = 0 then Exit;

  try
    @RtlGetVersion := GetProcAddress(DllHandle, 'RtlGetVersion');

    if @RtlGetVersion <> nil then
    begin
      if RtlGetVersion(VersionInfo) = 0 then
      begin
        if (VersionInfo.dwMajorVersion > wMajorVersion) then
          Result := True
        else if (VersionInfo.dwMajorVersion = wMajorVersion) then
        begin
          if (VersionInfo.dwMinorVersion > wMinorVersion) then
            Result := True
          else if (VersionInfo.dwMinorVersion = wMinorVersion) then
            Result := (VersionInfo.wServicePackMajor >= wServicePackMajor);
        end;
      end;
    end;
  finally
    FreeLibrary(DllHandle);
  end;
end;

function IsWindowsXPOrGreater(): Boolean;
begin
  Result := IsWindowsVersionOrGreater(HIBYTE(_WIN32_WINNT_WINXP), LOBYTE(_WIN32_WINNT_WINXP), 0);
end;
 
function IsWindowsXPSP1OrGreater(): Boolean;
begin
  Result := IsWindowsVersionOrGreater(HIBYTE(_WIN32_WINNT_WINXP), LOBYTE(_WIN32_WINNT_WINXP), 1);
end;

function IsWindowsXPSP2OrGreater(): Boolean;
begin
  Result := IsWindowsVersionOrGreater(HIBYTE(_WIN32_WINNT_WINXP), LOBYTE(_WIN32_WINNT_WINXP), 2);
end;
 
function IsWindowsXPSP3OrGreater(): Boolean;
begin
  Result := IsWindowsVersionOrGreater(HIBYTE(_WIN32_WINNT_WINXP), LOBYTE(_WIN32_WINNT_WINXP), 3);
end;
 
function IsWindowsVistaOrGreater(): Boolean;
begin
  Result := IsWindowsVersionOrGreater(HIBYTE(_WIN32_WINNT_VISTA), LOBYTE(_WIN32_WINNT_VISTA), 0);
end;
 
function IsWindowsVistaSP1OrGreater(): Boolean;
begin
  Result := IsWindowsVersionOrGreater(HIBYTE(_WIN32_WINNT_VISTA), LOBYTE(_WIN32_WINNT_VISTA), 1);
end;
 
function IsWindowsVistaSP2OrGreater(): Boolean;
begin
  Result := IsWindowsVersionOrGreater(HIBYTE(_WIN32_WINNT_VISTA), LOBYTE(_WIN32_WINNT_VISTA), 2);
end;
 
function IsWindows7OrGreater(): Boolean;
begin
  Result := IsWindowsVersionOrGreater(HIBYTE(_WIN32_WINNT_WIN7), LOBYTE(_WIN32_WINNT_WIN7), 0);
end;
 
function IsWindows7SP1OrGreater(): Boolean;
begin
  Result := IsWindowsVersionOrGreater(HIBYTE(_WIN32_WINNT_WIN7), LOBYTE(_WIN32_WINNT_WIN7), 1);
end;

function IsWindows8OrGreater(): Boolean;
begin
  Result := IsWindowsVersionOrGreater(HIBYTE(_WIN32_WINNT_WIN8), LOBYTE(_WIN32_WINNT_WIN8), 0);
end;

function IsWindows8Point1OrGreater(): Boolean;
begin
  Result := IsWindowsVersionOrGreater(HIBYTE(_WIN32_WINNT_WINBLUE), LOBYTE(_WIN32_WINNT_WINBLUE), 0);
end;

function IsWindows10OrGreater(): Boolean;
begin
  Result := IsWindowsVersionOrGreater(HIBYTE(_WIN32_WINNT_WIN10), LOBYTE(_WIN32_WINNT_WIN10), 0);
end;

function IsWindowsServer(): Boolean;
var
  Kernel32Handle: THandle;
  Version: TOSVersionInfoExW;
  dwlConditionMask: Int64;

  VerifyVersionInfoW: function(var Version: TOSVersionInfoExW; const dwTypeMask: DWORD; const dwlConditionMask: UInt64): BOOL stdcall;
  VerSetConditionMask: function(const ConditionMask: UInt64; const TypeMask: DWORD; const Condition: Byte): UInt64 stdcall;
begin
  Result := False;
  
  Kernel32Handle := GetModuleHandle('KERNEL32.DLL');
  if Kernel32Handle = 0 then
    Kernel32Handle := LoadLibrary('KERNEL32.DLL');

  if Kernel32Handle <> 0 then
  begin
    @VerifyVersionInfoW := GetProcAddress(Kernel32Handle, 'VerifyVersionInfoW');
    @VerSetConditionMask := GetProcAddress(Kernel32Handle, 'VerSetConditionMask');

    if (@VerifyVersionInfoW <> nil) and (@VerSetConditionMask <> nil) then
    begin
      FillChar(Version, SizeOf(Version), 0);
      Version.dwOSVersionInfoSize := SizeOf(Version);

      Version.wProductType := VER_NT_WORKSTATION;
      dwlConditionMask := VerSetConditionMask(0, VER_PRODUCT_TYPE, VER_EQUAL);
      Result := not VerifyVersionInfoW(Version, VER_PRODUCT_TYPE, dwlConditionMask);
    end;
  end;
end;

function IsWin64: Boolean;
var
  Kernel32Handle: THandle;
  IsWow64: BOOL;
  SystemInfo: TSystemInfo;

  IsWow64Process: function(Handle: THandle; var Res: BOOL): BOOL; stdcall;
  GetNativeSystemInfo: procedure(var lpSystemInfo: TSystemInfo); stdcall;
begin
  Result := False;

  Kernel32Handle := GetModuleHandle('KERNEL32.DLL');
  if Kernel32Handle = 0 then
    Kernel32Handle := LoadLibrary('KERNEL32.DLL');

  if Kernel32Handle <> 0 then
  begin
    IsWow64Process := GetProcAddress(Kernel32Handle, 'IsWow64Process');
    GetNativeSystemInfo := GetProcAddress(Kernel32Handle, 'GetNativeSystemInfo');
    if Assigned(IsWow64Process) then
    begin
      IsWow64Process(GetCurrentProcess, IsWow64);
      Result := IsWow64 and Assigned(GetNativeSystemInfo);
      if Result then
      begin
        GetNativeSystemInfo(SystemInfo);
        Result := (SystemInfo.wProcessorArchitecture = PROCESSOR_ARCHITECTURE_AMD64) or (SystemInfo.wProcessorArchitecture = PROCESSOR_ARCHITECTURE_IA64);
      end;
    end;
  end;
end;

function GetOSString(): string;
resourcestring
  SVersionStr = '%s (Version %d.%d, Build %d, %5:s)';
  SSPVersionStr = '%s Service Pack %4:d (Version %1:d.%2:d, Build %3:d, %5:s)';

  SVersion32 = '32-bit Edition';
  SVersion64 = '64-bit Edition';
const
  VersionStr: array[Boolean] of PResStringRec = (@SVersionStr, @SSPVersionStr);
  EditionStr: array[Boolean] of PResStringRec = (@SVersion32, @SVersion64);
var
  DllHandle: THandle;
  SystemInfo: TSystemInfo;
  VersionInfo: TOSVersionInfoExW;
  OsName: string;

  RtlGetVersion: function(var Version: TOSVersionInfoExW): Integer; stdcall;
  GetNativeSystemInfo: procedure(var lpSystemInfo: TSystemInfo); stdcall;
begin
  Result := 'Unknow windows';

  DllHandle := LoadLibrary('ntdll.dll');
  if DllHandle = 0 then Exit;
  try
    RtlGetVersion := GetProcAddress(DllHandle, 'RtlGetVersion');

    if (not Assigned(RtlGetVersion)) or ((RtlGetVersion(VersionInfo) <> 0)) then
    begin
      Exit;
    end;
  finally
    FreeLibrary(DllHandle);
  end;

  DllHandle := GetModuleHandle('KERNEL32.DLL');
  if DllHandle = 0 then
    DllHandle := LoadLibrary('KERNEL32.DLL');
  if DllHandle <> 0 then
  begin
    GetNativeSystemInfo := GetProcAddress(DllHandle, 'GetNativeSystemInfo');
    if Assigned(GetNativeSystemInfo) then
      GetNativeSystemInfo(SystemInfo)
    else
      GetSystemInfo(SystemInfo);

    OsName := 'Windows';
    if VersionInfo.dwMajorVersion = 10 then
    begin
      OsName := 'Windows 10';
    end
    else if VersionInfo.dwMajorVersion = 6 then
    begin
      case VersionInfo.dwMinorVersion of
        0:
          begin
            if VersionInfo.wProductType = VER_NT_WORKSTATION then
              OsName := 'Windows Vista'
            else
              OsName := 'Windows Server 2008';
          end;
        1:
          begin
            if VersionInfo.wProductType = VER_NT_WORKSTATION then
              OsName := 'Windows 7'
            else
              OsName := 'Windows Server 2008 R2';
          end;
        2:
          begin
            OsName := 'Windows 8';
          end;
      end;
    end
    else if VersionInfo.dwMajorVersion = 5 then
    begin
      case VersionInfo.dwMinorVersion of
        0:
          begin
            OsName := 'Windows 2000';
          end;
        1:
          begin
            OsName := 'Windows Server 2008 R2';
          end;
        2:
          begin
            if (VersionInfo.wProductType = VER_NT_WORKSTATION) and
              (SystemInfo.wProcessorArchitecture = PROCESSOR_ARCHITECTURE_AMD64) then
              OsName := 'Windows XP'
            else
            begin
              if GetSystemMetrics(SM_SERVERR2) = 0 then
                OsName := 'Windows 2003'
              else
                OsName := 'Windows Server 2003 R2'
            end;
          end;
      end;
    end;

    Result := Format(LoadResString(VersionStr[(VersionInfo.wServicePackMajor <> 0) and (VersionInfo.dwMajorVersion < 10)]),
      [OsName,
      VersionInfo.dwMajorVersion,
      VersionInfo.dwMinorVersion,
      VersionInfo.dwBuildNumber,
      VersionInfo.wServicePackMajor,
      LoadResString(EditionStr[IsWin64])]);
  end;
end;

end.
