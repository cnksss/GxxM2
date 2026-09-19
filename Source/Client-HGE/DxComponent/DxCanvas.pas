unit DxCanvas;

interface

{$DEFINE HGE_DX8}

uses
  Windows,
  SysUtils,
  Classes,
  Graphics,
  DIB,
  HGE,
  HGECanvas,
  HGEFontEx;

function CheckTextureAlpha(Source:TTexture; X, Y:Integer):Boolean;

function NewTexture(FileData:Pointer; FileSize:Integer; AWidth, AHeight:Integer; TransparentColor:Cardinal; UseD3DFormat:Boolean; Alpha:TDIB = nil):TTexture; overload;

// function NewTexture(Data: Pointer; AWidth, AHeight: Integer): TTexture; overload;
function NewTexture(Source:TDIB; Alpha:TDIB = nil):TTexture; overload;
function NewTextureGray(Source:TDIB; Alpha:TDIB = nil):TTexture;
function NewTextureBright(Source:TDIB; Alpha:TDIB = nil):TTexture;
function NewTextureFromGraphic(srcPic:TGraphic):TTexture; //HZQ 20230605 添加一各从Graphi创建材质的函数

function NullTexture():TTexture;

procedure CopyTexture(Source, Target:TTexture; x, y:Integer; Blend:Boolean = False);

procedure ConvertLine32(SrcP, DesP:PByte; Width:Integer; BitCount:Byte; ColorTable:TRGBQuads; Alpha:PByte = nil);
procedure ConvertGrayLine32(SrcP, DesP:PByte; Width:Integer; BitCount:Byte; ColorTable:TRGBQuads; Alpha:PByte = nil);
procedure ConvertBrightLine32(SrcP, DesP:PByte; Width:Integer; BitCount:Byte; ColorTable:TRGBQuads; Alpha:PByte = nil);

procedure ConvertLine16(SrcP, DesP:PByte; Width:Integer; BitCount:Byte; ColorTable:TRGBQuads);

procedure ConvertLine32_8(SrcP, DesP:PByte; Width:Integer; ColorTable:TRGBQuads; Alpha:PByte);
procedure ConvertLine32_16(SrcP, DesP:PByte; Width:Integer; ColorTable:TRGBQuads; Alpha:PByte);
procedure ConvertLine32_24(SrcP, DesP:PByte; Width:Integer; ColorTable:TRGBQuads; Alpha:PByte);
procedure ConvertLine32_32(SrcP, DesP:PByte; Width:Integer; ColorTable:TRGBQuads; Alpha:PByte);

function GetTextTexture(HGEFont:THGEFont; SL:TStrings):TTexture;

var
  g_DefColorTable:TRGBQuads;//  array[0..255] of TRGBQuad;

  g_Grays:array[0..767] of Integer;

  ColorTable_565:array[0..High(Byte)] of Word;

  ColorTable_8_16Bit:array[0..High(Byte)] of Word;
  ColorTableBright_8_16Bit:array[0..High(Byte)] of Word;
  ColorTableGray_8_16Bit:array[0..High(Byte)] of Word;

  ColorTable_8_32Bit:array[0..High(Byte)] of Cardinal;
  ColorTableBright_8_32Bit:array[0..High(Byte)] of Cardinal;
  ColorTableGray_8_32Bit:array[0..High(Byte)] of Cardinal;

  ColorTable_16_32Bit:array[0..High(Word)] of Cardinal;
  ColorTableBright_16_32Bit:array[0..High(Word)] of Cardinal;
  ColorTableGray_16_32Bit:array[0..High(Word)] of Cardinal;

  ColorTableBright_16:array[0..High(Word)] of Word;
  ColorTableGray_16:array[0..High(Word)] of Word;

implementation

uses
  {$IFDEF HGE_DX8}DirectXGraphics{$ELSE}Direct3D9{$ENDIF},
  Math;

function CheckTextureAlpha(Source:TTexture; X, Y:Integer):Boolean;
var
  Pix:Cardinal;
begin
  Result := False;
  if Assigned(Source) then begin
    Pix := Source.Pixels[X, Y];
    Result := {(Pix <> $FF000000) and}(Pix <> $00000000);
  end;
end;

function NullTexture():TTexture;
begin
  Result := GameCanvas.HGE.Texture_Create(1, 1);
end;

{function NewTexture(Data: Pointer; AWidth, AHeight: Integer): TTexture;
var
  I, X, Y: Integer;
  Bits: Pointer;
  Pitch: Integer;
  BytesPerPixel: Integer;
  SrcP: PByte;
  DesP: PByte;
begin
  Result := GameCanvas.HGE.Texture_Create(AWidth, AHeight);
  if Assigned(Result) then begin

    BytesPerPixel := 4;
    // GameCanvas.HGE.Lock;
    // try
    if Result.Lock(Bits, Pitch, False) and (Bits <> nil) and (Pitch > 0) then begin
      try
        for Y := 0 to AHeight - 1 do begin
          SrcP := PByte(Integer(Data) + Y * AWidth * 4);
          DesP := PByte(Integer(Bits) + Y * Pitch);
          for X := 0 to AWidth - 1 do begin
            if PCardinal(SrcP)^ <> 0 then
              PCardinal(DesP)^ := PCardinal(SrcP)^ or $FF000000; // $FF000000;

            Inc(SrcP, BytesPerPixel);
            Inc(DesP, BytesPerPixel);
          end;
        end;
      finally
        Result.Unlock();
      end;
    end;
    // finally
      // Result.Unlock;
      // GameCanvas.HGE.UnLock;
    // end;
  end;
end;  }

const
  MaxPixelCount = 32768;
type
  pRGBArray32 = ^TRGBArray32;
  TRGBArray32 = array[0..MaxPixelCount - 1] of TRGBQuad;

  pRGBArray24 = ^TRGBArray24;
  TRGBArray24 = array[0..MaxPixelCount - 1] of TRGBTriple;

procedure ConvertLine16(SrcP, DesP:PByte; Width:Integer; BitCount:Byte; ColorTable:TRGBQuads);
var
  X:Integer;
begin
  case BitCount of
    8:begin
        for X := 0 to Width - 1 do begin
          PWord(DesP)^ := ColorTable_8_16Bit[SrcP^];
          Inc(SrcP);
          Inc(PWord(DesP));
        end;
      end;
    16:begin
        Move(SrcP^, DesP^, Width * 2);
      end;
    24:begin
        {OrigRow24 := pRGBArray24(SrcP);
        DestRow32 := pRGBArray32(DesP);
        for X := 0 to Width - 1 do begin
          if (OrigRow24[X].rgbtRed <> 0) or (OrigRow24[X].rgbtGreen <> 0) or (OrigRow24[X].rgbtBlue <> 0) then begin
            DestRow32[X].rgbRed := OrigRow24[X].rgbtRed;
            DestRow32[X].rgbGreen := OrigRow24[X].rgbtGreen;
            DestRow32[X].rgbBlue := OrigRow24[X].rgbtBlue;
            DestRow32[X].rgbReserved := 255;
          end;
        end;}
      end;
    32:begin

        {for X := 0 to Width - 1 do begin
          if PCardinal(SrcP)^ <> 0 then
            PCardinal(DesP)^ := PCardinal(SrcP)^ or $FF000000;
          Inc(SrcP, 4);
          Inc(DesP, 4);
        end;}
      end;
  end;
end;

procedure ConvertBrightLine16(SrcP, DesP:PByte; Width:Integer; BitCount:Byte; ColorTable:TRGBQuads);
var
  X:Integer;
begin
  case BitCount of
    8:begin
        for X := 0 to Width - 1 do begin
          PWord(DesP)^ := ColorTableBright_8_16Bit[SrcP^];
          Inc(SrcP);
          Inc(PWord(DesP));
        end;
      end;
    16:begin
        for X := 0 to Width - 1 do begin
          PWord(DesP)^ := ColorTableBright_16[PWord(SrcP)^];
          Inc(SrcP, 2);
          Inc(DesP, 2);
        end;
      end;
    24:begin
        { OrigRow24 := pRGBArray24(SrcP);
         DestRow32 := pRGBArray32(DesP);
         for X := 0 to Width - 1 do begin
           if (OrigRow24[X].rgbtRed <> 0) or (OrigRow24[X].rgbtGreen <> 0) or (OrigRow24[X].rgbtBlue <> 0) then begin
             DestRow32[X].rgbRed := MIN(255, Round(OrigRow24[X].rgbtRed * 1.3));
             DestRow32[X].rgbGreen := MIN(255, Round(OrigRow24[X].rgbtGreen * 1.3));
             DestRow32[X].rgbBlue := MIN(255, Round(OrigRow24[X].rgbtBlue * 1.3));
             DestRow32[X].rgbReserved := 255;
           end;
         end;}
      end;
    32:begin
        {OrigRow32 := pRGBArray32(SrcP);
        DestRow32 := pRGBArray32(DesP);
        for X := 0 to Width - 1 do begin
          if (OrigRow32[X].rgbRed <> 0) or (OrigRow32[X].rgbGreen <> 0) or (OrigRow32[X].rgbBlue <> 0) then begin
            DestRow32[X].rgbRed := MIN(255, Round(OrigRow32[X].rgbRed * 1.3));
            DestRow32[X].rgbGreen := MIN(255, Round(OrigRow32[X].rgbGreen * 1.3));
            DestRow32[X].rgbBlue := MIN(255, Round(OrigRow32[X].rgbBlue * 1.3));
            DestRow32[X].rgbReserved := 255;
          end;
        end;}
      end;
  end;
end;

procedure ConvertGrayLine16(SrcP, DesP:PByte; Width:Integer; BitCount:Byte; ColorTable:TRGBQuads);
var
  X:Integer;
  RGB:TRGBQuad;
  OrigRow24:pRGBArray24;
  OrigRow32:pRGBArray32;
begin
  case BitCount of
    8:begin
        for X := 0 to Width - 1 do begin
          PWord(DesP)^ := ColorTableGray_8_16Bit[SrcP^];
          Inc(SrcP);
          Inc(PWord(DesP));
        end;
      end;
    16:begin
        for X := 0 to Width - 1 do begin
          PWord(DesP)^ := ColorTableGray_16[PWord(SrcP)^];
          Inc(PWord(SrcP));
          Inc(PWord(DesP));
        end;
      end;
    24:begin
        OrigRow24 := pRGBArray24(SrcP);
        for X := 0 to Width - 1 do begin
          if (OrigRow24[X].rgbtRed <> 0) or (OrigRow24[X].rgbtGreen <> 0) or (OrigRow24[X].rgbtBlue <> 0) then begin
            RGB.rgbRed := g_Grays[OrigRow24[X].rgbtRed + OrigRow24[X].rgbtGreen + OrigRow24[X].rgbtBlue];
            RGB.rgbGreen := RGB.rgbRed;
            RGB.rgbBlue := RGB.rgbRed;
            PWord(DesP)^ := (RGB.rgbRed shl 8 and $F800) or (RGB.rgbGreen shl 3 and $07E0) or (RGB.rgbBlue shr 3 and $001F);
            Inc(DesP, 2);
          end;
        end;
      end;
    32:begin
        OrigRow32 := pRGBArray32(SrcP);
        for X := 0 to Width - 1 do begin
          if (OrigRow32[X].rgbRed <> 0) or (OrigRow32[X].rgbGreen <> 0) or (OrigRow32[X].rgbBlue <> 0) then begin
            RGB.rgbRed := g_Grays[OrigRow32[X].rgbRed + OrigRow32[X].rgbGreen + OrigRow32[X].rgbBlue];
            RGB.rgbGreen := RGB.rgbRed;
            RGB.rgbBlue := RGB.rgbRed;
            PWord(DesP)^ := (RGB.rgbRed shl 8 and $F800) or (RGB.rgbGreen shl 3 and $07E0) or (RGB.rgbBlue shr 3 and $001F);
            Inc(DesP, 2);
          end;
        end;
      end;
  end;
end;

{-------------------------------------------------------------------------------}

procedure ConvertLine32(SrcP, DesP:PByte; Width:Integer; BitCount:Byte; ColorTable:TRGBQuads; Alpha:PByte);
var
  X:Integer;
  OrigRow24:pRGBArray24;
  DestRow32:pRGBArray32;
begin
  if Alpha <> nil then begin
    case BitCount of
      8:begin
          for X := 0 to Width - 1 do begin
            PCardinal(DesP)^ := ColorTable_8_32Bit[SrcP^];
            //if Alpha <> nil then
            begin
              PCardinal(DesP)^ := (PCardinal(DesP)^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
            Inc(SrcP);
            Inc(PCardinal(DesP));
          end;
        end;
      16:begin
          for X := 0 to Width - 1 do begin
            PCardinal(DesP)^ := ColorTable_16_32Bit[PWord(SrcP)^];
            //if Alpha <> nil then
            begin
              PCardinal(DesP)^ := (PCardinal(DesP)^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
            Inc(SrcP, 2);
            Inc(DesP, 4);
          end;
        end;
      24:begin
          OrigRow24 := pRGBArray24(SrcP);
          DestRow32 := pRGBArray32(DesP);
          for X := 0 to Width - 1 do begin
            if (OrigRow24[X].rgbtRed <> 0) or (OrigRow24[X].rgbtGreen <> 0) or (OrigRow24[X].rgbtBlue <> 0) then begin
              DestRow32[X].rgbRed := OrigRow24[X].rgbtRed;
              DestRow32[X].rgbGreen := OrigRow24[X].rgbtGreen;
              DestRow32[X].rgbBlue := OrigRow24[X].rgbtBlue;
              DestRow32[X].rgbReserved := 255;
            end;
            //if Alpha <> nil then
            begin
              PCardinal(@DestRow32[X])^ := (PCardinal(@DestRow32[X])^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
          end;
        end;
      32:begin
          for X := 0 to Width - 1 do begin
            if PCardinal(SrcP)^ <> 0 then
              PCardinal(DesP)^ := PCardinal(SrcP)^ or $FF000000;

            //if Alpha <> nil then
            begin
              PCardinal(DesP)^ := (PCardinal(DesP)^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;

            Inc(SrcP, 4);
            Inc(DesP, 4);
          end;
        end;
    end;
  end
  else begin
    case BitCount of
      8:begin
          for X := 0 to Width - 1 do begin
            PCardinal(DesP)^ := ColorTable_8_32Bit[SrcP^];

            {
            if Alpha <> nil then
            begin
              PCardinal(DesP)^ := (PCardinal(DesP)^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
            }

            Inc(SrcP);
            Inc(PCardinal(DesP));
          end;
        end;
      16:begin
          for X := 0 to Width - 1 do begin
            PCardinal(DesP)^ := ColorTable_16_32Bit[PWord(SrcP)^];

            {
            if Alpha <> nil then
            begin
              PCardinal(DesP)^ := (PCardinal(DesP)^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
            }

            Inc(SrcP, 2);
            Inc(DesP, 4);
          end;
        end;
      24:begin
          OrigRow24 := pRGBArray24(SrcP);
          DestRow32 := pRGBArray32(DesP);
          for X := 0 to Width - 1 do begin
            if (OrigRow24[X].rgbtRed <> 0) or (OrigRow24[X].rgbtGreen <> 0) or (OrigRow24[X].rgbtBlue <> 0) then begin
              DestRow32[X].rgbRed := OrigRow24[X].rgbtRed;
              DestRow32[X].rgbGreen := OrigRow24[X].rgbtGreen;
              DestRow32[X].rgbBlue := OrigRow24[X].rgbtBlue;
              DestRow32[X].rgbReserved := 255;
            end;

            {
            if Alpha <> nil then
            begin
              PCardinal(@DestRow32[X])^ := (PCardinal(@DestRow32[X])^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
            }
          end;
        end;
      32:begin
          for X := 0 to Width - 1 do begin
            if PCardinal(SrcP)^ <> 0 then
              PCardinal(DesP)^ := PCardinal(SrcP)^ or $FF000000;

            {
            if Alpha <> nil then
            begin
              PCardinal(DesP)^ := (PCardinal(DesP)^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
            }

            Inc(SrcP, 4);
            Inc(DesP, 4);
          end;
        end;
    end;
  end;
end;

procedure ConvertLine32_8(SrcP, DesP:PByte; Width:Integer; ColorTable:TRGBQuads; Alpha:PByte);
var
  X:Integer;
begin
  if Alpha <> nil then begin
    for X := 0 to Width - 1 do begin
      PCardinal(DesP)^ := ColorTable_8_32Bit[SrcP^];
      //if Alpha <> nil then
      begin
        PCardinal(DesP)^ := (PCardinal(DesP)^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
        Inc(Alpha);
      end;
      Inc(SrcP);
      Inc(PCardinal(DesP));
    end;
  end
  else begin
    for X := 0 to Width - 1 do begin
      PCardinal(DesP)^ := ColorTable_8_32Bit[SrcP^];

      {
      if Alpha <> nil then
      begin
        PCardinal(DesP)^ := (PCardinal(DesP)^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
        Inc(Alpha);
      end;
      }

      Inc(SrcP);
      Inc(PCardinal(DesP));
    end;
  end;
end;

procedure ConvertLine32_16(SrcP, DesP:PByte; Width:Integer; ColorTable:TRGBQuads; Alpha:PByte);
var
  X:Integer;
  P:PByte;
begin
  P := Alpha;
  if P <> nil then begin
    for X := 0 to Width - 1 do begin
      PCardinal(DesP)^ := ColorTable_16_32Bit[PWord(SrcP)^];
      PCardinal(DesP)^ := (PCardinal(DesP)^ and $00FFFFFF) or (Longword(Byte(P^)) shl 24);
      Inc(P);
      Inc(SrcP, 2);
      Inc(DesP, 4);
    end;
  end
  else begin
    for X := 0 to Width - 1 do begin
      PCardinal(DesP)^ := ColorTable_16_32Bit[PWord(SrcP)^];
      Inc(SrcP, 2);
      Inc(DesP, 4);
    end;
  end;
end;

procedure ConvertLine32_24(SrcP, DesP:PByte; Width:Integer; ColorTable:TRGBQuads; Alpha:PByte);
var
  X:Integer;
  OrigRow24:pRGBArray24;
  DestRow32:pRGBArray32;
begin
  OrigRow24 := pRGBArray24(SrcP);
  DestRow32 := pRGBArray32(DesP);

  if Alpha <> nil then begin
    for X := 0 to Width - 1 do begin
      if (OrigRow24[X].rgbtRed <> 0) or (OrigRow24[X].rgbtGreen <> 0) or (OrigRow24[X].rgbtBlue <> 0) then begin
        DestRow32[X].rgbRed := OrigRow24[X].rgbtRed;
        DestRow32[X].rgbGreen := OrigRow24[X].rgbtGreen;
        DestRow32[X].rgbBlue := OrigRow24[X].rgbtBlue;
        DestRow32[X].rgbReserved := 255;
      end;
      //if Alpha <> nil then
      begin
        PCardinal(@DestRow32[X])^ := (PCardinal(@DestRow32[X])^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
        Inc(Alpha);
      end;
    end;
  end
  else begin
    for X := 0 to Width - 1 do begin
      if (OrigRow24[X].rgbtRed <> 0) or (OrigRow24[X].rgbtGreen <> 0) or (OrigRow24[X].rgbtBlue <> 0) then begin
        DestRow32[X].rgbRed := OrigRow24[X].rgbtRed;
        DestRow32[X].rgbGreen := OrigRow24[X].rgbtGreen;
        DestRow32[X].rgbBlue := OrigRow24[X].rgbtBlue;
        DestRow32[X].rgbReserved := 255;
      end;

      {
      if Alpha <> nil then
      begin
        PCardinal(@DestRow32[X])^ := (PCardinal(@DestRow32[X])^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
        Inc(Alpha);
      end;
      }
    end;
  end;
end;

procedure ConvertLine32_32(SrcP, DesP:PByte; Width:Integer; ColorTable:TRGBQuads; Alpha:PByte);
var
  X:Integer;
begin
  if Alpha <> nil then begin
    for X := 0 to Width - 1 do begin
      if PCardinal(SrcP)^ <> 0 then
        PCardinal(DesP)^ := PCardinal(SrcP)^ or $FF000000;

      //if Alpha <> nil then
      begin
        PCardinal(DesP)^ := (PCardinal(DesP)^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
        Inc(Alpha);
      end;

      Inc(SrcP, 4);
      Inc(DesP, 4);
    end;
  end
  else begin
    for X := 0 to Width - 1 do begin
      if PCardinal(SrcP)^ <> 0 then
        PCardinal(DesP)^ := PCardinal(SrcP)^ or $FF000000;

      {
      if Alpha <> nil then
      begin
        PCardinal(DesP)^ := (PCardinal(DesP)^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
        Inc(Alpha);
      end;
      }

      Inc(SrcP, 4);
      Inc(DesP, 4);
    end;
  end;
end;

procedure ConvertBrightLine32(SrcP, DesP:PByte; Width:Integer; BitCount:Byte; ColorTable:TRGBQuads; Alpha:PByte);
var
  X:Integer;
  OrigRow24:pRGBArray24;
  OrigRow32, DestRow32:pRGBArray32;
begin
  if Alpha <> nil then begin
    case BitCount of
      8:begin
          for X := 0 to Width - 1 do begin
            PCardinal(DesP)^ := ColorTableBright_8_32Bit[SrcP^];
            //if Alpha <> nil then
            begin
              PCardinal(DesP)^ := (PCardinal(DesP)^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
            Inc(SrcP);
            Inc(PCardinal(DesP));
          end;
        end;
      16:begin
          for X := 0 to Width - 1 do begin
            PCardinal(DesP)^ := ColorTableBright_16_32Bit[PWord(SrcP)^];
            //if Alpha <> nil then
            begin
              PCardinal(DesP)^ := (PCardinal(DesP)^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
            Inc(SrcP, 2);
            Inc(DesP, 4);
          end;
        end;
      24:begin
          OrigRow24 := pRGBArray24(SrcP);
          DestRow32 := pRGBArray32(DesP);
          for X := 0 to Width - 1 do begin
            if (OrigRow24[X].rgbtRed <> 0) or (OrigRow24[X].rgbtGreen <> 0) or (OrigRow24[X].rgbtBlue <> 0) then begin
              DestRow32[X].rgbRed := MIN(255, Round(OrigRow24[X].rgbtRed * 1.3));
              DestRow32[X].rgbGreen := MIN(255, Round(OrigRow24[X].rgbtGreen * 1.3));
              DestRow32[X].rgbBlue := MIN(255, Round(OrigRow24[X].rgbtBlue * 1.3));
              DestRow32[X].rgbReserved := 255;
            end;
            //if Alpha <> nil then
            begin
              PCardinal(@DestRow32[X])^ := (PCardinal(@DestRow32[X])^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
          end;
        end;
      32:begin
          OrigRow32 := pRGBArray32(SrcP);
          DestRow32 := pRGBArray32(DesP);
          for X := 0 to Width - 1 do begin
            if (OrigRow32[X].rgbRed <> 0) or (OrigRow32[X].rgbGreen <> 0) or (OrigRow32[X].rgbBlue <> 0) then begin
              DestRow32[X].rgbRed := MIN(255, Round(OrigRow32[X].rgbRed * 1.3));
              DestRow32[X].rgbGreen := MIN(255, Round(OrigRow32[X].rgbGreen * 1.3));
              DestRow32[X].rgbBlue := MIN(255, Round(OrigRow32[X].rgbBlue * 1.3));
              DestRow32[X].rgbReserved := 255;
            end;
            //if Alpha <> nil then
            begin
              PCardinal(@DestRow32[X])^ := (PCardinal(@DestRow32[X])^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
          end;
        end;
    end;
  end
  else begin
    case BitCount of
      8:begin
          for X := 0 to Width - 1 do begin
            PCardinal(DesP)^ := ColorTableBright_8_32Bit[SrcP^];

            {
            if Alpha <> nil then
            begin
              PCardinal(DesP)^ := (PCardinal(DesP)^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
            }

            Inc(SrcP);
            Inc(PCardinal(DesP));
          end;
        end;
      16:begin
          for X := 0 to Width - 1 do begin
            PCardinal(DesP)^ := ColorTableBright_16_32Bit[PWord(SrcP)^];

            {
            if Alpha <> nil then
            begin
              PCardinal(DesP)^ := (PCardinal(DesP)^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
            }

            Inc(SrcP, 2);
            Inc(DesP, 4);
          end;
        end;
      24:begin
          OrigRow24 := pRGBArray24(SrcP);
          DestRow32 := pRGBArray32(DesP);
          for X := 0 to Width - 1 do begin
            if (OrigRow24[X].rgbtRed <> 0) or (OrigRow24[X].rgbtGreen <> 0) or (OrigRow24[X].rgbtBlue <> 0) then begin
              DestRow32[X].rgbRed := MIN(255, Round(OrigRow24[X].rgbtRed * 1.3));
              DestRow32[X].rgbGreen := MIN(255, Round(OrigRow24[X].rgbtGreen * 1.3));
              DestRow32[X].rgbBlue := MIN(255, Round(OrigRow24[X].rgbtBlue * 1.3));
              DestRow32[X].rgbReserved := 255;
            end;

            {
            if Alpha <> nil then
            begin
              PCardinal(@DestRow32[X])^ := (PCardinal(@DestRow32[X])^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
            }
          end;
        end;
      32:begin
          OrigRow32 := pRGBArray32(SrcP);
          DestRow32 := pRGBArray32(DesP);
          for X := 0 to Width - 1 do begin
            if (OrigRow32[X].rgbRed <> 0) or (OrigRow32[X].rgbGreen <> 0) or (OrigRow32[X].rgbBlue <> 0) then begin
              DestRow32[X].rgbRed := MIN(255, Round(OrigRow32[X].rgbRed * 1.3));
              DestRow32[X].rgbGreen := MIN(255, Round(OrigRow32[X].rgbGreen * 1.3));
              DestRow32[X].rgbBlue := MIN(255, Round(OrigRow32[X].rgbBlue * 1.3));
              DestRow32[X].rgbReserved := 255;
            end;

            {
            if Alpha <> nil then
            begin
              PCardinal(@DestRow32[X])^ := (PCardinal(@DestRow32[X])^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
            }
          end;
        end;
    end;
  end;
end;

procedure ConvertGrayLine32(SrcP, DesP:PByte; Width:Integer; BitCount:Byte; ColorTable:TRGBQuads; Alpha:PByte);
var
  X:Integer;
  OrigRow24:pRGBArray24;
  OrigRow32, DestRow32:pRGBArray32;
begin
  if Alpha <> nil then begin
    case BitCount of
      8:begin
          for X := 0 to Width - 1 do begin
            PCardinal(DesP)^ := ColorTableGray_8_32Bit[SrcP^];
            //if Alpha <> nil then
            begin
              PCardinal(DesP)^ := (PCardinal(DesP)^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
            Inc(SrcP);
            Inc(PCardinal(DesP));
          end;
        end;
      16:begin
          for X := 0 to Width - 1 do begin
            PCardinal(DesP)^ := ColorTableGray_16_32Bit[PWord(SrcP)^];
            //if Alpha <> nil then
            begin
              PCardinal(DesP)^ := (PCardinal(DesP)^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
            Inc(SrcP, 2);
            Inc(DesP, 4);
          end;
        end;
      24:begin
          OrigRow24 := pRGBArray24(SrcP);
          DestRow32 := pRGBArray32(DesP);
          for X := 0 to Width - 1 do begin
            if (OrigRow24[X].rgbtRed <> 0) or (OrigRow24[X].rgbtGreen <> 0) or (OrigRow24[X].rgbtBlue <> 0) then begin
              DestRow32[X].rgbRed := g_Grays[OrigRow24[X].rgbtRed + OrigRow24[X].rgbtGreen + OrigRow24[X].rgbtBlue];
              DestRow32[X].rgbGreen := DestRow32[X].rgbRed;
              DestRow32[X].rgbBlue := DestRow32[X].rgbRed;
              DestRow32[X].rgbReserved := 255;
            end;
            //if Alpha <> nil then
            begin
              PCardinal(@DestRow32[X])^ := (PCardinal(@DestRow32[X])^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
          end;
        end;
      32:begin
          OrigRow32 := pRGBArray32(SrcP);
          DestRow32 := pRGBArray32(DesP);
          for X := 0 to Width - 1 do begin
            if (OrigRow32[X].rgbRed <> 0) or (OrigRow32[X].rgbGreen <> 0) or (OrigRow32[X].rgbBlue <> 0) then begin
              DestRow32[X].rgbRed := g_Grays[OrigRow32[X].rgbRed + OrigRow32[X].rgbGreen + OrigRow32[X].rgbBlue];
              DestRow32[X].rgbGreen := DestRow32[X].rgbRed;
              DestRow32[X].rgbBlue := DestRow32[X].rgbRed;
              DestRow32[X].rgbReserved := 255;
            end;
            //if Alpha <> nil then
            begin
              PCardinal(@DestRow32[X])^ := (PCardinal(@DestRow32[X])^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
          end;
        end;
    end;
  end
  else begin
    case BitCount of
      8:begin
          for X := 0 to Width - 1 do begin
            PCardinal(DesP)^ := ColorTableGray_8_32Bit[SrcP^];

            {
            if Alpha <> nil then
            begin
              PCardinal(DesP)^ := (PCardinal(DesP)^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
            }

            Inc(SrcP);
            Inc(PCardinal(DesP));
          end;
        end;
      16:begin
          for X := 0 to Width - 1 do begin
            PCardinal(DesP)^ := ColorTableGray_16_32Bit[PWord(SrcP)^];

            {
            if Alpha <> nil then
            begin
              PCardinal(DesP)^ := (PCardinal(DesP)^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
            }

            Inc(SrcP, 2);
            Inc(DesP, 4);
          end;
        end;
      24:begin
          OrigRow24 := pRGBArray24(SrcP);
          DestRow32 := pRGBArray32(DesP);
          for X := 0 to Width - 1 do begin
            if (OrigRow24[X].rgbtRed <> 0) or (OrigRow24[X].rgbtGreen <> 0) or (OrigRow24[X].rgbtBlue <> 0) then begin
              DestRow32[X].rgbRed := g_Grays[OrigRow24[X].rgbtRed + OrigRow24[X].rgbtGreen + OrigRow24[X].rgbtBlue];
              DestRow32[X].rgbGreen := DestRow32[X].rgbRed;
              DestRow32[X].rgbBlue := DestRow32[X].rgbRed;
              DestRow32[X].rgbReserved := 255;
            end;

            {
            if Alpha <> nil then
            begin
              PCardinal(@DestRow32[X])^ := (PCardinal(@DestRow32[X])^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
            }
          end;
        end;
      32:begin
          OrigRow32 := pRGBArray32(SrcP);
          DestRow32 := pRGBArray32(DesP);
          for X := 0 to Width - 1 do begin
            if (OrigRow32[X].rgbRed <> 0) or (OrigRow32[X].rgbGreen <> 0) or (OrigRow32[X].rgbBlue <> 0) then begin
              DestRow32[X].rgbRed := g_Grays[OrigRow32[X].rgbRed + OrigRow32[X].rgbGreen + OrigRow32[X].rgbBlue];
              DestRow32[X].rgbGreen := DestRow32[X].rgbRed;
              DestRow32[X].rgbBlue := DestRow32[X].rgbRed;
              DestRow32[X].rgbReserved := 255;
            end;

            {
            if Alpha <> nil then
            begin
              PCardinal(@DestRow32[X])^ := (PCardinal(@DestRow32[X])^ and $00FFFFFF) or (Longword(Byte(Alpha^)) shl 24);
              Inc(Alpha);
            end;
            }
          end;
        end;
    end;
  end;
end;

function NewTextureGray(Source:TDIB; Alpha:TDIB):TTexture;
var
  Y:Integer;
  Bits:Pointer;
  Pitch:Integer;
  SrcP:PByte;
  DesP:PByte;
  AlphaP:PByte;
begin
  Result := GameCanvas.HGE.Texture_Create(Source.Width, Source.Height);

  if Assigned(Result) then begin
    if Result.Lock(Bits, Pitch, False) and (Bits <> nil) and (Pitch > 0) then begin
      try
        if Alpha <> nil then begin
          for Y := 0 to Source.Height - 1 do begin
            SrcP := Source.ScanLine[Y];
            DesP := PByte(Integer(Bits) + Y * Pitch);
            AlphaP := Alpha.ScanLine[Y];
            ConvertGrayLine32(SrcP, DesP, Source.Width, Source.BitCount, Source.ColorTable, AlphaP);
          end;
        end
        else begin
          for Y := 0 to Source.Height - 1 do begin
            SrcP := Source.ScanLine[Y];
            DesP := PByte(Integer(Bits) + Y * Pitch);
            ConvertGrayLine32(SrcP, DesP, Source.Width, Source.BitCount, Source.ColorTable, nil);
          end;
        end;
      finally
        Result.Unlock();
      end;
    end;
  end;
end;

function NewTextureBright(Source:TDIB; Alpha:TDIB):TTexture;
var
  Y:Integer;
  Bits:Pointer;
  Pitch:Integer;
  SrcP:PByte;
  DesP:PByte;
  AlphaP:PByte;
begin
  Result := GameCanvas.HGE.Texture_Create(Source.Width, Source.Height);
  if Assigned(Result) then begin
    if Result.Lock(Bits, Pitch, False) and (Bits <> nil) and (Pitch > 0) then begin
      try
        if Alpha <> nil then begin
          for Y := 0 to Source.Height - 1 do begin
            SrcP := Source.ScanLine[Y];
            DesP := PByte(Integer(Bits) + Y * Pitch);
            AlphaP := Alpha.ScanLine[Y];
            ConvertBrightLine32(SrcP, DesP, Source.Width, Source.BitCount, Source.ColorTable, AlphaP);
          end;
        end
        else begin
          for Y := 0 to Source.Height - 1 do begin
            SrcP := Source.ScanLine[Y];
            DesP := PByte(Integer(Bits) + Y * Pitch);
            ConvertBrightLine32(SrcP, DesP, Source.Width, Source.BitCount, Source.ColorTable, nil);
          end;
        end;
      finally
        Result.Unlock();
      end;
    end;
  end;
end;

function NewTexture(Source:TDIB; Alpha:TDIB):TTexture;
var
  Y:Integer;
  Bits:Pointer;
  Pitch:Integer;
  SrcP:PByte;
  DesP:PByte;
  AlphaP:PByte;
begin
  Result := GameCanvas.HGE.Texture_Create(Source.Width, Source.Height);
  if Assigned(Result) then begin
    if Result.Lock(Bits, Pitch, False) and (Bits <> nil) and (Pitch > 0) then begin
      try
        if Alpha <> nil then begin
          case Source.BitCount of
            8:begin
                for Y := 0 to Source.Height - 1 do begin
                  SrcP := Source.ScanLine[Y];
                  DesP := PByte(Integer(Bits) + Y * Pitch);
                  AlphaP := Alpha.ScanLine[Y];
                  ConvertLine32_8(SrcP, DesP, Source.Width, Source.ColorTable, AlphaP);
                end;
              end;
            16:begin
                for Y := 0 to Source.Height - 1 do begin
                  SrcP := Source.ScanLine[Y];
                  DesP := PByte(Integer(Bits) + Y * Pitch);
                  AlphaP := Alpha.ScanLine[Y];
                  ConvertLine32_16(SrcP, DesP, Source.Width, Source.ColorTable, AlphaP);
                end;
              end;
            24:begin
                for Y := 0 to Source.Height - 1 do begin
                  SrcP := Source.ScanLine[Y];
                  DesP := PByte(Integer(Bits) + Y * Pitch);
                  AlphaP := Alpha.ScanLine[Y];
                  ConvertLine32_24(SrcP, DesP, Source.Width, Source.ColorTable, AlphaP);
                end;
              end;
            32:begin
                for Y := 0 to Source.Height - 1 do begin
                  SrcP := Source.ScanLine[Y];
                  DesP := PByte(Integer(Bits) + Y * Pitch);
                  AlphaP := Alpha.ScanLine[Y];
                  ConvertLine32_32(SrcP, DesP, Source.Width, Source.ColorTable, AlphaP);
                end;
              end;
          end;
        end
        else begin
          case Source.BitCount of
            8:begin
                for Y := 0 to Source.Height - 1 do begin
                  SrcP := Source.ScanLine[Y];
                  DesP := PByte(Integer(Bits) + Y * Pitch);
                  ConvertLine32_8(SrcP, DesP, Source.Width, Source.ColorTable, nil);
                end;
              end;
            16:begin
                for Y := 0 to Source.Height - 1 do begin
                  SrcP := Source.ScanLine[Y];
                  DesP := PByte(Integer(Bits) + Y * Pitch);
                  ConvertLine32_16(SrcP, DesP, Source.Width, Source.ColorTable, nil);
                end;
              end;
            24:begin
                for Y := 0 to Source.Height - 1 do begin
                  SrcP := Source.ScanLine[Y];
                  DesP := PByte(Integer(Bits) + Y * Pitch);
                  ConvertLine32_24(SrcP, DesP, Source.Width, Source.ColorTable, nil);
                end;
              end;
            32:begin
                for Y := 0 to Source.Height - 1 do begin
                  SrcP := Source.ScanLine[Y];
                  DesP := PByte(Integer(Bits) + Y * Pitch);
                  ConvertLine32_32(SrcP, DesP, Source.Width, Source.ColorTable, nil);
                end;
              end;
          end;
        end;
      finally
        Result.Unlock();
      end;
    end;
  end;
end;

function NewTextureFromGraphic(srcPic:TGraphic):TTexture;
var
  objDib:TDIB;
begin
  Result := nil;
  if Assigned(srcPic) then begin
    objDib := TDIB.Create;
    try
      objDib.Assign(srcPic);
      Result := NewTexture(objDib);
    finally
      FreeAndNil(objDib);
    end;
  end;
end;

function NewTexture(FileData:Pointer; FileSize:Integer; AWidth, AHeight:Integer; TransparentColor:Cardinal; UseD3DFormat:Boolean; Alpha:TDIB = nil):TTexture;
var
  Texture:TTexture;
begin
  Texture := nil;
  if UseD3DFormat then begin
    Texture := GameCanvas.HGE.Texture_LoadDDS(D3DFMT_DXT1, FileData, FileSize, False);
    if Texture = nil then
      Texture := GameCanvas.HGE.Texture_LoadDDS(D3DFMT_DXT3, FileData, FileSize, False);
    if Texture = nil then
      Texture := GameCanvas.HGE.Texture_LoadDDS(D3DFMT_DXT5, FileData, FileSize, False);
    if Texture = nil then
      Texture := GameCanvas.HGE.Texture_LoadDDS(D3DFMT_DXT2, FileData, FileSize, False);
    if Texture = nil then
      Texture := GameCanvas.HGE.Texture_LoadDDS(D3DFMT_DXT4, FileData, FileSize, False);
  end;
  if Texture = nil then
    Texture := GameCanvas.HGE.Texture_Load(FileData, FileSize, False);
  Result := Texture;

  {
  if Assigned(Result) and (Alpha <> nil) then
  begin
    Bits := nil;
    Pitch := 0;
    if Result.Lock(Bits, Pitch, True) and (Bits <> nil) and (Pitch > 0) then
    begin
      try
        for Y := 0 to Result.Height - 1 do
        begin
          AlphaP := Alpha.ScanLine[Y];

          for X := 0 to Result.Width - 1 do
          begin
            DestP := PInteger(Integer(Bits) + Pitch * Y + X * 4);

            DestP^ := AlphaP^;
            Inc(AlphaP);
          end;
        end;
      finally
        Result.Unlock();
      end;
    end;
  end;
  }
end;

procedure CopyTexture(Source, Target:TTexture; x, y:Integer; Blend:Boolean);
var
  srcBits, destBits:Pointer;
  srcPitch, destPitch:Integer;
  srcleft, srcwidth, srctop, srcbottom, I, j:Integer;
  srcPoint, targetPoint:PInteger;
  nGray:Integer;
  RGBQuad:PRGBQuad;
begin
  if Source = nil then exit;
  if Target = nil then Exit;
  if x >= Target.Width then exit;
  if y >= Target.Height then exit;
  if x < 0 then begin
    srcleft := -x;
    srcwidth := Source.Width + x;
    x := 0;
  end
  else begin
    srcleft := 0;
    srcwidth := Source.Width;
  end;
  if y < 0 then begin
    srctop := -y;
    srcbottom := srctop + Source.Height + y;
    y := 0;
  end
  else begin
    srctop := 0;
    srcbottom := srctop + Source.Height;
  end;

  if (srcleft + srcwidth) > Source.Width then
    srcwidth := Source.Width - srcleft;
  if srcbottom > Source.Height then
    srcbottom := Source.Height;
  if (x + srcwidth) > Target.Width then
    srcwidth := Target.Width - x;

  if (y + srcbottom - srctop) > Target.Height then
    srcbottom := Target.Height - y + srctop;

  if (srcwidth <= 0) or (srcbottom <= 0) or (srcleft >= Source.Width) or (srctop >= Source.Height) then
    exit;

  if Source.Lock(srcBits, srcPitch, True) then begin
    try
      if Target.Lock(destBits, destPitch) then begin
        try
          if not Blend then begin
            for i := srctop to srcbottom - 1 do begin
              for j := srcLeft to srcwidth - 1 do begin
                srcPoint := PInteger(Integer(srcBits) + srcPitch * I + j * 4);
                targetPoint := PInteger(Integer(destBits) + destPitch * (y + i - srctop) + (x + j - srcleft) * 4);
                if srcPoint^ <> 0 then
                  targetPoint^ := srcPoint^;
              end;
            end;
          end
          else begin
            for i := srctop to srcbottom - 1 do begin
              for j := srcLeft to srcwidth - 1 do begin
                srcPoint := PInteger(Integer(srcBits) + srcPitch * I + j * 4);
                targetPoint := PInteger(Integer(destBits) + destPitch * (y + i - srctop) + (x + j - srcleft) * 4);
                if srcPoint^ <> 0 then begin
                  targetPoint^ := srcPoint^;
                  RGBQuad := PRGBQuad(targetPoint);
                  nGray := (30 * RGBQuad.rgbBlue + 59 * RGBQuad.rgbGreen + 11 * RGBQuad.rgbRed) div 100;
                  RGBQuad.rgbReserved := Byte(nGray);
                end;
              end;
            end;
          end;
        finally
          Target.UnLock;
        end;
      end;
    finally
      Source.Unlock;
    end;
  end;
end;

function GetTextTexture(HGEFont:THGEFont; SL:TStrings):TTexture;
var
  HHDC:HDC;
  TextSize:TSize;
  BitmapInfo:TBitmapInfo;
  PBitmapBits:Pointer; // PIntegerArray;
  HHBitmap:HBitmap;
  R:TRect;

  Bits:Pointer;
  X, Y, Pitch:Integer;

  SrcP:PByte;
  DesP:PByte;

  I, nW, nH, nLH:Integer;

  S:string;
begin
  //Result := nil; //HZQ 20230519

  nW := 0;
  HHDC := CreateCompatibleDC(0);
  try
    SelectObject(HHDC, HGEFont.Font.Handle);

    for I := 0 to SL.Count - 1 do begin
      S := SL.Strings[I];
      Windows.GetTextExtentPoint32(HHDC, PChar(S), Length(S), TextSize);
      if TextSize.cx > nW then nW := TextSize.cx;
    end;

    Windows.GetTextExtentPoint32(HHDC, '|', 1, TextSize);
    nLH := TextSize.cy;

    nW := nW;
    nH := nLH * SL.Count;

    FillChar(BitmapInfo, SizeOf(BitmapInfo), 0);
    with BitmapInfo.bmiHeader do begin
      //位图信息头
      biSize := SizeOf(TBitmapInfoHeader);
      biWidth := nW;
      biHeight := -nH;
      biPlanes := 1;
      biBitCount := 32;
      biCompression := BI_RGB;
    end;

    Result := GameCanvas.HGE.Texture_Create(nW, nH);

    PBitmapBits := nil;
    HHBitmap := CreateDIBSection(HHDC, BitmapInfo, DIB_RGB_COLORS, PBitmapBits, 0, 0);

    if (HHBitmap <> 0) and (PBitmapBits <> nil) then begin
      SelectObject(HHDC, HHBitmap);
      SetTextColor(HHDC, RGB(255, 255, 255)); //设文字颜色为白色
      SetBkColor(HHDC, RGB(0, 0, 0));

      for I := 0 to SL.Count - 1 do begin
        S := SL.Strings[I];
        R := Rect(0, I * nLH, nW, I * nLH + nLH);
        Windows.DrawText(HHDC, PChar(S), Length(S), R, DT_CENTER or DT_SINGLELINE);
      end;

      R := Rect(0, 0, nW, nH);
      try
        if Result.Lock(R, Bits, Pitch, False) then begin
          if (Bits <> nil) and (Pitch > 0) then begin
            for Y := 0 to nH - 1 do begin
              SrcP := PByte(Integer(PBitmapBits) + Y * nW * 4);
              DesP := PByte(Integer(Bits) + Y * Pitch);
              for X := 0 to nW - 1 do begin
                if PCardinal(SrcP)^ <> 0 then
                  PCardinal(DesP)^ := PCardinal(SrcP)^ or $FF000000 //$FF000000;
                else
                  PCardinal(DesP)^ := $00000000;

                Inc(SrcP, 4);
                Inc(DesP, 4);
              end;
            end;
          end
        end;
      finally
        Result.Unlock;
      end;

      DeleteObject(HHBitmap);
    end;
  finally
    DeleteDC(HHDC);
  end;
end;

end.
