unit DesUnit;

interface
uses
  Classes, SysUtils
  {$IFDEF CPUX64},VMProtectSDK{$ENDIF};

function EncryptDes(var Data; DataSize: Integer; const Key: string): Integer;
function DecryptDes(var Data; DataSize: Integer; const Key: string): Integer;

function EncryptStrDes(const Source, Key: string): string;
function DecryptStrDes(const Source, Key: string): string;

function DecryStringHex(StrHex, Key: string): string;
function EncryStringHex(StrHex, Key: string): string;
{procedure DES(var Data; const Keys; cLoop: Integer);
// Data: 待加密数据，64字节，每字节代表一位；
// Keys:密钥数据，48*CLoop字节，每字节代表一位，一次迭代用48字节(48位)
// cLoop:迭代次数，标准DES为16。
procedure UNDES(var Data; const Keys; cLoop: Integer);
// Data: 待解密数据，64字节，每字节代表一位；
// Keys:密钥数据，48*CLoop字节，每字节代表一位，一次迭代用48字节(48位)
// cLoop:迭代次数，标准DES为16。
procedure SetBits(const Source; var Dest; cByte: Integer);
// 把每字节1位的数据转换为每字节8位的数据，cByte为转换后的字节数
procedure GetBits(const Source; var Dest; cByte: Integer);
// 把每字节8位的数据转换为每字节1位的数据，cByte为转换前字节数。
procedure MakeDesKeyData(const Key; var KeyData);}
// 用DES标准密钥生成方法生成密钥数据。
// Key为密钥，8字节，每字节使用低7位，共使用56位。
// Dest为密钥数据，48*16字节，每字节保存1位。

{
加密：
生成48*16字节密钥数据(KeyData)
以8字节为一组，对每一组：
GetBits(当前组， Data, 8);
Des(Data, KeyData, 16);
SetBits(Data, 当前组, 8);
直到处理完毕

解密：
生成48*16字节密钥数据(KeyData)// 与加密时要相同
以8字节为一组，对每一组：
GetBits(当前组， Data, 8);
UnDes(Data, KeyData, 16);
SetBits(Data, 当前组, 8);
直到处理完毕

}
implementation
uses Windows, Variants, HashUnit;
type
  TBits64 = packed array[0..63] of Byte;
  TBits4 = packed array[0..3] of Byte;
  TBits6 = packed array[0..3] of Byte;
  TBits48 = packed array[0..48] of Byte;

  TByteAry = array[0..32767] of Byte;
  PByteAry = ^TByteAry;
  TByteVarAry = array of Byte;
  PByteVarAry = ^TByteVarAry;

const

  tblIP: packed array[0..63] of Byte =
  (58, 50, 42, 34, 26, 18, 10, 2,
    60, 52, 44, 36, 28, 20, 12, 4,
    62, 54, 46, 38, 30, 22, 14, 6,
    64, 56, 48, 40, 32, 24, 16, 8,
    57, 49, 41, 33, 25, 17, 9, 1,
    59, 51, 43, 35, 27, 19, 11, 3,
    61, 53, 45, 37, 29, 21, 13, 5,
    63, 55, 47, 39, 31, 23, 15, 7);

  tblUnIP: packed array[0..63] of Byte =
  (40, 8, 48, 16, 56, 24, 64, 32,
    39, 7, 47, 15, 55, 23, 63, 31,
    38, 6, 46, 14, 54, 22, 62, 30,
    37, 5, 45, 13, 53, 21, 61, 29,
    36, 4, 44, 12, 52, 20, 60, 28,
    35, 3, 43, 11, 51, 19, 59, 27,
    34, 2, 42, 10, 50, 18, 58, 26,
    33, 1, 41, 9, 49, 17, 57, 25);

  tblE: packed array[0..47] of Byte =
  (32, 1, 2, 3, 4, 5,
    4, 5, 6, 7, 8, 9,
    8, 9, 10, 11, 12, 13,
    12, 13, 14, 15, 16, 17,
    16, 17, 18, 19, 20, 21,
    20, 21, 22, 23, 24, 25,
    24, 25, 26, 27, 28, 29,
    28, 29, 30, 31, 32, 1);

  tblS: packed array[0..7, 0..3, 0..15] of Byte =
  (((14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7),
    (0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8),
    (4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0),
    (15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13)),
    ((15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10),
    (3, 13, 4, 7, 15, 2, 8, 15, 12, 0, 1, 10, 6, 9, 11, 5),
    (0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15),
    (13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9)),
    ((10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8),
    (13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1),
    (13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7),
    (1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12)),
    ((7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15),
    (13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9),
    (10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4),
    (3, 15, 0, 6, 10, 10, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14)),
    ((2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9),
    (14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6),
    (4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14),
    (11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3)),
    ((12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11),
    (10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8),
    (9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6),
    (4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13)),
    ((4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1),
    (13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6),
    (1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2),
    (6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12)),
    ((13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7),
    (1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2),
    (7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8),
    (2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11)));

  tblP: packed array[0..31] of Byte =
  (16, 7, 20, 21,
    29, 12, 28, 17,
    1, 15, 23, 26,
    5, 18, 31, 10,
    2, 8, 24, 14,
    32, 27, 3, 9,
    19, 13, 30, 6,
    22, 11, 4, 25);

function FindByteBack(Pattern: Byte; const Data; DataSize: Integer): Integer; assembler;
asm
    PUSH EDI;
    LEA EDI, [EDX + ECX - 1];
    STD;
    CMP ECX, 1;
    REPNE SCASB;
    MOV EAX, ECX;
    CLD;
    JE @FOUND;
    MOV EAX, -1;
    @FOUND:
    POP EDI;
end;

procedure DES(var Data; const Keys; cLoop: Integer);
var
  Bits: packed array[0..1, 0..1, 0..31] of Byte; // 0..127
  Bit48s: packed array[0..47] of Byte; // 128..175
  KeysAddr: Cardinal; // 176..179
  LoopCount: Cardinal; // 180..183
  DataAddr: Cardinal; // 184..187
  Current: Cardinal; // 188..191
  Next: Cardinal; // 192..195
asm
  PUSHAD;
  MOV DWORD PTR KeysAddr, EDX; // KeysAddr
  MOV DWORD PTR LoopCount, ECX; // LoopCount
  MOV DWORD PTR DataAddr, EAX; // DataAddr
// IP
  XOR EDI, EDI;
  LEA EBX, tblIP;
  LEA ESI, BITS;
  XOR ECX, ECX;
  @IP1:
  MOV CL, [EBX][EDI];
  DEC CL;
  MOV DL, [EAX][ECX];
  MOV [ESI][EDI],DL;
  INC EDI;
  CMP EDI, 64;
  JL @IP1;

  MOV DWORD PTR Current, 64; // Current
  MOV DWORD PTR Next, 0; // Next
// for i := 1 to cLoop do
// begin
  MOV ECX, LoopCount;// Loop;
  @LOOPKEY1:
  PUSH ECX;
  XOR DWORD PTR Current, 64;// Current
  XOR DWORD PTR Next, 64; // Next

// L(i)= R(i-1)
  MOV ECX, 8;
  LEA ESI, BITS
  MOV EDI, ESI;
  ADD ESI, Current;// Current
  ADD EDI, Next;// Next
  ADD ESI, 32;
  REP MOVSD;

// R(i-1) => 48
  LEA ESI, Bit48s;
  LEA EBX, tblE;
  LEA EAX, Bits;
  ADD EAX, Current;
  ADD EAX, 32;
  XOR EDI, EDI;
  XOR ECX, ECX;
  @EXT1:
  MOV CL, [EBX][EDI];
  DEC CL;
  MOV DL, [EAX][ECX];
  MOV [ESI][EDI],DL;
  INC EDI;
  CMP EDI, 48;
  JL @EXT1;

// R(i-1) ^ Key(i-1)
  XOR EDI, EDI;
  MOV EBX, KeysAddr;
  @USEK1:
  MOV EAX, [EBX][EDI];
  XOR [ESI][EDI], EAX;
  ADD EDI, 4;
  CMP EDI, 48;
  JL @USEK1;
  ADD EBX, 48;
  MOV KeysAddr, EBX;

// S(Ri-1)
  LEA EDI, BITS;
  ADD EDI, Current;
  ADD EDI, 32;
  LEA EBX, tblS;
  MOV ECX,4
  @S1:
  XOR EDX, EDX;
  XOR EAX, EAX;
  MOV DL, [ESI];
  SHL DL, 1;
  OR DL, [ESI][5];
  SHL EDX, CL;
  MOV AL,[ESI][1];
  SHL AL, 1;
  OR AL, [ESI][2];
  SHL AL, 1;
  OR AL, [ESI][3];
  SHL AL, 1;
  OR AL, [ESI][4];
  ADD EDX, EAX;
  XOR EAX,EAX;
  MOV AL, [EBX][EDX]
  ROR EAX, CL;
  ROL EAX, 1;
  MOV [EDI], AL;
  ROL EAX, 1;
  AND AL, 1;
  MOV [EDI][1], AL;
  ROL EAX, 1;
  AND EAX, 1;
  MOV [EDI][2], AL;
  ROL EAX, 1;
  AND AL,1;
  MOV [EDI][3], AL;
  ADD ESI, 6;
  ADD EDI, 4;
  INC CH;
  CMP CH, 8;
  JL @S1;

// P(Ri)
  LEA EBX, tblP;
  LEA ESI, BITS;
  ADD ESI, 32;
  MOV EDI, ESI;
  ADD ESI, CURRENT;
  ADD EDI, NEXT;
  XOR EAX,EAX;
  XOR ECX,ECX;
  @P1:
  MOV AL, [EBX][ECX];
  DEC AL
  MOV DL, [ESI][EAX];
  MOV [EDI][ECX],DL;
  INC ECX;
  CMP ECX, 32
  JL @P1;

// R(i) = Li-1 ^ Ri-1
  XOR ECX, ECX
  SUB ESI, 32;
  @L1:
  MOV EAX, [ESI][ECX];
  XOR [EDI][ECX], EAX;
  ADD ECX, 4;
  CMP ECX, 32;
  JL @L1;
// end;
  POP ECX;
// LOOP @LOOPKEY1;
  DEC ECX;
  JNZ @LOOPKEY1;

// Left = R16
  LEA ESI, BITS;
  MOV EDI, ESI;
  ADD ESI, NEXT;
  ADD EDI, CURRENT;
  ADD ESI, 32
  MOV ECX, 8;
  REP MOVSD;
// R=L16
  LEA ESI, BITS;
  MOV EDI, ESI;
  ADD ESI, NEXT;
  ADD EDI, CURRENT;
  ADD EDI, 32
  MOV ECX, 8;
  REP MOVSD;

  XOR NEXT, 64;

// UnIP
  LEA ESI, BITS;
  ADD ESI, NEXT;
  LEA EBX, tblUnIP;
  MOV EDI, DataAddr;
  XOR EAX, EAX;
  XOR ECX, ECX;
  @UNIP1:
  MOV AL, [EBX][ECX];
  DEC AL;
  MOV DL, [ESI][EAX];
  MOV [EDI][ECX], DL;
  INC ECX;
  CMP ECX, 64;
  JL @UNIP1;

  POPAD;
end;

procedure UNDES(var Data; const Keys; cLoop: Integer);
var
  Bits: packed array[0..1, 0..1, 0..31] of Byte; // 0..127
  Bit48s: packed array[0..47] of Byte; // 128..175
  KeysAddr: Cardinal; // 176..179
  LoopCount: Cardinal; // 180..183
  DataAddr: Cardinal; // 184..187
  Current: Cardinal; // 188..191
  Next: Cardinal; // 192..195
asm
  PUSHAD;
  MOV DWORD PTR LoopCount, ECX; // LoopCount
  IMUL ECX, 48;
  ADD EDX, ECX;
  MOV DWORD PTR KeysAddr, EDX; // KeysAddr
  MOV DWORD PTR DataAddr, EAX; // DataAddr
// IP
  XOR EDI, EDI;
  LEA EBX, tblIP;
  LEA ESI, BITS
  XOR ECX, ECX;
  @IP1:
  MOV CL, [EBX][EDI];
  DEC CL;
  MOV DL, [EAX][ECX];
  MOV [ESI][EDI],DL;
  INC EDI;
  CMP EDI, 64;
  JL @IP1;

// R16= Left
  LEA ESI, BITS;
  MOV EDI, ESI;
  ADD EDI, 64;
  ADD ESI, 32
  MOV ECX, 8;
  REP MOVSD;
// L16=Right
  LEA ESI, BITS;
  MOV EDI, ESI;
  ADD EDI, 64;
  ADD EDI, 32
  MOV ECX, 8;
  REP MOVSD;

  MOV DWORD PTR Current, 0; // Current
  MOV DWORD PTR Next, 64; // Next
// for i := 16 downto 1 do
// begin
  MOV ECX, LoopCount;// Loop;
  @Loop1:
  PUSH ECX;
  XOR DWORD PTR Current, 64;// Current
  XOR DWORD PTR Next, 64; // Next

// R(i-1) = L(i)
  MOV ECX, 8;
  LEA ESI, BITS;
  MOV EDI, ESI;
  ADD ESI, Current;// Current
  ADD EDI, Next;// Next
  ADD EDI, 32;
  REP MOVSD;

// 48b(Li)
  LEA ESI, Bit48s;
  LEA EBX, tblE;
  LEA EAX, Bits;
  ADD EAX, Current;
  XOR EDI, EDI;
  XOR ECX, ECX;
  @EXT1:
  MOV CL, [EBX][EDI];
  DEC CL;
  MOV DL, [EAX][ECX];
  MOV [ESI][EDI],DL;
  INC EDI;
  CMP EDI, 48;
  JL @EXT1;

// Li ^ Key i-1
  XOR EDI, EDI;
  MOV EBX, KeysAddr;
  SUB EBX, 48;
  @USEK1:
  MOV EAX, [EBX][EDI];
  XOR [ESI][EDI], EAX;
  ADD EDI, 4;
  CMP EDI, 48;
  JL @USEK1;
  MOV KeysAddr, EBX;

// S(Li)
  LEA EDI, BITS;
  ADD EDI, Current;
  LEA EBX, tblS;
  MOV ECX,4
  @S1:
  XOR EDX, EDX;
  XOR EAX, EAX;
  MOV DL, [ESI];
  SHL DL, 1;
  OR DL, [ESI][5];
  SHL EDX, CL;
  MOV AL,[ESI][1];
  SHL AL, 1;
  OR AL, [ESI][2];
  SHL AL, 1;
  OR AL, [ESI][3];
  SHL AL, 1;
  OR AL, [ESI][4];
  ADD EDX, EAX;
  XOR EAX,EAX;
  MOV AL, [EBX][EDX]
  ROR EAX, CL;
  ROL EAX, 1;
  MOV [EDI], AL;
  ROL EAX, 1;
  AND AL, 1;
  MOV [EDI][1], AL;
  ROL EAX, 1;
  AND EAX, 1;
  MOV [EDI][2], AL;
  ROL EAX, 1;
  AND AL,1;
  MOV [EDI][3], AL;
  ADD ESI, 6;
  ADD EDI, 4;
  INC CH;
  CMP CH, 8;
  JL @S1;

// P(Li)
  LEA EBX, tblP;
  LEA ESI, BITS;
  MOV EDI, ESI;
  ADD ESI, CURRENT;
  ADD EDI, NEXT;
  XOR EAX,EAX;
  XOR ECX,ECX;
  @P1:
  MOV AL, [EBX][ECX];
  DEC AL;
  MOV DL, [ESI][EAX];
  MOV [EDI][ECX],DL;
  INC ECX;
  CMP ECX, 32
  JL @P1;

// L(i-1) = R(i) ^ Li
  XOR ECX, ECX
  ADD ESI, 32;
  @L1:
  MOV EAX, [ESI][ECX];
  XOR [EDI][ECX], EAX;
  ADD ECX, 4;
  CMP ECX, 32;
  JL @L1;
// end;
  POP ECX;
  DEC ECX;
  JNZ @LOOP1;


// UnIP
  LEA ESI, BITS;
  ADD ESI, NEXT;
  LEA EBX, tblUnIP;
  MOV EDI, DataAddr;
  XOR EAX, EAX;
  XOR ECX, ECX;
  @UNIP1:
  MOV AL, [EBX][ECX];
  DEC AL;
  MOV DL, [ESI][EAX];
  MOV [EDI][ECX], DL;
  INC ECX;
  CMP ECX, 64;
  JL @UNIP1;

  POPAD;
end;

procedure GetBits(const Source; var Dest; cByte: Integer);
asm
  PUSH EBX;
  PUSH EDI;
  PUSH ESI;

  XOR EDI, EDI;
  XOR ESI, ESI;
  XOR EBX,EBX
  @LOOP1:
  MOV BL, [EAX][ESI];
  ROR EBX,7;
  AND BL,1;
  MOV [EDX][EDI], BL;
  ROL EBX,1;
  AND BL,1;
  MOV [EDX][EDI+1], BL;
  ROL EBX,1;
  AND BL,1;
  MOV [EDX][EDI+2], BL;
  ROL EBX,1;
  AND BL,1;
  MOV [EDX][EDI+3], BL;
  ROL EBX,1;
  AND BL,1;
  MOV [EDX][EDI+4], BL;
  ROL EBX,1;
  AND BL,1;
  MOV [EDX][EDI+5], BL;
  ROL EBX,1;
  AND BL,1;
  MOV [EDX][EDI+6], BL;
  ROL EBX,1;
  AND BL,1;
  MOV [EDX][EDI+7], BL;
  ADD EDI, 8;
  INC ESI;
  LOOP @LOOP1
  POP ESI;
  POP EDI;
  POP EBX;
end;

// ///////////////////////////////////////////////////////////////////////////////////////////////////
// 续上:

procedure SetBits(const Source; var Dest; cByte: Integer);
asm
  PUSH EBX;
  PUSH EDI;
  PUSH ESI;

  XOR EDI, EDI;
  XOR ESI, ESI;
  @LOOP1:
  MOV BL, [EAX][ESI];
  SHL BL, 1;
  OR BL, [EAX][ESI + 1];
  SHL BL, 1;
  OR BL, [EAX][ESI + 2];
  SHL BL, 1;
  OR BL, [EAX][ESI + 3];
  SHL BL, 1;
  OR BL, [EAX][ESI + 4];
  SHL BL, 1;
  OR BL, [EAX][ESI + 5];
  SHL BL, 1;
  OR BL, [EAX][ESI + 6];
  SHL BL, 1;
  OR BL, [EAX][ESI + 7];
  MOV [EDX][EDI], BL
  ADD ESI, 8;
  INC EDI;
  LOOP @LOOP1

  POP ESI;
  POP EDI;
  POP EBX;
end;

procedure MakeDesKeyData(const Key; var KeyData); assembler;
const
  Idx56In64: packed array[0..55] of Byte =
  (57, 49, 41, 33, 25, 17, 9,
    1, 58, 50, 42, 34, 26, 18,
    10, 2, 59, 51, 43, 35, 27,
    19, 11, 3, 60, 52, 44, 36,
    63, 55, 47, 39, 31, 23, 15,
    7, 62, 54, 46, 38, 30, 22,
    14, 6, 61, 53, 45, 37, 29,
    21, 13, 5, 28, 20, 12, 4);
  Idx48In56: packed array[0..47] of Byte =
  (14, 17, 11, 24, 1, 5,
    3, 28, 15, 6, 21, 10,
    23, 19, 12, 4, 26, 8,
    16, 7, 27, 20, 13, 2,
    41, 52, 31, 37, 47, 55,
    30, 40, 51, 45, 33, 48,
    44, 49, 39, 56, 34, 53,
    46, 42, 50, 36, 29, 32);
  KeyShift: packed array[0..15] of Integer =
  (1, 1, 2, 2,
    2, 2, 2, 2,
    1, 2, 2, 2,
    2, 2, 2, 1);
var
  Key64: packed array[0..63] of Byte;
  Key56: packed array[0..55] of Byte;
  KeyTemp: packed array[0..55] of Byte;
  KeyDataAdr: Cardinal;
asm
// Get64Bits
  PUSH ESI;
  PUSH EDI;
  PUSH EBX;
  MOV KeyDataAdr, EDX;
  LEA EDX, Key64;
  MOV ECX, 8;
  CALL GetBits;
// 64 Bits ==> 56 Bits
  LEA EDI, Key56;
  LEA ESI, Key64;
  LEA EBX, Idx56In64;
  XOR EAX, EAX;
  XOR ECX, ECX;
  @1:
  MOV AL, [EBX][ESI];
  DEC AL;
  MOV CL, [ESI][EAX];
  MOV [EDI][ESI], CL;
  INC CH;
  CMP CH, 56;
  JL @1;

  XOR EDX,EDX;
  @2:
// L <<< move[i];
  LEA ESI,Key56;
  LEA EDI,KeyTemp;
  XOR ECX,ECX;
  LEA EBX,KeyShift;
  MOV BYTE PTR CL, [EDX][EBX];
  REP MOVSB;

  LEA EDI, Key56;
  MOV ESI, EDI;
  MOV CL, [EBX][EDX];
  ADD ESI, ECX;
  MOV ECX, 28;
  SUB CL, [EBX][EDX];
  REP MOVSB;

  LEA ESI, KeyTemp;
  MOV CL, [EBX][EDX];
  REP MOVSB;

// R <<< move[i];
  LEA ESI,Key56[28];
  LEA EDI,KeyTemp;
  XOR ECX,ECX;
  LEA EBX,KeyShift;
  MOV BYTE PTR CL, [EDX][EBX];
  REP MOVSB;

  LEA EDI, Key56[28];
  MOV ESI, EDI;
  MOV CL, [EBX][EDX];
  ADD ESI, ECX;
  MOV ECX, 28;
  SUB CL, [EBX][EDX];
  REP MOVSB;

  LEA ESI, KeyTemp;
  MOV CL, [EBX][EDX];
  REP MOVSB;


// Get48Bits
  MOV EAX, EDX;
  IMUL EAX, 48;
  MOV EDI, KeyDataAdr;
  ADD EDI, EAX;
  LEA ESI, Key56;
  LEA EBX, Idx48In56;
  XOR EAX, EAX;
  XOR ECX, ECX;
  @3:
  MOV AL, [EBX][ESI];
  DEC AL;
  MOV CL, [ESI][EAX];
  MOV [EDI][ESI], CL;
  INC CH;
  CMP CH, 56;
  JL @3;

  INC EDX;
  CMP EDX, 16;
  JL @2;
end;

function EncryptDes(var Data; DataSize: Integer; const Key: string): Integer;
var
  I: Integer;
  PackedKeyData: array[0..95] of Byte;
  KeyData: array[0..15] of array[0..47] of Byte;

  DataBits: array[0..63] of Byte;
  Level: Integer;
  ptrData: PByteArray;
  OldDataSize: Integer;
begin
  FillChar(PackedKeyData, SizeOf(PackedKeyData), 0);
  FillChar(KeyData, SizeOf(KeyData), 0);
  Hash(Key, PackedKeyData);

  Move(PackedKeyData, PackedKeyData[20], 20);
  Move(PackedKeyData, PackedKeyData[40], 20);
  Move(PackedKeyData, PackedKeyData[60], 20);
  Move(PackedKeyData, PackedKeyData[80], 16);
  for I := 0 to 15 do
    GetBits(PackedKeyData[I * 6], KeyData[I], 6);

  Level := 8;
  OldDataSize := DataSize;
  DataSize := (DataSize div 8 + 1) * 8;
  ptrData := @Data;
  PtrData[OldDataSize] := $FF;
  for I := OldDataSize + 1 to DataSize - 1 do
    PtrData[I] := Random(255);

  I := 0;
  while (I < DataSize) do
  begin
    GetBits(ptrData^[I], DataBits, 8);
    DES(DataBits, KeyData[0], Level);
    SetBits(DataBits, ptrData^[I], 8);
    Inc(I, 8);
  end;
  Result := DataSize;
end;

function DecryptDes(var Data; DataSize: Integer; const Key: string): Integer;
var
  I: Integer;
  PackedKeyData: array[0..95] of Byte;
  KeyData: array[0..15] of array[0..47] of Byte;

  DataBits: array[0..63] of Byte;
  Level: Integer;
  ptrData: PByteArray;
begin
  FillChar(PackedKeyData, SizeOf(PackedKeyData), 0);
  FillChar(KeyData, SizeOf(KeyData), 0);
  Hash(Key, PackedKeyData);

  Move(PackedKeyData, PackedKeyData[20], 20);
  Move(PackedKeyData, PackedKeyData[40], 20);
  Move(PackedKeyData, PackedKeyData[60], 20);
  Move(PackedKeyData, PackedKeyData[80], 16);
  for I := 0 to 15 do
    GetBits(PackedKeyData[I * 6], KeyData[I], 6);

  Level := 8;

  if DataSize mod 8 <> 0 then
  begin
    DataSize := (DataSize div 8) * 8;
  end;
  ptrData := @Data;
  I := 0;
  while (I < DataSize) do
  begin
    GetBits(ptrData^[I], DataBits, 8);
    UNDES(DataBits, KeyData[0], Level);
    SetBits(DataBits, ptrData^[I], 8);
    Inc(I, 8);
  end;
  Result := FindByteBack($FF, Data, DataSize);
end;

function EncryptStrDes(const Source, Key: string): string;
var
  Temp: string;
  DataSize: Integer;
begin
  if Source <> '' then begin
    Temp := Source;
    DataSize := Length(Temp);
    SetLength(Temp, ((DataSize + 1) div 8 + 1) * 8);
    DataSize := EncryptDes(Temp[1], DataSize, Key);
    if DataSize > 0 then begin
      SetLength(Temp, DataSize);
      Result := Temp;
    end else Result := '';
  end else Result := '';
end;

function DecryptStrDes(const Source, Key: string): string;
var
  Temp: string;
  DataSize: Integer;
begin
  if Source <> '' then begin
    Temp := Source;
    DataSize := Length(Temp);
    SetLength(Temp, DataSize);
    DataSize := DecryptDes(Temp[1], DataSize, Key);
    if DataSize > 0 then begin
      SetLength(Temp, DataSize);
      Result := Temp;
    end else Result := '';
  end else Result := '';
end;

function DecryStringHex(StrHex, Key: string): string;
  function HexToInt(Hex: string): Integer;
  var
    I, Res: Integer;
    Ch: Char;
  begin
    Res := 0;
    for I := 0 to Length(Hex) - 1 do begin
      Ch := Hex[I + 1];
      if (Ch >= '0') and (Ch <= '9') then
        Res := Res * 16 + Ord(Ch) - Ord('0')
      else if (Ch >= 'A') and (Ch <= 'F') then
        Res := Res * 16 + Ord(Ch) - Ord('A') + 10
      else if (Ch >= 'a') and (Ch <= 'f') then
        Res := Res * 16 + Ord(Ch) - Ord('a') + 10
     // else raise Exception.Create('Error: not a Hex String');
    end;
    Result := Res;
  end;

var
  Str, temp: string;
  I: Integer;
begin
  Str := '';
  for I := 0 to Length(StrHex) div 2 - 1 do begin
    temp := Copy(StrHex, I * 2 + 1, 2);
    Str := Str + Chr(HexToInt(temp));
  end;
  try
    Result := DecryptStrDes(Str, Key);
  except
    Result := '';
  end;
end;

function EncryStringHex(StrHex, Key: string): string;
var
  StrResult, TempResult, temp: string;
  I: Integer;
begin
  try
    TempResult := EncryptStrDes(StrHex, Key);
  except
    TempResult := '';
  end;

  StrResult := '';
  for I := 0 to Length(TempResult) - 1 do begin
    temp := Format('%x', [Ord(TempResult[I + 1])]);
    if Length(temp) = 1 then temp := '0' + temp;
    StrResult := StrResult + temp;
  end;
  Result := StrResult;
end;

end.

