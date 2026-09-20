// 源单元：Source/M2Engine/SqliteAuctionDB.pas（1-1603 行，1:1 移植）
//   TSqliteAuctionDB = class(TAuctionDB)：Create/Destroy、DoInit、DoFinal、
//   20 个 Do* 覆写（DoQueryAllItems / DoQueryMyItems / DoQueryMyAttentionItems / DoGetAllItemsPageCount /
//   DoGet*Count / DoAddAuctionItem / DoCancelAuctionItem / DoRetrieveAuctionItem / DoDeleteAuctionItem /
//   DoAddAttentionItem / DoDeleteAttentionItem / DoJoinItemBid / DoGetAuctionInfo / DoGetAuctionRecord /
//   DoHumanRename / DoRun）。
//   public 包装层（Lock/try/except MainOutMessage/finally UnLock、RunTick2 节流）已在 DbBases.cs:640-1039
//   的 TAuctionDB 中实现，本文件**只**写 protected Do* + Create/Destroy + private 字段/局部过程。
//
// SQL 逐字保真：21 条 <c>AddSQLStatement</c> 语句在 SqlStatements.SqliteAuctionDB.cs；方法级动态 SQL
// 模板（1 段多语句删除脚本 + 16 段动态拼接模板）在 SqlStatements.SqliteAuctionDB.Scripts.cs
// —— 全部由 _recon/p3-sql.mjs + p3-gen.mjs 从 GBK 原文机械抽取，**零手工转录**。
// 本实现把拼接点按原文顺序接到 <c>_P0/_P1/...</c> 常量上，送给 driver 的字符串与原文逐字一致。
//
// 方言要点（对照 MySqlAuctionDB）：
//   * 事务 BeginTransaction/Commit/RollBack；批量脚本 Execute(sql)；
//   * 结果集 `Ret := Step; while Ret = SQLITE_ROW do ... Ret := Step;`，单行 `if Step = SQLITE_ROW`；
//   * 无结果集语句的成功判据是 `Step in [SQLITE_OK, SQLITE_DONE]`（原文 SqliteAuctionDB.pas:573/676/710/736/748）；
//   * `sLineBreak`（Windows = #13#10）拼接多语句脚本；`AddDateTime` 在 SQLite 库里是 INTEGER（Unix 秒）；
//   * 唯一非方言语义差异：AddDateTime 在 SQLite 用 OrderGetColumnValueInt（原始 Unix 秒直接赋给
//     TDateTime），MySQL 用 OrderGetColumnValueDateTime（见 docs/并行报告-p2b-m2-dblayer.md:100）。
//
// 原文缺陷/易错点（逐字保留，测试锁定；详见 docs/并行报告-p3-m2-dbdata.md §7）：
//   * SqliteAuctionDB.pas:207/255/260/274/279/290/293/296/299 —— 一批 SQL **末尾缺分号**（原文如此）；
//   * SqliteAuctionDB.pas:545-591 —— DoAddAuctionItem 的 `if AuctionID > 0` 分支内没有 else，
//     GetMaxAuctionID 返回 0 时静默返回 0（不插入、不报错）；
//   * SqliteAuctionDB.pas:486-543 —— StdMode=28 在第 3 个分支（照明物）已被吃进 igSpecial，
//     第 14 个分支（马牌）是**死代码**（原文如此）；
//   * SqliteAuctionDB.pas:568 —— 用 `UserItem.btValue[13]`（不是 btValue[0]）判定改名物品；
//   * SqliteAuctionDB.pas:873/877/1189-1196/1892/1962 —— sColors 在语句内**不复位**（局部串先加后删尾逗号）；
//   * SqliteAuctionDB.pas:1351-1364 —— 玩家不在线时 GoldType 缺省（无 else）却仍调 HumanChangeGold；
//   * SqliteAuctionDB.pas:1212 —— DoGetMyAttentionPageCount 的 Reset 在 finally（原文如此）；
//   * SqliteAuctionDB.pas:1543-1544 —— `except end;` 空异常处理，吞掉 NPC GotoLable 段的全部异常；
//   * SqliteAuctionDB.pas:1567-1569 —— DoHumanRename 的 3 个 Reset 在 try **之外**（异常时不复位）。
//
// 已知接缝缺口（**已按父 agent 修补后的接缝订正，无遗留近似**）：
//   * 原文 1462 <c>StdItem.Name</c>（显示名）→ 已用 IAuctionStdItem.Name（M2DataDbSupport.cs:240）；
//   * 原文 1470 <c>StdItem.NeedIdentify = 1</c> → 已用 IAuctionStdItem.NeedIdentify（M2DataDbSupport.cs:244）。
// 其余接缝映射（非缺口）：
//   * 原文 568 <c>UserItem.btValue[13]</c> / <c>Length(UserItem.Name)</c> → 走 M2ItemDbAccess.GetValue / userItem.NameStr。
//   * 原文 1467 <c>Round(...)</c>（Delphi 银行家舍入）→ C# <c>Math.Round</c>（同为 ToEven），语义一致。

using System;
using GXX.Core.Protocol;

namespace GXX.M2Server.DbLayer;

/// <summary>SqliteAuctionDB.pas:17-118 <c>TSqliteAuctionDB</c>。</summary>
public sealed class TSqliteAuctionDB : TAuctionDB
{
    private ISqliteDatabase? _fdb;

    /// <summary><c>true</c> = 构造时显式注入了 db（优先于原文 202-203 的 Owner.DataBase 回退）。</summary>
    private readonly bool _dbInjected;

    /// <summary>SqliteAuctionDB.pas:21 <c>FStatementUpdateAuctionItemFail</c>（更新拍卖物品的状态）。</summary>
    private ISqliteStatement? _fStatementUpdateAuctionItemFail;

    /// <summary>SqliteAuctionDB.pas:22 <c>FStatementUpdateAuctionItemSuccess</c>（更新拍卖物品的状态）。</summary>
    private ISqliteStatement? _fStatementUpdateAuctionItemSuccess;

    /// <summary>SqliteAuctionDB.pas:23 <c>FStatementQueryAuctionItemSuccess</c>（查询拍卖成功的物品）。</summary>
    private ISqliteStatement? _fStatementQueryAuctionItemSuccess;

    /// <summary>SqliteAuctionDB.pas:25 <c>FStatementQueryMyItems</c>（查询我的拍卖物品）。</summary>
    private ISqliteStatement? _fStatementQueryMyItems;

    /// <summary>SqliteAuctionDB.pas:26 <c>FStatementGetMyItemsCount</c>（我正在拍卖物品总页数）。</summary>
    private ISqliteStatement? _fStatementGetMyItemsCount;

    /// <summary>SqliteAuctionDB.pas:28 <c>FStatementQueryMyAttentionItems</c>（查询关注物品）。</summary>
    private ISqliteStatement? _fStatementQueryMyAttentionItems;

    /// <summary>SqliteAuctionDB.pas:29 <c>FStatementQueryMyAttentionItemsCount</c>（我的关注物品总页数）。</summary>
    private ISqliteStatement? _fStatementQueryMyAttentionItemsCount;

    /// <summary>SqliteAuctionDB.pas:31 <c>FStatementQueryOneItem</c>（查询单个物品）。</summary>
    private ISqliteStatement? _fStatementQueryOneItem;

    /// <summary>SqliteAuctionDB.pas:33 <c>FStatementGetMyAuctioningItemsCount</c>（我正在拍卖物品的数量）。</summary>
    private ISqliteStatement? _fStatementGetMyAuctioningItemsCount;

    /// <summary>SqliteAuctionDB.pas:34 <c>FStatementGetMySellFailItemsCount</c>（我流拍未取的物品数量）。</summary>
    private ISqliteStatement? _fStatementGetMySellFailItemsCount;

    /// <summary>SqliteAuctionDB.pas:35 <c>FStatementGetMyBuyOKItemsCount</c>（我拍买未取的物品数量）。</summary>
    private ISqliteStatement? _fStatementGetMyBuyOKItemsCount;

    /// <summary>SqliteAuctionDB.pas:37 <c>FStatementGetMaxAuctionID</c>。</summary>
    private ISqliteStatement? _fStatementGetMaxAuctionID;

    /// <summary>SqliteAuctionDB.pas:38 <c>FStatementInsertAuctionItem</c>。</summary>
    private ISqliteStatement? _fStatementInsertAuctionItem;

    /// <summary>SqliteAuctionDB.pas:39 <c>FStatementInsertAttentionItem</c>。</summary>
    private ISqliteStatement? _fStatementInsertAttentionItem;

    /// <summary>SqliteAuctionDB.pas:40 <c>FStatementCheckInAttentionItem</c>。</summary>
    private ISqliteStatement? _fStatementCheckInAttentionItem;

    /// <summary>SqliteAuctionDB.pas:41 <c>FStatementDeleteAttentionItem</c>。</summary>
    private ISqliteStatement? _fStatementDeleteAttentionItem;

    /// <summary>SqliteAuctionDB.pas:43 <c>FStatementJoinItemBid</c>（参与物品竞价）。</summary>
    private ISqliteStatement? _fStatementJoinItemBid;

    /// <summary>SqliteAuctionDB.pas:44 <c>FStatementBuyItem</c>（一口价购买物品）。</summary>
    private ISqliteStatement? _fStatementBuyItem;

    /// <summary>SqliteAuctionDB.pas:46 <c>FStatementAuctionDataHumanRename</c>。</summary>
    private ISqliteStatement? _fStatementAuctionDataHumanRename;

    /// <summary>SqliteAuctionDB.pas:47 <c>FStatementAuctionDataLastBidderRename</c>。</summary>
    private ISqliteStatement? _fStatementAuctionDataLastBidderRename;

    /// <summary>SqliteAuctionDB.pas:48 <c>FStatementAuctionAttentionRename</c>。</summary>
    private ISqliteStatement? _fStatementAuctionAttentionRename;

    /// <summary>SqliteAuctionDB.pas:142-191 <c>constructor Create</c>：inherited Create(AOwner) 后
    /// 逐个语句字段置 nil（原文每行后面紧跟一个多余的空语句 `;`，见 147/149/151... 行）。
    /// <para><paramref name="db"/> 为接缝注入位（与同族 <c>TMySqlAuctionDB(owner, db)</c> 同形）；
    /// 未注入时 DoInit 仍按原文 202-203 的 <c>Owner.DataBase is TSqlite3DataBase</c> 判定回退
    /// （无宿主时会落到 <see cref="UnavailableSqliteDatabase"/> 并抛 NotSupportedException）。</para></summary>
    public TSqliteAuctionDB(IDbLayerHost owner, ISqliteDatabase? db = null) : base(owner)
    {
        // 与同族 TSqliteM2DataDB(owner, db) 同形：未注入时用"不可用"接缝占位。
        // 另记 _dbInjected 以便 DoInit 区分"显式注入"与"占位"——原文 202-203 的 is 判定回退只在未注入时生效。
        _fdb = db ?? UnavailableSqliteDatabase.Instance;
        _dbInjected = db != null;
        _fStatementUpdateAuctionItemFail = null;
        // 原文如此（SqliteAuctionDB.pas:147）：多余的空语句 `;`
        _fStatementUpdateAuctionItemSuccess = null;
        // 原文如此（SqliteAuctionDB.pas:149）：多余的空语句 `;`
        _fStatementQueryAuctionItemSuccess = null;
        // 原文如此（SqliteAuctionDB.pas:151）：多余的空语句 `;`

        _fStatementQueryMyItems = null;
        // 原文如此（SqliteAuctionDB.pas:154）：多余的空语句 `;`
        _fStatementGetMyItemsCount = null;
        // 原文如此（SqliteAuctionDB.pas:156）：多余的空语句 `;`

        _fStatementQueryMyAttentionItems = null;
        // 原文如此（SqliteAuctionDB.pas:159）：多余的空语句 `;`
        _fStatementQueryMyAttentionItemsCount = null;
        // 原文如此（SqliteAuctionDB.pas:161）：多余的空语句 `;`

        _fStatementQueryOneItem = null;
        // 原文如此（SqliteAuctionDB.pas:164）：多余的空语句 `;`

        _fStatementGetMyAuctioningItemsCount = null;
        // 原文如此（SqliteAuctionDB.pas:167）：多余的空语句 `;`

        _fStatementGetMaxAuctionID = null;
        // 原文如此（SqliteAuctionDB.pas:170）：多余的空语句 `;`
        _fStatementInsertAuctionItem = null;
        // 原文如此（SqliteAuctionDB.pas:172）：多余的空语句 `;`
        _fStatementInsertAttentionItem = null;
        // 原文如此（SqliteAuctionDB.pas:174）：多余的空语句 `;`
        _fStatementCheckInAttentionItem = null;
        // 原文如此（SqliteAuctionDB.pas:176）：多余的空语句 `;`
        _fStatementDeleteAttentionItem = null;
        // 原文如此（SqliteAuctionDB.pas:178）：多余的空语句 `;`

        _fStatementJoinItemBid = null;
        // 原文如此（SqliteAuctionDB.pas:181）：多余的空语句 `;`
        _fStatementBuyItem = null;
        // 原文如此（SqliteAuctionDB.pas:183）：多余的空语句 `;`

        _fStatementAuctionDataHumanRename = null;
        // 原文如此（SqliteAuctionDB.pas:186）：多余的空语句 `;`
        _fStatementAuctionDataLastBidderRename = null;
        // 原文如此（SqliteAuctionDB.pas:188）：多余的空语句 `;`
        _fStatementAuctionAttentionRename = null;
        // 原文如此（SqliteAuctionDB.pas:190）：多余的空语句 `;`
    }

    /// <summary>SqliteAuctionDB.pas:193-196 <c>destructor Destroy</c>：仅 inherited。
    /// 托管侧无显式析构；DoFinal 由宿主在 TM2DataDB.Final 时调用（与原文一致）。</summary>
    public void Destroy()
    {
        // 原文只有 inherited;（基类 TAuctionDB 未声明析构体）
    }

    /// <summary>SqliteAuctionDB.pas:198-336 <c>DoInit</c>。</summary>
    public override void DoInit()
    {
        // 原文 200 行 inherited;（基类 DoInit 为 abstract，无实现体）

        // 原文 202-203：if (Owner.DataBase <> nil) and (Owner.DataBase is TSqlite3DataBase) then FDB := Owner.DataBase as TSqlite3DataBase;
        // 未显式注入时按原文从宿主取；显式注入优先（与同族 TMySqlAuctionDB 的注入语义一致）。
        if (!_dbInjected && Owner.DataBase is ISqliteDatabase sqliteDb) _fdb = sqliteDb;

        ISqliteDatabase fdb = _fdb;

        _fStatementUpdateAuctionItemFail = fdb.AddSQLStatement("Auction_UpdateAuctionItemFail");
        _fStatementUpdateAuctionItemFail.Sql = SqliteAuctionDbStatements.Auction_UpdateAuctionItemFail;

        _fStatementUpdateAuctionItemSuccess = fdb.AddSQLStatement("Auction_UpdateAuctionItemSuccess");
        _fStatementUpdateAuctionItemSuccess.Sql = SqliteAuctionDbStatements.Auction_UpdateAuctionItemSuccess;

        _fStatementQueryAuctionItemSuccess = fdb.AddSQLStatement("Auction_QueryAuctionItemSuccess");
        _fStatementQueryAuctionItemSuccess.Sql = SqliteAuctionDbStatements.Auction_QueryAuctionItemSuccess;

        // 查询我的拍卖物品
        _fStatementQueryMyItems = fdb.AddSQLStatement("Auction_QueryAuctionItems");
        _fStatementQueryMyItems.Sql = SqliteAuctionDbStatements.Auction_QueryAuctionItems;

        // 我的拍卖物品总页数
        _fStatementGetMyItemsCount = fdb.AddSQLStatement("Auction_GetMyItemsCount");
        _fStatementGetMyItemsCount.Sql = SqliteAuctionDbStatements.Auction_GetMyItemsCount;

        // 查询我的关注
        _fStatementQueryMyAttentionItems = fdb.AddSQLStatement("Auction_QueryAttentionItems");
        _fStatementQueryMyAttentionItems.Sql = SqliteAuctionDbStatements.Auction_QueryAttentionItems;

        // 我的关注物品总页数
        _fStatementQueryMyAttentionItemsCount = fdb.AddSQLStatement("Auction_GetMyAttentionItemsCount");
        _fStatementQueryMyAttentionItemsCount.Sql = SqliteAuctionDbStatements.Auction_GetMyAttentionItemsCount;

        // 查询单个物品信息
        _fStatementQueryOneItem = fdb.AddSQLStatement("Auction_QueryOneItem");
        _fStatementQueryOneItem.Sql = SqliteAuctionDbStatements.Auction_QueryOneItem;

        // 我正在拍卖的物品数量
        _fStatementGetMyAuctioningItemsCount = fdb.AddSQLStatement("Auction_GetMyAuctioningItemsCount");
        _fStatementGetMyAuctioningItemsCount.Sql = SqliteAuctionDbStatements.Auction_GetMyAuctioningItemsCount;

        // 我流拍未取的物品数量
        _fStatementGetMySellFailItemsCount = fdb.AddSQLStatement("Auction_GetMySellFailItemsCount");
        _fStatementGetMySellFailItemsCount.Sql = SqliteAuctionDbStatements.Auction_GetMySellFailItemsCount;

        // 我拍买未取的物品数量
        _fStatementGetMyBuyOKItemsCount = fdb.AddSQLStatement("Auction_GetMyBuyOKItemsCount");
        _fStatementGetMyBuyOKItemsCount.Sql = SqliteAuctionDbStatements.Auction_GetMyBuyOKItemsCount;

        _fStatementGetMaxAuctionID = fdb.AddSQLStatement("Auction_GetMaxAuctionID");
        _fStatementGetMaxAuctionID.Sql = SqliteAuctionDbStatements.Auction_GetMaxAuctionID;

        // 添加拍卖物品
        _fStatementInsertAuctionItem = fdb.AddSQLStatement("Auction_InsertAuctionItem");
        _fStatementInsertAuctionItem.Sql = SqliteAuctionDbStatements.Auction_InsertAuctionItem;

        // 参与物品竞价
        _fStatementJoinItemBid = fdb.AddSQLStatement("Auction_JoinItemBid");
        _fStatementJoinItemBid.Sql = SqliteAuctionDbStatements.Auction_JoinItemBid;

        // 一口价购买物品
        _fStatementBuyItem = fdb.AddSQLStatement("Auction_BuyItem");
        _fStatementBuyItem.Sql = SqliteAuctionDbStatements.Auction_BuyItem;

        // 添加关注物品
        _fStatementInsertAttentionItem = fdb.AddSQLStatement("Auction_AddAttentionItem");
        _fStatementInsertAttentionItem.Sql = SqliteAuctionDbStatements.Auction_AddAttentionItem;
        _fStatementCheckInAttentionItem = fdb.AddSQLStatement("Auction_CheckInAttentionItem");
        _fStatementCheckInAttentionItem.Sql = SqliteAuctionDbStatements.Auction_CheckInAttentionItem;

        // 删除关注物品
        _fStatementDeleteAttentionItem = fdb.AddSQLStatement("Auction_DeleteAuctionItem");
        _fStatementDeleteAttentionItem.Sql = SqliteAuctionDbStatements.Auction_DeleteAuctionItem;

        _fStatementAuctionDataHumanRename = fdb.AddSQLStatement("Auction_HumanRename");
        _fStatementAuctionDataHumanRename.Sql = SqliteAuctionDbStatements.Auction_HumanRename;

        _fStatementAuctionDataLastBidderRename = fdb.AddSQLStatement("Auction_LastBidderRename");
        _fStatementAuctionDataLastBidderRename.Sql = SqliteAuctionDbStatements.Auction_LastBidderRename;

        _fStatementAuctionAttentionRename = fdb.AddSQLStatement("Auction_AttentionRename");
        _fStatementAuctionAttentionRename.Sql = SqliteAuctionDbStatements.Auction_AttentionRename;

        try
        {
            _fStatementUpdateAuctionItemFail.Prepare();          // 更新拍卖物品的状态
            _fStatementUpdateAuctionItemSuccess.Prepare();       // 更新拍卖物品的状态
            _fStatementQueryAuctionItemSuccess.Prepare();

            _fStatementQueryMyItems.Prepare();                   // 查询我的拍卖物品
            _fStatementGetMyItemsCount.Prepare();                // 我正在拍卖物品总页数

            _fStatementQueryMyAttentionItems.Prepare();          // 查询关注物品
            _fStatementQueryMyAttentionItemsCount.Prepare();     // 我的关注物品总页数
            _fStatementQueryOneItem.Prepare();                   // 查询单个物品数量

            _fStatementGetMyAuctioningItemsCount.Prepare();      // 我正在拍卖物品的数量
            _fStatementGetMySellFailItemsCount.Prepare();        // 我流拍未取的物品数量
            _fStatementGetMyBuyOKItemsCount.Prepare();

            _fStatementGetMaxAuctionID.Prepare();
            _fStatementInsertAuctionItem.Prepare();
            _fStatementInsertAttentionItem.Prepare();
            _fStatementCheckInAttentionItem.Prepare();
            _fStatementDeleteAttentionItem.Prepare();

            _fStatementJoinItemBid.Prepare();                    // 参与物品竞价
            _fStatementBuyItem.Prepare();                        // 一口价购买物品

            _fStatementAuctionDataHumanRename.Prepare();
            _fStatementAuctionDataLastBidderRename.Prepare();
            _fStatementAuctionAttentionRename.Prepare();
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
    }

    /// <summary>SqliteAuctionDB.pas:337-465 <c>DoFinal</c>：逐个 Finalize 并置 nil（顺序与 DoInit 相同）。</summary>
    public override void DoFinal()
    {
        // 原文 339 行 inherited;（基类 DoFinal 为 abstract，无实现体）

        if (_fStatementUpdateAuctionItemFail != null)        // 更新拍卖物品的状态
        {
            _fStatementUpdateAuctionItemFail.StatementFinalize();
            _fStatementUpdateAuctionItemFail = null;
        }

        if (_fStatementUpdateAuctionItemSuccess != null)     // 更新拍卖物品的状态
        {
            _fStatementUpdateAuctionItemSuccess.StatementFinalize();
            _fStatementUpdateAuctionItemSuccess = null;
        }

        if (_fStatementQueryAuctionItemSuccess != null)
        {
            _fStatementQueryAuctionItemSuccess.StatementFinalize();
            _fStatementQueryAuctionItemSuccess = null;
        }

        if (_fStatementQueryMyItems != null)                 // 查询我的拍卖物品
        {
            _fStatementQueryMyItems.StatementFinalize();
            _fStatementQueryMyItems = null;
        }

        if (_fStatementGetMyItemsCount != null)              // 我正在拍卖物品总页数
        {
            _fStatementGetMyItemsCount.StatementFinalize();
            _fStatementGetMyItemsCount = null;
        }

        if (_fStatementQueryMyAttentionItems != null)        // 查询关注物品
        {
            _fStatementQueryMyAttentionItems.StatementFinalize();
            _fStatementQueryMyAttentionItems = null;
        }

        if (_fStatementQueryMyAttentionItemsCount != null)   // 我的关注物品总页数
        {
            _fStatementQueryMyAttentionItemsCount.StatementFinalize();
            _fStatementQueryMyAttentionItemsCount = null;
        }

        if (_fStatementQueryOneItem != null)                 // 查询单个物品数量
        {
            _fStatementQueryOneItem.StatementFinalize();
            _fStatementQueryOneItem = null;
        }

        if (_fStatementGetMyAuctioningItemsCount != null)    // 我正在拍卖物品的数量
        {
            _fStatementGetMyAuctioningItemsCount.StatementFinalize();
            _fStatementGetMyAuctioningItemsCount = null;
        }

        if (_fStatementGetMySellFailItemsCount != null)      // 我流拍未取的物品数量
        {
            _fStatementGetMySellFailItemsCount.StatementFinalize();
            _fStatementGetMySellFailItemsCount = null;
        }

        if (_fStatementGetMyBuyOKItemsCount != null)         // 我拍买未取的物品数量
        {
            _fStatementGetMyBuyOKItemsCount.StatementFinalize();
            _fStatementGetMyBuyOKItemsCount = null;
        }

        if (_fStatementGetMaxAuctionID != null)
        {
            _fStatementGetMaxAuctionID.StatementFinalize();
            _fStatementGetMaxAuctionID = null;
        }

        if (_fStatementInsertAuctionItem != null)
        {
            _fStatementInsertAuctionItem.StatementFinalize();
            _fStatementInsertAuctionItem = null;
        }

        if (_fStatementInsertAttentionItem != null)
        {
            _fStatementInsertAttentionItem.StatementFinalize();
            _fStatementInsertAttentionItem = null;
        }

        if (_fStatementCheckInAttentionItem != null)
        {
            _fStatementCheckInAttentionItem.StatementFinalize();
            _fStatementCheckInAttentionItem = null;
        }

        if (_fStatementDeleteAttentionItem != null)
        {
            _fStatementDeleteAttentionItem.StatementFinalize();
            _fStatementDeleteAttentionItem = null;
        }

        if (_fStatementJoinItemBid != null)                  // 参与物品竞价
        {
            _fStatementJoinItemBid.StatementFinalize();
            _fStatementJoinItemBid = null;
        }

        if (_fStatementBuyItem != null)                      // 一口价购买物品
        {
            _fStatementBuyItem.StatementFinalize();
            _fStatementBuyItem = null;
        }

        if (_fStatementAuctionDataHumanRename != null)
        {
            _fStatementAuctionDataHumanRename.StatementFinalize();
            _fStatementAuctionDataHumanRename = null;
        }

        if (_fStatementAuctionDataLastBidderRename != null)
        {
            _fStatementAuctionDataLastBidderRename.StatementFinalize();
            _fStatementAuctionDataLastBidderRename = null;
        }

        if (_fStatementAuctionAttentionRename != null)
        {
            _fStatementAuctionAttentionRename.StatementFinalize();
            _fStatementAuctionAttentionRename = null;
        }
    }

    /// <summary>SqliteAuctionDB.pas:467-592 <c>DoAddAuctionItem</c>。</summary>
    protected override int DoAddAuctionItem(string humanName, int auctionTime, int startingPrice, int sellingPrice,
        int currencyType, TUserItem userItem, object? stdItem)
    {
        int result = 0;
        int auctionId;
        TItemGroup itemGroup;
        string changeName;

        // 原文 468 的形参 StdItem: PTStdItem —— 按 IAuctionStdItem 接缝解引用（原文全程直接 StdItem.Xxx）。
        var std = (IAuctionStdItem)stdItem!;

        try
        {
            _fStatementGetMaxAuctionID!.Reset();
            if (_fStatementGetMaxAuctionID!.Step() == SqliteCodes.SQLITE_ROW)
                auctionId = _fStatementGetMaxAuctionID!.OrderGetColumnValueInt;
            else
                auctionId = 1;
        }
        finally
        {
            _fStatementGetMaxAuctionID!.Reset();
        }

        itemGroup = TItemGroup.igOther;
        if (std.StdMode == 10 || std.StdMode == 11)                 // 衣服
            itemGroup = TItemGroup.igDress;
        else if (std.StdMode == 5 || std.StdMode == 6)              // 武器
            itemGroup = TItemGroup.igWeapon;
        else if (std.StdMode == 28 || std.StdMode == 30)            // 照明物
            itemGroup = TItemGroup.igSpecial;
        else if (std.StdMode == 19 || std.StdMode == 20 || std.StdMode == 21)   // 项链
        {
            if (std.OverLap == 2 || std.OverLap == 4 || std.OverLap == 6)
                itemGroup = TItemGroup.igSpecial;
            else
                itemGroup = TItemGroup.igNecklace;
        }
        else if (std.StdMode == 15 || std.StdMode == 78)            // 头盔
        {
            if (std.StdMode == 15 && (std.OverLap == 2 || std.OverLap == 4 || std.OverLap == 6))
                itemGroup = TItemGroup.igSpecial;
            else
                itemGroup = TItemGroup.igHelmet;
        }
        else if (std.StdMode == 24 || std.StdMode == 26)            // 手镯
        {
            if (std.OverLap == 2 || std.OverLap == 4 || std.OverLap == 6)
                itemGroup = TItemGroup.igSpecial;
            else
                itemGroup = TItemGroup.igArmRing;
        }
        else if (std.StdMode == 22 || std.StdMode == 23)            // 戒指
        {
            if (std.OverLap == 2 || std.OverLap == 4 || std.OverLap == 6)
                itemGroup = TItemGroup.igSpecial;
            else
                itemGroup = TItemGroup.igRing;
        }
        else if (std.StdMode == 25 || std.StdMode == 51)            // 符毒
            itemGroup = TItemGroup.igSpecial;
        else if (std.StdMode == 54 || std.StdMode == 64)            // 腰带
            itemGroup = TItemGroup.igBelt;
        else if (std.StdMode == 52 || std.StdMode == 62)            // 靴子
            itemGroup = TItemGroup.igBoots;
        else if (std.StdMode == 53 || std.StdMode == 63 || std.StdMode == 7)    // 宝石
            itemGroup = TItemGroup.igSpecial;
        else if (std.StdMode >= 66 && std.StdMode <= 89)            // 时装（原文 StdMode in [66..89]）
            itemGroup = TItemGroup.igFashion;
        else if (std.StdMode == 16)                                 // 斗笠
            itemGroup = TItemGroup.igSpecial;
        else if (std.StdMode == 65)                                 // 军鼓
            itemGroup = TItemGroup.igSpecial;
        // 原文如此（SqliteAuctionDB.pas:534）：StdMode = 28（马牌）死代码 —— 已在上面“照明物”分支吃掉
        else if (std.StdMode == 28)                                 // 马牌
            itemGroup = TItemGroup.igSpecial;
        else if (std.StdMode == 12)                                 // 盾牌
            itemGroup = TItemGroup.igSpecial;
        else if (std.StdMode == 90)                                 // 灵玉
            itemGroup = TItemGroup.igSpecial;
        else if (std.StdMode == 0 || std.StdMode == 1
            || (std.StdMode == 3 && std.Shape == 12))               // 药品
            itemGroup = TItemGroup.igDrug;
        else if (std.StdMode == 4)                                  // 技能书籍
            itemGroup = TItemGroup.igSpecial;

        if (auctionId > 0)
        {
            _fdb!.BeginTransaction();
            try
            {
                try
                {
                    _fStatementInsertAuctionItem!.Reset();
                    _fStatementInsertAuctionItem!.OrderBindInt(auctionId);
                    _fStatementInsertAuctionItem!.OrderBindText(humanName);
                    // 原文 553：OrderBindInt(Integer(ItemGroup)) —— TItemGroup 序数直传
                    _fStatementInsertAuctionItem!.OrderBindInt((int)itemGroup);

                    // 原文 556：UserItem.btColor（Byte）→ OrderBindInt
                    if (userItem.btColor > 0)
                        _fStatementInsertAuctionItem!.OrderBindInt(userItem.btColor);
                    else
                        _fStatementInsertAuctionItem!.OrderBindInt(std.Color);

                    _fStatementInsertAuctionItem!.OrderBindInt(auctionTime);
                    _fStatementInsertAuctionItem!.OrderBindInt(startingPrice);
                    _fStatementInsertAuctionItem!.OrderBindInt(sellingPrice);
                    _fStatementInsertAuctionItem!.OrderBindInt(currencyType);

                    _fStatementInsertAuctionItem!.OrderBindText(std.DBName);

                    changeName = "";
                    // 原文 568：UserItem.btValue[13]（不是 btValue[0]）判据 + Length(UserItem.Name) > 0
                    if (M2ItemDbAccess.GetValue(ref userItem, 13) == 1 && userItem.NameStr.Length > 0)
                        changeName = ProcessItemName(userItem.NameStr);

                    _fStatementInsertAuctionItem!.OrderBindText(changeName);

                    int stepCode = _fStatementInsertAuctionItem!.Step();
                    if (stepCode == SqliteCodes.SQLITE_OK || stepCode == SqliteCodes.SQLITE_DONE)
                    {
                        Owner.SaveItemToDB(userItem, auctionId, DataItemTypes.AuctionItemType, 0);

                        result = auctionId;
                    }
                }
                finally
                {
                    _fStatementInsertAuctionItem!.Reset();
                }

                _fdb!.Commit();
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage(e.Message);
                _fdb!.RollBack();
            }
        }

        return result;
    }

    /// <summary>SqliteAuctionDB.pas:594-608 <c>DoCancelAuctionItem</c>（取消拍卖物品）。</summary>
    protected override bool DoCancelAuctionItem(string humanName, int auctionId)
    {
        bool result = false;
        try
        {
            _fdb!.Execute("update AuctionData set TradingStatus = 1 where AuctionID = " + IntToStr(auctionId)
                + " and HumanName = \"" + humanName + "\";");
            result = true;
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>SqliteAuctionDB.pas:610-623 <c>DoRetrieveAuctionItem</c>（取回拍卖物品）。</summary>
    protected override bool DoRetrieveAuctionItem(int auctionId)
    {
        bool result = false;
        try
        {
            _fdb!.Execute("update AuctionData set IsItemGive = 1 where AuctionID = " + IntToStr(auctionId) + ";");
            result = true;
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>SqliteAuctionDB.pas:625-653 <c>DoDeleteAuctionItem</c>（多语句脚本，sLineBreak 分隔）。</summary>
    protected override bool DoDeleteAuctionItem(string humanName, int auctionId)
    {
        bool result = false;
        string sWhere = string.Format(" WHERE ParentID = {0} and ItemType = " + IntToStr(DataItemTypes.AuctionItemType)
            + " and ItemIndex = 0;", auctionId);

        _fdb!.BeginTransaction();
        try
        {
            string sql = SqliteAuctionDbScripts.DoDeleteAuctionItem_L635_Sql_P0 + sWhere
                + SqliteAuctionDbScripts.DoDeleteAuctionItem_L635_Sql_P2 + sWhere
                + SqliteAuctionDbScripts.DoDeleteAuctionItem_L635_Sql_P4 + sWhere
                + SqliteAuctionDbScripts.DoDeleteAuctionItem_L635_Sql_P6 + sWhere
                + SqliteAuctionDbScripts.DoDeleteAuctionItem_L635_Sql_P8 + sWhere
                + SqliteAuctionDbScripts.DoDeleteAuctionItem_L635_Sql_P10 + sWhere
                + SqliteAuctionDbScripts.DoDeleteAuctionItem_L635_Sql_P12 + sWhere
                + SqliteAuctionDbScripts.DoDeleteAuctionItem_L635_Sql_P14 + sWhere
                + SqliteAuctionDbScripts.DoDeleteAuctionItem_L635_Sql_P16 + sWhere
                + SqliteAuctionDbScripts.DoDeleteAuctionItem_L635_Sql_P18 + IntToStr(auctionId)
                + SqliteAuctionDbScripts.DoDeleteAuctionItem_L635_Sql_P20 + IntToStr(auctionId)
                + SqliteAuctionDbScripts.DoDeleteAuctionItem_L635_Sql_P22;

            _fdb!.Execute(sql);
            _fdb!.Commit();

            result = true;
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
            _fdb!.RollBack();
        }
        return result;
    }

    /// <summary>SqliteAuctionDB.pas:655-687 <c>DoAddAttentionItem</c>（添加关注物品）。</summary>
    protected override bool DoAddAttentionItem(string humanName, int index)
    {
        bool result = false;
        try
        {
            bool isAttentioned;
            try
            {
                _fStatementCheckInAttentionItem!.Reset();
                _fStatementCheckInAttentionItem!.OrderBindText(humanName);
                _fStatementCheckInAttentionItem!.OrderBindInt(index);
                isAttentioned = _fStatementCheckInAttentionItem!.Step() == SqliteCodes.SQLITE_ROW;
            }
            finally
            {
                _fStatementCheckInAttentionItem!.Reset();
            }

            if (!isAttentioned)
            {
                try
                {
                    _fStatementInsertAttentionItem!.Reset();
                    _fStatementInsertAttentionItem!.OrderBindText(humanName);
                    _fStatementInsertAttentionItem!.OrderBindInt(index);
                    int stepCode = _fStatementInsertAttentionItem!.Step();
                    result = stepCode == SqliteCodes.SQLITE_OK || stepCode == SqliteCodes.SQLITE_DONE;
                }
                finally
                {
                    _fStatementInsertAttentionItem!.Reset();
                }
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>SqliteAuctionDB.pas:689-721 <c>DoDeleteAttentionItem</c>（删除关注物品）。</summary>
    protected override bool DoDeleteAttentionItem(string humanName, int index)
    {
        bool result = false;
        try
        {
            bool isAttentioned;
            try
            {
                _fStatementCheckInAttentionItem!.Reset();
                _fStatementCheckInAttentionItem!.OrderBindText(humanName);
                _fStatementCheckInAttentionItem!.OrderBindInt(index);
                isAttentioned = _fStatementCheckInAttentionItem!.Step() == SqliteCodes.SQLITE_ROW;
            }
            finally
            {
                _fStatementCheckInAttentionItem!.Reset();
            }

            if (isAttentioned)
            {
                try
                {
                    _fStatementDeleteAttentionItem!.Reset();
                    _fStatementDeleteAttentionItem!.OrderBindText(humanName);
                    _fStatementDeleteAttentionItem!.OrderBindInt(index);
                    int stepCode = _fStatementDeleteAttentionItem!.Step();
                    result = stepCode == SqliteCodes.SQLITE_OK || stepCode == SqliteCodes.SQLITE_DONE;
                }
                finally
                {
                    _fStatementDeleteAttentionItem!.Reset();
                }
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>SqliteAuctionDB.pas:723-759 <c>DoJoinItemBid</c>（参加物品竞价；原文此处走基类
    /// AddAttentionItem 包装层，故**带锁**，与 DoAddAttentionItem 不同）。</summary>
    protected override bool DoJoinItemBid(string humanName, int index, int prices, bool isSell)
    {
        bool result = false;
        try
        {
            AddAttentionItem(humanName, index);

            if (isSell)
            {
                try
                {
                    _fStatementBuyItem!.Reset();
                    _fStatementBuyItem!.OrderBindText(humanName);
                    _fStatementBuyItem!.OrderBindInt(prices);
                    _fStatementBuyItem!.OrderBindInt(index);
                    int stepCode = _fStatementBuyItem!.Step();
                    result = stepCode == SqliteCodes.SQLITE_OK || stepCode == SqliteCodes.SQLITE_DONE;
                }
                finally
                {
                    _fStatementBuyItem!.Reset();
                }
            }
            else
            {
                try
                {
                    _fStatementJoinItemBid!.Reset();
                    _fStatementJoinItemBid!.OrderBindText(humanName);
                    _fStatementJoinItemBid!.OrderBindInt(prices);
                    _fStatementJoinItemBid!.OrderBindInt(index);
                    int stepCode = _fStatementJoinItemBid!.Step();
                    result = stepCode == SqliteCodes.SQLITE_OK || stepCode == SqliteCodes.SQLITE_DONE;
                }
                finally
                {
                    _fStatementJoinItemBid!.Reset();
                }
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>SqliteAuctionDB.pas:761-914 <c>DoQueryAllItems</c>（查询可竞拍的商品列表；动态拼 SQL）。</summary>
    protected override int DoQueryAllItems(string itemName, int itemGroup, string humanName, int nPage,
        int topmostAuctionId, int itemColors, int sortField, bool sortAsc, int moneyType, uint minPrices, uint maxPrices,
        TAuctionItemList itemList)
    {
        int result = 0;
        try
        {
            // 查询可竞拍的商品列表
            ISqliteStatement sm = _fdb!.AddSQLStatement("Auction_QueryAllItems");
            try
            {
                sm.Sql = SqliteAuctionDbScripts.DoQueryAllItems_L775_sm_Sql_P0 + IntToStr(topmostAuctionId)
                    + SqliteAuctionDbScripts.DoQueryAllItems_L775_sm_Sql_P2;

                if (itemGroup != (int)TItemGroup.igAll)
                {
                    sm.Sql = sm.Sql + SqliteAuctionDbScripts.DoQueryAllItems_L785_sm_Sql_P2
                        + IntToStr(itemGroup) + SqliteAuctionDbScripts.DoQueryAllItems_L785_sm_Sql_P4;
                }

                if (itemColors != 0)
                {
                    string sColors = AuctionColorsSql(itemColors);

                    if (sColors.Length > 0)
                    {
                        sm.Sql = sm.Sql + SqliteAuctionDbScripts.DoQueryAllItems_L822_sm_Sql_P4
                            + sColors.Substring(0, sColors.Length - 1) + SqliteAuctionDbScripts.DoQueryAllItems_L822_sm_Sql_P6;
                    }
                }

                if (moneyType > 0)
                {
                    sm.Sql = sm.Sql + SqliteAuctionDbScripts.DoQueryAllItems_L828_sm_Sql_P6
                        + IntToStr(moneyType - 1);
                }

                if (minPrices > 0)
                {
                    sm.Sql = sm.Sql + SqliteAuctionDbScripts.DoQueryAllItems_L833_sm_Sql_P8 + IntToStr((int)minPrices);
                }

                if (maxPrices > 0)
                {
                    sm.Sql = sm.Sql + SqliteAuctionDbScripts.DoQueryAllItems_L838_sm_Sql_P10 + IntToStr((int)maxPrices);
                }

                if (itemName.Length > 0)
                {
                    sm.Sql = sm.Sql + SqliteAuctionDbScripts.DoQueryAllItems_L843_sm_Sql_P12 + itemName
                        + SqliteAuctionDbScripts.DoQueryAllItems_L843_sm_Sql_P14 + itemName
                        + SqliteAuctionDbScripts.DoQueryAllItems_L843_sm_Sql_P16;
                }

                // 原文 768 行 var sOrderBy: string（未初始化，默认空串）；
                // 原文 SortField 不在 0..3 时 sOrderBy 保持空串（原文如此）。
                string sOrderBy = "";
                if (sortField == 1)
                {
                    if (sortAsc)
                        sOrderBy = " order by ifnull(A.LastBidPrice, A.StartingPrice)";
                    else
                        sOrderBy = " order by ifnull(A.LastBidPrice, A.StartingPrice) desc";
                }
                else if (sortField == 2)
                {
                    if (sortAsc)
                        sOrderBy = " order by A.SellingPrice";
                    else
                        sOrderBy = " order by A.SellingPrice desc";
                }
                else if (sortField == 3)
                {
                    if (sortAsc)
                        sOrderBy = " order by (A.AddDateTime + A.AuctionTime * 3600) - strftime(\"%s\", \"now\")";
                    else
                        sOrderBy = " order by (A.AddDateTime + A.AuctionTime * 3600) - strftime(\"%s\", \"now\") desc";
                }
                else if (sortField == 0)
                {
                    sOrderBy = SqliteAuctionDbScripts.DoQueryAllItems_L869_sOrderBy;
                }

                sm.Sql = sm.Sql + sOrderBy + " limit ? offset ?;";

                sm.Prepare();

                sm.OrderBindText(humanName);
                sm.OrderBindInt(Grobal2Const.AUCTION_PAGE_COUNT);
                sm.OrderBindInt((nPage - 1) * Grobal2Const.AUCTION_PAGE_COUNT);
                int ret = sm.Step();

                while (ret == SqliteCodes.SQLITE_ROW)
                {
                    var auctionRecord = new TAuctionRecord();
                    auctionRecord.AuctionID = sm.OrderGetColumnValueInt;
                    auctionRecord.HumanName = sm.OrderGetColumnValueText;
                    auctionRecord.AddDateTime = AuctionAddDateTime(sm.OrderGetColumnValueInt);
                    auctionRecord.AuctionTime = sm.OrderGetColumnValueInt;
                    auctionRecord.TimeLeft = sm.OrderGetColumnValueInt;
                    auctionRecord.StartingPrice = (uint)sm.OrderGetColumnValueInt;
                    auctionRecord.SellingPrice = (uint)sm.OrderGetColumnValueInt;
                    auctionRecord.CurrencyType = sm.OrderGetColumnValueInt;
                    auctionRecord.LastBidPrice = sm.OrderGetColumnValueInt;
                    auctionRecord.LastBidder = sm.OrderGetColumnValueText;
                    auctionRecord.TradingStatus = sm.OrderGetColumnValueInt;
                    auctionRecord.IsItemGive = sm.OrderGetColumnValueBool;
                    auctionRecord.IsAttention = sm.OrderGetColumnValueBool;

                    Owner.LoadItemFromDB(ref auctionRecord.ActionItem, auctionRecord.AuctionID,
                        DataItemTypes.AuctionItemType, 0);

                    itemList.Add(auctionRecord);

                    ret = sm.Step();
                    result++;
                }
            }
            finally
            {
                sm.Reset();
                sm.StatementFinalize();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>SqliteAuctionDB.pas:916-962 <c>DoQueryMyItems</c>（查询我的拍卖物品）。</summary>
    protected override int DoQueryMyItems(string humanName, int nPage, TAuctionItemList itemList)
    {
        int result = 0;
        try
        {
            try
            {
                _fStatementQueryMyItems!.Reset();
                _fStatementQueryMyItems!.OrderBindText(humanName);
                _fStatementQueryMyItems!.OrderBindInt(Grobal2Const.AUCTION_PAGE_COUNT);
                _fStatementQueryMyItems!.OrderBindInt((nPage - 1) * Grobal2Const.AUCTION_PAGE_COUNT);
                int ret = _fStatementQueryMyItems!.Step();

                while (ret == SqliteCodes.SQLITE_ROW)
                {
                    var auctionRecord = new TAuctionRecord();
                    auctionRecord.AuctionID = _fStatementQueryMyItems!.OrderGetColumnValueInt;
                    auctionRecord.HumanName = _fStatementQueryMyItems!.OrderGetColumnValueText;
                    auctionRecord.AddDateTime = AuctionAddDateTime(_fStatementQueryMyItems!.OrderGetColumnValueInt);
                    auctionRecord.AuctionTime = _fStatementQueryMyItems!.OrderGetColumnValueInt;
                    auctionRecord.TimeLeft = _fStatementQueryMyItems!.OrderGetColumnValueInt;
                    auctionRecord.StartingPrice = (uint)_fStatementQueryMyItems!.OrderGetColumnValueInt;
                    auctionRecord.SellingPrice = (uint)_fStatementQueryMyItems!.OrderGetColumnValueInt;
                    auctionRecord.CurrencyType = _fStatementQueryMyItems!.OrderGetColumnValueInt;
                    auctionRecord.LastBidPrice = _fStatementQueryMyItems!.OrderGetColumnValueInt;
                    auctionRecord.LastBidder = _fStatementQueryMyItems!.OrderGetColumnValueText;
                    auctionRecord.TradingStatus = _fStatementQueryMyItems!.OrderGetColumnValueInt;
                    auctionRecord.IsItemGive = _fStatementQueryMyItems!.OrderGetColumnValueBool;

                    Owner.LoadItemFromDB(ref auctionRecord.ActionItem, auctionRecord.AuctionID,
                        DataItemTypes.AuctionItemType, 0);

                    itemList.Add(auctionRecord);

                    ret = _fStatementQueryMyItems!.Step();
                    result++;
                }
            }
            finally
            {
                _fStatementQueryMyItems!.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>SqliteAuctionDB.pas:964-1017 <c>DoQueryMyAttentionItems</c>（查询关注物品；
    /// 每行再用 FStatementQueryOneItem 单独查一条；finally 里两个 Reset 的顺序与原文一致）。</summary>
    protected override int DoQueryMyAttentionItems(string humanName, int nPage, TAuctionItemList itemList)
    {
        int result = 0;
        try
        {
            try
            {
                _fStatementQueryMyAttentionItems!.Reset();
                _fStatementQueryMyAttentionItems!.OrderBindText(humanName);
                _fStatementQueryMyAttentionItems!.OrderBindInt(Grobal2Const.AUCTION_PAGE_COUNT);
                _fStatementQueryMyAttentionItems!.OrderBindInt((nPage - 1) * Grobal2Const.AUCTION_PAGE_COUNT);
                int ret = _fStatementQueryMyAttentionItems!.Step();

                while (ret == SqliteCodes.SQLITE_ROW)
                {
                    var auctionRecord = new TAuctionRecord();
                    auctionRecord.AuctionID = _fStatementQueryMyAttentionItems!.OrderGetColumnValueInt;

                    _fStatementQueryOneItem!.Reset();
                    _fStatementQueryOneItem!.OrderBindInt(auctionRecord.AuctionID);
                    if (_fStatementQueryOneItem!.Step() == SqliteCodes.SQLITE_ROW)
                    {
                        auctionRecord.HumanName = _fStatementQueryOneItem!.OrderGetColumnValueText;
                        auctionRecord.AddDateTime = AuctionAddDateTime(_fStatementQueryOneItem!.OrderGetColumnValueInt);
                        auctionRecord.AuctionTime = _fStatementQueryOneItem!.OrderGetColumnValueInt;
                        auctionRecord.TimeLeft = _fStatementQueryOneItem!.OrderGetColumnValueInt;
                        auctionRecord.StartingPrice = (uint)_fStatementQueryOneItem!.OrderGetColumnValueInt;
                        auctionRecord.SellingPrice = (uint)_fStatementQueryOneItem!.OrderGetColumnValueInt;
                        auctionRecord.CurrencyType = _fStatementQueryOneItem!.OrderGetColumnValueInt;
                        auctionRecord.LastBidPrice = _fStatementQueryOneItem!.OrderGetColumnValueInt;
                        auctionRecord.LastBidder = _fStatementQueryOneItem!.OrderGetColumnValueText;
                        auctionRecord.TradingStatus = _fStatementQueryOneItem!.OrderGetColumnValueInt;
                        auctionRecord.IsItemGive = _fStatementQueryOneItem!.OrderGetColumnValueBool;
                    }

                    Owner.LoadItemFromDB(ref auctionRecord.ActionItem, auctionRecord.AuctionID,
                        DataItemTypes.AuctionItemType, 0);

                    itemList.Add(auctionRecord);

                    ret = _fStatementQueryMyAttentionItems!.Step();
                    result++;
                }
            }
            finally
            {
                _fStatementQueryOneItem!.Reset();
                _fStatementQueryMyAttentionItems!.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>SqliteAuctionDB.pas:1019-1052 <c>DoGetAuctionInfo</c>。</summary>
    protected override bool DoGetAuctionInfo(int index, out TAuctionInfo auctionInfo)
    {
        auctionInfo = new TAuctionInfo();
        bool result = false;
        try
        {
            try
            {
                _fStatementQueryOneItem!.Reset();
                _fStatementQueryOneItem!.OrderBindInt(index);
                if (_fStatementQueryOneItem!.Step() == SqliteCodes.SQLITE_ROW)
                {
                    auctionInfo.AuctionID = index;
                    auctionInfo.HumanName = _fStatementQueryOneItem!.OrderGetColumnValueText;
                    auctionInfo.AddDateTime = AuctionAddDateTime(_fStatementQueryOneItem!.OrderGetColumnValueInt);
                    auctionInfo.AuctionTime = _fStatementQueryOneItem!.OrderGetColumnValueInt;
                    auctionInfo.TimeLeft = _fStatementQueryOneItem!.OrderGetColumnValueInt;
                    auctionInfo.StartingPrice = (uint)_fStatementQueryOneItem!.OrderGetColumnValueInt;
                    auctionInfo.SellingPrice = (uint)_fStatementQueryOneItem!.OrderGetColumnValueInt;
                    auctionInfo.CurrencyType = _fStatementQueryOneItem!.OrderGetColumnValueInt;
                    auctionInfo.LastBidPrice = _fStatementQueryOneItem!.OrderGetColumnValueInt;
                    auctionInfo.LastBidder = _fStatementQueryOneItem!.OrderGetColumnValueText;
                    auctionInfo.TradingStatus = _fStatementQueryOneItem!.OrderGetColumnValueInt;
                    auctionInfo.IsItemGive = _fStatementQueryOneItem!.OrderGetColumnValueBool;

                    result = true;
                }
            }
            finally
            {
                _fStatementQueryOneItem!.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>SqliteAuctionDB.pas:1054-1090 <c>DoGetAuctionRecord</c>。</summary>
    protected override bool DoGetAuctionRecord(int index, out TAuctionRecord auctionRecord)
    {
        auctionRecord = new TAuctionRecord();
        bool result = false;
        try
        {
            try
            {
                _fStatementQueryOneItem!.Reset();
                _fStatementQueryOneItem!.OrderBindInt(index);
                if (_fStatementQueryOneItem!.Step() == SqliteCodes.SQLITE_ROW)
                {
                    auctionRecord.AuctionID = index;
                    auctionRecord.HumanName = _fStatementQueryOneItem!.OrderGetColumnValueText;
                    auctionRecord.AddDateTime = AuctionAddDateTime(_fStatementQueryOneItem!.OrderGetColumnValueInt);
                    auctionRecord.AuctionTime = _fStatementQueryOneItem!.OrderGetColumnValueInt;
                    auctionRecord.TimeLeft = _fStatementQueryOneItem!.OrderGetColumnValueInt;
                    auctionRecord.StartingPrice = (uint)_fStatementQueryOneItem!.OrderGetColumnValueInt;
                    auctionRecord.SellingPrice = (uint)_fStatementQueryOneItem!.OrderGetColumnValueInt;
                    auctionRecord.CurrencyType = _fStatementQueryOneItem!.OrderGetColumnValueInt;
                    auctionRecord.LastBidPrice = _fStatementQueryOneItem!.OrderGetColumnValueInt;
                    auctionRecord.LastBidder = _fStatementQueryOneItem!.OrderGetColumnValueText;
                    auctionRecord.TradingStatus = _fStatementQueryOneItem!.OrderGetColumnValueInt;
                    auctionRecord.IsItemGive = _fStatementQueryOneItem!.OrderGetColumnValueBool;
                    auctionRecord.IsAttention = false;

                    Owner.LoadItemFromDB(ref auctionRecord.ActionItem, auctionRecord.AuctionID,
                        DataItemTypes.AuctionItemType, 0);

                    result = true;
                }
            }
            finally
            {
                _fStatementQueryOneItem!.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>SqliteAuctionDB.pas:1092-1183 <c>DoGetAllItemsPageCount</c>（动态拼 SQL；这里语句
    /// 只在 finally 里 Finalize，**没有** AddSQLStatement 之外的 Reset，与原文一致）。</summary>
    protected override int DoGetAllItemsPageCount(string itemName, int itemGroup, int itemColors, int moneyType,
        uint minPrices, uint maxPrices)
    {
        int result = 0;
        try
        {
            ISqliteStatement sm = _fdb!.AddSQLStatement("Auction_GetAllItemsCount");
            try
            {
                // 查询所有拍卖物品总页数
                sm.Sql = SqliteAuctionDbScripts.DoGetAllItemsPageCount_L1103_sm_Sql;

                if (itemGroup != (int)TItemGroup.igAll)
                {
                    // 原文 1108：' and ItemGroup = ' + IntToStr(Integer(ItemGroup))
                    sm.Sql = sm.Sql + " and ItemGroup = " + IntToStr(itemGroup);
                }

                if (itemColors != 0)
                {
                    string sColors = AuctionColorsSql(itemColors);

                    if (sColors.Length > 0)
                    {
                        sm.Sql = sm.Sql + SqliteAuctionDbScripts.DoGetAllItemsPageCount_L1145_sm_Sql_P2
                            + sColors.Substring(0, sColors.Length - 1) + SqliteAuctionDbScripts.DoGetAllItemsPageCount_L1145_sm_Sql_P4;
                    }
                }

                if (moneyType > 0)
                {
                    sm.Sql = sm.Sql + SqliteAuctionDbScripts.DoGetAllItemsPageCount_L1151_sm_Sql_P4
                        + IntToStr(moneyType - 1);
                }

                if (minPrices > 0)
                {
                    sm.Sql = sm.Sql + SqliteAuctionDbScripts.DoGetAllItemsPageCount_L1156_sm_Sql_P6
                        + IntToStr((int)minPrices);
                }

                if (maxPrices > 0)
                {
                    sm.Sql = sm.Sql + SqliteAuctionDbScripts.DoGetAllItemsPageCount_L1161_sm_Sql_P8
                        + IntToStr((int)maxPrices);
                }

                if (itemName.Length > 0)
                {
                    sm.Sql = sm.Sql + SqliteAuctionDbScripts.DoGetAllItemsPageCount_L1166_sm_Sql_P10 + itemName
                        + SqliteAuctionDbScripts.DoGetAllItemsPageCount_L1166_sm_Sql_P12 + itemName
                        + SqliteAuctionDbScripts.DoGetAllItemsPageCount_L1166_sm_Sql_P14;
                }

                sm.Prepare();
                if (sm.Step() == SqliteCodes.SQLITE_ROW)
                {
                    result = (sm.OrderGetColumnValueInt + Grobal2Const.AUCTION_PAGE_COUNT - 1) / Grobal2Const.AUCTION_PAGE_COUNT;
                }
            }
            finally
            {
                sm.StatementFinalize();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>SqliteAuctionDB.pas:1185-1198 <c>DoGetMyItemsPageCount</c>（原文**无** except，仅 finally）。</summary>
    protected override int DoGetMyItemsPageCount(string humanName)
    {
        int result = 0;
        try
        {
            _fStatementGetMyItemsCount!.Reset();
            _fStatementGetMyItemsCount!.OrderBindText(humanName);
            if (_fStatementGetMyItemsCount!.Step() == SqliteCodes.SQLITE_ROW)
            {
                result = (_fStatementGetMyItemsCount!.OrderGetColumnValueInt + Grobal2Const.AUCTION_PAGE_COUNT - 1)
                    / Grobal2Const.AUCTION_PAGE_COUNT;
            }
        }
        finally
        {
            _fStatementGetMyItemsCount!.Reset();
        }
        return result;
    }

    /// <summary>SqliteAuctionDB.pas:1200-1220 <c>DoGetMyAttentionPageCount</c>。</summary>
    protected override int DoGetMyAttentionPageCount(string humanName)
    {
        int result = 0;
        try
        {
            try
            {
                _fStatementQueryMyAttentionItemsCount!.Reset();
                _fStatementQueryMyAttentionItemsCount!.OrderBindText(humanName);
                if (_fStatementQueryMyAttentionItemsCount!.Step() == SqliteCodes.SQLITE_ROW)
                {
                    result = (_fStatementQueryMyAttentionItemsCount!.OrderGetColumnValueInt + Grobal2Const.AUCTION_PAGE_COUNT - 1)
                        / Grobal2Const.AUCTION_PAGE_COUNT;
                }
            }
            finally
            {
                _fStatementQueryMyAttentionItemsCount!.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>SqliteAuctionDB.pas:1222-1242 <c>DoGetMyAuctioningItemsCount</c>。</summary>
    protected override int DoGetMyAuctioningItemsCount(string humanName)
    {
        int result = 0;
        try
        {
            try
            {
                _fStatementGetMyAuctioningItemsCount!.Reset();
                _fStatementGetMyAuctioningItemsCount!.OrderBindText(humanName);
                if (_fStatementGetMyAuctioningItemsCount!.Step() == SqliteCodes.SQLITE_ROW)
                {
                    result = _fStatementGetMyAuctioningItemsCount!.OrderGetColumnValueInt;
                }
            }
            finally
            {
                _fStatementGetMyAuctioningItemsCount!.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>SqliteAuctionDB.pas:1244-1265 <c>DoGetMySellFailItemsCount</c>（我流拍未取的物品数量）。</summary>
    protected override int DoGetMySellFailItemsCount(string humanName)
    {
        int result = 0;
        try
        {
            try
            {
                _fStatementGetMySellFailItemsCount!.Reset();
                _fStatementGetMySellFailItemsCount!.OrderBindText(humanName);
                if (_fStatementGetMySellFailItemsCount!.Step() == SqliteCodes.SQLITE_ROW)
                {
                    result = _fStatementGetMySellFailItemsCount!.OrderGetColumnValueInt;
                }
            }
            finally
            {
                _fStatementGetMySellFailItemsCount!.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>SqliteAuctionDB.pas:1267-1288 <c>DoGetMyBuyOKItemsCount</c>（我拍买到未取的物品数量）。</summary>
    protected override int DoGetMyBuyOKItemsCount(string humanName)
    {
        int result = 0;
        try
        {
            try
            {
                _fStatementGetMyBuyOKItemsCount!.Reset();
                _fStatementGetMyBuyOKItemsCount!.OrderBindText(humanName);
                if (_fStatementGetMyBuyOKItemsCount!.Step() == SqliteCodes.SQLITE_ROW)
                {
                    result = _fStatementGetMyBuyOKItemsCount!.OrderGetColumnValueInt;
                }
            }
            finally
            {
                _fStatementGetMyBuyOKItemsCount!.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>SqliteAuctionDB.pas:1290-1562 <c>DoRun</c>（拍卖到期结算；内嵌过程
    /// IncPlayerGameMoney 见 1292-1411）。</summary>
    protected override void DoRun()
    {
        // 原文 1292-1411 的内嵌过程 IncPlayerGameMoney（嵌套在 DoRun 里，故此处同层收成 private 方法）。
        try
        {
            try
            {
                _fStatementUpdateAuctionItemFail!.Reset();
                _fStatementUpdateAuctionItemFail!.Step();
            }
            finally
            {
                _fStatementUpdateAuctionItemFail!.Reset();
            }

            try
            {
                _fStatementQueryAuctionItemSuccess!.Reset();
                int ret = _fStatementQueryAuctionItemSuccess!.Step();
                while (ret == SqliteCodes.SQLITE_ROW)
                {
                    int auctionId = _fStatementQueryAuctionItemSuccess!.OrderGetColumnValueInt;
                    string humanName = _fStatementQueryAuctionItemSuccess!.OrderGetColumnValueText;
                    int currencyType = _fStatementQueryAuctionItemSuccess!.OrderGetColumnValueInt;
                    int startingPrice = _fStatementQueryAuctionItemSuccess!.OrderGetColumnValueInt;
                    int sellingPrice = _fStatementQueryAuctionItemSuccess!.OrderGetColumnValueInt;
                    string lastBidder = _fStatementQueryAuctionItemSuccess!.OrderGetColumnValueText;
                    int lastBidPrice = _fStatementQueryAuctionItemSuccess!.OrderGetColumnValueInt;
                    int dbIndex = _fStatementQueryAuctionItemSuccess!.OrderGetColumnValueInt;
                    int makeIndex = _fStatementQueryAuctionItemSuccess!.OrderGetColumnValueInt;

                    int nAuctionTaxRate = 0;

                    switch (currencyType)
                    {
                        case 0:
                            nAuctionTaxRate = (int)DbLayerGlobals.Environment.dwAuctionGameGoldTaxRate;       // 元宝
                            break;
                        case 1:
                            nAuctionTaxRate = (int)DbLayerGlobals.Environment.dwAuctionGamePointTaxRate;      // 游戏点
                            break;
                        case 2:
                            nAuctionTaxRate = (int)DbLayerGlobals.Environment.dwAuctionGoldTaxRate;           // 金币
                            break;
                        case 3:
                            nAuctionTaxRate = (int)DbLayerGlobals.Environment.dwAuctionGameDiamondTaxRate;    // 金刚石
                            break;
                        case 4:
                            nAuctionTaxRate = (int)DbLayerGlobals.Environment.dwAuctionGameGirdTaxRate;       // 灵符
                            break;
                    }

                    IAuctionStdItem? stdItem = AuctionDbRunSeam.GetStdItem(dbIndex);

                    string itemName;
                    if (stdItem != null)
                        // 已按接缝 IAuctionStdItem.Name 订正（M2DataDbSupport.cs:240）——原文 1462 StdItem.Name（显示名）。
                        itemName = stdItem.Name;
                    else
                        itemName = "";

                    // 加拍卖者的钱
                    IncPlayerGameMoney(humanName, makeIndex, currencyType,
                        lastBidPrice - (int)Math.Round(lastBidPrice / 100.0 * nAuctionTaxRate),
                        "卖出物品: " + itemName);
                    try
                    {
                        // 已按接缝 IAuctionStdItem.NeedIdentify 订正（M2DataDbSupport.cs:244）——原文 1470 StdItem.NeedIdentify = 1。
                        if (stdItem != null && stdItem.NeedIdentify == 1)
                        {
                            AddGameDataLog(AuctionLogTypes.LOG_ItemSell, AuctionLogTypes.LOG_ActionNone, AuctionLogTypes.latHuman,
                                "0", 0, 0, stdItem.Name, makeIndex, humanName, "拍卖行-到期", 0, 0, "买入:" + lastBidder);
                            AddGameDataLog(AuctionLogTypes.LOG_ItemBuy, AuctionLogTypes.LOG_ActionNone, AuctionLogTypes.latHuman,
                                "0", 0, 0, stdItem.Name, makeIndex, lastBidder, "拍卖行-到期", 0, 0, "待取回, 卖出:" + humanName);
                        }

                        IAuctionPlayer? player = AuctionDbRunSeam.GetPlayObject(humanName);
                        if (player != null)
                        {
                            player.m_nScriptGotoCount = 0;

                            // 原文 1484 / 1517：Player.m_sAuctionItemName := StdItem.Name（显示名，不是 DBName）
                            if (stdItem != null)
                                player.m_sAuctionItemName = stdItem.Name;
                            else
                                player.m_sAuctionItemName = "";

                            player.m_sAuctionItemHumanName = humanName;         // 物品拍卖者
                            player.m_sAuctionItemBidHumanName = lastBidder;     // 竞拍出价者

                            player.m_nAuctionItemStartPrice = startingPrice;    // 底价
                            player.m_nAuctionItemSellPrice = sellingPrice;      // 一口价
                            player.m_nAuctionItemFinaPrice = lastBidPrice;      // 成交价
                            player.m_nAuctionItemInvalidPrice = 0;              // 失效价
                            player.m_nAuctionItemMoneyType = currencyType;      // 货币类型
                            player.m_boAuctionItemSelled = true;                // 物品是否被秒杀/出售

                            AuctionDbRunSeam.GotoLable(player, "@AuctionSellItem");

                            player.m_sAuctionItemName = "";
                            player.m_sAuctionItemHumanName = "";                // 物品拍卖者
                            player.m_sAuctionItemBidHumanName = "";             // 竞拍出价者
                            player.m_nAuctionItemStartPrice = 0;                // 底价
                            player.m_nAuctionItemSellPrice = 0;                 // 一口价
                            player.m_nAuctionItemInvalidPrice = 0;              // 失效价
                            player.m_nAuctionItemFinaPrice = 0;                 // 成交价
                            player.m_nAuctionItemMoneyType = 0;                 // 货币类型
                            player.m_boAuctionItemSelled = false;               // 物品是否被秒杀/出售
                        }

                        player = AuctionDbRunSeam.GetPlayObject(lastBidder);
                        if (player != null)
                        {
                            player.m_nScriptGotoCount = 0;

                            // 原文 1484 / 1517：Player.m_sAuctionItemName := StdItem.Name（显示名，不是 DBName）
                            if (stdItem != null)
                                player.m_sAuctionItemName = stdItem.Name;
                            else
                                player.m_sAuctionItemName = "";

                            player.m_sAuctionItemHumanName = humanName;         // 物品拍卖者
                            player.m_sAuctionItemBidHumanName = lastBidder;     // 竞拍出价者

                            player.m_nAuctionItemStartPrice = startingPrice;    // 底价
                            player.m_nAuctionItemSellPrice = sellingPrice;      // 一口价
                            player.m_nAuctionItemFinaPrice = lastBidPrice;      // 成交价
                            player.m_nAuctionItemInvalidPrice = 0;              // 失效价
                            player.m_nAuctionItemMoneyType = currencyType;      // 货币类型
                            player.m_boAuctionItemSelled = true;                // 物品是否被秒杀/出售

                            AuctionDbRunSeam.GotoLable(player, "@AuctionBuyItem");

                            player.m_sAuctionItemName = "";
                            player.m_sAuctionItemHumanName = "";                // 物品拍卖者
                            player.m_sAuctionItemBidHumanName = "";             // 竞拍出价者
                            player.m_nAuctionItemStartPrice = 0;                // 底价
                            player.m_nAuctionItemSellPrice = 0;                 // 一口价
                            player.m_nAuctionItemInvalidPrice = 0;              // 失效价
                            player.m_nAuctionItemFinaPrice = 0;                 // 成交价
                            player.m_nAuctionItemMoneyType = 0;                 // 货币类型
                            player.m_boAuctionItemSelled = false;               // 物品是否被秒杀/出售
                        }
                    }
                    catch
                    {
                        // 原文如此（SqliteAuctionDB.pas:1543-1544）：except end;（空处理，吞掉本段全部异常）
                    }

                    _fStatementUpdateAuctionItemSuccess!.Reset();
                    _fStatementUpdateAuctionItemSuccess!.OrderBindInt(auctionId);
                    _fStatementUpdateAuctionItemSuccess!.Step();

                    ret = _fStatementQueryAuctionItemSuccess!.Step();
                }
            }
            finally
            {
                _fStatementQueryAuctionItemSuccess!.Reset();
                _fStatementUpdateAuctionItemSuccess!.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
    }

    /// <summary>SqliteAuctionDB.pas:1564-1601 <c>DoHumanRename</c>（3 个 Reset 在 try 之外，原文如此）。</summary>
    protected override bool DoHumanRename(string oldName, string newName)
    {
        bool result = false;
        _fStatementAuctionDataHumanRename!.Reset();
        _fStatementAuctionDataLastBidderRename!.Reset();
        _fStatementAuctionAttentionRename!.Reset();

        try
        {
            _fdb!.BeginTransaction();
            try
            {
                _fStatementAuctionDataHumanRename!.OrderBindText(newName);
                _fStatementAuctionDataHumanRename!.OrderBindText(oldName);
                _fStatementAuctionDataHumanRename!.Step();

                _fStatementAuctionDataLastBidderRename!.OrderBindText(newName);
                _fStatementAuctionDataLastBidderRename!.OrderBindText(oldName);
                _fStatementAuctionDataLastBidderRename!.Step();

                _fStatementAuctionAttentionRename!.OrderBindText(newName);
                _fStatementAuctionAttentionRename!.OrderBindText(oldName);
                _fStatementAuctionAttentionRename!.Step();

                _fdb!.Commit();

                result = true;
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage(e.Message);
                _fdb!.RollBack();
            }
        }
        finally
        {
            _fStatementAuctionDataHumanRename!.Reset();
            _fStatementAuctionDataLastBidderRename!.Reset();
            _fStatementAuctionAttentionRename!.Reset();
        }
        return result;
    }

    // ---------------------------------------------------------------------
    // 以下为原文 DoRun 的内嵌过程与单元级辅助的 1:1 落点（保持方法名逐字）。
    // ---------------------------------------------------------------------

    /// <summary>SqliteAuctionDB.pas:1292-1411 <c>DoRun.IncPlayerGameMoney</c>（DoRun 的内嵌过程）。</summary>
    private static void IncPlayerGameMoney(string playerName, int makeIndex, int nCurrencyType, int prices, string logAdd)
    {
        long nValue;
        TDBChangeGoldType goldType = default;   // 原文如此（SqliteAuctionDB.pas:1351-1364）：Delphi 局部变量未初始化

        IAuctionPlayer? player = AuctionDbRunSeam.GetPlayObject(playerName);
        if (player != null)
        {
            switch (nCurrencyType)
            {
                case 0:
                    {
                        nValue = (long)player.m_nGameGold + prices;
                        if (nValue > uint.MaxValue)   // 原文 High(LongWord)
                            nValue = uint.MaxValue;
                        player.m_nGameGold = (int)nValue;   // 原文 m_nGameGold 是 Integer，赋 Int64 后截断

                        player.GameGoldChanged();
                        break;
                    }
                case 1:
                    {
                        nValue = (long)player.m_nGamePoint + prices;
                        if (nValue > uint.MaxValue)
                            nValue = uint.MaxValue;
                        player.m_nGamePoint = (int)nValue;

                        player.GameGoldChanged();
                        break;
                    }
                case 2:
                    {
                        nValue = (long)player.m_nGold + prices;
                        if (nValue > DbLayerGlobals.Environment.nHumanMaxGold)
                            nValue = DbLayerGlobals.Environment.nHumanMaxGold;
                        player.m_nGold = (int)nValue;

                        player.GoldChanged();
                        break;
                    }
                case 3:
                    {
                        nValue = (long)player.m_nGameDiamond + prices;
                        if (nValue > uint.MaxValue)
                            nValue = uint.MaxValue;
                        player.m_nGameDiamond = (int)nValue;

                        player.NewGamePointChanged();
                        break;
                    }
                case 4:
                    {
                        nValue = (long)player.m_nGameGird + prices;
                        if (nValue > uint.MaxValue)
                            nValue = uint.MaxValue;
                        player.m_nGameGird = (int)nValue;

                        player.NewGamePointChanged();
                        break;
                    }
            }
        }
        else
        {
            switch (nCurrencyType)
            {
                case 0:
                    goldType = TDBChangeGoldType.cgtGameGold;
                    break;
                case 1:
                    goldType = TDBChangeGoldType.cgtGamePoint;
                    break;
                case 2:
                    goldType = TDBChangeGoldType.cgtGold;
                    break;
                case 3:
                    goldType = TDBChangeGoldType.cgtGameDiamond;
                    break;
                case 4:
                    goldType = TDBChangeGoldType.cgtGameGird;
                    break;
                default:
                    return;   // 原文 1362-1363：else Exit;
            }

            // 原文 1366：DataEngine.HumanChangeGold(nil, nil, GoldType, PlayerName, PlayerName, Prices)
            AuctionDbRunSeam.HumanChangeGold(goldType, playerName, prices);
        }

        switch (nCurrencyType)
        {
            case 0:
                {
                    if (AuctionDbRunSeam.GBoGameLogGameGold)
                    {
                        AddGameDataLog(AuctionLogTypes.LOG_GameGoldChange, AuctionLogTypes.LOG_ActionNone,
                            AuctionLogTypes.latHuman, "0", 0, 0, DbLayerGlobals.Environment.sGameGoldName, makeIndex,
                            playerName, "拍卖行-到期", 0, prices, logAdd);
                    }
                    break;
                }
            case 1: // 排除
                {
                    if (AuctionDbRunSeam.GBoGameLogGameGold)
                    {
                        AddGameDataLog(AuctionLogTypes.LOG_GamePointChange, AuctionLogTypes.LOG_ActionNone,
                            AuctionLogTypes.latHuman, "0", 0, 0, DbLayerGlobals.Environment.sGamePointName, makeIndex,
                            playerName, "拍卖行-到期", 0, prices, logAdd);
                    }
                    break;
                }
            case 2:
                {
                    if (AuctionDbRunSeam.GBoGameLogGold)
                    {
                        AddGameDataLog(AuctionLogTypes.LOG_GoldChange, AuctionLogTypes.LOG_ActionNone,
                            AuctionLogTypes.latHuman, "0", 0, 0, AuctionDbRunSeam.SStringGoldName, makeIndex,
                            playerName, "拍卖行-到期", 0, prices, logAdd);
                    }
                    break;
                }
            case 3:
                {
                    if (AuctionDbRunSeam.GBoGameLogGameGold)
                    {
                        AddGameDataLog(AuctionLogTypes.LOG_GameDiamondChange, AuctionLogTypes.LOG_ActionNone,
                            AuctionLogTypes.latHuman, "0", 0, 0, DbLayerGlobals.Environment.sGameDiamondName, makeIndex,
                            playerName, "拍卖行-到期", 0, prices, logAdd);
                    }
                    break;
                }
            case 4:
                {
                    if (AuctionDbRunSeam.GBoGameLogGameGold)
                    {
                        AddGameDataLog(AuctionLogTypes.LOG_GameGirdChange, AuctionLogTypes.LOG_ActionNone,
                            AuctionLogTypes.latHuman, "0", 0, 0, DbLayerGlobals.Environment.sGameGirdName, makeIndex,
                            playerName, "拍卖行-到期", 0, prices, logAdd);
                    }
                    break;
                }
        }
    }

    /// <summary>原文 M2Share <c>AddGameDataLog(LogType, ActionType, ActorType, '0', 0, 0, Name, MakeIndex,
    /// HumanName, Remark, 0, Prices, LogAdd)</c>（13 个实参）。接缝：待 M2Share 移植后接入。</summary>
    private static void AddGameDataLog(int logType, int actionType, int actorType, string param0, int param1, int param2,
        string name, int makeIndex, string humanName, string remark, int param3, int prices, string logAdd)
    {
        AuctionDbRunSeam.AddGameDataLog(new AuctionGameDataLogArgs
        {
            LogType = logType,
            ActionType = actionType,
            ActorType = actorType,
            Param0 = param0,
            Param1 = param1,
            Param2 = param2,
            Name = name,
            MakeIndex = makeIndex,
            HumanName = humanName,
            Remark = remark,
            Param3 = param3,
            Prices = prices,
            LogAdd = logAdd,
        });
    }

    /// <summary>SqliteAuctionDB.pas:788-824 / 1111-1147 的 <c>if ItemColors and N &lt;&gt; 0 then sColors := sColors +
    /// IntToStr(g_Config.btAuctionItemColors[K]) + ',';</c>（6 位，位序 1/2/4/8/16/32 → 下标 0..5）。
    /// 原文 sColors 是这个 if 块内的局部串，每次进入该块都从空串开始（与 SqliteUserShopDB 的同名块不同）。</summary>
    private static string AuctionColorsSql(int itemColors)
    {
        string sColors = "";
        if ((itemColors & 1) != 0)
        {
            sColors = sColors + IntToStr(DbLayerGlobals.Environment.btAuctionItemColors) + ",";
        }

        if ((itemColors & 2) != 0)
        {
            sColors = sColors + IntToStr(DbLayerGlobals.Environment.btAuctionItemColors) + ",";
        }

        if ((itemColors & 4) != 0)
        {
            sColors = sColors + IntToStr(DbLayerGlobals.Environment.btAuctionItemColors) + ",";
        }

        if ((itemColors & 8) != 0)
        {
            sColors = sColors + IntToStr(DbLayerGlobals.Environment.btAuctionItemColors) + ",";
        }

        if ((itemColors & 16) != 0)
        {
            sColors = sColors + IntToStr(DbLayerGlobals.Environment.btAuctionItemColors) + ",";
        }

        if ((itemColors & 32) != 0)
        {
            sColors = sColors + IntToStr(DbLayerGlobals.Environment.btAuctionItemColors) + ",";
        }

        return sColors;
    }

    /// <summary>
    /// SQLite 方言：<c>AuctionData.AddDateTime</c> 是 INTEGER（Unix 秒，建表默认值就是
    /// <c>strftime('%s','now')</c>），原文用 <c>OrderGetColumnValueInt</c> 读出后**直接赋给 TDateTime**
    /// （SqliteAuctionDB.pas:885/935/987/1030/1065）；MySQL 侧同名列走
    /// <c>OrderGetColumnValueDateTime</c>（真 DATETIME）—— 这是原文的方言差异。
    /// <para>
    /// ★ 托管侧偏离说明（必要，已在报告登记）：原文的 <c>TDateTime := Integer</c> 是"把 Unix 秒当成
    /// Delphi 日期序列号"，托管侧 <see cref="DateTime"/> 无法表示（<c>DateTime.FromOADate(1600000000)</c>
    /// 直接抛 <see cref="ArgumentOutOfRangeException"/>，会让 DoQueryAllItems 对**任何真实数据**都返回 0 条）。
    /// 故这里按该列的真实语义把 Unix 秒还原成 <see cref="DateTime"/>，使 SQLite/MySQL 两个方言在托管模型里
    /// 对同一个逻辑字段给出一致的值（原文两库的存储形态本来就不同，这正是不移植 Delphi 数值语义的原因）。
    /// </para>
    /// </summary>
    private static DateTime AuctionAddDateTime(int rawUnixSeconds) => DelphiDateUtil.UnixToDateTime(rawUnixSeconds);

    /// <summary>原文 <c>IntToStr</c>。</summary>
    private static string IntToStr(int value) => GXX.Core.Rtl.DelphiRTL.IntToStr(value);

    /// <summary>M2Share.pas <c>ProcessItemName(s: string): string</c>（Grobal2.pas:6380）。
    /// 接缝：待 Grobal2 的 ProcessItemName 移植后接入（原文按物品名映射"改名"规则）。</summary>
    private static string ProcessItemName(string name) => DbLayerRunSeam.ProcessItemName(name);
}
