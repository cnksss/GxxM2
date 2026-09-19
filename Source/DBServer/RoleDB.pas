unit RoleDB;

interface

uses
  Windows, SysUtils, Classes, Grobal2;

const
  NO_ID = -1;

type
  // 搜索匹配模式：完整匹配，模糊匹配
  TSearchMatchType = (smtComplete, smtFuzzy);

type
  // 查询人物，主要用于登录时的角色网关显示角色
  PTQueryHumanData = ^TQueryHumanData;
  TQueryHumanData = record
    HumanName: string;
    IsSelect: Boolean;
    Sex: Integer;
    Job: Integer;
    Hair: Integer;
    Level: LongWord;
  end;

  // 搜索角色，主要用于DBServer的数据管理
  PTSerarchRoleData = ^TSerarchRoleData;
  TSerarchRoleData = record
    Account: string;
    RoleName: string;
    IsDelete: Integer;
    IsHero: Boolean;
    Sex: Integer;
    Job: Integer;
    Level: LongWord;
  end;

  PTRoleRankData = ^TRoleRankData;
  TRoleRankData = record
    RankIndex: Integer;
    HumanName: string;
    HeroName: string;

    case Integer of
      0:
        (Level: LongWord);
      1:
        (MasterCount: LongWord);
  end;

  THumanList = class(TObject)
  private
    FList: TList;
    function GetCount: Integer;
    function GetItems(Index: Integer): PTHumData;
  public
    constructor Create;
    destructor Destroy; override;

    property Count: Integer read GetCount;
    property Items[Index: Integer]: PTHumData read GetItems; default;
    function Add(HumData: PTHumData): PTHumData;
    procedure Clear;
    procedure SetCapacity(Value: Integer);
  end;

  TQueryHumanList = class(TObject)
  private
    FList: TList;
    function GetCount: Integer;
    function GetItems(Index: Integer): PTQueryHumanData;
  public
    constructor Create;
    destructor Destroy; override;

    property Count: Integer read GetCount;
    property Items[Index: Integer]: PTQueryHumanData read GetItems; default;
    function Add(QueryHumanData: PTQueryHumanData): PTQueryHumanData;
    procedure Clear;
    procedure SetCapacity(Value: Integer);
  end;

  TSerarchRoleList = class(TObject)
  private
    FList: TList;
    function GetCount: Integer;
    function GetItems(Index: Integer): PTSerarchRoleData;
  public
    constructor Create;
    destructor Destroy; override;

    property Count: Integer read GetCount;
    property Items[Index: Integer]: PTSerarchRoleData read GetItems; default;
    function Add(SerarchRoleData: PTSerarchRoleData): PTSerarchRoleData;
    procedure Clear;
    procedure SetCapacity(Value: Integer);
  end;

  TRoleRankList = class(TObject)
  private
    FList: TList;
    function GetCount: Integer;
    function GetItems(Index: Integer): PTRoleRankData;
  public
    constructor Create;
    destructor Destroy; override;

    property Count: Integer read GetCount;
    property Items[Index: Integer]: PTRoleRankData read GetItems; default;
    function Add(RoleRankData: PTRoleRankData): PTRoleRankData;
    procedure Clear;
    procedure SetCapacity(Value: Integer);
  end;

  TRoleDB = class;
  THumanDB = class(TObject)
  private
    FOwner: TRoleDB;
  protected
    procedure DoInit; virtual; abstract;
    procedure DoFinal; virtual; abstract;

    function DoGetID(HumanName: string): Integer; virtual; abstract;
    function DoCheckHumanExists(Account, HumanName: string): Boolean; virtual; abstract;

    function DoGetHumanCount(Account: string): Integer; virtual; abstract;
    function DoGetOtherHumanName(Account, HumanName: string): string; virtual; abstract;
    function DoGetHumanHeroName(Account, HumanName: string; var HeroName, DeputyHeroName: string): Boolean; virtual; abstract;

    function DoGetBaseInfo(HumanName: string; var Sex, Job, Level, LastLogin: Integer): Boolean; virtual; abstract;

    function DoQueryHumans(Account: string; HumanList: TQueryHumanList): Integer; virtual; abstract;
    function DoQueryDeleteHumans(Account: string; HumanList: TQueryHumanList): Integer; virtual; abstract;

    function DoSearchByAccount(Account: string; MatchType: TSearchMatchType; RoleList: TSerarchRoleList): Integer; virtual; abstract;
    function DoSearchByName(HumanName: string; MatchType: TSearchMatchType; RoleList: TSerarchRoleList): Integer; virtual; abstract;
    function DoSearchByLevel(LimitCount, MinLevel: Integer; RoleList: TSerarchRoleList): Integer; virtual; abstract;

    function DoGetMobileNumbers(OnlyBindMobile: Boolean; MobileNumberList: TStrings): Integer; virtual; abstract;

    function DoSelect(Account, HumanName: string): Boolean; virtual; abstract;
    function DoGet(Account, HumanName: string; var HumData: THumData; var HumanID: Integer): Boolean; virtual; abstract;
    function DoAdd(Account, HumanName: string; IsSelect: Boolean; Sex, Job, Hair: Byte): Boolean; virtual; abstract;

    function DoDelete(Account, HumanName: string): Boolean; virtual; abstract;
    function DoDeleteRestore(Account, HumanName: string): Boolean; virtual; abstract;
    function DoSetEnabled(Account, HumanName: string; Enabled: Integer): Boolean; virtual; abstract;

    function DoErase(Account, HumanName: string): Boolean; virtual; abstract;
    function DoRecordLoginTime(Account, HumanName: string): Boolean; virtual; abstract;

    function DoSave(HumanID: Integer; HumData: PTHumData): Boolean; virtual; abstract;
    function DoRename(Account, HumanName: string; HumanID: Integer; NewName: string): Boolean; virtual; abstract;

    function DoChangedGold(HumanName: string; ChangeType: TDBChangeGoldType; ChangedValue: Integer; var ResultValue: LongWord): Boolean; virtual; abstract;
                                                  
    function DoChangedCustomMoney(HumanID: Integer; CustomMoneyName: string; ChangedValue: Integer; var ResultValue: LongWord): Boolean; virtual; abstract;
    
    procedure DoGetRankData(MinLevel, MaxLevel, TopCount: LongWord;
      HumanRankList, WarriorRankList, WizardRankList, TaoistRankList, MasterRankList: TRoleRankList); virtual; abstract;

    function DoBuyPlayer(const sSellAccount, sSellHumanName, sBuyAccount, sBuyHumanName: string): Boolean; virtual; abstract;
  public
    constructor Create(AOwner: TRoleDB); virtual;
    destructor Destroy; override;
    procedure BeforeDestruction; override;

    property Owner: TRoleDB read FOwner;

    procedure Init;
    procedure Fainal;

    function GetID(HumanName: string): Integer;
    function CheckHumanExists(Account, HumanName: string): Boolean;

    function GetHumanCount(Account: string): Integer;
    function GetOtherHumanName(Account, HumanName: string): string;
    function GetHumanHeroName(Account, HumanName: string; var HeroName, DeputyHeroName: string): Boolean;

    function GetBaseInfo(HumanName: string; var Sex, Job, Level, LastLogin: Integer): Boolean;

    function QueryHumans(Account: string; HumanList: TQueryHumanList): Integer;
    function QueryDeleteHumans(Account: string; HumanList: TQueryHumanList): Integer;

    function SearchByAccount(Account: string; MatchType: TSearchMatchType; RoleList: TSerarchRoleList): Integer;
    function SearchByName(HumanName: string; MatchType: TSearchMatchType; RoleList: TSerarchRoleList): Integer;
    function SearchByLevel(LimitCount, MinLevel: Integer; RoleList: TSerarchRoleList): Integer;

    function GetMobileNumbers(OnlyBindMobile: Boolean; MobileNumberList: TStrings): Integer;

    function Select(Account, HumanName: string): Boolean;
    function Get(Account, HumanName: string; var HumData: THumData; var HumanID: Integer): Boolean;
    function Add(Account, HumanName: string; IsSelect: Boolean; Sex, Job, Hair: Byte): Boolean;
    function Delete(Account, HumanName: string): Boolean;
    function DeleteRestore(Account, HumanName: string): Boolean;
    function SetEnabled(Account, HumanName: string; Enabled: Integer): Boolean;
    function Erase(Account, HumanName: string): Boolean;
    function RecordLoginTime(Account, HumanName: string): Boolean;
    function Save(HumanID: Integer; HumData: PTHumData): Boolean;
    function Rename(Account, HumanName: string; HumanID: Integer; NewName: string): Boolean;

    function ChangedGold(HumanName: string; ChangeType: TDBChangeGoldType; ChangedValue: Integer; CustomMoneyName: string; var ResultValue: LongWord): Boolean;

    procedure GetRankData(MinLevel, MaxLevel, TopCount: LongWord;
      HumanRankList, WarriorRankList, WizardRankList, TaoistRankList, MasterRankList: TRoleRankList);

    function BuyPlayer(const sSellAccount, sSellHumanName, sBuyAccount, sBuyHumanName: string): Boolean;
  end;

  THeroDB = class(TObject)
  private
    FOwner: TRoleDB;
  protected
    procedure DoInit; virtual; abstract;
    procedure DoFinal; virtual; abstract;

    function DoGetID(HeroName: string): Integer; virtual; abstract;
    function DoGetHumanInfo(HeroName: string; var HumanID: Integer; var HumanName: string): Boolean; virtual; abstract;

    function DoSearchByAccount(Account: string; MatchType: TSearchMatchType; RoleList: TSerarchRoleList): Integer; virtual; abstract;
    function DoSearchByName(HeroName: string; MatchType: TSearchMatchType; RoleList: TSerarchRoleList): Integer; virtual; abstract;

    function DoGet(HeroName: string; var HeroData: THeroData; var HeroID: Integer): Boolean; virtual; abstract;
    function DoAdd(Account, HumanName: string; HumanID: Integer; HeroName: string; Sex, Job, Hair: Byte; IsDeputyHero: Boolean): Boolean; virtual; abstract;
    //function DoDelete(HeroName: string): Boolean; virtual; abstract;
    //function DoDeleteRestore(HeroName: string): Boolean; virtual; abstract;
    function DoErase(HeroName: string): Boolean; virtual; abstract;
    function DoSave(HeroID: Integer; HeroData: PTHeroData): Boolean; virtual; abstract;
    function DoRename(HeroID: Integer; HeroName, NewName: string): Boolean; virtual; abstract;
    function DoAssess(HeroID: Integer; HeroName, DeputyHeroName: string): Boolean; virtual; abstract;

    procedure DoGetRankData(MinLevel, MaxLevel, TopCount: LongWord;
      HeroRankList, WarriorRankList, WizardRankList, TaoistRankList: TRoleRankList); virtual; abstract;
  public
    constructor Create(AOwner: TRoleDB); virtual;
    destructor Destroy; override;
    procedure BeforeDestruction; override;

    property Owner: TRoleDB read FOwner;

    procedure Init;
    procedure Fainal;

    function GetID(HeroName: string): Integer;

    function SearchByAccount(Account: string; MatchType: TSearchMatchType; RoleList: TSerarchRoleList): Integer;
    function SearchByName(HeroName: string; MatchType: TSearchMatchType; RoleList: TSerarchRoleList): Integer;

    function Get(HeroName: string; var HeroData: THeroData; var HeroID: Integer): Boolean;
    function Add(Account, HumanName: string; HumanID: Integer; HeroName: string; Sex, Job, Hair: Byte; IsDeputyHero: Boolean): Boolean;
    //function Delete(HeroName: string): Boolean;
    //function DeleteRestore(HeroName: string): Boolean;
    function Erase(HeroName: string): Boolean; overload;
    function Save(HeroID: Integer; HeroData: PTHeroData): Boolean;
    function Rename(HeroID: Integer; HeroName, NewName: string): Boolean;
    function Assess(HeroID: Integer; HeroName, DeputyHeroName: string): Boolean;

    procedure GetRankData(MinLevel, MaxLevel, TopCount: LongWord;
      HeroRankList, WarriorRankList, WizardRankList, TaoistRankList: TRoleRankList);
  end;

  THumanDBClass = class of THumanDB;
  THeroDBClass = class of THeroDB;

  TRoleDB = class(TObject)
  private
    FHumanDB: THumanDB;
    FHeroDB: THeroDB;

    FCriticalSection: TRTLCriticalSection;
  protected
    procedure DoInit; virtual;
    procedure DoFainal; virtual;

    function GetHumanDBClass: THumanDBClass; virtual; abstract;
    function GetHeroDBClass: THeroDBClass; virtual; abstract;
  public
    constructor Create; virtual;
    destructor Destroy; override;

    procedure BeforeDestruction; override;

    procedure Init;
    procedure Fainal;

    procedure Lock;
    procedure UnLock;

    procedure Run; virtual;

    property HumanDB: THumanDB read FHumanDB;
    property HeroDB: THeroDB read FHeroDB;
  end;

implementation

uses
  DBShare;

{ THumanList }

constructor THumanList.Create;
begin
  FList := TList.Create;
end;

destructor THumanList.Destroy;
begin
  Clear;
  FList.Free;
  inherited;
end;

function THumanList.Add(HumData: PTHumData): PTHumData;
begin
  New(Result);
  Result^ := HumData^;
  FList.Add(Result);
end;

procedure THumanList.Clear;
var
  I: Integer;
begin
  for I := 0 to FList.Count - 1 do
  begin
    Dispose(PTHumData(FList.Items[I]));
  end;
  FList.Clear;
end;

function THumanList.GetCount: Integer;
begin
  Result := FList.Count;
end;

function THumanList.GetItems(Index: Integer): PTHumData;
begin
  if (Index >= 0) and (Index < FList.Count) then
    Result := FList.Items[Index]
  else
    Result := nil;
end;

procedure THumanList.SetCapacity(Value: Integer);
begin
  if FList.Capacity <> Value then
    FList.Capacity := Value;
end;

{ TQueryHumanList }

constructor TQueryHumanList.Create;
begin
  FList := TList.Create;
end;

destructor TQueryHumanList.Destroy;
begin
  Clear;
  FList.Free;
  inherited;
end;

function TQueryHumanList.Add(QueryHumanData: PTQueryHumanData): PTQueryHumanData;
begin
  New(Result);
  Result^ := QueryHumanData^;
  FList.Add(Result);
end;

procedure TQueryHumanList.Clear;
var
  I: Integer;
begin
  for I := 0 to FList.Count - 1 do
  begin
    Dispose(PTQueryHumanData(FList.Items[I]));
  end;
  FList.Clear;
end;

function TQueryHumanList.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TQueryHumanList.GetItems(Index: Integer): PTQueryHumanData;
begin
  if (Index >= 0) and (Index < FList.Count) then
    Result := FList.Items[Index]
  else
    Result := nil;
end;

procedure TQueryHumanList.SetCapacity(Value: Integer);
begin
  if FList.Capacity <> Value then
    FList.Capacity := Value;
end;

{ TSerarchRoleList }

constructor TSerarchRoleList.Create;
begin
  FList := TList.Create;
end;

destructor TSerarchRoleList.Destroy;
begin
  Clear;
  FList.Free;
  inherited;
end;

function TSerarchRoleList.Add(SerarchRoleData: PTSerarchRoleData): PTSerarchRoleData;
begin
  New(Result);
  Result^ := SerarchRoleData^;
  FList.Add(Result);
end;

procedure TSerarchRoleList.Clear;
var
  I: Integer;
begin
  for I := 0 to FList.Count - 1 do
  begin
    Dispose(PTSerarchRoleData(FList.Items[I]));
  end;
  FList.Clear;
end;

function TSerarchRoleList.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TSerarchRoleList.GetItems(Index: Integer): PTSerarchRoleData;
begin
  if (Index >= 0) and (Index < FList.Count) then
    Result := FList.Items[Index]
  else
    Result := nil;
end;

procedure TSerarchRoleList.SetCapacity(Value: Integer);
begin
  if FList.Capacity <> Value then
    FList.Capacity := Value;
end;

{ TRoleRankList }

constructor TRoleRankList.Create;
begin
  FList := TList.Create;
end;

destructor TRoleRankList.Destroy;
begin
  Clear;
  FList.Free;
  inherited;
end;

function TRoleRankList.Add(RoleRankData: PTRoleRankData): PTRoleRankData;
begin
  New(Result);
  Result^ := RoleRankData^;
  FList.Add(Result);
end;

procedure TRoleRankList.Clear;
var
  I: Integer;
begin
  for I := 0 to FList.Count - 1 do
  begin
    Dispose(PTRoleRankData(FList.Items[I]));
  end;
  FList.Clear;
end;

function TRoleRankList.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TRoleRankList.GetItems(Index: Integer): PTRoleRankData;
begin
  if (Index >= 0) and (Index < FList.Count) then
    Result := FList.Items[Index]
  else
    Result := nil;
end;

procedure TRoleRankList.SetCapacity(Value: Integer);
begin
  if FList.Capacity <> Value then
    FList.Capacity := Value;
end;

{ THumanDB }

constructor THumanDB.Create(AOwner: TRoleDB);
begin
  FOwner := AOwner;

end;

destructor THumanDB.Destroy;
begin
  inherited;
end;

procedure THumanDB.BeforeDestruction;
begin
  DoFinal;
end;

procedure THumanDB.Init;
begin
  DoInit;
end;

procedure THumanDB.Fainal;
begin
  DoFinal;
end;

function THumanDB.GetID(HumanName: string): Integer;
begin
  Result := NO_ID;
  FOwner.Lock;
  try
    try
      Result := DoGetID(HumanName);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THumanDB.CheckHumanExists(Account, HumanName: string): Boolean;
begin
  Result := False;
  FOwner.Lock;
  try
    try
      Result := DoCheckHumanExists(Account, HumanName);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THumanDB.GetHumanCount(Account: string): Integer;
begin
  Result := 0;
  FOwner.Lock;
  try
    try
      Result := DoGetHumanCount(Account);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THumanDB.GetOtherHumanName(Account, HumanName: string): string;
begin
  Result := '';
  FOwner.Lock;
  try
    try
      Result := DoGetOtherHumanName(Account, HumanName);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THumanDB.GetHumanHeroName(Account, HumanName: string; var HeroName, DeputyHeroName: string): Boolean;
begin
  Result := False;
  FOwner.Lock;
  try
    try
      Result := DoGetHumanHeroName(Account, HumanName, HeroName, DeputyHeroName);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THumanDB.GetBaseInfo(HumanName: string; var Sex, Job, Level, LastLogin: Integer): Boolean;
begin
  Result := False;
  FOwner.Lock;
  try
    try
      Result := DoGetBaseInfo(HumanName, Sex, Job, Level, LastLogin);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THumanDB.QueryHumans(Account: string; HumanList: TQueryHumanList): Integer;
begin
  Result := 0;
  FOwner.Lock;
  try
    try
      Result := DoQueryHumans(Account, HumanList);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THumanDB.QueryDeleteHumans(Account: string; HumanList: TQueryHumanList): Integer;
begin
  Result := 0;
  FOwner.Lock;
  try
    try
      Result := DoQueryDeleteHumans(Account, HumanList);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THumanDB.SearchByAccount(Account: string; MatchType: TSearchMatchType;
  RoleList: TSerarchRoleList): Integer;
begin
  Result := 0;
  FOwner.Lock;
  try
    try
      Result := DoSearchByAccount(Account, MatchType, RoleList);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THumanDB.SearchByName(HumanName: string; MatchType: TSearchMatchType;
  RoleList: TSerarchRoleList): Integer;
begin
  Result := 0;

  FOwner.Lock;
  try
    try
      Result := DoSearchByName(HumanName, MatchType, RoleList);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THumanDB.SearchByLevel(LimitCount, MinLevel: Integer; RoleList: TSerarchRoleList): Integer;
begin
  Result := 0;
  FOwner.Lock;
  try
    try
      Result := DoSearchByLevel(LimitCount, MinLevel, RoleList);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THumanDB.GetMobileNumbers(OnlyBindMobile: Boolean; MobileNumberList: TStrings): Integer;
begin
  Result := 0;
  FOwner.Lock;
  try
    try
      Result := DoGetMobileNumbers(OnlyBindMobile, MobileNumberList);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THumanDB.Select(Account, HumanName: string): Boolean;
begin
  Result := False;
  FOwner.Lock;
  try
    try
      Result := DoSelect(Account, HumanName);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THumanDB.Get(Account, HumanName: string;
  var HumData: THumData; var HumanID: Integer): Boolean;
begin
  Result := False;
  FOwner.Lock;
  try
    try
      Result := DoGet(Account, HumanName, HumData, HumanID);

      if Result then
      begin
        if (HumData.btSex < 0) or (HumData.btSex > 1) then HumData.btSex := 0;
        if (HumData.btJob < 0) or (HumData.btJob > 2) then HumData.btJob := 0;
      end;
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;


function THumanDB.Add(Account, HumanName: string; IsSelect: Boolean; Sex, Job, Hair: Byte): Boolean;
begin
  Result := False;
  
  FOwner.Lock;
  try
    try
      if (Sex < 0) or (Sex > 1) then Sex := 0;
      if (Job < 0) or (Job > 2) then Job := 0;
      
      Result := DoAdd(Account, HumanName, IsSelect, Sex, Job, Hair);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;


function THumanDB.Delete(Account, HumanName: string): Boolean;
begin
  Result := False;
  
  FOwner.Lock;
  try
    try
      Result := DoDelete(Account, HumanName);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THumanDB.SetEnabled(Account, HumanName: string; Enabled: Integer): Boolean;
begin
  Result := False;
  FOwner.Lock;
  try
    try
      Result := DoSetEnabled(Account, HumanName, Enabled);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THumanDB.DeleteRestore(Account, HumanName: string): Boolean;
begin
  Result := False;
  FOwner.Lock;
  try
    try
      Result := DoDeleteRestore(Account, HumanName);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THumanDB.Erase(Account, HumanName: string): Boolean;
begin
  Result := False;
  FOwner.Lock;
  try
    try
      Result := DoErase(Account, HumanName);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THumanDB.RecordLoginTime(Account, HumanName: string): Boolean;
begin
  Result := False;
  FOwner.Lock;
  try
    try
      Result := DoRecordLoginTime(Account, HumanName);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THumanDB.Save(HumanID: Integer; HumData: PTHumData): Boolean;
begin
  Result := False;
  FOwner.Lock;
  try
    try
      if (HumData.btSex < 0) or (HumData.btSex > 1) then HumData.btSex := 0;
      if (HumData.btJob < 0) or (HumData.btJob > 2) then HumData.btJob := 0;

      Result := DoSave(HumanID, HumData);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THumanDB.Rename(Account, HumanName: string; HumanID: Integer; NewName: string): Boolean;
begin
  Result := False;
  FOwner.Lock;
  try
    try
      Result := DoRename(Account, HumanName, HumanID, NewName);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THumanDB.ChangedGold(HumanName: string; ChangeType: TDBChangeGoldType; ChangedValue: Integer; CustomMoneyName: string; var ResultValue: LongWord): Boolean;
begin
  Result := False;
  FOwner.Lock;
  try
    try
      if ChangeType = cgtCustomMoney then
        DoChangedCustomMoney(DoGetID(HumanName), CustomMoneyName, ChangedValue, ResultValue)
      else
        Result := DoChangedGold(HumanName, ChangeType, ChangedValue, ResultValue);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

procedure THumanDB.GetRankData(MinLevel, MaxLevel, TopCount: LongWord;
  HumanRankList, WarriorRankList, WizardRankList, TaoistRankList, MasterRankList: TRoleRankList);
begin
  FOwner.Lock;
  try
    try
      DoGetRankData(MinLevel, MaxLevel, TopCount, HumanRankList, WarriorRankList, WizardRankList, TaoistRankList, MasterRankList);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THumanDB.BuyPlayer(const sSellAccount, sSellHumanName, sBuyAccount, sBuyHumanName: string): Boolean;
begin
  FOwner.Lock;
  try
    try
      Result := DoBuyPlayer(sSellAccount, sSellHumanName, sBuyAccount, sBuyHumanName);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

{ THeroDB }

constructor THeroDB.Create(AOwner: TRoleDB);
begin
  FOwner := AOwner;
end;

destructor THeroDB.Destroy;
begin
  inherited;
end;

procedure THeroDB.BeforeDestruction;
begin
  DoFinal;
end;

procedure THeroDB.Init;
begin
  DoInit;
end;

procedure THeroDB.Fainal;
begin
  DoFinal;
end;

function THeroDB.GetID(HeroName: string): Integer;
begin
  Result := NO_ID;
  FOwner.Lock;
  try
    try
      Result := DoGetID(HeroName);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THeroDB.SearchByAccount(Account: string; MatchType: TSearchMatchType;
  RoleList: TSerarchRoleList): Integer;
begin
  Result := 0;
  FOwner.Lock;
  try
    try
      Result := DoSearchByAccount(Account, MatchType, RoleList);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THeroDB.SearchByName(HeroName: string; MatchType: TSearchMatchType;
  RoleList: TSerarchRoleList): Integer;
begin
  Result := 0;
  FOwner.Lock;
  try
    try
      Result := DoSearchByName(HeroName, MatchType, RoleList);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THeroDB.Get(HeroName: string;
  var HeroData: THeroData; var HeroID: Integer): Boolean;
begin
  Result := False;
  FOwner.Lock;
  try
    try
      Result := DoGet(HeroName, HeroData, HeroID);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THeroDB.Add(Account, HumanName: string; HumanID: Integer;
  HeroName: string; Sex, Job, Hair: Byte; IsDeputyHero: Boolean): Boolean;
begin
  Result := False;
  FOwner.Lock;
  try
    try
      Result := DoAdd(Account, HumanName, HumanID, HeroName, Sex, Job, Hair, IsDeputyHero);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

{
function THeroDB.Delete(HeroName: string): Boolean;
begin
  FOwner.Lock;
  try
    try
      Result := DoDelete(HeroName);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THeroDB.DeleteRestore(HeroName: string): Boolean;
begin
  FOwner.Lock;
  try
    try
      Result := DoDeleteRestore(HeroName);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;
}

function THeroDB.Erase(HeroName: string): Boolean;
begin
  Result := False;
  FOwner.Lock;
  try
    try
      Result := DoErase(HeroName);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THeroDB.Save(HeroID: Integer; HeroData: PTHeroData): Boolean;
begin
  Result := False;
  FOwner.Lock;
  try
    try
      Result := DoSave(HeroID, HeroData);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THeroDB.Rename(HeroID: Integer; HeroName, NewName: string): Boolean;
begin
  Result := False;
  FOwner.Lock;
  try
    try
      Result := DoRename(HeroID, HeroName, NewName);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function THeroDB.Assess(HeroID: Integer; HeroName, DeputyHeroName: string): Boolean;
begin
  Result := False;
  FOwner.Lock;
  try
    try
      Result := DoAssess(HeroID, HeroName, DeputyHeroName);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

procedure THeroDB.GetRankData(MinLevel, MaxLevel, TopCount: LongWord;
  HeroRankList, WarriorRankList, WizardRankList, TaoistRankList: TRoleRankList);
begin
  FOwner.Lock;
  try
    try
      DoGetRankData(MinLevel, MaxLevel, TopCount, HeroRankList, WarriorRankList, WizardRankList, TaoistRankList);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

{ TRoleDB }

procedure TRoleDB.BeforeDestruction;
begin
  inherited;
  DoFainal;
end;

constructor TRoleDB.Create;
begin
  InitializeCriticalSection(FCriticalSection);
  FHumanDB := GetHumanDBClass.Create(Self);
  FHeroDB := GetHeroDBClass.Create(Self);
end;

destructor TRoleDB.Destroy;
begin
  FHumanDB.Free;
  FHeroDB.Free;
  DeleteCriticalSection(FCriticalSection);
  inherited;
end;

procedure TRoleDB.Init;
begin
  DoInit;
end;

procedure TRoleDB.Fainal;
begin
  DoFainal;
end;

procedure TRoleDB.DoInit;
begin
  FHumanDB.DoInit;
  FHeroDB.DoInit;
end;

procedure TRoleDB.DoFainal;
begin
  FHumanDB.DoFinal;
  FHeroDB.DoFinal;
end;

procedure TRoleDB.Lock;
begin
  EnterCriticalSection(FCriticalSection);
end;

procedure TRoleDB.UnLock;
begin
  LeaveCriticalSection(FCriticalSection);
end;

procedure TRoleDB.Run;
begin
  
end;

end.
 