// 源单元：Source/M2Engine/MySqlUserShopDB.pas（1-2190 行，1:1 移植）
//   TMySqlUserShopDB = class(TUserShopDB)：DoInit / DoFinal / 20 个 Do* 覆写 + Create/Destroy。
//
// SQL 文本**逐字**保留。与 SqliteUserShopDB 的差异面（已用 _recon/diff-sql.mjs 全量对账：
// 44 条语句、32 条逐字相同、12 条不同）：
//   1) FStatementUpdateUserShopItem / FStatementBuyUserShopItem：
//        SQLite  CreateDate = (strftime(''%s'', ''now''))   MySQL  CreateDate = CURRENT_TIMESTAMP
//   2) FStatementGetTimeHasArrivedSellItems：
//        SQLite  ... and (strftime("%s", "now")) - A.createdate > ?
//        MySQL   ... and TIMESTAMPDIFF(SECOND, A.createdate, CURRENT_TIMESTAMP) > ?
//   3) FStatementGetUserShopInfo / GetAllShop_Sort0..5 / SGetAllShopQueryField：
//        SQLite 子查询用 shopid = a.shopid（小写相关子查询）
//        MySQL  子查询用 ShopID = A.ShopID（大写）
//      —— 这是**本项目里唯一的非方言差异**（原文如此，逐字保留）。
//   4) MySQL 独有 2 条语句：FStatementSetTimeHasArrivedSellItems（DoRun 用它把到时物品
//      直接置 IsAllowSell=0）、FStatementGetItemBindOption（DoRun 用 MakeIndex 查 Items 取绑定）。
//   5) FStatementGetAllShop_Sort5 在**两个**文件里都没有 Prepare（逐字保留该不对称）。
//
// 方言要点：事务 StartTransaction/Commit/RollBack；批量脚本 Exec(sql)；时间用
// OrderBindParamDateTime / OrderGetColumnValueDateTime（不是 SQLite 的 Unix 秒 + 8h）；
// 结果集必须 `if X.Query then while (X.Fetch) do`；无结果集语句成功判据是 `Step`（Boolean）。

using System;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;

namespace GXX.M2Server.DbLayer;

/// <summary>MySqlUserShopDB.pas:16-128 <c>TMySqlUserShopDB</c>。</summary>
public sealed class TMySqlUserShopDB : TUserShopDB
{
    private IMySqlDatabase? _fdb;

    private IMySqlStatement? _fStatementUpdateAllBusiness;
    private IMySqlStatement? _fStatementUpdateHumanBusiness;

    private IMySqlStatement? _fStatementHumanNameExists;
    private IMySqlStatement? _fStatementShopNameExists;
    private IMySqlStatement? _fStatementInsertUserShop;
    private IMySqlStatement? _fStatementUserShopRename;
    private IMySqlStatement? _fStatementUserShopRename2;
    private IMySqlStatement? _fStatementShopItemBuyerRenameName;

    private IMySqlStatement? _fStatementGetUserShopInfo;
    private IMySqlStatement? _fStatementIncUserShopCareValue;
    private IMySqlStatement? _fStatementGetMaxShopItemID;
    private IMySqlStatement? _fStatementInsertUserShopItem;
    private IMySqlStatement? _fStatementUpdateUserShopItem;
    private IMySqlStatement? _fStatementBuyUserShopItem;
    private IMySqlStatement? _fStatementGetMoneyShopItem;
    private IMySqlStatement? _fStatementGetSelledAndNoGetMoneyTotal;

    private IMySqlStatement? _fStatementGetTimeHasArrivedSellItems;
    private IMySqlStatement? _fStatementSetTimeHasArrivedSellItems;

    private IMySqlStatement? _fStatementGetShopSellingItem_ASC;
    private IMySqlStatement? _fStatementGetShopSellingItem_DESC;

    private IMySqlStatement? _fStatementGetShopSelledItem_ASC;
    private IMySqlStatement? _fStatementGetShopSelledItem_DESC;

    private IMySqlStatement? _fStatementGetShopStorageItem_ASC;
    private IMySqlStatement? _fStatementGetShopStorageItem_DESC;

    private IMySqlStatement? _fStatementGetShopSellingAndStorageItem_ASC;
    private IMySqlStatement? _fStatementGetShopSellingAndStorageItem_DESC;

    private IMySqlStatement? _fStatementGetShopSellingItem_Count;
    private IMySqlStatement? _fStatementGetShopSelledItem_Count;
    private IMySqlStatement? _fStatementGetShopStorageItem_Count;
    private IMySqlStatement? _fStatementGetShopSellingAndStorageItem_Count;

    private IMySqlStatement? _fStatementGetAllShop_Sort0;
    private IMySqlStatement? _fStatementGetAllShop_Sort1;
    private IMySqlStatement? _fStatementGetAllShop_Sort2;
    private IMySqlStatement? _fStatementGetAllShop_Sort3;
    private IMySqlStatement? _fStatementGetAllShop_Sort4;
    private IMySqlStatement? _fStatementGetAllShop_Sort5;

    private IMySqlStatement? _fStatementGetAllShop_Count;

    private IMySqlStatement? _fStatementGetAllShop_Ex;

    private IMySqlStatement? _fStatementGetHumanSellingItem_MakeIndex;
    private IMySqlStatement? _fStatementGetHumanSelledItem_MakeIndex;
    private IMySqlStatement? _fStatementGetHumanStorageItem_MakeIndex;
    private IMySqlStatement? _fStatementGetHumanSellingAndStorageItem_MakeIndex;

    /// <summary>MySqlUserShopDB.pas:80 —— MySQL 独有的第 21 条语句。</summary>
    private IMySqlStatement? _fStatementGetItemBindOption;

    /// <summary>MySqlUserShopDB.pas:137-202 <c>constructor Create</c>。</summary>
    public TMySqlUserShopDB(IDbLayerHost owner) : base(owner)
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
        _fStatementSetTimeHasArrivedSellItems = null;

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

        _fStatementGetItemBindOption = null;
    }

    /// <summary>MySqlUserShopDB.pas:209-492 <c>DoInit</c>。</summary>
    public override void DoInit()
    {
        // 原文 232 行 inherited;

        // 原文 234-235：if (Owner.DataBase <> nil) and (Owner.DataBase is TMySqlDatabase) then FDB := Owner.DataBase as TMySqlDatabase;
        if (Owner.DataBase is IMySqlDatabase mySqlDb) _fdb = mySqlDb;

        IMySqlDatabase fdb = _fdb!;

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
        _fStatementGetUserShopInfo.Sql = MySqlUserShopStatements.UserShop_GetUserShopInfo;
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
            "update UserShopItem set CreateDate = CURRENT_TIMESTAMP, ItemType = ?, IsAllowSell = ?, MoneyType = ?, ItemPrice = ? where length(ifnull(BuyerName, '''')) = 0 and ShopID = ? and ItemID = ?";
        _fStatementUpdateUserShopItem.Prepare();

        _fStatementBuyUserShopItem = fdb.AddSQLStatement("UserShop_BuyShopItem");
        _fStatementBuyUserShopItem.Sql =
            "update UserShopItem set CreateDate = CURRENT_TIMESTAMP, BuyerName = ? where length(ifnull(BuyerName, '''')) = 0 and ShopID = ? and ItemID = ?";
        _fStatementBuyUserShopItem.Prepare();

        _fStatementGetMoneyShopItem = fdb.AddSQLStatement("UserShop_GetMoneyShopItem");
        _fStatementGetMoneyShopItem.Sql =
            "update UserShopItem set IsGetMoney = 1 where length(ifnull(BuyerName, '''')) > 0 and ShopID = ? and ItemID = ?";
        _fStatementGetMoneyShopItem.Prepare();

        _fStatementGetSelledAndNoGetMoneyTotal = fdb.AddSQLStatement("UserShop_GetSelledAndNoGetMoneyTotal");
        _fStatementGetSelledAndNoGetMoneyTotal.Sql = "SELECT " + "B.HumanName, " + "A.MoneyType, " + "Sum(A.ItemPrice) SumPrice " +
            "FROM " + "UserShopItem A, " + "UserShop B " + "WHERE " +
            "A.ShopID = B.ShopID and length(ifnull(A.BuyerName, '''')) > 0 and IsGetMoney = 0 " + "GROUP BY B.HumanName, A.MoneyType " +
            "ORDER BY B.HumanName";
        _fStatementGetSelledAndNoGetMoneyTotal.Prepare();

        _fStatementGetTimeHasArrivedSellItems = fdb.AddSQLStatement("UserShop_GetTimeHasArrivedSellItems");
        _fStatementGetTimeHasArrivedSellItems.Sql = MySqlUserShopStatements.UserShop_GetTimeHasArrivedSellItems;
        _fStatementGetTimeHasArrivedSellItems.Prepare();

        _fStatementSetTimeHasArrivedSellItems = fdb.AddSQLStatement("UserShop_SetTimeHasArrivedSellItems");
        _fStatementSetTimeHasArrivedSellItems.Sql = "UPDATE UserShopItem set " + "IsAllowSell = 0 " +
            "WHERE (IsAllowSell = 1) and length(ifnull(BuyerName, '''')) = 0 and TIMESTAMPDIFF(SECOND, CreateDate, CURRENT_TIMESTAMP) > ?";
        _fStatementSetTimeHasArrivedSellItems.Prepare();

        _fStatementGetShopSellingItem_ASC = fdb.AddSQLStatement("UserShop_GetSellingItem_ASC");
        _fStatementGetShopSellingItem_ASC.Sql = MySqlUserShopStatements.UserShop_GetSellingItem_ASC;
        _fStatementGetShopSellingItem_ASC.Prepare();

        _fStatementGetShopSellingItem_DESC = fdb.AddSQLStatement("UserShop_GetSellingItem_DESC");
        _fStatementGetShopSellingItem_DESC.Sql = MySqlUserShopStatements.UserShop_GetSellingItem_DESC;
        _fStatementGetShopSellingItem_DESC.Prepare();

        _fStatementGetShopSelledItem_ASC = fdb.AddSQLStatement("UserShop_GetSelledItem_ASC");
        _fStatementGetShopSelledItem_ASC.Sql = MySqlUserShopStatements.UserShop_GetSelledItem_ASC;
        _fStatementGetShopSelledItem_ASC.Prepare();

        _fStatementGetShopSelledItem_DESC = fdb.AddSQLStatement("UserShop_GetSelledItem_DESC");
        _fStatementGetShopSelledItem_DESC.Sql = MySqlUserShopStatements.UserShop_GetSelledItem_DESC;
        _fStatementGetShopSelledItem_DESC.Prepare();

        _fStatementGetShopStorageItem_ASC = fdb.AddSQLStatement("UserShop_GetStorageItem_ASC");
        _fStatementGetShopStorageItem_ASC.Sql = MySqlUserShopStatements.UserShop_GetStorageItem_ASC;
        _fStatementGetShopStorageItem_ASC.Prepare();

        _fStatementGetShopStorageItem_DESC = fdb.AddSQLStatement("UserShop_GetStorageItem_DESC");
        _fStatementGetShopStorageItem_DESC.Sql = MySqlUserShopStatements.UserShop_GetStorageItem_DESC;
        _fStatementGetShopStorageItem_DESC.Prepare();

        _fStatementGetShopSellingAndStorageItem_ASC = fdb.AddSQLStatement("UserShop_GetSellingAndStorageItem_ASC");
        _fStatementGetShopSellingAndStorageItem_ASC.Sql = MySqlUserShopStatements.UserShop_GetSellingAndStorageItem_ASC;
        _fStatementGetShopSellingAndStorageItem_ASC.Prepare();

        _fStatementGetShopSellingAndStorageItem_DESC = fdb.AddSQLStatement("UserShop_GetSellingAndStorageItem_DESC");
        _fStatementGetShopSellingAndStorageItem_DESC.Sql = MySqlUserShopStatements.UserShop_GetSellingAndStorageItem_DESC;
        _fStatementGetShopSellingAndStorageItem_DESC.Prepare();

        _fStatementGetShopSellingItem_Count = fdb.AddSQLStatement("UserShop_GetSellingItem_Count");
        _fStatementGetShopSellingItem_Count.Sql = MySqlUserShopStatements.UserShop_GetSellingItem_Count;
        _fStatementGetShopSellingItem_Count.Prepare();

        _fStatementGetShopSelledItem_Count = fdb.AddSQLStatement("UserShop_GetSelledItem_Count");
        _fStatementGetShopSelledItem_Count.Sql = MySqlUserShopStatements.UserShop_GetSelledItem_Count;
        _fStatementGetShopSelledItem_Count.Prepare();

        _fStatementGetShopStorageItem_Count = fdb.AddSQLStatement("UserShop_GetStorageItem_Count");
        _fStatementGetShopStorageItem_Count.Sql = MySqlUserShopStatements.UserShop_GetStorageItem_Count;
        _fStatementGetShopStorageItem_Count.Prepare();

        _fStatementGetShopSellingAndStorageItem_Count = fdb.AddSQLStatement("UserShop_GetSellingAndStorageItem_Count");
        _fStatementGetShopSellingAndStorageItem_Count.Sql = MySqlUserShopStatements.UserShop_GetSellingAndStorageItem_Count;
        _fStatementGetShopSellingAndStorageItem_Count.Prepare();

        _fStatementGetAllShop_Sort0 = fdb.AddSQLStatement("UserShop_GetAllShop_S0");
        _fStatementGetAllShop_Sort0.Sql = MySqlUserShopStatements.UserShop_GetAllShop_S0;
        _fStatementGetAllShop_Sort0.Prepare();

        _fStatementGetAllShop_Sort1 = fdb.AddSQLStatement("UserShop_GetAllShop_S1");
        _fStatementGetAllShop_Sort1.Sql = MySqlUserShopStatements.UserShop_GetAllShop_S1;
        _fStatementGetAllShop_Sort1.Prepare();

        _fStatementGetAllShop_Sort2 = fdb.AddSQLStatement("UserShop_GetAllShop_S2");
        _fStatementGetAllShop_Sort2.Sql = MySqlUserShopStatements.UserShop_GetAllShop_S2;
        _fStatementGetAllShop_Sort2.Prepare();

        _fStatementGetAllShop_Sort3 = fdb.AddSQLStatement("UserShop_GetAllShop_S3");
        _fStatementGetAllShop_Sort3.Sql = MySqlUserShopStatements.UserShop_GetAllShop_S3;
        _fStatementGetAllShop_Sort3.Prepare();

        _fStatementGetAllShop_Sort4 = fdb.AddSQLStatement("UserShop_GetAllShop_S4");
        _fStatementGetAllShop_Sort4.Sql = MySqlUserShopStatements.UserShop_GetAllShop_S4;
        _fStatementGetAllShop_Sort4.Prepare();

        // 原文 414 行：Sort5 只赋 Sql，没有 Prepare（与 SQLite 版同样不对称）。
        _fStatementGetAllShop_Sort5 = fdb.AddSQLStatement("UserShop_GetAllShop_S5");
        _fStatementGetAllShop_Sort5.Sql = MySqlUserShopStatements.UserShop_GetAllShop_S5;

        _fStatementGetAllShop_Count = fdb.AddSQLStatement("UserShop_GetAllShop_Count_S");
        _fStatementGetAllShop_Count.Sql = "SELECT " + "Count(*) " + "FROM " + "UserShop " + "WHERE " +
            " (IsBusiness >= ?) and (IsBusiness <= ?) ";
        _fStatementGetAllShop_Count.Prepare();

        _fStatementGetAllShop_Ex = fdb.AddSQLStatement("UserShop_GetAllShop_Ex");
        _fStatementGetAllShop_Ex.Sql = "SELECT " + "A.ShopID," + "A.HumanName," + "A.ShopName," + "A.IsBusiness," + "A.CreateDate," +
            "A.CareValue " + "FROM " + "UserShop A ";
        _fStatementGetAllShop_Ex.Prepare();

        // 原文 427-460 行花括号注释掉的 4 条 FStatementGetHumanSellingItem/Selled/Storage/SellingAndStorageItem。

        _fStatementGetHumanSellingItem_MakeIndex = fdb.AddSQLStatement("UserShop_GetHumanSellingItem_MakeIndex");
        _fStatementGetHumanSellingItem_MakeIndex.Sql = MySqlUserShopStatements.UserShop_GetHumanSellingItem_MakeIndex;
        _fStatementGetHumanSellingItem_MakeIndex.Prepare();

        _fStatementGetHumanSelledItem_MakeIndex = fdb.AddSQLStatement("UserShop_GetHumanSelledItem_MakeIndex");
        _fStatementGetHumanSelledItem_MakeIndex.Sql = MySqlUserShopStatements.UserShop_GetHumanSelledItem_MakeIndex;
        _fStatementGetHumanSelledItem_MakeIndex.Prepare();

        _fStatementGetHumanStorageItem_MakeIndex = fdb.AddSQLStatement("UserShop_GetHumanStorageItem_MakeIndex");
        _fStatementGetHumanStorageItem_MakeIndex.Sql = MySqlUserShopStatements.UserShop_GetHumanStorageItem_MakeIndex;
        _fStatementGetHumanStorageItem_MakeIndex.Prepare();

        _fStatementGetHumanSellingAndStorageItem_MakeIndex = fdb.AddSQLStatement("UserShop_GetHumanSellingAndStorageItem_MakeIndex");
        _fStatementGetHumanSellingAndStorageItem_MakeIndex.Sql = MySqlUserShopStatements.UserShop_GetHumanSellingAndStorageItem_MakeIndex;
        _fStatementGetHumanSellingAndStorageItem_MakeIndex.Prepare();

        _fStatementGetItemBindOption = fdb.AddSQLStatement("UserShop_GetItemBindOption");
        _fStatementGetItemBindOption.Sql = "select " + "MakeIndex," + "IsBind," + "BindOption " + "from Items " +
            "where ParentID = ? and ItemType = ? and ItemIndex = ?;";
        _fStatementGetItemBindOption.Prepare();
    }

    /// <summary>MySqlUserShopDB.pas:494-779 <c>DoFinal</c>。</summary>
    public override void DoFinal()
    {
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
        if (_fStatementSetTimeHasArrivedSellItems != null) { _fStatementSetTimeHasArrivedSellItems.StatementFinalize(); _fStatementSetTimeHasArrivedSellItems = null; }
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
        if (_fStatementGetItemBindOption != null) { _fStatementGetItemBindOption.StatementFinalize(); _fStatementGetItemBindOption = null; }
    }

    /// <summary>MySqlUserShopDB.pas:781-793 <c>DoShopAdd</c>。</summary>
    protected override bool DoShopAdd(string shopName, string humanName)
    {
        if (HumanNameExists(humanName) || ShopNameExists(shopName))
        {
            return false;
        }
        else
        {
            IMySqlStatement s = _fStatementInsertUserShop!;
            s.Reset();
            s.OrderBindParamText(humanName);
            s.OrderBindParamText(shopName);
            bool result = s.Step();
            s.Reset();
            return result;
        }
    }

    /// <summary>MySqlUserShopDB.pas:795-801 <c>DoHumanNameExists</c>。</summary>
    protected override bool DoHumanNameExists(string humanName)
    {
        IMySqlStatement s = _fStatementHumanNameExists!;
        s.Reset();
        s.OrderBindParamText(humanName);
        bool result = s.Query() && s.Fetch();
        s.Reset();
        return result;
    }

    /// <summary>MySqlUserShopDB.pas:803-809 <c>DoShopNameExists</c>。</summary>
    protected override bool DoShopNameExists(string shopName)
    {
        IMySqlStatement s = _fStatementShopNameExists!;
        s.Reset();
        s.OrderBindParamText(shopName);
        bool result = s.Query() && s.Fetch();
        s.Reset();
        return result;
    }

    /// <summary>MySqlUserShopDB.pas:811-982 <c>DoGetAllShop</c>。</summary>
    protected override int DoGetAllShop(int startIndex, string keyword, bool isKeywordHumanName, int sortType, UserShopList shopList)
    {
        int result = 0;
        try
        {
            if (keyword.Length > 0)
            {
                IMySqlStatement sm = _fdb!.AddSQLStatement("UserShop_GetAllShop");
                try
                {
                    sm.Sql = "SELECT " + "A.ShopID," + "A.HumanName," + "A.ShopName," + "A.IsBusiness," + "A.CreateDate," + "A.CareValue," +
                        "(select count(*) from UserShopItem where ShopID = A.ShopID and IsAllowSell = 1 and length(ifnull(BuyerName, '''')) = 0) as SellItemCount,"
                        +
                        "(select count(*) from UserShopItem where ShopID = A.ShopID and length(ifnull(BuyerName, '''')) > 0) as SelledItemCount,"
                        +
                        "(select count(*) from UserShopItem where ShopID = A.ShopID and IsAllowSell = 0 and length(ifnull(BuyerName, '''')) = 0) as StorageItemCount "
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
                    if (sm.Query())
                    {
                        while (sm.Fetch())
                        {
                            var userShop = new TUserShop();
                            userShop.ShopID = sm.OrderGetColumnValueInt;
                            userShop.sMasterName = sm.OrderGetColumnValueText;
                            userShop.sShopName = sm.OrderGetColumnValueText;
                            userShop.boBusiness = sm.OrderGetColumnValueBool;
                            userShop.dCreateDate = sm.OrderGetColumnValueDateTime;
                            userShop.nCareValue = sm.OrderGetColumnValueInt;
                            userShop.SellItemCount = sm.OrderGetColumnValueInt;
                            userShop.SelledItemCount = sm.OrderGetColumnValueInt;
                            userShop.StorageItemCount = sm.OrderGetColumnValueInt;

                            shopList.Add(userShop);

                            result++;
                        }
                    }
                }
                finally
                {
                    sm.Reset();
                }
            }
            else
            {
                IMySqlStatement? sm;
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
                    sm.OrderBindParamInt(1);
                    sm.OrderBindParamInt(1);
                }
                else
                {
                    sm.OrderBindParamInt(0);
                    sm.OrderBindParamInt(1);
                }

                sm.OrderBindParamInt(startIndex);

                if (sm.Step())
                {
                    while (sm.Fetch())
                    {
                        var userShop = new TUserShop();
                        userShop.ShopID = sm.OrderGetColumnValueInt;
                        userShop.sMasterName = sm.OrderGetColumnValueText;
                        userShop.sShopName = sm.OrderGetColumnValueText;
                        userShop.boBusiness = sm.OrderGetColumnValueBool;
                        userShop.dCreateDate = sm.OrderGetColumnValueDateTime;
                        userShop.nCareValue = sm.OrderGetColumnValueInt;
                        userShop.SellItemCount = sm.OrderGetColumnValueInt;
                        userShop.SelledItemCount = sm.OrderGetColumnValueInt;
                        userShop.StorageItemCount = sm.OrderGetColumnValueInt;

                        shopList.Add(userShop);

                        result++;
                    }
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

    /// <summary>MySqlUserShopDB.pas:984-1078 <c>DoGetAllShopCount</c>。</summary>
    protected override int DoGetAllShopCount(string keyword, bool isKeywordHumanName)
    {
        int result = 0;
        try
        {
            if (keyword.Length > 0)
            {
                IMySqlStatement sm = _fdb!.AddSQLStatement("UserShop_GetAllShopCount");
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
                    if (sm.Query() && sm.Fetch())
                    {
                        result = sm.OrderGetColumnValueInt;
                    }
                }
                finally
                {
                    sm.Reset();
                }
            }
            else
            {
                IMySqlStatement s = _fStatementGetAllShop_Count!;
                s.Reset();

                if (DbLayerGlobals.Environment.boOfflineCloseMyShop)
                {
                    s.OrderBindParamInt(1);
                    s.OrderBindParamInt(1);
                }
                else
                {
                    s.OrderBindParamInt(0);
                    s.OrderBindParamInt(1);
                }

                if (s.Query() && s.Fetch())
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

    /// <summary>MySqlUserShopDB.pas:1080-1114 <c>DoGetAllShopEx</c>。</summary>
    protected override int DoGetAllShopEx(UserShopList shopList)
    {
        int result = 0;
        try
        {
            IMySqlStatement s = _fStatementGetAllShop_Ex!;
            s.Reset();
            if (s.Query())
            {
                while (s.Fetch())
                {
                    var userShop = new TUserShop();
                    userShop.ShopID = s.OrderGetColumnValueInt;
                    userShop.sMasterName = s.OrderGetColumnValueText;
                    userShop.sShopName = s.OrderGetColumnValueText;
                    userShop.boBusiness = s.OrderGetColumnValueBool;
                    userShop.dCreateDate = s.OrderGetColumnValueDateTime;
                    userShop.nCareValue = s.OrderGetColumnValueInt;
                    userShop.SellItemCount = 0;
                    userShop.SelledItemCount = 0;
                    userShop.StorageItemCount = 0;

                    shopList.Add(userShop);

                    result++;
                }
            }

            s.Reset();
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
        }
        return result;
    }

    /// <summary>MySqlUserShopDB.pas:1116-1392 <c>DoGetSellItems</c>。</summary>
    protected override int DoGetSellItems(bool isMyShop, int startIndex, string humanName, string keyword, int itemType,
        int moneyType, int minPrice, int maxPrice, int sortType, UserShopItemList itemList, TShopItemType shopItemType)
    {
        int result = 0;
        try
        {
            if (keyword.Length > 0)
            {
                IMySqlStatement sm = _fdb!.AddSQLStatement("UserShop_GetAllSellItem");
                try
                {
                    sm.Sql = "SELECT " + "A.ShopID, " + "A.ItemID, " + "A.MoneyType, " + "A.ItemType, " + "A.IsAllowSell, " + "A.ItemPrice, "
                        + "A.CreateDate, " + "A.IsGetMoney," + "A.BuyerName, " + "B.ShopName, " + "B.HumanName " + "FROM " +
                        "UserShopItem A, UserShop B " + "WHERE A.ShopID = B.ShopID ";

                    if (shopItemType == TShopItemType.sitSelling)
                    {
                        if (isMyShop)
                            sm.Sql = sm.Sql + " and (A.IsAllowSell >= 1) and (A.IsAllowSell <= 2) and length(ifnull(A.BuyerName, '''')) = 0 ";
                        else
                            sm.Sql = sm.Sql + " and (A.IsAllowSell = 1) and length(ifnull(A.BuyerName, '''')) = 0 ";
                    }
                    else if (shopItemType == TShopItemType.sitSelled)
                    {
                        sm.Sql = sm.Sql + " and length(ifnull(A.BuyerName, '''')) > 0 ";
                    }
                    else if (shopItemType == TShopItemType.sitStorage)
                    {
                        sm.Sql = sm.Sql + " and (A.IsAllowSell = 0) and length(ifnull(A.BuyerName, '''')) = 0 ";
                    }
                    else
                    {
                        sm.Sql = sm.Sql + " and length(ifnull(A.BuyerName, '''')) = 0 ";
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
                    if (sm.Query())
                    {
                        while (sm.Fetch())
                        {
                            var shopItem = new TUserShopItem();

                            int shopId = sm.OrderGetColumnValueInt;
                            shopItem.ShopID = shopId;
                            shopItem.ItemID = sm.OrderGetColumnValueInt;
                            shopItem.btMoneyType = (byte)sm.OrderGetColumnValueInt;
                            shopItem.btItemType = (byte)sm.OrderGetColumnValueInt;
                            shopItem.btAllowSell = (byte)sm.OrderGetColumnValueInt;
                            shopItem.nPrice = sm.OrderGetColumnValueInt;
                            shopItem.dCreateDate = sm.OrderGetColumnValueDateTime;
                            shopItem.boGetMoney = sm.OrderGetColumnValueBool;
                            shopItem.sBuyName = sm.OrderGetColumnValueText;
                            shopItem.sShopName = sm.OrderGetColumnValueText;
                            shopItem.sMasterName = sm.OrderGetColumnValueText;

                            Owner.LoadItemFromDB(ref shopItem.UserItem, shopId, DataItemTypes.UserShopItemType, shopItem.ItemID);

                            itemList.Add(shopItem);

                            result++;
                        }
                    }
                }
                finally
                {
                    sm.Reset();
                }
            }
            else
            {
                IMySqlStatement sm;
                if (shopItemType == TShopItemType.sitSelling)
                {
                    sm = sortType == 0 ? _fStatementGetShopSellingItem_ASC! : _fStatementGetShopSellingItem_DESC!;
                    sm.Reset();

                    if (isMyShop)
                    {
                        sm.OrderBindParamInt(1);
                        sm.OrderBindParamInt(2);
                    }
                    else
                    {
                        sm.OrderBindParamInt(1);
                        sm.OrderBindParamInt(1);
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
                    sm.OrderBindParamInt(itemType);
                    sm.OrderBindParamInt(itemType);
                }
                else
                {
                    sm.OrderBindParamInt(0);
                    sm.OrderBindParamInt(255);
                }

                if (moneyType >= 0)
                {
                    sm.OrderBindParamInt(moneyType);
                    sm.OrderBindParamInt(moneyType);
                }
                else
                {
                    sm.OrderBindParamInt(0);
                    sm.OrderBindParamInt(6);
                }

                if (minPrice <= maxPrice && maxPrice > 0)
                {
                    sm.OrderBindParamInt(minPrice);
                    sm.OrderBindParamInt(maxPrice);
                }
                else
                {
                    sm.OrderBindParamInt(0);
                    sm.OrderBindParamInt(int.MaxValue);
                }

                if (humanName.Length > 0)
                {
                    sm.OrderBindParamInt(0);
                    sm.OrderBindParamText(humanName);
                }
                else
                {
                    sm.OrderBindParamInt(1);
                    sm.OrderBindParamText("*");
                }

                if (DbLayerGlobals.Environment.boOfflineCloseMyShop)
                {
                    sm.OrderBindParamInt(1);
                    sm.OrderBindParamInt(1);
                }
                else
                {
                    sm.OrderBindParamInt(0);
                    sm.OrderBindParamInt(1);
                }

                sm.OrderBindParamInt(startIndex);

                if (sm.Query())
                {
                    while (sm.Fetch())
                    {
                        var shopItem = new TUserShopItem();

                        int shopId = sm.OrderGetColumnValueInt;
                        shopItem.ShopID = shopId;
                        shopItem.ItemID = sm.OrderGetColumnValueInt;
                        shopItem.btMoneyType = (byte)sm.OrderGetColumnValueInt;
                        shopItem.btItemType = (byte)sm.OrderGetColumnValueInt;
                        shopItem.btAllowSell = (byte)sm.OrderGetColumnValueInt;
                        shopItem.nPrice = sm.OrderGetColumnValueInt;
                        shopItem.dCreateDate = sm.OrderGetColumnValueDateTime;
                        shopItem.boGetMoney = sm.OrderGetColumnValueBool;
                        shopItem.sBuyName = sm.OrderGetColumnValueText;
                        shopItem.sShopName = sm.OrderGetColumnValueText;
                        shopItem.sMasterName = sm.OrderGetColumnValueText;

                        Owner.LoadItemFromDB(ref shopItem.UserItem, shopId, DataItemTypes.UserShopItemType, shopItem.ItemID);

                        itemList.Add(shopItem);

                        result++;
                    }
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

    /// <summary>MySqlUserShopDB.pas:1394-1595 <c>DoGetSellItemsCount</c>。
    /// 原文 1476-1485 行 IsMyShop 两分支都绑 (1, 2)（与 DoGetSellItems 的 (1,1) 不对称）——逐字保留。</summary>
    protected override int DoGetSellItemsCount(bool isMyShop, string humanName, string keyword, int itemType, int moneyType,
        int minPrice, int maxPrice, TShopItemType shopItemType)
    {
        int result = 0;
        try
        {
            if (keyword.Length > 0)
            {
                IMySqlStatement sm = _fdb!.AddSQLStatement("UserShop_GetAllSellItemCount");
                try
                {
                    sm.Sql = "SELECT " + "Count(A.ShopID) " + "FROM " + "UserShopItem A, UserShop B " + "WHERE A.ShopID = B.ShopID ";

                    if (shopItemType == TShopItemType.sitSelling)
                    {
                        if (isMyShop)
                            sm.Sql = sm.Sql + " and (A.IsAllowSell in (1, 2)) and length(ifnull(A.BuyerName, '''')) = 0 ";
                        else
                            sm.Sql = sm.Sql + " and (A.IsAllowSell = 1) and length(ifnull(A.BuyerName, '''')) = 0 ";
                    }
                    else if (shopItemType == TShopItemType.sitSelled)
                    {
                        sm.Sql = sm.Sql + " and length(ifnull(A.BuyerName, '''')) > 0 ";
                    }
                    else if (shopItemType == TShopItemType.sitStorage)
                    {
                        sm.Sql = sm.Sql + " and (A.IsAllowSell = 0) and length(ifnull(A.BuyerName, '''')) = 0 ";
                    }
                    else
                    {
                        sm.Sql = sm.Sql + " and length(ifnull(A.BuyerName, '''')) = 0 ";
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
                    if (sm.Query() && sm.Fetch())
                    {
                        result = sm.OrderGetColumnValueInt;
                    }
                }
                finally
                {
                    sm.Reset();
                }
            }
            else
            {
                IMySqlStatement sm;
                if (shopItemType == TShopItemType.sitSelling)
                {
                    sm = _fStatementGetShopSellingItem_Count!;
                    sm.Reset();

                    // 原文 1476-1485：两分支完全相同（(1,2) / (1,2)）—— 逐字保留。
                    if (isMyShop)
                    {
                        sm.OrderBindParamInt(1);
                        sm.OrderBindParamInt(2);
                    }
                    else
                    {
                        sm.OrderBindParamInt(1);
                        sm.OrderBindParamInt(2);
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
                    sm.OrderBindParamInt(itemType);
                    sm.OrderBindParamInt(itemType);
                }
                else
                {
                    sm.OrderBindParamInt(0);
                    sm.OrderBindParamInt(255);
                }

                if (moneyType >= 0)
                {
                    sm.OrderBindParamInt(moneyType);
                    sm.OrderBindParamInt(moneyType);
                }
                else
                {
                    sm.OrderBindParamInt(0);
                    sm.OrderBindParamInt(6);
                }

                if (minPrice <= maxPrice && maxPrice > 0)
                {
                    sm.OrderBindParamInt(minPrice);
                    sm.OrderBindParamInt(maxPrice);
                }
                else
                {
                    sm.OrderBindParamInt(0);
                    sm.OrderBindParamInt(int.MaxValue);
                }

                if (humanName.Length > 0)
                {
                    sm.OrderBindParamInt(0);
                    sm.OrderBindParamText(humanName);
                }
                else
                {
                    sm.OrderBindParamInt(1);
                    sm.OrderBindParamText("*");
                }

                if (DbLayerGlobals.Environment.boOfflineCloseMyShop)
                {
                    sm.OrderBindParamInt(1);
                    sm.OrderBindParamInt(1);
                }
                else
                {
                    sm.OrderBindParamInt(0);
                    sm.OrderBindParamInt(1);
                }

                if (sm.Query() && sm.Fetch())
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

    /// <summary>MySqlUserShopDB.pas:1597-1702：<c>DoGetHumanItems</c> 整体被 (* *) 注释掉（原文无实现）。</summary>

    /// <summary>MySqlUserShopDB.pas:1704-1806 <c>DoGetHumanItemWithMakeIndex</c>。</summary>
    protected override bool DoGetHumanItemWithMakeIndex(bool isMyShop, string humanName, TShopItemType shopItemType,
        int itemMakeIndex, out TSimpleUserShopItem userShopItem)
    {
        userShopItem = new TSimpleUserShopItem();
        bool result = false;
        try
        {
            IMySqlStatement sm;
            if (shopItemType == TShopItemType.sitSelling)
            {
                sm = _fStatementGetHumanSellingItem_MakeIndex!;

                sm.Reset();

                if (isMyShop)
                {
                    sm.OrderBindParamInt(1);
                    sm.OrderBindParamInt(2);

                    sm.OrderBindParamInt(0);
                    sm.OrderBindParamInt(1);
                }
                else
                {
                    sm.OrderBindParamInt(1);
                    sm.OrderBindParamInt(1);

                    if (DbLayerGlobals.Environment.boOfflineCloseMyShop)
                    {
                        sm.OrderBindParamInt(1);
                        sm.OrderBindParamInt(1);
                    }
                    else
                    {
                        sm.OrderBindParamInt(0);
                        sm.OrderBindParamInt(1);
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
                sm.OrderBindParamInt(0);
                sm.OrderBindParamText(humanName);
            }
            else
            {
                sm.OrderBindParamInt(1);
                sm.OrderBindParamText("*");
            }

            sm.OrderBindParamInt(itemMakeIndex);

            if (sm.Query() && sm.Fetch())
            {
                userShopItem.ShopID = sm.OrderGetColumnValueInt;
                userShopItem.ItemID = sm.OrderGetColumnValueInt;
                userShopItem.btMoneyType = (byte)sm.OrderGetColumnValueInt;
                userShopItem.btItemType = (byte)sm.OrderGetColumnValueInt;
                userShopItem.btAllowSell = (byte)sm.OrderGetColumnValueInt;
                userShopItem.nPrice = sm.OrderGetColumnValueInt;
                userShopItem.dCreateDate = sm.OrderGetColumnValueDateTime;
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

    /// <summary>MySqlUserShopDB.pas:1808-1832 <c>DoGetUserShopInfo</c>（无 except，仅 finally）。</summary>
    protected override bool DoGetUserShopInfo(string humanName, out TUserShop userShop)
    {
        userShop = new TUserShop();
        bool result = false;

        IMySqlStatement s = _fStatementGetUserShopInfo!;
        s.Reset();
        try
        {
            s.OrderBindParamText(humanName);
            if (s.Query() && s.Fetch())
            {
                userShop.ShopID = s.OrderGetColumnValueInt;
                userShop.sMasterName = s.OrderGetColumnValueText;
                userShop.sShopName = s.OrderGetColumnValueText;
                userShop.boBusiness = s.OrderGetColumnValueBool;
                userShop.dCreateDate = s.OrderGetColumnValueDateTime;
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

    /// <summary>MySqlUserShopDB.pas:1834-1840 <c>DoIncUserShopCareValue</c>。</summary>
    protected override void DoIncUserShopCareValue(string humanName)
    {
        IMySqlStatement s = _fStatementIncUserShopCareValue!;
        s.Reset();
        s.OrderBindParamText(humanName);
        s.Step();
        s.Reset();
    }

    /// <summary>MySqlUserShopDB.pas:1842-1898 <c>DoAddItem</c>（ItemType? 用 OrderBindParamDateTime）。</summary>
    protected override bool DoAddItem(int shopId, TUserShopItem shopItem, string sItemName)
    {
        bool result = false;
        int itemId;
        try
        {
            IMySqlStatement maxStmt = _fStatementGetMaxShopItemID!;
            maxStmt.Reset();
            maxStmt.OrderBindParamInt(shopId);
            if (maxStmt.Query() && maxStmt.Fetch())
                itemId = maxStmt.OrderGetColumnValueInt;
            else
                itemId = 1;
        }
        finally
        {
            _fStatementGetMaxShopItemID!.Reset();
        }

        _fdb!.StartTransaction();
        try
        {
            IMySqlStatement s = _fStatementInsertUserShopItem!;
            s.Reset();
            try
            {
                s.OrderBindParamInt(shopId);
                s.OrderBindParamInt(itemId);
                s.OrderBindParamInt(shopItem.btItemType);
                s.OrderBindParamDateTime(shopItem.dCreateDate);
                s.OrderBindParamInt(shopItem.btAllowSell);
                s.OrderBindParamInt(shopItem.btMoneyType);
                s.OrderBindParamInt(shopItem.nPrice);
                s.OrderBindParamBool(shopItem.boGetMoney);
                s.OrderBindParamText(shopItem.sBuyName.Value);

                s.OrderBindParamText(sItemName);

                string changeName = "";
                if (shopItem.UserItem.GetBtValue(13) == 1 && shopItem.UserItem.NameStr.Length > 0)
                    changeName = DbLayerRunSeam.ProcessItemName(shopItem.UserItem.NameStr);

                s.OrderBindParamText(changeName);

                if (s.Step())
                {
                    Owner.SaveItemToDB(shopItem.UserItem, shopId, DataItemTypes.UserShopItemType, itemId);
                    result = true;
                }

                _fdb!.Commit();
            }
            finally
            {
                s.Reset();
            }
        }
        catch (Exception e)
        {
            DbLayerGlobals.MainOutMessage(e.Message);
            _fdb!.RollBack();
        }
        return result;
    }

    /// <summary>MySqlUserShopDB.pas:1900-1918 <c>DoUpdateItem</c>。</summary>
    protected override bool DoUpdateItem(int shopId, int itemId, int btItemType, int btAllowSell, int btMoneyType, int nPrice)
    {
        IMySqlStatement s = _fStatementUpdateUserShopItem!;
        s.Reset();
        try
        {
            s.OrderBindParamInt(btItemType);
            s.OrderBindParamInt(btAllowSell);
            s.OrderBindParamInt(btMoneyType);
            s.OrderBindParamInt(nPrice);
            s.OrderBindParamInt(shopId);
            s.OrderBindParamInt(itemId);

            return s.Step();
        }
        finally
        {
            s.Reset();
        }
    }

    /// <summary>MySqlUserShopDB.pas:1920-1956 <c>DoDeleteItem</c>。
    /// 与 SQLite 版的差异：多语句脚本用 sLineBreak 分隔但**没有** '-------------------------------' 分隔行（原文注释掉了）。</summary>
    protected override bool DoDeleteItem(int shopId, int itemId)
    {
        bool result = false;

        _fdb!.StartTransaction();
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
                " and ItemIndex = " + IntToStr(itemId) + ";" + LineBreak +
                "delete from UserShopItem where ShopID = " + IntToStr(shopId) + " and ItemID = " + IntToStr(itemId) + ";";

            _fdb!.Exec(s);

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

    /// <summary>MySqlUserShopDB.pas:1958-1970 <c>DoBuyItem</c>。</summary>
    protected override bool DoBuyItem(int shopId, int itemId, string buyer)
    {
        IMySqlStatement s = _fStatementBuyUserShopItem!;
        s.Reset();
        try
        {
            s.OrderBindParamText(buyer);
            s.OrderBindParamInt(shopId);
            s.OrderBindParamInt(itemId);

            return s.Step();
        }
        finally
        {
            s.Reset();
        }
    }

    /// <summary>MySqlUserShopDB.pas:1972-1983 <c>DoGetMoneyItem</c>。</summary>
    protected override bool DoGetMoneyItem(int shopId, int itemId)
    {
        IMySqlStatement s = _fStatementGetMoneyShopItem!;
        s.Reset();
        try
        {
            s.OrderBindParamInt(shopId);
            s.OrderBindParamInt(itemId);

            return s.Step();
        }
        finally
        {
            s.Reset();
        }
    }

    /// <summary>MySqlUserShopDB.pas:1985-2009 <c>DoGetSelledAndNoGetMoneyTotal</c>。</summary>
    protected override int DoGetSelledAndNoGetMoneyTotal(System.Collections.Generic.List<TSelledAndNoGetMoneyTotal> itemList)
    {
        int result = 0;
        IMySqlStatement s = _fStatementGetSelledAndNoGetMoneyTotal!;
        s.Reset();
        try
        {
            if (s.Query())
            {
                while (s.Fetch())
                {
                    var item = new TSelledAndNoGetMoneyTotal();
                    itemList.Add(item);

                    item.sMasterName = s.OrderGetColumnValueText;
                    item.btMoneyType = (byte)s.OrderGetColumnValueInt;
                    item.nSumPrice = s.OrderGetColumnValueInt64;

                    result++;
                }
            }
        }
        finally
        {
            s.Reset();
        }
        return result;
    }

    /// <summary>MySqlUserShopDB.pas:2011-2042 <c>DoShopDelete</c>（含 '-------------------------------' 分隔行）。</summary>
    protected override bool DoShopDelete(int shopId)
    {
        bool result = false;

        _fdb!.StartTransaction();
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

            _fdb!.Exec(s);
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

    /// <summary>MySqlUserShopDB.pas:2044-2054 <c>DoShopRename</c>。</summary>
    protected override bool DoShopRename(int shopId, string newShopName)
    {
        IMySqlStatement s = _fStatementUserShopRename!;
        s.Reset();
        try
        {
            s.OrderBindParamText(newShopName);
            s.OrderBindParamInt(shopId);
            return s.Step();
        }
        finally
        {
            s.Reset();
        }
    }

    /// <summary>MySqlUserShopDB.pas:2056-2086 <c>DoHumanRename</c>（StartTransaction 在 try 内，与 SQLite 版不同）。</summary>
    protected override bool DoHumanRename(string oldName, string newName)
    {
        bool result = false;

        IMySqlStatement rename2 = _fStatementUserShopRename2!;
        IMySqlStatement buyerRename = _fStatementShopItemBuyerRenameName!;
        rename2.Reset();
        buyerRename.Reset();

        try
        {
            _fdb!.StartTransaction();
            try
            {
                rename2.OrderBindParamText(newName);
                rename2.OrderBindParamText(oldName);
                rename2.Step();

                buyerRename.OrderBindParamText(newName);
                buyerRename.OrderBindParamText(oldName);
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

    /// <summary>MySqlUserShopDB.pas:2088-2098 <c>DoUpdateHumanBusiness</c>。</summary>
    protected override bool DoUpdateHumanBusiness(string humanName, bool isBusiness)
    {
        IMySqlStatement s = _fStatementUpdateHumanBusiness!;
        s.Reset();
        try
        {
            s.OrderBindParamBool(isBusiness);
            s.OrderBindParamText(humanName);
            return s.Step();
        }
        finally
        {
            s.Reset();
        }
    }

    /// <summary>MySqlUserShopDB.pas:2100-2187 <c>DoRun</c>（与 SQLite 版不同：走 Items 表查 MakeIndex/IsBind/BindOption，
    /// 且用 SetTimeHasArrivedSellItems 把超期物品直接落库）。</summary>
    protected override void DoRun()
    {
        if (!DbLayerGlobals.Environment.boEnabledMySellShopItemTime)
            return;

        var itemList = new System.Collections.Generic.List<TSimpleUserShopItem>();
        IMySqlStatement s = _fStatementGetTimeHasArrivedSellItems!;
        s.Reset();
        try
        {
            s.OrderBindParamInt(DbLayerGlobals.Environment.nMySellShopItemTime * 60);
            if (s.Query())
            {
                while (s.Fetch())
                {
                    var shopItem = new TSimpleUserShopItem();
                    itemList.Add(shopItem);

                    shopItem.ShopID = s.OrderGetColumnValueInt;
                    shopItem.ItemID = s.OrderGetColumnValueInt;
                    shopItem.btMoneyType = (byte)s.OrderGetColumnValueInt;
                    shopItem.btItemType = (byte)s.OrderGetColumnValueInt;
                    shopItem.btAllowSell = (byte)s.OrderGetColumnValueInt;
                    shopItem.nPrice = s.OrderGetColumnValueInt;

                    // 原文 2130 行：MySQL 版此处用 OrderGetColumnValueInt64 + UnixToDateTime（与 SQLite 版同形，
                    // 但 MySQL 侧已有 OrderGetColumnValueDateTime）。逐字保留原文写法。
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
                }
            }
        }
        finally
        {
            s.Reset();
        }

        foreach (TSimpleUserShopItem shopItem in itemList)
        {
            TStdItem? stdItem = DbLayerRunSeam.GetStdItem(shopItem.wIndex);

            if (stdItem != null)
            {
                bool isItemTimeExpired = false;
                if (DbLayerRunSeam.GetUserItemBindValue(shopItem.btBindOption, UserItemBindValue.ubNoStorage) && shopItem.boIsBind)
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
    }

    /// <summary>原文 <c>sLineBreak</c>（Windows = CRLF）。</summary>
    private static string LineBreak => "\r\n";

    /// <summary>原文 <c>IntToStr</c>。</summary>
    private static string IntToStr(int value) => GXX.Core.Rtl.DelphiRTL.IntToStr(value);

    /// <summary>M2Share/ObjBase <c>GetUserItemBindValue</c> 的绑定位枚举（ubNoStorage = 禁止存）。</summary>
    private static class UserItemBindValue
    {
        public const int ubNoStorage = 3;
    }
}
