unit SDK;

interface
uses
  Windows, SysUtils, Classes, Grobal2, Forms;
const
  MAXPULGCOUNT = 20;

  TESTMODE = 1;
  PRIVATE_CLIENT = 0;
  
type
  TPlugFile = record
    Module: Integer;
    Size: Integer;
    Crc: Integer;
  end;
  pTPlugFile = ^TPlugFile;

  TProcArrayInfo = record
    sProcName: string;
    nProcAddr: Pointer;
  end;
  pTProcArrayInfo = ^TProcArrayInfo;

  TObjectArrayInfo = record
    Obj: TObject;
    sObjcName: string;
  end;
  pTObjectArrayInfo = ^TObjectArrayInfo;

  TProcArray = array[0..MAXPULGCOUNT - 1] of TProcArrayInfo;
  TObjectArray = array[0..MAXPULGCOUNT - 1] of TObjectArrayInfo;

  TPlugBuffer = record
    AppHandle: HWnd;
    ServerGetHandle: Cardinal;
    ServerGetCrc: Cardinal;
    PlugGetHandle: Cardinal;
    PlugGetCrc: Cardinal;
  end;

  TMsgProc = procedure(Msg: PChar; nMsgLen: Integer; nMode: Integer); stdcall;
  TFindProc = function(ProcName: PChar; nNameLen: Integer): Pointer; stdcall;
  TSetProc = function(ProcAddr: Pointer; ProcName: PChar; nNameLen: Integer): Boolean; stdcall;
  TFindObj = function(ObjName: PChar; nNameLen: Integer): TObject; stdcall;
  TStartPlug = function(): Boolean; stdcall;
  TSetStartPlug = function(StartPlug: TStartPlug): Boolean; stdcall;

  TPlugInit = function(Apphandle: THandle; MsgProc: TMsgProc; FindProc: TFindProc; SetProc: TSetProc; FindObj: TFindObj): PChar; stdcall;
  TClassProc = procedure(Sender: TObject);
  TStartProc = procedure(); stdcall;
  TStartRegister = function(sRegisterInfo: PChar): Boolean; stdcall;
  TGetStrProc = procedure(sRegisterCode: PChar); stdcall;
  TGameDataLog = function(ProcName: PChar; nNameLen: Integer): Boolean; stdcall;
  TIPLocal = procedure(sIPaddr: PChar; sLocal: PChar; nLocalLen: Integer); stdcall;
  TDeCryptString = procedure(Src: PChar; Dest: PChar; var nDest: Integer; Pwd: PChar; PwdLen: Integer); stdcall;

  TStart = procedure();

  TGetProcInt = function(): Integer; stdcall;

  TSetPlugBuffer = procedure(var MemoryStream: TMemoryStream); stdcall;
  {===================================TGList===================================}

  TGList = class(TList)
  private
    CriticalSection: TRTLCriticalSection;
  public
    constructor Create;
    destructor Destroy; override;
    function TryLock: Boolean;
    procedure Lock;
    procedure UnLock;
    procedure Up(Item: Pointer);
    procedure Down(Item: Pointer);
  end;

  {=================================TGStringList================================}
  TGStringList = class(TStringList)
  private
    CriticalSection: TRTLCriticalSection;
  public
    constructor Create;
    destructor Destroy; override;
    function TryLock: Boolean;
    procedure Lock;
    procedure UnLock;
    procedure Up(AObject: TObject);
    procedure Down(AObject: TObject);
  end;
  TSortStringList = class(TStringList)
  public
    procedure NumberSort(Flag: Boolean);
    procedure DateTimeSort(Flag: Boolean);
    procedure StringSort(Flag: Boolean);
  end;
{ TValueList class }

  TValueList = class;

  PValueItem = ^TValueItem;
  TValueItem = record
    FName: string;
    FString: string;
    FObject: TObject;
  end;

  PValueItemList = ^TValueItemList;
  TValueItemList = array[0..MAXINT div 64] of TValueItem;
  TValueListSortCompare = function(List: TValueList; Index1, Index2: Integer): Integer;

  TValueList = class(TStrings)
  private
    FList: PValueItemList;
    FCount: Integer;
    FCapacity: Integer;
    FSorted: Boolean;
    FDuplicates: TDuplicates;
    FCaseSensitive: Boolean;
    FOnChange: TNotifyEvent;
    FOnChanging: TNotifyEvent;
    procedure ExchangeItems(Index1, Index2: Integer);
    procedure Grow;
    procedure QuickSort(L, R: Integer; SCompare: TValueListSortCompare);
    procedure SetSorted(Value: Boolean);
    procedure SetCaseSensitive(const Value: Boolean);

    function GetName(Index: Integer): string;
    procedure PutName(Index: Integer; const AName: string);
  protected
    procedure Changed; virtual;
    procedure Changing; virtual;
    function Get(Index: Integer): string; override;
    function GetCapacity: Integer; override;
    function GetCount: Integer; override;
    function GetObject(Index: Integer): TObject; override;
    procedure Put(Index: Integer; const S: string); override;
    procedure PutObject(Index: Integer; AObject: TObject); override;
    procedure SetCapacity(NewCapacity: Integer); override;
    procedure SetUpdateState(Updating: Boolean); override;
    function CompareStrings(const S1, S2: string): Integer; override;
    procedure InsertItem(Index: Integer; const AName, S: string; AObject: TObject); virtual;
  public
    destructor Destroy; override;
    function Add(const AName, S: string): Integer; reintroduce;
    function AddObject(const AName, S: string; AObject: TObject): Integer; reintroduce;
    procedure Clear; override;
    procedure Delete(Index: Integer); override;
    procedure Exchange(Index1, Index2: Integer); override;
    function Find(const S: string; var Index: Integer): Boolean; virtual;
    function IndexOf(const S: string): Integer; override;
    procedure Insert(Index: Integer; const AName, S: string); reintroduce;
    procedure InsertObject(Index: Integer; const AName, S: string;
      AObject: TObject); reintroduce;
    procedure Sort; virtual;
    procedure CustomSort(Compare: TValueListSortCompare); virtual;

    procedure SortString(nMin, nMax: Integer); virtual;
    function GetIndex(const AName: string): Integer; virtual;
    function AddRecord(const AName, S: string): Boolean; overload;
    function AddRecord(const AName: string; const Value: Integer): Boolean; overload;
    function AddRecord(const AName, S: string; const Value: Integer): Boolean; overload; virtual;

    property Duplicates: TDuplicates read FDuplicates write FDuplicates;
    property Sorted: Boolean read FSorted write SetSorted;
    property CaseSensitive: Boolean read FCaseSensitive write SetCaseSensitive;

    property Names[Index: Integer]: string read GetName write PutName; default;

    property OnChange: TNotifyEvent read FOnChange write FOnChange;
    property OnChanging: TNotifyEvent read FOnChanging write FOnChanging;
  end;

function ObjIntegerSort_1(List: TStringList; Index1, Index2: Integer): Integer;
function ObjIntegerSort_2(List: TStringList; Index1, Index2: Integer): Integer;

function ObjLongWordSort_1(List: TStringList; Index1, Index2: Integer): Integer;
function ObjLongWordSort_2(List: TStringList; Index1, Index2: Integer): Integer;
implementation
uses RTLConsts;

function NumberSort_1(List: TStringList; Index1, Index2: Integer): Integer;
var
  Value1, Value2: Integer;
begin
  Result := 0;
  try
    Value1 := StrToInt(List[Index1]);
    Value2 := StrToInt(List[Index2]);
    if Value1 > Value2 then
      Result := -1
    else if Value1 < Value2 then
      Result := 1
    else
      Result := 0;
  except
  end;
end;

// -------Êý×ÖÅÅÐò 2

function NumberSort_2(List: TStringList; Index1, Index2: Integer): Integer;
var
  Value1, Value2: Integer;
begin
  Result := 0;
  try
    Value1 := StrToInt(List[Index1]);
    Value2 := StrToInt(List[Index2]);
    if Value1 > Value2 then
      Result := 1
    else if Value1 < Value2 then
      Result := -1
    else
      Result := 0;
  except
  end;
end;

function ObjIntegerSort_1(List: TStringList; Index1, Index2: Integer): Integer;
var
  Value1, Value2: Integer;
begin
  Result := 0;
  try
    Value1 := Integer(List.Objects[Index1]);
    Value2 := Integer(List.Objects[Index2]);
    if Value1 > Value2 then
      Result := -1
    else if Value1 < Value2 then
      Result := 1
    else
      Result := 0;
  except
  end;
end;

function ObjIntegerSort_2(List: TStringList; Index1, Index2: Integer): Integer;
var
  Value1, Value2: Integer;
begin
  Result := 0;
  try
    Value1 := Integer(List.Objects[Index1]);
    Value2 := Integer(List.Objects[Index2]);
    if Value1 > Value2 then
      Result := 1
    else if Value1 < Value2 then
      Result := -1
    else
      Result := 0;
  except
  end;
end;

function ObjLongWordSort_1(List: TStringList; Index1, Index2: Integer): Integer;
var
  Value1, Value2: LongWord;
begin
  Result := 0;
  try
    Value1 := LongWord(List.Objects[Index1]);
    Value2 := LongWord(List.Objects[Index2]);
    if Value1 > Value2 then
      Result := -1
    else if Value1 < Value2 then
      Result := 1
    else
      Result := 0;
  except
  end;
end;

function ObjLongWordSort_2(List: TStringList; Index1, Index2: Integer): Integer;
var
  Value1, Value2: LongWord;
begin
  Result := 0;
  try
    Value1 := LongWord(List.Objects[Index1]);
    Value2 := LongWord(List.Objects[Index2]);
    if Value1 > Value2 then
      Result := 1
    else if Value1 < Value2 then
      Result := -1
    else
      Result := 0;
  except
  end;
end;

// -------ÈÕÆÚÅÅÐò 1

function DateTimeSort_1(List: TStringList; Index1, Index2: Integer): Integer;
var
  Value1, Value2: TDateTime;
begin
  Result := 0;
  try
    Value1 := StrToDateTime(List[Index1]);
    Value2 := StrToDateTime(List[Index2]);
    if Value1 > Value2 then
      Result := -1
    else if Value1 < Value2 then
      Result := 1
    else
      Result := 0;
  except
  end;
end;

// -------ÈÕÆÚÅÅÐò 2

function DateTimeSort_2(List: TStringList; Index1, Index2: Integer): Integer;
var
  Value1, Value2: TDateTime;
begin
  Result := 0;
  try
    Value1 := StrToDateTime(List[Index1]);
    Value2 := StrToDateTime(List[Index2]);
    if Value1 > Value2 then
      Result := 1
    else if Value1 < Value2 then
      Result := -1
    else
      Result := 0;
  except
  end;
end;

// -------×Ö·û´®ÅÅÐò 1

function StrSort_1(List: TStringList; Index1, Index2: Integer): Integer;
begin
  Result := 0;
  try
    Result := -CompareStr(List[Index1], List[Index2]);
  except
  end;
end;

// -------×Ö·û´®ÅÅÐò 2

function StrSort_2(List: TStringList; Index1, Index2: Integer): Integer;
begin
  Result := 0;
  try
    Result := CompareStr(List[Index1], List[Index2]);
  except
  end;
end;



procedure TSortStringList.NumberSort(Flag: Boolean);
begin
  if Flag then
    CustomSort(NumberSort_1)
  else
    CustomSort(NumberSort_2);
end;

procedure TSortStringList.DateTimeSort(Flag: Boolean);
begin
  if Flag then
    CustomSort(DateTimeSort_1)
  else
    CustomSort(DateTimeSort_2);
end;

procedure TSortStringList.StringSort(Flag: Boolean);
begin
  if Flag then
    CustomSort(StrSort_1)
  else
    CustomSort(StrSort_2);
end;

{ TValueList }

destructor TValueList.Destroy;
begin
  FOnChange := nil;
  FOnChanging := nil;
  inherited Destroy;
  if FCount <> 0 then Finalize(FList^[0], FCount);
  FCount := 0;
  SetCapacity(0);
end;

function TValueList.Add(const AName, S: string): Integer;
begin
  Result := AddObject(AName, S, nil);
end;

function TValueList.AddObject(const AName, S: string; AObject: TObject): Integer;
begin
  if not Sorted then
    Result := FCount
  else if Find(S, Result) then
    case Duplicates of
      dupIgnore: Exit;
      dupError: Error(@SDuplicateString, 0);
    end;
  InsertItem(Result, AName, S, AObject);
end;

procedure TValueList.Changed;
begin
  if (UpdateCount = 0) and Assigned(FOnChange) then
    FOnChange(Self);
end;

procedure TValueList.Changing;
begin
  if (UpdateCount = 0) and Assigned(FOnChanging) then
    FOnChanging(Self);
end;

procedure TValueList.Clear;
begin
  if FCount <> 0 then
  begin
    Changing;
    Finalize(FList^[0], FCount);
    FCount := 0;
    SetCapacity(0);
    Changed;
  end;
end;

procedure TValueList.Delete(Index: Integer);
begin
  if (Index < 0) or (Index >= FCount) then Error(@SListIndexError, Index);
  Changing;
  Finalize(FList^[Index]);
  Dec(FCount);
  if Index < FCount then
    System.Move(FList^[Index + 1], FList^[Index],
      (FCount - Index) * SizeOf(TValueItem));
  Changed;
end;

procedure TValueList.Exchange(Index1, Index2: Integer);
begin
  if (Index1 < 0) or (Index1 >= FCount) then Error(@SListIndexError, Index1);
  if (Index2 < 0) or (Index2 >= FCount) then Error(@SListIndexError, Index2);
  Changing;
  ExchangeItems(Index1, Index2);
  Changed;
end;

procedure TValueList.ExchangeItems(Index1, Index2: Integer);
var
  Temp: Pointer;
  Item1, Item2: PValueItem;
begin
  Item1 := @FList^[Index1];
  Item2 := @FList^[Index2];

  Temp := Pointer(Item1^.FString);
  Pointer(Item1^.FName) := Pointer(Item2^.FName);
  Pointer(Item2^.FName) := Temp;

  Temp := Pointer(Item1^.FString);
  Pointer(Item1^.FString) := Pointer(Item2^.FString);
  Pointer(Item2^.FString) := Temp;

  Temp := Pointer(Item1^.FObject);
  Pointer(Item1^.FObject) := Pointer(Item2^.FObject);
  Pointer(Item2^.FObject) := Temp;
end;

function TValueList.Find(const S: string; var Index: Integer): Boolean;
var
  L, H, I, C: Integer;
begin
  Result := False;
  L := 0;
  H := FCount - 1;
  while L <= H do
  begin
    I := (L + H) shr 1;
    C := CompareStrings(FList^[I].FName, S);
    if C < 0 then
      L := I + 1
    else
    begin
      H := I - 1;
      if C = 0 then
      begin
        Result := True;
        if Duplicates <> dupAccept then L := I;
      end;
    end;
  end;
  Index := L;
end;

function TValueList.Get(Index: Integer): string;
begin
  if (Index < 0) or (Index >= FCount) then Error(@SListIndexError, Index);
  Result := FList^[Index].FString;
end;

function TValueList.GetCapacity: Integer;
begin
  Result := FCapacity;
end;

function TValueList.GetCount: Integer;
begin
  Result := FCount;
end;

function TValueList.GetObject(Index: Integer): TObject;
begin
  if (Index < 0) or (Index >= FCount) then Error(@SListIndexError, Index);
  Result := FList^[Index].FObject;
end;

function TValueList.GetName(Index: Integer): string;
begin
  if (Index < 0) or (Index >= FCount) then Error(@SListIndexError, Index);
  Result := FList^[Index].FName;
end;

procedure TValueList.Grow;
var
  Delta: Integer;
begin
  if FCapacity > 64 then
    Delta := FCapacity div 4
  else if FCapacity > 8 then
    Delta := 16
  else
    Delta := 4;
  SetCapacity(FCapacity + Delta);
end;

function TValueList.IndexOf(const S: string): Integer;
begin
  if not Sorted then
    Result := inherited IndexOf(S)
  else if not Find(S, Result) then
    Result := -1;
end;

procedure TValueList.Insert(Index: Integer; const AName, S: string);
begin
  InsertObject(Index, AName, S, nil);
end;

procedure TValueList.InsertObject(Index: Integer; const AName, S: string;
  AObject: TObject);
begin
  if Sorted then Error(@SSortedListError, 0);
  if (Index < 0) or (Index > FCount) then Error(@SListIndexError, Index);
  InsertItem(Index, AName, S, AObject);
end;

procedure TValueList.InsertItem(Index: Integer; const AName, S: string; AObject: TObject);
var
  ValueItem: PValueItem;
begin
  Changing;
  if FCount = FCapacity then Grow;
  if Index < FCount then
    System.Move(FList^[Index], FList^[Index + 1],
      (FCount - Index) * SizeOf(TValueItem));

  ValueItem := @FList^[Index];
  begin
    Pointer(ValueItem.FString) := nil;
    Pointer(ValueItem.FName) := nil;
    ValueItem.FObject := AObject;
    ValueItem.FString := S;
    ValueItem.FName := AName;
  end;
  Inc(FCount);
  Changed;
end;

procedure TValueList.Put(Index: Integer; const S: string);
begin
  if Sorted then Error(@SSortedListError, 0);
  if (Index < 0) or (Index >= FCount) then Error(@SListIndexError, Index);
  Changing;
  FList^[Index].FString := S;
  Changed;
end;

procedure TValueList.PutObject(Index: Integer; AObject: TObject);
begin
  if (Index < 0) or (Index >= FCount) then Error(@SListIndexError, Index);
  Changing;
  FList^[Index].FObject := AObject;
  Changed;
end;

procedure TValueList.PutName(Index: Integer; const AName: string);
begin
  if Sorted then Error(@SSortedListError, 0);
  if (Index < 0) or (Index >= FCount) then Error(@SListIndexError, Index);
  Changing;
  FList^[Index].FName := AName;
  Changed;
end;

procedure TValueList.QuickSort(L, R: Integer; SCompare: TValueListSortCompare);
var
  I, J, P: Integer;
begin
  repeat
    I := L;
    J := R;
    P := (L + R) shr 1;
    repeat
      while SCompare(Self, I, P) < 0 do
        Inc(I);
      while SCompare(Self, J, P) > 0 do
        Dec(J);
      if I <= J then
      begin
        ExchangeItems(I, J);
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

procedure TValueList.SetCapacity(NewCapacity: Integer);
begin
  ReallocMem(FList, NewCapacity * SizeOf(TValueItem));
  FCapacity := NewCapacity;
end;

procedure TValueList.SetSorted(Value: Boolean);
begin
  if FSorted <> Value then
  begin
    if Value then Sort;
    FSorted := Value;
  end;
end;

procedure TValueList.SetUpdateState(Updating: Boolean);
begin
  if Updating then
    Changing
  else
    Changed;
end;

function StringListCompareStrings(List: TValueList; Index1, Index2: Integer): Integer;
begin
  Result := List.CompareStrings(List.FList^[Index1].FString,
    List.FList^[Index2].FString);
end;

procedure TValueList.Sort;
begin
  CustomSort(StringListCompareStrings);
end;

procedure TValueList.CustomSort(Compare: TValueListSortCompare);
begin
  if not Sorted and (FCount > 1) then
  begin
    Changing;
    QuickSort(0, FCount - 1, Compare);
    Changed;
  end;
end;

function TValueList.CompareStrings(const S1, S2: string): Integer;
begin
  if CaseSensitive then
    Result := AnsiCompareStr(S1, S2)
  else
    Result := AnsiCompareText(S1, S2);
end;

procedure TValueList.SetCaseSensitive(const Value: Boolean);
begin
  if Value <> FCaseSensitive then
  begin
    FCaseSensitive := Value;
    if Sorted then Sort;
  end;
end;

function TValueList.GetIndex(const AName: string): Integer;
var
  nLow, nHigh, nMed, nCompareVal: Integer;
begin
  Result := -1;
  if Self.Count <> 0 then
  begin
    if Self.Sorted then
    begin
      if Self.Count = 1 then
      begin
        if CompareStr(AName, Self.Names[0]) = 0 then Result := 0;
      end
      else
      begin
        nLow := 0;
        nHigh := Self.Count - 1;
        nMed := (nHigh - nLow) div 2 + nLow;
        while (true) do
        begin
          if (nHigh - nLow) = 1 then
          begin
            if CompareStr(AName, Self.Names[nHigh]) = 0 then Result := nHigh;
            if CompareStr(AName, Self.Names[nLow]) = 0 then Result := nLow;
            break;
          end
          else
          begin
            ;
            nCompareVal := CompareStr(AName, Self.Names[nMed]);
            if nCompareVal > 0 then
            begin
              nLow := nMed;
              nMed := (nHigh - nLow) div 2 + nLow;
              Continue;
            end;
            if nCompareVal < 0 then
            begin
              nHigh := nMed;
              nMed := (nHigh - nLow) div 2 + nLow;
              Continue;
            end;
            Result := nMed;
            break;
          end;
        end;
      end;
    end
    else
    begin
      if Self.Count = 1 then
      begin
        if CompareText(AName, Self.Names[0]) = 0 then Result := 0;
      end
      else
      begin

        nLow := 0;
        nHigh := Self.Count - 1;
        nMed := (nHigh - nLow) div 2 + nLow;
        while (true) do
        begin
          if (nHigh - nLow) = 1 then
          begin
            if CompareText(AName, Self.Names[nHigh]) = 0 then Result := nHigh;
            if CompareText(AName, Self.Names[nLow]) = 0 then Result := nLow;
            break;
          end
          else
          begin
            nCompareVal := CompareText(AName, Self.Names[nMed]);
            if nCompareVal > 0 then
            begin
              nLow := nMed;
              nMed := (nHigh - nLow) div 2 + nLow;
              Continue;
            end;
            if nCompareVal < 0 then
            begin
              nHigh := nMed;
              nMed := (nHigh - nLow) div 2 + nLow;
              Continue;
            end;
            Result := nMed;
            break;
          end;
        end;
      end;
    end;
  end;
end;

procedure TValueList.SortString(nMin, nMax: Integer);
var
  ntMin, ntMax: Integer;
  s18: string;
begin
  if Self.Count > 0 then
    while (True) do
    begin
      ntMin := nMin;
      ntMax := nMax;
      s18 := Self.Names[(nMin + nMax) shr 1];
      while (True) do
      begin
        while (CompareText(Self.Names[ntMin], s18) < 0) do
          Inc(ntMin);
        while (CompareText(Self.Names[ntMax], s18) > 0) do
          Dec(ntMax);
        if ntMin <= ntMax then
        begin
          Self.Exchange(ntMin, ntMax);
          Inc(ntMin);
          Dec(ntMax);
        end;
        if ntMin > ntMax then
          break
      end;
      if nMin < ntMax then SortString(nMin, ntMax);
      nMin := ntMin;
      if ntMin >= nMax then break;
    end;
end;

function TValueList.AddRecord(const AName: string; const Value: Integer): Boolean;
begin
  Result := AddRecord(AName, '', Value);
end;


function TValueList.AddRecord(const AName, S: string): Boolean;
begin
  Result := AddRecord(AName, S, 0);
end;

function TValueList.AddRecord(const AName, S: string; const Value: Integer): Boolean;
var
  nLow, nHigh, nMed, nCompareVal: Integer;
begin
  Result := True;
  if Self.Count = 0 then
  begin
    Self.AddObject(AName, S, TObject(Value));
  end
  else
  begin
    if Self.Sorted then
    begin
      if Self.Count = 1 then
      begin
        nMed := CompareStr(AName, Self.Names[0]);
        if nMed > 0 then
          Self.AddObject(AName, S, TObject(Value))
        else
        begin
          if nMed < 0 then Self.InsertObject(0, AName, S, TObject(Value));
        end;
      end
      else
      begin
        nLow := 0;
        nHigh := Self.Count - 1;
        nMed := (nHigh - nLow) div 2 + nLow;
        while (true) do
        begin
          if (nHigh - nLow) = 1 then
          begin
            nMed := CompareStr(AName, Self.Names[nHigh]);
            if nMed > 0 then
            begin
              Self.InsertObject(nHigh + 1, AName, S, TObject(Value));
              break;
            end
            else
            begin
              nMed := CompareStr(AName, Self.Names[nLow]);
              if nMed > 0 then
              begin
                Self.InsertObject(nLow + 1, AName, S, TObject(Value));
                break;
              end
              else
              begin
                if nMed < 0 then
                begin
                  Self.InsertObject(nLow, AName, S, TObject(Value));
                  break; ;
                end
                else
                begin
                  Result := False;
                  break;
                end;
              end;
            end;
          end
          else
          begin
            nCompareVal := CompareStr(AName, Self.Names[nMed]);
            if nCompareVal > 0 then
            begin
              nLow := nMed;
              nMed := (nHigh - nLow) div 2 + nLow;
              Continue;
            end;
            if nCompareVal < 0 then
            begin
              nHigh := nMed;
              nMed := (nHigh - nLow) div 2 + nLow;
              Continue;
            end;
            Result := False;
            break;
          end;
        end;
      end;
    end
    else
    begin
      if Self.Count = 1 then
      begin
        nMed := CompareText(AName, Self.Names[0]);
        if nMed > 0 then
          Self.AddObject(AName, S, TObject(Value))
        else
        begin
          if nMed < 0 then Self.InsertObject(0, AName, S, TObject(Value));
        end;
      end
      else
      begin
        nLow := 0;
        nHigh := Self.Count - 1;
        nMed := (nHigh - nLow) div 2 + nLow;
        while (true) do
        begin
          if (nHigh - nLow) = 1 then
          begin
            nMed := CompareText(AName, Self.Names[nHigh]);
            if nMed > 0 then
            begin
              Self.InsertObject(nHigh + 1, AName, S, TObject(Value));
              break;
            end
            else
            begin
              nMed := CompareText(AName, Self.Names[nLow]);
              if nMed > 0 then
              begin
                Self.InsertObject(nLow + 1, AName, S, TObject(Value));
                break;
              end
              else
              begin
                if nMed < 0 then
                begin
                  Self.InsertObject(nLow, AName, S, TObject(Value));
                  break;
                end
                else
                begin
                  Result := False;
                  break;
                end;
              end;
            end;
          end
          else
          begin
            nCompareVal := CompareText(AName, Self.Names[nMed]);
            if nCompareVal > 0 then
            begin
              nLow := nMed;
              nMed := (nHigh - nLow) div 2 + nLow;
              Continue;
            end;
            if nCompareVal < 0 then
            begin
              nHigh := nMed;
              nMed := (nHigh - nLow) div 2 + nLow;
              Continue;
            end;
            Result := False;
            break;
          end;
        end;
      end;
    end;
  end;
end;
{ TGList }

constructor TGList.Create;
begin
  inherited;
  InitializeCriticalSection(CriticalSection);
end;

destructor TGList.Destroy;
begin
  DeleteCriticalSection(CriticalSection);
  inherited;
end;

function TGList.TryLock: Boolean;
begin
  Result := TryEnterCriticalSection(CriticalSection);
end;

procedure TGList.Lock;
begin
  EnterCriticalSection(CriticalSection);
end;

procedure TGList.UnLock;
begin
  LeaveCriticalSection(CriticalSection);
end;

procedure TGList.Up(Item: Pointer);
var
  nIndex: Integer;
begin
  if Count <= 1 then Exit;
  nIndex := IndexOf(Item);
  if nIndex >= 0 then
  begin
    if (nIndex - 1 >= 0) and (nIndex - 1 < Count) then
    begin
      Delete(nIndex);
      Dec(nIndex);
      Insert(nIndex, Item);
    end;
  end;
end;

procedure TGList.Down(Item: Pointer);
var
  nIndex: Integer;
begin
  if Count <= 1 then Exit;
  nIndex := IndexOf(Item);
  if nIndex >= 0 then
  begin
    if (nIndex + 1 >= 0) and (nIndex + 1 < Count) then
    begin
      Delete(nIndex);
      Inc(nIndex);
      Insert(nIndex, Item);
    end;
  end;
end;
{ TGStringList }

constructor TGStringList.Create;
begin
  inherited;
  InitializeCriticalSection(CriticalSection);
end;

destructor TGStringList.Destroy;
begin
  DeleteCriticalSection(CriticalSection);
  inherited;
end;

function TGStringList.TryLock: Boolean;
begin
  Result := TryEnterCriticalSection(CriticalSection);
end;

procedure TGStringList.Lock;
begin
  EnterCriticalSection(CriticalSection);
end;

procedure TGStringList.UnLock;
begin
  LeaveCriticalSection(CriticalSection);
end;

procedure TGStringList.Up(AObject: TObject);
var
  nIndex: Integer;
  s: string;
begin
  if Count <= 1 then Exit;
  nIndex := IndexOfObject(AObject);
  if nIndex >= 0 then begin
    if (nIndex - 1 >= 0) and (nIndex - 1 < Count) then
    begin
      s := Strings[nIndex];
      Delete(nIndex);
      Dec(nIndex);
      InsertObject(nIndex, s, AObject);
    end;
  end;
end;

procedure TGStringList.Down(AObject: TObject);
var
  nIndex: Integer;
  s: string;
begin
  if Count <= 1 then Exit;
  nIndex := IndexOfObject(AObject);
  if nIndex >= 0 then
  begin
    if (nIndex + 1 >= 0) and (nIndex + 1 < Count) then
    begin
      s := Strings[nIndex];
      Delete(nIndex);
      Inc(nIndex);
      InsertObject(nIndex, s, AObject);
    end;
  end;
end;

end.
