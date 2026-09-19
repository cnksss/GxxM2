// 源单元（文件头证据，供 tools/audit-coverage.ps1 的 E2 规则识别）：
//   Source/M2Engine/M2DataCommon.pas   —— TUserShopDB / TAuctionDB / TM2DataDB 抽象基类与列表容器
//   Source/M2Engine/SqliteUserShopDB.pas
//   Source/M2Engine/MySqlUserShopDB.pas
//   Source/M2Engine/SqliteM2DataDB.pas
//   Source/M2Engine/MySqlM2DataDB.pas
//   Source/M2Engine/SqliteAuctionDB.pas
//   Source/M2Engine/MySqlAuctionDB.pas
//
// 本文件 = 六个 DB 访问单元共用的「数据库访问接缝层」（DbLayer）。
//
// 【为什么需要接缝】
//   原文的数据访问依赖 4 个**不在本仓库源码树内**的第三方包装单元：
//     TSQLite3Database / TSQLStatement（SQLite3DataBase.pas + SQLiteCli.pas）
//     TMySqlDatabase   / TMySqlStatement（MySqlDataBase.pas + MySqlWrap.pas，直连 libmysql-32.dll）
//   这三对类型在 Source/ 下既没有 .pas 也没有 .inc（已全树搜索确认），属外部依赖。
//   按 GXX.CSharp/docs/转换开发文档.md §2.3 与并行规程，本车道**不移植**它们，
//   而以接口把"原文实际用到的能力"逐一抽象出来，默认实现为真实 ADO 适配器
//   （SqliteStatementAdapter / MySqlStatementAdapter），单测注入内存实现，**不连真实数据库**。
//
// 【为什么把 SQLite 与 MySQL 两套接口分开】
//   两者的原文 API 语义**并不等价**，混成一个接口会丢掉原文语义：
//     SQLite：Step: Integer，结果集用 `while Ret = SQLITE_ROW do`；无结果集语句用
//             `Result := Step in [SQLITE_OK, SQLITE_DONE]`；事务 BeginTransaction/Commit/RollBack；
//             批量脚本 Execute(sql)；时间以 Unix 秒（Int64）绑/读。
//     MySQL ：Step: Boolean（无结果集语句）；结果集必须先 Query 再 `while Fetch`；
//             事务 StartTransaction/Commit/RollBack；批量脚本 Exec(sql)；时间以 DateTime 绑/读。
//   分开后 C# 侧可以**逐字保留**原文每个方法的调用形态（见 SqliteUserShopDB.cs / MySqlUserShopDB.cs）。

using System;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;

namespace GXX.M2Server.DbLayer;

// ---------------------------------------------------------------------------
// 1. 宿主（TM2DataDB 角色）
// ---------------------------------------------------------------------------

/// <summary>
/// M2DataCommon.pas:460-504 <c>TM2DataDB</c> 的角色接缝。
/// 六个 DB 单元只需要它的 4 个能力：互斥锁、取底层 DataBase 对象（TUserShopDB.Create 用 is 判定取 FDB）、
/// 物品读取/写回（DoGetSellItems / DoAddItem 调 Owner.LoadItemFromDB / SaveItemToDB）、日志。
/// 接缝：待 M2DataCommon.pas 的 TM2DataDB 完整移植后接入（本车道不顺手移植）。
/// </summary>
public interface IDbLayerHost
{
    /// <summary>M2DataCommon.pas:1689-1692 <c>TM2DataDB.Lock</c>（EnterCriticalSection）。</summary>
    void Lock();

    /// <summary>M2DataCommon.pas:1694-1697 <c>TM2DataDB.UnLock</c>（LeaveCriticalSection）。</summary>
    void UnLock();

    /// <summary>
    /// M2DataCommon.pas:486 <c>property DataBase: TObject read GetDataBase;</c>。
    /// 原文 SqliteUserShopDB.DoInit 用 <c>Owner.DataBase is TSqlite3DataBase</c> 判定后取 FDB；
    /// 托管侧同样是"判定类型再取"，故此处保持 <see cref="object"/> 形状。
    /// </summary>
    object? DataBase { get; }

    /// <summary>M2DataCommon.pas:480 <c>DoLoadItemFromDB(UserItem, ParentID, ItemType, ItemIndex)</c>（外层包 try/except）。</summary>
    void LoadItemFromDB(TUserItem userItem, int parentId, int itemType, int itemIndex);

    /// <summary>M2DataCommon.pas:480 <c>DoSaveItemToDB(UserItem, ParentID, ItemType, ItemIndex)</c>（外层包 try/except）。</summary>
    void SaveItemToDB(TUserItem userItem, int parentId, int itemType, int itemIndex);
}

/// <summary>
/// 日志接缝。<c>MainOutMessage</c>（M2Share.pas）在本车道未移植；
/// 原文每个 <c>DoXxx</c> 的 <c>except on E: Exception do MainOutMessage(E.Message)</c> 全部保留，
/// 只是把输出目标换成这里。
/// </summary>
public interface IDbLayerLog
{
    void MainOutMessage(string msg);
}

/// <summary>默认日志实现：写 <see cref="Console.Error"/>（原文 MainOutMessage 输出到 GUI 日志框）。</summary>
public sealed class ConsoleDbLayerLog : IDbLayerLog
{
    public static readonly ConsoleDbLayerLog Instance = new();

    public void MainOutMessage(string msg) => Console.Error.WriteLine(msg);
}

// ---------------------------------------------------------------------------
// 2. g_Config 相关全局（M2Share.pas unit-level var，六单元只读这些成员）
// ---------------------------------------------------------------------------

/// <summary>
/// 原文里被六个 DB 单元读取的 <c>g_Config.*</c> 成员（M2Share.pas）。
/// 全部是 unit-level 全局量，托管侧按 docs/转换开发文档.md §3.3 收进一个静态类。
/// 接缝：待 M2Server 的 g_Config（<c>GXX.M2Server.Engine.M2Config</c>）补齐这些成员后，
/// 把 <see cref="M2ConfigDbEnvironment"/> 的读数改为直接转调 M2Config（本车道不改既有 src/**）。
/// </summary>
public interface IDbLayerEnvironment
{
    /// <summary>M2Share <c>g_Config.boOfflineCloseMyShop</c>（离线关闭个人商店）。M2Config.FunctionShop.cs:18 已有。</summary>
    bool boOfflineCloseMyShop { get; }

    /// <summary>M2Share <c>g_Config.boEnabledMySellShopItemTime</c>（个人商店寄售超时处理开关）。</summary>
    bool boEnabledMySellShopItemTime { get; }

    /// <summary>M2Share <c>g_Config.nMySellShopItemTime</c>（寄售超时分钟数；SQL 里乘 60 转秒）。</summary>
    int nMySellShopItemTime { get; }

    /// <summary>M2Share <c>g_Config.nMaxMyShopStorageItemCount</c>。M2Config.FunctionShop.cs:17 已有。</summary>
    int nMaxMyShopStorageItemCount { get; }

    /// <summary>M2Share <c>g_Config.nHumanMaxGold</c>。M2Config.GameOption.cs:97 已有。</summary>
    int nHumanMaxGold { get; }

    /// <summary>M2Share <c>g_Config.btAuctionItemColors</c>（拍卖行按颜色过滤）。</summary>
    int btAuctionItemColors { get; }

    /// <summary>M2Share <c>g_Config.nAuctionCurrencyType</c>（拍卖行默认货币类型）。</summary>
    int nAuctionCurrencyType { get; }

    /// <summary>M2Share <c>g_Config.sGameGoldName</c>。M2Config.OnlineMsg.cs:47 已有。</summary>
    string sGameGoldName { get; }

    /// <summary>M2Share <c>g_Config.sGamePointName</c>。M2Config.OnlineMsg.cs:48 已有。</summary>
    string sGamePointName { get; }

    /// <summary>M2Share <c>g_Config.sGameDiamondName</c>。M2Config.OnlineMsg.cs:49 已有。</summary>
    string sGameDiamondName { get; }

    /// <summary>M2Share <c>g_Config.sGameGirdName</c>。M2Config.OnlineMsg.cs:50 已有。</summary>
    string sGameGirdName { get; }

    /// <summary>M2Share <c>g_Config.dwAuctionGoldTaxRate</c>（拍卖行金币税率）。</summary>
    uint dwAuctionGoldTaxRate { get; }

    /// <summary>M2Share <c>g_Config.dwAuctionGameGoldTaxRate</c>。</summary>
    uint dwAuctionGameGoldTaxRate { get; }

    /// <summary>M2Share <c>g_Config.dwAuctionGameDiamondTaxRate</c>。</summary>
    uint dwAuctionGameDiamondTaxRate { get; }

    /// <summary>M2Share <c>g_Config.dwAuctionGameGirdTaxRate</c>。</summary>
    uint dwAuctionGameGirdTaxRate { get; }

    /// <summary>M2Share <c>g_Config.dwAuctionGamePointTaxRate</c>。</summary>
    uint dwAuctionGamePointTaxRate { get; }
}

/// <summary>
/// <see cref="IDbLayerEnvironment"/> 的默认实现：能从既有 <c>M2Config</c> 读的直接读，
/// 尚未移植的成员给出与原文 M2Share.pas 声明一致的默认值（可写字段，供接入后赋值）。
/// </summary>
public sealed class M2ConfigDbEnvironment : IDbLayerEnvironment
{
    public static readonly M2ConfigDbEnvironment Instance = new();

    public bool boOfflineCloseMyShop => M2Config.boOfflineCloseMyShop;
    public int nMaxMyShopStorageItemCount => M2Config.nMaxMyShopStorageItemCount;
    public int nHumanMaxGold => M2Config.nHumanMaxGold;
    public string sGameGoldName => M2Config.sGameGoldName;
    public string sGamePointName => M2Config.sGamePointName;
    public string sGameDiamondName => M2Config.sGameDiamondName;
    public string sGameGirdName => M2Config.sGameGirdName;

    // 接缝：以下 6 个成员在既有 M2Config 中尚无对应字段（属 g_Config 未移植部分）；
    // 这里以可写静态字段承载，待 M2Config 补齐后改为转调（禁止在本车道改 src/GXX.M2Server/Engine/**）。
    /// <summary>接缝：M2Share <c>g_Config.boEnabledMySellShopItemTime</c>。</summary>
    public static bool SeamBoEnabledMySellShopItemTime;

    /// <summary>接缝：M2Share <c>g_Config.nMySellShopItemTime</c>（分钟）。</summary>
    public static int SeamNMySellShopItemTime;

    /// <summary>接缝：M2Share <c>g_Config.btAuctionItemColors</c>。</summary>
    public static int SeamBtAuctionItemColors;

    /// <summary>接缝：M2Share <c>g_Config.nAuctionCurrencyType</c>。</summary>
    public static int SeamNAuctionCurrencyType;

    /// <summary>接缝：M2Share <c>g_Config.dwAuctionGoldTaxRate</c>。</summary>
    public static uint SeamDwAuctionGoldTaxRate;

    /// <summary>接缝：M2Share <c>g_Config.dwAuctionGameGoldTaxRate</c>。</summary>
    public static uint SeamDwAuctionGameGoldTaxRate;

    /// <summary>接缝：M2Share <c>g_Config.dwAuctionGameDiamondTaxRate</c>。</summary>
    public static uint SeamDwAuctionGameDiamondTaxRate;

    /// <summary>接缝：M2Share <c>g_Config.dwAuctionGameGirdTaxRate</c>。</summary>
    public static uint SeamDwAuctionGameGirdTaxRate;

    /// <summary>接缝：M2Share <c>g_Config.dwAuctionGamePointTaxRate</c>。</summary>
    public static uint SeamDwAuctionGamePointTaxRate;

    public bool boEnabledMySellShopItemTime => SeamBoEnabledMySellShopItemTime;
    public int nMySellShopItemTime => SeamNMySellShopItemTime;
    public int btAuctionItemColors => SeamBtAuctionItemColors;
    public int nAuctionCurrencyType => SeamNAuctionCurrencyType;
    public uint dwAuctionGoldTaxRate => SeamDwAuctionGoldTaxRate;
    public uint dwAuctionGameGoldTaxRate => SeamDwAuctionGameGoldTaxRate;
    public uint dwAuctionGameDiamondTaxRate => SeamDwAuctionGameDiamondTaxRate;
    public uint dwAuctionGameGirdTaxRate => SeamDwAuctionGameGirdTaxRate;
    public uint dwAuctionGamePointTaxRate => SeamDwAuctionGamePointTaxRate;
}

/// <summary>
/// 六个 DB 单元读取全局量的入口。<see cref="Environment"/> 可在单测里替换为内存实现。
/// （原文直接引用 unit-level <c>g_Config</c>，托管侧以静态属性承载同一语义。）
/// </summary>
public static class DbLayerGlobals
{
    /// <summary>对应原文的 <c>g_Config</c> 引用。</summary>
    public static IDbLayerEnvironment Environment { get; set; } = M2ConfigDbEnvironment.Instance;

    /// <summary>对应原文的 <c>MainOutMessage</c>。</summary>
    public static IDbLayerLog Log { get; set; } = ConsoleDbLayerLog.Instance;

    /// <summary>
    /// 对应原文的 <c>MyGetTickCount</c>（M2Share.pas；被 TUserShopDB/TAuctionDB 的 FRunTick 使用）。
    /// 接缝：待 M2Share 移植后改调其实现；默认为 <see cref="System.Environment.TickCount"/>（uint 回绕）。
    /// </summary>
    public static Func<uint> GetTickCount { get; set; } = () => unchecked((uint)System.Environment.TickCount);

    /// <summary>把全局量恢复为默认实现（测试用）。</summary>
    public static void ResetDefaults()
    {
        Environment = M2ConfigDbEnvironment.Instance;
        Log = ConsoleDbLayerLog.Instance;
        GetTickCount = () => unchecked((uint)System.Environment.TickCount);
    }

    /// <summary>原文化简：<c>MainOutMessage(msg)</c>。</summary>
    public static void MainOutMessage(string msg) => Log.MainOutMessage(msg);
}

// ---------------------------------------------------------------------------
// 3. 六个单元对外暴露的"业务侧"接口（M2DataCommon 基类的 public 面）
// ---------------------------------------------------------------------------

/// <summary>
/// M2DataCommon.pas:159-257 <c>TUserShopDB</c> 的 public 面
/// （基类方法全部 <c>FOwner.Lock; try DoXxx; except MainOutMessage; finally FOwner.UnLock</c>）。
/// </summary>
public interface IUserShopDb
{
    /// <summary>M2DataCommon.pas:211 <c>constructor Create(AOwner: TM2DataDB)</c> 之后的 <c>DoInit</c>。</summary>
    void Init();

    /// <summary>M2DataCommon.pas:1649-1650 的 <c>DoFinal</c>。</summary>
    void Final();

    /// <summary>M2DataCommon.pas:216 <c>GetAllShop</c>。</summary>
    int GetAllShop(int startIndex, string keyword, bool isKeywordHumanName, int sortType, UserShopList shopList);

    /// <summary>M2DataCommon.pas:218 <c>GetAllShopCount</c>。</summary>
    int GetAllShopCount(string keyword, bool isKeywordHumanName);

    /// <summary>M2DataCommon.pas:220 <c>GetAllShopEx</c>。</summary>
    int GetAllShopEx(UserShopList shopList);

    /// <summary>M2DataCommon.pas:222-224 <c>GetSellItems</c>（ShopItemType 默认 sitSelling）。</summary>
    int GetSellItems(bool isMyShop, int startIndex, string humanName, string keyword, int itemType, int moneyType,
        int minPrice, int maxPrice, int sortType, UserShopItemList itemList,
        TShopItemType shopItemType = TShopItemType.sitSelling);

    /// <summary>M2DataCommon.pas:225-226 <c>GetSellItemsCount</c>。</summary>
    int GetSellItemsCount(bool isMyShop, string humanName, string keyword, int itemType, int moneyType,
        int minPrice, int maxPrice, TShopItemType shopItemType = TShopItemType.sitSelling);

    /// <summary>M2DataCommon.pas:230-231 <c>GetHumanItemWithMakeIndex</c>（var 参数 → 返回元组）。</summary>
    bool GetHumanItemWithMakeIndex(bool isMyShop, string humanName, TShopItemType shopItemType,
        int itemMakeIndex, out TSimpleUserShopItem userShopItem);

    /// <summary>M2DataCommon.pas:233 <c>GetSelledAndNoGetMoneyTotal</c>。</summary>
    int GetSelledAndNoGetMoneyTotal(System.Collections.Generic.List<TSelledAndNoGetMoneyTotal> itemList);

    /// <summary>M2DataCommon.pas:235 <c>GetUserShopInfo</c>（var 参数 → out）。</summary>
    bool GetUserShopInfo(string humanName, out TUserShop userShop);

    /// <summary>M2DataCommon.pas:237 <c>IncUserShopCareValue</c>。</summary>
    void IncUserShopCareValue(string humanName);

    /// <summary>M2DataCommon.pas:239 <c>HumanNameExists</c>。</summary>
    bool HumanNameExists(string humanName);

    /// <summary>M2DataCommon.pas:240 <c>ShopNameExists</c>。</summary>
    bool ShopNameExists(string shopName);

    /// <summary>M2DataCommon.pas:242 <c>ShopAdd(ShopName, HumanName)</c>。</summary>
    bool ShopAdd(string shopName, string humanName);

    /// <summary>M2DataCommon.pas:243 <c>ShopDelete</c>。</summary>
    bool ShopDelete(int shopId);

    /// <summary>M2DataCommon.pas:244 <c>ShopRename</c>。</summary>
    bool ShopRename(int shopId, string newShopName);

    /// <summary>M2DataCommon.pas:246 <c>HumanRename</c>。</summary>
    bool HumanRename(string oldName, string newName);

    /// <summary>M2DataCommon.pas:248 <c>UpdateHumanBusiness</c>。</summary>
    bool UpdateHumanBusiness(string humanName, bool isBusiness);

    /// <summary>M2DataCommon.pas:250 <c>AddItem</c>。</summary>
    bool AddItem(int shopId, TUserShopItem shopItem, string sItemName);

    /// <summary>M2DataCommon.pas:251 <c>UpdateItem</c>。</summary>
    bool UpdateItem(int shopId, int itemId, int btItemType, int btAllowSell, int btMoneyType, int nPrice);

    /// <summary>M2DataCommon.pas:252 <c>BuyItem</c>。</summary>
    bool BuyItem(int shopId, int itemId, string buyer);

    /// <summary>M2DataCommon.pas:253 <c>GetMoneyItem</c>。</summary>
    bool GetMoneyItem(int shopId, int itemId);

    /// <summary>M2DataCommon.pas:254 <c>DeleteItem</c>。</summary>
    bool DeleteItem(int shopId, int itemId);

    /// <summary>M2DataCommon.pas:256 <c>Run</c>（DoRun + FRunTick := MyGetTickCount）。</summary>
    void Run();
}

/// <summary>M2DataCommon.pas:317-454 <c>TAuctionDB</c> 的 public 面。</summary>
public interface IAuctionDb
{
    void Init();
    void Final();

    int QueryAllItems(string itemName, int itemGroup, string humanName, int nPage, int topmostAuctionId,
        int itemColors, int sortField, bool sortAsc, int moneyType, uint minPrices, uint maxPrices, TAuctionItemList itemList);

    int QueryMyItems(string humanName, int nPage, TAuctionItemList itemList);
    int QueryMyAttentionItems(string humanName, int nPage, TAuctionItemList itemList);
    int GetAllItemsPageCount(string itemName, int itemGroup, int itemColors, int moneyType, uint minPrices, uint maxPrices);
    int GetMyItemsPageCount(string humanName);
    int GetMyAttentionPageCount(string humanName);
    int GetMyAuctioningItemsCount(string humanName);
    int GetMySellFailItemsCount(string humanName);
    int GetMyBuyOKItemsCount(string humanName);
    int AddAuctionItem(string humanName, int auctionTime, int startingPrice, int sellingPrice, int currencyType,
        TUserItem userItem, object? stdItem);
    bool CancelAuctionItem(string humanName, int auctionId);
    bool RetrieveAuctionItem(int auctionId);
    bool DeleteAuctionItem(string humanName, int auctionId);
    bool AddAttentionItem(string humanName, int index);
    bool DeleteAttentionItem(string humanName, int index);
    bool JoinItemBid(string humanName, int index, int prices, bool isSell);
    bool GetAuctionInfo(int index, out TAuctionInfo auctionInfo);
    bool GetAuctionRecord(int index, out TAuctionRecord auctionRecord);
    bool HumanRename(string oldName, string newName);
    void Run();
}

/// <summary>
/// M2DataCommon.pas:460-504 <c>TM2DataDB</c> 的公开面（Database/AuctionDB/UserShopDB/StorageDB 四个属性）。
/// 接缝：完整 TM2DataDB 不在本车道范围；本接口只暴露六个 DB 单元需要被挂载的位置。
/// </summary>
public interface IM2DataDb
{
    /// <summary>M2DataCommon.pas:486 <c>property DataBase: TObject read GetDataBase</c>。</summary>
    object? DataBase { get; }

    /// <summary>M2DataCommon.pas:487 <c>property AuctionDB: TAuctionDB</c>。</summary>
    IAuctionDb? AuctionDB { get; }

    /// <summary>M2DataCommon.pas:488 <c>property UserShopDB: TUserShopDB</c>。</summary>
    IUserShopDb? UserShopDB { get; }

    /// <summary>M2DataCommon.pas:494 <c>LoadItemsFromDB</c>。</summary>
    void LoadItemsFromDB(int parentId, int itemType, bool isSort, System.Collections.Generic.List<TUserItem> list);

    /// <summary>M2DataCommon.pas:495 <c>LoadItemFromDB</c>。</summary>
    void LoadItemFromDB(TUserItem userItem, int parentId, int itemType, int itemIndex);

    /// <summary>M2DataCommon.pas:496 <c>SaveItemToDB</c>。</summary>
    void SaveItemToDB(TUserItem userItem, int parentId, int itemType, int itemIndex);

    void Init();
    void Final();

    /// <summary>M2DataCommon.pas:501 <c>property IsInitOK: Boolean</c>。</summary>
    bool IsInitOK { get; }

    void Run();
}

// ---------------------------------------------------------------------------
// 4. 底层 SQLite3DataBase / SQLiteCli 接缝
// ---------------------------------------------------------------------------

/// <summary>
/// Source/…/SQLite3DataBase.pas + SQLiteCli.pas 的 <c>TSQLStatement</c>（**不在源码树内**，外部依赖）。
/// 接口逐项对应原文 SqliteUserShopDB/SqliteM2DataDB/SqliteAuctionDB 实际调用的成员（已 grep 全量确认）。
/// </summary>
public interface ISqliteStatement
{
    /// <summary><c>TSQLStatement.Sql</c>（DoInit 里 AddSQLStatement 之后赋值，再 Prepare）。</summary>
    string Sql { get; set; }

    /// <summary><c>Prepare</c>。</summary>
    void Prepare();

    /// <summary><c>Finalize</c>（C# 中与 Object.Finalize 冲突，改名 StatementFinalize，调用点保持原文顺序）。</summary>
    void StatementFinalize();

    /// <summary><c>Reset</c>（清绑定 + 复位游标）。</summary>
    void Reset();

    /// <summary><c>OrderBindInt</c>（顺序绑定到下一个占位符）。</summary>
    void OrderBindInt(int value);

    /// <summary><c>OrderBindInt64</c>。</summary>
    void OrderBindInt64(long value);

    /// <summary><c>OrderBindBool</c>（Delphi Boolean → SQLite 整数 0/1）。</summary>
    void OrderBindBool(bool value);

    /// <summary><c>OrderBindText</c>。</summary>
    void OrderBindText(string value);

    /// <summary><c>Step</c>：返回 <c>SQLITE_ROW</c>/<c>SQLITE_DONE</c>/错误码（见 <see cref="SqliteCodes"/>）。</summary>
    int Step();

    /// <summary><c>OrderGetColumnValueInt</c>（Ordinal 递增取列）。</summary>
    int OrderGetColumnValueInt { get; }

    /// <summary><c>OrderGetColumnValueInt64</c>。</summary>
    long OrderGetColumnValueInt64 { get; }

    /// <summary><c>OrderGetColumnValueBool</c>。</summary>
    bool OrderGetColumnValueBool { get; }

    /// <summary><c>OrderGetColumnValueText</c>。</summary>
    string OrderGetColumnValueText { get; }

    /// <summary><c>OrderGetColumnValueDouble</c>（SqliteM2DataDB 用到）。</summary>
    double OrderGetColumnValueDouble { get; }
}

/// <summary>SQLite 返回码常量（SQLiteCli.pas；<c>SQLITE_OK=0 / SQLITE_ROW=100 / SQLITE_DONE=101</c>）。</summary>
public static class SqliteCodes
{
    public const int SQLITE_OK = 0;
    public const int SQLITE_ROW = 100;
    public const int SQLITE_DONE = 101;
}

/// <summary>
/// Source/…/SQLite3DataBase.pas 的 <c>TSQLite3Database</c>（**不在源码树内**，外部依赖）。
/// </summary>
public interface ISqliteDatabase
{
    /// <summary><c>TSQLite3Database.Statements.AddSQLStatement(name)</c>。</summary>
    ISqliteStatement AddSQLStatement(string name);

    /// <summary><c>BeginTransaction</c>。</summary>
    void BeginTransaction();

    /// <summary><c>Commit</c>。</summary>
    void Commit();

    /// <summary><c>RollBack</c>。</summary>
    void RollBack();

    /// <summary><c>Execute(sql)</c>（多语句脚本）。</summary>
    void Execute(string sql);

    /// <summary><c>Connected</c>（SqliteM2DataDB 用到）。</summary>
    bool Connected { get; set; }

    /// <summary><c>Database</c>（库文件路径；SqliteM2DataDB 用到）。</summary>
    string Database { get; set; }

    /// <summary><c>MustExist</c>（SqliteM2DataDB 用到）。</summary>
    bool MustExist { get; set; }

    /// <summary><c>UseThreadMode</c>（SqliteM2DataDB 用到）。</summary>
    bool UseThreadMode { get; set; }

    /// <summary><c>ErrorCode</c>（SqliteM2DataDB 用到）。</summary>
    int ErrorCode { get; }
}

// ---------------------------------------------------------------------------
// 5. 底层 MySqlDataBase / MySqlWrap 接缝
// ---------------------------------------------------------------------------

/// <summary>
/// Source/…/MySqlDataBase.pas + MySqlWrap.pas 的 <c>TMySqlStatement</c>（**不在源码树内**，外部依赖；
/// 原文直接调 libmysql-32.dll）。接口逐项对应三个 MySQL 单元实际调用的成员。
/// </summary>
public interface IMySqlStatement
{
    /// <summary><c>TMySqlStatement.Sql</c>（DoInit 里 AddSQLStatement 之后赋值，再 Prepare）。</summary>
    string Sql { get; set; }

    /// <summary><c>Prepare</c>。</summary>
    void Prepare();

    /// <summary><c>Finalize</c>（改名，见 <see cref="ISqliteStatement.StatementFinalize"/>）。</summary>
    void StatementFinalize();

    /// <summary><c>Reset</c>。</summary>
    void Reset();

    /// <summary><c>OrderBindParamInt</c>。</summary>
    void OrderBindParamInt(int value);

    /// <summary><c>OrderBindParamBool</c>（MySQL 侧绑为 0/1）。</summary>
    void OrderBindParamBool(bool value);

    /// <summary><c>OrderBindParamText</c>。</summary>
    void OrderBindParamText(string value);

    /// <summary><c>OrderBindParamDateTime</c>。</summary>
    void OrderBindParamDateTime(DateTime value);

    /// <summary><c>OrderBindDouble</c>（MySqlM2DataDB 用到）。</summary>
    void OrderBindDouble(double value);

    /// <summary><c>Query</c>：执行并准备结果集；无结果集语句返回 false。</summary>
    bool Query();

    /// <summary><c>Fetch</c>：游标前移一行。</summary>
    bool Fetch();

    /// <summary><c>Step</c>：执行无结果集语句（INSERT/UPDATE/DELETE）。</summary>
    bool Step();

    /// <summary><c>RowCount</c>（MySqlAuctionDB 的 CheckInAttentionItem 用到）。</summary>
    int RowCount { get; }

    /// <summary><c>OrderGetColumnValueInt</c>（Ordinal 递增取列）。</summary>
    int OrderGetColumnValueInt { get; }

    /// <summary><c>OrderGetColumnValueInt64</c>。</summary>
    long OrderGetColumnValueInt64 { get; }

    /// <summary><c>OrderGetColumnValueBool</c>。</summary>
    bool OrderGetColumnValueBool { get; }

    /// <summary><c>OrderGetColumnValueText</c>。</summary>
    string OrderGetColumnValueText { get; }

    /// <summary><c>OrderGetColumnValueDouble</c>（MySqlM2DataDB 用到）。</summary>
    double OrderGetColumnValueDouble { get; }

    /// <summary><c>OrderGetColumnValueDateTime</c>。</summary>
    DateTime OrderGetColumnValueDateTime { get; }
}

/// <summary>
/// Source/…/MySqlDataBase.pas 的 <c>TMySqlDatabase</c>（**不在源码树内**，外部依赖）。
/// </summary>
public interface IMySqlDatabase
{
    /// <summary><c>TMySqlDatabase.Statements.AddSQLStatement(name)</c>。</summary>
    IMySqlStatement AddSQLStatement(string name);

    /// <summary><c>StartTransaction</c>。</summary>
    void StartTransaction();

    /// <summary><c>Commit</c>。</summary>
    void Commit();

    /// <summary><c>RollBack</c>。</summary>
    void RollBack();

    /// <summary><c>Exec(sql)</c>（多语句脚本）。</summary>
    void Exec(string sql);

    /// <summary><c>ClearResult</c>（MySqlAuctionDB 用到）。</summary>
    void ClearResult();

    /// <summary><c>Connect(server, user, password, database, port, flags)</c>（MySqlM2DataDB 用到）。</summary>
    void Connect(string server, string user, string password, string database, ushort port, uint flags);

    /// <summary><c>Init</c>（MySqlM2DataDB 用到）。</summary>
    void Init();

    /// <summary><c>CharacterSetName</c>（原文赋 'utf8'）。</summary>
    string CharacterSetName { get; set; }

    /// <summary><c>MySQL</c>（连接句柄；未连接为 null）。</summary>
    object? MySql { get; }

    /// <summary><c>OnRequest</c>（每次请求回调，用于刷新 FLastRequestTick）。</summary>
    event Action? OnRequest;
}

// ---------------------------------------------------------------------------
// 6. 默认接缝实现：未接入真实驱动时抛出明确异常
// ---------------------------------------------------------------------------

/// <summary>
/// 接缝默认实现：真实 SQLite/MySQL 驱动尚未接入时，任何调用都抛出带接入说明的异常。
/// 单测一律注入内存实现（tests/GXX.M2Server.Tests/DbLayer*Tests.cs）——**不在单测里连数据库**。
/// </summary>
public sealed class UnavailableSqliteDatabase : ISqliteDatabase
{
    public static readonly UnavailableSqliteDatabase Instance = new();

    private static Exception Fail() => new NotSupportedException(
        "接缝：真实 SQLite3DataBase 驱动未接入。请在装配处注入 ISqliteDatabase 实现（单测用 DbLayerFakeDatabase）。");

    public ISqliteStatement AddSQLStatement(string name) => throw Fail();
    public void BeginTransaction() => throw Fail();
    public void Commit() => throw Fail();
    public void RollBack() => throw Fail();
    public void Execute(string sql) => throw Fail();
    public bool Connected { get => false; set => throw Fail(); }
    public string Database { get => ""; set => throw Fail(); }
    public bool MustExist { get => false; set => throw Fail(); }
    public bool UseThreadMode { get => false; set => throw Fail(); }
    public int ErrorCode => 0;
}

/// <summary>接缝默认实现：真实 libmysql 驱动未接入时抛出带接入说明的异常。</summary>
public sealed class UnavailableMySqlDatabase : IMySqlDatabase
{
    public static readonly UnavailableMySqlDatabase Instance = new();

    private static Exception Fail() => new NotSupportedException(
        "接缝：真实 MySqlDataBase 驱动未接入。请在装配处注入 IMySqlDatabase 实现（单测用 DbLayerFakeDatabase）。");

    public IMySqlStatement AddSQLStatement(string name) => throw Fail();
    public void StartTransaction() => throw Fail();
    public void Commit() => throw Fail();
    public void RollBack() => throw Fail();
    public void Exec(string sql) => throw Fail();
    public void ClearResult() => throw Fail();
    public void Connect(string server, string user, string password, string database, ushort port, uint flags) => throw Fail();
    public void Init() => throw Fail();
    public string CharacterSetName { get => ""; set => throw Fail(); }
    public object? MySql => null;
    public event Action? OnRequest { add { } remove { } }
}
