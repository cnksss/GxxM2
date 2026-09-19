unit EncodingHelper;

interface

uses
  System.SysUtils, System.Classes;

type
  TUTF8NoBomEncoding = class(TUTF8Encoding)
  public
    function GetPreamble: TBytes; override;
  end;

  TEncodingHelper = class helper for TEncoding
  strict private
    class var
      FNoBomEncoding: TUTF8NoBomEncoding;
    class function GetNoBomUTF8: TEncoding; static;
  public
    class function GetBufferEncoding(const Buffer: TBytes; var AEncoding: TEncoding): Integer; overload; static;
    class function GetBufferEncoding(const Buffer: TBytes; var AEncoding: TEncoding;
      ADefaultEncoding: TEncoding): Integer; overload; static;

    class property NoBomUTF8: TEncoding read GetNoBomUTF8;
  end;

implementation

{ TUTF8NoBomEncoding }

function TUTF8NoBomEncoding.GetPreamble: TBytes;
begin
  SetLength(Result, 0);
end;

{ TEncodingHelper }

{
  utf-8是一种多字节编码的字符集，表示一个Unicode字符时，它可以是1个至多个字节。
  即在文本全部是ASCII字符时utf-8是和ASCII一致的(utf-8向下兼容ASCII)。utf-8字节流如下所示：
  1字节 0xxxxxxx
  2字节 110xxxxx 10xxxxxx
  3字节 1110xxxx 10xxxxxx 10xxxxxx
  4字节 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx
  5字节 111110xx 10xxxxxx 10xxxxxx 10xxxxxx 10xxxxxx
  6字节 1111110x 10xxxxxx 10xxxxxx 10xxxxxx 10xxxxxx 10xxxxxx
}
function IsBufferUTF8(Buffer: TBytes): Boolean;
var
  C: Byte;
  P, EndPtr: PByte;
begin
  Result := False;
  if Length(Buffer) = 0 then Exit;

  P := PByte(Buffer);
  EndPtr := P + Length(Buffer);

  // skip leading US-ASCII part.
  while P < EndPtr do
  begin
    if P^ <= $7F then
      Inc(P)
    else
      Break;
  end;

  // If all character is US-ASCII, done.
  if P = EndPtr then Exit;

  while P < EndPtr do
  begin
    C := p^;
    case C of
      $00..$7F:                   // 1字节 0xxxxxxx
        Inc(P);

      $C0..$DF:                   // 2字节 110xxxxx 10xxxxxx
        if (P+1 < EndPtr)
            and ((P+1)^ in [$80..$BF]) then
          Inc(P, 2)
        else
          Break;

      $E0..$EF:                   // 3字节 1110xxxx 10xxxxxx 10xxxxxx
        if (P+2 < EndPtr)
            and ((P+1)^ in [$80..$BF])
            and ((P+2)^ in [$80..$BF]) then
          Inc(P, 3)
        else
          Break;

      $F0..$F7:                    // 4字节 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx
        if (P+3 < EndPtr)
            and ((P+1)^ in [$80..$BF])
            and ((P+2)^ in [$80..$BF])
            and ((P+3)^ in [$80..$BF]) then
          Inc(P, 4)
        else
          Break;

      $F8..$FB:                    // 5字节 111110xx 10xxxxxx 10xxxxxx 10xxxxxx 10xxxxxx
        if (P+4 < EndPtr)
            and ((P+1)^ in [$80..$BF])
            and ((P+2)^ in [$80..$BF])
            and ((P+3)^ in [$80..$BF])
            and ((P+4)^ in [$80..$BF]) then
          Inc(P, 5)
        else
          Break;

      $FC..$FD:                    // 6字节 1111110x 10xxxxxx 10xxxxxx 10xxxxxx 10xxxxxx 10xxxxxx
        if (P+5 < EndPtr)
            and ((P+1)^ in [$80..$BF])
            and ((P+2)^ in [$80..$BF])
            and ((P+3)^ in [$80..$BF])
            and ((P+4)^ in [$80..$BF])
            and ((P+5)^ in [$80..$BF]) then
          Inc(P, 6)
        else
          Break;
    else
      Break;
    end;
  end;

  if P = EndPtr then
    Result := True
  else
    Result := False;
end;

class function TEncodingHelper.GetBufferEncoding(const Buffer: TBytes; var AEncoding: TEncoding): Integer;
begin
  Result := GetBufferEncoding(Buffer, AEncoding, Default); // Must call property getter to create Encoding
end;


class function TEncodingHelper.GetBufferEncoding(const Buffer: TBytes;
  var AEncoding: TEncoding; ADefaultEncoding: TEncoding): Integer;

  function ContainsPreamble(const Buffer, Signature: array of Byte): Boolean;
  var
    I: Integer;
  begin
    Result := True;
    if Length(Buffer) >= Length(Signature) then
    begin
      for I := 1 to Length(Signature) do
        if Buffer[I - 1] <> Signature [I - 1] then
        begin
          Result := False;
          Break;
        end;
    end
    else
      Result := False;
  end;

var
  Preamble: TBytes;
begin
  Result := 0;
  if AEncoding = nil then
  begin
    // Find the appropraite encoding
    if ContainsPreamble(Buffer, TEncoding.UTF8.GetPreamble) then
      AEncoding := TEncoding.UTF8
    else if ContainsPreamble(Buffer, TEncoding.Unicode.GetPreamble) then
      AEncoding := TEncoding.Unicode
    else if ContainsPreamble(Buffer, TEncoding.BigEndianUnicode.GetPreamble) then
      AEncoding := TEncoding.BigEndianUnicode
    else if IsBufferUTF8(Buffer) then
      AEncoding := TEncoding.NoBomUTF8
    else
    begin
      AEncoding := ADefaultEncoding;
      Exit; // Don't proceed just in case ADefaultEncoding has a Preamble
    end;
    Result := Length(AEncoding.GetPreamble);
  end
  else
  begin
    Preamble := AEncoding.GetPreamble;
    if ContainsPreamble(Buffer, Preamble) then
      Result := Length(Preamble);
  end;
end;

class function TEncodingHelper.GetNoBomUTF8: TEncoding;
var
  LEncoding: TEncoding;
begin
  if FNoBomEncoding = nil then
  begin
    LEncoding := TUTF8NoBomEncoding.Create;
    if AtomicCmpExchange(Pointer(FNoBomEncoding), Pointer(LEncoding), nil) <> nil then
      LEncoding.Free;
{$IFDEF AUTOREFCOUNT}
    FNoBomEncoding.__ObjAddRef;
{$ENDIF AUTOREFCOUNT}
  end;
  Result := FNoBomEncoding;
end;

end.
