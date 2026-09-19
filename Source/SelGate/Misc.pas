unit Misc;

interface

{$DEFINE SIGN3D}

uses
  Windows, Messages, SysUtils, WinSock2, ClientSession;

const
  SG_FORMHANDLE = 1000; // 服务器HANLD
  SG_STARTNOW = 1001; // 正在启动服务器...
  SG_STARTOK = 1002; // 服务器启动完成...
  SG_ACTIVE = 1003;

  GS_QUIT = 2000; // 关闭

type
  TProgamType = (tDBServer, tLoginSrv, tLogServer, tM2Server, tLoginGate,
    tLoginGate1, tSelGate, tSelGate1, tRunGate, tRunGate1, tRunGate2,
    tRunGate3, tRunGate4, tRunGate5, tRunGate6, tRunGate7);

procedure CloseIPConnect(const nRemoteIP: Integer);
function KickUser(const nRemoteIP: Integer): Boolean; overload;
procedure KickUser(const UserObj: TSessionObj); overload;
procedure BlockUser(const UserObj: TSessionObj);

function ReverseIP(dwIP: DWORD): DWORD;
function AnsiStrToVal(const nPtr: PChar; var nPos: Integer): Integer;

procedure SendGameCenterMsg(wIdent: Word; sSendMsg: string);

function CheckAccountName(sName: string): Boolean;

implementation

uses
  AcceptExWorkedThread, SHSocket, AppMain, Protocol,
  ConfigManager, IPAddrFilter, Grobal2, SDK;

procedure CloseIPConnect(const nRemoteIP: Integer);
var
  n: Integer;
  UserObj: TSessionObj;
begin
  if not g_fServiceStarted then
    Exit;
  for n := 0 to USER_ARRAY_COUNT - 1 do
  begin
    UserObj := g_UserList[n];
    if (UserObj <> nil) and
      (UserObj.m_tLastGameSvr <> nil) and
      (UserObj.m_tLastGameSvr.Active) and
      (UserObj.m_pUserOBJ.nIPAddr = nRemoteIP) then
    begin
      if UserObj.m_fHandleLogin >= 2 then
      begin
        UserObj.SendDefMessage(SM_OUTOFCONNECTION, UserObj.m_nSvrObject, 0, 0, 0,
          '');
        UserObj.m_fKickFlag := True;
      end
      else
        SHSocket.FreeSocket(UserObj.m_pUserOBJ._SendObj.Socket);
    end;
  end;
end;

function KickUser(const nRemoteIP: Integer): Boolean;
begin
  Result := True;
  if g_pConfig.m_fKickOverPacketSize then
  begin
    case g_pConfig.m_tBlockIPMethod of
      mDisconnect:
        begin
          Result := False;
        end;
      mBlock:
        begin
          AddToTempBlockIPList(nRemoteIP);
          CloseIPConnect(nRemoteIP);
          Result := False;
        end;
      mBlockList:
        begin
          AddToBlockIPList(nRemoteIP);
          CloseIPConnect(nRemoteIP);
          Result := False;
        end;
    end;
  end;
end;

procedure KickUser(const UserObj: TSessionObj);
begin
  if g_pConfig.m_fKickOverPacketSize then
  begin
    SHSocket.FreeSocket(UserObj.m_pUserOBJ._SendObj.Socket);
    UserObj.m_fKickFlag := True;
    case g_pConfig.m_tBlockIPMethod of
      mBlock:
        begin
          AddToTempBlockIPList(UserObj.m_pUserOBJ.nIPAddr);
        end;
      mBlockList:
        begin
          AddToBlockIPList(UserObj.m_pUserOBJ.nIPAddr);
        end;
    end;
  end;
end;

procedure BlockUser(const UserObj: TSessionObj);
begin
  if g_pConfig.m_fKickOverPacketSize then
  begin
    UserObj.m_fKickFlag := True;
    case g_pConfig.m_tBlockIPMethod of
      mBlock:
        begin
          AddToTempBlockIPList(UserObj.m_pUserOBJ.nIPAddr);
        end;
      mBlockList:
        begin
          AddToBlockIPList(UserObj.m_pUserOBJ.nIPAddr);
        end;
    end;
  end;
end;

function ReverseIP(dwIP: DWORD): DWORD;
begin
  Result := (LOBYTE(LOWORD(dwIP)) shl 24) or
    (HIBYTE(LOWORD(dwIP)) shl 16) or
    (LOBYTE(HIWORD(dwIP)) shl 8) or
    (HIBYTE(HIWORD(dwIP)));
end;

function AnsiStrToVal(const nPtr: PChar; var nPos: Integer): Integer;
var
  c: Char;
  tPtr: PChar;
  total: Integer;
begin
  nPos := 0;
  Result := 0;
  if nPtr = nil then
    Exit;
  tPtr := nPtr;
  c := tPtr^;
  total := 0;
  while ((c >= '0') and (c <= '9')) do
  begin
    total := 10 * total + (Byte(c) - Byte('0'));
    inc(tPtr);
    inc(nPos);
    c := tPtr^;
  end;
  Result := total;
end;

procedure SendGameCenterMsg(wIdent: Word; sSendMsg: string);
var
  SendData: TCopyDataStruct;
  nParam: Integer;
begin
  if not g_boNetComGate then
    nParam := MakeLong(Word(tSelGate), wIdent)
  else
    nParam := MakeLong(Word(tSelGate1), wIdent);
  
  SendData.cbData := Length(sSendMsg) + 1;
  GetMem(SendData.lpData, SendData.cbData);
  StrCopy(SendData.lpData, PChar(sSendMsg));
  SendMessage(g_hGameCenterHandle, WM_COPYDATA, nParam, Cardinal(@SendData));
  FreeMem(SendData.lpData);
end;

function CheckAccountName(sName: string): Boolean;
var
  I: Integer;
  nLen: Integer;
begin
  Result := False;
  if sName = '' then
    Exit;
  Result := true;
  nLen := Length(sName);
  I := 1;
  while (true) do
  begin
    if I > nLen then
      Break;
    if (sName[I] < '0') or (sName[I] > 'z') then
    begin
      Result := False;
      if (sName[I] >= #$B0) and (sName[I] <= #$C8) then
      begin
        Inc(I);
        if I <= nLen then
          if (sName[I] >= #$A1) and (sName[I] <= #$FE) then
            Result := true;
      end;
      if not Result then
        Break;
    end;
    Inc(I);
  end;
end;

end.
