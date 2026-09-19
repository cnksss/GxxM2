unit kbmMove;

// =========================================================================
// kbmMove - fast memory move routines
//
// Copyright 1999-2012 Kim Bo Madsen/Components4Developers DK
// All rights reserved.
//
// This unit is based on the FastLib JOH5 memory move routines.
//
// LICENSE AGREEMENT
//
// This unit is not open source or freeware. Its part of the kbmMW package
// not the kbmMemTable package.
//
// All its contents is confidential, may not be discussed in public
// Any knowledge you may have gained from it may not be used for making
// products competing with C4D products.
//
// Before using this file you must have read, understood and accepted the
// the license agreement which you find in the file license.txt.
// If that file is not part of the package then the package is not valid and
// must be removed immediately. A valid package can be downloaded from
// Components4Developers at www.components4developers.com
// Further you must have read the license agreement in the top of this
// unit and accepted it.

interface

{$I kbmMemTable.inc}

procedure kbmMemMove(const Source; var Dest; Count : Integer);

implementation

{$IFDEF NEXTGEN}
procedure kbmMemMove(const Source; var Dest; Count : Integer);
begin
     move(Source,Dest,Count);
end;
{$ELSE}
 {$IFNDEF CPUX86}

procedure kbmMemMove(const Source; var Dest; Count : Integer);
begin
     move(Source,Dest,Count);
end;

 {$ELSE}
var {These are set by the Unit Initialisation}
  EnableMMX      : Boolean = False;
  EnableSSE      : Boolean = False;
  EnableSSE2     : Boolean = False;
  Enable3DNow    : Boolean = False;
  EnableExt3DNow : Boolean = False;

{---------------------------------------------------------------------------
---}
procedure GetCPUFeatures; {Called Once by Unit Initialisation}
asm
  push    ebx
{Test for CPUID Support}
  pushfd
  pop    eax
  mov    edx, eax
  xor    edx, $200000
  push   eax
  popfd
  pushfd
  pop    eax
  cmp    eax, edx
  jz     @Exit {No CPUID Support}
{Test for MMX Support}
  mov    eax, 1
  cpuid {Get feature bits into edx}
  test   edx, (1 shl 23)
  setnz  EnableMMX
{Test for SSE Support}
  test   edx,(1 shl 25)
  setnz  EnableSSE
{Test for SSE2 Support}
  test   edx,(1 shl 26)
  setnz  EnableSSE2
{Test for 3DNow!/Extended 3DNow! Support}
  mov     eax, $80000000
  cpuid {Get Max Extended CPUID Level}
  cmp     eax, $80000001
  jl      @End3dNow {Type/Family/Model/Stepping/Flags Unavailable}
  mov     eax, $80000001
  cpuid {Get Extended CPU Features}
{Test for 3DNow! Support}
  test    edx, (1 shl 31)
  setnz   Enable3DNow
{Test for Extended 3DNow! Support}
  test    edx, (1 shl 30)
  setnz   EnableExt3DNow
@End3dNow:
@Exit:
  pop     ebx
end; {GetCPUFeatures}

{---------------------------------------------------------------------------
---}
procedure AlignedFwdMoveMMX(const Source; var Dest; Count: Integer);
asm
  push   esi
  push   edi
  mov    esi,eax          {ESI = Source}
  mov    edi,edx          {EDI = Dest}
  mov    eax,ecx          {EAX = Count}
  and    eax,$FFFFFFC0    {EAX = No of Bytes to Blocks Moves}
  and    ecx,$3F          {ECX = Remaining Bytes to Move (0..63)}
  add    esi,eax
  add    edi,eax
  shr    eax,3            {EAX = No of QWORD's to Block Move}
  shr    ecx,2            {ECX = Remaining DWORD's to move (0.15)}
  neg    eax
@MMXcopyloop:
  movq   mm0,[esi+eax*8   ]
  movq   mm1,[esi+eax*8+ 8]
  movq   mm2,[esi+eax*8+16]
  movq   mm3,[esi+eax*8+24]
  movq   mm4,[esi+eax*8+32]
  movq   mm5,[esi+eax*8+40]
  movq   mm6,[esi+eax*8+48]
  movq   mm7,[esi+eax*8+56]
  movq   [edi+eax*8   ],mm0
  movq   [edi+eax*8+ 8],mm1
  movq   [edi+eax*8+16],mm2
  movq   [edi+eax*8+24],mm3
  movq   [edi+eax*8+32],mm4
  movq   [edi+eax*8+40],mm5
  movq   [edi+eax*8+48],mm6
  movq   [edi+eax*8+56],mm7
  add    eax,8
  jnz    @MMXcopyloop
  emms                    {Empty MMX State}
  rep    movsd            {Move Remaining Bytes}
  pop    edi
  pop    esi
end; {AlignedFwdMoveMMX}

{---------------------------------------------------------------------------
---}
procedure AlignedFwdMoveSSE2(const Source; var Dest; Count: Integer);
const
  Prefetch = 256;
asm
  push    esi
  push    edi
  mov     esi,eax          {ESI = Source}
  mov     edi,edx          {EDI = Dest}
  mov     eax,ecx          {EAX = Count}
  and     eax,$FFFFFF80    {EAX = No of Bytes to Blocks Moves}
  and     ecx,$7F          {ECX = Remaining Bytes to Move (0..127)}
  add     esi,eax
  add     edi,eax
  shr     eax,3            {EAX = No of QWORD's to Block Move}
  shr     ecx,2            {ECX = Remaining DWORD's to move (0.15)}
  neg     eax
  cmp     eax, -(32*1024)  {Count > 256K}
  jl      @Large
@Small:
  test    esi,15           {Check if Both Source/Dest Aligned}
  jnz     @SmallUnaligned
  nop                      {DWORD Align Loops}
@SmallAligned:
@SmallAlignedLoop:
  movaps  xmm0,[esi+8*eax]
  movaps  xmm1,[esi+8*eax+16]
  movaps  xmm2,[esi+8*eax+32]
  movaps  xmm3,[esi+8*eax+48]
  movaps  xmm4,[esi+8*eax+64]
  movaps  xmm5,[esi+8*eax+80]
  movaps  xmm6,[esi+8*eax+96]
  movaps  xmm7,[esi+8*eax+112]
  movaps  [edi+8*eax],xmm0
  movaps  [edi+8*eax+16],xmm1
  movaps  [edi+8*eax+32],xmm2
  movaps  [edi+8*eax+48],xmm3
  movaps  [edi+8*eax+64],xmm4
  movaps  [edi+8*eax+80],xmm5
  movaps  [edi+8*eax+96],xmm6
  movaps  [edi+8*eax+112],xmm7
  add     eax,16
  js      @SmallAlignedLoop
  rep     movsd            {Move Remaining Bytes}
  pop     edi
  pop     esi
  ret
@SmallUnaligned:
@SmallUnalignedLoop:
  movups  xmm0,[esi+8*eax]
  movups  xmm1,[esi+8*eax+16]
  movups  xmm2,[esi+8*eax+32]
  movups  xmm3,[esi+8*eax+48]
  movups  xmm4,[esi+8*eax+64]
  movups  xmm5,[esi+8*eax+80]
  movups  xmm6,[esi+8*eax+96]
  movups  xmm7,[esi+8*eax+112]
  movaps  [edi+8*eax],xmm0
  movaps  [edi+8*eax+16],xmm1
  movaps  [edi+8*eax+32],xmm2
  movaps  [edi+8*eax+48],xmm3
  movaps  [edi+8*eax+64],xmm4
  movaps  [edi+8*eax+80],xmm5
  movaps  [edi+8*eax+96],xmm6
  movaps  [edi+8*eax+112],xmm7
  add     eax,16
  js      @SmallUnalignedLoop
  rep     movsd            {Move Remaining Bytes}
  pop     edi
  pop     esi
  ret
@Large:
  test    esi,15           {Check if Both Source/Dest Aligned}
  jnz     @LargeUnaligned
@LargeAligned:
@LargeAlignedLoop:
  prefetchnta  [esi+8*eax+Prefetch]
  prefetchnta  [esi+8*eax+Prefetch+64]
  movdqa  xmm0,[esi+8*eax]
  movdqa  xmm1,[esi+8*eax+16]
  movdqa  xmm2,[esi+8*eax+32]
  movdqa  xmm3,[esi+8*eax+48]
  movdqa  xmm4,[esi+8*eax+64]
  movdqa  xmm5,[esi+8*eax+80]
  movdqa  xmm6,[esi+8*eax+96]
  movdqa  xmm7,[esi+8*eax+112]
  movntdq [edi+8*eax],xmm0
  movntdq [edi+8*eax+16],xmm1
  movntdq [edi+8*eax+32],xmm2
  movntdq [edi+8*eax+48],xmm3
  movntdq [edi+8*eax+64],xmm4
  movntdq [edi+8*eax+80],xmm5
  movntdq [edi+8*eax+96],xmm6
  movntdq [edi+8*eax+112],xmm7
  add     eax,16
  js      @LargeUnalignedLoop
  sfence
  rep     movsd            {Move Remaining Bytes}
  pop     edi
  pop     esi
  ret
  nop {Align}
@LargeUnaligned:
@LargeUnalignedLoop:
  prefetchnta  [esi+8*eax+Prefetch]
  prefetchnta  [esi+8*eax+Prefetch+64]
  movdqu  xmm0,[esi+8*eax]
  movdqu  xmm1,[esi+8*eax+16]
  movdqu  xmm2,[esi+8*eax+32]
  movdqu  xmm3,[esi+8*eax+48]
  movdqu  xmm4,[esi+8*eax+64]
  movdqu  xmm5,[esi+8*eax+80]
  movdqu  xmm6,[esi+8*eax+96]
  movdqu  xmm7,[esi+8*eax+112]
  movntdq [edi+8*eax],xmm0
  movntdq [edi+8*eax+16],xmm1
  movntdq [edi+8*eax+32],xmm2
  movntdq [edi+8*eax+48],xmm3
  movntdq [edi+8*eax+64],xmm4
  movntdq [edi+8*eax+80],xmm5
  movntdq [edi+8*eax+96],xmm6
  movntdq [edi+8*eax+112],xmm7
  add     eax,16
  js      @LargeUnalignedLoop
  sfence
  rep     movsd            {Move Remaining Bytes}
  pop     edi
  pop     esi
end; {AlignedFwdMoveSSE2}

{---------------------------------------------------------------------------
---}
procedure AlignedFwdMove(const Source; var Dest; Count: Integer); {Dest is
16 Byte Aligned and Count MOD 16 = 0}
asm
  cmp   EnableSSE2, 0
  jnz   AlignedFwdMoveSSE2
@NoSSE2:
  cmp   Enable3DNow,0
  jz    @CheckMMX
{Note - Still Need to Detect/Use 3DNow! Moves}
{Currently use FPU Based Move in preference to MMX on Athlon}
@FPUMove:
  lea   eax,[eax+ecx]
  lea   edx,[edx+ecx]
  neg   ecx
@FPULoop:
  fild  QWORD PTR [eax+ecx]
  fistp QWORD PTR [edx+ecx]
  fild  QWORD PTR [eax+ecx+8]
  fistp QWORD PTR [edx+ecx+8]
  add   ecx,16
  jnz   @FPULoop
  ret
@CheckMMX:
  cmp   EnableMMX,0
  jnz   AlignedFwdMoveMMX
@SimpleMove:
  push  edi
  push  esi
  shr   ecx,2
  mov   edi,edx
  mov   esi,eax
  rep   movsd
  pop   esi
  pop   edi
end; {AlignedFwdMove}

{---------------------------------------------------------------------------
---}
procedure kbmMemMove(const Source; var Dest; Count : Integer);
const
  TABLESIZE    =  36;
  LARGESIZE    = 256;
asm
  cmp   eax,edx
  jng   @CheckOverlap
@FwdMove:
  cmp   ecx,TABLESIZE
  jg    @FwdNotSmall
  add   eax,ecx
@FwdMove2:
  cmp   ecx,0
  jle   @Exit {Count <= 0}
  add   edx,ecx
  jmp   dword ptr [@FwdJumpTable+ecx*4]
@Exit:
  ret
@CheckOverlap:
  je    @Exit {Needed for Full Compatibility with Delphi's standard move
procedure for Source=Dest}
  add   eax,ecx
  cmp   eax,edx
  jg    @BwdMove {Source/Dest Overlap}
  {No overlap}
  cmp   ecx,TABLESIZE
  jle   @FwdMove2 {Source already incremented by Count}
  sub   eax,ecx {Restore Original Source}
@FwdNotSmall:
  cmp   ecx,LARGESIZE
  jge   @FwdLargeMove
  {Count > TABLESIZE and Count < LARGESIZE}
  cmp   ecx, 48
  jl    @FwdMoveNonMMX
  cmp   EnableMMX,0
  jz   @FwdMoveNonMMX
@FwdMoveMMX:
  push  ebx
  mov   ebx,edx
  movq  mm0,[eax] {First 8 Characters}
  add   ecx,eax
  sub   edx,eax
  add   eax,7
  and   eax,-8
  sub   ecx,eax
  add   edx,eax
  {Reads are now QWORD Aligned}
  add   eax,ecx
  add   edx,ecx
  sub   ecx,32
  neg   ecx
@FwdLoopMMX:
  movq  mm1,[eax+ecx-32]
  movq  [edx+ecx-32],mm1
  movq  mm1,[eax+ecx-24]
  movq  [edx+ecx-24],mm1
  movq  mm1,[eax+ecx-16]
  movq  [edx+ecx-16],mm1
  movq  mm1,[eax+ecx-8]
  movq  [edx+ecx-8],mm1
  add   ecx,32
  jle   @FwdLoopMMX
  movq  [ebx],mm0 {First 8 Characters}
  emms
  pop   ebx
  neg   ecx
  add   ecx,32
  jmp   dword ptr [@FwdJumpTable+ecx*4]
@FwdMoveNonMMX:
  push  ebx
  push  [eax]
  push  edx
  {DWORD Align Reads}
  add   ecx,eax
  sub   edx,eax
  add   eax,3
  and   eax,-4
  sub   ecx,eax
  add   edx,eax
  {Reads are now DWORD Aligned}
  add   eax,ecx
  add   edx,ecx
  sub   ecx,32
  neg   ecx
@FwdLoop:
  mov   ebx,[eax+ecx-32]
  mov   [edx+ecx-32],ebx
  mov   ebx,[eax+ecx-28]
  mov   [edx+ecx-28],ebx
  mov   ebx,[eax+ecx-24]
  mov   [edx+ecx-24],ebx
  mov   ebx,[eax+ecx-20]
  mov   [edx+ecx-20],ebx
  mov   ebx,[eax+ecx-16]
  mov   [edx+ecx-16],ebx
  mov   ebx,[eax+ecx-12]
  mov   [edx+ecx-12],ebx
  mov   ebx,[eax+ecx-8]
  mov   [edx+ecx-8],ebx
  mov   ebx,[eax+ecx-4]
  mov   [edx+ecx-4],ebx
  add   ecx,32
  jle   @FwdLoop
  pop   ebx {Orig EDX}
  pop   [ebx]
  neg   ecx
  add   ecx,32
  pop   ebx
  jmp   dword ptr [@FwdJumpTable+ecx*4]
@FwdLargeMove:
  push  ebx
  {16 byte Align Destination}
  mov   ebx,ecx
  mov   ecx,edx
  add   ecx,15
  and   ecx,-16
  push  ecx
  sub   ecx,edx
  add   eax,ecx
  add   edx,ecx
  sub   ebx,ecx
  call  dword ptr [@FwdJumpTable+ecx*4]
  {Destination now 16 Byte Aligned}
@FwdAligned:
  mov   ecx,ebx
  and   ecx,-16
  sub   ebx,ecx {EBX = Remainder}
  push  eax
  push  ecx
  call  AlignedFwdMove
  pop   ecx
  pop   eax
  pop   edx
  add   ecx,ebx
  add   eax,ecx
  add   edx,ecx
  lea   ecx,[@FwdJumpTable+ebx*4]
  pop   ebx
  jmp   [ecx]
@BwdMove: {Overlapping Source/Dest}
  sub   eax,ecx {Restore Original Source}
  cmp   ecx,TABLESIZE
  jg    @BwdNotSmall
  jmp   dword ptr [@BwdJumpTable+ecx*4]
@BwdNotSmall:
  push  esi
  push  edi
  push  ebx
  mov   esi,ecx
  mov   edi,edx
  sub   edi,eax
  cmp   edi,LARGESIZE {Dest-Source}
  jle   @BwdRevMove
  sub   esi,edi
  mov   ecx,edi
  mov   edi,edx
  mov   ebx,eax
  add   eax,esi
  add   edx,esi
  call  @FwdLargeMove
  mov   eax,ebx
  mov   edx,edi
  mov   ecx,esi
  cmp   ecx,TABLESIZE
  jle   @BwdTinyMove
  {Count > TABLESIZE and Count < LARGESIZE}
@BwdRevMove:
  cmp   ecx, 48
  jl    @BwdRevMoveNonMMX
  cmp   EnableMMX,0
  jz    @BwdRevMoveNonMMX
@BwdRevMoveMMX:
  movq  mm0,[eax+ecx-8] {Get Last QWORD}
  {QWORD Align Writes}
  lea   ebx,[edx+ecx]
  and   ebx,7
  sub   ecx,ebx
  {Writes are now QWORD Aligned}
  sub   ecx,32
@BwdLoopMMX:
  movq  mm1,[eax+ecx+24]
  movq  [edx+ecx+24], mm1
  movq  mm1,[eax+ecx+16]
  movq  [edx+ecx+16], mm1
  movq  mm1,[eax+ecx+8]
  movq  [edx+ecx+8], mm1
  movq  mm1,[eax+ecx]
  movq  [edx+ecx], mm1
  sub   ecx,32
  jge   @BwdLoopMMX
  movq  [edx+esi-8], mm0 {Last QWORD}
  emms
  add   ecx,32
  pop   ebx
  pop   edi
  pop   esi
  jmp   dword ptr [@BwdJumpTable+ecx*4]
@BwdRevMoveNonMMX:
  mov   edi,[eax+ecx-4] {Get Last DWORD}
  {DWORD Align Writes}
  lea   ebx,[edx+ecx]
  and   ebx,3
  sub   ecx,ebx
  {Writes are now DWORD Aligned}
  sub   ecx,32
@BwdLoop:
  mov   ebx,[eax+ecx+28]
  mov   [edx+ecx+28],ebx
  mov   ebx,[eax+ecx+24]
  mov   [edx+ecx+24],ebx
  mov   ebx,[eax+ecx+20]
  mov   [edx+ecx+20],ebx
  mov   ebx,[eax+ecx+16]
  mov   [edx+ecx+16],ebx
  mov   ebx,[eax+ecx+12]
  mov   [edx+ecx+12],ebx
  mov   ebx,[eax+ecx+8]
  mov   [edx+ecx+8],ebx
  mov   ebx,[eax+ecx+4]
  mov   [edx+ecx+4],ebx
  mov   ebx,[eax+ecx]
  mov   [edx+ecx],ebx
  sub   ecx,32
  jge   @BwdLoop
  add   ecx,32
  mov   [edx+esi-4],edi {Last DWORD}
@BwdTinyMove:
  pop   ebx
  pop   edi
  pop   esi
  jmp   dword ptr [@BwdJumpTable+ecx*4]
  nop {Align Jump Tables}
  nop
@FwdJumpTable:
  dd    @Fwd00 {Removes need to test for zero size move}
  dd    @Fwd01,@Fwd02,@Fwd03,@Fwd04,@Fwd05,@Fwd06,@Fwd07,@Fwd08
  dd    @Fwd09,@Fwd10,@Fwd11,@Fwd12,@Fwd13,@Fwd14,@Fwd15,@Fwd16
  dd    @Fwd17,@Fwd18,@Fwd19,@Fwd20,@Fwd21,@Fwd22,@Fwd23,@Fwd24
  dd    @Fwd25,@Fwd26,@Fwd27,@Fwd28,@Fwd29,@Fwd30,@Fwd31,@Fwd32
  dd    @Fwd33,@Fwd34,@Fwd35,@Fwd36
@BwdJumpTable:
  dd    @Bwd00 {Removes need to test for zero size move}
  dd    @Bwd01,@Bwd02,@Bwd03,@Bwd04,@Bwd05,@Bwd06,@Bwd07,@Bwd08
  dd    @Bwd09,@Bwd10,@Bwd11,@Bwd12,@Bwd13,@Bwd14,@Bwd15,@Bwd16
  dd    @Bwd17,@Bwd18,@Bwd19,@Bwd20,@Bwd21,@Bwd22,@Bwd23,@Bwd24
  dd    @Bwd25,@Bwd26,@Bwd27,@Bwd28,@Bwd29,@Bwd30,@Bwd31,@Bwd32
  dd    @Bwd33,@Bwd34,@Bwd35,@Bwd36
@Fwd36:
  mov   ecx,[eax-36]
  mov   [edx-36],ecx
  jmp   @Fwd32
@Fwd35:
  mov   cl,[eax-35]
  mov   [edx-35],cl
  mov   cl,[eax-34]
  mov   [edx-34],cl
  mov   cl,[eax-33]
  mov   [edx-33],cl
  jmp   @Fwd32
@Fwd34:
  mov   cx,[eax-34]
  mov   [edx-34],cx
  jmp   @Fwd32
  nop
  nop
@Fwd33:
  mov   cl,[eax-33]
  mov   [edx-33],cl
  nop
  nop
@Fwd32:
  mov   ecx,[eax-32]
  mov   [edx-32],ecx
  mov   ecx,[eax-28]
  mov   [edx-28],ecx
  mov   ecx,[eax-24]
  mov   [edx-24],ecx
  mov   ecx,[eax-20]
  mov   [edx-20],ecx
  mov   ecx,[eax-16]
  mov   [edx-16],ecx
  mov   ecx,[eax-12]
  mov   [edx-12],ecx
  mov   ecx,[eax-8]
  mov   [edx-8],ecx
  mov   ecx,[eax-4]
  mov   [edx-4],ecx
  ret
  nop
  nop
  nop
@Fwd31:
  mov   ecx,[eax-31]
  mov   [edx-31],ecx
  mov   ecx,[eax-27]
  mov   [edx-27],ecx
  mov   ecx,[eax-23]
  mov   [edx-23],ecx
  mov   ecx,[eax-19]
  mov   [edx-19],ecx
  mov   ecx,[eax-15]
  mov   [edx-15],ecx
  mov   ecx,[eax-11]
  mov   [edx-11],ecx
  mov   ecx,[eax-7]
  mov   [edx-7],ecx
  mov   cx,[eax-3]
  mov   [edx-3],cx
  mov   cl,[eax-1]
  mov   [edx-1],cl
  ret
  nop
  nop
  nop
@Fwd30:
  mov   ecx,[eax-30]
  mov   [edx-30],ecx
  mov   ecx,[eax-26]
  mov   [edx-26],ecx
  mov   ecx,[eax-22]
  mov   [edx-22],ecx
  mov   ecx,[eax-18]
  mov   [edx-18],ecx
  mov   ecx,[eax-14]
  mov   [edx-14],ecx
  mov   ecx,[eax-10]
  mov   [edx-10],ecx
  mov   ecx,[eax-6]
  mov   [edx-6],ecx
  mov   cx,[eax-2]
  mov   [edx-2],cx
  ret
  nop
@Fwd29:
  mov   ecx,[eax-29]
  mov   [edx-29],ecx
  mov   ecx,[eax-25]
  mov   [edx-25],ecx
  mov   ecx,[eax-21]
  mov   [edx-21],ecx
  mov   ecx,[eax-17]
  mov   [edx-17],ecx
  mov   ecx,[eax-13]
  mov   [edx-13],ecx
  mov   ecx,[eax-9]
  mov   [edx-9],ecx
  mov   ecx,[eax-5]
  mov   [edx-5],ecx
  mov   cl,[eax-1]
  mov   [edx-1],cl
  ret
  nop
  nop
  nop
@Fwd28:
  mov   ecx,[eax-28]
  mov   [edx-28],ecx
  mov   ecx,[eax-24]
  mov   [edx-24],ecx
  mov   ecx,[eax-20]
  mov   [edx-20],ecx
  mov   ecx,[eax-16]
  mov   [edx-16],ecx
  mov   ecx,[eax-12]
  mov   [edx-12],ecx
  mov   ecx,[eax-8]
  mov   [edx-8],ecx
  mov   ecx,[eax-4]
  mov   [edx-4],ecx
  ret
  nop
@Fwd27:
  mov   ecx,[eax-27]
  mov   [edx-27],ecx
  mov   ecx,[eax-23]
  mov   [edx-23],ecx
  mov   ecx,[eax-19]
  mov   [edx-19],ecx
  mov   ecx,[eax-15]
  mov   [edx-15],ecx
  mov   ecx,[eax-11]
  mov   [edx-11],ecx
  mov   ecx,[eax-7]
  mov   [edx-7],ecx
  mov   cx,[eax-3]
  mov   [edx-3],cx
  mov   cl,[eax-1]
  mov   [edx-1],cl
  ret
  nop
@Fwd26:
  mov   ecx,[eax-26]
  mov   [edx-26],ecx
  mov   ecx,[eax-22]
  mov   [edx-22],ecx
  mov   ecx,[eax-18]
  mov   [edx-18],ecx
  mov   ecx,[eax-14]
  mov   [edx-14],ecx
  mov   ecx,[eax-10]
  mov   [edx-10],ecx
  mov   ecx,[eax-6]
  mov   [edx-6],ecx
  mov   cx,[eax-2]
  mov   [edx-2],cx
  ret
  nop
  nop
  nop
@Fwd25:
  mov   ecx,[eax-25]
  mov   [edx-25],ecx
  mov   ecx,[eax-21]
  mov   [edx-21],ecx
  mov   ecx,[eax-17]
  mov   [edx-17],ecx
  mov   ecx,[eax-13]
  mov   [edx-13],ecx
  mov   ecx,[eax-9]
  mov   [edx-9],ecx
  mov   ecx,[eax-5]
  mov   [edx-5],ecx
  mov   cl,[eax-1]
  mov   [edx-1],cl
  ret
  nop
@Fwd24:
  mov   ecx,[eax-24]
  mov   [edx-24],ecx
  mov   ecx,[eax-20]
  mov   [edx-20],ecx
  mov   ecx,[eax-16]
  mov   [edx-16],ecx
  mov   ecx,[eax-12]
  mov   [edx-12],ecx
  mov   ecx,[eax-8]
  mov   [edx-8],ecx
  mov   ecx,[eax-4]
  mov   [edx-4],ecx
  ret
  nop
  nop
  nop
@Fwd23:
  mov   ecx,[eax-23]
  mov   [edx-23],ecx
  mov   ecx,[eax-19]
  mov   [edx-19],ecx
  mov   ecx,[eax-15]
  mov   [edx-15],ecx
  mov   ecx,[eax-11]
  mov   [edx-11],ecx
  mov   ecx,[eax-7]
  mov   [edx-7],ecx
  mov   cx,[eax-3]
  mov   [edx-3],cx
  mov   cl,[eax-1]
  mov   [edx-1],cl
  ret
  nop
  nop
  nop
@Fwd22:
  mov   ecx,[eax-22]
  mov   [edx-22],ecx
  mov   ecx,[eax-18]
  mov   [edx-18],ecx
  mov   ecx,[eax-14]
  mov   [edx-14],ecx
  mov   ecx,[eax-10]
  mov   [edx-10],ecx
  mov   ecx,[eax-6]
  mov   [edx-6],ecx
  mov   cx,[eax-2]
  mov   [edx-2],cx
  ret
  nop
@Fwd21:
  mov   ecx,[eax-21]
  mov   [edx-21],ecx
  mov   ecx,[eax-17]
  mov   [edx-17],ecx
  mov   ecx,[eax-13]
  mov   [edx-13],ecx
  mov   ecx,[eax-9]
  mov   [edx-9],ecx
  mov   ecx,[eax-5]
  mov   [edx-5],ecx
  mov   cl,[eax-1]
  mov   [edx-1],cl
  ret
  nop
  nop
  nop
@Fwd20:
  mov   ecx,[eax-20]
  mov   [edx-20],ecx
  mov   ecx,[eax-16]
  mov   [edx-16],ecx
  mov   ecx,[eax-12]
  mov   [edx-12],ecx
  mov   ecx,[eax-8]
  mov   [edx-8],ecx
  mov   ecx,[eax-4]
  mov   [edx-4],ecx
  ret
  nop
@Fwd19:
  mov   ecx,[eax-19]
  mov   [edx-19],ecx
  mov   ecx,[eax-15]
  mov   [edx-15],ecx
  mov   ecx,[eax-11]
  mov   [edx-11],ecx
  mov   ecx,[eax-7]
  mov   [edx-7],ecx
  mov   cx,[eax-3]
  mov   [edx-3],cx
  mov   cl,[eax-1]
  mov   [edx-1],cl
  ret
  nop
@Fwd18:
  mov   ecx,[eax-18]
  mov   [edx-18],ecx
  mov   ecx,[eax-14]
  mov   [edx-14],ecx
  mov   ecx,[eax-10]
  mov   [edx-10],ecx
  mov   ecx,[eax-6]
  mov   [edx-6],ecx
  mov   cx,[eax-2]
  mov   [edx-2],cx
  ret
  nop
  nop
  nop
@Fwd17:
  mov   ecx,[eax-17]
  mov   [edx-17],ecx
  mov   ecx,[eax-13]
  mov   [edx-13],ecx
  mov   ecx,[eax-9]
  mov   [edx-9],ecx
  mov   ecx,[eax-5]
  mov   [edx-5],ecx
  mov   cl,[eax-1]
  mov   [edx-1],cl
  ret
  nop
@Fwd16:
  mov   ecx,[eax-16]
  mov   [edx-16],ecx
  mov   ecx,[eax-12]
  mov   [edx-12],ecx
  mov   ecx,[eax-8]
  mov   [edx-8],ecx
  mov   ecx,[eax-4]
  mov   [edx-4],ecx
  ret
  nop
  nop
  nop
@Fwd15:
  mov   ecx,[eax-15]
  mov   [edx-15],ecx
  mov   ecx,[eax-11]
  mov   [edx-11],ecx
  mov   ecx,[eax-7]
  mov   [edx-7],ecx
  mov   cx,[eax-3]
  mov   [edx-3],cx
  mov   cl,[eax-1]
  mov   [edx-1],cl
  ret
  nop
  nop
  nop
@Fwd14:
  mov   ecx,[eax-14]
  mov   [edx-14],ecx
  mov   ecx,[eax-10]
  mov   [edx-10],ecx
  mov   ecx,[eax-6]
  mov   [edx-6],ecx
  mov   cx,[eax-2]
  mov   [edx-2],cx
  ret
  nop
@Fwd13:
  mov   ecx,[eax-13]
  mov   [edx-13],ecx
  mov   ecx,[eax-9]
  mov   [edx-9],ecx
  mov   ecx,[eax-5]
  mov   [edx-5],ecx
  mov   cl,[eax-1]
  mov   [edx-1],cl
  ret
  nop
  nop
  nop
@Fwd12:
  mov   ecx,[eax-12]
  mov   [edx-12],ecx
  mov   ecx,[eax-8]
  mov   [edx-8],ecx
  mov   ecx,[eax-4]
  mov   [edx-4],ecx
  ret
  nop
@Fwd11:
  mov   ecx,[eax-11]
  mov   [edx-11],ecx
  mov   ecx,[eax-7]
  mov   [edx-7],ecx
  mov   cx,[eax-3]
  mov   [edx-3],cx
  mov   cl,[eax-1]
  mov   [edx-1],cl
  ret
  nop
@Fwd10:
  mov   ecx,[eax-10]
  mov   [edx-10],ecx
  mov   ecx,[eax-6]
  mov   [edx-6],ecx
  mov   cx,[eax-2]
  mov   [edx-2],cx
  ret
  nop
  nop
  nop
@Fwd09:
  mov   ecx,[eax-9]
  mov   [edx-9],ecx
  mov   ecx,[eax-5]
  mov   [edx-5],ecx
  mov   cl,[eax-1]
  mov   [edx-1],cl
  ret
  nop
@Fwd08:
  mov   ecx,[eax-8]
  mov   [edx-8],ecx
  mov   ecx,[eax-4]
  mov   [edx-4],ecx
  ret
  nop
  nop
  nop
@Fwd07:
  mov   ecx,[eax-7]
  mov   [edx-7],ecx
  mov   cx,[eax-3]
  mov   [edx-3],cx
  mov   cl,[eax-1]
  mov   [edx-1],cl
  ret
  nop
  nop
  nop
@Fwd06:
  mov   ecx,[eax-6]
  mov   [edx-6],ecx
  mov   cx,[eax-2]
  mov   [edx-2],cx
  ret
  nop
@Fwd05:
  mov   ecx,[eax-5]
  mov   [edx-5],ecx
  mov   cl,[eax-1]
  mov   [edx-1],cl
  ret
  nop
  nop
  nop
@Fwd04:
  mov   ecx,[eax-4]
  mov   [edx-4],ecx
  ret
  nop
@Fwd03:
  mov   cx,[eax-3]
  mov   [edx-3],cx
  mov   cl,[eax-1]
  mov   [edx-1],cl
  ret
  nop
@Fwd02:
  mov   cx,[eax-2]
  mov   [edx-2],cx
  ret
  nop
  nop
  nop
@Fwd01:
  mov   cl,[eax-1]
  mov   [edx-1],cl
  nop
  nop
@Fwd00:
  ret
  nop
  nop
  nop
@Bwd36:
  mov   ecx,[eax+32]
  mov   [edx+32],ecx
  jmp   @Bwd32
@Bwd35:
  mov   cl,[eax+34]
  mov   [edx+34],cl
  mov   cl,[eax+33]
  mov   [edx+33],cl
  mov   cl,[eax+32]
  mov   [edx+32],cl
  jmp   @Bwd32
@Bwd34:
  mov   cx,[eax+32]
  mov   [edx+32],cx
  jmp   @Bwd32
  nop
  nop
@Bwd33:
  mov   cl,[eax+32]
  mov   [edx+32],cl
  nop
  nop
@Bwd32:
  mov   ecx,[eax+28]
  mov   [edx+28],ecx
  mov   ecx,[eax+24]
  mov   [edx+24],ecx
  mov   ecx,[eax+20]
  mov   [edx+20],ecx
  mov   ecx,[eax+16]
  mov   [edx+16],ecx
  mov   ecx,[eax+12]
  mov   [edx+12],ecx
  mov   ecx,[eax+8]
  mov   [edx+8],ecx
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
@Bwd31:
  mov   cl,[eax+30]
  mov   [edx+30],cl
  mov   cl,[eax+29]
  mov   [edx+29],cl
  mov   cl,[eax+28]
  mov   [edx+28],cl
  mov   ecx,[eax+24]
  mov   [edx+24],ecx
  mov   ecx,[eax+20]
  mov   [edx+20],ecx
  mov   ecx,[eax+16]
  mov   [edx+16],ecx
  mov   ecx,[eax+12]
  mov   [edx+12],ecx
  mov   ecx,[eax+8]
  mov   [edx+8],ecx
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
@Bwd30:
  mov   cl,[eax+29]
  mov   [edx+29],cl
  mov   cl,[eax+28]
  mov   [edx+28],cl
  mov   ecx,[eax+24]
  mov   [edx+24],ecx
  mov   ecx,[eax+20]
  mov   [edx+20],ecx
  mov   ecx,[eax+16]
  mov   [edx+16],ecx
  mov   ecx,[eax+12]
  mov   [edx+12],ecx
  mov   ecx,[eax+8]
  mov   [edx+8],ecx
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
  nop
  nop
@Bwd29:
  mov   cl,[eax+28]
  mov   [edx+28],cl
  mov   ecx,[eax+24]
  mov   [edx+24],ecx
  mov   ecx,[eax+20]
  mov   [edx+20],ecx
  mov   ecx,[eax+16]
  mov   [edx+16],ecx
  mov   ecx,[eax+12]
  mov   [edx+12],ecx
  mov   ecx,[eax+8]
  mov   [edx+8],ecx
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
@Bwd28:
  mov   ecx,[eax+24]
  mov   [edx+24],ecx
  mov   ecx,[eax+20]
  mov   [edx+20],ecx
  mov   ecx,[eax+16]
  mov   [edx+16],ecx
  mov   ecx,[eax+12]
  mov   [edx+12],ecx
  mov   ecx,[eax+8]
  mov   [edx+8],ecx
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
  nop
  nop
@Bwd27:
  mov   cl,[eax+26]
  mov   [edx+26],cl
  mov   cl,[eax+25]
  mov   [edx+25],cl
  mov   cl,[eax+24]
  mov   [edx+24],cl
  mov   ecx,[eax+20]
  mov   [edx+20],ecx
  mov   ecx,[eax+16]
  mov   [edx+16],ecx
  mov   ecx,[eax+12]
  mov   [edx+12],ecx
  mov   ecx,[eax+8]
  mov   [edx+8],ecx
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
  nop
  nop
@Bwd26:
  mov   cl,[eax+25]
  mov   [edx+25],cl
  mov   cl,[eax+24]
  mov   [edx+24],cl
  mov   ecx,[eax+20]
  mov   [edx+20],ecx
  mov   ecx,[eax+16]
  mov   [edx+16],ecx
  mov   ecx,[eax+12]
  mov   [edx+12],ecx
  mov   ecx,[eax+8]
  mov   [edx+8],ecx
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
@Bwd25:
  mov   cl,[eax+24]
  mov   [edx+24],cl
  mov   ecx,[eax+20]
  mov   [edx+20],ecx
  mov   ecx,[eax+16]
  mov   [edx+16],ecx
  mov   ecx,[eax+12]
  mov   [edx+12],ecx
  mov   ecx,[eax+8]
  mov   [edx+8],ecx
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
  nop
  nop
@Bwd24:
  mov   ecx,[eax+20]
  mov   [edx+20],ecx
  mov   ecx,[eax+16]
  mov   [edx+16],ecx
  mov   ecx,[eax+12]
  mov   [edx+12],ecx
  mov   ecx,[eax+8]
  mov   [edx+8],ecx
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
@Bwd23:
  mov   cl,[eax+22]
  mov   [edx+22],cl
  mov   cl,[eax+21]
  mov   [edx+21],cl
  mov   cl,[eax+20]
  mov   [edx+20],cl
  mov   ecx,[eax+16]
  mov   [edx+16],ecx
  mov   ecx,[eax+12]
  mov   [edx+12],ecx
  mov   ecx,[eax+8]
  mov   [edx+8],ecx
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
@Bwd22:
  mov   cl,[eax+21]
  mov   [edx+21],cl
  mov   cl,[eax+20]
  mov   [edx+20],cl
  mov   ecx,[eax+16]
  mov   [edx+16],ecx
  mov   ecx,[eax+12]
  mov   [edx+12],ecx
  mov   ecx,[eax+8]
  mov   [edx+8],ecx
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
  nop
  nop
@Bwd21:
  mov   cl,[eax+20]
  mov   [edx+20],cl
  mov   ecx,[eax+16]
  mov   [edx+16],ecx
  mov   ecx,[eax+12]
  mov   [edx+12],ecx
  mov   ecx,[eax+8]
  mov   [edx+8],ecx
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
@Bwd20:
  mov   ecx,[eax+16]
  mov   [edx+16],ecx
  mov   ecx,[eax+12]
  mov   [edx+12],ecx
  mov   ecx,[eax+8]
  mov   [edx+8],ecx
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
  nop
  nop
@Bwd19:
  mov   cl,[eax+18]
  mov   [edx+18],cl
  mov   cl,[eax+17]
  mov   [edx+17],cl
  mov   cl,[eax+16]
  mov   [edx+16],cl
  mov   ecx,[eax+12]
  mov   [edx+12],ecx
  mov   ecx,[eax+8]
  mov   [edx+8],ecx
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
  nop
  nop
@Bwd18:
  mov   cl,[eax+17]
  mov   [edx+17],cl
  mov   cl,[eax+16]
  mov   [edx+16],cl
  mov   ecx,[eax+12]
  mov   [edx+12],ecx
  mov   ecx,[eax+8]
  mov   [edx+8],ecx
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
@Bwd17:
  mov   cl,[eax+16]
  mov   [edx+16],cl
  mov   ecx,[eax+12]
  mov   [edx+12],ecx
  mov   ecx,[eax+8]
  mov   [edx+8],ecx
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
  nop
  nop
@Bwd16:
  mov   ecx,[eax+12]
  mov   [edx+12],ecx
  mov   ecx,[eax+8]
  mov   [edx+8],ecx
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
@Bwd15:
  mov   cl,[eax+14]
  mov   [edx+14],cl
  mov   cl,[eax+13]
  mov   [edx+13],cl
  mov   cl,[eax+12]
  mov   [edx+12],cl
  mov   ecx,[eax+8]
  mov   [edx+8],ecx
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
@Bwd14:
  mov   cl,[eax+13]
  mov   [edx+13],cl
  mov   cl,[eax+12]
  mov   [edx+12],cl
  mov   ecx,[eax+8]
  mov   [edx+8],ecx
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
  nop
  nop
@Bwd13:
  mov   cl,[eax+12]
  mov   [edx+12],cl
  mov   ecx,[eax+8]
  mov   [edx+8],ecx
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
@Bwd12:
  mov   ecx,[eax+8]
  mov   [edx+8],ecx
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
  nop
  nop
@Bwd11:
  mov   cl,[eax+10]
  mov   [edx+10],cl
  mov   cl,[eax+9]
  mov   [edx+9],cl
  mov   cl,[eax+8]
  mov   [edx+8],cl
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
  nop
  nop
@Bwd10:
  mov   cl,[eax+9]
  mov   [edx+9],cl
  mov   cl,[eax+8]
  mov   [edx+8],cl
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
@Bwd09:
  mov   cl,[eax+8]
  mov   [edx+8],cl
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
  nop
  nop
@Bwd08:
  mov   ecx,[eax+4]
  mov   [edx+4],ecx
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
@Bwd07:
  mov   cl,[eax+6]
  mov   [edx+6],cl
  mov   cl,[eax+5]
  mov   [edx+5],cl
  mov   cl,[eax+4]
  mov   [edx+4],cl
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
@Bwd06:
  mov   cl,[eax+5]
  mov   [edx+5],cl
  mov   cl,[eax+4]
  mov   [edx+4],cl
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
  nop
  nop
@Bwd05:
  mov   cl,[eax+4]
  mov   [edx+4],cl
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
@Bwd04:
  mov   ecx,[eax]
  mov   [edx],ecx
  ret
  nop
  nop
  nop
@Bwd03:
  mov   cl,[eax+2]
  mov   [edx+2],cl
  mov   cl,[eax+1]
  mov   [edx+1],cl
  mov   cl,[eax]
  mov   [edx],cl
  ret
  nop
  nop
  nop
@Bwd02:
  mov   cx,[eax]
  mov   [edx],cx
  ret
  nop
@Bwd01:
  mov   cl,[eax]
  mov   [edx],cl
@Bwd00:
end; {MoveJOH4}

initialization
  GetCPUFeatures;

 {$ENDIF}
{$ENDIF}

end.

