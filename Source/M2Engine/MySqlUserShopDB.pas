unit MySqlUserShopDB;

interface

uses
  Windows, Classes, SysUtils, Math, Grobal2, MySqlDataBase,

{$IF CompilerVersion >= 32.0}
  FireDac.Phys.SQLiteCli, FireDac.Phys.SQLiteWrapper.Stat,
{$ELSE}
  SQLiteCli,
{$ENDIF}
  DateUtils, M2DataCommon;

type
  TMySqlUserShopDB = class(TUserShopDB)
  private
    FDB: TMySqlDatabase;

    FStatementUpdateAllBusiness: TMySqlStatement;
    FStatementUpdateHumanBusiness: TMySqlStatement;

    FStatementHumanNameExists: TMySqlStatement;
    FStatementShopNameExists: TMySqlStatement;
    FStatementInsertUserShop: TMySqlStatement;
    FStatementUserShopRename: TMySqlStatement;
    FStatementUserShopRename2: TMySqlStatement;
    FStatementShopItemBuyerRenameName: TMySqlStatement;

    FStatementGetUserShopInfo: TMySqlStatement;
    FStatementIncUserShopCareValue: TMySqlStatement;
    FStatementGetMaxShopItemID: TMySqlStatement;
    FStatementInsertUserShopItem: TMySqlStatement;
    FStatementUpdateUserShopItem: TMySqlStatement;
    FStatementBuyUserShopItem: TMySqlStatement;
    FStatementGetMoneyShopItem: TMySqlStatement;
    FStatementGetSelledAndNoGetMoneyTotal: TMySqlStatement;

    FStatementGetTimeHasArrivedSellItems: TMySqlStatement;
    FStatementSetTimeHasArrivedSellItems: TMySqlStatement;

    FStatementGetShopSellingItem_ASC: TMySqlStatement;
    FStatementGetShopSellingItem_DESC: TMySqlStatement;

    FStatementGetShopSelledItem_ASC: TMySqlStatement;
    FStatementGetShopSelledItem_DESC: TMySqlStatement;

    FStatementGetShopStorageItem_ASC: TMySqlStatement;
    FStatementGetShopStorageItem_DESC: TMySqlStatement;

    FStatementGetShopSellingAndStorageItem_ASC: TMySqlStatement;
    FStatementGetShopSellingAndStorageItem_DESC: TMySqlStatement;

    FStatementGetShopSellingItem_Count: TMySqlStatement;
    FStatementGetShopSelledItem_Count: TMySqlStatement;
    FStatementGetShopStorageItem_Count: TMySqlStatement;
    FStatementGetShopSellingAndStorageItem_Count: TMySqlStatement;

    FStatementGetAllShop_Sort0: TMySqlStatement;
    FStatementGetAllShop_Sort1: TMySqlStatement;
    FStatementGetAllShop_Sort2: TMySqlStatement;
    FStatementGetAllShop_Sort3: TMySqlStatement;
    FStatementGetAllShop_Sort4: TMySqlStatement;
    FStatementGetAllShop_Sort5: TMySqlStatement;

    FStatementGetAllShop_Count: TMySqlStatement;

    FStatementGetAllShop_Ex: TMySqlStatement;

    // FStatementGetHumanSellingItem: TMySqlStatement;
    // FStatementGetHumanSelledItem: TMySqlStatement;
    // FStatementGetHumanStorageItem: TMySqlStatement;
    // FStatementGetHumanSellingAndStorageItem: TMySqlStatement;

    FStatementGetHumanSellingItem_MakeIndex: TMySqlStatement;
    FStatementGetHumanSelledItem_MakeIndex: TMySqlStatement;
    FStatementGetHumanStorageItem_MakeIndex: TMySqlStatement;
    FStatementGetHumanSellingAndStorageItem_MakeIndex: TMySqlStatement;

    FStatementGetItemBindOption: TMySqlStatement;
  protected
    procedure DoInit; override;
    procedure DoFinal; override;

    function DoGetAllShop(StartIndex: Integer; Keyword: string; IsKeywordHumanName: Boolean; SortType: Integer; ShopList:
      TUserShopList): Integer; override;
    function DoGetAllShopCount(Keyword: string; IsKeywordHumanName: Boolean): Integer; override;

    function DoGetAllShopEx(ShopList: TUserShopList): Integer; override;

    function DoGetSellItems(IsMyShop: Boolean; StartIndex: Integer; HumanName, Keyword: string; ItemType, MoneyType, MinPrice,
      MaxPrice, SortType: Integer; ItemList: TUserShopItemList; ShopItemType: TShopItemType = sitSelling): Integer; override;
    function DoGetSellItemsCount(IsMyShop: Boolean; HumanName, Keyword: string; ItemType, MoneyType, MinPrice, MaxPrice: Integer;
      ShopItemType: TShopItemType = sitSelling): Integer; override;

      // function DoGetHumanItems(IsMyShop: Boolean; ItemList: TUserShopItemList; HumanName: string; ShopItemType: TShopItemType): Integer; override;
    function DoGetHumanItemWithMakeIndex(IsMyShop: Boolean; HumanName: string; ShopItemType: TShopItemType; ItemMakeIndex: Integer;
      var UserShopItem: TSimpleUserShopItem): Boolean; override;

    function DoGetSelledAndNoGetMoneyTotal(ItemList: TList): Integer; override;

    function DoGetUserShopInfo(HumanName: string; var UserShop: TUserShop): Boolean; override;

    procedure DoIncUserShopCareValue(HumanName: string); override;

    function DoHumanNameExists(HumanName: string): Boolean; override;
    function DoShopNameExists(ShopName: string): Boolean; override;

    function DoShopAdd(ShopName, HumanName: string): Boolean; override;
    function DoShopDelete(ShopID: Integer): Boolean; override;
    function DoShopRename(ShopID: Integer; NewShopName: string): Boolean; override;

    function DoHumanRename(OldName, NewName: string): Boolean; override;

    function DoUpdateHumanBusiness(HumanName: string; IsBusiness: Boolean): Boolean; override;

    function DoAddItem(ShopID: Integer; ShopItem: pTUserShopItem; sItemName: string): Boolean; override;
    function DoUpdateItem(ShopID, ItemID: Integer; btItemType, btAllowSell, btMoneyType, nPrice: Integer): Boolean; override;
    function DoBuyItem(ShopID, ItemID: Integer; Buyer: string): Boolean; override;
    function DoGetMoneyItem(ShopID, ItemID: Integer): Boolean; override;
    function DoDeleteItem(ShopID, ItemID: Integer): Boolean; override;

    procedure DoRun; override;

  public
    constructor Create(AOwner: TM2DataDB); override;
    destructor Destroy; override;
  end;

implementation

uses
  M2Share;

{ TUserShopDB }

constructor TMySqlUserShopDB.Create(AOwner: TM2DataDB);
begin
  inherited Create(AOwner);

  FStatementUpdateAllBusiness := nil;
  FStatementUpdateHumanBusiness := nil;

  FStatementHumanNameExists := nil;
  FStatementShopNameExists := nil;
  FStatementInsertUserShop := nil;
  FStatementUserShopRename := nil;

  FStatementUserShopRename2 := nil;
  FStatementShopItemBuyerRenameName := nil;

  FStatementGetUserShopInfo := nil;
  FStatementIncUserShopCareValue := nil;
  FStatementGetMaxShopItemID := nil;
  FStatementInsertUserShopItem := nil;
  FStatementUpdateUserShopItem := nil;
  FStatementBuyUserShopItem := nil;
  FStatementGetMoneyShopItem := nil;
  FStatementGetSelledAndNoGetMoneyTotal := nil;
  FStatementGetTimeHasArrivedSellItems := nil;
  FStatementSetTimeHasArrivedSellItems := nil;

  FStatementGetShopSellingItem_ASC := nil;
  FStatementGetShopSellingItem_DESC := nil;

  FStatementGetShopSelledItem_ASC := nil;
  FStatementGetShopSelledItem_DESC := nil;

  FStatementGetShopStorageItem_ASC := nil;
  FStatementGetShopStorageItem_DESC := nil;

  FStatementGetShopSellingAndStorageItem_ASC := nil;
  FStatementGetShopSellingAndStorageItem_DESC := nil;

  FStatementGetShopSellingItem_Count := nil;
  FStatementGetShopSelledItem_Count := nil;
  FStatementGetShopStorageItem_Count := nil;
  FStatementGetShopSellingAndStorageItem_Count := nil;

  FStatementGetAllShop_Sort0 := nil;
  FStatementGetAllShop_Sort1 := nil;
  FStatementGetAllShop_Sort2 := nil;
  FStatementGetAllShop_Sort3 := nil;
  FStatementGetAllShop_Sort4 := nil;
  FStatementGetAllShop_Sort5 := nil;

  FStatementGetAllShop_Count := nil;

  FStatementGetAllShop_Ex := nil;

  // FStatementGetHumanSellingItem := nil;
  // FStatementGetHumanSelledItem := nil;
  // FStatementGetHumanStorageItem := nil;
  // FStatementGetHumanSellingAndStorageItem := nil;

  FStatementGetHumanSellingItem_MakeIndex := nil;
  FStatementGetHumanSelledItem_MakeIndex := nil;
  FStatementGetHumanStorageItem_MakeIndex := nil;
  FStatementGetHumanSellingAndStorageItem_MakeIndex := nil;

  FStatementGetItemBindOption := nil;
end;

destructor TMySqlUserShopDB.Destroy;
begin
  inherited;
end;

procedure TMySqlUserShopDB.DoInit;
const
  SGetShopItemQueryField = 'SELECT ' + 'A.ShopID, ' + 'A.ItemID, ' + 'A.MoneyType, ' + 'A.ItemType, ' + 'A.IsAllowSell, ' +
    'A.ItemPrice, ' + 'A.CreateDate, ' + 'A.IsGetMoney,' + 'A.BuyerName, ' + 'B.ShopName, ' + 'B.HumanName ' + 'FROM ' +
    'UserShopItem A, UserShop B ' + 'WHERE A.ShopID = B.ShopID ';
  SGetShopItemQueryField_MakeIndex = 'SELECT ' + '	A.ShopID,' + '	A.ItemID,' + '	A.MoneyType,' + '	A.ItemType,' +
    '	A.IsAllowSell,' + '	A.ItemPrice,' + '	A.CreateDate,' + '	A.IsGetMoney,' + '	A.BuyerName,' + '	B.ShopName,' + '	B.HumanName,'
    + '	C.MakeIndex,' + '	C.DBIndex,' + '	C.IsBind,' + '	C.BindOption, ' + '   C.Dura ' + 'FROM ' + '	UserShopItem A,' +
    '	UserShop B,' + '	Items C ' + 'WHERE ' + '	A.ShopID = B.ShopID ' + 'AND C.ParentID = A.ShopID ' + 'AND C.ItemType = 1 ' +
    'AND C.ItemIndex = A.ItemID ';
  SGetShopItemQueryCount = 'SELECT ' + 'Count(A.ShopID) ' + 'FROM ' + 'UserShopItem A, UserShop B ' + 'WHERE A.ShopID = B.ShopID ';
  SGetShopItemWhere = ' and (A.ItemType >= ?) and (A.ItemType <= ?) ' + ' and (A.MoneyType >= ?) and (A.MoneyType <= ?) ' +
    ' and (A.ItemPrice >= ?) and (A.ItemPrice <= ?) ' +
  // ' and ((1 = ?) or (A.ItemDBName like ?) or (A.ItemName like ?)) ' +
    ' and ((1 = ?) or (B.HumanName = ?)) ' + ' and (B.IsBusiness >= ?) and (B.IsBusiness <= ?) ';
  SGetAllShopQueryField = 'SELECT ' + 'A.ShopID,' + 'A.HumanName,' + 'A.ShopName,' + 'A.IsBusiness,' + 'A.CreateDate,' +
    'A.CareValue,' +
    '(select count(*) from UserShopItem where ShopID = A.ShopID and IsAllowSell = 1 and length(ifnull(BuyerName, '''')) = 0) as SellItemCount,'
    + '(select count(*) from UserShopItem where ShopID = A.ShopID and length(ifnull(BuyerName, '''')) > 0) as SelledItemCount,' +
    '(select count(*) from UserShopItem where ShopID = A.ShopID and IsAllowSell = 0 and length(ifnull(BuyerName, '''')) = 0) as StorageItemCount '
    + 'FROM ' + 'UserShop A ' + 'WHERE  ' + ' (A.IsBusiness >= ?) and (A.IsBusiness <= ?) ';
  // ' and ((1 = ?) or (A.HumanName like ?)) and ((1 = ?) or (A.ShopName like ?)) ';
begin
  inherited;

  if (Owner.DataBase <> nil) and (Owner.DataBase is TMySqlDatabase) then
    FDB := Owner.DataBase as TMySqlDatabase;

  FStatementUpdateAllBusiness := FDB.Statements.AddSQLStatement('UserShop_UpdateAllBusiness');
  FStatementUpdateAllBusiness.Sql := 'update UserShop set IsBusiness = 0;';
  FStatementUpdateAllBusiness.Prepare;
  FStatementUpdateAllBusiness.Step;

  FStatementUpdateHumanBusiness := FDB.Statements.AddSQLStatement('UserShop_UpdateHumanBusiness');
  FStatementUpdateHumanBusiness.Sql := 'update UserShop set IsBusiness = ? where HumanName = ?;';
  FStatementUpdateHumanBusiness.Prepare;

  FStatementHumanNameExists := FDB.Statements.AddSQLStatement('UserShop_CheckHumanNameExists');
  FStatementHumanNameExists.Sql := 'select 1 FROM UserShop where HumanName = ?;';
  FStatementHumanNameExists.Prepare;

  FStatementShopNameExists := FDB.Statements.AddSQLStatement('UserShop_CheckShopNameExists');
  FStatementShopNameExists.Sql := 'select 1 FROM UserShop where ShopName = ?;';
  FStatementShopNameExists.Prepare;

  FStatementInsertUserShop := FDB.Statements.AddSQLStatement('UserShop_InsertUserShop');
  FStatementInsertUserShop.Sql := 'insert into UserShop(HumanName, ShopName, IsBusiness) values(?, ?, 1);';
  FStatementInsertUserShop.Prepare;

  FStatementUserShopRename := FDB.Statements.AddSQLStatement('UserShop_UpdateShopName');
  FStatementUserShopRename.Sql := 'update UserShop set ShopName = ? where ShopID = ?;';
  FStatementUserShopRename.Prepare;

  FStatementUserShopRename2 := FDB.Statements.AddSQLStatement('UserShop_UpdateShopName2');
  FStatementUserShopRename2.Sql := 'update UserShop set HumanName = ? where HumanName = ?';
  FStatementUserShopRename2.Prepare;

  FStatementShopItemBuyerRenameName := FDB.Statements.AddSQLStatement('UserShop_ShopItemBuyerRenameName');
  FStatementShopItemBuyerRenameName.Sql := 'update UserShopItem set BuyerName = ? where BuyerName = ?';
  FStatementShopItemBuyerRenameName.Prepare;

  FStatementGetUserShopInfo := FDB.Statements.AddSQLStatement('UserShop_GetUserShopInfo');
  FStatementGetUserShopInfo.Sql := 'SELECT ' + 'A.ShopID,' + 'A.HumanName,' + 'A.ShopName,' + 'A.IsBusiness,' + 'A.CreateDate,' +
    'A.CareValue,' +
    '(select count(*) from UserShopItem where ShopID = A.ShopID and IsAllowSell = 1 and length(ifnull(BuyerName, '''')) = 0) as SellItemCount,'
    + '(select count(*) from UserShopItem where ShopID = A.ShopID and length(ifnull(BuyerName, '''')) > 0) as SelledItemCount,' +
    '(select count(*) from UserShopItem where ShopID = A.ShopID and IsAllowSell = 0 and length(ifnull(BuyerName, '''')) = 0) as StorageItemCount '
    + 'FROM ' + 'UserShop A ' + 'WHERE A.HumanName = ?;';
  FStatementGetUserShopInfo.Prepare;

  FStatementIncUserShopCareValue := FDB.Statements.AddSQLStatement('UserShop_IncUserShopCareValue');
  FStatementIncUserShopCareValue.Sql := 'UPDATE UserShop SET CareValue = CareValue + 1 where HumanName = ?';
  FStatementIncUserShopCareValue.Prepare;

  FStatementGetMaxShopItemID := FDB.Statements.AddSQLStatement('UserShop_GetMaxShopItemID');
  FStatementGetMaxShopItemID.Sql := 'select ifnull(Max(ItemID), 0) + 1 from UserShopItem where ShopID = ?;';
  FStatementGetMaxShopItemID.Prepare;

  FStatementInsertUserShopItem := FDB.Statements.AddSQLStatement('UserShop_InsertShopItem');
  FStatementInsertUserShopItem.Sql := 'insert into UserShopItem(' + 'ShopID,' + 'ItemID,' + 'ItemType,' + 'CreateDate,' +
    'IsAllowSell,' + 'MoneyType,' + 'ItemPrice,' + 'IsGetMoney,' + 'BuyerName,' + 'ItemDBName,' + 'ItemName) ' +
    'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';
  FStatementInsertUserShopItem.Prepare;

  FStatementUpdateUserShopItem := FDB.Statements.AddSQLStatement('UserShop_UpdateShopItem');
  FStatementUpdateUserShopItem.Sql :=
    'update UserShopItem set CreateDate = CURRENT_TIMESTAMP, ItemType = ?, IsAllowSell = ?, MoneyType = ?, ItemPrice = ? where length(ifnull(BuyerName, '''')) = 0 and ShopID = ? and ItemID = ?';
  FStatementUpdateUserShopItem.Prepare;

  FStatementBuyUserShopItem := FDB.Statements.AddSQLStatement('UserShop_BuyShopItem');
  FStatementBuyUserShopItem.Sql :=
    'update UserShopItem set CreateDate = CURRENT_TIMESTAMP, BuyerName = ? where length(ifnull(BuyerName, '''')) = 0 and ShopID = ? and ItemID = ?';
  FStatementBuyUserShopItem.Prepare;

  FStatementGetMoneyShopItem := FDB.Statements.AddSQLStatement('UserShop_GetMoneyShopItem');
  FStatementGetMoneyShopItem.Sql :=
    'update UserShopItem set IsGetMoney = 1 where length(ifnull(BuyerName, '''')) > 0 and ShopID = ? and ItemID = ?';
  FStatementGetMoneyShopItem.Prepare;

  FStatementGetSelledAndNoGetMoneyTotal := FDB.Statements.AddSQLStatement('UserShop_GetSelledAndNoGetMoneyTotal');
  FStatementGetSelledAndNoGetMoneyTotal.Sql := 'SELECT ' + 'B.HumanName, ' + 'A.MoneyType, ' + 'Sum(A.ItemPrice) SumPrice ' +
    'FROM ' + 'UserShopItem A, ' + 'UserShop B ' + 'WHERE ' +
    'A.ShopID = B.ShopID and length(ifnull(A.BuyerName, '''')) > 0 and IsGetMoney = 0 ' + 'GROUP BY B.HumanName, A.MoneyType ' +
    'ORDER BY B.HumanName';
  FStatementGetSelledAndNoGetMoneyTotal.Prepare;

  FStatementGetTimeHasArrivedSellItems := FDB.Statements.AddSQLStatement('UserShop_GetTimeHasArrivedSellItems');
  FStatementGetTimeHasArrivedSellItems.Sql := 'SELECT ' + 'A.ShopID, ' + 'A.ItemID, ' + 'A.MoneyType, ' + 'A.ItemType, ' +
    'A.IsAllowSell, ' + 'A.ItemPrice, ' + 'A.CreateDate, ' + 'A.IsGetMoney,' + 'A.BuyerName, ' + 'B.ShopName, ' + 'B.HumanName, '
    + 'C.MakeIndex,' + 'C.DBIndex,' + 'C.IsBind,' + 'C.BindOption, ' + 'C.Dura ' + 'FROM ' +
    'UserShopItem A, UserShop B, Items C ' + 'WHERE A.ShopID = B.ShopID ' + 'AND C.ParentID = A.ShopID ' + 'AND C.ItemType = 1 ' +
    'AND C.ItemIndex = A.ItemID ' +
    'AND (A.IsAllowSell = 1) and length(ifnull(A.BuyerName, '''')) = 0 and TIMESTAMPDIFF(SECOND, A.createdate, CURRENT_TIMESTAMP) > ?';
  FStatementGetTimeHasArrivedSellItems.Prepare;

  FStatementSetTimeHasArrivedSellItems := FDB.Statements.AddSQLStatement('UserShop_SetTimeHasArrivedSellItems');
  FStatementSetTimeHasArrivedSellItems.Sql := 'UPDATE UserShopItem set ' + 'IsAllowSell = 0 ' +
    'WHERE (IsAllowSell = 1) and length(ifnull(BuyerName, '''')) = 0 and TIMESTAMPDIFF(SECOND, CreateDate, CURRENT_TIMESTAMP) > ?';
  FStatementSetTimeHasArrivedSellItems.Prepare;

  FStatementGetShopSellingItem_ASC := FDB.Statements.AddSQLStatement('UserShop_GetSellingItem_ASC');
  FStatementGetShopSellingItem_ASC.Sql := SGetShopItemQueryField +
    ' and (A.IsAllowSell >= ?) and (A.IsAllowSell <= ?) and length(ifnull(A.BuyerName, '''')) = 0 ' + SGetShopItemWhere +
    ' order by A.ItemPrice limit 5 offset ? ';
  FStatementGetShopSellingItem_ASC.Prepare;

  FStatementGetShopSellingItem_DESC := FDB.Statements.AddSQLStatement('UserShop_GetSellingItem_DESC');
  FStatementGetShopSellingItem_DESC.Sql := SGetShopItemQueryField +
    ' and (A.IsAllowSell >= ?) and (A.IsAllowSell <= ?) and length(ifnull(A.BuyerName, '''')) = 0 ' + SGetShopItemWhere +
    ' order by A.ItemPrice desc limit 5 offset ? ';
  FStatementGetShopSellingItem_DESC.Prepare;

  FStatementGetShopSelledItem_ASC := FDB.Statements.AddSQLStatement('UserShop_GetSelledItem_ASC');
  FStatementGetShopSelledItem_ASC.Sql := SGetShopItemQueryField + ' and length(ifnull(A.BuyerName, '''')) > 0 ' +
    SGetShopItemWhere + ' order by A.ItemPrice limit 5 offset ? ';
  FStatementGetShopSelledItem_ASC.Prepare;

  FStatementGetShopSelledItem_DESC := FDB.Statements.AddSQLStatement('UserShop_GetSelledItem_DESC');
  FStatementGetShopSelledItem_DESC.Sql := SGetShopItemQueryField + ' and length(ifnull(A.BuyerName, '''')) > 0 ' +
    SGetShopItemWhere + ' order by A.ItemPrice desc limit 5 offset ? ';
  FStatementGetShopSelledItem_DESC.Prepare;

  FStatementGetShopStorageItem_ASC := FDB.Statements.AddSQLStatement('UserShop_GetStorageItem_ASC');
  FStatementGetShopStorageItem_ASC.Sql := SGetShopItemQueryField +
    ' and (A.IsAllowSell = 0) and length(ifnull(A.BuyerName, '''')) = 0 ' + SGetShopItemWhere +
    ' order by A.ItemPrice limit 5 offset ? ';
  FStatementGetShopStorageItem_ASC.Prepare;

  FStatementGetShopStorageItem_DESC := FDB.Statements.AddSQLStatement('UserShop_GetStorageItem_DESC');
  FStatementGetShopStorageItem_DESC.Sql := SGetShopItemQueryField +
    ' and (A.IsAllowSell = 0) and length(ifnull(A.BuyerName, '''')) = 0 ' + SGetShopItemWhere +
    ' order by A.ItemPrice desc limit 5 offset ? ';
  FStatementGetShopStorageItem_DESC.Prepare;

  FStatementGetShopSellingAndStorageItem_ASC := FDB.Statements.AddSQLStatement('UserShop_GetSellingAndStorageItem_ASC');
  FStatementGetShopSellingAndStorageItem_ASC.Sql := SGetShopItemQueryField + ' and length(ifnull(A.BuyerName, '''')) = 0 ' +
    SGetShopItemWhere + ' order by A.ItemPrice limit 5 offset ? ';
  FStatementGetShopSellingAndStorageItem_ASC.Prepare;

  FStatementGetShopSellingAndStorageItem_DESC := FDB.Statements.AddSQLStatement('UserShop_GetSellingAndStorageItem_DESC');
  FStatementGetShopSellingAndStorageItem_DESC.Sql := SGetShopItemQueryField + ' and length(ifnull(A.BuyerName, '''')) = 0 ' +
    SGetShopItemWhere + ' order by A.ItemPrice desc limit 5 offset ? ';
  FStatementGetShopSellingAndStorageItem_DESC.Prepare;

  FStatementGetShopSellingItem_Count := FDB.Statements.AddSQLStatement('UserShop_GetSellingItem_Count');
  FStatementGetShopSellingItem_Count.Sql := SGetShopItemQueryCount +
    ' and (A.IsAllowSell >= ?) and (A.IsAllowSell <= ?) and length(ifnull(A.BuyerName, '''')) = 0 ' + SGetShopItemWhere;
  FStatementGetShopSellingItem_Count.Prepare;

  FStatementGetShopSelledItem_Count := FDB.Statements.AddSQLStatement('UserShop_GetSelledItem_Count');
  FStatementGetShopSelledItem_Count.Sql := SGetShopItemQueryCount + ' and length(ifnull(A.BuyerName, '''')) > 0 ' +
    SGetShopItemWhere;
  FStatementGetShopSelledItem_Count.Prepare;

  FStatementGetShopStorageItem_Count := FDB.Statements.AddSQLStatement('UserShop_GetStorageItem_Count');
  FStatementGetShopStorageItem_Count.Sql := SGetShopItemQueryCount +
    ' and (A.IsAllowSell = 0) and length(ifnull(A.BuyerName, '''')) = 0 ' + SGetShopItemWhere;
  FStatementGetShopStorageItem_Count.Prepare;

  FStatementGetShopSellingAndStorageItem_Count := FDB.Statements.AddSQLStatement('UserShop_GetSellingAndStorageItem_Count');
  FStatementGetShopSellingAndStorageItem_Count.Sql := SGetShopItemQueryCount + ' and length(ifnull(A.BuyerName, '''')) = 0 ' +
    SGetShopItemWhere;
  FStatementGetShopSellingAndStorageItem_Count.Prepare;

  FStatementGetAllShop_Sort0 := FDB.Statements.AddSQLStatement('UserShop_GetAllShop_S0');
  FStatementGetAllShop_Sort0.Sql := SGetAllShopQueryField + ' order by SellItemCount desc ' + ' limit 8 offset ?';
  FStatementGetAllShop_Sort0.Prepare;

  FStatementGetAllShop_Sort1 := FDB.Statements.AddSQLStatement('UserShop_GetAllShop_S1');
  FStatementGetAllShop_Sort1.Sql := SGetAllShopQueryField + ' order by SellItemCount ' + ' limit 8 offset ?';
  FStatementGetAllShop_Sort1.Prepare;

  FStatementGetAllShop_Sort2 := FDB.Statements.AddSQLStatement('UserShop_GetAllShop_S2');
  FStatementGetAllShop_Sort2.Sql := SGetAllShopQueryField + ' order by SelledItemCount desc ' + ' limit 8 offset ?';
  FStatementGetAllShop_Sort2.Prepare;

  FStatementGetAllShop_Sort3 := FDB.Statements.AddSQLStatement('UserShop_GetAllShop_S3');
  FStatementGetAllShop_Sort3.Sql := SGetAllShopQueryField + ' order by SelledItemCount ' + ' limit 8 offset ?';
  FStatementGetAllShop_Sort3.Prepare;

  FStatementGetAllShop_Sort4 := FDB.Statements.AddSQLStatement('UserShop_GetAllShop_S4');
  FStatementGetAllShop_Sort4.Sql := SGetAllShopQueryField + ' order by CareValue desc ' + ' limit 8 offset ?';
  FStatementGetAllShop_Sort4.Prepare;

  FStatementGetAllShop_Sort5 := FDB.Statements.AddSQLStatement('UserShop_GetAllShop_S5');
  FStatementGetAllShop_Sort5.Sql := SGetAllShopQueryField + ' order by CareValue ' + ' limit 8 offset ?';

  FStatementGetAllShop_Count := FDB.Statements.AddSQLStatement('UserShop_GetAllShop_Count_S');
  FStatementGetAllShop_Count.Sql := 'SELECT ' + 'Count(*) ' + 'FROM ' + 'UserShop ' + 'WHERE ' +
    ' (IsBusiness >= ?) and (IsBusiness <= ?) ';

  FStatementGetAllShop_Count.Prepare;

  FStatementGetAllShop_Ex := FDB.Statements.AddSQLStatement('UserShop_GetAllShop_Ex');
  FStatementGetAllShop_Ex.Sql := 'SELECT ' + 'A.ShopID,' + 'A.HumanName,' + 'A.ShopName,' + 'A.IsBusiness,' + 'A.CreateDate,' +
    'A.CareValue ' + 'FROM ' + 'UserShop A ';
  FStatementGetAllShop_Ex.Prepare;

  {
    FStatementGetHumanSellingItem := FDB.Statements.AddSQLStatement('UserShop_GetHumanSellingItem');
    FStatementGetHumanSellingItem.Sql :=
    SGetShopItemQueryField +
    ' and (A.IsAllowSell >= ?) and (A.IsAllowSell <= ?) and length(ifnull(A.BuyerName, '''')) = 0 ' +
    ' and B.IsBusiness >= ? and B.IsBusiness <= ? ' +
    ' and ((1 = ?) or (B.HumanName = ?)) ' +
    ' order by A.ItemPrice ';
    FStatementGetHumanSellingItem.Prepare;

    FStatementGetHumanSelledItem := FDB.Statements.AddSQLStatement('UserShop_GetHumanSelledItem');
    FStatementGetHumanSelledItem.Sql :=
    SGetShopItemQueryField +
    ' and length(ifnull(A.BuyerName, '''')) > 0 ' +
    ' and ((1 = ?) or (B.HumanName = ?)) ' +
    ' order by A.ItemPrice ';
    FStatementGetHumanSelledItem.Prepare;

    FStatementGetHumanStorageItem := FDB.Statements.AddSQLStatement('UserShop_GetHumanStorageItem');
    FStatementGetHumanStorageItem.Sql :=
    SGetShopItemQueryField +
    ' and (A.IsAllowSell = 0) and length(ifnull(A.BuyerName, '''')) = 0 ' +
    ' and ((1 = ?) or (B.HumanName = ?)) ' +
    ' order by A.ItemPrice ';
    FStatementGetHumanStorageItem.Prepare;

    FStatementGetHumanSellingAndStorageItem := FDB.Statements.AddSQLStatement('UserShop_GetHumanSellingAndStorageItem');
    FStatementGetHumanSellingAndStorageItem.Sql :=
    SGetShopItemQueryField +
    ' and length(ifnull(A.BuyerName, '''')) = 0 ' +
    ' and ((1 = ?) or (B.HumanName = ?)) ' +
    ' order by A.ItemPrice ';
    FStatementGetHumanSellingAndStorageItem.Prepare;
  }

  FStatementGetHumanSellingItem_MakeIndex := FDB.Statements.AddSQLStatement('UserShop_GetHumanSellingItem_MakeIndex');
  FStatementGetHumanSellingItem_MakeIndex.Sql := SGetShopItemQueryField_MakeIndex +
    ' and (A.IsAllowSell >= ?) and (A.IsAllowSell <= ?) and length(ifnull(A.BuyerName, '''')) = 0 ' +
    ' and B.IsBusiness >= ? and B.IsBusiness <= ? ' + ' and ((1 = ?) or (B.HumanName = ?)) ' + ' and C.MakeIndex = ? ';
  // ' order by A.ItemPrice ';
  FStatementGetHumanSellingItem_MakeIndex.Prepare;

  FStatementGetHumanSelledItem_MakeIndex := FDB.Statements.AddSQLStatement('UserShop_GetHumanSelledItem_MakeIndex');
  FStatementGetHumanSelledItem_MakeIndex.Sql := SGetShopItemQueryField_MakeIndex + ' and length(ifnull(A.BuyerName, '''')) > 0 ' +
    ' and ((1 = ?) or (B.HumanName = ?)) ' + ' and C.MakeIndex = ? ';
  // ' order by A.ItemPrice ';
  FStatementGetHumanSelledItem_MakeIndex.Prepare;

  FStatementGetHumanStorageItem_MakeIndex := FDB.Statements.AddSQLStatement('UserShop_GetHumanStorageItem_MakeIndex');
  FStatementGetHumanStorageItem_MakeIndex.Sql := SGetShopItemQueryField_MakeIndex +
    ' and (A.IsAllowSell = 0) and length(ifnull(A.BuyerName, '''')) = 0 ' + ' and ((1 = ?) or (B.HumanName = ?)) ' +
    ' and C.MakeIndex = ? ';
  // ' order by A.ItemPrice ';
  FStatementGetHumanStorageItem_MakeIndex.Prepare;

  FStatementGetHumanSellingAndStorageItem_MakeIndex := FDB.Statements.AddSQLStatement('UserShop_GetHumanSellingAndStorageItem_MakeIndex');
  FStatementGetHumanSellingAndStorageItem_MakeIndex.Sql := SGetShopItemQueryField_MakeIndex +
    ' and length(ifnull(A.BuyerName, '''')) = 0 ' + ' and ((1 = ?) or (B.HumanName = ?)) ' + ' and C.MakeIndex = ? ';
  // ' order by A.ItemPrice ';
  FStatementGetHumanSellingAndStorageItem_MakeIndex.Prepare;

  FStatementGetItemBindOption := FDB.Statements.AddSQLStatement('UserShop_GetItemBindOption');
  FStatementGetItemBindOption.Sql := 'select ' + 'MakeIndex,' + 'IsBind,' + 'BindOption ' + 'from Items ' +
    'where ParentID = ? and ItemType = ? and ItemIndex = ?;';
  FStatementGetItemBindOption.Prepare;
end;

procedure TMySqlUserShopDB.DoFinal;
begin
  inherited;
  if FStatementUpdateAllBusiness <> nil then
  begin
    FStatementUpdateAllBusiness.Finalize;
    FStatementUpdateAllBusiness := nil;
  end;

  if FStatementUpdateHumanBusiness <> nil then
  begin
    FStatementUpdateHumanBusiness.Finalize;
    FStatementUpdateHumanBusiness := nil;
  end;

  if FStatementHumanNameExists <> nil then
  begin
    FStatementHumanNameExists.Finalize;
    FStatementHumanNameExists := nil;
  end;

  if FStatementShopNameExists <> nil then
  begin
    FStatementShopNameExists.Finalize;
    FStatementShopNameExists := nil;
  end;

  if FStatementInsertUserShop <> nil then
  begin
    FStatementInsertUserShop.Finalize;
    FStatementInsertUserShop := nil;
  end;

  if FStatementUserShopRename <> nil then
  begin
    FStatementUserShopRename.Finalize;
    FStatementUserShopRename := nil;
  end;

  if FStatementUserShopRename2 <> nil then
  begin
    FStatementUserShopRename2.Finalize;
    FStatementUserShopRename2 := nil;
  end;

  if FStatementShopItemBuyerRenameName <> nil then
  begin
    FStatementShopItemBuyerRenameName.Finalize;
    FStatementShopItemBuyerRenameName := nil;
  end;

  if FStatementGetUserShopInfo <> nil then
  begin
    FStatementGetUserShopInfo.Finalize;
    FStatementGetUserShopInfo := nil;
  end;

  if FStatementIncUserShopCareValue <> nil then
  begin
    FStatementIncUserShopCareValue.Finalize;
    FStatementIncUserShopCareValue := nil;
  end;

  if FStatementGetMaxShopItemID <> nil then
  begin
    FStatementGetMaxShopItemID.Finalize;
    FStatementGetMaxShopItemID := nil;
  end;

  if FStatementInsertUserShopItem <> nil then
  begin
    FStatementInsertUserShopItem.Finalize;
    FStatementInsertUserShopItem := nil;
  end;
  if FStatementUpdateUserShopItem <> nil then
  begin
    FStatementUpdateUserShopItem.Finalize;
    FStatementUpdateUserShopItem := nil;
  end;

  if FStatementBuyUserShopItem <> nil then
  begin
    FStatementBuyUserShopItem.Finalize;
    FStatementBuyUserShopItem := nil;
  end;

  if FStatementGetMoneyShopItem <> nil then
  begin
    FStatementGetMoneyShopItem.Finalize;
    FStatementGetMoneyShopItem := nil;
  end;

  if FStatementGetSelledAndNoGetMoneyTotal <> nil then
  begin
    FStatementGetSelledAndNoGetMoneyTotal.Finalize;
    FStatementGetSelledAndNoGetMoneyTotal := nil;
  end;

  if FStatementGetTimeHasArrivedSellItems <> nil then
  begin
    FStatementGetTimeHasArrivedSellItems.Finalize;
    FStatementGetTimeHasArrivedSellItems := nil;
  end;

  if FStatementSetTimeHasArrivedSellItems <> nil then
  begin
    FStatementSetTimeHasArrivedSellItems.Finalize;
    FStatementSetTimeHasArrivedSellItems := nil;
  end;

  if FStatementGetShopSellingItem_ASC <> nil then
  begin
    FStatementGetShopSellingItem_ASC.Finalize;
    FStatementGetShopSellingItem_ASC := nil;
  end;

  if FStatementGetShopSellingItem_DESC <> nil then
  begin
    FStatementGetShopSellingItem_DESC.Finalize;
    FStatementGetShopSellingItem_DESC := nil;
  end;

  if FStatementGetShopSelledItem_ASC <> nil then
  begin
    FStatementGetShopSelledItem_ASC.Finalize;
    FStatementGetShopSelledItem_ASC := nil;
  end;

  if FStatementGetShopSelledItem_DESC <> nil then
  begin
    FStatementGetShopSelledItem_DESC.Finalize;
    FStatementGetShopSelledItem_DESC := nil;
  end;

  if FStatementGetShopStorageItem_ASC <> nil then
  begin
    FStatementGetShopStorageItem_ASC.Finalize;
    FStatementGetShopStorageItem_ASC := nil;
  end;

  if FStatementGetShopStorageItem_DESC <> nil then
  begin
    FStatementGetShopStorageItem_DESC.Finalize;
    FStatementGetShopStorageItem_DESC := nil;
  end;

  if FStatementGetShopSellingAndStorageItem_ASC <> nil then
  begin
    FStatementGetShopSellingAndStorageItem_ASC.Finalize;
    FStatementGetShopSellingAndStorageItem_ASC := nil;
  end;

  if FStatementGetShopSellingAndStorageItem_DESC <> nil then
  begin
    FStatementGetShopSellingAndStorageItem_DESC.Finalize;
    FStatementGetShopSellingAndStorageItem_DESC := nil;
  end;

  if FStatementGetShopSellingItem_Count <> nil then
  begin
    FStatementGetShopSellingItem_Count.Finalize;
    FStatementGetShopSellingItem_Count := nil;
  end;

  if FStatementGetShopSelledItem_Count <> nil then
  begin
    FStatementGetShopSelledItem_Count.Finalize;
    FStatementGetShopSelledItem_Count := nil;
  end;

  if FStatementGetShopStorageItem_Count <> nil then
  begin
    FStatementGetShopStorageItem_Count.Finalize;
    FStatementGetShopStorageItem_Count := nil;
  end;

  if FStatementGetShopSellingAndStorageItem_Count <> nil then
  begin
    FStatementGetShopSellingAndStorageItem_Count.Finalize;
    FStatementGetShopSellingAndStorageItem_Count := nil;
  end;

  if FStatementGetAllShop_Sort0 <> nil then
  begin
    FStatementGetAllShop_Sort0.Finalize;
    FStatementGetAllShop_Sort0 := nil;
  end;

  if FStatementGetAllShop_Sort1 <> nil then
  begin
    FStatementGetAllShop_Sort1.Finalize;
    FStatementGetAllShop_Sort1 := nil;
  end;

  if FStatementGetAllShop_Sort2 <> nil then
  begin
    FStatementGetAllShop_Sort2.Finalize;
    FStatementGetAllShop_Sort2 := nil;
  end;

  if FStatementGetAllShop_Sort3 <> nil then
  begin
    FStatementGetAllShop_Sort3.Finalize;
    FStatementGetAllShop_Sort3 := nil;
  end;

  if FStatementGetAllShop_Sort4 <> nil then
  begin
    FStatementGetAllShop_Sort4.Finalize;
    FStatementGetAllShop_Sort4 := nil;
  end;

  if FStatementGetAllShop_Sort5 <> nil then
  begin
    FStatementGetAllShop_Sort5.Finalize;
    FStatementGetAllShop_Sort5 := nil;
  end;

  if FStatementGetAllShop_Count <> nil then
  begin
    FStatementGetAllShop_Count.Finalize;
    FStatementGetAllShop_Count := nil;
  end;

  if FStatementGetAllShop_Ex <> nil then
  begin
    FStatementGetAllShop_Ex.Finalize;
    FStatementGetAllShop_Ex := nil;
  end;

  {
    if FStatementGetHumanSellingItem <> nil then
    begin
    FStatementGetHumanSellingItem.Finalize;
    FStatementGetHumanSellingItem := nil;
    end;

    if FStatementGetHumanSelledItem <> nil then
    begin
    FStatementGetHumanSelledItem.Finalize;
    FStatementGetHumanSelledItem := nil;
    end;

    if FStatementGetHumanStorageItem <> nil then
    begin
    FStatementGetHumanStorageItem.Finalize;
    FStatementGetHumanStorageItem := nil;
    end;

    if FStatementGetHumanSellingAndStorageItem <> nil then
    begin
    FStatementGetHumanSellingAndStorageItem.Finalize;
    FStatementGetHumanSellingAndStorageItem := nil;
    end;
  }

  if FStatementGetHumanSellingItem_MakeIndex <> nil then
  begin
    FStatementGetHumanSellingItem_MakeIndex.Finalize;
    FStatementGetHumanSellingItem_MakeIndex := nil;
  end;

  if FStatementGetHumanSelledItem_MakeIndex <> nil then
  begin
    FStatementGetHumanSelledItem_MakeIndex.Finalize;
    FStatementGetHumanSelledItem_MakeIndex := nil;
  end;

  if FStatementGetHumanStorageItem_MakeIndex <> nil then
  begin
    FStatementGetHumanStorageItem_MakeIndex.Finalize;
    FStatementGetHumanStorageItem_MakeIndex := nil;
  end;

  if FStatementGetHumanSellingAndStorageItem_MakeIndex <> nil then
  begin
    FStatementGetHumanSellingAndStorageItem_MakeIndex.Finalize;
    FStatementGetHumanSellingAndStorageItem_MakeIndex := nil;
  end;

  if FStatementGetItemBindOption <> nil then
  begin
    FStatementGetItemBindOption.Finalize;
    FStatementGetItemBindOption := nil;
  end;
end;

function TMySqlUserShopDB.DoShopAdd(ShopName, HumanName: string): Boolean;
begin
  if HumanNameExists(HumanName) or ShopNameExists(ShopName) then
    Result := False
  else
  begin
    FStatementInsertUserShop.Reset;
    FStatementInsertUserShop.OrderBindParamText(HumanName);
    FStatementInsertUserShop.OrderBindParamText(ShopName);
    Result := FStatementInsertUserShop.Step;
    FStatementInsertUserShop.Reset;
  end;
end;

function TMySqlUserShopDB.DoHumanNameExists(HumanName: string): Boolean;
begin
  FStatementHumanNameExists.Reset;
  FStatementHumanNameExists.OrderBindParamText(HumanName);
  Result := FStatementHumanNameExists.Query and FStatementHumanNameExists.Fetch;
  FStatementHumanNameExists.Reset;
end;

function TMySqlUserShopDB.DoShopNameExists(ShopName: string): Boolean;
begin
  FStatementShopNameExists.Reset;
  FStatementShopNameExists.OrderBindParamText(ShopName);
  Result := FStatementShopNameExists.Query and FStatementShopNameExists.Fetch;
  FStatementShopNameExists.Reset;
end;

function TMySqlUserShopDB.DoGetAllShop(StartIndex: Integer; Keyword: string; IsKeywordHumanName: Boolean; SortType: Integer;
  ShopList: TUserShopList): Integer;
var
  sm: TMySqlStatement;
  UserShop: TUserShop;
begin
  Result := 0;
  try
    if (Length(Keyword) > 0) then
    begin
      sm := FDB.Statements.AddSQLStatement('UserShop_GetAllShop');
      try
        sm.Sql := 'SELECT ' + 'A.ShopID,' + 'A.HumanName,' + 'A.ShopName,' + 'A.IsBusiness,' + 'A.CreateDate,' + 'A.CareValue,' +
          '(select count(*) from UserShopItem where ShopID = A.ShopID and IsAllowSell = 1 and length(ifnull(BuyerName, '''')) = 0) as SellItemCount,'
          +
          '(select count(*) from UserShopItem where ShopID = A.ShopID and length(ifnull(BuyerName, '''')) > 0) as SelledItemCount,'
          +
          '(select count(*) from UserShopItem where ShopID = A.ShopID and IsAllowSell = 0 and length(ifnull(BuyerName, '''')) = 0) as StorageItemCount '
          + 'FROM ' + 'UserShop A ' + 'WHERE 1 = 1 ';

        if Length(Keyword) > 0 then
        begin
          if IsKeywordHumanName then
            sm.Sql := sm.Sql + ' and A.HumanName like ''%' + Keyword + '%'''
          else
            sm.Sql := sm.Sql + ' and A.ShopName like ''%' + Keyword + '%''';
        end;

        if (g_Config.boOfflineCloseMyShop) then
        begin
          sm.Sql := sm.Sql + ' and IsBusiness = 1 ';
        end;

        case SortType of
          0:
            sm.Sql := sm.Sql + ' order by SellItemCount desc ';
          1:
            sm.Sql := sm.Sql + ' order by SellItemCount ';
          2:
            sm.Sql := sm.Sql + ' order by SelledItemCount desc ';
          3:
            sm.Sql := sm.Sql + ' order by SelledItemCount ';
          4:
            sm.Sql := sm.Sql + ' order by CareValue desc ';
          5:
            sm.Sql := sm.Sql + ' order by CareValue ';
        end;

        sm.Sql := sm.Sql + 'limit 8 offset ' + IntToStr(StartIndex);

        sm.Prepare;
        if sm.Query then
        begin
          while (sm.Fetch) do
          begin
            UserShop.ShopID := sm.OrderGetColumnValueInt;
            UserShop.sMasterName := sm.OrderGetColumnValueText;
            UserShop.sShopName := sm.OrderGetColumnValueText;
            UserShop.boBusiness := sm.OrderGetColumnValueBool;
            UserShop.dCreateDate := sm.OrderGetColumnValueDateTime;
            UserShop.nCareValue := sm.OrderGetColumnValueInt;
            UserShop.SellItemCount := sm.OrderGetColumnValueInt;
            UserShop.SelledItemCount := sm.OrderGetColumnValueInt;
            UserShop.StorageItemCount := sm.OrderGetColumnValueInt;

            ShopList.Add(@UserShop);

            Inc(Result);
          end;
        end;
      finally
        sm.Reset;
      end;
    end
    else
    begin
      sm := nil;
      case SortType of
        0:
          sm := FStatementGetAllShop_Sort0;
        1:
          sm := FStatementGetAllShop_Sort1;
        2:
          sm := FStatementGetAllShop_Sort2;
        3:
          sm := FStatementGetAllShop_Sort3;
        4:
          sm := FStatementGetAllShop_Sort4;
        5:
          sm := FStatementGetAllShop_Sort5;
      end;

      if sm = nil then
        Exit;

      sm.Reset;

      // (sBusiness >= ?) and (sBusiness <= ?)
      if (g_Config.boOfflineCloseMyShop) then
      begin
        sm.OrderBindParamInt(1);
        sm.OrderBindParamInt(1);
      end
      else
      begin
        sm.OrderBindParamInt(0);
        sm.OrderBindParamInt(1);
      end;

      {
        //' and ((1 = ?) or (A.HumanName like ?)) ' +
        //' and ((1 = ?) or (A.ShopName like ?)) ' +
        if Length(Keyword) > 0 then
        begin
        if IsKeywordHumanName then
        begin
        sm.OrderBindParamInt(0);
        sm.OrderBindParamText('%' + Keyword + '%');

        sm.OrderBindParamInt(1);
        sm.OrderBindParamText('*');
        end
        else
        begin
        sm.OrderBindParamInt(1);
        sm.OrderBindParamText('*');

        sm.OrderBindParamInt(0);
        sm.OrderBindParamText('%' + Keyword + '%');
        end
        end
        else
        begin
        sm.OrderBindParamInt(1);
        sm.OrderBindParamText('*');

        sm.OrderBindParamInt(1);
        sm.OrderBindParamText('*');
        end;
      }

      sm.OrderBindParamInt(StartIndex);

      if sm.Step then
      begin
        while (sm.Fetch) do
        begin
          UserShop.ShopID := sm.OrderGetColumnValueInt;
          UserShop.sMasterName := sm.OrderGetColumnValueText;
          UserShop.sShopName := sm.OrderGetColumnValueText;
          UserShop.boBusiness := sm.OrderGetColumnValueBool;
          UserShop.dCreateDate := sm.OrderGetColumnValueDateTime;
          UserShop.nCareValue := sm.OrderGetColumnValueInt;
          UserShop.SellItemCount := sm.OrderGetColumnValueInt;
          UserShop.SelledItemCount := sm.OrderGetColumnValueInt;
          UserShop.StorageItemCount := sm.OrderGetColumnValueInt;

          ShopList.Add(@UserShop);

          Inc(Result);
        end;
      end;

      sm.Reset;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

function TMySqlUserShopDB.DoGetAllShopCount(Keyword: string; IsKeywordHumanName: Boolean): Integer;
var
  sm: TMySqlStatement;
begin
  Result := 0;
  try
    if Length(Keyword) > 0 then
    begin
      sm := FDB.Statements.AddSQLStatement('UserShop_GetAllShopCount');
      try
        sm.Sql := 'SELECT ' + 'Count(*) ' + 'FROM ' + 'UserShop ' + 'WHERE 1 = 1 ';

        if Length(Keyword) > 0 then
        begin
          if IsKeywordHumanName then
            sm.Sql := sm.Sql + ' and HumanName like ''%' + Keyword + '%'''
          else
            sm.Sql := sm.Sql + ' and ShopName like ''%' + Keyword + '%''';
        end;

        if (g_Config.boOfflineCloseMyShop) then
        begin
          sm.Sql := sm.Sql + ' and IsBusiness = 1 ';
        end;

        sm.Prepare;
        if sm.Query and sm.Fetch then
        begin
          Result := sm.OrderGetColumnValueInt;
        end;
      finally
        sm.Reset;
      end;
    end
    else
    begin
      FStatementGetAllShop_Count.Reset;

      if (g_Config.boOfflineCloseMyShop) then
      begin
        FStatementGetAllShop_Count.OrderBindParamInt(1);
        FStatementGetAllShop_Count.OrderBindParamInt(1);
      end
      else
      begin
        FStatementGetAllShop_Count.OrderBindParamInt(0);
        FStatementGetAllShop_Count.OrderBindParamInt(1);
      end;

      (*
        //' and ((1 = ?) or (A.HumanName like ?)) ' +
        //' and ((1 = ?) or (A.ShopName like ?)) ' +
        if Length(Keyword) > 0 then
        begin
        if IsKeywordHumanName then
        begin
        FStatementGetAllShop_Count.OrderBindParamInt(0);
        FStatementGetAllShop_Count.OrderBindParamText('%' + Keyword + '%');

        FStatementGetAllShop_Count.OrderBindParamInt(1);
        FStatementGetAllShop_Count.OrderBindParamText('*');
        end
        else
        begin
        FStatementGetAllShop_Count.OrderBindParamInt(1);
        FStatementGetAllShop_Count.OrderBindParamText('*');

        FStatementGetAllShop_Count.OrderBindParamInt(0);
        FStatementGetAllShop_Count.OrderBindParamText('%' + Keyword + '%');
        end
        end
        else
        begin
        FStatementGetAllShop_Count.OrderBindParamInt(1);
        FStatementGetAllShop_Count.OrderBindParamText('*');

        FStatementGetAllShop_Count.OrderBindParamInt(1);
        FStatementGetAllShop_Count.OrderBindParamText('*');
        end;
      *)

      if (FStatementGetAllShop_Count.Query and FStatementGetAllShop_Count.Fetch) then
      begin
        Result := FStatementGetAllShop_Count.OrderGetColumnValueInt;
      end;

      FStatementGetAllShop_Count.Reset;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

function TMySqlUserShopDB.DoGetAllShopEx(ShopList: TUserShopList): Integer;
var
  UserShop: TUserShop;
begin
  Result := 0;
  try
    FStatementGetAllShop_Ex.Reset;
    if FStatementGetAllShop_Ex.Query then
    begin
      while (FStatementGetAllShop_Ex.Fetch) do
      begin
        UserShop.ShopID := FStatementGetAllShop_Ex.OrderGetColumnValueInt;
        UserShop.sMasterName := FStatementGetAllShop_Ex.OrderGetColumnValueText;
        UserShop.sShopName := FStatementGetAllShop_Ex.OrderGetColumnValueText;
        UserShop.boBusiness := FStatementGetAllShop_Ex.OrderGetColumnValueBool;
        UserShop.dCreateDate := FStatementGetAllShop_Ex.OrderGetColumnValueDateTime;
        UserShop.nCareValue := FStatementGetAllShop_Ex.OrderGetColumnValueInt;
        UserShop.SellItemCount := 0;
        UserShop.SelledItemCount := 0;
        UserShop.StorageItemCount := 0;

        ShopList.Add(@UserShop);

        Inc(Result);
      end;
    end;

    FStatementGetAllShop_Ex.Reset;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

function TMySqlUserShopDB.DoGetSellItems(IsMyShop: Boolean; StartIndex: Integer; HumanName, Keyword: string; ItemType, MoneyType,
  MinPrice, MaxPrice, SortType: Integer; ItemList: TUserShopItemList; ShopItemType: TShopItemType): Integer;
var
  ShopItem: TUserShopItem;
  sm: TMySqlStatement;
  ShopID: Integer;
begin
  Result := 0;
  try
    if Length(Keyword) > 0 then
    begin
      sm := FDB.Statements.AddSQLStatement('UserShop_GetAllSellItem');
      try
        sm.Sql := 'SELECT ' + 'A.ShopID, ' + 'A.ItemID, ' + 'A.MoneyType, ' + 'A.ItemType, ' + 'A.IsAllowSell, ' + 'A.ItemPrice, '
          + 'A.CreateDate, ' + 'A.IsGetMoney,' + 'A.BuyerName, ' + 'B.ShopName, ' + 'B.HumanName ' + 'FROM ' +
          'UserShopItem A, UserShop B ' + 'WHERE A.ShopID = B.ShopID ';

        if ShopItemType = sitSelling then
        begin
          if IsMyShop then
            sm.Sql := sm.Sql + ' and (A.IsAllowSell >= 1) and (A.IsAllowSell <= 2) and length(ifnull(A.BuyerName, '''')) = 0 '
          else
            sm.Sql := sm.Sql + ' and (A.IsAllowSell = 1) and length(ifnull(A.BuyerName, '''')) = 0 '
        end
        else if ShopItemType = sitSelled then
        begin
          sm.Sql := sm.Sql + ' and length(ifnull(A.BuyerName, '''')) > 0 ';
        end
        else if ShopItemType = sitStorage then
        begin
          sm.Sql := sm.Sql + ' and (A.IsAllowSell = 0) and length(ifnull(A.BuyerName, '''')) = 0 ';
        end
        else
        begin
          // sm.Sql := sm.Sql + ' and length(ifnull(A.BuyerName, '''')) = 0';
          sm.Sql := sm.Sql + ' and length(ifnull(A.BuyerName, '''')) = 0 ';
        end;

        if ItemType >= 0 then
        begin
          sm.Sql := sm.Sql + ' and A.ItemType = ' + IntToStr(ItemType);
        end;

        if (MoneyType >= 0) then
        begin
          sm.Sql := sm.Sql + ' and A.MoneyType = ' + IntToStr(MoneyType);
        end;

        if (MinPrice <= MaxPrice) and (MaxPrice > 0) then
        begin
          sm.Sql := sm.Sql + ' and A.ItemPrice >= ' + IntToStr(MinPrice) + ' and A.ItemPrice <= ' + IntToStr(MaxPrice);
        end;

        if Length(Keyword) > 0 then
        begin
          sm.Sql := sm.Sql + ' and ((A.ItemDBName like ''%' + Keyword + '%''' + ') or (A.ItemName like ''%' + Keyword + '%''' +
            '))';
        end;

        if Length(HumanName) > 0 then
        begin
          sm.Sql := sm.Sql + ' and B.HumanName = ''' + HumanName + '''';
        end;

        if (g_Config.boOfflineCloseMyShop) then
        begin
          sm.Sql := sm.Sql + ' and B.IsBusiness = 1 ';
        end;

        case SortType of
          0:
            sm.Sql := sm.Sql + ' order by A.ItemPrice ';
          1:
            sm.Sql := sm.Sql + ' order by A.ItemPrice desc ';
        end;

        sm.Sql := sm.Sql + 'limit 5 offset ' + IntToStr(StartIndex);

        sm.Prepare;
        if sm.Query then
        begin
          while (sm.Fetch) do
          begin
            FillChar(ShopItem, SizeOf(ShopItem), 0);

            ShopID := sm.OrderGetColumnValueInt;
            ShopItem.ShopID := ShopID;
            ShopItem.ItemID := sm.OrderGetColumnValueInt;
            ShopItem.btMoneyType := sm.OrderGetColumnValueInt;
            ShopItem.btItemType := sm.OrderGetColumnValueInt;
            ShopItem.btAllowSell := sm.OrderGetColumnValueInt;
            ShopItem.nPrice := sm.OrderGetColumnValueInt;
            ShopItem.dCreateDate := sm.OrderGetColumnValueDateTime;
            ShopItem.boGetMoney := sm.OrderGetColumnValueBool;
            ShopItem.sBuyName := sm.OrderGetColumnValueText;
            ShopItem.sShopName := sm.OrderGetColumnValueText;
            ShopItem.sMasterName := sm.OrderGetColumnValueText;

            Owner.LoadItemFromDB(@ShopItem.UserItem, ShopID, USERSHOP_ITEM_TYPE, ShopItem.ItemID);

            ItemList.Add(@ShopItem);

            Inc(Result);
          end;
        end;
      finally
        sm.Reset;
      end;
    end
    else
    begin
      if ShopItemType = sitSelling then
      begin
        if SortType = 0 then
          sm := FStatementGetShopSellingItem_ASC
        else
          sm := FStatementGetShopSellingItem_DESC;

        sm.Reset;

        // (A.IsAllowSell >= ?) and (A.IsAllowSell <= ?)
        if IsMyShop then
        begin
          sm.OrderBindParamInt(1);
          sm.OrderBindParamInt(2);
        end
        else
        begin
          sm.OrderBindParamInt(1);
          sm.OrderBindParamInt(1);
        end;
      end
      else if ShopItemType = sitSelled then
      begin
        if SortType = 0 then
          sm := FStatementGetShopSelledItem_ASC
        else
          sm := FStatementGetShopSelledItem_DESC;

        sm.Reset;
      end
      else if ShopItemType = sitStorage then
      begin
        if SortType = 0 then
          sm := FStatementGetShopStorageItem_ASC
        else
          sm := FStatementGetShopStorageItem_DESC;

        sm.Reset;
      end
      else
      begin
        if SortType = 0 then
          sm := FStatementGetShopSellingAndStorageItem_ASC
        else
          sm := FStatementGetShopSellingAndStorageItem_DESC;

        sm.Reset;
      end;

      if sm = nil then
        Exit;

      // (A.ItemType >= ?) and (A.ItemType <= ?)
      if ItemType >= 0 then
      begin
        sm.OrderBindParamInt(ItemType);
        sm.OrderBindParamInt(ItemType);
      end
      else
      begin
        sm.OrderBindParamInt(0);
        sm.OrderBindParamInt(255);
      end;

      // (A.MoneyType >= ?) and (A.MoneyType <= ?)
      if (MoneyType >= 0) then
      begin
        sm.OrderBindParamInt(MoneyType);
        sm.OrderBindParamInt(MoneyType);
      end
      else
      begin
        sm.OrderBindParamInt(0);
        sm.OrderBindParamInt(6);
      end;

      // (A.ItemPrice >= ?) and (A.ItemPrice <= ?)
      if (MinPrice <= MaxPrice) and (MaxPrice > 0) then
      begin
        sm.OrderBindParamInt(MinPrice);
        sm.OrderBindParamInt(MaxPrice);
      end
      else
      begin
        sm.OrderBindParamInt(0);
        sm.OrderBindParamInt(High(Integer));
      end;

      {
        // ((1 = ?) or (A.ItemDBName like ?) or (A.ItemName like ?))
        if Length(Keyword) > 0 then
        begin
        sm.OrderBindParamInt(0);
        sm.OrderBindParamText('%' + Keyword + '%');
        sm.OrderBindParamText('%' + Keyword + '%');
        end
        else
        begin
        sm.OrderBindParamInt(1);
        sm.OrderBindParamText('*');
        sm.OrderBindParamText('*');
        end;
      }

      // ((1 = ?) or (B.HumanName = ?))
      if Length(HumanName) > 0 then
      begin
        sm.OrderBindParamInt(0);
        sm.OrderBindParamText(HumanName);
      end
      else
      begin
        sm.OrderBindParamInt(1);
        sm.OrderBindParamText('*');
      end;

      // (B.IsBusiness >= ?) and (B.IsBusiness <= ?)
      if (g_Config.boOfflineCloseMyShop) then
      begin
        sm.OrderBindParamInt(1);
        sm.OrderBindParamInt(1);
      end
      else
      begin
        sm.OrderBindParamInt(0);
        sm.OrderBindParamInt(1);
      end;

      sm.OrderBindParamInt(StartIndex);

      if sm.Query then
      begin
        while (sm.Fetch) do
        begin
          FillChar(ShopItem, SizeOf(ShopItem), 0);

          ShopID := sm.OrderGetColumnValueInt;
          ShopItem.ShopID := ShopID;
          ShopItem.ItemID := sm.OrderGetColumnValueInt;
          ShopItem.btMoneyType := sm.OrderGetColumnValueInt;
          ShopItem.btItemType := sm.OrderGetColumnValueInt;
          ShopItem.btAllowSell := sm.OrderGetColumnValueInt;
          ShopItem.nPrice := sm.OrderGetColumnValueInt;
          ShopItem.dCreateDate := sm.OrderGetColumnValueDateTime;
          ShopItem.boGetMoney := sm.OrderGetColumnValueBool;
          ShopItem.sBuyName := sm.OrderGetColumnValueText;
          ShopItem.sShopName := sm.OrderGetColumnValueText;
          ShopItem.sMasterName := sm.OrderGetColumnValueText;

          Owner.LoadItemFromDB(@ShopItem.UserItem, ShopID, USERSHOP_ITEM_TYPE, ShopItem.ItemID);

          ItemList.Add(@ShopItem);

          Inc(Result);
        end;
      end;

      sm.Reset;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

function TMySqlUserShopDB.DoGetSellItemsCount(IsMyShop: Boolean; HumanName, Keyword: string; ItemType, MoneyType, MinPrice,
  MaxPrice: Integer; ShopItemType: TShopItemType): Integer;
var
  sm: TMySqlStatement;
begin
  Result := 0;
  try
    if Length(Keyword) > 0 then
    begin
      sm := FDB.Statements.AddSQLStatement('UserShop_GetAllSellItemCount');
      try
        sm.Sql := 'SELECT ' + 'Count(A.ShopID) ' + 'FROM ' + 'UserShopItem A, UserShop B ' + 'WHERE A.ShopID = B.ShopID ';

        if ShopItemType = sitSelling then
        begin
          if IsMyShop then
            sm.Sql := sm.Sql + ' and (A.IsAllowSell in (1, 2)) and length(ifnull(A.BuyerName, '''')) = 0 '
          else
            sm.Sql := sm.Sql + ' and (A.IsAllowSell = 1) and length(ifnull(A.BuyerName, '''')) = 0 '
        end
        else if ShopItemType = sitSelled then
        begin
          sm.Sql := sm.Sql + ' and length(ifnull(A.BuyerName, '''')) > 0 ';
        end
        else if ShopItemType = sitStorage then
        begin
          sm.Sql := sm.Sql + ' and (A.IsAllowSell = 0) and length(ifnull(A.BuyerName, '''')) = 0 ';
        end
        else
        begin
          // sm.Sql := sm.Sql + ' and length(ifnull(A.BuyerName, '''')) = 0';
          sm.Sql := sm.Sql + ' and length(ifnull(A.BuyerName, '''')) = 0 ';
        end;

        if ItemType >= 0 then
        begin
          sm.Sql := sm.Sql + ' and A.ItemType = ' + IntToStr(ItemType);
        end;

        if (MoneyType >= 0) then
        begin
          sm.Sql := sm.Sql + ' and A.MoneyType = ' + IntToStr(MoneyType);
        end;

        if (MinPrice <= MaxPrice) and (MaxPrice > 0) then
        begin
          sm.Sql := sm.Sql + ' and A.ItemPrice >= ' + IntToStr(MinPrice) + ' and A.ItemPrice <= ' + IntToStr(MaxPrice);
        end;

        if Length(Keyword) > 0 then
        begin
          sm.Sql := sm.Sql + ' and ((A.ItemDBName like ''%' + Keyword + '%''' + ') or (A.ItemName like ''%' + Keyword + '%''' +
            '))';
        end;

        if Length(HumanName) > 0 then
        begin
          sm.Sql := sm.Sql + ' and B.HumanName = ''' + HumanName + '''';
        end;

        if (g_Config.boOfflineCloseMyShop) then
        begin
          sm.Sql := sm.Sql + ' and B.IsBusiness = 1 ';
        end;

        sm.Prepare;
        if (sm.Query and sm.Fetch) then
        begin
          Result := sm.OrderGetColumnValueInt;
        end;
      finally
        sm.Reset;
      end;
    end
    else
    begin
      if ShopItemType = sitSelling then
      begin
        sm := FStatementGetShopSellingItem_Count;
        sm.Reset;

        // (A.IsAllowSell >= ?) and (A.IsAllowSell <= ?)
        if IsMyShop then
        begin
          sm.OrderBindParamInt(1);
          sm.OrderBindParamInt(2);
        end
        else
        begin
          sm.OrderBindParamInt(1);
          sm.OrderBindParamInt(2);
        end;
      end
      else if ShopItemType = sitSelled then
      begin
        sm := FStatementGetShopSelledItem_Count;
        sm.Reset;
      end
      else if ShopItemType = sitStorage then
      begin
        sm := FStatementGetShopStorageItem_Count;
        sm.Reset;
      end
      else
      begin
        sm := FStatementGetShopSellingAndStorageItem_Count;
        sm.Reset;
      end;

      if sm = nil then
        Exit;

      // (A.ItemType >= ?) and (A.ItemType <= ?)
      if ItemType >= 0 then
      begin
        sm.OrderBindParamInt(ItemType);
        sm.OrderBindParamInt(ItemType);
      end
      else
      begin
        sm.OrderBindParamInt(0);
        sm.OrderBindParamInt(255);
      end;

      // (A.MoneyType >= ?) and (A.MoneyType <= ?)
      if (MoneyType >= 0) then
      begin
        sm.OrderBindParamInt(MoneyType);
        sm.OrderBindParamInt(MoneyType);
      end
      else
      begin
        sm.OrderBindParamInt(0);
        sm.OrderBindParamInt(6);
      end;

      // (A.ItemPrice >= ?) and (A.ItemPrice <= ?)
      if (MinPrice <= MaxPrice) and (MaxPrice > 0) then
      begin
        sm.OrderBindParamInt(MinPrice);
        sm.OrderBindParamInt(MaxPrice);
      end
      else
      begin
        sm.OrderBindParamInt(0);
        sm.OrderBindParamInt(High(Integer));
      end;

      {
        // ((1 = ?) or (A.ItemDBName like ?) or (A.ItemName like ?))
        if Length(Keyword) > 0 then
        begin
        sm.OrderBindParamInt(0);
        sm.OrderBindParamText('%' + Keyword + '%');
        sm.OrderBindParamText('%' + Keyword + '%');
        end
        else
        begin
        sm.OrderBindParamInt(1);
        sm.OrderBindParamText('*');
        sm.OrderBindParamText('*');
        end;
      }

      // ((1 = ?) or (B.HumanName = ?))
      if Length(HumanName) > 0 then
      begin
        sm.OrderBindParamInt(0);
        sm.OrderBindParamText(HumanName);
      end
      else
      begin
        sm.OrderBindParamInt(1);
        sm.OrderBindParamText('*');
      end;

      // (B.IsBusiness >= ?) and (B.IsBusiness <= ?)
      if (g_Config.boOfflineCloseMyShop) then
      begin
        sm.OrderBindParamInt(1);
        sm.OrderBindParamInt(1);
      end
      else
      begin
        sm.OrderBindParamInt(0);
        sm.OrderBindParamInt(1);
      end;

      if (sm.Query and sm.Fetch) then
      begin
        Result := sm.OrderGetColumnValueInt;
      end;

      sm.Reset;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

(*
  function TMySqlUserShopDB.DoGetHumanItems(IsMyShop: Boolean; ItemList: TUserShopItemList; HumanName: string; ShopItemType: TShopItemType): Integer;
  var
  ShopItem: TUserShopItem;
  sm: TMySqlStatement;
  ShopID: Integer;
  begin
  Result := 0;
  try
  if ShopItemType = sitSelling then
  begin
  sm := FStatementGetHumanSellingItem;

  sm.Reset;
  //' and (A.IsAllowSell >= ?) and (A.IsAllowSell <= ?) and length(ifnull(A.BuyerName, '''')) = 0 ' +
  //' and B.IsBusiness >= ? and B.IsBusiness <= ? ' +
  //' and ((B.HumanName = ?) or (1 = ?)) ' +

  if IsMyShop then
  begin
  sm.OrderBindParamInt(1);
  sm.OrderBindParamInt(2);

  sm.OrderBindParamInt(0);
  sm.OrderBindParamInt(1);
  end
  else
  begin
  sm.OrderBindParamInt(1);
  sm.OrderBindParamInt(1);

  if (g_Config.boOfflineCloseMyShop) then
  begin
  sm.OrderBindParamInt(1);
  sm.OrderBindParamInt(1);
  end
  else
  begin
  sm.OrderBindParamInt(0);
  sm.OrderBindParamInt(1);
  end;
  end;
  end
  else if ShopItemType = sitSelled then
  begin
  sm := FStatementGetHumanSelledItem;
  sm.Reset;
  end
  else if ShopItemType = sitStorage then
  begin
  sm := FStatementGetHumanStorageItem;
  sm.Reset;
  end
  else
  begin
  sm := FStatementGetHumanSellingAndStorageItem;
  sm.Reset;
  end;

  if Length(HumanName) > 0 then
  begin
  sm.OrderBindParamInt(0);
  sm.OrderBindParamText(HumanName);
  end
  else
  begin
  sm.OrderBindParamInt(1);
  sm.OrderBindParamText('*');
  end;

  if sm.Query then
  begin
  while (sm.Fetch) do
  begin
  FillChar(ShopItem, SizeOf(ShopItem), 0);

  ShopID := sm.OrderGetColumnValueInt;
  ShopItem.ShopID := ShopID;
  ShopItem.ItemID := sm.OrderGetColumnValueInt;
  ShopItem.btMoneyType := sm.OrderGetColumnValueInt;
  ShopItem.btItemType := sm.OrderGetColumnValueInt;
  ShopItem.btAllowSell := sm.OrderGetColumnValueInt;
  ShopItem.nPrice := sm.OrderGetColumnValueInt;
  ShopItem.dCreateDate := sm.OrderGetColumnValueDateTime;
  ShopItem.boGetMoney := sm.OrderGetColumnValueBool;
  ShopItem.sBuyName := sm.OrderGetColumnValueText;
  ShopItem.sShopName := sm.OrderGetColumnValueText;
  ShopItem.sMasterName := sm.OrderGetColumnValueText;

  Owner.LoadItemFromDB(@ShopItem.UserItem, ShopID, USERSHOP_ITEM_TYPE, ShopItem.ItemID);

  ItemList.Add(@ShopItem);

  Inc(Result);
  end;
  end;

  sm.Reset;
  except
  on E: Exception do
  begin
  MainOutMessage(E.Message);
  end;
  end;
  end;
*)

function TMySqlUserShopDB.DoGetHumanItemWithMakeIndex(IsMyShop: Boolean; HumanName: string; ShopItemType: TShopItemType;
  ItemMakeIndex: Integer; var UserShopItem: TSimpleUserShopItem): Boolean;
var
  sm: TMySqlStatement;
begin
  Result := False;
  try
    if ShopItemType = sitSelling then
    begin
      sm := FStatementGetHumanSellingItem_MakeIndex;

      sm.Reset;
      // ' and (A.IsAllowSell >= ?) and (A.IsAllowSell <= ?) and length(ifnull(A.BuyerName, '''')) = 0 ' +
      // ' and B.IsBusiness >= ? and B.IsBusiness <= ? ' +
      // ' and ((B.HumanName = ?) or (1 = ?)) ' +

      if IsMyShop then
      begin
        sm.OrderBindParamInt(1);
        sm.OrderBindParamInt(2);

        sm.OrderBindParamInt(0);
        sm.OrderBindParamInt(1);
      end
      else
      begin
        sm.OrderBindParamInt(1);
        sm.OrderBindParamInt(1);

        if (g_Config.boOfflineCloseMyShop) then
        begin
          sm.OrderBindParamInt(1);
          sm.OrderBindParamInt(1);
        end
        else
        begin
          sm.OrderBindParamInt(0);
          sm.OrderBindParamInt(1);
        end;
      end;
    end
    else if ShopItemType = sitSelled then
    begin
      sm := FStatementGetHumanSelledItem_MakeIndex;
      sm.Reset;
    end
    else if ShopItemType = sitStorage then
    begin
      sm := FStatementGetHumanStorageItem_MakeIndex;
      sm.Reset;
    end
    else
    begin
      sm := FStatementGetHumanSellingAndStorageItem_MakeIndex;
      sm.Reset;
    end;

    if Length(HumanName) > 0 then
    begin
      sm.OrderBindParamInt(0);
      sm.OrderBindParamText(HumanName);
    end
    else
    begin
      sm.OrderBindParamInt(1);
      sm.OrderBindParamText('*');
    end;

    sm.OrderBindParamInt(ItemMakeIndex);

    if sm.Query and sm.Fetch then
    begin
      FillChar(UserShopItem, SizeOf(UserShopItem), 0);

      UserShopItem.ShopID := sm.OrderGetColumnValueInt;
      UserShopItem.ItemID := sm.OrderGetColumnValueInt;
      UserShopItem.btMoneyType := sm.OrderGetColumnValueInt;
      UserShopItem.btItemType := sm.OrderGetColumnValueInt;
      UserShopItem.btAllowSell := sm.OrderGetColumnValueInt;
      UserShopItem.nPrice := sm.OrderGetColumnValueInt;
      UserShopItem.dCreateDate := sm.OrderGetColumnValueDateTime;
      UserShopItem.boGetMoney := sm.OrderGetColumnValueBool;
      UserShopItem.sBuyName := sm.OrderGetColumnValueText;
      UserShopItem.sShopName := sm.OrderGetColumnValueText;
      UserShopItem.sMasterName := sm.OrderGetColumnValueText;

      UserShopItem.MakeIndex := sm.OrderGetColumnValueInt;
      UserShopItem.wIndex := sm.OrderGetColumnValueInt;
      UserShopItem.boIsBind := sm.OrderGetColumnValueBool;
      UserShopItem.btBindOption := sm.OrderGetColumnValueInt;
      UserShopItem.Dura := sm.OrderGetColumnValueInt;

      Result := True;
    end;

    sm.Reset;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

function TMySqlUserShopDB.DoGetUserShopInfo(HumanName: string; var UserShop: TUserShop): Boolean;
begin
  Result := False;

  FStatementGetUserShopInfo.Reset;
  try
    FStatementGetUserShopInfo.OrderBindParamText(HumanName);
    if FStatementGetUserShopInfo.Query and FStatementGetUserShopInfo.Fetch then
    begin
      UserShop.ShopID := FStatementGetUserShopInfo.OrderGetColumnValueInt;
      UserShop.sMasterName := FStatementGetUserShopInfo.OrderGetColumnValueText;
      UserShop.sShopName := FStatementGetUserShopInfo.OrderGetColumnValueText;
      UserShop.boBusiness := FStatementGetUserShopInfo.OrderGetColumnValueBool;
      UserShop.dCreateDate := FStatementGetUserShopInfo.OrderGetColumnValueDateTime;
      UserShop.nCareValue := FStatementGetUserShopInfo.OrderGetColumnValueInt;
      UserShop.SellItemCount := FStatementGetUserShopInfo.OrderGetColumnValueInt;
      UserShop.SelledItemCount := FStatementGetUserShopInfo.OrderGetColumnValueInt;
      UserShop.StorageItemCount := FStatementGetUserShopInfo.OrderGetColumnValueInt;

      Result := True;
    end;
  finally
    FStatementGetUserShopInfo.Reset;
  end;
end;

procedure TMySqlUserShopDB.DoIncUserShopCareValue(HumanName: string);
begin
  FStatementIncUserShopCareValue.Reset;
  FStatementIncUserShopCareValue.OrderBindParamText(HumanName);
  FStatementIncUserShopCareValue.Step;
  FStatementIncUserShopCareValue.Reset;
end;

function TMySqlUserShopDB.DoAddItem(ShopID: Integer; ShopItem: pTUserShopItem; sItemName: string): Boolean;
var
  ItemID: Integer;
  ChangeName: string;
begin
  Result := False;
  try
    FStatementGetMaxShopItemID.Reset;
    FStatementGetMaxShopItemID.OrderBindParamInt(ShopID);
    if FStatementGetMaxShopItemID.Query and FStatementGetMaxShopItemID.Fetch then
      ItemID := FStatementGetMaxShopItemID.OrderGetColumnValueInt
    else
      ItemID := 1;
  finally
    FStatementGetMaxShopItemID.Reset;
  end;

  FDB.StartTransaction;
  try
    FStatementInsertUserShopItem.Reset;
    try
      FStatementInsertUserShopItem.OrderBindParamInt(ShopID);
      FStatementInsertUserShopItem.OrderBindParamInt(ItemID);
      FStatementInsertUserShopItem.OrderBindParamInt(ShopItem.btItemType);
      FStatementInsertUserShopItem.OrderBindParamDateTime(ShopItem.dCreateDate);
      FStatementInsertUserShopItem.OrderBindParamInt(ShopItem.btAllowSell);
      FStatementInsertUserShopItem.OrderBindParamInt(ShopItem.btMoneyType);
      FStatementInsertUserShopItem.OrderBindParamInt(ShopItem.nPrice);
      FStatementInsertUserShopItem.OrderBindParamBool(ShopItem.boGetMoney);
      FStatementInsertUserShopItem.OrderBindParamText(ShopItem.sBuyName);

      FStatementInsertUserShopItem.OrderBindParamText(sItemName);

      ChangeName := '';
      if (ShopItem.UserItem.btValue[13] = 1) and (Length(ShopItem.UserItem.Name) > 0) then
        ChangeName := ProcessItemName(ShopItem.UserItem.Name);

      FStatementInsertUserShopItem.OrderBindParamText(ChangeName);

      if FStatementInsertUserShopItem.Step then
      begin
        Owner.SaveItemToDB(@ShopItem.UserItem, ShopID, USERSHOP_ITEM_TYPE, ItemID);
        Result := True;
      end;

      FDB.Commit;
    finally
      FStatementInsertUserShopItem.Reset;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
      FDB.RollBack;
    end;
  end;
end;

function TMySqlUserShopDB.DoUpdateItem(ShopID, ItemID: Integer; btItemType, btAllowSell, btMoneyType, nPrice: Integer): Boolean;
begin
  // FStatementUpdateUserShopItem.Sql :=

  // 'update UserShopItem set CreateDate = CURRENT_TIMESTAMP, ItemType = ?, IsAllowSell = ?, MoneyType = ?, ItemPrice = ? where length(ifnull(BuyerName, '''')) = 0 and ShopID = ? and ItemID = ?';
  FStatementUpdateUserShopItem.Reset;
  try
    FStatementUpdateUserShopItem.OrderBindParamInt(btItemType);
    FStatementUpdateUserShopItem.OrderBindParamInt(btAllowSell);
    FStatementUpdateUserShopItem.OrderBindParamInt(btMoneyType);
    FStatementUpdateUserShopItem.OrderBindParamInt(nPrice);
    FStatementUpdateUserShopItem.OrderBindParamInt(ShopID);
    FStatementUpdateUserShopItem.OrderBindParamInt(ItemID);

    Result := FStatementUpdateUserShopItem.Step;
  finally
    FStatementUpdateUserShopItem.Reset;
  end;
end;

function TMySqlUserShopDB.DoDeleteItem(ShopID, ItemID: Integer): Boolean;
var
  S: string;
begin
  Result := False;
  FDB.StartTransaction;
  try
    S := 'delete from ItemElementAdd where ItemType = ' + IntToStr(USERSHOP_ITEM_TYPE) + ' and ParentID = ' + IntToStr(ShopID) +
      ' and ItemIndex = ' + IntToStr(ItemID) + ';' + sLineBreak + 'delete from ItemAddDataByte where ItemType = ' + IntToStr(USERSHOP_ITEM_TYPE)
      + ' and ParentID = ' + IntToStr(ShopID) + ' and ItemIndex = ' + IntToStr(ItemID) + ';' + sLineBreak +
      'delete from ItemAddDataInt where ItemType = ' + IntToStr(USERSHOP_ITEM_TYPE) + ' and ParentID = ' + IntToStr(ShopID) +
      ' and ItemIndex = ' + IntToStr(ItemID) + ';' + sLineBreak + 'delete from ItemAddDataText where ItemType = ' + IntToStr(USERSHOP_ITEM_TYPE)
      + ' and ParentID = ' + IntToStr(ShopID) + ' and ItemIndex = ' + IntToStr(ItemID) + ';' + sLineBreak +
      'delete from ItemFlute where ItemType = ' + IntToStr(USERSHOP_ITEM_TYPE) + ' and ParentID = ' + IntToStr(ShopID) +
      ' and ItemIndex = ' + IntToStr(ItemID) + ';' + sLineBreak + 'delete from ItemProgress where ItemType = ' + IntToStr(USERSHOP_ITEM_TYPE)
      + ' and ParentID = ' + IntToStr(ShopID) + ' and ItemIndex = ' + IntToStr(ItemID) + ';' + sLineBreak +
      'delete from ItemProperty where ItemType = ' + IntToStr(USERSHOP_ITEM_TYPE) + ' and ParentID = ' + IntToStr(ShopID) +
      ' and ItemIndex = ' + IntToStr(ItemID) + ';' + sLineBreak + 'delete from ItemValueAdd where ItemType = ' + IntToStr(USERSHOP_ITEM_TYPE)
      + ' and ParentID = ' + IntToStr(ShopID) + ' and ItemIndex = ' + IntToStr(ItemID) + ';' + sLineBreak +
      'delete from Items where ItemType = ' + IntToStr(USERSHOP_ITEM_TYPE) + ' and ParentID = ' + IntToStr(ShopID) +
      ' and ItemIndex = ' + IntToStr(ItemID) + ';' + sLineBreak +
    // '-------------------------------' + sLineBreak +
      'delete from UserShopItem where ShopID = ' + IntToStr(ShopID) + ' and ItemID = ' + IntToStr(ItemID) + ';';

    FDB.Exec(S);

    FDB.Commit;

    Result := True;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
      FDB.RollBack;
    end;
  end;
end;

function TMySqlUserShopDB.DoBuyItem(ShopID, ItemID: Integer; Buyer: string): Boolean;
begin
  FStatementBuyUserShopItem.Reset;
  try
    FStatementBuyUserShopItem.OrderBindParamText(Buyer);
    FStatementBuyUserShopItem.OrderBindParamInt(ShopID);
    FStatementBuyUserShopItem.OrderBindParamInt(ItemID);

    Result := FStatementBuyUserShopItem.Step;
  finally
    FStatementBuyUserShopItem.Reset;
  end;
end;

function TMySqlUserShopDB.DoGetMoneyItem(ShopID, ItemID: Integer): Boolean;
begin
  FStatementGetMoneyShopItem.Reset;
  try
    FStatementGetMoneyShopItem.OrderBindParamInt(ShopID);
    FStatementGetMoneyShopItem.OrderBindParamInt(ItemID);

    Result := FStatementGetMoneyShopItem.Step;
  finally
    FStatementGetMoneyShopItem.Reset;
  end;
end;

function TMySqlUserShopDB.DoGetSelledAndNoGetMoneyTotal(ItemList: TList): Integer;
var
  Item: PTSelledAndNoGetMoneyTotal;
begin
  Result := 0;
  FStatementGetSelledAndNoGetMoneyTotal.Reset;
  try
    if FStatementGetSelledAndNoGetMoneyTotal.Query then
    begin
      while (FStatementGetSelledAndNoGetMoneyTotal.Fetch) do
      begin
        New(Item);
        ItemList.Add(Item);

        Item.sMasterName := FStatementGetSelledAndNoGetMoneyTotal.OrderGetColumnValueText;
        Item.btMoneyType := FStatementGetSelledAndNoGetMoneyTotal.OrderGetColumnValueInt;
        Item.nSumPrice := FStatementGetSelledAndNoGetMoneyTotal.OrderGetColumnValueInt64;

        Inc(Result);
      end;
    end;
  finally
    FStatementGetSelledAndNoGetMoneyTotal.Reset;
  end;
end;

function TMySqlUserShopDB.DoShopDelete(ShopID: Integer): Boolean;
var
  S: string;
begin
  Result := False;
  FDB.StartTransaction;
  try
    S := 'delete from ItemElementAdd where ItemType = ' + IntToStr(USERSHOP_ITEM_TYPE) + ' and ParentID = ' + IntToStr(ShopID) +
      ';' + sLineBreak + 'delete from ItemAddDataByte where ItemType = ' + IntToStr(USERSHOP_ITEM_TYPE) + ' and ParentID = ' +
      IntToStr(ShopID) + ';' + sLineBreak + 'delete from ItemAddDataInt where ItemType = ' + IntToStr(USERSHOP_ITEM_TYPE) +
      ' and ParentID = ' + IntToStr(ShopID) + ';' + sLineBreak + 'delete from ItemAddDataText where ItemType = ' + IntToStr(USERSHOP_ITEM_TYPE)
      + ' and ParentID = ' + IntToStr(ShopID) + ';' + sLineBreak + 'delete from ItemFlute where ItemType = ' + IntToStr(USERSHOP_ITEM_TYPE)
      + ' and ParentID = ' + IntToStr(ShopID) + ';' + sLineBreak + 'delete from ItemProgress where ItemType = ' + IntToStr(USERSHOP_ITEM_TYPE)
      + ' and ParentID = ' + IntToStr(ShopID) + ';' + sLineBreak + 'delete from ItemProperty where ItemType = ' + IntToStr(USERSHOP_ITEM_TYPE)
      + ' and ParentID = ' + IntToStr(ShopID) + ';' + sLineBreak + 'delete from ItemValueAdd where ItemType = ' + IntToStr(USERSHOP_ITEM_TYPE)
      + ' and ParentID = ' + IntToStr(ShopID) + ';' + sLineBreak + 'delete from Items where ItemType = ' + IntToStr(USERSHOP_ITEM_TYPE)
      + ' and ParentID = ' + IntToStr(ShopID) + ';' + sLineBreak + '-------------------------------' + sLineBreak +
      'delete from UserShopItem where ShopID = ' + IntToStr(ShopID) + ';' + sLineBreak + 'delete from UserShop where ShopID = ' +
      IntToStr(ShopID) + ';';

    FDB.Exec(S);
    FDB.Commit;

    Result := True;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
      FDB.RollBack;
    end;
  end;
end;

function TMySqlUserShopDB.DoShopRename(ShopID: Integer; NewShopName: string): Boolean;
begin
  FStatementUserShopRename.Reset;
  try
    FStatementUserShopRename.OrderBindParamText(NewShopName);
    FStatementUserShopRename.OrderBindParamInt(ShopID);
    Result := FStatementUserShopRename.Step;
  finally
    FStatementUserShopRename.Reset;
  end;
end;

function TMySqlUserShopDB.DoHumanRename(OldName, NewName: string): Boolean;
begin
  Result := False;
  FStatementUserShopRename2.Reset;
  FStatementShopItemBuyerRenameName.Reset;

  try
    FDB.StartTransaction;
    try
      FStatementUserShopRename2.OrderBindParamText(NewName);
      FStatementUserShopRename2.OrderBindParamText(OldName);
      FStatementUserShopRename2.Step;

      FStatementShopItemBuyerRenameName.OrderBindParamText(NewName);
      FStatementShopItemBuyerRenameName.OrderBindParamText(OldName);
      FStatementShopItemBuyerRenameName.Step;

      FDB.Commit;
      Result := True;
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
        FDB.RollBack;
      end;
    end;
  finally
    FStatementUserShopRename2.Reset;
    FStatementShopItemBuyerRenameName.Reset;
  end;
end;

function TMySqlUserShopDB.DoUpdateHumanBusiness(HumanName: string; IsBusiness: Boolean): Boolean;
begin
  FStatementUpdateHumanBusiness.Reset;
  try
    FStatementUpdateHumanBusiness.OrderBindParamBool(IsBusiness);
    FStatementUpdateHumanBusiness.OrderBindParamText(HumanName);
    Result := FStatementUpdateHumanBusiness.Step;
  finally
    FStatementUpdateHumanBusiness.Reset;
  end;
end;

procedure TMySqlUserShopDB.DoRun;
var
  I: Integer;
  ItemList: TList;
  ShopItem: PTSimpleUserShopItem;
  StdItem: pTStdItem;
  IsItemTimeExpired: Boolean;
begin
  if not g_Config.boEnabledMySellShopItemTime then
    Exit;

  ItemList := TList.Create;
  try
    FStatementGetTimeHasArrivedSellItems.Reset;
    try
      FStatementGetTimeHasArrivedSellItems.OrderBindParamInt(g_Config.nMySellShopItemTime * 60);
      if FStatementGetTimeHasArrivedSellItems.Query then
      begin
        while (FStatementGetTimeHasArrivedSellItems.Fetch) do
        begin
          New(ShopItem);
          FillChar(ShopItem^, SizeOf(TSimpleUserShopItem), 0);
          ItemList.Add(ShopItem);

          ShopItem.ShopID := FStatementGetTimeHasArrivedSellItems.OrderGetColumnValueInt;
          ShopItem.ItemID := FStatementGetTimeHasArrivedSellItems.OrderGetColumnValueInt;
          ShopItem.btMoneyType := FStatementGetTimeHasArrivedSellItems.OrderGetColumnValueInt;
          ShopItem.btItemType := FStatementGetTimeHasArrivedSellItems.OrderGetColumnValueInt;
          ShopItem.btAllowSell := FStatementGetTimeHasArrivedSellItems.OrderGetColumnValueInt;
          ShopItem.nPrice := FStatementGetTimeHasArrivedSellItems.OrderGetColumnValueInt;
          ShopItem.dCreateDate := UnixToDateTime(FStatementGetTimeHasArrivedSellItems.OrderGetColumnValueInt64 + 8 * 60 * 60);
          ShopItem.boGetMoney := FStatementGetTimeHasArrivedSellItems.OrderGetColumnValueBool;
          ShopItem.sBuyName := FStatementGetTimeHasArrivedSellItems.OrderGetColumnValueText;
          ShopItem.sShopName := FStatementGetTimeHasArrivedSellItems.OrderGetColumnValueText;
          ShopItem.sMasterName := FStatementGetTimeHasArrivedSellItems.OrderGetColumnValueText;

          ShopItem.MakeIndex := FStatementGetTimeHasArrivedSellItems.OrderGetColumnValueInt;
          ShopItem.wIndex := FStatementGetTimeHasArrivedSellItems.OrderGetColumnValueInt;
          ShopItem.boIsBind := FStatementGetTimeHasArrivedSellItems.OrderGetColumnValueBool;
          ShopItem.btBindOption := FStatementGetTimeHasArrivedSellItems.OrderGetColumnValueInt;
          ShopItem.Dura := FStatementGetTimeHasArrivedSellItems.OrderGetColumnValueInt;
        end;
      end;
    finally
      FStatementGetTimeHasArrivedSellItems.Reset;
    end;

    for I := 0 to ItemList.Count - 1 do
    begin
      ShopItem := ItemList.Items[I];
      StdItem := UserEngine.GetStdItem(ShopItem.wIndex);

      if StdItem <> nil then
      begin
        IsItemTimeExpired := False;
        if (GetUserItemBindValue(ShopItem.btBindOption, ubNoStorage) and ShopItem.boIsBind) // Ω˚÷π¥Ê≤÷ø‚
          or g_ItemRules.Get(ShopItem.wIndex, 9) then
          IsItemTimeExpired := True;

        if (not IsItemTimeExpired) // ºÏ≤‚ «∑Ò¥Ê¬˙
          and (GetSellItemsCount(False, ShopItem.sMasterName, '', -1, -1, 0, 0, sitStorage) //
          >= g_Config.nMaxMyShopStorageItemCount) then
        begin
          IsItemTimeExpired := True;
        end;

        if IsItemTimeExpired then
          ShopItem.btAllowSell := 2
        else
          ShopItem.btAllowSell := 0;

        if g_M2DataDB.UserShopDB.UpdateItem(ShopItem.ShopID, ShopItem.ItemID, ShopItem.btItemType, ShopItem.btAllowSell, ShopItem.btMoneyType,
          ShopItem.nPrice) then
        begin
          if (not IsItemTimeExpired) and (StdItem.NeedIdentify = 1) then
          begin
            AddGameDataLog(LOG_ItemMove, LOG_ActionNone, latHuman, '0', 0, 0, StdItem.Name, ShopItem.MakeIndex, ShopItem.sMasterName,
              '∏ˆ»À…ÃµÍ', 0, 0, 'µÍ∆Ã->µÍ∆Ã≤÷ø‚[µΩ ±ŒÔ∆∑]');
          end;
        end;
      end;

      Dispose(ShopItem);
    end;
  finally
    ItemList.Free;
  end;
end;

end.

