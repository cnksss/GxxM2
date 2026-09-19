unit FuncForComm;

interface

uses
  Windows, Messages, SysUtils, Classes, ClientSession,
  WinSock2, ThreadPool, Protocol, SDK, ExtCtrls;

type
  TProcMsgThread = class(TTimer)
  public
    m_nProcIdx: Integer;
    m_xUserList: Classes.TList;
    m_xTempUserList: Classes.TList;
    m_csAccess: TRTLCriticalSection;
  protected
    //procedure Run(); override;
    procedure Run(Sender: TObject);
  public
    constructor Create;
    destructor Destroy; override;
    procedure Lock();
    procedure Unlock();
    procedure AddSession(CSession: TSessionObj);
    procedure DelSession(CSession: TSessionObj);
    function GetSession(nSock: Integer): TSessionObj;
  end;

procedure StartService();
procedure StopService();
procedure OnTimerProc(hWnd: hWnd; uMsg: UINT; idEvent: UINT_PTR; dwTime: DWORD); stdcall;

var
  g_LogLock: TRTLCriticalSection;
  g_AppLogPath: string = '';
  g_ProcMsgThread: TProcMsgThread = nil;
  g_LoginData: Integer = 0;

implementation

uses
  AppMain, IOCPManager, AcceptExWorkedThread, SHSocket,
  ConfigManager, Grobal2, HUtil32, Misc, LogManager, IPAddrFilter;

constructor TProcMsgThread.Create;
begin
  InitializeCriticalSection(m_csAccess);
  inherited Create(nil);
  m_nProcIdx := 0;
  m_xUserList := Classes.TList.Create;
  m_xTempUserList := Classes.TList.Create;
  Self.Interval := 1;
  Self.Enabled := True;
  Self.OnTimer := Run;
end;

destructor TProcMsgThread.Destroy;
begin
  m_xUserList.Free;
  m_xTempUserList.Free;
  DeleteCriticalSection(m_csAccess);
  inherited Destroy;
end;

procedure TProcMsgThread.Lock;
begin
  EnterCriticalSection(m_csAccess);
end;

procedure TProcMsgThread.Unlock;
begin
  LeaveCriticalSection(m_csAccess);
end;

procedure TProcMsgThread.AddSession(CSession: TSessionObj);
begin
  Lock();
  try
    m_xUserList.Add(CSession);
  finally
    Unlock();
  end;
end;

procedure TProcMsgThread.DelSession(CSession: TSessionObj);
var
  I: Integer;
begin
  Lock();
  try
    I := m_xUserList.IndexOf(CSession);
    if I >= 0 then
    begin
      m_xUserList.Delete(I);
    end;
  finally
    Unlock();
  end;
end;

function TProcMsgThread.GetSession(nSock: Integer): TSessionObj;
var
  I: Integer;
  UserOBJ: TSessionObj;
begin
  Result := nil;
  if nSock <> INVALID_SOCKET then
  begin
    Lock();
    try
      for I := 0 to m_xUserList.Count - 1 do
      begin
        UserOBJ := TSessionObj(m_xUserList[I]);
        if UserOBJ.m_pUserOBJ._SendObj.Socket = nSock then
        begin
          Result := UserOBJ;
          Break;
        end;
      end;
    finally
      Unlock();
    end;
  end;
end;

procedure TProcMsgThread.Run(Sender: TObject);
var
  I: Integer;
  CSession: TSessionObj;
begin
  //while not Terminated do begin
  //  if Terminated then
  //    Break;
  if not g_fServiceStarted then
    Exit;

  if m_xTempUserList.Count > 0 then
    m_xTempUserList.Clear;

  Lock();
  try
    for I := 0 to m_xUserList.Count - 1 do
    begin
      m_xTempUserList.Add(m_xUserList[I]);
    end;
  finally
    Unlock();
  end;

  for I := 0 to m_xTempUserList.Count - 1 do
  begin
    CSession := TSessionObj(m_xTempUserList[I]);
    if (CSession <> nil) and (CSession.m_tLastGameSvr <> nil) then
    begin
      if not CSession.m_fKickFlag then
      begin
        if (CSession.m_fHandleLogin < 3) then
        begin
          if (GetTickCount - CSession.m_dwClientTimeOutTick > g_pConfig.m_nClientTimeOutTime) then
          begin
            CSession.m_dwClientTimeOutTick := GetTickCount;
            CSession.SendDefMessage(SM_OUTOFCONNECTION, CSession.m_nSvrObject, 0, 0, 0, '');
            CSession.m_fKickFlag := True;
            BlockUser(CSession);
            if g_pLogMgr.CheckLevel(5) then
            begin
              g_pLogMgr.Add('Client Connect Time Out: ' + CSession.m_pUserOBJ.pszIPAddr);
            end;
          end;
        end;
      end
      else
      begin
        if (GetTickCount - CSession.m_dwClientTimeOutTick > g_pConfig.m_nClientTimeOutTime) then
        begin
          CSession.m_dwClientTimeOutTick := GetTickCount;
          SHSocket.FreeSocket(CSession.m_pUserOBJ._SendObj.Socket);
        end;
      end;
    end;
  end;
  //m_xTempUserList.Clear;

  //  Sleep(1);
  //end;
end;

procedure StartService();
begin
  g_fServiceStarted := True;

  g_ProcMsgThread.Enabled := True;

  g_pLogMgr.Add('正在启动服务 ...');
  SendGameCenterMsg(SG_STARTNOW, _STR_NOW_START);

  g_pConfig.LoadConfig();

  IPAddrFilter.ClearConnectOfIP();

  with FrmMain do begin
    if g_pConfig.m_szTitle <> '' then begin
        Caption := Format('SelGate - [%s]', [g_pConfig.m_szTitle]);
    end else begin
        Caption := 'SelGate';
    end;
    MENU_CONTROL_START.Enabled := False;
    MENU_CONTROL_STOP.Enabled := True;
    FrmMain.m_xRunServerList := TIOCPManager.Create;
    FrmMain.m_xGameServerList := TGameServerManager.Create;
    FrmMain.InitIOCPServer;
  end;

  g_pLogMgr.Add('服务已启动成功...');

  //SetTimer(g_hMainWnd, _IDM_TIMER_KEEP_ALIVE, 4 * 1000, Pointer(@OnTimerProc));
  SetTimer(g_hMainWnd, _IDM_TIMER_THREAD_INFO, 1000, Pointer(@OnTimerProc));

  //g_ProcMsgThread := TProcMsgThread.Create;

  //FrmMain.MENU_VIEW_HELP_ABOUTClick(nil);

  SendGameCenterMsg(SG_STARTOK, _STR_STARTED);
end;

procedure StopService;
var
  I: Integer;
begin
  g_fServiceStarted := False;

  //g_ProcMsgThread.Terminated := True;

  g_pLogMgr.Add('正在停止服务...');

  //KillTimer(g_hMainWnd, _IDM_TIMER_KEEP_ALIVE);

  with FrmMain do
  begin
    for I := 1 to GridSocketInfo.RowCount - 1 do
      GridSocketInfo.Rows[I].Clear;

    MENU_CONTROL_START.Enabled := True;
    MENU_CONTROL_STOP.Enabled := False;
    try
      m_xGameServerList.CloseAllGameServer;
      Sleep(80);
      m_xRunServerList.Free;
      Sleep(20);
      m_xGameServerList.Free;
    except
    end;
  end;
  IPAddrFilter.ClearConnectOfIP();

  SaveBlockIPList();
  SaveBlockIPAreaList();

  g_ProcMsgThread.Enabled := False;

  g_pLogMgr.Add('服务停止成功...');
end;

procedure KeepAlive();
var
  I: Integer;
  //CmdPacket                 : TSvrCmdPack;
begin
  if not g_fServiceStarted then  Exit;
  try
    for I := 0 to FrmMain.m_xGameServerList.GameServerCount - 1 do
    begin
      if FrmMain.m_xGameServerList[I].Active then
      begin
        FrmMain.m_xGameServerList[I].SendBuffer('%--$', 4);
      end;
      if FrmMain.m_xGameServerList[I].Active and FrmMain.m_xGameServerList[I].m_fKeepAlive then
      begin
        if GetTickCount() - FrmMain.m_xGameServerList[I].m_dwKeepAliveTick > 25 * 1000 then
        begin
          FrmMain.m_xGameServerList[I].m_dwKeepAliveTick := GetTickCount();
          FrmMain.m_xGameServerList[I].Active := False;
          FrmMain.m_xGameServerList[I].m_tSockThreadStutas := stTimeOut;
        end;
      end;
    end;

    //m_tSockThreadStutas
    //FrmMain.m_xRunServerList.IOCPServer.Writer.PostAllUserSend(_STR_KEEP_ALIVE, SizeOf(Char));
  except
    on E: exception do
      if g_pLogMgr.CheckLevel(5) then
        g_pLogMgr.Add('Keep Alive Except: ' + E.Message);
  end;
end;

procedure ShowThreadInfo();
var
  I, nLen, nRow: Integer;
  IOCPAccepter: TIOCPAccepter;
  PSI: PServerInfo;
  pszBuf: array[0..512 - 1] of Char;
begin
  if not g_fServiceStarted then
    Exit;

  IOCPAccepter := FrmMain.m_xRunServerList.IOCPServer.AcceptExSocket;

  nRow := 1;
  for I := 0 to IOCPAccepter.ServerCount - 1 do
  begin
    if IOCPAccepter.Active then
    begin
      PSI := @IOCPAccepter.FServerInfoList[I];
      if not PSI^.pClient.Active then
        Continue;
      FrmMain.GridSocketInfo.Cells[0, nRow] := PSI^.pClient.ServerIP;
      FrmMain.GridSocketInfo.Cells[1, nRow] := IntToStr(PSI^.nPort);
      case PSI.pClient.m_tSockThreadStutas of
        stConnecting: FrmMain.GridSocketInfo.Cells[2, nRow] := '连接中..';
        stConnected: FrmMain.GridSocketInfo.Cells[2, nRow] := '已连接';
        stTimeOut: FrmMain.GridSocketInfo.Cells[2, nRow] := '超时';
      end;

      FrmMain.StatusBar.Panels[0].Text := Format('连接: %d/%d', [IOCPAccepter.InUseBlock, IOCPAccepter.MaxInUseBlock]);

      if PSI^.pClient.m_dwSendBytes > (1024 * 1000) then
        StrFmt(@pszBuf[0], '↑%fM', [PSI^.pClient.m_dwSendBytes / (1024 * 1000)])
      else if PSI^.pClient.m_dwSendBytes > 1024 then
        StrFmt(@pszBuf[0], '↑%fK', [PSI^.pClient.m_dwSendBytes / 1024])
      else
        StrFmt(@pszBuf[0], '↑%dB', [PSI^.pClient.m_dwSendBytes]);
      PSI^.pClient.m_dwSendBytes := 0;

      nLen := StrLen(pszBuf);
      pszBuf[nLen] := ' ';
      pszBuf[nLen + 1] := ' ';
      if PSI^.pClient.m_dwRecvBytes > (1024 * 1000) then
        StrFmt(@pszBuf[nLen + 2], '↓%fM', [PSI^.pClient.m_dwRecvBytes / (1024 * 1000)])
      else if PSI^.pClient.m_dwRecvBytes > 1024 then
        StrFmt(@pszBuf[nLen + 2], '↓%fK', [PSI^.pClient.m_dwRecvBytes / 1024])
      else
        StrFmt(@pszBuf[nLen + 2], '↓%dB', [PSI^.pClient.m_dwRecvBytes]);
      PSI^.pClient.m_dwRecvBytes := 0;

      FrmMain.GridSocketInfo.Cells[3, nRow] := pszBuf;

    end;
    Inc(nRow);
  end;
end;

procedure OnTimerProc(hWnd: hWnd; uMsg: UINT; idEvent: UINT_PTR; dwTime: DWORD);
begin
  case idEvent of
    _IDM_TIMER_STARTSERVICE:
      begin
        KillTimer(g_hMainWnd, _IDM_TIMER_STARTSERVICE);
        StartService();
      end;
    _IDM_TIMER_STOPSERVICE:
      begin
        KillTimer(g_hMainWnd, _IDM_TIMER_STOPSERVICE);
        StopService();
      end;
    _IDM_TIMER_KEEP_ALIVE:
      begin
        KeepAlive();
      end;
    _IDM_TIMER_THREAD_INFO:
      begin
        ShowThreadInfo();
      end;
  end;
end;

end.
