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

  pTAddressInfo = ^TAddressInfo;
  TAddressInfo = record
    sIPaddr: string;
    nIPaddr: Integer;
    nCount: Integer;
    dwIPCountTick1: LongWord;
    nIPCount1: Integer;
    dwIPCountTick2: LongWord;
    nIPCount2: Integer;
    dwDenyTick: LongWord;
    nIPDenyCount: Integer;

    dwIPAccountErrTick: LongWord;
    nIPAccountErrCount: Integer;

    dwIPPasswordErrTick: LongWord;
    nIPPasswordErrCount: Integer;

    dwIPPasswordProtectedErrTick: LongWord;
    nIPPasswordProtectedErrCount: Integer;
  end;

  TAddressListEx = class(TObject)
  private
    FAddress: TList;
    GLock: TRTLCriticalSection;
    function GetItems(Index: Integer): pTAddressInfo;
    function GetCount: Integer;
  public
    constructor Create();
    destructor Destroy; override;

    property Items[Index: Integer]: pTAddressInfo read GetItems; default;
    property Count: Integer read GetCount;
    function Find(IP: string): PTAddressInfo;
    function Add(IP: string): PTAddressInfo;
    procedure Delete(AddressInfo: pTAddressInfo);

    procedure Clear;
    procedure Lock;
    procedure UnLock;
  end;

procedure StartService();
procedure StopService();
procedure OnTimerProc(hWnd: hWnd; uMsg: UINT; idEvent: UINT_PTR; dwTime: DWORD); stdcall;

var
  g_ProcMsgThread: TProcMsgThread = nil;
  g_CurrIPaddrList: TAddressListEx = nil;

  g_nProtocolKey1: DWORD = 0;

implementation

uses
  AppMain,
  IOCPManager,
  AcceptExWorkedThread,
  SHSocket,
  ConfigManager,
  Grobal2,
  HUtil32,
  Misc,
  LogManager,
  {$IF VER_TYPE = 1}
  //WinlicenseSDK,
  {$IFEND}
  IPAddrFilter;

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
  i: Integer;
begin
  Lock();
  try
    i := m_xUserList.IndexOf(CSession);
    if i >= 0 then
    begin
      m_xUserList.Delete(i);
    end;
  finally
    Unlock();
  end;
end;

function TProcMsgThread.GetSession(nSock: Integer): TSessionObj;
var
  i: Integer;
  UserOBJ: TSessionObj;
begin
  Result := nil;
  if nSock <> INVALID_SOCKET then
  begin
    Lock();
    try
      for i := 0 to m_xUserList.Count - 1 do
      begin
        UserOBJ := TSessionObj(m_xUserList[i]);
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
  i: Integer;
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
    for i := 0 to m_xUserList.Count - 1 do
    begin
      m_xTempUserList.Add(m_xUserList[i]);
    end;
  finally
    Unlock();
  end;

  for i := 0 to m_xTempUserList.Count - 1 do
  begin
    CSession := TSessionObj(m_xTempUserList[i]);
    if (CSession <> nil) and (CSession.m_tLastGameSvr <> nil) then
    begin
      if not CSession.m_fKickFlag then
      begin
        if (CSession.m_fHandleLogin < 3) then
        begin
          if CSession.m_IsDelayClose and (GetTickCount >= CSession.m_dwDelayCloseTick) then
          begin
            CSession.m_fKickFlag := True;
            CSession.m_IsDelayClose := False;
            SHSocket.FreeSocket(CSession.m_pUserOBJ._SendObj.Socket);
            {
            if g_pLogMgr.CheckLevel(5) then
            begin
              g_pLogMgr.Add('Client Connect delay: ' + CSession.m_pUserOBJ.pszIPAddr);
            end;
            }
          end
          else if (GetTickCount - CSession.m_dwClientTimeOutTick > g_pConfig.m_nClientTimeOutTime) then
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
          if g_pLogMgr.CheckLevel(5) then
          begin
            g_pLogMgr.Add('Client Connect Time Out 2: ' + CSession.m_pUserOBJ.pszIPAddr);
          end;

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

{ TAddressListEx }

constructor TAddressListEx.Create;
begin
  inherited Create;
  InitializeCriticalSection(GLock);
  FAddress := TList.Create;
end;

destructor TAddressListEx.Destroy;
begin
  Clear;
  FAddress.Free;
  DeleteCriticalSection(GLock);
  inherited Destroy;
end;

procedure TAddressListEx.Clear;
var
  I: Integer;
begin
  Lock;
  try
    for I := 0 to FAddress.Count - 1 do
    begin
      Dispose(pTAddressInfo(FAddress.Items[I]));
    end;
    FAddress.Clear;
  finally
    UnLock;
  end;
end;

function TAddressListEx.GetItems(Index: Integer): pTAddressInfo;
begin
  if (Index >= 0) and (Index <= FAddress.Count - 1) then
    Result := FAddress[Index]
  else
    Result := nil;
end;

procedure TAddressListEx.Lock;
begin
  EnterCriticalSection(GLock);
end;

procedure TAddressListEx.UnLock;
begin
  LeaveCriticalSection(GLock);
end;

function TAddressListEx.GetCount: Integer;
begin
  Result := FAddress.Count;
end;

function TAddressListEx.Add(IP: string): PTAddressInfo;
begin
  New(Result);
  ZeroMemory(Result, SizeOf(TAddressInfo));
  Result.nIPaddr := inet_addr(PChar(IP));
  Result.sIPaddr := IP;
  FAddress.Add(Result);
end;

function TAddressListEx.Find(IP: string): PTAddressInfo;
var
  I: Integer;
  nIP: Integer;
  AddressInfo: PTAddressInfo;
begin
  Result := nil;
  nIP := inet_addr(PChar(IP));
  for I := 0 to FAddress.Count - 1 do
  begin
    AddressInfo := FAddress.Items[I];
    if AddressInfo.nIPaddr = nIP then
    begin
      Result := AddressInfo;
      Exit;
    end;
  end;
end;

procedure TAddressListEx.Delete(AddressInfo: pTAddressInfo);
var
  I: Integer;
begin
  for I := 0 to FAddress.Count - 1 do
  begin
    if FAddress.Items[I] = AddressInfo then
    begin
      Dispose(AddressInfo);
      FAddress.Delete(I);
      Exit;
    end;
  end;
end;

procedure StartService();
{$IF VER_TYPE = 1}
var
  RetStatus, ExtendedInfo: Integer;
  ExpDate: TSystemTime;
  HWID: array[0..100] of Char;
{$IFEND}
begin
  g_fServiceStarted := True;

  g_ProcMsgThread.Enabled := True;

  g_pLogMgr.Add('正在启动服务 ...');
  SendGameCenterMsg(SG_STARTNOW, _STR_NOW_START);

  g_pConfig.LoadConfig();

  IPAddrFilter.ClearConnectOfIP();

  with FormMain do begin
    if g_pConfig.m_szTitle <> '' then begin
        Caption := Format('LoginGate - [%s]', [g_pConfig.m_szTitle]);
    end else begin
        Caption := 'LoginGate';
    end;

    {$IF VER_TYPE = 1}
    (*
    StrEncryptStart;
    if Register_Str <> '' then begin
      Caption := Caption + '【' + Register_Str + '】';
    end;
    StrEncryptEnd;
    *)
    {$IFEND}

    MENU_CONTROL_START.Enabled := False;
    MENU_CONTROL_STOP.Enabled := True;
    FormMain.m_xRunServerList := TIOCPManager.Create;
    FormMain.m_xGameServerList := TGameServerManager.Create;
    FormMain.InitIOCPServer;
  end;

  g_pLogMgr.Add('服务已启动成功...');

  //SetTimer(g_hMainWnd, _IDM_TIMER_KEEP_ALIVE, 4 * 1000, Pointer(@OnTimerProc));
  SetTimer(g_hMainWnd, _IDM_TIMER_THREAD_INFO, 1000, Pointer(@OnTimerProc));

  //g_ProcMsgThread := TProcMsgThread.Create;

  FormMain.MENU_VIEW_HELP_ABOUTClick(nil);

{$IF VER_TYPE = 1}
{.$I CodeReplace_Start.inc}
  RetStatus := WLRegGetStatus(ExtendedInfo);
  if RetStatus = wlIsRegistered then begin
    WLRegExpirationDate(ExpDate);
    g_pLogMgr.Add('授权到期：' + FormatDateTime('yyyy-mm-dd', SystemTimeToDateTime(ExpDate)));
  end else if RetStatus = wlIsTrial then begin
    WLHardwareGetID(HWID);
    g_pLogMgr.Add('机 器 码：' + StrPas(HWID));
  end else begin
    WLHardwareGetID(HWID);
    g_pLogMgr.Add('授权到期或无效授权；机器码：' + StrPas(HWID));
  end;
{.$I CodeReplace_end.inc}
{$IFEND}

  SendGameCenterMsg(SG_STARTOK, _STR_STARTED);
end;

procedure StopService;
var
  i: Integer;
begin
  g_fServiceStarted := False;

  //g_ProcMsgThread.Terminated := True;

  g_pLogMgr.Add('正在停止服务...');

  //KillTimer(g_hMainWnd, _IDM_TIMER_KEEP_ALIVE);

  with FormMain do
  begin
    for i := 1 to GridSocketInfo.RowCount - 1 do
      GridSocketInfo.Rows[i].Clear;

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
  i: Integer;
  //CmdPacket                 : TSvrCmdPack;
begin
  if not g_fServiceStarted then
    Exit;
  {with CmdPacket do begin
    Flag := RUNGATECODE;
    SockID := 0;
    Cmd := GM_CHECKCLIENT;
    DataLen := 0;
  end;}
  try
    for i := 0 to FormMain.m_xGameServerList.GameServerCount - 1 do
    begin
      if FormMain.m_xGameServerList[i].Active then
      begin
        FormMain.m_xGameServerList[i].SendBuffer('%--$', 4);
      end;
      if FormMain.m_xGameServerList[i].Active and FormMain.m_xGameServerList[i].m_fKeepAlive then
      begin
        if GetTickCount() - FormMain.m_xGameServerList[i].m_dwKeepAliveTick > 25 * 1000 then
        begin
          FormMain.m_xGameServerList[i].m_dwKeepAliveTick := GetTickCount();
          FormMain.m_xGameServerList[i].Active := False;
          FormMain.m_xGameServerList[i].m_tSockThreadStutas := stTimeOut;
        end;
      end;
    end;

    //m_tSockThreadStutas
    //FormMain.m_xRunServerList.IOCPServer.Writer.PostAllUserSend(_STR_KEEP_ALIVE, SizeOf(Char));
  except
    on E: exception do
      if g_pLogMgr.CheckLevel(5) then
        g_pLogMgr.Add('Keep Alive Except: ' + E.Message);
  end;
end;

procedure ShowThreadInfo();
var
  i, nLen, nRow: Integer;
  IOCPAccepter: TIOCPAccepter;
  PSI: PServerInfo;
  pszBuf: array[0..512 - 1] of Char;
begin
  if not g_fServiceStarted then
    Exit;

  IOCPAccepter := FormMain.m_xRunServerList.IOCPServer.AcceptExSocket;

  nRow := 1;
  for i := 0 to IOCPAccepter.ServerCount - 1 do
  begin
    if IOCPAccepter.Active then
    begin
      PSI := @IOCPAccepter.FServerInfoList[i];
      if not PSI^.pClient.Active then
        Continue;
      FormMain.GridSocketInfo.Cells[0, nRow] := PSI^.pClient.ServerIP;
      FormMain.GridSocketInfo.Cells[1, nRow] := IntToStr(PSI^.nPort);
      case PSI.pClient.m_tSockThreadStutas of
        stConnecting: FormMain.GridSocketInfo.Cells[2, nRow] := '连接中..';
        stConnected: FormMain.GridSocketInfo.Cells[2, nRow] := '已连接';
        stTimeOut: FormMain.GridSocketInfo.Cells[2, nRow] := '超时';
      end;

      FormMain.StatusBar.Panels[0].Text := Format('连接: %d/%d', [IOCPAccepter.InUseBlock, IOCPAccepter.MaxInUseBlock]);

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

      FormMain.GridSocketInfo.Cells[3, nRow] := pszBuf;

    end;
    Inc(nRow);
  end;
end;

procedure OnTimerProc(hWnd: hWnd; uMsg: UINT; idEvent: UINT_PTR; dwTime: DWORD);
var
  i: Integer;
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

