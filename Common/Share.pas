unit Share;

interface
uses
  Windows, Messages, SysUtils, Variants, Classes, HUtil32, Registry;
function MakeIPToInt(sIPaddr: string): Integer;
function MakeIntToIP(nIPaddr: Integer): string;
function GetWindowsVersion: string; // 取系统版本号（字符串形式）
implementation
const
  VER_NT_WORKSTATION = $00000001;
  VER_NT_DOMAIN_CONTROLLER = $00000002;
  VER_NT_SERVER = $00000003;

  VER_SERVER_NT = $80000000;
  VER_WORKSTATION_NT = $40000000;

  VER_SUITE_SMALLBUSINESS = $00000001;
  VER_SUITE_ENTERPRISE = $00000002;
  VER_SUITE_BACKOFFICE = $00000004;
  VER_SUITE_COMMUNICATIONS = $00000008;
  VER_SUITE_TERMINAL = $00000010;
  VER_SUITE_SMALLBUSINESS_RESTRICTED = $00000020;
  VER_SUITE_DATACENTER = $00000080;
  VER_SUITE_SINGLEUSERTS = $00000100;
  VER_SUITE_PERSONAL = $00000200;
  VER_SUITE_BLADE = $00000400;

type
  TIPaddr = record
    A: Byte;
    B: Byte;
    C: Byte;
    D: Byte;
    Port: Integer;
  end;


  POSVersionInfoEx = ^TOSVersionInfoEx;
  OSVERSIONINFOEXA = record
    dwOSVersionInfoSize: DWORD;
    dwMajorVersion: DWORD;
    dwMinorVersion: DWORD;
    dwBuildNumber: DWORD;
    dwPlatformId: DWORD;
    szCSDVersion: array[0..127] of AnsiChar;
    wServicePackMajor: WORD;
    wServicePackMinor: WORD;
    wSuiteMask: WORD;
    wProductType: BYTE;
    wReserved: BYTE;
  end;
  OSVERSIONINFOEXW = record
    dwOSVersionInfoSize: DWORD;
    dwMajorVersion: DWORD;
    dwMinorVersion: DWORD;
    dwBuildNumber: DWORD;
    dwPlatformId: DWORD;
    szCSDVersion: array[0..127] of WideChar;
    wServicePackMajor: WORD;
    wServicePackMinor: WORD;
    wSuiteMask: WORD;
    wProductType: BYTE;
    wReserved: BYTE;
  end;
  OSVERSIONINFOEX = OSVERSIONINFOEXA;
  TOSVersionInfoEx = OSVERSIONINFOEX;

function MakeIPToStr(IPAddr: TIPaddr): string;
begin
  Result := IntToStr(IPAddr.A) + '.' + IntToStr(IPAddr.B) + '.' + IntToStr(IPAddr.C) + '.' + IntToStr(IPAddr.D);
end;

function MakeIntToIP(nIPaddr: Integer): string;
var
  IPAddr: TIPaddr;
begin
  FillChar(IPAddr, SizeOf(TIPaddr), 0);
  IPAddr.A := LoByte(LoWord(nIPaddr));
  IPAddr.B := HiByte(LoWord(nIPaddr));
  IPAddr.C := LoByte(HiWord(nIPaddr));
  IPAddr.D := HiByte(HiWord(nIPaddr));
  Result := MakeIPToStr(IPAddr);
end;

function MakeIPToInt(sIPaddr: string): Integer;
var
  sA, sB, SC, sD: string;
  A, B, C, D: Byte;
begin
  Result := -1;
  sIPaddr := Trim(GetValidStr3(sIPaddr, sA, ['.']));
  sIPaddr := Trim(GetValidStr3(sIPaddr, sB, ['.']));
  sD := Trim(GetValidStr3(sIPaddr, SC, ['.']));
  if (sA <> '') and (sB <> '') and (SC <> '') and (sD <> '') then begin
    A := Str_ToInt(sA, 0);
    B := Str_ToInt(sB, 0);
    C := Str_ToInt(SC, 0);
    D := Str_ToInt(sD, 0);
    Result := MakeLong(MakeWord(A, B), MakeWord(C, D));
  end;
end;


function GetWindowsVersion: string; // 取系统版本号（字符串形式）
var
  osVerInfo: TOSVersionInfoEx;
  ExVerExist: Boolean;
  ProductType: string;
begin

  Result := 'Microsoft Windows';
  ExVerExist := True;
  osVerInfo.dwOSVersionInfoSize := SizeOf(TOSVersionInfoEx);
  if not GetVersionEx(POSVersionInfo(@osVerInfo)^) then
  begin
    osVerInfo.dwOSVersionInfoSize := SizeOf(TOSVersionInfo);
    GetVersionEx(POSVersionInfo(@osVerInfo)^);
    ExVerExist := False;
  end;
  with osVerInfo do begin
    case dwPlatformId of
      VER_PLATFORM_WIN32s: Result := Result + Format(' %d.%d', [dwMajorVersion, dwMinorVersion]);
      VER_PLATFORM_WIN32_WINDOWS: { Windows 9x/ME }
        begin
          if (dwMajorVersion = 4) and (dwMinorVersion = 0) then
          begin
            Result := Result + ' 95';
            if szCSDVersion[1] in ['B', 'C'] then
              Result := Result + ' OSR2';
          end
          else if (dwMajorVersion = 4) and (dwMinorVersion = 10) then
          begin
            Result := Result + ' 98';
            if (osVerInfo.szCSDVersion[1] = 'A') then
              Result := Result + ' Second Edition';
          end
          else if (dwMajorVersion = 4) and (dwMinorVersion = 90) then
            Result := Result + ' Millenium Edition';
        end;
      VER_PLATFORM_WIN32_NT: { Windows NT/2000 }
        begin
          case dwMajorVersion of
            3, 4: Result := Result + Format(' NT %d.%d', [dwMajorVersion, dwMinorVersion]);
            5: begin
                if dwMinorVersion = 0 then
                  Result := Result + ' 2000'
                else if dwMinorVersion = 1 then
                  Result := Result + ' XP'
                else if dwMinorVersion = 2 then
                  Result := Result + ' 2003 Server';
              end;
            6: begin
                Result := Result + ' Vista';
              end;
          end;

          if ExVerExist then
          begin
            if wProductType = VER_NT_WORKSTATION then
            begin
              if dwMajorVersion = 4 then
                Result := Result + ' Workstation'
              else if wSuiteMask and VER_SUITE_PERSONAL <> 0 then
                Result := Result + ' Home Edition'
              else
                Result := Result + ' Professional';
            end
            else if wProductType = VER_NT_SERVER then
            begin
              if (dwMajorVersion = 5) and (dwMinorVersion = 2) then
              begin
                if wSuiteMask and VER_SUITE_DATACENTER <> 0 then
                  Result := Result + ' Datacenter Edition'
                else if wSuiteMask and VER_SUITE_ENTERPRISE <> 0 then
                  Result := Result + ' Enterprise Edition'
                else if wSuiteMask and VER_SUITE_BLADE <> 0 then
                  Result := Result + ' Web Edition'
                else
                  Result := Result + ' Standard Edition';
              end
              else if (dwMajorVersion = 5) and (dwMinorVersion = 0) then
              begin
                if wSuiteMask and VER_SUITE_DATACENTER <> 0 then
                  Result := Result + ' Datacenter Server'
                else if wSuiteMask and VER_SUITE_ENTERPRISE <> 0 then
                  Result := Result + ' Advanced Server'
                else
                  Result := Result + ' Server'
              end
              else
              begin
                Result := Result + ' Server';
                if wSuiteMask and VER_SUITE_ENTERPRISE <> 0 then
                  Result := Result + ' Enterprise Edition';
              end;
            end;
          end
          else
          begin
            with TRegistry.Create do
            begin
              try
                RootKey := HKEY_LOCAL_MACHINE;
                if OpenKey('\SYSTEM\CurrentControlSet\Control\ProductOptions', False) then
                begin
                  if ValueExists('ProductType') then
                  begin
                    ProductType := ReadString('ProductType');
                    if SameText(ProductType, 'WinNT') then
                      Result := Result + ' Workstation'
                    else if SameText(ProductType, 'LanManNT') then
                      Result := Result + ' Server'
                    else if SameText(ProductType, 'ServerNT') then
                      Result := Result + ' Advance Server';
                  end;
                  CloseKey;
                end;
              finally
                Free;
              end;
            end;
          end;

          Result := Result + ' ' + szCSDVersion;
          if (dwMajorVersion = 4) and SameText(szCSDVersion, 'Service Pack 6') then
          begin
            with TRegistry.Create do
            begin
              try
                RootKey := HKEY_LOCAL_MACHINE;
                if OpenKey('\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Hotfix\Q246009', False) then
                begin
                  Result := Result + 'a';
                  CloseKey;
                end;
              finally
                Free;
              end;
            end;
          end;

          Result := Result + Format(' (Build %d)', [dwBuildNumber and $FFFF]);
        end;
    end;
  end
end;

end.

