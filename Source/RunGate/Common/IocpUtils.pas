unit IocpUtils;

interface

{$I Iocp.inc}

uses
  Classes, SysUtils, Windows, SyncObjs, IocpWinsock2, IocpCommon,
  Forms, IODataPool;

type
  TCloseFrom = (cfOther, cfPostWSASendCache1, cfPostWSASendCache2, cfProcessIOQueued);

  TIocpCore = class;

  TIocpThread = class(TThread)
    FIocpCore: TIocpCore;
  public
    constructor Create(AIocpCore: TIocpCore);
    destructor Destroy; override;
  end;

  // Iocp数据处理线程
  TWorkerThread = class(TIocpThread)
  private
    FIocpHandle: THandle;
    FWorkID: Cardinal;

    FIsRun: Boolean;
  protected
    procedure Execute; override;
  end;

  TIocpContext = class(TObject)
  private
    FSendCache: TSafeList;                    // 发送缓存<PSendBuffer>列表
    FCurrentSendBuffer: PSendBuffer;          // 当前正在投递的数据

    FRecvBuffers: string;
    FRecvBuffersLocker: TIocpCriticalSection;
                                              
    // 0缓冲区解锁
    FZeroBytesReadTick: LongWord;

    // 从缓存中投开始投递一块数据
    function CheckPostWSASendCache: Integer;
    function CheckPostWSASendCacheBeforeLock: Integer;
  private
    FIsBusying: Boolean;                     // 是否正在忙
    FIsWaitingGiveBack: Boolean;             // 等待回收标记,等忙完进行回收
    FIsPostedCloseQuest: Boolean;            // 已经投递了关闭请求

    FIocpCore: TIocpCore;

    FSocket: TSocket;

    FDataLogLocker: TIocpCriticalSection;

    FSendBlockCount: Integer;                // 发送数据块次数
    FSendBytesSize: Int64;                   // 发送数据包大小
    FRecvBlockCount: Integer;                // 接收数据块次数
    FRecvBytesSize: Int64;                   // 接收数据名大小

    F10035ErrorCount: Integer;               // WSAEWOULDBLOCK 错误次数

    FLastSendDataTick: DWORD;
    FLastRecvDataTick: DWORD;
    FConnectionTick: DWORD;

    // 投递一个关闭请求
    function PostWSAClose: Boolean;
    function GetUsing: Boolean;
  protected
    procedure DoRecvBuffer(Buffer: PAnsiChar; Len: Cardinal);
    procedure AddToRecvBuffer(Buffer: PAnsiChar; Len: Cardinal);
  protected
    // 关闭客户端连接
    procedure DoConnect; virtual;
    procedure DoDisconnect(ASocket: TSocket); virtual;

    procedure DoReset(IsClose: Boolean); virtual;
    procedure CloseContextSocket(ErrCode: Integer; CloseFrom: TCloseFrom); virtual;

    function DoCheckRecvBuffer(var S: string): Boolean; virtual;
    function DoZeroBytesRead(lvIOData: POVERLAPPEDEx): Boolean;
  public
    constructor Create(AIocpCore: TIocpCore; ASocket: TSocket = INVALID_SOCKET); virtual;

    destructor Destroy; override;

    procedure NotifyStopWork; virtual;

    // 清理发送缓存区
    procedure ClearSendCache;
    procedure ClearSendCacheUnLock;

    procedure Close;

    procedure PostSendBuffer(const Buffer: PAnsiChar; Len: Cardinal);
    procedure PostSendText(const S: string);

    property IsUsing: Boolean read GetUsing;
    property IsBusying: Boolean read FIsBusying;

    property IsPostedCloseQuest: Boolean read FIsPostedCloseQuest write FIsPostedCloseQuest;
    property IsWaitingGiveBack: Boolean read FIsWaitingGiveBack write FIsWaitingGiveBack;

    property IocpCore: TIocpCore read FIocpCore write FIocpCore;
    property Socket: TSocket read FSocket write FSocket;

    property SendBlockCount: Integer read FSendBlockCount;
    property SendBytesSize: Int64 read FSendBytesSize;
    property RecvBlockCount: Integer read FRecvBlockCount;
    property RecvBytesSize: Int64 read FRecvBytesSize;

    property ConnectionTick: DWORD read FConnectionTick;
    property LastSendDataTick: DWORD read FLastSendDataTick;
    property LastRecvDataTick: DWORD read FLastRecvDataTick;

    // 缓存解锁
    property ZeroBytesReadTick: DWORD read FZeroBytesReadTick;
  end;

  TDataSendRecvLog = record
    SendBlockCount: Integer;
    SendBytesSize: Int64;
    RecvBlockCount: Integer;
    RecvBytesSize: Int64;
  end;

  TIocpDataBufferEvent = procedure(Sender: TIocpCore; Context: TIocpContext; const Buffer: PChar; const BufferLen: Cardinal; const WorkerID: Cardinal) of object;
  TIocpErrorEvent = procedure(Sender: TIocpCore; ErrorStr: string; ErrorType: TIocpErrorType; ErrorCode: Integer) of object;
  TIocpContextConnectEvent = procedure(Sender: TIocpCore; Context: TIocpContext) of object;
  TIocpContextDisonnectEvent = procedure(Sender: TIocpCore; Context: TIocpContext; Socket: TSocket) of object;

  TIocpCore = class(TObject)
  private
    FOwner: TObject;

    // Iocp句柄
    FIocpHandle: THandle;

    //服务端套接字
    FSocket: TSocket;

    FIsActive: Boolean;

    FWorkerThreadList: TList;

    // 接收数据块默认大小
    FRecvDataDefaultSize: Cardinal;

  {$IFNDEF SHARE_POOL_MODE}
    FIODataPool: TIODataPool;
  {$ENDIF}

    FWSARecv_dwFlags: Cardinal;

    FCumulativeDataSendRecvLog: TDataSendRecvLog;
    FRealTimeDataSendRecvLog: TDataSendRecvLog;

    // 数据包发送接收记录锁

    FDataLogLocker: TIocpCriticalSection;
    FEventLocker: TIocpCriticalSection;

    FOnContextConnect: TIocpContextConnectEvent;
    FOnContextDisconnect: TIocpContextDisonnectEvent;
    FOnRecvDataBuffer: TIocpDataBufferEvent;
    FOnSendDataBuffer: TIocpDataBufferEvent;
    FOnError: TIocpErrorEvent;

    function GetWorkerThreadCount: Integer;
    procedure SetWorkerThreadCount(const Value: Integer);
  protected
    function GetIsFreeIOData(IoDataAllocSize: Cardinal): Boolean; virtual;
    procedure DoContextConnect(Context: TIocpContext); virtual;
    procedure DoContextDisconnect(Context: TIocpContext; ASocket: TSocket); virtual;
    procedure DoError(ErrorStr: string; ErrorType: TIocpErrorType; ErrorCode: Integer = 0); virtual;
    procedure DoRecvDataBuffer(Context: TIocpContext; const Buffer: PChar; const BufferLen: Cardinal; const WorkerID: Cardinal); virtual;
    procedure DoSendDataBuffer(Context: TIocpContext; const Buffer: PChar; const BufferLen: Cardinal; const WorkerID: Cardinal); virtual;
  public
    constructor Create(AOwner: TObject = nil);
    destructor Destroy; override;
    procedure ClearRealTimeDataSendRecvLog;

    property Owner: TObject read FOwner;
    property IocpHandle: THandle read FIocpHandle;
    property Socket: TSocket read FSocket write FSocket;

    property RecvDataDefaultSize: Cardinal read FRecvDataDefaultSize write FRecvDataDefaultSize;

  {$IFNDEF SHARE_POOL_MODE}
    property IODataPool: TIODataPool read FIODataPool;
  {$ENDIF}

    property OnContextConnect: TIocpContextConnectEvent read FOnContextConnect write FOnContextConnect;
    property OnContextDisconnect: TIocpContextDisonnectEvent read FOnContextDisconnect write FOnContextDisconnect;
    property OnRecvDataBuffer: TIocpDataBufferEvent read FOnRecvDataBuffer write FOnRecvDataBuffer;
    property OnSendDataBuffer: TIocpDataBufferEvent read FOnSendDataBuffer write FOnSendDataBuffer;
    property OnError: TIocpErrorEvent read FOnError write FOnError;

    property WorkerThreadCount: Integer read GetWorkerThreadCount write SetWorkerThreadCount;

    property CumulativeDataSendRecvLog: TDataSendRecvLog read FCumulativeDataSendRecvLog;
    property RealTimeDataSendRecvLog: TDataSendRecvLog read FRealTimeDataSendRecvLog;
  public
    function ProcessIOQueued(const WorkerID: Cardinal): Boolean;
    function PostWSARecv(Context: TIocpContext): Boolean;
    function PostWSASend(Context: TIocpContext; IOData: POVERLAPPEDEx): Integer;
    function PostWSAClose(Context: TIocpContext): Boolean;
    function PostExitIO: Boolean;
  end;

implementation

uses
  Grobal2_Ex;

const
  MaxInt64: Int64 = 4611686018427387904;

var
  _SocketStartup: Boolean = False;

{ TIocpThread }

constructor TIocpThread.Create(AIocpCore: TIocpCore);
begin
  inherited Create(True);
  FIocpCore := AIocpCore;
end;

destructor TIocpThread.Destroy;
begin
  inherited Destroy;
end;  

{ TWorkerThread }

procedure TWorkerThread.Execute;
begin
  inherited;
  FIsRun := True;
  try
    while not Terminated do
    begin
      if not FIocpCore.ProcessIOQueued(FWorkID) then
      begin
        FIocpCore.DoError('TWorkerThread.Execute ProcessIOQueued exit', Iocpet_Info);
        Exit;
      end;
    end;
  finally
    FIsRun := False;
  end;
end;

//------------------------------------------------------------------------------

{ TIocpContext }


constructor TIocpContext.Create(AIocpCore: TIocpCore; ASocket: TSocket);
begin
  FSendCache := TSafeList.Create({$IFDEF USE_SPINLOCK}'SendCacheLocker'{$ENDIF});                 // 发送缓存<PSendBuffer>列表

  FCurrentSendBuffer := nil;                      // 当前正在投递的数据
  FRecvBuffers := '';

  FRecvBuffersLocker := TIocpCriticalSection.Create({$IFDEF USE_SPINLOCK}'RecvBuffersLocker'{$ENDIF});

  FIocpCore := AIocpCore;

  FSocket := ASocket;

  FDataLogLocker := TIocpCriticalSection.Create({$IFDEF USE_SPINLOCK}'DataLogLocker_1'{$ENDIF});

  FSendBlockCount := 0;
  FSendBytesSize := 0;
  FRecvBlockCount := 0;
  FRecvBytesSize := 0;

  F10035ErrorCount := 0;

  FLastSendDataTick := MyGetTickCount;
  FLastRecvDataTick := MyGetTickCount;
  FConnectionTick := MyGetTickCount;

  FZeroBytesReadTick := MyGetTickCount;
end;

destructor TIocpContext.Destroy;
var
  lvSocket: TSocket;
begin
  ClearSendCacheUnLock;

  if FSocket <> INVALID_SOCKET then
  begin
    lvSocket := FSocket;
    FSocket := INVALID_SOCKET;
    CloseSocket(lvSocket);   //ERROR_SUCCESS
  end;

  FRecvBuffersLocker.Free;
  FSendCache.Free;
  FDataLogLocker.Free;

  inherited;
end;

procedure TIocpContext.AddToRecvBuffer(Buffer: PAnsiChar; Len: Cardinal);
var
  S: string;
  ErrNum: Integer;
begin
  ErrNum := 0;
  //加入到套接字对应的缓存
  try
    ErrNum := 1;
    SetLength(S, Len);
    ErrNum := 2;
    Move(Buffer^, S[1], Len);
    ErrNum := 3;
    FRecvBuffers := FRecvBuffers + S;
  except
    on E: Exception do
    begin
      if FIocpCore <> nil then
        FIocpCore.DoError('TIocpContext.AddToRecvBuffer error. ' + E.Message, Iocpet_Error, ErrNum);
    end;
  end;
end;

function TIocpContext.CheckPostWSASendCache: Integer;
var
  lvIOData: POVERLAPPEDEx;
  lvRet: Integer;
  ReadCount: Cardinal;
begin
  lvRet := ERROR_SUCCESS;
  if FIsPostedCloseQuest then 
  begin
    Result := ERROR_SUCCESS;
    Exit;
  end;

  FSendCache.Lock;
  try
    // 检测缓存中是否有需要发送的数据
    if FCurrentSendBuffer = nil then
    begin
      if FSendCache.Count > 0 then
      begin
        FCurrentSendBuffer := FSendCache.Items[0];
      end;
    end;

    // 发送一块内存块
    if (FCurrentSendBuffer <> nil) and (FCurrentSendBuffer.BufSize > 0) and (FCurrentSendBuffer.Position < FCurrentSendBuffer.BufSize) then
    begin
      ReadCount := FCurrentSendBuffer.BufSize - FCurrentSendBuffer.Position;
      if ReadCount > MAX_OVERLAPPEDEX_BUFFER_SIZE shl 10 then
      begin
        ReadCount := MAX_OVERLAPPEDEX_BUFFER_SIZE shl 10;
      end;

    {$IFDEF SHARE_POOL_MODE}
      lvIOData := TIODataPool.Instance.GetNewIOData(ReadCount);                   // 保证是4的倍数，不要???
    {$ELSE}
      lvIOData := FIocpCore.FIODataPool.GetNewIOData(ReadCount);                  // 保证是4的倍数，不要???
    {$ENDIF}

      lvIOData.IoType := itSend;

      Move(PChar(Cardinal(FCurrentSendBuffer.Buffer) + FCurrentSendBuffer.Position)^, lvIOData.DataBuf.buf^, ReadCount);
      FCurrentSendBuffer.Position := FCurrentSendBuffer.Position + ReadCount;

      lvIOData.DataBuf.len := ReadCount;

      // 发送一个内存块
      lvRet := FIocpCore.PostWSASend(Self, lvIOData);

      if lvRet <> ERROR_SUCCESS then
      begin
        // 发送不成功
      {$IFDEF SHARE_POOL_MODE}
        TIODataPool.Instance.GiveBackIOData(lvIOData);
      {$ELSE}
        FIocpCore.FIODataPool.GiveBackIOData(lvIOData);
      {$ENDIF}
      end;
    end;

    // 如果数据都发送完成从发送缓存中移除
    if (FCurrentSendBuffer <> nil) and ((FCurrentSendBuffer.BufSize = 0) or (FCurrentSendBuffer.BufSize = FCurrentSendBuffer.Position)) then
    begin
      FSendCache.Remove(FCurrentSendBuffer);

      // 释放发送的内存块
      FreeMem(FCurrentSendBuffer.Buffer, FCurrentSendBuffer.BufSize);
      FreeMem(FCurrentSendBuffer, SizeOf(TSendBuffer));

      FCurrentSendBuffer := nil;
    end;
  finally
    FSendCache.UnLock;
  end;

  Result := lvRet;
end;

function TIocpContext.CheckPostWSASendCacheBeforeLock: Integer;
var
  lvIOData: POVERLAPPEDEx;
  lvRet: Integer;
  ReadCount: Cardinal;
begin
  lvRet := ERROR_SUCCESS;
  if FIsPostedCloseQuest then 
  begin
    Result := ERROR_SUCCESS;
    Exit;
  end;

  // 检测缓存中是否有需要发送的数据
  if FCurrentSendBuffer = nil then
  begin
    if FSendCache.Count > 0 then
    begin
      FCurrentSendBuffer := FSendCache.Items[0];
    end;
  end;

  // 发送一块内存块
  if (FCurrentSendBuffer <> nil) and (FCurrentSendBuffer.BufSize > 0) and (FCurrentSendBuffer.Position < FCurrentSendBuffer.BufSize)  then
  begin
    ReadCount := FCurrentSendBuffer.BufSize - FCurrentSendBuffer.Position;

    if ReadCount > MAX_OVERLAPPEDEX_BUFFER_SIZE shl 10 then
    begin
      ReadCount := MAX_OVERLAPPEDEX_BUFFER_SIZE shl 10;
    end;

  {$IFDEF SHARE_POOL_MODE}
    lvIOData := TIODataPool.Instance.GetNewIOData(ReadCount);                   // 保证是4的倍数，不要???
  {$ELSE}
    lvIOData := FIocpCore.FIODataPool.GetNewIOData(ReadCount);                  // 保证是4的倍数，不要???
  {$ENDIF}

    lvIOData.IoType := itSend;

    Move(PChar(Cardinal(FCurrentSendBuffer.Buffer) + FCurrentSendBuffer.Position)^, lvIOData.DataBuf.buf^, ReadCount);
    FCurrentSendBuffer.Position := FCurrentSendBuffer.Position + ReadCount;

    lvIOData.DataBuf.len := ReadCount;

    // 发送一个内存块
    lvRet := FIocpCore.PostWSASend(Self, lvIOData);

    if lvRet <> ERROR_SUCCESS then
    begin
      // 发送不成功
    {$IFDEF SHARE_POOL_MODE}
      TIODataPool.Instance.GiveBackIOData(lvIOData);
    {$ELSE}
      FIocpCore.FIODataPool.GiveBackIOData(lvIOData);
    {$ENDIF}
    end;
  end;

  // 如果数据都发送完成从发送缓存中移除
  if (FCurrentSendBuffer <> nil) and ((FCurrentSendBuffer.BufSize = 0) or (FCurrentSendBuffer.BufSize = FCurrentSendBuffer.Position)) then
  begin
    FSendCache.Remove(FCurrentSendBuffer);

    // 释放发送的内存块
    FreeMem(FCurrentSendBuffer.Buffer, FCurrentSendBuffer.BufSize);
    FreeMem(FCurrentSendBuffer, SizeOf(TSendBuffer));

    FCurrentSendBuffer := nil;
  end;

  Result := lvRet;
end;

procedure TIocpContext.ClearSendCache;
var
  I: Integer;
  IsLock: Boolean;
begin
  IsLock := FSendCache.TryLock;
  if IsLock then
  begin
    FCurrentSendBuffer := nil;
    for I := 0 to FSendCache.Count - 1 do
    begin
      FreeMem(PSendBuffer(FSendCache.Items[I]).Buffer, PSendBuffer(FSendCache.Items[I]).BufSize);
      FreeMem(PSendBuffer(FSendCache.Items[I]), SizeOf(TSendBuffer));
    end;
    FSendCache.Clear;
    FSendCache.UnLock;
  end;
end;

procedure TIocpContext.ClearSendCacheUnLock;
var
  I: Integer;
begin
  FCurrentSendBuffer := nil;
  for I := 0 to FSendCache.Count - 1 do
  begin
    FreeMem(PSendBuffer(FSendCache.Items[I]).Buffer, PSendBuffer(FSendCache.Items[I]).BufSize);
    FreeMem(PSendBuffer(FSendCache.Items[I]), SizeOf(TSendBuffer));
  end;
  FSendCache.Clear;
end;

procedure TIocpContext.Close;
begin
  PostWSAClose;
end;

procedure TIocpContext.CloseContextSocket(ErrCode: Integer; CloseFrom: TCloseFrom);
var
  lvSocket: TSocket;
  //LingerOpt: TLinger;
begin
  lvSocket := FSocket;
  if (FSocket <> INVALID_SOCKET) then
  begin
    FSocket := INVALID_SOCKET;

    {
    LingerOpt.l_onoff := 1;
    LingerOpt.l_linger := 0;
    setsockopt(lvSocket, SOL_SOCKET, SO_LINGER, @LingerOpt, sizeof(LingerOpt));
    }

    // 先调用ShutDown，不然光 CloseSocket，过了一段时间还有 GetQueuedCompletionStatus，再回收IOData，导致回心IOData不及时
    // 而白白占用系统资源 chongchong 2016-08-24
    // ShutDown(lvSocket, SD_BOTH);

    CancelIo(lvSocket);         // 这里加了这个 chongchong  2017-09-09
    CloseSocket(lvSocket);
    DoDisconnect(lvSocket);
  end;
  DoReset(True);
end;

procedure TIocpContext.DoConnect;
begin
  FConnectionTick := MyGetTickCount;
  FIocpCore.DoContextConnect(Self);
end;

procedure TIocpContext.DoDisconnect(ASocket: TSocket);
begin
  FIocpCore.DoContextDisconnect(Self, ASocket);
end;

procedure TIocpContext.DoReset(IsClose: Boolean);
begin
  FIsPostedCloseQuest := False;
  FIsWaitingGiveBack := False;

  FSendBlockCount := 0;                
  FSendBytesSize := 0;
  FRecvBlockCount := 0;
  FRecvBytesSize := 0;

  if FRecvBuffersLocker.TryLock then
  begin
    FRecvBuffers := '';
    FRecvBuffersLocker.UnLock;
  end
  else
  begin
    FRecvBuffers := '';
  end;

  F10035ErrorCount := 0;
  
  FLastSendDataTick := MyGetTickCount;
  FLastRecvDataTick := MyGetTickCount;
  FConnectionTick := MyGetTickCount;

  if not IsClose then
    ClearSendCacheUnLock
  else
    ClearSendCache;
end;

function TIocpContext.DoCheckRecvBuffer(var S: string): Boolean;
begin
  Result := False;
end;

procedure TIocpContext.DoRecvBuffer(Buffer: PAnsiChar; Len: Cardinal);
var
  I: Integer;
begin
  // 先不要锁看下数据对不对 chongchong 2016-06-24
  FRecvBuffersLocker.Lock;
  try
    AddToRecvBuffer(Buffer, Len);

    // 当数据来了后，通过DoCheckRecvBuffer来处理接收到的包
    I := 0;
    while (FSocket <> INVALID_SOCKET) do
    begin
      if not DoCheckRecvBuffer(FRecvBuffers) then Break;

      if FIocpCore = nil then Exit;
      if Length(FRecvBuffers) = 0 then Exit;

      Inc(I);
      if I >= 1000 then
        raise Exception.Create('DoCheckRecvBuffer no result False');
    end;
  finally
    FRecvBuffersLocker.UnLock;
  end;
end;

procedure TIocpContext.NotifyStopWork;
begin
  //禁止进出
  Shutdown(FSocket, SD_BOTH);
  //CancelIo(FSocket);
  CloseContextSocket(ERROR_SUCCESS, cfOther);
end;

procedure TIocpContext.PostSendBuffer(const Buffer: PAnsiChar; Len: Cardinal);
var
  lvRet: Integer;
  lvOutBuffer: PSendBuffer;
begin
  if FSocket = INVALID_SOCKET then Exit;
  if FIsPostedCloseQuest then Exit;

  lvRet := ERROR_SUCCESS;

  FSendCache.Lock;
  try
    GetMem(lvOutBuffer, SizeOf(TSendBuffer));
    try
      lvOutBuffer.Position := 0;
      lvOutBuffer.BufSize := Len;

      GetMem(lvOutBuffer.Buffer, Len);
      Move(Buffer^, lvOutBuffer.Buffer^, Len);
    except
      FreeMem(lvOutBuffer, SizeOf(TSendBuffer));
      if FIocpCore <> nil then
        FIocpCore.DoError('TIocpContext.PostSendBuffer GetMem error', Iocpet_Error);
      raise;
    end;

    FSendCache.Add(lvOutBuffer);

    if FCurrentSendBuffer = nil then
    begin
      //准备投递一块数据
      lvRet := CheckPostWSASendCacheBeforeLock;
    end;
  finally
    FSendCache.UnLock;
  end;

  if lvRet <> ERROR_SUCCESS then
  begin
    // 下面这行代码会把 FCurrentSendBuffer 置nil，这里这里是因为 DoReset中中有FSendCacheLocker.Lock;锁2次会死锁
    CloseContextSocket(lvRet, cfPostWSASendCache2);
  end;
end;

procedure TIocpContext.PostSendText(const S: string);
begin
  PostSendBuffer(PChar(S), Length(S));
end;

function TIocpContext.GetUsing;
begin
  Result := (FSocket <> INVALID_SOCKET);
end;

function TIocpContext.PostWSAClose: Boolean;
begin
  Result := False;
  if not IsUsing then Exit;
  if FIocpCore <> nil then
    Result := FIocpCore.PostWSAClose(Self);
end;

//------------------------------------------------------------------------------

function TIocpContext.DoZeroBytesRead(lvIOData: POVERLAPPEDEx): Boolean;
var
  lvError: Integer;
begin
  if Socket = INVALID_SOCKET then
  begin
    Result := False;
    FZeroBytesReadTick := MyGetTickCount;
    Exit;
  end;

  if lvIOData = nil then
  begin
  {$IFDEF SHARE_POOL_MODE}
    lvIOData := TIODataPool.Instance.GetNewIOData(0);
  {$ELSE}
    lvIOData := FIODataPool.GetNewIOData(0);
  {$ENDIF}
  end;

  lvIOData.IoType := itZeroByteRead;

  FZeroBytesReadTick := MyGetTickCount;
  
  //通知工作线程,有新的套接字连接<第三个参数>
  Result := PostQueuedCompletionStatus(
    FIocpCore.FIocpHandle,
    0,
    Cardinal(Self),
    POverlapped(lvIOData));

  if not Result then
  begin
    lvError := GetLastError;
    if (lvError <> WSA_IO_PENDING) then
    begin
      if (lvError <> 10053) and (lvError <> 10054) and (lvError <> 10058) and (lvError <> 10038) then
      begin
        FIocpCore.DoError('TIocpContext.DoZeroBytesRead error', Iocpet_Error, lvError);
      end;
      CloseContextSocket(0, cfOther);
    end;
  end;
end;

{ TIocpCore }

constructor TIocpCore.Create(AOwner: TObject);
var
  I, ThreadCount: Integer;
  SystemInfo: TSystemInfo;
  WorkerThread: TWorkerThread;
begin
  inherited Create;
  FIsActive := False;

  FEventLocker := TIocpCriticalSection.Create({$IFDEF USE_SPINLOCK}'EventLocker'{$ENDIF});
  FDataLogLocker := TIocpCriticalSection.Create({$IFDEF USE_SPINLOCK}'DataLogLocker_2'{$ENDIF});
  FillChar(FCumulativeDataSendRecvLog, SizeOf(FCumulativeDataSendRecvLog), 0);
  FillChar(FRealTimeDataSendRecvLog, SizeOf(FRealTimeDataSendRecvLog), 0);

  FSocket := INVALID_SOCKET;
  FOwner := AOwner;

  FRecvDataDefaultSize := 512;

  FWorkerThreadList := TList.Create;

  GetSystemInfo(SystemInfo);
  ThreadCount := SystemInfo.dwNumberOfProcessors * 2;

  // 创建工作线程 chongchong 2014-05-12
  for I := 0 to ThreadCount - 1 do
  begin
    WorkerThread := TWorkerThread.Create(Self);
    WorkerThread.FIocpHandle := FIocpHandle;
    WorkerThread.FWorkID := FWorkerThreadList.Count;
    FWorkerThreadList.Add(WorkerThread);
  end;

  // 创建一个完成端口（内核对象）
  FIocpHandle := CreateIoCompletionPort(INVALID_HANDLE_VALUE, 0, 0, 0);
  if (FIocpHandle = 0) or (FIocpHandle = INVALID_HANDLE_VALUE) then
  begin
    DoError('TIocpCore.Initialize CreateIoCompletionPort error', Iocpet_Error);
  end
  else
  begin
    for I := 0 to FWorkerThreadList.Count - 1 do
    begin
      TWorkerThread(FWorkerThreadList.Items[I]).Resume;
    end;
  end;

{$IFNDEF SHARE_POOL_MODE}
  FIODataPool := TIODataPool.Create;
{$ENDIF}
end;

destructor TIocpCore.Destroy;
var
  I: Integer;
  WorkerThread: TWorkerThread;
  lvSocket: TSocket;
  IsStopAll: Boolean;
begin
  // 多次投递消息 不然ProcessIOQueued.GetQueuedCompletionStatus会挂起导致WaitFor一直等待 chongchong 2014-05-13
  for I := 0 to FWorkerThreadList.Count * 2 - 1 do
  begin
    PostExitIO;
    Sleep(5);
  end;

  for I := 0 to FWorkerThreadList.Count - 1 do
  begin
    WorkerThread := TWorkerThread(FWorkerThreadList.Items[I]);
    if WorkerThread.FIsRun then
    begin
      WorkerThread.Terminate;
    end;
  end;

{$IF NEED_REGISTER = 0}
  OutputDebugString('等待IOCP工作线程退出');
{$IFEND}

  while True do
  begin
    Sleep(10);
    Application.ProcessMessages;

    IsStopAll := True;
    for I := 0 to FWorkerThreadList.Count - 1 do
    begin
      WorkerThread := TWorkerThread(FWorkerThreadList.Items[I]);
      if WorkerThread.FIsRun then
      begin
        IsStopAll := False;
        Break;
      end;
    end;

    if IsStopAll then Break;
  end;

  for I := 0 to FWorkerThreadList.Count - 1 do
  begin
    WorkerThread := TWorkerThread(FWorkerThreadList.Items[I]);
    WorkerThread.Free;
  end;

{$IF NEED_REGISTER = 0}
  OutputDebugString('IOCP工作线程全释放');
{$IFEND}

  FWorkerThreadList.Free;

{$IFNDEF SHARE_POOL_MODE}
  FIODataPool.Free;
{$ENDIF}

  if FSocket <> INVALID_SOCKET then
  begin
    lvSocket := FSocket;
    FSocket := INVALID_SOCKET;

    CancelIo(lvSocket);    // add chongchong 2017-09-09
    CloseSocket(lvSocket);
  end;

  if FIocpHandle <> 0 then
    CloseHandle(FIocpHandle);

  FEventLocker.Free;
  FDataLogLocker.Free;
end;

function TIocpCore.PostWSARecv(Context: TIocpContext): Boolean;
var
  lvRet: Integer;
  RecvdCount{, Flag}: Cardinal;
  lvIOData: POVERLAPPEDEx;
begin
  Result := False;
  if Context.FSocket = INVALID_SOCKET then Exit;
{$IFDEF SHARE_POOL_MODE}
  lvIOData := TIODataPool.Instance.GetNewIOData(FRecvDataDefaultSize);
{$ELSE}
  lvIOData := FIODataPool.GetNewIOData(FRecvDataDefaultSize);
{$ENDIF}

  lvIOData.IoType := itRecv;
  RecvdCount := 0;

(*
  // 防坑记录：

    http://www.cppblog.com/tx7do/archive/2012/10/15/193298.html
    
    IOCP+UDP的底层出错了,在大数据量的时候经常会报错,并且清一色都是报的:报0xC000000005，读取0x00000010错误.报错之后,整个程序的堆栈就全部破坏掉了
    bool CUDPRecvSendThread::postRecvRequest(CUdpOverLappedRecv* pOverLappedRecv)
    {
      ....................
      int nSenderAddrSize = sizeof (sockaddr_in);
      int rc = 0;
      rc = ::WSARecvFrom(m_ServerSocket, pOverLappedRecv->GetWsaBuffer(), 1, &dwBytesRecv, &dwFlags,
          pOverLappedRecv->GetClientAddr(), &nSenderAddrSize,
          pOverLappedRecv->GetOverlapped(), NULL);
      ....................
    }

    问题就在于WSARecvFrom的7个参数.
    MSDN的描述:
    lpFromlen [in, out]

        A pointer to the size, in bytes, of the "from" buffer required only if lpFrom is specified.

    你会发现,这个参数是一个输入输出值.而WSARecvFrom投递的是一个异步的IOCP请求,故而,出了此方法(CUDPRecvSendThread::postRecvRequest)之后,nSenderAddrSize这个临时变量就会被回收.不出事才怪了.
    要解决这个问题,最好的办法就是把nSenderAddrSize作为CUdpOverLappedRecv的成员变量保存,这样生命周期可以得以保证.
*)

  // 没有这个会有 10045错误
  //Flag := 0;  chongchong 2017-04-30 ----> FWSARecv_dwFlags
  FWSARecv_dwFlags := 0;
  /////异步收取数据
  if (WSARecv(Context.FSocket,
     @lvIOData.DataBuf,
     1,
     RecvdCount,
     FWSARecv_dwFlags,
     @lvIOData.Overlapped, nil) = SOCKET_ERROR) then
  begin
    //重叠IO,出现ERROR_IO_PENDING是正常的，
    //表示数据尚未接收完成，如果有数据接收，GetQueuedCompletionStatus会有返回值

    // 如果 IOData.DataBuf 不是4字节对齐，这里会返回10014错误 chongchong 2014-05-13
    lvRet := WSAGetLastError();
    if (lvRet <> WSA_IO_PENDING) then
    begin
      PostWSAClose(Context);
      if (lvRet <> 10053) and (lvRet <> 10054) and (lvRet <> 10058) and (lvRet <> 10038) then
        DoError('TIocpCore.PostWSARecv WSARecv error', Iocpet_Error, lvRet);
    end;
  end;
end;

function TIocpCore.PostWSASend(Context: TIocpContext; IOData: POVERLAPPEDEx): Integer;
var
  lvErrCode, lvRet, I: Integer;
  SendCount{, Flag}: Cardinal;
begin
  Result := ERROR_SUCCESS;
  //Flag := 0; chongchong 2017-04-30   ----> FWSARecv_dwFlags
  SendCount := 0;

  I := 1;
  while I <= 2 do    //尝试10次,如果还不成功就返回false                                                  2014/6/12
  begin
    //如果立刻发送成功  0也会触发队列
    FWSARecv_dwFlags := 0;
    lvRet := WSASend(Context.FSocket, @IOData.DataBuf, 1, SendCount, FWSARecv_dwFlags, @IOData^, nil);
    if (lvRet = SOCKET_ERROR) then
    begin
      lvErrCode := GetLastError();
      case lvErrCode of
        ERROR_IO_PENDING:
         begin
            // 出现ERROR_IO_PENDING是正常的，表示数据尚未发送完成，
            // 如果数据发送成功，GetQueuedCompletionStatus会有返回值
            Result := ERROR_SUCCESS;
            Break;
         end;

        //首先，Winsock 异常 10035 WSAEWOULDBLOCK (WSAGetLastError) 的意识是 Output Buffer 已经满了，无法再写入数据。
        //确切的说它其实不算是个错误，出现这种异常的绝大部分时候其实都不存在 Output Buffer 已满情况，而是处于一种“忙”的状态，
        //而这种“忙”的状态还很大程度上是由于接收方造成的。
        //意思就是你要发送的对象，对方收的没你发的快或者对方的接受缓冲区已被填满，所以就返回你一个“忙”的标志，而这时你再发多少数据都没任何意义，
        //所以你的系统就抛出个 WSAEWOULDBLOCK 异常通知你，叫你别再瞎忙活了。
        WSAEWOULDBLOCK:
          begin
            // 休息100，等待再次发送
            // TIocpFileLogger.logErrMessage(Format('投递发送数据时发生了错误错误代码:%d', [lvErrCode]));
            Result := WSAEWOULDBLOCK;
            // 缓冲区满5次，这是第6次，不用再弄了(客户端网络太差)
            if Context.F10035ErrorCount >= 5 then Exit;
            Sleep(50);
          end;
        WSAECONNRESET:
          begin
            //An existing connection was forcibly closed by the remote host
            //TIocpFileLogger.logErrMessage(Format('投递发送数据时发生了错误错误代码:%d', [lvErrCode]));
            Result := WSAECONNRESET;
            Break;
          end;
        WSAENETRESET:
          begin
            // Network dropped connection on reset.
            // The connection has been broken due to keep-alive
            // activity detecting a failure while the operation was in progress.
            // TIocpFileLogger.logErrMessage(Format('投递发送数据时发生了错误错误代码:%d', [lvErrCode]));
            Result := WSAENETRESET;
            Break;
          end;
      else
        begin
          //退出循环
          //TIocpFileLogger.logErrMessage(Format('投递发送数据时发生了错误错误代码:%d', [lvErrCode]));
          Result := lvErrCode;
          Break;
        end;
      end;
    end
    else if lvRet = 0 then
    begin
      //没有错误,发送完成
      Result := ERROR_SUCCESS;
      Break;
    end;
    
    Inc(I);
  end;

  if Result = WSAEWOULDBLOCK then
    InterlockedIncrement(Context.F10035ErrorCount)
end;

function TIocpCore.PostWSAClose(Context: TIocpContext): Boolean;
var
  lvIOData: POVERLAPPEDEx;
  lvError: Integer;
begin
  Result := False;
  if Context.FIsPostedCloseQuest then Exit;

{$IFDEF SHARE_POOL_MODE}
  lvIOData := TIODataPool.Instance.GetNewIOData(0);
{$ELSE}
  lvIOData := FIODataPool.GetNewIOData(0);
{$ENDIF}

  lvIOData.IoType := itClose;

  //通知工作线程,有新的套接字连接<第三个参数>
  Result := PostQueuedCompletionStatus(
    FIocpHandle,
    0,
    Cardinal(Context),
    POverlapped(lvIOData));

  if Result then
    Context.FIsPostedCloseQuest := True
  else
  begin
    lvError := GetLastError;
    DoError('TIocpCore.PostWSAClose error', Iocpet_Error, lvError);
  end;
end;

function TIocpCore.PostExitIO: Boolean;
begin
  //通知工作线程,有新的套接字连接<第三个参数>
  Result := PostQueuedCompletionStatus(
    FIocpHandle,
    0,   
    0,
    POverlapped(IOCP_QUEUED_SHUTDOWN)
  );
end;

function TIocpCore.ProcessIOQueued(const WorkerID: Cardinal): Boolean;
var
  lvBytesTransferred: Cardinal;
  lvResultStatus: BOOL;
  lvContext: TIocpContext;

  lvIOData: POVERLAPPEDEx;
  ErrNum: Integer;
  lvRet: Integer;

  RecvdCount{, Flag}: Cardinal;
begin
  Result := True;

  //工作者线程会停止到GetQueuedCompletionStatus函数处，直到接受到数据为止

  /// //#include <boost/asio/detail/win_Iocp_io_service.hpp> 有一段这样的说明
  ///   意思是应该把INFINITE用其他代替(500)
  // Timeout to use with GetQueuedCompletionStatus. Some versions of windows
  // have a "bug" where a call to GetQueuedCompletionStatus can appear stuck
  // even though there are events waiting on the queue. Using a timeout helps
  // to work around the issue.
  lvResultStatus := GetQueuedCompletionStatus(FIocpHandle,
    lvBytesTransferred,
    Cardinal(lvContext),
    POverlapped(lvIOData),
    INFINITE);

  if DWORD(lvIOData) = IOCP_QUEUED_SHUTDOWN then
  begin
    Result := False;                                                                                //通知工作现在退出
  end
  else if (lvResultStatus = False) then
  begin
    ErrNum := GetLastError;

    if ErrNum = ERROR_NETNAME_DELETED then
    begin
      if lvContext <> nil then
        lvContext.CloseContextSocket(0, cfProcessIOQueued);
    end

    // IocpTcpClient连接服务器失败时，到这里来处理
    else if (lvIOData <> nil) and (lvIOData.IoType = itConnect) then
    begin
      if lvContext <> nil then
        lvContext.CloseContextSocket(ErrNum, cfProcessIOQueued);
    end
    else
    begin
      {
      if lvIOData = nil then
        DoError('[' + IntToStr(ErrNum) + ']TIocpCore.ProcessIOQueued GetQueuedCompletionStatus result false 1.', Iocpet_Info, ErrNum)
      else
        DoError('[' + IntToStr(ErrNum) + ']TIocpCore.ProcessIOQueued GetQueuedCompletionStatus result false 2.', Iocpet_Info, ErrNum);
      }
    end;

  {$IFDEF SHARE_POOL_MODE}
    if lvIOData <> nil then
      TIODataPool.Instance.GiveBackIOData(lvIOData);
  {$ELSE}
    if lvIOData <> nil then
      FIODataPool.GiveBackIOData(lvIOData);
  {$ENDIF}
  end

  // 关闭请求
  else if (lvIOData <> nil) and (lvIOData.IoType = itClose) then
  begin
    try
      lvContext.CloseContextSocket(ERROR_SUCCESS, cfOther);
    finally
    {$IFDEF SHARE_POOL_MODE}
      TIODataPool.Instance.GiveBackIOData(lvIOData);
    {$ELSE}
      FIODataPool.GiveBackIOData(lvIOData);
    {$ENDIF}
    end;
  end

  // IocpTcpClient与服务器连接上了 >>>>>>>>>>>>>>>>>>>>>>>>>>
  else if (lvIOData <> nil) and (lvIOData.IoType = itConnect) then
  begin
    if lvContext <> nil then
    begin
      lvContext.DoConnect;

      // 投递0字节解锁缓冲区，lvIOData还要用。不要回收 chongchong 2016-08-17
      lvContext.DoZeroBytesRead(lvIOData);

      PostWSARecv(lvContext);
    end
    else
    begin
    {$IFDEF SHARE_POOL_MODE}
      TIODataPool.Instance.GiveBackIOData(lvIOData);
    {$ELSE}
      FIODataPool.GiveBackIOData(lvIOData);
    {$ENDIF}
    end;
  end

  // ★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
  // 加 DoZeroBytesRead 用来处理WSAENOBUFS处理，当出更这个错误后，很难发现。
  // 目前在我们的系统表现为2个网关应用程序，均是多线程，运行一段时间后，公网掉线， 关掉一个就可以了
  // 据说 ZeroByteRead 是用来解锁缓存区，还待验证

  {
    WSAENOBUFS: 这个问题通常很难靠直觉发现，因为当你第一次看见的时候你或许认为是一个内存泄露错误。
    假定已经开发完成了你的完成端口服务器并且运行的一切良好，但是当你对其进行压力测试的时候突然发
    现服务器被中止而不处理任何请求了，如果你运气好的话你会很快发现是因为WSAENOBUFS 错误而影响了这一切。
  }

  // http://www.codeproject.com/KB/IP/iocp_server_client.aspx?fid=179244&df=90&mpp=25&noise=3&sort=Position&view=Quick&fr=26
  // A simple IOCP Server/Client Class
  else if (lvIOData <> nil) and (lvIOData.IoType = itZeroByteRead) then
  begin
    if (lvContext <> nil) and (lvContext.FSocket <> INVALID_SOCKET) then
    begin
      // lvIOData不回收，循环利用
      lvIOData.IoType := itZeroReadCompleted;

      RecvdCount := 0;

      // 不加这一行会出10045错误
      //Flag := 0; chongchong 2017-04-30  ----> FWSARecv_dwFlags
      FWSARecv_dwFlags := 0;

      if (WSARecv(lvContext.FSocket,
         @lvIOData.DataBuf,
         1,
         RecvdCount,

         //lpFlags [in, out] A pointer to flags used to modify the behavior of the WSARecv function call. For more information, see the Remarks section.
         // The lpFlags parameter is used both on input and returned on output, allowing applications to sense the output state of the MSG_PARTIAL flag bit. However, the MSG_PARTIAL flag bit is not supported by all protocols.
         FWSARecv_dwFlags,

         @lvIOData.Overlapped, nil) = SOCKET_ERROR) then
      begin
        lvRet := WSAGetLastError();
        if (lvRet <> WSA_IO_PENDING) then
        begin
          PostWSAClose(lvContext);
          if (lvRet <> 10053) and (lvRet <> 10054) and (lvRet <> 10058) and (lvRet <> 10038) then
            DoError('TIocpCore.WSARecv ZeroByteRead error.', Iocpet_Error, lvRet);
        end;
      end;
    end
    else
    begin
    {$IFDEF SHARE_POOL_MODE}
      if lvIOData <> nil then
        TIODataPool.Instance.GiveBackIOData(lvIOData);
    {$ELSE}
      if lvIOData <> nil then
        FIODataPool.GiveBackIOData(lvIOData);
    {$ENDIF}
    end;
  end

  else if (lvIOData <> nil) and (lvIOData.IoType = itZeroReadCompleted) then
  begin
    if (lvContext <> nil) and (lvContext.FSocket <> INVALID_SOCKET) then
    begin
      // lvIOData不回收，循环利用
      lvContext.DoZeroBytesRead(lvIOData);
    end
    else
    begin
    {$IFDEF SHARE_POOL_MODE}
      if lvIOData <> nil then
        TIODataPool.Instance.GiveBackIOData(lvIOData);
    {$ELSE}
      if lvIOData <> nil then
        FIODataPool.GiveBackIOData(lvIOData);
    {$ENDIF}
    end;
  end

  else if lvBytesTransferred = 0 then
  begin
    if lvContext <> nil then
    begin
      lvContext.CloseContextSocket(ERROR_SUCCESS, cfOther);
    end;

  {$IFDEF SHARE_POOL_MODE}
    if lvIOData <> nil then
      TIODataPool.Instance.GiveBackIOData(lvIOData);
  {$ELSE}
    if lvIOData <> nil then
      FIODataPool.GiveBackIOData(lvIOData);
  {$ENDIF}
  end
  else if (lvIOData <> nil) then
  begin
    ErrNum := 0;
    if lvIOData.IoType = itRecv then
    begin
      try
        try
          // 数据接收事件 chongchong 2014-06-13
          ErrNum := 1;
          DoRecvDataBuffer(lvContext, lvIOData.DataBuf.Buf, lvIOData.Overlapped.InternalHigh, WorkerID);

          ErrNum := 2;

          if FCumulativeDataSendRecvLog.RecvBlockCount >= High(FCumulativeDataSendRecvLog.RecvBlockCount) then
            InterlockedExchange(FCumulativeDataSendRecvLog.RecvBlockCount, 0);
          InterlockedIncrement(FCumulativeDataSendRecvLog.RecvBlockCount);

          if FRealTimeDataSendRecvLog.RecvBlockCount >= High(FRealTimeDataSendRecvLog.RecvBlockCount) then
            InterlockedExchange(FRealTimeDataSendRecvLog.RecvBlockCount, 0);
          InterlockedIncrement(FRealTimeDataSendRecvLog.RecvBlockCount);

          ErrNum := 3;

          FDataLogLocker.Lock;
          if FCumulativeDataSendRecvLog.RecvBytesSize >= MaxInt64 then
            FCumulativeDataSendRecvLog.RecvBytesSize := 0;
          FCumulativeDataSendRecvLog.RecvBytesSize := FCumulativeDataSendRecvLog.RecvBytesSize + lvBytesTransferred;

          if FRealTimeDataSendRecvLog.RecvBytesSize >= MaxInt64 then
            FRealTimeDataSendRecvLog.RecvBytesSize := 0;
          FRealTimeDataSendRecvLog.RecvBytesSize := FRealTimeDataSendRecvLog.RecvBytesSize + lvBytesTransferred;
          FDataLogLocker.UnLock;

          ErrNum := 4;
          if lvContext <> nil then
          begin
            ErrNum := 5;
            lvContext.FDataLogLocker.Lock;
            try
              if lvContext.FRecvBlockCount >= High(lvContext.FRecvBlockCount) then
                lvContext.FRecvBlockCount := 0;
              Inc(lvContext.FRecvBlockCount);

              if lvContext.FRecvBytesSize >= MaxInt64 then
                lvContext.FRecvBytesSize := 0;
              lvContext.FRecvBytesSize := lvContext.FRecvBytesSize + lvBytesTransferred;
            finally
              lvContext.FDataLogLocker.UnLock;
            end;

            lvContext.FLastRecvDataTick := MyGetTickCount;

            ErrNum := 6;
            lvContext.FIsBusying := True;
            try
              try
                lvContext.DoRecvBuffer(lvIOData.DataBuf.buf, lvIOData.Overlapped.InternalHigh);
              except
                DoError('TIocpCore.ProcessIOQueued.itRecv DoRecvBuffer error', Iocpet_Error, ErrNum);
              end;
            finally
              lvContext.FIsBusying := False;
            end;

            ErrNum := 7;
            if lvContext.FIsWaitingGiveBack then
            begin
              ErrNum := 8;
              if lvContext.IsBusying then
                lvContext.IsWaitingGiveBack := True
              else
                lvContext.CloseContextSocket(ERROR_SUCCESS, cfOther);
            end
            else
            begin
              ErrNum := 9;
              PostWSARecv(lvContext);
            end;
          end;
        except
          on E: Exception do
          begin
            DoError('TIocpCore.ProcessIOQueued.itRecv error', Iocpet_Error, ErrNum);
          end;
        end;
      finally
      {$IFDEF SHARE_POOL_MODE}
        TIODataPool.Instance.GiveBackIOData(lvIOData);
      {$ELSE}
        FIODataPool.GiveBackIOData(lvIOData);
      {$ENDIF}
      end;
    end
    else if lvIOData.IoType = itSend then
    begin
      try
        // 发送字节不一致
        if lvIOData.DataBuf.len <> lvBytesTransferred then
        begin

        end;

        DoSendDataBuffer(lvContext, lvIOData.DataBuf.Buf, lvIOData.Overlapped.InternalHigh, WorkerID);

        if FRealTimeDataSendRecvLog.SendBlockCount >= High(FRealTimeDataSendRecvLog.SendBlockCount) then
          InterlockedExchange(FRealTimeDataSendRecvLog.SendBlockCount, 0);
        InterlockedIncrement(FRealTimeDataSendRecvLog.SendBlockCount);

        if FCumulativeDataSendRecvLog.SendBlockCount >= High(FCumulativeDataSendRecvLog.SendBlockCount) then
          InterlockedExchange(FCumulativeDataSendRecvLog.SendBlockCount, 0);
        InterlockedIncrement(FCumulativeDataSendRecvLog.SendBlockCount);

        FDataLogLocker.Lock;

        if FCumulativeDataSendRecvLog.SendBytesSize >= MaxInt64 then
          FCumulativeDataSendRecvLog.SendBytesSize := MaxInt64;
        FCumulativeDataSendRecvLog.SendBytesSize := FCumulativeDataSendRecvLog.SendBytesSize + lvBytesTransferred;

        if FRealTimeDataSendRecvLog.SendBytesSize >= MaxInt64 then
          FRealTimeDataSendRecvLog.SendBytesSize := MaxInt64;
        FRealTimeDataSendRecvLog.SendBytesSize := FRealTimeDataSendRecvLog.SendBytesSize + lvBytesTransferred;

        FDataLogLocker.UnLock;

        // 包可能没有投递完，继续投递
        if lvContext <> nil then
        begin
          lvContext.FDataLogLocker.Lock;
          try
            if lvContext.FSendBlockCount >= High(lvContext.FSendBlockCount) then
              lvContext.FSendBlockCount := 0;
            Inc(lvContext.FSendBlockCount);

            if lvContext.FRecvBytesSize >= MaxInt64 then
              lvContext.FRecvBytesSize := 0;
            lvContext.FSendBytesSize := lvContext.FSendBytesSize + lvBytesTransferred;
          finally
            lvContext.FDataLogLocker.UnLock;
          end;

          lvContext.FLastSendDataTick := MyGetTickCount;

          if lvContext.FIsWaitingGiveBack then
            lvContext.CloseContextSocket(ERROR_SUCCESS, cfOther)
          else
          begin
            lvRet := lvContext.CheckPostWSASendCache;
            if lvRet <> ERROR_SUCCESS then
            begin
              // 下面这行代码会把 FCurrentSendBuffer 置nil
              lvContext.CloseContextSocket(lvRet, cfPostWSASendCache1);
            end;
          end;
        end;
      finally
      {$IFDEF SHARE_POOL_MODE}
        TIODataPool.Instance.GiveBackIOData(lvIOData);
      {$ELSE}
        FIODataPool.GiveBackIOData(lvIOData);
      {$ENDIF}
      end;
    end;
  end;
end;

procedure TIocpCore.DoContextConnect(Context: TIocpContext);
begin
  if Assigned(FOnContextConnect) then
  begin
    if FEventLocker.TryLock then
    begin
      FOnContextConnect(Self, Context);
      FEventLocker.UnLock;
    end
    else
    begin
      FOnContextConnect(Self, Context);
    end;
  end;
end;

procedure TIocpCore.DoContextDisconnect(Context: TIocpContext; ASocket: TSocket);
begin
  if Assigned(FOnContextDisconnect) then
  begin
    if FEventLocker.TryLock then
    begin
      FOnContextDisconnect(Self, Context, ASocket);
      FEventLocker.UnLock;
    end
    else
    begin
      FOnContextDisconnect(Self, Context, ASocket);
    end;
  end;
end;

procedure TIocpCore.DoError(ErrorStr: string; ErrorType: TIocpErrorType; ErrorCode: Integer);
begin
  if Assigned(FOnError) then
  begin
    if FEventLocker.TryLock then
    begin
      FOnError(Self, ErrorStr, ErrorType, ErrorCode);
      FEventLocker.UnLock;
    end
    else
    begin
      FOnError(Self, ErrorStr, ErrorType, ErrorCode);
    end;
  end;
end;

procedure TIocpCore.DoRecvDataBuffer(Context: TIocpContext; const Buffer: PChar; const BufferLen: Cardinal; const WorkerID: Cardinal);
begin
  if Assigned(FOnRecvDataBuffer) then
  begin
    if FEventLocker.TryLock then
    begin
      FOnRecvDataBuffer(Self, Context, Buffer, BufferLen, WorkerID);
      FEventLocker.UnLock;
    end
    else
    begin
      FOnRecvDataBuffer(Self, Context, Buffer, BufferLen, WorkerID);
    end;
  end;
end;

procedure TIocpCore.DoSendDataBuffer(Context: TIocpContext; const Buffer: PChar; const BufferLen: Cardinal; const WorkerID: Cardinal);
begin
  if Assigned(FOnSendDataBuffer) then
  begin
    if FEventLocker.TryLock then
    begin
      FOnSendDataBuffer(Self, Context, Buffer, BufferLen, WorkerID);
      FEventLocker.UnLock;
    end
    else
    begin
      FOnSendDataBuffer(Self, Context, Buffer, BufferLen, WorkerID);
    end;
  end;
end;

function TIocpCore.GetIsFreeIOData(IoDataAllocSize: Cardinal): Boolean;
begin
  Result := (IoDataAllocSize <> MAX_OVERLAPPEDEX_BUFFER_SIZE shl 10) and
    (IoDataAllocSize <> FRecvDataDefaultSize);
end;

function TIocpCore.GetWorkerThreadCount: Integer;
begin
  Result := FWorkerThreadList.Count;
end;

procedure TIocpCore.SetWorkerThreadCount(const Value: Integer);
var
  SystemInfo: TSystemInfo;
  I, Count: Integer;
  WorkerThread: TWorkerThread;
  IsStopAll: Boolean;
begin
  Count := Value;

  if Count <= 0 then
  begin
    GetSystemInfo(SystemInfo);
    Count := SystemInfo.dwNumberOfProcessors * 2;
  end;

  if Count > FWorkerThreadList.Count then
  begin
    Count := Count - FWorkerThreadList.Count;

    // 创建工作线程 chongchong 2014-05-12
    for I := 0 to Count - 1 do
    begin
      WorkerThread := TWorkerThread.Create(Self);
      WorkerThread.FIocpHandle := FIocpHandle;
      WorkerThread.FWorkID := FWorkerThreadList.Count;
      FWorkerThreadList.Add(WorkerThread);

      WorkerThread.Resume;
    end;
  end
  else if Count < FWorkerThreadList.Count then
  begin
    for I := 0 to FWorkerThreadList.Count - 1 do
    begin
      PostExitIO;
      Sleep(5);
    end;

    for I := 0 to FWorkerThreadList.Count - 1 do
    begin
      WorkerThread := TWorkerThread(FWorkerThreadList.Items[I]);
      if WorkerThread.FIsRun then
      begin
        WorkerThread.Terminate;
      end;
    end;

{$IF NEED_REGISTER = 0}
    OutputDebugString('等待IOCP工作线程退出');
{$IFEND}

    while True do
    begin
      Sleep(10);
      Application.ProcessMessages;

      IsStopAll := True;
      for I := 0 to FWorkerThreadList.Count - 1 do
      begin
        WorkerThread := TWorkerThread(FWorkerThreadList.Items[I]);
        if WorkerThread.FIsRun then
        begin
          IsStopAll := False;
          Break;
        end;
      end;

      if IsStopAll then Break;
    end;

    for I := 0 to FWorkerThreadList.Count - 1 do
    begin
      WorkerThread := TWorkerThread(FWorkerThreadList.Items[I]);
      WorkerThread.Free;
    end;

{$IF NEED_REGISTER = 0}
    OutputDebugString('IOCP工作线程全释放');
{$IFEND}

    FWorkerThreadList.Clear;

    for I := 0 to Count - 1 do
    begin
      WorkerThread := TWorkerThread.Create(Self);
      WorkerThread.FIocpHandle := FIocpHandle;
      WorkerThread.FWorkID := FWorkerThreadList.Count;
      FWorkerThreadList.Add(WorkerThread);

      WorkerThread.Resume;
    end;
  end;
end;

procedure TIocpCore.ClearRealTimeDataSendRecvLog;
begin
  FDataLogLocker.Lock;
  FillChar(FRealTimeDataSendRecvLog, SizeOf(FRealTimeDataSendRecvLog), 0);
  FDataLogLocker.UnLock;
end;

//------------------------------------------------------------------------------

procedure DoSocketStartup;
var
  WSData: TWSAData;
begin
  _SocketStartup := WSAStartup($0202, WSData) <> 0;
end;

procedure DoSocketCleanup;
begin
  if _SocketStartup then WSACleanup();
end;

initialization
begin
  DoSocketStartup;
end;

finalization
begin
  DoSocketCleanup;
end;

end.

