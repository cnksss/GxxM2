unit StreamEx;

interface
uses Classes, Dialogs;
type
  TFileStreamEx = class(TFileStream)
  public
    function InsertWrite(const Offset: Int64; const Buffer; Count: Longint): Longint;
    function Delete(const Offset: Int64; Count: Longint): Boolean;
  end;
  TMemoryStreamEx = class(TMemoryStream)
  public
    function InsertWrite(const Offset: Int64; const Buffer; Count: Longint): Longint;
    function Delete(const Offset: Int64; Count: Longint): Boolean;
  end;
implementation
uses Math;

function TMemoryStreamEx.Delete(const Offset: Int64; Count: Longint): Boolean;
var
  nOldSize, nSize, nDelSize, nOffset: Int64;
  MemoryStream: TMemoryStream;
begin
  Result := False;
  if (Offset >= 0) and (Offset < Size) and (Count > 0) then
  begin
    nOldSize := Size;

    nOffset := Min(Offset + Count, nOldSize);
    nDelSize := nOffset - Offset;
    nSize := nOldSize - nOffset;
    if nSize > 0 then
    begin
      Seek(nOffset, soBeginning);
      MemoryStream := TMemoryStream.Create;
      MemoryStream.CopyFrom(Self, nSize);
      MemoryStream.Position := 0;
      Seek(Offset, soBeginning);
      CopyFrom(MemoryStream, 0);
      MemoryStream.Free;
    end;
    Size := Size - nDelSize;
    Result := True;
  end;
end;

function TMemoryStreamEx.InsertWrite(const Offset: Int64; const Buffer; Count: Longint): Longint;
var
  nOldSize, nSize: Int64;
  MemoryStream: TMemoryStream;
begin
  if (Offset > 0) and (Count > 0) then
  begin
    nOldSize := Size;
    nSize := Max(nOldSize - Offset, 0);
    if nSize > 0 then
    begin
      Size := nOldSize + Count;
      Seek(Offset, soBeginning);

      MemoryStream := TMemoryStream.Create;
      MemoryStream.CopyFrom(Self, nSize);
      MemoryStream.Position := 0;
      Seek(Offset + Count, soBeginning);
      CopyFrom(MemoryStream, 0);
      MemoryStream.Free;
    end;

    Seek(Offset, soBeginning);
    Result := Self.Write(Buffer, Count);
  end
  else
    Result := Write(Buffer, Count);
end;

function TFileStreamEx.Delete(const Offset: Int64; Count: Longint): Boolean;
var
  nOldSize, nSize, nDelSize, nOffset: Int64;
  MemoryStream: TMemoryStream;
begin
  Result := False;
  if (Offset >= 0) and (Offset < Size) and (Count > 0) then
  begin
    nOldSize := Size;

    nOffset := Min(Offset + Count, nOldSize);
    nDelSize := nOffset - Offset;
    nSize := nOldSize - nOffset;
    if nSize > 0 then
    begin
      Seek(nOffset, soBeginning);
      MemoryStream := TMemoryStream.Create;
      MemoryStream.CopyFrom(Self, nSize);
      MemoryStream.Position := 0;
      Seek(Offset, soBeginning);
      CopyFrom(MemoryStream, 0);
      MemoryStream.Free;
    end;
    Size := Size - nDelSize;
    Result := True;
  end;
end;

function TFileStreamEx.InsertWrite(const Offset: Int64; const Buffer; Count: Longint): Longint;
var
  nOldSize, nSize: Int64;
  MemoryStream: TMemoryStream;
begin
  if (Offset > 0) and (Count > 0) then
  begin
    nOldSize := Size;
    nSize := Max(nOldSize - Offset, 0);
    if nSize > 0 then
    begin
      Size := nOldSize + Count;
      Seek(Offset, soBeginning);

      MemoryStream := TMemoryStream.Create;
      MemoryStream.CopyFrom(Self, nSize);
      MemoryStream.Position := 0;
      Seek(Offset + Count, soBeginning);
      CopyFrom(MemoryStream, 0);
      MemoryStream.Free;
    end;

    Seek(Offset, soBeginning);
    Result := Self.Write(Buffer, Count);
  end
  else
    Result := Write(Buffer, Count);
end;

end.
