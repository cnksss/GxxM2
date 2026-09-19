unit MySqlAccountDB;

interface

uses
  Windows, SysUtils, Classes, AccountDB, Grobal2, LSShare, MySQLDataBase, MySQLWrap, MySQLCli, MySqlCreateTableSql;

type
  PDBServerConfig = ^TDBServerConfig;
  TDBServerConfig = record
    DBServer: string;
    DBPort: Word;
    DBUser: string;
    DBPassword: string;
    DataBase: string;
  end;

  TMySqlAccountDB = class(TAccountDB)
  private
    FLastRequestTick: LongWord;

    FMySqlLib: TMySQLLib;
    FDB: TMySQLDataBase;
    FDBConfig: TDBServerConfig;

    FStatementGet: TMySqlStatement; 
    FStatementGetByPhone: TMySqlStatement;
    FStatementGetByQuick: TMySqlStatement;
    FStatementFind: TMySqlStatement;

    FStatementNew: TMySqlStatement;
    FStatementCheckExists: TMySqlStatement;
    FStatementUpdatePartField: TMySqlStatement;
    FStatementUpdateAllField: TMySqlStatement;
    FStatementUnLockAccount: TMySqlStatement;

    FStatementGetAllAccount: TMySqlStatement;
    FStatementEnabledAccount: TMySqlStatement;

    procedure OnRequest(Sender: TObject);
  protected
    procedure DoInit; override;
    procedure DoFinal; override;
                              
    function DoGetAccountByQuick(UID, ID: string; var AccountInfo: TAccountInfo): Boolean; override;                               
    function DoGetAccountByPhone(Phone: string; var AccountInfo: TAccountInfo): Boolean; override;
    function DoGetAccount(AccountName: string; var AccountInfo: TAccountInfo): Boolean; override;
    function DoFindAccount(AccountName: string; AccountList: TAccountList): Integer; override;
    function DoUpdateAccount(AccountInfo: TAccountInfo; UpdateField: TAccountUpdateField): Boolean; override;

    procedure DoGetAllAccount(AccountList: TStrings); override;
    function DoEnabledAccounts(AccountList: TStrings; Enabled: Boolean): Boolean; override;

    function DoCheckAccountExists(AccountName: string): Boolean; override;
    function DoAddAccount(AccountInfo: TAccountInfo): Boolean; override;

    function DoUnLockAccount(AccountName: string): Boolean; override;
  public
    constructor Create(DBServerConfig: TDBServerConfig); overload;
    destructor Destroy; override;

    procedure Run; override;
  end;

implementation

{ TSqliteAccountDB }

constructor TMySqlAccountDB.Create(DBServerConfig: TDBServerConfig);
begin
  inherited Create('');

  FDBConfig := DBServerConfig;

  FMySqlLib := TMySQLLib.Create(nil);
  FMySqlLib.Load('', 'libmysql-32.dll');

  FDB := TMySQLDataBase.Create(FMySqlLib);
  FDB.Init;
  FDB.OnRequest := OnRequest;

  FStatementGet := nil;
  FStatementGetByPhone := nil;    
  FStatementGetByQuick := nil;
  FStatementFind := nil;
                                    
  FStatementNew := nil;
  FStatementCheckExists := nil;
  FStatementUpdatePartField := nil;
  FStatementUpdateAllField := nil;
  FStatementUnLockAccount := nil;

  FStatementGetAllAccount := nil;
  FStatementEnabledAccount := nil;

  FLastRequestTick := GetTickCount;
end;

destructor TMySqlAccountDB.Destroy;
begin
  FDB.Free;
  FMySqlLib.Free;
  inherited;
end;

procedure TMySqlAccountDB.DoInit;
begin
  inherited;

  FDB.Connect(FDBConfig.DBServer, FDBConfig.DBUser, FDBConfig.DBPassword, FDBConfig.DataBase, FDBConfig.DBPort, CLIENT_MULTI_STATEMENTS);
  FDB.CharacterSetName := 'utf8';


  FStatementGet := FDB.Statements.AddSQLStatement('select_Account');
  FStatementGet.Sql :=
    'select Account, Disable, Password, UserName, IDCard, BirthDay, ' +
      'Questions1, Answers1, Questions2, Answers2, Phone, MobilePhone, Mail, ' +
      'L2Password, CreateDate, LoginDate, LoginMac, LoginIP, LastActionTick, ' +
      'ErrorCount, Memo ' +
    'from Account where Account = ?';

  FStatementGetByPhone := FDB.Statements.AddSQLStatement('select_Account_phone');
  FStatementGetByPhone.Sql :=
    'select Account, Disable, Password, UserName, IDCard, BirthDay, ' +
      'Questions1, Answers1, Questions2, Answers2, Phone, MobilePhone, Mail, ' +
      'L2Password, CreateDate, LoginDate, LoginMac, LoginIP, LastActionTick, ' +
      'ErrorCount, Memo ' +
    'from Account where MobilePhone = ?';

  FStatementGetByQuick := FDB.Statements.AddSQLStatement('select_Account_quick');
  FStatementGetByQuick.Sql :=
    'select Account, Disable, Password, UserName, IDCard, BirthDay, ' +
      'Questions1, Answers1, Questions2, Answers2, Phone, MobilePhone, Mail, ' +
      'L2Password, CreateDate, LoginDate, LoginMac, LoginIP, LastActionTick, ' +
      'ErrorCount, Memo, UID, CID ' +
    'from Account where UID = ? and CID = ?';

  FStatementFind := FDB.Statements.AddSQLStatement('find_Account');
  FStatementFind.Sql :=
    'select Account, Disable, Password, UserName, IDCard, BirthDay, ' +
      'Questions1, Answers1, Questions2, Answers2, Phone, MobilePhone, Mail, ' +
      'L2Password, CreateDate, LoginDate, LoginMac, LoginIP, LastActionTick, ' +
      'ErrorCount, Memo ' +
    'from Account where Account LIKE ?';

  FStatementNew := FDB.Statements.AddSQLStatement('new_Account');
  FStatementNew.Sql :=
    'insert into Account(' +
      'Account, ' +
      'Password, ' +
      'UserName, ' +
      'IDCard, ' +
      'BirthDay, ' +
      'Questions1, ' +
      'Answers1, ' +
      'Questions2, ' +
      'Answers2, ' +
      'Phone, ' +
      'MobilePhone,' +
      'Mail, ' +
      'L2Password, ' +
      'CreateDate, ' +   
      'Memo, ' +
      'UID, ' +
      'CID) ' +
    'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)';


  FStatementCheckExists := FDB.Statements.AddSQLStatement('exists_Account');
  FStatementCheckExists.Sql :=
    'select 1 from Account where Account = ?;';       // COLLATE NOCASE

  FStatementUpdatePartField := FDB.Statements.AddSQLStatement('update_Account_1');
  FStatementUpdatePartField.Sql :=
    'update Account set ' +
      'L2Password = ?, ' +
      'LoginDate = ?, ' +
      'LoginMac = ?, ' +
      'LoginIP = ?, ' +
      'LastActionTick = ?, ' +
      'ErrorCount = ? ' +
    'where Account = ? ';

  FStatementUpdateAllField := FDB.Statements.AddSQLStatement('update_Account_2');
  FStatementUpdateAllField.Sql :=
    'update Account set ' +
      'Password = ?, ' +
      'UserName = ?, ' +
      'IDCard = ?, ' +
      'BirthDay = ?, ' +
      'Questions1 = ?, ' +
      'Answers1 = ?, ' +
      'Questions2 = ?, ' +
      'Answers2 = ?, ' +
      'Phone = ?, ' +
      'MobilePhone = ?, ' +
      'Mail = ?, ' +
      'L2Password = ?, ' +
      'Memo = ? ' +
    'where Account = ? ';

  FStatementUnLockAccount := FDB.Statements.AddSQLStatement('unlock_Account');
  FStatementUnLockAccount.Sql :=
    'update Account set ' +
      'LastActionTick = 0, ' +
      'ErrorCount = 0 ' +
    'where Account = ? ';

  FStatementGetAllAccount := FDB.Statements.AddSQLStatement('get_all_Account');
  FStatementGetAllAccount.Sql :=
    'select Account, Disable from Account';

  FStatementEnabledAccount := FDB.Statements.AddSQLStatement('enabled_Account');
  FStatementEnabledAccount.Sql :=
    'update Account set Disable = ? where Account = ? ';

  FStatementGet.Prepare;
  FStatementGetByPhone.Prepare;  
  FStatementGetByQuick.Prepare;
  FStatementFind.Prepare;
                               
  FStatementNew.Prepare;     
  FStatementCheckExists.Prepare;
  FStatementUpdatePartField.Prepare;
  FStatementUpdateAllField.Prepare;
  FStatementUnLockAccount.Prepare;
  FStatementGetAllAccount.Prepare;
  FStatementEnabledAccount.Prepare;
end;


procedure TMySqlAccountDB.DoFinal;
begin       
  if FStatementGet <> nil then FStatementGet.Finalize;
  if FStatementGetByPhone <> nil then FStatementGetByPhone.Finalize;   
  if FStatementGetByQuick <> nil then FStatementGetByQuick.Finalize;
  if FStatementFind <> nil then FStatementFind.Finalize;
                                                           
  if FStatementNew <> nil then FStatementNew.Finalize;
  if FStatementCheckExists <> nil then FStatementCheckExists.Finalize;
  if FStatementUpdatePartField <> nil then FStatementUpdatePartField.Finalize;
  if FStatementUpdateAllField <> nil then FStatementUpdateAllField.Finalize;
  if FStatementUnLockAccount <> nil then FStatementUnLockAccount.Finalize;
  if FStatementGetAllAccount <> nil then FStatementGetAllAccount.Finalize;
  if FStatementEnabledAccount <> nil then FStatementEnabledAccount.Finalize;

  inherited;
end;

function TMySqlAccountDB.DoFindAccount(AccountName: string;
  AccountList: TAccountList): Integer;
var
  AccountInfo: TAccountInfo;
begin
  Result := 0;
  if FStatementFind = nil then Exit;

  AccountList.Clear;
  try
    FStatementFind.Reset;
    FStatementFind.BindParamText(0, AccountName + '%');
    if FStatementFind.Query then
    begin
      Assert(FStatementFind.FieldCount = 21, 'FindAccount column count error.');

      while FStatementFind.Fetch do
      begin
        AccountInfo.AccountName := FStatementFind.GetColumnValueText(0);
        AccountInfo.IsDisable := FStatementFind.GetColumnValueBool(1);
        AccountInfo.Password := FStatementFind.GetColumnValueText(2);
        AccountInfo.UserName := FStatementFind.GetColumnValueText(3);
        AccountInfo.IDCard := FStatementFind.GetColumnValueText(4);
        AccountInfo.BirthDay := FStatementFind.GetColumnValueText(5);
        AccountInfo.Questions1 := FStatementFind.GetColumnValueText(6);
        AccountInfo.Answers1 := FStatementFind.GetColumnValueText(7);
        AccountInfo.Questions2 := FStatementFind.GetColumnValueText(8);
        AccountInfo.Answers2 := FStatementFind.GetColumnValueText(9);
        AccountInfo.Phone := FStatementFind.GetColumnValueText(10);
        AccountInfo.MobilePhone := FStatementFind.GetColumnValueText(11);
        AccountInfo.Mail := FStatementFind.GetColumnValueText(12);
        AccountInfo.L2Password := FStatementFind.GetColumnValueText(13);
        AccountInfo.CreateDate := FStatementFind.GetColumnValueInt(14);
        AccountInfo.LoginDate := FStatementFind.GetColumnValueInt(15);
        AccountInfo.LoginMac := FStatementFind.GetColumnValueText(16);
        AccountInfo.LoginIP := FStatementFind.GetColumnValueInt(17);
        AccountInfo.LastActionTick := FStatementFind.GetColumnValueInt(18);
        AccountInfo.ErrorCount := FStatementFind.GetColumnValueInt(19);
        AccountInfo.Memo := FStatementFind.GetColumnValueText(20);

        AccountList.Add(AccountInfo);
      end;
    end;

    Result := AccountList.Count;
  finally
    FStatementFind.Reset;
  end;
end;

function TMySqlAccountDB.DoGetAccount(AccountName: string;
  var AccountInfo: TAccountInfo): Boolean;
begin
  Result := False;
  if FStatementGet = nil then Exit;
  
  try
    FStatementGet.Reset;
    FStatementGet.BindParamText(0, AccountName);
    if FStatementGet.Query and FStatementGet.Fetch then
    begin
      Assert(FStatementGet.FieldCount = 21, 'GetAccount column count error.');

      Result := True;
      AccountInfo.AccountName     := FStatementGet.GetColumnValueText(0);
      AccountInfo.IsDisable       := FStatementGet.GetColumnValueBool(1);
      AccountInfo.Password        := FStatementGet.GetColumnValueText(2);
      AccountInfo.UserName        := FStatementGet.GetColumnValueText(3);
      AccountInfo.IDCard          := FStatementGet.GetColumnValueText(4);
      AccountInfo.BirthDay        := FStatementGet.GetColumnValueText(5);
      AccountInfo.Questions1      := FStatementGet.GetColumnValueText(6);
      AccountInfo.Answers1        := FStatementGet.GetColumnValueText(7);
      AccountInfo.Questions2      := FStatementGet.GetColumnValueText(8);
      AccountInfo.Answers2        := FStatementGet.GetColumnValueText(9);
      AccountInfo.Phone           := FStatementGet.GetColumnValueText(10);
      AccountInfo.MobilePhone     := FStatementGet.GetColumnValueText(11);
      AccountInfo.Mail            := FStatementGet.GetColumnValueText(12);
      AccountInfo.L2Password      := FStatementGet.GetColumnValueText(13);
      AccountInfo.CreateDate      := FStatementGet.GetColumnValueInt(14);
      AccountInfo.LoginDate       := FStatementGet.GetColumnValueInt(15);
      AccountInfo.LoginMac        := FStatementGet.GetColumnValueText(16);
      AccountInfo.LoginIP         := FStatementGet.GetColumnValueInt(17);
      AccountInfo.LastActionTick  := FStatementGet.GetColumnValueInt(18);
      AccountInfo.ErrorCount      := FStatementGet.GetColumnValueInt(19);
      AccountInfo.Memo            := FStatementGet.GetColumnValueText(20);
    end;
  finally
    FStatementGet.Reset;
  end;
end;
 
function TMySqlAccountDB.DoGetAccountByQuick(UID, ID: string;
  var AccountInfo: TAccountInfo): Boolean;
begin
  Result := False;
  if FStatementGetByQuick = nil then Exit;
  
  try
    FStatementGetByQuick.Reset;
    FStatementGetByQuick.BindParamText(0, UID);
    FStatementGetByQuick.BindParamText(1, ID);
    if FStatementGetByQuick.Query and FStatementGetByQuick.Fetch then
    begin
      Assert(FStatementGetByQuick.FieldCount = 23, 'GetAccount column count error.');

      Result := True;
      AccountInfo.AccountName     := FStatementGetByQuick.GetColumnValueText(0);
      AccountInfo.IsDisable       := FStatementGetByQuick.GetColumnValueBool(1);
      AccountInfo.Password        := FStatementGetByQuick.GetColumnValueText(2);
      AccountInfo.UserName        := FStatementGetByQuick.GetColumnValueText(3);
      AccountInfo.IDCard          := FStatementGetByQuick.GetColumnValueText(4);
      AccountInfo.BirthDay        := FStatementGetByQuick.GetColumnValueText(5);
      AccountInfo.Questions1      := FStatementGetByQuick.GetColumnValueText(6);
      AccountInfo.Answers1        := FStatementGetByQuick.GetColumnValueText(7);
      AccountInfo.Questions2      := FStatementGetByQuick.GetColumnValueText(8);
      AccountInfo.Answers2        := FStatementGetByQuick.GetColumnValueText(9);
      AccountInfo.Phone           := FStatementGetByQuick.GetColumnValueText(10);
      AccountInfo.MobilePhone     := FStatementGetByQuick.GetColumnValueText(11);
      AccountInfo.Mail            := FStatementGetByQuick.GetColumnValueText(12);
      AccountInfo.L2Password      := FStatementGetByQuick.GetColumnValueText(13);
      AccountInfo.CreateDate      := FStatementGetByQuick.GetColumnValueInt(14);
      AccountInfo.LoginDate       := FStatementGetByQuick.GetColumnValueInt(15);
      AccountInfo.LoginMac        := FStatementGetByQuick.GetColumnValueText(16);
      AccountInfo.LoginIP         := FStatementGetByQuick.GetColumnValueInt(17);
      AccountInfo.LastActionTick  := FStatementGetByQuick.GetColumnValueInt(18);
      AccountInfo.ErrorCount      := FStatementGetByQuick.GetColumnValueInt(19);
      AccountInfo.Memo            := FStatementGetByQuick.GetColumnValueText(20);
      AccountInfo.UID             := FStatementGetByQuick.GetColumnValueText(21);
      AccountInfo.CID             := FStatementGetByQuick.GetColumnValueText(22);
    end;
  finally
    FStatementGetByQuick.Reset;
  end;
end;

function TMySqlAccountDB.DoGetAccountByPhone(Phone: string;
  var AccountInfo: TAccountInfo): Boolean;
begin
  Result := False;
  if FStatementGetByPhone = nil then Exit;
  
  try
    FStatementGetByPhone.Reset;
    FStatementGetByPhone.BindParamText(0, Phone);
    if FStatementGetByPhone.Query and FStatementGetByPhone.Fetch then
    begin
      Assert(FStatementGetByPhone.FieldCount = 21, 'GetAccount column count error.');

      Result := True;
      AccountInfo.AccountName     := FStatementGetByPhone.GetColumnValueText(0);
      AccountInfo.IsDisable       := FStatementGetByPhone.GetColumnValueBool(1);
      AccountInfo.Password        := FStatementGetByPhone.GetColumnValueText(2);
      AccountInfo.UserName        := FStatementGetByPhone.GetColumnValueText(3);
      AccountInfo.IDCard          := FStatementGetByPhone.GetColumnValueText(4);
      AccountInfo.BirthDay        := FStatementGetByPhone.GetColumnValueText(5);
      AccountInfo.Questions1      := FStatementGetByPhone.GetColumnValueText(6);
      AccountInfo.Answers1        := FStatementGetByPhone.GetColumnValueText(7);
      AccountInfo.Questions2      := FStatementGetByPhone.GetColumnValueText(8);
      AccountInfo.Answers2        := FStatementGetByPhone.GetColumnValueText(9);
      AccountInfo.Phone           := FStatementGetByPhone.GetColumnValueText(10);
      AccountInfo.MobilePhone     := FStatementGetByPhone.GetColumnValueText(11);
      AccountInfo.Mail            := FStatementGetByPhone.GetColumnValueText(12);
      AccountInfo.L2Password      := FStatementGetByPhone.GetColumnValueText(13);
      AccountInfo.CreateDate      := FStatementGetByPhone.GetColumnValueInt(14);
      AccountInfo.LoginDate       := FStatementGetByPhone.GetColumnValueInt(15);
      AccountInfo.LoginMac        := FStatementGetByPhone.GetColumnValueText(16);
      AccountInfo.LoginIP         := FStatementGetByPhone.GetColumnValueInt(17);
      AccountInfo.LastActionTick  := FStatementGetByPhone.GetColumnValueInt(18);
      AccountInfo.ErrorCount      := FStatementGetByPhone.GetColumnValueInt(19);
      AccountInfo.Memo            := FStatementGetByPhone.GetColumnValueText(20);
    end;
  finally
    FStatementGetByPhone.Reset;
  end;
end;

function TMySqlAccountDB.DoUpdateAccount(AccountInfo: TAccountInfo; UpdateField: TAccountUpdateField): Boolean;
begin
  Result := False;
  
  if UpdateField = ufPartField then
  begin
    if FStatementUpdatePartField = nil then Exit;

    try
      FStatementUpdatePartField.Reset;
      FStatementUpdatePartField.OrderBindParamText(AccountInfo.L2Password);
      FStatementUpdatePartField.OrderBindParamInt(AccountInfo.LoginDate);
      FStatementUpdatePartField.OrderBindParamText(AccountInfo.LoginMac);
      FStatementUpdatePartField.OrderBindParamInt(AccountInfo.LoginIP);
      FStatementUpdatePartField.OrderBindParamInt(AccountInfo.LastActionTick);
      FStatementUpdatePartField.OrderBindParamInt(AccountInfo.ErrorCount);
      FStatementUpdatePartField.OrderBindParamText(AccountInfo.AccountName);
      Result := FStatementUpdatePartField.Step;
    finally
      FStatementUpdatePartField.Reset;
    end;
  end
  else
  begin
    if FStatementUpdateAllField = nil then Exit;
    
    try
      FStatementUpdateAllField.Reset;
      FStatementUpdateAllField.OrderBindParamText(AccountInfo.Password);
      FStatementUpdateAllField.OrderBindParamText(AccountInfo.UserName);
      FStatementUpdateAllField.OrderBindParamText(AccountInfo.IDCard);
      FStatementUpdateAllField.OrderBindParamText(AccountInfo.BirthDay);
      FStatementUpdateAllField.OrderBindParamText(AccountInfo.Questions1);
      FStatementUpdateAllField.OrderBindParamText(AccountInfo.Answers1);
      FStatementUpdateAllField.OrderBindParamText(AccountInfo.Questions2);
      FStatementUpdateAllField.OrderBindParamText(AccountInfo.Answers2);
      FStatementUpdateAllField.OrderBindParamText(AccountInfo.Phone);
      FStatementUpdateAllField.OrderBindParamText(AccountInfo.MobilePhone);
      FStatementUpdateAllField.OrderBindParamText(AccountInfo.Mail);
      FStatementUpdateAllField.OrderBindParamText(AccountInfo.L2Password);
      FStatementUpdateAllField.OrderBindParamText(AccountInfo.Memo);
      FStatementUpdateAllField.OrderBindParamText(AccountInfo.AccountName);
      Result := FStatementUpdateAllField.Step;
    finally
      FStatementUpdateAllField.Reset;
    end;
  end;
end;

function TMySqlAccountDB.DoCheckAccountExists(AccountName: string): Boolean;
begin
  Result := False;
  if FStatementCheckExists = nil then Exit;
  
  FStatementCheckExists.Reset;
  try
    FStatementCheckExists.OrderBindParamText(AccountName);
    Result := FStatementCheckExists.Query and FStatementCheckExists.Fetch;
  finally
    FStatementCheckExists.Reset;
  end;
end;

function TMySqlAccountDB.DoAddAccount(AccountInfo: TAccountInfo): Boolean;
begin
  Result := False;
  if FStatementNew = nil then Exit;
  
  try
    FStatementNew.Reset;
    FStatementNew.OrderBindParamText(AccountInfo.AccountName);
    FStatementNew.OrderBindParamText(AccountInfo.Password);
    FStatementNew.OrderBindParamText(AccountInfo.UserName);
    FStatementNew.OrderBindParamText(AccountInfo.IDCard);
    FStatementNew.OrderBindParamText(AccountInfo.BirthDay);
    FStatementNew.OrderBindParamText(AccountInfo.Questions1);
    FStatementNew.OrderBindParamText(AccountInfo.Answers1);
    FStatementNew.OrderBindParamText(AccountInfo.Questions2);
    FStatementNew.OrderBindParamText(AccountInfo.Answers2);
    FStatementNew.OrderBindParamText(AccountInfo.Phone);
    FStatementNew.OrderBindParamText(AccountInfo.MobilePhone);
    FStatementNew.OrderBindParamText(AccountInfo.Mail);
    FStatementNew.OrderBindParamText(AccountInfo.L2Password);
    FStatementNew.OrderBindParamInt(Date2MyDate(Now));                    // CreateDate
    FStatementNew.OrderBindParamText(AccountInfo.Memo);
    FStatementNew.OrderBindParamText(AccountInfo.UID);
    FStatementNew.OrderBindParamText(AccountInfo.CID);
    Result := FStatementNew.Step;
  finally
    FStatementNew.Reset;
  end;
end;


function TMySqlAccountDB.DoUnLockAccount(AccountName: string): Boolean;
begin
  Result := False;
  if FStatementUnLockAccount = nil then Exit;
  
  try
    FStatementUnLockAccount.Reset;
    FStatementUnLockAccount.OrderBindParamText(AccountName);
    Result := FStatementUnLockAccount.Step;
  finally
    FStatementUnLockAccount.Reset;
  end;
end;

procedure TMySqlAccountDB.DoGetAllAccount(AccountList: TStrings);
begin
  if FStatementGetAllAccount = nil then Exit;
  
  AccountList.Clear;
  try
    FStatementGetAllAccount.Reset;
    if FStatementGetAllAccount.Query then
    begin
      while FStatementGetAllAccount.Fetch do
      begin
        AccountList.AddObject(FStatementGetAllAccount.OrderGetColumnValueText, TObject(FStatementGetAllAccount.OrderGetColumnValueInt));
      end;
    end;
  finally
    FStatementGetAllAccount.Reset;
  end;
end;

function TMySqlAccountDB.DoEnabledAccounts(AccountList: TStrings;
  Enabled: Boolean): Boolean;
var
  I: Integer;
begin
  Result := False;
  if FStatementEnabledAccount = nil then Exit;

  Result := False;
  FDB.StartTransaction;
  try
    for I := 0 to AccountList.Count - 1 do
    begin
      FStatementEnabledAccount.Reset;
      FStatementEnabledAccount.OrderBindParamInt(Integer(not Enabled));
      FStatementEnabledAccount.OrderBindParamText(AccountList.Strings[I]);

      FStatementEnabledAccount.Step;
    end;
    FDB.Commit;

    Result := True;
  except
    FDB.Rollback;
  end;
end;

procedure TMySqlAccountDB.Run;
begin
  inherited;
  if (FDB <> nil) and (FDB.MySQL <> nil) and (GetTickCount - FLastRequestTick >= 10 * 60000) then
  begin
    FDB.Exec('select 1');
  end;
end;

procedure TMySqlAccountDB.OnRequest(Sender: TObject);
begin
  FLastRequestTick := GetTickCount;  
end;

end.
