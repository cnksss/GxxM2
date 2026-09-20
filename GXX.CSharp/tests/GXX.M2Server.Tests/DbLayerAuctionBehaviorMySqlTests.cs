// 源单元：Source/M2Engine/MySqlAuctionDB.pas（1-1583 行）
//   ＋ 姊妹单元 Source/M2Engine/SqliteAuctionDB.pas（「双方言对账」测试组用）
//   ＋ public 包装层 Source/M2Engine/M2DataCommon.pas 的 TAuctionDB（C# 落点 DbBases.cs:640-1039）
//
// 行为保真测试（**不连真库**，全部注入 DbLayerTestKit 的内存接缝）。
// **期望值全部来自原文**，注释标注 `MySqlAuctionDB.pas:<行>`；SQL 文本只在必要时逐字写死。
//
// 覆盖：22 个 public 方法各 ≥3 个用例（空结果集 / 0 / 负数 / 超界页码 / itemColors 位组合 /
//       sortField 越界 / 语句抛错 / SQL 注入字符 / 事务序列 / 绑定参数顺序 / 结果集列序 /
//       itemList 记录数 / RowCount 判据），外加末尾「双方言对账」测试组。
//
// MySQL 方言专有点逐条锁死（**不**与姊妹 SQLite 版"顺手统一"）：
//   * 结果集：`if (sm.Query() && sm.Fetch())` / `while (sm.Fetch())`（SQLite：`Step() == SQLITE_ROW`）；
//   * 无结果集成功判据：`Step()` 返回 **bool**（SQLite：`Step() in [SQLITE_OK, SQLITE_DONE]`）；
//   * 存在性判据：`DoAddAttentionItem` / `DoDeleteAttentionItem` 用 **`RowCount != 0`**
//     （MySqlAuctionDB.pas:642/678，原文注释保留了早先的 `IsAttentioned := ...Step` 写法）；
//   * 绑定：`OrderBindParamText/Int/Bool/DateTime/ParamDouble`（SQLite：`OrderBindInt/Bool/Text/Double`）；
//   * 事务：`StartTransaction` vs SQLite 的 `BeginTransaction`；脚本：`Exec` vs `Execute`；
//     MySQL **独有** `ClearResult`（MySqlAuctionDB.pas:617）；
//   * `AuctionData.AddDateTime` 是 DATETIME，用 `OrderGetColumnValueDateTime`
//     （SQLite 用 `OrderGetColumnValueInt` + `DelphiDateUtil.UnixToDateTime`）；
//   * `DoQueryAllItems` / `DoGetAllItemsPageCount` 的 finally **只** `sm.Reset()`（MySqlAuctionDB.pas:885/1154），
//     **不**调 `Finalize`（SQLite 版两处都调 `sm.StatementFinalize()`，见 SqliteAuctionDB.cs:906-908）。
//
// 夹具说明：DbLayerTestKit 的 FakeMySqlDatabase 不带"抛错/记录 ClearResult"钩子（该文件只读），
// 故这里用 HookMySqlDatabase 包一层 —— AddSQLStatement 转给 FakeMySqlDatabase（保留按名复用与全部观测），
// 但可让指定 label 的语句在 Query/Fetch/Step 抛异常、可让 Exec 抛异常、并记录 ClearResult 调用次数。

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using GXX.Core.Protocol;
using GXX.M2Server.DbLayer;

namespace GXX.M2Server.Tests;

public class DbLayerAuctionBehaviorMySqlTests
{
    // ================================================================== 测试夹具

    /// <summary>包一层 FakeMySqlDatabase：记录 ClearResult 次数，并可注入抛错。</summary>
    private sealed class HookMySqlDatabase : IMySqlDatabase
    {
        private readonly FakeMySqlDatabase _inner;
        private readonly Dictionary<string, Exception> _throws;

        public Exception? ExecuteThrow { get; set; }
        public int ClearResultCount;

        public HookMySqlDatabase(FakeMySqlDatabase inner, Dictionary<string, Exception>? throws)
        {
            _inner = inner;
            _throws = throws;
        }

        public IMySqlStatement AddSQLStatement(string name)
            => _throws != null && _throws.TryGetValue(name, out Exception e)
                ? new ThrowingMySqlStatement(_inner.AddSQLStatement(name), e)
                : _inner.AddSQLStatement(name);

        public void StartTransaction() => _inner.StartTransaction();
        public void Commit() => _inner.Commit();
        public void RollBack() => _inner.RollBack();

        public void Exec(string sql)
        {
            if (ExecuteThrow != null) throw ExecuteThrow;
            _inner.Exec(sql);
        }

        public void ClearResult()
        {
            ClearResultCount++;
            _inner.ClearResult();
        }

        public void Connect(string server, string user, string password, string database, ushort port, uint flags)
            => _inner.Connect(server, user, password, database, port, flags);

        public void Init() => _inner.Init();
        public void ClearStatements() => _inner.ClearStatements();
        public string CharacterSetName { get => _inner.CharacterSetName; set => _inner.CharacterSetName = value; }
        public object MySql => _inner.MySql;
        public event Action OnRequest;
    }

    /// <summary>观测仍在 Fake 语句上，但 Query/Fetch/Step 抛指定异常（模拟 MySQL driver 失败）。</summary>
    private sealed class ThrowingMySqlStatement : IMySqlStatement
    {
        private readonly IMySqlStatement _inner;
        private readonly Exception _boom;

        public ThrowingMySqlStatement(IMySqlStatement inner, Exception boom)
        {
            _inner = inner;
            _boom = boom;
        }

        public string Sql { get => _inner.Sql; set => _inner.Sql = value; }
        public void Prepare() => _inner.Prepare();
        public void StatementFinalize() => _inner.StatementFinalize();
        public void Reset() => _inner.Reset();
        public void OrderBindParamInt(int value) => _inner.OrderBindParamInt(value);
        public void OrderBindParamBool(bool value) => _inner.OrderBindParamBool(value);
        public void OrderBindParamText(string value) => _inner.OrderBindParamText(value);
        public void OrderBindParamDateTime(DateTime value) => _inner.OrderBindParamDateTime(value);
        public void OrderBindParamDouble(double value) => _inner.OrderBindParamDouble(value);
        public bool Query() => throw _boom;
        public bool Fetch() => throw _boom;
        public bool Step() => throw _boom;
        public int RowCount => _inner.RowCount;
        public int OrderGetColumnValueInt => _inner.OrderGetColumnValueInt;
        public long OrderGetColumnValueInt64 => _inner.OrderGetColumnValueInt64;
        public bool OrderGetColumnValueBool => _inner.OrderGetColumnValueBool;
        public string OrderGetColumnValueText => _inner.OrderGetColumnValueText;
        public double OrderGetColumnValueDouble => _inner.OrderGetColumnValueDouble;
        public DateTime OrderGetColumnValueDateTime => _inner.OrderGetColumnValueDateTime;
    }

    /// <summary>SQLite 侧的对账钩子（Step 抛异常用；对账测试需要两侧都能"抛错"）。</summary>
    private sealed class HookSqliteDatabase : ISqliteDatabase
    {
        private readonly FakeSqliteDatabase _inner;
        private readonly Dictionary<string, Exception> _throws;

        public Exception? ExecuteThrow { get; set; }

        public HookSqliteDatabase(FakeSqliteDatabase inner, Dictionary<string, Exception>? throws)
        {
            _inner = inner;
            _throws = throws;
        }

        public ISqliteStatement AddSQLStatement(string name)
            => _throws != null && _throws.TryGetValue(name, out Exception e)
                ? new ThrowingSqliteStatement(_inner.AddSQLStatement(name), e)
                : _inner.AddSQLStatement(name);

        public void BeginTransaction() => _inner.BeginTransaction();
        public void Commit() => _inner.Commit();
        public void RollBack() => _inner.RollBack();

        public void Execute(string sql)
        {
            if (ExecuteThrow != null) throw ExecuteThrow;
            _inner.Execute(sql);
        }

        public bool Connected { get => _inner.Connected; set => _inner.Connected = value; }
        public string Database { get => _inner.Database; set => _inner.Database = value; }
        public bool MustExist { get => _inner.MustExist; set => _inner.MustExist = value; }
        public bool UseThreadMode { get => _inner.UseThreadMode; set => _inner.UseThreadMode = value; }
        public int ErrorCode => _inner.ErrorCode;
    }

    private sealed class ThrowingSqliteStatement : ISqliteStatement
    {
        private readonly ISqliteStatement _inner;
        private readonly Exception _boom;

        public ThrowingSqliteStatement(ISqliteStatement inner, Exception boom)
        {
            _inner = inner;
            _boom = boom;
        }

        public string Sql { get => _inner.Sql; set => _inner.Sql = value; }
        public void Prepare() => _inner.Prepare();
        public void StatementFinalize() => _inner.StatementFinalize();
        public void Reset() => _inner.Reset();
        public void OrderBindInt(int value) => _inner.OrderBindInt(value);
        public void OrderBindInt64(long value) => _inner.OrderBindInt64(value);
        public void OrderBindBool(bool value) => _inner.OrderBindBool(value);
        public void OrderBindText(string value) => _inner.OrderBindText(value);
        public void OrderBindDouble(double value) => _inner.OrderBindDouble(value);
        public int Step() => throw _boom;
        public int OrderGetColumnValueInt => _inner.OrderGetColumnValueInt;
        public long OrderGetColumnValueInt64 => _inner.OrderGetColumnValueInt64;
        public bool OrderGetColumnValueBool => _inner.OrderGetColumnValueBool;
        public string OrderGetColumnValueText => _inner.OrderGetColumnValueText;
        public double OrderGetColumnValueDouble => _inner.OrderGetColumnValueDouble;
    }

    /// <summary>AddSQLStatement 正常、Prepare 抛异常的库（验 MySqlAuctionDB.pas:302-307 的 except）。</summary>
    private sealed class ThrowingPrepareMySqlDatabase : IMySqlDatabase
    {
        public IMySqlStatement AddSQLStatement(string name) => new ThrowingPrepareMySqlStatement();
        public void StartTransaction() { }
        public void Commit() { }
        public void RollBack() { }
        public void Exec(string sql) { }
        public void ClearResult() { }
        public void Connect(string server, string user, string password, string database, ushort port, uint flags) { }
        public void Init() { }
        public void ClearStatements() { }
        public string CharacterSetName { get; set; } = "";
        public object MySql => null;
        public event Action OnRequest;
    }

    private sealed class ThrowingPrepareMySqlStatement : IMySqlStatement
    {
        public string Sql { get; set; } = "";
        public void Prepare() => throw new InvalidOperationException("prepare-boom");
        public void StatementFinalize() { }
        public void Reset() { }
        public void OrderBindParamInt(int value) { }
        public void OrderBindParamBool(bool value) { }
        public void OrderBindParamText(string value) { }
        public void OrderBindParamDateTime(DateTime value) { }
        public void OrderBindParamDouble(double value) { }
        public bool Query() => false;
        public bool Fetch() => false;
        public bool Step() => false;
        public int RowCount => 0;
        public int OrderGetColumnValueInt => 0;
        public long OrderGetColumnValueInt64 => 0;
        public bool OrderGetColumnValueBool => false;
        public string OrderGetColumnValueText => "";
        public double OrderGetColumnValueDouble => 0;
        public DateTime OrderGetColumnValueDateTime => default;
    }

    /// <summary>可编排的 StdItem 视图（对应 MySqlAuctionDB.pas:459-517 用到的 TStdItem 字段）。</summary>
    private sealed class FakeAuctionStdItem : IAuctionStdItem
    {
        public int StdMode { get; set; }
        public int OverLap { get; set; }
        public int Shape { get; set; }
        public int Color { get; set; }
        public string DBName { get; set; } = "";
        public string Name { get; set; } = "";
        public int NeedIdentify { get; set; }
    }

    /// <summary>可编排的在线玩家视图（对应 MySqlAuctionDB.pas:1277-1326 用到的 TPlayObject 字段）。</summary>
    private sealed class FakeAuctionPlayer : IAuctionPlayer
    {
        public int m_nGameGold { get; set; }
        public int m_nGamePoint { get; set; }
        public int m_nGold { get; set; }
        public int m_nGameDiamond { get; set; }
        public int m_nGameGird { get; set; }
        public int m_nScriptGotoCount { get; set; }
        public string m_sAuctionItemName { get; set; } = "";
        public string m_sAuctionItemHumanName { get; set; } = "";
        public string m_sAuctionItemBidHumanName { get; set; } = "";
        public int m_nAuctionItemStartPrice { get; set; }
        public int m_nAuctionItemSellPrice { get; set; }
        public int m_nAuctionItemFinaPrice { get; set; }
        public int m_nAuctionItemInvalidPrice { get; set; }
        public int m_nAuctionItemMoneyType { get; set; }
        public bool m_boAuctionItemSelled { get; set; }
        public int GameGoldChangedCount;
        public int GoldChangedCount;
        public int NewGamePointChangedCount;
        public void GameGoldChanged() => GameGoldChangedCount++;
        public void GoldChanged() => GoldChangedCount++;
        public void NewGamePointChanged() => NewGamePointChangedCount++;
    }

    // ---------------------------------------------------------------- 夹具构造

    private sealed class MySqlFixture
    {
        public TMySqlAuctionDB Unit = null!;
        public FakeMySqlDatabase Db = null!;
        public FakeDbLayerHost Host = null!;
        public FakeDbLayerEnvironment Env = null!;
        public CapturingDbLayerLog Log = null!;
        public HookMySqlDatabase Hook = null!;
    }

    /// <summary>MySqlAuctionDB.pas:752/1079 方法内现建的两个临时语句 label（不在 DoInit 的 21 条里）。
    /// 必须先建出来，方法内 <c>_fdb.AddSQLStatement(label)</c> 才会按名复用返回**同一实例**
    /// （FakeMySqlDatabase.AddSQLStatement 同名复用），否则预置的结果集方法内看不到。</summary>
    private static readonly string[] TemporaryStatementLabels =
    {
        "Auction_QueryAllItems",   // 原文 752
        "Auction_GetAllItemsCount", // 原文 1079
    };

    /// <summary>MySqlAuctionDB.pas:180-272 DoInit 登记的 21 条语句 label，按原文顺序。</summary>
    private static readonly string[] DoInitStatementLabels =
    {
        "Auction_UpdateAuctionItemFail", "Auction_UpdateAuctionItemSuccess", "Auction_QueryAuctionItemSuccess",
        "Auction_QueryAuctionItems", "Auction_GetMyItemsCount", "Auction_QueryAttentionItems",
        "Auction_GetMyAttentionItemsCount", "Auction_QueryOneItem", "Auction_GetMyAuctioningItemsCount",
        "Auction_GetMySellFailItemsCount", "Auction_GetMyBuyOKItemsCount", "Auction_GetMaxAuctionID",
        "Auction_InsertAuctionItem", "Auction_JoinItemBid", "Auction_BuyItem", "Auction_AddAttentionItem",
        "Auction_CheckInAttentionItem", "Auction_DeleteAuctionItem", "Auction_HumanRename",
        "Auction_LastBidderRename", "Auction_AttentionRename",
    };

    /// <summary>建一个已完成 DoInit 的 MySQL 拍卖库（**显式注入 db**，原文 177-178 的 is 判定回退不参与）。</summary>
    private static MySqlFixture NewMySql(bool preRegisterTemporary = true,
        Dictionary<string, Exception>? throws = null, Exception? executeThrow = null)
    {
        var (env, log) = DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        // 抛错语句必须**先建出来**（ThrowingMySqlStatement 包的是 _inner.AddSQLStatement(label)）。
        if (throws != null) foreach (string label in throws.Keys) db.AddSQLStatement(label);
        if (preRegisterTemporary) foreach (string label in TemporaryStatementLabels) db.AddSQLStatement(label);
        var hook = new HookMySqlDatabase(db, throws) { ExecuteThrow = executeThrow };
        var host = new FakeDbLayerHost { DataBase = db };
        var unit = new TMySqlAuctionDB(host, hook);
        unit.DoInit();
        return new MySqlFixture { Unit = unit, Db = db, Host = host, Env = env, Log = log, Hook = hook };
    }

    /// <summary>只跑 DoInit、不预建临时语句（用于断言 DoInit 之后恰好 21 条语句）。</summary>
    private static MySqlFixture NewMySqlWithoutTemporary() => NewMySql(preRegisterTemporary: false);

    /// <summary>指定 label 的语句 Query/Fetch/Step 抛异常；<paramref name="executeThrow"/> 让 Exec 抛。</summary>
    private static MySqlFixture NewMySqlThrowing(string label, Exception boom, Exception? executeThrow = null)
        => NewMySql(throws: new Dictionary<string, Exception> { [label] = boom }, executeThrow: executeThrow);

    // ---------------------------------------------------------------- 行构造辅助

    /// <summary><c>Auction_QueryAllItems</c> 的 13 列（原文 754-758 的 select，末列 IsAttention）。
    /// 列序：AuctionID, HumanName, AddDateTime, AuctionTime, TimeLeft, StartingPrice, SellingPrice,
    ///       CurrencyType, LastBidPrice, LastBidder, TradingStatus, IsItemGive, IsAttention。
    /// <paramref name="addDate"/> 用 DateTime（MySQL DATETIME 语义）或 long/int（Unix 秒，对账测试给 SQLite 侧用）。</summary>
    private static object[] AllItemsRow(int auctionId, string human, object addDate, int auctionTime, int timeLeft,
        int starting, int selling, int currency, int lastBidPrice, string lastBidder, int tradingStatus,
        int isItemGive, int isAttention)
        => new object[] { auctionId, human, addDate, auctionTime, timeLeft, starting, selling, currency,
            lastBidPrice, lastBidder, tradingStatus, isItemGive, isAttention };

    /// <summary><c>Auction_QueryAuctionItems</c> 的 12 列（原文 195-198，首列 AuctionID，**无** IsAttention）。</summary>
    private static object[] MyItemsRow(int auctionId, string human, object addDate, int auctionTime, int timeLeft,
        int starting, int selling, int currency, int lastBidPrice, string lastBidder, int tradingStatus, int isItemGive)
        => new object[] { auctionId, human, addDate, auctionTime, timeLeft, starting, selling, currency,
            lastBidPrice, lastBidder, tradingStatus, isItemGive };

    /// <summary><c>Auction_QueryOneItem</c> 的 11 列（原文 215-218，**没有** AuctionID 列；
    /// AuctionID 来自方法入参，原文 1007/1042）。</summary>
    private static object[] OneItemRow(string human, object addDate, int auctionTime, int timeLeft,
        int starting, int selling, int currency, int lastBidPrice, string lastBidder, int tradingStatus, int isItemGive)
        => new object[] { human, addDate, auctionTime, timeLeft, starting, selling, currency,
            lastBidPrice, lastBidder, tradingStatus, isItemGive };

    /// <summary><c>Auction_QueryAuctionItemSuccess</c> 的 9 列（原文 188-191 的 select）。
    /// 列序：A.AuctionID, A.HumanName, A.CurrencyType, A.StartingPrice, A.SellingPrice,
    ///       A.LastBidder, A.LastBidPrice, B.DBIndex, B.MakeIndex。</summary>
    private static object[] SuccessRow(int auctionId, string human, int currency, int starting, int selling,
        string lastBidder, int lastBidPrice, int dbIndex, int makeIndex)
        => new object[] { auctionId, human, currency, starting, selling, lastBidder, lastBidPrice, dbIndex, makeIndex };

    /// <summary>原文 <c>AUCTION_ITEM_TYPE = 5</c>（M2DataCommon.pas:12）。</summary>
    private const int AuctionItemType = 5;

    /// <summary>可安全用于任何绑定位置的注入串（原文绑定参数都不拼进 SQL）。</summary>
    private const string Injection = "'; drop table Items; --";

    /// <summary>igAll = 0（Grobal2.Types5.cs TItemGroup 序数与原文字面一致）。</summary>
    private const int IgAll = 0;

    private static readonly Dictionary<string, int> ItemGroupOrdinal = new()
    {
        ["igAll"] = 0, ["igWeapon"] = 1, ["igDress"] = 2, ["igHelmet"] = 3, ["igNecklace"] = 4,
        ["igArmRing"] = 5, ["igRing"] = 6, ["igBelt"] = 7, ["igBoots"] = 8, ["igFashion"] = 9,
        ["igDrug"] = 10, ["igSpecial"] = 11, ["igOther"] = 12,
    };

    private static TUserItem MakeUserItem(int btValue13, string name, byte color)
    {
        var item = new TUserItem();
        M2ItemDbAccess.SetValue(ref item, 13, btValue13);
        item.NameStr = name;
        item.btColor = color;
        return item;
    }

    // ==================================================================
    // 1. Init —— 原文 173-308
    // ==================================================================

    /// <summary>MySqlAuctionDB.pas:173-308 —— 显式注入 db 时 DoInit 登记 21 条语句，顺序与原文 180-272 一致。</summary>
    [Fact]
    public void Init_RegistersTheTwentyOneStatementsInOriginalOrder()
    {
        var fx = NewMySqlWithoutTemporary();

        Assert.Equal(21, fx.Db.AddedNames.Count);
        Assert.Equal(DoInitStatementLabels, fx.Db.AddedNames.ToArray());
    }

    /// <summary>MySqlAuctionDB.pas:274-301 —— 21 条 Prepare 各一次（且整体包在 try/except 里）。</summary>
    [Fact]
    public void Init_PreparesEveryStatement()
    {
        var fx = NewMySqlWithoutTemporary();

        Assert.Equal(21, fx.Db.Created.Count);
        foreach (string label in DoInitStatementLabels) Assert.Equal(1, fx.Db.For(label).PrepareCount);
    }

    /// <summary>MySqlAuctionDB.pas:177-178 —— 显式注入优先于 Owner.DataBase 的 is 判定回退。</summary>
    [Fact]
    public void Init_DoesNotFallBackToOwnerDataBase_WhenDbInjectedExplicitly()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var injected = new FakeMySqlDatabase();
        var other = new FakeMySqlDatabase();
        var host = new FakeDbLayerHost { DataBase = other };
        var unit = new TMySqlAuctionDB(host, injected);

        unit.DoInit();

        Assert.Equal(21, injected.AddedNames.Count);
        Assert.Empty(other.AddedNames);
    }

    /// <summary>MySqlAuctionDB.pas:177-178 —— 未注入时按 <c>Owner.DataBase is IMySqlDatabase</c> 回退。</summary>
    [Fact]
    public void Init_FallsBackToOwnerDataBase_WhenNoDbInjected()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        var unit = new TMySqlAuctionDB(new FakeDbLayerHost { DataBase = db });

        unit.DoInit();

        Assert.Equal(21, db.AddedNames.Count);
    }

    /// <summary>未注入且宿主无库 → <c>_fdb</c> 仍为 null → 原文 180 的 FDB（nil）解引用即 AV；
    /// 托管侧表现为 NullReferenceException。
    /// 注意与姊妹 SQLite 版的**接缝差异**：TSqliteAuctionDB 的构造把未注入时设为
    /// UnavailableSqliteDatabase.Instance（SqliteAuctionDB.cs:128），于是抛 NotSupportedException；
    /// TMySqlAuctionDB 的构造是 `_fdb = db;`（MySqlAuctionDB.cs:135），保持"nil 即 AV"的原文语义。</summary>
    [Fact]
    public void Init_WithoutAnyDatabase_DereferencesTheNullSeam()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var unit = new TMySqlAuctionDB(new FakeDbLayerHost());

        Assert.Throws<NullReferenceException>(() => unit.DoInit());
    }

    /// <summary>MySqlAuctionDB.pas:302-307 —— Prepare 抛异常被 except 吞掉并写 MainOutMessage。</summary>
    [Fact]
    public void Init_SwallowsPrepareExceptionsAndLogsThem()
    {
        var (_, log) = DbLayerTestKit.Isolate();
        var unit = new TMySqlAuctionDB(new FakeDbLayerHost { DataBase = new ThrowingPrepareMySqlDatabase() });

        unit.DoInit();

        Assert.Contains("prepare-boom", log.Messages);
    }

    // ==================================================================
    // 2. Final —— 原文 310-439
    // ==================================================================

    /// <summary>MySqlAuctionDB.pas:310-439 —— 21 条各 Finalize 一次（含 label 复用陷阱：删除关注用的是
    /// <c>Auction_DeleteAuctionItem</c>，原文 262）。</summary>
    [Fact]
    public void Final_FinalizesAllTwentyOneStatements()
    {
        var fx = NewMySqlWithoutTemporary();

        fx.Unit.DoFinal();

        foreach (string label in DoInitStatementLabels) Assert.Equal(1, fx.Db.For(label).FinalizeCount);
    }

    /// <summary>MySqlAuctionDB.pas:313-437 —— 每条都 <c>if ... &lt;&gt; nil then ... := nil</c>，第二次调用不重复 Finalize。</summary>
    [Fact]
    public void Final_IsIdempotent_SecondCallFinalizesNothing()
    {
        var fx = NewMySqlWithoutTemporary();

        fx.Unit.DoFinal();
        fx.Unit.DoFinal();

        Assert.All(fx.Db.Created, s => Assert.Equal(1, s.FinalizeCount));
    }

    /// <summary>MySqlAuctionDB.pas:313 —— 未 Init 时 21 个字段都是 nil，DoFinal 不抛。</summary>
    [Fact]
    public void Final_WithoutInit_DoesNotThrow()
    {
        var (_, _) = DbLayerTestKit.Isolate();
        var db = new FakeMySqlDatabase();
        var unit = new TMySqlAuctionDB(new FakeDbLayerHost { DataBase = db }, db);

        unit.DoFinal();

        Assert.Empty(db.Created);
    }

    /// <summary>MySqlAuctionDB.pas:315-316 —— Final 把字段置 nil；再 Init 会重新 AddSQLStatement（Fake 同名复用）
    /// 并再次 Prepare。</summary>
    [Fact]
    public void Final_ThenReInit_PreparesAgain()
    {
        var fx = NewMySqlWithoutTemporary();
        fx.Unit.DoFinal();

        fx.Unit.DoInit();

        Assert.Equal(2, fx.Db.For("Auction_QueryOneItem").PrepareCount);
        Assert.Equal(21 + 21, fx.Db.AddedNames.Count);   // DoInit 两次各登记 21 条
    }

    // ==================================================================
    // 3. QueryAllItems —— 原文 741-893（public 包装层 DbBases.cs:701-730）
    // ==================================================================

    /// <summary>MySqlAuctionDB.pas:859-883 —— 无预设行时 <c>Query()</c> 为真、首次 <c>Fetch()</c> 即假 → 0 条。</summary>
    [Fact]
    public void QueryAllItems_EmptyResult_ReturnsZeroAndQueriesOnce()
    {
        var fx = NewMySql();
        var list = new TAuctionItemList();

        int n = fx.Unit.QueryAllItems("", IgAll, "Alice", 1, -1, 0, 0, true, 0, 0, 0, list);

        Assert.Equal(0, n);
        Assert.Equal(0, list.Count);
        var sm = fx.Db.For("Auction_QueryAllItems");
        Assert.Equal(1, sm.QueryCount);
        Assert.Equal(1, sm.FetchCount);       // while 条件求值一次 → false
    }

    /// <summary>★ MySQL 方言（原文 859）：`if sm.Query then` —— Query 返回 false 时**连 Fetch 都不调**。
    /// （SQLite 版是 `Step() == SQLITE_ROW`，没有"Query 失败"这一层。）</summary>
    [Fact]
    public void QueryAllItems_QueryReturnsFalse_ReturnsZeroWithoutFetching()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryAllItems").QueryResult = false;    // 原文 859：if sm.Query then
        var list = new TAuctionItemList();

        int n = fx.Unit.QueryAllItems("", IgAll, "H", 1, -1, 0, 0, true, 0, 0, 0, list);

        Assert.Equal(0, n);
        Assert.Empty(list.Snapshot());
        Assert.Equal(0, fx.Db.For("Auction_QueryAllItems").FetchCount);
    }

    /// <summary>MySqlAuctionDB.pas:863-875 —— 13 列列序；AddDateTime 走 DATETIME 列。</summary>
    [Fact]
    public void QueryAllItems_TwoRows_ReturnsRecordsInColumnOrder()
    {
        var fx = NewMySql();
        var t0 = new DateTime(2024, 3, 1, 8, 30, 0, DateTimeKind.Unspecified);
        var t1 = new DateTime(2024, 3, 2, 9, 45, 1, DateTimeKind.Unspecified);
        fx.Db.For("Auction_QueryAllItems").AddRow(
            AllItemsRow(11, "Alice", t0, 24, 3600, 100, 500, 2, 150, "Bob", 0, 0, 1));
        fx.Db.For("Auction_QueryAllItems").AddRow(
            AllItemsRow(12, "Carol", t1, 12, -5, 200, 900, 1, 0, "", 1, 1, 0));
        var list = new TAuctionItemList();

        int n = fx.Unit.QueryAllItems("", IgAll, "Alice", 1, -1, 0, 0, true, 0, 0, 0, list);

        Assert.Equal(2, n);
        Assert.Equal(2, list.Count);
        Assert.Equal(11, list[0].AuctionID);
        Assert.Equal("Alice", list[0].HumanName);
        Assert.Equal(t0, list[0].AddDateTime);          // 原文 865：OrderGetColumnValueDateTime（真 DATETIME）
        Assert.Equal(24, list[0].AuctionTime);
        Assert.Equal(3600, list[0].TimeLeft);
        Assert.Equal(100u, list[0].StartingPrice);
        Assert.Equal(500u, list[0].SellingPrice);
        Assert.Equal(2, list[0].CurrencyType);
        Assert.Equal(150, list[0].LastBidPrice);
        Assert.Equal("Bob", list[0].LastBidder);
        Assert.Equal(0, list[0].TradingStatus);
        Assert.False(list[0].IsItemGive);
        Assert.True(list[0].IsAttention);               // 第 13 列（原文 875）
        Assert.Equal(12, list[1].AuctionID);
        Assert.Equal(t1, list[1].AddDateTime);
        Assert.True(list[1].IsItemGive);
        Assert.False(list[1].IsAttention);
        // 原文 877：每行都 Owner.LoadItemFromDB(@ActionItem, AuctionID, AUCTION_ITEM_TYPE, 0)。
        Assert.Equal(new[] { "11/5/0", "12/5/0" }, fx.Host.LoadedItems.ToArray());
        Assert.Equal(3, fx.Db.For("Auction_QueryAllItems").FetchCount);   // 两行 true + 末尾一次 false
    }

    /// <summary>MySqlAuctionDB.pas:856-858 —— 绑定顺序 HumanName、AUCTION_PAGE_COUNT、offset。</summary>
    [Fact]
    public void QueryAllItems_BindsHumanNameThenPageCountThenOffset()
    {
        var fx = NewMySql();

        fx.Unit.QueryAllItems("", IgAll, "Alice", 3, -1, 0, 0, true, 0, 0, 0, new TAuctionItemList());

        var binds = fx.Db.For("Auction_QueryAllItems").LastBinds;
        Assert.Equal(new[] { "text", "int", "int" }, binds.Select(b => b.Kind).ToArray());
        Assert.Equal("Alice", binds[0].Text);
        Assert.Equal(7, Convert.ToInt32(binds[1].Value));    // Grobal2Const.AUCTION_PAGE_COUNT = 7
        Assert.Equal(14, Convert.ToInt32(binds[2].Value));   // (3-1)*7
    }

    /// <summary>MySqlAuctionDB.pas:858 —— 无下界校验：nPage &lt;= 0 时 offset 就是 (nPage-1)*7（可为负）。</summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    [InlineData(int.MinValue)]
    public void QueryAllItems_OutOfRangePage_DoesNotClampTheOffset(int nPage)
    {
        var fx = NewMySql();

        fx.Unit.QueryAllItems("", IgAll, "H", nPage, -1, 0, 0, true, 0, 0, 0, new TAuctionItemList());

        Assert.Equal(unchecked((nPage - 1) * 7),
            Convert.ToInt32(fx.Db.For("Auction_QueryAllItems").LastBinds[2].Value));
    }

    /// <summary>MySqlAuctionDB.pas:826-850 —— sortField 不在 0..3 时 sOrderBy 保持空串（原文如此）。</summary>
    [Fact]
    public void QueryAllItems_SortFieldOutOfRange_LeavesOrderByEmpty()
    {
        var fx = NewMySql();

        fx.Unit.QueryAllItems("", IgAll, "H", 1, -1, 0, 99, true, 0, 0, 0, new TAuctionItemList());

        string sql = fx.Db.For("Auction_QueryAllItems").Sql;
        Assert.DoesNotContain(" order by ", sql);
        Assert.EndsWith(" limit ? offset ?;", sql);
    }

    /// <summary>MySqlAuctionDB.pas:826-845 —— sortField 0..3 的 4 段 order by 逐字（MySQL 用 TIMESTAMPDIFF，
    /// 不是 SQLite 的 strftime）。</summary>
    [Theory]
    [InlineData(0, true, " order by AuctionID desc")]                                        // 原文 849
    [InlineData(1, true, " order by ifnull(A.LastBidPrice, A.StartingPrice)")]               // 829
    [InlineData(1, false, " order by ifnull(A.LastBidPrice, A.StartingPrice) desc")]         // 831
    [InlineData(2, true, " order by A.SellingPrice")]                                        // 836
    [InlineData(2, false, " order by A.SellingPrice desc")]                                  // 838
    [InlineData(3, true,
        " order by TIMESTAMPDIFF(SECOND, CURRENT_TIMESTAMP, date_add(AddDateTime, interval AuctionTime hour))")]        // 843
    [InlineData(3, false,
        " order by TIMESTAMPDIFF(SECOND, CURRENT_TIMESTAMP, date_add(AddDateTime, interval AuctionTime hour)) desc")]   // 845
    [InlineData(0, false, " order by AuctionID desc")]                                       // SortField=0 时 SortASC 不参与
    public void QueryAllItems_SortField_ProducesTheOriginalOrderBy(int sortField, bool asc, string expected)
    {
        var fx = NewMySql();

        fx.Unit.QueryAllItems("", IgAll, "H", 1, -1, 0, sortField, asc, 0, 0, 0, new TAuctionItemList());

        Assert.Contains(expected, fx.Db.For("Auction_QueryAllItems").Sql);
    }

    /// <summary>MySqlAuctionDB.pas:754-761/765/808/813/818/823/849/852 —— 无过滤条件时拼接结果逐字等于原文。</summary>
    [Fact]
    public void QueryAllItems_UnfilteredSql_MatchesTheOriginalVerbatim()
    {
        var fx = NewMySql();

        fx.Unit.QueryAllItems("", IgAll, "H", 1, 42, 0, 0, true, 0, 0, 0, new TAuctionItemList());

        // 原文 754-761 的 select 前缀 + IntToStr(42) + ') '，原文 849 的 sOrderBy，原文 852 的 limit 尾巴。
        const string expectedPrefix =
            "select A.AuctionID, A.HumanName, A.AddDateTime, A.AuctionTime, " +
            "(TIMESTAMPDIFF(SECOND, CURRENT_TIMESTAMP, date_add(A.AddDateTime, interval A.AuctionTime hour))) as TimeLeft, " +
            "A.StartingPrice, A.SellingPrice, A.CurrencyType, A.LastBidPrice, A.LastBidder, " +
            "A.TradingStatus, A.IsItemGive, " +
            "ifnull((select 1 from AuctionAttention where AuctionID = A.AuctionID and HumanName = ?), 0) as IsAttention " +
            "from AuctionData A " +
            "where (ifnull(A.TradingStatus, 0) = 0) and " +
            "(TIMESTAMPDIFF(SECOND, CURRENT_TIMESTAMP, date_add(A.AddDateTime, interval A.AuctionTime hour)) > 0) and " +
            "(A.AuctionID <> 42) ";
        Assert.Equal(expectedPrefix + " order by AuctionID desc limit ? offset ?;",
            fx.Db.For("Auction_QueryAllItems").Sql);
    }

    /// <summary>MySqlAuctionDB.pas:806-819 —— TopmostAuctionID / MoneyType-1 / Min / Max 都是直拼串，不绑定。
    /// <para>★ 注：原文 765 的 <c>ItemGroup</c> 子句**未在此断言** —— MySqlAuctionDB.cs:831-833 把生成的
    /// 占位常量 <c>DoQueryAllItems_L765_sm_Sql_P3</c>（值 "@@IntToStr(ItemGroup)@@"）当字面量拼进 SQL，
    /// 而原文 765 拼的是 <c>IntToStr(Integer(ItemGroup))</c>。缺陷与报告见
    /// 「发现的实现缺陷」；修复后请把下面被注释的断言恢复。</para></summary>
    [Fact]
    public void QueryAllItems_TopmostAuctionIdAndFilters_AreConcatenatedVerbatim()
    {
        var fx = NewMySql();

        fx.Unit.QueryAllItems("剑", 5, "H", 1, 42, 0, 0, true, 3, 11, 99, new TAuctionItemList());

        string sql = fx.Db.For("Auction_QueryAllItems").Sql;
        Assert.Contains("and (A.AuctionID <> 42) ", sql);                 // 原文 761
        // 原文 765 期望：Assert.Contains(" and (ItemGroup = 5)", sql);
        // 实际：SQL 里出现的是 "@@IntToStr(ItemGroup)@@"（MySqlAuctionDB.cs:832 用了占位常量）。
        Assert.Contains(" and A.CurrencyType = 2", sql);                  // 原文 808（MoneyType - 1）
        Assert.Contains(" and A.SellingPrice >= 11", sql);                // 原文 813
        Assert.Contains(" and A.SellingPrice <= 99", sql);                // 原文 818
        Assert.Contains(" and ((A.ItemDBName like \"%剑%\") or (A.ItemName like \"%剑%\"))", sql);   // 原文 823
    }

    /// <summary>D1 回归位（**缺陷已修，用例已恢复**）：原文 765 的 ItemGroup 子句应拼成
    /// <c>' and (ItemGroup = 5)'</c>。曾有缺陷：拼上了占位常量
    /// <c>DoQueryAllItems_L765_sm_Sql_P3</c> = "@@IntToStr(ItemGroup)@@"。
    /// 证据：MySqlAuctionDB.pas:765 <c>sm.Sql := sm.Sql + ' and (ItemGroup = ' + IntToStr(Integer(ItemGroup)) + ')';</c>。</summary>
    [Fact]
    public void QueryAllItems_ItemGroupClause_UsesTheNumericValue_NotThePlaceholder()
    {
        var fx = NewMySql();
        fx.Unit.QueryAllItems("", 5, "H", 1, -1, 0, 0, true, 0, 0, 0, new TAuctionItemList());
        string sql = fx.Db.For("Auction_QueryAllItems").Sql;
        Assert.Contains(" and (ItemGroup = 5)", sql);
        Assert.DoesNotContain("@@", sql);
        // 基串尾是 ") "，故原文拼出的是 "…-1)  and (ItemGroup = 5)"（两个空格）。
        Assert.Contains("(A.AuctionID <> -1)  and (ItemGroup = 5)", sql);
    }

    /// <summary>D3 回归位（**缺陷已修，用例已恢复**）：原文 808 的 moneyType 子句只追加
    /// <c>' and A.CurrencyType = '</c>；上一状态（原文 761 的 <c>IntToStr(TopmostAuctionID) + ') '</c>）
    /// 已带右括号，故正确文本是 <c>"(A.AuctionID <> 42)  and A.CurrencyType = 2"</c>（一个右括号）。</summary>
    [Fact]
    public void QueryAllItems_MoneyTypeClause_HasExactlyOneClosingParen()
    {
        var fx = NewMySql();
        fx.Unit.QueryAllItems("", IgAll, "H", 1, 42, 0, 0, true, 3, 0, 0, new TAuctionItemList());
        Assert.Contains("(A.AuctionID <> 42)  and A.CurrencyType = 2", fx.Db.For("Auction_QueryAllItems").Sql);
    }

    /// <summary>D2 回归位（**缺陷已修，用例已恢复**）：原文 802 的 itemColors 子句只追加
    /// <c>' and ItemColor in ('</c>，正确文本是 <c>"(A.AuctionID <> 42)  and ItemColor in (37)"</c>。</summary>
    [Fact]
    public void QueryAllItems_ItemColorsClause_HasExactlyOneClosingParen()
    {
        var fx = NewMySql();
        fx.Env.btAuctionItemColors = 37;
        fx.Unit.QueryAllItems("", IgAll, "H", 1, 42, 1, 0, true, 0, 0, 0, new TAuctionItemList());
        Assert.Contains("(A.AuctionID <> 42)  and ItemColor in (37)", fx.Db.For("Auction_QueryAllItems").Sql);
    }

    /// <summary>D4 回归位（**缺陷已修，用例已恢复**）：原文 1130 的 moneyType 子句只追加
    /// <c>' and CurrencyType = '</c>；基础 SQL 以 <c>… &gt; 0)</c> 结尾，正确文本以
    /// <c>"&gt; 0) and CurrencyType = 0"</c> 收尾。</summary>
    [Fact]
    public void GetAllItemsPageCount_CurrencyTypeClause_HasExactlyOneClosingParen()
    {
        var fx = NewMySql();
        fx.Unit.GetAllItemsPageCount("", IgAll, 0, 1, 0, 0);
        Assert.Contains("interval AuctionTime hour)) > 0) and CurrencyType = 0",
            fx.Db.For("Auction_GetAllItemsCount").Sql);
    }

    /// <summary>D5 回归位（**缺陷已修**）：原文 1087 的 itemGroup 分支只追加
    /// <c>' and ItemGroup = '</c>；曾有缺陷把**整条基串快照**
    /// <c>DoGetAllItemsPageCount_L1087_sm_Sql_P0</c> 拼上去 ⇒ select 基串出现两次。</summary>
    [Fact]
    public void GetAllItemsPageCount_ItemGroupClause_DoesNotDuplicateTheBaseSql()
    {
        var fx = NewMySql();
        fx.Unit.GetAllItemsPageCount("", 5, 0, 0, 0, 0);
        string sql = fx.Db.For("Auction_GetAllItemsCount").Sql;
        Assert.Contains(" and ItemGroup = 5", sql);
        // 基串只能出现一次。
        Assert.Equal(1, sql.Split("select Count(*) from AuctionData", StringSplitOptions.None).Length - 1);
    }

    /// <summary>DbBases.cs:710-715（原文 M2DataCommon.pas:1236-1264）—— Min &gt; Max 时包装层交换后再拼。</summary>
    [Fact]
    public void QueryAllItems_SwapsMinAndMax_WhenMinGreaterThanMax()
    {
        var fx = NewMySql();

        fx.Unit.QueryAllItems("", IgAll, "H", 1, -1, 0, 0, true, 0, 999, 11, new TAuctionItemList());

        string sql = fx.Db.For("Auction_QueryAllItems").Sql;
        Assert.Contains(" and A.SellingPrice >= 11", sql);
        Assert.Contains(" and A.SellingPrice <= 999", sql);
    }

    /// <summary>MySqlAuctionDB.pas:768-804 —— itemColors 位组合按 1/2/4/8/16/32 逐位加色，尾逗号用
    /// <c>Copy(sColors,1,Length-1)</c> 去掉。原文 772-798 六处读的都是同一个 btAuctionItemColors
    /// （接缝只暴露一个 int，见 MySqlAuctionDB.cs:1793-1827）。</summary>
    [Theory]
    [InlineData(1, "37")]
    [InlineData(2, "37")]
    [InlineData(4, "37")]
    [InlineData(8, "37")]
    [InlineData(16, "37")]
    [InlineData(32, "37")]
    [InlineData(3, "37,37")]
    [InlineData(63, "37,37,37,37,37,37")]
    public void QueryAllItems_ItemColorsBitmask_TrimsTheTrailingComma(int mask, string expected)
    {
        var fx = NewMySql();
        fx.Env.btAuctionItemColors = 37;

        fx.Unit.QueryAllItems("", IgAll, "H", 1, -1, mask, 0, true, 0, 0, 0, new TAuctionItemList());

        Assert.Contains(" and ItemColor in (" + expected + ")", fx.Db.For("Auction_QueryAllItems").Sql);
    }

    /// <summary>MySqlAuctionDB.pas:768 —— ItemColors = 0 时整段跳过。</summary>
    [Fact]
    public void QueryAllItems_ItemColorsZero_SkipsTheColorClause()
    {
        var fx = NewMySql();

        fx.Unit.QueryAllItems("", IgAll, "H", 1, -1, 0, 0, true, 0, 0, 0, new TAuctionItemList());

        Assert.DoesNotContain("ItemColor in", fx.Db.For("Auction_QueryAllItems").Sql);
    }

    /// <summary>MySqlAuctionDB.pas:821-824 —— ItemName 直拼进 SQL（本参数不过绑定）。</summary>
    [Fact]
    public void QueryAllItems_SqlInjectionInItemName_IsConcatenatedNotBound()
    {
        var fx = NewMySql();

        fx.Unit.QueryAllItems(Injection, IgAll, "H", 1, -1, 0, 0, true, 0, 0, 0, new TAuctionItemList());

        var sm = fx.Db.For("Auction_QueryAllItems");
        Assert.Contains("like \"%" + Injection + "%\"", sm.Sql);
        Assert.DoesNotContain(Injection, sm.LastBinds.Select(b => b.Text));
    }

    /// <summary>MySqlAuctionDB.pas:856 —— HumanName 走 OrderBindParamText，不进 SQL 文本。</summary>
    [Fact]
    public void QueryAllItems_SqlInjectionInHumanName_IsBound()
    {
        var fx = NewMySql();

        fx.Unit.QueryAllItems("", IgAll, Injection, 1, -1, 0, 0, true, 0, 0, 0, new TAuctionItemList());

        var sm = fx.Db.For("Auction_QueryAllItems");
        Assert.Equal(Injection, sm.LastBinds[0].Text);
        Assert.DoesNotContain(Injection, sm.Sql);
    }

    /// <summary>MySqlAuctionDB.pas:887-891 —— 语句抛错被自身 except 吞掉（裸消息），Result 保持 0；
    /// 原文 884-885 的 finally 仍然 Reset。</summary>
    [Fact]
    public void QueryAllItems_QueryThrows_LogsAndReturnsZero()
    {
        var fx = NewMySqlThrowing("Auction_QueryAllItems", new InvalidOperationException("query-boom"));
        var list = new TAuctionItemList();

        int n = fx.Unit.QueryAllItems("", IgAll, "H", 1, -1, 0, 0, true, 0, 0, 0, list);

        Assert.Equal(0, n);
        Assert.Empty(list.Snapshot());
        Assert.Equal(new[] { "query-boom" }, fx.Log.Messages.ToArray());
        Assert.Equal(1, fx.Db.For("Auction_QueryAllItems").ResetCount);
    }

    /// <summary>MySqlAuctionDB.pas:884-885 —— MySQL 动态语句的 finally **只** Reset，**不** Finalize
    /// （姊妹 SQLite 版两处都调 StatementFinalize，SqliteAuctionDB.cs:906-908）。</summary>
    [Fact]
    public void QueryAllItems_FinalizeIsNotCalled_UnlikeTheSqliteSibling()
    {
        var fx = NewMySql();

        fx.Unit.QueryAllItems("", IgAll, "H", 1, -1, 0, 0, true, 0, 0, 0, new TAuctionItemList());

        Assert.Equal(0, fx.Db.For("Auction_QueryAllItems").FinalizeCount);
        Assert.Equal(1, fx.Db.For("Auction_QueryAllItems").ResetCount);
    }

    /// <summary>DbBases.cs:701-730 的 public 包装层：Lock/UnLock 各一次。</summary>
    [Fact]
    public void QueryAllItems_PublicWrapperLocksAndUnlocks()
    {
        var fx = NewMySql();

        fx.Unit.QueryAllItems("", IgAll, "H", 1, -1, 0, 0, true, 0, 0, 0, new TAuctionItemList());

        Assert.Equal(1, fx.Host.LockCount);
        Assert.Equal(1, fx.Host.UnLockCount);
    }

    // ==================================================================
    // 4. QueryMyItems —— 原文 895-941
    // ==================================================================

    /// <summary>MySqlAuctionDB.pas:908-931 —— 无行时 Result 0。</summary>
    [Fact]
    public void QueryMyItems_EmptyResult_ReturnsZero()
    {
        var fx = NewMySql();
        var list = new TAuctionItemList();

        int n = fx.Unit.QueryMyItems("Alice", 1, list);

        Assert.Equal(0, n);
        Assert.Equal(0, list.Count);
        Assert.Equal(1, fx.Db.For("Auction_QueryAuctionItems").FetchCount);
    }

    /// <summary>★ MySQL 方言（原文 908）：Query 返回 false → 不进 while。</summary>
    [Fact]
    public void QueryMyItems_QueryReturnsFalse_ReturnsZero()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryAuctionItems").QueryResult = false;

        int n = fx.Unit.QueryMyItems("Alice", 1, new TAuctionItemList());

        Assert.Equal(0, n);
        Assert.Equal(0, fx.Db.For("Auction_QueryAuctionItems").FetchCount);
    }

    /// <summary>MySqlAuctionDB.pas:912-923 —— 12 列列序（无 IsAttention 列 → 保持默认 false）。</summary>
    [Fact]
    public void QueryMyItems_TwoRows_ReturnsRecordsWithoutAttentionColumn()
    {
        var fx = NewMySql();
        var t0 = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        fx.Db.For("Auction_QueryAuctionItems").AddRow(
            MyItemsRow(21, "Alice", t0, 24, 100, 10, 20, 0, 15, "Dave", 0, 0));
        fx.Db.For("Auction_QueryAuctionItems").AddRow(
            MyItemsRow(22, "Alice", t0, 48, -100, 30, 40, 1, 0, "", 2, 1));
        var list = new TAuctionItemList();

        int n = fx.Unit.QueryMyItems("Alice", 1, list);

        Assert.Equal(2, n);
        Assert.Equal(21, list[0].AuctionID);
        Assert.Equal(t0, list[0].AddDateTime);
        Assert.Equal(10u, list[0].StartingPrice);
        Assert.Equal("Dave", list[0].LastBidder);
        Assert.False(list[0].IsAttention);          // 原文 912-923 没有这一列
        Assert.Equal(2, list[1].TradingStatus);
        Assert.True(list[1].IsItemGive);
        Assert.Equal(new[] { "21/5/0", "22/5/0" }, fx.Host.LoadedItems.ToArray());
    }

    /// <summary>MySqlAuctionDB.pas:903-906 —— 绑定顺序 HumanName、7、offset。</summary>
    [Fact]
    public void QueryMyItems_BindsHumanNamePageAndOffset()
    {
        var fx = NewMySql();

        fx.Unit.QueryMyItems("Alice", 2, new TAuctionItemList());

        var binds = fx.Db.For("Auction_QueryAuctionItems").LastBinds;
        Assert.Equal(new[] { "text", "int", "int" }, binds.Select(b => b.Kind).ToArray());
        Assert.Equal("Alice", binds[0].Text);
        Assert.Equal(7, Convert.ToInt32(binds[1].Value));
        Assert.Equal(7, Convert.ToInt32(binds[2].Value));   // (2-1)*7
    }

    /// <summary>MySqlAuctionDB.pas:906 —— nPage &lt;= 0 不夹取。</summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void QueryMyItems_NegativeOrZeroPage_PassesTheRawOffsetThrough(int nPage)
    {
        var fx = NewMySql();

        fx.Unit.QueryMyItems("H", nPage, new TAuctionItemList());

        Assert.Equal(unchecked((nPage - 1) * 7),
            Convert.ToInt32(fx.Db.For("Auction_QueryAuctionItems").LastBinds[2].Value));
    }

    /// <summary>MySqlAuctionDB.pas:904 —— 注入串走绑定。</summary>
    [Fact]
    public void QueryMyItems_SqlInjectionInHumanName_IsBoundNotConcatenated()
    {
        var fx = NewMySql();

        fx.Unit.QueryMyItems(Injection, 1, new TAuctionItemList());

        var sm = fx.Db.For("Auction_QueryAuctionItems");
        Assert.Equal(Injection, sm.LastBinds[0].Text);
        Assert.DoesNotContain(Injection, sm.Sql);
    }

    /// <summary>MySqlAuctionDB.pas:935-939 —— 自身 except 吞掉异常（裸消息）；原文 932-933 的 finally 仍 Reset。</summary>
    [Fact]
    public void QueryMyItems_QueryThrows_LogsAndReturnsZero()
    {
        var fx = NewMySqlThrowing("Auction_QueryAuctionItems", new InvalidOperationException("my-items-boom"));

        int n = fx.Unit.QueryMyItems("H", 1, new TAuctionItemList());

        Assert.Equal(0, n);
        Assert.Equal(new[] { "my-items-boom" }, fx.Log.Messages.ToArray());
        Assert.Equal(2, fx.Db.For("Auction_QueryAuctionItems").ResetCount);   // 903 + 933 finally
    }

    // ==================================================================
    // 5. QueryMyAttentionItems —— 原文 943-996
    // ==================================================================

    /// <summary>MySqlAuctionDB.pas:955-984 —— 无行 → 0。</summary>
    [Fact]
    public void QueryMyAttentionItems_EmptyResult_ReturnsZero()
    {
        var fx = NewMySql();
        var list = new TAuctionItemList();

        int n = fx.Unit.QueryMyAttentionItems("Alice", 1, list);

        Assert.Equal(0, n);
        Assert.Empty(list.Snapshot());
        Assert.Equal(0, fx.Db.For("Auction_QueryOneItem").QueryCount);
    }

    /// <summary>MySqlAuctionDB.pas:955-982 —— Query 返回 false → 不进 while（MySQL 独有形态）。</summary>
    [Fact]
    public void QueryMyAttentionItems_QueryReturnsFalse_ReturnsZero()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryAttentionItems").QueryResult = false;

        int n = fx.Unit.QueryMyAttentionItems("H", 1, new TAuctionItemList());

        Assert.Equal(0, n);
        Assert.Equal(0, fx.Db.For("Auction_QueryAttentionItems").FetchCount);
    }

    /// <summary>MySqlAuctionDB.pas:957-975 —— 先取 AuctionID，再用 Auction_QueryOneItem 查完整记录
    /// （AuctionID 来自行，其余字段来自第二条语句的 DATETIME 列）。</summary>
    [Fact]
    public void QueryMyAttentionItems_OneId_ThenLoadsTheFullRecordViaQueryOneItem()
    {
        var fx = NewMySql();
        var t = new DateTime(2024, 5, 6, 7, 8, 9, DateTimeKind.Unspecified);
        fx.Db.For("Auction_QueryAttentionItems").AddRow(31);
        fx.Db.For("Auction_QueryOneItem").AddRow(
            OneItemRow("Seller", t, 6, 60, 7, 8, 3, 9, "Bidder", 0, 0));
        var list = new TAuctionItemList();

        int n = fx.Unit.QueryMyAttentionItems("Alice", 1, list);

        Assert.Equal(1, n);
        Assert.Equal(31, list[0].AuctionID);
        Assert.Equal("Seller", list[0].HumanName);
        Assert.Equal(t, list[0].AddDateTime);
        Assert.Equal(7u, list[0].StartingPrice);
        Assert.Equal("Bidder", list[0].LastBidder);
        Assert.Equal(3, list[0].CurrencyType);
        Assert.Equal(1, fx.Db.For("Auction_QueryOneItem").QueryCount);
        Assert.Equal(1, fx.Db.For("Auction_QueryOneItem").FetchCount);
        Assert.Equal(new[] { "31/5/0" }, fx.Host.LoadedItems.ToArray());
    }

    /// <summary>MySqlAuctionDB.pas:963-976 —— Auction_QueryOneItem 无行时只填 AuctionID，其余保持默认
    /// （原文没有 else）。</summary>
    [Fact]
    public void QueryMyAttentionItems_AuctionIdStillAdded_WhenQueryOneItemFindsNothing()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryAttentionItems").AddRow(41);
        var list = new TAuctionItemList();

        int n = fx.Unit.QueryMyAttentionItems("Alice", 1, list);

        Assert.Equal(1, n);
        Assert.Equal(41, list[0].AuctionID);
        Assert.Equal("", list[0].HumanName);
        Assert.Equal(0u, list[0].StartingPrice);
        Assert.Equal(default, list[0].AddDateTime);
    }

    /// <summary>★ MySQL 方言（原文 963）：`if Query and Fetch then` —— 内层 Query 返回 false 时**不调 Fetch**，
    /// 记录仍然入列（原文 978-982 无条件）。</summary>
    [Fact]
    public void QueryMyAttentionItems_InnerQueryReturnsFalse_ShortCircuitsFetch()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryAttentionItems").AddRow(51);
        fx.Db.For("Auction_QueryOneItem").QueryResult = false;
        var list = new TAuctionItemList();

        int n = fx.Unit.QueryMyAttentionItems("H", 1, list);

        Assert.Equal(1, n);
        Assert.Equal(51, list[0].AuctionID);
        Assert.Equal(0, fx.Db.For("Auction_QueryOneItem").FetchCount);
        Assert.Equal("", list[0].HumanName);
    }

    /// <summary>MySqlAuctionDB.pas:957-982 —— 多行时外层每行 Reset 内层一次：2 条关注 → 2 条记录、
    /// 内层 Query/Reset 各 2 次（+ finally 1 次）。</summary>
    [Fact]
    public void QueryMyAttentionItems_TwoIds_QueriesOneItemOncePerRow()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryAttentionItems").AddRow(61);
        fx.Db.For("Auction_QueryAttentionItems").AddRow(62);
        fx.Db.For("Auction_QueryOneItem").AddRow(
            OneItemRow("S", 100L, 1, 2, 3, 4, 0, 5, "B", 0, 0));
        var list = new TAuctionItemList();

        int n = fx.Unit.QueryMyAttentionItems("H", 1, list);

        Assert.Equal(2, n);
        Assert.Equal(new[] { 61, 62 }, list.Snapshot().Select(r => r.AuctionID).ToArray());
        Assert.Equal(2, fx.Db.For("Auction_QueryOneItem").QueryCount);
        Assert.Equal(3, fx.Db.For("Auction_QueryOneItem").ResetCount);           // 每行 1 次（961）+ finally 1 次（987）
        Assert.Equal(2, fx.Db.For("Auction_QueryAttentionItems").ResetCount);    // 950 + 988 finally
        Assert.Equal(62, Convert.ToInt32(fx.Db.For("Auction_QueryOneItem").LastBinds[0].Value));
        Assert.Equal(3, fx.Db.For("Auction_QueryAttentionItems").FetchCount);    // 2 行 + 末尾 false
    }

    /// <summary>MySqlAuctionDB.pas:951-953 —— 外层绑定 HumanName、7、offset。</summary>
    [Fact]
    public void QueryMyAttentionItems_BindsHumanNamePageAndOffset()
    {
        var fx = NewMySql();

        fx.Unit.QueryMyAttentionItems("Alice", 2, new TAuctionItemList());

        var binds = fx.Db.For("Auction_QueryAttentionItems").LastBinds;
        Assert.Equal(new[] { "text", "int", "int" }, binds.Select(b => b.Kind).ToArray());
        Assert.Equal("Alice", binds[0].Text);
        Assert.Equal(7, Convert.ToInt32(binds[1].Value));
        Assert.Equal(7, Convert.ToInt32(binds[2].Value));
    }

    /// <summary>MySqlAuctionDB.pas:986-989 —— finally 里两个 Reset，且 QueryOneItem 在 QueryMyAttentionItems
    /// **之前**（原文顺序）。</summary>
    [Fact]
    public void QueryMyAttentionItems_ResetsBothStatementsInTheOriginalOrder()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryAttentionItems").AddRow(71);

        fx.Unit.QueryMyAttentionItems("H", 1, new TAuctionItemList());

        // 1 行关注：内层 QueryOneItem 在循环里 Reset 一次（原文 961）+ finally 的 987 一次 = 2。
        Assert.Equal(2, fx.Db.For("Auction_QueryOneItem").ResetCount);
        Assert.Equal(2, fx.Db.For("Auction_QueryAttentionItems").ResetCount);   // 950 + 988 finally
    }

    /// <summary>MySqlAuctionDB.pas:951 —— 注入串走绑定。</summary>
    [Fact]
    public void QueryMyAttentionItems_SqlInjectionInHumanName_IsBound()
    {
        var fx = NewMySql();

        fx.Unit.QueryMyAttentionItems(Injection, 1, new TAuctionItemList());

        Assert.Equal(Injection, fx.Db.For("Auction_QueryAttentionItems").LastBinds[0].Text);
        Assert.DoesNotContain(Injection, fx.Db.For("Auction_QueryAttentionItems").Sql);
    }

    /// <summary>MySqlAuctionDB.pas:990-995 —— 自身 except 吞掉异常（裸消息）。</summary>
    [Fact]
    public void QueryMyAttentionItems_QueryThrows_LogsAndReturnsZero()
    {
        var fx = NewMySqlThrowing("Auction_QueryAttentionItems", new InvalidOperationException("att-boom"));

        int n = fx.Unit.QueryMyAttentionItems("H", 1, new TAuctionItemList());

        Assert.Equal(0, n);
        Assert.Equal(new[] { "att-boom" }, fx.Log.Messages.ToArray());
    }

    // ==================================================================
    // 6. GetAllItemsPageCount —— 原文 1071-1162
    // ==================================================================

    /// <summary>MySqlAuctionDB.pas:1149-1152 —— 无行 → 0。</summary>
    [Fact]
    public void GetAllItemsPageCount_NoRow_ReturnsZero()
    {
        var fx = NewMySql();

        Assert.Equal(0, fx.Unit.GetAllItemsPageCount("", IgAll, 0, 0, 0, 0));
    }

    /// <summary>★ MySQL 方言（原文 1149）：`if sm.Query and sm.Fetch then` → Query 假时不 Fetch。</summary>
    [Fact]
    public void GetAllItemsPageCount_QueryReturnsFalse_ReturnsZero()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetAllItemsCount").QueryResult = false;

        Assert.Equal(0, fx.Unit.GetAllItemsPageCount("", IgAll, 0, 0, 0, 0));
        Assert.Equal(0, fx.Db.For("Auction_GetAllItemsCount").FetchCount);
    }

    /// <summary>MySqlAuctionDB.pas:1151 —— <c>(Count + AUCTION_PAGE_COUNT - 1) div AUCTION_PAGE_COUNT</c>（整除）。</summary>
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(7, 1)]
    [InlineData(8, 2)]
    [InlineData(14, 2)]
    [InlineData(15, 3)]
    public void GetAllItemsPageCount_CeilingDivisionBySeven(int count, int expected)
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetAllItemsCount").AddRow(count);

        Assert.Equal(expected, fx.Unit.GetAllItemsPageCount("", IgAll, 0, 0, 0, 0));
    }

    /// <summary>MySqlAuctionDB.pas:1079-1083 —— 无过滤时 SQL 逐字等于原文；且临时语句 finally 只 Reset、不 Finalize。</summary>
    [Fact]
    public void GetAllItemsPageCount_UnfilteredSql_MatchesTheOriginalVerbatim()
    {
        var fx = NewMySql();

        fx.Unit.GetAllItemsPageCount("", IgAll, 0, 0, 0, 0);

        Assert.Equal(
            "select Count(*) from AuctionData where (ifnull(TradingStatus, 0) = 0) and " +
            "(TIMESTAMPDIFF(SECOND, CURRENT_TIMESTAMP, date_add(AddDateTime, interval AuctionTime hour)) > 0)",
            fx.Db.For("Auction_GetAllItemsCount").Sql);
        Assert.Equal(1, fx.Db.For("Auction_GetAllItemsCount").ResetCount);       // 原文 1154 finally
        Assert.Equal(0, fx.Db.For("Auction_GetAllItemsCount").FinalizeCount);    // 原文 1153-1155 没有 Finalize
        Assert.True(fx.Db.Has("Auction_GetAllItemsCount"));
    }

    /// <summary>MySqlAuctionDB.pas:1087/1124/1130/1135/1140/1145 —— 六个过滤段直拼串
    /// （注意这里的列名没有 <c>A.</c> 前缀，与 DoQueryAllItems 不同）。</summary>
    [Fact]
    public void GetAllItemsPageCount_Filters_AreConcatenatedVerbatim()
    {
        var fx = NewMySql();
        fx.Env.btAuctionItemColors = 5;

        fx.Unit.GetAllItemsPageCount("剑", 5, 3, 1, 11, 99);

        string sql = fx.Db.For("Auction_GetAllItemsCount").Sql;
        Assert.Contains(" and ItemGroup = 5", sql);                          // 1087
        Assert.Contains(" and ItemColor in (5,5)", sql);                     // 1124
        Assert.Contains(" and CurrencyType = 0", sql);                       // 1130（MoneyType - 1）
        Assert.Contains(" and SellingPrice >= 11", sql);                     // 1135
        Assert.Contains(" and SellingPrice <= 99", sql);                     // 1140
        Assert.Contains(" and ((ItemDBName like \"%剑%\") or (ItemName like \"%剑%\"))", sql);   // 1145
    }

    /// <summary>DbBases.cs:787-792 —— 包装层的 Min/Max 互换守卫。</summary>
    [Fact]
    public void GetAllItemsPageCount_SwapsMinAndMax_WhenMinGreaterThanMax()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetAllItemsCount").AddRow(1);

        fx.Unit.GetAllItemsPageCount("", IgAll, 0, 0, 999, 11);

        string sql = fx.Db.For("Auction_GetAllItemsCount").Sql;
        Assert.Contains(" and SellingPrice >= 11", sql);
        Assert.Contains(" and SellingPrice <= 999", sql);
    }

    /// <summary>MySqlAuctionDB.pas:1133/1138 —— MinPrices/MaxPrices 为 0 时两段都跳过。</summary>
    [Fact]
    public void GetAllItemsPageCount_ZeroPrices_SkipBothPriceClauses()
    {
        var fx = NewMySql();

        fx.Unit.GetAllItemsPageCount("", IgAll, 0, 0, 0, 0);

        string sql = fx.Db.For("Auction_GetAllItemsCount").Sql;
        Assert.DoesNotContain("SellingPrice >=", sql);
        Assert.DoesNotContain("SellingPrice <=", sql);
    }

    /// <summary>MySqlAuctionDB.pas:1143-1146 —— ItemName 直拼串。</summary>
    [Fact]
    public void GetAllItemsPageCount_SqlInjectionInItemName_IsConcatenated()
    {
        var fx = NewMySql();

        fx.Unit.GetAllItemsPageCount(Injection, IgAll, 0, 0, 0, 0);

        Assert.Contains("like \"%" + Injection + "%\"", fx.Db.For("Auction_GetAllItemsCount").Sql);
    }

    /// <summary>MySqlAuctionDB.pas:1156-1160 —— 自身 except 吞掉异常（裸消息）。</summary>
    [Fact]
    public void GetAllItemsPageCount_QueryThrows_LogsAndReturnsZero()
    {
        var fx = NewMySqlThrowing("Auction_GetAllItemsCount", new InvalidOperationException("count-boom"));

        int n = fx.Unit.GetAllItemsPageCount("", IgAll, 0, 0, 0, 0);

        Assert.Equal(0, n);
        Assert.Equal(new[] { "count-boom" }, fx.Log.Messages.ToArray());
    }

    // ==================================================================
    // 7. GetMyItemsPageCount —— 原文 1164-1177（★ 无 except、只有 finally）
    // ==================================================================

    /// <summary>MySqlAuctionDB.pas:1170-1173 —— 无行 → 0。</summary>
    [Fact]
    public void GetMyItemsPageCount_NoRow_ReturnsZero()
    {
        var fx = NewMySql();

        Assert.Equal(0, fx.Unit.GetMyItemsPageCount("Alice"));
    }

    /// <summary>MySqlAuctionDB.pas:1172 —— 整除 7 向上取整。</summary>
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(7, 1)]
    [InlineData(8, 2)]
    [InlineData(21, 3)]
    public void GetMyItemsPageCount_CeilingDivisionBySeven(int count, int expected)
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMyItemsCount").AddRow(count);

        Assert.Equal(expected, fx.Unit.GetMyItemsPageCount("Alice"));
    }

    /// <summary>MySqlAuctionDB.pas:1169 —— 只绑 HumanName（无 limit/offset）。</summary>
    [Fact]
    public void GetMyItemsPageCount_BindsHumanNameOnly()
    {
        var fx = NewMySql();

        fx.Unit.GetMyItemsPageCount(Injection);

        var binds = fx.Db.For("Auction_GetMyItemsCount").LastBinds;
        Assert.Single(binds);
        Assert.Equal("text", binds[0].Kind);      // OrderBindParamText（不是 SQLite 的 OrderBindText）
        Assert.Equal(Injection, binds[0].Text);
        Assert.Equal(1, fx.Db.For("Auction_GetMyItemsCount").QueryCount);
    }

    /// <summary>MySqlAuctionDB.pas:1164-1177 —— ★ 本方法**没有** except、只有 finally（与 SQLite 版同形），
    /// 所以异常**穿透**到 public 包装层 DbBases.cs:808-819，日志带 <c>[Exception] TAuctionDB:GetMyItemsPageCount;</c> 前缀。</summary>
    [Fact]
    public void GetMyItemsPageCount_HasNoExceptOnlyFinally_StepErrorEscapesToTheWrapper()
    {
        var fx = NewMySqlThrowing("Auction_GetMyItemsCount", new InvalidOperationException("my-count-boom"));

        int n = fx.Unit.GetMyItemsPageCount("H");

        Assert.Equal(0, n);
        Assert.Contains(fx.Log.Messages,
            m => m.Contains("[Exception] TAuctionDB:GetMyItemsPageCount") && m.Contains("my-count-boom"));
        Assert.Equal(2, fx.Db.For("Auction_GetMyItemsCount").ResetCount);   // 1168 + 1175 finally
    }

    /// <summary>★ MySQL 方言（原文 1170）：Query 假 → 不进分支，Result 0。</summary>
    [Fact]
    public void GetMyItemsPageCount_QueryReturnsFalse_ReturnsZero()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMyItemsCount").QueryResult = false;

        Assert.Equal(0, fx.Unit.GetMyItemsPageCount("H"));
        Assert.Equal(0, fx.Db.For("Auction_GetMyItemsCount").FetchCount);
    }

    // ==================================================================
    // 8. GetMyAttentionPageCount —— 原文 1179-1199（对比 §7：有 except）
    // ==================================================================

    /// <summary>MySqlAuctionDB.pas:1186-1189 —— 无行 → 0。</summary>
    [Fact]
    public void GetMyAttentionPageCount_NoRow_ReturnsZero()
    {
        var fx = NewMySql();

        Assert.Equal(0, fx.Unit.GetMyAttentionPageCount("Alice"));
    }

    /// <summary>MySqlAuctionDB.pas:1188 —— 整除 7 向上取整。</summary>
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(7, 1)]
    [InlineData(8, 2)]
    [InlineData(70, 10)]
    public void GetMyAttentionPageCount_CeilingDivisionBySeven(int count, int expected)
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMyAttentionItemsCount").AddRow(count);

        Assert.Equal(expected, fx.Unit.GetMyAttentionPageCount("H"));
    }

    /// <summary>MySqlAuctionDB.pas:1185/1190-1191 —— 绑定 + finally Reset（2 次）。</summary>
    [Fact]
    public void GetMyAttentionPageCount_BindsHumanNameAndResetsInFinally()
    {
        var fx = NewMySql();

        fx.Unit.GetMyAttentionPageCount(Injection);

        var sm = fx.Db.For("Auction_GetMyAttentionItemsCount");
        Assert.Equal(Injection, sm.LastBinds[0].Text);
        Assert.Equal(2, sm.ResetCount);
    }

    /// <summary>MySqlAuctionDB.pas:1193-1197 —— 本方法**有**自己的 except → 日志是**裸消息**，
    /// 不带包装层的 <c>[Exception] TAuctionDB:</c> 前缀（与 §7 相反）。</summary>
    [Fact]
    public void GetMyAttentionPageCount_HasItsOwnExcept_SoTheErrorIsLoggedHere()
    {
        var fx = NewMySqlThrowing("Auction_GetMyAttentionItemsCount",
            new InvalidOperationException("att-count-boom"));

        int n = fx.Unit.GetMyAttentionPageCount("H");

        Assert.Equal(0, n);
        Assert.Equal(new[] { "att-count-boom" }, fx.Log.Messages.ToArray());
    }

    // ==================================================================
    // 9. GetMyAuctioningItemsCount —— 原文 1201-1221
    // ==================================================================

    /// <summary>MySqlAuctionDB.pas:1208-1211 —— 无行 → 0。</summary>
    [Fact]
    public void GetMyAuctioningItemsCount_NoRow_ReturnsZero()
    {
        var fx = NewMySql();

        Assert.Equal(0, fx.Unit.GetMyAuctioningItemsCount("Alice"));
    }

    /// <summary>MySqlAuctionDB.pas:1210 —— 直接取列值，**不**做页数整除（与 §7/§8 不同）。</summary>
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(123)]
    [InlineData(-5)]
    public void GetMyAuctioningItemsCount_ReturnsRawCountWithoutPageDivision(int count)
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMyAuctioningItemsCount").AddRow(count);

        Assert.Equal(count, fx.Unit.GetMyAuctioningItemsCount("H"));
    }

    /// <summary>MySqlAuctionDB.pas:1207/1212-1213 —— 绑定 + 两次 Reset。</summary>
    [Fact]
    public void GetMyAuctioningItemsCount_BindsHumanNameAndResets()
    {
        var fx = NewMySql();

        fx.Unit.GetMyAuctioningItemsCount(Injection);

        var sm = fx.Db.For("Auction_GetMyAuctioningItemsCount");
        Assert.Equal(Injection, sm.LastBinds[0].Text);
        Assert.Equal(2, sm.ResetCount);
    }

    /// <summary>MySqlAuctionDB.pas:1215-1219 —— 自身 except → 裸消息。</summary>
    [Fact]
    public void GetMyAuctioningItemsCount_QueryThrows_LogsAndReturnsZero()
    {
        var fx = NewMySqlThrowing("Auction_GetMyAuctioningItemsCount",
            new InvalidOperationException("auctioning-boom"));

        Assert.Equal(0, fx.Unit.GetMyAuctioningItemsCount("H"));
        Assert.Equal(new[] { "auctioning-boom" }, fx.Log.Messages.ToArray());
    }

    // ==================================================================
    // 10. GetMySellFailItemsCount —— 原文 1223-1244
    // ==================================================================

    /// <summary>MySqlAuctionDB.pas:1231-1234 —— 无行 → 0。</summary>
    [Fact]
    public void GetMySellFailItemsCount_NoRow_ReturnsZero()
    {
        var fx = NewMySql();

        Assert.Equal(0, fx.Unit.GetMySellFailItemsCount("Alice"));
    }

    /// <summary>MySqlAuctionDB.pas:1233 —— 原值返回。</summary>
    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(-1)]
    public void GetMySellFailItemsCount_ReturnsRawCount(int count)
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMySellFailItemsCount").AddRow(count);

        Assert.Equal(count, fx.Unit.GetMySellFailItemsCount("H"));
    }

    /// <summary>MySqlAuctionDB.pas:1230/1235-1236 —— 绑定 + 两次 Reset。</summary>
    [Fact]
    public void GetMySellFailItemsCount_BindsHumanName()
    {
        var fx = NewMySql();

        fx.Unit.GetMySellFailItemsCount(Injection);

        var sm = fx.Db.For("Auction_GetMySellFailItemsCount");
        Assert.Equal(new[] { "text" }, sm.LastBinds.Select(b => b.Kind).ToArray());
        Assert.Equal(Injection, sm.LastBinds[0].Text);
        Assert.Equal(2, sm.ResetCount);
    }

    /// <summary>MySqlAuctionDB.pas:1238-1242 —— 自身 except → 裸消息。</summary>
    [Fact]
    public void GetMySellFailItemsCount_QueryThrows_LogsAndReturnsZero()
    {
        var fx = NewMySqlThrowing("Auction_GetMySellFailItemsCount",
            new InvalidOperationException("sellfail-boom"));

        Assert.Equal(0, fx.Unit.GetMySellFailItemsCount("H"));
        Assert.Equal(new[] { "sellfail-boom" }, fx.Log.Messages.ToArray());
    }

    // ==================================================================
    // 11. GetMyBuyOKItemsCount —— 原文 1246-1267
    // ==================================================================

    /// <summary>MySqlAuctionDB.pas:1254-1257 —— 无行 → 0。</summary>
    [Fact]
    public void GetMyBuyOKItemsCount_NoRow_ReturnsZero()
    {
        var fx = NewMySql();

        Assert.Equal(0, fx.Unit.GetMyBuyOKItemsCount("Alice"));
    }

    /// <summary>MySqlAuctionDB.pas:1256 —— 原值返回。</summary>
    [Theory]
    [InlineData(0)]
    [InlineData(9)]
    [InlineData(int.MaxValue)]
    public void GetMyBuyOKItemsCount_ReturnsRawCount(int count)
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMyBuyOKItemsCount").AddRow(count);

        Assert.Equal(count, fx.Unit.GetMyBuyOKItemsCount("H"));
    }

    /// <summary>MySqlAuctionDB.pas:1253/1258-1259 —— 绑定 + 两次 Reset。</summary>
    [Fact]
    public void GetMyBuyOKItemsCount_BindsHumanName()
    {
        var fx = NewMySql();

        fx.Unit.GetMyBuyOKItemsCount(Injection);

        var sm = fx.Db.For("Auction_GetMyBuyOKItemsCount");
        Assert.Equal(Injection, sm.LastBinds[0].Text);
        Assert.Equal(2, sm.ResetCount);
    }

    /// <summary>MySqlAuctionDB.pas:1261-1265 —— 自身 except → 裸消息。</summary>
    [Fact]
    public void GetMyBuyOKItemsCount_QueryThrows_LogsAndReturnsZero()
    {
        var fx = NewMySqlThrowing("Auction_GetMyBuyOKItemsCount",
            new InvalidOperationException("buyok-boom"));

        Assert.Equal(0, fx.Unit.GetMyBuyOKItemsCount("H"));
        Assert.Equal(new[] { "buyok-boom" }, fx.Log.Messages.ToArray());
    }

    // ==================================================================
    // 12. AddAuctionItem —— 原文 441-566
    // ==================================================================

    /// <summary>MySqlAuctionDB.pas:450-457/524-557 —— 取最大 ID、插入、SaveItemToDB、Commit，返回 AuctionID。</summary>
    [Fact]
    public void AddAuctionItem_HappyPath_InsertsSavesAndCommits_ReturningTheAuctionId()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMaxAuctionID").AddRow(77);
        var std = new FakeAuctionStdItem { StdMode = 5, Color = 9, DBName = "WoodenSword" };

        int id = fx.Unit.AddAuctionItem("Alice", 24, 100, 500, 2, MakeUserItem(0, "", 0), std);

        Assert.Equal(77, id);
        Assert.Equal(new[] { "begin", "commit" }, fx.Db.Transactions.ToArray());   // 原文 521/557
        Assert.Equal(new[] { "77/5/0/0" }, fx.Host.SavedItems.ToArray());          // 原文 549
        Assert.Equal(2, fx.Db.For("Auction_GetMaxAuctionID").ResetCount);          // 450 + 456 finally
    }

    /// <summary>MySqlAuctionDB.pas:525-545 —— 10 个绑定参数按原文顺序
    /// （AuctionID, HumanName, ItemGroup, ItemColor, AuctionTime, StartingPrice, SellingPrice,
    ///   CurrencyType, ItemDBName, ItemName）。</summary>
    [Fact]
    public void AddAuctionItem_BindsTenParametersInOriginalOrder()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMaxAuctionID").AddRow(10);
        var std = new FakeAuctionStdItem { StdMode = 5, Color = 9, DBName = "Sword" };

        fx.Unit.AddAuctionItem("Alice", 24, 100, 500, 2, MakeUserItem(0, "", 0), std);

        var b = fx.Db.For("Auction_InsertAuctionItem").LastBinds;
        Assert.Equal(new[] { "int", "text", "int", "int", "int", "int", "int", "int", "text", "text" },
            b.Select(x => x.Kind).ToArray());
        Assert.Equal(10, Convert.ToInt32(b[0].Value));
        Assert.Equal("Alice", b[1].Text);
        Assert.Equal(1, Convert.ToInt32(b[2].Value));    // StdMode=5 → igWeapon = 1
        Assert.Equal(9, Convert.ToInt32(b[3].Value));    // btColor=0 → 用 StdItem.Color
        Assert.Equal(24, Convert.ToInt32(b[4].Value));
        Assert.Equal(100, Convert.ToInt32(b[5].Value));
        Assert.Equal(500, Convert.ToInt32(b[6].Value));
        Assert.Equal(2, Convert.ToInt32(b[7].Value));
        Assert.Equal("Sword", b[8].Text);
        Assert.Equal("", b[9].Text);
    }

    /// <summary>MySqlAuctionDB.pas:459-517 —— StdMode/OverLap/Shape → TItemGroup（分支顺序逐字保留）。</summary>
    [Theory]
    [InlineData(10, 0, "igDress")]        // 460-461 衣服
    [InlineData(11, 0, "igDress")]
    [InlineData(5, 0, "igWeapon")]        // 462-463 武器
    [InlineData(6, 0, "igWeapon")]
    [InlineData(28, 0, "igSpecial")]      // ★ 464-465「照明物」先吃 28 → 508-509「马牌」是死代码
    [InlineData(30, 0, "igSpecial")]
    [InlineData(19, 0, "igNecklace")]     // 466-472 项链
    [InlineData(19, 2, "igSpecial")]
    [InlineData(15, 0, "igHelmet")]       // 473-479 头盔
    [InlineData(15, 4, "igSpecial")]
    [InlineData(78, 2, "igHelmet")]       // StdMode=78 时 OverLap 不参与判定
    [InlineData(24, 0, "igArmRing")]      // 480-486 手镯
    [InlineData(26, 4, "igSpecial")]
    [InlineData(22, 0, "igRing")]         // 487-493 戒指
    [InlineData(23, 6, "igSpecial")]
    [InlineData(25, 0, "igSpecial")]      // 494-495 符毒
    [InlineData(54, 0, "igBelt")]         // 496-497 腰带
    [InlineData(52, 0, "igBoots")]        // 498-499 靴子
    [InlineData(53, 0, "igSpecial")]      // 500-501 宝石
    [InlineData(66, 0, "igFashion")]      // 502-503 时装 66..89
    [InlineData(89, 0, "igFashion")]
    [InlineData(16, 0, "igSpecial")]      // 504-505 斗笠
    [InlineData(65, 0, "igSpecial")]      // 506-507 军鼓
    [InlineData(12, 0, "igSpecial")]      // 510-511 盾牌
    [InlineData(90, 0, "igSpecial")]      // 512-513 灵玉
    [InlineData(0, 0, "igDrug")]          // 514-515 药品
    [InlineData(4, 0, "igSpecial")]       // 516-517 技能书籍
    [InlineData(99, 0, "igOther")]        // 任何分支都不命中 → igOther
    public void AddAuctionItem_MapsStdModeToItemGroup(int stdMode, int overLap, string expectedGroupName)
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMaxAuctionID").AddRow(1);
        var std = new FakeAuctionStdItem { StdMode = stdMode, OverLap = overLap, DBName = "X" };

        fx.Unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0), std);

        Assert.Equal(ItemGroupOrdinal[expectedGroupName],
            Convert.ToInt32(fx.Db.For("Auction_InsertAuctionItem").LastBinds[2].Value));
    }

    /// <summary>MySqlAuctionDB.pas:514 —— StdMode = 3 只有 Shape = 12 才算药品。</summary>
    [Fact]
    public void AddAuctionItem_DrugRequiresShape12_WhenStdModeIsThree()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMaxAuctionID").AddRow(1);

        fx.Unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0),
            new FakeAuctionStdItem { StdMode = 3, Shape = 12 });
        Assert.Equal(10, Convert.ToInt32(fx.Db.For("Auction_InsertAuctionItem").LastBinds[2].Value));

        fx.Unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0),
            new FakeAuctionStdItem { StdMode = 3, Shape = 99 });
        Assert.Equal(12, Convert.ToInt32(fx.Db.For("Auction_InsertAuctionItem").LastBinds[2].Value));
    }

    /// <summary>MySqlAuctionDB.pas:508-509 —— ★ 原文缺陷：28 在 464-465 已被判为 igSpecial，
    /// 「马牌」分支永不可达（与 30 同分支）。</summary>
    [Fact]
    public void AddAuctionItem_StdMode28_IsSpecialNotHorseCard_MirroringTheDeadBranch()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMaxAuctionID").AddRow(1);

        fx.Unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0), new FakeAuctionStdItem { StdMode = 28 });
        Assert.Equal(11, Convert.ToInt32(fx.Db.For("Auction_InsertAuctionItem").LastBinds[2].Value));

        fx.Unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0), new FakeAuctionStdItem { StdMode = 30 });
        Assert.Equal(11, Convert.ToInt32(fx.Db.For("Auction_InsertAuctionItem").LastBinds[2].Value));
    }

    /// <summary>MySqlAuctionDB.pas:529-532 —— btColor &gt; 0 用物品色，否则用 StdItem.Color。</summary>
    [Fact]
    public void AddAuctionItem_UserItemColorWinsWhenGreaterThanZero()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMaxAuctionID").AddRow(1);
        var std = new FakeAuctionStdItem { StdMode = 5, Color = 9, DBName = "S" };

        fx.Unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 200), std);
        Assert.Equal(200, Convert.ToInt32(fx.Db.For("Auction_InsertAuctionItem").LastBinds[3].Value));

        fx.Unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0), std);
        Assert.Equal(9, Convert.ToInt32(fx.Db.For("Auction_InsertAuctionItem").LastBinds[3].Value));
    }

    /// <summary>MySqlAuctionDB.pas:541-545 —— btValue[13] = 1 且 Name 非空时用 ProcessItemName 改名。</summary>
    [Fact]
    public void AddAuctionItem_ChangeNameUsedWhenBtValue13IsOneAndNameIsNotEmpty()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMaxAuctionID").AddRow(1);
        string renamed = null;
        DbLayerRunSeam.ProcessItemName = n => { renamed = n; return "[改名]" + n; };

        try
        {
            fx.Unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(1, "神剑", 0),
                new FakeAuctionStdItem { StdMode = 5, DBName = "S" });

            Assert.Equal("神剑", renamed);
            Assert.Equal("[改名]神剑", fx.Db.For("Auction_InsertAuctionItem").LastBinds[9].Text);
        }
        finally
        {
            DbLayerRunSeam.ResetDefaults();
        }
    }

    /// <summary>MySqlAuctionDB.pas:542 —— ★ 判据是 btValue[13]，**不是** btValue[0]。</summary>
    [Fact]
    public void AddAuctionItem_BtValue13IsTheJudgement_NotBtValue0()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMaxAuctionID").AddRow(1);
        DbLayerRunSeam.ProcessItemName = _ => throw new InvalidOperationException("ProcessItemName must not be called");

        try
        {
            var item = MakeUserItem(0, "神剑", 0);
            M2ItemDbAccess.SetValue(ref item, 0, 1);   // btValue[0]=1，btValue[13]=0

            fx.Unit.AddAuctionItem("H", 1, 1, 1, 0, item, new FakeAuctionStdItem { StdMode = 5, DBName = "S" });

            Assert.Equal("", fx.Db.For("Auction_InsertAuctionItem").LastBinds[9].Text);
        }
        finally
        {
            DbLayerRunSeam.ResetDefaults();
        }
    }

    /// <summary>MySqlAuctionDB.pas:542 —— btValue[13]=1 但 Name 为空时**不**改名。</summary>
    [Fact]
    public void AddAuctionItem_ChangeNameEmptyWhenBtValue13IsOneButNameIsEmpty()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMaxAuctionID").AddRow(1);
        DbLayerRunSeam.ProcessItemName = _ => throw new InvalidOperationException("ProcessItemName must not be called");

        try
        {
            fx.Unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(1, "", 0),
                new FakeAuctionStdItem { StdMode = 5, DBName = "S" });

            Assert.Equal("", fx.Db.For("Auction_InsertAuctionItem").LastBinds[9].Text);
        }
        finally
        {
            DbLayerRunSeam.ResetDefaults();
        }
    }

    /// <summary>MySqlAuctionDB.pas:451-454 —— Query 为真但 Fetch 为假（无行）→ AuctionID := 1。</summary>
    [Fact]
    public void AddAuctionItem_NoMaxRow_UsesOneAndInserts()
    {
        var fx = NewMySql();
        var std = new FakeAuctionStdItem { StdMode = 5, DBName = "S" };

        int id = fx.Unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0), std);

        Assert.Equal(1, id);
        Assert.Equal(1, Convert.ToInt32(fx.Db.For("Auction_InsertAuctionItem").LastBinds[0].Value));
        Assert.Single(fx.Host.SavedItems);
    }

    /// <summary>★ MySQL 方言（原文 451）：`if Query and Fetch then ... else AuctionID := 1` ——
    /// Query 返回 false 时同样落到 else，取 1。</summary>
    [Fact]
    public void AddAuctionItem_MaxIdQueryReturnsFalse_UsesOneAndInserts()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMaxAuctionID").QueryResult = false;

        int id = fx.Unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0),
            new FakeAuctionStdItem { StdMode = 5, DBName = "S" });

        Assert.Equal(1, id);
        Assert.Equal(new[] { "begin", "commit" }, fx.Db.Transactions.ToArray());
    }

    /// <summary>MySqlAuctionDB.pas:519-565 —— ★ 原文缺陷：`if AuctionID &gt; 0 then` **没有 else**，
    /// GetMaxAuctionID 列值 0 时静默返回 0（不插入、不开事务、不保存、不报错）。</summary>
    [Fact]
    public void AddAuctionItem_ZeroAuctionId_SilentlyReturnsZeroWithoutInsert()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMaxAuctionID").AddRow(0);

        int id = fx.Unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0),
            new FakeAuctionStdItem { StdMode = 5, DBName = "S" });

        Assert.Equal(0, id);
        Assert.Empty(fx.Db.Transactions);
        Assert.Equal(0, fx.Db.For("Auction_InsertAuctionItem").StepCount);
        Assert.Equal(0, fx.Db.For("Auction_InsertAuctionItem").ResetCount);
        Assert.Empty(fx.Host.SavedItems);
        Assert.Empty(fx.Log.Messages);
    }

    /// <summary>MySqlAuctionDB.pas:558-563 —— 插入抛错 → MainOutMessage + RollBack，Result 保持 0；
    /// 原文 553-554 的 finally 仍 Reset（524 + 554 = 2 次）。</summary>
    [Fact]
    public void AddAuctionItem_InsertStepThrows_RollsBackAndReturnsZero()
    {
        var fx = NewMySqlThrowing("Auction_InsertAuctionItem", new InvalidOperationException("insert-boom"));
        fx.Db.For("Auction_GetMaxAuctionID").AddRow(5);

        int id = fx.Unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0),
            new FakeAuctionStdItem { StdMode = 5, DBName = "S" });

        Assert.Equal(0, id);
        Assert.Equal(new[] { "insert-boom" }, fx.Log.Messages.ToArray());
        Assert.Equal(new[] { "begin", "rollback" }, fx.Db.Transactions.ToArray());
        Assert.Empty(fx.Host.SavedItems);
        Assert.Equal(2, fx.Db.For("Auction_InsertAuctionItem").ResetCount);
    }

    /// <summary>★ MySQL 方言（原文 547）：成功判据是 <c>Step()</c> 返回 true；返回 false 时
    /// 不 SaveItemToDB、不设 Result，但原文 557 的 Commit 在 if 之外**仍执行**。</summary>
    [Fact]
    public void AddAuctionItem_InsertStepReturnsFalse_StillCommitsWithoutSaving()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMaxAuctionID").AddRow(5);
        fx.Db.For("Auction_InsertAuctionItem").QueryResult = false;

        int id = fx.Unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0),
            new FakeAuctionStdItem { StdMode = 5, DBName = "S" });

        Assert.Equal(0, id);
        Assert.Empty(fx.Host.SavedItems);
        Assert.Equal(new[] { "begin", "commit" }, fx.Db.Transactions.ToArray());
        Assert.Equal(1, fx.Db.For("Auction_InsertAuctionItem").StepCount);
    }

    /// <summary>MySqlAuctionDB.pas:526/539 —— HumanName 与 StdItem.DBName 都走 OrderBindParamText。</summary>
    [Fact]
    public void AddAuctionItem_SqlInjectionInHumanNameAndDbName_AreBound()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMaxAuctionID").AddRow(1);

        fx.Unit.AddAuctionItem(Injection, 1, 1, 1, 0, MakeUserItem(0, "", 0),
            new FakeAuctionStdItem { StdMode = 5, DBName = Injection });

        var b = fx.Db.For("Auction_InsertAuctionItem").LastBinds;
        Assert.Equal(Injection, b[1].Text);
        Assert.Equal(Injection, b[8].Text);
        Assert.DoesNotContain(Injection, fx.Db.For("Auction_InsertAuctionItem").Sql);
    }

    /// <summary>MySqlAuctionDB.pas:524 + 553-554 —— 插入语句 Reset 两次。</summary>
    [Fact]
    public void AddAuctionItem_ResetsTheInsertStatementInFinally()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMaxAuctionID").AddRow(1);

        fx.Unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0), new FakeAuctionStdItem { StdMode = 5 });

        Assert.Equal(2, fx.Db.For("Auction_InsertAuctionItem").ResetCount);
    }

    /// <summary>MySqlAuctionDB.pas:549 —— SaveItemToDB 的 ItemType 是 AUCTION_ITEM_TYPE = 5。</summary>
    [Fact]
    public void AddAuctionItem_SavesWithAuctionItemTypeFive()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMaxAuctionID").AddRow(33);

        fx.Unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0), new FakeAuctionStdItem { StdMode = 5 });

        Assert.Equal("33/5/0/0", fx.Host.SavedItems[0]);
        Assert.Equal(AuctionItemType, 5);
    }

    /// <summary>接缝补丁（原文 441-566 **没有**外层 except：StdItem 为 nil 时原文直接 AV）：
    /// MySqlAuctionDB.cs:578-582 补的一层 except 会把托管侧的 NullReferenceException 记进日志并返回 0。
    /// 这里锁死"不抛到包装层 + 记裸消息 + Result 0"。</summary>
    [Fact]
    public void AddAuctionItem_NullStdItem_IsCaughtByTheAddedOuterExcept()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMaxAuctionID").AddRow(3);

        int id = fx.Unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0), null);

        Assert.Equal(0, id);
        Assert.Single(fx.Log.Messages);                       // 包装层没有再加一条 [Exception] TAuctionDB:
        Assert.DoesNotContain("[Exception]", fx.Log.Messages[0]);
        Assert.Empty(fx.Db.Transactions);                     // 还没走到 StartTransaction
        Assert.Empty(fx.Host.SavedItems);
    }

    /// <summary>DbBases.cs:873-891 的 public 包装层 Lock/UnLock。</summary>
    [Fact]
    public void AddAuctionItem_PublicWrapperLocksAndUnlocks()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_GetMaxAuctionID").AddRow(1);

        fx.Unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0), new FakeAuctionStdItem { StdMode = 5 });

        Assert.Equal(1, fx.Host.LockCount);
        Assert.Equal(1, fx.Host.UnLockCount);
    }

    // ==================================================================
    // 13. CancelAuctionItem —— 原文 568-582
    // ==================================================================

    /// <summary>MySqlAuctionDB.pas:573-574 —— 直拼串（双引号包 HumanName），Exec 后返回 True。</summary>
    [Fact]
    public void CancelAuctionItem_ExecutesTheOriginalUpdateAndReturnsTrue()
    {
        var fx = NewMySql();

        bool ok = fx.Unit.CancelAuctionItem("Alice", 42);

        Assert.True(ok);
        Assert.Equal("update AuctionData set TradingStatus = 1 where AuctionID = 42 and HumanName = \"Alice\";",
            fx.Db.ExecutedSql.Single());
        Assert.Empty(fx.Db.Transactions);
    }

    /// <summary>MySqlAuctionDB.pas:573 —— AuctionID 原样 IntToStr（0/负数/极值都不校验）。</summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public void CancelAuctionItem_PassesAnyAuctionIdThrough(int auctionId)
    {
        var fx = NewMySql();

        Assert.True(fx.Unit.CancelAuctionItem("H", auctionId));
        Assert.Contains("where AuctionID = " + auctionId + " ", fx.Db.ExecutedSql.Single());
    }

    /// <summary>MySqlAuctionDB.pas:573 —— HumanName 直拼（**无绑定**）。</summary>
    [Fact]
    public void CancelAuctionItem_SqlInjectionInHumanName_IsConcatenatedNotBound()
    {
        var fx = NewMySql();

        fx.Unit.CancelAuctionItem(Injection, 1);

        Assert.Contains("and HumanName = \"" + Injection + "\";", fx.Db.ExecutedSql.Single());
    }

    /// <summary>MySqlAuctionDB.pas:576-580 —— Exec 抛错 → MainOutMessage，Result 保持 False。</summary>
    [Fact]
    public void CancelAuctionItem_ExecThrows_LogsAndReturnsFalse()
    {
        var fx = NewMySql(executeThrow: new InvalidOperationException("cancel-boom"));

        bool ok = fx.Unit.CancelAuctionItem("H", 1);

        Assert.False(ok);
        Assert.Equal(new[] { "cancel-boom" }, fx.Log.Messages.ToArray());
    }

    /// <summary>DbBases.cs:893-904 的 public 包装层 Lock/UnLock。</summary>
    [Fact]
    public void CancelAuctionItem_PublicWrapperLocksAndUnlocks()
    {
        var fx = NewMySql();

        fx.Unit.CancelAuctionItem("H", 1);

        Assert.Equal(1, fx.Host.LockCount);
        Assert.Equal(1, fx.Host.UnLockCount);
    }

    // ==================================================================
    // 14. RetrieveAuctionItem —— 原文 584-597
    // ==================================================================

    /// <summary>MySqlAuctionDB.pas:589 —— 直拼串 + 返回 True。</summary>
    [Fact]
    public void RetrieveAuctionItem_ExecutesTheOriginalUpdateAndReturnsTrue()
    {
        var fx = NewMySql();

        Assert.True(fx.Unit.RetrieveAuctionItem(42));
        Assert.Equal("update AuctionData set IsItemGive = 1 where AuctionID = 42;", fx.Db.ExecutedSql.Single());
        Assert.Empty(fx.Db.Transactions);
    }

    /// <summary>MySqlAuctionDB.pas:589 —— AuctionID 原样透传。</summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-7)]
    public void RetrieveAuctionItem_PassesAnyAuctionIdThrough(int auctionId)
    {
        var fx = NewMySql();

        Assert.True(fx.Unit.RetrieveAuctionItem(auctionId));
        Assert.Contains("where AuctionID = " + auctionId + ";", fx.Db.ExecutedSql.Single());
    }

    /// <summary>MySqlAuctionDB.pas:591-595 —— Exec 抛错 → 裸消息日志 + False。</summary>
    [Fact]
    public void RetrieveAuctionItem_ExecThrows_LogsAndReturnsFalse()
    {
        var fx = NewMySql(executeThrow: new InvalidOperationException("retrieve-boom"));

        Assert.False(fx.Unit.RetrieveAuctionItem(1));
        Assert.Equal(new[] { "retrieve-boom" }, fx.Log.Messages.ToArray());
    }

    /// <summary>DbBases.cs:906-917 的 public 包装层 Lock/UnLock。</summary>
    [Fact]
    public void RetrieveAuctionItem_PublicWrapperLocksAndUnlocks()
    {
        var fx = NewMySql();

        fx.Unit.RetrieveAuctionItem(1);

        Assert.Equal(1, fx.Host.LockCount);
        Assert.Equal(1, fx.Host.UnLockCount);
    }

    // ==================================================================
    // 15. DeleteAuctionItem —— 原文 599-629（★ MySQL 独有 ClearResult）
    // ==================================================================

    /// <summary>MySqlAuctionDB.pas:606-619 —— 12 段脚本按原文顺序拼出（sLineBreak = CRLF），
    /// sWhere 用 11 次、IntToStr(AuctionID) 用 2 次；Exec 之后必须 ClearResult，再 Commit。</summary>
    [Fact]
    public void DeleteAuctionItem_BuildsTheScriptInOriginalOrder_AndClearsTheResult()
    {
        var fx = NewMySql();

        bool ok = fx.Unit.DeleteAuctionItem("Alice", 42);

        Assert.True(ok);
        string sql = fx.Db.ExecutedSql.Single();
        int iElement = sql.IndexOf("DELETE FROM ItemElementAdd", StringComparison.Ordinal);
        int iByte = sql.IndexOf("DELETE FROM ItemAddDataByte", StringComparison.Ordinal);
        int iInt = sql.IndexOf("DELETE FROM ItemAddDataInt", StringComparison.Ordinal);
        int iText = sql.IndexOf("DELETE FROM ItemAddDataText", StringComparison.Ordinal);
        int iFlute = sql.IndexOf("DELETE FROM ItemFlute", StringComparison.Ordinal);
        int iProgress = sql.IndexOf("DELETE FROM ItemProgress", StringComparison.Ordinal);
        int iProperty = sql.IndexOf("DELETE FROM ItemProperty", StringComparison.Ordinal);
        int iValueAdd = sql.IndexOf("DELETE FROM ItemValueAdd", StringComparison.Ordinal);
        int iItems = sql.IndexOf("DELETE FROM Items", StringComparison.Ordinal);
        int iAttention = sql.IndexOf("DELETE FROM AuctionAttention", StringComparison.Ordinal);
        int iData = sql.IndexOf("DELETE FROM AuctionData", StringComparison.Ordinal);
        Assert.True(iElement >= 0 && iElement < iByte && iByte < iInt && iInt < iText && iText < iFlute
            && iFlute < iProgress && iProgress < iProperty && iProperty < iValueAdd && iValueAdd < iItems
            && iItems < iAttention && iAttention < iData);
        Assert.Contains(" WHERE ParentID = 42 and ItemType = 5 and ItemIndex = 0;", sql);
        Assert.Contains("DELETE FROM AuctionAttention WHERE AuctionID = 42;", sql);
        Assert.Contains("DELETE FROM AuctionData WHERE AuctionID = 42;", sql);
        Assert.Contains("\r\n", sql);
        Assert.Equal(1, fx.Hook.ClearResultCount);                          // 原文 617
        Assert.Equal(new[] { "begin", "commit" }, fx.Db.Transactions.ToArray());   // 原文 607/619
    }

    /// <summary>MySqlAuctionDB.pas:602-614 —— HumanName 参数在脚本里根本没用到
    /// （脚本只由 sWhere + IntToStr(AuctionID) 组成）。</summary>
    [Fact]
    public void DeleteAuctionItem_HumanNameParameterIsUnusedInTheScript()
    {
        var fx = NewMySql();

        fx.Unit.DeleteAuctionItem("Alice", 7);

        string sql = fx.Db.ExecutedSql.Single();
        Assert.DoesNotContain("Alice", sql);
        Assert.Contains("WHERE ParentID = 7 ", sql);
    }

    /// <summary>MySqlAuctionDB.pas:606 —— 注入串换个名字进来也不进脚本。</summary>
    [Fact]
    public void DeleteAuctionItem_SqlInjectionInHumanName_HasNoEffect()
    {
        var fx = NewMySql();

        fx.Unit.DeleteAuctionItem(Injection, 1);

        Assert.DoesNotContain(Injection, fx.Db.ExecutedSql.Single());
    }

    /// <summary>MySqlAuctionDB.pas:600-614 —— AuctionID 原样 IntToStr 进 sWhere 与后两段。</summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    [InlineData(int.MaxValue)]
    public void DeleteAuctionItem_PassesAnyAuctionIdThrough(int auctionId)
    {
        var fx = NewMySql();

        Assert.True(fx.Unit.DeleteAuctionItem("H", auctionId));

        string sql = fx.Db.ExecutedSql.Single();
        Assert.Contains("WHERE ParentID = " + auctionId + " ", sql);
        Assert.Contains("DELETE FROM AuctionData WHERE AuctionID = " + auctionId + ";", sql);
    }

    /// <summary>MySqlAuctionDB.pas:622-627 —— Exec 抛错 → 裸消息 + RollBack + False，
    /// 且 **ClearResult 不会被调用**（原文 617 在异常点之后）。</summary>
    [Fact]
    public void DeleteAuctionItem_ExecThrows_RollsBackAndReturnsFalse()
    {
        var fx = NewMySql(executeThrow: new InvalidOperationException("delete-boom"));

        bool ok = fx.Unit.DeleteAuctionItem("H", 1);

        Assert.False(ok);
        Assert.Equal(new[] { "delete-boom" }, fx.Log.Messages.ToArray());
        Assert.Equal(new[] { "begin", "rollback" }, fx.Db.Transactions.ToArray());
        Assert.Equal(0, fx.Hook.ClearResultCount);
    }

    /// <summary>DbBases.cs:919-930 的 public 包装层 Lock/UnLock。</summary>
    [Fact]
    public void DeleteAuctionItem_PublicWrapperLocksAndUnlocks()
    {
        var fx = NewMySql();

        fx.Unit.DeleteAuctionItem("H", 1);

        Assert.Equal(1, fx.Host.LockCount);
        Assert.Equal(1, fx.Host.UnLockCount);
    }

    // ==================================================================
    // 16. AddAttentionItem —— 原文 631-665（★ 判据是 RowCount <> 0）
    // ==================================================================

    /// <summary>MySqlAuctionDB.pas:638-654 —— RowCount = 0 → 未关注 → 插入，Result = Step() = true。</summary>
    [Fact]
    public void AddAttentionItem_NotYetAttentioned_Inserts()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = 0;

        bool ok = fx.Unit.AddAttentionItem("Alice", 9);

        Assert.True(ok);
        Assert.Equal(1, fx.Db.For("Auction_AddAttentionItem").StepCount);
        Assert.Equal(2, fx.Db.For("Auction_CheckInAttentionItem").ResetCount);   // 638 + 645 finally
        Assert.Equal(2, fx.Db.For("Auction_AddAttentionItem").ResetCount);       // 651 + 656 finally
    }

    /// <summary>MySqlAuctionDB.pas:642/648 —— ★ RowCount &lt;&gt; 0 → 已关注 → 跳过插入，Result 保持 False。</summary>
    [Fact]
    public void AddAttentionItem_AlreadyAttentioned_SkipsTheInsert()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = 3;

        bool ok = fx.Unit.AddAttentionItem("Alice", 9);

        Assert.False(ok);
        Assert.Equal(0, fx.Db.For("Auction_AddAttentionItem").StepCount);
        Assert.Equal(0, fx.Db.For("Auction_AddAttentionItem").ResetCount);
    }

    /// <summary>★ MySQL 方言（原文 641，注释「修复查询不到结果的问题 By 一支笔 at:2022-01-06」）：
    /// 判据用 <c>Query()</c> + <c>RowCount</c>，**不**用 <c>Step()</c>（SQLite 版是 <c>Step() == ROW</c>）。</summary>
    [Fact]
    public void AddAttentionItem_CheckUsesQueryAndRowCount_NotStep()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = 1;

        fx.Unit.AddAttentionItem("H", 1);

        var check = fx.Db.For("Auction_CheckInAttentionItem");
        Assert.Equal(1, check.QueryCount);
        Assert.Equal(0, check.StepCount);
        Assert.Equal(0, check.FetchCount);      // 只看 RowCount，不取行
    }

    /// <summary>MySqlAuctionDB.pas:642 —— RowCount 为负（!= 0）也算"已关注"。</summary>
    [Fact]
    public void AddAttentionItem_NegativeRowCount_IsTreatedAsAttentioned()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = -1;

        Assert.False(fx.Unit.AddAttentionItem("H", 1));
        Assert.Equal(0, fx.Db.For("Auction_AddAttentionItem").StepCount);
    }

    /// <summary>MySqlAuctionDB.pas:639-640/652-653 —— 两条语句都是 OrderBindParamText(HumanName) + Int。</summary>
    [Fact]
    public void AddAttentionItem_BindsHumanNameThenIndexInBothStatements()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = 0;

        fx.Unit.AddAttentionItem("Alice", 9);

        foreach (string label in new[] { "Auction_CheckInAttentionItem", "Auction_AddAttentionItem" })
        {
            var b = fx.Db.For(label).LastBinds;
            Assert.Equal(new[] { "text", "int" }, b.Select(x => x.Kind).ToArray());
            Assert.Equal("Alice", b[0].Text);
            Assert.Equal(9, Convert.ToInt32(b[1].Value));
        }
    }

    /// <summary>★ MySQL 方言（原文 654）：<c>Result := FStatementInsertAttentionItem.Step</c>（bool）——
    /// 返回 false 时 Result 为 False。</summary>
    [Fact]
    public void AddAttentionItem_InsertStepReturnsFalse_IsNotSuccess()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = 0;
        fx.Db.For("Auction_AddAttentionItem").QueryResult = false;

        Assert.False(fx.Unit.AddAttentionItem("H", 1));
        Assert.Equal(1, fx.Db.For("Auction_AddAttentionItem").StepCount);
    }

    /// <summary>MySqlAuctionDB.pas:639/652 —— 注入串走绑定，不进 SQL。</summary>
    [Fact]
    public void AddAttentionItem_SqlInjectionInHumanName_IsBound()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = 0;

        fx.Unit.AddAttentionItem(Injection, 1);

        Assert.Equal(Injection, fx.Db.For("Auction_CheckInAttentionItem").LastBinds[0].Text);
        Assert.Equal(Injection, fx.Db.For("Auction_AddAttentionItem").LastBinds[0].Text);
        Assert.DoesNotContain(Injection, fx.Db.For("Auction_AddAttentionItem").Sql);
    }

    /// <summary>MySqlAuctionDB.pas:659-663 —— 检查语句抛错 → 裸消息，Result 保持 False，不插入。</summary>
    [Fact]
    public void AddAttentionItem_CheckQueryThrows_LogsAndReturnsFalse()
    {
        var fx = NewMySqlThrowing("Auction_CheckInAttentionItem", new InvalidOperationException("check-boom"));

        Assert.False(fx.Unit.AddAttentionItem("H", 1));
        Assert.Equal(new[] { "check-boom" }, fx.Log.Messages.ToArray());
        Assert.Equal(0, fx.Db.For("Auction_AddAttentionItem").StepCount);
        Assert.Equal(2, fx.Db.For("Auction_CheckInAttentionItem").ResetCount);   // 679 + 688 finally
    }

    /// <summary>MySqlAuctionDB.pas:659-663 —— 插入语句抛错 → 裸消息 + False（检查段的 finally 已 Reset）。</summary>
    [Fact]
    public void AddAttentionItem_InsertStepThrows_LogsAndReturnsFalse()
    {
        var fx = NewMySqlThrowing("Auction_AddAttentionItem", new InvalidOperationException("insert-att-boom"));
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = 0;

        Assert.False(fx.Unit.AddAttentionItem("H", 1));
        Assert.Equal(new[] { "insert-att-boom" }, fx.Log.Messages.ToArray());
        Assert.Equal(2, fx.Db.For("Auction_CheckInAttentionItem").ResetCount);
        Assert.Equal(2, fx.Db.For("Auction_AddAttentionItem").ResetCount);   // 695 + 702 finally
    }

    /// <summary>DbBases.cs:932-943 的 public 包装层 Lock/UnLock。</summary>
    [Fact]
    public void AddAttentionItem_PublicWrapperLocksAndUnlocks()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = 0;

        fx.Unit.AddAttentionItem("H", 1);

        Assert.Equal(1, fx.Host.LockCount);
        Assert.Equal(1, fx.Host.UnLockCount);
    }

    // ==================================================================
    // 17. DeleteAttentionItem —— 原文 667-701
    // ==================================================================

    /// <summary>MySqlAuctionDB.pas:678/684-690 —— RowCount != 0 → 删除，Result = Step() = true。</summary>
    [Fact]
    public void DeleteAttentionItem_Attentioned_Deletes()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = 1;

        bool ok = fx.Unit.DeleteAttentionItem("Alice", 9);

        Assert.True(ok);
        Assert.Equal(1, fx.Db.For("Auction_DeleteAuctionItem").StepCount);
        Assert.Equal(2, fx.Db.For("Auction_DeleteAuctionItem").ResetCount);
    }

    /// <summary>MySqlAuctionDB.pas:684 —— RowCount = 0 → 跳过删除，Result 保持 False。</summary>
    [Fact]
    public void DeleteAttentionItem_NotAttentioned_SkipsTheDelete()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = 0;

        Assert.False(fx.Unit.DeleteAttentionItem("Alice", 9));
        Assert.Equal(0, fx.Db.For("Auction_DeleteAuctionItem").StepCount);
    }

    /// <summary>★ 原文 262 —— 删除关注物品复用的语句 label 是 <c>Auction_DeleteAuctionItem</c>
    /// （**不是** Auction_DeleteAttentionItem），DoInit 里共 21 条且不含后者。</summary>
    [Fact]
    public void DeleteAttentionItem_UsesTheAuctionDeleteAuctionItemLabel()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = 1;

        fx.Unit.DeleteAttentionItem("H", 1);

        Assert.True(fx.Db.Has("Auction_DeleteAuctionItem"));
        Assert.DoesNotContain("Auction_DeleteAttentionItem", fx.Db.AddedNames);
        Assert.Contains("Auction_DeleteAuctionItem", DoInitStatementLabels);
    }

    /// <summary>MySqlAuctionDB.pas:684 —— RowCount 为负（!= 0）也删除。</summary>
    [Fact]
    public void DeleteAttentionItem_NegativeRowCount_Deletes()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = -1;

        Assert.True(fx.Unit.DeleteAttentionItem("H", 1));
        Assert.Equal(1, fx.Db.For("Auction_DeleteAuctionItem").StepCount);
    }

    /// <summary>MySqlAuctionDB.pas:675-676/688-689 —— 绑定顺序 text、int。</summary>
    [Fact]
    public void DeleteAttentionItem_BindsHumanNameThenIndex()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = 1;

        fx.Unit.DeleteAttentionItem(Injection, 5);

        var b = fx.Db.For("Auction_DeleteAuctionItem").LastBinds;
        Assert.Equal(new[] { "text", "int" }, b.Select(x => x.Kind).ToArray());
        Assert.Equal(Injection, b[0].Text);
        Assert.Equal(5, Convert.ToInt32(b[1].Value));
        Assert.DoesNotContain(Injection, fx.Db.For("Auction_DeleteAuctionItem").Sql);
    }

    /// <summary>MySqlAuctionDB.pas:695-699 —— 检查抛错 → 裸消息 + False。</summary>
    [Fact]
    public void DeleteAttentionItem_CheckQueryThrows_LogsAndReturnsFalse()
    {
        var fx = NewMySqlThrowing("Auction_CheckInAttentionItem", new InvalidOperationException("del-check-boom"));

        Assert.False(fx.Unit.DeleteAttentionItem("H", 1));
        Assert.Equal(new[] { "del-check-boom" }, fx.Log.Messages.ToArray());
    }

    /// <summary>★ MySQL 方言（原文 690）：删除 Step 返回 false → Result False（语句确实被执行了）。</summary>
    [Fact]
    public void DeleteAttentionItem_DeleteStepReturnsFalse_IsNotSuccess()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = 1;
        fx.Db.For("Auction_DeleteAuctionItem").QueryResult = false;

        Assert.False(fx.Unit.DeleteAttentionItem("H", 1));
        Assert.Equal(1, fx.Db.For("Auction_DeleteAuctionItem").StepCount);
    }

    // ==================================================================
    // 18. JoinItemBid —— 原文 703-739（★ 调基类 public AddAttentionItem → 二次加锁）
    // ==================================================================

    /// <summary>MySqlAuctionDB.pas:709-719 —— IsSell = true 走一口价语句 Auction_BuyItem。</summary>
    [Fact]
    public void JoinItemBid_IsSell_UsesTheBuyItemStatement()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = 1;

        bool ok = fx.Unit.JoinItemBid("Alice", 3, 500, true);

        Assert.True(ok);
        Assert.Equal(1, fx.Db.For("Auction_BuyItem").StepCount);
        Assert.Equal(0, fx.Db.For("Auction_JoinItemBid").StepCount);
        Assert.Equal(2, fx.Db.For("Auction_BuyItem").ResetCount);   // 712 + 718 finally
    }

    /// <summary>MySqlAuctionDB.pas:723-731 —— IsSell = false 走竞价语句 Auction_JoinItemBid。</summary>
    [Fact]
    public void JoinItemBid_NotSell_UsesTheJoinItemBidStatement()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = 1;

        bool ok = fx.Unit.JoinItemBid("Alice", 3, 500, false);

        Assert.True(ok);
        Assert.Equal(1, fx.Db.For("Auction_JoinItemBid").StepCount);
        Assert.Equal(0, fx.Db.For("Auction_BuyItem").StepCount);
    }

    /// <summary>MySqlAuctionDB.pas:707 —— 先无条件 AddAttentionItem(HumanName, Index)。</summary>
    [Fact]
    public void JoinItemBid_AlwaysAddsAttentionFirst()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = 1;

        fx.Unit.JoinItemBid("Alice", 3, 500, false);

        var check = fx.Db.For("Auction_CheckInAttentionItem");
        Assert.Equal(1, check.QueryCount);
        Assert.Equal("Alice", check.LastBinds[0].Text);
        Assert.Equal(3, Convert.ToInt32(check.LastBinds[1].Value));
    }

    /// <summary>MySqlAuctionDB.pas:707 —— ★ 调的是基类 public 包装 <c>AddAttentionItem</c>（带锁），
    /// 与 DoAddAttentionItem（无额外锁）不同 → Lock 两次。</summary>
    [Fact]
    public void JoinItemBid_LocksTwice_BecauseItCallsThePublicAddAttentionItemWrapper()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = 1;

        fx.Unit.JoinItemBid("H", 1, 1, false);

        Assert.Equal(2, fx.Host.LockCount);
        Assert.Equal(2, fx.Host.UnLockCount);
    }

    /// <summary>对照：直接调 public AddAttentionItem 只锁一次。</summary>
    [Fact]
    public void AddAttentionItem_DirectCall_LocksOnlyOnce_UnlikeJoinItemBid()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = 1;

        fx.Unit.AddAttentionItem("H", 1);

        Assert.Equal(1, fx.Host.LockCount);
    }

    /// <summary>★ MySQL 方言（原文 716/728）：Step 返回 false → Result False。</summary>
    [Fact]
    public void JoinItemBid_StepReturnsFalse_IsNotSuccess()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = 1;
        fx.Db.For("Auction_JoinItemBid").QueryResult = false;

        Assert.False(fx.Unit.JoinItemBid("H", 1, 1, false));
    }

    /// <summary>MySqlAuctionDB.pas:714/726 —— Prices 与 Index 原样绑定（负数/0 不校验）。</summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public void JoinItemBid_NegativeAndZeroPrices_ArePassedThrough(int prices)
    {
        var fx = NewMySql();
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = 1;

        fx.Unit.JoinItemBid("H", 7, prices, false);

        var b = fx.Db.For("Auction_JoinItemBid").LastBinds;
        Assert.Equal("H", b[0].Text);
        Assert.Equal(prices, Convert.ToInt32(b[1].Value));
        Assert.Equal(7, Convert.ToInt32(b[2].Value));
    }

    /// <summary>MySqlAuctionDB.pas:713/725 —— HumanName 走绑定。</summary>
    [Fact]
    public void JoinItemBid_SqlInjectionInHumanName_IsBound()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = 1;

        fx.Unit.JoinItemBid(Injection, 1, 1, true);

        Assert.Equal(Injection, fx.Db.For("Auction_BuyItem").LastBinds[0].Text);
        Assert.DoesNotContain(Injection, fx.Db.For("Auction_BuyItem").Sql);
    }

    /// <summary>MySqlAuctionDB.pas:733-737 —— 竞价语句抛错 → 裸消息 + False。</summary>
    [Fact]
    public void JoinItemBid_BidStepThrows_LogsAndReturnsFalse()
    {
        var fx = NewMySqlThrowing("Auction_JoinItemBid", new InvalidOperationException("bid-boom"));
        fx.Db.For("Auction_CheckInAttentionItem").RowCount = 1;

        Assert.False(fx.Unit.JoinItemBid("H", 1, 1, false));
        Assert.Equal(new[] { "bid-boom" }, fx.Log.Messages.ToArray());
        // 外层 public JoinItemBid 锁一次 + 内层 public AddAttentionItem 锁一次 = 2。
        Assert.Equal(2, fx.Host.LockCount);
        Assert.Equal(2, fx.Host.UnLockCount);
    }

    // ==================================================================
    // 19. GetAuctionInfo —— 原文 998-1031
    // ==================================================================

    /// <summary>MySqlAuctionDB.pas:1005-1021 —— 无行 → False。</summary>
    [Fact]
    public void GetAuctionInfo_NoRow_ReturnsFalse()
    {
        var fx = NewMySql();

        bool ok = fx.Unit.GetAuctionInfo(7, out TAuctionInfo info);

        Assert.False(ok);
        Assert.Equal(0, info.AuctionID);
        Assert.Equal("", info.HumanName);
        Assert.Equal(2, fx.Db.For("Auction_QueryOneItem").ResetCount);   // 1003 + 1023 finally
    }

    /// <summary>MySqlAuctionDB.pas:1007-1018 —— 12 个字段逐列读取（AddDateTime 走 DATETIME）。</summary>
    [Fact]
    public void GetAuctionInfo_Row_ReturnsTrueAndFillsAllFields()
    {
        var fx = NewMySql();
        var t = new DateTime(2023, 12, 31, 23, 59, 58, DateTimeKind.Unspecified);
        fx.Db.For("Auction_QueryOneItem").AddRow(OneItemRow("Seller", t, 6, 60, 7, 8, 3, 9, "Bidder", 2, 1));

        bool ok = fx.Unit.GetAuctionInfo(77, out TAuctionInfo info);

        Assert.True(ok);
        Assert.Equal(77, info.AuctionID);
        Assert.Equal("Seller", info.HumanName);
        Assert.Equal(t, info.AddDateTime);
        Assert.Equal(6, info.AuctionTime);
        Assert.Equal(60, info.TimeLeft);
        Assert.Equal(7u, info.StartingPrice);
        Assert.Equal(8u, info.SellingPrice);
        Assert.Equal(3, info.CurrencyType);
        Assert.Equal(9, info.LastBidPrice);
        Assert.Equal("Bidder", info.LastBidder);
        Assert.Equal(2, info.TradingStatus);
        Assert.True(info.IsItemGive);
    }

    /// <summary>MySqlAuctionDB.pas:1007 —— AuctionID 来自**入参** Index，不是结果集列
    /// （Auction_QueryOneItem 的 select 里没有 AuctionID 列，原文 215-218）。</summary>
    [Fact]
    public void GetAuctionInfo_AuctionIdComesFromTheParameterNotTheFirstColumn()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryOneItem").AddRow(OneItemRow("Whoever", 0L, 1, 2, 3, 4, 0, 5, "B", 0, 0));

        fx.Unit.GetAuctionInfo(99, out TAuctionInfo info);

        Assert.Equal(99, info.AuctionID);
    }

    /// <summary>MySqlAuctionDB.pas:1004/1022-1023 —— 绑定 Index，且 finally Reset。</summary>
    [Fact]
    public void GetAuctionInfo_BindsIndexAndResetsInFinally()
    {
        var fx = NewMySql();

        fx.Unit.GetAuctionInfo(-4, out TAuctionInfo _);

        var sm = fx.Db.For("Auction_QueryOneItem");
        Assert.Equal(new[] { "int" }, sm.LastBinds.Select(b => b.Kind).ToArray());
        Assert.Equal(-4, Convert.ToInt32(sm.LastBinds[0].Value));
        Assert.Equal(2, sm.ResetCount);
    }

    /// <summary>★ MySQL 方言（原文 1005）：`if Query and Fetch then` —— Query 假 → False。</summary>
    [Fact]
    public void GetAuctionInfo_QueryReturnsFalse_ReturnsFalse()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryOneItem").QueryResult = false;

        Assert.False(fx.Unit.GetAuctionInfo(1, out TAuctionInfo _));
        Assert.Equal(0, fx.Db.For("Auction_QueryOneItem").FetchCount);
    }

    /// <summary>MySqlAuctionDB.pas:1025-1029 —— 抛错 → 裸消息 + False。</summary>
    [Fact]
    public void GetAuctionInfo_QueryThrows_LogsAndReturnsFalse()
    {
        var fx = NewMySqlThrowing("Auction_QueryOneItem", new InvalidOperationException("info-boom"));

        bool ok = fx.Unit.GetAuctionInfo(1, out TAuctionInfo info);

        Assert.False(ok);
        Assert.Equal(0, info.AuctionID);
        Assert.Equal(new[] { "info-boom" }, fx.Log.Messages.ToArray());
    }

    // ==================================================================
    // 20. GetAuctionRecord —— 原文 1033-1069
    // ==================================================================

    /// <summary>MySqlAuctionDB.pas:1040-1058 —— 无行 → False，不加载物品。</summary>
    [Fact]
    public void GetAuctionRecord_NoRow_ReturnsFalseAndDoesNotLoad()
    {
        var fx = NewMySql();

        bool ok = fx.Unit.GetAuctionRecord(7, out TAuctionRecord rec);

        Assert.False(ok);
        Assert.Equal(0, rec.AuctionID);
        Assert.Empty(fx.Host.LoadedItems);
    }

    /// <summary>MySqlAuctionDB.pas:1042-1056 —— 有行 → True，逐字段填充并 LoadItemFromDB。</summary>
    [Fact]
    public void GetAuctionRecord_Row_ReturnsTrueAndLoadsTheItem()
    {
        var fx = NewMySql();
        var t = new DateTime(2022, 2, 2, 2, 2, 2, DateTimeKind.Unspecified);
        fx.Db.For("Auction_QueryOneItem").AddRow(OneItemRow("Seller", t, 6, 60, 7, 8, 3, 9, "Bidder", 2, 1));

        bool ok = fx.Unit.GetAuctionRecord(55, out TAuctionRecord rec);

        Assert.True(ok);
        Assert.Equal(55, rec.AuctionID);           // 来自入参（原文 1042）
        Assert.Equal("Seller", rec.HumanName);
        Assert.Equal(t, rec.AddDateTime);
        Assert.Equal(6, rec.AuctionTime);
        Assert.Equal(60, rec.TimeLeft);
        Assert.Equal(7u, rec.StartingPrice);
        Assert.Equal(8u, rec.SellingPrice);
        Assert.Equal(3, rec.CurrencyType);
        Assert.Equal(9, rec.LastBidPrice);
        Assert.Equal("Bidder", rec.LastBidder);
        Assert.Equal(2, rec.TradingStatus);
        Assert.True(rec.IsItemGive);
        Assert.Equal(new[] { "55/5/0" }, fx.Host.LoadedItems.ToArray());
    }

    /// <summary>MySqlAuctionDB.pas:1054 —— IsAttention 恒为 False（原文显式赋值）。</summary>
    [Fact]
    public void GetAuctionRecord_IsAttentionIsAlwaysFalse()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryOneItem").AddRow(OneItemRow("S", 0L, 1, 2, 3, 4, 0, 5, "B", 0, 0));

        fx.Unit.GetAuctionRecord(1, out TAuctionRecord rec);

        Assert.False(rec.IsAttention);
    }

    /// <summary>MySqlAuctionDB.pas:1039/1060-1061 —— 绑定 Index + 两次 Reset。</summary>
    [Fact]
    public void GetAuctionRecord_BindsIndexAndResetsInFinally()
    {
        var fx = NewMySql();

        fx.Unit.GetAuctionRecord(12, out TAuctionRecord _);

        var sm = fx.Db.For("Auction_QueryOneItem");
        Assert.Equal(12, Convert.ToInt32(sm.LastBinds[0].Value));
        Assert.Equal(2, sm.ResetCount);
    }

    /// <summary>★ MySQL 方言（原文 1040）：Query 假 → False，不取行。</summary>
    [Fact]
    public void GetAuctionRecord_QueryReturnsFalse_ReturnsFalse()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryOneItem").QueryResult = false;

        Assert.False(fx.Unit.GetAuctionRecord(1, out TAuctionRecord _));
        Assert.Equal(0, fx.Db.For("Auction_QueryOneItem").FetchCount);
    }

    /// <summary>MySqlAuctionDB.pas:1063-1067 —— 抛错 → 裸消息 + False。</summary>
    [Fact]
    public void GetAuctionRecord_QueryThrows_LogsAndReturnsFalse()
    {
        var fx = NewMySqlThrowing("Auction_QueryOneItem", new InvalidOperationException("record-boom"));

        bool ok = fx.Unit.GetAuctionRecord(1, out TAuctionRecord rec);

        Assert.False(ok);
        Assert.Equal(0, rec.AuctionID);
        Assert.Equal(new[] { "record-boom" }, fx.Log.Messages.ToArray());
    }

    // ==================================================================
    // 21. HumanRename —— 原文 1544-1581（★ 3 个 Reset 在 try 之外）
    // ==================================================================

    /// <summary>MySqlAuctionDB.pas:1552/1566 —— 一个事务里改三张表，每条语句 Step 一次（不判返回值）。</summary>
    [Fact]
    public void HumanRename_UpdatesAllThreeTablesInOneTransaction()
    {
        var fx = NewMySql();

        bool ok = fx.Unit.HumanRename("Old", "New");

        Assert.True(ok);
        Assert.Equal(new[] { "begin", "commit" }, fx.Db.Transactions.ToArray());
        Assert.Equal(1, fx.Db.For("Auction_HumanRename").StepCount);
        Assert.Equal(1, fx.Db.For("Auction_LastBidderRename").StepCount);
        Assert.Equal(1, fx.Db.For("Auction_AttentionRename").StepCount);
    }

    /// <summary>MySqlAuctionDB.pas:1554-1564 —— 每条都是 OrderBindParamText(NewName) 然后 OrderBindParamText(OldName)。</summary>
    [Fact]
    public void HumanRename_BindsNewNameThenOldNameForEachStatement()
    {
        var fx = NewMySql();

        fx.Unit.HumanRename("Old", "New");

        foreach (string label in new[] { "Auction_HumanRename", "Auction_LastBidderRename", "Auction_AttentionRename" })
        {
            var b = fx.Db.For(label).LastBinds;
            Assert.Equal(new[] { "text", "text" }, b.Select(x => x.Kind).ToArray());
            Assert.Equal("New", b[0].Text);
            Assert.Equal("Old", b[1].Text);
        }
    }

    /// <summary>★ 原文缺陷（1547-1549）：3 个 Reset 在 try **之外**；加上 1577-1579 的 finally 3 次
    /// → 每条 2 次（异常时也一样）。</summary>
    [Fact]
    public void HumanRename_ResetsEachStatementTwice()
    {
        var fx = NewMySql();

        fx.Unit.HumanRename("Old", "New");

        Assert.Equal(2, fx.Db.For("Auction_HumanRename").ResetCount);
        Assert.Equal(2, fx.Db.For("Auction_LastBidderRename").ResetCount);
        Assert.Equal(2, fx.Db.For("Auction_AttentionRename").ResetCount);
    }

    /// <summary>MySqlAuctionDB.pas:1569-1574 —— Step 抛错 → 裸消息 + RollBack + False，finally 仍全部 Reset。</summary>
    [Fact]
    public void HumanRename_StepThrows_RollsBackAndReturnsFalse_ButStillResets()
    {
        var fx = NewMySqlThrowing("Auction_LastBidderRename", new InvalidOperationException("rename-boom"));

        bool ok = fx.Unit.HumanRename("Old", "New");

        Assert.False(ok);
        Assert.Equal(new[] { "rename-boom" }, fx.Log.Messages.ToArray());
        Assert.Equal(new[] { "begin", "rollback" }, fx.Db.Transactions.ToArray());
        // 前置 1547-1549 已经跑过（异常发生在 try 内），finally 1577-1579 再跑 → 每条 2 次。
        Assert.Equal(2, fx.Db.For("Auction_HumanRename").ResetCount);
        Assert.Equal(2, fx.Db.For("Auction_LastBidderRename").ResetCount);
        Assert.Equal(2, fx.Db.For("Auction_AttentionRename").ResetCount);
    }

    /// <summary>MySqlAuctionDB.pas:1554-1564 —— 注入串走绑定。</summary>
    [Fact]
    public void HumanRename_SqlInjectionInNames_AreBound()
    {
        var fx = NewMySql();

        fx.Unit.HumanRename(Injection, Injection);

        var b = fx.Db.For("Auction_HumanRename").LastBinds;
        Assert.Equal(Injection, b[0].Text);
        Assert.Equal(Injection, b[1].Text);
        Assert.DoesNotContain(Injection, fx.Db.For("Auction_HumanRename").Sql);
    }

    /// <summary>MySqlAuctionDB.pas:1554-1564 —— 空名字照样绑定并 Commit。</summary>
    [Fact]
    public void HumanRename_EmptyNames_AreBoundAsEmptyText()
    {
        var fx = NewMySql();

        Assert.True(fx.Unit.HumanRename("", ""));
        var b = fx.Db.For("Auction_HumanRename").LastBinds;
        Assert.Equal("", b[0].Text);
        Assert.Equal("", b[1].Text);
        Assert.Equal(new[] { "begin", "commit" }, fx.Db.Transactions.ToArray());
    }

    // ==================================================================
    // 22. Run —— 原文 1269-1542（★ 1524-1525 空 except；1271-1390 IncPlayerGameMoney）
    // ==================================================================

    /// <summary>MySqlAuctionDB.pas:1400-1413 —— 先更新流拍状态，再查到期成交列表。</summary>
    [Fact]
    public void Run_UpdatesExpiredAuctionsFirst_ThenQueriesTheSuccesses()
    {
        var fx = NewMySql();

        fx.Unit.Run();

        var fail = fx.Db.For("Auction_UpdateAuctionItemFail");
        Assert.Equal(1, fail.StepCount);        // 原文 1403 不判返回值
        Assert.Equal(2, fail.ResetCount);       // 1402 + 1405 finally
        var query = fx.Db.For("Auction_QueryAuctionItemSuccess");
        Assert.Equal(1, query.QueryCount);
        Assert.Equal(1, query.FetchCount);      // 原文 1411-1413
    }

    /// <summary>MySqlAuctionDB.pas:1413-1530 —— 无行 → while 不进入、不更新状态、不开事务。</summary>
    [Fact]
    public void Run_NoSuccessfulRows_DoesNothingElse()
    {
        var fx = NewMySql();

        fx.Unit.Run();

        Assert.Equal(0, fx.Db.For("Auction_UpdateAuctionItemSuccess").StepCount);
        Assert.Empty(fx.Db.Transactions);
        Assert.Equal(2, fx.Db.For("Auction_QueryAuctionItemSuccess").ResetCount);   // 1409 + 1533 finally
    }

    /// <summary>★ MySQL 方言（原文 1411）：<c>if Query then while Fetch</c> —— Query 假时连 Fetch 都不调。</summary>
    [Fact]
    public void Run_QueryReturnsFalse_SkipsTheWholeLoop()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryAuctionItemSuccess").QueryResult = false;

        fx.Unit.Run();

        Assert.Equal(0, fx.Db.For("Auction_QueryAuctionItemSuccess").FetchCount);
        Assert.Equal(0, fx.Db.For("Auction_UpdateAuctionItemSuccess").StepCount);
    }

    /// <summary>MySqlAuctionDB.pas:1448-1449/1527-1529 —— 给拍卖者加钱（扣税）并把状态更新为 2。</summary>
    [Fact]
    public void Run_OneSuccessfulRow_PaysTheSellerAndMarksItTradingStatusTwo()
    {
        var fx = NewMySql();
        fx.Env.dwAuctionGameGoldTaxRate = 10;
        fx.Db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(5, "Seller", 0, 100, 500, "Bidder", 1000, 12, 34));
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "Sword", DBName = "SwordDb" };
        var player = new FakeAuctionPlayer { m_nGameGold = 0 };
        AuctionDbRunSeam.GetPlayObject = _ => player;

        try
        {
            fx.Unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        // 原文 1448：LastBidPrice - Round(LastBidPrice / 100 * nAuctionTaxRate) = 1000 - 100 = 900
        Assert.Equal(900, player.m_nGameGold);
        Assert.Equal(1, player.GameGoldChangedCount);
        var upd = fx.Db.For("Auction_UpdateAuctionItemSuccess");
        Assert.Equal(1, upd.StepCount);
        Assert.Equal(5, Convert.ToInt32(upd.LastBinds[0].Value));
    }

    /// <summary>MySqlAuctionDB.pas:1448 —— Delphi <c>Round</c> 是银行家舍入（12.5 → 12）。</summary>
    [Fact]
    public void Run_RoundUsesBankersRounding_LikeDelphiRound()
    {
        var fx = NewMySql();
        fx.Env.dwAuctionGameGoldTaxRate = 5;    // 250 * 0.05 = 12.5
        fx.Db.For("Auction_QueryAuctionItemSuccess").AddRow(SuccessRow(1, "S", 0, 1, 2, "B", 250, 1, 1));
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "S" };
        var player = new FakeAuctionPlayer { m_nGameGold = 0 };
        AuctionDbRunSeam.GetPlayObject = _ => player;

        try
        {
            fx.Unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        Assert.Equal(238, player.m_nGameGold);   // 250 - 12（不是 250 - 13）
    }

    /// <summary>MySqlAuctionDB.pas:1301-1306 —— 金币走 g_Config.nHumanMaxGold 上限（其它货币走 High(LongWord)）。</summary>
    [Fact]
    public void Run_GoldCurrency_ClampsToHumanMaxGold()
    {
        var fx = NewMySql();
        fx.Env.nHumanMaxGold = 500;
        fx.Db.For("Auction_QueryAuctionItemSuccess").AddRow(SuccessRow(1, "Seller", 2, 100, 500, "Bidder", 1000, 1, 1));
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "S" };
        var player = new FakeAuctionPlayer { m_nGold = 0 };
        AuctionDbRunSeam.GetPlayObject = _ => player;

        try
        {
            fx.Unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        Assert.Equal(500, player.m_nGold);
        Assert.Equal(1, player.GoldChangedCount);
        Assert.Equal(0, player.GameGoldChangedCount);
    }

    /// <summary>★ 原文 1284/1293/1311/1320：<c>if nValue &gt; High(LongWord) then nValue := High(LongWord)</c>，
    /// 随后 <c>Player.m_nGameGold := nValue</c> 把 Int64 赋回 **Integer** 字段（截断）。
    /// 用 int.MaxValue（余额）+ int.MaxValue（Prices，税率 0）得 nValue = 4294967294 ——
    /// 恰好比 High(LongWord) = 4294967295 **小 1**，所以钳位分支**不可达**，直接截断成 -2
    /// （若钳位真的命中，结果会是 (int)4294967295 = -1，本用例据此区分）。</summary>
    [Fact]
    public void Run_HighLongWordClampIsOffByOne_AndTheValueTruncates()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(1, "S0", 0, 1, 2, "B0", int.MaxValue, 1, 1));   // 元宝
        fx.Db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(2, "S1", 1, 1, 2, "B1", int.MaxValue, 1, 1));   // 游戏点
        fx.Db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(3, "S2", 3, 1, 2, "B2", int.MaxValue, 1, 1));   // 金刚石
        fx.Db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(4, "S3", 4, 1, 2, "B3", int.MaxValue, 1, 1));   // 灵符
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "S" };
        var s0 = new FakeAuctionPlayer { m_nGameGold = int.MaxValue };
        var s1 = new FakeAuctionPlayer { m_nGamePoint = int.MaxValue };
        var s2 = new FakeAuctionPlayer { m_nGameDiamond = int.MaxValue };
        var s3 = new FakeAuctionPlayer { m_nGameGird = int.MaxValue };
        AuctionDbRunSeam.GetPlayObject = n => n switch
        {
            "S0" => s0, "S1" => s1, "S2" => s2, "S3" => s3, _ => null,
        };

        try
        {
            fx.Unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        Assert.Equal(-2, s0.m_nGameGold);            // unchecked((int)4294967294L)
        Assert.Equal(1, s0.GameGoldChangedCount);
        Assert.Equal(-2, s1.m_nGamePoint);
        Assert.Equal(1, s1.GameGoldChangedCount);
        Assert.Equal(-2, s2.m_nGameDiamond);
        Assert.Equal(1, s2.NewGamePointChangedCount);
        Assert.Equal(-2, s3.m_nGameGird);
        Assert.Equal(1, s3.NewGamePointChangedCount);
        Assert.Equal(4, fx.Db.For("Auction_UpdateAuctionItemSuccess").StepCount);
    }

    /// <summary>MySqlAuctionDB.pas:1327-1345 —— 玩家不在线 → DataEngine.HumanChangeGold 接缝。</summary>
    [Fact]
    public void Run_OfflinePlayer_GoesThroughHumanChangeGoldSeam()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(1, "Seller", 0, 100, 500, "Bidder", 100, 1, 1));
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "S" };
        AuctionDbRunSeam.GetPlayObject = _ => null;
        TDBChangeGoldType captured = default;
        string capturedName = "";
        int capturedPrices = -1;
        AuctionDbRunSeam.HumanChangeGold = (t, n, p) => { captured = t; capturedName = n; capturedPrices = p; };

        try
        {
            fx.Unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        Assert.Equal(TDBChangeGoldType.cgtGameGold, captured);
        Assert.Equal("Seller", capturedName);
        Assert.Equal(100, capturedPrices);
    }

    /// <summary>MySqlAuctionDB.pas:1330-1343 —— 货币类型 → TDBChangeGoldType 的 case 映射。</summary>
    [Theory]
    [InlineData(0, TDBChangeGoldType.cgtGameGold)]
    [InlineData(1, TDBChangeGoldType.cgtGamePoint)]
    [InlineData(2, TDBChangeGoldType.cgtGold)]
    [InlineData(3, TDBChangeGoldType.cgtGameDiamond)]
    [InlineData(4, TDBChangeGoldType.cgtGameGird)]
    public void Run_OfflinePlayer_MapsCurrencyTypeToGoldType(int currency, TDBChangeGoldType expected)
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(1, "Seller", currency, 1, 2, "Bidder", 10, 1, 1));
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "S" };
        AuctionDbRunSeam.GetPlayObject = _ => null;
        TDBChangeGoldType captured = default;
        AuctionDbRunSeam.HumanChangeGold = (t, _, _) => captured = t;

        try
        {
            fx.Unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        Assert.Equal(expected, captured);
    }

    /// <summary>MySqlAuctionDB.pas:1341-1342 —— 未知货币走 case 的 <c>else Exit</c>：只离开 IncPlayerGameMoney，
    /// 不影响 DoRun 的循环（原文 1527 的 UpdateAuctionItemSuccess 仍执行）。</summary>
    [Fact]
    public void Run_UnknownCurrency_OfflineBranchExitsBeforeHumanChangeGold()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(1, "Seller", 99, 100, 500, "Bidder", 100, 1, 1));
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "S" };
        AuctionDbRunSeam.GetPlayObject = _ => null;
        bool called = false;
        AuctionDbRunSeam.HumanChangeGold = (_, _, _) => called = true;

        try
        {
            fx.Unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        Assert.False(called);
        Assert.Equal(1, fx.Db.For("Auction_UpdateAuctionItemSuccess").StepCount);
    }

    /// <summary>MySqlAuctionDB.pas:1442-1449 —— StdItem = nil 时 ItemName := ''，日志仍写 "卖出物品: "。</summary>
    [Fact]
    public void Run_NullStdItem_StillPaysAndLogsWithEmptyName()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(1, "Seller", 0, 100, 500, "Bidder", 100, 1, 1));
        AuctionDbRunSeam.GetStdItem = _ => null;
        AuctionDbRunSeam.GetPlayObject = _ => null;
        string logAdd = null;
        AuctionDbRunSeam.AddGameDataLog = a => { if (a.Remark == "拍卖行-到期") logAdd ??= a.LogAdd; };
        AuctionDbRunSeam.GBoGameLogGameGold = true;

        try
        {
            fx.Unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        Assert.Equal("卖出物品: ", logAdd);
    }

    /// <summary>★ 原文缺陷（1524-1525）：NPC GotoLable 段是 <c>except end;</c>（空处理），
    /// 吞掉该段全部异常且不进日志，但循环继续走到 UpdateAuctionItemSuccess。</summary>
    [Fact]
    public void Run_NpcGotoLableSectionExceptionsAreSwallowedByTheEmptyExcept()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(1, "Seller", 0, 100, 500, "Bidder", 100, 1, 1));
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "S", NeedIdentify = 1 };
        AuctionDbRunSeam.GetPlayObject = _ => new FakeAuctionPlayer();
        AuctionDbRunSeam.GotoLable = (_, _) => throw new InvalidOperationException("goto-boom");

        try
        {
            fx.Unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        Assert.DoesNotContain(fx.Log.Messages, m => m.Contains("TAuctionDB:Run"));
        Assert.DoesNotContain("goto-boom", fx.Log.Messages);
        Assert.Equal(1, fx.Db.For("Auction_UpdateAuctionItemSuccess").StepCount);
    }

    /// <summary>MySqlAuctionDB.pas:1536-1540 —— 外层 except 捕获第一条语句的异常并写裸消息，不再往下走。</summary>
    [Fact]
    public void Run_OuterException_IsCaughtByTheOriginalExceptAndLogged()
    {
        var fx = NewMySqlThrowing("Auction_UpdateAuctionItemFail", new InvalidOperationException("fail-update-boom"));

        fx.Unit.Run();

        Assert.Equal(new[] { "fail-update-boom" }, fx.Log.Messages.ToArray());
        Assert.Equal(0, fx.Db.For("Auction_QueryAuctionItemSuccess").QueryCount);
    }

    /// <summary>DbBases.cs:1027-1038 —— Run 有 try/except 但**不加锁**；RunTick2 只被更新、不参与节流。</summary>
    [Fact]
    public void Run_PublicWrapperDoesNotLock_AndRecordsTheTick()
    {
        var fx = NewMySql();

        Assert.Equal(0u, fx.Unit.RunTick2);   // Isolate() 把 GetTickCount 固定为 0
        fx.Unit.Run();

        Assert.Equal(0u, fx.Unit.RunTick2);
        Assert.Equal(0, fx.Host.LockCount);
        Assert.Equal(0, fx.Host.UnLockCount);
    }

    /// <summary>MySqlAuctionDB.pas:1448-1538 —— 两行都处理（AuctionID 逐行绑定）。</summary>
    [Fact]
    public void Run_TwoRows_ProcessesBoth()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryAuctionItemSuccess").AddRow(SuccessRow(1, "S1", 0, 1, 2, "B1", 10, 1, 1));
        fx.Db.For("Auction_QueryAuctionItemSuccess").AddRow(SuccessRow(2, "S2", 0, 1, 2, "B2", 20, 2, 2));
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "S" };
        AuctionDbRunSeam.GetPlayObject = _ => null;

        try
        {
            fx.Unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        var upd = fx.Db.For("Auction_UpdateAuctionItemSuccess");
        Assert.Equal(2, upd.StepCount);
        Assert.Equal(2, Convert.ToInt32(upd.LastBinds[0].Value));
    }

    /// <summary>MySqlAuctionDB.pas:1479/1512 —— 分别对拍卖者/竞拍者 GotoLable 且随后清空临时字段。</summary>
    [Fact]
    public void Run_GotoLableSellThenBuyLabelForSellerAndBidder()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(1, "Seller", 0, 10, 20, "Bidder", 30, 1, 1));
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "S", NeedIdentify = 1 };
        var seller = new FakeAuctionPlayer();
        var bidder = new FakeAuctionPlayer();
        AuctionDbRunSeam.GetPlayObject = n => n == "Seller" ? seller : bidder;
        var labels = new List<string>();
        AuctionDbRunSeam.GotoLable = (_, label) => labels.Add(label);

        try
        {
            fx.Unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        Assert.Equal(new[] { "@AuctionSellItem", "@AuctionBuyItem" }, labels.ToArray());
        Assert.Equal("", seller.m_sAuctionItemName);
        Assert.Equal(0, seller.m_nAuctionItemFinaPrice);
        Assert.False(seller.m_boAuctionItemSelled);
        Assert.Equal("", bidder.m_sAuctionItemHumanName);
        Assert.Equal(0, bidder.m_nAuctionItemStartPrice);
        Assert.False(bidder.m_boAuctionItemSelled);
    }

    /// <summary>MySqlAuctionDB.pas:1464-1510 —— GotoLable 之前玩家上的 9 个拍卖字段被填满。</summary>
    [Fact]
    public void Run_SellerFieldsArePopulatedBeforeGotoLable()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(1, "Seller", 4, 11, 22, "Bidder", 33, 1, 1));
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "SwordName", NeedIdentify = 0 };
        var seller = new FakeAuctionPlayer();
        AuctionDbRunSeam.GetPlayObject = n => n == "Seller" ? seller : null;
        var snapshot = new List<string>();
        AuctionDbRunSeam.GotoLable = (p, _) => snapshot.Add(
            $"{p.m_sAuctionItemName}|{p.m_sAuctionItemHumanName}|{p.m_sAuctionItemBidHumanName}|" +
            $"{p.m_nAuctionItemStartPrice}|{p.m_nAuctionItemSellPrice}|{p.m_nAuctionItemFinaPrice}|" +
            $"{p.m_nAuctionItemInvalidPrice}|{p.m_nAuctionItemMoneyType}|{p.m_boAuctionItemSelled}|{p.m_nScriptGotoCount}");

        try
        {
            fx.Unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        Assert.Equal("SwordName|Seller|Bidder|11|22|33|0|4|True|0", snapshot.Single());
    }

    /// <summary>MySqlAuctionDB.pas:1451-1457 —— NeedIdentify = 1 时写 LOG_ItemSell(10) / LOG_ItemBuy(11)。</summary>
    [Fact]
    public void Run_NeedIdentifyOne_WritesSellAndBuyItemLogs()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(1, "Seller", 0, 10, 20, "Bidder", 30, 1, 1));
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "屠龙", NeedIdentify = 1 };
        AuctionDbRunSeam.GetPlayObject = _ => null;
        var args = new List<AuctionGameDataLogArgs>();
        AuctionDbRunSeam.AddGameDataLog = a => args.Add(a);

        try
        {
            fx.Unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        var sell = args.Single(a => a.LogType == AuctionLogTypes.LOG_ItemSell);
        var buy = args.Single(a => a.LogType == AuctionLogTypes.LOG_ItemBuy);
        Assert.Equal("屠龙", sell.Name);
        Assert.Equal(1, sell.MakeIndex);
        Assert.Equal("Seller", sell.HumanName);
        Assert.Equal("拍卖行-到期", sell.Remark);
        Assert.Equal("买入:Bidder", sell.LogAdd);
        Assert.Equal("屠龙", buy.Name);
        Assert.Equal("Bidder", buy.HumanName);
        Assert.Equal("待取回, 卖出:Seller", buy.LogAdd);
    }

    /// <summary>MySqlAuctionDB.pas:1451 —— NeedIdentify &lt;&gt; 1 时跳过两条物品日志，但 NPC 流程照走。</summary>
    [Fact]
    public void Run_NeedIdentifyNotOne_SkipsTheItemLogsButStillRunsTheNpcFlow()
    {
        var fx = NewMySql();
        fx.Db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(1, "Seller", 0, 10, 20, "Bidder", 30, 1, 1));
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "S", NeedIdentify = 0 };
        var player = new FakeAuctionPlayer();
        AuctionDbRunSeam.GetPlayObject = _ => player;
        var logTypes = new List<int>();
        AuctionDbRunSeam.AddGameDataLog = a => logTypes.Add(a.LogType);

        try
        {
            fx.Unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        Assert.DoesNotContain(AuctionLogTypes.LOG_ItemSell, logTypes);
        Assert.DoesNotContain(AuctionLogTypes.LOG_ItemBuy, logTypes);
        Assert.Equal(0, player.m_nScriptGotoCount);
    }

    // ==================================================================
    // 23. 双方言对账（TMySqlAuctionDB ↔ TSqliteAuctionDB）
    //     —— 同一组输入分别驱动两个姊妹实现，逐字段 / 逐绑定比对。
    //     「差异面恰好是那 8 条时间写法」已由 DbLayerAuctionSqlFidelityTests 在**语句文本**层覆盖；
    //     本组补**行为**层：唯一允许不同的只有 AddDateTime 的**取值来源**
    //     （SQLite 行填 Unix 秒 → DelphiDateUtil.UnixToDateTime；MySQL 行填同一个 DateTime）。
    // ==================================================================

    private sealed class SqliteFixture
    {
        public TSqliteAuctionDB Unit = null!;
        public FakeSqliteDatabase Db = null!;
        public FakeDbLayerHost Host = null!;
        public HookSqliteDatabase Hook = null!;
    }

    private sealed class BothFixture
    {
        public MySqlFixture My = null!;
        public SqliteFixture Sq = null!;
        public FakeDbLayerEnvironment Env = null!;
        public CapturingDbLayerLog Log = null!;
    }

    /// <summary>同一 Isolate 下同时建两个方言的库（两边各自的 host 独立，锁计数分开断言）。</summary>
    private static BothFixture NewBoth(Dictionary<string, Exception>? myThrows = null,
        Dictionary<string, Exception>? sqThrows = null)
    {
        var (env, log) = DbLayerTestKit.Isolate();

        var myDb = new FakeMySqlDatabase();
        if (myThrows != null) foreach (string l in myThrows.Keys) myDb.AddSQLStatement(l);
        foreach (string l in TemporaryStatementLabels) myDb.AddSQLStatement(l);
        var myHook = new HookMySqlDatabase(myDb, myThrows);
        var myHost = new FakeDbLayerHost { DataBase = myDb };
        var myUnit = new TMySqlAuctionDB(myHost, myHook);
        myUnit.DoInit();

        var sqDb = new FakeSqliteDatabase();
        if (sqThrows != null) foreach (string l in sqThrows.Keys) sqDb.AddSQLStatement(l);
        foreach (string l in TemporaryStatementLabels) sqDb.AddSQLStatement(l);
        var sqHook = new HookSqliteDatabase(sqDb, sqThrows);
        var sqHost = new FakeDbLayerHost { DataBase = sqDb };
        var sqUnit = new TSqliteAuctionDB(sqHost, sqHook);
        sqUnit.DoInit();

        return new BothFixture
        {
            Env = env,
            Log = log,
            My = new MySqlFixture { Unit = myUnit, Db = myDb, Host = myHost, Env = env, Log = log, Hook = myHook },
            Sq = new SqliteFixture { Unit = sqUnit, Db = sqDb, Host = sqHost, Hook = sqHook },
        };
    }

    /// <summary>逐字段比对两条记录（AddDateTime 也在内 —— 对账的核心断言之一）。</summary>
    private static void AssertSameRecord(TAuctionRecord sq, TAuctionRecord my)
    {
        Assert.Equal(sq.AuctionID, my.AuctionID);
        Assert.Equal(sq.HumanName, my.HumanName);
        Assert.Equal(sq.AddDateTime, my.AddDateTime);
        Assert.Equal(sq.AuctionTime, my.AuctionTime);
        Assert.Equal(sq.TimeLeft, my.TimeLeft);
        Assert.Equal(sq.StartingPrice, my.StartingPrice);
        Assert.Equal(sq.SellingPrice, my.SellingPrice);
        Assert.Equal(sq.CurrencyType, my.CurrencyType);
        Assert.Equal(sq.LastBidPrice, my.LastBidPrice);
        Assert.Equal(sq.LastBidder, my.LastBidder);
        Assert.Equal(sq.TradingStatus, my.TradingStatus);
        Assert.Equal(sq.IsItemGive, my.IsItemGive);
        Assert.Equal(sq.IsAttention, my.IsAttention);
    }

    /// <summary>逐条记录比对（记录数 + 每个字段）。</summary>
    private static void AssertSameRecords(TAuctionItemList sq, TAuctionItemList my)
    {
        Assert.Equal(sq.Count, my.Count);
        for (int i = 0; i < sq.Count; i++) AssertSameRecord(sq[i], my[i]);
    }

    /// <summary>绑定参数序列（Kind + Text）比对。</summary>
    private static void AssertSameBinds(ScriptedStatementBase sq, ScriptedStatementBase my)
    {
        Assert.Equal(sq.LastBinds.Select(b => b.Kind).ToArray(), my.LastBinds.Select(b => b.Kind).ToArray());
        Assert.Equal(sq.LastBinds.Select(b => b.Text).ToArray(), my.LastBinds.Select(b => b.Text).ToArray());
    }

    /// <summary>对账①：QueryAllItems —— 13 个字段逐字段一致、绑定序列一致；
    /// 唯一不同的是 AddDateTime 的取值来源（Unix 秒 vs DATETIME），结果相等。</summary>
    [Fact]
    public void BothDialects_QueryAllItems_ProduceIdenticalRecordsAndBinds()
    {
        var b = NewBoth();
        const long unix0 = 1709251200L;                       // 2024-03-01 08:00:00 UTC
        DateTime t0 = DelphiDateUtil.UnixToDateTime(unix0);
        DateTime t1 = DelphiDateUtil.UnixToDateTime(unix0 + 60);

        b.Sq.Db.For("Auction_QueryAllItems").AddRow(
            AllItemsRow(11, "Alice", unix0, 24, 3600, 100, 500, 2, 150, "Bob", 0, 0, 1));
        b.Sq.Db.For("Auction_QueryAllItems").AddRow(
            AllItemsRow(12, "Carol", unix0 + 60, 12, -5, 200, 900, 1, 0, "", 1, 1, 0));
        b.My.Db.For("Auction_QueryAllItems").AddRow(
            AllItemsRow(11, "Alice", t0, 24, 3600, 100, 500, 2, 150, "Bob", 0, 0, 1));
        b.My.Db.For("Auction_QueryAllItems").AddRow(
            AllItemsRow(12, "Carol", t1, 12, -5, 200, 900, 1, 0, "", 1, 1, 0));

        var sqList = new TAuctionItemList();
        var myList = new TAuctionItemList();
        int nSq = b.Sq.Unit.QueryAllItems("", IgAll, "Alice", 1, -1, 0, 0, true, 0, 0, 0, sqList);
        int nMy = b.My.Unit.QueryAllItems("", IgAll, "Alice", 1, -1, 0, 0, true, 0, 0, 0, myList);

        Assert.Equal(nSq, nMy);
        Assert.Equal(2, nMy);
        AssertSameRecords(sqList, myList);
        AssertSameBinds(b.Sq.Db.For("Auction_QueryAllItems"), b.My.Db.For("Auction_QueryAllItems"));
        // 唯一的方言差异面：SQLite 从整数列取 Unix 秒再换算；MySQL 直接取 DATETIME。
        Assert.Equal(DelphiDateUtil.UnixToDateTime(unix0), sqList[0].AddDateTime);
        Assert.Equal(t0, myList[0].AddDateTime);
        Assert.Equal(sqList[0].AddDateTime, myList[0].AddDateTime);
        // 两侧的 Owner.LoadItemFromDB 调用序列也一致。
        Assert.Equal(b.Sq.Host.LoadedItems.ToArray(), b.My.Host.LoadedItems.ToArray());
    }

    /// <summary>对账②：QueryMyItems —— 记录字段（无 IsAttention 列）与绑定序列一致。</summary>
    [Fact]
    public void BothDialects_QueryMyItems_ProduceIdenticalRecordsAndBinds()
    {
        var b = NewBoth();
        const long unix = 1600000000L;
        DateTime t = DelphiDateUtil.UnixToDateTime(unix);

        b.Sq.Db.For("Auction_QueryAuctionItems").AddRow(
            MyItemsRow(21, "Alice", unix, 24, 100, 10, 20, 0, 15, "Dave", 0, 0));
        b.Sq.Db.For("Auction_QueryAuctionItems").AddRow(
            MyItemsRow(22, "Alice", unix, 48, -100, 30, 40, 1, 0, "", 2, 1));
        b.My.Db.For("Auction_QueryAuctionItems").AddRow(
            MyItemsRow(21, "Alice", t, 24, 100, 10, 20, 0, 15, "Dave", 0, 0));
        b.My.Db.For("Auction_QueryAuctionItems").AddRow(
            MyItemsRow(22, "Alice", t, 48, -100, 30, 40, 1, 0, "", 2, 1));

        var sqList = new TAuctionItemList();
        var myList = new TAuctionItemList();
        int nSq = b.Sq.Unit.QueryMyItems("Alice", 2, sqList);
        int nMy = b.My.Unit.QueryMyItems("Alice", 2, myList);

        Assert.Equal(nSq, nMy);
        AssertSameRecords(sqList, myList);
        AssertSameBinds(b.Sq.Db.For("Auction_QueryAuctionItems"), b.My.Db.For("Auction_QueryAuctionItems"));
        Assert.Equal(sqList[0].AddDateTime, myList[0].AddDateTime);
    }

    /// <summary>对账③：QueryMyAttentionItems —— 多行时两侧内层语句的绑定与记录逐字段一致。</summary>
    [Fact]
    public void BothDialects_QueryMyAttentionItems_ProduceIdenticalRecordsAndBinds()
    {
        var b = NewBoth();
        const long unix = 1500000000L;
        DateTime t = DelphiDateUtil.UnixToDateTime(unix);

        foreach (int id in new[] { 61, 62 })
        {
            b.Sq.Db.For("Auction_QueryAttentionItems").AddRow(id);
            b.My.Db.For("Auction_QueryAttentionItems").AddRow(id);
        }

        b.Sq.Db.For("Auction_QueryOneItem").AddRow(
            OneItemRow("Seller", unix, 6, 60, 7, 8, 3, 9, "Bidder", 0, 0));
        b.My.Db.For("Auction_QueryOneItem").AddRow(
            OneItemRow("Seller", t, 6, 60, 7, 8, 3, 9, "Bidder", 0, 0));

        var sqList = new TAuctionItemList();
        var myList = new TAuctionItemList();
        int nSq = b.Sq.Unit.QueryMyAttentionItems("Alice", 1, sqList);
        int nMy = b.My.Unit.QueryMyAttentionItems("Alice", 1, myList);

        Assert.Equal(nSq, nMy);
        AssertSameRecords(sqList, myList);
        Assert.Equal(new[] { 61, 62 }, myList.Snapshot().Select(r => r.AuctionID).ToArray());
        AssertSameBinds(b.Sq.Db.For("Auction_QueryAttentionItems"), b.My.Db.For("Auction_QueryAttentionItems"));
        AssertSameBinds(b.Sq.Db.For("Auction_QueryOneItem"), b.My.Db.For("Auction_QueryOneItem"));
        Assert.Equal(sqList[1].AddDateTime, myList[1].AddDateTime);
    }

    /// <summary>对账④：GetAuctionInfo / GetAuctionRecord —— 同样一行输入，两侧字段与绑定一致。</summary>
    [Fact]
    public void BothDialects_GetAuctionInfoAndRecord_MatchFieldByField()
    {
        var b = NewBoth();
        const long unix = 1400000000L;
        DateTime t = DelphiDateUtil.UnixToDateTime(unix);
        object[] SqRow() => OneItemRow("Seller", unix, 6, 60, 7, 8, 3, 9, "Bidder", 2, 1);
        object[] MyRow() => OneItemRow("Seller", t, 6, 60, 7, 8, 3, 9, "Bidder", 2, 1);

        b.Sq.Db.For("Auction_QueryOneItem").AddRow(SqRow());
        b.My.Db.For("Auction_QueryOneItem").AddRow(MyRow());
        b.Sq.Unit.GetAuctionInfo(77, out TAuctionInfo sqInfo);
        b.My.Unit.GetAuctionInfo(77, out TAuctionInfo myInfo);

        Assert.Equal(sqInfo.AuctionID, myInfo.AuctionID);
        Assert.Equal(sqInfo.HumanName, myInfo.HumanName);
        Assert.Equal(sqInfo.AddDateTime, myInfo.AddDateTime);
        Assert.Equal(sqInfo.AuctionTime, myInfo.AuctionTime);
        Assert.Equal(sqInfo.TimeLeft, myInfo.TimeLeft);
        Assert.Equal(sqInfo.StartingPrice, myInfo.StartingPrice);
        Assert.Equal(sqInfo.SellingPrice, myInfo.SellingPrice);
        Assert.Equal(sqInfo.CurrencyType, myInfo.CurrencyType);
        Assert.Equal(sqInfo.LastBidPrice, myInfo.LastBidPrice);
        Assert.Equal(sqInfo.LastBidder, myInfo.LastBidder);
        Assert.Equal(sqInfo.TradingStatus, myInfo.TradingStatus);
        Assert.Equal(sqInfo.IsItemGive, myInfo.IsItemGive);

        b.Sq.Db.For("Auction_QueryOneItem").Reset();
        b.My.Db.For("Auction_QueryOneItem").Reset();
        b.Sq.Db.For("Auction_QueryOneItem").AddRow(SqRow());
        b.My.Db.For("Auction_QueryOneItem").AddRow(MyRow());
        bool okSq = b.Sq.Unit.GetAuctionRecord(78, out TAuctionRecord sqRec);
        bool okMy = b.My.Unit.GetAuctionRecord(78, out TAuctionRecord myRec);

        Assert.Equal(okSq, okMy);
        AssertSameRecord(sqRec, myRec);
        Assert.Equal(b.Sq.Host.LoadedItems.ToArray(), b.My.Host.LoadedItems.ToArray());
    }

    /// <summary>对账⑤：nPage &lt;= 0 时 offset = (nPage-1)*7（可为负）—— 两侧逐值一致且都不夹取。</summary>
    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-3)]
    public void BothDialects_OutOfRangePage_ComputeTheSameOffset(int nPage)
    {
        var b = NewBoth();

        b.Sq.Unit.QueryAllItems("", IgAll, "H", nPage, -1, 0, 0, true, 0, 0, 0, new TAuctionItemList());
        b.My.Unit.QueryAllItems("", IgAll, "H", nPage, -1, 0, 0, true, 0, 0, 0, new TAuctionItemList());

        int sqOffset = Convert.ToInt32(b.Sq.Db.For("Auction_QueryAllItems").LastBinds[2].Value);
        int myOffset = Convert.ToInt32(b.My.Db.For("Auction_QueryAllItems").LastBinds[2].Value);
        Assert.Equal(sqOffset, myOffset);
        Assert.Equal(unchecked((nPage - 1) * 7), myOffset);

        b.Sq.Unit.QueryMyItems("H", nPage, new TAuctionItemList());
        b.My.Unit.QueryMyItems("H", nPage, new TAuctionItemList());
        Assert.Equal(Convert.ToInt32(b.Sq.Db.For("Auction_QueryAuctionItems").LastBinds[2].Value),
            Convert.ToInt32(b.My.Db.For("Auction_QueryAuctionItems").LastBinds[2].Value));
    }

    /// <summary>对账⑥：sortField 越界 / itemColors = 0 / 价格上下限为 0 —— 两侧都不加对应子句。</summary>
    [Fact]
    public void BothDialects_OutOfRangeSortAndZeroFilters_AddNoClausesOnEitherSide()
    {
        var b = NewBoth();

        b.Sq.Unit.QueryAllItems("", IgAll, "H", 1, -1, 0, 99, true, 0, 0, 0, new TAuctionItemList());
        b.My.Unit.QueryAllItems("", IgAll, "H", 1, -1, 0, 99, true, 0, 0, 0, new TAuctionItemList());
        foreach (string sql in new[]
                 {
                     b.Sq.Db.For("Auction_QueryAllItems").Sql, b.My.Db.For("Auction_QueryAllItems").Sql,
                 })
        {
            Assert.DoesNotContain(" order by ", sql);
            Assert.DoesNotContain("ItemColor in", sql);
            Assert.DoesNotContain("SellingPrice >=", sql);
            Assert.DoesNotContain("SellingPrice <=", sql);
            Assert.EndsWith(" limit ? offset ?;", sql);
        }

        b.Sq.Unit.GetAllItemsPageCount("", IgAll, 0, 0, 0, 0);
        b.My.Unit.GetAllItemsPageCount("", IgAll, 0, 0, 0, 0);
        foreach (string sql in new[]
                 {
                     b.Sq.Db.For("Auction_GetAllItemsCount").Sql, b.My.Db.For("Auction_GetAllItemsCount").Sql,
                 })
        {
            Assert.DoesNotContain("ItemColor in", sql);
            Assert.DoesNotContain("SellingPrice", sql);
            Assert.DoesNotContain("ItemGroup =", sql);
        }
    }

    /// <summary>对账⑦：minPrices &gt; maxPrices 时包装层交换 —— 两侧拼出的上下限子句一致。</summary>
    [Fact]
    public void BothDialects_MinMaxSwap_ProducesTheSameClauses()
    {
        var b = NewBoth();

        b.Sq.Unit.GetAllItemsPageCount("", IgAll, 0, 0, 999, 11);
        b.My.Unit.GetAllItemsPageCount("", IgAll, 0, 0, 999, 11);

        foreach (string sql in new[]
                 {
                     b.Sq.Db.For("Auction_GetAllItemsCount").Sql, b.My.Db.For("Auction_GetAllItemsCount").Sql,
                 })
        {
            Assert.Contains(" and SellingPrice >= 11", sql);
            Assert.Contains(" and SellingPrice <= 999", sql);
        }
    }

    /// <summary>对账⑧：GetMyItemsPageCount 的「无 except、只有 finally」——
    /// 两侧异常都**穿透**到包装层，日志前缀同为 <c>TAuctionDB:GetMyItemsPageCount</c>，返回值同为 0，锁各一次。</summary>
    [Fact]
    public void BothDialects_GetMyItemsPageCount_ThrowEscapesToTheWrapperOnBothSides()
    {
        var boom = new InvalidOperationException("my-count-boom");
        var b = NewBoth(myThrows: new Dictionary<string, Exception> { ["Auction_GetMyItemsCount"] = boom },
            sqThrows: new Dictionary<string, Exception> { ["Auction_GetMyItemsCount"] = boom });

        int nSq = b.Sq.Unit.GetMyItemsPageCount("H");
        int nMy = b.My.Unit.GetMyItemsPageCount("H");

        Assert.Equal(nSq, nMy);
        Assert.Equal(0, nMy);
        // 两次都带包装层前缀（[Exception] TAuctionDB:GetMyItemsPageCount;）——这是"内层没有 except"的判据。
        Assert.Equal(2, b.Log.Messages.Count);
        Assert.All(b.Log.Messages, m => Assert.Contains("[Exception] TAuctionDB:GetMyItemsPageCount", m));
        Assert.All(b.Log.Messages, m => Assert.Contains("my-count-boom", m));
        // finally 仍然 Reset（前置 Reset + finally Reset = 2 次，两侧一致）。
        Assert.Equal(2, b.Sq.Db.For("Auction_GetMyItemsCount").ResetCount);
        Assert.Equal(2, b.My.Db.For("Auction_GetMyItemsCount").ResetCount);
        // 各自的 host 各锁一次（两个 host 分开断言）。
        Assert.Equal(1, b.Sq.Host.LockCount);
        Assert.Equal(1, b.My.Host.LockCount);
        Assert.Equal(1, b.Sq.Host.UnLockCount);
        Assert.Equal(1, b.My.Host.UnLockCount);
    }

    /// <summary>对账⑨：有自身 except 的方法（QueryMyItems）—— 两侧都写**裸消息**（不带
    /// <c>[Exception] TAuctionDB:</c> 前缀），返回值同为 0。</summary>
    [Fact]
    public void BothDialects_MethodsWithInnerExcept_LogBareMessagesOnBothSides()
    {
        var boom = new InvalidOperationException("my-items-boom");
        var b = NewBoth(myThrows: new Dictionary<string, Exception> { ["Auction_QueryAuctionItems"] = boom },
            sqThrows: new Dictionary<string, Exception> { ["Auction_QueryAuctionItems"] = boom });

        int nSq = b.Sq.Unit.QueryMyItems("H", 1, new TAuctionItemList());
        int nMy = b.My.Unit.QueryMyItems("H", 1, new TAuctionItemList());

        Assert.Equal(nSq, nMy);
        Assert.Equal(0, nMy);
        Assert.Equal(2, b.Log.Messages.Count);
        Assert.All(b.Log.Messages, m => Assert.Equal("my-items-boom", m));   // 裸消息：无包装层前缀
    }

    /// <summary>对账⑩：关注物品的"已关注"判据 —— MySQL 用 <c>RowCount != 0</c>、SQLite 用 <c>Step() == ROW</c>，
    /// 但**行为**一致：已关注 → 跳过插入且返回 false；未关注 → 插入且返回 true。</summary>
    [Fact]
    public void BothDialects_AttentionExistenceSemantics_MatchBehaviourally()
    {
        var b = NewBoth();

        // 已关注：SQLite 给一行（Step 返回 ROW），MySQL 给 RowCount = 1。
        b.Sq.Db.For("Auction_CheckInAttentionItem").AddRow(1);
        b.My.Db.For("Auction_CheckInAttentionItem").RowCount = 1;
        bool sqAlready = b.Sq.Unit.AddAttentionItem("H", 1);
        bool myAlready = b.My.Unit.AddAttentionItem("H", 1);

        Assert.Equal(sqAlready, myAlready);
        Assert.False(myAlready);
        Assert.Equal(0, b.Sq.Db.For("Auction_AddAttentionItem").StepCount);
        Assert.Equal(0, b.My.Db.For("Auction_AddAttentionItem").StepCount);

        // 未关注：SQLite 无行（Step 返回 DONE），MySQL RowCount = 0。
        var b2 = NewBoth();
        b2.My.Db.For("Auction_CheckInAttentionItem").RowCount = 0;
        bool sqFresh = b2.Sq.Unit.AddAttentionItem("H", 1);
        bool myFresh = b2.My.Unit.AddAttentionItem("H", 1);

        Assert.Equal(sqFresh, myFresh);
        Assert.True(myFresh);
        Assert.Equal(1, b2.Sq.Db.For("Auction_AddAttentionItem").StepCount);
        Assert.Equal(1, b2.My.Db.For("Auction_AddAttentionItem").StepCount);
    }

    /// <summary>对账⑪：AddAuctionItem —— 绑定序列、事务序列、SaveItemToDB 与返回值两侧一致
    /// （SQLite 认 Step in [OK,DONE]，MySQL 认 Step 返回 true）。</summary>
    [Fact]
    public void BothDialects_AddAuctionItem_MatchBindsTransactionsAndOutcome()
    {
        var b = NewBoth();
        b.Sq.Db.For("Auction_GetMaxAuctionID").AddRow(77);
        b.My.Db.For("Auction_GetMaxAuctionID").AddRow(77);
        var std = new FakeAuctionStdItem { StdMode = 5, Color = 9, DBName = "Sword" };

        int idSq = b.Sq.Unit.AddAuctionItem("Alice", 24, 100, 500, 2, MakeUserItem(0, "", 0), std);
        int idMy = b.My.Unit.AddAuctionItem("Alice", 24, 100, 500, 2, MakeUserItem(0, "", 0), std);

        Assert.Equal(idSq, idMy);
        Assert.Equal(77, idMy);
        AssertSameBinds(b.Sq.Db.For("Auction_InsertAuctionItem"), b.My.Db.For("Auction_InsertAuctionItem"));
        Assert.Equal(b.Sq.Db.Transactions.ToArray(), b.My.Db.Transactions.ToArray());
        Assert.Equal(new[] { "begin", "commit" }, b.My.Db.Transactions.ToArray());
        Assert.Equal(b.Sq.Host.SavedItems.ToArray(), b.My.Host.SavedItems.ToArray());
    }

    /// <summary>对账⑫：DeleteAuctionItem —— 两侧同一事务序列；MySQL 独有 ClearResult（原文 617），
    /// SQLite 侧没有这个概念（Execute 即脚本执行）。</summary>
    [Fact]
    public void BothDialects_DeleteAuctionItem_ShareTheTransactionShape_MySqlAlsoClearsTheResult()
    {
        var b = NewBoth();

        bool okSq = b.Sq.Unit.DeleteAuctionItem("Alice", 42);
        bool okMy = b.My.Unit.DeleteAuctionItem("Alice", 42);

        Assert.Equal(okSq, okMy);
        Assert.True(okMy);
        Assert.Equal(b.Sq.Db.Transactions.ToArray(), b.My.Db.Transactions.ToArray());
        Assert.Equal(new[] { "begin", "commit" }, b.My.Db.Transactions.ToArray());
        foreach (string sql in new[] { b.Sq.Db.ExecutedSql.Single(), b.My.Db.ExecutedSql.Single() })
        {
            Assert.Contains(" WHERE ParentID = 42 and ItemType = 5 and ItemIndex = 0;", sql);
            Assert.Contains("DELETE FROM AuctionAttention WHERE AuctionID = 42;", sql);
            Assert.Contains("DELETE FROM AuctionData WHERE AuctionID = 42;", sql);
        }

        Assert.Equal(1, b.My.Hook.ClearResultCount);   // ★ MySQL 独有
    }
}