unit AppMain;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, Menus, StdCtrls, ExtCtrls, Grids, MsXML, DateUtils, ComCtrls, IniFiles,
  IOCPManager, ClientThread, AcceptExWorkedThread, Protocol, SDK;

const
  WM_SET_PARENT_WINDOW = WM_USER + 123;       // 嵌入到控制台

type
  TFrmMain = class(TForm)
    MainMenu: TMainMenu;
    MENU_CONTROL: TMenuItem;
    MENU_CONTROL_START: TMenuItem;
    MENU_CONTROL_STOP: TMenuItem;
    MENU_CONTROL_RELOADCONFIG: TMenuItem;
    MENU_CONTROL_CLEAELOG: TMenuItem;
    MENU_CONTROL_EXIT: TMenuItem;
    MENU_OPTION: TMenuItem;
    MENU_OPTION_GENERAL: TMenuItem;
    MENU_OPTION_IPFILTER: TMenuItem;
    MENU_VIEW_HELP: TMenuItem;
    MENU_VIEW_HELP_ABOUT: TMenuItem;
    GridSocketInfo: TStringGrid;
    Splitter: TSplitter;
    StatusBar: TStatusBar;
    MemoLog: TMemo;
    mmiLogData: TMenuItem;
    procedure FormCreate(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure FormCloseQuery(Sender: TObject; var CanClose: Boolean);
    procedure MENU_VIEW_HELP_ABOUTClick(Sender: TObject);
    procedure MENU_CONTROL_STARTClick(Sender: TObject);
    procedure MENU_CONTROL_STOPClick(Sender: TObject);
    procedure MENU_CONTROL_EXITClick(Sender: TObject);
    procedure MemoLogDblClick(Sender: TObject);
    procedure GridSocketInfoDrawCell(Sender: TObject; ACol, ARow: Integer; Rect: TRect; State: TGridDrawState);
    procedure MENU_OPTION_GENERALClick(Sender: TObject);
    procedure MENU_OPTION_IPFILTERClick(Sender: TObject);
    procedure MENU_CONTROL_CLEAELOGClick(Sender: TObject);
    procedure MENU_CONTROL_RELOADCONFIGClick(Sender: TObject);
    procedure mmiLogDataClick(Sender: TObject);

  private
    FIsEmbeddedGameCenter: Boolean;
    procedure WMSysCommand(var Message: TWMSysCommand); message WM_SYSCOMMAND;
    procedure WMSetParentWindow(var Message: TMessage); message WM_SET_PARENT_WINDOW;

    procedure OnAppModalBegin(Sender: TObject);
    procedure OnAppOnModalEnd(Sender: TObject);
  private
    { Private declarations }
    procedure OnProgramException(Sender: TObject; E: Exception);
    procedure MyMessage(var MsgData: TWmCopyData); message WM_COPYDATA;
  public
    m_xRunServerList: TIOCPManager;
    m_xGameServerList: TGameServerManager;
    procedure InitIOCPServer();
    procedure CltConnect(Sender: TObject; const Connected: Boolean);
    procedure CltOnRead(ClientThread: TClientThread; const Buffer: PChar; const BufLen: UINT);
    procedure CltOnClose(Sender: TObject);
    procedure ClientClosed(UserOBJ: pTUserOBJ; wParam, lParam: DWORD);
    procedure UserEnterEvent(Sender: TUserManager; const UserOBJ: pTUserOBJ); //用户进入事件
    procedure UserLeaveEvent(Sender: TUserManager; const UserOBJ: pTUserOBJ); //用户离开事件
    procedure UserReadBuffer(UserOBJ: TObject; const Buffer: PChar; var BufLen: DWORD; var Succeed: BOOL);
  end;

var
  FrmMain: TFrmMain;

implementation

{$R *.dfm}

uses
  FuncForComm, ClientSession, Grobal2, Misc, ConfigManager, LogManager, HUtil32,
  IPAddrFilter, GeneralConfig, PacketRuleConfig,
  SHSocket;



procedure TFrmMain.OnProgramException(Sender: TObject; E: Exception);
begin
  g_pLogMgr.Add(E.Message);
end;


procedure TFrmMain.FormCreate(Sender: TObject);
var
  nX, nY: Integer;
begin
  {
  asm
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  nop
  end;  }
  g_AppLogPath := ExtractFilePath(Application.ExeName) + 'DataLog\';
  if not DirectoryExists(g_AppLogPath) then
    SysUtils.ForceDirectories(g_AppLogPath);

  InitializeCriticalSection(g_LogLock);

  FIsEmbeddedGameCenter := False;

  Application.OnModalBegin := OnAppModalBegin;
  Application.OnModalEnd := OnAppOnModalEnd;

  Application.OnException := OnProgramException;

  g_ProcMsgThread := TProcMsgThread.Create;

  g_hMainWnd := Self.Handle;
  g_hGameCenterHandle := Str_ToInt(ParamStr(1), 0); //运行到此，g_hGameCenterHandle值是 0
  nX := Str_ToInt(ParamStr(2), -1);
  nY := Str_ToInt(ParamStr(3), -1);
  if (nX >= 0) or (nY >= 0) then
  begin
    Left := nX;
    Top := nY;
  end;

  g_boNetComGate := ParamStr(4) = '1';

  g_pLogMgr := TLogMgr.Create(MemoLog.Handle);
  g_pConfig := TConfigMgr.Create(_STR_CONFIG_FILE);

  SendGameCenterMsg(SG_FORMHANDLE, IntToStr(g_hMainWnd));


  GridSocketInfo.Cells[0, 0] := _STR_GRID_IP;
  GridSocketInfo.Cells[1, 0] := _STR_GRID_PORT;
  GridSocketInfo.Cells[2, 0] := _STR_GRID_CONNECT_STATUS;
  GridSocketInfo.Cells[3, 0] := _STR_GRID_ONLINE_USER;

  LoadBlockIPList();
  LoadBlockIPAreaList();

  SetTimer(g_hMainWnd, _IDM_TIMER_STARTSERVICE, 100, Pointer(@OnTimerProc));
end;

procedure TFrmMain.FormDestroy(Sender: TObject);
begin
  g_pConfig.Free;
  g_pLogMgr.Free;
  g_ProcMsgThread.Free;

  DeleteCriticalSection(g_LogLock);
end;


procedure TFrmMain.FormCloseQuery(Sender: TObject; var CanClose: Boolean);
begin
  if g_fCanClose then
    Exit;
  if Application.MessageBox('是否确认退出服务器？', '确认信息', MB_YESNO + MB_ICONQUESTION) = IDYES then
  begin
    if g_fServiceStarted then
    begin
      StopService();
      Sleep(20);
      CanClose := True;
    end;
  end
  else
    CanClose := False;
end;

procedure TFrmMain.GridSocketInfoDrawCell(Sender: TObject; ACol, ARow: Integer; Rect: TRect; State: TGridDrawState);
begin
  with Sender as TStringGrid do
  begin
    Canvas.FillRect(Rect);
    DrawText(Canvas.Handle,
      PChar(Cells[ACol, ARow]),
      Length(Cells[ACol, ARow]),
      Rect, // 包含文字的矩形
      DT_CENTER or // 水平居中 DT_RIGHT 水平居右
      DT_SINGLELINE or // 不折行
      DT_VCENTER); // 垂直居中
  end;
end;

procedure TFrmMain.MemoLogDblClick(Sender: TObject);
begin
  if MemoLog.Lines.Count = 0 then
    Exit;
  if Application.MessageBox('是否确认清除显示的日志信息？', '确认信息', MB_OKCANCEL + MB_ICONQUESTION) <> IDOK then
    Exit;
  MemoLog.Clear;
end;

procedure TFrmMain.MENU_CONTROL_CLEAELOGClick(Sender: TObject);
begin
  MemoLogDblClick(nil);
end;

procedure TFrmMain.MENU_CONTROL_EXITClick(Sender: TObject);
begin
  Close;
end;

procedure TFrmMain.MENU_CONTROL_RELOADCONFIGClick(Sender: TObject);
begin
  g_pConfig.LoadConfig();
  if g_pConfig.m_szTitle <> '' then begin
      Self.Caption := Format('SelGate - [%s]', [g_pConfig.m_szTitle]);
  end else begin
      Self.Caption := 'SelGate';
  end;

  LoadBlockIPList();
  LoadBlockIPAreaList();

  g_pLogMgr.Add('重新加载配置完成...');
end;

procedure TFrmMain.MENU_CONTROL_STARTClick(Sender: TObject);
begin
  StartService();
end;

procedure TFrmMain.MENU_CONTROL_STOPClick(Sender: TObject);
begin
  StopService();
end;

procedure TFrmMain.MENU_OPTION_GENERALClick(Sender: TObject);
begin
  with frmGeneralConfig, g_pConfig do
  begin
    frmGeneralConfig.Top := Self.Top + 20;
    frmGeneralConfig.Left := Self.Left;
    m_Showed := False;
    EditTitle.Text := m_szTitle;
    TrackBarLogLevel.Position := m_nShowLogLevel;
    speGateCount.Value := m_nGateCount;
    speGateIdx.Value := 1;
    EditServerIPaddr.Text := g_pConfig.m_xGameGateList[1].sServerAdress;
    EditServerPort.Text := IntToStr(g_pConfig.m_xGameGateList[1].nServerPort);
    EditGatePort.Text := IntToStr(g_pConfig.m_xGameGateList[1].nGatePort);
    m_Showed := True;
    ShowModal;
  end;
end;

procedure TFrmMain.MENU_OPTION_IPFILTERClick(Sender: TObject);
var
  I, n: Integer;
  UserOBJ: TSessionObj;
begin
  with frmPacketRule, g_pConfig do
  begin
    m_ShowOpen := False;
    frmPacketRule.Top := Self.Top + 20;
    frmPacketRule.Left := Self.Left;

    ListBoxActiveList.Clear;
    if g_fServiceStarted then
    begin
      for n := 0 to USER_ARRAY_COUNT - 1 do
      begin
        UserOBJ := g_UserList[n];
        if (UserOBJ <> nil) and (UserOBJ.m_tLastGameSvr <> nil) and (UserOBJ.m_tLastGameSvr.Active) and not UserOBJ.m_fKickFlag then
          ListBoxActiveList.Items.AddObject(Trim(UserOBJ.m_pUserOBJ.pszIPAddr), TObject(UserOBJ));
      end;
    end;
    ListBoxTempList.Clear;
    for I := 0 to g_TempBlockIPList.Count - 1 do
      ListBoxTempList.Items.AddObject(g_TempBlockIPList.Strings[I], g_TempBlockIPList.Objects[I]);

    ListBoxBlockList.Clear;
    for I := 0 to g_BlockIPList.Count - 1 do
      ListBoxBlockList.Items.AddObject(g_BlockIPList.Strings[I], g_BlockIPList.Objects[I]);

    ListBoxIPAreaFilter.Clear;
    for I := 0 to g_BlockIPAreaList.Count - 1 do
      ListBoxIPAreaFilter.Items.AddObject(g_BlockIPAreaList.Strings[I], g_BlockIPAreaList.Objects[I]);

    etMaxConnectOfIP.Value := m_nMaxConnectOfIP;
    etClientTimeOutTime.Value := m_nClientTimeOutTime div 1000;
    case m_tBlockIPMethod of
      mDisconnect: rdDisConnect.Checked := True;
      mBlock: rdAddTempList.Checked := True;
      mBlockList: rdAddBlockList.Checked := True;
    end;
    etNomClientPacketSize.Value := m_nNomClientPacketSize;
    etMaxClientMsgCount.Value := m_nMaxClientPacketCount;
    cbKickOverPacketSize.Checked := m_fKickOverPacketSize;
    cbCheckNullConnect.Checked := m_fCheckNullSession;
    cbDefenceCC.Checked := m_fDefenceCCPacket;

    pcProcessPack.ActivePageIndex := 0;
    m_ShowOpen := True;
    btnSave.Enabled := False;
    ShowModal;
  end;
end;

procedure TFrmMain.MENU_VIEW_HELP_ABOUTClick(Sender: TObject);
begin
  g_pLogMgr.Add('更新日期: 2025/04/28');
  g_pLogMgr.Add('角色网关');
end;

procedure TFrmMain.InitIOCPServer;
var
  I: Integer;
  ClientThread: TClientThread;
begin
  with m_xRunServerList.IOCPServer do
  begin
    UserManager.OnUserEnter := Self.UserEnterEvent;
    UserManager.OnUserLeave := Self.UserLeaveEvent;
    Reader.OnReadEvent := Self.UserReadBuffer;
  end;

  if g_pConfig.m_nGateCount > 0 then
  begin
    for I := 1 to g_pConfig.m_nGateCount do
    begin
      ClientThread := m_xGameServerList.InitGameServer(g_pConfig.m_xGameGateList[I].nServerPort, g_pConfig.m_xGameGateList[I].sServerAdress);
      ClientThread.m_nPos := I;
      ClientThread.OnConnectEvent := CltConnect;
      ClientThread.OnReadEvent := CltOnRead;
      ClientThread.OnCloseEvent := CltOnClose;
      m_xRunServerList.InitServer(g_pConfig.m_xGameGateList[I].nGatePort, ClientThread);
      ClientThread.Active := True;
    end;
    m_xRunServerList.IOCPServer.StartService;
  end
  else
  begin
    MessageBox(0, '监听端口数量不能少于1个', '错误', MB_OK);
  end;
end;

procedure TFrmMain.CltConnect(Sender: TObject; const Connected: Boolean);
var
  S: string;
begin
  S := '%S' + TClientThread(Sender).ServerIP + '$';
  TClientThread(Sender).SendBuffer(@S[1], Length(S));
end;

procedure TFrmMain.CltOnRead(ClientThread: TClientThread; const Buffer: PChar; const BufLen: UINT);
var
  nPos, nSock, nBufLen: Integer;
  pTRBuf: PChar;
  pTREnd: PChar;
  pTRBuffer: PChar;
  UserOBJ: TSessionObj;
label
  LOOP;
begin
  //ExecLen := 0;
  pTRBuf := Buffer;
  pTREnd := Buffer + BufLen;

  LOOP:
  while DWORD(pTRBuf) < DWORD(pTREnd) do
  begin
    if pTRBuf^ <> '%' then
    begin
      Inc(pTRBuf);
      Continue;
    end;
    if DWORD(pTREnd) - DWORD(pTRBuf) <= 2 then
      Break;
    pTRBuffer := pTRBuf + 1;
    while DWORD(pTRBuffer) < DWORD(pTREnd) do
    begin
      if pTRBuffer^ <> '$' then
      begin
        Inc(pTRBuffer);
        Continue;
      end;
      Inc(pTRBuf, 1);
      nBufLen := UINT(pTRBuffer) - UINT(pTRBuf);

      if nBufLen >= 2 then
      begin
        with g_ProcMsgThread do
        begin
          if pTRBuf^ = '+' then
          begin
            if (pTRBuf + 1)^ = '-' then
            begin
              nSock := Misc.AnsiStrToVal(pTRBuf + 2, nPos);
              UserOBJ := GetSession(nSock);
              if UserOBJ <> nil then
                SHSocket.FreeSocket(UserOBJ.m_pUserOBJ._SendObj.Socket);
            end
            else
            begin
              ClientThread.m_dwKeepAliveTick := GetTickCount();
              //ClientThread.m_fKeepAliveTimcOut := False;
            end;
          end
          else
          begin
            nSock := Misc.AnsiStrToVal(pTRBuf, nPos);
            UserOBJ := GetSession(nSock);
            if (UserOBJ <> nil) and (UserOBJ.m_tLastGameSvr = ClientThread) then
            begin
              UserOBJ.ProcessSvrData(ClientThread, Integer(pTRBuf + nPos + 1), nBufLen - nPos - 1);
            end;
          end;
        end;
      end;
      Inc(pTRBuf, nBufLen + 1);

      //ExecLen := DWORD(pTRBuf) - DWORD(Buffer);

      goto LOOP;
    end;
    Break;
  end;
  //if ExecLen > 0 then
  //  ClientThread.ReaderDone(ExecLen)
  //else
  ClientThread.ReaderDone(DWORD(pTRBuf) - DWORD(Buffer));
end;


procedure TFrmMain.CltOnClose(Sender: TObject);
begin
  if g_pLogMgr.CheckLevel(5) then
    g_pLogMgr.Add('服务器连接已关闭: ' + TClientThread(Sender).ServerIP + ':' + IntToStr(TClientThread(Sender).ServerPort));
  with m_xRunServerList.IOCPServer do
    Writer.BroadUserMsg(0, Integer(Sender), ClientClosed);
end;

procedure TFrmMain.ClientClosed(UserOBJ: pTUserOBJ; wParam, lParam: DWORD);
var
  ClientObj: TSessionObj;
begin
  ClientObj := TSessionObj(UserOBJ.tData);
  if ClientObj = nil then
    Exit;
  with ClientObj do
  begin
    if m_tLastGameSvr = TClientThread(lParam) then
      m_tIOCPSender.DeleteSocket(m_pOverlapSend);
  end;
end;

procedure TFrmMain.UserEnterEvent(Sender: TUserManager; const UserOBJ: pTUserOBJ);
var
  CSession: TSessionObj;
begin
  CSession := TSessionObj(UserOBJ.tData);
  if CSession <> nil then
  begin
    CSession.ReCreate();
  end
  else
  begin
    CSession := TSessionObj.Create;
    UserOBJ.tData := CSession;
  end;
  with CSession do
  begin
    m_pUserOBJ := UserOBJ;
    m_tIOCPSender := UserOBJ.Writer;
    m_pOverlapSend := UserOBJ._SendObj;
    m_dwSessionID := UserOBJ.dwUID;
    InterlockedExchange(Integer(m_tLastGameSvr), Integer(UserOBJ.GameServ));
    InterlockedExchange(Integer(g_UserList[m_dwSessionID]), Integer(CSession));
    UserEnter();
  end;
end;

procedure TFrmMain.UserLeaveEvent(Sender: TUserManager; const UserOBJ: pTUserOBJ);
begin
  if UserOBJ.tData <> nil then
  begin
    with TSessionObj(UserOBJ.tData) do
    begin
      UserLeave();
      InterlockedExchange(Integer(m_tLastGameSvr), 0);
    end;
  end;
end;

procedure TFrmMain.UserReadBuffer(UserOBJ: TObject; const Buffer: PChar; var BufLen: DWORD; var Succeed: BOOL);
label
  LOOP;
var
  fCDPacket: Boolean;
  iLen, nMsgCount: Integer;
  pTRBuf, pTRBuffer: PByte;
  dwEnd: DWORD;
begin
  fCDPacket := False;
  //ExecLen := 0;
  nMsgCount := 0;
  pTRBuf := PByte(Buffer);
  dwEnd := DWORD(Buffer) + BufLen;

  LOOP:
  while DWORD(pTRBuf) < dwEnd do
  begin
    if (pTRBuf^ <> $23) then
    begin //# &
      Inc(pTRBuf);
      Continue;
    end;
    if (dwEnd - DWORD(pTRBuf)) <= 2 then
      Break;
    pTRBuffer := Pointer(Integer(pTRBuf) + 2);
    while DWORD(pTRBuffer) < dwEnd do
    begin
      if pTRBuffer^ <> $21 then
      begin
        Inc(pTRBuffer);
        Continue;
      end;
      Inc(nMsgCount);
      Inc(pTRBuf, 2);
      iLen := UINT(pTRBuffer) - UINT(pTRBuf);
      TSessionObj(UserOBJ).ProcessCltData(Integer(pTRBuf), iLen, Succeed, fCDPacket);

      if not Succeed then
        Break;

      Inc(pTRBuf, iLen + 1);

      //ExecLen := DWORD(pTRBuf) - DWORD(Buffer);

      if nMsgCount >= g_pConfig.m_nMaxClientPacketCount then
      begin
        KickUser(TSessionObj(UserOBJ).m_pUserOBJ.nIPAddr);
        Break;
      end;
      goto LOOP;
    end;
    Break;
  end;
  if Succeed then
    BufLen := DWORD(pTRBuf) - DWORD(Buffer); //ExecLen;
end;

procedure TFrmMain.MyMessage(var MsgData: TWmCopyData);
var
  sData: string;
  wIdent: Word;
begin
  wIdent := HiWord(MsgData.From);
  sData := StrPas(MsgData.CopyDataStruct^.lpData);
  case wIdent of
    GS_QUIT:
      begin
        if g_fServiceStarted then
        begin
          StopService();
          g_fCanClose := True;
          Close();
        end;
      end;
  end;
end;

procedure TFrmMain.WMSetParentWindow(var Message: TMessage);
begin
  if Message.WParam = 1 then begin
    Windows.SetParent(Handle, Message.LParam);
    SetWindowLong(Application.Handle, GWL_EXSTYLE, GetWindowLong(Application.Handle, GWL_EXSTYLE) or WS_EX_TOOLWINDOW);
    FIsEmbeddedGameCenter := True;
    Self.Hide;
    Self.Show;
  end else begin
    Windows.SetParent(Handle, 0);
    SetWindowLong(Application.Handle, GWL_EXSTYLE, GetWindowLong(Application.Handle, GWL_EXSTYLE) and (not WS_EX_TOOLWINDOW));
    FIsEmbeddedGameCenter := False;
  end;
end;

procedure TFrmMain.WMSysCommand(var Message: TWMSysCommand);
const
  GWW_HWNDPARENT = -8;
begin
  if FIsEmbeddedGameCenter and (Message.CmdType and $FFF0 = SC_MINIMIZE) then
  begin
    DefaultHandler(Message);
  end
  else
    inherited;
end;

procedure TFrmMain.OnAppModalBegin(Sender: TObject);
begin
  if FIsEmbeddedGameCenter then
  begin
    Enabled := False;
  end;
end;

procedure TFrmMain.OnAppOnModalEnd(Sender: TObject);
begin
  if FIsEmbeddedGameCenter then
  begin
    Enabled := True;
  end;
end;

procedure TFrmMain.mmiLogDataClick(Sender: TObject);
begin
  if g_LoginData = 0 then
  begin
    InterlockedExchange(g_LoginData, 1);
    mmiLogData.Checked := True;
  end
  else
  begin
    InterlockedExchange(g_LoginData, 0);
    mmiLogData.Checked := False;
  end;
end;

end.

