unit EDcode;

{
  版本：V1.0 chongchong

  重写编解码单元文件，原来的单元在加载反外挂模块时会出错

  原因是原来的代码，栈中分配了大量的空间，导致空间不足

  2017-01-05 开始重构并测试代码

}

interface

uses
  Windows, SysUtils, Classes, Grobal2_Ex;
     
const
  EDCODEBEIJING = 0; //1;
function GetEncodeSize(InSize: Integer): Integer;
function GetDecodeSize(InSize: Integer): Integer;

//-----------------------------------------------------------------------------------

function Encode6BitBuf(Src, Dest: PAnsiChar; SrcLen, DestLen: Integer): Integer;

function Decode6BitBuf(Src, Dest: PAnsiChar; SrcLen, DestLen: Integer): Integer;

function Encode6BitBuf_N(Src, Dest: PAnsiChar; SrcLen, DestLen: Integer): Integer;

function Decode6BitBuf_N(Src, Dest: PAnsiChar; SrcLen, DestLen: Integer): Integer;

function Encode6BitBuf_C(Src, Dest: PAnsiChar; SrcLen, DestLen: Integer): Integer;

function Decode6BitBuf_C(Src, Dest: PAnsiChar; SrcLen, DestLen: Integer): Integer;

function Encode6BitBuf_S(Src, Dest: PAnsiChar; SrcLen, DestLen: Integer): Integer;

function Decode6BitBuf_S(Src, Dest: PAnsiChar; SrcLen, DestLen: Integer): Integer;

//-----------------------------------------------------------------------------------

function MakeDefaultMsg(wIdent: Word; nRecog: Int64; wParam, wTag, wSeries: Word): TDefaultMessage;

function EncodeMessage(Msg: TDefaultMessage): string; overload;
function EncodeMessage(Msg: TDefaultMessage; Dest: PChar; DestLen: Integer): Integer; overload;

function DecodeMessage(const S: string): TDefaultMessage; overload;
function DecodeMessage(Src: PChar; SrcLen: Integer): TDefaultMessage; overload;

//-----------------------------------------------------------------------------------
function EncodeString(const S: string): string; overload;
function EncodeString(const S: string; Dest: PChar; DestLen: Integer): Integer; overload;

function EncodeBuffer(Src: PChar; SrcLen: Integer): string; overload;
function EncodeBuffer(Src: PChar; SrcLen: Integer; Dest: PChar; var DestLen: Integer): Integer; overload;

function DecodeString(const S: string): string; overload;
function DecodeString(const S: string; Dest: PChar; DestLen: Integer): Integer; overload;

function DecodeBuffer(Src: PChar; SrcLen: Integer): string; overload;
function DecodeBuffer(Src: PChar; SrcLen: Integer; Dest: PChar; DestLen: Integer): Integer; overload;

//-----------------------------------------------------------------------------------
function EncodeStringK(const S: string): string;
function DecodeStringK(const S: string): string;
//-----------------------------------------------------------------------------------

function zEncodeString(const S: string): string; overload;
function zEncodeString(Src: PChar; SrcLen: Integer): string; overload;

function zDecodeString(const S: string): string; overload;
function zDecodeString(Src: PChar; SrcLen: Integer): string; overload;

function zEncodeBuffer(Src: PChar; SrcLen: Integer): string;
function zDecodeBuffer(const S: string; Dest: PChar; DestLen: Integer): Integer;

//-----------------------------------------------------------------------------------

function zLibEncodeString(const S: string): string; overload;
function zLibEncodeString(const S: string; Dest: PChar; DestLen: Integer): Integer; overload;

function zLibEncodeBuffer(Src: PChar; SrcLen: Integer): string; overload;
function zLibEncodeBuffer(Src: PChar; SrcLen: Integer; Dest: PChar; DestLen: Integer): Integer; overload;

function zLibDecodeString(const S: string): string; overload;
function zLibDecodeString(const S: string; Dest: PChar; DestLen: Integer): Integer; overload;

function zLibDecodeBuffer(Src: PChar; SrcLen: Integer): string; overload;
function zLibDecodeBuffer(Src: PChar; SrcLen: Integer; Dest: PChar; DestLen: Integer): Integer; overload;

//-----------------------------------------------------------------------------------
function zLibCompressString(const S: string): string; overload;
function zLibCompressString(const S: string; Dest: PChar; DestLen: Integer): Integer; overload;

function zLibCompressBuffer(Src: PChar; SrcLen: Integer): string; overload;
function zLibCompressBuffer(Src: PChar; SrcLen: Integer; Dest: PChar; DestLen: Integer): Integer; overload;

function zLibDecompressString(const S: string): string; overload;
function zLibDecompressString(const S: string; Dest: PChar; DestLen: Integer): Integer; overload;

function zLibDecompressBuffer(Src: PChar; SrcLen: Integer): string; overload;
function zLibDecompressBuffer(Src: PChar; SrcLen: Integer; Dest: PChar; DestLen: Integer): Integer; overload;

implementation

uses
  ZlibEx;

function GetEncodeSize(InSize: Integer): Integer;
begin
  Result := (InSize * 4 + 2) div 3;
end;

function GetDecodeSize(InSize: Integer): Integer;
begin
  Result := Trunc(InSize * 3 / 4);
end;

//-----------------------------------------------------------------------------------

(*

int GetEncodeSize(int InSize)
{
return (int)(InSize * 4 + 2) / 3;
}

int GetDecodeSize(int InSize)
{
return (int)(InSize * 3) / 4;
}

int Encode(char* inData, int inDataLen, char* outData, int outDataSize)
{
if (GetEncodeSize(inDataLen) >= outDataSize)
inDataLen = GetDecodeSize(outDataSize);

unsigned char Tmp[3] = {0};
    for(int i = 0; i < (int)(inDataLen / 3); i++)
    {
        Tmp[0] = *inData++;
        Tmp[1] = *inData++;
        Tmp[2] = *inData++;
        *outData++ = ((Tmp[0] >> 2) & 0x3F) + 0x3C;
        *outData++ = (((Tmp[0] << 4) | (Tmp[1] >> 4)) & 0x3F) + 0x3C;
        *outData++ = (((Tmp[1] << 2) | (Tmp[2] >> 6)) & 0x3F) + 0x3C;
        *outData++ = (Tmp[2] & 0x3F) + 0x3C;
    }

    //对剩余数据进行编码
    int Mod = inDataLen % 3;
    if(Mod == 1)
    {
        Tmp[0] = *inData++;
        *outData++ = ((Tmp[0] >> 2) & 0x3F) + 0x3C;
        *outData++ = (((Tmp[0] & 0x03) << 4) & 0x3F) + 0x3C;
    }
    else if(Mod == 2)
    {
        Tmp[0] = *inData++;
        Tmp[1] = *inData++;
        *outData++ = ((Tmp[0] >> 2) & 0x3F) + 0x3C;
        *outData++ = (((Tmp[0] << 4) | (Tmp[1] >> 4)) & 0x3F) + 0x3C;
        *outData++ = ((Tmp[1] << 2) & 0x3F) + 0x3C;
    }

    return GetEncodeSize(inDataLen);
}

function Encode(InData, OutData: PByte; InDataLen, OutDataLen: Integer): Integer;
const
  OFFSET = $3C;
var
  I, Count: Integer;
  B1, B2, B3: Byte;
begin
  if (GetEncodeSize(InDataLen) >= OutDataLen) then
    InDataLen := GetDecodeSize(OutDataLen);

  Count := InDataLen div 3;
  for I := 0 to Count - 1 do
  begin
    B1 := InData^;   Inc(InData);
    B2 := InData^;   Inc(InData);
    B3 := InData^;   Inc(InData);

    {3Byte ---> 4Byte:  6  2  4  4  2  6  ->  6,  2 4,  4 2,  6}

    OutData^ := (B1 shr 2) and $3F + OFFSET;                     // B1前6位
    Inc(OutData);

    OutData^ := ((B1 shl 4) or (B2 shr 4)) and $3F + OFFSET;     // B1后2位，B2前4位
    Inc(OutData);

    OutData^ := ((B2 shl 2) or (B3 shr 6)) and $3F + OFFSET;     // B2后4位，B3前2位
    Inc(OutData);

    OutData^ := (B3 and $3F) + OFFSET;                           // B3后6位
    Inc(OutData);
  end;

  Count := InDataLen mod 3;
  if Count = 1 then
  begin
    B1 := InData^;   Inc(InData);

    OutData^ := (B1 shr 2) and $3F + OFFSET;                     // B1前6位
    Inc(OutData);

    OutData^ := ((B1 and $03) shl 4) and $3F + OFFSET;           // B1后2位，后面补0000
    Inc(OutData);
  end
  else if Count = 2 then
  begin
    B1 := InData^;   Inc(InData);
    B2 := InData^;   Inc(InData);

    OutData^ := (B1 shr 2) and $3F + OFFSET;                     // B1前6位
    Inc(OutData);

    OutData^ := ((B1 shl 4) or (B2 shr 4)) and $3F + OFFSET;     // B1后2位，B2前4位
    Inc(OutData);

    OutData^ := (B2 shl 2) and $3F + OFFSET;                    // B2后4位，后面补0000
    Inc(OutData);
  end;
end;
*)

function Encode6BitBuf(Src, Dest: PAnsiChar; SrcLen, DestLen: Integer): Integer;
begin
  {$IF EDCODEBEIJING = 0}
  Result := Encode6BitBuf_N(Src, Dest, SrcLen, DestLen);
  {$ELSE}
  Result := Encode6BitBuf_C(Src, Dest, SrcLen, DestLen);
  {$IFEND}

//  {$IFDEF EDCODESERVER}
//  Encode6BitBuf_S(Src, Dest, SrcLen, DestLen);
//  {$ELSE}
//  {$IFDEF EDCODECLIENT}
//  Encode6BitBuf_C(Src, Dest, SrcLen, DestLen);
//  {$ELSE}
//  Encode6BitBuf_N(Src, Dest, SrcLen, DestLen);
//  {$ENDIF}
//  {$ENDIF}
end;

function Decode6BitBuf(Src, Dest: PAnsiChar; SrcLen, DestLen: Integer): Integer;
begin
  {$IF EDCODEBEIJING = 0}
  Result := Decode6BitBuf_N(Src, Dest, SrcLen, DestLen);
  {$ELSE}
  Result := Decode6BitBuf_S(Src, Dest, SrcLen, DestLen);
  {$IFEND}
//  {$IFDEF EDCODESERVER}
//  Decode6BitBuf_S(Src, Dest, SrcLen, DestLen);
//  {$ELSE}
//  {$IFDEF EDCODECLIENT}
//  Decode6BitBuf_C(Src, Dest, SrcLen, DestLen);
//  {$ELSE}
//  Decode6BitBuf_N(Src, Dest, SrcLen, DestLen);
//  {$ENDIF}
//  {$ENDIF}
end;

function Encode6BitBuf_C(Src, Dest: PAnsiChar; SrcLen, DestLen: Integer): Integer;
// 修改
const
  mask: array[0..255] of Byte = ($3b, $9c, $d4, $7e, $d7, $98, $a2, $77, $41, $62, $1a, $c6, $c5, $ac, $5c, $18, $08, $b9, $ad, $78, $b4, $8a, $ee, $96, $e0, $8f, $b0, $f4, $6c, $46, $be, $97, $31, $6a, $63, $42, $4f, $36, $e9, $86, $05, $8e, $8c, $f1, $9b, $80, $fe, $0f, $e6, $a0, $aa, $df, $2d, $7d, $1e, $01, $9a, $8d, $a3, $8b, $74, $b8, $38, $4d, $64, $cf, $57, $82, $21, $e8, $e2, $dc, $47, $5a, $7a, $e3, $4b, $45, $06, $20, $d1, $26, $81, $70, $9f, $67, $30, $bd, $2c, $6b, $16, $25, $ce, $a4, $04, $ca,
    $0e, $02, $c3, $d0, $29, $5e, $09, $49, $bf, $9e, $da, $79, $d2, $7b, $60, $ea, $ef, $a6, $12, $a7, $9d, $c1, $b3, $c9, $91, $bb, $19, $07, $51, $2b, $a1, $94, $68, $bc, $69, $fb, $ff, $52, $34, $d3, $93, $75, $b2, $7f, $65, $5f, $0d, $a8, $7c, $87, $66, $83, $d9, $88, $54, $58, $f6, $32, $a9, $92, $cd, $90, $23, $5b, $fa, $eb, $3a, $6d, $03, $6f, $2f, $22, $55, $ab, $e4, $3d, $4e, $0c, $e5, $dd, $f5, $17, $de, $cb, $39, $f7, $c7, $d6, $0a, $d5, $cc, $43, $59, $1b, $1c, $3c, $fd, $ba, $d8, $3f, $53, $fc,
    $c8, $71, $3e, $76, $37, $73, $ae, $28, $e7, $b6, $48, $e1, $15, $95, $f3, $35, $13, $84, $1d, $00, $1f, $f2, $ec, $72, $4c, $61, $24, $f8, $4a, $40, $50, $a5, $ed, $f0, $b5, $89, $33, $14, $c2, $af, $2a, $5d, $0b, $85, $c4, $99, $11, $56, $b7, $2e, $c0, $db, $44, $10, $b1, $27, $f9, $6e);
  mask2: array[0..255] of Byte = ($de, $73, $19, $1b, $70, $46, $9b, $44, $79, $76, $1d, $e8, $55, $50, $96, $c7, $c8, $86, $05, $b3, $63, $c0, $e3, $23, $30, $3a, $74, $89, $f1, $d4, $91, $6d, $1a, $65, $ec, $ba, $a3, $ed, $82, $9f, $9a, $42, $04, $a0, $e1, $d9, $37, $d7, $5b, $03, $94, $8d, $18, $39, $9c, $10, $bb, $a8, $07, $26, $3b, $6b, $d2, $bc, $c9, $71, $5f, $a4, $d0, $a9, $54, $22, $98, $64, $31, $8f, $f7, $cc, $e0, $1e, $c4, $2a, $c5, $87, $0a, $48, $dd, $c1, $58, $fa, $cb, $c2, $bd, $e6, $5c, $0f,
    $47, $fc, $06, $60, $4e, $cf, $09, $7e, $1f, $ef, $b8, $62, $81, $80, $28, $4d, $57, $b6, $08, $95, $a1, $56, $db, $4a, $2b, $a5, $ad, $69, $1c, $f9, $20, $83, $59, $0c, $fd, $b5, $68, $2e, $fe, $0d, $ae, $88, $ac, $99, $4c, $a2, $4b, $12, $41, $3f, $f6, $af, $67, $0b, $5d, $6a, $ca, $52, $72, $17, $0e, $93, $11, $00, $fb, $2d, $3d, $b4, $d1, $5a, $75, $25, $5e, $ff, $dc, $3c, $d6, $7f, $ee, $7a, $4f, $d3, $c6, $90, $92, $d5, $27, $c3, $6e, $8c, $16, $f0, $43, $78, $6f, $24, $7c, $49, $29, $8b, $38, $53,
    $8a, $45, $7d, $33, $8e, $66, $13, $a7, $df, $da, $3e, $eb, $e7, $2c, $b1, $cd, $ce, $f8, $d8, $b0, $61, $b9, $f2, $ab, $36, $f5, $e9, $bf, $e4, $9d, $02, $84, $b7, $f3, $35, $e2, $85, $2f, $15, $14, $77, $b2, $aa, $6c, $32, $21, $e5, $34, $97, $01, $51, $40, $f4, $ea, $7b, $be, $a6, $9e);
var
  I: Integer;
  pbSrc, pbDest: PByte;
  CurrentBit6Value: Byte;
  SurplusValue: Byte;     // 剩余值
  SurplusCount: Byte;     // 剩余Bit位数
  ResultLen: Integer;

  // 修改
  encodeLength: Byte;
  c: Byte;
begin
  ResultLen := GetEncodeSize(SrcLen);
  if ResultLen > DestLen then
    SrcLen := GetDecodeSize(DestLen);

  //Assert(ResultLen <= DestLen, '编码目标长度不够，DestLen:' + IntToStr(DestLen));

  SurplusValue := 0;
  SurplusCount := 0;

  pbSrc := PByte(Src);
  pbDest := PByte(Dest);

  // 修改
  encodeLength := mask[((SrcLen + (SrcLen + 2) div 3) mod 256) xor $90];

  for I := 0 to SrcLen - 1 do
  begin
    // 修改
    c := pbSrc^;
    c := mask2[c xor $CC] xor encodeLength;

    CurrentBit6Value := (SurplusValue or (c shr (2 + SurplusCount))) and $3F;
    SurplusValue := (({pbSrc^}c shl (8 - (2 + SurplusCount))) shr 2) and $3F;
    Inc(SurplusCount, 2);

    pbDest^ := CurrentBit6Value + $3C;
    Inc(pbDest);

    if SurplusCount = 6 then
    begin
      pbDest^ := SurplusValue + $3C;
      Inc(pbDest);
      SurplusValue := 0;
      SurplusCount := 0;
    end;

    Inc(pbSrc);
  end;

  if SurplusCount > 0 then
  begin
    pbDest^ := SurplusValue + $3C;
  end;

  Result := GetEncodeSize(SrcLen);
end;

function Decode6BitBuf_C(Src, Dest: PAnsiChar; SrcLen, DestLen: Integer): Integer;
// 修改
const
  reverse3: array[0..255] of Byte = ($7b, $57, $5c, $05, $74, $51, $b9, $d0, $fd, $4a, $71, $83, $93, $e1, $90, $82, $c2, $4e, $7e, $45, $b6, $fa, $fc, $6e, $1c, $3f, $b5, $07, $04, $70, $65, $2e, $2c, $58, $7c, $9b, $4f, $4c, $5e, $73, $03, $3a, $85, $81, $bc, $6f, $d5, $95, $30, $17, $15, $99, $80, $96, $23, $e3, $df, $ab, $3d, $31, $54, $cb, $ef, $19, $a7, $21, $10, $61, $cd, $48, $aa, $0b, $22, $5d, $28, $37, $f9, $c6, $9c, $62, $a6, $86, $79, $d2, $77, $e5, $98, $8b, $84, $c4, $14, $a9, $1f, $ce, $b4,
    $5f, $9d, $f5, $34, $66, $bd, $ad, $b8, $0a, $56, $f6, $9f, $0d, $c3, $44, $6c, $de, $26, $c8, $c1, $bb, $4b, $50, $9e, $91, $6a, $2b, $53, $92, $4d, $0f, $d8, $fb, $75, $42, $6b, $88, $12, $b2, $f0, $60, $89, $13, $d4, $bf, $3c, $b3, $52, $f2, $c0, $dc, $d6, $ca, $20, $59, $cc, $02, $ec, $76, $1e, $9a, $d1, $38, $08, $5a, $f8, $0c, $fe, $e7, $68, $8e, $a3, $78, $d9, $7a, $47, $09, $a8, $db, $b0, $d7, $94, $e8, $8f, $87, $46, $e6, $e9, $33, $2a, $8d, $e0, $be, $1d, $f3, $32, $01, $e4, $35, $16, $43, $0e,
    $c9, $2d, $67, $ee, $c5, $69, $00, $25, $b1, $a1, $63, $55, $11, $49, $ba, $7d, $da, $3e, $a4, $6d, $f7, $2f, $ae, $cf, $5b, $a5, $ed, $ac, $1b, $18, $dd, $97, $1a, $72, $36, $ff, $c7, $f1, $39, $7f, $41, $8a, $f4, $24, $27, $8c, $40, $64, $a2, $e2, $ea, $06, $29, $eb, $d3, $a0, $af, $b7, $3b);
  reverse4: array[0..255] of Byte = ($3d, $87, $89, $1f, $82, $64, $94, $57, $66, $24, $21, $e6, $b8, $ce, $a6, $50, $ba, $5b, $06, $8f, $28, $a4, $fd, $43, $df, $39, $ea, $6e, $67, $96, $d1, $d3, $7f, $aa, $e5, $fe, $ee, $83, $70, $c0, $e4, $20, $f7, $e9, $84, $90, $46, $e0, $45, $37, $11, $35, $b7, $60, $6d, $9f, $72, $de, $7a, $65, $e1, $2d, $a8, $13, $0c, $91, $0d, $bb, $af, $8c, $5a, $f4, $30, $53, $40, $74, $ef, $44, $9d, $b4, $f2, $17, $2b, $fc, $c7, $bc, $23, $b2, $55, $27, $54, $75, $5e, $b0, $f9,
    $1a, $3b, $fa, $6b, $6f, $2e, $ab, $ad, $99, $0b, $63, $22, $36, $05, $fb, $5d, $4e, $93, $42, $e8, $d0, $3c, $8d, $ac, $7c, $04, $a0, $0f, $cc, $5c, $c3, $d9, $9c, $4b, $29, $7e, $e2, $86, $33, $3e, $00, $34, $10, $73, $ca, $07, $f5, $62, $02, $26, $52, $cd, $eb, $a9, $c5, $59, $85, $cb, $1d, $14, $7b, $9a, $9e, $15, $8a, $12, $68, $2c, $69, $b9, $0a, $4c, $8b, $f8, $be, $f6, $7d, $ae, $ff, $16, $97, $80, $0e, $56, $c2, $a5, $47, $1c, $5f, $a3, $a2, $c1, $76, $bd, $81, $08, $6a, $d2, $58, $03, $a7, $cf,
    $6c, $f3, $01, $4f, $b5, $e3, $32, $b6, $d7, $61, $db, $dd, $bf, $95, $d6, $b1, $c9, $c6, $9b, $77, $c4, $d4, $31, $88, $25, $18, $79, $8e, $e7, $1b, $2a, $51, $98, $4a, $f1, $da, $1e, $3a, $49, $3f, $78, $48, $b3, $a1, $ed, $dc, $4d, $f0, $41, $d5, $09, $38, $71, $d8, $19, $c8, $92, $ec, $2f);
var
  I: Integer;
  bSrc: Byte;
  pbSrc, pbDest: PByte;
  CurrentBit8Value: Byte;
  SurplusValue: Byte;     // 剩余值
  SurplusCount: Byte;     // 剩余Bit位数
  ResultLen: Integer;

  // 修改
  encodeLength: Byte;
begin
  Result := 0;

  ResultLen := GetDecodeSize(SrcLen);
  if ResultLen > DestLen then
    SrcLen := GetEncodeSize(DestLen);

  //Assert(ResultLen <= DestLen, '编码目标长度不够，DestLen:' + IntToStr(DestLen));

  SurplusValue := 0;
  SurplusCount := 0;

  pbSrc := PByte(Src);
  pbDest := PByte(Dest);

  // 修改
  encodeLength := (SrcLen mod 256) xor $BA;

  for I := 0 to SrcLen - 1 do
  begin
    if (pbSrc^ < $3C) or (pbSrc^ >= $3C + 64) then
    begin
      FillChar(Dest^, DestLen, 0);
      Exit;
    end;

    bSrc := pbSrc^ - $3C;
    if SurplusCount > 0 then
    begin
      // 6 - (8 - SurplusCount) = SurplusCount - 2
      CurrentBit8Value := (SurplusValue or (bSrc shr (SurplusCount - 2)));

      // 修改
      CurrentBit8Value := reverse4[reverse3[CurrentBit8Value xor encodeLength]] xor $A8;

      pbDest^ := CurrentBit8Value;
      Inc(pbDest);
      SurplusValue := bSrc shl (8 - (SurplusCount - 2));
      SurplusCount := SurplusCount - 2;
    end
    else
    begin
      SurplusCount := 6;
      SurplusValue := (bSrc shl 2) and $FC;
    end;

    Inc(pbSrc);
  end;

  Result := GetDecodeSize(SrcLen);
end;
//

//

//===================================================================================

function Encode6BitBuf_S(Src, Dest: PAnsiChar; SrcLen, DestLen: Integer): Integer;
// 修改
const
  mask3: array[0..255] of Byte = ($cb, $bf, $97, $28, $1c, $03, $f8, $1b, $9e, $ab, $67, $47, $a1, $6b, $c4, $7d, $42, $d1, $84, $89, $5a, $32, $c2, $31, $e2, $3f, $e5, $e1, $18, $bc, $9a, $5c, $94, $41, $48, $36, $f0, $cc, $70, $f1, $4a, $f9, $b8, $79, $20, $c6, $1f, $da, $30, $3b, $be, $b7, $62, $c1, $e7, $4b, $9d, $eb, $29, $ff, $8c, $3a, $d6, $19, $f3, $ed, $81, $c3, $6d, $13, $b4, $aa, $45, $d2, $09, $74, $25, $7c, $11, $24, $75, $05, $8e, $7a, $3c, $d0, $68, $01, $21, $95, $9f, $dd, $02, $49, $26, $5f,
    $87, $43, $4f, $cf, $f4, $1e, $63, $c7, $a4, $ca, $78, $82, $6e, $d8, $17, $2d, $1d, $0a, $e6, $27, $04, $80, $99, $54, $a7, $52, $a9, $00, $22, $d4, $12, $ec, $34, $2b, $0f, $0b, $58, $2a, $51, $b3, $83, $88, $ee, $57, $f2, $b9, $a5, $b2, $0e, $77, $7b, $0c, $b0, $2f, $35, $e4, $56, $33, $9b, $23, $4e, $60, $76, $6a, $fc, $ce, $f5, $a6, $d7, $de, $50, $40, $ac, $5b, $46, $39, $e0, $65, $db, $fd, $ae, $cd, $85, $8d, $5e, $1a, $14, $fe, $66, $06, $d3, $73, $2c, $64, $bb, $8b, $90, $72, $10, $6c, $59, $c9,
    $4d, $e9, $71, $c5, $93, $3d, $96, $44, $5d, $dc, $07, $9c, $53, $fb, $8a, $2e, $92, $af, $7e, $a8, $d5, $ad, $91, $e3, $6f, $38, $ba, $0d, $f6, $37, $c0, $55, $b5, $a3, $b1, $b6, $f7, $fa, $98, $df, $c8, $3e, $86, $ea, $8f, $bd, $ef, $61, $69, $d9, $a0, $4c, $15, $7f, $16, $08, $a2, $e8);
  mask4: array[0..255] of Byte = ($87, $c7, $8f, $c2, $78, $6c, $12, $8c, $be, $f7, $a5, $68, $40, $42, $b1, $7a, $89, $32, $a0, $3f, $9a, $9e, $ae, $51, $de, $fb, $5f, $e2, $b6, $99, $e9, $03, $29, $0a, $6a, $56, $09, $dd, $90, $59, $14, $81, $e3, $52, $a2, $3d, $64, $ff, $48, $db, $cb, $85, $88, $33, $6b, $31, $f8, $19, $ea, $60, $74, $00, $86, $ec, $4a, $f5, $71, $17, $4d, $30, $2e, $b5, $ee, $eb, $e6, $80, $a6, $f3, $6f, $c8, $0f, $e4, $91, $49, $5a, $58, $b2, $07, $c1, $96, $46, $11, $7c, $6e, $5c, $b7,
    $35, $ce, $8e, $69, $05, $3b, $08, $1c, $a1, $a3, $bf, $62, $c5, $36, $1b, $63, $26, $f9, $38, $8a, $4b, $5b, $bb, $d8, $ed, $df, $3a, $9b, $77, $ab, $82, $20, $b0, $bd, $04, $25, $2c, $97, $84, $01, $dc, $02, $9f, $a7, $45, $75, $e0, $13, $2d, $41, $fd, $70, $06, $d2, $1d, $af, $e5, $67, $9c, $d7, $7f, $4e, $9d, $37, $79, $f0, $b9, $b8, $15, $b4, $0e, $c3, $3e, $94, $21, $65, $76, $66, $ac, $44, $5d, $d4, $57, $ef, $4f, $c9, $cc, $34, $0c, $a4, $10, $43, $55, $bc, $a9, $d1, $27, $ba, $b3, $7d, $d9, $95,
    $d6, $54, $fc, $d5, $8b, $98, $7b, $92, $0d, $c4, $73, $1e, $c0, $1f, $da, $f6, $d3, $cd, $fa, $7e, $e8, $cf, $f2, $d0, $39, $18, $2f, $3c, $83, $ca, $28, $22, $0b, $e1, $72, $2b, $1a, $93, $fe, $f1, $24, $4c, $f4, $e7, $50, $c6, $47, $8d, $aa, $2a, $a8, $5e, $61, $6d, $53, $16, $23, $ad);
var
  I: Integer;
  pbSrc, pbDest: PByte;
  CurrentBit6Value: Byte;
  SurplusValue: Byte;     // 剩余值
  SurplusCount: Byte;     // 剩余Bit位数
  ResultLen: Integer;

  // 修改
  encodeLength: Byte;
  c: Byte;
begin
  ResultLen := GetEncodeSize(SrcLen);
  if ResultLen > DestLen then
    SrcLen := GetDecodeSize(DestLen);

  //Assert(ResultLen <= DestLen, '编码目标长度不够，DestLen:' + IntToStr(DestLen));

  SurplusValue := 0;
  SurplusCount := 0;

  pbSrc := PByte(Src);
  pbDest := PByte(Dest);

  // 修改
  encodeLength := ((SrcLen + (SrcLen + 2) div 3) mod 256) xor $BA;

  for I := 0 to SrcLen - 1 do
  begin
    // 修改
    c := pbSrc^;
    c := mask3[mask4[c xor $A8]] xor encodeLength;

    CurrentBit6Value := (SurplusValue or (c shr (2 + SurplusCount))) and $3F;
    SurplusValue := ((c{pbSrc^}  shl (8 - (2 + SurplusCount))) shr 2) and $3F;
    Inc(SurplusCount, 2);

    pbDest^ := CurrentBit6Value + $3C;
    Inc(pbDest);

    if SurplusCount = 6 then
    begin
      pbDest^ := SurplusValue + $3C;
      Inc(pbDest);
      SurplusValue := 0;
      SurplusCount := 0;
    end;

    Inc(pbSrc);
  end;

  if SurplusCount > 0 then
  begin
    pbDest^ := SurplusValue + $3C;
  end;

  Result := GetEncodeSize(SrcLen);
end;

function Decode6BitBuf_S(Src, Dest: PAnsiChar; SrcLen, DestLen: Integer): Integer;
// 修改
const
  mask: array[0..255] of Byte = ($3b, $9c, $d4, $7e, $d7, $98, $a2, $77, $41, $62, $1a, $c6, $c5, $ac, $5c, $18, $08, $b9, $ad, $78, $b4, $8a, $ee, $96, $e0, $8f, $b0, $f4, $6c, $46, $be, $97, $31, $6a, $63, $42, $4f, $36, $e9, $86, $05, $8e, $8c, $f1, $9b, $80, $fe, $0f, $e6, $a0, $aa, $df, $2d, $7d, $1e, $01, $9a, $8d, $a3, $8b, $74, $b8, $38, $4d, $64, $cf, $57, $82, $21, $e8, $e2, $dc, $47, $5a, $7a, $e3, $4b, $45, $06, $20, $d1, $26, $81, $70, $9f, $67, $30, $bd, $2c, $6b, $16, $25, $ce, $a4, $04, $ca,
    $0e, $02, $c3, $d0, $29, $5e, $09, $49, $bf, $9e, $da, $79, $d2, $7b, $60, $ea, $ef, $a6, $12, $a7, $9d, $c1, $b3, $c9, $91, $bb, $19, $07, $51, $2b, $a1, $94, $68, $bc, $69, $fb, $ff, $52, $34, $d3, $93, $75, $b2, $7f, $65, $5f, $0d, $a8, $7c, $87, $66, $83, $d9, $88, $54, $58, $f6, $32, $a9, $92, $cd, $90, $23, $5b, $fa, $eb, $3a, $6d, $03, $6f, $2f, $22, $55, $ab, $e4, $3d, $4e, $0c, $e5, $dd, $f5, $17, $de, $cb, $39, $f7, $c7, $d6, $0a, $d5, $cc, $43, $59, $1b, $1c, $3c, $fd, $ba, $d8, $3f, $53, $fc,
    $c8, $71, $3e, $76, $37, $73, $ae, $28, $e7, $b6, $48, $e1, $15, $95, $f3, $35, $13, $84, $1d, $00, $1f, $f2, $ec, $72, $4c, $61, $24, $f8, $4a, $40, $50, $a5, $ed, $f0, $b5, $89, $33, $14, $c2, $af, $2a, $5d, $0b, $85, $c4, $99, $11, $56, $b7, $2e, $c0, $db, $44, $10, $b1, $27, $f9, $6e);
  reverse2: array[0..255] of Byte = ($9f, $f7, $e4, $31, $2a, $12, $62, $3a, $72, $66, $54, $95, $81, $87, $9c, $5f, $37, $9e, $8f, $cc, $ed, $ec, $ba, $9b, $34, $02, $20, $03, $7c, $0a, $4f, $68, $7e, $f3, $47, $17, $bf, $a7, $3b, $b6, $6e, $c2, $51, $78, $d3, $a1, $85, $eb, $18, $4a, $f2, $c9, $f5, $e8, $de, $2e, $c4, $35, $19, $3c, $ab, $a2, $d0, $91, $f9, $90, $29, $bc, $07, $c7, $05, $60, $55, $c1, $77, $8e, $8c, $6f, $64, $b0, $0d, $f8, $99, $c5, $46, $0c, $75, $70, $58, $80, $a5, $30, $5e, $96, $a8,
    $42, $63, $da, $6b, $14, $49, $21, $cb, $94, $84, $7b, $97, $3d, $f1, $1f, $b8, $be, $04, $41, $9a, $01, $1a, $a6, $09, $ee, $bd, $08, $af, $fc, $c0, $c8, $67, $ad, $6d, $6c, $26, $7f, $e5, $ea, $11, $53, $89, $1b, $c6, $c3, $b9, $33, $ca, $4b, $b3, $1e, $b4, $9d, $32, $73, $0e, $f6, $48, $8b, $28, $06, $36, $e3, $ff, $27, $2b, $74, $8d, $24, $43, $79, $fe, $cd, $39, $45, $f0, $dd, $8a, $7a, $88, $93, $d9, $d4, $ef, $13, $a3, $83, $71, $e6, $6a, $db, $23, $38, $3f, $5c, $fd, $e1, $15, $57, $5b, $b7, $50,
    $52, $b2, $0f, $10, $40, $98, $5a, $4d, $d5, $d6, $65, $44, $a4, $3e, $b1, $1d, $b5, $ac, $2f, $d8, $2d, $cf, $76, $aa, $56, $00, $ce, $4e, $2c, $e9, $16, $e2, $f4, $5d, $d2, $0b, $e0, $fb, $d1, $22, $25, $ae, $69, $bb, $1c, $dc, $e7, $fa, $df, $92, $4c, $d7, $7d, $59, $a0, $61, $82, $86, $a9);
var
  I: Integer;
  bSrc: Byte;
  pbSrc, pbDest: PByte;
  CurrentBit8Value: Byte;
  SurplusValue: Byte;     // 剩余值
  SurplusCount: Byte;     // 剩余Bit位数
  ResultLen: Integer;

  // 修改
  encodeLength: Byte;
begin
  Result := 0;

  ResultLen := GetDecodeSize(SrcLen);
  if ResultLen > DestLen then
    SrcLen := GetEncodeSize(DestLen);

  //Assert(ResultLen <= DestLen, '编码目标长度不够，DestLen:' + IntToStr(DestLen));

  SurplusValue := 0;
  SurplusCount := 0;

  pbSrc := PByte(Src);
  pbDest := PByte(Dest);

  // 修改
  encodeLength := mask[(SrcLen mod 256) xor $90];

  for I := 0 to SrcLen - 1 do
  begin
    if (pbSrc^ < $3C) or (pbSrc^ >= $3C + 64) then
    begin
      FillChar(Dest^, DestLen, 0);
      Exit;
    end;

    bSrc := pbSrc^ - $3C;
    if SurplusCount > 0 then
    begin
      // 6 - (8 - SurplusCount) = SurplusCount - 2
      CurrentBit8Value := (SurplusValue or (bSrc shr (SurplusCount - 2)));

      // 修改
      CurrentBit8Value := reverse2[CurrentBit8Value xor encodeLength] xor $CC;

      pbDest^ := CurrentBit8Value;
      Inc(pbDest);
      SurplusValue := bSrc shl (8 - (SurplusCount - 2));
      SurplusCount := SurplusCount - 2;
    end
    else
    begin
      SurplusCount := 6;
      SurplusValue := (bSrc shl 2) and $FC;
    end;

    Inc(pbSrc);
  end;

  Result := GetDecodeSize(SrcLen);
end;

function Encode6BitBuf_N(Src, Dest: PAnsiChar; SrcLen, DestLen: Integer): Integer;
var
  I: Integer;
  pbSrc, pbDest: PByte;
  CurrentBit6Value: Byte;
  SurplusValue: Byte;     // 剩余值
  SurplusCount: Byte;     // 剩余Bit位数
  ResultLen: Integer;
begin
  ResultLen := GetEncodeSize(SrcLen);
  if ResultLen > DestLen then               
    SrcLen := GetDecodeSize(DestLen);

  //Assert(ResultLen <= DestLen, '编码目标长度不够，DestLen:' + IntToStr(DestLen));

  SurplusValue := 0;
  SurplusCount := 0;

  pbSrc := PByte(Src);
  pbDest := PByte(Dest);

  for I := 0 to SrcLen - 1 do
  begin
    CurrentBit6Value := (SurplusValue or (pbSrc^ shr (2 + SurplusCount))) and $3F;
    SurplusValue := ((pbSrc^ shl (8 - (2 + SurplusCount))) shr 2) and $3F;
    Inc(SurplusCount, 2);

    pbDest^ := CurrentBit6Value + $3C;
    Inc(pbDest);

    if SurplusCount = 6 then
    begin
      pbDest^ := SurplusValue + $3C;
      Inc(pbDest);
      SurplusValue := 0;
      SurplusCount := 0;
    end;

    Inc(pbSrc);
  end;

  if SurplusCount > 0 then
  begin
    pbDest^ := SurplusValue + $3C;
  end;

  Result := GetEncodeSize(SrcLen);
end;

function Decode6BitBuf_N(Src, Dest: PAnsiChar; SrcLen, DestLen: Integer): Integer;
var
  I: Integer;
  bSrc: Byte;
  pbSrc, pbDest: PByte;
  CurrentBit8Value: Byte;
  SurplusValue: Byte;     // 剩余值
  SurplusCount: Byte;     // 剩余Bit位数
  ResultLen: Integer;
begin
  Result := 0;

  ResultLen := GetDecodeSize(SrcLen);
  if ResultLen > DestLen then
    SrcLen := GetEncodeSize(DestLen);

  //Assert(ResultLen <= DestLen, '编码目标长度不够，DestLen:' + IntToStr(DestLen));

  SurplusValue := 0;
  SurplusCount := 0;

  pbSrc := PByte(Src);
  pbDest := PByte(Dest);

  for I := 0 to SrcLen - 1 do
  begin
    if (pbSrc^ < $3C) or (pbSrc^ >= $3C + 64) then
    begin
      FillChar(Dest^, DestLen, 0);
      Exit;
    end;

    bSrc := pbSrc^ - $3C;
    if SurplusCount > 0 then
    begin
      // 6 - (8 - SurplusCount) = SurplusCount - 2
      CurrentBit8Value := (SurplusValue or (bSrc shr (SurplusCount - 2)));
      pbDest^ := CurrentBit8Value;
      Inc(pbDest);
      SurplusValue := bSrc shl (8 - (SurplusCount - 2));
      SurplusCount := SurplusCount - 2;
    end
    else
    begin
      SurplusCount := 6;
      SurplusValue := (bSrc shl 2) and $FC;
    end;

    Inc(pbSrc);
  end;

  Result := GetDecodeSize(SrcLen);
end;


//-----------------------------------------------------------------------------------

function MakeDefaultMsg(wIdent: Word; nRecog: Int64; wParam, wTag, wSeries: Word): TDefaultMessage;
begin
  //Result.Code := $B0A04CC0;
  Result.Recog := nRecog;
  Result.Ident := wIdent;
  Result.Param := wParam;
  Result.Tag := wTag;
  Result.Series := wSeries;
end;

function EncodeMessage(Msg: TDefaultMessage): string;
begin
  SetLength(Result, GetEncodeSize(SizeOf(TDefaultMessage)));
  Encode6BitBuf(@Msg, @Result[1], SizeOf(Msg), Length(Result));
end;

function EncodeMessage(Msg: TDefaultMessage; Dest: PChar; DestLen: Integer): Integer;
begin
  Result := Encode6BitBuf(@Msg, Dest, SizeOf(Msg), DestLen);
end;

function DecodeMessage(const S: string): TDefaultMessage;
begin
  if Length(S) = 0 then Exit;

  Decode6BitBuf(@S[1], @Result, Length(S), SizeOf(Result));
end;

function DecodeMessage(Src: PChar; SrcLen: Integer): TDefaultMessage;
begin
  if SrcLen = 0 then Exit;
  Decode6BitBuf(Src, @Result, SrcLen, SizeOf(Result));
end;

function EncodeString(const S: string): string;
begin
  if Length(S) = 0 then
  begin
    Result := '';
    Exit;
  end;

  SetLength(Result, GetEncodeSize(Length(S)));
  Encode6BitBuf(@S[1], @Result[1], Length(S), Length(Result));
end;

//-----------------------------------------------------------------------------------

function EncodeString(const S: string; Dest: PChar; DestLen: Integer): Integer;
begin
  if Length(S) = 0 then
  begin
    Result := 0;
    Exit;
  end;

  Result := Encode6BitBuf(@S[1], Dest, Length(S), DestLen);
end;

function EncodeBuffer(Src: PChar; SrcLen: Integer): string;
begin
  if SrcLen = 0 then
  begin
    Result := '';
    Exit;
  end;

  SetLength(Result, GetEncodeSize(SrcLen));
  Encode6BitBuf(Src, @Result[1], SrcLen, Length(Result));
end;

function EncodeBuffer(Src: PChar; SrcLen: Integer; Dest: PChar; var DestLen: Integer): Integer;
begin
  if SrcLen = 0 then
  begin
    Result := 0;
    Exit;
  end;

  Result := Encode6BitBuf(Src, Dest, SrcLen, DestLen);
end;

function DecodeString(const S: string): string;
begin
  if Length(S) = 0 then
  begin
    Result := '';
    Exit;
  end;

  SetLength(Result, GetDecodeSize(Length(S)));
  Decode6BitBuf(@S[1], @Result[1], Length(S), Length(Result));
end;

function DecodeString(const S: string; Dest: PChar; DestLen: Integer): Integer;
begin
  if Length(S) = 0 then
  begin
    Result := 0;
    Exit;
  end;

  Result := Decode6BitBuf(@S[1], Dest, Length(S), DestLen);
end;

function DecodeBuffer(Src: PChar; SrcLen: Integer): string;
begin
  if SrcLen = 0 then
  begin
    Result := '';
    Exit;
  end;

  SetLength(Result, GetDecodeSize(SrcLen));
  Decode6BitBuf(Src, @Result[1], SrcLen, Length(Result));
end;

function DecodeBuffer(Src: PChar; SrcLen: Integer; Dest: PChar; DestLen: Integer): Integer;
begin
  if SrcLen = 0 then
  begin
    Result := 0;
    Exit;
  end;

  Result := Decode6BitBuf(Src, Dest, SrcLen, DestLen);
end;

//--------------------------------------------------------------------------------

function EncodeStringK(const S: string): string;
begin
  Result := EncodeString(S);
end;

function DecodeStringK(const S: string): string;
begin
  Result := DecodeString(S);
end;

//--------------------------------------------------------------------------------

function zEncodeString(const S: string): string;
begin
  Result := EncodeString(S);
end;

function zEncodeString(Src: PChar; SrcLen: Integer): string;
begin
  Result := EncodeBuffer(Src, SrcLen);
end;

function zDecodeString(const S: string): string;
begin
  Result := DecodeString(S);
end;

function zDecodeString(Src: PChar; SrcLen: Integer): string;
begin
  Result := DecodeBuffer(Src, SrcLen);
end;

function zEncodeBuffer(Src: PChar; SrcLen: Integer): string;
begin
  Result := EncodeBuffer(Src, SrcLen);
end;

function zDecodeBuffer(const S: string; Dest: PChar; DestLen: Integer): Integer;
begin
  Result := DecodeString(S, Dest, DestLen);
end;

//--------------------------------------------------------------------------------

function zLibEncodeString(const S: string): string;
var
  zLibBuf: PChar;
  zLibBufSize: Integer;
begin
  Result := '';
  if Length(S) = 0 then Exit;

  zLibBuf := nil;
  zLibBufSize := 0;

  try
    CompressBuf(@S[1], Length(S), Pointer(zLibBuf), zLibBufSize);
  except
    if zLibBuf <> nil then FreeMem(zLibBuf);
    zLibBufSize := 0;
  end;

  if zLibBufSize > 0 then
  begin
    SetLength(Result, GetEncodeSize(zLibBufSize));
    Encode6BitBuf(zLibBuf, @Result[1], zLibBufSize, Length(Result));
    FreeMem(zLibBuf);
  end;
end;

function zLibEncodeString(const S: string; Dest: PChar; DestLen: Integer): Integer;
var
  zLibBuf: PChar;
  zLibBufSize: Integer;
begin
  Result := 0;
  if Length(S) = 0 then Exit;

  zLibBuf := nil;
  zLibBufSize := 0;

  try
    CompressBuf(@S[1], Length(S), Pointer(zLibBuf), zLibBufSize);
  except
    if zLibBuf <> nil then FreeMem(zLibBuf);
    zLibBufSize := 0;
  end;

  if zLibBufSize > 0 then
  begin
    Assert(GetEncodeSize(zLibBufSize) <= DestLen, 'zLibEncodeString压缩编码目标长度不够，DestLen:' + IntToStr(DestLen));

    Result := Encode6BitBuf(zLibBuf, Dest, zLibBufSize, DestLen);
    FreeMem(zLibBuf);
  end;
end;

function zLibEncodeBuffer(Src: PChar; SrcLen: Integer): string;
var
  zLibBuf: PChar;
  zLibBufSize: Integer;
begin
  Result := '';
  if SrcLen = 0 then Exit;

  zLibBuf := nil;
  zLibBufSize := 0;

  try
    CompressBuf(Src, SrcLen, Pointer(zLibBuf), zLibBufSize);
  except
    if zLibBuf <> nil then FreeMem(zLibBuf);
    zLibBufSize := 0;
  end;

  if zLibBufSize > 0 then
  begin
    SetLength(Result, GetEncodeSize(zLibBufSize));
    Encode6BitBuf(zLibBuf, @Result[1], zLibBufSize, Length(Result));
    FreeMem(zLibBuf);
  end;
end;

function zLibEncodeBuffer(Src: PChar; SrcLen: Integer; Dest: PChar; DestLen: Integer): Integer;
var
  zLibBuf: PChar;
  zLibBufSize: Integer;
begin
  Result := 0;
  if SrcLen = 0 then Exit;

  zLibBuf := nil;
  zLibBufSize := 0;

  try
    CompressBuf(Src, SrcLen, Pointer(zLibBuf), zLibBufSize);
  except
    if zLibBuf <> nil then FreeMem(zLibBuf);
    zLibBufSize := 0;
  end;

  if zLibBufSize > 0 then
  begin
    Assert(GetEncodeSize(zLibBufSize) <= DestLen, 'zLibEncodeBuffer压缩编码目标长度不够，DestLen:' + IntToStr(DestLen));

    Result := Encode6BitBuf(zLibBuf, Dest, zLibBufSize, DestLen);
    FreeMem(zLibBuf);
  end;
end;

function zLibDecodeString(const S: string): string;
var
  DecodeBuf: PChar;
  DecodeBufSize: Integer;

  zLibBuf: PChar;
  zLibBufSize: Integer;
begin
  Result := '';
  if Length(S) = 0 then Exit;
  
  DecodeBufSize := GetDecodeSize(Length(S));
  GetMem(DecodeBuf, DecodeBufSize);
  try
    Decode6BitBuf(@S[1], DecodeBuf, Length(S), DecodeBufSize);

    try
      DecompressBuf(DecodeBuf, DecodeBufSize, 0, Pointer(zLibBuf), zLibBufSize);
    except
      if zLibBuf <> nil then FreeMem(zLibBuf);
      zLibBufSize := 0;
    end;

    if zLibBufSize > 0 then
    begin
      SetLength(Result, zLibBufSize);
      Move(zLibBuf^, Result[1], zLibBufSize);
      FreeMem(zLibBuf);
    end;
  finally
    FreeMem(DecodeBuf, DecodeBufSize);
  end;
end;

function zLibDecodeString(const S: string; Dest: PChar; DestLen: Integer): Integer;
var
  DecodeBuf: PChar;
  DecodeBufSize: Integer;

  zLibBuf: PChar;
  zLibBufSize: Integer;
begin
  Result := 0;
  if Length(S) = 0 then Exit;

  DecodeBufSize := GetDecodeSize(Length(S));
  GetMem(DecodeBuf, DecodeBufSize);
  try
    Decode6BitBuf(@S[1], DecodeBuf, Length(S), DecodeBufSize);

    try
      DecompressBuf(DecodeBuf, DecodeBufSize, 0, Pointer(zLibBuf), zLibBufSize);
    except
      if zLibBuf <> nil then FreeMem(zLibBuf);
      zLibBufSize := 0;
    end;

    if zLibBufSize > 0 then
    begin
      Assert(zLibBufSize <= DestLen, 'zLibDecodeString压缩解码目标长度不够，DestLen:' + IntToStr(DestLen));
      Move(zLibBuf^, Dest^, zLibBufSize);
      Result := zLibBufSize;
      FreeMem(zLibBuf);
    end;
  finally
    FreeMem(DecodeBuf, DecodeBufSize);
  end;
end;

function zLibDecodeBuffer(Src: PChar; SrcLen: Integer): string;
var
  DecodeBuf: PChar;
  DecodeBufSize: Integer;

  zLibBuf: PChar;
  zLibBufSize: Integer;
begin
  Result := '';
  if SrcLen = 0 then Exit;

  DecodeBufSize := GetDecodeSize(SrcLen);
  GetMem(DecodeBuf, DecodeBufSize);
  try
    Decode6BitBuf(Src, DecodeBuf, SrcLen, DecodeBufSize);

    try
      DecompressBuf(DecodeBuf, DecodeBufSize, 0, Pointer(zLibBuf), zLibBufSize);
    except
      if zLibBuf <> nil then FreeMem(zLibBuf);
      zLibBufSize := 0;
    end;

    if zLibBufSize > 0 then
    begin
      SetLength(Result, zLibBufSize);
      Move(zLibBuf^, Result[1], zLibBufSize);
      FreeMem(zLibBuf);
    end;
  finally
    FreeMem(DecodeBuf, DecodeBufSize);
  end;
end;

function zLibDecodeBuffer(Src: PChar; SrcLen: Integer; Dest: PChar; DestLen: Integer): Integer;
var
  DecodeBuf: PChar;
  DecodeBufSize: Integer;

  zLibBuf: PChar;
  zLibBufSize: Integer;
begin
  Result := 0;
  if SrcLen = 0 then Exit;

  DecodeBufSize := GetDecodeSize(SrcLen);
  GetMem(DecodeBuf, DecodeBufSize);
  try
    Decode6BitBuf(Src, DecodeBuf, SrcLen, DecodeBufSize);

    try
      DecompressBuf(DecodeBuf, DecodeBufSize, 0, Pointer(zLibBuf), zLibBufSize);
    except
      if zLibBuf <> nil then FreeMem(zLibBuf);
      zLibBufSize := 0;
    end;

    if zLibBufSize > 0 then
    begin
      Assert(zLibBufSize <= DestLen, 'zLibDecodeBuffer压缩解码目标长度不够，DestLen:' + IntToStr(DestLen));
      Move(zLibBuf^, Dest^, zLibBufSize);
      Result := zLibBufSize;
      FreeMem(zLibBuf);
    end;
  finally
    FreeMem(DecodeBuf, DecodeBufSize);
  end;
end;

//--------------------------------------------------------------------------------

function zLibCompressString(const S: string): string;
var
  zLibBuf: PChar;
  zLibBufSize: Integer;
begin
  Result := '';
  if Length(S) = 0 then Exit;

  zLibBuf := nil;
  zLibBufSize := 0;

  try
    CompressBuf(@S[1], Length(S), Pointer(zLibBuf), zLibBufSize);
  except
    if zLibBuf <> nil then FreeMem(zLibBuf);
    zLibBufSize := 0;
  end;

  if zLibBufSize > 0 then
  begin
    SetLength(Result, zLibBufSize);
    Move(zLibBuf^, Result[1], zLibBufSize);
    FreeMem(zLibBuf);
  end;
end;

function zLibCompressString(const S: string; Dest: PChar; DestLen: Integer): Integer;
var
  zLibBuf: PChar;
  zLibBufSize: Integer;
begin
  Result := 0;
  if Length(S) = 0 then Exit;

  zLibBuf := nil;
  zLibBufSize := 0;

  try
    CompressBuf(@S[1], Length(S), Pointer(zLibBuf), zLibBufSize);
  except
    if zLibBuf <> nil then FreeMem(zLibBuf);
    zLibBufSize := 0;
  end;

  if zLibBufSize > 0 then
  begin
    if DestLen >= zLibBufSize then
    begin
      Move(zLibBuf^, Dest^, zLibBufSize);
      Result := zLibBufSize;
    end;
    
    FreeMem(zLibBuf);
  end;
end;

function zLibCompressBuffer(Src: PChar; SrcLen: Integer): string;
var
  zLibBuf: PChar;
  zLibBufSize: Integer;
begin
  Result := '';
  if SrcLen = 0 then Exit;

  zLibBuf := nil;
  zLibBufSize := 0;

  try
    CompressBuf(Src, SrcLen, Pointer(zLibBuf), zLibBufSize);
  except
    if zLibBuf <> nil then FreeMem(zLibBuf);
    zLibBufSize := 0;
  end;

  if zLibBufSize > 0 then
  begin
    SetLength(Result, zLibBufSize);
    Move(zLibBuf^, Result[1], zLibBufSize);
    FreeMem(zLibBuf);
  end;
end;

function zLibCompressBuffer(Src: PChar; SrcLen: Integer; Dest: PChar; DestLen: Integer): Integer;
var
  zLibBuf: PChar;
  zLibBufSize: Integer;
begin
  Result := 0;
  if SrcLen = 0 then Exit;

  zLibBuf := nil;
  zLibBufSize := 0;

  try
    CompressBuf(Src, SrcLen, Pointer(zLibBuf), zLibBufSize);
  except
    if zLibBuf <> nil then FreeMem(zLibBuf);
    zLibBufSize := 0;
  end;

  if zLibBufSize > 0 then
  begin
    if DestLen >= zLibBufSize then
    begin
      Move(zLibBuf^, Dest^, zLibBufSize);
      Result := zLibBufSize;
    end;
    FreeMem(zLibBuf);
  end;
end;

function zLibDecompressString(const S: string): string;
var
  zLibBuf: PChar;
  zLibBufSize: Integer;
begin
  Result := '';
  if Length(S) = 0 then Exit;

  try
    DecompressBuf(@S[1], Length(S), 0, Pointer(zLibBuf), zLibBufSize);
  except
    if zLibBuf <> nil then FreeMem(zLibBuf);
    zLibBufSize := 0;
  end;

  if zLibBufSize > 0 then
  begin
    SetLength(Result, zLibBufSize);
    Move(zLibBuf^, Result[1], zLibBufSize);
    FreeMem(zLibBuf);
  end;
end;

function zLibDecompressString(const S: string; Dest: PChar; DestLen: Integer): Integer;
var
  zLibBuf: PChar;
  zLibBufSize: Integer;
begin
  Result := 0;
  if Length(S) = 0 then Exit;

  try
    DecompressBuf(@S[1], Length(S), 0, Pointer(zLibBuf), zLibBufSize);
  except
    if zLibBuf <> nil then FreeMem(zLibBuf);
    zLibBufSize := 0;
  end;

  if zLibBufSize > 0 then
  begin
    if DestLen >= zLibBufSize then
    begin
      Move(zLibBuf^, Dest^, zLibBufSize);
      Result := zLibBufSize;
    end;
    
    FreeMem(zLibBuf);
  end;
end;

function zLibDecompressBuffer(Src: PChar; SrcLen: Integer): string;
var
  zLibBuf: PChar;
  zLibBufSize: Integer;
begin
  Result := '';
  if SrcLen = 0 then Exit;

  try
    DecompressBuf(Src, SrcLen, 0, Pointer(zLibBuf), zLibBufSize);
  except
    if zLibBuf <> nil then FreeMem(zLibBuf);
    zLibBufSize := 0;
  end;

  if zLibBufSize > 0 then
  begin
    SetLength(Result, zLibBufSize);
    Move(zLibBuf^, Result[1], zLibBufSize);
    FreeMem(zLibBuf);
  end;
end;

function zLibDecompressBuffer(Src: PChar; SrcLen: Integer; Dest: PChar; DestLen: Integer): Integer;
var
  zLibBuf: PChar;
  zLibBufSize: Integer;
begin
  Result := 0;
  if SrcLen = 0 then Exit;

  try
    DecompressBuf(Src, SrcLen, 0, Pointer(zLibBuf), zLibBufSize);
  except
    if zLibBuf <> nil then FreeMem(zLibBuf);
    zLibBufSize := 0;
  end;

  if zLibBufSize > 0 then
  begin
    if DestLen >= zLibBufSize then
    begin
      Move(zLibBuf^, Dest^, zLibBufSize);
      Result := zLibBufSize;
    end;
    FreeMem(zLibBuf);
  end;
end;

end.
