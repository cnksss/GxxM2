unit SqliteStorageDB;

interface

uses
  Classes,
  SysUtils,
  SQLite3DataBase,

  {$IF CompilerVersion >= 32.0}
  FireDac.Phys.SQLiteCli,
  FireDAC.Phys.SQLiteWrapper.Stat,
  {$ELSE}
  SQLiteCli,
  {$ENDIF}

  Grobal2,
  M2DataCommon;

type
  TSqliteStorageDB = class(TStorageDB)
  private
    FDB: TSQLite3Database;

    FStatementGetStorageID: TSQLStatement;
    FStatementInsertStorageEx: TSQLStatement;
    FStatementGetMaxItemIndex: TSQLStatement;

    FStatementGetItemIndex: TSQLStatement;
    FStatementGetAllHumans: TSQLStatement;

    FStatementHumanRename: TSQLStatement;
  protected
    procedure DoInit; override;
    procedure DoFinal; override;
    
    procedure DoLoadStorageItems(sHumanName: string; List: TList; var StorageID: Integer); override;
    procedure DoSaveStorageItems(sHumanName: string; List: TList; StorageID: Integer); override;

    function DoDeleteStorageItem(StorageID: Integer; sHumanName: string; nMakeIndex, nDBIndex: Integer): Boolean; override;
    function DoAddStorageItem(StorageID: Integer; sHumanName: string; UserItem: PTUserItem): Boolean; override;
    function DoClearStorageItem(StorageID: Integer; sHumanName: string): Boolean; override;

    procedure DoRenameHumanName(sOldName, sNewName: string); override;

    procedure DoGetAllHumans(var SL: TStringList); override;
  public
    constructor Create(AOwner: TM2DataDB); override;
    destructor Destroy; override;
  end;


implementation

{ TStorageEx }


constructor TSqliteStorageDB.Create(AOwner: TM2DataDB);
begin
  inherited Create(AOwner);

  FStatementGetStorageID := nil;
  FStatementInsertStorageEx := nil;
  FStatementGetMaxItemIndex := nil;

  FStatementGetItemIndex := nil;
  FStatementGetAllHumans := nil;

  FStatementHumanRename := nil;
end;

destructor TSqliteStorageDB.Destroy;
begin

  
  inherited;
end;

procedure TSqliteStorageDB.DoInit;
begin
  inherited;
  if (Owner.DataBase <> nil) and (Owner.DataBase is TSqlite3DataBase) then
    FDB := Owner.DataBase as TSqlite3DataBase;

  FStatementGetStorageID := FDB.Statements.AddSQLStatement('StorageEx_GetStorageID');
  FStatementGetStorageID.Sql :=
    'select StorageID from StorageEx where HumanName = ?;';
  FStatementGetStorageID.Prepare;

  FStatementInsertStorageEx := FDB.Statements.AddSQLStatement('StorageEx_Insert');
  FStatementInsertStorageEx.Sql :=
    'insert into StorageEx(HumanName) values (?);';
  FStatementInsertStorageEx.Prepare;

  FStatementGetMaxItemIndex := FDB.Statements.AddSQLStatement('StorageEx_GetMaxItemIndex');
  FStatementGetMaxItemIndex.Sql :=
    'select ifnull(Max(ItemIndex), 0) + 1 from Items where ParentID = ? and ItemType = ' + IntToStr(STORAGEEX_ITEM_TYPE) + ';';
  FStatementGetMaxItemIndex.Prepare;

  FStatementGetItemIndex := FDB.Statements.AddSQLStatement('StorageEx_GetItemIndex');
  FStatementGetItemIndex.Sql :=
    'select ItemIndex from Items where ParentID = ? and MakeIndex = ? and DBIndex = ? and ItemType = ' + IntToStr(STORAGEEX_ITEM_TYPE) + ';';
  FStatementGetItemIndex.Prepare;

  FStatementGetAllHumans := FDB.Statements.AddSQLStatement('StorageEx_GetAllHumans');
  FStatementGetAllHumans.Sql :=
    'select A.StorageID, A.HumanName from StorageEx A WHERE A.StorageID in (select ParentID FROM items where ItemType = 0 and ParentID = A.StorageID);';
  FStatementGetAllHumans.Prepare;

  FStatementHumanRename := FDB.Statements.AddSQLStatement('StorageEx_HumanRename');
  FStatementHumanRename.Sql :=
    'update StorageEx set humanname = ? where humanname = ? and not exists (select * from StorageEx t2 where t2.HumanName = ?)';
  FStatementHumanRename.Prepare;
end;

procedure TSqliteStorageDB.DoFinal;
begin
  inherited;
  if FStatementGetStorageID <> nil then
  begin
    FStatementGetStorageID.Finalize;
    FStatementGetStorageID := nil;
  end;

  if FStatementInsertStorageEx <> nil then
  begin
    FStatementInsertStorageEx.Finalize;
    FStatementInsertStorageEx := nil;
  end;

  if FStatementGetMaxItemIndex <> nil then
  begin
    FStatementGetMaxItemIndex.Finalize;
    FStatementGetMaxItemIndex := nil;
  end;

  if FStatementGetItemIndex <> nil then
  begin
    FStatementGetItemIndex.Finalize;
    FStatementGetItemIndex := nil;
  end;

  if FStatementGetAllHumans <> nil then
  begin
    FStatementGetAllHumans.Finalize;
    FStatementGetAllHumans := nil;
  end;

  if FStatementHumanRename <> nil then
  begin
    FStatementHumanRename.Finalize;
    FStatementHumanRename := nil;
  end;
end;

procedure TSqliteStorageDB.DoLoadStorageItems(sHumanName: string; List: TList; var StorageID: Integer);
begin
  StorageID := 0;
  FStatementGetStorageID.Reset;
  FStatementGetStorageID.OrderBindText(sHumanName);
  if FStatementGetStorageID.Step = SQLITE_ROW then
  begin
    StorageID := FStatementGetStorageID.GetColumnValueInt(0);
    FStatementGetStorageID.Reset;
  end
  else
  begin
    FStatementGetStorageID.Reset;
    FStatementInsertStorageEx.Reset;
    FStatementInsertStorageEx.OrderBindText(sHumanName);
    FStatementInsertStorageEx.Step;
    FStatementInsertStorageEx.Reset;

    FStatementGetStorageID.OrderBindText(sHumanName);
    if FStatementGetStorageID.Step = SQLITE_ROW then
    begin
      StorageID := FStatementGetStorageID.GetColumnValueInt(0);
      FStatementGetStorageID.Reset;
    end;
  end;

  if StorageID <> 0 then
  begin
    Owner.LoadItemsFromDB(StorageID, 0, True, List);
  end;
end;

procedure TSqliteStorageDB.DoRenameHumanName(sOldName, sNewName: string);
begin
  FStatementHumanRename.Reset;
  FStatementHumanRename.OrderBindText(sNewName);
  FStatementHumanRename.OrderBindText(sOldName);
  FStatementHumanRename.OrderBindText(sNewName);
  FStatementHumanRename.Step;
  FStatementHumanRename.Reset;
end;

procedure TSqliteStorageDB.DoSaveStorageItems(sHumanName: string; List: TList; StorageID: Integer);
var
  I: Integer;
  UserItem: PTUserItem;
  S: string;
begin
  if StorageID = 0 then
  begin
    FStatementGetStorageID.Reset;
    FStatementGetStorageID.OrderBindText(sHumanName);
    if FStatementGetStorageID.Step = SQLITE_ROW then
    begin
      StorageID := FStatementGetStorageID.GetColumnValueInt(0);
      FStatementGetStorageID.Reset;
    end
    else
    begin
      FStatementGetStorageID.Reset;
      FStatementInsertStorageEx.Reset;
      FStatementInsertStorageEx.OrderBindText(sHumanName);
      FStatementInsertStorageEx.Step;
      FStatementInsertStorageEx.Reset;

      FStatementGetStorageID.OrderBindText(sHumanName);
      if FStatementGetStorageID.Step = SQLITE_ROW then
      begin
        StorageID := FStatementGetStorageID.GetColumnValueInt(0);
        FStatementGetStorageID.Reset;
      end;
    end;
  end;

  if StorageID <> 0 then
  begin
    FDB.BeginTransaction;
    try
      S :=
        'delete from ItemElementAdd where ItemType = ' + IntToStr(STORAGEEX_ITEM_TYPE) + ' and ParentID = ' + IntToStr(StorageID) + ';' + sLineBreak +
        'delete from ItemAddDataByte where ItemType = ' + IntToStr(STORAGEEX_ITEM_TYPE) + ' and ParentID = ' + IntToStr(StorageID) + ';' + sLineBreak +
        'delete from ItemAddDataInt where ItemType = ' + IntToStr(STORAGEEX_ITEM_TYPE) + ' and ParentID = ' + IntToStr(StorageID) + ';' + sLineBreak +
        'delete from ItemAddDataText where ItemType = ' + IntToStr(STORAGEEX_ITEM_TYPE) + ' and ParentID = ' + IntToStr(StorageID) + ';' + sLineBreak +
        'delete from ItemFlute where ItemType = ' + IntToStr(STORAGEEX_ITEM_TYPE) + ' and ParentID = ' + IntToStr(StorageID) + ';' + sLineBreak +
        'delete from ItemProgress where ItemType = ' + IntToStr(STORAGEEX_ITEM_TYPE) + ' and ParentID = ' + IntToStr(StorageID) + ';' + sLineBreak +
        'delete from ItemProperty where ItemType = ' + IntToStr(STORAGEEX_ITEM_TYPE) + ' and ParentID = ' + IntToStr(StorageID) + ';' + sLineBreak +
        'delete from ItemValueAdd where ItemType = ' + IntToStr(STORAGEEX_ITEM_TYPE) + ' and ParentID = ' + IntToStr(StorageID) + ';' + sLineBreak +
        'delete from Items where ItemType = 0 and ParentID = ' + IntToStr(StorageID) + ';';
      FDB.Execute(S);
      for I := 0 to List.Count - 1 do
      begin
        UserItem := List.Items[I];
        Owner.SaveItemToDB(UserItem, StorageID, STORAGEEX_ITEM_TYPE, I);
      end;

      FDB.Commit;
    except
      FDB.RollBack;
    end;
  end;
end;

function TSqliteStorageDB.DoAddStorageItem(StorageID: Integer; sHumanName: string;
  UserItem: PTUserItem): Boolean;
var
  ItemIndex: Integer;
begin
  Result := False;
  if StorageID = 0 then
  begin
    FStatementGetStorageID.Reset;
    FStatementGetStorageID.OrderBindText(sHumanName);
    if FStatementGetStorageID.Step = SQLITE_ROW then
    begin
      StorageID := FStatementGetStorageID.GetColumnValueInt(0);
      FStatementGetStorageID.Reset;
    end
    else
    begin
      FStatementGetStorageID.Reset;
      FStatementInsertStorageEx.Reset;
      FStatementInsertStorageEx.OrderBindText(sHumanName);
      FStatementInsertStorageEx.Step;
      FStatementInsertStorageEx.Reset;

      FStatementGetStorageID.OrderBindText(sHumanName);
      if FStatementGetStorageID.Step = SQLITE_ROW then
      begin
        StorageID := FStatementGetStorageID.GetColumnValueInt(0);
        FStatementGetStorageID.Reset;
      end;
    end;
  end;

  if StorageID <> 0 then
  begin
    FStatementGetMaxItemIndex.Reset;
    FStatementGetMaxItemIndex.OrderBindInt(StorageID);
    if FStatementGetMaxItemIndex.Step = SQLITE_ROW then
      ItemIndex := FStatementGetMaxItemIndex.OrderGetColumnValueInt
    else
      ItemIndex := 1;
    FStatementGetMaxItemIndex.Reset;

    Owner.SaveItemToDB(UserItem, StorageID, STORAGEEX_ITEM_TYPE, ItemIndex);

    Result := True;
  end;
end;

function TSqliteStorageDB.DoDeleteStorageItem(StorageID: Integer;sHumanName: string; nMakeIndex,
  nDBIndex: Integer): Boolean;
var
  ItemIndex: Integer;
  sWhere, S: string;
begin
  Result := False;
  if StorageID = 0 then
  begin
    FStatementGetStorageID.Reset;
    FStatementGetStorageID.OrderBindText(sHumanName);
    if FStatementGetStorageID.Step = SQLITE_ROW then
    begin
      StorageID := FStatementGetStorageID.GetColumnValueInt(0);
    end;
    FStatementGetStorageID.Reset;
  end;

  if StorageID > 0 then
  begin
    FStatementGetItemIndex.Reset;
    FStatementGetItemIndex.OrderBindInt(StorageID);
    FStatementGetItemIndex.OrderBindInt(nMakeIndex);
    FStatementGetItemIndex.OrderBindInt(nDBIndex);
    if FStatementGetItemIndex.Step = SQLITE_ROW then
    begin
      ItemIndex := FStatementGetItemIndex.OrderGetColumnValueInt;
      FStatementGetItemIndex.Reset;

      FDB.BeginTransaction;
      try
        sWhere := ' where ItemType = ' + IntToStr(STORAGEEX_ITEM_TYPE) + ' and ParentID = ' + IntToStr(StorageID) + ' and ItemIndex = ' +  IntToStr(ItemIndex) + ';';
        S :=
          'delete from ItemElementAdd ' + sWhere + sLineBreak +
          'delete from ItemAddDataByte ' + sWhere + sLineBreak +
          'delete from ItemAddDataInt ' + sWhere + sLineBreak +
          'delete from ItemAddDataText ' + sWhere + sLineBreak +
          'delete from ItemFlute ' + sWhere + sLineBreak +
          'delete from ItemProgress ' + sWhere + sLineBreak +
          'delete from ItemProperty ' + sWhere + sLineBreak +
          'delete from ItemValueAdd ' + sWhere + sLineBreak +
          'delete from Items ' + sWhere + sLineBreak;

        FDB.Execute(S);

        FDB.Commit;
        Result := True;
      except
        FDB.RollBack;
      end;
    end;

    FStatementGetItemIndex.Reset;
  end;
end;

function TSqliteStorageDB.DoClearStorageItem(StorageID: Integer;
  sHumanName: string): Boolean;
var
  sWhere, S: string;
begin
  Result := False;

  if StorageID = 0 then
  begin
    FStatementGetStorageID.Reset;
    FStatementGetStorageID.OrderBindText(sHumanName);
    if FStatementGetStorageID.Step = SQLITE_ROW then
    begin
      StorageID := FStatementGetStorageID.GetColumnValueInt(0);
    end;
    FStatementGetStorageID.Reset;
  end;

  if StorageID > 0 then
  begin
    FDB.BeginTransaction;
    try
      sWhere := ' where ItemType = ' + IntToStr(STORAGEEX_ITEM_TYPE) + ' and ParentID = ' + IntToStr(StorageID) + ';';
      S :=
        'delete from ItemElementAdd ' + sWhere + sLineBreak +
        'delete from ItemAddDataByte ' + sWhere + sLineBreak +
        'delete from ItemAddDataInt ' + sWhere + sLineBreak +
        'delete from ItemAddDataText ' + sWhere + sLineBreak +
        'delete from ItemFlute ' + sWhere + sLineBreak +
        'delete from ItemProgress ' + sWhere + sLineBreak +
        'delete from ItemProperty ' + sWhere + sLineBreak +
        'delete from ItemValueAdd ' + sWhere + sLineBreak +
        'delete from Items ' + sWhere + sLineBreak;

      FDB.Execute(S);

      FDB.Commit;

      Result := True;
    except
      FDB.RollBack;
    end;
  end;
end;

procedure TSqliteStorageDB.DoGetAllHumans(var SL: TStringList);
var
  Ret: Integer;
  StorageID: Integer;
  HumanName: string;
begin
  SL.Clear;
  FStatementGetAllHumans.Reset;
  Ret := FStatementGetAllHumans.Step;
  while Ret = SQLITE_ROW do
  begin
    StorageID := FStatementGetAllHumans.GetColumnValueInt(0);
    HumanName := FStatementGetAllHumans.GetColumnValueText(1);
    SL.AddObject(HumanName, TObject(StorageID));
    
    Ret := FStatementGetAllHumans.Step;
  end;
  FStatementGetAllHumans.Reset;
end;

end.
