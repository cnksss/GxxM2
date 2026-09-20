// 源单元：Source/M2Engine/MySqlAuctionDB.pas（1-1583 行，1:1 移植）
//   TMySqlAuctionDB = class(TAuctionDB)：Create/Destroy、DoInit(173-308)、DoFinal(310-439)、
//   20 个 Do* 覆写（DoAddAuctionItem / DoCancelAuctionItem / DoRetrieveAuctionItem / DoDeleteAuctionItem /
//   DoAddAttentionItem / DoDeleteAttentionItem / DoJoinItemBid / DoQueryAllItems / DoQueryMyItems /
//   DoQueryMyAttentionItems / DoGetAuctionInfo / DoGetAuctionRecord / DoGetAllItemsPageCount /
//   DoGetMyItemsPageCount / DoGetMyAttentionPageCount / DoGetMyAuctioningItemsCount /
//   DoGetMySellFailItemsCount / DoGetMyBuyOKItemsCount / DoRun / DoHumanRename）。
//   public 包装层（Lock/try/except MainOutMessage/finally UnLock、RunTick2 节流）已在 DbBases.cs:640-1039
//   的 TAuctionDB 中实现，本文件**只**写 protected Do* + Create/Destroy + private 字段/局部过程。
//
// SQL 逐字保真：21 条 <c>AddSQLStatement</c> 语句在 SqlStatements.MySqlAuctionDB.cs；方法级动态 SQL
// 模板（1 段多语句删除脚本 + 16 段动态拼接模板）在 SqlStatements.MySqlAuctionDB.Scripts.cs
// —— 全部由 _recon/p3-sql.mjs + p3-gen.mjs 从 GBK 原文机械抽取，**零手工转录**。
// 本实现把拼接点按原文顺序接到 <c>_P0/_P1/...</c> 常量上，送给 driver 的字符串与原文逐字一致
// （已用二次校验脚本把拼接结果与 _recon/p3-MySqlAuctionDB.json 的 16 条 assignment value 逐字比对：
//   DoQueryAllItems 605/647/708/757/803/849/930/972、DoGetAllItemsPageCount 210/271/318/362/406/483、
//   DoDeleteAuctionItem 465）。
//
// 方言要点（对照 SqliteAuctionDB）：
//   * 事务 StartTransaction/Commit/RollBack（不是 SQLite 的 BeginTransaction）；
//   * 批量脚本 Exec(sql)（不是 Execute），多语句删除脚本后还要 ClearResult（MySqlAuctionDB.pas:617）；
//   * 结果集必须 `if X.Query then while (X.Fetch) do`（不是 SQLite 的 `Ret := Step; while Ret = SQLITE_ROW`）；
//   * 无结果集语句的成功判据是 `Step`（Boolean，不是 SQLITE_OK/SQLITE_DONE）；
//   * DoAddAttentionItem/DoDeleteAttentionItem 的存在性判据是 `RowCount <> 0` **不是** 取行
//     （MySqlAuctionDB.pas:642/678，原文注释保留了早先的 `IsAttentioned := ...Step` 写法）；
//   * 绑定/取值一律 OrderBindParamText / OrderBindParamInt（不是 OrderBindText / OrderBindInt）；
//   * AuctionData.AddDateTime 在 MySQL 库是 DATETIME，用 OrderGetColumnValueDateTime 取
//     （不是 SQLite 的 OrderGetColumnValueInt + 直接赋 TDateTime 的原始 Unix 秒）；
//   * 时间比较/排序全用 TIMESTAMPDIFF(SECOND, CURRENT_TIMESTAMP, date_add(AddDateTime, interval AuctionTime hour))
//     （不是 SQLite 的 strftime('%s','now') 差值）；
//   * 字符串字面量用双引号（like "%x%"、HumanName = "x"），SQLite 版用单引号；
//   * 分页 `limit ? offset ?` 两库相同，但 MySQL 的 Prepare 是可重复的（SQLite 版在动态 SQL 上不 Prepare）。
//
// 原文缺陷/易错点（逐字保留，详见 docs/并行报告-p3-m2-dbdata.md §7）：
//   * MySqlAuctionDB.pas:508-509 —— StdMode=28 在第 3 个分支（照明物）已被吃进 igSpecial，
//     第 14 个分支（马牌）是**死代码**（原文如此）；
//   * MySqlAuctionDB.pas:542 —— 用 UserItem.btValue[13]（不是 btValue[0]）判定改名物品；
//   * MySqlAuctionDB.pas:519-565 —— DoAddAuctionItem 的 `if AuctionID > 0` 分支内没有 else，
//     GetMaxAuctionID 返回 0 时静默返回 0（不插入、不报错）；
//   * MySqlAuctionDB.pas:768-804 / 1090-1126 —— sColors 在该 if 块内**不复位**（局部串先加后删尾逗号）；
//   * MySqlAuctionDB.pas:775-798 与 :780-784 —— 位序 1/2/4/8/16/32 全部读**同一个**
//     g_Config.btAuctionItemColors（原文如此，不是 btAuctionItemColors[K]）；
//   * MySqlAuctionDB.pas:1302-1326 —— 玩家不在线时 GoldType 缺省（无 else）却仍调 HumanChangeGold；
//   * MySqlAuctionDB.pas:1284/1293/1302 —— 用 High(LongWord)（4294967295）当上限，赋回 Integer 字段后截断；
//   * MySqlAuctionDB.pas:1524-1525 —— `except end;` 空异常处理，吞掉 NPC GotoLable 段的全部异常；
//   * MySqlAuctionDB.pas:1547-1549 —— DoHumanRename 的 3 个 Reset 在 try **之外**（异常时不复位）；
//   * MySqlAuctionDB.pas:1151 -> :1210 -> :1233 / :1256 —— 页数/数量用 `div`（整除）。
//
// 接缝取用：本文件只用 IDbLayerHost（Lock/UnLock/DataBase/SaveItemToDB）、IMySqlDatabase/IMySqlStatement、
// AuctionDbRunSeam（GetPlayObject/GetStdItem/HumanChangeGold/GotoLable/AddGameDataLog/g_boGameLog*）、
// DbLayerRunSeam.ProcessItemName、IAuctionStdItem（StdMode/OverLap/Shape/Color/DBName/Name/NeedIdentify）。
// 注意：IDbLayerHost.LoadItemFromDB 目前仍是**按值** TUserItem（DbSeam.cs:62），而 DoQueryAllItems /
// DoQueryMyItems / DoQueryMyAttentionItems / DoGetAuctionRecord 需要把读出的物品**写回**记录，
// 故这里一律走与 TM2DataDB 等价的 IDbLayerHost（M2DataDbSupport/DbSeam 的 ref 化只在 IM2DataDb 上完成）。

using GXX.Core.Rtl;
using GXX.Core.Protocol;

namespace GXX.M2Server.DbLayer;

/// <summary>MySqlAuctionDB.pas:10-111 <c>TMySqlAuctionDB</c>（拍卖行数据，MySQL 方言）。</summary>
public sealed class TMySqlAuctionDB : TAuctionDB
{
    private IMySqlDatabase? _fdb;

    /// <summary>MySqlAuctionDB.pas:14 <c>FStatementUpdateAuctionItemFail</c>（更新拍卖物品的状态）。</summary>
    private IMySqlStatement? _fStatementUpdateAuctionItemFail;

    /// <summary>MySqlAuctionDB.pas:15 <c>FStatementUpdateAuctionItemSuccess</c>（更新拍卖物品的状态）。</summary>
    private IMySqlStatement? _fStatementUpdateAuctionItemSuccess;

    /// <summary>MySqlAuctionDB.pas:16 <c>FStatementQueryAuctionItemSuccess</c>（查询拍卖成功的物品）。</summary>
    private IMySqlStatement? _fStatementQueryAuctionItemSuccess;

    /// <summary>MySqlAuctionDB.pas:18 <c>FStatementQueryMyItems</c>（查询我的拍卖物品）。</summary>
    private IMySqlStatement? _fStatementQueryMyItems;

    /// <summary>MySqlAuctionDB.pas:19 <c>FStatementGetMyItemsCount</c>（我正在拍卖物品总页数）。</summary>
    private IMySqlStatement? _fStatementGetMyItemsCount;

    /// <summary>MySqlAuctionDB.pas:21 <c>FStatementQueryMyAttentionItems</c>（查询关注物品）。</summary>
    private IMySqlStatement? _fStatementQueryMyAttentionItems;

    /// <summary>MySqlAuctionDB.pas:22 <c>FStatementQueryMyAttentionItemsCount</c>（我的关注物品总页数）。</summary>
    private IMySqlStatement? _fStatementQueryMyAttentionItemsCount;

    /// <summary>MySqlAuctionDB.pas:24 <c>FStatementQueryOneItem</c>（查询单个物品）。</summary>
    private IMySqlStatement? _fStatementQueryOneItem;

    /// <summary>MySqlAuctionDB.pas:26 <c>FStatementGetMyAuctioningItemsCount</c>（我正在拍卖物品的数量）。</summary>
    private IMySqlStatement? _fStatementGetMyAuctioningItemsCount;

    /// <summary>MySqlAuctionDB.pas:27 <c>FStatementGetMySellFailItemsCount</c>（我流拍未取的物品数量）。</summary>
    private IMySqlStatement? _fStatementGetMySellFailItemsCount;

    /// <summary>MySqlAuctionDB.pas:28 <c>FStatementGetMyBuyOKItemsCount</c>（我拍买未取的物品数量）。</summary>
    private IMySqlStatement? _fStatementGetMyBuyOKItemsCount;

    /// <summary>MySqlAuctionDB.pas:30 <c>FStatementGetMaxAuctionID</c>。</summary>
    private IMySqlStatement? _fStatementGetMaxAuctionID;

    /// <summary>MySqlAuctionDB.pas:31 <c>FStatementInsertAuctionItem</c>。</summary>
    private IMySqlStatement? _fStatementInsertAuctionItem;

    /// <summary>MySqlAuctionDB.pas:32 <c>FStatementInsertAttentionItem</c>（添加关注物品）。</summary>
    private IMySqlStatement? _fStatementInsertAttentionItem;

    /// <summary>MySqlAuctionDB.pas:33 <c>FStatementCheckInAttentionItem</c>。</summary>
    private IMySqlStatement? _fStatementCheckInAttentionItem;

    /// <summary>MySqlAuctionDB.pas:34 <c>FStatementDeleteAttentionItem</c>（删除关注物品）。</summary>
    private IMySqlStatement? _fStatementDeleteAttentionItem;

    /// <summary>MySqlAuctionDB.pas:36 <c>FStatementJoinItemBid</c>（参与物品竞价）。</summary>
    private IMySqlStatement? _fStatementJoinItemBid;

    /// <summary>MySqlAuctionDB.pas:37 <c>FStatementBuyItem</c>（一口价购买物品）。</summary>
    private IMySqlStatement? _fStatementBuyItem;

    /// <summary>MySqlAuctionDB.pas:39 <c>FStatementAuctionDataHumanRename</c>。</summary>
    private IMySqlStatement? _fStatementAuctionDataHumanRename;

    /// <summary>MySqlAuctionDB.pas:40 <c>FStatementAuctionDataLastBidderRename</c>。</summary>
    private IMySqlStatement? _fStatementAuctionDataLastBidderRename;

    /// <summary>MySqlAuctionDB.pas:41 <c>FStatementAuctionAttentionRename</c>。</summary>
    private IMySqlStatement? _fStatementAuctionAttentionRename;

    /// <summary>MySqlAuctionDB.pas:134-166 <c>constructor TMySqlAuctionDB.Create(AOwner: TM2DataDB)</c>。
    /// 接缝：装配方可显式注入 IMySqlDatabase（与 TSqliteAuctionDB/TMySqlAuctionDB 的
    /// <c>(IDbLayerHost owner, IMySqlDatabase? db = null)</c> 约定一致）；
    /// 未注入时 DoInit 仍按原文 177-178 的 <c>Owner.DataBase is TMySqlDatabase</c> 判定回退。</summary>
    public TMySqlAuctionDB(IDbLayerHost owner, IMySqlDatabase? db = null) : base(owner)
    {
        _fdb = db;   // 原文 136：inherited Create(AOwner);（FDB 由 DoInit 的 is 判定取得）

        _fStatementUpdateAuctionItemFail = null;
        _fStatementUpdateAuctionItemSuccess = null;
        _fStatementQueryAuctionItemSuccess = null;

        _fStatementQueryMyItems = null;
        _fStatementGetMyItemsCount = null;

        _fStatementQueryMyAttentionItems = null;
        _fStatementQueryMyAttentionItemsCount = null;

        _fStatementQueryOneItem = null;

        _fStatementGetMyAuctioningItemsCount = null;
        _fStatementGetMySellFailItemsCount = null;
        _fStatementGetMyBuyOKItemsCount = null;

        _fStatementGetMaxAuctionID = null;
        _fStatementInsertAuctionItem = null;
        _fStatementInsertAttentionItem = null;
        _fStatementCheckInAttentionItem = null;
        _fStatementDeleteAttentionItem = null;

        _fStatementJoinItemBid = null;
        _fStatementBuyItem = null;

        _fStatementAuctionDataHumanRename = null;
        _fStatementAuctionDataLastBidderRename = null;
        _fStatementAuctionAttentionRename = null;
    }

    /// <summary>MySqlAuctionDB.pas:168-171 <c>destructor TMySqlAuctionDB.Destroy</c>（只 inherited）。</summary>
    // 托管侧无需覆写析构（原文 168-171 也只有 inherited）。

    // =====================================================================
    // DoInit / DoFinal
    // =====================================================================

    /// <summary>MySqlAuctionDB.pas:173-308 <c>DoInit</c>（21 条 AddSQLStatement + Prepare）。</summary>
    public override void DoInit()
    {
        // 原文 175 行 inherited;

        // 原文 177-178：if (Owner.DataBase <> nil) and (Owner.DataBase is TMySqlDatabase) then FDB := Owner.DataBase as TMySqlDatabase;
        // 接缝：构造函数已注入时优先用注入实例（原文此处会无条件覆盖，等价于"总是从 Owner 取"）。
        if (_fdb == null && Owner.DataBase is IMySqlDatabase mySqlDb) _fdb = mySqlDb;

        IMySqlDatabase fdb = _fdb;

        // ---- 180-191：拍卖成功/失败状态与到期查询 ----
        _fStatementUpdateAuctionItemFail = fdb.AddSQLStatement("Auction_UpdateAuctionItemFail");
        _fStatementUpdateAuctionItemFail.Sql = MySqlAuctionDbStatements.Auction_UpdateAuctionItemFail;

        _fStatementUpdateAuctionItemSuccess = fdb.AddSQLStatement("Auction_UpdateAuctionItemSuccess");
        _fStatementUpdateAuctionItemSuccess.Sql = MySqlAuctionDbStatements.Auction_UpdateAuctionItemSuccess;

        _fStatementQueryAuctionItemSuccess = fdb.AddSQLStatement("Auction_QueryAuctionItemSuccess");
        _fStatementQueryAuctionItemSuccess.Sql = MySqlAuctionDbStatements.Auction_QueryAuctionItemSuccess;

        // 原文 193：// 查询我的拍卖物品
        _fStatementQueryMyItems = fdb.AddSQLStatement("Auction_QueryAuctionItems");
        _fStatementQueryMyItems.Sql = MySqlAuctionDbStatements.Auction_QueryAuctionItems;

        // 原文 200：// 我的拍卖物品总页数
        _fStatementGetMyItemsCount = fdb.AddSQLStatement("Auction_GetMyItemsCount");
        _fStatementGetMyItemsCount.Sql = MySqlAuctionDbStatements.Auction_GetMyItemsCount;

        // 原文 204：// 查询我的关注
        _fStatementQueryMyAttentionItems = fdb.AddSQLStatement("Auction_QueryAttentionItems");
        _fStatementQueryMyAttentionItems.Sql = MySqlAuctionDbStatements.Auction_QueryAttentionItems;

        // 原文 209：// 我的关注物品总页数
        _fStatementQueryMyAttentionItemsCount = fdb.AddSQLStatement("Auction_GetMyAttentionItemsCount");
        _fStatementQueryMyAttentionItemsCount.Sql = MySqlAuctionDbStatements.Auction_GetMyAttentionItemsCount;

        // 原文 213：// 查询单个物品信息
        _fStatementQueryOneItem = fdb.AddSQLStatement("Auction_QueryOneItem");
        _fStatementQueryOneItem.Sql = MySqlAuctionDbStatements.Auction_QueryOneItem;

        // 原文 220：// 我正在拍卖的物品数量
        _fStatementGetMyAuctioningItemsCount = fdb.AddSQLStatement("Auction_GetMyAuctioningItemsCount");
        _fStatementGetMyAuctioningItemsCount.Sql = MySqlAuctionDbStatements.Auction_GetMyAuctioningItemsCount;

        // 原文 225：// 我流拍未取的物品数量
        _fStatementGetMySellFailItemsCount = fdb.AddSQLStatement("Auction_GetMySellFailItemsCount");
        _fStatementGetMySellFailItemsCount.Sql = MySqlAuctionDbStatements.Auction_GetMySellFailItemsCount;

        // 原文 230：// 我拍买未取的物品数量
        _fStatementGetMyBuyOKItemsCount = fdb.AddSQLStatement("Auction_GetMyBuyOKItemsCount");
        _fStatementGetMyBuyOKItemsCount.Sql = MySqlAuctionDbStatements.Auction_GetMyBuyOKItemsCount;

        _fStatementGetMaxAuctionID = fdb.AddSQLStatement("Auction_GetMaxAuctionID");
        _fStatementGetMaxAuctionID.Sql = MySqlAuctionDbStatements.Auction_GetMaxAuctionID;

        // 原文 238：// 添加拍卖物品
        _fStatementInsertAuctionItem = fdb.AddSQLStatement("Auction_InsertAuctionItem");
        _fStatementInsertAuctionItem.Sql = MySqlAuctionDbStatements.Auction_InsertAuctionItem;

        // 原文 244：// 参与物品竞价
        _fStatementJoinItemBid = fdb.AddSQLStatement("Auction_JoinItemBid");
        _fStatementJoinItemBid.Sql = MySqlAuctionDbStatements.Auction_JoinItemBid;

        // 原文 249：// 一口价购买物品
        _fStatementBuyItem = fdb.AddSQLStatement("Auction_BuyItem");
        _fStatementBuyItem.Sql = MySqlAuctionDbStatements.Auction_BuyItem;

        // 原文 254：// 添加关注物品
        _fStatementInsertAttentionItem = fdb.AddSQLStatement("Auction_AddAttentionItem");
        _fStatementInsertAttentionItem.Sql = MySqlAuctionDbStatements.Auction_AddAttentionItem;

        _fStatementCheckInAttentionItem = fdb.AddSQLStatement("Auction_CheckInAttentionItem");
        _fStatementCheckInAttentionItem.Sql = MySqlAuctionDbStatements.Auction_CheckInAttentionItem;

        // 原文 261：// 删除关注物品
        // 原文如此（MySqlAuctionDB.pas:262）：删除关注物品的语句 label 用的是 'Auction_DeleteAuctionItem'
        // （不是 Auction_DeleteAttentionItem）。
        _fStatementDeleteAttentionItem = fdb.AddSQLStatement("Auction_DeleteAuctionItem");
        _fStatementDeleteAttentionItem.Sql = MySqlAuctionDbStatements.Auction_DeleteAuctionItem;

        _fStatementAuctionDataHumanRename = fdb.AddSQLStatement("Auction_HumanRename");
        _fStatementAuctionDataHumanRename.Sql = MySqlAuctionDbStatements.Auction_HumanRename;

        _fStatementAuctionDataLastBidderRename = fdb.AddSQLStatement("Auction_LastBidderRename");
        _fStatementAuctionDataLastBidderRename.Sql = MySqlAuctionDbStatements.Auction_LastBidderRename;

        _fStatementAuctionAttentionRename = fdb.AddSQLStatement("Auction_AttentionRename");
        _fStatementAuctionAttentionRename.Sql = MySqlAuctionDbStatements.Auction_AttentionRename;

        // ---- 274-307：全部 Prepare，整体包在 try/except MainOutMessage 里 ----
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
            _fStatementGetMyBuyOKItemsCount.Prepare();           // 我拍买未取的物品数量

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

    /// <summary>MySqlAuctionDB.pas:310-439 <c>DoFinal</c>（21 个 Finalize + 置 nil，顺序与原文一致）。</summary>
    public override void DoFinal()
    {
        // 原文 312 行 inherited;

        if (_fStatementUpdateAuctionItemFail != null)   // 更新拍卖物品的状态
        {
            _fStatementUpdateAuctionItemFail.StatementFinalize();
            _fStatementUpdateAuctionItemFail = null;
        }

        if (_fStatementUpdateAuctionItemSuccess != null) // 更新拍卖物品的状态
        {
            _fStatementUpdateAuctionItemSuccess.StatementFinalize();
            _fStatementUpdateAuctionItemSuccess = null;
        }

        if (_fStatementQueryAuctionItemSuccess != null)
        {
            _fStatementQueryAuctionItemSuccess.StatementFinalize();
            _fStatementQueryAuctionItemSuccess = null;
        }

        if (_fStatementQueryMyItems != null)            // 查询我的拍卖物品
        {
            _fStatementQueryMyItems.StatementFinalize();
            _fStatementQueryMyItems = null;
        }

        if (_fStatementGetMyItemsCount != null)         // 我正在拍卖物品总页数
        {
            _fStatementGetMyItemsCount.StatementFinalize();
            _fStatementGetMyItemsCount = null;
        }

        if (_fStatementQueryMyAttentionItems != null)   // 查询关注物品
        {
            _fStatementQueryMyAttentionItems.StatementFinalize();
            _fStatementQueryMyAttentionItems = null;
        }

        if (_fStatementQueryMyAttentionItemsCount != null) // 我的关注物品总页数
        {
            _fStatementQueryMyAttentionItemsCount.StatementFinalize();
            _fStatementQueryMyAttentionItemsCount = null;
        }

        if (_fStatementQueryOneItem != null)            // 查询单个物品数量
        {
            _fStatementQueryOneItem.StatementFinalize();
            _fStatementQueryOneItem = null;
        }

        if (_fStatementGetMyAuctioningItemsCount != null) // 我正在拍卖物品的数量
        {
            _fStatementGetMyAuctioningItemsCount.StatementFinalize();
            _fStatementGetMyAuctioningItemsCount = null;
        }

        // 原文如此（MySqlAuctionDB.pas:367）：这里的注释写的是"我正在拍卖物品的数量"（复制粘贴残留）。
        if (_fStatementGetMySellFailItemsCount != null) // 我正在拍卖物品的数量
        {
            _fStatementGetMySellFailItemsCount.StatementFinalize();
            _fStatementGetMySellFailItemsCount = null;
        }

        // 原文如此（MySqlAuctionDB.pas:373）：同上注释残留。
        if (_fStatementGetMyBuyOKItemsCount != null)    // 我正在拍卖物品的数量
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

        if (_fStatementJoinItemBid != null)             // 参与物品竞价
        {
            _fStatementJoinItemBid.StatementFinalize();
            _fStatementJoinItemBid = null;
        }

        if (_fStatementBuyItem != null)                 // 一口价购买物品
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

    // =====================================================================
    // DoAddAuctionItem（441-566）
    // =====================================================================

    /// <summary>MySqlAuctionDB.pas:441-566 <c>DoAddAuctionItem</c>（添加拍卖物品）。</summary>
    protected override int DoAddAuctionItem(string humanName, int auctionTime, int startingPrice, int sellingPrice,
        int currencyType, TUserItem userItem, object? stdItem)
    {
        int result = 0;
        try
        {
            int auctionId;
            try
            {
                _fStatementGetMaxAuctionID.Reset();
                if (_fStatementGetMaxAuctionID.Query() && _fStatementGetMaxAuctionID.Fetch())
                    auctionId = _fStatementGetMaxAuctionID.OrderGetColumnValueInt;
                else
                    auctionId = 1;
            }
            finally
            {
                _fStatementGetMaxAuctionID.Reset();
            }

            IAuctionStdItem std = (IAuctionStdItem)stdItem;

            // 原文 459-517：按 StdMode/OverLap/Shape 归类（分支顺序逐字保留）
            TItemGroup itemGroup = TItemGroup.igOther;
            if (std.StdMode == 10 || std.StdMode == 11)                                             // 衣服
                itemGroup = TItemGroup.igDress;
            else if (std.StdMode == 5 || std.StdMode == 6)                                          // 武器
                itemGroup = TItemGroup.igWeapon;
            else if (std.StdMode == 28 || std.StdMode == 30)                                        // 照明物
                itemGroup = TItemGroup.igSpecial;
            else if (std.StdMode == 19 || std.StdMode == 20 || std.StdMode == 21)                   // 项链
            {
                if (std.OverLap == 2 || std.OverLap == 4 || std.OverLap == 6)
                    itemGroup = TItemGroup.igSpecial;
                else
                    itemGroup = TItemGroup.igNecklace;
            }
            else if (std.StdMode == 15 || std.StdMode == 78)                                        // 头盔
            {
                if (std.StdMode == 15 && (std.OverLap == 2 || std.OverLap == 4 || std.OverLap == 6))
                    itemGroup = TItemGroup.igSpecial;
                else
                    itemGroup = TItemGroup.igHelmet;
            }
            else if (std.StdMode == 24 || std.StdMode == 26)                                        // 手镯
            {
                if (std.OverLap == 2 || std.OverLap == 4 || std.OverLap == 6)
                    itemGroup = TItemGroup.igSpecial;
                else
                    itemGroup = TItemGroup.igArmRing;
            }
            else if (std.StdMode == 22 || std.StdMode == 23)                                        // 戒指
            {
                if (std.OverLap == 2 || std.OverLap == 4 || std.OverLap == 6)
                    itemGroup = TItemGroup.igSpecial;
                else
                    itemGroup = TItemGroup.igRing;
            }
            else if (std.StdMode == 25 || std.StdMode == 51)                                        // 符毒
                itemGroup = TItemGroup.igSpecial;
            else if (std.StdMode == 54 || std.StdMode == 64)                                        // 腰带
                itemGroup = TItemGroup.igBelt;
            else if (std.StdMode == 52 || std.StdMode == 62)                                        // 靴子
                itemGroup = TItemGroup.igBoots;
            else if (std.StdMode == 53 || std.StdMode == 63 || std.StdMode == 7)                    // 宝石
                itemGroup = TItemGroup.igSpecial;
            else if (std.StdMode >= 66 && std.StdMode <= 89)                                        // 时装
                itemGroup = TItemGroup.igFashion;
            else if (std.StdMode == 16)                                                             // 斗笠
                itemGroup = TItemGroup.igSpecial;
            else if (std.StdMode == 65)                                                             // 军鼓
                itemGroup = TItemGroup.igSpecial;
            // 原文如此（MySqlAuctionDB.pas:508-509）：StdMode=28 已在第 3 个分支被吃进 igSpecial，
            // 这个"马牌"分支是**死代码**。
            else if (std.StdMode == 28)                                                             // 马牌
                itemGroup = TItemGroup.igSpecial;
            else if (std.StdMode == 12)                                                             // 盾牌
                itemGroup = TItemGroup.igSpecial;
            else if (std.StdMode == 90)                                                             // 灵玉
                itemGroup = TItemGroup.igSpecial;
            else if (std.StdMode == 0 || std.StdMode == 1 || (std.StdMode == 3 && std.Shape == 12))  // 药品
                itemGroup = TItemGroup.igDrug;
            else if (std.StdMode == 4)                                                              // 技能书籍
                itemGroup = TItemGroup.igSpecial;

            if (auctionId > 0)
            {
                _fdb.StartTransaction();
                try
                {
                    try
                    {
                        _fStatementInsertAuctionItem.Reset();
                        _fStatementInsertAuctionItem.OrderBindParamInt(auctionId);
                        _fStatementInsertAuctionItem.OrderBindParamText(humanName);
                        _fStatementInsertAuctionItem.OrderBindParamInt((int)itemGroup);

                        // 原文 529-532：if UserItem.btColor > 0 then OrderBindParamInt(UserItem.btColor) else ... Color
                        if (userItem.btColor > 0)
                            _fStatementInsertAuctionItem.OrderBindParamInt(userItem.btColor);
                        else
                            _fStatementInsertAuctionItem.OrderBindParamInt(std.Color);

                        _fStatementInsertAuctionItem.OrderBindParamInt(auctionTime);
                        _fStatementInsertAuctionItem.OrderBindParamInt(startingPrice);
                        _fStatementInsertAuctionItem.OrderBindParamInt(sellingPrice);
                        _fStatementInsertAuctionItem.OrderBindParamInt(currencyType);

                        _fStatementInsertAuctionItem.OrderBindParamText(std.DBName);

                        string changeName = "";
                        // 原文如此（MySqlAuctionDB.pas:542）：判的是 btValue[13]（不是 btValue[0]）
                        if (M2ItemDbAccess.GetValue(ref userItem, 13) == 1 && userItem.NameStr.Length > 0)
                            changeName = ProcessItemName(userItem.NameStr);

                        _fStatementInsertAuctionItem.OrderBindParamText(changeName);

                        if (_fStatementInsertAuctionItem.Step())
                        {
                            Owner.SaveItemToDB(userItem, auctionId, DataItemTypes.AuctionItemType, 0);

                            result = auctionId;
                        }
                    }
                    finally
                    {
                        _fStatementInsertAuctionItem.Reset();
                    }

                    _fdb.Commit();
                }
                catch (Exception e)
                {
                    DbLayerGlobals.MainOutMessage(e.Message);
                    _fdb.RollBack();
                }
            }
        }
        catch (Exception e)
        {
            // 接缝：原文没有外层 except（stdItem 为空时原文直接 AV）；托管侧为可空引用，故补一层。
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    // =====================================================================
    // DoCancelAuctionItem / DoRetrieveAuctionItem / DoDeleteAuctionItem（569-629）
    // =====================================================================

    /// <summary>MySqlAuctionDB.pas:568-582 <c>DoCancelAuctionItem</c>（取消拍卖物品）。</summary>
    protected override bool DoCancelAuctionItem(string humanName, int auctionId)
    {
        bool result = false;
        try
        {
            // 原文 573-574：字符串拼接（双引号包裹 HumanName），逐字保留
            _fdb.Exec("update AuctionData set TradingStatus = 1 where AuctionID = " + IntToStr(auctionId) + " and HumanName = \"" +
                humanName + "\";");
            result = true;
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>MySqlAuctionDB.pas:584-597 <c>DoRetrieveAuctionItem</c>（取回拍卖物品）。</summary>
    protected override bool DoRetrieveAuctionItem(int auctionId)
    {
        bool result = false;
        try
        {
            _fdb.Exec("update AuctionData set IsItemGive = 1 where AuctionID = " + IntToStr(auctionId) + ";");
            result = true;
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>MySqlAuctionDB.pas:599-629 <c>DoDeleteAuctionItem</c>（多语句删除脚本，sLineBreak 分隔）。
    /// 脚本由 <c>DoDeleteAuctionItem_L609_Sql_P0..P22</c> 逐段拼出；<c>sWhere</c> 用 11 次、
    /// <c>IntToStr(AuctionID)</c> 用 2 次（与 _RuntimeValueCount = 11 一致）。</summary>
    protected override bool DoDeleteAuctionItem(string humanName, int auctionId)
    {
        bool result = false;
        // 原文 606：sWhere := Format(' WHERE ParentID = %d and ItemType = ' + IntToStr(AUCTION_ITEM_TYPE) + ' and ItemIndex = 0;', [AuctionID]);
        string sWhere = " WHERE ParentID = " + IntToStr(auctionId) + " and ItemType = " +
            IntToStr(DataItemTypes.AuctionItemType) + " and ItemIndex = 0;";
        _fdb.StartTransaction();
        try
        {
            // 原文 609-614：11 个 sWhere 拼接点共用同一个运行时值（sWhere 常量重复引用 11 次）
            string sql = MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P0 + sWhere
                + MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P2 + sWhere
                + MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P4 + sWhere
                + MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P6 + sWhere
                + MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P8 + sWhere
                + MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P10 + sWhere
                + MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P12 + sWhere
                + MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P14 + sWhere
                + MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P16 + sWhere
                + MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P18 + IntToStr(auctionId)
                + MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P20 + IntToStr(auctionId)
                + MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P22;

            _fdb.Exec(sql);
            _fdb.ClearResult();   // 原文 617：MySQL 多语句脚本执行后必须清结果集

            _fdb.Commit();

            result = true;
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
            _fdb.RollBack();
        }
        return result;
    }

    // =====================================================================
    // DoAddAttentionItem / DoDeleteAttentionItem（631-701）
    // =====================================================================

    /// <summary>MySqlAuctionDB.pas:631-665 <c>DoAddAttentionItem</c>（添加关注物品）。
    /// 存在性判据是 <c>RowCount &lt;&gt; 0</c>（原文 641-643：注释保留了早先的 <c>Step</c> 写法）。</summary>
    protected override bool DoAddAttentionItem(string humanName, int index)
    {
        bool result = false;
        try
        {
            bool isAttentioned;
            try
            {
                _fStatementCheckInAttentionItem.Reset();
                _fStatementCheckInAttentionItem.OrderBindParamText(humanName);
                _fStatementCheckInAttentionItem.OrderBindParamInt(index);
                _fStatementCheckInAttentionItem.Query(); // 修复查询不到结果的问题  By 一支笔 at:2022-01-06 16:46:44
                isAttentioned = _fStatementCheckInAttentionItem.RowCount != 0;
                // IsAttentioned := FStatementCheckInAttentionItem.Step;
            }
            finally
            {
                _fStatementCheckInAttentionItem.Reset();
            }

            if (!isAttentioned)
            {
                try
                {
                    _fStatementInsertAttentionItem.Reset();
                    _fStatementInsertAttentionItem.OrderBindParamText(humanName);
                    _fStatementInsertAttentionItem.OrderBindParamInt(index);
                    result = _fStatementInsertAttentionItem.Step();
                }
                finally
                {
                    _fStatementInsertAttentionItem.Reset();
                }
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>MySqlAuctionDB.pas:667-701 <c>DoDeleteAttentionItem</c>（删除关注物品）。
    /// 存在性判据同 DoAddAttentionItem（<c>RowCount &lt;&gt; 0</c>）。</summary>
    protected override bool DoDeleteAttentionItem(string humanName, int index)
    {
        bool result = false;
        try
        {
            bool isAttentioned;
            try
            {
                _fStatementCheckInAttentionItem.Reset();
                _fStatementCheckInAttentionItem.OrderBindParamText(humanName);
                _fStatementCheckInAttentionItem.OrderBindParamInt(index);
                _fStatementCheckInAttentionItem.Query(); // 修复查询不到结果的问题  By 一支笔 at:2022-01-06 16:46:44
                isAttentioned = _fStatementCheckInAttentionItem.RowCount != 0;
                // IsAttentioned := FStatementCheckInAttentionItem.Step;
            }
            finally
            {
                _fStatementCheckInAttentionItem.Reset();
            }

            if (isAttentioned)
            {
                try
                {
                    _fStatementDeleteAttentionItem.Reset();
                    _fStatementDeleteAttentionItem.OrderBindParamText(humanName);
                    _fStatementDeleteAttentionItem.OrderBindParamInt(index);
                    result = _fStatementDeleteAttentionItem.Step();
                }
                finally
                {
                    _fStatementDeleteAttentionItem.Reset();
                }
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>MySqlAuctionDB.pas:703-739 <c>DoJoinItemBid</c>（参加物品竞价；IsSell = 一口价）。</summary>
    protected override bool DoJoinItemBid(string humanName, int index, int prices, bool isSell)
    {
        bool result = false;
        try
        {
            // 原文 707 调的是基类的 public 包装（带锁）
            AddAttentionItem(humanName, index);

            if (isSell)
            {
                try
                {
                    _fStatementBuyItem.Reset();
                    _fStatementBuyItem.OrderBindParamText(humanName);
                    _fStatementBuyItem.OrderBindParamInt(prices);
                    _fStatementBuyItem.OrderBindParamInt(index);
                    result = _fStatementBuyItem.Step();
                }
                finally
                {
                    _fStatementBuyItem.Reset();
                }
            }
            else
            {
                try
                {
                    _fStatementJoinItemBid.Reset();
                    _fStatementJoinItemBid.OrderBindParamText(humanName);
                    _fStatementJoinItemBid.OrderBindParamInt(prices);
                    _fStatementJoinItemBid.OrderBindParamInt(index);
                    result = _fStatementJoinItemBid.Step();
                }
                finally
                {
                    _fStatementJoinItemBid.Reset();
                }
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    // =====================================================================
    // DoQueryAllItems（741-893）
    // =====================================================================

    /// <summary>MySqlAuctionDB.pas:741-893 <c>DoQueryAllItems</c>（查询可竞拍的商品列表）。
    /// 动态 SQL 由 <c>DoQueryAllItems_L754/L765/L802/L808/L813/L818/L823_sm_Sql</c> 与
    /// <c>DoQueryAllItems_L849_sOrderBy</c> 逐段拼出（与原文 `sm.Sql := sm.Sql + ...` 形态一致）。</summary>
    protected override int DoQueryAllItems(string itemName, int itemGroup, string humanName, int nPage, int topmostAuctionId,
        int itemColors, int sortField, bool sortAsc, int moneyType, uint minPrices, uint maxPrices, TAuctionItemList itemList)
    {
        int result = 0;
        try
        {
            // 原文 752：// 查询可竞拍的商品列表
            IMySqlStatement sm = _fdb.AddSQLStatement("Auction_QueryAllItems");
            try
            {
                // 原文 754-761：select ... and (A.AuctionID <> IntToStr(TopmostAuctionID) + ') '
                sm.Sql = MySqlAuctionDbScripts.DoQueryAllItems_L754_sm_Sql_P0
                    + IntToStr(topmostAuctionId)
                    + MySqlAuctionDbScripts.DoQueryAllItems_L754_sm_Sql_P2;

                if (itemGroup != (int)TItemGroup.igAll)
                {
                    // 原文 765：sm.Sql := sm.Sql + ' and (ItemGroup = ' + IntToStr(Integer(ItemGroup)) + ')';
                    // ★ DoQueryAllItems_L765_sm_Sql_P2 是**累积快照**的字面段（= 上一状态尾部的 ') ' +
                    //   本步新增的 ' and (ItemGroup = '），首字符 ')' 属于上一状态，故 Substring(1)。
                    // ★ _P3 是**拼接点占位符**（"@@IntToStr(ItemGroup)@@"），必须换成真正的 IntToStr(itemGroup)；
                    //   早期版本直接拼了占位符 → itemGroup ≠ igAll 时 SQL 非法（由 DbLayerAuctionBehaviorMySqlTests 探针抓出）。
                    sm.Sql = sm.Sql + MySqlAuctionDbScripts.DoQueryAllItems_L765_sm_Sql_P2.Substring(MySqlAuctionDbScripts.DoQueryAllItems_L754_sm_Sql_P2.Length)
                        + IntToStr(itemGroup)
                        + MySqlAuctionDbScripts.DoQueryAllItems_L765_sm_Sql_P4;
                }

                if (itemColors != 0)
                {
                    string sColors = AuctionColorsSql(itemColors);

                    if (sColors.Length > 0)
                    {
                        // 原文 800-803：sm.Sql := sm.Sql + ' and ItemColor in (' + Copy(sColors, 1, Length(sColors) - 1) + ')';
                        // ★ _P4 快照字面段以 ')' 开头（那是 ItemGroup 的收尾括号），故 Substring(1)。
                        sm.Sql = sm.Sql + MySqlAuctionDbScripts.DoQueryAllItems_L802_sm_Sql_P4.Substring(MySqlAuctionDbScripts.DoQueryAllItems_L765_sm_Sql_P4.Length)
                            + Copy(sColors, 1, sColors.Length - 1)
                            + MySqlAuctionDbScripts.DoQueryAllItems_L802_sm_Sql_P6;
                    }
                }

                if (moneyType > 0)
                {
                    // 原文 806-809：sm.Sql := sm.Sql + ' and A.CurrencyType = ' + IntToStr(MoneyType - 1);
                    // ★ _P6 快照字面段以 ')' 开头（那是 ItemColor 的收尾括号），故 Substring(1)。
                    sm.Sql = sm.Sql + MySqlAuctionDbScripts.DoQueryAllItems_L808_sm_Sql_P6.Substring(MySqlAuctionDbScripts.DoQueryAllItems_L802_sm_Sql_P6.Length)
                        + IntToStr(moneyType - 1);
                }

                if (minPrices > 0)
                {
                    // 原文 811-814：sm.Sql := sm.Sql + ' and A.SellingPrice >= ' + IntToStr(MinPrices);
                    sm.Sql = sm.Sql + MySqlAuctionDbScripts.DoQueryAllItems_L813_sm_Sql_P8
                        + IntToStr((int)minPrices);
                }

                if (maxPrices > 0)
                {
                    // 原文 816-819：sm.Sql := sm.Sql + ' and A.SellingPrice <= ' + IntToStr(MaxPrices);
                    sm.Sql = sm.Sql + MySqlAuctionDbScripts.DoQueryAllItems_L818_sm_Sql_P10
                        + IntToStr((int)maxPrices);
                }

                if (itemName.Length > 0)
                {
                    // 原文 821-824：sm.Sql := sm.Sql + ' and ((A.ItemDBName like "%' + ItemName + '%") or (A.ItemName like "%' + ItemName + '%"))';
                    sm.Sql = sm.Sql + MySqlAuctionDbScripts.DoQueryAllItems_L823_sm_Sql_P12
                        + itemName
                        + MySqlAuctionDbScripts.DoQueryAllItems_L823_sm_Sql_P14
                        + itemName
                        + MySqlAuctionDbScripts.DoQueryAllItems_L823_sm_Sql_P16;
                }

                // 原文 747：var sOrderBy（未初始化）；原文 826-850 只在 SortField ∈ [0,3] 时赋值
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
                        sOrderBy = " order by TIMESTAMPDIFF(SECOND, CURRENT_TIMESTAMP, date_add(AddDateTime, interval AuctionTime hour))";
                    else
                        sOrderBy = " order by TIMESTAMPDIFF(SECOND, CURRENT_TIMESTAMP, date_add(AddDateTime, interval AuctionTime hour)) desc";
                }
                else if (sortField == 0)
                {
                    // 原文 849：唯一的静态 sOrderBy 常量
                    sOrderBy = MySqlAuctionDbScripts.DoQueryAllItems_L849_sOrderBy;
                }

                sm.Sql = sm.Sql + sOrderBy + " limit ? offset ?;";

                sm.Prepare();

                sm.OrderBindParamText(humanName);
                sm.OrderBindParamInt(Grobal2Const.AUCTION_PAGE_COUNT);
                sm.OrderBindParamInt((nPage - 1) * Grobal2Const.AUCTION_PAGE_COUNT);
                if (sm.Query())
                {
                    while (sm.Fetch())
                    {
                        var auctionRecord = new TAuctionRecord();
                        auctionRecord.AuctionID = sm.OrderGetColumnValueInt;
                        auctionRecord.HumanName = sm.OrderGetColumnValueText;
                        // 方言：MySQL 侧 AddDateTime 是 DATETIME，用 OrderGetColumnValueDateTime
                        auctionRecord.AddDateTime = sm.OrderGetColumnValueDateTime;
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

                        result++;
                    }
                }
            }
            finally
            {
                sm.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    // =====================================================================
    // DoQueryMyItems / DoQueryMyAttentionItems（896-996）
    // =====================================================================

    /// <summary>MySqlAuctionDB.pas:895-941 <c>DoQueryMyItems</c>（查询我的拍卖物品）。</summary>
    protected override int DoQueryMyItems(string humanName, int nPage, TAuctionItemList itemList)
    {
        int result = 0;
        try
        {
            try
            {
                _fStatementQueryMyItems.Reset();
                _fStatementQueryMyItems.OrderBindParamText(humanName);
                _fStatementQueryMyItems.OrderBindParamInt(Grobal2Const.AUCTION_PAGE_COUNT);
                _fStatementQueryMyItems.OrderBindParamInt((nPage - 1) * Grobal2Const.AUCTION_PAGE_COUNT);

                if (_fStatementQueryMyItems.Query())
                {
                    while (_fStatementQueryMyItems.Fetch())
                    {
                        var auctionRecord = new TAuctionRecord();
                        auctionRecord.AuctionID = _fStatementQueryMyItems.OrderGetColumnValueInt;
                        auctionRecord.HumanName = _fStatementQueryMyItems.OrderGetColumnValueText;
                        auctionRecord.AddDateTime = _fStatementQueryMyItems.OrderGetColumnValueDateTime;
                        auctionRecord.AuctionTime = _fStatementQueryMyItems.OrderGetColumnValueInt;
                        auctionRecord.TimeLeft = _fStatementQueryMyItems.OrderGetColumnValueInt;
                        auctionRecord.StartingPrice = (uint)_fStatementQueryMyItems.OrderGetColumnValueInt;
                        auctionRecord.SellingPrice = (uint)_fStatementQueryMyItems.OrderGetColumnValueInt;
                        auctionRecord.CurrencyType = _fStatementQueryMyItems.OrderGetColumnValueInt;
                        auctionRecord.LastBidPrice = _fStatementQueryMyItems.OrderGetColumnValueInt;
                        auctionRecord.LastBidder = _fStatementQueryMyItems.OrderGetColumnValueText;
                        auctionRecord.TradingStatus = _fStatementQueryMyItems.OrderGetColumnValueInt;
                        auctionRecord.IsItemGive = _fStatementQueryMyItems.OrderGetColumnValueBool;

                        Owner.LoadItemFromDB(ref auctionRecord.ActionItem, auctionRecord.AuctionID,
                            DataItemTypes.AuctionItemType, 0);

                        itemList.Add(auctionRecord);

                        result++;
                    }
                }
            }
            finally
            {
                _fStatementQueryMyItems.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>MySqlAuctionDB.pas:943-996 <c>DoQueryMyAttentionItems</c>（查询我的关注物品）。
    /// 原文 986-989：finally 里 <c>QueryOneItem.Reset</c> 在 <c>QueryMyAttentionItems.Reset</c> **之前**。</summary>
    protected override int DoQueryMyAttentionItems(string humanName, int nPage, TAuctionItemList itemList)
    {
        int result = 0;
        try
        {
            try
            {
                _fStatementQueryMyAttentionItems.Reset();
                _fStatementQueryMyAttentionItems.OrderBindParamText(humanName);
                _fStatementQueryMyAttentionItems.OrderBindParamInt(Grobal2Const.AUCTION_PAGE_COUNT);
                _fStatementQueryMyAttentionItems.OrderBindParamInt((nPage - 1) * Grobal2Const.AUCTION_PAGE_COUNT);

                if (_fStatementQueryMyAttentionItems.Query())
                {
                    while (_fStatementQueryMyAttentionItems.Fetch())
                    {
                        var auctionRecord = new TAuctionRecord();
                        auctionRecord.AuctionID = _fStatementQueryMyAttentionItems.OrderGetColumnValueInt;

                        _fStatementQueryOneItem.Reset();
                        _fStatementQueryOneItem.OrderBindParamInt(auctionRecord.AuctionID);
                        if (_fStatementQueryOneItem.Query() && _fStatementQueryOneItem.Fetch())
                        {
                            auctionRecord.HumanName = _fStatementQueryOneItem.OrderGetColumnValueText;
                            auctionRecord.AddDateTime = _fStatementQueryOneItem.OrderGetColumnValueDateTime;
                            auctionRecord.AuctionTime = _fStatementQueryOneItem.OrderGetColumnValueInt;
                            auctionRecord.TimeLeft = _fStatementQueryOneItem.OrderGetColumnValueInt;
                            auctionRecord.StartingPrice = (uint)_fStatementQueryOneItem.OrderGetColumnValueInt;
                            auctionRecord.SellingPrice = (uint)_fStatementQueryOneItem.OrderGetColumnValueInt;
                            auctionRecord.CurrencyType = _fStatementQueryOneItem.OrderGetColumnValueInt;
                            auctionRecord.LastBidPrice = _fStatementQueryOneItem.OrderGetColumnValueInt;
                            auctionRecord.LastBidder = _fStatementQueryOneItem.OrderGetColumnValueText;
                            auctionRecord.TradingStatus = _fStatementQueryOneItem.OrderGetColumnValueInt;
                            auctionRecord.IsItemGive = _fStatementQueryOneItem.OrderGetColumnValueBool;
                        }

                        Owner.LoadItemFromDB(ref auctionRecord.ActionItem, auctionRecord.AuctionID,
                            DataItemTypes.AuctionItemType, 0);

                        itemList.Add(auctionRecord);

                        result++;
                    }
                }
            }
            finally
            {
                _fStatementQueryOneItem.Reset();
                _fStatementQueryMyAttentionItems.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    // =====================================================================
    // DoGetAuctionInfo / DoGetAuctionRecord（998-1069）
    // =====================================================================

    /// <summary>MySqlAuctionDB.pas:998-1031 <c>DoGetAuctionInfo</c>（获取竞拍物品信息）。</summary>
    protected override bool DoGetAuctionInfo(int index, out TAuctionInfo auctionInfo)
    {
        // 原文 var 参数是调用方栈上的记录；托管侧 out 必须先赋实例（逐字保留原文的赋值顺序）
        auctionInfo = new TAuctionInfo();
        bool result = false;
        try
        {
            try
            {
                _fStatementQueryOneItem.Reset();
                _fStatementQueryOneItem.OrderBindParamInt(index);
                if (_fStatementQueryOneItem.Query() && _fStatementQueryOneItem.Fetch())
                {
                    auctionInfo.AuctionID = index;
                    auctionInfo.HumanName = _fStatementQueryOneItem.OrderGetColumnValueText;
                    auctionInfo.AddDateTime = _fStatementQueryOneItem.OrderGetColumnValueDateTime;
                    auctionInfo.AuctionTime = _fStatementQueryOneItem.OrderGetColumnValueInt;
                    auctionInfo.TimeLeft = _fStatementQueryOneItem.OrderGetColumnValueInt;
                    auctionInfo.StartingPrice = (uint)_fStatementQueryOneItem.OrderGetColumnValueInt;
                    auctionInfo.SellingPrice = (uint)_fStatementQueryOneItem.OrderGetColumnValueInt;
                    auctionInfo.CurrencyType = _fStatementQueryOneItem.OrderGetColumnValueInt;
                    auctionInfo.LastBidPrice = _fStatementQueryOneItem.OrderGetColumnValueInt;
                    auctionInfo.LastBidder = _fStatementQueryOneItem.OrderGetColumnValueText;
                    auctionInfo.TradingStatus = _fStatementQueryOneItem.OrderGetColumnValueInt;
                    auctionInfo.IsItemGive = _fStatementQueryOneItem.OrderGetColumnValueBool;

                    result = true;
                }
            }
            finally
            {
                _fStatementQueryOneItem.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>MySqlAuctionDB.pas:1033-1069 <c>DoGetAuctionRecord</c>（获取竞拍物品记录）。</summary>
    protected override bool DoGetAuctionRecord(int index, out TAuctionRecord auctionRecord)
    {
        auctionRecord = new TAuctionRecord();
        bool result = false;
        try
        {
            try
            {
                _fStatementQueryOneItem.Reset();
                _fStatementQueryOneItem.OrderBindParamInt(index);
                if (_fStatementQueryOneItem.Query() && _fStatementQueryOneItem.Fetch())
                {
                    auctionRecord.AuctionID = index;
                    auctionRecord.HumanName = _fStatementQueryOneItem.OrderGetColumnValueText;
                    auctionRecord.AddDateTime = _fStatementQueryOneItem.OrderGetColumnValueDateTime;
                    auctionRecord.AuctionTime = _fStatementQueryOneItem.OrderGetColumnValueInt;
                    auctionRecord.TimeLeft = _fStatementQueryOneItem.OrderGetColumnValueInt;
                    auctionRecord.StartingPrice = (uint)_fStatementQueryOneItem.OrderGetColumnValueInt;
                    auctionRecord.SellingPrice = (uint)_fStatementQueryOneItem.OrderGetColumnValueInt;
                    auctionRecord.CurrencyType = _fStatementQueryOneItem.OrderGetColumnValueInt;
                    auctionRecord.LastBidPrice = _fStatementQueryOneItem.OrderGetColumnValueInt;
                    auctionRecord.LastBidder = _fStatementQueryOneItem.OrderGetColumnValueText;
                    auctionRecord.TradingStatus = _fStatementQueryOneItem.OrderGetColumnValueInt;
                    auctionRecord.IsItemGive = _fStatementQueryOneItem.OrderGetColumnValueBool;
                    auctionRecord.IsAttention = false;

                    Owner.LoadItemFromDB(ref auctionRecord.ActionItem, auctionRecord.AuctionID,
                        DataItemTypes.AuctionItemType, 0);

                    result = true;
                }
            }
            finally
            {
                _fStatementQueryOneItem.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    // =====================================================================
    // DoGetAllItemsPageCount（1071-1162）
    // =====================================================================

    /// <summary>MySqlAuctionDB.pas:1071-1162 <c>DoGetAllItemsPageCount</c>（查询所有拍卖物品总页数）。
    /// 动态 SQL 由 <c>DoGetAllItemsPageCount_L1082_sm_Sql</c> 起步，
    /// 依次拼 <c>L1087/L1124/L1130/L1135/L1140/L1145_sm_Sql</c> 的 literal run（与原文形态一致）。</summary>
    protected override int DoGetAllItemsPageCount(string itemName, int itemGroup, int itemColors, int moneyType,
        uint minPrices, uint maxPrices)
    {
        int result = 0;
        try
        {
            IMySqlStatement sm = _fdb.AddSQLStatement("Auction_GetAllItemsCount");
            try
            {
                // 原文 1082-1083：// 查询所有拍卖物品总页数
                sm.Sql = MySqlAuctionDbScripts.DoGetAllItemsPageCount_L1082_sm_Sql;

                if (itemGroup != (int)TItemGroup.igAll)
                {
                    // 原文 1085-1088：sm.Sql := sm.Sql + ' and ItemGroup = ' + IntToStr(Integer(ItemGroup));
                    // 原文 1085-1088：sm.Sql := sm.Sql + ' and ItemGroup = ' + IntToStr(Integer(ItemGroup));
                    // ★ _L1087_sm_Sql_P0 是**累积快照**（把基串又内联了一遍）；早期版本直接把它拼上去，
                    //   结果整条 select 基串被拼了两次（SQL 非法，由 DbLayerAuctionBehaviorMySqlTests 探针抓出）。
                    //   Substring(基串长度) 只取本步新增的 ' and ItemGroup = '。
                    sm.Sql = sm.Sql + MySqlAuctionDbScripts.DoGetAllItemsPageCount_L1087_sm_Sql_P0
                        .Substring(MySqlAuctionDbScripts.DoGetAllItemsPageCount_L1082_sm_Sql.Length)
                        + IntToStr(itemGroup);
                }

                if (itemColors != 0)
                {
                    string sColors = AuctionColorsSql(itemColors);

                    if (sColors.Length > 0)
                    {
                        // 原文 1122-1125：sm.Sql := sm.Sql + ' and ItemColor in (' + Copy(sColors, 1, Length(sColors) - 1) + ')';
                        sm.Sql = sm.Sql + MySqlAuctionDbScripts.DoGetAllItemsPageCount_L1124_sm_Sql_P2
                            + Copy(sColors, 1, sColors.Length - 1)
                            + MySqlAuctionDbScripts.DoGetAllItemsPageCount_L1124_sm_Sql_P4;
                    }
                }

                if (moneyType > 0)
                {
                    // 原文 1128-1131：sm.Sql := sm.Sql + ' and CurrencyType = ' + IntToStr(MoneyType - 1);
                    // ★ _P4 快照字面段以 ')' 开头（那是 ItemColor 子句的收尾括号；ItemColor 未启用时
                    //   基串本身也已以 ')' 收尾），故必须 Substring(1)。
                    sm.Sql = sm.Sql + MySqlAuctionDbScripts.DoGetAllItemsPageCount_L1130_sm_Sql_P4.Substring(MySqlAuctionDbScripts.DoGetAllItemsPageCount_L1124_sm_Sql_P4.Length)
                        + IntToStr(moneyType - 1);
                }

                if (minPrices > 0)
                {
                    // 原文 1133-1136：sm.Sql := sm.Sql + ' and SellingPrice >= ' + IntToStr(MinPrices);
                    sm.Sql = sm.Sql + MySqlAuctionDbScripts.DoGetAllItemsPageCount_L1135_sm_Sql_P6
                        + IntToStr((int)minPrices);
                }

                if (maxPrices > 0)
                {
                    // 原文 1138-1141：sm.Sql := sm.Sql + ' and SellingPrice <= ' + IntToStr(MaxPrices);
                    sm.Sql = sm.Sql + MySqlAuctionDbScripts.DoGetAllItemsPageCount_L1140_sm_Sql_P8
                        + IntToStr((int)maxPrices);
                }

                if (itemName.Length > 0)
                {
                    // 原文 1143-1146：sm.Sql := sm.Sql + ' and ((ItemDBName like "%' + ItemName + '%") or (ItemName like "%' + ItemName + '%"))';
                    sm.Sql = sm.Sql + MySqlAuctionDbScripts.DoGetAllItemsPageCount_L1145_sm_Sql_P10
                        + itemName
                        + MySqlAuctionDbScripts.DoGetAllItemsPageCount_L1145_sm_Sql_P12
                        + itemName
                        + MySqlAuctionDbScripts.DoGetAllItemsPageCount_L1145_sm_Sql_P14;
                }

                sm.Prepare();
                if (sm.Query() && sm.Fetch())
                {
                    // 原文 1151：div（整除）
                    result = (sm.OrderGetColumnValueInt + Grobal2Const.AUCTION_PAGE_COUNT - 1) / Grobal2Const.AUCTION_PAGE_COUNT;
                }
            }
            finally
            {
                sm.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    // =====================================================================
    // DoGetMyItemsPageCount / DoGetMyAttentionPageCount / DoGetMy*ItemsCount（1164-1267）
    // =====================================================================

    /// <summary>MySqlAuctionDB.pas:1164-1177 <c>DoGetMyItemsPageCount</c>（我的拍卖物品页数）。
    /// 原文 1167-1176：**没有内层 try**（Reset 在 finally，与外层 except 直接配对）。</summary>
    protected override int DoGetMyItemsPageCount(string humanName)
    {
        int result = 0;
        try
        {
            _fStatementGetMyItemsCount.Reset();
            _fStatementGetMyItemsCount.OrderBindParamText(humanName);
            if (_fStatementGetMyItemsCount.Query() && _fStatementGetMyItemsCount.Fetch())
            {
                result = (_fStatementGetMyItemsCount.OrderGetColumnValueInt + Grobal2Const.AUCTION_PAGE_COUNT - 1)
                    / Grobal2Const.AUCTION_PAGE_COUNT;
            }
        }
        finally
        {
            _fStatementGetMyItemsCount.Reset();
        }
        return result;
    }

    /// <summary>MySqlAuctionDB.pas:1179-1199 <c>DoGetMyAttentionPageCount</c>（我的关注物品页数）。
    /// 原文 1182-1192：内层 try/finally 在**内层** try/except 之内（与外层不同）。</summary>
    protected override int DoGetMyAttentionPageCount(string humanName)
    {
        int result = 0;
        try
        {
            try
            {
                _fStatementQueryMyAttentionItemsCount.Reset();
                _fStatementQueryMyAttentionItemsCount.OrderBindParamText(humanName);
                if (_fStatementQueryMyAttentionItemsCount.Query() && _fStatementQueryMyAttentionItemsCount.Fetch())
                {
                    result = (_fStatementQueryMyAttentionItemsCount.OrderGetColumnValueInt + Grobal2Const.AUCTION_PAGE_COUNT - 1)
                        / Grobal2Const.AUCTION_PAGE_COUNT;
                }
            }
            finally
            {
                _fStatementQueryMyAttentionItemsCount.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>MySqlAuctionDB.pas:1201-1221 <c>DoGetMyAuctioningItemsCount</c>（我正在拍卖的物品数量）。</summary>
    protected override int DoGetMyAuctioningItemsCount(string humanName)
    {
        int result = 0;
        try
        {
            try
            {
                _fStatementGetMyAuctioningItemsCount.Reset();
                _fStatementGetMyAuctioningItemsCount.OrderBindParamText(humanName);
                if (_fStatementGetMyAuctioningItemsCount.Query() && _fStatementGetMyAuctioningItemsCount.Fetch())
                {
                    result = _fStatementGetMyAuctioningItemsCount.OrderGetColumnValueInt;
                }
            }
            finally
            {
                _fStatementGetMyAuctioningItemsCount.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>MySqlAuctionDB.pas:1223-1244 <c>DoGetMySellFailItemsCount</c>（我流拍未取的物品数量）。</summary>
    protected override int DoGetMySellFailItemsCount(string humanName)
    {
        int result = 0;
        try
        {
            try
            {
                _fStatementGetMySellFailItemsCount.Reset();
                _fStatementGetMySellFailItemsCount.OrderBindParamText(humanName);
                if (_fStatementGetMySellFailItemsCount.Query() && _fStatementGetMySellFailItemsCount.Fetch())
                {
                    result = _fStatementGetMySellFailItemsCount.OrderGetColumnValueInt;
                }
            }
            finally
            {
                _fStatementGetMySellFailItemsCount.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>MySqlAuctionDB.pas:1246-1267 <c>DoGetMyBuyOKItemsCount</c>（我拍买到未取的物品数量）。</summary>
    protected override int DoGetMyBuyOKItemsCount(string humanName)
    {
        int result = 0;
        try
        {
            try
            {
                _fStatementGetMyBuyOKItemsCount.Reset();
                _fStatementGetMyBuyOKItemsCount.OrderBindParamText(humanName);
                if (_fStatementGetMyBuyOKItemsCount.Query() && _fStatementGetMyBuyOKItemsCount.Fetch())
                {
                    result = _fStatementGetMyBuyOKItemsCount.OrderGetColumnValueInt;
                }
            }
            finally
            {
                _fStatementGetMyBuyOKItemsCount.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    // =====================================================================
    // DoRun（1269-1542）+ 内嵌过程 IncPlayerGameMoney（1271-1390）
    // =====================================================================

    /// <summary>MySqlAuctionDB.pas:1269-1542 <c>DoRun</c>（拍卖到期结算；内嵌过程
    /// IncPlayerGameMoney 见 1271-1390，此处同层收成 private 方法）。</summary>
    protected override void DoRun()
    {
        try
        {
            try
            {
                _fStatementUpdateAuctionItemFail.Reset();
                _fStatementUpdateAuctionItemFail.Step();
            }
            finally
            {
                _fStatementUpdateAuctionItemFail.Reset();
            }

            try
            {
                _fStatementQueryAuctionItemSuccess.Reset();

                if (_fStatementQueryAuctionItemSuccess.Query())
                {
                    while (_fStatementQueryAuctionItemSuccess.Fetch())
                    {
                        int auctionId = _fStatementQueryAuctionItemSuccess.OrderGetColumnValueInt;
                        string humanName = _fStatementQueryAuctionItemSuccess.OrderGetColumnValueText;
                        int currencyType = _fStatementQueryAuctionItemSuccess.OrderGetColumnValueInt;
                        int startingPrice = _fStatementQueryAuctionItemSuccess.OrderGetColumnValueInt;
                        int sellingPrice = _fStatementQueryAuctionItemSuccess.OrderGetColumnValueInt;
                        string lastBidder = _fStatementQueryAuctionItemSuccess.OrderGetColumnValueText;
                        int lastBidPrice = _fStatementQueryAuctionItemSuccess.OrderGetColumnValueInt;
                        int dbIndex = _fStatementQueryAuctionItemSuccess.OrderGetColumnValueInt;
                        int makeIndex = _fStatementQueryAuctionItemSuccess.OrderGetColumnValueInt;

                        int nAuctionTaxRate = 0;

                        switch (currencyType)
                        {
                            case 0:
                                nAuctionTaxRate = (int)DbLayerGlobals.Environment.dwAuctionGameGoldTaxRate;      // 元宝
                                break;
                            case 1:
                                nAuctionTaxRate = (int)DbLayerGlobals.Environment.dwAuctionGamePointTaxRate;     // 游戏点
                                break;
                            case 2:
                                nAuctionTaxRate = (int)DbLayerGlobals.Environment.dwAuctionGoldTaxRate;          // 金币
                                break;
                            case 3:
                                nAuctionTaxRate = (int)DbLayerGlobals.Environment.dwAuctionGameDiamondTaxRate;   // 金刚石
                                break;
                            case 4:
                                nAuctionTaxRate = (int)DbLayerGlobals.Environment.dwAuctionGameGirdTaxRate;      // 灵符
                                break;
                        }

                        IAuctionStdItem stdItem = AuctionDbRunSeam.GetStdItem(dbIndex);

                        // 原文 1440-1445：StdItem := UserEngine.GetStdItem(DBIndex); if StdItem <> nil then ItemName := StdItem.Name else ItemName := ''
                        string itemName;
                        if (stdItem != null)
                            itemName = stdItem.Name;
                        else
                            itemName = "";

                        // 原文 1447-1449：// 加拍卖者的钱
                        IncPlayerGameMoney(humanName, makeIndex, currencyType,
                            lastBidPrice - (int)Math.Round(lastBidPrice / 100.0 * nAuctionTaxRate),
                            "卖出物品: " + itemName);
                        try
                        {
                            // 原文 1451-1454：if (StdItem <> nil) and (StdItem.NeedIdentify = 1) then
                            if (stdItem != null && stdItem.NeedIdentify == 1)
                            {
                                AddGameDataLog(AuctionLogTypes.LOG_ItemSell, AuctionLogTypes.LOG_ActionNone,
                                    AuctionLogTypes.latHuman, "0", 0, 0, stdItem.Name, makeIndex, humanName,
                                    "拍卖行-到期", 0, 0, "买入:" + lastBidder);
                                AddGameDataLog(AuctionLogTypes.LOG_ItemBuy, AuctionLogTypes.LOG_ActionNone,
                                    AuctionLogTypes.latHuman, "0", 0, 0, stdItem.Name, makeIndex, lastBidder,
                                    "拍卖行-到期", 0, 0, "待取回, 卖出:" + humanName);
                            }

                            IAuctionPlayer player = AuctionDbRunSeam.GetPlayObject(humanName);
                            if (player != null)
                            {
                                player.m_nScriptGotoCount = 0;

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

                                if (stdItem != null)
                                    player.m_sAuctionItemName = stdItem.Name;
                                else
                                    player.m_sAuctionItemName = "";

                                player.m_sAuctionItemHumanName = humanName;         // 物品拍卖者
                                player.m_sAuctionItemBidHumanName = lastBidder;     // 竞拍出价者

                                player.m_nAuctionItemStartPrice = startingPrice;    // 底价
                                player.m_nAuctionItemSellPrice = sellingPrice;      // 一口价
                                player.m_nAuctionItemInvalidPrice = 0;              // 失效价
                                player.m_nAuctionItemFinaPrice = lastBidPrice;      // 成交价
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
                            // 原文如此（MySqlAuctionDB.pas:1524-1525）：except end;（空处理，吞掉本段全部异常）
                        }

                        _fStatementUpdateAuctionItemSuccess.Reset();
                        _fStatementUpdateAuctionItemSuccess.OrderBindParamInt(auctionId);
                        _fStatementUpdateAuctionItemSuccess.Step();
                    }
                }
            }
            finally
            {
                _fStatementQueryAuctionItemSuccess.Reset();
                _fStatementUpdateAuctionItemSuccess.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
    }

    /// <summary>MySqlAuctionDB.pas:1271-1390 <c>DoRun.IncPlayerGameMoney</c>（DoRun 的内嵌过程）。</summary>
    private static void IncPlayerGameMoney(string playerName, int makeIndex, int nCurrencyType, int prices, string logAdd)
    {
        long nValue;
        // 原文如此（MySqlAuctionDB.pas:1330-1343）：玩家不在线分支里 GoldType 只在 case 中赋值，
        // case 之外（default → Exit）不会用到；托管侧给出 default 初值以表达"未初始化"。
        TDBChangeGoldType goldType = default;

        IAuctionPlayer player = AuctionDbRunSeam.GetPlayObject(playerName);
        if (player != null)
        {
            switch (nCurrencyType)
            {
                case 0:
                    {
                        nValue = (long)player.m_nGameGold + prices;
                        // 原文 1284：High(LongWord) = 4294967295（赋回 Integer 字段后截断）
                        if (nValue > uint.MaxValue)
                            nValue = uint.MaxValue;
                        player.m_nGameGold = (int)nValue;

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
                    return;   // 原文 1341-1342：else Exit;
            }

            // 原文 1345：DataEngine.HumanChangeGold(nil, nil, GoldType, PlayerName, PlayerName, Prices)
            AuctionDbRunSeam.HumanChangeGold(goldType, playerName, prices);
        }

        // 原文 1348-1389：按货币类型写日志（注意 1 号分支原文注释"// 排除"）
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

    // =====================================================================
    // DoHumanRename（1544-1581）
    // =====================================================================

    /// <summary>MySqlAuctionDB.pas:1544-1581 <c>DoHumanRename</c>（3 个 Reset 在 try **之外**，原文如此）。</summary>
    protected override bool DoHumanRename(string oldName, string newName)
    {
        bool result = false;
        _fStatementAuctionDataHumanRename.Reset();
        _fStatementAuctionDataLastBidderRename.Reset();
        _fStatementAuctionAttentionRename.Reset();

        try
        {
            _fdb.StartTransaction();
            try
            {
                _fStatementAuctionDataHumanRename.OrderBindParamText(newName);
                _fStatementAuctionDataHumanRename.OrderBindParamText(oldName);
                _fStatementAuctionDataHumanRename.Step();

                _fStatementAuctionDataLastBidderRename.OrderBindParamText(newName);
                _fStatementAuctionDataLastBidderRename.OrderBindParamText(oldName);
                _fStatementAuctionDataLastBidderRename.Step();

                _fStatementAuctionAttentionRename.OrderBindParamText(newName);
                _fStatementAuctionAttentionRename.OrderBindParamText(oldName);
                _fStatementAuctionAttentionRename.Step();

                _fdb.Commit();

                result = true;
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage(e.Message);
                _fdb.RollBack();
            }
        }
        finally
        {
            _fStatementAuctionDataHumanRename.Reset();
            _fStatementAuctionDataLastBidderRename.Reset();
            _fStatementAuctionAttentionRename.Reset();
        }
        return result;
    }

    // =====================================================================
    // 单元级辅助（原文函数/局部块）
    // =====================================================================

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

    /// <summary>MySqlAuctionDB.pas:768-804 / 1090-1126 的
    /// <c>if ItemColors and N &lt;&gt; 0 then sColors := sColors + IntToStr(g_Config.btAuctionItemColors[K]) + ',';</c>
    /// （6 位，位序 1/2/4/8/16/32 → 下标 0..5）。
    /// 原文 sColors 是该 if 块内的局部串（每次进入该块都从空串开始）。
    /// 原文如此（MySqlAuctionDB.pas:772-798）：6 处读的都是同一个 <c>g_Config.btAuctionItemColors</c>
    /// （接缝侧只暴露了一个 int，原文写的是 <c>btAuctionItemColors[0..5]</c> 数组）。</summary>
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

    /// <summary>原文 <c>IntToStr</c>。</summary>
    private static string IntToStr(int value) => DelphiRTL.IntToStr(value);

    /// <summary>原文 <c>Copy(sColors, 1, Length(sColors) - 1)</c>（Delphi 1-based 截尾逗号）。</summary>
    private static string Copy(string s, int index, int count) => DelphiRTL.Copy(s, index, count);

    /// <summary>M2Share.pas <c>ProcessItemName(s: string): string</c>（Grobal2.pas:6380）。
    /// 接缝：待 Grobal2 的 ProcessItemName 移植后接入（原文按物品名映射"改名"规则）。</summary>
    private static string ProcessItemName(string name) => DbLayerRunSeam.ProcessItemName(name);
}
