unit RunGateUtils;

interface

uses
  Windows, Classes, SysUtils, Forms, ExtCtrls, SyncObjs, IocpUtils,
  IocpTcpServer, MirClientContext, Grobal2_Ex, GateShare, ZlibEx,
  EDcode, IocpWinsock2, IocpCommon, Math, LbAsym, LbRSA, MD5Util,
  CheckUnit, {$IF UseIocpClient <> 0} IocpTcpClient, {$IFEND}
  {$IF NEED_REGISTER = 1} //WinlicenseSDK,
  DesUtils, DateUtils, {$IFEND}
  JSocket, MagicIntervalUtils, CheckCrc;

{$I Iocp.inc}

type
  TRunGate = class;
  TRunGateManager = class;

{$IF UseIocpClient = 0}
  TProcessServerReceiveThread = class(TThread)
  private
    FRunGate: TRunGate;
  protected
    procedure Execute; override;
  public
    property Terminated;
  end;
{$ELSE}
  TMirRemoteContext = class(TIocpRemoteContext)
  private
    FRunGate: TRunGate;
    FSendCheckClientTick: DWORD;                                        // 最后发送心跳包到服务器时间
    FRecvCheckServerTick: DWORD;                                        // 最后返回服务器的心跳包时间

    FTryConnecttionTick: DWORD;                                         // 最后尝试连接时间

    FCacheDatas: array[0..13] of string;

    FSendToServerBufLocker: TIocpCriticalSection;
    FSendToServerBuf: PChar;                                            // 发送数据到服务器 - 数据交换区
    FSendToServerBufLen: Integer;                                       // 发送数据到服务器 - 数据交换区长度
  protected
    function DoCheckRecvBuffer(var S: string): Boolean; override;
    procedure DoConnect; override;
    procedure DoDisconnect(ASocket: TSocket); override;
  private
    procedure ProcessRecvPacket(Buffer: PChar; BufferLen: Cardinal);
    procedure ProcessDecompressPacket(Buffer: Pointer; BufferLen: Integer);

    // 处理转发数据到客户端
    procedure DoForwardToClientData(ASocket, ASocketIndex: Integer; Buffer: PChar; BufferLen: Integer);

    // 收到全服消息
    procedure DoRecvFullServiceMsg(Buffer: PChar; BufferLen: Integer);
  public
    constructor Create(AIocpCore: TIocpCore; ASocket: TSocket = 0); override;
    destructor Destroy; override;

    procedure SendServerMsg(nIdent: Integer; wSocketIndex: Word; nSocket, nUserListIndex: Integer; Buffer: PChar; BufferLen: Integer);
  end;
{$IFEND}

  TRunGate = class
  private
    FOnwer: TRunGateManager;
    FID: Integer;
    FFormHandle: THandle;

{$IF UseIocpClient = 0}
    //FPort: Word;
    FIsStart: Boolean;
    FIsReady: Boolean;

    FTotalRecvBufSize: Int64;                                           // 统计所有接收的数据包长度

    FSendToServerBuf: PChar;                                            // 发送数据到服务器 - 数据交换区
    FSendToServerBufLen: Cardinal;                                      // 发送数据到服务器 - 数据交换区长度

    FSendToClientBuf: PChar;                                            // 数据交换区
    FSendToClientBufLen: Cardinal;                                      // 数据交换区长度

    FRecvPacket: PChar;
    FRecvPacketLen: Cardinal;
    FRecvPacketNoProcessLen: Cardinal;

    FSendCheckClientTick: DWORD;                                        // 最后发送心跳包到服务器时间
    FRecvCheckServerTick: DWORD;                                        // 最后返回服务器的心跳包时间
{$IFEND}

    FOnlineUser: TSafeList;
{$IF NEED_REGISTER = 1}
    FStopCheckIPCount1: Boolean;
{$IFEND}

    FRecvCheckToM2: Boolean;
    FRecvCheckToM2Tick: LongWord;
    FRecvCheckToM2ResponseTime: LongWord;
    FREcvCheckToM2ResponseCount: LongWord;
    FRecvCheckToM2Data1: LongWord;
    FRecvCheckToM2Data2: LongWord;
  private
{$IF UseIocpClient = 0}
    FClientSocket: TClientSocket;
    FIsTryConnectting: Boolean;                                         // 正在尝试连接
    FTryConnecttionTick: DWORD;                                         // 最后尝试连接时间
    FCacheDatas: array[0..12] of string;

    FSendToServerBufLocker: TIocpCriticalSection;
    FReceiveServerBufLocker: TIocpCriticalSection;

    FProcessServerReceiveThread: TProcessServerReceiveThread;

    procedure OnClientSocketConnect(Sender: TObject; Socket: TCustomWinSocket);
    procedure OnClientSocketDisconnect(Sender: TObject; Socket: TCustomWinSocket);
    procedure OnClientSocketError(Sender: TObject; Socket:
      TCustomWinSocket; ErrorEvent: TErrorEvent; var ErrorCode: Integer);
    procedure OnClientSocketRead(Sender: TObject; Socket: TCustomWinSocket);

    procedure ProcessRecvBuffer(Thread: TProcessServerReceiveThread);

    procedure ProcessRecvPacket(Buffer: PChar; BufferLen: Cardinal);
    procedure ProcessDecompressPacket(Buffer: Pointer; BufferLen: Integer);

    // 处理转发数据到客户端
    procedure DoForwardToClientData(ASocket, ASocketIndex: Integer; Buffer: PChar; BufferLen: Integer);
    procedure DoRecvFullServiceMsg(Buffer: PChar; BufferLen: Integer);
  private
{$ELSE}
    FTcpClient: TMirRemoteContext;
{$IFEND}
    FTcpServer: TIocpTcpServer;
    procedure OnAcceptConnect(Sender: TIocpTcpServer; RemoteAddr: string; RemotePort: Word; var IsAccept: Boolean);
  public
    // 最多在线人数
    nMaxOnlineUserCount: Integer;

    // 被攻击次数
    nTotalAttackCount: Integer;

    dwCurDefenseLevel: LongWord;

    dwClearTempTick: LongWord;
    dwResotreDefenseTick: LongWord;

{$IF NEED_REGISTER = 1}
    FStopCheckIPCount2: Boolean;
{$IFEND}
  public
{$IF UseIocpClient = 0}
    procedure SendServerMsg(nIdent: Integer; wSocketIndex: Word;
      nSocket, nUserListIndex: Integer; Buffer: PChar; BufferLen: Integer);
{$IFEND}
    procedure SendOnLineUserMsg(DefMsg: pTDefaultMessage; DataAdd: PChar; DataAddLen: LongWord; CheckLogin: Boolean);
    procedure Run;
  public
    constructor Create(AOnwer: TRunGateManager; APort: Word; AFormHandle: THandle; AID: Integer);
    destructor Destroy; override;

    property ID: Integer read FID;
    function StartService(ThreadCount: Integer): Boolean;
    function StopServices: Boolean;
    property TcpServer: TIocpTcpServer read FTcpServer;
    property OnlineUser: TSafeList read FOnlineUser;
    property MaxOnlineUserCount: Integer read nMaxOnlineUserCount;
{$IF UseIocpClient = 0}
    property IsStart: Boolean read FIsStart;
    property IsReady: Boolean read FIsReady;
{$ELSE}
    property TcpClient: TMirRemoteContext read FTcpClient;
{$IFEND}
  end;

{$IF MultiThreadRunContext <> 0}
  TFullServiceMsgProcessThread = class(TThread)
  private
    FRunGateManager: TRunGateManager;
    FIsRun: Boolean;
  protected
    constructor Create(CreateSuspended: Boolean);
    procedure Execute; override;
  public
    property Terminated;
  end;

  TClientContextRunThread = class(TThread)
  private
    FRunThreadIndex: Integer;
    FIsRun: Boolean;
  protected
    constructor Create(CreateSuspended: Boolean);
    procedure Execute; override;
  public
    property Terminated;
  end;
{$IFEND}

  TRunGateManager = class(TObject)
  private
    FFormHandle: THandle;
    FRunGateList: TList;
    FIsStart: Boolean;

    FTimerCheckConnect: TTimer;

{$IF UseIocpClient <> 0}
    FIocpClient: TIocpTcpClient;
{$IFEND}
    // 全服消息 chongchong 2016-06-30
    FFullServiceMsgList: TSafeList;

{$IF NEED_REGISTER = 1}
    dwCheck1Tick: LongWord;
    dwCheck1Time: LongWord;
{$IFEND}

{$IF CLIENT_ANTIPLUG = 1}
    FLastSendAntiPlugTick: LongWord;
{$IFEND}

    function GetCount: Integer;
    function GetRunGates(Index: Integer): TRunGate;
    function GetRecvBlockCount: Int64;
    function GetRecvBytesSize: Int64;
    function GetSendBlockCount: Int64;
    function GetSendBytesSize: Int64;
    function GetWorkerThreadCount: Integer;

    procedure AddFullServiceMsg(DefMessage: PTDefaultMessage; Buffer: PChar; BufSize: Integer);
    procedure ClearFullServiceMsgList;

    procedure OnIocpError(Sender: TIocpCore; ErrorStr: string; ErrorType: TIocpErrorType; ErrorCode: Integer);
  private
{$IF MultiThreadRunContext = 0}
    FTimerRunContext: TTimer;
    procedure OnTimerRunContext(Sender: TObject);
{$ELSE}
    FFullServiceMsgProcessThread: TFullServiceMsgProcessThread;
    FClientContextRunThreads: array[0..MIRCONTEXT_RUN_THREAD_COUNT - 1] of TClientContextRunThread;
{$IFEND}

{$IF NEED_REGISTER = 1}
  private
    FConnectVerifyServerTick: LongWord;
    FIsVerifyServerResult: Boolean;

    FConnectVerifyServerIndex: Integer;
    FVerifyDataKey: DWORD;
    FVerifyDataText: string;

    FRetryConnectVerifyServerTime: LongWord;
    FRetryConnectVerifyServerCount: Integer;
    FIsCheckRetryVerifyServerResult: Boolean;
    FIsRetryVerifyServerResult: Boolean;

    FTimeLeftStartTick: LongWord;
    FTimeLeft: Int64;
    FIsShowTimeLeft: Boolean;

    FClientSocetkVerify: TClientSocket;

    procedure OnClientSocetkVerifyConnect(Sender: TObject; Socket: TCustomWinSocket);
    procedure OnClientSocetkVerifyError(Sender: TObject;
      Socket: TCustomWinSocket; ErrorEvent: TErrorEvent;
      var ErrorCode: Integer);
    procedure OnClientSocetkVerifyRead(Sender: TObject; Socket: TCustomWinSocket);
{$IFEND}
  public
    procedure OnTimerCheckConnect(Sender: TObject);
  public
    constructor Create(AFormHandle: THandle);
    destructor Destroy; override;

    function Add(APort: Word; AID: Integer): TRunGate;
    property Count: Integer read GetCount;
    property Items[Index: Integer]: TRunGate read GetRunGates;
    property IsStart: Boolean read FIsStart;

    function StartRunGates(RunGateThread: Integer): Boolean;
    procedure StopRunGates;

    procedure SendOnLineUserMsg(DefMsg: pTDefaultMessage; DataAdd: PChar; DataAddLen: LongWord; CheckLogin: Boolean);

    property WorkerThreadCount: Integer read GetWorkerThreadCount;
    property SendBlockCount: Int64 read GetSendBlockCount;
    property SendBytesSize: Int64 read GetSendBytesSize;
    property RecvBlockCount: Int64 read GetRecvBlockCount;
    property RecvBytesSize: Int64 read GetRecvBytesSize;

    procedure GetOnlineUser(List: TList);
  end;

implementation

{$IF UseIocpClient <> 0}

{ TMirRemoteContext }

constructor TMirRemoteContext.Create(AIocpCore: TIocpCore; ASocket: TSocket = 0);
begin
  inherited Create(AIocpCore, ASocket);
  FSendToServerBufLocker := TIocpCriticalSection.Create({$IFDEF USE_SPINLOCK}'SendToServerBufLocker'{$ENDIF});
  FSendToServerBufLen := 1 shl 20;    // 1M
  GetMem(FSendToServerBuf, FSendToServerBufLen);
end;

destructor TMirRemoteContext.Destroy;
begin
  FreeMem(FSendToServerBuf, FSendToServerBufLen);
  FSendToServerBufLocker.Free;
  inherited Destroy;
end;

procedure TMirRemoteContext.DoConnect;
var
  FSetting : TFormatSettings;
  Y, M, D: Word;
  IntVer: Integer;
  GateMsg: TM2MsgHeader;
begin
  inherited DoConnect;
  FSendCheckClientTick := MyGetTickCount;
  FRecvCheckServerTick := MyGetTickCount;

  //if not g_boSendVersionToM2 then
  begin
    FSetting.ShortDateFormat := 'yyyy-MM-dd';
    FSetting.DateSeparator := '-';
    DecodeDate(StrToDate(g_sUpdateTime, FSetting), Y, M, D);
    IntVer := Y * 10000 + M * 100 + D;

  {$I VMProtectBegin.inc}
    GateMsg.dwCode := $AA55AA55;
    GateMsg.nSocket := IntVer;
    GateMsg.wGSocketIdx := 5;
    GateMsg.wIdent := GM_RUN_GATE_VER;
    GateMsg.wUserListIndex := 0;
    GateMsg.nLength := 0;
    PostSendBuffer(@GateMsg, SizeOf(TM2MsgHeader));
  {$I VMProtectEnd.inc}

    //g_boSendVersionToM2 := True;
  end;

  if not g_boRequestMagicList then
  begin
    g_boRequestMagicList := True;

    GateMsg.dwCode := $AA55AA55;
    GateMsg.nSocket := 0;
    GateMsg.wGSocketIdx := 0;
    GateMsg.wIdent := GM_RUN_GATE_MAGICS;
    GateMsg.wUserListIndex := 0;
    GateMsg.nLength := 0;
    PostSendBuffer(@GateMsg, SizeOf(TM2MsgHeader));
  end;
end;

procedure TMirRemoteContext.DoDisconnect(ASocket: TSocket);
var
  MirContext: TMirClientContext;
  I: Integer;
  List: TList;
begin
  inherited DoDisconnect(ASocket);
  List := TList.Create;
  try
    FRunGate.OnlineUser.Lock;
    try
      for I := FRunGate.OnlineUser.Count - 1 downto 0 do
      begin
        MirContext := FRunGate.OnlineUser.Items[I];
        List.Add(MirContext);
      end;
    finally
      FRunGate.OnlineUser.UnLock;
    end;

    for I := 0 to List.Count - 1 do
    begin
      MirContext := List.Items[I];
      
      // 服务器主动断开，不用发送 GM_CLOSE 到服务器了
      MirContext.boSendGmClose := False;

      MirContext.Close;
    end;
  finally
    List.Free;
  end;

  FRunGate.FOnwer.ClearFullServiceMsgList;
end;

procedure TMirRemoteContext.SendServerMsg(nIdent: Integer; wSocketIndex: Word; nSocket, nUserListIndex: Integer; Buffer: PChar; BufferLen: Integer);
var
  GateMsg: TM2MsgHeader;
  nLen: Integer;
begin
  if not Active then Exit;

  GateMsg.dwCode := RUNGATECODE;
  GateMsg.nSocket := nSocket;
  GateMsg.wGSocketIdx := wSocketIndex;
  GateMsg.wIdent := nIdent;
  GateMsg.wUserListIndex := nUserListIndex;
  GateMsg.nLength := BufferLen;

  if Buffer = nil then
  begin
    PostSendBuffer(@GateMsg, SizeOf(TM2MsgHeader));
  end
  else
  begin
    FSendToServerBufLocker.Lock;
    try
      // 因为最后要补个 #0，故加1
      nLen := BufferLen + SizeOf(TM2MsgHeader) + 1;
      if FSendToServerBufLen <= nLen then
      begin
        FSendToServerBufLen := (nLen shr 10 + 1) shl 10;
        ReallocMem(FSendToServerBuf, FSendToServerBufLen);
      end;

      Move(GateMsg, FSendToServerBuf^, SizeOf(TM2MsgHeader));

      Move(Buffer^, PChar(Cardinal(FSendToServerBuf) + SizeOf(TM2MsgHeader))^, BufferLen);

      // 最后一位清0发过去
      PChar(Cardinal(FSendToServerBuf) + nLen - 1)^ := #0;

      PostSendBuffer(FSendToServerBuf, nLen);
    finally
      FSendToServerBufLocker.UnLock;
    end;
  end;
end;

function TMirRemoteContext.DoCheckRecvBuffer(var S: string): Boolean;
var
  MsgHeader: pTM2MsgHeader;
  PacketLen, Len: Integer;
  P: PChar;
  TempS: string;
begin
  Result := False;
  P := PChar(S);
  Len := Length(S);

  if Len >= SizeOf(TM2MsgHeader) then
  begin
    while True do
    begin
      MsgHeader := pTM2MsgHeader(P);

      if MsgHeader.dwCode <> RUNGATECODE then
      begin
        if MsgHeader.wIdent = GM_RUN_GATE_VER then
        begin
          S := Format('!!!!!!!!!!!!!!!!!!!!网关与M2不配套，请更新网关到 %d 及以上版本!!!!!!!!!!!!!!!!!!!!', [MsgHeader.nSocket]);
          AddMainLogMsg(S, 0);
          AddMainLogMsg(S, 0);
          AddMainLogMsg(S, 0);
          AddMainLogMsg(S, 0);
          AddMainLogMsg(S, 0);
          S := '';
          Exit;
        end
        else
        begin
          S := '';
          AddMainLogMsg('TMirRemoteContext.DoCheckRecvBuffer error 1 ', 1);
          Exit;
        end;
      end
      else
      begin
        PacketLen := Abs(MsgHeader.nLength) + SizeOf(TM2MsgHeader);

        // 数据包不完整
        if Len < PacketLen then
        begin
          if P <> PChar(S) then
          begin
            SetLength(TempS, Len);
            Move(P^, TempS[1], Len);
            S := TempS;
          end;
          Exit;
        end;

        if (MsgHeader.wIdent = GM_COMPDATA) then
        begin
          if LongWord(MsgHeader.nSocket) = RUNGATECODEX then
          begin
            ProcessRecvPacket(P, PacketLen);
            Inc(P, PacketLen);
            Dec(Len, PacketLen);
          end
          else
          begin
            AddMainLogMsg('TMirRemoteContext.DoCheckRecvBuffer error 2', 1);
            S := '';
            Exit;
          end;
        end
        else
        begin
          ProcessRecvPacket(P, PacketLen);
          Inc(P, PacketLen);
          Dec(Len, PacketLen);
        end;

        if Len = 0 then
        begin
          S := '';
          Exit;
        end
        else if Len <= SizeOf(TM2MsgHeader) then
        begin
          SetLength(TempS, Len);
          Move(P^, TempS[1], Len);
          S := TempS;
          Exit;
        end;
      end;
    end;
  end;
end;

procedure TMirRemoteContext.ProcessRecvPacket(Buffer: PChar; BufferLen: Cardinal);
var
  MsgHeader: pTM2MsgHeader;
  Context: TMirClientContext;
  BufData: PChar;
  OutBuf: Pointer;
  OutBytes: Integer;

  ErrorNum: Integer;
  pDefMsg: pTDefaultMessage;

  S: string;
begin
  ErrorNum := 0;              
  try
    if BufferLen < SizeOf(TM2MsgHeader) then Exit;

    ErrorNum := 1;
    MsgHeader := pTM2MsgHeader(Buffer);

  {$IF NEED_REGISTER = 0}
    case MsgHeader.wIdent of
  {$IFEND}

    {$IF NEED_REGISTER = 0}
      GM_CHECKSERVER:
    {$ELSE}
        if MsgHeader.wIdent = GM_CHECKSERVER then
    {$IFEND}
        begin
          ErrorNum := 2;
          FRecvCheckServerTick := MyGetTickCount();
        end {$IF NEED_REGISTER = 0};{$IFEND}

    {$IF NEED_REGISTER = 0}
      GM_FULL_SERVICE_MSG:
    {$ELSE}
        else if MsgHeader.wIdent = GM_FULL_SERVICE_MSG then
    {$IFEND}
        begin
          BufData := PChar(Integer(Buffer) + SizeOf(TM2MsgHeader));
          DoRecvFullServiceMsg(BufData, MsgHeader.nLength);
        end {$IF NEED_REGISTER = 0};{$IFEND}

    {$IF NEED_REGISTER = 0}
      GM_SERVERUSERINDEX:
    {$ELSE}
        else if MsgHeader.wIdent = GM_SERVERUSERINDEX then
    {$IFEND}
        begin
          ErrorNum := 3;
          Context := TMirClientContext(TIocpClientContextPool.Instance.Contexts[MsgHeader.wGSocketIdx]);
          if (Context <> nil) and (MsgHeader.nSocket = Context.Socket) then
          begin
            Context.nUserListIndex := MsgHeader.wUserListIndex;
          end;
        end {$IF NEED_REGISTER = 0};{$IFEND}

    {$IF NEED_REGISTER = 0}
      GM_RECEIVE_OK:
    {$ELSE}
        else if MsgHeader.wIdent = GM_RECEIVE_OK then
    {$IFEND}
        begin
          ErrorNum := 4;
          SendServerMsg(GM_RECEIVE_OK, 0, 0, 0, nil, 0);
        end {$IF NEED_REGISTER = 0};{$IFEND}

    {$IF NEED_REGISTER = 0}
      GM_DATA:
    {$ELSE}
        else if MsgHeader.wIdent = GM_DATA then
    {$IFEND}
        begin
          ErrorNum := 5;
          if BufferLen = SizeOf(TM2MsgHeader) then
          begin
            AddMainLogMsg('TProcessRecvDataThread.ProcessRecvPacket, BufferLen = ' + IntToStr(SizeOf(TM2MsgHeader)), 0);
          end
          else
          begin
            ErrorNum := 51;
            BufData := PChar(Integer(Buffer) + SizeOf(TM2MsgHeader));
            ErrorNum := 52;

            if MsgHeader.nLength > 0 then
            begin
              pDefMsg := pTDefaultMessage(BufData);

              // M2发来检查网关是否合法 chongchong 2017-11-14
              {
              if pDefMsg.Ident = SM_CHECK_RUNGATE1 then
              begin
                SendServerMsg(GM_DATA_CACHE, SM_CHECK_RUNGATE1, 0, 0, 0, 0);
              end
              else
              }
              if pDefMsg.Ident = SM_CHECK_RUNGATE2 then
              begin
              {$I VMProtectBegin.inc}
                FRunGate.FRecvCheckToM2Tick := MyGetTickCount;
                FRunGate.FRecvCheckToM2ResponseTime := MsgHeader.nSocket;
                FRunGate.FRecvCheckToM2Data1 := pDefMsg.Recog;
                FRunGate.FRecvCheckToM2Data2 := MakeLong(pDefMsg.Param, pDefMsg.Tag);
                FRunGate.FREcvCheckToM2ResponseCount := 0;
                FRunGate.FRecvCheckToM2 := True;
              {$I VMProtectEnd.inc}
              end
              else
              begin
                DoForwardToClientData(MsgHeader.nSocket, MsgHeader.wGSocketIdx, BufData, MsgHeader.nLength);
              end;
            end
            else
            begin
              DoForwardToClientData(MsgHeader.nSocket, MsgHeader.wGSocketIdx, BufData, MsgHeader.nLength);
            end;
          end;
        end {$IF NEED_REGISTER = 0};{$IFEND}

    {$IF NEED_REGISTER = 0}
      GM_KICK:
    {$ELSE}
        else if MsgHeader.wIdent = GM_KICK then
    {$IFEND}
        begin
          ErrorNum := 6;
          Context := TMirClientContext(TIocpClientContextPool.Instance.Contexts[MsgHeader.wGSocketIdx]);
          if (Context <> nil) and (MsgHeader.nSocket = Context.Socket) then
          begin
            ErrorNum := 7;
            // 怕等100秒排列排不到这个消息，故先清理未发完的消息 chongchong 2015-01-11
            Context.ClearSendCache;
            Context.SendMessaggeToClient('当前登录帐号正在其它位置登录，本机已被强行离线！！！', 0, 255, 249);

            // 延时关闭，保证消息发过去
            Context.DelayClose(100);
          end;
        end {$IF NEED_REGISTER = 0};{$IFEND}

    {$IF NEED_REGISTER = 0}
      GM_CLOSE:   // 人物死了半天不动，被M2踢掉的时候 chongchong 2016-07-16
    {$ELSE}
        else if MsgHeader.wIdent = GM_CLOSE then
    {$IFEND}
        begin
          ErrorNum := 6;
          Context := TMirClientContext(TIocpClientContextPool.Instance.Contexts[MsgHeader.wUserListIndex - 1]);
          if (Context <> nil) and (MsgHeader.nSocket = Context.Socket) then
          begin
            Context.boSendGmClose := False;
            Context.DelayClose(100); // Context.Close;
          end;
        end {$IF NEED_REGISTER = 0};{$IFEND}

    {$IF NEED_REGISTER = 0}
      GM_COMPDATA:    //压缩数据
    {$ELSE}
        else if MsgHeader.wIdent = GM_COMPDATA then
    {$IFEND}
        begin
          ErrorNum := 8;
          OutBuf := nil;
          OutBytes := 0;
          try
            OutBytes := BufferLen;
            BufData := PChar(LongWord(Buffer) + SizeOf(TM2MsgHeader));
            ErrorNum := 9;
            if (MsgHeader.nLength > 0) then
            begin
              if MsgHeader.wUserListIndex = Crc32(System.PByte(BufData), MsgHeader.nLength) then
              begin
                DecompressBuf(Pointer(BufData), MsgHeader.nLength, 0, OutBuf, OutBytes);
              end
              else
              begin
                AddMainLogMsg('[Error] DecompressBuf Crc Error', 1);
              end;
            end;
          except
            AddMainLogMsg('[Exception] DecompressBuf Len = ' + IntToStr(MsgHeader.nLength) + ', Error = ' + IntToStr(GetLastError), 1);
            if (OutBuf <> nil) then FreeMem(OutBuf);
            OutBuf := nil;
          end;

          if (OutBuf <> nil) then
          begin
            ErrorNum := 10;
            ProcessDecompressPacket(OutBuf, OutBytes);
            FreeMem(OutBuf, OutBytes);
          end;
        end {$IF NEED_REGISTER = 0};{$IFEND}

    {$IF NEED_REGISTER = 0}
      GM_RUN_GATE_VER:
    {$ELSE}
        else if MsgHeader.wIdent = GM_RUN_GATE_VER then
    {$IFEND}
        begin
          S := Format('!!!!!!!!!!!!!!!!!!!!网关与M2不配套，请更新网关到 %d 及以上版本!!!!!!!!!!!!!!!!!!!!', [MsgHeader.nSocket]);
          AddMainLogMsg(S, 0);
          AddMainLogMsg(S, 0);
          AddMainLogMsg(S, 0);
          AddMainLogMsg(S, 0);
          AddMainLogMsg(S, 0);
        end;

  {$IF NEED_REGISTER = 0}
    end;
  {$IFEND}
  except
    on E: Exception do
      AddMainLogMsg('TMirRemoteContext.ProcessRecvPacket, ErrorNum = ' + IntToStr(ErrorNum) +', ' + E.Message, 0);
  end;
end;

procedure TMirRemoteContext.ProcessDecompressPacket(Buffer: Pointer; BufferLen: Integer);
var
  nLen: Integer;
  P, BufData: PChar;
  MsgHeader: pTM2MsgHeader;
  pDefMsg: pTDefaultMessage;
  Context: TMirClientContext;

  s: string;
begin
  try
    nLen := BufferLen;
    P := Buffer;
    if nLen >= SizeOf(TM2MsgHeader) then
    begin
      while (True) do
      begin
        MsgHeader := pTM2MsgHeader(P);
        if MsgHeader.dwCode = RUNGATECODE then
        begin
          if (Abs(MsgHeader.nLength) + SizeOf(TM2MsgHeader)) > nLen then Break;

        {$IF NEED_REGISTER = 0}
          case MsgHeader.wIdent of
        {$IFEND}

          {$IF NEED_REGISTER = 0}
            GM_CHECKSERVER:
          {$ELSE}
              if MsgHeader.wIdent = GM_CHECKSERVER then
          {$IFEND}
              begin
                FRecvCheckServerTick := MyGetTickCount();
              end {$IF NEED_REGISTER = 0};{$IFEND}

          {$IF NEED_REGISTER = 0}
            GM_FULL_SERVICE_MSG:
          {$ELSE}
              else if MsgHeader.wIdent = GM_FULL_SERVICE_MSG then
          {$IFEND}
              begin
                BufData := Ptr(Integer(P) + SizeOf(TM2MsgHeader));
                DoRecvFullServiceMsg(BufData, MsgHeader.nLength);
              end {$IF NEED_REGISTER = 0};{$IFEND}

          {$IF NEED_REGISTER = 0}
            GM_SERVERUSERINDEX:
          {$ELSE}
              else if MsgHeader.wIdent = GM_SERVERUSERINDEX then
          {$IFEND}
              begin
                Context := TMirClientContext(TIocpClientContextPool.Instance.Contexts[MsgHeader.wGSocketIdx]);
                if (Context <> nil) and (MsgHeader.nSocket = Context.Socket) then
                begin
                  Context.nUserListIndex := MsgHeader.wUserListIndex;
                end;
              end {$IF NEED_REGISTER = 0};{$IFEND}

          {$IF NEED_REGISTER = 0}
            GM_RECEIVE_OK:
          {$ELSE}
              else if MsgHeader.wIdent = GM_RECEIVE_OK then
          {$IFEND}
              begin
                {
                FRunGate.dwCheckServerTimeMin := tick_diff(FRunGate.dwCheckRecviceTick, MyGetTickCount);
                if FRunGate.dwCheckServerTimeMin > FRunGate.dwCheckServerTimeMax then
                  FRunGate.dwCheckServerTimeMax := FRunGate.dwCheckServerTimeMin;
                FRunGate.dwCheckRecviceTick := MyGetTickCount();
                }
                SendServerMsg(GM_RECEIVE_OK, 0, 0, 0, nil, 0);
              end {$IF NEED_REGISTER = 0};{$IFEND}

          {$IF NEED_REGISTER = 0}
            GM_DATA:
          {$ELSE}
              else if MsgHeader.wIdent = GM_DATA then
          {$IFEND}
              begin
                BufData := Ptr(Integer(P) + SizeOf(TM2MsgHeader));

                if MsgHeader.nLength > 0 then
                begin
                  pDefMsg := pTDefaultMessage(BufData);
                  // M2发来检查网关是否合法 chongchong 2017-11-14
                  {
                  if pDefMsg.Ident = SM_CHECK_RUNGATE1 then
                  begin
                    SendServerMsg(GM_DATA_CACHE, SM_CHECK_RUNGATE1, 0, 0, 0, 0);
                  end
                  else
                  }
                  if pDefMsg.Ident = SM_CHECK_RUNGATE2 then
                  begin
                  {$I VMProtectBegin.inc}
                    FRunGate.FRecvCheckToM2Tick := MyGetTickCount;
                    FRunGate.FRecvCheckToM2ResponseTime := MsgHeader.nSocket;
                    FRunGate.FRecvCheckToM2Data1 := pDefMsg.Recog;
                    FRunGate.FRecvCheckToM2Data2 := MakeLong(pDefMsg.Param, pDefMsg.Tag);
                    FRunGate.FRecvCheckToM2ResponseCount := 0;
                    FRunGate.FRecvCheckToM2 := True;
                  {$I VMProtectEnd.inc}
                  end
                  else
                  begin
                    DoForwardToClientData(MsgHeader.nSocket, MsgHeader.wGSocketIdx, BufData, MsgHeader.nLength);
                  end;
                end
                else
                begin
                  DoForwardToClientData(MsgHeader.nSocket, MsgHeader.wGSocketIdx, BufData, MsgHeader.nLength);
                end;
              end {$IF NEED_REGISTER = 0};{$IFEND}

          {$IF NEED_REGISTER = 0}
            GM_KICK:
          {$ELSE}
              else if MsgHeader.wIdent = GM_KICK then
          {$IFEND}
              begin
                Context := TMirClientContext(TIocpClientContextPool.Instance.Contexts[MsgHeader.wGSocketIdx]);
                if (Context <> nil) and (MsgHeader.nSocket = Context.Socket) then
                begin
                  // 怕等100秒排列排不到这个消息，故先清理未发完的消息 chongchong 2015-01-11
                  Context.ClearSendCache;
                  Context.SendMessaggeToClient('当前登录帐号正在其它位置登录，本机已被强行离线！！！', 0, 255, 249);
                  Context.DelayClose(100);
                end;
              end {$IF NEED_REGISTER = 0};{$IFEND}


          {$IF NEED_REGISTER = 0}
            GM_CLOSE:     // 人物死了半天不动，被M2踢掉的时候 chongchong 2016-07-16
          {$ELSE}
              else if MsgHeader.wIdent = GM_CLOSE then
          {$IFEND}
              begin
                Context := TMirClientContext(TIocpClientContextPool.Instance.Contexts[MsgHeader.wUserListIndex - 1]);
                if (Context <> nil) and (MsgHeader.nSocket = Context.Socket) then
                begin
                  Context.boSendGmClose := False;
                  Context.DelayClose(100); // Context.Close;
                end;
              end {$IF NEED_REGISTER = 0};{$IFEND}

          {$IF NEED_REGISTER = 0}
            GM_RUN_GATE_VER:
          {$ELSE}
              else if MsgHeader.wIdent = GM_RUN_GATE_VER then
          {$IFEND}
              begin
                S := Format('!!!!!!!!!!!!!!!!!!!!网关与M2不配套，请更新网关到 %d 及以上版本!!!!!!!!!!!!!!!!!!!!', [MsgHeader.nSocket]);
                AddMainLogMsg(S, 0);
                AddMainLogMsg(S, 0);
                AddMainLogMsg(S, 0);
                AddMainLogMsg(S, 0);
                AddMainLogMsg(S, 0);
              end;
        {$IF NEED_REGISTER = 0}
          end;
        {$IFEND}
          P := @P[SizeOf(TM2MsgHeader) + Abs(MsgHeader.nLength)];
          nLen := nLen - (Abs(MsgHeader.nLength) + SizeOf(TM2MsgHeader));
        end
        else
        begin
          Inc(P);
          Dec(nLen);
        end;
        if nLen < SizeOf(TM2MsgHeader) then break;
      end;
    end;
  except
    on E: Exception do begin
      AddMainLogMsg('[Exception] TMirRemoteContext.ProcessDecompressPacket', 1);
    end;
  end;
end;

procedure TMirRemoteContext.DoForwardToClientData(ASocket, ASocketIndex: Integer; Buffer: PChar; BufferLen: Integer);
var
  sSendMsg, sTemp: string;
  pDefMsg: pTDefaultMessage;
  Context: TMirClientContext;

  DataAdd: PAnsiChar;
  DataAddLen: Integer;

  ErrorNum: Integer;

  Index: Integer;
  TheDefMsg: TDefaultMessage;

  CRC, dwValue: LongWord;
begin
  ErrorNum := 0;
  try
    if BufferLen < SizeOf(TDefaultMessage) then Exit;

    pDefMsg := pTDefaultMessage(Buffer);

    // 网关发来验证1
    if pDefMsg.Ident = SM_CHECK_RUNGATE1 then
    begin
    {$I VMProtectBegin.inc}
      CRC := MakeLong(pDefMsg.Param, pDefMsg.Tag);
      dwValue := pDefMsg.Recog;

      SetLength(sSendMsg, SizeOf(dwValue) + SizeOf(CRC));
      Move(CRC, sSendMsg[1], SizeOf(CRC));
      Move(dwValue, sSendMsg[1 + SizeOf(CRC)], SizeOf(dwValue));

      CRC := $522582F4;
      for Index := 1 to Length(sSendMsg) do
      begin
        CRC := ((CRC shr 2) xor (CRC shl 6)) xor Byte(sSendMsg[Index]);    
      end;

      SendServerMsg(GM_DATA_CACHE, SM_CHECK_RUNGATE1, CRC, CRC xor dwValue, nil, 0);
      SendServerMsg(GM_DATA_CACHE, SM_CHECK_RUNGATE1, CRC, CRC xor dwValue, nil, 0);
    {$I VMProtectEnd.inc}

      Exit;
    end;

    Context := TMirClientContext(TIocpClientContextPool.Instance.Contexts[ASocketIndex]);

    if (Context = nil) then Exit;
    if (Context.Socket <> ASocket) then Exit;

    ErrorNum := 1;
    // 数据包不带包头
    if BufferLen <= 0 then
    begin
      Exit;
    end;

    DataAdd := nil;
    DataAddLen := 0;

    if BufferLen > SizeOf(TDefaultMessage) then
    begin
      DataAdd := Buffer;
      Inc(DataAdd, SizeOf(TDefaultMessage));

      DataAddLen := BufferLen - SizeOf(TDefaultMessage);
    end;

    // 通过服务器返回取得对象id chongchong 2014-12-15
    if pDefMsg.Ident = SM_LOGON then
    begin
      Context.nRecogId := pDefMsg.Recog;
      Context.boFirstClientQueryBagItems := False;
      Context.boChangeMap := False;
    end

  {$IF NEED_REGISTER = 0}
    else if pDefMsg.Ident = SM_RUNGATE_DISABLE_LOGIN then
    begin
      AddMainLogMsg('Recv m2server message SM_RUNGATE_DISABLE_LOGIN', 0);
    end
  {$IFEND}

    // 服务器告诉客户端相关速度
    else if (pDefMsg.Ident = SM_CHANGESPEED) and
      (pDefMsg.Recog = Context.nRecogId) then
    begin
      ErrorNum := 5;
      Context.nMoveSpeed := SmallInt(pDefMsg.Param);
      Context.nAttackSpeed := SmallInt(pDefMsg.Tag);
      Context.nSpellSpeed := SmallInt(pDefMsg.Series);
    end

    else if (pDefMsg.Ident = SM_ABILITY) then
    begin
      ErrorNum := 6;
      Context.btJob := LoByte(pDefMsg.Param);
      if Context.btJob > 2 then Context.btJob := 0;
    end

    else if (pDefMsg.Ident = SM_HEROABILITY) then
    begin
      ErrorNum := 7;
      Context.btHeroJob := LoByte(pDefMsg.Param);
      if Context.btHeroJob > 2 then Context.btHeroJob := 0;
    end

    else if pDefMsg.Ident = SM_CHANGEMAP then
    begin
      if (BufferLen > SizeOf(TDefaultMessage)) then
      begin
        Context.sMapName := DecodeBuffer(DataAdd, DataAddLen);
        Index := Pos(sLineBreak, Context.sMapName);
        if Index > 0 then
        begin
          Context.sMapName := Copy(Context.sMapName, Index + 2, MaxInt);
        end;
      end;
      Context.boChangeMap := True;

      if g_boOpenVerifyCode then
      begin
        Context.boSendVerifyCode := False;
        Context.nVerifyCodeErrCount := 0;
        Context.nVerifyCodeRefreshCount := 0;
        Context.sVerifyCode := '';
      end;
    end

    else if pDefMsg.Ident = SM_NEWMAP then
    begin
      if (BufferLen > SizeOf(TDefaultMessage)) then
      begin
        Context.sMapName := DecodeBuffer(DataAdd, DataAddLen);
        Index := Pos(sLineBreak, Context.sMapName);
        if Index > 0 then
        begin
          Context.sMapName := Copy(Context.sMapName, Index + 2, MaxInt);
        end;
      end;

      if g_boOpenVerifyCode then
      begin
        Context.dwSendVerifyCodeTick := MyGetTickCount;

        // 验证失败重进游戏立即验证
        if g_boVerifyFailLoginVerify then
        begin
          g_VerifyFailUserList.Lock;
          try
            if g_VerifyFailUserList.IndexOf(Context.sChrName) >= 0 then
            begin
              Context.dwSendVerifyCodeTick := 0;
              Context.boVerifyDisableAttack := True;
            end;
          finally
            g_VerifyFailUserList.UnLock;
          end;
        end;

        Context.boSendVerifyCode := False;
        Context.nVerifyCodeErrCount := 0;
        Context.nVerifyCodeRefreshCount := 0;
        Context.sVerifyCode := '';
      end;
    end

    // 将部分数据缓存到网关，以提高M2到网关的数据转换率 chongchong 2015-05-16
    else if (pDefMsg.Ident = SM_MODULEMD5) or
      (pDefMsg.Ident = SM_SENDCUSTOMMONSTERCONFIG) or
      (pDefMsg.Ident = SM_STDITEMLIST) or
      (pDefMsg.Ident = SM_SENDITEMDESCLIST) or
      (pDefMsg.Ident = SM_SENDTZITEMDESCLIST) or
      (pDefMsg.Ident = SM_SENDFILTERITEMLIST) or
      (pDefMsg.Ident = SM_EFFECTIMAGELIST) or
      (pDefMsg.Ident = SM_SPECIALCMD) or
      (pDefMsg.Ident = SM_SENDCUSTOMMAGICCONFIG) or
      (pDefMsg.Ident = SM_PLUGFILE) or
      (pDefMsg.Ident = SM_SERVERCONFIG) or
      (pDefMsg.Ident = SM_SENDCUSTOMNPCCONFIG) or
      (pDefMsg.Ident = SM_SENDITEMDESCTOPLIST) or
      (pDefMsg.Ident = SM_SENDDROPITEMEFFECTLIST) then
    begin
      // 先要缓存看看
      if (DataAdd <> nil) and (DataAddLen > 0) then
      begin
      {$IF NEED_REGISTER = 0}
        case pDefMsg.Ident of
          SM_MODULEMD5:                 Index := 0;
          SM_SENDCUSTOMMONSTERCONFIG:   Index := 1;
          SM_STDITEMLIST:               Index := 2;
          SM_SENDITEMDESCLIST:          Index := 3;
          SM_SENDTZITEMDESCLIST:        Index := 4;
          SM_SENDFILTERITEMLIST:        Index := 5;
          SM_EFFECTIMAGELIST:           Index := 6;
          SM_SPECIALCMD:                Index := 7;
          SM_SENDCUSTOMMAGICCONFIG:     Index := 8;
          SM_PLUGFILE:                  Index := 9;
          SM_SERVERCONFIG:              Index := 10;
          SM_SENDCUSTOMNPCCONFIG:       Index := 11;
          SM_SENDITEMDESCTOPLIST:       Index := 12;
          SM_SENDDROPITEMEFFECTLIST:    Index := 13;
        else
          Index := -1;
        end;
      {$ELSE}
        if pDefMsg.Ident = SM_MODULEMD5 then
          Index := 0
        else if pDefMsg.Ident = SM_SENDCUSTOMMONSTERCONFIG then
          Index := 1
        else if pDefMsg.Ident = SM_STDITEMLIST then
          Index := 2
        else if pDefMsg.Ident = SM_SENDITEMDESCLIST then
          Index := 3
        else if pDefMsg.Ident = SM_SENDTZITEMDESCLIST then
          Index := 4
        else if pDefMsg.Ident = SM_SENDFILTERITEMLIST then
          Index := 5
        else if pDefMsg.Ident = SM_EFFECTIMAGELIST then
          Index := 6
        else if pDefMsg.Ident = SM_SPECIALCMD then
          Index := 7
        else if pDefMsg.Ident = SM_SENDCUSTOMMAGICCONFIG then
          Index := 8
        else if pDefMsg.Ident = SM_PLUGFILE then
          Index := 9
        else if pDefMsg.Ident = SM_SERVERCONFIG then
          Index := 10
        else if pDefMsg.Ident = SM_SENDCUSTOMNPCCONFIG then
          Index := 11
        else if pDefMsg.Ident = SM_SENDITEMDESCTOPLIST then
          Index := 12
        else if pDefMsg.Ident = SM_SENDDROPITEMEFFECTLIST then
          Index := 13
        else
          Index := -1;
      {$IFEND}

        if Index <> -1 then
        begin
          SetLength(FCacheDatas[Index], DataAddLen);
          Move(DataAdd^, FCacheDatas[Index][1], DataAddLen);

          CRC := BufferCrc(DataAdd, DataAddLen);
          SendServerMsg(GM_DATA_CACHE, pDefMsg.Ident, CRC, 0, nil, 0);
        end;
      end;
    end
    else if (pDefMsg.Ident >= SM_MODULEMD5_CACHE) and (pDefMsg.Ident <= SM_SENDDROPITEMEFFECTLIST_CACHE) then
    begin
      TheDefMsg := pDefMsg^;

    {$IF NEED_REGISTER = 0}
      case pDefMsg.Ident of
        SM_MODULEMD5_CACHE:                TheDefMsg.Ident := SM_MODULEMD5;
        SM_SENDCUSTOMMONSTERCONFIG_CACHE:  TheDefMsg.Ident := SM_SENDCUSTOMMONSTERCONFIG;
        SM_STDITEMLIST_CACHE:              TheDefMsg.Ident := SM_STDITEMLIST;
        SM_SENDITEMDESCLIST_CACHE:         TheDefMsg.Ident := SM_SENDITEMDESCLIST;
        SM_SENDTZITEMDESCLIST_CACHE:       TheDefMsg.Ident := SM_SENDTZITEMDESCLIST;
        SM_SENDFILTERITEMLIST_CACHE:       TheDefMsg.Ident := SM_SENDFILTERITEMLIST;
        SM_EFFECTIMAGELIST_CACHE:          TheDefMsg.Ident := SM_EFFECTIMAGELIST;
        SM_SPECIALCMD_CACHE:               TheDefMsg.Ident := SM_SPECIALCMD;
        SM_SENDCUSTOMMAGICCONFIG_CACHE:    TheDefMsg.Ident := SM_SENDCUSTOMMAGICCONFIG;
        SM_PLUGFILE_CACHE:                 TheDefMsg.Ident := SM_PLUGFILE;
        SM_SERVERCONFIG_CACHE:             TheDefMsg.Ident := SM_SERVERCONFIG;
        SM_SENDCUSTOMNPCCONFIG_CACHE:      TheDefMsg.Ident := SM_SENDCUSTOMNPCCONFIG;
        SM_SENDITEMDESCTOPLIST_CACHE:      TheDefMsg.Ident := SM_SENDITEMDESCTOPLIST;
        SM_SENDDROPITEMEFFECTLIST_CACHE:   TheDefMsg.Ident := SM_SENDDROPITEMEFFECTLIST;
      end;
    {$ELSE}
      if pDefMsg.Ident = SM_MODULEMD5_CACHE then
        TheDefMsg.Ident := SM_MODULEMD5
      else if pDefMsg.Ident = SM_SENDCUSTOMMONSTERCONFIG_CACHE then
        TheDefMsg.Ident := SM_SENDCUSTOMMONSTERCONFIG
      else if pDefMsg.Ident = SM_STDITEMLIST_CACHE then
        TheDefMsg.Ident := SM_STDITEMLIST
      else if pDefMsg.Ident = SM_SENDITEMDESCLIST_CACHE then
        TheDefMsg.Ident := SM_SENDITEMDESCLIST
      else if pDefMsg.Ident = SM_SENDTZITEMDESCLIST_CACHE then
        TheDefMsg.Ident := SM_SENDTZITEMDESCLIST
      else if pDefMsg.Ident = SM_SENDFILTERITEMLIST_CACHE then
        TheDefMsg.Ident := SM_SENDFILTERITEMLIST
      else if pDefMsg.Ident = SM_EFFECTIMAGELIST_CACHE then
        TheDefMsg.Ident := SM_EFFECTIMAGELIST
      else if pDefMsg.Ident = SM_SPECIALCMD_CACHE then
        TheDefMsg.Ident := SM_SPECIALCMD
      else if pDefMsg.Ident = SM_SENDCUSTOMMAGICCONFIG_CACHE then
        TheDefMsg.Ident := SM_SENDCUSTOMMAGICCONFIG
      else if pDefMsg.Ident =  SM_PLUGFILE_CACHE then
        TheDefMsg.Ident := SM_PLUGFILE
      else if pDefMsg.Ident = SM_SERVERCONFIG_CACHE then
        TheDefMsg.Ident := SM_SERVERCONFIG
      else if pDefMsg.Ident = SM_SENDCUSTOMNPCCONFIG_CACHE then
        TheDefMsg.Ident := SM_SENDCUSTOMNPCCONFIG
      else if pDefMsg.Ident = SM_SENDITEMDESCTOPLIST_CACHE then
        TheDefMsg.Ident := SM_SENDITEMDESCTOPLIST
      else if pDefMsg.Ident = SM_SENDDROPITEMEFFECTLIST_CACHE then
        TheDefMsg.Ident := SM_SENDDROPITEMEFFECTLIST;
    {$IFEND}

      sTemp := FCacheDatas[pDefMsg.Ident - SM_MODULEMD5_CACHE];
      Context.AddServerMsg(@TheDefMsg, PChar(sTemp), Length(sTemp));

      Exit;
    end

    // 发送客户端反外挂模块，插到发送黑名单前面 chongchong 2016-05-28
    else if pDefMsg.Ident = SM_BLACKMODULEMD5 then
    begin
    {$IF CLIENT_ANTIPLUG = 1}
      // 黑名单功能已经取消没鸟用，直接用内核反外挂把这个替换掉 chongchong 2016-05-28
      if (not g_boLogoutNoResendAntiplugStream) or (Context.dwRecvClientAntiplugCRC <> g_ClientAntiPlugDllStringCRC) then
      begin
        Context.SendAntiPlugStreamInfo;
      end;
    {$IFEND}

      // 黑名单进程在这里发过去算了 chongchong 2016-08-01
      if (Length(g_ProcessBlacklistStr) > 0) and (SM_PROCESSBLACKLIST > 0) then
      begin
        Context.dwSendProcessBlacklistTick := MyGetTickCount;
        Context.SendProcessBlacklistMD5 := g_ProcessBlacklistMd5;
        TheDefMsg := MakeDefaultMsg(SM_PROCESSBLACKLIST, Length(g_ProcessBlacklistStr), 0, 0, 0);
        Context.AddServerMsg(@TheDefMsg, PChar(g_ProcessBlacklistStr), Length(g_ProcessBlacklistStr));
      end;

      if not Context.boIsOldClient then
      begin
        TheDefMsg := MakeDefaultMsg(SM_ITEMEAT_CDTIME, Length(sSendMsg), 0, 0, 0);
        Context.AddServerMsg(@TheDefMsg, @g_EatItemCDConfig, SizeOf(g_EatItemCDConfig));

        TheDefMsg := MakeDefaultMsg(SM_RUNGATE_SPEED_INTERVALS, Length(g_SendToClientSpeedIntervalsText), LoWord(g_SendToClientSpeedIntervalsLen), HiWord(g_SendToClientSpeedIntervalsLen), 0);
        Context.AddServerMsg(@TheDefMsg, PChar(g_SendToClientSpeedIntervalsText), Length(g_SendToClientSpeedIntervalsText));

        TheDefMsg := MakeDefaultMsg(SM_SOFT_DELAY_EXIT, 0, 0, 0, 0);
        Context.AddServerMsg(@TheDefMsg, nil, 0);
      end;
    end

    // 点了公告后，第二次反外挂模块验证
    else if pDefMsg.Ident = SM_SENDNOTICE then
    begin

    end

    // 添加技能，更新技能CD间隔
    else if (DataAdd <> nil) and (DataAddLen > 0) then
    begin
      ErrorNum := 50;
      if (pDefMsg.Ident = SM_SENDMYMAGIC) then
      begin
        ErrorNum := 51;
        if Context.ProcesssSendToClientSendMyMagic(pDefMsg, DataAdd, DataAddLen) then
        begin
          Exit;
        end;
      end

      else if (pDefMsg.Ident = SM_ADDMAGIC) then
      begin
        ErrorNum := 54;
        if Context.ProcesssSendToClientSendAddMagic(pDefMsg, DataAdd, DataAddLen) then
        begin
          Exit;
        end;
      end

      //记录人物包裹药品
      else if (pDefMsg.Ident = SM_BAGITEMS) or (pDefMsg.Ident = SM_HEROBAGITEMS) then
      begin
        ErrorNum := 55;
        Context.ProcesssSendToClientBagItems(pDefMsg, DataAdd, DataAddLen, pDefMsg.Ident = SM_BAGITEMS);
      end

      else if (pDefMsg.Ident = SM_ADDITEM) or (pDefMsg.Ident = SM_HEROADDITEM) then
      begin
        ErrorNum := 56;
        Context.ProcesssSendToClientAddItem(pDefMsg, DataAdd, DataAddLen, pDefMsg.Ident = SM_ADDITEM);
      end

      else if (pDefMsg.Ident = SM_DELITEM) or (pDefMsg.Ident = SM_HERODELITEM) then
      begin
        ErrorNum := 57;
        Context.ProcesssSendToClientDelItems(pDefMsg, DataAdd, DataAddLen, pDefMsg.Ident = SM_DELITEM);
      end

      else if (pDefMsg.Ident = SM_DELITEMS) or (pDefMsg.Ident = SM_HERODELITEMS) then
      begin
        ErrorNum := 58;
        Context.ProcesssSendToClientDelItems(pDefMsg, DataAdd, DataAddLen, pDefMsg.Ident = SM_DELITEMS);
      end

      else if (pDefMsg.Ident = SM_DROPITEM_SUCCESS) or (pDefMsg.Ident = SM_HERODROPITEM_SUCCESS) then
      begin
        ErrorNum := 59;
        Context.ProcesssSendToClientDropItem(pDefMsg, nil, 0, pDefMsg.Ident = SM_DROPITEM_SUCCESS);
      end
    end
    else if (pDefMsg.Ident = SM_EAT_OK) or (pDefMsg.Ident = SM_AUTOEAT_OK) then
    begin
      ErrorNum := 60;
      Context.ProcesssSendToClientEatItemOK(pDefMsg, nil, 0, True);
    end
    else if (pDefMsg.Ident = SM_HEROEAT_OK) or (pDefMsg.Ident = SM_HEROAUTOEAT_OK) then
    begin
      ErrorNum := 70;
      Context.ProcesssSendToClientEatItemOK(pDefMsg, nil, 0, False);
    end

    else if (pDefMsg.Ident = SM_MASTERBAGTOHEROBAG_OK) then
    begin
      ErrorNum := 90;
      Context.ProcesssSendToClientMasterBagToHeroBagOK(pDefMsg, nil, 0, True);
    end

    else if (pDefMsg.Ident = SM_HEROBAGTOMASTERBAG_OK) then
    begin
      ErrorNum := 100;
      Context.ProcesssSendToClientHeroBagToMasterBagOK(pDefMsg, nil, 0, False);
    end

    // 将物品发送间隔附加上去 2020-03-08
    else if (pDefMsg.Ident = SM_ENABLE_UPLOAD_PICKITEMS) then
    begin
      pDefMsg.Tag := g_Config.dwClientUploadPickItemsTime;
      Context.boEnableClientUploadPickItems := True;
    end;

    ErrorNum := 110;

    // 转发数据到客户端
    Context.AddServerMsg(pDefMsg, DataAdd, DataAddLen);
  except
    on E: Exception do
    begin
      AddMainLogMsg('TMirRemoteContext.DoForwardToClientData, ErrorNum = ' + IntToStr(ErrorNum) +', ' + E.Message, 0);
    end;
  end;
end;

procedure TMirRemoteContext.DoRecvFullServiceMsg(Buffer: PChar; BufferLen: Integer);
var
  pDefMsg: pTDefaultMessage;
begin
  if (BufferLen > SizeOf(TDefaultMessage)) then
  begin
    pDefMsg := pTDefaultMessage(Buffer);

    // 后面有个 #0 这里干脆不要算了
    FRunGate.FOnwer.AddFullServiceMsg(pDefMsg, @Buffer[SizeOf(TDefaultMessage)], BufferLen - SizeOf(TDefaultMessage));
  end;
end;

{$IFEND}

{ TRunGate }

constructor TRunGate.Create(AOnwer: TRunGateManager; APort: Word; AFormHandle: THandle; AID: Integer);
begin
  inherited Create;
  FOnwer := AOnwer;

  FID := AID;
  FFormHandle := AFormHandle;

  TIocpClientContextPool.Instance.RegisterClientContextClass(TMirClientContext);
  FTcpServer := TIocpTcpServer.Create(nil);
  FTcpServer.Address := g_sGateAddr;
  FTcpServer.Port := APort;
  FTcpServer.SystemSocketHeartState := False;
  FTcpServer.OnError := AOnwer.OnIocpError;
  FTcpServer.BindObject := Self;
  FTcpServer.OnAcceptConnect := OnAcceptConnect;

{$IF UseIocpClient = 0}
  FClientSocket := TClientSocket.Create(nil);
  FClientSocket.ClientType := ctNonBlocking;
  FClientSocket.OnConnect := OnClientSocketConnect;
  FClientSocket.OnDisconnect := OnClientSocketDisconnect;
  FClientSocket.OnError := OnClientSocketError;
  FClientSocket.OnRead := OnClientSocketRead;

  FSendToServerBufLen := 4 shl 20;    // 4M
  GetMem(FSendToServerBuf, FSendToServerBufLen);

  FSendToClientBufLen := 4 shl 20;
  GetMem(FSendToClientBuf, FSendToClientBufLen);

  FRecvPacketLen := 4 shl 20;
  GetMem(FRecvPacket, FRecvPacketLen);
  FRecvPacketNoProcessLen := 0;

  FSendToServerBufLocker := TIocpCriticalSection.Create({$IFDEF USE_SPINLOCK}'SendToServerBufLocker'{$ENDIF});
  FReceiveServerBufLocker := TIocpCriticalSection.Create({$IFDEF USE_SPINLOCK}'ReceiveServerBufLocker'{$ENDIF});
{$ELSE}
  FTcpClient := TMirRemoteContext(AOnwer.FIocpClient.Add);
  FTcpClient.Address := g_sServerAddr;
  FTcpClient.Port := g_wdServerPort;
  FTcpClient.FRunGate := Self;
{$IFEND}

  nMaxOnlineUserCount := 0;
  nTotalAttackCount := 0;

  FOnlineUser := TSafeList.Create({$IFDEF USE_SPINLOCK}'OnlineUserLocker'{$ENDIF});
  dwCurDefenseLevel := g_dwDefenseLevel;

{$IF NEED_REGISTER = 1}
  FStopCheckIPCount1 := False;
  FStopCheckIPCount2 := False;
{$IFEND}

  FRecvCheckToM2Tick := MyGetTickCount;
  FRecvCheckToM2ResponseTime := 0;
  FRecvCheckToM2 := False;
  FRecvCheckToM2Data1 := 0;
  FRecvCheckToM2Data2 := 0;
  FRecvCheckToM2ResponseCount := 0;
end;

destructor TRunGate.Destroy;
begin
  FTcpServer.Free;
  FTcpServer := nil;

{$IF UseIocpClient = 0}
  FClientSocket.Free;
  FreeMem(FSendToServerBuf, FSendToServerBufLen);
  FreeMem(FSendToClientBuf, FSendToClientBufLen);
  FreeMem(FRecvPacket, FRecvPacketLen);
  FSendToServerBufLocker.Free;
  FReceiveServerBufLocker.Free;
{$ELSE}
  FTcpClient.Active := False;
{$IFEND}

  FOnlineUser.Free; 

  inherited;
end;

function TRunGate.StartService(ThreadCount: Integer): Boolean;
begin
  Result := False;
  try
    SendGameCenterMsg(SG_STARTNOW, IntToStr(FFormHandle));
    AddMainLogMsg('正在启动服务...', 2);

    FTcpServer.OnError := nil;
    FTcpServer.Active := False;
    FTcpServer.IocpCore.WorkerThreadCount := ThreadCount;
    FTcpServer.OnError := FOnwer.OnIocpError;
    FTcpServer.Active := True;

{$IF UseIocpClient = 0}
    FTryConnecttionTick := MyGetTickCount;
    FSendCheckClientTick := MyGetTickCount;
    FRecvCheckServerTick := MyGetTickCount;

    FClientSocket.Active := False;
    FClientSocket.Address := g_sServerAddr;
    FClientSocket.Port := g_wdServerPort;

    FIsStart := True;
    FIsReady := False;
{$IFEND}

//{$IF NEED_REGISTER = 0}
  {$IF UseIocpClient = 0}
    FIsTryConnectting := True;
    FClientSocket.Active := True;
  {$ELSE}
    //FOnwer.FIocpClient.OnError := nil;
    FTcpClient.Active := False;
    FTcpClient.FTryConnecttionTick := MyGetTickCount;
    FTcpClient.Active := True;
    //FOnwer.FIocpClient.OnError := FOnwer.OnIocpError;
  {$IFEND}
//{$IFEND}

    AddMainLogMsg('启动服务成功', 2);
    SendGameCenterMsg(SG_STARTOK, IntToStr(FFormHandle));

    Result := True;
  except
    on E: Exception do
    begin
      AddMainLogMsg(E.Message, 0);
    end;
  end;
end;

function TRunGate.StopServices: Boolean;
begin
  AddMainLogMsg('正在停止服务...', 2);

  FTcpServer.Active := False;

{$IF UseIocpClient = 0}
  FIsStart := False;
  FIsReady := False;
  if FClientSocket <> nil then
    FClientSocket.Close;
{$ELSE}
  FTcpClient.Active := False;
{$IFEND}

  AddMainLogMsg('停止服务成功...', 2);
  Result := True;
end;

{$IF UseIocpClient = 0}
procedure TRunGate.OnClientSocketConnect(Sender: TObject;
  Socket: TCustomWinSocket);
var
  I: Integer;
  MirContext: TMirClientContext;
  ErrorNum: Integer;
  List: TList;

  FSetting : TFormatSettings;
  Y, M, D: Word;
  IntVer: Integer;

  GateMsg: TM2MsgHeader;
begin
  ErrorNum := 0;
  try
    List := TList.Create;
    try
      FOnlineUser.Lock;
      try
        for I := FOnlineUser.Count - 1 downto 0 do
        begin
          MirContext := FOnlineUser.Items[I];
          List.Add(MirContext);
        end;
      finally
        FOnlineUser.UnLock;
      end;

      for I := 0 to List.Count - 1 do
      begin
        MirContext := List.Items[I];
        MirContext.Close;
      end;
    finally
      List.Free;
    end;

    ErrorNum := 2;
    FRecvPacketNoProcessLen := 0;

    ErrorNum := 3;
    FIsReady := True;
    FIsTryConnectting := False;
    FRecvCheckServerTick := MyGetTickCount();
    AddMainLogMsg('连接M2Server[' + Socket.RemoteAddress + ':' + IntToStr(Socket.RemotePort) + ']成功', 1);

    if Assigned(FProcessServerReceiveThread) then
    begin
      FProcessServerReceiveThread.Terminate;
      FProcessServerReceiveThread.WaitFor;
      FProcessServerReceiveThread.Free;
      FProcessServerReceiveThread := nil;
    end;

    //if not g_boSendVersionToM2 then
    begin
      FSetting.ShortDateFormat := 'yyyy-MM-dd';
      FSetting.DateSeparator := '-';
      DecodeDate(StrToDate(g_sUpDateTime, FSetting), Y, M, D);
      IntVer := Y * 10000 + M * 100 + D;

      GateMsg.dwCode := $AA55AA55;
      GateMsg.nSocket := IntVer;
      GateMsg.wGSocketIdx := 5;
      GateMsg.wIdent := GM_RUN_GATE_VER;
      GateMsg.wUserListIndex := 0;
      GateMsg.nLength := 0;
      FClientSocket.Socket.SendBuf(@GateMsg, SizeOf(TM2MsgHeader));

      //g_boSendVersionToM2 := True;
    end;

    FProcessServerReceiveThread := TProcessServerReceiveThread.Create(True);
    FProcessServerReceiveThread.FRunGate := Self;
    FProcessServerReceiveThread.FreeOnTerminate := False;
    FProcessServerReceiveThread.Resume;
  except
    on E: Exception do
      AddMainLogMsg('TRunGate.OnClientSocketConnect, ErrorNum = ' + IntToStr(ErrorNum) +', ' + E.Message, 0);
  end;
end;

procedure TRunGate.OnClientSocketDisconnect(Sender: TObject; Socket: TCustomWinSocket);
var
  I: Integer;
  MirContext: TMirClientContext;
  ErrorNum: Integer;
  List: TList;
begin
  ErrorNum := 0;
  try
    List := TList.Create;
    try
      FOnlineUser.Lock;
      try
        for I := FOnlineUser.Count - 1 downto 0 do
        begin
          MirContext := FOnlineUser.Items[I];
          List.Add(MirContext);
        end;
      finally
        FOnlineUser.UnLock;
      end;

      for I := 0 to List.Count - 1 do
      begin
        MirContext := List.Items[I];
        // 服务器主动断开，不用发送 GM_CLOSE 到服务器了
        MirContext.boSendGmClose := False;
        MirContext.Close;
      end;
    finally
      List.Free;
    end;

    ErrorNum := 1;
    FIsReady := False;
    FIsTryConnectting := False;

    AddMainLogMsg('断开M2Server连接[' + Socket.RemoteAddress + ':' + IntToStr(Socket.RemotePort) + ']', 1);
    
    ErrorNum := 2;
    FOnwer.ClearFullServiceMsgList;

    ErrorNum := 3;
    if Assigned(FProcessServerReceiveThread) then
    begin
      FProcessServerReceiveThread.Terminate;
      FProcessServerReceiveThread.WaitFor;
      FProcessServerReceiveThread.Free;
      FProcessServerReceiveThread := nil;
    end;
    FRecvPacketNoProcessLen := 0;
  except
    on E: Exception do
      AddMainLogMsg('TRunGate.OnClientSocketDisconnect, ErrorNum = ' + IntToStr(ErrorNum) +', ' + E.Message, 0);
  end;
end;

procedure TRunGate.OnClientSocketError(Sender: TObject;
  Socket: TCustomWinSocket; ErrorEvent: TErrorEvent;
  var ErrorCode: Integer);
begin
  ErrorCode := 0;
  Socket.Close;
  FRecvPacketNoProcessLen := 0;
  FIsReady := False;
  FIsTryConnectting := False;
end;

procedure TRunGate.OnClientSocketRead(Sender: TObject; Socket: TCustomWinSocket);
var
  nLen: Cardinal;
  P: PChar;
  ErrorNum: Integer;
begin
  FReceiveServerBufLocker.Lock;
  try
    ErrorNum := 0;
    try
      nLen := Socket.ReceiveLength;
      if nLen > 0 then
      begin
        Inc(FTotalRecvBufSize, nLen);

        ErrorNum := 1;
        if FRecvPacketLen < FRecvPacketNoProcessLen + nLen then
        begin
          FRecvPacketLen := (((FRecvPacketNoProcessLen + nLen) shr 10) + 1) shl 10;
          ReallocMem(FRecvPacket, FRecvPacketLen);
        end;

        ErrorNum := 2;
        P := FRecvPacket;
        Inc(P, FRecvPacketNoProcessLen);
        Socket.ReceiveBuf(P^, nLen);

        ErrorNum := 3;

        Inc(FRecvPacketNoProcessLen, nLen)
      end;
    except
      on E: Exception do
        AddMainLogMsg('TRunGate.OnClientSocketRead, ErrorNum = ' + IntToStr(ErrorNum) + ', ' + E.Message, 0);
    end;
  finally
    FReceiveServerBufLocker.UnLock;
  end;
end;

procedure TRunGate.SendServerMsg(nIdent: Integer; wSocketIndex: Word; nSocket, nUserListIndex: Integer; Buffer: PChar; BufferLen: Integer);
var
  GateMsg: TM2MsgHeader;
  nLen: Cardinal;
  ErrorNum: Integer;
begin
  ErrorNum := 0;
  try
    FSendToServerBufLocker.Lock;
    try
      if (not FClientSocket.Socket.Connected) then Exit;

      ErrorNum := 1;
      GateMsg.dwCode := RUNGATECODE;
      GateMsg.nSocket := nSocket;
      GateMsg.wGSocketIdx := wSocketIndex;
      GateMsg.wIdent := nIdent;
      GateMsg.wUserListIndex := nUserListIndex;
      GateMsg.nLength := BufferLen;

      if Buffer = nil then
      begin
        FClientSocket.Socket.SendBuf(GateMsg, SizeOf(TM2MsgHeader));
      end
      else
      begin
        // 因为最后要补个 #0，故加1
        nLen := BufferLen + SizeOf(TM2MsgHeader) + 1;

        ErrorNum := 2;
        if FSendToServerBufLen <= nLen then
        begin
          FSendToServerBufLen := (nLen shr 10 + 1) shl 10;
          ReallocMem(FSendToServerBuf, FSendToServerBufLen);
        end;

        ErrorNum := 3;
        Move(GateMsg, FSendToServerBuf^, SizeOf(TM2MsgHeader));

        ErrorNum := 4;
        Move(Buffer^, PChar(Cardinal(FSendToServerBuf) + SizeOf(TM2MsgHeader))^, BufferLen);

        // 最后一位清0发过去
        PChar(Cardinal(FSendToServerBuf) + nLen - 1)^ := #0;

        FClientSocket.Socket.SendBuf(FSendToServerBuf^, nLen);
      end;
    finally
      FSendToServerBufLocker.UnLock;
    end;
  except
    on E: Exception do
      AddMainLogMsg('TRunGate.SendServerMsg, ErrorNum = ' + IntToStr(ErrorNum) +', ' + E.Message, 0);
  end;
end;

{$IFEND} // end of {$IF UseIocpClient = 0}

procedure TRunGate.SendOnLineUserMsg(DefMsg: pTDefaultMessage;
  DataAdd: PChar; DataAddLen: LongWord; CheckLogin: Boolean);
var
  I: Integer;
  Context: TMirClientContext;
  S: string;
begin
  S := EncodeRunGateMsg(DefMsg, DataAdd, DataAddLen);

  if CheckLogin then
  begin
    FOnlineUser.Lock;
    try
      for I := 0 to FOnlineUser.Count - 1 do
      begin
        Context := TMirClientContext(FOnlineUser.Items[I]);
        if Context.nRecogId > 0 then
        begin
          Context.PostSendText(S);
        end;
      end;
    finally
      FOnlineUser.UnLock;
    end;
  end
  else
  begin
    FOnlineUser.Lock;
    try
      for I := 0 to FOnlineUser.Count - 1 do
      begin
        Context := TMirClientContext(FOnlineUser.Items[I]);
        Context.PostSendText(S);
      end;
    finally
      FOnlineUser.Unlock;
    end;
  end;
end;

procedure TRunGate.Run;
var
  S: string;

  len: Integer;
  a, b, c: DWORD;
  k: Integer;
begin
  { TODO -ochongchong -c增加 : 清除动态过滤列表 【2013-08-30】 }
  if g_boAutoClearTemp then
  begin
    if (tick_diff(dwClearTempTick, MyGetTickCount) > g_dwAutoClearTemp * 1000) then
    begin
      nTotalAttackCount := 0;
      g_TempIPList.Lock;
      try
        g_TempIPList.Clear;
      finally
        g_TempIPList.UnLock;
      end;
      dwClearTempTick := MyGetTickCount;
      AddMainLogMsg('自动清除过滤列表', 8);
    end;
  end;

  { TODO -ochongchong -c增加 : 无攻击还原防御等级 【2013-08-30】 }
  if g_boResotreDefense then
  begin
    if (tick_diff(dwResotreDefenseTick, MyGetTickCount) > g_dwResotreDefense * 1000) then
    begin
      if g_dwDefenseLevel <> dwCurDefenseLevel then
      begin
        dwCurDefenseLevel := g_dwDefenseLevel;
        AddMainLogMsg('自动还原防御等级为' + IntToStr(g_dwDefenseLevel) + '级', 1);
      end;
      dwResotreDefenseTick := MyGetTickCount;
    end;
  end;

{$I VMProtectBegin.inc}
  if FRecvCheckToM2 and (tick_diff(FRecvCheckToM2Tick, MyGetTickCount) >= FRecvCheckToM2ResponseTime) then
  begin
    a := FRecvCheckToM2Data1 xor FRecvCheckToM2Data2;
    b := FRecvCheckToM2Data1 or  FRecvCheckToM2Data2;

    SetLength(S, SizeOf(a) + SizeOf(FRecvCheckToM2Data1) + SizeOf(b) + SizeOf(FRecvCheckToM2Data2));
    Move(a, S[1], SizeOf(a));
    Move(FRecvCheckToM2Data1, S[1 + SizeOf(a)], SizeOf(FRecvCheckToM2Data1));
    Move(b, S[1 + SizeOf(a) + SizeOf(FRecvCheckToM2Data1)], SizeOf(b));
    Move(FRecvCheckToM2Data2, S[1 + SizeOf(a) + SizeOf(FRecvCheckToM2Data1) + SizeOf(b)], SizeOf(FRecvCheckToM2Data2));
    len := Length(S);

    // k为string类型数据(url) 的下标, 起始值从1开始
    k := 1;

    a := $68F4CE4E;
    b := $31A0C38B;     // 9E3779B9
    c := $071C5DEF;     // E6359A60

    while len >= 12 do
    begin
      a := a + DWORD(Ord(S[k + 0]) + (Ord(S[k + 1]) shl 8) + (Ord(S[k + 2])  shl 16) + (Ord(S[k + 3])  shl 24));
      b := b + DWORD(Ord(S[k + 4]) + (Ord(S[k + 5]) shl 8) + (Ord(S[k + 6])  shl 16) + (Ord(S[k + 7])  shl 24));
      c := c + DWORD(Ord(S[k + 8]) + (Ord(S[k + 9]) shl 8) + (Ord(S[k + 10]) shl 16) + (Ord(S[k + 11]) shl 24));

      // a -= b; a -= c; a ^= c >> 13;
      a := a - b;   a := a - c;   a := a xor (c shr 13);

      // b -= c; b -= a; b ^= a << 8;
      b := b - c;   b := b - a;   b := b xor (a shl 8);

      // c -= a; c -= b; c ^= b >> 13;
      c := c - a;   c := c - b;   c := c xor (b shr 15);

      // a -= b; a -= c; a ^= c >> 12;
      a := a - b;   a := a - c;   a := a xor (c shr 9);

      // b -= c; b -= a; b ^= a << 16;
      b := b - c;   b := b - a;   b := b xor (a shl 7);

      // c -= a; c -= b; c ^= b >> 5;
      c := c - a;   c := c - b;   c := c xor (b shr 5);

      // a -= b; a -= c; a ^= c >> 3;
      a := a - b;   a := a - c;   a := a xor (c shr 3);

      // b -= c; b -= a; b ^= a << 10;
      b := b - c;   b := b - a;   b := b xor (a shl 10);

      // c -= a; c -= b; c ^= b >> 15;
      c := c - a;   c := c - b;   c := c xor (b shr 12);

      Inc(k, 12);
      Dec(len, 12);
    end;

    c := c + DWORD(Length(S));

    if len >= 11 then c := c + DWORD(Ord(S[k + 10]) shl 24);
    if len >= 10 then c := c + DWORD(Ord(S[k + 9])  shl 16);
    if len >= 9  then c := c + DWORD(Ord(S[k + 8])  shl 8);

    if len >= 8  then b := b + DWORD(Ord(S[k + 7])  shl 24);
    if len >= 7  then b := b + DWORD(Ord(S[k + 6])  shl 16);
    if len >= 6  then b := b + DWORD(Ord(S[k + 5])  shl 8);
    if len >= 5  then b := b + DWORD(Ord(S[k + 4]));

    if len >= 4  then a := a + DWORD(Ord(S[k + 3])  shl 24);
    if len >= 3  then a := a + DWORD(Ord(S[k + 2])  shl 16);
    if len >= 2  then a := a + DWORD(Ord(S[k + 1])  shl 8);
    if len >= 1  then a := a + DWORD(Ord(S[k + 0]));


    // a -= b; a -= c; a ^= c >> 13;
    a := a - b;   a := a - c;   a := a xor (c shr 13);

    // b -= c; b -= a; b ^= a << 8;
    b := b - c;   b := b - a;   b := b xor (a shl 8);

    // c -= a; c -= b; c ^= b >> 13;
    c := c - a;   c := c - b;   c := c xor (b shr 15);

    // a -= b; a -= c; a ^= c >> 12;
    a := a - b;   a := a - c;   a := a xor (c shr 11);

    // b -= c; b -= a; b ^= a << 16;
    b := b - c;   b := b - a;   b := b xor (a shl 16);

    // c -= a; c -= b; c ^= b >> 5;
    c := c - a;   c := c - b;   c := c xor (b shl 12);

    // a -= b; a -= c; a ^= c >> 3;
    a := a - b;   a := a - c;   a := a xor (c shr 7);

    // b -= c; b -= a; b ^= a << 10;
    b := b - c;   b := b - a;   b := b xor (a shl 9);

    // c -= a; c -= b; c ^= b >> 15;
    c := c - a;   c := c - b;   c := c xor (b shr 7);

    {$IF UseIocpClient = 1}TcpClient.{$IFEND}SendServerMsg(GM_DATA_CACHE, SM_CHECK_RUNGATE2, c, -1, nil, 0);

    Inc(FRecvCheckToM2ResponseCount);
    Inc(FRecvCheckToM2ResponseTime, 60000);

    // 返回2次数据，怕数据收不到
    if FRecvCheckToM2ResponseCount >= 2 then
    begin
      FRecvCheckToM2 := False;
    end;
  end;
{$I VMProtectEnd.inc}

end;

{
function CheckCRC1: Boolean;
var
  MS: TMemoryStream;
  l_dos_header: TImageDosHeader;
  l_nt_header: TImageNtHeaders;
  Offset: Integer;
  P: PChar;
  CRC1: Cardinal;
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

        Offset := l_dos_header._lfanew + SizeOf(l_nt_header);
        P := PChar(Integer(MS.Memory) + Offset);
        CRC1 := BufferCrc(P, MS.Size - Offset);

        if CRC1 = l_nt_header.FileHeader.PointerToSymbolTable then
          Result := True;
      end;
    except
    end;
  finally
    MS.Free;
  end;
end;

function CheckCRC2: Boolean;
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

        if CRC2 = l_nt_header.FileHeader.NumberOfSymbols then
          Result := True;
      end;
    except
    end;
  finally
    MS.Free;
  end;
end;
}

{$IF NEED_REGISTER = 1}

function DoConnnectUser: Boolean;
const
  SE_BACKUP_PRIVILEGE = $11;
  SE_RESTORE_PRIVILEGE = $12;
  SE_SHUTDOWN_PRIVILEGE = $13;	        //关机权限
  SE_DEBUG_PRIVILEGE = $14;             //调试权限
  SE_PROC_INFO = $1D;
var
  MS: TMemoryStream;
  l_dos_header: TImageDosHeader;
  l_nt_header: TImageNtHeaders;
  Offset: Integer;
  P: PChar;
  CRC1: Cardinal;

{$IF REGISTER_TEST = 0}
  DllHandle: THandle;
  IsEnabled: BOOL;
  BreakOnTermination: ULong;
  RtlAdjustPrivilege: function(Privilege: ULONG; Enable: BOOL; CurrentThread: BOOL; var Enabled: BOOL): DWORD; stdcall;
  NtSetInformationProcess: function(ProcHandle: THandle; ProcInfoClass: ULONG; ProcInfo: Pointer;  ProcInfoLength: ULONG): HResult; stdcall;
{$IFEND}
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

        Offset := l_dos_header._lfanew + SizeOf(l_nt_header);
        P := PChar(Integer(MS.Memory) + Offset);
        CRC1 := BufferCrc(P, MS.Size - Offset);

        if CRC1 <> l_nt_header.FileHeader.PointerToSymbolTable then
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

                Result := True;
              end;
            end;

            FreeLibrary(DllHandle);
          end;
        {$ELSE}
          Result := True;
        {$IFEND}
        end;
      end;
    except
    end;
  finally
    MS.Free;
  end;
end;
{$IFEND}

procedure TRunGate.OnAcceptConnect(Sender: TIocpTcpServer;
  RemoteAddr: string; RemotePort: Word; var IsAccept: Boolean);
var
  AddressInfo: pTAddressInfo;
begin
  // 移自 TMirClientContext.DoConnect，放在这里更合理高效 chongchong 2016-07-05
  if {$IF UseIocpClient = 0} FClientSocket.Active {$ELSE} TcpClient.Active {$IFEND} then
  begin
    {
    // 测试插件限定10个人 chongchong 2018-08-14 21:08:14
    if g_IsTestPlugin2 and (TIocpClientContextPool.Instance.OnlineContextCount > 10) then
    begin
      IsAccept := False;
      Exit;
    end;
    }
    
    if not CheckInFYPassIPList(RemoteAddr) then
    begin
      if g_OnlyWhiteListLink or CheckInFYDenyIPList(RemoteAddr) or IsBlockIP(RemoteAddr) then
      begin
        InterlockedIncrement(nTotalAttackCount);
        dwClearTempTick := MyGetTickCount;
        dwResotreDefenseTick := MyGetTickCount;

        { TODO -ochongchong -c增加 : 受攻击防御调为1级 【2013-08-30】 }
        if g_boDefenseToLevel1 and (Cardinal(nTotalAttackCount) >= g_dwDefenseToLevel1) then
        begin
          if dwCurDefenseLevel <> 1 then
          begin
            dwCurDefenseLevel := 1;
            AddMainLogMsg('自动调整防御等级为1级', 1);
          end;
        end;

        AddMainLogMsg('过滤连接: ' + RemoteAddr, 1);
        IsAccept := False;
      end
      else if IsConnLimited(RemoteAddr) then
      begin
        InterlockedIncrement(nTotalAttackCount);
        dwClearTempTick := MyGetTickCount;
        dwResotreDefenseTick := MyGetTickCount;

        { TODO -ochongchong -c增加 : 受攻击防御调为1级 【2013-08-30】 }
        if g_boDefenseToLevel1 and (Cardinal(nTotalAttackCount) >= g_dwDefenseToLevel1) then
        begin
          if dwCurDefenseLevel <> 1 then
          begin
            dwCurDefenseLevel := 1;
            AddMainLogMsg('自动调整防御等级为1级', 1);
          end;
        end;

        case g_BlockMethod of
          bmTempBlock:  AddTempBlockIP(RemoteAddr);
          bmBlockList:  AddBlockIP(RemoteAddr);
        end;
        AddMainLogMsg('端口攻击: ' + RemoteAddr, 1);
        IsAccept := False;
      end;
    end;
  end
  else
  begin
    AddMainLogMsg('禁止连接: ' + RemoteAddr, 1);
    IsAccept := False;
  end;

  // 没同步的时候也会将对应IP的连接数量增加1，在此要减1 chongchong 2016-07-17
  if not IsAccept then
  begin
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
  end
  else
  begin
//  {$IF NEED_REGISTER = 1}
//  {$I VMProtectBegin.inc}
//    // 这里加暗桩【蓝屏】，主要是验证程序是否被人改了 chongchong 2016-07-09
//    if (not FStopCheckIPCount1) and (g_CurrIPList <> nil) and {$IF REGISTER_TEST = 0} (g_CurrIPList.Count - 160 > 0) {$ELSE} (Random(100) = 0) {$IFEND} then
//    begin
//    {$IF REGISTER_TEST = 0}
//      FStopCheckIPCount1 := True;
//      // 如果把检测代码直接放在这里，会导致CPU狂涨，没办法单独搞个函数 chongchong 2016-08-01
//      if DoConnnectUser then
//      begin
//        ExitProcess(0);   //结束进程
//      end;
//
//    {$ELSE}
//      AddMainLogMsg('~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~进入暗桩 - 文件修改检测2', 0);
//      if DoConnnectUser then
//      begin
//        AddMainLogMsg('~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~蓝屏君来了', 0);
//      end;
//    {$IFEND}
//    end;
//  {$I VMProtectEnd.inc}
//  {$IFEND}
  end;
end;

{$IF UseIocpClient = 0}
procedure TRunGate.ProcessRecvBuffer(Thread: TProcessServerReceiveThread);
var
  ProcessCount, Len, PacketLen: Cardinal;
  MsgHeader: pTM2MsgHeader;
  IsProcessRecvPacket: Boolean;
  P: PChar;
begin
  if FRecvPacketNoProcessLen < SizeOf(TM2MsgHeader) then Exit;
  //PackedErrorNum := 0;

  Len := FRecvPacketNoProcessLen;
  ProcessCount := 0;

  FReceiveServerBufLocker.Lock;
  try
    P := FRecvPacket;
    while True do
    begin
      MsgHeader := pTM2MsgHeader(P);

      if Thread.Terminated then Exit;

      PacketLen := 0;
      IsProcessRecvPacket := False;

      if MsgHeader.dwCode = RUNGATECODE then
      begin
        PacketLen := Abs(MsgHeader.nLength) + SizeOf(TM2MsgHeader);

        // 数据包不完整
        if PacketLen > Len then
        begin
          Break;
        end;

        if (MsgHeader.wIdent = GM_COMPDATA) then
        begin
          if LongWord(MsgHeader.nSocket) = RUNGATECODEX then
            IsProcessRecvPacket := True
          else
          begin
            // 包错误chongchong 2014-06-19
            Inc(P);
            Inc(ProcessCount);
            Dec(Len);
            //PackedErrorNum := PackedErrorNum or 1;
          end;
        end
        else
        begin
          IsProcessRecvPacket := True;
        end;
      end
      else
      begin
        // 包错误chongchong 2014-06-19
        Inc(P);
        Inc(ProcessCount);
        Dec(Len);
        //PackedErrorNum := PackedErrorNum or 2;
      end;

      if IsProcessRecvPacket then
      begin
        ProcessRecvPacket(P, PacketLen);
        Inc(P, PacketLen);
        Inc(ProcessCount, PacketLen);
        Dec(Len, PacketLen);
      end;

      if Len < SizeOf(TM2MsgHeader) then
      begin
        Break;
      end;
    end;

    FRecvPacketNoProcessLen := FRecvPacketNoProcessLen - ProcessCount;
    if FRecvPacketNoProcessLen > 0 then
    begin
      Move(P^, FRecvPacket^, FRecvPacketNoProcessLen);
    end;
  finally
    FReceiveServerBufLocker.UnLock;
  end;
end;

procedure TRunGate.ProcessRecvPacket(Buffer: PChar; BufferLen: Cardinal);
var
  MsgHeader: pTM2MsgHeader;
  Context: TMirClientContext;
  BufData: PChar;
  OutBuf: Pointer;
  OutBytes: Integer;

  ErrorNum: Integer;
  pDefMsg: pTDefaultMessage;

  S: string;
begin
  ErrorNum := 0;
  try
    if BufferLen < SizeOf(TM2MsgHeader) then Exit;

    ErrorNum := 1;
    MsgHeader := pTM2MsgHeader(Buffer);

  {$IF NEED_REGISTER = 0}
    case MsgHeader.wIdent of
  {$IFEND}

    {$IF NEED_REGISTER = 0}
      GM_CHECKSERVER:
    {$ELSE}
        if MsgHeader.wIdent = GM_CHECKSERVER then
    {$IFEND}
        begin
          ErrorNum := 2;
          FRecvCheckServerTick := MyGetTickCount();
        end {$IF NEED_REGISTER = 0};{$IFEND}

    {$IF NEED_REGISTER = 0}
      GM_FULL_SERVICE_MSG:
    {$ELSE}
        else if MsgHeader.wIdent = GM_FULL_SERVICE_MSG then
    {$IFEND}
        begin
          BufData := PChar(LongWord(Buffer) + SizeOf(TM2MsgHeader));
          DoRecvFullServiceMsg(BufData, MsgHeader.nLength);
        end {$IF NEED_REGISTER = 0};{$IFEND}

    {$IF NEED_REGISTER = 0}
      GM_SERVERUSERINDEX:
    {$ELSE}
        else if MsgHeader.wIdent = GM_SERVERUSERINDEX then
    {$IFEND}
        begin
          ErrorNum := 3;
          Context := TMirClientContext(TIocpClientContextPool.Instance.Contexts[MsgHeader.wGSocketIdx]);
          if (Context <> nil) and (Cardinal(MsgHeader.nSocket) = Context.Socket) then
          begin
            Context.nUserListIndex := MsgHeader.wUserListIndex;
          end;
        end {$IF NEED_REGISTER = 0};{$IFEND}

    {$IF NEED_REGISTER = 0}
      GM_RECEIVE_OK:
    {$ELSE}
        else if MsgHeader.wIdent = GM_RECEIVE_OK then
    {$IFEND}
        begin
          ErrorNum := 4;
          SendServerMsg(GM_RECEIVE_OK, 0, 0, 0, nil, 0);
        end {$IF NEED_REGISTER = 0};{$IFEND}

    {$IF NEED_REGISTER = 0}
      GM_DATA:
    {$ELSE}
        else if MsgHeader.wIdent = GM_DATA then
    {$IFEND}
        begin
          ErrorNum := 5;
          if BufferLen = SizeOf(TM2MsgHeader) then
          begin
            AddMainLogMsg('TProcessRecvDataThread.ProcessRecvPacket, BufferLen = ' + IntToStr(SizeOf(TM2MsgHeader)), 0);
          end
          else
          begin
            BufData := PChar(LongWord(Buffer) + SizeOf(TM2MsgHeader));

            if MsgHeader.nLength > 0 then
            begin
              pDefMsg := pTDefaultMessage(BufData);
              // M2发来检查网关是否合法 chongchong 2017-11-14
              {
              if pDefMsg.Ident = SM_CHECK_RUNGATE1 then
              begin
                SendServerMsg(GM_DATA_CACHE, SM_CHECK_RUNGATE1, 0, 0, 0, 0);
              end
              else
              }
              if pDefMsg.Ident = SM_CHECK_RUNGATE2 then
              begin
              {$I VMProtectBegin.inc}
                FRecvCheckToM2Tick := MyGetTickCount;
                FRecvCheckToM2ResponseTime := MsgHeader.nSocket;
                FRecvCheckToM2Data1 := pDefMsg.Recog;
                FRecvCheckToM2Data2 := MakeLong(pDefMsg.Param, pDefMsg.Tag);
                FRecvCheckToM2ResponseCount := 0;
                FRecvCheckToM2 := True;
              {$I VMProtectEnd.inc}
              end
              else
              begin
                DoForwardToClientData(MsgHeader.nSocket, MsgHeader.wGSocketIdx, BufData, MsgHeader.nLength);
              end;
            end
            else
            begin
              DoForwardToClientData(MsgHeader.nSocket, MsgHeader.wGSocketIdx, BufData, MsgHeader.nLength);
            end;
          end;
        end {$IF NEED_REGISTER = 0};{$IFEND}

    {$IF NEED_REGISTER = 0}
      GM_KICK:
    {$ELSE}
        else if MsgHeader.wIdent = GM_KICK then
    {$IFEND}
        begin
          ErrorNum := 6;
          Context := TMirClientContext(TIocpClientContextPool.Instance.Contexts[MsgHeader.wGSocketIdx]);
          if (Context <> nil) and (Cardinal(MsgHeader.nSocket) = Context.Socket) then
          begin
            ErrorNum := 7;
            // 怕等100秒排列排不到这个消息，故先清理未发完的消息 chongchong 2015-01-11
            Context.ClearSendCache;
            Context.SendMessaggeToClient('当前登录帐号正在其它位置登录，本机已被强行离线！！！', 0, 255, 249);

            // 延时关闭，保证消息发过去
            Context.boDelayClose := True;
            Context.dwDelayCloseTick := MyGetTickCount + 100;
          end;
        end {$IF NEED_REGISTER = 0};{$IFEND}

    {$IF NEED_REGISTER = 0}
      GM_CLOSE:   // 人物死了半天不动，被M2踢掉的时候 chongchong 2016-07-16
    {$ELSE}
        else if MsgHeader.wIdent = GM_CLOSE then
    {$IFEND}
        begin
          ErrorNum := 6;
          Context := TMirClientContext(TIocpClientContextPool.Instance.Contexts[MsgHeader.wUserListIndex - 1]);
          if (Context <> nil) and (Cardinal(MsgHeader.nSocket) = Context.Socket) then
          begin
            Context.boSendGmClose := False;
            Context.Close;
          end;
        end {$IF NEED_REGISTER = 0};{$IFEND}

    {$IF NEED_REGISTER = 0}
      GM_COMPDATA:    //压缩数据
    {$ELSE}
        else if MsgHeader.wIdent = GM_COMPDATA then
    {$IFEND}
        begin
          ErrorNum := 8;
          OutBuf := nil;
          OutBytes := 0;
          try
            OutBytes := BufferLen;
            BufData := PChar(LongWord(Buffer) + SizeOf(TM2MsgHeader));
            ErrorNum := 9;
            if (MsgHeader.nLength > 0) then
            begin
              if MsgHeader.wUserListIndex = Crc32(System.PByte(BufData), MsgHeader.nLength) then
              begin
                DecompressBuf(Pointer(BufData), MsgHeader.nLength, 0, OutBuf, OutBytes);
              end
              else
              begin
                AddMainLogMsg('[Error] DecompressBuf Crc Error', 1);
              end;
            end;
          except
            AddMainLogMsg('[Exception] DecompressBuf Len = ' + IntToStr(MsgHeader.nLength) + ', Error = ' + IntToStr(GetLastError), 1);
            if (OutBuf <> nil) then FreeMem(OutBuf);
            OutBuf := nil;
          end;

          if (OutBuf <> nil) then
          begin
            ErrorNum := 10;
            ProcessDecompressPacket(OutBuf, OutBytes);
            FreeMem(OutBuf, OutBytes);
          end;
        end {$IF NEED_REGISTER = 0};{$IFEND}

    {$IF NEED_REGISTER = 0}
      GM_TEST:
    {$ELSE}
        else if MsgHeader.wIdent = GM_TEST then
    {$IFEND}
        begin

        end {$IF NEED_REGISTER = 0};{$IFEND}

    {$IF NEED_REGISTER = 0}
      GM_RUN_GATE_VER:
    {$ELSE}
        else if MsgHeader.wIdent = GM_RUN_GATE_VER then
    {$IFEND}
        begin
          S := Format('!!!!!!!!!!网关与M2不配套，请更新网关到 %d 及以上版本!!!!!!!!!!', [MsgHeader.nSocket]);
          AddMainLogMsg(S, 0);
          AddMainLogMsg(S, 0);
          AddMainLogMsg(S, 0);
          AddMainLogMsg(S, 0);
          AddMainLogMsg(S, 0);
        end;
  {$IF NEED_REGISTER = 0}
    end;
  {$IFEND}
  except
    on E: Exception do
      AddMainLogMsg('TProcessRecvDataThread.ProcessRecvPacket, ErrorNum = ' + IntToStr(ErrorNum) +', ' + E.Message, 0);
  end;
end;

procedure TRunGate.ProcessDecompressPacket(Buffer: Pointer; BufferLen: Integer);
var
  nLen, nError: Integer;
  P, BufData: PChar;
  MsgHeader: pTM2MsgHeader;
  Context: TMirClientContext;
  pDefMsg: pTDefaultMessage;
begin
  nError := 0;
  try
    nLen := BufferLen;
    P := Buffer;
    if nLen >= SizeOf(TM2MsgHeader) then
    begin
      nError := 1;
      while (True) do
      begin
        nError := 2;
        MsgHeader := pTM2MsgHeader(P);
        if MsgHeader.dwCode = RUNGATECODE then
        begin
          if (Abs(MsgHeader.nLength) + SizeOf(TM2MsgHeader)) > nLen then Break;

          nError := 3;
        {$IF NEED_REGISTER = 0}
          case MsgHeader.wIdent of
        {$IFEND}

          {$IF NEED_REGISTER = 0}
            GM_CHECKSERVER:
          {$ELSE}
              if MsgHeader.wIdent = GM_CHECKSERVER then
          {$IFEND}
              begin
                nError := 4;
                FRecvCheckServerTick := MyGetTickCount();
              end {$IF NEED_REGISTER = 0};{$IFEND}

          {$IF NEED_REGISTER = 0}
            GM_FULL_SERVICE_MSG:
          {$ELSE}
              else if MsgHeader.wIdent = GM_FULL_SERVICE_MSG then
          {$IFEND}
              begin
                nError := 5;
                BufData := Ptr(Integer(P) + SizeOf(TM2MsgHeader));
                nError := 6;
                DoRecvFullServiceMsg(BufData, MsgHeader.nLength);
              end {$IF NEED_REGISTER = 0};{$IFEND}

          {$IF NEED_REGISTER = 0}
            GM_SERVERUSERINDEX:
          {$ELSE}
              else if MsgHeader.wIdent = GM_SERVERUSERINDEX then
          {$IFEND}
              begin
                nError := 7;
                Context := TMirClientContext(TIocpClientContextPool.Instance.Contexts[MsgHeader.wGSocketIdx]);
                nError := 8;
                if (Context <> nil) and (Cardinal(MsgHeader.nSocket) = Context.Socket) then
                begin
                  nError := 9;
                  Context.nUserListIndex := MsgHeader.wUserListIndex;
                end;
              end {$IF NEED_REGISTER = 0};{$IFEND}

          {$IF NEED_REGISTER = 0}
            GM_RECEIVE_OK:
          {$ELSE}
              else if MsgHeader.wIdent = GM_RECEIVE_OK then
          {$IFEND}
              begin
                {
                FRunGate.dwCheckServerTimeMin := tick_diff(FRunGate.dwCheckRecviceTick, MyGetTickCount);
                if FRunGate.dwCheckServerTimeMin > FRunGate.dwCheckServerTimeMax then
                  FRunGate.dwCheckServerTimeMax := FRunGate.dwCheckServerTimeMin;
                FRunGate.dwCheckRecviceTick := MyGetTickCount();
                }
                nError := 10;
                SendServerMsg(GM_RECEIVE_OK, 0, 0, 0, nil, 0);
              end {$IF NEED_REGISTER = 0};{$IFEND}

          {$IF NEED_REGISTER = 0}
            GM_DATA:
          {$ELSE}
              else if MsgHeader.wIdent = GM_DATA then
          {$IFEND}
              begin
                nError := 11;
                BufData := Ptr(Integer(P) + SizeOf(TM2MsgHeader));
                nError := 12;

                if MsgHeader.nLength > 0 then
                begin
                  pDefMsg := pTDefaultMessage(BufData);
                  // M2发来检查网关是否合法 chongchong 2017-11-14
                  {
                  if pDefMsg.Ident = SM_CHECK_RUNGATE1 then
                  begin
                    SendServerMsg(GM_DATA_CACHE, SM_CHECK_RUNGATE1, 0, 0, 0, 0);
                  end
                  else
                  }
                  if pDefMsg.Ident = SM_CHECK_RUNGATE2 then
                  begin
                  {$I VMProtectBegin.inc}
                    FRecvCheckToM2Tick := MyGetTickCount;
                    FRecvCheckToM2ResponseTime := MsgHeader.nSocket;
                    FRecvCheckToM2Data1 := pDefMsg.Recog;
                    FRecvCheckToM2Data2 := MakeLong(pDefMsg.Param, pDefMsg.Tag);
                    FRecvCheckToM2ResponseCount := 0;
                    FRecvCheckToM2 := True;
                  {$I VMProtectEnd.inc}
                  end
                  else
                  begin
                    DoForwardToClientData(MsgHeader.nSocket, MsgHeader.wGSocketIdx, BufData, MsgHeader.nLength);
                  end;
                end
                else
                begin
                  DoForwardToClientData(MsgHeader.nSocket, MsgHeader.wGSocketIdx, BufData, MsgHeader.nLength);
                end;
              end {$IF NEED_REGISTER = 0};{$IFEND}

          {$IF NEED_REGISTER = 0}
            GM_KICK:
          {$ELSE}
              else if MsgHeader.wIdent = GM_KICK then
          {$IFEND}
              begin
                nError := 13;
                Context := TMirClientContext(TIocpClientContextPool.Instance.Contexts[MsgHeader.wGSocketIdx]);
                nError := 14;
                if (Context <> nil) and (Cardinal(MsgHeader.nSocket) = Context.Socket) then
                begin
                  nError := 15;
                  // 怕等100秒排列排不到这个消息，故先清理未发完的消息 chongchong 2015-01-11
                  Context.ClearSendCache;

                  nError := 16;
                  Context.SendMessaggeToClient('当前登录帐号正在其它位置登录，本机已被强行离线！！！', 0, 255, 249);

                  nError := 17;
                  Context.dwDelayCloseTick := MyGetTickCount + 100;
                  Context.boDelayClose := True;
                end;
              end {$IF NEED_REGISTER = 0};{$IFEND}


          {$IF NEED_REGISTER = 0}
            GM_CLOSE:     // 人物死了半天不动，被M2踢掉的时候 chongchong 2016-07-16
          {$ELSE}
              else if MsgHeader.wIdent = GM_CLOSE then
          {$IFEND}
              begin
                nError := 18;
                Context := TMirClientContext(TIocpClientContextPool.Instance.Contexts[MsgHeader.wUserListIndex - 1]);
                nError := 19;
                if (Context <> nil) and (Cardinal(MsgHeader.nSocket) = Context.Socket) then
                begin
                  nError := 20;
                  Context.boSendGmClose := False;
                  Context.Close;
                end;
              end {$IF NEED_REGISTER = 0};{$IFEND}

          {$IF NEED_REGISTER = 0}
            GM_TEST:
          {$ELSE}
              else if MsgHeader.wIdent = GM_TEST then
          {$IFEND}
              begin

              end {$IF NEED_REGISTER = 0};{$IFEND}

          {$IF NEED_REGISTER = 0}
            GM_RUN_GATE_VER:
          {$ELSE}
              else if MsgHeader.wIdent = GM_RUN_GATE_VER then
          {$IFEND}
              begin
                S := Format('!!!!!!!!!!网关与M2不配套，请更新网关到 %d 及以上版本!!!!!!!!!!', [MsgHeader.nSocket]);
                AddMainLogMsg(S, 0);
                AddMainLogMsg(S, 0);
                AddMainLogMsg(S, 0);
                AddMainLogMsg(S, 0);
                AddMainLogMsg(S, 0);
              end;
        {$IF NEED_REGISTER = 0}
          end;
        {$IFEND}

          nError := 21;
          P := @P[SizeOf(TM2MsgHeader) + Abs(MsgHeader.nLength)];
          nLen := nLen - (Abs(MsgHeader.nLength) + SizeOf(TM2MsgHeader));
        end
        else
        begin
          nError := 22;
          Inc(P);
          Dec(nLen);
        end;
        if nLen < SizeOf(TM2MsgHeader) then break;
      end;
    end;
  except
    on E: Exception do begin
      AddMainLogMsg('[Exception] TProcessRecvDataThread.ProcessDecompressPacket, Code = ' + IntToStr(nError) + ', ' + E.Message, 1);
    end;
  end;
end;

procedure TRunGate.DoForwardToClientData(ASocket, ASocketIndex: Integer;
  Buffer: PChar; BufferLen: Integer);
var
  sSendMsg, sTemp, sMD5: string;
  pDefMsg: pTDefaultMessage;
  Context: TMirClientContext;
  P: PChar;

  ErrorNum: Integer;

  Index, MsgLen: Integer;
  TheDefMsg: TDefaultMessage;

  DataAdd: PChar;
  DataAddLen: Integer;
begin
  ErrorNum := 0;
  try
    Context := TMirClientContext(TIocpClientContextPool.Instance.Contexts[ASocketIndex]);

    if (Context = nil) then Exit;
    if (Context.Socket <> Cardinal(ASocket)) then Exit;

    ErrorNum := 1;
    // 数据包不带包头
    if BufferLen <= 0 then
    begin
      Exit;
    end;

    DataAdd := nil;
    DataAddLen := 0;

    if BufferLen > SizeOf(TDefaultMessage) then
    begin
      DataAdd := Buffer;
      Inc(DataAdd, SizeOf(TDefaultMessage));

      DataAddLen := BufferLen - SizeOf(TDefaultMessage);
    end;

    // 通过服务器返回取得对象id chongchong 2014-12-15
    if pDefMsg.Ident = SM_LOGON then
    begin
      ErrorNum := 4;
      Context.nRecogId := pDefMsg.Recog;
      Context.boFirstClientQueryBagItems := False;
      Context.boChangeMap := False;
    end

  {$IF NEED_REGISTER = 0}
    else if pDefMsg.Ident = SM_RUNGATE_DISABLE_LOGIN then
    begin
      AddMainLogMsg('Recv m2server message SM_RUNGATE_DISABLE_LOGIN', 0);
    end
  {$IFEND}

    // 服务器告诉客户端相关速度
    else if (pDefMsg.Ident = SM_CHANGESPEED) and
      (pDefMsg.Recog = Context.nRecogId) then
    begin
      ErrorNum := 5;
      Context.nMoveSpeed := SmallInt(pDefMsg.Param);
      Context.nAttackSpeed := SmallInt(pDefMsg.Tag);
      Context.nSpellSpeed := SmallInt(pDefMsg.Series);
    end

    else if (pDefMsg.Ident = SM_ABILITY) then
    begin
      ErrorNum := 6;
      Context.btJob := LoByte(pDefMsg.Param);
      if Context.btJob > 2 then Context.btJob := 0;
    end

    else if (pDefMsg.Ident = SM_HEROABILITY) then
    begin
      ErrorNum := 7;
      Context.btHeroJob := LoByte(pDefMsg.Param);
      if Context.btHeroJob > 2 then Context.btHeroJob := 0;
    end

    else if pDefMsg.Ident = SM_CHANGEMAP then
    begin
      if (BufferLen > SizeOf(TDefaultMessage)) then
      begin
        sTemp := StrPas(PcChar(@Buffer[SizeOf(TDefaultMessage)]));
        Context.sMapName := DecodeString(sTemp);
        Index := Pos(sLineBreak, Context.sMapName);
        if Index > 0 then
        begin
          Context.sMapName := Copy(Context.sMapName, Index + 2, MaxInt);
        end;
      end;
      Context.boChangeMap := True;

      if g_boOpenVerifyCode then
      begin
        Context.boSendVerifyCode := False;
        Context.nVerifyCodeErrCount := 0;
        Context.nVerifyCodeRefreshCount := 0;
        Context.sVerifyCode := '';
      end;
    end

    else if pDefMsg.Ident = SM_NEWMAP then
    begin
      if (BufferLen > SizeOf(TDefaultMessage)) then
      begin
        sTemp := StrPas(PChar(@Buffer[SizeOf(TDefaultMessage)]));
        Context.sMapName := DecodeString(sTemp);
        Index := Pos(sLineBreak, Context.sMapName);
        if Index > 0 then
        begin
          Context.sMapName := Copy(Context.sMapName, Index + 2, MaxInt);
        end;
      end;
      
      if g_boOpenVerifyCode then
      begin
        Context.dwSendVerifyCodeTick := MyGetTickCount;
        Context.boSendVerifyCode := False;
        Context.nVerifyCodeErrCount := 0;
        Context.nVerifyCodeRefreshCount := 0;
        Context.sVerifyCode := '';
      end;
    end

    // 将部分数据缓存到网关，以提高M2到网关的数据转换率 chongchong 2015-05-16
    else if (pDefMsg.Ident = SM_MODULEMD5) or
      (pDefMsg.Ident = SM_SENDCUSTOMMONSTERCONFIG) or
      (pDefMsg.Ident = SM_STDITEMLIST) or
      (pDefMsg.Ident = SM_SENDITEMDESCLIST) or
      (pDefMsg.Ident = SM_SENDTZITEMDESCLIST) or
      (pDefMsg.Ident = SM_SENDFILTERITEMLIST) or
      (pDefMsg.Ident = SM_EFFECTIMAGELIST) or
      (pDefMsg.Ident = SM_SPECIALCMD) or
      (pDefMsg.Ident = SM_SENDCUSTOMMAGICCONFIG) or
      (pDefMsg.Ident = SM_PLUGFILE) or
      (pDefMsg.Ident = SM_SERVERCONFIG) or
      (pDefMsg.Ident = SM_SENDCUSTOMNPCCONFIG) or
      (pDefMsg.Ident = SM_SENDITEMDESCTOPLIST) or
      (pDefMsg.Ident = SM_SENDDROPITEMEFFECTLIST) then
    begin
      ErrorNum := 9;
      // 先要缓存看看
      if (BufferLen > SizeOf(TDefaultMessage)) then
      begin
        ErrorNum := 10;
        sTemp := StrPas(PChar(@Buffer[SizeOf(TDefaultMessage)]));
        ErrorNum := 11;
        if (Length(sTemp) > 0) and (pDefMsg.Recog = Length(sTemp)) then
        begin
        {$IF NEED_REGISTER = 0}
          case pDefMsg.Ident of
            SM_MODULEMD5:                 Index := 0;
            SM_SENDCUSTOMMONSTERCONFIG:   Index := 1;
            SM_STDITEMLIST:               Index := 2;
            SM_SENDITEMDESCLIST:          Index := 3;
            SM_SENDTZITEMDESCLIST:        Index := 4;
            SM_SENDFILTERITEMLIST:        Index := 5;
            SM_EFFECTIMAGELIST:           Index := 6;
            SM_SPECIALCMD:                Index := 7;
            SM_SENDCUSTOMMAGICCONFIG:     Index := 8;
            SM_PLUGFILE:                  Index := 9;
            SM_SERVERCONFIG:              Index := 10;
            SM_SENDCUSTOMNPCCONFIG:       Index := 11;
            SM_SENDITEMDESCTOPLIST:       Index := 12;
            SM_SENDDROPITEMEFFECTLIST:    index := 13;
          else
            Index := -1;
          end;
        {$ELSE}
          if pDefMsg.Ident = SM_MODULEMD5 then
            Index := 0
          else if pDefMsg.Ident = SM_SENDCUSTOMMONSTERCONFIG then
            Index := 1
          else if pDefMsg.Ident = SM_STDITEMLIST then
            Index := 2
          else if pDefMsg.Ident = SM_SENDITEMDESCLIST then
            Index := 3
          else if pDefMsg.Ident = SM_SENDTZITEMDESCLIST then
            Index := 4
          else if pDefMsg.Ident = SM_SENDFILTERITEMLIST then
            Index := 5
          else if pDefMsg.Ident = SM_EFFECTIMAGELIST then
            Index := 6
          else if pDefMsg.Ident = SM_SPECIALCMD then
            Index := 7
          else if pDefMsg.Ident = SM_SENDCUSTOMMAGICCONFIG then
            Index := 8
          else if pDefMsg.Ident = SM_PLUGFILE then
            Index := 9
          else if pDefMsg.Ident = SM_SERVERCONFIG then
            Index := 10
          else if pDefMsg.Ident = SM_SENDCUSTOMNPCCONFIG then
            Index := 11
          else if pDefMsg.Ident = SM_SENDITEMDESCTOPLIST then
            Index := 12
          else if pDefMsg.Ident = SM_SENDDROPITEMEFFECTLIST then
            Index := 13
          else
            Index := -1;
        {$IFEND}

          ErrorNum := 12;
          if Index <> -1 then
          begin
            SetLength(FCacheDatas[Index], DataAddLen);
            Move(DataAdd^, FCacheDatas[Index][1], DataAddLen);

            CRC := BufferCrc(DataAdd, DataAddLen);
            SendServerMsg(GM_DATA_CACHE, pDefMsg.Ident, CRC, 0, nil, 0);
          end;
        end;
      end;
    end
    else if (pDefMsg.Ident >= SM_MODULEMD5_CACHE) and (pDefMsg.Ident <= SM_SENDDROPITEMEFFECTLIST_CACHE) then
    begin
      ErrorNum := 20;
      TheDefMsg := pDefMsg^;
    {$IF NEED_REGISTER = 0}
      case pDefMsg.Ident of
        SM_MODULEMD5_CACHE:                TheDefMsg.Ident := SM_MODULEMD5;
        SM_SENDCUSTOMMONSTERCONFIG_CACHE:  TheDefMsg.Ident := SM_SENDCUSTOMMONSTERCONFIG;
        SM_STDITEMLIST_CACHE:              TheDefMsg.Ident := SM_STDITEMLIST;
        SM_SENDITEMDESCLIST_CACHE:         TheDefMsg.Ident := SM_SENDITEMDESCLIST;
        SM_SENDTZITEMDESCLIST_CACHE:       TheDefMsg.Ident := SM_SENDTZITEMDESCLIST;
        SM_SENDFILTERITEMLIST_CACHE:       TheDefMsg.Ident := SM_SENDFILTERITEMLIST;
        SM_EFFECTIMAGELIST_CACHE:          TheDefMsg.Ident := SM_EFFECTIMAGELIST;
        SM_SPECIALCMD_CACHE:               TheDefMsg.Ident := SM_SPECIALCMD;
        SM_SENDCUSTOMMAGICCONFIG_CACHE:    TheDefMsg.Ident := SM_SENDCUSTOMMAGICCONFIG;
        SM_PLUGFILE_CACHE:                 TheDefMsg.Ident := SM_PLUGFILE;
        SM_SERVERCONFIG_CACHE:             TheDefMsg.Ident := SM_SERVERCONFIG;
        SM_SENDCUSTOMNPCCONFIG_CACHE:      TheDefMsg.Ident := SM_SENDCUSTOMNPCCONFIG;
        SM_SENDITEMDESCTOPLIST_CACHE:      TheDefMsg.Ident := SM_SENDITEMDESCTOPLIST;
        SM_SENDDROPITEMEFFECTLIST_CACHE:   TheDefMsg.Ident := SM_SENDDROPITEMEFFECTLIST;
      end;
    {$ELSE}
      if pDefMsg.Ident = SM_MODULEMD5_CACHE then
        TheDefMsg.Ident := SM_MODULEMD5
      else if pDefMsg.Ident = SM_SENDCUSTOMMONSTERCONFIG_CACHE then
        TheDefMsg.Ident := SM_SENDCUSTOMMONSTERCONFIG
      else if pDefMsg.Ident = SM_STDITEMLIST_CACHE then
        TheDefMsg.Ident := SM_STDITEMLIST
      else if pDefMsg.Ident = SM_SENDITEMDESCLIST_CACHE then
        TheDefMsg.Ident := SM_SENDITEMDESCLIST
      else if pDefMsg.Ident = SM_SENDTZITEMDESCLIST_CACHE then
        TheDefMsg.Ident := SM_SENDTZITEMDESCLIST
      else if pDefMsg.Ident = SM_SENDFILTERITEMLIST_CACHE then
        TheDefMsg.Ident := SM_SENDFILTERITEMLIST
      else if pDefMsg.Ident = SM_EFFECTIMAGELIST_CACHE then
        TheDefMsg.Ident := SM_EFFECTIMAGELIST
      else if pDefMsg.Ident = SM_SPECIALCMD_CACHE then
        TheDefMsg.Ident := SM_SPECIALCMD
      else if pDefMsg.Ident = SM_SENDCUSTOMMAGICCONFIG_CACHE then
        TheDefMsg.Ident := SM_SENDCUSTOMMAGICCONFIG
      else if pDefMsg.Ident =  SM_PLUGFILE_CACHE then
        TheDefMsg.Ident := SM_PLUGFILE
      else if pDefMsg.Ident = SM_SERVERCONFIG_CACHE then
        TheDefMsg.Ident := SM_SERVERCONFIG
      else if pDefMsg.Ident = SM_SENDCUSTOMNPCCONFIG_CACHE then
        TheDefMsg.Ident := SM_SENDCUSTOMNPCCONFIG
      else if pDefMsg.Ident = SM_SENDITEMDESCTOPLIST_CACHE then
        TheDefMsg.Ident := SM_SENDITEMDESCTOPLIST
      else if pDefMsg.Ident = SM_SENDDROPITEMEFFECTLIST_CACHE then
        TheDefMsg.Ident := SM_SENDDROPITEMEFFECTLIST
    {$IFEND}
      ErrorNum := 21;

      sTemp := FCacheDatas[pDefMsg.Ident - SM_MODULEMD5_CACHE];
      Context.AddServerMsg(@TheDefMsg, PChar(sTemp), Length(sTemp));

      Exit;
    end

    // 发送客户端反外挂模块，插到发送黑名单前面 chongchong 2016-05-28
    else if pDefMsg.Ident = SM_BLACKMODULEMD5 then
    begin
      ErrorNum := 30;
    {$IF CLIENT_ANTIPLUG = 1}
      // 黑名单功能已经取消没鸟用，直接用内核反外挂把这个替换掉 chongchong 2016-05-28
      if (not g_boLogoutNoResendAntiplugStream) or (Context.dwRecvClientAntiplugCRC <> g_ClientAntiPlugDllStringCRC) then
      begin
        Context.SendAntiPlugStreamInfo;
      end;
    {$IFEND}

      // 黑名单进程在这里发过去算了 chongchong 2016-08-01
      if (Length(g_ProcessBlacklistStr) > 0) and (SM_PROCESSBLACKLIST > 0) then
      begin
        Context.dwSendProcessBlacklistTick := MyGetTickCount;
        Context.SendProcessBlacklistMD5 := g_ProcessBlacklistMd5;
        TheDefMsg := MakeDefaultMsg(SM_PROCESSBLACKLIST, Length(g_ProcessBlacklistStr), 0, 0, 0);

        Context.AddServerMsg(@TheDefMsg, PChar(g_ProcessBlacklistStr), Length(g_ProcessBlacklistStr));
      end;

      if not Context.boIsOldClient then
      begin
        TheDefMsg := MakeDefaultMsg(SM_ITEMEAT_CDTIME, Length(sSendMsg), 0, 0, 0);
        Context.AddServerMsg(@TheDefMsg, @g_EatItemCDConfig, SizeOf(g_EatItemCDConfig));

        TheDefMsg := MakeDefaultMsg(SM_RUNGATE_SPEED_INTERVALS, Length(g_SendToClientSpeedIntervalsText), LoWord(g_SendToClientSpeedIntervalsLen), HiWord(g_SendToClientSpeedIntervalsLen), 0);
        Context.AddServerMsg(@TheDefMsg, PChar(g_SendToClientSpeedIntervalsText), Length(g_SendToClientSpeedIntervalsText));

        TheDefMsg := MakeDefaultMsg(SM_SOFT_DELAY_EXIT, 0, 0, 0, 0);
        Context.AddServerMsg(@TheDefMsg, nil, 0);
      end;
    end

    // 点了公告后，第二次反外挂模块验证
    else if pDefMsg.Ident = SM_SENDNOTICE then
    begin

    end

    // 添加技能，更新技能CD间隔
    else if (BufferLen > SizeOf(TDefaultMessage)) then
    begin
      // Buffer的最后一位是 #0，在此去掉 chongchong 2016-11-03
      MsgLen := BufferLen - SizeOf(TDefaultMessage);
      P := Buffer;
      Inc(P, SizeOf(TDefaultMessage));

      ErrorNum := 50;
      if (pDefMsg.Ident = SM_SENDMYMAGIC) then
      begin
        ErrorNum := 51;
        if Context.ProcesssSendToClientSendMyMagic(pDefMsg, P, MsgLen) then
        begin
          Exit;
        end;
      end

      else if (pDefMsg.Ident = SM_ADDMAGIC) then
      begin
        ErrorNum := 54;
        if Context.ProcesssSendToClientSendAddMagic(pDefMsg, P, MsgLen) then
        begin
          Exit;
        end;
      end

      //记录人物包裹药品
      else if (pDefMsg.Ident = SM_BAGITEMS) or (pDefMsg.Ident = SM_HEROBAGITEMS) then
      begin
        ErrorNum := 55;
        Context.ProcesssSendToClientBagItems(pDefMsg, P, MsgLen, pDefMsg.Ident = SM_BAGITEMS);
      end

      else if (pDefMsg.Ident = SM_ADDITEM) or (pDefMsg.Ident = SM_HEROADDITEM) then
      begin
        ErrorNum := 56;
        Context.ProcesssSendToClientAddItem(pDefMsg, P, MsgLen, pDefMsg.Ident = SM_ADDITEM);
      end

      else if (pDefMsg.Ident = SM_DELITEM) or (pDefMsg.Ident = SM_HERODELITEM) then
      begin
        ErrorNum := 57;
        Context.ProcesssSendToClientDelItems(pDefMsg, P, MsgLen, pDefMsg.Ident = SM_DELITEM);
      end

      else if (pDefMsg.Ident = SM_DELITEMS) or (pDefMsg.Ident = SM_HERODELITEMS) then
      begin
        ErrorNum := 58;
        Context.ProcesssSendToClientDelItems(pDefMsg, P, MsgLen, pDefMsg.Ident = SM_DELITEMS);
      end

      else if (pDefMsg.Ident = SM_DROPITEM_SUCCESS) or (pDefMsg.Ident = SM_HERODROPITEM_SUCCESS) then
      begin
        ErrorNum := 59;
        Context.ProcesssSendToClientDropItem(pDefMsg, nil, 0, pDefMsg.Ident = SM_DROPITEM_SUCCESS);
      end
    end
    else if (pDefMsg.Ident = SM_EAT_OK) or (pDefMsg.Ident = SM_AUTOEAT_OK) then
    begin
      ErrorNum := 60;
      Context.ProcesssSendToClientEatItemOK(pDefMsg, nil, 0, True);
    end
    else if (pDefMsg.Ident = SM_HEROEAT_OK) or (pDefMsg.Ident = SM_HEROAUTOEAT_OK) then
    begin
      ErrorNum := 70;
      Context.ProcesssSendToClientEatItemOK(pDefMsg, nil, 0, False);
    end

    else if (pDefMsg.Ident = SM_MASTERBAGTOHEROBAG_OK) then
    begin
      ErrorNum := 90;
      Context.ProcesssSendToClientMasterBagToHeroBagOK(pDefMsg, nil, 0, True);
    end

    else if (pDefMsg.Ident = SM_HEROBAGTOMASTERBAG_OK) then
    begin
      ErrorNum := 100;
      Context.ProcesssSendToClientHeroBagToMasterBagOK(pDefMsg, nil, 0, False);
    end

    // 将发送间隔附加到数据包上面 2020-03-08
    else if (pDefMsg.Ident = SM_ENABLE_UPLOAD_PICKITEMS) then
    begin
      pDefMsg.Tag := g_Config.dwClientUploadPickItemsTime;
      Context.boEnableClientUploadPickItems := True;
    end;

    ErrorNum := 110;
    // 数据包只有包头
    if BufferLen = SizeOf(TDefaultMessage) then
    begin
      ErrorNum := 111;
      Context.AddServerMsg(pDefMsg, nil, 0);
    end
    else

    // 数据包带包头 数据包的长度 = 包头长 + 附加数据长
    begin
      // 数据体
      ErrorNum := 113;
      P := Buffer;
      Inc(P, SizeOf(TDefau1ltMessage));
      ErrorNum := 114;

      MsgLen := BufferLen - SizeOf(TDefaultMessage);

      ErrorNum := 115;
      //Context.PostSendBuffer(PChar(sSendMsg), Length(sSendMsg));
      Context.AddServerMsg(pDefMsg, P, MsgLen);
    end;
  except
    on E: Exception do
    begin
      AddMainLogMsg('TProcessRecvDataThread.DoForwardToClientData, ErrorNum = ' + IntToStr(ErrorNum) +', ' + E.Message, 0);
    end;
  end;
end;

procedure TRunGate.DoRecvFullServiceMsg(Buffer: PChar; BufferLen: Integer);
var
  pDefMsg: pTDefaultMessage;
  sTemp: string;
begin
  if (BufferLen > SizeOf(TDefaultMessage)) then
  begin
    pDefMsg := pTDefaultMessage(Buffer);
    sTemp := StrPas(PChar(@Buffer[SizeOf(TDefaultMessage)]));

    FOnwer.AddFullServiceMsg(pDefMsg, sTemp);
  end;
end;

{ TProcessServerReceiveThread }

procedure TProcessServerReceiveThread.Execute;
begin
   while not Terminated do
  begin
    FRunGate.ProcessRecvBuffer(Self);
    Sleep(1);
  end;
end;

{$IFEND} // end of {$IF UseIocpClient = 0}

{$IF MultiThreadRunContext <> 0}

{ TFullServiceMsgProcessThread }

constructor TFullServiceMsgProcessThread.Create(CreateSuspended: Boolean);
begin
  inherited Create(CreateSuspended);
end;

procedure TFullServiceMsgProcessThread.Execute;
var
  I: Integer;
  List: TList;
  MirContext: TMirClientContext;

  FullServiceMsg: PClientMsg;
  FullServiceMsgText_New, S: string;     // FullServiceMsgText_Old
  Count: Integer;
begin
  FIsRun := True;
  try
    while not Terminated do
    begin
      //FullServiceMsgText_Old := '';
      FullServiceMsgText_New := '';

      FRunGateManager.FFullServiceMsgList.Lock;
      try
        Count := 0;
        while FRunGateManager.FFullServiceMsgList.Count > 0 do
        begin
          FullServiceMsg := FRunGateManager.FFullServiceMsgList.Items[0];

          if (FullServiceMsg.pBuffer <> nil) and (FullServiceMsg.nBufferLen > 0) then
          begin
            //
            //  SM_SYSMESSAGE
            //  SM_TOPCHATBOARDMESSAGE
            //  SM_MOVEMESSAGE
            //  SM_SUPERMOVEMESSAGE
            //  SM_NEWLINEMESSAGE
            //  SM_CENTERMESSAGE
            //
            // 如果一次太多，只发10算了。
            if Count < 10 then
            begin
              {
              if (FullServiceMsg.DefMessage.Ident = SM_SUPERMOVEMESSAGE) or
                (FullServiceMsg.DefMessage.Ident = SM_NEWLINEMESSAGE) then
              begin
                //S := '#' + EncodeMessage(FullServiceMsg.DefMessage) + StrPas(FullServiceMsg.pBuffer) + '!';

                sTemp := EncodeMessage(FullServiceMsg.DefMessage);
                nLen := Length(sTemp);

                SetLength(S, 2 + nLen + FullServiceMsg.nBufferLen);

                S[1] := '#';
                S[Length(S)] := '!';
                Move(sTemp[1], S[2], nLen);
                Move(FullServiceMsg.pBuffer^, S[2 + nLen], FullServiceMsg.nBufferLen);

                //FullServiceMsgText_Old := '';
                FullServiceMsgText_New := FullServiceMsgText_New + S;
              end
              else
              begin
                //S := '#' + EncodeMessage(FullServiceMsg.DefMessage) + StrPas(FullServiceMsg.pBuffer) + '!';

                sTemp := EncodeMessage(FullServiceMsg.DefMessage);
                nLen := Length(sTemp);

                SetLength(S, 2 + nLen + FullServiceMsg.nBufferLen);

                S[1] := '#';
                S[Length(S)] := '!';
                Move(sTemp[1], S[2], nLen);
                Move(FullServiceMsg.pBuffer^, S[2 + nLen], FullServiceMsg.nBufferLen);

                FullServiceMsgText_Old := FullServiceMsgText_Old + S;
                FullServiceMsgText_New := FullServiceMsgText_New + S;
              end;
              }

              S := EncodeRunGateMsg(@FullServiceMsg.DefMessage, FullServiceMsg.pBuffer, FullServiceMsg.nBufferLen - 1);

              FullServiceMsgText_New := FullServiceMsgText_New + S;
            end;

            FreeMem(FullServiceMsg.pBuffer, FullServiceMsg.nBufferLen);
          end;

          FreeMem(FullServiceMsg);
          FRunGateManager.FFullServiceMsgList.Delete(0);

          Inc(Count);
        end;
      finally
        FRunGateManager.FFullServiceMsgList.Unlock;
      end;

      if {(Length(FullServiceMsgText_Old) > 0) or} (Length(FullServiceMsgText_New) > 0) then
      begin
        List := TList.Create;
        try
          TIocpClientContextPool.Instance.GetOnlineContextList(List);
          for I := List.Count - 1 downto 0 do
          begin
            MirContext := List.Items[I];
            if (MirContext.Socket = INVALID_SOCKET) then
            begin
              TIocpClientContextPool.Instance.TryCloseContext(MirContext);
            end
            else
            begin
              if not MirContext.boIsOldClient then
                MirContext.AddServerText(FullServiceMsgText_New);
            end;

            if Terminated then Break;
          end;
        finally
          List.Free;
        end;
      end;

      Sleep(1);
    end;
  finally
    FIsRun := False;
  end;
end;

{ TClientContextRunThread }

constructor TClientContextRunThread.Create(CreateSuspended: Boolean);
begin
  inherited Create(CreateSuspended);
end;

procedure TClientContextRunThread.Execute;
var
  I: Integer;
  List: TList;
  MirContext: TMirClientContext;

{$IF CLIENT_ANTIPLUG = 1}
  LastSendAntiPlugInfoTick: LongWord;
  LastSendAntiPlugTick: LongWord;
{$IFEND}

  LastSendUploadDllTick: LongWord;
begin
  FIsRun := True;
{$IF CLIENT_ANTIPLUG = 1}
  Randomize;
  LastSendAntiPlugInfoTick := MyGetTickCount - Random(3000);
  LastSendAntiPlugTick := MyGetTickCount - 1000;
{$IFEND}

  List := TList.Create;
  List.Capacity := 1000;
  try
    while not Terminated do
    begin
      List.Count := 0;
      
      TIocpClientContextPool.Instance.GetOnlineContextList(List);
      for I := List.Count - 1 downto 0 do
      begin
        MirContext := List.Items[I];
        if (MirContext.Socket <> INVALID_SOCKET) and (MirContext.RunThreadIndex = FRunThreadIndex) then
        begin
          MirContext.Run(Self);

          if MirContext.IsPostedCloseQuest or MirContext.boDelayClose then Continue;

        (*
        {$IF CLIENT_ANTIPLUG = 1}
          if (not MirContext.IsWaitingGiveBack) then
          begin
            EnterCriticalSection(g_CSRunGatePlug);
            try
              if Assigned(g_rgpRunContext) then
              begin
                g_rgpRunContext(MirContext.ContextID);
              end;
            finally
              LeaveCriticalSection(g_CSRunGatePlug);
            end;
          end;
        {$IFEND}
        *)

{$IF CLIENT_ANTIPLUG = 1}
          if (g_ClientAntiPlugDllSize > 0) then
          begin
            if MirContext.boFirstClientQueryBagItems and
              (tick_diff(LastSendAntiPlugInfoTick, MyGetTickCount) >= 100) and
              (MirContext.dwClientAntiPlugVersion <> g_ClientAntiPlugVersion) then
            begin
              if (not g_boLogoutNoResendAntiplugStream) or (MirContext.dwRecvClientAntiplugCRC <> g_ClientAntiPlugDllStringCRC) then
              begin
                MirContext.SendAntiPlugStreamInfo;
                LastSendAntiPlugInfoTick := MyGetTickCount;
              end
              else if (g_RunGatePlugDllHandle <> 0) then
              begin
                MirContext.SendAntiPlugStreamLoadCache;
              end;
            end
            else if MirContext.boSendLoadAntiPlug and (not MirContext.boSendLoadAntiPlugFinished) and
              (tick_diff(MirContext.dwSendLoadAntiPlugTick, MyGetTickCount) >= 200) and  // 限定单人最大速度
              (tick_diff(LastSendAntiPlugTick, MyGetTickCount) >= g_ClientAntiPlugDllSendInterval) then
            begin
              if MirContext.SendAntiPlugStream then
              begin
                LastSendAntiPlugTick := MyGetTickCount;
              end;
            end;

            // 卸载插件后，未更新直接发缓存加载 2020-01-07
            if (g_RunGatePlugDllHandle <> 0) and
              MirContext.boWaitLoadAntiPlug and (tick_diff(MirContext.dwWaitLoadAntiPlugTick, MyGetTickCount) >= 6000) and
              (MirContext.dwClientAntiPlugVersion = g_ClientAntiPlugVersion) then
            begin
              MirContext.SendAntiPlugStreamLoadCache;
            end;
          end;
{$IFEND}
        end;

        if Terminated then Break;
      end;

      Sleep(1);
    end;
  finally
    List.Free;
    FIsRun := False;
  end;
end;

{$IFEND}

{ TRunGateManager }

constructor TRunGateManager.Create(AFormHandle: THandle);
{$IF NEED_REGISTER = 1}
var
  ExtendedInfo: Integer;
{$IFEND}
begin
  FFormHandle := AFormHandle;
  FRunGateList := TList.Create;

  FTimerCheckConnect := TTimer.Create(nil);
  FTimerCheckConnect.Interval := 1000;
  FTimerCheckConnect.Enabled := False;
  FTimerCheckConnect.OnTimer := OnTimerCheckConnect;

{$IF UseIocpClient <> 0}
  FIocpClient := TIocpTcpClient.Create(nil);
  FIocpClient.RegisterContextClass(TMirRemoteContext);
  FIocpClient.IocpCore.RecvDataDefaultSize := MAX_IOCP_CLIENT_RECV_BUFFER_SIZE;     // 2M接收缓冲区 用来收M2的数据
  FIocpClient.OnError := OnIocpError;
{$IFEND}

{$IF MultiThreadRunContext = 0}
  FTimerRunContext := TTimer.Create(nil);
  FTimerRunContext.Interval := 1;
  FTimerRunContext.Enabled := False;
  FTimerRunContext.OnTimer := OnTimerRunContext;
{$IFEND}

{$IF CLIENT_ANTIPLUG = 1}
  FLastSendAntiPlugTick := MyGetTickCount;
{$IFEND}

(*
{$IF NEED_REGISTER = 1}
{$I Registered_Start.inc}
  // 暗桩：取客户端时间对比到期时间
  if WLRegGetStatus(ExtendedInfo) = wlIsRegistered then
  begin
    Randomize;
    dwCheck1Tick := MyGetTickCount;

  {$IF REGISTER_TEST = 0}
    dwCheck1Time := 1800000 + Random(3600000);    // 30 - 90 分钟
  {$ELSE}
    dwCheck1Time := 30000 + Random(30000);        // 30 - 60 秒
  {$IFEND}
  end;
{$I Registered_End.inc}
{$IFEND}
*)

  FFullServiceMsgList := TSafeList.Create({$IFDEF USE_SPINLOCK}'FullServiceMsgLocker'{$ENDIF});

{$IF NEED_REGISTER = 1}
//{$I Registered_Start.inc}
//  // 用来连接验证服务器
//  if WLRegGetStatus(ExtendedInfo) = wlIsRegistered then
//  begin
//    FIsVerifyServerResult := False;
//    FConnectVerifyServerTick := 0;
//    FConnectVerifyServerIndex := 0;
//
//    FRetryConnectVerifyServerTime := 8*60*60*1000 + Random(16*60*60*1000);
//    FRetryConnectVerifyServerCount := 0;
//    FIsCheckRetryVerifyServerResult := False;
//    FIsRetryVerifyServerResult := False;
//
//    FTimeLeft := 0;
//    FIsShowTimeLeft := False;
//  end;
//{$I Registered_End.inc}
{$IFEND}
end;

destructor TRunGateManager.Destroy;
var
  I: Integer;
  RunGate: TRunGate;
begin
  for I := 0 to FRunGateList.Count - 1 do
  begin
    RunGate := FRunGateList.Items[I];
    RunGate.Free;
  end;
  FRunGateList.Free;

  FTimerCheckConnect.Free;

{$IF UseIocpClient <> 0}
  FIocpClient.Free;
{$IFEND}

{$IF MultiThreadRunContext = 0}
  FTimerRunContext.Free;
{$IFEND}

  ClearFullServiceMsgList;
  FFullServiceMsgList.Free;

  inherited;
end;

function TRunGateManager.Add(APort: Word;  AID: Integer): TRunGate;
begin
  Result := TRunGate.Create(Self, APort, FFormHandle, AID);
  FRunGateList.Add(Result);
end;

function TRunGateManager.GetCount: Integer;
begin
  Result := FRunGateList.Count;
end;

function TRunGateManager.GetRunGates(Index: Integer): TRunGate;
begin
  Result := FRunGateList.Items[Index];
end;

function TRunGateManager.StartRunGates(RunGateThread: Integer): Boolean;
var
  I: Integer;
  boRet: Boolean;
begin
  Result := False;

{$IF UseIocpClient <> 0}
  FIocpClient.OnError := nil;
  for I := 0 to FIocpClient.Count - 1 do
  begin
    TIocpRemoteContext(FIocpClient.Items[I]).Active := False;
  end;
  FIocpClient.IocpCore.WorkerThreadCount := Max(FRunGateList.Count * 4, 4);
  FIocpClient.OnError := OnIocpError;
{$IFEND}

  for I := 0  to FRunGateList.Count - 1 do
  begin
    boRet := TRunGate(FRunGateList.Items[I]).StartService(RunGateThread);
    if boRet then Result := True;
  end;

  if Result then
  begin
    FIsStart := True;
    FTimerCheckConnect.Enabled := True;

  {$IF MultiThreadRunContext = 0}
    FTimerRunContext.Enabled := True;
  {$ELSE}
    FFullServiceMsgProcessThread := TFullServiceMsgProcessThread.Create(True);
    FFullServiceMsgProcessThread.FreeOnTerminate := False;
    FFullServiceMsgProcessThread.FRunGateManager := Self;
    FFullServiceMsgProcessThread.Resume;

    for I := Low(FClientContextRunThreads) to High(FClientContextRunThreads) do
    begin
      FClientContextRunThreads[I] := TClientContextRunThread.Create(True);
      FClientContextRunThreads[I].FRunThreadIndex := I;
      FClientContextRunThreads[I].FreeOnTerminate := False;
      FClientContextRunThreads[I].Resume;
    end;
  {$IFEND}
  end;
end;

procedure TRunGateManager.StopRunGates;
var
  I: Integer;
  IsStopAll: Boolean;
begin
{$IF UseIocpClient <> 0}
  for I := 0 to FIocpClient.Count - 1 do
  begin
    TIocpRemoteContext(FIocpClient.Items[I]).Active := False;
  end;
{$IFEND}

  for I := 0  to FRunGateList.Count - 1 do
  begin
    TRunGate(FRunGateList.Items[I]).StopServices;
  end;
  FIsStart := False;

  FTimerCheckConnect.Enabled := False;

{$IF MultiThreadRunContext = 0}
  FTimerRunContext.Enabled := False;
{$ELSE}
  FFullServiceMsgProcessThread.Terminate;
  for I := Low(FClientContextRunThreads) to High(FClientContextRunThreads) do
    FClientContextRunThreads[I].Terminate;

{$IF NEED_REGISTER = 0}
  OutputDebugString('等待运行线程退出');
{$IFEND}

  while True do
  begin
    IsStopAll := not FFullServiceMsgProcessThread.FIsRun;

    if IsStopAll then
    begin
      for I := Low(FClientContextRunThreads) to High(FClientContextRunThreads) do
      begin
        if FClientContextRunThreads[I].FIsRun then
        begin
          IsStopAll := False;
          Break;
        end;
      end;
    end;

    if IsStopAll then Break;

    Sleep(10);
    Application.ProcessMessages;
  end;

  FFullServiceMsgProcessThread.Free;

  for I := Low(FClientContextRunThreads) to High(FClientContextRunThreads) do
  begin
    FClientContextRunThreads[I].Free;
  end;

{$IF NEED_REGISTER = 0}
  OutputDebugString('运行线程全释放');
{$IFEND}

{$IFEND}
end;

procedure TRunGateManager.SendOnLineUserMsg(DefMsg: pTDefaultMessage; DataAdd: PChar; DataAddLen: LongWord; CheckLogin: Boolean);
var
  I: Integer;
  RunGate: TRunGate;
begin
  if not FIsStart then Exit;
  for I := 0  to FRunGateList.Count - 1 do
  begin
    RunGate := FRunGateList.Items[I];
    RunGate.SendOnLineUserMsg(DefMsg, DataAdd, DataAddLen, CheckLogin);
  end;
end;

function TRunGateManager.GetRecvBlockCount: Int64;
var
  I: Integer;
  RunGate: TRunGate;
begin
  Result := 0;
  for I := 0 to FRunGateList.Count - 1 do
  begin
    RunGate := FRunGateList.Items[I];
    Result := Result + RunGate.TcpServer.IocpCore.CumulativeDataSendRecvLog.RecvBlockCount;
  end;
end;

function TRunGateManager.GetRecvBytesSize: Int64;
var
  I: Integer;
  RunGate: TRunGate;
begin
  Result := 0;
  for I := 0 to FRunGateList.Count - 1 do
  begin
    RunGate := FRunGateList.Items[I];
    Result := Result + RunGate.TcpServer.IocpCore.CumulativeDataSendRecvLog.RecvBytesSize;
  end;
end;

function TRunGateManager.GetSendBlockCount: Int64;
var
  I: Integer;
  RunGate: TRunGate;
begin
  Result := 0;
  for I := 0 to FRunGateList.Count - 1 do
  begin
    RunGate := FRunGateList.Items[I];
    Result := Result + RunGate.TcpServer.IocpCore.CumulativeDataSendRecvLog.SendBlockCount;
  end;
end;

function TRunGateManager.GetSendBytesSize: Int64;
var
  I: Integer;
  RunGate: TRunGate;
begin
  Result := 0;
  for I := 0 to FRunGateList.Count - 1 do
  begin
    RunGate := FRunGateList.Items[I];
    Result := Result + RunGate.TcpServer.IocpCore.CumulativeDataSendRecvLog.SendBytesSize;
  end;
end;

function TRunGateManager.GetWorkerThreadCount: Integer;
var
  I: Integer;
  RunGate: TRunGate;
begin
  Result := 0;
  for I := 0 to FRunGateList.Count - 1 do
  begin
    RunGate := FRunGateList.Items[I];
    Result := Result + RunGate.TcpServer.IocpCore.WorkerThreadCount;
    Result := Result + 1;                           // TAcceptListenerThread
  end;

{$IF UseIocpClient <> 0}
  Result := Result + FIocpClient.IocpCore.WorkerThreadCount;
{$ELSE}
  Result := Result + FRunGateList.Count;            // TProcessServerReceiveThread
{$IFEND}

{$IF MultiThreadRunContext <> 0}
  Result := Result + 1;                             // FFullServiceMsgProcessThread
  Result := Result  + MIRCONTEXT_RUN_THREAD_COUNT;  // TClientContextRunThreads
{$IFEND}
end;

{$IF NEED_REGISTER = 1}
type
  PIPCheckInfo = ^TIPCheckInfo;
  TIPCheckInfo = packed record
    IP: Cardinal;
    Date: Integer;
    RunGateIP: Integer;
  end;

  PCheckItemInfo = ^TCheckItemInfo;
  TCheckItemInfo = record
    Value: Integer;
    Count: Integer;
  end;

(*
function GetClientRunGateIP(List: TList): Integer;
var
  I, J, AllCount: Integer;
  List2: TList;
  IsFound: Boolean;
  IPCheckInfo: PIPCheckInfo;
  ItemInfo: PCheckItemInfo;
begin
  Result := 0;
  if List.Count < 20 then Exit;

  List2 := TList.Create;
  try
    AllCount := 0;
    for I := 0 to List.Count - 1 do
    begin
      IPCheckInfo := List.Items[I];

      if IPCheckInfo.RunGateIP <> 0 then
      begin
        Inc(AllCount);

        IsFound := False;

        for J := 0 to List2.Count - 1 do
        begin
          ItemInfo := List2.Items[J];
          if ItemInfo.Value = IPCheckInfo.RunGateIP then
          begin
            IsFound := True;
            Inc(ItemInfo.Count);
            Break;
          end;
        end;

        if not IsFound then
        begin
          GetMem(ItemInfo, SizeOf(TCheckItemInfo));
          ItemInfo.Value := IPCheckInfo.RunGateIP;
          ItemInfo.Count := 1;
          List2.Add(ItemInfo);
        end;
      end;
    end;

    if AllCount >= 20 then
    begin
      for I := 0 to List2.Count - 1 do
      begin
        ItemInfo := List2.Items[I];
        if ItemInfo.Count >= AllCount div 2 then
        begin
          Result := ItemInfo.Value;
          Break;
        end;
      end;
    end;
  finally
    for I := 0 to List2.Count - 1 do
    begin
      ItemInfo := List2.Items[I];
      FreeMem(ItemInfo);
    end;

    List2.Free;
  end;
end;
*)

function GetClientDate(List: TList): Integer;
var
  I, J, AllCount: Integer;
  List2: TList;
  IsFound: Boolean;
  IPCheckInfo: PIPCheckInfo;
  ItemInfo: PCheckItemInfo;
begin
  Result := 0;
  if List.Count < 20 then Exit;

  List2 := TList.Create;
  try
    AllCount := 0;
    for I := 0 to List.Count - 1 do
    begin
      IPCheckInfo := List.Items[I];

      if IPCheckInfo.Date <> 0 then
      begin
        Inc(AllCount);

        IsFound := False;

        for J := 0 to List2.Count - 1 do
        begin
          ItemInfo := List2.Items[J];
          if ItemInfo.Value = IPCheckInfo.Date then
          begin
            IsFound := True;
            Inc(ItemInfo.Count);
            Break;
          end;
        end;

        if not IsFound then
        begin
          GetMem(ItemInfo, SizeOf(TCheckItemInfo));
          ItemInfo.Value := IPCheckInfo.Date;
          ItemInfo.Count := 1;
          List2.Add(ItemInfo);
        end;
      end;
    end;

    if AllCount >= 20 then
    begin
      for I := 0 to List2.Count - 1 do
      begin
        ItemInfo := List2.Items[I];
        if ItemInfo.Count >= AllCount div 2 then
        begin
          Result := ItemInfo.Value;
          Break;
        end;
      end;
    end;
  finally
    for I := 0 to List2.Count - 1 do
    begin
      ItemInfo := List2.Items[I];
      FreeMem(ItemInfo);
    end;

    List2.Free;
  end;
end;
{$IFEND}

{$IF NEED_REGISTER = 1}
//// 这里加暗桩，检查服务器时间是否更改 chongchong 2016-07-09
//function CheckIsRun(LastDay: Integer; var CheckTick, CheckTime: LongWord): Boolean;
//var
//  I, J: Integer;
//  IPCheckInfoList, List: TList;
//  MirContext: TMirClientContext;
//  boFound: Boolean;
//  IPCheckInfo: PIPCheckInfo;
//  ClientDate: Integer;
//begin
//  Result := False;
//
//  Randomize;
//  CheckTick := MyGetTickCount;
//
//{$IF REGISTER_TEST = 0}
//  CheckTime := 1800000 + Random(3600000);    // 30 - 90 分钟
//{$ELSE}
//  CheckTime := 30000 + Random(30000);        // 30 - 60 秒
//  AddMainLogMsg('~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~进入暗桩 - 时间检测1', 0);
//{$IFEND}
//
//  List := TList.Create;
//  try
//    TIocpClientContextPool.Instance.GetOnlineContextList(List);
//
//    IPCheckInfoList := TList.Create;
//    try
//      for I := List.Count - 1 downto 0 do
//      begin
//        MirContext := List.Items[I];
//        boFound := False;
//
//        if (MirContext.Socket <> INVALID_SOCKET) then
//        begin
//          for J := 0 to IPCheckInfoList.Count - 1 do
//          begin
//            IPCheckInfo := IPCheckInfoList.Items[J];
//            if IPCheckInfo.IP = MirContext.RemoteAddrValue then
//            begin
//              boFound := True;
//
//              if MirContext.nClientSendDate <> 0 then
//                IPCheckInfo.Date := MirContext.nClientSendDate;
//              if MirContext.nClientSendRunGateIP <> 0 then
//                IPCheckInfo.RunGateIP := MirContext.nClientSendRunGateIP;
//
//              Break;
//            end;
//          end;
//
//          if not boFound then
//          begin
//            GetMem(IPCheckInfo, SizeOf(TIPCheckInfo));
//            IPCheckInfoList.Add(IPCheckInfo);
//            IPCheckInfo.IP := MirContext.RemoteAddrValue;
//            if MirContext.nClientSendDate <> 0 then
//              IPCheckInfo.Date := MirContext.nClientSendDate;
//            if MirContext.nClientSendRunGateIP <> 0 then
//              IPCheckInfo.RunGateIP := MirContext.nClientSendRunGateIP;
//          end;
//
//          if IPCheckInfoList.Count >= 20 then
//          begin
//            //ClientRunGateIP := GetClientRunGateIP(IPCheckInfoList);
//            ClientDate := GetClientDate(IPCheckInfoList);
//
//            if (ClientDate <> 0) and (DaysBetween(ClientDate, Now()) >= 2) and (ClientDate > (LastDay xor $0B5F4B3E)) then
//            begin
//              Result := True;
//            end;
//          end;
//        end;
//      end;
//    finally
//      for J := 0 to IPCheckInfoList.Count - 1 do
//      begin
//        IPCheckInfo := IPCheckInfoList.Items[J];
//        FreeMem(IPCheckInfo);
//      end;
//      IPCheckInfoList.Free;
//    end;
//  finally
//    List.Free;
//  end;
//end;
{$IFEND}

procedure TRunGateManager.OnTimerCheckConnect(Sender: TObject);
var
{$IF NEED_REGISTER = 1}
  ExtendedInfo: Integer;
  List: TList;
  MirContext: TMirClientContext;
{$IFEND}

{$IF UseIocpClient = 0}
  RunGate: TRunGate;
{$ELSE}
  Context: TMirRemoteContext;
{$IFEND}

  I: Integer;
begin
  if not IsStart then Exit;
{$IF NEED_REGISTER = 1}
//{$I Registered_Start.inc}
//  // 连接检测服务器，看是否是合法的注册 chongchong 2016-07-12
//  if WLRegGetStatus(ExtendedInfo) = wlIsRegistered then
//  begin
//    if ((not FIsVerifyServerResult) and (tick_diff(FConnectVerifyServerTick, MyGetTickCount) >= 15000)) then
//    begin
//      FConnectVerifyServerTick := MyGetTickCount;
//
//      if Assigned(FClientSocetkVerify) then
//        FreeAndNil(FClientSocetkVerify);
//
//      FClientSocetkVerify := TClientSocket.Create(nil);
//      FClientSocetkVerify.OnConnect := OnClientSocetkVerifyConnect;
//      FClientSocetkVerify.OnError := OnClientSocetkVerifyError;
//      FClientSocetkVerify.OnRead := OnClientSocetkVerifyRead;
//
//      FClientSocetkVerify.ClientType := ctNonBlocking;
//
//      Randomize;
//
//      if FConnectVerifyServerIndex > 1 then
//      begin
//        FConnectVerifyServerIndex := 0;
//        AddMainLogMsg('连接验证服务器失败，尝试重新连接', 1);
//      end;
//
//    {$IF REGISTER_TEST = 0}
//      case FConnectVerifyServerIndex of
//        0: FClientSocetkVerify.Host := '127.0.0.1'; //'183.2.195.111';
//        1: FClientSocetkVerify.Host := '127.0.0.1';
//      end;
//      FClientSocetkVerify.Port := 16161 + Random(8);          // 16161-16168 随机端口
//    {$ELSE}
//      case FConnectVerifyServerIndex of
//        0: FClientSocetkVerify.Address := '127.0.0.1';
//        1: FClientSocetkVerify.Address := '127.0.0.1';
//      end;
//      FClientSocetkVerify.Port := 16161;
//    {$IFEND}
////
//      Inc(FConnectVerifyServerIndex);
////
//      FClientSocetkVerify.Active := True;
//    end
//    else if FIsVerifyServerResult then
//    begin
//      if (GM_DATA = 5) and (tick_diff(FConnectVerifyServerTick, MyGetTickCount) >= FRetryConnectVerifyServerTime) then
//      begin
//        FRetryConnectVerifyServerTime := 8*60*60*1000 + Random(16*60*60*1000);
//
//      {$IF REGISTER_TEST = 1}
//        AddMainLogMsg('~~~~~~~~~~~~~~~~~~~~~~~~~~~重新验证', 1);
//      {$IFEND}
//
//        FIsCheckRetryVerifyServerResult := True;
//        FIsRetryVerifyServerResult := False;
//
//        FConnectVerifyServerTick := MyGetTickCount;
//
//        if Assigned(FClientSocetkVerify) then
//          FreeAndNil(FClientSocetkVerify);
//
//        FClientSocetkVerify := TClientSocket.Create(nil);
//        FClientSocetkVerify.OnConnect := OnClientSocetkVerifyConnect;
//        FClientSocetkVerify.OnError := OnClientSocetkVerifyError;
//        FClientSocetkVerify.OnRead := OnClientSocetkVerifyRead;
//
//        FClientSocetkVerify.ClientType := ctNonBlocking;
//        
//        Randomize;
//
//        if FConnectVerifyServerIndex > 1 then
//        begin
//          FConnectVerifyServerIndex := 0;
//        end;
//
//      {$IF REGISTER_TEST = 0}
//        case FConnectVerifyServerIndex of
//          0: FClientSocetkVerify.Host := 'yz.fengwg.net'; //'183.2.195.111';
//          1: FClientSocetkVerify.Host := 'yz1.fengwg.net';
//        end;
//        FClientSocetkVerify.Port := 16161 + Random(8);          // 16161-16168 随机端口
//      {$ELSE}
//        case FConnectVerifyServerIndex of
//          0: FClientSocetkVerify.Address := '10.0.0.5';  
//          1: FClientSocetkVerify.Address := '10.0.0.5'; 
//        end;
//        FClientSocetkVerify.Port := 16161;          
//      {$IFEND}
//
//        Inc(FConnectVerifyServerIndex);
//
//        FClientSocetkVerify.Active := True;
//      end;
//
//      if FIsCheckRetryVerifyServerResult and (not FIsRetryVerifyServerResult) and (tick_diff(FConnectVerifyServerTick, MyGetTickCount) >= 180000{三分钟都没回应}) then
//      begin
//        FIsCheckRetryVerifyServerResult := False;
//        Inc(FRetryConnectVerifyServerCount);
//
//      {$IF REGISTER_TEST = 1}
//        AddMainLogMsg('~~~~~~~~~~~~~~~~~~~~~~~~~~~' + IntToStr(FRetryConnectVerifyServerCount) + ' 没连上', 1);
//      {$IFEND}
//
//        if FRetryConnectVerifyServerCount >= 5 then
//        begin
//          Move(g_boVerifyFailTriggerScript, RUNGATECODE, 100);
//          GM_DATA := 6 + Random(10000);
//
//        {$IF REGISTER_TEST = 1}
//          AddMainLogMsg('~~~~~~~~~~~~~~~~~~~~~~~~~~~几次都连不上服务器', 1);
//        {$IFEND}
//        end;
//      end;
//    end;
//    
//    // 显示倒计时
//    if FIsShowTimeLeft then
//    begin
//      if tick_diff(FTimeLeftStartTick, MyGetTickCount) >= 3600000 then
//      begin
//        Dec(FTimeLeft);
//        FTimeLeftStartTick := MyGetTickCount;
//
//        if FTimeLeft <= 0 then
//        begin
//          List := TList.Create;
//          try
//            TIocpClientContextPool.Instance.GetOnlineContextList(List);
//            for I := List.Count - 1 downto 0 do
//            begin
//              MirContext := List.Items[I];
//              MirContext.Close;
//            end;
//          finally
//            List.Free;
//          end;
//
//        {$IF REGISTER_TEST = 1}
//          AddMainLogMsg('~~~~~~~~~~~~~~~~~~~~~~~~~~~打乱RUNGATECODE', 1);
//        {$IFEND}
//
//          RUNGATECODE := Random(High(Integer));
//          Move(g_boOpenVerifyCode, GM_OPEN, 64);
//        end;
//      end;
//
//      if FTimeLeft >= 0 then
//        Application.MainForm.Caption := GateName + '-' + g_sTitleName + '  [剩余时间: ' + IntToStr(FTimeLeft) + ' 小时]'
//      else
//        Application.MainForm.Caption := GateName + '-' + g_sTitleName + '  [剩余时间: 0 小时]';
//    end;
//  end;
//{$I Registered_end.inc}
{$IFEND}

{$IF UseIocpClient = 0}
  for I := 0 to FRunGateList.Count - 1 do
  begin
    RunGate := FRunGateList.Items[I];

  {$IF NEED_REGISTER = 1}
    {$I Registered_Start.inc}
    // 连接检测服务器，看是否是合法的注册 chongchong 2016-07-12
//    if WLRegGetStatus(ExtendedInfo) = wlIsRegistered then
//    begin
      if (not RunGate.FClientSocket.Active) and FIsVerifyServerResult and (tick_diff(RunGate.FTryConnecttionTick, MyGetTickCount) >= 8000)  then
      begin
        RunGate.FTryConnecttionTick := MyGetTickCount;
        RunGate.FClientSocket.Active := True;
      end;
//    end;
    {$I Registered_End.inc}
  {$ELSE}
    if (not RunGate.FClientSocket.Active) and (tick_diff(RunGate.FTryConnecttionTick, MyGetTickCount) >= 8000)  then
    begin
      RunGate.FTryConnecttionTick := MyGetTickCount;
      RunGate.FClientSocket.Active := True;
    end;
  {$IFEND}

    // 过一会向服务器发一个心跳包 (总检测时间的1/4，不用过高)
    if RunGate.FIsReady and RunGate.FClientSocket.Active and (tick_diff(RunGate.FSendCheckClientTick, MyGetTickCount) >= Max(g_dwCheckServerTimeOutTime div 4 * 1000, 45000)) then
    begin
      RunGate.FSendCheckClientTick := MyGetTickCount;
      RunGate.SendServerMsg(GM_CHECKCLIENT, 0, 0, 0, nil, 0);
    end;

    if RunGate.FIsReady and RunGate.FClientSocket.Active and ((tick_diff(RunGate.FRecvCheckServerTick, MyGetTickCount)) > g_dwCheckServerTimeOutTime * 1000) then
    begin
      AddMainLogMsg('服务器检测超时，断开M2服务器连接', 1);
      RunGate.FClientSocket.Active := False;
    end;
  end;
{$ELSE}
  for I := 0 to FIocpClient.Count - 1 do
  begin
    Context := TMirRemoteContext(FIocpClient.Items[I]);

  {$IF NEED_REGISTER = 1}
    {$I Registered_Start.inc}
    // 连接检测服务器，看是否是合法的注册 chongchong 2016-07-12
//    if WLRegGetStatus(ExtendedInfo) = wlIsRegistered then
//    begin
      if (not Context.Active) and{ FIsVerifyServerResult and} (tick_diff(Context.FTryConnecttionTick, MyGetTickCount) >= 8000)  then
      begin
        Context.FTryConnecttionTick := MyGetTickCount;
        Context.Active := False;
        Context.Active := True;
      end;
//    end;
    {$I Registered_End.inc}
  {$ELSE}
    if (not Context.Active) and (tick_diff(Context.FTryConnecttionTick, MyGetTickCount) >= 8000)  then
    begin
      Context.FTryConnecttionTick := MyGetTickCount;
      Context.Active := False;
      Context.Active := True;
    end;
  {$IFEND}

    // 过一会向服务器发一个心跳包 (总检测时间的1/4，不用过高)
    if Context.Active and (tick_diff(Context.FSendCheckClientTick, MyGetTickCount) >= Max(g_dwCheckServerTimeOutTime div 4 * 1000, 45000)) then
    begin
      Context.FSendCheckClientTick := MyGetTickCount;
      Context.SendServerMsg(GM_CHECKCLIENT, 0, 0, 0, nil, 0);
    end;

    if Context.Active and ((tick_diff(Context.FRecvCheckServerTick, MyGetTickCount)) > g_dwCheckServerTimeOutTime * 1000) then
    begin
      AddMainLogMsg('服务器检测超时，断开M2服务器连接', 1);
      Context.Active := False;
    end;
  end;
{$IFEND}

{$IF NEED_REGISTER = 1}
{$I VMProtectBegin.inc}
//  // 这里加暗桩，对比客户端时间和服务器时间 chongchong 2016-07-09
//  if (TIocpClientContextPool.Instance.ContextCount > 160) and (tick_diff(dwCheck1Tick, MyGetTickCount) >= dwCheck1Time) then
//  begin
//    if (g_nKeyLastDay1 > 0) and CheckIsRun(g_nKeyLastDay1, dwCheck1Tick, dwCheck1Time) then
//    begin
    {$IF REGISTER_TEST = 0}
//      RUNGATECODE := Random(High(Integer));
//      GM_DATA := 0;
//      GM_DATA_CACHE := 0;
    {$IFEND}
//    end;
//  end;
{$I VMProtectEnd.inc}
{$IFEND}
end;

{$IF MultiThreadRunContext = 0}
procedure TRunGateManager.OnTimerRunContext(Sender: TObject);
var
  I: Integer;
  List: TList;
  MirContext: TMirClientContext;

  FullServiceMsg: PClientMsg;
  FullServiceMsgText_New, S: string; // FullServiceMsgText_Old
  Count: Integer;
begin
  FullServiceMsgText_Old := '';
  FullServiceMsgText_New := '';

  FFullServiceMsgList.Lock;
  try
    Count := 0;
    while FFullServiceMsgList.Count > 0 do
    begin
      FullServiceMsg := FFullServiceMsgList.Items[0];

      if (FullServiceMsg.pBuffer <> nil) and (FullServiceMsg.nBufferLen > 0) then
      begin
        //
        //  SM_SYSMESSAGE
        //  SM_TOPCHATBOARDMESSAGE
        //  SM_MOVEMESSAGE
        //  SM_SUPERMOVEMESSAGE
        //  SM_NEWLINEMESSAGE
        //  SM_CENTERMESSAGE
        //
        // 如果一次太多，只发10算了。
        if Count < 10 then
        begin
          {
          if (FullServiceMsg.DefMessage.Ident = SM_SUPERMOVEMESSAGE) or
            (FullServiceMsg.DefMessage.Ident = SM_NEWLINEMESSAGE) then
          begin
            //S := '#' + EncodeMessage(FullServiceMsg.DefMessage) + StrPas(FullServiceMsg.pBuffer) + '!';

            sTemp := EncodeMessage(FullServiceMsg.DefMessage);
            nLen := Length(sTemp);

            SetLength(S, 2 + nLen + FullServiceMsg.nBufferLen);

            S[1] := '#';
            S[Length(S)] := '!';
            Move(sTemp[1], S[2], nLen);
            Move(FullServiceMsg.pBuffer^, S[2 + nLen], FullServiceMsg.nBufferLen);

            //FullServiceMsgText_Old := '';
            FullServiceMsgText_New := FullServiceMsgText_New + S;
          end
          else
          begin
            //S := '#' + EncodeMessage(FullServiceMsg.DefMessage) + StrPas(FullServiceMsg.pBuffer) + '!';

            sTemp := EncodeMessage(FullServiceMsg.DefMessage);
            nLen := Length(sTemp);

            SetLength(S, 2 + nLen + FullServiceMsg.nBufferLen);

            S[1] := '#';
            S[Length(S)] := '!';
            Move(sTemp[1], S[2], nLen);
            Move(FullServiceMsg.pBuffer^, S[2 + nLen], FullServiceMsg.nBufferLen);

            FullServiceMsgText_Old := FullServiceMsgText_Old + S;
            FullServiceMsgText_New := FullServiceMsgText_New + S;
          end;
          }
          S := EncodeRunGateMsg(@FullServiceMsg.DefMessage, FullServiceMsg.pBuffer, FullServiceMsg.nBufferLen - 1);

          FullServiceMsgText_New := FullServiceMsgText_New + S;
        end;

        FreeMem(FullServiceMsg.pBuffer, FullServiceMsg.nBufferLen);
      end;

      FreeMem(FullServiceMsg);
      FFullServiceMsgList.Delete(0);

      Inc(Count);
    end;
  finally
    FFullServiceMsgList.Unlock;
  end;

  List := TList.Create;
  try
    TIocpClientContextPool.Instance.GetOnlineContextList(List);
    for I := List.Count - 1 downto 0 do
    begin
      MirContext := List.Items[I];
      if (MirContext.Socket = INVALID_SOCKET) then
      begin
        TIocpClientContextPool.Instance.TryCloseContext(MirContext);
      end
      else
      begin
        if MirContext.boIsOldClient then
        begin
          //MirContext.Run(FullServiceMsgText_Old)
        end
        else
        begin
          MirContext.Run(FullServiceMsgText_New);
        end;

{$IF CLIENT_ANTIPLUG = 1}
        if (g_ClientAntiPlugDllSize > 0) and (tick_diff(FLastSendAntiPlugTick, MyGetTickCount) >= 500) then
        begin
          if MirContext.boFirstClientQueryBagItems and
            (not MirContext.IsPostedCloseQuest) and
            (not MirContext.boDelayClose) then
          begin
            if (MirContext.dwClientAntiPlugVersion <> g_ClientAntiPlugVersion) then
            begin
              if (not g_boLogoutNoResendAntiplugStream) or (MirContext.dwRecvClientAntiplugCRC <> g_ClientAntiPlugDllStringCRC) then
              begin
                MirContext.SendAntiPlugStreamInfo;
                FLastSendAntiPlugTick := GetTickCountEx;
              end
              else if (g_RunGatePlugDllHandle <> 0) then
              begin
                MirContext.SendAntiPlugStreamLoadCache;
              end;
            end;

            // 卸载插件后，未更新直接发缓存加载 2020-01-07
            if (g_RunGatePlugDllHandle <> 0) and
              MirContext.boWaitLoadAntiPlug and (tick_diff(MirContext.dwWaitLoadAntiPlugTick, MyGetTickCount) >= 6000) and
              (MirContext.dwClientAntiPlugVersion = g_ClientAntiPlugVersion) then
            begin
              MirContext.SendAntiPlugStreamLoadCache;
            end;
          end;
        end;
      end;
{$IFEND}
    end;
  finally
    List.Free;
  end;
end;
{$IFEND}

procedure TRunGateManager.AddFullServiceMsg(DefMessage: PTDefaultMessage; Buffer: PChar; BufSize: Integer);
var
  FullServiceMsg: PClientMsg;
begin
  GetMem(FullServiceMsg, SizeOf(TClientMsg));

  FullServiceMsg.pBuffer := nil;
  FullServiceMsg.nBufferLen := BufSize;

  FullServiceMsg.DefMessage := DefMessage^;
  if BufSize > 0 then
  begin
    GetMem(FullServiceMsg.pBuffer, BufSize);
    Move(Buffer^, FullServiceMsg.pBuffer^, BufSize);
  end;

  FFullServiceMsgList.Lock;
  try
    FFullServiceMsgList.Add(FullServiceMsg);
  finally
    FFullServiceMsgList.UnLock;
  end;
end;

procedure TRunGateManager.ClearFullServiceMsgList;
var
  I: Integer;
  FullServiceMsg: PClientMsg;
begin
  FFullServiceMsgList.Lock;
  try
    for I := 0 to FFullServiceMsgList.Count - 1 do
    begin
      FullServiceMsg := FFullServiceMsgList.Items[I];
      if (FullServiceMsg.pBuffer <> nil) and (FullServiceMsg.nBufferLen > 0) then
      begin
        FreeMem(FullServiceMsg.pBuffer, FullServiceMsg.nBufferLen);
      end;

      FreeMem(FullServiceMsg);
    end;

    FFullServiceMsgList.Clear;
  finally
    FFullServiceMsgList.Unlock;
  end;
end;

procedure TRunGateManager.OnIocpError(Sender: TIocpCore; ErrorStr: string; ErrorType: TIocpErrorType; ErrorCode: Integer);
begin
  if ErrorType = Iocpet_Info then
    AddIocpLogMsg(ErrorStr)
  else
    AddIocpLogMsg(Format('[%s:%d] %s', ['错误', ErrorCode, ErrorStr]));
end;

{$IF NEED_REGISTER = 1}
procedure TRunGateManager.OnClientSocetkVerifyConnect(Sender: TObject;
  Socket: TCustomWinSocket);
//var
//  I, ExtendedInfo: Integer;
//  RungateVerifyHeader: TRungateVerifyHeader;
//  IsOK: Boolean;
//  C1, C2: Char;
//  KeyFlag: LongWord;
//  LicenseDataInfo: TLicenseDataInfo;
//  VerifyData: TRungateVerifyData_New;
//
//  HWID1, HWID2: array[0..100] of Char;
//  sHWID1, sHWID2: string;
//  UserName: array[0..100] of Char;
//  Organization: array[0..100] of Char;
//  CustomData: array[0..102400] of Char;
//  MachineID: array[0..7] of DWORD;
//  S, TempStr, Password: string;
//  Len: Integer;
//
//  FSetting : TFormatSettings;
//  Y, M, D: Word;
begin
//{$I Registered_Start.inc}
//  FVerifyDataText := '';
//
//  if (WLRegGetStatus(ExtendedInfo) = wlIsRegistered) then
//  begin
//    WLHardwareGetID(HWID1);
//    WLRegGetLicenseHardwareID(HWID2);
//
//    sHWID1 := StrPas(HWID1);
//    sHWID2 := StrPas(HWID2);
//
//    if SameText(sHWID1, sHWID2) then
//    begin
//      FillChar(VerifyData, SizeOf(VerifyData), 0);
//      FillChar(UserName, SizeOf(UserName), 0);
//      FillChar(Organization, SizeOf(Organization), 0);
//
//      WLRegGetLicenseInfo(UserName, Organization, CustomData);
//      S := StrPas(CustomData);
//
//      if (Length(S) > 0) and (Length(S) mod 2 = 0) then
//      begin
//        Len := Length(S) div 2;
//        SetLength(TempStr, Len);
//
//        IsOK := False;
//        if Len = SizeOf(TLicenseDataInfo) then
//        begin
//          IsOK := True;
//          for I := 1 to Len do
//          begin
//            C1 := S[I * 2 - 1];
//            C2 := S[I * 2];
//            if (C1 in ['0'..'9', 'a'..'f', 'A'..'F']) and (C2 in ['0'..'9', 'a'..'f', 'A'..'F']) then
//            begin
//              TempStr[I] := Chr(StrToInt('$' + C1 + C2))
//            end
//            else
//            begin
//              IsOK := False;
//              Break;
//            end;
//          end;
//        end;
//
//        {$IF REGISTER_TEST = 1}
//          AddMainLogMsg('~~~~~~~~~~~~~~~~~~~~~~~~~~连接成功，发送验证数据', 1);
//        {$IFEND}
//
//        if IsOK then
//        begin
//
//      {$IF RungateLEG_IOCP = 1}
//          RungateVerifyHeader.dwCode := $3B21D6AF;
//      {$ELSEIF RungateLEG_IOCP = 2}
//          RungateVerifyHeader.dwCode := $82ABACA3;
//      {$ELSEIF RungateLEG_IOCP = 3}
//          RungateVerifyHeader.dwCode := $328C9341;
//      {$ELSEIF RungateLEG_IOCP = 4}
//          RungateVerifyHeader.dwCode := $0DCAF30B;
//      {$ELSEIF RungateLEG_IOCP = 5}
//          RungateVerifyHeader.dwCode := $7B1E4CC7;
//      {$IFEND}
//
//          RungateVerifyHeader.dwCmd := 302;
//          RungateVerifyHeader.nLength := SizeOf(TRungateVerifyHeader);
//
//          for I := 0 to 7 do
//          begin
//            MachineID[I] := StrToIntDef('$' + Copy(HWID1, I * 5 + 1, 4), 0);
//          end;
//
//          Randomize;
//          FVerifyDataKey := Random(High(Integer));
//
//          VerifyData.Key := FVerifyDataKey;
//          Move(Organization[0], VerifyData.IP[0], SizeOf(VerifyData.IP));
//          Move(MachineID[0], VerifyData.HWID[0], SizeOf(VerifyData.HWID));
//
//          FSetting.ShortDateFormat := 'yyyy-MM-dd';
//          FSetting.DateSeparator := '-';
//          DecodeDate(StrToDate(g_sUpdateTime, FSetting), Y, M, D);
//
//          VerifyData.UpdateDate := Y * 10000 + M * 100 + D;
//          VerifyData.Version := 20;
//
//          SetLength(S, SizeOf(TRungateVerifyHeader) + SizeOf(VerifyData));
//          Move(RungateVerifyHeader, S[1], SizeOf(TRungateVerifyHeader));
//          Move(VerifyData, S[1 + SizeOf(TRungateVerifyHeader)], SizeOf(VerifyData));
//
//          Socket.SendBuf(S[1], Length(S));
//        end;
//      end;
//    end;
//  end;
//{$I Registered_End.inc}
end;

procedure TRunGateManager.OnClientSocetkVerifyError(Sender: TObject;
  Socket: TCustomWinSocket; ErrorEvent: TErrorEvent; var ErrorCode: Integer);
begin
//  ErrorCode := 0;
//  Socket.Close;
end;

procedure TRunGateManager.OnClientSocetkVerifyRead(Sender: TObject;
  Socket: TCustomWinSocket);
//var
//  I: Integer;
//  RSA: TLbRSA;
//  OutBuf: array[0..512] of Byte;
//  OutBufSize: Integer;
//  KeyDataInfo: TRemoteKeyDataInfo;
//  SrvMsgFlag: TServerMessageFlag;
//  ClientMsgFlag: TClientMessageFlag;
//  Hash, EncKey: DWORD;
//  sKey: string;
//  nRemoteIP: Integer;
//
//  RungateVerifyHeader: PRungateVerifyHeader;
begin
{$I VMProtectBeginUltra.inc}
//  FVerifyDataText := FVerifyDataText + Socket.ReceiveText;
//  if Length(FVerifyDataText) >= SizeOf(TRungateVerifyHeader) then
//  begin
//    RungateVerifyHeader := PRungateVerifyHeader(@FVerifyDataText[1]);
//  {$IF RungateLEG_IOCP = 1}
//    if RungateVerifyHeader.dwCode = $3B21D6AF then
//  {$ELSEIF RungateLEG_IOCP = 2}
//    if RungateVerifyHeader.dwCode = $82ABACA3 then
//  {$ELSEIF RungateLEG_IOCP = 3}
//    if RungateVerifyHeader.dwCode = $328C9341 then
//  {$ELSEIF RungateLEG_IOCP = 4}
//    if RungateVerifyHeader.dwCode = $0DCAF30B then
//  {$ELSEIF RungateLEG_IOCP = 5}
//    if RungateVerifyHeader.dwCode = $7B1E4CC7 then
//  {$IFEND}
//    begin
//      if SizeOf(TRungateVerifyHeader) + RungateVerifyHeader.nLength = Length(FVerifyDataText) then
//      begin
//{$IF REGISTER_TEST = 1}
//        AddMainLogMsg('~~~~~~~~~~~~~~~~~~~~~~~~~~~cmd: ' + IntToStr(RungateVerifyHeader.dwCmd), 0);
//{$IFEND}
//
//        if (RungateVerifyHeader.dwCmd = 302) then
//        begin
//          if (RungateVerifyHeader.nLength = 256) then
//          begin
//            RSA := TLbRSA.Create(nil);
//            try
//              RSA.KeySize := aks1024;
//
//            {$IF RungateLEG_IOCP = 1}
//              RSA.PrivateKey.ModulusAsString :=
//                'FB5A3530C31C88AD029C766B782EB3B953782CCCD29E17FA21EBD3A1CF44B1C4D' +
//                '221CA0C93A178273AEE588E55431A6B68EFE92E5114EF351920F439D771AD64A9' +
//                '516E1B7B56B827BB60B02AB06AAE962BD5A43F1CDC3EDF19B133CDD1683C737D6' +
//                'CE1DDC943BC60621938AB937AB8AD202616832D2D29D3C5095A70D366007A';
//              RSA.PrivateKey.ExponentAsString := '9931';
//            {$ELSEIF RungateLEG_IOCP = 2}
//              RSA.PrivateKey.ModulusAsString :=
//                '91142D1CAB440307331FEE16A5685B8DAF90DB259DCE6481910D5F9F13F4FB0F8' +
//                '4301D79C338637D8CEEC30E9848D7B803A250021572C2775055E39D007C31CB48' +
//                '7D192B8062C7F8173C8B89A3DF9A65AA15BC6F24316A3628DF58A2A3A4CF31ABE' +
//                'C7C5D68C389EDF7E07AFE3997C62A114B27B9D9274B4ECE0FC50DCF61F1B2';
//              RSA.PrivateKey.ExponentAsString := '653F';
//            {$ELSEIF RungateLEG_IOCP = 3}
//              RSA.PrivateKey.ModulusAsString :=
//                'F5608D27BE08D11912C4E0B238C3E6F6E7AD2A68473BCD9D5113F098FC5F572D7' +
//                '4AB1965E25447A03DC275FEA33F740E5AC436ABF6EA3345680C94DC6866DBBB26' +
//                '2C9B1CBED987ED20919DF9BB217FFC33A6F0F841D03DC4D785A3775586BC789E9' +
//                'BC380A0B93BDAF3D1A755305EF99C704142C63CC5097296DA181DFD87BC9A';
//              RSA.PrivateKey.ExponentAsString := '1B40';
//            {$ELSEIF RungateLEG_IOCP = 4}
//              RSA.PrivateKey.ModulusAsString :=
//                '91007E1306C028EB47527A2A67034CFFC08B1D86DBA8EA4578DB425FEAC4D59F' +
//                '3DFE1E637016C2A6B231A283E91310FDC3B0F6F14044EAB37DCBF0F1D5FF142F' +
//                '9ADFE0A4183DF1EAE9BFAE94D8DEB999B41582E57FF86BE6270546D7B71E1190' +
//                '0A2F1004FB0E736E21225E877E61B92D82999AF8D330528422EE8DBD22E42988';
//              RSA.PrivateKey.ExponentAsString := '4F1C';
//            {$ELSEIF RungateLEG_IOCP = 5}
//              RSA.PrivateKey.ModulusAsString :=
//                'C50E56EFDFC4C5A50729FDCBA25D5EF2A5A51366387B6BDB490A5B66C95AD446' +
//                '2E79784F505D9A2E48EF773DFDAE1700BCAFEF288CB68EBDA939171B71F1365C' +
//                '4F28EC8CED8CD0E8F1970730E5FF41472FCC915CC146A7CF2D16B8EC97B0B651' +
//                '1AF74B60D41D59ECABF8C792457F97DE0CFFE6FF8869FD0D619D17BA299CDBA2';
//              RSA.PrivateKey.ExponentAsString := '172C';
//            {$IFEND}
//            
//              try
//                OutBufSize := RSA.DecryptBuffer(FVerifyDataText[1 + SizeOf(TRungateVerifyHeader)], Length(FVerifyDataText) - SizeOf(TRungateVerifyHeader), OutBuf[0]);
//              except
//                OutBufSize := 0;
//              end;
//
//              if OutBufSize = SizeOf(TRemoteKeyDataInfo) then
//              begin
//                Move(OutBuf[0], KeyDataInfo, OutBufSize);
//                if (KeyDataInfo.SrvMsgFlagCRC = Crc32(@KeyDataInfo.SrvMsgFlag, SizeOf(TServerMessageFlag))) then
//                begin
//                  Hash := $A7EC4BA6;
//                  for I := Low(KeyDataInfo.RandomCode1Arr) to High(KeyDataInfo.RandomCode1Arr) - 1 do
//                  begin
//                    Hash := Hash + (Hash shl 5) + KeyDataInfo.RandomCode1Arr[I];
//                  end;
//
//                  SetLength(sKey, 16);
//                  Move(Hash, sKey[1], SizeOf(Hash));
//
//                  EncKey := ((FVerifyDataKey shl 3) xor Hash) + KeyDataInfo.RandomCode1Arr[8];
//                  Move(EncKey, sKey[5], SizeOf(EncKey));
//
//                  EncKey := ((FVerifyDataKey and Hash) shr 1) or KeyDataInfo.RandomCode1Arr[9] or KeyDataInfo.RandomCode1Arr[5] or KeyDataInfo.RandomCode1Arr[3] or KeyDataInfo.RandomCode1Arr[1];
//                  Move(EncKey, sKey[9], SizeOf(EncKey));
//
//                  nRemoteIP := Socket.RemoteAddr.sin_addr.S_addr;
//                  Move(nRemoteIP, sKey[13], SizeOf(nRemoteIP));
//
//
//                  DecryptDes_New(KeyDataInfo.SrvMsgFlag, SrvMsgFlag, SizeOf(TServerMessageFlag), sKey);
//
//                  GM_DATA := SrvMsgFlag.gmDATA;
//                  GM_KICK := SrvMsgFlag.gmKICK;
//                  GM_COMPDATA := SrvMsgFlag.gmCOMPDATA;
//                  GM_DATA_CACHE := SrvMsgFlag.gmDataCache;
//                  GM_NO_CERTIFICATION := SrvMsgFlag.gmNoCertification;
//                  GM_FULL_SERVICE_MSG := SrvMsgFlag.gmFullServiceMsg;
//
//                  FTimeLeftStartTick := MyGetTickCount;
//                  FTimeLeft := KeyDataInfo.TimeLeft;
//                  FIsShowTimeLeft := True;
//
//  {$IF REGISTER_TEST = 1}
//                  AddMainLogMsg('~~~~~~~~~~~~Verify server ip:' + IntToStr(nRemoteIP), 0);
//                  AddMainLogMsg('~~~~~~~~~~~~gmNoCertification:' + IntToStr(SrvMsgFlag.gmNoCertification), 0);
//  {$IFEND}
//                end;
//
//                if (KeyDataInfo.ClientMsgFlagCRC = Crc32(@KeyDataInfo.ClientMsgFlag, SizeOf(TClientMessageFlag))) then
//                begin
//                  Hash := 0;
//                  for I := Low(KeyDataInfo.RandomCode1Arr) to High(KeyDataInfo.RandomCode1Arr) do
//                  begin
//                    Hash := KeyDataInfo.RandomCode1Arr[I] + (Hash shl 6) + (Hash shl 16) - Hash;
//                  end;
//
//                  SetLength(sKey, 16);
//                  Move(Hash, sKey[1], SizeOf(Hash));
//
//                  EncKey := (FVerifyDataKey shl 5) and (KeyDataInfo.RandomCode1Arr[8] shl 1) or (Hash shr 4) and (KeyDataInfo.RandomCode1Arr[2] shr 3);
//                  Move(EncKey, sKey[5], SizeOf(EncKey));
//
//                  EncKey := (FVerifyDataKey shl (Hash and $F)) and KeyDataInfo.RandomCode1Arr[12] xor KeyDataInfo.RandomCode1Arr[15];
//                  Move(EncKey, sKey[9], SizeOf(EncKey));
//
//                  EncKey := KeyDataInfo.RandomCode1Arr[10] or (KeyDataInfo.RandomCode1Arr[1] shl 2);
//                  Move(EncKey, sKey[13], SizeOf(EncKey));
//
//                  DecryptDes_New(KeyDataInfo.ClientMsgFlag, ClientMsgFlag, SizeOf(TClientMessageFlag), sKey);
//
//                  CM_SOFTCLOSE := ClientMsgFlag.cmSoftClose; // 小退
//                  CM_SAY := ClientMsgFlag.cmSay; // 说话
//                  CM_QUERYBAGITEMS := ClientMsgFlag.cmQueryBagItems; // 查询包裹
//                  CM_DEALTRY := ClientMsgFlag.cmDealTry; // 尝试交易
//                  CM_CHALLENGETRY := ClientMsgFlag.cmChallengeTry; // 请求挑战
//
//                  CM_SPELL := ClientMsgFlag.cmSpell; // 魔法
//
//                  CM_HIT := ClientMsgFlag.cmHit; // 物理攻击
//                  CM_HEAVYHIT := ClientMsgFlag.cmHeavyHit; // 跳起来砍
//                  CM_BIGHIT := ClientMsgFlag.cmBigHit; // 强攻
//                  CM_POWERHIT := ClientMsgFlag.cmPowerHit; // 攻杀
//                  CM_LONGHIT := ClientMsgFlag.cmLongHit; // 刺杀
//                  CM_WIDEHIT := ClientMsgFlag.cmWideHit; // 半月
//                  CM_FIREHIT := ClientMsgFlag.cmFireHit; // 烈火
//                  CM_CRSHIT := ClientMsgFlag.cmCrsHit; // 抱月
//                  CM_TWNHIT := ClientMsgFlag.cmTwnHit; // 龙影
//                  CM_43HIT := ClientMsgFlag.cm43HIT; // 雷霆剑法
//                  CM_SWORDHIT := ClientMsgFlag.cmSwordHit; // 逐日剑法     ID=56
//                  CM_66HIT := ClientMsgFlag.cm66HIT; // 开天斩
//                  CM_66HIT1 := ClientMsgFlag.cm66HIT1; // 开天斩
//                  CM_101HIT := ClientMsgFlag.cm101HIT; // 三绝杀
//                  CM_102HIT := ClientMsgFlag.cm102HIT; // 断岳斩
//                  CM_103HIT := ClientMsgFlag.cm103HIT; // 横扫千军
//                  // CM_CUSTOM_HIT1 := ClientMsgFlag.cmCustomHit1; // 自定义技能1
//                  // CM_CUSTOM_HIT100 := ClientMsgFlag.cmCustomHit100; // 自定义技能100
//
//                  CM_WALK := ClientMsgFlag.cmWalk; // 走路
//                  CM_RUN := ClientMsgFlag.cmRun; // 跑步
//                  CM_TURN := ClientMsgFlag.cmTurn; // 转弯
//                  CM_SITDOWN := ClientMsgFlag.cmSitdown; // 挖肉
//                  CM_DROPITEM := ClientMsgFlag.cmDropItem; // 丢物品
//                  CM_PICKUP := ClientMsgFlag.cmPickUp; // 捡起
//
//  {$IF REGISTER_TEST = 1}
//                  AddMainLogMsg('~~~~~~~~~~~~cmPickUp:' + IntToStr(ClientMsgFlag.cmPickUp), 0);
//                  AddMainLogMsg('~~~~~~~~~~~~cmCustomHit100:' + IntToStr(ClientMsgFlag.cmCustomHit100), 0);
//  {$IFEND}
//                end;
//              end;
//            finally
//              RSA.Free;
//            end;
//          end;
//        end
//        else if (RungateVerifyHeader.dwCmd = 1111) then
//        begin
//          Move(OutBufSize, RUNGATECODE, 100);
//          GM_DATA := 10 + Random(10000);
//
//{$IF REGISTER_TEST = 1}
//          AddMainLogMsg('~~~~~~~~~~~~~~~~~~~~~~~~~~~复位数据RUNGATECODE' + IntToStr(RUNGATECODE), 0);
//{$IFEND}
//        end
//        else if (RungateVerifyHeader.dwCmd = 8888) then         // 标题显示
//        begin
//          if (RungateVerifyHeader.nLength > 0) then
//          begin
//            SetLength(sKey, RungateVerifyHeader.nLength);
//            Move(FVerifyDataText[1 + SizeOf(TRungateVerifyHeader)], sKey[1], RungateVerifyHeader.nLength);
//            Application.MainForm.Caption := Application.MainForm.Caption + sKey;
//          end;
//        end
//        else if (RungateVerifyHeader.dwCmd = 9999) then         // 日志显示
//        begin
//          if (RungateVerifyHeader.nLength > 0) then
//          begin
//            SetLength(sKey, RungateVerifyHeader.nLength);
//            Move(FVerifyDataText[1 + SizeOf(TRungateVerifyHeader)], sKey[1], RungateVerifyHeader.nLength);
//
//            AddMainLogMsg(sKey, 0);
//          end;
//        end;
//
//        FVerifyDataText := '';
//        FIsVerifyServerResult := True;
//        FIsRetryVerifyServerResult := True;
//        FRetryConnectVerifyServerCount := 0;
//      end;
//    end;
//  end;
{$I VMProtectEnd.inc}
end;
{$IFEND}

procedure TRunGateManager.GetOnlineUser(List: TList);
var
  I, J: Integer;
  RunGate: TRunGate;
begin
  for I := 0 to FRunGateList.Count - 1 do
  begin
    RunGate := FRunGateList.Items[I];
    RunGate.FOnlineUser.Lock;
    try
      for J := 0 to RunGate.FOnlineUser.Count - 1 do
      begin
        List.Add(RunGate.FOnlineUser.Items[J]);
      end;
    finally
      RunGate.FOnlineUser.UnLock;
    end;
  end;
end;


end.
