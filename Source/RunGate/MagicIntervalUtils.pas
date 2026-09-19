unit MagicIntervalUtils;

interface

uses
  Windows, Classes, SysUtils, SyncObjs;

type
  PMagicInterval = ^TMagicInterval;
  TMagicInterval = record
    MagicId: Word;
    Interval: LongWord;
  end;

  TMagicIntervalList = class(TObject)
  private
    FCS: TRTLCriticalSection;
    FDuplicates: Boolean;
    FList: TList;
    function GetCount: Integer;
    function GetItems(Index: Integer): PMagicInterval;
  public
    constructor Create;
    destructor Destroy; override;

    property Duplicates: Boolean read FDuplicates write FDuplicates;
    property Count: Integer read GetCount;
    property Items[Index: Integer]: PMagicInterval read GetItems;

    procedure Clear;
    function Add(MagicID: Word): PMagicInterval;
    function Find(MagicID: Word): PMagicInterval;
    function Search(MagicID: Word; var Index: Integer): Boolean; virtual;

    procedure Lock;
    procedure UnLock;
    
    procedure SaveToFile(FileName: string);
    procedure LoadFromFile(FileName: string);
  end;

implementation

{ TMagicIntervalList }

constructor TMagicIntervalList.Create;
begin
  FList := TList.Create;
  FDuplicates := False;
  InitializeCriticalSection(FCS);
end;

destructor TMagicIntervalList.Destroy;
begin
  Clear;
  FList.Free;
  DeleteCriticalSection(FCS);
  inherited;
end;

function TMagicIntervalList.Add(MagicID: Word): PMagicInterval;
var
  I: Integer;
begin
  Result := nil;
  if not Search(MagicID, I) or FDuplicates then
  begin
    New(Result);
    Result.MagicId := MagicID;
    FList.Insert(I, Result);
  end;
end;

procedure TMagicIntervalList.Clear;
var
  I: Integer;
begin
  for I := 0 to FList.Count - 1 do
  begin
    Dispose(PMagicInterval(FList.Items[I]));
  end;
  FList.Clear
end;

function TMagicIntervalList.Find(MagicID: Word): PMagicInterval;
var
  I: Integer;
begin
  if not Search(MagicID, I) then
    Result := nil
  else
    Result := FList.Items[I];
end;

function TMagicIntervalList.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TMagicIntervalList.GetItems(Index: Integer): PMagicInterval;
begin
  if (Index >= 0) and (Index <= FList.Count - 1) then
    Result := FList[Index]
  else
    Result := nil;
end;

function TMagicIntervalList.Search(MagicID: Word; var Index: Integer): Boolean;
var
  L, H, I, C: Integer;
begin
  Result := False;
  L := 0;
  H := FList.Count - 1;
  while L <= H do
  begin
    I := (L + H) shr 1;
    C := PMagicInterval(FList.Items[I]).MagicId - MagicID;
    if C < 0 then
      L := I + 1
    else
    begin
      H := I - 1;
      if C = 0 then
      begin
        Result := True;
        if not FDuplicates then L := I;
      end;
    end;
  end;
  Index := L;
end;

procedure TMagicIntervalList.LoadFromFile(FileName: string);
var
  I: Integer;
  sMagicID, sMagicValue: string;
  nMagicID, nMagicValue: Integer;
  SL: TStringList;
  MagicInterval: PMagicInterval;
begin
  if not FileExists(FileName) then Exit;
  Clear;
  SL := TStringList.Create;
  try
    SL.LoadFromFile(FileName);
    for I := 0 to SL.Count - 1 do
    begin
      sMagicID := Trim(SL.Names[I]);
      sMagicValue := Trim(SL.ValueFromIndex[I]);

      nMagicID := StrToIntDef(sMagicID, -1);
      nMagicValue := StrToIntDef(sMagicValue, -1);
      if (nMagicID >= 0) and (nMagicValue >= 0) then
      begin
        MagicInterval := Add(nMagicID);
        if MagicInterval <> nil then
        begin
          MagicInterval.Interval := nMagicValue;
        end;
      end;
    end;
  finally
    SL.Free;
  end;
end;

procedure TMagicIntervalList.SaveToFile(FileName: string);
var
  I: Integer;
  SL: TStringList;
  MagicInterval: PMagicInterval;
begin
  SL := TStringList.Create;
  try
    for I := 0 to FList.Count - 1 do
    begin
      MagicInterval := FList.Items[I];
      SL.Add(IntToStr(MagicInterval.MagicId) + '=' + IntToStr(MagicInterval.Interval));
    end;

    SL.SaveToFile(FileName);
  finally
    SL.Free;
  end;
end;

procedure TMagicIntervalList.Lock;
begin
  EnterCriticalSection(FCS);
end;

procedure TMagicIntervalList.UnLock;
begin
  LeaveCriticalSection(FCS);
end;

end.
