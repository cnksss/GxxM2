unit BagItemList;

interface

{$I Iocp.inc}

uses
  Classes, SysUtils, IocpCommon;

type
  PBagItem = ^TBagItem;
  TBagItem = packed record
    MakeIndex: Integer;
    StdMode: Byte;
    Shape: Byte;
    Reserved: Word;
    //Name: string[30];
    AC1: Integer;
    MAC1: Integer;
  end;

  TBagItemList = class(TObject)
  private
    FDuplicates: Boolean;
    FList: TList;
    FLocker: TIocpCriticalSection;
    function GetCapacity: Integer;
    //function GetCount: Integer;
    //function GetItems(Index: Integer): PBagItem;
    procedure SetCapacity(const Value: Integer);
  protected
    function Search(MakeIndex: Integer; var Index: Integer): Boolean; virtual;
  public
    constructor Create(ACapacity: Integer); virtual;
    destructor Destroy; override;

    function Add(MakeIndex: Integer): PBagItem; virtual;
    function Find(MakeIndex: Integer): PBagItem;

    procedure Clear; virtual;
    procedure Delete(Index: Integer);
    procedure Remove(MakeIndex: Integer); overload;
    procedure Remove(BagItem: PBagItem); overload;

    property Capacity: Integer read GetCapacity write SetCapacity;
    //property Count: Integer read GetCount;
    //property Items[Index: Integer]: PBagItem read GetItems; default;
    property Duplicates: Boolean read FDuplicates write FDuplicates;

    procedure Lock;
    procedure UnLock;
  end;

implementation

{ TBagItemList }

constructor TBagItemList.Create(ACapacity: Integer);
begin
  FLocker := TIocpCriticalSection.Create({$IFDEF USE_SPINLOCK}'TBagItemList.FLocker'{$ENDIF});
  FList := TList.Create;
  FList.Capacity := ACapacity;
  FDuplicates := False;
end;

function TBagItemList.Add(MakeIndex: Integer): PBagItem;
var
  Index: Integer;
begin
  if Search(MakeIndex, Index) then
  begin
    Result := FList.Items[Index];
  end
  else
  begin
    New(Result);
    Result.MakeIndex := MakeIndex;
    FList.Insert(Index, Result);
  end;
end;

procedure TBagItemList.Clear;
var
  I: Integer;
  BagItem: PBagItem;
begin
  for I := 0 to FList.Count - 1 do
  begin
    BagItem := FList.Items[I];
    Dispose(BagItem);
  end;
  FList.Clear;
end;

procedure TBagItemList.Delete(Index: Integer);
begin
  if (Index >= 0) and (Index < FList.Count) then
  begin
    Dispose(PBagItem(FList.Items[Index]));
    FList.Delete(Index);
  end;
end;

destructor TBagItemList.Destroy;
begin
  Clear;
  FList.Free;
  FLocker.Free;
  inherited;
end;

function TBagItemList.GetCapacity: Integer;
begin
  Result := FList.Capacity;
end;

{
function TBagItemList.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TBagItemList.GetItems(Index: Integer): PBagItem;
begin
  if (Index >= 0) and (Index < FList.Count) then
    Result := FList.Items[Index]
  else
    Result := nil;
end;
}

procedure TBagItemList.Remove(MakeIndex: Integer);
var
  Index: Integer;
begin
  if Search(MakeIndex, Index) then
  begin
    Dispose(PBagItem(FList.Items[Index]));
    FList.Delete(Index);
  end;
end;

procedure TBagItemList.Remove(BagItem: PBagItem);
begin
  FList.Remove(BagItem);
  Dispose(BagItem);
end;

function TBagItemList.Search(MakeIndex: Integer;
  var Index: Integer): Boolean;
var
  L, H, I, C: Integer;
begin
  Result := False;
  L := 0;
  H := FList.Count - 1;
  while L <= H do
  begin
    I := L + (H - L) shr 2;
    C := PBagItem(FList.Items[I]).MakeIndex - MakeIndex;
    if C < 0 then
      L := I + 1
    else
    begin
      H := I - 1;
      if C = 0 then
      begin
        Result := True;
        if not Duplicates then L := I;
      end;
    end;
  end;
  Index := L;
end;

procedure TBagItemList.SetCapacity(const Value: Integer);
begin
  FList.Capacity := Value;
end;

function TBagItemList.Find(MakeIndex: Integer): PBagItem;
var
  Index: Integer;
begin
  if Search(MakeIndex, Index) then
    Result := FList.Items[Index]
  else
    Result := nil;
end;

procedure TBagItemList.Lock;
begin
  FLocker.Lock;
end;

procedure TBagItemList.UnLock;
begin
  FLocker.UnLock;
end;

end.
