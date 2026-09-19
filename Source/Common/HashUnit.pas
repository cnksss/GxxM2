unit HashUnit;

interface
uses
  Classes, Sysutils;

procedure Hash(Key: string; var Digest);
implementation
type
  dword = longint;

  Pbyte = ^byte;
  Pword = ^word;
  Pdword = ^dword;

  Pwordarray = ^Twordarray;
  Twordarray = array[0..19383] of word;
  Pdwordarray = ^Tdwordarray;
  Tdwordarray = array[0..8191] of dword;

  TCurrentHash = array[0..4] of Dword;
  THashBuffer = array[0..63] of byte;
  PTCurrentHash = ^TCurrentHash;
  PTHashBuffer = ^THashBuffer;

function LRot16(X: Word; c: longint): Word;
begin
  LRot16 := (X shl c) or (X shr (16 - c));
end;

function RRot16(X: Word; c: longint): Word;
begin
  RRot16 := (X shr c) or (X shl (16 - c));
end;

function LRot32(X: DWord; c: longint): DWord;
begin
  LRot32 := (X shl c) or (X shr (32 - c));
end;

function RRot32(X: DWord; c: longint): DWord;
begin
  RRot32 := (X shr c) or (X shl (32 - c));
end;

function SwapDWord(X: DWord): DWord;
begin
  Result := (X shr 24) or ((X shr 8) and $FF00) or ((X shl 8) and $FF0000) or (X shl 24);
end;

procedure Compress(CurrentHash: PTCurrentHash; HashBuffer: PTHashBuffer);
var
  A, B, C, D, E: DWord;
  W: array[0..79] of DWord;
  I: longint;
begin
  Move(HashBuffer^, W, Sizeof(THashBuffer));
  for i := 0 to 15 do
    W[i] := SwapDWord(W[i]);
  for i := 16 to 79 do
    W[i] := ((W[i - 3] xor W[i - 8] xor W[i - 14] xor W[i - 16]) shl 1) or ((W[i - 3] xor W[i - 8] xor W[i - 14] xor W[i - 16]) shr 31);
  A := CurrentHash[0]; B := CurrentHash[1]; C := CurrentHash[2]; D := CurrentHash[3]; E := CurrentHash[4];

  Inc(E, ((A shl 5) or (A shr 27)) + (D xor (B and (C xor D))) + $5A827999 + W[0]); B := (B shl 30) or (B shr 2);
  Inc(D, ((E shl 5) or (E shr 27)) + (C xor (A and (B xor C))) + $5A827999 + W[1]); A := (A shl 30) or (A shr 2);
  Inc(C, ((D shl 5) or (D shr 27)) + (B xor (E and (A xor B))) + $5A827999 + W[2]); E := (E shl 30) or (E shr 2);
  Inc(B, ((C shl 5) or (C shr 27)) + (A xor (D and (E xor A))) + $5A827999 + W[3]); D := (D shl 30) or (D shr 2);
  Inc(A, ((B shl 5) or (B shr 27)) + (E xor (C and (D xor E))) + $5A827999 + W[4]); C := (C shl 30) or (C shr 2);
  Inc(E, ((A shl 5) or (A shr 27)) + (D xor (B and (C xor D))) + $5A827999 + W[5]); B := (B shl 30) or (B shr 2);
  Inc(D, ((E shl 5) or (E shr 27)) + (C xor (A and (B xor C))) + $5A827999 + W[6]); A := (A shl 30) or (A shr 2);
  Inc(C, ((D shl 5) or (D shr 27)) + (B xor (E and (A xor B))) + $5A827999 + W[7]); E := (E shl 30) or (E shr 2);
  Inc(B, ((C shl 5) or (C shr 27)) + (A xor (D and (E xor A))) + $5A827999 + W[8]); D := (D shl 30) or (D shr 2);
  Inc(A, ((B shl 5) or (B shr 27)) + (E xor (C and (D xor E))) + $5A827999 + W[9]); C := (C shl 30) or (C shr 2);
  Inc(E, ((A shl 5) or (A shr 27)) + (D xor (B and (C xor D))) + $5A827999 + W[10]); B := (B shl 30) or (B shr 2);
  Inc(D, ((E shl 5) or (E shr 27)) + (C xor (A and (B xor C))) + $5A827999 + W[11]); A := (A shl 30) or (A shr 2);
  Inc(C, ((D shl 5) or (D shr 27)) + (B xor (E and (A xor B))) + $5A827999 + W[12]); E := (E shl 30) or (E shr 2);
  Inc(B, ((C shl 5) or (C shr 27)) + (A xor (D and (E xor A))) + $5A827999 + W[13]); D := (D shl 30) or (D shr 2);
  Inc(A, ((B shl 5) or (B shr 27)) + (E xor (C and (D xor E))) + $5A827999 + W[14]); C := (C shl 30) or (C shr 2);
  Inc(E, ((A shl 5) or (A shr 27)) + (D xor (B and (C xor D))) + $5A827999 + W[15]); B := (B shl 30) or (B shr 2);
  Inc(D, ((E shl 5) or (E shr 27)) + (C xor (A and (B xor C))) + $5A827999 + W[16]); A := (A shl 30) or (A shr 2);
  Inc(C, ((D shl 5) or (D shr 27)) + (B xor (E and (A xor B))) + $5A827999 + W[17]); E := (E shl 30) or (E shr 2);
  Inc(B, ((C shl 5) or (C shr 27)) + (A xor (D and (E xor A))) + $5A827999 + W[18]); D := (D shl 30) or (D shr 2);
  Inc(A, ((B shl 5) or (B shr 27)) + (E xor (C and (D xor E))) + $5A827999 + W[19]); C := (C shl 30) or (C shr 2);

  Inc(E, ((A shl 5) or (A shr 27)) + (B xor C xor D) + $6ED9EBA1 + W[20]); B := (B shl 30) or (B shr 2);
  Inc(D, ((E shl 5) or (E shr 27)) + (A xor B xor C) + $6ED9EBA1 + W[21]); A := (A shl 30) or (A shr 2);
  Inc(C, ((D shl 5) or (D shr 27)) + (E xor A xor B) + $6ED9EBA1 + W[22]); E := (E shl 30) or (E shr 2);
  Inc(B, ((C shl 5) or (C shr 27)) + (D xor E xor A) + $6ED9EBA1 + W[23]); D := (D shl 30) or (D shr 2);
  Inc(A, ((B shl 5) or (B shr 27)) + (C xor D xor E) + $6ED9EBA1 + W[24]); C := (C shl 30) or (C shr 2);
  Inc(E, ((A shl 5) or (A shr 27)) + (B xor C xor D) + $6ED9EBA1 + W[25]); B := (B shl 30) or (B shr 2);
  Inc(D, ((E shl 5) or (E shr 27)) + (A xor B xor C) + $6ED9EBA1 + W[26]); A := (A shl 30) or (A shr 2);
  Inc(C, ((D shl 5) or (D shr 27)) + (E xor A xor B) + $6ED9EBA1 + W[27]); E := (E shl 30) or (E shr 2);
  Inc(B, ((C shl 5) or (C shr 27)) + (D xor E xor A) + $6ED9EBA1 + W[28]); D := (D shl 30) or (D shr 2);
  Inc(A, ((B shl 5) or (B shr 27)) + (C xor D xor E) + $6ED9EBA1 + W[29]); C := (C shl 30) or (C shr 2);
  Inc(E, ((A shl 5) or (A shr 27)) + (B xor C xor D) + $6ED9EBA1 + W[30]); B := (B shl 30) or (B shr 2);
  Inc(D, ((E shl 5) or (E shr 27)) + (A xor B xor C) + $6ED9EBA1 + W[31]); A := (A shl 30) or (A shr 2);
  Inc(C, ((D shl 5) or (D shr 27)) + (E xor A xor B) + $6ED9EBA1 + W[32]); E := (E shl 30) or (E shr 2);
  Inc(B, ((C shl 5) or (C shr 27)) + (D xor E xor A) + $6ED9EBA1 + W[33]); D := (D shl 30) or (D shr 2);
  Inc(A, ((B shl 5) or (B shr 27)) + (C xor D xor E) + $6ED9EBA1 + W[34]); C := (C shl 30) or (C shr 2);
  Inc(E, ((A shl 5) or (A shr 27)) + (B xor C xor D) + $6ED9EBA1 + W[35]); B := (B shl 30) or (B shr 2);
  Inc(D, ((E shl 5) or (E shr 27)) + (A xor B xor C) + $6ED9EBA1 + W[36]); A := (A shl 30) or (A shr 2);
  Inc(C, ((D shl 5) or (D shr 27)) + (E xor A xor B) + $6ED9EBA1 + W[37]); E := (E shl 30) or (E shr 2);
  Inc(B, ((C shl 5) or (C shr 27)) + (D xor E xor A) + $6ED9EBA1 + W[38]); D := (D shl 30) or (D shr 2);
  Inc(A, ((B shl 5) or (B shr 27)) + (C xor D xor E) + $6ED9EBA1 + W[39]); C := (C shl 30) or (C shr 2);

  Inc(E, ((A shl 5) or (A shr 27)) + ((B and C) or (D and (B or C))) + $8F1BBCDC + W[40]); B := (B shl 30) or (B shr 2);
  Inc(D, ((E shl 5) or (E shr 27)) + ((A and B) or (C and (A or B))) + $8F1BBCDC + W[41]); A := (A shl 30) or (A shr 2);
  Inc(C, ((D shl 5) or (D shr 27)) + ((E and A) or (B and (E or A))) + $8F1BBCDC + W[42]); E := (E shl 30) or (E shr 2);
  Inc(B, ((C shl 5) or (C shr 27)) + ((D and E) or (A and (D or E))) + $8F1BBCDC + W[43]); D := (D shl 30) or (D shr 2);
  Inc(A, ((B shl 5) or (B shr 27)) + ((C and D) or (E and (C or D))) + $8F1BBCDC + W[44]); C := (C shl 30) or (C shr 2);
  Inc(E, ((A shl 5) or (A shr 27)) + ((B and C) or (D and (B or C))) + $8F1BBCDC + W[45]); B := (B shl 30) or (B shr 2);
  Inc(D, ((E shl 5) or (E shr 27)) + ((A and B) or (C and (A or B))) + $8F1BBCDC + W[46]); A := (A shl 30) or (A shr 2);
  Inc(C, ((D shl 5) or (D shr 27)) + ((E and A) or (B and (E or A))) + $8F1BBCDC + W[47]); E := (E shl 30) or (E shr 2);
  Inc(B, ((C shl 5) or (C shr 27)) + ((D and E) or (A and (D or E))) + $8F1BBCDC + W[48]); D := (D shl 30) or (D shr 2);
  Inc(A, ((B shl 5) or (B shr 27)) + ((C and D) or (E and (C or D))) + $8F1BBCDC + W[49]); C := (C shl 30) or (C shr 2);
  Inc(E, ((A shl 5) or (A shr 27)) + ((B and C) or (D and (B or C))) + $8F1BBCDC + W[50]); B := (B shl 30) or (B shr 2);
  Inc(D, ((E shl 5) or (E shr 27)) + ((A and B) or (C and (A or B))) + $8F1BBCDC + W[51]); A := (A shl 30) or (A shr 2);
  Inc(C, ((D shl 5) or (D shr 27)) + ((E and A) or (B and (E or A))) + $8F1BBCDC + W[52]); E := (E shl 30) or (E shr 2);
  Inc(B, ((C shl 5) or (C shr 27)) + ((D and E) or (A and (D or E))) + $8F1BBCDC + W[53]); D := (D shl 30) or (D shr 2);
  Inc(A, ((B shl 5) or (B shr 27)) + ((C and D) or (E and (C or D))) + $8F1BBCDC + W[54]); C := (C shl 30) or (C shr 2);
  Inc(E, ((A shl 5) or (A shr 27)) + ((B and C) or (D and (B or C))) + $8F1BBCDC + W[55]); B := (B shl 30) or (B shr 2);
  Inc(D, ((E shl 5) or (E shr 27)) + ((A and B) or (C and (A or B))) + $8F1BBCDC + W[56]); A := (A shl 30) or (A shr 2);
  Inc(C, ((D shl 5) or (D shr 27)) + ((E and A) or (B and (E or A))) + $8F1BBCDC + W[57]); E := (E shl 30) or (E shr 2);
  Inc(B, ((C shl 5) or (C shr 27)) + ((D and E) or (A and (D or E))) + $8F1BBCDC + W[58]); D := (D shl 30) or (D shr 2);
  Inc(A, ((B shl 5) or (B shr 27)) + ((C and D) or (E and (C or D))) + $8F1BBCDC + W[59]); C := (C shl 30) or (C shr 2);

  Inc(E, ((A shl 5) or (A shr 27)) + (B xor C xor D) + $CA62C1D6 + W[60]); B := (B shl 30) or (B shr 2);
  Inc(D, ((E shl 5) or (E shr 27)) + (A xor B xor C) + $CA62C1D6 + W[61]); A := (A shl 30) or (A shr 2);
  Inc(C, ((D shl 5) or (D shr 27)) + (E xor A xor B) + $CA62C1D6 + W[62]); E := (E shl 30) or (E shr 2);
  Inc(B, ((C shl 5) or (C shr 27)) + (D xor E xor A) + $CA62C1D6 + W[63]); D := (D shl 30) or (D shr 2);
  Inc(A, ((B shl 5) or (B shr 27)) + (C xor D xor E) + $CA62C1D6 + W[64]); C := (C shl 30) or (C shr 2);
  Inc(E, ((A shl 5) or (A shr 27)) + (B xor C xor D) + $CA62C1D6 + W[65]); B := (B shl 30) or (B shr 2);
  Inc(D, ((E shl 5) or (E shr 27)) + (A xor B xor C) + $CA62C1D6 + W[66]); A := (A shl 30) or (A shr 2);
  Inc(C, ((D shl 5) or (D shr 27)) + (E xor A xor B) + $CA62C1D6 + W[67]); E := (E shl 30) or (E shr 2);
  Inc(B, ((C shl 5) or (C shr 27)) + (D xor E xor A) + $CA62C1D6 + W[68]); D := (D shl 30) or (D shr 2);
  Inc(A, ((B shl 5) or (B shr 27)) + (C xor D xor E) + $CA62C1D6 + W[69]); C := (C shl 30) or (C shr 2);
  Inc(E, ((A shl 5) or (A shr 27)) + (B xor C xor D) + $CA62C1D6 + W[70]); B := (B shl 30) or (B shr 2);
  Inc(D, ((E shl 5) or (E shr 27)) + (A xor B xor C) + $CA62C1D6 + W[71]); A := (A shl 30) or (A shr 2);
  Inc(C, ((D shl 5) or (D shr 27)) + (E xor A xor B) + $CA62C1D6 + W[72]); E := (E shl 30) or (E shr 2);
  Inc(B, ((C shl 5) or (C shr 27)) + (D xor E xor A) + $CA62C1D6 + W[73]); D := (D shl 30) or (D shr 2);
  Inc(A, ((B shl 5) or (B shr 27)) + (C xor D xor E) + $CA62C1D6 + W[74]); C := (C shl 30) or (C shr 2);
  Inc(E, ((A shl 5) or (A shr 27)) + (B xor C xor D) + $CA62C1D6 + W[75]); B := (B shl 30) or (B shr 2);
  Inc(D, ((E shl 5) or (E shr 27)) + (A xor B xor C) + $CA62C1D6 + W[76]); A := (A shl 30) or (A shr 2);
  Inc(C, ((D shl 5) or (D shr 27)) + (E xor A xor B) + $CA62C1D6 + W[77]); E := (E shl 30) or (E shr 2);
  Inc(B, ((C shl 5) or (C shr 27)) + (D xor E xor A) + $CA62C1D6 + W[78]); D := (D shl 30) or (D shr 2);
  Inc(A, ((B shl 5) or (B shr 27)) + (C xor D xor E) + $CA62C1D6 + W[79]); C := (C shl 30) or (C shr 2);

  CurrentHash[0] := CurrentHash[0] + A;
  CurrentHash[1] := CurrentHash[1] + B;
  CurrentHash[2] := CurrentHash[2] + C;
  CurrentHash[3] := CurrentHash[3] + D;
  CurrentHash[4] := CurrentHash[4] + E;
  // FillChar(W, Sizeof(W), 0);
  // FillChar(HashBuffer^, Sizeof(THashBuffer), 0);
end;

procedure Hash(Key: string; var Digest);
var
  PBuf: ^byte;
  Buffer: PChar;
  Size: longint;
  Index: Integer;
  LenHi: longint;
  LenLo2, LenLo1: word; { annoying fix for D1-3 users who don't have longword }
  CurrentHash: TCurrentHash;
  HashBuffer: THashBuffer;
begin
  LenHi := 0; LenLo1 := 0; LenLo2 := 0;
  Index := 0;
  FillChar(HashBuffer, Sizeof(HashBuffer), 0);
  FillChar(CurrentHash, Sizeof(CurrentHash), 0);

  CurrentHash[0] := $67452301;
  CurrentHash[1] := $EFCDAB89;
  CurrentHash[2] := $98BADCFE;
  CurrentHash[3] := $10325476;
  CurrentHash[4] := $C3D2E1F0;

  Size := Length(Key);
  Inc(LenLo1, (Size shl 3) and $FFFF);
  if LenLo1 < ((Size shl 3) and $FFFF) then
  begin
    Inc(LenLo2);
    if LenLo2 = 0 then
      Inc(LenHi);
  end;
  Inc(LenLo2, Size shr 13);
  if LenLo2 < ((Size shr 13) and $FFFF) then
    Inc(LenHi);
  Inc(LenHi, Size shr 29);

  PBuf := @Key[1];
  while Size > 0 do
  begin
    if (Sizeof(HashBuffer) - Index) <= DWord(Size) then
    begin
      Move(PBuf^, HashBuffer[Index], Sizeof(HashBuffer) - Index);
      Dec(Size, Sizeof(HashBuffer) - Index);
      Inc(PBuf, Sizeof(HashBuffer) - Index);
      Compress(@CurrentHash, @HashBuffer);
    end
    else
    begin
      Move(PBuf^, HashBuffer[Index], Size);
      Inc(Index, Size);
      Size := 0;
    end;
  end;

  HashBuffer[Index] := $80;
  if Index >= 56 then
    Compress(@CurrentHash, @HashBuffer);

  PDWord(@HashBuffer[56])^ := SwapDWord(LenHi);
  PDWord(@HashBuffer[60])^ := SwapDWord(LenLo1 or (longint(LenLo2) shl 16));

  Compress(@CurrentHash, @HashBuffer);

  CurrentHash[0] := SwapDWord(CurrentHash[0]);
  CurrentHash[1] := SwapDWord(CurrentHash[1]);
  CurrentHash[2] := SwapDWord(CurrentHash[2]);
  CurrentHash[3] := SwapDWord(CurrentHash[3]);
  CurrentHash[4] := SwapDWord(CurrentHash[4]);
  Move(CurrentHash, Digest, Sizeof(CurrentHash));

  LenHi := 0; LenLo1 := 0; LenLo2 := 0;
  Index := 0;
  FillChar(HashBuffer, Sizeof(HashBuffer), 0);
  FillChar(CurrentHash, Sizeof(CurrentHash), 0);
end;

end.

