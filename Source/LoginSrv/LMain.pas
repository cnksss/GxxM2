unit LMain;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ExtCtrls, AccountDB, JSocket, SyncObjs, Grids, Buttons, IniFiles,
  MudUtil, Parse, Menus, Grobal2, LSShare, MD5Util, winSock, SqliteAccountDB,
  MySqlAccountDB, uFrmBatchEditAccountInfo, VerifyCodeUtils, DateUtils;

const
  WM_SET_PARENT_WINDOW = WM_USER + 123;
  IntMultiplication: array[1..17] of Integer = (7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2);

type
  TFrmMain = class(TForm)
    GSocket: TServerSocket;
    ExecTimer: TTimer;
    Panel1: TPanel;
    Memo1: TMemo;
    Timer1: TTimer;
    StartTimer: TTimer;
    WebLogTimer: TTimer;
    BtnDump: TSpeedButton;
    LogTimer: TTimer;
    MonitorGrid: TStringGrid;
    Panel2: TPanel;
    Label1: TLabel;
    SpeedButton1: TSpeedButton;
    LbMasCount: TLabel;
    CkLogin: TCheckBox;
    CbViewLog: TCheckBox;
    BtnView: TSpeedButton;
    CountLogTimer: TTimer;
    BtnShowServerUsers: TSpeedButton;
    MonitorTimer: TTimer;
    SpeedButton2: TSpeedButton;
    MainMenu: TMainMenu;
    MENU_CONTROL: TMenuItem;
    MENU_CONTROL_EXIT: TMenuItem;
    MENU_VIEW: TMenuItem;
    MENU_OPTION: TMenuItem;
    MENU_TOOLS: TMenuItem;
    MENU_HELP: TMenuItem;
    MENU_HELP_VERSION: TMenuItem;
    MENU_OPTION_GENERAL: TMenuItem;
    MENU_OPTION_ROUTE: TMenuItem;
    MENU_VIEW_SESSION: TMenuItem;
    ServerSocketControl: TServerSocket;
    N1: TMenuItem;
    test1: TMenuItem;
    procedure FormCreate(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure ExecTimerTimer(Sender: TObject);
    procedure Memo1DblClick(Sender: TObject);
    procedure Timer1Timer(Sender: TObject);
    procedure StartTimerTimer(Sender: TObject);
    procedure SpeedButton1Click(Sender: TObject);
    procedure BtnViewClick(Sender: TObject);
    procedure CountLogTimerTimer(Sender: TObject);
    procedure BtnShowServerUsersClick(Sender: TObject);
    procedure MonitorTimerTimer(Sender: TObject);
    procedure SpeedButton2Click(Sender: TObject);
    procedure FormCloseQuery(Sender: TObject; var CanClose: Boolean);
    procedure GSocketClientConnect(Sender: TObject; Socket: TCustomWinSocket);
    procedure GSocketClientDisconnect(Sender: TObject; Socket: TCustomWinSocket);
    procedure GSocketClientError(Sender: TObject; Socket: TCustomWinSocket; ErrorEvent: TErrorEvent; var ErrorCode: Integer);
    procedure GSocketClientRead(Sender: TObject; Socket: TCustomWinSocket);
    procedure Panel2DblClick(Sender: TObject);
    procedure CbViewLogClick(Sender: TObject);
    procedure MENU_CONTROL_EXITClick(Sender: TObject);
    procedure MENU_OPTION_ROUTEClick(Sender: TObject);
    procedure MENU_VIEW_SESSIONClick(Sender: TObject);
    procedure MENU_HELP_VERSIONClick(Sender: TObject);
    procedure MENU_OPTION_GENERALClick(Sender: TObject);
    procedure ServerSocketControlAccept(Sender: TObject; Socket: TCustomWinSocket);
    procedure ServerSocketControlClientConnect(Sender: TObject; Socket: TCustomWinSocket);
    procedure ServerSocketControlClientDisconnect(Sender: TObject; Socket: TCustomWinSocket);
    procedure ServerSocketControlClientError(Sender: TObject; Socket: TCustomWinSocket; ErrorEvent: TErrorEvent; var ErrorCode: Integer);
    procedure ServerSocketControlClientRead(Sender: TObject; Socket: TCustomWinSocket);
    procedure N1Click(Sender: TObject);
    procedure test1Click(Sender: TObject);
  private
    FIsEmbeddedGameCenter: Boolean;
    ParseList: TThreadParseList; //0x350

    procedure GameCenterGetUserAccount(sData: string);
    procedure GameCenterChangeAccountInfo(sData: string);
    procedure OpenRouteConfig();
    procedure SendControlMsg(Socket: TCustomWinSocket; dwCmd: LongWord; Buf: PChar; BufLen: Integer);
    { Private declarations }

    procedure WMSysCommand(var Message: TWMSysCommand); message WM_SYSCOMMAND;
    procedure WMSetParentWindow(var Message: TMessage); message WM_SET_PARENT_WINDOW;
    procedure OnAppModalBegin(Sender: TObject);
    procedure OnAppOnModalEnd(Sender: TObject);
  public
    { Public declarations }
    procedure MyMessage(var MsgData: TWmCopyData); message WM_COPYDATA;
    procedure OnProgramException(Sender: TObject; E: Exception);
  end;

procedure StartService();

procedure StopService();

procedure InitializeConfig();

procedure UnInitializeConfig();

procedure LoadConfig();

procedure LoadAddrTable();

procedure GenServerNameList();

procedure WriteLogMsg(sType: string; AccountInfo: TAccountInfo);

procedure SaveContLogMsg(sLogMsg: string);

procedure SendGateMsg(Socket: TCustomWinSocket; sSockIndex, sMsg: string);

procedure SendGateKickMsg(Socket: TCustomWinSocket; sSockIndex: string);

procedure SendKeepAlivePacket(Socket: TCustomWinSocket);

procedure SessionAdd(sAccount, sIPaddr: string; nSessionID: Integer; boPayCost, bo11: Boolean); overload;

procedure SessionAdd(sAccount, sIPaddr, sServerName: string; nSessionID: Integer; boPayCost, bo11: Boolean); overload;

procedure SessionDel(nSessionID: Integer);

procedure SessionKick(sLoginID: string);

procedure SessionUpdate(nSessionID: Integer; sServerName: string; boPayCost: Boolean);

procedure SessionClearKick();

function IsPayMent(sIPaddr, sAccount: string): Boolean;

procedure SessionClearNoPayMent();

function IsLogin(sLoginID: string): Boolean; overload;

function IsLogin(nSessionID: Integer): Boolean; overload;

function GetServerListInfo(account: string = ''): string;

procedure GetSelGateInfo(sServerName, sIPaddr: string; var sSelGateIP: string; var nSelGatePort: Integer);

function KickUser(UserInfo: pTUserInfo; IsLock: Boolean = True): Boolean;

procedure CloseUser(sServerName, sAccount: string; nSessionID: Integer);

procedure AccountCreate(UserInfo: pTUserInfo; sData: string);

procedure AccountChangePassword(UserInfo: pTUserInfo; sData: string);

procedure AccountChangePhone(UserInfo: pTUserInfo; sData: string);

procedure AccountGetPassWordBack(UserInfo: pTUserInfo; sData: string);

procedure AccountGetPassWordBack_Phone(UserInfo: pTUserInfo; sData: string);

procedure AccountCheckProtocol(UserInfo: pTUserInfo; nDate: Integer);

procedure AccountLogin(UserInfo: pTUserInfo; sData: string; boLegend: Boolean = True);

procedure AccountLoginPhone(UserInfo: pTUserInfo; sData: string);

procedure AccountCreateNew(UserInfo: pTUserInfo; sData: string);

procedure AccountRealName(UserInfo: pTUserInfo; sData: string);

procedure AccountQuickLogin(UserInfo: pTUserInfo; sData: string);

procedure AccountBindPhone(UserInfo: pTUserInfo; sData: string);

function IsRightID(sID: string{; var sSex: string; var age: Integer}): Boolean;

procedure AccountSelectServer(UserInfo: pTUserInfo; sData: string);

procedure AccountUpdateUserInfo(UserInfo: pTUserInfo; sData: string);

procedure AccountGetBackPassword(UserInfo: pTUserInfo; sData: string);

procedure AccountSetL2Password(IsFirstSet: Boolean; UserInfo: pTUserInfo; sData: string);

procedure AccountCheckL2Password(UserInfo: pTUserInfo; sData: string);

procedure GetPhoneVerification(UserInfo: pTUserInfo; sData: string);

procedure ReceiveSendUser(sSockIndex: string; GateInfo: pTGateInfo; sData: string);

procedure ReceiveOpenUser(sSockIndex: string; sIPaddr: string; GateInfo: pTGateInfo);

procedure ReceiveCloseUser(sSockIndex: string; GateInfo: pTGateInfo);

procedure SendRandomCode(UserInfo: pTUserInfo; RandCodeType: TRandCodeType);

procedure ProcessUserMsg(UserInfo: pTUserInfo; sMsg: string);

procedure DecodeGateData(GateInfo: pTGateInfo);

procedure DecodeUserData(UserInfo: pTUserInfo);

procedure ProcessGate();

procedure LoadAccountCostList(QuickList: TQuickList);

procedure LoadIPaddrCostList(QuickList: TQuickList);

procedure UserIsRealName(return: string; return1: string; return2: string; return3: string; return4: string; userinfo: Integer);

procedure QuickLogin(return: string; return1: string; return2: string; return3: string; return4: string; userinfo: Integer);

var
  FrmMain: TFrmMain;

implementation

uses
  MasSock, FrmFindId, HUtil32, EDcode, GateSet, FAccountView, GrobalSession,
  Common, SDK, EncryptUnit, BasicSet, MonSoc;

{$R *.DFM}

procedure TFrmMain.OpenRouteConfig;
begin
  FrmGateSetting := TFrmGateSetting.Create(nil);
  FrmGateSetting.Open;
  FrmGateSetting.Free;
end;
{
procedure TFrmMain.OpenRouteConfig;
var
  Config  :pTConfig;
begin
  Config:=@g_Config;
  if FrmGateSetting.Open then begin
    LoadAddrTable(Config);
  end;
end;
}

procedure TFrmMain.MENU_OPTION_ROUTEClick(Sender: TObject);
begin
  OpenRouteConfig();
end;

procedure TFrmMain.MENU_VIEW_SESSIONClick(Sender: TObject);
begin
  frmGrobalSession := TfrmGrobalSession.Create(nil);
  frmGrobalSession.Open;
  frmGrobalSession.Free;
end;

procedure TFrmMain.CbViewLogClick(Sender: TObject);
begin
  g_Config.boShowDetailMsg := CbViewLog.Checked;
end;

procedure TFrmMain.GSocketClientConnect(Sender: TObject; Socket: TCustomWinSocket);
var
  I: Integer;
  GateInfo: pTGateInfo;
  sRemoteAddr: string;
  boAllowed: Boolean;
begin
  Socket.nIndex := -1;
  if not ExecTimer.Enabled then
  begin
    Socket.Close;
    Exit;
  end;

  sRemoteAddr := Socket.RemoteAddress;
  boAllowed := False;
  for I := Low(g_ServerAddr) to g_ServerAddrCount - 1 do
  begin
    if SameText(sRemoteAddr, g_ServerAddr[I]) then
    begin
      boAllowed := True;
      break;
    end;
  end;

  if not boAllowed then
  begin
    if g_Config.boShowBlockIPLog then
    begin
      MainOutMessage('拒绝未授权IP连接服务器：' + sRemoteAddr);
    end;

    Socket.Close;
    Exit;
  end;

  EnterCriticalSection(g_Config.GateCriticalSection);
  try
    for I := 0 to Length(g_Config.GateList) - 1 do
    begin
      if g_Config.GateList[I].Socket = nil then
      begin
        Socket.nIndex := I;
        GateInfo := @g_Config.GateList[I];
        GateInfo.Socket := Socket;
        GateInfo.sRemoteAddress := Socket.RemoteAddress;
        GateInfo.sIPaddr := GetGatePublicAddr(Socket.RemoteAddress);
        GateInfo.sReceiveMsg := '';
        GateInfo.dwKeepAliveTick := GetTickCount();
        MainOutMessage('登录网关连接成功: ' + Socket.RemoteAddress);
        break;
      end;
    end;
  finally
    LeaveCriticalSection(g_Config.GateCriticalSection);
  end;

  if Socket.nIndex < 0 then
  begin
    MainOutMessage('Kick: ' + Socket.RemoteAddress + ' 当前连接已满');
    Socket.Close;
  end;

end;

procedure TFrmMain.GSocketClientDisconnect(Sender: TObject; Socket: TCustomWinSocket);
var
  Index: Integer;
  I: Integer;
  GateInfo: pTGateInfo;
  UserInfo: pTUserInfo;
begin
  Index := Socket.nIndex;
  if (Index >= 0) and (Index < Length(g_Config.GateList)) then
  begin
    EnterCriticalSection(g_Config.GateCriticalSection);
    try
      GateInfo := @g_Config.GateList[Index];
      GateInfo.Socket := nil;
      GateInfo.sReceiveMsg := '';
      GateInfo.sIPaddr := '';
      for I := 0 to GateInfo.UserList.Count - 1 do
      begin
        UserInfo := GateInfo.UserList.Items[I];
        if g_Config.boShowDetailMsg then
          MainOutMessage('Close: ' + UserInfo.sUserIPaddr);
        Dispose(UserInfo);
      end;
      GateInfo.UserList.Clear;
      MainOutMessage('登录网关连接断开: ' + Socket.RemoteAddress);
    finally
      LeaveCriticalSection(g_Config.GateCriticalSection);
    end;
  end;
end;

procedure TFrmMain.GSocketClientError(Sender: TObject; Socket: TCustomWinSocket; ErrorEvent: TErrorEvent; var ErrorCode: Integer);
begin
  ErrorCode := 0;
  Socket.Close;
end;

procedure TFrmMain.GSocketClientRead(Sender: TObject; Socket: TCustomWinSocket);
var
  Index: Integer;
  GateInfo: pTGateInfo;
begin
  Index := Socket.nIndex;
  if (Index >= 0) and (Index < Length(g_Config.GateList)) then
  begin
    EnterCriticalSection(g_Config.GateCriticalSection);
    try
      GateInfo := @g_Config.GateList[Index];
      //if GateInfo.Socket = Socket then begin
      GateInfo.sReceiveMsg := GateInfo.sReceiveMsg + Socket.ReceiveText;
      //end;
    finally
      LeaveCriticalSection(g_Config.GateCriticalSection);
    end;
  end;
end;

procedure LoadAddrTable();
var
  LoadList: TStringList;
  sFileName: string;
  i: Integer;
  nRouteIdx: Integer;
  nSelGateIdx: Integer;
  sLineText, sTitle, sServerName, sGate, sRemote, sPublic, sGatePort: string;
begin
  sFileName := '.\!addrtable.txt';
  LoadList := TStringList.Create;
  if FileExists(sFileName) then
  begin
    LoadList.LoadFromFile(sFileName);
    nRouteIdx := 0;
    for i := 0 to LoadList.Count - 1 do
    begin
      sLineText := LoadList.Strings[i];
      if (sLineText <> '') and (sLineText[1] <> ';') then
      begin
        sLineText := GetValidStr3(sLineText, sServerName, [' ']);
        sLineText := GetValidStr3(sLineText, sTitle, [' ']);
        sLineText := GetValidStr3(sLineText, sRemote, [' ']);
        sLineText := GetValidStr3(sLineText, sPublic, [' ']);
        sLineText := Trim(sLineText);
        if (sTitle <> '') and (sRemote <> '') and (sPublic <> '') and (nRouteIdx < 60) then
        begin
          g_Config.GateRoute[nRouteIdx].sServerName := sServerName;
          g_Config.GateRoute[nRouteIdx].sTitle := sTitle;
          g_Config.GateRoute[nRouteIdx].sRemoteAddr := sRemote;
          g_Config.GateRoute[nRouteIdx].sPublicAddr := sPublic;
          nSelGateIdx := 0;
          while (sLineText <> '') do
          begin
            if nSelGateIdx > 9 then
              break;
            sLineText := GetValidStr3(sLineText, sGate, [' ']);
            if sGate <> '' then
            begin
              if sGate[1] = '*' then
              begin
                sGate := Copy(sGate, 2, length(sGate) - 1);
                g_Config.GateRoute[nRouteIdx].Gate[nSelGateIdx].boEnable := False;
              end
              else
              begin
                g_Config.GateRoute[nRouteIdx].Gate[nSelGateIdx].boEnable := True;
              end;
              sGatePort := GetValidStr3(sGate, sGate, [':']);
              g_Config.GateRoute[nRouteIdx].Gate[nSelGateIdx].sIPaddr := sGate;
              g_Config.GateRoute[nRouteIdx].Gate[nSelGateIdx].nPort := StrToIntDef(sGatePort, 0);
              g_Config.GateRoute[nRouteIdx].nSelIdx := 0;
              Inc(nSelGateIdx);
            end;
            sLineText := Trim(sLineText);
          end;
          Inc(nRouteIdx);
        end;
      end;
    end;
    g_Config.nRouteCount := nRouteIdx;
  end;
  LoadList.Free;
  GenServerNameList();
end;

procedure TFrmMain.OnProgramException(Sender: TObject; E: Exception);
begin
  MainOutMessage(E.Message);
end;

procedure TFrmMain.FormCreate(Sender: TObject);
var
  I, nX, nY: Integer;
  AppPath: string;
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
  end;
  }
  AppPath := ExtractFilePath(Application.ExeName);

  FIsEmbeddedGameCenter := False;
  Randomize;
  g_dwGameCenterHandle := StrToIntDef(ParamStr(1), 0);
  nX := StrToIntDef(ParamStr(2), -1);
  nY := StrToIntDef(ParamStr(3), -1);
  if (nX >= 0) or (nY >= 0) then
  begin
    Left := nX;
    Top := nY;
  end;
  g_Config.boRemoteClose := False;

  SendGameCenterMsg(SG_FORMHANDLE, IntToStr(Self.Handle));

  Application.OnException := OnProgramException;
  Application.OnModalBegin := OnAppModalBegin;
  Application.OnModalEnd := OnAppOnModalEnd;

  CS_DB := TCriticalSection.Create;

  nSessionIdx := 1;
  n47328C := 1;
  //nMemoHeigh := Memo1.Height;
  nMemoHeigh := 39;
  FillChar(g_Config.GateList, SizeOf(g_Config.GateList), #0);

  for I := 0 to Length(g_Config.GateList) - 1 do
  begin
    g_Config.GateList[I].UserList := TList.Create;
  end;

  g_Config.SessionList := TGList.Create;
  g_Config.ServerNameList := TStringList.Create;
  g_Config.AccountCostList := TQuickList.Create;
  g_Config.IPaddrCostList := TQuickList.Create;
  ParseList := TThreadParseList.Create(True);
  LoadAddrTable();
  MonitorGrid.Cells[0, 0] := '服务器名';
  MonitorGrid.Cells[1, 0] := '用户数';
  MonitorGrid.Cells[2, 0] := '状态';
  MonitorGrid.Cells[3, 0] := '服务器名';
  MonitorGrid.Cells[4, 0] := '用户数';
  MonitorGrid.Cells[5, 0] := '状态';

  g_ControlIPFile := AppPath + 'ControlIP.txt';
  g_ControlIPList := TSafeHashStringList.Create;
  if FileExists(g_ControlIPFile) then
  begin
    g_ControlIPList.LoadFromFile(g_ControlIPFile);
  end;

  g_DisablePasswordFile := AppPath + 'DisablePassword.txt';
  g_DisablePasswordList := TStringList.Create;
  if FileExists(g_DisablePasswordFile) then
  begin
    g_DisablePasswordList.LoadFromFile(g_DisablePasswordFile);
  end;
end;

procedure TFrmMain.FormDestroy(Sender: TObject);
var
  I, II: integer;
  GateInfo: pTGateInfo;
  UserInfo: pTUserInfo;
begin
  StopService();
  if g_AccountDB <> nil then
  begin
    g_AccountDB.Free;
    g_AccountDB := nil;
  end;

  for I := 0 to Length(g_Config.GateList) - 1 do
  begin
    GateInfo := @g_Config.GateList[I];
    for II := 0 to GateInfo.UserList.Count - 1 do
    begin
      UserInfo := GateInfo.UserList.Items[II];
      Dispose(UserInfo);
    end;
    GateInfo.UserList.Free;
  end;
  g_Config.SessionList.Free;
  g_Config.ServerNameList.Free;
  CS_DB.Free;

  // 去内存泄露 chongchong 2013-08-28
  g_Config.AccountCostList.Free;
  g_Config.IPaddrCostList.Free;
  ParseList.Free;

  g_ControlIPList.Free;
  g_DisablePasswordList.Free;
end;

procedure TFrmMain.ExecTimerTimer(Sender: TObject);
begin
  if bo470D20 and not g_boDataDBReady then
    exit;
  bo470D20 := True;
  try
    ProcessGate();
  finally
    bo470D20 := False;
  end;
end;

procedure TFrmMain.Memo1DblClick(Sender: TObject);
begin
  OpenRouteConfig();
end;

procedure TFrmMain.Timer1Timer(Sender: TObject);
var
  I: Integer;
  UserSession: PControlSessionInfo;
begin
  Label1.Caption := IntToStr(g_Config.dwProcessGateTime);
  CkLogin.Checked := GSocket.Socket.Connected;
  CkLogin.Caption := '连接 (' + IntToStr(GSocket.Socket.ActiveConnections) + ')';
  LbMasCount.Caption := IntToStr(nOnlineCountMin) + '/' + IntToStr(nOnlineCountMax);
  if Memo1.Lines.Count > 2000 then
    Memo1.Clear;
  EnterCriticalSection(g_OutMessageCS);
  try
    for I := 0 to g_MainMsgList.Count - 1 do
    begin
      Memo1.Lines.Add(g_MainMsgList.Strings[I]);
    end;
    g_MainMsgList.Clear;
  finally
    LeaveCriticalSection(g_OutMessageCS);
  end;

  SessionClearKick();
  SessionClearNoPayMent();

  for I := 0 to Length(g_ControlSessionArray) - 1 do
  begin
    UserSession := @g_ControlSessionArray[I];
    if (UserSession.Socket = nil) then
      Continue;

    if (tick_diff(UserSession.ConnectTick, GetTickCount) >= 5000) and (not UserSession.IsPasswordOK) then begin
      UserSession.ConnectTick := GetTickCount;
      UserSession.Socket.Close;
    //end else if UserSession.DelayClose and (tick_diff(UserSession.DelayCloseTick, GetTickCount) >= 0) then begin
    end else if UserSession.DelayClose and (tick_diff(UserSession.DelayCloseTick, GetTickCount) > 6000) then begin //HZQ
      {$MESSAGE HINT '这个判断或者判断的顺序很不合理，先改成大于6000'} //HZQ 
      UserSession.ConnectTick := GetTickCount;
      UserSession.Socket.Close;
    end else if tick_diff(UserSession.RecvTick, GetTickCount) >= 90000 then begin     // 90秒钟没有收到数据包，连接超时，断开
      UserSession.ConnectTick := GetTickCount;
      UserSession.Socket.Close;
    end else if tick_diff(UserSession.RecvTick, GetTickCount) >= 30000 then begin     // 30秒钟没有收到数据包，发一个包过去
      if tick_diff(UserSession.SendHeartbeatTick, GetTickCount) >= 30000 then begin
        UserSession.SendHeartbeatTick := GetTickCount;
        SendControlMsg(UserSession.Socket, SMSG_HEARTBEAT, nil, 0);
      end;
    end;
  end;

  if g_AccountDB <> nil then
    g_AccountDB.Run;
end;

procedure TFrmMain.StartTimerTimer(Sender: TObject);
var
  DBConfig: TDBServerConfig;
begin
  StartTimer.Enabled := False;
  StartService();

  //Caption := Caption + '-' + g_Config.sServerName + '  [' + ExtractFilePath(Application.ExeName) + ']';
  if g_Config.sServerName <> '' then begin
      Caption := Format('LoginSrv - [%s]', [g_Config.sServerName]);
  end else begin
      Caption := 'LoginSrv';
  end;

  SendGameCenterMsg(SG_STARTNOW, '正在启动登录服务器...');
  Memo1.Lines.Add('1) 正在启动服务器...');

  if g_Config.nDataSaveDBType = 0 then begin
    g_AccountDB := TSqliteAccountDB.Create(g_Config.sIdDir + 'Account.DB');
  end else begin
//    DBConfig.DBServer := '118.195.196.12'; //'10.206.0.38'; //g_Config.sDataSaveDBServer;
//    DBConfig.DBPort := 3306; //g_Config.wDataSaveDBPort;
//    DBConfig.DBUser := 'root'; //g_Config.sDataSaveDBUser;
//    DBConfig.DBPassword := 'B8vRj9QN^08xj!9vh!O7';
//    DBConfig.DataBase := 'gmserver';

    DBConfig.DBServer := g_Config.sDataSaveDBServer;
    DBConfig.DBPort := g_Config.wDataSaveDBPort;
    DBConfig.DBUser := g_Config.sDataSaveDBUser;
    DBConfig.DBPassword := g_Config.sDataSaveDBPassword;
    DBConfig.DataBase := g_Config.sDataSaveDataBase;
    g_AccountDB := TMySqlAccountDB.Create(DBConfig);
  end;

  g_AccountDB.Init;

  ParseList.Resume;
  Memo1.Lines.Add('2) 正在等待服务器连接...');
  FrmMonSoc.StartService;
  FrmMasSoc.StartService;
  while (True) do
  begin
    Application.ProcessMessages;
    if Application.Terminated then
      exit;
    if FrmMasSoc.CheckReadyServers then
      break;
    Sleep(1);
  end;
  GSocket.Active := False;
  GSocket.Address := g_Config.sGateAddr;
  GSocket.Port := g_Config.nGatePort;
  GSocket.Active := True;
  Memo1.Lines.Add('3) 服务器启动完成...');

  if g_Config.nControlPort > 0 then
  begin
    ServerSocketControl.Active := False;
    ServerSocketControl.Address := '0.0.0.0';
    ServerSocketControl.Port := g_Config.nControlPort;
    ServerSocketControl.Active := True;

    FillChar(g_ControlSessionArray, SizeOf(g_ControlSessionArray), 0);

    Memo1.Lines.Add('4) 远程控制端口[' + IntToStr(g_Config.nControlPort) + ']监视成功...');
  end;

  ExecTimer.Enabled := True;
  SendGameCenterMsg(SG_STARTOK, '登录服务器启动完成...');
end;

procedure TFrmMain.SpeedButton1Click(Sender: TObject);
begin
  FrmFindUserId.Show;
end;

procedure TFrmMain.MENU_CONTROL_EXITClick(Sender: TObject);
begin
  Close;
end;

procedure TFrmMain.FormCloseQuery(Sender: TObject; var CanClose: Boolean);
resourcestring
  sExitMsg = '是否确认停止登录服务器 ?';
  sExitTitle = '确认信息';
begin
  if g_Config.boRemoteClose then
    exit;
  if MessageBox(Handle, PChar(sExitMsg), PChar(sExitTitle), MB_YESNO + MB_ICONQUESTION) = mrYes then
    CanClose := True
  else
    CanClose := False;
end;

procedure TFrmMain.BtnViewClick(Sender: TObject);
begin
  try
    CS_DB.Enter;
    FrmAccountView.ListBox1.Items := g_Config.AccountCostList;
    FrmAccountView.ListBox2.Items := g_Config.IPaddrCostList;
  finally
    CS_DB.Leave;
  end;
  FrmAccountView.ShowModal;
end;

procedure TFrmMain.CountLogTimerTimer(Sender: TObject);
var
  sLogMsg: string;
resourcestring
  sFormatMsg = '%d/%d';
begin
  sLogMsg := format(sFormatMsg, [nOnlineCountMin, nOnlineCountMax]);
  SaveContLogMsg(sLogMsg);
  nOnlineCountMax := 0;
end;

procedure TFrmMain.BtnShowServerUsersClick(Sender: TObject);
var
  I: Integer;
begin
  for I := 0 to nUserLimit - 1 do
  begin
    MainOutMessage(UserLimit[I].sServerName + ' ' + IntToStr(UserLimit[I].nLimitCountMin) + '/' + IntToStr(UserLimit[I].nLimitCountMax));
  end;
end;

procedure TFrmMain.MonitorTimerTimer(Sender: TObject);
var
  I: Integer;
  nCol: Integer;
  sServerName: string;
  ServerList: TList;
  MsgServer: pTMsgServerInfo;
begin
  try
    ServerList := FrmMasSoc.m_ServerList;
    if (ServerList.Count div 2) < 2 then
    begin
      MonitorGrid.RowCount := 2;
      MonitorGrid.Cells[0, 1] := '';
      MonitorGrid.Cells[1, 1] := '';
      MonitorGrid.Cells[2, 1] := '';
      MonitorGrid.Cells[3, 1] := '';
      MonitorGrid.Cells[4, 1] := '';
      MonitorGrid.Cells[5, 1] := '';
    end
    else
    begin
      MonitorGrid.RowCount := ((ServerList.Count div 2) + 1) + (ServerList.Count mod 2);
    end;
                                                                                                    //0046ED54
    for I := 0 to ServerList.Count - 1 do
    begin
      nCol := (I mod 2) * 3;
      MsgServer := ServerList.Items[I];
      sServerName := MsgServer.sServerName;
      if sServerName <> '' then
      begin
        if MsgServer.nServerIndex = 99 then
          MonitorGrid.Cells[nCol, (I div 2 + 1)] := sServerName + ' [DB]'
        else
          MonitorGrid.Cells[nCol, (I div 2 + 1)] := sServerName + ' ' + IntToStr(MsgServer.nServerIndex);

        MonitorGrid.Cells[nCol + 1, (I div 2 + 1)] := IntToStr(MsgServer.nOnlineCount);

        if (GetTickCount - MsgServer.dwKeepAliveTick) < 30000 then
          MonitorGrid.Cells[nCol + 2, (I div 2 + 1)] := '正常'
        else
          MonitorGrid.Cells[nCol + 2, (I div 2 + 1)] := '超时';
      end
      else
      begin //0046EEF2
        MonitorGrid.Cells[nCol, (I div 2 + 1)] := '-';
        MonitorGrid.Cells[nCol + 1, (I div 2 + 1)] := '-';
        MonitorGrid.Cells[nCol + 2, (I div 2 + 1)] := '-';
      end;
    end;
  except
    MainOutMessage('TFrmMain.MonitorTimerTimer');
  end;
end;

procedure TFrmMain.SpeedButton2Click(Sender: TObject);
begin
  {if Memo1.Height = nMemoHeigh then Memo1.Height := nMemoHeigh * 2
  else Memo1.Height := nMemoHeigh;}
end;

function IsPayMent(sIPaddr, sAccount: string): Boolean;
begin
  Result := False;
  try
    CS_DB.Enter;
    if (g_Config.AccountCostList.GetIndex(sAccount) >= 0) or (g_Config.IPaddrCostList.GetIndex(sIPaddr) >= 0) then
      Result := True;
  finally
    CS_DB.Leave;
  end;
end;

procedure CloseUser(sServerName, sAccount: string; nSessionID: Integer);
var
  ConnInfo: pTConnInfo;
  I: Integer;
  boFind: Boolean;
begin
  boFind := False;
  //MainOutMessage('CloseUser 1 ' + sAccount + ' ' + IntToStr(nSessionID));
  g_Config.SessionList.Lock;
  try
    for I := g_Config.SessionList.Count - 1 downto 0 do
    begin
      ConnInfo := g_Config.SessionList.Items[I];
      if (ConnInfo.sAccount = sAccount) and (ConnInfo.nSessionID = nSessionID) and (not ConnInfo.boKicked) then
      begin
        boFind := True;
        FrmMasSoc.SendServerMsg(SS_CLOSESESSION, ConnInfo.sServerName, ConnInfo.sAccount + '/' + IntToStr(ConnInfo.nSessionID));
        //MainOutMessage('CloseUser 2 ' + ConnInfo.sAccount + ' ' + IntToStr(ConnInfo.nSessionID));
        g_Config.SessionList.Delete(I);
        Dispose(ConnInfo);
        break;
      end;
    end;
  finally
    g_Config.SessionList.UnLock;
  end;
  if not boFind then
  begin
    FrmMasSoc.SendServerMsg(SS_CLOSESESSION, sServerName, sAccount + '/' + IntToStr(nSessionID));
  end;
end;

procedure ProcessGate();
var
  I: Integer;
  II: Integer;
  GateInfo: pTGateInfo;
  UserInfo: pTUserInfo;
begin
  EnterCriticalSection(g_Config.GateCriticalSection);
  try
    g_Config.dwProcessGateTick := GetTickCount();

    for I := 0 to Length(g_Config.GateList) - 1 do
    begin
      GateInfo := @g_Config.GateList[I];
      if (GateInfo.Socket <> nil) then
      begin
        if (GateInfo.sReceiveMsg <> '') then
        begin
          DecodeGateData(GateInfo);
          g_Config.sGateIPaddr := GateInfo.sIPaddr;
          II := 0;
          while (True) do
          begin
            if GateInfo.UserList.Count <= II then
              break;
            UserInfo := GateInfo.UserList.Items[II];
            if UserInfo.sReceiveMsg <> '' then
              DecodeUserData(UserInfo);

            Inc(II);
          end;
        end;

        II := 0;
        while (True) do
        begin
          if GateInfo.UserList.Count <= II then
            break;
          UserInfo := GateInfo.UserList.Items[II];

          if UserInfo.boDealyClose and (GetTickCount >= UserInfo.dwDelayCloseTick) then
          begin
            KickUser(UserInfo, False);
          end;

          Inc(II);
        end;
      end;
    end;
    if g_Config.dwProcessGateTime < g_Config.dwProcessGateTick then
      g_Config.dwProcessGateTime := GetTickCount - g_Config.dwProcessGateTick;
    if g_Config.dwProcessGateTime > 100 then
      Dec(g_Config.dwProcessGateTime, 100);
  finally
    LeaveCriticalSection(g_Config.GateCriticalSection);
  end;
end;

procedure DecodeGateData(GateInfo: pTGateInfo);
var
  nCount: Integer;
  sMsg: string;
  sSockIndex: string;
  sData: string;
  Code: Char;
begin
  try
    nCount := 0;
    while (True) do
    begin
      if Pos('$', GateInfo.sReceiveMsg) <= 0 then
        break;
      GateInfo.sReceiveMsg := ArrestStringEx(GateInfo.sReceiveMsg, '%', '$', sMsg);
      if sMsg <> '' then
      begin
        ;
        Code := sMsg[1];
        sMsg := Copy(sMsg, 2, Length(sMsg) - 1);
        case Code of
          '-':
            begin
              SendKeepAlivePacket(GateInfo.Socket);
              GateInfo.dwKeepAliveTick := GetTickCount();
            end;
          'A':
            begin
              sData := GetValidStr3(sMsg, sSockIndex, ['/']);
              ReceiveSendUser(sSockIndex, GateInfo, sData);
            end;
          'O':
            begin
              sData := GetValidStr3(sMsg, sSockIndex, ['/']);
              ReceiveOpenUser(sSockIndex, sData, GateInfo);
            end;
          'X':
            begin
              sSockIndex := sMsg;
              ReceiveCloseUser(sSockIndex, GateInfo);
            end;
          'S':
            begin
              if CompareLStr(sMsg, '127.0.0.', Length('127.0.0.')) then
              begin
                GateInfo.sIPaddr := GetGatePublicAddr(sMsg);
                if GateInfo.sRemoteAddress <> sMsg then
                  MainOutMessage('登录网关连接地址: ' + sMsg);
              end;
              //MainOutMessage('TFrmMain.DecodeGateData:'+sMsg+' '+GateInfo.sIPaddr);
            end;
        end;
      end
      else
      begin //0046AD85
        if nCount >= 1 then
          GateInfo.sReceiveMsg := '';
        Inc(nCount);
      end;
    end;
  except
    MainOutMessage('[Exception] TFrmMain.DecodeGateData');
  end;
end;

procedure SendKeepAlivePacket(Socket: TCustomWinSocket);
begin
  if Socket.Connected then
    Socket.SendText('%++$');
end;

procedure ReceiveCloseUser(sSockIndex: string; GateInfo: pTGateInfo);
var
  UserInfo: pTUserInfo;
  I: Integer;
resourcestring
  sCloseMsg = 'Close: %s';
begin
  for I := 0 to GateInfo.UserList.Count - 1 do
  begin
    UserInfo := GateInfo.UserList.Items[I];
    if UserInfo.sSockIndex = sSockIndex then
    begin
      if g_Config.boShowDetailMsg then
        MainOutMessage(format(sCloseMsg, [UserInfo.sUserIPaddr]));
      if not UserInfo.boSelServer then
        SessionDel(UserInfo.nSessionID);
      GateInfo.UserList.Delete(I);
      Dispose(UserInfo);
      break;
    end;
  end;
end;

procedure ReceiveOpenUser(sSockIndex, sIPaddr: string; GateInfo: pTGateInfo);
var
  UserInfo: pTUserInfo;
  I: Integer;
  sGateIPaddr: string;
  sUserIPaddr: string;
  DefMsg: TDefaultMessage;
  RandCodeType: TRandCodeType;
resourcestring
  sOpenMsg = 'Open: %s/%s';
begin
  sGateIPaddr := GetValidStr3(sIPaddr, sUserIPaddr, ['/']);
  try
    for I := 0 to GateInfo.UserList.Count - 1 do
    begin
      UserInfo := GateInfo.UserList.Items[I];
      if UserInfo.sSockIndex = sSockIndex then
      begin
        UserInfo.dwClientTick := GetTickCount() - 1000;
        UserInfo.sUserIPaddr := sUserIPaddr;
        UserInfo.sGateIPaddr := sGateIPaddr;
        UserInfo.sAccount := '';
        UserInfo.nSessionID := 0;
        UserInfo.sReceiveMsg := '';

        for RandCodeType := Low(TRandCodeType) to High(TRandCodeType) do
        begin
          UserInfo.sRandomCode[RandCodeType] := '';
          UserInfo.nRandomCodeErrorMaxCount[RandCodeType] := 0;
          UserInfo.nRandomCodeRefreshMaxCount[RandCodeType] := 0;
          UserInfo.boRandomCodeOK[RandCodeType] := False;
        end;

        UserInfo.sL2Password := '';
        UserInfo.boL2PasswordOK := False;
        UserInfo.nL2ErrorCount := 0;
        UserInfo.sLoginMAC := '';
        UserInfo.boDealyClose := False;
        UserInfo.dwDelayCloseTick := GetTickCount;
        UserInfo.dwTime5C := GetTickCount();
        UserInfo.sVerificationCode := '';
        UserInfo.dwLastVerificationCodeTick := 0;

        UserInfo.sReLoginUser := '';
        UserInfo.sReLoginPassword := '';
        Exit;
      end;
    end;
    New(UserInfo);
    UserInfo.dwClientTick := GetTickCount() - 1000;
    UserInfo.sAccount := '';
    UserInfo.sUserIPaddr := sUserIPaddr;
    UserInfo.sGateIPaddr := sGateIPaddr;
    UserInfo.sSockIndex := sSockIndex;
    UserInfo.nVersionDate := 0;
    UserInfo.boCertificationOK := False;
    UserInfo.nSessionID := 0;
    UserInfo.Socket := GateInfo.Socket;
    UserInfo.sReceiveMsg := '';
    UserInfo.dwTime5C := GetTickCount();
    UserInfo.Gate := GateInfo;

    for RandCodeType := Low(TRandCodeType) to High(TRandCodeType) do
    begin
      UserInfo.sRandomCode[RandCodeType] := '';
      UserInfo.nRandomCodeErrorMaxCount[RandCodeType] := 0;
      UserInfo.nRandomCodeRefreshMaxCount[RandCodeType] := 0;
      UserInfo.boRandomCodeOK[RandCodeType] := False;
    end;

    UserInfo.sL2Password := '';
    UserInfo.boL2PasswordOK := False;
    UserInfo.nL2ErrorCount := 0;
    UserInfo.sLoginMAC := '';
    UserInfo.boDealyClose := False;
    UserInfo.dwDelayCloseTick := GetTickCount;

    UserInfo.sVerificationCode := '';
    UserInfo.dwLastVerificationCodeTick := 0;

    UserInfo.sReLoginUser := '';
    UserInfo.sReLoginPassword := '';

    GateInfo.UserList.Add(UserInfo);
    if g_Config.boShowDetailMsg then
      MainOutMessage(format(sOpenMsg, [sUserIPaddr, sGateIPaddr]));

    if g_Config.boNewLoginDlg or g_Config.boEnabledL2Password or g_Config.boRandomCode[rctLogin] or g_Config.boRandomCode[rctRegister] or g_Config.boRandomCode[rctPwdGetback] or g_Config.boRandomCode[rctPwdChange] then
    begin
      DefMsg := MakeDefaultMsg(SM_OPENL2PASSWORD, Integer(g_Config.boNewLoginDlg), Integer(g_Config.boEnabledL2Password), MakeWord(Byte(g_Config.boRandomCode[rctLogin]), Byte(g_Config.boRandomCode[rctRegister])), MakeWord(Byte(g_Config.boRandomCode[rctPwdGetback]), Byte(g_Config.boRandomCode[rctPwdChange])));

      SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
    end;

    {
    if g_Config.boRandomCode then
    begin
      SendRandomCode(UserInfo);
    end;
    }
  except
    MainOutMessage('TFrmMain.ReceiveOpenUser');
  end;
end;

procedure ReceiveSendUser(sSockIndex: string; GateInfo: pTGateInfo; sData: string);
var
  UserInfo: pTUserInfo;
  I: integer;
begin
  try
    for I := 0 to GateInfo.UserList.Count - 1 do
    begin
      UserInfo := GateInfo.UserList.Items[I];
      if UserInfo.sSockIndex = sSockIndex then
      begin
        if Length(UserInfo.sReceiveMsg) < 4069 then
        begin
          UserInfo.sReceiveMsg := UserInfo.sReceiveMsg + sData;
        end;
        Break;
      end;
    end;
  except
    MainOutMessage('TFrmMain.ReceiveSendUser');
  end;
end;

procedure SessionClearKick();
var
  I: Integer;
  ConnInfo: pTConnInfo;
begin
  g_Config.SessionList.Lock;
  try
    for I := g_Config.SessionList.Count - 1 downto 0 do
    begin
      ConnInfo := g_Config.SessionList.Items[I];
      if ConnInfo.boKicked and ((GetTickCount - ConnInfo.dwKickTick) > 2 * 1000) then
      begin
        //MainOutMessage('SessionClearKick ' + ConnInfo.sAccount + ' ' + IntToStr(ConnInfo.nSessionID));
        g_Config.SessionList.Delete(I);
        Dispose(ConnInfo);
      end;
    end;
  finally
    g_Config.SessionList.UnLock;
  end;
end;

procedure DecodeUserData(UserInfo: pTUserInfo);
var
  sMsg: string;
  nCount: Integer;
begin
  nCount := 0;
  try
    //if UserInfo = nil then nErrCode:=1;
    while (True) do
    begin
      if Pos('!', UserInfo.sReceiveMsg) <= 0 then
        break;
      UserInfo.sReceiveMsg := ArrestStringEx(UserInfo.sReceiveMsg, '#', '!', sMsg);
      if sMsg <> '' then
      begin
        ;
        if Length(sMsg) >= DEF_BLOCK_SIZE + 1 then
        begin
          sMsg := Copy(sMsg, 2, Length(sMsg) - 1);
          ProcessUserMsg(UserInfo, sMsg);
        end;
      end
      else
      begin
        if nCount >= 1 then
          UserInfo.sReceiveMsg := '';
        Inc(nCount);
      end;
      if UserInfo.sReceiveMsg = '' then
        break;
    end;
  except
    MainOutMessage('[Exception] TFrmMain.DecodeUserData ');
  end;
end;

procedure SessionDel(nSessionID: Integer);
var
  ConnInfo: pTConnInfo;
  I: Integer;
begin
  g_Config.SessionList.Lock;
  try
    for I := 0 to g_Config.SessionList.Count - 1 do
    begin
      ConnInfo := g_Config.SessionList.Items[I];
      if (ConnInfo.nSessionID = nSessionID) and (not ConnInfo.boKicked) then
      begin
        //MainOutMessage('SessionDel ' + ConnInfo.sAccount + ' ' + IntToStr(nSessionID));
        g_Config.SessionList.Delete(I);
        Dispose(ConnInfo);
        break;
      end;
    end;
  finally
    g_Config.SessionList.UnLock;
  end;
end;

function CheckStringValid(S: WideString): Boolean;
var
  I: Integer;
  WC: WideChar;
begin
  Result := True;
  for I := 1 to Length(S) do
  begin
    WC := S[I];
    if (WC in [WideChar('/'), WideChar('@'), WideChar('$'), WideChar('<'), WideChar('>')]) then
    begin
      Result := False;
      Exit;
    end;
  end;
end;

function CheckStringValid2(S: WideString): Boolean;
var
  I: Integer;
  WC: WideChar;
begin
  Result := True;
  for I := 1 to Length(S) do
  begin
    WC := S[I];
    if (WC in [WideChar('/'), WideChar('$'), WideChar('<'), WideChar('>')]) then
    begin
      Result := False;
      Exit;
    end;
  end;
end;

procedure GetRandomCode(var StrShow, StrRandCode: string; RandCodeType: TRandCodeType);
var
  Num, Num1, Num2: Integer;
begin
  Randomize();
  Num := Random(3);

  // 登录外的其他不用字母
  if (RandCodeType <> rctLogin) and (Num = 0) then
    Num := 1;

  if Num = 0 then
  begin
    StrRandCode := Chr(65 + Random(27)) + Chr(65 + Random(27)) + Chr(65 + Random(27)) + Chr(65 + Random(27));

    StrShow := StrRandCode;
  end
  else if Num = 1 then
  begin
    Num1 := Random(40);
    Num2 := Random(40);
    StrRandCode := IntToStr(Num1 + Num2);

    StrShow := IntToStr(Num1) + '+' + IntToStr(Num2) + '=';
  end
  else
  begin
    Num1 := Random(90);
    Num2 := Random(90);

    if Num1 >= Num2 then
    begin
      StrRandCode := IntToStr(Num1 - Num2);
      StrShow := IntToStr(Num1) + '-' + IntToStr(Num2) + '=';
    end
    else
    begin
      StrRandCode := IntToStr(Num2 - Num1);
      StrShow := IntToStr(Num2) + '-' + IntToStr(Num1) + '=';
    end;
  end;
end;

procedure SendRandomCode(UserInfo: pTUserInfo; RandCodeType: TRandCodeType);
var
  ErrCode: Integer;
  DefMsg: TDefaultMessage;
  Bitmap: TBitmap;
  MS: TMemoryStream;
  sTempVerifyCode, sSendText: string;
begin
  Randomize();
  GetRandomCode(sTempVerifyCode, UserInfo.sRandomCode[RandCodeType], RandCodeType);

  ErrCode := 1;
  try
    Bitmap := TBitmap.Create;
    try
      if RandCodeType = rctLogin then
        Bitmap.PixelFormat := pf8bit
      else
        Bitmap.PixelFormat := pf24bit;

      ErrCode := 2;
      Bitmap.Canvas.Lock;
      try
        ErrCode := 3;

        if RandCodeType = rctLogin then
        begin
          Bitmap.Width := 170;
          Bitmap.Height := 70;
          Bitmap.Canvas.Font.Size := 32;

          //MakeVerifyCode(sTempVerifyCode, Bitmap, Bitmap.Canvas.Font, -(8 + Random(6)), False, -3);
          MakeVerifyCode(sTempVerifyCode, Bitmap, Bitmap.Canvas.Font, -(g_Config.btLoginWaveValue + Random(g_Config.btLoginWaveValue)), False, -3);
        end
        else
        begin
          Bitmap.Width := 110;
          Bitmap.Height := 30;
          Bitmap.Canvas.Font.Size := 22;

          //MakeVerifyCode(sTempVerifyCode, Bitmap, Bitmap.Canvas.Font, -(3 + Random(4)), False, -1);
          MakeVerifyCode(sTempVerifyCode, Bitmap, Bitmap.Canvas.Font, -(g_Config.btOtherWaveValue + Random(g_Config.btOtherWaveValue)), False, -1);
        end;

        ErrCode := 5;
        MS := TMemoryStream.Create;
        try
          ErrCode := 6;
          Bitmap.SaveToStream(MS);

          UserInfo.boRandomCodeOK[RandCodeType] := False;

          ErrCode := 7;
          sSendText := zLibEncodeBuffer(MS.Memory, MS.Size);
          DefMsg := MakeDefaultMsg(SM_RANDOMCODE, Length(sSendText), LoWord(MS.Size), HiWord(MS.Size), Byte(RandCodeType));
          sSendText := EncodeMessage(DefMsg) + sSendText;

          ErrCode := 8;
          SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, sSendText);
        finally
          MS.Free;
        end;
      finally
        Bitmap.Canvas.Unlock;
      end;
    finally
      Bitmap.Free;
    end;
  except
    on E: Exception do
      MainOutMessage('SendRandomCode Error, Code = ' + IntToStr(ErrCode) + ',' + E.Message);
  end;
end;

procedure QuickLogin(return: string; return1: string; return2: string; return3: string; return4: string; userinfo: Integer);

  function CreateString(): string;
  var
    SourceStr, str: string;
    i: integer;
  begin
    SourceStr := 'abcdefghijklmnopqrstuvwxyz0123456789';
    randomize;
    for i := 1 to 10 do
      str := str + SourceStr[Random(Length(SourceStr)) + 1];
    Result := str;
  end;

var
  User: pTUserInfo;
  AccountInfo: TAccountInfo;
  DefMsg: TDefaultMessage;
  isCreate: Boolean;
  UID, CID: string;
  sCrateAccount, sCreatePassWord, sServerName: string;
  nCode: Integer;
  nIDCost: Integer;
  nIPCost: Integer;
  nIDCostIndex: Integer;
  nIPCostIndex: Integer;
begin
  User := pTUserInfo(userinfo);
  if User = nil then Exit; //HZQ
  
  UID := return2;
  CID := return3;
  MainOutMessage(return);
  MainOutMessage(return1);
  MainOutMessage(return2);
  MainOutMessage(return3);
  MainOutMessage(return4);

  DefMsg := MakeDefaultMsg(SM_QUICKLOGIN, 0, 0, 0, 0);
  SendGateMsg(User.Socket, User.sSockIndex, EncodeMessage(DefMsg) + EncodeString(return));
          
  if return = '0' then
    exit;


//  if Pos('0', return) > 0 then //由于Delphi对JSON支持不好 故直接判断字符串里有没有0  0是成功  -1是失败
//  begin
//    if g_AccountDB.GetAccount(User.sReLoginUser, AccountInfo) then
//    begin
//      AccountInfo.UserName := return1;
//      AccountInfo.IDCard := return2;
//      g_AccountDB.UpdateAccount(AccountInfo, ufAllField);
//
//      WriteLogMsg('upg', AccountInfo);
//      AccountLogin(User, EnCodeString(User.sReLoginUser + '/' + User.sReLoginPassword));
//    end;
//  end
//  else
//  begin
//    DefMsg := MakeDefaultMsg(SM_REALNAME, -1, 0, 0, 0);
//    SendGateMsg(User.Socket, User.sSockIndex, EncodeMessage(DefMsg));
//  end;
  isCreate := False;
  if (not g_AccountDB.GetAccountByQuick(UID, CID, AccountInfo)) or (AccountInfo.IsDisable) then begin
    sCrateAccount := CreateString();
    sCreatePassWord := CreateString();
      //MainOutMessage('没有找到' + sPhone + '自动创建帐号：' + sCrateAccount + '自动创建密码：' + sCreatePassWord);
    while not isCreate do begin
      if not g_AccountDB.CheckAccountExists(sCrateAccount) then begin
        FillChar(AccountInfo, SizeOf(AccountInfo), #0);
        AccountInfo.IsDisable := False;
        AccountInfo.CreateDate := Date2MyDate(Now);
        AccountInfo.AccountName := sCrateAccount;
        AccountInfo.Password := sCreatePassWord;
//          AccountInfo.MobilePhone := sPhone;
        AccountInfo.BirthDay := '1999/9/9';
        AccountInfo.UID := UID;
        AccountInfo.CID := CID;
  //        AccountInfo.UserName := '123123';
    //      AccountInfo.UserName := UserEntry.sUserName;
    //      AccountInfo.IDCard := UserEntry.sSSNo;
    //      AccountInfo.Phone := UserEntry.sPhone;
    //      AccountInfo.Questions1 := UserEntry.sQuiz;
    //      AccountInfo.Answers1 := UserEntry.sAnswer;
    //      AccountInfo.Mail := UserEntry.sEMail;
    //
    //      AccountInfo.BirthDay := UserAddEntry.sBirthDay;
    //      AccountInfo.Questions2 := UserAddEntry.sQuiz2;
    //      AccountInfo.Answers2 := UserAddEntry.sAnswer2;
    //      AccountInfo.MobilePhone := UserAddEntry.sMobilePhone;

        if g_AccountDB.AddAccount(AccountInfo) then begin
            //nErrCode := 1;
          isCreate := True;
          WriteLogMsg('new', AccountInfo);
        end;
      end
    end;
  end;

//  ReadUserIP := AccountInfo.LoginIP;
//  ReadUserMac := AccountInfo.LoginMac;
    {自动解除锁定账号}
  if (g_Config.boUnLockAccount) and (AccountInfo.ErrorCount >= 5)
  and ((GetTickCount - AccountInfo.LastActionTick) >= Cardinal(g_Config.dwUnLockAccountTime * 60 * 1000)) then begin
    AccountInfo.ErrorCount := 0;
    AccountInfo.LastActionTick := GetTickCount - 70000;
  end;

  if (AccountInfo.ErrorCount < 5) or ((GetTickCount - AccountInfo.LastActionTick) > 60000) then begin
//    if AccountInfo.MobilePhone = sPhone then begin
//      if (AccountInfo.UserName = '') or (AccountInfo.Questions2 = '') then begin
//        UserEntry.sAccount := AccountInfo.AccountName;
//        UserEntry.sPassword := AccountInfo.Password;
//        UserEntry.sUserName := AccountInfo.UserName;
//        UserEntry.sSSNo := AccountInfo.IDCard;
//        UserEntry.sPhone := AccountInfo.Phone;
//        UserEntry.sQuiz := AccountInfo.Questions1;
//        UserEntry.sAnswer := AccountInfo.Answers1;
//        UserEntry.sEMail := AccountInfo.Mail;
//
//        UserAdd.sQuiz2 := AccountInfo.Questions2;
//        UserAdd.sAnswer2 := AccountInfo.Answers2;
//        UserAdd.sBirthDay := AccountInfo.BirthDay;
//        UserAdd.sMemo := AccountInfo.Memo;
//
//        AccountInfo.ErrorCount := 0;
//        boNeedUpdate := True;
//      end;

        AccountInfo.LoginDate := Date2MyDate(Now());
//      IsUpdated := True;
//      nCode := 1;
//    end else begin
//      Inc(AccountInfo.ErrorCount);
//      AccountInfo.LastActionTick := GetTickCount();
//
//      IsUpdated := True;
//      nCode := -1;
//
////      if g_Config.boRandomCode[rctLogin] then
////      begin
////        UserInfo.dwClientTick := GetTickCount();
////        UserInfo.nRandomCodeRefreshMaxCount[rctLogin] := 0;
////        SendRandomCode(UserInfo, rctLogin);
////      end;
//    end;
//  end
//  else
//  begin
//    nCode := -2;
//    AccountInfo.LastActionTick := GetTickCount();
//    IsUpdated := True;
  end;

  {$MESSAGE HINT '这里的逻辑有很明显的错误，nCode是不确定的值'}
  //看代码意思, 尝试补全代码 HZQ
  if AccountInfo.MobilePhone = string(User.sLoginPhone) then begin
     nCode := 0;
  end else begin
     nCode := -1;
  end;

  if (nCode = 1) and IsLogin(AccountInfo.AccountName) then begin
    SessionKick(AccountInfo.AccountName);
    nCode := -3;
  end;

  if nCode = 0 then begin
    User.sAccount := AccountInfo.AccountName;
////    实名认证检测 By 一支笔 at:2022-01-06 12:47:08
//
//    if not IsRightID(AccountInfo.IDCard) then
//    begin
//      User.sReLoginUser := AccountInfo.AccountName;
//      User.sReLoginPassword := AccountInfo.Password;
//      DefMsg := MakeDefaultMsg(SM_REALNAME, 0, 0, 0, 0);
//      SendGateMsg(User.Socket, User.sSockIndex, EncodeMessage(DefMsg));
//      Exit;
//    end;

//    if (AccountInfo.MobilePhone = '') and g_Config.boNewLoginPhone then
//    begin
//      User.sReLoginUser := AccountInfo.AccountName;
//      User.sReLoginPassword := AccountInfo.Password;
//      DefMsg := MakeDefaultMsg(SM_BINDPHONE, 0, 0, 0, 0);
//      SendGateMsg(User.Socket, User.sSockIndex, EncodeMessage(DefMsg));
//      Exit;
//    end;

    User.nSessionID := GetSessionID();
    User.boSelServer := False;
    try
      CS_DB.Enter;
      nIDCostIndex := g_Config.AccountCostList.GetIndex(User.sAccount);
      nIPCostIndex := g_Config.IPaddrCostList.GetIndex(User.sUserIPaddr);
      nIDCost := 0;
      nIPCost := 0;
        //boPayCost := False;
      if nIDCostIndex >= 0 then
        nIDCost := Integer(g_Config.AccountCostList.Objects[nIDCostIndex]);
      if nIPCostIndex >= 0 then
      begin
        nIPCost := Integer(g_Config.IPaddrCostList.Objects[nIPCostIndex]);
          //boPayCost := True;
      end;
    finally
      CS_DB.Leave;
    end;

    if (nIDCost >= 0) or (nIPCost >= 0) then
      User.boPayCost := True
    else
      User.boPayCost := False;

    User.nIDDay := LoWord(nIDCost);
    User.nIDHour := HiWord(nIDCost);
    User.nIPDay := LoWord(nIPCost);
    User.nIPHour := HiWord(nIPCost);

//    if g_Config.boEnabledL2Password then
//    begin
//        // 设置Mac
//      if (Length(sTemp) > 0) and CheckValidMac(sTemp) then
//      begin
//        userinfo.sLoginMAC := sTemp;
//      end;
//
//      if Length(AccountInfo.L2Password) = 0 then
//      begin
//        DefMsg := MakeDefaultMsg(SM_SETL2PASSWORD, 0, 0, 0, 0);
//
//        SendGateMsg(userinfo.Socket, userinfo.sSockIndex, EncodeMessage(DefMsg));
//
//        Exit;
//      end
//      else
//      begin
//        if g_Config.boAlwaysCheckL2 then
//        begin
//          DefMsg := MakeDefaultMsg(SM_CHECKL2PASSWORD, 0, 0, 0, 0);
//          SendGateMsg(userinfo.Socket, userinfo.sSockIndex, EncodeMessage(DefMsg));
//          Exit;
//        end
//        else
//        begin
//          if (g_Config.boChangedIPCheckL2) then
//          begin
//            if (ReadUserIP <> 0) and (inet_addr(pchar(userinfo.sUserIPaddr)) <> ReadUserIP) then
//            begin
//              DefMsg := MakeDefaultMsg(SM_CHECKL2PASSWORD, 0, 0, 0, 0);
//              SendGateMsg(userinfo.Socket, userinfo.sSockIndex, EncodeMessage(DefMsg));
//              Exit;
//            end;
//          end;
//
//          if (g_Config.boChangedMACCheckL2) then
//          begin
//            if (Length(ReadUserMac) > 0) then
//            begin
//              if not SameText(userinfo.sLoginMAC, ReadUserMac) then
//              begin
//                DefMsg := MakeDefaultMsg(SM_CHECKL2PASSWORD, 0, 0, 0, 0);
//                SendGateMsg(userinfo.Socket, userinfo.sSockIndex, EncodeMessage(DefMsg));
//                Exit;
//              end;
//            end;
//          end;
//        end;
//      end;
//    end;

    AccountInfo.LoginIP := inet_addr(pchar(User.sUserIPaddr));
    if Length(User.sLoginMAC) > 0 then
    begin
      AccountInfo.LoginMac := User.sLoginMAC;
    end;
//    IsUpdated := True;

    User.boL2PasswordOK := True;

    SessionAdd(User.sAccount, User.sUserIPaddr, User.nSessionID, User.boPayCost, False);

    if not User.boPayCost then
    begin
      DefMsg := MakeDefaultMsg(SM_PASSOK_SELECTSERVER, 0, 0, 0, g_Config.ServerNameList.Count);
    end
    else
    begin
      DefMsg := MakeDefaultMsg(SM_PASSOK_SELECTSERVER, nIDCost, Loword(nIPCost), HiWord(nIPCost), g_Config.ServerNameList.Count);
    end;
    sServerName := GetServerListInfo;
    SendGateMsg(User.Socket, User.sSockIndex, EncodeMessage(DefMsg) + EncodeString(sServerName));

      // 选择服务器这里不要输入验证码 2021-03-04 19:09:24
      {
      if g_Config.boRandomCode[rctLogin] then
      begin
        UserInfo.dwClientTick := GetTickCount();
        UserInfo.nRandomCodeRefreshMaxCount[rctLogin] := 0;
        //SendRandomCode(UserInfo, rctLogin);
      end;
      }
//    if isCreate then
//    begin
//      DefMsg := MakeDefaultMsg(SM_PHONELOGIN, nCode, 0, 0, 0);
//      SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg) + EncodeString(AccountInfo.AccountName + '/' + AccountInfo.Password));
//    end;
//    AccountLogin(UserInfo, EncodeString(AccountInfo.AccountName + '/' + AccountInfo.Password));
//    Exit;
  end;
end;

procedure UserIsRealName(return: string; return1: string; return2: string; return3: string; return4: string; userinfo: Integer);
var
  User: pTUserInfo;
  AccountInfo: TAccountInfo;
  DefMsg: TDefaultMessage;
begin
  User := pTUserInfo(userinfo);
  if Pos('0', return) > 0 then //由于Delphi对JSON支持不好 故直接判断字符串里有没有0  0是成功  -1是失败
  begin
    if g_AccountDB.GetAccount(User.sReLoginUser, AccountInfo) then
    begin
      AccountInfo.UserName := return1;
      AccountInfo.IDCard := return2;
      g_AccountDB.UpdateAccount(AccountInfo, ufAllField);

      WriteLogMsg('upg', AccountInfo);
      AccountLogin(User, EnCodeString(User.sReLoginUser + '/' + User.sReLoginPassword));
    end;
  end
  else
  begin
    DefMsg := MakeDefaultMsg(SM_REALNAME, -1, 0, 0, 0);
    SendGateMsg(User.Socket, User.sSockIndex, EncodeMessage(DefMsg));
  end;
end;

procedure ProcessUserMsg(UserInfo: pTUserInfo; sMsg: string);
var
  sDefMsg: string;
  sData: string;
  DefMsg: TDefaultMessage;
  RandCodeType: TRandCodeType;
begin
  try
    sDefMsg := Copy(sMsg, 1, DEF_BLOCK_SIZE);
    sData := Copy(sMsg, DEF_BLOCK_SIZE + 1, Length(sMsg) - DEF_BLOCK_SIZE);
    DefMsg := DecodeMessage(sDefMsg);
    //AddLogMsg('Code: ' + IntToStr(DefMsg.Ident) + ' Msg: ' + sData,0);
    case DefMsg.Ident of
      CM_QUICKLOGIN:
        begin
          AccountQuickLogin(UserInfo, sData);
        end;
      CM_REALNAME:
        begin
          AccountRealName(UserInfo, sData);
        end;
      CM_CREATEACCOUNT:
        begin
          AccountCreateNew(UserInfo, sData);
        end;
      CM_BINDPHONE:
        begin
          AccountBindPhone(UserInfo, sData);
        end;
      CM_GETPASSWORDBACK:
        begin
          AccountGetPasswordBack(UserInfo, sData);
        end;
      CM_GETPASSWORDBACK_PHONE:
        begin
          AccountGetPasswordBack_Phone(UserInfo, sData);
        end;
      CM_CHANGEPHONE:
        begin
          AccountChangePhone(UserInfo, sData);
        end;
      CM_PHONELOGIN:
        begin
          AccountLoginPhone(UserInfo, sData);
        end;
      CM_GETVERIFICATIONCODE:
        begin
          GetPhoneVerification(UserInfo, sData);
        end;
      CM_SELECTSERVER:
        begin           
          if not UserInfo.boSelServer then
          begin
            AccountSelectServer(UserInfo, sData);
          end;
        end;
      CM_PROTOCOL:
        begin
          AccountCheckProtocol(UserInfo, DefMsg.Recog);
        end;
      CM_IDPASSWORD:
        begin
          if g_Config.boRandomCode[rctLogin] and (not UserInfo.boRandomCodeOK[rctLogin]) then
          begin
            MainOutMessage('[非法操作] 验证码未验证 ' + '/' + UserInfo.sUserIPaddr);
            UserInfo.Socket.Close;
          end;

          if UserInfo.sAccount = '' then
          begin
            AccountLogin(UserInfo, sData);
          end
          else
          begin
            KickUser(UserInfo);
          end;

          // CM_CHECKISMYSELFSERVER:
          // 检测是否为我们自己的服务端
          // DefMsg := MakeDefaultMsg(SM_CHECKISMYSELFSERVER, 0, 0, 0, 0);
          // SendUserSocket(UserInfo.sConnID, EncodeMessage(DefMsg));
          DefMsg := MakeDefaultMsg(SM_CHECKISMYSELFSERVER, 0, 0, 0, 0);
          SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
        end;
      CM_SETL2PASSWORD:
        begin
          AccountSetL2Password(DefMsg.Param = 0, UserInfo, sData);
        end;
      CM_CHECKL2PASSWORD:
        begin
          AccountCheckL2Password(UserInfo, sData);
        end;
      CM_ADDNEWUSER:
        begin
          if g_Config.boEnableMakingID then
          begin
            if (GetTickCount - UserInfo.dwClientTick) > 3000 then
            begin
              UserInfo.dwClientTick := GetTickCount();
              AccountCreate(UserInfo, sData);
            end
            else
            begin
              MainOutMessage('[超速操作] 创建帐号 ' + '/' + UserInfo.sUserIPaddr);
            end;
          end
          else
          begin
            DefMsg := MakeDefaultMsg(SM_NEWID_FAIL, -3, 0, 0, 0); // 服务器禁止创建ID账号
            SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
          end;
        end;
      CM_CHANGEPASSWORD:
        begin
          if UserInfo.sAccount = '' then
          begin
            if (GetTickCount - UserInfo.dwClientTick) > 3000 then
            begin
              UserInfo.dwClientTick := GetTickCount();
              AccountChangePassword(UserInfo, sData);
            end
            else
            begin
              MainOutMessage('[超速操作] 修改密码 ' + '/' + UserInfo.sUserIPaddr);
            end;
          end
          else
          begin
            UserInfo.sAccount := '';
          end;
        end;
      CM_UPDATEUSER:
        begin
          if (GetTickCount - UserInfo.dwClientTick) > 3000 then
          begin
            UserInfo.dwClientTick := GetTickCount();
            AccountUpdateUserInfo(UserInfo, sData);
          end
          else
          begin
            MainOutMessage('[超速操作] 更新帐号 ' + '/' + UserInfo.sUserIPaddr);
          end;
        end;
      CM_GETBACKPASSWORD:
        begin
          if g_Config.boEnableGetbackPassword then
          begin
            if (GetTickCount - UserInfo.dwClientTick) > 3000 then
            begin
              UserInfo.dwClientTick := GetTickCount();
              AccountGetBackPassword(UserInfo, sData);
            end
            else
            begin
              MainOutMessage('[超速操作] 找回密码 ' + '/' + UserInfo.sUserIPaddr);
            end;
          end
          else
          begin
            // 禁止找回密码时，发送提示包 piaoyun 2013-08-30
            DefMsg := MakeDefaultMsg(SM_GETBACKPASSWD_FAIL, -9, 0, 0, 0);
            SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
          end;
        end;
      CM_CHECKRANDOMCODE:
        begin
          if {(DefMsg.Param >= Integer(Low(TRandCodeType))) and} (DefMsg.Param <= Integer(High(TRandCodeType))) then begin //HZQ
            RandCodeType := TRandCodeType(DefMsg.Param);

            if (CompareText(UserInfo.sRandomCode[RandCodeType], DecodeString(sData)) = 0) then begin
              UserInfo.boRandomCodeOK[RandCodeType] := True;

              DefMsg := MakeDefaultMsg(SM_RANDOMCODE_RET, 0, 0, 0, 0);
              SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
            end else begin
              Inc(UserInfo.nRandomCodeErrorMaxCount[RandCodeType]);
              if (UserInfo.nRandomCodeErrorMaxCount[RandCodeType] >= g_Config.nRandomCodeErrorMaxCount) then begin //验证码输入错误次数超过
                DefMsg := MakeDefaultMsg(SM_QUERYCHR_FAIL, 0, 0, 0, 0);
                SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));

                // 错误达到最高次数，断开 2020-10-29
                UserInfo.dwDelayCloseTick := GetTickCount + 1000;
                UserInfo.boDealyClose := True;
              end
              else
              begin
                DefMsg := MakeDefaultMsg(SM_RANDOMCODE_RET, 0, 1, 0, 0);
                SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
              end;
            end;
          end;
        end;

      CM_CHANGERANDOMCODE:
        begin
          if {(DefMsg.Param >= Integer(Low(TRandCodeType))) and} (DefMsg.Param <= Integer(High(TRandCodeType))) then begin //HZQ
            RandCodeType := TRandCodeType(DefMsg.Param);

            if g_Config.boRandomCode[RandCodeType] then begin
              if (GetTickCount - UserInfo.dwClientTick) >= 600 then begin
                if UserInfo.nRandomCodeRefreshMaxCount[RandCodeType] <= g_Config.nRandomCodeRefreshMaxCount then begin
                  UserInfo.dwClientTick := GetTickCount();
                  Inc(UserInfo.nRandomCodeRefreshMaxCount[RandCodeType]);
                  SendRandomCode(UserInfo, RandCodeType);
                end else begin
                  DefMsg := MakeDefaultMsg(SM_RANDOMCODE_RET, 0, 2, 0, 0);
                  SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
                end;
              end else begin
                MainOutMessage('[超速操作] 更换登录验证码 ' + '/' + UserInfo.sUserIPaddr);
              end;
            end;
          end;
        end;
    end;
  except
    MainOutMessage('[Exception] TFrmMain.ProcessUserMsg ' + 'wIdent: ' + IntToStr(DefMsg.Ident) + ' sData: ' + sData);
  end;
end;

// 检查帐户是否只有数字和英文字母

function CheckAccountValid(Account: WideString): Boolean;
var
  I: Integer;
  WC: WideChar;
begin
  Result := True;
  for I := 1 to Length(Account) do
  begin
    WC := Account[I];
    if not (WC in [WideChar('0')..WideChar('9'), WideChar('a')..WideChar('z'), WideChar('A')..WideChar('Z')]) then
    begin
      Result := False;
      Exit;
    end;
  end;
end;

procedure AccountRealName(UserInfo: pTUserInfo; sData: string);
var
  sMsg, sName, sID: string;
begin
  sMsg := DecodeString(sData);
  sMsg := GetValidStr3(sMsg, sName, ['/']);
  sMsg := GetValidStr3(sMsg, sID, ['/']);

  HttpPost(1, sName, sID, '', '', UserIsRealName, Integer(UserInfo));
end;

procedure AccountQuickLogin(UserInfo: pTUserInfo; sData: string);
var
  sMsg, sToken, sUID, sID: string;
begin
  sMsg := DecodeString(sData);
  sMsg := GetValidStr3(sMsg, sToken, ['/']);
  sMsg := GetValidStr3(sMsg, sUID, ['/']);
  sMsg := GetValidStr3(sMsg, sID, ['/']);
  MainOutMessage('Token:' + sToken);
  MainOutMessage('UID:' + sUID);
  MainOutMessage('ID:' + sID);
  HttpPost(2, sToken, sUID, sID, '', QuickLogin, Integer(UserInfo));
end;

procedure AccountCreateNew(UserInfo: pTUserInfo; sData: string); //0046C244
var
  UserEntry: TUserEntry;
  UserAddEntry: TUserEntryAdd;
  AccountInfo: TAccountInfo;
  nLen: Integer;
  sUserEntryMsg: string;
  sUserAddEntryMsg: string;
  nErrCode: Integer;
  DefMsg: TDefaultMessage;
  bo21: Boolean;
  I: Integer;
  WS: WideString;
  WC: WideChar;
  boTemp: Boolean;
  sTemp: string;
resourcestring
  sAddNewuserFail = '[新建帐号失败] %s/%s';
  sLogFlag = 'new';
begin
  try
    nErrCode := -1;
    FillChar(UserEntry, SizeOf(TUserEntry), #0);
    FillChar(UserAddEntry, SizeOf(TUserEntryAdd), #0);
    nLen := GetCodeMsgSize(SizeOf(TUserEntry) * 4 / 3);
    bo21 := False;
    sUserEntryMsg := Copy(sData, 1, nLen);
    sUserAddEntryMsg := Copy(sData, nLen + 1, Length(sData) - nLen);
    if (sUserEntryMsg <> '') and (sUserAddEntryMsg <> '') then
    begin
      DecodeString(sUserEntryMsg, @UserEntry, SizeOf(TUserEntry));
      DecodeString(sUserAddEntryMsg, @UserAddEntry, SizeOf(TUserEntryAdd));
      if CheckAccountName(UserEntry.sAccount) then
        bo21 := True;

      // 用户名最小为3位
      if (Length(UserEntry.sAccount) < MIN_ACCOUNT_LEN) then
      begin
        bo21 := False;
        nErrCode := -2;
      end;

      if bo21 and (Length(UserEntry.sPassword) < 3) then
      begin
        bo21 := False;
        nErrCode := -22;
      end;

      // 检查帐户是否只有数字和英文字母 chongchong 2016-07-17
      if bo21 and (not CheckAccountValid(UserEntry.sAccount)) then
      begin
        bo21 := False;
        nErrCode := -7;
      end;

      if bo21 and (not CheckStringValid(UserEntry.sAccount)) then
      begin
        bo21 := False;
        nErrCode := -8;
      end;

      if bo21 and (not CheckStringValid(UserEntry.sPassword)) then
      begin
        bo21 := False;
        nErrCode := -9;
      end;

      if bo21 and (not CheckStringValid(UserEntry.sQuiz)) then
      begin
        bo21 := False;
        nErrCode := -10;
      end;

      if bo21 and (not CheckStringValid(UserEntry.sAnswer)) then
      begin
        bo21 := False;
        nErrCode := -11;
      end;

      { TODO -ochongchong -c修改 : 禁止ID密码相同 【2013-08-28】 }
      if bo21 and (g_Config.boDisableIDSamePassword) and SameText(UserEntry.sAccount, UserEntry.sPassword) then
      begin
        bo21 := False;
        nErrCode := -4;
      end;

      if bo21 and g_Config.boDisableQuizSameAnswer and SameText(UserEntry.sQuiz, UserEntry.sAnswer) then
      begin
        bo21 := False;
        nErrCode := -5;
      end;

      if bo21 and g_Config.boDisablePwdSameChr then
      begin
        WS := UserEntry.sPassword;
        WC := WS[1];
        boTemp := True;
        for I := 2 to Length(WS) do
        begin
          if WS[I] <> WC then
          begin
            boTemp := False;
            Break;
          end;
        end;

        if boTemp then
        begin
          bo21 := False;
          nErrCode := -18;
        end;
      end;

      if bo21 and g_Config.boDisablePwdAllNum then
      begin
        boTemp := True;
        WS := UserEntry.sPassword;
        for I := 1 to Length(WS) do
        begin
          if not (WS[I] in [WideChar('0')..WideChar('9')]) then
          begin
            boTemp := False;
            Break;
          end;
        end;

        if boTemp then
        begin
          bo21 := False;
          nErrCode := -19;
        end;
      end;

      if bo21 and g_Config.boDisablePwdAllLetter then
      begin
        boTemp := True;
        WS := UserEntry.sPassword;
        for I := 1 to Length(WS) do
        begin
          if not (WS[I] in [WideChar('a')..WideChar('z'), WideChar('A')..WideChar('Z')]) then
          begin
            boTemp := False;
            Break;
          end;
        end;

        if boTemp then
        begin
          bo21 := False;
          nErrCode := -20;
        end;
      end;

      if bo21 and (g_DisablePasswordList.Count > 0) then
      begin
        for I := 0 to g_DisablePasswordList.Count - 1 do
        begin
          sTemp := g_DisablePasswordList.Strings[I];
          if (Length(sTemp) > 0) and (Pos(sTemp, UserEntry.sPassword) > 0) then
          begin
            bo21 := False;
            nErrCode := -21;
          end;
        end;
      end;

      if bo21 then
      begin
        if not g_AccountDB.CheckAccountExists(UserEntry.sAccount) then
        begin
          FillChar(AccountInfo, SizeOf(AccountInfo), #0);
          AccountInfo.IsDisable := False;
          AccountInfo.CreateDate := Date2MyDate(Now);
          AccountInfo.AccountName := UserEntry.sAccount;
          AccountInfo.Password := UserEntry.sPassword;
          AccountInfo.UserName := UserEntry.sUserName;
          AccountInfo.IDCard := UserEntry.sSSNo;
          AccountInfo.Phone := UserEntry.sPhone;
          AccountInfo.Questions1 := UserEntry.sQuiz;
          AccountInfo.Answers1 := UserEntry.sAnswer;
          AccountInfo.Mail := UserEntry.sEMail;

          AccountInfo.BirthDay := UserAddEntry.sBirthDay;
          AccountInfo.Questions2 := UserAddEntry.sQuiz2;
          AccountInfo.Answers2 := UserAddEntry.sAnswer2;
          AccountInfo.MobilePhone := UserAddEntry.sMobilePhone;

          if g_AccountDB.AddAccount(AccountInfo) then
          begin
            nErrCode := 1;
            WriteLogMsg(sLogFlag, AccountInfo);
          end;
        end
        else
          nErrCode := 0;
      end
      else
      begin
        //MainOutMessage(Format(sAddNewuserFail, [UserEntry.sAccount, UserAddEntry.sQuiz2]));
      end; //0046C480
    end;

    if nErrCode = 1 then
    begin
      if g_Config.boNewLoginInto then
      begin
        AccountLogin(UserInfo, EncodeString(AccountInfo.AccountName + '/' + AccountInfo.Password));
      end
      else
      begin
        DefMsg := MakeDefaultMsg(SM_NEWID_SUCCESS, Integer(g_Config.boNewLoginInto), 0, 0, 0);
        SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
      end;
//      if g_Config.boRandomCode[rctRegister] then
//      begin
//        UserInfo.dwClientTick := GetTickCount();
//        UserInfo.nRandomCodeRefreshMaxCount[rctRegister] := 0;
//        SendRandomCode(UserInfo, rctRegister);
//      end;
    end
    else
    begin
      DefMsg := MakeDefaultMsg(SM_CREATEID_FAIL, nErrCode, 0, 0, 0);
      SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
//      if g_Config.boRandomCode[rctRegister] then
//      begin
//        UserInfo.dwClientTick := GetTickCount();
//        UserInfo.nRandomCodeRefreshMaxCount[rctRegister] := 0;
//        SendRandomCode(UserInfo, rctRegister);
//      end;
    end;

  except
    MainOutMessage('TFrmMain.AddNewUser');
  end;
end;

procedure AccountCreate(UserInfo: pTUserInfo; sData: string); //0046C244
var
  UserEntry: TUserEntry;
  UserAddEntry: TUserEntryAdd;
  AccountInfo: TAccountInfo;
  nLen: Integer;
  sUserEntryMsg: string;
  sUserAddEntryMsg: string;
  nErrCode: Integer;
  DefMsg: TDefaultMessage;
  bo21: Boolean;
  I: Integer;
  WS: WideString;
  WC: WideChar;
  boTemp: Boolean;
  sTemp: string;
resourcestring
  sAddNewuserFail = '[新建帐号失败] %s/%s';
  sLogFlag = 'new';
begin
  try
    nErrCode := -1;
    FillChar(UserEntry, SizeOf(TUserEntry), #0);
    FillChar(UserAddEntry, SizeOf(TUserEntryAdd), #0);
    nLen := GetCodeMsgSize(SizeOf(TUserEntry) * 4 / 3);
    bo21 := False;
    sUserEntryMsg := Copy(sData, 1, nLen);
    sUserAddEntryMsg := Copy(sData, nLen + 1, Length(sData) - nLen);
    if (sUserEntryMsg <> '') and (sUserAddEntryMsg <> '') then
    begin
      DecodeString(sUserEntryMsg, @UserEntry, SizeOf(TUserEntry));
      DecodeString(sUserAddEntryMsg, @UserAddEntry, SizeOf(TUserEntryAdd));
      if CheckAccountName(UserEntry.sAccount) then
        bo21 := True;

      if g_Config.boRandomCode[rctRegister] and ((Length(UserInfo.sRandomCode[rctRegister]) = 0) or (not SameText(UserInfo.sRandomCode[rctRegister], UserEntry.sRandCode))) then
      begin
        bo21 := False;
        nErrCode := -88;

        Inc(UserInfo.nRandomCodeErrorMaxCount[rctRegister]);
        if UserInfo.nRandomCodeErrorMaxCount[rctRegister] >= g_Config.nRandomCodeErrorMaxCount then
        begin
          DefMsg := MakeDefaultMsg(SM_RUNGATE_DISABLE_LOGIN, 0, 0, 0, 0);
          SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg) + EncodeString('输入验证码失败次数过多，连接将会断开'));

          // 错误达到最高次数，断开 2020-10-29
          UserInfo.dwDelayCloseTick := GetTickCount + 1000;
          UserInfo.boDealyClose := True;

          Exit;
        end;
      end;

      // 用户名最小为3位
      if (Length(UserEntry.sAccount) < MIN_ACCOUNT_LEN) then
      begin
        bo21 := False;
        nErrCode := -2;
      end;

      if bo21 and (Length(UserEntry.sPassword) < 3) then
      begin
        bo21 := False;
        nErrCode := -22;
      end;

      // 检查帐户是否只有数字和英文字母 chongchong 2016-07-17
      if bo21 and (not CheckAccountValid(UserEntry.sAccount)) then
      begin
        bo21 := False;
        nErrCode := -7;
      end;

      if bo21 and (not CheckStringValid(UserEntry.sAccount)) then
      begin
        bo21 := False;
        nErrCode := -8;
      end;

      if bo21 and (not CheckStringValid(UserEntry.sPassword)) then
      begin
        bo21 := False;
        nErrCode := -9;
      end;

      if bo21 and (not CheckStringValid(UserEntry.sUserName)) then
      begin
        bo21 := False;
        nErrCode := -14;
      end;

      if bo21 and (not CheckStringValid(UserEntry.sQuiz)) then
      begin
        bo21 := False;
        nErrCode := -10;
      end;

      if bo21 and (not CheckStringValid(UserEntry.sAnswer)) then
      begin
        bo21 := False;
        nErrCode := -11;
      end;

      if bo21 and (not CheckStringValid(UserAddEntry.sQuiz2)) then
      begin
        bo21 := False;
        nErrCode := -12;
      end;

      if bo21 and (not CheckStringValid(UserAddEntry.sAnswer2)) then
      begin
        bo21 := False;
        nErrCode := -13;
      end;

      if bo21 and (not CheckStringValid(UserEntry.sPhone)) then
      begin
        bo21 := False;
        nErrCode := -15;
      end;

      if bo21 and (not CheckStringValid(UserAddEntry.sMobilePhone)) then
      begin
        bo21 := False;
        nErrCode := -16;
      end;

      if bo21 and (not CheckStringValid2(UserEntry.sEMail)) then
      begin
        bo21 := False;
        nErrCode := -17;
      end;


      { TODO -ochongchong -c修改 : 禁止ID密码相同 【2013-08-28】 }
      if bo21 and (g_Config.boDisableIDSamePassword) and SameText(UserEntry.sAccount, UserEntry.sPassword) then
      begin
        bo21 := False;
        nErrCode := -4;
      end;

      if bo21 and g_Config.boDisableQuizSameAnswer and SameText(UserEntry.sQuiz, UserEntry.sAnswer) then
      begin
        bo21 := False;
        nErrCode := -5;
      end;

      if bo21 and g_Config.boDisableQuizSameAnswer and SameText(UserAddEntry.sQuiz2, UserAddEntry.sAnswer2) then
      begin
        bo21 := False;
        nErrCode := -6;
      end;

      if bo21 and g_Config.boDisablePwdSameChr then
      begin
        WS := UserEntry.sPassword;
        WC := WS[1];
        boTemp := True;
        for I := 2 to Length(WS) do
        begin
          if WS[I] <> WC then
          begin
            boTemp := False;
            Break;
          end;
        end;

        if boTemp then
        begin
          bo21 := False;
          nErrCode := -18;
        end;
      end;

      if bo21 and g_Config.boDisablePwdAllNum then
      begin
        boTemp := True;
        WS := UserEntry.sPassword;
        for I := 1 to Length(WS) do
        begin
          if not (WS[I] in [WideChar('0')..WideChar('9')]) then
          begin
            boTemp := False;
            Break;
          end;
        end;

        if boTemp then
        begin
          bo21 := False;
          nErrCode := -19;
        end;
      end;

      if bo21 and g_Config.boDisablePwdAllLetter then
      begin
        boTemp := True;
        WS := UserEntry.sPassword;
        for I := 1 to Length(WS) do
        begin
          if not (WS[I] in [WideChar('a')..WideChar('z'), WideChar('A')..WideChar('Z')]) then
          begin
            boTemp := False;
            Break;
          end;
        end;

        if boTemp then
        begin
          bo21 := False;
          nErrCode := -20;
        end;
      end;

      if bo21 and (g_DisablePasswordList.Count > 0) then
      begin
        for I := 0 to g_DisablePasswordList.Count - 1 do
        begin
          sTemp := g_DisablePasswordList.Strings[I];
          if (Length(sTemp) > 0) and (Pos(sTemp, UserEntry.sPassword) > 0) then
          begin
            bo21 := False;
            nErrCode := -21;
          end;
        end;
      end;

      if bo21 then
      begin
        if not g_AccountDB.CheckAccountExists(UserEntry.sAccount) then
        begin
          FillChar(AccountInfo, SizeOf(AccountInfo), #0);
          AccountInfo.IsDisable := False;
          AccountInfo.CreateDate := Date2MyDate(Now);
          AccountInfo.AccountName := UserEntry.sAccount;
          AccountInfo.Password := UserEntry.sPassword;
          AccountInfo.UserName := UserEntry.sUserName;
          AccountInfo.IDCard := UserEntry.sSSNo;
          AccountInfo.Phone := UserEntry.sPhone;
          AccountInfo.Questions1 := UserEntry.sQuiz;
          AccountInfo.Answers1 := UserEntry.sAnswer;
          AccountInfo.Mail := UserEntry.sEMail;

          AccountInfo.BirthDay := UserAddEntry.sBirthDay;
          AccountInfo.Questions2 := UserAddEntry.sQuiz2;
          AccountInfo.Answers2 := UserAddEntry.sAnswer2;
          AccountInfo.MobilePhone := UserAddEntry.sMobilePhone;

          if g_AccountDB.AddAccount(AccountInfo) then
          begin
            nErrCode := 1;
            WriteLogMsg(sLogFlag, AccountInfo);
          end;
        end
        else
          nErrCode := 0;
      end
      else
      begin
        //MainOutMessage(Format(sAddNewuserFail, [UserEntry.sAccount, UserAddEntry.sQuiz2]));
      end; //0046C480
    end;

    if nErrCode = 1 then
    begin
      DefMsg := MakeDefaultMsg(SM_NEWID_SUCCESS, 0, 0, 0, 0);
      if g_Config.boRandomCode[rctRegister] then
      begin
        UserInfo.dwClientTick := GetTickCount();
        UserInfo.nRandomCodeRefreshMaxCount[rctRegister] := 0;
        SendRandomCode(UserInfo, rctRegister);
      end;
    end
    else
    begin
      DefMsg := MakeDefaultMsg(SM_NEWID_FAIL, nErrCode, 0, 0, 0);
      if g_Config.boRandomCode[rctRegister] then
      begin
        UserInfo.dwClientTick := GetTickCount();
        UserInfo.nRandomCodeRefreshMaxCount[rctRegister] := 0;
        SendRandomCode(UserInfo, rctRegister);
      end;
    end;

    SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
  except
    MainOutMessage('TFrmMain.AddNewUser');
  end;
end;

procedure AccountChangePassword(UserInfo: pTUserInfo; sData: string);
var
  sMsg: string;
  sLoginID: string;
  sOldPassword: string;
  sNewPassword: string;
  sL2Password: string;
  sRandomCode: string;
  DefMsg: TDefaultMessage;
  nCode: Integer;
  AccountInfo: TAccountInfo;
  I: Integer;
  WS: WideString;
  WC: WideChar;
  IsOK: Boolean;
  boTemp: Boolean;
  sTemp: string;
resourcestring
  sChgMsg = 'chg';
begin
  try
    sMsg := DecodeString(sData);
    sMsg := GetValidStr3(sMsg, sLoginID, [#9]);
    sMsg := GetValidStr3(sMsg, sOldPassword, [#9]);
    sMsg := GetValidStr3(sMsg, sNewPassword, [#9]);

    if (Length(sMsg) > 0) and (sMsg[1] = #9) then
    begin
      sL2Password := '';
      sRandomCode := Copy(sMsg, 2, Length(sMsg) - 1);
    end
    else
    begin
      sMsg := GetValidStr3(sMsg, sL2Password, [#9]);
      sMsg := GetValidStr3(sMsg, sRandomCode, [#9]);
    end;

    nCode := 0;
    if (Length(sNewPassword) >= 3) then
    begin
      if g_AccountDB.GetAccount(sLoginID, AccountInfo) and (not AccountInfo.IsDisable) then
      begin
        if (AccountInfo.ErrorCount < 5) or ((GetTickCount - AccountInfo.LastActionTick) > 180000) then
        begin
          if AccountInfo.Password = sOldPassword then
          begin
            // 检测二级密码是否正确 chongchong 2016-11-06
            if g_Config.boEnabledL2Password and (not SameText(AccountInfo.L2Password, sL2Password)) then
            begin
              Inc(AccountInfo.ErrorCount);
              AccountInfo.LastActionTick := GetTickCount();
              nCode := -3;
            end            { TODO -ochongchong -c修改 : 禁止ID密码相同 【2013-08-28】 }
            else if (g_Config.boDisableIDSamePassword) and SameText(AccountInfo.AccountName, sNewPassword) then
            begin
              Inc(AccountInfo.ErrorCount);
              AccountInfo.LastActionTick := GetTickCount();
              nCode := -4;
            end
            else if not CheckStringValid(sNewPassword) then
            begin
              Inc(AccountInfo.ErrorCount);
              AccountInfo.LastActionTick := GetTickCount();
              nCode := -5;
            end
            else if g_Config.boRandomCode[rctPwdChange] and ((Length(UserInfo.sRandomCode[rctPwdChange]) = 0) or (not SameText(UserInfo.sRandomCode[rctPwdChange], sRandomCode))) then
            begin
              Inc(AccountInfo.ErrorCount);
              AccountInfo.LastActionTick := GetTickCount();
              nCode := -10;

              Inc(UserInfo.nRandomCodeErrorMaxCount[rctPwdChange]);
              if UserInfo.nRandomCodeErrorMaxCount[rctPwdChange] >= g_Config.nRandomCodeErrorMaxCount then
              begin
                DefMsg := MakeDefaultMsg(SM_RUNGATE_DISABLE_LOGIN, 0, 0, 0, 0);
                SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg) + EncodeString('输入验证码失败次数过多，连接将会断开'));

                // 错误达到最高次数，断开 2020-10-29
                UserInfo.dwDelayCloseTick := GetTickCount + 1000;
                UserInfo.boDealyClose := True;

                Exit;
              end;
            end
            else
            begin
              IsOK := True;

              if g_Config.boDisablePwdSameChr then
              begin
                WS := sNewPassword;
                WC := WS[1];
                boTemp := True;
                for I := 2 to Length(WS) do
                begin
                  if WS[I] <> WC then
                  begin
                    boTemp := False;
                    Break;
                  end;

                end;

                if boTemp then
                begin
                  IsOK := False;
                  nCode := -6;
                end;
              end;

              if IsOK and g_Config.boDisablePwdAllNum then
              begin
                boTemp := True;
                WS := sNewPassword;
                for I := 1 to Length(WS) do
                begin
                  if not (WS[I] in [WideChar('0')..WideChar('9')]) then
                  begin
                    boTemp := False;
                    Break;
                  end;
                end;

                if boTemp then
                begin
                  IsOK := False;
                  nCode := -7;
                end;
              end;

              if IsOK and g_Config.boDisablePwdAllLetter then
              begin
                boTemp := True;
                WS := sNewPassword;
                for I := 1 to Length(WS) do
                begin
                  if not (WS[I] in [WideChar('a')..WideChar('z'), WideChar('A')..WideChar('Z')]) then
                  begin
                    boTemp := False;
                    Break;
                  end;
                end;

                if boTemp then
                begin
                  IsOK := False;
                  nCode := -8;
                end;
              end;

              if IsOK and (g_DisablePasswordList.Count > 0) then
              begin
                for I := 0 to g_DisablePasswordList.Count - 1 do
                begin
                  sTemp := g_DisablePasswordList.Strings[I];
                  if (Length(sTemp) > 0) and (Pos(sTemp, sNewPassword) > 0) then
                  begin
                    nCode := -9;
                  end;
                end;
              end;

              if nCode <> 0 then
              begin
                Inc(AccountInfo.ErrorCount);
                AccountInfo.LastActionTick := GetTickCount();
              end
              else
              begin
                AccountInfo.ErrorCount := 0;
                AccountInfo.Password := sNewPassword;
                nCode := 1;
                WriteLogMsg(sChgMsg, AccountInfo);
              end;
            end;
          end
          else
          begin
            Inc(AccountInfo.ErrorCount);
            AccountInfo.LastActionTick := GetTickCount();
            nCode := 0; // 原来是-1 表示原密码错误，会被扫号器利用。改成0 2019-09-21 01:02:31

            if g_Config.boRandomCode[rctPwdChange] then
            begin
              UserInfo.dwClientTick := GetTickCount();
              UserInfo.nRandomCodeRefreshMaxCount[rctPwdChange] := 0;
              SendRandomCode(UserInfo, rctPwdChange);
            end;
          end;

          g_AccountDB.UpdateAccount(AccountInfo, ufAllField);
        end
        else
        begin
          nCode := -2;
          if GetTickCount < AccountInfo.LastActionTick then
          begin
            AccountInfo.LastActionTick := GetTickCount();
            g_AccountDB.UpdateAccount(AccountInfo, ufPartField);
          end;
        end;
      end
      else
      begin
        if g_Config.boRandomCode[rctPwdChange] then
        begin
          UserInfo.dwClientTick := GetTickCount();
          UserInfo.nRandomCodeRefreshMaxCount[rctPwdChange] := 0;
          SendRandomCode(UserInfo, rctPwdChange);
        end;
      end;
    end;

    if nCode = 1 then
    begin
      DefMsg := MakeDefaultMsg(SM_CHGPASSWD_SUCCESS, 0, 0, 0, 0);

      if g_Config.boRandomCode[rctPwdChange] then
      begin
        UserInfo.dwClientTick := GetTickCount();
        UserInfo.nRandomCodeRefreshMaxCount[rctPwdChange] := 0;
        SendRandomCode(UserInfo, rctPwdChange);
      end;
    end
    else
    begin
      DefMsg := MakeDefaultMsg(SM_CHGPASSWD_FAIL, nCode, 0, 0, 0);
    end;
    SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
  except
    MainOutMessage('TFrmMain.ChangePassword');
  end;
end;

procedure AccountBindPhone(UserInfo: pTUserInfo; sData: string);
var
  sMsg: string;
  sPhone: string;
  sVerifiaction: string;
  DefMsg: TDefaultMessage;
  nCode: Integer;
  AccountInfo: TAccountInfo;
  //I: Integer;
  //WS: WideString;
  //WC: WideChar;
  //IsOK: Boolean;
  //boTemp: Boolean;
  //sTemp: string;
resourcestring
  sChgMsg = 'chg';
begin
  try
    sMsg := DecodeString(sData);
    sMsg := GetValidStr3(sMsg, sPhone, ['/']);
    sMsg := GetValidStr3(sMsg, sVerifiaction, ['/']);

    nCode := 0;
    if (Length(sPhone) = 11) and (UserInfo.sLoginPhone = sPhone) and (UserInfo.sVerificationCode = sVerifiaction) then begin
      if g_AccountDB.GetAccount(UserInfo.sAccount, AccountInfo) and (not AccountInfo.IsDisable) then begin
        if (AccountInfo.ErrorCount < 5) or ((GetTickCount - AccountInfo.LastActionTick) > 180000) then begin
          //IsOK := True;
          if nCode <> 0 then begin
            Inc(AccountInfo.ErrorCount);
            AccountInfo.LastActionTick := GetTickCount();
          end else begin
            AccountInfo.ErrorCount := 0;
            AccountInfo.MobilePhone := sPhone;
            nCode := 1;
            WriteLogMsg(sChgMsg, AccountInfo);
          end;
          g_AccountDB.UpdateAccount(AccountInfo, ufAllField);
        end else begin
          nCode := -2;
          if GetTickCount < AccountInfo.LastActionTick then begin
            AccountInfo.LastActionTick := GetTickCount();
            g_AccountDB.UpdateAccount(AccountInfo, ufPartField);
          end;
        end;
      end;
    end;

    if nCode = 1 then begin
      AccountLogin(UserInfo, EncodeString(UserInfo.sReLoginUser + '/' + UserInfo.sReLoginPassword));
//      DefMsg := MakeDefaultMsg(SM_CHANGEPHONE_SUCCESS, 0, 0, 0, 0);
//
//      if g_Config.boRandomCode[rctPwdChange] then
//      begin
//        UserInfo.dwClientTick := GetTickCount();
//        UserInfo.nRandomCodeRefreshMaxCount[rctPwdChange] := 0;
//        SendRandomCode(UserInfo, rctPwdChange);
//      end;
    end
    else
    begin
      DefMsg := MakeDefaultMsg(SM_BINDPHONE_FAIL, nCode, 0, 0, 0);
    end;
    UserInfo.sLoginPhone := '';
    UserInfo.sVerificationCode := '';
    SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
  except
    MainOutMessage('TFrmMain.BindPhone');
  end;
end;

procedure AccountChangePhone(UserInfo: pTUserInfo; sData: string);
var
  sMsg: string;
  sLoginID: string;
  sPassword: string;
  sQ1: string;
  sA1: string;
  sPhone: string;
  sVerifiaction: string;
  DefMsg: TDefaultMessage;
  nCode: Integer;
  AccountInfo: TAccountInfo;
  //I: Integer;
  //WS: WideString;
  //WC: WideChar;
  //IsOK: Boolean;
  //boTemp: Boolean;
  //sTemp: string;
resourcestring
  sChgMsg = 'chg';
begin
  try
    sMsg := DecodeString(sData);
    sMsg := GetValidStr3(sMsg, sLoginID, ['/']);
    sMsg := GetValidStr3(sMsg, sPassword, ['/']);
    sMsg := GetValidStr3(sMsg, sQ1, ['/']);
    sMsg := GetValidStr3(sMsg, sA1, ['/']);
    sMsg := GetValidStr3(sMsg, sPhone, ['/']);
    sMsg := GetValidStr3(sMsg, sVerifiaction, ['/']);

    nCode := 0;
    if (Length(sPhone) = 11) and (UserInfo.sLoginPhone = sPhone) and (UserInfo.sVerificationCode = sVerifiaction) then begin
      if g_AccountDB.GetAccount(sLoginID, AccountInfo) and (not AccountInfo.IsDisable) then begin
        if (AccountInfo.ErrorCount < 5) or ((GetTickCount - AccountInfo.LastActionTick) > 180000) then begin
          if (AccountInfo.Password = sPassword) and (AccountInfo.Questions1 = sQ1) and (AccountInfo.Answers1 = sA1) then begin
            begin
              //IsOK := True;
              if nCode <> 0 then begin
                Inc(AccountInfo.ErrorCount);
                AccountInfo.LastActionTick := GetTickCount();
              end else begin
                AccountInfo.ErrorCount := 0;
                AccountInfo.MobilePhone := sPhone;
                nCode := 1;
                WriteLogMsg(sChgMsg, AccountInfo);
              end;
            end;
          end else begin
            Inc(AccountInfo.ErrorCount);
            AccountInfo.LastActionTick := GetTickCount();
            nCode := 0; // 原来是-1 表示原密码错误，会被扫号器利用。改成0 2019-09-21 01:02:31
          end;

          g_AccountDB.UpdateAccount(AccountInfo, ufAllField);
        end else begin
          nCode := -2;
          if GetTickCount < AccountInfo.LastActionTick then begin
            AccountInfo.LastActionTick := GetTickCount();
            g_AccountDB.UpdateAccount(AccountInfo, ufPartField);
          end;
        end;
      end;
    end;

    if nCode = 1 then begin
      DefMsg := MakeDefaultMsg(SM_CHANGEPHONE_SUCCESS, 0, 0, 0, 0);

      if g_Config.boRandomCode[rctPwdChange] then begin
        UserInfo.dwClientTick := GetTickCount();
        UserInfo.nRandomCodeRefreshMaxCount[rctPwdChange] := 0;
        SendRandomCode(UserInfo, rctPwdChange);
      end;
    end else begin
      DefMsg := MakeDefaultMsg(SM_CHANGEPHONE_FAIL, nCode, 0, 0, 0);
    end;
    UserInfo.sLoginPhone := '';
    UserInfo.sVerificationCode := '';
    SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
  except
    MainOutMessage('TFrmMain.ChangePhone');
  end;
end;

procedure AccountCheckProtocol(UserInfo: pTUserInfo; nDate: Integer);
var
  DefMsg: TDefaultMessage;
begin
  if nDate < nVersionDate then
  begin
    DefMsg := MakeDefaultMsg(SM_CERTIFICATION_FAIL, 0, 0, 0, 0);
  end
  else
  begin
    DefMsg := MakeDefaultMsg(SM_CERTIFICATION_SUCCESS, 0, 0, 0, 0);
    UserInfo.nVersionDate := nDate;
    UserInfo.boCertificationOK := True;
  end;
  SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
end;

function KickUser(UserInfo: pTUserInfo; IsLock: Boolean): Boolean;
var
  I: Integer;
  II: Integer;
  GateInfo: pTGateInfo;
  User: pTUserInfo;
resourcestring
  sKickMsg = 'Kick: %s';
begin
  Result := False;
  if IsLock then
    EnterCriticalSection(g_Config.GateCriticalSection);
  try
    for I := 0 to Length(g_Config.GateList) - 1 do
    begin
      GateInfo := @g_Config.GateList[I];
      if (GateInfo.Socket <> nil) then
      begin
        for II := 0 to GateInfo.UserList.Count - 1 do
        begin
          User := GateInfo.UserList.Items[II];
          if User = UserInfo then
          begin
            if g_Config.boShowDetailMsg then
              MainOutMessage(format(sKickMsg, [UserInfo.sUserIPaddr]));
            SendGateKickMsg(GateInfo.Socket, UserInfo.sSockIndex);
            GateInfo.UserList.Delete(II);
            Dispose(UserInfo);

            Result := True;
            Exit;
          end;
        end;
      end;
    end;
  finally
    if IsLock then
      LeaveCriticalSection(g_Config.GateCriticalSection);
  end;
end;

function IsRightID(sID: string{; var sSex: string; var age: Integer}): Boolean;
  //判断是否存在非法字符
var
  sSex: string;
  //age: Integer;

  function JudgeIllegal(str: string): Boolean;
  var
    i: Integer;
  begin
    Result := False;
    for i := 1 to Length(str) do
    begin
      case str[i] of
        '0'..'9':
          begin
          end
      else
        begin
          Exit;
        end;
      end;
    end;
    Result := True;
  end;
  //加权函数

  function Weighted(str: string): string;
  var
    i: Integer;
    sum: Integer;
  begin
    sum := 0;
    for i := 1 to 17 do
    begin
      sum := sum + StrToInt(str[i]) * IntMultiplication[i];
    end;
    sum := sum mod 11;
    case sum of
      0:
        Result := '1';
      1:
        Result := '0';
      2:
        Result := 'X';
      3:
        Result := '9';
      4:
        Result := '8';
      5:
        Result := '7';
      6:
        Result := '6';
      7:
        Result := '5';
      8:
        Result := '4';
      9:
        Result := '3';
      10:
        Result := '2';
    end;
  end;

begin
  Result := False;
  try
    if Length(sID) <> 18 then begin
//      Result := '身份证号应为18位！';
      exit;
    end;
    if not JudgeIllegal(Copy(sID, 1, 17)) then begin
//      Result := '身份证号前17位应为数字！';
      exit;
    end;
    //判断成功
    if (Weighted(sID) = Copy(sID, 18, 1)) then begin
//      Result := 'OK';
      Result := True;
      //判断性别
      if (StrToInt(Copy(sID, 17, 1)) mod 2) = 0 then
        sSex := '女'
      else
        sSex := '男';
      //计算年龄
      //age := Yearof(Now) - StrToInt(Copy(sID, 7, 4));
    end;
  except
    on E: Exception do begin
//      Result := '异常:' + E.Message;
      Result := False;
    end;
  end;
end;

procedure AccountLogin(UserInfo: pTUserInfo; sData: string; boLegend: Boolean);
var
  sLoginID: string;
  sPassword, sTemp: string;
  nCode: Integer;
  boNeedUpdate: Boolean;
  DefMsg: TDefaultMessage;
  UserEntry: TUserEntry;
  UserAdd: TUserEntryAdd;
  nIDCost: Integer;
  nIPCost: Integer;
  nIDCostIndex: Integer;
  nIPCostIndex: Integer;
  AccountInfo: TAccountInfo;
  IsUpdated: Boolean;
  sServerName: string;
  ReadUserIP: Integer;
  ReadUserMac: string;
begin
  try
    // 登录验证码
    if (g_Config.boRandomCode[rctLogin]) and (not UserInfo.boRandomCodeOK[rctLogin]) then
    begin
      if (UserInfo.nRandomCodeErrorMaxCount[rctLogin] < g_Config.nRandomCodeErrorMaxCount) then
      begin
        SendRandomCode(UserInfo, rctLogin);
      end;

      Exit;
    end;

    sTemp := GetValidStr3(DecodeString(sData), sLoginID, ['/']);
    sTemp := GetValidStr3(sTemp, sPassword, ['/']);

    boNeedUpdate := False;

    if (not g_AccountDB.GetAccount(sLoginID, AccountInfo)) or (AccountInfo.IsDisable) then
    begin
      DefMsg := MakeDefaultMsg(SM_PASSWD_FAIL, -1, 0, 0, 0);
      SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));

      if g_Config.boRandomCode[rctLogin] then
      begin
        UserInfo.dwClientTick := GetTickCount();
        UserInfo.nRandomCodeRefreshMaxCount[rctLogin] := 0;
        SendRandomCode(UserInfo, rctLogin);
      end;
      Exit;
    end;

    ReadUserIP := AccountInfo.LoginIP;
    ReadUserMac := AccountInfo.LoginMac;

    {自动解除锁定账号}
    if (g_Config.boUnLockAccount) and (AccountInfo.ErrorCount >= 5)
      and ((GetTickCount - AccountInfo.LastActionTick) >= Cardinal(g_Config.dwUnLockAccountTime * 60 * 1000)) then begin
      AccountInfo.ErrorCount := 0;
      AccountInfo.LastActionTick := GetTickCount - 70000;
    end;

    if (AccountInfo.ErrorCount < 5) or ((GetTickCount - AccountInfo.LastActionTick) > 60000) then
    begin
      if AccountInfo.Password = sPassword then
      begin
        if ((AccountInfo.UserName = '') or (AccountInfo.Questions2 = '')) and (not g_Config.boNewLoginDlg) then
        begin
          UserEntry.sAccount := AccountInfo.AccountName;
          UserEntry.sPassword := AccountInfo.Password;
          UserEntry.sUserName := AccountInfo.UserName;
          UserEntry.sSSNo := AccountInfo.IDCard;
          UserEntry.sPhone := AccountInfo.Phone;
          UserEntry.sQuiz := AccountInfo.Questions1;
          UserEntry.sAnswer := AccountInfo.Answers1;
          UserEntry.sEMail := AccountInfo.Mail;

          UserAdd.sQuiz2 := AccountInfo.Questions2;
          UserAdd.sAnswer2 := AccountInfo.Answers2;
          UserAdd.sBirthDay := AccountInfo.BirthDay;
          UserAdd.sMemo := AccountInfo.Memo;

          AccountInfo.ErrorCount := 0;
          boNeedUpdate := True;
        end;

        AccountInfo.LoginDate := Date2MyDate(Now());
        IsUpdated := True;
        nCode := 1;
      end
      else
      begin
        Inc(AccountInfo.ErrorCount);
        AccountInfo.LastActionTick := GetTickCount();

        IsUpdated := True;
        nCode := -1;

        if g_Config.boRandomCode[rctLogin] then
        begin
          UserInfo.dwClientTick := GetTickCount();
          UserInfo.nRandomCodeRefreshMaxCount[rctLogin] := 0;
          SendRandomCode(UserInfo, rctLogin);
        end;
      end;
    end
    else
    begin
      nCode := -2;
      AccountInfo.LastActionTick := GetTickCount();
      IsUpdated := True;
    end;

    if (nCode = 1) and IsLogin(sLoginID) then
    begin
      SessionKick(sLoginID);
      nCode := -3;
    end;

    if boNeedUpdate then
    begin
      SetLength(sTemp, SizeOf(TUserEntry) + SizeOf(TUserEntryAdd));
      Move(UserEntry, sTemp[1], SizeOf(TUserEntry));
      Move(UserAdd, sTemp[1 + SizeOf(TUserEntry)], SizeOf(TUserEntry));

      DefMsg := MakeDefaultMsg(SM_NEEDUPDATE_ACCOUNT, 0, 0, 0, 0);
      SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg) + EncodeString(sTemp));
    end;

    if nCode = 1 then
    begin
      UserInfo.sAccount := sLoginID;           
//    实名认证检测 By 一支笔 at:2022-01-06 12:47:08

//      if not IsRightID(AccountInfo.IDCard) then
//      begin
//        UserInfo.sReLoginUser := AccountInfo.AccountName;
//        UserInfo.sReLoginPassword := AccountInfo.Password;
//        DefMsg := MakeDefaultMsg(SM_REALNAME, 0, 0, 0, 0);
//        SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
//        Exit;
//      end;
//
//      if (AccountInfo.MobilePhone = '') and g_Config.boNewLoginPhone then
//      begin
//        UserInfo.sReLoginUser := AccountInfo.AccountName;
//        UserInfo.sReLoginPassword := AccountInfo.Password;
//        DefMsg := MakeDefaultMsg(SM_BINDPHONE, 0, 0, 0, 0);
//        SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
//        Exit;
//      end;

      UserInfo.nSessionID := GetSessionID();
      UserInfo.boSelServer := False;
      try
        CS_DB.Enter;
        nIDCostIndex := g_Config.AccountCostList.GetIndex(UserInfo.sAccount);
        nIPCostIndex := g_Config.IPaddrCostList.GetIndex(UserInfo.sUserIPaddr);
        nIDCost := 0;
        nIPCost := 0;
        //boPayCost := False;
        if nIDCostIndex >= 0 then
          nIDCost := Integer(g_Config.AccountCostList.Objects[nIDCostIndex]);
        if nIPCostIndex >= 0 then
        begin
          nIPCost := Integer(g_Config.IPaddrCostList.Objects[nIPCostIndex]);
          //boPayCost := True;
        end;
      finally
        CS_DB.Leave;
      end;

      if (nIDCost >= 0) or (nIPCost >= 0) then
        UserInfo.boPayCost := True
      else
        UserInfo.boPayCost := False;

      UserInfo.nIDDay := LoWord(nIDCost);
      UserInfo.nIDHour := HiWord(nIDCost);
      UserInfo.nIPDay := LoWord(nIPCost);
      UserInfo.nIPHour := HiWord(nIPCost);

      if g_Config.boEnabledL2Password then
      begin
        // 设置Mac
        if (Length(sTemp) > 0) and CheckValidMac(sTemp) then
        begin
          UserInfo.sLoginMAC := sTemp;
        end;

        if Length(AccountInfo.L2Password) = 0 then
        begin
          DefMsg := MakeDefaultMsg(SM_SETL2PASSWORD, 0, 0, 0, 0);

          SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));

          Exit;
        end
        else
        begin
          if g_Config.boAlwaysCheckL2 then
          begin
            DefMsg := MakeDefaultMsg(SM_CHECKL2PASSWORD, 0, 0, 0, 0);
            SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
            Exit;
          end
          else
          begin
            if (g_Config.boChangedIPCheckL2) then
            begin
              if (ReadUserIP <> 0) and (inet_addr(pchar(UserInfo.sUserIPaddr)) <> ReadUserIP) then
              begin
                DefMsg := MakeDefaultMsg(SM_CHECKL2PASSWORD, 0, 0, 0, 0);
                SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
                Exit;
              end;
            end;

            if (g_Config.boChangedMACCheckL2) then
            begin
              if (Length(ReadUserMac) > 0) then
              begin
                if not SameText(UserInfo.sLoginMAC, ReadUserMac) then
                begin
                  DefMsg := MakeDefaultMsg(SM_CHECKL2PASSWORD, 0, 0, 0, 0);
                  SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
                  Exit;
                end;
              end;
            end;
          end;
        end;
      end;

      AccountInfo.LoginIP := inet_addr(pchar(UserInfo.sUserIPaddr));
      if Length(UserInfo.sLoginMAC) > 0 then
      begin
        AccountInfo.LoginMac := UserInfo.sLoginMAC;
      end;
      IsUpdated := True;

      UserInfo.boL2PasswordOK := True;

      SessionAdd(UserInfo.sAccount, UserInfo.sUserIPaddr, UserInfo.nSessionID, UserInfo.boPayCost, False);

      if not UserInfo.boPayCost then
      begin
        DefMsg := MakeDefaultMsg(SM_PASSOK_SELECTSERVER, 0, 0, 0, g_Config.ServerNameList.Count);
      end
      else
      begin
        DefMsg := MakeDefaultMsg(SM_PASSOK_SELECTSERVER, nIDCost, Loword(nIPCost), HiWord(nIPCost), g_Config.ServerNameList.Count);
      end;
      sServerName := GetServerListInfo;
      SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg) + EncodeString(sServerName));

      // 选择服务器这里不要输入验证码 2021-03-04 19:09:24
      {
      if g_Config.boRandomCode[rctLogin] then
      begin
        UserInfo.dwClientTick := GetTickCount();
        UserInfo.nRandomCodeRefreshMaxCount[rctLogin] := 0;
        //SendRandomCode(UserInfo, rctLogin);
      end;
      }
    end
    else
    begin
      DefMsg := MakeDefaultMsg(SM_PASSWD_FAIL, nCode, 0, 0, 0);
      SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
    end;

    if IsUpdated then
    begin
      g_AccountDB.UpdateAccount(AccountInfo, ufPartField);
    end;
  except
    MainOutMessage('TFrmMain.LoginUser');
  end;
end;

procedure GetSelGateInfo(sServerName, sIPaddr: string; var sSelGateIP: string; var nSelGatePort: Integer);
var
  I: Integer;
  nGateIdx: Integer;
  nGateCount: Integer;
  nSelIdx: Integer;
  boSelected: Boolean;
begin
  try
    sSelGateIP := '';
    nSelGatePort := 0;
    for I := 0 to g_Config.nRouteCount - 1 do
    begin
      if {g_Config.boDynamicIPMode or } ((g_Config.GateRoute[I].sServerName = sServerName) and (g_Config.GateRoute[I].sPublicAddr = sIPaddr)) then
      begin
        nGateCount := 0;
        nGateIdx := 0;
        while (True) do
        begin
          if (g_Config.GateRoute[I].Gate[nGateIdx].sIPaddr <> '') and (g_Config.GateRoute[I].Gate[nGateIdx].boEnable) then
            Inc(nGateCount);
          Inc(nGateIdx);
          if nGateIdx >= 10 then
            break;
        end;
        if nGateCount <= 0 then
          break; //如果没有相关网关IP设置，则跳出

        nSelIdx := g_Config.GateRoute[I].nSelIdx;
        boSelected := False;
        for nGateIdx := nSelIdx + 1 to 9 do
        begin
          if (g_Config.GateRoute[I].Gate[nGateIdx].sIPaddr <> '') and (g_Config.GateRoute[I].Gate[nGateIdx].boEnable) then
          begin
            g_Config.GateRoute[I].nSelIdx := nGateIdx;
            boSelected := True;
            break;
          end;
        end;
        if not boSelected then
        begin
          for nGateIdx := 0 to nSelIdx - 1 do
          begin
            if (g_Config.GateRoute[I].Gate[nGateIdx].sIPaddr <> '') and (g_Config.GateRoute[I].Gate[nGateIdx].boEnable) then
            begin
              g_Config.GateRoute[I].nSelIdx := nGateIdx;
              break;
            end;
          end;
        end; //0046DA2B
        nSelIdx := g_Config.GateRoute[I].nSelIdx;
        sSelGateIP := g_Config.GateRoute[I].Gate[nSelIdx].sIPaddr;
        nSelGatePort := g_Config.GateRoute[I].Gate[nSelIdx].nPort;
        break;
      end; //0046DA72
    end; //0046DA7E
  except
    MainOutMessage('TFrmMain.GetSelGateInfo');
  end;
end;

function GetServerListInfo(account: string): string;
var
  sServerInfo: string;
  I: Integer;
  sServerName: string;
begin
  try
    for I := 0 to g_Config.ServerNameList.Count - 1 do
    begin
      sServerName := g_Config.ServerNameList.Strings[I];
      if sServerName <> '' then
        sServerInfo := sServerInfo + sServerName + '/' + IntToStr(FrmMasSoc.ServerStatus(sServerName)) + '/';
    end;
  {
  for I := 0 to n473290 - 1 do begin
    if (GateRoute[i].sServerName <> '') then begin
      sServerInfo:=sServerInfo + GateRoute[i].sServerName + '/' + IntToStr(FrmMasSoc.ServerStatus(GateRoute[i].sServerName)) + '/';
    end;
  end;
  }
    Result := sServerInfo;
  except
    MainOutMessage('TFrmMain.GetServerListInfo');
  end;
end;

procedure AccountLoginPhone(UserInfo: pTUserInfo; sData: string);

  function CreateString(): string;
  var
    SourceStr, str: string;
    i: integer;
  begin
    SourceStr := 'abcdefghijklmnopqrstuvwxyz0123456789';
    randomize;
    for i := 1 to 10 do
      str := str + SourceStr[Random(Length(SourceStr)) + 1];
    Result := str;
  end;

var
  //I: Integer;
  sTemp, sPhone, sVerificationCode: string;
  sCrateAccount, sCreatePassWord: string;
  AccountInfo: TAccountInfo;
  //IsUpdated: Boolean;
  //sServerName: string;
  //ReadUserIP: Integer;
  ReadUserMac: string;
  //nIDCost: Integer;
  //nIPCost: Integer;
  //nIDCostIndex: Integer;
  //nIPCostIndex: Integer;
  nCode: Integer;
  //UserEntry: TUserEntry;
  //UserAdd: TUserEntryAdd;
  DefMsg: TDefaultMessage;
  isCreate: Boolean;
begin
  sTemp := GetValidStr3(DecodeString(sData), sPhone, ['/']);
  sTemp := GetValidStr3(sTemp, sVerificationCode, ['/']);

  if UserInfo.sLoginPhone <> sPhone then begin
    DefMsg := MakeDefaultMsg(SM_PHONELOGIN, -1, 0, 0, 0);
    SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
    Exit
  end;

  if UserInfo.sVerificationCode <> sVerificationCode then begin
    DefMsg := MakeDefaultMsg(SM_PHONELOGIN, -2, 0, 0, 0);
    SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
    Exit;
  end;

  UserInfo.sVerificationCode := '';
  UserInfo.dwLastVerificationCodeTick := 0;
  isCreate := False;

  if (not g_AccountDB.GetAccountByPhone(sPhone, AccountInfo)) or (AccountInfo.IsDisable) then begin
    sCrateAccount := CreateString();
    sCreatePassWord := CreateString();
    //MainOutMessage('没有找到' + sPhone + '自动创建帐号：' + sCrateAccount + '自动创建密码：' + sCreatePassWord);
    while not isCreate do begin
      if not g_AccountDB.CheckAccountExists(sCrateAccount) then begin
        FillChar(AccountInfo, SizeOf(AccountInfo), #0);
        AccountInfo.IsDisable := False;
        AccountInfo.CreateDate := Date2MyDate(Now);
        AccountInfo.AccountName := sCrateAccount;
        AccountInfo.Password := sCreatePassWord;
        AccountInfo.MobilePhone := sPhone;
        AccountInfo.BirthDay := '1999/9/9';
//        AccountInfo.UserName := '123123';
  //      AccountInfo.UserName := UserEntry.sUserName;
  //      AccountInfo.IDCard := UserEntry.sSSNo;
  //      AccountInfo.Phone := UserEntry.sPhone;
  //      AccountInfo.Questions1 := UserEntry.sQuiz;
  //      AccountInfo.Answers1 := UserEntry.sAnswer;
  //      AccountInfo.Mail := UserEntry.sEMail;
  //
  //      AccountInfo.BirthDay := UserAddEntry.sBirthDay;
  //      AccountInfo.Questions2 := UserAddEntry.sQuiz2;
  //      AccountInfo.Answers2 := UserAddEntry.sAnswer2;
  //      AccountInfo.MobilePhone := UserAddEntry.sMobilePhone;

        if g_AccountDB.AddAccount(AccountInfo) then begin
          //nErrCode := 1;
          isCreate := True;
          WriteLogMsg('new', AccountInfo);
        end;
      end
    end;
  end;

  //ReadUserIP := AccountInfo.LoginIP;
  ReadUserMac := AccountInfo.LoginMac;

    {自动解除锁定账号}
  if (g_Config.boUnLockAccount) and (AccountInfo.ErrorCount >= 5)
    and ((GetTickCount - AccountInfo.LastActionTick) >= Cardinal(g_Config.dwUnLockAccountTime * 60 * 1000)) then begin
    AccountInfo.ErrorCount := 0;
    AccountInfo.LastActionTick := GetTickCount - 70000;
  end;

  if (AccountInfo.ErrorCount < 5) or ((GetTickCount - AccountInfo.LastActionTick) > 60000) then begin
    if AccountInfo.MobilePhone = sPhone then begin
//      if (AccountInfo.UserName = '') or (AccountInfo.Questions2 = '') then
//      begin
//        UserEntry.sAccount := AccountInfo.AccountName;
//        UserEntry.sPassword := AccountInfo.Password;
//        UserEntry.sUserName := AccountInfo.UserName;
//        UserEntry.sSSNo := AccountInfo.IDCard;
//        UserEntry.sPhone := AccountInfo.Phone;
//        UserEntry.sQuiz := AccountInfo.Questions1;
//        UserEntry.sAnswer := AccountInfo.Answers1;
//        UserEntry.sEMail := AccountInfo.Mail;
//
//        UserAdd.sQuiz2 := AccountInfo.Questions2;
//        UserAdd.sAnswer2 := AccountInfo.Answers2;
//        UserAdd.sBirthDay := AccountInfo.BirthDay;
//        UserAdd.sMemo := AccountInfo.Memo;
//
//        AccountInfo.ErrorCount := 0;
//        boNeedUpdate := True;
//      end;

      AccountInfo.LoginDate := Date2MyDate(Now());
      //IsUpdated := True;
      nCode := 1;
    end else begin
      Inc(AccountInfo.ErrorCount);
      AccountInfo.LastActionTick := GetTickCount();

      //IsUpdated := True;
      nCode := -1;

      if g_Config.boRandomCode[rctLogin] then begin
        UserInfo.dwClientTick := GetTickCount();
        UserInfo.nRandomCodeRefreshMaxCount[rctLogin] := 0;
        SendRandomCode(UserInfo, rctLogin);
      end;
    end;
  end else begin
    nCode := -2;
    AccountInfo.LastActionTick := GetTickCount();
    //IsUpdated := True;
  end;

  if (nCode = 1) and IsLogin(AccountInfo.AccountName) then begin
    SessionKick(AccountInfo.AccountName);
    nCode := -3;
  end;

//    实名认证检测 By 一支笔 at:2022-01-06 12:47:08

//    if not IsRightID(AccountInfo.IDCard) then
//    begin
//      DefMsg := MakeDefaultMsg(SM_REALNAME, 0, 0, 0, 0);
//      SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
//      Exit;
//    end;

//  if boNeedUpdate then
//  begin
//    SetLength(sTemp, SizeOf(TUserEntry) + SizeOf(TUserEntryAdd));
//    Move(UserEntry, sTemp[1], SizeOf(TUserEntry));
//    Move(UserAdd, sTemp[1 + SizeOf(TUserEntry)], SizeOf(TUserEntry));
//
//    DefMsg := MakeDefaultMsg(SM_NEEDUPDATE_ACCOUNT, 0, 0, 0, 0);
//    SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg) + EncodeString(sTemp));
//  end;

  if nCode = 1 then begin
    if isCreate then begin
      DefMsg := MakeDefaultMsg(SM_PHONELOGIN, nCode, 0, 0, 0);
      SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg) + EncodeString(AccountInfo.AccountName + '/' + AccountInfo.Password));
    end;
    AccountLogin(UserInfo, EncodeString(AccountInfo.AccountName + '/' + AccountInfo.Password));
    Exit;
  end;
//  else
//  begin
//  end;
//    UserInfo.sAccount := AccountInfo.AccountName;
//    UserInfo.nSessionID := GetSessionID();
//    UserInfo.boSelServer := False;
//    try
//      CS_DB.Enter;
//      nIDCostIndex := g_Config.AccountCostList.GetIndex(UserInfo.sAccount);
//      nIPCostIndex := g_Config.IPaddrCostList.GetIndex(UserInfo.sUserIPaddr);
//      nIDCost := 0;
//      nIPCost := 0;
//        //boPayCost := False;
//      if nIDCostIndex >= 0 then
//        nIDCost := Integer(g_Config.AccountCostList.Objects[nIDCostIndex]);
//      if nIPCostIndex >= 0 then
//      begin
//        nIPCost := Integer(g_Config.IPaddrCostList.Objects[nIPCostIndex]);
//          //boPayCost := True;
//      end;
//    finally
//      CS_DB.Leave;
//    end;
//
//    if (nIDCost >= 0) or (nIPCost >= 0) then
//      UserInfo.boPayCost := True
//    else
//      UserInfo.boPayCost := False;
//
//    UserInfo.nIDDay := LoWord(nIDCost);
//    UserInfo.nIDHour := HiWord(nIDCost);
//    UserInfo.nIPDay := LoWord(nIPCost);
//    UserInfo.nIPHour := HiWord(nIPCost);
//
//    if g_Config.boEnabledL2Password then
//    begin
//        // 设置Mac
//      if (Length(sTemp) > 0) and CheckValidMac(sTemp) then
//      begin
//        UserInfo.sLoginMAC := sTemp;
//      end;
//
//      if Length(AccountInfo.L2Password) = 0 then
//      begin
//        DefMsg := MakeDefaultMsg(SM_SETL2PASSWORD, 0, 0, 0, 0);
//
//        SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
//
//        Exit;
//      end
//      else
//      begin
//        if g_Config.boAlwaysCheckL2 then
//        begin
//          DefMsg := MakeDefaultMsg(SM_CHECKL2PASSWORD, 0, 0, 0, 0);
//          SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
//          Exit;
//        end
//        else
//        begin
//          if (g_Config.boChangedIPCheckL2) then
//          begin
//            if (ReadUserIP <> 0) and (inet_addr(pchar(UserInfo.sUserIPaddr)) <> ReadUserIP) then
//            begin
//              DefMsg := MakeDefaultMsg(SM_CHECKL2PASSWORD, 0, 0, 0, 0);
//              SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
//              Exit;
//            end;
//          end;
//
//          if (g_Config.boChangedMACCheckL2) then
//          begin
//            if (Length(ReadUserMac) > 0) then
//            begin
//              if not SameText(UserInfo.sLoginMAC, ReadUserMac) then
//              begin
//                DefMsg := MakeDefaultMsg(SM_CHECKL2PASSWORD, 0, 0, 0, 0);
//                SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
//                Exit;
//              end;
//            end;
//          end;
//        end;
//      end;
//    end;
//
//    AccountInfo.LoginIP := inet_addr(pchar(UserInfo.sUserIPaddr));
//    if Length(UserInfo.sLoginMAC) > 0 then
//    begin
//      AccountInfo.LoginMac := UserInfo.sLoginMAC;
//    end;
//    IsUpdated := True;
//
//    UserInfo.boL2PasswordOK := True;
//
//    SessionAdd(UserInfo.sAccount, UserInfo.sUserIPaddr, UserInfo.nSessionID, UserInfo.boPayCost, False);
//
//    if not UserInfo.boPayCost then
//    begin
//      DefMsg := MakeDefaultMsg(SM_PASSOK_SELECTSERVER, 0, 0, 0, g_Config.ServerNameList.Count);
//    end
//    else
//    begin
//      DefMsg := MakeDefaultMsg(SM_PASSOK_SELECTSERVER, nIDCost, Loword(nIPCost), HiWord(nIPCost), g_Config.ServerNameList.Count);
//    end;
//    sServerName := GetServerListInfo(UserInfo.sAccount);
//    SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg) + EncodeString(sServerName));
//
//      // 选择服务器这里不要输入验证码 2021-03-04 19:09:24
//      {
//      if g_Config.boRandomCode[rctLogin] then
//      begin
//        UserInfo.dwClientTick := GetTickCount();
//        UserInfo.nRandomCodeRefreshMaxCount[rctLogin] := 0;
//        //SendRandomCode(UserInfo, rctLogin);
//      end;
//      }
//  end
//  else
//  begin
//    DefMsg := MakeDefaultMsg(SM_PASSWD_FAIL, nCode, 0, 0, 0);
//    SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
//  end;
end;

procedure GetPhoneVerification(UserInfo: pTUserInfo; sData: string);
var
  I: Integer;
  sPhone: string;
  DefMsg: TDefaultMessage;
begin
  sPhone := DecodeString(sData);
  if GetTickCount - UserInfo.dwLastVerificationCodeTick > 60000 then
  begin
    UserInfo.sVerificationCode := '';
    for I := 0 to 5 do
    begin
      UserInfo.sVerificationCode := UserInfo.sVerificationCode + IntToStr(Random(10));
    end;
    UserInfo.sLoginPhone := sPhone;
    HttpPost(0, sPhone, UserInfo.sVerificationCode, '', '', nil, Integer(UserInfo));
//    MainOutMessage('收到手机号：' + sPhone + '等着傻逼周杨把短信接口给我然后发送验证码：' + UserInfo.sVerificationCode);
    UserInfo.dwLastVerificationCodeTick := GetTickCount;
    DefMsg := MakeDefaultMsg(SM_GETVERIFICATION, 1, 0, 0, 0);
    SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg) + EncodeString(UserInfo.sLoginPhone));
  end
  else
  begin
    DefMsg := MakeDefaultMsg(SM_GETVERIFICATION, 0, 60 - (GetTickCount - UserInfo.dwLastVerificationCodeTick) div 1000, 0, 0);
    SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
//    MainOutMessage(sPhone + IntToStr((60000 - (GetTickCount - UserInfo.dwLastVerificationCodeTick)) div 1000) + '秒');
  end;
end;

procedure AccountSelectServer(UserInfo: pTUserInfo; sData: string); //0046B908
var
  sServerName: string;
  DefMsg: TDefaultMessage;
  boPayCost: Boolean;
  nPayMode: Integer;
  sSelGateIP: string;
  nSelGatePort: Integer;
resourcestring
  sSelServerMsg = 'Server: %s/%s-%s:%d';
begin
  sServerName := DecodeString(sData);
//  MainOutMessage('CM_SELECTSERVER:' + sServerName);
  if (UserInfo.sAccount <> '') and (sServerName <> '') and IsLogin(UserInfo.nSessionID) then
  begin
    GetSelGateInfo(sServerName, g_Config.sGateIPaddr, sSelGateIP, nSelGatePort);
    //MainOutMessage('AccountSelectServer 2 ' + sSelGateIP + ':' + IntToStr(nSelGatePort));
    if (sSelGateIP <> '') and (nSelGatePort > 0) then
    begin
      if g_Config.boDynamicIPMode then
        sSelGateIP := UserInfo.sGateIPaddr; //增加支动态IP
      //MainOutMessage('AccountSelectServer 3 '+sSelGateIP+':'+IntToStr(nSelGatePort));
      if g_Config.boShowDetailMsg then
        MainOutMessage(format(sSelServerMsg, [sServerName, g_Config.sGateIPaddr, sSelGateIP, nSelGatePort]));

      UserInfo.boSelServer := True;
      boPayCost := False;
      nPayMode := 5;
      if UserInfo.nIDHour > 0 then
        nPayMode := 2;
      if UserInfo.nIPHour > 0 then
        nPayMode := 4;
      if UserInfo.nIPDay > 0 then
        nPayMode := 3;
      if UserInfo.nIDDay > 0 then
        nPayMode := 1;
      if FrmMasSoc.IsNotUserFull(sServerName) then
      begin
        SessionUpdate(UserInfo.nSessionID, sServerName, boPayCost);
        FrmMasSoc.SendServerMsg(SS_OPENSESSION, sServerName, UserInfo.sAccount + '/' + IntToStr(UserInfo.nSessionID) + '/' + IntToStr(Integer(UserInfo.boPayCost)) + '/' + IntToStr(nPayMode) + '/' + UserInfo.sUserIPaddr);
        DefMsg := MakeDefaultMsg(SM_SELECTSERVER_OK, UserInfo.nSessionID, 0, 0, 0);

        SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg) + EncodeString(sSelGateIP + '/' + IntToStr(nSelGatePort) + '/' + IntToStr(UserInfo.nSessionID) + '/' + UserInfo.sAccount));
      end
      else
      begin
        UserInfo.boSelServer := False;
        SessionDel(UserInfo.nSessionID);
        DefMsg := MakeDefaultMsg(SM_STARTFAIL, 0, 0, 0, 0);
        SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
      end;
    end;
  end;
end;

procedure AccountUpdateUserInfo(UserInfo: pTUserInfo; sData: string);
var
  UserEntry: TUserEntry;
  UserAddEntry: TUserEntryAdd;
  AccountInfo: TAccountInfo;
  nLen: Integer;
  sUserEntryMsg: string;
  sUserAddEntryMsg: string;
  nCode: Integer;
  DefMsg: TDefaultMessage;
begin
  try
    FillChar(UserEntry, SizeOf(TUserEntry), #0);
    FillChar(UserAddEntry, SizeOf(TUserEntryAdd), #0);
    nLen := GetCodeMsgSize(SizeOf(TUserEntry) * 4 / 3);
    sUserEntryMsg := Copy(sData, 1, nLen);
    sUserAddEntryMsg := Copy(sData, nLen + 1, Length(sData) - nLen);
    DecodeString(sUserEntryMsg, @UserEntry, SizeOf(TUserEntry));
    DecodeString(sUserAddEntryMsg, @UserAddEntry, SizeOf(TUserEntryAdd));
    nCode := -1;

    // 不让随意修改信息 2019-09-21 01:20:31
    if (UserInfo.sAccount = UserEntry.sAccount) and CheckAccountName(UserEntry.sAccount) then
    begin
      if g_AccountDB.GetAccount(UserEntry.sAccount, AccountInfo) then
      begin
        AccountInfo.Password := AccountInfo.Password;
        AccountInfo.UserName := UserEntry.sUserName;
        AccountInfo.IDCard := UserEntry.sSSNo;
        AccountInfo.Phone := UserEntry.sPhone;
        AccountInfo.Questions1 := UserEntry.sQuiz;
        AccountInfo.Answers1 := UserEntry.sAnswer;
        AccountInfo.Mail := UserEntry.sEMail;

        AccountInfo.BirthDay := UserAddEntry.sBirthDay;
        AccountInfo.Questions2 := UserAddEntry.sQuiz2;
        AccountInfo.Answers2 := UserAddEntry.sAnswer2;
        AccountInfo.MobilePhone := UserAddEntry.sMobilePhone;

        g_AccountDB.UpdateAccount(AccountInfo, ufAllField);

        WriteLogMsg('upg', AccountInfo);
        nCode := 1;
      end
      else
        nCode := 0;
    end; //0046C74B
    if nCode = 1 then
    begin
      DefMsg := MakeDefaultMsg(SM_UPDATEID_SUCCESS, 0, 0, 0, 0);
    end
    else
    begin
      DefMsg := MakeDefaultMsg(SM_UPDATEID_FAIL, nCode, 0, 0, 0);
    end;
    SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
  except
    MainOutMessage('TFrmMain.UpdateUserInfo');
  end;
end;

procedure AccountGetBackPassword(UserInfo: pTUserInfo; sData: string);
var
  sMsg: string;
  sAccount: string;
  sQuest1: string;
  sAnswer1: string;
  sQuest2: string;
  sAnswer2: string;
  sPassword: string;
  sBirthDay: string;
  sL2Password: string;
  sRandomCode: string;
  nCode: Integer;
  DefMsg: TDefaultMessage;
  AccountInfo: TAccountInfo;
begin
  sMsg := DecodeString(sData);
  sMsg := GetValidStr3(sMsg, sAccount, [#9]);
  sMsg := GetValidStr3(sMsg, sQuest1, [#9]);
  sMsg := GetValidStr3(sMsg, sAnswer1, [#9]);
  sMsg := GetValidStr3(sMsg, sQuest2, [#9]);
  sMsg := GetValidStr3(sMsg, sAnswer2, [#9]);
  sMsg := GetValidStr3(sMsg, sBirthDay, [#9]);

  if (Length(sMsg) > 0) and (sMsg[1] = #9) then
  begin
    sL2Password := '';
    sRandomCode := Copy(sMsg, 2, Length(sMsg) - 1);
  end
  else
  begin
    sMsg := GetValidStr3(sMsg, sL2Password, [#9]);
    sMsg := GetValidStr3(sMsg, sRandomCode, [#9]);
  end;

  // 修改错误为 帐号或答案错误 2019-09-21 02:03:10
  nCode := -1;
  if (sAccount <> '') then
  begin
    if g_AccountDB.GetAccount(sAccount, AccountInfo) and (not AccountInfo.IsDisable) then
    begin
      if (AccountInfo.ErrorCount < 5) or ((GetTickCount - AccountInfo.LastActionTick) > 180000) then
      begin
        // 检测二级密码是否正确 chongchong 2016-11-06
        if g_Config.boEnabledL2Password and (not SameText(AccountInfo.L2Password, sL2Password)) then
        begin
          nCode := -4;
          Inc(AccountInfo.ErrorCount);
          AccountInfo.LastActionTick := GetTickCount();

          g_AccountDB.UpdateAccount(AccountInfo, ufPartField);
        end
        else if g_Config.boRandomCode[rctPwdGetback] and ((Length(UserInfo.sRandomCode[rctPwdGetback]) = 0) or (not SameText(UserInfo.sRandomCode[rctPwdGetback], sRandomCode))) then
        begin
          nCode := -10;

          Inc(AccountInfo.ErrorCount);
          AccountInfo.LastActionTick := GetTickCount();
          g_AccountDB.UpdateAccount(AccountInfo, ufPartField);

          Inc(UserInfo.nRandomCodeErrorMaxCount[rctPwdGetback]);
          if UserInfo.nRandomCodeErrorMaxCount[rctPwdGetback] >= g_Config.nRandomCodeErrorMaxCount then
          begin
            DefMsg := MakeDefaultMsg(SM_RUNGATE_DISABLE_LOGIN, 0, 0, 0, 0);
            SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg) + EncodeString('输入验证码失败次数过多，连接将会断开'));

            // 错误达到最高次数，断开 2020-10-29
            UserInfo.dwDelayCloseTick := GetTickCount + 1000;
            UserInfo.boDealyClose := True;

            Exit;
          end;
        end
        else
        begin
          if not g_Config.boGetbackPasswordCheckAll then
          begin
            if (AccountInfo.Questions1 = sQuest1) then
            begin
              nCode := -1;
              if AccountInfo.Answers1 = sAnswer1 then
              begin
                if AccountInfo.BirthDay = sBirthDay then
                begin
                  nCode := 1;
                end;
              end;
            end;
            if nCode <> 1 then
            begin
              if (AccountInfo.Questions2 = sQuest2) then
              begin
                nCode := -1;
                if AccountInfo.Answers2 = sAnswer2 then
                begin
                  if AccountInfo.BirthDay = sBirthDay then
                  begin
                    nCode := 1;
                  end;
                end;
              end;
            end;
          end
          else
          begin
            nCode := -3;
            if (AccountInfo.Questions1 = sQuest1) and (AccountInfo.Answers1 = sAnswer1) and (AccountInfo.Questions2 = sQuest2) and (AccountInfo.Answers2 = sAnswer2) and (AccountInfo.BirthDay = sBirthDay) then
            begin
              nCode := 1;
            end;
          end;

          if nCode = 1 then
          begin
            sPassword := AccountInfo.Password;
          end
          else
          begin
            Inc(AccountInfo.ErrorCount);
            AccountInfo.LastActionTick := GetTickCount();
            g_AccountDB.UpdateAccount(AccountInfo, ufPartField);
          end;
        end;
      end
      else
      begin
        nCode := -2;
        if GetTickCount < AccountInfo.LastActionTick then
        begin
          AccountInfo.LastActionTick := GetTickCount();
          g_AccountDB.UpdateAccount(AccountInfo, ufPartField);
        end;
      end;
    end;
  end;

  if nCode = 1 then
  begin
    DefMsg := MakeDefaultMsg(SM_GETBACKPASSWD_SUCCESS, 0, 0, 0, 0);
    SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg) + EncodeString(sPassword));

    if g_Config.boRandomCode[rctPwdGetback] then
    begin
      UserInfo.dwClientTick := GetTickCount();
      UserInfo.nRandomCodeRefreshMaxCount[rctPwdGetback] := 0;
      SendRandomCode(UserInfo, rctPwdGetback);
    end;
  end
  else
  begin
    DefMsg := MakeDefaultMsg(SM_GETBACKPASSWD_FAIL, nCode, 0, 0, 0);
    SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));

    if g_Config.boRandomCode[rctPwdGetback] then
    begin
      UserInfo.dwClientTick := GetTickCount();
      UserInfo.nRandomCodeRefreshMaxCount[rctPwdGetback] := 0;
      SendRandomCode(UserInfo, rctPwdGetback);
    end;
  end;
end;

procedure AccountGetPassWordBack(UserInfo: pTUserInfo; sData: string);
var
  sMsg: string;
  sAccount: string;
  sQuest1: string;
  sAnswer1: string;
  sPassword: string;
  nCode: Integer;
  DefMsg: TDefaultMessage;
  AccountInfo: TAccountInfo;
begin
  sMsg := DecodeString(sData);
  sMsg := GetValidStr3(sMsg, sAccount, ['/']);
  sMsg := GetValidStr3(sMsg, sQuest1, ['/']);
  sMsg := GetValidStr3(sMsg, sAnswer1, ['/']);

  // 修改错误为 帐号或答案错误 2019-09-21 02:03:10
  nCode := -1;
  if (sAccount <> '') then
  begin
    if g_AccountDB.GetAccount(sAccount, AccountInfo) and (not AccountInfo.IsDisable) then
    begin
      if (AccountInfo.ErrorCount < 5) or ((GetTickCount - AccountInfo.LastActionTick) > 180000) then
      begin
        begin
          if not g_Config.boGetbackPasswordCheckAll then
          begin
            if (AccountInfo.Questions1 = sQuest1) then
            begin
              nCode := -1;
              if AccountInfo.Answers1 = sAnswer1 then
              begin
                nCode := 1;
              end;
            end;
          end
          else
          begin
            nCode := -3;
            if (AccountInfo.Questions1 = sQuest1) and (AccountInfo.Answers1 = sAnswer1) then
            begin
              nCode := 1;
            end;
          end;

          if nCode = 1 then
          begin
            sPassword := AccountInfo.Password;
          end
          else
          begin
            Inc(AccountInfo.ErrorCount);
            AccountInfo.LastActionTick := GetTickCount();
            g_AccountDB.UpdateAccount(AccountInfo, ufPartField);
          end;
        end;
      end
      else
      begin
        nCode := -2;
        if GetTickCount < AccountInfo.LastActionTick then
        begin
          AccountInfo.LastActionTick := GetTickCount();
          g_AccountDB.UpdateAccount(AccountInfo, ufPartField);
        end;
      end;
    end;
  end;

  if nCode = 1 then
  begin
    DefMsg := MakeDefaultMsg(SM_GETBACKPASSWD_SUCCESS, 1, 0, 0, 0);
    SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg) + EncodeString(sPassword));

    if g_Config.boRandomCode[rctPwdGetback] then
    begin
      UserInfo.dwClientTick := GetTickCount();
      UserInfo.nRandomCodeRefreshMaxCount[rctPwdGetback] := 0;
      SendRandomCode(UserInfo, rctPwdGetback);
    end;
  end
  else
  begin
    DefMsg := MakeDefaultMsg(SM_GETBACKPASSWD_FAIL, nCode, 0, 0, 0);
    SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));

    if g_Config.boRandomCode[rctPwdGetback] then
    begin
      UserInfo.dwClientTick := GetTickCount();
      UserInfo.nRandomCodeRefreshMaxCount[rctPwdGetback] := 0;
      SendRandomCode(UserInfo, rctPwdGetback);
    end;
  end;
end;

procedure AccountGetPassWordBack_Phone(UserInfo: pTUserInfo; sData: string);
var
  sMsg: string;
  sAccount: string;
  sPhone: string;
  sVerification: string;
  sPassword: string;
  nCode: Integer;
  DefMsg: TDefaultMessage;
  AccountInfo: TAccountInfo;
begin
  sMsg := DecodeString(sData);
  sMsg := GetValidStr3(sMsg, sAccount, ['/']);
  sMsg := GetValidStr3(sMsg, sPhone, ['/']);
  sMsg := GetValidStr3(sMsg, sVerification, ['/']);

  // 修改错误为 帐号或答案错误 2019-09-21 02:03:10
  nCode := -1;
  if (sAccount <> '') then
  begin
    if g_AccountDB.GetAccount(sAccount, AccountInfo) and (not AccountInfo.IsDisable) then
    begin
      if (AccountInfo.ErrorCount < 5) or ((GetTickCount - AccountInfo.LastActionTick) > 180000) then
      begin
        begin
          if (UserInfo.sVerificationCode = sVerification) and (AccountInfo.MobilePhone = sPhone) and (UserInfo.sLoginPhone = sPhone) then
          begin
            nCode := 1;
          end;

          if nCode = 1 then
          begin
            sPassword := AccountInfo.Password;
          end
          else
          begin
            Inc(AccountInfo.ErrorCount);
            AccountInfo.LastActionTick := GetTickCount();
            g_AccountDB.UpdateAccount(AccountInfo, ufPartField);
          end;
        end;
      end
      else
      begin
        nCode := -2;
        if GetTickCount < AccountInfo.LastActionTick then
        begin
          AccountInfo.LastActionTick := GetTickCount();
          g_AccountDB.UpdateAccount(AccountInfo, ufPartField);
        end;
      end;
    end;
  end;
  UserInfo.sLoginPhone := '';
  UserInfo.sVerificationCode := '';
  if nCode = 1 then
  begin
    DefMsg := MakeDefaultMsg(SM_GETBACKPASSWD_SUCCESS, 2, 0, 0, 0);
    SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg) + EncodeString(sPassword));

    if g_Config.boRandomCode[rctPwdGetback] then
    begin
      UserInfo.dwClientTick := GetTickCount();
      UserInfo.nRandomCodeRefreshMaxCount[rctPwdGetback] := 0;
      SendRandomCode(UserInfo, rctPwdGetback);
    end;
  end
  else
  begin
    DefMsg := MakeDefaultMsg(SM_GETBACKPASSWD_FAIL, nCode, 0, 0, 0);
    SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));

    if g_Config.boRandomCode[rctPwdGetback] then
    begin
      UserInfo.dwClientTick := GetTickCount();
      UserInfo.nRandomCodeRefreshMaxCount[rctPwdGetback] := 0;
      SendRandomCode(UserInfo, rctPwdGetback);
    end;
  end;
end;

procedure AccountSetL2Password(IsFirstSet: Boolean; UserInfo: pTUserInfo; sData: string);
var
  AccountInfo: TAccountInfo;
  DefMsg: TDefaultMessage;
  IsUpdated: Boolean;
  S: string;
begin
  if UserInfo.nSessionID <= 0 then
    Exit;
  if not g_AccountDB.GetAccount(UserInfo.sAccount, AccountInfo) then
    Exit;

  if Length(AccountInfo.L2Password) > 0 then
  begin
    DefMsg := MakeDefaultMsg(SM_SETL2PASSWORD_RESULT, 0, 0, 0, 0);

    SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));

    UserInfo.dwDelayCloseTick := GetTickCount + 1000;
    UserInfo.boDealyClose := True;

    Exit;
  end;

  IsUpdated := False;

  {自动解除锁定账号}
  if (g_Config.boUnLockAccount) and (AccountInfo.ErrorCount >= 5)
    and ((GetTickCount - AccountInfo.LastActionTick) >= Cardinal(g_Config.dwUnLockAccountTime * 60 * 1000)) then begin
    AccountInfo.ErrorCount := 0;
    AccountInfo.LastActionTick := GetTickCount - 70000;
    IsUpdated := True;
  end;

  // 密码为空
  if Length(sData) = 0 then
  begin
    UserInfo.nL2ErrorCount := UserInfo.nL2ErrorCount + 1;

    if UserInfo.nL2ErrorCount >= 2 then
    begin
      DefMsg := MakeDefaultMsg(SM_SETL2PASSWORD_RESULT, 0, 0, 0, 0);

      SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));

      UserInfo.dwDelayCloseTick := GetTickCount + 1000;
      UserInfo.boDealyClose := True;
    end
    else
    begin
      DefMsg := MakeDefaultMsg(SM_SETL2PASSWORD, 0, DefMsg.Param, 0, 0);

      SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
    end;

    if IsUpdated then
      g_AccountDB.UpdateAccount(AccountInfo, ufPartField);
    Exit;
  end;

  // 第一次输入二级密码
  if IsFirstSet then
  begin
    S := DecodeString(sData);

    // 禁止帐户和二级密码一致
    if g_Config.boDisableIDSameL2Password and SameText(UserInfo.sAccount, S) then
    begin
      DefMsg := MakeDefaultMsg(SM_SETL2PASSWORD, 0, 3, 0, 0);

      SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));

      Inc(AccountInfo.ErrorCount);
      IsUpdated := True;
    end
    else if g_Config.boDisableL2SamePassword and SameText(AccountInfo.Password, S) then
    begin
      DefMsg := MakeDefaultMsg(SM_SETL2PASSWORD, 0, 4, 0, 0);

      SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));

      Inc(AccountInfo.ErrorCount);
      IsUpdated := True;
    end
    else if not CheckStringValid(S) then
    begin
      DefMsg := MakeDefaultMsg(SM_SETL2PASSWORD, 0, 5, 0, 0);

      SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));

      Inc(AccountInfo.ErrorCount);
      IsUpdated := True;
    end
    else
    begin
      UserInfo.sL2Password := S;
      DefMsg := MakeDefaultMsg(SM_SETL2PASSWORD, 0, 1, 0, 0);

      SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
    end;
  end
  else
  begin
    // 二级密码设置成功
    if SameText(UserInfo.sL2Password, DecodeString(sData)) then
    begin
      AccountInfo.ErrorCount := 0;
      AccountInfo.LastActionTick := GetTickCount - 70000;
      AccountInfo.L2Password := UserInfo.sL2Password; // update...
      AccountInfo.LoginIP := inet_addr(pchar(UserInfo.sUserIPaddr));

      if Length(UserInfo.sLoginMAC) > 0 then
      begin
        Move(UserInfo.sLoginMAC[1], AccountInfo.LoginMac[1], Length(UserInfo.sLoginMAC));
      end;

      IsUpdated := True;

      DefMsg := MakeDefaultMsg(SM_SETL2PASSWORD_RESULT, 0, 1, 0, 0);
      SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));

      UserInfo.boL2PasswordOK := True;

      SessionAdd(UserInfo.sAccount, UserInfo.sUserIPaddr, UserInfo.nSessionID, UserInfo.boPayCost, False);

      if not UserInfo.boPayCost then
      begin
        DefMsg := MakeDefaultMsg(SM_PASSOK_SELECTSERVER, 0, 0, 0, g_Config.ServerNameList.Count);
      end
      else
      begin
        DefMsg := MakeDefaultMsg(SM_PASSOK_SELECTSERVER, MakeLong(Word(UserInfo.nIDDay), Word(UserInfo.nIDHour)), UserInfo.nIPDay, UserInfo.nIPHour, g_Config.ServerNameList.Count);
      end;
      S := GetServerListInfo;
      SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg) + EncodeString(S));
    end    // 第2次二级密码和第一次不一致
    else
    begin
      UserInfo.nL2ErrorCount := UserInfo.nL2ErrorCount + 1;

      if UserInfo.nL2ErrorCount >= 2 then
      begin
        DefMsg := MakeDefaultMsg(SM_SETL2PASSWORD_RESULT, 0, 0, 0, 0);

        SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));

        UserInfo.dwDelayCloseTick := GetTickCount + 1000;
        UserInfo.boDealyClose := True;
      end
      else
      begin
        DefMsg := MakeDefaultMsg(SM_SETL2PASSWORD, 0, 2, 0, 0);

        SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
      end;
    end;
  end;

  if IsUpdated then
    g_AccountDB.UpdateAccount(AccountInfo, ufPartField);
end;

procedure AccountCheckL2Password(UserInfo: pTUserInfo; sData: string);
var
  AccountInfo: TAccountInfo;
  DefMsg: TDefaultMessage;
  sServerName: string;
begin
  if UserInfo.nSessionID <= 0 then
    Exit;
  if not g_AccountDB.GetAccount(UserInfo.sAccount, AccountInfo) then
    Exit;

  if Length(AccountInfo.L2Password) = 0 then
  begin
    UserInfo.boL2PasswordOK := True;

    SessionAdd(UserInfo.sAccount, UserInfo.sUserIPaddr, UserInfo.nSessionID, UserInfo.boPayCost, False);

    if not UserInfo.boPayCost then
    begin
      DefMsg := MakeDefaultMsg(SM_PASSOK_SELECTSERVER, 0, 0, 0, g_Config.ServerNameList.Count);
    end
    else
    begin
      DefMsg := MakeDefaultMsg(SM_PASSOK_SELECTSERVER, MakeLong(Word(UserInfo.nIDDay), Word(UserInfo.nIDHour)), UserInfo.nIPDay, UserInfo.nIPHour, g_Config.ServerNameList.Count);
    end;
    sServerName := GetServerListInfo;
    SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg) + EncodeString(sServerName));

    Exit;
  end;

  if SameText(AccountInfo.L2Password, DecodeString(sData)) then
  begin
    AccountInfo.ErrorCount := 0;
    AccountInfo.LoginIP := inet_addr(pchar(UserInfo.sUserIPaddr));
    if Length(UserInfo.sLoginMAC) > 0 then
    begin
      Move(UserInfo.sLoginMAC[1], AccountInfo.LoginMac[1], Length(UserInfo.sLoginMAC));
    end;

    g_AccountDB.UpdateAccount(AccountInfo, ufPartField);

    UserInfo.boL2PasswordOK := True;

    SessionAdd(UserInfo.sAccount, UserInfo.sUserIPaddr, UserInfo.nSessionID, UserInfo.boPayCost, False);

    if not UserInfo.boPayCost then
    begin
      DefMsg := MakeDefaultMsg(SM_PASSOK_SELECTSERVER, 0, 0, 0, g_Config.ServerNameList.Count);
    end
    else
    begin
      DefMsg := MakeDefaultMsg(SM_PASSOK_SELECTSERVER, MakeLong(Word(UserInfo.nIDDay), Word(UserInfo.nIDHour)), UserInfo.nIPDay, UserInfo.nIPHour, g_Config.ServerNameList.Count);
    end;
    sServerName := GetServerListInfo;
    SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg) + EncodeString(sServerName));
  end
  else
  begin
    AccountInfo.ErrorCount := AccountInfo.ErrorCount + 2;
    AccountInfo.LastActionTick := GetTickCount;

    g_AccountDB.UpdateAccount(AccountInfo, ufPartField);

    if AccountInfo.ErrorCount >= 5 then
    begin
      DefMsg := MakeDefaultMsg(SM_PASSWD_FAIL, -2, 0, 0, 0);
      SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
    end
    else
    begin
      DefMsg := MakeDefaultMsg(SM_CHECKL2PASSWORD, 0, 1, 0, 0);
      SendGateMsg(UserInfo.Socket, UserInfo.sSockIndex, EncodeMessage(DefMsg));
    end;
  end;
end;

procedure SendGateMsg(Socket: TCustomWinSocket; sSockIndex, sMsg: string);
var
  sSendMsg: string;
begin
  sSendMsg := '%' + sSockIndex + '/#' + sMsg + '!$';
  Socket.SendText(sSendMsg);
end;

function IsLogin(nSessionID: Integer): Boolean;
var
  ConnInfo: pTConnInfo;
  I: Integer;
begin
  Result := False;
  g_Config.SessionList.Lock;
  try
    for I := 0 to g_Config.SessionList.Count - 1 do
    begin
      ConnInfo := g_Config.SessionList.Items[I];
      if (ConnInfo.nSessionID = nSessionID) and (not ConnInfo.boKicked) then
      begin
        Result := True;
        break;
      end;
    end;
  finally
    g_Config.SessionList.UnLock;
  end;
end;

function IsLogin(sLoginID: string): Boolean;
var
  ConnInfo: pTConnInfo;
  I: Integer;
begin
  Result := False;
  g_Config.SessionList.Lock;
  try
    for I := 0 to g_Config.SessionList.Count - 1 do
    begin
      ConnInfo := g_Config.SessionList.Items[I];
      if (ConnInfo.sAccount = sLoginID) and (not ConnInfo.boKicked) then
      begin
        Result := True;
        break;
      end;
    end;
  finally
    g_Config.SessionList.UnLock;
  end;
end;

procedure SessionKick(sLoginID: string);
var
  ConnInfo: pTConnInfo;
  I: Integer;
begin
  g_Config.SessionList.Lock;
  try
    for I := 0 to g_Config.SessionList.Count - 1 do
    begin
      ConnInfo := g_Config.SessionList.Items[I];
      if (ConnInfo.sAccount = sLoginID) and (not ConnInfo.boKicked) then
      begin
        FrmMasSoc.SendServerMsg(SS_CLOSESESSION, ConnInfo.sServerName, ConnInfo.sAccount + '/' + IntToStr(ConnInfo.nSessionID));
        ConnInfo.dwKickTick := GetTickCount();
        ConnInfo.boKicked := True;
      end;
    end;
  finally
    g_Config.SessionList.UnLock;
  end;
end;

procedure SessionAdd(sAccount, sIPaddr: string; nSessionID: Integer; boPayCost, bo11: Boolean);
var
  ConnInfo: pTConnInfo;
begin
  New(ConnInfo);
  ConnInfo.sAccount := sAccount;
  ConnInfo.sIPaddr := sIPaddr;
  ConnInfo.nSessionID := nSessionID;
  ConnInfo.boPayCost := boPayCost;
  ConnInfo.bo11 := bo11;
  ConnInfo.dwKickTick := GetTickCount();
  ConnInfo.dwStartTick := GetTickCount();
  ConnInfo.boKicked := False;
  g_Config.SessionList.Lock;
  try
    g_Config.SessionList.Add(ConnInfo);
  finally
    g_Config.SessionList.UnLock;
  end;
  //MainOutMessage('SessionAdd ' + sAccount + ' nSessionID:' + IntToStr(nSessionID) + ' SessionList:' + IntToStr(Integer(Config.SessionList)) + ' Count:' + IntToStr(Config.SessionList.Count));
end;

procedure SessionAdd(sAccount, sIPaddr, sServerName: string; nSessionID: Integer; boPayCost, bo11: Boolean);
var
  ConnInfo: pTConnInfo;
begin
  New(ConnInfo);
  ConnInfo.sAccount := sAccount;
  ConnInfo.sIPaddr := sIPaddr;
  ConnInfo.nSessionID := nSessionID;
  ConnInfo.boPayCost := boPayCost;
  ConnInfo.bo11 := bo11;
  ConnInfo.dwKickTick := GetTickCount();
  ConnInfo.dwStartTick := GetTickCount();
  ConnInfo.boKicked := False;
  ConnInfo.sServerName := sServerName;
  g_Config.SessionList.Lock;
  try
    g_Config.SessionList.Add(ConnInfo);
  finally
    g_Config.SessionList.UnLock;
  end;
end;

procedure SendGateKickMsg(Socket: TCustomWinSocket; sSockIndex: string);
var
  sSendMsg: string;
begin
  sSendMsg := '%+-' + sSockIndex + '$';
  Socket.SendText(sSendMsg);
end;

procedure SessionUpdate(nSessionID: Integer; sServerName: string; boPayCost: Boolean);
var
  ConnInfo: pTConnInfo;
  I: Integer;
begin
  g_Config.SessionList.Lock;
  try
    for I := 0 to g_Config.SessionList.Count - 1 do
    begin
      ConnInfo := g_Config.SessionList.Items[I];
      if (ConnInfo.nSessionID = nSessionID) and not ConnInfo.boKicked then
      begin
        ConnInfo.sServerName := sServerName;
        ConnInfo.bo11 := boPayCost;
        break;
      end;
    end;
  finally
    g_Config.SessionList.UnLock;
  end;
end;

procedure GenServerNameList();
var
  I, II: Integer;
  boD: Boolean;
begin
  try
    g_Config.ServerNameList.Clear;
    for I := 0 to g_Config.nRouteCount - 1 do
    begin
      boD := True;
      for II := 0 to g_Config.ServerNameList.Count - 1 do
      begin
        if g_Config.ServerNameList.Strings[II] = g_Config.GateRoute[I].sServerName then
          boD := False;
      end;
      if boD then
        g_Config.ServerNameList.Add(g_Config.GateRoute[I].sServerName);
    end;
  except
    MainOutMessage('TFrmMain.GenServerNameList');
  end;
end;
//00469DB4

procedure SessionClearNoPayMent();
var
  I: Integer;
  ConnInfo: pTConnInfo;
begin
  g_Config.SessionList.Lock;
  try
    for I := g_Config.SessionList.Count - 1 downto 0 do
    begin
      ConnInfo := g_Config.SessionList.Items[I];
      if (not ConnInfo.boKicked) and (not g_Config.boTestServer) and (not ConnInfo.bo11) then
      begin
        if (GetTickCount - ConnInfo.dwStartTick) > 60 * 60 * 1000 then
        begin
          ConnInfo.dwStartTick := GetTickCount();
          if not IsPayMent(ConnInfo.sIPaddr, ConnInfo.sAccount) then
          begin
            FrmMasSoc.SendServerMsg(SS_KICKUSER, ConnInfo.sServerName, ConnInfo.sAccount + '/' + IntToStr(ConnInfo.nSessionID));
            //MainOutMessage('SessionClearNoPayMent ' + ConnInfo.sAccount + ' ' + IntToStr(ConnInfo.nSessionID));
            Dispose(ConnInfo);
            g_Config.SessionList.Delete(I);
          end;
        end;
      end;
    end;
  finally
    g_Config.SessionList.UnLock;
  end;
end;

procedure LoadIPaddrCostList(QuickList: TQuickList);
begin
  try
    CS_DB.Enter;
    g_Config.IPaddrCostList.Clear;
    g_Config.IPaddrCostList.AddStrings(QuickList);
  finally
    CS_DB.Leave;
  end;
end;

procedure LoadAccountCostList(QuickList: TQuickList);
begin
  try
    CS_DB.Enter;
    g_Config.AccountCostList.Clear;
    g_Config.AccountCostList.AddStrings(QuickList);
  finally
    CS_DB.Leave;
  end;
end;

procedure TFrmMain.Panel2DblClick(Sender: TObject);
begin
  MainOutMessage(GetServerListInfo)
end;

procedure TFrmMain.MyMessage(var MsgData: TWmCopyData);
var
  sData: string;
  wIdent: Word;
begin
  wIdent := HiWord(MsgData.From);
  sData := StrPas(MsgData.CopyDataStruct^.lpData);
  case wIdent of //
    GS_QUIT:
      begin
        g_Config.boRemoteClose := True;
        Close();
      end;
    GS_USERACCOUNT:
      begin
        GameCenterGetUserAccount(sData);
      end;
    GS_CHANGEACCOUNTINFO:
      begin
        GameCenterChangeAccountInfo(sData);
      end;
    3:
      ;
  end;
end;

procedure TFrmMain.GameCenterGetUserAccount(sData: string);
var
  AccountInfo: TAccountInfo;
  DefMsg: TDefaultMessage;
begin
  if g_AccountDB.GetAccount(sData, AccountInfo) then
  begin
    DefMsg := MakeDefaultMsg(0, 1, 0, 0, 0);
    SendGameCenterMsg(SG_USERACCOUNT, EncodeMessage(DefMsg) + EncodeBuffer(@AccountInfo, SizeOf(AccountInfo)));
  end
  else
  begin
    DefMsg := MakeDefaultMsg(SG_USERACCOUNTNOTFOUND, -1, 0, 0, 0);
    SendGameCenterMsg(SG_USERACCOUNT, EncodeMessage(DefMsg));
  end;
end;

procedure TFrmMain.GameCenterChangeAccountInfo(sData: string);
var
  NewRecord, AccountInfo: TAccountInfo;
  DefMsg: TDefaultMessage;
  sDefMsg: string;
  nCode: integer;
begin
  if Length(sData) < DEF_BLOCK_SIZE then
    Exit;
  sDefMsg := Copy(sData, 1, DEF_BLOCK_SIZE);
  sData := Copy(sData, DEF_BLOCK_SIZE + 1, Length(sData) - DEF_BLOCK_SIZE);

  DefMsg := DecodeMessage(sDefMsg);
  DecodeString(sData, @NewRecord, SizeOf(NewRecord));
  nCode := -1;

  if g_AccountDB.GetAccount(NewRecord.AccountName, AccountInfo) then
  begin
    AccountInfo.ErrorCount := 0;
    AccountInfo.LastActionTick := 0;

    AccountInfo.Password := NewRecord.Password;
    AccountInfo.UserName := NewRecord.UserName;
    AccountInfo.IDCard := NewRecord.IDCard;
    AccountInfo.BirthDay := NewRecord.BirthDay;
    AccountInfo.Questions1 := NewRecord.Questions1;
    AccountInfo.Answers1 := NewRecord.Answers1;
    AccountInfo.Questions2 := NewRecord.Questions2;
    AccountInfo.Answers2 := NewRecord.Answers2;
    AccountInfo.Phone := NewRecord.Phone;
    AccountInfo.MobilePhone := NewRecord.MobilePhone;
    AccountInfo.Mail := NewRecord.Mail;
    AccountInfo.L2Password := NewRecord.L2Password;
    AccountInfo.Memo := NewRecord.Memo;

    if g_AccountDB.UpdateAccount(AccountInfo, ufAllField) then
      nCode := 1
    else
      nCode := 2;
  end;

  DefMsg := MakeDefaultMsg(0, nCode, 0, 0, 0);
  SendGameCenterMsg(SG_USERACCOUNTCHANGESTATUS, EncodeMessage(DefMsg));
end;

procedure SaveContLogMsg(sLogMsg: string);
var
  Year, Month, Day, Hour, Min, Sec, MSec: Word;
  sLogDir, sLogFileName: string;
  LogFile: TextFile;
begin
  if sLogMsg = '' then
    exit;

  DecodeDate(Date, Year, Month, Day);
  DecodeTime(Time, Hour, Min, Sec, MSec);
  if not DirectoryExists(g_Config.sCountLogDir) then
  begin
    CreateDir(g_Config.sCountLogDir);
  end;
  sLogDir := g_Config.sCountLogDir + IntToStr(Year) + '-' + IntToStr2(Month);
  if not DirectoryExists(sLogDir) then
  begin
    CreateDirectory(PChar(sLogDir), nil);
  end;
  sLogFileName := sLogDir + '\' + IntToStr(Year) + '-' + IntToStr2(Month) + '-' + IntToStr2(Day) + '.txt';
  AssignFile(LogFile, sLogFileName);
  if not FileExists(sLogFileName) then
  begin
    Rewrite(LogFile);
  end
  else
  begin
    Append(LogFile);
  end;
  sLogMsg := sLogMsg + #9 + TimeToStr(Time);

  WriteLn(LogFile, sLogMsg);
  CloseFile(LogFile);
end;

procedure WriteLogMsg(sType: string; AccountInfo: TAccountInfo);
var
  Year, Month, Day: Word;
  sLogDir, sLogFileName: string;
  LogFile: TextFile;
  sLogFormat, sLogMsg: string;
begin

  DecodeDate(Date, Year, Month, Day);
  if not DirectoryExists(g_Config.sChrLogDir) then
  begin
    CreateDir(g_Config.sChrLogDir);
  end;
  sLogDir := g_Config.sChrLogDir + IntToStr(Year) + '-' + IntToStr2(Month);
  if not DirectoryExists(sLogDir) then
  begin
    CreateDirectory(PChar(sLogDir), nil);
  end;
  sLogFileName := sLogDir + '\Id_' + IntToStr2(Day) + '.log';
  AssignFile(LogFile, sLogFileName);
  if not FileExists(sLogFileName) then
  begin
    Rewrite(LogFile);
  end
  else
  begin
    Append(LogFile);
  end;
  sLogFormat := '*%s*'#9'%s'#9'"%s"'#9'%s'#9'%s'#9'%s'#9'%s'#9'%s'#9'%s'#9'%s'#9'%s'#9'%s'#9'[%s]';
  sLogMsg := Format(sLogFormat, [sType, AccountInfo.AccountName, AccountInfo.Password, AccountInfo.UserName, AccountInfo.IDCard, AccountInfo.Questions1, AccountInfo.Answers1, AccountInfo.Mail, AccountInfo.Questions2, AccountInfo.Answers2, AccountInfo.BirthDay, AccountInfo.MobilePhone, TimeToStr(Now)]);

  //sLogMsg:= UserAddEntry.sQuiz2 + UserAddEntry.sAnswer2 + UserAddEntry.sBirthDay + UserAddEntry.sMobilePhone + '[' + TimeToStr(Now) + ']';

  WriteLn(LogFile, sLogMsg);
  CloseFile(LogFile);
end;

procedure StartService();
begin
  InitializeConfig();
  LoadConfig();
end;

procedure StopService();
begin
  UnInitializeConfig();
end;

procedure InitializeConfig();
resourcestring
  sConfigFile = 'Logsrv.ini';
begin
  g_Config.IniConf := TIniFile.Create(ExtractFilePath(ParamStr(0)) + sConfigFile);
  InitializeCriticalSection(g_Config.GateCriticalSection);
end;

procedure UnInitializeConfig();
begin
  g_Config.IniConf.Free;
  DeleteCriticalSection(g_Config.GateCriticalSection);
end;

procedure LoadConfig();

  function LoadConfigString(sSection, sIdent, sDefault: string): string;
  var
    sString: string;
  begin
    sString := g_Config.IniConf.ReadString(sSection, sIdent, '');
    if sString = '' then
    begin
      g_Config.IniConf.WriteString(sSection, sIdent, sDefault);
      Result := sDefault;
    end
    else
    begin
      Result := sString;
    end;
  end;

  function LoadConfigInteger(sSection, sIdent: string; nDefault: Integer): Integer;
  var
    nLoadInteger: Integer;
  begin
    nLoadInteger := g_Config.IniConf.ReadInteger(sSection, sIdent, -1);
    if nLoadInteger < 0 then
    begin
      g_Config.IniConf.WriteInteger(sSection, sIdent, nDefault);
      Result := nDefault;
    end
    else
    begin
      Result := nLoadInteger;
    end;
  end;

  function LoadConfigBoolean(sSection, sIdent: string; boDefault: Boolean): Boolean;
  var
    nLoadInteger: Integer;
  begin
    nLoadInteger := g_Config.IniConf.ReadInteger(sSection, sIdent, -1);
    if nLoadInteger < 0 then
    begin
      g_Config.IniConf.WriteBool(sSection, sIdent, boDefault);
      Result := boDefault;
    end
    else
    begin
      Result := nLoadInteger = 1;
    end;
  end;

var
  RandCodeType: TRandCodeType;
begin
  g_Config.sDBServer := LoadConfigString(sSectionServer, sIdentDBServer, g_Config.sDBServer);
  g_Config.sFeeServer := LoadConfigString(sSectionServer, sIdentFeeServer, g_Config.sFeeServer);
  g_Config.sLogServer := LoadConfigString(sSectionServer, sIdentLogServer, g_Config.sLogServer);

  g_Config.sGateAddr := LoadConfigString(sSectionServer, sIdentGateAddr, g_Config.sGateAddr);
  g_Config.nGatePort := LoadConfigInteger(sSectionServer, sIdentGatePort, g_Config.nGatePort);
  g_Config.sServerAddr := LoadConfigString(sSectionServer, sIdentServerAddr, g_Config.sServerAddr);
  g_Config.sServerName := LoadConfigString(sSectionServer, sIdentServerName, g_Config.sServerName);
  g_Config.nServerPort := LoadConfigInteger(sSectionServer, sIdentServerPort, g_Config.nServerPort);
  g_Config.sMonAddr := LoadConfigString(sSectionServer, sIdentMonAddr, g_Config.sMonAddr);
  g_Config.nMonPort := LoadConfigInteger(sSectionServer, sIdentMonPort, g_Config.nMonPort);

  g_Config.sControlPassword := LoadConfigString(sSectionServer, sIdentControlPassword, g_Config.sControlPassword);
  g_Config.nControlPort := LoadConfigInteger(sSectionServer, sIdentControlPort, g_Config.nControlPort);

  g_Config.boShowBlockIPLog := LoadConfigBoolean(sSectionServer, sIdentShowBlockIPLog, g_Config.boShowBlockIPLog);

  g_Config.nDBSPort := LoadConfigInteger(sSectionServer, sIdentDBSPort, g_Config.nDBSPort);
  g_Config.nFeePort := LoadConfigInteger(sSectionServer, sIdentFeePort, g_Config.nFeePort);
  g_Config.nLogPort := LoadConfigInteger(sSectionServer, sIdentLogPort, g_Config.nLogPort);
  g_Config.nReadyServers := LoadConfigInteger(sSectionServer, sIdentReadyServers, g_Config.nReadyServers);
  g_Config.boEnableMakingID := LoadConfigBoolean(sSectionServer, sIdentEnableMakingID, g_Config.boEnableMakingID);
  g_Config.boTestServer := LoadConfigBoolean(sSectionServer, sIdentTestServer, g_Config.boTestServer);

  g_Config.boEnableGetbackPassword := LoadConfigBoolean(sSectionServer, sIdentEnableGetbackPassword, g_Config.boEnableGetbackPassword);
  g_Config.boGetbackPasswordCheckAll := LoadConfigBoolean(sSectionServer, sIdentGetbackPasswordCheckAll, g_Config.boGetbackPasswordCheckAll);
  g_Config.boDisableIDSamePassword := LoadConfigBoolean(sSectionServer, sIdentDisableIDSamePassword, g_Config.boDisableIDSamePassword);
  g_Config.boDisableQuizSameAnswer := LoadConfigBoolean(sSectionServer, sIdentDisableQuizSameAnswer, g_Config.boDisableQuizSameAnswer);

  g_Config.boDisableIDSameL2Password := LoadConfigBoolean(sSectionServer, sIdentDisableIDSameL2Password, g_Config.boDisableIDSameL2Password);
  g_Config.boDisableL2SamePassword := LoadConfigBoolean(sSectionServer, sIdentDisableL2SamePassword, g_Config.boDisableL2SamePassword);

  g_Config.boDisablePwdSameChr := LoadConfigBoolean(sSectionServer, sIdentDisablePwdSameChr, g_Config.boDisablePwdSameChr);
  g_Config.boDisablePwdAllNum := LoadConfigBoolean(sSectionServer, sIdentDisablePwdAllNum, g_Config.boDisablePwdAllNum);
  g_Config.boDisablePwdAllLetter := LoadConfigBoolean(sSectionServer, sIdentDisablePwdAllLetter, g_Config.boDisablePwdAllLetter);

  g_Config.boAutoClearID := LoadConfigBoolean(sSectionServer, sIdentAutoClearID, g_Config.boAutoClearID);
  g_Config.dwAutoClearTime := LoadConfigInteger(sSectionServer, sIdentAutoClearTime, g_Config.dwAutoClearTime);
  g_Config.boUnLockAccount := LoadConfigBoolean(sSectionServer, sIdentUnLockAccount, g_Config.boUnLockAccount);
  g_Config.dwUnLockAccountTime := LoadConfigInteger(sSectionServer, sIdentUnLockAccountTime, g_Config.dwUnLockAccountTime);

  // 修复动态IP支持 piaoyun 2013-08-30
  //g_Config.boDynamicIPMode := False;
  g_Config.boDynamicIPMode := LoadConfigBoolean(sSectionServer, sIdentDynamicIPMode, g_Config.boDynamicIPMode);

  for RandCodeType := Low(TRandCodeType) to High(TRandCodeType) do
  begin
    g_Config.boRandomCode[RandCodeType] := LoadConfigBoolean(sSectionServer, sIdentRandomCode[RandCodeType], g_Config.boRandomCode[RandCodeType]);
  end;

  g_Config.btLoginWaveValue := LoadConfigInteger(sSectionServer, sIdentLoginWaveValue, g_Config.btLoginWaveValue);
  g_Config.btOtherWaveValue := LoadConfigInteger(sSectionServer, sIdentOtherWaveValue, g_Config.btOtherWaveValue);

  g_Config.nRandomCodeErrorMaxCount := LoadConfigInteger(sSectionServer, sIdentRandomCodeErrorMaxCount, g_Config.nRandomCodeErrorMaxCount);
  g_Config.nRandomCodeRefreshMaxCount := LoadConfigInteger(sSectionServer, sIdentRandomCodeRefreshMaxCount, g_Config.nRandomCodeRefreshMaxCount);

  g_Config.sIdDir := LoadConfigString(sSectionDB, sIdentIdDir, g_Config.sIdDir);
  g_Config.sWebLogDir := LoadConfigString(sSectionDB, sIdentWebLogDir, g_Config.sWebLogDir);
  g_Config.sCountLogDir := LoadConfigString(sSectionDB, sIdentCountLogDir, g_Config.sCountLogDir);
  g_Config.sFeedIDList := LoadConfigString(sSectionDB, sIdentFeedIDList, g_Config.sFeedIDList);
  g_Config.sFeedIPList := LoadConfigString(sSectionDB, sIdentFeedIPList, g_Config.sFeedIPList);

  g_Config.boEnabledL2Password := LoadConfigInteger(sSectionServer, sIdentEnabledL2Password, 0) <> 0;
  g_Config.boChangedMACCheckL2 := LoadConfigBoolean(sSectionServer, sIdentChangedMACCheckL2, g_Config.boChangedMACCheckL2);
  g_Config.boChangedIPCheckL2 := LoadConfigBoolean(sSectionServer, sIdentChangedIPCheckL2, g_Config.boChangedIPCheckL2);
  g_Config.boAlwaysCheckL2 := LoadConfigBoolean(sSectionServer, sIdentAlwaysCheckL2, g_Config.boAlwaysCheckL2);

  g_Config.nDataSaveDBType := LoadConfigInteger(sSectionDataSaveDB, sIdentDataSaveDBType, g_Config.nDataSaveDBType);
  g_Config.sDataSaveDBServer := LoadConfigString(sSectionDataSaveDB, sIdentDataSaveDBServer, g_Config.sDataSaveDBServer);
  g_Config.wDataSaveDBPort := LoadConfigInteger(sSectionDataSaveDB, sIdentDataSaveDBPort, g_Config.wDataSaveDBPort);
  g_Config.sDataSaveDBUser := LoadConfigString(sSectionDataSaveDB, sIdentDataSaveDBUser, g_Config.sDataSaveDBUser);
  g_Config.sDataSaveDBPassword := LoadConfigString(sSectionDataSaveDB, sIdentDataSaveDBPassword, g_Config.sDataSaveDBPassword);
  g_Config.sDataSaveDataBase := LoadConfigString(sSectionDataSaveDB, sIdentDataSaveDataBase, g_Config.sDataSaveDataBase);

  g_Config.boNewLoginDlg := LoadConfigBoolean(sSectionServer, sIdentNewLoginDlg, g_Config.boNewLoginDlg);
  g_Config.boNewLoginInto := LoadConfigBoolean(sSectionServer, sIdentNewLoginInto, g_Config.boNewLoginInto);
  g_Config.boNewLoginPhone := LoadConfigBoolean(sSectionServer, sIdentNewLoginPhone, g_Config.boNewLoginPhone);
  g_Config.boNewLoginMustHasPhone := LoadConfigBoolean(sSectionServer, sIdentNewLoginMustHasPhone, g_Config.boNewLoginMustHasPhone);

end;

procedure TFrmMain.MENU_HELP_VERSIONClick(Sender: TObject);
begin
  //MainOutMessage(g_sUpDateTime);
  MainOutMessage(g_sProductName);
  MainOutMessage(g_sProgram);
  MainOutMessage(g_sWebSite);
end;

procedure TFrmMain.MENU_OPTION_GENERALClick(Sender: TObject);
begin
  FrmBasicSet := TFrmBasicSet.Create(Self);
  FrmBasicSet.OpenBasicSet();

  if g_Config.nControlPort <> ServerSocketControl.Port then
  begin
    ServerSocketControl.Active := False;
    ServerSocketControl.Address := '0.0.0.0';
    ServerSocketControl.Port := g_Config.nControlPort;
    ServerSocketControl.Active := True;

    FillChar(g_ControlSessionArray, SizeOf(g_ControlSessionArray), 0);

    Memo1.Lines.Add('远程控制端口[' + IntToStr(g_Config.nControlPort) + ']监视成功...');
  end;

  FrmBasicSet.Free;
end;

procedure TFrmMain.ServerSocketControlAccept(Sender: TObject; Socket: TCustomWinSocket);
var
  RemoteAddr: string;
  IsPassIP: Boolean;
begin
  RemoteAddr := Socket.RemoteAddress;

  g_ControlIPList.Lock;
  try
    if g_ControlIPList.Count = 0 then
    begin
      IsPassIP := True;
    end
    else
    begin
      IsPassIP := g_ControlIPList.IndexOf(RemoteAddr) >= 0;
    end;
  finally
    g_ControlIPList.UnLock;
  end;

  if not IsPassIP then
  begin
    MainOutMessage('远程控制过滤连接: ' + RemoteAddr);
    Socket.Close;
  end;
end;

procedure TFrmMain.ServerSocketControlClientConnect(Sender: TObject; Socket: TCustomWinSocket);
var
  I: Integer;
  UserSession: PControlSessionInfo;
begin
  Socket.nIndex := -1;
  for I := 0 to Length(g_ControlSessionArray) - 1 do
  begin
    UserSession := @g_ControlSessionArray[I];
    if UserSession.Socket = nil then
    begin
      UserSession.Socket := Socket;
      UserSession.ConnectTick := GetTickCount;
      UserSession.RecvText := '';
      UserSession.RecvTick := GetTickCount;
      UserSession.SendHeartbeatTick := GetTickCount;
      UserSession.IsPasswordOK := False;
      UserSession.DelayCloseTick := GetTickCount;
      UserSession.DelayClose := False;

      Socket.nIndex := I;
      MainOutMessage('远程控制开始连接: ' + Socket.RemoteAddress);
      Exit;
    end;
  end;

  MainOutMessage('远程控制数量已满: ' + Socket.RemoteAddress);
  Socket.Close;
end;

procedure TFrmMain.ServerSocketControlClientDisconnect(Sender: TObject; Socket: TCustomWinSocket);
var
  nSockIndex: Integer;
  UserSession: PControlSessionInfo;
begin
  nSockIndex := Socket.nIndex;
  if (nSockIndex >= 0) and (nSockIndex < Length(g_ControlSessionArray)) then
  begin
    UserSession := @g_ControlSessionArray[nSockIndex];
    UserSession.Socket := nil;
    UserSession.RecvText := '';
    UserSession.RecvTick := GetTickCount;
    UserSession.SendHeartbeatTick := GetTickCount;
    UserSession.IsPasswordOK := False;
    UserSession.DelayCloseTick := GetTickCount;
    UserSession.DelayClose := False;

    MainOutMessage('远程控制断开连接: ' + Socket.RemoteAddress);

    Socket.nIndex := -1;
  end;
end;

procedure TFrmMain.ServerSocketControlClientError(Sender: TObject; Socket: TCustomWinSocket; ErrorEvent: TErrorEvent; var ErrorCode: Integer);
begin
  ErrorCode := 0;
  Socket.Close;
end;

procedure TFrmMain.ServerSocketControlClientRead(Sender: TObject; Socket: TCustomWinSocket);
type
  PTAccountData = ^TAccountData;

  TAccountData = packed record
    UserEntry: TUserEntry;
    UserEntryAdd: TUserEntryAdd;
    CreateDate: TDateTime;
    UpdateDate: TDateTime;
  end;
var
  I, nCount, nSockIndex: Integer;
  UserSession: PControlSessionInfo;
  ControlMsgHeader: PControlMsgHeader;
  dwCmd: LongWord;
  sData: string;
  AccountList: TAccountList;
  AccountData: PTAccountData;
  AccountDatas: array[0..999] of TAccountData;
  IsChanged: Boolean;
  _AccountInfo: PTAccountInfo;
  AccountInfo: TAccountInfo;
begin
  nSockIndex := Socket.nIndex;
  if (nSockIndex >= 0) and (nSockIndex < Length(g_ControlSessionArray)) then
  begin
    UserSession := @g_ControlSessionArray[nSockIndex];
    UserSession.RecvText := UserSession.RecvText + Socket.ReceiveText;
    UserSession.RecvTick := GetTickCount;

    while Length(UserSession.RecvText) >= SizeOf(TControlMsgHeader) do
    begin
      ControlMsgHeader := @UserSession.RecvText[1];
      if ControlMsgHeader.dwCode <> ControlMsgHeaderIdent then
      begin
        UserSession.RecvText := '';
        Break;
      end;

      if Cardinal(Length(UserSession.RecvText)) < SizeOf(TControlMsgHeader) + ControlMsgHeader.nLength then begin
        Break;
      end;

      case ControlMsgHeader.dwCmd of
        CMSG_CHECK_PASSWORD:
          begin
            if ControlMsgHeader.nLength = 0 then
              sData := ''
            else
              sData := Copy(UserSession.RecvText, SizeOf(TControlMsgHeader) + 1, ControlMsgHeader.nLength);

            if SameText(sData, MD5Print(MD5String(g_Config.sControlPassword))) then
            begin
              dwCmd := SMSG_CHECK_PASSWORD_OK;
              UserSession.IsPasswordOK := True;
            end
            else
            begin
              dwCmd := SMSG_CHECK_PASSWORD_FAIL;
              UserSession.DelayCloseTick := GetTickCount + 1000;
              UserSession.DelayClose := True;
            end;

            SendControlMsg(Socket, dwCmd, nil, 0);
          end;
        CMSG_QUERY_ACCOUNT_LIST:
          begin
            if ControlMsgHeader.nLength > 0 then
            begin
              sData := Copy(UserSession.RecvText, SizeOf(TControlMsgHeader) + 1, ControlMsgHeader.nLength);

              nCount := 0;
              AccountList := TAccountList.Create;

              if g_AccountDB.FindAccount(sData, AccountList) > 0 then
              begin
                for I := 0 to AccountList.Count - 1 do
                begin
                  _AccountInfo := AccountList.Items[I];
                  if nCount >= Length(AccountDatas) - 1 then
                    Break;

                  AccountDatas[nCount].UserEntry.sAccount := _AccountInfo.AccountName;
                  AccountDatas[nCount].UserEntry.sPassword := _AccountInfo.Password;
                  AccountDatas[nCount].UserEntry.sUserName := _AccountInfo.UserName;
                  AccountDatas[nCount].UserEntry.sSSNo := _AccountInfo.IDCard;
                  AccountDatas[nCount].UserEntry.sPhone := _AccountInfo.Phone;
                  AccountDatas[nCount].UserEntry.sQuiz := _AccountInfo.Questions1;
                  AccountDatas[nCount].UserEntry.sAnswer := _AccountInfo.Answers1;
                  AccountDatas[nCount].UserEntry.sEMail := _AccountInfo.Mail;

                  AccountDatas[nCount].UserEntryAdd.sQuiz2 := _AccountInfo.Questions2;
                  AccountDatas[nCount].UserEntryAdd.sAnswer2 := _AccountInfo.Answers2;
                  AccountDatas[nCount].UserEntryAdd.sBirthDay := _AccountInfo.BirthDay;
                  AccountDatas[nCount].UserEntryAdd.sMobilePhone := _AccountInfo.MobilePhone;
                  AccountDatas[nCount].UserEntryAdd.sMemo := _AccountInfo.Memo;
                  AccountDatas[nCount].UserEntryAdd.sL2Password := _AccountInfo.L2Password;

                  AccountDatas[nCount].CreateDate := MyDate2Date(_AccountInfo.CreateDate);
                  AccountDatas[nCount].UpdateDate := MyDate2Date(_AccountInfo.LoginDate);
                  Inc(nCount);
                end;
              end;

              if nCount = 0 then
              begin
                SendControlMsg(Socket, SMSG_RESPONSE_ACCOUNT_LIST, nil, 0);
              end
              else
              begin
                SendControlMsg(Socket, SMSG_RESPONSE_ACCOUNT_LIST, PChar(@AccountDatas[0]), SizeOf(TAccountData) * nCount);
              end;

            end;
          end;
        CMSG_EDIT_ACCOUNT_INFO:
          begin
            if ControlMsgHeader.nLength = SizeOf(TAccountData) then
            begin
              AccountData := PTAccountData(Integer(PChar(UserSession.RecvText)) + SizeOf(TControlMsgHeader));

              if g_AccountDB.GetAccount(AccountData.UserEntry.sAccount, AccountInfo) then
              begin
                AccountInfo.Password := AccountData.UserEntry.sPassword;
                AccountInfo.UserName := AccountData.UserEntry.sUserName;
                AccountInfo.IDCard := AccountData.UserEntry.sSSNo;
                AccountInfo.BirthDay := AccountData.UserEntryAdd.sBirthDay;
                AccountInfo.Questions1 := AccountData.UserEntry.sQuiz;
                AccountInfo.Answers1 := AccountData.UserEntry.sAnswer;
                AccountInfo.Questions2 := AccountData.UserEntryAdd.sQuiz2;
                AccountInfo.Answers2 := AccountData.UserEntryAdd.sAnswer2;
                AccountInfo.Phone := AccountData.UserEntry.sPhone;
                AccountInfo.MobilePhone := AccountData.UserEntryAdd.sMobilePhone;
                AccountInfo.Mail := AccountData.UserEntry.sEMail;
                AccountInfo.L2Password := AccountData.UserEntryAdd.sL2Password;
                AccountInfo.Memo := AccountData.UserEntryAdd.sMemo;

                IsChanged := g_AccountDB.UpdateAccount(AccountInfo, ufAllField);
                if IsChanged then
                begin
                  WriteLogMsg('远程管理', AccountInfo);
                  SendControlMsg(Socket, SMSG_EDIT_ACCOUNT_INFO_OK, nil, 0);
                end
                else
                begin
                  SendControlMsg(Socket, SMSG_EDIT_ACCOUNT_INFO_FAIL, nil, 0);
                end;
              end
              else
              begin
                SendControlMsg(Socket, SMSG_EDIT_ACCOUNT_INFO_FAIL, nil, 0);
              end;
            end
            else
            begin
              SendControlMsg(Socket, SMSG_EDIT_ACCOUNT_INFO_FAIL, nil, 0);
            end;
          end;
        CMSG_HEARTBEAT:
          begin
            // 心跳包不处理
          end;
      end;

      UserSession.RecvText := Copy(UserSession.RecvText, SizeOf(TControlMsgHeader) + ControlMsgHeader.nLength + 1, MaxInt);
    end;
  end;
end;

procedure TFrmMain.SendControlMsg(Socket: TCustomWinSocket; dwCmd: LongWord; Buf: PChar; BufLen: Integer);
var
  Msg: TControlMsgHeader;
  S: string;
begin
  if (Buf = nil) or (BufLen = 0) then
  begin
    Msg.dwCode := ControlMsgHeaderIdent;
    Msg.dwCmd := dwCmd;
    Msg.nLength := 0;
    Socket.SendBuf(Msg, SizeOf(Msg));
  end
  else
  begin
    Msg.dwCode := ControlMsgHeaderIdent;
    Msg.dwCmd := dwCmd;
    Msg.nLength := BufLen;

    SetLength(S, SizeOf(Msg) + BufLen);
    Move(Msg, S[1], SizeOf(Msg));
    Move(Buf^, S[1 + SizeOf(Msg)], BufLen);
    Socket.SendText(S);
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

procedure TFrmMain.N1Click(Sender: TObject);
begin
  ShowFrmBatchEditAccountInfo;
end;

//这是一段测试代码
procedure TFrmMain.test1Click(Sender: TObject);
//var
  //UserInfo: pTUserInfo;
begin
  QuickLogin('1', '@171@91@163@82@111@88@155@153@146@157@101@154@105@99@109@148@86@96@87@172@159@149@90@114@83@105@108@103@107@100@104@106@101@114@100@99@104@104@107@110@97@112@110@113@87@95@86@167@157@161@154@89@112@102@113@108@93@87@153@87@112@85@112@112@105@114@82@173'
  , '4625113594332480998', '9999', '', 0);
//  HttpPost(2, '@171@91@163@82@111@88@107@154@101@110@105@110@150@103@101@103@86@96@87@172@159@149@90@114@83@105@108@103@107@100@104@106@101@114@100@99@104@104@107@110@97@112@110@113@87@95@86@167@157@161@154@89@112@100@110@106@93@87@153@87@112@85@112@112@105@114@82@173'
//  , '4625113594332480998', '', '', QuickLogin, Integer(UserInfo));
end;

end.

