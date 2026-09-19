unit uRunGateList;

interface

uses
  Windows, Classes, SysUtils;

type
  PRunGateInfo = ^TRunGateInfo;
  TRunGateInfo = record
    Enabled: Boolean;
    IP: string[15];
    Port: Word;
    DBPort: Word;
    Level: Integer;
    LastResponseTick: LongWord;     // 最后响应时间 chongchong 2015-07-27
    IsConnect: Boolean;
  end;

type
  TRunGateList = class(TObject)
  private
    FList: TList;
    FSortList: TList;
    function GetItems(Index: Integer): PRunGateInfo;
    function GetSortItems(Index: Integer): PRunGateInfo;
    function GetCount: Integer;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Clear;
    procedure DoSort;
    function Add(AEnabled: Boolean; AIP: string; APort, ADBPort: Word; ALevel: Integer): PRunGateInfo;

    property Count: Integer read GetCount;
    property Items[Index: Integer]: PRunGateInfo read GetItems;
    property SortItems[Index: Integer]: PRunGateInfo read GetSortItems;
  end;

implementation

{ TRunGateList }

constructor TRunGateList.Create;
begin
  FList := TList.Create;
  FSortList := TList.Create;
end;

destructor TRunGateList.Destroy;
begin
  Clear;
  FList.Free;
  FSortList.Free;
  inherited;
end;

function TRunGateList.Add(AEnabled: Boolean; AIP: string; APort, ADBPort: Word;
  ALevel: Integer): PRunGateInfo;
begin
  New(Result);
  FList.Add(Result);
  Result.Enabled := AEnabled;
  Result.IP := AIP;
  Result.Port := APort;
  Result.DBPort := ADBPort;
  Result.Level := ALevel;
  Result.LastResponseTick := GetTickCount;
end;

procedure TRunGateList.Clear;
var
  I: Integer;
begin
  for I := 0 to FList.Count - 1 do
  begin
    Dispose(PRunGateInfo(FList.Items[I]));
  end;
  FList.Clear;
  FSortList.Clear;
end;

procedure TRunGateList.DoSort;
  procedure QuickSort(L, R: Integer);
  var
    I, J: Integer;
    nLevel: Integer;
  begin
    repeat
      I := L;
      J := R;
      nLevel := PRunGateInfo(FSortList[L + (R - L) div 2]).Level;
      repeat
        while PRunGateInfo(FSortList[I]).Level > nLevel do
          Inc(I);
        while PRunGateInfo(FSortList[J]).Level < nLevel do
          Dec(J);

        if I <= J then
        begin
          FSortList.Exchange(I, J);
          Inc(I);
          Dec(J);
        end;
      until I > J;
      if L < J then QuickSort(L, J);
      L := I;
    until I >= R;
  end;
begin
  if FList.Count > 1 then
  begin
    FSortList.Clear;
    FSortList.Assign(FList);
    QuickSort(0, FSortList.Count - 1);
  end;
end;

function TRunGateList.GetItems(Index: Integer): PRunGateInfo;
begin
  Result := nil;
  if (Index >= 0) and (Index < FList.Count) then
  begin
    Result := FList.Items[Index];
  end;
end;

function TRunGateList.GetSortItems(Index: Integer): PRunGateInfo;
begin
  Result := nil;
  if (Index >= 0) and (Index < FSortList.Count) then
  begin
    Result := FSortList.Items[Index];
  end;
end;

function TRunGateList.GetCount: Integer;
begin
  Result := FList.Count;
end;

end.
