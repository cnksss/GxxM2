unit MySqlM2DataDB;

interface

uses
  Windows, Classes, SysUtils, M2DataCommon, Grobal2, MySqlAuctionDB, MySqlStorageDB, MySqlUserShopDB,
  MySqlDataBase, MySqlCli, MySqlCreateTableSql, MySQLWrap;

type
  TMySqlM2DataDB = class(TM2DataDB)
  private
    FLastRequestTick: LongWord;
    FMySqlLib: TMySQLLib;
    FDB: TMySqlDataBase;

    FStatementInsertItems: TMySqlStatement;
    FStatementInsertItemValueAdd: TMySqlStatement;
    FStatementInsertItemElementAdd: TMySqlStatement;
    FStatementInsertItemAddDataByte: TMySqlStatement;
    FStatementInsertItemAddDataInt: TMySqlStatement;
    FStatementInsertItemAddDataText: TMySqlStatement;

    FStatementInsertItemFlute: TMySqlStatement;
    FStatementInsertItemProgress: TMySqlStatement;
    FStatementInsertItemProperty: TMySqlStatement;

    FStatementGetItems: TMySqlStatement;
    FStatementGetItems_Sort: TMySqlStatement;
    FStatementGetItem: TMySqlStatement;
    FStatementGetItemValueAdd: TMySqlStatement;
    FStatementGetItemElementAdd: TMySqlStatement;
    FStatementGetItemAddDataByte: TMySqlStatement;
    FStatementGetItemAddDataInt: TMySqlStatement;
    FStatementGetItemAddDataText: TMySqlStatement;

    FStatementGetItemFlute: TMySqlStatement;
    FStatementGetItemProgress: TMySqlStatement;
    FStatementGetItemProperty: TMySqlStatement;

    procedure OnRequest(Sender: TObject);

    procedure UpdaeDB_1;
    procedure UpdaeDB_2;
    procedure UpdaeDB_3;
    procedure UpdaeDB_4;
    procedure UpdaeDB_5;
  protected
    function GetAuctionDBClass: TAuctionDBClass; override;
    function GetUserShopDBClass: TUserShopDBClass; override;
    function GetStorageDBClass: TStorageDBClass; override;

    procedure DoInit; override;
    procedure DoFinal; override;

    procedure DoLoadItemsFromDB(ParentID, ItemType: Integer; IsSort: Boolean; List: TList); override;
    procedure DoLoadItemFromDB(UserItem: PTUserItem; ParentID, ItemType, ItemIndex: Integer); override;
    procedure DoSaveItemToDB(UserItem: PTUserItem; ParentID, ItemType, ItemIndex: Integer); override;

    function GetDataBase: TObject; override;
  public
    constructor Create; override;
    destructor Destroy; override;

    procedure Run; override;
  end;
  
implementation

uses
  M2Share;

{ TMySqlM2DataDB }

constructor TMySqlM2DataDB.Create;
begin
  inherited;

  FMySqlLib := TMySQLLib.Create(nil);
{$IFDEF WIN32}
  FMySqlLib.Load('', 'libmysql-32.dll');
{$ELSE}
  FMySqlLib.Load('', 'libmysql-64.dll');
{$ENDIF}
  FDB := TMySqlDataBase.Create(FMySqlLib);
  FDB.Init;

  FDB.OnRequest := OnRequest;

  FStatementInsertItems := nil;
  FStatementInsertItemValueAdd := nil;
  FStatementInsertItemElementAdd := nil;
  FStatementInsertItemAddDataByte := nil;
  FStatementInsertItemAddDataInt := nil;
  FStatementInsertItemAddDataText := nil;
  FStatementInsertItemFlute := nil;
  FStatementInsertItemProgress := nil;
  FStatementInsertItemProperty := nil;

  FStatementGetItems := nil;
  FStatementGetItems_Sort := nil;
  FStatementGetItem := nil;
  FStatementGetItemValueAdd := nil;
  FStatementGetItemElementAdd := nil;
  FStatementGetItemAddDataByte := nil;
  FStatementGetItemAddDataInt := nil;
  FStatementGetItemAddDataText := nil;
  FStatementGetItemFlute := nil;
  FStatementGetItemProgress := nil;
  FStatementGetItemProperty := nil;

  FLastRequestTick := MyGetTickCount;
end;

destructor TMySqlM2DataDB.Destroy;
begin
  FDB.Free;
  FMySqlLib.Free;
  inherited;
end;

function TMySqlM2DataDB.GetAuctionDBClass: TAuctionDBClass;
begin
  Result := TMySqlAuctionDB;
end;

function TMySqlM2DataDB.GetStorageDBClass: TStorageDBClass;
begin
  Result := TMySqlStorageDB;
end;

function TMySqlM2DataDB.GetUserShopDBClass: TUserShopDBClass;
begin
  Result := TMySqlUserShopDB;
end;

procedure TMySqlM2DataDB.UpdaeDB_1;
begin
  FDB.StartTransaction;
  try

    FDB.Exec('ALTER TABLE ItemProperty ADD COLUMN `Value2` INTEGER DEFAULT 0;');
    FDB.Exec('ALTER TABLE ItemProperty ADD COLUMN `Value3` INTEGER DEFAULT 0;');

    FDB.Exec('REPLACE INTO db_constant(ConstName, ConstValue) values ("m2data_version", ' + MYSQL_M2DBVERSION  + ');');
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TMySqlM2DataDB.UpdaeDB_2;
begin
  FDB.StartTransaction;
  try

    FDB.Exec('ALTER TABLE ItemFlute ADD COLUMN `OverlapCount` INTEGER DEFAULT 0;');

    FDB.Exec('REPLACE INTO db_constant(ConstName, ConstValue) values ("m2data_version", ' + MYSQL_M2DBVERSION  + ');');
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TMySqlM2DataDB.UpdaeDB_3;
begin
  FDB.StartTransaction;
  try

    FDB.Exec('ALTER TABLE Items ADD COLUMN `NewExpand3` INTEGER DEFAULT 0;');
    FDB.Exec('ALTER TABLE Items ADD COLUMN `NewExpand4` INTEGER DEFAULT 0;');

    FDB.Exec('REPLACE INTO db_constant(ConstName, ConstValue) values ("m2data_version", ' + MYSQL_M2DBVERSION  + ');');
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TMySqlM2DataDB.UpdaeDB_4;
begin
  FDB.StartTransaction;
  try
    FDB.Exec('ALTER TABLE ItemProperty ADD COLUMN `HintModule` INTEGER DEFAULT 0;');
    FDB.Exec('REPLACE INTO db_constant(ConstName, ConstValue) values ("m2data_version", ' + MYSQL_M2DBVERSION  + ');');
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;

procedure TMySqlM2DataDB.UpdaeDB_5;
var
  S: string;
begin
  FDB.StartTransaction;
  try
    S :=
    'drop table IF EXISTS `ItemAddDataByte`;' + sLineBreak +
    'CREATE TABLE `ItemAddDataByte` (' + sLineBreak +
    '`ParentID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`ParentID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +

    'drop table IF EXISTS `ItemAddDataInt`;' + sLineBreak +
    'CREATE TABLE `ItemAddDataInt` (' + sLineBreak +
    '`ParentID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  INTEGER,' + sLineBreak +
    'PRIMARY KEY (`ParentID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak +

    'drop table IF EXISTS `ItemAddDataText`;' + sLineBreak +
    'CREATE TABLE `ItemAddDataText` (' + sLineBreak +
    '`ParentID`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemType`  INTEGER NOT NULL,' + sLineBreak +
    '`ItemIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`ValueIndex`  INTEGER NOT NULL,' + sLineBreak +
    '`Value`  VARCHAR(30),' + sLineBreak +
    'PRIMARY KEY (`ParentID`, `ItemType`, `ItemIndex`, `ValueIndex`)' + sLineBreak +
    ');' + sLineBreak;

    FDB.Exec(S);
    FDB.Exec('REPLACE INTO db_constant(ConstName, ConstValue) values ("m2data_version", ' + MYSQL_M2DBVERSION  + ');');
    FDB.Commit;
  except
    FDB.RollBack;
  end;
end;


procedure TMySqlM2DataDB.DoInit;
var
  sm: TMySqlStatement;
  DB_Version: Integer;
begin
  FDB.Connect(g_sDataSaveDBServer, g_sDataSaveDBUser, g_sDataSaveDBPassword, g_sDataSaveDataBase, g_wDataSaveDBPort, CLIENT_MULTI_STATEMENTS);
  FDB.CharacterSetName := 'utf8';
  FLastRequestTick := MyGetTickCount;

  DB_Version := 0;
  sm := FDB.Statements.AddSQLStatement('get_db_constant_value');
  try
    sm.Sql := 'select ConstValue from db_constant where ConstName = ''m2data_version'';';
    sm.Prepare;
    if sm.Query and sm.Fetch then
    begin
      DB_Version := sm.OrderGetColumnValueInt;
    end;
    sm.Reset;
  finally
    FDB.Statements.Clear;
  end;

  if DB_Version = 20180615  then
  begin
    UpdaeDB_1;
    UpdaeDB_2;
    UpdaeDB_3;
    UpdaeDB_4;
    UpdaeDB_5;
  end
  else if DB_Version = 20190314  then
  begin
    UpdaeDB_2;
    UpdaeDB_3;
    UpdaeDB_4;
    UpdaeDB_5;
  end
  else if DB_Version = 20190606  then
  begin
    UpdaeDB_3;
    UpdaeDB_4;
    UpdaeDB_5;
  end
  else if DB_Version = 20190929  then
  begin
    UpdaeDB_4;
    UpdaeDB_5;
  end
  else if DB_Version = 20200813  then
  begin
    UpdaeDB_5;
  end;

  // 物品表中插入物品
  FStatementInsertItems := FDB.Statements.AddSQLStatement('InsertItems');
  FStatementInsertItems.Sql :=
    'insert into Items(' +
      'ParentID, ' +
      'ItemType, ' +
      'ItemIndex, ' +
      'MakeIndex, ' +
      'DBIndex, ' +
      'Name, ' +
      'Dura, ' +
      'DuraMax, ' +
      'HeroM2DressEffect, ' +
      'UpgradeCount, ' +
      'IsStartTime, ' +
      'LimitTime, '  +
      'HeroM2Light, ' +
      'Color, ' +
      'IsBind, ' +
      'BindOption, ' +
      'Effect, ' +
      'NewLooks, ' +
      'NewShape, ' +
      'FluteCount, ' +
      'PropertyText, ' +
      'PropertyTextColor, ' +
      'ItemFrom, ' +
      'ItemFromMap, ' +
      'ItemFromMon, ' +
      'ItemFromMaker, ' +
      'ItemFromDate, ' +
      'InsuranceCount, ' +
      'NewExpand3, ' +
      'NewExpand4 ' +
      ') ' +
    'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ' +
      '?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';
  FStatementInsertItems.Prepare;


  FStatementInsertItemValueAdd := FDB.Statements.AddSQLStatement('InsertItemValueAdd');
  FStatementInsertItemValueAdd.Sql :=
    'insert into ItemValueAdd(' +
      'ParentID, ' +
      'ItemType, ' +
      'ItemIndex, ' +
      'ValueIndex, ' +
      'Value) ' +
    'values(?, ?, ?, ?, ?);';
  FStatementInsertItemValueAdd.Prepare;

  FStatementInsertItemElementAdd := FDB.Statements.AddSQLStatement('InsertItemElementAdd');
  FStatementInsertItemElementAdd.Sql :=
    'insert into ItemElementAdd(' +
      'ParentID, ' +
      'ItemType, ' +
      'ItemIndex, ' +
      'ValueIndex, ' +
      'Value) ' +
    'values(?, ?, ?, ?, ?);';
  FStatementInsertItemElementAdd.Prepare;

  FStatementInsertItemAddDataByte := FDB.Statements.AddSQLStatement('InsertItemAddDataByte');
  FStatementInsertItemAddDataByte.Sql :=
    'insert into ItemAddDataByte(' +
      'ParentID, ' +
      'ItemType, ' +
      'ItemIndex, ' +
      'ValueIndex, ' +
      'Value) ' +
    'values(?, ?, ?, ?, ?);';
  FStatementInsertItemAddDataByte.Prepare;

  FStatementInsertItemAddDataInt := FDB.Statements.AddSQLStatement('InsertItemAddDataInt');
  FStatementInsertItemAddDataInt.Sql :=
    'insert into ItemAddDataInt(' +
      'ParentID, ' +
      'ItemType, ' +
      'ItemIndex, ' +
      'ValueIndex, ' +
      'Value) ' +
    'values(?, ?, ?, ?, ?);';
  FStatementInsertItemAddDataInt.Prepare;

  FStatementInsertItemAddDataText := FDB.Statements.AddSQLStatement('InsertItemAddDataText');
  FStatementInsertItemAddDataText.Sql :=
    'insert into ItemAddDataText(' +
      'ParentID, ' +
      'ItemType, ' +
      'ItemIndex, ' +
      'ValueIndex, ' +
      'Value) ' +
    'values(?, ?, ?, ?, ?);';
  FStatementInsertItemAddDataText.Prepare;

  FStatementInsertItemFlute := FDB.Statements.AddSQLStatement('InsertItemFlute');
  FStatementInsertItemFlute.Sql :=
    'insert into ItemFlute(' +
      'ParentID, ' +
      'ItemType, ' +
      'ItemIndex, ' +
      'ValueIndex, ' +
      'Value, ' +
      'OverlapCount) ' +
    'values(?, ?, ?, ?, ?, ?);';
  FStatementInsertItemFlute.Prepare;

  FStatementInsertItemProgress := FDB.Statements.AddSQLStatement('InsertItemProgress');
  FStatementInsertItemProgress.Sql :=
    'insert into ItemProgress(' +
      'ParentID, ' +
      'ItemType, ' +
      'ItemIndex, ' +
      'ValueIndex, ' +
      'IsOpen, ' +
      'NameColor, ' +
      'Count, ' +
      'ShowType, ' +
      'Max, ' +
      'Value, ' +
      'Level, ' +
      'Name) ' +
    'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';
  FStatementInsertItemProgress.Prepare;

  FStatementInsertItemProperty := FDB.Statements.AddSQLStatement('InsertItemProperty');
  FStatementInsertItemProperty.Sql :=
    'insert into ItemProperty(' +
      'ParentID, ' +
      'ItemType, ' +
      'ItemIndex, ' +
      'ValueIndex, ' +
      'Color, ' +
      'BindType, ' +
      'ShowFlag, ' +
      'IsPercent, ' +
      'Hintmodule, ' +
      'Value, ' +
      'Value2, ' +
      'Value3) ' +
    'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';
  FStatementInsertItemProperty.Prepare;

  FStatementGetItems := FDB.Statements.AddSQLStatement('SelectItems');
  FStatementGetItems.Sql :=
    'select ' +
      'ItemIndex,' +
      'MakeIndex,' +
      'DBIndex,' +
      'Name,' +
      'Dura,' +
      'DuraMax,' +
      'HeroM2DressEffect,' +
      'UpgradeCount,' +
      'IsStartTime,' +
      'LimitTime,' +
      'HeroM2Light,' +
      'Color,' +
      'IsBind,' +
      'BindOption,' +
      'Effect,' +
      'NewLooks,' +
      'NewShape,' +
      'FluteCount,' +
      'PropertyText,' +
      'PropertyTextColor,' +
      'ItemFrom,' +
      'ItemFromMap,' +
      'ItemFromMon,' +
      'ItemFromMaker,' +
      'ItemFromDate,' +
      'InsuranceCount, ' +
      'NewExpand3, ' +
      'NewExpand4 ' +
    'from Items ' +
    'where ParentID = ? and ItemType = ?;';
  FStatementGetItems.Prepare;

  FStatementGetItems_Sort := FDB.Statements.AddSQLStatement('SelectItems_Sort');
  FStatementGetItems_Sort.Sql :=
    'select ' +
      'ItemIndex,' +
      'MakeIndex,' +
      'DBIndex,' +
      'Name,' +
      'Dura,' +
      'DuraMax,' +
      'HeroM2DressEffect,' +
      'UpgradeCount,' +
      'IsStartTime,' +
      'LimitTime,' +
      'HeroM2Light,' +
      'Color,' +
      'IsBind,' +
      'BindOption,' +
      'Effect,' +
      'NewLooks,' +
      'NewShape,' +
      'FluteCount,' +
      'PropertyText,' +
      'PropertyTextColor,' +
      'ItemFrom,' +
      'ItemFromMap,' +
      'ItemFromMon,' +
      'ItemFromMaker,' +
      'ItemFromDate,' +
      'InsuranceCount, ' +
      'NewExpand3, ' +
      'NewExpand4 ' +
    'from Items ' +
    'where ParentID = ? and ItemType = ? order by DBIndex;';
  FStatementGetItems_Sort.Prepare;


  FStatementGetItem := FDB.Statements.AddSQLStatement('SelectItem');
  FStatementGetItem.Sql :=
    'select ' +
      'MakeIndex,' +
      'DBIndex,' +
      'Name,' +
      'Dura,' +
      'DuraMax,' +
      'HeroM2DressEffect,' +
      'UpgradeCount,' +
      'IsStartTime,' +
      'LimitTime,' +
      'HeroM2Light,' +
      'Color,' +
      'IsBind,' +
      'BindOption,' +
      'Effect,' +
      'NewLooks,' +
      'NewShape,' +
      'FluteCount,' +
      'PropertyText,' +
      'PropertyTextColor,' +
      'ItemFrom,' +
      'ItemFromMap,' +
      'ItemFromMon,' +
      'ItemFromMaker,' +
      'ItemFromDate,' +
      'InsuranceCount, ' +
      'NewExpand3, ' +
      'NewExpand4 ' +
    'from Items ' +
    'where ParentID = ? and ItemType = ? and ItemIndex = ?;';
  FStatementGetItem.Prepare;

  FStatementGetItemValueAdd := FDB.Statements.AddSQLStatement('SelectitemValueAdd');
  FStatementGetItemValueAdd.Sql :=
    'select ' +
      'ValueIndex,' +
      'Value ' +
    'from ItemValueAdd ' +
    'where ParentID = ? and ItemType = ? and ItemIndex = ?;';
  FStatementGetItemValueAdd.Prepare;

  FStatementGetItemElementAdd := FDB.Statements.AddSQLStatement('SelectitemElementAdd');
  FStatementGetItemElementAdd.Sql :=
    'select ' +
      'ValueIndex,' +
      'Value ' +
    'from ItemElementAdd ' +
    'where ParentID = ? and ItemType = ? and ItemIndex = ?;';
  FStatementGetItemElementAdd.Prepare;

  FStatementGetItemAddDataByte := FDB.Statements.AddSQLStatement('SelectitemAddDataByte');
  FStatementGetItemAddDataByte.Sql :=
    'select ' +
      'ValueIndex,' +
      'Value ' +
    'from ItemAddDataByte ' +
    'where ParentID = ? and ItemType = ? and ItemIndex = ?;';
  FStatementGetItemAddDataByte.Prepare;

  FStatementGetItemAddDataInt := FDB.Statements.AddSQLStatement('SelectitemAddDataInt');
  FStatementGetItemAddDataInt.Sql :=
    'select ' +
      'ValueIndex,' +
      'Value ' +
    'from ItemAddDataInt ' +
    'where ParentID = ? and ItemType = ? and ItemIndex = ?;';
  FStatementGetItemAddDataInt.Prepare;

  FStatementGetItemAddDataText := FDB.Statements.AddSQLStatement('SelectitemAddDataText');
  FStatementGetItemAddDataText.Sql :=
    'select ' +
      'ValueIndex,' +
      'Value ' +
    'from ItemAddDataText ' +
    'where ParentID = ? and ItemType = ? and ItemIndex = ?;';
  FStatementGetItemAddDataText.Prepare;

  FStatementGetItemFlute := FDB.Statements.AddSQLStatement('SelectitemFlute');
  FStatementGetItemFlute.Sql :=
    'select ' +
      'ValueIndex,' +
      'Value, ' +
      'OverlapCount ' +
    'from ItemFlute ' +
    'where ParentID = ? and ItemType = ? and ItemIndex = ?;';
  FStatementGetItemFlute.Prepare;

  FStatementGetItemProgress := FDB.Statements.AddSQLStatement('SelectItemProgress');
  FStatementGetItemProgress.Sql :=
    'select ' +
      'ValueIndex,' +
      'IsOpen,' +
      'NameColor,' +
      'Count,' +
      'ShowType,' +
      'Max,' +
      'Value,' +
      'Level,' +
      'Name ' +
    'from ItemProgress ' +
    'where ParentID = ? and ItemType = ? and ItemIndex = ?;';
  FStatementGetItemProgress.Prepare;

  FStatementGetItemProperty := FDB.Statements.AddSQLStatement('SelectItemProperty');
  FStatementGetItemProperty.Sql :=
    'select ' +
      'ValueIndex,' +
      'Color,' +
      'BindType,' +
      'ShowFlag,' +
      'IsPercent,' +
      'Hintmodule,' +
      'Value,' +
      'Value2,' +
      'Value3 ' +
    'from ItemProperty ' +
    'where ParentID = ? and ItemType = ? and ItemIndex = ?;';
  FStatementGetItemProperty.Prepare;
end;

procedure TMySqlM2DataDB.DoFinal;
begin
  inherited;
  if FStatementInsertItems <> nil then
  begin
    FStatementInsertItems.Finalize;
    FStatementInsertItems := nil;
  end;

  if FStatementInsertItemValueAdd <> nil then 
  begin
    FStatementInsertItemValueAdd.Finalize;
    FStatementInsertItemValueAdd := nil;
  end;

  if FStatementInsertItemElementAdd <> nil then
  begin
    FStatementInsertItemElementAdd.Finalize;
    FStatementInsertItemElementAdd := nil;
  end;

  if FStatementInsertItemAddDataByte <> nil then
  begin
    FStatementInsertItemAddDataByte.Finalize;
    FStatementInsertItemAddDataByte := nil;
  end;

  if FStatementInsertItemAddDataInt <> nil then
  begin
    FStatementInsertItemAddDataInt.Finalize;
    FStatementInsertItemAddDataInt := nil;
  end;

  if FStatementInsertItemAddDataText <> nil then
  begin
    FStatementInsertItemAddDataText.Finalize;
    FStatementInsertItemAddDataText := nil;
  end;

  if FStatementInsertItemFlute <> nil then 
  begin
    FStatementInsertItemFlute.Finalize;
    FStatementInsertItemFlute := nil;
  end;

  if FStatementInsertItemProgress <> nil then 
  begin
    FStatementInsertItemProgress.Finalize;
    FStatementInsertItemProgress := nil;
  end;

  if FStatementInsertItemProperty <> nil then 
  begin
    FStatementInsertItemProperty.Finalize;
    FStatementInsertItemProperty := nil;
  end;

  if FStatementGetItem <> nil then 
  begin
    FStatementGetItem.Finalize;
    FStatementGetItem := nil;
  end;

  if FStatementGetItemValueAdd <> nil then 
  begin
    FStatementGetItemValueAdd.Finalize;
    FStatementGetItemValueAdd := nil;
  end;

  if FStatementGetItemElementAdd <> nil then
  begin
    FStatementGetItemElementAdd.Finalize;
    FStatementGetItemElementAdd := nil;
  end;

  if FStatementGetItemAddDataByte <> nil then
  begin
    FStatementGetItemAddDataByte.Finalize;
    FStatementGetItemAddDataByte := nil;
  end;

  if FStatementGetItemAddDataInt <> nil then
  begin
    FStatementGetItemAddDataInt.Finalize;
    FStatementGetItemAddDataInt := nil;
  end;

  if FStatementGetItemAddDataText <> nil then
  begin
    FStatementGetItemAddDataText.Finalize;
    FStatementGetItemAddDataText := nil;
  end;

  if FStatementGetItemFlute <> nil then
  begin
    FStatementGetItemFlute.Finalize;
    FStatementGetItemFlute := nil;
  end;

  if FStatementGetItemProgress <> nil then 
  begin
    FStatementGetItemProgress.Finalize;
    FStatementGetItemProgress := nil;
  end;

  if FStatementGetItemProperty <> nil then 
  begin
    FStatementGetItemProperty.Finalize;
    FStatementGetItemProperty := nil;
  end;
end;

procedure TMySqlM2DataDB.DoLoadItemsFromDB(ParentID, ItemType: Integer; IsSort: Boolean;
  List: TList);
var
  Statement: TMySqlStatement;

  ItemIndex: Integer;
  Index2, Value: Integer;
  UserItem: TUserItem;
  PUserItem: pTUserItem;
  sTemp: string;
begin
  if IsSort then
    Statement := FStatementGetItems_Sort
  else
    Statement := FStatementGetItems;

  Statement.Reset;
  Statement.OrderBindParamInt(ParentID);
  Statement.OrderBindParamInt(ItemType);
  if Statement.Query then
  begin
    while (Statement.Fetch) do
    begin
      FillChar(UserItem, SizeOf(UserItem), 0);

      ItemIndex := Statement.OrderGetColumnValueInt;
      UserItem.MakeIndex := Statement.OrderGetColumnValueInt;
      UserItem.wIndex := Statement.OrderGetColumnValueInt;
      UserItem.Name := Statement.OrderGetColumnValueText;
      UserItem.Dura := Statement.OrderGetColumnValueInt;
      UserItem.DuraMax := Statement.OrderGetColumnValueInt;
      UserItem.dwHeroM2DressEffect := Statement.OrderGetColumnValueInt;
      UserItem.btUpgradeCount := Statement.OrderGetColumnValueInt;
      UserItem.boStartTime := Statement.OrderGetColumnValueBool;
      UserItem.nLimitTime := Statement.OrderGetColumnValueInt;
      UserItem.btHeroM2Light := Statement.OrderGetColumnValueInt;
      UserItem.btColor := Statement.OrderGetColumnValueInt;
      UserItem.boIsBind := Statement.OrderGetColumnValueBool;
      UserItem.btBindOption := Statement.OrderGetColumnValueInt;
      UserItem.wEffect := Statement.OrderGetColumnValueInt;
      UserItem.wNewLooks := Statement.OrderGetColumnValueInt;
      UserItem.wNewShape := Statement.OrderGetColumnValueInt;
      UserItem.btFluteCount := Statement.OrderGetColumnValueInt;
      UserItem.CustomProperty.sText := Statement.OrderGetColumnValueText;
      UserItem.CustomProperty.btTextColor := Statement.OrderGetColumnValueInt;
      UserItem.ItemFrom.ItemForm := TItemFormType(Statement.OrderGetColumnValueInt);
      UserItem.ItemFrom.sMapName := Statement.OrderGetColumnValueText;
      UserItem.ItemFrom.sMonName := Statement.OrderGetColumnValueText;
      UserItem.ItemFrom.sMakerName := Statement.OrderGetColumnValueText;
      UserItem.ItemFrom.DateTime := Statement.OrderGetColumnValueDouble;
      UserItem.wInsuranceCount := Statement.OrderGetColumnValueInt;
      UserItem.wNewExpand3 := Statement.OrderGetColumnValueInt;
      UserItem.wNewExpand4 := Statement.OrderGetColumnValueInt;

      FStatementGetItemValueAdd.Reset;
      FStatementGetItemValueAdd.OrderBindParamInt(ParentID);
      FStatementGetItemValueAdd.OrderBindParamInt(ItemType);
      FStatementGetItemValueAdd.OrderBindParamInt(ItemIndex);
      if FStatementGetItemValueAdd.Query then
      begin
        while (FStatementGetItemValueAdd.Fetch) do
        begin
          Index2 := FStatementGetItemValueAdd.OrderGetColumnValueInt;
          if (Index2 >= Low(UserItem.btValue)) and (Index2 <= High(UserItem.btValue)) then
          begin
            Value := FStatementGetItemValueAdd.OrderGetColumnValueInt;
            UserItem.btValue[Index2] := Value;
          end;
        end;
      end;
      FStatementGetItemValueAdd.Reset;

      FStatementGetItemElementAdd.Reset;
      FStatementGetItemElementAdd.OrderBindParamInt(ParentID);
      FStatementGetItemElementAdd.OrderBindParamInt(ItemType);
      FStatementGetItemElementAdd.OrderBindParamInt(ItemIndex);
      if FStatementGetItemElementAdd.Query then
      begin
        while (FStatementGetItemElementAdd.Fetch) do
        begin
          Index2 := FStatementGetItemElementAdd.OrderGetColumnValueInt;
          if (Index2 >= Low(UserItem.btNewValue)) and (Index2 <= High(UserItem.btNewValue)) then
          begin
            Value := FStatementGetItemElementAdd.OrderGetColumnValueInt;
            UserItem.btNewValue[Index2] := Value;
          end;
        end;
      end;
      FStatementGetItemElementAdd.Reset;

      FStatementGetItemAddDataByte.Reset;
      FStatementGetItemAddDataByte.OrderBindParamInt(ParentID);
      FStatementGetItemAddDataByte.OrderBindParamInt(ItemType);
      FStatementGetItemAddDataByte.OrderBindParamInt(ItemIndex);
      if FStatementGetItemAddDataByte.Query then
      begin
        while (FStatementGetItemAddDataByte.Fetch) do
        begin
          Index2 := FStatementGetItemAddDataByte.OrderGetColumnValueInt;
          if (Index2 >= Low(UserItem.btAddDataByte)) and (Index2 <= High(UserItem.btAddDataByte)) then
          begin
            Value := FStatementGetItemAddDataByte.OrderGetColumnValueInt;
            UserItem.btAddDataByte[Index2] := Value;
          end;
        end;
      end;
      FStatementGetItemAddDataByte.Reset;

      
      FStatementGetItemAddDataInt.Reset;
      FStatementGetItemAddDataInt.OrderBindParamInt(ParentID);
      FStatementGetItemAddDataInt.OrderBindParamInt(ItemType);
      FStatementGetItemAddDataInt.OrderBindParamInt(ItemIndex);
      if FStatementGetItemAddDataInt.Query then
      begin
        while (FStatementGetItemAddDataInt.Fetch) do
        begin
          Index2 := FStatementGetItemAddDataInt.OrderGetColumnValueInt;
          if (Index2 >= Low(UserItem.nAddDataInt)) and (Index2 <= High(UserItem.nAddDataInt)) then
          begin
            Value := FStatementGetItemAddDataInt.OrderGetColumnValueInt;
            UserItem.nAddDataInt[Index2] := Value;
          end;
        end;
      end;
      FStatementGetItemAddDataInt.Reset;

      FStatementGetItemAddDataText.Reset;
      FStatementGetItemAddDataText.OrderBindParamInt(ParentID);
      FStatementGetItemAddDataText.OrderBindParamInt(ItemType);
      FStatementGetItemAddDataText.OrderBindParamInt(ItemIndex);
      if FStatementGetItemAddDataText.Query then
      begin
        while (FStatementGetItemAddDataText.Fetch) do
        begin
          Index2 := FStatementGetItemAddDataText.OrderGetColumnValueInt;
          if (Index2 >= Low(UserItem.sAddDataText)) and (Index2 <= High(UserItem.sAddDataText)) then
          begin
            sTemp := FStatementGetItemAddDataText.OrderGetColumnValueText;
            UserItem.sAddDataText[Index2] := sTemp;
          end;
        end;
      end;
      FStatementGetItemAddDataText.Reset;

      FStatementGetItemFlute.Reset;
      FStatementGetItemFlute.OrderBindParamInt(ParentID);
      FStatementGetItemFlute.OrderBindParamInt(0);
      FStatementGetItemFlute.OrderBindParamInt(ItemIndex);
      if FStatementGetItemFlute.Query then
      begin
        while (FStatementGetItemFlute.Fetch) do
        begin
          Index2 := FStatementGetItemFlute.OrderGetColumnValueInt;
          if (Index2 >= Low(UserItem.Flutes)) and (Index2 <= High(UserItem.Flutes)) then
          begin
            UserItem.Flutes[Index2].GemIndex := FStatementGetItemFlute.OrderGetColumnValueInt;
            UserItem.Flutes[Index2].GemCount := FStatementGetItemFlute.OrderGetColumnValueInt;

            if (UserItem.Flutes[Index2].GemIndex > 0) and (UserItem.Flutes[Index2].GemCount = 0) then
              UserItem.Flutes[Index2].GemCount := 1;
          end;
        end;
      end;
      FStatementGetItemFlute.Reset;

      FStatementGetItemProgress.Reset;
      FStatementGetItemProgress.OrderBindParamInt(ParentID);
      FStatementGetItemProgress.OrderBindParamInt(ItemType);
      FStatementGetItemProgress.OrderBindParamInt(ItemIndex);
      if FStatementGetItemProgress.Query then
      begin
        while (FStatementGetItemProgress.Fetch) do
        begin
          Index2 := FStatementGetItemProgress.OrderGetColumnValueInt;
          if (Index2 >= Low(UserItem.Progress)) and (Index2 <= High(UserItem.Progress)) then
          begin
            UserItem.Progress[Index2].boOpen := FStatementGetItemProgress.OrderGetColumnValueBool;
            UserItem.Progress[Index2].btNameColor := FStatementGetItemProgress.OrderGetColumnValueInt;
            UserItem.Progress[Index2].btCount := FStatementGetItemProgress.OrderGetColumnValueInt;
            UserItem.Progress[Index2].btShowType := FStatementGetItemProgress.OrderGetColumnValueInt;
            UserItem.Progress[Index2].wMax := FStatementGetItemProgress.OrderGetColumnValueInt;
            UserItem.Progress[Index2].wValue := FStatementGetItemProgress.OrderGetColumnValueInt;
            UserItem.Progress[Index2].wLevel := FStatementGetItemProgress.OrderGetColumnValueInt;
            UserItem.Progress[Index2].sName := FStatementGetItemProgress.OrderGetColumnValueText;
          end;
        end;
      end;

      FStatementGetItemProgress.Reset;

      FStatementGetItemProperty.Reset;
      FStatementGetItemProperty.OrderBindParamInt(ParentID);
      FStatementGetItemProperty.OrderBindParamInt(ItemType);
      FStatementGetItemProperty.OrderBindParamInt(ItemIndex);
      if FStatementGetItemProperty.Query then
      begin
        while (FStatementGetItemProperty.Fetch) do
        begin
          Index2 := FStatementGetItemProperty.OrderGetColumnValueInt;
          if (Index2 >= Low(UserItem.CustomProperty.Properties)) and (Index2 <= High(UserItem.CustomProperty.Properties)) then
          begin
            UserItem.CustomProperty.Properties[Index2].btColor := FStatementGetItemProperty.OrderGetColumnValueInt;
            UserItem.CustomProperty.Properties[Index2].btBindType := FStatementGetItemProperty.OrderGetColumnValueInt;
            UserItem.CustomProperty.Properties[Index2].btShowFlag := FStatementGetItemProperty.OrderGetColumnValueInt;
            UserItem.CustomProperty.Properties[Index2].btPercent := FStatementGetItemProperty.OrderGetColumnValueInt;
            UserItem.CustomProperty.Properties[Index2].btHintmodule := FStatementGetItemProperty.OrderGetColumnValueInt;
            UserItem.CustomProperty.Properties[Index2].nValues[0] := FStatementGetItemProperty.OrderGetColumnValueInt;
            UserItem.CustomProperty.Properties[Index2].nValues[1] := FStatementGetItemProperty.OrderGetColumnValueInt;
            UserItem.CustomProperty.Properties[Index2].nValues[2] := FStatementGetItemProperty.OrderGetColumnValueInt;
          end;
        end;
      end;
      FStatementGetItemProperty.Reset;

      New(PUserItem);
      PUserItem^ := UserItem;
      List.Add(PUserItem);
    end;
  end;
end;

procedure TMySqlM2DataDB.DoLoadItemFromDB(UserItem: PTUserItem;
  ParentID, ItemType, ItemIndex: Integer);
var
  Index2, Value: Integer;
  sTemp: string;
begin
  FillChar(UserItem^, SizeOf(TUserItem), 0);
  FStatementGetItem.Reset;
  FStatementGetItem.OrderBindParamInt(ParentID);
  FStatementGetItem.OrderBindParamInt(ItemType);
  FStatementGetItem.OrderBindParamInt(ItemIndex);
  if FStatementGetItem.Query and FStatementGetItem.Fetch then
  begin
    UserItem.MakeIndex := FStatementGetItem.OrderGetColumnValueInt;
    UserItem.wIndex := FStatementGetItem.OrderGetColumnValueInt;
    UserItem.Name := FStatementGetItem.OrderGetColumnValueText;
    UserItem.Dura := FStatementGetItem.OrderGetColumnValueInt;
    UserItem.DuraMax := FStatementGetItem.OrderGetColumnValueInt;
    UserItem.dwHeroM2DressEffect := FStatementGetItem.OrderGetColumnValueInt;
    UserItem.btUpgradeCount := FStatementGetItem.OrderGetColumnValueInt;
    UserItem.boStartTime := FStatementGetItem.OrderGetColumnValueBool;
    UserItem.nLimitTime := FStatementGetItem.OrderGetColumnValueInt;
    UserItem.btHeroM2Light := FStatementGetItem.OrderGetColumnValueInt;
    UserItem.btColor := FStatementGetItem.OrderGetColumnValueInt;
    UserItem.boIsBind := FStatementGetItem.OrderGetColumnValueBool;
    UserItem.btBindOption := FStatementGetItem.OrderGetColumnValueInt;
    UserItem.wEffect := FStatementGetItem.OrderGetColumnValueInt;
    UserItem.wNewLooks := FStatementGetItem.OrderGetColumnValueInt;
    UserItem.wNewShape := FStatementGetItem.OrderGetColumnValueInt;
    UserItem.btFluteCount := FStatementGetItem.OrderGetColumnValueInt;
    UserItem.CustomProperty.sText := FStatementGetItem.OrderGetColumnValueText;
    UserItem.CustomProperty.btTextColor := FStatementGetItem.OrderGetColumnValueInt;
    UserItem.ItemFrom.ItemForm := TItemFormType(FStatementGetItem.OrderGetColumnValueInt);
    UserItem.ItemFrom.sMapName := FStatementGetItem.OrderGetColumnValueText;
    UserItem.ItemFrom.sMonName := FStatementGetItem.OrderGetColumnValueText;
    UserItem.ItemFrom.sMakerName := FStatementGetItem.OrderGetColumnValueText;
    UserItem.ItemFrom.DateTime := FStatementGetItem.OrderGetColumnValueDouble;
    UserItem.wInsuranceCount := FStatementGetItem.OrderGetColumnValueInt;
    UserItem.wNewExpand3 := FStatementGetItem.OrderGetColumnValueInt;
    UserItem.wNewExpand4 := FStatementGetItem.OrderGetColumnValueInt;
  end;
  FStatementGetItem.Reset;

  FStatementGetItemValueAdd.Reset;
  FStatementGetItemValueAdd.OrderBindParamInt(ParentID);
  FStatementGetItemValueAdd.OrderBindParamInt(ItemType);
  FStatementGetItemValueAdd.OrderBindParamInt(ItemIndex);

  if FStatementGetItemValueAdd.Query then
  begin
    while (FStatementGetItemValueAdd.Fetch) do
    begin
      Index2 := FStatementGetItemValueAdd.OrderGetColumnValueInt;
      if (Index2 >= Low(UserItem.btValue)) and (Index2 <= High(UserItem.btValue)) then
      begin
        Value := FStatementGetItemValueAdd.OrderGetColumnValueInt;
        UserItem.btValue[Index2] := Value;
      end;
    end;
  end;

  FStatementGetItemValueAdd.Reset;

  FStatementGetItemElementAdd.Reset;
  FStatementGetItemElementAdd.OrderBindParamInt(ParentID);
  FStatementGetItemElementAdd.OrderBindParamInt(ItemType);
  FStatementGetItemElementAdd.OrderBindParamInt(ItemIndex);
  if FStatementGetItemElementAdd.Query then
  begin
    while (FStatementGetItemElementAdd.Fetch) do
    begin
      Index2 := FStatementGetItemElementAdd.OrderGetColumnValueInt;
      if (Index2 >= Low(UserItem.btNewValue)) and (Index2 <= High(UserItem.btNewValue)) then
      begin
        Value := FStatementGetItemElementAdd.OrderGetColumnValueInt;
        UserItem.btNewValue[Index2] := Value;
      end;
    end;
  end;
  FStatementGetItemElementAdd.Reset;

  FStatementGetItemAddDataByte.Reset;
  FStatementGetItemAddDataByte.OrderBindParamInt(ParentID);
  FStatementGetItemAddDataByte.OrderBindParamInt(ItemType);
  FStatementGetItemAddDataByte.OrderBindParamInt(ItemIndex);
  if FStatementGetItemAddDataByte.Query then
  begin
    while (FStatementGetItemAddDataByte.Fetch) do
    begin
      Index2 := FStatementGetItemAddDataByte.OrderGetColumnValueInt;
      if (Index2 >= Low(UserItem.btAddDataByte)) and (Index2 <= High(UserItem.btAddDataByte)) then
      begin
        Value := FStatementGetItemAddDataByte.OrderGetColumnValueInt;
        UserItem.btAddDataByte[Index2] := Value;
      end;
    end;
  end;
  FStatementGetItemAddDataByte.Reset;

  FStatementGetItemAddDataInt.Reset;
  FStatementGetItemAddDataInt.OrderBindParamInt(ParentID);
  FStatementGetItemAddDataInt.OrderBindParamInt(ItemType);
  FStatementGetItemAddDataInt.OrderBindParamInt(ItemIndex);
  if FStatementGetItemAddDataInt.Query then
  begin
    while (FStatementGetItemAddDataInt.Fetch) do
    begin
      Index2 := FStatementGetItemAddDataInt.OrderGetColumnValueInt;
      if (Index2 >= Low(UserItem.nAddDataInt)) and (Index2 <= High(UserItem.nAddDataInt)) then
      begin
        Value := FStatementGetItemAddDataInt.OrderGetColumnValueInt;
        UserItem.nAddDataInt[Index2] := Value;
      end;
    end;
  end;
  FStatementGetItemAddDataInt.Reset;

  FStatementGetItemAddDataText.Reset;
  FStatementGetItemAddDataText.OrderBindParamInt(ParentID);
  FStatementGetItemAddDataText.OrderBindParamInt(ItemType);
  FStatementGetItemAddDataText.OrderBindParamInt(ItemIndex);
  if FStatementGetItemAddDataText.Query then
  begin
    while (FStatementGetItemAddDataText.Fetch) do
    begin
      Index2 := FStatementGetItemAddDataText.OrderGetColumnValueInt;
      if (Index2 >= Low(UserItem.sAddDataText)) and (Index2 <= High(UserItem.sAddDataText)) then
      begin
        sTemp := FStatementGetItemAddDataText.OrderGetColumnValueText;
        UserItem.sAddDataText[Index2] := sTemp;
      end;
    end;
  end;
  FStatementGetItemAddDataText.Reset;

  FStatementGetItemFlute.Reset;
  FStatementGetItemFlute.OrderBindParamInt(ParentID);
  FStatementGetItemFlute.OrderBindParamInt(ItemType);
  FStatementGetItemFlute.OrderBindParamInt(ItemIndex);

  if FStatementGetItemFlute.Query then
  begin
    while (FStatementGetItemFlute.Fetch) do
    begin
      Index2 := FStatementGetItemFlute.OrderGetColumnValueInt;
      if (Index2 >= Low(UserItem.Flutes)) and (Index2 <= High(UserItem.Flutes)) then
      begin
        UserItem.Flutes[Index2].GemIndex := FStatementGetItemFlute.OrderGetColumnValueInt;
        UserItem.Flutes[Index2].GemCount := FStatementGetItemFlute.OrderGetColumnValueInt;

        if (UserItem.Flutes[Index2].GemIndex > 0) and (UserItem.Flutes[Index2].GemCount = 0) then
          UserItem.Flutes[Index2].GemCount := 1;
      end;
    end;
  end;
  FStatementGetItemFlute.Reset;

  FStatementGetItemProgress.Reset;
  FStatementGetItemProgress.OrderBindParamInt(ParentID);
  FStatementGetItemProgress.OrderBindParamInt(ItemType);
  FStatementGetItemProgress.OrderBindParamInt(ItemIndex);

  if FStatementGetItemProgress.Query then
  begin
    while (FStatementGetItemProgress.Fetch) do
    begin
      Index2 := FStatementGetItemProgress.OrderGetColumnValueInt;
      if (Index2 >= Low(UserItem.Progress)) and (Index2 <= High(UserItem.Progress)) then
      begin
        UserItem.Progress[Index2].boOpen := FStatementGetItemProgress.OrderGetColumnValueBool;
        UserItem.Progress[Index2].btNameColor := FStatementGetItemProgress.OrderGetColumnValueInt;
        UserItem.Progress[Index2].btCount := FStatementGetItemProgress.OrderGetColumnValueInt;
        UserItem.Progress[Index2].btShowType := FStatementGetItemProgress.OrderGetColumnValueInt;
        UserItem.Progress[Index2].wMax := FStatementGetItemProgress.OrderGetColumnValueInt;
        UserItem.Progress[Index2].wValue := FStatementGetItemProgress.OrderGetColumnValueInt;
        UserItem.Progress[Index2].wLevel := FStatementGetItemProgress.OrderGetColumnValueInt;
        UserItem.Progress[Index2].sName := FStatementGetItemProgress.OrderGetColumnValueText;
      end;
    end;
  end;
  FStatementGetItemProgress.Reset;

  FStatementGetItemProperty.Reset;
  FStatementGetItemProperty.OrderBindParamInt(ParentID);
  FStatementGetItemProperty.OrderBindParamInt(ItemType);
  FStatementGetItemProperty.OrderBindParamInt(ItemIndex);

  if FStatementGetItemProperty.Query then
  begin
    while (FStatementGetItemProperty.Fetch) do
    begin
      Index2 := FStatementGetItemProperty.OrderGetColumnValueInt;
      if (Index2 >= Low(UserItem.CustomProperty.Properties)) and (Index2 <= High(UserItem.CustomProperty.Properties)) then
      begin
        UserItem.CustomProperty.Properties[Index2].btColor := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].btBindType := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].btShowFlag := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].btPercent := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].btHintmodule := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].nValues[0] := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].nValues[1] := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].nValues[2] := FStatementGetItemProperty.OrderGetColumnValueInt;
      end;
    end;
  end;

  FStatementGetItemProperty.Reset;
end;

procedure TMySqlM2DataDB.DoSaveItemToDB(UserItem: PTUserItem;
  ParentID, ItemType, ItemIndex: Integer);
var
  J: Integer;
begin
  FStatementInsertItems.Reset;
  try
    FStatementInsertItems.OrderBindParamInt(ParentID);
    FStatementInsertItems.OrderBindParamInt(ItemType);
    FStatementInsertItems.OrderBindParamInt(ItemIndex);
    FStatementInsertItems.OrderBindParamInt(UserItem.MakeIndex);
    FStatementInsertItems.OrderBindParamInt(UserItem.wIndex);
    FStatementInsertItems.OrderBindParamText(UserItem.Name);
    FStatementInsertItems.OrderBindParamInt(UserItem.Dura);
    FStatementInsertItems.OrderBindParamInt(UserItem.DuraMax);
    FStatementInsertItems.OrderBindParamInt(UserItem.dwHeroM2DressEffect);
    FStatementInsertItems.OrderBindParamInt(UserItem.btUpgradeCount);
    FStatementInsertItems.OrderBindParamBool(UserItem.boStartTime);
    FStatementInsertItems.OrderBindParamInt(UserItem.nLimitTime);
    FStatementInsertItems.OrderBindParamInt(UserItem.btHeroM2Light);
    FStatementInsertItems.OrderBindParamInt(UserItem.btColor);
    FStatementInsertItems.OrderBindParamBool(UserItem.boIsBind);
    FStatementInsertItems.OrderBindParamInt(UserItem.btBindOption);
    FStatementInsertItems.OrderBindParamInt(UserItem.wEffect);
    FStatementInsertItems.OrderBindParamInt(UserItem.wNewLooks);
    FStatementInsertItems.OrderBindParamInt(UserItem.wNewShape);
    FStatementInsertItems.OrderBindParamInt(UserItem.btFluteCount);
    FStatementInsertItems.OrderBindParamText(UserItem.CustomProperty.sText);
    FStatementInsertItems.OrderBindParamInt(UserItem.CustomProperty.btTextColor);
    FStatementInsertItems.OrderBindParamInt(Integer(UserItem.ItemFrom.ItemForm));
    FStatementInsertItems.OrderBindParamText(UserItem.ItemFrom.sMapName);
    FStatementInsertItems.OrderBindParamText(UserItem.ItemFrom.sMonName);
    FStatementInsertItems.OrderBindParamText(UserItem.ItemFrom.sMakerName);
    FStatementInsertItems.OrderBindParamDouble(UserItem.ItemFrom.DateTime);
    FStatementInsertItems.OrderBindParamInt(UserItem.wInsuranceCount);
    FStatementInsertItems.OrderBindParamInt(UserItem.wNewExpand3);
    FStatementInsertItems.OrderBindParamInt(UserItem.wNewExpand4);
    FStatementInsertItems.Step;
  finally
    FStatementInsertItems.Reset;
  end;


  //---------------------------------------------------------------------------------------------

  try
    for J := Low(UserItem.btValue) to High(UserItem.btValue) do
    begin
      if UserItem.btValue[J] <> 0 then
      begin
        FStatementInsertItemValueAdd.Reset;
        FStatementInsertItemValueAdd.OrderBindParamInt(ParentID);
        FStatementInsertItemValueAdd.OrderBindParamInt(ItemType);
        FStatementInsertItemValueAdd.OrderBindParamInt(ItemIndex);
        FStatementInsertItemValueAdd.OrderBindParamInt(J);
        FStatementInsertItemValueAdd.OrderBindParamInt(UserItem.btValue[J]);
        FStatementInsertItemValueAdd.Step;
      end;
    end;
  finally
    FStatementInsertItemValueAdd.Reset;
  end;

  try
    for J := Low(UserItem.btNewValue) to High(UserItem.btNewValue) do
    begin
      if UserItem.btNewValue[J] <> 0 then
      begin
        FStatementInsertItemElementAdd.Reset;
        FStatementInsertItemElementAdd.OrderBindParamInt(ParentID);
        FStatementInsertItemElementAdd.OrderBindParamInt(ItemType);
        FStatementInsertItemElementAdd.OrderBindParamInt(ItemIndex);
        FStatementInsertItemElementAdd.OrderBindParamInt(J);
        FStatementInsertItemElementAdd.OrderBindParamInt(UserItem.btNewValue[J]);
        FStatementInsertItemElementAdd.Step;
      end;
    end;
  finally
    FStatementInsertItemElementAdd.Reset;
  end;

  try
    for J := Low(UserItem.btAddDataByte) to High(UserItem.btAddDataByte) do
    begin
      if UserItem.btAddDataByte[J] <> 0 then
      begin
        FStatementInsertItemAddDataByte.Reset;
        FStatementInsertItemAddDataByte.OrderBindParamInt(ParentID);
        FStatementInsertItemAddDataByte.OrderBindParamInt(ItemType);
        FStatementInsertItemAddDataByte.OrderBindParamInt(ItemIndex);
        FStatementInsertItemAddDataByte.OrderBindParamInt(J);
        FStatementInsertItemAddDataByte.OrderBindParamInt(UserItem.btAddDataByte[J]);
        FStatementInsertItemAddDataByte.Step;
      end;
    end;
  finally
    FStatementInsertItemAddDataByte.Reset;
  end;
  
  try
    for J := Low(UserItem.nAddDataInt) to High(UserItem.nAddDataInt) do
    begin
      if UserItem.nAddDataInt[J] <> 0 then
      begin
        FStatementInsertItemAddDataInt.Reset;
        FStatementInsertItemAddDataInt.OrderBindParamInt(ParentID);
        FStatementInsertItemAddDataInt.OrderBindParamInt(ItemType);
        FStatementInsertItemAddDataInt.OrderBindParamInt(ItemIndex);
        FStatementInsertItemAddDataInt.OrderBindParamInt(J);
        FStatementInsertItemAddDataInt.OrderBindParamInt(UserItem.nAddDataInt[J]);
        FStatementInsertItemAddDataInt.Step;
      end;
    end;
  finally
    FStatementInsertItemAddDataInt.Reset;
  end;

  try
    for J := Low(UserItem.sAddDataText) to High(UserItem.sAddDataText) do
    begin
      if UserItem.sAddDataText[J] <> '' then
      begin
        FStatementInsertItemAddDataText.Reset;
        FStatementInsertItemAddDataText.OrderBindParamInt(ParentID);
        FStatementInsertItemAddDataText.OrderBindParamInt(ItemType);
        FStatementInsertItemAddDataText.OrderBindParamInt(ItemIndex);
        FStatementInsertItemAddDataText.OrderBindParamInt(J);
        FStatementInsertItemAddDataText.OrderBindParamText(UserItem.sAddDataText[J]);
        FStatementInsertItemAddDataText.Step;
      end;
    end;
  finally
    FStatementInsertItemAddDataText.Reset;
  end;



  try
    for J := Low(UserItem.Flutes) to High(UserItem.Flutes) do
    begin
      if UserItem.Flutes[J].GemIndex <> 0 then
      begin
        FStatementInsertItemFlute.Reset;
        FStatementInsertItemFlute.OrderBindParamInt(ParentID);
        FStatementInsertItemFlute.OrderBindParamInt(ItemType);
        FStatementInsertItemFlute.OrderBindParamInt(ItemIndex);
        FStatementInsertItemFlute.OrderBindParamInt(J);
        FStatementInsertItemFlute.OrderBindParamInt(UserItem.Flutes[J].GemIndex);
        FStatementInsertItemFlute.OrderBindParamInt(UserItem.Flutes[J].GemCount);
        FStatementInsertItemFlute.Step;
      end;
    end;
  finally
    FStatementInsertItemFlute.Reset;
  end;

  try
    for J := Low(UserItem.Progress) to High(UserItem.Progress) do
    begin
      if UserItem.Progress[J].boOpen then
      begin
        FStatementInsertItemProgress.Reset;
        FStatementInsertItemProgress.OrderBindParamInt(ParentID);
        FStatementInsertItemProgress.OrderBindParamInt(ItemType);
        FStatementInsertItemProgress.OrderBindParamInt(ItemIndex);
        FStatementInsertItemProgress.OrderBindParamInt(J);
        FStatementInsertItemProgress.OrderBindParamBool(UserItem.Progress[J].boOpen);
        FStatementInsertItemProgress.OrderBindParamInt(UserItem.Progress[J].btNameColor);
        FStatementInsertItemProgress.OrderBindParamInt(UserItem.Progress[J].btCount);
        FStatementInsertItemProgress.OrderBindParamInt(UserItem.Progress[J].btShowType);
        FStatementInsertItemProgress.OrderBindParamInt(UserItem.Progress[J].wMax);
        FStatementInsertItemProgress.OrderBindParamInt(UserItem.Progress[J].wValue);
        FStatementInsertItemProgress.OrderBindParamInt(UserItem.Progress[J].wLevel);
        FStatementInsertItemProgress.OrderBindParamText(UserItem.Progress[J].sName);
        FStatementInsertItemProgress.Step;
      end;
    end;
  finally
    FStatementInsertItemProgress.Reset;
  end;

  try
    for J := Low(UserItem.CustomProperty.Properties) to High(UserItem.CustomProperty.Properties) do
    begin
      if (UserItem.CustomProperty.Properties[J].nValues[0] > 0) or  (UserItem.CustomProperty.Properties[J].nValues[1] > 0) or
        (UserItem.CustomProperty.Properties[J].nValues[2] > 0) then
      begin
        FStatementInsertItemProperty.Reset;
        FStatementInsertItemProperty.OrderBindParamInt(ParentID);
        FStatementInsertItemProperty.OrderBindParamInt(ItemType);
        FStatementInsertItemProperty.OrderBindParamInt(ItemIndex);
        FStatementInsertItemProperty.OrderBindParamInt(J);
        FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].btColor);
        FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].btBindType);
        FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].btShowFlag);
        FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].btPercent);
        FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].btHintmodule);
        FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].nValues[0]);
        FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].nValues[1]);
        FStatementInsertItemProperty.OrderBindParamInt(UserItem.CustomProperty.Properties[J].nValues[2]);
        FStatementInsertItemProperty.Step;
      end;
    end;
  finally
    FStatementInsertItemProperty.Reset;
  end;
end;

function TMySqlM2DataDB.GetDataBase: TObject;
begin
  Result := FDB;
end;

procedure TMySqlM2DataDB.OnRequest(Sender: TObject);
begin
  FLastRequestTick := MyGetTickCount;
end;

procedure TMySqlM2DataDB.Run;
begin
  inherited;
  if (FDB <> nil) and (FDB.MySQL <> nil) and (MyGetTickCount - FLastRequestTick >= 10 * 60000) then
  begin
    FDB.Exec('select 1');
  end;
end;

end.
