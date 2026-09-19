unit ItemDropLimit;

interface

uses
  Windows, Classes, SysUtils, Grobal2, FastIniFile, HUtil32, Envir, DateUtils,
  System.Types;

type
  TIntervalType = (itDay, itHour, itMinute);
  PPDropItemRule = ^PDropItemRule;
  PDropItemRule = ^TDropItemRule;
  TDropItemRule = record
    MapName: string[MAP_NAME_LEN];
    ClearInterval: Integer;
    DropInterval: Integer;
    IntervalType: TIntervalType;
    LimitCount: Integer;
    DropedCount: Integer;
    AllDropedCount: Integer;
    LastClearDate: TDateTime;
    LastDropTime: TDateTime;
  end;

  TDropLimitManager = class;

  PDropLimitItem = ^TDropLimitItem;
  TDropLimitItem = class(TObject)
  private
    FOwner: TDropLimitManager;
    FIsChanged: Boolean;
    FIsRecordLog: Boolean;
    FName: string;
    FItemRules: TList;
    function GetCount: Integer;
    function GetItemRule(Index: Integer): PDropItemRule;
  public
    constructor Create(AOwner: TDropLimitManager; AName: string);
    destructor Destroy; override;

    property Name: string read FName;
    property IsChanged: Boolean read FIsChanged write FIsChanged;
    property IsRecordLog: Boolean read FIsRecordLog write FIsRecordLog;

    procedure Clear;
    function Add(ItemRule: TDropItemRule): PDropItemRule;
    function Remove(ItemRule: PDropItemRule): Boolean;
    property Count: Integer read GetCount;
    property Rules[Index: Integer]: PDropItemRule read GetItemRule;

    procedure Save;
    procedure Load;
  end;

  TDropLimitManager = class(TObject)
  private
    FIniFile: TFastIniFile;
    FItems: TList;
    FSortItems: TList;
    FLogDir: string;
    FCriticalSection: TRTLCriticalSection;
    function GetCount: Integer;
    function GetItems(Index: Integer): TDropLimitItem;

    procedure Lock;
    procedure UnLock;
  public
    constructor Create;
    destructor Destroy; override;

    procedure Clear;
    function AddItem(ItemName: string): TDropLimitItem;
    function Remove(Item: TDropLimitItem): Boolean;
    property Count: Integer read GetCount;
    property Items[Index: Integer]: TDropLimitItem read GetItems;

    function Search(ItemName: string; var Index: Integer): Boolean;
    function CanDropItem(Envir: TEnvirnoment; ItemName: string): Boolean;
    function DropItem(Envir: TEnvirnoment; ItemName: string; DropMonName, ItemCreateName: string; DropPoint: TPoint): Boolean;

    procedure LoadConfig;
  end;

const
  TIntervalTypeNames: array[TIntervalType] of string = ('Ìì', 'Ê±', '·Ö');

implementation

uses
  M2Share;

{ TDropLimitItem }

constructor TDropLimitItem.Create(AOwner: TDropLimitManager; AName: string);
begin
  FOwner := AOwner;
  FName := AName;
  FItemRules := TList.Create;
  FIsChanged := False;
end;

destructor TDropLimitItem.Destroy;
begin
  Clear;
  FItemRules.Free;
  inherited;
end;

function TDropLimitItem.Add(ItemRule: TDropItemRule): PDropItemRule;
begin
  New(Result);
  FItemRules.Add(Result);
  Result^:= ItemRule;
end;

procedure TDropLimitItem.Clear;
var
  I: Integer;
begin
  for I := 0 to FItemRules.Count - 1 do
  begin
    Dispose(PDropItemRule(FItemRules.Items[I]));
  end;
  FItemRules.Clear;
end;

function TDropLimitItem.GetCount: Integer;
begin
  Result := FItemRules.Count;
end;

function TDropLimitItem.GetItemRule(Index: Integer): PDropItemRule;
begin
  if (Index >= 0) and (Index < FItemRules.Count) then
    Result := FItemRules.Items[Index]
  else
    Result := nil;
end;

function Date2MyDate(Dt: TDateTime): Integer;
var
  Y, M, D: Word;
begin
  DecodeDate(Dt, Y, M, D);
  Result := Y * 10000 + M * 100 + D;
end;

function MyDate2Date(dt: Integer): TDateTime;
var
  Y, M, D: Word;
begin
  Result := 0;
  if dt > 10000000 then
  begin
    Y := dt div 10000;
    M := (dt - Y * 10000) div 100;
    D := dt mod 100;
    TryEncodeDate(Y, M, D, Result);
  end;
end;

function Hour2MyHour(Dt: TDateTime): Integer;
var
  Hour, Min, Sec, MSec: Word;
begin
  DecodeTime(Dt, Hour, Min, Sec, MSec);
  Result := Hour * 10000 + Min * 100 + Sec;
end;

function MyHour2Hour(dt: Integer): TDateTime;
var
  Hour, Min, Sec: Word;
begin
  Result := 0;
  if dt > 0 then
  begin
    Hour := dt div 10000;
    Min := (dt - Hour * 10000) div 100;
    Sec := dt mod 100;
    TryEncodeTime(Hour, Min, Sec, 0, Result);
  end;
end;

procedure TDropLimitItem.Save;
var
  I: Integer;
  PItemRule: PDropItemRule;
  StrValue: string;
begin
  if g_nKey_DropLimitExt = 0 then Exit;

  try
    FOwner.FIniFile.ClearSection(FName);
    FOwner.FIniFile.WriteBoolean(FName, 'RecordLog', FIsRecordLog);
    for I := 0 to FItemRules.Count - 1 do
    begin
      PItemRule := FItemRules.Items[I];
      {
      MapName: string[MAP_NAME_LEN];
      ClearInterval: Integer;
      LimitCount: Integer;
      DropedCount: Integer;
      LastClearDate: TDateTime;
      }

      StrValue := PItemRule.MapName + #9 +
        IntToStr(PItemRule.ClearInterval) + #9 +
        IntToStr(PItemRule.LimitCount) + #9 +
        IntToStr(PItemRule.DropedCount) + #9 +
        IntToStr(Date2MyDate(PItemRule.LastClearDate)) + #9 +
        IntToStr(Hour2MyHour(PItemRule.LastClearDate)) + #9 +
        IntToStr(PItemRule.DropInterval) + #9 +
        IntToStr(Integer(PItemRule.IntervalType)) + #9 +
        IntToStr(PItemRule.AllDropedCount) + #9 +
        FloatToStr(PItemRule.LastDropTime);

      FOwner.FIniFile.WriteString(FName, IntToStr(I), StrValue);
    end;

    FOwner.FIniFile.UpdateFile;
  except
  end;
end;

procedure TDropLimitItem.Load;
var
  I, nTemp: Integer;
  S, S1, S2, S3, S4, S5, S6, S7, S8, S9, S10: string;
  ItemRule: TDropItemRule;
begin
  if g_nKey_DropLimitExt = 0 then Exit;

  Clear;
  FIsRecordLog := FOwner.FIniFile.ReadBoolean(FName, 'RecordLog', False);

  for I := 0 to 1000 do
  begin
    S := FOwner.FIniFile.ReadString(FName, IntToStr(I), '');
    if Length(S) > 0 then
    begin
      S := GetValidStr3_Ex(S, S1, #9);
      S := GetValidStr3_Ex(S, S2, #9);
      S := GetValidStr3_Ex(S, S3, #9);
      S := GetValidStr3_Ex(S, S4, #9);
      S := GetValidStr3_Ex(S, S5, #9);
      S := GetValidStr3_Ex(S, S6, #9);
      S := GetValidStr3_Ex(S, S7, #9);
      S := GetValidStr3_Ex(S, S8, #9);
      S := GetValidStr3_Ex(S, S9, #9);
      S := GetValidStr3_Ex(S, S10, #9);

      if Length(S1) > 0 then
      begin
        ItemRule.MapName := S1;
        ItemRule.ClearInterval := StrToIntDef(S2, 0);
        ItemRule.LimitCount := StrToIntDef(S3, 0);
        ItemRule.DropedCount := StrToIntDef(S4, 0);
        ItemRule.LastClearDate := MyDate2Date(StrToIntDef(S5, 0)) + MyHour2Hour(StrToIntDef(S6, 0));
        ItemRule.DropInterval := StrToIntDef(S7, 0);
        nTemp := StrToIntDef(S8, 0);
        if (nTemp >= Integer(Low(TIntervalType))) and (nTemp <= Integer(High(TIntervalType))) then
          ItemRule.IntervalType := TIntervalType(nTemp)
        else
          ItemRule.IntervalType := itDay;

        ItemRule.AllDropedCount := StrToIntDef(S9, 0);
        ItemRule.LastDropTime := StrToFloatDef(S10, 0);
          
        Add(ItemRule);
      end;
    end
    else
    begin
      Break;
    end;
  end;
end;

function TDropLimitItem.Remove(ItemRule: PDropItemRule): Boolean;
var
  Index: Integer;
begin
  Result := False;
  Index := FItemRules.IndexOf(ItemRule);
  if Index >= 0 then
  begin
    Dispose(PDropItemRule(FItemRules.Items[Index]));
    FItemRules.Delete(Index);
    Result := True;

    Save;
  end;
end;

{ TDropLimitManager }

constructor TDropLimitManager.Create;
begin
  FItems := TList.Create;
  FSortItems := TList.Create;

  InitializeCriticalSection(FCriticalSection);

  if not DirectoryExists(g_Config.sItemDropLimit) then
    ForceDirectories(g_Config.sItemDropLimit);

  FLogDir := g_Config.sItemDropLogDir;
  if not DirectoryExists(FLogDir) then
  begin
    CreateDirectory(PChar(FLogDir), nil);
  end;
end;

destructor TDropLimitManager.Destroy;
begin
  Clear;
  FItems.Free;
  FSortItems.Free;

  if FIniFile <> nil then
  begin
    FIniFile.Free;
  end;

  DeleteCriticalSection(FCriticalSection);
  
  inherited;
end;

function TDropLimitManager.AddItem(ItemName: string): TDropLimitItem;
var
  Index: Integer;
begin
  Result := nil;
  if g_nKey_DropLimitExt = 0 then Exit;

  if not Search(ItemName, Index) then
  begin
    Result := TDropLimitItem.Create(Self, ItemName);
    FItems.Add(Result);

    FSortItems.Insert(Index, Result);
  end
  else
  begin
    Result := FSortItems.Items[Index];
  end;
end;

function TDropLimitManager.CanDropItem(Envir: TEnvirnoment; ItemName: string): Boolean;
var
  I, Index: Integer;
  Item: TDropLimitItem;
  Rule: PDropItemRule;
  sMapName: string;
begin
  if (Envir.m_boMirror) or (Envir.m_boFB) then
    sMapName := Envir.sMainMapName
  else
    sMapName := Envir.sMapName;

  if not Search(ItemName, Index) then
    Result := True
  else
  begin
    Item := FSortItems.Items[Index];

    for I := 0 to Item.Count - 1 do
    begin
      Rule := Item.Rules[I];
      if SameText(Rule.MapName, sMapName) or (Rule.MapName = '*') then
      begin
        if Rule.LimitCount <= Rule.DropedCount then
        begin
          Result := False;
          Exit;
        end;
      end;
    end;

    Result := True;
  end;
end;

procedure TDropLimitManager.Clear;
var
  I: Integer;
begin
  for I := 0 to FItems.Count - 1 do
  begin
    TDropLimitItem(FItems.Items[I]).Free;
  end;
  FItems.Clear;
  FSortItems.Clear;
end;

function TDropLimitManager.DropItem(Envir: TEnvirnoment; ItemName: string; DropMonName, ItemCreateName: string; DropPoint: TPoint): Boolean;
var
  I, Index: Integer;
  Item: TDropLimitItem;
  Rule: PDropItemRule;
  sMapName: string;
  _sLogFileName: string;
  LogFile: TextFile;
  IsChange: Boolean;
begin
  if g_nKey_DropLimitExt = 0 then
  begin
    Result := True;
    Exit;
  end;

  if not Search(ItemName, Index) then
  begin
    Result := True;
    Exit;
  end;

  if Envir = nil then
  begin
    Result := True;
    Exit;
  end;

  if (Envir.m_boMirror) or (Envir.m_boFB) then
    sMapName := Envir.sMainMapName
  else
    sMapName := Envir.sMapName;

  Item := FSortItems.Items[Index];

  if Item = nil then
  begin
    Result := True;
    Exit;
  end;

  IsChange := False;
  Lock;
  try
    for I := 0 to Item.Count - 1 do
    begin
      Rule := Item.Rules[I];
      if Rule <> nil then
      begin
        if SameText(Rule.MapName, sMapName) or (Rule.MapName = '*') then
        begin
          if Rule.LimitCount <= Rule.DropedCount then
          begin
            Result := False;
            Exit;
          end;

          if (Rule.DropInterval > 0) and (MinutesBetween(Now, Rule.LastDropTime) < Rule.DropInterval) then
          begin
            Result := False;
            Exit;
          end;
        end;
      end;
    end;

    for I := 0 to Item.Count - 1 do
    begin
      Rule := Item.Rules[I];
      if Rule <> nil then
      begin
        if SameText(Rule.MapName, sMapName) or (Rule.MapName = '*') then
        begin
          Rule.LastDropTime := Now;
          Inc(Rule.DropedCount);
          Inc(Rule.AllDropedCount);
          IsChange := True;
        end;
      end;
    end;

    if IsChange then
    begin
      Item.Save;
    end;
  finally
    UnLock;
  end;

  if Item.FIsRecordLog and (Item.Count > 0) then
  begin
    _sLogFileName := FLogDir + Item.Name + '.txt';
    try
      if not FileExists(_sLogFileName) then
      begin
        AssignFile(LogFile, _sLogFileName);
        Rewrite(LogFile);
      end
      else
      begin
        AssignFile(LogFile, _sLogFileName);
        Append(LogFile);
      end;

      if (Envir.m_boMirror) then
        sMapName := sMapName + '(Mirror)'
      else if (Envir.m_boFB) then
        sMapName := sMapName + '(FB)';

      Writeln(LogFile, FormatDateTime('yyyy/mm/dd hh:nn:ss', Now) + #9 + ItemCreateName + #9 + DropMonName + #9 + sMapName + #9 + IntToStr(DropPoint.X) + #9 + IntToStr(DropPoint.Y));
      CloseFile(LogFile);
    except
    end;
  end;

  Result := True;
end;

function TDropLimitManager.GetCount: Integer;
begin
  Result := FItems.Count;
end;

function TDropLimitManager.GetItems(Index: Integer): TDropLimitItem;
begin
  if (Index >= 0) and (Index < FItems.Count) then
    Result := FItems.Items[Index]
  else
    Result := nil;
end;

function TDropLimitManager.Search(ItemName: string;
  var Index: Integer): Boolean;
var
  L, H, C, I: Integer;
  Item: TDropLimitItem;
begin
  Result := False;

  L := 0;
  H := FSortItems.Count - 1;
  while L <= H do
  begin
    I := (L + H) shr 1;
    Item := FSortItems.Items[I];
    C := AnsiCompareText(Item.FName, ItemName);
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

function TDropLimitManager.Remove(Item: TDropLimitItem): Boolean;
var
  Index: Integer;
  ItemName: string;
begin
  Result := False;
  if g_nKey_DropLimitExt = 0 then Exit;

  Index := FItems.IndexOf(Item);
  if Index >= 0 then
  begin
    ItemName := Item.FName;

    FItems.Delete(Index);
    FSortItems.Remove(Item);

    Item.Free;

    Result := True;

    FIniFile.ClearSection(ItemName);
    FIniFile.EraseSection(ItemName);
    FIniFile.UpdateFile;
  end;
end;

procedure TDropLimitManager.LoadConfig;
var
  I: Integer;
  LimitItem: TDropLimitItem;
  Sections: TStringList;
begin
  if g_nKey_DropLimitExt = 0 then Exit;

  if FIniFile <> nil then
  begin
    FIniFile.Free;
    FIniFile := nil;
  end;

  FIniFile := TFastIniFile.Create(g_Config.sItemDropLimit + 'DropLimitConfig.ini');

  Clear;
  Sections := TStringList.Create;
  try
    FIniFile.ReadSections(Sections);

    for I := 0 to Sections.Count - 1 do
    begin
      LimitItem := AddItem(Sections.Strings[I]);
      LimitItem.Load;
    end;
  finally
    Sections.Free;
  end;
end;

procedure TDropLimitManager.Lock;
begin
  EnterCriticalSection(FCriticalSection);
end;

procedure TDropLimitManager.UnLock;
begin
  LeaveCriticalSection(FCriticalSection);
end;

end.
