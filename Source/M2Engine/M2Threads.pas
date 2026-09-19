unit M2Threads;

interface

uses
  Windows, Classes, SysUtils, MMSystem;

type
  TRunProcedure = procedure;

  TM2RunThread = class(TThread)
  private
    FRunTick: LongWord;
    FMinRunTick: LongWord;
    FMaxRunTick: LongWord;

    FThreadDesc: string;
    FRunProc: TRunProcedure;

    FStartByThread: Int64;
    FCPUPrevByThread: Int64;

    FMaxThreadCPUUsage: Integer;
  private
    function GetThreadCPUUsage: Int64;
  protected
    procedure Execute; override;
  public
    constructor Create(CreateSuspended: Boolean);
    destructor Destroy; override;

    property ThreadDesc: string read FThreadDesc;
    property RunTick: LongWord read FRunTick;
    property MinRunTick: LongWord read FMinRunTick;
    property MaxRunTick: LongWord read FMaxRunTick;

    function ThreadCPUUsage: Integer;
    property MaxThreadCPUUsage: Integer read FMaxThreadCPUUsage write FMaxThreadCPUUsage;
  end;

  TM2RunThreadMgr = class(TObject)
  private
    FThreads: TList;
    function GetCount: Integer;
    function GetItems(Index: Integer): TM2RunThread;
  public
    constructor Create;
    destructor Destroy; override;

    property Count: Integer read GetCount;
    property Items[Index: Integer]: TM2RunThread read GetItems;

    procedure Clear;
    function AddThread(AThreadDesc: string; ARunProc: TRunProcedure): TM2RunThread;
  end;

var
  g_M2RunThreadMgr: TM2RunThreadMgr;
  g_MultiThreadRun: Boolean = False;

implementation

uses
  M2Share;

{ TM2RunThread }

constructor TM2RunThread.Create(CreateSuspended: Boolean);
begin
  inherited Create(CreateSuspended);
  FreeOnTerminate := False;
  FThreadDesc := '';
  FRunProc := nil;

  FRunTick := 0;
  FMinRunTick := High(LongWord);
  FMaxRunTick := 0;

  FStartByThread := 0;
  FCPUPrevByThread := 0;
  TimeBeginPeriod(1);
  FMaxThreadCPUUsage := 0;
end;

destructor TM2RunThread.Destroy;
begin
  inherited;
end;

procedure TM2RunThread.Execute;
var
  StartRunTick: LongWord;
begin
  inherited;

  while not Terminated do
  begin
    StartRunTick := MyGetTickCount;

    if Assigned(FRunProc) then
      FRunProc;

    if MyGetTickCount >= StartRunTick then
      FRunTick := MyGetTickCount - StartRunTick
    else
      FRunTick := High(Cardinal) - StartRunTick + MyGetTickCount;

    if FRunTick < FMinRunTick then FMinRunTick := FRunTick;
    if FRunTick > FMaxRunTick then FMaxRunTick := FRunTick;

    Sleep(1);
  end;
end;

function TM2RunThread.GetThreadCPUUsage: Int64;
var
  CreationTime, ExitTime, KernelTime, UserTime: TFileTime;
begin
  try
    Result := 0;
    if GetThreadTimes(Handle, CreationTime, ExitTime, KernelTime, UserTime) then
    begin
      Result := Int64(KernelTime.dwLowDateTime or (KernelTime.dwHighDateTime shr 32)) + Int64(UserTime.dwLowDateTime or (UserTime.dwHighDateTime shr 32));
    end;
  except
    Result := 0;
  end;
end;

function TM2RunThread.ThreadCPUUsage: Integer;
const
  bWaitTime = 700;
var
  timespan, dtThread: Int64;
begin
  try
    Result := 0;
    timespan := 0;
    if (FStartByThread > 0) and (FCPUPrevByThread > 0) then
    begin
      timespan := timeGetTime() - FStartByThread;
      dtThread := GetThreadCPUUsage - FCPUPrevByThread;
      if (timespan >= bWaitTime) and (dtThread > 0) then
        Result := Round(((dtThread / timespan) / 100) / 2) //这里就是取CPU使用率了,不知为何总要除以2
      else
        Result := 0;
    end;
    if (timespan <= 0) or (timespan >= bWaitTime) then
      FStartByThread := timeGetTime();
    FCPUPrevByThread := GetThreadCPUUsage;
  except
    Result := 0;
  end;
end;

{ TM2RunThreadMgr }

constructor TM2RunThreadMgr.Create;
begin
  FThreads := TList.Create;
end;

destructor TM2RunThreadMgr.Destroy;
begin
  Clear;
  FThreads.Free;
  inherited;
end;

function TM2RunThreadMgr.AddThread(AThreadDesc: string;
  ARunProc: TRunProcedure): TM2RunThread;
begin
  Result := TM2RunThread.Create(True);

  Result.FThreadDesc := AThreadDesc;
  Result.FRunProc := ARunProc;
  FThreads.Add(Result);
{$IF CompilerVersion >= 22}
  Result.Suspended := False;
{$ELSE}
  Result.Resume;
{$IFEND}
end;

procedure TM2RunThreadMgr.Clear;
var
  I: Integer;
  Thread: TM2RunThread;
begin
  for I := 0 to FThreads.Count - 1 do
  begin
    Thread := FThreads.Items[I];
    Thread.Terminate;
  end;

  for I := 0 to FThreads.Count - 1 do
  begin
    Thread := FThreads.Items[I];
    Thread.WaitFor;

    Thread.Free;
  end;

  FThreads.Clear;
end;

function TM2RunThreadMgr.GetCount: Integer;
begin
  Result := FThreads.Count;
end;

function TM2RunThreadMgr.GetItems(Index: Integer): TM2RunThread;
begin
  if (Index >= 0) and (Index < FThreads.Count) then
    Result := FThreads.Items[Index]
  else
    Result := nil;
end;

initialization
  g_M2RunThreadMgr := TM2RunThreadMgr.Create;

finalization
  g_M2RunThreadMgr.Free;

end.
