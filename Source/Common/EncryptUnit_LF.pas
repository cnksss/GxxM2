unit EncryptUnit_LF;

interface
uses
  Windows, SysUtils{, DesUtils};

function DecryScript_LF(Str: AnsiString): AnsiString;
function EncryScript_LF(Str: AnsiString): AnsiString;

function DecryString_LF(Str: AnsiString): AnsiString;
function EncryString_LF(Str: AnsiString): AnsiString;

//function DecryString_LF2(Str: AnsiString): AnsiString;
//function EncryString_LF2(Str: AnsiString): AnsiString;

function EncryStringHex_LF(Src: AnsiString): AnsiString;
function DecryStringHex_LF(Src: AnsiString): AnsiString;

function EncryBuffer_LF(Buf: PAnsiChar; Bufsize: Integer): AnsiString;
procedure DecryBuffer_LF(Src: AnsiString; Buf: PAnsiChar; Bufsize: Integer);

function EncryBufferA_LF(Buf: PAnsiChar; Bufsize: Integer): AnsiString; overload;
procedure EncryBufferA_LF(Buf: PAnsiChar; Bufsize: Integer; OutBuf: PAnsiChar); overload;
procedure DecryBufferA_LF(Src: AnsiString; Buf: PAnsiChar; Bufsize: Integer); overload;
procedure DecryBufferA_LF(Source: PAnsiChar; SourceSize: Integer; Outdata: PAnsiChar); overload;

function DecryBufferA_LF(Buf: PAnsiChar; Bufsize: Integer): AnsiString; overload;
function DecryBufferA_LF(Src: AnsiString): AnsiString; overload;

implementation
uses EDcode, UnitDes, Base64;
// ==============================================================================

function DecryScript_LF(Str: AnsiString): AnsiString;
begin
  Result := DecryptStrDes(Str, IntToStr(NewEncryKey^));
end;

function EncryScript_LF(Str: AnsiString): AnsiString;
begin
  Result := EncryptStrDes(Str, IntToStr(NewEncryKey^));
end;

function DecryString_LF(Str: AnsiString): AnsiString;
begin
  if Str <> '' then
  begin
    Result := DecryptStrDes(Base64DecodeStr(DecodeString(Str)), IntToStr(NewEncryKey^));
  end
  else
    Result := '';
end;

function EncryString_LF(Str: AnsiString): AnsiString;
begin
  if Str <> '' then
    Result := EncryBuffer_LF(@Str[1], Length(Str))
  else
    Result := '';
end;

function DecryStringHex_LF(Src: AnsiString): AnsiString;

  function HexToInt(Hex: AnsiString): Integer;
  var
    I, Res: Integer;
    Ch: AnsiChar;
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
     // else raise Exception.Create('Error: not a Hex AnsiString');
    end;
    Result := Res;
  end;
var
  Str, temp: AnsiString;
  I: Integer;
begin
  if Src <> '' then
  begin
    Str := '';
    for I := 0 to Length(Src) div 2 - 1 do
    begin
      temp := Copy(Src, I * 2 + 1, 2);
      Str := Str + Chr(HexToInt(temp));
    end;

    Result := DecryptStrDes(Base64DecodeStr(Str), IntToStr(NewEncryKey^));
  end
  else
    Result := '';
end;

(*
function DecryString_LF2(Str: AnsiString): AnsiString;
begin
  if Str <> '' then
  begin
    Result := Base64DecodeStr(DecodeString(Str));
    DecryptDes_New(Result[1], Result[1], Length(Result), IntToStr(NewEncryKey^));
  end
  else
    Result := '';
end;

function EncryString_LF2(Str: AnsiString): AnsiString;
begin
  if Str <> '' then
  begin
    EncryptDes_New(Str[1], Str[1], Length(Str), IntToStr(NewEncryKey^));
    Result := EncodeString(Base64EncodeStr(Str));
  end
  else
    Result := '';
end;
*)

function EncryStringHex_LF(Src: AnsiString): AnsiString;
var
  sText, temp, TempResult, StrResult: AnsiString;
  I, Bufsize: Integer;
begin
  Bufsize := Length(Src);
  if Bufsize > 0 then
  begin
    SetLength(sText, Bufsize);
    EncryptDes(Src[1], sText[1], Bufsize, IntToStr(NewEncryKey^));
    TempResult := Base64EncodeStr(sText);
    StrResult := '';
    for I := 0 to Length(TempResult) - 1 do
    begin
      temp := Format('%x', [Ord(TempResult[I + 1])]);
      if Length(temp) = 1 then temp := '0' + temp;
      StrResult := StrResult + temp;
    end;

    Result := StrResult;
  end
  else
    Result := '';
end;

function EncryBuffer_LF(Buf: PAnsiChar; Bufsize: Integer): AnsiString;
var
  sText: AnsiString;
begin
  if Bufsize > 0 then
  begin
    SetLength(sText, Bufsize);
    EncryptDes(Buf^, sText[1], Bufsize, IntToStr(NewEncryKey^));
    Result := EncodeString(Base64EncodeStr(sText));
  end
  else
    Result := '';
end;

procedure DecryBuffer_LF(Src: AnsiString; Buf: PAnsiChar; Bufsize: Integer);
var
  sText: AnsiString;
begin
  if Src <> '' then
  begin
    sText := Base64DecodeStr(DecodeString(Src));                                                    // DecodeString    DecryStrHex
    DecryptDes(sText[1], Buf^, Bufsize, IntToStr(NewEncryKey^));
  end;
end;
// ==============================================================================

function EncryBufferA_LF(Buf: PAnsiChar; Bufsize: Integer): AnsiString;
begin
  SetLength(Result, Bufsize);
  EncryptDes(Buf^, Result[1], Bufsize, IntToStr(NewEncryKey^));
end;

procedure EncryBufferA_LF(Buf: PAnsiChar; Bufsize: Integer; OutBuf: PAnsiChar);
begin
  EncryptDes(Buf^, OutBuf^, Bufsize, IntToStr(NewEncryKey^));
end;

procedure DecryBufferA_LF(Src: AnsiString; Buf: PAnsiChar; Bufsize: Integer);
begin
  DecryptDes(Src[1], Buf^, Bufsize, IntToStr(NewEncryKey^));
end;

function DecryBufferA_LF(Buf: PAnsiChar; Bufsize: Integer): AnsiString;
begin
  SetLength(Result, Bufsize);
  DecryptDes(Buf^, Result[1], Bufsize, IntToStr(NewEncryKey^));
end;

function DecryBufferA_LF(Src: AnsiString): AnsiString;
begin
  Result := DecryptStrDes(Src, IntToStr(NewEncryKey^));
end;

procedure DecryBufferA_LF(Source: PAnsiChar; SourceSize: Integer; Outdata: PAnsiChar);
begin
  DecryptDes(Source^, Outdata^, SourceSize, IntToStr(NewEncryKey^));
end;



end.
