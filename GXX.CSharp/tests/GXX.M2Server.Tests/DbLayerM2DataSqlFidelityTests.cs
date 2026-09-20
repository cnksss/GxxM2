// 源单元（文件头证据）：
//   Source/M2Engine/SqliteM2DataDB.pas
//   Source/M2Engine/MySqlM2DataDB.pas
//   Source/M2Engine/SqliteCreateTableSql.pas（只用到 SQLITE_CREATE_M2DATA_TABLES / SQLITE_M2DBVERSION）
//
// 测试组 1：SQL 文本保真（**逐字 + 指纹 + 回读比对**）
//   1. 实现真正绑到 FDB 的 20 条语句，其 SHA-256 必须等于从 GBK 原文机械抽取的期望值
//      （期望值见 DbLayerFingerprints.SqliteM2DataDB.cs / .MySqlM2DataDB.cs，
//       由 _recon/p3-fingerprints.mjs 直接从 p3-<unit>.json 生成，**不经过 C# 常量生成步骤**，
//       因此指纹与实现是两条独立路径 —— 这就是"脚本抽取 + 回读比对"）。
//   2. AddSQLStatement 的数量与 label 顺序必须与原文 DoInit 一致（20 条）。
//   3. 两个方言的差异面必须**恰好**等于"已核实的 2 条"（`HintModule` vs `Hintmodule`，
//      原文大小写笔误），其余 18 条逐字相同 —— 防止"顺手统一方言"。
//   4. 方法级迁移脚本必须等于原文的 `前缀 + IntToStr(g_Config.nAuctionCurrencyType) + 后缀`
//      （回读：把实现真正 Execute 出去的字符串与抽取出的三段常量拼接结果逐字比对）。
//
// 测试组 2：DoInit / DoUpdate / DoFinal 生命周期与版本分支（逐字锁定原文的分支顺序与调用集合）。
//
// **测试绝不连真实数据库**：全部 DB 访问注入 DbLayerTestKit 的内存实现；
// 文件系统（ExtractFilePath/DirectoryExists/FileExists/SaveResourceToFile）也全部替换为内存接缝。

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using GXX.M2Server.DbLayer;

namespace GXX.M2Server.Tests;

public class DbLayerM2DataSqlFidelityTests
{
    /// <summary>把 SQLite 侧文件系统接缝替换为"目录存在 / 库文件是否已存在由参数决定"。</summary>
    private static void IsolateSqliteFiles(bool fileExists)
    {
        TSqliteM2DataDB.ExtractFilePath = () => @"C:\M2Test\";
        TSqliteM2DataDB.DirectoryExists = _ => true;
        TSqliteM2DataDB.ForceDirectories = _ => { };
        TSqliteM2DataDB.FileExists = _ => fileExists;
        TSqliteM2DataDB.DeleteFile = _ => { };
        TSqliteM2DataDB.SaveResourceToFile = (_, _) => { };
        TSqliteM2DataDB.Sqlite3ConfigMultithread = () => { };
    }

    /// <summary>原文 DoInit 的 20 条 AddSQLStatement label，按原文出现顺序（两个方言完全相同）。</summary>
    private static readonly string[] OriginalStatementOrder =
    {
        "InsertItems",
        "InsertItemValueAdd",
        "InsertItemElementAdd",
        "InsertItemAddDataByte",
        "InsertItemAddDataInt",
        "InsertItemAddDataText",
        "InsertItemFlute",
        "InsertItemProgress",
        "InsertItemProperty",
        "SelectItems",
        "SelectItems_Sort",
        "SelectItem",
        "SelectitemValueAdd",
        "SelectitemElementAdd",
        "SelectitemAddDataByte",
        "SelectitemAddDataInt",
        "SelectitemAddDataText",
        "SelectitemFlute",
        "SelectItemProgress",
        "SelectItemProperty",
    };

    // ---------------------------------------------------------------- 1. 指纹（脚本抽取 → 回读比对）

    [Fact]
    public void Sqlite_DoInit_BindsExactlyTheExtractedSql()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles(fileExists: false);
        var db = new FakeSqliteDatabase();
        new TSqliteM2DataDB(new FakeDbLayerHost { DataBase = db }, db).DoInit();

        Assert.Equal(SqliteM2DataDbFingerprints.Statements.Length, db.Created.Count);
        var actual = db.Created.ToDictionary(s => s.Label, s => DbLayerTestKit.Sha256(s.Sql), StringComparer.Ordinal);
        foreach ((string label, string sha) in SqliteM2DataDbFingerprints.Statements)
        {
            Assert.True(actual.ContainsKey(label), $"SqliteM2DataDB: 缺少语句 {label}");
            Assert.True(sha == actual[label],
                $"SqliteM2DataDB: {label} 的 SQL 与原文抽取的 SHA-256 不一致（期望 {sha}，实际 {actual[label]}）");
        }
    }

    [Fact]
    public void MySql_DoInit_BindsExactlyTheExtractedSql()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        new TMySqlM2DataDB(new FakeDbLayerHost { DataBase = db }, db).DoInit();

        Assert.Equal(MySqlM2DataDbFingerprints.Statements.Length, db.Created.Count);
        var actual = db.Created.ToDictionary(s => s.Label, s => DbLayerTestKit.Sha256(s.Sql), StringComparer.Ordinal);
        foreach ((string label, string sha) in MySqlM2DataDbFingerprints.Statements)
        {
            Assert.True(actual.ContainsKey(label), $"MySqlM2DataDB: 缺少语句 {label}");
            Assert.True(sha == actual[label],
                $"MySqlM2DataDB: {label} 的 SQL 与原文抽取的 SHA-256 不一致（期望 {sha}，实际 {actual[label]}）");
        }
    }

    // ---------------------------------------------------------------- 2. 语句数量与顺序

    [Fact]
    public void Sqlite_DoInit_AddsTheOriginalStatementLabelsInOrder()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles(fileExists: false);
        var db = new FakeSqliteDatabase();
        new TSqliteM2DataDB(new FakeDbLayerHost { DataBase = db }, db).DoInit();

        Assert.Equal(OriginalStatementOrder, db.AddedNames);
        Assert.Equal(20, db.AddedNames.Count);
    }

    [Fact]
    public void MySql_DoInit_AddsTheOriginalStatementLabelsInOrder()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        new TMySqlM2DataDB(new FakeDbLayerHost { DataBase = db }, db).DoInit();

        Assert.Equal(OriginalStatementOrder, db.AddedNames);
        Assert.Equal(20, db.AddedNames.Count);
    }

    // ---------------------------------------------------------------- 3. 方言差异面

    /// <summary>
    /// 两个方言的 20 条语句中，逐字相同 18 条、不同 2 条。
    /// 差异是**原文的大小写笔误**：SQLite 写 <c>HintModule</c>，MySQL 写 <c>Hintmodule</c>。
    /// 本测试同时锁死"不同的那 2 条"与"其余 18 条必须逐字相同"。
    /// </summary>
    [Fact]
    public void Dialects_DifferInExactlyTheTwoHintModuleStatements()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles(fileExists: false);
        var sqliteDb = new FakeSqliteDatabase();
        var mySqlDb = new FakeMySqlDatabase();
        new TSqliteM2DataDB(new FakeDbLayerHost { DataBase = sqliteDb }, sqliteDb).DoInit();
        new TMySqlM2DataDB(new FakeDbLayerHost { DataBase = mySqlDb }, mySqlDb).DoInit();

        var a = sqliteDb.Created.ToDictionary(s => s.Label, s => s.Sql, StringComparer.Ordinal);
        var b = mySqlDb.Created.ToDictionary(s => s.Label, s => s.Sql, StringComparer.Ordinal);

        List<string> common = a.Keys.Intersect(b.Keys, StringComparer.Ordinal).ToList();
        List<string> differing = common.Where(k => !string.Equals(a[k], b[k], StringComparison.Ordinal))
            .OrderBy(k => k, StringComparer.Ordinal).ToList();

        Assert.Equal(20, common.Count);
        Assert.Equal(new[] { "InsertItemProperty", "SelectItemProperty" },
            differing.OrderBy(x => x, StringComparer.Ordinal));

        // 原文如此（SqliteM2DataDB.pas:268 / MySqlM2DataDB.pas:418）：两处大小写不一致。
        Assert.Contains("HintModule", a["InsertItemProperty"], StringComparison.Ordinal);
        Assert.DoesNotContain("HintModule", b["InsertItemProperty"], StringComparison.Ordinal);
        Assert.Contains("Hintmodule", b["InsertItemProperty"], StringComparison.Ordinal);
        Assert.Contains("HintModule", a["SelectItemProperty"], StringComparison.Ordinal);
        Assert.Contains("Hintmodule", b["SelectItemProperty"], StringComparison.Ordinal);

        // 其余 18 条逐字相同。
        foreach (string key in common.Except(differing, StringComparer.Ordinal))
        {
            Assert.True(string.Equals(a[key], b[key], StringComparison.Ordinal),
                $"{key} 本应逐字相同，但两边不同");
        }
    }

    // ---------------------------------------------------------------- 4. DoInit 两条路径

    /// <summary>
    /// 原文 SqliteM2DataDB.pas:156-206：库文件**已存在** → 不建表，只做 PRAGMA + DoUpdate。
    /// 4 条 PRAGMA 的文本与顺序逐字锁定（194/199 两条被原文注释掉，不得出现）。
    /// </summary>
    [Fact]
    public void Sqlite_DoInit_ExistingFile_RunsTheFourPragmas_ThenDoUpdate()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles(fileExists: true);
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteM2DataDB(new FakeDbLayerHost { DataBase = db }, db);

        unit.DoInit();

        Assert.Equal(new[]
        {
            "PRAGMA cache_size = 32768;",
            "PRAGMA mmap_size = 32768",
            "PRAGMA locking_mode = EXCLUSIVE;",
            "PRAGMA journal_mode = WAL;",
        }, db.ExecutedSql.Take(4));

        // 原文 194/199 两条被注释掉：不得出现。
        Assert.DoesNotContain("PRAGMA synchronous = NORMAL;", db.ExecutedSql);
        Assert.DoesNotContain("PRAGMA temp_store = MEMORY;", db.ExecutedSql);

        Assert.True(db.MustExist);
        Assert.True(db.UseThreadMode);
        Assert.Equal(@"C:\M2Test\M2Data.DB", db.Database);
        Assert.Equal(1, db.For("get_db_constant_value").PrepareCount);
        Assert.False(unit.IsInitOK);   // ★ 原文 DoInit 不设 IsInitOK（由 TM2DataDB.Init 设置）
        unit.Init();
        Assert.True(unit.IsInitOK);
    }

    /// <summary>
    /// 原文 SqliteM2DataDB.pas:205-221：库文件**不存在** → 从资源释放 M2Data.DB，
    /// 在一个事务里 Execute(SQLITE_CREATE_M2DATA_TABLES) 然后 Commit；**不调用 DoUpdate**。
    /// </summary>
    [Fact]
    public void Sqlite_DoInit_MissingFile_CreatesTablesFromTheExtractedDdl_AndSkipsDoUpdate()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles(fileExists: false);
        var db = new FakeSqliteDatabase();
        var released = new List<string>();
        TSqliteM2DataDB.SaveResourceToFile = (res, file) => released.Add(res + "|" + file);

        var unit = new TSqliteM2DataDB(new FakeDbLayerHost { DataBase = db }, db);
        unit.DoInit();

        // M2Data 资源被释放到 <dir>M2Data.DB。
        Assert.Equal(new[] { @"M2Data|C:\M2Test\M2Data.DB" }, released);

        Assert.False(db.MustExist);
        Assert.Equal(new[] { "begin", "commit" }, db.Transactions);
        Assert.Single(db.ExecutedSql);
        // 回读：实现建表用的 DDL 必须逐字等于从 SqliteCreateTableSql.pas:973-1227 抽取的常量。
        Assert.Equal(SqliteCreateTableSql.SQLITE_CREATE_M2DATA_TABLES, db.ExecutedSql[0]);
        Assert.DoesNotContain("get_db_constant_value", db.AddedNames);

        Assert.False(unit.IsInitOK);   // ★ 原文 DoInit 不设 IsInitOK
        Assert.Equal(20, db.Created.Count);
    }

    /// <summary>建表 DDL 里的版本号必须等于原文 SQLITE_M2DBVERSION（SqliteCreateTableSql.pas:970）。</summary>
    [Fact]
    public void SqliteCreateTableSql_CarriesTheOriginalVersionAndSourceLineRange()
    {
        Assert.Equal("20200916", TSqliteM2DataDB.SQLITE_M2DBVERSION);
        Assert.Equal("20200916", SqliteCreateTableSql.SQLITE_M2DBVERSION);
        // 原文 20200916 是 SQLITE_M2DBVERSION；SQLITE_DBVERSION 是另一个（20220219），两者不可混用。
        Assert.Equal("20220219", SqliteCreateTableSql.SQLITE_DBVERSION);
        // DDL 必须覆盖 M2Data 家族的 8 张表 + Items。
        foreach (string table in new[]
                 {
                     "AuctionAttention", "AuctionData", "Items", "ItemAddDataByte", "ItemAddDataInt",
                     "ItemAddDataText", "ItemFlute", "ItemProgress", "ItemProperty", "ItemValueAdd",
                     "StorageEx", "UserShop", "UserShopItem", "db_constant",
                 })
        {
            Assert.Contains("\"" + table + "\"", SqliteCreateTableSql.SQLITE_CREATE_M2DATA_TABLES, StringComparison.Ordinal);
        }
    }

    // ---------------------------------------------------------------- 5. 迁移脚本回读比对

    /// <summary>原文 DoUpdate 的 6 段动态迁移脚本 = 抽取出的 {前缀, 后缀} + IntToStr(nAuctionCurrencyType)。</summary>
    public static IEnumerable<object[]> DynamicMigrationScripts => new[]
    {
        new object[] { 20170506, SqliteM2DataDbScripts.DoUpdate_L507_S_P0, SqliteM2DataDbScripts.DoUpdate_L507_S_P2 },
        new object[] { 20170603, SqliteM2DataDbScripts.DoUpdate_L569_S_P0, SqliteM2DataDbScripts.DoUpdate_L569_S_P2 },
        new object[] { 20170610, SqliteM2DataDbScripts.DoUpdate_L630_S_P0, SqliteM2DataDbScripts.DoUpdate_L630_S_P2 },
        new object[] { 20170701, SqliteM2DataDbScripts.DoUpdate_L674_S_P0, SqliteM2DataDbScripts.DoUpdate_L674_S_P2 },
        new object[] { 20180512, SqliteM2DataDbScripts.DoUpdate_L719_S_P0, SqliteM2DataDbScripts.DoUpdate_L719_S_P2 },
    };

    [Theory]
    [MemberData(nameof(DynamicMigrationScripts))]
    public void Sqlite_DoUpdate_AssemblesTheMigrationScriptVerbatim(int dbVersion, string prefix, string suffix)
    {
        var (env, _) = DbLayerTestKit.Isolate();
        env.nAuctionCurrencyType = 3;
        IsolateSqliteFiles(fileExists: true);
        var db = new FakeSqliteDatabase();
        // DoUpdate 里才 AddSQLStatement("get_db_constant_value")；先预登记同名语句并塞版本行
        // （SQLite3DataBase 的 AddSQLStatement 同名复用返回同一实例）。
        ((ScriptedSqliteStatement)db.AddSQLStatement("get_db_constant_value")).AddRow(dbVersion);

        new TSqliteM2DataDB(new FakeDbLayerHost { DataBase = db }, db).DoInit();

        // ExecutedSql[0..3] 是 4 条 PRAGMA（原文 194-201）；迁移脚本是第 5 条。
        // 回读：它就是"前缀 + IntToStr(g_Config.nAuctionCurrencyType) + 后缀"。
        string expected = prefix + "3" + suffix;
        Assert.Equal(expected, db.ExecutedSql[4]);
    }

    /// <summary>原文 761-768 的 20180613 迁移是**静态**脚本（没有运行时拼接）。</summary>
    [Fact]
    public void Sqlite_DoUpdate_20180613_UsesTheStaticScript()
    {
        var (env, _) = DbLayerTestKit.Isolate();
        env.nAuctionCurrencyType = 9;
        IsolateSqliteFiles(fileExists: true);
        var db = new FakeSqliteDatabase();
        ((ScriptedSqliteStatement)db.AddSQLStatement("get_db_constant_value")).AddRow(20180613);

        new TSqliteM2DataDB(new FakeDbLayerHost { DataBase = db }, db).DoInit();

        Assert.Equal(SqliteM2DataDbScripts.DoUpdate_L761_S, db.ExecutedSql[4]);
        Assert.DoesNotContain("CurrencyType = 9", db.ExecutedSql[4], StringComparison.Ordinal);
    }

    // ---------------------------------------------------------------- 6. 版本分支的调用集合

    /// <summary>
    /// 原文 DoUpdate 的 if/else-if 链（503-869）。每行 = (dbVersion, 期望的 UpdateDB_N 调用数)。
    /// 计数 = 4 条 PRAGMA（原文 194-201）+ 迁移脚本（若有）+ 每个 UpdateDB_N 的 1 次 <c>_fdb.Execute</c>。
    /// </summary>
    public static IEnumerable<object[]> VersionBranches => new[]
    {
        new object[] { 20170506, 9 },   // 4 PRAGMA + 迁移 + UpdateDB_1/3/5/6
        new object[] { 20170603, 10 },  // 4 PRAGMA + 迁移 + 1/3/4/5/6
        new object[] { 20170610, 10 },  // 4 PRAGMA + 迁移 + 1/3/4/5/6
        new object[] { 20170701, 11 },  // 4 PRAGMA + 迁移 + 1/2/3/4/5/6
        new object[] { 20180512, 11 },  // 4 PRAGMA + 迁移 + 1/2/3/4/5/6
        new object[] { 20180613, 11 },  // 4 PRAGMA + 迁移 + 1/2/3/4/5/6
        new object[] { 20180615, 10 },  // 4 PRAGMA + 无迁移脚本 + 1/2/3/4/5/6
        new object[] { 20190318, 9 },   // 4 PRAGMA + 无迁移脚本 + 2/3/4/5/6（dbVersion <= 20190318）
        new object[] { 20190606, 7 },   // 4 PRAGMA + 4/5/6
        new object[] { 20190928, 6 },   // 4 PRAGMA + 5/6
        new object[] { 20200813, 5 },   // 4 PRAGMA + 6
    };

    [Theory]
    [MemberData(nameof(VersionBranches))]
    public void Sqlite_DoUpdate_RunsTheOriginalUpdateDbSetForTheVersion(int dbVersion, int expectedExecuteCount)
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles(fileExists: true);
        var db = new FakeSqliteDatabase();
        // DoUpdate 里才 AddSQLStatement("get_db_constant_value")；先预登记同名语句并塞版本行
        // （SQLite3DataBase 的 AddSQLStatement 同名复用返回同一实例）。
        ((ScriptedSqliteStatement)db.AddSQLStatement("get_db_constant_value")).AddRow(dbVersion);

        new TSqliteM2DataDB(new FakeDbLayerHost { DataBase = db }, db).DoInit();

        Assert.Equal(expectedExecuteCount, db.ExecutedSql.Count);
        // 版本分支 1 对 begin/commit + 871-961 的两段回填各 1 对 = 3 对。
        Assert.Equal(3, db.Transactions.Count(x => x == "begin"));
        Assert.Equal(3, db.Transactions.Count(x => x == "commit"));
    }

    /// <summary>原文 867 行：版本号高于所有已知版本时（&gt; 20200813）**什么迁移都不做**，只有回填。</summary>
    [Fact]
    public void Sqlite_DoUpdate_UnknownNewerVersion_RunsNoMigration()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles(fileExists: true);
        var db = new FakeSqliteDatabase();
        ((ScriptedSqliteStatement)db.AddSQLStatement("get_db_constant_value")).AddRow(20990101);

        new TSqliteM2DataDB(new FakeDbLayerHost { DataBase = db }, db).DoInit();

        // 只有 4 条 PRAGMA，没有任何迁移脚本。
        Assert.Equal(4, db.ExecutedSql.Count);
        // 未知版本不迁移，但 871-961 的两段回填仍然各开一个事务。
        Assert.Equal(2, db.Transactions.Count(x => x == "begin"));
    }

    // ---------------------------------------------------------------- 7. DoFinal 的原文缺陷

    /// <summary>
    /// ★ 原文缺陷（SqliteM2DataDB.pas:963-1073）：DoFinal **不 Finalize**
    /// <c>FStatementGetItems</c> / <c>FStatementGetItems_Sort</c>（其余 18 条都 Finalize）。
    /// 本测试锁定该不对称，防止"顺手修好"。
    /// </summary>
    [Fact]
    public void Sqlite_DoFinal_DoesNotFinalizeTheTwoGetItemsStatements()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles(fileExists: false);
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteM2DataDB(new FakeDbLayerHost { DataBase = db }, db);
        unit.DoInit();

        unit.DoFinal();

        Assert.Equal(0, db.For("SelectItems").FinalizeCount);
        Assert.Equal(0, db.For("SelectItems_Sort").FinalizeCount);
        foreach (ScriptedSqliteStatement s in db.Created)
        {
            if (s.Label is "SelectItems" or "SelectItems_Sort") continue;
            Assert.Equal(1, s.FinalizeCount);
        }

        // 二次 DoFinal 幂等（字段已置 nil）。
        unit.DoFinal();
        foreach (ScriptedSqliteStatement s in db.Created)
        {
            if (s.Label is "SelectItems" or "SelectItems_Sort") continue;
            Assert.Equal(1, s.FinalizeCount);
        }
    }

    /// <summary>★ 同一缺陷在 MySQL 姊妹实现里同样存在（MySqlM2DataDB.pas:628-739）。</summary>
    [Fact]
    public void MySql_DoFinal_DoesNotFinalizeTheTwoGetItemsStatements()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        var unit = new TMySqlM2DataDB(new FakeDbLayerHost { DataBase = db }, db);
        unit.DoInit();

        unit.DoFinal();

        Assert.Equal(0, db.For("SelectItems").FinalizeCount);
        Assert.Equal(0, db.For("SelectItems_Sort").FinalizeCount);
        foreach (ScriptedMySqlStatement s in db.Created)
        {
            if (s.Label is "SelectItems" or "SelectItems_Sort") continue;
            Assert.Equal(1, s.FinalizeCount);
        }
    }

    // ---------------------------------------------------------------- 8. DoInit 的 Prepare 对称性

    /// <summary>20 条语句在 DoInit 里全部 Prepare 恰好一次（与 UserShop 的 Sort5 不对称不同）。</summary>
    [Fact]
    public void Sqlite_DoInit_PreparesEveryStatementExactlyOnce()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles(fileExists: false);
        var db = new FakeSqliteDatabase();
        new TSqliteM2DataDB(new FakeDbLayerHost { DataBase = db }, db).DoInit();

        foreach (ScriptedSqliteStatement s in db.Created) Assert.Equal(1, s.PrepareCount);
    }

    /// <summary>
    /// 原文 DoUpdate 871-957 的 `temp` 语句：同一条 SQL 上先查 Items(itemtype=1) 回填 UserShopItem，
    /// 再查 Items(itemtype=5) 回填 AuctionData；SQLite3DataBase 的 AddSQLStatement **同名复用**同一实例。
    /// </summary>
    [Fact]
    public void Sqlite_DoUpdate_TempStatementIsReusedByName_AndRunsTwoBackfills()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        IsolateSqliteFiles(fileExists: true);
        var db = new FakeSqliteDatabase();
        ((ScriptedSqliteStatement)db.AddSQLStatement("get_db_constant_value")).AddRow(20990101);

        new TSqliteM2DataDB(new FakeDbLayerHost { DataBase = db }, db).DoInit();

        // AddSQLStatement("temp") 出现两次 → AddedNames 两次，但 Created 只一条。
        Assert.Equal(2, db.AddedNames.Count(n => n == "temp"));
        Assert.Single(db.Created, s => s.Label == "temp");
        // 第二条 temp SQL 覆盖第一条（同一个语句实例）。
        Assert.Equal(SqliteM2DataDbScripts.DoUpdate_L934_sm_Sql, db.For("temp").Sql);
        // 两次 Prepare（873-888 与 917-932 各一次），Finalize 两次。
        Assert.Equal(2, db.For("temp").PrepareCount);
        Assert.Equal(2, db.For("temp").FinalizeCount);
    }
}
