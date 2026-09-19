unit AccountDB;

interface

uses
  Windows, Classes, SysUtils, Grobal2;

type
  TAccountList = class(TObject)
  private
    FList: TList;
    function GetCount: Integer;
    function GetItems(Index: Integer): PTAccountInfo;
  public
    constructor Create;
    destructor Destroy; override;

    property Count: Integer read GetCount;
    property Items[Index: Integer]: PTAccountInfo read GetItems; default;

    function Add(AccountInfo: TAccountInfo): PTAccountInfo;
    procedure Clear;

    procedure SetCapacity(Value: Integer);
  end;

  TAccountUpdateField = (ufPartField, ufAllField);
  TAccountDB = class(TObject)
  private
    FFileName: string;
    FCriticalSection: TRTLCriticalSection;
  protected
    procedure Lock;
    procedure UnLock;

    procedure DoInit; virtual; abstract;
    procedure DoFinal; virtual; abstract;

    function DoGetAccountByQuick(UID, ID: string; var AccountInfo: TAccountInfo): Boolean; virtual; abstract;
    function DoGetAccountByPhone(Phone: string; var AccountInfo: TAccountInfo): Boolean; virtual; abstract;
    function DoGetAccount(AccountName: string; var AccountInfo: TAccountInfo): Boolean; virtual; abstract;
    function DoFindAccount(AccountName: string; AccountList: TAccountList): Integer; virtual; abstract;
    function DoUpdateAccount(AccountInfo: TAccountInfo; UpdateField: TAccountUpdateField): Boolean; virtual; abstract;

    procedure DoGetAllAccount(AccountList: TStrings); virtual; abstract;
    function DoEnabledAccounts(AccountList: TStrings; Enabled: Boolean): Boolean; virtual; abstract;

    function DoCheckAccountExists(AccountName: string): Boolean; virtual; abstract;
    function DoAddAccount(AccountInfo: TAccountInfo): Boolean; virtual; abstract;

    function DoUnLockAccount(AccountName: string): Boolean; virtual; abstract;
  public
    constructor Create(FileName: string); virtual;
    destructor Destroy; override;
    procedure BeforeDestruction; override;

    property FileName: string read FFileName;

    procedure Init;
    procedure Fainal;
                                                                                   
    function GetAccountByQuick(UID, ID: string; var AccountInfo: TAccountInfo): Boolean;
    function GetAccountByPhone(Phone: string; var AccountInfo: TAccountInfo): Boolean;
    function GetAccount(AccountName: string; var AccountInfo: TAccountInfo): Boolean;
    function FindAccount(AccountName: string; AccountList: TAccountList): Integer;
    function UpdateAccount(AccountInfo: TAccountInfo; UpdateField: TAccountUpdateField): Boolean;

    procedure GetAllAccount(AccountList: TStrings);
    function EnabledAccounts(AccountList: TStrings; Enabled: Boolean): Boolean;

    function CheckAccountExists(AccountName: string): Boolean;
    function AddAccount(AccountInfo: TAccountInfo): Boolean;

    function UnLockAccount(AccountName: string): Boolean;

    procedure Run; virtual;
  end;

var
  g_AccountDB: TAccountDB = nil;

implementation

uses
  LSShare;

{ TAccountList }

constructor TAccountList.Create;
begin
  FList := TList.Create;
end;

destructor TAccountList.Destroy;
begin
  Clear;
  FList.Free;
  inherited;
end;

function TAccountList.Add(AccountInfo: TAccountInfo): PTAccountInfo;
begin
  New(Result);
  Result^ := AccountInfo;
  FList.Add(Result);
end;

procedure TAccountList.Clear;
var
  I: Integer;
begin
  for I := 0 to FList.Count - 1 do
  begin
    Dispose(PTAccountInfo(FList.Items[I]));
  end;
  FList.Clear;
end;

function TAccountList.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TAccountList.GetItems(Index: Integer): PTAccountInfo;
begin
  if (Index >= 0) and (Index < FList.Count) then
    Result := FList.Items[Index]
  else
    Result := nil;
end;

procedure TAccountList.SetCapacity(Value: Integer);
begin
  if FList.Capacity <> Value then
    FList.Capacity := Value;
end;

{ TAccountDB }

constructor TAccountDB.Create(FileName: string);
begin
  FFileName := FileName;
  InitializeCriticalSection(FCriticalSection);
end;

destructor TAccountDB.Destroy;
begin
  DeleteCriticalSection(FCriticalSection);
  inherited;
end;

procedure TAccountDB.BeforeDestruction;
begin
  DoFinal;
end;

procedure TAccountDB.Init;
begin
  DoInit;
end;

procedure TAccountDB.Fainal;
begin
  DoFinal;
end;

procedure TAccountDB.Lock;
begin
  EnterCriticalSection(FCriticalSection);
end;

procedure TAccountDB.UnLock;
begin
  LeaveCriticalSection(FCriticalSection);
end;

function TAccountDB.AddAccount(AccountInfo: TAccountInfo): Boolean;
begin
  Lock;
  try
    try
      Result := DoAddAccount(AccountInfo);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    UnLock;
  end;
end;

function TAccountDB.CheckAccountExists(AccountName: string): Boolean;
begin
  Lock;
  try
    try
      Result := DoCheckAccountExists(AccountName);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    UnLock;
  end;
end;

function TAccountDB.FindAccount(AccountName: string;
  AccountList: TAccountList): Integer;
begin
  Lock;
  try
    try
      Result := DoFindAccount(AccountName, AccountList);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    UnLock;
  end;
end;

function TAccountDB.GetAccount(AccountName: string;
  var AccountInfo: TAccountInfo): Boolean;
begin
  Lock;
  try
    try
      Result := DoGetAccount(AccountName, AccountInfo);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    UnLock;
  end;
end;

function TAccountDB.GetAccountByPhone(Phone: string; var AccountInfo: TAccountInfo): Boolean;
begin
  Lock;
  try
    try
      Result := DoGetAccountByPhone(Phone, AccountInfo);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    UnLock;
  end;
end;   

function TAccountDB.GetAccountByQuick(UID, ID: string; var AccountInfo: TAccountInfo): Boolean;
begin
  Lock;
  try
    try
      Result := DoGetAccountByQuick(UID, ID, AccountInfo);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    UnLock;
  end;
end;

function TAccountDB.UpdateAccount(AccountInfo: TAccountInfo;
  UpdateField: TAccountUpdateField): Boolean;
begin
  Lock;
  try
    try
      Result := DoUpdateAccount(AccountInfo, UpdateField);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    UnLock;
  end;
end;

function TAccountDB.UnLockAccount(AccountName: string): Boolean;
begin
  Lock;
  try
    try
      Result := DoUnLockAccount(AccountName);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    UnLock;
  end;
end;

procedure TAccountDB.GetAllAccount(AccountList: TStrings);
begin
  Lock;
  try
    try
      DoGetAllAccount(AccountList);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    UnLock;
  end;
end;

function TAccountDB.EnabledAccounts(AccountList: TStrings; Enabled: Boolean): Boolean;
begin
  Lock;
  try
    try
      Result := DoEnabledAccounts(AccountList, Enabled);
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
      end;
    end;
  finally
    UnLock;
  end;
end;

procedure TAccountDB.Run;
begin

end;

end.
