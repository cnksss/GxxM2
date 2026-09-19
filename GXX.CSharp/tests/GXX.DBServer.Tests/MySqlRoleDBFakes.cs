using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GXX.DBServer;

namespace GXX.DBServer.Tests;

/// <summary>
/// 内存版 MySQL 接缝实现（**绝不连真实 MySQL**）。
///
/// 模拟 TMySQLDataBase / TMySqlStatement 的可观测行为：
///   · 每个语句按 <c>name → 结果集队列</c> 出结果（同名语句可以排队多次，用于"多职业块"这类重复查询）；
///   · 记录每次 Reset / Prepare / FinalizeStatement / 参数绑定 / Query / Fetch / Step / Exec，便于断言顺序；
///   · Exec 走 <c>sql → 预置结果/异常/返回值</c> 脚本。
/// </summary>
public sealed class FakeMySqlStatement : IRoleMySqlStatement
{
    private readonly FakeMySqlDatabase _db;
    private readonly string _name;
    private readonly Queue<FakeResultSet> _results = new();
    private readonly List<(string Kind, object? Value)> _binds = new();
    private readonly List<(string Kind, object? Value)> _bindHistory = new();
    private readonly List<int> _columnValueIntCalls = new();

    private FakeResultSet? _current;
    private int _columnIndex;
    private bool _rowsOpen;
    private int _rowCursor = -1;

    public FakeMySqlStatement(FakeMySqlDatabase db, string name)
    {
        _db = db;
        _name = name;
    }

    public string Name => _name;
    public string Sql { get; set; } = "";
    public int PrepareCount { get; private set; }
    public int ResetCount { get; private set; }
    public int FinalizeCount { get; private set; }
    public int QueryCount { get; private set; }
    public int StepCount { get; private set; }
    public bool LastStepResult { get; set; } = true;

    /// <summary>按 [text, int, bool, double] 记录的绑定值（文本 / 整数 / 布尔 / 浮点分开存）。</summary>
    public IReadOnlyList<(string Kind, object? Value)> Binds => _binds;
    /// <summary>历史绑定（Reset 不清空；原文的 finally-Reset 会清掉当前绑定，测试需要历史）。</summary>
    public IReadOnlyList<(string Kind, object? Value)> BindHistory => _bindHistory;
    public List<string> AllBoundTexts => _bindHistory.Where(b => b.Kind == "T").Select(b => (string)b.Value!).ToList();
    public List<int> AllBoundNumbers => _bindHistory.Where(b => b.Kind is "I" or "B" or "D").Select(b => b.Value switch { int i => i, bool bl => bl ? 1 : 0, double d => (int)d, _ => 0 }).ToList();

    /// <summary>只取文本绑定。</summary>
    public List<string> BoundTexts => _binds.Where(b => b.Kind == "T").Select(b => (string)b.Value!).ToList();

    /// <summary>只取整数/布尔/浮点绑定（按绑定顺序，整型化）。</summary>
    public List<int> BoundNumbers => _binds
        .Where(b => b.Kind is "I" or "B" or "D")
        .Select(b => b.Value switch
        {
            int i => i,
            bool bl => bl ? 1 : 0,
            double d => (int)d,
            _ => 0,
        }).ToList();

    /// <summary>结果集用完后自动补的默认空结果（true 时不会因为"没排队"而报错）。</summary>
    public bool AllowEmptyFallback { get; set; } = true;

    public void EnqueueResult(FakeResultSet rs) => _results.Enqueue(rs);

    public void Prepare() { PrepareCount++; _db.Log($"{_name}.Prepare"); }

    public void FinalizeStatement() { FinalizeCount++; _db.Log($"{_name}.Finalize"); }

    public void Reset()
    {
        ResetCount++;
        _binds.Clear();
        _columnValueIntCalls.Clear();
        _current = null;
        _columnIndex = 0;
        _rowsOpen = false;
        _rowCursor = -1;
        _db.Log($"{_name}.Reset");
    }

    public void OrderBindParamText(string value) { _binds.Add(("T", value)); _bindHistory.Add(("T", value)); _db.Log($"{_name}.BindText({value})"); }
    public void OrderBindParamInt(int value) { _binds.Add(("I", value)); _bindHistory.Add(("I", value)); _db.Log($"{_name}.BindInt({value})"); }
    public void OrderBindParamBool(bool value) { _binds.Add(("B", value)); _bindHistory.Add(("B", value)); _db.Log($"{_name}.BindBool({value})"); }
    public void OrderBindParamDouble(double value) { _binds.Add(("D", value)); _bindHistory.Add(("D", value)); _db.Log($"{_name}.BindDouble({value})"); }

    public bool Query()
    {
        QueryCount++;
        _db.Log($"{_name}.Query");
        if (_results.Count > 0)
        {
            _current = _results.Dequeue();
        }
        else if (AllowEmptyFallback)
        {
            _current = FakeResultSet.Empty;
        }
        else
        {
            throw new InvalidOperationException($"FakeMySqlStatement('{_name}') 没有排队结果集");
        }

        if (_current.ThrowOnQuery is not null) throw _current.ThrowOnQuery;
        _columnIndex = 0;
        _rowCursor = -1;
        _rowsOpen = true;
        return _current.Rows.Count > 0;
    }

    public bool Fetch()
    {
        if (!_rowsOpen || _current is null) return false;
        _rowCursor++;
        // 每换一行，列游标归零（对应 C 侧 statement 的按行重置行为）
        if (_rowCursor > 0) _columnIndex = 0;
        return _rowCursor < _current.Rows.Count;
    }

    public bool Step()
    {
        StepCount++;
        _db.Log($"{_name}.Step");
        // 原文常见的 "Step 型写语句" 路径是 Reset -> 绑参 -> Step（不经过 Query）；
        // 为了让测试能用 EnqueueResult 预置 Step 结果，这里在 _current 为空时也取一次队列。
        if (_current is null && _results.Count > 0) _current = _results.Dequeue();
        if (_current is not null && _current.ThrowOnQuery is not null) throw _current.ThrowOnQuery;
        return _current?.StepResult ?? LastStepResult;
    }

    private FakeRow CurrentRow
    {
        get
        {
            if (_current is null || _rowCursor < 0 || _rowCursor >= _current.Rows.Count)
                throw new InvalidOperationException($"FakeMySqlStatement('{_name}') 未在有效行上取值");
            return _current.Rows[_rowCursor];
        }
    }

    public string GetColumnValueText(int index) { _db.Log($"{_name}.GetText({index})"); return CurrentRow.Text(index); }
    public int GetColumnValueInt(int index) { _columnValueIntCalls.Add(index); _db.Log($"{_name}.GetInt({index})"); return CurrentRow.Int(index); }

    public string OrderGetColumnValueText
    {
        get { _db.Log($"{_name}.OrderText@{_columnIndex}"); return CurrentRow.Text(_columnIndex++); }
    }

    public int OrderGetColumnValueInt
    {
        get { _db.Log($"{_name}.OrderInt@{_columnIndex}"); return CurrentRow.Int(_columnIndex++); }
    }

    public bool OrderGetColumnValueBool
    {
        get { _db.Log($"{_name}.OrderBool@{_columnIndex}"); return CurrentRow.Bool(_columnIndex++); }
    }

    public double OrderGetColumnValueDouble
    {
        get { _db.Log($"{_name}.OrderDouble@{_columnIndex}"); return CurrentRow.Double(_columnIndex++); }
    }
}

/// <summary>一行结果（按列序存文本/整数/浮点/空值）。</summary>
public sealed class FakeRow
{
    private readonly List<object?> _cells = new();

    public static FakeRow Of(params object?[] cells)
    {
        var r = new FakeRow();
        r._cells.AddRange(cells);
        return r;
    }

    public object? Cell(int i) => i < _cells.Count ? _cells[i] : null;

    public string Text(int i)
    {
        var c = Cell(i);
        return c switch
        {
            null => "",
            string s => s,
            bool b => b ? "1" : "0",
            _ => Convert.ToString(c, CultureInfo.InvariantCulture) ?? "",
        };
    }

    /// <summary>对应 GetColumnValueInt：null / 非数字文本 → 0（原文 libmysql 亦为 0）。</summary>
    public int Int(int i)
    {
        var c = Cell(i);
        return c switch
        {
            null => 0,
            int v => v,
            long v => (int)v,
            short v => v,
            byte v => v,
            ushort v => v,
            uint v => (int)v,
            bool b => b ? 1 : 0,
            double d => (int)d,
            string s => int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int p) ? p : 0,
            _ => 0,
        };
    }

    public bool Bool(int i)
    {
        var c = Cell(i);
        return c switch
        {
            null => false,
            bool b => b,
            string s => s is "1" or "-1" or "true" or "True",
            _ => Int(i) != 0,
        };
    }

    public double Double(int i)
    {
        var c = Cell(i);
        return c switch
        {
            null => 0,
            double d => d,
            string s => double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double p) ? p : 0,
            _ => Int(i),
        };
    }
}

/// <summary>一个语句的一次查询结果集。</summary>
public sealed class FakeResultSet
{
    public static readonly FakeResultSet Empty = new FakeResultSet();

    public readonly List<FakeRow> Rows = new();
    public Exception? ThrowOnQuery;
    public bool StepResult = true;

    public static FakeResultSet Rows_(params FakeRow[] rows)
    {
        var rs = new FakeResultSet();
        rs.Rows.AddRange(rows);
        return rs;
    }

    public static FakeResultSet Of(params object?[][] rows)
    {
        var rs = new FakeResultSet();
        foreach (var r in rows) rs.Rows.Add(FakeRow.Of(r));
        return rs;
    }

    public static FakeResultSet Throwing(Exception ex) => new FakeResultSet { ThrowOnQuery = ex };

    public static FakeResultSet Step(bool ok) => new FakeResultSet { StepResult = ok };
}

/// <summary>内存版 TMySQLDataBase。</summary>
public sealed class FakeMySqlDatabase : IRoleMySqlDatabase
{
    private readonly List<FakeMySqlStatement> _statements = new();
    private readonly Dictionary<string, FakeMySqlStatement> _byName = new();
    private readonly List<string> _log = new();

    public readonly List<string> Executed = new();
    public readonly List<string> Connected = new();
    public int InitCount, CommitCount, RollBackCount, StartTransactionCount, ClearStatementsCount;
    public string CharacterSet = "";
    public IRoleMySqlClient? FakeClient = new FakeMySqlClient();
    public Exception? ThrowOnExec;
    public Exception? ThrowOnStartTransaction;

    public IReadOnlyList<FakeMySqlStatement> Statements => _statements;
    public IReadOnlyList<string> Trace => _log;

    public void Log(string s) => _log.Add(s);

    public void Init() => InitCount++;
    public void Connect(string server, string user, string password, string database, int port, uint flags)
        => Connected.Add($"{server}|{user}|{password}|{database}|{port}|{flags}");
    public string CharacterSetName { get => CharacterSet; set => CharacterSet = value; }
    public IRoleMySqlClient? MySql => FakeClient;

    /// <summary>
    /// 同名语句只返回同一个实例（对应原文 `Stms.AddSQLStatement(name)` 在真实实现里的按名缓存；
    /// 测试可以先建好 get_db_constant_value 再让 DoInit 复用）。
    /// </summary>
    public IRoleMySqlStatement AddSQLStatement(string name)
    {
        if (_byName.TryGetValue(name, out var existing)) return existing;
        var s = new FakeMySqlStatement(this, name);
        _statements.Add(s);
        _byName[name] = s;
        return s;
    }

    /// <summary>原文 6064 `FDB.Statements.Clear`：清空语句表（版本查询那条随之消失）。</summary>
    public void ClearStatements()
    {
        ClearStatementsCount++;
        _statements.Clear();
        _byName.Clear();
    }

    public void StartTransaction()
    {
        StartTransactionCount++;
        if (ThrowOnStartTransaction is not null) throw ThrowOnStartTransaction;
    }

    public void Commit() => CommitCount++;
    public void RollBack() => RollBackCount++;
    public void Disconnect() { }

    public void Exec(string sql)
    {
        Executed.Add(sql);
        if (ThrowOnExec is not null) throw ThrowOnExec;
    }

    public event Action<object?>? OnRequest;

    /// <summary>模拟 libmysql 的"每次请求回调"（原文 FDB.OnRequest → FLastRequestTick）。</summary>
    public void RaiseOnRequest() => OnRequest?.Invoke(this);

    /// <summary>按 AddSQLStatement 的语句名取语句（原文 FStatementXxx 字段的等价物）。</summary>
    public FakeMySqlStatement Stmt(string name)
        => _byName.TryGetValue(name, out var s)
            ? s
            : throw new KeyNotFoundException($"没有名为 '{name}' 的语句（已注册：{string.Join(",", _byName.Keys.OrderBy(k => k))}）");

    public bool HasStatement(string name) => _byName.ContainsKey(name);

    public List<string> StatementNames => _statements.Select(s => s.Name).ToList();
}

/// <summary>内存版 TMySQLCli（仅作为"已连接"标记）。</summary>
public sealed class FakeMySqlClient : IRoleMySqlClient
{
}

/// <summary>内存版工厂。</summary>
public sealed class FakeMySqlFactory : IRoleMySqlDatabaseFactory
{
    public readonly FakeMySqlDatabase Database = new();
    public readonly FakeMySqlLib Lib = new();
    public int CreateLibCount, CreateDatabaseCount;

    public IRoleMySqlLib CreateLib() { CreateLibCount++; return Lib; }
    public IRoleMySqlDatabase CreateDatabase(IRoleMySqlLib lib) { CreateDatabaseCount++; return Database; }
}

/// <summary>内存版 TMySQLLib。</summary>
public sealed class FakeMySqlLib : IRoleMySqlLib
{
}
