{$IF PRIVATE_CLIENT = 0}
  {.$I VMProtectBeginUltra.inc}
{$IFEND}
  Key := LoginGatePassword[0] xor LoginGatePassword[1] xor LoginGatePassword[2] xor LoginGatePassword[3];

  {
  g_ConfigClient.nPivateClientKey1 := $5EFCE91B;
  g_ConfigClient.nPivateClientKey2 := $76CBFCD1;
  g_ConfigClient.nPivateClientKey3 := 1409455540;
  }
  
{$IF PRIVATE_CLIENT = 0}
  EncryptDes(SendData[1], SendData[1], Length(SendData), IntToStr(Key));
{$ELSE}
  EncryptDes(SendData[1], SendData[1], Length(SendData), IntToStr(Key or (Key xor (g_ConfigClient.nPivateClientKey1 xor g_ConfigClient.nPivateClientKey3))));
{$IFEND}
  SendData := EncodeString(SendData);

  Len := Length(SendData);

  k := 1;

  ArrKey[0] := Key and $0000000F;
  ArrKey[1] := Key and $000000F0;
  ArrKey[2] := Key and $00000F00;
  ArrKey[3] := Key and $0000F000;
  ArrKey[4] := Key and $000F0000;
  ArrKey[5] := Key and $00F00000;
  ArrKey[6] := Key and $0F000000;
  ArrKey[7] := Key and $F0000000;

{$IF PRIVATE_CLIENT = 1}
  a := g_ConfigClient.nPivateClientKey1;
  b := g_ConfigClient.nPivateClientKey2;
{$ELSE}
  a := $801B0D01;
  b := $5DAE409F;
{$IFEND}

  c := Key;

  while len >= 12 do
  begin
    a := a + DWORD(Ord(SendData[k + 0]) + (Ord(SendData[k + 1]) shl 8) + (Ord(SendData[k + 2])  shl 16) + (Ord(SendData[k + 3])  shl 24));
    b := b + DWORD(Ord(SendData[k + 4]) + (Ord(SendData[k + 5]) shl 8) + (Ord(SendData[k + 6])  shl 16) + (Ord(SendData[k + 7])  shl 24));
    c := c + DWORD(Ord(SendData[k + 8]) + (Ord(SendData[k + 9]) shl 8) + (Ord(SendData[k + 10]) shl 16) + (Ord(SendData[k + 11]) shl 24));

    // a -= b; a -= c; a ^= c >> 13;
    a := a - b;   a := a - c;   a := a xor (c shr ArrKey[5]);

    // b -= c; b -= a; b ^= a << 8;
    b := b - c;   b := b - a;   b := b xor (a shl ArrKey[7]);

    // c -= a; c -= b; c ^= b >> 13;
    c := c - a;   c := c - b;   c := c xor (b shr ArrKey[2]);

    // a -= b; a -= c; a ^= c >> 12;
    a := a - b;   a := a - c;   a := a xor (c shr ArrKey[4]);

    // b -= c; b -= a; b ^= a << 16;
    b := b - c;   b := b - a;   b := b xor (a shl ArrKey[3]);

    // c -= a; c -= b; c ^= b >> 5;
    c := c - a;   c := c - b;   c := c xor (b shr ArrKey[0]);

    // a -= b; a -= c; a ^= c >> 3;
    a := a - b;   a := a - c;   a := a xor (c shr ArrKey[6]);

    // b -= c; b -= a; b ^= a << 10;
    b := b - c;   b := b - a;   b := b xor (a shl ArrKey[1]);

    // c -= a; c -= b; c ^= b >> 15;
    c := c - a;   c := c - b;   c := c xor (b shr 3);

    Inc(k, 12);
    Dec(len, 12);
  end;

  c := c + DWORD(Length(SendData));

  if len >= 11 then c := c + DWORD(Ord(SendData[k + 10]) shl 24);
  if len >= 10 then c := c + DWORD(Ord(SendData[k + 9])  shl 16);
  if len >= 9  then c := c + DWORD(Ord(SendData[k + 8])  shl 8);

  if len >= 8  then b := b + DWORD(Ord(SendData[k + 7])  shl 24);
  if len >= 7  then b := b + DWORD(Ord(SendData[k + 6])  shl 16);
  if len >= 6  then b := b + DWORD(Ord(SendData[k + 5])  shl 8);
  if len >= 5  then b := b + DWORD(Ord(SendData[k + 4]));

  if len >= 4  then a := a + DWORD(Ord(SendData[k + 3])  shl 24);
  if len >= 3  then a := a + DWORD(Ord(SendData[k + 2])  shl 16);
  if len >= 2  then a := a + DWORD(Ord(SendData[k + 1])  shl 8);
  if len >= 1  then a := a + DWORD(Ord(SendData[k + 0]));

  // a -= b; a -= c; a ^= c >> 13;
  a := a - b;   a := a - c;   a := a xor (c shr ArrKey[5]);

  // b -= c; b -= a; b ^= a << 8;
  b := b - c;   b := b - a;   b := b xor (a shl ArrKey[7]);

  // c -= a; c -= b; c ^= b >> 13;
  c := c - a;   c := c - b;   c := c xor (b shr ArrKey[2]);

  // a -= b; a -= c; a ^= c >> 12;
  a := a - b;   a := a - c;   a := a xor (c shr ArrKey[4]);

  // b -= c; b -= a; b ^= a << 16;
  b := b - c;   b := b - a;   b := b xor (a shl ArrKey[3]);

  // c -= a; c -= b; c ^= b >> 5;
  c := c - a;   c := c - b;   c := c xor (b shl ArrKey[0]);

  // a -= b; a -= c; a ^= c >> 3;
  a := a - b;   a := a - c;   a := a xor (c shr ArrKey[6]);

  // b -= c; b -= a; b ^= a << 10;
  b := b - c;   b := b - a;   b := b xor (a shl ArrKey[1]);

  // c -= a; c -= b; c ^= b >> 15;
  c := c - a;   c := c - b;   c := c xor (b shr 3);

{$IF PRIVATE_CLIENT = 0}
  {.$I VMProtectEnd.inc}
{$IFEND}
