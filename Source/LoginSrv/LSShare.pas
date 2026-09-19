unit LSShare;

interface
uses
  Windows, Messages, Classes, SysUtils, SyncObjs, MudUtil, JSocket, IniFiles, Grobal2, SDK, Common, WinInet, IdURI;
resourcestring
  g_sUpDateTime = '更新日期: 2025/04/28';
  g_sProductName = '程序名称: GxxM2登录服务器 V1.0';
  g_sProgram = '程序制作: GxxM2';
  g_sWebSite = '程序网站: http://www.gxxm2.com';

const
  ControlMsgHeaderIdent = $EA59C795;

  SMSG_CHECK_PASSWORD_OK = 1;                                         // 无效密码
  SMSG_CHECK_PASSWORD_FAIL = 2;                                       // 密码验证成功
  SMSG_RESPONSE_ACCOUNT_LIST = 3;                                     // 返回帐户列表
  SMSG_EDIT_ACCOUNT_INFO_OK = 4;
  SMSG_EDIT_ACCOUNT_INFO_FAIL = 5;
  SMSG_HEARTBEAT = 6;

  CMSG_CHECK_PASSWORD = 1;
  CMSG_QUERY_ACCOUNT_LIST = 2;
  CMSG_EDIT_ACCOUNT_INFO = 3;
  CMSG_HEARTBEAT = 6;
  
type
  TResultEvent = procedure (return: string; return1: string; return2: string; return3: string; return4: string; userinfo: Integer);
  THttpThread = class(TThread)
  private
    FPostType: Byte;
    FParam1: string;
    FParam2: string;
    FParam3: string;
    FParam4: string;     
    FResult: string;  
    //FResult1: string;
    //FResult2: string;
    FResultUser: Integer;
    FResultEvent: TResultEvent;
    procedure SendSMS(sPhone, sCode: string);
    procedure CheckRealName(sName, sId: string);    
    procedure QuickLogin(sToken, sUID, sID: string);
    procedure Post(URL, Data, Param: string; Res: TStream);
    procedure CallEvent;
  protected
    procedure Execute; override;
  public
    property ResultEvent: TResultEvent read FResultEvent write FResultEvent;
    constructor Create(const btPostType: Byte; Param1: string; Param2: string; Param3: string; Param4: string; resultevent: TResultEvent; userinfo: Integer);
  end;

  TConnInfo = record                                                  //Size 0x20 Address: 0x00468601
    sAccount: string;                                                 //0x00
    sIPaddr: string;                                                  //0x04
    sServerName: string;                                              //0x08
    nSessionID: Integer;                                              //0x0C
    boPayCost: Boolean;                                               //0x10
    bo11: Boolean;                                                    //0x11
    dwKickTick: LongWord;                                             //0x14
    dwStartTick: LongWord;                                            //0x18
    boKicked: Boolean;                                                //0x1C
    nLockCount: Integer;
  end;
  pTConnInfo = ^TConnInfo;
  TGateInfo = record                                                  //Size 0x14 Address: 0x004686A0
    Socket: TCustomWinSocket;                                         //0x00
    sRemoteAddress: string;
    sIPaddr: string;                                                  //0x04
    sReceiveMsg: string;                                              //0x08
    UserList: TList;                                                  //0x0C
    dwKeepAliveTick: LongWord;                                        //0x10
  end;
  pTGateInfo = ^TGateInfo;

  TMacID = array[0..15] of Byte;

  TUserInfo = record                                                  //Size 0x68 Address: 0x004686C8
    sAccount: string;                                                 //0x00
    sUserIPaddr: string;                                              //0x0B
    sGateIPaddr: string;                                              //用户连接到网关，网关的连接IP
    sSockIndex: string;                                               //0x20
    nVersionDate: Integer;                                            //0x24
    boCertificationOK: Boolean;                                       //0x28
    nSessionID: Integer;                                              //0x2C
    boPayCost: Boolean;                                               //0x30
    nIDDay: Integer;                                                  //0x34
    nIDHour: Integer;                                                 //0x38
    nIPDay: Integer;                                                  //0x3C
    nIPHour: Integer;                                                 //0x40
    dtDateTime: TDateTime;                                            //0x48
    boSelServer: Boolean;                                             //0x50
    Socket: TCustomWinSocket;                                         //0x54
    sReceiveMsg: string;                                              //0x58
    dwTime5C: LongWord;                                               //0x5C
    dwClientTick: LongWord;                                           //0x64

    sRandomCode: array[TRandCodeType] of string;
    nRandomCodeErrorMaxCount: array[TRandCodeType] of Integer;
    nRandomCodeRefreshMaxCount: array[TRandCodeType] of Integer;
    boRandomCodeOK: array[TRandCodeType] of Boolean;

    boL2PasswordOK: Boolean;                                          //
    sL2Password: string;
    sLoginMAC: string[32];
    nL2ErrorCount: Integer;
    boDealyClose: Boolean;
    dwDelayCloseTick: LongWord;

    sLoginPhone: string[11];//登录的手机号
    sVerificationCode: string[6];//手机验证码
    dwLastVerificationCodeTick: LongWord;//手机验证码获取时间 (间隔以及到期时间都用这个变量判断)

    sReLoginUser: string;
    sReLoginPassword: string;

    Gate: pTGateInfo
  end;
  pTUserInfo = ^TUserInfo;

  TControlSessionInfo = record
    Socket: TCustomWinSocket;
    ConnectTick: LongWord;
    RecvText: string;
    RecvTick: LongWord;
    SendHeartbeatTick: LongWord;
    IsPasswordOK: Boolean;
    DelayClose: Boolean;
    DelayCloseTick: LongWord;
  end;
  PControlSessionInfo = ^TControlSessionInfo;

  PControlMsgHeader = ^TControlMsgHeader;
  TControlMsgHeader = record
    dwCode: LongWord;
    dwCmd: LongWord;
    nLength: LongWord;
  end;

  TGateNet = record
    sIPaddr: string;
    nPort: Integer;
    boEnable: Boolean;
  end;
  TGateRoute = record
    sServerName: string;
    sTitle: string;
    sRemoteAddr: string;
    sPublicAddr: string;
    nSelIdx: Integer;
    Gate: array[0..9] of TGateNet;
  end;

  pTConfig = ^TConfig;
  TConfig = record
    IniConf: TIniFile;
    boRemoteClose: Boolean;
    sDBServer: string[30];                                            //0x00475368
    nDBSPort: Integer;                                                //0x00475374
    sFeeServer: string[30];                                           //0x0047536C
    nFeePort: Integer;                                                //0x00475378
    sLogServer: string[30];                                           //0x00475370
    nLogPort: Integer;                                                //0x0047537C
    sGateAddr: string[30];
    nGatePort: Integer;
    sServerAddr: string[30];
    sServerName: string[30];
    nServerPort: Integer;
    sMonAddr: string[30];
    nMonPort: Integer;
    nControlPort: Integer;
    sControlPassword: string[30];

    boShowBlockIPLog: Boolean;

    sGateIPaddr: string[30];                                          //当前处理的网关连接IP地址
    sIdDir: string[50];
    sWebLogDir: string[50];
    sFeedIDList: string[50];
    sFeedIPList: string[50];
    sCountLogDir: string[50];
    sChrLogDir: string[50];
    boTestServer: Boolean;
    boEnableMakingID: Boolean;
    boDynamicIPMode: Boolean;
    boEnableGetbackPassword: Boolean;
    boGetbackPasswordCheckAll: Boolean;
    boDisableIDSamePassword: Boolean;                                 // 禁止帐户和密码相同 chongchong 2013-08-28
    boDisableQuizSameAnswer: Boolean;                                 // 禁止问题答案相同 chongchong 2016-03-11
    boDisableIDSameL2Password: Boolean;
    boDisableL2SamePassword: Boolean;
    boDisablePwdSameChr: Boolean;
    boDisablePwdAllNum: Boolean;
    boDisablePwdAllLetter: Boolean;

    boAutoClearID: Boolean;
    boUnLockAccount: Boolean;
    nReadyServers: Integer;

    dwAutoClearTime: Integer;
    dwUnLockAccountTime: Integer;

    GateCriticalSection: TRTLCriticalSection;
    GateList: array[0..20 - 1] of TGateInfo;                          //TList;

    SessionList: TGList;
    ServerNameList: TStringList;
    AccountCostList: TQuickList;
    IPaddrCostList: TQuickList;
    boShowDetailMsg: Boolean;

    boRandomCode: array[TRandCodeType] of Boolean;

    btLoginWaveValue: Byte;
    btOtherWaveValue: Byte;

    nRandomCodeErrorMaxCount: Integer;
    nRandomCodeRefreshMaxCount: Integer;
    boEnabledL2Password: Boolean;
    boChangedMACCheckL2: Boolean;
    boChangedIPCheckL2: Boolean;
    boAlwaysCheckL2: Boolean;

    dwProcessGateTick: LongWord;
    dwProcessGateTime: LongWord;
    nRouteCount: Integer;

    nDataSaveDBType: Integer;
    sDataSaveDBServer: string;
    wDataSaveDBPort: Word;
    sDataSaveDBUser: string;
    sDataSaveDBPassword: string;
    sDataSaveDataBase: string;

    boNewLoginDlg: Boolean;
    boNewLoginInto: Boolean;
    boNewLoginPhone: Boolean;
    boNewLoginMustHasPhone: Boolean;

    GateRoute: array[0..59] of TGateRoute;
  end;

  TSafeHashStringList = class(THashedStringList)
  private
    FCS: TRTLCriticalSection;
  public
    constructor Create();
    destructor Destroy; override;
    procedure Lock;
    procedure UnLock;
  end;

function GetCodeMsgSize(X: Double): Integer;
function CheckAccountName(sName: string): Boolean;
function GetSessionID(): Integer;
procedure SaveGateConfig();
function GetGatePublicAddr(sGateIP: string): string;
function GenSpaceString(sStr: string; nSpaceCOunt: Integer): string;
procedure MainOutMessage(sMsg: string);
procedure SendGameCenterMsg(wIdent: Word; sSendMsg: string);

function CheckValidMac(S: string): Boolean;
function MacToHex(MacID: TMacID): string;
function Date2MyDate(Dt: TDateTime): Integer;
function MyDate2Date(dt: Integer): TDateTime;

function tick_diff(tick_start, tick_end: Cardinal): Cardinal;
                                      

procedure HttpPost(btType: Byte; Param1: string; Param2: string; Param3: string; Param4: string; resultevent: TResultEvent; userinfo: Integer);

var
  g_Config: TConfig = (
    boRemoteClose: False;
    sDBServer: '127.0.0.1';
    nDBSPort: 16300;
    sFeeServer: '127.0.0.1';
    nFeePort: 16301;
    sLogServer: '127.0.0.1';
    nLogPort: 16301;
    sGateAddr: '0.0.0.0';
    nGatePort: 5500;
    sServerAddr: '0.0.0.0';
    nServerPort: 5600;
    sMonAddr: '0.0.0.0';
    nMonPort: 3000;
    nControlPort: 0;
    sControlPassword: 'bmm2'; //'geem2'; //HZQ
    boShowBlockIPLog: False;
    sIdDir: '.\DB\';                                                  //0x00470D04
    sWebLogDir: '.\Share\';                                           //0x00470D08
    sFeedIDList: '.\FeedIDList.txt';                                  //0x00470D0C
    sFeedIPList: '.\FeedIPList.txt';                                  //0x00470D10
    sCountLogDir: '.\CountLog\';                                      //0x00470D14
    sChrLogDir: '.\ChrLog\';
    boTestServer: True;
    boEnableMakingID: True;
    boDynamicIPMode: False;
    boEnableGetbackPassword: True;
    boGetbackPasswordCheckAll: False;
    boDisableIDSamePassword: False;                                   // 禁止帐户和密码相同 chongchong 2013-08-28
    boDisableQuizSameAnswer: False;
    boDisableIDSameL2Password: False;
    boDisableL2SamePassword: False;
    boDisablePwdSameChr: False;
    boDisablePwdAllNum: False;
    boDisablePwdAllLetter: False;

    boAutoClearID: False;
    boUnLockAccount: True;
    nReadyServers: 0;
    boShowDetailMsg: False;

    boRandomCode: (False, False, False, False);

    btLoginWaveValue: 8;
    btOtherWaveValue: 4;

    nRandomCodeErrorMaxCount: 3;
    nRandomCodeRefreshMaxCount: 5;
    boEnabledL2Password: False;
    boChangedMACCheckL2: False;
    boChangedIPCheckL2: False;
    boAlwaysCheckL2: False;

    nDataSaveDBType: 0;
    sDataSaveDBServer: '';
    wDataSaveDBPort: 3306;
    sDataSaveDBUser: '';
    sDataSaveDBPassword: '';
    sDataSaveDataBase: '';

    boNewLoginDlg: False; //HZQ 默认为假，防止，有些服务端配置缺失导致的登录UI不正常
    boNewLoginInto: True;
    boNewLoginPhone: True;
    boNewLoginMustHasPhone: True;
    );

  nOnlineCountMin: Integer;                                           //0x00475390
  nOnlineCountMax: Integer;                                           //0x00475394
  nMemoHeigh: Integer;                                                //0x00475398
  g_OutMessageCS: TRTLCriticalSection;
  g_MainMsgList: TStringList;                                         //0x0047539C
  CS_DB: TCriticalSection;                                            //0x004753A0
  n4753A4: Integer;                                                   //0x004753A4
  n4753A8: Integer;                                                   //0x004753A8
  n4753B0: Integer;                                                   //0x004753B0

  //sIdDir            :String = '.\DB\';                              //0x00470D04
  //sWebLogDir        :String = '.\Share\';                           //0x00470D08
  //sFeedIDList       :String = '.\FeedIDList.txt';                   //0x00470D0C
  //sFeedIPList       :String = '.\FeedIPList.txt';                   //0x00470D10
  //sCountLogDir      :String = '.\CountLog\';                        //0x00470D14
  //sChrLogDir        :String = '.\ChrLog\';
  //boTestServer      :Boolean = False;                               //0x00470D18
  //boEnableMakingID  :Boolean = True;                                //0x00470D18

  n47328C: Integer;

  nSessionIdx: Integer;                                               //0x00473294

  g_n472A6C: Integer;
  g_n472A70: Integer;
  g_n472A74: Integer;
  g_boDataDBReady: Boolean;                                           //0x00472A78
  bo470D20: Boolean;

  nVersionDate: Integer = 20100705;

  g_ServerAddr: array[0..99] of string[15];
  g_ServerAddrCount: Integer = 0;

  g_dwGameCenterHandle: THandle;

  g_ControlIPFile: string;
  g_ControlIPList: TSafeHashStringList;
  g_ControlSessionArray: array[0..30 - 1] of TControlSessionInfo;

  g_DisablePasswordFile: string;
  g_DisablePasswordList: TStringList;

implementation

function MacToHex(MacID: TMacID): string;
var
  I: Byte;
  IsZero: Boolean;
const
  Digits: array[0..15] of char =
  ('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F');
begin
  Result := '';

  IsZero := True;
  for I := 0 to Length(MacID) - 1 do
  begin
    if MacID[I] <> 0 then
    begin
      IsZero := False;
      Break;
    end;
  end;

  if not IsZero then
  begin
    for I := 0 to Length(MacID) - 1 do
    begin
      Result := Result + Digits[(MacID[I] shr 4) and $0F] + Digits[MacID[I] and $0F];
    end;
  end;
end;

function CheckValidMac(S: string): Boolean;
var
  I: Integer;
begin
  Result := False;
  if Length(S) = 32 then
  begin
    for I := 1 to Length(S) do
    begin
      if not (S[I] in ['0'..'9', 'A'..'F', 'a'..'f']) then
      begin
        Exit;
      end;
    end;

    Result := True;
  end;
end;

function Date2MyDate(Dt: TDateTime): Integer;
var
  Y, M, D: Word;
begin
  DecodeDate(Dt, Y, M, D);
  Result := Y * 10000 + M * 100 + D;
end;

function MyDate2Date(dt: Integer): TDateTime;
var
  Y, M, D: Word;
begin
  Result := 0;

  if dt > 10000000 then
  begin
    Y := dt div 10000;
    M := (dt - Y * 10000) div 100;
    D := dt mod 100;
    TryEncodeDate(Y, M, D, Result);
  end;
end;

function GetCodeMsgSize(X: Double): Integer;
begin
  if INT(X) < X then
    Result := TRUNC(X) + 1
  else
    Result := TRUNC(X)
end;

function CheckAccountName(sName: string): Boolean;
var
  I: Integer;
  nLen: Integer;
begin
  Result := False;
  if sName = '' then exit;
  Result := True;
  nLen := length(sName);
  I := 1;
  while (True) do
  begin
    if I > nLen then break;
    if (sName[I] < '0') or (sName[I] > 'z') then
    begin
      if (sName[I] = '@') or (sName[I] = '.') then
      begin
        Inc(I);
        Continue;
      end;
      Result := False;
      if (sName[I] >= #$B0) and (sName[I] <= #$C8) then
      begin
        Inc(I);
        if I <= nLen then
          if (sName[I] >= #$A1) and (sName[I] <= #$FE) then Result := True;
      end;
      if not Result then break;
    end;
    Inc(I);
  end;
end;

function GetSessionID(): Integer;
begin
  Inc(nSessionIdx);
  if nSessionIdx >= High(Integer) then
  begin
    nSessionIdx := 2;
  end;
  Result := nSessionIdx;
end;
//0046D4F4

procedure SaveGateConfig();
var
  SaveList: TStringList;
  i, n8: Integer;
  s10, sC: string;
begin
  SaveList := TStringList.Create;
  SaveList.Add(';No space allowed');
  SaveList.Add(GenSpaceString(';Server', 15) + GenSpaceString('Title', 15) + GenSpaceString('Remote', 17) + GenSpaceString('Public', 17) + 'Gate...');
  for i := 0 to g_Config.nRouteCount - 1 do
  begin
    sC := GenSpaceString(g_Config.GateRoute[i].sServerName, 15) + GenSpaceString(g_Config.GateRoute[i].sTitle, 15) + GenSpaceString(g_Config.GateRoute[i].sRemoteAddr, 17) + GenSpaceString(g_Config.GateRoute[i].sPublicAddr, 17);
    n8 := 0;
    while (True) do
    begin
      s10 := g_Config.GateRoute[i].Gate[n8].sIPaddr;
      if s10 = '' then break;
      if not g_Config.GateRoute[i].Gate[n8].boEnable then
        s10 := '*' + s10;
      s10 := s10 + ':' + IntToStr(g_Config.GateRoute[i].Gate[n8].nPort);
      sC := sC + GenSpaceString(s10, 17);
      Inc(n8);
      if n8 >= 10 then break;
    end;
    SaveList.Add(sC);
  end;
  SaveList.SaveToFile('.\!addrtable.txt');
  SaveList.Free;
end;
//0046D7F8

function GetGatePublicAddr(sGateIP: string): string;
var
  I: Integer;
begin
  Result := sGateIP;
  for I := 0 to g_Config.nRouteCount - 1 do
  begin
    if g_Config.GateRoute[I].sRemoteAddr = sGateIP then
    begin
      Result := g_Config.GateRoute[I].sPublicAddr;
      break;
    end;
  end;
end;
//004541C4

function GenSpaceString(sStr: string; nSpaceCOunt: Integer): string;
var
  I: Integer;
begin
  Result := sStr + ' ';
  for I := 1 to nSpaceCOunt - length(sStr) do
  begin
    Result := Result + ' ';
  end;
end;
//00468F00

procedure MainOutMessage(sMsg: string);
begin
  EnterCriticalSection(g_OutMessageCS);
  try
    g_MainMsgList.Add('[' + DateTimeToStr(Now) + '] ' + sMsg)
  finally
    LeaveCriticalSection(g_OutMessageCS);
  end;
end;

procedure SendGameCenterMsg(wIdent: Word; sSendMsg: string);
var
  SendData: TCopyDataStruct;
  nParam: Integer;
begin
  nParam := MakeLong(Word(tLoginSrv), wIdent);
  SendData.cbData := Length(sSendMsg) + 1;
  GetMem(SendData.lpData, SendData.cbData);
  StrCopy(SendData.lpData, PChar(sSendMsg));
  SendMessage(g_dwGameCenterHandle, WM_COPYDATA, nParam, Cardinal(@SendData));
  FreeMem(SendData.lpData);
end;

function tick_diff(tick_start, tick_end: Cardinal): Cardinal;
begin
  if tick_end >= tick_start then
    result := tick_end - tick_start
  else
    result := High(Cardinal) - tick_start + tick_end;
end;

{ TSafeHashStringList }

constructor TSafeHashStringList.Create;
begin
  inherited Create;
  InitializeCriticalSection(FCS);
end;

destructor TSafeHashStringList.Destroy;
begin
  DeleteCriticalSection(FCS);
  inherited;
end;

procedure TSafeHashStringList.Lock;
begin
  EnterCriticalSection(FCS);
end;

procedure TSafeHashStringList.UnLock;
begin
  LeaveCriticalSection(FCS);
end;







procedure HttpPost(btType: Byte; Param1: string; Param2: string; Param3: string; Param4: string; resultevent: TResultEvent; userinfo: Integer);
begin
  THttpThread.Create(btType, Param1, Param2, Param3, Param4, resultevent, userinfo);
end;

constructor THttpThread.Create(const btPostType: Byte; Param1: string; Param2: string; Param3: string; Param4: string; resultevent: TResultEvent; userinfo: Integer);
begin
  FreeOnTerminate := True;
  FPostType := btPostType;
  FParam1 := Param1;
  FParam2 := Param2;
  FParam3 := Param3;
  FParam4 := Param4;
  FResultUser := userinfo;
  FResultEvent := resultevent;
  inherited Create(False);
end;

procedure THttpThread.CallEvent;
begin
  if Assigned(FResultEvent) then
  FResultEvent(FResult, FParam1, FParam2, FParam3, FParam4, FResultUser);
end;

procedure THttpThread.Execute;
begin
  FreeOnTerminate := True;
  case FPostType of
    0:
      begin
        SendSMS(FParam1, FParam2);
//        Synchronize(CallEvent);
      end;
    1:
      begin
        CheckRealName(FParam1, FParam2);
        Synchronize(CallEvent);
      end;
    2:
      begin
        QuickLogin(FParam1, FParam2, FParam3);
        Synchronize(CallEvent);
      end;
  end;
end;

procedure THttpThread.SendSMS(sPhone, sCode: string);
var
//  ResponseStr: string;
  ResponseStream: TStringStream; //返回信息
begin          
  ResponseStream := TStringStream.Create('');
  //Post('https://gm.geem2.com/gmserver/gmuser/client/sendSMS', '{"mobile":"' + sPhone + '","code":"' + sCode + '"}', '', ResponseStream);
  //HZQ 20230407
  Post('https://gm.gxxm2.com/gmserver/gmuser/client/sendSMS', '{"mobile":"' + sPhone + '","code":"' + sCode + '"}', '', ResponseStream);

  FResult := UTF8Decode(ResponseStream.DataString);
end;

procedure THttpThread.CheckRealName(sName, sId: string);
var
//  ResponseStr: string;
  ResponseStream: TStringStream; //返回信息
begin
  ResponseStream := TStringStream.Create('');
  //Post('https://gm.geem2.com/gmserver/gmuser/client/realm', '{"idcard":"' + sId + '","username":"' + Utf8Encode(sName) + '"}', '', ResponseStream);
  //HZQ 20230407
  Post('https://gm.gxxm2.com/gmserver/gmuser/client/realm', '{"idcard":"' + sId + '","username":"' + Utf8Encode(sName) + '"}', '', ResponseStream);
  FResult := UTF8Decode(ResponseStream.DataString);
end;

procedure THttpThread.QuickLogin(sToken, sUID, sID: string);
var
  sendstr: string;
  ResponseStream: TStringStream; //返回信息
begin
  ResponseStream := TStringStream.Create('');
  sendstr := '?token=' + Utf8Encode(sToken) + '&uid=' + Utf8Encode(sUID) + '&product_code=64424235419854008746518503196222';
  if sID <> '' then
    sendstr := sendstr + '&channel_code=' + Utf8Encode(sID);
//  Post('http://checkuser.quickapi.net/v2/checkUserInfo', '{"token":"' + Utf8Encode(sToken) + '","uid":"' + Utf8Encode(sUID) + '","product_code":"64424235419854008746518503196222"}', ResponseStream);
  Post('http://checkuser.quickapi.net/v2/checkUserInfo', '', sendstr, ResponseStream);
  FResult := UTF8Decode(ResponseStream.DataString);
end;

procedure THttpThread.Post(URL, Data, Param: string; Res: TStream);
var
  hInt, hConn, hreq: HINTERNET;
  buffer: PChar;
  dwRead, dwFlags: cardinal;
  port: Word;
  uri: TIdURI;
  proto, host, path: string;
  {dwError,} dwBuffLen: Cardinal;
begin
  uri := TIdURI.Create(URL);
  host := uri.Host;
  path := uri.Path + uri.Document + Param;
  proto := uri.Protocol;
  uri.Free;
  if UpperCase(proto) = 'HTTPS' then
  begin
    port := INTERNET_DEFAULT_HTTPS_PORT;
    dwFlags := INTERNET_FLAG_SECURE;
  end
  else
  begin
    port := INTERNET_INVALID_PORT_NUMBER;
    dwFlags := INTERNET_FLAG_RELOAD;
  end;
  hInt := InternetOpen('Delphi', INTERNET_OPEN_TYPE_PRECONFIG, nil, nil, 0);
  hConn := InternetConnect(hInt, PChar(host), port, nil, nil, INTERNET_SERVICE_HTTP, 0, 0);
  hreq := HttpOpenRequest(hConn, 'POST', PChar(path), 'HTTP/1.1', nil, nil, dwFlags, 0);

  InternetQueryOption(hreq, INTERNET_OPTION_SECURITY_FLAGS, @dwFlags, dwBuffLen);

  dwFlags := dwFlags or SECURITY_FLAG_IGNORE_REVOCATION;

  InternetSetOption(hreq, INTERNET_OPTION_SECURITY_FLAGS, @dwFlags, sizeof(dwFlags)) ;

  GetMem(buffer, 65536);
  if HttpSendRequest(hreq, nil, 0, PChar(Data), Length(Data)) then
  begin
    dwRead := 0;
    repeat
      InternetReadFile(hreq, buffer, 65536, dwRead);
      if dwRead <> 0 then
        Res.Write(buffer^, dwRead);
    until dwRead = 0;
  end;
//  ShowMessage(IntToStr(GetLastError));
  InternetCloseHandle(hreq);
  InternetCloseHandle(hConn);
  InternetCloseHandle(hInt);
  FreeMem(buffer);
end;




initialization
  begin
    InitializeCriticalSection(g_OutMessageCS);
    g_MainMsgList := TStringList.Create;
  end;

finalization
  begin
    g_MainMsgList.Free;
    DeleteCriticalSection(g_OutMessageCS);
  end;
end.
