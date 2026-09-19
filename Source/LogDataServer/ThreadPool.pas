{-----------------------------------------------------------------------------

  单元说明： 线程池模型
  创建日期:  2013-1-10
    创建人:  虫虫(huan_2004@163.com)
    版本号:  v1.0

  修改记录:  v1.0 2013-1-10<p/>
                  + 创建单元，实现基本功能<p/><p/>

------------------------------------------------------------------------------}

unit ThreadPool;

interface

uses
  Windows, Classes, SysUtils, SyncObjs, RTLConsts, Forms;

type
  TPoolManager = class;

  TPoolTask = class(TObject)
  private
    FCanceled: Boolean;
    FFinished: Boolean;
    procedure AssignError(Source: TPoolTask);
  protected
    procedure AssignTo(Dest: TPoolTask); virtual;
  public
    constructor Create; virtual;
    procedure Cancel;
    property Canceled: Boolean read FCanceled;
    property Finished: Boolean read FFinished write FFinished;
    function CompareTask(Task: TPoolTask): Boolean; virtual; abstract;
    procedure Assign(Source: TPoolTask); virtual;
  end;

  TPoolThread = class(TThread)
  private
    FOwner: TPoolManager;
    FEvent: TEvent;
    FInExecuteLoop: Boolean;
    function GetSleeping: Boolean;
  protected
    FContextTask: TPoolTask;

    procedure Execute; override;
    procedure TriggerEvent;

    function GetTerminated: Boolean; virtual;

    { 线程开始运行前 }
    procedure DoExecInitialize; virtual;

    { 线程结束运行时 }
    procedure DoExecFinalize; virtual;

    { 线程不停的执行 }
    procedure DoExecuteLoop; virtual; abstract;

    procedure DoTaskFinished;
  public
    constructor Create(AOwner: TPoolManager; CreateSuspended: Boolean); virtual;
    destructor Destroy; override;
		procedure Terminate; reintroduce; virtual;

    property Sleeping: Boolean read GetSleeping;
    property Owner: TPoolManager read FOwner;
    property ContextTask: TPoolTask read FContextTask;
  end;

  TPoolThreadClass = class of TPoolThread;
  TTaskFinishedEvent = procedure(Manager: TPoolManager; Thread: TPoolThread; Task: TPoolTask) of object;
  TPoolManager = class(TObject)
  private
    FIsWantDestroy: Boolean;

    FThreads: TList;
    FTasks: TList;
    FSectionTask: TCriticalSection;
    FOnTaskFinished: TTaskFinishedEvent;

    function GetSleepingThread: TPoolThread;
    function GetTaskCount: Integer;
    function GetThreadCount: Integer;
    function GetTasks(Index: Integer): TPoolTask;
    function GetThreads(Index: Integer): TPoolThread;
  protected
    function GetPoolThreadClass: TPoolThreadClass; virtual;
    procedure DoTaskFinished(Thread: TPoolThread; Task: TPoolTask); virtual;
  public
    constructor Create(ThreadCount: Integer); virtual;
    destructor Destroy; override;

    procedure AddTask(Task: TPoolTask); virtual;
    procedure ClearTasks;
    procedure CancelAndClearAllTask;

    function FindTask(Task: TPoolTask): TPoolTask; virtual; 

    property TaskCount: Integer read GetTaskCount;
    property Tasks[Index: Integer]: TPoolTask read GetTasks;

    property ThreadCount: Integer read GetThreadCount;
    property Threads[Index: Integer]: TPoolThread read GetThreads;

    property OnTaskFinished: TTaskFinishedEvent read FOnTaskFinished write FOnTaskFinished;

    function LockTaskList: TList;
    procedure UnlockTaskList;

    property IsWantDestroy: Boolean read FIsWantDestroy;
  end;


implementation

{ TPoolTask }

constructor TPoolTask.Create;
begin
  FCanceled := False;
end;

procedure TPoolTask.Assign(Source: TPoolTask);
begin
  if Source <> nil then Source.AssignTo(Self) else AssignError(nil);
end;

procedure TPoolTask.AssignError(Source: TPoolTask);
var
  SourceName: string;
begin
  if Source <> nil then
    SourceName := Source.ClassName else
    SourceName := 'nil';
  raise EConvertError.CreateResFmt(@SAssignError, [SourceName, ClassName]);
end;

procedure TPoolTask.AssignTo(Dest: TPoolTask);
begin
  Dest.AssignError(Self);
end;

procedure TPoolTask.Cancel;
begin
  FCanceled := True;
end;

{ TPoolThread }

constructor TPoolThread.Create(AOwner: TPoolManager; CreateSuspended: Boolean);
begin
  inherited Create(CreateSuspended);
  FreeOnTerminate := False;
  FContextTask := nil;
  FOwner := AOwner;

  {
    TEvent.Create
    参数2, Fase 事件对象控制一次后将立即重置(暂停); True 可手动暂停
    参数3, Fase 对象建立后控制为暂停状态; True 可运行状态
  }
  FEvent := TEvent.Create(nil, False, False, '');
end;

destructor TPoolThread.Destroy;
begin
  FEvent.Free;
  if Assigned(FContextTask) then
    FreeAndNil(FContextTask);
  inherited;
end;

procedure TPoolThread.DoExecInitialize;
begin

end;

procedure TPoolThread.DoExecFinalize;
begin

end;

procedure TPoolThread.DoTaskFinished;
begin
  FOwner.DoTaskFinished(Self, FContextTask);
end;

procedure TPoolThread.Execute;
begin
  DoExecInitialize;
  try
    while not GetTerminated do
    begin
      FEvent.WaitFor(INFINITE);
      FInExecuteLoop := True;
      try
        DoExecuteLoop;
      finally
        FInExecuteLoop := False;
      end;
    end;
  finally
    DoExecFinalize;
  end;
end;

function TPoolThread.GetSleeping: Boolean;
begin
  Result := (not FInExecuteLoop) and (not Suspended);
end;

function TPoolThread.GetTerminated: Boolean;
begin
  Result := Terminated;
end;

procedure TPoolThread.Terminate;
begin
  if Assigned(FContextTask) then FContextTask.Cancel;
  inherited Terminate;
  TriggerEvent;
end;

procedure TPoolThread.TriggerEvent;
begin
  FEvent.SetEvent;
end;

{ TPoolManager }

constructor TPoolManager.Create(ThreadCount: Integer);
var
  I: Integer;
  Thread: TPoolThread;
begin
  FIsWantDestroy := False;
  FThreads := TList.Create;
  FTasks   := TList.Create;
  FSectionTask := TCriticalSection.Create;

  for I := 0 to ThreadCount - 1 do
  begin
    Thread := GetPoolThreadClass.Create(Self, True);
    FThreads.Add(Thread);
  end;

  for I := 0 to FThreads.Count - 1 do
  begin
    Thread := FThreads[I];
    Thread.Resume;
  end;
end;

destructor TPoolManager.Destroy;
var
  I: Integer;
  Thread: TPoolThread;
begin
  FIsWantDestroy := True;
  CancelAndClearAllTask;

  for I := 0 to FThreads.Count - 1 do
  begin
    Thread := FThreads[I];
    while not Thread.Sleeping do
    begin
      Sleep(1);
      Application.ProcessMessages;
    end;
    Thread.Terminate;
  end;

  for I := 0 to FThreads.Count - 1 do
  begin
    Thread := FThreads[I];
    //Thread.WaitFor;
    Thread.Free;
  end;
  FThreads.Free;

  ClearTasks;
  FTasks.Free;

  FSectionTask.Free;
  inherited;
end;

procedure TPoolManager.DoTaskFinished(Thread: TPoolThread; Task: TPoolTask);
begin
  if Assigned(FOnTaskFinished) then
    FOnTaskFinished(Self, Thread, Task);
end;

function TPoolManager.FindTask(Task: TPoolTask): TPoolTask;
var
  I: Integer;
  Thread: TPoolThread;
  ContextTask: TPoolTask;
begin
  Result := nil;

  for I := 0 to FThreads.Count - 1 do
  begin
    Thread := FThreads[I];
    ContextTask := Thread.ContextTask;
    if (ContextTask <> nil) and (ContextTask.CompareTask(Task)) then
    begin
      Result := ContextTask;
      Exit;
    end;
  end;

  FSectionTask.Enter;
  try
    for I := 0 to FTasks.Count - 1 do
    begin
      ContextTask := FTasks[I];
      if ContextTask.CompareTask(Task) then
      begin
        Result := ContextTask;
        Break;
      end;
    end;
  finally
    FSectionTask.Leave;
  end;
end;

procedure TPoolManager.AddTask(Task: TPoolTask);
var
  SleepThread: TPoolThread;
begin
  FSectionTask.Enter;
  try
    FTasks.Add(Task);
  finally
    FSectionTask.Leave;
  end;

  { 如果有空闲线程，把任务加进去，并让空闲线程执行 }
  SleepThread := GetSleepingThread;
  if SleepThread <> nil then
  begin
    SleepThread.TriggerEvent;
    // 小停片刻，让空闲线程先跑起来，不然 AddTask 速度太快，而空闲线程还未置为非空闲
    //Sleep(5);
  end;
end;

procedure TPoolManager.CancelAndClearAllTask;
var
  I: Integer;
  Thread: TPoolThread;
begin
  ClearTasks;

  for I := 0 to FThreads.Count - 1 do
  begin
    Thread := FThreads[I];
    if (not Thread.GetSleeping) and Assigned(Thread.FContextTask) then
    begin
      Thread.FContextTask.Cancel;
    end;
  end;
end;

procedure TPoolManager.ClearTasks;
var
  I: Integer;
  Task: TPoolTask;
begin
  FSectionTask.Enter;
  try
    for I := 0 to FTasks.Count - 1 do
    begin
      Task := FTasks.Items[I];
      Task.Free;
    end;
    FTasks.Clear;
  finally
    FSectionTask.Leave;
  end;
end;

function TPoolManager.GetPoolThreadClass: TPoolThreadClass;
begin
  Result := TPoolThread;
end;

function TPoolManager.GetSleepingThread: TPoolThread;
var
  I: Integer;
  Thread: TPoolThread;
begin
  Result := nil;
  for I := 0 to FThreads.Count - 1 do
  begin
    Thread := FThreads[I];
    if Thread.GetSleeping then
    begin
      Result := Thread;
      Break;
    end;
  end;
end;

function TPoolManager.LockTaskList: TList;
begin
  FSectionTask.Enter;
  Result := FTasks;
end;

procedure TPoolManager.UnlockTaskList;
begin
  FSectionTask.Leave;
end;

function TPoolManager.GetTaskCount: Integer;
begin
  FSectionTask.Enter;
  Result := FTasks.Count;
  FSectionTask.Leave;
end;

function TPoolManager.GetThreadCount: Integer;
begin
  Result := FThreads.Count;
end;

function TPoolManager.GetTasks(Index: Integer): TPoolTask;
begin
  FSectionTask.Enter;
  Result := FTasks[Index];
  FSectionTask.Leave;
end;

function TPoolManager.GetThreads(Index: Integer): TPoolThread;
begin
  Result := FThreads[Index];
end;

end.
