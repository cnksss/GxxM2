unit RLEUnit;

interface

function EncodeRLE(InData, OutData: Pointer; Width, Height, BytesPerPixel: Integer): Integer;
procedure DecodeRLE(InData, OutData: Pointer; Width, Height, BytesPerPixel: Integer);

implementation
type
  TByteArray = array[0..MaxInt - 1] of Byte;
  PByteArray = ^TByteArray;
  TWordArray = array[0..MaxInt div 2 - 1] of Word;
  PWordArray = ^TWordArray;
  TLongIntArray = array[0..MaxInt div 4 - 1] of LongInt;
  PLongIntArray = ^TLongIntArray;
  TLongWordArray = array[0..MaxInt div 4 - 1] of LongWord;
  PLongWordArray = ^TLongWordArray;

  { Color value for 32 bit images.}
  TColor32 = LongWord;
  PColor32 = ^TColor32;

  { Color value for 64 bit images.}
  TColor64 = type Int64;
  PColor64 = ^TColor64;

  { Color record for 24 bit images, which allows access to individual color
    channels.}
  TColor24Rec = packed record
    case LongInt of
      0: (B, G, R: Byte);
      1: (Channels: array[0..2] of Byte);
  end;
  PColor24Rec = ^TColor24Rec;
  TColor24RecArray = array[0..MaxInt div SizeOf(TColor24Rec) - 1] of TColor24Rec;
  PColor24RecArray = ^TColor24RecArray;

  { Color record for 32 bit images, which allows access to individual color
    channels.}
  TColor32Rec = packed record
    case LongInt of
      0: (Color: TColor32);
      1: (B, G, R, A: Byte);
      2: (Channels: array[0..3] of Byte);
      3: (Color24Rec: TColor24Rec);
  end;
  PColor32Rec = ^TColor32Rec;
  TColor32RecArray = array[0..MaxInt div SizeOf(TColor32Rec) - 1] of TColor32Rec;
  PColor32RecArray = ^TColor32RecArray;

function EncodeRLE(InData, OutData: Pointer; Width, Height, BytesPerPixel: Integer): Integer;
var
  WidthBytes, Written, I, Total: LongInt;

  function CountDiff(Data: PByte; Bpp, PixelCount: Longint): LongInt;
  var
    Pixel: LongWord;
    NextPixel: LongWord;
    N: LongInt;
  begin
    N := 0;
    Pixel := 0;
    NextPixel := 0;
    if PixelCount = 1 then
    begin
      Result := PixelCount;
      Exit;
    end;
    case Bpp of
      1: Pixel := Data^;
      2: Pixel := PWord(Data)^;
      3: PColor24Rec(@Pixel)^ := PColor24Rec(Data)^;
      4: Pixel := PLongWord(Data)^;
    end;
    while PixelCount > 1 do
    begin
      Inc(Data, Bpp);
      case Bpp of
        1: NextPixel := Data^;
        2: NextPixel := PWord(Data)^;
        3: PColor24Rec(@NextPixel)^ := PColor24Rec(Data)^;
        4: NextPixel := PLongWord(Data)^;
      end;
      if NextPixel = Pixel then
        Break;
      Pixel := NextPixel;
      N := N + 1;
      PixelCount := PixelCount - 1;
    end;
    if NextPixel = Pixel then
      Result := N
    else
      Result := N + 1;
  end;

  function CountSame(Data: PByte; Bpp, PixelCount: LongInt): LongInt;
  var
    Pixel: LongWord;
    NextPixel: LongWord;
    N: LongInt;
  begin
    N := 1;
    Pixel := 0;
    NextPixel := 0;
    case Bpp of
      1: Pixel := Data^;
      2: Pixel := PWord(Data)^;
      3: PColor24Rec(@Pixel)^ := PColor24Rec(Data)^;
      4: Pixel := PLongWord(Data)^;
    end;
    PixelCount := PixelCount - 1;
    while PixelCount > 0 do
    begin
      Inc(Data, Bpp);
      case Bpp of
        1: NextPixel := Data^;
        2: NextPixel := PWord(Data)^;
        3: PColor24Rec(@NextPixel)^ := PColor24Rec(Data)^;
        4: NextPixel := PLongWord(Data)^;
      end;
      if NextPixel <> Pixel then
        Break;
      N := N + 1;
      PixelCount := PixelCount - 1;
    end;
    Result := N;
  end;

  procedure RleCompressLine(Data: PByte; PixelCount, Bpp: LongInt; Dest:
    PByte; var Written: LongInt);
  const
    MaxRun = 128;
  var
    DiffCount: LongInt;
    SameCount: LongInt;
    RleBufSize: LongInt;
  begin
    RleBufSize := 0;
    while PixelCount > 0 do
    begin
      DiffCount := CountDiff(Data, Bpp, PixelCount);
      SameCount := CountSame(Data, Bpp, PixelCount);
      if (DiffCount > MaxRun) then
        DiffCount := MaxRun;
      if (SameCount > MaxRun) then
        SameCount := MaxRun;
      if (DiffCount > 0) then
      begin
        Dest^ := Byte(DiffCount - 1);
        Inc(Dest);
        PixelCount := PixelCount - DiffCount;
        RleBufSize := RleBufSize + (DiffCount * Bpp) + 1;
        Move(Data^, Dest^, DiffCount * Bpp);
        Inc(Data, DiffCount * Bpp);
        Inc(Dest, DiffCount * Bpp);
      end;
      if SameCount > 1 then
      begin
        Dest^ := Byte((SameCount - 1) or $80);
        Inc(Dest);
        PixelCount := PixelCount - SameCount;
        RleBufSize := RleBufSize + Bpp + 1;
        Inc(Data, (SameCount - 1) * Bpp);
        case Bpp of
          1: Dest^ := Data^;
          2: PWord(Dest)^ := PWord(Data)^;
          3: PColor24Rec(Dest)^ := PColor24Rec(Data)^;
          4: PLongWord(Dest)^ := PLongWord(Data)^;
        end;
        Inc(Data, Bpp);
        Inc(Dest, Bpp);
      end;
    end;
    Written := RleBufSize;
  end;

begin
  WidthBytes := Width * BytesPerPixel;
  Total := 0;
  for I := 0 to Height - 1 do
  begin
    RleCompressLine(@PByteArray(InData)[I * WidthBytes], Width,
      BytesPerPixel, @PByteArray(OutData)[Total], Written);
    Total := Total + Written;
  end;

  Result := Total;
end;

procedure DecodeRLE(InData, OutData: Pointer; Width, Height, BytesPerPixel: Integer);
var
  I, CPixel, Cnt: LongInt;
  Bpp, Rle: Byte;
  Src, Dest: PByte;
begin
  Src := InData;
  Dest := OutData;

  Cnt := Width * Height;
  Bpp := BytesPerPixel;
  CPixel := 0;
  while CPixel < Cnt do
  begin
    Rle := Src^;
    Inc(Src);
    if Rle < 128 then
    begin
          // Process uncompressed pixel
      Rle := Rle + 1;
      CPixel := CPixel + Rle;
      for I := 0 to Rle - 1 do
      begin
            // Copy pixel from src to dest
        case Bpp of
          1: Dest^ := Src^;
          2: PWord(Dest)^ := PWord(Src)^;
          3: PColor24Rec(Dest)^ := PColor24Rec(Src)^;
          4: PLongWord(Dest)^ := PLongWord(Src)^;
        end;
        Inc(Src, Bpp);
        Inc(Dest, Bpp);
      end;
    end
    else
    begin
          // Process compressed pixels
      Rle := Rle - 127;
      CPixel := CPixel + Rle;
          // Copy one pixel from src to dest (many times there)
      for I := 0 to Rle - 1 do
      begin
        case Bpp of
          1: Dest^ := Src^;
          2: PWord(Dest)^ := PWord(Src)^;
          3: PColor24Rec(Dest)^ := PColor24Rec(Src)^;
          4: PLongWord(Dest)^ := PLongWord(Src)^;
        end;
        Inc(Dest, Bpp);
      end;
      Inc(Src, Bpp);
    end;
  end;
end;

end.

