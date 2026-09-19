unit MySqlAuctionDB;

interface

uses
  Windows, Classes, SysUtils, Grobal2, MySqlDataBase, M2DataCommon;

type
  // 拍卖行数据
  TMySqlAuctionDB = class(TAuctionDB)
  private
    FDB: TMySqlDatabase;

    FStatementUpdateAuctionItemFail: TMySqlStatement; // 更新拍卖物品的状态
    FStatementUpdateAuctionItemSuccess: TMySqlStatement; // 更新拍卖物品的状态
    FStatementQueryAuctionItemSuccess: TMySqlStatement; // 查询拍卖成功的物品

    FStatementQueryMyItems: TMySqlStatement; // 查询我的拍卖物品
    FStatementGetMyItemsCount: TMySqlStatement; // 我正在拍卖物品总页数

    FStatementQueryMyAttentionItems: TMySqlStatement; // 查询关注物品
    FStatementQueryMyAttentionItemsCount: TMySqlStatement; // 我的关注物品总页数

    FStatementQueryOneItem: TMySqlStatement; // 查询单个物品

    FStatementGetMyAuctioningItemsCount: TMySqlStatement; // 我正在拍卖物品的数量
    FStatementGetMySellFailItemsCount: TMySqlStatement; // 我流拍未取的物品数量
    FStatementGetMyBuyOKItemsCount: TMySqlStatement; // 我拍买未取的物品数量

    FStatementGetMaxAuctionID: TMySqlStatement;
    FStatementInsertAuctionItem: TMySqlStatement;
    FStatementInsertAttentionItem: TMySqlStatement;
    FStatementCheckInAttentionItem: TMySqlStatement;
    FStatementDeleteAttentionItem: TMySqlStatement;

    FStatementJoinItemBid: TMySqlStatement; // 参与物品竞价
    FStatementBuyItem: TMySqlStatement; // 一口价购买物品

    FStatementAuctionDataHumanRename: TMySqlStatement;
    FStatementAuctionDataLastBidderRename: TMySqlStatement;
    FStatementAuctionAttentionRename: TMySqlStatement;

  protected
    procedure DoInit; override;
    procedure DoFinal; override;

    // 查询商品列表
    function DoQueryAllItems(ItemName: string; ItemGroup: TItemGroup; HumanName: string; nPage, TopmostAuctionID: Integer;
      ItemColors: Integer; SortField: Integer; SortASC: Boolean; MoneyType: Integer; MinPrices, MaxPrices: LongWord; ItemList:
      TAuctionItemList): Integer; override;

    // 查询我的拍卖物品
    function DoQueryMyItems(HumanName: string; nPage: Integer; ItemList: TAuctionItemList): Integer; override;

    // 查询关注物品
    function DoQueryMyAttentionItems(HumanName: string; nPage: Integer; ItemList: TAuctionItemList): Integer; override;

    // 得到商品列表页数
    function DoGetAllItemsPageCount(ItemName: string; ItemGroup: TItemGroup; ItemColors: Integer; MoneyType: Integer; MinPrices,
      MaxPrices: LongWord): Integer; override;

    // 得到我的拍卖物品页数
    function DoGetMyItemsPageCount(HumanName: string): Integer; override;

    // 得到我的关注物品页数
    function DoGetMyAttentionPageCount(HumanName: string): Integer; override;

    // 查询正在拍卖的物品数量
    function DoGetMyAuctioningItemsCount(HumanName: string): Integer; override;

    // 我流拍未取的物品数量
    function DoGetMySellFailItemsCount(HumanName: string): Integer; override;

    // 我拍买到未取的物品数量
    function DoGetMyBuyOKItemsCount(HumanName: string): Integer; override;

    // 添加拍卖物品
    function DoAddAuctionItem(HumanName: string; AuctionTime: Integer; StartingPrice: Integer; SellingPrice: Integer; CurrencyType:
      Integer; UserItem: PTUserItem; StdItem: PTStdItem): Integer; override;

    // 取消拍卖物品
    function DoCancelAuctionItem(HumanName: string; AuctionID: Integer): Boolean; override;

    // 取回拍卖物品
    function DoRetrieveAuctionItem(AuctionID: Integer): Boolean; override;

    // 删除拍卖物品
    function DoDeleteAuctionItem(HumanName: string; AuctionID: Integer): Boolean; override;

    // 添加关注物品
    function DoAddAttentionItem(HumanName: string; Index: Integer): Boolean; override;

    // 删除关注物品
    function DoDeleteAttentionItem(HumanName: string; Index: Integer): Boolean; override;

    // 参加物品竞价
    function DoJoinItemBid(HumanName: string; Index: Integer; Prices: Integer; IsSell: Boolean { 一口价 } ): Boolean; override;

    // 获取竞拍物品信息
    function DoGetAuctionInfo(Index: Integer; var AuctionInfo: TAuctionInfo): Boolean; override;

    // 获取竞拍物品信息
    function DoGetAuctionRecord(Index: Integer; var AuctionRecord: TAuctionRecord): Boolean; override;

    function DoHumanRename(OldName, NewName: string): Boolean; override;

    procedure DoRun; override;
  public
    constructor Create(AOwner: TM2DataDB); override;
    destructor Destroy; override;
  end;

implementation

uses
  ObjPlayer, UsrEngn, DataEngn, M2Share;

{
  Sqlite日期相关。现在数据库用的 unix timestamp

  --select   julianday(strftime('%Y-%m-%d %H:%M',datetime('now','localtime')))
  --select julianday( datetime('2013-10-09 17:40') )

  --SELECT datetime(strftime('%s','now'), 'unixepoch', 'localtime');
  --SELECT  datetime(datetime(strftime('%s','now'), 'unixepoch', 'localtime'), '+7 days');

  --SELECT datetime(1495273549 + 5 * 86400, 'unixepoch', 'localtime');

  SELECT strftime('%s','now'), datetime(strftime('%s','now'), 'unixepoch', 'localtime');
}

{ TAuctionDB }

constructor TMySqlAuctionDB.Create(AOwner: TM2DataDB);
begin
  inherited Create(AOwner);

  FStatementUpdateAuctionItemFail := nil;
  FStatementUpdateAuctionItemSuccess := nil;
  FStatementQueryAuctionItemSuccess := nil;

  FStatementQueryMyItems := nil;
  FStatementGetMyItemsCount := nil;

  FStatementQueryMyAttentionItems := nil;
  FStatementQueryMyAttentionItemsCount := nil;

  FStatementQueryOneItem := nil;

  FStatementGetMyAuctioningItemsCount := nil;
  FStatementGetMySellFailItemsCount := nil;
  FStatementGetMyBuyOKItemsCount := nil;

  FStatementGetMaxAuctionID := nil;
  FStatementInsertAuctionItem := nil;
  FStatementInsertAttentionItem := nil;
  FStatementCheckInAttentionItem := nil;
  FStatementDeleteAttentionItem := nil;

  FStatementJoinItemBid := nil;
  FStatementBuyItem := nil;

  FStatementAuctionDataHumanRename := nil;
  FStatementAuctionDataLastBidderRename := nil;
  FStatementAuctionAttentionRename := nil;
end;

destructor TMySqlAuctionDB.Destroy;
begin
  inherited;
end;

procedure TMySqlAuctionDB.DoInit;
begin
  inherited;

  if (Owner.DataBase <> nil) and (Owner.DataBase is TMySqlDatabase) then
    FDB := Owner.DataBase as TMySqlDatabase;

  FStatementUpdateAuctionItemFail := FDB.Statements.AddSQLStatement('Auction_UpdateAuctionItemFail');
  FStatementUpdateAuctionItemFail.Sql :=
    'update AuctionData set TradingStatus = 1 where (length(ifnull(LastBidder, '''')) = 0) and (ifnull(TradingStatus, 0) = 0) and (TIMESTAMPDIFF(SECOND, CURRENT_TIMESTAMP, date_add(AddDateTime, interval AuctionTime hour)) <= 0);';

  FStatementUpdateAuctionItemSuccess := FDB.Statements.AddSQLStatement('Auction_UpdateAuctionItemSuccess');
  FStatementUpdateAuctionItemSuccess.Sql := 'update AuctionData set TradingStatus = 2 where AuctionID = ?;';

  FStatementQueryAuctionItemSuccess := FDB.Statements.AddSQLStatement('Auction_QueryAuctionItemSuccess');
  FStatementQueryAuctionItemSuccess.Sql :=
    'select A.AuctionID, A.HumanName, A.CurrencyType, A.StartingPrice, A.SellingPrice, A.LastBidder, A.LastBidPrice, B.DBIndex, B.MakeIndex from AuctionData A, Items B '
    + ' where (A.AuctionID = B.ParentID) and (B.ItemType = ' + IntToStr(AUCTION_ITEM_TYPE) + ') and ' +
    ' (length(ifnull(A.LastBidder, '''')) > 0) and (ifnull(A.TradingStatus, 0) = 0) and (TIMESTAMPDIFF(SECOND, CURRENT_TIMESTAMP, date_add(A.AddDateTime, interval A.AuctionTime hour)) <= 0);';

  // 查询我的拍卖物品
  FStatementQueryMyItems := FDB.Statements.AddSQLStatement('Auction_QueryAuctionItems');
  FStatementQueryMyItems.Sql := 'select ' + 'AuctionID, ' + 'HumanName, ' + 'AddDateTime, ' + 'AuctionTime, ' +
    '(TIMESTAMPDIFF(SECOND, CURRENT_TIMESTAMP, date_add(AddDateTime, interval AuctionTime hour))) as TimeLeft, ' +
    'StartingPrice, ' + 'SellingPrice, ' + 'CurrencyType, ' + 'LastBidPrice, ' + 'LastBidder, ' + 'TradingStatus, ' +
    'IsItemGive ' + 'from AuctionData ' + 'where HumanName = ? order by auctionid desc limit ? offset ?;';

  // 我的拍卖物品总页数
  FStatementGetMyItemsCount := FDB.Statements.AddSQLStatement('Auction_GetMyItemsCount');
  FStatementGetMyItemsCount.Sql := 'select ' + 'Count(*) ' + 'from AuctionData ' + 'where HumanName = ?;';

  // 查询我的关注
  FStatementQueryMyAttentionItems := FDB.Statements.AddSQLStatement('Auction_QueryAttentionItems');
  FStatementQueryMyAttentionItems.Sql := 'select ' + 'AuctionID ' + 'from AuctionAttention ' +
    'where HumanName = ? order by Time desc limit ? offset ?;';

  // 我的关注物品总页数
  FStatementQueryMyAttentionItemsCount := FDB.Statements.AddSQLStatement('Auction_GetMyAttentionItemsCount');
  FStatementQueryMyAttentionItemsCount.Sql := 'select ' + 'Count(*) ' + 'from AuctionAttention ' + 'where HumanName = ?;';

  // 查询单个物品信息
  FStatementQueryOneItem := FDB.Statements.AddSQLStatement('Auction_QueryOneItem');
  FStatementQueryOneItem.Sql := 'select ' + 'HumanName, ' + 'AddDateTime, ' + 'AuctionTime, ' +
    '(TIMESTAMPDIFF(SECOND, CURRENT_TIMESTAMP, date_add(AddDateTime, interval AuctionTime hour))) as TimeLeft, ' +
    'StartingPrice, ' + 'SellingPrice, ' + 'CurrencyType, ' + 'LastBidPrice, ' + 'LastBidder, ' + 'TradingStatus, ' +
    'IsItemGive ' + 'from AuctionData ' + 'where AuctionID = ?;';

  // 我正在拍卖的物品数量
  FStatementGetMyAuctioningItemsCount := FDB.Statements.AddSQLStatement('Auction_GetMyAuctioningItemsCount');
  FStatementGetMyAuctioningItemsCount.Sql := 'select ' + 'Count(*) ' + 'from AuctionData ' +
    'where HumanName = ? and (ifnull(TradingStatus, 0) = 0) and (TIMESTAMPDIFF(SECOND, CURRENT_TIMESTAMP, date_add(AddDateTime, interval AuctionTime hour)) > 0);';

  // 我流拍未取的物品数量
  FStatementGetMySellFailItemsCount := FDB.Statements.AddSQLStatement('Auction_GetMySellFailItemsCount');
  FStatementGetMySellFailItemsCount.Sql := 'select ' + 'Count(*) ' + 'from AuctionData ' +
    'where HumanName = ? and IsItemGive = 0 and ((length(ifnull(LastBidder, '''')) = 0) and (ifnull(TradingStatus, 0) = 0) and (TIMESTAMPDIFF(SECOND, CURRENT_TIMESTAMP, date_add(AddDateTime, interval AuctionTime hour)) <= 0) or (ifnull(TradingStatus, 0) = 1))';

  // 我拍买未取的物品数量
  FStatementGetMyBuyOKItemsCount := FDB.Statements.AddSQLStatement('Auction_GetMyBuyOKItemsCount');
  FStatementGetMyBuyOKItemsCount.Sql := 'select ' + 'Count(*) ' + 'from AuctionData ' +
    'where (ifnull(TradingStatus, 0) = 2) and IsItemGive = 0 and LastBidder = ?';

  FStatementGetMaxAuctionID := FDB.Statements.AddSQLStatement('Auction_GetMaxAuctionID');
  FStatementGetMaxAuctionID.Sql := 'select ifnull(Max(AuctionID), 0) + 1 from AuctionData';

  // 添加拍卖物品
  FStatementInsertAuctionItem := FDB.Statements.AddSQLStatement('Auction_InsertAuctionItem');
  FStatementInsertAuctionItem.Sql := 'insert into AuctionData(' + 'AuctionID, ' + 'HumanName, ' + 'ItemGroup, ' + 'ItemColor, ' +
    'AuctionTime, ' + 'StartingPrice, ' + 'SellingPrice, ' + 'CurrencyType, ' + 'ItemDBName,' + 'ItemName) ' +
    'values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?);';

  // 参与物品竞价
  FStatementJoinItemBid := FDB.Statements.AddSQLStatement('Auction_JoinItemBid');
  FStatementJoinItemBid.Sql :=
    'UPDATE AuctionData set LastBidder = ?, LastBidPrice = ?, LastBidTime = CURRENT_TIMESTAMP where AuctionID = ?';

  // 一口价购买物品
  FStatementBuyItem := FDB.Statements.AddSQLStatement('Auction_BuyItem');
  FStatementBuyItem.Sql :=
    'UPDATE AuctionData set LastBidder = ?, LastBidPrice = ?, LastBidTime = CURRENT_TIMESTAMP, TradingStatus = 2 where AuctionID = ?';

  // 添加关注物品
  FStatementInsertAttentionItem := FDB.Statements.AddSQLStatement('Auction_AddAttentionItem');
  FStatementInsertAttentionItem.Sql := 'INSERT INTO AuctionAttention(' + // REPLACE
    'HumanName, ' + 'AuctionID) ' + 'values(?, ?);';
  FStatementCheckInAttentionItem := FDB.Statements.AddSQLStatement('Auction_CheckInAttentionItem');
  FStatementCheckInAttentionItem.Sql := 'SELECT 1 FROM AuctionAttention where HumanName = ? and AuctionID = ?;';

  // 删除关注物品
  FStatementDeleteAttentionItem := FDB.Statements.AddSQLStatement('Auction_DeleteAuctionItem');
  FStatementDeleteAttentionItem.Sql := 'delete from AuctionAttention where HumanName = ? and AuctionID = ?';

  FStatementAuctionDataHumanRename := FDB.Statements.AddSQLStatement('Auction_HumanRename');
  FStatementAuctionDataHumanRename.Sql := 'update AuctionData set HumanName = ? where HumanName = ?';

  FStatementAuctionDataLastBidderRename := FDB.Statements.AddSQLStatement('Auction_LastBidderRename');
  FStatementAuctionDataLastBidderRename.Sql := 'update AuctionData set LastBidder = ? where LastBidder = ?';

  FStatementAuctionAttentionRename := FDB.Statements.AddSQLStatement('Auction_AttentionRename');
  FStatementAuctionAttentionRename.Sql := 'update AuctionAttention set HumanName = ? where HumanName = ?';

  try
    FStatementUpdateAuctionItemFail.Prepare; // 更新拍卖物品的状态
    FStatementUpdateAuctionItemSuccess.Prepare; // 更新拍卖物品的状态
    FStatementQueryAuctionItemSuccess.Prepare;

    FStatementQueryMyItems.Prepare; // 查询我的拍卖物品
    FStatementGetMyItemsCount.Prepare; // 我正在拍卖物品总页数

    FStatementQueryMyAttentionItems.Prepare; // 查询关注物品
    FStatementQueryMyAttentionItemsCount.Prepare; // 我的关注物品总页数
    FStatementQueryOneItem.Prepare; // 查询单个物品数量

    FStatementGetMyAuctioningItemsCount.Prepare; // 我正在拍卖物品的数量
    FStatementGetMySellFailItemsCount.Prepare; // 我流拍未取的物品数量
    FStatementGetMyBuyOKItemsCount.Prepare; // 我拍买未取的物品数量

    FStatementGetMaxAuctionID.Prepare;
    FStatementInsertAuctionItem.Prepare;
    FStatementInsertAttentionItem.Prepare;
    FStatementCheckInAttentionItem.Prepare;
    FStatementDeleteAttentionItem.Prepare;

    FStatementJoinItemBid.Prepare; // 参与物品竞价
    FStatementBuyItem.Prepare; // 一口价购买物品

    FStatementAuctionDataHumanRename.Prepare;
    FStatementAuctionDataLastBidderRename.Prepare;
    FStatementAuctionAttentionRename.Prepare;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

procedure TMySqlAuctionDB.DoFinal;
begin
  inherited;
  if FStatementUpdateAuctionItemFail <> nil then // 更新拍卖物品的状态
  begin
    FStatementUpdateAuctionItemFail.Finalize;
    FStatementUpdateAuctionItemFail := nil;
  end;

  if FStatementUpdateAuctionItemSuccess <> nil then // 更新拍卖物品的状态
  begin
    FStatementUpdateAuctionItemSuccess.Finalize;
    FStatementUpdateAuctionItemSuccess := nil;
  end;

  if FStatementQueryAuctionItemSuccess <> nil then
  begin
    FStatementQueryAuctionItemSuccess.Finalize;
    FStatementQueryAuctionItemSuccess := nil;
  end;

  if FStatementQueryMyItems <> nil then // 查询我的拍卖物品
  begin
    FStatementQueryMyItems.Finalize;
    FStatementQueryMyItems := nil;
  end;

  if FStatementGetMyItemsCount <> nil then // 我正在拍卖物品总页数
  begin
    FStatementGetMyItemsCount.Finalize;
    FStatementGetMyItemsCount := nil;
  end;

  if FStatementQueryMyAttentionItems <> nil then // 查询关注物品
  begin
    FStatementQueryMyAttentionItems.Finalize;
    FStatementQueryMyAttentionItems := nil;
  end;

  if FStatementQueryMyAttentionItemsCount <> nil then // 我的关注物品总页数
  begin
    FStatementQueryMyAttentionItemsCount.Finalize;
    FStatementQueryMyAttentionItemsCount := nil;
  end;

  if FStatementQueryOneItem <> nil then // 查询单个物品数量
  begin
    FStatementQueryOneItem.Finalize;
    FStatementQueryOneItem := nil;
  end;

  if FStatementGetMyAuctioningItemsCount <> nil then // 我正在拍卖物品的数量
  begin
    FStatementGetMyAuctioningItemsCount.Finalize;
    FStatementGetMyAuctioningItemsCount := nil;
  end;

  if FStatementGetMySellFailItemsCount <> nil then // 我正在拍卖物品的数量
  begin
    FStatementGetMySellFailItemsCount.Finalize;
    FStatementGetMySellFailItemsCount := nil;
  end;

  if FStatementGetMyBuyOKItemsCount <> nil then // 我正在拍卖物品的数量
  begin
    FStatementGetMyBuyOKItemsCount.Finalize;
    FStatementGetMyBuyOKItemsCount := nil;
  end;

  if FStatementGetMaxAuctionID <> nil then
  begin
    FStatementGetMaxAuctionID.Finalize;
    FStatementGetMaxAuctionID := nil;
  end;

  if FStatementInsertAuctionItem <> nil then
  begin
    FStatementInsertAuctionItem.Finalize;
    FStatementInsertAuctionItem := nil;
  end;

  if FStatementInsertAttentionItem <> nil then
  begin
    FStatementInsertAttentionItem.Finalize;
    FStatementInsertAttentionItem := nil;
  end;

  if FStatementCheckInAttentionItem <> nil then
  begin
    FStatementCheckInAttentionItem.Finalize;
    FStatementCheckInAttentionItem := nil;
  end;

  if FStatementDeleteAttentionItem <> nil then
  begin
    FStatementDeleteAttentionItem.Finalize;
    FStatementDeleteAttentionItem := nil;
  end;

  if FStatementJoinItemBid <> nil then // 参与物品竞价
  begin
    FStatementJoinItemBid.Finalize;
    FStatementJoinItemBid := nil;
  end;

  if FStatementBuyItem <> nil then // 一口价购买物品
  begin
    FStatementBuyItem.Finalize;
    FStatementBuyItem := nil;
  end;

  if FStatementAuctionDataHumanRename <> nil then
  begin
    FStatementAuctionDataHumanRename.Finalize;
    FStatementAuctionDataHumanRename := nil;
  end;

  if FStatementAuctionDataLastBidderRename <> nil then
  begin
    FStatementAuctionDataLastBidderRename.Finalize;
    FStatementAuctionDataLastBidderRename := nil;
  end;

  if FStatementAuctionAttentionRename <> nil then
  begin
    FStatementAuctionAttentionRename.Finalize;
    FStatementAuctionAttentionRename := nil;
  end;

end;

function TMySqlAuctionDB.DoAddAuctionItem(HumanName: string; AuctionTime, StartingPrice, SellingPrice, CurrencyType: Integer;
  UserItem: PTUserItem; StdItem: PTStdItem): Integer;
var
  AuctionID: Integer;
  ItemGroup: TItemGroup;
  ChangeName: string;
begin
  Result := 0;
  try
    FStatementGetMaxAuctionID.Reset;
    if FStatementGetMaxAuctionID.Query and FStatementGetMaxAuctionID.Fetch then
      AuctionID := FStatementGetMaxAuctionID.OrderGetColumnValueInt
    else
      AuctionID := 1;
  finally
    FStatementGetMaxAuctionID.Reset;
  end;

  ItemGroup := igOther;
  if StdItem.StdMode in [10, 11] then // 衣服
    ItemGroup := igDress
  else if StdItem.StdMode in [5, 6] then // 武器
    ItemGroup := igWeapon
  else if StdItem.StdMode in [28, 30] then // 照明物
    ItemGroup := igSpecial
  else if StdItem.StdMode in [19, 20, 21] then // 项链
  begin
    if (StdItem.OverLap in [2, 4, 6]) then
      ItemGroup := igSpecial
    else
      ItemGroup := igNecklace
  end
  else if StdItem.StdMode in [15, 78] then // 头盔
  begin
    if (StdItem.StdMode = 15) and (StdItem.OverLap in [2, 4, 6]) then
      ItemGroup := igSpecial
    else
      ItemGroup := igHelmet
  end
  else if StdItem.StdMode in [24, 26] then // 手镯
  begin
    if (StdItem.OverLap in [2, 4, 6]) then
      ItemGroup := igSpecial
    else
      ItemGroup := igArmRing
  end
  else if StdItem.StdMode in [22, 23] then // 戒指
  begin
    if (StdItem.OverLap in [2, 4, 6]) then
      ItemGroup := igSpecial
    else
      ItemGroup := igRing
  end
  else if StdItem.StdMode in [25, 51] then // 符毒
    ItemGroup := igSpecial
  else if StdItem.StdMode in [54, 64] then // 腰带
    ItemGroup := igBelt
  else if StdItem.StdMode in [52, 62] then // 靴子
    ItemGroup := igBoots
  else if StdItem.StdMode in [53, 63, 7] then // 宝石
    ItemGroup := igSpecial
  else if StdItem.StdMode in [66..89] then // 时装
    ItemGroup := igFashion
  else if StdItem.StdMode = 16 then // 斗笠
    ItemGroup := igSpecial
  else if StdItem.StdMode = 65 then // 军鼓
    ItemGroup := igSpecial
  else if StdItem.StdMode = 28 then // 马牌
    ItemGroup := igSpecial
  else if StdItem.StdMode = 12 then // 盾牌
    ItemGroup := igSpecial
  else if StdItem.StdMode = 90 then // 灵玉
    ItemGroup := igSpecial
  else if (StdItem.StdMode in [0, 1]) or ((StdItem.StdMode = 3) and (StdItem.Shape = 12)) then // 药品
    ItemGroup := igDrug
  else if StdItem.StdMode = 4 then // 技能书籍
    ItemGroup := igSpecial;

  if AuctionID > 0 then
  begin
    FDB.StartTransaction;
    try
      try
        FStatementInsertAuctionItem.Reset;
        FStatementInsertAuctionItem.OrderBindParamInt(AuctionID);
        FStatementInsertAuctionItem.OrderBindParamText(HumanName);
        FStatementInsertAuctionItem.OrderBindParamInt(Integer(ItemGroup));

        if UserItem.btColor > 0 then
          FStatementInsertAuctionItem.OrderBindParamInt(UserItem.btColor)
        else
          FStatementInsertAuctionItem.OrderBindParamInt(StdItem.Color);

        FStatementInsertAuctionItem.OrderBindParamInt(AuctionTime);
        FStatementInsertAuctionItem.OrderBindParamInt(StartingPrice);
        FStatementInsertAuctionItem.OrderBindParamInt(SellingPrice);
        FStatementInsertAuctionItem.OrderBindParamInt(CurrencyType);

        FStatementInsertAuctionItem.OrderBindParamText(StdItem.DBName);

        ChangeName := '';
        if (UserItem.btValue[13] = 1) and (Length(UserItem.Name) > 0) then
          ChangeName := ProcessItemName(UserItem.Name);

        FStatementInsertAuctionItem.OrderBindParamText(ChangeName);

        if FStatementInsertAuctionItem.Step then
        begin
          Owner.SaveItemToDB(UserItem, AuctionID, AUCTION_ITEM_TYPE, 0);

          Result := AuctionID;
        end;
      finally
        FStatementInsertAuctionItem.Reset;
      end;

      FDB.Commit;
    except
      on E: Exception do
      begin
        MainOutMessage(E.Message);
        FDB.RollBack;
      end;
    end;
  end;
end;

// 取消拍卖物品
function TMySqlAuctionDB.DoCancelAuctionItem(HumanName: string; AuctionID: Integer): Boolean;
begin
  Result := False;
  try
    FDB.Exec('update AuctionData set TradingStatus = 1 where AuctionID = ' + IntToStr(AuctionID) + ' and HumanName = "' +
      HumanName + '";');
    Result := True;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

// 取回拍卖物品
function TMySqlAuctionDB.DoRetrieveAuctionItem(AuctionID: Integer): Boolean;
begin
  Result := False;
  try
    FDB.Exec('update AuctionData set IsItemGive = 1 where AuctionID = ' + IntToStr(AuctionID) + ';');
    Result := True;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

// 删除拍卖物品
function TMySqlAuctionDB.DoDeleteAuctionItem(HumanName: string; AuctionID: Integer): Boolean;
var
  Sql: string;
  sWhere: string;
begin
  Result := False;
  sWhere := Format(' WHERE ParentID = %d and ItemType = ' + IntToStr(AUCTION_ITEM_TYPE) + ' and ItemIndex = 0;', [AuctionID]);
  FDB.StartTransaction;
  try
    Sql := 'DELETE FROM ItemElementAdd' + sWhere + sLineBreak + 'DELETE FROM ItemAddDataByte' + sWhere + sLineBreak +
      'DELETE FROM ItemAddDataInt' + sWhere + sLineBreak + 'DELETE FROM ItemAddDataText' + sWhere + sLineBreak +
      'DELETE FROM ItemFlute' + sWhere + sLineBreak + 'DELETE FROM ItemProgress' + sWhere + sLineBreak +
      'DELETE FROM ItemProperty' + sWhere + sLineBreak + 'DELETE FROM ItemValueAdd' + sWhere + sLineBreak + 'DELETE FROM Items' +
      sWhere + sLineBreak + 'DELETE FROM AuctionAttention WHERE AuctionID = ' + IntToStr(AuctionID) + ';' + sLineBreak +
      'DELETE FROM AuctionData WHERE AuctionID = ' + IntToStr(AuctionID) + ';' + sLineBreak;

    FDB.Exec(Sql);
    FDB.ClearResult;

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

function TMySqlAuctionDB.DoAddAttentionItem(HumanName: string; Index: Integer): Boolean;
var
  IsAttentioned: Boolean;
begin
  Result := False;
  try
    try
      FStatementCheckInAttentionItem.Reset;
      FStatementCheckInAttentionItem.OrderBindParamText(HumanName);
      FStatementCheckInAttentionItem.OrderBindParamInt(Index);
      FStatementCheckInAttentionItem.Query; // 修复查询不到结果的问题  By 一支笔 at:2022-01-06 16:46:44
      IsAttentioned := FStatementCheckInAttentionItem.RowCount <> 0;
      // IsAttentioned := FStatementCheckInAttentionItem.Step;
    finally
      FStatementCheckInAttentionItem.Reset;
    end;

    if not IsAttentioned then
    begin
      try
        FStatementInsertAttentionItem.Reset;
        FStatementInsertAttentionItem.OrderBindParamText(HumanName);
        FStatementInsertAttentionItem.OrderBindParamInt(Index);
        Result := FStatementInsertAttentionItem.Step;
      finally
        FStatementInsertAttentionItem.Reset;
      end;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

function TMySqlAuctionDB.DoDeleteAttentionItem(HumanName: string; Index: Integer): Boolean;
var
  IsAttentioned: Boolean;
begin
  Result := False;
  try
    try
      FStatementCheckInAttentionItem.Reset;
      FStatementCheckInAttentionItem.OrderBindParamText(HumanName);
      FStatementCheckInAttentionItem.OrderBindParamInt(Index);
      FStatementCheckInAttentionItem.Query; // 修复查询不到结果的问题  By 一支笔 at:2022-01-06 16:46:44
      IsAttentioned := FStatementCheckInAttentionItem.RowCount <> 0;
      // IsAttentioned := FStatementCheckInAttentionItem.Step;
    finally
      FStatementCheckInAttentionItem.Reset;
    end;

    if IsAttentioned then
    begin
      try
        FStatementDeleteAttentionItem.Reset;
        FStatementDeleteAttentionItem.OrderBindParamText(HumanName);
        FStatementDeleteAttentionItem.OrderBindParamInt(Index);
        Result := FStatementDeleteAttentionItem.Step;
      finally
        FStatementDeleteAttentionItem.Reset;
      end;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

function TMySqlAuctionDB.DoJoinItemBid(HumanName: string; Index, Prices: Integer; IsSell: Boolean): Boolean;
begin
  Result := False;
  try
    AddAttentionItem(HumanName, Index);

    if IsSell then
    begin
      try
        FStatementBuyItem.Reset;
        FStatementBuyItem.OrderBindParamText(HumanName);
        FStatementBuyItem.OrderBindParamInt(Prices);
        FStatementBuyItem.OrderBindParamInt(Index);
        Result := FStatementBuyItem.Step;
      finally
        FStatementBuyItem.Reset;
      end;
    end
    else
    begin
      try
        FStatementJoinItemBid.Reset;
        FStatementJoinItemBid.OrderBindParamText(HumanName);
        FStatementJoinItemBid.OrderBindParamInt(Prices);
        FStatementJoinItemBid.OrderBindParamInt(Index);
        Result := FStatementJoinItemBid.Step;
      finally
        FStatementJoinItemBid.Reset;
      end;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

function TMySqlAuctionDB.DoQueryAllItems(ItemName: string; ItemGroup: TItemGroup; HumanName: string; nPage, TopmostAuctionID:
  Integer; ItemColors: Integer; SortField: Integer; SortASC: Boolean; MoneyType: Integer; MinPrices, MaxPrices: LongWord; ItemList:
  TAuctionItemList): Integer;
var
  AuctionRecord: TAuctionRecord;
  sm: TMySqlStatement;
  sOrderBy, sColors: string;
begin
  Result := 0;
  try
    // 查询可竞拍的商品列表
    sm := FDB.Statements.AddSQLStatement('Auction_QueryAllItems');
    try
      sm.Sql := 'select ' + 'A.AuctionID, ' + 'A.HumanName, ' + 'A.AddDateTime, ' + 'A.AuctionTime, ' +
        '(TIMESTAMPDIFF(SECOND, CURRENT_TIMESTAMP, date_add(A.AddDateTime, interval A.AuctionTime hour))) as TimeLeft, ' +
        'A.StartingPrice, ' + 'A.SellingPrice, ' + 'A.CurrencyType, ' + 'A.LastBidPrice, ' + 'A.LastBidder, ' +
        'A.TradingStatus, ' + 'A.IsItemGive, ' +
        'ifnull((select 1 from AuctionAttention where AuctionID = A.AuctionID and HumanName = ?), 0) as IsAttention ' +
        'from AuctionData A ' +
        'where (ifnull(A.TradingStatus, 0) = 0) and (TIMESTAMPDIFF(SECOND, CURRENT_TIMESTAMP, date_add(A.AddDateTime, interval A.AuctionTime hour)) > 0) and (A.AuctionID <> '
        + IntToStr(TopmostAuctionID) + ') ';

      if ItemGroup <> igAll then
      begin
        sm.Sql := sm.Sql + ' and (ItemGroup = ' + IntToStr(Integer(ItemGroup)) + ')';
      end;

      if ItemColors <> 0 then
      begin
        if ItemColors and 1 <> 0 then
        begin
          sColors := sColors + IntToStr(g_Config.btAuctionItemColors[0]) + ',';
        end;

        if ItemColors and 2 <> 0 then
        begin
          sColors := sColors + IntToStr(g_Config.btAuctionItemColors[1]) + ',';
        end;

        if ItemColors and 4 <> 0 then
        begin
          sColors := sColors + IntToStr(g_Config.btAuctionItemColors[2]) + ',';
        end;

        if ItemColors and 8 <> 0 then
        begin
          sColors := sColors + IntToStr(g_Config.btAuctionItemColors[3]) + ',';
        end;

        if ItemColors and 16 <> 0 then
        begin
          sColors := sColors + IntToStr(g_Config.btAuctionItemColors[4]) + ',';
        end;

        if ItemColors and 32 <> 0 then
        begin
          sColors := sColors + IntToStr(g_Config.btAuctionItemColors[5]) + ',';
        end;

        if Length(sColors) > 0 then
        begin
          sm.Sql := sm.Sql + ' and ItemColor in (' + Copy(sColors, 1, Length(sColors) - 1) + ')';
        end;
      end;

      if MoneyType > 0 then
      begin
        sm.Sql := sm.Sql + ' and A.CurrencyType = ' + IntToStr(MoneyType - 1);
      end;

      if MinPrices > 0 then
      begin
        sm.Sql := sm.Sql + ' and A.SellingPrice >= ' + IntToStr(MinPrices);
      end;

      if MaxPrices > 0 then
      begin
        sm.Sql := sm.Sql + ' and A.SellingPrice <= ' + IntToStr(MaxPrices);
      end;

      if Length(ItemName) > 0 then
      begin
        sm.Sql := sm.Sql + ' and ((A.ItemDBName like "%' + ItemName + '%") or (A.ItemName like "%' + ItemName + '%"))';
      end;

      if SortField = 1 then
      begin
        if SortASC then
          sOrderBy := ' order by ifnull(A.LastBidPrice, A.StartingPrice)'
        else
          sOrderBy := ' order by ifnull(A.LastBidPrice, A.StartingPrice) desc';
      end
      else if SortField = 2 then
      begin
        if SortASC then
          sOrderBy := ' order by A.SellingPrice'
        else
          sOrderBy := ' order by A.SellingPrice desc';
      end
      else if SortField = 3 then
      begin
        if SortASC then
          sOrderBy := ' order by TIMESTAMPDIFF(SECOND, CURRENT_TIMESTAMP, date_add(AddDateTime, interval AuctionTime hour))'
        else
          sOrderBy := ' order by TIMESTAMPDIFF(SECOND, CURRENT_TIMESTAMP, date_add(AddDateTime, interval AuctionTime hour)) desc';
      end
      else if SortField = 0 then
      begin
        sOrderBy := ' order by AuctionID desc';
      end;

      sm.Sql := sm.Sql + sOrderBy + ' limit ? offset ?;';

      sm.Prepare;

      sm.OrderBindParamText(HumanName);
      sm.OrderBindParamInt(AUCTION_PAGE_COUNT);
      sm.OrderBindParamInt((nPage - 1) * AUCTION_PAGE_COUNT);
      if sm.Query then
      begin
        while (sm.Fetch) do
        begin
          AuctionRecord.AuctionID := sm.OrderGetColumnValueInt;
          AuctionRecord.HumanName := sm.OrderGetColumnValueText;
          AuctionRecord.AddDateTime := sm.OrderGetColumnValueDateTime;
          AuctionRecord.AuctionTime := sm.OrderGetColumnValueInt;
          AuctionRecord.TimeLeft := sm.OrderGetColumnValueInt;
          AuctionRecord.StartingPrice := sm.OrderGetColumnValueInt;
          AuctionRecord.SellingPrice := sm.OrderGetColumnValueInt;
          AuctionRecord.CurrencyType := sm.OrderGetColumnValueInt;
          AuctionRecord.LastBidPrice := sm.OrderGetColumnValueInt;
          AuctionRecord.LastBidder := sm.OrderGetColumnValueText;
          AuctionRecord.TradingStatus := sm.OrderGetColumnValueInt;
          AuctionRecord.IsItemGive := sm.OrderGetColumnValueBool;
          AuctionRecord.IsAttention := sm.OrderGetColumnValueBool;

          Owner.LoadItemFromDB(@AuctionRecord.ActionItem, AuctionRecord.AuctionID, AUCTION_ITEM_TYPE, 0);

          ItemList.Add(@AuctionRecord);

          Inc(Result);
        end;
      end;
    finally
      sm.Reset;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

// 查询我的拍卖物品
function TMySqlAuctionDB.DoQueryMyItems(HumanName: string; nPage: Integer; ItemList: TAuctionItemList): Integer;
var
  AuctionRecord: TAuctionRecord;
begin
  Result := 0;
  try
    try
      FStatementQueryMyItems.Reset;
      FStatementQueryMyItems.OrderBindParamText(HumanName);
      FStatementQueryMyItems.OrderBindParamInt(AUCTION_PAGE_COUNT);
      FStatementQueryMyItems.OrderBindParamInt((nPage - 1) * AUCTION_PAGE_COUNT);

      if FStatementQueryMyItems.Query then
      begin
        while (FStatementQueryMyItems.Fetch) do
        begin
          AuctionRecord.AuctionID := FStatementQueryMyItems.OrderGetColumnValueInt;
          AuctionRecord.HumanName := FStatementQueryMyItems.OrderGetColumnValueText;
          AuctionRecord.AddDateTime := FStatementQueryMyItems.OrderGetColumnValueDateTime;
          AuctionRecord.AuctionTime := FStatementQueryMyItems.OrderGetColumnValueInt;
          AuctionRecord.TimeLeft := FStatementQueryMyItems.OrderGetColumnValueInt;
          AuctionRecord.StartingPrice := FStatementQueryMyItems.OrderGetColumnValueInt;
          AuctionRecord.SellingPrice := FStatementQueryMyItems.OrderGetColumnValueInt;
          AuctionRecord.CurrencyType := FStatementQueryMyItems.OrderGetColumnValueInt;
          AuctionRecord.LastBidPrice := FStatementQueryMyItems.OrderGetColumnValueInt;
          AuctionRecord.LastBidder := FStatementQueryMyItems.OrderGetColumnValueText;
          AuctionRecord.TradingStatus := FStatementQueryMyItems.OrderGetColumnValueInt;
          AuctionRecord.IsItemGive := FStatementQueryMyItems.OrderGetColumnValueBool;

          Owner.LoadItemFromDB(@AuctionRecord.ActionItem, AuctionRecord.AuctionID, AUCTION_ITEM_TYPE, 0);

          ItemList.Add(@AuctionRecord);

          Inc(Result);
        end;
      end;
    finally
      FStatementQueryMyItems.Reset;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

function TMySqlAuctionDB.DoQueryMyAttentionItems(HumanName: string; nPage: Integer; ItemList: TAuctionItemList): Integer;
var
  AuctionRecord: TAuctionRecord;
begin
  Result := 0;
  try
    try
      FStatementQueryMyAttentionItems.Reset;
      FStatementQueryMyAttentionItems.OrderBindParamText(HumanName);
      FStatementQueryMyAttentionItems.OrderBindParamInt(AUCTION_PAGE_COUNT);
      FStatementQueryMyAttentionItems.OrderBindParamInt((nPage - 1) * AUCTION_PAGE_COUNT);

      if FStatementQueryMyAttentionItems.Query then
      begin
        while (FStatementQueryMyAttentionItems.Fetch) do
        begin
          AuctionRecord.AuctionID := FStatementQueryMyAttentionItems.OrderGetColumnValueInt;

          FStatementQueryOneItem.Reset;
          FStatementQueryOneItem.OrderBindParamInt(AuctionRecord.AuctionID);
          if FStatementQueryOneItem.Query and FStatementQueryOneItem.Fetch then
          begin
            AuctionRecord.HumanName := FStatementQueryOneItem.OrderGetColumnValueText;
            AuctionRecord.AddDateTime := FStatementQueryOneItem.OrderGetColumnValueDateTime;
            AuctionRecord.AuctionTime := FStatementQueryOneItem.OrderGetColumnValueInt;
            AuctionRecord.TimeLeft := FStatementQueryOneItem.OrderGetColumnValueInt;
            AuctionRecord.StartingPrice := FStatementQueryOneItem.OrderGetColumnValueInt;
            AuctionRecord.SellingPrice := FStatementQueryOneItem.OrderGetColumnValueInt;
            AuctionRecord.CurrencyType := FStatementQueryOneItem.OrderGetColumnValueInt;
            AuctionRecord.LastBidPrice := FStatementQueryOneItem.OrderGetColumnValueInt;
            AuctionRecord.LastBidder := FStatementQueryOneItem.OrderGetColumnValueText;
            AuctionRecord.TradingStatus := FStatementQueryOneItem.OrderGetColumnValueInt;
            AuctionRecord.IsItemGive := FStatementQueryOneItem.OrderGetColumnValueBool;
          end;

          Owner.LoadItemFromDB(@AuctionRecord.ActionItem, AuctionRecord.AuctionID, AUCTION_ITEM_TYPE, 0);

          ItemList.Add(@AuctionRecord);

          Inc(Result);
        end;
      end;

    finally
      FStatementQueryOneItem.Reset;
      FStatementQueryMyAttentionItems.Reset;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

function TMySqlAuctionDB.DoGetAuctionInfo(Index: Integer; var AuctionInfo: TAuctionInfo): Boolean;
begin
  Result := False;
  try
    try
      FStatementQueryOneItem.Reset;
      FStatementQueryOneItem.OrderBindParamInt(Index);
      if FStatementQueryOneItem.Query and FStatementQueryOneItem.Fetch then
      begin
        AuctionInfo.AuctionID := Index;
        AuctionInfo.HumanName := FStatementQueryOneItem.OrderGetColumnValueText;
        AuctionInfo.AddDateTime := FStatementQueryOneItem.OrderGetColumnValueDateTime;
        AuctionInfo.AuctionTime := FStatementQueryOneItem.OrderGetColumnValueInt;
        AuctionInfo.TimeLeft := FStatementQueryOneItem.OrderGetColumnValueInt;
        AuctionInfo.StartingPrice := FStatementQueryOneItem.OrderGetColumnValueInt;
        AuctionInfo.SellingPrice := FStatementQueryOneItem.OrderGetColumnValueInt;
        AuctionInfo.CurrencyType := FStatementQueryOneItem.OrderGetColumnValueInt;
        AuctionInfo.LastBidPrice := FStatementQueryOneItem.OrderGetColumnValueInt;
        AuctionInfo.LastBidder := FStatementQueryOneItem.OrderGetColumnValueText;
        AuctionInfo.TradingStatus := FStatementQueryOneItem.OrderGetColumnValueInt;
        AuctionInfo.IsItemGive := FStatementQueryOneItem.OrderGetColumnValueBool;

        Result := True;
      end;
    finally
      FStatementQueryOneItem.Reset;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

function TMySqlAuctionDB.DoGetAuctionRecord(Index: Integer; var AuctionRecord: TAuctionRecord): Boolean;
begin
  Result := False;
  try
    try
      FStatementQueryOneItem.Reset;
      FStatementQueryOneItem.OrderBindParamInt(Index);
      if FStatementQueryOneItem.Query and FStatementQueryOneItem.Fetch then
      begin
        AuctionRecord.AuctionID := Index;
        AuctionRecord.HumanName := FStatementQueryOneItem.OrderGetColumnValueText;
        AuctionRecord.AddDateTime := FStatementQueryOneItem.OrderGetColumnValueDateTime;
        AuctionRecord.AuctionTime := FStatementQueryOneItem.OrderGetColumnValueInt;
        AuctionRecord.TimeLeft := FStatementQueryOneItem.OrderGetColumnValueInt;
        AuctionRecord.StartingPrice := FStatementQueryOneItem.OrderGetColumnValueInt;
        AuctionRecord.SellingPrice := FStatementQueryOneItem.OrderGetColumnValueInt;
        AuctionRecord.CurrencyType := FStatementQueryOneItem.OrderGetColumnValueInt;
        AuctionRecord.LastBidPrice := FStatementQueryOneItem.OrderGetColumnValueInt;
        AuctionRecord.LastBidder := FStatementQueryOneItem.OrderGetColumnValueText;
        AuctionRecord.TradingStatus := FStatementQueryOneItem.OrderGetColumnValueInt;
        AuctionRecord.IsItemGive := FStatementQueryOneItem.OrderGetColumnValueBool;
        AuctionRecord.IsAttention := False;

        Owner.LoadItemFromDB(@AuctionRecord.ActionItem, AuctionRecord.AuctionID, AUCTION_ITEM_TYPE, 0);

        Result := True;
      end;
    finally
      FStatementQueryOneItem.Reset;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

function TMySqlAuctionDB.DoGetAllItemsPageCount(ItemName: string; ItemGroup: TItemGroup; ItemColors: Integer; MoneyType: Integer;
  MinPrices, MaxPrices: LongWord): Integer;
var
  sm: TMySqlStatement;
  sColors: string;
begin
  Result := 0;
  try
    sm := FDB.Statements.AddSQLStatement('Auction_GetAllItemsCount');
    try
      // 查询所有拍卖物品总页数
      sm.Sql := 'select ' + 'Count(*) ' + 'from AuctionData ' +
        'where (ifnull(TradingStatus, 0) = 0) and (TIMESTAMPDIFF(SECOND, CURRENT_TIMESTAMP, date_add(AddDateTime, interval AuctionTime hour)) > 0)';

      if ItemGroup <> igAll then
      begin
        sm.Sql := sm.Sql + ' and ItemGroup = ' + IntToStr(Integer(ItemGroup));
      end;

      if ItemColors <> 0 then
      begin
        if ItemColors and 1 <> 0 then
        begin
          sColors := sColors + IntToStr(g_Config.btAuctionItemColors[0]) + ',';
        end;

        if ItemColors and 2 <> 0 then
        begin
          sColors := sColors + IntToStr(g_Config.btAuctionItemColors[1]) + ',';
        end;

        if ItemColors and 4 <> 0 then
        begin
          sColors := sColors + IntToStr(g_Config.btAuctionItemColors[2]) + ',';
        end;

        if ItemColors and 8 <> 0 then
        begin
          sColors := sColors + IntToStr(g_Config.btAuctionItemColors[3]) + ',';
        end;

        if ItemColors and 16 <> 0 then
        begin
          sColors := sColors + IntToStr(g_Config.btAuctionItemColors[4]) + ',';
        end;

        if ItemColors and 32 <> 0 then
        begin
          sColors := sColors + IntToStr(g_Config.btAuctionItemColors[5]) + ',';
        end;

        if Length(sColors) > 0 then
        begin
          sm.Sql := sm.Sql + ' and ItemColor in (' + Copy(sColors, 1, Length(sColors) - 1) + ')';
        end;
      end;

      if MoneyType > 0 then
      begin
        sm.Sql := sm.Sql + ' and CurrencyType = ' + IntToStr(MoneyType - 1);
      end;

      if MinPrices > 0 then
      begin
        sm.Sql := sm.Sql + ' and SellingPrice >= ' + IntToStr(MinPrices);
      end;

      if MaxPrices > 0 then
      begin
        sm.Sql := sm.Sql + ' and SellingPrice <= ' + IntToStr(MaxPrices);
      end;

      if Length(ItemName) > 0 then
      begin
        sm.Sql := sm.Sql + ' and ((ItemDBName like "%' + ItemName + '%") or (ItemName like "%' + ItemName + '%"))';
      end;

      sm.Prepare;
      if sm.Query and sm.Fetch then
      begin
        Result := (sm.OrderGetColumnValueInt + AUCTION_PAGE_COUNT - 1) div AUCTION_PAGE_COUNT;
      end;
    finally
      sm.Reset;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

function TMySqlAuctionDB.DoGetMyItemsPageCount(HumanName: string): Integer;
begin
  Result := 0;
  try
    FStatementGetMyItemsCount.Reset;
    FStatementGetMyItemsCount.OrderBindParamText(HumanName);
    if FStatementGetMyItemsCount.Query and FStatementGetMyItemsCount.Fetch then
    begin
      Result := (FStatementGetMyItemsCount.OrderGetColumnValueInt + AUCTION_PAGE_COUNT - 1) div AUCTION_PAGE_COUNT;
    end;
  finally
    FStatementGetMyItemsCount.Reset;
  end;
end;

function TMySqlAuctionDB.DoGetMyAttentionPageCount(HumanName: string): Integer;
begin
  Result := 0;
  try
    try
      FStatementQueryMyAttentionItemsCount.Reset;
      FStatementQueryMyAttentionItemsCount.OrderBindParamText(HumanName);
      if FStatementQueryMyAttentionItemsCount.Query and FStatementQueryMyAttentionItemsCount.Fetch then
      begin
        Result := (FStatementQueryMyAttentionItemsCount.OrderGetColumnValueInt + AUCTION_PAGE_COUNT - 1) div AUCTION_PAGE_COUNT;
      end;
    finally
      FStatementQueryMyAttentionItemsCount.Reset;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

function TMySqlAuctionDB.DoGetMyAuctioningItemsCount(HumanName: string): Integer;
begin
  Result := 0;
  try
    try
      FStatementGetMyAuctioningItemsCount.Reset;
      FStatementGetMyAuctioningItemsCount.OrderBindParamText(HumanName);
      if FStatementGetMyAuctioningItemsCount.Query and FStatementGetMyAuctioningItemsCount.Fetch then
      begin
        Result := FStatementGetMyAuctioningItemsCount.OrderGetColumnValueInt;
      end;
    finally
      FStatementGetMyAuctioningItemsCount.Reset;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

// 我流拍未取的物品数量
function TMySqlAuctionDB.DoGetMySellFailItemsCount(HumanName: string): Integer;
begin
  Result := 0;
  try
    try
      FStatementGetMySellFailItemsCount.Reset;
      FStatementGetMySellFailItemsCount.OrderBindParamText(HumanName);
      if FStatementGetMySellFailItemsCount.Query and FStatementGetMySellFailItemsCount.Fetch then
      begin
        Result := FStatementGetMySellFailItemsCount.OrderGetColumnValueInt;
      end;
    finally
      FStatementGetMySellFailItemsCount.Reset;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

// 我拍买到未取的物品数量
function TMySqlAuctionDB.DoGetMyBuyOKItemsCount(HumanName: string): Integer;
begin
  Result := 0;
  try
    try
      FStatementGetMyBuyOKItemsCount.Reset;
      FStatementGetMyBuyOKItemsCount.OrderBindParamText(HumanName);
      if FStatementGetMyBuyOKItemsCount.Query and FStatementGetMyBuyOKItemsCount.Fetch then
      begin
        Result := FStatementGetMyBuyOKItemsCount.OrderGetColumnValueInt;
      end;
    finally
      FStatementGetMyBuyOKItemsCount.Reset;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

procedure TMySqlAuctionDB.DoRun;

  procedure IncPlayerGameMoney(PlayerName: string; MakeIndex: Integer; nCurrencyType, Prices: Integer; LogAdd: string);
  var
    nValue: Int64;
    Player: TPlayObject;
    GoldType: TDBChangeGoldType;
  begin
    Player := UserEngine.GetPlayObject(PlayerName);
    if Player <> nil then
    begin
      case nCurrencyType of
        0:
          begin
            nValue := Int64(Player.m_nGameGold) + Prices;
            if nValue > High(LongWord) then
              nValue := High(LongWord);
            Player.m_nGameGold := nValue;

            Player.GameGoldChanged;
          end;
        1:
          begin
            nValue := Int64(Player.m_nGamePoint) + Prices;
            if nValue > High(LongWord) then
              nValue := High(LongWord);
            Player.m_nGamePoint := nValue;

            Player.GameGoldChanged;
          end;
        2:
          begin
            nValue := Int64(Player.m_nGold) + Prices;
            if nValue > g_Config.nHumanMaxGold then
              nValue := g_Config.nHumanMaxGold;
            Player.m_nGold := nValue;

            Player.GoldChanged();
          end;
        3:
          begin
            nValue := Int64(Player.m_nGameDiamond) + Prices;
            if nValue > High(LongWord) then
              nValue := High(LongWord);
            Player.m_nGameDiamond := nValue;

            Player.NewGamePointChanged;
          end;
        4:
          begin
            nValue := Int64(Player.m_nGameGird) + Prices;
            if nValue > High(LongWord) then
              nValue := High(LongWord);
            Player.m_nGameGird := nValue;

            Player.NewGamePointChanged;
          end;
      end;
    end
    else
    begin
      case nCurrencyType of
        0:
          GoldType := cgtGameGold;
        1:
          GoldType := cgtGamePoint;
        2:
          GoldType := cgtGold;
        3:
          GoldType := cgtGameDiamond;
        4:
          GoldType := cgtGameGird;
      else
        Exit;
      end;

      DataEngine.HumanChangeGold(nil, nil, GoldType, PlayerName, PlayerName, Prices);
    end;

    case nCurrencyType of
      0:
        begin
          if g_boGameLogGameGold then
          begin
            AddGameDataLog(LOG_GameGoldChange, LOG_ActionNone, latHuman, '0', 0, 0, g_Config.sGameGoldName, MakeIndex, PlayerName,
              '拍卖行-到期', 0, Prices, LogAdd);
          end;
        end;
      1: // 排除
        begin
          if g_boGameLogGameGold then
          begin
            AddGameDataLog(LOG_GamePointChange, LOG_ActionNone, latHuman, '0', 0, 0, g_Config.sGamePointName, MakeIndex,
              PlayerName, '拍卖行-到期', 0, Prices, LogAdd);
          end;
        end;
      2:
        begin
          if g_boGameLogGold then
          begin
            AddGameDataLog(LOG_GoldChange, LOG_ActionNone, latHuman, '0', 0, 0, sSTRING_GOLDNAME, MakeIndex, PlayerName,
              '拍卖行-到期', 0, Prices, LogAdd);
          end;
        end;
      3:
        begin
          if g_boGameLogGameGold then
          begin
            AddGameDataLog(LOG_GameDiamondChange, LOG_ActionNone, latHuman, '0', 0, 0, g_Config.sGameDiamondName, MakeIndex,
              PlayerName, '拍卖行-到期', 0, Prices, LogAdd);
          end;
        end;
      4:
        begin
          if g_boGameLogGameGold then
          begin
            AddGameDataLog(LOG_GameGirdChange, LOG_ActionNone, latHuman, '0', 0, 0, g_Config.sGameGirdName, MakeIndex, PlayerName,
              '拍卖行-到期', 0, Prices, LogAdd);
          end;
        end;
    end;
  end;

var
  HumanName, LastBidder: string;
  AuctionID, CurrencyType, StartingPrice, SellingPrice, LastBidPrice, DBIndex, MakeIndex: Integer;
  nAuctionTaxRate: Integer;
  Player: TPlayObject;
  StdItem: PTStdItem;
  ItemName: string;
begin
  try
    try
      FStatementUpdateAuctionItemFail.Reset;
      FStatementUpdateAuctionItemFail.Step;
    finally
      FStatementUpdateAuctionItemFail.Reset;
    end;

    try
      FStatementQueryAuctionItemSuccess.Reset;

      if FStatementQueryAuctionItemSuccess.Query then
      begin
        while (FStatementQueryAuctionItemSuccess.Fetch) do
        begin
          AuctionID := FStatementQueryAuctionItemSuccess.OrderGetColumnValueInt;
          HumanName := FStatementQueryAuctionItemSuccess.OrderGetColumnValueText;
          CurrencyType := FStatementQueryAuctionItemSuccess.OrderGetColumnValueInt;
          StartingPrice := FStatementQueryAuctionItemSuccess.OrderGetColumnValueInt;
          SellingPrice := FStatementQueryAuctionItemSuccess.OrderGetColumnValueInt;
          LastBidder := FStatementQueryAuctionItemSuccess.OrderGetColumnValueText;
          LastBidPrice := FStatementQueryAuctionItemSuccess.OrderGetColumnValueInt;
          DBIndex := FStatementQueryAuctionItemSuccess.OrderGetColumnValueInt;
          MakeIndex := FStatementQueryAuctionItemSuccess.OrderGetColumnValueInt;

          nAuctionTaxRate := 0;

          case CurrencyType of
            0:
              nAuctionTaxRate := g_Config.dwAuctionGameGoldTaxRate; // 元宝
            1:
              nAuctionTaxRate := g_Config.dwAuctionGamePointTaxRate; // 游戏点
            2:
              nAuctionTaxRate := g_Config.dwAuctionGoldTaxRate; // 金币
            3:
              nAuctionTaxRate := g_Config.dwAuctionGameDiamondTaxRate; // 金刚石
            4:
              nAuctionTaxRate := g_Config.dwAuctionGameGirdTaxRate; // 灵符
          end;

          StdItem := UserEngine.GetStdItem(DBIndex);

          if StdItem <> nil then
            ItemName := StdItem.Name
          else
            ItemName := '';

          // 加拍卖者的钱
          IncPlayerGameMoney(HumanName, MakeIndex, CurrencyType, LastBidPrice - Round((LastBidPrice / 100 * nAuctionTaxRate)),
            '卖出物品: ' + ItemName);
          try
            if (StdItem <> nil) and (StdItem.NeedIdentify = 1) then
            begin
              AddGameDataLog(LOG_ItemSell, LOG_ActionNone, latHuman, '0', 0, 0, StdItem.Name, MakeIndex, HumanName, '拍卖行-到期', 0, 0,
                '买入:' + LastBidder);
              AddGameDataLog(LOG_ItemBuy, LOG_ActionNone, latHuman, '0', 0, 0, StdItem.Name, MakeIndex, LastBidder, '拍卖行-到期', 0, 0,
                '待取回, 卖出:' + HumanName);
            end;

            Player := UserEngine.GetPlayObject(HumanName);
            if (Player <> nil) then
            begin
              Player.m_nScriptGotoCount := 0;

              if StdItem <> nil then
                Player.m_sAuctionItemName := StdItem.Name
              else
                Player.m_sAuctionItemName := '';

              Player.m_sAuctionItemHumanName := HumanName; // 物品拍卖者
              Player.m_sAuctionItemBidHumanName := LastBidder; // 竞拍出价者

              Player.m_nAuctionItemStartPrice := StartingPrice; // 底价
              Player.m_nAuctionItemSellPrice := SellingPrice; // 一口价
              Player.m_nAuctionItemFinaPrice := LastBidPrice; // 成交价
              Player.m_nAuctionItemInvalidPrice := 0; // 失效价
              Player.m_nAuctionItemMoneyType := CurrencyType; // 货币类型
              Player.m_boAuctionItemSelled := True; // 物品是否被秒杀/出售

              g_FunctionNPC.GotoLable(Player, '@AuctionSellItem', False);

              Player.m_sAuctionItemName := '';
              Player.m_sAuctionItemHumanName := ''; // 物品拍卖者
              Player.m_sAuctionItemBidHumanName := ''; // 竞拍出价者
              Player.m_nAuctionItemStartPrice := 0; // 底价
              Player.m_nAuctionItemSellPrice := 0; // 一口价
              Player.m_nAuctionItemInvalidPrice := 0; // 失效价
              Player.m_nAuctionItemFinaPrice := 0; // 成交价
              Player.m_nAuctionItemMoneyType := 0; // 货币类型
              Player.m_boAuctionItemSelled := False; // 物品是否被秒杀/出售
            end;

            Player := UserEngine.GetPlayObject(LastBidder);
            if (Player <> nil) then
            begin
              Player.m_nScriptGotoCount := 0;

              if StdItem <> nil then
                Player.m_sAuctionItemName := StdItem.Name
              else
                Player.m_sAuctionItemName := '';

              Player.m_sAuctionItemHumanName := HumanName; // 物品拍卖者
              Player.m_sAuctionItemBidHumanName := LastBidder; // 竞拍出价者

              Player.m_nAuctionItemStartPrice := StartingPrice; // 底价
              Player.m_nAuctionItemSellPrice := SellingPrice; // 一口价
              Player.m_nAuctionItemInvalidPrice := 0; // 失效价
              Player.m_nAuctionItemFinaPrice := LastBidPrice; // 成交价
              Player.m_nAuctionItemMoneyType := CurrencyType; // 货币类型
              Player.m_boAuctionItemSelled := True; // 物品是否被秒杀/出售

              g_FunctionNPC.GotoLable(Player, '@AuctionBuyItem', False);

              Player.m_sAuctionItemName := '';
              Player.m_sAuctionItemHumanName := ''; // 物品拍卖者
              Player.m_sAuctionItemBidHumanName := ''; // 竞拍出价者
              Player.m_nAuctionItemStartPrice := 0; // 底价
              Player.m_nAuctionItemSellPrice := 0; // 一口价
              Player.m_nAuctionItemInvalidPrice := 0; // 失效价
              Player.m_nAuctionItemFinaPrice := 0; // 成交价
              Player.m_nAuctionItemMoneyType := 0; // 货币类型
              Player.m_boAuctionItemSelled := False; // 物品是否被秒杀/出售
            end;
          except
          end;

          FStatementUpdateAuctionItemSuccess.Reset;
          FStatementUpdateAuctionItemSuccess.OrderBindParamInt(AuctionID);
          FStatementUpdateAuctionItemSuccess.Step;
        end;
      end;
    finally
      FStatementQueryAuctionItemSuccess.Reset;
      FStatementUpdateAuctionItemSuccess.Reset;
    end;
  except
    on E: Exception do
    begin
      MainOutMessage(E.Message);
    end;
  end;
end;

function TMySqlAuctionDB.DoHumanRename(OldName, NewName: string): Boolean;
begin
  Result := False;
  FStatementAuctionDataHumanRename.Reset;
  FStatementAuctionDataLastBidderRename.Reset;
  FStatementAuctionAttentionRename.Reset;

  try
    FDB.StartTransaction;
    try
      FStatementAuctionDataHumanRename.OrderBindParamText(NewName);
      FStatementAuctionDataHumanRename.OrderBindParamText(OldName);
      FStatementAuctionDataHumanRename.Step;

      FStatementAuctionDataLastBidderRename.OrderBindParamText(NewName);
      FStatementAuctionDataLastBidderRename.OrderBindParamText(OldName);
      FStatementAuctionDataLastBidderRename.Step;

      FStatementAuctionAttentionRename.OrderBindParamText(NewName);
      FStatementAuctionAttentionRename.OrderBindParamText(OldName);
      FStatementAuctionAttentionRename.Step;

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
    FStatementAuctionDataHumanRename.Reset;
    FStatementAuctionDataLastBidderRename.Reset;
    FStatementAuctionAttentionRename.Reset;
  end;
end;

end.

