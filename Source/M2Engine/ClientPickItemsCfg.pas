{
  客户端内挂上传的捡取物品

  这样搞的原因是减少内存占用 2020-03-08 01:36:31

  X X 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
  前面02bit是标记：特殊物品，可捡物品
  后面14bit是物品数据库的Idx + 1，金币特殊处理Idx = 0
}

unit ClientPickItemsCfg;

interface

uses
  Windows, Classes, SysUtils;

type
  TClientPickItems = class(TObject)
  private
    FItems: array of Word;
    function CompareItem(Index1, Index2: Integer): Integer;
    function SearchItem(ItemIdx: Integer): Integer;
    function GetCount: Integer;
  public
    constructor Create;
    destructor Destroy; override;

    procedure Clear;

    property Count: Integer read GetCount;
    
    procedure SetData(Buf: PByte; BufLen: Integer);
    procedure Sort(L, R: Integer);

    function CheckPriorityPickup(ItemIdx: Integer): Boolean;          // 检查是否为优先捡取
    function CheckEnablePickup(ItemIdx: Integer): Boolean;            // 检查是否为捡取物品

    function CheckCanPickItem(ItemIdx: Integer): Boolean;             // 检查是否可捡  优先 或 捡取
  end;

implementation

{ TClientPickItems }

constructor TClientPickItems.Create;
begin

end;

destructor TClientPickItems.Destroy;
begin
  FItems := nil;
  inherited;
end;

procedure TClientPickItems.Clear;
begin
  SetLength(FItems, 0);
end;

function TClientPickItems.GetCount: Integer;
begin
  Result := Length(FItems);
end;

function TClientPickItems.CheckPriorityPickup(ItemIdx: Integer): Boolean;
var
  Index: Integer;
begin
  Result := False;
  Index := SearchItem(ItemIdx);
  if Index >= 0 then
  begin
    Result := FItems[Index] and $8000 <> 0;
  end;
end;

function TClientPickItems.CheckEnablePickup(ItemIdx: Integer): Boolean;
var
  Index: Integer;
begin
  Result := False;
  Index := SearchItem(ItemIdx);
  if Index >= 0 then
  begin
    Result := FItems[Index] and $4000 <> 0;
  end;
end;

function TClientPickItems.CheckCanPickItem(ItemIdx: Integer): Boolean;
var
  Index: Integer;
begin
  Result := False;
  Index := SearchItem(ItemIdx);
  if Index >= 0 then
  begin
    Result := FItems[Index] and $C000 <> 0;       // 允许捡或优先捡
  end;
end;

procedure TClientPickItems.SetData(Buf: PByte; BufLen: Integer);
var
  Count: Integer;
begin
  if BufLen mod 2 <> 0 then
  begin
    Exit;
  end;

  Count := BufLen div 2;
  SetLength(FItems, Count);
  if Count > 0 then
  begin
    Move(Buf^, FItems[0], BufLen);

    Sort(0, Count - 1);
  end;
end;

function TClientPickItems.CompareItem(Index1, Index2: Integer): Integer;
var
  V1, V2: Integer;
begin
  V1 := FItems[Index1] and $3FFF;
  V2 := FItems[Index2] and $3FFF;
  Result := V1 - V2;
end;

procedure TClientPickItems.Sort(L, R: Integer);
var
  I, J, P: Integer;
  TempValue: Word;
begin
  repeat
    I := L;
    J := R;
    P := (L + R) shr 1;
    repeat
      while CompareItem(I, P) < 0 do Inc(I);
      while CompareItem(J, P) > 0 do Dec(J);
      if I <= J then
      begin
        if I <> J then
        begin
          TempValue := FItems[I];
          FItems[I] := FItems[J];
          FItems[J] := TempValue;
        end;

        if P = I then
          P := J
        else if P = J then
          P := I;
        Inc(I);
        Dec(J);
      end;
    until I > J;
    if L < J then Sort(L, J);
    L := I;
  until I >= R;
end;

function TClientPickItems.SearchItem(ItemIdx: Integer): Integer;
var
  L, H, I, C: Integer;
  CurItem: Integer;
begin
  Result := -1;
  L := 0;
  H := Length(FItems) - 1;
  while L <= H do
  begin
    I := L + (H - L) shr 1;
    CurItem := FItems[I] and $3FFF;
    C := CurItem - ItemIdx;
    if C < 0 then
      L := I + 1
    else
    begin
      H := I - 1;
      if C = 0 then
      begin
        L := I;
        Result := I;
      end;
    end;
  end;
end;

end.
