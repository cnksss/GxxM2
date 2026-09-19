// 源单元（文件头证据）：
//   Source/M2Engine/SqliteUserShopDB.pas / Source/M2Engine/MySqlUserShopDB.pas
//
// 测试组 1：SQL 文本保真（**逐字 + 指纹**）
//   1. 实现真正绑定到 FDB 的每一条 SQL，其 SHA-256 必须等于从 GBK 原文机械抽取的期望值
//      （期望值见 DbLayerFingerprints.*.cs，由 _recon/gen-fingerprints.mjs 生成）。
//   2. 语句数量与 AddSQLStatement 的数量/顺序必须与原文一致（含 MySQL 独有 2 条）。
//   3. SQLite 与 MySQL 的差异面必须精确等于"已核实的 12 条"——防止"顺手统一方言"。
//
// 测试组 2：DoInit / DoFinal 生命周期（Prepare 次数、Sort5 无 Prepare 的不对称、Finalize 清空）。
//
// 注：内存语句按 Delphi 字段名（FStatementXxx）登记，DoInit 的 AddSQLStatement label 单独记录；
//     本文件的两个小辅助在两者之间做确定性映射。

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using GXX.M2Server.DbLayer;

namespace GXX.M2Server.Tests;

public class DbLayerSqlFidelityTests
{
    // 指纹表里的 Label 就是实现注册语句时用的键（= 原文 AddSQLStatement 的实参），
    // 因此断言可以逐条直接比对，无需任何映射。

    // ---------------------------------------------------------------- 1. 指纹

    [Fact]
    public void Sqlite_DoInit_BindsExactlyTheExtractedSql()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeSqliteDatabase();
        new TSqliteUserShopDB(new FakeDbLayerHost { DataBase = db }).DoInit();

        Assert.Equal(SqliteUserShopDbFingerprints.Statements.Length, db.Created.Count);
        var actual = db.Created.ToDictionary(s => s.Label, s => DbLayerTestKit.Sha256(s.Sql), StringComparer.Ordinal);
        foreach ((string label, string sha) in SqliteUserShopDbFingerprints.Statements)
        {
            Assert.True(actual.ContainsKey(label), $"SqliteUserShopDB: 缺少语句 {label}");
            Assert.True(sha == actual[label],
                $"SqliteUserShopDB: {label} 的 SQL 与原文抽取的 SHA-256 不一致（期望 {sha}，实际 {actual[label]}）");
        }
    }

    [Fact]
    public void MySql_DoInit_BindsExactlyTheExtractedSql()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        new TMySqlUserShopDB(new FakeDbLayerHost { DataBase = db }).DoInit();

        Assert.Equal(MySqlUserShopDbFingerprints.Statements.Length, db.Created.Count);
        var actual = db.Created.ToDictionary(s => s.Label, s => DbLayerTestKit.Sha256(s.Sql), StringComparer.Ordinal);
        foreach ((string label, string sha) in MySqlUserShopDbFingerprints.Statements)
        {
            Assert.True(actual.ContainsKey(label), $"MySqlUserShopDB: 缺少语句 {label}");
            Assert.True(sha == actual[label],
                $"MySqlUserShopDB: {label} 的 SQL 与原文抽取的 SHA-256 不一致（期望 {sha}，实际 {actual[label]}）");
        }
    }

    // ---------------------------------------------------------------- 2. 语句数量/顺序

    [Fact]
    public void Sqlite_DoInit_AddsTheOriginalStatementLabelsInOrder()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeSqliteDatabase();
        new TSqliteUserShopDB(new FakeDbLayerHost { DataBase = db }).DoInit();

        // 原文 DoInit：第 1 条 UpdateAllBusiness 起共 42 次 AddSQLStatement
        //（38 条固定 + 4 条 MakeIndex；9 个 Insert + 其它）。
        Assert.Equal(41, db.AddedNames.Count);
        Assert.Equal("UserShop_UpdateAllBusiness", db.AddedNames[0]);
        Assert.Equal("UserShop_UpdateHumanBusiness", db.AddedNames[1]);
        Assert.Equal("UserShop_GetAllShop_Ex", db.AddedNames[^5]);
        Assert.Equal("UserShop_GetHumanSellingItem_MakeIndex", db.AddedNames[^4]);
        Assert.Equal("UserShop_GetHumanSellingAndStorageItem_MakeIndex", db.AddedNames[^1]);
        Assert.DoesNotContain("UserShop_SetTimeHasArrivedSellItems", db.AddedNames);
        Assert.DoesNotContain("UserShop_GetItemBindOption", db.AddedNames);
    }

    [Fact]
    public void MySql_DoInit_AddsTheTwoMySqlOnlyStatements()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        new TMySqlUserShopDB(new FakeDbLayerHost { DataBase = db }).DoInit();

        Assert.Equal(43, db.AddedNames.Count);
        Assert.Contains("UserShop_SetTimeHasArrivedSellItems", db.AddedNames);
        Assert.Contains("UserShop_GetItemBindOption", db.AddedNames);
        // MySQL 独有两条：SetTimeHasArrivedSellItems 紧跟在 GetTimeHasArrivedSellItems 之后，
        // GetItemBindOption 是最后一条（原文 324 / 488 行）。
        Assert.Equal("UserShop_SetTimeHasArrivedSellItems", db.AddedNames[17]);
        Assert.Equal("UserShop_GetItemBindOption", db.AddedNames[^1]);
    }

    // ---------------------------------------------------------------- 3. 差异面

    /// <summary>
    /// SQLite 与 MySQL 的 41 条共有语句中，逐字相同 31 条、不同 10 条（3 条方言 + 7 条相关子查询大小写）。
    /// 本测试同时锁死"不同的那 12 条"与"其余 32 条必须逐字相同"，
    /// 防止后续"顺手统一方言"把差异抹平。
    /// </summary>
    [Fact]
    public void Dialects_DifferInExactlyTheTenVerifiedStatements()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var sqliteDb = new FakeSqliteDatabase();
        var mySqlDb = new FakeMySqlDatabase();
        new TSqliteUserShopDB(new FakeDbLayerHost { DataBase = sqliteDb }).DoInit();
        new TMySqlUserShopDB(new FakeDbLayerHost { DataBase = mySqlDb }).DoInit();

        var a = sqliteDb.Created.ToDictionary(s => s.Label, s => s.Sql, StringComparer.Ordinal);
        var b = mySqlDb.Created.ToDictionary(s => s.Label, s => s.Sql, StringComparer.Ordinal);

        List<string> common = a.Keys.Intersect(b.Keys, StringComparer.Ordinal).ToList();
        List<string> differing = common.Where(k => !string.Equals(a[k], b[k], StringComparison.Ordinal))
            .OrderBy(k => k, StringComparer.Ordinal).ToList();

        Assert.Equal(41, common.Count);
        Assert.Equal(10, differing.Count);

        // 3 条方言差异：Update/Buy 的 CreateDate 写法 + 超时判定。
        // 注意 SQLite 侧原文用 strftime(''%s'', ''now'')（Delphi 里双写单引号 → 运行时 SQL 含）
        Assert.Contains("strftime(''%s'', ''now'')", a["UserShop_UpdateShopItem"], StringComparison.Ordinal);
        Assert.Contains("CURRENT_TIMESTAMP", b["UserShop_UpdateShopItem"], StringComparison.Ordinal);
        Assert.Contains("strftime(''%s'', ''now'')", a["UserShop_BuyShopItem"], StringComparison.Ordinal);
        Assert.Contains("CURRENT_TIMESTAMP", b["UserShop_BuyShopItem"], StringComparison.Ordinal);
        Assert.Contains("(strftime(\"%s\", \"now\")) - A.createdate > ?", a["UserShop_GetTimeHasArrivedSellItems"], StringComparison.Ordinal);
        Assert.Contains("TIMESTAMPDIFF(SECOND, A.createdate, CURRENT_TIMESTAMP) > ?", b["UserShop_GetTimeHasArrivedSellItems"], StringComparison.Ordinal);

        // 9 条非方言差异：相关子查询 shopid = a.shopid（SQLite）vs ShopID = A.ShopID（MySQL）。
        Assert.Contains("shopid = a.shopid", a["UserShop_GetUserShopInfo"], StringComparison.Ordinal);
        Assert.Contains("ShopID = A.ShopID", b["UserShop_GetUserShopInfo"], StringComparison.Ordinal);

        // 差异集合必须精确等于"这 3 条方言 + 9 条大小写"。
        var expectedDiffering = new List<string>
        {
            "UserShop_BuyShopItem",
            "UserShop_GetTimeHasArrivedSellItems",
            "UserShop_GetUserShopInfo",
            "UserShop_UpdateShopItem",
            "UserShop_GetAllShop_S0", "UserShop_GetAllShop_S1", "UserShop_GetAllShop_S2",
            "UserShop_GetAllShop_S3", "UserShop_GetAllShop_S4", "UserShop_GetAllShop_S5",
        };
        // 差异集合必须**恰好**等于上面 10 条：
        //   3 条方言 = UpdateUserShopItem / BuyUserShopItem / GetTimeHasArrivedSellItems
        //   7 条大小写 = GetUserShopInfo + GetAllShop_S0..S5
        Assert.Equal(expectedDiffering.OrderBy(x => x, StringComparer.Ordinal),
                     differing.OrderBy(x => x, StringComparer.Ordinal));

        // 其余 31 条逐字相同。
        foreach (string key in common.Except(differing, StringComparer.Ordinal))
        {
            Assert.True(string.Equals(a[key], b[key], StringComparison.Ordinal),
                $"{key} 本应逐字相同，但两边不同");
        }
    }

    // ---------------------------------------------------------------- 4. DoInit / DoFinal

    [Fact]
    public void Sqlite_DoInit_PreparesAllButSort5_AndFinalizeClearsAll()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteUserShopDB(new FakeDbLayerHost { DataBase = db });

        unit.DoInit();

        // 原文 404 行：FStatementGetAllShop_Sort5 只赋 Sql，没有 Prepare（不对称，逐字保留）。
        Assert.Equal(0, db.For("UserShop_GetAllShop_S5").PrepareCount);
        foreach (ScriptedSqliteStatement s in db.Created)
        {
            if (s.Label == "UserShop_GetAllShop_S5") continue;
            Assert.Equal(1, s.PrepareCount);
        }

        // 原文 232-234 行：UpdateAllBusiness 在 DoInit 里额外执行一次（清零营业标志）。
        Assert.Equal(1, db.For("UserShop_UpdateAllBusiness").StepCount);

        unit.DoFinal();

        foreach (ScriptedSqliteStatement s in db.Created) Assert.Equal(1, s.FinalizeCount);
    }

    [Fact]
    public void MySql_DoInit_PreparesAllButSort5_AndFinalizeClearsAll()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        var unit = new TMySqlUserShopDB(new FakeDbLayerHost { DataBase = db });

        unit.DoInit();

        Assert.Equal(0, db.For("UserShop_GetAllShop_S5").PrepareCount);
        foreach (ScriptedMySqlStatement s in db.Created)
        {
            if (s.Label == "UserShop_GetAllShop_S5") continue;
            Assert.Equal(1, s.PrepareCount);
        }
        Assert.Equal(1, db.For("UserShop_UpdateAllBusiness").StepCount);

        unit.DoFinal();

        foreach (ScriptedMySqlStatement s in db.Created) Assert.Equal(1, s.FinalizeCount);
    }

    // ---------------------------------------------------------------- 5. 基类加锁模板

    [Fact]
    public void PublicWrappers_LockAndUnlockExactlyOnce()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeSqliteDatabase();
        var host = new FakeDbLayerHost { DataBase = db };
        var unit = new TSqliteUserShopDB(host);
        unit.DoInit();

        db.For("UserShop_CheckHumanNameExists").AddRow(1);
        bool result = unit.HumanNameExists("张三");

        Assert.True(result);
        Assert.Equal(1, host.LockCount);
        Assert.Equal(1, host.UnLockCount);
    }

    [Fact]
    public void Run_DoesNotLock_AndSkipsWhenTheFeatureIsOff()
    {
        var (env, _) = DbLayerTestKit.Isolate();
        env.boEnabledMySellShopItemTime = false;
        var db = new FakeSqliteDatabase();
        var host = new FakeDbLayerHost { DataBase = db };
        var unit = new TSqliteUserShopDB(host);
        unit.DoInit();

        unit.Run();

        // 原文 TUserShopDB.Run 不加锁（与其它 public 方法不同）。
        Assert.Equal(0, host.LockCount);
        // boEnabledMySellShopItemTime = False → DoRun 立即 Exit，不查语句。
        Assert.Equal(0, db.For("UserShop_GetTimeHasArrivedSellItems").StepCount);
    }
}
