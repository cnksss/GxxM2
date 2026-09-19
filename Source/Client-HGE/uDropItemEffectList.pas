unit uDropItemEffectList;

interface

uses
  Windows,
  SysUtils,
  Classes,
  Grobal2;

type
  TDropItemEffectList = class(TObject)
  private
    FList:TList;
    FIsSorted:Boolean;
    function GetCount:Integer;
    function GetItems(Index:Integer):PDropItemEffect;

    procedure QuickSort(L, R:Integer);
    function CompareItem(Index1, Index2:Integer):Integer;
  public
    constructor Create;
    destructor Destroy; override;

    property Count:Integer read GetCount;
    property Items[Index:Integer]:PDropItemEffect read GetItems;

    function Add(DropItemEffect:PDropItemEffect):PDropItemEffect;
    function IndexOf(ItemEffectIndex:Integer):Integer;
    function Get(ItemEffectIndex:Integer):PDropItemEffect;

    procedure Sort;
    procedure Clear;
  end;

implementation

{ TDropItemEffectList }

constructor TDropItemEffectList.Create;
begin
  FList := TList.Create;
  FIsSorted := False;
end;

destructor TDropItemEffectList.Destroy;
begin
  Clear;
  FList.Free;
  inherited;
end;

function TDropItemEffectList.Add(DropItemEffect:PDropItemEffect):PDropItemEffect;
var
  Index:Integer;
begin
  Index := IndexOf(DropItemEffect.ItemEffectIndex);
  if Index < 0 then begin
    New(Result);
    FList.Add(Result);
    FIsSorted := False;
  end
  else begin
    Result := FList.Items[Index];
  end;
  Result^ := DropItemEffect^;
end;

procedure TDropItemEffectList.Clear;
var
  I:Integer;
  Item:PDropItemEffect;
begin
  for I := 0 to FList.Count - 1 do begin
    Item := FList.Items[I];
    Dispose(Item);
  end;
  FList.Clear;
  FIsSorted := False;
end;

function TDropItemEffectList.Get(ItemEffectIndex:Integer):PDropItemEffect;
var
  Index:Integer;
begin
  Index := IndexOf(ItemEffectIndex);
  if Index < 0 then
    Result := nil
  else
    Result := FList.Items[Index];
end;

function TDropItemEffectList.GetCount:Integer;
begin
  Result := FList.Count;
end;

function TDropItemEffectList.GetItems(Index:Integer):PDropItemEffect;
begin
  if (Index < 0) or (Index >= FList.Count) then
    Result := nil
  else
    Result := FList.Items[Index];
end;

function TDropItemEffectList.IndexOf(ItemEffectIndex:Integer):Integer;
var
  L, H, I, C:Integer;
  Item:PDropItemEffect;
begin
  Result := -1;
  if FList.Count = 0 then Exit;

  if FIsSorted then begin
    L := 0;
    H := FList.Count - 1;
    while L <= H do begin
      I := (L + H) shr 1;
      Item := FList.Items[I];
      C := Item.ItemEffectIndex - ItemEffectIndex;
      if C < 0 then
        L := I + 1
      else begin
        H := I - 1;
        if C = 0 then begin
          Result := I;
          Break;
        end;
      end;
    end;
  end
  else begin
    for I := 0 to FList.Count - 1 do begin
      Item := FList.Items[I];
      if Item.ItemEffectIndex = ItemEffectIndex then begin
        Result := I;
        Break;
      end;
    end;
  end;
end;

procedure TDropItemEffectList.Sort;
begin
  if FList.Count > 0 then begin
    QuickSort(0, FList.Count - 1);
    FIsSorted := True;
  end;
end;

procedure TDropItemEffectList.QuickSort(L, R:Integer);
var
  I, J, P:Integer;
begin
  repeat
    I := L;
    J := R;
    P := (L + R) shr 1;
    repeat
      while CompareItem(I, P) < 0 do Inc(I);
      while CompareItem(J, P) > 0 do Dec(J);
      if I <= J then begin
        FList.Exchange(I, J);
        if P = I then
          P := J
        else if P = J then
          P := I;
        Inc(I);
        Dec(J);
      end;
    until I > J;
    if L < J then QuickSort(L, J);
    L := I;
  until I >= R;
end;

function TDropItemEffectList.CompareItem(Index1, Index2:Integer):Integer;
begin
  Result := PDropItemEffect(FList[Index1]).ItemEffectIndex - PDropItemEffect(FList[Index2]).ItemEffectIndex;
end;

end.
