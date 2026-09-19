// 源单元（文件头证据，供 tools/audit-coverage.ps1 的 E2 规则识别）：
//   Source/M2Engine/SqliteUserShopDB.pas
//   Source/M2Engine/MySqlUserShopDB.pas
//
// DbLayer 测试基础设施：
//   * <see cref="FakeDbLayerHost"/>             —— IDbLayerHost 的内存实现（含锁计数、物品读写记录）
//   * <see cref="FakeSqliteDatabase"/>          —— ISqliteDatabase 的内存实现（记录每个语句的 SQL 与绑定/读取序号）
//   * <see cref="FakeMySqlDatabase"/>           —— IMySqlDatabase 的内存实现
//   * <see cref="ScriptedSqliteStatement"/> / <see cref="ScriptedMySqlStatement"/> —— 可编排结果集的语句
//   * <see cref="FakeDbLayerEnvironment"/>       —— g_Config 相关成员的内存实现
// **测试绝不连真实数据库**：所有 DB 访问都注入上面的内存实现。

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using GXX.M2Server.DbLayer;

namespace GXX.M2Server.Tests;

/// <summary>记录一次"顺序绑定"调用。</summary>
public sealed class BindCall
{
    public string Kind = "";
    public object? Value;
    public string Text => Value?.ToString() ?? "";
}

/// <summary>记录一次"顺序读列"调用。</summary>
public sealed class ReadCall
{
    public string Kind = "";
    public int ColumnIndex;
}

/// <summary>
/// 一个可编排的内存语句：记录 <c>.Sql</c> 赋值、绑定序列、读列序列；
/// <c>Rows</c> 为预设结果集（每行一个 object?[]），<c>StepCode</c> 控制 <c>Step()</c> 返回值。
/// </summary>
public class ScriptedStatementBase
{
    /// <summary>Delphi 侧把这个 statement 绑定到哪个字段（测试用来对指纹）。</summary>
    public string Label = "";

    public readonly List<string> SqlAssignments = new();
    /// <summary>累计的绑定调用（跨多次 Reset，单位是"调用次数"）。</summary>
    public readonly List<BindCall> Binds = new();
    /// <summary>最近一次 Step()/Query() 执行时的绑定快照（Reset 不会清空）——断言参数顺序用这个。</summary>
    public readonly List<BindCall> LastBinds = new();
    public readonly List<ReadCall> Reads = new();

    public int PrepareCount;
    public int ResetCount;
    public int FinalizeCount;
    public int StepCount;
    public int QueryCount;
    public int FetchCount;

    /// <summary>预设结果集。</summary>
    public readonly List<object?[]> Rows = new();

    /// <summary>Step() 在第一行之前返回的码（SQLite：SQLITE_ROW=100 表示有行，SQLITE_DONE=101 表示无）。</summary>
    public int StepCode = 100;

    /// <summary>Step()/Query() 返回值（MySQL 布尔语义）。</summary>
    public bool QueryResult = true;

    protected int _rowIndex;
    protected int _readColumn;
    protected int _bindIndex;

    /// <summary>最近一次被赋的 SQL。</summary>
    public string Sql { get; protected set; } = "";

    public void AddRow(params object?[] cells) => Rows.Add(cells);

    protected object? Cell(int column)
    {
        if (_rowIndex <= 0 || _rowIndex > Rows.Count) return null;
        object?[] row = Rows[_rowIndex - 1];
        return column >= 0 && column < row.Length ? row[column] : null;
    }

    protected object? NextCell()
    {
        object? v = Cell(_readColumn);
        Reads.Add(new ReadCall { Kind = "next", ColumnIndex = _readColumn });
        _readColumn++;
        return v;
    }
}

/// <summary>SQLite 语句的内存实现。</summary>
public sealed class ScriptedSqliteStatement : ScriptedStatementBase, ISqliteStatement
{
    public string Sql
    {
        get => base.Sql;
        set { base.Sql = value; SqlAssignments.Add(value); }
    }

    public void Prepare() => PrepareCount++;

    public void StatementFinalize() => FinalizeCount++;

    public void Reset()
    {
        ResetCount++;
        _rowIndex = 0;
        _readColumn = 0;
        _bindIndex = 0;
        // 清空当前绑定向量（Re-armed）：LastBinds 保留上一次 Step/Query 的快照供断言。
        Binds.Clear();
    }

    public void OrderBindInt(int value) => Binds.Add(new BindCall { Kind = "int", Value = value });

    public void OrderBindInt64(long value) => Binds.Add(new BindCall { Kind = "int64", Value = value });

    public void OrderBindBool(bool value) => Binds.Add(new BindCall { Kind = "bool", Value = value });

    public void OrderBindText(string value) => Binds.Add(new BindCall { Kind = "text", Value = value ?? "" });

    public int Step()
    {
        StepCount++;
        LastBinds.Clear();
        LastBinds.AddRange(Binds);
        _readColumn = 0;
        // Reset() 复位游标；未 Reset 时 Step 继续前移（与 sqlite3_step 一致：DONE 之后必须 Reset）。
        if (_rowIndex >= Rows.Count) return SqliteCodes.SQLITE_DONE;
        if (StepCode == SqliteCodes.SQLITE_DONE) return SqliteCodes.SQLITE_DONE;
        _rowIndex++;
        return StepCode;
    }

    public int OrderGetColumnValueInt => Convert.ToInt32(NextCell() ?? 0);
    public long OrderGetColumnValueInt64 => Convert.ToInt64(NextCell() ?? 0L);
    public bool OrderGetColumnValueBool => Convert.ToInt64(NextCell() ?? 0L) != 0;
    public string OrderGetColumnValueText => NextCell()?.ToString() ?? "";
    public double OrderGetColumnValueDouble => Convert.ToDouble(NextCell() ?? 0.0);
}

/// <summary>MySQL 语句的内存实现。</summary>
public sealed class ScriptedMySqlStatement : ScriptedStatementBase, IMySqlStatement
{
    public string Sql
    {
        get => base.Sql;
        set { base.Sql = value; SqlAssignments.Add(value); }
    }

    /// <summary>MySQL 版的 RowCount（DoAddAttentionItem 的"存在性"判据）。返回结果集行数或受影响行数。</summary>
    public int RowCount { get; set; }

    public void Prepare() => PrepareCount++;

    public void StatementFinalize() => FinalizeCount++;

    public void Reset()
    {
        ResetCount++;
        _rowIndex = 0;
        _readColumn = 0;
        _bindIndex = 0;
        // 清空当前绑定向量（Re-armed）：LastBinds 保留上一次 Step/Query 的快照供断言。
        Binds.Clear();
    }

    public void OrderBindParamInt(int value) => Binds.Add(new BindCall { Kind = "int", Value = value });

    public void OrderBindParamBool(bool value) => Binds.Add(new BindCall { Kind = "bool", Value = value });

    public void OrderBindParamText(string value) => Binds.Add(new BindCall { Kind = "text", Value = value ?? "" });

    public void OrderBindParamDateTime(DateTime value) => Binds.Add(new BindCall { Kind = "datetime", Value = value });

    public void OrderBindDouble(double value) => Binds.Add(new BindCall { Kind = "double", Value = value });

    public bool Query()
    {
        QueryCount++;
        LastBinds.Clear();
        LastBinds.AddRange(Binds);
        _readColumn = 0;
        return QueryResult;
    }

    public bool Fetch()
    {
        FetchCount++;
        _readColumn = 0;
        if (_rowIndex >= Rows.Count) return false;
        _rowIndex++;
        return true;
    }

    public bool Step()
    {
        StepCount++;
        LastBinds.Clear();
        LastBinds.AddRange(Binds);
        return QueryResult;
    }

    public int OrderGetColumnValueInt => Convert.ToInt32(NextCell() ?? 0);
    public long OrderGetColumnValueInt64 => Convert.ToInt64(NextCell() ?? 0L);
    public bool OrderGetColumnValueBool => Convert.ToInt64(NextCell() ?? 0L) != 0;
    public string OrderGetColumnValueText => NextCell()?.ToString() ?? "";
    public double OrderGetColumnValueDouble => Convert.ToDouble(NextCell() ?? 0.0);
    public DateTime OrderGetColumnValueDateTime
    {
        get
        {
            object? v = NextCell();
            return v switch
            {
                DateTime dt => dt,
                long l => DelphiDateUtil.UnixToDateTime(l),
                int i => DelphiDateUtil.UnixToDateTime(i),
                string s => DateTime.TryParse(s, out var p) ? p : default,
                _ => default,
            };
        }
    }
}

/// <summary>SQLite 库的内存实现：<c>AddSQLStatement(name)</c> 按名复用（与原文 SQLite3DataBase.pas:1164 一致）。</summary>
public sealed class FakeSqliteDatabase : ISqliteDatabase
{
    private readonly Dictionary<string, ScriptedSqliteStatement> _byName = new(StringComparer.Ordinal);
    private readonly Dictionary<string, ScriptedSqliteStatement> _byLabel = new(StringComparer.Ordinal);

    /// <summary>按 DoInit 顺序登记的语句（用于指纹对账）。</summary>
    public readonly List<ScriptedSqliteStatement> Created = new();

    /// <summary>DoInit 里每个 <c>AddSQLStatement</c> 的实参（顺序，可含重复）。</summary>
    public readonly List<string> AddedNames = new();

    public readonly List<string> ExecutedSql = new();
    public readonly List<string> Transactions = new();

    public bool Connected { get; set; }
    public string Database { get; set; } = "";
    public bool MustExist { get; set; }
    public bool UseThreadMode { get; set; }
    public int ErrorCode => 0;

    /// <summary>给某个 label 的语句预置结果集。</summary>
    public ScriptedSqliteStatement For(string label) => _byLabel[label];

    /// <summary>是否已为某 label 创建语句。</summary>
    public bool Has(string label) => _byLabel.ContainsKey(label);

    public ISqliteStatement AddSQLStatement(string name)
    {
        AddedNames.Add(name);
        if (_byName.TryGetValue(name, out ScriptedSqliteStatement? existing)) return existing;

        var stmt = new ScriptedSqliteStatement { Label = name };
        _byName[name] = stmt;
        _byLabel[name] = stmt;
        Created.Add(stmt);
        return stmt;
    }

    public void BeginTransaction() => Transactions.Add("begin");
    public void Commit() => Transactions.Add("commit");
    public void RollBack() => Transactions.Add("rollback");
    public void Execute(string sql) => ExecutedSql.Add(sql);
}

/// <summary>MySQL 库的内存实现。</summary>
public sealed class FakeMySqlDatabase : IMySqlDatabase
{
    private readonly Dictionary<string, ScriptedMySqlStatement> _byName = new(StringComparer.Ordinal);
    private readonly Dictionary<string, ScriptedMySqlStatement> _byLabel = new(StringComparer.Ordinal);

    public readonly List<ScriptedMySqlStatement> Created = new();
    public readonly List<string> AddedNames = new();
    public readonly List<string> ExecutedSql = new();
    public readonly List<string> Transactions = new();

    public string CharacterSetName { get; set; } = "";
    public object? MySql => this;
    public bool InitCalled;
    public string ConnectedTo = "";

    public ScriptedMySqlStatement For(string label) => _byLabel[label];
    public bool Has(string label) => _byLabel.ContainsKey(label);

    public IMySqlStatement AddSQLStatement(string name)
    {
        AddedNames.Add(name);
        if (_byName.TryGetValue(name, out ScriptedMySqlStatement? existing)) return existing;

        var stmt = new ScriptedMySqlStatement { Label = name };
        _byName[name] = stmt;
        _byLabel[name] = stmt;
        Created.Add(stmt);
        return stmt;
    }

    public void StartTransaction() => Transactions.Add("begin");
    public void Commit() => Transactions.Add("commit");
    public void RollBack() => Transactions.Add("rollback");
    public void Exec(string sql) => ExecutedSql.Add(sql);
    public void ClearResult() { }
    public void Connect(string server, string user, string password, string database, ushort port, uint flags)
        => ConnectedTo = $"{server}/{user}/{password}/{database}/{port}/{flags}";
    public void Init() => InitCalled = true;
    public event Action? OnRequest;
    public void RaiseOnRequest() => OnRequest?.Invoke();
}

/// <summary>IDbLayerHost 的内存实现。</summary>
public sealed class FakeDbLayerHost : IDbLayerHost
{
    public object? DataBase { get; set; }

    public int LockCount;
    public int UnLockCount;

    public readonly List<string> LoadedItems = new();
    public readonly List<string> SavedItems = new();

    /// <summary>被 LoadItemFromDB 写入的 UserItem（按 ParentID 索引）。</summary>
    public readonly Dictionary<int, GXX.Core.Protocol.TUserItem> LoadedByParent = new();

    public void Lock() => LockCount++;

    public void UnLock() => UnLockCount++;

    public void LoadItemFromDB(GXX.Core.Protocol.TUserItem userItem, int parentId, int itemType, int itemIndex)
    {
        LoadedItems.Add($"{parentId}/{itemType}/{itemIndex}");
        if (LoadedByParent.TryGetValue(parentId, out GXX.Core.Protocol.TUserItem item))
        {
            userItem.MakeIndex = item.MakeIndex;
            userItem.wIndex = item.wIndex;
            userItem.NameStr = item.NameStr;
            userItem.Dura = item.Dura;
        }
    }

    public void SaveItemToDB(GXX.Core.Protocol.TUserItem userItem, int parentId, int itemType, int itemIndex)
        => SavedItems.Add($"{parentId}/{itemType}/{itemIndex}/{userItem.MakeIndex}");
}

/// <summary>IDbLayerEnvironment 的内存实现（对应原文 g_Config 的相关成员）。</summary>
public sealed class FakeDbLayerEnvironment : IDbLayerEnvironment
{
    public bool boOfflineCloseMyShop { get; set; }
    public bool boEnabledMySellShopItemTime { get; set; }
    public int nMySellShopItemTime { get; set; }
    public int nMaxMyShopStorageItemCount { get; set; } = 7;
    public int nHumanMaxGold { get; set; }
    public int btAuctionItemColors { get; set; }
    public int nAuctionCurrencyType { get; set; }
    public string sGameGoldName { get; set; } = "";
    public string sGamePointName { get; set; } = "";
    public string sGameDiamondName { get; set; } = "";
    public string sGameGirdName { get; set; } = "";
    public uint dwAuctionGoldTaxRate { get; set; }
    public uint dwAuctionGameGoldTaxRate { get; set; }
    public uint dwAuctionGameDiamondTaxRate { get; set; }
    public uint dwAuctionGameGirdTaxRate { get; set; }
    public uint dwAuctionGamePointTaxRate { get; set; }
}

/// <summary>记录 MainOutMessage 的日志接缝。</summary>
public sealed class CapturingDbLayerLog : IDbLayerLog
{
    public readonly List<string> Messages = new();
    public void MainOutMessage(string msg) => Messages.Add(msg);
}

/// <summary>DbLayer 测试的公共辅助。</summary>
public static class DbLayerTestKit
{
    /// <summary>SQL 文本的 SHA-256（小写十六进制），与 _recon/gen-fingerprints.mjs 口径一致。</summary>
    public static string Sha256(string sql)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(sql));
        var sb = new StringBuilder(hash.Length * 2);
        foreach (byte b in hash) sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    /// <summary>建立一个隔离的测试环境（全局 g_Config / 日志 / 时钟接缝全部替换为内存实现）。</summary>
    public static (FakeDbLayerEnvironment Env, CapturingDbLayerLog Log) Isolate()
    {
        var env = new FakeDbLayerEnvironment();
        var log = new CapturingDbLayerLog();
        DbLayerGlobals.Environment = env;
        DbLayerGlobals.Log = log;
        DbLayerGlobals.GetTickCount = () => 0u;
        DbLayerRunSeam.ResetDefaults();
        return (env, log);
    }
}
