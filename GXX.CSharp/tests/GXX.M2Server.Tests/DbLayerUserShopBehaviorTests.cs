// 源单元（文件头证据）：
//   Source/M2Engine/SqliteUserShopDB.pas / Source/M2Engine/MySqlUserShopDB.pas
//
// 行为保真测试：每个 Do* 分支的**参数绑定顺序**、**读列顺序**、**边界语义**
// （空结果集、字段缺失、超长字符串、负数、时间戳格式、重复插入）、
// 以及"看起来一样实则不同"的方言差异断言。
//
// 所有用例都注入内存 DB（DbLayerTestKit），不连真实数据库。

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using GXX.Core.Protocol;
using GXX.M2Server.DbLayer;

namespace GXX.M2Server.Tests;

public class DbLayerUserShopBehaviorTests
{
    // ------------------------------------------------------------------ 空结果集

    [Fact]
    public void Sqlite_GetSellItems_EmptyResult_ReturnsZeroAndLeavesListUntouched()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeSqliteDatabase();
        var host = new FakeDbLayerHost { DataBase = db };
        var unit = new TSqliteUserShopDB(host);
        unit.DoInit();

        var list = new UserShopItemList();
        // 语句无预设行 → Step 返回 SQLITE_DONE
        int n = unit.GetSellItems(false, 0, "", "", -1, -1, 0, 0, 0, list);

        Assert.Equal(0, n);
        Assert.Equal(0, list.Count);
        // 原文先 Reset 再绑 IsMyShop 的 (1,1)，随后是 SGetShopItemWhere 的 10 个参数
        // （ItemType×2, MoneyType×2, ItemPrice×2, ((1=?)|(B.HumanName=?))×2, IsBusiness×2），
        // 最后是 Offset —— 共 13 个绑定（SGetShopItemQueryField/Where 的实际拼接顺序见 SqlStatements）。
        var stmt = db.For("UserShop_GetSellingItem_ASC");
        Assert.Equal(
            new[] { "int", "int", "int", "int", "int", "int", "int", "int", "int", "text", "int", "int", "int" },
            stmt.LastBinds.Select(b => b.Kind).ToArray());
        Assert.Equal(1, Convert.ToInt32(stmt.LastBinds[0].Value));   // IsMyShop=false → AllowSell(1,1)
        Assert.Equal(1, Convert.ToInt32(stmt.LastBinds[1].Value));
        Assert.Equal(1, Convert.ToInt32(stmt.LastBinds[8].Value));   // ((1 = ?) or (B.HumanName = ?))
        Assert.Equal("*", stmt.LastBinds[9].Text);
        Assert.Equal(0, Convert.ToInt32(stmt.LastBinds[10].Value));  // IsBusiness 区间 (0,1)
        Assert.Equal(1, Convert.ToInt32(stmt.LastBinds[11].Value));
        Assert.Equal(0, Convert.ToInt32(stmt.LastBinds[^1].Value));  // StartIndex
    }

    [Fact]
    public void MySql_GetSellItems_EmptyResult_ReturnsZero()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        var unit = new TMySqlUserShopDB(new FakeDbLayerHost { DataBase = db });
        unit.DoInit();

        var list = new UserShopItemList();
        int n = unit.GetSellItems(false, 0, "", "", -1, -1, 0, 0, 0, list);

        Assert.Equal(0, n);
        // MySQL 用 Query + Fetch 循环：Query 返回 true 但 Fetch 立刻 false。
        var stmt = db.For("UserShop_GetSellingItem_ASC");
        Assert.Equal(1, stmt.QueryCount);
        Assert.Equal(1, stmt.FetchCount);
    }

    // ------------------------------------------------------------------ 处理"我会员 shop"分支的 (1,2) vs (1,1)

    [Fact]
    public void Sqlite_GetSellItems_IsMyShop_SwitchesTheAllowSellBounds()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteUserShopDB(new FakeDbLayerHost { DataBase = db });
        unit.DoInit();

        unit.GetSellItems(true, 0, "", "", -1, -1, 0, 0, 0, new UserShopItemList());
        var mine = db.For("UserShop_GetSellingItem_ASC");
        var mineBinds = mine.LastBinds.ToArray();
        Assert.Equal(1, Convert.ToInt32(mineBinds[0].Value));
        Assert.Equal(2, Convert.ToInt32(mineBinds[1].Value));

        unit.GetSellItems(false, 0, "", "", -1, -1, 0, 0, 0, new UserShopItemList());
        var other = db.For("UserShop_GetSellingItem_ASC");
        var otherBinds = other.LastBinds.ToArray();
        Assert.Equal(1, Convert.ToInt32(otherBinds[0].Value));
        Assert.Equal(1, Convert.ToInt32(otherBinds[1].Value));
        Assert.Equal(0, Convert.ToInt32(otherBinds[^1].Value));   // StartIndex
    }

    /// <summary>
    /// 原文 1443-1452 行 bug：DoGetSellItemsCount 的 IsMyShop 两分支都绑 (1, 2)
    /// （而 DoGetSellItems 是 (1,1)）。本测试锁死该不对称，防止"顺手修正"。
    /// </summary>
    [Fact]
    public void Sqlite_GetSellItemsCount_BindsOneTwoForBothBranches_MirroringTheOriginalBug()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteUserShopDB(new FakeDbLayerHost { DataBase = db });
        unit.DoInit();

        unit.GetSellItemsCount(true, "", "", -1, -1, 0, 0);
        var stmt = db.For("UserShop_GetSellingItem_Count");
        var mineBinds = stmt.LastBinds.ToArray();
        Assert.Equal(1, Convert.ToInt32(mineBinds[0].Value));
        Assert.Equal(2, Convert.ToInt32(mineBinds[1].Value));

        unit.GetSellItemsCount(false, "", "", -1, -1, 0, 0);
        var otherBinds = stmt.LastBinds.ToArray();
        // 原文 bug：IsMyShop=False 也绑 (1,2)。
        Assert.Equal(1, Convert.ToInt32(otherBinds[0].Value));
        Assert.Equal(2, Convert.ToInt32(otherBinds[1].Value));
    }

    [Fact]
    public void MySql_GetSellItemsCount_BindsOneTwoForBothBranches_MirroringTheOriginalBug()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        var unit = new TMySqlUserShopDB(new FakeDbLayerHost { DataBase = db });
        unit.DoInit();

        unit.GetSellItemsCount(true, "", "", -1, -1, 0, 0);
        var stmt = db.For("UserShop_GetSellingItem_Count");
        var mineBinds = stmt.LastBinds.ToArray();
        Assert.Equal(1, Convert.ToInt32(mineBinds[0].Value));
        Assert.Equal(2, Convert.ToInt32(mineBinds[1].Value));

        unit.GetSellItemsCount(false, "", "", -1, -1, 0, 0);
        var otherBinds = stmt.LastBinds.ToArray();
        // 原文 bug：IsMyShop=False 也绑 (1,2)。
        Assert.Equal(1, Convert.ToInt32(otherBinds[0].Value));
        Assert.Equal(2, Convert.ToInt32(otherBinds[1].Value));
    }

    // ------------------------------------------------------------------ 负数/边界绑定

    [Fact]
    public void Sqlite_GetSellItems_NegativeFilters_BindTheOriginalClamps()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteUserShopDB(new FakeDbLayerHost { DataBase = db });
        unit.DoInit();

        unit.GetSellItems(false, 3, "店主", "", -1, -1, -5, 0, 1, new UserShopItemList());

        var stmt = db.For("UserShop_GetSellingItem_DESC");
        // ItemType < 0 → (0, 255)；MoneyType < 0 → (0, 6)；价格无效 → (0, High(Integer))。
        Assert.Equal(0, Convert.ToInt32(stmt.LastBinds[2].Value));
        Assert.Equal(255, Convert.ToInt32(stmt.LastBinds[3].Value));
        Assert.Equal(0, Convert.ToInt32(stmt.LastBinds[4].Value));
        Assert.Equal(6, Convert.ToInt32(stmt.LastBinds[5].Value));
        Assert.Equal(0, Convert.ToInt32(stmt.LastBinds[6].Value));
        Assert.Equal(int.MaxValue, Convert.ToInt32(stmt.LastBinds[7].Value));
        // HumanName 非空 → (0, 名字)
        Assert.Equal(0, Convert.ToInt32(stmt.LastBinds[8].Value));
        Assert.Equal("店主", stmt.LastBinds[9].Text);
        Assert.Equal(3, Convert.ToInt32(stmt.LastBinds[^1].Value));
    }

    [Fact]
    public void MySql_GetSellItems_NegativeFilters_BindTheOriginalClamps()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        var unit = new TMySqlUserShopDB(new FakeDbLayerHost { DataBase = db });
        unit.DoInit();

        unit.GetSellItems(false, 3, "店主", "", -1, -1, -5, 0, 1, new UserShopItemList());

        var stmt = db.For("UserShop_GetSellingItem_DESC");
        Assert.Equal(0, Convert.ToInt32(stmt.LastBinds[2].Value));
        Assert.Equal(255, Convert.ToInt32(stmt.LastBinds[3].Value));
        Assert.Equal(0, Convert.ToInt32(stmt.LastBinds[4].Value));
        Assert.Equal(6, Convert.ToInt32(stmt.LastBinds[5].Value));
        Assert.Equal(0, Convert.ToInt32(stmt.LastBinds[6].Value));
        Assert.Equal(int.MaxValue, Convert.ToInt32(stmt.LastBinds[7].Value));
        Assert.Equal("店主", stmt.LastBinds[9].Text);
    }

    // ------------------------------------------------------------------ 超长字符串（ShortString13 GBK 截断）

    [Fact]
    public void UserShopItem_ShortNames_AreTruncatedToThirteenGbkBytes()
    {
        var item = new TUserShopItem();
        string longName = new string('中', 10);     // 10 个汉字 = GBK 20 字节
        item.sMasterName = longName;
        item.sBuyName = "abcdefghijklmnopqrstuvwxyz";

        // 13 字节 = 6 个汉字（12 字节）+ 1 个被截断的汉字 → GBK 解码会丢弃半个字符
        Assert.True(System.Text.Encoding.GetEncoding(936).GetByteCount(item.sMasterName.Value) <= 13);
        Assert.Equal("abcdefghijklm", item.sBuyName.Value);
    }

    // ------------------------------------------------------------------ 时间戳格式（方言差异断言）

    [Fact]
    public void Sqlite_AddItem_BindsCreateDateAsUnixSecondsMinusEightHours()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeSqliteDatabase();
        var host = new FakeDbLayerHost { DataBase = db };
        var unit = new TSqliteUserShopDB(host);
        unit.DoInit();

        var createDate = new DateTime(2020, 9, 16, 12, 0, 0, DateTimeKind.Local);
        var item = new TUserShopItem { btItemType = 1, btAllowSell = 1, btMoneyType = 2, nPrice = 100, dCreateDate = createDate };

        db.For("UserShop_GetMaxShopItemID").AddRow(7);
        // InsertShopItem 的 Step 返回 SQLITE_DONE（成功）
        db.For("UserShop_InsertShopItem").StepCode = SqliteCodes.SQLITE_DONE;

        bool ok = unit.AddItem(42, item, "物品名");

        Assert.True(ok);
        Assert.Equal("42/1/7/0", host.SavedItems.Single());  // ParentID/ItemType/ItemIndex/UserItem.MakeIndex

        var insert = db.For("UserShop_InsertShopItem");
        // 第 4 个绑定是 CreateDate：Unix 秒 - 8h
        Assert.Equal("int64", insert.LastBinds[3].Kind);
        long expected = DelphiDateUtil.DateTimeToUnix(createDate) - 8 * 60 * 60;
        Assert.Equal(expected, Convert.ToInt64(insert.LastBinds[3].Value));
        Assert.Equal("text", insert.LastBinds[8].Kind);   // BuyerName
        Assert.Equal("text", insert.LastBinds[9].Kind);   // ItemDBName(sItemName)
        Assert.Equal("物品名", insert.LastBinds[9].Text);
        Assert.Single(db.Transactions.Where(t => t == "begin"));
        Assert.Single(db.Transactions.Where(t => t == "commit"));
    }

    [Fact]
    public void MySql_AddItem_BindsCreateDateAsDateTime()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        var host = new FakeDbLayerHost { DataBase = db };
        var unit = new TMySqlUserShopDB(host);
        unit.DoInit();

        var createDate = new DateTime(2020, 9, 16, 12, 0, 0, DateTimeKind.Local);
        var item = new TUserShopItem { btItemType = 1, btAllowSell = 1, btMoneyType = 2, nPrice = 100, dCreateDate = createDate };

        db.For("UserShop_GetMaxShopItemID").AddRow(7);
        db.For("UserShop_InsertShopItem").QueryResult = true;   // Step（布尔）成功

        bool ok = unit.AddItem(42, item, "物品名");

        Assert.True(ok);
        var insert = db.For("UserShop_InsertShopItem");
        Assert.Equal("datetime", insert.LastBinds[3].Kind);
        Assert.Equal(createDate, (DateTime)insert.LastBinds[3].Value!);
        Assert.Single(db.Transactions.Where(t => t == "begin"));
        Assert.Single(db.Transactions.Where(t => t == "commit"));
    }

    [Fact]
    public void Sqlite_AddItem_MaxItemIdMissing_FallsBackToOne()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteUserShopDB(new FakeDbLayerHost { DataBase = db });
        unit.DoInit();

        // GetMaxShopItemID 无行 → ItemID := 1
        db.For("UserShop_InsertShopItem").StepCode = SqliteCodes.SQLITE_DONE;
        bool ok = unit.AddItem(42, new TUserShopItem(), "x");

        Assert.True(ok);
        Assert.Equal(1, Convert.ToInt32(db.For("UserShop_InsertShopItem").LastBinds[1].Value));
    }

    // ------------------------------------------------------------------ 重复插入 / 名称占用

    [Fact]
    public void Sqlite_ShopAdd_RejectsDuplicateHumanOrShopName()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteUserShopDB(new FakeDbLayerHost { DataBase = db });
        unit.DoInit();

        // HumanNameExists 命中 → 不执行 insert
        db.For("UserShop_CheckHumanNameExists").AddRow(1);
        bool added = unit.ShopAdd("我的店", "张三");

        Assert.False(added);
        Assert.Equal(0, db.For("UserShop_InsertUserShop").StepCount);
    }

    [Fact]
    public void Sqlite_ShopAdd_InsertsWhenBothNamesAreFree()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteUserShopDB(new FakeDbLayerHost { DataBase = db });
        unit.DoInit();

        // 两个 exists 查询都没有行（Step → SQLITE_DONE）
        db.For("UserShop_InsertUserShop").StepCode = SqliteCodes.SQLITE_DONE;
        bool added = unit.ShopAdd("我的店", "张三");

        Assert.True(added);
        var insert = db.For("UserShop_InsertUserShop");
        // 原文绑定顺序：HumanName, ShopName（注意 DoShopAdd(ShopName, HumanName) 形参顺序相反）
        Assert.Equal("张三", insert.LastBinds[0].Text);
        Assert.Equal("我的店", insert.LastBinds[1].Text);
    }

    [Fact]
    public void MySql_ShopAdd_RejectsDuplicateHumanOrShopName()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        var unit = new TMySqlUserShopDB(new FakeDbLayerHost { DataBase = db });
        unit.DoInit();

        db.For("UserShop_CheckHumanNameExists").AddRow(1);
        bool added = unit.ShopAdd("我的店", "张三");

        Assert.False(added);
        Assert.Equal(0, db.For("UserShop_InsertUserShop").StepCount);
    }

    // ------------------------------------------------------------------ 时间字段读回（Unix vs DateTime）

    [Fact]
    public void Sqlite_GetUserShopInfo_ReadsCreateDateAsUnixSeconds()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteUserShopDB(new FakeDbLayerHost { DataBase = db });
        unit.DoInit();

        long unix = 1_600_000_000;   // 2020-09-13T12:26:40Z
        db.For("UserShop_GetUserShopInfo").AddRow(5, "张三", "我的店", 1, unix, 9, 1, 2, 3);

        bool found = unit.GetUserShopInfo("张三", out TUserShop shop);

        Assert.True(found);
        Assert.Equal(5, shop.ShopID);
        Assert.Equal("张三", shop.sMasterName.Value);
        Assert.Equal("我的店", shop.sShopName.Value);
        Assert.True(shop.boBusiness);
        Assert.Equal(DelphiDateUtil.UnixToDateTime(unix + 8 * 60 * 60), shop.dCreateDate);
        Assert.Equal(9, shop.nCareValue);
        Assert.Equal(1, shop.SellItemCount);
        Assert.Equal(2, shop.SelledItemCount);
        Assert.Equal(3, shop.StorageItemCount);
    }

    [Fact]
    public void MySql_GetUserShopInfo_ReadsCreateDateAsDateTimeColumn()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        var unit = new TMySqlUserShopDB(new FakeDbLayerHost { DataBase = db });
        unit.DoInit();

        var createDate = new DateTime(2020, 9, 16, 12, 0, 0, DateTimeKind.Local);
        db.For("UserShop_GetUserShopInfo").AddRow(5, "张三", "我的店", 1, createDate, 9, 1, 2, 3);

        bool found = unit.GetUserShopInfo("张三", out TUserShop shop);

        Assert.True(found);
        // MySQL 侧直接读 DateTime 列，**不加 8 小时**（与 SQLite 语义不同，逐字保留）。
        Assert.Equal(createDate, shop.dCreateDate);
    }

    [Fact]
    public void Sqlite_GetUserShopInfo_MissingRow_ReturnsFalseAndZeroedRecord()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteUserShopDB(new FakeDbLayerHost { DataBase = db });
        unit.DoInit();

        bool found = unit.GetUserShopInfo("不存在", out TUserShop shop);

        Assert.False(found);
        Assert.Equal(0, shop.ShopID);
        Assert.Equal("", shop.sMasterName.Value);
        // 原文 finally 里 Reset（无 except，异常外抛到基类包装器）。
        Assert.Equal(2, db.For("UserShop_GetUserShopInfo").ResetCount);
    }

    // ------------------------------------------------------------------ 事务边界

    [Fact]
    public void Sqlite_DeleteItem_ExecutesOneMultiStatementScriptInATransaction()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteUserShopDB(new FakeDbLayerHost { DataBase = db });
        unit.DoInit();

        bool ok = unit.DeleteItem(42, 7);

        Assert.True(ok);
        Assert.Single(db.ExecutedSql);
        string sql = db.ExecutedSql[0];
        Assert.Contains("delete from ItemElementAdd where ItemType = 1 and ParentID = 42 and ItemIndex = 7;", sql, StringComparison.Ordinal);
        Assert.Contains("delete from UserShopItem where ShopID = 42 and ItemID = 7;", sql, StringComparison.Ordinal);
        Assert.Contains("-------------------------------", sql, StringComparison.Ordinal);
        Assert.Contains("\r\n", sql, StringComparison.Ordinal);
        Assert.Equal(new[] { "begin", "commit" }, db.Transactions.ToArray());
    }

    [Fact]
    public void MySql_DeleteItem_OmitsTheCommentSeparatorLine()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        var unit = new TMySqlUserShopDB(new FakeDbLayerHost { DataBase = db });
        unit.DoInit();

        bool ok = unit.DeleteItem(42, 7);

        Assert.True(ok);
        string sql = db.ExecutedSql.Single();
        // 原文 1941 行把 '-------------------------------' 注释掉了 → MySQL 版没有该分隔行。
        Assert.DoesNotContain("-------------------------------", sql, StringComparison.Ordinal);
        Assert.Contains("delete from UserShopItem where ShopID = 42 and ItemID = 7;", sql, StringComparison.Ordinal);
    }

    [Fact]
    public void Sqlite_ShopDelete_KeepsTheCommentSeparatorLine()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteUserShopDB(new FakeDbLayerHost { DataBase = db });
        unit.DoInit();

        bool ok = unit.ShopDelete(42);

        Assert.True(ok);
        string sql = db.ExecutedSql.Single();
        Assert.Contains("-------------------------------", sql, StringComparison.Ordinal);
        Assert.Contains("delete from UserShop where ShopID = 42;", sql, StringComparison.Ordinal);
    }

    [Fact]
    public void Sqlite_HumanRename_RunsTwoUpdatesInOneTransaction()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteUserShopDB(new FakeDbLayerHost { DataBase = db });
        unit.DoInit();

        bool ok = unit.HumanRename("旧名", "新名");

        Assert.True(ok);
        var rename2 = db.For("UserShop_UpdateShopName2");
        Assert.Equal("新名", rename2.LastBinds[0].Text);
        Assert.Equal("旧名", rename2.LastBinds[1].Text);
        var buyer = db.For("UserShop_ShopItemBuyerRenameName");
        Assert.Equal("新名", buyer.LastBinds[0].Text);
        Assert.Equal("旧名", buyer.LastBinds[1].Text);
        Assert.Equal(new[] { "begin", "commit" }, db.Transactions.ToArray());
    }

    [Fact]
    public void MySql_UpdateHumanBusiness_BindsBoolThenName()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        var unit = new TMySqlUserShopDB(new FakeDbLayerHost { DataBase = db });
        unit.DoInit();

        db.For("UserShop_UpdateHumanBusiness").QueryResult = false;
        bool ok = unit.UpdateHumanBusiness("张三", true);

        var stmt = db.For("UserShop_UpdateHumanBusiness");
        Assert.Equal("bool", stmt.LastBinds[0].Kind);
        Assert.Equal(true, stmt.LastBinds[0].Value);
        Assert.Equal("text", stmt.LastBinds[1].Kind);
        Assert.Equal("张三", stmt.LastBinds[1].Text);
        Assert.False(ok);
    }

    /// <summary>SQLite 的 DoUpdateHumanBusiness 判据是 <c>Step = SQLITE_ROW</c>（不是 OK/DONE）。</summary>
    [Fact]
    public void Sqlite_UpdateHumanBusiness_SucceedsOnlyOnRow()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteUserShopDB(new FakeDbLayerHost { DataBase = db });
        unit.DoInit();

        // 无预设行 → Step 返回 SQLITE_DONE → False（尽管语句"执行成功"）
        Assert.False(unit.UpdateHumanBusiness("张三", true));

        // 预设一行 → SQLITE_ROW → True
        db.For("UserShop_UpdateHumanBusiness").AddRow(1);
        Assert.True(unit.UpdateHumanBusiness("张三", true));
    }

    // ------------------------------------------------------------------ 列表容器语义

    [Fact]
    public void UserShopItemList_AddCopiesAndDeleteIsBoundsSafe()
    {
        var list = new UserShopItemList();
        var item = new TUserShopItem { ItemID = 1, nPrice = 10 };
        TUserShopItem added = list.Add(item);

        Assert.NotSame(item, added);
        item.nPrice = 999;
        Assert.Equal(10, list[0]!.nPrice);   // 原记录后续改动不影响已入列元素

        list.Delete(-1);
        list.Delete(5);
        Assert.Equal(1, list.Count);
        Assert.Null(list[5]);
    }

    [Fact]
    public void UserShopList_AddCopies()
    {
        var list = new UserShopList();
        var shop = new TUserShop { ShopID = 1, sShopName = "甲" };
        list.Add(shop);
        shop.sShopName = "乙";
        Assert.Equal("甲", list[0]!.sShopName.Value);
    }

    [Fact]
    public void AuctionItemList_InsertGoesToTheFront()
    {
        var list = new TAuctionItemList();
        list.Add(new TAuctionRecord { AuctionID = 1 });
        list.Insert(new TAuctionRecord { AuctionID = 2 });
        Assert.Equal(2, list[0]!.AuctionID);   // 原文 FList.Insert(0, ...)
        Assert.Equal(1, list[1]!.AuctionID);
    }
}
