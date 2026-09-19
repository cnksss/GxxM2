unit uMagicACUtils;

interface

uses
  Classes, SysUtils, M2Locker;

type
  PMagicACInfo = ^TMagicACInfo;
  TMagicACInfo = record
    wMagicId: Word;
    boEnabled: Boolean;
    btHum: Byte;
    btMon: Byte;
    btHero: Byte;
    btDefenceHum: Byte;
    btDefenceMon: Byte;
    btDefenceHero: Byte;
    sMagicName: string;
  end;

  TMagicACListSortCompare = function(Magic1, Magic2: PMagicACInfo): Integer;

  TMagicACList = class(TObject)
  private
    FIsSort: Boolean;
    procedure QuickSort(L, R: Integer; SCompare: TMagicACListSortCompare);
  private
    FList: TSafeList;
    function GetCount: Integer;
    function GetItems(Index: Integer): PMagicACInfo;
  public
    constructor Create(ALockName: string);
    destructor Destroy; override;
    procedure Clear;
    procedure Sort; virtual;
    procedure CustomSort(Compare: TMagicACListSortCompare); virtual;

    function Add(MagicID: Word; MagicName: string): PMagicACInfo;
    property Count: Integer read GetCount;
    property Items[Index: Integer]: PMagicACInfo read GetItems; default;

    function Get(MagicID: Word): PMagicACInfo;
  end;

implementation

{ TMagicACList }

constructor TMagicACList.Create(ALockName: string);
begin
  FList := TSafeList.Create(ALockName);
  FIsSort := False;
end;

destructor TMagicACList.Destroy;
begin
  Clear;
  FList.Free;
  inherited;
end;

function TMagicACList.Add(MagicID: Word; MagicName: string): PMagicACInfo;
var
  I: Integer;
  MagicACInfo: PMagicACInfo;
begin
  FList.LockW(1);
  try
    for I := 0 to FList.Count - 1 do
    begin
      MagicACInfo := FList.Items[I];
      if MagicACInfo.wMagicId = MagicID then
      begin
        Result := MagicACInfo;
        Exit;
      end;
    end;

    New(Result);
    Result.wMagicId := MagicID;
    Result.sMagicName := MagicName;
    if MagicID = 12 then
    begin
      Result.boEnabled := True;
      Result.btHum := 100;
      Result.btMon := 100;
      Result.btHero := 100;

      Result.btDefenceHum := 0;
      Result.btDefenceMon := 0;
      Result.btDefenceHero := 0;
    end
    else
    begin
      Result.boEnabled := False;
      Result.btHum := 0;
      Result.btMon := 0;
      Result.btHero := 0;

      Result.btDefenceHum := 0;
      Result.btDefenceMon := 0;
      Result.btDefenceHero := 0;
    end;
    FList.Add(Result);
  finally
    FList.UnLockW;
  end;
end;

procedure TMagicACList.Clear;
var
  I: Integer;
begin
  FList.LockW(2);
  try
    for I := 0 to FList.Count - 1 do
    begin
      Dispose(PMagicACInfo(FList.Items[I]));
    end;
    FList.Clear;
  finally
    FList.UnLockW;
  end;
end;

function TMagicACList.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TMagicACList.GetItems(Index: Integer): PMagicACInfo;
begin
  FList.LockR(3);
  try
    if (Index >= 0) and (Index <= FList.Count - 1) then
      Result := FList.Items[index]
    else
      Result := nil;
  finally
    FList.UnLockR;
  end;
end;

function MagicACListCompareMagicID(Magic1, Magic2: PMagicACInfo): Integer;
begin
  Result := Magic1.wMagicId - Magic2.wMagicId;
end;

procedure TMagicACList.Sort;
begin
  CustomSort(MagicACListCompareMagicID);
end;

procedure TMagicACList.CustomSort(Compare: TMagicACListSortCompare);
begin
  FList.LockW(4);
  try
    if not FIsSort and (FList.Count > 1) then
    begin
      QuickSort(0, FList.Count - 1, Compare);
      FIsSort := True;
    end;
  finally
    FList.UnLockW;
  end;
end;

procedure TMagicACList.QuickSort(L, R: Integer;
  SCompare: TMagicACListSortCompare);
var
  I, J, P: Integer;
begin
  repeat
    I := L;
    J := R;
    P := (L + R) shr 1;
    repeat
      while SCompare(FList.Items[I], FList.Items[P]) < 0 do Inc(I);
      while SCompare(FList.Items[J], FList.Items[P]) > 0 do Dec(J);
      if I <= J then
      begin
        FList.Exchange(I, J);
        if P = I then
          P := J
        else if P = J then
          P := I;
        Inc(I);
        Dec(J);
      end;
    until I > J;
    if L < J then QuickSort(L, J, SCompare);
    L := I;
  until I >= R;
end;

function TMagicACList.Get(MagicID: Word): PMagicACInfo;
var
  L, H, I, C: Integer;
  MagicACInfo: PMagicACInfo;
begin
  FList.LockR(5);
  try
    Result := nil;
    if FIsSort then
    begin
      L := 0;
      H := FList.Count - 1;
      while L <= H do
      begin
        I := L + (H - L) div 2;
        MagicACInfo := FList[I];
        C := MagicACInfo.wMagicId - MagicID;
        if C < 0 then L := I + 1 else
        begin
          H := I - 1;
          if C = 0 then
          begin
            Result := MagicACInfo;
            Break;
          end;
        end;
      end;
    end
    else
    begin
      for I := 0 to FList.Count - 1 do
      begin
        MagicACInfo := FList[I];
        if MagicACInfo.wMagicId = MagicID then
        begin
          Result := MagicACInfo;
          Break;
        end;
      end;
    end;
  finally
    FList.UnLockR;
  end;
end;

end.
