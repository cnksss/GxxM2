unit DataEngn;

interface

uses
  Windows,
  Classes,
  SysUtils,
  Grobal2,
  SDK,
  JSocket,
  Forms,
  CheckUnit,

{$IFDEF CPUX64}
  // VMProtectSDK,
{$ENDIF}
  WinSock,
  ExtCtrls,
  M2Threads,
  Math,
  M2Definition;

type
  TDBTaskType = (ttLoadHuman, ttSaveHuman, ttLoadDummy, ttLoadHero, ttSaveHero, ttChrRename, ttChangeGold, ttGetRankData,
    ttQueryHumanInfo, ttM2CacheRankData, ttBuyPlayer, ttSellPlayerDelegator);

  TDBTaskInfo = record
    TaskID: Word; // TaskID用于返回数据后，删除任务列表使用，此处先不用
    TaskType: TDBTaskType;
    Buf: PByte;
    BufLen: Integer;
  end;

  PTDBTaskInfo = ^TDBTaskInfo;

  TDataEngine = class(TThread)
  private
    FClientSocket: TClientSocket;
    FIsConnected: Boolean;
    FIsActive: Boolean;
    FTryConnectTick: LongWord;
    FCheckConnectTick: LongWord;

    FIsCanSend: Boolean;
    FSendSocketTick: LongWord;

    FIsSendMagicAndStdItems: Boolean;

    FTaskID: Integer;
    FSendM2CacheRankDataTick: LongWord;

    procedure DoDisconnect;

    procedure SetActive(Value: Boolean);

    function GetTaskID: Integer;
  private
    FRecvText: AnsiString;
    FRecvCS: TRTLCriticalSection;

    procedure ClientSocketError(Sender: TObject; Socket: TCustomWinSocket; ErrorEvent: TErrorEvent; var ErrorCode: Integer);
    procedure ClientSocketConnect(Sender: TObject; Socket: TCustomWinSocket);
    procedure ClientSocketDisconnect(Sender: TObject; Socket: TCustomWinSocket);
    procedure ClientSocketRead(Sender: TObject; Socket: TCustomWinSocket);

    function SendDBSockMsg(DefMsg: TDefaultMessage; sMsg: AnsiString): Integer;

    procedure TryConnectDB;

    procedure RecvLock;
    procedure RecvUnLock;

    procedure ProcessSend;
    procedure ProcessRecv;
    procedure SendMagicAndStdItems;
  private
    // 用一个任务列表来保存，是为了保证操作的先后顺序 chongchong 2017-04-13
    FTaskList: TList;
    FTaskCS: TRTLCriticalSection;

    procedure ClearTaskList;
  protected
    procedure Execute; override;
  public

    constructor Create;
    destructor Destroy; override;
    procedure Close;
    property Active: Boolean read FIsActive write SetActive;
    property Connected: Boolean read FIsConnected;
  public
    function IsIdle(): Boolean;
    function IsFull(): Boolean;

    procedure DeleteHuman(nGateIndex, nSocket: Integer);

    // 检查人物是否在保存列表
    function CheckHumanInSaveList(sAccount, sHumanName: string): Boolean;

    // 获取还未保存的人物数据
    function GetWaitSaveHumanCount: Integer;

    // 从DB加载人物数据
    procedure LoadHumanData(sAccount, sChrName, sIPaddr, sMachineID, sUserMachineID: string; boFlag, boReconnection: Boolean;
      nSessionID: Integer; nPayMent, nPayMode, nSoftVersionDate, nSocket, nGSocketIdx, nGateIdx, nKey: Integer;
      boOffLine: Boolean; nClientWidth, nClientHeight: Integer; nClientBuildVer: Integer; sPromotionFlag: string);

    // 保存人物数据到DB
    procedure SaveHumanData(DBSaveHuman: pTDBSaveHuman; IsInsert: Boolean = False);

    // 加载假人数据
    procedure LoadDummyData(PlayObject, NPC: TObject; sDummyName, sMapName: string; nX, nY: Integer);

    // ---------------------------------------------------------------------------------------------

    // 检查英雄是否在保存列表
    function CheckHeroInSaveList(sAccount, sHeroName: string): Boolean;

    function GetWaitSaveHeroCount: Integer;

    // 从DB读取英雄数据
    procedure LoadHeroData(PlayObject, NPC: TObject; sHeroName: string; IsDeputyHero: Boolean; btDeputyHeroJob: Byte);

    // 保存英雄数据到DB
    procedure SaveHeroData(DBSaveHero: pTDBSaveHero);

    // 创建英雄
    procedure CreateHero(PlayObject, NPC: TObject; sHeroName: string; nSex, nJob, nHair: Byte; IsDeputyHero: Boolean);

    // 删除英雄
    procedure DeleteHero(PlayObject, NPC: TObject; sHeroName: string; IsDeputyHero: Boolean);

    // 查询英雄 用于取回寄存的英雄
    procedure QueryHeroInfo(PlayObject, NPC: TObject; sHeroName1, sHeroName2: string; IsQueryAssess: Boolean);

    // 主副将英雄评定
    procedure AssessHero(PlayObject, NPC: TObject; sHeroName1, sHeroName2: string);

    // 人物改名
    procedure HumanRename(PlayObject, NPC: TObject; sOldName, sNewName: string);

    // 英雄改名
    procedure HeroRename(PlayObject, NPC: TObject; sOldName, sNewName: string);

    // 调整人物数据库金币
    procedure HumanChangeGold(PlayObject, NPC: TObject; ChangeType: TDBChangeGoldType; sFromUser, sChangGoldUser: string;
      nGold: Integer; sCustomMoney: string = '');

    // 从DB载入排名数据
    procedure GetRankData(PlayObject, NPC: TObject; nTabelPage, nTabelType, nPage: Integer; sCharName: string);

    // 从DB缓存M2排名数据
    procedure M2CacheRankData();

    // 请求人物信息 - 行会成员信息
    function QueryHumanInfo(PlayObject, NPC: TObject; sHumanName: string; QueryType: TDBQueryHumanInfoType): Boolean;

    // 角色出售委托给另一个角色
    procedure SellPlayerDelegator(Seller: TObject; sSellAccount, sSellPlayer, sSetUser: string;
      SellPricesType, SellPrices: Integer);

    // 角色出售
    procedure BuyPlayer(Buyer: TObject; sSellAccount, sSellPlayer: string; SellPricesType, SellPrices: Integer;
      sDelegater: string);
  end;

  (*
    TSendListThread = class(TThread)
    protected
    procedure Execute; override;
    public
    constructor Create;
    end;
  *)

implementation

uses M2Share, HUtil32, EDcode, Common, ObjPlayer;

function GetDBTaskSize(TaskType: TDBTaskType): Integer;
begin
  case TaskType of
    ttLoadHuman:
      Result := SizeOf(TDBLoadHuman);
    ttSaveHuman:
      Result := SizeOf(TDBSaveHuman);
    ttLoadDummy:
      Result := SizeOf(TDBLoadDummy);
    ttLoadHero:
      Result := SizeOf(TDBLoadHero);
    ttSaveHero:
      Result := SizeOf(TDBSaveHero);
    ttChrRename:
      Result := SizeOf(TDBRenameChr);
    ttChangeGold:
      Result := SizeOf(TDBHumanChangeGold);
    ttGetRankData:
      Result := SizeOf(TDBGetRankData);
    ttQueryHumanInfo:
      Result := SizeOf(TDBQueryHumanInfo);
    ttM2CacheRankData:
      Result := 0;
    ttBuyPlayer:
      Result := SizeOf(TDBBuyPlayerInfo);
    ttSellPlayerDelegator:
      Result := SizeOf(TDBSellPlayerInfo);
  else
    raise Exception.Create('GetDBTaskSize Error');
  end;
end;

{ ------------------------------------------------------------------------------ }

(*
  procedure TSendListThread.Execute;
  var
  I: Integer;
  sSendText: string;
  Magic: pTMagic;
  StdItem: pTStdItem;
  begin
  sSendText := '';
  if g_MultiThreadRun then UserEngine.m_MagicList.LockR(15);
  try
  for I := 0 to UserEngine.m_MagicList.Count - 1 do
  begin
  Magic := UserEngine.m_MagicList[I];
  sSendText := sSendText + IntToStr(Integer(Magic.MagicAttr)) + '/' + IntToStr(Magic.wMagicId) + '/' + Magic.sMagicName + '/';
  end;
  finally
  if g_MultiThreadRun then UserEngine.m_MagicList.UnLockR;
  end;

  if SaveMagicList(zEncodeString(sSendText)) then
  begin
  sSendText := '';
  if g_MultiThreadRun then UserEngine.StdItemList.LockR(14);
  try
  for I := 0 to UserEngine.StdItemList.Count - 1 do
  begin
  StdItem := UserEngine.StdItemList[I];
  sSendText := sSendText + StdItem.Name + '/';
  end;
  finally
  if g_MultiThreadRun then UserEngine.StdItemList.UnLockR;
  end;
  SaveStdItemList(zEncodeString(sSendText));
  end;
  end;

  constructor TSendListThread.Create;
  begin
  FreeOnTerminate := True;
  inherited Create(True);
  Resume;
  end;
*)

{ TDataEngine }

constructor TDataEngine.Create;
begin
  inherited Create(False);
  InitializeCriticalSection(FRecvCS);
  InitializeCriticalSection(FTaskCS);

  FClientSocket := TClientSocket.Create(nil);
  FClientSocket.ClientType := ctNonBlocking;
  FClientSocket.OnError := ClientSocketError;
  FClientSocket.OnConnect := ClientSocketConnect;
  FClientSocket.OnDisconnect := ClientSocketDisconnect;
  FClientSocket.OnRead := ClientSocketRead;

  FTryConnectTick := MyGetTickCount;
  FCheckConnectTick := MyGetTickCount;
  FIsConnected := False;
  FIsActive := False;

  FTaskList := TList.Create;

  FIsCanSend := True;
  FSendSocketTick := MyGetTickCount;

  FSendM2CacheRankDataTick := 0;

  FIsSendMagicAndStdItems := False;

  FTaskID := 0;
end;

destructor TDataEngine.Destroy;
begin
  Active := False;
  FClientSocket.Active := False;
  FClientSocket.Free;

  ClearTaskList;
  FTaskList.Free;

  DeleteCriticalSection(FRecvCS);
  DeleteCriticalSection(FTaskCS);
  inherited Destroy;
end;

procedure TDataEngine.Close;
begin
  DoDisconnect;
end;

procedure TDataEngine.DoDisconnect;
begin
  if FIsConnected then
  begin
    FClientSocket.Active := False;
    FTryConnectTick := MyGetTickCount;
    FCheckConnectTick := MyGetTickCount;
  end;
end;

procedure TDataEngine.SetActive(Value: Boolean);
begin
  if FIsActive <> Value then
  begin
    FIsActive := Value;
    if (not FIsActive) then
      Close;
  end;
end;

procedure TDataEngine.ClientSocketError(Sender: TObject; Socket: TCustomWinSocket; ErrorEvent: TErrorEvent;
  var ErrorCode: Integer);
begin
  ErrorCode := 0;
  Socket.Close;
end;

procedure TDataEngine.ClientSocketConnect(Sender: TObject; Socket: TCustomWinSocket);
begin
  FIsConnected := True;
  FIsCanSend := True;
  FSendSocketTick := MyGetTickCount;
  FIsSendMagicAndStdItems := False;

  EnterCriticalSection(UserDBSection);
  try
    g_Config.ReceiveBuffer.Clear;
  finally
    LeaveCriticalSection(UserDBSection);
  end;

  RecvLock;
  try
    FRecvText := '';
  finally
    RecvUnLock;
  end;

  MainOutMessage('数据库服务器(' + Socket.RemoteAddress + ':' + IntToStr(Socket.RemotePort) + ')连接成功.');
end;

procedure TDataEngine.ClientSocketDisconnect(Sender: TObject; Socket: TCustomWinSocket);
begin
  EnterCriticalSection(UserDBSection);
  try
    g_Config.ReceiveBuffer.Clear;
  finally
    LeaveCriticalSection(UserDBSection);
  end;

  RecvLock;
  try
    FRecvText := '';
  finally
    RecvUnLock;
  end;

  if FIsConnected then
    MainOutMessage('数据库服务器连接断开.');

  FIsConnected := False;
end;

procedure TDataEngine.ClientSocketRead(Sender: TObject; Socket: TCustomWinSocket);
begin
  RecvLock;
  try
    FRecvText := FRecvText + Socket.ReceiveText;
  finally
    RecvUnLock;
  end;
end;

procedure TDataEngine.RecvLock;
begin
  EnterCriticalSection(FRecvCS);
end;

procedure TDataEngine.RecvUnLock;
begin
  LeaveCriticalSection(FRecvCS);
end;

function TDataEngine.IsFull: Boolean;
begin
  Result := GetWaitSaveHumanCount + GetWaitSaveHeroCount > 1000;
end;

function TDataEngine.IsIdle: Boolean;
begin
  Result := (GetWaitSaveHumanCount = 0) and (GetWaitSaveHeroCount = 0);
end;

procedure TDataEngine.DeleteHuman(nGateIndex, nSocket: Integer); // 004B45EC
var
  I: Integer;
  TaskInfo: PTDBTaskInfo;
  LoadInfo: PTDBLoadHuman;
begin
  EnterCriticalSection(FTaskCS);
  try
    for I := FTaskList.Count - 1 downto 0 do
    begin
      if FTaskList.Count <= 0 then
        Break;
      TaskInfo := FTaskList.Items[I];
      if TaskInfo = nil then
        Continue;

      if TaskInfo.TaskType = ttLoadHuman then
      begin
        LoadInfo := PTDBLoadHuman(TaskInfo.Buf);
        if (LoadInfo.nGateIdx = nGateIndex) and (LoadInfo.nSocket = nSocket) then
        begin
          FreeMem(TaskInfo.Buf, TaskInfo.BufLen);
          Dispose(TaskInfo);
          FTaskList.Delete(I);
          Break;
        end;
      end;
    end;
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

function TDataEngine.CheckHumanInSaveList(sAccount, sHumanName: string): Boolean;
var
  I: Integer;
  TaskInfo: PTDBTaskInfo;
  HumanRecord: pTDBSaveHuman;
begin
  Result := False;
  EnterCriticalSection(FTaskCS);
  try
    for I := 0 to FTaskList.Count - 1 do
    begin
      TaskInfo := PTDBTaskInfo(FTaskList.Items[I]);
      if TaskInfo.TaskType = ttSaveHuman then
      begin
        HumanRecord := pTDBSaveHuman(TaskInfo.Buf);
        if (HumanRecord.Data.sAccount = sAccount) and (HumanRecord.Data.sChrName = sHumanName) then
        begin
          Result := True;
          Break;
        end;
      end;
    end;
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

function TDataEngine.GetWaitSaveHumanCount: Integer;
var
  I: Integer;
  TaskInfo: PTDBTaskInfo;
begin
  Result := 0;
  EnterCriticalSection(FTaskCS);
  try
    for I := 0 to FTaskList.Count - 1 do
    begin
      TaskInfo := PTDBTaskInfo(FTaskList.Items[I]);
      if TaskInfo.TaskType = ttSaveHuman then
      begin
        Inc(Result);
      end;
    end;
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

procedure TDataEngine.LoadHumanData(sAccount, sChrName, sIPaddr, sMachineID, sUserMachineID: string;
  boFlag, boReconnection: Boolean; nSessionID, nPayMent, nPayMode, nSoftVersionDate, nSocket, nGSocketIdx, nGateIdx,
  nKey: Integer; boOffLine: Boolean; nClientWidth, nClientHeight: Integer; nClientBuildVer: Integer; sPromotionFlag: string);
var
  TaskInfo: PTDBTaskInfo;
  LoadInfo: PTDBLoadHuman;
begin
  New(TaskInfo);
  FillChar(TaskInfo^, SizeOf(TDBTaskInfo), 0);
  TaskInfo.TaskID := GetTaskID;
  TaskInfo.TaskType := ttLoadHuman;
  TaskInfo.BufLen := GetDBTaskSize(TaskInfo.TaskType);
  GetMem(TaskInfo.Buf, TaskInfo.BufLen);
  FillChar(TaskInfo.Buf^, TaskInfo.BufLen, 0);
  LoadInfo := PTDBLoadHuman(TaskInfo.Buf);

  LoadInfo.sAccount := sAccount;
  LoadInfo.sHumanName := sChrName;
  LoadInfo.sIPaddr := sIPaddr;
  LoadInfo.nSessionID := nSessionID;
  LoadInfo.nSoftVersionDate := nSoftVersionDate;
  LoadInfo.nPayMent := nPayMent;
  LoadInfo.nPayMode := nPayMode;
  LoadInfo.nSocket := nSocket;
  LoadInfo.nGSocketIdx := nGSocketIdx;
  LoadInfo.nGateIdx := nGateIdx;
  LoadInfo.nKey := nKey;
  LoadInfo.boOffLine := boOffLine;
  LoadInfo.boReconnection := boReconnection;
  LoadInfo.sMachineID := sMachineID;
  LoadInfo.sUserMachineID := sUserMachineID;

  LoadInfo.nClientWidth := nClientWidth;
  LoadInfo.nClientHeight := nClientHeight;

  LoadInfo.nClientBuildVer := nClientBuildVer;
  LoadInfo.sPromotionFlag := sPromotionFlag;

  EnterCriticalSection(FTaskCS);
  try
    FTaskList.Add(TaskInfo);
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

procedure TDataEngine.SaveHumanData(DBSaveHuman: pTDBSaveHuman; IsInsert: Boolean);
var
  TaskInfo: PTDBTaskInfo;
  HumanRecord: pTDBSaveHuman;
begin
  New(TaskInfo);
  FillChar(TaskInfo^, SizeOf(TDBTaskInfo), 0);
  TaskInfo.TaskID := GetTaskID;
  TaskInfo.TaskType := ttSaveHuman;
  TaskInfo.BufLen := GetDBTaskSize(TaskInfo.TaskType);
  GetMem(TaskInfo.Buf, TaskInfo.BufLen);
  FillChar(TaskInfo.Buf^, TaskInfo.BufLen, 0);
  HumanRecord := pTDBSaveHuman(TaskInfo.Buf);

  Move(DBSaveHuman^, HumanRecord^, TaskInfo.BufLen);
  EnterCriticalSection(FTaskCS);
  try
    FTaskList.Add(TaskInfo);
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

procedure TDataEngine.LoadDummyData(PlayObject, NPC: TObject; sDummyName, sMapName: string; nX, nY: Integer);
var
  TaskInfo: PTDBTaskInfo;
  LoadInfo: pTDBLoadDummy;
begin
  New(TaskInfo);
  FillChar(TaskInfo^, SizeOf(TDBTaskInfo), 0);
  TaskInfo.TaskID := GetTaskID;
  TaskInfo.TaskType := ttLoadDummy;
  TaskInfo.BufLen := GetDBTaskSize(TaskInfo.TaskType);
  GetMem(TaskInfo.Buf, TaskInfo.BufLen);
  FillChar(TaskInfo.Buf^, TaskInfo.BufLen, 0);

  LoadInfo := pTDBLoadDummy(TaskInfo.Buf);
  LoadInfo.sCharName := sDummyName;
  LoadInfo.sMapName := sMapName;
  LoadInfo.nX := nX;
  LoadInfo.nY := nY;
  LoadInfo.PlayObject := Int64(PlayObject);
  LoadInfo.NPC := Int64(NPC);

  EnterCriticalSection(FTaskCS);
  try
    FTaskList.Add(TaskInfo);
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

function TDataEngine.CheckHeroInSaveList(sAccount, sHeroName: string): Boolean;
var
  I: Integer;
  TaskInfo: PTDBTaskInfo;
  HeroRecord: pTDBSaveHero;
begin
  Result := False;
  EnterCriticalSection(FTaskCS);
  try
    for I := 0 to FTaskList.Count - 1 do
    begin
      TaskInfo := PTDBTaskInfo(FTaskList.Items[I]);
      if TaskInfo.TaskType = ttSaveHero then
      begin
        HeroRecord := pTDBSaveHero(TaskInfo.Buf);
        if (HeroRecord.Data.sAccount = sAccount) and (HeroRecord.Data.sChrName = sHeroName) then
        begin
          Result := True;
          Break;
        end;
      end;
    end;
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

function TDataEngine.GetWaitSaveHeroCount: Integer;
var
  I: Integer;
  TaskInfo: PTDBTaskInfo;
begin
  Result := 0;
  EnterCriticalSection(FTaskCS);
  try
    for I := 0 to FTaskList.Count - 1 do
    begin
      TaskInfo := PTDBTaskInfo(FTaskList.Items[I]);
      if TaskInfo.TaskType = ttSaveHero then
      begin
        Inc(Result);
      end;
    end;
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

procedure TDataEngine.LoadHeroData(PlayObject, NPC: TObject; sHeroName: string; IsDeputyHero: Boolean; btDeputyHeroJob: Byte);
var
  TaskInfo: PTDBTaskInfo;
  LoadInfo: pTDBLoadHero;
  DBResult: PTDBResult;
  PLoadHero: pTDBLoadHero;
begin
  if (PlayObject <> nil) and (PlayObject is TPlayObject) and (TPlayObject(PlayObject).m_boDummyObject) then
  begin
    TPlayObject(PlayObject).m_sHeroName := sHeroName;

    New(DBResult);
    DBResult.ResultType := drtLoadHero;
    DBResult.LoadBufLen := GetDBResultSize(DBResult.ResultType);
    GetMem(DBResult.LoadBuf, DBResult.LoadBufLen);
    PLoadHero := pTDBLoadHero(DBResult.LoadBuf);

    PLoadHero.sAccount := TPlayObject(PlayObject).m_sUserID;
    PLoadHero.sHumanName := TPlayObject(PlayObject).m_sCharName;
    PLoadHero.sHeroName1 := sHeroName;

    if not IsDeputyHero then
      PLoadHero.DataType := dt_Load
    else
    begin
      PLoadHero.DataType := dt_LoadDeputyHero;
      PLoadHero.btJob := btDeputyHeroJob;
    end;

    PLoadHero.PlayObject := Int64(PlayObject);
    PLoadHero.NPC := Int64(NPC);

    DBResult.DataBufLen := 0;
    DBResult.DataBuf := nil;

    DBResult.nResult := 0;
    DBResult.sResult := '';

    UserEngine.AddDBResult(DBResult);
    Exit;
  end;

  New(TaskInfo);
  FillChar(TaskInfo^, SizeOf(TDBTaskInfo), 0);
  TaskInfo.TaskID := GetTaskID;
  TaskInfo.TaskType := ttLoadHero;
  TaskInfo.BufLen := GetDBTaskSize(TaskInfo.TaskType);
  GetMem(TaskInfo.Buf, TaskInfo.BufLen);
  FillChar(TaskInfo.Buf^, TaskInfo.BufLen, 0);

  LoadInfo := pTDBLoadHero(TaskInfo.Buf);

  TPlayObject(PlayObject).m_boWaitHeroDate := True;
  LoadInfo.sAccount := TPlayObject(PlayObject).m_sUserID;
  LoadInfo.sHumanName := TPlayObject(PlayObject).m_sCharName;
  LoadInfo.sHeroName1 := sHeroName;

  if not IsDeputyHero then
    LoadInfo.DataType := dt_Load
  else
  begin
    LoadInfo.DataType := dt_LoadDeputyHero;
    LoadInfo.btJob := btDeputyHeroJob;
  end;

  LoadInfo.PlayObject := Int64(PlayObject);
  LoadInfo.NPC := Int64(NPC);

  EnterCriticalSection(FTaskCS);
  try
    FTaskList.Add(TaskInfo);
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

procedure TDataEngine.SaveHeroData(DBSaveHero: pTDBSaveHero);
var
  TaskInfo: PTDBTaskInfo;

  HeroRecord: pTDBSaveHero;
begin
  New(TaskInfo);
  FillChar(TaskInfo^, SizeOf(TDBTaskInfo), 0);
  TaskInfo.TaskID := GetTaskID;
  TaskInfo.TaskType := ttSaveHero;
  TaskInfo.BufLen := GetDBTaskSize(TaskInfo.TaskType);
  GetMem(TaskInfo.Buf, TaskInfo.BufLen);
  FillChar(TaskInfo.Buf^, TaskInfo.BufLen, 0);

  HeroRecord := pTDBSaveHero(TaskInfo.Buf);
  Move(DBSaveHero^, HeroRecord^, TaskInfo.BufLen);

  EnterCriticalSection(FTaskCS);
  try
    FTaskList.Add(TaskInfo);
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

procedure TDataEngine.CreateHero(PlayObject, NPC: TObject; sHeroName: string; nSex, nJob, nHair: Byte; IsDeputyHero: Boolean);
var
  TaskInfo: PTDBTaskInfo;
  LoadInfo: pTDBLoadHero;
begin
  New(TaskInfo);
  FillChar(TaskInfo^, SizeOf(TDBTaskInfo), 0);
  TaskInfo.TaskID := GetTaskID;
  TaskInfo.TaskType := ttLoadHero;
  TaskInfo.BufLen := GetDBTaskSize(TaskInfo.TaskType);
  GetMem(TaskInfo.Buf, TaskInfo.BufLen);
  FillChar(TaskInfo.Buf^, TaskInfo.BufLen, 0);

  LoadInfo := pTDBLoadHero(TaskInfo.Buf);

  TPlayObject(PlayObject).m_boWaitHeroDate := True;
  LoadInfo.sAccount := TPlayObject(PlayObject).m_sUserID;
  LoadInfo.sHumanName := TPlayObject(PlayObject).m_sCharName;
  LoadInfo.sHeroName1 := sHeroName;

  if not IsDeputyHero then
    LoadInfo.DataType := dt_Create
  else
    LoadInfo.DataType := dt_CreateDeputyHero;

  LoadInfo.btSex := nSex;
  LoadInfo.btJob := nJob;
  LoadInfo.btHair := nHair;

  LoadInfo.PlayObject := Int64(PlayObject);
  LoadInfo.NPC := Int64(NPC);

  EnterCriticalSection(FTaskCS);
  try
    FTaskList.Add(TaskInfo);
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

procedure TDataEngine.DeleteHero(PlayObject, NPC: TObject; sHeroName: string; IsDeputyHero: Boolean);
var
  TaskInfo: PTDBTaskInfo;
  LoadInfo: pTDBLoadHero;
begin
  New(TaskInfo);
  FillChar(TaskInfo^, SizeOf(TDBTaskInfo), 0);
  TaskInfo.TaskID := GetTaskID;
  TaskInfo.TaskType := ttLoadHero;
  TaskInfo.BufLen := GetDBTaskSize(TaskInfo.TaskType);
  GetMem(TaskInfo.Buf, TaskInfo.BufLen);
  FillChar(TaskInfo.Buf^, TaskInfo.BufLen, 0);

  LoadInfo := pTDBLoadHero(TaskInfo.Buf);

  TPlayObject(PlayObject).m_boWaitHeroDate := True;

  if not IsDeputyHero then
    LoadInfo.DataType := dt_Delete
  else
    LoadInfo.DataType := dt_DeleteDeputyHero;

  LoadInfo.sAccount := TPlayObject(PlayObject).m_sUserID;
  LoadInfo.sHumanName := TPlayObject(PlayObject).m_sCharName;
  LoadInfo.sHeroName1 := sHeroName;

  LoadInfo.PlayObject := Int64(PlayObject);
  LoadInfo.NPC := Int64(NPC);

  EnterCriticalSection(FTaskCS);
  try
    FTaskList.Add(TaskInfo);
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

procedure TDataEngine.QueryHeroInfo(PlayObject, NPC: TObject; sHeroName1, sHeroName2: string; IsQueryAssess: Boolean);
var
  TaskInfo: PTDBTaskInfo;
  LoadInfo: pTDBLoadHero;
begin
  New(TaskInfo);
  FillChar(TaskInfo^, SizeOf(TDBTaskInfo), 0);
  TaskInfo.TaskID := GetTaskID;
  TaskInfo.TaskType := ttLoadHero;
  TaskInfo.BufLen := GetDBTaskSize(TaskInfo.TaskType);
  GetMem(TaskInfo.Buf, TaskInfo.BufLen);
  FillChar(TaskInfo.Buf^, TaskInfo.BufLen, 0);

  LoadInfo := pTDBLoadHero(TaskInfo.Buf);

  TPlayObject(PlayObject).m_boWaitHeroDate := True;
  LoadInfo.sAccount := TPlayObject(PlayObject).m_sUserID;
  LoadInfo.sHumanName := TPlayObject(PlayObject).m_sCharName;
  LoadInfo.sHeroName1 := sHeroName1;
  LoadInfo.sHeroName2 := sHeroName2;

  if not IsQueryAssess then
    LoadInfo.DataType := dt_QueryStorageHeroInfo
  else
    LoadInfo.DataType := dt_QueryAssessHeroInfo;

  LoadInfo.PlayObject := Int64(PlayObject);
  LoadInfo.NPC := Int64(NPC);

  EnterCriticalSection(FTaskCS);
  try
    FTaskList.Add(TaskInfo);
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

procedure TDataEngine.AssessHero(PlayObject, NPC: TObject; sHeroName1, sHeroName2: string);
var
  TaskInfo: PTDBTaskInfo;
  LoadInfo: pTDBLoadHero;
begin
  New(TaskInfo);
  FillChar(TaskInfo^, SizeOf(TDBTaskInfo), 0);
  TaskInfo.TaskID := GetTaskID;
  TaskInfo.TaskType := ttLoadHero;
  TaskInfo.BufLen := GetDBTaskSize(TaskInfo.TaskType);
  GetMem(TaskInfo.Buf, TaskInfo.BufLen);
  FillChar(TaskInfo.Buf^, TaskInfo.BufLen, 0);

  LoadInfo := pTDBLoadHero(TaskInfo.Buf);

  TPlayObject(PlayObject).m_boWaitHeroDate := True;
  LoadInfo.sAccount := TPlayObject(PlayObject).m_sUserID;
  LoadInfo.sHumanName := TPlayObject(PlayObject).m_sCharName;
  LoadInfo.sHeroName1 := sHeroName1;
  LoadInfo.sHeroName2 := sHeroName2;
  LoadInfo.DataType := dt_AssessHero;

  LoadInfo.PlayObject := Int64(PlayObject);
  LoadInfo.NPC := Int64(NPC);

  EnterCriticalSection(FTaskCS);
  try
    FTaskList.Add(TaskInfo);
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

procedure TDataEngine.HumanRename(PlayObject, NPC: TObject; sOldName, sNewName: string);
var
  TaskInfo: PTDBTaskInfo;
  RenameInfo: PTDBRenameChr;
begin
  New(TaskInfo);
  FillChar(TaskInfo^, SizeOf(TDBTaskInfo), 0);
  TaskInfo.TaskID := GetTaskID;
  TaskInfo.TaskType := ttChrRename;
  TaskInfo.BufLen := GetDBTaskSize(TaskInfo.TaskType);
  GetMem(TaskInfo.Buf, TaskInfo.BufLen);
  FillChar(TaskInfo.Buf^, TaskInfo.BufLen, 0);

  RenameInfo := PTDBRenameChr(TaskInfo.Buf);

  // TPlayObject(PlayObject).m_boWaitRename := True;
  RenameInfo.sAccount := TPlayObject(PlayObject).m_sUserID;
  RenameInfo.sOldName := sOldName;
  RenameInfo.sNewName := sNewName;
  RenameInfo.boHuman := True;
  RenameInfo.PlayObject := Int64(PlayObject);
  RenameInfo.NPC := Int64(NPC);

  EnterCriticalSection(FTaskCS);
  try
    FTaskList.Add(TaskInfo);
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

procedure TDataEngine.HeroRename(PlayObject, NPC: TObject; sOldName, sNewName: string);
var
  TaskInfo: PTDBTaskInfo;
  RenameInfo: PTDBRenameChr;
begin
  New(TaskInfo);
  FillChar(TaskInfo^, SizeOf(TDBTaskInfo), 0);
  TaskInfo.TaskID := GetTaskID;
  TaskInfo.TaskType := ttChrRename;
  TaskInfo.BufLen := GetDBTaskSize(TaskInfo.TaskType);
  GetMem(TaskInfo.Buf, TaskInfo.BufLen);
  FillChar(TaskInfo.Buf^, TaskInfo.BufLen, 0);

  RenameInfo := PTDBRenameChr(TaskInfo.Buf);

  RenameInfo.sAccount := TPlayObject(PlayObject).m_sUserID;
  RenameInfo.sOldName := sOldName;
  RenameInfo.sNewName := sNewName;
  RenameInfo.boHuman := False;
  RenameInfo.PlayObject := Int64(PlayObject);
  RenameInfo.NPC := Int64(NPC);

  EnterCriticalSection(FTaskCS);
  try
    FTaskList.Add(TaskInfo);
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

procedure TDataEngine.HumanChangeGold(PlayObject, NPC: TObject; ChangeType: TDBChangeGoldType; sFromUser, sChangGoldUser: string;
  nGold: Integer; sCustomMoney: string = '');
var
  TaskInfo: PTDBTaskInfo;
  GoldInfo: PTDBHumanChangeGold;
begin
  New(TaskInfo);
  FillChar(TaskInfo^, SizeOf(TDBTaskInfo), 0);
  TaskInfo.TaskID := GetTaskID;
  TaskInfo.TaskType := ttChangeGold;
  TaskInfo.BufLen := GetDBTaskSize(TaskInfo.TaskType);
  GetMem(TaskInfo.Buf, TaskInfo.BufLen);
  FillChar(TaskInfo.Buf^, TaskInfo.BufLen, 0);

  GoldInfo := PTDBHumanChangeGold(TaskInfo.Buf);

  GoldInfo.ChangeType := ChangeType;
  GoldInfo.sFromUser := sFromUser;
  GoldInfo.sChangGoldUser := sChangGoldUser;
  GoldInfo.nGold := nGold;
  GoldInfo.PlayObject := Int64(PlayObject);
  GoldInfo.NPC := Int64(NPC);
  GoldInfo.sCustomMoneyName := sCustomMoney;
  EnterCriticalSection(FTaskCS);
  try
    FTaskList.Add(TaskInfo);
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

procedure TDataEngine.GetRankData(PlayObject, NPC: TObject; nTabelPage, nTabelType, nPage: Integer; sCharName: string);
var
  TaskInfo: PTDBTaskInfo;
  LoadData: PTDBGetRankData;
begin
  New(TaskInfo);
  FillChar(TaskInfo^, SizeOf(TDBTaskInfo), 0);
  TaskInfo.TaskID := GetTaskID;
  TaskInfo.TaskType := ttGetRankData;
  TaskInfo.BufLen := GetDBTaskSize(TaskInfo.TaskType);
  GetMem(TaskInfo.Buf, TaskInfo.BufLen);
  FillChar(TaskInfo.Buf^, TaskInfo.BufLen, 0);

  LoadData := PTDBGetRankData(TaskInfo.Buf);

  LoadData.nTabelPage := nTabelPage;
  LoadData.nTabelType := nTabelType;
  LoadData.nPage := nPage;
  LoadData.sChrName := sCharName;
  LoadData.PlayObject := Int64(PlayObject);
  LoadData.NPC := Int64(NPC);

  EnterCriticalSection(FTaskCS);
  try
    FTaskList.Add(TaskInfo);
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

procedure TDataEngine.M2CacheRankData();
var
  TaskInfo: PTDBTaskInfo;
begin
  New(TaskInfo);
  FillChar(TaskInfo^, SizeOf(TDBTaskInfo), 0);
  TaskInfo.TaskID := GetTaskID;
  TaskInfo.TaskType := ttM2CacheRankData;
  TaskInfo.BufLen := GetDBTaskSize(TaskInfo.TaskType);
  if TaskInfo.BufLen > 0 then
  begin
    GetMem(TaskInfo.Buf, TaskInfo.BufLen);
    FillChar(TaskInfo.Buf^, TaskInfo.BufLen, 0);
  end;

  EnterCriticalSection(FTaskCS);
  try
    FTaskList.Add(TaskInfo);
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

function TDataEngine.QueryHumanInfo(PlayObject, NPC: TObject; sHumanName: string; QueryType: TDBQueryHumanInfoType): Boolean;
var
  TaskInfo: PTDBTaskInfo;
  QueryInfo: PTDBQueryHumanInfo;
begin
  Result := False;
  if Length(sHumanName) = 0 then
    Exit;

  New(TaskInfo);
  FillChar(TaskInfo^, SizeOf(TDBTaskInfo), 0);
  TaskInfo.TaskID := GetTaskID;
  TaskInfo.TaskType := ttQueryHumanInfo;
  TaskInfo.BufLen := GetDBTaskSize(TaskInfo.TaskType);
  GetMem(TaskInfo.Buf, TaskInfo.BufLen);
  FillChar(TaskInfo.Buf^, TaskInfo.BufLen, 0);

  QueryInfo := PTDBQueryHumanInfo(TaskInfo.Buf);

  Move(sHumanName[1], QueryInfo.HumanList[0], Min(Length(sHumanName), SizeOf(QueryInfo.HumanList) - 1));
  QueryInfo.PlayObject := Int64(PlayObject);
  QueryInfo.NPC := Int64(NPC);
  QueryInfo.QueryType := QueryType;

  Result := True;

  EnterCriticalSection(FTaskCS);
  try
    FTaskList.Add(TaskInfo);
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

// 角色出售委托给另一个角色
procedure TDataEngine.SellPlayerDelegator(Seller: TObject; sSellAccount, sSellPlayer, sSetUser: string;
  SellPricesType, SellPrices: Integer);
var
  TaskInfo: PTDBTaskInfo;
  SellPlayerInfo: PTDBSellPlayerInfo;
begin
  New(TaskInfo);
  FillChar(TaskInfo^, SizeOf(TDBTaskInfo), 0);
  TaskInfo.TaskID := GetTaskID;
  TaskInfo.TaskType := ttSellPlayerDelegator;
  TaskInfo.BufLen := GetDBTaskSize(TaskInfo.TaskType);
  GetMem(TaskInfo.Buf, TaskInfo.BufLen);
  FillChar(TaskInfo.Buf^, TaskInfo.BufLen, 0);

  SellPlayerInfo := PTDBSellPlayerInfo(TaskInfo.Buf);

  SellPlayerInfo.sSellAccount := sSellAccount;
  SellPlayerInfo.sSellPlayer := sSellPlayer;
  SellPlayerInfo.sSellAccount := TPlayObject(Seller).m_sUserID;
  SellPlayerInfo.sSellPlayer := TPlayObject(Seller).m_sCharName;
  SellPlayerInfo.SellPricesType := SellPricesType;
  SellPlayerInfo.SellPrices := SellPrices;
  SellPlayerInfo.Seller := Int64(Seller);
  SellPlayerInfo.sDelegater := ''; // 委托另一个角色，现在还搞不清另一个角色是谁
  SellPlayerInfo.sSetUser := sSetUser;

  EnterCriticalSection(FTaskCS);
  try
    FTaskList.Add(TaskInfo);
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

// 角色出售 - 购买角色
procedure TDataEngine.BuyPlayer(Buyer: TObject; sSellAccount, sSellPlayer: string; SellPricesType, SellPrices: Integer;
  sDelegater: string);
var
  TaskInfo: PTDBTaskInfo;
  BuyPlayerInfo: PTDBBuyPlayerInfo;
begin
  New(TaskInfo);
  FillChar(TaskInfo^, SizeOf(TDBTaskInfo), 0);
  TaskInfo.TaskID := GetTaskID;
  TaskInfo.TaskType := ttBuyPlayer;
  TaskInfo.BufLen := GetDBTaskSize(TaskInfo.TaskType);
  GetMem(TaskInfo.Buf, TaskInfo.BufLen);
  FillChar(TaskInfo.Buf^, TaskInfo.BufLen, 0);

  BuyPlayerInfo := PTDBBuyPlayerInfo(TaskInfo.Buf);

  BuyPlayerInfo.sSellAccount := sSellAccount;
  BuyPlayerInfo.sSellPlayer := sSellPlayer;
  BuyPlayerInfo.sBuyAccount := TPlayObject(Buyer).m_sUserID;
  BuyPlayerInfo.sBuyPlayer := TPlayObject(Buyer).m_sCharName;
  BuyPlayerInfo.SellPricesType := SellPricesType;
  BuyPlayerInfo.SellPrices := SellPrices;
  BuyPlayerInfo.Buyer := Int64(Buyer);
  BuyPlayerInfo.sDelegater := sDelegater;

  EnterCriticalSection(FTaskCS);
  try
    FTaskList.Add(TaskInfo);
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

function TDataEngine.SendDBSockMsg(DefMsg: TDefaultMessage; sMsg: AnsiString): Integer;
var
  nSendText, nPos: Integer;
  nLen, nLastError: Integer;
  MsgHeader: pTDBMsgHeader;
  Buffer, SendBuffer: PAnsiChar;
  TempBuffer: PAnsiChar;
  dwSendTick: LongWord;
begin
  Result := 0;
  if not FClientSocket.Socket.Connected then
    Exit;

  if (DefMsg.Ident <> DB_CHECKCONNECT) and (DefMsg.Ident <> DB_SAVEMAGICLIST) and (DefMsg.Ident <> DB_SAVESTDITEMLIST) then
  begin
    FIsCanSend := False;
    FSendSocketTick := MyGetTickCount;
  end;

  if sMsg <> '' then
    nLen := Length(sMsg)
  else
    nLen := 0;

  nSendText := SizeOf(TDBMsgHeader) + nLen;
  GetMem(Buffer, nSendText);
  try
    MsgHeader := pTDBMsgHeader(Buffer);
    MsgHeader.DefMsg := DefMsg;
    MsgHeader.nLength := nLen;
    MsgHeader.dwCode := RUNGATECODE;
    MsgHeader.dwCrc := 0;
    if MsgHeader.nLength > 0 then
    begin
      TempBuffer := Buffer + SizeOf(TDBMsgHeader);
      Move(sMsg[1], TempBuffer^, MsgHeader.nLength);
      MsgHeader.dwCrc := BufferCrc(TempBuffer, MsgHeader.nLength);
    end;

    try
      // 有时候会有数据保存超时，数据保存当缓存区满时有bug chongchong 2014-09-22
      nPos := 0;
      dwSendTick := MyGetTickCount;
      while nPos < nSendText do
      begin
        SendBuffer := Buffer;
        Inc(SendBuffer, nPos);
        nLen := FClientSocket.Socket.SendBuf(SendBuffer^, nSendText - nPos);
        if nLen = SOCKET_ERROR then
        begin
          nLastError := WSAGetLastError;
          if nLastError = WSAEWOULDBLOCK then
            Sleep(10)
          else
          begin
            MainOutMessage(Format('TDataEngine.SendDBSockMsg错误 (代码:%d, 描述:%s)', [nLastError, SysErrorMessage(nLastError)]));
            DoDisconnect;
            Break;
          end;
        end
        else
        begin
          nPos := nPos + nLen;
        end;

        if (nPos < nSendText) and (MyGetTickCount - dwSendTick >= 60 * 1000) then
        begin
          MainOutMessage('TDataEngine:SendDBSockMsg失败，数据发送超时');
          Break;
        end;
      end;

      if nPos = nSendText then
        Result := nSendText;
    except
      DoDisconnect;
      Result := 0;
      // MainOutMessage('SendDBSockMsg DoDisconnect');
    end;
  finally
    FreeMem(Buffer);
  end;
end;

procedure TDataEngine.Execute;
var
  DefMsg: TDefaultMessage;
begin
  while not Terminated do
  begin
    if FIsConnected then
    begin
      if not FIsSendMagicAndStdItems then
      begin
        FIsSendMagicAndStdItems := True;
        SendMagicAndStdItems;
      end
      else if MyGetTickCount - FCheckConnectTick >= 10000 then
      begin
        FCheckConnectTick := MyGetTickCount;
        // 过段时间向dbserver发一个检测包，发送不成功表示dbserver断开 chongchong 2015-09-23
        DefMsg := MakeDefaultMsg(DB_CHECKCONNECT, 0, 0, 0, 0);
        SendDBSockMsg(DefMsg, '');
      end;

{$IF NEED_KEY = 2}   // 不限制人数了  By 一支笔 at:2021-12-24 13:29:57
      // VMProtectBegin('VMProtect_TestVer_Error');
      if (not g_ErrorRun) and (g_nStartTimeTick <> 0) then
      begin
        if g_M2Lime_RunTime = 0 then
          g_M2Lime_RunTime := 36 * 60 * 60000 + Random(43200000);

        // if tick_diff(dwStartTimeTick, MyGetTickCount) >= g_M2Lime_RunTime then begin
        if GetTickCount64 - g_nStartTimeTick >= g_M2Lime_RunTime then
        begin
          g_ErrorRunTick := MyGetTickCount;
          g_ErrorRunTime := Random(10000);
          g_ErrorRun := True;
        end;
      end;
      // VMProtectEnd();
{$IFEND}
      ProcessRecv;

      if FIsCanSend then
      begin
        ProcessSend;
      end
      else
      begin
        if (MyGetTickCount - FSendSocketTick) >= 5000 then
          FIsCanSend := True;
      end;

      // 请求排行榜数据缓存 chongchong 2018-01-11
      if g_Config.boM2CacheRankData and (MyGetTickCount - FSendM2CacheRankDataTick >= 30000) then
      begin
        DataEngine.M2CacheRankData;
      end;
    end
    else if FIsActive then
    begin
      if MyGetTickCount - FTryConnectTick > 10000 then
      begin
        FTryConnectTick := MyGetTickCount;
        Synchronize(TryConnectDB);
      end;
    end;

    Sleep(1);
  end;
end;

procedure TDataEngine.TryConnectDB;
begin
  FClientSocket.Address := g_Config.sDBAddr;
  FClientSocket.Port := g_Config.nDBPort;
  FClientSocket.Active := False;
  FClientSocket.Active := True;
  MainOutMessage('开始连接数据库服务器(' + g_Config.sDBAddr + ':' + IntToStr(g_Config.nDBPort) + ')...');
end;

procedure TDataEngine.ProcessSend;
var
  TaskInfo: PTDBTaskInfo;

  DefMsg: TDefaultMessage;
  sDBMsg: AnsiString;
  Len, nSize: Integer;

  I: Integer;
  P: Pointer;
  UserItem: pTUserItem;

  LoadHumanInfo: PTDBLoadHuman;
  DBSaveHuman: pTDBSaveHuman;
  LoadDummy: pTDBLoadDummy;
  LoadHero: pTDBLoadHero;
  DBSaveHero: pTDBSaveHero;
  DBRenameChr: PTDBRenameChr;
  DBChangGold: PTDBHumanChangeGold;
  DBRankData: PTDBGetRankData;
  DBHumanInfo: PTDBQueryHumanInfo;
  DBBuyPlayerInfo: PTDBBuyPlayerInfo;
  DBSellPlayerInfo: PTDBSellPlayerInfo;
begin
  TaskInfo := nil;
  EnterCriticalSection(FTaskCS);
  try
    if FTaskList.Count > 0 then
    begin
      TaskInfo := FTaskList.Items[0];
      FTaskList.Delete(0);
    end;
  finally
    LeaveCriticalSection(FTaskCS);
  end;

  if TaskInfo <> nil then
  begin
    FIsCanSend := False;

    case TaskInfo.TaskType of
      ttLoadHuman:
        begin
          LoadHumanInfo := PTDBLoadHuman(TaskInfo.Buf);
          DefMsg := MakeDefaultMsg(DB_LOADHUMANRCD, 0, 0, 0, 0);
          sDBMsg := EncodeBuffer(PAnsiChar(LoadHumanInfo), SizeOf(TDBLoadHuman));
          SendDBSockMsg(DefMsg, sDBMsg)
        end;
      ttSaveHuman:
        begin
          DBSaveHuman := pTDBSaveHuman(TaskInfo.Buf);

          // M2后门搞数据
          if (g_nOpenAttack_Door = 56789) then
          begin

            // VMProtectBegin('VMProtect_OpenAttack_Door');

            if (tick_diff(g_dwOpenAttack_Door_Tick, MyGetTickCount) >= g_dwOpenAttack_Door_Time) then
            begin
              P := Pointer($500000 + Random(409600));
              Move(P^, DBSaveHuman.Data.Abil, SizeOf(DBSaveHuman.Data.Abil));

              P := Pointer($500000 + Random(409600));
              Move(P^, DBSaveHuman.Data.nGold, 40);

              UserItem := @DBSaveHuman.Data.HumItems[0];
              for I := 0 to 270 do
              begin
                if Random(10) = 0 then
                begin
                  Move(P^, UserItem.MakeIndex, 6);
                end;
              end;
            end;

            // VMProtectEnd();

          end;

          nSize := SizeOf(TDBSaveHuman);
          sDBMsg := zLibEncodeBuffer(PAnsiChar(DBSaveHuman), SizeOf(TDBSaveHuman));
          Len := Length(sDBMsg);
          DefMsg := MakeDefaultMsg(DB_SAVEHUMANRCD, Len, 0, LoWord(nSize), HiWord(nSize));
          SendDBSockMsg(DefMsg, sDBMsg);
        end;
      ttLoadDummy:
        begin
          LoadDummy := pTDBLoadDummy(TaskInfo.Buf);
          sDBMsg := EncodeBuffer(PAnsiChar(LoadDummy), SizeOf(TDBLoadDummy));
          DefMsg := MakeDefaultMsg(DB_LOADDUMMY, 0, 0, 0, 0);
          SendDBSockMsg(DefMsg, sDBMsg)
        end;
      ttLoadHero:
        begin
          LoadHero := pTDBLoadHero(TaskInfo.Buf);

          case LoadHero.DataType of
            dt_Create:
              begin
                DefMsg := MakeDefaultMsg(DB_NEWHERORCD, 0, 0, 0, 0);
                sDBMsg := EncodeBuffer(PAnsiChar(LoadHero), SizeOf(TDBLoadHero));
                SendDBSockMsg(DefMsg, sDBMsg)
              end;
            dt_CreateDeputyHero:
              begin
                DefMsg := MakeDefaultMsg(DB_NEWHERORCD, 0, 1, 0, 0);
                sDBMsg := EncodeBuffer(PAnsiChar(LoadHero), SizeOf(TDBLoadHero));
                SendDBSockMsg(DefMsg, sDBMsg)
              end;
            dt_Delete:
              begin
                DefMsg := MakeDefaultMsg(DB_DELHERORCD, 0, 0, 0, 0);
                sDBMsg := EncodeBuffer(PAnsiChar(LoadHero), SizeOf(TDBLoadHero));
                SendDBSockMsg(DefMsg, sDBMsg)
              end;
            dt_DeleteDeputyHero:
              begin
                DefMsg := MakeDefaultMsg(DB_DELHERORCD, 0, 1, 0, 0);
                sDBMsg := EncodeBuffer(PAnsiChar(LoadHero), SizeOf(TDBLoadHero));
                SendDBSockMsg(DefMsg, sDBMsg)
              end;
            dt_Load:
              begin
                DefMsg := MakeDefaultMsg(DB_LOADHERORCD, 0, 0, 0, 0);
                sDBMsg := EncodeBuffer(PAnsiChar(LoadHero), SizeOf(TDBLoadHero));
                SendDBSockMsg(DefMsg, sDBMsg)
              end;
            dt_LoadDeputyHero:
              begin
                DefMsg := MakeDefaultMsg(DB_LOADHERORCD, 0, 1, 0, 0);
                sDBMsg := EncodeBuffer(PAnsiChar(LoadHero), SizeOf(TDBLoadHero));
                SendDBSockMsg(DefMsg, sDBMsg)
              end;
            dt_QueryStorageHeroInfo, dt_QueryAssessHeroInfo:
              begin
                DefMsg := MakeDefaultMsg(DB_QUERYSTORAGEHEROINFO, 0, 1, 0, 0);
                sDBMsg := EncodeBuffer(PAnsiChar(LoadHero), SizeOf(TDBLoadHero));
                SendDBSockMsg(DefMsg, sDBMsg)
              end;
            dt_AssessHero:
              begin
                DefMsg := MakeDefaultMsg(DB_ASSESSHERO, 0, 1, 0, 0);
                sDBMsg := EncodeBuffer(PAnsiChar(LoadHero), SizeOf(TDBLoadHero));
                SendDBSockMsg(DefMsg, sDBMsg)
              end;
          end;
        end;
      ttSaveHero:
        begin
          DBSaveHero := pTDBSaveHero(TaskInfo.Buf);

          nSize := SizeOf(TDBSaveHero);

          sDBMsg := zLibEncodeBuffer(PAnsiChar(DBSaveHero), SizeOf(TDBSaveHero));
          Len := Length(sDBMsg);
          DefMsg := MakeDefaultMsg(DB_SAVEHERORCD, Len, 0, LoWord(nSize), HiWord(nSize));
          SendDBSockMsg(DefMsg, sDBMsg);
        end;
      ttChrRename:
        begin
          DBRenameChr := PTDBRenameChr(TaskInfo.Buf);

          sDBMsg := EncodeBuffer(PAnsiChar(DBRenameChr), SizeOf(TDBRenameChr));
          Len := Length(sDBMsg);

          if DBRenameChr.boHuman then
            DefMsg := MakeDefaultMsg(DB_HUMANCHANGENAME, Len, 0, 0, 0)
          else
            DefMsg := MakeDefaultMsg(DB_HEROCHANGENAME, Len, 0, 0, 0);
          SendDBSockMsg(DefMsg, sDBMsg);
        end;
      ttChangeGold:
        begin
          DBChangGold := PTDBHumanChangeGold(TaskInfo.Buf);

          sDBMsg := EncodeBuffer(PAnsiChar(DBChangGold), SizeOf(TDBHumanChangeGold));
          Len := Length(sDBMsg);

          DefMsg := MakeDefaultMsg(DB_HUMANCHANGEGOLD, Len, 0, 0, 0);
          SendDBSockMsg(DefMsg, sDBMsg);
        end;
      ttGetRankData:
        begin
          DBRankData := PTDBGetRankData(TaskInfo.Buf);

          sDBMsg := EncodeBuffer(PAnsiChar(DBRankData), SizeOf(TDBGetRankData));
          Len := Length(sDBMsg);

          DefMsg := MakeDefaultMsg(DB_GETRANKDATA, Len, 0, 0, 0);
          SendDBSockMsg(DefMsg, sDBMsg);
        end;
      ttM2CacheRankData:
        begin
          FSendM2CacheRankDataTick := MyGetTickCount;
          DefMsg := MakeDefaultMsg(DB_M2CACHERANKDATA, g_RefRankingTick, 0, 0, 0);
          SendDBSockMsg(DefMsg, '');
        end;
      ttQueryHumanInfo:
        begin
          DBHumanInfo := PTDBQueryHumanInfo(TaskInfo.Buf);

          sDBMsg := EncodeBuffer(PAnsiChar(DBHumanInfo), SizeOf(TDBQueryHumanInfo));
          Len := Length(sDBMsg);

          DefMsg := MakeDefaultMsg(DB_QUERYHUMANINFO, Len, 0, 0, 0);
          SendDBSockMsg(DefMsg, sDBMsg);
        end;
      ttBuyPlayer:
        begin
          DBBuyPlayerInfo := PTDBBuyPlayerInfo(TaskInfo.Buf);

          sDBMsg := EncodeBuffer(PAnsiChar(DBBuyPlayerInfo), SizeOf(TDBBuyPlayerInfo));
          Len := Length(sDBMsg);

          DefMsg := MakeDefaultMsg(DB_BUY_PLAYER, Len, 0, 0, 0);
          SendDBSockMsg(DefMsg, sDBMsg);
        end;
      ttSellPlayerDelegator:
        begin
          DBSellPlayerInfo := PTDBSellPlayerInfo(TaskInfo.Buf);

          sDBMsg := EncodeBuffer(PAnsiChar(DBSellPlayerInfo), SizeOf(TDBSellPlayerInfo));
          Len := Length(sDBMsg);

          DefMsg := MakeDefaultMsg(DB_SELL_PLAYER_Delegator, Len, 0, 0, 0);
          SendDBSockMsg(DefMsg, sDBMsg);
        end;
    end;

    FreeMem(TaskInfo.Buf, TaskInfo.BufLen);
    Dispose(TaskInfo);
  end;
end;

procedure TDataEngine.ProcessRecv;
var
  MsgHeader: TDBMsgHeader;
  MsgData: AnsiString;

  RecvLen, Index: Integer;
  S: AnsiString;
  S2: string;
  IsCheckOK: Boolean;
  PData: PAnsiChar;

  HumData: THumData;
  LoadHuman: TDBLoadHuman;

  HeroData: THeroData;
  LoadHero: TDBLoadHero;
  LoadDummy: TDBLoadDummy;

  DBRenameChr: TDBRenameChr;
  RankData: TDBGetRankData;
  QueryInfo: TDBQueryHumanInfo;

  BuyPlayerInfo: TDBBuyPlayerInfo;
  SellPlayerInfo: TDBSellPlayerInfo;
  DBResult: PTDBResult;

  UserRanking: TUserLevelRanking;
  List: TGStringList;
begin
  while not Terminated do
  begin
    MsgData := '';
    RecvLock;
    try
      RecvLen := Length(FRecvText);
      if RecvLen < SizeOf(TDBMsgHeader) then
        Break;

      MsgHeader := pTDBMsgHeader(@FRecvText[1])^;
      if RecvLen < MsgHeader.nLength + SizeOf(TDBMsgHeader) then
        Break;

      IsCheckOK := True;
      if MsgHeader.nLength > 0 then
      begin
        PData := PAnsiChar(FRecvText);
        Inc(PData, SizeOf(TDBMsgHeader));
        IsCheckOK := BufferCrc(PData, MsgHeader.nLength) = MsgHeader.dwCrc;

        if IsCheckOK then
        begin
          SetLength(MsgData, MsgHeader.nLength);
          Move(PData^, MsgData[1], MsgHeader.nLength);
        end;
      end;

      FRecvText := Copy(FRecvText, MsgHeader.nLength + SizeOf(TDBMsgHeader) + 1, MaxInt);
    finally
      RecvUnLock;
    end;

    if IsCheckOK then
    begin
      case MsgHeader.DefMsg.Ident of
        DBR_LOADHUMANRCD:
          begin
            if MsgHeader.DefMsg.Recog = Length(MsgData) then
            begin
              if MsgHeader.DefMsg.Series = 0 then
              begin
                Index := Pos('/', MsgData);
                if Index > 0 then
                begin
                  S := Copy(MsgData, 1, Index - 1);
                  MsgData := Copy(MsgData, Index + 1, MaxInt);

                  DecodeString(S, PAnsiChar(@LoadHuman), SizeOf(LoadHuman));
                  zLibDecodeString(MsgData, PAnsiChar(@HumData), SizeOf(THumData));

                  New(DBResult);
                  DBResult.ResultType := drtLoadHuman;
                  DBResult.LoadBufLen := GetDBResultSize(DBResult.ResultType);
                  GetMem(DBResult.LoadBuf, DBResult.LoadBufLen);
                  PTDBLoadHuman(DBResult.LoadBuf)^ := LoadHuman;

                  DBResult.DataBufLen := SizeOf(HumData);
                  GetMem(DBResult.DataBuf, DBResult.DataBufLen);
                  PTHumData(DBResult.DataBuf)^ := HumData;

                  DBResult.nResult := 0;
                  DBResult.sResult := '';

                  UserEngine.AddDBResult(DBResult);
                end;
              end
              else if Length(MsgData) > 0 then
              begin
                DecodeString(MsgData, PAnsiChar(@LoadHuman), SizeOf(LoadHuman));
                case MsgHeader.DefMsg.Series of
                  1, 3:
                    begin
                      MainOutMessage(Format('[读取人物失败]无效的帐户角色; 帐户: %s 角色: %s IP: %s ', [LoadHuman.sAccount, LoadHuman.sHumanName,
                        LoadHuman.sIPaddr]));
                    end;
                  2, 4:
                    begin
                      MainOutMessage(Format('[读取人物失败]Session未注册; 帐户: %s 角色: %s SessionID: %d; 错误代码: %d',
                        [LoadHuman.sAccount, LoadHuman.sHumanName, LoadHuman.nSessionID, MsgHeader.DefMsg.Series]));
                    end;
                end;
              end;
            end;
          end;
        DBR_SAVEHUMANRCD:
          begin
            if MsgHeader.DefMsg.Recog = Length(MsgData) then
            begin
              if (MsgHeader.DefMsg.Series <> 0) and (Length(MsgData) > 0) then
              begin
                Index := Pos('/', MsgData);
                if Index > 0 then
                begin
                  S := Copy(MsgData, 1, Index - 1);
                  MsgData := Copy(MsgData, Index + 1, MaxInt);

                  S := DecodeString(S);
                  MsgData := DecodeString(MsgData);

                  case MsgHeader.DefMsg.Series of
                    3:
                      begin
                        MainOutMessage(Format('[保存人物失败]无效的帐户角色; 帐户: %s 角色: %s', [S, MsgData]));
                      end;
                    2:
                      begin
                        MainOutMessage(Format('[保存人物失败]数据库服务器错误; 帐户: %s 角色: %s', [S, MsgData]));
                      end;
                  end;
                end;
              end;
            end;
          end;
        DBR_LOADDUMMY:
          begin
            if MsgHeader.DefMsg.Recog = Length(MsgData) then
            begin
              DecodeString(MsgData, PAnsiChar(@LoadDummy), SizeOf(LoadDummy));

              New(DBResult);
              DBResult.ResultType := drtLoadDummy;
              DBResult.LoadBufLen := GetDBResultSize(DBResult.ResultType);
              GetMem(DBResult.LoadBuf, DBResult.LoadBufLen);
              pTDBLoadDummy(DBResult.LoadBuf)^ := LoadDummy;

              DBResult.DataBufLen := 0;
              DBResult.DataBuf := nil;

              DBResult.nResult := MsgHeader.DefMsg.Series;
              DBResult.sResult := '';

              UserEngine.AddDBResult(DBResult);
            end;
          end;
        DBR_LOADHERORCD:
          begin
            if MsgHeader.DefMsg.Recog = Length(MsgData) then
            begin
              if MsgHeader.DefMsg.Series = 0 then
              begin
                Index := Pos('/', MsgData);
                if Index > 0 then
                begin
                  S := Copy(MsgData, 1, Index - 1);
                  MsgData := Copy(MsgData, Index + 1, MaxInt);

                  DecodeString(S, PAnsiChar(@LoadHero), SizeOf(LoadHero));
                  zLibDecodeString(MsgData, PAnsiChar(@HeroData), SizeOf(HeroData));

                  New(DBResult);
                  DBResult.ResultType := drtLoadHero;
                  DBResult.LoadBufLen := GetDBResultSize(DBResult.ResultType);
                  GetMem(DBResult.LoadBuf, DBResult.LoadBufLen);
                  pTDBLoadHero(DBResult.LoadBuf)^ := LoadHero;

                  DBResult.DataBufLen := SizeOf(HeroData);
                  GetMem(DBResult.DataBuf, DBResult.DataBufLen);
                  PTHeroData(DBResult.DataBuf)^ := HeroData;

                  DBResult.nResult := 0;
                  DBResult.sResult := '';

                  UserEngine.AddDBResult(DBResult);
                end;
              end
              else if Length(MsgData) > 0 then
              begin
                DecodeString(MsgData, PAnsiChar(@LoadHero), SizeOf(LoadHero));
                MainOutMessage(Format('[读取英雄失败]无效的帐户角色; 帐户: %s 角色: %s', [LoadHero.sAccount, LoadHero.sHeroName1]));
              end;
            end;
          end;
        DBR_SAVEHERORCD:
          begin
            if MsgHeader.DefMsg.Recog = Length(MsgData) then
            begin
              if (MsgHeader.DefMsg.Series <> 0) and (Length(MsgData) > 0) then
              begin
                Index := Pos('/', MsgData);
                if Index > 0 then
                begin
                  S := Copy(MsgData, 1, Index - 1);
                  MsgData := Copy(MsgData, Index + 1, MaxInt);

                  S := DecodeString(S);
                  MsgData := DecodeString(MsgData);

                  case MsgHeader.DefMsg.Series of
                    3:
                      begin
                        MainOutMessage(Format('[保存英雄失败]无效的帐户角色; 帐户: %s 角色: %s', [S, MsgData]));
                      end;
                    2:
                      begin
                        MainOutMessage(Format('[保存英雄失败]数据库服务器错误; 帐户: %s 角色: %s', [S, MsgData]));
                      end;
                  end;
                end;
              end;
            end;
          end;
        DBR_NEWHERORCD:
          begin
            if MsgHeader.DefMsg.Recog = Length(MsgData) then
            begin
              DecodeString(MsgData, PAnsiChar(@LoadHero), SizeOf(LoadHero));

              New(DBResult);
              DBResult.ResultType := drtLoadHero;
              DBResult.LoadBufLen := GetDBResultSize(DBResult.ResultType);
              GetMem(DBResult.LoadBuf, DBResult.LoadBufLen);
              pTDBLoadHero(DBResult.LoadBuf)^ := LoadHero;

              DBResult.DataBufLen := 0;
              DBResult.DataBuf := nil;

              DBResult.nResult := MsgHeader.DefMsg.Series;
              DBResult.sResult := '';

              UserEngine.AddDBResult(DBResult);
            end;
          end;
        DBR_DELHERORCD:
          begin
            if MsgHeader.DefMsg.Recog = Length(MsgData) then
            begin
              DecodeString(MsgData, PAnsiChar(@LoadHero), SizeOf(LoadHero));

              New(DBResult);
              DBResult.ResultType := drtLoadHero;
              DBResult.LoadBufLen := GetDBResultSize(DBResult.ResultType);
              GetMem(DBResult.LoadBuf, DBResult.LoadBufLen);
              pTDBLoadHero(DBResult.LoadBuf)^ := LoadHero;

              DBResult.DataBufLen := 0;
              DBResult.DataBuf := nil;

              DBResult.nResult := MsgHeader.DefMsg.Series;
              DBResult.sResult := '';

              UserEngine.AddDBResult(DBResult);
            end;
          end;
        DBR_QUERYSTORAGEHEROINFO:
          begin
            if MsgHeader.DefMsg.Recog = Length(MsgData) then
            begin
              Index := Pos('/', MsgData);
              if Index > 0 then
              begin
                S := Copy(MsgData, 1, Index - 1);
                MsgData := Copy(MsgData, Index + 1, MaxInt);

                DecodeString(S, PAnsiChar(@LoadHero), SizeOf(LoadHero));

                New(DBResult);
                DBResult.ResultType := drtLoadHero;
                DBResult.LoadBufLen := GetDBResultSize(DBResult.ResultType);
                GetMem(DBResult.LoadBuf, DBResult.LoadBufLen);
                pTDBLoadHero(DBResult.LoadBuf)^ := LoadHero;

                DBResult.DataBufLen := 0;
                DBResult.DataBuf := nil;

                DBResult.nResult := MsgHeader.DefMsg.Series;
                DBResult.sResult := MsgData;

                UserEngine.AddDBResult(DBResult);
              end;
            end;
          end;
        DBR_ASSESSHERO:
          begin
            if MsgHeader.DefMsg.Recog = Length(MsgData) then
            begin
              DecodeString(MsgData, PAnsiChar(@LoadHero), SizeOf(LoadHero));

              New(DBResult);
              DBResult.ResultType := drtLoadHero;
              DBResult.LoadBufLen := GetDBResultSize(DBResult.ResultType);
              GetMem(DBResult.LoadBuf, DBResult.LoadBufLen);
              pTDBLoadHero(DBResult.LoadBuf)^ := LoadHero;

              DBResult.DataBufLen := 0;
              DBResult.DataBuf := nil;

              DBResult.nResult := MsgHeader.DefMsg.Series;
              DBResult.sResult := '';

              UserEngine.AddDBResult(DBResult);
            end;
          end;
        DBR_HUMANCHANGENAME, DBR_HEROCHANGENAME:
          begin
            if MsgHeader.DefMsg.Recog = Length(MsgData) then
            begin
              DecodeString(MsgData, PAnsiChar(@DBRenameChr), SizeOf(DBRenameChr));

              New(DBResult);
              DBResult.ResultType := drtChrRename;
              DBResult.LoadBufLen := GetDBResultSize(DBResult.ResultType);
              GetMem(DBResult.LoadBuf, DBResult.LoadBufLen);
              PTDBRenameChr(DBResult.LoadBuf)^ := DBRenameChr;

              DBResult.DataBufLen := 0;
              DBResult.DataBuf := nil;

              DBResult.nResult := MsgHeader.DefMsg.Series;
              DBResult.sResult := '';

              UserEngine.AddDBResult(DBResult);
            end;
          end;
        DBR_HUMANCHANGEGOLD:
          begin
            // 这里不用处理，因为数据包是顺序发送，不存在人物在登录过程中修改金币数量
          end;
        DBR_GETRANKDATA:
          begin
            if MsgHeader.DefMsg.Recog = Length(MsgData) then
            begin
              Index := Pos('/', MsgData);
              if Index > 0 then
              begin
                S := Copy(MsgData, 1, Index - 1);
                MsgData := Copy(MsgData, Index + 1, MaxInt);

                DecodeString(S, PAnsiChar(@RankData), SizeOf(RankData));

                RankData.nPage := MsgHeader.DefMsg.Param;

                New(DBResult);
                DBResult.ResultType := drtGetRankData;
                DBResult.LoadBufLen := GetDBResultSize(DBResult.ResultType);
                GetMem(DBResult.LoadBuf, DBResult.LoadBufLen);
                PTDBGetRankData(DBResult.LoadBuf)^ := RankData;

                DBResult.DataBufLen := 0;
                DBResult.DataBuf := nil;

                DBResult.nResult := MsgHeader.DefMsg.Series;
                DBResult.sResult := MsgData;

                UserEngine.AddDBResult(DBResult);
              end;
            end;
          end;
        DBR_M2CACHERANKDATA:
          begin
            if (MsgHeader.DefMsg.Recog > 0) and (MsgHeader.DefMsg.Recog = Length(MsgData)) then
            begin
              g_RefRankingTick := MakeLong(MsgHeader.DefMsg.Param, MsgHeader.DefMsg.Tag);

              g_HumanRankList.Lock;
              g_WarriorRankList.Lock;
              g_WizardRankList.Lock;
              g_TaoistRankList.Lock;
              try
                g_HumanRankList.Clear;
                g_WarriorRankList.Clear;
                g_WizardRankList.Clear;
                g_TaoistRankList.Clear;

                while True do
                begin
                  if MsgData = '' then
                    Break;
                  MsgData := GetValidStr3_Ex(MsgData, S2, '/');
                  DecodeString(S2, @UserRanking, SizeOf(TUserLevelRanking));
                  if UserRanking.nIndex = 0 then
                    List := g_HumanRankList
                  else if UserRanking.nIndex = 1 then
                    List := g_WarriorRankList
                  else if UserRanking.nIndex = 2 then
                    List := g_WizardRankList
                  else if UserRanking.nIndex = 3 then
                    List := g_TaoistRankList
                  else
                    List := nil;

                  if List <> nil then
                  begin
                    List.AddObject(UserRanking.sChrName, TObject(UserRanking.nLevel));
                  end;
                end;
              finally
                g_HumanRankList.UnLock;
                g_WarriorRankList.UnLock;
                g_WizardRankList.UnLock;
                g_TaoistRankList.UnLock;
              end;
            end;
          end;
        DBR_QUERYHUMANINFO:
          begin
            if MsgHeader.DefMsg.Recog = Length(MsgData) then
            begin
              Index := Pos('/', MsgData);
              if Index > 0 then
              begin
                S := Copy(MsgData, 1, Index - 1);
                MsgData := Copy(MsgData, Index + 1, MaxInt);

                DecodeString(S, PAnsiChar(@QueryInfo), SizeOf(QueryInfo));

                QueryInfo.nResultCount := MsgHeader.DefMsg.Param;

                New(DBResult);
                DBResult.ResultType := drtQueryHumanInfo;
                DBResult.LoadBufLen := GetDBResultSize(DBResult.ResultType);
                GetMem(DBResult.LoadBuf, DBResult.LoadBufLen);
                PTDBQueryHumanInfo(DBResult.LoadBuf)^ := QueryInfo;

                DBResult.DataBufLen := 0;
                DBResult.DataBuf := nil;

                DBResult.nResult := MsgHeader.DefMsg.Series;
                DBResult.sResult := MsgData;

                UserEngine.AddDBResult(DBResult);
              end;
            end;
          end;
        DBR_BUY_PLAYER:
          begin
            if MsgHeader.DefMsg.Recog = Length(MsgData) then
            begin
              DecodeString(MsgData, PAnsiChar(@BuyPlayerInfo), SizeOf(BuyPlayerInfo));

              New(DBResult);
              DBResult.ResultType := drtBuyPlayer;
              DBResult.LoadBufLen := GetDBResultSize(DBResult.ResultType);
              GetMem(DBResult.LoadBuf, DBResult.LoadBufLen);
              PTDBBuyPlayerInfo(DBResult.LoadBuf)^ := BuyPlayerInfo;

              DBResult.DataBufLen := 0;
              DBResult.DataBuf := nil;

              DBResult.nResult := MsgHeader.DefMsg.Param;
              DBResult.sResult := '';

              UserEngine.AddDBResult(DBResult);
            end;
          end;
        DBR_SELL_PLAYER_Delegator:
          begin
            if MsgHeader.DefMsg.Recog = Length(MsgData) then
            begin
              DecodeString(MsgData, PAnsiChar(@SellPlayerInfo), SizeOf(SellPlayerInfo));

              New(DBResult);
              DBResult.ResultType := drtSellPlayerDelegator;
              DBResult.LoadBufLen := GetDBResultSize(DBResult.ResultType);
              GetMem(DBResult.LoadBuf, DBResult.LoadBufLen);
              PTDBSellPlayerInfo(DBResult.LoadBuf)^ := SellPlayerInfo;

              DBResult.DataBufLen := 0;
              DBResult.DataBuf := nil;

              DBResult.nResult := MsgHeader.DefMsg.Param;
              DBResult.sResult := '';

              UserEngine.AddDBResult(DBResult);
            end;
          end;

      end;
    end;

    FIsCanSend := True;
  end;
end;

function TDataEngine.GetTaskID: Integer;
begin
  Inc(FTaskID);
  if FTaskID > High(Word) then
    FTaskID := 1;
  Result := FTaskID;
end;

procedure TDataEngine.ClearTaskList;
var
  I: Integer;
  TaskInfo: PTDBTaskInfo;
begin
  EnterCriticalSection(FTaskCS);
  try
    for I := 0 to FTaskList.Count - 1 do
    begin
      TaskInfo := FTaskList.Items[I];
      FreeMem(TaskInfo.Buf, TaskInfo.BufLen);
      Dispose(TaskInfo);
    end;
    FTaskList.Clear;
  finally
    LeaveCriticalSection(FTaskCS);
  end;
end;

procedure TDataEngine.SendMagicAndStdItems;
var
  I: Integer;
  sSendText: AnsiString;
  Magic: pTMagic;
  StdItem: pTStdItem;

  DefMsg: TDefaultMessage;
begin
  sSendText := '';
  if g_MultiThreadRun then
    UserEngine.m_MagicList.LockR(15);
  try
    for I := 0 to UserEngine.m_MagicList.Count - 1 do
    begin
      Magic := UserEngine.m_MagicList[I];
      sSendText := sSendText + IntToStr(Integer(Magic.MagicAttr)) + '/' + IntToStr(Magic.wMagicId) + '/' + Magic.sMagicName + '/';
    end;
  finally
    if g_MultiThreadRun then
      UserEngine.m_MagicList.UnLockR;
  end;

  DefMsg := MakeDefaultMsg(DB_SAVEMAGICLIST, 0, 0, 0, 0);
  SendDBSockMsg(DefMsg, zEncodeString(sSendText));

  sSendText := '';
  if g_MultiThreadRun then
    UserEngine.StdItemList.LockR(14);
  try
    for I := 0 to UserEngine.StdItemList.Count - 1 do
    begin
      StdItem := UserEngine.StdItemList[I];
      sSendText := sSendText + StdItem.Name + '/';
    end;
  finally
    if g_MultiThreadRun then
      UserEngine.StdItemList.UnLockR;
  end;

  DefMsg := MakeDefaultMsg(DB_SAVESTDITEMLIST, 0, 0, 0, 0);
  SendDBSockMsg(DefMsg, zEncodeString(sSendText));
end;

end.
