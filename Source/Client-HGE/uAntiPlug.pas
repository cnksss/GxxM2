unit uAntiPlug;

interface

uses
  Windows,
  SysUtils,
  Classes;

implementation

function EnableDebugPrivilege:Boolean;
var
  Token:THandle;
  tp:TTokenPrivileges;
  prev:TTokenPrivileges;
  ReturnLength:Dword;
begin
  Result := False;
  if (OpenProcessToken(GetCurrentProcess(), TOKEN_QUERY or TOKEN_ADJUST_PRIVILEGES, Token)) then begin
    ZeroMemory(@tp, SizeOf(tp));
    tp.PrivilegeCount := 1;

    if LookupPrivilegeValue(nil, 'SeDebugPrivilege', tp.Privileges[0].Luid) then begin
      tp.Privileges[0].Attributes := SE_PRIVILEGE_ENABLED;
      if (AdjustTokenPrivileges(Token, FALSE, tp, SizeOf(tp), prev, ReturnLength)) then
        Result := True;
      CloseHandle(Token);
    end;
  end;
end;

end.
