unit SqliteAccountDB;

interface

uses
  SysUtils, AccountDB, SQLite3DataBase, SQLiteCli, Grobal2, LSShare, SqliteCreateTableSql, Classes;

type
  TSqliteAccountDB = class(TAccountDB)
  private
    FDB: TSQLite3Database;

    FStatementCheckTableExists: TSQLStatement;
    FStatementGet: TSQLStatement;
    FStatementGetByPhone: TSQLStatement;
    FStatementGetByQuick: TSQLStatement;
    FStatementFind: TSQLStatement;

    FStatementNew: TSQLStatement;
    FStatementCheckExists: TSQLStatement;
    FStatementUpdatePartField: TSQLStatement;
    FStatementUpdateAllField: TSQLStatement;
    FStatementUnLockAccount: TSQLStatement;

    FStatementGetAllAccount: TSQLStatement;
    FStatementEnabledAccount: TSQLStatement;
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
    constructor Create(FileName: string); override;
    destructor Destroy; override;
  end;

implementation

{ TSqliteAccountDB }

const
  AlterFieldAccount =
    'ALTER TABLE "Account" RENAME TO "_Account_old_20170420";' + sLineBreak +

    'CREATE TABLE "Account" (' + sLineBreak +
    '	"Account"  TEXT(10) NOT NULL COLLATE NOCASE,' + sLineBreak +                    // ºöÂÔ´óÐ¡Ð´
    '	"Password"  TEXT(10) NOT NULL,' + sLineBreak +
    '	"UserName"  TEXT(20) NOT NULL,' + sLineBreak +
    '	"Disable"  INTEGER DEFAULT 0,' + sLineBreak +
    '	"IDCard"  TEXT(18),' + sLineBreak +
    '	"BirthDay"  TEXT(10),' + sLineBreak +
    '	"Questions1"  TEXT(20),' + sLineBreak +
    '	"Answers1"  TEXT(12),' + sLineBreak +
    '	"Questions2"  TEXT(20),' + sLineBreak +
    '	"Answers2"  TEXT(12),' + sLineBreak +
    '	"Phone"  TEXT(14),' + sLineBreak +
    '	"MobilePhone"  TEXT(13),' + sLineBreak +
    '	"Mail"  TEXT(40),' + sLineBreak +
    '	"L2Password"  TEXT(20),' + sLineBreak +
    '	"CreateDate"  INTEGER,' + sLineBreak +
    '	"LoginDate"  INTEGER,' + sLineBreak +
    '	"LoginMac"  TEXT(32),' + sLineBreak +
    '	"LoginIP"  INTEGER,' + sLineBreak +
    '	"LastActionTick"  INTEGER,' + sLineBreak +
    '	"ErrorCount"  INTEGER,' + sLineBreak +
    '	"Memo"  TEXT(20),' + sLineBreak +       
    '	"UID"  TEXT(70),' + sLineBreak +
    '	"CID"  TEXT(30),' + sLineBreak +
    '	PRIMARY KEY ("Account" ASC),' + sLineBreak +
    '	CONSTRAINT "uk_Account" UNIQUE ("Account" ASC)' + sLineBreak +
    ');' +

    'INSERT INTO "Account" (' + sLineBreak +
    '	"Account",' + sLineBreak +
    '	"Disable",' + sLineBreak +
    '	"Password",' + sLineBreak +
    '	"UserName",' + sLineBreak +
    '	"IDCard",' + sLineBreak +
    '	"BirthDay",' + sLineBreak +
    '	"Questions1",' + sLineBreak +
    '	"Answers1",' + sLineBreak +
    '	"Questions2",' + sLineBreak +
    '	"Answers2",' + sLineBreak +
    '	"Phone",' + sLineBreak +
    '	"MobilePhone",' + sLineBreak +
    '	"Mail",' + sLineBreak +
    '	"L2Password",' + sLineBreak +
    '	"CreateDate",' + sLineBreak +
    '	"LoginDate",' + sLineBreak +
    '	"LoginMac",' + sLineBreak +
    '	"LoginIP",' + sLineBreak +
    '	"LastActionTick",' + sLineBreak +
    '	"ErrorCount",' + sLineBreak +
    '	"Memo"' + sLineBreak +
    ') SELECT' + sLineBreak +
    '	"Account",' + sLineBreak +
    '	"Disable",' + sLineBreak +
    '	"Password",' + sLineBreak +
    '	"UserName",' + sLineBreak +
    '	"IDCard",' + sLineBreak +
    '	"BirthDay",' + sLineBreak +
    '	"Questions1",' + sLineBreak +
    '	"Answers1",' + sLineBreak +
    '	"Questions2",' + sLineBreak +
    '	"Answers2",' + sLineBreak +
    '	"Phone",' + sLineBreak +
    '	"MobilePhone",' + sLineBreak +
    '	"Mail",' + sLineBreak +
    '	"L2Password",' + sLineBreak +
    '	"CreateDate",' + sLineBreak +
    '	"LoginDate",' + sLineBreak +
    '	"LoginMac",' + sLineBreak +
    '	"LoginIP",' + sLineBreak +
    '	"LastActionTick",' + sLineBreak +
    '	"ErrorCount",' + sLineBreak +
    '	"Memo"' + sLineBreak +     
    '	"UID"' + sLineBreak +
    '	"CID"' + sLineBreak +
    'FROM' + sLineBreak +
    '	"_Account_old_20170420";' + sLineBreak +

    'DROP TABLE _Account_old_20170420;'  + sLineBreak +

    'CREATE TABLE IF NOT EXISTS "db_constant" ('  + sLineBreak +
    '"ConstName"  TEXT(30) NOT NULL COLLATE NOCASE ,'  + sLineBreak +
    '"ConstValue"  INTEGER,'  + sLineBreak +
    'PRIMARY KEY ("ConstName")'  + sLineBreak +
    ');'  + sLineBreak +
    'REPLACE INTO db_constant(ConstName, ConstValue) values ("version", ' + SQLITE_DBVERSION  + ');';

constructor TSqliteAccountDB.Create(FileName: string);
begin
  inherited Create(FileName);
  FDB := TSQLite3Database.Create();
  FDB.MustExist := True;

  FStatementCheckTableExists := FDB.Statements.AddSQLStatement('check_table_db_constant');
  FStatementCheckTableExists.Sql :=
    'select 1 from sqlite_master where type = "table" and name = ?;';

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
end;

destructor TSqliteAccountDB.Destroy;
begin
  FDB.Free;
  inherited;
end;

procedure TSqliteAccountDB.DoInit;
begin
  inherited;
  FDB.Database := FileName;
  if FileExists(FileName) then
  begin
    FDB.MustExist := True;
    FDB.Connected := True;

    FStatementCheckTableExists.Prepare;
    FStatementCheckTableExists.Reset;
    FStatementCheckTableExists.OrderBindText('db_constant');
    if FStatementCheckTableExists.Step <> SQLITE_ROW then
    begin
      FDB.Execute('PRAGMA foreign_keys = OFF;');
      try
        FDB.BeginTransaction;
        try
          FDB.Execute(AlterFieldAccount);
          FDB.Commit;
        except
          on E: Exception do
          begin
            MainOutMessage(E.Message);
            FDB.RollBack;
          end;
        end;
      finally
        FDB.Execute('PRAGMA foreign_keys = ON;');
      end;
    end;
    FStatementCheckTableExists.Reset;
  end
  else
  begin
    FDB.MustExist := False;
    FDB.Connected := True;
    FDB.Execute(SQLITE_SQL_CREATE_ACCOUNT_TABLES);
  end;

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


procedure TSqliteAccountDB.DoFinal;
begin
  FStatementGet.Finalize;
  FStatementGetByPhone.Finalize;  
  FStatementGetByQuick.Finalize;
  FStatementFind.Finalize;        

  FStatementNew.Finalize;
  FStatementCheckExists.Finalize;  
  FStatementUpdatePartField.Finalize;
  FStatementUpdateAllField.Finalize;
  FStatementUnLockAccount.Finalize;
  FStatementGetAllAccount.Finalize;
  FStatementEnabledAccount.Finalize;

  FStatementCheckTableExists.Finalize;

  FDB.Connected := False;
  inherited;
end;

function TSqliteAccountDB.DoFindAccount(AccountName: string;
  AccountList: TAccountList): Integer;
var
  Ret: Integer;
  AccountInfo: TAccountInfo;
begin
  AccountList.Clear;
  try
    FStatementFind.Reset;
    FStatementFind.BindText(1, AccountName + '%');
    Ret := FStatementFind.Step;
    Assert(FStatementFind.GetColumnCount = 21, 'FindAccount column count error.');

    while (Ret = SQLITE_ROW) do
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

      Ret := FStatementFind.Step;
    end;

    Result := AccountList.Count;
  finally
    FStatementFind.Reset;
  end;
end;

function TSqliteAccountDB.DoGetAccount(AccountName: string;
  var AccountInfo: TAccountInfo): Boolean;
var
  Ret: Integer;
begin
  Result := False;
  try
    FStatementGet.Reset;
    FStatementGet.BindText(1, AccountName);
    Ret := FStatementGet.Step;
    Assert(FStatementGet.GetColumnCount = 21, 'GetAccount column count error.');
    if (Ret = SQLITE_ROW) then
    begin
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
       
function TSqliteAccountDB.DoGetAccountByQuick(UID, ID: string;
  var AccountInfo: TAccountInfo): Boolean;
var
  Ret: Integer;
begin
  Result := False;
  try
    FStatementGetByQuick.Reset;
    FStatementGetByQuick.BindText(1, UID);
    FStatementGetByQuick.BindText(2, ID);
    Ret := FStatementGetByQuick.Step;
    Assert(FStatementGetByQuick.GetColumnCount = 23, 'GetAccount column count error.');
    if (Ret = SQLITE_ROW) then
    begin
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

function TSqliteAccountDB.DoGetAccountByPhone(Phone: string;
  var AccountInfo: TAccountInfo): Boolean;
var
  Ret: Integer;
begin
  Result := False;
  try
    FStatementGetByPhone.Reset;
    FStatementGetByPhone.BindText(1, Phone);
    Ret := FStatementGetByPhone.Step;
    Assert(FStatementGetByPhone.GetColumnCount = 21, 'GetAccount column count error.');
    if (Ret = SQLITE_ROW) then
    begin
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

function TSqliteAccountDB.DoUpdateAccount(AccountInfo: TAccountInfo; UpdateField: TAccountUpdateField): Boolean;
begin
  if UpdateField = ufPartField then
  begin
    try
      FStatementUpdatePartField.Reset;
      FStatementUpdatePartField.OrderBindText(AccountInfo.L2Password);
      FStatementUpdatePartField.OrderBindInt(AccountInfo.LoginDate);
      FStatementUpdatePartField.OrderBindText(AccountInfo.LoginMac);
      FStatementUpdatePartField.OrderBindInt(AccountInfo.LoginIP);
      FStatementUpdatePartField.OrderBindInt(AccountInfo.LastActionTick);
      FStatementUpdatePartField.OrderBindInt(AccountInfo.ErrorCount);
      FStatementUpdatePartField.OrderBindText(AccountInfo.AccountName);
      Result := FStatementUpdatePartField.Step in [SQLITE_OK, SQLITE_DONE];
    finally
      FStatementUpdatePartField.Reset;
    end;
  end
  else
  begin
    try
      FStatementUpdateAllField.Reset;
      FStatementUpdateAllField.OrderBindText(AccountInfo.Password);
      FStatementUpdateAllField.OrderBindText(AccountInfo.UserName);
      FStatementUpdateAllField.OrderBindText(AccountInfo.IDCard);
      FStatementUpdateAllField.OrderBindText(AccountInfo.BirthDay);
      FStatementUpdateAllField.OrderBindText(AccountInfo.Questions1);
      FStatementUpdateAllField.OrderBindText(AccountInfo.Answers1);
      FStatementUpdateAllField.OrderBindText(AccountInfo.Questions2);
      FStatementUpdateAllField.OrderBindText(AccountInfo.Answers2);
      FStatementUpdateAllField.OrderBindText(AccountInfo.Phone);
      FStatementUpdateAllField.OrderBindText(AccountInfo.MobilePhone);
      FStatementUpdateAllField.OrderBindText(AccountInfo.Mail);
      FStatementUpdateAllField.OrderBindText(AccountInfo.L2Password);
      FStatementUpdateAllField.OrderBindText(AccountInfo.Memo);
      FStatementUpdateAllField.OrderBindText(AccountInfo.AccountName);
      Result := FStatementUpdateAllField.Step in [SQLITE_OK, SQLITE_DONE];
    finally
      FStatementUpdateAllField.Reset;
    end;
  end;
end;

function TSqliteAccountDB.DoCheckAccountExists(AccountName: string): Boolean;
begin
  FStatementCheckExists.Reset;
  try
    FStatementCheckExists.OrderBindText(AccountName);
    Result := FStatementCheckExists.Step = SQLITE_ROW;
  finally
    FStatementCheckExists.Reset;
  end;
end;

function TSqliteAccountDB.DoAddAccount(AccountInfo: TAccountInfo): Boolean;
begin
  try
    FStatementNew.Reset;
    FStatementNew.OrderBindText(AccountInfo.AccountName);
    FStatementNew.OrderBindText(AccountInfo.Password);
    FStatementNew.OrderBindText(AccountInfo.UserName);
    FStatementNew.OrderBindText(AccountInfo.IDCard);
    FStatementNew.OrderBindText(AccountInfo.BirthDay);
    FStatementNew.OrderBindText(AccountInfo.Questions1);
    FStatementNew.OrderBindText(AccountInfo.Answers1);
    FStatementNew.OrderBindText(AccountInfo.Questions2);
    FStatementNew.OrderBindText(AccountInfo.Answers2);
    FStatementNew.OrderBindText(AccountInfo.Phone);
    FStatementNew.OrderBindText(AccountInfo.MobilePhone);
    FStatementNew.OrderBindText(AccountInfo.Mail);
    FStatementNew.OrderBindText(AccountInfo.L2Password);
    FStatementNew.OrderBindInt(Date2MyDate(Now));                    // CreateDate
    FStatementNew.OrderBindText(AccountInfo.Memo);
    FStatementNew.OrderBindText(AccountInfo.UID);           
    FStatementNew.OrderBindText(AccountInfo.CID);
    Result := FStatementNew.Step in [SQLITE_OK, SQLITE_DONE];
  finally
    FStatementNew.Reset;
  end;
end;


function TSqliteAccountDB.DoUnLockAccount(AccountName: string): Boolean;
begin
  try
    FStatementUnLockAccount.Reset;
    FStatementUnLockAccount.OrderBindText(AccountName);
    Result := FStatementUnLockAccount.Step in [SQLITE_OK, SQLITE_DONE];
  finally
    FStatementUnLockAccount.Reset;
  end;
end;

procedure TSqliteAccountDB.DoGetAllAccount(AccountList: TStrings);
var
  Ret: Integer;
begin
  AccountList.Clear;
  try
    FStatementGetAllAccount.Reset;
    Ret := FStatementGetAllAccount.Step;
    while (Ret = SQLITE_ROW) do
    begin
      AccountList.AddObject(FStatementGetAllAccount.OrderGetColumnValueText, TObject(FStatementGetAllAccount.OrderGetColumnValueInt));

      Ret := FStatementGetAllAccount.Step;
    end;
  finally
    FStatementGetAllAccount.Reset;
  end;
end;

function TSqliteAccountDB.DoEnabledAccounts(AccountList: TStrings;
  Enabled: Boolean): Boolean;
var
  I: Integer;
begin
  Result := False;
  FDB.BeginTransaction;
  try
    for I := 0 to AccountList.Count - 1 do
    begin
      FStatementEnabledAccount.Reset;
      FStatementEnabledAccount.OrderBindInt(Integer(not Enabled));
      FStatementEnabledAccount.OrderBindText(AccountList.Strings[I]);

      FStatementEnabledAccount.Step;
    end;
    FDB.Commit;

    Result := True;
  except
    FDB.Rollback;
  end;
end;

end.
