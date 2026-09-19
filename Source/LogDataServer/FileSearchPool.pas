unit FileSearchPool;

interface

uses
  Windows, Classes, SysUtils, ThreadPool, StdCtrls, ExtCtrls, ComCtrls,
  LDShare, Hutil32, StrUtils;

type
  TCustomMemoryStreamEx = class(TStream)
  private
    FMemory: Pointer;
    FSize, FPosition: Longint;
  protected
    procedure SetPointer(Ptr: Pointer; Size: Longint);
  public
    function Read(var Buffer; Count: Longint): Longint; override;
    function Seek(Offset: Longint; Origin: Word): Longint; override;
    procedure SaveToStream(Stream: TStream);
    procedure SaveToFile(const FileName: string);
    property Memory: Pointer read FMemory;
  end;

  TMemoryStreamEx = class(TCustomMemoryStreamEx)
  private
    FCapacity: Longint;
    procedure SetCapacity(NewCapacity: Longint);
  protected
    function Realloc(var NewCapacity: Longint): Pointer; virtual;
    property Capacity: Longint read FCapacity write SetCapacity;
  public
    destructor Destroy; override;
    procedure Clear;
    procedure LoadFromStream(Stream: TStream);
    procedure LoadFromFile(const FileName: string);
    procedure SetSize(NewSize: Longint); override;
    function Write(const Buffer; Count: Longint): Longint; override;
  end;

  TSearchTask = class(TPoolTask)
  private
    FTaskID: Integer;
    FFileName: string;
    FPanel: TStatusPanel;
  protected
    procedure AssignTo(Dest: TPoolTask); override;
  public
    constructor Create; virtual;
    destructor Destroy; override;
    
    property TaskID: Integer read FTaskID write FTaskID;
    property ShowPanel: TStatusPanel read FPanel write FPanel;
    property FileName: string read FFileName write FFileName;

    function CompareTask(Task: TPoolTask): Boolean; override;
  end;

  TSearchThread = class(TPoolThread)
  private
    FFileName: string;
    FMemoryStream: TMemoryStreamEx;

    procedure RefreshLabel;
    procedure DoTaskComplete;

    procedure DoSearch(Task: TSearchTask);
  protected
    { 线程不停的执行 }
    procedure DoExecuteLoop; override;
  public
    constructor Create(AOwner: TPoolManager; CreateSuspended: Boolean); override;
    destructor Destroy; override;
  end;

  TTaskCompleteEvent = procedure(Task: TPoolTask) of object;
  TSearchManager = class(TPoolManager)
  private
    FOnTaskComplete: TTaskCompleteEvent;

    function GetActionChecked(nAction: LongWord): Boolean;
  protected
    function GetPoolThreadClass: TPoolThreadClass; override;
    procedure DoTaskComplete(Task: TPoolTask); virtual;
  public
    SearchActions: array[Byte] of Boolean;

    SearchWhere: Integer;
    SearchObjName: string;
    SearchActObjName: string;
    SearchActObjType: Integer;
    SearchItemName: string;
    SearchItemID: Integer;

    SearchDataList: TThreadList;

    procedure AddTask(Task: TPoolTask); override;
    property OnTaskComplete: TTaskCompleteEvent read FOnTaskComplete write FOnTaskComplete;
  end;


implementation

uses
  RTLConsts;

{ TCustomMemoryStream }

procedure TCustomMemoryStreamEx.SetPointer(Ptr: Pointer; Size: Longint);
begin
  FMemory := Ptr;
  FSize := Size;
end;

function TCustomMemoryStreamEx.Read(var Buffer; Count: Longint): Longint;
begin
  if (FPosition >= 0) and (Count >= 0) then
  begin
    Result := FSize - FPosition;
    if Result > 0 then
    begin
      if Result > Count then Result := Count;
      Move(Pointer(Longint(FMemory) + FPosition)^, Buffer, Result);
      Inc(FPosition, Result);
      Exit;
    end;
  end;
  Result := 0;
end;

function TCustomMemoryStreamEx.Seek(Offset: Longint; Origin: Word): Longint;
begin
  case Origin of
    soFromBeginning: FPosition := Offset;
    soFromCurrent: Inc(FPosition, Offset);
    soFromEnd: FPosition := FSize + Offset;
  end;
  Result := FPosition;
end;

procedure TCustomMemoryStreamEx.SaveToStream(Stream: TStream);
begin
  if FSize <> 0 then Stream.WriteBuffer(FMemory^, FSize);
end;

procedure TCustomMemoryStreamEx.SaveToFile(const FileName: string);
var
  Stream: TStream;
begin
  Stream := TFileStream.Create(FileName, fmCreate);
  try
    SaveToStream(Stream);
  finally
    Stream.Free;
  end;
end;


{ TMemoryStream }

const
  MemoryDelta = $2000; { Must be a power of 2 }

destructor TMemoryStreamEx.Destroy;
begin
  Clear;
  inherited Destroy;
end;

procedure TMemoryStreamEx.Clear;
begin
  SetCapacity(0);
  FSize := 0;
  FPosition := 0;
end;

procedure TMemoryStreamEx.LoadFromStream(Stream: TStream);
var
  Count: Longint;
begin
  Stream.Position := 0;
  Count := Stream.Size;
  SetSize(Count);
  if Count <> 0 then Stream.ReadBuffer(FMemory^, Count);
end;

procedure TMemoryStreamEx.LoadFromFile(const FileName: string);
var
  Stream: TStream;
begin
  Stream := TFileStream.Create(FileName, fmOpenRead or fmShareDenyRead);
  try
    LoadFromStream(Stream);
  finally
    Stream.Free;
  end;
end;

procedure TMemoryStreamEx.SetCapacity(NewCapacity: Longint);
begin
  SetPointer(Realloc(NewCapacity), FSize);
  FCapacity := NewCapacity;
end;

procedure TMemoryStreamEx.SetSize(NewSize: Longint);
var
  OldPosition: Longint;
begin
  OldPosition := FPosition;
  SetCapacity(NewSize);
  FSize := NewSize;
  if OldPosition > NewSize then Seek(0, soFromEnd);
end;

function TMemoryStreamEx.Realloc(var NewCapacity: Longint): Pointer;
begin
  if (NewCapacity > 0) and (NewCapacity <> FSize) then
    NewCapacity := (NewCapacity + (MemoryDelta - 1)) and not (MemoryDelta - 1);
  Result := Memory;
  if NewCapacity <> FCapacity then
  begin
    if NewCapacity = 0 then
    begin
{$IFDEF MSWINDOWS}
      GlobalFreePtr(Memory);
{$ELSE}
      FreeMem(Memory);
{$ENDIF}
      Result := nil;
    end else
    begin
{$IFDEF MSWINDOWS}
      if Capacity = 0 then
        Result := GlobalAllocPtr(HeapAllocFlags, NewCapacity)
      else
        Result := GlobalReallocPtr(Memory, NewCapacity, HeapAllocFlags);
{$ELSE}
      if Capacity = 0 then
        GetMem(Result, NewCapacity)
      else
        ReallocMem(Result, NewCapacity);
{$ENDIF}
      if Result = nil then raise EStreamError.CreateRes(@SMemoryStreamError);
    end;
  end;
end;

function TMemoryStreamEx.Write(const Buffer; Count: Longint): Longint;
var
  Pos: Longint;
begin
  if (FPosition >= 0) and (Count >= 0) then
  begin
    Pos := FPosition + Count;
    if Pos > 0 then
    begin
      if Pos > FSize then
      begin
        if Pos > FCapacity then
          SetCapacity(Pos);
        FSize := Pos;
      end;
      System.Move(Buffer, Pointer(Longint(FMemory) + FPosition)^, Count);
      FPosition := Pos;
      Result := Count;
      Exit;
    end;
  end;
  Result := 0;
end;

{ TFilePoolTask }

constructor TSearchTask.Create;
begin

end;

destructor TSearchTask.Destroy;
begin

  inherited;
end;

procedure TSearchTask.AssignTo(Dest: TPoolTask);
begin
  if Dest is TSearchTask then
    with TSearchTask(Dest) do
    begin
      FPanel := Self. FPanel;
    end
  else
    inherited;
end;

function TSearchTask.CompareTask(Task: TPoolTask): Boolean;
//var
  //CompTask: TSearchTask;
begin
  Result := False;
end;

{ TSearchThread }

constructor TSearchThread.Create(AOwner: TPoolManager;
  CreateSuspended: Boolean);
begin
  inherited;
  FMemoryStream := TMemoryStreamEx.Create;
end;

destructor TSearchThread.Destroy;
begin
  FMemoryStream.Free;
  inherited;
end;

procedure TSearchThread.DoTaskComplete;
begin
  (Owner as TSearchManager).DoTaskComplete(FContextTask);
end;

procedure TSearchThread.DoExecuteLoop;
var
  Tasks: TList;
  Task: TSearchTask;
begin
  if Assigned(FContextTask) then
    FreeAndNil(FContextTask);

  Tasks := Owner.LockTaskList;
  try
    if Tasks.Count = 0 then Exit;
    FContextTask := Tasks[0];
    Tasks.Delete(0);
  finally
    Owner.UnlockTaskList;
  end;

  if FContextTask = nil then Exit;

  if FContextTask is TSearchTask then
  begin
    Task := FContextTask as TSearchTask;

    FFileName := Task.FFileName;
    Synchronize(RefreshLabel);

    DoSearch(Task);

    Synchronize(DoTaskComplete);

    TriggerEvent;
  end;
end;

procedure TSearchThread.RefreshLabel;
var
  Task: TSearchTask;
begin
  Task := FContextTask as TSearchTask;
  Task.FPanel.Text := FFileName;
end;

function GetSearch(nWhere: Integer; sObjName, sActObjName, sItemName: string; nActObjType: Integer; nItemID: Integer; LogData: pTLogData): Boolean;
//var
  //I: Integer;
begin
  Result := True;
  if nWhere <= 0 then Exit;

  if nWhere and 1 = 1 then
    Result := AnsiContainsText(LogData.sObjectName, sObjName);
  if not Result then Exit;

  if nWhere and 2 = 2 then
    Result := Integer(LogData.ObjectType) = nActObjType;
  if not Result then Exit;
  
  if nWhere and 4 = 4 then
    Result := AnsiContainsText(LogData.sActObjectName, sActObjName);
  if not Result then Exit;

  if nWhere and 8 = 8 then
    Result := AnsiContainsText(LogData.sItemName, sItemName);
  if not Result then Exit;

  if nWhere and 16 = 16 then
    Result := LogData.nItemIndex = nItemID;
end;

procedure TSearchThread.DoSearch(Task: TSearchTask);
var
  nIndex: Integer;
  nLen: Integer;
  nServerNumber: Integer;
  nServerIndex: Integer;
  nAction: LongWord;
  sMapName: string;
  nX, nY: Integer;
  sObjectName: string;
  sItemName: string;
  nItemIndex: Integer;
  sActObjectName: string;
  nData1, nData2: Integer;
  sLogDesc: string;
  Dt: TDateTime;
  btActorType: Byte;

  LogData: TLogData;
  PLogData: PTLogData;
  SM: TSearchManager;
  List: TList;
begin
  try
    FMemoryStream.LoadFromFile(Task.FFileName);
    nIndex := 0;

    FMemoryStream.Seek(0, soFromBeginning);
    while FMemoryStream.Position < FMemoryStream.Size do
    begin
      if FMemoryStream.Read(nServerNumber, SizeOf(nServerNumber)) <> SizeOf(nServerNumber) then Break;

      if FMemoryStream.Read(nServerIndex, SizeOf(nServerIndex)) <> SizeOf(nServerIndex) then Break;

      if FMemoryStream.Read(nAction, SizeOf(nAction)) <> SizeOf(nAction) then Break;

      sMapName := '';
      if FMemoryStream.Read(nLen, SizeOf(nLen)) <> SizeOf(nLen) then Break;
      if nLen > 0 then
      begin
        SetLength(sMapName, nLen);

        if FMemoryStream.Read(sMapName[1], nLen) <> nLen then Break;
      end;

      if FMemoryStream.Read(nX, SizeOf(nX)) <> SizeOf(nX) then Break;

      if FMemoryStream.Read(nY, SizeOf(nY)) <> SizeOf(nY) then Break;

      sObjectName := '';
      if FMemoryStream.Read(nLen, SizeOf(nLen)) <> SizeOf(nLen) then Break;
      if nLen > 0 then
      begin
        SetLength(sObjectName, nLen);
        if FMemoryStream.Read(sObjectName[1], nLen) <> nLen then Break;
      end;

      if FMemoryStream.Read(btActorType, SizeOf(btActorType)) <> SizeOf(btActorType) then Break;
      if (btActorType < Integer(Low(TLogActorType))) or (btActorType > Integer(High(TLogActorType))) then
      begin
        btActorType := Integer(latNone);
      end;

      sItemName := '';
      if FMemoryStream.Read(nLen, SizeOf(nLen)) <> SizeOf(nLen) then Break;
      if nLen > 0 then
      begin
        SetLength(sItemName, nLen);
        if FMemoryStream.Read(sItemName[1], nLen) <> nLen then Break;
      end;

      if FMemoryStream.Read(nItemIndex, SizeOf(nItemIndex)) <> SizeOf(nItemIndex) then Break;

      sActObjectName := '';
      if FMemoryStream.Read(nLen, SizeOf(nLen)) <> SizeOf(nLen) then Break;
      if nLen > 0 then
      begin
        SetLength(sActObjectName, nLen);
        if FMemoryStream.Read(sActObjectName[1], nLen) <> nLen then Break;
      end;

      if FMemoryStream.Read(nData1, SizeOf(nData1)) <> SizeOf(nData1) then Break;
      if FMemoryStream.Read(nData2, SizeOf(nData2)) <> SizeOf(nData2) then Break;

      sLogDesc := '';
      if FMemoryStream.Read(nLen, SizeOf(nLen)) <> SizeOf(nLen) then Break;
      if nLen > 0 then
      begin
        SetLength(sLogDesc, nLen);
        if FMemoryStream.Read(sLogDesc[1], nLen) <> nLen then Break;
      end;

      if FMemoryStream.Read(Dt, SizeOf(Dt)) <> SizeOf(Dt) then Break;

      Inc(nIndex);

      if Task.Canceled then Break;

      LogData.nAct := nAction;
      LogData.sMapName := sMapName;
      LogData.nX := nX;
      LogData.nY := nY;
      LogData.sObjectName := sObjectName;
      LogData.ObjectType := TLogActorType(btActorType);
      LogData.sItemName := sItemName;
      LogData.nItemIndex := nItemIndex;
      LogData.sActObjectName := sActObjectName;
      LogData.nData1 := nData1;
      LogData.nData2 := nData2;
      LogData.LogDesc := sLogDesc;
      LogData.Date := Dt;

      SM := TSearchManager(Owner);

      if SM.GetActionChecked(nAction) and
        GetSearch(SM.SearchWhere, SM.SearchObjName, SM.SearchActObjName, SM.SearchItemName, SM.SearchActObjType, SM.SearchItemID, @LogData) then
      begin
        LogData.nIndx := Task.FTaskID * 1000000 + nIndex;

        New(PLogData);
        PLogData^ := LogData;

        List := SM.SearchDataList.LockList;
        try
          List.Add(PLogData)
        finally
          SM.SearchDataList.UnlockList;
        end;
      end;
    end;
  except
  end;
end;

{ TImagePoolManager }

procedure TSearchManager.AddTask(Task: TPoolTask);
begin
  if not (Task is TSearchTask) then Exit;
  inherited;
end;

procedure TSearchManager.DoTaskComplete(Task: TPoolTask);
begin
  if Assigned(FOnTaskComplete) then
    FOnTaskComplete(Task);
end;

function TSearchManager.GetActionChecked(nAction: LongWord): Boolean;
var
  W1: Word;
  B1, B2: Byte;
begin
  // 日志类型暂时只用了Word
  W1 := LoWord(nAction);
  B1 := LoByte(W1);
  B2 := HiByte(W1);

  Result := SearchActions[B1] or SearchActions[B2];
end;

function TSearchManager.GetPoolThreadClass: TPoolThreadClass;
begin
  Result := TSearchThread;
end;


end.
