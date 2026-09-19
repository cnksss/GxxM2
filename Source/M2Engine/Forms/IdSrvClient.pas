unit IdSrvClient;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms, Dialogs, ExtCtrls, IniFiles, JSocket, WinSock, SDK,
  Grobal2, Common, M2Share,DateUtils,
{$IFDEF CPUX64}
  // VMProtectSDK,
{$ENDIF}
  M2Definition, System.Win.ScktComp;

type
  TFrmIDSoc = class(TForm)
    IDSocket: TClientSocket;
    Timer1: TTimer;
    procedure Timer1Timer(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure IDSocketError(Sender: TObject; Socket: TCustomWinSocket; ErrorEvent: TErrorEvent; var ErrorCode: Integer);
    procedure IDSocketRead(Sender: TObject; Socket: TCustomWinSocket);
    procedure IDSocketConnect(Sender: TObject; Socket: TCustomWinSocket);
    procedure IDSocketDisconnect(Sender: TObject; Socket: TCustomWinSocket);
  private
    TList_2DC: TList;
    IDSrvAddr: string; // 0x2E0
    IDSrvPort: Integer; // 0x2E4
    // sIDSckStr :String; //0x2E8
    // boConnected:Boolean;
    procedure GetPasswdSuccess(sData: string);
    procedure GetCancelAdmission(sData: string);
    procedure GetCancelAdmissionA(sData: string);
    procedure SetTotalHumanCount(sData: string);
    procedure GetServerLoad(sData: string);
    procedure SetPlayerAccountInfo(sData: string);
    procedure SetPlayerChangeAccountInfoRet(sData: string);
    procedure DelSession(nSessionID: Integer);
    procedure NewSession(sAccount, sIPaddr: string; nSessionID, nPayMent, nPayMode: Integer);
    procedure ClearSession();
    // procedure ClearEmptySession();
    procedure SendSocket(sSENDMSG: AnsiString);
    { Private declarations }
  public
    m_SessionList: TGList; // 0x2D8
    procedure Initialize();
    procedure Run();
    procedure SendOnlineHumCountMsg(nCount: Integer);
    procedure SendHumanLogOutMsg(sUserID: string; nID: Integer);
    function GetAdmission(sAccount, sIPaddr: string; nSessionID: Integer; var nPayMode: Integer; var nPayMent: Integer):
      pTSessInfo;
    function GetAdmissionEx(sAccount, sIPaddr: string; var nSessionID: Integer; var nPayMode: Integer; var nPayMent: Integer):
      pTSessInfo;
    function GetSessionCount(): Integer;
    procedure GetSessionList(List: TList);
    procedure SendLogonCostMsg(sAccount: string; nTime: Integer);
    procedure SendLogon(sAccount: string);
    procedure SendGetAccountInfo(sAccount: string; sUserName: string);
    procedure SendChangeAccountInfo(AccountInfo: pTAccountInfo2);
    procedure Close();
  end;

procedure IDSocketThread(ThreadInfo: pTThreadInfo); stdcall;

var
  FrmIDSoc: TFrmIDSoc;

implementation

uses
  HUtil32, ObjPlayer, EDcode, ObjNpc;
{$R *.dfm}
{ TFrmIDSoc }
{ .$DEFINE LOG_SESSION }
{$IFDEF LOG_SESSION}

type
  TSessionOperate = (soAdd, soModify, soDel, soClose, soClear, soNoFound);

var
  g_LockLogSession: TRTLCriticalSection;

procedure LogSession(Operate: TSessionOperate; sSessionID, sAccount, sIPaddr: string; boWriteDate: Boolean = False);
var
  Msg, sFilePath: string;
  flname: string;
  fhandle: TextFile;
  Year, Month, Day: Word;
begin
  EnterCriticalSection(g_LockLogSession);
  try
    if boWriteDate then
      Msg := Msg + FormatDateTime('yyyy/mm/dd hh:mm:ss', Now) + ' ' + Msg;
    case Operate of
      soAdd:
        Msg := Msg + '添加 ';
      soModify:
        Msg := Msg + '修改 ';
      soDel:
        Msg := Msg + '删除 ';
      soClose:
        Msg := Msg + '关闭 ';
      soClear:
        Msg := Msg + '清空 ';
      soNoFound:
        Msg := Msg + '认证失败 ';
    end;
    Msg := Msg + sSessionID + ' ' + sAccount + ' ' + sIPaddr;
    DecodeDate(Now, Year, Month, Day);
    sFilePath := g_sSelfFilePath + 'session_log\' + IntToStr(Year) + '-' + IntToStr2(Month) + '\';
    // showmessage(sFilePath);
    if not DirectoryExists(sFilePath) then
    begin
      ForceDirectories(sFilePath);
    end;
    { if GameCanvas <> nil then begin
      GameCanvas.HGE.System_Log(Msg);
      end else begin }
    flname := sFilePath + IntToStr2(Day) + '.txt';
    // showmessage(flname);
    if FileExists(flname) then
    begin
      AssignFile(fhandle, flname);
      Append(fhandle);
    end
    else
    begin
      AssignFile(fhandle, flname);
      Rewrite(fhandle);
    end;
    // Writeln(fhandle, TimeToStr(Time) + ' ' + Msg);
    Writeln(fhandle, Msg);
    CloseFile(fhandle);
    // end;
  finally
    LeaveCriticalSection(g_LockLogSession);
  end;
end;
{$ENDIF}

procedure TFrmIDSoc.FormCreate(Sender: TObject);
var
  Conf: TIniFile;
begin
  IDSocket.Host := '';
  if FileExists(g_sSelfFilePath + sConfigFileName) then
  begin
    Conf := TIniFile.Create(g_sSelfFilePath + sConfigFileName);
    if Conf <> nil then
    begin
      IDSrvAddr := Conf.ReadString('Server', 'IDSAddr', '127.0.0.1');
      IDSrvPort := Conf.ReadInteger('Server', 'IDSPort', 5600);
      Conf.Free;
    end;
  end; // else
  // ShowMessage('配置文件' + sConfigFileName + '未找到！');
  m_SessionList := TGList.Create;
  TList_2DC := TList.Create;
  g_Config.boIDSocketConnected := False;
  // sub_48D290();
end;

procedure TFrmIDSoc.FormDestroy(Sender: TObject);
begin
  ClearSession();
  m_SessionList.Free;
  TList_2DC.Free;
end;

procedure TFrmIDSoc.Timer1Timer(Sender: TObject);
begin
  if not IDSocket.Active then
  begin
    IDSocket.Active := True;
  end;
end;

procedure TFrmIDSoc.IDSocketError(Sender: TObject; Socket: TCustomWinSocket; ErrorEvent: TErrorEvent; var ErrorCode: Integer);
begin
  ErrorCode := 0;
  Socket.Close;
end;

procedure TFrmIDSoc.IDSocketRead(Sender: TObject; Socket: TCustomWinSocket);
begin
  EnterCriticalSection(g_Config.UserIDSection);
  try
    g_Config.sIDSocketRecvText := g_Config.sIDSocketRecvText + Socket.ReceiveText;
  finally
    LeaveCriticalSection(g_Config.UserIDSection);
  end;
end;

procedure TFrmIDSoc.Initialize; // 0048D3F8
begin
  IDSocket.Active := False;
  IDSocket.Address := IDSrvAddr;
  IDSocket.Port := IDSrvPort;
  IDSocket.Active := True;
  Timer1.Enabled := True;
  MainOutMessage('开始连接登录服务器(' + IDSrvAddr + ':' + IntToStr(IDSrvPort) + ')...');
end;
{$IF IDSOCKETMODE = TIMERENGINE}

procedure TFrmIDSoc.SendSocket(sSENDMSG: AnsiString);
begin
  if IDSocket.Socket.Connected then
  begin
    IDSocket.Socket.SendText(sSENDMSG);
  end;
end;
{$ELSE}

procedure TFrmIDSoc.SendSocket(sSENDMSG: AnsiString);
var
  boSendData: Boolean;
  Config: pTConfig;
  ThreadInfo: pTThreadInfo;
  timeout: TTimeVal;
  writefds: TFDSet;
  nRet: Integer;
  s: TSocket;
begin
  Config := @g_Config;
  ThreadInfo := @g_Config.DBSOcketThread;
  s := Config.IDSocket;
  boSendData := False;
  while True do
  begin
    if not boSendData then
      Sleep(1)
    else
      Sleep(0);
    boSendData := False;
    ThreadInfo.dwRunTick := MyGetTickCount();
    ThreadInfo.boActived := True;
    ThreadInfo.nRunFlag := 128;
    ThreadInfo.nRunFlag := 129;
    timeout.tv_sec := 0;
    timeout.tv_usec := 20;
    writefds.fd_count := 1;
    writefds.fd_array[0] := s;
    nRet := select(0, nil, @writefds, nil, @timeout);
    if nRet = SOCKET_ERROR then
    begin
      nRet := WSAGetLastError();
      Config.nIDSocketWSAErrCode := nRet - WSABASEERR;
      Inc(Config.nIDSocketErrorCount);
      if nRet = WSAEWOULDBLOCK then
      begin
        Continue;
      end;
      if Config.IDSocket = INVALID_SOCKET then
        Break;
      Config.IDSocket := INVALID_SOCKET;
      Sleep(100);
      Config.boIDSocketConnected := False;
      Break;
    end;
    if nRet <= 0 then
    begin
      Continue;
    end;
    boSendData := True;
    nRet := Send(s, sSENDMSG[1], Length(sSENDMSG), 0);
    if nRet = SOCKET_ERROR then
    begin
      Inc(Config.nIDSocketErrorCount);
      Config.nIDSocketWSAErrCode := WSAGetLastError - WSABASEERR;
      Continue;
    end;
    Inc(Config.nDBSocketSendLen, nRet);
    Break;
  end;
end;
{$IFEND}

procedure TFrmIDSoc.SendHumanLogOutMsg(sUserID: string; nID: Integer); // 0048D448
var
  I: Integer;
  SessInfo: pTSessInfo;
resourcestring
  sFormatMsg = '(%d/%s/%d)';
begin
  m_SessionList.Lock;
  try
    for I := 0 to m_SessionList.Count - 1 do
    begin
      SessInfo := m_SessionList.Items[I];
      if (SessInfo.nSessionID = nID) and (SessInfo.sAccount = sUserID) then
      begin
        SessInfo.dwCloseTick := MyGetTickCount();
        SessInfo.boClose := True;
{$IFDEF LOG_SESSION}
        LogSession(soClose, IntToStr(SessInfo.nSessionID), SessInfo.sAccount, SessInfo.sIPaddr);
{$ENDIF}
        Break;
      end;
    end;
  finally
    m_SessionList.UnLock;
  end;
  SendSocket(Format(sFormatMsg, [SS_SOFTOUTSESSION, sUserID, nID]));
end;

procedure TFrmIDSoc.SendLogon(sAccount: string);
const
  sFormatMsg = '(%d/%s)';
begin
  SendSocket(Format(sFormatMsg, [SS_PASSWORDSUCCESS, sAccount]));
end;

procedure TFrmIDSoc.SendGetAccountInfo(sAccount: string; sUserName: string);
const
  sFormatMsg = '(%d/%s/%s)';
begin
  // M2获取用户注册信息 chongchong 2015-11-13
  SendSocket(Format(sFormatMsg, [SS_GETACCOUNTINFO, sAccount, sUserName]));
end;

procedure TFrmIDSoc.SendChangeAccountInfo(AccountInfo: pTAccountInfo2);
resourcestring
  sFormatMsg = '(%d/%s)';
var
  sSend: string;
begin
  sSend := Format(sFormatMsg, [SS_ChangeAccountInfo, EncodeBuffer(PAnsiChar(AccountInfo), SizeOf(TAccountInfo2))]);
  SendSocket(sSend);
end;

procedure TFrmIDSoc.SendLogonCostMsg(sAccount: string; nTime: Integer); // 0048D53C
resourcestring
  sFormatMsg = '(%d/%s/%d)';
begin
  SendSocket(Format(sFormatMsg, [SS_LOGINCOST, sAccount, nTime]));
end;

procedure TFrmIDSoc.SendOnlineHumCountMsg(nCount: Integer);
resourcestring
  sFormatMsg = '(%d/%s/%d/%d)';
begin
  SendSocket(Format(sFormatMsg, [SS_SERVERINFO, g_Config.sServerName, nServerIndex, nCount]));
end;

procedure TFrmIDSoc.Run; // 0048D724
var
  sSocketText: string;
  sData: string;
  sBody: string;
  sCode: string;
  Config: pTConfig;
resourcestring
  sExceptionMsg = '[Exception] TFrmIdSoc.DecodeSocStr';
begin
  Config := @g_Config;
  EnterCriticalSection(Config.UserIDSection);
  if Config.sIDSocketRecvText <> '' then
  begin
    try
      if Pos(')', Config.sIDSocketRecvText) <= 0 then
        Exit;
      sSocketText := Config.sIDSocketRecvText;
      Config.sIDSocketRecvText := '';
    finally
      LeaveCriticalSection(Config.UserIDSection);
    end;
  end;
  try
    while (True) do
    begin
      sSocketText := ArrestStringEx(sSocketText, '(', ')', sData);
      if sData = '' then
        Break;
      sBody := GetValidStr3_Ex(sData, sCode, '/');
      case StrToIntDef(sCode, 0) of
        SS_OPENSESSION { 100 } :
          GetPasswdSuccess(sBody);
        SS_CLOSESESSION { 101 } :
          GetCancelAdmission(sBody);
        SS_KEEPALIVE { 104 } :
          SetTotalHumanCount(sBody);
        UNKNOWMSG:
          ;
        SS_KICKUSER { 111 } :
          GetCancelAdmissionA(sBody);
        SS_SERVERLOAD { 113 } :
          GetServerLoad(sBody);
        SS_GetAccountInfoRet:
          SetPlayerAccountInfo(sBody);
        SS_ChangeAccountInfoRet:
          SetPlayerChangeAccountInfoRet(sBody);
      end;
      if Pos(')', sSocketText) <= 0 then
        Break;
    end;
    EnterCriticalSection(Config.UserIDSection);
    try
      Config.sIDSocketRecvText := sSocketText + Config.sIDSocketRecvText;
    finally
      LeaveCriticalSection(Config.UserIDSection);
    end;
  except
    MainOutMessage(sExceptionMsg);
  end;
{$IF NEED_KEY = 1}   // 不限制人数了  By 一支笔 at:2021-12-24 13:29:57
  if SecondsBetween(Now, g_StartRunTime) >= g_M2Lime_RunTime div 1000 + 287 then
  begin
    g_ErrorRun := False;
    MemCpy(@nServerIndex, @g_sCanNotRun, 100 + Random(400)); // 搞异常 2020-11-09 11:18:07
  end;
{$IFEND}
end;

procedure TFrmIDSoc.GetPasswdSuccess(sData: string); // 0048D9B4
var
  sAccount: string;
  sSessionID: string;
  sPayCost: string;
  sIPaddr: string;
  sPayMode: string;
resourcestring
  sExceptionMsg = '[Exception] TFrmIdSoc.GetPasswdSuccess';
begin
  try
    sData := GetValidStr3_Ex(sData, sAccount, '/');
    sData := GetValidStr3_Ex(sData, sSessionID, '/');
    sData := GetValidStr3_Ex(sData, sPayCost, '/'); // boPayCost
    sData := GetValidStr3_Ex(sData, sPayMode, '/'); // nPayMode
    sData := GetValidStr3_Ex(sData, sIPaddr, '/'); // sIPaddr
    NewSession(sAccount, sIPaddr, StrToIntDef(sSessionID, 0), StrToIntDef(sPayCost, 0), StrToIntDef(sPayMode, 0));
{$IFDEF LOG_SESSION}
    LogSession(soAdd, sSessionID, sAccount, sIPaddr);
{$ENDIF}
  except
    MainOutMessage(sExceptionMsg);
  end;
end;

procedure TFrmIDSoc.GetCancelAdmission(sData: string); // 0048DB60
var
  SC, sSessionID: string;
resourcestring
  sExceptionMsg = '[Exception] TFrmIdSoc.GetCancelAdmission';
begin
  try
    sSessionID := GetValidStr3_Ex(sData, SC, '/');
    DelSession(StrToIntDef(sSessionID, 0));
  except
    on E: Exception do
    begin
      MainOutMessage(sExceptionMsg);
      MainOutMessage(E.Message);
    end;
  end;
end;

procedure TFrmIDSoc.NewSession(sAccount, sIPaddr: string; nSessionID, nPayMent, nPayMode: Integer); // 0048DC44
var
  I: Integer;
  SessInfo: pTSessInfo;
  boFind: Boolean;
begin
  boFind := False;
  m_SessionList.Lock;
  try
    for I := 0 to m_SessionList.Count - 1 do
    begin
      SessInfo := m_SessionList.Items[I];
      if (SessInfo.sAccount = sAccount) then
      begin
        SessInfo.sIPaddr := sIPaddr;
        SessInfo.nSessionID := nSessionID;
        SessInfo.nPayMent := nPayMent;
        SessInfo.nPayMode := nPayMode;
        SessInfo.nSessionStatus := 0;
        SessInfo.dwStartTick := MyGetTickCount();
        SessInfo.dwActiveTick := MyGetTickCount();
        SessInfo.nRefCount := 1;
        SessInfo.boClose := False;
        SessInfo.dwCloseTick := MyGetTickCount();
        boFind := True;
        Break;
      end;
    end;
  finally
    m_SessionList.UnLock;
  end;
  if not boFind then
  begin
    New(SessInfo);
    SessInfo.sAccount := sAccount;
    SessInfo.sIPaddr := sIPaddr;
    SessInfo.nSessionID := nSessionID;
    SessInfo.nPayMent := nPayMent;
    SessInfo.nPayMode := nPayMode;
    SessInfo.nSessionStatus := 0;
    SessInfo.dwStartTick := MyGetTickCount();
    SessInfo.dwActiveTick := MyGetTickCount();
    SessInfo.nRefCount := 1;
    SessInfo.boClose := False;
    SessInfo.dwCloseTick := MyGetTickCount();
    m_SessionList.Lock;
    try
      m_SessionList.Add(SessInfo);
    finally
      m_SessionList.UnLock;
    end;
  end;
end;

procedure TFrmIDSoc.DelSession(nSessionID: Integer); // 0048DD5C
var
  I: Integer;
  sAccount: string;
  SessInfo: pTSessInfo;
resourcestring
  sExceptionMsg = '[Exception] FrmIdSoc.DelSession %d';
begin
  try
    sAccount := '';
    m_SessionList.Lock;
    try
      for I := m_SessionList.Count - 1 downto 0 do
      begin
        // for i := 0 to m_SessionList.Count - 1 do begin
        SessInfo := m_SessionList.Items[I];
        if SessInfo.nSessionID = nSessionID then
        begin
{$IFDEF LOG_SESSION}
          LogSession(soDel, IntToStr(nSessionID), SessInfo.sAccount, SessInfo.sIPaddr);
{$ENDIF}
          sAccount := SessInfo.sAccount;
          m_SessionList.Delete(I);
          Dispose(SessInfo);
          Break;
        end;
      end;
    finally
      m_SessionList.UnLock;
    end;
    if sAccount <> '' then
    begin
      RunSocket.KickUser(sAccount, nSessionID);
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(Format(sExceptionMsg, [0]));
      MainOutMessage(E.Message);
    end;
  end;
end;

(*
  procedure TFrmIDSoc.ClearEmptySession;
  var
  I: Integer;
  SessInfo: pTSessInfo;
  begin
  m_SessionList.Lock;
  try
  for I := m_SessionList.Count - 1 downto 0 do
  begin
  SessInfo := m_SessionList.Items[I];
  if SessInfo.nRefCount <= 0 then
  begin
  m_SessionList.Delete(I);
  Dispose(SessInfo);
  Continue;
  end;
  end;
  finally
  m_SessionList.UnLock;
  end;
  end;
*)
procedure TFrmIDSoc.ClearSession;
var
  I: Integer;
begin
  m_SessionList.Lock;
  try
    for I := 0 to m_SessionList.Count - 1 do
    begin
      Dispose(pTSessInfo(m_SessionList.Items[I]));
    end;
    m_SessionList.Clear;
{$IFDEF LOG_SESSION}
    LogSession(soClear, '*', '*', '*');
{$ENDIF}
  finally
    m_SessionList.UnLock;
  end;
end;

function TFrmIDSoc.GetAdmission(sAccount, sIPaddr: string; nSessionID: Integer; var nPayMode: Integer; var nPayMent: Integer):
  pTSessInfo; // 0048DE80
var
  I: Integer;
  SessInfo: pTSessInfo;
  boFound: Boolean;
  boClose: Boolean;
resourcestring
  sGetFailMsg = '[非法登录] 全局会话验证失败1(%s/%s/%d)';
  sGetFailMsg2 = '[非法登录] 全局会话验证失败--关闭(%s/%s/%d)';
begin
  // Result:=3;
  // exit;
  boFound := False;
  boClose := False;
  Result := nil;
  nPayMent := 0;
  nPayMode := 0;
  m_SessionList.Lock;
  try
    for I := 0 to m_SessionList.Count - 1 do
    begin
      SessInfo := m_SessionList.Items[I];
      if (SessInfo.nSessionID = nSessionID) and (SessInfo.sAccount = sAccount) { and (SessInfo.sIPaddr = sIPaddr) } then
      begin
        boClose := True;
        if (not SessInfo.boClose) then
        begin
          case SessInfo.nPayMent of
            2:
              nPayMent := 3;
            1:
              nPayMent := 2;
            0:
              nPayMent := 1;
          end;
          Result := SessInfo;
          nPayMode := SessInfo.nPayMode;
          boFound := True;
          Break;
        end;
      end;
    end;
  finally
    m_SessionList.UnLock;
  end;
{$IFDEF LOG_SESSION}
  if not boFound then
  begin
    LogSession(soNoFound, IntToStr(nSessionID), sAccount, sIPaddr);
  end;
{$ENDIF}
  if g_Config.boViewAdmissionFailure and not boFound then
  begin
    if not boClose then
      MainOutMessage(Format(sGetFailMsg, [sAccount, sIPaddr, nSessionID]))
    else
      MainOutMessage(Format(sGetFailMsg2, [sAccount, sIPaddr, nSessionID]))
  end;
end;

function TFrmIDSoc.GetAdmissionEx(sAccount, sIPaddr: string; var nSessionID: Integer; var nPayMode: Integer; var nPayMent: Integer):
  pTSessInfo;
var
  I: Integer;
  SessInfo: pTSessInfo;
  boFound: Boolean;
resourcestring
  sGetFailMsg = '[非法登录] 全局会话验证失败2(%s/%s/%d)';
begin
  // Result:=3;
  // exit;
  boFound := False;
  Result := nil;
  nPayMent := 0;
  nPayMode := 0;
  nSessionID := 0;
  m_SessionList.Lock;
  try
    for I := 0 to m_SessionList.Count - 1 do
    begin
      SessInfo := m_SessionList.Items[I];
      if (not SessInfo.boClose) and (SessInfo.sAccount = sAccount) then
      begin
        case SessInfo.nPayMent of
          2:
            nPayMent := 3;
          1:
            nPayMent := 2;
          0:
            nPayMent := 1;
        end;
        Result := SessInfo;
        nSessionID := SessInfo.nSessionID;
        nPayMode := SessInfo.nPayMode;
        boFound := True;
        Break;
      end;
    end;
  finally
    m_SessionList.UnLock;
  end;
{$IFDEF LOG_SESSION}
  if not boFound then
  begin
    LogSession(soNoFound, '?', sAccount, sIPaddr);
  end;
{$ENDIF}
  if g_Config.boViewAdmissionFailure and not boFound then
  begin
    MainOutMessage(Format(sGetFailMsg, [sAccount, sIPaddr, nSessionID]));
  end;
end;

procedure TFrmIDSoc.SetTotalHumanCount(sData: string); // 0048E014
begin
  g_nTotalHumCount := StrToIntDef(sData, 0)
end;

procedure TFrmIDSoc.GetCancelAdmissionA(sData: string); // 0048E06C
var
  nSessionID: Integer;
  sSessionID: string;
  sAccount: string;
resourcestring
  sExceptionMsg = '[Exception] FrmIdSoc.GetCancelAdmissionA';
begin
  try
    sSessionID := GetValidStr3_Ex(sData, sAccount, '/');
    nSessionID := StrToIntDef(sSessionID, 0);
    if not g_Config.boTestServer then
    begin
      UserEngine.HumanExpire(sAccount);
      DelSession(nSessionID);
    end;
  except
    MainOutMessage(sExceptionMsg);
  end;
end;

procedure TFrmIDSoc.GetServerLoad(sData: string); // 0048E174
var
  SC, s10, s14, s18, s1C: string;
begin
  sData := GetValidStr3_Ex(sData, SC, '/');
  sData := GetValidStr3_Ex(sData, s10, '/');
  sData := GetValidStr3_Ex(sData, s14, '/');
  sData := GetValidStr3_Ex(sData, s18, '/');
  sData := GetValidStr3_Ex(sData, s1C, '/');
  nCurrentMonthly := StrToIntDef(SC, 0);
  nLastMonthlyTotalUsage := StrToIntDef(s10, 0);
  nTotalTimeUsage := StrToIntDef(s14, 0);
  nGrossTotalCnt := StrToIntDef(s18, 0);
  nGrossResetCnt := StrToIntDef(s1C, 0);
end;

procedure TFrmIDSoc.SetPlayerAccountInfo(sData: string); // 0048E174
var
  sUserName, sRegUserName, sPassword, sQuiz, sAnswer, sQuiz2, sAnswer2, sBirthDay, sMobilePhone, sPhone, sEMail: string;
  Player: TPlayObject;
begin
  sData := GetValidStr3_Ex(sData, sUserName, '/');
  sUserName := Copy(sUserName, 1, Length(sUserName) - 1);
  Player := UserEngine.GetPlayObject(sUserName);
  if Player <> nil then
  begin
    sData := GetValidStr3_Ex(sData, sRegUserName, #9);
    sData := GetValidStr3_Ex(sData, sPassword, #9);
    sData := GetValidStr3_Ex(sData, sQuiz, #9);
    sData := GetValidStr3_Ex(sData, sAnswer, #9);
    sData := GetValidStr3_Ex(sData, sQuiz2, #9);
    sData := GetValidStr3_Ex(sData, sAnswer2, #9);
    sData := GetValidStr3_Ex(sData, sBirthDay, #9);
    sData := GetValidStr3_Ex(sData, sMobilePhone, #9);
    sData := GetValidStr3_Ex(sData, sPhone, #9);
    sData := GetValidStr3_Ex(sData, sEMail, #9);
    Player.m_sUserEntryUserName := Copy(sRegUserName, 1, Length(sRegUserName) - 1);
    Player.m_sUserEntryPassword := Copy(sPassword, 1, Length(sPassword) - 1);
    Player.m_sUserEntryQuiz := Copy(sQuiz, 1, Length(sQuiz) - 1);
    Player.m_sUserEntryAnswer := Copy(sAnswer, 1, Length(sAnswer) - 1);
    Player.m_sUserEntryQuiz2 := Copy(sQuiz2, 1, Length(sQuiz2) - 1);
    Player.m_sUserEntryAnswer2 := Copy(sAnswer2, 1, Length(sAnswer2) - 1);
    Player.m_sUserEntryBirthDay := Copy(sBirthDay, 1, Length(sBirthDay) - 1);
    Player.m_sUserEntryMobilePhone := Copy(sMobilePhone, 1, Length(sMobilePhone) - 1);
    Player.m_sUserEntryPhone := Copy(sPhone, 1, Length(sPhone) - 1);
    Player.m_sUserEntryEMail := Copy(sEMail, 1, Length(sEMail) - 1);
  end;
end;

procedure TFrmIDSoc.SetPlayerChangeAccountInfoRet(sData: string);
var
  sCode, sLabel: string;
  nCode: Integer;
  AccountInfo: TAccountInfo2;
  Player: TPlayObject;
  Npc: TNormNpc;
begin
  sData := GetValidStr3_Ex(sData, sCode, '/');
  nCode := StrToIntDef(sCode, -1);
  if Length(sData) = GetEncodeSize(SizeOf(TAccountInfo2)) then
  begin
    DecodeString(sData, @AccountInfo, SizeOf(TAccountInfo2));
    Player := UserEngine.GetPlayObject(TObject(AccountInfo.PlayObject));
    if Player = nil then
      Exit;
    Npc := UserEngine.FindMerchant(TObject(AccountInfo.Npc));
    if Npc = nil then
      Npc := UserEngine.FindNPC(TObject(AccountInfo.Npc));
    if Npc = nil then
      Exit;
    case nCode of
      0:
        sLabel := '@ChangeAccountInfoOK';
      -1:
        sLabel := '@ChangeAccountInfo_Error_NotExists';
      -2, -7:
        sLabel := '@ChangeAccountInfo_Error_Password';
      -3:
        sLabel := '@ChangeAccountInfo_Error_PasswordSame';
      -4, -5, -6:
        sLabel := '@ChangeAccountInfo_Error_PasswordSimple';
      -8:
        sLabel := '@ChangeAccountInfo_Error_UserName';
      -9:
        sLabel := '@ChangeAccountInfo_Error_Quiz1';
      -10:
        sLabel := '@ChangeAccountInfo_Error_Answer1';
      -11:
        sLabel := '@ChangeAccountInfo_Error_Quiz2';
      -12:
        sLabel := '@ChangeAccountInfo_Error_Answer2';
      -13:
        sLabel := '@ChangeAccountInfo_Error_MobilePhone';
      -14:
        sLabel := '@ChangeAccountInfo_Error_Mail';
      -15:
        sLabel := '@ChangeAccountInfo_Error_Quiz1Answer1Same';
      -16:
        sLabel := '@ChangeAccountInfo_Error_Quiz2Answer2Same';
      -17:
        sLabel := '@ChangeAccountInfo_Error_L2Password';
      -18:
        sLabel := '@ChangeAccountInfo_Error_L2PasswordSameAccount';
      -19:
        sLabel := '@ChangeAccountInfo_Error_L2PasswordSamePassword';
      -20:
        sLabel := '@ChangeAccountInfoError';
    else
      sLabel := '';
    end;
    if (sLabel <> '') then
    begin
      Player.m_nScriptGotoCount := 0;
      Npc.GotoLable(Player, sLabel, False);
    end;
  end;
end;

procedure TFrmIDSoc.IDSocketConnect(Sender: TObject; Socket: TCustomWinSocket);
begin
  g_Config.boIDSocketConnected := True;
  MainOutMessage('登录服务器(' + Socket.RemoteAddress + ':' + IntToStr(Socket.RemotePort) + ')连接成功.');
end;

procedure TFrmIDSoc.IDSocketDisconnect(Sender: TObject; Socket: TCustomWinSocket);
begin
  if g_Config.boIDSocketConnected then
  begin
    ClearSession();
    g_Config.boIDSocketConnected := False;
    MainOutMessage('登录服务器(' + Socket.RemoteAddress + ':' + IntToStr(Socket.RemotePort) + ')断开连接.');
  end;
end;
{$IF IDSOCKETMODE = TIMERENGINE}

procedure TFrmIDSoc.Close;
begin
  Timer1.Enabled := False;
  IDSocket.Active := False;
end;
{$ELSE}

procedure TFrmIDSoc.Close;
var
  ThreadInfo: pTThreadInfo;
begin
  ThreadInfo := @g_Config.IDSocketThread;
  ThreadInfo.boTerminaled := True;
  if WaitForSingleObject(ThreadInfo.hThreadHandle, 1000) <> 0 then
  begin
    SuspendThread(ThreadInfo.hThreadHandle);
  end;
end;
{$IFEND}

function TFrmIDSoc.GetSessionCount: Integer;
begin
  m_SessionList.Lock;
  try
    Result := m_SessionList.Count;
  finally
    m_SessionList.UnLock;
  end;
end;

procedure TFrmIDSoc.GetSessionList(List: TList);
var
  I: Integer;
begin
  m_SessionList.Lock;
  try
    for I := 0 to m_SessionList.Count - 1 do
    begin
      List.Add(m_SessionList.Items[I]);
    end;
  finally
    m_SessionList.UnLock;
  end;
end;

procedure IDSocketRead(Config: pTConfig);
var
  dwReceiveTimeTick: LongWord;
  nReceiveTime: Integer;
  sRecvText: string;
  nRecvLen: Integer;
  nRet: Integer;
begin
  if Config.DBSocket = INVALID_SOCKET then
    Exit;
  dwReceiveTimeTick := MyGetTickCount();
  nRet := ioctlsocket(Config.DBSocket, FIONREAD, nRecvLen);
  if (nRet = SOCKET_ERROR) or (nRecvLen = 0) then
  begin
    Config.DBSocket := INVALID_SOCKET;
    Sleep(100);
    Config.boIDSocketConnected := False;
    Exit;
  end;
  SetLength(sRecvText, nRecvLen);
  nRecvLen := recv(Config.DBSocket, Pointer(sRecvText)^, nRecvLen, 0);
  SetLength(sRecvText, nRecvLen);
  Inc(Config.nIDSocketRecvIncLen, nRecvLen);
  if (nRecvLen <> SOCKET_ERROR) and (nRecvLen > 0) then
  begin
    if nRecvLen > Config.nIDSocketRecvMaxLen then
      Config.nIDSocketRecvMaxLen := nRecvLen;
    EnterCriticalSection(Config.UserIDSection);
    try
      Config.sIDSocketRecvText := Config.sIDSocketRecvText + sRecvText;
    finally
      LeaveCriticalSection(Config.UserIDSection);
    end;
    FrmIDSoc.Run;
  end;
  Inc(Config.nIDSocketRecvCount);
  nReceiveTime := MyGetTickCount - dwReceiveTimeTick;
  if Config.nIDReceiveMaxTime < nReceiveTime then
    Config.nIDReceiveMaxTime := nReceiveTime;
end;

procedure IDSocketProcess(Config: pTConfig; ThreadInfo: pTThreadInfo);
var
  s: TSocket;
  Name: sockaddr_in;
  HostEnt: PHostEnt;
  argp: LongInt;
  readfds: TFDSet;
  timeout: TTimeVal;
  nRet: Integer;
  boRecvData: BOOL;
  nRunTime: Integer;
  dwRunTick: LongWord;
resourcestring
  sIDServerConnected = '登录服务器(%s:%d)连接成功.';
begin
  s := INVALID_SOCKET;
  if Config.DBSocket <> INVALID_SOCKET then
    s := Config.DBSocket;
  dwRunTick := MyGetTickCount();
  ThreadInfo.dwRunTick := dwRunTick;
  boRecvData := False;
  while True do
  begin
    if ThreadInfo.boTerminaled then
      Break;
    if not boRecvData then
      Sleep(1)
    else
      Sleep(0);
    boRecvData := False;
    nRunTime := MyGetTickCount - ThreadInfo.dwRunTick;
    if ThreadInfo.nRunTime < nRunTime then
      ThreadInfo.nRunTime := nRunTime;
    if ThreadInfo.nMaxRunTime < nRunTime then
      ThreadInfo.nMaxRunTime := nRunTime;
    if MyGetTickCount - dwRunTick >= 1000 then
    begin
      dwRunTick := MyGetTickCount();
      if ThreadInfo.nRunTime > 0 then
        Dec(ThreadInfo.nRunTime);
    end;
    ThreadInfo.dwRunTick := MyGetTickCount();
    ThreadInfo.boActived := True;
    ThreadInfo.nRunFlag := 125;
    if (Config.DBSocket = INVALID_SOCKET) or (s = INVALID_SOCKET) then
    begin
      if Config.DBSocket <> INVALID_SOCKET then
      begin
        Config.DBSocket := INVALID_SOCKET;
        Sleep(100);
        ThreadInfo.nRunFlag := 126;
        Config.boIDSocketConnected := False;
      end;
      if s <> INVALID_SOCKET then
      begin
        closesocket(s);
        s := INVALID_SOCKET;
      end;
      if Config.sIDSAddr = '' then
        Continue;
      s := Socket(PF_INET, SOCK_STREAM, IPPROTO_IP);
      if s = INVALID_SOCKET then
        Continue;
      ThreadInfo.nRunFlag := 127;
      HostEnt := gethostbyname(PAnsiChar(@Config.sIDSAddr[1]));
      if HostEnt = nil then
        Continue;
      PInteger(@Name.sin_addr.S_addr)^ := PInteger(HostEnt.h_addr^)^;
      Name.sin_family := HostEnt.h_addrtype;
      Name.sin_port := htons(Config.nIDSPort);
      Name.sin_family := PF_INET;
      ThreadInfo.nRunFlag := 128;
      if connect(s, Name, SizeOf(Name)) = SOCKET_ERROR then
      begin
        closesocket(s);
        s := INVALID_SOCKET;
        Continue;
      end;
      argp := 1;
      if ioctlsocket(s, FIONBIO, argp) = SOCKET_ERROR then
      begin
        closesocket(s);
        s := INVALID_SOCKET;
        Continue;
      end;
      ThreadInfo.nRunFlag := 129;
      Config.DBSocket := s;
      Config.boIDSocketConnected := True;
      MainOutMessage(Format(sIDServerConnected, [Config.sIDSAddr, Config.nIDSPort]));
    end;
    readfds.fd_count := 1;
    readfds.fd_array[0] := s;
    timeout.tv_sec := 0;
    timeout.tv_usec := 20;
    ThreadInfo.nRunFlag := 130;
    nRet := select(0, @readfds, nil, nil, @timeout);
    if nRet = SOCKET_ERROR then
    begin
      ThreadInfo.nRunFlag := 131;
      nRet := WSAGetLastError;
      if nRet = WSAEWOULDBLOCK then
      begin
        Sleep(10);
        Continue;
      end;
      ThreadInfo.nRunFlag := 132;
      nRet := WSAGetLastError;
      Config.nIDSocketWSAErrCode := nRet - WSABASEERR;
      Inc(Config.nIDSocketErrorCount);
      Config.DBSocket := INVALID_SOCKET;
      Sleep(100);
      Config.boIDSocketConnected := False;
      closesocket(s);
      s := INVALID_SOCKET;
      Continue;
    end;
    boRecvData := True;
    ThreadInfo.nRunFlag := 133;
    while (nRet > 0) do
    begin
      IDSocketRead(Config);
      Dec(nRet);
    end;
  end;
  if Config.DBSocket <> INVALID_SOCKET then
  begin
    Config.DBSocket := INVALID_SOCKET;
    Config.boIDSocketConnected := False;
  end;
  if s <> INVALID_SOCKET then
  begin
    closesocket(s);
  end;
end;

procedure IDSocketThread(ThreadInfo: pTThreadInfo); stdcall;
var
  nErrorCount: Integer;
resourcestring
  sExceptionMsg = '[Exception] DBSocketThread ErrorCount = %d';
begin
  nErrorCount := 0;
  while True do
  begin
    try
      IDSocketProcess(@g_Config, ThreadInfo);
      Break;
    except
      Inc(nErrorCount);
      if nErrorCount > 10 then
        Break;
      MainOutMessage(Format(sExceptionMsg, [nErrorCount]));
    end;
  end;
  ExitThread(0);
end;
{$IFDEF LOG_SESSION}

initialization
  InitializeCriticalSection(g_LockLogSession);


finalization
  DeleteCriticalSection(g_LockLogSession);
{$ENDIF}

end.

