unit M2Locker;

interface

uses
  Windows, Classes, SysUtils;

{$DEFINE USE_SPINLOCK}

type
  TSafeList = class(TList)
  private
  {$IFDEF USE_SPINLOCK}
    FName: string;
    FIsLock: Boolean;
    FLocker: Integer;
    FLockerID: Integer;
  {$ELSE}
    FCS: TRTLCriticalSection;          
  {$ENDIF}
  public
    constructor Create({$IFDEF USE_SPINLOCK}AName: string{$ENDIF});
    destructor Destroy; override;
    //function TryLock: Boolean;
    procedure LockR(LockID: Integer);
    procedure UnLockR;
    procedure LockW(LockID: Integer);
    procedure UnLockW;
  end;

  TSafeStringList = class(TStringList)
  private
  {$IFDEF USE_SPINLOCK}
    FName: string;
    FIsLock: Boolean;
    FLocker: Integer;
    FLockerID: Integer;
  {$ELSE}
    FCS: TRTLCriticalSection;          
  {$ENDIF}
  public
    constructor Create({$IFDEF USE_SPINLOCK}AName: string{$ENDIF});
    destructor Destroy; override;
    //function TryLock: Boolean;
    procedure LockR(LockID: Integer);
    procedure UnLockR;
    procedure LockW(LockID: Integer);
    procedure UnLockW;
  end;

  TM2CriticalSection = class(TObject)
  private
  {$IFDEF USE_SPINLOCK}
    FName: string;
    FIsLock: Boolean;
    FLockerID: Integer;
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
    //function TryLock: Boolean;
    procedure LockR(LockID: Integer);
    procedure UnLockR;
    procedure LockW(LockID: Integer);
    procedure UnLockW;
  end;

{$IFDEF USE_SPINLOCK}
  function SpinLock(var Target: Integer; const LockName: string): Boolean;
  function SpinUnLock(var Target: Integer; const LockName: string): Boolean;

  function BeginRead(var Target: Integer): Boolean;
  procedure EndRead(var Target: Integer);

  function BeginWrite(var Target: Integer): Boolean;
  procedure EndWrite(var Target: Integer);
{$ENDIF}

implementation

uses
  M2Share;


{$IFNDEF CPUX64}
function InterlockedCompareExchange(var Destination: Integer; Exchange: Integer; Comperand: Integer): Integer stdcall;external kernel32 name 'InterlockedCompareExchange';
{$ENDIF}

{ TSafeList }

constructor TSafeList.Create({$IFDEF USE_SPINLOCK}AName: string{$ENDIF});
begin
  inherited Create;
{$IFDEF USE_SPINLOCK}
  FName := AName;
  FIsLock := False;
  FLocker := 0;
  FLockerID := 0
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

procedure TSafeList.LockR(LockID: Integer);
begin
{$IFDEF USE_SPINLOCK}
  FIsLock := True;
  if BeginRead(FLocker) then
  begin
    FLockerID := LockID;
  end
  else
  begin
    //MainOutMessage('发现死锁:' + FName + '; ' + IntToStr(FLockerID) + ', ' + IntToStr(LockID));
  end;
{$ELSE}
  EnterCriticalSection(FCS);
{$ENDIF}
end;

procedure TSafeList.UnLockR;
begin
{$IFDEF USE_SPINLOCK}
  EndRead(FLocker);
  FIsLock := False;
{$ELSE}
  LeaveCriticalSection(FCS);
{$ENDIF}
end;

procedure TSafeList.LockW(LockID: Integer);
begin
{$IFDEF USE_SPINLOCK}
  FIsLock := True;
  if BeginWrite(FLocker) then begin
    FLockerID := LockID;
  end else begin
    MainOutMessage('发现死锁:' + FName + '; ' + IntToStr(FLockerID) + ', ' + IntToStr(LockID));
  end;
{$ELSE}
  EnterCriticalSection(FCS);
{$ENDIF}
end;

procedure TSafeList.UnLockW;
begin
{$IFDEF USE_SPINLOCK}
  EndWrite(FLocker);
  FLockerID := 0;
  FIsLock := False;
{$ELSE}
  LeaveCriticalSection(FCS);
{$ENDIF}
end;

{ TSafeStringList }

constructor TSafeStringList.Create({$IFDEF USE_SPINLOCK}AName: string{$ENDIF});
begin
  inherited Create;
{$IFDEF USE_SPINLOCK}
  FName := AName;
  FIsLock := False;
  FLocker := 0;
  FLockerID := 0;
{$ELSE}
  InitializeCriticalSection(FCS);
{$ENDIF}
end;

destructor TSafeStringList.Destroy;
begin
{$IFDEF USE_SPINLOCK}
  FIsLock := False;
  FLocker := 0;
{$ELSE}
  DeleteCriticalSection(FCS);
{$ENDIF}
  inherited;
end;

(*
function TSafeStringList.TryLock: Boolean;
begin
{$IFDEF USE_SPINLOCK}
  Result := not FIsLock;
  if Result then Lock;
{$ELSE}
  Result := TryEnterCriticalSection(FCS);
{$ENDIF}
end;
*)

procedure TSafeStringList.LockR(LockID: Integer);
begin
{$IFDEF USE_SPINLOCK}
  FIsLock := True;
  if BeginRead(FLocker) then
  begin
    FLockerID := LockID;
  end
  else
  begin
    MainOutMessage('发现死锁:' + FName + '; ' + IntToStr(FLockerID) + ', ' + IntToStr(LockID));
  end;
{$ELSE}
  EnterCriticalSection(FCS);
{$ENDIF}
end;

procedure TSafeStringList.UnLockR;
begin
{$IFDEF USE_SPINLOCK}
  EndRead(FLocker);
  FLockerID := 0;
  FIsLock := False;
{$ELSE}
  LeaveCriticalSection(FCS);
{$ENDIF}
end;

procedure TSafeStringList.LockW(LockID: Integer);
begin
{$IFDEF USE_SPINLOCK}
  FIsLock := True;
  if BeginWrite(FLocker) then
  begin
    FLockerID := LockID;
  end
  else
  begin
    MainOutMessage('发现死锁:' + FName + '; ' + IntToStr(FLockerID) + ', ' + IntToStr(LockID));
  end;
{$ELSE}
  EnterCriticalSection(FCS);
{$ENDIF}
end;

procedure TSafeStringList.UnLockW;
begin
{$IFDEF USE_SPINLOCK}
  EndWrite(FLocker);
  FLockerID := 0;
  FIsLock := False;
{$ELSE}
  LeaveCriticalSection(FCS);
{$ENDIF}
end;

{$IFDEF USE_SPINLOCK}
function SpinLock(var Target: Integer; const LockName: string): Boolean;
var
  StartTick: LongWord;
begin
  Result := True;
  StartTick := MyGetTickCount;
  while InterlockedCompareExchange(Target, 1, 0) <> 0 do
  begin
    Sleep(0);
    if MyGetTickCount - StartTick >= 3000 then
    begin
      //MainOutMessage('发现死锁:' + LockName);
      Result := False;
      Break;
    end;
  end;
end;

function SpinUnLock(var Target: Integer; const LockName: string): Boolean;
var
  StartTick: LongWord;
begin
  Result := True;
  StartTick := MyGetTickCount;
  while InterlockedCompareExchange(Target, 0, 1) <> 1 do
  begin
    Sleep(0);
    if MyGetTickCount - StartTick >= 3000 then
    begin
      //MainOutMessage('发现死锁:' + LockName);
      Result := False;
      Break;
    end;
  end;
end;

function BeginRead(var Target: Integer): Boolean;
var
  CurrentReference: Integer;
  StartTick: LongWord;
begin
  Result := True;
  StartTick := MyGetTickCount;

  // 等待写入器复位写入标志，因此Target.Bit0必须为0
  repeat
    CurrentReference := Target and $FFFFFFFC;

    if MyGetTickCount - StartTick >= 1000 then
    begin
      Result := False;
      Break;
    end;

    Sleep(0);
  until InterlockedCompareExchange(Target, CurrentReference + 2, CurrentReference) = CurrentReference;
end;

procedure EndRead(var Target: Integer);
begin
  InterlockedExchangeAdd(Target, -2);
end;

function BeginWrite(var Target: Integer): Boolean;
var
  CurrentReference: Integer;
  StartTick: LongWord;
begin
  Result := True;
  StartTick := MyGetTickCount;

  // 等待写入器复位写入标志，因此Target.Bit0必须为0，然后设置Target.Bit0
  repeat
    CurrentReference := Target and $FFFFFFFC;

    if MyGetTickCount - StartTick >= 1000 then
    begin
      Result := False;
      Break;
    end;

    Sleep(0);
  until InterlockedCompareExchange(Target, CurrentReference + 1, CurrentReference) = CurrentReference;

  StartTick := MyGetTickCount;

  if not Result then
  begin
    // 等待所有读取
    repeat
      if MyGetTickCount - StartTick >= 1000 then
      begin
        Result := False;
        Break;
      end;

      Sleep(0);
    until Target = 1;
  end;
end;

procedure EndWrite(var Target: Integer);
begin
  Target := 0;
end;

{$ENDIF}

{ TIocpCriticalSection }

constructor TM2CriticalSection.Create({$IFDEF USE_SPINLOCK}AName: string{$ENDIF});
begin
  inherited Create;
{$IFDEF USE_SPINLOCK}
  FName := AName;
  FIsLock := False;
  FSection := 0;
  FLockerID := 0;
{$ELSE}
  InitializeCriticalSection(FSection);
{$ENDIF}
end;

destructor TM2CriticalSection.Destroy;
begin
{$IFDEF USE_SPINLOCK}
  FIsLock := False;
  FSection := 0;
{$ELSE}
  DeleteCriticalSection(FSection);
{$ENDIF}
  inherited Destroy;
end;

(*
function TM2CriticalSection.TryLock: Boolean;
begin
{$IFDEF USE_SPINLOCK}
  Result := not FIsLock;
  if Result then Lock;
{$ELSE}
  Result := TryEnterCriticalSection(FSection);
{$ENDIF}
end;
*)

procedure TM2CriticalSection.LockR(LockID: Integer);
begin
{$IFDEF USE_SPINLOCK}
  FIsLock := True;
  if BeginRead(FSection) then
  begin
    FLockerID := LockID;
  end
  else
  begin
    MainOutMessage('发现死锁:' + FName + '; ' + IntToStr(FLockerID) + ', ' + IntToStr(LockID));
  end;
{$ELSE}
  EnterCriticalSection(FSection);
{$ENDIF}
end;

procedure TM2CriticalSection.UnLockR;
begin
{$IFDEF USE_SPINLOCK}
  EndRead(FSection);
  FLockerID := 0;
  FIsLock := False;
{$ELSE}
  LeaveCriticalSection(FSection);
{$ENDIF}
end;

procedure TM2CriticalSection.LockW(LockID: Integer);
begin
{$IFDEF USE_SPINLOCK}
  FIsLock := True;
  if BeginWrite(FSection) then
  begin
    FLockerID := LockID;
  end
  else
  begin
    MainOutMessage('发现死锁:' + FName + '; ' + IntToStr(FLockerID) + ', ' + IntToStr(LockID));
  end;
{$ELSE}
  EnterCriticalSection(FSection);
{$ENDIF}
end;

procedure TM2CriticalSection.UnLockW;
begin
{$IFDEF USE_SPINLOCK}
  EndWrite(FSection);
  FLockerID := 0;
  FIsLock := False;
{$ELSE}
  LeaveCriticalSection(FSection);
{$ENDIF}
end;

end.
