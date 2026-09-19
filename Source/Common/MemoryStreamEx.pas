unit MemoryStreamEx;

interface
uses
  SysUtils, Classes;
const
  MAXMEMORYSIZE = 1024 * 1024 * 4;
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
    FLength: Longint;
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
    property Length: Longint read FLength write FLength;
  end;
implementation
resourcestring
  SMemoryStreamError = 'Out of memory while expanding memory streamex';
{ TCustomMemoryStreamEx }

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
      Move(Pointer(NativeInt(FMemory) + FPosition)^, Buffer, Result);
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

const
  MemoryDelta = $2000;                                                                              { Must be a power of 2 }

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
  FLength := 0;
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
  Stream := TFileStream.Create(FileName, fmOpenRead or fmShareDenyWrite);
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

      FreeMem(Memory);

      Result := nil;
    end
    else
    begin
      if Capacity = 0 then
        GetMem(Result, NewCapacity)
      else
        ReallocMem(Result, NewCapacity);
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
      System.Move(Buffer, Pointer(NativeInt(FMemory) + FPosition)^, Count);
      FPosition := Pos;
      Result := Count;
      Exit;
    end;
  end;
  Result := 0;
end;

end.
