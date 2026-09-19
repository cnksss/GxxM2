unit PowerBase64;

//可变码表Base64 (he)

interface

uses
{$IFDEF USE_VMP}
  VMProtectSDK,
{$ENDIF}
  Windows;

function EncodeBase64(pData: Pointer; nDataSize: UINT): AnsiString;

function DecodeBase64(strBase64: AnsiString; pData: Pointer; nDataSize: UINT): UINT;

function EncodeBase64String(strSrc: AnsiString): AnsiString;

function DecodeBase64String(strBase64: AnsiString): AnsiString;

function EncryptAndEncodeBase64(pData: Pointer; nDataSize: UINT): AnsiString;

function DecryptAndiDecodeBase64(strBase64: AnsiString; pData: Pointer; nDataSize: UINT): UINT;

function EncryptAndEncodeBase64String(strSrc: AnsiString): AnsiString;

function DecryptAndDecodeBase64String(strBase64: AnsiString): AnsiString;

implementation

const
  EncodeTable: array[0..64 - 1] of AnsiChar = ('4', '5', '6', '7', '8', '9', '#', '/', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'Q',
    'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'A', 'B',
    'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'w', 'x', 'y', 'z', '0', '1', '2', '3');
  DEF_FILL_CHAR = '+';

var
  DecodeTable: array[0..128 - 1] of Byte;

procedure XOR_encrypt(pData: PByte; dwLen: DWORD);
var
  i: Cardinal;
const
  CIPHER_INDEX: array[0..16 - 1] of BYTE = (4, 12, 9, 15, 13, 8, 11, 5, 14, 6, 0, 7, 2, 10, 1, 3);
  BASE_CIPHER: array[0..16 - 1] of BYTE = ($A5, $5A, $96, $69, $AF, $FA, $5F, $F5, $9F, $F9, $6F, $F6, $AA, $55, $66, $99);
begin
    {$IFDEF USE_VMP}
  VMProtectBeginVirtualization('PowerBase64.XorEncrypt');
    {$ENDIF}

  for i := 0 to dwLen - 1 do
  begin
    pData[i] := pData[i] xor BASE_CIPHER[CIPHER_INDEX[i and $F]];
  end;

    {$IFDEF USE_VMP}
  VMProtectEnd();
    {$ENDIF}
end;

procedure XOR_decrypt(pData: PByte; dwLen: DWORD);
begin
  XOR_encrypt(pData, dwLen);
end;

function EncodeBase64(pData: Pointer; nDataSize: UINT): AnsiString;
var
  nFillLen, nCycle, nRem: UINT;
  i: UINT;
  nData: UINT;
  d0, d1, d2, d3: Byte;
  pStr: PAnsiChar;
begin
  if (nDataSize = 0) then
    exit;
  nRem := nDataSize mod 3;
  if (nRem <> 0) then
    nFillLen := 3 - (nRem)
  else
    nFillLen := 0;
  nCycle := nDataSize div 3;
  if (nFillLen = 0) then
  begin
    SetLength(Result, nCycle * 4);
  end
  else
  begin
    SetLength(Result, (nCycle + 1) * 4 + nFillLen);
  end;
  pStr := PAnsiChar(Result);

  i := 0;
  while i < nCycle do
  begin
    nData := (PUINT(pData))^;
    nData := nData and $00FFFFFF;
    d0 := (nData and $0000003F);
    nData := nData shr 6;
    d1 := (nData and $0000003F);
    nData := nData shr 6;
    d2 := (nData and $0000003F);
    nData := nData shr 6;
    d3 := (nData and $0000003F);

    pStr[0] := EncodeTable[d0];
    pStr[1] := EncodeTable[d1];
    pStr[2] := EncodeTable[d2];
    pStr[3] := EncodeTable[d3];

    pData := Pointer(UintPtr(pData) + 3); //Inc(pData, 3);
    Inc(pStr, 4);
    Inc(i);
  end;

  if (nFillLen = 2) then
  begin
    nData := PUINT(pData)^;
    nData := nData and $000000FF;
    d0 := (nData and $0000003F);
    nData := nData shr 6;
    d1 := (nData and $0000003F);
    nData := nData shr 6;
    d2 := (nData and $0000003F);
    nData := nData shr 6;
    d3 := (nData and $0000003F);
    pStr[0] := EncodeTable[d0];
    pStr[1] := EncodeTable[d1];
    pStr[2] := EncodeTable[d2];
    pStr[3] := EncodeTable[d3];
    pStr[4] := DEF_FILL_CHAR;
    pStr[5] := DEF_FILL_CHAR;
  end
  else if (nFillLen = 1) then
  begin
    nData := PUINT(pData)^;
    nData := nData and $0000FFFF;
    d0 := (nData and $0000003F);
    nData := nData shr 6;
    d1 := (nData and $0000003F);
    nData := nData shr 6;
    d2 := (nData and $0000003F);
    nData := nData shr 6;
    d3 := (nData and $0000003F);
    pStr[0] := EncodeTable[d0];
    pStr[1] := EncodeTable[d1];
    pStr[2] := EncodeTable[d2];
    pStr[3] := EncodeTable[d3];
    pStr[4] := DEF_FILL_CHAR;
  end;

end;

function DecodeBase64(strBase64: AnsiString; pData: Pointer; nDataSize: UINT): UINT;
var
  nStrSize, nFillLen: UINT;
  nBufLen, nCycle: UINT;
  i: UINT;
  pch: PAnsiChar;
  d0, d1, d2, d3: Byte;
  i0, i1, i2, i3: Byte;
  nData: UINT;
  pbData: PByte;
begin
    {$IFDEF USE_VMP}
  VMProtectBeginVirtualization('PowerBase64.DecodeBase64');
    {$ENDIF}

  nStrSize := Length(strBase64);
  if (nStrSize < 4) then
  begin
    Result := 0;
    exit;
  end;

  nFillLen := 0;
  if strBase64[nStrSize - 1] = DEF_FILL_CHAR then
  begin
    nFillLen := 2;
  end
  else if strBase64[nStrSize] = DEF_FILL_CHAR then
  begin
    nFillLen := 1;
  end;

  if ((nStrSize - nFillLen) < 4) or (((nStrSize - nFillLen) mod 4) <> 0) then
  begin
    Result := 0;
    Exit;
  end;

  nCycle := (nStrSize - nFillLen) div 4; //上面的判断已经确保nCycle - nFillLen > 0
  nBufLen := nCycle * 3 - nFillLen;
  Result := nBufLen; //
  if (pData = nil) or (nDataSize < nBufLen) then
  begin  //缓冲区长度不够，返回真实长度
    exit;
  end;

  if (nFillLen <> 0) then
    nCycle := nCycle - 1; //如果有填充，则最后一个循环需要特殊处理

  pch := PAnsiChar(strBase64);
  pbData := pData;
  if (nCycle > 0) then
  begin
    for i := 0 to nCycle - 1 do
    begin
      i0 := Byte(pch[0]);
      i1 := Byte(pch[1]);
      i2 := Byte(pch[2]);
      i3 := Byte(pch[3]);
      if (i0 > $7F) or (i1 > $7F) or (i2 > $7F) or (i3 > $7F) then
      begin
        Result := 0;
        exit;
      end;
      d0 := DecodeTable[i0];
      d1 := DecodeTable[i1];
      d2 := DecodeTable[i2];
      d3 := DecodeTable[i3];
      nData := d3;
      nData := nData shl 6;
      nData := nData or d2;
      nData := nData shl 6;
      nData := nData or d1;
      nData := nData shl 6;
      nData := nData or d0;

             //兼容大小端模式
      PBYTE(UINT_PTR(pbData) + 0)^ := nData and $FF; //pbData[0] := nData and $FF;
      PBYTE(UINT_PTR(pbData) + 1)^ := (nData shr 8) and $FF; //pbData[1] := (nData shr 8)  and $FF;
      PBYTE(UINT_PTR(pbData) + 2)^ := (nData shr 16) and $FF; //pbData[2] := (nData shr 16) and $FF;
      Inc(pch, 4);
      Inc(pbData, 3);
    end;
  end;

  if (nFillLen <> 0) then
  begin
    i0 := Byte(pch[0]);
    i1 := Byte(pch[1]);
    i2 := Byte(pch[2]);
    i3 := Byte(pch[3]);
    if (i0 > $7F) or (i1 > $7F) or (i2 > $7F) or (i3 > $7F) then
    begin
      Result := 0;
      exit;
    end;
    d0 := DecodeTable[i0];
    d1 := DecodeTable[i1];
    d2 := DecodeTable[i2];
    d3 := DecodeTable[i3];
    nData := d3;
    nData := nData shl 6;
    nData := nData or d2;
    nData := nData shl 6;
    nData := nData or d1;
    nData := nData shl 6;
    nData := nData or d0;

    if (nFillLen = 1) then
    begin
      PBYTE(UintPtr(pbData) + 0)^ := nData and $FF; // pbData[0] := nData and $FF;
      PBYTE(UintPtr(pbData) + 1)^ := (nData shr 8) and $FF; //pbData[1] := (nData shr 8)  and $FF;
    end
    else if (nFillLen = 2) then
    begin
      PBYTE(UintPtr(pbData) + 0)^ := nData and $FF; // pbData[0] := nData and $FF;
    end;
  end;

    {$IFDEF USE_VMP}
  VMProtectEnd();
    {$ENDIF}
end;

function EncryptAndEncodeBase64(pData: Pointer; nDataSize: UINT): AnsiString;
var
  pLocalData: Pointer;
begin
  if nDataSize = 0 then
  begin
    Result := '';
    exit;
  end;

  pLocalData := GetMemory(nDataSize);
  try
    CopyMemory(pLocalData, pData, nDataSize);
    XOR_encrypt(pLocalData, nDataSize);
    Result := EncodeBase64(pLocalData, nDataSize);
  finally
    FreeMemory(pLocalData);
  end;
end;

function DecryptAndiDecodeBase64(strBase64: AnsiString; pData: Pointer; nDataSize: UINT): UINT;
var
  nRealDataSize: UINT;
begin
  nRealDataSize := DecodeBase64(strBase64, nil, 0);
  if (pData = nil) or (nDataSize < nRealDataSize) then
  begin
    Result := nRealDataSize;
    Exit;
  end;
  Result := DecodeBase64(strBase64, pData, nDataSize);
  XOR_decrypt(pData, nDataSize);
end;

function EncryptAndEncodeBase64String(strSrc: AnsiString): AnsiString;
var
  nStrSize: UINT;
begin
  nStrSize := Length(strSrc);
  if (nStrSize > 0) then
  begin
    XOR_encrypt(Pointer(@strSrc[1]), nStrSize);
    Result := EncodeBase64(PAnsiChar(strSrc), nStrSize);
  end
  else
  begin
    Result := '';
  end;
end;

function DecryptAndDecodeBase64String(strBase64: AnsiString): AnsiString;
var
  nDataSize: UINT;
begin
  nDataSize := DecodeBase64(strBase64, nil, 0);
  if nDataSize > 0 then
  begin
    SetLength(Result, nDataSize);
    if (DecodeBase64(strBase64, @Result[1], nDataSize) = 0) then
    begin
      Result := '';
    end
    else
    begin
      XOR_decrypt(Pointer(PAnsiChar(Result)), nDataSize);
    end;
  end
  else
  begin
    Result := '';
  end;
end;

function EncodeBase64String(strSrc: AnsiString): AnsiString;
var
  nStrSize: UINT;
begin
  nStrSize := Length(strSrc);
  Result := EncodeBase64(PAnsiChar(strSrc), nStrSize);
end;

function DecodeBase64String(strBase64: AnsiString): AnsiString;
var
  nDataSize: UINT;
begin
  nDataSize := DecodeBase64(strBase64, nil, 0);
  SetLength(Result, nDataSize);
  if (DecodeBase64(strBase64, PAnsiChar(Result), nDataSize) = 0) then
  begin
    Result := '';
  end;
end;

procedure InitDecodeTable();
var
  i: Integer;
begin
  {$IFDEF USE_VMP}
  VMProtectBeginVirtualization('PowerBase64.InitDecodeTable');
  {$ENDIF}

  ZeroMemory(@DecodeTable, SizeOf(DecodeTable)); //清零
  for i := 0 to 64 - 1 do
  begin
    DecodeTable[Byte(EncodeTable[i])] := i;
  end;

  {$IFDEF USE_VMP}
  VMProtectEnd();
  {$ENDIF}
end;

initialization
  InitDecodeTable; //初始化解码表



finalization

end.

