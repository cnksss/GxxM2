unit IocpCommon;

interface

{$I Iocp.inc}

uses
  Windows, IocpWinsock2, Classes, MMSystem;

const
  IOCP_QUEUED_SHUTDOWN = $FFFFFFFF;

  // 和M2通讯是否采用IocpClient组件
  UseIocpClient = 1;

{$IF UseIocpClient <> 0}
  MAX_IOCP_CLIENT_RECV_BUFFER_SIZE: Integer = 1 shl 20;  //1M
{$IFEND}

var
  //每次接收最大的字节数
  //OVERLAPPEDEx.DataBuf中每次分配空间数
  //每次发送最大的字节数
  MAX_OVERLAPPEDEX_BUFFER_SIZE: LongWord = 5;  //5KBytes

  MAX_PREALLOCATED_MEMORY_SIZE: Integer = 1024;

type
  TIoType = (itNone, itConnect, itRecv, itSend, itClose, itZeroByteRead, itZeroReadCompleted);

type
  TIocpErrorType = (Iocpet_Error, Iocpet_Info);

  POVERLAPPEDEx = ^OVERLAPPEDEx;
  OVERLAPPEDEx = record
    Overlapped: OVERLAPPED;
    DataBuf: TWSABUF;               // 这个地址地址必须是4的倍数，不然出10014错误

    IoType: TIoType;
    AllocSize: Integer;
  end;

  PWorkData = ^TWorkerData;
  TWorkerData = record
    IocpHandle: THandle;
    WorkerID: Cardinal;
  end;

  PSendBuffer = ^TSendBuffer;
  TSendBuffer = record
    Position: Cardinal;
    BufSize: Cardinal;
    Buffer: PChar;
  end;

  TSafeList = class(TList)
  private
  {$IFDEF USE_SPINLOCK}
    FName: string;
    FIsLock: Boolean;
    FLocker: Integer;
  {$ELSE}
    FCS: TRTLCriticalSection;          
  {$ENDIF}
  public
    constructor Create({$IFDEF USE_SPINLOCK}AName: string{$ENDIF});
    destructor Destroy; override;
    function TryLock: Boolean;
    procedure Lock;
    procedure UnLock;
  end;

  TIocpCriticalSection = class(TObject)
  private
  {$IFDEF USE_SPINLOCK}
    FName: string;
    FIsLock: Boolean;
  {$ENDIF}
  protected
  {$IFDEF USE_SPINLOCK}
    FSection: Integer;
  {$ELSE}
    FSection: TRTLCriticalSection;
  {$ENDIF}
  public
    constructor Create({$IFDEF USE_SPINLOCK}AName: string{$ENDIF});
    destructor Destroy; override;
    function TryLock: Boolean;
    procedure Lock;
    procedure UnLock;
  end;

const
  SIO_KEEPALIVE_VALS = IOC_IN or IOC_VENDOR or 4;

type
  TKeepAlive = record
    OnOff: Integer;
    KeepAliveTime: Integer;
    KeepAliveInterval: Integer;
  end;
  TTCP_KEEPALIVE = TKeepAlive;
  PTCP_KEEPALIVE = ^TKeepAlive;

  function InitializeSocketHeart(Socket: TSocket): Boolean;

{$IFDEF USE_SPINLOCK}
  procedure SpinLock(var Target: Integer; const LockName: string);
  procedure SpinUnLock(var Target: Integer; const LockName: string);
{$ENDIF}

  function MyGetTickCount: DWORD; stdcall; external mmsyst name 'timeGetTime';
  // function MyGetTickCount: DWORD; stdcall;; external kernel32 name 'GetTickCount';

implementation

uses
  GateShare;

function InterlockedCompareExchange(var Destination: Integer; Exchange: Integer; Comperand: Integer): Integer stdcall;external kernel32 name 'InterlockedCompareExchange';

function InitializeSocketHeart(Socket: TSocket): Boolean;
var
  Opt: integer;
  InSize, OutSize, OutBytes: DWORD;
  InKeepAlive, OutKeepAlive: TTCP_KEEPALIVE;
begin
  Result := False;
  Opt := 1;
  if SetSockopt(Socket, SOL_SOCKET, SO_KEEPALIVE, @Opt, SizeOf(Opt)) = SOCKET_ERROR then
  begin
    Exit;
  end;

  InKeepAlive.OnOff := 1;

  //设置30秒钟时间间隔
  InKeepAlive.KeepAliveTime := 5000;

  //设置每30秒中发送１次的心跳
  InKeepAlive.KeepAliveInterval := 1;

  InSize := SizeOf(TTCP_KEEPALIVE);
  OutSize := SizeOf(TTCP_KEEPALIVE);

  Result := WSAIoctl(Socket,
    SIO_KEEPALIVE_VALS,
    @InKeepAlive, InSize,
    @OutKeepAlive,
    OutSize, OutBytes, nil, nil) <> SOCKET_ERROR;
end;

{ TSafeList }

constructor TSafeList.Create({$IFDEF USE_SPINLOCK}AName: string{$ENDIF});
begin
  inherited Create;
{$IFDEF USE_SPINLOCK}
  FName := AName;
  FIsLock := False;
  FLocker := 0;
{$ELSE}
  InitializeCriticalSection(FCS);
{$ENDIF}
end;

destructor TSafeList.Destroy;
begin
{$IFDEF USE_SPINLOCK}
  FIsLock := False;
  FLocker := 0;
{$ELSE}
  DeleteCriticalSection(FCS);
{$ENDIF}
  inherited;
end;

function TSafeList.TryLock: Boolean;
begin
{$IFDEF USE_SPINLOCK}
  Result := not FIsLock;
  if Result then Lock;
{$ELSE}
  Result := TryEnterCriticalSection(FCS);
{$ENDIF}
end;

procedure TSafeList.Lock;
begin
{$IFDEF USE_SPINLOCK}
  FIsLock := True;
  SpinLock(FLocker, FName);
{$ELSE}
  EnterCriticalSection(FCS);
{$ENDIF}
end;

procedure TSafeList.UnLock;
begin
{$IFDEF USE_SPINLOCK}
  SpinUnLock(FLocker, FName);
  FIsLock := False;
{$ELSE}
  LeaveCriticalSection(FCS);
{$ENDIF}
end;

{$IFDEF USE_SPINLOCK}
procedure SpinLock(var Target: Integer; const LockName: string);
var
  StartTick: LongWord;
begin
  StartTick := MyGetTickCount;
  while InterlockedCompareExchange(Target, 1, 0) <> 0 do
  begin
    Sleep(0);
    if MyGetTickCount - StartTick >= 3000 then
    begin
      AddMainLogMsg('发现死锁:' + LockName , 1);
      Break;
    end;
  end;
end;

procedure SpinUnLock(var Target: Integer; const LockName: string);
var
  StartTick: LongWord;
begin
  StartTick := MyGetTickCount;
  while InterlockedCompareExchange(Target, 0, 1) <> 1 do
  begin
    Sleep(0);
    if MyGetTickCount - StartTick >= 3000 then
    begin
      AddMainLogMsg('发现死锁:' + LockName , 1);
      Break;
    end;
  end;
end;
{$ENDIF}

{ TIocpCriticalSection }

constructor TIocpCriticalSection.Create({$IFDEF USE_SPINLOCK}AName: string{$ENDIF});
begin
  inherited Create;
{$IFDEF USE_SPINLOCK}
  FName := AName;
  FIsLock := False;
  FSection := 0;
{$ELSE}
  InitializeCriticalSection(FSection);
{$ENDIF}
end;

destructor TIocpCriticalSection.Destroy;
begin
{$IFDEF USE_SPINLOCK}
  FIsLock := False;
  FSection := 0;
{$ELSE}
  DeleteCriticalSection(FSection);
{$ENDIF}
  inherited Destroy;
end;

function TIocpCriticalSection.TryLock: Boolean;
begin
{$IFDEF USE_SPINLOCK}
  Result := not FIsLock;
  if Result then Lock;
{$ELSE}
  Result := TryEnterCriticalSection(FSection);
{$ENDIF}
end;

procedure TIocpCriticalSection.Lock;
begin
{$IFDEF USE_SPINLOCK}
  FIsLock := True;
  SpinLock(FSection, FName);
{$ELSE}
  EnterCriticalSection(FSection);
{$ENDIF}
end;

procedure TIocpCriticalSection.UnLock;
begin
{$IFDEF USE_SPINLOCK}
  SpinUnLock(FSection, FName);
  FIsLock := False;
{$ELSE}
  LeaveCriticalSection(FSection);
{$ENDIF}
end;

end.
