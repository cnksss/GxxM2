unit StringHashMap;

interface

uses SysUtils, Classes;

const
EMPTY_HASH = -1;

type
TMapKey = String;
TMapValue = Pointer;

type
{ THashTable }
TStringHashMapPair = record
    Key: TMapKey;
    Value: TMapValue;
end;
PStringHashMapPair = ^TStringHashMapPair;

PPHashMapItem = ^PStringHashMapItem;
PStringHashMapItem = ^TStringHashMapItem;
TStringHashMapItem = record
    HashCode:Cardinal;
    Next: PStringHashMapItem;
    Pair:TStringHashMapPair;
end;

TStringHashItemArray = array of PStringHashMapItem;

TStringHashMapPairEnumerator = class;

TStringHashMap = class
private
  Buckets: TStringHashItemArray;
  FRecordCount: Integer;
  FGrowThreshold:Integer;
private
  function CalcCapacity(nCapacity:Integer):Integer;
  function GetValue(const Key: TMapKey): TMapValue;
  procedure SetValue(const Key: TMapKey; Value: TMapValue);
  function DefaultEmptyValue:TMapValue; inline;
  function HashLittle(const Data; Len, InitVal: Integer): Integer;
protected
  procedure Rehash(NewCapPow2: Integer);
  procedure Grow();
  function GetBucketCount: Integer;
  function DefaultEmptyKey:TMapKey;inline;
  function Find(const Key: TMapKey): PPHashMapItem; overload;
  function Find(const Key:TMapKey; HashIndex:Integer):PPHashMapItem; overload;
  procedure Delete(Index: Integer);

  function Modify(const Key: TMapKey; Value: TMapValue): Boolean;// 此函数未用上
  
public
  constructor Create(Size: Integer = 256);
  destructor Destroy; override;

  function HashOf(pData:Pointer; nLen:Integer):Cardinal; overload; virtual;
  function HashOf(const Key: string): Cardinal;overload; virtual;
  function HashOf(Key:Integer):Cardinal; overload; virtual;

  procedure Clear;
  procedure Remove(const Key: TMapKey);

  function TryAdd(const Key: TMapKey; Value: TMapValue): Boolean;
  function IsContainKey(const Key:TMapKey):Boolean;
  function GetEnumerator:TStringHashMapPairEnumerator;
  function TryGetValue(const Key:TMapKey; var Value:TMapValue):Boolean;

  procedure AddOrSet(const Key:TMapKey; Value:TMapValue); inline;   

public
  property Values[const Key: TMapKey]: TMapValue read GetValue write SetValue; default;
  property Count: Integer read FRecordCount;

end;

TStringHashMapPairEnumerator = class
private
    m_BucketIndex:Integer;
    m_pCurBucket:PStringHashMapItem;
    m_hash:TStringHashMap;
private
    constructor Create; overload;
    constructor Create(hash:TStringHashMap); overload;
    function GetCurrent:PStringHashMapPair;
public
    destructor Destroy; override;
    function MoveNext:Boolean;
    property Current:PStringHashMapPair read GetCurrent;
end;

implementation

{ THashTable }

//返回H
function TStringHashMap.DefaultEmptyValue:TMapValue;
begin
    Result := nil;
end;

function TStringHashMap.DefaultEmptyKey:TMapKey;
begin
    Result := '';
end;

function TStringHashMap.HashOf(const Key: string): Cardinal;
var
    pStr:pAnsiChar;
    nHash:Integer;
const
    PositiveMask = not Integer($80000000);
begin
    pStr := PAnsiChar(Key);
    nHash := HashLittle(pstr^, Length(Key), 0);
    Result := PositiveMask and ((PositiveMask and nHash) + 1);
end;

function TStringHashMap.HashOf(pData:Pointer; nLen:Integer):Cardinal;
var
   nHash:Integer;
const
    PositiveMask = not Integer($80000000);
begin
    nHash := HashLittle(pData, nLen, 0);
    Result := PositiveMask and ((PositiveMask and nHash) + 1);
end;

//function THashMap.HashOf(Key: Integer): Cardinal;
//var
//    nHash:Integer;
//const
//    PositiveMask = not Integer($80000000);
//begin
//    nHash := HashLittle(Key, SizeOf(Key), 0);
//    Result := PositiveMask and ((PositiveMask and nHash) + 1);
//end;

function TStringHashMap.HashOf(Key:Integer):Cardinal;
const
    A = 0.6180339887; // (sqrt(5) - 1) / 2
begin
    Result := Trunc(Length(Buckets) * (Frac(Cardinal(Key) * A)));
end;

function TStringHashMap.HashLittle(const Data; Len, InitVal: Integer): Integer;
  function Rot(x, k: Cardinal): Cardinal; inline;
  begin
      Result := (x shl k) or (x shr (32 - k));
  end;

  procedure Mix(var a, b, c: Cardinal); inline;
  begin
    Dec(a, c); a := a xor Rot(c, 4); Inc(c, b);
    Dec(b, a); b := b xor Rot(a, 6); Inc(a, c);
    Dec(c, b); c := c xor Rot(b, 8); Inc(b, a);
    Dec(a, c); a := a xor Rot(c,16); Inc(c, b);
    Dec(b, a); b := b xor Rot(a,19); Inc(a, c);
    Dec(c, b); c := c xor Rot(b, 4); Inc(b, a);
  end;

  procedure DoFinal(var a, b, c: Cardinal); inline;
  begin
    c := c xor b; Dec(c, Rot(b,14));
    a := a xor c; Dec(a, Rot(c,11));
    b := b xor a; Dec(b, Rot(a,25));
    c := c xor b; Dec(c, Rot(b,16));
    a := a xor c; Dec(a, Rot(c, 4));
    b := b xor a; Dec(b, Rot(a,14));
    c := c xor b; Dec(c, Rot(b,24));
  end;

  function GetCardinalValue(p:PCardinal; nOffset:Integer):Cardinal; inline;
  var
      pDest:PCardinal;
  begin
      pDest := Pointer(NativeInt(p) + (nOffset * Integer(Sizeof(Cardinal))));
      Result := pDest^;
  end;

  function GetByteValue(p:PByte; noffset:Integer):Byte;inline;
  var
      pDest:PByte;
  begin
      pDest := Pointer(NativeInt(p) + (nOffset* Integer(Sizeof(Byte))));
      Result := pDest^;
  end;

var
  pb: PByte;
  pd: PCardinal absolute pb;
  a, b, c: Cardinal;
label
  case_1, case_2, case_3, case_4, case_5, case_6,
  case_7, case_8, case_9, case_10, case_11, case_12;
begin
  a := Cardinal($DEADBEEF) + Cardinal(Len) + Cardinal(InitVal);
  b := a;
  c := a;

  pb := @Data;

  // 4-byte aligned data
  if (Cardinal(pb) and 3) = 0 then begin
    while Len > 12 do begin
      Inc(a, GetCardinalValue(pd, 0)); //Inc(a, pd[0]);
      Inc(b, GetCardinalValue(pd, 1)); // Inc(b, pd[1]);
      Inc(c, GetCardinalValue(pd, 2)); //Inc(c, pd[2]);
      Mix(a, b, c);
      Dec(Len, 12);
      Inc(pd, 3);
    end;

    case Len of
      0: begin Result := Integer(c); Exit; end;// Exit(Integer(c));
      1: Inc(a, GetCardinalValue(pd, 0) and $FF); //Inc(a, pd[0] and $FF);
      2: Inc(a, GetCardinalValue(pd, 0) and $FFFF);
      3: Inc(a, GetCardinalValue(pd, 0) and $FFFFFF);
      4: Inc(a, GetCardinalValue(pd, 0));
      5: begin
        Inc(a, GetCardinalValue(pd, 0));
        Inc(b, GetCardinalValue(pd, 1) and $FF);
      end;
      6: begin
        Inc(a, GetCardinalValue(pd, 0));
        Inc(b, GetCardinalValue(pd, 1) and $FFFF);
      end;
      7: begin
        Inc(a, GetCardinalValue(pd, 0));
        Inc(b, GetCardinalValue(pd, 1) and $FFFFFF);
      end;
      8: begin
        Inc(a, GetCardinalValue(pd, 0));
        Inc(b, GetCardinalValue(pd, 1));
      end;
      9: begin
        Inc(a, GetCardinalValue(pd, 0));
        Inc(b, GetCardinalValue(pd, 1));
        Inc(c, GetCardinalValue(pd, 2) and $FF);
      end;
      10: begin
        Inc(a, GetCardinalValue(pd, 0));
        Inc(b, GetCardinalValue(pd, 1));
        Inc(c, GetCardinalValue(pd, 2) and $FFFF);
      end;
      11: begin
        Inc(a, GetCardinalValue(pd, 0));
        Inc(b, GetCardinalValue(pd, 1));
        Inc(c, GetCardinalValue(pd, 2) and $FFFFFF);
      end;
      12: begin
        Inc(a, GetCardinalValue(pd, 0));
        Inc(b, GetCardinalValue(pd, 1));
        Inc(c, GetCardinalValue(pd, 2));
      end;
    end;
  end else begin
    // Ignoring rare case of 2-byte aligned data. This handles all other cases.
    while Len > 12 do begin
      Inc(a, GetByteValue(pb, 0) + GetByteValue(pb, 1) shl 8 + GetByteValue(pb, 2) shl 16 + GetByteValue(pb, 3) shl 24);
      Inc(b, GetByteValue(pb, 4) + GetByteValue(pb, 5) shl 8 + GetByteValue(pb, 6) shl 16 + GetByteValue(pb, 7) shl 24);
      Inc(c, GetByteValue(pb, 8) + GetByteValue(pb, 9) shl 8 + GetByteValue(pb, 10) shl 16 + GetByteValue(pb, 11) shl 24);
      Mix(a, b, c);
      Dec(Len, 12);
      Inc(pb, 12);
    end;

    case Len of
      0: begin Result := Integer(c); exit; end; //Exit(Integer(c));
      1: goto case_1;
      2: goto case_2;
      3: goto case_3;
      4: goto case_4;
      5: goto case_5;
      6: goto case_6;
      7: goto case_7;
      8: goto case_8;
      9: goto case_9;
      10: goto case_10;
      11: goto case_11;
      12: goto case_12;
    end;

case_12:
    Inc(c, GetByteValue(pb, 11) shl 24);
case_11:
    Inc(c, GetByteValue(pb,10) shl 16);
case_10:
    Inc(c, GetByteValue(pb, 9) shl 8);
case_9:
    Inc(c, GetByteValue(pb, 8));
case_8:
    Inc(b, GetByteValue(pb, 7) shl 24);
case_7:
    Inc(b, GetByteValue(pb, 6) shl 16);
case_6:
    Inc(b, GetByteValue(pb, 5) shl 8);
case_5:
    Inc(b, GetByteValue(pb, 4));
case_4:
    Inc(a, GetByteValue(pb, 3) shl 24);
case_3:
    Inc(a, GetByteValue(pb, 2) shl 16);
case_2:
    Inc(a, GetByteValue(pb, 1) shl 8);
case_1:
    Inc(a, GetByteValue(pb, 0));
  end;

  DoFinal(a, b, c);
  Result := Integer(c);
end;

function TStringHashMap.GetBucketCount: Integer;
begin
  Result := Length(Buckets);
end;

function TStringHashMap.GetValue(const Key: TMapKey): TMapValue;
var
  P: PStringHashMapItem;
begin
  P := Find(Key)^;
  if P <> nil then begin
     Result := P^.Pair.Value
  end else begin
     Result := DefaultEmptyValue;
  end;
end;

procedure TStringHashMap.Grow;
var
    newCap: Integer;
begin
    newCap := Length(Buckets) * 2;
    if newCap = 0 then  newCap := 4;
    Rehash(newCap);
end;

procedure TStringHashMap.SetValue(const Key: TMapKey; Value: TMapValue);
var
    HashIndex: Integer;
    Bucket: PStringHashMapItem;
    prev:PPHashMapItem;
begin
    HashIndex := HashOf(Key) mod Cardinal(Length(Buckets));

    prev := Find(Key, HashIndex);
    if Prev^ <> nil then begin
        Prev^.Pair.Value := Value;
    end else begin
        New(Bucket);
        Bucket^.pair.Key := Key;
        Bucket^.Next := Buckets[HashIndex];
        Bucket^.Pair.Value := Value;
        Buckets[HashIndex] := Bucket;
        Inc(FRecordCount);

        if FRecordCount >= FGrowThreshold then begin
            Grow;
        end;
    end;
end;

function TStringHashMap.CalcCapacity(nCapacity: Integer): Integer;
var
    newCap:Integer;
begin
    if nCapacity = 0 then begin
        Result := 4;
    end else begin
        newCap := 4;
        while ((newCap shr 1) + (newCap shr 2)) <= nCapacity do begin
           newCap := newCap shl 1;
        end;
        Result := newCap;
    end;
end;

procedure TStringHashMap.Clear;
var
  I: Integer;
  P, N: PStringHashMapItem;
begin
  FRecordCount := 0;
  for I := 0 to Length(Buckets) - 1 do begin
    P := Buckets[I];
    while P <> nil do begin
      N := P^.Next;
      Dispose(P);
      P := N;
    end;
    Buckets[I] := nil;
  end;
end;

constructor TStringHashMap.Create(Size: Integer);
var
   NewCapPow2:Integer;
begin
    inherited Create;
    NewCapPow2 := CalcCapacity(Size);
    FGrowThreshold := (NewCapPow2 shr 1) + (NewCapPow2 shr 2);
    SetLength(Buckets, NewCapPow2);
    FRecordCount := 0;
end;

function TStringHashMap.GetEnumerator: TStringHashMapPairEnumerator;
begin
    Result := TStringHashMapPairEnumerator.Create(self);
end;

destructor TStringHashMap.Destroy;
begin
  Clear;
  inherited;
end;

function TStringHashMap.Find(const Key: TMapKey): PPHashMapItem;
var
  HashIndex: Integer;
begin
  HashIndex := HashOf(Key) mod Cardinal(Length(Buckets));
  Result := @Buckets[HashIndex];
  while Result^ <> nil do begin
    if Result^.Pair.Key = Key then begin
        Exit;
    end else begin
        Result := @Result^.Next;
    end;
  end;
end;

function TStringHashMap.Find(const Key:TMapKey; HashIndex:Integer):PPHashMapItem;
begin
  Result := @Buckets[HashIndex];
  while Result^ <> nil do begin
    if Result^.Pair.Key = Key then begin
        Exit;
    end else begin
        Result := @Result^.Next;
    end;
  end;
end;

function TStringHashMap.TryGetValue(const Key: TMapKey; var Value: TMapValue): Boolean;
var
    ppItem:PPHashMapItem;
begin
    ppItem := Find(Key);
    if ppItem <> nil then begin
        Result := True;
        Value := ppItem^.Pair.Value;
    end else begin
        Result := False;
        Value := DefaultEmptyValue;
    end;
end; 

function TStringHashMap.IsContainKey(const Key:TMapKey):Boolean;
begin
    Result := Find(Key)^ <> nil;
end;

function TStringHashMap.TryAdd(const Key: TMapKey; Value: TMapValue): Boolean;
var
    Hash,HashIndex: Cardinal;
    pItem: PStringHashMapItem;
begin
    Hash := HashOf(Key);
    HashIndex := Hash mod Cardinal(Length(Buckets));
    if Find(Key, HashIndex)^ = nil then begin
        pItem := New(PStringHashMapItem);
        pItem^.HashCode := Hash;
        pItem^.Pair.Key := Key;
        pItem^.Next := Buckets[HashIndex];
        pItem^.Pair.Value := Value;
        Buckets[HashIndex] := pItem;
        Inc(FRecordCount);

        if FRecordCount >= FGrowThreshold then begin
            Grow;
        end;

        Result := True;
    end else begin
        Result := False;
    end;
end;

procedure TStringHashMap.AddOrSet(const Key: TMapKey; Value: TMapValue);
begin
    SetValue(Key, Value);
end;

function TStringHashMap.Modify(const Key: TMapKey; Value: TMapValue): Boolean;
var
  P: PStringHashMapItem;
begin
  P := Find(Key)^;
  if P <> nil then begin
    Result := True;
    P^.Pair.Value := Value;
  end else begin
    Result := False;
  end;
end;

procedure TStringHashMap.Delete(Index: Integer);
var
    P: PStringHashMapItem;
    Prev: PPHashMapItem;
begin
    Prev := @Buckets[Index];
    P := Prev^;
    if P <> nil then begin
        Prev^ := P^.Next;
        Dispose(P);
        Dec(FRecordCount);
    end;
end;

procedure TStringHashMap.Remove(const Key: TMapKey);
var
    P: PStringHashMapItem;
    Prev: PPHashMapItem;
begin
    Prev := Find(Key);
    //if Prev <> nil then begin Prev始终落在桶内，不可能为空
        P := Prev^;
        if P <> nil then begin
            Prev^ := P^.Next;
            Dispose(P);
            Dec(FRecordCount);
        end;
    //end;
end;

procedure TStringHashMap.Rehash(NewCapPow2: Integer);
var
    oldItems, newItems: TStringHashItemArray;
    i, nNewIndex: Integer;
    Bucket, Next:PStringHashMapItem;
begin
    if NewCapPow2 < 0 then exit; //OutOfMemoryError;
    if NewCapPow2 = Length(Buckets) then exit;
    oldItems := Buckets;

    SetLength(newItems, NewCapPow2);
    Buckets := newItems;

    for i := 0 to Length(oldItems) - 1 do begin
        Bucket := oldItems[i];
        while Bucket <> nil do begin
            nNewIndex := Bucket.HashCode mod Cardinal(Length(NewItems));
            Next := Bucket.Next;
            Bucket^.Next := newItems[nNewIndex];
            Buckets[nNewIndex] := Bucket;
            Bucket := Next;
        end;
    end;
    SetLength(oldItems, 0);

    FGrowThreshold := (NewCapPow2 shr 1) + (NewCapPow2 shr 2); // 75%
end;

{ TMapPairEnumerator }

constructor TStringHashMapPairEnumerator.Create(hash: TStringHashMap);
begin
    m_hash := hash;
    m_pCurBucket := nil;
    m_BucketIndex := -1;
end;

constructor TStringHashMapPairEnumerator.Create;
begin
    m_hash := nil;
    m_pCurBucket := nil;
    m_BucketIndex := -1;
end;

destructor TStringHashMapPairEnumerator.Destroy;
begin
    inherited;
end;

function TStringHashMapPairEnumerator.GetCurrent: PStringHashMapPair;
begin
    if m_pCurBucket <> nil then begin
        Result := @m_pCurBucket.Pair;
    end else begin
        Result := nil;
    end;
end;


function TStringHashMapPairEnumerator.MoveNext: Boolean;
var
    i, nCount:Integer;
begin
    Result := False;
    if m_hash = nil then exit;

    if m_BucketIndex < 0 then begin //first
        nCount := m_hash.GetBucketCount;
        for i := 0 to nCount - 1 do begin
            m_pCurBucket := m_hash.Buckets[i];
            if m_pCurBucket <> nil then begin
                m_BucketIndex := i;
                Result := True;
                break;
            end;
        end;
    end else begin
       if m_pCurBucket.Next = nil then begin
          m_pCurBucket := nil;
          nCount := m_hash.GetBucketCount;
          for i := m_BucketIndex + 1 to nCount - 1 do begin
              m_pCurBucket := m_hash.Buckets[i];
              if m_pCurBucket <> nil then begin
                   m_BucketIndex := i;
                   Result := True;
                   break;
              end;
          end;
       end else begin
          m_pCurBucket := m_pCurBucket.Next;
          Result := True;
       end;
    end;
end;

end.

