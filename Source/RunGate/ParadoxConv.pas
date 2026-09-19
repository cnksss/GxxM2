{
    *********************************************************************
    Copyright (C) 2007-2008  Kudriavtsev Pavel (divinus)

    e-mail: paulkudr@mail.ru
    ICQ: 301273438

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
    *********************************************************************

}

unit ParadoxConv;

{$IFDEF FPC}
  {$mode DELPHI}
{$ENDIF}

{$H+}

interface

uses
  {$IFDEF WINDOWS}
  Windows,
  {$ENDIF}
  Classes, SysUtils;

type

  eConvException = class(Exception)
    SrcEncoding: string;
    DstEncoding: string;
    Symbol: string;
  end;

  TEncoding = (UCS4, UTF8, KOI8R, ISO88595, CP1251, CP866);

  function GetCodepage: string;
  function Encoding(Source, Dest: TEncoding; S: AnsiString): AnsiString;

  function Utf8ToUCS4(S: AnsiString): AnsiString;
  function UCS4ToUtf8(S: AnsiString): AnsiString;

  function Cp1251ToUCS4(S: AnsiString): AnsiString;
  function UCS4ToCp1251(S: AnsiString): AnsiString;
  
//  function Cp866ToUCS4(S: AnsiString): AnsiString;
  function UCS4ToCp866(S: AnsiString): AnsiString;

  function Koi8rToUCS4(S: AnsiString): AnsiString;
  function UCS4ToKoi8r(S: AnsiString): AnsiString;
  
  function ISO88595ToUCS4(S: AnsiString): AnsiString;
  function UCS4ToISO88595(S: AnsiString): AnsiString;

implementation

function CreateConvException(Source, Dest, Sym: string): eConvException;
begin
  Result := eConvException.Create(Format('Can''t convert from %S to %S symbol %S', [Source, Dest, Sym]));
  with Result do
  begin
    SrcEncoding := Source;
    DstEncoding := Dest;
    Symbol := Sym;
  end;
end;

function GetCodepage: string;
{$IFDEF UNIX}
var
  Lang: string;
  Index: Integer;
{$ENDIF}
begin
  Result := '';
{$IFDEF WINDOWS}
  Result := 'CP' + IntToStr(GetACP);
{$ENDIF}

{$IFDEF UNIX}
  Lang := GetEnvironmentVariable('LANG');
  Index := Pos('.', Lang);
  if (Index > 0) and (Index <= length(Lang)) then Result := copy(Lang, Index + 1, length(Lang) - Index);
{$ENDIF}
end;

function Encoding(Source, Dest: TEncoding; S: AnsiString): AnsiString;
begin
  Result := '';
  case Source of
    UCS4:
      begin
        case Dest of
          UTF8:      Result := UCS4ToUtf8(S);
          KOI8R:     Result := UCS4ToKoi8r(S);
          ISO88595:  Result := UCS4ToISO88595(S);
          CP1251:    Result := UCS4ToCp1251(S);
          CP866:     Result := UCS4ToCp866(S);
        end;
      end;
    UTF8:
      begin
        case Dest of
          UCS4:      Result := Utf8ToUCS4(S);
          KOI8R:     Result := UCS4ToKoi8r(Utf8ToUCS4(S));
          ISO88595:  Result := UCS4ToISO88595(Utf8ToUCS4(S));
          CP1251:    Result := UCS4ToCp1251(Utf8ToUCS4(S));
          CP866:     Result := UCS4ToCp866(Utf8ToUCS4(S));
        end;
      end;
    KOI8R:
      begin
        case Dest of
          UTF8:      Result := UCS4ToUtf8(Koi8rTOUCS4(S));
          UCS4 :     Result := Koi8rToUCS4(S);
          ISO88595:  Result := UCS4ToISO88595(Koi8rTOUCS4(S));
          CP1251:    Result := UCS4ToCp1251(Koi8rTOUCS4(S));
          CP866:     Result := UCS4ToCp866(Koi8rTOUCS4(S));
        end;
      end;
    ISO88595:
      begin
        case Dest of
          UTF8:      Result := UCS4ToUtf8(ISO88595ToUCS4(S));
          KOI8R:     Result := UCS4ToKoi8r(ISO88595ToUCS4(S));
          UCS4:      Result := ISO88595ToUCS4(S);
          CP1251:    Result := UCS4ToCp1251(ISO88595ToUCS4(S));
          CP866:     Result := UCS4ToCp866(ISO88595ToUCS4(S));
        end;
      end;
    CP1251:
      begin
        case Dest of
          UTF8:      Result := UCS4ToUtf8(Cp1251ToUCS4(S));
          KOI8R:     Result := UCS4ToKoi8r(Cp1251ToUCS4(S));
          ISO88595:  Result := UCS4ToISO88595(Cp1251ToUCS4(S));
          UCS4:      Result := Cp1251ToUCS4(S);
          CP866:     Result := UCS4ToCp866(Cp1251ToUCS4(S));
        end;
      end;
{    CP866:
      begin
        case Dest of
          UTF8:      Result := UCS4ToUtf8(Cp866ToUCS4(S));
          KOI8R:     Result := UCS4ToKoi8r(Cp866ToUCS4(S));
          ISO88595:  Result := UCS4ToISO88595(Cp866ToUCS4(S));
          CP1251:    Result := UCS4ToCp1251(Cp866ToUCS4(S));
          UCS4:      Result := Cp866ToUCS4(S);
        end;
      end;                     }
  end;
end;

function Utf8ToUCS4(S: AnsiString): AnsiString;
var
  Count, I, II, Value: Integer;
  B: Byte;
begin
  Result := '';
  I := 1;
  while I <= Length(S) do
  begin
    B := Ord(S[I]);

    if (B and $80) = 0 then
      Count := 1
    else
      Count := 0;

    while (B and $80) <> 0 do
    begin
      Count := Count + 1;
      B := B shl 1;
    end;
    
    if Count > 1 then
      B := B shr Count;

    Value := 0;
    Value := Value or B;

    if (Length(S) - I) < (Count - 1) then
      raise CreateConvException('Utf8', 'UCS4', '');

    for II := 1 to Count - 1 do
    begin
      if (Ord(S[I + II]) and $C0) <> $80 then
        raise CreateConvException('Utf8', 'UCS4', '');
      Value := (Value shl 6) or (Ord(S[I + II]) and $3F);
    end;

    Result := Result + AnsiChar((Value shr 24) and $FF);
    Result := Result + AnsiChar((Value shr 16) and $FF);
    Result := Result + AnsiChar((Value shr 8) and $FF);
    Result := Result + AnsiChar(Value and $FF);

    I := I + Count;
  end;
end;

function UCS4ToUtf8(S: AnsiString): AnsiString;
var
  Count, I: Integer;
  Value: Integer;
  StrTemp: AnsiString;
begin
  Result := '';
  if Length(S) = 0 then Exit;
  if (Length(S) < 4) or ((Length(S) mod 4) <> 0) then
    raise CreateConvException('UCS4', 'Utf8', '');

  I := 1;
  while I <= Length(S) do
  begin
    Value := 0;
    Value := Value or Ord(S[I]);
    Inc(I);

    Value := (Value shl 8) or Ord(S[I]);
    Inc(I);

    Value := (Value shl 8) or Ord(S[I]);
    Inc(I);

    Value := (Value shl 8) or Ord(S[I]);
    Inc(I);

    if Value < $80 then
      Count := 1
    else if Value < $800 then
        Count := 2
    else if Value < $10000 then
      Count := 3
    else if Value < $200000 then
      Count := 4
    else if Value < $4000000 then
      Count := 5
    else //if Value <= $7FFFFFFF then
      Count := 6;

    StrTemp := '';
    if Count >= 6 then
    begin
      StrTemp := AnsiChar($80 or (Value and $3F)) + StrTemp;
      Value := Value shr 6;
      Value := Value or $4000000;
    end;

    if Count >= 5 then
    begin
      StrTemp := AnsiChar($80 or (Value and $3F)) + StrTemp;
      Value := Value shr 6;
      Value := Value or $200000;
    end;

    if Count >= 4 then
    begin
      StrTemp := AnsiChar($80 or (Value and $3F)) + StrTemp;
      Value := Value shr 6;
      Value := Value or $10000;
    end;

    if Count >= 3 then
    begin
      StrTemp := AnsiChar($80 or (Value and $3F)) + StrTemp;
      Value := Value shr 6;
      Value := Value or $800;
    end;

    if Count >= 2 then
    begin
      StrTemp := AnsiChar($80 or (Value and $3F)) + StrTemp;
      Value := Value shr 6;
      Value := Value or $C0;
    end;

    StrTemp := AnsiChar(Value) + StrTemp;
    
    Result := Result + StrTemp;
  end;
end;

function Cp1251ToUCS4(S: AnsiString): AnsiString;
var
  I: Integer;
begin
  Result := '';

  for I := 1 to Length(S) do
  begin
    Result := Result + #0#0;
    case S[I] of
      #$00..#$7F: Result := Result + #$00 + S[I];
      #$80: Result := Result + #$04#$02;
      #$81: Result := Result + #$04#$03;
      #$82: Result := Result + #$20#$1A;
      #$83: Result := Result + #$04#$53;
      #$84: Result := Result + #$20#$1E;
      #$85: Result := Result + #$20#$26;
      #$86: Result := Result + #$20#$20;
      #$87: Result := Result + #$20#$21;
      #$88: Result := Result + #$20#$AC;
      #$89: Result := Result + #$20#$30;
      #$8A: Result := Result + #$04#$09;
      #$8B: Result := Result + #$20#$39;
      #$8C: Result := Result + #$04#$0A;
      #$8D: Result := Result + #$04#$0C;
      #$8E: Result := Result + #$04#$0B;
      #$8F: Result := Result + #$04#$0F;
      #$90: Result := Result + #$04#$52;
      #$91: Result := Result + #$20#$18;
      #$92: Result := Result + #$20#$19;
      #$93: Result := Result + #$20#$1C;
      #$94: Result := Result + #$20#$1D;
      #$95: Result := Result + #$20#$22;
      #$96: Result := Result + #$20#$13;
      #$97: Result := Result + #$20#$14;
      #$99: Result := Result + #$21#$22;
      #$9A: Result := Result + #$04#$59;
      #$9B: Result := Result + #$20#$3A;
      #$9C: Result := Result + #$04#$5A;
      #$9D: Result := Result + #$04#$5C;
      #$9E: Result := Result + #$04#$5B;
      #$9F: Result := Result + #$04#$5F;
      #$A0: Result := Result + #$00#$A0;
      #$A1: Result := Result + #$04#$0E;
      #$A2: Result := Result + #$04#$5E;
      #$A3: Result := Result + #$04#$08;
      #$A4: Result := Result + #$00#$A4;
      #$A5: Result := Result + #$04#$90;
      #$A6: Result := Result + #$00#$A6;
      #$A7: Result := Result + #$00#$A7;
      #$A8: Result := Result + #$04#$01;
      #$A9: Result := Result + #$00#$A9;
      #$AA: Result := Result + #$04#$04;
      #$AB: Result := Result + #$00#$AB;
      #$AC: Result := Result + #$00#$AC;
      #$AD: Result := Result + #$00#$AD;
      #$AE: Result := Result + #$00#$AE;
      #$AF: Result := Result + #$04#$07;
      #$B0: Result := Result + #$00#$B0;
      #$B1: Result := Result + #$00#$B1;
      #$B2: Result := Result + #$04#$06;
      #$B3: Result := Result + #$04#$56;
      #$B4: Result := Result + #$04#$91;
      #$B5: Result := Result + #$00#$B5;
      #$B6: Result := Result + #$00#$B6;
      #$B7: Result := Result + #$00#$B7;
      #$B8: Result := Result + #$04#$51;
      #$B9: Result := Result + #$21#$16;
      #$BA: Result := Result + #$04#$54;
      #$BB: Result := Result + #$00#$BB;
      #$BC: Result := Result + #$04#$58;
      #$BD: Result := Result + #$04#$05;
      #$BE: Result := Result + #$04#$55;
      #$BF: Result := Result + #$04#$57;
      #$C0..#$FF: Result := Result + #04 + AnsiChar(Ord(S[I]) - $B0);
    else
      raise CreateConvException('Cp1251', 'UCS4', '$' + IntToHex(Ord(S[I]), 2));
    end;
  end;
end;

function UCS4ToCp1251(S: AnsiString): AnsiString;
var
  I: Integer;
begin
  Result := '';
  if Length(S) = 0 then Exit;
  if (Length(S) < 4) or ((Length(S) mod 4) <> 0) then
    raise CreateConvException('UCS4', 'Cp1251', '');
  
  I := 1;
  while I <= Length(S) do
  begin
    if (S[I] <> #0) or (S[I + 1] <> #0) then
      raise CreateConvException('UCS4', 'Cp1251', '$' + IntToHex(Ord(S[I]), 2) + IntToHex(Ord(S[I + 1]), 2) + IntToHex(Ord(S[I+2]), 2) + IntToHex(Ord(S[I + 3]), 2));
    case S[I + 2] of
      #$00:
        begin
          case S[I + 3] of
            #$00..#$7F: Result := Result + S[I + 3];
            #$A0: Result := Result + #$A0;
            #$A4: Result := Result + #$A4;
            #$A6: Result := Result + #$A6;
            #$A7: Result := Result + #$A7;
            #$A9: Result := Result + #$A9;
            #$AB: Result := Result + #$AB;
            #$AC: Result := Result + #$AC;
            #$AD: Result := Result + #$AD;
            #$AE: Result := Result + #$AE;
            #$B0: Result := Result + #$B0;
            #$B1: Result := Result + #$B1;
            #$B5: Result := Result + #$B5;
            #$B6: Result := Result + #$B6;
            #$B7: Result := Result + #$B7;
            #$BB: Result := Result + #$BB;
          else
            raise CreateConvException('UCS4', 'Cp1251', '$' + IntToHex(Ord(S[I]), 2) + IntToHex(Ord(S[I + 1]), 2) + IntToHex(Ord(S[I + 2]), 2) + IntToHex(Ord(S[I + 3]), 2));
          end;
        end;
      #$04:
        begin
          case S[I + 3] of
            #$02: Result := Result + #$80;
            #$03: Result := Result + #$81;
            #$53: Result := Result + #$83;
            #$09: Result := Result + #$8A;
            #$0A: Result := Result + #$8C;
            #$0C: Result := Result + #$8D;
            #$0B: Result := Result + #$8E;
            #$0F: Result := Result + #$8F;
            #$52: Result := Result + #$90;
            #$59: Result := Result + #$9A;
            #$5A: Result := Result + #$9C;
            #$5C: Result := Result + #$9D;
            #$5B: Result := Result + #$9E;
            #$5F: Result := Result + #$9F;
            #$0E: Result := Result + #$A1;
            #$5E: Result := Result + #$A2;
            #$08: Result := Result + #$A3;
            #$90: Result := Result + #$A5;
            #$01: Result := Result + #$A8;
            #$04: Result := Result + #$AA;
            #$07: Result := Result + #$AF;
            #$06: Result := Result + #$B2;
            #$56: Result := Result + #$B3;
            #$91: Result := Result + #$B4;
            #$51: Result := Result + #$B8;
            #$54: Result := Result + #$BA;
            #$58: Result := Result + #$BC;
            #$05: Result := Result + #$BD;
            #$55: Result := Result + #$BE;
            #$57: Result := Result + #$BF;
            #$10..#$4F: Result := Result + AnsiChar(Ord(S[I + 3]) + $B0);
          else
            raise CreateConvException('UCS4', 'Cp1251', '$' + IntToHex(Ord(S[I]), 2) + IntToHex(Ord(S[I + 1]), 2) + IntToHex(Ord(S[I + 2]), 2) + IntToHex(Ord(S[I + 3]), 2));
          end;
        end;
      #$20:
        begin
          case S[I + 3] of
            #$1A: Result := Result + #$82;
            #$1E: Result := Result + #$84;
            #$26: Result := Result + #$85;
            #$20: Result := Result + #$86;
            #$21: Result := Result + #$87;
            #$AC: Result := Result + #$88;
            #$30: Result := Result + #$89;
            #$39: Result := Result + #$8B;
            #$18: Result := Result + #$91;
            #$19: Result := Result + #$92;
            #$1C: Result := Result + #$93;
            #$1D: Result := Result + #$94;
            #$22: Result := Result + #$95;
            #$13: Result := Result + #$96;
            #$14: Result := Result + #$97;
            #$3A: Result := Result + #$9B;
          else
            raise CreateConvException('UCS4', 'Cp1251', '$' + IntToHex(Ord(S[I]), 2) + IntToHex(Ord(S[I + 1]), 2) + IntToHex(Ord(S[I + 2]), 2) + IntToHex(Ord(S[I + 3]), 2));
          end;
        end;
      #$21:
        begin
          case S[I + 3] of
            #$22: Result := Result + #$99;
            #$16: Result := Result + #$B9;
          else
            raise CreateConvException('UCS4', 'Cp1251', '$' + IntToHex(Ord(S[I]), 2) + IntToHex(Ord(S[I + 1]), 2) + IntToHex(Ord(S[I + 2]), 2) + IntToHex(Ord(S[I + 3]), 2));
          end;
        end;
    else
      raise CreateConvException('UCS4', 'Cp1251', '$' + IntToHex(Ord(S[I]), 2));
    end;
    I := I + 4;
  end;
end;
{
function Cp866ToUCS4(S: AnsiString): AnsiString;
var
  I: Integer;

begin
  Result := '';
  for I := 1 to Length(S) do
  begin
    Result := Result + #0#0;
    case S[I] of
      #$00..#$7F: Result := Result + #$00 + S[I];
      #$80..#$AF: Result := Result  + #$04 + AnsiChar(Ord(S[I]) - $70);
      #$B0: Result := Result + #$25#$91;
      #$B1: Result := Result + #$25#$92;
      #$B2: Result := Result + #$25#$93;
      #$B3: Result := Result + #$25#$02;
      #$B4: Result := Result + #$25#$24;
      #$B5: Result := Result + #$25#$61;
      #$B6: Result := Result + #$25#$62;
      #$B7: Result := Result + #$25#$56;
      #$B8: Result := Result + #$25#$55;
      #$B9: Result := Result + #$25#$63;
      #$BA: Result := Result + #$25#$51;
      #$BB: Result := Result + #$25#$57;
      #$BC: Result := Result + #$25#$5D;
      #$BD: Result := Result + #$25#$5C;
      #$BE: Result := Result + #$25#$5B;
      #$BF: Result := Result + #$25#$10;
      #$C0: Result := Result + #$25#$14;
      #$C1: Result := Result + #$25#$34;
      #$C2: Result := Result + #$25#$2C;
      #$C3: Result := Result + #$25#$1C;
      #$C4: Result := Result + #$25#$00;
      #$C5: Result := Result + #$25#$3C;
      #$C6: Result := Result + #$25#$5E;
      #$C7: Result := Result + #$25#$5F;
      #$C8: Result := Result + #$25#$5A;
      #$C9: Result := Result + #$25#$54;
      #$CA: Result := Result + #$25#$69;
      #$CB: Result := Result + #$25#$66;
      #$CC: Result := Result + #$25#$60;
      #$CD: Result := Result + #$25#$50;
      #$CE: Result := Result + #$25#$6C;
      #$CF: Result := Result + #$25#$67;
      #$D0: Result := Result + #$25#$68;
      #$D1: Result := Result + #$25#$64;
      #$D2: Result := Result + #$25#$65;
      #$D3: Result := Result + #$25#$59;
      #$D4: Result := Result + #$25#$58;
      #$D5: Result := Result + #$25#$52;
      #$D6: Result := Result + #$25#$53;
      #$D7: Result := Result + #$25#$6B;
      #$D8: Result := Result + #$25#$6A;
      #$D9: Result := Result + #$25#$18;
      #$DA: Result := Result + #$25#$0C;
      #$DB: Result := Result + #$25#$88;
      #$DC: Result := Result + #$25#$84;
      #$DD: Result := Result + #$25#$8C;
      #$DE: Result := Result + #$25#$90;
      #$DF: Result := Result + #$25#$80;
      #$E0..#$EF: Result := Result + #$04 + AnsiChar(Ord(S[I]) - $A0);
      #$F0: Result := Result + #$04#$01;
      #$F1: Result := Result + #$04#$51;
      #$F2: Result := Result + #$04#$04;
      #$F3: Result := Result + #$04#$54;
      #$F4: Result := Result + #$04#$07;
      #$F5: Result := Result + #$04#$57;
      #$F6: Result := Result + #$04#$0E;
      #$F7: Result := Result + #$04#$5E;
      #$F8: Result := Result + #$00#$B0;
      #$F9: Result := Result + #$22#$19;
      #$FA: Result := Result + #$00#$B7;
      #$FB: Result := Result + #$22#$1A;
      #$FC: Result := Result + #$21#$16;
      #$FD: Result := Result + #$00#$A4;
      #$FE: Result := Result + #$25#$A0;
      #$FF: Result := Result + #$00#$A0;
    else
      raise CreateConvException('Cp866', 'Unicode', '$' + IntToHex(Ord(S[I]), 2));
    end;
  end;
end;}

function UCS4ToCp866(S: AnsiString): AnsiString;
var
  I: Integer;
begin
  Result := '';
  if Length(S) = 0 then Exit;
  if (Length(S) < 4) or ((Length(S) mod 4) <> 0) then
    raise CreateConvException('UCS4', 'Cp866', '');

  I := 1;
  while I <= Length(S) do
  begin
    if (S[I] <> #0) or (S[I + 1] <> #0) then
      raise CreateConvException('UCS4', 'Cp866', '$' + IntToHex(Ord(S[I]), 2) + IntToHex(Ord(S[I + 1]), 2) + IntToHex(Ord(S[I + 2]), 2) + IntToHex(Ord(S[I + 3]), 2));
    case S[I + 2] of
      #$00:
        begin
          case S[I + 3] of
            #$00..#$7F: Result := Result + S[I + 3];
            #$B0: Result := Result + #$F8;
            #$B7: Result := Result + #$FA;
            #$A4: Result := Result + #$FD;
            #$A0: Result := Result + #$FF;
          else
            raise CreateConvException('UCS4', 'Cp866', '$' + IntToHex(Ord(S[I]), 2) + IntToHex(Ord(S[I + 1]), 2) + IntToHex(Ord(S[I + 2]), 2) + IntToHex(Ord(S[I + 3]), 2));
          end;
        end;
      #$04:
        begin
          case S[I + 3] of
            #$10..#$3F: Result := Result + AnsiChar(Ord(S[I + 3]) + $70);
            #$40..#$4F: Result := Result + AnsiChar(Ord(S[I + 3]) + $A0);
            #$01: Result := Result + #$F0;
            #$51: Result := Result + #$F1;
            #$04: Result := Result + #$F2;
            #$54: Result := Result + #$F3;
            #$07: Result := Result + #$F4;
            #$57: Result := Result + #$F5;
            #$0E: Result := Result + #$F6;
            #$5E: Result := Result + #$F7;
          else
            raise CreateConvException('UCS4', 'Cp866', '$' + IntToHex(Ord(S[I]), 2) + IntToHex(Ord(S[I + 1]), 2) + IntToHex(Ord(S[I + 2]), 2) + IntToHex(Ord(S[I + 3]), 2));
          end;
        end;
      #$21:
        begin
          case S[I + 3] of
            #$16: Result := Result + #$FC;
          else
            raise CreateConvException('UCS4', 'Cp866', '$' + IntToHex(Ord(S[I]), 2) + IntToHex(Ord(S[I + 1]), 2) + IntToHex(Ord(S[I + 2]), 2) + IntToHex(Ord(S[I + 3]), 2));
          end;
        end;
      #$22:
        begin
          case S[I + 3] of
            #$19: Result := Result + #$F9;
            #$1A: Result := Result + #$FB;
          else
            raise CreateConvException('UCS4', 'Cp866', '$' + IntToHex(Ord(S[I]), 2) + IntToHex(Ord(S[I + 1]), 2) + IntToHex(Ord(S[I + 2]), 2) + IntToHex(Ord(S[I + 3]), 2));
          end;
        end;
      #$25:
        begin
          case S[I + 3] of
            #$91: Result := Result + #$B0;
            #$92: Result := Result + #$B1;
            #$93: Result := Result + #$B2;
            #$02: Result := Result + #$B3;
            #$24: Result := Result + #$B4;
            #$61: Result := Result + #$B5;
            #$62: Result := Result + #$B6;
            #$56: Result := Result + #$B7;
            #$55: Result := Result + #$B8;
            #$63: Result := Result + #$B9;
            #$51: Result := Result + #$BA;
            #$57: Result := Result + #$BB;
            #$5D: Result := Result + #$BC;
            #$5C: Result := Result + #$BD;
            #$5B: Result := Result + #$BE;
            #$10: Result := Result + #$BF;
            #$14: Result := Result + #$C0;
            #$34: Result := Result + #$C1;
            #$2C: Result := Result + #$C2;
            #$1C: Result := Result + #$C3;
            #$00: Result := Result + #$C4;
            #$3C: Result := Result + #$C5;
            #$5E: Result := Result + #$C6;
            #$5F: Result := Result + #$C7;
            #$5A: Result := Result + #$C8;
            #$54: Result := Result + #$C9;
            #$69: Result := Result + #$CA;
            #$66: Result := Result + #$CB;
            #$60: Result := Result + #$CC;
            #$50: Result := Result + #$CD;
            #$6C: Result := Result + #$CE;
            #$67: Result := Result + #$CF;
            #$68: Result := Result + #$D0;
            #$64: Result := Result + #$D1;
            #$65: Result := Result + #$D2;
            #$59: Result := Result + #$D3;
            #$58: Result := Result + #$D4;
            #$52: Result := Result + #$D5;
            #$53: Result := Result + #$D6;
            #$6B: Result := Result + #$D7;
            #$6A: Result := Result + #$D8;
            #$18: Result := Result + #$D9;
            #$0C: Result := Result + #$DA;
            #$88: Result := Result + #$DB;
            #$84: Result := Result + #$DC;
            #$8C: Result := Result + #$DD;
            #$90: Result := Result + #$DE;
            #$80: Result := Result + #$DF;
            #$A0: Result := Result + #$FE;
          else
            raise CreateConvException('UCS4', 'Cp866', '$' + IntToHex(Ord(S[I]), 2) + IntToHex(Ord(S[I + 1]), 2) + IntToHex(Ord(S[I + 2]), 2) + IntToHex(Ord(S[I + 3]), 2));
          end;
        end;
    else
      raise CreateConvException('UCS4', 'Cp866', '$' + IntToHex(Ord(S[I]), 2));
    end;
    I := I + 4;
  end;
end;

function Koi8rToUCS4(S: AnsiString): AnsiString;
var
  I: Integer;
begin
  Result := '';
  for I := 1 to Length(S) do
  begin
    Result := Result + #0#0;
    case S[I] of
      #$00..#$7F: Result := Result + #$00 + S[I];
      #$80: Result := Result + #$25#$00;
      #$81: Result := Result + #$25#$02;
      #$82: Result := Result + #$25#$0C;
      #$83: Result := Result + #$25#$10;
      #$84: Result := Result + #$25#$14;
      #$85: Result := Result + #$25#$18;
      #$86: Result := Result + #$25#$1C;
      #$87: Result := Result + #$25#$24;
      #$88: Result := Result + #$25#$2C;
      #$89: Result := Result + #$25#$34;
      #$8A: Result := Result + #$25#$3C;
      #$8B: Result := Result + #$25#$80;
      #$8C: Result := Result + #$25#$84;
      #$8D: Result := Result + #$25#$88;
      #$8E: Result := Result + #$25#$8C;
      #$8F: Result := Result + #$25#$90;
      #$90: Result := Result + #$25#$91;
      #$91: Result := Result + #$25#$92;
      #$92: Result := Result + #$25#$93;
      #$93: Result := Result + #$23#$20;
      #$94: Result := Result + #$25#$A0;
      #$95: Result := Result + #$22#$19;
      #$96: Result := Result + #$22#$1A;
      #$97: Result := Result + #$22#$48;
      #$98: Result := Result + #$22#$64;
      #$99: Result := Result + #$22#$65;
      #$9A: Result := Result + #$00#$A0;
      #$9B: Result := Result + #$23#$21;
      #$9C: Result := Result + #$00#$B0;
      #$9D: Result := Result + #$00#$B2;
      #$9E: Result := Result + #$00#$B7;
      #$9F: Result := Result + #$00#$F7;
      #$A0: Result := Result + #$25#$50;
      #$A1: Result := Result + #$25#$51;
      #$A2: Result := Result + #$25#$52;
      #$A3: Result := Result + #$04#$51;
      #$A4: Result := Result + #$25#$53;
      #$A5: Result := Result + #$25#$54;
      #$A6: Result := Result + #$25#$55;
      #$A7: Result := Result + #$25#$56;
      #$A8: Result := Result + #$25#$57;
      #$A9: Result := Result + #$25#$58;
      #$AA: Result := Result + #$25#$59;
      #$AB: Result := Result + #$25#$5A;
      #$AC: Result := Result + #$25#$5B;
      #$AD: Result := Result + #$25#$5C;
      #$AE: Result := Result + #$25#$5D;
      #$AF: Result := Result + #$25#$5E;
      #$B0: Result := Result + #$25#$5F;
      #$B1: Result := Result + #$25#$60;
      #$B2: Result := Result + #$25#$61;
      #$B3: Result := Result + #$04#$01;
      #$B4: Result := Result + #$25#$62;
      #$B5: Result := Result + #$25#$63;
      #$B6: Result := Result + #$25#$64;
      #$B7: Result := Result + #$25#$65;
      #$B8: Result := Result + #$25#$66;
      #$B9: Result := Result + #$25#$67;
      #$BA: Result := Result + #$25#$68;
      #$BB: Result := Result + #$25#$69;
      #$BC: Result := Result + #$25#$6A;
      #$BD: Result := Result + #$25#$6B;
      #$BE: Result := Result + #$25#$6C;
      #$BF: Result := Result + #$00#$A9;
      #$C0: Result := Result + #$04#$4E;
      #$C1: Result := Result + #$04#$30;
      #$C2: Result := Result + #$04#$31;
      #$C3: Result := Result + #$04#$46;
      #$C4: Result := Result + #$04#$34;
      #$C5: Result := Result + #$04#$35;
      #$C6: Result := Result + #$04#$44;
      #$C7: Result := Result + #$04#$33;
      #$C8: Result := Result + #$04#$45;
      #$C9: Result := Result + #$04#$38;
      #$CA: Result := Result + #$04#$39;
      #$CB: Result := Result + #$04#$3A;
      #$CC: Result := Result + #$04#$3B;
      #$CD: Result := Result + #$04#$3C;
      #$CE: Result := Result + #$04#$3D;
      #$CF: Result := Result + #$04#$3E;
      #$D0: Result := Result + #$04#$3F;
      #$D1: Result := Result + #$04#$4F;
      #$D2: Result := Result + #$04#$40;
      #$D3: Result := Result + #$04#$41;
      #$D4: Result := Result + #$04#$42;
      #$D5: Result := Result + #$04#$43;
      #$D6: Result := Result + #$04#$36;
      #$D7: Result := Result + #$04#$32;
      #$D8: Result := Result + #$04#$4C;
      #$D9: Result := Result + #$04#$4B;
      #$DA: Result := Result + #$04#$37;
      #$DB: Result := Result + #$04#$48;
      #$DC: Result := Result + #$04#$4D;
      #$DD: Result := Result + #$04#$49;
      #$DE: Result := Result + #$04#$47;
      #$DF: Result := Result + #$04#$4A;
      #$E0: Result := Result + #$04#$2E;
      #$E1: Result := Result + #$04#$10;
      #$E2: Result := Result + #$04#$11;
      #$E3: Result := Result + #$04#$26;
      #$E4: Result := Result + #$04#$14;
      #$E5: Result := Result + #$04#$15;
      #$E6: Result := Result + #$04#$24;
      #$E7: Result := Result + #$04#$13;
      #$E8: Result := Result + #$04#$25;
      #$E9: Result := Result + #$04#$18;
      #$EA: Result := Result + #$04#$19;
      #$EB: Result := Result + #$04#$1A;
      #$EC: Result := Result + #$04#$1B;
      #$ED: Result := Result + #$04#$1C;
      #$EE: Result := Result + #$04#$1D;
      #$EF: Result := Result + #$04#$1E;
      #$F0: Result := Result + #$04#$1F;
      #$F1: Result := Result + #$04#$2F;
      #$F2: Result := Result + #$04#$20;
      #$F3: Result := Result + #$04#$21;
      #$F4: Result := Result + #$04#$22;
      #$F5: Result := Result + #$04#$23;
      #$F6: Result := Result + #$04#$16;
      #$F7: Result := Result + #$04#$12;
      #$F8: Result := Result + #$04#$2C;
      #$F9: Result := Result + #$04#$2B;
      #$FA: Result := Result + #$04#$17;
      #$FB: Result := Result + #$04#$28;
      #$FC: Result := Result + #$04#$2D;
      #$FD: Result := Result + #$04#$29;
      #$FE: Result := Result + #$04#$27;
      #$FF: Result := Result + #$04#$2A;
    else
      raise CreateConvException('Koi8r', 'UCS4', '$' + IntToHex(Ord(S[I]), 2));
    end;
  end;
end;

function UCS4ToKoi8r(S: AnsiString): AnsiString;
var
  I: Integer;
begin
  Result := '';
  if Length(S) = 0 then Exit;
  if (Length(S) < 4) or ((Length(S) mod 4) <> 0) then
    raise CreateConvException('UCS4', 'Koi8r', '');

  I := 1;
  while I <= Length(S) do
  begin
    if (S[I] <> #0) or (S[I + 1] <> #0) then
      raise CreateConvException('UCS4', 'Koi8r', '$' + IntToHex(Ord(S[I]), 2) + IntToHex(Ord(S[I + 1]), 2) + IntToHex(Ord(S[I + 2]), 2) + IntToHex(Ord(S[I + 3]), 2));
    case S[I + 2] of
      #$00:
        begin
          case S[I + 3] of
            #$00..#$7F: Result := Result + S[I + 3];
            #$A0: Result := Result + #$9A;
            #$B0: Result := Result + #$9C;
            #$B2: Result := Result + #$9D;
            #$B7: Result := Result + #$9E;
            #$F7: Result := Result + #$9F;
            #$A9: Result := Result + #$BF;
          else
            raise CreateConvException('UCS4', 'Koi8r', '$' + IntToHex(Ord(S[I]), 2) + IntToHex(Ord(S[I + 1]), 2) + IntToHex(Ord(S[I + 2]), 2) + IntToHex(Ord(S[I + 3]), 2));
          end;
        end;
      #$04:
        begin
          case S[I + 3] of
            #$51: Result := Result + #$A3;
            #$01: Result := Result + #$B3;
            #$4E: Result := Result + #$C0;
            #$30: Result := Result + #$C1;
            #$31: Result := Result + #$C2;
            #$46: Result := Result + #$C3;
            #$34: Result := Result + #$C4;
            #$35: Result := Result + #$C5;
            #$44: Result := Result + #$C6;
            #$33: Result := Result + #$C7;
            #$45: Result := Result + #$C8;
            #$38: Result := Result + #$C9;
            #$39: Result := Result + #$CA;
            #$3A: Result := Result + #$CB;
            #$3B: Result := Result + #$CC;
            #$3C: Result := Result + #$CD;
            #$3D: Result := Result + #$CE;
            #$3E: Result := Result + #$CF;
            #$3F: Result := Result + #$D0;
            #$4F: Result := Result + #$D1;
            #$40: Result := Result + #$D2;
            #$41: Result := Result + #$D3;
            #$42: Result := Result + #$D4;
            #$43: Result := Result + #$D5;
            #$36: Result := Result + #$D6;
            #$32: Result := Result + #$D7;
            #$4C: Result := Result + #$D8;
            #$4B: Result := Result + #$D9;
            #$37: Result := Result + #$DA;
            #$48: Result := Result + #$DB;
            #$4D: Result := Result + #$DC;
            #$49: Result := Result + #$DD;
            #$47: Result := Result + #$DE;
            #$4A: Result := Result + #$DF;
            #$2E: Result := Result + #$E0;
            #$10: Result := Result + #$E1;
            #$11: Result := Result + #$E2;
            #$26: Result := Result + #$E3;
            #$14: Result := Result + #$E4;
            #$15: Result := Result + #$E5;
            #$24: Result := Result + #$E6;
            #$13: Result := Result + #$E7;
            #$25: Result := Result + #$E8;
            #$18: Result := Result + #$E9;
            #$19: Result := Result + #$EA;
            #$1A: Result := Result + #$EB;
            #$1B: Result := Result + #$EC;
            #$1C: Result := Result + #$ED;
            #$1D: Result := Result + #$EE;
            #$1E: Result := Result + #$EF;
            #$1F: Result := Result + #$F0;
            #$2F: Result := Result + #$F1;
            #$20: Result := Result + #$F2;
            #$21: Result := Result + #$F3;
            #$22: Result := Result + #$F4;
            #$23: Result := Result + #$F5;
            #$16: Result := Result + #$F6;
            #$12: Result := Result + #$F7;
            #$2C: Result := Result + #$F8;
            #$2B: Result := Result + #$F9;
            #$17: Result := Result + #$FA;
            #$28: Result := Result + #$FB;
            #$2D: Result := Result + #$FC;
            #$29: Result := Result + #$FD;
            #$27: Result := Result + #$FE;
            #$2A: Result := Result + #$FF;
          else
            raise CreateConvException('UCS4', 'Koi8r', '$' + IntToHex(Ord(S[I]), 2) + IntToHex(Ord(S[I + 1]), 2) + IntToHex(Ord(S[I + 2]), 2) + IntToHex(Ord(S[I + 3]), 2));
          end;
        end;
      #$22:
        begin
          case S[I + 3] of
            #$19: Result := Result + #$95;
            #$1A: Result := Result + #$96;
            #$48: Result := Result + #$97;
            #$64: Result := Result + #$98;
            #$65: Result := Result + #$99;
          else
            raise CreateConvException('UCS4', 'Koi8r', '$' + IntToHex(Ord(S[I]), 2) + IntToHex(Ord(S[I + 1]), 2) + IntToHex(Ord(S[I + 2]), 2) + IntToHex(Ord(S[I + 3]), 2));
          end;
        end;
      #$23:
        begin
          case S[I + 3] of
            #$20: Result := Result + #$93;
            #$21: Result := Result + #$9B;
          else
            raise CreateConvException('UCS4', 'Koi8r', '$' + IntToHex(Ord(S[I]), 2) + IntToHex(Ord(S[I + 1]), 2) + IntToHex(Ord(S[I + 2]), 2) + IntToHex(Ord(S[I + 3]), 2));
          end;
        end;
      #$25:
        begin
          case S[I + 3] of
            #$00: Result := Result + #$80;
            #$02: Result := Result + #$81;
            #$0C: Result := Result + #$82;
            #$10: Result := Result + #$83;
            #$14: Result := Result + #$84;
            #$18: Result := Result + #$85;
            #$1C: Result := Result + #$86;
            #$24: Result := Result + #$87;
            #$2C: Result := Result + #$88;
            #$34: Result := Result + #$89;
            #$3C: Result := Result + #$8A;
            #$80: Result := Result + #$8B;
            #$84: Result := Result + #$8C;
            #$88: Result := Result + #$8D;
            #$8C: Result := Result + #$8E;
            #$90: Result := Result + #$8F;
            #$91: Result := Result + #$90;
            #$92: Result := Result + #$91;
            #$93: Result := Result + #$92;
            #$A0: Result := Result + #$94;
            #$50: Result := Result + #$A0;
            #$51: Result := Result + #$A1;
            #$52: Result := Result + #$A2;
            #$53: Result := Result + #$A4;
            #$54: Result := Result + #$A5;
            #$55: Result := Result + #$A6;
            #$56: Result := Result + #$A7;
            #$57: Result := Result + #$A8;
            #$58: Result := Result + #$A9;
            #$59: Result := Result + #$AA;
            #$5A: Result := Result + #$AB;
            #$5B: Result := Result + #$AC;
            #$5C: Result := Result + #$AD;
            #$5D: Result := Result + #$AE;
            #$5E: Result := Result + #$AF;
            #$5F: Result := Result + #$B0;
            #$60: Result := Result + #$B1;
            #$61: Result := Result + #$B2;
            #$62: Result := Result + #$B4;
            #$63: Result := Result + #$B5;
            #$64: Result := Result + #$B6;
            #$65: Result := Result + #$B7;
            #$66: Result := Result + #$B8;
            #$67: Result := Result + #$B9;
            #$68: Result := Result + #$BA;
            #$69: Result := Result + #$BB;
            #$6A: Result := Result + #$BC;
            #$6B: Result := Result + #$BD;
            #$6C: Result := Result + #$BE;
          else
            raise CreateConvException('UCS4', 'Koi8r', '$' + IntToHex(Ord(S[I]), 2) + IntToHex(Ord(S[I + 1]), 2) + IntToHex(Ord(S[I + 2]), 2) + IntToHex(Ord(S[I + 3]), 2));
          end;
        end;
    else
      raise CreateConvException('UCS4', 'Koi8r', '$' + IntToHex(Ord(S[I]), 2));
    end;
    I := I + 4;
  end;
end;

function ISO88595ToUCS4(S: AnsiString): AnsiString;
var
  I: Integer;
begin
  Result := '';
  for I := 1 to Length(S) do
  begin
    Result := Result + #0#0;
    case S[I] of
      #$00..#$A0: Result := Result + #$00 + S[I];
      #$A1..#$AC: Result := Result  + #$04 + AnsiChar(Ord(S[I]) - $A0);
      #$AD: Result := Result + #$00#$AD;
      #$AE..#$EF: Result := Result  + #$04 + AnsiChar(Ord(S[I]) - $A0);
      #$F0: Result := Result + #$21#$16;
      #$F1..#$FC: Result := Result  + #$04 + AnsiChar(Ord(S[I]) - $A0);
      #$FD: Result := Result + #$00#$A7;
      #$FE: Result := Result + #$04#$5E;
      #$FF: Result := Result + #$04#$5F;
    end;
  end;
end;

function UCS4ToISO88595(S: AnsiString): AnsiString;
var
  I: Integer;
begin
  Result := '';
  if Length(S) = 0 then Exit;
  if (Length(S) < 4) or ((Length(S) mod 4) <> 0) then
    raise CreateConvException('UCS4', 'ISO 8859-5', '');

  I := 1;
  while I <= Length(S) do
  begin
    if (S[I] <> #0) or (S[I + 1] <> #0) then
      raise CreateConvException('UCS4', 'ISO 8859-5', '$' + IntToHex(Ord(S[I]), 2) + IntToHex(Ord(S[I + 1]), 2) + IntToHex(Ord(S[I + 2]), 2) + IntToHex(Ord(S[I + 3]), 2));
    case S[I + 2] of
      #$00:
        begin
          case S[I + 3] of
            #$00..#$A0: Result := Result + S[I + 3];
            #$AD: Result := Result + #$AD;
            #$A7: Result := Result + #$FD;
          else
            raise CreateConvException('UCS4', 'ISO 8859-5', '$' + IntToHex(Ord(S[I]), 2) + IntToHex(Ord(S[I + 1]), 2) + IntToHex(Ord(S[I + 2]), 2) + IntToHex(Ord(S[I + 3]), 2));
          end;
        end;
      #$04:
        begin
          case S[I + 3] of
            #$01..#$0C: Result := Result + AnsiChar(Ord(S[I + 3]) + $A0);
            #$0E..#$4F: Result := Result + AnsiChar(Ord(S[I + 3]) + $A0);
            #$51..#$5C: Result := Result + AnsiChar(Ord(S[I + 3]) + $A0);
            #$5E: Result := Result + #$FE;
            #$5F: Result := Result + #$FF;
          else
            raise CreateConvException('UCS4', 'ISO 8859-5', '$' + IntToHex(Ord(S[I]), 2) + IntToHex(Ord(S[I + 1]), 2) + IntToHex(Ord(S[I + 2]), 2) + IntToHex(Ord(S[I + 3]), 2));
          end;
        end;
      #$21:
        begin
          case S[I + 3] of
            #$16: Result := Result + #$F0;
          else
            raise CreateConvException('UCS4', 'ISO 8859-5', '$' + IntToHex(Ord(S[I]), 2) + IntToHex(Ord(S[I + 1]), 2) + IntToHex(Ord(S[I + 2]), 2) + IntToHex(Ord(S[I + 3]), 2));
          end;
        end;
    else
      raise CreateConvException('UCS4', 'ISO 8859-5', '$' + IntToHex(Ord(S[I]), 2));
    end;
    I := I + 4;
  end;
end;

end.

