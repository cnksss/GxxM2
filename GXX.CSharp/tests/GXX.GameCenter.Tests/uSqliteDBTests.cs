using System;
using System.Collections.Generic;
using System.IO;
using GXX.GameCenter;
using Xunit;

namespace GXX.GameCenter.Tests;

/// <summary>
/// uSqliteDB.pas（125 行）1:1 移植测试。
/// 用内存 <see cref="ISqliteDatabaseAdapter"/> 注入，**不连真实数据库文件**。
/// </summary>
[Collection("GameCenterSequential")]
public sealed class uSqliteDBTests : GameCenterTestBase
{
    /// <summary>内存适配器：可配置三张表的"存在性"与列名，并记录 Execute 的 SQL。</summary>
    private sealed class MemorySqliteAdapter : ISqliteDatabaseAdapter
    {
        public bool MustExist { get; set; }
        public string Database { get; set; } = "";
        public bool Connected { get; set; }
        public bool Disposed { get; private set; }

        public bool StdItemsExists { get; set; } = true;
        public bool MonsterExists { get; set; } = true;
        public bool MagicExists { get; set; } = true;

        public List<string>? StdItemsColumns { get; set; } = new() { "Id", "Name" };
        public List<string>? MonsterColumns { get; set; } = new() { "Id", "Name" };
        public bool FailStdItemsColumnQuery { get; set; }
        public bool FailMonsterColumnQuery { get; set; }

        public readonly List<string> Executed = new();

        public void Execute(string sql) => Executed.Add(sql);

        public bool StepHasRow(string sql)
        {
            if (sql.Contains("stditems", StringComparison.OrdinalIgnoreCase)) return StdItemsExists;
            if (sql.Contains("monster", StringComparison.OrdinalIgnoreCase)) return MonsterExists;
            if (sql.Contains("magic", StringComparison.OrdinalIgnoreCase)) return MagicExists;
            throw new InvalidOperationException("unexpected query: " + sql);
        }

        public List<string>? GetColumnNames(string sql)
        {
            if (sql.Contains("StdItems", StringComparison.Ordinal))
                return FailStdItemsColumnQuery ? null : StdItemsColumns == null ? null : new List<string>(StdItemsColumns);
            if (sql.Contains("Monster", StringComparison.Ordinal))
                return FailMonsterColumnQuery ? null : MonsterColumns == null ? null : new List<string>(MonsterColumns);
            throw new InvalidOperationException("unexpected query: " + sql);
        }

        public void Dispose() => Disposed = true;
    }

    private MemorySqliteAdapter Last;

    private string MakeFile()
    {
        string f = Under("BmM2.db");
        File.WriteAllBytes(f, new byte[] { 1, 2, 3 });
        return f;
    }

    private void UseAdapter(MemorySqliteAdapter adapter)
    {
        Last = adapter;
        uSqliteDB.AdapterFactory = () => adapter;
    }

    private void ResetAdapterFactory() => uSqliteDB.AdapterFactory = () => new Sqlite3DatabaseAdapter();

    // ---------------- 文件不存在 ----------------

    [Fact]
    public void ProcessSqliteDB_MissingFile_ShowsMessageAndReturnsFalse()
    {
        var adapter = new MemorySqliteAdapter();
        UseAdapter(adapter);

        try
        {
            string missing = Under("no_such.db");
            Assert.False(uSqliteDB.ProcessSqliteDB(missing));
            Assert.Equal("Sqlite数据库: " + missing + " 不存在", GameCenterDialogs.LastMessage);
            // 文件不存在时不会创建适配器/连接
            Assert.Equal("", adapter.Database);
            Assert.False(adapter.Connected);
        }
        finally { ResetAdapterFactory(); }
    }

    // ---------------- 正常路径 ----------------

    [Fact]
    public void ProcessSqliteDB_AllTablesPresent_AddsAllFourStdItemsColumns()
    {
        var adapter = new MemorySqliteAdapter { StdItemsColumns = new List<string> { "Id", "Name" } };
        UseAdapter(adapter);
        try
        {
            Assert.True(uSqliteDB.ProcessSqliteDB(MakeFile()));

            Assert.True(adapter.MustExist);
            Assert.True(adapter.Connected);
            Assert.True(adapter.Disposed);
            Assert.Equal(new[]
            {
                "ALTER TABLE StdItems ADD COLUMN Element21 INTEGER;",
                "ALTER TABLE StdItems ADD COLUMN Element22 INTEGER;",
                "ALTER TABLE StdItems ADD COLUMN Element23 INTEGER;",
                "ALTER TABLE StdItems ADD COLUMN Element24 INTEGER;",
                "ALTER TABLE Monster ADD COLUMN ExploreItem INTEGER default 0;",
                "ALTER TABLE Monster ADD COLUMN DisableSimpleActor INTEGER default 0;",
            }, adapter.Executed);
        }
        finally { ResetAdapterFactory(); }
    }

    [Fact]
    public void ProcessSqliteDB_AllColumnsPresent_ExecutesNoAlter()
    {
        var adapter = new MemorySqliteAdapter
        {
            StdItemsColumns = new List<string> { "Element21", "Element22", "Element23", "Element24" },
            MonsterColumns = new List<string> { "ExploreItem", "DisableSimpleActor" },
        };
        UseAdapter(adapter);
        try
        {
            Assert.True(uSqliteDB.ProcessSqliteDB(MakeFile()));
            Assert.Empty(adapter.Executed);
        }
        finally { ResetAdapterFactory(); }
    }

    [Fact]
    public void ProcessSqliteDB_PartialColumns_OnlyMissingOnesAltered()
    {
        var adapter = new MemorySqliteAdapter
        {
            // Element21/23 已存在，22/24 缺失
            StdItemsColumns = new List<string> { "Element21", "Element23" },
            // DisableSimpleActor 已存在
            MonsterColumns = new List<string> { "DisableSimpleActor" },
        };
        UseAdapter(adapter);
        try
        {
            Assert.True(uSqliteDB.ProcessSqliteDB(MakeFile()));
            Assert.Equal(new[]
            {
                "ALTER TABLE StdItems ADD COLUMN Element22 INTEGER;",
                "ALTER TABLE StdItems ADD COLUMN Element24 INTEGER;",
                "ALTER TABLE Monster ADD COLUMN ExploreItem INTEGER default 0;",
            }, adapter.Executed);
        }
        finally { ResetAdapterFactory(); }
    }

    [Fact]
    public void ProcessSqliteDB_ColumnNameLookupIsCaseSensitiveAfterSort()
    {
        // 原文 ColumnNames.Sorted := True 后用 IndexOf（TStringList 默认区分大小写）：
        // 'element21'（小写）视为不存在 → 依然补列。
        var adapter = new MemorySqliteAdapter
        {
            StdItemsColumns = new List<string> { "element21", "ELEMENT22" },
            MonsterColumns = new List<string> { "exploreitem" },
        };
        UseAdapter(adapter);
        try
        {
            Assert.True(uSqliteDB.ProcessSqliteDB(MakeFile()));
            Assert.Equal(new[]
            {
                "ALTER TABLE StdItems ADD COLUMN Element21 INTEGER;",
                "ALTER TABLE StdItems ADD COLUMN Element22 INTEGER;",
                "ALTER TABLE StdItems ADD COLUMN Element23 INTEGER;",
                "ALTER TABLE StdItems ADD COLUMN Element24 INTEGER;",
                "ALTER TABLE Monster ADD COLUMN ExploreItem INTEGER default 0;",
                "ALTER TABLE Monster ADD COLUMN DisableSimpleActor INTEGER default 0;",
            }, adapter.Executed);
        }
        finally { ResetAdapterFactory(); }
    }

    [Fact]
    public void ProcessSqliteDB_ColumnQueryFails_SkipsAlterButContinues()
    {
        var adapter = new MemorySqliteAdapter
        {
            StdItemsColumns = null,
            MonsterColumns = null,
        };
        UseAdapter(adapter);
        try
        {
            Assert.True(uSqliteDB.ProcessSqliteDB(MakeFile()));
            Assert.Empty(adapter.Executed);
        }
        finally { ResetAdapterFactory(); }
    }

    // ---------------- 表缺失（三处早退） ----------------

    [Fact]
    public void ProcessSqliteDB_StdItemsMissing_ShowsExactMessageAndReturnsFalse()
    {
        var adapter = new MemorySqliteAdapter { StdItemsExists = false };
        UseAdapter(adapter);
        try
        {
            Assert.False(uSqliteDB.ProcessSqliteDB(MakeFile()));
            Assert.Equal("数据库中的表: StdItems 不存在", GameCenterDialogs.LastMessage);
            Assert.Empty(adapter.Executed);
        }
        finally { ResetAdapterFactory(); }
    }

    [Fact]
    public void ProcessSqliteDB_MonsterMissing_ShowsExactMessageAndReturnsFalse()
    {
        var adapter = new MemorySqliteAdapter { MonsterExists = false };
        UseAdapter(adapter);
        try
        {
            Assert.False(uSqliteDB.ProcessSqliteDB(MakeFile()));
            Assert.Equal("数据库中的表: Monster 不存在", GameCenterDialogs.LastMessage);
            // StdItems 补列已先执行（原文顺序：StdItems 建列 → 检查 Monster）
            Assert.Equal(4, adapter.Executed.Count);
        }
        finally { ResetAdapterFactory(); }
    }

    [Fact]
    public void ProcessSqliteDB_MagicMissing_ShowsExactMessageAndReturnsFalse()
    {
        var adapter = new MemorySqliteAdapter { MagicExists = false };
        UseAdapter(adapter);
        try
        {
            Assert.False(uSqliteDB.ProcessSqliteDB(MakeFile()));
            Assert.Equal("数据库中的表: Magic 不存在", GameCenterDialogs.LastMessage);
            Assert.Equal(6, adapter.Executed.Count);
        }
        finally { ResetAdapterFactory(); }
    }

    [Fact]
    public void ProcessSqliteDB_MonsterMissing_DoesNotRunMonsterAlters()
    {
        var adapter = new MemorySqliteAdapter { MonsterExists = false };
        UseAdapter(adapter);
        try
        {
            uSqliteDB.ProcessSqliteDB(MakeFile());
            Assert.DoesNotContain(adapter.Executed, s => s.Contains("Monster"));
        }
        finally { ResetAdapterFactory(); }
    }

    // ---------------- 常量与查询文本 ----------------

    [Fact]
    public void SqliteConstants_MatchSqliteCliValues()
    {
        Assert.Equal(100, uSqliteDB.SQLITE_ROW);
        Assert.Equal(101, uSqliteDB.SQLITE_DONE);
    }

    [Fact]
    public void DefaultAdapterFactory_ReturnsMicrosoftSqliteAdapter()
    {
        ResetAdapterFactory();
        using var a = uSqliteDB.AdapterFactory();
        Assert.IsType<Sqlite3DatabaseAdapter>(a);
    }

    [Fact]
    public void Sqlite3DatabaseAdapter_MustExistOnMissingFile_ThrowsFileNotFound()
    {
        using var a = new Sqlite3DatabaseAdapter();
        a.MustExist = true;
        a.Database = Under("ghost.db");
        Assert.Throws<FileNotFoundException>(() => a.Connected = true);
    }

    [Fact]
    public void Sqlite3DatabaseAdapter_RealSqlite_AlterRoundTrip()
    {
        // 用真实 Microsoft.Data.Sqlite 跑一遍最小流程（临时文件，非"真实业务库"）。
        string db = Under("t.db");
        using (var a = new Sqlite3DatabaseAdapter())
        {
            a.MustExist = false;
            a.Database = db;
            a.Connected = true;
            a.Execute("CREATE TABLE StdItems (Id INTEGER);");
            Assert.True(a.StepHasRow("select 1 from sqlite_master where type = \"table\" and lower(name) = \"stditems\";"));
            var cols = a.GetColumnNames("select * from StdItems LIMIT 0;");
            Assert.NotNull(cols);
            Assert.Contains("Id", cols!);
            a.Execute("ALTER TABLE StdItems ADD COLUMN Element21 INTEGER;");
            cols = a.GetColumnNames("select * from StdItems LIMIT 0;");
            Assert.Contains("Element21", cols!);
        }
        using (var b = new Sqlite3DatabaseAdapter())
        {
            b.MustExist = true;
            b.Database = db;
            b.Connected = true;
            // 不存在的表 → GetColumnNames 返回 null（对应原文 Prepare 失败分支）
            Assert.Null(b.GetColumnNames("select * from NoSuchTable LIMIT 0;"));
        }
    }
}
