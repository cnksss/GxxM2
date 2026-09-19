unit WideCharList;

interface

uses
  Windows,
  SysUtils,
  Classes;

type
  PWideCharItem = ^TWideCharItem;
  TWideCharItem = record
    C:WideChar;
    P:Pointer;
  end;

  TWideCharList = class(TObject)
  private
    FList:TList;
    FCriticalSection:TRTLCriticalSection;
    function FindIndex(WC:WideChar; var Index:Integer):Integer;
  public
    constructor Create(Size:Cardinal = 512);
    destructor Destroy; override;
    function TryLock:Boolean;
    procedure Lock;
    procedure UnLock;
    procedure Clear;
    function Find(WC:WideChar):Pointer;
    procedure Remove(WC:WideChar);
    function Add(WC:WideChar; Point:Pointer):PWideCharItem;
  end;

implementation

{ TWideCharList }

constructor TWideCharList.Create(Size:Cardinal);
begin
  inherited Create;
  FList := TList.Create;
  FList.Capacity := Size;
  InitializeCriticalSection(FCriticalSection);
end;

destructor TWideCharList.Destroy;
begin
  Clear;
  FList.Free;
  DeleteCriticalSection(FCriticalSection);
  inherited;
end;

procedure TWideCharList.Lock;
begin
  EnterCriticalSection(FCriticalSection);
end;

function TWideCharList.TryLock:Boolean;
begin
  Result := TryEnterCriticalSection(FCriticalSection);
end;

procedure TWideCharList.UnLock;
begin
  LeaveCriticalSection(FCriticalSection);
end;

procedure TWideCharList.Clear;
var
  I:Integer;
  Item:PWideCharItem;
begin
  for I := 0 to FList.Count - 1 do begin
    Item := FList.Items[I];
    Dispose(Item);
  end;
  FList.Clear;
end;

function TWideCharList.Find(WC:WideChar):Pointer;
var
  L, H, I, C:Integer;
  Item:PWideCharItem;
begin
  Result := nil;
  L := 0;
  H := FList.Count - 1;
  while L <= H do begin
    I := L + (H - L) div 2;
    Item := FList.Items[I];
    C := Ord(Item.C) - Ord(WC);
    if C < 0 then
      L := I + 1
    else begin
      H := I - 1;
      if C = 0 then begin
        Result := Item.P;
        Exit;
      end;
    end;
  end;
end;

function TWideCharList.FindIndex(WC:WideChar; var Index:Integer):Integer;
var
  L, H, I, C:Integer;
  Item:PWideCharItem;
begin
  Result := -1;
  L := 0;
  H := FList.Count - 1;
  while L <= H do begin
    I := L + (H - L) div 2;
    Item := FList.Items[I];
    C := Ord(Item.C) - Ord(WC);
    if C < 0 then
      L := I + 1
    else begin
      H := I - 1;
      if C = 0 then begin
        Result := I;
      end;
    end;
  end;
  Index := L;
end;

procedure TWideCharList.Remove(WC:WideChar);
var
  Index, I:Integer;
  Item:PWideCharItem;
begin
  Index := FindIndex(WC, I);
  if Index <> -1 then begin
    Item := FList.Items[Index];
    Dispose(Item);
    FList.Delete(Index);
  end;
end;

function TWideCharList.Add(WC:WideChar; Point:Pointer):PWideCharItem;
var
  Index, I:Integer;
  Item:PWideCharItem;
begin
  Result := nil;
  Index := FindIndex(WC, I);
  if Index = -1 then begin
    New(Item);
    Item.C := WC;
    Item.P := Point;
    Result := Item;

    if I >= FList.Count then
      FList.Add(Item)
    else
      FList.Insert(I, Item);
  end;
end;

end.
