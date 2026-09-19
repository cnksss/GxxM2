unit StruckDamageAbsorbUtils;

interface

uses
  Windows, Classes, SysUtils;

type
  PMonterStruckDamageAbsorb = ^TMonterStruckDamageAbsorb;

  TMonterStruckDamageAbsorb = record
    MonsterName: string;
    AbsorbDamageRate: Byte;
    AbsorbDamageValue: Byte;
    StartTime: LongWord;
    EffectiveTime: LongWord;
  end;

  TStruckDamageAbsorbMgr = class(TObject)
  private
    FList: TList;
    function GetCount: Integer;
    function GetItems(Index: Integer): PMonterStruckDamageAbsorb;

    function Search(MonsterName: string; var Index: Integer): Boolean;
  public
    constructor Create;
    destructor Destroy; override;

    function Add(MonsterName: string; AbsorbDamageRate, AbsorbDamageValue: Byte; EffectiveTime: LongWord)
      : PMonterStruckDamageAbsorb;
    function GetStruckDamage(MonsterName: string; nDamage: Integer): Integer;
    procedure Clear;
    procedure Run;
    property Count: Integer read GetCount;
    property Items[Index: Integer]: PMonterStruckDamageAbsorb read GetItems;
  end;

implementation

uses
  M2Share;

{ TStruckDamageAbsorbMgr }

constructor TStruckDamageAbsorbMgr.Create;
begin
  FList := TList.Create;
end;

destructor TStruckDamageAbsorbMgr.Destroy;
begin
  Clear;
  FList.Free;
  inherited;
end;

procedure TStruckDamageAbsorbMgr.Clear;
var
  I: Integer;
  Item: PMonterStruckDamageAbsorb;
begin
  for I := 0 to FList.Count - 1 do
  begin
    Item := FList.Items[I];
    Dispose(Item);
  end;
  FList.Clear;
end;

function TStruckDamageAbsorbMgr.Add(MonsterName: string; AbsorbDamageRate, AbsorbDamageValue: Byte; EffectiveTime: LongWord)
  : PMonterStruckDamageAbsorb;
var
  Index: Integer;
begin
  if AbsorbDamageRate > 100 then
    AbsorbDamageRate := 100;
  if AbsorbDamageValue > 100 then
    AbsorbDamageValue := 100;

  if Search(MonsterName, Index) then
  begin
    Result := FList.Items[Index];
    if (AbsorbDamageRate <= 0) or (AbsorbDamageValue <= 0) then
    begin
      Dispose(Result);
      FList.Delete(Index);
      Result := nil;
      Exit;
    end;
  end
  else
  begin
    if (AbsorbDamageRate <= 0) or (AbsorbDamageValue <= 0) then
    begin
      Result := nil;
      Exit;
    end;

    New(Result);
    FList.Insert(Index, Result)
  end;

  Result.MonsterName := MonsterName;
  Result.AbsorbDamageRate := AbsorbDamageRate;
  Result.AbsorbDamageValue := AbsorbDamageValue;
  Result.EffectiveTime := EffectiveTime;
  Result.StartTime := MyGetTickCount;
end;

function TStruckDamageAbsorbMgr.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TStruckDamageAbsorbMgr.GetItems(Index: Integer): PMonterStruckDamageAbsorb;
begin
  if (Index >= 0) and (Index <= FList.Count - 1) then
    Result := FList.Items[Index]
  else
    Result := nil;
end;

procedure TStruckDamageAbsorbMgr.Run;
var
  I: Integer;
  Item: PMonterStruckDamageAbsorb;
begin
  for I := FList.Count - 1 downto 0 do
  begin
    Item := FList.Items[I];
    if (Item.EffectiveTime > 0) and (tick_diff(Item.StartTime, MyGetTickCount) >= Item.EffectiveTime * 1000) then
    begin
      Dispose(Item);
      FList.Delete(I);
    end;
  end;
end;

function TStruckDamageAbsorbMgr.Search(MonsterName: string; var Index: Integer): Boolean;
var
  L, H, C, I: Integer;
  Item: PMonterStruckDamageAbsorb;
begin
  Result := False;

  L := 0;
  H := FList.Count - 1;
  while L <= H do
  begin
    I := (L + H) shr 1;
    Item := FList.Items[I];
    C := AnsiCompareText(Item.MonsterName, MonsterName);
    if C < 0 then
      L := I + 1
    else
    begin
      H := I - 1;
      if C = 0 then
      begin
        Result := True;
        L := I;
      end;
    end;
  end;
  Index := L;
end;

function TStruckDamageAbsorbMgr.GetStruckDamage(MonsterName: string; nDamage: Integer): Integer;
var
  Item: PMonterStruckDamageAbsorb;
  Index: Integer;
begin
  Result := nDamage;

  if Search(MonsterName, Index) then
    Item := FList.Items[Index]
  else if Search('*', Index) then
    Item := FList.Items[Index]
  else
    Item := nil;

  if Item <> nil then
  begin
    if (Random(100) < Item.AbsorbDamageRate) then
    begin
      Result := nDamage - Round(nDamage / 100 * Item.AbsorbDamageValue);
      if Result < 0 then
        Result := 0;
    end;
  end;
end;

end.
