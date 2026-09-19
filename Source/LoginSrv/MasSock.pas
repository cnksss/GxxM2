unit MasSock;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics,
  Controls, Forms, Dialogs, StdCtrls, JSocket;
type
  TMsgServerInfo = record
    sReceiveMsg: string;
    Socket: TCustomWinSocket;
    sServerName: string;                                              //0x08
    nServerIndex: Integer;                                            //0x0C
    nOnlineCount: Integer;                                            //0x10
    dwKeepAliveTick: LongWord;                                        //0x14
    sIPaddr: string;
  end;
  pTMsgServerInfo = ^TMsgServerInfo;
  TLimitServerUserInfo = record
    sServerName: string;
    sName: string;
    nLimitCountMin: Integer;
    nLimitCountMax: Integer;
  end;
  pTLimitServerUserInfo = ^TLimitServerUserInfo;
  TFrmMasSoc = class(TForm)
    MSocket: TServerSocket;

    procedure FormCreate(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure MSocketClientConnect(Sender: TObject;
      Socket: TCustomWinSocket);
    procedure MSocketClientDisconnect(Sender: TObject;
      Socket: TCustomWinSocket);
    procedure MSocketClientError(Sender: TObject; Socket: TCustomWinSocket;
      ErrorEvent: TErrorEvent; var ErrorCode: Integer);
    procedure MSocketClientRead(Sender: TObject; Socket: TCustomWinSocket);
  private
    procedure SortServerList(nIndex: Integer);
    procedure RefServerLimit(sServerName: string);
    function LimitName(sServerName: string): string;
    procedure LoadUserLimit;
    { Private declarations }
  public
    m_ServerList: TList;
    procedure LoadServerAddr();
    function CheckReadyServers(): Boolean;
    procedure SendServerMsg(wIdent: Word; sServerName, sMsg: string);
    procedure SendServerMsgA(wIdent: Word; sMsg: string);
    function IsNotUserFull(sServerName: string): Boolean;
    function ServerStatus(sServerName: string): Integer;
    function GetOnlineHumCount(): Integer;
    procedure StartService;
  end;

var
  FrmMasSoc: TFrmMasSoc;
  nUserLimit: Integer;
  UserLimit: array[0..99] of TLimitServerUserInfo;

implementation

uses LSShare, LMain, HUtil32, Grobal2, Common, AccountDB, EDCode;

{$R *.DFM}

{.$DEFINE LOG_SESSION}

{$IFDEF LOG_SESSION}
type
  TSessionOperate = (soOpen, soClose, soSendOpen, soSendClose);

var
  g_LockLogSession: TRTLCriticalSection;

procedure LogSession(Operate: TSessionOperate; Msg: string; boWriteDate: Boolean = False);
var
  sFilePath: string;
  flname: string;
  fhandle: TextFile;
  Year, Month, Day: Word;
begin
  EnterCriticalSection(g_LockLogSession);
  try
    case Operate of
      soOpen: Msg := '打开 ' + Msg;
      soClose: Msg := '关闭 ' + Msg;
      soSendOpen: Msg := '发送打开 ' + Msg;
      soSendClose: Msg := '发送关闭 ' + Msg;
    end;

    if boWriteDate then
      Msg := FormatDateTime('yyyy-mm-dd hh:mm:ss', Now) + ' ' + Msg;

    DecodeDate(Now, Year, Month, Day);

    sFilePath := ExtractFilePath(Application.ExeName) + 'session_log\' + IntToStr(Year) + '-' + IntToStr2(Month) + '\';
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


procedure TFrmMasSoc.FormCreate(Sender: TObject);
begin
  m_ServerList := TList.Create;
  LoadServerAddr();
  LoadUserLimit();
end;

procedure TFrmMasSoc.StartService;
var
  Config: pTConfig;
begin
  Config := @g_Config;
  MSocket.Address := Config.sServerAddr;
  MSocket.Port := Config.nServerPort;
  try
    MSocket.Active := True;
    MainOutMessage(Format('游戏中心服务启动成功(%s:%d)...', [Config.sServerAddr, Config.nServerPort]));
  except
    MainOutMessage('TFrmMasSoc.StartService');
  end;
end;

procedure TFrmMasSoc.MSocketClientConnect(Sender: TObject;
  Socket: TCustomWinSocket);
var
  I: Integer;
  sRemoteAddr: string;
  boAllowed: Boolean;
  MsgServer: pTMsgServerInfo;
begin
  sRemoteAddr := Socket.RemoteAddress;
  boAllowed := False;
  for I := Low(g_ServerAddr) to g_ServerAddrCount - 1 do
  begin
    if sRemoteAddr = g_ServerAddr[I] then
    begin
      boAllowed := True;
      break;
    end;
  end;
  
  if boAllowed then
  begin
    New(MsgServer);
    FillChar(MsgServer^, SizeOf(TMsgServerInfo), #0);
    MsgServer.sReceiveMsg := '';
    MsgServer.Socket := Socket;
    m_ServerList.Add(MsgServer);
  end
  else
  begin
    MainOutMessage('非法地址连接:' + sRemoteAddr);
    Socket.Close;
  end;
end;
//00465C54

procedure TFrmMasSoc.MSocketClientDisconnect(Sender: TObject;
  Socket: TCustomWinSocket);
var
  I: Integer;
  MsgServer: pTMsgServerInfo;
begin
  for I := 0 to m_ServerList.Count - 1 do
  begin
    MsgServer := m_ServerList.Items[I];
    if MsgServer.Socket = Socket then
    begin
      Dispose(MsgServer);
      m_ServerList.Delete(I);
      break;
    end;
  end;
end;

procedure TFrmMasSoc.MSocketClientError(Sender: TObject;
  Socket: TCustomWinSocket; ErrorEvent: TErrorEvent;
  var ErrorCode: Integer);
begin
  ErrorCode := 0;
  Socket.Close;
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

procedure TFrmMasSoc.MSocketClientRead(Sender: TObject;
  Socket: TCustomWinSocket);
resourcestring
  sFormatMsg = '(%d/%s)';
var
  I, II: Integer;
  MsgServer: pTMsgServerInfo;
  sReviceMsg: string;
  sMsg: string;
  sCode: string;
  sAccount: string;
  sServerName: string;
  sIndex: string;
  sOnlineCount: string;
  nCode: Integer;

  nSessionID: Integer;
  AccountInfo: TAccountInfo;
  ChangeAccountInfo: TAccountInfo2;
  bo21: Boolean;
  nErrCode: Integer;
  WS: WideString;
  WC: WideChar;
  boTemp: Boolean;
  sTemp: string;
begin
  for I := 0 to m_ServerList.Count - 1 do
  begin
    MsgServer := m_ServerList.Items[I];
    if MsgServer.Socket = Socket then
    begin
      sReviceMsg := MsgServer.sReceiveMsg + Socket.ReceiveText;
      while (Pos(')', sReviceMsg) > 0) do
      begin
        sReviceMsg := ArrestStringEx(sReviceMsg, '(', ')', sMsg);
        if sMsg = '' then break;
        sMsg := GetValidStr3(sMsg, sCode, ['/']);
        nCode := StrToIntDef(sCode, -1);
        case nCode of
          SS_SOFTOUTSESSION:
            begin
              sMsg := GetValidStr3(sMsg, sAccount, ['/']);
              CloseUser(MsgServer.sServerName, sAccount, StrToIntDef(sMsg, 0));
            end;
          SS_SERVERINFO:
            begin
              sMsg := GetValidStr3(sMsg, sServerName, ['/']);
              sMsg := GetValidStr3(sMsg, sIndex, ['/']);
              sMsg := GetValidStr3(sMsg, sOnlineCount, ['/']);
              MsgServer.sServerName := sServerName;
              MsgServer.nServerIndex := StrToIntDef(sIndex, 0);
              MsgServer.nOnlineCount := StrToIntDef(sOnlineCount, 0);
              MsgServer.dwKeepAliveTick := GetTickCount();
              SortServerList(I);
              nOnlineCountMin := GetOnlineHumCount();
              if nOnlineCountMin > nOnlineCountMax then nOnlineCountMax := nOnlineCountMin;
              SendServerMsgA(SS_KEEPALIVE, IntToStr(nOnlineCountMin));
              RefServerLimit(sServerName);
            end;
          UNKNOWMSG: SendServerMsgA(UNKNOWMSG, sMsg);
          SS_PASSWORDSUCCESS:
            begin
              sAccount := sMsg;
              nSessionID := GetSessionID;
              {SessionAdd(Config,
                sAccount,
                Socket.RemoteAddress,
                sServerName,
                nSessionID,
                False,
                False);}

              {sMsg := sAccount + '/' + IntToStr(nSessionID) + '/' + IntToStr(Integer(True)) + '/' + IntToStr(5) + '/' + Socket.RemoteAddress;
              sMsg := format(sFormatMsg, [SS_OPENSESSION, sMsg]);
              Socket.SendText(sMsg);}
              SendServerMsg(SS_OPENSESSION, MsgServer.sServerName,
                sAccount + '/' + IntToStr(nSessionID) + '/' + IntToStr(Integer(True)) + '/' + IntToStr(5) + '/' + Socket.RemoteAddress);
            end;

          // M2获取用户注册信息 chongchong 2015-11-13
          SS_GETACCOUNTINFO:
            begin
              // 帐户id/用户名
              sMsg := GetValidStr3(sMsg, sAccount, ['/']);    // smsg 用户名

              if (Length(sAccount) > 0) then
              begin
                if g_AccountDB.GetAccount(sAccount, AccountInfo) then
                begin
                  SendServerMsg(SS_GetAccountInfoRet, MsgServer.sServerName,
                    sMsg + ' /' +
                    AccountInfo.UserName + ' '#9 +
                    AccountInfo.Password + ' '#9 +
                    AccountInfo.Questions1 + ' '#9 +
                    AccountInfo.Answers1 + ' '#9 +
                    AccountInfo.Questions2 + ' '#9 +
                    AccountInfo.Answers2 + ' '#9 +
                    AccountInfo.BirthDay+ ' '#9 +
                    AccountInfo.MobilePhone + ' '#9 +
                    AccountInfo.Phone + ' '#9 +
                    AccountInfo.Mail + ' '#9);
                end;
              end;
            end;

          // M2修改帐户注册信息 chongchong 2020-10-17 00:58:27
          SS_ChangeAccountInfo:
            begin
              if (GetDecodeSize(Length(sMsg)) = SizeOf(TAccountInfo2)) then
              begin
                DecodeString(sMsg, @ChangeAccountInfo, SizeOf(ChangeAccountInfo));
                if not g_AccountDB.GetAccount(ChangeAccountInfo.AccountName, AccountInfo) then
                begin
                  SendServerMsg(SS_ChangeAccountInfoRet, MsgServer.sServerName, '-1/' + sMsg);
                  Exit;
                end;

                nErrCode := 0;
                bo21 := True;

                // 又不改帐号，在此没有必要验证了 
                {
                if (not CheckAccountName(ChangeAccountInfo.AccountName)) or
                  (Length(ChangeAccountInfo.AccountName) < MIN_ACCOUNT_LEN) or // 用户名最小为3位
                  (not CheckAccountValid(ChangeAccountInfo.AccountName)) or
                  (not CheckStringValid(ChangeAccountInfo.AccountName)) then
                begin
                  bo21 := False;
                  nErrCode := -1;
                end;
                }

                if bo21 then
                begin
                  if (Length(ChangeAccountInfo.Password) < 3) or
                    (not CheckStringValid(ChangeAccountInfo.Password)) then
                  begin
                    bo21 := False;
                    nErrCode := -2;
                  end

                  { TODO -ochongchong -c修改 : 禁止ID密码相同 【2013-08-28】 }
                  else if (g_Config.boDisableIDSamePassword) and SameText(ChangeAccountInfo.AccountName, ChangeAccountInfo.Password) then
                  begin
                    bo21 := False;
                    nErrCode := -3;
                  end
                  else
                  begin
                    if g_Config.boDisablePwdSameChr then
                    begin
                      WS := ChangeAccountInfo.Password;
                      WC := WS[1];
                      boTemp := True;
                      for II := 2 to Length(WS) do
                      begin
                        if WS[II] <> WC then
                        begin
                          boTemp := False;
                          Break;
                        end;
                      end;

                      if boTemp then
                      begin
                        bo21 := False;
                        nErrCode := -4;
                      end;
                    end;

                    if bo21 then
                    begin
                      if g_Config.boDisablePwdAllNum then
                      begin
                        boTemp := True;
                        WS := ChangeAccountInfo.Password;
                        for II := 1 to Length(WS) do
                        begin
                          if not (WS[II] in [WideChar('0')..WideChar('9')]) then
                          begin
                            boTemp := False;
                            Break;
                          end;
                        end;

                        if boTemp then
                        begin
                          bo21 := False;
                          nErrCode := -5;
                        end;
                      end;
                    end;

                    if bo21 then
                    begin
                      if g_Config.boDisablePwdAllLetter then
                      begin
                        boTemp := True;
                        WS := ChangeAccountInfo.Password;
                        for II := 1 to Length(WS) do
                        begin
                          if not (WS[II] in [WideChar('a')..WideChar('z'), WideChar('A')..WideChar('Z')]) then
                          begin
                            boTemp := False;
                            Break;
                          end;
                        end;

                        if boTemp then
                        begin
                          bo21 := False;
                          nErrCode := -6;
                        end;
                      end;
                    end;

                    if bo21 then
                    begin
                      if (g_DisablePasswordList.Count > 0) then
                      begin
                        for II := 0 to g_DisablePasswordList.Count - 1 do
                        begin
                          sTemp := g_DisablePasswordList.Strings[II];
                          if (Length(sTemp) > 0) and (Pos(sTemp, ChangeAccountInfo.Password) > 0) then
                          begin
                            bo21 := False;
                            nErrCode := -7;
                          end;
                        end;
                      end;
                    end;
                  end;
                end;

                if bo21 and (not CheckStringValid(ChangeAccountInfo.UserName)) then
                begin
                  bo21 := False;
                  nErrCode := -8;
                end;

                if bo21 and (not CheckStringValid(ChangeAccountInfo.Questions1)) then
                begin
                  bo21 := False;
                  nErrCode := -9;
                end;

                if bo21 and (not CheckStringValid(ChangeAccountInfo.Answers1)) then
                begin
                  bo21 := False;
                  nErrCode := -10;
                end;

                if bo21 and (not CheckStringValid(ChangeAccountInfo.Questions2)) then
                begin
                  bo21 := False;
                  nErrCode := -11;
                end;

                if bo21 and (not CheckStringValid(ChangeAccountInfo.Answers2)) then
                begin
                  bo21 := False;
                  nErrCode := -12;
                end;

                if bo21 and (not CheckStringValid(ChangeAccountInfo.MobilePhone)) then
                begin
                  bo21 := False;
                  nErrCode := -13;
                end;


                if bo21 then
                begin
                  boTemp := False;
                  WS := ChangeAccountInfo.MobilePhone;
                  for II := 1 to Length(WS) do
                  begin
                    if not (WS[II] in [WideChar('0')..WideChar('9')]) then
                    begin
                      boTemp := True;
                      Break;
                    end;
                  end;

                  if boTemp then
                  begin
                    bo21 := False;
                    nErrCode := -13;
                  end;
                end;

                if bo21 and (not CheckStringValid2(ChangeAccountInfo.Mail)) then
                begin
                  bo21 := False;
                  nErrCode := -14;
                end;

                if bo21 and g_Config.boDisableQuizSameAnswer and SameText(ChangeAccountInfo.Questions1, ChangeAccountInfo.Answers1) then
                begin
                  bo21 := False;
                  nErrCode := -15;
                end;

                if bo21 and g_Config.boDisableQuizSameAnswer and SameText(ChangeAccountInfo.Questions2, ChangeAccountInfo.Answers2) then
                begin
                  bo21 := False;
                  nErrCode := -16;
                end;

                if bo21 and (not CheckStringValid(ChangeAccountInfo.L2Password)) then
                begin
                  bo21 := False;
                  nErrCode := -17;
                end;

                // 禁止帐户和二级密码一致
                if bo21 and (Length(ChangeAccountInfo.L2Password) > 0) then
                begin
                  if g_Config.boDisableIDSameL2Password and SameText(AccountInfo.AccountName, ChangeAccountInfo.L2Password) then
                  begin
                    bo21 := False;
                    nErrCode := -18;
                  end
                end;

                if bo21 and (Length(ChangeAccountInfo.L2Password) > 0) then
                begin
                  if g_Config.boDisableL2SamePassword and SameText(ChangeAccountInfo.Password, ChangeAccountInfo.L2Password) then
                  begin
                    bo21 := False;
                    nErrCode := -19;
                  end
                end;

                if bo21 then                  
                begin
                  AccountInfo.Password := ChangeAccountInfo.Password;
                  AccountInfo.UserName := ChangeAccountInfo.UserName;
                  AccountInfo.BirthDay := ChangeAccountInfo.BirthDay;
                  AccountInfo.Questions1 := ChangeAccountInfo.Questions1;
                  AccountInfo.Answers1 := ChangeAccountInfo.Answers1;
                  AccountInfo.Questions2 := ChangeAccountInfo.Questions2;
                  AccountInfo.Answers2 := ChangeAccountInfo.Answers2;
                  AccountInfo.MobilePhone := ChangeAccountInfo.MobilePhone;
                  AccountInfo.Mail := ChangeAccountInfo.Mail;
                  AccountInfo.L2Password := ChangeAccountInfo.L2Password;

                  if g_AccountDB.UpdateAccount(AccountInfo, ufAllField) then
                    nErrCode := 0
                  else
                    nErrCode := -20;
                end;

                SendServerMsg(SS_ChangeAccountInfoRet, MsgServer.sServerName,  IntToStr(nErrCode) + '/' + sMsg);
                Exit;
              end;
            end;
        end;
      end;
    end;
    MsgServer.sReceiveMsg := sReviceMsg;
  end;
end;

procedure TFrmMasSoc.FormDestroy(Sender: TObject);
var
  I: Integer;
  MsgServer: pTMsgServerInfo;
begin
  for I := 0 to m_ServerList.Count - 1 do
  begin
    MsgServer := m_ServerList.Items[I];
    Dispose(MsgServer);
  end;
  m_ServerList.Free;
end;

//00465CF8

procedure TFrmMasSoc.RefServerLimit(sServerName: string);
var
  I: Integer;
  nCount: Integer;
  MsgServer: pTMsgServerInfo;
begin
  try
    nCount := 0;
    for I := 0 to m_ServerList.Count - 1 do
    begin
      MsgServer := m_ServerList.Items[I];
      if (MsgServer.nServerIndex <> 99) and (MsgServer.sServerName = sServerName) then
        Inc(nCount, MsgServer.nOnlineCount);
    end;
    for I := Low(UserLimit) to High(UserLimit) do
    begin
      if UserLimit[I].sServerName = sServerName then
      begin
        UserLimit[I].nLimitCountMin := nCount;
        break;
      end;
    end;
  except
    MainOutMessage('TFrmMasSoc.RefServerLimit');
  end;
end;


//00465E78

function TFrmMasSoc.IsNotUserFull(sServerName: string): Boolean;
var
  I: Integer;
begin
  Result := True;
  for I := Low(UserLimit) to High(UserLimit) do
  begin
    if UserLimit[I].sServerName = sServerName then
    begin
      if UserLimit[I].nLimitCountMin > UserLimit[I].nLimitCountMax then
        Result := False;
      break;
    end;
  end;
end;
//00465F18

procedure TFrmMasSoc.SortServerList(nIndex: Integer);
var
  nC, n10, n14: Integer;
  MsgServerSort: pTMsgServerInfo;
  MsgServer: pTMsgServerInfo;
  nNewIndex: integer;
begin
  try
    if m_ServerList.Count <= nIndex then exit;
    MsgServerSort := m_ServerList.Items[nIndex];
    m_ServerList.Delete(nIndex);
    for nC := 0 to m_ServerList.Count - 1 do
    begin
      MsgServer := m_ServerList.Items[nC];
      if MsgServer.sServerName = MsgServerSort.sServerName then
      begin
        if MsgServer.nServerIndex < MsgServerSort.nServerIndex then
        begin
          m_ServerList.Insert(nC, MsgServerSort);
          exit;
        end
        else
        begin                                                         //00465FD8
          nNewIndex := nC + 1;
          if nNewIndex < m_ServerList.Count then
          begin                                                       //Jacky 增加
            for n10 := nNewIndex to m_ServerList.Count - 1 do
            begin
              MsgServer := m_ServerList.Items[n10];
              if MsgServer.sServerName = MsgServerSort.sServerName then
              begin
                if MsgServer.nServerIndex < MsgServerSort.nServerIndex then
                begin
                  m_ServerList.Insert(n10, MsgServerSort);
                  for n14 := n10 + 1 to m_ServerList.Count - 1 do
                  begin
                    MsgServer := m_ServerList.Items[n14];
                    if (MsgServer.sServerName = MsgServerSort.sServerName) and (MsgServer.nServerIndex = MsgServerSort.nServerIndex) then
                    begin
                      m_ServerList.Delete(n14);
                      exit;
                    end;
                  end;
                  exit;
                end
                else
                begin                                                 //004660D1
                  nNewIndex := n10 + 1;
                end;
              end;
            end;                                                      //00465FF1
            m_ServerList.Insert(nNewIndex, MsgServerSort);
            exit;
          end;
        end;
      end;
    end;
    m_ServerList.Add(MsgServerSort);
  except
    MainOutMessage('TFrmMasSoc.SortServerList');
  end;
end;


//004665BD

procedure TFrmMasSoc.SendServerMsg(wIdent: Word; sServerName, sMsg: string);
var
  I: Integer;
  MsgServer: pTMsgServerInfo;
  sSendMsg: string;
  s18: string;
resourcestring
  sFormatMsg = '(%d/%s)';
begin
  try
{$IFDEF LOG_SESSION}
    if wIdent = SS_OPENSESSION then
      LogSession(soOpen, sMsg)
    else if wIdent = SS_CLOSESESSION then
      LogSession(soClose, sMsg);
{$ENDIF}

    s18 := LimitName(sServerName);
    sSendMsg := format(sFormatMsg, [wIdent, sMsg]);
    for I := 0 to m_ServerList.Count - 1 do
    begin
      MsgServer := pTMsgServerInfo(m_ServerList.Items[I]);
      if MsgServer.Socket.Connected then
      begin
        if (s18 = '') or (MsgServer.sServerName = '') or (CompareText(MsgServer.sServerName, s18) = 0) or (MsgServer.nServerIndex = 99) then
        begin
          MsgServer.Socket.SendText(sSendMsg, True);

        {$IFDEF LOG_SESSION}
          if wIdent = SS_OPENSESSION then
            LogSession(soSendOpen, sSendMsg)
          else if wIdent = SS_CLOSESESSION then
            LogSession(soSendClose, sSendMsg);
        {$ENDIF}
        end;
      end;
    end;
  except
    MainOutMessage('TFrmMasSoc.SendServerMsg');
  end;
end;
//004659BC

procedure TFrmMasSoc.LoadServerAddr();
var
  sFileName: string;
  LoadList: TStringList;
  I, nServerIdx: Integer;
  sLineText: string;
begin
  sFileName := '.\!ServerAddr.txt';
  nServerIdx := 0;
  FillChar(g_ServerAddr, SizeOf(g_ServerAddr), #0);
  if FileExists(sFileName) then
  begin
    LoadList := TStringList.Create;
    LoadList.LoadFromFile(sFileName);
    for I := 0 to LoadList.Count - 1 do
    begin
      sLineText := Trim(LoadList.Strings[i]);
      if (sLineText <> '') and (sLineText[I] <> ';') then
      begin
        if TagCount(sLineText, '.') = 3 then
        begin
          g_ServerAddr[nServerIdx] := sLineText;
          Inc(nServerIdx);
          if nServerIdx >= 100 then break;
        end;
      end;

      g_ServerAddrCount := nServerIdx;
    end;
    LoadList.Free;
  end;
end;


//00466460

function TFrmMasSoc.GetOnlineHumCount(): Integer;
var
  i, nCount: Integer;
  MsgServer: pTMsgServerInfo;
begin
  Result := 0;
  try
    nCount := 0;
    for i := 0 to m_ServerList.Count - 1 do
    begin
      MsgServer := m_ServerList.Items[i];
      if MsgServer.nServerIndex <> 99 then
        Inc(nCount, MsgServer.nOnlineCount);
    end;
    Result := nCount;
  except
    MainOutMessage('TFrmMasSoc.GetOnlineHumCount');
  end;
end;
//00465AD8

function TFrmMasSoc.CheckReadyServers: Boolean;
var
  Config: pTConfig;
begin
  Config := @g_Config;
  Result := False;
  if m_ServerList.Count >= Config.nReadyServers then
    Result := True;
end;

//004664B0

procedure TFrmMasSoc.SendServerMsgA(wIdent: Word; sMsg: string);
var
  I: Integer;
  sSendMsg: string;
  MsgServer: pTMsgServerInfo;
resourcestring
  sFormatMsg = '(%d/%s)';
begin
  try
    sSendMsg := format(sFormatMsg, [wIdent, sMsg]);
    for I := 0 to m_ServerList.Count - 1 do
    begin
      MsgServer := pTMsgServerInfo(m_ServerList.Items[I]);
      if MsgServer.Socket.Connected then MsgServer.Socket.SendText(sSendMsg);
    end;
  except
    on e: Exception do
    begin
      MainOutMessage('TFrmMasSoc.SendServerMsgA');
      MainOutMessage(E.Message);
    end;
  end;
end;
//00465DE0

function TFrmMasSoc.LimitName(sServerName: string): string;
var
  i: Integer;
begin
  try
    Result := '';
    for i := Low(UserLimit) to High(UserLimit) do
    begin
      if CompareText(UserLimit[i].sServerName, sServerName) = 0 then
      begin
        Result := UserLimit[i].sName;
        break;
      end;
    end;
  except
    MainOutMessage('TFrmMasSoc.LimitName');
  end;
end;
//00465730

procedure TFrmMasSoc.LoadUserLimit();
var
  LoadList: TStringList;
  sFileName: string;
  i, nC: integer;
  sLineText, sServerName, s10, s14: string;

begin
  nC := 0;
  sFileName := '.\!UserLimit.txt';
  if FileExists(sFileName) then
  begin
    LoadList := TStringList.Create;
    LoadList.LoadFromFile(sFileName);
    for i := 0 to LoadList.Count - 1 do
    begin
      sLineText := LoadList.Strings[i];
      sLineText := GetValidStr3(sLineText, sServerName, [' ', #9]);
      sLineText := GetValidStr3(sLineText, s10, [' ', #9]);
      sLineText := GetValidStr3(sLineText, s14, [' ', #9]);
      if sServerName <> '' then
      begin
        UserLimit[nC].sServerName := sServerName;
        UserLimit[nC].sName := s10;
        UserLimit[nC].nLimitCountMax := StrToIntDef(s14, 3000);
        UserLimit[nC].nLimitCountMin := 0;
        Inc(nC);
      end;
    end;
    nUserLimit := nC;
    LoadList.Free;
  end
  else
    ShowMessage('[Critical Failure] file not found. .\!UserLimit.txt');
end;

function TFrmMasSoc.ServerStatus(sServerName: string): Integer;
var
  I: Integer;
  nStatus: Integer;
  MsgServer: pTMsgServerInfo;
  boServerOnLine: Boolean;
begin
  Result := 0;

  try
    boServerOnLine := False;
    for I := 0 to m_ServerList.Count - 1 do
    begin
      MsgServer := m_ServerList.Items[I];
      if (MsgServer.nServerIndex <> 99) and (MsgServer.sServerName = sServerName) then
      begin
        boServerOnLine := True;
      end;
    end;
    if not boServerOnLine then exit;

    nStatus := 0;
    for I := Low(UserLimit) to High(UserLimit) do
    begin
      if UserLimit[I].sServerName = sServerName then
      begin
        if UserLimit[I].nLimitCountMin <= UserLimit[I].nLimitCountMax div 2 then
        begin
          nStatus := 1;                                               //空闲
          break;
        end;

        if UserLimit[I].nLimitCountMin <= UserLimit[I].nLimitCountMax - (UserLimit[I].nLimitCountMax div 5) then
        begin
          nStatus := 2;                                               //良好
          break;
        end;
        if UserLimit[I].nLimitCountMin < UserLimit[I].nLimitCountMax then
        begin
          nStatus := 3;                                               //繁忙
          break;
        end;
        if UserLimit[I].nLimitCountMin >= UserLimit[I].nLimitCountMax then
        begin
          nStatus := 4;                                               //满员
          break;
        end;
      end;
    end;

    Result := nStatus;
  except
    MainOutMessage('TFrmMasSoc.ServerStatus');
  end;
end;

{$IFDEF LOG_SESSION}
initialization
  InitializeCriticalSection(g_LockLogSession);

finalization
  DeleteCriticalSection(g_LockLogSession);
{$ENDIF}

end.
