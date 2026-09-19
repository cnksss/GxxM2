unit IODataPool;

interface

{$I iocp.inc}

uses
  Windows, Classes, SysUtils, SyncObjs, IOCPCommon;

type
  TIODataPool = class
  private
    FAllocMemSize: Integer;
    FMaxAllocMemSize: Integer;

    FAllocCount: Integer;
    FUseCount: Integer;
    FMaxUseCount: Integer;

    FIODataLists: array of TList;
    FNoUseIODataLists: array of TList;

{$IF UseIocpClient <> 0}
    FIODataLists_2: TList;
    FNoUseIODataLists_2: TList;
{$IFEND}

    //FSendBufferCount: Integer;
    //FSendBufferList: TList;
    //FNoUseSendBufferList: TList;

    FCS: TIocpCriticalSection;
    function GetCount: Integer;
    function GetUseCount: Integer;
    function GetNoUseCount: Integer;
  public
  {$IFDEF SHARE_POOL_MODE}
    class function Instance: TIODataPool;
  {$ENDIF}
    procedure Clear;
    constructor Create;
    destructor Destroy; override;

    function GetNewIOData(MaxSize: Integer): POVERLAPPEDEx;
    function GiveBackIOData(const IOData: POVERLAPPEDEx): Boolean;

    //function GetNewSendBufer(BufSize: Integer): PSendBuffer;
    //function GiveBackSendBuffer(const SendBuffer: PSendBuffer): Boolean;

    property Count: Integer read GetCount;
    property UseCount: Integer read GetUseCount;
    property NoUseCount: Integer read GetNoUseCount;

    property AllocMemSize: Integer read FAllocMemSize;
    property MaxAllocMemSize: Integer read FMaxAllocMemSize;
    property MaxUseCount: Integer read FMaxUseCount;
  end;

implementation

{ TIODataPool }

{$IFDEF SHARE_POOL_MODE}
var
  _IODataPoolInstance: TIODataPool = nil;
{$ENDIF}

procedure DivMod(Dividend: Integer; Divisor: Word;
  var Result, Remainder: Word);
asm
        PUSH    EBX
        MOV     EBX,EDX
        MOV     EDX,EAX
        SHR     EDX,16
        DIV     BX
        MOV     EBX,Remainder
        MOV     [ECX],AX
        MOV     [EBX],DX
        POP     EBX
end;

constructor TIODataPool.Create;
var
  I, J, Count: Integer;
  IoData: POVERLAPPEDEx;
  //SendBuf: PSendBuffer;
begin
  FCS := TIocpCriticalSection.Create({$IFDEF USE_SPINLOCK}'TIODataPoolLocker'{$ENDIF});

  FAllocMemSize := 0;
  FMaxAllocMemSize := 0;

  FUseCount := 0;
  FMaxUseCount := 0;

  Count := MAX_OVERLAPPEDEX_BUFFER_SIZE shl 10 shr 7 + 1;
  SetLength(FIODataLists, Count);
  SetLength(FNoUseIODataLists, Count);

  for I := Low(FIODataLists) to High(FIODataLists) do
  begin
    FIODataLists[I] := TList.Create;
    FNoUseIODataLists[I] := TList.Create;

    for J := 0 to MAX_PREALLOCATED_MEMORY_SIZE - 1 do
    begin
      IoData := AllocMem(SizeOf(OVERLAPPEDEx));

      IoData.AllocSize := I shl 7;
      if IoData.AllocSize > 0 then
      begin
        IoData.DataBuf.buf := AllocMem(IoData.AllocSize);
      end;

      FIODataLists[I].Add(IoData);
      FNoUseIODataLists[I].Add(IoData);

      FAllocMemSize := FAllocMemSize + SizeOf(OVERLAPPEDEx) + IoData.AllocSize;


      IoData.DataBuf.len := IoData.AllocSize;
      IoData.IoType := itNone;
      FillChar(IoData.Overlapped, SizeOf(IoData.Overlapped), 0);
    end;
  end;

{$IF UseIocpClient <> 0}
  FIODataLists_2 := TList.Create;
  FNoUseIODataLists_2 := TList.Create;

  for J := 0 to 60 - 1 do
  begin
    IoData := AllocMem(SizeOf(OVERLAPPEDEx));

    IoData.AllocSize := MAX_IOCP_CLIENT_RECV_BUFFER_SIZE;
    if IoData.AllocSize > 0 then
    begin
      IoData.DataBuf.buf := AllocMem(IoData.AllocSize);
    end;

    FIODataLists_2.Add(IoData);
    FNoUseIODataLists_2.Add(IoData);

    FAllocMemSize := FAllocMemSize + SizeOf(OVERLAPPEDEx) + IoData.AllocSize;

    IoData.DataBuf.len := IoData.AllocSize;
    IoData.IoType := itNone;
    FillChar(IoData.Overlapped, SizeOf(IoData.Overlapped), 0);
  end;
{$IFEND}
  {
  FSendBufferCount := 60 shl 20 div (MAX_OVERLAPPEDEX_BUFFER_SIZE shl 10 + SizeOf(TSendBuffer));    // 60M
  FSendBufferList := TList.Create;
  FNoUseSendBufferList := TList.Create;
  for I := 0 to FSendBufferCount - 1 do
  begin
    SendBuf := AllocMem(SizeOf(TSendBuffer));
    SendBuf.Position := 0;
    SendBuf.BufSize := 0;
    SendBuf.AllocSize := MAX_OVERLAPPEDEX_BUFFER_SIZE shl 10;
    SendBuf.Buffer := AllocMem(SendBuf.AllocSize);
    FAllocMemSize := FAllocMemSize + SizeOf(TSendBuffer) + SendBuf.AllocSize;

    FSendBufferList.Add(SendBuf);
    FNoUseSendBufferList.Add(SendBuf);
  end;
  }

  FMaxAllocMemSize := FAllocMemSize;

  FAllocCount := Length(FIODataLists) * MAX_PREALLOCATED_MEMORY_SIZE;
end;

destructor TIODataPool.Destroy;
var
  I: Integer;
begin
  Clear;
  for I := Low(FIODataLists) to High(FIODataLists) do
  begin
    FIODataLists[I].Free;
    FNoUseIODataLists[I].Free;
  end;

{$IF UseIocpClient <> 0}
  FNoUseIODataLists_2.Free;
  FIODataLists_2.Free;
{$IFEND}

  {
  FSendBufferList.Free;
  FNoUseSendBufferList.Free;
  }

  FCS.Free;

  inherited;
end;

procedure TIODataPool.Clear;
var
  I, J: Integer;
  IOData: POVERLAPPEDEx;
  //SendBuf: PSendBuffer;
begin
  FCS.Lock;
  try
    for I := Low(FIODataLists) to High(FIODataLists) do
    begin
      FNoUseIODataLists[I].Clear;

      for J := 0 to FIODataLists[I].Count - 1 do
      begin
        IOData := FIODataLists[I].Items[J];

        if IOData.AllocSize > 0 then
          FreeMem(IOData.DataBuf.buf, IOData.AllocSize);

        //GlobalFree(Cardinal(IOData));
        FreeMem(IOData, SizeOf(OVERLAPPEDEx));
      end;

      FIODataLists[I].Clear;
    end;


  {$IF UseIocpClient <> 0}
    FNoUseIODataLists_2.Clear;

    for J := 0 to FIODataLists_2.Count - 1 do
    begin
      IOData := FIODataLists_2.Items[J];

      if IOData.AllocSize > 0 then
        FreeMem(IOData.DataBuf.buf, IOData.AllocSize);

      //GlobalFree(Cardinal(IOData));
      FreeMem(IOData, SizeOf(OVERLAPPEDEx));
    end;

    FIODataLists_2.Clear;
  {$IFEND}
    {
    FNoUseSendBufferList.Clear;
    for I := 0 to FSendBufferList.Count - 1 do
    begin
      SendBuf := FSendBufferList.Items[I];
      if SendBuf.AllocSize > 0 then
        FreeMem(SendBuf.Buffer, SendBuf.AllocSize);
      FreeMem(SendBuf, SizeOf(TSendBuffer));
    end;
    FSendBufferList.Clear;
    }

    FAllocMemSize := 0;
    FMaxAllocMemSize := 0;

    FAllocCount := 0;
    FUseCount := 0;
    FMaxUseCount := 0;
  finally
    FCS.UnLock;
  end;
end;

function TIODataPool.GetNewIOData(MaxSize: Integer): POVERLAPPEDEx;
var
  I: Integer;
  w1, w2: Word;
  IsNeedAllocMem: Boolean;
  IoData: POVERLAPPEDEx;
begin
  FCS.Lock;
  try
    IsNeedAllocMem := True;
{$IF UseIocpClient <> 0}
    if MaxSize = MAX_IOCP_CLIENT_RECV_BUFFER_SIZE then
    begin
      for I := 0 to FNoUseIODataLists_2.Count - 1 do
      begin
        IoData := FNoUseIODataLists_2.Items[I];
        FNoUseIODataLists_2.Delete(I);
        IsNeedAllocMem := False;
        Result := IoData;
        Break;
      end;

      if IsNeedAllocMem then
      begin
        //Result := POVERLAPPEDEx(GlobalAlloc(GPTR, SizeOf(OVERLAPPEDEx)));
        Result := AllocMem(SizeOf(OVERLAPPEDEx));

        Result.AllocSize := MAX_IOCP_CLIENT_RECV_BUFFER_SIZE;
        if Result.AllocSize > 0 then
        begin
          Result.DataBuf.buf := AllocMem(Result.AllocSize);
        end;

        FIODataLists_2.Add(Result);

        FAllocMemSize := FAllocMemSize + SizeOf(OVERLAPPEDEx) + Result.AllocSize;
        if FMaxAllocMemSize < FAllocMemSize then
          FMaxAllocMemSize := FAllocMemSize;

        Inc(FAllocCount);
      end;

      Result.DataBuf.len := Result.AllocSize;
      Result.IoType := itNone;
      FillChar(Result.Overlapped, SizeOf(Result.Overlapped), 0);

      Inc(FUseCount);
      if FUseCount > FMaxUseCount then
        FMaxUseCount := FUseCount;

      Exit;
    end;
{$IFEND}
    DivMod(MaxSize, 1 shl 7, w1, w2);
    if w2 > 0 then Inc(w1);

    if w1 <= High(FNoUseIODataLists) then
    begin
      for I := 0 to FNoUseIODataLists[w1].Count - 1 do
      begin
        IoData := FNoUseIODataLists[w1].Items[I];
        FNoUseIODataLists[w1].Delete(I);
        IsNeedAllocMem := False;
        Result := IoData;
        Break;
      end;

      if IsNeedAllocMem then
      begin
        //Result := POVERLAPPEDEx(GlobalAlloc(GPTR, SizeOf(OVERLAPPEDEx)));
        Result := AllocMem(SizeOf(OVERLAPPEDEx));

        Result.AllocSize := w1 shl 7;
        if Result.AllocSize > 0 then
        begin
          Result.DataBuf.buf := AllocMem(Result.AllocSize);
        end;

        FIODataLists[w1].Add(Result);

        FAllocMemSize := FAllocMemSize + SizeOf(OVERLAPPEDEx) + Result.AllocSize;
        if FMaxAllocMemSize < FAllocMemSize then
          FMaxAllocMemSize := FAllocMemSize;

        Inc(FAllocCount);
      end;

      Result.DataBuf.len := Result.AllocSize;
      Result.IoType := itNone;
      FillChar(Result.Overlapped, SizeOf(Result.Overlapped), 0);

      Inc(FUseCount);
      if FUseCount > FMaxUseCount then
        FMaxUseCount := FUseCount;
    end
    else
      raise Exception.Create('GetNewIOData Size error');
  finally
    FCS.UnLock;
  end;
end;

function TIODataPool.GetCount: Integer;
begin
  Result := FAllocCount;
end;

function TIODataPool.GetUseCount: Integer;
begin
  Result := FUseCount;
end;

function TIODataPool.GetNoUseCount: Integer;
begin
  Result := FAllocCount - FUseCount;
end;

function TIODataPool.GiveBackIOData(const IOData: POVERLAPPEDEx): Boolean;
var
  Index, I: Integer;
  w1, w2: Word;
  TempIoData: POVERLAPPEDEx;
begin
  Result := False;
{$IF UseIocpClient <> 0}
  if IOData.AllocSize = MAX_IOCP_CLIENT_RECV_BUFFER_SIZE then
  begin
    FCS.Lock;
    try
      Dec(FUseCount);

      if FNoUseIODataLists_2.Count >= 60 then
      begin
        FAllocMemSize := FAllocMemSize - SizeOf(OVERLAPPEDEx) - IOData.AllocSize;
        Dec(FAllocCount);

        FIODataLists_2.Remove(IOData);
        FNoUseIODataLists_2.Remove(IOData);

        if IOData.AllocSize > 0 then
          FreeMem(IOData.DataBuf.buf, IOData.AllocSize);

        //GlobalFree(Cardinal(IOData));
        FreeMem(IOData, SizeOf(OVERLAPPEDEx));
      end
      else
      begin
        IOData.DataBuf.len := IOData.AllocSize;
        IOData.IoType := itNone;
        FillChar(IOData.Overlapped, SizeOf(IOData.Overlapped), 0);
        FNoUseIODataLists_2.Add(IOData);
      end;

      Result := True;
      Exit;
    finally
      FCS.UnLock;
    end;
  end;
{$IFEND}

  DivMod(IOData.AllocSize, 1 shl 7, w1, w2);
  if w2 > 0 then Inc(w1);

  if w1 <= High(FNoUseIODataLists) then
  begin
    FCS.Lock;
    try
      if AllocMemSize >= 500 shl 20 then
      begin
        // 随机性的释放内存空间 chongchong 2016-08-24
        Index := Random(High(FNoUseIODataLists));
        for I := FNoUseIODataLists[Index].Count - 1 downto 0 do
        begin
          TempIoData := FNoUseIODataLists[Index].Items[I];

          FAllocMemSize := FAllocMemSize - SizeOf(OVERLAPPEDEx) - TempIoData.AllocSize;
          Dec(FAllocCount);

          FIODataLists[Index].Remove(TempIoData);

          if TempIoData.AllocSize > 0 then
            FreeMem(TempIoData.DataBuf.buf, TempIoData.AllocSize);

          //GlobalFree(Cardinal(TempIoData));
          FreeMem(TempIoData, SizeOf(OVERLAPPEDEx));
        end;

        FNoUseIODataLists[Index].Clear;
      end;

      Dec(FUseCount);

      if FNoUseIODataLists[w1].Count >= MAX_PREALLOCATED_MEMORY_SIZE then
      begin
        FAllocMemSize := FAllocMemSize - SizeOf(OVERLAPPEDEx) - IOData.AllocSize;
        Dec(FAllocCount);

        FIODataLists[w1].Remove(IOData);
        FNoUseIODataLists[w1].Remove(IOData);

        if IOData.AllocSize > 0 then
          FreeMem(IOData.DataBuf.buf, IOData.AllocSize);

        //GlobalFree(Cardinal(IOData));
        FreeMem(IOData, SizeOf(OVERLAPPEDEx));
      end
      else
      begin
        IOData.DataBuf.len := IOData.AllocSize;
        IOData.IoType := itNone;
        FillChar(IOData.Overlapped, SizeOf(IOData.Overlapped), 0);
        FNoUseIODataLists[w1].Add(IOData);
      end;

      Result := True;
    finally
      FCS.UnLock;
    end;
  end;
end;

{
function TIODataPool.GetNewSendBufer(BufSize: Integer): PSendBuffer;
var
  I: Integer;
  IsNeedAllocMem: Boolean;
  SendBuf: PSendBuffer;
begin
  IsNeedAllocMem := True;

  for I := 0 to FNoUseSendBufferList.Count - 1 do
  begin
    SendBuf := FNoUseSendBufferList.Items[I];
    FNoUseSendBufferList.Delete(I);
    IsNeedAllocMem := False;
    Result := SendBuf;
    Break;
  end;

  if IsNeedAllocMem then
  begin
    //Result := POVERLAPPEDEx(GlobalAlloc(GPTR, SizeOf(OVERLAPPEDEx)));
    Result := AllocMem(SizeOf(TSendBuffer));

    Result.AllocSize := MAX_OVERLAPPEDEX_BUFFER_SIZE shl 10;
    if Result.AllocSize > 0 then
    begin
      Result.Buffer := AllocMem(Result.AllocSize);
    end;

    FSendBufferList.Add(Result);

    FAllocMemSize := FAllocMemSize + SizeOf(TSendBuffer) + Result.AllocSize;
    if FMaxAllocMemSize < FAllocMemSize then
      FMaxAllocMemSize := FAllocMemSize;

    Inc(FAllocCount);
  end;

  Result.Position := 0;
  Result.BufSize := BufSize;

  //FillChar(Result.Overlapped, SizeOf(Result.Overlapped), 0);

  Inc(FUseCount);
  if FUseCount > FMaxUseCount then
    FMaxUseCount := FUseCount;
end;

function TIODataPool.GiveBackSendBuffer(const SendBuffer: PSendBuffer): Boolean;
begin
  Dec(FUseCount);

  if FNoUseSendBufferList.Count > FSendBufferCount then
  begin
    FAllocMemSize := FAllocMemSize - SizeOf(TSendBuffer) - SendBuffer.AllocSize;
    Dec(FAllocCount);

    FSendBufferList.Remove(SendBuffer);
    FNoUseSendBufferList.Remove(SendBuffer);

    if SendBuffer.AllocSize > 0 then
      FreeMem(SendBuffer.Buffer, SendBuffer.AllocSize);

    //GlobalFree(Cardinal(IOData));
    FreeMem(SendBuffer, SizeOf(TSendBuffer));
  end
  else
  begin
    SendBuffer.Position := 0;
    SendBuffer.BufSize := 0;
    //FillChar(IOData.Overlapped, SizeOf(IOData.Overlapped), 0);
    FNoUseSendBufferList.Add(SendBuffer);
  end;

  Result := True;
end;
}

{$IFDEF SHARE_POOL_MODE}
class function TIODataPool.Instance: TIODataPool;
begin
  if _IODataPoolInstance = nil then
    _IODataPoolInstance := TIODataPool.Create;

  Result := _IODataPoolInstance;
end;

initialization
  //_IODataPoolInstance := TIODataPool.Create;

finalization
  if _IODataPoolInstance <> nil then
  begin
    _IODataPoolInstance.Free;
    _IODataPoolInstance := nil;
  end;
{$ENDIF}

end.
