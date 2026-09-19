{-----------------------------------------------------------------------------}

{ support delphi XE chongchong 2020-12-15}
{-----------------------------------------------------------------------------}

unit CheckUnit;

interface
uses
  Windows,
  SysUtils,
  Classes,
  CheckCrc;

function FileCrc(const FileName: string): Cardinal;
function BufferCrc(Buffer: PAnsiChar; nSize: Integer): Cardinal;
function StringCrc(const Value: AnsiString): Cardinal;
function CalcFileCRC(const sFileName: string): Integer;
function HashPJW(const Value: AnsiString): Longint;

implementation
//获得CRC校验值

function BufferCRC(Buffer: PAnsiChar; nSize: Integer): Cardinal;
begin
  Result := CheckCrc.Crc32(PByte(Buffer), nSize);
end;

function StringCrc(const Value: AnsiString): Cardinal;
begin
  Result := 0;
  if Value <> '' then
    Result := CheckCrc.Crc32(@Value[1], Length(Value));
end;

function FileCrc(const FileName: string): Cardinal;
var
  MemoryStream: TMemoryStream;
begin
  if FileExists(FileName) then
  begin
    MemoryStream := TMemoryStream.Create;
    try
      MemoryStream.LoadFromFile(FileName);
    except
      MemoryStream.Free;
      Result := 0;
      Exit;
    end;
    Result := CheckCrc.Crc32(PByte(MemoryStream.Memory), MemoryStream.Size);
    MemoryStream.Free;
  end
  else
    Result := 0;
end;

function HashPJW(const Value: AnsiString): Longint;
var
  I: Integer;
  G: Longint;
begin
  Result := 0;
  for I := 1 to Length(Value) do
  begin
    Result := (Result shl 4) + Ord(Value[I]);
    G := Result and $F0000000;
    if G <> 0 then
      Result := (Result xor (G shr 24)) xor G;
  end;
end;

function CalcFileCRC(const sFileName: string): Integer;
var
  I: Integer;
  nFileHandle: Integer;
  nFileSize, nBuffSize: Integer;
  Buffer: PChar;
  Int: ^Integer;
  nCrc: Integer;
begin
  Result := 0;
  if not FileExists(sFileName) then Exit;
  nFileHandle := FileOpen(sFileName, fmOpenRead or fmShareDenyNone);
  if nFileHandle = 0 then
    Exit;
  nFileSize := FileSeek(nFileHandle, 0, 2);
  nBuffSize := (nFileSize div 4) * 4;
  GetMem(Buffer, nBuffSize);
  FillChar(Buffer^, nBuffSize, 0);
  FileSeek(nFileHandle, 0, 0);
  FileRead(nFileHandle, Buffer^, nBuffSize);
  FileClose(nFileHandle);
  Int := Pointer(Buffer);
  nCrc := 0;
  { TODO -ochongchong -c内存泄露 : 去内存泄露【2013-07-17】 }
  //Exception.Create(IntToStr(SizeOf(Integer)));
  for I := 0 to nBuffSize div 4 - 1 do
  begin
    nCrc := nCrc xor Int^;
    Int := Pointer(NativeInt(Int) + 4);
  end;
  FreeMem(Buffer);
  Result := nCrc;
end;

function CalcBufferCRC(Buffer: PAnsiChar; nSize: Integer): Integer;
var
  I: Integer;
  Int: ^Integer;
  nCrc: Integer;
begin
  Int := Pointer(Buffer);
  nCrc := 0;
  for I := 0 to nSize div 4 - 1 do
  begin
    nCrc := nCrc xor Int^;
    Int := Pointer(NativeInt(Int) + 4);
  end;
  Result := nCrc;
end;

function CalcStreamCRC(M: TCustomMemoryStream): Integer;
var
  I: Integer;
  Int: ^Integer;
  nCrc: Integer;
  nSize: Integer;
begin
  nSize := M.Size;
  Int := M.Memory;
  nCrc := 0;
  for I := 0 to nSize div 4 - 1 do
  begin
    nCrc := nCrc xor Int^;
    Int := Pointer(NativeInt(Int) + 4);
  end;
  Result := nCrc;
end;

end.
