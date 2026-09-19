unit SqliteM2DataDB;

interface

uses
  Classes,
  SysUtils,
  M2DataCommon,
  Grobal2,
  SqliteAuctionDB,
  SqliteStorageDB,
  SqliteUserShopDB,
  SQLite3DataBase,

{$IF CompilerVersion >= 32.0}
  FireDac.Phys.SQLiteCli,
  FireDac.Phys.SQLiteWrapper.Stat,
{$ELSE}
  SQLiteCli,
{$ENDIF}
  SqliteCreateTableSql;

type
  TSqliteM2DataDB = class(TM2DataDB)
  private
    FDB: TSQLite3Database;

    FStatementInsertItems: TSQLStatement;
    FStatementInsertItemValueAdd: TSQLStatement;
    FStatementInsertItemElementAdd: TSQLStatement;
    FStatementInsertItemAddDataByte: TSQLStatement;
    FStatementInsertItemAddDataInt: TSQLStatement;
    FStatementInsertItemAddDataText: TSQLStatement;
    FStatementInsertItemFlute: TSQLStatement;
    FStatementInsertItemProgress: TSQLStatement;
    FStatementInsertItemProperty: TSQLStatement;

    FStatementGetItems: TSQLStatement;
    FStatementGetItems_Sort: TSQLStatement;

    FStatementGetItem: TSQLStatement;
    FStatementGetItemValueAdd: TSQLStatement;
    FStatementGetItemElementAdd: TSQLStatement;
    FStatementGetItemAddDataByte: TSQLStatement;
    FStatementGetItemAddDataInt: TSQLStatement;
    FStatementGetItemAddDataText: TSQLStatement;
    FStatementGetItemFlute: TSQLStatement;
    FStatementGetItemProgress: TSQLStatement;
    FStatementGetItemProperty: TSQLStatement;

    procedure UpdateDB_1;
    procedure UpdateDB_2;
    procedure UpdateDB_3;
    procedure UpdateDB_4;
    procedure UpdateDB_5;
    procedure UpdateDB_6;
    procedure DoUpdate;
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
  end;

implementation

uses
  M2Share;

{ TSqliteM2DataDB }

constructor TSqliteM2DataDB.Create;
begin
  inherited;

  FDB := TSQLite3Database.Create;

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
end;

destructor TSqliteM2DataDB.Destroy;
begin
  FDB.Free;
  inherited;
end;

function TSqliteM2DataDB.GetAuctionDBClass: TAuctionDBClass;
begin
  Result := TSqliteAuctionDB;
end;

function TSqliteM2DataDB.GetStorageDBClass: TStorageDBClass;
begin
  Result := TSqliteStorageDB;
end;

function TSqliteM2DataDB.GetUserShopDBClass: TUserShopDBClass;
begin
  Result := TSqliteUserShopDB;
end;

procedure TSqliteM2DataDB.DoInit;
var
  FileName: string;
  Dir: string;
  IsTryAgain: Boolean;
  RS: TResourceStream;
begin
  Dir := ExtractFilePath(ParamStr(0)) + 'M2Data\';
  if not DirectoryExists(Dir) then
    ForceDirectories(Dir);

  FileName := Dir + 'M2Data.DB';

  // 多线程模式 chongchong 20210306
  sqlite3_config(SQLITE_CONFIG_MULTITHREAD);
  if not FileExists(FileName) then
  begin
    RS := TResourceStream.Create(HInstance, 'M2Data', PChar('SQLITEDB'));
    RS.SaveToFile(FileName);
  end;

  if FileExists(FileName) then
  begin
    FDB.MustExist := True;
    FDB.Database := FileName;
    FDB.UseThreadMode := True;

    IsTryAgain := False;
    try
      FDB.Connected := True;
    except
      on E: Exception do
      begin
        if FDB.ErrorCode = 11 then
        begin
          IsTryAgain := True;
        end
        else
        begin
          raise Exception.Create(E.Message);
        end;
      end;
    end;

    if IsTryAgain then
    begin
      FileName := Dir + 'M2Data.DB-shm';
      if FileExists(FileName) then
      begin
        DeleteFile(FileName);
      end;

      FileName := Dir + 'M2Data.DB-wal';
      if FileExists(FileName) then
      begin
        DeleteFile(FileName);
      end;

      FDB.Connected := True;
    end;

    // FDB.Exec('PRAGMA synchronous = NORMAL;');           // OFF = 0, NORMAL = 1, FULL = 2
    FDB.Execute('PRAGMA cache_size = 32768;');
    FDB.Execute('PRAGMA mmap_size = 32768');
    FDB.Execute('PRAGMA locking_mode = EXCLUSIVE;');
    FDB.Execute('PRAGMA journal_mode = WAL;');
    // FDB.Execute('PRAGMA temp_store = MEMORY;');              // DEFAULT = 0, FILE = 1, MEMORY = 2;

    DoUpdate;
  end
  else
  begin
    FDB.MustExist := False;
    FDB.Database := FileName;
    FDB.Connected := True;
    FDB.BeginTransaction;
    try
      FDB.Execute(SQLITE_CREATE_M2DATA_TABLES);
      FDB.Commit;
    except
      on E: Exception do
      begin
        FDB.RollBack;
        MainOutMessage(E.Message);
      end;
    end;
  end;

  // 物品表中插入物品
  FStatementInsertItems := FDB.Statements.AddSQLStatement('InsertItems');
  FStatementInsertItems.Sql := 'insert into Items(' + 'ParentID, ' + 'ItemType, ' + 'ItemIndex, ' + 'MakeIndex, ' + 'DBIndex, ' +
    'Name, ' + 'Dura, ' + 'DuraMax, ' + 'HeroM2DressEffect, ' + 'UpgradeCount, ' + 'IsStartTime, ' + 'LimitTime, ' +
    'HeroM2Light, ' + 'Color, ' + 'IsBind, ' + 'BindOption, ' + 'Effect, ' + 'NewLooks, ' + 'NewShape, ' + 'FluteCount, ' +
    'PropertyText, ' + 'PropertyTextColor, ' + 'ItemFrom, ' + 'ItemFromMap, ' + 'ItemFromMon, ' + 'ItemFromMaker, ' +
    'ItemFromDate, ' + 'InsuranceCount, ' + 'NewExpand3, ' + 'NewExpand4 ' + ') ' + 'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, '
    + '?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';
  FStatementInsertItems.Prepare;

  FStatementInsertItemValueAdd := FDB.Statements.AddSQLStatement('InsertItemValueAdd');
  FStatementInsertItemValueAdd.Sql := 'insert into ItemValueAdd(' + 'ParentID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' +
    'Value) ' + 'values(?, ?, ?, ?, ?);';
  FStatementInsertItemValueAdd.Prepare;

  FStatementInsertItemElementAdd := FDB.Statements.AddSQLStatement('InsertItemElementAdd');
  FStatementInsertItemElementAdd.Sql := 'insert into ItemElementAdd(' + 'ParentID, ' + 'ItemType, ' + 'ItemIndex, ' +
    'ValueIndex, ' + 'Value) ' + 'values(?, ?, ?, ?, ?);';
  FStatementInsertItemElementAdd.Prepare;

  FStatementInsertItemAddDataByte := FDB.Statements.AddSQLStatement('InsertItemAddDataByte');
  FStatementInsertItemAddDataByte.Sql := 'insert into ItemAddDataByte(' + 'ParentID, ' + 'ItemType, ' + 'ItemIndex, ' +
    'ValueIndex, ' + 'Value) ' + 'values(?, ?, ?, ?, ?);';
  FStatementInsertItemAddDataByte.Prepare;

  FStatementInsertItemAddDataInt := FDB.Statements.AddSQLStatement('InsertItemAddDataInt');
  FStatementInsertItemAddDataInt.Sql := 'insert into ItemAddDataInt(' + 'ParentID, ' + 'ItemType, ' + 'ItemIndex, ' +
    'ValueIndex, ' + 'Value) ' + 'values(?, ?, ?, ?, ?);';
  FStatementInsertItemAddDataInt.Prepare;

  FStatementInsertItemAddDataText := FDB.Statements.AddSQLStatement('InsertItemAddDataText');
  FStatementInsertItemAddDataText.Sql := 'insert into ItemAddDataText(' + 'ParentID, ' + 'ItemType, ' + 'ItemIndex, ' +
    'ValueIndex, ' + 'Value) ' + 'values(?, ?, ?, ?, ?);';
  FStatementInsertItemAddDataText.Prepare;

  FStatementInsertItemFlute := FDB.Statements.AddSQLStatement('InsertItemFlute');
  FStatementInsertItemFlute.Sql := 'insert into ItemFlute(' + 'ParentID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' +
    'Value, ' + 'OverlapCount) ' + 'values(?, ?, ?, ?, ?, ?);';
  FStatementInsertItemFlute.Prepare;

  FStatementInsertItemProgress := FDB.Statements.AddSQLStatement('InsertItemProgress');
  FStatementInsertItemProgress.Sql := 'insert into ItemProgress(' + 'ParentID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' +
    'IsOpen, ' + 'NameColor, ' + 'Count, ' + 'ShowType, ' + 'Max, ' + 'Value, ' + 'Level, ' + 'Name) ' +
    'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';
  FStatementInsertItemProgress.Prepare;

  FStatementInsertItemProperty := FDB.Statements.AddSQLStatement('InsertItemProperty');
  FStatementInsertItemProperty.Sql := 'insert into ItemProperty(' + 'ParentID, ' + 'ItemType, ' + 'ItemIndex, ' + 'ValueIndex, ' +
    'Color, ' + 'BindType, ' + 'ShowFlag, ' + 'IsPercent, ' + 'HintModule, ' + 'Value, ' + 'Value2, ' + 'Value3) ' +
    'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';
  FStatementInsertItemProperty.Prepare;

  FStatementGetItems := FDB.Statements.AddSQLStatement('SelectItems');
  FStatementGetItems.Sql := 'select ' + 'ItemIndex,' + 'MakeIndex,' + 'DBIndex,' + 'Name,' + 'Dura,' + 'DuraMax,' +
    'HeroM2DressEffect,' + 'UpgradeCount,' + 'IsStartTime,' + 'LimitTime,' + 'HeroM2Light,' + 'Color,' + 'IsBind,' + 'BindOption,'
    + 'Effect,' + 'NewLooks,' + 'NewShape,' + 'FluteCount,' + 'PropertyText,' + 'PropertyTextColor,' + 'ItemFrom,' +
    'ItemFromMap,' + 'ItemFromMon,' + 'ItemFromMaker,' + 'ItemFromDate,' + 'InsuranceCount, ' + 'NewExpand3, ' + 'NewExpand4 ' +
    'from Items ' + 'where ParentID = ? and ItemType = ?;';
  FStatementGetItems.Prepare;

  FStatementGetItems_Sort := FDB.Statements.AddSQLStatement('SelectItems_Sort');
  FStatementGetItems_Sort.Sql := 'select ' + 'ItemIndex,' + 'MakeIndex,' + 'DBIndex,' + 'Name,' + 'Dura,' + 'DuraMax,' +
    'HeroM2DressEffect,' + 'UpgradeCount,' + 'IsStartTime,' + 'LimitTime,' + 'HeroM2Light,' + 'Color,' + 'IsBind,' + 'BindOption,'
    + 'Effect,' + 'NewLooks,' + 'NewShape,' + 'FluteCount,' + 'PropertyText,' + 'PropertyTextColor,' + 'ItemFrom,' +
    'ItemFromMap,' + 'ItemFromMon,' + 'ItemFromMaker,' + 'ItemFromDate,' + 'InsuranceCount, ' + 'NewExpand3, ' + 'NewExpand4 ' +
    'from Items ' + 'where ParentID = ? and ItemType = ? order by DBIndex;';
  FStatementGetItems_Sort.Prepare;

  FStatementGetItem := FDB.Statements.AddSQLStatement('SelectItem');
  FStatementGetItem.Sql := 'select ' + 'MakeIndex,' + 'DBIndex,' + 'Name,' + 'Dura,' + 'DuraMax,' + 'HeroM2DressEffect,' +
    'UpgradeCount,' + 'IsStartTime,' + 'LimitTime,' + 'HeroM2Light,' + 'Color,' + 'IsBind,' + 'BindOption,' + 'Effect,' +
    'NewLooks,' + 'NewShape,' + 'FluteCount,' + 'PropertyText,' + 'PropertyTextColor,' + 'ItemFrom,' + 'ItemFromMap,' +
    'ItemFromMon,' + 'ItemFromMaker,' + 'ItemFromDate,' + 'InsuranceCount, ' + 'NewExpand3, ' + 'NewExpand4 ' + 'from Items ' +
    'where ParentID = ? and ItemType = ? and ItemIndex = ?;';
  FStatementGetItem.Prepare;

  FStatementGetItemValueAdd := FDB.Statements.AddSQLStatement('SelectitemValueAdd');
  FStatementGetItemValueAdd.Sql := 'select ' + 'ValueIndex,' + 'Value ' + 'from ItemValueAdd ' +
    'where ParentID = ? and ItemType = ? and ItemIndex = ?;';
  FStatementGetItemValueAdd.Prepare;

  FStatementGetItemElementAdd := FDB.Statements.AddSQLStatement('SelectitemElementAdd');
  FStatementGetItemElementAdd.Sql := 'select ' + 'ValueIndex,' + 'Value ' + 'from ItemElementAdd ' +
    'where ParentID = ? and ItemType = ? and ItemIndex = ?;';
  FStatementGetItemElementAdd.Prepare;

  FStatementGetItemAddDataByte := FDB.Statements.AddSQLStatement('SelectitemAddDataByte');
  FStatementGetItemAddDataByte.Sql := 'select ' + 'ValueIndex,' + 'Value ' + 'from ItemAddDataByte ' +
    'where ParentID = ? and ItemType = ? and ItemIndex = ?;';
  FStatementGetItemAddDataByte.Prepare;

  FStatementGetItemAddDataInt := FDB.Statements.AddSQLStatement('SelectitemAddDataInt');
  FStatementGetItemAddDataInt.Sql := 'select ' + 'ValueIndex,' + 'Value ' + 'from ItemAddDataInt ' +
    'where ParentID = ? and ItemType = ? and ItemIndex = ?;';
  FStatementGetItemAddDataInt.Prepare;

  FStatementGetItemAddDataText := FDB.Statements.AddSQLStatement('SelectitemAddDataText');
  FStatementGetItemAddDataText.Sql := 'select ' + 'ValueIndex,' + 'Value ' + 'from ItemAddDataText ' +
    'where ParentID = ? and ItemType = ? and ItemIndex = ?;';
  FStatementGetItemAddDataText.Prepare;

  FStatementGetItemFlute := FDB.Statements.AddSQLStatement('SelectitemFlute');
  FStatementGetItemFlute.Sql := 'select ' + 'ValueIndex,' + 'Value, ' + 'OverlapCount ' + 'from ItemFlute ' +
    'where ParentID = ? and ItemType = ? and ItemIndex = ?;';
  FStatementGetItemFlute.Prepare;

  FStatementGetItemProgress := FDB.Statements.AddSQLStatement('SelectItemProgress');
  FStatementGetItemProgress.Sql := 'select ' + 'ValueIndex,' + 'IsOpen,' + 'NameColor,' + 'Count,' + 'ShowType,' + 'Max,' +
    'Value,' + 'Level,' + 'Name ' + 'from ItemProgress ' + 'where ParentID = ? and ItemType = ? and ItemIndex = ?;';
  FStatementGetItemProgress.Prepare;

  FStatementGetItemProperty := FDB.Statements.AddSQLStatement('SelectItemProperty');
  FStatementGetItemProperty.Sql := 'select ' + 'ValueIndex,' + 'Color,' + 'BindType,' + 'ShowFlag,' + 'IsPercent,' + 'HintModule,'
    + 'Value,' + 'Value2,' + 'Value3 ' + 'from ItemProperty ' + 'where ParentID = ? and ItemType = ? and ItemIndex = ?;';
  FStatementGetItemProperty.Prepare;
end;

procedure TSqliteM2DataDB.UpdateDB_1;
var
  S: string;
begin
  S := 'ALTER TABLE ItemProperty ADD COLUMN "Value2" INTEGER DEFAULT 0;' + sLineBreak +
    'ALTER TABLE ItemProperty ADD COLUMN "Value3" INTEGER DEFAULT 0;' + sLineBreak +

    'UPDATE db_constant set ConstValue = "' + SQLITE_M2DBVERSION + '" where ConstName = "version"';

  FDB.Execute(S);
end;

procedure TSqliteM2DataDB.UpdateDB_2;
var
  S: string;
begin
  S := 'drop table if EXISTS "_AuctionData_old_20190318";' + sLineBreak +
    'ALTER TABLE "AuctionData" RENAME TO "_AuctionData_old_20190318";' + sLineBreak + 'DROP INDEX "idx_AuctionDataItemDBName";' +
    sLineBreak + 'DROP INDEX "idx_AuctionDataItemGroup";' + sLineBreak + 'DROP INDEX "idx_AuctionDataItemName";' + sLineBreak +

    'CREATE TABLE "AuctionData" (' + sLineBreak + ' "AuctionID" INTEGER NOT NULL,' + sLineBreak +
    ' "HumanName" TEXT (14) NOT NULL COLLATE NOCASE,' + sLineBreak + ' "ItemGroup" INTEGER NOT NULL DEFAULT 0,' + sLineBreak +
    ' "ItemColor" INTEGER DEFAULT 0,' + sLineBreak + ' "AddDateTime" INTEGER NOT NULL DEFAULT (strftime(''%s'', ''now'')),' +
    sLineBreak + ' "AuctionTime" INTEGER NOT NULL,' + sLineBreak + ' "StartingPrice" INTEGER DEFAULT 0,' + sLineBreak +
    ' "SellingPrice" INTEGER DEFAULT 0,' + sLineBreak + ' "CurrencyType" INTEGER,' + sLineBreak +
    ' "LastBidder" TEXT (14) COLLATE NOCASE,' + sLineBreak + ' "LastBidPrice" INTEGER,' + sLineBreak + ' "LastBidTime" INTEGER,' +
    sLineBreak + ' "TradingStatus" INTEGER NOT NULL DEFAULT 0,' + sLineBreak + ' "IsItemGive" INTEGER NOT NULL DEFAULT 0,' +
    sLineBreak + ' "IsTopmost" INTEGER NOT NULL DEFAULT 0,' + sLineBreak + ' "ItemDBName" TEXT (60) COLLATE NOCASE,' + sLineBreak
    + ' "ItemName" TEXT (60) COLLATE NOCASE,' + sLineBreak + ' PRIMARY KEY ("AuctionID" ASC)' + sLineBreak + ');' + sLineBreak +

    'INSERT INTO "AuctionData" (' + sLineBreak + ' "AuctionID",' + sLineBreak + ' "HumanName",' + sLineBreak + ' "ItemGroup",' +
    sLineBreak + ' "ItemColor",' + sLineBreak + ' "AddDateTime",' + sLineBreak + ' "AuctionTime",' + sLineBreak +
    ' "StartingPrice",' + sLineBreak + ' "SellingPrice",' + sLineBreak + ' "CurrencyType",' + sLineBreak + ' "LastBidder",' +
    sLineBreak + ' "LastBidPrice",' + sLineBreak + ' "LastBidTime",' + sLineBreak + ' "TradingStatus",' + sLineBreak +
    ' "IsItemGive",' + sLineBreak + ' "IsTopmost",' + sLineBreak + ' "ItemDBName",' + sLineBreak + ' "ItemName"' + sLineBreak +
    ') SELECT' + sLineBreak + ' "AuctionID",' + sLineBreak + ' "HumanName",' + sLineBreak + ' "ItemGroup",' + sLineBreak +
    ' "ItemColor",' + sLineBreak + ' "AddDateTime",' + sLineBreak + ' "AuctionTime",' + sLineBreak + ' "StartingPrice",' +
    sLineBreak + ' "SellingPrice",' + sLineBreak + ' "CurrencyType",' + sLineBreak + ' "LastBidder",' + sLineBreak +
    ' "LastBidPrice",' + sLineBreak + ' "LastBidTime",' + sLineBreak + ' "TradingStatus",' + sLineBreak + ' "IsItemGive",' +
    sLineBreak + ' "IsTopmost",' + sLineBreak + ' "ItemDBName",' + sLineBreak + ' "ItemName"' + sLineBreak + 'FROM' + sLineBreak +
    ' "_AuctionData_old_20190318";' + sLineBreak + 'CREATE INDEX "idx_AuctionDataItemDBName" ON "AuctionData" ("ItemDBName" ASC);'
    + sLineBreak + 'CREATE INDEX "idx_AuctionDataItemGroup" ON "AuctionData" ("ItemGroup" ASC);' + sLineBreak +
    'CREATE INDEX "idx_AuctionDataItemName" ON "AuctionData" ("ItemName" ASC);' + sLineBreak +

    'drop table if EXISTS "_UserShopItem_old_20190318";' + sLineBreak +
    'ALTER TABLE "UserShopItem" RENAME TO "_UserShopItem_old_20190318";' + sLineBreak + 'DROP INDEX "idx_ItemDBName";' +
    sLineBreak + 'DROP INDEX "idx_ItemName";' + sLineBreak + 'DROP INDEX "idx_ItemPrices";' + sLineBreak +
    'CREATE TABLE "UserShopItem" (' + sLineBreak + '"ShopID"  INTEGER NOT NULL,' + sLineBreak + '"ItemID"  INTEGER NOT NULL,' +
    sLineBreak + '"ItemType"  INTEGER,' + sLineBreak + '"CreateDate"  INTEGER,' + sLineBreak + '"IsAllowSell"  INTEGER,' +
    sLineBreak + '"MoneyType"  INTEGER,' + sLineBreak + '"ItemPrice"  INTEGER,' + sLineBreak + '"IsGetMoney"  INTEGER,' +
    sLineBreak + '"BuyerName"  TEXT(14),' + sLineBreak + '"ItemDBName"  TEXT(60) COLLATE NOCASE ,' + sLineBreak +
    '"ItemName"  TEXT(60) COLLATE NOCASE ,' + sLineBreak + 'PRIMARY KEY ("ShopID" ASC, "ItemID" ASC)' + sLineBreak + ');' +
    sLineBreak +

    'INSERT INTO "UserShopItem" (' + sLineBreak + ' "ShopID",' + sLineBreak + ' "ItemID",' + sLineBreak + ' "ItemType",' +
    sLineBreak + ' "CreateDate",' + sLineBreak + ' "IsAllowSell",' + sLineBreak + ' "MoneyType",' + sLineBreak + ' "ItemPrice",' +
    sLineBreak + ' "IsGetMoney",' + sLineBreak + ' "BuyerName",' + sLineBreak + ' "ItemDBName",' + sLineBreak + ' "ItemName"' +
    sLineBreak + ') SELECT' + sLineBreak + ' "ShopID",' + sLineBreak + ' "ItemID",' + sLineBreak + ' "ItemType",' + sLineBreak +
    ' "CreateDate",' + sLineBreak + ' "IsAllowSell",' + sLineBreak + ' "MoneyType",' + sLineBreak + ' "ItemPrice",' + sLineBreak +
    ' "IsGetMoney",' + sLineBreak + ' "BuyerName",' + sLineBreak + ' "ItemDBName",' + sLineBreak + ' "ItemName"' + sLineBreak +
    'FROM' + sLineBreak + ' "_UserShopItem_old_20190318";' + sLineBreak +

    'CREATE INDEX "idx_ItemDBName" ON "UserShopItem" ("ItemDBName" ASC);' + sLineBreak +
    'CREATE INDEX "idx_ItemName" ON "UserShopItem" ("ItemName" ASC);' + sLineBreak +
    'CREATE INDEX "idx_ItemPrices" ON "UserShopItem" ("ItemPrice" ASC);' + sLineBreak +

  // 'drop table "_AuctionData_old_20190318";' + sLineBreak +
  // 'drop table "_UserShopItem_old_20190318";' + sLineBreak +

    'UPDATE db_constant set ConstValue = "' + SQLITE_M2DBVERSION + '" where ConstName = "version"';

  FDB.Execute(S);
end;

procedure TSqliteM2DataDB.UpdateDB_3;
var
  S: string;
begin
  S := 'ALTER TABLE ItemFlute ADD COLUMN "OverlapCount" INTEGER DEFAULT 0;' + sLineBreak +

    'UPDATE db_constant set ConstValue = "' + SQLITE_M2DBVERSION + '" where ConstName = "version"';

  FDB.Execute(S);
end;

procedure TSqliteM2DataDB.UpdateDB_4;
var
  S: string;
begin
  S := 'ALTER TABLE Items ADD COLUMN "NewExpand3" INTEGER DEFAULT 0;' + sLineBreak +
    'ALTER TABLE Items ADD COLUMN "NewExpand4" INTEGER DEFAULT 0;' + sLineBreak +

    'UPDATE db_constant set ConstValue = "' + SQLITE_M2DBVERSION + '" where ConstName = "version"';

  FDB.Execute(S);
end;

procedure TSqliteM2DataDB.UpdateDB_5;
var
  S: string;
begin
  S := 'ALTER TABLE ItemProperty ADD COLUMN "HintModule" INTEGER DEFAULT 0;' + sLineBreak +
    'UPDATE db_constant set ConstValue = "' + SQLITE_M2DBVERSION + '" where ConstName = "version"';

  FDB.Execute(S);
end;

procedure TSqliteM2DataDB.UpdateDB_6;
var
  S: string;
begin
  S := '-- ----------------------------' + sLineBreak + '-- Table structure for ItemAddDataByte' + sLineBreak +
    '-- ----------------------------' + sLineBreak + 'DROP TABLE IF EXISTS "ItemAddDataByte";' + sLineBreak +
    'CREATE TABLE "ItemAddDataByte" (' + sLineBreak + '"ParentID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak + '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak + '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("ParentID", "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak + ');' + sLineBreak +

    '-- ----------------------------' + sLineBreak + '-- Table structure for ItemAddDataInt' + sLineBreak +
    '-- ----------------------------' + sLineBreak + 'DROP TABLE IF EXISTS "ItemAddDataInt";' + sLineBreak +
    'CREATE TABLE "ItemAddDataInt" (' + sLineBreak + '"ParentID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak + '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak + '"Value"  INTEGER,' + sLineBreak +
    'PRIMARY KEY ("ParentID", "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak + ');' + sLineBreak +

    '-- ----------------------------' + sLineBreak + '-- Table structure for ItemAddDataText' + sLineBreak +
    '-- ----------------------------' + sLineBreak + 'DROP TABLE IF EXISTS "ItemAddDataText";' + sLineBreak +
    'CREATE TABLE "ItemAddDataText" (' + sLineBreak + '"ParentID"  INTEGER NOT NULL,' + sLineBreak +
    '"ItemType"  INTEGER NOT NULL,' + sLineBreak + '"ItemIndex"  INTEGER NOT NULL,' + sLineBreak +
    '"ValueIndex"  INTEGER NOT NULL,' + sLineBreak + '"Value"  TEXT,' + sLineBreak +
    'PRIMARY KEY ("ParentID", "ItemType" ASC, "ItemIndex" ASC, "ValueIndex" ASC)' + sLineBreak + ');' + sLineBreak +

    'UPDATE db_constant set ConstValue = "' + SQLITE_M2DBVERSION + '" where ConstName = "version"';

  FDB.Execute(S);
end;

procedure TSqliteM2DataDB.DoUpdate;
type
  PItemInfo = ^TItemInfo;

  TItemInfo = record
    ParentID: Integer;
    ItemID: Integer;
    DBIndex: Integer;
    Name: string;
  end;
var
  S: string;
  DB_Version: Integer;
  sm: TSQLStatement;
  I, Ret: Integer;
  ItemInfo: PItemInfo;
  List: TList;
begin
  sm := FDB.Statements.AddSQLStatement('get_db_constant_value');
  sm.Sql := 'select ConstValue from db_constant where ConstName = "version";';
  sm.Prepare;
  DB_Version := 0;
  if sm.Step = SQLITE_ROW then
  begin
    DB_Version := sm.OrderGetColumnValueInt;
  end;
  sm.Finalize;

  if DB_Version = 20170506 then
  begin
    FDB.BeginTransaction;
    try
      S := 'ALTER TABLE AuctionData ADD COLUMN "ItemColor" INTEGER DEFAULT 0;' + sLineBreak +
        'ALTER TABLE AuctionData ADD COLUMN "IsTopmost" INTEGER DEFAULT 0;' + sLineBreak +

        'ALTER TABLE AuctionData ADD COLUMN "ItemDBName" TEXT(60) COLLATE NOCASE;' + sLineBreak +
        'ALTER TABLE AuctionData ADD COLUMN "ItemName" TEXT(60) COLLATE NOCASE;' + sLineBreak +

        'CREATE INDEX if not exists "idx_AuctionDataItemGroup" ON "AuctionData" ("ItemGroup" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_AuctionDataItemName" ON "AuctionData" ("ItemName" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_AuctionDataItemDBName" ON "AuctionData" ("ItemDBName" ASC); ' + sLineBreak +

        '---------------------------------------------------------------' + sLineBreak + 'CREATE TABLE "StorageEx" (' + sLineBreak
        + '"StorageID"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL DEFAULT 0,' + sLineBreak +
        '"HumanName"  TEXT(14) NOT NULL COLLATE NOCASE ,' + sLineBreak + 'CONSTRAINT "uk_HumanName" UNIQUE ("HumanName" ASC)' +
        sLineBreak + ');' + sLineBreak +

        '---------------------------------------------------------------' + sLineBreak + 'CREATE TABLE "UserShop" (' + sLineBreak
        + '"ShopID"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,' + sLineBreak + '"HumanName"  TEXT(14) COLLATE NOCASE ,' +
        sLineBreak + '"ShopName"  TEXT(14) COLLATE NOCASE ,' + sLineBreak + '"IsBusiness"  INTEGER,' + sLineBreak +
        '"CreateDate"  INTEGER DEFAULT (strftime(''%s'', ''now'')),' + sLineBreak + '"CareValue"  INTEGER DEFAULT 0,' + sLineBreak
        + 'CONSTRAINT "uk_HumanName" UNIQUE ("HumanName" ASC)' + sLineBreak + ');' + sLineBreak +

        '---------------------------------------------------------------' + sLineBreak + 'CREATE TABLE "UserShopItem" (' +
        sLineBreak + '"ShopID"  INTEGER NOT NULL,' + sLineBreak + '"ItemID"  INTEGER NOT NULL,' + sLineBreak +
        '"ItemType"  INTEGER,' + sLineBreak + '"CreateDate"  INTEGER,' + sLineBreak + '"IsAllowSell"  INTEGER,' + sLineBreak +
        '"MoneyType"  INTEGER,' + sLineBreak + '"ItemPrice"  INTEGER,' + sLineBreak + '"IsGetMoney"  INTEGER,' + sLineBreak +
        '"BuyerName"  TEXT(14),' + sLineBreak + '"ItemDBName"  TEXT(60) COLLATE NOCASE ,' + sLineBreak +
        '"ItemName"  TEXT(60) COLLATE NOCASE ,' + sLineBreak + 'PRIMARY KEY ("ShopID" ASC, "ItemID" ASC)' + sLineBreak + ');' +
        sLineBreak +

        'CREATE INDEX if not exists "idx_HumanName" ON "UserShop" ("HumanName" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_ShopName" ON "UserShop" ("ShopName" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_IsBusiness" ON "UserShop" ("IsBusiness" ASC); ' + sLineBreak +

        'CREATE INDEX if not exists "idx_ItemPrices" ON "UserShopItem" ("ItemPrice" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_ItemName" ON "UserShopItem" ("ItemName" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_ItemDBName" ON "UserShopItem" ("ItemDBName" ASC); ' + sLineBreak +

        'UPDATE AuctionData set CurrencyType = ' + IntToStr(g_Config.nAuctionCurrencyType) + ' where CurrencyType = 0; ' +
        sLineBreak +

        'UPDATE db_constant set ConstValue = "' + SQLITE_M2DBVERSION + '" where ConstName = "version"';

      FDB.Execute(S);

      UpdateDB_1;
      UpdateDB_3;
      UpdateDB_5;
      UpdateDB_6;

      FDB.Commit;
    except
      on E: Exception do
      begin
        FDB.RollBack;
        MainOutMessage(E.Message);
      end;
    end;
  end
  else if DB_Version = 20170603 then
  begin
    FDB.BeginTransaction;
    try
      S := '---------------------------------------------------------------' + sLineBreak + 'CREATE TABLE "StorageEx" (' +
        sLineBreak + '"StorageID"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL DEFAULT 0,' + sLineBreak +
        '"HumanName"  TEXT(14) NOT NULL COLLATE NOCASE ,' + sLineBreak + 'CONSTRAINT "uk_HumanName" UNIQUE ("HumanName" ASC)' +
        sLineBreak + ');' + sLineBreak +

        '---------------------------------------------------------------' + sLineBreak + 'CREATE TABLE "UserShop" (' + sLineBreak
        + '"ShopID"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,' + sLineBreak + '"HumanName"  TEXT(14) COLLATE NOCASE ,' +
        sLineBreak + '"ShopName"  TEXT(14) COLLATE NOCASE ,' + sLineBreak + '"IsBusiness"  INTEGER,' + sLineBreak +
        '"CreateDate"  INTEGER DEFAULT (strftime(''%s'', ''now'')),' + sLineBreak + '"CareValue"  INTEGER DEFAULT 0,' + sLineBreak
        + 'CONSTRAINT "uk_HumanName" UNIQUE ("HumanName" ASC)' + sLineBreak + ');' + sLineBreak +

        '---------------------------------------------------------------' + sLineBreak + 'CREATE TABLE "UserShopItem" (' +
        sLineBreak + '"ShopID"  INTEGER NOT NULL,' + sLineBreak + '"ItemID"  INTEGER NOT NULL,' + sLineBreak +
        '"ItemType"  INTEGER,' + sLineBreak + '"CreateDate"  INTEGER,' + sLineBreak + '"IsAllowSell"  INTEGER,' + sLineBreak +
        '"MoneyType"  INTEGER,' + sLineBreak + '"ItemPrice"  INTEGER,' + sLineBreak + '"IsGetMoney"  INTEGER,' + sLineBreak +
        '"BuyerName"  TEXT(14),' + sLineBreak + '"ItemDBName"  TEXT(60) COLLATE NOCASE ,' + sLineBreak +
        '"ItemName"  TEXT(60) COLLATE NOCASE ,' + sLineBreak + 'PRIMARY KEY ("ShopID" ASC, "ItemID" ASC)' + sLineBreak + ');' +
        sLineBreak +

        'CREATE INDEX if not exists "idx_HumanName" ON "UserShop" ("HumanName" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_ShopName" ON "UserShop" ("ShopName" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_IsBusiness" ON "UserShop" ("IsBusiness" ASC); ' + sLineBreak +

        'CREATE INDEX if not exists "idx_ItemPrices" ON "UserShopItem" ("ItemPrice" ASC); ' + sLineBreak +

        'CREATE INDEX if not exists "idx_ItemName" ON "UserShopItem" ("ItemName" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_ItemDBName" ON "UserShopItem" ("ItemDBName" ASC); ' + sLineBreak +

        'ALTER TABLE AuctionData ADD COLUMN "ItemDBName" TEXT(60) COLLATE NOCASE;' + sLineBreak +
        'ALTER TABLE AuctionData ADD COLUMN "ItemName" TEXT(60) COLLATE NOCASE;' + sLineBreak +

        'CREATE INDEX if not exists "idx_AuctionDataItemGroup" ON "AuctionData" ("ItemGroup" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_AuctionDataItemName" ON "AuctionData" ("ItemName" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_AuctionDataItemDBName" ON "AuctionData" ("ItemDBName" ASC); ' + sLineBreak +

        'UPDATE AuctionData set CurrencyType = ' + IntToStr(g_Config.nAuctionCurrencyType) + ' where CurrencyType = 0; ' +
        sLineBreak +

        'UPDATE db_constant set ConstValue = "' + SQLITE_M2DBVERSION + '" where ConstName = "version"';

      FDB.Execute(S);

      UpdateDB_1;
      UpdateDB_3;
      UpdateDB_4;
      UpdateDB_5;
      UpdateDB_6;

      FDB.Commit;
    except
      on E: Exception do
      begin
        FDB.RollBack;
        MainOutMessage(E.Message);
      end;
    end;
  end
  else if DB_Version = 20170610 then
  begin
    FDB.BeginTransaction;
    try
      S := 'ALTER TABLE UserShopItem ADD COLUMN "ItemDBName" TEXT(60) COLLATE NOCASE;' + sLineBreak +
        'ALTER TABLE UserShopItem ADD COLUMN "ItemName" TEXT(60) COLLATE NOCASE;' + sLineBreak +

        'CREATE INDEX if not exists "idx_HumanName" ON "UserShop" ("HumanName" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_ShopName" ON "UserShop" ("ShopName" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_IsBusiness" ON "UserShop" ("IsBusiness" ASC); ' + sLineBreak +

        'CREATE INDEX if not exists "idx_ItemPrices" ON "UserShopItem" ("ItemPrice" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_ItemName" ON "UserShopItem" ("ItemName" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_ItemDBName" ON "UserShopItem" ("ItemDBName" ASC); ' + sLineBreak +

        'ALTER TABLE AuctionData ADD COLUMN "ItemDBName" TEXT(60) COLLATE NOCASE;' + sLineBreak +
        'ALTER TABLE AuctionData ADD COLUMN "ItemName" TEXT(60) COLLATE NOCASE;' + sLineBreak +

        'CREATE INDEX if not exists "idx_AuctionDataItemGroup" ON "AuctionData" ("ItemGroup" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_AuctionDataItemName" ON "AuctionData" ("ItemName" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_AuctionDataItemDBName" ON "AuctionData" ("ItemDBName" ASC); ' + sLineBreak +

        'UPDATE AuctionData set CurrencyType = ' + IntToStr(g_Config.nAuctionCurrencyType) + ' where CurrencyType = 0; ' +
        sLineBreak +

        'UPDATE db_constant set ConstValue = "' + SQLITE_M2DBVERSION + '" where ConstName = "version"';

      FDB.Execute(S);

      UpdateDB_1;
      UpdateDB_3;
      UpdateDB_4;
      UpdateDB_5;
      UpdateDB_6;

      FDB.Commit;
    except
      on E: Exception do
      begin
        FDB.RollBack;
        MainOutMessage(E.Message);
      end;
    end;
  end
  else if DB_Version = 20170701 then
  begin
    FDB.BeginTransaction;
    try
      S := 'ALTER TABLE UserShopItem ADD COLUMN "ItemDBName" TEXT(60) COLLATE NOCASE;' + sLineBreak +

        'CREATE INDEX if not exists "idx_HumanName" ON "UserShop" ("HumanName" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_ShopName" ON "UserShop" ("ShopName" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_IsBusiness" ON "UserShop" ("IsBusiness" ASC); ' + sLineBreak +

        'CREATE INDEX if not exists "idx_ItemPrices" ON "UserShopItem" ("ItemPrice" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_ItemName" ON "UserShopItem" ("ItemName" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_ItemDBName" ON "UserShopItem" ("ItemDBName" ASC); ' + sLineBreak +

        'ALTER TABLE AuctionData ADD COLUMN "ItemDBName" TEXT(60) COLLATE NOCASE;' + sLineBreak +
        'ALTER TABLE AuctionData ADD COLUMN "ItemName" TEXT(60) COLLATE NOCASE;' + sLineBreak +

        'CREATE INDEX if not exists "idx_AuctionDataItemGroup" ON "AuctionData" ("ItemGroup" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_AuctionDataItemName" ON "AuctionData" ("ItemName" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_AuctionDataItemDBName" ON "AuctionData" ("ItemDBName" ASC); ' + sLineBreak +

        'UPDATE AuctionData set CurrencyType = ' + IntToStr(g_Config.nAuctionCurrencyType) + ' where CurrencyType = 0; ' +
        sLineBreak +

        'UPDATE db_constant set ConstValue = "' + SQLITE_M2DBVERSION + '" where ConstName = "version"';

      FDB.Execute(S);

      UpdateDB_1;
      UpdateDB_2;
      UpdateDB_3;
      UpdateDB_4;
      UpdateDB_5;
      UpdateDB_6;

      FDB.Commit;
    except
      on E: Exception do
      begin
        FDB.RollBack;
        MainOutMessage(E.Message);
      end;
    end;
  end
  else if DB_Version = 20180512 then
  begin
    // 本版本所有拍卖货币只支持一种类型CurrencyType均填0，2018-06-13要支持多种类型，所以要修改数据
    FDB.BeginTransaction;
    try
      S := 'CREATE INDEX if not exists "idx_HumanName" ON "UserShop" ("HumanName" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_ShopName" ON "UserShop" ("ShopName" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_IsBusiness" ON "UserShop" ("IsBusiness" ASC); ' + sLineBreak +

        'CREATE INDEX if not exists "idx_ItemPrices" ON "UserShopItem" ("ItemPrice" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_ItemName" ON "UserShopItem" ("ItemName" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_ItemDBName" ON "UserShopItem" ("ItemDBName" ASC); ' + sLineBreak +

        'ALTER TABLE AuctionData ADD COLUMN "ItemDBName" TEXT(60) COLLATE NOCASE;' + sLineBreak +
        'ALTER TABLE AuctionData ADD COLUMN "ItemName" TEXT(60) COLLATE NOCASE;' + sLineBreak +

        'CREATE INDEX if not exists "idx_AuctionDataItemGroup" ON "AuctionData" ("ItemGroup" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_AuctionDataItemName" ON "AuctionData" ("ItemName" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_AuctionDataItemDBName" ON "AuctionData" ("ItemDBName" ASC); ' + sLineBreak +

        'UPDATE AuctionData set CurrencyType = ' + IntToStr(g_Config.nAuctionCurrencyType) + ' where CurrencyType = 0; ' +
        sLineBreak + 'UPDATE db_constant set ConstValue = "' + SQLITE_M2DBVERSION + '" where ConstName = "version"';

      FDB.Execute(S);

      UpdateDB_1;
      UpdateDB_2;
      UpdateDB_3;
      UpdateDB_4;
      UpdateDB_5;
      UpdateDB_6;

      FDB.Commit;
    except
      on E: Exception do
      begin
        FDB.RollBack;
        MainOutMessage(E.Message);
      end;
    end;
  end

  else if DB_Version = 20180613 then
  begin
    // 本版本所有拍卖货币只支持一种类型CurrencyType均填0，2018-06-13要支持多种类型，所以要修改数据
    FDB.BeginTransaction;
    try
      S := 'ALTER TABLE AuctionData ADD COLUMN "ItemDBName" TEXT(60) COLLATE NOCASE;' + sLineBreak +
        'ALTER TABLE AuctionData ADD COLUMN "ItemName" TEXT(60) COLLATE NOCASE;' + sLineBreak +

        'CREATE INDEX if not exists "idx_AuctionDataItemGroup" ON "AuctionData" ("ItemGroup" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_AuctionDataItemName" ON "AuctionData" ("ItemName" ASC); ' + sLineBreak +
        'CREATE INDEX if not exists "idx_AuctionDataItemDBName" ON "AuctionData" ("ItemDBName" ASC); ' + sLineBreak +

        'UPDATE db_constant set ConstValue = "' + SQLITE_M2DBVERSION + '" where ConstName = "version"';

      FDB.Execute(S);

      UpdateDB_1;
      UpdateDB_2;
      UpdateDB_3;
      UpdateDB_4;
      UpdateDB_5;
      UpdateDB_6;

      FDB.Commit;
    except
      on E: Exception do
      begin
        FDB.RollBack;
        MainOutMessage(E.Message);
      end;
    end;
  end
  else if DB_Version = 20180615 then
  begin
    FDB.BeginTransaction;
    try
      UpdateDB_1;
      UpdateDB_2;
      UpdateDB_3;
      UpdateDB_4;
      UpdateDB_5;
      UpdateDB_6;
      FDB.Commit;
    except
      on E: Exception do
      begin
        FDB.RollBack;
        MainOutMessage(E.Message);
      end;
    end;
  end
  else if DB_Version <= 20190318 then
  begin
    FDB.BeginTransaction;
    try
      UpdateDB_2;
      UpdateDB_3;
      UpdateDB_4;
      UpdateDB_5;
      UpdateDB_6;
      FDB.Commit;
    except
      on E: Exception do
      begin
        FDB.RollBack;
        MainOutMessage(E.Message);
      end;
    end;
  end
  else if DB_Version <= 20190606 then
  begin
    FDB.BeginTransaction;
    try
      UpdateDB_4;
      UpdateDB_5;
      UpdateDB_6;
      FDB.Commit;
    except
      on E: Exception do
      begin
        FDB.RollBack;
        MainOutMessage(E.Message);
      end;
    end;
  end
  else if DB_Version <= 20190928 then
  begin
    FDB.BeginTransaction;
    try
      UpdateDB_5;
      UpdateDB_6;
      FDB.Commit;
    except
      on E: Exception do
      begin
        FDB.RollBack;
        MainOutMessage(E.Message);
      end;
    end;
  end
  else if DB_Version <= 20200813 then
  begin
    FDB.BeginTransaction;
    try
      UpdateDB_6;
      FDB.Commit;
    except
      on E: Exception do
      begin
        FDB.RollBack;
        MainOutMessage(E.Message);
      end;
    end;
  end;

  List := TList.Create;
  try
    sm := FDB.Statements.AddSQLStatement('temp');
    sm.Sql := 'select ParentID, ItemIndex, DBIndex, Name from Items where itemtype = 1;';
    sm.Prepare;
    Ret := sm.Step;
    while Ret = SQLITE_ROW do
    begin
      New(ItemInfo);
      ItemInfo.ParentID := sm.OrderGetColumnValueInt;
      ItemInfo.ItemID := sm.OrderGetColumnValueInt;
      ItemInfo.DBIndex := sm.OrderGetColumnValueInt;
      ItemInfo.Name := sm.OrderGetColumnValueText;
      List.Add(ItemInfo);
      Ret := sm.Step;
    end;
    sm.Reset;
    sm.Finalize;

    sm.Sql := 'update UserShopItem set ItemDBName = ?, ItemName = ? where ShopID = ? and ItemID = ?;';
    sm.Prepare;
    FDB.BeginTransaction;
    try
      for I := 0 to List.Count - 1 do
      begin
        ItemInfo := List.Items[I];
        sm.Reset;
        sm.OrderBindText(UserEngine.GetStdItemName(ItemInfo.DBIndex));
        sm.OrderBindText(ProcessItemName(ItemInfo.Name));
        sm.OrderBindInt(ItemInfo.ParentID);
        sm.OrderBindInt(ItemInfo.ItemID);
        Dispose(ItemInfo);
        sm.Step;
      end;
      FDB.Commit;
    except
      on E: Exception do
      begin
        FDB.RollBack;
        MainOutMessage(E.Message);
      end;
    end;

    sm.Finalize;
    List.Clear;

    sm := FDB.Statements.AddSQLStatement('temp');
    sm.Sql := 'select ParentID, ItemIndex, DBIndex, Name from Items where itemtype = ' + IntToStr(AUCTION_ITEM_TYPE) + ';';
    sm.Prepare;
    Ret := sm.Step;
    while Ret = SQLITE_ROW do
    begin
      New(ItemInfo);
      ItemInfo.ParentID := sm.OrderGetColumnValueInt;
      ItemInfo.ItemID := sm.OrderGetColumnValueInt;
      ItemInfo.DBIndex := sm.OrderGetColumnValueInt;
      ItemInfo.Name := sm.OrderGetColumnValueText;
      List.Add(ItemInfo);
      Ret := sm.Step;
    end;
    sm.Reset;
    sm.Finalize;

    sm.Sql := 'update AuctionData set ItemDBName = ?, ItemName = ? where AuctionID = ?;';
    sm.Prepare;
    FDB.BeginTransaction;
    try
      for I := 0 to List.Count - 1 do
      begin
        ItemInfo := List.Items[I];
        sm.Reset;
        sm.OrderBindText(UserEngine.GetStdItemName(ItemInfo.DBIndex));
        sm.OrderBindText(ProcessItemName(ItemInfo.Name));
        sm.OrderBindInt(ItemInfo.ParentID);
        Dispose(ItemInfo);
        sm.Step;
      end;
      FDB.Commit;
    except
      on E: Exception do
      begin
        FDB.RollBack;
        MainOutMessage(E.Message);
      end;
    end;

    sm.Finalize;
  finally
    List.Free;
  end;
end;

procedure TSqliteM2DataDB.DoFinal;
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

procedure TSqliteM2DataDB.DoLoadItemsFromDB(ParentID, ItemType: Integer; IsSort: Boolean; List: TList);
var
  Statement: TSQLStatement;
  Ret, Ret2, ItemIndex: Integer;
  Index2, Value: Integer;
  UserItem: TUserItem;
  PUserItem: PTUserItem;
  sTemp: string;
begin
  if IsSort then
    Statement := FStatementGetItems_Sort
  else
    Statement := FStatementGetItems;

  Statement.Reset;
  Statement.OrderBindInt(ParentID);
  Statement.OrderBindInt(ItemType);

  Ret := Statement.Step;
  while (Ret = SQLITE_ROW) do
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
    FStatementGetItemValueAdd.OrderBindInt(ParentID);
    FStatementGetItemValueAdd.OrderBindInt(ItemType);
    FStatementGetItemValueAdd.OrderBindInt(ItemIndex);
    Ret2 := FStatementGetItemValueAdd.Step;
    while (Ret2 = SQLITE_ROW) do
    begin
      Index2 := FStatementGetItemValueAdd.OrderGetColumnValueInt;
      if (Index2 >= Low(UserItem.btValue)) and (Index2 <= High(UserItem.btValue)) then
      begin
        Value := FStatementGetItemValueAdd.OrderGetColumnValueInt;
        UserItem.btValue[Index2] := Value;
      end;

      Ret2 := FStatementGetItemValueAdd.Step;
    end;
    FStatementGetItemValueAdd.Reset;

    FStatementGetItemElementAdd.Reset;
    FStatementGetItemElementAdd.OrderBindInt(ParentID);
    FStatementGetItemElementAdd.OrderBindInt(ItemType);
    FStatementGetItemElementAdd.OrderBindInt(ItemIndex);
    Ret2 := FStatementGetItemElementAdd.Step;
    while (Ret2 = SQLITE_ROW) do
    begin
      Index2 := FStatementGetItemElementAdd.OrderGetColumnValueInt;
      if (Index2 >= Low(UserItem.btNewValue)) and (Index2 <= High(UserItem.btNewValue)) then
      begin
        Value := FStatementGetItemElementAdd.OrderGetColumnValueInt;
        UserItem.btNewValue[Index2] := Value;
      end;

      Ret2 := FStatementGetItemElementAdd.Step;
    end;
    FStatementGetItemElementAdd.Reset;

    FStatementGetItemAddDataByte.Reset;
    FStatementGetItemAddDataByte.OrderBindInt(ParentID);
    FStatementGetItemAddDataByte.OrderBindInt(ItemType);
    FStatementGetItemAddDataByte.OrderBindInt(ItemIndex);
    Ret2 := FStatementGetItemAddDataByte.Step;
    while (Ret2 = SQLITE_ROW) do
    begin
      Index2 := FStatementGetItemAddDataByte.OrderGetColumnValueInt;
      if (Index2 >= Low(UserItem.btAddDataByte)) and (Index2 <= High(UserItem.btAddDataByte)) then
      begin
        Value := FStatementGetItemAddDataByte.OrderGetColumnValueInt;
        UserItem.btAddDataByte[Index2] := Value;
      end;

      Ret2 := FStatementGetItemAddDataByte.Step;
    end;
    FStatementGetItemAddDataByte.Reset;

    FStatementGetItemAddDataInt.Reset;
    FStatementGetItemAddDataInt.OrderBindInt(ParentID);
    FStatementGetItemAddDataInt.OrderBindInt(ItemType);
    FStatementGetItemAddDataInt.OrderBindInt(ItemIndex);
    Ret2 := FStatementGetItemAddDataInt.Step;
    while (Ret2 = SQLITE_ROW) do
    begin
      Index2 := FStatementGetItemAddDataInt.OrderGetColumnValueInt;
      if (Index2 >= Low(UserItem.nAddDataInt)) and (Index2 <= High(UserItem.nAddDataInt)) then
      begin
        Value := FStatementGetItemAddDataInt.OrderGetColumnValueInt;
        UserItem.nAddDataInt[Index2] := Value;
      end;

      Ret2 := FStatementGetItemAddDataInt.Step;
    end;
    FStatementGetItemAddDataInt.Reset;

    FStatementGetItemAddDataText.Reset;
    FStatementGetItemAddDataText.OrderBindInt(ParentID);
    FStatementGetItemAddDataText.OrderBindInt(ItemType);
    FStatementGetItemAddDataText.OrderBindInt(ItemIndex);
    Ret2 := FStatementGetItemAddDataText.Step;
    while (Ret2 = SQLITE_ROW) do
    begin
      Index2 := FStatementGetItemAddDataText.OrderGetColumnValueInt;
      if (Index2 >= Low(UserItem.sAddDataText)) and (Index2 <= High(UserItem.sAddDataText)) then
      begin
        sTemp := FStatementGetItemAddDataText.OrderGetColumnValueText;
        UserItem.sAddDataText[Index2] := sTemp;
      end;

      Ret2 := FStatementGetItemAddDataText.Step;
    end;
    FStatementGetItemAddDataText.Reset;

    FStatementGetItemFlute.Reset;
    FStatementGetItemFlute.OrderBindInt(ParentID);
    FStatementGetItemFlute.OrderBindInt(0);
    FStatementGetItemFlute.OrderBindInt(ItemIndex);
    Ret2 := FStatementGetItemFlute.Step;
    while (Ret2 = SQLITE_ROW) do
    begin
      Index2 := FStatementGetItemFlute.OrderGetColumnValueInt;
      if (Index2 >= Low(UserItem.Flutes)) and (Index2 <= High(UserItem.Flutes)) then
      begin
        UserItem.Flutes[Index2].GemIndex := FStatementGetItemFlute.OrderGetColumnValueInt;
        UserItem.Flutes[Index2].GemCount := FStatementGetItemFlute.OrderGetColumnValueInt;

        if (UserItem.Flutes[Index2].GemIndex > 0) and (UserItem.Flutes[Index2].GemCount = 0) then
          UserItem.Flutes[Index2].GemCount := 1;
      end;

      Ret2 := FStatementGetItemFlute.Step;
    end;
    FStatementGetItemFlute.Reset;

    FStatementGetItemProgress.Reset;
    FStatementGetItemProgress.OrderBindInt(ParentID);
    FStatementGetItemProgress.OrderBindInt(ItemType);
    FStatementGetItemProgress.OrderBindInt(ItemIndex);
    Ret2 := FStatementGetItemProgress.Step;
    while (Ret2 = SQLITE_ROW) do
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

      Ret2 := FStatementGetItemProgress.Step;
    end;
    FStatementGetItemProgress.Reset;

    FStatementGetItemProperty.Reset;
    FStatementGetItemProperty.OrderBindInt(ParentID);
    FStatementGetItemProperty.OrderBindInt(ItemType);
    FStatementGetItemProperty.OrderBindInt(ItemIndex);
    Ret2 := FStatementGetItemProperty.Step;
    while (Ret2 = SQLITE_ROW) do
    begin
      Index2 := FStatementGetItemProperty.OrderGetColumnValueInt;
      if (Index2 >= Low(UserItem.CustomProperty.Properties)) and (Index2 <= High(UserItem.CustomProperty.Properties)) then
      begin
        UserItem.CustomProperty.Properties[Index2].btColor := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].btBindType := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].btShowFlag := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].btPercent := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].btHintModule := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].nValues[0] := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].nValues[1] := FStatementGetItemProperty.OrderGetColumnValueInt;
        UserItem.CustomProperty.Properties[Index2].nValues[2] := FStatementGetItemProperty.OrderGetColumnValueInt;
      end;

      Ret2 := FStatementGetItemProperty.Step;
    end;
    FStatementGetItemProperty.Reset;

    Ret := Statement.Step;

    New(PUserItem);
    PUserItem^ := UserItem;
    List.Add(PUserItem);
  end;
end;

procedure TSqliteM2DataDB.DoLoadItemFromDB(UserItem: PTUserItem; ParentID, ItemType, ItemIndex: Integer);
var
  Ret: Integer;
  Index2, Value: Integer;
  sTemp: string;
begin
  FillChar(UserItem^, SizeOf(TUserItem), 0);
  FStatementGetItem.Reset;
  FStatementGetItem.OrderBindInt(ParentID);
  FStatementGetItem.OrderBindInt(ItemType);
  FStatementGetItem.OrderBindInt(ItemIndex);
  Ret := FStatementGetItem.Step;
  if (Ret = SQLITE_ROW) then
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
  FStatementGetItemValueAdd.OrderBindInt(ParentID);
  FStatementGetItemValueAdd.OrderBindInt(ItemType);
  FStatementGetItemValueAdd.OrderBindInt(ItemIndex);
  Ret := FStatementGetItemValueAdd.Step;
  while (Ret = SQLITE_ROW) do
  begin
    Index2 := FStatementGetItemValueAdd.OrderGetColumnValueInt;
    if (Index2 >= Low(UserItem.btValue)) and (Index2 <= High(UserItem.btValue)) then
    begin
      Value := FStatementGetItemValueAdd.OrderGetColumnValueInt;
      UserItem.btValue[Index2] := Value;
    end;

    Ret := FStatementGetItemValueAdd.Step;
  end;
  FStatementGetItemValueAdd.Reset;

  FStatementGetItemElementAdd.Reset;
  FStatementGetItemElementAdd.OrderBindInt(ParentID);
  FStatementGetItemElementAdd.OrderBindInt(ItemType);
  FStatementGetItemElementAdd.OrderBindInt(ItemIndex);
  Ret := FStatementGetItemElementAdd.Step;
  while (Ret = SQLITE_ROW) do
  begin
    Index2 := FStatementGetItemElementAdd.OrderGetColumnValueInt;
    if (Index2 >= Low(UserItem.btNewValue)) and (Index2 <= High(UserItem.btNewValue)) then
    begin
      Value := FStatementGetItemElementAdd.OrderGetColumnValueInt;
      UserItem.btNewValue[Index2] := Value;
    end;

    Ret := FStatementGetItemElementAdd.Step;
  end;
  FStatementGetItemElementAdd.Reset;

  FStatementGetItemAddDataByte.Reset;
  FStatementGetItemAddDataByte.OrderBindInt(ParentID);
  FStatementGetItemAddDataByte.OrderBindInt(ItemType);
  FStatementGetItemAddDataByte.OrderBindInt(ItemIndex);
  Ret := FStatementGetItemAddDataByte.Step;
  while (Ret = SQLITE_ROW) do
  begin
    Index2 := FStatementGetItemAddDataByte.OrderGetColumnValueInt;
    if (Index2 >= Low(UserItem.btAddDataByte)) and (Index2 <= High(UserItem.btAddDataByte)) then
    begin
      Value := FStatementGetItemAddDataByte.OrderGetColumnValueInt;
      UserItem.btAddDataByte[Index2] := Value;
    end;

    Ret := FStatementGetItemAddDataByte.Step;
  end;
  FStatementGetItemAddDataByte.Reset;

  FStatementGetItemAddDataInt.Reset;
  FStatementGetItemAddDataInt.OrderBindInt(ParentID);
  FStatementGetItemAddDataInt.OrderBindInt(ItemType);
  FStatementGetItemAddDataInt.OrderBindInt(ItemIndex);
  Ret := FStatementGetItemAddDataInt.Step;
  while (Ret = SQLITE_ROW) do
  begin
    Index2 := FStatementGetItemAddDataInt.OrderGetColumnValueInt;
    if (Index2 >= Low(UserItem.nAddDataInt)) and (Index2 <= High(UserItem.nAddDataInt)) then
    begin
      Value := FStatementGetItemAddDataInt.OrderGetColumnValueInt;
      UserItem.nAddDataInt[Index2] := Value;
    end;

    Ret := FStatementGetItemAddDataInt.Step;
  end;
  FStatementGetItemAddDataInt.Reset;

  FStatementGetItemAddDataText.Reset;
  FStatementGetItemAddDataText.OrderBindInt(ParentID);
  FStatementGetItemAddDataText.OrderBindInt(ItemType);
  FStatementGetItemAddDataText.OrderBindInt(ItemIndex);
  Ret := FStatementGetItemAddDataText.Step;
  while (Ret = SQLITE_ROW) do
  begin
    Index2 := FStatementGetItemAddDataText.OrderGetColumnValueInt;
    if (Index2 >= Low(UserItem.sAddDataText)) and (Index2 <= High(UserItem.sAddDataText)) then
    begin
      sTemp := FStatementGetItemAddDataText.OrderGetColumnValueText;
      UserItem.sAddDataText[Index2] := sTemp;
    end;

    Ret := FStatementGetItemAddDataText.Step;
  end;
  FStatementGetItemAddDataText.Reset;

  FStatementGetItemFlute.Reset;
  FStatementGetItemFlute.OrderBindInt(ParentID);
  FStatementGetItemFlute.OrderBindInt(ItemType);
  FStatementGetItemFlute.OrderBindInt(ItemIndex);
  Ret := FStatementGetItemFlute.Step;
  while (Ret = SQLITE_ROW) do
  begin
    Index2 := FStatementGetItemFlute.OrderGetColumnValueInt;
    if (Index2 >= Low(UserItem.Flutes)) and (Index2 <= High(UserItem.Flutes)) then
    begin
      UserItem.Flutes[Index2].GemIndex := FStatementGetItemFlute.OrderGetColumnValueInt;
      UserItem.Flutes[Index2].GemCount := FStatementGetItemFlute.OrderGetColumnValueInt;

      if (UserItem.Flutes[Index2].GemIndex > 0) and (UserItem.Flutes[Index2].GemCount = 0) then
        UserItem.Flutes[Index2].GemCount := 1;
    end;

    Ret := FStatementGetItemFlute.Step;
  end;
  FStatementGetItemFlute.Reset;

  FStatementGetItemProgress.Reset;
  FStatementGetItemProgress.OrderBindInt(ParentID);
  FStatementGetItemProgress.OrderBindInt(ItemType);
  FStatementGetItemProgress.OrderBindInt(ItemIndex);
  Ret := FStatementGetItemProgress.Step;
  while (Ret = SQLITE_ROW) do
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

    Ret := FStatementGetItemProgress.Step;
  end;
  FStatementGetItemProgress.Reset;

  FStatementGetItemProperty.Reset;
  FStatementGetItemProperty.OrderBindInt(ParentID);
  FStatementGetItemProperty.OrderBindInt(ItemType);
  FStatementGetItemProperty.OrderBindInt(ItemIndex);
  Ret := FStatementGetItemProperty.Step;
  while (Ret = SQLITE_ROW) do
  begin
    Index2 := FStatementGetItemProperty.OrderGetColumnValueInt;
    if (Index2 >= Low(UserItem.CustomProperty.Properties)) and (Index2 <= High(UserItem.CustomProperty.Properties)) then
    begin
      UserItem.CustomProperty.Properties[Index2].btColor := FStatementGetItemProperty.OrderGetColumnValueInt;
      UserItem.CustomProperty.Properties[Index2].btBindType := FStatementGetItemProperty.OrderGetColumnValueInt;
      UserItem.CustomProperty.Properties[Index2].btShowFlag := FStatementGetItemProperty.OrderGetColumnValueInt;
      UserItem.CustomProperty.Properties[Index2].btPercent := FStatementGetItemProperty.OrderGetColumnValueInt;
      UserItem.CustomProperty.Properties[Index2].btHintModule := FStatementGetItemProperty.OrderGetColumnValueInt;
      UserItem.CustomProperty.Properties[Index2].nValues[0] := FStatementGetItemProperty.OrderGetColumnValueInt;
      UserItem.CustomProperty.Properties[Index2].nValues[1] := FStatementGetItemProperty.OrderGetColumnValueInt;
      UserItem.CustomProperty.Properties[Index2].nValues[2] := FStatementGetItemProperty.OrderGetColumnValueInt;
    end;

    Ret := FStatementGetItemProperty.Step;
  end;
  FStatementGetItemProperty.Reset;
end;

procedure TSqliteM2DataDB.DoSaveItemToDB(UserItem: PTUserItem; ParentID, ItemType, ItemIndex: Integer);
var
  J: Integer;
begin
  FStatementInsertItems.Reset;
  try
    FStatementInsertItems.OrderBindInt(ParentID);
    FStatementInsertItems.OrderBindInt(ItemType);
    FStatementInsertItems.OrderBindInt(ItemIndex);
    FStatementInsertItems.OrderBindInt(UserItem.MakeIndex);
    FStatementInsertItems.OrderBindInt(UserItem.wIndex);
    FStatementInsertItems.OrderBindText(UserItem.Name);
    FStatementInsertItems.OrderBindInt(UserItem.Dura);
    FStatementInsertItems.OrderBindInt(UserItem.DuraMax);
    FStatementInsertItems.OrderBindInt(UserItem.dwHeroM2DressEffect);
    FStatementInsertItems.OrderBindInt(UserItem.btUpgradeCount);
    FStatementInsertItems.OrderBindBool(UserItem.boStartTime);
    FStatementInsertItems.OrderBindInt(UserItem.nLimitTime);
    FStatementInsertItems.OrderBindInt(UserItem.btHeroM2Light);
    FStatementInsertItems.OrderBindInt(UserItem.btColor);
    FStatementInsertItems.OrderBindBool(UserItem.boIsBind);
    FStatementInsertItems.OrderBindInt(UserItem.btBindOption);
    FStatementInsertItems.OrderBindInt(UserItem.wEffect);
    FStatementInsertItems.OrderBindInt(UserItem.wNewLooks);
    FStatementInsertItems.OrderBindInt(UserItem.wNewShape);
    FStatementInsertItems.OrderBindInt(UserItem.btFluteCount);
    FStatementInsertItems.OrderBindText(UserItem.CustomProperty.sText);
    FStatementInsertItems.OrderBindInt(UserItem.CustomProperty.btTextColor);
    FStatementInsertItems.OrderBindInt(Integer(UserItem.ItemFrom.ItemForm));
    FStatementInsertItems.OrderBindText(UserItem.ItemFrom.sMapName);
    FStatementInsertItems.OrderBindText(UserItem.ItemFrom.sMonName);
    FStatementInsertItems.OrderBindText(UserItem.ItemFrom.sMakerName);
    FStatementInsertItems.OrderBindDouble(UserItem.ItemFrom.DateTime);
    FStatementInsertItems.OrderBindInt(UserItem.wInsuranceCount);
    FStatementInsertItems.OrderBindInt(UserItem.wNewExpand3);
    FStatementInsertItems.OrderBindInt(UserItem.wNewExpand4);
    FStatementInsertItems.Step;
  finally
    FStatementInsertItems.Reset;
  end;


  // ---------------------------------------------------------------------------------------------

  try
    for J := Low(UserItem.btValue) to High(UserItem.btValue) do
    begin
      if UserItem.btValue[J] <> 0 then
      begin
        FStatementInsertItemValueAdd.Reset;
        FStatementInsertItemValueAdd.OrderBindInt(ParentID);
        FStatementInsertItemValueAdd.OrderBindInt(ItemType);
        FStatementInsertItemValueAdd.OrderBindInt(ItemIndex);
        FStatementInsertItemValueAdd.OrderBindInt(J);
        FStatementInsertItemValueAdd.OrderBindInt(UserItem.btValue[J]);
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
        FStatementInsertItemElementAdd.OrderBindInt(ParentID);
        FStatementInsertItemElementAdd.OrderBindInt(ItemType);
        FStatementInsertItemElementAdd.OrderBindInt(ItemIndex);
        FStatementInsertItemElementAdd.OrderBindInt(J);
        FStatementInsertItemElementAdd.OrderBindInt(UserItem.btNewValue[J]);
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
        FStatementInsertItemAddDataByte.OrderBindInt(ParentID);
        FStatementInsertItemAddDataByte.OrderBindInt(ItemType);
        FStatementInsertItemAddDataByte.OrderBindInt(ItemIndex);
        FStatementInsertItemAddDataByte.OrderBindInt(J);
        FStatementInsertItemAddDataByte.OrderBindInt(UserItem.btAddDataByte[J]);
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
        FStatementInsertItemAddDataInt.OrderBindInt(ParentID);
        FStatementInsertItemAddDataInt.OrderBindInt(ItemType);
        FStatementInsertItemAddDataInt.OrderBindInt(ItemIndex);
        FStatementInsertItemAddDataInt.OrderBindInt(J);
        FStatementInsertItemAddDataInt.OrderBindInt(UserItem.nAddDataInt[J]);
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
        FStatementInsertItemAddDataText.OrderBindInt(ParentID);
        FStatementInsertItemAddDataText.OrderBindInt(ItemType);
        FStatementInsertItemAddDataText.OrderBindInt(ItemIndex);
        FStatementInsertItemAddDataText.OrderBindInt(J);
        FStatementInsertItemAddDataText.OrderBindText(UserItem.sAddDataText[J]);
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
        FStatementInsertItemFlute.OrderBindInt(ParentID);
        FStatementInsertItemFlute.OrderBindInt(ItemType);
        FStatementInsertItemFlute.OrderBindInt(ItemIndex);
        FStatementInsertItemFlute.OrderBindInt(J);
        FStatementInsertItemFlute.OrderBindInt(UserItem.Flutes[J].GemIndex);
        FStatementInsertItemFlute.OrderBindInt(UserItem.Flutes[J].GemCount);
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
        FStatementInsertItemProgress.OrderBindInt(ParentID);
        FStatementInsertItemProgress.OrderBindInt(ItemType);
        FStatementInsertItemProgress.OrderBindInt(ItemIndex);
        FStatementInsertItemProgress.OrderBindInt(J);
        FStatementInsertItemProgress.OrderBindBool(UserItem.Progress[J].boOpen);
        FStatementInsertItemProgress.OrderBindInt(UserItem.Progress[J].btNameColor);
        FStatementInsertItemProgress.OrderBindInt(UserItem.Progress[J].btCount);
        FStatementInsertItemProgress.OrderBindInt(UserItem.Progress[J].btShowType);
        FStatementInsertItemProgress.OrderBindInt(UserItem.Progress[J].wMax);
        FStatementInsertItemProgress.OrderBindInt(UserItem.Progress[J].wValue);
        FStatementInsertItemProgress.OrderBindInt(UserItem.Progress[J].wLevel);
        FStatementInsertItemProgress.OrderBindText(UserItem.Progress[J].sName);
        FStatementInsertItemProgress.Step;
      end;
    end;
  finally
    FStatementInsertItemProgress.Reset;
  end;

  try
    for J := Low(UserItem.CustomProperty.Properties) to High(UserItem.CustomProperty.Properties) do
    begin
      if (UserItem.CustomProperty.Properties[J].nValues[0] > 0) or (UserItem.CustomProperty.Properties[J].nValues[1] > 0) or
        (UserItem.CustomProperty.Properties[J].nValues[2] > 0) then
      begin
        FStatementInsertItemProperty.Reset;
        FStatementInsertItemProperty.OrderBindInt(ParentID);
        FStatementInsertItemProperty.OrderBindInt(ItemType);
        FStatementInsertItemProperty.OrderBindInt(ItemIndex);
        FStatementInsertItemProperty.OrderBindInt(J);
        FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].btColor);
        FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].btBindType);
        FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].btShowFlag);
        FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].btPercent);
        FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].btHintModule);
        FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].nValues[0]);
        FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].nValues[1]);
        FStatementInsertItemProperty.OrderBindInt(UserItem.CustomProperty.Properties[J].nValues[2]);
        FStatementInsertItemProperty.Step;
      end;
    end;
  finally
    FStatementInsertItemProperty.Reset;
  end;
end;

function TSqliteM2DataDB.GetDataBase: TObject;
begin
  Result := FDB;
end;

end.
