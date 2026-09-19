unit HashTable;

interface

uses SysUtils, Classes;

type
      { THashTable }
  PPHashItem = ^PHashItem;
  PHashItem = ^THashItem;
  THashItem = record
    Next: PHashItem;
    Key: string;
    Value: string;
    Data: Pointer;
    Int: Integer;
  end;

  THashTable = class
  private
    FSize: Integer;
    Buckets: array of Pointer;
    FRecordCount: Integer;
    function GetCount: Integer;
    function GetValue(const Name: string): string;
    procedure SetValue(const Name: string; Value: string);

    function GetInteger(const Name: string): Integer;
    procedure SetInteger(const Name: string; Value: Integer);


    function GetData(const Name: string): Pointer;
    procedure SetData(const Name: string; Value: Pointer);

    function Get(Index: Integer): Pointer;

    procedure Put(Index: Integer; Value: Pointer);

    function GetString(Index: Integer): string;
    procedure SetString(Index: Integer; Value: string);
  protected
    function Find(const Name: string): PPHashItem;

  public
    constructor Create(Size: Integer = 256);
    destructor Destroy; override;
    function HashOf(const Name: string): Cardinal; virtual;
    function IndexOf(const Name: string): Integer;
    procedure Clear;
    procedure Remove(const Name: string);
    procedure Delete(Index: Integer);
    function Modify(const Name: string; Value: string): Boolean;
    function Add(const Name: string; const Value: string; P: Pointer; Int: Integer = -1): Boolean;
    property Values[const Name: string]: string read GetValue write SetValue;
    property Integers[const Name: string]: Integer read GetInteger write SetInteger;
    property Datas[const Name: string]: Pointer read GetData write SetData;
    property Count: Integer read GetCount;
    property RecordCount: Integer read FRecordCount;
    property Items[Index: Integer]: Pointer read Get write Put;
    property Strings[Index: Integer]: string read GetString write SetString;
  end;
function HashIndex(Value: Integer): Integer;
implementation
const
  TABLE_SIZE = 256;

const
  CRC16Table: array[0..TABLE_SIZE - 1] of Word = (
    $0000, $1021, $2042, $3063, $4084, $50A5, $60C6, $70E7,
    $8108, $9129, $A14A, $B16B, $C18C, $D1AD, $E1CE, $F1EF,
    $1231, $0210, $3273, $2252, $52B5, $4294, $72F7, $62D6,
    $9339, $8318, $B37B, $A35A, $D3BD, $C39C, $F3FF, $E3DE,
    $2462, $3443, $0420, $1401, $64E6, $74C7, $44A4, $5485,
    $A56A, $B54B, $8528, $9509, $E5EE, $F5CF, $C5AC, $D58D,
    $3653, $2672, $1611, $0630, $76D7, $66F6, $5695, $46B4,
    $B75B, $A77A, $9719, $8738, $F7DF, $E7FE, $D79D, $C7BC,
    $48C4, $58E5, $6886, $78A7, $0840, $1861, $2802, $3823,
    $C9CC, $D9ED, $E98E, $F9AF, $8948, $9969, $A90A, $B92B,
    $5AF5, $4AD4, $7AB7, $6A96, $1A71, $0A50, $3A33, $2A12,
    $DBFD, $CBDC, $FBBF, $EB9E, $9B79, $8B58, $BB3B, $AB1A,
    $6CA6, $7C87, $4CE4, $5CC5, $2C22, $3C03, $0C60, $1C41,
    $EDAE, $FD8F, $CDEC, $DDCD, $AD2A, $BD0B, $8D68, $9D49,
    $7E97, $6EB6, $5ED5, $4EF4, $3E13, $2E32, $1E51, $0E70,
    $FF9F, $EFBE, $DFDD, $CFFC, $BF1B, $AF3A, $9F59, $8F78,
    $9188, $81A9, $B1CA, $A1EB, $D10C, $C12D, $F14E, $E16F,
    $1080, $00A1, $30C2, $20E3, $5004, $4025, $7046, $6067,
    $83B9, $9398, $A3FB, $B3DA, $C33D, $D31C, $E37F, $F35E,
    $02B1, $1290, $22F3, $32D2, $4235, $5214, $6277, $7256,
    $B5EA, $A5CB, $95A8, $8589, $F56E, $E54F, $D52C, $C50D,
    $34E2, $24C3, $14A0, $0481, $7466, $6447, $5424, $4405,
    $A7DB, $B7FA, $8799, $97B8, $E75F, $F77E, $C71D, $D73C,
    $26D3, $36F2, $0691, $16B0, $6657, $7676, $4615, $5634,
    $D94C, $C96D, $F90E, $E92F, $99C8, $89E9, $B98A, $A9AB,
    $5844, $4865, $7806, $6827, $18C0, $08E1, $3882, $28A3,
    $CB7D, $DB5C, $EB3F, $FB1E, $8BF9, $9BD8, $ABBB, $BB9A,
    $4A75, $5A54, $6A37, $7A16, $0AF1, $1AD0, $2AB3, $3A92,
    $FD2E, $ED0F, $DD6C, $CD4D, $BDAA, $AD8B, $9DE8, $8DC9,
    $7C26, $6C07, $5C64, $4C45, $3CA2, $2C83, $1CE0, $0CC1,
    $EF1F, $FF3E, $CF5D, $DF7C, $AF9B, $BFBA, $8FD9, $9FF8,
    $6E17, $7E36, $4E55, $5E74, $2E93, $3EB2, $0ED1, $1EF0);

  Crc16Start: Cardinal = $FFFF;
  Crc16Bytes = 2;
  Crc16Bits = 16;

function CRC16(s: PByteArray; iCount: Integer; OldCRC: Word = 0): Word;
var
  I, Step, DecCount: Integer;
begin
  Result := Crc16Start;
  if iCount < 32 then
  begin
    for I := 0 to iCount - 1 do
    begin
      Result := CRC16Table[Result shr (CRC16Bits - 8)] xor Word((Result shl 8)) xor s[I];
    end;
  end
  else
  begin
    Step := iCount div 32 + 1;
    I := 0;
    DecCount := iCount - 1;
    while I < DecCount do
    begin
      Result := CRC16Table[Result shr (CRC16Bits - 8)] xor Word((Result shl 8)) xor s[I];
      Inc(I, Step);
    end;
  end;
  for i := 0 to Crc16Bytes - 1 do
  begin
    Result := CRC16Table[Result shr (CRC16Bits - 8)] xor Word((Result shl 8)) xor (OldCRC shr (CRC16Bits - 8));
    OldCRC := Word(OldCRC shl 8);
  end;
end;

function HashIndex(Value: Integer): Integer;
begin
  Result := CRC16(Pointer(IntToStr(Value)), Length(IntToStr(Value)), 0) mod 2000;
end;
    { THashTable }

function THashTable.GetCount: Integer;
begin
  Result := Length(Buckets);
end;

function THashTable.GetString(Index: Integer): string;
begin
  if Buckets[Index] <> nil then
    Result := PHashItem(Buckets[Index]).Value
  else
    Result := '';
end;

procedure THashTable.SetString(Index: Integer; Value: string);
begin
  if Buckets[Index] <> nil then
    PHashItem(Buckets[Index]).Value := Value;
end;

function THashTable.GetInteger(const Name: string): Integer;
var
  P: PHashItem;
begin
  P := Find(Name)^;
  if P <> nil then
    Result := P^.Int
  else
    Result := -1;
end;

procedure THashTable.SetInteger(const Name: string; Value: Integer);
var
  P: PHashItem;
begin
  P := Find(Name)^;
  if P <> nil then
    P^.Int := Value;
end;

function THashTable.Get(Index: Integer): Pointer;
begin
  if Buckets[Index] <> nil then
    Result := PHashItem(Buckets[Index]).Data
  else
    Result := nil;
end;

procedure THashTable.Put(Index: Integer; Value: Pointer);
begin
  if Buckets[Index] <> nil then
    PHashItem(Buckets[Index]).Data := Value;
end;

function THashTable.GetValue(const Name: string): string;
var
  P: PHashItem;
begin
  P := Find(Name)^;
  if P <> nil then
    Result := P^.Value
  else
    Result := '';
end;

function THashTable.IndexOf(const Name: string): Integer;
begin
  Result := HashOf(Name) mod Cardinal(Length(Buckets));
end;

procedure THashTable.SetValue(const Name: string; Value: string);
var
  Hash: Integer;
  Bucket: PHashItem;
begin
  Hash := HashOf(Name) mod Cardinal(Length(Buckets));
  New(Bucket);
  Bucket^.Key := Name;
  Bucket^.Value := Value;
  Bucket^.Next := Buckets[Hash];
  Bucket^.Data := nil;
  Bucket^.Int := -1;
  Buckets[Hash] := Bucket;
  Inc(FRecordCount);
end;

function THashTable.GetData(const Name: string): Pointer;
var
  P: PHashItem;
begin
  P := Find(Name)^;
  if P <> nil then
    Result := P^.Data
  else
    Result := nil;
end;

procedure THashTable.SetData(const Name: string; Value: Pointer);
var
  Hash: Integer;
  Bucket: PHashItem;
begin
  Hash := HashOf(Name) mod Cardinal(Length(Buckets));
  New(Bucket);
  Bucket^.Key := Name;
  Bucket^.Value := '';
  Bucket^.Next := Buckets[Hash];
  Bucket^.Data := Value;
  Bucket^.Int := -1;
  Buckets[Hash] := Bucket;
  Inc(FRecordCount);
end;

procedure THashTable.Clear;
var
  I: Integer;
  P, N: PHashItem;
begin
  FRecordCount := 0;
  for I := 0 to Length(Buckets) - 1 do
  begin
    P := Buckets[I];
    while P <> nil do
    begin
      N := P^.Next;
      Dispose(P);
      P := N;
    end;
    Buckets[I] := nil;
  end;
end;

constructor THashTable.Create(Size: Integer);
begin
  inherited Create;
  FSize := Size;
  SetLength(Buckets, Size);
  FRecordCount := 0;
end;

destructor THashTable.Destroy;
begin
  Clear;

  inherited;
end;

function THashTable.Find(const Name: string): PPHashItem;
var
  Hash: Integer;
begin
  Hash := HashOf(Name) mod Cardinal(Length(Buckets));
  Result := @Buckets[Hash];
  while Result^ <> nil do begin
    if Result^.Key = Name then
      Exit
    else
      Result := @Result^.Next;
  end;
end;

function THashTable.HashOf(const Name: string): Cardinal;
// var
// I: Integer;
begin
  Result := CRC16(Pointer(Name), Length(Name), 0);
  {Result := 0;
  for I := 1 to Length(Name) do
    Result := ((Result shl 2) or (Result shr (SizeOf(Result) * 8 - 2))) xor
      Ord(Name[I])};
end;

function THashTable.Add(const Name: string; const Value: string; P: Pointer; Int: Integer): Boolean;
var
  Hash: Integer;
  Bucket: PHashItem;
begin
  Hash := HashOf(Name) mod Cardinal(Length(Buckets));
  New(Bucket);
  Bucket^.Key := Name;
  Bucket^.Value := Value;
  Bucket^.Next := Buckets[Hash];
  Bucket^.Data := P;
  Bucket^.Int := Int;
  Buckets[Hash] := Bucket;
  Inc(FRecordCount);
end;

function THashTable.Modify(const Name: string; Value: string): Boolean;
var
  P: PHashItem;
begin
  P := Find(Name)^;
  if P <> nil then
  begin
    Result := True;
    P^.Value := Value;
  end
  else
    Result := False;
end;

procedure THashTable.Delete(Index: Integer);
var
  P: PHashItem;
  Prev: PPHashItem;
begin
  Prev := @Buckets[Index];
  P := Prev^;
  if P <> nil then
  begin
    Prev^ := P^.Next;
    Dispose(P);
    Dec(FRecordCount);
  end;
end;

procedure THashTable.Remove(const Name: string);
{var
  Hash: Integer;
  Prev: PPHashItem;
begin
  Hash := HashOf(Name) mod Cardinal(Length(Buckets));
  Prev := @Buckets[Hash];

  if Prev <> nil then begin
    if Prev^.Key = Name then begin
      P := Prev^;
      if P <> nil then
      begin
        Prev^ := P^.Next;
        Dispose(P);
      end;
      Buckets[Hash] := nil;
    end;
  end;
end;}

var
  P: PHashItem;
  Prev: PPHashItem;
begin
  Prev := Find(Name);
  if Prev <> nil then begin
    P := Prev^;
    if P <> nil then
    begin
      Prev^ := P^.Next;
      Dispose(P);
      Dec(FRecordCount);
    end;
  end;
end;

end.

