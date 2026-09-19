unit IocpTcpServer;

interface

{$I iocp.inc}

uses
  Classes, SysUtils, Windows, SyncObjs, IocpWinsock2, IocpCommon,
  IODataPool, IocpUtils,
  Forms;

const
  MAX_CONTEXT_COUNT = 5000;
  
type
  TIocpTcpServer = class;

  TIocpAcceptConnectEvent = procedure(Sender: TIocpTcpServer; RemoteAddr: string; RemotePort: Word; var IsAccept: Boolean) of object;

  // Iocp客户端连接处理线程
  TAcceptListenerThread = class(TIocpThread)
  private
    FIsRun: Boolean;
    FTcpServer: TIocpTcpServer;
  protected
    procedure Execute; override;
  end;

{$IFNDEF SHARE_POOL_MODE}
  TIocpClientContextPool = class;
{$ENDIF}

  TIocpClientContext = class(TIocpContext)
  private
    FContextID: Integer;                      // ID

    FRemoteAddrValue: Cardinal;
    FRemoteAddr: string;
    FRemotePort: Word;

  {$IFNDEF SHARE_POOL_MODE}
    FContextPool: TIocpClientContextPool;
  {$ENDIF}
    procedure InvokeConnect;
  protected
    // 关闭客户端连接
    procedure CloseContextSocket(ErrCode: Integer; CloseFrom: TCloseFrom); override;
  public
    constructor Create(AIocpCore: TIocpCore; ASocket: TSocket = 0); override;
    destructor Destroy; override;

    property RemoteAddrValue: Cardinal read FRemoteAddrValue;
    property RemoteAddr: string read FRemoteAddr;
    property RemotePort: Word read FRemotePort;
    property ContextID: Integer read FContextID;

  {$IFNDEF SHARE_POOL_MODE}
    property ContextPool: TIocpClientContextPool read FContextPool write FContextPool;
  {$ENDIF}
  end;

  TIocpClientContextClass = class of TIocpClientContext;

  TIocpClientContextPool = class(TObject)
  private
  {$IFNDEF SHARE_POOL_MODE}
    FIocpCore: TIocpCore;
  {$ENDIF}

    FContextClass: TIocpClientContextClass;
    FContextList: TList;
    FOnlineContextList: TList;
    FNoUseContextList: TList;

    FLocker: TIocpCriticalSection;

    // 最大在线客户端数量 chongchong 2014-06-14
    FMaxOnlineContextCount: Integer;

    function GetOnlineContextCount: Integer;
    function GetNoUseContextCount: Integer;
    function GetContextCount: Integer;
    function GetContexts(Index: Integer): TIocpClientContext;
  public
  {$IFDEF SHARE_POOL_MODE}
    class function Instance: TIocpClientContextPool;
    constructor Create;
  {$ELSE}
    constructor Create(AIocpCore: TIocpCore);
  {$ENDIF}

    destructor Destroy; override;
    procedure Clear;

    procedure RegisterClientContextClass(AContextClass: TIocpClientContextClass);

    function GetNewContext(AIocpCore: TIocpCore; ASocket: TSocket): TIocpClientContext;

    procedure TryCloseContext(AContext: TIocpClientContext);
    procedure GetOnlineContextList(List: TList);

    // 清除所有在线用户，不断线而已；这个函数仅仅是用来恶心破解的人
    procedure ClearOnlineContexts;

    //function TryLock: Boolean;
    procedure Lock;
    procedure UnLock;

  {$IFNDEF SHARE_POOL_MODE}
    property IocpCore: TIocpCore read FIocpCore;
  {$ENDIF}
    property MaxOnlineContextCount: Integer read FMaxOnlineContextCount;
    property OnlineContextCount: Integer read GetOnlineContextCount;
    property NoUseContextCount: Integer read GetNoUseContextCount;
    property ContextCount: Integer read GetContextCount;
    property Contexts[Index: Integer]: TIocpClientContext read GetContexts; default;
  end;

  TIocpTcpServer = class(TComponent)
  private
    FAddress: string;
    FPort: Word;
    FActive: Boolean;

    FSystemSocketHeartState: Boolean;

    FIocpCore: TIocpCore;
    FAccetpThread: TAcceptListenerThread;

    FBindObject: TObject;

    FOnContextConnect: TIocpContextConnectEvent;
    FOnContextDisconnect: TIocpContextDisonnectEvent;
    FOnRecvDataBuffer: TIocpDataBufferEvent;
    FOnSendDataBuffer: TIocpDataBufferEvent;
    FOnError: TIocpErrorEvent;

    FOnAcceptConnect: TIocpAcceptConnectEvent;

  {$IFNDEF SHARE_POOL_MODE}
    FClientContextPool: TIocpClientContextPool;
    FIODataPool: TIODataPool;
  {$ENDIF}

    procedure SetActive(const Value: Boolean);
    procedure SetAddress(const Value: string);
    procedure SetPort(const Value: Word);

    procedure SetOnContextConnect(const Value: TIocpContextConnectEvent);
    procedure SetOnContextDisconnect(const Value: TIocpContextDisonnectEvent);
    procedure SetOnError(const Value: TIocpErrorEvent);
    procedure SetOnRecvDataBuffer(const Value: TIocpDataBufferEvent);
    procedure SetOnSendDataBuffer(const Value: TIocpDataBufferEvent);

    //ocedure OnAccetpThreadTerminate(Sender: TObject);
  protected
    procedure DoError(ErrorStr: string; ErrorType: TIocpErrorType; ErrorCode: Integer = 0); virtual;
    procedure DoAcceptConnect(RemoteAddr: string; RemotePort: Word; var IsAccept: Boolean); virtual;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;

    property IocpCore: TIocpCore read FIocpCore;

    property Address: string read FAddress write SetAddress;
    property Port: Word read FPort write SetPort;
    property Active: Boolean read FActive write SetActive;

    // 是否处理默认的socket心跳
    property SystemSocketHeartState: Boolean read FSystemSocketHeartState write
        FSystemSocketHeartState default False;

    property OnContextConnect: TIocpContextConnectEvent read FOnContextConnect write SetOnContextConnect;
    property OnContextDisconnect: TIocpContextDisonnectEvent read FOnContextDisconnect write SetOnContextDisconnect;
    property OnRecvDataBuffer: TIocpDataBufferEvent read FOnRecvDataBuffer write SetOnRecvDataBuffer;
    property OnSendDataBuffer: TIocpDataBufferEvent read FOnSendDataBuffer write SetOnSendDataBuffer;
    property OnError: TIocpErrorEvent read FOnError write SetOnError;
    property OnAcceptConnect: TIocpAcceptConnectEvent read FOnAcceptConnect write FOnAcceptConnect;

    property BindObject: TObject read FBindObject write FBindObject;
  end;

implementation

uses
  Grobal2_Ex;

{$IFDEF SHARE_POOL_MODE}
var
  _ClientContextPoolInstance: TIocpClientContextPool;
{$ENDIF}

{ TAcceptListenerThread }

procedure TAcceptListenerThread.Execute;
var
  lvSocket: TSocket;
  lvClientContext: TIocpClientContext;
  lvIOPort: THandle;

  SockAddrIn: TSockAddrIn;
  nSize: Integer;
  RemoteAddr: string;
  RemotePort: Word;
  IsAccept: Boolean;
begin
  inherited;
  FIsRun := True;
  try
    while not Terminated do
    begin
      try
        lvSocket := WSAAccept(FIocpCore.Socket, nil, nil, nil, 0);

        if (lvSocket = INVALID_SOCKET) then
          FTcpServer.DoError('TAcceptListenerThread WSAAccept error', Iocpet_Info)
        else
        begin
          // 连接数已达到最大数量，不再接受连接
          if (TIocpClientContextPool.Instance.ContextCount >= MAX_CONTEXT_COUNT) and
            (TIocpClientContextPool.Instance.NoUseContextCount = 0) then
          begin
            FTcpServer.DoError('Maximum number of concurrent connections', Iocpet_Info, -999);
            Continue;
          end;

          // --------------------------------------  新的链接来了

          nSize := SizeOf(SockAddrIn);
          GetPeerName(lvSocket, TSockAddr(SockAddrIn), nSize);
          RemoteAddr := inet_ntoa(SockAddrIn.sin_addr);
          RemotePort := ntohs(SockAddrIn.sin_port);

          IsAccept := True;
          FTcpServer.DoAcceptConnect(RemoteAddr, RemotePort, IsAccept);
          if not IsAccept then
          begin
            CloseSocket(lvSocket);
          end
          else
          begin
            // 加入心跳
            if FTcpServer.SystemSocketHeartState then
            begin
              InitializeSocketHeart(lvSocket);
            end;

          {$IFDEF SHARE_POOL_MODE}
            lvClientContext := TIocpClientContextPool.Instance.GetNewContext(FIocpCore, lvSocket);

            lvIOPort := CreateIoCompletionPort(lvSocket, FIocpCore.IocpHandle, Cardinal(lvClientContext), 0);
            if (lvIOPort = 0) then
            begin
              TIocpClientContextPool.Instance.Lock;
              try
                lvClientContext.Socket := INVALID_SOCKET;
                TIocpClientContextPool.Instance.FNoUseContextList.Add(lvClientContext);
              finally
                TIocpClientContextPool.Instance.UnLock;
              end;
              Exit;
            end;
          {$ELSE}
            lvClientContext := FClientContextPool.GetNewContext(FIocpCore, lvSocket);

            lvIOPort := CreateIoCompletionPort(lvSocket, FIocpCore.IocpHandle, Cardinal(lvClientContext), 0);
            if (lvIOPort = 0) then
            begin
              FClientContextPool.Lock;
              try
                lvClientContext.Socket := INVALID_SOCKET;
                FClientContextPool.FNoUseContextList.Add(lvClientContext);
              finally
                FClientContextPool.UnLock;
              end;
              Exit;
            end;
          {$ENDIF}

            if lvIOPort <> 0 then
            begin
              lvClientContext.InvokeConnect;

              // 投递0字节解锁缓冲区 chongchong 2016-08-17
              lvClientContext.DoZeroBytesRead(nil);

              // 接收数据
              FIocpCore.PostWSARecv(lvClientContext);
            end;
          end;
        end;
      except
      end;
    end;
  finally
    FIsRun := False;
  end;
end;


//------------------------------------------------------------------------------

{ TIocpClientContext }


constructor TIocpClientContext.Create(AIocpCore: TIocpCore; ASocket: TSocket);
begin
  inherited Create(AIocpCore, ASocket);
  FContextID := -1;
{$IFNDEF SHARE_POOL_MODE}
  FContextPool := nil;
{$ENDIF}
end;

destructor TIocpClientContext.Destroy;
begin
  FContextID := -1;
{$IFNDEF SHARE_POOL_MODE}
  FContextPool := nil;
{$ENDIF}
  inherited;
end;

procedure TIocpClientContext.CloseContextSocket(ErrCode: Integer; CloseFrom: TCloseFrom);
var
  ASocket: TSocket;
begin
  ASocket := Socket;

  inherited CloseContextSocket(ErrCode, CloseFrom);

{$IFDEF SHARE_POOL_MODE}
  if (ASocket <> INVALID_SOCKET) and (ASocket <> 0) then
  begin
    TIocpClientContextPool.Instance.Lock;
    try
      TIocpClientContextPool.Instance.FOnlineContextList.Remove(Self);
      TIocpClientContextPool.Instance.FNoUseContextList.Add(Self);
    finally
      TIocpClientContextPool.Instance.UnLock;
    end;
  end;
{$ELSE}
  if (ASocket <> INVALID_SOCKET) and (ASocket <> 0) then
  begin
    FContextPool.Lock
    try
      FContextPool.FOnlineContextList.Remove(Self);
      FContextPool.FNoUseContextList.Add(Self);
    finally
      FContextPool.UnLock;
    end;
  end;
{$ENDIF}
end;

procedure TIocpClientContext.InvokeConnect;
var
  SockAddrIn: TSockAddrIn;
  nSize: Integer;
  OnlineCount: Integer;
begin
  DoReset(False);

  nSize := SizeOf(SockAddrIn);
  GetPeerName(Socket, TSockAddr(SockAddrIn), nSize);
  FRemoteAddrValue := SockAddrIn.sin_addr.S_addr;
  FRemoteAddr := inet_ntoa(SockAddrIn.sin_addr);
  FRemotePort := ntohs(SockAddrIn.sin_port);

{$IFDEF SHARE_POOL_MODE}
  TIocpClientContextPool.Instance.Lock;
  try
    TIocpClientContextPool.Instance.FOnlineContextList.Add(Self);
    OnlineCount := TIocpClientContextPool.Instance.FOnlineContextList.Count;
    if OnlineCount > TIocpClientContextPool.Instance.FMaxOnlineContextCount then
      TIocpClientContextPool.Instance.FMaxOnlineContextCount := OnlineCount;
  finally
    TIocpClientContextPool.Instance.UnLock;
  end;
{$ELSE}
  FContextPool.Lock;
  try
    FContextPool.FOnlineContextList.Add(Self);
    OnlineCount := FContextPool.FOnlineContextList.Count;
    if OnlineCount > FContextPool.FMaxOnlineContextCount then
      FContextPool.FMaxOnlineContextCount := OnlineCount;
  finally
    FContextPool.UnLock;
  end;
{$ENDIF}

  DoConnect;
end;

//------------------------------------------------------------------------------

{ TIocpClientContextPool }

{$IFDEF SHARE_POOL_MODE}
class function TIocpClientContextPool.Instance: TIocpClientContextPool;
begin
  Result := _ClientContextPoolInstance;
end;
{$ENDIF}

{$IFDEF SHARE_POOL_MODE}
constructor TIocpClientContextPool.Create;
{$ELSE}
constructor TIocpClientContextPool.Create(AIocpCore: TIocpCore);
{$ENDIF}
begin
  FLocker := TIocpCriticalSection.Create({$IFDEF USE_SPINLOCK}'ContextPoolLocker'{$ENDIF});

{$IFNDEF SHARE_POOL_MODE}
  FIocpCore := AIocpCore;
{$ENDIF}

  FContextClass := nil;
  FContextList := TList.Create;
  FOnlineContextList := TList.Create;
  FNoUseContextList := TList.Create;

  FMaxOnlineContextCount := 0;
end;

destructor TIocpClientContextPool.Destroy;
begin
  Clear;
  FContextList.Free;
  FOnlineContextList.Free;
  FNoUseContextList.Free;

  FLocker.Free;
  inherited;
end;


function TIocpClientContextPool.GetNewContext(AIocpCore: TIocpCore; ASocket: TSocket): TIocpClientContext;
begin
  Result := nil;
  Lock;
  try
    if (FNoUseContextList.Count > 0) then
    begin
      Result := FNoUseContextList.Items[0];
      FNoUseContextList.Delete(0);
      Result.IocpCore := AIocpCore;
      Result.Socket := ASocket;
    end;
  finally
    UnLock;
  end;

  if Result = nil then
  begin
    if FContextClass = nil then
      raise Exception.Create('No register FContextClass')
    else
    begin
      Result := FContextClass.Create(AIocpCore, ASocket);

    {$IFNDEF SHARE_POOL_MODE}
      Result.ContextPool := Self;
    {$ENDIF}

      Lock;
      try
        Result.FContextID := FContextList.Count;
        FContextList.Add(Result);
      finally
        UnLock;
      end;
    end;
  end;
end;

{
function TIocpClientContextPool.TryLock: Boolean;
begin
  Result := FLocker.TryLock;
end;
}

procedure TIocpClientContextPool.Lock;
begin
  FLocker.Lock;
end;

procedure TIocpClientContextPool.UnLock;
begin
  FLocker.UnLock;
end;

procedure TIocpClientContextPool.RegisterClientContextClass(
  AContextClass: TIocpClientContextClass);
begin
  FContextClass := AContextClass;
end;

procedure TIocpClientContextPool.TryCloseContext(AContext: TIocpClientContext);
begin
  if AContext.IsBusying then
  begin
    AContext.IsWaitingGiveBack := True;
  end
  else
  begin
    AContext.CloseContextSocket(ERROR_SUCCESS, cfOther);
  end;
end;

procedure TIocpClientContextPool.Clear;
var
  I: Integer;
begin
  Lock;
  try
    FOnlineContextList.Clear;
    FNoUseContextList.Clear;
    for I := 0 to FContextList.Count - 1 do
    begin
    {$IFDEF SHARE_POOL_MODE}
      //TIocpClientContext(FContextList.Items[I]).FIocpCore := nil;
    {$ENDIF}
    
      TIocpClientContext(FContextList.Items[I]).Free;
    end;
    FContextList.Clear;
  finally
    UnLock;
  end;
end;

// 清除所有在线用户，不断线而已；这个函数仅仅是用来恶心破解的人
procedure TIocpClientContextPool.ClearOnlineContexts;
var
  I: Integer;
begin
  Lock;
  try
    for I := 0 to FOnlineContextList.Count - 1 do
    begin
      FNoUseContextList.Add(FOnlineContextList.Items[I]);
    end;
  finally
    UnLock;
  end;
end;

procedure TIocpClientContextPool.GetOnlineContextList(List: TList);
var
  I: Integer;
begin
  Lock;
  try
    for I := FOnlineContextList.Count - 1 downto 0 do
      List.Add(FOnlineContextList.Items[I]);
  finally
    UnLock;
  end;
end;

function TIocpClientContextPool.GetOnlineContextCount: Integer;
begin
  Result := FOnlineContextList.Count;
end;

function TIocpClientContextPool.GetNoUseContextCount: Integer;
begin
  Result := FNoUseContextList.Count;
end;

function TIocpClientContextPool.GetContextCount: Integer;
begin
  Result := FContextList.Count;
end;

function TIocpClientContextPool.GetContexts(Index: Integer): TIocpClientContext;
begin
  Result := nil;
  Lock;
  try
    if (Index >= 0) and (Index < FContextList.Count) then
      Result := FContextList[Index];
  finally
    UnLock;
  end;
end;

//-----------------------------------------------------------------------------

{ TIocpTcpServer }

constructor TIocpTcpServer.Create(AOwner: TComponent);
begin
  inherited;
  FActive := False;

  FIocpCore := TIocpCore.Create(Self);
  FIocpCore.OnRecvDataBuffer := FOnRecvDataBuffer;
  FIocpCore.OnSendDataBuffer := FOnSendDataBuffer;
  FIocpCore.OnError := FOnError;

  FAccetpThread := nil;    // 2020-04-18 18:33:32s

{$IFNDEF SHARE_POOL_MODE}
  FClientContextPool := TIOCPClientContextPool.Create(Self);
  FIODataPool := TIODataPool.Create;
{$ENDIF}

  FSystemSocketHeartState := False;

  FBindObject := nil;
end;

destructor TIocpTcpServer.Destroy;
begin
  Active := False;

  if FAccetpThread <> nil then           // 2020-04-18 18:33:32s
  begin
    if FAccetpThread.Suspended then
      FAccetpThread.Resume;

    FAccetpThread.Terminate;

  {$IF NEED_REGISTER = 0}
    OutputDebugString('等待IOCP接受线程退出');
  {$IFEND}

    while FAccetpThread.FIsRun do
    begin
      Sleep(10);
      Application.ProcessMessages;
    end;

    FAccetpThread.Free;
  end;

{$IF NEED_REGISTER = 0}
  OutputDebugString('IOCP接受线程释放');
{$IFEND}

  FIocpCore.Free;

{$IFNDEF SHARE_POOL_MODE}
  FClientContextPool.Free;
  FIODataPool.Free;
{$ENDIF}

  inherited;
end;

{
procedure TIocpTcpServer.OnAccetpThreadTerminate(Sender: TObject);
begin

end;
}

procedure TIocpTcpServer.SetAddress(const Value: string);
begin
  if not FActive then
  begin
    FAddress := Value;
  end;
end;

procedure TIocpTcpServer.SetPort(const Value: Word);
begin
  if not FActive then
  begin
    FPort := Value;
  end;
end;

procedure TIocpTcpServer.SetActive(const Value: Boolean);
var
  I: Integer;
  SockAddrIn: TSockAddrIn;
  Context: TIocpClientContext;
  List: TList;
begin
  if FActive <> Value then
  begin
    if Value then
    begin
      FIocpCore.Socket := WSASocket(AF_INET, SOCK_STREAM, {IPPROTO_IP}IPPROTO_TCP, nil, 0, WSA_FLAG_OVERLAPPED);
      if FIocpCore.Socket = INVALID_SOCKET then
      begin
        DoError('TIocpTcpServer WSASocket error', Iocpet_Error);
        Exit;
      end;

      if SystemSocketHeartState then
      begin
        if not InitializeSocketHeart(FIocpCore.Socket) then
        begin
          DoError('TIocpTcpServer InitializeSocketHeart error', Iocpet_Error);
          CloseSocket(FIocpCore.Socket);
          FIocpCore.Socket := INVALID_SOCKET;
          Exit;
        end;
      end;

      SockAddrIn.sin_family := AF_INET;
      SockAddrIn.sin_port := htons(FPort);

      if Length(FAddress) = 0 then
        SockAddrIn.sin_addr.s_addr := htonl(INADDR_ANY)
      else
      begin
        SockAddrIn.sin_addr.S_addr := inet_addr(PChar(FAddress));
      end;

      if Bind(FIocpCore.Socket, TSockAddr(SockAddrIn), SizeOf(SockAddrIn)) = SOCKET_ERROR then
      begin
        DoError('TIocpTcpServer Bind error', Iocpet_Error);
        CloseSocket(FIocpCore.Socket);
        FIocpCore.Socket := INVALID_SOCKET;
        Exit;
      end;

      // If no error occurs, listen returns zero. Otherwise,
      // a value of SOCKET_ERROR is returned, and a specific error code
      // can be retrieved by calling WSAGetLastError.
      if Listen(FIocpCore.Socket, 20) = SOCKET_ERROR then
      begin
        DoError('TIocpTcpServer Listen error', Iocpet_Error);
        CloseSocket(FIocpCore.Socket);
        FIocpCore.Socket := INVALID_SOCKET;
        Exit;
      end;

      if FAccetpThread <> nil then                 // 2020-04-18 18:33:32s
      begin
        FAccetpThread.Terminate;
        Sleep(10);
        FreeAndNil(FAccetpThread);
      end;

      FAccetpThread := TAcceptListenerThread.Create(FIocpCore);
      FAccetpThread.FTcpServer := Self;
      FAccetpThread.Resume;

      FActive := True;
    end
    else
    begin
      List := TList.Create;
      try
      {$IFDEF SHARE_POOL_MODE}
        TIocpClientContextPool.Instance.Lock;
        try
          for I := 0 to TIocpClientContextPool.Instance.ContextCount - 1 do
          begin
            Context := TIocpClientContextPool.Instance.FContextList[I];
            List.Add(Context)
          end;
        finally
          TIocpClientContextPool.Instance.UnLock;
        end;
      {$ELSE}
        FClientContextPool.Lock;
        try
          for I := 0 to FClientContextPool.ContextCount - 1 do
          begin
            Context := FClientContextPool.FContextList[I];
            List.Add(Context);
          end;
        finally
          FClientContextPool.UnLock;
        end;
      {$ENDIF}
      
        for I := List.Count - 1 downto 0 do
        begin
          Context := List.Items[I];
          Context.CloseContextSocket(ERROR_SUCCESS, cfOther);
        end;
      finally
        List.Free;
      end;

      if FIocpCore.Socket <> INVALID_SOCKET then
      begin
        CloseSocket(FIocpCore.Socket);
        FIocpCore.Socket := INVALID_SOCKET;
      end;

      if FAccetpThread <> nil then            // 2020-04-18 18:33:32s
      begin
        FAccetpThread.Terminate;
        Sleep(10);
        FreeAndNil(FAccetpThread);
      end;

      FActive := False;
    end;
  end;
end;

procedure TIocpTcpServer.SetOnContextConnect(const Value: TIocpContextConnectEvent);
begin
  FOnContextConnect := Value;
  FIocpCore.OnContextConnect := Value;
end;

procedure TIocpTcpServer.SetOnContextDisconnect(const Value: TIocpContextDisonnectEvent);
begin
  FOnContextDisconnect := Value;
  FIocpCore.OnContextDisconnect := Value;
end;

procedure TIocpTcpServer.SetOnError(const Value: TIocpErrorEvent);
begin
  FOnError := Value;
  FIocpCore.OnError := Value;
end;

procedure TIocpTcpServer.SetOnRecvDataBuffer(const Value: TIocpDataBufferEvent);
begin
  FOnRecvDataBuffer := Value;
  FIocpCore.OnRecvDataBuffer := Value;
end;

procedure TIocpTcpServer.SetOnSendDataBuffer(const Value: TIocpDataBufferEvent);
begin
  FOnSendDataBuffer := Value;
  FIocpCore.OnSendDataBuffer := Value;
end;

procedure TIocpTcpServer.DoError(ErrorStr: string; ErrorType: TIocpErrorType; ErrorCode: Integer);
begin
  if Assigned(FOnError) then
    FOnError(FIocpCore, ErrorStr, ErrorType, ErrorCode);
end;

//------------------------------------------------------------------------------

{$IFDEF SHARE_POOL_MODE}
procedure ClearClientContextPoolInstance_IocpCore;
var
  I: Integer;
begin
  _ClientContextPoolInstance.Lock;
  try
    for I := 0 to _ClientContextPoolInstance.FContextList.Count - 1 do
    begin
      TIocpClientContext(_ClientContextPoolInstance.FContextList.Items[I]).IocpCore := nil;
    end;
  finally
    _ClientContextPoolInstance.UnLock;
  end;

end;
{$ENDIF}

procedure TIocpTcpServer.DoAcceptConnect(RemoteAddr: string;
  RemotePort: Word; var IsAccept: Boolean);
begin
  if Assigned(FOnAcceptConnect) then
    FOnAcceptConnect(Self, RemoteAddr, RemotePort, IsAccept);
end;

initialization
begin
{$IFDEF SHARE_POOL_MODE}
  _ClientContextPoolInstance := TIocpClientContextPool.Create;
{$ENDIF}
end;

finalization
begin
{$IFDEF SHARE_POOL_MODE}
  ClearClientContextPoolInstance_IocpCore;
  _ClientContextPoolInstance.Free;
  _ClientContextPoolInstance := nil;
{$ENDIF}
end;

end.
 