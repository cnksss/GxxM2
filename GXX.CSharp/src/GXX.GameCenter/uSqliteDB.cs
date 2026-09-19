using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace GXX.GameCenter;

/// <summary>
/// uSqliteDB.pas:1-8 <c>function ProcessSqliteDB(FileName: string): Boolean;</c> 的外部依赖接缝。
/// 原文使用 SQLite3DataBase/SQLiteCli（TSQLite3Database/TSQLStatement）；
/// 托管侧以本接口抽象出原文实际用到的 5 个能力，默认实现为
/// <see cref="Sqlite3DatabaseAdapter"/>（Microsoft.Data.Sqlite，GXX.GameCenter.csproj 既有 PackageReference）。
/// 单测注入内存实现，**不连真实数据库文件**。
/// </summary>
public interface ISqliteDatabaseAdapter : IDisposable
{
    /// <summary><c>TSQLite3Database.MustExist := True</c>（库文件不存在即失败）。</summary>
    bool MustExist { get; set; }

    /// <summary><c>TSQLite3Database.Database := FileName</c>。</summary>
    string Database { get; set; }

    /// <summary><c>TSQLite3Database.Connected := True</c>。</summary>
    bool Connected { get; set; }

    /// <summary><c>DB.Execute(sql)</c>（不取结果集）。</summary>
    void Execute(string sql);

    /// <summary>
    /// 等价于原文 <c>sm.Prepare; nRet := sm.Step; sm.Reset;</c>：
    /// 返回 <c>true</c> 表示 Step 得到 <c>SQLITE_ROW</c>，<c>false</c> 表示 <c>SQLITE_DONE</c>。
    /// 原文只与 <c>SQLITE_ROW</c> 比较，故布尔等效（见 <see cref="ProcessSqliteDB"/> 的差异说明）。
    /// </summary>
    bool StepHasRow(string sql);

    /// <summary>
    /// 等价于原文 <c>Prepare; Reset; Step = SQLITE_DONE ? GetColumnCount/GetColumnName : 跳过</c>：
    /// 语句可成功准备时返回列名列表，准备失败时返回 <c>null</c>（原文该分支不执行）。
    /// </summary>
    List<string>? GetColumnNames(string sql);
}

/// <summary>
/// <see cref="ISqliteDatabaseAdapter"/> 默认实现（Microsoft.Data.Sqlite）。
/// 说明：原文 <c>SQLite3DataBase</c> 在 <c>MustExist=True</c> 且文件不存在时抛异常（等价于"库不存在"）；
/// 本实现仅支持真实文件路径（内存库用 <c>Data Source=:memory:</c>），单测请改用内存实现注入。
/// </summary>
public sealed class Sqlite3DatabaseAdapter : ISqliteDatabaseAdapter
{
    private SqliteConnection? _connection;
    private string _database = "";
    private bool _mustExist;

    public bool MustExist { get => _mustExist; set => _mustExist = value; }

    public string Database
    {
        get => _database;
        set
        {
            if (_connection != null) { _connection.Dispose(); _connection = null; }
            _database = value ?? "";
        }
    }

    public bool Connected
    {
        get => _connection != null && _connection.State == System.Data.ConnectionState.Open;
        set
        {
            if (!value)
            {
                _connection?.Dispose();
                _connection = null;
                return;
            }
            if (_mustExist && !File.Exists(_database))
                throw new FileNotFoundException("Sqlite 数据库不存在", _database);
            _connection = new SqliteConnection("Data Source=" + _database);
            _connection.Open();
        }
    }

    public void Execute(string sql)
    {
        using var cmd = _connection!.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    public bool StepHasRow(string sql)
    {
        using var cmd = _connection!.CreateCommand();
        cmd.CommandText = sql;
        using var reader = cmd.ExecuteReader();
        return reader.Read();
    }

    public List<string>? GetColumnNames(string sql)
    {
        try
        {
            using var cmd = _connection!.CreateCommand();
            cmd.CommandText = sql;
            using var reader = cmd.ExecuteReader();
            var names = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
                names.Add(reader.GetName(i));
            return names;
        }
        catch (SqliteException)
        {
            // 原文 Prepare 失败时该分支不执行（等价于返回 null）。
            return null;
        }
    }

    public void Dispose()
    {
        _connection?.Dispose();
        _connection = null;
    }
}

/// <summary>
/// uSqliteDB.pas 全文 1:1 移植（125 行）。
/// <c>ProcessSqliteDB</c> 校验 BmM2.db 的三张主表（StdItems/Monster/Magic）是否存在，
/// 并对 StdItems 补 Element21..Element24、对 Monster 补 ExploreItem/DisableSimpleActor 列。
/// </summary>
public static class uSqliteDB
{
    /// <summary>原文 SQLiteCli 常量：<c>SQLITE_ROW = 100</c>。</summary>
    public const int SQLITE_ROW = 100;

    /// <summary>原文 SQLiteCli 常量：<c>SQLITE_DONE = 101</c>。</summary>
    public const int SQLITE_DONE = 101;

    /// <summary>适配器工厂（单测注入内存实现；默认创建真实 Microsoft.Data.Sqlite 适配器）。</summary>
    public static Func<ISqliteDatabaseAdapter> AdapterFactory = () => new Sqlite3DatabaseAdapter();

    /// <summary>
    /// uSqliteDB.pas:12 <c>function ProcessSqliteDB(FileName: string): Boolean;</c>
    /// 原文校验顺序：文件存在 → 连接 → StdItems 表存在 → StdItems 补列 → Monster 表存在 → Monster 补列 → Magic 表存在。
    /// </summary>
    public static bool ProcessSqliteDB(string FileName)
    {
        bool Result = false;

        if (!File.Exists(FileName))
        {
            GameCenterDialogs.ShowMessage("Sqlite数据库: " + FileName + " 不存在");
            return Result;
        }

        ISqliteDatabaseAdapter DB = AdapterFactory();
        try
        {
            DB.MustExist = true;
            DB.Database = FileName;
            DB.Connected = true;

            bool nRet = DB.StepHasRow("select 1 from sqlite_master where type = \"table\" and lower(name) = \"stditems\";");

            if (!nRet)
            {
                GameCenterDialogs.ShowMessage("数据库中的表: StdItems 不存在");
                return Result;
            }

            List<string>? ColumnNames = DB.GetColumnNames("select * from StdItems LIMIT 0;");
            if (ColumnNames != null)
            {
                var cols = new List<string>(ColumnNames);

                cols.Sort(StringComparer.Ordinal);

                // 扩展三个字段 chongchong 2017-04-16
                if (cols.IndexOf("Element21") < 0)
                {
                    DB.Execute("ALTER TABLE StdItems ADD COLUMN Element21 INTEGER;");
                }

                if (cols.IndexOf("Element22") < 0)
                {
                    DB.Execute("ALTER TABLE StdItems ADD COLUMN Element22 INTEGER;");
                }

                if (cols.IndexOf("Element23") < 0)
                {
                    DB.Execute("ALTER TABLE StdItems ADD COLUMN Element23 INTEGER;");
                }

                if (cols.IndexOf("Element24") < 0)
                {
                    DB.Execute("ALTER TABLE StdItems ADD COLUMN Element24 INTEGER;");
                }
            }

            nRet = DB.StepHasRow("select 1 from sqlite_master where type = \"table\" and lower(name) = \"monster\";");

            if (!nRet)
            {
                GameCenterDialogs.ShowMessage("数据库中的表: Monster 不存在");
                return Result;
            }

            ColumnNames = DB.GetColumnNames("select * from Monster LIMIT 0;");
            if (ColumnNames != null)
            {
                var cols = new List<string>(ColumnNames);

                cols.Sort(StringComparer.Ordinal);

                // 扩展三个字段 chongchong 2017-04-16
                if (cols.IndexOf("ExploreItem") < 0)
                {
                    DB.Execute("ALTER TABLE Monster ADD COLUMN ExploreItem INTEGER default 0;");
                }

                if (cols.IndexOf("DisableSimpleActor") < 0)
                {
                    DB.Execute("ALTER TABLE Monster ADD COLUMN DisableSimpleActor INTEGER default 0;");
                }
            }

            nRet = DB.StepHasRow("select 1 from sqlite_master where type = \"table\" and lower(name) = \"magic\";");

            if (!nRet)
            {
                GameCenterDialogs.ShowMessage("数据库中的表: Magic 不存在");
                return Result;
            }

            Result = true;
        }
        finally
        {
            DB.Dispose();
        }
        return Result;
    }
}
