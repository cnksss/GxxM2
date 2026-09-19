unit M2DataCommon;

interface

uses
  Windows, Classes, SysUtils, Grobal2;

const
  STORAGEEX_ITEM_TYPE = 0;
  USERSHOP_ITEM_TYPE = 1;
  GOLDDEAL_ITEM_TYPE = 2;
  AUCTION_ITEM_TYPE = 5;

type
  TM2DataDB = class;

  TStorageDB = class(TObject)
  private
    FOwner: TM2DataDB;
  protected
    procedure DoInit; virtual; abstract;
    procedure DoFinal; virtual; abstract;

    procedure DoLoadStorageItems(sHumanName: string; List: TList; var StorageID: Integer); virtual; abstract;
    procedure DoSaveStorageItems(sHumanName: string; List: TList; StorageID: Integer); virtual; abstract;

    function DoDeleteStorageItem(StorageID: Integer; sHumanName: string; nMakeIndex, nDBIndex: Integer): Boolean;
      virtual; abstract;
    function DoAddStorageItem(StorageID: Integer; sHumanName: string; UserItem: PTUserItem): Boolean; virtual; abstract;
    function DoClearStorageItem(StorageID: Integer; sHumanName: string): Boolean; virtual; abstract;

    procedure DoRenameHumanName(sOldName, sNewName: string); virtual; abstract;

    procedure DoGetAllHumans(var SL: TStringList); virtual; abstract;
  public
    constructor Create(AOwner: TM2DataDB); virtual;

    property Owner: TM2DataDB read FOwner;

    procedure LoadStorageItems(sHumanName: string; List: TList; var StorageID: Integer);
    procedure SaveStorageItems(sHumanName: string; List: TList; StorageID: Integer);

    function DeleteStorageItem(StorageID: Integer; sHumanName: string; nMakeIndex, nDBIndex: Integer): Boolean;
    function AddStorageItem(StorageID: Integer; sHumanName: string; UserItem: PTUserItem): Boolean;
    function ClearStorageItem(StorageID: Integer; sHumanName: string): Boolean;

    procedure RenameHumanName(sOldName, sNewName: string);

    procedure GetAllHumans(var SL: TStringList);
  end;

  // --------------------------------------------------------------------------------------

  TShopItemType = (sitSelling, sitSelled, sitStorage, sitSellingAndStorage);

  TUserShop = packed record // size =  116
    ShopID: Integer;
    boBusiness: Boolean; // 是否营业
    sShopName: string[ACTOR_NAME_LEN]; // 店铺名称 14
    sMasterName: string[ACTOR_NAME_LEN]; // 店主名称 14
    dCreateDate: TDateTime; // 创建时间 8
    nCareValue: Integer; // 关注度   4

    SellItemCount: Integer;
    SelledItemCount: Integer;
    StorageItemCount: Integer;
  end;

  pTUserShop = ^TUserShop;

  TUserShopItem = packed record
    ShopID: Integer;
    ItemID: Integer;
    btAllowSell: Byte; // 允许出售 (0:仓库物品; 1:出售物品; 2:出售超期不能放入仓库物品)
    boGetMoney: Boolean; // 是否已取款
    btItemType: Byte; // 物品类型
    btMoneyType: Byte; // 货币类型
    nPrice: Integer; // 出售价格
    dCreateDate: TDateTime; // 创建时间 8
    UserItem: TUserItem;
    sBuyName: string[ACTOR_NAME_LEN]; // 购买人名称   已经出售
    sShopName: string[ACTOR_NAME_LEN]; // 店铺名称 14
    sMasterName: string[ACTOR_NAME_LEN]; // 店主名称 14
  end;

  pTUserShopItem = ^TUserShopItem;

  TSimpleUserShopItem = packed record
    ShopID: Integer;
    ItemID: Integer;
    btAllowSell: Byte; // 允许出售 (0:仓库物品; 1:出售物品; 2:出售超期不能放入仓库物品)
    boGetMoney: Boolean; // 是否已取款
    btItemType: Byte; // 物品类型
    btMoneyType: Byte; // 货币类型
    nPrice: Integer; // 出售价格
    dCreateDate: TDateTime; // 创建时间 8
    sBuyName: string[ACTOR_NAME_LEN]; // 购买人名称   已经出售
    sShopName: string[ACTOR_NAME_LEN]; // 店铺名称 14
    sMasterName: string[ACTOR_NAME_LEN]; // 店主名称 14

    MakeIndex: Integer;
    wIndex: Word; // 物品id
    boIsBind: Boolean; // 是否绑定
    btBindOption: Byte;
    // 绑定选项对应Bit位 TUserItemBindValueType
    // 1: 禁止扔
    // 2: 禁止交易
    // 3: 禁止存
    // 4: 禁止修
    // 5: 禁止出售
    // 6: 禁止爆出
    // 7: 丢弃消失
    Dura: Word;
  end;

  pTSimpleUserShopItem = ^TSimpleUserShopItem;

  TSelledAndNoGetMoneyTotal = packed record
    sMasterName: string[ACTOR_NAME_LEN]; // 店主名称 14
    btMoneyType: Byte; // 货币类型
    nSumPrice: Int64; // 出售价格
  end;

  PTSelledAndNoGetMoneyTotal = ^TSelledAndNoGetMoneyTotal;

  TUserShopList = class(TObject)
  private
    FList: TList;
    function GetCount: Integer;
    function GetItems(Index: Integer): pTUserShop;
  public
    constructor Create;
    destructor Destroy; override;

    property Count: Integer read GetCount;
    property Items[Index: Integer]: pTUserShop read GetItems; default;
    function Add(UserShop: pTUserShop): pTUserShop;
    procedure Clear;
    procedure SetCapacity(Value: Integer);
  end;

  TUserShopItemList = class(TObject)
  private
    FList: TList;
    function GetCount: Integer;
    function GetItems(Index: Integer): pTUserShopItem;
  public
    constructor Create;
    destructor Destroy; override;

    property Count: Integer read GetCount;
    property Items[Index: Integer]: pTUserShopItem read GetItems; default;
    function Add(UserShopItem: pTUserShopItem): pTUserShopItem;
    procedure Clear;
    procedure SetCapacity(Value: Integer);
    procedure Delete(Index: Integer);
  end;

  TUserShopDB = class
  private
    FOwner: TM2DataDB;
    FRunTick: LongWord;
  protected
    procedure DoInit; virtual; abstract;
    procedure DoFinal; virtual; abstract;

    function DoGetAllShop(StartIndex: Integer; Keyword: string; IsKeywordHumanName: Boolean; SortType: Integer;
      ShopList: TUserShopList): Integer; virtual; abstract;
    function DoGetAllShopCount(Keyword: string; IsKeywordHumanName: Boolean): Integer; virtual; abstract;

    function DoGetAllShopEx(ShopList: TUserShopList): Integer; virtual; abstract;

    function DoGetSellItems(IsMyShop: Boolean; StartIndex: Integer; HumanName, Keyword: string;
      ItemType, MoneyType, MinPrice, MaxPrice, SortType: Integer; ItemList: TUserShopItemList;
      ShopItemType: TShopItemType = sitSelling): Integer; virtual; abstract;
    function DoGetSellItemsCount(IsMyShop: Boolean; HumanName, Keyword: string; ItemType, MoneyType, MinPrice, MaxPrice: Integer;
      ShopItemType: TShopItemType = sitSelling): Integer; virtual; abstract;

    // function DoGetHumanItems(IsMyShop: Boolean; ItemList: TUserShopItemList; HumanName: string; ShopItemType: TShopItemType): Integer; virtual; abstract;

    function DoGetHumanItemWithMakeIndex(IsMyShop: Boolean; HumanName: string; ShopItemType: TShopItemType;
      ItemMakeIndex: Integer; var UserShopItem: TSimpleUserShopItem): Boolean; virtual; abstract;

    function DoGetSelledAndNoGetMoneyTotal(ItemList: TList): Integer; virtual; abstract;

    function DoGetUserShopInfo(HumanName: string; var UserShop: TUserShop): Boolean; virtual; abstract;

    procedure DoIncUserShopCareValue(HumanName: string); virtual; abstract;

    function DoHumanNameExists(HumanName: string): Boolean; virtual; abstract;
    function DoShopNameExists(ShopName: string): Boolean; virtual; abstract;

    function DoShopAdd(ShopName, HumanName: string): Boolean; virtual; abstract;
    function DoShopDelete(ShopID: Integer): Boolean; virtual; abstract;
    function DoShopRename(ShopID: Integer; NewShopName: string): Boolean; virtual; abstract;

    function DoHumanRename(OldName, NewName: string): Boolean; virtual; abstract;

    function DoUpdateHumanBusiness(HumanName: string; IsBusiness: Boolean): Boolean; virtual; abstract;

    function DoAddItem(ShopID: Integer; ShopItem: pTUserShopItem; sItemName: string): Boolean; virtual; abstract;
    function DoUpdateItem(ShopID, ItemID: Integer; btItemType, btAllowSell, btMoneyType, nPrice: Integer): Boolean;
      virtual; abstract;

    function DoBuyItem(ShopID, ItemID: Integer; Buyer: string): Boolean; virtual; abstract;
    function DoGetMoneyItem(ShopID, ItemID: Integer): Boolean; virtual; abstract;
    function DoDeleteItem(ShopID, ItemID: Integer): Boolean; virtual; abstract;

    procedure DoRun; virtual; abstract;
  public
    constructor Create(AOwner: TM2DataDB); virtual;

    property Owner: TM2DataDB read FOwner;
    property RunTick2: LongWord read FRunTick;

    function GetAllShop(StartIndex: Integer; Keyword: string; IsKeywordHumanName: Boolean; SortType: Integer;
      ShopList: TUserShopList): Integer;
    function GetAllShopCount(Keyword: string; IsKeywordHumanName: Boolean): Integer;

    function GetAllShopEx(ShopList: TUserShopList): Integer;

    function GetSellItems(IsMyShop: Boolean; StartIndex: Integer; HumanName, Keyword: string;
      ItemType, MoneyType, MinPrice, MaxPrice, SortType: Integer; ItemList: TUserShopItemList;
      ShopItemType: TShopItemType = sitSelling): Integer;
    function GetSellItemsCount(IsMyShop: Boolean; HumanName, Keyword: string; ItemType, MoneyType, MinPrice, MaxPrice: Integer;
      ShopItemType: TShopItemType = sitSelling): Integer;

    // function GetHumanItems(IsMyShop: Boolean; ItemList: TUserShopItemList; HumanName: string; ShopItemType: TShopItemType): Integer;

    function GetHumanItemWithMakeIndex(IsMyShop: Boolean; HumanName: string; ShopItemType: TShopItemType; ItemMakeIndex: Integer;
      var UserShopItem: TSimpleUserShopItem): Boolean;

    function GetSelledAndNoGetMoneyTotal(ItemList: TList): Integer;

    function GetUserShopInfo(HumanName: string; var UserShop: TUserShop): Boolean;

    procedure IncUserShopCareValue(HumanName: string);

    function HumanNameExists(HumanName: string): Boolean;
    function ShopNameExists(ShopName: string): Boolean;

    function ShopAdd(ShopName, HumanName: string): Boolean;
    function ShopDelete(ShopID: Integer): Boolean;
    function ShopRename(ShopID: Integer; NewShopName: string): Boolean;

    function HumanRename(OldName, NewName: string): Boolean;

    function UpdateHumanBusiness(HumanName: string; IsBusiness: Boolean): Boolean;

    function AddItem(ShopID: Integer; ShopItem: pTUserShopItem; sItemName: string): Boolean;
    function UpdateItem(ShopID, ItemID: Integer; btItemType, btAllowSell, btMoneyType, nPrice: Integer): Boolean;
    function BuyItem(ShopID, ItemID: Integer; Buyer: string): Boolean;
    function GetMoneyItem(ShopID, ItemID: Integer): Boolean;
    function DeleteItem(ShopID, ItemID: Integer): Boolean;

    procedure Run;
  end;

  // 拍卖物品数据记录
  PAuctionRecord = ^TAuctionRecord;

  TAuctionRecord = record
    AuctionID: Integer;
    HumanName: string; // 拍卖人
    AddDateTime: TDateTime; // 开始时间
    AuctionTime: Integer; // 拍卖时间
    TimeLeft: Integer; // 剩余时间
    StartingPrice: LongWord; // 底价
    SellingPrice: LongWord; // 一口价
    CurrencyType: Integer; // 货币类型
    LastBidPrice: Integer; // 最后价格
    LastBidder: string; // 最后出价人
    // LastBidTime: TDateTime;                 // 最后出价时间
    TradingStatus: Integer; // 交易状态
    IsItemGive: Boolean; // 物品是否交接
    IsAttention: Boolean; // 是否被关注
    ActionItem: TUserItem; // 拍卖物品
  end;

  PAuctionInfo = ^TAuctionInfo;

  TAuctionInfo = record
    AuctionID: Integer;
    HumanName: string; // 拍卖人
    AddDateTime: TDateTime; // 开始时间
    AuctionTime: Integer; // 拍卖时间
    TimeLeft: Integer; // 剩余时间
    StartingPrice: LongWord; // 底价
    SellingPrice: LongWord; // 一口价
    CurrencyType: Integer; // 货币类型
    LastBidPrice: Integer; // 最后价格
    LastBidder: string; // 最后出价人
    // LastBidTime: TDateTime;                 // 最后出价时间
    TradingStatus: Integer; // 交易状态
    IsItemGive: Boolean; // 物品是否交接
  end;

  // 拍卖物品列表
  TAuctionItemList = class(TObject)
  private
    FList: TList;
    function GetCount: Integer;
    function GetItems(Index: Integer): PAuctionRecord;
  public
    constructor Create;
    destructor Destroy; override;

    property Count: Integer read GetCount;
    property Items[Index: Integer]: PAuctionRecord read GetItems; default;
    function Add(AuctionRecord: PAuctionRecord): PAuctionRecord;
    function Insert(AuctionRecord: PAuctionRecord): PAuctionRecord;
    procedure Clear;
    procedure SetCapacity(Value: Integer);
  end;

  // 拍卖行数据
  TAuctionDB = class(TObject)
  private
    FOwner: TM2DataDB;
    FRunTick: LongWord;
  protected
    procedure DoInit; virtual; abstract;
    procedure DoFinal; virtual; abstract;

    // 查询商品列表
    function DoQueryAllItems(ItemName: string; ItemGroup: TItemGroup; HumanName: string; nPage, TopmostAuctionID: Integer;
      ItemColors: Integer; SortField: Integer; SortASC: Boolean; MoneyType: Integer; MinPrices, MaxPrices: LongWord;
      ItemList: TAuctionItemList): Integer; virtual; abstract;

    // 查询我的拍卖物品
    function DoQueryMyItems(HumanName: string; nPage: Integer; ItemList: TAuctionItemList): Integer; virtual; abstract;

    // 查询关注物品
    function DoQueryMyAttentionItems(HumanName: string; nPage: Integer; ItemList: TAuctionItemList): Integer; virtual; abstract;

    // 得到商品列表页数
    function DoGetAllItemsPageCount(ItemName: string; ItemGroup: TItemGroup; ItemColors: Integer; MoneyType: Integer;
      MinPrices, MaxPrices: LongWord): Integer; virtual; abstract;

    // 得到我的拍卖物品页数
    function DoGetMyItemsPageCount(HumanName: string): Integer; virtual; abstract;

    // 得到我的关注物品页数
    function DoGetMyAttentionPageCount(HumanName: string): Integer; virtual; abstract;

    // 查询正在拍卖的物品数量
    function DoGetMyAuctioningItemsCount(HumanName: string): Integer; virtual; abstract;

    // 我流拍未取的物品数量
    function DoGetMySellFailItemsCount(HumanName: string): Integer; virtual; abstract;

    // 我拍买到未取的物品数量
    function DoGetMyBuyOKItemsCount(HumanName: string): Integer; virtual; abstract;

    // 添加拍卖物品
    function DoAddAuctionItem(HumanName: string; AuctionTime: Integer; StartingPrice: Integer; SellingPrice: Integer;
      CurrencyType: Integer; UserItem: PTUserItem; StdItem: PTStdItem): Integer; virtual; abstract;

    // 取消拍卖物品
    function DoCancelAuctionItem(HumanName: string; AuctionID: Integer): Boolean; virtual; abstract;

    // 取回拍卖物品
    function DoRetrieveAuctionItem(AuctionID: Integer): Boolean; virtual; abstract;

    // 删除拍卖物品
    function DoDeleteAuctionItem(HumanName: string; AuctionID: Integer): Boolean; virtual; abstract;

    // 添加关注物品
    function DoAddAttentionItem(HumanName: string; Index: Integer): Boolean; virtual; abstract;

    // 删除关注物品
    function DoDeleteAttentionItem(HumanName: string; Index: Integer): Boolean; virtual; abstract;

    // 参加物品竞价
    function DoJoinItemBid(HumanName: string; Index: Integer; Prices: Integer; IsSell: Boolean { 一口价 } ): Boolean;
      virtual; abstract;

    // 获取竞拍物品信息
    function DoGetAuctionInfo(Index: Integer; var AuctionInfo: TAuctionInfo): Boolean; virtual; abstract;

    // 获取竞拍物品信息
    function DoGetAuctionRecord(Index: Integer; var AuctionRecord: TAuctionRecord): Boolean; virtual; abstract;

    function DoHumanRename(OldName, NewName: string): Boolean; virtual; abstract;

    procedure DoRun; virtual; abstract;
  public
    constructor Create(AOwner: TM2DataDB); virtual;

    property Owner: TM2DataDB read FOwner;
    property RunTick2: LongWord read FRunTick;

    // 查询商品列表
    function QueryAllItems(ItemName: string; ItemGroup: TItemGroup; HumanName: string; nPage, TopmostAuctionID: Integer;
      ItemColors: Integer; SortField: Integer; SortASC: Boolean; MoneyType: Integer; MinPrices, MaxPrices: LongWord;
      ItemList: TAuctionItemList): Integer;

    // 查询我的拍卖物品
    function QueryMyItems(HumanName: string; nPage: Integer; ItemList: TAuctionItemList): Integer;

    // 查询关注物品
    function QueryMyAttentionItems(HumanName: string; nPage: Integer; ItemList: TAuctionItemList): Integer;

    // 得到商品列表页数
    function GetAllItemsPageCount(ItemName: string; ItemGroup: TItemGroup; ItemColors: Integer; MoneyType: Integer;
      MinPrices, MaxPrices: LongWord): Integer;

    // 得到我的拍卖物品页数
    function GetMyItemsPageCount(HumanName: string): Integer;

    // 得到我的关注物品页数
    function GetMyAttentionPageCount(HumanName: string): Integer;

    // 查询正在拍卖的物品数量
    function GetMyAuctioningItemsCount(HumanName: string): Integer;

    // 我流拍未取的物品数量
    function GetMySellFailItemsCount(HumanName: string): Integer;

    // 我拍买到未取的物品数量
    function GetMyBuyOKItemsCount(HumanName: string): Integer;

    // 添加拍卖物品
    function AddAuctionItem(HumanName: string; AuctionTime: Integer; StartingPrice: Integer; SellingPrice: Integer;
      CurrencyType: Integer; UserItem: PTUserItem; StdItem: PTStdItem): Integer;

    // 取消拍卖物品
    function CancelAuctionItem(HumanName: string; AuctionID: Integer): Boolean;

    // 取回拍卖物品
    function RetrieveAuctionItem(AuctionID: Integer): Boolean;

    // 删除拍卖物品
    function DeleteAuctionItem(HumanName: string; AuctionID: Integer): Boolean;

    // 添加关注物品
    function AddAttentionItem(HumanName: string; Index: Integer): Boolean;

    // 删除关注物品
    function DeleteAttentionItem(HumanName: string; Index: Integer): Boolean;

    // 参加物品竞价
    function JoinItemBid(HumanName: string; Index: Integer; Prices: Integer; IsSell: Boolean { 一口价 } ): Boolean;

    // 获取竞拍物品信息
    function GetAuctionInfo(Index: Integer; var AuctionInfo: TAuctionInfo): Boolean;

    // 获取竞拍物品信息
    function GetAuctionRecord(Index: Integer; var AuctionRecord: TAuctionRecord): Boolean;

    function HumanRename(OldName, NewName: string): Boolean;

    procedure Run;
  end;

  TAuctionDBClass = class of TAuctionDB;
  TUserShopDBClass = class of TUserShopDB;
  TStorageDBClass = class of TStorageDB;

  TM2DataDB = class(TObject)
  private
    FIsInitOK: Boolean;

    FAuctionDB: TAuctionDB;
    FUserShopDB: TUserShopDB;
    FStorageDB: TStorageDB;

    FCS: TRTLCriticalSection;
  protected
    function GetAuctionDBClass: TAuctionDBClass; virtual; abstract;
    function GetUserShopDBClass: TUserShopDBClass; virtual; abstract;
    function GetStorageDBClass: TStorageDBClass; virtual; abstract;

    procedure DoInit; virtual; abstract;
    procedure DoFinal; virtual; abstract;

    procedure DoLoadItemsFromDB(ParentID, ItemType: Integer; IsSort: Boolean; List: TList); virtual; abstract;
    procedure DoLoadItemFromDB(UserItem: PTUserItem; ParentID, ItemType, ItemIndex: Integer); virtual; abstract;
    procedure DoSaveItemToDB(UserItem: PTUserItem; ParentID, ItemType, ItemIndex: Integer); virtual; abstract;

    function GetDataBase: TObject; virtual; abstract;
  public
    constructor Create; virtual;
    destructor Destroy; override;

    property DataBase: TObject read GetDataBase;
    property AuctionDB: TAuctionDB read FAuctionDB;
    property UserShopDB: TUserShopDB read FUserShopDB;
    property StorageDB: TStorageDB read FStorageDB;

    procedure Lock;
    procedure UnLock;

    procedure LoadItemsFromDB(ParentID, ItemType: Integer; IsSort: Boolean; List: TList);
    procedure LoadItemFromDB(UserItem: PTUserItem; ParentID, ItemType, ItemIndex: Integer);
    procedure SaveItemToDB(UserItem: PTUserItem; ParentID, ItemType, ItemIndex: Integer);

    procedure Init;
    procedure Final;

    property IsInitOK: Boolean read FIsInitOK;

    procedure Run; virtual;
  end;

implementation

uses
  M2Share;

{ TStorageEx }

constructor TStorageDB.Create(AOwner: TM2DataDB);
begin
  FOwner := AOwner;
end;

procedure TStorageDB.LoadStorageItems(sHumanName: string; List: TList; var StorageID: Integer);
begin
  FOwner.Lock;
  try
    try
      DoLoadStorageItems(sHumanName, List, StorageID);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TStorageDB:LoadStorageItems;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

procedure TStorageDB.SaveStorageItems(sHumanName: string; List: TList; StorageID: Integer);
begin
  FOwner.Lock;
  try
    try
      DoSaveStorageItems(sHumanName, List, StorageID);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TStorageDB:SaveStorageItems;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TStorageDB.DeleteStorageItem(StorageID: Integer; sHumanName: string; nMakeIndex, nDBIndex: Integer): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoDeleteStorageItem(StorageID, sHumanName, nMakeIndex, nDBIndex);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TStorageDB:DeleteStorageItem;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TStorageDB.AddStorageItem(StorageID: Integer; sHumanName: string; UserItem: PTUserItem): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoAddStorageItem(StorageID, sHumanName, UserItem);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TStorageDB:AddStorageItem;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TStorageDB.ClearStorageItem(StorageID: Integer; sHumanName: string): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoClearStorageItem(StorageID, sHumanName);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TStorageDB:ClearStorageItem;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

procedure TStorageDB.RenameHumanName(sOldName, sNewName: string);
begin
  FOwner.Lock;
  try
    try
      DoRenameHumanName(sOldName, sNewName);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TStorageDB:RenameHumanName;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

procedure TStorageDB.GetAllHumans(var SL: TStringList);
begin
  FOwner.Lock;
  try
    try
      DoGetAllHumans(SL);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TStorageDB:GetAllHumans;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

{ TUserShopList }

constructor TUserShopList.Create;
begin
  FList := TList.Create;
end;

destructor TUserShopList.Destroy;
begin
  Clear;
  FList.Free;
  inherited;
end;

function TUserShopList.Add(UserShop: pTUserShop): pTUserShop;
begin
  New(Result);
  Result^ := UserShop^;
  FList.Add(Result);
end;

procedure TUserShopList.Clear;
var
  I: Integer;
begin
  for I := 0 to FList.Count - 1 do
  begin
    Dispose(pTUserShop(FList.Items[I]));
  end;
  FList.Clear;
end;

function TUserShopList.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TUserShopList.GetItems(Index: Integer): pTUserShop;
begin
  if (Index >= 0) and (Index < FList.Count) then
    Result := FList.Items[Index]
  else
    Result := nil;
end;

procedure TUserShopList.SetCapacity(Value: Integer);
begin
  if FList.Capacity <> Value then
    FList.Capacity := Value;
end;

{ TUserShopItemList }

constructor TUserShopItemList.Create;
begin
  FList := TList.Create;
end;

destructor TUserShopItemList.Destroy;
begin
  Clear;
  FList.Free;
  inherited;
end;

function TUserShopItemList.Add(UserShopItem: pTUserShopItem): pTUserShopItem;
begin
  New(Result);
  Result^ := UserShopItem^;
  FList.Add(Result);
end;

procedure TUserShopItemList.Clear;
var
  I: Integer;
begin
  for I := 0 to FList.Count - 1 do
  begin
    Dispose(pTUserShopItem(FList.Items[I]));
  end;
  FList.Clear;
end;

function TUserShopItemList.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TUserShopItemList.GetItems(Index: Integer): pTUserShopItem;
begin
  if (Index >= 0) and (Index < FList.Count) then
    Result := FList.Items[Index]
  else
    Result := nil;
end;

procedure TUserShopItemList.SetCapacity(Value: Integer);
begin
  if FList.Capacity <> Value then
    FList.Capacity := Value;
end;

procedure TUserShopItemList.Delete(Index: Integer);
begin
  if (Index >= 0) and (Index < FList.Count) then
  begin
    Dispose(pTUserShopItem(FList.Items[Index]));
    FList.Delete(Index);
  end;
end;

{ TUserShopDB }

constructor TUserShopDB.Create(AOwner: TM2DataDB);
begin
  FRunTick := MyGetTickCount;
  FOwner := AOwner;
end;

function TUserShopDB.GetAllShop(StartIndex: Integer; Keyword: string; IsKeywordHumanName: Boolean; SortType: Integer;
  ShopList: TUserShopList): Integer;
begin
  FOwner.Lock;
  try
    Result := 0;
    try
      Result := DoGetAllShop(StartIndex, Keyword, IsKeywordHumanName, SortType, ShopList);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TUserShopDB:GetAllShop;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TUserShopDB.GetAllShopCount(Keyword: string; IsKeywordHumanName: Boolean): Integer;
begin
  FOwner.Lock;
  try
    Result := 0;
    try
      Result := DoGetAllShopCount(Keyword, IsKeywordHumanName);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TUserShopDB:GetAllShopCount;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TUserShopDB.GetAllShopEx(ShopList: TUserShopList): Integer;
begin
  FOwner.Lock;
  try
    Result := 0;
    try
      Result := DoGetAllShopEx(ShopList);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TUserShopDB:GetAllShopEx;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TUserShopDB.GetSellItems(IsMyShop: Boolean; StartIndex: Integer; HumanName, Keyword: string;
  ItemType, MoneyType, MinPrice, MaxPrice, SortType: Integer; ItemList: TUserShopItemList; ShopItemType: TShopItemType): Integer;
begin
  FOwner.Lock;
  try
    Result := 0;
    try
      Result := DoGetSellItems(IsMyShop, StartIndex, HumanName, Keyword, ItemType, MoneyType, MinPrice, MaxPrice, SortType,
        ItemList, ShopItemType);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TUserShopDB:GetSellItems;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TUserShopDB.GetSellItemsCount(IsMyShop: Boolean; HumanName, Keyword: string;
  ItemType, MoneyType, MinPrice, MaxPrice: Integer; ShopItemType: TShopItemType): Integer;
begin
  FOwner.Lock;
  try
    Result := 0;
    try
      Result := DoGetSellItemsCount(IsMyShop, HumanName, Keyword, ItemType, MoneyType, MinPrice, MaxPrice, ShopItemType);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TUserShopDB:GetSellItemsCount;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

{
  function TUserShopDB.GetHumanItems(IsMyShop: Boolean;
  ItemList: TUserShopItemList; HumanName: string;
  ShopItemType: TShopItemType): Integer;
  begin
  try
  Result := DoGetHumanItems(IsMyShop, ItemList, HumanName, ShopItemType);
  except
  on E: Exception do
  begin
  MainOutMessage('[Exception] TUserShopDB:GetHumanItems;' + E.Message);
  end;
  end;
  end;
}

function TUserShopDB.GetHumanItemWithMakeIndex(IsMyShop: Boolean; HumanName: string; ShopItemType: TShopItemType;
  ItemMakeIndex: Integer; var UserShopItem: TSimpleUserShopItem): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoGetHumanItemWithMakeIndex(IsMyShop, HumanName, ShopItemType, ItemMakeIndex, UserShopItem);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TUserShopDB:GetMyItemWithMakeIndex;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TUserShopDB.GetSelledAndNoGetMoneyTotal(ItemList: TList): Integer;
begin
  FOwner.Lock;
  try
    Result := 0;
    try
      Result := DoGetSelledAndNoGetMoneyTotal(ItemList);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TUserShopDB:GetSelledAndNoGetMoneyTotal;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TUserShopDB.GetUserShopInfo(HumanName: string; var UserShop: TUserShop): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoGetUserShopInfo(HumanName, UserShop);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TUserShopDB:GetUserShopInfo;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

procedure TUserShopDB.IncUserShopCareValue(HumanName: string);
begin
  FOwner.Lock;
  try
    try
      DoIncUserShopCareValue(HumanName);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TUserShopDB:IncUserShopCareValue;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TUserShopDB.HumanNameExists(HumanName: string): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoHumanNameExists(HumanName);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TUserShopDB:HumanNameExists;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TUserShopDB.ShopNameExists(ShopName: string): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoShopNameExists(ShopName);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TUserShopDB:ShopNameExists;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TUserShopDB.ShopAdd(ShopName, HumanName: string): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoShopAdd(ShopName, HumanName);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TUserShopDB:ShopAdd;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TUserShopDB.ShopDelete(ShopID: Integer): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoShopDelete(ShopID);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TUserShopDB:ShopDelete;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TUserShopDB.ShopRename(ShopID: Integer; NewShopName: string): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoShopRename(ShopID, NewShopName);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TUserShopDB:ShopRename;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TUserShopDB.HumanRename(OldName, NewName: string): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoHumanRename(OldName, NewName);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TUserShopDB:HumanRename;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TUserShopDB.UpdateHumanBusiness(HumanName: string; IsBusiness: Boolean): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoUpdateHumanBusiness(HumanName, IsBusiness);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TUserShopDB:UpdateHumanBusiness;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TUserShopDB.AddItem(ShopID: Integer; ShopItem: pTUserShopItem; sItemName: string): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoAddItem(ShopID, ShopItem, sItemName);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TUserShopDB:AddItem;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TUserShopDB.UpdateItem(ShopID, ItemID: Integer; btItemType, btAllowSell, btMoneyType, nPrice: Integer): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoUpdateItem(ShopID, ItemID, btItemType, btAllowSell, btMoneyType, nPrice);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TUserShopDB:UpdateItem;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TUserShopDB.GetMoneyItem(ShopID, ItemID: Integer): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoGetMoneyItem(ShopID, ItemID);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TUserShopDB:GetMoneyItem;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TUserShopDB.BuyItem(ShopID, ItemID: Integer; Buyer: string): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoBuyItem(ShopID, ItemID, Buyer);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TUserShopDB:BuyItem;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TUserShopDB.DeleteItem(ShopID, ItemID: Integer): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoDeleteItem(ShopID, ItemID);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TUserShopDB:DeleteItem;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

procedure TUserShopDB.Run;
begin
  try
    DoRun();
    FRunTick := MyGetTickCount;
  except
    on E: Exception do
    begin
      MainOutMessage('[Exception] TUserShopDB:Run;' + E.Message);
    end;
  end;
end;

{ TAuctionItemList }

constructor TAuctionItemList.Create;
begin
  FList := TList.Create;
end;

destructor TAuctionItemList.Destroy;
begin
  Clear;
  FList.Free;
  inherited;
end;

function TAuctionItemList.Add(AuctionRecord: PAuctionRecord): PAuctionRecord;
begin
  New(Result);
  Result^ := AuctionRecord^;
  FList.Add(Result);
end;

function TAuctionItemList.Insert(AuctionRecord: PAuctionRecord): PAuctionRecord;
begin
  New(Result);
  Result^ := AuctionRecord^;
  FList.Insert(0, Result);
end;

procedure TAuctionItemList.Clear;
var
  I: Integer;
begin
  for I := 0 to FList.Count - 1 do
  begin
    Dispose(PAuctionRecord(FList.Items[I]));
  end;
  FList.Clear;
end;

function TAuctionItemList.GetCount: Integer;
begin
  Result := FList.Count;
end;

function TAuctionItemList.GetItems(Index: Integer): PAuctionRecord;
begin
  if (Index >= 0) and (Index < FList.Count) then
    Result := FList.Items[Index]
  else
    Result := nil;
end;

procedure TAuctionItemList.SetCapacity(Value: Integer);
begin
  if FList.Capacity <> Value then
    FList.Capacity := Value;
end;

{ TAuctionDB }

constructor TAuctionDB.Create(AOwner: TM2DataDB);
begin
  FRunTick := MyGetTickCount;
  FOwner := AOwner;
end;

function TAuctionDB.QueryAllItems(ItemName: string; ItemGroup: TItemGroup; HumanName: string;
  nPage, TopmostAuctionID, ItemColors, SortField: Integer; SortASC: Boolean; MoneyType: Integer; MinPrices, MaxPrices: LongWord;
  ItemList: TAuctionItemList): Integer;
var
  TempPrices: LongWord;
begin
  FOwner.Lock;
  try
    Result := 0;
    try
      if (MinPrices <> 0) and (MaxPrices <> 0) and (MinPrices > MaxPrices) then
      begin
        TempPrices := MinPrices;
        MinPrices := MaxPrices;
        MaxPrices := TempPrices;
      end;

      Result := DoQueryAllItems(ItemName, ItemGroup, HumanName, nPage, TopmostAuctionID, ItemColors, SortField, SortASC,
        MoneyType, MinPrices, MaxPrices, ItemList);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TAuctionDB:QueryAllItems;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TAuctionDB.QueryMyItems(HumanName: string; nPage: Integer; ItemList: TAuctionItemList): Integer;
begin
  FOwner.Lock;
  try
    Result := 0;
    try
      Result := DoQueryMyItems(HumanName, nPage, ItemList);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TAuctionDB:QueryMyItems;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TAuctionDB.QueryMyAttentionItems(HumanName: string; nPage: Integer; ItemList: TAuctionItemList): Integer;
begin
  FOwner.Lock;
  try
    Result := 0;
    try
      Result := DoQueryMyAttentionItems(HumanName, nPage, ItemList);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TAuctionDB:QueryMyAttentionItems;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TAuctionDB.GetAllItemsPageCount(ItemName: string; ItemGroup: TItemGroup; ItemColors: Integer; MoneyType: Integer;
  MinPrices, MaxPrices: LongWord): Integer;
var
  TempPrices: LongWord;
begin
  FOwner.Lock;
  try
    Result := 0;
    try
      if (MinPrices <> 0) and (MaxPrices <> 0) and (MinPrices > MaxPrices) then
      begin
        TempPrices := MinPrices;
        MinPrices := MaxPrices;
        MaxPrices := TempPrices;
      end;

      Result := DoGetAllItemsPageCount(ItemName, ItemGroup, ItemColors, MoneyType, MinPrices, MaxPrices);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TAuctionDB:GetAllItemsPageCount;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TAuctionDB.GetMyItemsPageCount(HumanName: string): Integer;
begin
  FOwner.Lock;
  try
    Result := 0;
    try
      Result := DoGetMyItemsPageCount(HumanName);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TAuctionDB:GetMyItemsPageCount;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TAuctionDB.GetMyAttentionPageCount(HumanName: string): Integer;
begin
  FOwner.Lock;
  try
    Result := 0;
    try
      Result := DoGetMyAttentionPageCount(HumanName);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TAuctionDB:GetMyAttentionPageCount;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TAuctionDB.GetMyAuctioningItemsCount(HumanName: string): Integer;
begin
  FOwner.Lock;
  try
    Result := 0;
    try
      Result := DoGetMyAuctioningItemsCount(HumanName);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TAuctionDB:GetMyAuctioningItemsCount;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TAuctionDB.GetMySellFailItemsCount(HumanName: string): Integer;
begin
  FOwner.Lock;
  try
    Result := 0;
    try
      Result := DoGetMySellFailItemsCount(HumanName);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TAuctionDB:GetMySellFailItemsCount;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

// 我拍买到未取的物品数量
function TAuctionDB.GetMyBuyOKItemsCount(HumanName: string): Integer;
begin
  FOwner.Lock;
  try
    Result := 0;
    try
      Result := DoGetMyBuyOKItemsCount(HumanName);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TAuctionDB:GetMyBuyOKItemsCount;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TAuctionDB.AddAuctionItem(HumanName: string; AuctionTime, StartingPrice, SellingPrice, CurrencyType: Integer;
  UserItem: PTUserItem; StdItem: PTStdItem): Integer;
begin
  FOwner.Lock;
  try
    Result := 0;
    try
      Result := DoAddAuctionItem(HumanName, AuctionTime, StartingPrice, SellingPrice, CurrencyType, UserItem, StdItem);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TAuctionDB:AddAuctionItem;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TAuctionDB.CancelAuctionItem(HumanName: string; AuctionID: Integer): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoCancelAuctionItem(HumanName, AuctionID);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TAuctionDB:CancelAuctionItem;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TAuctionDB.RetrieveAuctionItem(AuctionID: Integer): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoRetrieveAuctionItem(AuctionID);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TAuctionDB:RetrieveAuctionItem;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TAuctionDB.DeleteAuctionItem(HumanName: string; AuctionID: Integer): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoDeleteAuctionItem(HumanName, AuctionID);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TAuctionDB:DeleteAuctionItem;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TAuctionDB.AddAttentionItem(HumanName: string; Index: Integer): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoAddAttentionItem(HumanName, Index);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TAuctionDB:AddAttentionItem;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TAuctionDB.DeleteAttentionItem(HumanName: string; Index: Integer): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoDeleteAttentionItem(HumanName, Index);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TAuctionDB:DeleteAttentionItem;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TAuctionDB.JoinItemBid(HumanName: string; Index, Prices: Integer; IsSell: Boolean): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoJoinItemBid(HumanName, Index, Prices, IsSell);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TAuctionDB:JoinItemBid;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TAuctionDB.GetAuctionInfo(Index: Integer; var AuctionInfo: TAuctionInfo): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoGetAuctionInfo(Index, AuctionInfo);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TAuctionDB:GetAuctionInfo;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TAuctionDB.GetAuctionRecord(Index: Integer; var AuctionRecord: TAuctionRecord): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoGetAuctionRecord(Index, AuctionRecord);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TAuctionDB:GetAuctionRecord;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

function TAuctionDB.HumanRename(OldName, NewName: string): Boolean;
begin
  FOwner.Lock;
  try
    Result := False;
    try
      Result := DoHumanRename(OldName, NewName);
    except
      on E: Exception do
      begin
        MainOutMessage('[Exception] TAuctionDB:HumanRename;' + E.Message);
      end;
    end;
  finally
    FOwner.UnLock;
  end;
end;

procedure TAuctionDB.Run;
begin
  try
    DoRun();
    FRunTick := MyGetTickCount;
  except
    on E: Exception do
    begin
      MainOutMessage('[Exception] TAuctionDB:Run;' + E.Message);
    end;
  end;
end;

{ TM2DataDB }
constructor TM2DataDB.Create;
begin
  FIsInitOK := False;
  InitializeCriticalSection(FCS);
  FAuctionDB := GetAuctionDBClass.Create(Self);
  FUserShopDB := GetUserShopDBClass.Create(Self);
  FStorageDB := GetStorageDBClass.Create(Self);
end;

destructor TM2DataDB.Destroy;
begin
  FAuctionDB.Free;
  FUserShopDB.Free;
  FStorageDB.Free;
  DeleteCriticalSection(FCS);
  inherited;
end;

procedure TM2DataDB.Init;
begin
  DoInit;

  FIsInitOK := True;

  FAuctionDB.DoInit;
  FUserShopDB.DoInit;
  FStorageDB.DoInit;
end;

procedure TM2DataDB.Final;
begin
  DoFinal;
  FAuctionDB.DoFinal;
  FUserShopDB.DoFinal;
  FStorageDB.DoFinal;
end;

procedure TM2DataDB.LoadItemFromDB(UserItem: PTUserItem; ParentID, ItemType, ItemIndex: Integer);
begin
  try
    DoLoadItemFromDB(UserItem, ParentID, ItemType, ItemIndex);
  except
    on E: Exception do
    begin
      MainOutMessage('[Exception] TM2DataDB:LoadItemFromDB;' + E.Message);
    end;
  end;
end;

procedure TM2DataDB.SaveItemToDB(UserItem: PTUserItem; ParentID, ItemType, ItemIndex: Integer);
begin
  try
    DoSaveItemToDB(UserItem, ParentID, ItemType, ItemIndex);
  except
    on E: Exception do
    begin
      MainOutMessage('[Exception] TM2DataDB:SaveItemToDB;' + E.Message);
    end;
  end;
end;

procedure TM2DataDB.LoadItemsFromDB(ParentID, ItemType: Integer; IsSort: Boolean; List: TList);
begin
  try
    DoLoadItemsFromDB(ParentID, ItemType, IsSort, List);
  except
    on E: Exception do
    begin
      MainOutMessage('[Exception] TM2DataDB:LoadItemsFromDB;' + E.Message);
    end;
  end;
end;

procedure TM2DataDB.Lock;
begin
  EnterCriticalSection(FCS);
end;

procedure TM2DataDB.UnLock;
begin
  LeaveCriticalSection(FCS);
end;

procedure TM2DataDB.Run;
begin

end;

end.
