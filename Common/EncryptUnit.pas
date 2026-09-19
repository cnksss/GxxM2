unit EncryptUnit;

interface
uses
  Windows, SysUtils, Classes;
function DecryStrHex(StrHex: string): string;
function EncryStrHex(Str: string): string;

function DecryScript(Str: string): string;
function EncryScript(Str: string): string;

function DecryString(Str: string): string;
function EncryString(Str: string): string;

function EncryBuffer(Buf: PChar; Bufsize: Integer): string;
procedure DecryBuffer(Src: string; Buf: PChar; Bufsize: Integer);



function EncryBufferK(Buf: PChar; Bufsize: Integer; Key: string): string;
procedure DecryBufferK(Src: string; Buf: PChar; Bufsize: Integer; Key: string);
function zEncryBufferK(Buf: PChar; Bufsize: Integer; Key: string): string;
procedure zDecryBufferK(Src: string; Buf: PChar; Bufsize: Integer; Key: string);
function EncryStringK(const Src, Key: string): string;
function DecryStringK(const Src, Key: string): string;

function zEncryStringK(const Src, Key: string): string;
function zDecryStringK(const Src, Key: string): string;

function EncryBufferA(Buf: PChar; Bufsize: Integer): string; overload;
procedure EncryBufferA(Buf: PChar; Bufsize: Integer; OutBuf: PChar); overload;
procedure DecryBufferA(Src: string; Buf: PChar; Bufsize: Integer); overload;
procedure DecryBufferA(Source: PChar; SourceSize: Integer; Outdata: PChar); overload;

function DecryBufferA(Buf: PChar; Bufsize: Integer): string; overload;
function DecryBufferA(Src: string): string; overload;

function GetKeyValue(Value: Integer): string; overload;
function GetKeyValue: Integer; overload;

function GetRandomValue(Value: Integer): string; overload;
function GetRandomValue: Integer; overload;

implementation
uses ZlibEx, EDcode, UnitDes, Base64;
const
  EncryKey = 20120101;
{
Index:0 Chr(Index):48
Index:1 Chr(Index):49
Index:2 Chr(Index):50
Index:3 Chr(Index):51
Index:4 Chr(Index):52
Index:5 Chr(Index):53
Index:6 Chr(Index):54
Index:7 Chr(Index):55
Index:8 Chr(Index):56
Index:9 Chr(Index):57

Index:0 B64[I]:65 Chr(B64[I]):A
Index:1 B64[I]:66 Chr(B64[I]):B
Index:2 B64[I]:67 Chr(B64[I]):C
Index:3 B64[I]:68 Chr(B64[I]):D
Index:4 B64[I]:69 Chr(B64[I]):E
Index:5 B64[I]:70 Chr(B64[I]):F
Index:6 B64[I]:71 Chr(B64[I]):G
Index:7 B64[I]:72 Chr(B64[I]):H
Index:8 B64[I]:73 Chr(B64[I]):I
Index:9 B64[I]:74 Chr(B64[I]):J
Index:10 B64[I]:75 Chr(B64[I]):K
Index:11 B64[I]:76 Chr(B64[I]):L
Index:12 B64[I]:77 Chr(B64[I]):M
Index:13 B64[I]:78 Chr(B64[I]):N
Index:14 B64[I]:79 Chr(B64[I]):O
Index:15 B64[I]:80 Chr(B64[I]):P
Index:16 B64[I]:81 Chr(B64[I]):Q
Index:17 B64[I]:82 Chr(B64[I]):R
Index:18 B64[I]:83 Chr(B64[I]):S
Index:19 B64[I]:84 Chr(B64[I]):T
Index:20 B64[I]:85 Chr(B64[I]):U
Index:21 B64[I]:86 Chr(B64[I]):V
Index:22 B64[I]:87 Chr(B64[I]):W
Index:23 B64[I]:88 Chr(B64[I]):X
Index:24 B64[I]:89 Chr(B64[I]):Y
Index:25 B64[I]:90 Chr(B64[I]):Z

Index:26 B64[I]:97 Chr(B64[I]):a
Index:27 B64[I]:98 Chr(B64[I]):b
Index:28 B64[I]:99 Chr(B64[I]):c
Index:29 B64[I]:100 Chr(B64[I]):d
Index:30 B64[I]:101 Chr(B64[I]):e
Index:31 B64[I]:102 Chr(B64[I]):f
Index:32 B64[I]:103 Chr(B64[I]):g
Index:33 B64[I]:104 Chr(B64[I]):h
Index:34 B64[I]:105 Chr(B64[I]):i
Index:35 B64[I]:106 Chr(B64[I]):j
Index:36 B64[I]:107 Chr(B64[I]):k
Index:37 B64[I]:108 Chr(B64[I]):l
Index:38 B64[I]:109 Chr(B64[I]):m
Index:39 B64[I]:110 Chr(B64[I]):n
Index:40 B64[I]:111 Chr(B64[I]):o
Index:41 B64[I]:112 Chr(B64[I]):p
Index:42 B64[I]:113 Chr(B64[I]):q
Index:43 B64[I]:114 Chr(B64[I]):r
Index:44 B64[I]:115 Chr(B64[I]):s
Index:45 B64[I]:116 Chr(B64[I]):t
Index:46 B64[I]:117 Chr(B64[I]):u
Index:47 B64[I]:118 Chr(B64[I]):v
Index:48 B64[I]:119 Chr(B64[I]):w
Index:49 B64[I]:120 Chr(B64[I]):x
Index:50 B64[I]:121 Chr(B64[I]):y
Index:51 B64[I]:122 Chr(B64[I]):z
Index:52 B64[I]:60 Chr(B64[I]):<
Index:53 B64[I]:61 Chr(B64[I]):=
Index:54 B64[I]:62 Chr(B64[I]):>
Index:55 B64[I]:64 Chr(B64[I]):@
Index:56 B64[I]:91 Chr(B64[I]):[
Index:57 B64[I]:92 Chr(B64[I]):\
Index:58 B64[I]:93 Chr(B64[I]):]
}


function GetRandomValue(Value: Integer): string;
const
  Random55: array[0..54] of Byte =
    // 0~9 没有0和1
  (50, 51, 52, 53, 54, 55, 56, 57,
   // A~Z 没有73:I 79:O
    65, 66, 67, 68, 69, 70, 71, 72, 74, 75, 76, 77, 78, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90,
    // a~z 没有105:I 108:l 111:o
    97, 98, 99, 100, 101, 102, 103, 104, 106, 107, 109, 110, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122);
var
  btIdx: Byte;
  P: PChar;
begin
  SetLength(Result, 4);
  P := PChar(Result);

  btIdx := LoByte(LoWord(Value));
  P^ := Chr(Random55[btIdx]);
  Inc(P);

  btIdx := HiByte(LoWord(Value));
  P^ := Chr(Random55[btIdx]);
  Inc(P);

  btIdx := LoByte(HiWord(Value));
  P^ := Chr(Random55[btIdx]);
  Inc(P);

  btIdx := HiByte(HiWord(Value));
  P^ := Chr(Random55[btIdx]);
end;

function GetRandomValue: Integer;
begin
  Result := MakeLong(MakeWord(Random(55), Random(55)), MakeWord(Random(55), Random(55)));
end;

function GetKeyValue: Integer;
begin
  Result := MakeLong(MakeWord(Random(58), Random(58)), MakeWord(Random(58), Random(58)));
end;

function GetKeyValue(Value: Integer): string;
const
  B64: array[0..58] of Byte = (65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80,
    81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108,
    109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 60, 61, 62, 64, 91, 92, 93);
var
  btIdx: Byte;
  P: PChar;
begin
  SetLength(Result, 4);
  P := PChar(Result);

  btIdx := LoByte(LoWord(Value));
  P^ := Chr(B64[btIdx]);
  Inc(P);

  btIdx := HiByte(LoWord(Value));
  P^ := Chr(B64[btIdx]);
  Inc(P);

  btIdx := LoByte(HiWord(Value));
  P^ := Chr(B64[btIdx]);
  Inc(P);

  btIdx := HiByte(HiWord(Value));
  P^ := Chr(B64[btIdx]);
end;

function DecryScript(Str: string): string;
begin
  Result := DecryptStrDes(Str, IntToStr(EncryKey));
end;

function EncryScript(Str: string): string;
begin
  Result := EncryptStrDes(Str, IntToStr(EncryKey));
end;

// /////////////////////////////////////////////////////////  442517066

function DecryString(Str: string): string;
var
  Key: string;
begin
  if Length(Str) > 4 then
  begin
    Key := Str[1] + Str[2] + Str[Length(Str) - 1] + Str[Length(Str)];
    Str := Copy(Str, 3, Length(Str) - 4);
    Result := DecryptStrDes(Base64DecodeStr(DecodeString(Str)), Key);
  end
  else
    Result := '';
end;

function EncryString(Str: string): string;
begin
  if Str <> '' then
    Result := EncryBuffer(@Str[1], Length(Str))
  else
    Result := '';
end;


function EncryBufferA(Buf: PChar; Bufsize: Integer): string;
begin
  SetLength(Result, Bufsize);
  EncryptDes(Buf^, Result[1], Bufsize, IntToStr(EncryKey));
end;

procedure EncryBufferA(Buf: PChar; Bufsize: Integer; OutBuf: PChar);
begin
  EncryptDes(Buf^, OutBuf^, Bufsize, IntToStr(EncryKey));
end;

procedure DecryBufferA(Src: string; Buf: PChar; Bufsize: Integer);
begin
  DecryptDes(Src[1], Buf^, Bufsize, IntToStr(EncryKey));
end;

function DecryBufferA(Buf: PChar; Bufsize: Integer): string;
begin
  SetLength(Result, Bufsize);
  DecryptDes(Buf^, Result[1], Bufsize, IntToStr(EncryKey));
end;

function DecryBufferA(Src: string): string;
begin
  Result := DecryptStrDes(Src, IntToStr(EncryKey));
end;

procedure DecryBufferA(Source: PChar; SourceSize: Integer; Outdata: PChar);
begin
  DecryptDes(Source^, Outdata^, SourceSize, IntToStr(EncryKey));
end;

function EncryBufferK(Buf: PChar; Bufsize: Integer; Key: string): string;
var
  sText: string;
begin
  if Bufsize > 0 then
  begin
    SetLength(sText, Bufsize);
    EncryptDes(Buf^, sText[1], Bufsize, Key);
    Result := EncodeString(Base64EncodeStr(sText));
  end
  else
    Result := '';
end;

function zEncryBufferK(Buf: PChar; Bufsize: Integer; Key: string): string;
var
  sText: string;
  OutBuf: PChar; OutBytes: Integer;
begin
  sText := '';
  if Bufsize > 0 then
  begin
    OutBuf := nil;
    OutBytes := 0;
    try
      CompressBuf(Buf, Bufsize, Pointer(OutBuf), OutBytes);
      SetLength(sText, OutBytes);
      Move(OutBuf^, sText[1], OutBytes);
    finally
      FreeMem(OutBuf);
    end;
    if sText <> '' then
    begin
      SetLength(Result, Length(sText));
      EncryptDes(sText[1], Result[1], Length(sText), Key);
      Result := EncodeStringK(Base64EncodeStr(Result));
    end
    else
      Result := '';
  end
  else
    Result := '';
end;

procedure zDecryBufferK(Src: string; Buf: PChar; Bufsize: Integer; Key: string);
var
  sText, sTemp: string;
  OutBuf: Pointer; OutBytes: Integer;
begin
  if Length(Src) > 0 then
  begin

    sText := Base64DecodeStr(DecodeString(Src));                                                    // DecodeString    DecryStrHex
    SetLength(sTemp, Length(sText));
    DecryptDes(sText[1], sTemp[1], Length(sText), Key);

    if sTemp <> '' then
    begin
      OutBuf := nil;
      OutBytes := 0;
      try
        DecompressBuf(@sTemp[1], Length(sTemp), 0, OutBuf, OutBytes);
        Move(OutBuf^, Buf^, OutBytes);
      finally
        FreeMem(OutBuf);
      end;
    end;
  end;
end;


function EncryStringK(const Src, Key: string): string;
begin
  if Src <> '' then
    Result := EncodeString(Base64EncodeStr(EncryptStrDes(Src, Key)))
  else
    Result := '';
end;

function zEncryStringK(const Src, Key: string): string;
begin
  if Src <> '' then
    Result := zEncodeString(Base64EncodeStr(EncryptStrDes(Src, Key)))
  else
    Result := '';
end;

procedure DecryBufferK(Src: string; Buf: PChar; Bufsize: Integer; Key: string);
var
  sText: string;
begin
  if Length(Src) > 0 then
  begin
    sText := Base64DecodeStr(DecodeStringK(Src));                                                   // DecodeString    DecryStrHex
    DecryptDes(sText[1], Buf^, Bufsize, Key);
  end;
end;



function DecryStringK(const Src, Key: string): string;
begin
  if Src <> '' then
    Result := DecryptStrDes(Base64DecodeStr(DecodeStringK(Src)), Key)
  else
    Result := '';
end;

function zDecryStringK(const Src, Key: string): string;
begin
  if Src <> '' then
    Result := DecryptStrDes(Base64DecodeStr(zDecodeString(Src)), Key)
  else
    Result := '';
end;

function EncryBuffer(Buf: PChar; Bufsize: Integer): string;
const
  B64: array[0..58] of byte = (65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80,
    81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108,
    109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 60, 61, 62, 64, 91, 92, 93);
var
  I: Integer;
  Key: string;
  P: PChar;
  sText: string;
begin
  if Bufsize > 0 then
  begin
    SetLength(Key, 4);
    P := PChar(Key);
    for I := 1 to 4 do
    begin
      P^ := Chr(B64[Random(58)]);
      Inc(P);
    end;
    SetLength(sText, Bufsize);
    EncryptDes(Buf^, sText[1], Bufsize, Key);
    Result := Key[1] + Key[2] + EncodeString(Base64EncodeStr(sText)) + Key[3] + Key[4];
  end
  else
    Result := '';
end;

procedure DecryBuffer(Src: string; Buf: PChar; Bufsize: Integer);
var
  sText: string;
  Key: string;
begin
  if Length(Src) > 4 then
  begin
    Key := Src[1] + Src[2] + Src[Length(Src) - 1] + Src[Length(Src)];
    Src := Copy(Src, 3, Length(Src) - 4);
    sText := Base64DecodeStr(DecodeString(Src));                                                    // DecodeString    DecryStrHex
    DecryptDes(sText[1], Buf^, Bufsize, Key);
  end;
end;

function EncryStrHex(Str: string): string;
var
  StrResult, TempResult, temp: string;
  I: Integer;
begin
  TempResult := Str;
  StrResult := '';
  for I := 0 to Length(TempResult) - 1 do
  begin
    temp := Format('%x', [Ord(TempResult[I + 1])]);
    if Length(temp) = 1 then temp := '0' + temp;
    StrResult := StrResult + temp;
  end;
  Result := StrResult;
end;

function DecryStrHex(StrHex: string): string;

  function HexToInt(Hex: string): Integer;
  var
    I, Res: Integer;
    Ch: Char;
  begin
    Res := 0;
    for I := 0 to Length(Hex) - 1 do
    begin
      Ch := Hex[I + 1];
      if (Ch >= '0') and (Ch <= '9') then
        Res := Res * 16 + Ord(Ch) - Ord('0')
      else if (Ch >= 'A') and (Ch <= 'F') then
        Res := Res * 16 + Ord(Ch) - Ord('A') + 10
      else if (Ch >= 'a') and (Ch <= 'f') then
        Res := Res * 16 + Ord(Ch) - Ord('a') + 10
      else
        raise Exception.Create('Error: not a Hex String');
    end;
    Result := Res;
  end;

var
  Str, temp: string;
  I: Integer;
begin
  Str := '';
  for I := 0 to Length(StrHex) div 2 - 1 do
  begin
    temp := Copy(StrHex, I * 2 + 1, 2);
    Str := Str + Chr(HexToInt(temp));
  end;
  Result := Str;
end;

end.
