// 源单元（文件头证据）：
//   Source/M2Engine/SqliteAuctionDB.pas
//   Source/M2Engine/MySqlAuctionDB.pas
//
// 测试组 1：SQL 文本保真（**逐字 + 指纹 + 回读比对**）
//   1. 实现真正绑到 FDB 的 21 条语句，其 SHA-256 必须等于从 GBK 原文机械抽取的期望值
//      （DbLayerFingerprints.SqliteAuctionDB.cs / .MySqlAuctionDB.cs，由 _recon/p3-fingerprints.mjs
//       直接从 p3-<unit>.json 生成，**不经过 C# 常量生成步骤** ⇒ 抽取与实现是两条独立路径）。
//   2. AddSQLStatement 的数量与 label 顺序必须与原文 DoInit 一致（21 条）。
//   3. 两个方言的差异面必须**恰好**等于"已核实的 8 条"：
//        Auction_UpdateAuctionItemFail / Auction_QueryAuctionItemSuccess / Auction_QueryAuctionItems /
//        Auction_QueryOneItem / Auction_GetMyAuctioningItemsCount / Auction_GetMySellFailItemsCount /
//        Auction_JoinItemBid / Auction_BuyItem
//      —— 差异全是时间写法：SQLite <c>strftime("%s","now")</c> / <c>(AddDateTime + AuctionTime * 3600)</c>
//      vs MySQL <c>CURRENT_TIMESTAMP</c> / <c>TIMESTAMPDIFF(SECOND, CURRENT_TIMESTAMP, date_add(...))</c>。
//      其余 13 条必须逐字相同 —— 防止"顺手统一方言"。
//
// 测试组 2：DoInit / DoFinal 生命周期 + 公开包装层的 Lock 模板（DbBases.cs TAuctionDB：317-454）。
//
// **测试绝不连真实数据库**：全部 DB 访问注入 DbLayerTestKit 的内存实现。

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using GXX.M2Server.DbLayer;

namespace GXX.M2Server.Tests;

public class DbLayerAuctionSqlFidelityTests
{
    /// <summary>SQLite 侧的库文件接缝替换（AuctionDB 的 DoInit 与 M2DataDB 不同，见具体测试）。</summary>
    private static void IsolateSqliteFiles()
    {
        TSqliteM2DataDB.ExtractFilePath = () => @"C:\M2Test\";
        TSqliteM2DataDB.DirectoryExists = _ => true;
        TSqliteM2DataDB.ForceDirectories = _ => { };
        TSqliteM2DataDB.FileExists = _ => true;
        TSqliteM2DataDB.DeleteFile = _ => { };
        TSqliteM2DataDB.SaveResourceToFile = (_, _) => { };
        TSqliteM2DataDB.Sqlite3ConfigMultithread = () => { };
    }

    /// <summary>原文 DoInit 的 21 条 AddSQLStatement label，按原文出现顺序（两个方言相同）。</summary>
    private static readonly string[] OriginalStatementOrder =
    {
        "Auction_UpdateAuctionItemFail",
        "Auction_UpdateAuctionItemSuccess",
        "Auction_QueryAuctionItemSuccess",
        "Auction_QueryAuctionItems",
        "Auction_GetMyItemsCount",
        "Auction_QueryAttentionItems",
        "Auction_GetMyAttentionItemsCount",
        "Auction_QueryOneItem",
        "Auction_GetMyAuctioningItemsCount",
        "Auction_GetMySellFailItemsCount",
        "Auction_GetMyBuyOKItemsCount",
        "Auction_GetMaxAuctionID",
        "Auction_InsertAuctionItem",
        "Auction_JoinItemBid",
        "Auction_BuyItem",
        "Auction_AddAttentionItem",
        "Auction_CheckInAttentionItem",
        "Auction_DeleteAuctionItem",
        "Auction_HumanRename",
        "Auction_LastBidderRename",
        "Auction_AttentionRename",
    };

    // ---------------------------------------------------------------- 1. 指纹

    [Fact]
    public void Sqlite_DoInit_BindsExactlyTheExtractedSql()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteAuctionDB(new FakeDbLayerHost { DataBase = db }, db);

        unit.DoInit();

        Assert.Equal(SqliteAuctionDbFingerprints.Statements.Length, db.Created.Count);
        var actual = db.Created.ToDictionary(s => s.Label, s => DbLayerTestKit.Sha256(s.Sql), StringComparer.Ordinal);
        foreach ((string label, string sha) in SqliteAuctionDbFingerprints.Statements)
        {
            Assert.True(actual.ContainsKey(label), $"SqliteAuctionDB: 缺少语句 {label}");
            Assert.True(sha == actual[label],
                $"SqliteAuctionDB: {label} 的 SQL 与原文抽取的 SHA-256 不一致（期望 {sha}，实际 {actual[label]}）");
        }
    }

    [Fact]
    public void MySql_DoInit_BindsExactlyTheExtractedSql()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        var unit = new TMySqlAuctionDB(new FakeDbLayerHost { DataBase = db }, db);

        unit.DoInit();

        Assert.Equal(MySqlAuctionDbFingerprints.Statements.Length, db.Created.Count);
        var actual = db.Created.ToDictionary(s => s.Label, s => DbLayerTestKit.Sha256(s.Sql), StringComparer.Ordinal);
        foreach ((string label, string sha) in MySqlAuctionDbFingerprints.Statements)
        {
            Assert.True(actual.ContainsKey(label), $"MySqlAuctionDB: 缺少语句 {label}");
            Assert.True(sha == actual[label],
                $"MySqlAuctionDB: {label} 的 SQL 与原文抽取的 SHA-256 不一致（期望 {sha}，实际 {actual[label]}）");
        }
    }

    // ---------------------------------------------------------------- 2. 语句数量与顺序

    [Fact]
    public void Sqlite_DoInit_AddsTheOriginalStatementLabelsInOrder()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles();
        var db = new FakeSqliteDatabase();
        new TSqliteAuctionDB(new FakeDbLayerHost { DataBase = db }, db).DoInit();

        Assert.Equal(OriginalStatementOrder, db.AddedNames);
    }

    [Fact]
    public void MySql_DoInit_AddsTheOriginalStatementLabelsInOrder()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        new TMySqlAuctionDB(new FakeDbLayerHost { DataBase = db }, db).DoInit();

        Assert.Equal(OriginalStatementOrder, db.AddedNames);
    }

    // ---------------------------------------------------------------- 3. 方言差异面

    /// <summary>
    /// 21 条共有语句中逐字相同 13 条、不同 8 条（全部是"剩余时间/时间戳"的方言写法）。
    /// 本测试同时锁死"不同的那 8 条"与"其余 13 条必须逐字相同"。
    /// </summary>
    [Fact]
    public void Dialects_DifferInExactlyTheEightTimeStatements()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles();
        var sqliteDb = new FakeSqliteDatabase();
        var mySqlDb = new FakeMySqlDatabase();
        new TSqliteAuctionDB(new FakeDbLayerHost { DataBase = sqliteDb }, sqliteDb).DoInit();
        new TMySqlAuctionDB(new FakeDbLayerHost { DataBase = mySqlDb }, mySqlDb).DoInit();

        var a = sqliteDb.Created.ToDictionary(s => s.Label, s => s.Sql, StringComparer.Ordinal);
        var b = mySqlDb.Created.ToDictionary(s => s.Label, s => s.Sql, StringComparer.Ordinal);

        List<string> common = a.Keys.Intersect(b.Keys, StringComparer.Ordinal).ToList();
        List<string> differing = common.Where(k => !string.Equals(a[k], b[k], StringComparison.Ordinal))
            .OrderBy(k => k, StringComparer.Ordinal).ToList();

        Assert.Equal(21, common.Count);
        Assert.Equal(new[]
        {
            "Auction_BuyItem",
            "Auction_GetMyAuctioningItemsCount",
            "Auction_GetMySellFailItemsCount",
            "Auction_JoinItemBid",
            "Auction_QueryAuctionItemSuccess",
            "Auction_QueryAuctionItems",
            "Auction_QueryOneItem",
            "Auction_UpdateAuctionItemFail",
        }, differing.OrderBy(x => x, StringComparer.Ordinal));

        // SQLite：时间用 strftime("%s","now") / (AddDateTime + AuctionTime * 3600)
        Assert.Contains("strftime(\"%s\", \"now\")", a["Auction_JoinItemBid"], StringComparison.Ordinal);
        Assert.Contains("strftime(\"%s\", \"now\")", a["Auction_BuyItem"], StringComparison.Ordinal);
        Assert.Contains("(AddDateTime + AuctionTime * 3600)", a["Auction_QueryAuctionItems"], StringComparison.Ordinal);
        Assert.Contains("(A.AddDateTime + A.AuctionTime * 3600)", a["Auction_QueryAuctionItemSuccess"], StringComparison.Ordinal);

        // MySQL：时间用 CURRENT_TIMESTAMP / TIMESTAMPDIFF + date_add(... interval AuctionTime hour)
        Assert.Contains("CURRENT_TIMESTAMP", b["Auction_JoinItemBid"], StringComparison.Ordinal);
        Assert.Contains("CURRENT_TIMESTAMP", b["Auction_BuyItem"], StringComparison.Ordinal);
        Assert.Contains("TIMESTAMPDIFF(SECOND, CURRENT_TIMESTAMP, date_add(AddDateTime, interval AuctionTime hour))",
            b["Auction_QueryAuctionItems"], StringComparison.Ordinal);

        // 其余 13 条逐字相同。
        foreach (string key in common.Except(differing, StringComparer.Ordinal))
        {
            Assert.True(string.Equals(a[key], b[key], StringComparison.Ordinal),
                $"{key} 本应逐字相同，但两边不同");
        }
    }

    /// <summary>两条"名字相同、语义不同"的语句必须各自保留：JoinItemBid 不动 TradingStatus，BuyItem 置 2。</summary>
    [Fact]
    public void JoinItemBidAndBuyItem_AreDistinctStatementsInBothDialects()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles();
        var sqliteDb = new FakeSqliteDatabase();
        new TSqliteAuctionDB(new FakeDbLayerHost { DataBase = sqliteDb }, sqliteDb).DoInit();

        string join = sqliteDb.For("Auction_JoinItemBid").Sql;
        string buy = sqliteDb.For("Auction_BuyItem").Sql;

        Assert.DoesNotContain("TradingStatus", join, StringComparison.Ordinal);
        Assert.Contains("TradingStatus = 2", buy, StringComparison.Ordinal);
        Assert.NotEqual(join, buy);
    }

    /// <summary>MySQL 的 DoDeleteAuctionItem 动态 SQL 里 <c>sWhere</c> 出现 11 次（原文 609-614）。</summary>
    [Fact]
    public void MySqlDeleteAuctionItemScript_HasElevenRuntimeValueSlots()
    {
        Assert.Equal(11, MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_RuntimeValueCount);
        Assert.Equal("@@sWhere@@", MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_RuntimeValue_sWhere);

        string[] parts =
        {
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P0,
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P1,
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P2,
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P3,
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P4,
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P5,
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P6,
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P7,
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P8,
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P9,
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P10,
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P11,
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P12,
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P13,
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P14,
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P15,
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P16,
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P17,
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P18,
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P19,
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P20,
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P21,
            MySqlAuctionDbScripts.DoDeleteAuctionItem_L609_Sql_P22,
        };
        Assert.Equal(23, parts.Length);

        string template = string.Concat(parts);
        // 11 个运行时值 = 9 个 sWhere + 2 个 IntToStr(AuctionID)。
        Assert.Equal(9, template.Split("@@sWhere@@", StringSplitOptions.None).Length - 1);
        Assert.Equal(2, template.Split("@@IntToStr(AuctionID)@@", StringSplitOptions.None).Length - 1);
        // 原文 609-614 的删除顺序（sWhere 用同一个 where 串）。
        Assert.StartsWith("DELETE FROM ItemElementAdd", template, StringComparison.Ordinal);
        Assert.Contains("DELETE FROM Items", template, StringComparison.Ordinal);
        Assert.EndsWith("DELETE FROM AuctionData WHERE AuctionID = @@IntToStr(AuctionID)@@;\r\n", template, StringComparison.Ordinal);
    }

    // ---------------------------------------------------------------- 4. 公开包装层（DbBases.cs TAuctionDB）

    /// <summary>
    /// <c>GetMyItemsPageCount</c> = <c>(Count + AUCTION_PAGE_COUNT - 1) div AUCTION_PAGE_COUNT</c>
    /// （SqliteAuctionDB.pas:1186-1187，<c>AUCTION_PAGE_COUNT = 7</c>）；公开包装层 Lock/UnLock 恰好一次。
    /// </summary>
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(7, 1)]
    [InlineData(8, 2)]
    [InlineData(14, 2)]
    public void Sqlite_GetMyItemsPageCount_DividesBySeven(int rawCount, int expectedPages)
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles();
        var db = new FakeSqliteDatabase();
        var host = new FakeDbLayerHost { DataBase = db };
        var unit = new TSqliteAuctionDB(host, db);
        unit.DoInit();
        if (rawCount >= 0) db.For("Auction_GetMyItemsCount").AddRow(rawCount);

        int pages = unit.GetMyItemsPageCount("张三");

        Assert.Equal(expectedPages, pages);
        Assert.Equal(new[] { "张三" }, db.For("Auction_GetMyItemsCount").LastBinds.Select(b => b.Text));
        Assert.Equal(1, host.LockCount);
        Assert.Equal(1, host.UnLockCount);
    }

    /// <summary>MySQL 姊妹实现同样除以 7，但走 <c>Query() &amp;&amp; Fetch()</c>（MySqlAuctionDB.pas:1164-1177）。</summary>
    [Theory]
    [InlineData(0, 0)]
    [InlineData(8, 2)]
    public void MySql_GetMyItemsPageCount_DividesBySevenViaQueryFetch(int rawCount, int expectedPages)
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        var host = new FakeDbLayerHost { DataBase = db };
        var unit = new TMySqlAuctionDB(host, db);
        unit.DoInit();
        db.For("Auction_GetMyItemsCount").AddRow(rawCount);

        int pages = unit.GetMyItemsPageCount("张三");

        Assert.Equal(expectedPages, pages);
        Assert.Equal(1, db.For("Auction_GetMyItemsCount").QueryCount);
        Assert.Equal(1, host.LockCount);
        Assert.Equal(1, host.UnLockCount);
    }

    /// <summary>
    /// 原文 M2DataCommon.pas:1236-1264（DbBases.cs:701-733）：Min/Max 互换守卫
    /// （<c>if (MinPrices &lt;&gt; 0) and (MaxPrices &lt;&gt; 0) and (MinPrices &gt; MaxPrices)</c> 时交换）。
    /// 交换后落到 SQL 的 <c>and A.SellingPrice &gt;= 100</c> / <c>&lt;= 900</c>
    /// （脚本常量 DoQueryAllItems_L833_sm_Sql_P8 / L838_P10）。
    /// </summary>
    [Fact]
    public void QueryAllItems_SwapsMinAndMaxPricesWhenReversed()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles();
        var db = new FakeSqliteDatabase();
        var host = new FakeDbLayerHost { DataBase = db };
        var unit = new TSqliteAuctionDB(host, db);
        unit.DoInit();

        var list = new TAuctionItemList();
        unit.QueryAllItems("", 0, "", 1, 0, 0, 0, true, 0, 900, 100, list);

        string sql = db.For("Auction_QueryAllItems").Sql;
        Assert.Contains(" and A.SellingPrice >= 100", sql, StringComparison.Ordinal);
        Assert.Contains(" and A.SellingPrice <= 900", sql, StringComparison.Ordinal);
        // limit ? offset ? —— 第 1 页：offset = (1 - 1) * 7 = 0。
        Assert.EndsWith(" limit ? offset ?;", sql, StringComparison.Ordinal);
        Assert.Equal(new[] { "", "7", "0" }, db.For("Auction_QueryAllItems").LastBinds.Select(b => b.Text));
        Assert.Equal(1, host.LockCount);
        Assert.Equal(1, host.UnLockCount);
    }

    /// <summary>Min/Max 不互换：只有 <c>MaxPrices = 0</c> 时不生成上限子句（原文守卫要求两者都非 0）。</summary>
    [Fact]
    public void QueryAllItems_DoesNotSwapWhenOneBoundIsZero()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteAuctionDB(new FakeDbLayerHost { DataBase = db }, db);
        unit.DoInit();

        unit.QueryAllItems("", 0, "", 2, 0, 0, 0, true, 0, 900, 0, new TAuctionItemList());

        string sql = db.For("Auction_QueryAllItems").Sql;
        Assert.Contains(" and A.SellingPrice >= 900", sql, StringComparison.Ordinal);
        Assert.DoesNotContain("SellingPrice <=", sql, StringComparison.Ordinal);
        // 第 2 页：offset = (2 - 1) * 7 = 7。
        Assert.Equal("7", db.For("Auction_QueryAllItems").LastBinds[2].Text);
    }

    /// <summary>SortField 不在 0..3 时 sOrderBy 保持空串（SqliteAuctionDB.pas:828-830 原文如此）。</summary>
    [Fact]
    public void QueryAllItems_UnknownSortField_LeavesOrderByEmpty()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteAuctionDB(new FakeDbLayerHost { DataBase = db }, db);
        unit.DoInit();

        unit.QueryAllItems("", 0, "", 1, 0, 0, 99, true, 0, 0, 0, new TAuctionItemList());

        string sql = db.For("Auction_QueryAllItems").Sql;
        Assert.DoesNotContain("order by", sql, StringComparison.Ordinal);
        Assert.EndsWith(")  limit ? offset ?;", sql, StringComparison.Ordinal);
    }

    /// <summary>SortField = 0 且降序 → 原文 869 行的 <c>' order by AuctionID desc'</c>。</summary>
    [Fact]
    public void QueryAllItems_SortFieldZeroDescending_UsesAuctionIdDesc()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteAuctionDB(new FakeDbLayerHost { DataBase = db }, db);
        unit.DoInit();

        unit.QueryAllItems("", 0, "", 1, 0, 0, 0, false, 0, 0, 0, new TAuctionItemList());

        Assert.Contains(SqliteAuctionDbScripts.DoQueryAllItems_L869_sOrderBy,
            db.For("Auction_QueryAllItems").Sql, StringComparison.Ordinal);
    }

    /// <summary>结果集 → TAuctionItemList：13 列顺序（SqliteAuctionDB.pas:851-863）+ 每条都回调 Owner.LoadItemFromDB。</summary>
    [Fact]
    public void QueryAllItems_ResultSet_MapsTheThirteenColumnsInOrder()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles();
        var db = new FakeSqliteDatabase();
        var host = new FakeDbLayerHost { DataBase = db };
        var unit = new TSqliteAuctionDB(host, db);
        unit.DoInit();
        // DoQueryAllItems 用的是 DoInit 之后才 AddSQLStatement("Auction_QueryAllItems") 的动态语句；
        // 先预登记同名语句并塞结果集（SQLite3DataBase 的 AddSQLStatement 同名复用返回同一实例）。
        ((ScriptedSqliteStatement)db.AddSQLStatement("Auction_QueryAllItems")).AddRow(
            11, "李四", 1600000000, 48, 3600, 100, 500, 1, 250, "王五", 0, 0, 1);

        var list = new TAuctionItemList();
        int n = unit.QueryAllItems("", 0, "李四", 1, 0, 0, 0, true, 0, 0, 0, list);

        Assert.Equal(1, n);
        Assert.Equal(1, list.Count);
        TAuctionRecord? r = list[0];
        Assert.NotNull(r);
        Assert.Equal(11, r!.AuctionID);
        Assert.Equal("李四", r.HumanName);
        Assert.Equal(48, r.AuctionTime);
        Assert.Equal(3600, r.TimeLeft);
        Assert.Equal(100u, r.StartingPrice);
        Assert.Equal(500u, r.SellingPrice);
        Assert.Equal(1, r.CurrencyType);
        Assert.Equal(250, r.LastBidPrice);
        Assert.Equal("王五", r.LastBidder);
        Assert.Equal(0, r.TradingStatus);
        Assert.False(r.IsItemGive);
        Assert.True(r.IsAttention);
        Assert.Equal(new[] { "11/5/0" }, host.LoadedItems);
    }

    /// <summary>空结果集 → 0，且语句仍被 Reset + Finalize（原文 finally）。</summary>
    [Fact]
    public void QueryAllItems_EmptyResultSet_ReturnsZeroAndFinalizes()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteAuctionDB(new FakeDbLayerHost { DataBase = db }, db);
        unit.DoInit();

        var list = new TAuctionItemList();
        int n = unit.QueryAllItems("", 0, "", 1, 0, 0, 0, true, 0, 0, 0, list);

        Assert.Equal(0, n);
        Assert.Empty(list.Snapshot());
        Assert.Equal(1, db.For("Auction_QueryAllItems").FinalizeCount);
    }

    /// <summary>SQL 注入字符作为 <c>itemName</c> 原样进入 LIKE 串（原文 843 行不做转义），但不改变语句骨架。</summary>
    [Fact]
    public void QueryAllItems_SqlInjectionInItemName_IsConcatenatedVerbatim()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteAuctionDB(new FakeDbLayerHost { DataBase = db }, db);
        unit.DoInit();
        const string evil = "'; drop table Items; --";

        unit.QueryAllItems(evil, 0, "", 1, 0, 0, 0, true, 0, 0, 0, new TAuctionItemList());

        string sql = db.For("Auction_QueryAllItems").Sql;
        // 原文把 ItemName 直接拼进 LIKE（无参数化）——这是原文行为，测试锁定它。
        Assert.Equal(2, sql.Split(evil, StringSplitOptions.None).Length - 1);
        Assert.Contains("limit ? offset ?;", sql, StringComparison.Ordinal);
    }

    /// <summary>itemColors 位组合 → <c>btAuctionItemColors</c> 的 6 位顺序（SqliteAuctionDB.pas:1111-1147 / 788-824）。</summary>
    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(4, true)]
    [InlineData(8, true)]
    [InlineData(16, true)]
    [InlineData(32, true)]
    [InlineData(63, true)]
    public void QueryAllItems_ItemColors_AddsTheColorFilterOnlyWhenNonZero(int itemColors, bool expectFilter)
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteAuctionDB(new FakeDbLayerHost { DataBase = db }, db);
        unit.DoInit();

        unit.QueryAllItems("", 0, "", 1, 0, itemColors, 0, true, 0, 0, 0, new TAuctionItemList());

        string sql = db.For("Auction_QueryAllItems").Sql;
        if (expectFilter)
        {
            Assert.Contains(")  and ItemColor in (", sql, StringComparison.Ordinal);
            Assert.DoesNotContain(",,", sql, StringComparison.Ordinal);
            Assert.DoesNotContain("(,", sql, StringComparison.Ordinal);
        }
        else
        {
            Assert.DoesNotContain("ItemColor in (", sql, StringComparison.Ordinal);
        }
    }

    // ================================================================
    // 5. ★ 动态拼接的**逐字**对账（本轮抓出 6 个真缺陷后补的守卫）
    //
    // 生成器把 `sm.Sql := sm.Sql + '...'` 渲染成**累积快照**常量：`_L<行>_sm_Sql_P<n>` 里的字面段
    // 既含本步新增的文本，也含上一状态尾部的文本（例如 ") " 或 ")"）。因此"追加式"实现必须
    // 去掉与上一状态**重叠的那一段**，否则会多出右括号、甚至把整条基串再拼一次。
    //
    // 之前这里只做 `Contains` 弱断言 —— 多一个 ')' 照样通过。下面用**手写期望串**（字面量直接
    // 抄自原文 SqliteAuctionDB.pas:781/785/802/808/813/818/843 与 MySqlAuctionDB.pas:754/765/802/808/813/818/823）
    // 把基串之后的**整段尾部**逐字钉死，包括空格数量。
    // ================================================================

    private const string QueryAllItemsExpectedTail =
        "42)  and (ItemGroup = 5) and ItemColor in (0,0,0) and A.CurrencyType = 2"
        + " and A.SellingPrice >= 1 and A.SellingPrice <= 9"
        + " and ((A.ItemDBName like \"%evil%\") or (A.ItemName like \"%evil%\"))"
        + " order by AuctionID desc limit ? offset ?;";

    private const string PageCountExpectedTail =
        " and ItemGroup = 5 and ItemColor in (0,0,0) and CurrencyType = 2"
        + " and SellingPrice >= 1 and SellingPrice <= 9"
        + " and ((ItemDBName like \"%evil%\") or (ItemName like \"%evil%\"))";

    [Fact]
    public void Sqlite_QueryAllItems_MaximalFilters_MatchTheOriginalConcatenationVerbatim()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteAuctionDB(new FakeDbLayerHost { DataBase = db }, db);
        unit.DoInit();

        unit.QueryAllItems("evil", 5, "h", 1, 42, 37, 0, false, 3, 1, 9, new TAuctionItemList());

        string sql = db.For("Auction_QueryAllItems").Sql;
        string prefix = SqliteAuctionDbScripts.DoQueryAllItems_L775_sm_Sql_P0;
        Assert.StartsWith(prefix, sql, StringComparison.Ordinal);
        Assert.Equal(QueryAllItemsExpectedTail, sql.Substring(prefix.Length));
    }

    [Fact]
    public void MySql_QueryAllItems_MaximalFilters_MatchTheOriginalConcatenationVerbatim()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        var unit = new TMySqlAuctionDB(new FakeDbLayerHost { DataBase = db }, db);
        unit.DoInit();

        unit.QueryAllItems("evil", 5, "h", 1, 42, 37, 0, false, 3, 1, 9, new TAuctionItemList());

        string sql = db.For("Auction_QueryAllItems").Sql;
        string prefix = MySqlAuctionDbScripts.DoQueryAllItems_L754_sm_Sql_P0;
        Assert.StartsWith(prefix, sql, StringComparison.Ordinal);
        Assert.Equal(QueryAllItemsExpectedTail, sql.Substring(prefix.Length));
    }

    [Fact]
    public void Sqlite_GetAllItemsPageCount_MaximalFilters_MatchTheOriginalConcatenationVerbatim()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteAuctionDB(new FakeDbLayerHost { DataBase = db }, db);
        unit.DoInit();

        unit.GetAllItemsPageCount("evil", 5, 37, 3, 1, 9);

        string sql = db.For("Auction_GetAllItemsCount").Sql;
        string prefix = SqliteAuctionDbScripts.DoGetAllItemsPageCount_L1103_sm_Sql;
        Assert.StartsWith(prefix, sql, StringComparison.Ordinal);
        Assert.Equal(PageCountExpectedTail, sql.Substring(prefix.Length));
    }

    [Fact]
    public void MySql_GetAllItemsPageCount_MaximalFilters_MatchTheOriginalConcatenationVerbatim()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        var unit = new TMySqlAuctionDB(new FakeDbLayerHost { DataBase = db }, db);
        unit.DoInit();

        unit.GetAllItemsPageCount("evil", 5, 37, 3, 1, 9);

        string sql = db.For("Auction_GetAllItemsCount").Sql;
        string prefix = MySqlAuctionDbScripts.DoGetAllItemsPageCount_L1082_sm_Sql;
        Assert.StartsWith(prefix, sql, StringComparison.Ordinal);
        Assert.Equal(PageCountExpectedTail, sql.Substring(prefix.Length));
    }

    /// <summary>
    /// 兜底守卫：动态拼出来的 SQL **不得**含拼接点占位符 `@@…@@`（那说明把生成器的运行时占位常量
    /// 当字面量拼了进去），也不得出现 `") )"`（快照分片的前导 `)` 被重复拼入）。
    /// </summary>
    [Fact]
    public void DynamicSql_NeverContainsPlaceholdersOrDuplicatedParens()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles();
        var sqliteDb = new FakeSqliteDatabase();
        var mySqlDb = new FakeMySqlDatabase();
        var sqlite = new TSqliteAuctionDB(new FakeDbLayerHost { DataBase = sqliteDb }, sqliteDb);
        var mySql = new TMySqlAuctionDB(new FakeDbLayerHost { DataBase = mySqlDb }, mySqlDb);
        sqlite.DoInit();
        mySql.DoInit();

        sqlite.QueryAllItems("evil", 5, "h", 1, 42, 37, 0, false, 3, 1, 9, new TAuctionItemList());
        sqlite.GetAllItemsPageCount("evil", 5, 37, 3, 1, 9);
        mySql.QueryAllItems("evil", 5, "h", 1, 42, 37, 0, false, 3, 1, 9, new TAuctionItemList());
        mySql.GetAllItemsPageCount("evil", 5, 37, 3, 1, 9);

        foreach ((string label, FakeSqliteDatabase d) in new[]
                 {
                     ("Auction_QueryAllItems", sqliteDb), ("Auction_GetAllItemsCount", sqliteDb),
                 })
        {
            string sql = d.For(label).Sql;
            Assert.DoesNotContain("@@", sql, StringComparison.Ordinal);
            Assert.DoesNotContain(") )", sql, StringComparison.Ordinal);
        }
        foreach ((string label, FakeMySqlDatabase d) in new[]
                 {
                     ("Auction_QueryAllItems", mySqlDb), ("Auction_GetAllItemsCount", mySqlDb),
                 })
        {
            string sql = d.For(label).Sql;
            Assert.DoesNotContain("@@", sql, StringComparison.Ordinal);
            Assert.DoesNotContain(") )", sql, StringComparison.Ordinal);
        }
    }

    /// <summary>只启用 itemGroup 一个过滤条件时，不得把整条基串拼第二遍（原文只追加 ' and ItemGroup = '）。</summary>
    [Fact]
    public void ItemGroupOnlyFilter_AppendsTheClauseOnceForBothDialectsAndBothStatements()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles();
        var sqliteDb = new FakeSqliteDatabase();
        var mySqlDb = new FakeMySqlDatabase();
        var sqlite = new TSqliteAuctionDB(new FakeDbLayerHost { DataBase = sqliteDb }, sqliteDb);
        var mySql = new TMySqlAuctionDB(new FakeDbLayerHost { DataBase = mySqlDb }, mySqlDb);
        sqlite.DoInit();
        mySql.DoInit();

        sqlite.QueryAllItems("", 5, "", 1, 42, 0, 0, true, 0, 0, 0, new TAuctionItemList());
        sqlite.GetAllItemsPageCount("", 5, 0, 0, 0, 0);
        mySql.QueryAllItems("", 5, "", 1, 42, 0, 0, true, 0, 0, 0, new TAuctionItemList());
        mySql.GetAllItemsPageCount("", 5, 0, 0, 0, 0);

        Assert.Equal("42)  and (ItemGroup = 5) order by AuctionID desc limit ? offset ?;",
            sqliteDb.For("Auction_QueryAllItems").Sql.Substring(SqliteAuctionDbScripts.DoQueryAllItems_L775_sm_Sql_P0.Length));
        Assert.Equal(" and ItemGroup = 5",
            sqliteDb.For("Auction_GetAllItemsCount").Sql.Substring(SqliteAuctionDbScripts.DoGetAllItemsPageCount_L1103_sm_Sql.Length));
        Assert.Equal("42)  and (ItemGroup = 5) order by AuctionID desc limit ? offset ?;",
            mySqlDb.For("Auction_QueryAllItems").Sql.Substring(MySqlAuctionDbScripts.DoQueryAllItems_L754_sm_Sql_P0.Length));
        Assert.Equal(" and ItemGroup = 5",
            mySqlDb.For("Auction_GetAllItemsCount").Sql.Substring(MySqlAuctionDbScripts.DoGetAllItemsPageCount_L1082_sm_Sql.Length));
    }
}
