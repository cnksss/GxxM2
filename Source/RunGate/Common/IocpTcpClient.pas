unit IocpTcpClient;

interface

{$I iocp.inc}

uses
  Classes, SysUtils, Windows, SyncObjs, IocpWinsock2, IocpCommon,
  {$IFDEF USE_BUFFER_LINK}uBuffer,{$ENDIF} IODataPool, IocpUtils;

type
  TIocpTcpClient = class;

  TIocpRemoteContext = class(TIocpContext)
  private
    FOwner: TIocpTcpClient;
    FAddress: string;
    FPort: Word;
    FActive: Boolean;
    procedure SetActive(const Value: Boolean);
  protected
    // 关闭客户端连接
    procedure CloseContextSocket(ErrCode: Integer; CloseFrom: TCloseFrom); override;

    procedure DoConnect; override;
    procedure DoDisconnect(ASocket: TSocket); override;

    procedure DoReset(IsClose: Boolean); override;
  public
    constructor Create(AIocpCore: TIocpCore; ASocket: TSocket = 0); override;
    destructor Destroy; override;

    property Owner: TIocpTcpClient read FOwner;
    property Address: string read FAddress write FAddress;
    property Port: Word read FPort write FPort;

    property Active: Boolean read FActive write SetActive;
  end;

  TIocpRemoteContextClass = class of TIocpRemoteContext;

  TIocpTcpClient = class(TComponent)
  private
    FIocpCore: TIocpCore;

    FOnContextConnect: TIocpContextConnectEvent;
    FOnContextDisconnect: TIocpContextDisonnectEvent;
    FOnRecvDataBuffer: TIocpDataBufferEvent;
    FOnSendDataBuffer: TIocpDataBufferEvent;
    FOnError: TIocpErrorEvent;

    FList: TList;

    FContextClass: TIocpRemoteContextClass;

    procedure SetOnContextConnect(const Value: TIocpContextConnectEvent);
    procedure SetOnContextDisconnect(const Value: TIocpContextDisonnectEvent);
    procedure SetOnError(const Value: TIocpErrorEvent);
    procedure SetOnRecvDataBuffer(const Value: TIocpDataBufferEvent);
    procedure SetOnSendDataBuffer(const Value: TIocpDataBufferEvent);

    function GetCount: Integer;
    function GetItems(Index: Integer): TIocpRemoteContext;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;

    property IocpCore: TIocpCore read FIocpCore;

    procedure RegisterContextClass(AContextClass: TIocpRemoteContextClass);
    procedure ClearContexts;

    function Add: TIocpRemoteContext;
    property Count: Integer read GetCount;
    property Items[Index: Integer]: TIocpRemoteContext read GetItems; default;

    property OnContextConnect: TIocpContextConnectEvent read FOnContextConnect write SetOnContextConnect;
    property OnContextDisconnect: TIocpContextDisonnectEvent read FOnContextDisconnect write SetOnContextDisconnect;
    property OnRecvDataBuffer: TIocpDataBufferEvent read FOnRecvDataBuffer write SetOnRecvDataBuffer;
    property OnSendDataBuffer: TIocpDataBufferEvent read FOnSendDataBuffer write SetOnSendDataBuffer;
    property OnError: TIocpErrorEvent read FOnError write SetOnError;
  end;

implementation

{ TIocpRemoteContext }

constructor TIocpRemoteContext.Create(AIocpCore: TIocpCore;
  ASocket: TSocket);
begin
  inherited Create(AIocpCore, ASocket);
end;

destructor TIocpRemoteContext.Destroy;
begin
  CloseSocket(0);
  inherited;
end;

procedure TIocpRemoteContext.CloseContextSocket(ErrCode: Integer; CloseFrom: TCloseFrom);
begin
  inherited CloseContextSocket(ErrCode, CloseFrom);
end;

procedure TIocpRemoteContext.DoConnect;
begin
  inherited DoConnect;
  FActive := True;
end;

procedure TIocpRemoteContext.DoDisconnect(ASocket: TSocket);
begin
  inherited DoDisconnect(ASocket);
  FActive := False;
end;

procedure TIocpRemoteContext.DoReset(IsClose: Boolean);
begin
  inherited;
  FActive := False;
end;

procedure TIocpRemoteContext.SetActive(const Value: Boolean);
type
  TIocpConnectEx = function(const s : TSocket; const name: PSOCKADDR;
      const namelen: Integer; lpSendBuffer: Pointer; dwSendDataLength: DWORD;
      var lpdwBytesSent: DWORD; lpOverlapped: LPWSAOVERLAPPED): BOOL; stdcall;

const
  WSAID_CONNECTEX: TGuid = (D1:$25a207b9;D2:$ddf3;D3:$4660;D4:($8e,$e9,$76,$e5,$8c,$74,$06,$3e));
var
  SockAddrIn: TSockAddrIn;
  lvIOPort: THandle;
  IocpConnectEx: TIocpConnectEx;
  lvCode: Integer;
  dwBytesSend, dwBytesReturn: Cardinal;
  lvIOData: POVERLAPPEDEx;
begin
  if FActive = Value then
  begin
    Exit;
  end;

  if Value then
  begin
    {
    // 请求连接已经发过去了。但是还未连接上的时候 断开 chongchong 2016-08-17
    if Socket <> INVALID_SOCKET then
    begin
      CloseSocket(0);
      Socket := INVALID_SOCKET;
    end;
    }
    
    Socket := WSASocket(AF_INET, SOCK_STREAM, {IPPROTO_IP}IPPROTO_TCP, nil, 0, WSA_FLAG_OVERLAPPED);

    if Socket = INVALID_SOCKET then
    begin
      //DoError('TIocpCore.Initialize WSASocket error', Iocpet_Error);
      CloseSocket(Socket);
      Socket := INVALID_SOCKET;
      Exit;
    end;

    SockAddrIn.sin_family := AF_INET;
    SockAddrIn.sin_port := htons(0);
    SockAddrIn.sin_addr.S_addr := htonl(INADDR_ANY);

    if Bind(Socket, TSockAddr(SockAddrIn), SizeOf(SockAddrIn)) = SOCKET_ERROR then
    begin
      //DoError('TIocpCore.Initialize Bind error', Iocpet_Error);
      CloseSocket(Socket);
      Socket := INVALID_SOCKET;
      Exit;
    end;

    lvIOPort := CreateIoCompletionPort(Socket, IocpCore.IocpHandle, Cardinal(Self), 0);
    if lvIOPort = 0 then
    begin
      CloseSocket(Socket);
      Exit;
    end;

    // 加载 IocpConnectEx 函数
    lvCode := WSAIoctl(Socket,
          SIO_GET_EXTENSION_FUNCTION_POINTER,
          @WSAID_CONNECTEX,
          SizeOf(WSAID_CONNECTEX),
          @@IocpConnectEx,
          SizeOf(Pointer),
          dwBytesReturn,
          nil,
          nil);

    if lvCode <> 0 then
    begin
      CloseSocket(Socket);
      Socket := INVALID_SOCKET;
      Exit;
    end;

    SockAddrIn.sin_family := AF_INET;
    SockAddrIn.sin_port := htons(FPort);

    if Length(FAddress) = 0 then
      SockAddrIn.sin_addr.s_addr := htonl(INADDR_ANY)
    else
    begin
      SockAddrIn.sin_addr.S_addr := inet_addr(PChar(FAddress));
    end;

    // 请求连接到服务器>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

  {$IFDEF SHARE_POOL_MODE}
    lvIOData := TIODataPool.Instance.GetNewIOData(0);
  {$ELSE}
    lvIOData := FIODataPool.GetNewIOData(0);
  {$ENDIF}
    lvIOData.IoType := itConnect;
    dwBytesSend := 0;
    
    IocpConnectEx(Socket,
      @SockAddrIn,
      SizeOf(SockAddrIn),
      @lvIOData.DataBuf,
      0,
      dwBytesSend,
      @lvIOData.Overlapped
    );
  end
  else
  begin
    CloseSocket(0);
    Socket := INVALID_SOCKET;
    FActive := False;
  end;
end;

{ TIocpTcpClient }

constructor TIocpTcpClient.Create(AOwner: TComponent);
begin
  inherited;
  FIocpCore := TIocpCore.Create(Self);
  FIocpCore.OnRecvDataBuffer := FOnRecvDataBuffer;
  FIocpCore.OnSendDataBuffer := FOnSendDataBuffer;
  FIocpCore.OnError := FOnError;

  FList := TList.Create;

  FContextClass := nil;
end;

destructor TIocpTcpClient.Destroy;

begin
  FIocpCore.Free;

  ClearContexts;
  FList.Free;

  inherited;
end;

procedure TIocpTcpClient.ClearContexts;
var
  I: Integer;
begin
  for I := 0 to FList.Count - 1 do
  begin
    TIocpRemoteContext(FList.Items[I]).Free;
  end;
  FList.Clear;
end;

function TIocpTcpClient.Add: TIocpRemoteContext;
begin
  if FContextClass = nil then
  begin
    Result := TIocpRemoteContext.Create(FIocpCore, INVALID_SOCKET);
  end
  else
  begin
    Result := TIocpRemoteContext(FContextClass.Create(FIocpCore, INVALID_SOCKET));
  end;

  Result.FOwner := Self;
  FList.Add(Result);
end;

function TIocpTcpClient.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TIocpTcpClient.GetItems(Index: Integer): TIocpRemoteContext;
begin
  if (Index >= 0) and (Index < FList.Count) then
    Result := FList.Items[Index]
  else
    Result := nil;
end;

procedure TIocpTcpClient.SetOnContextConnect(const Value: TIocpContextConnectEvent);
begin
  FOnContextConnect := Value;
  FIocpCore.OnContextConnect := Value;
end;

procedure TIocpTcpClient.SetOnContextDisconnect(const Value: TIocpContextDisonnectEvent);
begin
  FOnContextDisconnect := Value;
  FIocpCore.OnContextDisconnect := Value;
end;

procedure TIocpTcpClient.SetOnError(const Value: TIocpErrorEvent);
begin
  FOnError := Value;
  FIocpCore.OnError := Value;
end;

procedure TIocpTcpClient.SetOnRecvDataBuffer(const Value: TIocpDataBufferEvent);
begin
  FOnRecvDataBuffer := Value;
  FIocpCore.OnRecvDataBuffer := Value;
end;

procedure TIocpTcpClient.SetOnSendDataBuffer(const Value: TIocpDataBufferEvent);
begin
  FOnSendDataBuffer := Value;
  FIocpCore.OnSendDataBuffer := Value;
end;

procedure TIocpTcpClient.RegisterContextClass(AContextClass: TIocpRemoteContextClass);
begin
  FContextClass := AContextClass;
end;


end.
