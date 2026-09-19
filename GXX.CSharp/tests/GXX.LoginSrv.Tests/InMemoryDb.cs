using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Util;
using GXX.LoginSrv;

namespace GXX.LoginSrv.Tests;

/// <summary>内存版 TAccountDB（单测用；对应 AccountDB.pas 抽象层）。</summary>
public sealed class InMemoryAccountDb : TAccountDB
{
    public readonly Dictionary<string, TAccountInfo> Store = new(StringComparer.Ordinal);
    public readonly List<string> EnabledCalls = new();

    public int InitCount;
    public int FinalCount;

    public InMemoryAccountDb() : base("") { }

    protected override void DoInit() => InitCount++;
    protected override void DoFinal() => FinalCount++;

    protected override bool DoGetAccountByQuick(string UID, string ID, ref TAccountInfo AccountInfo)
    {
        foreach (var kv in Store)
        {
            if (kv.Value.UidStr == UID && kv.Value.CidStr == ID) { AccountInfo = kv.Value; return true; }
        }
        return false;
    }

    protected override bool DoGetAccountByPhone(string Phone, ref TAccountInfo AccountInfo)
    {
        foreach (var kv in Store)
        {
            if (kv.Value.MobilePhoneStr == Phone) { AccountInfo = kv.Value; return true; }
        }
        return false;
    }

    protected override bool DoGetAccount(string AccountName, ref TAccountInfo AccountInfo)
    {
        if (Store.TryGetValue(AccountName, out TAccountInfo v)) { AccountInfo = v; return true; }
        return false;
    }

    protected override int DoFindAccount(string AccountName, TAccountList AccountList)
    {
        AccountList.Clear();
        foreach (var kv in Store)
        {
            if (kv.Key.StartsWith(AccountName, StringComparison.Ordinal)) AccountList.Add(kv.Value);
        }
        return AccountList.Count;
    }

    protected override bool DoUpdateAccount(TAccountInfo AccountInfo, TAccountUpdateField UpdateField)
    {
        Store[AccountInfo.AccountNameStr] = AccountInfo;
        return true;
    }

    protected override void DoGetAllAccount(TStringList AccountList)
    {
        AccountList.Clear();
        foreach (var kv in Store)
        {
            AccountList.AddObject(kv.Key, (int)kv.Value.IsDisable);
        }
    }

    protected override bool DoEnabledAccounts(TStringList AccountList, bool Enabled)
    {
        for (int i = 0; i < AccountList.Count; i++)
        {
            string name = AccountList[i];
            EnabledCalls.Add(name + "=" + (Enabled ? "1" : "0"));
            if (Store.TryGetValue(name, out TAccountInfo v))
            {
                v.IsDisable = Enabled ? (byte)0 : (byte)1;
                Store[name] = v;
            }
        }
        return true;
    }

    protected override bool DoCheckAccountExists(string AccountName) => Store.ContainsKey(AccountName);

    protected override bool DoAddAccount(TAccountInfo AccountInfo)
    {
        if (Store.ContainsKey(AccountInfo.AccountNameStr)) return false;
        Store[AccountInfo.AccountNameStr] = AccountInfo;
        return true;
    }

    protected override bool DoUnLockAccount(string AccountName)
    {
        if (!Store.TryGetValue(AccountName, out TAccountInfo v)) return false;
        v.LastActionTick = 0;
        v.ErrorCount = 0;
        Store[AccountName] = v;
        return true;
    }
}

// ---------------------------------------------------------------------------
// MySqlAccountDB 接缝的内存实现（不连真实 MySQL）
// ---------------------------------------------------------------------------

/// <summary>接缝：TMySQLLib 的内存替身。</summary>
public sealed class FakeMySqlLib : IMySqlLib
{
}

/// <summary>接缝：IMySqlDatabaseFactory 的内存实现。</summary>
public sealed class FakeMySqlFactory : IMySqlDatabaseFactory
{
    public FakeMySqlDatabase? Last;

    public IMySqlLib CreateLib() => new FakeMySqlLib();

    public IMySqlDatabase CreateDatabase(IMySqlLib lib)
    {
        Last = new FakeMySqlDatabase();
        return Last;
    }
}

/// <summary>接缝：IMySqlClient 的内存替身。</summary>
public sealed class FakeMySqlClient : IMySqlClient
{
}

/// <summary>一行 Account 表数据（列顺序与 select_Account 的 21/23 列 1:1）。</summary>
public sealed class FakeRow
{
    /// <summary>0 Account,1 Disable,2 Password,3 UserName,4 IDCard,5 BirthDay,6 Questions1,7 Answers1,
    /// 8 Questions2,9 Answers2,10 Phone,11 MobilePhone,12 Mail,13 L2Password,14 CreateDate,15 LoginDate,
    /// 16 LoginMac,17 LoginIP,18 LastActionTick,19 ErrorCount,20 Memo,21 UID,22 CID</summary>
    public readonly string[] Cols = new string[23];

    public FakeRow()
    {
        for (int i = 0; i < Cols.Length; i++) Cols[i] = "";
        Cols[1] = "0";
        Cols[14] = "0";
        Cols[15] = "0";
        Cols[17] = "0";
        Cols[18] = "0";
        Cols[19] = "0";
    }

    public string Account { get => Cols[0]; set => Cols[0] = value; }
    public int Disable { get => int.Parse(Cols[1]); set => Cols[1] = value.ToString(); }
    public string MobilePhone { get => Cols[11]; set => Cols[11] = value; }
    public string UID { get => Cols[21]; set => Cols[21] = value; }
    public string CID { get => Cols[22]; set => Cols[22] = value; }
}

/// <summary>接缝：TMySqlStatement 的内存实现（按语句名模拟同一套 SQL 语义）。</summary>
public sealed class FakeMySqlStatement : IMySqlStatement
{
    public readonly string Name;
    private readonly FakeMySqlDatabase _db;

    public FakeMySqlStatement(string name, FakeMySqlDatabase db)
    {
        Name = name;
        _db = db;
    }

    public string Sql { get; set; } = "";

    public int PrepareCount;
    public int FinalizeCount;
    public int ResetCount;

    /// <summary>顺序绑定参数（OrderBindParam*）。</summary>
    public readonly List<object> Ordered = new();

    /// <summary>按序号绑定参数（BindParamText(index, ...)）。</summary>
    public readonly Dictionary<int, string> Indexed = new();

    /// <summary>最近一次 Query/Step 时的参数快照（原文 Reset 会清空参数，故需快照）。</summary>
    public List<object> LastStepParams = new();
    public Dictionary<int, string> LastQueryIndexed = new();

    public int FieldCount { get; set; }

    public List<FakeRow> ResultRows = new();
    private int _cursor = -1;

    public void Prepare() => PrepareCount++;
    public void FinalizeStatement() => FinalizeCount++;
    public void Reset()
    {
        ResetCount++;
        Ordered.Clear();
        Indexed.Clear();
        ResultRows = new List<FakeRow>();
        _cursor = -1;
        FieldCount = 0;
    }

    public void BindParamText(int index, string value) => Indexed[index] = value;
    public void BindParamText(string value) => Ordered.Add(value);
    public void BindParamInt(int value) => Ordered.Add(value);

    public bool Query()
    {
        LastQueryIndexed = new Dictionary<int, string>(Indexed);
        _db.RaiseRequest();
        _cursor = -1;
        ResultRows = _db.Select(this);
        FieldCount = _db.FieldCountOf(Name);
        return ResultRows.Count > 0;
    }

    public bool Fetch()
    {
        _cursor++;
        return _cursor < ResultRows.Count;
    }

    public bool Step()
    {
        LastStepParams = new List<object>(Ordered);
        _db.RaiseRequest();
        return _db.StepStatement(this);
    }

    private string Col(int i) => ResultRows[_cursor].Cols[i];

    public string GetColumnValueText(int index) => Col(index);
    public bool GetColumnValueBool(int index) => Col(index) != "0";
    public int GetColumnValueInt(int index)
        => string.IsNullOrEmpty(Col(index)) ? 0 : int.Parse(Col(index));

    public string OrderGetColumnValueText => Col(0);
    public int OrderGetColumnValueInt => string.IsNullOrEmpty(Col(1)) ? 0 : int.Parse(Col(1));
}

/// <summary>接缝：TMySQLDataBase 的内存实现（记录连接串/语句/事务，模拟 Account 表）。</summary>
public sealed class FakeMySqlDatabase : IMySqlDatabase
{
    public readonly Dictionary<string, FakeMySqlStatement> Statements = new(StringComparer.Ordinal);
    public readonly List<string> ConnectLog = new();
    public readonly List<string> ExecLog = new();
    public readonly List<FakeRow> Rows = new();

    private bool _connected;
    private readonly FakeMySqlClient _client = new();

    public int InitCount;
    public int CommitCount;
    public int RollbackCount;
    public bool InTransaction;

    /// <summary>测试辅助：强制覆盖某语句返回的列数（触发原文 Assert）。</summary>
    public (string Name, int Count)? ForceFieldCount;

    /// <summary>测试辅助：Step 抛异常（触发 DoEnabledAccounts 的 except → Rollback）。</summary>
    public bool ThrowOnStep;

    public string CharacterSetName { get; set; } = "";
    public IMySqlClient? MySql => _connected ? _client : null;

    public event Action<object?>? OnRequest;

    public void RaiseRequest() => OnRequest?.Invoke(this);

    /// <summary>测试辅助：强制断开（对应 FDB.MySQL = nil）。</summary>
    public void SetConnectedForTest(bool value) => _connected = value;

    public void Init() => InitCount++;

    public void Connect(string server, string user, string password, string database, ushort port, uint flags)
    {
        ConnectLog.Add($"{server}|{user}|{password}|{database}|{port}|{flags}");
        _connected = true;
    }

    public IMySqlStatement AddSQLStatement(string name)
    {
        var st = new FakeMySqlStatement(name, this);
        Statements[name] = st;
        return st;
    }

    public void StartTransaction() { InTransaction = true; CommitCount += 0; }
    public void Commit() { CommitCount++; InTransaction = false; }
    public void Rollback() { RollbackCount++; InTransaction = false; }

    public void Exec(string sql) => ExecLog.Add(sql);

    public int FieldCountOf(string name)
    {
        if (ForceFieldCount.HasValue && ForceFieldCount.Value.Name == name)
            return ForceFieldCount.Value.Count;
        return name switch
        {
            "select_Account_quick" => 23,
            "exists_Account" => 1,
            "get_all_Account" => 2,
            _ => 21,
        };
    }

    /// <summary>按语句名 + 绑定参数模拟 SELECT（同时兼容顺序绑定与按序号绑定）。</summary>
    public List<FakeRow> Select(FakeMySqlStatement st)
    {
        var result = new List<FakeRow>();
        switch (st.Name)
        {
            case "select_Account":
            {
                string account = Param(st, 0);
                foreach (var r in Rows) if (r.Account == account) result.Add(r);
                break;
            }
            case "select_Account_phone":
            {
                string phone = Param(st, 0);
                foreach (var r in Rows) if (r.MobilePhone == phone) result.Add(r);
                break;
            }
            case "select_Account_quick":
            {
                string uid = Param(st, 0);
                string cid = Param(st, 1);
                foreach (var r in Rows) if (r.UID == uid && r.CID == cid) result.Add(r);
                break;
            }
            case "find_Account":
            {
                string pattern = Param(st, 0);
                string prefix = pattern.EndsWith("%", StringComparison.Ordinal) ? pattern.Substring(0, pattern.Length - 1) : pattern;
                foreach (var r in Rows) if (r.Account.StartsWith(prefix, StringComparison.Ordinal)) result.Add(r);
                break;
            }
            case "exists_Account":
            {
                string account = Param(st, 0);   // 原文此处用 OrderBindParamText（顺序绑定）
                foreach (var r in Rows) if (r.Account == account) result.Add(r);
                break;
            }
            case "get_all_Account":
                result.AddRange(Rows);
                break;
        }
        return result;
    }

    /// <summary>取第 i 个绑定参数：优先按序号绑定（BindParamText(i, ...)），否则用顺序绑定。</summary>
    private static string Param(FakeMySqlStatement st, int i)
    {
        if (st.Indexed.TryGetValue(i, out string? v)) return v;
        return i < st.Ordered.Count ? st.Ordered[i] as string ?? "" : "";
    }

    /// <summary>按语句名 + 顺序参数模拟 INSERT/UPDATE。</summary>
    public bool StepStatement(FakeMySqlStatement st)
    {
        if (ThrowOnStep) throw new InvalidOperationException("Step failed (test)");
        switch (st.Name)
        {
            case "new_Account":
            {
                var row = new FakeRow
                {
                    Account = Str(st, 0),
                };
                row.Cols[2] = Str(st, 1);
                row.Cols[3] = Str(st, 2);
                row.Cols[4] = Str(st, 3);
                row.Cols[5] = Str(st, 4);
                row.Cols[6] = Str(st, 5);
                row.Cols[7] = Str(st, 6);
                row.Cols[8] = Str(st, 7);
                row.Cols[9] = Str(st, 8);
                row.Cols[10] = Str(st, 9);
                row.Cols[11] = Str(st, 10);
                row.Cols[12] = Str(st, 11);
                row.Cols[13] = Str(st, 12);
                row.Cols[14] = Int(st, 13).ToString();
                row.Cols[20] = Str(st, 14);
                row.Cols[21] = Str(st, 15);
                row.Cols[22] = Str(st, 16);
                Rows.Add(row);
                return true;
            }
            case "update_Account_1":
            {
                var row = Find(st, 6);
                if (row == null) return false;
                row.Cols[13] = Str(st, 0);
                row.Cols[15] = Int(st, 1).ToString();
                row.Cols[16] = Str(st, 2);
                row.Cols[17] = Int(st, 3).ToString();
                row.Cols[18] = Int(st, 4).ToString();
                row.Cols[19] = Int(st, 5).ToString();
                return true;
            }
            case "update_Account_2":
            {
                var row = Find(st, 13);
                if (row == null) return false;
                row.Cols[2] = Str(st, 0);
                row.Cols[3] = Str(st, 1);
                row.Cols[4] = Str(st, 2);
                row.Cols[5] = Str(st, 3);
                row.Cols[6] = Str(st, 4);
                row.Cols[7] = Str(st, 5);
                row.Cols[8] = Str(st, 6);
                row.Cols[9] = Str(st, 7);
                row.Cols[10] = Str(st, 8);
                row.Cols[11] = Str(st, 9);
                row.Cols[12] = Str(st, 10);
                row.Cols[13] = Str(st, 11);
                row.Cols[20] = Str(st, 12);
                return true;
            }
            case "unlock_Account":
            {
                var row = Find(st, 0);
                if (row == null) return false;
                row.Cols[18] = "0";
                row.Cols[19] = "0";
                return true;
            }
            case "enabled_Account":
            {
                var row = Find(st, 1);
                if (row == null) return false;
                row.Cols[1] = Int(st, 0).ToString();
                return true;
            }
        }
        return false;
    }

    private FakeRow? Find(FakeMySqlStatement st, int accountIndex)
    {
        string account = Str(st, accountIndex);
        foreach (var r in Rows) if (r.Account == account) return r;
        return null;
    }

    private static string Str(FakeMySqlStatement st, int i)
        => i < st.Ordered.Count ? st.Ordered[i] as string ?? "" : "";

    private static int Int(FakeMySqlStatement st, int i)
        => i < st.Ordered.Count ? Convert.ToInt32(st.Ordered[i]) : 0;

    /// <summary>测试辅助：造一行数据。</summary>
    public FakeRow AddRow(string account, string password = "", int disable = 0)
    {
        var row = new FakeRow { Account = account, Disable = disable };
        row.Cols[2] = password;
        Rows.Add(row);
        return row;
    }
}
