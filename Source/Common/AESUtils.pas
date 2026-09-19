unit AESUtils;

(*
    该算法摘自Synopse framework - SynCrypto.pas
    (SynCrypto.pas, SynCommons.pas, SynLZ.pas, SynTable.pas, Synopse.inc, sha512-x86.obj)

    Synopse framework. Copyright (C) 2020 Arnaud Bouchez
    Synopse Informatique - https://synopse.info

    AES-CRT模式加解密 TAESCTR

    2020-03-29: 初始移值, D7下测试成功
*)


interface

{.$DEFINE USEAESNI}

type
{$IFDEF WIN32}
  PtrInt = Integer;
  PtrUInt = Cardinal;
{$ELSE}
  PtrInt = NativeInt;
  PtrUInt = NativeUInt;
{$ENDIF}

  PPtrUInt = ^PtrUInt;
  PPtrInt = ^PtrInt;

  PByteArray = ^TByteArray;
  TByteArray = array[0..MaxInt - 1] of Byte; // redefine here with {$R-}

  TCardinalArray = array[0..MaxInt div SizeOf(Cardinal)-1] of Cardinal;
  PCardinalArray = ^TCardinalArray;

const
  AES_MAX_ROUND = 14;             // AES加密算法最多14轮  128bit:10轮  192bit:12轮   256bit:14轮

type
  TAESKeySizeType = (ks_128bit, ks_192bit, ks_256bit);  // 16位密钥=128位，24位密钥=192位，32位密钥=256位

  TAESBlock = array[0..15] of Byte;
  TBlock128 = array[0..3] of Cardinal;
  PBlock128 = ^TBlock128;

  TKeyArray = packed array[0..AES_MAX_ROUND] of TAESBlock;
  TKeyArrayCardinal = packed array[0..4 * (AES_MAX_ROUND + 1) - 1] of Cardinal; // Key as array of Cardinal
  PKeyArrayCardinal = ^TKeyArrayCardinal;

  TAESContext = record
    KeyArr: TKeyArray;                              // Key (encr. or decr.)
    DoBlock: procedure(const ctxt, source, dest);   // main AES function
    Rounds: byte;                                   // Number of rounds
    KeyBits: word;                                  // Number of bits in key (128/192/256)
  end;
  PAESContext = ^TAESContext;


procedure AESEncrypt(
  KeyBuf: Pointer; KeyBufLen: Integer;
  InBuf, OutBuf: Pointer; BufLen: Integer;
  const KeySize: TAESKeySizeType = ks_128bit);

procedure AESDecrypt(
  KeyBuf: Pointer; KeyBufLen: Integer;
  InBuf, OutBuf: Pointer; BufLen: Integer;
  const KeySize: TAESKeySizeType = ks_128bit);

implementation

// AES computed tables
var
  SBox, InvSBox: array[Byte] of Byte;
  Td0, Td1, Td2, Td3, Te0, Te1, Te2, Te3: array[Byte] of Cardinal;

const
  RCon: array[0..9] of Cardinal = ($01, $02, $04, $08, $10, $20, $40, $80, $1B, $36);

{$IFDEF USEAESNI} // should be put outside the main method for FPC :(
{$IFDEF WIN32}
procedure AesNiEncryptXmm7_128;
asm // input: eax=TAESContext, xmm7=data; output: eax=TAESContext, xmm7=data
        movups  xmm0, [eax + 16 * 0]
        movups  xmm1, [eax + 16 * 1]
        movups  xmm2, [eax + 16 * 2]
        movups  xmm3, [eax + 16 * 3]
        movups  xmm4, [eax + 16 * 4]
        movups  xmm5, [eax + 16 * 5]
        movups  xmm6, [eax + 16 * 6]
        pxor    xmm7, xmm0
  {$IFDEF HASAESNI}
        aesenc  xmm7, xmm1
        aesenc  xmm7, xmm2
        aesenc  xmm7, xmm3
        aesenc  xmm7, xmm4
        aesenc  xmm7, xmm5
        aesenc  xmm7, xmm6
  {$ELSE}
        db      $66, $0F, $38, $DC, $F9
        db      $66, $0F, $38, $DC, $FA
        db      $66, $0F, $38, $DC, $FB
        db      $66, $0F, $38, $DC, $FC
        db      $66, $0F, $38, $DC, $FD
        db      $66, $0F, $38, $DC, $FE
  {$ENDIF}
        movups  xmm0, [eax + 16 * 7]
        movups  xmm1, [eax + 16 * 8]
        movups  xmm2, [eax + 16 * 9]
        movups  xmm3, [eax + 16 * 10]
  {$IFDEF HASAESNI}
        aesenc  xmm7, xmm0
        aesenc  xmm7, xmm1
        aesenc  xmm7, xmm2
        aesenclast xmm7, xmm3
  {$ELSE}
        db      $66, $0F, $38, $DC, $F8
        db      $66, $0F, $38, $DC, $F9
        db      $66, $0F, $38, $DC, $FA
        db      $66, $0F, $38, $DD, $FB
  {$ENDIF}
end;

procedure aesniencrypt128(const ctxt, source, dest);
asm // eax=ctxt edx=source ecx=dest
        movups  xmm7, [edx]
        call    AesNiEncryptXmm7_128
        movups  [ecx], xmm7
        pxor    xmm7, xmm7 // for safety
end;

procedure AesNiEncryptXmm7_192;
asm // input: eax=TAESContext, xmm7=data; output: eax=TAESContext, xmm7=data
        movups  xmm0, [eax + 16 * 0]
        movups  xmm1, [eax + 16 * 1]
        movups  xmm2, [eax + 16 * 2]
        movups  xmm3, [eax + 16 * 3]
        movups  xmm4, [eax + 16 * 4]
        movups  xmm5, [eax + 16 * 5]
        movups  xmm6, [eax + 16 * 6]
        pxor    xmm7, xmm0
  {$IFDEF HASAESNI}
        aesenc  xmm7, xmm1
        aesenc  xmm7, xmm2
        aesenc  xmm7, xmm3
        aesenc  xmm7, xmm4
        aesenc  xmm7, xmm5
        aesenc  xmm7, xmm6
  {$ELSE}
        db      $66, $0F, $38, $DC, $F9
        db      $66, $0F, $38, $DC, $FA
        db      $66, $0F, $38, $DC, $FB
        db      $66, $0F, $38, $DC, $FC
        db      $66, $0F, $38, $DC, $FD
        db      $66, $0F, $38, $DC, $FE
  {$ENDIF}
        movups  xmm0, [eax + 16 * 7]
        movups  xmm1, [eax + 16 * 8]
        movups  xmm2, [eax + 16 * 9]
        movups  xmm3, [eax + 16 * 10]
        movups  xmm4, [eax + 16 * 11]
        movups  xmm5, [eax + 16 * 12]
  {$IFDEF HASAESNI}
        aesenc  xmm7, xmm0
        aesenc  xmm7, xmm1
        aesenc  xmm7, xmm2
        aesenc  xmm7, xmm3
        aesenc  xmm7, xmm4
        aesenclast xmm7, xmm5
  {$ELSE}
        db      $66, $0F, $38, $DC, $F8
        db      $66, $0F, $38, $DC, $F9
        db      $66, $0F, $38, $DC, $FA
        db      $66, $0F, $38, $DC, $FB
        db      $66, $0F, $38, $DC, $FC
        db      $66, $0F, $38, $DD, $FD
  {$ENDIF}
end;

procedure aesniencrypt192(const ctxt, source, dest);
asm // eax=ctxt edx=source ecx=dest
        movups  xmm7, [edx]
        call    AesNiEncryptXmm7_192
        movups  [ecx], xmm7
        pxor    xmm7, xmm7 // for safety
end;

procedure AesNiEncryptXmm7_256;
asm // input: eax=TAESContext, xmm7=data; output: eax=TAESContext, xmm7=data
        movups  xmm0, [eax + 16 * 0]
        movups  xmm1, [eax + 16 * 1]
        movups  xmm2, [eax + 16 * 2]
        movups  xmm3, [eax + 16 * 3]
        movups  xmm4, [eax + 16 * 4]
        movups  xmm5, [eax + 16 * 5]
        movups  xmm6, [eax + 16 * 6]
        pxor    xmm7, xmm0
  {$IFDEF HASAESNI}
        aesenc  xmm7, xmm1
        aesenc  xmm7, xmm2
        aesenc  xmm7, xmm3
        aesenc  xmm7, xmm4
        aesenc  xmm7, xmm5
        aesenc  xmm7, xmm6
  {$ELSE}
        db      $66, $0F, $38, $DC, $F9
        db      $66, $0F, $38, $DC, $FA
        db      $66, $0F, $38, $DC, $FB
        db      $66, $0F, $38, $DC, $FC
        db      $66, $0F, $38, $DC, $FD
        db      $66, $0F, $38, $DC, $FE
  {$ENDIF}
        movups  xmm0, [eax + 16 * 7]
        movups  xmm1, [eax + 16 * 8]
        movups  xmm2, [eax + 16 * 9]
        movups  xmm3, [eax + 16 * 10]
        movups  xmm4, [eax + 16 * 11]
        movups  xmm5, [eax + 16 * 12]
        movups  xmm6, [eax + 16 * 13]
  {$IFDEF HASAESNI}
        aesenc  xmm7, xmm0
        aesenc  xmm7, xmm1
        aesenc  xmm7, xmm2
        aesenc  xmm7, xmm3
        aesenc  xmm7, xmm4
        aesenc  xmm7, xmm5
        aesenc  xmm7, xmm6
  {$ELSE}
        db      $66, $0F, $38, $DC, $F8
        db      $66, $0F, $38, $DC, $F9
        db      $66, $0F, $38, $DC, $FA
        db      $66, $0F, $38, $DC, $FB
        db      $66, $0F, $38, $DC, $FC
        db      $66, $0F, $38, $DC, $FD
        db      $66, $0F, $38, $DC, $FE
  {$ENDIF}
        movups  xmm1, [eax + 16 * 14]
  {$IFDEF HASAESNI}
        aesenclast xmm7, xmm1
  {$ELSE}
        db      $66, $0F, $38, $DD, $F9
  {$ENDIF}
end;

procedure aesniencrypt256(const ctxt, source, dest);
asm // eax=ctxt edx=source ecx=dest
        movups  xmm7, [edx]
        call    AesNiEncryptXmm7_256
        movups  [ecx], xmm7
        pxor    xmm7, xmm7 // for safety
end;

{$ELSE}

procedure aesniencrypt128(const ctxt, source, dest); {$IFDEF FPC}nostackframe; assembler;
asm {$ELSE} asm .noframe {$ENDIF}
        movups  xmm7, dqword ptr[source]
        movups  xmm0, dqword ptr[ctxt + 16 * 0]
        movups  xmm1, dqword ptr[ctxt + 16 * 1]
        movups  xmm2, dqword ptr[ctxt + 16 * 2]
        movups  xmm3, dqword ptr[ctxt + 16 * 3]
        movups  xmm4, dqword ptr[ctxt + 16 * 4]
        movups  xmm5, dqword ptr[ctxt + 16 * 5]
        movups  xmm6, dqword ptr[ctxt + 16 * 6]
        movups  xmm8, dqword ptr[ctxt + 16 * 7]
        movups  xmm9, dqword ptr[ctxt + 16 * 8]
        movups  xmm10, dqword ptr[ctxt + 16 * 9]
        movups  xmm11, dqword ptr[ctxt + 16 * 10]
        pxor    xmm7, xmm0
        aesenc  xmm7, xmm1
        aesenc  xmm7, xmm2
        aesenc  xmm7, xmm3
        aesenc  xmm7, xmm4
        aesenc  xmm7, xmm5
        aesenc  xmm7, xmm6
        aesenc  xmm7, xmm8
        aesenc  xmm7, xmm9
        aesenc  xmm7, xmm10
        aesenclast xmm7, xmm11
        movups  dqword ptr[dest], xmm7
        pxor    xmm7, xmm7 // for safety
end;

procedure aesniencrypt192(const ctxt, source, dest); {$IFDEF FPC}nostackframe; assembler;
asm {$ELSE} asm .noframe {$ENDIF}
        movups  xmm7, dqword ptr[source]
        movups  xmm0, dqword ptr[ctxt + 16 * 0]
        movups  xmm1, dqword ptr[ctxt + 16 * 1]
        movups  xmm2, dqword ptr[ctxt + 16 * 2]
        movups  xmm3, dqword ptr[ctxt + 16 * 3]
        movups  xmm4, dqword ptr[ctxt + 16 * 4]
        movups  xmm5, dqword ptr[ctxt + 16 * 5]
        movups  xmm6, dqword ptr[ctxt + 16 * 6]
        movups  xmm8, dqword ptr[ctxt + 16 * 7]
        movups  xmm9, dqword ptr[ctxt + 16 * 8]
        movups  xmm10, dqword ptr[ctxt + 16 * 9]
        movups  xmm11, dqword ptr[ctxt + 16 * 10]
        movups  xmm12, dqword ptr[ctxt + 16 * 11]
        movups  xmm13, dqword ptr[ctxt + 16 * 12]
        pxor    xmm7, xmm0
        aesenc  xmm7, xmm1
        aesenc  xmm7, xmm2
        aesenc  xmm7, xmm3
        aesenc  xmm7, xmm4
        aesenc  xmm7, xmm5
        aesenc  xmm7, xmm6
        aesenc  xmm7, xmm8
        aesenc  xmm7, xmm9
        aesenc  xmm7, xmm10
        aesenc  xmm7, xmm11
        aesenc  xmm7, xmm12
        aesenclast xmm7, xmm13
        movups  dqword ptr[dest], xmm7
        pxor    xmm7, xmm7 // for safety
end;

procedure aesniencrypt256(const ctxt, source, dest); {$IFDEF FPC}nostackframe; assembler;
asm {$ELSE} asm .noframe {$ENDIF}
        movups  xmm7, dqword ptr[source]
        movups  xmm0, dqword ptr[ctxt + 16 * 0]
        movups  xmm1, dqword ptr[ctxt + 16 * 1]
        movups  xmm2, dqword ptr[ctxt + 16 * 2]
        movups  xmm3, dqword ptr[ctxt + 16 * 3]
        movups  xmm4, dqword ptr[ctxt + 16 * 4]
        movups  xmm5, dqword ptr[ctxt + 16 * 5]
        movups  xmm6, dqword ptr[ctxt + 16 * 6]
        movups  xmm8, dqword ptr[ctxt + 16 * 7]
        movups  xmm9, dqword ptr[ctxt + 16 * 8]
        movups  xmm10, dqword ptr[ctxt + 16 * 9]
        movups  xmm11, dqword ptr[ctxt + 16 * 10]
        movups  xmm12, dqword ptr[ctxt + 16 * 11]
        movups  xmm13, dqword ptr[ctxt + 16 * 12]
        movups  xmm14, dqword ptr[ctxt + 16 * 13]
        movups  xmm15, dqword ptr[ctxt + 16 * 14]
        pxor    xmm7, xmm0
        aesenc  xmm7, xmm1
        aesenc  xmm7, xmm2
        aesenc  xmm7, xmm3
        aesenc  xmm7, xmm4
        aesenc  xmm7, xmm5
        aesenc  xmm7, xmm6
        aesenc  xmm7, xmm8
        aesenc  xmm7, xmm9
        aesenc  xmm7, xmm10
        aesenc  xmm7, xmm11
        aesenc  xmm7, xmm12
        aesenc  xmm7, xmm13
        aesenc  xmm7, xmm14
        aesenclast xmm7, xmm15
        movups  dqword ptr[dest], xmm7
        pxor    xmm7, xmm7 // for safety
end;

procedure aesnidecrypt128(const ctxt, source, dest); {$IFDEF FPC}nostackframe; assembler;
asm {$ELSE} asm .noframe {$ENDIF}
        movups  xmm7, dqword ptr[source]
        movups  xmm0, dqword ptr[ctxt + 16 * 10]
        movups  xmm1, dqword ptr[ctxt + 16 * 9]
        movups  xmm2, dqword ptr[ctxt + 16 * 8]
        movups  xmm3, dqword ptr[ctxt + 16 * 7]
        movups  xmm4, dqword ptr[ctxt + 16 * 6]
        movups  xmm5, dqword ptr[ctxt + 16 * 5]
        movups  xmm6, dqword ptr[ctxt + 16 * 4]
        movups  xmm8, dqword ptr[ctxt + 16 * 3]
        movups  xmm9, dqword ptr[ctxt + 16 * 2]
        movups  xmm10, dqword ptr[ctxt + 16 * 1]
        movups  xmm11, dqword ptr[ctxt + 16 * 0]
        pxor    xmm7, xmm0
        aesdec  xmm7, xmm1
        aesdec  xmm7, xmm2
        aesdec  xmm7, xmm3
        aesdec  xmm7, xmm4
        aesdec  xmm7, xmm5
        aesdec  xmm7, xmm6
        aesdec  xmm7, xmm8
        aesdec  xmm7, xmm9
        aesdec  xmm7, xmm10
        aesdeclast xmm7, xmm11
        movups  dqword ptr[dest], xmm7
        pxor    xmm7, xmm7 // for safety
end;

procedure aesnidecrypt192(const ctxt, source, dest); {$IFDEF FPC}nostackframe; assembler;
asm {$ELSE} asm .noframe {$ENDIF}
        movups  xmm7, dqword ptr[source]
        movups  xmm0, dqword ptr[ctxt + 16 * 12]
        movups  xmm1, dqword ptr[ctxt + 16 * 11]
        movups  xmm2, dqword ptr[ctxt + 16 * 10]
        movups  xmm3, dqword ptr[ctxt + 16 * 9]
        movups  xmm4, dqword ptr[ctxt + 16 * 8]
        movups  xmm5, dqword ptr[ctxt + 16 * 7]
        movups  xmm6, dqword ptr[ctxt + 16 * 6]
        movups  xmm8, dqword ptr[ctxt + 16 * 5]
        movups  xmm9, dqword ptr[ctxt + 16 * 4]
        movups  xmm10, dqword ptr[ctxt + 16 * 3]
        movups  xmm11, dqword ptr[ctxt + 16 * 2]
        movups  xmm12, dqword ptr[ctxt + 16 * 1]
        movups  xmm13, dqword ptr[ctxt + 16 * 0]
        pxor    xmm7, xmm0
        aesdec  xmm7, xmm1
        aesdec  xmm7, xmm2
        aesdec  xmm7, xmm3
        aesdec  xmm7, xmm4
        aesdec  xmm7, xmm5
        aesdec  xmm7, xmm6
        aesdec  xmm7, xmm8
        aesdec  xmm7, xmm9
        aesdec  xmm7, xmm10
        aesdec  xmm7, xmm11
        aesdec  xmm7, xmm12
        aesdeclast xmm7, xmm13
        movups  dqword ptr[dest], xmm7
        pxor    xmm7, xmm7 // for safety
end;
procedure aesnidecrypt256(const ctxt, source, dest); {$IFDEF FPC}nostackframe; assembler;
asm {$ELSE} asm .noframe {$ENDIF}
        movups  xmm7, dqword ptr[source]
        movups  xmm0, dqword ptr[ctxt + 16 * 14]
        movups  xmm1, dqword ptr[ctxt + 16 * 13]
        movups  xmm2, dqword ptr[ctxt + 16 * 12]
        movups  xmm3, dqword ptr[ctxt + 16 * 11]
        movups  xmm4, dqword ptr[ctxt + 16 * 10]
        movups  xmm5, dqword ptr[ctxt + 16 * 9]
        movups  xmm6, dqword ptr[ctxt + 16 * 8]
        movups  xmm8, dqword ptr[ctxt + 16 * 7]
        movups  xmm9, dqword ptr[ctxt + 16 * 6]
        movups  xmm10, dqword ptr[ctxt + 16 * 5]
        movups  xmm11, dqword ptr[ctxt + 16 * 4]
        movups  xmm12, dqword ptr[ctxt + 16 * 3]
        movups  xmm13, dqword ptr[ctxt + 16 * 2]
        movups  xmm14, dqword ptr[ctxt + 16 * 1]
        movups  xmm15, dqword ptr[ctxt + 16 * 0]
        pxor    xmm7, xmm0
        aesdec  xmm7, xmm1
        aesdec  xmm7, xmm2
        aesdec  xmm7, xmm3
        aesdec  xmm7, xmm4
        aesdec  xmm7, xmm5
        aesdec  xmm7, xmm6
        aesdec  xmm7, xmm8
        aesdec  xmm7, xmm9
        aesdec  xmm7, xmm10
        aesdec  xmm7, xmm11
        aesdec  xmm7, xmm12
        aesdec  xmm7, xmm13
        aesdec  xmm7, xmm14
        aesdeclast xmm7, xmm15
        movups  dqword ptr[dest], xmm7
        pxor    xmm7, xmm7 // for safety
end;
{$ENDIF WIN32}
{$ENDIF USEAESNI}

{$IFDEF WIN32}
procedure aesencrypt386(const ctxt: TAESContext; bi, bo: PBlock128);
asm // rolled optimized encryption asm version by A. Bouchez
        push    ebx
        push    esi
        push    edi
        push    ebp
        add     esp,  - 24
        mov     [esp + 4], ecx
        mov     ecx, eax // ecx=pk
        movzx   eax, byte ptr[eax].taescontext.rounds
        dec     eax
        mov     [esp + 20], eax
        mov     ebx, [edx]
        xor     ebx, [ecx]
        mov     esi, [edx + 4]
        xor     esi, [ecx + 4]
        mov     eax, [edx + 8]
        xor     eax, [ecx + 8]
        mov     edx, [edx + 12]
        xor     edx, [ecx + 12]
        lea     ecx, [ecx + 16]
@1:     // pk=ecx s0=ebx s1=esi s2=eax s3=edx
        movzx   edi, bl
        mov     edi, dword ptr[4 * edi + te0]
        movzx   ebp, si
        shr     ebp, $08
        xor     edi, dword ptr[4 * ebp + te1]
        mov     ebp, eax
        shr     ebp, $10
        and     ebp, $ff
        xor     edi, dword ptr[4 * ebp + te2]
        mov     ebp, edx
        shr     ebp, $18
        xor     edi, dword ptr[4 * ebp + te3]
        mov     [esp + 8], edi
        mov     edi, esi
        and     edi, 255
        mov     edi, dword ptr[4 * edi + te0]
        movzx   ebp, ax
        shr     ebp, $08
        xor     edi, dword ptr[4 * ebp + te1]
        mov     ebp, edx
        shr     ebp, $10
        and     ebp, 255
        xor     edi, dword ptr[4 * ebp + te2]
        mov     ebp, ebx
        shr     ebp, $18
        xor     edi, dword ptr[4 * ebp + te3]
        mov     [esp + 12], edi
        movzx   edi, al
        mov     edi, dword ptr[4 * edi + te0]
        movzx   ebp, dh
        xor     edi, dword ptr[4 * ebp + te1]
        mov     ebp, ebx
        shr     ebp, $10
        and     ebp, 255
        xor     edi, dword ptr[4 * ebp + te2]
        mov     ebp, esi
        shr     ebp, $18
        xor     edi, dword ptr[4 * ebp + te3]
        mov     [esp + 16], edi
        and     edx, 255
        mov     edx, dword ptr[4 * edx + te0]
        shr     ebx, $08
        and     ebx, 255
        xor     edx, dword ptr[4 * ebx + te1]
        shr     esi, $10
        and     esi, 255
        xor     edx, dword ptr[4 * esi + te2]
        shr     eax, $18
        xor     edx, dword ptr[4 * eax + te3]
        mov     ebx, [ecx]
        xor     ebx, [esp + 8]
        mov     esi, [ecx + 4]
        xor     esi, [esp + 12]
        mov     eax, [ecx + 8]
        xor     eax, [esp + 16]
        xor     edx, [ecx + 12]
        lea     ecx, [ecx + 16]
        dec     byte ptr[esp + 20]
        jne     @1
        mov     ebp, ecx // ebp=pk
        movzx   ecx, bl
        mov     edi, esi
        movzx   ecx, byte ptr[ecx + SBox]
        shr     edi, $08
        and     edi, 255
        movzx   edi, byte ptr[edi + SBox]
        shl     edi, $08
        xor     ecx, edi
        mov     edi, eax
        shr     edi, $10
        and     edi, 255
        movzx   edi, byte ptr[edi + SBox]
        shl     edi, $10
        xor     ecx, edi
        mov     edi, edx
        shr     edi, $18
        movzx   edi, byte ptr[edi + SBox]
        shl     edi, $18
        xor     ecx, edi
        xor     ecx, [ebp]
        mov     edi, [esp + 4]
        mov     [edi], ecx
        mov     ecx, esi
        and     ecx, 255
        movzx   ecx, byte ptr[ecx + SBox]
        movzx   edi, ah
        movzx   edi, byte ptr[edi + SBox]
        shl     edi, $08
        xor     ecx, edi
        mov     edi, edx
        shr     edi, $10
        and     edi, 255
        movzx   edi, byte ptr[edi + SBox]
        shl     edi, $10
        xor     ecx, edi
        mov     edi, ebx
        shr     edi, $18
        movzx   edi, byte ptr[edi + SBox]
        shl     edi, $18
        xor     ecx, edi
        xor     ecx, [ebp + 4]
        mov     edi, [esp + 4]
        mov     [edi + 4], ecx
        mov     ecx, eax
        and     ecx, 255
        movzx   ecx, byte ptr[ecx + SBox]
        movzx   edi, dh
        movzx   edi, byte ptr[edi + SBox]
        shl     edi, $08
        xor     ecx, edi
        mov     edi, ebx
        shr     edi, $10
        and     edi, 255
        movzx   edi, byte ptr[edi + SBox]
        shl     edi, $10
        xor     ecx, edi
        mov     edi, esi
        shr     edi, $18
        movzx   edi, byte ptr[edi + SBox]
        shl     edi, $18
        xor     ecx, edi
        xor     ecx, [ebp + 8]
        mov     edi, [esp + 4]
        mov     [edi + 8], ecx
        and     edx, 255
        movzx   edx, byte ptr[edx + SBox]
        shr     ebx, $08
        and     ebx, 255
        xor     ecx, ecx
        mov     cl, byte ptr[ebx + SBox]
        shl     ecx, $08
        xor     edx, ecx
        shr     esi, $10
        and     esi, 255
        xor     ecx, ecx
        mov     cl, byte ptr[esi + SBox]
        shl     ecx, $10
        xor     edx, ecx
        shr     eax, $18
        movzx   eax, byte ptr[eax + SBox]
        shl     eax, $18
        xor     edx, eax
        xor     edx, [ebp + 12]
        mov     eax, [esp + 4]
        mov     [eax + 12], edx
        add     esp, 24
        pop     ebp
        pop     edi
        pop     esi
        pop     ebx
end;
{$ELSE}
procedure aesencryptx64(const ctxt: TAESContext; bi, bo: PBlock128);
{$IFDEF FPC}nostackframe; assembler; asm{$ELSE}
        asm // input: rcx/rdi=TAESContext, rdx/rsi=source, r8/rdx=dest
        .noframe
{$ENDIF} // rolled optimized encryption asm version by A. Bouchez
        push    r15
        push    r14
        push    r13
        push    r12
        push    rbx
        push    rbp
        {$IFDEF win64}
        push    rdi
        push    rsi
        mov     r15, r8
        mov     r12, rcx
        {$ELSE}
        mov     r15, rdx
        mov     rdx, rsi
        mov     r12, rdi
        {$ENDIF win64}
        movzx   r13, byte ptr [r12].TAESContext.Rounds
        mov     eax, dword ptr [rdx]
        mov     ebx, dword ptr [rdx+4H]
        mov     ecx, dword ptr [rdx+8H]
        mov     edx, dword ptr [rdx+0CH]
        xor     eax, dword ptr [r12]
        xor     ebx, dword ptr [r12+4H]
        xor     ecx, dword ptr [r12+8H]
        xor     edx, dword ptr [r12+0CH]
        sub     r13, 1
        add     r12, 16
        lea     r14, [rip+Te0]
        {$IFDEF FPC} align 16 {$ELSE} .align 16 {$ENDIF}
@round: mov     esi, eax
        mov     edi, edx
        movzx   r8d, al
        movzx   r9d, cl
        movzx   r10d, bl
        mov     r8d, dword ptr [r14+r8*4]
        mov     r9d, dword ptr [r14+r9*4]
        mov     r10d, dword ptr [r14+r10*4]
        shr     esi, 16
        shr     edi, 16
        movzx   ebp, bh
        xor     r8d, dword ptr [r14+rbp*4+400H]
        movzx   ebp, dh
        xor     r9d, dword ptr [r14+rbp*4+400H]
        movzx   ebp, ch
        xor     r10d, dword ptr [r14+rbp*4+400H]
        shr     ebx, 16
        shr     ecx, 16
        movzx   ebp, dl
        mov     edx, dword ptr [r14+rbp*4]
        movzx   ebp, cl
        xor     r8d, dword ptr [r14+rbp*4+800H]
        movzx   ebp, sil
        xor     r9d, dword ptr [r14+rbp*4+800H]
        movzx   r11, dil
        movzx   eax, ah
        shr     edi, 8
        movzx   ebp, bh
        shr     esi, 8
        xor     r10d, dword ptr [r14+r11*4+800H]
        xor     edx, dword ptr [r14+rax*4+400H]
        xor     r8d, dword ptr [r14+rdi*4+0C00H]
        xor     r9d, dword ptr [r14+rbp*4+0C00H]
        xor     r10d, dword ptr [r14+rsi*4+0C00H]
        movzx   ebp, bl
        xor     edx, dword ptr [r14+rbp*4+800H]
        mov     rbx, r10
        mov     rax, r8
        movzx   ebp, ch
        xor     edx, dword ptr [r14+rbp*4+0C00H]
        mov     rcx, r9
        xor     eax, dword ptr [r12]
        xor     ebx, dword ptr [r12+4H]
        xor     ecx, dword ptr [r12+8H]
        xor     edx, dword ptr [r12+0CH]
        add     r12, 16
        sub     r13, 1
        jnz     @round
        lea     r9, [rip+SBox]
        movzx   r8, al
        movzx   r14, byte ptr [r9+r8]
        movzx   edi, bh
        movzx   r8, byte ptr [r9+rdi]
        shl     r8d, 8
        xor     r14d, r8d
        mov     r11, rcx
        shr     r11, 16
        and     r11, 0FFH
        movzx   r8, byte ptr [r9+r11]
        shl     r8d, 16
        xor     r14d, r8d
        mov     r11, rdx
        shr     r11, 24
        movzx   r8, byte ptr [r9+r11]
        shl     r8d, 24
        xor     r14d, r8d
        xor     r14d, dword ptr [r12]
        mov     dword ptr [r15], r14d
        movzx   r8, bl
        movzx   r14, byte ptr [r9+r8]
        movzx   edi, ch
        movzx   r8, byte ptr [r9+rdi]
        shl     r8d, 8
        xor     r14d, r8d
        mov     r11, rdx
        shr     r11, 16
        and     r11, 0FFH
        movzx   r8, byte ptr [r9+r11]
        shl     r8d, 16
        xor     r14d, r8d
        mov     r11, rax
        shr     r11, 24
        movzx   r8, byte ptr [r9+r11]
        shl     r8d, 24
        xor     r14d, r8d
        xor     r14d, dword ptr [r12+4H]
        mov     dword ptr [r15+4H], r14d
        movzx   r8, cl
        movzx   r14, byte ptr [r9+r8]
        movzx   edi, dh
        movzx   r8, byte ptr [r9+rdi]
        shl     r8d, 8
        xor     r14d, r8d
        mov     r11, rax
        shr     r11, 16
        and     r11, 0FFH
        movzx   r8, byte ptr [r9+r11]
        shl     r8d, 16
        xor     r14d, r8d
        mov     r11, rbx
        shr     r11, 24
        movzx   r8, byte ptr [r9+r11]
        shl     r8d, 24
        xor     r14d, r8d
        xor     r14d, dword ptr [r12+8H]
        mov     dword ptr [r15+8H], r14d
        and     rdx, 0FFH
        movzx   r14, byte ptr [r9+rdx]
        movzx   eax, ah
        movzx   r8, byte ptr [r9+rax]
        shl     r8d, 8
        xor     r14d, r8d
        shr     rbx, 16
        and     rbx, 0FFH
        movzx   r8, byte ptr [r9+rbx]
        shl     r8d, 16
        xor     r14d, r8d
        shr     rcx, 24
        movzx   r8, byte ptr [r9+rcx]
        shl     r8d, 24
        xor     r14d, r8d
        xor     r14d, dword ptr [r12+0CH]
        mov     dword ptr [r15+0CH], r14d
        {$IFDEF win64}
        pop     rsi
        pop     rdi
        {$ENDIF win64}
        pop     rbp
        pop     rbx
        pop     r12
        pop     r13
        pop     r14
        pop     r15
end;
{$ENDIF WIN32}

{$IFDEF WIN32}
procedure XorBlock16(A,B: PCardinalArray); overload;
begin
  A[0] := A[0] xor B[0];
  A[1] := A[1] xor B[1];
  A[2] := A[2] xor B[2];
  A[3] := A[3] xor B[3];
end;

procedure XorBlock16(A,B,C: PCardinalArray); overload;
begin
  B[0] := A[0] xor C[0];
  B[1] := A[1] xor C[1];
  B[2] := A[2] xor C[2];
  B[3] := A[3] xor C[3];
end;
{$ELSE}
procedure XorBlock16(A,B: PInt64Array); overload;
begin
  A[0] := A[0] xor B[0];
  A[1] := A[1] xor B[1];
end;

procedure XorBlock16(A,B,C: PInt64Array); overload;
begin
  B[0] := A[0] xor C[0];
  B[1] := A[1] xor C[1];
end;
{$ENDIF WIN32}

procedure XorMemory(Dest, Source1, Source2: PByteArray; Len: PtrInt);
begin
  while Len >= SizeOf(PtrInt) do
  begin
    Dec(Len, SizeOf(PtrInt));
    PPtrInt(Dest)^ := PPtrInt(Source1)^ xor PPtrInt(Source2)^;
    Inc(PPtrInt(Dest));
    Inc(PPtrInt(Source1));
    Inc(PPtrInt(Source2));
  end;

  while Len > 0 do
  begin
    Dec(Len);
    Dest[Len] := Source1[Len] xor Source2[Len];
  end;
end;

procedure Shift(const KeySize: Integer; Keys: PKeyArrayCardinal);
var
  I: Integer;
  Temp: Cardinal;
begin
  // 32 bit use shift and mask
  case KeySize of
    128:
      for I := 0 to 9 do
      begin
        Temp := Keys^[3];

        // SubWord(RotWord(Temp)) if "word" count mod 4 = 0
        Keys^[4] := ((SBox[(Temp shr 8) and $FF])) xor
          ((SBox[(Temp shr 16) and $FF]) shl 8) xor
          ((SBox[(Temp shr 24)]) shl 16) xor
          ((SBox[(Temp) and $FF]) shl 24) xor
          Keys^[0] xor RCon[I];

        Keys^[5] := Keys^[1] xor Keys^[4];
        Keys^[6] := Keys^[2] xor Keys^[5];
        Keys^[7] := Keys^[3] xor Keys^[6];
        Inc(PByte(Keys), 4 * 4);
      end;
    192:
      for I := 0 to 7 do
      begin
        Temp := Keys^[5];

        // SubWord(RotWord(Temp)) if "word" count mod 6 = 0
        Keys^[6] := ((SBox[(Temp shr 8) and $FF])) xor
          ((SBox[(Temp shr 16) and $FF]) shl 8) xor
          ((SBox[(Temp shr 24)]) shl 16) xor
          ((SBox[(Temp) and $FF]) shl 24) xor
          Keys^[0] xor RCon[I];

        Keys^[7] := Keys^[1] xor Keys^[6];
        Keys^[8] := Keys^[2] xor Keys^[7];
        Keys^[9] := Keys^[3] xor Keys^[8];

        if I = 7 then Exit;

        Keys^[10] := Keys^[4] xor Keys^[9];
        Keys^[11] := Keys^[5] xor Keys^[10];
        Inc(PByte(Keys), 6 * 4);
      end;
  else // 256:
    for I := 0 to 6 do
    begin
      Temp := Keys^[7];

      // SubWord(RotWord(Temp)) if "word" count mod 8 = 0
      Keys^[8] := ((SBox[(Temp shr 8) and $FF])) xor
        ((SBox[(Temp shr 16) and $FF]) shl 8) xor
        ((SBox[(Temp shr 24)]) shl 16) xor
        ((SBox[(Temp) and $FF]) shl 24) xor
        Keys^[0] xor RCon[I];

      Keys^[9] := Keys^[1] xor Keys^[8];
      Keys^[10] := Keys^[2] xor Keys^[9];
      Keys^[11] := Keys^[3] xor Keys^[10];

      if I = 6 then Exit;

      Temp := Keys^[11];

      // SubWord(Temp) if "word" count mod 8 = 4
      Keys^[12] := ((SBox[(Temp) and $FF])) xor
        ((SBox[(Temp shr 8) and $FF]) shl 8) xor
        ((SBox[(Temp shr 16) and $FF]) shl 16) xor
        ((SBox[(Temp shr 24)]) shl 24) xor
        Keys^[4];

      Keys^[13] := Keys^[5] xor Keys^[12];
      Keys^[14] := Keys^[6] xor Keys^[13];
      Keys^[15] := Keys^[7] xor Keys^[14];
      Inc(PByte(Keys), 8 * 4);
    end;
  end;
end;

{$IFDEF USEAESNI} // should be put outside the main method for FPC :(
procedure ShiftAesNi(KeySize: Cardinal; pk: Pointer);
{$IFDEF WIN32}
asm // eax=KeySize edx=pk
        movups  xmm1, [edx]
        movups  xmm5, dqword ptr[@mask]
        cmp     al, 128
        je      @128
        cmp     al, 192
        je      @e // 192 bits is very complicated -> skip by now (use 128+256)
@256:   movups  xmm3, [edx + 16]
        add     edx, 32
        db      $66, $0F, $3A, $DF, $D3, $01 // aeskeygenassist xmm2,xmm3,1
        call    @exp256
        db      $66, $0F, $3A, $DF, $D3, $02 // aeskeygenassist xmm2,xmm3,2
        call    @exp256
        db      $66, $0F, $3A, $DF, $D3, $04 // aeskeygenassist xmm2,xmm3,4
        call    @exp256
        db      $66, $0F, $3A, $DF, $D3, $08 // aeskeygenassist xmm2,xmm3,8
        call    @exp256
        db      $66, $0F, $3A, $DF, $D3, $10 // aeskeygenassist xmm2,xmm3,$10
        call    @exp256
        db      $66, $0F, $3A, $DF, $D3, $20 // aeskeygenassist xmm2,xmm3,$20
        call    @exp256
        db      $66, $0F, $3A, $DF, $D3, $40 // aeskeygenassist xmm2,xmm3,$40
        pshufd  xmm2, xmm2, $FF
        movups  xmm4, xmm1
        db      $66, $0F, $38, $00, $E5 // pshufb xmm4,xmm5
        pxor    xmm1, xmm4
        db      $66, $0F, $38, $00, $E5 // pshufb xmm4,xmm5
        pxor    xmm1, xmm4
        db      $66, $0F, $38, $00, $E5 // pshufb xmm4,xmm5
        pxor    xmm1, xmm4
        pxor    xmm1, xmm2
        movups  [edx], xmm1
        jmp     @e
@mask:  dd      $ffffffff
        dd      $03020100
        dd      $07060504
        dd      $0b0a0908
@exp256:pshufd  xmm2, xmm2, $ff
        movups  xmm4, xmm1
        db      $66, $0F, $38, $00, $E5 // pshufb xmm4,xmm5
        pxor    xmm1, xmm4
        db      $66, $0F, $38, $00, $E5 // pshufb xmm4,xmm5
        pxor    xmm1, xmm4
        db      $66, $0F, $38, $00, $E5 // pshufb xmm4,xmm5
        pxor    xmm1, xmm4
        pxor    xmm1, xmm2
        movups  [edx], xmm1
        add     edx, $10
        db      $66, $0F, $3A, $DF, $E1, $00 // aeskeygenassist xmm4,xmm1,0
        pshufd  xmm2, xmm4, $AA
        movups  xmm4, xmm3
        db      $66, $0F, $38, $00, $E5 // pshufb xmm4,xmm5
        pxor    xmm3, xmm4
        db      $66, $0F, $38, $00, $E5 // pshufb xmm4,xmm5
        pxor    xmm3, xmm4
        db      $66, $0F, $38, $00, $E5 // pshufb xmm4,xmm5
        pxor    xmm3, xmm4
        pxor    xmm3, xmm2
        movups  [edx], xmm3
        add     edx, $10
        ret
@exp128:pshufd  xmm2, xmm2, $FF
        movups  xmm3, xmm1
        db      $66, $0F, $38, $00, $DD // pshufb xmm3,xmm5
        pxor    xmm1, xmm3
        db      $66, $0F, $38, $00, $DD // pshufb xmm3,xmm5
        pxor    xmm1, xmm3
        db      $66, $0F, $38, $00, $DD // pshufb xmm3,xmm5
        pxor    xmm1, xmm3
        pxor    xmm1, xmm2
        movups  [edx], xmm1
        add     edx, $10
        ret
@128:   add     edx, 16
        db      $66, $0F, $3A, $DF, $D1, $01 // aeskeygenassist xmm2,xmm1,1
        call    @exp128
        db      $66, $0F, $3A, $DF, $D1, $02 // aeskeygenassist xmm2,xmm1,2
        call    @exp128
        db      $66, $0F, $3A, $DF, $D1, $04 // aeskeygenassist xmm2,xmm1,4
        call    @exp128
        db      $66, $0F, $3A, $DF, $D1, $08 // aeskeygenassist xmm2,xmm1,8
        call    @exp128
        db      $66, $0F, $3A, $DF, $D1, $10 // aeskeygenassist xmm2,xmm1,$10
        call    @exp128
        db      $66, $0F, $3A, $DF, $D1, $20 // aeskeygenassist xmm2,xmm1,$20
        call    @exp128
        db      $66, $0F, $3A, $DF, $D1, $40 // aeskeygenassist xmm2,xmm1,$40
        call    @exp128
        db      $66, $0F, $3A, $DF, $D1, $80 // aeskeygenassist xmm2,xmm1,$80
        call    @exp128
        db      $66, $0F, $3A, $DF, $D1, $1b // aeskeygenassist xmm2,xmm1,$1b
        call    @exp128
        db      $66, $0F, $3A, $DF, $D1, $36 // aeskeygenassist xmm2,xmm1,$36
        call    @exp128
@e:     db      $f3 // rep ret
end;
{$ELSE}
{$IFDEF FPC} nostackframe; assembler; asm {$ELSE} asm .noframe {$ENDIF}
        mov     eax, keysize
        movups  xmm1, dqword ptr[pk]
        movaps  xmm5, dqword ptr[rip + @mask]
        cmp     al, 128
        je      @128
        cmp     al, 192
        je      @e // 192 bits is very complicated -> skip by now (128+256)
@256:   movups  xmm3, dqword ptr[pk + 16]
        add     pk, 32
        aeskeygenassist xmm2, xmm3, 1
        call    @exp256
        aeskeygenassist xmm2, xmm3, 2
        call    @exp256
        aeskeygenassist xmm2, xmm3, 4
        call    @exp256
        aeskeygenassist xmm2, xmm3, 8
        call    @exp256
        aeskeygenassist xmm2, xmm3, $10
        call    @exp256
        aeskeygenassist xmm2, xmm3, $20
        call    @exp256
        aeskeygenassist xmm2, xmm3, $40
        pshufd  xmm2, xmm2, $FF
        movups  xmm4, xmm1
        pshufb  xmm4, xmm5
        pxor    xmm1, xmm4
        pshufb  xmm4, xmm5
        pxor    xmm1, xmm4
        pshufb  xmm4, xmm5
        pxor    xmm1, xmm4
        pxor    xmm1, xmm2
        movups  dqword ptr[pk], xmm1
        jmp     @e
{$IFDEF FPC} align 16 {$ELSE} .align 16 {$ENDIF}
@mask:  dd      $ffffffff
        dd      $03020100
        dd      $07060504
        dd      $0b0a0908
@exp256:pshufd  xmm2, xmm2, $ff
        movups  xmm4, xmm1
        pshufb  xmm4, xmm5
        pxor    xmm1, xmm4
        pshufb  xmm4, xmm5
        pxor    xmm1, xmm4
        pshufb  xmm4, xmm5
        pxor    xmm1, xmm4
        pxor    xmm1, xmm2
        movups  dqword ptr[pk], xmm1
        add     pk, $10
        aeskeygenassist xmm4, xmm1, 0
        pshufd  xmm2, xmm4, $AA
        movups  xmm4, xmm3
        pshufb  xmm4, xmm5
        pxor    xmm3, xmm4
        pshufb  xmm4, xmm5
        pxor    xmm3, xmm4
        pshufb  xmm4, xmm5
        pxor    xmm3, xmm4
        pxor    xmm3, xmm2
        movups  dqword ptr[pk], xmm3
        add     pk, $10
@e:     ret
@exp128:pshufd  xmm2, xmm2, $FF
        movups  xmm3, xmm1
        pshufb  xmm3, xmm5
        pxor    xmm1, xmm3
        pshufb  xmm3, xmm5
        pxor    xmm1, xmm3
        pshufb  xmm3, xmm5
        pxor    xmm1, xmm3
        pxor    xmm1, xmm2
        movups  dqword ptr[pk], xmm1
        add     pk, $10
        ret
@128:   add     pk, 16
        aeskeygenassist xmm2, xmm1, 1
        call    @exp128
        aeskeygenassist xmm2, xmm1, 2
        call    @exp128
        aeskeygenassist xmm2, xmm1, 4
        call    @exp128
        aeskeygenassist xmm2, xmm1, 8
        call    @exp128
        aeskeygenassist xmm2, xmm1, $10
        call    @exp128
        aeskeygenassist xmm2, xmm1, $20
        call    @exp128
        aeskeygenassist xmm2, xmm1, $40
        call    @exp128
        aeskeygenassist xmm2, xmm1, $80
        call    @exp128
        aeskeygenassist xmm2, xmm1, $1b
        call    @exp128
        aeskeygenassist xmm2, xmm1, $36
        call    @exp128
end;
{$ENDIF CPU64}
{$ENDIF USEAESNI}

procedure AESContextInit(KeyBuf: Pointer; KeyBufLen: Integer; var Context: TAESContext; const KeySizeType: TAESKeySizeType);
const
  KeySizeLen: array[TAESKeySizeType] of Integer = (128, 192, 256);
var
  KeySize, MinKeyLen: Integer;
begin
  FillChar(Context, SizeOf(Context), 0);
  KeySize := KeySizeLen[KeySizeType];
  Context.Rounds := KeySize div 32 + 6;
  Context.KeyBits := KeySize;

  MinKeyLen := KeySize div 8;
  if KeyBufLen < MinKeyLen then
    MinKeyLen := KeyBufLen;

  Move(KeyBuf^, Context.KeyArr, MinKeyLen);

{$IFDEF WIN32}
  Context.DoBlock := @aesencrypt386;
{$ELSE}
  Context.DoBlock := @aesencryptx64;
{$ENDIF WIN32}

{$IFDEF USEAESNI}
  //if cfAESNI in CpuFeatures then
  begin
     case KeySizeType of
       ks_128bit: Context.DoBlock := @aesniencrypt128;
       ks_192bit: Context.DoBlock := @aesniencrypt192;
       ks_256bit: Context.DoBlock := @aesniencrypt256;
     end;
  end;

  if (KeySizeType <> ks_192bit) {and (cfAESNI in CpuFeatures)} then
    ShiftAesNi(KeySize, @Context.KeyArr)
  else
    Shift(KeySize, Pointer(@Context.KeyArr));
{$ELSE}
  Shift(KeySize, Pointer(@Context.KeyArr));
{$ENDIF}
end;

procedure DoAESEncrypt(Context: TAESContext; InBuf, OutBuf: Pointer; Count: Cardinal);
var
  I: Integer;
  Offset: PtrInt;
  Temp: TAESBlock;
  Block: TAESBlock;
begin
  FillChar(Block, SizeOf(Block), 0);
  FillChar(Temp, SizeOf(Temp), 0);

  for I := 1 to Count shr 4 do
  begin
    Context.DoBlock(Context, Block, Temp);

    Offset := 7;
    Inc(Block[Offset]);
    if Block[Offset] = 0 then // manual big-endian increment
      repeat
        Dec(Offset);
        Inc(Block[Offset]);
        if (Block[Offset] <> 0) or (Offset = 7) then
          break;
      until False;
    XorBlock16(InBuf, OutBuf, Pointer(@Temp));
    PtrInt(InBuf) := PtrInt(InBuf) + 16;
    PtrInt(OutBuf) := PtrInt(OutBuf) + 16;
  end;
  
  Count := Count and 15;
  if Count <> 0 then
  begin
    Context.DoBlock(Context, Block, Temp);
    XorMemory(OutBuf, InBuf, @Temp, Count);
  end;
end;

procedure AESEncrypt(
  KeyBuf: Pointer; KeyBufLen: Integer;
  InBuf, OutBuf: Pointer; BufLen: Integer;
  const KeySize: TAESKeySizeType = ks_128bit);
var
  Context: TAESContext;
begin
  AESContextInit(KeyBuf, KeyBufLen, Context, KeySize);
  DoAESEncrypt(Context, InBuf, OutBuf, BufLen);
end;

procedure AESDecrypt(
  KeyBuf: Pointer; KeyBufLen: Integer;
  InBuf, OutBuf: Pointer; BufLen: Integer;
  const KeySize: TAESKeySizeType = ks_128bit);
var
  Context: TAESContext;
begin
  AESContextInit(KeyBuf, KeyBufLen, Context, KeySize);
  DoAESEncrypt(Context, InBuf, OutBuf, BufLen);
end;

procedure ComputeAesStaticTables;
var
  I, X, Y: Byte;
  Pow, Log: array[Byte] of Byte;
  C: Cardinal;
begin // 835 bytes of code to compute 4.5 KB of tables
  X := 1;
  for I := 0 to 255 do
  begin
    Pow[I] := X;
    Log[X] := I;
    if X and $80 <> 0 then
      X := X xor (X shl 1) xor $1B
    else
      X := X xor (X shl 1);
  end;
  SBox[0] := $63;
  InvSBox[$63] := 0;
  for I := 1 to 255 do
  begin
    X := Pow[255 - Log[I]];
    Y := (X shl 1) + (X shr 7);
    X := X xor Y;
    Y := (Y shl 1) + (Y shr 7);
    X := X xor Y;
    Y := (Y shl 1) + (Y shr 7);
    X := X xor Y;
    Y := (Y shl 1) + (Y shr 7);
    X := X xor Y xor $63;
    SBox[I] := X;
    InvSBox[X] := I;
  end;

  for I := 0 to 255 do
  begin
    X := SBox[I];
    Y := X shl 1;
    if X and $80 <> 0 then
      Y := Y xor $1B;
    Te0[I] := Y + X shl 8 + X shl 16 + (Y xor X) shl 24;
    Te1[I] := Te0[I] shl 8 + Te0[I] shr 24;
    Te2[I] := Te1[I] shl 8 + Te1[I] shr 24;
    Te3[I] := Te2[I] shl 8 + Te2[I] shr 24;
    X := InvSBox[I];
    if X = 0 then
      continue;
    C := Log[X]; // Td0[C] = Si[C].[0e,09,0d,0b] -> e.g. Log[$0e]=223 below
    Td0[I] := Pow[(C + 223) mod 255] + Pow[(C + 199) mod 255] shl 8 +
      Pow[(C + 238) mod 255] shl 16 + Pow[(C + 104) mod 255] shl 24;
    Td1[I] := Td0[I] shl 8 + Td0[I] shr 24;
    Td2[I] := Td1[I] shl 8 + Td1[I] shr 24;
    Td3[I] := Td2[I] shl 8 + Td2[I] shr 24;
  end;
end;

initialization
  ComputeAesStaticTables;

end.

