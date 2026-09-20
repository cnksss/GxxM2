// 源单元：Source/M2Engine/SqliteUserShopDB.pas（1-2161 行，1:1 移植）
//   TSqliteUserShopDB = class(TUserShopDB)：DoInit / DoFinal / 20 个 Do* 覆写 + Create/Destroy。
//
// SQL 文本**逐字**保留（含 limit/offset 写法、空格、以及原文混用的 `strftime('%s', 'now')` /
// `strftime("%s", "now")` 两种写法——后者原文就写成双引号，SQLite 会把它当字符串，逐字保留）。
// ★ Delphi 字符串字面量里的 `''` 是**一个**单引号的转义、编译期即折叠，所以运行期 SQL 是
//   `ifnull(BuyerName, '')`（空串）；早期版本把它当成"源码形态保留"，导致送给 driver 的 SQL 变成
//   `ifnull(BuyerName, '''')`（SQL 里是**一个单字符的串**，`length` 恒为 1 ⇒ 整族判定失效）。
//   已用 _recon/delphi-sql-extract.mjs 修正解码并重新生成（见 docs/并行报告-p3-m2-dbdata.md §4.1）。
// 全部 42 条静态语句的 SQL 已由 _recon 从 GBK 原文机械抽取并写入 SqlStatements.SqliteUserShopDB.cs；
// 其中 5 条方法级 const 见 SqlConsts.SqliteUserShopDB.cs（同样机械抽取）。
//
// 方言要点（对照 MySqlUserShopDB）：
//   * 事务 BeginTransaction/Commit/RollBack；批量脚本 Execute(sql)；时间以 Unix 秒（Int64）绑/读
//     （读时 UnixToDateTime(x + 8*60*60)）。
//   * 无结果集语句的成功判据是 `Step in [SQLITE_OK, SQLITE_DONE]`（枚举两值，非单纯 =SQLITE_ROW）。
//   * 结果集循环 `Ret := Step; while Ret = SQLITE_ROW do ... Ret := Step;`。
//   * DoRun 走 MakeIndex 版本语句（GetHumanSellingItem_MakeIndex 系列），且无 SetTimeHasArrivedSellItems。

using System;
using GXX.Core.Rtl;
using GXX.M2Server.Engine;

namespace GXX.M2Server.DbLayer;

/// <summary>SqliteUserShopDB.pas:16-125 <c>TSqliteUserShopDB</c>。</summary>
public sealed class TSqliteUserShopDB : TUserShopDB
{
    private ISqliteDatabase? _fdb;

    private ISqliteStatement? _fStatementUpdateAllBusiness;
    private ISqliteStatement? _fStatementUpdateHumanBusiness;

    private ISqliteStatement? _fStatementHumanNameExists;
    private ISqliteStatement? _fStatementShopNameExists;
    private ISqliteStatement? _fStatementInsertUserShop;
    private ISqliteStatement? _fStatementUserShopRename;
    private ISqliteStatement? _fStatementUserShopRename2;
    private ISqliteStatement? _fStatementShopItemBuyerRenameName;

    private ISqliteStatement? _fStatementGetUserShopInfo;
    private ISqliteStatement? _fStatementIncUserShopCareValue;
    private ISqliteStatement? _fStatementGetMaxShopItemID;
    private ISqliteStatement? _fStatementInsertUserShopItem;
    private ISqliteStatement? _fStatementUpdateUserShopItem;
    private ISqliteStatement? _fStatementBuyUserShopItem;
    private ISqliteStatement? _fStatementGetMoneyShopItem;
    private ISqliteStatement? _fStatementGetSelledAndNoGetMoneyTotal;

    private ISqliteStatement? _fStatementGetTimeHasArrivedSellItems;

    private ISqliteStatement? _fStatementGetShopSellingItem_ASC;
    private ISqliteStatement? _fStatementGetShopSellingItem_DESC;

    private ISqliteStatement? _fStatementGetShopSelledItem_ASC;
    private ISqliteStatement? _fStatementGetShopSelledItem_DESC;

    private ISqliteStatement? _fStatementGetShopStorageItem_ASC;
    private ISqliteStatement? _fStatementGetShopStorageItem_DESC;

    private ISqliteStatement? _fStatementGetShopSellingAndStorageItem_ASC;
    private ISqliteStatement? _fStatementGetShopSellingAndStorageItem_DESC;

    private ISqliteStatement? _fStatementGetShopSellingItem_Count;
    private ISqliteStatement? _fStatementGetShopSelledItem_Count;
    private ISqliteStatement? _fStatementGetShopStorageItem_Count;
    private ISqliteStatement? _fStatementGetShopSellingAndStorageItem_Count;

    private ISqliteStatement? _fStatementGetAllShop_Sort0;
    private ISqliteStatement? _fStatementGetAllShop_Sort1;
    private ISqliteStatement? _fStatementGetAllShop_Sort2;
    private ISqliteStatement? _fStatementGetAllShop_Sort3;
    private ISqliteStatement? _fStatementGetAllShop_Sort4;
    private ISqliteStatement? _fStatementGetAllShop_Sort5;

    private ISqliteStatement? _fStatementGetAllShop_Count;

    private ISqliteStatement? _fStatementGetAllShop_Ex;

    // 原文 69-72 行注释掉的 4 个 FStatementGetHuman{Selling,Selled,Storage,SellingAndStorage}Item。

    private ISqliteStatement? _fStatementGetHumanSellingItem_MakeIndex;
    private ISqliteStatement? _fStatementGetHumanSelledItem_MakeIndex;
    private ISqliteStatement? _fStatementGetHumanStorageItem_MakeIndex;
    private ISqliteStatement? _fStatementGetHumanSellingAndStorageItem_MakeIndex;

    /// <summary>SqliteUserShopDB.pas:134-196 <c>constructor Create</c>：逐个字段置 nil。</summary>
    public TSqliteUserShopDB(IDbLayerHost owner) : base(owner)
    {
        _fStatementUpdateAllBusiness = null;
        _fStatementUpdateHumanBusiness = null;

        _fStatementHumanNameExists = null;
        _fStatementShopNameExists = null;
        _fStatementInsertUserShop = null;
        _fStatementUserShopRename = null;

        _fStatementUserShopRename2 = null;
        _fStatementShopItemBuyerRenameName = null;

        _fStatementGetUserShopInfo = null;
        _fStatementIncUserShopCareValue = null;
        _fStatementGetMaxShopItemID = null;
        _fStatementInsertUserShopItem = null;
        _fStatementUpdateUserShopItem = null;
        _fStatementBuyUserShopItem = null;
        _fStatementGetMoneyShopItem = null;
        _fStatementGetSelledAndNoGetMoneyTotal = null;
        _fStatementGetTimeHasArrivedSellItems = null;

        _fStatementGetShopSellingItem_ASC = null;
        _fStatementGetShopSellingItem_DESC = null;

        _fStatementGetShopSelledItem_ASC = null;
        _fStatementGetShopSelledItem_DESC = null;

        _fStatementGetShopStorageItem_ASC = null;
        _fStatementGetShopStorageItem_DESC = null;

        _fStatementGetShopSellingAndStorageItem_ASC = null;
        _fStatementGetShopSellingAndStorageItem_DESC = null;

        _fStatementGetShopSellingItem_Count = null;
        _fStatementGetShopSelledItem_Count = null;
        _fStatementGetShopStorageItem_Count = null;
        _fStatementGetShopSellingAndStorageItem_Count = null;

        _fStatementGetAllShop_Sort0 = null;
        _fStatementGetAllShop_Sort1 = null;
        _fStatementGetAllShop_Sort2 = null;
        _fStatementGetAllShop_Sort3 = null;
        _fStatementGetAllShop_Sort4 = null;
        _fStatementGetAllShop_Sort5 = null;

        _fStatementGetAllShop_Count = null;

        _fStatementGetAllShop_Ex = null;

        _fStatementGetHumanSellingItem_MakeIndex = null;
        _fStatementGetHumanSelledItem_MakeIndex = null;
        _fStatementGetHumanStorageItem_MakeIndex = null;
        _fStatementGetHumanSellingAndStorageItem_MakeIndex = null;
    }

    /// <summary>SqliteUserShopDB.pas:198-201 <c>destructor Destroy</c>：仅 inherited（无 DoFinal）。</summary>
    // 托管侧无显式析构；DoFinal 由宿主在 TM2DataDB.Final 时调用（与原文一致）。

    /// <summary>SqliteUserShopDB.pas:203-473 <c>DoInit</c>。</summary>
    public override void DoInit()
    {
        // 原文 226 行 inherited;（基类 DoInit 为 abstract，无实现体）

        // 原文 228-229：if (Owner.DataBase <> nil) and (Owner.DataBase is TSqlite3DataBase) then FDB := Owner.DataBase as TSqlite3DataBase;
        if (Owner.DataBase is ISqliteDatabase sqliteDb) _fdb = sqliteDb;

        ISqliteDatabase fdb = _fdb!;

        _fStatementUpdateAllBusiness = fdb.AddSQLStatement("UserShop_UpdateAllBusiness");
        _fStatementUpdateAllBusiness.Sql = "update UserShop set IsBusiness = 0;";
        _fStatementUpdateAllBusiness.Prepare();
        _fStatementUpdateAllBusiness.Step();

        _fStatementUpdateHumanBusiness = fdb.AddSQLStatement("UserShop_UpdateHumanBusiness");
        _fStatementUpdateHumanBusiness.Sql = "update UserShop set IsBusiness = ? where HumanName = ?;";
        _fStatementUpdateHumanBusiness.Prepare();

        _fStatementHumanNameExists = fdb.AddSQLStatement("UserShop_CheckHumanNameExists");
        _fStatementHumanNameExists.Sql = "select 1 FROM UserShop where HumanName = ?;";
        _fStatementHumanNameExists.Prepare();

        _fStatementShopNameExists = fdb.AddSQLStatement("UserShop_CheckShopNameExists");
        _fStatementShopNameExists.Sql = "select 1 FROM UserShop where ShopName = ?;";
        _fStatementShopNameExists.Prepare();

        _fStatementInsertUserShop = fdb.AddSQLStatement("UserShop_InsertUserShop");
        _fStatementInsertUserShop.Sql = "insert into UserShop(HumanName, ShopName, IsBusiness) values(?, ?, 1);";
        _fStatementInsertUserShop.Prepare();

        _fStatementUserShopRename = fdb.AddSQLStatement("UserShop_UpdateShopName");
        _fStatementUserShopRename.Sql = "update UserShop set ShopName = ? where ShopID = ?;";
        _fStatementUserShopRename.Prepare();

        _fStatementUserShopRename2 = fdb.AddSQLStatement("UserShop_UpdateShopName2");
        _fStatementUserShopRename2.Sql = "update UserShop set HumanName = ? where HumanName = ?";
        _fStatementUserShopRename2.Prepare();

        _fStatementShopItemBuyerRenameName = fdb.AddSQLStatement("UserShop_ShopItemBuyerRenameName");
        _fStatementShopItemBuyerRenameName.Sql = "update UserShopItem set BuyerName = ? where BuyerName = ?";
        _fStatementShopItemBuyerRenameName.Prepare();

        _fStatementGetUserShopInfo = fdb.AddSQLStatement("UserShop_GetUserShopInfo");
        _fStatementGetUserShopInfo.Sql = SqliteUserShopStatements.UserShop_GetUserShopInfo;
        _fStatementGetUserShopInfo.Prepare();

        _fStatementIncUserShopCareValue = fdb.AddSQLStatement("UserShop_IncUserShopCareValue");
        _fStatementIncUserShopCareValue.Sql = "UPDATE UserShop SET CareValue = CareValue + 1 where HumanName = ?";
        _fStatementIncUserShopCareValue.Prepare();

        _fStatementGetMaxShopItemID = fdb.AddSQLStatement("UserShop_GetMaxShopItemID");
        _fStatementGetMaxShopItemID.Sql = "select ifnull(Max(ItemID), 0) + 1 from UserShopItem where ShopID = ?;";
        _fStatementGetMaxShopItemID.Prepare();

        _fStatementInsertUserShopItem = fdb.AddSQLStatement("UserShop_InsertShopItem");
        _fStatementInsertUserShopItem.Sql = "insert into UserShopItem(" + "ShopID," + "ItemID," + "ItemType," + "CreateDate," +
            "IsAllowSell," + "MoneyType," + "ItemPrice," + "IsGetMoney," + "BuyerName," + "ItemDBName," + "ItemName) " +
            "values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";
        _fStatementInsertUserShopItem.Prepare();

        _fStatementUpdateUserShopItem = fdb.AddSQLStatement("UserShop_UpdateShopItem");
        _fStatementUpdateUserShopItem.Sql =
            "update UserShopItem set CreateDate = (strftime('%s', 'now')), ItemType = ?, IsAllowSell = ?, MoneyType = ?, ItemPrice = ? where length(ifnull(BuyerName, '')) = 0 and ShopID = ? and ItemID = ?";
        _fStatementUpdateUserShopItem.Prepare();

        _fStatementBuyUserShopItem = fdb.AddSQLStatement("UserShop_BuyShopItem");
        _fStatementBuyUserShopItem.Sql =
            "update UserShopItem set CreateDate = (strftime('%s', 'now')), BuyerName = ? where length(ifnull(BuyerName, '')) = 0 and ShopID = ? and ItemID = ?";
        _fStatementBuyUserShopItem.Prepare();

        _fStatementGetMoneyShopItem = fdb.AddSQLStatement("UserShop_GetMoneyShopItem");
        _fStatementGetMoneyShopItem.Sql =
            "update UserShopItem set IsGetMoney = 1 where length(ifnull(BuyerName, '')) > 0 and ShopID = ? and ItemID = ?";
        _fStatementGetMoneyShopItem.Prepare();

        _fStatementGetSelledAndNoGetMoneyTotal = fdb.AddSQLStatement("UserShop_GetSelledAndNoGetMoneyTotal");
        _fStatementGetSelledAndNoGetMoneyTotal.Sql = "SELECT " + "B.HumanName, " + "A.MoneyType, " + "Sum(A.ItemPrice) SumPrice " +
            "FROM " + "UserShopItem A, " + "UserShop B " + "WHERE " +
            "A.ShopID = B.ShopID and length(ifnull(A.BuyerName, '')) > 0 and IsGetMoney = 0 " + "GROUP BY B.HumanName, A.MoneyType " +
            "ORDER BY B.HumanName";
        _fStatementGetSelledAndNoGetMoneyTotal.Prepare();

        _fStatementGetTimeHasArrivedSellItems = fdb.AddSQLStatement("UserShop_GetTimeHasArrivedSellItems");
        _fStatementGetTimeHasArrivedSellItems.Sql = SqliteUserShopStatements.UserShop_GetTimeHasArrivedSellItems;
        _fStatementGetTimeHasArrivedSellItems.Prepare();

        _fStatementGetShopSellingItem_ASC = fdb.AddSQLStatement("UserShop_GetSellingItem_ASC");
        _fStatementGetShopSellingItem_ASC.Sql = SqliteUserShopStatements.UserShop_GetSellingItem_ASC;
        _fStatementGetShopSellingItem_ASC.Prepare();

        _fStatementGetShopSellingItem_DESC = fdb.AddSQLStatement("UserShop_GetSellingItem_DESC");
        _fStatementGetShopSellingItem_DESC.Sql = SqliteUserShopStatements.UserShop_GetSellingItem_DESC;
        _fStatementGetShopSellingItem_DESC.Prepare();

        _fStatementGetShopSelledItem_ASC = fdb.AddSQLStatement("UserShop_GetSelledItem_ASC");
        _fStatementGetShopSelledItem_ASC.Sql = SqliteUserShopStatements.UserShop_GetSelledItem_ASC;
        _fStatementGetShopSelledItem_ASC.Prepare();

        _fStatementGetShopSelledItem_DESC = fdb.AddSQLStatement("UserShop_GetSelledItem_DESC");
        _fStatementGetShopSelledItem_DESC.Sql = SqliteUserShopStatements.UserShop_GetSelledItem_DESC;
        _fStatementGetShopSelledItem_DESC.Prepare();

        _fStatementGetShopStorageItem_ASC = fdb.AddSQLStatement("UserShop_GetStorageItem_ASC");
        _fStatementGetShopStorageItem_ASC.Sql = SqliteUserShopStatements.UserShop_GetStorageItem_ASC;
        _fStatementGetShopStorageItem_ASC.Prepare();

        _fStatementGetShopStorageItem_DESC = fdb.AddSQLStatement("UserShop_GetStorageItem_DESC");
        _fStatementGetShopStorageItem_DESC.Sql = SqliteUserShopStatements.UserShop_GetStorageItem_DESC;
        _fStatementGetShopStorageItem_DESC.Prepare();

        _fStatementGetShopSellingAndStorageItem_ASC = fdb.AddSQLStatement("UserShop_GetSellingAndStorageItem_ASC");
        _fStatementGetShopSellingAndStorageItem_ASC.Sql = SqliteUserShopStatements.UserShop_GetSellingAndStorageItem_ASC;
        _fStatementGetShopSellingAndStorageItem_ASC.Prepare();

        _fStatementGetShopSellingAndStorageItem_DESC = fdb.AddSQLStatement("UserShop_GetSellingAndStorageItem_DESC");
        _fStatementGetShopSellingAndStorageItem_DESC.Sql = SqliteUserShopStatements.UserShop_GetSellingAndStorageItem_DESC;
        _fStatementGetShopSellingAndStorageItem_DESC.Prepare();

        _fStatementGetShopSellingItem_Count = fdb.AddSQLStatement("UserShop_GetSellingItem_Count");
        _fStatementGetShopSellingItem_Count.Sql = SqliteUserShopStatements.UserShop_GetSellingItem_Count;
        _fStatementGetShopSellingItem_Count.Prepare();

        _fStatementGetShopSelledItem_Count = fdb.AddSQLStatement("UserShop_GetSelledItem_Count");
        _fStatementGetShopSelledItem_Count.Sql = SqliteUserShopStatements.UserShop_GetSelledItem_Count;
        _fStatementGetShopSelledItem_Count.Prepare();

        _fStatementGetShopStorageItem_Count = fdb.AddSQLStatement("UserShop_GetStorageItem_Count");
        _fStatementGetShopStorageItem_Count.Sql = SqliteUserShopStatements.UserShop_GetStorageItem_Count;
        _fStatementGetShopStorageItem_Count.Prepare();

        _fStatementGetShopSellingAndStorageItem_Count = fdb.AddSQLStatement("UserShop_GetSellingAndStorageItem_Count");
        _fStatementGetShopSellingAndStorageItem_Count.Sql = SqliteUserShopStatements.UserShop_GetSellingAndStorageItem_Count;
        _fStatementGetShopSellingAndStorageItem_Count.Prepare();

        _fStatementGetAllShop_Sort0 = fdb.AddSQLStatement("UserShop_GetAllShop_S0");
        _fStatementGetAllShop_Sort0.Sql = SqliteUserShopStatements.UserShop_GetAllShop_S0;
        _fStatementGetAllShop_Sort0.Prepare();

        _fStatementGetAllShop_Sort1 = fdb.AddSQLStatement("UserShop_GetAllShop_S1");
        _fStatementGetAllShop_Sort1.Sql = SqliteUserShopStatements.UserShop_GetAllShop_S1;
        _fStatementGetAllShop_Sort1.Prepare();

        _fStatementGetAllShop_Sort2 = fdb.AddSQLStatement("UserShop_GetAllShop_S2");
        _fStatementGetAllShop_Sort2.Sql = SqliteUserShopStatements.UserShop_GetAllShop_S2;
        _fStatementGetAllShop_Sort2.Prepare();

        _fStatementGetAllShop_Sort3 = fdb.AddSQLStatement("UserShop_GetAllShop_S3");
        _fStatementGetAllShop_Sort3.Sql = SqliteUserShopStatements.UserShop_GetAllShop_S3;
        _fStatementGetAllShop_Sort3.Prepare();

        _fStatementGetAllShop_Sort4 = fdb.AddSQLStatement("UserShop_GetAllShop_S4");
        _fStatementGetAllShop_Sort4.Sql = SqliteUserShopStatements.UserShop_GetAllShop_S4;
        _fStatementGetAllShop_Sort4.Prepare();

        // 原文 404 行：Sort5 只赋 Sql，**没有 Prepare**（逐字保留该不对称）。
        _fStatementGetAllShop_Sort5 = fdb.AddSQLStatement("UserShop_GetAllShop_S5");
        _fStatementGetAllShop_Sort5.Sql = SqliteUserShopStatements.UserShop_GetAllShop_S5;

        _fStatementGetAllShop_Count = fdb.AddSQLStatement("UserShop_GetAllShop_Count_S");
        _fStatementGetAllShop_Count.Sql = "SELECT " + "Count(*) " + "FROM " + "UserShop " + "WHERE " +
            " (IsBusiness >= ?) and (IsBusiness <= ?) ";
        _fStatementGetAllShop_Count.Prepare();

        _fStatementGetAllShop_Ex = fdb.AddSQLStatement("UserShop_GetAllShop_Ex");
        _fStatementGetAllShop_Ex.Sql = "SELECT " + "A.ShopID," + "A.HumanName," + "A.ShopName," + "A.IsBusiness," + "A.CreateDate," +
            "A.CareValue " + "FROM " + "UserShop A ";
        _fStatementGetAllShop_Ex.Prepare();

        // 原文 417-450 行花括号注释掉的 4 条 FStatementGetHumanSellingItem/Selled/Storage/SellingAndStorageItem。

        _fStatementGetHumanSellingItem_MakeIndex = fdb.AddSQLStatement("UserShop_GetHumanSellingItem_MakeIndex");
        _fStatementGetHumanSellingItem_MakeIndex.Sql = SqliteUserShopStatements.UserShop_GetHumanSellingItem_MakeIndex;
        _fStatementGetHumanSellingItem_MakeIndex.Prepare();

        _fStatementGetHumanSelledItem_MakeIndex = fdb.AddSQLStatement("UserShop_GetHumanSelledItem_MakeIndex");
        _fStatementGetHumanSelledItem_MakeIndex.Sql = SqliteUserShopStatements.UserShop_GetHumanSelledItem_MakeIndex;
        _fStatementGetHumanSelledItem_MakeIndex.Prepare();

        _fStatementGetHumanStorageItem_MakeIndex = fdb.AddSQLStatement("UserShop_GetHumanStorageItem_MakeIndex");
        _fStatementGetHumanStorageItem_MakeIndex.Sql = SqliteUserShopStatements.UserShop_GetHumanStorageItem_MakeIndex;
        _fStatementGetHumanStorageItem_MakeIndex.Prepare();

        _fStatementGetHumanSellingAndStorageItem_MakeIndex = fdb.AddSQLStatement("UserShop_GetHumanSellingAndStorageItem_MakeIndex");
        _fStatementGetHumanSellingAndStorageItem_MakeIndex.Sql = SqliteUserShopStatements.UserShop_GetHumanSellingAndStorageItem_MakeIndex;
        _fStatementGetHumanSellingAndStorageItem_MakeIndex.Prepare();
    }

    /// <summary>SqliteUserShopDB.pas:475-748 <c>DoFinal</c>：逐个 Finalize 并置 nil（顺序与 DoInit 相同）。</summary>
    public override void DoFinal()
    {
        // 原文 477 inherited;（基类 DoFinal 为 abstract）

        if (_fStatementUpdateAllBusiness != null) { _fStatementUpdateAllBusiness.StatementFinalize(); _fStatementUpdateAllBusiness = null; }
        if (_fStatementUpdateHumanBusiness != null) { _fStatementUpdateHumanBusiness.StatementFinalize(); _fStatementUpdateHumanBusiness = null; }
        if (_fStatementHumanNameExists != null) { _fStatementHumanNameExists.StatementFinalize(); _fStatementHumanNameExists = null; }
        if (_fStatementShopNameExists != null) { _fStatementShopNameExists.StatementFinalize(); _fStatementShopNameExists = null; }
        if (_fStatementInsertUserShop != null) { _fStatementInsertUserShop.StatementFinalize(); _fStatementInsertUserShop = null; }
        if (_fStatementUserShopRename != null) { _fStatementUserShopRename.StatementFinalize(); _fStatementUserShopRename = null; }
        if (_fStatementUserShopRename2 != null) { _fStatementUserShopRename2.StatementFinalize(); _fStatementUserShopRename2 = null; }
        if (_fStatementShopItemBuyerRenameName != null) { _fStatementShopItemBuyerRenameName.StatementFinalize(); _fStatementShopItemBuyerRenameName = null; }
        if (_fStatementGetUserShopInfo != null) { _fStatementGetUserShopInfo.StatementFinalize(); _fStatementGetUserShopInfo = null; }
        if (_fStatementIncUserShopCareValue != null) { _fStatementIncUserShopCareValue.StatementFinalize(); _fStatementIncUserShopCareValue = null; }
        if (_fStatementGetMaxShopItemID != null) { _fStatementGetMaxShopItemID.StatementFinalize(); _fStatementGetMaxShopItemID = null; }
        if (_fStatementInsertUserShopItem != null) { _fStatementInsertUserShopItem.StatementFinalize(); _fStatementInsertUserShopItem = null; }
        if (_fStatementUpdateUserShopItem != null) { _fStatementUpdateUserShopItem.StatementFinalize(); _fStatementUpdateUserShopItem = null; }
        if (_fStatementBuyUserShopItem != null) { _fStatementBuyUserShopItem.StatementFinalize(); _fStatementBuyUserShopItem = null; }
        if (_fStatementGetMoneyShopItem != null) { _fStatementGetMoneyShopItem.StatementFinalize(); _fStatementGetMoneyShopItem = null; }
        if (_fStatementGetSelledAndNoGetMoneyTotal != null) { _fStatementGetSelledAndNoGetMoneyTotal.StatementFinalize(); _fStatementGetSelledAndNoGetMoneyTotal = null; }
        if (_fStatementGetTimeHasArrivedSellItems != null) { _fStatementGetTimeHasArrivedSellItems.StatementFinalize(); _fStatementGetTimeHasArrivedSellItems = null; }
        if (_fStatementGetShopSellingItem_ASC != null) { _fStatementGetShopSellingItem_ASC.StatementFinalize(); _fStatementGetShopSellingItem_ASC = null; }
        if (_fStatementGetShopSellingItem_DESC != null) { _fStatementGetShopSellingItem_DESC.StatementFinalize(); _fStatementGetShopSellingItem_DESC = null; }
        if (_fStatementGetShopSelledItem_ASC != null) { _fStatementGetShopSelledItem_ASC.StatementFinalize(); _fStatementGetShopSelledItem_ASC = null; }
        if (_fStatementGetShopSelledItem_DESC != null) { _fStatementGetShopSelledItem_DESC.StatementFinalize(); _fStatementGetShopSelledItem_DESC = null; }
        if (_fStatementGetShopStorageItem_ASC != null) { _fStatementGetShopStorageItem_ASC.StatementFinalize(); _fStatementGetShopStorageItem_ASC = null; }
        if (_fStatementGetShopStorageItem_DESC != null) { _fStatementGetShopStorageItem_DESC.StatementFinalize(); _fStatementGetShopStorageItem_DESC = null; }
        if (_fStatementGetShopSellingAndStorageItem_ASC != null) { _fStatementGetShopSellingAndStorageItem_ASC.StatementFinalize(); _fStatementGetShopSellingAndStorageItem_ASC = null; }
        if (_fStatementGetShopSellingAndStorageItem_DESC != null) { _fStatementGetShopSellingAndStorageItem_DESC.StatementFinalize(); _fStatementGetShopSellingAndStorageItem_DESC = null; }
        if (_fStatementGetShopSellingItem_Count != null) { _fStatementGetShopSellingItem_Count.StatementFinalize(); _fStatementGetShopSellingItem_Count = null; }
        if (_fStatementGetShopSelledItem_Count != null) { _fStatementGetShopSelledItem_Count.StatementFinalize(); _fStatementGetShopSelledItem_Count = null; }
        if (_fStatementGetShopStorageItem_Count != null) { _fStatementGetShopStorageItem_Count.StatementFinalize(); _fStatementGetShopStorageItem_Count = null; }
        if (_fStatementGetShopSellingAndStorageItem_Count != null) { _fStatementGetShopSellingAndStorageItem_Count.StatementFinalize(); _fStatementGetShopSellingAndStorageItem_Count = null; }
        if (_fStatementGetAllShop_Sort0 != null) { _fStatementGetAllShop_Sort0.StatementFinalize(); _fStatementGetAllShop_Sort0 = null; }
        if (_fStatementGetAllShop_Sort1 != null) { _fStatementGetAllShop_Sort1.StatementFinalize(); _fStatementGetAllShop_Sort1 = null; }
        if (_fStatementGetAllShop_Sort2 != null) { _fStatementGetAllShop_Sort2.StatementFinalize(); _fStatementGetAllShop_Sort2 = null; }
        if (_fStatementGetAllShop_Sort3 != null) { _fStatementGetAllShop_Sort3.StatementFinalize(); _fStatementGetAllShop_Sort3 = null; }
        if (_fStatementGetAllShop_Sort4 != null) { _fStatementGetAllShop_Sort4.StatementFinalize(); _fStatementGetAllShop_Sort4 = null; }
        if (_fStatementGetAllShop_Sort5 != null) { _fStatementGetAllShop_Sort5.StatementFinalize(); _fStatementGetAllShop_Sort5 = null; }
        if (_fStatementGetAllShop_Count != null) { _fStatementGetAllShop_Count.StatementFinalize(); _fStatementGetAllShop_Count = null; }
        if (_fStatementGetAllShop_Ex != null) { _fStatementGetAllShop_Ex.StatementFinalize(); _fStatementGetAllShop_Ex = null; }
        if (_fStatementGetHumanSellingItem_MakeIndex != null) { _fStatementGetHumanSellingItem_MakeIndex.StatementFinalize(); _fStatementGetHumanSellingItem_MakeIndex = null; }
        if (_fStatementGetHumanSelledItem_MakeIndex != null) { _fStatementGetHumanSelledItem_MakeIndex.StatementFinalize(); _fStatementGetHumanSelledItem_MakeIndex = null; }
        if (_fStatementGetHumanStorageItem_MakeIndex != null) { _fStatementGetHumanStorageItem_MakeIndex.StatementFinalize(); _fStatementGetHumanStorageItem_MakeIndex = null; }
        if (_fStatementGetHumanSellingAndStorageItem_MakeIndex != null) { _fStatementGetHumanSellingAndStorageItem_MakeIndex.StatementFinalize(); _fStatementGetHumanSellingAndStorageItem_MakeIndex = null; }
    }

    /// <summary>SqliteUserShopDB.pas:750-762 <c>DoShopAdd</c>。</summary>
    protected override bool DoShopAdd(string shopName, string humanName)
    {
        if (HumanNameExists(humanName) || ShopNameExists(shopName))
        {
            return false;
        }
        else
        {
            ISqliteStatement s = _fStatementInsertUserShop!;
            s.Reset();
            s.OrderBindText(humanName);
            s.OrderBindText(shopName);
            int stepCode = s.Step();
            bool result = stepCode == SqliteCodes.SQLITE_OK || stepCode == SqliteCodes.SQLITE_DONE;
            s.Reset();
            return result;
        }
    }

    /// <summary>SqliteUserShopDB.pas:764-770 <c>DoHumanNameExists</c>。</summary>
    protected override bool DoHumanNameExists(string humanName)
    {
        ISqliteStatement s = _fStatementHumanNameExists!;
        s.Reset();
        s.OrderBindText(humanName);
        bool result = s.Step() == SqliteCodes.SQLITE_ROW;
        s.Reset();
        return result;
    }

    /// <summary>SqliteUserShopDB.pas:772-778 <c>DoShopNameExists</c>。</summary>
    protected override bool DoShopNameExists(string shopName)
    {
        ISqliteStatement s = _fStatementShopNameExists!;
        s.Reset();
        s.OrderBindText(shopName);
        bool result = s.Step() == SqliteCodes.SQLITE_ROW;
        s.Reset();
        return result;
    }

    /// <summary>SqliteUserShopDB.pas:780-950 <c>DoGetAllShop</c>。</summary>
    protected override int DoGetAllShop(int startIndex, string keyword, bool isKeywordHumanName, int sortType, UserShopList shopList)
    {
        int result = 0;
        try
        {
            if (keyword.Length > 0)
            {
                ISqliteStatement sm = _fdb!.AddSQLStatement("UserShop_GetAllShop");
                try
                {
                    sm.Sql = "SELECT " + "A.ShopID," + "A.HumanName," + "A.ShopName," + "A.IsBusiness," + "A.CreateDate," + "A.CareValue," +
                        "(select count(*) from UserShopItem where shopid = a.shopid and IsAllowSell = 1 and length(ifnull(BuyerName, '')) = 0) as SellItemCount,"
                        +
                        "(select count(*) from UserShopItem where shopid = a.shopid and length(ifnull(BuyerName, '')) > 0) as SelledItemCount,"
                        +
                        "(select count(*) from UserShopItem where shopid = a.shopid and IsAllowSell = 0 and length(ifnull(BuyerName, '')) = 0) as StorageItemCount "
                        + "FROM " + "UserShop A " + "WHERE 1 = 1 ";

                    if (keyword.Length > 0)
                    {
                        if (isKeywordHumanName)
                            sm.Sql = sm.Sql + " and A.HumanName like '%" + keyword + "%'";
                        else
                            sm.Sql = sm.Sql + " and A.ShopName like '%" + keyword + "%'";
                    }

                    if (DbLayerGlobals.Environment.boOfflineCloseMyShop)
                    {
                        sm.Sql = sm.Sql + " and IsBusiness = 1 ";
                    }

                    switch (sortType)
                    {
                        case 0: sm.Sql = sm.Sql + " order by SellItemCount desc "; break;
                        case 1: sm.Sql = sm.Sql + " order by SellItemCount "; break;
                        case 2: sm.Sql = sm.Sql + " order by SelledItemCount desc "; break;
                        case 3: sm.Sql = sm.Sql + " order by SelledItemCount "; break;
                        case 4: sm.Sql = sm.Sql + " order by CareValue desc "; break;
                        case 5: sm.Sql = sm.Sql + " order by CareValue "; break;
                    }

                    sm.Sql = sm.Sql + "limit 8 offset " + IntToStr(startIndex);

                    sm.Prepare();
                    int ret = sm.Step();
                    while (ret == SqliteCodes.SQLITE_ROW)
                    {
                        var userShop = new TUserShop();
                        userShop.ShopID = sm.OrderGetColumnValueInt;
                        userShop.sMasterName = sm.OrderGetColumnValueText;
                        userShop.sShopName = sm.OrderGetColumnValueText;
                        userShop.boBusiness = sm.OrderGetColumnValueBool;
                        userShop.dCreateDate = DelphiDateUtil.UnixToDateTime(sm.OrderGetColumnValueInt64 + 8 * 60 * 60);
                        userShop.nCareValue = sm.OrderGetColumnValueInt;
                        userShop.SellItemCount = sm.OrderGetColumnValueInt;
                        userShop.SelledItemCount = sm.OrderGetColumnValueInt;
                        userShop.StorageItemCount = sm.OrderGetColumnValueInt;

                        shopList.Add(userShop);

                        ret = sm.Step();
                        result++;
                    }
                }
                finally
                {
                    sm.StatementFinalize();
                }
            }
            else
            {
                ISqliteStatement? sm;
                switch (sortType)
                {
                    case 0: sm = _fStatementGetAllShop_Sort0; break;
                    case 1: sm = _fStatementGetAllShop_Sort1; break;
                    case 2: sm = _fStatementGetAllShop_Sort2; break;
                    case 3: sm = _fStatementGetAllShop_Sort3; break;
                    case 4: sm = _fStatementGetAllShop_Sort4; break;
                    case 5: sm = _fStatementGetAllShop_Sort5; break;
                    default: sm = null; break;
                }

                if (sm == null) return result;

                sm.Reset();

                if (DbLayerGlobals.Environment.boOfflineCloseMyShop)
                {
                    sm.OrderBindInt(1);
                    sm.OrderBindInt(1);
                }
                else
                {
                    sm.OrderBindInt(0);
                    sm.OrderBindInt(1);
                }

                sm.OrderBindInt(startIndex);

                int ret = sm.Step();
                while (ret == SqliteCodes.SQLITE_ROW)
                {
                    var userShop = new TUserShop();
                    userShop.ShopID = sm.OrderGetColumnValueInt;
                    userShop.sMasterName = sm.OrderGetColumnValueText;
                    userShop.sShopName = sm.OrderGetColumnValueText;
                    userShop.boBusiness = sm.OrderGetColumnValueBool;
                    userShop.dCreateDate = DelphiDateUtil.UnixToDateTime(sm.OrderGetColumnValueInt64 + 8 * 60 * 60);
                    userShop.nCareValue = sm.OrderGetColumnValueInt;
                    userShop.SellItemCount = sm.OrderGetColumnValueInt;
                    userShop.SelledItemCount = sm.OrderGetColumnValueInt;
                    userShop.StorageItemCount = sm.OrderGetColumnValueInt;

                    shopList.Add(userShop);

                    ret = sm.Step();
                    result++;
                }

                sm.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>SqliteUserShopDB.pas:952-1046 <c>DoGetAllShopCount</c>。</summary>
    protected override int DoGetAllShopCount(string keyword, bool isKeywordHumanName)
    {
        int result = 0;
        try
        {
            if (keyword.Length > 0)
            {
                ISqliteStatement sm = _fdb!.AddSQLStatement("UserShop_GetAllShopCount");
                try
                {
                    sm.Sql = "SELECT " + "Count(*) " + "FROM " + "UserShop " + "WHERE 1 = 1 ";

                    if (keyword.Length > 0)
                    {
                        if (isKeywordHumanName)
                            sm.Sql = sm.Sql + " and HumanName like '%" + keyword + "%'";
                        else
                            sm.Sql = sm.Sql + " and ShopName like '%" + keyword + "%'";
                    }

                    if (DbLayerGlobals.Environment.boOfflineCloseMyShop)
                    {
                        sm.Sql = sm.Sql + " and IsBusiness = 1 ";
                    }

                    sm.Prepare();
                    if (sm.Step() == SqliteCodes.SQLITE_ROW)
                    {
                        result = sm.OrderGetColumnValueInt;
                    }
                }
                finally
                {
                    sm.StatementFinalize();
                }
            }
            else
            {
                ISqliteStatement s = _fStatementGetAllShop_Count!;
                s.Reset();

                if (DbLayerGlobals.Environment.boOfflineCloseMyShop)
                {
                    s.OrderBindInt(1);
                    s.OrderBindInt(1);
                }
                else
                {
                    s.OrderBindInt(0);
                    s.OrderBindInt(1);
                }

                if (s.Step() == SqliteCodes.SQLITE_ROW)
                {
                    result = s.OrderGetColumnValueInt;
                }

                s.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>SqliteUserShopDB.pas:1048-1082 <c>DoGetAllShopEx</c>。</summary>
    protected override int DoGetAllShopEx(UserShopList shopList)
    {
        int result = 0;
        try
        {
            ISqliteStatement s = _fStatementGetAllShop_Ex!;
            s.Reset();
            int ret = s.Step();
            while (ret == SqliteCodes.SQLITE_ROW)
            {
                var userShop = new TUserShop();
                userShop.ShopID = s.OrderGetColumnValueInt;
                userShop.sMasterName = s.OrderGetColumnValueText;
                userShop.sShopName = s.OrderGetColumnValueText;
                userShop.boBusiness = s.OrderGetColumnValueBool;
                userShop.dCreateDate = DelphiDateUtil.UnixToDateTime(s.OrderGetColumnValueInt64 + 8 * 60 * 60);
                userShop.nCareValue = s.OrderGetColumnValueInt;
                userShop.SellItemCount = 0;
                userShop.SelledItemCount = 0;
                userShop.StorageItemCount = 0;

                shopList.Add(userShop);

                ret = s.Step();
                result++;
            }

            s.Reset();
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>SqliteUserShopDB.pas:1084-1359 <c>DoGetSellItems</c>。</summary>
    protected override int DoGetSellItems(bool isMyShop, int startIndex, string humanName, string keyword, int itemType,
        int moneyType, int minPrice, int maxPrice, int sortType, UserShopItemList itemList, TShopItemType shopItemType)
    {
        int result = 0;
        try
        {
            if (keyword.Length > 0)
            {
                ISqliteStatement sm = _fdb!.AddSQLStatement("UserShop_GetAllSellItem");
                try
                {
                    sm.Sql = "SELECT " + "A.ShopID, " + "A.ItemID, " + "A.MoneyType, " + "A.ItemType, " + "A.IsAllowSell, " + "A.ItemPrice, "
                        + "A.CreateDate, " + "A.IsGetMoney," + "A.BuyerName, " + "B.ShopName, " + "B.HumanName " + "FROM " +
                        "UserShopItem A, UserShop B " + "WHERE A.ShopID = B.ShopID ";

                    if (shopItemType == TShopItemType.sitSelling)
                    {
                        if (isMyShop)
                            sm.Sql = sm.Sql + " and (A.IsAllowSell >= 1) and (A.IsAllowSell <= 2) and length(ifnull(A.BuyerName, '')) = 0 ";
                        else
                            sm.Sql = sm.Sql + " and (A.IsAllowSell = 1) and length(ifnull(A.BuyerName, '')) = 0 ";
                    }
                    else if (shopItemType == TShopItemType.sitSelled)
                    {
                        sm.Sql = sm.Sql + " and length(ifnull(A.BuyerName, '')) > 0 ";
                    }
                    else if (shopItemType == TShopItemType.sitStorage)
                    {
                        sm.Sql = sm.Sql + " and (A.IsAllowSell = 0) and length(ifnull(A.BuyerName, '')) = 0 ";
                    }
                    else
                    {
                        sm.Sql = sm.Sql + " and length(ifnull(A.BuyerName, '')) = 0 ";
                    }

                    if (itemType >= 0)
                    {
                        sm.Sql = sm.Sql + " and A.ItemType = " + IntToStr(itemType);
                    }

                    if (moneyType >= 0)
                    {
                        sm.Sql = sm.Sql + " and A.MoneyType = " + IntToStr(moneyType);
                    }

                    if (minPrice <= maxPrice && maxPrice > 0)
                    {
                        sm.Sql = sm.Sql + " and A.ItemPrice >= " + IntToStr(minPrice) + " and A.ItemPrice <= " + IntToStr(maxPrice);
                    }

                    if (keyword.Length > 0)
                    {
                        sm.Sql = sm.Sql + " and ((A.ItemDBName like '%" + keyword + "%'" + ") or (A.ItemName like '%" + keyword + "%'" +
                            "))";
                    }

                    if (humanName.Length > 0)
                    {
                        sm.Sql = sm.Sql + " and B.HumanName = '" + humanName + "'";
                    }

                    if (DbLayerGlobals.Environment.boOfflineCloseMyShop)
                    {
                        sm.Sql = sm.Sql + " and B.IsBusiness = 1 ";
                    }

                    switch (sortType)
                    {
                        case 0: sm.Sql = sm.Sql + " order by A.ItemPrice "; break;
                        case 1: sm.Sql = sm.Sql + " order by A.ItemPrice desc "; break;
                    }

                    sm.Sql = sm.Sql + "limit 5 offset " + IntToStr(startIndex);

                    sm.Prepare();
                    int ret = sm.Step();
                    while (ret == SqliteCodes.SQLITE_ROW)
                    {
                        var shopItem = new TUserShopItem();

                        int shopId = sm.OrderGetColumnValueInt;
                        shopItem.ShopID = shopId;
                        shopItem.ItemID = sm.OrderGetColumnValueInt;
                        shopItem.btMoneyType = (byte)sm.OrderGetColumnValueInt;
                        shopItem.btItemType = (byte)sm.OrderGetColumnValueInt;
                        shopItem.btAllowSell = (byte)sm.OrderGetColumnValueInt;
                        shopItem.nPrice = sm.OrderGetColumnValueInt;
                        shopItem.dCreateDate = DelphiDateUtil.UnixToDateTime(sm.OrderGetColumnValueInt64 + 8 * 60 * 60);
                        shopItem.boGetMoney = sm.OrderGetColumnValueBool;
                        shopItem.sBuyName = sm.OrderGetColumnValueText;
                        shopItem.sShopName = sm.OrderGetColumnValueText;
                        shopItem.sMasterName = sm.OrderGetColumnValueText;

                        Owner.LoadItemFromDB(ref shopItem.UserItem, shopId, DataItemTypes.UserShopItemType, shopItem.ItemID);

                        itemList.Add(shopItem);

                        ret = sm.Step();
                        result++;
                    }
                }
                finally
                {
                    sm.StatementFinalize();
                }
            }
            else
            {
                ISqliteStatement sm;
                if (shopItemType == TShopItemType.sitSelling)
                {
                    sm = sortType == 0 ? _fStatementGetShopSellingItem_ASC! : _fStatementGetShopSellingItem_DESC!;
                    sm.Reset();

                    if (isMyShop)
                    {
                        sm.OrderBindInt(1);
                        sm.OrderBindInt(2);
                    }
                    else
                    {
                        sm.OrderBindInt(1);
                        sm.OrderBindInt(1);
                    }
                }
                else if (shopItemType == TShopItemType.sitSelled)
                {
                    sm = sortType == 0 ? _fStatementGetShopSelledItem_ASC! : _fStatementGetShopSelledItem_DESC!;
                    sm.Reset();
                }
                else if (shopItemType == TShopItemType.sitStorage)
                {
                    sm = sortType == 0 ? _fStatementGetShopStorageItem_ASC! : _fStatementGetShopStorageItem_DESC!;
                    sm.Reset();
                }
                else
                {
                    sm = sortType == 0 ? _fStatementGetShopSellingAndStorageItem_ASC! : _fStatementGetShopSellingAndStorageItem_DESC!;
                    sm.Reset();
                }

                if (itemType >= 0)
                {
                    sm.OrderBindInt(itemType);
                    sm.OrderBindInt(itemType);
                }
                else
                {
                    sm.OrderBindInt(0);
                    sm.OrderBindInt(255);
                }

                if (moneyType >= 0)
                {
                    sm.OrderBindInt(moneyType);
                    sm.OrderBindInt(moneyType);
                }
                else
                {
                    sm.OrderBindInt(0);
                    sm.OrderBindInt(6);
                }

                if (minPrice <= maxPrice && maxPrice > 0)
                {
                    sm.OrderBindInt(minPrice);
                    sm.OrderBindInt(maxPrice);
                }
                else
                {
                    sm.OrderBindInt(0);
                    sm.OrderBindInt(int.MaxValue);
                }

                if (humanName.Length > 0)
                {
                    sm.OrderBindInt(0);
                    sm.OrderBindText(humanName);
                }
                else
                {
                    sm.OrderBindInt(1);
                    sm.OrderBindText("*");
                }

                if (DbLayerGlobals.Environment.boOfflineCloseMyShop)
                {
                    sm.OrderBindInt(1);
                    sm.OrderBindInt(1);
                }
                else
                {
                    sm.OrderBindInt(0);
                    sm.OrderBindInt(1);
                }

                sm.OrderBindInt(startIndex);

                int ret = sm.Step();
                while (ret == SqliteCodes.SQLITE_ROW)
                {
                    var shopItem = new TUserShopItem();

                    int shopId = sm.OrderGetColumnValueInt;
                    shopItem.ShopID = shopId;
                    shopItem.ItemID = sm.OrderGetColumnValueInt;
                    shopItem.btMoneyType = (byte)sm.OrderGetColumnValueInt;
                    shopItem.btItemType = (byte)sm.OrderGetColumnValueInt;
                    shopItem.btAllowSell = (byte)sm.OrderGetColumnValueInt;
                    shopItem.nPrice = sm.OrderGetColumnValueInt;
                    shopItem.dCreateDate = DelphiDateUtil.UnixToDateTime(sm.OrderGetColumnValueInt64 + 8 * 60 * 60);
                    shopItem.boGetMoney = sm.OrderGetColumnValueBool;
                    shopItem.sBuyName = sm.OrderGetColumnValueText;
                    shopItem.sShopName = sm.OrderGetColumnValueText;
                    shopItem.sMasterName = sm.OrderGetColumnValueText;

                    Owner.LoadItemFromDB(ref shopItem.UserItem, shopId, DataItemTypes.UserShopItemType, shopItem.ItemID);

                    itemList.Add(shopItem);

                    ret = sm.Step();
                    result++;
                }

                sm.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>SqliteUserShopDB.pas:1361-1562 <c>DoGetSellItemsCount</c>。
    /// 注意原文 1443-1452 行 bug：IsMyShop 两分支都绑 (1, 2)（与 DoGetSellItems 的 (1,1) 不对称）——逐字保留。</summary>
    protected override int DoGetSellItemsCount(bool isMyShop, string humanName, string keyword, int itemType, int moneyType,
        int minPrice, int maxPrice, TShopItemType shopItemType)
    {
        int result = 0;
        try
        {
            if (keyword.Length > 0)
            {
                ISqliteStatement sm = _fdb!.AddSQLStatement("UserShop_GetAllSellItemCount");
                try
                {
                    sm.Sql = "SELECT " + "Count(A.ShopID) " + "FROM " + "UserShopItem A, UserShop B " + "WHERE A.ShopID = B.ShopID ";

                    if (shopItemType == TShopItemType.sitSelling)
                    {
                        if (isMyShop)
                            sm.Sql = sm.Sql + " and (A.IsAllowSell in (1, 2)) and length(ifnull(A.BuyerName, '')) = 0 ";
                        else
                            sm.Sql = sm.Sql + " and (A.IsAllowSell = 1) and length(ifnull(A.BuyerName, '')) = 0 ";
                    }
                    else if (shopItemType == TShopItemType.sitSelled)
                    {
                        sm.Sql = sm.Sql + " and length(ifnull(A.BuyerName, '')) > 0 ";
                    }
                    else if (shopItemType == TShopItemType.sitStorage)
                    {
                        sm.Sql = sm.Sql + " and (A.IsAllowSell = 0) and length(ifnull(A.BuyerName, '')) = 0 ";
                    }
                    else
                    {
                        sm.Sql = sm.Sql + " and length(ifnull(A.BuyerName, '')) = 0 ";
                    }

                    if (itemType >= 0)
                    {
                        sm.Sql = sm.Sql + " and A.ItemType = " + IntToStr(itemType);
                    }

                    if (moneyType >= 0)
                    {
                        sm.Sql = sm.Sql + " and A.MoneyType = " + IntToStr(moneyType);
                    }

                    if (minPrice <= maxPrice && maxPrice > 0)
                    {
                        sm.Sql = sm.Sql + " and A.ItemPrice >= " + IntToStr(minPrice) + " and A.ItemPrice <= " + IntToStr(maxPrice);
                    }

                    if (keyword.Length > 0)
                    {
                        sm.Sql = sm.Sql + " and ((A.ItemDBName like '%" + keyword + "%'" + ") or (A.ItemName like '%" + keyword + "%'" +
                            "))";
                    }

                    if (humanName.Length > 0)
                    {
                        sm.Sql = sm.Sql + " and B.HumanName = '" + humanName + "'";
                    }

                    if (DbLayerGlobals.Environment.boOfflineCloseMyShop)
                    {
                        sm.Sql = sm.Sql + " and B.IsBusiness = 1 ";
                    }

                    sm.Prepare();
                    if (sm.Step() == SqliteCodes.SQLITE_ROW)
                    {
                        result = sm.OrderGetColumnValueInt;
                    }
                }
                finally
                {
                    sm.StatementFinalize();
                }
            }
            else
            {
                ISqliteStatement sm;
                if (shopItemType == TShopItemType.sitSelling)
                {
                    sm = _fStatementGetShopSellingItem_Count!;
                    sm.Reset();

                    // 原文 1443-1452：两分支完全相同（(1,2) / (1,2)）—— 逐字保留。
                    if (isMyShop)
                    {
                        sm.OrderBindInt(1);
                        sm.OrderBindInt(2);
                    }
                    else
                    {
                        sm.OrderBindInt(1);
                        sm.OrderBindInt(2);
                    }
                }
                else if (shopItemType == TShopItemType.sitSelled)
                {
                    sm = _fStatementGetShopSelledItem_Count!;
                    sm.Reset();
                }
                else if (shopItemType == TShopItemType.sitStorage)
                {
                    sm = _fStatementGetShopStorageItem_Count!;
                    sm.Reset();
                }
                else
                {
                    sm = _fStatementGetShopSellingAndStorageItem_Count!;
                    sm.Reset();
                }

                if (itemType >= 0)
                {
                    sm.OrderBindInt(itemType);
                    sm.OrderBindInt(itemType);
                }
                else
                {
                    sm.OrderBindInt(0);
                    sm.OrderBindInt(255);
                }

                if (moneyType >= 0)
                {
                    sm.OrderBindInt(moneyType);
                    sm.OrderBindInt(moneyType);
                }
                else
                {
                    sm.OrderBindInt(0);
                    sm.OrderBindInt(6);
                }

                if (minPrice <= maxPrice && maxPrice > 0)
                {
                    sm.OrderBindInt(minPrice);
                    sm.OrderBindInt(maxPrice);
                }
                else
                {
                    sm.OrderBindInt(0);
                    sm.OrderBindInt(int.MaxValue);
                }

                if (humanName.Length > 0)
                {
                    sm.OrderBindInt(0);
                    sm.OrderBindText(humanName);
                }
                else
                {
                    sm.OrderBindInt(1);
                    sm.OrderBindText("*");
                }

                if (DbLayerGlobals.Environment.boOfflineCloseMyShop)
                {
                    sm.OrderBindInt(1);
                    sm.OrderBindInt(1);
                }
                else
                {
                    sm.OrderBindInt(0);
                    sm.OrderBindInt(1);
                }

                if (sm.Step() == SqliteCodes.SQLITE_ROW)
                {
                    result = sm.OrderGetColumnValueInt;
                }

                sm.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>SqliteUserShopDB.pas:1564-1669：<c>DoGetHumanItems</c> 整体被花括号注释掉（原文无实现）。</summary>

    /// <summary>SqliteUserShopDB.pas:1671-1773 <c>DoGetHumanItemWithMakeIndex</c>。</summary>
    protected override bool DoGetHumanItemWithMakeIndex(bool isMyShop, string humanName, TShopItemType shopItemType,
        int itemMakeIndex, out TSimpleUserShopItem userShopItem)
    {
        userShopItem = new TSimpleUserShopItem();
        bool result = false;
        try
        {
            ISqliteStatement sm;
            if (shopItemType == TShopItemType.sitSelling)
            {
                sm = _fStatementGetHumanSellingItem_MakeIndex!;

                sm.Reset();

                if (isMyShop)
                {
                    sm.OrderBindInt(1);
                    sm.OrderBindInt(2);

                    sm.OrderBindInt(0);
                    sm.OrderBindInt(1);
                }
                else
                {
                    sm.OrderBindInt(1);
                    sm.OrderBindInt(1);

                    if (DbLayerGlobals.Environment.boOfflineCloseMyShop)
                    {
                        sm.OrderBindInt(1);
                        sm.OrderBindInt(1);
                    }
                    else
                    {
                        sm.OrderBindInt(0);
                        sm.OrderBindInt(1);
                    }
                }
            }
            else if (shopItemType == TShopItemType.sitSelled)
            {
                sm = _fStatementGetHumanSelledItem_MakeIndex!;
                sm.Reset();
            }
            else if (shopItemType == TShopItemType.sitStorage)
            {
                sm = _fStatementGetHumanStorageItem_MakeIndex!;
                sm.Reset();
            }
            else
            {
                sm = _fStatementGetHumanSellingAndStorageItem_MakeIndex!;
                sm.Reset();
            }

            if (humanName.Length > 0)
            {
                sm.OrderBindInt(0);
                sm.OrderBindText(humanName);
            }
            else
            {
                sm.OrderBindInt(1);
                sm.OrderBindText("*");
            }

            sm.OrderBindInt(itemMakeIndex);

            if (sm.Step() == SqliteCodes.SQLITE_ROW)
            {
                userShopItem.ShopID = sm.OrderGetColumnValueInt;
                userShopItem.ItemID = sm.OrderGetColumnValueInt;
                userShopItem.btMoneyType = (byte)sm.OrderGetColumnValueInt;
                userShopItem.btItemType = (byte)sm.OrderGetColumnValueInt;
                userShopItem.btAllowSell = (byte)sm.OrderGetColumnValueInt;
                userShopItem.nPrice = sm.OrderGetColumnValueInt;
                userShopItem.dCreateDate = DelphiDateUtil.UnixToDateTime(sm.OrderGetColumnValueInt64 + 8 * 60 * 60);
                userShopItem.boGetMoney = sm.OrderGetColumnValueBool;
                userShopItem.sBuyName = sm.OrderGetColumnValueText;
                userShopItem.sShopName = sm.OrderGetColumnValueText;
                userShopItem.sMasterName = sm.OrderGetColumnValueText;

                userShopItem.MakeIndex = sm.OrderGetColumnValueInt;
                userShopItem.wIndex = (ushort)sm.OrderGetColumnValueInt;
                userShopItem.boIsBind = sm.OrderGetColumnValueBool;
                userShopItem.btBindOption = (byte)sm.OrderGetColumnValueInt;
                userShopItem.Dura = (ushort)sm.OrderGetColumnValueInt;

                result = true;
            }

            sm.Reset();
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>SqliteUserShopDB.pas:1775-1799 <c>DoGetUserShopInfo</c>（无 except，仅 finally）。</summary>
    protected override bool DoGetUserShopInfo(string humanName, out TUserShop userShop)
    {
        userShop = new TUserShop();
        bool result = false;

        ISqliteStatement s = _fStatementGetUserShopInfo!;
        s.Reset();
        try
        {
            s.OrderBindText(humanName);
            if (s.Step() == SqliteCodes.SQLITE_ROW)
            {
                userShop.ShopID = s.OrderGetColumnValueInt;
                userShop.sMasterName = s.OrderGetColumnValueText;
                userShop.sShopName = s.OrderGetColumnValueText;
                userShop.boBusiness = s.OrderGetColumnValueBool;
                userShop.dCreateDate = DelphiDateUtil.UnixToDateTime(s.OrderGetColumnValueInt64 + 8 * 60 * 60);
                userShop.nCareValue = s.OrderGetColumnValueInt;
                userShop.SellItemCount = s.OrderGetColumnValueInt;
                userShop.SelledItemCount = s.OrderGetColumnValueInt;
                userShop.StorageItemCount = s.OrderGetColumnValueInt;

                result = true;
            }
        }
        finally
        {
            s.Reset();
        }
        return result;
    }

    /// <summary>SqliteUserShopDB.pas:1801-1807 <c>DoIncUserShopCareValue</c>（返回值被丢弃）。</summary>
    protected override void DoIncUserShopCareValue(string humanName)
    {
        ISqliteStatement s = _fStatementIncUserShopCareValue!;
        s.Reset();
        s.OrderBindText(humanName);
        s.Step();
        s.Reset();
    }

    /// <summary>SqliteUserShopDB.pas:1809-1865 <c>DoAddItem</c>。</summary>
    protected override bool DoAddItem(int shopId, TUserShopItem shopItem, string sItemName)
    {
        bool result = false;
        int itemId;
        try
        {
            ISqliteStatement maxStmt = _fStatementGetMaxShopItemID!;
            maxStmt.Reset();
            maxStmt.OrderBindInt(shopId);
            if (maxStmt.Step() == SqliteCodes.SQLITE_ROW)
                itemId = maxStmt.OrderGetColumnValueInt;
            else
                itemId = 1;
        }
        finally
        {
            _fStatementGetMaxShopItemID!.Reset();
        }

        ISqliteStatement s = _fStatementInsertUserShopItem!;
        s.Reset();
        try
        {
            _fdb!.BeginTransaction();
            try
            {
                s.OrderBindInt(shopId);
                s.OrderBindInt(itemId);
                s.OrderBindInt(shopItem.btItemType);
                s.OrderBindInt64(DelphiDateUtil.DateTimeToUnix(shopItem.dCreateDate) - 8 * 60 * 60);
                s.OrderBindInt(shopItem.btAllowSell);
                s.OrderBindInt(shopItem.btMoneyType);
                s.OrderBindInt(shopItem.nPrice);
                s.OrderBindBool(shopItem.boGetMoney);
                s.OrderBindText(shopItem.sBuyName.Value);

                s.OrderBindText(sItemName);

                string changeName = "";
                if (shopItem.UserItem.GetBtValue(13) == 1 && shopItem.UserItem.NameStr.Length > 0)
                    changeName = ProcessItemName(shopItem.UserItem.NameStr);

                s.OrderBindText(changeName);

                int stepCode = s.Step();
                if (stepCode == SqliteCodes.SQLITE_OK || stepCode == SqliteCodes.SQLITE_DONE)
                {
                    Owner.SaveItemToDB(shopItem.UserItem, shopId, DataItemTypes.UserShopItemType, itemId);
                    result = true;
                }

                _fdb!.Commit();
            }
            catch (Exception e)
            {
                DbLayerGlobals.MainOutMessage(e.Message);
                _fdb!.RollBack();
            }
        }
        finally
        {
            s.Reset();
        }
        return result;
    }

    /// <summary>SqliteUserShopDB.pas:1867-1885 <c>DoUpdateItem</c>。</summary>
    protected override bool DoUpdateItem(int shopId, int itemId, int btItemType, int btAllowSell, int btMoneyType, int nPrice)
    {
        ISqliteStatement s = _fStatementUpdateUserShopItem!;
        s.Reset();
        try
        {
            s.OrderBindInt(btItemType);
            s.OrderBindInt(btAllowSell);
            s.OrderBindInt(btMoneyType);
            s.OrderBindInt(nPrice);
            s.OrderBindInt(shopId);
            s.OrderBindInt(itemId);

            int stepCode = s.Step();
            return stepCode == SqliteCodes.SQLITE_OK || stepCode == SqliteCodes.SQLITE_DONE;
        }
        finally
        {
            s.Reset();
        }
    }

    /// <summary>SqliteUserShopDB.pas:1887-1922 <c>DoDeleteItem</c>（多语句脚本，sLineBreak 分隔）。</summary>
    protected override bool DoDeleteItem(int shopId, int itemId)
    {
        bool result = false;

        _fdb!.BeginTransaction();
        try
        {
            string s = "delete from ItemElementAdd where ItemType = " + IntToStr(DataItemTypes.UserShopItemType) + " and ParentID = " + IntToStr(shopId) +
                " and ItemIndex = " + IntToStr(itemId) + ";" + LineBreak + "delete from ItemAddDataByte where ItemType = " + IntToStr(DataItemTypes.UserShopItemType)
                + " and ParentID = " + IntToStr(shopId) + " and ItemIndex = " + IntToStr(itemId) + ";" + LineBreak +
                "delete from ItemAddDataInt where ItemType = " + IntToStr(DataItemTypes.UserShopItemType) + " and ParentID = " + IntToStr(shopId) +
                " and ItemIndex = " + IntToStr(itemId) + ";" + LineBreak + "delete from ItemAddDataText where ItemType = " + IntToStr(DataItemTypes.UserShopItemType)
                + " and ParentID = " + IntToStr(shopId) + " and ItemIndex = " + IntToStr(itemId) + ";" + LineBreak +
                "delete from ItemFlute where ItemType = " + IntToStr(DataItemTypes.UserShopItemType) + " and ParentID = " + IntToStr(shopId) +
                " and ItemIndex = " + IntToStr(itemId) + ";" + LineBreak + "delete from ItemProgress where ItemType = " + IntToStr(DataItemTypes.UserShopItemType)
                + " and ParentID = " + IntToStr(shopId) + " and ItemIndex = " + IntToStr(itemId) + ";" + LineBreak +
                "delete from ItemProperty where ItemType = " + IntToStr(DataItemTypes.UserShopItemType) + " and ParentID = " + IntToStr(shopId) +
                " and ItemIndex = " + IntToStr(itemId) + ";" + LineBreak + "delete from ItemValueAdd where ItemType = " + IntToStr(DataItemTypes.UserShopItemType)
                + " and ParentID = " + IntToStr(shopId) + " and ItemIndex = " + IntToStr(itemId) + ";" + LineBreak +
                "delete from Items where ItemType = " + IntToStr(DataItemTypes.UserShopItemType) + " and ParentID = " + IntToStr(shopId) +
                " and ItemIndex = " + IntToStr(itemId) + ";" + LineBreak + "-------------------------------" + LineBreak +
                "delete from UserShopItem where ShopID = " + IntToStr(shopId) + " and ItemID = " + IntToStr(itemId) + ";";

            _fdb!.Execute(s);

            result = true;
            _fdb!.Commit();
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
            _fdb!.RollBack();
        }
        return result;
    }

    /// <summary>SqliteUserShopDB.pas:1924-1936 <c>DoBuyItem</c>。</summary>
    protected override bool DoBuyItem(int shopId, int itemId, string buyer)
    {
        ISqliteStatement s = _fStatementBuyUserShopItem!;
        s.Reset();
        try
        {
            s.OrderBindText(buyer);
            s.OrderBindInt(shopId);
            s.OrderBindInt(itemId);

            int stepCode = s.Step();
            return stepCode == SqliteCodes.SQLITE_OK || stepCode == SqliteCodes.SQLITE_DONE;
        }
        finally
        {
            s.Reset();
        }
    }

    /// <summary>SqliteUserShopDB.pas:1938-1949 <c>DoGetMoneyItem</c>。</summary>
    protected override bool DoGetMoneyItem(int shopId, int itemId)
    {
        ISqliteStatement s = _fStatementGetMoneyShopItem!;
        s.Reset();
        try
        {
            s.OrderBindInt(shopId);
            s.OrderBindInt(itemId);

            int stepCode = s.Step();
            return stepCode == SqliteCodes.SQLITE_OK || stepCode == SqliteCodes.SQLITE_DONE;
        }
        finally
        {
            s.Reset();
        }
    }

    /// <summary>SqliteUserShopDB.pas:1951-1976 <c>DoGetSelledAndNoGetMoneyTotal</c>。</summary>
    protected override int DoGetSelledAndNoGetMoneyTotal(System.Collections.Generic.List<TSelledAndNoGetMoneyTotal> itemList)
    {
        int result = 0;
        ISqliteStatement s = _fStatementGetSelledAndNoGetMoneyTotal!;
        s.Reset();
        try
        {
            int ret = s.Step();
            while (ret == SqliteCodes.SQLITE_ROW)
            {
                var item = new TSelledAndNoGetMoneyTotal();
                itemList.Add(item);

                item.sMasterName = s.OrderGetColumnValueText;
                item.btMoneyType = (byte)s.OrderGetColumnValueInt;
                item.nSumPrice = s.OrderGetColumnValueInt64;

                ret = s.Step();

                result++;
            }
        }
        finally
        {
            s.Reset();
        }
        return result;
    }

    /// <summary>SqliteUserShopDB.pas:1978-2010 <c>DoShopDelete</c>。</summary>
    protected override bool DoShopDelete(int shopId)
    {
        bool result = false;

        _fdb!.BeginTransaction();
        try
        {
            string s = "delete from ItemElementAdd where ItemType = " + IntToStr(DataItemTypes.UserShopItemType) + " and ParentID = " + IntToStr(shopId) +
                ";" + LineBreak + "delete from ItemAddDataByte where ItemType = " + IntToStr(DataItemTypes.UserShopItemType) + " and ParentID = " +
                IntToStr(shopId) + ";" + LineBreak + "delete from ItemAddDataInt where ItemType = " + IntToStr(DataItemTypes.UserShopItemType) +
                " and ParentID = " + IntToStr(shopId) + ";" + LineBreak + "delete from ItemAddDataText where ItemType = " + IntToStr(DataItemTypes.UserShopItemType)
                + " and ParentID = " + IntToStr(shopId) + ";" + LineBreak + "delete from ItemFlute where ItemType = " + IntToStr(DataItemTypes.UserShopItemType)
                + " and ParentID = " + IntToStr(shopId) + ";" + LineBreak + "delete from ItemProgress where ItemType = " + IntToStr(DataItemTypes.UserShopItemType)
                + " and ParentID = " + IntToStr(shopId) + ";" + LineBreak + "delete from ItemProperty where ItemType = " + IntToStr(DataItemTypes.UserShopItemType)
                + " and ParentID = " + IntToStr(shopId) + ";" + LineBreak + "delete from ItemValueAdd where ItemType = " + IntToStr(DataItemTypes.UserShopItemType)
                + " and ParentID = " + IntToStr(shopId) + ";" + LineBreak + "delete from Items where ItemType = " + IntToStr(DataItemTypes.UserShopItemType)
                + " and ParentID = " + IntToStr(shopId) + ";" + LineBreak + "-------------------------------" + LineBreak +
                "delete from UserShopItem where ShopID = " + IntToStr(shopId) + ";" + LineBreak + "delete from UserShop where ShopID = " +
                IntToStr(shopId) + ";";

            _fdb!.Execute(s);
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

    /// <summary>SqliteUserShopDB.pas:2012-2022 <c>DoShopRename</c>。</summary>
    protected override bool DoShopRename(int shopId, string newShopName)
    {
        ISqliteStatement s = _fStatementUserShopRename!;
        s.Reset();
        try
        {
            s.OrderBindText(newShopName);
            s.OrderBindInt(shopId);
            int stepCode = s.Step();
            return stepCode == SqliteCodes.SQLITE_OK || stepCode == SqliteCodes.SQLITE_DONE;
        }
        finally
        {
            s.Reset();
        }
    }

    /// <summary>SqliteUserShopDB.pas:2024-2055 <c>DoHumanRename</c>。</summary>
    protected override bool DoHumanRename(string oldName, string newName)
    {
        bool result = false;

        ISqliteStatement rename2 = _fStatementUserShopRename2!;
        ISqliteStatement buyerRename = _fStatementShopItemBuyerRenameName!;
        rename2.Reset();
        buyerRename.Reset();
        try
        {
            _fdb!.BeginTransaction();
            try
            {
                rename2.OrderBindText(newName);
                rename2.OrderBindText(oldName);
                rename2.Step();

                buyerRename.OrderBindText(newName);
                buyerRename.OrderBindText(oldName);
                buyerRename.Step();

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
            rename2.Reset();
            buyerRename.Reset();
        }
        return result;
    }

    /// <summary>SqliteUserShopDB.pas:2057-2067 <c>DoUpdateHumanBusiness</c>（判据是 =SQLITE_ROW，非 OK/DONE）。</summary>
    protected override bool DoUpdateHumanBusiness(string humanName, bool isBusiness)
    {
        ISqliteStatement s = _fStatementUpdateHumanBusiness!;
        s.Reset();
        try
        {
            s.OrderBindBool(isBusiness);
            s.OrderBindText(humanName);
            return s.Step() == SqliteCodes.SQLITE_ROW;
        }
        finally
        {
            s.Reset();
        }
    }

    /// <summary>SqliteUserShopDB.pas:2069-2158 <c>DoRun</c>。</summary>
    protected override void DoRun()
    {
        if (!DbLayerGlobals.Environment.boEnabledMySellShopItemTime)
            return;

        var itemList = new System.Collections.Generic.List<TSimpleUserShopItem>();
        ISqliteStatement s = _fStatementGetTimeHasArrivedSellItems!;
        s.Reset();
        try
        {
            s.OrderBindInt(DbLayerGlobals.Environment.nMySellShopItemTime * 60);
            int ret = s.Step();
            while (ret == SqliteCodes.SQLITE_ROW)
            {
                var shopItem = new TSimpleUserShopItem();
                itemList.Add(shopItem);

                shopItem.ShopID = s.OrderGetColumnValueInt;
                shopItem.ItemID = s.OrderGetColumnValueInt;
                shopItem.btMoneyType = (byte)s.OrderGetColumnValueInt;
                shopItem.btItemType = (byte)s.OrderGetColumnValueInt;
                shopItem.btAllowSell = (byte)s.OrderGetColumnValueInt;
                shopItem.nPrice = s.OrderGetColumnValueInt;
                shopItem.dCreateDate = DelphiDateUtil.UnixToDateTime(s.OrderGetColumnValueInt64 + 8 * 60 * 60);
                shopItem.boGetMoney = s.OrderGetColumnValueBool;
                shopItem.sBuyName = s.OrderGetColumnValueText;
                shopItem.sShopName = s.OrderGetColumnValueText;
                shopItem.sMasterName = s.OrderGetColumnValueText;

                shopItem.MakeIndex = s.OrderGetColumnValueInt;
                shopItem.wIndex = (ushort)s.OrderGetColumnValueInt;
                shopItem.boIsBind = s.OrderGetColumnValueBool;
                shopItem.btBindOption = (byte)s.OrderGetColumnValueInt;
                shopItem.Dura = (ushort)s.OrderGetColumnValueInt;

                ret = s.Step();
            }
        }
        finally
        {
            s.Reset();
        }

        foreach (TSimpleUserShopItem shopItem in itemList)
        {
            // 接缝：UserEngine.GetStdItem / g_ItemRules / AddGameDataLog / g_M2DataDB 未移植
            //      （见 DbLayerRunSeam）。原文此处为：
            //        StdItem := UserEngine.GetStdItem(ShopItem.wIndex);
            //        if StdItem <> nil then begin ... end;
            var stdItem = DbLayerRunSeam.GetStdItem(shopItem.wIndex);
            if (stdItem == null) continue;

            bool isItemTimeExpired = false;
            if ((GetUserItemBindValue(shopItem.btBindOption, UserItemBindValue.ubNoStorage) && shopItem.boIsBind)
                || DbLayerRunSeam.GetItemRule(shopItem.wIndex, 9))
            {
                // 禁止存仓库
                isItemTimeExpired = true;
            }

            // 检测是否存满
            if (!isItemTimeExpired
                && GetSellItemsCount(false, shopItem.sMasterName.Value, "", -1, -1, 0, 0, TShopItemType.sitStorage)
                   >= DbLayerGlobals.Environment.nMaxMyShopStorageItemCount)
            {
                isItemTimeExpired = true;
            }

            if (isItemTimeExpired)
                shopItem.btAllowSell = 2;
            else
                shopItem.btAllowSell = 0;

            if (DbLayerRunSeam.UpdateShopItem(shopItem.ShopID, shopItem.ItemID, shopItem.btItemType, shopItem.btAllowSell,
                shopItem.btMoneyType, shopItem.nPrice))
            {
                if (!isItemTimeExpired && stdItem.Value.NeedIdentify == 1)
                {
                    DbLayerRunSeam.AddGameDataLogItemMove(stdItem.Value.NameStr, shopItem.MakeIndex,
                        shopItem.sMasterName.Value, "店铺->店铺仓库[到时物品]");
                }
            }
        }
    }

    /// <summary>M2Share.pas <c>ProcessItemName(s: string): string</c>（Grobal2.pas:6380）。
    /// 接缝：待 Grobal2 的 ProcessItemName 移植后接入（原文按物品名映射"改名"规则）。</summary>
    private static string ProcessItemName(string name) => DbLayerRunSeam.ProcessItemName(name);

    /// <summary>原文 <c>sLineBreak</c>（Delphi 平台换行，Windows = CRLF）。</summary>
    private static string LineBreak => "\r\n";

    /// <summary>原文 <c>IntToStr</c>。</summary>
    private static string IntToStr(int value) => GXX.Core.Rtl.DelphiRTL.IntToStr(value);

    /// <summary>M2Share/ObjBase <c>GetUserItemBindValue</c> 的绑定位枚举（原文 ubNoStorage = 禁止存）。</summary>
    private static class UserItemBindValue
    {
        public const int ubNoStorage = 3;
    }

    /// <summary>原文 <c>GetUserItemBindValue(btBindOption, ubNoStorage)</c>：判定绑定选项位。
    /// 接缝：待 ObjBase/M2Share 的 GetUserItemBindValue 移植后接入。</summary>
    private static bool GetUserItemBindValue(int bindOption, int bit)
        => DbLayerRunSeam.GetUserItemBindValue(bindOption, bit);
}
