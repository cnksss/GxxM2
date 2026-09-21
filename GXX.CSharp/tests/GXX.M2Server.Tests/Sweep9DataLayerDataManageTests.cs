// 源单元：Source/M2Engine/DataManage.pas（278 行）
// 对应实现：src/GXX.M2Server/Sweep9/DataLayer/DataManage.cs
//   （DataManageConst / DataManageGlobals / DataManageAccessSeam / TAccessEngine / TAccessTable）
//
// 覆盖口径：**每个公开成员至少一条用例**；每个分支/边界/原文缺陷都有差异断言。
// **绝不连真实数据库/COM/Access/Jet 引擎**：ADO 与 Access 全部走内存替身。

using System;
using System.Collections.Generic;
using GXX.Core.Util;
using GXX.M2Server.Sweep9.DataLayer;
using Xunit;

namespace GXX.M2Server.Tests;

// ===========================================================================
// 一、常量（DataManage.pas:55-73）—— 逐字保真
// ===========================================================================
public class Sweep9DataLayerDataManageConstTests
{
    [Fact]
    public void G_sShopItemTable_IsExactOriginalLiteral()
    {
        Assert.Equal("TBL_SHOPITEM", DataManageConst.g_sShopItemTable);
    }

    [Fact]
    public void G_sSQLString_IsExactOriginalLiteral()
    {
        Assert.Equal(
            "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=%s;Persist Security Info=False",
            DataManageConst.g_sSQLString);
    }

    /// <summary>
    /// `g_sShopItemField` 是原文 7 个字面量用 `+` 拼接的结果。
    /// **逐字符**比对（含原文的 `\t` 与对齐空格）—— 这处排版极易在移植时被"美化"掉。
    /// </summary>
    [Fact]
    public void G_sShopItemField_MatchesOriginalConcatenationByteForByte()
    {
        const string expected =
            "FLD_ITEMTYPE\t\t TINYINT      NOT NULL," +
            "FLD_ITEMNAME\t\t CHAR(14)\t    NOT NULL," +
            "FLD_ITEMPRICE   INT          NOT NULL," +
            "FLD_IMAGEINDEX  INT          NOT NULL," +
            "FLD_IMAGECOUNT  INT          NOT NULL," +
            "FLD_ITEMMEMO1\t CHAR(18)\t    NULL," +
            "FLD_ITEMMEMO2   BINARY(150)  NULL";
        Assert.Equal(expected, DataManageConst.g_sShopItemField);
    }

    /// <summary>计数取证：拼接后应含 7 个字段定义（6 个逗号分隔 + 末项无逗号）。</summary>
    [Fact]
    public void G_sShopItemField_HasSevenFieldDefinitions()
    {
        string s = DataManageConst.g_sShopItemField;
        Assert.Equal(6, s.Split(',').Length - 1);
        Assert.DoesNotContain(",", s.Substring(s.LastIndexOf("FLD_ITEMMEMO2", StringComparison.Ordinal)));
    }

    /// <summary>`FormatConnectionString` 只替换 `%s`，其余逐字不变。</summary>
    [Fact]
    public void FormatConnectionString_ReplacesOnlyThePlaceholder()
    {
        Assert.Equal(
            "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\\db\\shop.mdb;Persist Security Info=False",
            DataManageGlobals.FormatConnectionString("D:\\db\\shop.mdb"));
    }
}

// ===========================================================================
// 二、TAccessEngine 构造 / 析构（DataManage.pas:74-128）
// ===========================================================================
public class Sweep9DataLayerAccessEngineCreateTests
{
    private static (FakeAdoConnection Conn, FakeAdoQuery Qry, List<string> AccessCalls) WireFakes()
    {
        Sweep9DataLayerTestKit.Isolate();
        var conn = new FakeAdoConnection();
        var qry = new FakeAdoQuery();
        var calls = new List<string>();

        DataManageAccessSeam.CreateAdoConnection = () => conn;
        DataManageAccessSeam.CreateAdoQuery = () => qry;
        DataManageAccessSeam.CreateAccessDB = (f, b) => { calls.Add($"CreateAccessDB({f},{b})"); return true; };
        DataManageAccessSeam.TableExists = (f, t) => { calls.Add($"TableExists({f},{t})"); return true; };
        DataManageAccessSeam.AccessCreateTable = (f, t, s) => calls.Add($"AccessCreateTable({f},{t})");
        DataManageAccessSeam.CreateAccessIndex = (f, t, i, c, u, p) => calls.Add($"CreateAccessIndex({f},{t},{i},{c},{u},{p})");
        DataManageAccessSeam.GetTableList = (f, list) => { calls.Add($"GetTableList({f})"); list.Add("TBL_A"); list.Add("TBL_B"); };
        DataManageAccessSeam.CoInitialize = () => calls.Add("CoInitialize");
        DataManageAccessSeam.CoUnInitialize = () => calls.Add("CoUnInitialize");
        return (conn, qry, calls);
    }

    [Fact]
    public void Ctor_CallsCoInitializeFirst()
    {
        var (_, _, calls) = WireFakes();
        _ = new TAccessEngine("x.mdb");
        Assert.Equal("CoInitialize", calls[0]);
    }

    [Fact]
    public void Ctor_StoresFileNameAndOverwritesUnitGlobals()
    {
        var (conn, qry, _) = WireFakes();
        var engine = new TAccessEngine("shop.mdb");

        Assert.Equal("shop.mdb", engine.m_sFileName);
        Assert.Same(conn, DataManageGlobals.ADOConnection);
        Assert.Same(qry, DataManageGlobals.DBQry);
    }

    /// <summary>
    /// 原文 :87-89 的**空 then 块**：`CreateAccessDB` 返回 false 也完全不影响流程。差异断言。
    /// </summary>
    [Fact]
    public void Ctor_CreateAccessDbResultIsDiscarded_EmptyThenBlock_OriginalFlaw()
    {
        Sweep9DataLayerTestKit.Isolate();
        var calls = new List<string>();
        DataManageAccessSeam.CreateAdoConnection = () => new FakeAdoConnection();
        DataManageAccessSeam.CreateAdoQuery = () => new FakeAdoQuery();
        DataManageAccessSeam.CreateAccessDB = (_, _) => { calls.Add("CreateAccessDB"); return false; };
        DataManageAccessSeam.TableExists = (_, _) => { calls.Add("TableExists"); return true; };
        DataManageAccessSeam.GetTableList = (_, _) => calls.Add("GetTableList");

        _ = new TAccessEngine("x.mdb");

        // 建库失败（false）后流程照常继续到 TableExists / GetTableList
        Assert.Equal(new[] { "CreateAccessDB", "TableExists", "GetTableList" }, calls);
    }

    /// <summary>表不存在 ⇒ 建表 + 建索引（原文 :91-94）。</summary>
    [Fact]
    public void Ctor_WhenTableMissing_CreatesTableAndIndex()
    {
        var (_, _, calls) = WireFakes();
        DataManageAccessSeam.TableExists = (_, _) => false;

        _ = new TAccessEngine("x.mdb");

        Assert.Contains("AccessCreateTable(x.mdb,TBL_SHOPITEM)", calls);
        Assert.Contains("CreateAccessIndex(x.mdb,TBL_SHOPITEM,iFLD_ITEMNAME,FLD_ITEMNAME,True,True)", calls);
    }

    /// <summary>表已存在 ⇒ **不**建表、**不**建索引。</summary>
    [Fact]
    public void Ctor_WhenTableExists_DoesNotCreateTable()
    {
        var (_, _, calls) = WireFakes();
        _ = new TAccessEngine("x.mdb");

        Assert.DoesNotContain(calls, c => c.StartsWith("AccessCreateTable", StringComparison.Ordinal));
        Assert.DoesNotContain(calls, c => c.StartsWith("CreateAccessIndex", StringComparison.Ordinal));
    }

    /// <summary>
    /// `GetTableList` 写入的表名会被逐个包成 `TAccessTable` 存进 `TableList.Objects`
    /// （原文 :97-100），且名字顺序不变。
    /// </summary>
    [Fact]
    public void Ctor_WrapsEveryTableNameIntoAnAccessTable()
    {
        WireFakes();
        var engine = new TAccessEngine("x.mdb");

        Assert.Equal(2, engine.TableList.Count);
        Assert.Equal("TBL_A", engine.TableList[0]);
        Assert.Equal("TBL_B", engine.TableList[1]);

        var t0 = Assert.IsType<TAccessTable>(engine.TableList.GetObject(0));
        var t1 = Assert.IsType<TAccessTable>(engine.TableList.GetObject(1));
        Assert.Equal("TBL_A", t0.m_sTable);
        Assert.Equal("TBL_B", t1.m_sTable);
    }

    /// <summary>无表时 `TableList` 为空，循环体 0 次。</summary>
    [Fact]
    public void Ctor_WithNoTables_LeavesTableListEmpty()
    {
        WireFakes();
        DataManageAccessSeam.GetTableList = (_, _) => { };

        var engine = new TAccessEngine("x.mdb");
        Assert.Equal(0, engine.TableList.Count);
    }

    [Fact]
    public void Ctor_SetsConnectionStringLoginPromptKeepConnectionAndQueryWiring()
    {
        var (conn, qry, _) = WireFakes();
        _ = new TAccessEngine("shop.mdb");

        Assert.Equal(
            "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=shop.mdb;Persist Security Info=False",
            conn.ConnectionString);
        Assert.False(conn.LoginPrompt);
        Assert.True(conn.KeepConnection);
        Assert.Same(conn, qry.Connection);
        Assert.True(qry.Prepared);
    }

    /// <summary>构造末尾要尝试 `Connected := True`（原文 :109）。</summary>
    [Fact]
    public void Ctor_AttemptsToConnect()
    {
        var (conn, _, _) = WireFakes();
        _ = new TAccessEngine("shop.mdb");
        Assert.Equal(new[] { true }, conn.ConnectedWrites);
    }

    /// <summary>
    /// 原文 :108-112 的 `try/except`：连接抛异常 ⇒ 写
    /// `'[Exception] TAccessEngine ADOConnection:Connected'` 且**不冒泡**。
    /// </summary>
    [Fact]
    public void Ctor_ConnectThrows_LogsExactMessageAndDoesNotRethrow()
    {
        var (conn, _, _) = WireFakes();
        conn.ThrowOnConnect = true;
        Sweep9DataLayerSeam.LoggedMessages.Clear();

        var ex = Record.Exception(() => new TAccessEngine("x.mdb"));

        Assert.Null(ex);
        Assert.Contains("[Exception] TAccessEngine ADOConnection:Connected", Sweep9DataLayerSeam.LoggedMessages);
    }

    /// <summary>
    /// 原文缺陷（差异断言）：:101-106 的"设连接串/接查询"在 `try` **之外**
    /// ⇒ 那里抛异常会**直接冒泡**（与 :108-112 吞异常不同）。
    /// </summary>
    [Fact]
    public void Ctor_ConnectionStringThrows_PropagatesBecauseItIsOutsideTry_OriginalFlaw()
    {
        Sweep9DataLayerTestKit.Isolate();
        var conn = new FakeAdoConnection { ThrowOnConnectionString = true };
        DataManageAccessSeam.CreateAdoConnection = () => conn;
        DataManageAccessSeam.CreateAdoQuery = () => new FakeAdoQuery();
        DataManageAccessSeam.GetTableList = (_, _) => { };

        Assert.Throws<InvalidOperationException>(() => new TAccessEngine("x.mdb"));
    }

    /// <summary>
    /// 原文缺陷（差异断言）：第二次构造会**无条件覆写**单元级全局，
    /// 且**不释放**第一次的对象；也不写 `AccessEngine := Self`。
    /// </summary>
    [Fact]
    public void Ctor_SecondInstanceOverwritesGlobalsAndNeverAssignsAccessEngine_OriginalFlaw()
    {
        Sweep9DataLayerTestKit.Isolate();
        var first = new FakeAdoConnection();
        var second = new FakeAdoConnection();
        var q1 = new FakeAdoQuery();
        var q2 = new FakeAdoQuery();
        int n = 0;
        DataManageAccessSeam.CreateAdoConnection = () => (n++ == 0) ? first : second;
        DataManageAccessSeam.CreateAdoQuery = () => (n == 1) ? q1 : q2;
        DataManageAccessSeam.GetTableList = (_, _) => { };

        _ = new TAccessEngine("a.mdb");
        Assert.Same(first, DataManageGlobals.ADOConnection);

        _ = new TAccessEngine("b.mdb");
        Assert.Same(second, DataManageGlobals.ADOConnection);   // 被覆写

        // `AccessEngine` 在原文全文无赋值 ⇒ 保持 null
        Assert.Null(DataManageGlobals.AccessEngine);
    }

    /// <summary>
    /// 析构：遍历表 →（原文 Free 列表与两个全局）→ `CoUnInitialize`；
    /// 且**不清空**全局指针（原文缺陷）。
    /// </summary>
    [Fact]
    public void Destroy_CoUnInitializesAndLeavesGlobalsPointingAtOldObjects_OriginalFlaw()
    {
        var (conn, qry, calls) = WireFakes();
        var engine = new TAccessEngine("x.mdb");
        calls.Clear();

        engine.Destroy();

        Assert.Contains("CoUnInitialize", calls);
        Assert.Same(conn, DataManageGlobals.ADOConnection);
        Assert.Same(qry, DataManageGlobals.DBQry);
    }

    /// <summary>空表析构不抛异常。</summary>
    [Fact]
    public void Destroy_OnEmptyTableList_DoesNotThrow()
    {
        WireFakes();
        DataManageAccessSeam.GetTableList = (_, _) => { };
        var engine = new TAccessEngine("x.mdb");
        engine.Destroy();
    }
}

// ===========================================================================
// 三、TAccessEngine 锁（DataManage.pas:130-138）
// ===========================================================================
public class Sweep9DataLayerAccessEngineLockTests
{
    [Fact]
    public void Lock_ThenUnLock_IsReentrantAndBalanced()
    {
        Sweep9DataLayerTestKit.Isolate();
        var engine = new TAccessEngine("x.mdb");

        engine.Lock();
        engine.Lock();      // Win32 临界区可重入 ⇒ 托管 Monitor 同样可重入
        engine.UnLock();
        engine.UnLock();

        // 能再次进入说明计数已平衡
        engine.Lock();
        engine.UnLock();
    }
}

// ===========================================================================
// 四、TAccessEngine.LoadTable / CreateTable（DataManage.pas:140-153）
// ===========================================================================
public class Sweep9DataLayerAccessEngineTableTests
{
    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public void LoadTable_ReturnsTableExistsResult(bool exists, bool expected)
    {
        Sweep9DataLayerTestKit.Isolate();
        DataManageAccessSeam.TableExists = (_, _) => exists;
        var engine = new TAccessEngine("x.mdb");

        Assert.Equal(expected, engine.LoadTable("T"));
    }

    [Fact]
    public void CreateTable_WhenMissing_PassesEngineFileNameNotParameter()
    {
        Sweep9DataLayerTestKit.Isolate();
        var seen = new List<string>();
        DataManageAccessSeam.TableExists = (_, _) => false;
        DataManageAccessSeam.AccessCreateTable = (f, t, s) => seen.Add($"{f}|{t}|{s}");
        DataManageAccessSeam.GetTableList = (_, _) => { };
        var engine = new TAccessEngine("engine.mdb");
        seen.Clear();   // 丢掉构造期自动建 `TBL_SHOPITEM` 的那一条（原文 :92）

        engine.CreateTable("T1", "DDL-1");

        Assert.Equal(new[] { "engine.mdb|T1|DDL-1" }, seen);
    }

    [Fact]
    public void CreateTable_WhenExists_DoesNothing()
    {
        Sweep9DataLayerTestKit.Isolate();
        int n = 0;
        DataManageAccessSeam.TableExists = (_, _) => true;
        DataManageAccessSeam.AccessCreateTable = (_, _, _) => n++;
        DataManageAccessSeam.GetTableList = (_, _) => { };
        var engine = new TAccessEngine("x.mdb");

        engine.CreateTable("T1", "DDL-1");

        Assert.Equal(0, n);
    }
}

// ===========================================================================
// 五、TAccessEngine.Connect / DisConnect（DataManage.pas:155-179）
// ===========================================================================
public class Sweep9DataLayerAccessEngineConnectTests
{
    private static (TAccessEngine Engine, FakeAdoConnection Conn, FakeAdoQuery Qry) Wire(bool connected)
    {
        Sweep9DataLayerTestKit.Isolate();
        var conn = new FakeAdoConnection();
        var qry = new FakeAdoQuery();
        DataManageAccessSeam.CreateAdoConnection = () => conn;
        DataManageAccessSeam.CreateAdoQuery = () => qry;
        DataManageAccessSeam.GetTableList = (_, _) => { };
        var engine = new TAccessEngine("x.mdb");
        conn.Connected = connected;
        conn.ConnectedWrites.Clear();
        conn.KeepConnectionWrites.Clear();   // 构造期 :103 写过一次 True，这里清掉
        qry.Prepared = connected;
        return (engine, conn, qry);
    }

    /// <summary>已连接 ⇒ 直接返回 True，且**不进**重连分支（不写任何属性）。</summary>
    [Fact]
    public void Connect_WhenAlreadyConnected_ReturnsTrueWithoutRewiring()
    {
        var (engine, conn, qry) = Wire(true);

        Assert.True(engine.Connect());
        Assert.Empty(conn.ConnectedWrites);
        Assert.Equal(0, qry.Calls.Count);
    }

    /// <summary>未连接 ⇒ 重设连接串/LoginPrompt/KeepConnection + DBQry 接线，再连接并返回 True。</summary>
    [Fact]
    public void Connect_WhenDisconnected_RewiresAndConnects()
    {
        var (engine, conn, qry) = Wire(false);

        Assert.True(engine.Connect());
        Assert.Equal(
            "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=x.mdb;Persist Security Info=False",
            conn.ConnectionString);
        Assert.False(conn.LoginPrompt);
        Assert.True(conn.KeepConnection);
        Assert.Same(conn, qry.Connection);
        Assert.True(qry.Prepared);
        Assert.Equal(new[] { true }, conn.ConnectedWrites);
    }

    /// <summary>
    /// 原文 :169-170 的 `except` 是**空块** ⇒ 连接失败时 `Result` 保持 :157 读到的旧值 False，
    /// 且**不写任何日志**（与构造函数的 except 不同 —— 差异断言）。
    /// </summary>
    [Fact]
    public void Connect_WhenConnectThrows_ReturnsFalseAndLogsNothing_OriginalFlaw()
    {
        var (engine, conn, _) = Wire(false);
        conn.ThrowOnConnect = true;
        Sweep9DataLayerSeam.LoggedMessages.Clear();

        Assert.False(engine.Connect());
        Assert.Empty(Sweep9DataLayerSeam.LoggedMessages);
    }

    /// <summary>
    /// 原文 :176-178：`DisConnect` 依次写 `Connected=False`、`KeepConnection=False`、
    /// `DBQry.Prepared=False`。断言**末值**与**写入序列**（`Wire` 已清空构造期的写入记录，
    /// 故这里应恰好只剩断开这一次）。
    /// </summary>
    [Fact]
    public void DisConnect_ClearsThreeFlagsInOriginalOrder()
    {
        var (engine, conn, qry) = Wire(true);

        engine.DisConnect();

        Assert.False(conn.Connected);
        Assert.False(conn.KeepConnection);
        Assert.False(qry.Prepared);
        Assert.Equal(new[] { false }, conn.ConnectedWrites);
        Assert.Equal(new[] { false }, conn.KeepConnectionWrites);
    }
}

// ===========================================================================
// 六、TAccessEngine.GetTable / SetTable / 索引器（DataManage.pas:181-205、:29）
// ===========================================================================
public class Sweep9DataLayerAccessEngineTableLookupTests
{
    private static TAccessEngine Make(out TStringList list)
    {
        Sweep9DataLayerTestKit.Isolate();
        DataManageAccessSeam.GetTableList = (_, l) => { l.Add("TBL_A"); l.Add("TBL_B"); };
        var engine = new TAccessEngine("x.mdb");
        list = engine.TableList;
        return engine;
    }

    [Fact]
    public void GetTable_ReturnsWrappedInstanceByName()
    {
        var engine = Make(out TStringList list);

        var a = engine.GetTable("TBL_A");
        Assert.NotNull(a);
        Assert.Same(list.GetObject(0), a);
        Assert.Equal("TBL_A", a!.m_sTable);
    }

    /// <summary>名字比较是**大小写敏感**（Delphi `=` 字符串比较）。差异断言。</summary>
    [Fact]
    public void GetTable_IsCaseSensitive_OriginalSemantics()
    {
        var engine = Make(out _);
        Assert.Null(engine.GetTable("tbl_a"));
        Assert.NotNull(engine.GetTable("TBL_A"));
    }

    [Fact]
    public void GetTable_MissingName_ReturnsNull()
    {
        var engine = Make(out _);
        Assert.Null(engine.GetTable("NOPE"));
    }

    /// <summary>`SetTable` 命中 ⇒ 覆盖 `Objects[I]`（原文会先 `Free` 旧值，托管侧交给 GC）。</summary>
    [Fact]
    public void SetTable_ReplacesObjectAtMatchingName()
    {
        var engine = Make(out TStringList list);
        var replacement = new TAccessTable("TBL_A");

        engine.SetTable("TBL_A", replacement);

        Assert.Same(replacement, list.GetObject(0));
        Assert.Same(replacement, engine.GetTable("TBL_A"));
    }

    /// <summary>
    /// `SetTable` **未命中则静默无操作**（原文不追加新行）—— 差异断言。
    /// </summary>
    [Fact]
    public void SetTable_MissingName_IsSilentlyIgnored_OriginalFlaw()
    {
        var engine = Make(out TStringList list);
        int before = list.Count;

        engine.SetTable("NOPE", new TAccessTable("NOPE"));

        Assert.Equal(before, list.Count);
        Assert.Null(engine.GetTable("NOPE"));
    }

    /// <summary>`SetTable(name, null)` 会把槽位写成 null（原文不判空）。</summary>
    [Fact]
    public void SetTable_NullValue_WritesNullSlot_OriginalDoesNotGuard()
    {
        var engine = Make(out TStringList list);

        engine.SetTable("TBL_B", null);

        Assert.Null(list.GetObject(1));
        Assert.Null(engine.GetTable("TBL_B"));
    }

    [Fact]
    public void Indexer_ReadsAndWritesThroughSameLogic()
    {
        var engine = Make(out TStringList list);

        Assert.Same(list.GetObject(0), engine["TBL_A"]);

        var replacement = new TAccessTable("TBL_A");
        engine["TBL_A"] = replacement;
        Assert.Same(replacement, engine["TBL_A"]);
    }

    /// <summary>
    /// 原文缺陷（差异断言）：`AccessTableList`（:13）在全文**零引用**
    /// （既无 `SetLength` 也无下标读写）⇒ 托管侧同样恒为空、永不参与逻辑。
    /// 计数取证：构造含 2 张表的引擎后仍为 0。
    /// </summary>
    [Fact]
    public void AccessTableList_IsNeverPopulated_ZeroReferencesInOriginal()
    {
        var engine = Make(out TStringList list);

        Assert.Equal(0, engine.AccessTableList.Count);
        Assert.Equal(2, list.Count);   // 而 TableList 有 2 —— 两者互不相干
    }
}

// ===========================================================================
// 七、TAccessTable（DataManage.pas:208-275）
// ===========================================================================
public class Sweep9DataLayerAccessTableTests
{
    private static (TAccessTable Table, FakeAdoQuery Qry) MakePair()
    {
        Sweep9DataLayerTestKit.Isolate();
        var qry = new FakeAdoQuery();
        DataManageGlobals.DBQry = qry;
        return (new TAccessTable("TBL_X"), qry);
    }

    [Fact]
    public void Ctor_StoresTableName()
    {
        var (table, _) = MakePair();
        Assert.Equal("TBL_X", table.m_sTable);
    }

    [Fact]
    public void Destroy_IsEmptyAndDoesNotThrow()
    {
        var (table, _) = MakePair();
        table.Destroy();
        Assert.NotNull(DataManageGlobals.DBQry);   // 原文空析构，不释放 DBQry
    }

    /// <summary>
    /// `Count` 读的是**全局 DBQry** 的 `RecordCount`（原文 :222），
    /// **不是**表 `m_sTable` 的行数 —— 差异断言（改名/换表都不会改变它）。
    /// </summary>
    [Fact]
    public void Count_ReadsGlobalQueryRecordCount_NotTheNamedTable()
    {
        var (table, qry) = MakePair();
        qry.RecordCount = 42;

        Assert.Equal(42, table.Count);
        Assert.Equal(42, new TAccessTable("别的表").Count);   // 同一个全局
    }

    [Fact]
    public void AdoQuery_ExposesGlobalQueryInstance()
    {
        var (table, qry) = MakePair();
        Assert.Same(qry, table.ADOQuery);
    }

    [Fact]
    public void GetCountValue_MirrorsCount()
    {
        var (table, qry) = MakePair();
        qry.RecordCount = 7;
        Assert.Equal(7, table.GetCountValue);
    }

    /// <summary>`ClearSQL` → `DBQry.SQL.Clear`（原文 :240）。</summary>
    [Fact]
    public void ClearSQL_ClearsGlobalSqlBuffer()
    {
        var (table, qry) = MakePair();
        qry.SQL.Add("SELECT 1");

        table.ClearSQL();

        Assert.Equal(0, qry.SQL.Count);
    }

    /// <summary>`AddSQL` → `DBQry.SQL.Add`（原文 :245），**累积**多行且顺序保留。</summary>
    [Fact]
    public void AddSQL_AppendsLinesInOrder()
    {
        var (table, qry) = MakePair();

        table.AddSQL("SELECT *");
        table.AddSQL("FROM T");

        Assert.Equal(2, qry.SQL.Count);
        Assert.Equal("SELECT *", qry.SQL[0]);
        Assert.Equal("FROM T", qry.SQL[1]);
    }

    [Fact]
    public void OpenNextCloseExecSql_ForwardToGlobalQueryInOrder()
    {
        var (table, qry) = MakePair();

        table.OpenSQL();
        table.NextSQL();
        table.CloseSQL();
        table.ExecSQL();

        Assert.Equal(new[] { "Open", "Next", "Close", "ExecSQL" }, qry.Calls);
    }

    [Fact]
    public void GetField_ForwardsToFieldByName()
    {
        var (table, qry) = MakePair();

        table.GetField("FLD_ITEMNAME");

        Assert.Equal(new[] { "FieldByName(FLD_ITEMNAME)" }, qry.Calls);
    }

    /// <summary>字段不存在 ⇒ 返回 null（原文不判空，交由调用方解引用）。</summary>
    [Fact]
    public void GetField_MissingField_ReturnsNull()
    {
        var (table, _) = MakePair();
        Assert.Null(table.GetField("NOPE"));
    }

    [Fact]
    public void GetParameters_ForwardsToGlobalQuery()
    {
        var (table, qry) = MakePair();
        Assert.Same(qry.Parameters, table.GetParameters);
        Assert.Same(qry.Parameters, table.Parameters);
    }

    [Fact]
    public void Indexer_ByName_ForwardsToGetField()
    {
        var (table, qry) = MakePair();
        _ = table["FLD_A"];
        Assert.Equal(new[] { "FieldByName(FLD_A)" }, qry.Calls);
    }

    /// <summary>
    /// 原文缺陷（差异断言）：`TAccessTable.Lock`/`UnLock` 转调**单元级** `AccessEngine`，
    /// 而 `DataManage.pas` 全文**没有** `AccessEngine := ...` ⇒ 原文此处必 AV。
    /// 托管侧保留"未赋值即 null"并抛 <see cref="NullReferenceException"/>，**不静默兜底**。
    /// </summary>
    [Fact]
    public void Lock_WhenAccessEngineUnassigned_Throws_OriginalFlaw()
    {
        var (table, _) = MakePair();
        DataManageGlobals.AccessEngine = null;

        Assert.Throws<NullReferenceException>(() => table.Lock());
        Assert.Throws<NullReferenceException>(() => table.UnLock());
    }

    /// <summary>对照组：显式装配 `AccessEngine` 后 Lock/UnLock 正常转调。</summary>
    [Fact]
    public void Lock_WhenAccessEngineAssigned_Forwards()
    {
        var (table, _) = MakePair();
        DataManageAccessSeam.GetTableList = (_, _) => { };
        DataManageAccessSeam.CreateAdoConnection = () => new FakeAdoConnection();
        DataManageAccessSeam.CreateAdoQuery = () => new FakeAdoQuery();
        var engine = new TAccessEngine("x.mdb");
        DataManageGlobals.AccessEngine = engine;

        table.Lock();
        table.UnLock();
    }

    /// <summary>
    /// 全局 `DBQry` 为 null 时，全部转发方法**静默无操作**（原文会 AV；托管侧取"沉默"路径，
    /// 已登记 D-P9-04）。计数取证：无异常、无副作用。
    /// </summary>
    [Fact]
    public void AllForwarders_WhenGlobalQueryIsNull_AreSilentNoOps()
    {
        Sweep9DataLayerTestKit.Isolate();
        DataManageGlobals.DBQry = null;
        var table = new TAccessTable("T");

        Assert.Equal(0, table.Count);
        Assert.Null(table.ADOQuery);
        Assert.Null(table.GetField("F"));
        Assert.Null(table.GetParameters);

        var ex = Record.Exception(() =>
        {
            table.ClearSQL();
            table.AddSQL("X");
            table.OpenSQL();
            table.NextSQL();
            table.CloseSQL();
            table.ExecSQL();
        });
        Assert.Null(ex);
    }
}
